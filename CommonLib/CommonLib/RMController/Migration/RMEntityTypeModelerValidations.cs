using com.ivp.commom.commondal;
using com.ivp.rad.data;
using com.ivp.rad.transport;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace com.ivp.common.Migration
{
    //class RMEntityTypeModelerValidations
    //{
    //}

    public class RMEntityTypeSetupSync
    {
        //static IRLogger mLogger = RLogFactory.CreateLogger("RefMEntityModelerMigration");

        public List<string> validateEntityDisplayName(string displayName)
        {
            List<string> validationError = new List<string>();
            if (displayName == null || displayName.Trim() == string.Empty)
            {
                validationError.Add("Entity Type name cannot be blank");
            }

            if (!Regex.IsMatch(displayName, @"^[\w ]+$"))
            {
                validationError.Add("Entity type name can only contain alphanumeric, spaces and underscore characters.");
            }

            return validationError;
        }

        public List<string> validateLegName(string displayName)
        {
            List<string> validationError = new List<string>();
            if (displayName == null || displayName.Trim() == string.Empty)
            {
                validationError.Add("Leg name cannot be blank");
            }

            if (!Regex.IsMatch(displayName, @"^[\w ]+$"))
            {
                validationError.Add("Leg name can only contain alphanumeric, spaces and underscore characters.");
            }

            return validationError;
        }

        public List<string> validateTagName(string tagName)
        {
            List<string> validationError = new List<string>();
            if (!Regex.IsMatch(tagName, @"^[\w ,]+$") && !string.IsNullOrEmpty(tagName))
            {
                validationError.Add("Tags can only contain alphanumeric, spaces, underscore characters.");
            }

            return validationError;
        }

        public List<string> validateUniqueKeyName(string uniqueKey)
        {
            List<string> validationError = new List<string>();
            if (uniqueKey == null || uniqueKey.Trim() == string.Empty)
            {
                validationError.Add("Leg name cannot be blank");
            }

            if (!Regex.IsMatch(uniqueKey, @"^[\w ]+$"))
            {
                validationError.Add("Unique Key name can only contain alphanumeric, spaces and underscore characters.");
            }

            return validationError;
        }

        public List<string> validateUniqueKeyAttributes(string attributeName)
        {
            List<string> validationError = new List<string>();
            if (string.IsNullOrEmpty(attributeName))
                validationError.Add("Attribute names cannot be left blank.");
            else
            {
                List<string> attributeList = attributeName.Trim().Split(',').ToList();
                if (attributeList.GroupBy(attr => attr).Any(c => c.Count() > 1))
                    validationError.Add("Duplicate attribute names entered.");
            }
            return validationError;
        }

        //List<string> validate
    }
    public class RMAttributeSetupMigrationValidations
    {
        //ObjectTable dbMasterEntityTypes = null;
        ObjectTable dbMasterAttributes = null;
        ObjectTable dbLegAttributes = null;
        //ObjectSet dbEntityTypesDetails = null;
        //ObjectSet dbSecurityTypes = null;
        Dictionary<string, RMEntityDetailsInfo> entityDetailsInfo;
        Dictionary<string, SecurityTypeMasterInfo> secTypeDetailsInfo;
        Dictionary<string, RMEntityDetailsInfo> allModulesEntityDetailsInfo;
        Dictionary<string, AttrInfo> commonAttributes;

        public RMAttributeSetupMigrationValidations(ObjectSet dbEntityTypesDetails, Dictionary<string, RMEntityDetailsInfo> entityDetails, Dictionary<string, SecurityTypeMasterInfo> secTypeDetails, Dictionary<string, AttrInfo> commonAttributes, Dictionary<string, RMEntityDetailsInfo> allModulesEntityDetailsInfo)
        {
            entityDetailsInfo = entityDetails;
            secTypeDetailsInfo = secTypeDetails;
            //dbMasterEntityTypes = dbEntityTypesDetails.Tables["Definition"];
            dbMasterAttributes = dbEntityTypesDetails.Tables["Master Attributes"];
            dbLegAttributes = dbEntityTypesDetails.Tables["Leg Attributes"];
            this.commonAttributes = commonAttributes;
            this.allModulesEntityDetailsInfo = allModulesEntityDetailsInfo;
        }

        public List<string> ValidateAttributeName(string entityTypeName, string attributeName, string dataType, bool isInsert, string attributeRealName)
        {
            List<string> errorMsgs = new List<string>();
            bool duplicateAttributeName = false;
            int attributeFoundCount = 1;
            if (isInsert)
                attributeFoundCount = 0;

            if (!new Regex(RMAttributeSetupConstants.ALPHA_NUMERIC_REGEX).IsMatch(attributeName))
                errorMsgs.Add(RMAttributeSetupConstants.INVALID_ATTRIBUTE_NAME);

            if (RMAttributeSetupConstants.RESTRICTED_ATTRIBUTE_NAMES.SRMContainsWithIgnoreCase(attributeName))
                errorMsgs.Add(RMAttributeSetupConstants.ATTRIBUTE_NAME_RESTICTED_ERROR);

            if (dbMasterAttributes.Rows.AsEnumerable().Where(x => Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entityTypeName) && (Convert.ToString(x[RMModelerConstants.Attribute_Name]).SRMEqualWithIgnoreCase(attributeName)
                || Convert.ToString(x[RMModelerConstants.Attribute_Real_Name]).SRMEqualWithIgnoreCase(attributeRealName))).Count() > attributeFoundCount)
                duplicateAttributeName = true;

            if (!duplicateAttributeName)
            {
                //if (dbLegAttributes.Rows.AsEnumerable().Where(x => Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entityTypeName) && Convert.ToString(x["Attribute Name"]).SRMEqualWithIgnoreCase(attributeName)).Count() > attributeFoundCount)
                if (dbLegAttributes.Rows.AsEnumerable().Where(x => Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(entityTypeName) && (Convert.ToString(x[RMModelerConstants.Attribute_Name]).SRMEqualWithIgnoreCase(attributeName)
                || Convert.ToString(x[RMModelerConstants.Attribute_Real_Name]).SRMEqualWithIgnoreCase(attributeRealName))).Count() > attributeFoundCount)
                    duplicateAttributeName = true;
            }
            if (duplicateAttributeName)
                errorMsgs.Add(RMAttributeSetupConstants.ATTRIBUTE_NAME_NOT_UNIQUE_ERROR);

            return errorMsgs;
        }

        public List<string> ValidateAttributeDisplay(string dataType, string showEntityCode, string orderByAttr, string allowComma, string allowPercent, string allowMultiplier)
        {
            List<string> errorMsgs = new List<string>();
            if(dataType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.DATA_TYPE_INT) || dataType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.DATA_TYPE_DECIMAL))
            {
                if (!string.IsNullOrEmpty(Convert.ToString(showEntityCode)))
                    errorMsgs.Add(RMAttributeSetupConstants.INVALID_SHOW_ENTITY_CODE);
                if(!string.IsNullOrEmpty(orderByAttr))
                    errorMsgs.Add(RMAttributeSetupConstants.INVALID_ORDER_BY_ATTRIBUTE);
                if (Convert.ToBoolean(allowPercent) && Convert.ToBoolean(allowMultiplier))
                    errorMsgs.Add("Both Allow Percent and Allow Comma columns cannot be true.");
            }
            else if (dataType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.DATA_TYPE_LOOKUP))
            {
                if (!string.IsNullOrEmpty(Convert.ToString(allowComma)))
                    errorMsgs.Add(RMAttributeSetupConstants.INVALID_ALLOW_COMMA);
                if (!string.IsNullOrEmpty(Convert.ToString(allowPercent)))
                    errorMsgs.Add(RMAttributeSetupConstants.INVALID_APPEND_PERCENTAGE);
                if (!string.IsNullOrEmpty(Convert.ToString(allowMultiplier)))
                    errorMsgs.Add(RMAttributeSetupConstants.INVALID_APPEND_MULTIPLIER);
            }
            else if (dataType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.DATA_TYPE_SECURITY_LOOKUP))
            {
                if (!string.IsNullOrEmpty(Convert.ToString(allowComma)))
                    errorMsgs.Add(RMAttributeSetupConstants.INVALID_ALLOW_COMMA);
                if (!string.IsNullOrEmpty(Convert.ToString(allowPercent)))
                    errorMsgs.Add(RMAttributeSetupConstants.INVALID_APPEND_PERCENTAGE);
                if (!string.IsNullOrEmpty(Convert.ToString(allowMultiplier)))
                    errorMsgs.Add(RMAttributeSetupConstants.INVALID_APPEND_MULTIPLIER);
                if (!string.IsNullOrEmpty(orderByAttr))
                    errorMsgs.Add(RMAttributeSetupConstants.INVALID_ORDER_BY_ATTRIBUTE);
            }
            else
            {
                if (!string.IsNullOrEmpty(Convert.ToString(allowComma)) || !string.IsNullOrEmpty(Convert.ToString(allowPercent)) || !string.IsNullOrEmpty(Convert.ToString(allowMultiplier)) || !string.IsNullOrEmpty(orderByAttr) || !string.IsNullOrEmpty(Convert.ToString(showEntityCode)))
                    errorMsgs.Add("Invalid values in display configuration columns.");
            }

            return errorMsgs;
        }

        public List<string> ValidateEncryptedAttributes(string dataType)
        {
            List<string> errorMsgs = new List<string>();

            if (dataType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.DATA_TYPE_LOOKUP) || dataType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.DATA_TYPE_SECURITYLOOKUP)
                                     || dataType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.DATA_TYPE_SECURITY_LOOKUP) || dataType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.DATA_TYPE_FILE))
                errorMsgs.Add(RMAttributeSetupConstants.ATTRIBUTE_CANNOT_BE_ENCRYPTED);

            return errorMsgs;
        }

        public List<string> ValidateAttributeDataTypeLength(string dataType, string dataTypeLength)
        {
            List<string> errorMsgs = new List<string>();
            switch (dataType.ToUpper())
            {
                case RMAttributeSetupConstants.DATA_TYPE_VARCHAR:
                    int length = -1;
                    if (!string.IsNullOrEmpty(dataTypeLength))
                    {
                        if (!Int32.TryParse(dataTypeLength, out length))
                            errorMsgs.Add(RMAttributeSetupConstants.DATA_LENGTH_INVALID_ERROR);

                        if (!new Regex(RMAttributeSetupConstants.NUMERIC_REGEX).IsMatch(dataTypeLength))
                            errorMsgs.Add(RMAttributeSetupConstants.DATA_LENGTH_INVALID_ERROR);

                        if (Int32.TryParse(dataTypeLength, out length) && length > Convert.ToInt32(RMAttributeSetupConstants.DATA_TYPE_VARCHAR_MAX_LENGTH))
                            errorMsgs.Add(RMAttributeSetupConstants.DATA_LENGTH_INVALID_ERROR);
                    }
                    else
                        errorMsgs.Add(RMAttributeSetupConstants.DATA_LENGTH_EMPTY_ERROR);
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_DECIMAL:
                    if (!string.IsNullOrEmpty(dataTypeLength))
                    {

                        if (!dataTypeLength.Contains("."))
                            errorMsgs.Add(RMAttributeSetupConstants.DATA_LENGTH_INVALID_ERROR);

                        string[] values = dataTypeLength.Split('.');

                        if (!new Regex(RMAttributeSetupConstants.NUMERIC_REGEX).IsMatch(values[0]))
                            errorMsgs.Add(RMAttributeSetupConstants.DATA_LENGTH_INVALID_ERROR);

                        if (!new Regex(RMAttributeSetupConstants.NUMERIC_REGEX).IsMatch(values[1]))
                            errorMsgs.Add(RMAttributeSetupConstants.DATA_LENGTH_INVALID_ERROR);

                        int numberPart = Convert.ToInt32(values[0]);
                        int decimalPart = Convert.ToInt32(values[1]);
                        int totalLength = numberPart + decimalPart;

                        if (numberPart < 0 || decimalPart < 0 || totalLength < 0)
                            errorMsgs.Add(RMAttributeSetupConstants.DATA_LENGTH_INVALID_DECIMAL_ERROR);

                        if (totalLength > 28 || totalLength < 1)
                            errorMsgs.Add("The number part cannot be " + ((totalLength > 28) ? "greater than 28" : "less than 1"));

                    }
                    else
                        errorMsgs.Add(RMAttributeSetupConstants.DATA_LENGTH_EMPTY_ERROR);
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_INT:
                case RMAttributeSetupConstants.DATA_TYPE_LOOKUP:
                case RMAttributeSetupConstants.DATA_TYPE_SECURITYLOOKUP:
                case RMAttributeSetupConstants.DATA_TYPE_SECURITY_LOOKUP:
                case RMAttributeSetupConstants.DATA_TYPE_FILE:
                case RMAttributeSetupConstants.DATA_TYPE_BIT:
                case RMAttributeSetupConstants.DATA_TYPE_DATETIME:
                case RMAttributeSetupConstants.DATA_TYPE_VARCHAR_MAX:
                    if (!string.IsNullOrEmpty(dataTypeLength))
                        errorMsgs.Add("Length cannot be entered for this data type.");
                    break;

            }
            return errorMsgs;
        }
        public List<string> ValidateAttributeDataLength(int attributeId, string newDataLength)
        {
            List<string> errorMsgs = new List<string>();

            string maxLength = new RMModelerDBManager().GetMaxAttributeDataLength(attributeId);
            string newDecimalLength = "-1";
            if (newDataLength.Split('.').Length == 2)
                newDecimalLength = newDataLength.Split('.')[1];
            newDataLength = newDataLength.Split('.')[0];

            if (maxLength != "-1")
            {
                if (maxLength.Split('.').Length == 1)
                {
                    if (Convert.ToInt32(newDataLength) < Convert.ToInt32(maxLength))
                        errorMsgs.Add("Data Type Length cannot be less than " + maxLength);
                }
                else if (maxLength.Split('.').Length == 2)
                {
                    if ((Convert.ToInt32(newDataLength) < Convert.ToInt32(maxLength.Split('.')[0])) || (Convert.ToInt32(newDecimalLength) < Convert.ToInt32(maxLength.Split('.')[1])))
                        errorMsgs.Add("Data Type Length cannot be less than " + maxLength);
                }
            }
            return errorMsgs;
        }

        public List<string> ValidateRestrictedChars(string attributeName, string restrictedChars)
        {
            List<string> errorMsgs = new List<string>();

            List<char> restrictedCharList = new List<char>();
            restrictedCharList = restrictedChars.Trim().ToCharArray().ToList();

            if (restrictedCharList.Distinct().Count() != restrictedCharList.Count())
                errorMsgs.Add(RMAttributeSetupConstants.RESTRICTED_CHARS_DUPLICATE);
            return errorMsgs;
        }

        public List<string> ValidateMandatory(string entityTypeName, string attributeName)
        {
            List<string> errorMsgs = new List<string>();

            bool containsNullData = false;
            string query = @"DECLARE @entityTypeDisplayName VARCHAR(MAX) = '" + entityTypeName + @"', @attributeDisplayName VARCHAR(MAX) = '" + attributeName + @"',@entityTypeName VARCHAR(MAX) = '', @attributeName VARCHAR(MAX) = '',@entityTypeID INT = 0

                        SELECT @entityTypeName = entity_type_name, @entityTypeID = entity_type_id FROM IVPRefMaster.dbo.ivp_refm_entity_type WHERE is_active = 1 AND entity_display_name = @entityTypeDisplayName
                        SELECT @attributeName = attribute_name FROM IVPRefMaster.dbo.ivp_refm_entity_attribute WHERE display_name = @attributeDisplayName AND entity_type_id = @entityTypeID
                        EXEC('select TOP 1 1 as null_values_count from IVPRefMaster.dbo.[' + @entityTypeName + '] where [' + @attributeName + '] is null;')";

            DataSet outputDs = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);

            if (outputDs != null && outputDs.Tables.Count > 0 && outputDs.Tables[0].Rows.Count > 0 && outputDs.Tables[0].Columns.Contains("null_values_count"))
                containsNullData = Convert.ToBoolean(outputDs.Tables[0].Rows[0]["null_values_count"]);

            if (containsNullData)
                errorMsgs.Add(RMAttributeSetupConstants.MANDATORY_ATTRIBUTE_NULL_ERROR);
            return errorMsgs;
        }

        bool ValidateSearchViewPosition(string attributeName, string restrictedChars)
        {
            throw new NotImplementedException();
        }

        public List<string> ValidateLegPrimary(string entityTypeName, string legEntityTypeName, string attributeName)
        {
            List<string> errorMsgs = new List<string>();

            string query = @"DECLARE @entityTypeDisplayName VARCHAR(MAX) = '" + entityTypeName + @"', @attributeDisplayName VARCHAR(MAX) = '" + attributeName + @"',@legEntityTypeDisplayName VARCHAR(MAX) = '" + legEntityTypeName + @"' , 
                            @legEntityTypeName VARCHAR(MAX) = '', @attributeName VARCHAR(MAX) = '', @entityTypeID INT = 0, @legEntityTypeId INT = 0

                            SELECT @entityTypeID = entity_type_id FROM IVPRefMaster.dbo.ivp_refm_entity_type WHERE is_active = 1 AND entity_display_name = @entityTypeDisplayName

                            SELECT @legEntityTypeName = entity_type_name, @legEntityTypeId = entity_type_id FROM IVPRefMaster.dbo.ivp_refm_entity_type WHERE derived_from_entity_type_id = @entityTypeID AND is_active = 1 AND entity_display_name = @legEntityTypeDisplayName

                            SELECT @attributeName = attribute_name FROM IVPRefMaster.dbo.ivp_refm_entity_attribute WHERE display_name = @attributeDisplayName AND entity_type_id = @legEntityTypeId

                            EXEC('select count(entity_code) from IVPRefMaster.dbo.[' + @legEntityTypeName + '] ')";

            DataSet outputDs = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);

            if (outputDs != null && outputDs.Tables.Count > 0 && outputDs.Tables[0].Rows.Count > 0 && Convert.ToInt32(outputDs.Tables[0].Rows[0][0]) > 0)
                errorMsgs.Add(RMAttributeSetupConstants.PRIMARY_CONTAINS_DATA_ERROR);

            return errorMsgs;
        }

        public List<string> ValidateReferenceType(bool isInsert, string entityTypeName, string attributeName, string dataType, string referenceType, string referenceAttribute, string referenceDisplayAttributes, string legName, IEnumerable<ObjectRow> currentRows)
        {
            List<string> errorMsgs = new List<string>();

            List<string> lookupDispAttributes = new List<string>();
            if (!string.IsNullOrEmpty(referenceDisplayAttributes.Trim()))
                lookupDispAttributes = referenceDisplayAttributes.Split(',').ToList();

            bool isLeg = !string.IsNullOrEmpty(legName);
            if (isInsert)
            {
                switch (dataType.ToUpper())
                {
                    case RMAttributeSetupConstants.DATA_TYPE_LOOKUP:

                        //dbMasterEntityTypes.Rows.AsEnumerable().Where(x => Convert.ToString(x["Entity Type Name"]).SRMEqualWithIgnoreCase(referenceType)).Count() == 0
                        if (!allModulesEntityDetailsInfo.ContainsKey(referenceType))
                            errorMsgs.Add(RMAttributeSetupConstants.INVALID_REFERENCE_TYPE_ERROR);

                        //IEnumerable<ObjectRow> attributes = dbMasterAttributes.Rows.AsEnumerable().Where(x => Convert.ToString(x["Entity Type Name"]).SRMEqualWithIgnoreCase(referenceType));

                        //if (attributes.Where(x => Convert.ToString(x["Attribute Name"]).Equals(referenceAttribute, StringComparison.InvariantCultureIgnoreCase)).Count() == 0)
                        //if (entityDetailsInfo.ContainsKey(referenceType) && !entityDetailsInfo[referenceType].Attributes.ContainsKey(referenceAttribute))
                        //    errorMsgs.Add(RMAttributeSetupConstants.INVALID_REFERENCE_ATTRIBUTE_ERROR);

                        if (currentRows.Where(x => Convert.ToString(x[RMModelerConstants.Attribute_Name]).SRMEqualWithIgnoreCase(referenceAttribute) &&
                            Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(referenceType) &&
                            !Convert.ToString(x[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).Count() == 0 &&
                            !(allModulesEntityDetailsInfo.ContainsKey(referenceType) && allModulesEntityDetailsInfo[referenceType].Attributes.ContainsKey(referenceAttribute)))
                            errorMsgs.Add(RMAttributeSetupConstants.INVALID_REFERENCE_ATTRIBUTE_ERROR);


                        foreach (string dispAttr in lookupDispAttributes)
                        {
                            //if (!string.IsNullOrEmpty(dispAttr) && entityDetailsInfo.ContainsKey(referenceType) && !entityDetailsInfo[referenceType].Attributes.ContainsKey(dispAttr))
                            //    //if (attributes.Where(x => Convert.ToString(x["Attribute Name"]).SRMEqualWithIgnoreCase(dispAttr)).Count() == 0)
                            //    errorMsgs.Add(dispAttr + " - " + RMAttributeSetupConstants.INVALID_REFERENCE_DISPLAY_ATTRIBUTE_ERROR);


                            if (currentRows.Where(x => Convert.ToString(x[RMModelerConstants.Attribute_Name]).SRMEqualWithIgnoreCase(dispAttr) &&
                                                Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(referenceType) &&
                                                !Convert.ToString(x[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).Count() == 0 &&
                                                !(allModulesEntityDetailsInfo.ContainsKey(referenceType) && allModulesEntityDetailsInfo[referenceType].Attributes.ContainsKey(dispAttr)))

                            {
                                errorMsgs.Add(dispAttr + " - " + RMAttributeSetupConstants.INVALID_REFERENCE_DISPLAY_ATTRIBUTE_ERROR);
                            }

                        }
                        break;
                    case RMAttributeSetupConstants.DATA_TYPE_SECURITYLOOKUP:
                    case RMAttributeSetupConstants.DATA_TYPE_SECURITY_LOOKUP:
                        if (!secTypeDetailsInfo.ContainsKey(referenceType) && !referenceType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.ALL_SECURITY_TYPES))
                            errorMsgs.Add(RMAttributeSetupConstants.INVALID_REFERENCE_TYPE_ERROR);

                        if (secTypeDetailsInfo.ContainsKey(referenceType) && !secTypeDetailsInfo[referenceType].AttributeInfo.MasterAttrs.ContainsKey(referenceAttribute))
                            errorMsgs.Add(RMAttributeSetupConstants.INVALID_REFERENCE_ATTRIBUTE_ERROR);

                        if (referenceType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.ALL_SECURITY_TYPES) && !commonAttributes.ContainsKey(referenceAttribute))
                            errorMsgs.Add(RMAttributeSetupConstants.INVALID_REFERENCE_ATTRIBUTE_ERROR);

                        foreach (string dispAttr in lookupDispAttributes)
                        {
                            if (!string.IsNullOrEmpty(dispAttr) && secTypeDetailsInfo.ContainsKey(referenceType) && !secTypeDetailsInfo[referenceType].AttributeInfo.MasterAttrs.ContainsKey(dispAttr))
                                //if (attributes.Where(x => Convert.ToString(x["Attribute Name"]).SRMEqualWithIgnoreCase(dispAttr)).Count() == 0)
                                errorMsgs.Add(dispAttr + " - " + RMAttributeSetupConstants.INVALID_REFERENCE_DISPLAY_ATTRIBUTE_ERROR);

                            if (!string.IsNullOrEmpty(dispAttr) && referenceType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.ALL_SECURITY_TYPES) && !commonAttributes.ContainsKey(dispAttr))
                                errorMsgs.Add(dispAttr + " - " + RMAttributeSetupConstants.INVALID_REFERENCE_DISPLAY_ATTRIBUTE_ERROR);

                            //if (currentRows.Where(x => Convert.ToString(x[RMModelerConstants.Attribute_Name]).SRMEqualWithIgnoreCase(dispAttr) &&
                            //                    Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(referenceType) &&
                            //                    !Convert.ToString(x[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).Count() == 0 &&
                            //                    !(entityDetailsInfo.ContainsKey(referenceType) && entityDetailsInfo[referenceType].Attributes.ContainsKey(dispAttr)))

                            //{
                            //    errorMsgs.Add(dispAttr + " - " + RMAttributeSetupConstants.INVALID_REFERENCE_DISPLAY_ATTRIBUTE_ERROR);
                            //}
                        }

                        break;
                }
            }
            else
            {
                switch (dataType.ToUpper())
                {
                    case RMAttributeSetupConstants.DATA_TYPE_LOOKUP:

                        if (!allModulesEntityDetailsInfo.ContainsKey(referenceType))
                            errorMsgs.Add(RMAttributeSetupConstants.INVALID_REFERENCE_TYPE_ERROR);

                        if (isLeg)
                        {
                            if (allModulesEntityDetailsInfo.ContainsKey(entityTypeName) && allModulesEntityDetailsInfo[entityTypeName].Legs.ContainsKey(legName) && allModulesEntityDetailsInfo[entityTypeName].Legs[legName].Attributes.ContainsKey(attributeName) && !allModulesEntityDetailsInfo[entityTypeName].Legs[legName].Attributes[attributeName].LookupEntityTypeName.SRMEqualWithIgnoreCase(referenceType))
                                errorMsgs.Add(RMAttributeSetupConstants.INVALID_UPDATE_REFERENCE_TYPE_ERROR);
                        }
                        else
                        {
                            if (allModulesEntityDetailsInfo.ContainsKey(entityTypeName) && allModulesEntityDetailsInfo[entityTypeName].Attributes.ContainsKey(attributeName) && !allModulesEntityDetailsInfo[entityTypeName].Attributes[attributeName].LookupEntityTypeName.SRMEqualWithIgnoreCase(referenceType))
                                errorMsgs.Add(RMAttributeSetupConstants.INVALID_UPDATE_REFERENCE_TYPE_ERROR);
                        }

                        //if (entityDetailsInfo.ContainsKey(referenceType) && !entityDetailsInfo[referenceType].Attributes.ContainsKey(referenceAttribute))
                        //    errorMsgs.Add(RMAttributeSetupConstants.INVALID_REFERENCE_ATTRIBUTE_ERROR);

                        if (currentRows.Where(x => Convert.ToString(x[RMModelerConstants.Attribute_Name]).SRMEqualWithIgnoreCase(referenceAttribute) &&
                            Convert.ToString(x[RMModelerConstants.ENTITY_DISPLAY_NAME]).SRMEqualWithIgnoreCase(referenceType) &&
                            !Convert.ToString(x[RMCommonConstants.MIGRATION_COL_STATUS]).SRMEqualWithIgnoreCase(RMCommonConstants.MIGRATION_FAILED)).Count() == 0 &&
                            !(allModulesEntityDetailsInfo.ContainsKey(referenceType) && allModulesEntityDetailsInfo[referenceType].Attributes.ContainsKey(referenceAttribute)))

                            errorMsgs.Add(RMAttributeSetupConstants.INVALID_REFERENCE_ATTRIBUTE_ERROR);

                        //List<string> lookupDispAttributes = referenceDisplayAttributes.Split(',').ToList();
                        foreach (string dispAttr in lookupDispAttributes)
                        {
                            if (allModulesEntityDetailsInfo.ContainsKey(referenceType) && !allModulesEntityDetailsInfo[referenceType].Attributes.ContainsKey(dispAttr))
                                errorMsgs.Add(dispAttr + " - " + RMAttributeSetupConstants.INVALID_REFERENCE_DISPLAY_ATTRIBUTE_ERROR);
                        }
                        break;
                    case RMAttributeSetupConstants.DATA_TYPE_SECURITYLOOKUP:
                    case RMAttributeSetupConstants.DATA_TYPE_SECURITY_LOOKUP:

                        if (!secTypeDetailsInfo.ContainsKey(referenceType) && !referenceType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.ALL_SECURITY_TYPES))
                            errorMsgs.Add(RMAttributeSetupConstants.INVALID_REFERENCE_TYPE_ERROR);

                        if (isLeg)
                        {
                            if (allModulesEntityDetailsInfo.ContainsKey(entityTypeName) && allModulesEntityDetailsInfo[entityTypeName].Legs.ContainsKey(legName) && allModulesEntityDetailsInfo[entityTypeName].Legs[legName].Attributes.ContainsKey(attributeName) && !allModulesEntityDetailsInfo[entityTypeName].Legs[legName].Attributes[attributeName].LookupEntityTypeName.SRMEqualWithIgnoreCase(referenceType))
                                errorMsgs.Add(RMAttributeSetupConstants.INVALID_UPDATE_REFERENCE_TYPE_ERROR);
                        }
                        else
                        {
                            if (allModulesEntityDetailsInfo.ContainsKey(entityTypeName) && allModulesEntityDetailsInfo[entityTypeName].Attributes.ContainsKey(attributeName) && !allModulesEntityDetailsInfo[entityTypeName].Attributes[attributeName].LookupEntityTypeName.SRMEqualWithIgnoreCase(referenceType))
                                errorMsgs.Add(RMAttributeSetupConstants.INVALID_UPDATE_REFERENCE_TYPE_ERROR);
                        }

                        if (secTypeDetailsInfo.ContainsKey(referenceType) && !secTypeDetailsInfo[referenceType].AttributeInfo.MasterAttrs.ContainsKey(referenceAttribute))
                            errorMsgs.Add(RMAttributeSetupConstants.INVALID_REFERENCE_ATTRIBUTE_ERROR);

                        if (referenceType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.ALL_SECURITY_TYPES) && !commonAttributes.ContainsKey(referenceAttribute))
                            errorMsgs.Add(RMAttributeSetupConstants.INVALID_REFERENCE_ATTRIBUTE_ERROR);

                        foreach (string dispAttr in lookupDispAttributes)
                        {
                            if (secTypeDetailsInfo.ContainsKey(referenceType) && !secTypeDetailsInfo[referenceType].AttributeInfo.MasterAttrs.ContainsKey(dispAttr))
                                errorMsgs.Add(dispAttr + " - " + RMAttributeSetupConstants.INVALID_REFERENCE_DISPLAY_ATTRIBUTE_ERROR);

                            if (!string.IsNullOrEmpty(dispAttr) && referenceType.SRMEqualWithIgnoreCase(RMAttributeSetupConstants.ALL_SECURITY_TYPES) && !commonAttributes.ContainsKey(dispAttr))
                                errorMsgs.Add(dispAttr + " - " + RMAttributeSetupConstants.INVALID_REFERENCE_DISPLAY_ATTRIBUTE_ERROR);
                        }
                        break;
                }
            }
            return errorMsgs;
        }

        public List<string> ValidateDefaultValue(string dataType, ref string defaultValue, string dateFormat, string attributeLength)
        {
            List<string> errorMsgs = new List<string>();
            switch (dataType)
            {
                case RMAttributeSetupConstants.DATA_TYPE_INT:
                    if (!string.IsNullOrEmpty(defaultValue) && !new Regex(RMAttributeSetupConstants.INT_REGEX).IsMatch(defaultValue))
                        errorMsgs.Add(RMAttributeSetupConstants.DEFAULT_VALUE_ERROR);
                    else if (!string.IsNullOrEmpty(defaultValue) && (Convert.ToInt32(defaultValue) < -2147483648 || Convert.ToInt32(defaultValue) > 2147483647))
                        errorMsgs.Add(RMAttributeSetupConstants.DEFAULT_VALUE_ERROR);
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_BIT:
                    if (!string.IsNullOrEmpty(defaultValue))
                    {
                        List<string> positivevalidValues = new List<string>() { "1", "Yes", "True", "T" };
                        List<string> negativevalidValues = new List<string>() { "0", "No", "False", "F" };
                        if (!positivevalidValues.SRMContainsWithIgnoreCase(defaultValue) && !negativevalidValues.SRMContainsWithIgnoreCase(defaultValue))
                            errorMsgs.Add(RMAttributeSetupConstants.DEFAULT_VALUE_ERROR);
                        else
                        {
                            if (positivevalidValues.SRMContainsWithIgnoreCase(defaultValue))
                                defaultValue = "true";
                            else
                                defaultValue = "false";
                        }
                    }
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_DECIMAL:
                    if (!string.IsNullOrEmpty(defaultValue) && !new Regex(RMAttributeSetupConstants.DECIMAL_REGEX).IsMatch(defaultValue))
                        errorMsgs.Add(RMAttributeSetupConstants.DEFAULT_VALUE_ERROR);
                    else if (!string.IsNullOrEmpty(defaultValue))
                    {
                        bool isNegativeDefaultValue = false;
                        int numericPart = Convert.ToInt32(attributeLength.Split('.')[0]);
                        int decimalPart = 0;
                        if (attributeLength.Contains('.'))
                        {
                            decimalPart = Convert.ToInt32(attributeLength.Split('.')[1]);
                        }
                        if (defaultValue.StartsWith("+"))
                            defaultValue = defaultValue.Substring(1);
                        else if (defaultValue.StartsWith("-"))
                        {
                            isNegativeDefaultValue = true;
                            defaultValue = defaultValue.Substring(1);
                        }

                        string[] defaultValueParts = defaultValue.Split('.');

                        if (defaultValueParts.Length == 1)
                        {
                            if (defaultValueParts[0].Length > numericPart)
                                errorMsgs.Add(RMAttributeSetupConstants.DEFAULT_VALUE_ERROR);
                        }
                        else
                        {
                            if (defaultValueParts[0].Length > numericPart)
                                errorMsgs.Add(RMAttributeSetupConstants.DEFAULT_VALUE_ERROR);

                            else if (defaultValueParts[1].Length > decimalPart)
                                errorMsgs.Add(RMAttributeSetupConstants.DEFAULT_VALUE_ERROR);

                        }
                        if (isNegativeDefaultValue)
                            defaultValue = "-" + defaultValue;
                    }
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_VARCHAR:
                    if (!string.IsNullOrEmpty(defaultValue) && defaultValue.Length > Convert.ToInt32(attributeLength))
                        errorMsgs.Add(RMAttributeSetupConstants.DEFAULT_VALUE_ERROR);
                    break;
                case RMAttributeSetupConstants.DATA_TYPE_DATETIME:
                    DateTime date = DateTime.Now;
                    if (!string.IsNullOrEmpty(defaultValue) && !DateTime.TryParseExact(defaultValue, dateFormat, null, System.Globalization.DateTimeStyles.None, out date))
                    {
                        if (DateTime.TryParseExact(defaultValue.Split(' ')[0], dateFormat, null, System.Globalization.DateTimeStyles.None, out date))
                        {
                            defaultValue = date.Date.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                        }
                        else
                            errorMsgs.Add(RMAttributeSetupConstants.DEFAULT_VALUE_ERROR);
                    }
                    else if(!string.IsNullOrEmpty(defaultValue))
                        defaultValue = date.Date.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                    break;
            }
            return errorMsgs;
        }

    }

    public class RMLayoutMangementValidations
    {
        Dictionary<string, string> validLayouts;
        Dictionary<string, string> validTabs;
        ObjectTable dbLayoutDetails = null;
        Dictionary<string, RMEntityDetailsInfo> entityInfo = null;
        public RMLayoutMangementValidations(ObjectSet dbEntityTypesDetails)
        {
            //entityInfo = entityDetails;
            dbLayoutDetails = dbEntityTypesDetails.Tables["Layouts"];
        }

        public List<string> validateLayoutName(string entityTypeName, string layoutName)
        {
            List<string> errorMsgs = new List<string>();
            //bool duplicateLayoutNames = false;

            if (string.IsNullOrEmpty(layoutName))
                errorMsgs.Add("Layout name cannot be empty or blank");

            if (!new Regex("^[0-9a-zA-Z ]*$").IsMatch(layoutName))
                errorMsgs.Add("Layout name cannot contain any special characters");

            if (dbLayoutDetails.Rows.AsEnumerable().Where(x => Convert.ToString(x["Entity Type Name"]).SRMEqualWithIgnoreCase(entityTypeName) && Convert.ToString(x["Layout Name"]).SRMEqualWithIgnoreCase(layoutName)).Count() > 1)
                errorMsgs.Add("Layout name already exist for this entity type");

            return errorMsgs;
        }

        public List<string> validateLayoutType(string enityTypeName, string layoutType, string dependentName)
        {
            List<string> errorMsgs = new List<string>();

            if (dbLayoutDetails.Rows.AsEnumerable().Where(x => Convert.ToString(x["Entity Type Name"]).SRMEqualWithIgnoreCase(enityTypeName) && Convert.ToString(x["Layout Type"]).SRMEqualWithIgnoreCase(layoutType) && Convert.ToString(x["Dependent Name"]).SRMEqualWithIgnoreCase(dependentName)).Count() > 1)
                errorMsgs.Add("Each Entity Type can have only one " + layoutType + " type layout");

            return errorMsgs;
        }

        public List<string> validateEntityStates(string enteredStates, int entityTypeId, Dictionary<string, List<int>> dictPossibleLayoutStates)
        {
            List<string> errorMsgs = new List<string>();
            var statesInSheet = enteredStates.Split(',').ToList();
            foreach (string state in statesInSheet)
            {
                if (!dictPossibleLayoutStates.ContainsKey(state) || (dictPossibleLayoutStates.ContainsKey(state) && !dictPossibleLayoutStates[state].Contains(-1) && !dictPossibleLayoutStates[state].Contains(entityTypeId)))
                    errorMsgs.Add("Incorrect entity state :" + state);
            }

            return errorMsgs;
        }

        public List<string> validateTabName(string layoutName, string tabName)
        {
            List<string> errorMsgs = new List<string>();
            //bool duplicateLayoutNames = false;

            if (string.IsNullOrEmpty(tabName))
                errorMsgs.Add("Tab name cannot be empty or blank");

            if (!new Regex("^[0-9a-zA-Z ]*$").IsMatch(tabName))
                errorMsgs.Add("Tab name cannot contain any special characters");

            return errorMsgs;
        }
        //Add validation for consecutive tab order.

        public List<string> validateTabOrder(Dictionary<string, Dictionary<string, RMEntityTypeTemplateDetails>> entityTypeVstemplateInfo, string entitytypeName, string layoutName, string tabName, int tabOrder)
        {
            List<string> errorMsgs = new List<string>();
            if (!entityTypeVstemplateInfo[entitytypeName][layoutName].TabNameVsId.ContainsKey(tabName) && entityTypeVstemplateInfo[entitytypeName][layoutName].TabNameVsId.Values.AsEnumerable().Any(x=> x.tabOrder.Equals(tabOrder)))
            {
                    errorMsgs.Add("Tab Order already assigned to another tab.");
            }
            if (entityTypeVstemplateInfo[entitytypeName][layoutName].TabNameVsId.ContainsKey(tabName) && !entityTypeVstemplateInfo[entitytypeName][layoutName].TabNameVsId.Values.AsEnumerable().Any(x => x.tabOrder.Equals("1")) && tabOrder != 1)
                errorMsgs.Add("Atleast one tab must have order as 1.");

            return errorMsgs;
        }
        public bool checkAllOrderValuesUnderMaxExist(IEnumerable<ObjectRow> lstObjRows, string columnName/*, string errorMessage*/, bool takeDistinctAndCheck)
        {
            IEnumerable<ObjectRow> lstObjRowsToCheck = lstObjRows;
            if (takeDistinctAndCheck)
            {
                lstObjRowsToCheck = lstObjRows.GroupBy(x => Convert.ToInt32(x[columnName])).Select(x => x.FirstOrDefault());
            }

            try
            {
                int max_order = lstObjRowsToCheck.Count();
                int sum = 0;

                foreach (var objRow in lstObjRowsToCheck)
                {
                    int order = Convert.ToInt32(objRow[columnName]);

                    if (order <= max_order)
                        sum += order;
                    else
                    {
                        break;
                    }
                }

                if (sum != (max_order * (max_order + 1) / 2))
                {
                    //foreach (var row in lstObjRows)
                    //    new RMCommonMigration().SetFailedRow(row, errorMessage, true);

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                //foreach (var row in lstObjRows)
                //    new RMCommonMigration().SetFailedRow(row, errorMessage, true);
                return false;
            }
        }
        public List<string> validateCheckBoxList(bool isMandatory, bool isVisible, bool isReadOnly, bool isLegPrimary)
        {
            List<string> errorMsgs = new List<string>();
            if (isMandatory && (!isVisible || isReadOnly) && !isLegPrimary)
                errorMsgs.Add("Mandatory attribute has to be set as visible and cannot be read only.");
            else if (isMandatory && !isVisible && isLegPrimary)
                errorMsgs.Add("Leg Primary attribute has to be set as visible");
            return errorMsgs;
        }

        public List<string> ValidateAttributesInTemplate(RMEntityDetailsInfo entityDetails, List<string> attributesToValidate)
        {
            List<string> dbAttributes = new List<string>();
            List<string> errorMsgs = new List<string>();
            dbAttributes = entityDetails.Attributes.Keys.ToList();
            foreach (string legName in entityDetails.Legs.Keys)
            {
                dbAttributes.AddRange(entityDetails.Legs[legName].Attributes.Keys.ToList());
            }
            //if (string.IsNullOrEmpty(legName))
            //else
            //dbAttributes.AddRange(entityDetails.Legs[legName].Attributes.Keys.ToList());

            if (!(attributesToValidate.All(dbAttributes.SRMContainsWithIgnoreCase) && attributesToValidate.Distinct().Count() == dbAttributes.Distinct().Count()))
                errorMsgs.Add("Attribute count mismatch in template");
            return errorMsgs;
        }
    }

    public class RMAttributeConfigMgmtValidations
    {
        public List<string> validatePageAndFuncIdentifier(string page_iden, string functionality_iden)
        {
            List<string> errorMsgs = new List<string>();
            if (page_iden.SRMEqualWithIgnoreCase(PageIdentifierValues.ENTITY_IDENTIFIER) && !(functionality_iden.SRMEqualWithIgnoreCase(FunctionalityIdentifier.DUPLICATES) || functionality_iden.SRMEqualWithIgnoreCase(FunctionalityIdentifier.ENTITY_TAB)))
                errorMsgs.Add("Functionaity identifier set for Entity Identifier is incorrect.");
            else if (page_iden.SRMEqualWithIgnoreCase(PageIdentifierValues.DASHBOARD) && !functionality_iden.SRMEqualWithIgnoreCase(FunctionalityIdentifier.OVERRIDE_ATTRIBUTES))
                errorMsgs.Add("Functionaity identifier set for Dashboard is incorrect.");
            else if (page_iden.SRMEqualWithIgnoreCase(PageIdentifierValues.WORKFLOW) && !functionality_iden.SRMEqualWithIgnoreCase(FunctionalityIdentifier.WORKFLOW_INBOX))
                errorMsgs.Add("Functionaity identifier set for Workflow is incorrect.");
            else if (page_iden.SRMEqualWithIgnoreCase(PageIdentifierValues.DATA_RECONCILIATION) && !functionality_iden.SRMEqualWithIgnoreCase(FunctionalityIdentifier.MISMATCH_MANAGER))
                errorMsgs.Add("Functionaity identifier set for Data Reconciliation is incorrect.");
            return errorMsgs;
        }
    }

    public class RMEventnotificationValidations
    {
        public List<string> validateQueues(string queues)
        {
            List<string> errorMsgs = new List<string>();
            List<string> allConfiguredTransports = new RTransportService().GetAllQueueTransportName();
            List<string> lstqueues = queues.Split(',').ToList();
            if (lstqueues.Except(allConfiguredTransports, StringComparer.OrdinalIgnoreCase).Any())
                errorMsgs.Add("Invalid Queues.");
            return errorMsgs;
        }
        public List<string> validateActions(string requested_actions)
        {
            List<string> errorMsgs = new List<string>();
            List<string> possibleActionTypes = CommonDALWrapper.ExecuteSelectQuery("select action_type_name from IVPRefMaster.dbo.ivp_refm_event_action_type", ConnectionConstants.RefMaster_Connection).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["action_type_name"])).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            List<string> lstActions = requested_actions.Split(',').ToList();
            if (lstActions.Except(possibleActionTypes, StringComparer.OrdinalIgnoreCase).Any())
                errorMsgs.Add("Invalid Actions.");
            return errorMsgs;
        }
    }

    public static class RMAttributeSetupConstants
    {

        public const string INVALID_ENITY_TYPE = "Entity type not found.";
        public const string INVALID_LEG_ENITY_TYPE = "Leg Entity type not found.";
        public const string INVALID_ATTRIBUTE = "Attribute not found.";
        public const string ATTRIBUTE_NAME_NOT_UNIQUE_ERROR = "Attribute Name already exists either for Main or it's Detail Entity Types.";
        public const string ATTRIBUTE_NAME_RESTICTED_ERROR = "Attribute Name cannot be a restricted string.";
        public const string DATA_LENGTH_EMPTY_ERROR = "Data Type Length cannot be left blank.";
        public const string DATA_LENGTH_INVALID_ERROR = "Data Type Length invalid.";
        public const string DATA_LENGTH_INVALID_DECIMAL_ERROR = "The number part and decimal part cannot be less than 0.";
        public const string INVALID_REFERENCE_TYPE_ERROR = "The lookup type does not exist.";
        public const string INVALID_UPDATE_REFERENCE_TYPE_ERROR = "The lookup type cannot be updated.";
        public const string INVALID_REFERENCE_ATTRIBUTE_ERROR = "The lookup attribute does not exists.";
        public const string INVALID_REFERENCE_DISPLAY_ATTRIBUTE_ERROR = "The lookup display attribute does not exists.";
        public const string MISSING_REFERENCE_DISPLAY_ATTRIBUTE_ERROR = "Atleast one lookup display attribute should be selected.";
        public const string MANDATORY_ATTRIBUTE_NULL_ERROR = "The Attribute cannot be marked as mandatory because entity type attribute contains NULL data.";
        public const string RESTRICTED_CHARS_DUPLICATE = "Duplicate restricted character in restricted characters list.";
        public const string PRIMARY_CONTAINS_DATA_ERROR = "Attribute cannot be marked as primary because leg already contains data";
        public const string DEFAULT_VALUE_ERROR = "Invalid default value provided.";
        public const string ATTRIBUTE_CANNOT_BE_ENCRYPTED = "Lookup attributes cannot be encrypted.";
        public const string INVALID_ATTRIBUTE_NAME = "Attribute name is not in proper format.";

        public const string INVALID_SHOW_ENTITY_CODE = "Show Entity Code column must be empty.";
        public const string INVALID_ORDER_BY_ATTRIBUTE = "Invalid value for Order By Attribute Column.";
        public const string INVALID_APPEND_PERCENTAGE = "Show Pecentage column must be empty.";
        public const string INVALID_APPEND_MULTIPLIER = "Show Multiple column must be empty.";
        public const string INVALID_ALLOW_COMMA = "Comma Formatting column must be empty.";

        public const string DATA_TYPE_VARCHAR_MAX_LENGTH = "8000";
        public const string DATA_TYPE_VARCHAR_MAX = "VARCHARMAX";
        public const string DATA_TYPE_VARCHAR = "VARCHAR";
        public const string DATA_TYPE_LOOKUP = "LOOKUP";
        public const string DATA_TYPE_SECURITYLOOKUP = "SECURITY LOOKUP";
        public const string DATA_TYPE_SECURITY_LOOKUP = "SECURITY_LOOKUP";
        public const string DATA_TYPE_DECIMAL = "DECIMAL";
        public const string DATA_TYPE_INT = "INT";
        public const string DATA_TYPE_BIT = "BIT";
        public const string DATA_TYPE_FILE = "FILE";
        public const string DATA_TYPE_DATETIME = "DATETIME";
        public const string ALL_SECURITY_TYPES = "All Security Types";

        public const string NUMERIC_REGEX = "^[0-9]*$";
        public const string INT_REGEX = "^[-+]?\\b\\d+\\b$";
        public const string DECIMAL_REGEX = "^[-+]?[0-9]*\\.?[0-9]+([eE][-+]?[0-9]+)?$";
        public const string ALPHA_NUMERIC_REGEX = "^[0-9a-zA-Z_\\s]*$";

        public static readonly List<string> RESTRICTED_ATTRIBUTE_NAMES = new List<string>() { "id", "entity_code", "is_active", "is_latest", "created_by", "last_modified_by", "loading_time", "instance_id", "knowledge_date", "is_deleted", "entity code", "is active", "is latest", "created by", "last modified by", "loading time", "instance id", "knowledge date", "is deleted", "created on", "created_on", "master_entity_code", "master entity code" };
    }

    public class PageIdentifierValues
    {
        public const string ENTITY_IDENTIFIER = "Entity Identifier";
        public const string DASHBOARD = "Dashboard";
        public const string WORKFLOW = "Workflow";
        public const string DATA_RECONCILIATION = "Data Reconciliation";
    }
    public class FunctionalityIdentifier
    {
        public const string DUPLICATES = "Duplicates";
        public const string OVERRIDE_ATTRIBUTES = "Override Attributes";
        public const string WORKFLOW_INBOX = "Workflow Inbox";
        public const string ENTITY_TAB = "Entity Tab";
        public const string MISMATCH_MANAGER = "Mismatch Manager";
    }

    public class RMLayoutManagementConstants
    {
        public const string SYSTEM_LAYOUT_TYPE = "System";
        public const string USER_LAYOUT_TYPE = "User";
        public const string GROUP_LAYOUT_TYPE = "Group";
        public const string CREATE_STATE = "create";
        public const string UPDATE_STATE = "update";
    }
}
