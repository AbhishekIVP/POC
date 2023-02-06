using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.Migration
{
    public class RMTimeSeriesTaskSync
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RMTimeSeriesTaskSync");

        public void StartSync(ObjectSet deltaSet, int moduleID, string userName, DateTime syncDateTime, out string errorMessage)
        {
            mLogger.Debug("RMTimeSeriesTaskSync : SyncTimeSeriesTask -> Start");
            try
            {
                RDBConnectionManager mDBCon = null;
                errorMessage = string.Empty;
                ObjectTable deltaTable = deltaSet.Tables[0];

                if (deltaTable != null && deltaTable.Rows.Count > 0)
                {
                    Dictionary<string, RMEntityDetailsInfo> entityTypeDetails = new RMModelerController().GetEntityTypeDetails(moduleID, null);
                    ObjectTable existingTasks = new RMTimeSeriesController().GetTimeSeriesTaskConfiguration(null, moduleID, true, null).Tables[0];
                    Dictionary<string, ObjectRow> dictExistingTasks = existingTasks != null ? existingTasks.AsEnumerable()
                        .ToDictionary<ObjectRow, string, ObjectRow>(row => row.Field<string>(RMCommonConstants.ENTITY_TYPE_NAME), row => row, StringComparer.InvariantCultureIgnoreCase) : new Dictionary<string, ObjectRow>();

                    //Looping through rows which have not failed any common validation
                    foreach (ObjectRow row in deltaTable.Rows.Where(r => string.IsNullOrEmpty(RMCommonStatic.ConvertToLower(r[RMCommonConstants.MIGRATION_COL_STATUS]))))
                    {
                        List<string> errors = new List<string>();
                        RMTimeSeriesTaskInfo timeSeriesInfo = null;
                        mDBCon = new RMCommonController().OpenNewConnection(ConnectionConstants.RefMasterVendor_Connection);
                        try
                        {
                            bool isValid = true;

                            //Check if row is insert or update
                            bool isInsert = true;

                            //Getting values from row
                            string fullEntityTypeName = Convert.ToString(row[RMCommonConstants.ENTITY_TYPE_NAME]);
                            string entityTypeName = fullEntityTypeName.Split('-')[0].Trim();
                            string legName = fullEntityTypeName.Split('-').Length > 1 ? fullEntityTypeName.Split('-')[1].Trim() : string.Empty;
                            string primaryAttributeName = Convert.ToString(row[RMCommonConstants.PRIMARY_ATTRIBUTE]);
                            string fileType = Convert.ToString(row[RMCommonConstants.FILE_TYPE]);
                            string dateFormat = Convert.ToString(row[RMCommonConstants.DATE_FORMAT]);
                            string filePath = Convert.ToString(row[RMCommonConstants.FILE_PATH]);
                            int fileTypeID = fileType.SRMEqualWithIgnoreCase("delimited") ? 1 : (fileType.SRMEqualWithIgnoreCase("excel") ? 3 : -1);
                            string taskName = fullEntityTypeName + "_TimeSeries";
                            int timeSeriesTaskID = -1;

                            if (dictExistingTasks != null)
                            {
                                foreach (var kvp in dictExistingTasks)
                                {
                                    if (kvp.Key.Replace(" - ", "-").SRMEqualWithIgnoreCase(fullEntityTypeName.Replace(" - ", "-")))
                                    {
                                        isInsert = false;
                                        ObjectRow dbRow = kvp.Value;
                                        taskName = Convert.ToString(dbRow["task_name"]);
                                        timeSeriesTaskID = Convert.ToInt32(dbRow["time_series_id"]);
                                        break;
                                    }
                                }
                            }

                            isValid = ValidateTimeSeriesTaskData(ref timeSeriesInfo, timeSeriesTaskID, taskName, entityTypeName, legName, primaryAttributeName, dateFormat, fileTypeID, filePath, entityTypeDetails, userName, syncDateTime, errors);

                            if (isValid)
                            {
                                SaveTimeSeriesTask(timeSeriesInfo, isInsert, mDBCon);
                                new RMCommonMigration().SetPassedRow(row, string.Empty);
                            }
                            else
                            {
                                new RMCommonMigration().SetFailedRow(row, errors, false);
                            }

                            mDBCon.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            if (mDBCon != null)
                                mDBCon.RollbackTransaction();

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

                }
            }
            catch (Exception ex)
            {
                mLogger.Debug("RMTimeSeriesTaskSync : SyncTimeSeriesTask -> Start");
                errorMessage = ex.Message;
            }
            finally
            {
                mLogger.Debug("RMTimeSeriesTaskSync : SyncTimeSeriesTask -> End");
            }
        }


        private bool ValidateTimeSeriesTaskData(ref RMTimeSeriesTaskInfo timeSeriesInfo, int taskID, string taskName, string entityTypeName, string legName, string attributeName, string dateFormat, int fileType, string filePath, Dictionary<string, RMEntityDetailsInfo> entityTypeDetails, string userName, DateTime syncDateTime, List<string> errors)
        {
            mLogger.Debug("RMTimeSeriesTaskSync : ValidateTimeSeriesTaskData -> Start");
            bool isValid = true;
            int entityTypeID = -1;
            int attributeID = -2;

            try
            {

                //Entity Type validations
                if (entityTypeDetails.Keys.SRMContainsWithIgnoreCase(entityTypeName)) //Check entity type exists or not
                {
                    RMEntityDetailsInfo entityTypeInfo = entityTypeDetails[entityTypeName];
                    entityTypeID = entityTypeInfo.EntityTypeID;

                    if (!string.IsNullOrEmpty(legName)) //Check leg exists or not
                    {
                        if (entityTypeInfo.Legs.Keys.SRMContainsWithIgnoreCase(legName))
                        {
                            entityTypeID = entityTypeInfo.Legs[legName].EntityTypeID;
                        }
                        else
                        {
                            isValid = false;
                            errors.Add(RMCommonConstants.ENTITY_TYPE_LEG_NOT_FOUND);
                        }
                    }

                    if (!attributeName.SRMEqualWithIgnoreCase("entity code")) //Check if primary attribute is entity code
                    {
                        if (entityTypeInfo.Attributes.Keys.SRMContainsWithIgnoreCase(attributeName))
                        {
                            RMEntityAttributeInfo attrInfo = entityTypeInfo.Attributes[attributeName];
                            attributeID = attrInfo.EntityAttributeID;
                        }
                        else
                        {
                            isValid = false;
                            errors.Add(RMCommonConstants.ENTITY_ATTRIBUTE_NOT_FOUND);
                        }
                    }
                }
                else
                {
                    isValid = false;
                    errors.Add(RMCommonConstants.ENTITY_TYPE_NOT_FOUND);
                }

                //Other Validations
                //Validate date format
                if (!new RMCommonDBManager(null).GetAllCultureDateFormats().Contains(dateFormat))
                {
                    isValid = false;
                    errors.Add(RMCommonConstants.INVALID_DATE_FORMAT);
                }

                //Validate File Type
                if (fileType <= 0)
                {
                    isValid = false;
                    errors.Add(RMCommonConstants.INVALID_FILE_TYPE);
                }

                if (isValid)
                {
                    RADFilePropertyInfo radfileInfo = new RADFilePropertyInfo();
                    radfileInfo.CreatedBy = userName;
                    radfileInfo.FeedName = taskName;
                    radfileInfo.FieldDelimiter = ',';
                    radfileInfo.FileName = filePath;
                    radfileInfo.FileType = fileType.ToString();
                    radfileInfo.RowDelimiter = "\r\n";
                    radfileInfo.LastModifiedBy = userName;
                    radfileInfo.LastModifiedOn = syncDateTime;

                    timeSeriesInfo = new RMTimeSeriesTaskInfo();
                    timeSeriesInfo.EntityTypeID = entityTypeID;
                    timeSeriesInfo.RadFileInfo = radfileInfo;
                    timeSeriesInfo.IsActive = true;
                    timeSeriesInfo.LastModifiedBy = userName;
                    timeSeriesInfo.LastModifiedOn = syncDateTime;
                    timeSeriesInfo.TimeSeriesTaskID = taskID;
                    timeSeriesInfo.DateFormat = dateFormat;
                    timeSeriesInfo.UniqueAttributeId = attributeID;
                }

                mLogger.Debug("RMTimeSeriesTaskSync : ValidateTimeSeriesTaskData -> End");
                return isValid;
            }
            catch
            {
                mLogger.Debug("RMTimeSeriesTaskSync : ValidateTimeSeriesTaskData -> Error");
                throw;
            }
        }


        private void SaveTimeSeriesTask(RMTimeSeriesTaskInfo taskInfo, bool isInsert, RDBConnectionManager mDBCon)
        {
            mLogger.Debug("RMTimeSeriesTaskSync : SaveTimeSeriesTask -> Start");
            try
            {
                string methodName = isInsert ? "InsertTimeSeriesTaskInfo" : "UpdateTimeSeriesTaskInfo";
                Assembly RefMControllerAssembly = Assembly.Load("RefMController");
                Type objType = RefMControllerAssembly.GetType("com.ivp.refmaster.controller.RMEntityTimeSeries");
                MethodInfo method = objType.GetMethod(methodName);
                object obj = Activator.CreateInstance(objType);
                method.Invoke(obj, new object[] { taskInfo, mDBCon });
                mLogger.Debug("RMTimeSeriesTaskSync : SaveTimeSeriesTask -> End");
            }
            catch
            {
                mLogger.Debug("RMTimeSeriesTaskSync : SaveTimeSeriesTask -> Error");
                throw;
            }

        }
    }
}
