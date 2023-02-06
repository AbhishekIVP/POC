using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.utils;
using com.ivp.secm.exceptionsmanager;
using com.ivp.SRMCommonModules;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.Exceptions
{
    public class EMExceptionController
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("EMExceptionController");

        public List<SRMEventInfo> ManageDataExceptions(EMManageExceptionsInfo manageExceptionsInfo, RDBConnectionManager mDBCon = null)
        {

            if (manageExceptionsInfo == null || manageExceptionsInfo.ExceptionInfo == null || manageExceptionsInfo.ExceptionInfo.Count == 0)
                return new List<SRMEventInfo>();

            mLogger.Debug("ManageDataExceptions - Start");

            bool isConnectionCreated = false;
            List<SRMEventInfo> eventsInfo = new List<SRMEventInfo>();
            try
            {

                DataTable dtExceptionMaster = new DataTable();
                dtExceptionMaster.Columns.Add("row_id", typeof(int));
                dtExceptionMaster.Columns.Add("entity_code");
                dtExceptionMaster.Columns.Add("attribute_id", typeof(int));
                dtExceptionMaster.Columns.Add("exception_type_id", typeof(int));
                dtExceptionMaster.Columns.Add("db_value");
                dtExceptionMaster.Columns.Add("leg_type_id", typeof(int));
                dtExceptionMaster.Columns.Add("remarks");

                DataTable dtExceptionDetails = new DataTable();
                dtExceptionDetails.Columns.Add("row_id", typeof(int));
                dtExceptionDetails.Columns.Add("dependent_id", typeof(int));
                dtExceptionDetails.Columns.Add("dependent_type_id", typeof(int));
                dtExceptionDetails.Columns.Add("dependent_value");

                if (mDBCon == null)
                {
                    string refMDBConnectionId = RConfigReader.GetConfigAppSettings(ConnectionConstants.RefMaster_Connection);
                    mDBCon = RDALAbstractFactory.DBFactory.GetConnectionManager(refMDBConnectionId);
                    mDBCon.UseTransaction = true;
                    isConnectionCreated = true;
                }

                int count = 1;
                Dictionary<int, Dictionary<int, Dictionary<string, List<EMExceptionDependentValues>>>> suppressedExceptionInfo = GetSuppressedExceptionInfo(manageExceptionsInfo.EntityTypeId, mDBCon);

                foreach (var info in manageExceptionsInfo.ExceptionInfo)
                {
                    if (!manageExceptionsInfo.InsertInactive)
                    {
                        foreach (var attrInfo in info.AttributesModified)
                        {
                            dtExceptionMaster.Rows.Add(-1, info.EntityCode, attrInfo, null, null, null, null);
                        }
                    }

                    foreach (var exInfo in info.ExceptionInfo)
                    {
                        bool isSuppressed = true;
                        if (exInfo.DependentValues == null || exInfo.DependentValues.Count == 0)
                        {
                            if (!(suppressedExceptionInfo.ContainsKey(exInfo.ExceptionTypeId)
                            && suppressedExceptionInfo[exInfo.ExceptionTypeId].ContainsKey(exInfo.AttributeId)
                            && suppressedExceptionInfo[exInfo.ExceptionTypeId][exInfo.AttributeId].ContainsKey(info.EntityCode)))
                            {
                                isSuppressed = false;
                            }
                        }
                        else
                        {
                            foreach (var dependentInfo in exInfo.DependentValues)
                            {
                                if (!(suppressedExceptionInfo.ContainsKey(exInfo.ExceptionTypeId)
                                && suppressedExceptionInfo[exInfo.ExceptionTypeId].ContainsKey(exInfo.AttributeId)
                                && suppressedExceptionInfo[exInfo.ExceptionTypeId][exInfo.AttributeId].ContainsKey(info.EntityCode)
                                && suppressedExceptionInfo[exInfo.ExceptionTypeId][exInfo.AttributeId][info.EntityCode].Any(e => e.DependentId == dependentInfo.DependentId && e.DependentValue.Equals(dependentInfo.DependentValue, StringComparison.OrdinalIgnoreCase))))
                                {
                                    isSuppressed = false;
                                    break;
                                }
                            }
                        }
                        if (!isSuppressed)
                        {
                            dtExceptionMaster.Rows.Add(count, info.EntityCode, exInfo.AttributeId, exInfo.ExceptionTypeId, exInfo.DBValue, exInfo.LegEntityTypeId, exInfo.AdditionalInfo);
                            foreach (var dependentInfo in exInfo.DependentValues)
                            {
                                dtExceptionDetails.Rows.Add(count, dependentInfo.DependentId, dependentInfo.DependentTypeId, dependentInfo.DependentValue);
                            }
                        }
                        count++;
                    }
                }

                string createTableQuery = string.Empty;
                createTableQuery = @"CREATE TABLE #exceptionMaster (row_id INT, entity_code VARCHAR(100), attribute_id INT, exception_type_id INT, db_value VARCHAR(MAX), leg_type_id INT, remarks VARCHAR(MAX)) 
                                 CREATE TABLE #exceptionDetails (row_id INT, dependent_id INT, dependent_type_id INT, dependent_value VARCHAR(MAX))";


                CommonDALWrapper.ExecuteSelectQuery(createTableQuery, mDBCon);


                if (dtExceptionMaster.Rows.Count > 0)
                {
                    CommonDALWrapper.ExecuteBulkUpload("#exceptionMaster", dtExceptionMaster, mDBCon);
                }

                if (dtExceptionDetails.Rows.Count > 0)
                {
                    CommonDALWrapper.ExecuteBulkUpload("#exceptionDetails", dtExceptionDetails, mDBCon);
                }

                DataSet dsResult = new DataSet();

                dsResult = CommonDALWrapper.ExecuteSelectQuery("EXEC REFM_ManageDataExceptionsBasedOnPreference " + manageExceptionsInfo.EntityTypeId + ",'" + manageExceptionsInfo.UserName + "',"
                    + manageExceptionsInfo.TaskMasterId + ",'" + manageExceptionsInfo.CreatedOn.ToString("yyyyMMdd HH:mm:ss.fff") + "'," + manageExceptionsInfo.InsertInactive + ",0", mDBCon);


                if (!manageExceptionsInfo.InsertInactive && dsResult != null && dsResult.Tables.Count > 0)
                {
                    if (dsResult.Tables[0] != null && dsResult.Tables[0].Rows.Count > 0)
                    {
                        Dictionary<string, Dictionary<int, SRMEventInfo>> dictDeleteECVsExceptionTypeVsInfo = new Dictionary<string, Dictionary<int, SRMEventInfo>>();
                        foreach (DataRow row in dsResult.Tables[0].Rows)
                        {
                            if (!dictDeleteECVsExceptionTypeVsInfo.ContainsKey(Convert.ToString(row["entity_code"])))
                            {
                                dictDeleteECVsExceptionTypeVsInfo.Add(Convert.ToString(row["entity_code"]), new Dictionary<int, SRMEventInfo>());
                            }
                            if (!dictDeleteECVsExceptionTypeVsInfo[Convert.ToString(row["entity_code"])].ContainsKey(Convert.ToInt32(row["exception_type_id"])))
                            {
                                dictDeleteECVsExceptionTypeVsInfo[Convert.ToString(row["entity_code"])].Add(Convert.ToInt32(row["exception_type_id"]), new SRMEventInfo());
                                SRMEventInfo eventInfo = new SRMEventInfo();
                                eventInfo.Action = SRMEventActionType.Exception_Delete;
                                eventInfo.ID = Convert.ToString(row["entity_code"]);
                                eventInfo.ExceptionType = (SMExceptionsType)Convert.ToInt32(row["exception_type_id"]);
                                eventInfo.User = manageExceptionsInfo.UserName;
                                eventInfo.Type = Convert.ToString(row["entity_display_name"]);
                                eventInfo.Module = (SRMEventModule)Convert.ToInt32(row["module_id"]);
                                eventInfo.TimeStamp = manageExceptionsInfo.CreatedOn;
                                eventInfo.Attributes = new List<SRMEventAttributeDetails>();
                                eventInfo.Attributes.Add(new SRMEventAttributeDetails() { RealName = Convert.ToString(row["attribute_name"]), Name = Convert.ToString(row["display_name"]) });
                                dictDeleteECVsExceptionTypeVsInfo[Convert.ToString(row["entity_code"])][Convert.ToInt32(row["exception_type_id"])] = eventInfo;
                            }
                            else
                            {
                                dictDeleteECVsExceptionTypeVsInfo[Convert.ToString(row["entity_code"])][Convert.ToInt32(row["exception_type_id"])].Attributes.Add(new SRMEventAttributeDetails() { RealName = Convert.ToString(row["attribute_name"]), Name = Convert.ToString(row["display_name"]) });
                            }
                        }
                        foreach (var kvp in dictDeleteECVsExceptionTypeVsInfo)
                        {
                            foreach (var info in kvp.Value)
                            {
                                eventsInfo.Add(info.Value);
                            }
                        }
                    }

                    if (dsResult.Tables.Count > 1 && dsResult.Tables[1] != null && dsResult.Tables[1].Rows.Count > 0)
                    {
                        Dictionary<string, Dictionary<int, SRMEventInfo>> dictInsertECVsExceptionTypeVsInfo = new Dictionary<string, Dictionary<int, SRMEventInfo>>();
                        foreach (DataRow row in dsResult.Tables[1].Rows)
                        {
                            if (!dictInsertECVsExceptionTypeVsInfo.ContainsKey(Convert.ToString(row["entity_code"])))
                            {
                                dictInsertECVsExceptionTypeVsInfo.Add(Convert.ToString(row["entity_code"]), new Dictionary<int, SRMEventInfo>());
                            }
                            if (!dictInsertECVsExceptionTypeVsInfo[Convert.ToString(row["entity_code"])].ContainsKey(Convert.ToInt32(row["exception_type_id"])))
                            {
                                dictInsertECVsExceptionTypeVsInfo[Convert.ToString(row["entity_code"])].Add(Convert.ToInt32(row["exception_type_id"]), new SRMEventInfo());
                                SRMEventInfo eventInfo = new SRMEventInfo();
                                eventInfo.Action = SRMEventActionType.Exception_Raised;
                                eventInfo.ID = Convert.ToString(row["entity_code"]);
                                eventInfo.ExceptionType = (SMExceptionsType)Convert.ToInt32(row["exception_type_id"]);
                                eventInfo.User = manageExceptionsInfo.UserName;
                                eventInfo.Type = Convert.ToString(row["entity_display_name"]);
                                eventInfo.Module = (SRMEventModule)Convert.ToInt32(row["module_id"]);
                                eventInfo.TimeStamp = manageExceptionsInfo.CreatedOn;
                                eventInfo.Attributes = new List<SRMEventAttributeDetails>();
                                eventInfo.Attributes.Add(new SRMEventAttributeDetails() { Name = Convert.ToString(row["display_name"]), RealName = Convert.ToString(row["attribute_name"]) });
                                dictInsertECVsExceptionTypeVsInfo[Convert.ToString(row["entity_code"])][Convert.ToInt32(row["exception_type_id"])] = eventInfo;
                            }
                            else
                            {
                                dictInsertECVsExceptionTypeVsInfo[Convert.ToString(row["entity_code"])][Convert.ToInt32(row["exception_type_id"])].Attributes.Add(new SRMEventAttributeDetails() { Name = Convert.ToString(row["display_name"]), RealName = Convert.ToString(row["attribute_name"]) });
                            }
                        }
                        foreach (var kvp in dictInsertECVsExceptionTypeVsInfo)
                        {
                            foreach (var info in kvp.Value)
                            {
                                eventsInfo.Add(info.Value);
                            }
                        }
                    }
                }
                if (isConnectionCreated && mDBCon != null)
                {
                    mDBCon.CommitTransaction();
                }
            }
            catch (Exception ex)
            {
                if (isConnectionCreated && mDBCon != null)
                {
                    mDBCon.RollbackTransaction();
                }
                mLogger.Error("ManageDataExceptions - Error - " + ex.ToString());

                throw;
            }
            finally
            {
                if (isConnectionCreated && mDBCon != null)
                    RDALAbstractFactory.DBFactory.PutConnectionManager(mDBCon);
                mLogger.Debug("ManageDataExceptions - End");
            }

            return eventsInfo;
        }

        private DataSet GetSuppressedExceptions(int entityTypeId, RDBConnectionManager mDBCon)
        {
            mLogger.Debug("GetSuppressedExceptions -> Start");
            RHashlist hlist = null;
            DataSet dsResult = null;
            try
            {

                hlist = new RHashlist();
                hlist.Add("MainEntityTypeID", entityTypeId);
                dsResult = (DataSet)CommonDALWrapper.ExecuteProcedure("REFM:GetExceptionDataForCoreUpdate", hlist, mDBCon)["DataSet"];
                // Need only table 2 and 3
                dsResult.Tables.RemoveAt(4);
                dsResult.Tables.RemoveAt(1);
                dsResult.Tables.RemoveAt(0);
                return dsResult;
            }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw;
            }
            finally
            {
                if (hlist != null)
                {
                    hlist.Dispose();
                }
                mLogger.Debug("GetSuppressedExceptions -> End");
            }
        }

        private Dictionary<int, Dictionary<int, Dictionary<string, List<EMExceptionDependentValues>>>> GetSuppressedExceptionInfo(int entityTypeId, RDBConnectionManager mDBCon)
        {
            Dictionary<int, Dictionary<int, Dictionary<string, List<EMExceptionDependentValues>>>> suppressedExceptionInfo = new Dictionary<int, Dictionary<int, Dictionary<string, List<EMExceptionDependentValues>>>>();
            DataSet dsSuppressedExceptions = GetSuppressedExceptions(entityTypeId, mDBCon);
            DataTable masterExceptionData = dsSuppressedExceptions.Tables[0];
            DataTable detailExceptionData = dsSuppressedExceptions.Tables[1];

            if (masterExceptionData.Rows.Count == 0)
                masterExceptionData = new DataTable();

            if (detailExceptionData.Rows.Count == 0)
                detailExceptionData = new DataTable();

            var setException = from m in masterExceptionData.AsEnumerable()
                               join d in detailExceptionData.AsEnumerable()
                               on m["exception_id"] equals d["exception_id"]
                               select SetExceptionData(m, d, ref suppressedExceptionInfo);

            setException.Count();
            return suppressedExceptionInfo;
        }

        private bool SetExceptionData(DataRow row, DataRow detailRow, ref Dictionary<int, Dictionary<int, Dictionary<string, List<EMExceptionDependentValues>>>> suppressedExceptionInfo)
        {
            int exception_type_id = Convert.ToInt32(row["exception_type_id"]);
            int attribute_id = Convert.ToInt32(row["entity_attribute_id"]);
            string entity_code = Convert.ToString(row["entity_code"]);
            string value = Convert.ToString(detailRow["dependent_value"]);
            int dependent_id = Convert.ToInt32(detailRow["dependent_id"]);

            if (!suppressedExceptionInfo.ContainsKey(exception_type_id))
                suppressedExceptionInfo.Add(exception_type_id, new Dictionary<int, Dictionary<string, List<EMExceptionDependentValues>>>());
            if (!suppressedExceptionInfo[exception_type_id].ContainsKey(attribute_id))
                suppressedExceptionInfo[exception_type_id].Add(attribute_id, new Dictionary<string, List<EMExceptionDependentValues>>());
            if (!suppressedExceptionInfo[exception_type_id][attribute_id].ContainsKey(entity_code))
                suppressedExceptionInfo[exception_type_id][attribute_id].Add(entity_code, new List<EMExceptionDependentValues>());

            suppressedExceptionInfo[exception_type_id][attribute_id][entity_code].Add(new EMExceptionDependentValues() { DependentId = dependent_id, DependentValue = value });
            return true;
        }

        //public List<SRMEventInfo> InsertInactiveExceptions(EMManageExceptionsInfo manageExceptionsInfo, RDBConnectionManager mDBCon = null)
        //{
        //    if (manageExceptionsInfo == null || manageExceptionsInfo.ExceptionInfo == null || manageExceptionsInfo.ExceptionInfo.Count == 0)
        //        return new List<SRMEventInfo>();

        //    List<SRMEventInfo> eventsInfo = new List<SRMEventInfo>();

        //    DataTable dtExceptionMaster = new DataTable();
        //    dtExceptionMaster.Columns.Add("row_id", typeof(int));
        //    dtExceptionMaster.Columns.Add("entity_code");
        //    dtExceptionMaster.Columns.Add("attribute_id", typeof(int));
        //    dtExceptionMaster.Columns.Add("exception_type_id", typeof(int));
        //    dtExceptionMaster.Columns.Add("db_value");
        //    dtExceptionMaster.Columns.Add("leg_type_id", typeof(int));
        //}
    }
}
