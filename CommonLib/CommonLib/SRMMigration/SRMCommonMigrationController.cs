using com.ivp.rad.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.ivp.commom.commondal;
using System.Data;
using com.ivp.rad.data;
using com.ivp.common;
using System.Reflection;
using com.ivp.rad.dal;
using com.ivp.common.reporting;
using System.Text.RegularExpressions;
using com.ivp.common.srmdwhjob;
using com.ivp.common.srmdownstreamcontroller;
using com.ivp.rad.transport;

namespace com.ivp.srmcommon
{
    public class SRMCommonMigrationController
    {
        private List<CommonSheetInfo> lstCommonSheetInfo;
        private string featureName;
        private MigrationFeatureEnum enumFeatureName;
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMCommonMigration");

        public SRMCommonMigrationController()
        {
            this.lstCommonSheetInfo = new List<CommonSheetInfo>();
            this.featureName = String.Empty;
        }

        public SRMCommonMigrationController(MigrationFeatureEnum featureNameEnum, int moduleId) : this()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> SRMCommonMigrationInfo -> Start");

            this.enumFeatureName = featureNameEnum;

            switch (featureNameEnum)
            {
                case MigrationFeatureEnum.SM_SecurityTypeModeler:
                    //this.featureName = "Security Master - Security Type Modeler";
                    populateStaticDataForSMSecurityTypeModeler();
                    break;
                case MigrationFeatureEnum.SM_CommonLegs:
                    populateStaticDataForSMCommonLegs();
                    break;
                case MigrationFeatureEnum.SM_LegsOrder:
                    populateStaticDataForSMLegsOrder();
                    break;
                case MigrationFeatureEnum.SM_CommonRules:
                    populateStaticDataForSMCommonRules();
                    break;
                case MigrationFeatureEnum.RM_EntityTypeModeler:
                    populateStaticDataForRMModeler();
                    break;
                case MigrationFeatureEnum.RM_DataSource:
                    populateStaticDataForRMDataSource();
                    break;
                case MigrationFeatureEnum.SM_RealtimePreference:
                    populateStaticDataForSMRealtimePreference();
                    break;
                case MigrationFeatureEnum.SM_DataSourcePrioritization:
                    populateStaticDataForSMDataSourcePrioritization();
                    break;
                case MigrationFeatureEnum.SRM_TaskManager:
                    populateStaticDataForSRMTaskManager();
                    break;
                case MigrationFeatureEnum.SRM_WorkFlowModeler:
                    populateStaticDataForSRMWorkFlowModeler(moduleId);
                    break;
                case MigrationFeatureEnum.SM_DataSource:
                    populateStaticDataForSMDataSource();
                    break;
                case MigrationFeatureEnum.SM_ValidationTasks:
                    populateStaticDataForSMValidationTask();
                    break;
                case MigrationFeatureEnum.RM_Reports:
                    populateStaticDataForRMReports();
                    break;
                case MigrationFeatureEnum.RM_Prioritization:
                    populateStaticDataForRMPrioritization();
                    break;
                case MigrationFeatureEnum.SM_CommonAttributes:
                    populateStaticDataForSMCommonAttributes();
                    break;
                case MigrationFeatureEnum.RM_RealtimePreference:
                    populateStaticDataForRMRealtimePreference();
                    break;
                case MigrationFeatureEnum.SM_CommonConfig:
                    populateStaticDataForCommonConfiguration();
                    break;
                case MigrationFeatureEnum.SM_UniqueKeys:
                    populateStaticDataForSMUniqueKeys();
                    break;
                case MigrationFeatureEnum.RM_DownstreamTasks:
                    populateStaticDataForRMDownstreamTasks();
                    break;
                case MigrationFeatureEnum.SM_DownstreamTasks:
                    populateStaticDataForSMReportingTask();
                    break;
                case MigrationFeatureEnum.SM_DownstreamSystems:
                    populateStaticDataForDownstreamSystemsSM();
                    break;
                case MigrationFeatureEnum.RM_DownstreamSystems:
                    populateStaticDataForRMDownstreamSystems();
                    break;
                case MigrationFeatureEnum.SM_TransportTasks:
                    populateStaticDataForSMTransportTask();
                    break;
                case MigrationFeatureEnum.RM_TransportTasks:
                    populateStaticDataForRMTransportTask();
                    break;
                case MigrationFeatureEnum.SRM_VendorSettings:
                    populateStaticDataForVendorSettings();
                    break;
                case MigrationFeatureEnum.SM_DownstreamReports:
                    populateStaticDataForSMDownstreamReports();
                    break;
                case MigrationFeatureEnum.RM_TimeSeriesUpdateTasks:
                    populateStaticDataForRMTimeSeriesTasks();
                    break;
                case MigrationFeatureEnum.SRM_DownstreamSync:
                    populateStaticDataForSRMDownstreamSync();
                    break;
            }
            mLogger.Debug("SRMCommonMigrationInfo -> SRMCommonMigrationInfo -> End");
        }

        public List<CommonSheetInfo> getLstCommonSheetInfo()
        {
            return lstCommonSheetInfo;
        }


        /////////////////////////////////////////////////
        // WRITE METHODS TO POPULATE STATIC DATA BELOW //
        /////////////////////////////////////////////////

        // SM Security Type Modeler
        private void populateStaticDataForSMSecurityTypeModeler()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMSecurityTypeModeler -> Start");
            try
            {
                //List<string> CreationDateTypePossibleValues = CommonDALWrapper.ExecuteSelectQuery("Select DISTINCT date_type FROM IVPSecMaster.dbo.ivp_secm_security_creation_date_types", ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["date_type"])).ToList();

                #region SM_SecurityTypeModeler_Sheet1_Definition_SheetDetails
                {
                    CommonSheetInfo cmnSheet1 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet1.sheetName = SM_SecurityTypeModeler_SheetNames.Definition;

                    //List of Column Infos
                    cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Description, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = true });
                    cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo()
                    {
                        columnName = SM_ColumnNames.Type,
                        dataTypeName = DataTypeName.ENUM_VARCHAR,
                        lstPossibleVal = new List<string>() { SM_SecurityTypeType.Vanilla_Structure,
                                                      SM_SecurityTypeType.Exotic_Structure,
                                                      SM_SecurityTypeType.Vanilla_Structure_With_Leg }
                    });
                    cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Has_Master_Underlyer, dataTypeName = DataTypeName.BIT });
                    cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Master_Underlyer_Sectype, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                    cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Underlyer_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });
                    cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Tags, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                    cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Creation_Date_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Creation_Date_Value, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "50", allowBlanks = true });
                    cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Allowed_Users, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lookupType = LookupType.RAD_USERS, allowBlanks = true });
                    cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Allowed_Groups, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lookupType = LookupType.RAD_GROUPS, allowBlanks = true });

                    //List of Primary Column Names
                    cmnSheet1.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type_Name);

                    this.lstCommonSheetInfo.Add(cmnSheet1);
                }
                #endregion

                #region SM_SecurityTypeModeler_Sheet2_MasterAttributes_SheetDetails
                {
                    CommonSheetInfo cmnSheet2 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet2.sheetName = SM_SecurityTypeModeler_SheetNames.Master_Attributes;

                    //List of Column Infos
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Description, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo()
                    {
                        columnName = SM_ColumnNames.Data_Type,
                        dataTypeName = DataTypeName.ENUM_VARCHAR,
                        lstPossibleVal = new List<string>() { SM_AttributesDataType.Boolean,
                                                          SM_AttributesDataType.Date,
                                                          SM_AttributesDataType.DateTime,
                                                          SM_AttributesDataType.File,
                                                          SM_AttributesDataType.Numeric,
                                                          SM_AttributesDataType.Reference,
                                                          SM_AttributesDataType.String,
                                                          SM_AttributesDataType.Time }
                    });
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Length, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Reference_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.REF, allowBlanks = true });
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Reference_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.REF, allowBlanks = true });
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Reference_Display_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.REF, allowBlanks = true });
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Reference_Leg_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.REF, allowBlanks = true });
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Is_Cloneable, dataTypeName = DataTypeName.BIT });
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Is_Mandatory, dataTypeName = DataTypeName.BIT });
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Tags, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });

                    //new added
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SHOW_ENTITY_CODE, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ORDER_BY_ATTRIBUTE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.REF, allowBlanks = true });
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.COMMA_FORMATTING, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SHOW_PERCENTAGE, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.APPEND_MULTIPLIER, dataTypeName = DataTypeName.BIT, allowBlanks = true });

                    //List of Primary Column Names
                    cmnSheet2.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type_Name);
                    cmnSheet2.lstPrimaryAttr.Add(SM_ColumnNames.Attribute_Name);
                    //cmnSheet2.lstPrimaryAttr.Add(SM_ColumnNames.Data_Type);
                    //cmnSheet666664442.lstPrimaryAttr.Add(SM_ColumnNames.Reference_Type);

                    this.lstCommonSheetInfo.Add(cmnSheet2);
                }
                #endregion

                #region SM_SecurityTypeModeler_Sheet3_Baskets_SheetDetails
                {
                    CommonSheetInfo cmnSheet3 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet3.sheetName = SM_SecurityTypeModeler_SheetNames.Baskets;

                    //List of Column Infos
                    cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                    cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo()
                    {
                        columnName = SM_ColumnNames.Type,
                        dataTypeName = DataTypeName.ENUM_VARCHAR,
                        dataTypeLength = "50",
                        lstPossibleVal = new List<string>() { SM_LegType.SingleInfo, SM_LegType.MultiInfo }
                    });


                    cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Has_Constituent, dataTypeName = DataTypeName.BIT });
                    cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Constituent_Security_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                    cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Primary_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });
                    cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Tags, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });

                    //List of Primary Column Names
                    cmnSheet3.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type_Name);
                    cmnSheet3.lstPrimaryAttr.Add(SM_ColumnNames.Leg_Name);

                    this.lstCommonSheetInfo.Add(cmnSheet3);
                }
                #endregion

                #region SM_SecurityTypeModeler_Sheet4_BasketAttributes_SheetDetails
                {
                    CommonSheetInfo cmnSheet4 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet4.sheetName = SM_SecurityTypeModeler_SheetNames.Basket_Attributes;

                    //List of Column Infos
                    cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                    cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                    cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Description, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                    cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo()
                    {
                        columnName = SM_ColumnNames.Data_Type,
                        dataTypeName = DataTypeName.ENUM_VARCHAR,
                        lstPossibleVal = new List<string>() { SM_AttributesDataType.Boolean,
                                                          SM_AttributesDataType.Date,
                                                          SM_AttributesDataType.DateTime,
                                                          SM_AttributesDataType.File,
                                                          SM_AttributesDataType.Numeric,
                                                          SM_AttributesDataType.Reference,
                                                          SM_AttributesDataType.String,
                                                          SM_AttributesDataType.Time }
                    });
                    cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Length, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                    cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Reference_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.REF, allowBlanks = true });
                    cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Reference_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.REF, allowBlanks = true });
                    cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Reference_Display_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.REF, allowBlanks = true });
                    cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Reference_Leg_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.REF, allowBlanks = true });
                    cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Is_Cloneable, dataTypeName = DataTypeName.BIT });
                    cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Is_Mandatory, dataTypeName = DataTypeName.BIT });
                    cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Tags, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });

                    //new added
                    cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SHOW_ENTITY_CODE, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                    cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ORDER_BY_ATTRIBUTE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.REF, allowBlanks = true });
                    cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.COMMA_FORMATTING, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                    cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SHOW_PERCENTAGE, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                    cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.APPEND_MULTIPLIER, dataTypeName = DataTypeName.BIT, allowBlanks = true });

                    //List of Primary Column Names
                    cmnSheet4.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type_Name);
                    cmnSheet4.lstPrimaryAttr.Add(SM_ColumnNames.Leg_Name);
                    cmnSheet4.lstPrimaryAttr.Add(SM_ColumnNames.Attribute_Name);
                    //cmnSheet4.lstPrimaryAttr.Add(SM_ColumnNames.Data_Type);
                    //cmnSheet4.lstPrimaryAttr.Add(SM_ColumnNames.Reference_Type);

                    this.lstCommonSheetInfo.Add(cmnSheet4);
                }
                #endregion

                #region SM_SecurityTypeModeler_Sheet5_AttributeLevelRules_SheetDetails
                {
                    CommonSheetInfo cmnSheet5 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet5.sheetName = SM_SecurityTypeModeler_SheetNames.Attribute_Level_Rules;

                    //Allow Multiples Against Primary
                    cmnSheet5.allowMultiplesAgainstPrimary = true;

                    //List of Column Infos
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo()
                    {
                        columnName = SM_ColumnNames.Rule_Type,
                        dataTypeName = DataTypeName.ENUM_VARCHAR,
                        dataTypeLength = "200",
                        lstPossibleVal = new List<string>()
                    {
                        SM_RuleTypes.Alert,
                        SM_RuleTypes.Calculated_Field,
                        SM_RuleTypes.Mnemonic,
                        SM_RuleTypes.Validation
                    }
                    });
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_Group_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_Text, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", isCaseSensitive = true });
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Priority, dataTypeName = DataTypeName.INT });
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_State, dataTypeName = DataTypeName.BIT });

                    //List of Primary Column Names
                    cmnSheet5.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type_Name);
                    cmnSheet5.lstPrimaryAttr.Add(SM_ColumnNames.Rule_Type);
                    cmnSheet5.lstPrimaryAttr.Add(SM_ColumnNames.Attribute_Name);

                    cmnSheet5.lstGroupValidations.Add(new SRMMigrationGroupValidations() { lstGVColumns = { SM_ColumnNames.Attribute_Name, SM_ColumnNames.Rule_Group_Name } });
                    //cmnSheet5.lstAttributesInGroupValidation.Add(SM_ColumnNames.Attribute_Name);
                    //cmnSheet5.lstAttributesInGroupValidation.Add(SM_ColumnNames.Rule_Group_Name);

                    //List of Unique Keys
                    cmnSheet5.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Rule_Name } });
                    cmnSheet5.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Priority } });

                    this.lstCommonSheetInfo.Add(cmnSheet5);
                }
                #endregion

                #region SM_SecurityTypeModeler_Sheet5B_BasketLevelRules_SheetDetails
                {
                    CommonSheetInfo cmnSheet5B = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet5B.sheetName = SM_SecurityTypeModeler_SheetNames.Basket_Level_Rules;

                    //Allow Multiples Against Primary
                    cmnSheet5B.allowMultiplesAgainstPrimary = true;

                    //List of Column Infos
                    cmnSheet5B.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet5B.lstColumnInfo.Add(new CommonColumnInfo()
                    {
                        columnName = SM_ColumnNames.Rule_Type,
                        dataTypeName = DataTypeName.ENUM_VARCHAR,
                        dataTypeLength = "200",
                        lstPossibleVal = new List<string>()
                        {
                            SM_RuleTypes.Basket_Alert,
                            SM_RuleTypes.Basket_Validation
                        }
                    });
                    cmnSheet5B.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                    cmnSheet5B.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet5B.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_Text, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", isCaseSensitive = true });
                    cmnSheet5B.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Priority, dataTypeName = DataTypeName.INT });
                    cmnSheet5B.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_State, dataTypeName = DataTypeName.BIT });

                    //List of Primary Column Names
                    cmnSheet5B.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type_Name);
                    cmnSheet5B.lstPrimaryAttr.Add(SM_ColumnNames.Rule_Type);
                    cmnSheet5B.lstPrimaryAttr.Add(SM_ColumnNames.Leg_Name);

                    //List of Unique Keys
                    cmnSheet5B.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Priority } });
                    cmnSheet5B.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Rule_Name } });

                    this.lstCommonSheetInfo.Add(cmnSheet5B);
                }
                #endregion

                #region SM_SecurityTypeModeler_Sheet5C_ConditionalFilter_SheetDetails
                {
                    CommonSheetInfo cmnSheet5C = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet5C.sheetName = SM_SecurityTypeModeler_SheetNames.Conditional_Filter;
                    //Allow Multiples Against Primary
                    cmnSheet5C.allowMultiplesAgainstPrimary = true;
                    //List of Column Infos
                    cmnSheet5C.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet5C.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Group_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
                    cmnSheet5C.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attributes_In_Group, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });

                    //List of Primary Column Names
                    cmnSheet5C.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type_Name);
                    cmnSheet5C.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Group_Name } });

                    this.lstCommonSheetInfo.Add(cmnSheet5C);
                }
                #endregion

                #region SM_SecurityTypeModeler_Sheet6_Defaults_SheetDetails
                {
                    CommonSheetInfo cmnSheet6 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet6.sheetName = SM_SecurityTypeModeler_SheetNames.Defaults;

                    //Allow Multiples Against Primary
                    cmnSheet6.allowMultiplesAgainstPrimary = true;

                    //List of Column Infos
                    cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Default_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                    cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Is_Default, dataTypeName = DataTypeName.BIT });
                    cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Description, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = true });
                    cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });
                    cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                    cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Default_Value, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });

                    //List of Primary Column Names
                    cmnSheet6.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type_Name);
                    cmnSheet6.lstPrimaryAttr.Add(SM_ColumnNames.Default_Name);

                    //List of Unique Keys
                    cmnSheet6.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Attribute_Name } });

                    this.lstCommonSheetInfo.Add(cmnSheet6);
                }
                #endregion

                #region SM_SecurityTypeModeler_Sheet7_Layouts_SheetDetails
                {
                    CommonSheetInfo cmnSheet7 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet7.sheetName = SM_SecurityTypeModeler_SheetNames.Layouts;

                    //List of Column Infos
                    cmnSheet7.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet7.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Layout_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", acceptorRegex = new System.Text.RegularExpressions.Regex("^[0-9a-zA-Z ]*$") });
                    cmnSheet7.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.User_Or_Group, dataTypeName = DataTypeName.ENUM_VARCHAR, lookupType = LookupType.RAD_USERS_OR_GROUPS, lookupColumnName = SM_ColumnNames.Layout_Type });
                    cmnSheet7.lstColumnInfo.Add(new CommonColumnInfo()
                    {
                        columnName = SM_ColumnNames.Layout_Type,
                        dataTypeName = DataTypeName.ENUM_VARCHAR,
                        dataTypeLength = "50",
                        lstPossibleVal = new List<string>()
                    {
                        SM_Layout_Type.System,
                        SM_Layout_Type.User,
                        SM_Layout_Type.Group
                    }
                    });

                    //List of Primary Column Names
                    cmnSheet7.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type_Name);
                    cmnSheet7.lstPrimaryAttr.Add(SM_ColumnNames.Layout_Name);

                    //List of Unique Keys
                    cmnSheet7.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.User_Or_Group } });

                    this.lstCommonSheetInfo.Add(cmnSheet7);
                }
                #endregion

                #region SM_SecurityTypeModeler_Sheet8_LayoutsDetails_SheetDetails
                {
                    CommonSheetInfo cmnSheet8 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet8.sheetName = SM_SecurityTypeModeler_SheetNames.Layouts_Details;

                    //Allow Multiples Against Primary
                    cmnSheet8.allowMultiplesAgainstPrimary = true;

                    //List of Column Infos
                    cmnSheet8.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet8.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Layout_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                    cmnSheet8.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Tab_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", acceptorRegex = new System.Text.RegularExpressions.Regex("^[0-9a-zA-Z _]*$") });
                    cmnSheet8.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Tab_Order, dataTypeName = DataTypeName.INT });
                    cmnSheet8.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Sub_Tab_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", acceptorRegex = new System.Text.RegularExpressions.Regex("^[0-9a-zA-Z _]*$") });
                    cmnSheet8.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Sub_Tab_Order, dataTypeName = DataTypeName.INT });

                    //List of Primary Column Names
                    cmnSheet8.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type_Name);
                    cmnSheet8.lstPrimaryAttr.Add(SM_ColumnNames.Layout_Name);

                    //List of Unique Keys
                    cmnSheet8.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Tab_Order, SM_ColumnNames.Sub_Tab_Order } });
                    cmnSheet8.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Sub_Tab_Name } });

                    //List of Attributes in Group Validation
                    cmnSheet8.lstGroupValidations.Add(new SRMMigrationGroupValidations() { lstGVColumns = { SM_ColumnNames.Tab_Name, SM_ColumnNames.Tab_Order } });
                    //cmnSheet8.lstAttributesInGroupValidation.Add(SM_ColumnNames.Tab_Name);
                    //cmnSheet8.lstAttributesInGroupValidation.Add(SM_ColumnNames.Tab_Order);

                    this.lstCommonSheetInfo.Add(cmnSheet8);
                }
                #endregion

                #region SM_SecurityTypeModeler_Sheet9_LayoutsAndPermissions_SheetDetails
                {
                    CommonSheetInfo cmnSheet9 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet9.sheetName = SM_SecurityTypeModeler_SheetNames.Layouts_And_Permissions;

                    //Allow Multiples Against Primary
                    cmnSheet9.allowMultiplesAgainstPrimary = true;

                    //List of Column Infos
                    cmnSheet9.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet9.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Layout_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                    cmnSheet9.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                    cmnSheet9.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Leg_Or_Sub_Tab_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                    cmnSheet9.lstColumnInfo.Add(new CommonColumnInfo()
                    {
                        columnName = SM_ColumnNames.Panel,
                        dataTypeName = DataTypeName.ENUM_VARCHAR,
                        lstPossibleVal = new List<string>()
                    {
                        SM_Layout_Panel.Center,
                        SM_Layout_Panel.Left,
                        SM_Layout_Panel.Right
                    }
                    });
                    cmnSheet9.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Order, dataTypeName = DataTypeName.INT });
                    cmnSheet9.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Is_Read_Only, dataTypeName = DataTypeName.BIT });
                    cmnSheet9.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Is_Mandatory, dataTypeName = DataTypeName.BIT });
                    cmnSheet9.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.To_Show, dataTypeName = DataTypeName.BIT });

                    //List of Primary Column Names
                    cmnSheet9.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type_Name);
                    cmnSheet9.lstPrimaryAttr.Add(SM_ColumnNames.Layout_Name);

                    //List of Unique Keys
                    cmnSheet9.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Attribute_Name } });
                    cmnSheet9.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Leg_Or_Sub_Tab_Name, SM_ColumnNames.Panel, SM_ColumnNames.Order } });

                    this.lstCommonSheetInfo.Add(cmnSheet9);
                }
                #endregion
                #region SM_SecurityTypeModeler_Sheet12_User_Group_Layout_Priority
                {
                    CommonSheetInfo cmnSheet12 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet12.sheetName = SM_SecurityTypeModeler_SheetNames.User_Group_Layout_Priority;

                    //List of Column Infos
                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR });

                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.USER_NAME, dataTypeName = DataTypeName.VARCHAR });
                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.GROUP_LEVEL_LAYOUT, dataTypeName = DataTypeName.VARCHAR });


                    cmnSheet12.allowMultiplesAgainstPrimary = true;
                    cmnSheet12.lstPrimaryAttr = new List<string>() { SM_ColumnNames.Security_Type_Name };
                    SRMMigrationUniqueKeys key = new SRMMigrationUniqueKeys();
                    key.lstUniqueColumns = new List<string>() { SM_ColumnNames.Security_Type_Name, SM_ColumnNames.USER_NAME };
                    cmnSheet12.lstUniqueKeys = new List<SRMMigrationUniqueKeys>() { key };
                    this.lstCommonSheetInfo.Add(cmnSheet12);


                }
                #endregion
                #region SM_SecurityTypeModeler_Sheet10_Exception_Preference
                {
                    CommonSheetInfo cmnSheet10 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet10.sheetName = SM_SecurityTypeModeler_SheetNames.Exception_Preferences;

                    //List of Column Infos
                    cmnSheet10.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet10.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ALERTS, dataTypeName = DataTypeName.BIT });
                    cmnSheet10.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Duplicates, dataTypeName = DataTypeName.BIT });
                    cmnSheet10.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.VENDOR_VALUE_MISSING, dataTypeName = DataTypeName.BIT });
                    cmnSheet10.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.INVALID_DATA, dataTypeName = DataTypeName.BIT });
                    cmnSheet10.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.NO_VENDOR_VALUE, dataTypeName = DataTypeName.BIT });
                    cmnSheet10.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REF_DATA_MISSING, dataTypeName = DataTypeName.BIT });
                    cmnSheet10.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SHOW_AS_EXCEPTION, dataTypeName = DataTypeName.BIT });
                    cmnSheet10.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.UNDERLIER_MISSING, dataTypeName = DataTypeName.BIT });
                    cmnSheet10.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.VALIDATION, dataTypeName = DataTypeName.BIT });
                    cmnSheet10.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.VALUE_CHANGED, dataTypeName = DataTypeName.BIT });
                    cmnSheet10.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.VENDOR_MISMATCH, dataTypeName = DataTypeName.BIT });

                    this.lstCommonSheetInfo.Add(cmnSheet10);

                    cmnSheet10.lstPrimaryAttr = new List<string>() { SM_ColumnNames.Security_Type_Name };

                }
                #endregion

                #region SM_SecurityTypeModeler_Sheet11_Action_Notification
                {
                    CommonSheetInfo cmnSheet11 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet11.sheetName = SM_SecurityTypeModeler_SheetNames.Action_Notification;

                    //List of Column Infos
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ACTION_LEVEL, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { "Attribute", "Leg", "Security" }, dataTypeLength = "500" });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ATTRIBUTE_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });

                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.QUEUES, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ACTIONS, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    this.lstCommonSheetInfo.Add(cmnSheet11);

                    cmnSheet11.lstPrimaryAttr = new List<string>() { SM_ColumnNames.Security_Type_Name, SM_ColumnNames.ACTION_LEVEL, SM_ColumnNames.Attribute_Name, SM_ColumnNames.Leg_Name };

                }
                #endregion

            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForSMSecurityTypeModeler -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMSecurityTypeModeler -> End");
            }
        }

        private void populateStaticDataForSMCommonLegs()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMCommonLegs -> Start");
            try
            {
                #region SM_SecurityTypeModeler_Sheet10_CommonLegDefinition_SheetDetails commented
                {
                    //CommonSheetInfo cmnSheet10 = new CommonSheetInfo();

                    ////Sheet Name
                    //cmnSheet10.sheetName = SM_CommonLegs_SheetNames.Common_Leg_Definition;

                    //cmnSheet10.allowMultiplesAgainstPrimary = true;

                    ////List of Column Infos
                    //cmnSheet10.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Common_Leg_Id, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
                    //cmnSheet10.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    //cmnSheet10.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });

                    ////List of Primary Column Names
                    //cmnSheet10.lstPrimaryAttr.Add(SM_ColumnNames.Common_Leg_Id);
                    //cmnSheet10.lstPrimaryAttr.Add(SM_ColumnNames.Leg_Name);
                    ////cmnSheet10.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type_Name);

                    ////List of Unique Keys
                    //cmnSheet10.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Security_Type_Name } });

                    //this.lstCommonSheetInfo.Add(cmnSheet10);
                }
                #endregion

                #region SM_SecurityTypeModeler_Sheet11_CommonLegAttributeDefinition_SheetDetails
                {
                    CommonSheetInfo cmnSheet11 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet11.sheetName = SM_CommonLegs_SheetNames.Common_Leg_Attribute_Definition;

                    //List of Column Infos
                    //cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Common_Leg_Id, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Multiple_Security_Types, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = new System.Text.RegularExpressions.Regex(@"^[ A-Za-z0-9$%*()_./-]+$") });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo()
                    {
                        columnName = SM_ColumnNames.Data_Type,
                        dataTypeName = DataTypeName.ENUM_VARCHAR,
                        lstPossibleVal = new List<string>()
                    {
                        SM_CommonLegAttributesDataType.Boolean,
                        SM_CommonLegAttributesDataType.Date,
                        SM_CommonLegAttributesDataType.DateTime,
                        SM_CommonLegAttributesDataType.File,
                        SM_CommonLegAttributesDataType.Numeric,
                        SM_CommonLegAttributesDataType.Reference,
                        SM_CommonLegAttributesDataType.String,
                        SM_CommonLegAttributesDataType.Time,
                        SM_CommonLegAttributesDataType.Security,
                        SM_CommonLegAttributesDataType.Text
                    }
                    });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Or_Reference_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.SEC_OR_REF, allowBlanks = true });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Reference_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.REF, allowBlanks = true });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Reference_Display_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.REF, allowBlanks = true });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Length, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Is_Primary, dataTypeName = DataTypeName.BIT });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Is_Cloneable, dataTypeName = DataTypeName.BIT });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Is_Mandatory, dataTypeName = DataTypeName.BIT });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Description, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });

                    //new added
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SHOW_ENTITY_CODE, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ORDER_BY_ATTRIBUTE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.REF, allowBlanks = true });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.COMMA_FORMATTING, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SHOW_PERCENTAGE, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                    cmnSheet11.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.APPEND_MULTIPLIER, dataTypeName = DataTypeName.BIT, allowBlanks = true });

                    //List of Primary Column Names
                    //cmnSheet11.lstPrimaryAttr.Add(SM_ColumnNames.Common_Leg_Id);
                    cmnSheet11.lstPrimaryAttr.Add(SM_ColumnNames.Leg_Name);
                    cmnSheet11.lstPrimaryAttr.Add(SM_ColumnNames.Multiple_Security_Types);
                    cmnSheet11.lstPrimaryAttr.Add(SM_ColumnNames.Attribute_Name);

                    this.lstCommonSheetInfo.Add(cmnSheet11);
                }
                #endregion

                #region SM_SecurityTypeModeler_Sheet12_CommonLegTemplates_SheetDetails
                {
                    CommonSheetInfo cmnSheet12 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet12.sheetName = SM_CommonLegs_SheetNames.Common_Leg_Templates;

                    //List of Column Infos
                    //cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Common_Leg_Id, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Multiple_Security_Types, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Template_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.User_Or_Group, dataTypeName = DataTypeName.ENUM_VARCHAR, lookupType = LookupType.RAD_USERS_OR_GROUPS, lookupColumnName = SM_ColumnNames.Layout_Type });
                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo()
                    {
                        columnName = SM_ColumnNames.Layout_Type,
                        dataTypeName = DataTypeName.ENUM_VARCHAR,
                        dataTypeLength = "50",
                        lstPossibleVal = new List<string>()
                    {
                        SM_Layout_Type.System,
                        SM_Layout_Type.User,
                        SM_Layout_Type.Group
                    }
                    });
                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Show_Created_By, dataTypeName = DataTypeName.BIT });
                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Show_Created_On, dataTypeName = DataTypeName.BIT });
                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Show_Last_Moodified_By, dataTypeName = DataTypeName.BIT });
                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Show_Last_Moodified_On, dataTypeName = DataTypeName.BIT });
                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Can_Add, dataTypeName = DataTypeName.BIT });
                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Can_Update, dataTypeName = DataTypeName.BIT });
                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Can_Delete, dataTypeName = DataTypeName.BIT });
                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Order_By_Clause, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });

                    //List of Primary Column Names
                    //cmnSheet12.lstPrimaryAttr.Add(SM_ColumnNames.Common_Leg_Id);
                    cmnSheet12.lstPrimaryAttr.Add(SM_ColumnNames.Leg_Name);
                    cmnSheet12.lstPrimaryAttr.Add(SM_ColumnNames.Multiple_Security_Types);
                    cmnSheet12.lstPrimaryAttr.Add(SM_ColumnNames.Template_Name);

                    //List of Unique Keys
                    cmnSheet12.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.User_Or_Group } });

                    this.lstCommonSheetInfo.Add(cmnSheet12);
                }
                #endregion

                #region SM_SecurityTypeModeler_Sheet13_CommonLegLayoutAndPermissions_SheetDetails
                {
                    CommonSheetInfo cmnSheet13 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet13.sheetName = SM_CommonLegs_SheetNames.Common_Leg_Layout_and_Permissions;

                    //Allow Multiples Against Primary
                    cmnSheet13.allowMultiplesAgainstPrimary = true;

                    //List of Column Infos
                    //cmnSheet13.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Common_Leg_Id, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
                    cmnSheet13.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet13.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Multiple_Security_Types, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
                    cmnSheet13.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Template_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet13.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Display_Order, dataTypeName = DataTypeName.INT });
                    cmnSheet13.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet13.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Is_Read_Only, dataTypeName = DataTypeName.BIT });
                    cmnSheet13.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Is_Mandatory, dataTypeName = DataTypeName.BIT });
                    cmnSheet13.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.To_Show, dataTypeName = DataTypeName.BIT });

                    //List of Primary Column Names
                    //cmnSheet13.lstPrimaryAttr.Add(SM_ColumnNames.Common_Leg_Id);
                    cmnSheet13.lstPrimaryAttr.Add(SM_ColumnNames.Leg_Name);
                    cmnSheet13.lstPrimaryAttr.Add(SM_ColumnNames.Multiple_Security_Types);
                    cmnSheet13.lstPrimaryAttr.Add(SM_ColumnNames.Template_Name);

                    //List of Unique Keys
                    cmnSheet13.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Attribute_Name } });
                    cmnSheet13.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Display_Order } });

                    this.lstCommonSheetInfo.Add(cmnSheet13);
                }
                #endregion

                #region SM_SecurityTypeModeler_Sheet14_CommonLegTemplateSecurityTypeMapping_SheetDetails
                {
                    CommonSheetInfo cmnSheet14 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet14.sheetName = SM_CommonLegs_SheetNames.Common_Leg_Security_Type_Mapping;

                    //Allow Multiples Against Primary
                    cmnSheet14.allowMultiplesAgainstPrimary = true;

                    //List of Column Infos
                    //cmnSheet14.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Common_Leg_Id, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
                    cmnSheet14.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet14.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Multiple_Security_Types, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
                    cmnSheet14.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Template_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet14.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });

                    //List of Primary Column Names
                    //cmnSheet14.lstPrimaryAttr.Add(SM_ColumnNames.Common_Leg_Id);
                    cmnSheet14.lstPrimaryAttr.Add(SM_ColumnNames.Leg_Name);
                    cmnSheet14.lstPrimaryAttr.Add(SM_ColumnNames.Multiple_Security_Types);
                    cmnSheet14.lstPrimaryAttr.Add(SM_ColumnNames.Template_Name);

                    //List of Unique Keys
                    cmnSheet14.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Security_Type_Name } });

                    this.lstCommonSheetInfo.Add(cmnSheet14);
                }
                #endregion

                #region SM_SecurityTypeModeler_Sheet12_User_Group_Layout_Priority
                {
                    CommonSheetInfo cmnSheet12 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet12.sheetName = SM_CommonLegs_SheetNames.User_Group_Layout_Priority;

                    //List of Column Infos
                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR });
                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Multiple_Security_Types, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });

                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.USER_NAME, dataTypeName = DataTypeName.VARCHAR });
                    cmnSheet12.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.GROUP_LEVEL_LAYOUT, dataTypeName = DataTypeName.VARCHAR });


                    cmnSheet12.allowMultiplesAgainstPrimary = true;
                    cmnSheet12.lstPrimaryAttr = new List<string>() { SM_ColumnNames.Leg_Name, SM_ColumnNames.Multiple_Security_Types };
                    SRMMigrationUniqueKeys key = new SRMMigrationUniqueKeys();
                    key.lstUniqueColumns = new List<string>() { SM_ColumnNames.Leg_Name, SM_ColumnNames.Multiple_Security_Types, SM_ColumnNames.USER_NAME };
                    cmnSheet12.lstUniqueKeys = new List<SRMMigrationUniqueKeys>() { key };
                    this.lstCommonSheetInfo.Add(cmnSheet12);


                }
                #endregion

                #region SM_SecurityTypeModeler_Sheet5_AttributeLevelRules_SheetDetails
                {
                    CommonSheetInfo cmnSheet5 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet5.sheetName = SM_CommonLegs_SheetNames.Common_Leg_Attribute_Level_Rules;

                    //Allow Multiples Against Primary
                    cmnSheet5.allowMultiplesAgainstPrimary = true;

                    //List of Column Infos
                    //cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Common_Leg_Id, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo()
                    {
                        columnName = SM_ColumnNames.Rule_Type,
                        dataTypeName = DataTypeName.ENUM_VARCHAR,
                        dataTypeLength = "200",
                        lstPossibleVal = new List<string>()
                    {
                        SM_RuleTypes.Alert,
                        SM_RuleTypes.Calculated_Field,
                        SM_RuleTypes.Mnemonic,
                        SM_RuleTypes.Validation
                    }
                    });
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_Group_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_Text, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", isCaseSensitive = true });
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Priority, dataTypeName = DataTypeName.INT });
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_State, dataTypeName = DataTypeName.BIT });

                    //List of Primary Column Names
                    //cmnSheet5.lstPrimaryAttr.Add(SM_ColumnNames.Common_Leg_Id);

                    cmnSheet5.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type_Name);
                    cmnSheet5.lstPrimaryAttr.Add(SM_ColumnNames.Rule_Type);
                    cmnSheet5.lstPrimaryAttr.Add(SM_ColumnNames.Attribute_Name);
                    cmnSheet5.lstPrimaryAttr.Add(SM_ColumnNames.Leg_Name);

                    cmnSheet5.lstGroupValidations.Add(new SRMMigrationGroupValidations() { lstGVColumns = { SM_ColumnNames.Attribute_Name, SM_ColumnNames.Rule_Group_Name } });
                    //cmnSheet5.lstAttributesInGroupValidation.Add(SM_ColumnNames.Attribute_Name);
                    //cmnSheet5.lstAttributesInGroupValidation.Add(SM_ColumnNames.Rule_Group_Name);

                    //List of Unique Keys
                    cmnSheet5.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Rule_Name } });
                    cmnSheet5.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Priority } });

                    this.lstCommonSheetInfo.Add(cmnSheet5);
                }
                #endregion

                #region SM_SecurityTypeModeler_Sheet5B_BasketLevelRules_SheetDetails
                {
                    CommonSheetInfo cmnSheet5B = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet5B.sheetName = SM_CommonLegs_SheetNames.Common_Leg_Basket_Level_Rules;

                    //Allow Multiples Against Primary
                    cmnSheet5B.allowMultiplesAgainstPrimary = true;

                    //List of Column Infos
                    //cmnSheet5B.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Common_Leg_Id, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
                    cmnSheet5B.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet5B.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet5B.lstColumnInfo.Add(new CommonColumnInfo()
                    {
                        columnName = SM_ColumnNames.Rule_Type,
                        dataTypeName = DataTypeName.ENUM_VARCHAR,
                        dataTypeLength = "200",
                        lstPossibleVal = new List<string>()
                        {
                            SM_RuleTypes.Basket_Alert,
                            SM_RuleTypes.Basket_Validation
                        }
                    });
                    //cmnSheet5B.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                    cmnSheet5B.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet5B.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_Text, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", isCaseSensitive = true });
                    cmnSheet5B.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Priority, dataTypeName = DataTypeName.INT });
                    cmnSheet5B.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_State, dataTypeName = DataTypeName.BIT });

                    //List of Primary Column Names
                    //cmnSheet5B.lstPrimaryAttr.Add(SM_ColumnNames.Common_Leg_Id);
                    cmnSheet5B.lstPrimaryAttr.Add(SM_ColumnNames.Leg_Name);
                    cmnSheet5B.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type_Name);
                    cmnSheet5B.lstPrimaryAttr.Add(SM_ColumnNames.Rule_Type);

                    //List of Unique Keys
                    cmnSheet5B.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Priority } });
                    cmnSheet5B.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Rule_Name } });

                    this.lstCommonSheetInfo.Add(cmnSheet5B);
                }
                #endregion

            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForSMCommonLegs -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMCommonLegs -> End");
            }
        }

        private void populateStaticDataForSMLegsOrder()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMLegsOrder -> Start");
            try
            {
                #region SM_SecurityTypeModeler_Sheet15_LegOrder_SheetDetails
                {
                    CommonSheetInfo cmnSheet15 = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet15.sheetName = SM_LegsOrder_SheetNames.Leg_Order;

                    //Allow Multiples Against Primary
                    cmnSheet15.allowMultiplesAgainstPrimary = true;

                    //List of Column Infos
                    cmnSheet15.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet15.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Template_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet15.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet15.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Order, dataTypeName = DataTypeName.INT });

                    //List of Primary Column Names
                    cmnSheet15.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type_Name);
                    cmnSheet15.lstPrimaryAttr.Add(SM_ColumnNames.Template_Name);

                    //List of Unique Keys
                    cmnSheet15.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Leg_Name } });
                    cmnSheet15.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.Order } });

                    this.lstCommonSheetInfo.Add(cmnSheet15);
                }
                #endregion
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForSMLegsOrder -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMLegsOrder -> End");
            }
        }

        // SM Common Rules

        private void populateStaticDataForSMCommonRules()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMCommonRules -> Start");
            try
            {
                List<string> CreationDateTypePossibleValues = CommonDALWrapper.ExecuteSelectQuery("Select DISTINCT date_type FROM IVPSecMaster.dbo.ivp_secm_security_creation_date_types", ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["date_type"])).ToList();

                #region SM_CommonRules_Sheet1_Definition_SheetDetails
                CommonSheetInfo cmnSheet1 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet1.sheetName = SM_CommonRules_SheetNames.All_Common_Rules;

                cmnSheet1.allowMultiplesAgainstPrimary = true;

                //List of Column Infos
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_Group_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_Text, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", isCaseSensitive = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Priority, dataTypeName = DataTypeName.INT, dataTypeLength = "1000" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_State, dataTypeName = DataTypeName.BIT });

                //List of Primary Column Names

                cmnSheet1.lstPrimaryAttr.Add(SM_ColumnNames.Rule_Group_Name);
                cmnSheet1.lstPrimaryAttr.Add(SM_ColumnNames.Rule_Type);
                cmnSheet1.lstPrimaryAttr.Add(SM_ColumnNames.Attribute_Name);

                cmnSheet1.lstUniqueKeys.Add(new SRMMigrationUniqueKeys { lstUniqueColumns = new List<string> { SM_ColumnNames.Priority } });

                this.lstCommonSheetInfo.Add(cmnSheet1);
                #endregion


                #region SM_SecurityTypeModeler_Sheet2_MasterAttributes_SheetDetails
                CommonSheetInfo cmnSheet2 = new CommonSheetInfo();

                cmnSheet2.allowMultiplesAgainstPrimary = true;

                //Sheet Name
                cmnSheet2.sheetName = SM_CommonRules_SheetNames.Configured_Security_Types;
                //List of Column Infos
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_Group_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000" });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000" });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000" });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000" });

                //List of Primary Column Names
                cmnSheet2.lstPrimaryAttr.Add(SM_ColumnNames.Rule_Group_Name);
                cmnSheet2.lstPrimaryAttr.Add(SM_ColumnNames.Rule_Type);
                //   cmnSheet2.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type_Name);
                cmnSheet2.lstPrimaryAttr.Add(SM_ColumnNames.Attribute_Name);
                this.lstCommonSheetInfo.Add(cmnSheet2);
                #endregion
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForSMCommonRules -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMCommonRules -> End");
            }
        }

        private void populateStaticDataForSMCommonAttributes()
        {
            CommonSheetInfo cmnSheet = new CommonSheetInfo();

            //Sheet Name
            cmnSheet.sheetName = SM_CommonAttributes_SheetNames.Attribute_Definition;

            //List of Column Infos
            cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
            cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Description, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
            cmnSheet.lstColumnInfo.Add(new CommonColumnInfo()
            {
                columnName = SM_ColumnNames.Attribute_Type,
                dataTypeName = DataTypeName.ENUM_VARCHAR,
                lstPossibleVal = new List<string>() {
                    SM_CommonAttribute_AttributeType.Identifiers,
                    SM_CommonAttribute_AttributeType.Reference_Data,
                    SM_CommonAttribute_AttributeType.Security_Master_Information,
                }
            });
            cmnSheet.lstColumnInfo.Add(new CommonColumnInfo()
            {
                columnName = SM_ColumnNames.Data_Type,
                dataTypeName = DataTypeName.ENUM_VARCHAR,
                lstPossibleVal = new List<string>() { SM_AttributesDataType.Boolean,
                                                          SM_AttributesDataType.Date,
                                                          SM_AttributesDataType.DateTime,
                                                          SM_AttributesDataType.File,
                                                          SM_AttributesDataType.Numeric,
                                                          SM_AttributesDataType.Reference,
                                                          SM_AttributesDataType.String,
                                                          SM_AttributesDataType.Time }
            });
            cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Reference_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.REF, allowBlanks = true });
            cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Reference_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.REF, allowBlanks = true });
            cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Reference_Display_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.REF, allowBlanks = true });
            cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Reference_Leg_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.REF, allowBlanks = true });
            cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Length, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
            cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Is_Cloneable, dataTypeName = DataTypeName.BIT });
            cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Is_Mandatory, dataTypeName = DataTypeName.BIT });
            cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Tags, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });

            //new added
            cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SHOW_ENTITY_CODE, dataTypeName = DataTypeName.BIT, allowBlanks = true });
            cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ORDER_BY_ATTRIBUTE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.REF, allowBlanks = true });
            cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.COMMA_FORMATTING, dataTypeName = DataTypeName.BIT, allowBlanks = true });
            cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SHOW_PERCENTAGE, dataTypeName = DataTypeName.BIT, allowBlanks = true });
            cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.APPEND_MULTIPLIER, dataTypeName = DataTypeName.BIT, allowBlanks = true });

            //List of Primary Column Names
            cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.Attribute_Name);

            this.lstCommonSheetInfo.Add(cmnSheet);
        }

        private void populateStaticDataForSMUniqueKeys()
        {
            CommonSheetInfo cmnSheet1 = new CommonSheetInfo();
            CommonSheetInfo cmnSheet2 = new CommonSheetInfo();
            CommonSheetInfo cmnSheet3 = new CommonSheetInfo();
            CommonSheetInfo cmnSheet4 = new CommonSheetInfo();

            cmnSheet1.sheetName = SM_UniqueKeys_SheetNames.Definition;
            cmnSheet2.sheetName = SM_UniqueKeys_SheetNames.Security_Types;
            cmnSheet3.sheetName = SM_UniqueKeys_SheetNames.Legs;
            cmnSheet4.sheetName = SM_UniqueKeys_SheetNames.Attributes;

            //definition sheet

            cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_UniqueKeys_ColumnNames.Key_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });

            cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo()
            {
                columnName = SM_UniqueKeys_ColumnNames.Level,
                dataTypeName = DataTypeName.ENUM_VARCHAR,
                lstPossibleVal = new List<string>() { SM_UniqueKeys_LevelTypes.Attribute_Level, SM_UniqueKeys_LevelTypes.Leg_Level }
            });


            cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_UniqueKeys_ColumnNames.Is_Across_Securities, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { SM_BooleanValues_YesNo.Yes, SM_BooleanValues_YesNo.No } });
            cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_UniqueKeys_ColumnNames.Check_In_Drafts, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { SM_BooleanValues_YesNo.Yes, SM_BooleanValues_YesNo.No } });
            cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_UniqueKeys_ColumnNames.Check_In_Workflows, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { SM_BooleanValues_YesNo.Yes, SM_BooleanValues_YesNo.No } });
            cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_UniqueKeys_ColumnNames.Consider_Null_As_Unique, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { SM_BooleanValues_YesNo.Yes, SM_BooleanValues_YesNo.No } });
            cmnSheet1.lstPrimaryAttr.Add(SM_UniqueKeys_ColumnNames.Key_Name);

            //sectypes sheet
            cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_UniqueKeys_ColumnNames.Key_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
            cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_UniqueKeys_ColumnNames.Security_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
            cmnSheet2.allowMultiplesAgainstPrimary = true;
            cmnSheet2.lstPrimaryAttr.Add(SM_UniqueKeys_ColumnNames.Key_Name);
            cmnSheet2.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = new List<string>() { SM_UniqueKeys_ColumnNames.Security_Type } });

            //legs sheet
            cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_UniqueKeys_ColumnNames.Key_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
            cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_UniqueKeys_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
            cmnSheet3.lstPrimaryAttr.Add(SM_UniqueKeys_ColumnNames.Key_Name);

            //attributes sheet
            cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_UniqueKeys_ColumnNames.Key_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
            cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo()
            {
                columnName
                = SM_UniqueKeys_ColumnNames.Attribute_Name,
                dataTypeName = DataTypeName.VARCHAR,
                dataTypeLength = "-1"
            });
            cmnSheet4.allowMultiplesAgainstPrimary = true;
            cmnSheet4.lstPrimaryAttr.Add(SM_UniqueKeys_ColumnNames.Key_Name);
            cmnSheet4.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = new List<string>() { SM_UniqueKeys_ColumnNames.Attribute_Name } });

            this.lstCommonSheetInfo.Add(cmnSheet1);
            this.lstCommonSheetInfo.Add(cmnSheet2);
            this.lstCommonSheetInfo.Add(cmnSheet3);
            this.lstCommonSheetInfo.Add(cmnSheet4);
        }

        private void populateStaticDataForSRMTaskManager()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSRMTaskManager -> Start");
            try
            {
                CommonSheetInfo cmnSheet1 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet1.sheetName = SRM_TaskManager_SheetNames.Chain_Information;

                //List of Column Infos
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Chain_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Calendar_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Recurrence_Type, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Start_Date, dataTypeName = DataTypeName.DATE, allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Recurrence_Pattern, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Interval, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.End_Date, dataTypeName = DataTypeName.DATETIME, allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Number_of_Recurrences, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Start_Time, dataTypeName = DataTypeName.DATETIME, allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Time_Interval_of_Recurrence, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Never_End_Job, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Days_Of_Week, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });

                //List of Primary Column Names
                cmnSheet1.lstPrimaryAttr.Add(SM_ColumnNames.Chain_Name);

                this.lstCommonSheetInfo.Add(cmnSheet1);


                CommonSheetInfo cmnSheet2 = new CommonSheetInfo { allowMultiplesAgainstPrimary = true };

                //Sheet Name
                cmnSheet2.sheetName = SRM_TaskManager_SheetNames.Task_Information;

                //List of Column Infos
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Chain_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Task_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Task_Type, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Module_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Is_Muted, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Proceed_On_Fail, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ReRun_on_Fail, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Retry_Duration, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Number_of_Retries, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.On_Fail_Run_Task, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.On_Fail_Run_Task_Task_Type, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.On_Fail_Run_Task_Module_Name, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Time_Out, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Task_Instance_Wait, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Task_Wait_Subscription, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });

                //List of Primary Column Names
                cmnSheet2.lstPrimaryAttr.Add(SM_ColumnNames.Chain_Name);
                cmnSheet2.lstUniqueKeys.Add(new SRMMigrationUniqueKeys { lstUniqueColumns = new List<string> { SM_ColumnNames.Task_Name, SM_ColumnNames.Task_Type } });

                this.lstCommonSheetInfo.Add(cmnSheet2);

                //Sheet Name
                CommonSheetInfo cmnSheet3 = new CommonSheetInfo { allowMultiplesAgainstPrimary = true };
                cmnSheet3.sheetName = SRM_TaskManager_SheetNames.Task_Dependency;
                //List of Column Infos
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Chain_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Task_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Task_Type, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Module_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Dependent_On_Task, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Dependent_On_Task_Type, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Dependent_On_Module_Name, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Dependency_Relation, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });

                //List of Primary Column Names
                cmnSheet3.lstPrimaryAttr.Add(SM_ColumnNames.Chain_Name);

                this.lstCommonSheetInfo.Add(cmnSheet3);

                //Sheet Name
                #region CHAIN SUBSCRIPTION REMOVED
                //CommonSheetInfo cmnSheet4 = new CommonSheetInfo { allowMultiplesAgainstPrimary = true };
                //cmnSheet4.sheetName = SRM_TaskManager_SheetNames.Chain_Subscription;
                ////List of Column Infos
                //cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Chain_Name, dataTypeName = DataTypeName.VARCHAR });
                //cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Subscription_Type, dataTypeName = DataTypeName.VARCHAR });
                //cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.To_Mail, dataTypeName = DataTypeName.VARCHAR });
                //cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Subject, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                //cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Body, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });

                ////List of Primary Column Names
                //cmnSheet4.lstPrimaryAttr.Add(SM_ColumnNames.Chain_Name);

                //this.lstCommonSheetInfo.Add(cmnSheet4);
                #endregion

                //Sheet Name
                CommonSheetInfo cmnSheet5 = new CommonSheetInfo { allowMultiplesAgainstPrimary = true };
                cmnSheet5.sheetName = SRM_TaskManager_SheetNames.Task_Subscription;
                //List of Column Infos
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Chain_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Task_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Task_Type, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Module_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Subscription_Type, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.To_Mail, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Subject, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Body, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });

                //List of Primary Column Names
                cmnSheet5.lstPrimaryAttr.Add(SM_ColumnNames.Chain_Name);

                this.lstCommonSheetInfo.Add(cmnSheet5);
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForSRMTaskManager -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSRMTaskManager -> End");
            }
        }

        private void populateStaticDataForVendorSettings()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForVendorSettings -> Start");
            try
            {
                List<string> lstVendorType = CommonDALWrapper.ExecuteSelectQuery("Select DISTINCT vendor_name FROM IVPRefMasterVendor.dbo.ivp_rad_vendor_management_vendor_type", ConnectionConstants.RefMasterVendor_Connection).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["vendor_name"])).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                List<string> lstRequestType = CommonDALWrapper.ExecuteSelectQuery("SELECT DISTINCT request_type_name FROM IVPRefMasterVendor.dbo.ivp_rad_vendor_management_request_type", ConnectionConstants.RefMasterVendor_Connection).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["request_type_name"])).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                List<string> lstHeaderType = CommonDALWrapper.ExecuteSelectQuery("SELECT DISTINCT header_type_name FROM IVPRefMasterVendor.dbo.ivp_rad_vendor_management_header_type", ConnectionConstants.RefMasterVendor_Connection).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["header_type_name"])).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                #region SRM_Vendor_Settings_Configurations_Sheet
                CommonSheetInfo cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = SRM_VendorManagement_SheetNames.CONFIGURATIONS;

                //Column List
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_VendorManagement_ColumnNames.PREFERENCE_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_VendorManagement_ColumnNames.VENDOR,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    dataTypeLength = "200",
                    lstPossibleVal = lstVendorType,
                    allowBlanks = false
                });

                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_VendorManagement_ColumnNames.REQUEST_TYPE,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    dataTypeLength = "200",
                    lstPossibleVal = lstRequestType,
                    allowBlanks = false
                });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_VendorManagement_ColumnNames.CONFIGURATION_KEY, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_VendorManagement_ColumnNames.VALUE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = false });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(SRM_VendorManagement_ColumnNames.PREFERENCE_NAME);
                cmnSheet.lstPrimaryAttr.Add(SRM_VendorManagement_ColumnNames.VENDOR);
                cmnSheet.lstPrimaryAttr.Add(SRM_VendorManagement_ColumnNames.REQUEST_TYPE);
                cmnSheet.lstPrimaryAttr.Add(SRM_VendorManagement_ColumnNames.CONFIGURATION_KEY);
                cmnSheet.allowMultiplesAgainstPrimary = false;

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region SRM_Vendor_Settings_Headers_Sheet
                cmnSheet = new CommonSheetInfo();
                //Sheet Name
                cmnSheet.sheetName = SRM_VendorManagement_SheetNames.HEADERS;

                //Column List
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_VendorManagement_ColumnNames.PREFERENCE_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_VendorManagement_ColumnNames.VENDOR,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    dataTypeLength = "200",
                    lstPossibleVal = lstVendorType,
                    allowBlanks = false
                });

                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_VendorManagement_ColumnNames.REQUEST_TYPE,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    dataTypeLength = "200",
                    lstPossibleVal = lstRequestType,
                    allowBlanks = false
                });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_VendorManagement_ColumnNames.HEADER_TYPE_NAME,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    dataTypeLength = "500",
                    lstPossibleVal = lstHeaderType,
                    allowBlanks = false
                });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_VendorManagement_ColumnNames.HEADER_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_VendorManagement_ColumnNames.HEADER_VALUE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });

                cmnSheet.lstPrimaryAttr.Add(SRM_VendorManagement_ColumnNames.VENDOR);
                cmnSheet.lstPrimaryAttr.Add(SRM_VendorManagement_ColumnNames.PREFERENCE_NAME);
                cmnSheet.lstPrimaryAttr.Add(SRM_VendorManagement_ColumnNames.REQUEST_TYPE);
                cmnSheet.lstPrimaryAttr.Add(SRM_VendorManagement_ColumnNames.HEADER_TYPE_NAME);
                cmnSheet.allowMultiplesAgainstPrimary = true;

                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SRM_VendorManagement_ColumnNames.HEADER_NAME } });
                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForVendorSettings -> Error : " + ex.Message.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForVendorSettings -> End");
            }
        }
        private void populateStaticDataForDownstreamSystemsSM()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForDownstreamSystemsSM -> Start");
            try
            {
                #region sheet1
                CommonSheetInfo cmnSheet1 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet1.sheetName = SM_DownstreamSystem_SheetNames.Definition;

                //List of Column Infos
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SYSTEM_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = new System.Text.RegularExpressions.Regex(@"^[ A-Za-z0-9@$:]+$") });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SYSTEM_SHORT_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "3", allowBlanks = true, acceptorRegex = new System.Text.RegularExpressions.Regex(@"^[A-Za-z0-9]+$") });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Description, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true, acceptorRegex = new System.Text.RegularExpressions.Regex(@"^[ A-Za-z0-9@$:.\\]+$") });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ASSEMBLY_PATH, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = new System.Text.RegularExpressions.Regex(@"^[ \w@$:.\\/]+$") });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CLASS_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = new System.Text.RegularExpressions.Regex(@"^[ \w@$:.\\/]+$") });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.VERSION, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = new System.Text.RegularExpressions.Regex(@"^[ A-Za-z0-9@$:.\\/]+$"), rejectorRegex = new System.Text.RegularExpressions.Regex(@"^[_]+$") });
              
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Primary_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.AUDIT, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "500", lstPossibleVal = new List<string>() { SM_Downstream_Systems_Audit_Level.Security_Level, SM_Downstream_Systems_Audit_Level.Report_Attribute_Level } });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REQUIRE_AUTOMATIC_POSTING_OF_UNDERLIER, dataTypeName = DataTypeName.BIT });

                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DEFAULT_SELECTED, dataTypeName = DataTypeName.BIT });

                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.GROUPS, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });

                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.USERS, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });


                cmnSheet1.lstPrimaryAttr.Add(SM_ColumnNames.SYSTEM_NAME);
                this.lstCommonSheetInfo.Add(cmnSheet1);
                #endregion sheet1
                #region sheet2

                CommonSheetInfo cmnSheet2 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet2.sheetName = SM_DownstreamSystem_SheetNames.Mapping;

                cmnSheet2.allowMultiplesAgainstPrimary = true;

                //List of Column Infos
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SYSTEM_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REPORT_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });

                cmnSheet2.lstPrimaryAttr.Add(SM_ColumnNames.SYSTEM_NAME);
                this.lstCommonSheetInfo.Add(cmnSheet2);
                #endregion sheet2

            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForDownstreamSystemsSM -> Error : " + ex.Message.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForDownstreamSystemsSM -> End");
            }
        }
        private void populateStaticDataForRMDownstreamSystems()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForRMDownstreamSystems -> Start");
            try
            {
                #region sheet1
                CommonSheetInfo cmnSheet1 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet1.sheetName = SM_DownstreamSystem_SheetNames.Definition;

                //List of Column Infos
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SYSTEM_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });

                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Description, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ASSEMBLY_PATH, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CLASS_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.VERSION, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "100" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REQUIRE_REPORT_ATTRIBUTE_LEVEL_AUDIT, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "50",
                    lstPossibleVal =
                    new List<string>() {
                        
                                                          "YES","NO","TRUE","FALSE" }
                }
               );

                cmnSheet1.lstPrimaryAttr.Add(SM_ColumnNames.SYSTEM_NAME);
                this.lstCommonSheetInfo.Add(cmnSheet1);
                #endregion sheet1
                #region sheet2

                CommonSheetInfo cmnSheet2 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet2.sheetName = SM_DownstreamSystem_SheetNames.Mapping;

                cmnSheet2.allowMultiplesAgainstPrimary = true;

                //List of Column Infos
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SYSTEM_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REPORT_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REPORT_ATTRIBUTE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                //cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ENTITY_TYPE_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });

                cmnSheet2.lstPrimaryAttr.Add(SM_ColumnNames.SYSTEM_NAME);
                // cmnSheet2.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.SYSTEM_NAME } });
                // cmnSheet2.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_ColumnNames.enti } });

                this.lstCommonSheetInfo.Add(cmnSheet2);
                #endregion sheet2

            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForRMDownstreamSystems -> Error : " + ex.Message.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForRMDownstreamSystems -> End");
            }
        }
        #region Transport Tasks
        private void populateStaticDataForSMTransportTask()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMTransportTask -> Start");
            try
            {
                #region sheet1
                CommonSheetInfo cmnSheet1 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet1.sheetName = SM_TransportTask_SheetNames.Definition;
                cmnSheet1.allowMultiplesAgainstPrimary = true;
                //List of Column Infos
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Task_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", acceptorRegex = new System.Text.RegularExpressions.Regex(@"^[ \w.-]+$") });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Description, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = true });//@"^[ \w@$:()*&!#?/\.,;[]{}']+$"
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.TRANSPORT_TYPE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REMOTE_FILE_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REMOTE_FILE_LOCATION, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.LOCAL_FILE_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.LOCAL_FILE_LOCATION, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.USE_DEFAULT_PATH, dataTypeName = DataTypeName.BIT });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EXTRACT_ALL_FILES, dataTypeName = DataTypeName.BIT });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.PGP_KEY_USERNAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "100", allowBlanks = true });

                // cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.USER_NAME, dataTypeName = DataTypeName.BIT });

                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.PGP_KEY_PASSPHRASE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "100", allowBlanks = true });

                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FILE_DATE_TYPE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "100", allowBlanks = true });
                //cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FILE_DATE_DAYS, dataTypeName = DataTypeName.INT, dataTypeLength = "4", allowBlanks = true });

                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CUSTOM_VALUE_FILE_DATE_TYPE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "100", allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.STATE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "3", lstPossibleVal = new List<string>() { "On", "Off" } });

                cmnSheet1.lstUniqueKeys.Add(new SRMMigrationUniqueKeys
                {
                    lstUniqueColumns = new List<string>() { SM_ColumnNames.TASK_NAME, SM_ColumnNames.TRANSPORT_TYPE, SM_ColumnNames.REMOTE_FILE_NAME,
                    SM_ColumnNames.REMOTE_FILE_LOCATION, SM_ColumnNames.LOCAL_FILE_NAME }
                });

                cmnSheet1.lstPrimaryAttr.Add(SM_ColumnNames.TASK_NAME);
                this.lstCommonSheetInfo.Add(cmnSheet1);
                #endregion sheet1
                #region sheet2

                CommonSheetInfo cmnSheet2 = new CommonSheetInfo();
                cmnSheet2.allowMultiplesAgainstPrimary = true;
                //Sheet Name
                cmnSheet2.sheetName = SM_TransportTask_SheetNames.CustomClasses;

                //List of Column Infos
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Task_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", acceptorRegex = new System.Text.RegularExpressions.Regex(@"^[ \w.-]+$") });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.TRANSPORT_TYPE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REMOTE_FILE_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REMOTE_FILE_LOCATION, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000" });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.LOCAL_FILE_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CALL_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "50", lstPossibleVal = new List<string>() { "PRE", "POST" } });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CLASS_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "50", lstPossibleVal = new List<string>() { "Custom Class", "Script Executable" } });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SCRIPT_OR_CLASS_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "100", acceptorRegex = new System.Text.RegularExpressions.Regex(@"^[\w.-]+$") });

                // cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.USER_NAME, dataTypeName = DataTypeName.BIT });

                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SEQUENCE_NUMBER, dataTypeName = DataTypeName.INT, dataTypeLength = "3" });

                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ASSEMBLY_PATH, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true, acceptorRegex = new System.Text.RegularExpressions.Regex(@"^[ \w.-:\\]+$") });
                // cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CUSTOM_VALUE_FILE_DATA_TYPE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });
                // cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.STATE, dataTypeName = DataTypeName.BIT });

                cmnSheet2.lstPrimaryAttr.AddRange(new List<string>() { SM_ColumnNames.TASK_NAME, SM_ColumnNames.TRANSPORT_TYPE, SM_ColumnNames.REMOTE_FILE_NAME,
                    SM_ColumnNames.REMOTE_FILE_LOCATION
                });
                cmnSheet2.lstUniqueKeys.Add(new SRMMigrationUniqueKeys { lstUniqueColumns = new List<string>() { SM_ColumnNames.SEQUENCE_NUMBER } });
                cmnSheet2.lstUniqueKeys.Add(new SRMMigrationUniqueKeys
                {
                    lstUniqueColumns = new List<string>() { SM_ColumnNames.TASK_NAME, SM_ColumnNames.TRANSPORT_TYPE, SM_ColumnNames.REMOTE_FILE_NAME,
                    SM_ColumnNames.REMOTE_FILE_LOCATION, SM_ColumnNames.LOCAL_FILE_NAME, SM_ColumnNames.SCRIPT_OR_CLASS_NAME }
                });
                this.lstCommonSheetInfo.Add(cmnSheet2);
                #endregion sheet2

            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForSMTransportTask -> Error : " + ex.Message.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMTransportTask -> End");
            }
        }
        private void populateStaticDataForRMTransportTask()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForRMTransportTask -> Start");
            try
            {
                #region sheet1
                CommonSheetInfo cmnSheet1 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet1.sheetName = SM_TransportTask_SheetNames.Definition;

                //List of Column Infos
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Task_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Description, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.TRANSPORT_TYPE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REMOTE_FILE_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REMOTE_FILE_LOCATION, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.LOCAL_FILE_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.LOCAL_FILE_LOCATION, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.USE_DEFAULT_PATH, dataTypeName = DataTypeName.BIT });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EXTRACT_ALL_FILES, dataTypeName = DataTypeName.BIT });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.GPG_KEY_USERNAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "100", allowBlanks = true });

                // cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.USER_NAME, dataTypeName = DataTypeName.BIT });

                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.GPG_KEY_PASSPHRASE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "100", allowBlanks = true });

                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FILE_DATE_TYPE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "100" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CUSTOM_VALUE_FILE_DATE_TYPE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "100", allowBlanks = true });

                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.STATE, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "3", lstPossibleVal = new List<string>() { "On", "Off" } });

                cmnSheet1.allowMultiplesAgainstPrimary = true;

                cmnSheet1.lstPrimaryAttr.Add(SM_ColumnNames.TASK_NAME);
                cmnSheet1.lstUniqueKeys.Add(new SRMMigrationUniqueKeys
                {
                    lstUniqueColumns = new List<string>() { SM_ColumnNames.TASK_NAME, SM_ColumnNames.TRANSPORT_TYPE, SM_ColumnNames.REMOTE_FILE_NAME,
                    SM_ColumnNames.REMOTE_FILE_LOCATION, SM_ColumnNames.LOCAL_FILE_NAME, SM_ColumnNames.STATE }
                });

                this.lstCommonSheetInfo.Add(cmnSheet1);
                #endregion sheet1
                #region sheet2

                CommonSheetInfo cmnSheet2 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet2.sheetName = SM_TransportTask_SheetNames.CustomClasses;

                //List of Column Infos
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Task_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.TRANSPORT_TYPE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REMOTE_FILE_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REMOTE_FILE_LOCATION, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000" });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.LOCAL_FILE_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CALL_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "50", lstPossibleVal = new List<string>() { "PRE", "POST" } });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CLASS_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "50", lstPossibleVal = new List<string>() { "Custom Class", "Script Executable" } });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SCRIPT_OR_CLASS_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "100" });

                // cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.USER_NAME, dataTypeName = DataTypeName.BIT });

                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SEQUENCE_NUMBER, dataTypeName = DataTypeName.INT, dataTypeLength = "4" });

                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ASSEMBLY_PATH, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                // cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CUSTOM_VALUE_FILE_DATA_TYPE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });
                // cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.STATE, dataTypeName = DataTypeName.BIT });

                cmnSheet2.allowMultiplesAgainstPrimary = true;
                cmnSheet2.lstPrimaryAttr.AddRange(new List<string>() { SM_ColumnNames.TASK_NAME, SM_ColumnNames.TRANSPORT_TYPE, SM_ColumnNames.REMOTE_FILE_NAME,
                    SM_ColumnNames.REMOTE_FILE_LOCATION
                });
                cmnSheet2.lstUniqueKeys.Add(new SRMMigrationUniqueKeys { lstUniqueColumns = new List<string>() { SM_ColumnNames.SEQUENCE_NUMBER } });
                cmnSheet2.lstUniqueKeys.Add(new SRMMigrationUniqueKeys
                {
                    lstUniqueColumns = new List<string>() { SM_ColumnNames.TASK_NAME, SM_ColumnNames.TRANSPORT_TYPE, SM_ColumnNames.REMOTE_FILE_NAME,
                    SM_ColumnNames.REMOTE_FILE_LOCATION, SM_ColumnNames.LOCAL_FILE_NAME, SM_ColumnNames.SCRIPT_OR_CLASS_NAME }
                });

                this.lstCommonSheetInfo.Add(cmnSheet2);
                #endregion sheet2

            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForRMTransportTask -> Error : " + ex.Message.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForRMTransportTask -> End");
            }
        }
        #endregion Transport Tasks

        private void populateStaticDataForRMTimeSeriesTasks()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForRMTimeSeriesTasks -> Start");
            try
            {
                #region Task Setup
                CommonSheetInfo cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_TimeSeriesTask_SheetNames.Definition;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Primary_Attribute, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Feed_File_Type, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Date_Format, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.File_Path, dataTypeName = DataTypeName.VARCHAR });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Entity_Type_Name);

                cmnSheet.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForRMTimeSeriesTasks -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForRMTimeSeriesTasks -> End");
            }
        }

        private void populateStaticDataForRMModeler()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForRMModeler -> Start");
            try
            {
                List<string> DataTypePossibleValues = new RMCommonController().GetAttributeDataTypes();

                List<string> possiblePageIden = CommonDALWrapper.ExecuteSelectQuery("select page_identifier from IVPRefMaster.dbo.ivp_refm_configuration_master", ConnectionConstants.RefMaster_Connection).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["page_identifier"])).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                List<string> possibleFuncIden = CommonDALWrapper.ExecuteSelectQuery("select functionality_identifier from IVPRefMaster.dbo.ivp_refm_configuration_master", ConnectionConstants.RefMaster_Connection).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["functionality_identifier"])).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                //List<string> allConfiguredTransports = new RTransportService().GetAllQueueTransportName();
                //allConfiguredTransports.Add(",");
                List<string> possibleActionLevels = CommonDALWrapper.ExecuteSelectQuery("select level_type_name from IVPRefMaster.dbo.ivp_refm_event_level_type", ConnectionConstants.RefMaster_Connection).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["level_type_name"])).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                //List<string> lstPossibleLevels = new List<string>();
                //if (possibleActionLevels.Contains("INSTRUMENT",StringComparer.OrdinalIgnoreCase))
                possibleActionLevels.Add("ENTITY");
                //List<string> possibleActionTypes = CommonDALWrapper.ExecuteSelectQuery("select action_type_name from IVPRefMaster.dbo.ivp_refm_event_action_type", ConnectionConstants.RefMaster_Connection).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["action_type_name"])).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                //possibleActionTypes.Add(",");


                #region RM_Entity_Type_Modeler_Sheet1_Definition
                CommonSheetInfo cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_EntityTypeModeler_SheetNames.Definition;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Tags, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Allowed_Users, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lookupType = LookupType.RAD_USERS, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Allowed_Groups, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lookupType = LookupType.RAD_GROUPS, allowBlanks = true });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Entity_Type_Name);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Entity_Type_Modeler_Sheet2_MasterAttributes
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_EntityTypeModeler_SheetNames.Master_Attributes;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Attribute_Description, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Data_Type, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = DataTypePossibleValues });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Attribute_Length, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Lookup_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.SEC_OR_REF, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Lookup_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.SEC_OR_REF, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Lookup_Display_Attributes, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Default_Value, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Search_View_Position, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_Mandatory, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Visible_In_Search, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_Cloneable, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_Encrypted, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_PII, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Display_Meta_Data, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Tags, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Restricted_Characters, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Show_Entity_Code, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Order_By_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Comma_Formatting, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Show_Percentage, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Show_Multiple, dataTypeName = DataTypeName.BIT, allowBlanks = true });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Entity_Type_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Attribute_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Data_Type);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Entity_Type_Modeler_Sheet3_Legs_Configuration
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_EntityTypeModeler_SheetNames.Legs_Configuration;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Entity_Type_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Leg_Name);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Entity_Type_Modeler_Sheet4_Leg_Attributes
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_EntityTypeModeler_SheetNames.Leg_Attributes;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Attribute_Description, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Data_Type, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = DataTypePossibleValues });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Attribute_Length, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Lookup_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.SEC_OR_REF, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Lookup_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.SEC_OR_REF, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Lookup_Display_Attributes, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Default_Value, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_Mandatory, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_Cloneable, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_Primary, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_Encrypted, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_PII, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Display_Meta_Data, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Tags, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Restricted_Characters, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Show_Entity_Code, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Order_By_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Comma_Formatting, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Show_Percentage, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Show_Multiple, dataTypeName = DataTypeName.BIT, allowBlanks = true });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Entity_Type_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Attribute_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Leg_Name);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Entity_Type_Modeler_Sheet5_Unique_Keys
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_EntityTypeModeler_SheetNames.Unique_Keys;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Level, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Key_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_Across_Entities, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { "Yes", "No" } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Check_In_Drafts, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { "Yes", "No" } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Check_In_Workflows, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { "Yes", "No" } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Consider_Null_As_Unique, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { "Yes", "No" } });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Entity_Type_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Level);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Key_Name);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Entity_Type_Modeler_Sheet6_Attribute_Rules
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_EntityTypeModeler_SheetNames.Attribute_Rules;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = RM_ColumnNames.Rule_Type,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    dataTypeLength = "200",
                    lstPossibleVal = new List<string>()
                    {
                        RM_Modeler_RuleTypes.Alert,
                        RM_Modeler_RuleTypes.Calculated_Field,
                        RM_Modeler_RuleTypes.Validation
                    }
                });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Rule_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Priority, dataTypeName = DataTypeName.INT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Rule_State, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Rule_Text, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", isCaseSensitive = true });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Entity_Type_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Rule_Type);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Attribute_Name);
                cmnSheet.allowMultiplesAgainstPrimary = true;

                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { RM_ColumnNames.Rule_Name } });
                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { RM_ColumnNames.Priority } });

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Entity_Type_Modeler_Sheet7_Basket_Rules
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_EntityTypeModeler_SheetNames.Basket_Rules;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = RM_ColumnNames.Rule_Type,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    dataTypeLength = "200",
                    lstPossibleVal = new List<string>()
                    {
                        RM_Modeler_RuleTypes.Group_Validations
                    }
                });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Rule_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Priority, dataTypeName = DataTypeName.INT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Rule_State, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Rule_Text, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", isCaseSensitive = true });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Entity_Type_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Rule_Type);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Leg_Name);
                cmnSheet.allowMultiplesAgainstPrimary = true;

                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { RM_ColumnNames.Rule_Name } });
                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { RM_ColumnNames.Priority } });

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Entity_Type_Modeler_Sheet7_Layouts
                cmnSheet = new CommonSheetInfo();
                //Sheet Name
                cmnSheet.sheetName = RM_EntityTypeModeler_SheetNames.Layouts;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Layout_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Dependent_Name, dataTypeName = DataTypeName.ENUM_VARCHAR, lookupType = LookupType.RAD_USERS_OR_GROUPS, lookupColumnName = RM_ColumnNames.Layout_Type });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = RM_ColumnNames.Layout_Type,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    dataTypeLength = "50",
                    lstPossibleVal = new List<string>()
                    {
                        RM_Layout_Type.System,
                        RM_Layout_Type.User,
                        RM_Layout_Type.Group
                    }
                });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = RM_ColumnNames.Entity_States,
                    dataTypeName = DataTypeName.VARCHAR,
                    dataTypeLength = "-1"
                });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Entity_Type_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Layout_Name);


                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Entity_Type_Modeler_Sheet_Layout_Preferences

                cmnSheet = new CommonSheetInfo();
                //Sheet Name
                cmnSheet.sheetName = RM_EntityTypeModeler_SheetNames.User_Group_Layout_Priority;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.User_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Group_Level_Layout, dataTypeName = DataTypeName.VARCHAR });


                cmnSheet.allowMultiplesAgainstPrimary = true;
                cmnSheet.lstPrimaryAttr = new List<string>() { RM_ColumnNames.Entity_Type_Name };
                SRMMigrationUniqueKeys key = new SRMMigrationUniqueKeys();
                key.lstUniqueColumns = new List<string>() { RM_ColumnNames.Entity_Type_Name, RM_ColumnNames.User_Name, RM_ColumnNames.Group_Level_Layout };
                cmnSheet.lstUniqueKeys = new List<SRMMigrationUniqueKeys>() { key };
                this.lstCommonSheetInfo.Add(cmnSheet);

                #endregion

                #region RM_Entity_Type_Modeler_Sheet8_Tab_Management
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_EntityTypeModeler_SheetNames.Tab_Management;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Layout_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Tab_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Tab_Order, dataTypeName = DataTypeName.INT });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Entity_Type_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Layout_Name);
                cmnSheet.allowMultiplesAgainstPrimary = true;

                cmnSheet.lstUniqueKeys = new List<SRMMigrationUniqueKeys>() { new SRMMigrationUniqueKeys() { lstUniqueColumns = new List<string>() { RM_ColumnNames.Tab_Name } } ,
                                        new SRMMigrationUniqueKeys() { lstUniqueColumns = new List<string>() { RM_ColumnNames.Tab_Order } }};

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Entity_Type_Modeler_Sheet9_Attribute_Management
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_EntityTypeModeler_SheetNames.Attribute_Management;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Layout_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Tab_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = RM_ColumnNames.Panel,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>()
                    {
                        RM_Layout_Panel.Center,
                        RM_Layout_Panel.Left,
                        RM_Layout_Panel.Right
                    }
                });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Order, dataTypeName = DataTypeName.INT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_Visible, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_Mandatory, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_Read_Only, dataTypeName = DataTypeName.BIT });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Entity_Type_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Layout_Name);
                cmnSheet.allowMultiplesAgainstPrimary = true;

                cmnSheet.lstUniqueKeys = new List<SRMMigrationUniqueKeys>() { new SRMMigrationUniqueKeys() { lstUniqueColumns = new List<string>() { RM_ColumnNames.Attribute_Name } },
                  new SRMMigrationUniqueKeys() { lstUniqueColumns = new List<string>() { RM_ColumnNames.Tab_Name, RM_ColumnNames.Panel, RM_ColumnNames.Order } } };

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Entity_Type_Modeler_Sheet10_Leg_Order
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_EntityTypeModeler_SheetNames.Leg_Order;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Layout_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Display_Order, dataTypeName = DataTypeName.INT });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Entity_Type_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Layout_Name);
                //cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Leg_Name);

                cmnSheet.lstUniqueKeys = new List<SRMMigrationUniqueKeys>(){ new SRMMigrationUniqueKeys() { lstUniqueColumns = new List<string>() { RM_ColumnNames.Leg_Name } },
                new SRMMigrationUniqueKeys() { lstUniqueColumns = new List<string>() { RM_ColumnNames.Display_Order } }};

                cmnSheet.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Entity_Type_Modeler_Sheet11_Attribute_Configuration
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_EntityTypeModeler_SheetNames.Attribute_Configuration;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = RM_ColumnNames.Page_Identifier,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    dataTypeLength = "200",
                    lstPossibleVal = possiblePageIden
                });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = RM_ColumnNames.Functionality_Identifier,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    dataTypeLength = "200",
                    lstPossibleVal = possibleFuncIden
                });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Entity_Type_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Page_Identifier);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Functionality_Identifier);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Entity_Type_Modeler_Sheet11_Exception_Preference
                {
                    cmnSheet = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet.sheetName = RM_EntityTypeModeler_SheetNames.Exception_Preferences;

                    //List of Column Infos
                    cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.ALERTS, dataTypeName = DataTypeName.BIT });
                    cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Duplicates, dataTypeName = DataTypeName.BIT });
                    cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.VENDOR_VALUE_MISSING, dataTypeName = DataTypeName.BIT });
                    cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.INVALID_DATA, dataTypeName = DataTypeName.BIT });
                    cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.NO_VENDOR_VALUE, dataTypeName = DataTypeName.BIT });
                    cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.REF_DATA_MISSING, dataTypeName = DataTypeName.BIT });
                    cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.SHOW_AS_EXCEPTION, dataTypeName = DataTypeName.BIT });
                    cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.VALIDATION, dataTypeName = DataTypeName.BIT });
                    cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.VALUE_CHANGED, dataTypeName = DataTypeName.BIT });
                    cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.VENDOR_MISMATCH, dataTypeName = DataTypeName.BIT });

                    this.lstCommonSheetInfo.Add(cmnSheet);

                    cmnSheet.lstPrimaryAttr = new List<string>() { RM_ColumnNames.Entity_Type_Name };

                }
                #endregion
                #region RM_Entity_Type_Modeler_Sheet12_Action_Notifications
                {
                    cmnSheet = new CommonSheetInfo();

                    //Sheet Name
                    cmnSheet.sheetName = RM_EntityTypeModeler_SheetNames.Action_Notifications;

                    //List of Column Infos
                    cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                    cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                    cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                    cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Queues, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
                    cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Actions, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
                    cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Action_Level, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "500", lstPossibleVal = possibleActionLevels });

                    this.lstCommonSheetInfo.Add(cmnSheet);

                    cmnSheet.lstPrimaryAttr = new List<string>() { RM_ColumnNames.Entity_Type_Name, RM_ColumnNames.Attribute_Name, RM_ColumnNames.Leg_Name, RM_ColumnNames.Action_Level };

                }
                #endregion

            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForRMModeler -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForRMModeler -> End");
            }

        }

        private void populateStaticDataForSRMDownstreamSync()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForDownstreamSync -> Start");
            try
            {
                #region Downstream_Sync_Sheet1_Downstream_Sync_Setup
                CommonSheetInfo cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = SRM_DownstreamSync_SheetNames.Downstream_Sync_Setup;
                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Setup_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Connection_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_DownstreamSync_ColumnNames.Calendar,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = SRMCommonRAD.GetCalendarNames().ToList(),
                    allowBlanks = false
                });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Effective_From_Date, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = false });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(SRM_DownstreamSync_ColumnNames.Setup_Name);
                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion


                #region RM_Reports_Sheet2_Downstream_Sync_Configuration
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = SRM_DownstreamSync_SheetNames.Downstream_Sync_Configuration;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Setup_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Block_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Report_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Module, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "100", allowBlanks = false });


                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_DownstreamSync_ColumnNames.Start_Date,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SM_DownstreamSyncReports_DateType.None,
                                                          //SM_DownstreamSyncReports_DateType.Today,
                                                          //SM_DownstreamSyncReports_DateType.Yesterday,
                                                          //SM_DownstreamSyncReports_DateType.LastBusinessDay,
                                                          //SM_DownstreamSyncReports_DateType.T_Minus_N,
                                                          //SM_DownstreamSyncReports_DateType.Custom,
                                                          //SM_DownstreamSyncReports_DateType.Now,
                                                          //SM_DownstreamSyncReports_DateType.FirstBusinessDayOfMonth,
                                                          //SM_DownstreamSyncReports_DateType.FirstBusinessDayOfYear,
                                                          //SM_DownstreamSyncReports_DateType.LastBusinessDayOfMonth,
                                                          //SM_DownstreamSyncReports_DateType.LastBusinessDayOfYear,
                                                          //SM_DownstreamSyncReports_DateType.LastBusinessDayOfPreviousMonth_Plus_N,
                                                          //SM_DownstreamSyncReports_DateType.LastBusinessDayOfPreviousYear_Plus_N,
                                                          //SM_DownstreamSyncReports_DateType.FirstBusinessDayOfMonth_Plus_N,
                                                          //SM_DownstreamSyncReports_DateType.FirstBusinessDayOfYear_Plus_N,
                                                          SM_DownstreamSyncReports_DateType.LastExtractionDate
                                                        },
                    allowBlanks = true
                });

                //cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Custom_Start_Date, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });

                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_DownstreamSync_ColumnNames.End_Date,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() {
                        //SM_DownstreamSyncReports_DateType.None,
                    //                                      SM_DownstreamSyncReports_DateType.Today,
                    //                                      SM_DownstreamSyncReports_DateType.Yesterday,
                    //                                      SM_DownstreamSyncReports_DateType.LastBusinessDay,
                    //                                      SM_DownstreamSyncReports_DateType.T_Minus_N,
                    //                                      SM_DownstreamSyncReports_DateType.Custom,
                                                          SM_DownstreamSyncReports_DateType.Now,
                                                          //SM_DownstreamSyncReports_DateType.FirstBusinessDayOfMonth,
                                                          //SM_DownstreamSyncReports_DateType.FirstBusinessDayOfYear,
                                                          //SM_DownstreamSyncReports_DateType.LastBusinessDayOfMonth,
                                                          //SM_DownstreamSyncReports_DateType.LastBusinessDayOfYear,
                                                          //SM_DownstreamSyncReports_DateType.LastBusinessDayOfPreviousMonth_Plus_N,
                                                          //SM_DownstreamSyncReports_DateType.LastBusinessDayOfPreviousYear_Plus_N,
                                                          //SM_DownstreamSyncReports_DateType.FirstBusinessDayOfMonth_Plus_N,
                                                          //SM_DownstreamSyncReports_DateType.FirstBusinessDayOfYear_Plus_N,
                                                          //SM_DownstreamSyncReports_DateType.LastExtractionDate
                                                        },
                    allowBlanks = true
                });

                //cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Custom_End_Date, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Table_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Batch_Size, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Require_Time_in_TS_Report, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Require_Knowledge_Date_Reporting, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Require_Deleted_Asset_Types, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Require_Lookup_Massaging_Start_Date, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Require_Lookup_Massaging_Current_Knowledge_Date, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.CC_Assembly_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.CC_Class_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.CC_Method_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_DownstreamSync_ColumnNames.Queue_Name,
                    dataTypeName = DataTypeName.VARCHAR,
                    lstPossibleVal = RTransportConfigLoader.GetAllTransports().Tables[0].AsEnumerable()
                                                      .Where(r => r.Field<string>("transport_type_name").Equals("MSMQ", StringComparison.OrdinalIgnoreCase) || r.Field<string>("transport_type_name").Equals("WMQ", StringComparison.OrdinalIgnoreCase) || r.Field<string>("transport_type_name").Equals("RabbitMQ", StringComparison.OrdinalIgnoreCase) || r.Field<string>("transport_type_name").Equals("KafkaMQ", StringComparison.OrdinalIgnoreCase))
                                                      .Select(row => row.Field<string>("transport_name")).ToList(),

                    allowBlanks = true
                });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Failure_Email_Id, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });


                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(SRM_DownstreamSync_ColumnNames.Setup_Name);
                cmnSheet.lstPrimaryAttr.Add(SRM_DownstreamSync_ColumnNames.Report_Name);
                cmnSheet.lstPrimaryAttr.Add(SRM_DownstreamSync_ColumnNames.Block_Type_Name);
                cmnSheet.lstPrimaryAttr.Add(SRM_DownstreamSync_ColumnNames.Module);

                this.lstCommonSheetInfo.Add(cmnSheet);

                #endregion

                #region Downstream_sync_Sheet3_Downstream_Sync_SchedulerCommonSheetInfo cmnSheet = new CommonSheetInfo();
                cmnSheet = new CommonSheetInfo();
                //Sheet Name
                cmnSheet.sheetName = SRM_DownstreamSync_SheetNames.Downstream_Sync_Scheduler;
                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Setup_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Recurrence_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Recurrence_Pattern, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Interval, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Number_of_Recurrences, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Time_Interval_of_Recurrence, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Days_Of_Week, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Never_End_Job, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Start_Date, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.End_Date, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_DownstreamSync_ColumnNames.Start_Time, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(SRM_DownstreamSync_ColumnNames.Setup_Name);
                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForDownstreamSyncReports -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForDownstreamSyncReports -> End");
            }
        }
        private void populateStaticDataForSRMWorkFlowModeler(int moduleId)
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSRMWorkFlowModeler -> Start");
            try
            {
                #region SRM_Workflow_Sheet1_Workflows_SheetDetails
                CommonSheetInfo cmnSheet1 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet1.sheetName = SRM_WorkFlow_SheetNames.Workflows;

                //List of Column Infos
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Workflow_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Workflow_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = false });

                //List of Primary Column Names
                cmnSheet1.lstPrimaryAttr.Add(SRM_WorkFlow_ColumnNames.Workflow_Name);
                cmnSheet1.allowMultiplesAgainstPrimary = false;

                this.lstCommonSheetInfo.Add(cmnSheet1);
                #endregion

                #region SRM_Workflow_Sheet2_Workflow_Mapping_SheetDetails
                CommonSheetInfo cmnSheet2 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet2.sheetName = SRM_WorkFlow_SheetNames.Workflow_Mapping;
                //List of Column Infos
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Workflow_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                if (moduleId == 3)
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Security_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = false });
                else
                    cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Entity_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = false });


                //List of Primary Column Names
                if (moduleId == 3)
                {
                    cmnSheet2.lstPrimaryAttr.Add(SRM_WorkFlow_ColumnNames.Workflow_Name);
                    cmnSheet2.allowMultiplesAgainstPrimary = true;
                }
                else
                {
                    cmnSheet2.lstPrimaryAttr.Add(SRM_WorkFlow_ColumnNames.Workflow_Name);
                    cmnSheet2.allowMultiplesAgainstPrimary = false;
                }

                this.lstCommonSheetInfo.Add(cmnSheet2);
                #endregion

                #region SRM_Workflow_Sheet3_Workflow_Mapping_SheetDetails
                CommonSheetInfo cmnSheet3 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet3.sheetName = SRM_WorkFlow_SheetNames.Workflow_Inbox_Attributes;
                //List of Column Infos
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Workflow_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = false });
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_WorkFlow_ColumnNames.Is_Primary_Attribute,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SRM_WorkFlow_TF_Type.True,
                                                          SRM_WorkFlow_TF_Type.False
                                                        },
                    allowBlanks = false
                });

                //List of Primary Column Names
                cmnSheet3.lstPrimaryAttr.Add(SRM_WorkFlow_ColumnNames.Workflow_Name);
                cmnSheet3.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet3);
                #endregion

                #region SRM_Workflow_Sheet4_Workflow_Template_Mapping_SheetDetails
                CommonSheetInfo cmnSheet4 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet4.sheetName = SRM_WorkFlow_SheetNames.Workflow_Template_Mapping;
                //List of Column Infos
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Workflow_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_WorkFlow_ColumnNames.Is_Rule_Configured,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                    allowBlanks = false
                });
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_WorkFlow_ColumnNames.Is_Default_Workflow,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                    allowBlanks = false
                });
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Rule_Text, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true, isCaseSensitive = true });
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Rule_Priority, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Rad_Workflow_Template_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = false });

                //List of Primary Column Names
                cmnSheet4.lstPrimaryAttr.Add(SRM_WorkFlow_ColumnNames.Workflow_Name);
                cmnSheet4.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet4);
                #endregion

                #region SRM_Workflow_Sheet5_Workflow_Data_Validation_SheetDetails
                CommonSheetInfo cmnSheet5 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet5.sheetName = SRM_WorkFlow_SheetNames.Data_Validation_Checks;
                //List of Column Infos

                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Workflow_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_WorkFlow_ColumnNames.Is_Default_Workflow,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                    allowBlanks = false
                });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Rule_Text, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true, isCaseSensitive = true });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Rule_Priority, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Rad_Workflow_Template_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = false });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Stage, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = false });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_WorkFlow_ColumnNames.Mandatory,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                    allowBlanks = false
                });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_WorkFlow_ColumnNames.Uniqueness,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                    allowBlanks = false
                });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_WorkFlow_ColumnNames.Primary_Key,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                    allowBlanks = false
                });

                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_WorkFlow_ColumnNames.Validations,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                    allowBlanks = false
                });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_WorkFlow_ColumnNames.Alerts,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                    allowBlanks = false
                });

                if (moduleId == 3)
                {
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo()
                    {
                        columnName = SRM_WorkFlow_ColumnNames.Basket_Validations,
                        dataTypeName = DataTypeName.ENUM_VARCHAR,
                        lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                        allowBlanks = false
                    });
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo()
                    {
                        columnName = SRM_WorkFlow_ColumnNames.Basket_Alerts,
                        dataTypeName = DataTypeName.ENUM_VARCHAR,
                        lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                        allowBlanks = false
                    });
                }
                else
                {
                    cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo()
                    {
                        columnName = SRM_WorkFlow_ColumnNames.Group_Validation,
                        dataTypeName = DataTypeName.ENUM_VARCHAR,
                        lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                        allowBlanks = false
                    });
                }

                //List of Primary Column Names
                cmnSheet5.lstPrimaryAttr.Add(SRM_WorkFlow_ColumnNames.Workflow_Name);
                cmnSheet5.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet5);
                #endregion

                #region SRM_Workflow_Sheet6_Workflow_Email_Configuration_SheetDetails
                CommonSheetInfo cmnSheet6 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet6.sheetName = SRM_WorkFlow_SheetNames.Email_Configuration;
                //List of Column Infos
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Workflow_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Rule_Priority, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_WorkFlow_ColumnNames.Include_Action,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                    allowBlanks = false
                });
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Action_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = false });
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_WorkFlow_ColumnNames.Keep_Application_URL_In_Footer,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                    allowBlanks = false
                });
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_WorkFlow_ColumnNames.Send_Consolidated_Email_For_Bulk_Action,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                    allowBlanks = false
                });
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SRM_WorkFlow_ColumnNames.Keep_Creator_In_CC,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                    allowBlanks = false
                });
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.To, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "10000", allowBlanks = true });
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Subject, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "10000", allowBlanks = true });
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Bulk_Subject, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "10000", allowBlanks = true });
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.Mail_Body_Content, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "10000", allowBlanks = true });
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SRM_WorkFlow_ColumnNames.DataSectionAttributes, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "10000", allowBlanks = true });


                cmnSheet6.lstPrimaryAttr.Add(SRM_WorkFlow_ColumnNames.Workflow_Name);
                cmnSheet6.allowMultiplesAgainstPrimary = true;
                this.lstCommonSheetInfo.Add(cmnSheet6);
                #endregion
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForSRMWorkFlowModeler -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSRMWorkFlowModeler -> End");
            }
        }


        private void populateStaticDataForRMDataSource()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForRMDataSource -> Start");
            try
            {

                #region RM_Data_Source_Definition
                CommonSheetInfo cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_DataSource_SheetNames.DataSourceAndFeedDefinition;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Data_Source, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Data_Source_Description, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Feed_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Feed_Type, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { RM_Migration_Constants.Feed_Type_Manual, RM_Migration_Constants.Feed_Type_File_Template, RM_Migration_Constants.Feed_Type_Load_From_DB } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Feed_File_Type, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { RM_Migration_Constants.File_Type_Delimited, RM_Migration_Constants.File_Type_DB, RM_Migration_Constants.File_Type_Excel, RM_Migration_Constants.File_Type_XML, RM_Migration_Constants.File_Type_Fixed_Width } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Record_Delimiter, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "4", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Record_Length, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Field_Delimiter, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "1" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Comment_Char, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "50" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Single_Escape, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "50" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Paired_Escape, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "50" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Exclude_Regex, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "3000" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Root_XPath, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Record_XPath, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Server_Type, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { "SQL", "Oracle" }, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Loading_Criteria, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Connection_String, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Data_Source);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Feed_Name);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Data_Source_Feed_Fields
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_DataSource_SheetNames.Feed_Fields;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Data_Source, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Feed_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Feed_Field_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Feed_Field_Description, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Start_Index, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.End_Index, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Position, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Mandatory, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Persistable, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Allow_Trim, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.X_Path, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "100" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Remove_White_Spaces, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_API, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_FTP, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_Bulk, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_Encrypted, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_Primary, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_Unique, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_PII, dataTypeName = DataTypeName.BIT });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Data_Source);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Feed_Name);

                cmnSheet.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Data_Source_Feed_Mapping
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_DataSource_SheetNames.Feed_Mapping;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Data_Source, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Feed_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Mapped_Column_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Primary_Column_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Map_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Map_State, dataTypeName = DataTypeName.BIT });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Data_Source);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Feed_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Mapped_Column_Name);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Data_Source_Feed_Rules
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_DataSource_SheetNames.Feed_Rules;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Feed_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Data_Source, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Rule_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Rule_Type, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { RM_Migration_Constants.Feed_Rule_Validation, RM_Migration_Constants.Feed_Rule_Transformation } });

                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Rule_Text, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Priority, dataTypeName = DataTypeName.INT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Rule_State, dataTypeName = DataTypeName.BIT });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Data_Source);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Feed_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Rule_Type);

                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = new List<string>() { RM_ColumnNames.Rule_Name } });
                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = new List<string>() { RM_ColumnNames.Priority } });

                cmnSheet.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Data_Source_Feed_Entity_Type_Mapping
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_DataSource_SheetNames.Entity_Type_Mapping;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Feed_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Data_Source, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_Insert, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_Update, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Leg_Name, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Feed_Field_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Lookup_Type, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Lookup_Attribute, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Date_Format, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Update_Blank, dataTypeName = DataTypeName.BIT });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Data_Source);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Feed_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Entity_Type_Name);

                cmnSheet.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Data_Source_Feed_Entity_type_rules
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_DataSource_SheetNames.Entity_Type_Rules;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Feed_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Data_Source, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Rule_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Rule_Type, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { RM_Migration_Constants.Feed_Rule_Entity_Transformation, RM_Migration_Constants.Filter_Rule, RM_Migration_Constants.Request_Filter_Rule } });

                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Rule_Text, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Priority, dataTypeName = DataTypeName.INT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Rule_State, dataTypeName = DataTypeName.BIT });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Data_Source);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Feed_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Entity_Type_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Rule_Type);

                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = new List<string>() { RM_ColumnNames.Rule_Name } });
                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = new List<string>() { RM_ColumnNames.Priority } });

                cmnSheet.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Data_Source_Feed_Bulk_License_Setup
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_DataSource_SheetNames.Bulk_License_Setup;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Feed_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Data_Source, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Loading_Task_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Loading_Task_Description, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Bulk_File_Path, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Bulk_File_Date_Type, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Bulk_File_Date, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Bulk_File_Business_Days, dataTypeName = DataTypeName.INT });

                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Difference_File_Path, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Difference_File_Date_Type, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Difference_File_Date, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Difference_File_Business_Days, dataTypeName = DataTypeName.INT });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Data_Source);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Feed_Name);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Data_Source_Feed_API_License_Setup
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_DataSource_SheetNames.API_License_Setup;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Feed_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Data_Source, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Import_Task_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Import_Task_Description, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Vendor_Type, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { RM_Migration_Constants.Vendor_Type_Bloomberg, RM_Migration_Constants.Vendor_Type_Reuters } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Request_Type, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Vendor_Identifier, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Asset, dataTypeName = DataTypeName.VARCHAR });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Data_Source);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Feed_Name);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Data_Source_Feed_FTP_License_Setup
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_DataSource_SheetNames.FTP_License_Setup;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Feed_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Data_Source, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Request_Task_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Request_Task_Description, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Request_Vendor_Type, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { RM_Migration_Constants.Vendor_Type_Bloomberg, RM_Migration_Constants.Vendor_Type_Reuters } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Request_Transport_Type, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Request_Vendor_Identifier, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Request_Outgoing_FTP, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Request_Market_Sector, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Request_Data_Request_Type, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });

                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Response_Task_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Response_Task_Description, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Response_Vendor_Type, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { RM_Migration_Constants.Vendor_Type_Bloomberg, RM_Migration_Constants.Vendor_Type_Reuters } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Response_Transport_Type, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Response_Incoming_FTP, dataTypeName = DataTypeName.VARCHAR });
                //cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Response_Data_Request_Type, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Data_Source);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Feed_Name);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Data_Source_Feed_Custom_Classes
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_DataSource_SheetNames.Custom_Classes;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Feed_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Data_Source, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Loading_Task_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Call_Type, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { RM_Migration_Constants.CC_Pre_Loading, RM_Migration_Constants.CC_Post_Loading } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Class_Type, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { RM_Migration_Constants.CC_Class_Type_Script_Executable, RM_Migration_Constants.CC_Class_Type_Custom_Class } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Script_Or_Class_Name, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Assembly_Path, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Sequence_Number, dataTypeName = DataTypeName.INT });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Data_Source);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Feed_Name);
                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = new List<string>() { RM_ColumnNames.Script_Or_Class_Name } });
                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = new List<string>() { RM_ColumnNames.Sequence_Number } });

                cmnSheet.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForRMDataSource -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForRMDataSource -> End");
            }

        }

        private void populateStaticDataForSMDataSource()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMDataSource -> Start");
            try
            {
                var acceptorRegex = new Regex("^[a-zA-Z0-9 @$]+$");

                #region SM_Data_Source_Definition
                CommonSheetInfo cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = SM_DataSource_SheetNames.DATA_SOURCES;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DATA_SOURCE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DATA_SOURCE_DESCRIPTION, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "1000" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DATA_SOURCE_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "Data Provider", "Internal" } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FEED_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FEED_SOURCE_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { "Load From DB", "Manual Setup", "File Template", "Bloomberg Bulk List" } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.BLOOMBERG_LICENSE_CATEGORY, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = SMDataSourceController.GetBloombergLicenseCategory(), allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FILE_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "Fixed Width", "Delimited", "Xml", "Excel" }, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.RECORD_DELIMITER, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "10" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FIELD_DELIMITER, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "1" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.COMMENT_CHAR, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "50" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SINGLE_ESCAPE, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "50" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.PAIRED_ESCAPE, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "50" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EXCLUDE_REGEX, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "3000" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.RECORD_LENGTH, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ROOT_XPATH, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.RECORD_XPATH, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.BLOOMBERG_BULK_FIELD, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = SMDataSourceController.GetBloombergBulkListMapping(), allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SERVER_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { "SQL Server", "Oracle" }, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CONNECTION_STRING, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.LOADING_CRITERIA, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.DATA_SOURCE);
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.FEED_NAME);

                //cmnSheet.lstAttributesInGroupValidation.Add(SM_ColumnNames.DATA_SOURCE_DESCRIPTION);
                //cmnSheet.lstAttributesInGroupValidation.Add(SM_ColumnNames.DATA_SOURCE_TYPE);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region SM_Data_Source_Feed_Fields
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = SM_DataSource_SheetNames.FEED_FIELDS;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DATA_SOURCE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FEED_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FIELD_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "100" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FIELD_DESCRIPTION, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.IS_PRIMARY, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.START_INDEX, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.END_INDEX, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FIELD_POSITION, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FIELD_XPATH, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.MANDATORY, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.PERSISTABLE, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ALLOW_TRIM, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REMOVE_WHITE_SPACE, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.IS_BLOOMBERG_MNEMONIC, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FOR_API, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FOR_FTP, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FOR_BULK, dataTypeName = DataTypeName.BIT });

                //List of Primary Column Names
                cmnSheet.allowMultiplesAgainstPrimary = true;
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.DATA_SOURCE);
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.FEED_NAME);

                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys { lstUniqueColumns = new List<string> { SM_ColumnNames.FIELD_NAME } });

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                //done
                #region SM_Data_Source_Feed_Rules
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = SM_DataSource_SheetNames.FEED_RULES;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DATA_SOURCE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FEED_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.RULE_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { "Validation", "Transformation" } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.RULE_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.RULE_TEXT, dataTypeName = DataTypeName.VARCHAR, isCaseSensitive = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.PRIORITY, dataTypeName = DataTypeName.INT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.RULE_STATE, dataTypeName = DataTypeName.BIT });

                //List of Primary Column Names
                cmnSheet.allowMultiplesAgainstPrimary = true;
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.DATA_SOURCE);
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.FEED_NAME);
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.RULE_TYPE);

                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys { lstUniqueColumns = new List<string> { SM_ColumnNames.RULE_NAME } });
                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys { lstUniqueColumns = new List<string> { SM_ColumnNames.PRIORITY } });

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region SM_Data_Source_Feed_Security_Type_Mapping
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = SM_DataSource_SheetNames.SECURITY_TYPE_MAPPING;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DATA_SOURCE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FEED_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SECURITY_TYPE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.LEG_NAME, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ATTRIBUTE_NAME, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FIELD_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "100" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REFERENCE_ATTRIBUTE, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DATE_FORMAT, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.UPDATE_BLANK, dataTypeName = DataTypeName.BIT, allowBlanks = true });

                //List of Primary Column Names
                cmnSheet.allowMultiplesAgainstPrimary = true;
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.DATA_SOURCE);
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.FEED_NAME);
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.SECURITY_TYPE);

                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys { lstUniqueColumns = new List<string> { SM_ColumnNames.LEG_NAME, SM_ColumnNames.ATTRIBUTE_NAME } });

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                //done
                #region SM_Data_Source_Feed_Security_type_rules
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = SM_DataSource_SheetNames.SECURITY_TYPE_RULES;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DATA_SOURCE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FEED_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SECURITY_TYPE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.RULE_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { "Rule Filter", "Request Rule Filter" } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.RULE_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.RULE_TEXT, dataTypeName = DataTypeName.VARCHAR, isCaseSensitive = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.PRIORITY, dataTypeName = DataTypeName.INT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.RULE_STATE, dataTypeName = DataTypeName.BIT });

                //List of Primary Column Names
                cmnSheet.allowMultiplesAgainstPrimary = true;
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.DATA_SOURCE);
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.FEED_NAME);
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.SECURITY_TYPE);
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.RULE_TYPE);

                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys { lstUniqueColumns = new List<string> { SM_ColumnNames.RULE_NAME } });
                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys { lstUniqueColumns = new List<string> { SM_ColumnNames.PRIORITY } });

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                //done
                #region SM_Data_Source_Feed_Bulk_License_Setup
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = SM_DataSource_SheetNames.BULK_LICENSE_SETUP;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DATA_SOURCE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FEED_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.LOADING_TASK_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.LOADING_TASK_DESCRIPTION, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "1000" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.BULK_FILE_PATH, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.BULK_FILE_DATE_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = SMDataSourceController.GetFileDateTypes(), allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.BULK_FILE_DATE, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.BULK_FILE_BUSINESS_DAYS, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DIFF_FILE_PATH, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DIFF_FILE_DATE_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = SMDataSourceController.GetFileDateTypes(), allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DIFF_FILE_DATE, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DIFF_FILE_BUSINESS_DAYS, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CHECK_EXISTING_SECURITY, dataTypeName = DataTypeName.BIT, allowBlanks = true });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.DATA_SOURCE);
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.FEED_NAME);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                //done
                #region SM_Data_Source_Feed_Bulk_License_Custom_Class
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = SM_DataSource_SheetNames.BULK_LICENSE_CUSTOM_CLASS;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DATA_SOURCE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FEED_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.LOADING_TASK_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CALL_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "PRE", "POST" } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CLASS_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "Custom Class", "Script Executable" } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SCRIPT_CLASS_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "100" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ASSEMBLY_PATH, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SEQUENCE_NUMBER, dataTypeName = DataTypeName.INT });

                //List of Primary Column Names
                cmnSheet.allowMultiplesAgainstPrimary = true;
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.DATA_SOURCE);
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.FEED_NAME);

                cmnSheet.lstGroupValidations.Add(new SRMMigrationGroupValidations() { lstGVColumns = { SM_ColumnNames.FEED_NAME, SM_ColumnNames.LOADING_TASK_NAME } });
                //cmnSheet.lstAttributesInGroupValidation.Add(SM_ColumnNames.FEED_NAME);
                //cmnSheet.lstAttributesInGroupValidation.Add(SM_ColumnNames.LOADING_TASK_NAME);

                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys { lstUniqueColumns = new List<string> { SM_ColumnNames.CALL_TYPE, SM_ColumnNames.SEQUENCE_NUMBER } });//, SM_ColumnNames.CLASS_TYPE

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                //done
                #region SM_Data_Source_Feed_API_License_Setup
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = SM_DataSource_SheetNames.API_LICENSE_SETUP;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DATA_SOURCE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FEED_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.IMPORT_TASK_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.IMPORT_TASK_DESCRIPTION, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "1000" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.VENDOR_MANAGEMENT_PREFERENCE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = SMDataSourceController.GetVendorManagementPreferenceNames() });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REQUEST_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "Heavy", "Lite", "GlobalAPI" } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.VENDOR_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { "Bloomberg" } });//, "Reuters", "MarkitWSO"
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.VENDOR_IDENTIFIER, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.VENDOR_IDENTIFIER_MAPPED_ATTRIBUTE, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.MARKET_SECTOR, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "Comdty", "Corp", "Curncy", "Equity", "Govt", "Index", "MMkt", "Mtge", "Muni", "None", "Pfd" } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.MARKET_SECTOR_MAPPED_ATTRIBUTE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = SMDataSourceController.GetMarketSectorAttributes(), allowBlanks = true });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.DATA_SOURCE);
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.FEED_NAME);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                //done
                #region SM_Data_Source_Feed_FTP_License_Setup
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = SM_DataSource_SheetNames.FTP_LICENSE_SETUP;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DATA_SOURCE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FEED_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.VENDOR_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string>() { "Bloomberg" } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.VENDOR_MANAGEMENT_PREFERENCE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = SMDataSourceController.GetVendorManagementPreferenceNames() });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REQUEST_TASK_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REQUEST_TASK_DESCRIPTION, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "1000" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.VENDOR_IDENTIFIER, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.VENDOR_IDENTIFIER_MAPPED_ATTRIBUTE, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.MARKET_SECTOR, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "Comdty", "Corp", "Curncy", "Equity", "Govt", "Index", "MMkt", "Mtge", "Muni", "None", "Pfd" } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.MARKET_SECTOR_MAPPED_ATTRIBUTE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = SMDataSourceController.GetMarketSectorAttributes(), allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REQUEST_TRANSPORT_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "FTP" } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.OUTGOING_FTP, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = SMDataSourceController.GetFTPTransportNames() });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.BVAL_MAPPED_ATTRIBUTE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = SMDataSourceController.GetMarketSectorAttributes(), allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.RESPONSE_TASK_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.RESPONSE_TASK_DESCRIPTION, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "1000" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.RESPONSE_TRANSPORT_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "FTP" } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.INCOMING_FTP, dataTypeName = DataTypeName.VARCHAR, lstPossibleVal = SMDataSourceController.GetFTPTransportNames() });


                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.DATA_SOURCE);
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.FEED_NAME);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                //done
                #region SM_Data_Source_API_FTP_Bloomberg_Headers
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = SM_DataSource_SheetNames.API_FTP_BLOOMBERG_HEADERS;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DATA_SOURCE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FEED_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.LICENSE_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "API", "FTP" } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.HEADER_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.HEADER_VALUE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });

                //List of Primary Column Names
                cmnSheet.allowMultiplesAgainstPrimary = true;
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.DATA_SOURCE);
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.FEED_NAME);
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.LICENSE_TYPE);

                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys { lstUniqueColumns = new List<string> { SM_ColumnNames.HEADER_NAME } });

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                //done
                #region SM_Data_Source_Load_From_Vendor_License_Setup
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = SM_DataSource_SheetNames.LOAD_FROM_VENDOR_LICENSE_SETUP;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DATA_SOURCE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FEED_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", acceptorRegex = acceptorRegex });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.TASK_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.MASTER_DATA_SOURCE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.MASTER_FEED_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.MASTER_FEED_PRIMARY_FIELD, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "100" });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.DATA_SOURCE);
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.FEED_NAME);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForSMDataSource -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMDataSource -> End");
            }

        }

        private void populateStaticDataForSMValidationTask()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMValidationTask -> Start");
            try
            {

                #region SM_Definition
                CommonSheetInfo cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = SM_ValidationTask_SheetNames.DEFINITION;

                var endDateTypes = SMDataSourceController.GetFileDateTypes();
                var startDateTypes = endDateTypes.ToList();
                startDateTypes.Add("LastRunDate");

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.TASK_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.TASK_DESCRIPTION, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "1000" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.APPLY_VALIDATION, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.APPLY_UNIQUENESS, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.APPLY_ALERT, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DELETE_PREVIOUS_EXCEPTIONS, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DELETE_PREVIOUS_EXCEPTIONS_CONSIDERED_SECURITIES, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CALENDAR_NAME, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = SRMCommonRAD.GetCalendarNames().ToList() });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.START_DATE_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = startDateTypes });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CUSTOM_START_DATE, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.START_DATE_BUSINESS_DAYS, dataTypeName = DataTypeName.INT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.END_DATE_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = endDateTypes, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CUSTOM_END_DATE, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.END_DATE_BUSINESS_DAYS, dataTypeName = DataTypeName.INT, allowBlanks = true });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.TASK_NAME);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region SM_Selected_Security_Types
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = SM_ValidationTask_SheetNames.SECURITY_TYPE;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.TASK_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SECURITY_TYPE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });

                //List of Primary Column Names
                cmnSheet.allowMultiplesAgainstPrimary = true;
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.TASK_NAME);

                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys { lstUniqueColumns = new List<string> { SM_ColumnNames.SECURITY_TYPE } });

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForSMValidationTask -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMValidationTask -> End");
            }

        }

        private void populateStaticDataForSMReportingTask()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMReportingTask -> Start");
            try
            {

                #region SM_Definition
                CommonSheetInfo cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = SM_ValidationTask_SheetNames.DEFINITION;

                var downstreamSystems = SMDataSourceController.GetDownstreamSystems();
                var radTransports = SMDataSourceController.GetRADTransportsForReportingTask();

                var publicationTransports = radTransports.Select("transport_type_name IN ('RABBITMQ', 'KAFKAMQ', 'MSMQ')").Select(x => x.Field<string>("transport_name")).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var extractionTransports = radTransports.Select("transport_type_name IN ('WFT','FTP', 'SFTP')").Select(x => x.Field<string>("transport_name")).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                var subscriptionTransports = radTransports.Select("transport_type_name IN ('WFT')").Select(x => x.Field<string>("transport_name")).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                var endDateTypes = SMDataSourceController.GetFileDateTypes();
                var startDateTypes = endDateTypes.ToList();
                startDateTypes.Add("LastExtractionDate");
                startDateTypes.Add("LastSuccessfulPushTime");

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.TASK_NAME, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.TASK_DESCRIPTION, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "1000" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REPORTS, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.REPORT_SYSTEM, dataTypeName = DataTypeName.ENUM_VARCHAR, allowBlanks = true, lstPossibleVal = downstreamSystems });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.AUDIT_LEVEL, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "Security Level", "Report Attribute Level" } });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CALENDAR_NAME, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = SRMCommonRAD.GetCalendarNames().ToList() });

                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.START_DATE_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = startDateTypes });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CUSTOM_START_DATE, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.START_DATE_BUSINESS_DAYS, dataTypeName = DataTypeName.INT, allowBlanks = true });

                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.END_DATE_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = endDateTypes, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.CUSTOM_END_DATE, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.END_DATE_BUSINESS_DAYS, dataTypeName = DataTypeName.INT, allowBlanks = true });

                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.POST_TO_DOWNSTREAM_SYSTEM, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SEND_REAL_TIME_UPDATES, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SEND_REAL_TIME_UPDATES_IN_FLOW_TASK, dataTypeName = DataTypeName.BIT });

                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.PUBLICATION_QUEUES, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });//, lstPossibleVal = publicationTransports
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.PUBLICATION_FORMAT, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "delimited", "xml" }, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.PUBLICATION_DELIMITER, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "|", "," }, allowBlanks = true });

                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EXTRACTION_TRANSPORT_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = extractionTransports, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EXTRACTION_REMOTE_FILE_LOCATION, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EXTRACTION_REMOTE_FILE_NAME, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EXTRACTION_REMOTE_FILE_FORMAT, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "delimited", "xml", "Pdf", "Excel" }, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EXTRACTION_REMOTE_FILE_DELIMITER, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "|", "," }, allowBlanks = true });

                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EXTRACTION_REPORT_FILE_DATE_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = endDateTypes, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EXTRACTION_REPORT_CUSTOM_FILE_DATE, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EXTRACTION_REPORT_FILE_DATE_BUSINESS_DAYS, dataTypeName = DataTypeName.INT, allowBlanks = true });

                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EMAIL_IDS, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });

                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EMAIL_TRANSPORT, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = subscriptionTransports, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EMAIL_FILE_FORMAT, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "delimited", "xml", "Pdf", "Excel" }, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EMAIL_FILE_DELIMITER, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "|", "," }, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EMAIL_REPORT_FILE_NAME, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true, dataTypeLength = "200" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EMAIL_REPORT_FILE_DATE_TYPE, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = endDateTypes, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EMAIL_REPORT_CUSTOM_FILE_DATE, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.EMAIL_REPORT_FILE_DATE_BUSINESS_DAYS, dataTypeName = DataTypeName.INT, allowBlanks = true });

                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ENCODING, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = Enum.GetNames(typeof(SRMEncodingType)).ToList() });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.IGNORE_ARCHIVE_RECORDS, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.IGNORE_SECURITY_KEY, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.PROCESS_IN_BATCH, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.BATCH_SIZE, dataTypeName = DataTypeName.INT, allowBlanks = true });


                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(SM_ColumnNames.TASK_NAME);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForSMReportingTask -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMReportingTask -> End");
            }

        }

        private void populateStaticDataForSMRealtimePreference()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMRealtimePreference -> Start");
            try
            {
                #region SM_RealtimePreference_Sheet1_Preference_Setup_SheetDetails
                CommonSheetInfo cmnSheet1 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet1.sheetName = SM_RealtimePreference_SheetNames.PREFERENCE_SETUP;

                //List of Column Infos
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Vendor_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Preference_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Preference_Description, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Request_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Market_Sector, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Data_Source, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Vendor_Identifier, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Sectype_Identifier, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Transport, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_mapped_to_Market_Sector, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Create_in_Background, dataTypeName = DataTypeName.BIT });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Save_as_Draft, dataTypeName = DataTypeName.BIT });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Create_Underlier, dataTypeName = DataTypeName.BIT });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Create_constituent, dataTypeName = DataTypeName.BIT });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Check_Existing, dataTypeName = DataTypeName.BIT });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Check_Existing_attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });

                //List of Primary Column Names
                cmnSheet1.lstPrimaryAttr.Add(SM_ColumnNames.Vendor_Type);
                cmnSheet1.lstPrimaryAttr.Add(SM_ColumnNames.Preference_Name);

                this.lstCommonSheetInfo.Add(cmnSheet1);
                #endregion


                #region SM_RealtimePreference_Sheet2_Preference_Details_SheetDetails
                CommonSheetInfo cmnSheet2 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet2.sheetName = SM_RealtimePreference_SheetNames.PREFERENCE_DETAILS;

                //List of Column Infos
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Vendor_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Preference_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Mapped_Bloomberg_Security_Types, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Prefered_Attributes, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Vendor_Management_Preference, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });

                //List of Primary Column Names
                cmnSheet2.lstPrimaryAttr.Add(SM_ColumnNames.Vendor_Type);
                cmnSheet2.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type);

                this.lstCommonSheetInfo.Add(cmnSheet2);
                #endregion


                #region SM_RealtimePreference_Sheet3_Basket_Preference_SheetDetails
                CommonSheetInfo cmnSheet3 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet3.sheetName = SM_RealtimePreference_SheetNames.BASKET_PREFERENCE;

                //List of Column Infos
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Vendor_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Preference_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Basket_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Exotic_field, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Vendor_Management_Preference, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });

                //List of Primary Column Names
                cmnSheet3.lstPrimaryAttr.Add(SM_ColumnNames.Vendor_Type);
                cmnSheet3.lstPrimaryAttr.Add(SM_ColumnNames.Preference_Name);
                cmnSheet3.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type);
                cmnSheet3.lstPrimaryAttr.Add(SM_ColumnNames.Basket_Name);

                this.lstCommonSheetInfo.Add(cmnSheet3);
                #endregion


                #region SM_RealtimePreference_Sheet4_Basket_Preference_mapping_SheetDetails
                CommonSheetInfo cmnSheet4 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet4.sheetName = SM_RealtimePreference_SheetNames.BASKET_PREFERENCE_MAPPING;

                //List of Column Infos
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Vendor_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Preference_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Basket_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Field_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Mapped_Format, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Reference_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Underlying_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Update_Blank, dataTypeName = DataTypeName.BIT });

                //List of Primary Column Names
                cmnSheet4.lstPrimaryAttr.Add(SM_ColumnNames.Vendor_Type);
                cmnSheet4.lstPrimaryAttr.Add(SM_ColumnNames.Preference_Name);
                cmnSheet4.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type);
                cmnSheet4.lstPrimaryAttr.Add(SM_ColumnNames.Basket_Name);

                cmnSheet4.lstUniqueKeys.Add(new SRMMigrationUniqueKeys { lstUniqueColumns = new List<string> { SM_ColumnNames.Attribute_Name } });

                cmnSheet4.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet4);
                #endregion


                #region SM_RealtimePreference_Sheet5_Bloomberg_Headers_SheetDetails
                CommonSheetInfo cmnSheet5 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet5.sheetName = SM_RealtimePreference_SheetNames.BLOOMBERG_HEADERS;

                //List of Column Infos
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Vendor_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Preference_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Basket_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Header_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Header_Value, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });

                //List of Primary Column Names
                cmnSheet5.lstPrimaryAttr.Add(SM_ColumnNames.Vendor_Type);
                cmnSheet5.lstPrimaryAttr.Add(SM_ColumnNames.Preference_Name);
                cmnSheet5.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type);
                cmnSheet5.lstPrimaryAttr.Add(SM_ColumnNames.Basket_Name);

                cmnSheet5.lstUniqueKeys.Add(new SRMMigrationUniqueKeys { lstUniqueColumns = new List<string> { SM_ColumnNames.Header_Name } });

                cmnSheet5.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet5);
                #endregion


                #region SM_RealtimePreference_Sheet6_Underlyer_Detector_Setup_SheetDetails
                CommonSheetInfo cmnSheet6 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet6.sheetName = SM_RealtimePreference_SheetNames.UNDERLYER_DETECTOR_SETUP;

                //List of Column Infos
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Vendor_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Preference_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Basket_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Vendor_Identifier, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Market_Sector, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Is_Second_Fetch, dataTypeName = DataTypeName.BIT });
                cmnSheet6.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Fields_for_Second_Fetch, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });

                //List of Primary Column Names
                cmnSheet6.lstPrimaryAttr.Add(SM_ColumnNames.Vendor_Type);
                cmnSheet6.lstPrimaryAttr.Add(SM_ColumnNames.Preference_Name);
                cmnSheet6.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type);
                cmnSheet6.lstPrimaryAttr.Add(SM_ColumnNames.Basket_Name);

                this.lstCommonSheetInfo.Add(cmnSheet6);
                #endregion


                #region SM_RealtimePreference_Sheet7_Underlyer_Detector_Rules_SheetDetails
                CommonSheetInfo cmnSheet7 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet7.sheetName = SM_RealtimePreference_SheetNames.UNDERLYER_DETECTOR_RULES;

                //List of Column Infos
                cmnSheet7.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Vendor_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet7.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Preference_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet7.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet7.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Basket_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet7.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet7.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_Text, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
                cmnSheet7.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Priority, dataTypeName = DataTypeName.INT });
                cmnSheet7.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Rule_State, dataTypeName = DataTypeName.BIT });

                //List of Primary Column Names
                cmnSheet7.lstPrimaryAttr.Add(SM_ColumnNames.Vendor_Type);
                cmnSheet7.lstPrimaryAttr.Add(SM_ColumnNames.Preference_Name);
                cmnSheet7.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type);
                cmnSheet7.lstPrimaryAttr.Add(SM_ColumnNames.Basket_Name);

                cmnSheet7.lstUniqueKeys.Add(new SRMMigrationUniqueKeys { lstUniqueColumns = new List<string> { SM_ColumnNames.Rule_Name } });
                cmnSheet7.lstUniqueKeys.Add(new SRMMigrationUniqueKeys { lstUniqueColumns = new List<string> { SM_ColumnNames.Priority } });

                cmnSheet7.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet7);
                #endregion
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForSMRealtimePreference -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMRealtimePreference -> End");
            }
        }

        private void populateStaticDataForSMDataSourcePrioritization()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMDataSourcePrioritization -> Start");
            try
            {
                #region SM_DataSourcePrioritization_Sheet1_Security_Type_SheetDetails
                CommonSheetInfo cmnSheet1 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet1.sheetName = SM_DataSourcePrioritization_SheetNames.SECURITY_TYPE;

                //List of Column Infos
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.SECURITY_TYPE, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ENTITY_TYPES_FOR_CREATION, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FIRST_PRIORITY_VENDOR_EXCEPTION_CONFIGURED, dataTypeName = DataTypeName.BIT });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DELETE_PREVIOUS_EXCEPTION, dataTypeName = DataTypeName.BIT });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.DELETE_PREVIOUS_EXCEPTION_CONSIDERED_SECURITIES, dataTypeName = DataTypeName.BIT });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.ALL_VENDOR_VALUE_MISSING_EXCEPTION_CONFIGURED, dataTypeName = DataTypeName.BIT });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.RUN_CALCULATED_RULES, dataTypeName = DataTypeName.BIT });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.FLUSH_AND_FILL_LEGS, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.MERGE_LEGS, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.OVERRIDE_CONSIDER_ALL_VENDORS, dataTypeName = DataTypeName.BIT });

                //List of Primary Column Names
                cmnSheet1.lstPrimaryAttr.Add(SM_ColumnNames.SECURITY_TYPE);

                this.lstCommonSheetInfo.Add(cmnSheet1);
                #endregion


                #region SM_DataSourcePrioritization_Sheet2_Attributes_SheetDetails
                CommonSheetInfo cmnSheet2 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet2.sheetName = SM_DataSourcePrioritization_SheetNames.ATTRIBUTES;

                //List of Column Infos
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Security_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200" });

                //List of Primary Column Names
                cmnSheet2.lstPrimaryAttr.Add(SM_ColumnNames.Security_Type);

                cmnSheet2.allowExtraColumnsInSheet = true;
                cmnSheet2.allowMultiplesAgainstPrimary = true;
                cmnSheet2.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = new List<string>() { SM_ColumnNames.Attribute_Name } });

                this.lstCommonSheetInfo.Add(cmnSheet2);
                #endregion                
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForSMDataSourcePrioritization -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForSMDataSourcePrioritization -> End");
            }
        }

        private void populateStaticDataForRMReports()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForRMReports -> Start");
            try
            {
                #region RM_Reports_Sheet1_Report_Setup
                CommonSheetInfo cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = EMReportingConstants.REPORT_SETUP;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Repository_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Repository_Description, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Report_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Report_Type, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = new EMReportingController().GetAllReportTypes().Values.ToList() });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Legacy_Report, dataTypeName = DataTypeName.BIT, allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Report_Name);
                cmnSheet.allowMultiplesAgainstPrimary = true;
                cmnSheet.lstGroupValidations = new List<SRMMigrationGroupValidations>();
                //cmnSheet.lstGroupValidations.Add(new SRMMigrationGroupValidations() { lstGVColumns = new List<string>() { "Repository Name", "Repository Description" } });
                cmnSheet.lstGroupValidations.Add(new SRMMigrationGroupValidations() { lstGVColumns = new List<string>() { "Report Name", "Report Type", "Legacy Report", "Repository Name" } });

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Reports_Sheet2_Attribute_Mapping
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = EMReportingConstants.ATTRIBUTE_MAPPING;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Report_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Lookup_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.SEC_OR_REF, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Report_Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Report_Attribute_Description, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Data_Type, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = new EMReportingController().GetReportAttributeDataTypes(), allowBlanks = true });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Report_Name);
                cmnSheet.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Reports_Sheet3_Report_Configuration
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = EMReportingConstants.REPORT_CONFIGURATION;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Report_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Report_Header, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Multisheet_Report, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Calendar, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = SRMCommonRAD.GetCalendarNames().ToList() });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Start_Date, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = new EMReportingController().GetReportDateTypes(EMDateType.StartDate).Values.ToList() });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Custom_Value_Start_Date, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "50", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.End_Date, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = new EMReportingController().GetReportDateTypes(EMDateType.EndDate).Values.ToList(), allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Custom_Value_End_Date, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "50", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_DWH_Extract, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Show_Entity_Codes, dataTypeName = DataTypeName.BIT, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Show_Display_Names, dataTypeName = DataTypeName.BIT, allowBlanks = true });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Report_Name);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Reports_Sheet4_Report_Rules
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = EMReportingConstants.REPORT_RULES;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Report_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Rule_Type, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new EMReportingController().GetReportRuleTypes() });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Rule_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Rule_Text, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", isCaseSensitive = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Priority, dataTypeName = DataTypeName.INT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Rule_State, dataTypeName = DataTypeName.BIT });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Report_Name);
                cmnSheet.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Reports_Sheet5_Report_Attribute_Order
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = EMReportingConstants.REPORT_ATTRIBUTE_ORDER;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Report_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Lookup_Attribute, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", lookupType = LookupType.SEC_OR_REF, allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Display_Order, dataTypeName = DataTypeName.INT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Column_Width, dataTypeName = DataTypeName.INT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Decimal_Places, dataTypeName = DataTypeName.INT, allowBlanks = true });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Report_Name);
                cmnSheet.allowMultiplesAgainstPrimary = true;
                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForRMReports -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForRMReports -> End");
            }
        }

        private void populateStaticDataForRMPrioritization()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForRMPrioritization -> Start");
            try
            {
                #region RM_Prioritization_Sheet1_Entity_Type_Configuration
                CommonSheetInfo cmnSheet = new CommonSheetInfo();
                //cmnSheet.allowExtraColumnsInSheet = true;
                //Sheet Name
                cmnSheet.sheetName = RM_Prioritization_SheetNames.ENTITY_TYPE_CONFIGURATION;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.First_Priority_Vendor_Exception, dataTypeName = DataTypeName.BIT, allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Delete_Previous_Exception, dataTypeName = DataTypeName.BIT, allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.All_Vendor_Missing_Exception, dataTypeName = DataTypeName.BIT, allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Run_Calculated_Rules, dataTypeName = DataTypeName.BIT, allowBlanks = false });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Entity_Type_Name);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion

                #region RM_Prioritization_Sheet1_Attribute_Prioritization
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_Prioritization_SheetNames.ATTRIBUTE_PRIORITIZATION;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                //cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Tab_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Entity_Type_Name);
                cmnSheet.allowMultiplesAgainstPrimary = true;
                cmnSheet.allowExtraColumnsInSheet = true;

                //List of Unique Column Names
                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = new List<string>() { RM_ColumnNames.Attribute_Name } });

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion


                #region RM_Prioritization_Sheet1_Data_Source_Merging
                cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_Prioritization_SheetNames.DATA_SOURCE_MERGING;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Data_Source_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Unique_Key_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Is_Insert, dataTypeName = DataTypeName.BIT, allowBlanks = false });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Entity_Type_Name);
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Data_Source_Name);
                //cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Attribute_Name);
                cmnSheet.allowMultiplesAgainstPrimary = true;

                ////List of Unique Column Names
                cmnSheet.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = new List<string>() { RM_ColumnNames.Data_Source_Name } });

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion



            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForRMPrioritization -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForRMPrioritization -> End");
            }
        }

        private void populateStaticDataForCommonConfiguration()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForCommonConfiguration -> Start");
            try
            {
                CommonSheetInfo cmnSheet1 = new CommonSheetInfo { allowMultiplesAgainstPrimary = true };

                //Sheet Name
                cmnSheet1.sheetName = SM_CommonConfiguration_SheetNames.Configuration;

                //List of Column Infos
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Page_Identifier, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Functionality_Identifier, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Attribute, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });

                //List of Primary Column Names
                cmnSheet1.lstPrimaryAttr.Add(SM_ColumnNames.Page_Identifier);
                cmnSheet1.lstPrimaryAttr.Add(SM_ColumnNames.Functionality_Identifier);
                cmnSheet1.lstUniqueKeys.Add(new SRMMigrationUniqueKeys { lstUniqueColumns = new List<string> { SM_ColumnNames.Attribute } });

                this.lstCommonSheetInfo.Add(cmnSheet1);


                CommonSheetInfo cmnSheet2 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet2.sheetName = SM_CommonConfiguration_SheetNames.Advanced_Configuration;
                //List of Column Infos
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Page_Identifier, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Functionality_Identifier, dataTypeName = DataTypeName.VARCHAR });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Include_Security_Type, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "Yes", "No" } });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Include_Security_Id, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "Yes", "No" } });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Include_Last_Modified_By, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "Yes", "No" } });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Include_Last_Modified_On, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "Yes", "No" } });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Include_Created_By, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "Yes", "No" } });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_ColumnNames.Include_Created_On, dataTypeName = DataTypeName.ENUM_VARCHAR, lstPossibleVal = new List<string> { "Yes", "No" } });

                //List of Primary Column Names
                cmnSheet2.lstPrimaryAttr.Add(SM_ColumnNames.Page_Identifier);
                cmnSheet2.lstPrimaryAttr.Add(SM_ColumnNames.Functionality_Identifier);

                this.lstCommonSheetInfo.Add(cmnSheet2);
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForCommonConfiguration -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForCommonConfiguration -> End");
            }
        }

        private void populateStaticDataForRMDownstreamTasks()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForRMDownstreamTasks -> Start");
            try
            {
                #region RM_Downstream_Tasks_Sheet1_Definition
                CommonSheetInfo cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = EMReportingConstants.REPORTING_TASK_CONFIGURATION;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Task_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Task_Description, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Reports, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1" });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Report_System, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.REQUIRE_REPORT_ATTRIBUTE_LEVEL_AUDIT, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "50",
                    lstPossibleVal =
                    new List<string>() {
                                                          "YES","NO","TRUE","FALSE"  }
                });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Calendar, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = SRMCommonRAD.GetCalendarNames().ToList() });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Start_Date, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = new EMReportingController().GetReportDateTypes(EMDateType.StartDate, true).Values.ToList() });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Custom_Value_Start_Date, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "50", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.End_Date, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = new EMReportingController().GetReportDateTypes(EMDateType.EndDate).Values.ToList(), allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Custom_Value_End_Date, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "50", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Direct_Downstream_Post, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Send_Realtime_Updates, dataTypeName = DataTypeName.BIT });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Publication_Queues, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Publication_Format, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = new EMReportingTaskController().GetPublicationFileFormats(), allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Publication_Delimiter, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = new EMReportingTaskController().GetReportFileDelimiters(), allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Extraction_Transport, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = new EMReportingTaskController().GetFileExtractionTransports(), allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Remote_File_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Remote_File_Location, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Remote_File_Format, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = new EMReportingTaskController().GetReportFileFormats(), allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Remote_File_Delimiter, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = new EMReportingTaskController().GetReportFileDelimiters(), allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Report_File_Date_Type, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = new EMReportingController().GetReportDateTypes().Values.ToList(), allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Custom_Value_Report_File_Date_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "100", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Email_Ids, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Email_Transport, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = new EMReportingTaskController().GetEmailTransports(), allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Email_File_Format, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = new EMReportingTaskController().GetReportFileFormats(), allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Email_File_Delimiter, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = new EMReportingTaskController().GetReportFileDelimiters(), allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Email_File_Transport, dataTypeName = DataTypeName.ENUM_VARCHAR, dataTypeLength = "-1", lstPossibleVal = new EMReportingTaskController().GetEmailFileExtractionTransports(), allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Email_File_Location, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
             
                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Task_Name);
                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForRMDownstreamTasks -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForRMDownstreamTasks -> End");
            }
        }


        private void populateStaticDataForSMDownstreamReports()
        {
            mLogger.Debug("SMCommonMigrationController -> populateStaticDataForSMDownstreamReports -> Start");
            try
            {
                #region SM_DownstreamReports_ReportSetup_SheetDetails
                CommonSheetInfo cmnSheet1 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet1.sheetName = SM_DownstreamReports_SheetNames.Report_Setup;

                //List of Column Infos
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Repository_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "100", allowBlanks = false });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Repository_Description, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Report_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = false });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SM_DownstreamReports_ColumnNames.Report_Type,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SM_DownstreamReports_ReportType.Multiple_Security_Type_Custom_Attributes,
                                                          SM_DownstreamReports_ReportType.Multiple_Security_Type_Custom_Attributes_No_Audit,
                                                          SM_DownstreamReports_ReportType.Attribute_Level_Audit_History_Report
                                                        },
                    allowBlanks = false
                });

                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Security_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = false });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Insert_Text, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Delete_Text, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });
                cmnSheet1.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Update_Text, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });

                //List of Primary Column Names
                cmnSheet1.lstPrimaryAttr.Add(SM_DownstreamReports_ColumnNames.Report_Name);
                cmnSheet1.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet1);
                #endregion

                #region SM_DownstreamReports_AttributeMapping_SheetDetails
                CommonSheetInfo cmnSheet2 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet2.sheetName = SM_DownstreamReports_SheetNames.Attribute_Mapping;
                //List of Column Infos
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Report_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = false });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Security_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Security_Type_SubInstruments_Legs, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "500", allowBlanks = true });

                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Attribute_Name, dataTypeName = DataTypeName.VARCHAR, allowBlanks = false });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Reference_Underlyer_Attribute_Name, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Custom_Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });
                cmnSheet2.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SM_DownstreamReports_ColumnNames.Data_Type,
                    dataTypeName = DataTypeName.VARCHAR,

                    lstPossibleVal = new List<string>() { SM_AttributesDataType.Boolean,
                                                          SM_AttributesDataType.Date,
                                                          SM_AttributesDataType.DateTime,
                                                          SM_AttributesDataType.File,
                                                          SM_AttributesDataType.Numeric,
                                                          SM_AttributesDataType.String,
                                                          SM_AttributesDataType.Time },
                    allowBlanks = true
                });

                cmnSheet2.lstPrimaryAttr.Add(SM_DownstreamReports_ColumnNames.Report_Name);
                cmnSheet2.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet2);
                #endregion

                #region SM_DownstreamReportsConfiguration_SheetDetails
                CommonSheetInfo cmnSheet3 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet3.sheetName = SM_DownstreamReports_SheetNames.Report_Configuration;
                //List of Column Infos
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Report_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = false });
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Report_Header, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = true });
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SM_DownstreamReports_ColumnNames.MultiSheet_Report,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                    allowBlanks = false
                });

                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SM_DownstreamReports_ColumnNames.Expandable,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                    allowBlanks = false
                });

                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SM_DownstreamReports_ColumnNames.Show_Security_Type_Name,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                    allowBlanks = false
                });

                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SM_DownstreamReports_ColumnNames.Calender,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = SRMCommonRAD.GetCalendarNames().ToList(),
                    allowBlanks = false
                });

                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SM_DownstreamReports_ColumnNames.Start_Date,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SM_DownstreamReports_DateType.None,
                                                          SM_DownstreamReports_DateType.Todays,
                                                          SM_DownstreamReports_DateType.Yesterdays,
                                                          SM_DownstreamReports_DateType.LastBusinessDays,
                                                          SM_DownstreamReports_DateType.TminusN,
                                                          SM_DownstreamReports_DateType.Custom,
                                                          SM_DownstreamReports_DateType.Now,
                                                          SM_DownstreamReports_DateType.FirstBusinessDayOfMonth,
                                                          SM_DownstreamReports_DateType.FirstBusinessDayOfYear,
                                                          SM_DownstreamReports_DateType.LastBusinessDayOfMonth,
                                                          SM_DownstreamReports_DateType.LastBusinessDayOfYear
                                                        },
                    allowBlanks = false
                });

                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Custom_Value_Start_Date, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });

                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SM_DownstreamReports_ColumnNames.End_Date,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SM_DownstreamReports_DateType.None,
                                                          SM_DownstreamReports_DateType.Todays,
                                                          SM_DownstreamReports_DateType.Yesterdays,
                                                          SM_DownstreamReports_DateType.LastBusinessDays,
                                                          SM_DownstreamReports_DateType.TminusN,
                                                          SM_DownstreamReports_DateType.Custom,
                                                          SM_DownstreamReports_DateType.Now,
                                                          SM_DownstreamReports_DateType.FirstBusinessDayOfMonth,
                                                          SM_DownstreamReports_DateType.FirstBusinessDayOfYear,
                                                          SM_DownstreamReports_DateType.LastBusinessDayOfMonth,
                                                          SM_DownstreamReports_DateType.LastBusinessDayOfYear
                                                        },
                    allowBlanks = false
                });

                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Custom_Value_End_Date, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });
                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Share_Report, dataTypeName = DataTypeName.VARCHAR, allowBlanks = true });

                cmnSheet3.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SM_DownstreamReports_ColumnNames.From_To_View,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SRM_WorkFlow_YN_Type.Yes,
                                                          SRM_WorkFlow_YN_Type.No
                                                        },
                    allowBlanks = false
                });

                //List of Primary Column Names
                cmnSheet3.lstPrimaryAttr.Add(SM_DownstreamReports_ColumnNames.Report_Name);
                cmnSheet3.allowMultiplesAgainstPrimary = false;

                this.lstCommonSheetInfo.Add(cmnSheet3);
                #endregion

                #region SM_DownstreamReports_Rules_SheetDetails
                CommonSheetInfo cmnSheet4 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet4.sheetName = SM_DownstreamReports_SheetNames.Report_Rules;
                //List of Column Infos
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Report_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = false });
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SM_DownstreamReports_ColumnNames.Rule_Type,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SM_DownstreamReports_RuleType.Filter_Rule,
                                                          SM_DownstreamReports_RuleType.Transformation_Rule
                                                        },
                    allowBlanks = false
                });

                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Rule_Name, dataTypeName = DataTypeName.VARCHAR, allowBlanks = false });

                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Rule_Text, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "-1", allowBlanks = false, isCaseSensitive = true });
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Priority, dataTypeName = DataTypeName.INT, allowBlanks = false });
                cmnSheet4.lstColumnInfo.Add(new CommonColumnInfo()
                {
                    columnName = SM_DownstreamReports_ColumnNames.Rule_State,
                    dataTypeName = DataTypeName.ENUM_VARCHAR,
                    lstPossibleVal = new List<string>() { SRM_WorkFlow_TF_Type.False,
                                                          SRM_WorkFlow_TF_Type.True
                                                        },
                    allowBlanks = false
                });

                //List of Primary Column Names
                cmnSheet4.lstPrimaryAttr.Add(SM_DownstreamReports_ColumnNames.Report_Name);
                cmnSheet4.lstPrimaryAttr.Add(SM_DownstreamReports_ColumnNames.Rule_Type);
                cmnSheet4.lstUniqueKeys.Add(new SRMMigrationUniqueKeys() { lstUniqueColumns = { SM_DownstreamReports_ColumnNames.Rule_Name } });

                cmnSheet4.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet4);
                #endregion

                #region SM_DownstreamReports_Attribute_Order_SheetDetails
                CommonSheetInfo cmnSheet5 = new CommonSheetInfo();

                //Sheet Name
                cmnSheet5.sheetName = SM_DownstreamReports_SheetNames.Report_Attribute_Order;
                //List of Column Infos

                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Report_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = false });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Custom_Attribute_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "200", allowBlanks = false });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Display_Order, dataTypeName = DataTypeName.INT, allowBlanks = false });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Column_Width, dataTypeName = DataTypeName.INT, allowBlanks = false });
                cmnSheet5.lstColumnInfo.Add(new CommonColumnInfo() { columnName = SM_DownstreamReports_ColumnNames.Numeric_Format, dataTypeName = DataTypeName.INT, allowBlanks = true });

                //List of Primary Column Names
                cmnSheet5.lstPrimaryAttr.Add(SM_DownstreamReports_ColumnNames.Report_Name);
                cmnSheet5.allowMultiplesAgainstPrimary = true;

                this.lstCommonSheetInfo.Add(cmnSheet5);
                #endregion

            }
            catch (Exception ex)
            {
                mLogger.Error("SMCommonMigrationController -> populateStaticDataForSMDownstreamReports -> Error : " + ex.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SMCommonMigrationController -> populateStaticDataForSMDownstreamReports -> End");
            }
        }

        #region Migration RM Specific Methods

        private void populateStaticDataForRMRealtimePreference()
        {
            mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForRMRealtimePreference -> Start");
            try
            {
                #region RM_RealTime_Preference
                CommonSheetInfo cmnSheet = new CommonSheetInfo();

                //Sheet Name
                cmnSheet.sheetName = RM_RealTimePreference_SheetName.PREFERENCE_SETUP;

                //List of Column Infos
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Entity_Type_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Preference_Name, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Preference_Description, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Vendor_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Request_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Market_Sector, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Data_Source, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Vendor_Identifier, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Transport, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = false });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Data_Request_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = true });
                cmnSheet.lstColumnInfo.Add(new CommonColumnInfo() { columnName = RM_ColumnNames.Asset_Type, dataTypeName = DataTypeName.VARCHAR, dataTypeLength = "1000", allowBlanks = true });

                //List of Primary Column Names
                cmnSheet.lstPrimaryAttr.Add(RM_ColumnNames.Preference_Name);

                this.lstCommonSheetInfo.Add(cmnSheet);
                #endregion                
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMCommonMigrationInfo -> populateStaticDataForRMRealtimePreference -> Error : " + ex.Message.ToString());
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationInfo -> populateStaticDataForRMRealtimePreference -> End");
            }
        }

        private ObjectSet RMGetDataSourceConfiguration(List<int> feedIds, int moduleID)
        {
            mLogger.Debug("SRMCommonMigrationController : RMGetDataSourceConfiguration -> Start");
            try
            {
                ObjectSet objSet = new RMDataSourceControllerNew().GetDataSourceConfiguration(feedIds, moduleID, false, false);


                return objSet;
            }
            catch (Exception ex)
            {
                mLogger.Debug("SRMCommonMigrationController : RMGetDataSourceConfiguration -> Error: " + ex.Message);
                throw;
            }
            finally
            {
                mLogger.Debug("SRMCommonMigrationController : RMGetDataSourceConfiguration -> End");
            }
        }

        #endregion
    }

}
