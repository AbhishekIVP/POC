using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.rad.utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.reporting
{
    public class EMReportingDBManager
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("EMReportingDBManager");
        private RHashlist mList = null;
        private RDBConnectionManager mDBCon = null;

        public EMReportingDBManager()
        {
        }
        public EMReportingDBManager(RDBConnectionManager conMgr)
        {
            this.mDBCon = conMgr;
        }

        internal object GetReportMetadata(List<object> lstReports, string userName, EMModule module = EMModule.AllSystems, EMInputType inputType = EMInputType.Id, EMDataType dataType = EMDataType.ObjectSet)
        {
            try
            {
                string reportList = string.Empty;
                string queryText = string.Empty;

                if (lstReports != null && lstReports.Count > 0)
                {
                    reportList = string.Join(",", lstReports);
                }

                queryText = "EXEC [IVPRefMaster].[dbo].[REFM_GetReportMetadata] '" + reportList + "','" + userName + "'," + (int)module + "," + (int)inputType;

                if (this.mDBCon != null)
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                        return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, this.mDBCon);
                        return result;
                    }
                }
                else
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMaster_Connection);
                        return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, ConnectionConstants.RefMaster_Connection);
                        return result;
                    }
                }
                return null;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("GetReportMetadata -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }

        internal object GetReportingTaskMetadata(List<object> lstTasks, string userName, EMModule module = EMModule.AllSystems, EMInputType inputType = EMInputType.Id, EMDataType dataType = EMDataType.ObjectSet)
        {
            try
            {
                string taskList = string.Empty;
                string queryText = string.Empty;

                if (lstTasks != null && lstTasks.Count > 0)
                {
                    taskList = string.Join(",", lstTasks);
                }

                queryText = "EXEC [IVPRefMaster].[dbo].[REFM_GetReportingTaskMetadata] '" + taskList + "','" + userName + "'," + (int)module + "," + (int)inputType;

                if (this.mDBCon != null)
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                        return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, this.mDBCon);
                        return result;
                    }
                }
                else
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMaster_Connection);
                        return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, ConnectionConstants.RefMaster_Connection);
                        return result;
                    }
                }
                return null;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("GetReportingTaskMetadata -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }

        internal object GetAllReports(string userName, EMModule module = EMModule.AllSystems, EMDataType dataType = EMDataType.ObjectSet)
        {
            try
            {
                string queryText = string.Empty;

                queryText = "EXEC [IVPRefMaster].[dbo].[REFM_GetReportsBasedOnModules] " + (int)module;

                if (this.mDBCon != null)
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                        return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, this.mDBCon);
                        return result;
                    }
                }
                else
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMaster_Connection);
                        return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, ConnectionConstants.RefMaster_Connection);
                        return result;
                    }
                }
                return null;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("GetAllReports -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }

        internal object GetAllReportingTasks(string userName, EMModule module = EMModule.AllSystems, EMDataType dataType = EMDataType.ObjectSet)
        {
            try
            {
                string queryText = string.Empty;

                queryText = "EXEC [IVPRefMaster].[dbo].[REFM_GetReportingTasksBasedOnModules] " + (int)module;

                if (this.mDBCon != null)
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                        return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, this.mDBCon);
                        return result;
                    }
                }
                else
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMaster_Connection);
                        return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, ConnectionConstants.RefMaster_Connection);
                        return result;
                    }
                }
                return null;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("GetAllReportingTasks -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }

        internal DataSet GetAllReportTypes()
        {
            try
            {
                DataSet dsResult;
                if (this.mDBCon != null)
                {
                    dsResult = CommonDALWrapper.ExecuteSelectQuery(EMQueryConstants.GET_REPORT_TYPE, this.mDBCon);
                }
                else
                {
                    dsResult = CommonDALWrapper.ExecuteSelectQuery(EMQueryConstants.GET_REPORT_TYPE, ConnectionConstants.RefMaster_Connection);
                }
                return dsResult;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("GetAllReportTypes -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }

        internal DataSet GetTransportByTransportType(List<string> lstTransportTypes)
        {
            try
            {
                DataSet dsResult;
                string queryText = @"SELECT transport_type_name, transport_name FROM [ivprad].[dbo].[ivp_rad_transport_type_master] mas (NOLOCK) 
                                                                        INNER JOIN[ivprad].[dbo].[ivp_rad_transport_config_details] det(NOLOCK) ON(mas.transport_type_id = det.transport_type_id)
                                                                        WHERE is_active = 1 AND transport_type_name IN('" + string.Join("','", lstTransportTypes) + "') ORDER BY transport_name";
                if (this.mDBCon != null)
                {
                    dsResult = CommonDALWrapper.ExecuteSelectQuery(queryText, this.mDBCon);
                }
                else
                {
                    dsResult = CommonDALWrapper.ExecuteSelectQuery(queryText, ConnectionConstants.RefMaster_Connection);
                }
                return dsResult;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("GetTransportByTransportType -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }

        internal DataSet GetReportFileFormats()
        {
            try
            {
                DataSet dsResult;
                string queryText = @"SELECT * FROM [ivprefmastervendor].[dbo].[ivp_refm_file_format] (NOLOCK) ORDER BY report_file_format";
                if (this.mDBCon != null)
                {
                    dsResult = CommonDALWrapper.ExecuteSelectQuery(queryText, this.mDBCon);
                }
                else
                {
                    dsResult = CommonDALWrapper.ExecuteSelectQuery(queryText, ConnectionConstants.RefMaster_Connection);
                }
                return dsResult;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("GetTransportByTransportType -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }

        internal object SaveReportRepository(EMReport reportInfo, DateTime updateTime, EMCallingInterface callingInterface, EMModule module = EMModule.AllSystems, EMDataType dataType = EMDataType.DataSet)
        {
            try
            {
                string queryText = string.Empty;
                bool checkExistingForCreate = false;
                bool includeDBTransaction = false;
                if (callingInterface == EMCallingInterface.Sync)
                    checkExistingForCreate = false;
                else
                    checkExistingForCreate = true;

                if (this.mDBCon != null)
                {
                    includeDBTransaction = false;
                }
                else
                {
                    includeDBTransaction = true;
                }

                queryText = "EXEC [IVPRefMaster].[dbo].[REFM_SaveReportRepository] " + reportInfo.Repository.Id + ",'" + reportInfo.Repository.Name + "','" + reportInfo.Repository.Description + "','"
                        + updateTime.ToString("yyyyMMdd HH:mm:ss.fff") + "','" + reportInfo.UserName + "','" + checkExistingForCreate + "','" + includeDBTransaction + "'," + (int)module;

                if (this.mDBCon != null)
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                        return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, this.mDBCon);
                        return result;
                    }
                }
                else
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMaster_Connection);
                        return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, ConnectionConstants.RefMaster_Connection);
                        return result;
                    }
                }
                return null;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("SaveReportRepository -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }
        internal object SaveReportSetup(EMReport reportInfo, DateTime updateTime, EMCallingInterface callingInterface, EMModule module = EMModule.AllSystems, EMDataType dataType = EMDataType.DataSet)
        {
            try
            {
                string queryText = string.Empty;
                bool includeDBTransaction = false;

                if (this.mDBCon != null)
                {
                    includeDBTransaction = false;
                }
                else
                {
                    includeDBTransaction = true;
                }

                queryText = "EXEC [IVPRefMaster].[dbo].[REFM_SaveReportSetup] " + reportInfo.Report.Id + ",'" + reportInfo.Report.Name + "'," + reportInfo.Report.TypeId + ",'"
                        + string.Join(",", reportInfo.Mapping.Select(x => x.EntityTypeId).ToList()) + "'," + reportInfo.Repository.Id + ",'" + reportInfo.Report.IsLegacy + "','"
                        + updateTime.ToString("yyyyMMdd HH:mm:ss.fff") + "','" + reportInfo.UserName + "','" + includeDBTransaction + "'," + (int)module;

                if (this.mDBCon != null)
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                        return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, this.mDBCon);
                        return result;
                    }
                }
                else
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMaster_Connection);
                        return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, ConnectionConstants.RefMaster_Connection);
                        return result;
                    }
                }
                return null;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("SaveReportSetup -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }

        internal void DeleteReportRules(EMReport reportInfo, DateTime updateTime, EMModule module = EMModule.AllSystems, EMDataType dataType = EMDataType.DataSet)
        {
            try
            {
                string queryText = string.Empty;
                bool includeDBTransaction = false;

                if (this.mDBCon != null)
                {
                    includeDBTransaction = false;
                }
                else
                {
                    includeDBTransaction = true;
                }

                queryText = "EXEC [IVPRefMaster].[dbo].[Refm_DeleteRulesForReports] " + reportInfo.Report.Id + ",'" + includeDBTransaction + "'";

                if (this.mDBCon != null)
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                        //return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, this.mDBCon);
                        //return result;
                    }
                }
                else
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMaster_Connection);
                        //return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, ConnectionConstants.RefMaster_Connection);
                        //return result;
                    }
                }
                //return null;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("SaveReportConfiguration -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }
        internal object SaveReportConfiguration(EMReport reportInfo, DateTime updateTime, EMModule module = EMModule.AllSystems, EMDataType dataType = EMDataType.DataSet)
        {
            try
            {
                string queryText = string.Empty;
                bool includeDBTransaction = false;

                if (this.mDBCon != null)
                {
                    includeDBTransaction = false;
                }
                else
                {
                    includeDBTransaction = true;
                }

                queryText = "EXEC [IVPRefMaster].[dbo].[REFM_SaveReportConfiguration] " + reportInfo.Report.Id + ",'" + reportInfo.Configuration.ReportHeader + "'," + reportInfo.Configuration.IsMultiSheet + ","
                                + reportInfo.Configuration.CalendarId + "," + reportInfo.Configuration.StartDate.Id + ",'" + reportInfo.Configuration.StartDate.CustomValue
                                + "'," + reportInfo.Configuration.EndDate.Id + ",'" + reportInfo.Configuration.EndDate.CustomValue + "'," + reportInfo.Configuration.IsFromToView + ","
                                + reportInfo.Configuration.ShowEntityCodes + "," + reportInfo.Configuration.ShowDisplayNames
                                + ",'" + updateTime.ToString("yyyyMMdd HH:mm:ss.fff") + "','" + reportInfo.UserName + "','" + includeDBTransaction + "'," + (int)module;

                if (this.mDBCon != null)
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                        return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, this.mDBCon);
                        return result;
                    }
                }
                else
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMaster_Connection);
                        return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, ConnectionConstants.RefMaster_Connection);
                        return result;
                    }
                }
                return null;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("SaveReportConfiguration -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }
        internal object SaveReportAttributeMapping(int reportId, string mappingXML, string userName, DateTime updateTime, EMModule module = EMModule.AllSystems, EMDataType dataType = EMDataType.DataSet)
        {
            try
            {
                string queryText = string.Empty;
                bool includeDBTransaction = false;

                if (this.mDBCon != null)
                {
                    includeDBTransaction = false;
                }
                else
                {
                    includeDBTransaction = true;
                }

                queryText = "EXEC [IVPRefMaster].[dbo].[REFM_SaveReportAttributeMapping] " + reportId + ",'" + mappingXML + "','" + updateTime.ToString("yyyyMMdd HH:mm:ss.fff") + "','" + userName + "','" + includeDBTransaction + "'," + (int)module;

                if (this.mDBCon != null)
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                        return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, this.mDBCon);
                        return result;
                    }
                }
                else
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMaster_Connection);
                        return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, ConnectionConstants.RefMaster_Connection);
                        return result;
                    }
                }
                return null;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("SaveReportAttributeMapping -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }
        internal object SaveReportAttributeConfiguration(int reportId, string configurationXML, string userName, DateTime updateTime, EMModule module = EMModule.AllSystems, EMDataType dataType = EMDataType.DataSet)
        {
            try
            {
                string queryText = string.Empty;
                bool includeDBTransaction = false;

                if (this.mDBCon != null)
                {
                    includeDBTransaction = false;
                }
                else
                {
                    includeDBTransaction = true;
                }

                queryText = "EXEC [IVPRefMaster].[dbo].[REFM_SaveReportAttributeConfiguration] " + reportId + ",'" + configurationXML + "','" + updateTime.ToString("yyyyMMdd HH:mm:ss.fff") + "','" + userName + "','" + includeDBTransaction + "'," + (int)module;

                if (this.mDBCon != null)
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, this.mDBCon);
                        return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, this.mDBCon);
                        return result;
                    }
                }
                else
                {
                    if (dataType == EMDataType.ObjectSet)
                    {
                        ObjectSet result = CommonDALWrapper.ExecuteSelectQueryObject(queryText, ConnectionConstants.RefMaster_Connection);
                        return result;
                    }
                    else if (dataType == EMDataType.DataSet)
                    {
                        DataSet result = CommonDALWrapper.ExecuteSelectQuery(queryText, ConnectionConstants.RefMaster_Connection);
                        return result;
                    }
                }
                return null;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("SaveReportAttributeConfiguration -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }

        internal bool IsReportingTaskNameUnique(RMReportingTaskInfo objRMReportingTaskInfo)
        {
            mLogger.Debug("RMReportingTaskController: IsReportingTaskNameUnique -> Start ");
            try
            {
                DataSet dsResult = new DataSet();
                mList = new RHashlist();
                mList.Add(RMReportingTaskConstants.TASK_MASTER_ID, objRMReportingTaskInfo.TaskMasterID);
                mList.Add(RMReportingTaskConstants.TASK_NAME, objRMReportingTaskInfo.ReportingTaskName);
                dsResult = CommonDALWrapper.ExecuteSelectQuery(EMQueryConstants.IS_REPORTING_TASK_NAME_UNIQUE,
                    mList, ConnectionConstants.RefMasterVendor_Connection);

                if (dsResult.Tables[0].Rows[0][0].ToString() == "0")
                    return true;
                else
                    return false;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("IsReportingTaskNameUnique -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }
        internal DataSet SaveReportingTask(RMReportingTaskInfo objRMReportingTaskInfo, DataSet dsReportDetails)
        {
            mLogger.Debug("RMReportingTaskController : SaveReportingTask -> Start");
            try
            {
                int startDateType = 0, endDateType = 0;

                mList = new RHashlist();
                DataSet dsResult = new DataSet();
                mList.Add(RMReportingTaskConstants.TASK_MASTER_ID, objRMReportingTaskInfo.TaskMasterID);
                mList.Add(RMReportingTaskConstants.TASK_NAME, objRMReportingTaskInfo.ReportingTaskName);
                mList.Add(RMReportingTaskConstants.TASK_DESC, objRMReportingTaskInfo.ReportingTaskDescription);
                mList.Add(RMReportingTaskConstants.USER_NAME, objRMReportingTaskInfo.UserName);
                mList.Add(RMReportingTaskConstants.REPORT_SETUP_IDs, string.Join(",", dsReportDetails.Tables[0].Select("report_name IN ('" + string.Join("','", objRMReportingTaskInfo.ReportNames.ToArray()) + "')").Select(x => x.Field<string>("report_setup_id").ToString()).ToArray()));

                mList.Add(RMReportingTaskConstants.START_DATE_TYPE, (int)objRMReportingTaskInfo.StartDate);
                if (objRMReportingTaskInfo.StartDate == RMReportingTaskStartDate.CUSTOMDATE)
                    mList.Add(RMReportingTaskConstants.CUSTOM_START_DATE, objRMReportingTaskInfo.CustomStartDateValue);
                else
                    mList.Add(RMReportingTaskConstants.CUSTOM_START_DATE, null);
                startDateType = Convert.ToInt32(objRMReportingTaskInfo.StartDate);
                if (startDateType == 4 || startDateType >= 11 || startDateType <= 14)
                    mList.Add(RMReportingTaskConstants.START_DATE_DAYS, objRMReportingTaskInfo.CustomStartDateDays);
                else
                    mList.Add(RMReportingTaskConstants.START_DATE_DAYS, null);

                if (objRMReportingTaskInfo.StartDate == RMReportingTaskStartDate.LASTEXTRACTIONDATE || objRMReportingTaskInfo.StartDate == RMReportingTaskStartDate.LASTSUCCESSFULPUSHTIME)
                    mList.Add(RMReportingTaskConstants.END_DATE_TYPE, (int)RMReportingTaskEndDate.NONE);
                else
                    mList.Add(RMReportingTaskConstants.END_DATE_TYPE, (int)objRMReportingTaskInfo.EndDate);
                if (objRMReportingTaskInfo.EndDate == RMReportingTaskEndDate.CUSTOMDATE)
                    mList.Add(RMReportingTaskConstants.CUSTOM_END_DATE, objRMReportingTaskInfo.CustomEndDateValue);
                else
                    mList.Add(RMReportingTaskConstants.CUSTOM_END_DATE, null);

                endDateType = Convert.ToInt32(objRMReportingTaskInfo.EndDate);
                if (endDateType == 4 || endDateType >= 11 || endDateType <= 14)
                    mList.Add(RMReportingTaskConstants.END_DATE_DAYS, objRMReportingTaskInfo.CustomEndDateDays);
                else
                    mList.Add(RMReportingTaskConstants.END_DATE_DAYS, null);

                mList.Add(RMReportingTaskConstants.CALENDAR_ID, objRMReportingTaskInfo.CalendarId);

                if (dsReportDetails.Tables[1].Select("report_system_name = '" + objRMReportingTaskInfo.ReportSystem + "'").Count() > 0)
                    mList.Add(RMReportingTaskConstants.DOWNSTREAM_SYSTEM_ID, dsReportDetails.Tables[1].Select("report_system_name = '" + objRMReportingTaskInfo.ReportSystem + "'").Select(x => x.Field<int>("report_system_id")).First());
                else
                    mList.Add(RMReportingTaskConstants.DOWNSTREAM_SYSTEM_ID, null);
                mList.Add(RMReportingTaskConstants.IS_AUDIT_SECURITY_LEVEL, false);
                if (objRMReportingTaskInfo.objRMReportPublicationInfo != null && objRMReportingTaskInfo.objRMReportPublicationInfo.PublicationQueues != null && objRMReportingTaskInfo.objRMReportPublicationInfo.PublicationQueues.Count > 0)
                {
                    mList.Add(RMReportingTaskConstants.PUBLICATION_QUEUE_NAMES, string.Join(",", objRMReportingTaskInfo.objRMReportPublicationInfo.PublicationQueues.ToArray()));
                    mList.Add(RMReportingTaskConstants.PUBLICATION_FORMAT_ID, dsReportDetails.Tables[3].Select("report_file_format = '" + objRMReportingTaskInfo.objRMReportPublicationInfo.ReportFileFormatName + "' AND ( report_delimiter_char = '" + objRMReportingTaskInfo.objRMReportPublicationInfo.ReportFileFormatDelimiter + "' OR report_delimiter_char IS NULL )").Select(x => x.Field<int>("report_file_format_id")).First());
                }
                else
                {
                    mList.Add(RMReportingTaskConstants.PUBLICATION_QUEUE_NAMES, null);
                    mList.Add(RMReportingTaskConstants.PUBLICATION_FORMAT_ID, null);
                }
                if (objRMReportingTaskInfo.objRMReportExtractionInfo != null && !string.IsNullOrEmpty(objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionName))
                {
                    mList.Add(RMReportingTaskConstants.EXTRACTION_TRANSPORT_NAME, objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionName);
                    mList.Add(RMReportingTaskConstants.EXTRACTION_REMOTE_FILE_NAME, objRMReportingTaskInfo.objRMReportExtractionInfo.RemoteFileName);
                    mList.Add(RMReportingTaskConstants.EXTRACTION_REMOTE_FILE_LOCATION, objRMReportingTaskInfo.objRMReportExtractionInfo.RemoteFileLocation);
                    mList.Add(RMReportingTaskConstants.EXTRACTION_FILE_DATE_TYPE, (int)objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionDateType);

                    //mList.Add(RMReportingTaskConstants.EXTRACTION_FILE_DATE, null);

                    if (objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionDateType == RMExtractionDate.CUSTOMDATE)
                        mList.Add(RMReportingTaskConstants.EXTRACTION_FILE_DATE, objRMReportingTaskInfo.objRMReportExtractionInfo.CustomExtractionDateValue);
                    else
                        mList.Add(RMReportingTaskConstants.EXTRACTION_FILE_DATE, null);

                    startDateType = Convert.ToInt32(objRMReportingTaskInfo.objRMReportExtractionInfo.ExtractionDateType);
                    if (startDateType == 4 || startDateType >= 11 || startDateType <= 14)
                        mList.Add(RMReportingTaskConstants.EXTRACTION_FILE_DATE_DAYS, objRMReportingTaskInfo.objRMReportExtractionInfo.CustomExtractionDateDays);
                    else
                        mList.Add(RMReportingTaskConstants.EXTRACTION_FILE_DATE_DAYS, null);

                    //mList.Add(RMReportingTaskConstants.EXTRACTION_FILE_DATE_DAYS, null);

                    mList.Add(RMReportingTaskConstants.EXTRACTION_FORMAT_ID, dsReportDetails.Tables[3].Select("report_file_format = '" + objRMReportingTaskInfo.objRMReportExtractionInfo.ReportFileFormatName + "' AND ( report_delimiter_char = '" + objRMReportingTaskInfo.objRMReportExtractionInfo.ReportFileFormatDelimiter + "' OR report_delimiter_char IS NULL )").Select(x => x.Field<int>("report_file_format_id")).First());
                }
                else
                {
                    mList.Add(RMReportingTaskConstants.EXTRACTION_TRANSPORT_NAME, null);
                    mList.Add(RMReportingTaskConstants.EXTRACTION_REMOTE_FILE_NAME, null);
                    mList.Add(RMReportingTaskConstants.EXTRACTION_REMOTE_FILE_LOCATION, null);
                    mList.Add(RMReportingTaskConstants.EXTRACTION_FILE_DATE_TYPE, null);
                    mList.Add(RMReportingTaskConstants.EXTRACTION_FILE_DATE, null);
                    mList.Add(RMReportingTaskConstants.EXTRACTION_FILE_DATE_DAYS, null);
                    mList.Add(RMReportingTaskConstants.EXTRACTION_FORMAT_ID, null);
                }
                if (objRMReportingTaskInfo.objRMEmailTaskInfo != null && objRMReportingTaskInfo.objRMEmailTaskInfo.EmailIds != null && objRMReportingTaskInfo.objRMEmailTaskInfo.EmailIds.Count > 0)
                {
                    mList.Add(RMReportingTaskConstants.SUBSCRIPTION_EMAIL_IDs, string.Join(",", objRMReportingTaskInfo.objRMEmailTaskInfo.EmailIds.ToArray()));
                    if (dsReportDetails.Tables[2].Select("transport_type_name = 'SMTP' AND transport_name = '" + objRMReportingTaskInfo.objRMEmailTaskInfo.EmailTransportTypeName + "'").Count() > 0)
                        mList.Add(RMReportingTaskConstants.EMAIL_TRANSPORT_NAME, objRMReportingTaskInfo.objRMEmailTaskInfo.EmailTransportTypeName);
                    else
                        mList.Add(RMReportingTaskConstants.EMAIL_TRANSPORT_NAME, null);
                    if (dsReportDetails.Tables[2].Select("transport_type_name = 'WFT' AND transport_name = '" + objRMReportingTaskInfo.objRMEmailTaskInfo.FileTransportTypeName + "'").Count() > 0)
                    {
                        mList.Add(RMReportingTaskConstants.FILE_TRANSPORT_NAME, objRMReportingTaskInfo.objRMEmailTaskInfo.FileTransportTypeName);
                        mList.Add(RMReportingTaskConstants.FILE_TRANSPORT_LOCATION, objRMReportingTaskInfo.objRMEmailTaskInfo.FileTransportLocation);
                    }
                    else
                    {
                        mList.Add(RMReportingTaskConstants.FILE_TRANSPORT_NAME, null);
                        mList.Add(RMReportingTaskConstants.FILE_TRANSPORT_LOCATION, null);
                    }
                    mList.Add(RMReportingTaskConstants.SUBSCRIPTION_FORMAT_ID, dsReportDetails.Tables[3].Select("report_file_format = '" + objRMReportingTaskInfo.objRMEmailTaskInfo.ReportFileFormatName + "' AND (report_delimiter_char = '" + objRMReportingTaskInfo.objRMEmailTaskInfo.ReportFileFormatDelimiter + "' OR report_delimiter_char IS NULL )").Select(x => x.Field<int>("report_file_format_id")).First());
                }
                else
                {
                    mList.Add(RMReportingTaskConstants.SUBSCRIPTION_EMAIL_IDs, null);
                    mList.Add(RMReportingTaskConstants.EMAIL_TRANSPORT_NAME, null);
                    mList.Add(RMReportingTaskConstants.FILE_TRANSPORT_NAME, null);
                    mList.Add(RMReportingTaskConstants.FILE_TRANSPORT_LOCATION, null);
                    mList.Add(RMReportingTaskConstants.SUBSCRIPTION_FORMAT_ID, null);
                }

                mList.Add(RMReportingTaskConstants.USED_FOR_REALTIME_UPDATE, objRMReportingTaskInfo.UsedForRealTimeUpdate);
                mList.Add(RMReportingTaskConstants.REALTIME_UPDATE_AFTER_FLOW_TASK, objRMReportingTaskInfo.RealTimeUpdateAfterFlowTask);
                mList.Add(RMReportingTaskConstants.USED_FOR_DIRECT_DOWNSTREAM_POSTING, objRMReportingTaskInfo.UsedForDirectDownstreamPosting);
                mList.Add(RMReportingTaskConstants.IS_REPORT_ATTRIBUTE_LEVEL_AUDIT, objRMReportingTaskInfo.RequireReportAttributeLevelAudit);
                dsResult = (DataSet)CommonDALWrapper.ExecuteProcedure(EMQueryConstants.SAVE_REPORTING_TASK, mList, ConnectionConstants.RefMasterVendor_Connection)[0];
                return dsResult;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("SaveReportingTask -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }
        public DataSet GetDataForReportingTaskControls()
        {
            mLogger.Debug("RMReportingTaskController : GetDataForReportingTaskControls -> Start");
            try
            {
                mList = new RHashlist();
                DataSet dsResult = (DataSet)CommonDALWrapper.ExecuteProcedure(EMQueryConstants.GET_DATA_FOR_REPORTING_TASK_CONTROLS, mList,
                    ConnectionConstants.RefMaster_Connection)["DataSet"];
                return dsResult;
            }
            catch (CommonDALException dalEx) { throw new CommonDALException(dalEx.Message, dalEx); }
            catch (Exception ex)
            {
                mLogger.Debug("GetDataForReportingTaskControls -> Error" + ex.ToString());
                throw;
            }
            finally
            {
            }
        }
    }
}