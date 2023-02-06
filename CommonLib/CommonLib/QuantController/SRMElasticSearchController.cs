using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Net;
using System.IO;
using com.ivp.rad.utils;
using com.ivp.rad.common;
using Newtonsoft.Json;
using System.Dynamic;
using System.Globalization;
using com.ivp.rad.dal;
using System.Configuration;
using System.Timers;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using com.ivp.commom.commondal;

namespace com.ivp.common
{
    public class SMElasticSearchSQLAPI
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("SMElasticSearchSQLAPI");
        public static List<string> getSectypes()
        {
            return CommonDALWrapper.ExecuteSelectQuery("SELECT sectype_name FROM IVPSecMaster.dbo.ivp_secm_sectype_master WHERE is_active = 1 AND is_complete = 1;", ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["sectype_name"])).ToList();
        }

        public static Dictionary<int, Dictionary<string, string>> fetchTableSchema(string sectype)
        {
            mLogger.Error("FetchTableSchema ---------> START");
            Dictionary<int, Dictionary<string, string>> tableVscolumnVstype = new Dictionary<int, Dictionary<string, string>>();
            Dictionary<int, int> tableVspriority = new Dictionary<int, int>();
            DataTable dt = new DataTable();

            RDBConnectionManager dbConn = CommonDALWrapper.GetConnectionManager(ConnectionConstants.SecMaster_Connection, true, IsolationLevel.Serializable);
            try
            {
                StringBuilder query = new StringBuilder("SELECT sm.sectype_id, priority FROM IVPSecMaster.dbo.ivp_secm_sectype_table st INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_master sm ON st.sectype_id = sm.sectype_id WHERE priority = 1 AND sectype_name = '").Append(sectype).Append("';");

                dt = CommonDALWrapper.ExecuteSelectQuery(query.ToString(), dbConn).Tables[0];
                foreach (DataRow dr in dt.AsEnumerable())
                {
                    int s = Convert.ToInt32(dr["sectype_id"]);
                    tableVspriority.Add(s, Convert.ToInt32(dr["priority"]));
                    tableVscolumnVstype.Add(s, new Dictionary<string, string>());
                }
                foreach (KeyValuePair<int, Dictionary<string, string>> kvp in tableVscolumnVstype)
                {
                    query = new StringBuilder(string.Format(@"SELECT attribute_name,data_type_name FROM IVPSecMaster.dbo.ivp_secm_sectype_table st
                    INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_details ad ON ad.sectype_table_id = st.sectype_table_id 
                    INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_subtype ass ON ass.attribute_subtype_id = ad.attribute_subtype_id
                    INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_type att ON ass.attribute_type_id = att.attribute_type_id
                    WHERE (st.priority = 1 AND st.sectype_id = {0}) OR sectype_id = 0", kvp.Key));

                    mLogger.Error("Query : " + query.ToString());

                    Dictionary<string, string> temp = CommonDALWrapper.ExecuteSelectQuery(query.ToString(), dbConn).Tables[0].AsEnumerable().ToDictionary(x => Convert.ToString(x["attribute_name"]), y => Convert.ToString(y["data_type_name"]));
                    temp.Add("created_on", "DATETIME");
                    temp.Add("last_modified_on", "DATETIME");
                    temp.Add("created_by", "STRING");
                    temp.Add("last_modified_by", "STRING");
                    temp.Add("sectype_id", "NUMERIC");
                    temp.Add("sectype_name", "STRING");
                    temp.Add("sec_id", "STRING");

                    foreach (var kvpp in temp)
                        tableVscolumnVstype[kvp.Key].Add(kvpp.Key, kvpp.Value);
                }
                dbConn.CommitTransaction();
            }
            catch (Exception ee)
            {
                dbConn.RollbackTransaction();
                mLogger.Error("FetchTableSchema ---------> ERROR : " + ee.ToString());
                throw ee;
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(dbConn);
            }
            mLogger.Error("FetchTableSchema ---------> END");
            return tableVscolumnVstype;
        }

        public static Dictionary<KeyValuePair<int, string>, DataTable> fetchTableData(string sectype, List<string> secIds = null)
        {
            mLogger.Error("FetchTableData ---------> START");
            List<string> tableNames = new List<string>();
            Dictionary<int, string> sectypeIdVsTableName = new Dictionary<int, string>();
            Dictionary<KeyValuePair<int, string>, DataTable> tableVscolumnVstype = new Dictionary<KeyValuePair<int, string>, DataTable>();
            DataTable dt = null;

            RDBConnectionManager dbConn = CommonDALWrapper.GetConnectionManager(ConnectionConstants.SecMaster_Connection, true, IsolationLevel.Serializable);

            try
            {
                StringBuilder query = new StringBuilder("SELECT sm.sectype_id, fully_qualified_table_name, ad.attribute_name AS [primary_attribute], priority FROM IVPSecMaster.dbo.ivp_secm_sectype_table st INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_master sm ON (st.sectype_id = sm.sectype_id AND priority = 1) LEFT JOIN IVPSecMaster.dbo.ivp_secm_attribute_details ad ON ad.attribute_id = st.primary_attribute_id WHERE sectype_name = '").Append(sectype).Append("';");

                dt = CommonDALWrapper.ExecuteSelectQuery(query.ToString(), dbConn).Tables[0];

                foreach (DataRow dr in dt.AsEnumerable())
                {
                    int sectypeId = Convert.ToInt32(dr["sectype_id"]);
                    string pr = Convert.ToString(dr["primary_attribute"]);
                    string fqt = Convert.ToString(dr["fully_qualified_table_name"]);

                    if (string.IsNullOrEmpty(pr))
                    {
                        int priority = Convert.ToInt32(dr["priority"]);
                        if (priority == 1)
                            pr = "sec_id";
                        else if (priority > 1)
                            pr = "sec_id";
                        else
                            pr = string.Empty;
                    }
                    tableNames.Add(fqt);
                    sectypeIdVsTableName.Add(sectypeId, fqt);
                    tableVscolumnVstype.Add(new KeyValuePair<int, string>(sectypeId, pr), new DataTable());
                }
                foreach (KeyValuePair<int, string> item in sectypeIdVsTableName)
                {
                    dt = new DataTable();

                    if (secIds != null)
                    {
                        query = new StringBuilder(string.Format(@"DECLARE @tab TABLE(sec_id VARCHAR(100));
                        INSERT INTO @tab
                        SELECT item FROM IVPSecMaster.dbo.SECM_GetList2Table('{1}',',');
                        
                        SELECT sm.sectype_id, smas.sectype_name, sm.created_by, sm.created_on, sm.last_modified_on, sm.last_modified_by, tab.* 
                        FROM {0} tab 
                        INNER JOIN @tab tt ON tab.sec_id = tt.sec_id
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_sec_master sm ON (tab.sec_id = sm.sec_id AND sm.is_active = 1) 
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_master smas ON (sm.sectype_id = smas.sectype_id AND smas.is_active = 1) ", item.Value, string.Join(",", secIds)));
                    }
                    else
                    {
                        query = new StringBuilder(string.Format(@"
                        SELECT sm.sectype_id, smas.sectype_name, sm.created_by, sm.created_on, sm.last_modified_on, sm.last_modified_by, tab.* 
                        FROM {0} tab 
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_sec_master sm ON (tab.sec_id = sm.sec_id AND sm.is_active = 1) 
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_master smas ON (sm.sectype_id = smas.sectype_id AND smas.is_active = 1) ", item.Value));
                    }

                    dt = CommonDALWrapper.ExecuteSelectQuery(query.ToString(), dbConn).Tables[0];
                    IEnumerable<KeyValuePair<KeyValuePair<int, string>, DataTable>> ie = tableVscolumnVstype.Where(x => x.Key.Key == item.Key);
                    if (ie.Count() > 0)
                        tableVscolumnVstype[ie.FirstOrDefault().Key] = dt;
                }
                dbConn.CommitTransaction();
            }
            catch (Exception ee)
            {
                dbConn.RollbackTransaction();
                mLogger.Error("FetchTableData ---------> ERORR : " + ee.ToString());
                throw ee;
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(dbConn);
            }
            mLogger.Error("FetchTableData ---------> END");
            return tableVscolumnVstype;
        }

        public static List<string> fetchSecIds(string sectype, DateTime? lastTime)
        {
            mLogger.Error("FetchSecIds ---------> START");
            List<string> secs = null;
            RDBConnectionManager dbConn = CommonDALWrapper.GetConnectionManager(ConnectionConstants.SecMaster_Connection, true, IsolationLevel.Serializable);

            try
            {
                StringBuilder query = new StringBuilder();

                if (lastTime == null)
                    query.Append("SELECT sm.sec_id FROM IVPSecMaster.dbo.ivp_secm_sec_master sm INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_master smas ON (sm.sectype_id = smas.sectype_id AND smas.is_active = 1) WHERE sectype_name = '").Append(sectype).Append("' AND sm.is_active = 1;");
                else
                    query.Append("SELECT sm.sec_id FROM IVPSecMaster.dbo.ivp_secm_sec_master sm INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_master smas ON (sm.sectype_id = smas.sectype_id AND smas.is_active = 1) WHERE sectype_name = '").Append(sectype).Append("' AND sm.is_active = 1 AND sm.last_modified_on >= '").Append(lastTime).Append("';");

                secs = CommonDALWrapper.ExecuteSelectQuery(query.ToString(), dbConn).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["sec_id"])).OrderBy(y => y).ToList();

                dbConn.CommitTransaction();
            }
            catch (Exception ee)
            {
                dbConn.RollbackTransaction();
                mLogger.Error("FetchSecIds ---------> ERROR : " + ee.ToString());
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(dbConn);
            }
            mLogger.Error("FetchSecIds ---------> END");
            return secs;
        }

        public static void populateElasticSearchMappingTable()
        {
            mLogger.Error("PopulateElasticSearchMappingTable ---------> START");
            RDBConnectionManager dbConn = CommonDALWrapper.GetConnectionManager(ConnectionConstants.SecMaster_Connection, true, IsolationLevel.Serializable);

            try
            {
                StringBuilder query = new StringBuilder(@"TRUNCATE TABLE IVPSecMaster.dbo.ivp_secm_elastic_search_mapping; 

                    INSERT INTO IVPSecMaster.dbo.ivp_secm_elastic_search_mapping 
                    SELECT sectype_id, REPLACE(LOWER(sectype_name),' ', '_') AS index_name 
                    FROM IVPSecMaster.dbo.ivp_secm_sectype_master WHERE is_active = 1 AND is_complete = 1;");

                CommonDALWrapper.ExecuteQuery(query.ToString(), CommonQueryType.Insert, dbConn);
                dbConn.CommitTransaction();
            }
            catch (Exception ee)
            {
                dbConn.RollbackTransaction();
                mLogger.Error("PopulateElasticSearchMappingTable ---------> ERROR : " + ee.ToString());
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(dbConn);
            }
            mLogger.Error("PopulateElasticSearchMappingTable ---------> END");
        }

        public static Dictionary<int, string> fetchElasticSearchMappingTable()
        {
            mLogger.Error("FetchElasticSearchMappingTable ---------> START");
            Dictionary<int, string> sectypeVsIndexName = new Dictionary<int, string>();
            RDBConnectionManager dbConn = CommonDALWrapper.GetConnectionManager(ConnectionConstants.SecMaster_Connection, true, IsolationLevel.Serializable);

            try
            {
                sectypeVsIndexName = CommonDALWrapper.ExecuteSelectQuery("SELECT * FROM IVPSecMaster.dbo.ivp_secm_elastic_search_mapping;", dbConn).Tables[0].AsEnumerable().ToDictionary(x => Convert.ToInt32(x["sectype_id"]), y => Convert.ToString(y["index_name"]));
                dbConn.CommitTransaction();
            }
            catch (Exception ee)
            {
                dbConn.RollbackTransaction();
                mLogger.Error("FetchElasticSearchMappingTable ---------> ERROR : " + ee.ToString());
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(dbConn);
            }
            return sectypeVsIndexName;
            mLogger.Error("FetchElasticSearchMappingTable ---------> END");
        }
    }

    public class ElasticAPI
    {
        private static readonly string url = null;
        private static IRLogger mLogger = RLogFactory.CreateLogger("ElasticAPI");

        static ElasticAPI()
        {
            url = RConfigReader.GetConfigAppSettings("ElasticSearchUrl");
        }

        public static bool BulkInsert(string index, int type, DataTable data, string primaryAttribute, out string output)
        {
            bool hasFailed = false;
            output = string.Empty;
            StringBuilder json = new StringBuilder();

            for (int i = 0; i < data.Rows.Count; i++)
            {
                DataRow dr = data.Rows[i];
                string temp = string.Empty;
                if (!string.IsNullOrEmpty(primaryAttribute))
                    temp = string.Format(", \"_id\" : \"{0}\"", Convert.ToString(dr[primaryAttribute]));

                json.AppendFormat("{{\"index\" : {{ \"_index\" : \"{0}\", \"_type\" : \"{1}\"{2} }} }}", index, type, temp);
                json.AppendLine();
                json.Append("{");
                foreach (DataColumn dc in data.Columns)
                {
                    if (dc.DataType == typeof(string))
                        json.AppendFormat("\"{0}\":\"{1}\",", dc.ColumnName, Convert.ToString(dr[dc.ColumnName]));
                    else if (dc.DataType == typeof(DateTime))
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(dr[dc.ColumnName])))
                            json.AppendFormat("\"{0}\":{1},", dc.ColumnName, "null");
                        else
                            json.AppendFormat("\"{0}\":\"{1}\",", dc.ColumnName, Convert.ToDateTime(dr[dc.ColumnName]).ToString(@"yyyy-MM-dd hh\:mm\:ss.fff"));//.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff"));
                    }
                    else if (dc.DataType == typeof(TimeSpan))
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(dr[dc.ColumnName])))
                            json.AppendFormat("\"{0}\":{1},", dc.ColumnName, "null");
                        else
                            json.AppendFormat("\"{0}\":\"{1}\",", dc.ColumnName, ((TimeSpan)dr[dc.ColumnName]).ToString(@"hh\:mm\:ss\.fff"));
                    }
                    else if (dc.DataType == typeof(bool))
                        json.AppendFormat("\"{0}\":{1},", dc.ColumnName, (string.IsNullOrEmpty(Convert.ToString(dr[dc.ColumnName])) ? "false" : (Convert.ToBoolean(dr[dc.ColumnName]) ? "true" : "false")));
                    else if (dc.DataType == typeof(int) || dc.DataType == typeof(float) || dc.DataType == typeof(double) || dc.DataType == typeof(Decimal))
                        json.AppendFormat("\"{0}\":{1},", dc.ColumnName, (string.IsNullOrEmpty(Convert.ToString(dr[dc.ColumnName])) ? Convert.ToString(0) : Convert.ToString(dr[dc.ColumnName])));
                }
                json.Remove(json.Length - 1, 1);
                json.Append("}");
                json.AppendLine();
            }
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url + "/_bulk");
                req.ServicePoint.Expect100Continue = false;
                req.KeepAlive = true;
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";

                StreamWriter requestStream = new StreamWriter(req.GetRequestStream());
                requestStream.Write(json.ToString());
                requestStream.Flush();
                requestStream.Close();


                using (HttpWebResponse responseStream = (HttpWebResponse)req.GetResponse())
                {
                    StreamReader sr = new StreamReader(responseStream.GetResponseStream());
                    output = sr.ReadToEnd();
                    responseStream.Close();
                }

            }
            catch (WebException we)
            {
                using (HttpWebResponse response = (HttpWebResponse)we.Response)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            output = readStream.ReadToEnd();
                            hasFailed = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                output = e.Message;
                hasFailed = true;
            }

            mLogger.Error("Bulk Insert Output : " + output);
            if (hasFailed == true)
            {
                throw new Exception(output);
            }
            else
                return true;
        }

        public static bool BulkDelete(string index, int type, string primaryAttribute, IEnumerable<string> secIdsToDelete, out string output)
        {
            bool hasFailed = false;
            output = string.Empty;
            StringBuilder json = new StringBuilder();

            foreach (string secId in secIdsToDelete)
            {
                if (!string.IsNullOrEmpty(primaryAttribute))
                {
                    json.AppendFormat("{{\"delete\" : {{ \"_index\" : \"{0}\", \"_type\" : \"{1}\", \"_id\" : \"{2}\" }} }}", index, type, secId);
                    json.AppendLine();
                }
            }
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url + "/_bulk");
                req.KeepAlive = true;
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";

                StreamWriter requestStream = new StreamWriter(req.GetRequestStream());
                requestStream.Write(json.ToString());
                requestStream.Flush();
                requestStream.Close();


                using (HttpWebResponse responseStream = (HttpWebResponse)req.GetResponse())
                {
                    StreamReader sr = new StreamReader(responseStream.GetResponseStream());
                    output = sr.ReadToEnd();
                    responseStream.Close();
                }

            }
            catch (WebException we)
            {
                using (HttpWebResponse response = (HttpWebResponse)we.Response)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            output = readStream.ReadToEnd();
                            hasFailed = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                output = e.Message;
                hasFailed = true;
            }
            if (hasFailed == true)
            {
                throw new Exception(output);
            }
            else
                return true;
        }

        public static bool CreateIndex(string index, Dictionary<int, Dictionary<string, string>> tableVscolumnVstype, out string output)
        {
            try
            {
                StringBuilder json = new StringBuilder("{\"settings\":{ \"index\":{ \"max_result_window\" : 100000, \"analysis\":{ \"analyzer\":{ \"analyzer_keyword\":{ \"tokenizer\":\"keyword\", \"filter\":\"lowercase\" } } } },\"number_of_replicas\" : 0},\"mappings\":{");//,\"analyzer_text\":{ \"tokenizer\":\"keyword\", \"filter\":\"lowercase\" }
                foreach (KeyValuePair<int, Dictionary<string, string>> kvp1 in tableVscolumnVstype)
                {
                    json.Append("\"").Append(kvp1.Key).Append("\":{\"properties\":{");
                    foreach (KeyValuePair<string, string> kvp in kvp1.Value)
                    {
                        string type = string.Empty;
                        switch (kvp.Value)
                        {
                            case "STRING": type = "text"; break;
                            case "NUMERIC": type = "double"; break;
                            case "DATETIME":
                            case "DATE":
                            case "TIME": type = "date"; break;
                            case "FILE": type = "text"; break;
                            case "BOOLEAN": type = "boolean"; break;
                        }

                        if (type == "double")
                            json.Append(string.Format("\"{0}\": {{\"type\":\"{1}\", \"include_in_all\": false }},", kvp.Key, type));

                        else if (type == "boolean")
                            json.Append(string.Format("\"{0}\": {{\"type\":\"{1}\", \"include_in_all\": false }},", kvp.Key, type));

                        else if (type == "date")
                            json.Append(string.Format("\"{0}\": {{\"type\":\"{1}\", \"format\":\"yyyy-MM-dd HH:mm:ss.SSS||hour_minute_second_fraction\", \"include_in_all\": false }},", kvp.Key, type));

                        else if (type == "text")
                            json.Append("\"" + kvp.Key + "\": {\"type\":\"text\", \"fielddata\":true, \"fields\": { \"keyword\": { \"type\": \"text\", \"fielddata\":true, \"analyzer\": \"analyzer_keyword\" } }, \"include_in_all\": false },");//, \"analyzer\": \"analyzer_text\"
                    }
                    json.Remove(json.Length - 1, 1);
                    json.Append("}},");
                }
                json.Remove(json.Length - 1, 1);
                json.Append("}}");

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url + "/" + index);
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "PUT";
                req.KeepAlive = true;

                StreamWriter requestStream = new StreamWriter(req.GetRequestStream());
                requestStream.Write(json.ToString());
                requestStream.Flush();
                requestStream.Close();

                output = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            output = readStream.ReadToEnd();
                        }
                    }
                }
                return true;
            }
            catch (WebException we)
            {
                using (HttpWebResponse response = (HttpWebResponse)we.Response)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            output = readStream.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                output = e.Message;
            }
            return false;
        }

        public static bool GetAggregationQueryData(string indexName, string query, List<string> columns, out string output, int from = 0, int size = 10000)
        {
            output = string.Empty;
            try
            {
                StringBuilder json = new StringBuilder().Append(query);//
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url + "/" + indexName + "/" + "_search"); //+ "/" + _index + 
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";

                mLogger.Error("QueryData -----------> Query :" + json.ToString());

                StreamWriter requestStream = new StreamWriter(req.GetRequestStream());
                requestStream.Write(json.ToString());
                requestStream.Flush();
                requestStream.Close();

                output = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            output = readStream.ReadToEnd();
                        }
                    }
                }
                return true;
            }
            catch (WebException we)
            {
                using (HttpWebResponse response = (HttpWebResponse)we.Response)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            output = readStream.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                output = e.Message;
            }
            return false;
        }

        public static bool QueryData(string query, List<string> columns, out string output, int from = 0, int size = 10000)
        {
            output = string.Empty;
            try
            {
                StringBuilder json = new StringBuilder("{\"_source\" : [\"sec_id\",\"sectype_id\",\"sectype_name\",\"" + string.Join("\",\"", columns) + "\"], \"from\":" + from + ", \"size\":" + size + ", \"query\": ").Append(query).Append(" }");//
                //StringBuilder json = new StringBuilder("{\"_source\" : [], \"from\":" + from + ", \"size\":" + size + ", \"query\": ").Append(query).Append(" }");//
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url + "/" + "_search"); //+ "/" + _index + 
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";

                mLogger.Error("QueryData -----------> Query :" + json.ToString());

                StreamWriter requestStream = new StreamWriter(req.GetRequestStream());
                requestStream.Write(json.ToString());
                requestStream.Flush();
                requestStream.Close();

                output = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            output = readStream.ReadToEnd();
                        }
                    }
                }
                return true;
            }
            catch (WebException we)
            {
                using (HttpWebResponse response = (HttpWebResponse)we.Response)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            output = readStream.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                output = e.Message;
            }
            return false;
        }

        public static bool Count(string query, out string output)
        {
            output = string.Empty;
            try
            {
                StringBuilder json = new StringBuilder("{ \"query\": ").Append(query).Append(" }");//
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url + "/" + "_count"); //+ "/" + _index + 
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "POST";

                mLogger.Error("Count -----------> Query :" + json.ToString());

                StreamWriter requestStream = new StreamWriter(req.GetRequestStream());
                requestStream.Write(json.ToString());
                requestStream.Flush();
                requestStream.Close();

                output = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            output = readStream.ReadToEnd();
                        }
                    }
                }
                return true;
            }
            catch (WebException we)
            {
                using (HttpWebResponse response = (HttpWebResponse)we.Response)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            output = readStream.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                output = e.Message;
            }
            return false;
        }

        public static bool DeleteIndex(string index, out string output)
        {
            output = string.Empty;
            try
            {
                string urll = string.Empty;
                if (string.IsNullOrEmpty(index))
                    urll = url + "/" + "_all";
                else
                    urll = url + "/" + index;

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(urll);
                req.ContentType = "application/x-www-form-urlencoded";
                req.Method = "DELETE";
                req.KeepAlive = true;

                output = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            output = readStream.ReadToEnd();
                        }
                    }
                }
                return true;
            }
            catch (WebException we)
            {
                using (HttpWebResponse response = (HttpWebResponse)we.Response)
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        using (StreamReader readStream = new StreamReader(responseStream, Encoding.UTF8))
                        {
                            output = readStream.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                output = e.Message;
            }
            return false;
        }
    }

    public static class SMElasticSearchController
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("SMElasticSearchController");
        private static List<string> freeTextAttributes = new List<string>();

        static SMElasticSearchController()
        {
            string attributes = RConfigReader.GetConfigAppSettings("FreeTextSearchAttributes");
            if (!string.IsNullOrEmpty(attributes))
            {
                freeTextAttributes = CommonDALWrapper.ExecuteSelectQuery(@"DECLARE @attr VARCHAR(MAX) = '" + attributes + @"'
                        DECLARE @tab TABLE (sectype_name VARCHAR(MAX), attribute_name VARCHAR(MAX))
                        DECLARE @ret TABLE (attribute_name VARCHAR(MAX))

                        INSERT INTO @tab(attribute_name)
                        SELECT item FROM IVPSecMaster.dbo.SECM_GetList2Table(@attr,',')

                        UPDATE @tab
                        SET sectype_name = SUBSTRING(attribute_name,1, CHARINDEX('-',attribute_name) -1),
	                        attribute_name = SUBSTRING(attribute_name,CHARINDEX('-',attribute_name) + 1, LEN(attribute_name) - CHARINDEX('-',attribute_name))
                        FROM @tab
                        WHERE attribute_name LIKE '%-%'

                        INSERT INTO @ret
                        SELECT DISTINCT ad.attribute_name 
                        FROM IVPSecMaster.dbo.ivp_secm_attribute_details ad 
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_template_details td ON ad.attribute_id = td.attribute_id 
                        INNER JOIN @tab tt ON td.display_name = tt.attribute_name
                        WHERE ad.is_active = 1 AND td.is_active = 1 AND tt.sectype_name IS NULL

                        INSERT INTO @ret
                        SELECT DISTINCT ad.attribute_name 
                        FROM IVPSecMaster.dbo.ivp_secm_attribute_details ad 
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_template_details td ON ad.attribute_id = td.attribute_id 
                        INNER JOIN @tab tt ON td.display_name = tt.attribute_name
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_table st ON ad.sectype_table_id = st.sectype_table_id
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_master sm ON st.sectype_id = st.sectype_id
                        WHERE ad.is_active = 1 AND td.is_active = 1 AND tt.sectype_name IS NOT NULL AND sm.sectype_name = tt.sectype_name


                        SELECT * FROM @ret", ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["attribute_name"]) + ".keyword").ToList();

                if (!freeTextAttributes.Contains("sec_id"))
                    freeTextAttributes.Add("sec_id");
            }
        }

        public static ESQueryResposeInfo Search(ESQueryRequestInfo requestInfo)
        {
            mLogger.Error("Search ----------> START");

            string output;
            ESQueryResposeInfo responseInfo = new ESQueryResposeInfo();
            HashSet<string> SecIdsPushedToDownstream;
            try
            {
                dynamic queryObject = QueryBuilder(requestInfo, out SecIdsPushedToDownstream);
                string query = JsonConvert.SerializeObject(queryObject);

                bool result = ElasticAPI.QueryData(query, requestInfo.RequiredAttributeRealNames, out output, requestInfo.From, requestInfo.Size);

                if (result)
                {
                    responseInfo.IsSuccess = true;
                    responseInfo.FailureMessage = string.Empty;
                    responseInfo.SecIdsPushedToDownstream = SecIdsPushedToDownstream;

                    dynamic deserializedResult = JsonConvert.DeserializeObject<dynamic>(output);
                    if (requestInfo.RequireDataTable)
                    {
                    }
                    else
                        responseInfo.ResultObject = deserializedResult;
                }
                else
                {
                    responseInfo.IsSuccess = false;
                    responseInfo.FailureMessage = output;
                }
            }
            catch (Exception ee)
            {
                mLogger.Error("Search :Exception  ------------>" + ee.ToString());
                responseInfo.IsSuccess = false;
                responseInfo.FailureMessage = ee.ToString();
            }

            mLogger.Error("Search ----------> END");
            return responseInfo;
        }

        public static int Count(ESQueryRequestInfo requestInfo)
        {
            mLogger.Error("Count ----------> START");

            int count = 0;
            string output;
            ESQueryResposeInfo responseInfo = new ESQueryResposeInfo();
            HashSet<string> SecIdsPushedToDownstream;
            try
            {
                dynamic queryObject = QueryBuilder(requestInfo, out SecIdsPushedToDownstream);
                string query = JsonConvert.SerializeObject(queryObject);

                bool result = ElasticAPI.Count(query, out output);

                if (result)
                {
                    dynamic deserializedResult = JsonConvert.DeserializeObject<dynamic>(output);
                    count = Convert.ToInt32(deserializedResult.count);
                }
            }
            catch (Exception ee)
            {
                mLogger.Error("Count :Exception  ------------>" + ee.ToString());
            }

            mLogger.Error("Count ----------> END");
            return count;
        }

        private static dynamic GetAdvancedQuery(ESQueryEntity entity)
        {
            dynamic qsObject = new ExpandoObject();

            switch (entity.MatchType)
            {
                case ESAdvancedQueryMatchType.BEGINS_WITH:
                    qsObject.query_string = new ExpandoObject();
                    qsObject.query_string.fields = new List<string> { entity.AttributeRealName + ".keyword" };
                    qsObject.query_string.query = entity.Value.ToLower().Replace(" ", "\\ ") + "*";
                    break;

                case ESAdvancedQueryMatchType.ENDS_WITH:
                    qsObject.query_string = new ExpandoObject();
                    qsObject.query_string.fields = new List<string> { entity.AttributeRealName + ".keyword" };
                    qsObject.query_string.query = "*" + entity.Value.ToLower().Replace(" ", "\\ ");
                    break;

                case ESAdvancedQueryMatchType.CONTAIN:
                    qsObject.query_string = new ExpandoObject();
                    qsObject.query_string.fields = new List<string> { entity.AttributeRealName + ".keyword" };
                    qsObject.query_string.query = "*" + entity.Value.ToLower().Replace(" ", "\\ ") + "*";
                    break;

                case ESAdvancedQueryMatchType.DOES_NOT_CONTAIN:
                    dynamic must_not = new ExpandoObject();
                    must_not.query_string = new ExpandoObject();
                    must_not.query_string.fields = new List<string> { entity.AttributeRealName + ".keyword" };
                    must_not.query_string.query = "*" + entity.Value.ToLower().Replace(" ", "\\ ") + "*";
                    ((IDictionary<String, Object>)qsObject)["bool"] = new ExpandoObject();
                    ((IDictionary<String, Object>)(((IDictionary<String, Object>)qsObject)["bool"]))["must_not"] = must_not;
                    break;

                case ESAdvancedQueryMatchType.DOES_NOT_EQUAL:
                    dynamic must_not_equal = new ExpandoObject();
                    must_not_equal.query_string = new ExpandoObject();

                    switch (entity.DataType)
                    {
                        case ESValueDataType.BOOLEAN:
                            bool outResult;
                            Boolean.TryParse(entity.Value, out outResult);
                            must_not_equal.query_string.fields = new List<string> { entity.AttributeRealName };
                            must_not_equal.query_string.query = outResult;
                            break;
                        case ESValueDataType.DATE:
                            DateTime outDateResult;
                            DateTime.TryParse(entity.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateResult);
                            must_not_equal.query_string.fields = new List<string> { entity.AttributeRealName };
                            must_not_equal.query_string.query = outDateResult.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.TIME:
                            TimeSpan outTimeResult;
                            TimeSpan.TryParse(entity.Value, out outTimeResult);
                            must_not_equal.query_string.fields = new List<string> { entity.AttributeRealName };
                            must_not_equal.query_string.query = outTimeResult.ToString(@"hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.DATETIME:
                            DateTime outDateTimeResult;
                            DateTime.TryParse(entity.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateTimeResult);
                            must_not_equal.query_string.fields = new List<string> { entity.AttributeRealName };
                            must_not_equal.query_string.query = outDateTimeResult.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.NUMERIC:
                            must_not_equal.query_string.fields = new List<string> { entity.AttributeRealName };
                            must_not_equal.query_string.query = Convert.ToDouble(entity.Value);
                            break;
                        case ESValueDataType.STRING:
                            must_not_equal.query_string.fields = new List<string> { entity.AttributeRealName + ".keyword" };
                            must_not_equal.query_string.query = entity.Value.ToLower().Replace(" ", "\\ ");
                            break;
                    }
                    ((IDictionary<String, Object>)qsObject)["bool"] = new ExpandoObject();
                    ((IDictionary<String, Object>)(((IDictionary<String, Object>)qsObject)["bool"]))["must_not"] = must_not_equal;
                    break;

                case ESAdvancedQueryMatchType.EQUALS:
                    qsObject.query_string = new ExpandoObject();

                    switch (entity.DataType)
                    {
                        case ESValueDataType.BOOLEAN:
                            bool outResult;
                            Boolean.TryParse(entity.Value, out outResult);
                            qsObject.query_string.fields = new List<string> { entity.AttributeRealName };
                            qsObject.query_string.query = outResult;
                            break;
                        case ESValueDataType.DATE:
                            DateTime outDateResult;
                            DateTime.TryParse(entity.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateResult);
                            qsObject.query_string.fields = new List<string> { entity.AttributeRealName };
                            qsObject.query_string.query = outDateResult.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.TIME:
                            TimeSpan outTimeResult;
                            TimeSpan.TryParse(entity.Value, out outTimeResult);
                            qsObject.query_string.fields = new List<string> { entity.AttributeRealName };
                            qsObject.query_string.query = outTimeResult.ToString(@"hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.DATETIME:
                            DateTime outDateTimeResult;
                            DateTime.TryParse(entity.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateTimeResult);
                            qsObject.query_string.fields = new List<string> { entity.AttributeRealName };
                            qsObject.query_string.query = outDateTimeResult.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.NUMERIC:
                            qsObject.query_string.fields = new List<string> { entity.AttributeRealName };
                            qsObject.query_string.query = Convert.ToDouble(entity.Value);
                            break;
                        case ESValueDataType.STRING:
                            qsObject.query_string.fields = new List<string> { entity.AttributeRealName + ".keyword" };
                            qsObject.query_string.query = entity.Value.ToLower().Replace(" ", "\\ ");
                            break;
                    }
                    break;

                case ESAdvancedQueryMatchType.IN:
                    qsObject.query_string = new ExpandoObject();
                    List<string> vals = entity.Value.Split(new string[] { " OR " }, StringSplitOptions.None).ToList();

                    switch (entity.DataType)
                    {
                        case ESValueDataType.BOOLEAN:
                            bool outResult;
                            Boolean.TryParse(entity.Value, out outResult);
                            qsObject.query_string.fields = new List<string> { entity.AttributeRealName };
                            qsObject.query_string.query = outResult;
                            break;
                        case ESValueDataType.DATE:
                            DateTime outDateResult;
                            DateTime.TryParse(entity.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateResult);
                            qsObject.query_string.fields = new List<string> { entity.AttributeRealName };
                            qsObject.query_string.query = outDateResult.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.TIME:
                            TimeSpan outTimeResult;
                            TimeSpan.TryParse(entity.Value, out outTimeResult);
                            qsObject.query_string.fields = new List<string> { entity.AttributeRealName };
                            qsObject.query_string.query = outTimeResult.ToString(@"hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.DATETIME:
                            DateTime outDateTimeResult;
                            DateTime.TryParse(entity.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateTimeResult);
                            qsObject.query_string.fields = new List<string> { entity.AttributeRealName };
                            qsObject.query_string.query = outDateTimeResult.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.NUMERIC:
                            qsObject.query_string.fields = new List<string> { entity.AttributeRealName };
                            qsObject.query_string.query = Convert.ToDouble(entity.Value);
                            break;
                        case ESValueDataType.STRING:
                            qsObject.query_string.fields = new List<string> { entity.AttributeRealName };
                            string finalVal = "";
                            foreach (string strVal in vals)
                            {
                                finalVal = finalVal + (finalVal == "" ? "" : " OR ") + strVal.ToLower().Replace(" ", "\\ ");
                            }
                            qsObject.query_string.query = finalVal;
                            break;
                    }


                    break;

                case ESAdvancedQueryMatchType.BETWEEN:
                    dynamic betweenObject = new ExpandoObject();
                    betweenObject.range = new ExpandoObject();
                    ((IDictionary<String, Object>)betweenObject.range)[entity.AttributeRealName] = new ExpandoObject();
                    //betweenObject.range[entity.AttributeRealName] = new ExpandoObject();

                    switch (entity.DataType)
                    {
                        case ESValueDataType.DATE:
                            DateTime outDateResult;
                            DateTime.TryParse(entity.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateResult);
                            DateTime outDateResult2;
                            DateTime.TryParse(entity.Value2, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateResult2);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)betweenObject.range)[entity.AttributeRealName]))["gte"] = outDateResult.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)betweenObject.range)[entity.AttributeRealName]))["lte"] = outDateResult2.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.TIME:
                            TimeSpan outTimeResult;
                            TimeSpan.TryParse(entity.Value, out outTimeResult);
                            TimeSpan outTimeResult2;
                            TimeSpan.TryParse(entity.Value2, out outTimeResult2);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)betweenObject.range)[entity.AttributeRealName]))["gte"] = outTimeResult.ToString(@"hh\:mm\:ss\.fff");
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)betweenObject.range)[entity.AttributeRealName]))["lte"] = outTimeResult2.ToString(@"hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.DATETIME:
                            DateTime outDateTimeResult;
                            DateTime.TryParse(entity.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateTimeResult);
                            DateTime outDateTimeResult2;
                            DateTime.TryParse(entity.Value2, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateTimeResult2);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)betweenObject.range)[entity.AttributeRealName]))["gte"] = outDateTimeResult.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)betweenObject.range)[entity.AttributeRealName]))["lte"] = outDateTimeResult2.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.NUMERIC:
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)betweenObject.range)[entity.AttributeRealName]))["gte"] = Convert.ToDouble(entity.Value);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)betweenObject.range)[entity.AttributeRealName]))["lte"] = Convert.ToDouble(entity.Value2);
                            break;
                    }
                    qsObject = betweenObject;
                    break;


                case ESAdvancedQueryMatchType.GREATER_THAN:
                case ESAdvancedQueryMatchType.AFTER:
                    dynamic gtObject = new ExpandoObject();
                    gtObject.range = new ExpandoObject();
                    ((IDictionary<String, Object>)gtObject.range)[entity.AttributeRealName] = new ExpandoObject();

                    switch (entity.DataType)
                    {
                        case ESValueDataType.DATE:
                            DateTime outDateResult;
                            DateTime.TryParse(entity.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateResult);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)gtObject.range)[entity.AttributeRealName]))["gt"] = outDateResult.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.TIME:
                            TimeSpan outTimeResult;
                            TimeSpan.TryParse(entity.Value, out outTimeResult);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)gtObject.range)[entity.AttributeRealName]))["gt"] = outTimeResult.ToString(@"hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.DATETIME:
                            DateTime outDateTimeResult;
                            DateTime.TryParse(entity.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateTimeResult);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)gtObject.range)[entity.AttributeRealName]))["gt"] = outDateTimeResult.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.NUMERIC:
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)gtObject.range)[entity.AttributeRealName]))["gt"] = Convert.ToDouble(entity.Value);
                            break;
                    }
                    qsObject = gtObject;
                    break;

                case ESAdvancedQueryMatchType.GREATER_OR_EQUAL:
                    dynamic gteObject = new ExpandoObject();
                    gteObject.range = new ExpandoObject();
                    ((IDictionary<String, Object>)gteObject.range)[entity.AttributeRealName] = new ExpandoObject();

                    switch (entity.DataType)
                    {
                        case ESValueDataType.DATE:
                            DateTime outDateResult;
                            DateTime.TryParse(entity.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateResult);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)gteObject.range)[entity.AttributeRealName]))["gte"] = outDateResult.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.TIME:
                            TimeSpan outTimeResult;
                            TimeSpan.TryParse(entity.Value, out outTimeResult);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)gteObject.range)[entity.AttributeRealName]))["gte"] = outTimeResult.ToString(@"hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.DATETIME:
                            DateTime outDateTimeResult;
                            DateTime.TryParse(entity.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateTimeResult);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)gteObject.range)[entity.AttributeRealName]))["gte"] = outDateTimeResult.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.NUMERIC:
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)gteObject.range)[entity.AttributeRealName]))["gte"] = Convert.ToDouble(entity.Value);
                            break;
                    }
                    qsObject = gteObject;
                    break;

                case ESAdvancedQueryMatchType.LESS_OR_EQUAL:
                    dynamic lteObject = new ExpandoObject();
                    lteObject.range = new ExpandoObject();
                    ((IDictionary<String, Object>)lteObject.range)[entity.AttributeRealName] = new ExpandoObject();

                    switch (entity.DataType)
                    {
                        case ESValueDataType.DATE:
                            DateTime outDateResult;
                            DateTime.TryParse(entity.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateResult);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)lteObject.range)[entity.AttributeRealName]))["lte"] = outDateResult.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.TIME:
                            TimeSpan outTimeResult;
                            TimeSpan.TryParse(entity.Value, out outTimeResult);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)lteObject.range)[entity.AttributeRealName]))["lte"] = outTimeResult.ToString(@"hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.DATETIME:
                            DateTime outDateTimeResult;
                            DateTime.TryParse(entity.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateTimeResult);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)lteObject.range)[entity.AttributeRealName]))["lte"] = outDateTimeResult.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.NUMERIC:
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)lteObject.range)[entity.AttributeRealName]))["lte"] = Convert.ToDouble(entity.Value);
                            break;
                    }
                    qsObject = lteObject;
                    break;

                case ESAdvancedQueryMatchType.LESS_THAN:
                case ESAdvancedQueryMatchType.BEFORE:
                    dynamic ltObject = new ExpandoObject();
                    ltObject.range = new ExpandoObject();
                    ((IDictionary<String, Object>)ltObject.range)[entity.AttributeRealName] = new ExpandoObject();

                    switch (entity.DataType)
                    {
                        case ESValueDataType.DATE:
                            DateTime outDateResult;
                            DateTime.TryParse(entity.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateResult);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)ltObject.range)[entity.AttributeRealName]))["lt"] = outDateResult.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.TIME:
                            TimeSpan outTimeResult;
                            TimeSpan.TryParse(entity.Value, out outTimeResult);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)ltObject.range)[entity.AttributeRealName]))["lt"] = outTimeResult.ToString(@"hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.DATETIME:
                            DateTime outDateTimeResult;
                            DateTime.TryParse(entity.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateTimeResult);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)ltObject.range)[entity.AttributeRealName]))["lt"] = outDateTimeResult.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.NUMERIC:
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)ltObject.range)[entity.AttributeRealName]))["lt"] = Convert.ToDouble(entity.Value);
                            break;
                    }
                    qsObject = ltObject;
                    break;


                case ESAdvancedQueryMatchType.OUTSIDE:
                    dynamic notbetweenObject = new ExpandoObject();
                    notbetweenObject.range = new ExpandoObject();
                    ((IDictionary<String, Object>)notbetweenObject.range)[entity.AttributeRealName] = new ExpandoObject();

                    switch (entity.DataType)
                    {
                        case ESValueDataType.DATE:
                            DateTime outDateResult;
                            DateTime.TryParse(entity.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateResult);
                            DateTime outDateResult2;
                            DateTime.TryParse(entity.Value2, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateResult2);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)notbetweenObject.range)[entity.AttributeRealName]))["gte"] = outDateResult.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)notbetweenObject.range)[entity.AttributeRealName]))["lte"] = outDateResult2.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.TIME:
                            TimeSpan outTimeResult;
                            TimeSpan.TryParse(entity.Value, out outTimeResult);
                            TimeSpan outTimeResult2;
                            TimeSpan.TryParse(entity.Value2, out outTimeResult2);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)notbetweenObject.range)[entity.AttributeRealName]))["gte"] = outTimeResult.ToString(@"hh\:mm\:ss\.fff");
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)notbetweenObject.range)[entity.AttributeRealName]))["lte"] = outTimeResult2.ToString(@"hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.DATETIME:
                            DateTime outDateTimeResult;
                            DateTime.TryParse(entity.Value, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateTimeResult);
                            DateTime outDateTimeResult2;
                            DateTime.TryParse(entity.Value2, CultureInfo.CreateSpecificCulture("en-US"), DateTimeStyles.None, out outDateTimeResult2);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)notbetweenObject.range)[entity.AttributeRealName]))["gte"] = outDateTimeResult.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)notbetweenObject.range)[entity.AttributeRealName]))["lte"] = outDateTimeResult2.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");
                            break;
                        case ESValueDataType.NUMERIC:
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)notbetweenObject.range)[entity.AttributeRealName]))["gte"] = Convert.ToDouble(entity.Value);
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)notbetweenObject.range)[entity.AttributeRealName]))["lte"] = Convert.ToDouble(entity.Value2);
                            break;
                    }
                    ((IDictionary<String, Object>)qsObject)["bool"] = new ExpandoObject();
                    ((IDictionary<String, Object>)(((IDictionary<String, Object>)qsObject)["bool"]))["must_not"] = notbetweenObject;
                    break;
            }

            return qsObject;
        }

        private static dynamic QueryBuilder(ESQueryRequestInfo requestInfo, out HashSet<string> SecIdsPushedToDownstream)
        {
            List<string> indices = null;
            StringBuilder sqlQuery = new StringBuilder();
            IDictionary<string, Object> queryObject = (new ExpandoObject() as IDictionary<string, Object>);
            dynamic boolObject = new ExpandoObject();
            List<dynamic> lstMust = new List<dynamic>();
            SecIdsPushedToDownstream = new HashSet<string>();

            RDBConnectionManager dbConn = CommonDALWrapper.GetConnectionManager(ConnectionConstants.SecMaster_Connection, true, IsolationLevel.Serializable);

            try
            {
                if (requestInfo.SecurityTypeIds != null && requestInfo.SecurityTypeIds.Count > 0)
                {
                    sqlQuery.Append("DECLARE @temp TABLE(sectype_id INT); INSERT INTO @temp VALUES (").Append(string.Join("),(", requestInfo.SecurityTypeIds)).Append("); ").Append("SELECT * FROM IVPSecMaster.dbo.ivp_secm_elastic_search_mapping sm").Append(" INNER JOIN @temp tm ON sm.sectype_id = tm.sectype_id;");
                    indices = CommonDALWrapper.ExecuteSelectQuery(sqlQuery.ToString(), dbConn).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["index_name"])).ToList();

                    dynamic indicesObject = new ExpandoObject();
                    indicesObject.terms = new ExpandoObject();
                    indicesObject.terms._index = indices;
                    lstMust.Add(indicesObject);
                }

                else if (!string.IsNullOrEmpty(requestInfo.indexName))
                {
                    dynamic indicesObject = new ExpandoObject();
                    indicesObject.terms = new ExpandoObject();
                    indicesObject.terms._index = new List<string>() { requestInfo.indexName };
                    lstMust.Add(indicesObject);
                }

                switch (requestInfo.QueryType)
                {
                    case ESQueryTypeEnum.BrowseSecurities:
                        dynamic queryBrowseText = new ExpandoObject();
                        queryBrowseText.query_string = new ExpandoObject();
                        queryBrowseText.query_string.query = "*";
                        lstMust.Add(queryBrowseText);
                        break;

                    case ESQueryTypeEnum.Search:
                        dynamic boolSearch = new ExpandoObject();

                        if (requestInfo.Query[0] is ESQueryEntity)
                        {
                            ESQueryEntity searchEntity = requestInfo.Query[0] as ESQueryEntity;

                            dynamic queryKeywordObject = new ExpandoObject();
                            queryKeywordObject.query_string = new ExpandoObject();

                            if (searchEntity.DataType.Equals(ESValueDataType.STRING))
                            {
                                queryKeywordObject.query_string.fields = new List<string> { searchEntity.AttributeRealName + ".keyword" };

                                if (searchEntity.MatchType.Equals(ESAdvancedQueryMatchType.EQUALS))
                                    queryKeywordObject.query_string.query = searchEntity.Value.ToLower().Replace(" ", "\\ ");
                                else
                                    queryKeywordObject.query_string.query = searchEntity.Value.ToLower().Replace(" ", "\\ ") + "*";
                            }
                            else
                            {
                                queryKeywordObject.query_string.fields = new List<string> { searchEntity.AttributeRealName };
                                queryKeywordObject.query_string.query = searchEntity.Value.ToLower().Replace(" ", "\\ ");
                            }

                            boolSearch.should = new List<dynamic> { queryKeywordObject };

                            lstMust.Add(boolSearch);
                        }
                        break;

                    case ESQueryTypeEnum.FreeTextSearch:
                        dynamic boolFreeSearch = new ExpandoObject();

                        if (requestInfo.Query[0] is ESQueryEntity)
                        {
                            List<dynamic> lstSho = new List<dynamic>();
                            ESQueryEntity searchEntity = requestInfo.Query[0] as ESQueryEntity;
                            string entityValue = searchEntity.Value.ToLower().Replace(" ", "\\ ");

                            dynamic queryKeywordObject = new ExpandoObject();
                            queryKeywordObject.query_string = new ExpandoObject();

                            if (string.IsNullOrEmpty(searchEntity.AttributeRealName))
                            {
                                if (freeTextAttributes.Count == 0)
                                {
                                    queryKeywordObject.query_string.fields = new List<string> { "*.keyword" };
                                    queryKeywordObject.query_string.query = entityValue + "*";
                                    lstSho.Add(queryKeywordObject);
                                }
                                else
                                {
                                    queryKeywordObject.query_string.fields = freeTextAttributes;
                                    queryKeywordObject.query_string.query = "*" + entityValue + "*";
                                    lstSho.Add(queryKeywordObject);

                                    dynamic queryKeywordObject2 = new ExpandoObject();
                                    queryKeywordObject2.query_string = new ExpandoObject();
                                    queryKeywordObject2.query_string.fields = new List<string> { "*.keyword" };
                                    queryKeywordObject2.query_string.query = entityValue + "*";
                                    lstSho.Add(queryKeywordObject2);
                                }
                            }
                            else
                            {
                                if (searchEntity.DataType.Equals(ESValueDataType.STRING))
                                {
                                    queryKeywordObject.query_string.fields = new List<string> { searchEntity.AttributeRealName + ".keyword" };

                                    if (searchEntity.MatchType.Equals(ESAdvancedQueryMatchType.EQUALS))
                                        queryKeywordObject.query_string.query = entityValue;
                                    else
                                        queryKeywordObject.query_string.query = entityValue + "*";
                                }
                                else
                                {
                                    queryKeywordObject.query_string.fields = new List<string> { searchEntity.AttributeRealName };
                                    queryKeywordObject.query_string.query = entityValue;
                                }
                                lstSho.Add(queryKeywordObject);
                            }

                            dynamic sho = new ExpandoObject();
                            var shodic = (IDictionary<String, Object>)sho;
                            dynamic temp = new ExpandoObject();
                            temp.should = lstSho;
                            shodic["bool"] = temp;
                            lstMust.Add(sho);
                        }
                        break;

                    case ESQueryTypeEnum.SearchIntellisense:
                        dynamic queryIntelObject = new ExpandoObject();
                        queryIntelObject.query_string = new ExpandoObject();
                        List<string> intelFields = new List<string>();
                        string intelValue = string.Empty;

                        for (int i = 0; i < requestInfo.Query.Count; i++)
                        {
                            ESQueryExecution item = requestInfo.Query[i];
                            if (item is ESQueryEntity)
                            {
                                ESQueryEntity intelEntity = item as ESQueryEntity;

                                if (intelEntity.DataType.Equals(ESValueDataType.STRING))
                                    intelFields.Add(intelEntity.AttributeRealName + ".keyword");
                                else
                                    intelFields.Add(intelEntity.AttributeRealName);

                                if (i == 0)
                                    intelValue = intelEntity.Value.ToLower().Replace(" ", "\\ ");
                            }
                        }
                        queryIntelObject.query_string.fields = intelFields;
                        queryIntelObject.query_string.query = intelValue + "*";
                        lstMust.Add(queryIntelObject);
                        break;

                    case ESQueryTypeEnum.AdvancedSearch:

                        List<List<ESQueryExecution>> lstOr = new List<List<ESQueryExecution>>();

                        if (requestInfo.Query.Count > 0)
                        {
                            List<ESQueryExecution> temp = new List<ESQueryExecution>();

                            foreach (ESQueryExecution item in requestInfo.Query)
                            {
                                if (item is ESQueryOperator && (item as ESQueryOperator).Operator.Equals(ESQueryOperatorTypeEnum.OR))
                                {
                                    if (temp.Count > 0)
                                    {
                                        lstOr.Add(temp);
                                        temp = new List<ESQueryExecution>();
                                    }
                                }
                                else
                                    temp.Add(item);
                            }
                            if (temp.Count > 0)
                                lstOr.Add(temp);
                        }

                        if (lstOr.Count > 0)
                        {
                            dynamic bObject = new ExpandoObject();
                            List<dynamic> orList = new List<dynamic>();

                            foreach (var item in lstOr)
                            {
                                if (item.Count > 0)
                                {
                                    if (item.Count > 1)
                                    {
                                        dynamic bandObject = new ExpandoObject();
                                        List<dynamic> andList = new List<dynamic>();

                                        foreach (var subitem in item)
                                        {
                                            if (subitem is ESQueryEntity)
                                                andList.Add(GetAdvancedQuery(subitem as ESQueryEntity));
                                        }
                                        ((IDictionary<String, Object>)bandObject)["bool"] = new ExpandoObject();
                                        ((IDictionary<String, Object>)(((IDictionary<String, Object>)bandObject)["bool"]))["must"] = andList;
                                        orList.Add(bandObject);
                                    }
                                    else
                                    {
                                        if (item[0] is ESQueryEntity)
                                            orList.Add(GetAdvancedQuery(item[0] as ESQueryEntity));
                                    }
                                }
                            }

                            ((IDictionary<String, Object>)bObject)["bool"] = new ExpandoObject();
                            ((IDictionary<String, Object>)(((IDictionary<String, Object>)bObject)["bool"]))["should"] = orList;
                            lstMust.Add(bObject);
                        }
                        break;
                }

                foreach (var filter in requestInfo.Filters)
                {
                    if (filter.FilterStartDate != null || filter.FilterEndDate != null)
                    {
                        dynamic rangeObject = new ExpandoObject();
                        rangeObject.range = new ExpandoObject();

                        switch (filter.FilterType)
                        {
                            case ESQueryFilterType.Created:
                                rangeObject.range.created_on = new ExpandoObject();

                                if (filter.FilterStartDate != null)
                                    rangeObject.range.created_on.gte = filter.FilterStartDate.Value.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");

                                if (filter.FilterEndDate != null)
                                    rangeObject.range.created_on.lte = filter.FilterEndDate.Value.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");

                                lstMust.Add(rangeObject);
                                break;

                            case ESQueryFilterType.Modified:
                                rangeObject.range.last_modified_on = new ExpandoObject();

                                if (filter.FilterStartDate != null)
                                    rangeObject.range.last_modified_on.gte = filter.FilterStartDate.Value.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");

                                if (filter.FilterEndDate != null)
                                    rangeObject.range.last_modified_on.lte = filter.FilterEndDate.Value.ToString(@"yyyy-MM-dd hh\:mm\:ss\.fff");

                                lstMust.Add(rangeObject);
                                break;
                        }
                    }
                }

                boolObject.must = lstMust;
                queryObject["bool"] = boolObject;

                dbConn.CommitTransaction();
            }
            catch (Exception ee)
            {
                dbConn.RollbackTransaction();
                mLogger.Error(ee.ToString());
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(dbConn);
            }
            return queryObject;
        }

        public static HashSet<string> GetPostToDowstreamSecurities(ESQueryFilter filter)
        {
            try
            {
                StringBuilder sqlQuery = new StringBuilder("SELECT * FROM IVPSecMaster.dbo.ivp_secm_downstream_system_status WHERE ");

                if (filter.FilterStartDate != null && filter.FilterEndDate == null)
                    sqlQuery.Append("last_modified_on >= '").Append(filter.FilterStartDate.Value).Append("'");

                else if (filter.FilterEndDate != null && filter.FilterStartDate == null)
                    sqlQuery.Append("last_modified_on <= '").Append(filter.FilterEndDate.Value).Append("'");

                else
                    sqlQuery.Append("last_modified_on BETWEEN '").Append(filter.FilterStartDate.Value).Append("' AND '").Append(filter.FilterEndDate.Value).Append("';");

                return new HashSet<string>(CommonDALWrapper.ExecuteSelectQuery(sqlQuery.ToString(), ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().Select(x => Convert.ToString(x["sec_id"])));
            }
            catch (Exception ex)
            {
                mLogger.Error("GetPostToDowstreamSecurities -> " + ex.ToString());
                return new HashSet<string>();
            }
        }
    }


    public static class SMESSyncController
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("SMESSyncController");
        private static readonly int batchSize;

        private static bool isReconciling = false;
        private static Queue<string> secIdQueue = new Queue<string>();
        private static HashSet<string> secIdHash = new HashSet<string>();

        private static object lockObj = new object();
        private static object syncProcessLock = new object();
        private static object reconcileLock = new object();

        static SMESSyncController()
        {
            string value = ConfigurationManager.AppSettings["ElasticSearchSyncBatchSize"];
            if (string.IsNullOrEmpty(value))
                batchSize = 10000;
            else
                batchSize = Convert.ToInt32(value);
        }

        public static void syncTimer_Elapsed(Object sender, ElapsedEventArgs args)
        {
            bool flag = false;
            lock (reconcileLock)
            {
                if (!isReconciling)
                    flag = true;
            }
            if (flag)
                SyncData();
        }

        private static void DumpData()
        {
            DateTime now = DateTime.Now;
            mLogger.Error("DumpData ---------> START");
            string output;

            try
            {
                List<string> sectypes = SMElasticSearchSQLAPI.getSectypes();

                foreach (string sectype in sectypes)
                {
                    Dictionary<int, Dictionary<string, string>> tableVscolumnVstype = SMElasticSearchSQLAPI.fetchTableSchema(sectype);
                    ElasticAPI.CreateIndex(sectype.Replace(" ", "_").ToLower(), tableVscolumnVstype, out output);

                    List<string> secIds = SMElasticSearchSQLAPI.fetchSecIds(sectype, null);
                    if (secIds.Count > batchSize)
                    {
                        int start = 0, batchSizeTemp = batchSize;
                        while (true)
                        {
                            bool flag = false;

                            if ((start + batchSize) > secIds.Count)
                            {
                                batchSizeTemp = secIds.Count - start;
                                flag = true;
                            }

                            TriggerExecutor(sectype, secIds.GetRange(start, batchSizeTemp));

                            start += batchSize;
                            if (flag)
                                break;
                        }
                    }
                    else
                        TriggerExecutor(sectype, secIds);

                    mLogger.Error("DumpData ---------> Inserted for : " + sectype);
                }
                CommonDALWrapper.ExecuteQuery("TRUNCATE TABLE IVPSecMaster.dbo.ivp_secm_elastic_search_sync_timestamp; INSERT INTO IVPSecMaster.dbo.ivp_secm_elastic_search_sync_timestamp VALUES('" + now + "');", CommonQueryType.Insert, ConnectionConstants.SecMaster_Connection);
            }
            catch (Exception ee)
            {
                mLogger.Error("DumpData ---------> ERROR : " + ee.ToString());
            }

            mLogger.Error("DumpData ---------> END");
        }

        public static void ReconcileAndSyncData()
        {
            try
            {
                lock (reconcileLock)
                {
                    isReconciling = true;
                }
                mLogger.Error("ReconcileAndSyncData ---------> START");

                DateTime now = DateTime.Now;
                string output;
                DateTime? lastTime = null;
                DataTable dt = CommonDALWrapper.ExecuteSelectQuery("SELECT * FROM IVPSecMaster.dbo.ivp_secm_elastic_search_sync_timestamp", ConnectionConstants.SecMaster_Connection).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    string lt = Convert.ToString(dt.Rows[0]["timestamp"]);
                    if (!string.IsNullOrEmpty(lt))
                    {
                        lastTime = Convert.ToDateTime(lt);

                        lock (syncProcessLock)
                        {
                            List<string> sectypes = SMElasticSearchSQLAPI.getSectypes();

                            foreach (string sectype in sectypes)
                            {
                                Dictionary<int, Dictionary<string, string>> tableVscolumnVstype = SMElasticSearchSQLAPI.fetchTableSchema(sectype);
                                ElasticAPI.CreateIndex(sectype.Replace(" ", "_").ToLower(), tableVscolumnVstype, out output);

                                List<string> secIds = SMElasticSearchSQLAPI.fetchSecIds(sectype, lastTime);
                                if (secIds.Count > batchSize)
                                {
                                    int start = 0, batchSizeTemp = batchSize;
                                    while (true)
                                    {
                                        bool flag = false;

                                        if ((start + batchSize) > secIds.Count)
                                        {
                                            batchSizeTemp = secIds.Count - start;
                                            flag = true;
                                        }

                                        TriggerExecutor(sectype, secIds.GetRange(start, batchSizeTemp));

                                        start += batchSize;
                                        if (flag)
                                            break;
                                    }
                                }
                                else
                                    TriggerExecutor(sectype, secIds);

                                mLogger.Error("ReconcileAndSyncData ---------> Inserted for : " + sectype);
                            }
                            CommonDALWrapper.ExecuteQuery("TRUNCATE TABLE IVPSecMaster.dbo.ivp_secm_elastic_search_sync_timestamp; INSERT INTO IVPSecMaster.dbo.ivp_secm_elastic_search_sync_timestamp VALUES('" + now + "');", CommonQueryType.Insert, ConnectionConstants.SecMaster_Connection);
                        }
                    }
                }
                lock (reconcileLock)
                {
                    isReconciling = false;
                }
            }
            catch (Exception ee)
            {
                mLogger.Error("ReconcileAndSyncData ---------> ERROR : " + ee.ToString());
            }

            mLogger.Error("ReconcileAndSyncData ---------> END");
        }

        private static void TriggerExecutor(string sectype, List<string> secIds)
        {
            string secIdString = string.Join(",", secIds);
            mLogger.Error("Trigger Process -------> starting process for sectype: " + sectype + " for SecIds : " + secIdString);

            string guid = Guid.NewGuid().ToString();
            DataTable dt = new DataTable();
            dt.Columns.Add("job_id", typeof(string));
            dt.Columns.Add("sec_id", typeof(string));

            foreach (string secId in secIds)
            {
                DataRow dr = dt.NewRow();
                dr["job_id"] = guid;
                dr["sec_id"] = secId;
                dt.Rows.Add(dr);
            }

            CommonDALWrapper.ExecuteBulkUpload("IVPSecMaster.dbo.ivp_secm_elastic_search_sync_process_info", dt, ConnectionConstants.SecMaster_Connection);

            ProcessStartInfo processInfo = new ProcessStartInfo { FileName = "SMElasticSearchSyncProcess.exe", Arguments = sectype.Replace(" ", "ž") + " " + guid, CreateNoWindow = false, UseShellExecute = false, WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory, RedirectStandardError = true, RedirectStandardInput = true, RedirectStandardOutput = true };
            Process pipeClient = Process.Start(processInfo);
            pipeClient.WaitForExit();

            string s = pipeClient.StandardOutput.ReadToEnd();
            if (pipeClient.ExitCode == 1)
                throw new Exception(s);
        }

        private static void SyncData()
        {
            DateTime now = DateTime.Now;
            mLogger.Error("SyncData --------> START");
            try
            {
                SMElasticSearchSQLAPI.populateElasticSearchMappingTable();

                Dictionary<string, List<string>> secTypeVsSecIds = new Dictionary<string, List<string>>();
                Dictionary<string, KeyValuePair<string, int>> shortNameVsSectype = CommonDALWrapper.ExecuteSelectQuery("SELECT sectype_short_name, sectype_name, sectype_id FROM IVPSecMaster.dbo.ivp_secm_sectype_master WHERE is_active = 1 AND is_complete = 1;", ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().ToDictionary(x => Convert.ToString(x["sectype_short_name"]), y => new KeyValuePair<string, int>(Convert.ToString(y["sectype_name"]), Convert.ToInt32(y["sectype_id"])));

                lock (lockObj)
                {
                    if (secIdHash.Count > batchSize)
                    {
                        int counter = 0;
                        List<string> temp = new List<string>();

                        while (counter < batchSize)
                        {
                            while (true)
                            {
                                var item = secIdQueue.Dequeue();
                                if (secIdHash.Contains(item))
                                {
                                    temp.Add(item);
                                    secIdHash.Remove(item);
                                    break;
                                }
                            }
                            counter++;
                        }

                        if (temp.Count > 0)
                            secTypeVsSecIds = temp.GroupBy(x => x.Substring(0, 4)).ToDictionary(y => shortNameVsSectype[y.Key].Key, z => z.ToList());
                    }
                    else
                    {
                        if (secIdHash.Count > 0)
                        {
                            secTypeVsSecIds = secIdHash.GroupBy(x => x.Substring(0, 4)).ToDictionary(y => shortNameVsSectype[y.Key].Key, z => z.ToList());
                            secIdQueue.Clear();
                            secIdHash.Clear();
                        }
                    }
                }

                lock (syncProcessLock)
                {
                    if (secTypeVsSecIds.Count > 0)
                    {
                        mLogger.Error("SyncData --------> Syncing Insert/Update --------> START");
                        foreach (var kvp in secTypeVsSecIds)
                        {
                            TriggerExecutor(kvp.Key, kvp.Value);
                        }
                        mLogger.Error("SyncData --------> Syncing Insert/Update --------> END");
                    }
                }
                CommonDALWrapper.ExecuteQuery("TRUNCATE TABLE IVPSecMaster.dbo.ivp_secm_elastic_search_sync_timestamp; INSERT INTO IVPSecMaster.dbo.ivp_secm_elastic_search_sync_timestamp VALUES('" + now + "');", CommonQueryType.Insert, ConnectionConstants.SecMaster_Connection);
            }
            catch (Exception ee)
            {
                mLogger.Error("SyncData --------> ERROR : " + ee.ToString());
            }
            mLogger.Error("SyncData --------> END");
        }

        public static void SeedData()
        {
            mLogger.Error("SeedData --------> START");
            try
            {
                lock (syncProcessLock)
                {
                    Parallel.Invoke(() =>
                    {
                        string output;
                        bool ret = ElasticAPI.DeleteIndex(string.Empty, out output);
                    }, () =>
                    {
                        SMElasticSearchSQLAPI.populateElasticSearchMappingTable();
                    });

                    DumpData();
                }
            }
            catch (Exception ee)
            {
                mLogger.Error("SeedData --------> ERROR : " + ee.ToString());
            }
            mLogger.Error("SeedData --------> END");
        }

        public static void RegisterSyncing(List<string> secIds)
        {
            mLogger.Error("RegisterSyncing --------> START");
            try
            {
                lock (lockObj)
                {
                    var secIdsToQueue = secIds.Except(secIdHash);
                    foreach (var secid in secIdsToQueue)
                    {
                        secIdQueue.Enqueue(secid);
                        secIdHash.Add(secid);
                    }
                }
            }
            catch (Exception ee)
            {
                mLogger.Error("RegisterSyncing --------> ERROR : " + ee.ToString());
            }
            mLogger.Error("RegisterSyncing --------> END");
        }

        public static void RegisterDeletion(List<string> secIds)
        {
            mLogger.Error("RegisterDeletion --------> START");
            try
            {
                var hsh = new HashSet<string>(secIds);
                Dictionary<string, KeyValuePair<string, int>> shortNameVsSectype = CommonDALWrapper.ExecuteSelectQuery("SELECT sectype_short_name, sectype_name, sectype_id FROM IVPSecMaster.dbo.ivp_secm_sectype_master WHERE is_active = 1 AND is_complete = 1;", ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().ToDictionary(x => Convert.ToString(x["sectype_short_name"]), y => new KeyValuePair<string, int>(Convert.ToString(y["sectype_name"]), Convert.ToInt32(y["sectype_id"])));
                lock (lockObj)
                {
                    secIdHash.RemoveWhere(x => hsh.Contains(x));

                    var delSecTypeVsSecIds = secIds.GroupBy(x => x.Substring(0, 4)).ToDictionary(y => shortNameVsSectype[y.Key], z => z.ToList());

                    foreach (var kvo in delSecTypeVsSecIds)
                    {
                        string output;
                        ElasticAPI.BulkDelete(kvo.Key.Key.Replace(" ", "_").ToLower(), kvo.Key.Value, "sec_id", kvo.Value, out output);

                        dynamic res = JsonConvert.DeserializeObject<dynamic>(output);

                        if (res != null && (((JContainer)(res.ResultObject)))["errors"] != null && ((bool)((JValue)res.errors).Value))
                            throw new Exception(output);

                        mLogger.Error("Syncing Delete --------> Response : " + output);
                    }
                }
            }
            catch (Exception ee)
            {
                mLogger.Error("RegisterDeletion --------> ERROR : " + ee.ToString());
            }
            mLogger.Error("RegisterDeletion --------> END");
        }
    }

    [Serializable]
    public class ESQueryRequestInfo
    {
        public List<int> SecurityTypeIds { get; set; }
        public List<string> RequiredAttributeRealNames { get; set; }
        public ESQueryTypeEnum QueryType { get; set; }
        public List<ESQueryExecution> Query { get; set; }
        public List<ESQueryFilter> Filters { set; get; }
        public bool RequireDataTable { get; set; }
        public int From { get; set; }
        public int Size { get; set; }
        public string indexName { get; set; }
    }

    public class ESQueryResposeInfo
    {
        public bool IsSuccess { get; set; }
        public DataTable ResultTable { get; set; }
        public dynamic ResultObject { get; set; }
        public string FailureMessage { get; set; }
        public HashSet<string> SecIdsPushedToDownstream { get; set; }
    }

    [Serializable]
    public abstract class ESQueryExecution
    {
    }

    [Serializable]
    public class ESQueryEntity : ESQueryExecution
    {
        public string AttributeRealName { get; set; }
        public ESAdvancedQueryMatchType MatchType { get; set; }
        public string Value { get; set; }
        public string Value2 { get; set; }
        public ESValueDataType DataType { get; set; }
    }

    [Serializable]
    public class ESQueryOperator : ESQueryExecution
    {
        public ESQueryOperatorTypeEnum Operator { get; set; }
    }

    [Serializable]
    public class ESQueryFilter
    {
        public ESQueryFilterType FilterType { get; set; }
        public DateTime? FilterStartDate { get; set; }
        public DateTime? FilterEndDate { get; set; }
    }

    [Serializable]
    public enum ESAdvancedQueryMatchType
    {
        EQUALS = 0,
        DOES_NOT_EQUAL,
        BEFORE,
        AFTER,
        BETWEEN,
        OUTSIDE,
        BEGINS_WITH,
        ENDS_WITH,
        CONTAIN,
        DOES_NOT_CONTAIN,
        GREATER_THAN,
        LESS_THAN,
        IN,
        GREATER_OR_EQUAL,
        LESS_OR_EQUAL
    }

    [Serializable]
    public enum ESQueryTypeEnum
    {
        BrowseSecurities = 0,
        SearchIntellisense,
        Search,
        FreeTextSearch,
        AdvancedSearch
    }

    [Serializable]
    public enum ESQueryOperatorTypeEnum
    {
        OR = 0,
        AND
    }

    [Serializable]
    public enum ESQueryFilterType
    {
        None = 0,
        Created,
        Modified,
        NotPushedToDownstream
    }

    [Serializable]
    public enum ESValueDataType
    {
        STRING = 0,
        NUMERIC,
        DATE,
        TIME,
        DATETIME,
        BOOLEAN
    }
}
