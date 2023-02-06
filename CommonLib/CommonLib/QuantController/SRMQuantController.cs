using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.utils;
using System.Runtime.Serialization;
using com.ivp.rad.dal;
using Newtonsoft.Json;

namespace com.ivp.common
{
    public static class SRMQuantController
    {
        #region private member variables
        static IRLogger mLogger = RLogFactory.CreateLogger("SRMQuantController");
        public static List<SRMQuantAggregationInfo> lstAggregationInfo = new List<SRMQuantAggregationInfo>();
        #endregion

        /// <summary>
        /// GetQuantIntellisenseData - Used for building query on UI
        /// </summary>
        /// <returns>List of SRMQuantFilterInfo</returns>
        public static List<SRMQuantFilterInfo> GetQuantIntellisenseData()
        {
            mLogger.Debug("SRMQuantController : Start -> GeQuantData");
            RHashlist mHList = new RHashlist();
            //Dictionary<string, SMQuantFilterInfo> dictFilterInfo = new Dictionary<string, SMQuantFilterInfo>();
            List<SRMQuantFilterInfo> lstQuantData = new List<SRMQuantFilterInfo>();
            try
            {
                DataSet quantDS = CommonDALWrapper.ExecuteSelectQuery(@"SELECT ke.keyword_id,COALESCE(keysyn.synonym, ke.keyword_name) AS keyword_name,ke.help_text, COALESCE(syn.synonym, word.keyword_name) as 'child_keyword_name', 
	                                                                    COALESCE(syn.keyword_id, word.keyword_id) as 'child_keyword_id', ke.keyword_level,
		                                                                    CASE WHEN (keysyn.synonym <> ke.keyword_name) THEN 1 ELSE 0 END AS 'is_hidden'
		                                                                    ,syn.synonym as 'synonyms', kt.keyword_type, ISNULL(ke.index_name,'') as index_name
                                                                            , ISNULL(ke.is_reference_attribute,0) as is_reference_attribute
                                                                            , ISNULL(ke.parent_attribute_name,'') as parent_attribute_name, ISNULL(ke.child_attribute_name,'') as child_attribute_name
                                                                            ,ISNULL(GRID.index_name,'') as parent_index
                                                                            ,ISNULL(GRID.data_type,'') as data_type
                                                                            ,ISNULL(ODM.data_type,'') as operator_type
	                                                                    FROM IVPSecMaster.dbo.ivp_secm_quant_keyword_type kt
	                                                                    INNER JOIN IVPSecMaster.dbo.ivp_secm_quant_keywords ke
	                                                                    ON (kt.keyword_type_id = ke.keyword_type_id)
                                                                        LEFT JOIN ivp_secm_quant_search_grid_info GRID
	                                                                    ON ke.keyword_name = GRID.attribute_name 
                                                                        LEFT JOIN ivp_secm_quant_operator_datatype_mapping ODM
	                                                                    ON ke.keyword_id = ODM.keyword_id
                                                                        LEFT JOIN IVPSecMaster.dbo.ivp_secm_quant_synonyms keysyn
	                                                                    ON (keysyn.keyword_id = ke.keyword_id)
	                                                                    LEFT JOIN IVPSecMaster.dbo.ivp_secm_quant_keyword_suggestion_mapping swm
	                                                                    ON (swm.keyword_id = ke.keyword_id)
	                                                                    LEFT JOIN IVPSecMaster.dbo.ivp_secm_quant_suggestion_sets ss
	                                                                    ON (ss.suggestion_set_id = swm.suggestion_set)
	                                                                    LEFT JOIN IVPSecMaster.dbo.ivp_secm_quant_suggestion_set_keyword_mapping km
	                                                                    ON (km.suggestion_set_id = ss.suggestion_set_id)
	                                                                    LEFT JOIN IVPSecMaster.dbo.ivp_secm_quant_keywords word
	                                                                    ON (word.keyword_id = km.keyword_id)
	                                                                    LEFT JOIN IVPSecMaster.dbo.ivp_secm_quant_synonyms syn
	                                                                    ON (syn.keyword_id = word.keyword_id)", ConnectionConstants.SecMaster_Connection);

                if (quantDS != null && quantDS.Tables.Count > 0 && quantDS.Tables[0] != null)
                {
                    lstQuantData = quantDS.Tables[0].AsEnumerable().Select(x => new { FilterID = x["keyword_id"].ToString(), FilterName = x["keyword_name"].ToString(), Hidden = Convert.ToBoolean(x["is_hidden"]), HelpText = Convert.ToString(x["help_text"]), KeyWordLevel = Convert.ToString(x["keyword_level"]), KeywordType = Convert.ToString(x["keyword_type"]), IndexName = Convert.ToString(x["index_name"]), ParentAttribute = Convert.ToString(x["parent_attribute_name"]), ChildAttribute = Convert.ToString(x["child_attribute_name"]), ParentIndex = Convert.ToString(x["parent_index"]), DataType = Convert.ToString(x["data_type"]), OperatorType = Convert.ToString(x["operator_type"]), IsReferenceAttribute = Convert.ToBoolean(x["is_reference_attribute"]) }).Distinct().Select(x => new SRMQuantFilterInfo() { FilterID = x.FilterID, FilterName = x.FilterName, Hidden = x.Hidden, IndexName = x.IndexName, ParentAttribute = x.ParentAttribute, ChildAttribute = x.ChildAttribute, ParentIndex = x.ParentIndex, DataType = x.DataType, OperatorType = x.OperatorType, IsReferenceAttribute = x.IsReferenceAttribute, Filters = new List<SRMQuantFilterInfo>(), Parameters = new Dictionary<string, List<SRMQuantFilterInfo>>(), KeyWordLevel = x.KeyWordLevel, HelpText = x.HelpText, KeywordType = x.KeywordType }).ToList();
                    quantDS.Tables[0].AsEnumerable().ToList().ForEach(x =>
                    {
                        if (lstQuantData.Find(y => y.FilterID == x["child_keyword_id"].ToString() && y.FilterName == x["child_keyword_name"].ToString()) != null)
                        {
                            lstQuantData.Where(y => y.FilterID == x["keyword_id"].ToString()).ToList().ForEach(z =>
                            {
                                if (z.Filters.Find(y => y.FilterID == x["child_keyword_id"].ToString() && y.FilterName == x["child_keyword_name"].ToString()) == null)
                                {
                                    z.Filters.Add(new SRMQuantFilterInfo() { FilterID = lstQuantData.Find(y => y.FilterID == x["child_keyword_id"].ToString() && y.FilterName == x["child_keyword_name"].ToString()).FilterID });
                                }
                            });
                        }
                    });
                }

                DataSet functionDS = CommonDALWrapper.ExecuteSelectQuery(@"SELECT 	
	                                                                        par.parameter_id,
	                                                                        par.parameter_name,
	                                                                        ke.keyword_id AS 'function_keyword_id',
	                                                                        ke.keyword_name AS 'function_keyword_name',
	                                                                        childKey.keyword_id AS 'child_keyword_id', 	
	                                                                        CASE WHEN syn.synonym IS NOT NULL THEN syn.synonym  ELSE childKey.keyword_name END AS 'child_keyword_name'
	                                                                        FROM 
	                                                                        IVPSecMaster.dbo.ivp_secm_quant_function_parameters par
	                                                                        INNER JOIN IVPSecMaster.dbo.ivp_secm_quant_keyword_parameter_mapping kpm
	                                                                        ON (par.parameter_id = kpm.parameter_id)
	                                                                        LEFT JOIN IVPSecMaster.dbo.ivp_secm_quant_parameter_suggestion_set_mapping ssm
	                                                                        ON (kpm.parameter_id = ssm.parameter_id)
	                                                                        LEFT JOIN IVPSecMaster.dbo.ivp_secm_quant_suggestion_set_keyword_mapping ssetkm
	                                                                        ON (ssetkm.suggestion_set_id = ssm.suggestion_set_id)
	                                                                        LEFT JOIN IVPSecMaster.dbo.ivp_secm_quant_keywords childKey
	                                                                        ON (childKey.keyword_id = ssetkm.keyword_id)
	                                                                        LEFT JOIN IVPSecMaster.dbo.ivp_secm_quant_synonyms syn
	                                                                        ON (syn.keyword_id = childKey.keyword_id)	
	                                                                        LEFT JOIN IVPSecMaster.dbo.ivp_secm_quant_keywords ke
	                                                                        ON (ke.keyword_id = kpm.keyword_id)", ConnectionConstants.SecMaster_Connection);

                if (functionDS != null && functionDS.Tables.Count > 0 && functionDS.Tables[0] != null)
                {
                    functionDS.Tables[0].AsEnumerable().ToList().ForEach(x =>
                    {
                        lstQuantData.Where(y => y.FilterID == x["function_keyword_id"].ToString()).ToList().ForEach(z =>
                        {
                            if (z.Parameters.ContainsKey(Convert.ToString(x["parameter_name"])) && !string.IsNullOrEmpty(Convert.ToString(x["child_keyword_id"])))
                            {
                                z.Parameters[Convert.ToString(x["parameter_name"])].Add(new SRMQuantFilterInfo() { FilterID = Convert.ToString(x["child_keyword_id"]), FilterName = Convert.ToString(x["child_keyword_name"]) });
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(Convert.ToString(x["child_keyword_id"])))
                                    z.Parameters.Add(Convert.ToString(x["parameter_name"]), new List<SRMQuantFilterInfo>() { });
                                else
                                    z.Parameters.Add(Convert.ToString(x["parameter_name"]), new List<SRMQuantFilterInfo>() { new SRMQuantFilterInfo() { FilterID = Convert.ToString(x["child_keyword_id"]), FilterName = Convert.ToString(x["child_keyword_name"]) } });
                            }
                        });
                    });
                }

                return lstQuantData;
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return null;
            }
            finally
            {
                if (mHList != null)
                {
                    mHList.Clear();
                    mHList = null;
                }
                mLogger.Debug("SRMQuantController : End -> GeQuantData");
            }
        }

        /// <summary>
        /// GetQuantSavedSearches - Fetches saved search
        /// </summary>
        /// <param name="userName">Name of the user allowed to view saved searches</param>
        /// <returns>List of SRMQuantSavedSearches</returns>
        public static List<SRMQuantSavedSearches> GetQuantSavedSearches(string userName)
        {
            mLogger.Debug("SRMQuantController : Start -> GetQuantSavedSearches");
            RHashlist mHList = new RHashlist();
            List<SRMQuantSavedSearches> lstQuantSavedSearches = new List<SRMQuantSavedSearches>();
            try
            {
                DataSet savedSearchesDS = CommonDALWrapper.ExecuteSelectQuery(@"  SELECT search_id,search_name,
		                                                        search_query,search_encoded_query
		                                                        FROM IVPSecMaster.dbo.ivp_secm_quant_saved_searches WHERE created_by = '" + userName + "' AND is_active = 1", ConnectionConstants.SecMaster_Connection);

                if (savedSearchesDS != null && savedSearchesDS.Tables.Count > 0 && savedSearchesDS.Tables[0] != null)
                {
                    foreach (DataRow dataRow in savedSearchesDS.Tables[0].Rows)
                    {
                        SRMQuantSavedSearches srmQuantSavedSearches = new SRMQuantSavedSearches();
                        srmQuantSavedSearches.SearchId = Convert.ToString(dataRow["search_id"]);
                        srmQuantSavedSearches.SearchName = Convert.ToString(dataRow["search_name"]);
                        srmQuantSavedSearches.SearchQuery = Convert.ToString(dataRow["search_query"]);
                        srmQuantSavedSearches.SearchEncodedQuery = Convert.ToString(dataRow["search_encoded_query"]);

                        lstQuantSavedSearches.Add(srmQuantSavedSearches);
                    }
                }
                return lstQuantSavedSearches;
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return lstQuantSavedSearches;
            }
            finally
            {
                if (mHList != null)
                {
                    mHList.Clear();
                    mHList = null;
                }
                mLogger.Debug("SRMQuantController : End -> GetQuantSavedSearches");
            }
        }

        /// <summary>
        /// InsertUpdateQuantSavedSearch - Inserts or Updates the saved search
        /// </summary>
        /// <param name="searchID">Populated only in case of update saved search</param>
        /// <param name="userName">Name of the user inserting/updating saved search</param>
        /// <param name="searchName">Name of the saved search</param>
        /// <param name="searchQuery">Search Query in readable format</param>
        /// <param name="searchEncodedQuery">Search Query in encoded format</param>
        /// <returns>SRMQuantResponse</returns>
        public static SRMQuantResponse InsertUpdateQuantSavedSearch(string searchID, string userName, string searchName, string searchQuery, string searchEncodedQuery)
        {
            mLogger.Debug("SRMQuantController : Start -> InsertUpdateQuantSavedSearch");
            RHashlist mHList = new RHashlist();
            RDBConnectionManager dbConn = CommonDALWrapper.GetConnectionManager(ConnectionConstants.SecMaster_Connection, true, IsolationLevel.Serializable);
            List<SRMQuantSavedSearches> lstQuantSavedSearches = new List<SRMQuantSavedSearches>();
            SRMQuantResponse responseInfo = new SRMQuantResponse();
            try
            {
                if (string.IsNullOrEmpty(searchID))
                {
                    CommonDALWrapper.ExecuteQuery("INSERT INTO IVPSecMaster.dbo.ivp_secm_quant_saved_searches VALUES ('" + searchName + "','" + searchQuery + "','" + searchEncodedQuery + "','true','" + userName + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "','" + userName + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "')", CommonQueryType.Insert, dbConn);
                }
                else
                {
                    CommonDALWrapper.ExecuteQuery("UPDATE IVPSecMaster.dbo.ivp_secm_quant_saved_searches SET search_name = '" + searchName + "',search_query = '" + searchQuery + "', search_encoded_query = '" + searchEncodedQuery + "', last_modified_by = '" + userName + "', last_modified_on = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' WHERE search_id = " + searchID, CommonQueryType.Update, dbConn);
                }
                dbConn.CommitTransaction();
                responseInfo.Message = "Search saved";
                responseInfo.IsSuccess = true;
            }
            catch (Exception Ex)
            {
                dbConn.RollbackTransaction();
                responseInfo.IsSuccess = false;
                responseInfo.Message = "Save operation failed : " + Ex.Message;
                mLogger.Error(Ex.ToString());
                return responseInfo;
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(dbConn);
                if (mHList != null)
                {
                    mHList.Clear();
                    mHList = null;
                }
                mLogger.Debug("SRMQuantController : End -> InsertUpdateQuantSavedSearch");
            }
            return responseInfo;
        }

        /// <summary>
        /// DeleteQuantSavedSearch - Deletes the saved search
        /// </summary>
        /// <param name="searchID">Search ID to be deleted</param>
        /// <param name="userName">Name of the user deleting the saved search</param>
        /// <returns>SRMQuantResponse</returns>
        public static SRMQuantResponse DeleteQuantSavedSearch(string searchID, string userName)
        {
            mLogger.Debug("SRMQuantController : Start -> DeleteQuantSavedSearch");
            RHashlist mHList = new RHashlist();
            RDBConnectionManager dbConn = CommonDALWrapper.GetConnectionManager(ConnectionConstants.SecMaster_Connection, true, IsolationLevel.Serializable);
            List<SRMQuantSavedSearches> lstQuantSavedSearches = new List<SRMQuantSavedSearches>();
            SRMQuantResponse responseInfo = new SRMQuantResponse();
            try
            {
                CommonDALWrapper.ExecuteQuery("UPDATE IVPSecMaster.dbo.ivp_secm_quant_saved_searches SET is_active = 0, last_modified_by = '" + userName + "', last_modified_on = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' WHERE search_id = " + searchID, CommonQueryType.Update, dbConn);
                dbConn.CommitTransaction();
                responseInfo.Message = "Search Deleted";
                responseInfo.IsSuccess = true;
            }
            catch (Exception Ex)
            {
                dbConn.RollbackTransaction();
                responseInfo.IsSuccess = false;
                responseInfo.Message = "Delete operation failed : " + Ex.Message;
                mLogger.Error(Ex.ToString());
                return responseInfo;
                //return lstQuantSavedSearches;
            }
            finally
            {
                CommonDALWrapper.PutConnectionManager(dbConn);
                if (mHList != null)
                {
                    mHList.Clear();
                    mHList = null;
                }
                mLogger.Debug("SRMQuantController : End -> DeleteQuantSavedSearch");
            }
            return responseInfo;
        }

        //Start Vikas Bhat, 07-Apr-2017

        /// <summary>
        /// Fetches the results in the form of datatable based on the query input
        /// </summary>
        /// <param name="query">The query in text form</param>
        /// <param name="mainSeparator">Character separating the universe keywords</param>
        /// <param name="functionSeparator">Character separating the values inside a function</param>
        /// <param name="functionStart">Character at the start of a function</param>
        /// <param name="functionEnd">Character at the end of a function</param>
        /// <returns>DataTable</returns>
        public static DataTable GetQueryResults(string queryText, char mainSeparator, char functionSeparator, char functionStart, char functionEnd, char valueIdentifier, char querySeparator)
        {
            mLogger.Debug("SRMQuantController -> GetQueryResults -> Start");

            //char querySeparator = '♫';
            DataTable dtResult = new DataTable();
            List<SRMQuantFilterInfo> filterInfo = GetQuantIntellisenseData();
            lstAggregationInfo = new List<SRMQuantAggregationInfo>();
            string realQuery = string.Empty;
            ESQueryRequestInfo requestInfo = null;
            DataSet dsResult = null;
            List<string> lstKeys = null;
            string parentAttribute = string.Empty;
            string childAttribute = string.Empty;
            string pa = string.Empty;
            string ca = string.Empty;
            List<DataColumn> lstFilterColumns = null;

            try
            {
                mLogger.Debug("SRMQuantController -> GetQueryResults -> Getting number of queries");
                List<string> queries = queryText.Split(querySeparator).ToList();
                mLogger.Debug("SRMQuantController -> GetQueryResults -> Number of queries: " + queries.Count.ToString());

                int queryIndex = 1;

                foreach (string query in queries)
                {
                    mLogger.Debug("SRMQuantController -> GetQueryResults -> Current Query: " + query);
                    if (!string.IsNullOrEmpty(query))
                    {
                        lstFilterColumns = new List<DataColumn>();

                        mLogger.Debug("SRMQuantController -> GetQueryResults -> Getting parent and child attribute");
                        query.Split(mainSeparator).ToList().ForEach(str =>
                        {
                            pa = filterInfo.Where(x => x.FilterID == str && !string.IsNullOrEmpty(x.ParentAttribute)).Select(y => y.ParentAttribute).FirstOrDefault();
                            ca = filterInfo.Where(x => x.FilterID == str && !string.IsNullOrEmpty(x.ChildAttribute)).Select(y => y.ChildAttribute).FirstOrDefault();

                            if (!string.IsNullOrEmpty(pa))
                                parentAttribute = pa;
                            if (!string.IsNullOrEmpty(ca))
                                childAttribute = ca;
                        });
                        mLogger.Debug("SRMQuantController -> GetQueryResults -> Parent Attribute: " + (!string.IsNullOrEmpty(parentAttribute) ? parentAttribute : string.Empty));
                        mLogger.Debug("SRMQuantController -> GetQueryResults -> Child Attribute: " + (!string.IsNullOrEmpty(childAttribute) ? childAttribute : string.Empty));

                        realQuery = GetQueryTextFromId(query, mainSeparator, functionSeparator, functionStart, functionEnd, valueIdentifier, filterInfo);

                        mLogger.Debug("SRMQuantController -> GetQueryResults --> QueryText: " + realQuery);

                        if (!string.IsNullOrEmpty(realQuery))
                            requestInfo = GetFilterData(realQuery, filterInfo, functionSeparator, mainSeparator, valueIdentifier, parentAttribute, childAttribute, lstKeys, queryIndex, lstFilterColumns);
                        if (requestInfo != null)
                            dtResult = SearchData(requestInfo, lstFilterColumns);
                        else
                            dtResult = null;

                        if (dtResult != null && dtResult.Rows.Count > 0 && queryIndex < queries.Count)
                        {
                            lstKeys = dtResult.AsEnumerable().Select(r => r.Field<string>(parentAttribute)).Distinct().ToList();
                            mLogger.Debug("SRMQuantController -> GetQueryResults --> Keys for next wrapping: " + lstKeys.Count.ToString());
                        }
                    }
                    queryIndex++;
                }

                //if (lstAggregationInfo != null && lstAggregationInfo.Count > 0)
                //{
                //    dtResult = ApplyAllAggregations(dtResult, lstAggregationInfo);
                //}
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMQuantController -> GetQueryResults -> " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMQuantController -> GetQueryResults -> End");
            }


            return dtResult;
        }




        private static DataTable ApplyAllAggregations(DataTable dtResult, List<SRMQuantAggregationInfo> lstAggregationInfo)
        {
            foreach (SRMQuantAggregationInfo aggrInfo in lstAggregationInfo)
            {
                dtResult = ApplyAggregation(dtResult, aggrInfo);
            }

            return dtResult;
        }

        private static DataTable ApplyAggregation(DataTable dtResult, SRMQuantAggregationInfo aggrInfo)
        {
            string attributeName = aggrInfo.AttributeName;
            string value = aggrInfo.Value1;
            string value2 = aggrInfo.Value2;
            string parentAttribute = "Ticker and Exchange Code";
            DataTable dtNew = dtResult.Clone();

            switch (aggrInfo.AggregationType)
            {
                case SRMQuantAggregationType.AVERAGE:
                    switch (aggrInfo.MatchType)
                    {
                        case SRMQuantAggregationMatchType.GREATER_THAN:
                            var questions = from tab in dtResult.AsEnumerable()
                                            group tab by tab[parentAttribute]
                                                into final
                                                where final.Average(x => Convert.ToDecimal(x[attributeName])) > Convert.ToDecimal(value)
                                                select final;


                            foreach (var item in questions)
                            {
                                foreach (DataRow row in item)
                                    dtNew.Rows.Add(row.ItemArray);
                            }

                            break;

                        case SRMQuantAggregationMatchType.EQUALS:
                            questions = from tab in dtResult.AsEnumerable()
                                        group tab by tab[parentAttribute]
                                            into final
                                            where final.Average(x => Convert.ToDecimal(x[attributeName])) == Convert.ToDecimal(value)
                                            select final;


                            foreach (var item in questions)
                            {
                                foreach (DataRow row in item)
                                    dtNew.Rows.Add(row.ItemArray);
                            }

                            break;

                        default:
                            break;
                    }


                    break;

                default:
                    break;
            }

            return dtNew;
        }

        public static string GetNumerics(string value)
        {
            value = value.Trim();


            if (value.Length > 1)
            {
                value = GetNumericValue(value, value.Substring(value.Length - 1, 1).ToLower());
            }

            return value;
        }

        public static string GetNumericValue(string value, string text)
        {
            int prefix = 1;

            switch (text)
            {
                case "m":
                    value = value.ToLower().Replace(text, "").Trim();
                    prefix = 1000000;
                    break;
                case "k":
                    value = value.ToLower().Replace(text, "").Trim();
                    prefix = 1000;
                    break;
                case "b":
                    value = value.ToLower().Replace(text, "").Trim();
                    prefix = 1000000000;
                    break;
                default:
                    break;

            }

            if (prefix > 1)
                value = Convert.ToString(Convert.ToDecimal(value) * prefix);

            return value;
        }

        public static string GetAggregationOperator(string operatorText)
        {
            string operatorType = string.Empty;
            Dictionary<string, List<string>> lstOperators = new Dictionary<string, List<string>>();
            lstOperators.Add("==", new List<string>() { "=", "==", "equal to", "equals to", "equals", "is equal to" });
            lstOperators.Add("!=", new List<string>() { "!=", "<>", "not equal to", "not equals to", "not equals", "does not equal", "does not equals" });
            lstOperators.Add(">", new List<string>() { ">", "greater than", "is greater than", "more than", "is more than", "above", "is above" });
            lstOperators.Add("<", new List<string>() { "<", "less than", "lesser than", "is less than", "is lesser than", "below", "is below" });
            lstOperators.Add(">=", new List<string>() { ">=", "is greater than or equal to", "is greater than equal to", "greater than or equal to", "greater than equal to" });
            lstOperators.Add("<=", new List<string>() { "<=", "is less than or equal to", "is less than equal to", "less than or equal to", "less than equal to", "is lesser than or equal to", "is lesser than equal to", "lesser than or equal to", "lesser than equal to" });
            lstOperators.Add("between", new List<string>() { "is between", "between", "in between" });

            operatorType = lstOperators.Where(x => x.Value.Contains(operatorText)).Select(y => y.Key).FirstOrDefault();
            return operatorType;
        }

        public static string GetAggregationQuery(string attributeName, string parentName, string value1, string value2, string operatorText, string aggregationText, int period, string periodType)
        {
            string query = string.Empty;
            string operatorValue = GetAggregationOperator(operatorText);
            string periodValue = "now-" + Convert.ToString(period) + periodType;

            if (!string.IsNullOrEmpty(value1))
                value1 = GetNumerics(value1);
            if (!string.IsNullOrEmpty(value2))
                value2 = GetNumerics(value2);

            query = "{";
            query = query + "\"size\": 0, ";
            query = query + "\"query\": { ";
            query = query + "\"range\": { ";
            query = query + "\"last_modified_on\": { ";
            query = query + "\"from\": \"" + periodValue + "\" ";
            query = query + "}}},";
            query = query + "\"aggs\": { ";
            query = query + "\"group_by_parent\": { ";
            query = query + "\"terms\": { ";
            query = query + "\"field\": ";
            query = query + "\"" + parentName + ".keyword\",";
            query = query + "\"size\": 1000000 ";
            query = query + "},";
            query = query + "\"aggs\": { ";
            query = query + "\"aggr_value\": { ";
            query = query + "\"avg\": { ";
            query = query + "\"field\": ";
            query = query + "\"" + attributeName + "\"";
            query = query + "}},";
            query = query + "\"beta_value\": { ";
            query = query + "\"bucket_selector\": { ";
            query = query + "\"buckets_path\": { ";
            query = query + "\"betaRange\":";
            query = query + "\"aggr_value\"";
            query = query + "},";
            query = query + "\"script\": ";
            if (operatorValue == "between")
                query = query + "\"params.betaRange >= " + value1 + " && params.betaRange <= " + value2 + " \" ";
            else
                query = query + "\"params.betaRange " + operatorValue + " " + value1 + " \" ";
            query = query + "}}}}}}";

            return query;
        }

        public static HashSet<string> GetAggregatedTable(List<string> filters, List<SRMQuantFilterInfo> filterInfo, string queryText, char functionSeparator, char mainSeparator, char valueIdentifier, int index, int maxIndex, string parentAttribute, List<DataColumn> lstFilterColumns)
        {
            mLogger.Debug("SRMQuantController -> GetAggregatedTable -> Start");

            string aggrOp = string.Empty;
            string aggrOperator = string.Empty;
            string aggrAttr = string.Empty;
            string aggrAttribute = string.Empty;
            int operatorIndex = 0;
            int attributeIndex = 0;
            int nextIndex = 0;
            string value = string.Empty;
            string value1 = string.Empty;
            string value2 = string.Empty;
            HashSet<string> keys = new HashSet<string>();

            try
            {
                for (int j = index + 1; j <= maxIndex; j++)
                {
                    aggrOp = filters[j];
                    aggrOperator = filterInfo.Where(x => x.FilterName == aggrOp && x.KeywordType == "Operator").Select(y => y.FilterName).FirstOrDefault();
                    operatorIndex = j;
                    if (!string.IsNullOrEmpty(aggrOperator))
                        break;
                }

                mLogger.Debug("SRMQuantController -> GetAggregatedTable -> Operator: " + aggrOperator);

                for (int j = index + 1; j <= maxIndex; j++)
                {
                    aggrAttr = filters[j];
                    aggrAttribute = filterInfo.Where(x => x.FilterName == aggrAttr && (x.KeywordType == "LHSAttribute" || x.KeywordType == "RHSAttribute")).Select(y => y.FilterName).FirstOrDefault();
                    attributeIndex = j;
                    if (!string.IsNullOrEmpty(aggrAttribute))
                        break;
                }

                mLogger.Debug("SRMQuantController -> GetAggregatedTable -> Attribute: " + aggrAttribute);

                //string aggrOperator = filters.Where(x => filters.IndexOf(x) > index).FirstOrDefault();
                value = filters.Where(x => filters.IndexOf(x) > operatorIndex).FirstOrDefault();
                nextIndex = operatorIndex + 2;


                if (value.Split(functionSeparator).Length > 1)
                {
                    value1 = value.Split(functionSeparator)[0];
                    value2 = value.Split(functionSeparator)[1];
                }
                else if (value.Split(functionSeparator).Length == 1)
                {
                    value1 = value.Split(functionSeparator)[0];
                }

                value1 = value1.Substring(1, value1.LastIndexOf(valueIdentifier) - 1);
                if (!string.IsNullOrEmpty(value2))
                    value2 = value2.Substring(1, value2.LastIndexOf(valueIdentifier) - 1);

                string query = string.Empty;
                string output = string.Empty;
                int period = 0;
                string periodType = string.Empty;

                mLogger.Debug("SRMQuantController -> GetAggregatedTable -> Getting Periodicity");
                if (filters[attributeIndex + 1][0] == valueIdentifier)
                {
                    string periodValue = filters[attributeIndex + 1];
                    periodValue = periodValue.Substring(1, periodValue.LastIndexOf(valueIdentifier) - 1);
                    periodValue = periodValue.ToLower();
                    periodValue = periodValue.Replace("d", " d").Replace("m", " m").Replace("y", " y").Replace("da y", "day");
                    periodValue = periodValue.Replace("  ", " ");
                    if (periodValue.Split(' ').Length > 1)
                    {
                        periodType = periodValue.Split(' ')[1];
                        period = Convert.ToInt32(periodValue.Split(' ')[0]);
                        //DateTime? periodDate = null;
                        if (periodType == "day" || periodType == "days" || periodType == "d")
                        {
                            periodType = "d";
                        }
                        else if (periodType == "month" || periodType == "months" || periodType == "m")
                        {
                            periodType = "M";
                        }
                        else if (periodType == "year" || periodType == "years" || periodType == "y")
                        {
                            periodType = "y";
                        }

                    }
                }

                mLogger.Debug("SRMQuantController -> GetAggregatedTable -> Period Value: " + period);
                mLogger.Debug("SRMQuantController -> GetAggregatedTable -> Period Type: " + periodType);

                DataColumn aggrColumn = new DataColumn() { ColumnName = aggrAttribute, DataType = typeof(decimal) };
                if (!lstFilterColumns.Contains(aggrColumn))
                {
                    lstFilterColumns.Add(aggrColumn);
                }

                query = GetAggregationQuery(aggrAttribute, parentAttribute, value1, value2, aggrOperator, filters[index], period, periodType);

                mLogger.Debug("SRMQuantController -> GetAggregatedTable -> Aggregation Query Formed: " + query);

                string esIndex = string.Empty;
                string historicalIndex = string.Empty;

                foreach (string filter in filters)
                {
                    esIndex = filterInfo.Where(x => x.FilterName == filter && x.KeywordType == "Universe").Select(y => y.IndexName).FirstOrDefault();
                    if (!string.IsNullOrEmpty(esIndex))
                    {
                        esIndex = esIndex.Substring(0, esIndex.LastIndexOf("_1"));
                        historicalIndex = esIndex + "historical_1";
                    }
                }

                mLogger.Debug("SRMQuantController -> GetAggregatedTable -> Historical Index: " + historicalIndex);
                if (!string.IsNullOrEmpty(query) && !string.IsNullOrEmpty(historicalIndex))
                {
                    string esURL = string.Empty;
                    //esURL = "http://192.168.0.103:9200/" + historicalIndex;
                    ElasticAPI.GetAggregationQueryData(historicalIndex, query, null, out output, 0, 10000);
                }

                if (!string.IsNullOrEmpty(output))
                {
                    int cnt = output.IndexOf("\"buckets\":");
                    output = output.Substring(cnt, output.Length - cnt - 3).Replace("\"buckets\":", "");
                    //output = "{" + output + "}";
                }

                mLogger.Debug("SRMQuantController -> GetAggregatedTable -> Aggregation Output: " + (!string.IsNullOrEmpty(output) ? "true" : "false"));

                if (!string.IsNullOrEmpty(output))
                {
                    keys = new HashSet<string>(((Newtonsoft.Json.Linq.JArray)JsonConvert.DeserializeObject(output)).Select(x => Convert.ToString(x["key"])));
                }

            }

            catch (Exception ex)
            {
                mLogger.Error("SRMQuantController -> GetAggregatedTable -> " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMQuantController -> GetAggregatedTable -> End");
            }

            return keys;
        }

        /// <summary>
        /// Prepares the request info that is fed to the Elastic Search
        /// </summary>
        /// <param name="queryText">The query in text form</param>
        /// <param name="filterInfo">The intellisense object</param>
        /// <param name="functionSeparator">Character separating the values inside a function</param>
        /// <param name="mainSeparator">Character separating the universe keywords</param>
        /// <returns>ESQueryRequestInfo</returns>
        private static ESQueryRequestInfo GetFilterData(string queryText, List<SRMQuantFilterInfo> filterInfo, char functionSeparator, char mainSeparator, char valueIdentifier, string parentAttribute, string childAttribute, List<string> lstKeys, int queryIndex, List<DataColumn> lstFilterColumns)
        {
            mLogger.Debug("SRMQuantController -> GetFilterData -> Start");
            ESQueryRequestInfo requestInfo = new ESQueryRequestInfo();
            ESQueryEntity subQuery = new ESQueryEntity();
            SRMQuantAggregationInfo aggregationInfo = null;
            //string childAttribute = "Underlying Ticker";

            try
            {
                if (filterInfo == null)
                    filterInfo = GetQuantIntellisenseData();


                requestInfo.QueryType = ESQueryTypeEnum.AdvancedSearch;

                requestInfo.RequireDataTable = false;
                requestInfo.Size = 100000;
                requestInfo.Filters = new List<ESQueryFilter>();
                requestInfo.RequiredAttributeRealNames = new List<string>();
                requestInfo.Query = new List<ESQueryExecution>();

                if (queryIndex > 1 && (lstKeys == null || lstKeys.Count == 0))
                    return null;

                if (lstKeys != null && lstKeys.Count > 0)
                {
                    mLogger.Debug("SRMQuantController -> GetFilterData -> Creating wrapping query");

                    ESQueryEntity wrappingQuery = new ESQueryEntity();
                    string keyValue = string.Empty;
                    keyValue = string.Join(" OR ", lstKeys).Replace("/", "\\/");
                    //foreach (string key in lstKeys)
                    //{
                    //    keyValue = keyValue + (keyValue == string.Empty ? "" : " OR ") + key.Replace("/", "\\/");
                    //}
                    wrappingQuery.AttributeRealName = childAttribute + ".keyword";
                    wrappingQuery.DataType = ESValueDataType.STRING;
                    wrappingQuery.MatchType = ESAdvancedQueryMatchType.IN;
                    wrappingQuery.Value = keyValue;

                    requestInfo.Query.Add(wrappingQuery);
                }

                List<string> filters = queryText.Split(mainSeparator).ToList();

                int maxIndex = filters.Count - 1;

                foreach (string filter in filters)
                {
                    //filterInfo.Where(x => x.FilterName == filter && (x.KeywordType == "LHSAttribute" || x.KeywordType == "RHSAttribute"))

                    if (filterInfo.Any(x => x.FilterName == filter && x.KeywordType == "Function"))
                    {
                        mLogger.Debug("SRMQuantController -> GetFilterData -> Aggregation Start");
                        int thisIndex = filters.IndexOf(filter);
                        HashSet<string> keys = GetAggregatedTable(filters, filterInfo, queryText, functionSeparator, mainSeparator, valueIdentifier, thisIndex, maxIndex, parentAttribute, lstFilterColumns);
                        if (keys != null && keys.Count > 0)
                        {
                            mLogger.Debug("SRMQuantController -> GetFilterData -> Aggregation Keys Found: " + keys.Count.ToString());

                            ESQueryEntity aggrQuery = new ESQueryEntity();
                            string keyValue = string.Empty;
                            keyValue = string.Join(" OR ", keys).Replace("/", "\\/");
                            //foreach (string str in keys)
                            //{
                            //    keyValue = keyValue + (keyValue == string.Empty ? "" : " OR ") + str.Replace("/", "\\/");
                            //}
                            aggrQuery.AttributeRealName = parentAttribute + ".keyword";
                            aggrQuery.DataType = ESValueDataType.STRING;
                            aggrQuery.MatchType = ESAdvancedQueryMatchType.IN;
                            aggrQuery.Value = keyValue;

                            requestInfo.Query.Add(aggrQuery);
                        }

                        else
                            return null;

                        mLogger.Debug("SRMQuantController -> GetFilterData -> Aggregation End");
                    }

                }

                int index = 0;
                int nextIndex = 0;

                for (int i = 0; i < filters.Count; i++)
                {
                    if (i < nextIndex)
                        continue;

                    string value = string.Empty;
                    string value1 = string.Empty;
                    string value2 = string.Empty;
                    subQuery = new ESQueryEntity();
                    string filter = filters[i];
                    index = i;

                    SRMQuantFilterInfo info = filterInfo.Where(x => x.FilterName.ToLower() == filter.ToLower()).FirstOrDefault();
                    if (info != null)
                    {
                        mLogger.Debug("SRMQuantController -> GetFilterData -> Current Keyword: " + info.FilterName + ", Type: " + info.KeywordType);

                        if (info.KeywordType == "Universe" && !string.IsNullOrEmpty(info.IndexName))
                        {
                            requestInfo.indexName = info.IndexName;
                        }
                        else if (info.KeywordType == "Sub-Universe")
                        {
                            string parentConstituent = filters[index - 1];
                            if (parentConstituent == "SPX" || parentConstituent == "DJX")
                            {
                                ESQueryEntity constituentQuery = new ESQueryEntity();
                                string keyValue = string.Empty;
                                constituentQuery.AttributeRealName = parentConstituent;
                                constituentQuery.DataType = ESValueDataType.STRING;
                                constituentQuery.MatchType = ESAdvancedQueryMatchType.EQUALS;
                                constituentQuery.Value = parentConstituent;

                                requestInfo.Query.Add(constituentQuery);
                            }
                        }
                        else if (info.KeywordType == "Boolean Attribute")
                        {
                            ESQueryEntity boolQuery = new ESQueryEntity();
                            boolQuery.AttributeRealName = filter;
                            boolQuery.DataType = ESValueDataType.BOOLEAN;
                            boolQuery.MatchType = ESAdvancedQueryMatchType.EQUALS;
                            boolQuery.Value = "true";

                            requestInfo.Query.Add(boolQuery);
                        }
                        else if (info.KeywordType == "LHSAttribute" || info.KeywordType == "RHSAttribute")
                        {
                            subQuery.AttributeRealName = filter;
                            string operatorText = filters[index + 1];
                            value = filters[index + 2];
                            value1 = string.Empty;
                            value2 = string.Empty;

                            mLogger.Debug("SRMQuantController -> GetFilterData -> OperatorText: " + operatorText);
                            mLogger.Debug("SRMQuantController -> GetFilterData -> Filter: " + filter);

                            subQuery.MatchType = GetMatchType(operatorText);
                            subQuery.DataType = GetAttributeDataType(filter);

                            if (value.Split(functionSeparator).Length > 1)
                            {
                                value1 = value.Split(functionSeparator)[0];
                                value2 = value.Split(functionSeparator)[1];
                            }
                            else if (value.Split(functionSeparator).Length == 1)
                            {
                                value1 = value.Split(functionSeparator)[0];
                            }

                            value1 = value1.Substring(1, value1.LastIndexOf(valueIdentifier) - 1);
                            if (!string.IsNullOrEmpty(value2))
                                value2 = value2.Substring(1, value2.LastIndexOf(valueIdentifier) - 1);


                            if (subQuery.DataType == ESValueDataType.NUMERIC)
                            {
                                if (!string.IsNullOrEmpty(value1))
                                    value1 = GetNumerics(value1);
                                if (!string.IsNullOrEmpty(value2))
                                    value2 = GetNumerics(value2);
                            }


                            subQuery.Value = value1;
                            subQuery.Value2 = value2;

                            mLogger.Debug("SRMQuantController -> GetFilterData -> Value1: " + value1);
                            mLogger.Debug("SRMQuantController -> GetFilterData -> Value2: " + value2);

                            requestInfo.Query.Add(subQuery);

                            DataColumn filColumn = new DataColumn() { ColumnName = filter, DataType = (subQuery.DataType == ESValueDataType.NUMERIC) ? typeof(decimal) : typeof(string) };
                            if (!lstFilterColumns.Contains(filColumn))
                            {
                                lstFilterColumns.Add(filColumn);
                            }
                        }
                        else if (info.KeywordType == "Filter Connectors")
                        {
                            ESQueryOperator operatorType = new ESQueryOperator();
                            if (filter.ToLower().Trim() == "or")
                                operatorType.Operator = ESQueryOperatorTypeEnum.OR;
                            else
                                operatorType.Operator = ESQueryOperatorTypeEnum.AND;
                            requestInfo.Query.Add(operatorType);
                        }

                        else if (info.KeywordType == "Function")
                        {
                            //int operatorIndex = filter.
                            string aggrOp = string.Empty;
                            string aggrOperator = string.Empty;
                            string aggrAttr = string.Empty;
                            string aggrAttribute = string.Empty;
                            int operatorIndex = 0;
                            int attributeIndex = 0;

                            for (int j = index + 1; j <= maxIndex; j++)
                            {
                                aggrOp = filters[j];
                                aggrOperator = filterInfo.Where(x => x.FilterName == aggrOp && x.KeywordType == "Operator").Select(y => y.FilterName).FirstOrDefault();
                                operatorIndex = j;
                                if (!string.IsNullOrEmpty(aggrOperator))
                                    break;
                            }

                            for (int j = index + 1; j <= maxIndex; j++)
                            {
                                aggrAttr = filters[j];
                                aggrAttribute = filterInfo.Where(x => x.FilterName == aggrAttr && (x.KeywordType == "LHSAttribute" || x.KeywordType == "RHSAttribute")).Select(y => y.FilterName).FirstOrDefault();
                                attributeIndex = j;
                                if (!string.IsNullOrEmpty(aggrAttribute))
                                    break;
                            }

                            //string aggrOperator = filters.Where(x => filters.IndexOf(x) > index).FirstOrDefault();
                            value = filters.Where(x => filters.IndexOf(x) > operatorIndex).FirstOrDefault();
                            nextIndex = operatorIndex + 2;

                            //requestInfo.Filters = new List<ESQueryFilter>();
                            //ESQueryFilter periodInfo = new ESQueryFilter();
                            //if (filters[attributeIndex + 1][0] == valueIdentifier)
                            //{
                            //    string periodValue = filters[attributeIndex + 1];
                            //    periodValue = periodValue.Substring(1, periodValue.LastIndexOf(valueIdentifier) - 1);
                            //    if (periodValue.Split(' ').Length > 1)
                            //    {
                            //        string periodType = periodValue.Split(' ')[1];
                            //        int period = Convert.ToInt32(periodValue.Split(' ')[0]);
                            //        DateTime? periodDate = null;
                            //        if (periodType.Contains("day"))
                            //        {
                            //            periodDate = DateTime.Now.AddDays(0 - period);
                            //        }
                            //        else if (periodType.Contains("month"))
                            //        {
                            //            periodDate = DateTime.Now.AddMonths(0 - period);
                            //        }
                            //        if (periodType.Contains("year"))
                            //        {
                            //            periodDate = DateTime.Now.AddYears(0 - period);
                            //        }

                            //        if (periodDate != null)
                            //        {
                            //            periodInfo.FilterStartDate = periodDate;
                            //            periodInfo.FilterType = ESQueryFilterType.Modified;

                            //            // requestInfo.Filters.Add(periodInfo);
                            //        }
                            //    }
                            //}

                        }
                    }
                }


            }

            catch (Exception ex)
            {
                mLogger.Error("SRMQuantController -> GetFilterData -> " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMQuantController -> GetFilterData -> End");
            }

            return requestInfo;
        }

        /// <summary>
        /// Method to fetch the match type enum based on the operator against a keyword
        /// </summary>
        /// <param name="operatorText">Operator against which the enum value needs to be fetched (e.g. "equal to")</param>
        /// <returns>ESAdvancedQueryMatchType</returns>
        private static ESAdvancedQueryMatchType GetMatchType(string operatorText)
        {
            mLogger.Debug("SRMQuantController -> GetMatchType -> Start");
            ESAdvancedQueryMatchType matchType;
            try
            {
                Dictionary<ESAdvancedQueryMatchType, List<string>> dictMatchTypeVsOperators = new Dictionary<ESAdvancedQueryMatchType, List<string>>();
                dictMatchTypeVsOperators = GetMatchTypeDictionary();
                if (dictMatchTypeVsOperators.Any(x => x.Value.Contains(operatorText)))
                    matchType = dictMatchTypeVsOperators.Where(x => x.Value.Contains(operatorText)).Select(y => y.Key).FirstOrDefault();
                else
                    matchType = ESAdvancedQueryMatchType.EQUALS;

            }
            catch (Exception ex)
            {
                mLogger.Error("SRMQuantController -> GetMatchType -> " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMQuantController -> GetMatchType -> End");
            }
            return matchType;

        }

        private static Dictionary<ESAdvancedQueryMatchType, List<string>> GetMatchTypeDictionary()
        {
            Dictionary<ESAdvancedQueryMatchType, List<string>> dictMatchTypeVsOperators = new Dictionary<ESAdvancedQueryMatchType, List<string>>();
            dictMatchTypeVsOperators.Add(ESAdvancedQueryMatchType.EQUALS, new List<string>() { "=", "as", "is" });
            dictMatchTypeVsOperators.Add(ESAdvancedQueryMatchType.DOES_NOT_EQUAL, new List<string>() { "not equal to" });
            dictMatchTypeVsOperators.Add(ESAdvancedQueryMatchType.GREATER_THAN, new List<string>() { ">" });
            dictMatchTypeVsOperators.Add(ESAdvancedQueryMatchType.LESS_THAN, new List<string>() { "<" });
            dictMatchTypeVsOperators.Add(ESAdvancedQueryMatchType.BETWEEN, new List<string>() { "between" });
            dictMatchTypeVsOperators.Add(ESAdvancedQueryMatchType.CONTAIN, new List<string>() { "contains" });
            dictMatchTypeVsOperators.Add(ESAdvancedQueryMatchType.GREATER_OR_EQUAL, new List<string>() { ">=" });
            dictMatchTypeVsOperators.Add(ESAdvancedQueryMatchType.LESS_OR_EQUAL, new List<string>() { "<=" });
            return dictMatchTypeVsOperators;
        }

        private static Dictionary<SRMQuantAggregationMatchType, List<string>> GetAggregationMatchTypeDictionary()
        {
            Dictionary<SRMQuantAggregationMatchType, List<string>> dictMatchTypeVsOperators = new Dictionary<SRMQuantAggregationMatchType, List<string>>();
            dictMatchTypeVsOperators.Add(SRMQuantAggregationMatchType.EQUALS, new List<string>() { "=" });
            dictMatchTypeVsOperators.Add(SRMQuantAggregationMatchType.DOES_NOT_EQUAL, new List<string>() { "not equal to" });
            dictMatchTypeVsOperators.Add(SRMQuantAggregationMatchType.GREATER_THAN, new List<string>() { ">" });
            dictMatchTypeVsOperators.Add(SRMQuantAggregationMatchType.LESS_THAN, new List<string>() { "<" });
            dictMatchTypeVsOperators.Add(SRMQuantAggregationMatchType.BETWEEN, new List<string>() { "between" });
            dictMatchTypeVsOperators.Add(SRMQuantAggregationMatchType.CONTAIN, new List<string>() { "contains" });
            return dictMatchTypeVsOperators;
        }

        private static SRMQuantAggregationMatchType GetAggregationMatchType(string operatorText)
        {
            mLogger.Debug("SRMQuantController -> GetMatchType -> Start");
            SRMQuantAggregationMatchType matchType;
            try
            {
                Dictionary<SRMQuantAggregationMatchType, List<string>> dictMatchTypeVsOperators = new Dictionary<SRMQuantAggregationMatchType, List<string>>();
                dictMatchTypeVsOperators = GetAggregationMatchTypeDictionary();
                if (dictMatchTypeVsOperators.Any(x => x.Value.Contains(operatorText)))
                    matchType = dictMatchTypeVsOperators.Where(x => x.Value.Contains(operatorText)).Select(y => y.Key).FirstOrDefault();
                else
                    matchType = SRMQuantAggregationMatchType.EQUALS;

            }
            catch (Exception ex)
            {
                mLogger.Error("SRMQuantController -> GetMatchType -> " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMQuantController -> GetMatchType -> End");
            }
            return matchType;

        }

        private static Dictionary<string, ESValueDataType> GetAttributeDataTypeDictionary()
        {
            Dictionary<string, ESValueDataType> dictAttributeVsDataTypes = new Dictionary<string, ESValueDataType>();
            string attribute_name = string.Empty;
            string data_type = string.Empty;

            DataTable dtResult = CommonDALWrapper.ExecuteSelectQuery(@"SELECT attribute_name, data_type FROM ivp_secm_quant_search_grid_info ", ConnectionConstants.SecMaster_Connection).Tables[0];

            if (dtResult != null && dtResult.Rows.Count > 0)
            {
                foreach (DataRow row in dtResult.Rows)
                {
                    attribute_name = Convert.ToString(row["attribute_name"]);
                    data_type = Convert.ToString(row["data_type"]);

                    if (!dictAttributeVsDataTypes.Keys.Contains(attribute_name))
                    {
                        switch (data_type)
                        {
                            case "STRING":
                                dictAttributeVsDataTypes.Add(attribute_name, ESValueDataType.STRING);
                                break;
                            case "NUMERIC":
                            case "INT":
                                dictAttributeVsDataTypes.Add(attribute_name, ESValueDataType.NUMERIC);
                                break;
                            case "DATE":
                            case "DATETIME":
                                dictAttributeVsDataTypes.Add(attribute_name, ESValueDataType.DATETIME);
                                break;
                            case "BIT":
                            case "BOOLEAN":
                                dictAttributeVsDataTypes.Add(attribute_name, ESValueDataType.BOOLEAN);
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            return dictAttributeVsDataTypes;
        }

        /// <summary>
        /// Method to get the datatype enum of an attribute
        /// </summary>
        /// <param name="attributeName">Name of the attribute</param>
        /// <returns>ESValueDataType</returns>
        private static ESValueDataType GetAttributeDataType(string attributeName)
        {
            mLogger.Debug("SRMQuantController -> GetAttributeDataType -> Start");
            Dictionary<string, ESValueDataType> dictAttributeVsDataTypes = new Dictionary<string, ESValueDataType>();
            try
            {
                dictAttributeVsDataTypes = GetAttributeDataTypeDictionary();

                if (dictAttributeVsDataTypes.Keys.Contains(attributeName))
                    return dictAttributeVsDataTypes[attributeName];
                else
                    return ESValueDataType.STRING;
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMQuantController -> GetAttributeDataType -> " + ex.Message);
                throw ex;
            }

            finally
            {
                mLogger.Debug("SRMQuantController -> GetAttributeDataType -> End");
            }

        }

        private static SRMQuantAggregationType GetAggregationType(string aggregationText)
        {
            return SRMQuantAggregationType.AVERAGE;
        }

        public static List<string> GetReferenceData(string attributeName)
        {
            List<string> lstValues = new List<string>();
            string indexName = "ix_" + attributeName.Replace(" ", "").ToLower() + "_1";
            string attrValue = string.Empty;
            ESQueryRequestInfo requestInfo = new ESQueryRequestInfo();

            mLogger.Debug("SRMQuantController -> SearchData -> Start");
            try
            {
                requestInfo.QueryType = ESQueryTypeEnum.AdvancedSearch;

                requestInfo.RequireDataTable = false;
                requestInfo.Size = 100000;
                requestInfo.Filters = new List<ESQueryFilter>();
                requestInfo.Query = new List<ESQueryExecution>();

                requestInfo.RequiredAttributeRealNames = new List<string>() { attributeName };
                requestInfo.indexName = indexName;

                ESQueryResposeInfo res = SMElasticSearchController.Search(requestInfo);

                if (res.IsSuccess && String.IsNullOrEmpty(res.FailureMessage) && (res.ResultObject != null) && (((((Newtonsoft.Json.Linq.JContainer)(res.ResultObject)))["errors"] == null) || ((((Newtonsoft.Json.Linq.JContainer)(res.ResultObject)))["errors"] != null && !((bool)(((Newtonsoft.Json.Linq.JValue)((((Newtonsoft.Json.Linq.JContainer)(res.ResultObject)))["errors"]))).Value))))
                {
                    foreach (var row in ((Newtonsoft.Json.Linq.JArray)((((Newtonsoft.Json.Linq.JProperty)(((((((Newtonsoft.Json.Linq.JContainer)(res.ResultObject))).Last).Last)).Last))).Value)))
                    {
                        foreach (var item in ((Newtonsoft.Json.Linq.JObject)(((row).Last).First)))
                        {
                            attrValue = Convert.ToString((((Newtonsoft.Json.Linq.JValue)(item.Value))).Value);
                        }

                        if (!string.IsNullOrEmpty(attrValue) && !lstValues.Contains(attrValue))
                            lstValues.Add(attrValue);
                        //break;
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMQuantController -> SearchData -> " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMQuantController -> SearchData -> End");
            }
            return lstValues;

        }

        /// <summary>
        /// Searches the data based on the request info given
        /// </summary>
        /// <param name="requestInfo">The info object</param>
        /// <returns>DataTable</returns>
        private static DataTable SearchData(ESQueryRequestInfo requestInfo, List<DataColumn> lstFilterColumns)
        {
            mLogger.Debug("SRMQuantController -> SearchData -> Start");
            DataTable gridDataTable = new DataTable();
            List<DataColumn> lstDataColumns = new List<DataColumn>();
            string whereQuery = string.Empty;

            try
            {
                if (requestInfo != null && !string.IsNullOrEmpty(requestInfo.indexName))
                {
                    whereQuery = " AND index_name = '" + requestInfo.indexName + "' ";
                }

                DataSet attributeDS = CommonDALWrapper.ExecuteSelectQuery("SELECT attribute_name,data_type FROM IVPSecMaster.dbo.ivp_secm_quant_search_grid_info WHERE is_displayed = 1 " + whereQuery + " ORDER BY attribute_display_order ", ConnectionConstants.SecMaster_Connection);
                if (attributeDS != null && attributeDS.Tables.Count > 0 && attributeDS.Tables[0].Rows.Count > 0)
                {
                    requestInfo.RequiredAttributeRealNames = new List<string>();
                    foreach (DataRow dataRow in attributeDS.Tables[0].Rows)
                    {
                        requestInfo.RequiredAttributeRealNames.Add(Convert.ToString(dataRow["attribute_name"]));
                        DataColumn tabColumn = new DataColumn();
                        tabColumn.ColumnName = Convert.ToString(dataRow["attribute_name"]);
                        string colDataType = Convert.ToString(dataRow["data_type"]);
                        switch (colDataType)
                        {
                            case "STRING":
                                tabColumn.DataType = typeof(string);
                                break;

                            case "INT":
                                tabColumn.DataType = typeof(Int64);
                                break;

                            case "NUMERIC":
                                tabColumn.DataType = typeof(decimal);
                                break;

                            default:
                                tabColumn.DataType = typeof(string);
                                break;
                        }

                        lstDataColumns.Add(tabColumn);

                        //lstDataColumns.Add(new DataColumn() { ColumnName = Convert.ToString(dataRow["attribute_name"]), DataType = Convert.ToString(dataRow["data_type"]) == "STRING" ? typeof(string) : typeof(decimal) });
                    }

                }

                if (lstFilterColumns != null && lstFilterColumns.Count > 0)
                {
                    lstFilterColumns.ForEach(col =>
                    {
                        if (!lstDataColumns.Any(x => x.ColumnName == col.ColumnName))
                            lstDataColumns.Add(col);

                        if (!requestInfo.RequiredAttributeRealNames.Contains(col.ColumnName))
                            requestInfo.RequiredAttributeRealNames.Add(col.ColumnName);
                    });
                }


                mLogger.Debug("Getting response from ES Start: " + DateTime.Now.ToString("hh:MM:ss.fff"));
                ESQueryResposeInfo res = SMElasticSearchController.Search(requestInfo);
                mLogger.Debug("Getting response from ES End: " + DateTime.Now.ToString("hh:MM:ss.fff"));

                mLogger.Debug("Converting to DataTable Start: " + DateTime.Now.ToString("hh:MM:ss.fff"));
                if (res.IsSuccess && String.IsNullOrEmpty(res.FailureMessage) && (res.ResultObject != null) && (((((Newtonsoft.Json.Linq.JContainer)(res.ResultObject)))["errors"] == null) || ((((Newtonsoft.Json.Linq.JContainer)(res.ResultObject)))["errors"] != null && !((bool)(((Newtonsoft.Json.Linq.JValue)((((Newtonsoft.Json.Linq.JContainer)(res.ResultObject)))["errors"]))).Value))))
                {
                    DataColumn autoColumn = new DataColumn();
                    autoColumn.ColumnName = "row_id";
                    autoColumn.AutoIncrement = true;
                    autoColumn.AutoIncrementSeed = 1;
                    gridDataTable.Columns.Add(autoColumn);

                    gridDataTable.Columns.AddRange(lstDataColumns.ToArray());

                    //foreach (var row in ((Newtonsoft.Json.Linq.JArray)((((Newtonsoft.Json.Linq.JProperty)(((((((Newtonsoft.Json.Linq.JContainer)(res.ResultObject))).Last).Last)).Last))).Value)))
                    //{
                    //    foreach (var item in ((Newtonsoft.Json.Linq.JObject)(((row).Last).First)))
                    //    {
                    //        DataColumn column = new DataColumn();
                    //        column.ColumnName = item.Key;
                    //        gridDataTable.Columns.Add(column);
                    //    }
                    //    break;
                    //}
                    foreach (var row in ((Newtonsoft.Json.Linq.JArray)((((Newtonsoft.Json.Linq.JProperty)(((((((Newtonsoft.Json.Linq.JContainer)(res.ResultObject))).Last).Last)).Last))).Value)))
                    {
                        DataRow dr = gridDataTable.NewRow();
                        foreach (var item in ((Newtonsoft.Json.Linq.JObject)(((row).Last).First)))
                        {
                            string attrValue = Convert.ToString((((Newtonsoft.Json.Linq.JValue)(item.Value))).Value);
                            if (!string.IsNullOrEmpty(attrValue))
                                dr[item.Key] = attrValue;
                        }
                        gridDataTable.Rows.Add(dr);
                        //break;
                    }
                }
                mLogger.Debug("Converting to DataTable End: " + DateTime.Now.ToString("hh:MM:ss.fff"));
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMQuantController -> SearchData -> " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMQuantController -> SearchData -> End");
            }
            return gridDataTable;
        }

        /// <summary>
        /// Method to prepare the actual query text from the encoded text containing keyword ids
        /// </summary>
        /// <param name="query">The query having keyword ids</param>
        /// <param name="mainSeparator">Character separating the universe keywords</param>
        /// <param name="functionSeparator">Character separating the values inside a function</param>
        /// <param name="functionStart">Character at the start of a function</param>
        /// <param name="functionEnd">Character at the end of a function</param>
        /// <returns>string</returns>
        private static string GetQueryTextFromId(string query, char mainSeparator, char functionSeparator, char functionStart, char functionEnd, char valueIdentifier, List<SRMQuantFilterInfo> filterInfo)
        {
            mLogger.Debug("SRMQuantController -> GetQueryTextFromId -> Start");
            string finalQuery = string.Empty;

            try
            {
                if (filterInfo != null)
                {
                    List<string> lstUniverse = query.Split(mainSeparator).ToList();
                    foreach (string universe in lstUniverse)
                    {
                        string copy = universe;
                        finalQuery = finalQuery + (finalQuery == string.Empty ? "" : mainSeparator.ToString());

                        if (universe[0] == valueIdentifier)
                        {
                            //copy = copy.TrimStart(functionStart).TrimEnd(functionEnd);
                            List<string> funcValues = copy.Split(functionSeparator).ToList();
                            int valIndex = 0;
                            foreach (string val in funcValues)
                            {
                                if (val[0] == valueIdentifier)
                                    finalQuery = finalQuery + (valIndex > 0 ? functionSeparator.ToString() : "") + val;
                                else
                                    finalQuery = finalQuery + filterInfo.Where(x => x.FilterID == val).Select(y => y.FilterName).FirstOrDefault();


                                valIndex++;
                            }
                        }
                        else if (universe[0] == functionStart)
                        {
                            string funcParam = copy.Substring(1, copy.LastIndexOf(functionEnd) - 1);
                            List<string> funcParams = funcParam.Split(functionSeparator).ToList();
                            int paramIndex = 0;

                            foreach (string funcVal in funcParams)
                            {
                                if (paramIndex > 0)
                                    finalQuery = finalQuery + (finalQuery == string.Empty ? "" : mainSeparator.ToString());

                                if (funcVal[0] == valueIdentifier)
                                {
                                    List<string> fnValues = funcVal.Split(functionSeparator).ToList();
                                    int valIndex = 0;
                                    foreach (string val in fnValues)
                                    {
                                        if (val[0] == valueIdentifier)
                                            finalQuery = finalQuery + (valIndex > 0 ? functionSeparator.ToString() : "") + val;
                                        else
                                            finalQuery = finalQuery + filterInfo.Where(x => x.FilterID == val).Select(y => y.FilterName).FirstOrDefault();


                                        valIndex++;
                                    }
                                }
                                else
                                    finalQuery = finalQuery + filterInfo.Where(x => x.FilterID == funcVal && !x.Hidden).Select(y => y.FilterName).FirstOrDefault();

                                paramIndex++;
                            }
                        }
                        else
                            finalQuery = finalQuery + filterInfo.Where(x => x.FilterID == universe && !x.Hidden).Select(y => y.FilterName).FirstOrDefault();
                    }
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("SRMQuantController -> GetQueryTextFromId -> " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("SRMQuantController -> GetQueryTextFromId -> End");
            }

            return finalQuery;
        }

        //End Vikas Bhat, 07-Apr-2017
    }

    [DataContract]
    public class SRMQuantFilterInfo
    {
        [DataMember]
        public string FilterName { get; set; }
        [DataMember]
        public string FilterID { get; set; }
        [DataMember]
        public string HelpText { get; set; }
        [DataMember]
        public string KeyWordLevel { get; set; }
        [DataMember]
        public bool Hidden { get; set; }
        [DataMember]
        public string KeywordType { get; set; }
        [DataMember]
        public string IndexName { get; set; }
        [DataMember]
        public string ParentIndex { get; set; }
        [DataMember]
        public string ParentAttribute { get; set; }
        [DataMember]
        public string ChildAttribute { get; set; }
        [DataMember]
        public string DataType { get; set; }
        [DataMember]
        public string OperatorType { get; set; }
        [DataMember]
        public bool IsReferenceAttribute { get; set; }
        [DataMember]
        public List<SRMQuantFilterInfo> Filters { get; set; }
        [DataMember]
        public Dictionary<string, List<SRMQuantFilterInfo>> Parameters { get; set; }
    }

    [DataContract]
    public class SRMQuantSavedSearches
    {
        [DataMember]
        public string SearchId { get; set; }
        [DataMember]
        public string SearchName { get; set; }
        [DataMember]
        public string SearchQuery { get; set; }
        [DataMember]
        public string SearchEncodedQuery { get; set; }

    }

    public class SRMQuantAggregationInfo
    {
        public SRMQuantAggregationType AggregationType { get; set; }
        public string AttributeName { get; set; }
        public string Periodicity { get; set; }
        public string Period { get; set; }
        public string Period2 { get; set; }
        public SRMQuantAggregationMatchType MatchType { get; set; }
        public string DataType { get; set; }
        public string Value1 { get; set; }
        public string Value2 { get; set; }


    }

    public enum SRMQuantAggregationType
    {
        AVERAGE = 0,
        SUM,
        MAX,
        MIN,
        COUNT
    }

    public enum SRMQuantAggregationMatchType
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
        LESS_THAN
    }

    [DataContract]
    public class SRMQuantResponse
    {
        [DataMember]
        public string Message { get; set; }
        [DataMember]
        public bool IsSuccess { get; set; }
    }


}
