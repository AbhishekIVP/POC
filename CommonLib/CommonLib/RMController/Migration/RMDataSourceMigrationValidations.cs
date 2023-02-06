using com.ivp.rad.common;
using com.ivp.rad.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.ivp.common;
using com.ivp.rad.dal;
using com.ivp.commom.commondal;
using com.ivp.srmcommon;
using System.Data;
using com.ivp.srm.vendorapi;
using System.Collections;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace com.ivp.common.Migration
{
    public partial class RMDataSourceMigrationSync
    {
        //static IRLogger mLogger = RLogFactory.CreateLogger("RMDataSourceMigrationValidations");

        const string COL_REMARKS = "Remarks";
        const string HARD_ERROR = "IsHard";

        private void FailEntityMappingGroup(List<ObjectRow> entityTypeRows, string errorMessage)
        {
            entityTypeRows.ForEach(row =>
            {
                new RMCommonMigration().SetFailedRow(row, new List<string>() { errorMessage }, false);
            });
        }


        private int ValidateEntityTypeMappingDetails(List<ObjectRow> entityTypeRows, Dictionary<string, RMEntityDetailsInfo> moduleEntityTypeDetails,
           Dictionary<string, RMEntityDetailsInfo> allEntityTypeDetails, Dictionary<string, SecurityTypeMasterInfo> secTypeDetails, Dictionary<string, AttrInfo> secTypeCommonAttributes, string feedName, Dictionary<string, RMFeedInfoDetailed> dictFeedInfo, ref bool isFailed, ref int mappedEntityTypeID, ref string entityTypeDisplayName)
        {
            List<string> errors = new List<string>();
            int entityTypeID = -1;
            List<string> lstPrimaryFields = null;
            bool isLegFeed = false;
            int mappedLegID = -1;
            bool primaryMapped = false;
            bool b1Raised = false;
            bool b2Raised = false;
            bool b3Raised = false;

            try
            {
                RMFeedInfoDetailed feedInfo = dictFeedInfo.Where(d => d.Key.SRMEqualWithIgnoreCase(feedName)).Select(d => d.Value).FirstOrDefault();

                if (feedInfo != null)
                {
                    lstPrimaryFields = feedInfo.FeedFields.Where(f => f.IsPrimary).Select(f => f.FieldName).ToList();
                }

                List<string> mappedLegs = new List<string>();
                bool isMasterMapped = false;

                entityTypeRows.ForEach(row =>
                {
                    string legName = Convert.ToString(row[RMModelerConstants.Leg_Name]).Trim();
                    if (string.IsNullOrEmpty(legName))
                        isMasterMapped = true;
                    else if (!mappedLegs.SRMContainsWithIgnoreCase(legName))
                        mappedLegs.Add(legName);
                });

                foreach (ObjectRow row in entityTypeRows)
                {
                    bool isLookupValid = true;
                    bool isLegRow = false;
                    string entityTypeName = Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME]).Trim();
                    string legName = Convert.ToString(row[RMModelerConstants.Leg_Name]).Trim();
                    string attributeName = Convert.ToString(row[RMModelerConstants.Attribute_Name]).Trim();
                    string lookupType = Convert.ToString(row[RMModelerConstants.Lookup_Type]).Trim();
                    string lookupAttribute = Convert.ToString(row[RMModelerConstants.Lookup_Attribute]).Trim();
                    string fieldName = Convert.ToString(row[RMDataSourceConstants.Feed_Field_Name]).Trim();
                    bool isPrimaryMapping = lstPrimaryFields != null && lstPrimaryFields.SRMContainsWithIgnoreCase(fieldName);
                    bool isInsert = GetBooleanValue(Convert.ToString(row[RMDataSourceConstants.Is_Insert]).Trim());
                    bool isUpdate = GetBooleanValue(Convert.ToString(row[RMDataSourceConstants.Is_Update]).Trim());
                    int legID = -1;
                    int attributeID = -1;
                    RMEntityDetailsInfo mappedInfo = null;
                    RMEntityDetailsInfo entityTypeInfo = null;
                    RMEntityAttributeInfo attributeInfo = null;
                    entityTypeDisplayName = entityTypeName;


                    entityTypeInfo = moduleEntityTypeDetails.Where(e => e.Key.SRMEqualWithIgnoreCase(entityTypeName)).Select(e => e.Value).FirstOrDefault();

                    if (!isInsert && !isUpdate)
                    {
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INSERT_UPDATE_BOTH_FALSE }, false);
                        errors.Add(RMCommonConstants.INSERT_UPDATE_BOTH_FALSE);
                    }

                    if (entityTypeInfo != null)
                    {
                        entityTypeID = entityTypeInfo.EntityTypeID;
                        if (!string.IsNullOrEmpty(legName))
                        {
                            isLegRow = true;
                            mappedInfo = entityTypeInfo.Legs.Where(l => l.Key.SRMEqualWithIgnoreCase(legName) && l.Value.Attributes.Any(x => x.Value.IsPrimary)).Select(l => l.Value).FirstOrDefault();
                        }
                        else
                        {
                            mappedInfo = entityTypeInfo;
                        }

                        if (mappedInfo != null)
                        {
                            if (isLegRow)
                                legID = mappedInfo.EntityTypeID;

                            if (isPrimaryMapping)
                            {
                                primaryMapped = true;
                                isLegFeed = isLegRow;

                                if (mappedEntityTypeID > 0 && mappedInfo.EntityTypeID != mappedEntityTypeID)
                                {
                                    new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_PRIMARY_FIELD_MAPPING }, false);
                                    errors.Add(RMCommonConstants.INVALID_PRIMARY_FIELD_MAPPING);
                                }
                                else
                                    mappedEntityTypeID = mappedInfo.EntityTypeID;
                            }

                            if (!b1Raised && primaryMapped && !isLegFeed && mappedLegs.Count > 0)
                            {
                                b1Raised = true;
                                FailEntityMappingGroup(entityTypeRows, RMCommonConstants.LEG_ATTRIBUTE_IN_MASTER_FEED);
                                errors.Add(RMCommonConstants.LEG_ATTRIBUTE_IN_MASTER_FEED);
                            }
                            else if (!b2Raised && primaryMapped && isLegFeed && !isMasterMapped)
                            {
                                b2Raised = true;
                                FailEntityMappingGroup(entityTypeRows, RMCommonConstants.NO_MASTER_IN_LEG_FEED);
                                errors.Add(RMCommonConstants.NO_MASTER_IN_LEG_FEED);
                            }
                            else if (!b3Raised && mappedLegs.Count > 1)
                            {
                                b3Raised = true;
                                FailEntityMappingGroup(entityTypeRows, RMCommonConstants.MORE_THAN_ONE_LEG_IN_FEED);
                                errors.Add(RMCommonConstants.MORE_THAN_ONE_LEG_IN_FEED);
                            }
                            else if (isLegFeed)
                                mappedLegID = mappedInfo.EntityTypeID;

                            attributeInfo = mappedInfo.Attributes.Where(a => a.Key.SRMEqualWithIgnoreCase(attributeName)).Select(a => a.Value).FirstOrDefault();

                            if (attributeInfo != null)
                            {
                                attributeID = attributeInfo.EntityAttributeID;
                                RMDBDataTypes dataType = attributeInfo.DataType;

                                row[RMModelerConstants.ENTITY_ATTRIBUTE_ID] = attributeID;

                                if (dataType == RMDBDataTypes.DATETIME && string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Date_Format])))
                                {
                                    new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.EMPTY_DATE_FORMAT }, false);
                                    errors.Add(RMCommonConstants.EMPTY_DATE_FORMAT);
                                }

                                if (dataType == RMDBDataTypes.LOOKUP || dataType == RMDBDataTypes.SECURITY_LOOKUP)
                                {
                                    if (!attributeInfo.LookupEntityTypeName.SRMEqualWithIgnoreCase(lookupType))
                                        isLookupValid = false;
                                    else
                                    {
                                        #region Ref Lookup

                                        if (dataType == RMDBDataTypes.LOOKUP)
                                        {
                                            RMEntityDetailsInfo lookupEntityTypeInfo = allEntityTypeDetails.Where(e => e.Key.SRMEqualWithIgnoreCase(lookupType)).Select(e => e.Value).FirstOrDefault();

                                            if (lookupEntityTypeInfo != null)
                                            {
                                                RMEntityAttributeInfo lookupAttributeInfo = lookupEntityTypeInfo.Attributes
                                                .Where(a => a.Key.SRMEqualWithIgnoreCase(lookupAttribute)).Select(a => a.Value).FirstOrDefault();

                                                if (lookupAttributeInfo != null || lookupAttribute.SRMEqualWithIgnoreCase("Entity Code"))
                                                {
                                                    if (lookupAttributeInfo != null)
                                                    {
                                                        row[RMModelerConstants.Lookup_Type_Id] = lookupEntityTypeInfo.EntityTypeID;
                                                        row[RMModelerConstants.Lookup_Attribute_Id] = lookupAttributeInfo.EntityAttributeID;
                                                        row[RMModelerConstants.Lookup_Attribute_Real_Name] = lookupAttributeInfo.AttributeName;
                                                        row[RMModelerConstants.Attribute_lookup_identity] = attributeInfo.AttributeLookupIdentityColumn;
                                                        row[RMDataSourceConstants.Lookup_Type_Ref_Sec] = 0;
                                                    }
                                                    else
                                                    {
                                                        row[RMModelerConstants.Lookup_Type_Id] = lookupEntityTypeInfo.EntityTypeID;
                                                        row[RMModelerConstants.Lookup_Attribute_Id] = 0;
                                                        row[RMModelerConstants.Lookup_Attribute_Real_Name] = "entity_code";
                                                        row[RMModelerConstants.Attribute_lookup_identity] = attributeInfo.AttributeLookupIdentityColumn;
                                                        row[RMDataSourceConstants.Lookup_Type_Ref_Sec] = 0;
                                                    }
                                                }
                                                else
                                                    isLookupValid = false;
                                            }
                                            else
                                                isLookupValid = false;

                                        }
                                        #endregion

                                        #region Sec Lookup

                                        else if (dataType == RMDBDataTypes.SECURITY_LOOKUP)
                                        {
                                            SecurityTypeMasterInfo secTypeInfo = null;
                                            int secTypeID = -1;
                                            AttrInfo secTypeAttributeInfo = null;
                                            if (lookupType.SRMEqualWithIgnoreCase("All Security Types"))
                                            {
                                                secTypeAttributeInfo = secTypeCommonAttributes.Where(s => s.Key.SRMEqualWithIgnoreCase(lookupAttribute))
                                                .Select(a => a.Value).FirstOrDefault();
                                                secTypeID = 0;
                                            }
                                            else
                                            {
                                                secTypeInfo = secTypeDetails.Where(s => s.Key.SRMEqualWithIgnoreCase(lookupType))
                                                .Select(s => s.Value).FirstOrDefault();

                                                if (secTypeInfo != null)
                                                {
                                                    secTypeID = secTypeInfo.SectypeId;
                                                    secTypeAttributeInfo = secTypeInfo.AttributeInfo.MasterAttrs
                                                      .Where(a => a.Key.SRMEqualWithIgnoreCase(lookupAttribute))
                                                      .Select(a => a.Value).FirstOrDefault();
                                                }
                                                else
                                                    isLookupValid = false;
                                            }

                                            if (secTypeAttributeInfo != null && secTypeID >= 0)
                                            {
                                                row[RMModelerConstants.Lookup_Type_Id] = secTypeID;
                                                row[RMModelerConstants.Lookup_Attribute_Id] = secTypeAttributeInfo.AttrId;
                                                row[RMModelerConstants.Lookup_Attribute_Real_Name] = secTypeAttributeInfo.AttrRealName;
                                                row[RMModelerConstants.Attribute_lookup_identity] = attributeInfo.AttributeLookupIdentityColumn;
                                                row[RMDataSourceConstants.Lookup_Type_Ref_Sec] = 1;
                                            }
                                            else
                                                isLookupValid = false;
                                        }

                                        #endregion
                                    }
                                }

                                if (!isLookupValid)
                                {
                                    new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.INVALID_LOOKUP_DETAILS }, false);
                                    errors.Add(RMCommonConstants.INVALID_LOOKUP_DETAILS);
                                }



                            }
                            else
                            {
                                new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.ENTITY_ATTRIBUTE_NOT_FOUND }, false);
                                errors.Add(RMCommonConstants.ENTITY_ATTRIBUTE_NOT_FOUND);
                            }
                        }
                        else
                        {
                            string errorMessage = string.Empty;
                            if (entityTypeInfo.Legs.Where(l => l.Key.SRMEqualWithIgnoreCase(legName)).Select(l => l.Value).FirstOrDefault() == null)
                                errorMessage = RMCommonConstants.ENTITY_TYPE_LEG_NOT_FOUND;
                            else
                                errorMessage = RMCommonConstants.ENTITY_TYPE_LEG_WITHOUT_PRIMARY;

                            new RMCommonMigration().SetFailedRow(row, new List<string>() { errorMessage }, false);
                            errors.Add(errorMessage);
                        }
                    }
                    else
                    {
                        new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.ENTITY_TYPE_NOT_FOUND }, false);
                        errors.Add(RMCommonConstants.ENTITY_TYPE_NOT_FOUND);
                    }

                }

                isFailed = (errors != null && errors.Count > 0);
                return entityTypeID; ;

            }
            catch
            {
                throw;
            }
        }

        private List<string> ValidateFeedFieldAttributeMapping(List<ObjectRow> rows, string feedName, Dictionary<string, RMFeedInfoDetailed> dictFeedInfo, ref bool isFailed)
        {
            List<string> errors = new List<string>();
            List<string> primaryField = new List<string>();
            List<string> primaryAttribute = new List<string>();
            List<string> legsMapped = new List<string>();
            bool primaryMapped = false;
            bool invalidPrimaryMapping = false;
            bool isLegFeed = false;
            string entityTypeMapped = string.Empty;

            try
            {
                RMFeedInfoDetailed feedInfo = dictFeedInfo.Where(d => d.Key.SRMEqualWithIgnoreCase(feedName)).Select(d => d.Value).FirstOrDefault();

                if (feedInfo != null)
                {
                    List<string> lstPrimaryFields = feedInfo.FeedFields.Where(f => f.IsPrimary).Select(f => f.FieldName).ToList();


                    if (lstPrimaryFields.Count <= 0)
                    {
                        mLogger.Debug("No primary");
                        isFailed = true;
                        FailDataSourceGroup(rows, new List<string>() { RMCommonConstants.NO_PRIMARY_FIELD }, false);
                    }
                    else
                    {
                        foreach (ObjectRow row in rows)
                        {
                            bool isLegRow = false;
                            entityTypeMapped = string.Empty;
                            entityTypeMapped = Convert.ToString(row[RMModelerConstants.ENTITY_DISPLAY_NAME]);
                            if (!string.IsNullOrEmpty(Convert.ToString(row[RMModelerConstants.Leg_Name])))
                            {
                                isLegRow = true;
                                isLegFeed = true;
                                legsMapped.Add(RMCommonStatic.ConvertToLower(row[RMModelerConstants.Leg_Name]));
                            }

                            RMFeedFieldInfo fieldInfo = feedInfo.FeedFields.Find(f => f.FieldName.SRMEqualWithIgnoreCase(Convert.ToString(row[RMDataSourceConstants.Feed_Field_Name])));

                            if (fieldInfo != null)
                            {
                                row[RMDataSourceConstants.Feed_Field_ID] = fieldInfo.FieldID;

                                if (fieldInfo.IsPrimary)
                                {
                                    primaryMapped = true;

                                    if (!isLegRow && isLegFeed)
                                        invalidPrimaryMapping = true;

                                    if (primaryAttribute.Any(p => p.SRMEqualWithIgnoreCase(entityTypeMapped) == false))
                                        invalidPrimaryMapping = true;
                                    else
                                        primaryAttribute.Add(entityTypeMapped);

                                    if (!primaryField.SRMContainsWithIgnoreCase(fieldInfo.FieldName))
                                    {
                                        primaryField.Add(fieldInfo.FieldName);
                                    }
                                }
                            }
                            else
                            {
                                isFailed = true;
                                new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.FEED_FIELD_INVALID }, false);
                            }
                        }

                        if (!primaryMapped)
                        {
                            isFailed = true;
                            FailDataSourceGroup(rows, new List<string>() { RMCommonConstants.PRIMARY_NOT_MAPPED }, false);
                        }
                        if (invalidPrimaryMapping || primaryField.Distinct().Count() != lstPrimaryFields.Count)
                        {
                            isFailed = true;
                            FailDataSourceGroup(rows, new List<string>() { RMCommonConstants.INVALID_PRIMARY_FIELD_MAPPING }, false);
                        }

                    }
                }
                else
                {
                    mLogger.Debug("Invalid Feed");
                    isFailed = true;
                    FailDataSourceGroup(rows, new List<string>() { RMCommonConstants.DS_FEED_NOT_PRESENT }, false);
                }
                return errors;

            }
            catch
            {
                throw;
            }
        }

        private List<string> ValidateFeedMapping(ObjectRow row)
        {
            List<string> errors = new List<string>();
            string dataSourceName = Convert.ToString(row[RMDataSourceConstants.Data_Source_Name]);
            string feedName = Convert.ToString(row[RMDataSourceConstants.Feed_Name]);
            string fieldName = Convert.ToString(row[RMDataSourceConstants.Primary_Column_Name]);

            if ((!processedDataSourceVsFeed.Keys.SRMContainsWithIgnoreCase(dataSourceName))
                        ||
                        (
                            processedDataSourceVsFeed.Keys.SRMContainsWithIgnoreCase(dataSourceName)
                            &&
                            !processedDataSourceVsFeed[dataSourceName].SRMContainsWithIgnoreCase(feedName)
                        ))
            {
                errors.Add(RMCommonConstants.DS_FEED_NOT_PRESENT);
            }

            else if ((!processedFeedVsFields.Keys.SRMContainsWithIgnoreCase(feedName))
                        ||
                        (
                            processedFeedVsFields.Keys.SRMContainsWithIgnoreCase(feedName)
                            &&
                            !processedFeedVsFields[feedName].SRMContainsWithIgnoreCase(fieldName)
                        ))
            {
                errors.Add(RMCommonConstants.FEED_MAP_PRIM_COL_NOT_PRESENT);
            }

            return errors;
        }

        private void ValidateDataSourceAndFeed(IEnumerable<ObjectRow> rows, bool setNotProcessed)
        {
            rows.Where(r => !Convert.ToString(r[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)
            && !Convert.ToString(r[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_NOT_PROCESSED))
            .ToList().ForEach(row =>
            {
                string dataSourceName = Convert.ToString(row[RMDataSourceConstants.Data_Source_Name]);
                string feedName = Convert.ToString(row[RMDataSourceConstants.Feed_Name]);

                if ((!processedDataSourceVsFeed.Keys.SRMContainsWithIgnoreCase(dataSourceName))
                            ||
                            (
                                processedDataSourceVsFeed.Keys.SRMContainsWithIgnoreCase(dataSourceName)
                                &&
                                !processedDataSourceVsFeed[dataSourceName].SRMContainsWithIgnoreCase(feedName)
                            ))
                {
                    new RMCommonMigration().SetFailedRow(row, new List<string> { RMCommonConstants.DS_FEED_NOT_PRESENT }, setNotProcessed);
                }
            });
        }

        private bool ValidateFeedRules(List<ObjectRow> feedRuleRows)
        {
            return true;
            bool isValid = true;

            var groups = feedRuleRows.GroupBy(r => new { dup = RMCommonStatic.ConvertToLower(r[RMModelerConstants.Priority]) });

            foreach (var group in groups)
            {
                if (group.ToList<ObjectRow>().Count > 1) //Duplicate rules (By Priority)
                {
                    isValid = false;
                    group.ToList<ObjectRow>().ForEach(dupRow =>
                    { new RMCommonMigration().SetFailedRow(dupRow, new List<string>() { RMCommonConstants.RULE_PRIORITY_DUPLICATE }, false); });
                }
            }

            groups = feedRuleRows.GroupBy(r => new { dup = RMCommonStatic.ConvertToLower(r[RMModelerConstants.Rule_Name]) });

            foreach (var group in groups)
            {
                if (group.ToList<ObjectRow>().Count > 1) //Duplicate rules (By Name)
                {
                    isValid = false;
                    group.ToList<ObjectRow>().ForEach(dupRow =>
                    { new RMCommonMigration().SetFailedRow(dupRow, new List<string>() { RMCommonConstants.RULE_NAME_DUPLICATE }, false); });
                }
            }

            return isValid;
        }

        private bool ValidateFeedEntityTypeRules(List<ObjectRow> feedRuleRows, Dictionary<string, RMEntityDetailsInfo> entityTypeDetails, int feedSummaryID, ref int mappingID, ref int entityTypeID, RDBConnectionManager mDBCon)
        {
            bool isValid = true;
            string entityTypeName = Convert.ToString(feedRuleRows[0][RMModelerConstants.ENTITY_DISPLAY_NAME]);
            entityTypeID = -1;
            mappingID = -1;

            if (!entityTypeDetails.Keys.SRMContainsWithIgnoreCase(entityTypeName))
            {
                isValid = false;
                FailDataSourceGroup(feedRuleRows, new List<string>() { RMCommonConstants.ENTITY_TYPE_NOT_FOUND }, false);
            }
            else
            {
                entityTypeID = entityTypeDetails[entityTypeName].EntityTypeID;
                mappingID = new RMDataSourceDBManager(mDBCon).GetEntityTypeFeedMappingID(feedSummaryID, entityTypeID);

                if (mappingID <= 0)
                {
                    isValid = false;
                    FailDataSourceGroup(feedRuleRows, new List<string>() { RMCommonConstants.ENTITY_TYPE_NOT_MAPPED }, false);
                }
            }


            return isValid;
        }

        private bool ValidateFeedFields(List<ObjectRow> feedFieldRows, List<ObjectRow> existingFeedFieldRows, int fileTypeID)
        {
            var groups = feedFieldRows.GroupBy(r => new { dup = RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Feed_Field_Name]) });
            List<string> errors = new List<string>();
            bool isValid = true;

            //check primary field present
            if (!feedFieldRows.Any(f => Convert.ToBoolean(f[RMModelerConstants.Is_Primary])))
            {
                isValid = false;
                feedFieldRows.ToList<ObjectRow>().ForEach(r =>
                { new RMCommonMigration().SetFailedRow(r, new List<string>() { RMCommonConstants.PRIMARY_FIELD_NOT_PRESENT }, false); });
            }

            //check primary fields changed (not allowed from UI)
            if (existingFeedFieldRows != null)
            {
                if (feedFieldRows.Any(f => existingFeedFieldRows
                .Any(e => Convert.ToString(e[RMDataSourceConstants.Feed_Field_Name]).SRMEqualWithIgnoreCase(Convert.ToString(f[RMDataSourceConstants.Feed_Field_Name])) &&
                  Convert.ToBoolean(e[RMModelerConstants.Is_Primary]) != Convert.ToBoolean(f[RMModelerConstants.Is_Primary])
                )))
                {
                    isValid = false;
                    feedFieldRows.ToList<ObjectRow>().ForEach(r =>
                    { new RMCommonMigration().SetFailedRow(r, new List<string>() { RMCommonConstants.PRIMARY_FIELD_CHANGED }, false); });
                }
            }

            foreach (var group in groups)
            {
                if (group.ToList<ObjectRow>().Count > 1) //Duplicate Fields (By Name)
                {
                    isValid = false;
                    group.ToList<ObjectRow>().ForEach(dupRow =>
                    { new RMCommonMigration().SetFailedRow(dupRow, new List<string>() { RMCommonConstants.FEED_FIELDS_DUPLICATE }, false); });
                }
            }
            groups = feedFieldRows.Where(fRow => !string.IsNullOrEmpty(Convert.ToString(fRow[RMDataSourceConstants.Position])))
                .GroupBy(r => new { dup = RMCommonStatic.ConvertToLower(r[RMDataSourceConstants.Position]) });

            foreach (var group in groups)
            {
                if (group.ToList<ObjectRow>().Count > 1 && fileTypeID != 4) //Duplicate Fields (By Position) - Not to be checked for load from db
                {
                    isValid = false;
                    group.ToList<ObjectRow>().ForEach(dupRow =>
                    { new RMCommonMigration().SetFailedRow(dupRow, new List<string>() { RMCommonConstants.FIELD_POSITION_DUPLICATE }, false); });
                }
            }

            feedFieldRows.ForEach(row =>
            {
                errors = new List<string>();
                if (fileTypeID == 0) //Fixed width
                {
                    errors.AddRange(ValidateStartEndIndex(row));
                    errors.AddRange(ValidateFieldPosition(row));
                }
                if (fileTypeID == 1) //Delimited
                {
                    errors.AddRange(ValidateFieldPosition(row));
                }
                if (fileTypeID == 2) //XML
                {
                    errors.AddRange(ValidateFieldXPath(row));
                }
                if (fileTypeID == 3) //Excel
                {
                    errors.AddRange(ValidateFieldPosition(row));
                }
                if (fileTypeID == 0)
                {
                    errors.AddRange(ValidateStartEndIndex(row));
                    errors.AddRange(ValidateFieldPosition(row));
                }
                if (Convert.ToInt32(row[RMDataSourceConstants.Position]) <= 0 && fileTypeID != 4)
                {
                    //new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.FIELD_POSITION_INVALID }, false);
                    errors.Add(RMCommonConstants.FIELD_POSITION_INVALID);
                }
                if (GetBooleanValue(Convert.ToString(row[RMModelerConstants.Is_PII])) && !GetBooleanValue(Convert.ToString(row[RMModelerConstants.Is_Encrypted])))
                {
                    //new RMCommonMigration().SetFailedRow(row, new List<string>() { RMCommonConstants.PII_NOT_ENCRYPTED }, false);
                    errors.Add(RMCommonConstants.PII_NOT_ENCRYPTED);
                }
                if (errors.Count > 0)
                {
                    new RMCommonMigration().SetFailedRow(row, errors, false);
                    isValid = false;
                }


            });

            return isValid;
        }

        private List<string> ValidateStartEndIndex(ObjectRow row)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(Convert.ToString(row[RMDataSourceConstants.Start_Index])))
                errors.Add(RMCommonConstants.START_INDEX_EMPTY);
            if (string.IsNullOrEmpty(Convert.ToString(row[RMDataSourceConstants.End_Index])))
                errors.Add(RMCommonConstants.END_INDEX_EMPTY);

            return errors;
        }

        private List<string> ValidateFieldPosition(ObjectRow row)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(Convert.ToString(row[RMDataSourceConstants.Position])))
                errors.Add(RMCommonConstants.FIELD_POSITION_EMPTY);

            return errors;
        }

        private List<string> ValidateFieldXPath(ObjectRow row)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(Convert.ToString(row[RMDataSourceConstants.X_Path])))
                errors.Add(RMCommonConstants.FIELD_XPATH_EMPTY);

            return errors;
        }

        private List<string> ValidateLoadFromDBFeed(ObjectRow row)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Server_Type])))
                errors.Add(RMCommonConstants.SERVER_TYPE_MISSING);

            if (string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Connection_String])))
                errors.Add(RMCommonConstants.CONNECTION_STRING_MISSING);

            if (string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Loading_Criteria])))
                errors.Add(RMCommonConstants.LOADING_CRITERIA_MISSING);

            return errors;
        }

        private List<string> ValidateFixedWidthFeed(ObjectRow row)
        {
            List<string> errors = new List<string>();
            Regex regex = new Regex(@"[A-Za-z0-9_]");

            if (string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Record_Delimiter]))
                && string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Record_Length])))
                errors.Add(RMCommonConstants.RECORD_DELIMITER_LENGTH_MISSING);

            if (!string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Comment_Char]))
                &&
                (regex.IsMatch(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Comment_Char]))
                || RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Comment_Char]).Length > 1
                ))
            {
                errors.Add(RMCommonConstants.INVALID_COMMENT_CHAR);
            }

            return errors;
        }

        private List<string> ValidateDelimitedFeed(ObjectRow row)
        {
            List<string> errors = new List<string>();
            Regex regex = new Regex(@"[A-Za-z0-9_]");

            if (string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Record_Delimiter])))
                errors.Add(RMCommonConstants.RECORD_DELIMITER_MISSING);

            if (string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Field_Delimiter])))
                errors.Add(RMCommonConstants.FIELD_DELIMITER_MISSING);

            if (!string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Comment_Char]))
                &&
                (regex.IsMatch(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Comment_Char]))
                || RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Comment_Char]).Length > 1
                ))
            {
                errors.Add(RMCommonConstants.INVALID_COMMENT_CHAR);
            }

            return errors;
        }

        private List<string> ValidateXMLFeed(ObjectRow row)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Root_XPath])))
                errors.Add(RMCommonConstants.ROOT_XPATH_MISSING);

            if (string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(row[RMDataSourceConstants.Record_XPath])))
                errors.Add(RMCommonConstants.RECORD_XPATH_MISSING);

            return errors;
        }
    }
}
