using com.ivp.commom.commondal;
using com.ivp.common.Migration;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.srm.vendorapi;
using com.ivp.srm.vendorapi.bloomberg;
using com.ivp.srm.vendorapi.reuters;
using com.ivp.srmcommon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common
{
    public partial class RMRealTimePreferenceSync
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RMRealTimePreferenceSync");
        const string COL_IS_INSERT = "@isInsert";
        public void StartSync(int moduleId, ObjectSet deltaSet, string userName, DateTime syncDateTime, string dateFormat, out string errorMessage)
        {
            mLogger.Debug("RMRealTimePreferenceSync : SyncRealTimePreference -> Start");
            errorMessage = string.Empty;
            bool isInsert = false;
            RMPreferenceController prefController = new RMPreferenceController();
            Dictionary<string, RMEntityDetailsInfo> entityDetails = new Dictionary<string, RMEntityDetailsInfo>();
            Dictionary<int, string> dataSourceDict = new Dictionary<int, string>();
            ArrayList allFTPTransports = new ArrayList();

            //List<string> bloombergRequests = Enum.GetNames(typeof(RBbgRequestType)).ToList();
            //List<string> reutersRequests = Enum.GetNames(typeof(RReuterRequestType)).ToList();
            //List<string> reutersVendorIdentifiers = Enum.GetNames(typeof(RReuterInstrumentIdType)).ToList();
            //List<string> bloombergVendorIdentifiers = Enum.GetNames(typeof(RBbgInstrumentIdType)).ToList();
            //List<string> marketSectors = Enum.GetNames(typeof(RBbgMarketSector)).ToList();
            //List<string> dataRequestTypes = new List<string>() { "Current Data", "Company Data" };
            //List<string> assetTypes = Enum.GetNames(typeof(RReuterAssetTypes)).ToList();            
            List<string> vendorTypes = Enum.GetNames(typeof(RVendorType)).ToList();
            vendorTypes.Remove(RVendorType.MarkitWSO.ToString());

            try
            {
                if (deltaSet.Tables.Contains(RMPreferenceConstants.TABLE_PREFERENCE_SETUP) && deltaSet.Tables[RMPreferenceConstants.TABLE_PREFERENCE_SETUP] != null
                            && deltaSet.Tables[RMPreferenceConstants.TABLE_PREFERENCE_SETUP].Rows.Count > 0)
                {
                    ObjectTable sourceTable = deltaSet.Tables[RMPreferenceConstants.TABLE_PREFERENCE_SETUP];
                    ObjectSet dbData = prefController.GetRealTimePreferences(moduleId, new List<int>(), true);
                    entityDetails = new RMModelerController().GetEntityTypeDetails(moduleId);
                    dataSourceDict = new RMDataSourceControllerNew().GetAllDataSources();
                    allFTPTransports = new VendorInterfaceAPI().GetAllTransportsNew();

                    ObjectTable dbExistingData = new ObjectTable();
                    if (dbData != null && dbData.Tables.Count > 0)
                        dbExistingData = dbData.Tables[0];

                    List<RMCommonColumnInfo> columnsToAdd = new List<RMCommonColumnInfo>();                    
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = RMPreferenceConstants.preference_id, DataType = typeof(System.Int32), DefaultValue = 0 });
                    columnsToAdd.Add(new RMCommonColumnInfo() { ColumnName = COL_IS_INSERT, DataType = typeof(bool), DefaultValue = true });                    
                    new RMCommonController().AddColumnsToObjectTable(sourceTable, columnsToAdd);

                    var assign = from db in dbExistingData.AsEnumerable()
                                 join upl in sourceTable.AsEnumerable()
                                 on ConvertToLower(db[RMPreferenceConstants.Preference_Name]) equals ConvertToLower(upl[RMPreferenceConstants.Preference_Name])
                                 select AssignExistingPreferences(db, upl);

                    assign.Count();

                    foreach (ObjectRow row in sourceTable.Rows.Where(r => string.IsNullOrEmpty(Convert.ToString(r[RMCommonConstants.MIGRATION_COL_STATUS]))))
                    {
                        RDBConnectionManager mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMasterVendor_Connection);                        
                        List<string> errorMessages = new List<string>();
                        RMPreferenceInfo objPreferencesInfo = new RMPreferenceInfo();
                        try
                        {
                            isInsert = Convert.ToBoolean(row[COL_IS_INSERT]);
                            string entityTypeName = Convert.ToString(row[RMPreferenceConstants.Entity_Type_Name]);
                            List<string> entityTypeNames = new List<string>();
                            if(!string.IsNullOrEmpty(entityTypeName))
                                entityTypeNames = entityTypeName.Split(',').ToList();
                            string preferenceName = Convert.ToString(row[RMPreferenceConstants.Preference_Name]);
                            string preferenceDescription = Convert.ToString(row[RMPreferenceConstants.Preference_Description]);
                            string vendorType = Convert.ToString(row[RMPreferenceConstants.Vendor_Type]);
                            string requestType = Convert.ToString(row[RMPreferenceConstants.Request_Type]);
                            string marketSector = Convert.ToString(row[RMPreferenceConstants.Market_Sector]);
                            string dataSource = Convert.ToString(row[RMPreferenceConstants.Data_Source_Name]);
                            string vendorIdentifier = Convert.ToString(row[RMPreferenceConstants.Vendor_Identifier]);
                            string transport = Convert.ToString(row[RMPreferenceConstants.Transport]);
                            string dataRequestType = Convert.ToString(row[RMPreferenceConstants.Data_Request_Type]);
                            string assetType = Convert.ToString(row[RMPreferenceConstants.Asset_Type]);
                            int preferenceId = 0;

                            if (!isInsert)
                                preferenceId = Convert.ToInt32(row[RMPreferenceConstants.preference_id]);

                            foreach(string eName in entityTypeNames)
                            {
                                errorMessages.AddRange(ValidateEntityType(eName, entityDetails));
                            }

                            errorMessages.AddRange(ValidateDataSource(dataSource, dataSourceDict));

                            errorMessages.AddRange(ValidateVendorType(vendorTypes, ref vendorType));

                            errorMessages.AddRange(ValidateRequestTypes(vendorType, ref requestType));

                            errorMessages.AddRange(ValidateVendorIdentifiers(vendorType, ref vendorIdentifier));

                            errorMessages.AddRange(ValidateMarketSector(vendorType, ref marketSector));

                            errorMessages.AddRange(ValidateAssetType(vendorType, ref assetType));

                            errorMessages.AddRange(ValidateDataRequestType(vendorType, ref dataRequestType));

                            errorMessages.AddRange(ValidateTransport(ref transport, allFTPTransports));

                            if (errorMessages.Count() == 0)
                            {
                                string preferenceXML = prefController.PopulateRealTimePreferenceXML(preferenceName, preferenceDescription, vendorType, requestType, marketSector, dataSource, vendorIdentifier, transport, dataRequestType, assetType, dataSourceDict);

                                objPreferencesInfo.PreferenceID = preferenceId;
                                objPreferencesInfo.PreferenceName = preferenceName;
                                objPreferencesInfo.DataSourceID = dataSourceDict.AsEnumerable().Where(x => x.Value.SRMEqualWithIgnoreCase(dataSource)).Select(x => x.Key).FirstOrDefault();                                
                                objPreferencesInfo.VendorDetails = preferenceXML;
                                objPreferencesInfo.CreatedBy = userName;
                                objPreferencesInfo.LastModifiedBy = userName;

                                Assembly RefMControllerAssembly = Assembly.Load("RefMController");
                                Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMRealTimePreferenceController");
                                object obj = Activator.CreateInstance(objType);
                                bool status = false;
                                if (isInsert)
                                {
                                    MethodInfo createMethod = objType.GetMethod("CreateVendorPreference");
                                    status = (bool)createMethod.Invoke(obj, new object[] { objPreferencesInfo, mDBCon });
                                }
                                else
                                {
                                    MethodInfo updateMethod = objType.GetMethod("UpdateVendorPreference");
                                    status = (bool)updateMethod.Invoke(obj, new object[] { objPreferencesInfo, mDBCon });
                                }
                                if(status)
                                {
                                    mDBCon.CommitTransaction();
                                    new RMCommonMigration().SetPassedRow(row, string.Empty);
                                }
                                    
                                else
                                {
                                    new RMCommonMigration().SetFailedRow(row, new List<string>() { "Failed to save preference" }, false);
                                    mDBCon.RollbackTransaction();
                                }
                            }
                            else
                            {
                                new RMCommonMigration().SetFailedRow(row, errorMessages, false);
                                mDBCon.RollbackTransaction();
                            }
                                

                        }
                        catch (Exception ex)
                        {
                            mLogger.Debug("RMRealTimePreferenceSync : SyncRealTimePreference -> Error" + ex.Message);
                            new RMCommonMigration().SetFailedRow(row, new List<string>() { ex.Message }, false);
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

                    List<string> colsToRemove = new List<string>() { COL_IS_INSERT, RMPreferenceConstants.preference_id };
                    new RMCommonController().RemoveColumnsFromObjectTable(sourceTable, colsToRemove);
                }
            }
            catch (Exception ex)
            {
                mLogger.Debug("RMRealTimePreferenceSync : SyncRealTimePreference -> Error" + ex.Message);
                errorMessage = ex.Message;                
            }
            finally
            {
                mLogger.Debug("RMRealTimePreferenceSync : SyncRealTimePreference -> End");
            }
        }

        private bool AssignExistingPreferences(ObjectRow dbRow, ObjectRow sourceRow)
        {
            sourceRow[COL_IS_INSERT] = false;
            sourceRow[RMPreferenceConstants.preference_id] = Convert.ToInt32(dbRow[RMPreferenceConstants.preference_id]);
            //sourceRow[RMPreferenceConstants.entity_type_id] = Convert.ToInt32(dbRow[RMPreferenceConstants.entity_type_id]);
            //sourceRow[RMPreferenceConstants.data_source_id] = Convert.ToInt32(dbRow[RMPreferenceConstants.data_source_id]);
            //sourceRow[RMPreferenceConstants.Data_Source_Name] = Convert.ToString(dbRow[RMPreferenceConstants.Data_Source_Name]);
            //sourceRow[RMPreferenceConstants.Entity_Type_Name] = Convert.ToString(dbRow[RMPreferenceConstants.Entity_Type_Name]);
            return true;
        }
        private string ConvertToLower(object ob)
        {
            if (string.IsNullOrEmpty(Convert.ToString(ob)))
                return string.Empty;
            else
                return ob.ToString().ToLower();
        }
    }
}
