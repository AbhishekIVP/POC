using System;
using System.Text;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.utils;
using System.Collections.Generic;
using com.ivp.rad.data;

namespace com.ivp.commom.commondal
{
    public partial class CommonDALWrapper
    {
        public static ObjectSet ExecuteSelectQueryObject(string queryID, RHashlist paramaters, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start->Executing SELECT Query Object");
            ObjectSet dsDataSet = null;

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

                dsDataSet = mDbConn.ExecuteQueryObject(queryID, paramaters);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                mLogger.Debug("End->Executing SELECT Query Object");
            }
            return dsDataSet;
        }

        public static ObjectSet ExecuteSelectQueryObject(string queryText, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start->Executing SELECT Query Object");
            ObjectSet dsDataSet = null;

            try
            {
                dsDataSet = mDbConn.ExecuteQueryObject(DALWrapperAppend.Replace(queryText), RQueryType.Select);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                mLogger.Debug("End->Executing SELECT Query Object");
            }
            return dsDataSet;
        }

        public static void ExecuteQueryObject(string queryID, RHashlist paramaters, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start->Executing Query Object");

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

                mDbConn.ExecuteQueryObject(queryID, paramaters);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                mLogger.Debug("End->Executing Query Object");
            }
        }

        public static void ExecuteQueryObject(string queryID, RHashlist paramaters, bool state, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start->Executing Query Object");

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

                mDbConn.ExecuteQueryObject(queryID, paramaters, state);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                mLogger.Debug("End->Executing Query Object");
            }
        }

        public static void ExecuteQueryObject(string queryText, CommonQueryType queryType, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start->Executing Query Object");
            RQueryType radQueryType;

            try
            {
                radQueryType = (RQueryType)Enum.Parse(typeof(RQueryType), queryType.ToString());
                mDbConn.ExecuteQueryObject(DALWrapperAppend.Replace(queryText), radQueryType);

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
                mLogger.Debug("End->Executing Query Object");
            }
        }

        public static RHashlist ExecuteProcedureObject(string queryID, RHashlist paramaters, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start->Executing Procedure Object");

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

                return mDbConn.ExecuteProcedureObject(queryID, paramaters);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception ex)
            {
                mLogger.Error(ex.ToString());
                throw new CommonDALException(ex.ToString(), ex);
            }
            finally
            {
                mLogger.Debug("End->Executing Procedure Object");
            }
        }

        public static ObjectSet GetTableSchemaObject(string tableName, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start -> Getting Table Schema Object");

            try
            {
                return mDbConn.ExecuteQueryToGetSchemaObject(DALWrapperAppend.Replace(tableName));
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception rdEx)
            {
                mLogger.Error(rdEx.ToString());
                throw new CommonDALException(rdEx.Message, rdEx);
            }
            finally
            {
                mLogger.Debug("End -> Getting Table Schema Object");
            }
        }

        public static void ExecuteBulkUploadObject(string tableName, ObjectTable dtTable, RDBConnectionManager mDbConn)
        {
            mLogger.Debug("Start -> Bulk Uploading Object");

            try
            {
                mDbConn.ExecuteBulkCopyObject(DALWrapperAppend.Replace(tableName), dtTable, 100000);
            }
            catch (RDALException dalEx) { throw new CommonDALException(dalEx.ToString(), dalEx); }
            catch (Exception rdEx)
            {
                mLogger.Error(rdEx.ToString());
                throw new CommonDALException(rdEx.Message, rdEx);
            }
            finally
            {
                mLogger.Debug("End -> Bulk Uploading Object");
            }
        }
        
        public static ObjectSet ExecuteSelectQueryObject(string queryID, RHashlist paramaters, string connectionType)
        {
            mLogger.Debug("Start->Executing SELECT Query Object");
            ObjectSet dsDataSet = null;
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

                dsDataSet = mDbConn.ExecuteQueryObject(queryID, paramaters);
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
                mLogger.Debug("End->Executing SELECT Query Object");
            }
            return dsDataSet;
        }

        public static ObjectSet ExecuteSelectQueryObject(string queryText, string connectionType)
        {
            mLogger.Debug("Start->Executing SELECT Query Object");
            ObjectSet dsDataSet = null;
            RDBConnectionManager mDbConn = null;

            try
            {
                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(
                    RConfigReader.GetConfigAppSettings(connectionType));

                dsDataSet = mDbConn.ExecuteQueryObject(DALWrapperAppend.Replace(queryText), RQueryType.Select);
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
                mLogger.Debug("End->Executing SELECT Query Object");
            }
            return dsDataSet;
        }

        public static void ExecuteQueryObject(string queryID, RHashlist paramaters, string connectionType)
        {
            mLogger.Debug("Start->Executing Query Object");
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

                mDbConn.ExecuteQueryObject(queryID, paramaters);
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
                mLogger.Debug("End->Executing Query Object");
            }
        }

        public static void ExecuteQueryObject(string queryID, RHashlist paramaters, bool state, string connectionType)
        {
            mLogger.Debug("Start->Executing Query Object");
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

                mDbConn.ExecuteQueryObject(queryID, paramaters, state);
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
                mLogger.Debug("End->Executing Query Object");
            }
        }

        public static void ExecuteQueryObject(string queryText, CommonQueryType queryType, string connectionType)
        {
            mLogger.Debug("Start->Executing Query Object");
            RDBConnectionManager mDbConn = null;
            RQueryType radQueryType;

            try
            {
                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(
                    RConfigReader.GetConfigAppSettings(connectionType));

                radQueryType = (RQueryType)Enum.Parse(typeof(RQueryType), queryType.ToString());
                mDbConn.ExecuteQueryObject(DALWrapperAppend.Replace(queryText), radQueryType);

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
                mLogger.Debug("End->Executing Query Object");
            }
        }

        public static RHashlist ExecuteProcedureObject(string queryID, RHashlist paramaters, string connectionType)
        {
            mLogger.Debug("Start->Executing Procedure Object");
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

                return mDbConn.ExecuteProcedureObject(queryID, paramaters);
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
                mLogger.Debug("End->Executing Procedure Object");
            }
        }

        public static ObjectSet GetTableSchemaObject(string tableName, string connectionType)
        {
            mLogger.Debug("Start -> Getting Table Schema Object");
            RDBConnectionManager mDbConn = null;

            try
            {
                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(
                    RConfigReader.GetConfigAppSettings(connectionType));
                return mDbConn.ExecuteQueryToGetSchemaObject(DALWrapperAppend.Replace(tableName));

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
                mLogger.Debug("End -> Getting Table Schema Object");
            }
        }

        public static void ExecuteBulkUploadObject(string tableName, ObjectTable dtTable, string connectionType)
        {
            mLogger.Debug("Start -> Bulk Uploading Object");
            RDBConnectionManager mDbConn = null;

            try
            {
                mDbConn = RDALAbstractFactory.DBFactory.GetConnectionManager(
                    RConfigReader.GetConfigAppSettings(connectionType));
                mDbConn.ExecuteBulkCopyObject(DALWrapperAppend.Replace(tableName), dtTable, 100000);
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
                mLogger.Debug("End -> Bulk Uploading Object");
            }
        }
    }
}
