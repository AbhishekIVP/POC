using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace com.ivp.common.lookupdatamassage
{
    public enum RMLookupInputType
    {
        REALNAME = 0,
        DISPLAYNAME = 1,
        ID = 2
    }
    public class RMLookupDataMassageInfo
    {
        public bool IsArchive = false;
        public bool IsEntityCodeToValues = true;
        public bool IsAuditView = false;
        public bool GetDisplayNames = false;
        public bool GetWorkflowData = false;
        public bool GetDraftData = false;
        public RMLookupInputType InputType;
        public DateTime DefaultDateForEffectiveDateColumn;
    }
    public class RMLookupAttributeInfo
    {
        public string AttributeRealName;
        public string AttributeDisplayName;
        public int AttributeId;
        public string EntityTypeRealName;
        public string EntityTypeDisplayName;
        public int EntityTypeId;
        public string ParentAttributeRealName;
        public string ParentAttributeDisplayName;
        public int ParentAttributeId;
        public string ParentRealName;
        public string ParentDisplayName;
        public int ParentId;
        public bool IsSecurityLookup;
        public List<string> MappedColumns;
        public string EffectiveDateColumn;
        public string KnowledgeDateColumn;
    }

    public class RMSecurityLookupAttributeInfo
    {
        public int ParentSecurityTypeID;
        public int ParentSecurityAttributeID;
        public List<string> MappedColumns;
    }
    public static class SRMExtensionMethodsLocal
    {
        public static HashSet<T> SRMToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }
        public static HashSet<T> SRMToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
        {
            return new HashSet<T>(source, comparer);
        }
    }

    public class RMLookupDataMassage
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("RMLookupDataMassage");
        private class RMAttributeInfoPerEntityType
        {
            public string EntityType;
            public HashSet<string> AttributesToFetch;
            public HashSet<object> ValuesToSearch;
            public string SearchColumn;
            public List<RMLookupEntityCodesAndDates> ArchiveData;
        }

        private class RMLookupEntityCodesAndDates
        {
            public object Value;
            public DateTime EffectiveDate;
            public DateTime KnowledgeDate;
        }

        #region Security Lookups

        public List<SecurityLookupOutputInfo> GetSecurityLookupData(IEnumerable<DataRow> dtInput, RMSecurityLookupAttributeInfo lookup, bool isSecIdToValue)
        {
            try
            {
                mLogger.Debug("GetSecurityLookupData -> Start");

                List<SecurityLookupOutputInfo> finalOutputInfo = new List<SecurityLookupOutputInfo>();
                List<SecurityLookupOutputInfo> outputInfo = null;
                List<object> valuesToSearch = new List<object>();
                SecurityLookupInputInfo inputInfo = new SecurityLookupInputInfo();
                outputInfo = new List<SecurityLookupOutputInfo>();

                lookup.MappedColumns.ForEach(mc =>
                {
                    valuesToSearch.AddRange(dtInput.AsEnumerable().Where(r => !string.IsNullOrEmpty(Convert.ToString(r[mc]))).Select(row => row[mc]).Distinct());
                });

                valuesToSearch = valuesToSearch.Distinct().ToList<object>();

                inputInfo.isSecIdToValues = isSecIdToValue;
                inputInfo.searchObjects = new List<SecurityLookupInfo>() { new SecurityLookupInfo() { SecurityAttributeID = lookup.ParentSecurityAttributeID, SecurityTypeID = lookup.ParentSecurityTypeID, ValuesToSearch = valuesToSearch } };

                mLogger.Debug("Start getting security lookup data");
                outputInfo = new SRMCommon().GetSecurityLookupData(inputInfo);
                mLogger.Debug("End getting security lookup data");

                if (outputInfo != null)
                {
                    finalOutputInfo.AddRange(outputInfo);
                }

                mLogger.Debug("GetSecurityLookupData -> End");
                return finalOutputInfo;
            }
            catch
            {
                mLogger.Debug("GetSecurityLookupData -> Error");
                throw;
            }
        }


        public void MassageSecurityLookupData(IEnumerable<DataRow> dtInput, List<RMSecurityLookupAttributeInfo> lstLookupAttributeInfo, bool isSecIdToValue)
        {
            try
            {
                mLogger.Debug("MassageSecurityLookups -> Start");

                List<SecurityLookupOutputInfo> outputInfo = null;

                lstLookupAttributeInfo.ForEach(lookup =>
                {
                    outputInfo = GetSecurityLookupData(dtInput, lookup, isSecIdToValue);

                    if (outputInfo != null && outputInfo.Count > 0)
                    {
                        List<SecurityLookupAttributeInfo> secData = outputInfo[0].ValuesToSearch;

                        if (secData != null && secData.Count > 0 && !isSecIdToValue)
                        {
                            var groupedData = secData.GroupBy(s => s.AttributeValue).Where(ss => ss.Count() > 1);

                            foreach (List<SecurityLookupAttributeInfo> group in groupedData)
                            {
                                group.ForEach(g =>
                                {
                                    secData.Remove(g);
                                });
                            }
                        }

                        if (secData != null && secData.Count > 0)
                        {
                            lookup.MappedColumns.ForEach(map =>
                            {
                                if (isSecIdToValue)
                                {
                                    var assign = from input in dtInput.Where(r => !string.IsNullOrEmpty(Convert.ToString(r[map])))
                                                 join sec in secData.AsEnumerable()
                                                 on input[map] equals sec.SecurityID
                                                 select AssignSecurityValues(input, map, sec.AttributeValue);

                                    assign.Count();
                                }
                                else
                                {
                                    var assign = from input in dtInput.Where(r => !string.IsNullOrEmpty(Convert.ToString(r[map])))
                                                 join sec in secData.AsEnumerable()
                                                 on input[map] equals sec.AttributeValue
                                                 select AssignSecurityValues(input, map, sec.SecurityID);

                                    assign.Count();
                                }
                            });
                        }
                    }
                });

                mLogger.Debug("MassageSecurityLookups -> End");
            }
            catch
            {
                mLogger.Debug("MassageSecurityLookups -> Error");
                throw;
            }
        }


        private bool AssignSecurityValues(DataRow row, string column, object value)
        {
            row[column] = value;
            return true;
        }

        #endregion

        #region Reference Lookups

        public void MassageLookupData(DataTable dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lstLookupAttributeInfo, RDBConnectionManager mDBCon = null)
        {
            mLogger.Debug("RMLookupDataMassage -> MassageLookupData -> Start");
            try
            {
                MassageLookupData(dtInput.AsEnumerable(), objRMLookupDataMassageInfo, lstLookupAttributeInfo, mDBCon);
            }
            catch (Exception ex)
            {
                mLogger.Error("RMLookupDataMassage -> MassageLookupData -> Error: " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMLookupDataMassage -> MassageLookupData -> End");
            }
            //return dtInput;
        }

        public void MassageLookupData(IEnumerable<DataRow> dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lstLookupAttributeInfo, RDBConnectionManager mDBCon = null)
        {
            mLogger.Debug("RMLookupDataMassage -> MassageLookupData -> Start");
            try
            {
                if (objRMLookupDataMassageInfo.IsEntityCodeToValues)
                {
                    if (!objRMLookupDataMassageInfo.IsAuditView)
                        ConvertLookupEntityCodesToValues(dtInput, objRMLookupDataMassageInfo, lstLookupAttributeInfo, mDBCon);
                }
                else
                {
                    if (!objRMLookupDataMassageInfo.IsAuditView)
                        ConvertLookupValuesToEntityCodes(dtInput, objRMLookupDataMassageInfo, lstLookupAttributeInfo, mDBCon);
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("RMLookupDataMassage -> MassageLookupData -> Error: " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMLookupDataMassage -> MassageLookupData -> End");
            }
            //return dtInput;
        }

        public void MassageLookupDataForArchive(IEnumerable<DataRow> dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lookupAttributeInfo, bool requireDayBestCopy, RDBConnectionManager mDBCon = null)
        {
            mLogger.Debug("RMLookupDataMassage -> MassageLookupData -> Start");
            try
            {

                if (!objRMLookupDataMassageInfo.IsArchive)
                    return;

                if (objRMLookupDataMassageInfo.IsEntityCodeToValues)
                {
                    ConvertLookupEntityCodesToValuesForAuditViewForArchive(dtInput, objRMLookupDataMassageInfo, lookupAttributeInfo, requireDayBestCopy, mDBCon);
                }
            }
            catch (Exception ex)
            {
                mLogger.Error("RMLookupDataMassage -> MassageLookupData -> Error: " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMLookupDataMassage -> MassageLookupData -> End");
            }
            //return dtInput;
        }

        private void ConvertLookupEntityCodesToValues(IEnumerable<DataRow> dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lstLookupAttributeInfo, RDBConnectionManager mDBCon)
        {
            mLogger.Debug("RMLookupDataMassage -> ConvertLookupEntityCodesToValues -> Start");
            DataSet result = null;
            List<RMAttributeInfoPerEntityType> lstEntityTypeInfo = new List<RMAttributeInfoPerEntityType>();

            try
            {

                lstLookupAttributeInfo.ForEach(lst =>
                {
                    List<object> values = new List<object>();
                    RMAttributeInfoPerEntityType objEntityTypeInfo = new RMAttributeInfoPerEntityType();

                    if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME)
                        objEntityTypeInfo.EntityType = lst.ParentRealName;
                    else if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME)
                        objEntityTypeInfo.EntityType = lst.ParentDisplayName;
                    else if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.ID)
                        objEntityTypeInfo.EntityType = lst.ParentId.ToString();

                    string fetchCol = lst.ParentAttributeRealName;

                    if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME)
                    {
                        fetchCol = lst.ParentDisplayName;
                    }
                    if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.ID)
                    {
                        fetchCol = lst.ParentAttributeId.ToString();
                    }

                    objEntityTypeInfo.AttributesToFetch = new HashSet<string>() { fetchCol };
                    //objEntityTypeInfo.AttributesToFetch = new HashSet<string>() { lst.ParentAttributeRealName };

                    if (dtInput != null && dtInput.Count() > 0)
                    {
                        lst.MappedColumns.ForEach(map =>
                        {
                            values.AddRange(dtInput.Where(i => !string.IsNullOrEmpty(Convert.ToString(i[map]))).Select(inp => inp[map]).Distinct().ToList<object>());
                        });
                    }
                    objEntityTypeInfo.SearchColumn = "entity_code";
                    objEntityTypeInfo.ValuesToSearch = values.SRMToHashSet();
                    lstEntityTypeInfo.Add(objEntityTypeInfo);
                });

                int rowCount = 0;
                XElement entityTypeXML = new XElement("root", from item in lstEntityTypeInfo.Where(x => x != null)
                                                              select new XElement("row",
                                                                 new XAttribute("ref", "R" + (++rowCount).ToString()),
                                                                 new XAttribute("entity_type_id", item.EntityType),
                                                                 new XAttribute("colsToFetch", string.Join(",", item.AttributesToFetch)),
                                                                 new XAttribute("searchColumn", item.SearchColumn),
                                                                from val in item.ValuesToSearch
                                                                select
                                                              new XElement("value", val)
                                                     ));

                rowCount = 0;

                XElement mappingXML = new XElement("root", from map in lstLookupAttributeInfo.Where(x => false && x != null && (x.ParentDisplayName != null || x.ParentId > 0 || x.ParentRealName != null))
                                                           select new XElement("row",
                                                              new XAttribute("ref", "R" + (++rowCount).ToString()),
                                                                new XElement("attr",
                                                                   new XAttribute("name", objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME ? map.AttributeRealName : (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME ? map.AttributeDisplayName : map.AttributeId.ToString())),
                                                                    new XAttribute("parentET", objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME ? map.ParentRealName : (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME ? map.ParentDisplayName : map.ParentId.ToString())),
                                                                    new XAttribute("parentAttr", objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME ? map.ParentAttributeRealName : (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME ? map.ParentAttributeDisplayName : map.ParentAttributeId.ToString()))
                                                               )
                                                               )
                                                     );


                result = GetData(entityTypeXML.ToString(), mappingXML.ToString(), (int)objRMLookupDataMassageInfo.InputType, objRMLookupDataMassageInfo.GetDisplayNames, objRMLookupDataMassageInfo.GetWorkflowData, objRMLookupDataMassageInfo.GetDraftData, mDBCon);

                //Start Massaging Logic
                int tabCount = 0;
                if (result != null)
                {
                    lstLookupAttributeInfo.ForEach(attr =>
                    {
                        if (result.Tables.Count > tabCount)
                        {
                            DataTable output = result.Tables[tabCount];
                            string joinCol = "entity_code";
                            string assignCol = "entity_code";

                            if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME)
                                assignCol = attr.ParentAttributeRealName;
                            else if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME)
                                assignCol = attr.ParentAttributeDisplayName;
                            else if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.ID)
                                assignCol = attr.ParentAttributeId.ToString();


                            attr.MappedColumns.ForEach(map =>
                            {
                                var assign = from ip in dtInput.Where(i => !string.IsNullOrEmpty(Convert.ToString(i[map])))
                                             join op in output.AsEnumerable()
                                             on ip[map] equals op[joinCol]
                                             //where !string.IsNullOrEmpty(Convert.ToString(ip[map]))
                                             select AssignValues(ip, op, map, assignCol);

                                assign.Count();
                            });
                        }

                        tabCount++;

                    });
                }
                //End Massaging Logic

            }
            catch (Exception ex)
            {
                mLogger.Error("RMLookupDataMassage -> ConvertLookupEntityCodesToValues -> Error: " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMLookupDataMassage -> ConvertLookupEntityCodesToValues -> End");
            }
        }

        private void ConvertLookupValuesToEntityCodes(IEnumerable<DataRow> dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lstLookupAttributeInfo, RDBConnectionManager mDBCon)
        {
            mLogger.Debug("RMLookupDataMassage -> ConvertLookupEntityCodesToValues -> Start");
            DataSet result = null;
            List<RMAttributeInfoPerEntityType> lstEntityTypeInfo = new List<RMAttributeInfoPerEntityType>();
            try
            {

                lstLookupAttributeInfo.ForEach(lst =>
                {
                    List<object> values = new List<object>();
                    RMAttributeInfoPerEntityType objEntityTypeInfo = new RMAttributeInfoPerEntityType();
                    if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME)
                        objEntityTypeInfo.EntityType = lst.ParentRealName;
                    else if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME)
                        objEntityTypeInfo.EntityType = lst.ParentDisplayName;
                    else if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.ID)
                        objEntityTypeInfo.EntityType = lst.ParentId.ToString();

                    string fetchCol = lst.ParentAttributeRealName;
                    string searchCol = lst.ParentAttributeRealName;

                    if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME)
                    {
                        fetchCol = lst.ParentAttributeDisplayName;
                        searchCol = lst.ParentAttributeDisplayName;
                    }
                    if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.ID)
                    {
                        fetchCol = lst.ParentAttributeId.ToString();
                        searchCol = lst.ParentAttributeId.ToString();
                    }

                    objEntityTypeInfo.AttributesToFetch = new HashSet<string>() { fetchCol };
                    //objEntityTypeInfo.AttributesToFetch = new HashSet<string>() { lst.ParentAttributeRealName };

                    if (dtInput != null && dtInput.Count() > 0)
                    {
                        lst.MappedColumns.ForEach(map =>
                        {
                            values.AddRange(dtInput.Where(i => !string.IsNullOrEmpty(Convert.ToString(i[map]))).Select(inp => inp[map]).Distinct().ToList<object>());
                        });
                    }
                    objEntityTypeInfo.SearchColumn = searchCol;
                    objEntityTypeInfo.ValuesToSearch = values.SRMToHashSet();
                    lstEntityTypeInfo.Add(objEntityTypeInfo);
                });

                int rowCount = 0;
                XElement entityTypeXML = new XElement("root", from item in lstEntityTypeInfo.Where(x => x != null)
                                                              select new XElement("row",
                                                                 new XAttribute("ref", "R" + (++rowCount).ToString()),
                                                                 new XAttribute("entity_type_id", item.EntityType),
                                                                 new XAttribute("colsToFetch", string.Join(",", item.AttributesToFetch)),
                                                                 new XAttribute("searchColumn", item.SearchColumn),
                                                                from val in item.ValuesToSearch
                                                                select
                                                              new XElement("value", val)
                                                     ));

                rowCount = 0;

                XElement mappingXML = new XElement("root", from map in lstLookupAttributeInfo.Where(x => false && x != null && (x.ParentDisplayName != null || x.ParentId > 0 || x.ParentRealName != null))
                                                           select new XElement("row",
                                                              new XAttribute("ref", "R" + (++rowCount).ToString()),
                                                                new XElement("attr",
                                                                    new XAttribute("name", objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME ? map.AttributeRealName : (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME ? map.AttributeDisplayName : map.AttributeId.ToString())),
                                                                    new XAttribute("parentET", objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME ? map.ParentRealName : (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME ? map.ParentDisplayName : map.ParentId.ToString())),
                                                                    new XAttribute("parentAttr", objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME ? map.ParentAttributeRealName : (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME ? map.ParentAttributeDisplayName : map.ParentAttributeId.ToString()))
                                                               )
                                                               )
                                                     );


                result = GetData(entityTypeXML.ToString(), mappingXML.ToString(), (int)objRMLookupDataMassageInfo.InputType, objRMLookupDataMassageInfo.GetDisplayNames, objRMLookupDataMassageInfo.GetWorkflowData, objRMLookupDataMassageInfo.GetDraftData, mDBCon);

                //Start Massaging Logic
                int tabCount = 0;
                if (result != null)
                {
                    lstLookupAttributeInfo.ForEach(attr =>
                    {
                        if (result.Tables.Count > tabCount)
                        {
                            DataTable output = result.Tables[tabCount];
                            string joinCol = "entity_code";
                            string assignCol = "entity_code";

                            if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME)
                                joinCol = attr.ParentAttributeRealName;
                            else if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME)
                                joinCol = attr.ParentAttributeDisplayName;
                            else if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.ID)
                                joinCol = attr.ParentAttributeId.ToString();

                            attr.MappedColumns.ForEach(map =>
                            {
                                var assign = from ip in dtInput.Where(i => !string.IsNullOrEmpty(Convert.ToString(i[map])))
                                             join op in output.AsEnumerable()
                                             on ip[map] equals op[joinCol]
                                             //where !string.IsNullOrEmpty(Convert.ToString(ip[map]))
                                             select AssignValues(ip, op, map, assignCol);

                                assign.Count();
                            });
                        }

                        tabCount++;

                    });
                }
                //End Massaging Logic

            }
            catch (Exception ex)
            {
                mLogger.Error("RMLookupDataMassage -> ConvertLookupEntityCodesToValues -> Error: " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMLookupDataMassage -> ConvertLookupEntityCodesToValues -> End");
            }
        }

        private void ConvertLookupEntityCodesToValuesForArchive(IEnumerable<DataRow> dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lstLookupAttributeInfo, RDBConnectionManager mDBCon)
        {
        }

        private void ConvertLookupValuesToEntityCodesForArchive(IEnumerable<DataRow> dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lstLookupAttributeInfo, RDBConnectionManager mDBCon)
        {
        }

        private void ConvertLookupEntityCodesToValuesForAuditView(IEnumerable<DataRow> dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lstLookupAttributeInfo, RDBConnectionManager mDBCon)
        {
        }

        private void ConvertLookupValuesToEntityCodesForAuditView(IEnumerable<DataRow> dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lstLookupAttributeInfo, RDBConnectionManager mDBCon)
        {
        }

        private void ConvertLookupEntityCodesToValuesForAuditViewForArchive(IEnumerable<DataRow> dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lstLookupAttributeInfo, bool requireDayBestCopy, RDBConnectionManager mDBCon)
        {
            mLogger.Debug("RMLookupDataMassage -> ConvertLookupEntityCodesToValuesForAuditViewForArchive -> Start");
            DataSet result = null;            
            string longDateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + " " + System.Text.RegularExpressions.Regex.Replace(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern, "(:ss|:s)", "$1.fff");
            List<RMAttributeInfoPerEntityType> lstEntityTypeInfo = new List<RMAttributeInfoPerEntityType>();

            try
            {
                //List<RMLookupAttributeInfo> lstLookupAttributeInfo = new List<RMLookupAttributeInfo>() { lookupAttributeInfo };

                lstLookupAttributeInfo.ForEach(lst =>
                {
                    //List<object> values = new List<object>();
                    List<RMLookupEntityCodesAndDates> lookupValues = new List<RMLookupEntityCodesAndDates>();
                    RMLookupEntityCodesAndDates rowData = null;

                    RMAttributeInfoPerEntityType objEntityTypeInfo = new RMAttributeInfoPerEntityType();

                    if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME)
                        objEntityTypeInfo.EntityType = lst.ParentRealName;
                    else if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME)
                        objEntityTypeInfo.EntityType = lst.ParentDisplayName;
                    else if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.ID)
                        objEntityTypeInfo.EntityType = lst.ParentId.ToString();

                    string fetchCol = lst.ParentAttributeRealName;

                    if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME)
                    {
                        fetchCol = lst.ParentDisplayName;
                    }
                    if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.ID)
                    {
                        fetchCol = lst.ParentAttributeId.ToString();
                    }

                    objEntityTypeInfo.AttributesToFetch = new HashSet<string>() { fetchCol };
                    //objEntityTypeInfo.AttributesToFetch = new HashSet<string>() { lst.ParentAttributeRealName };

                    if (dtInput != null && dtInput.Count() > 0)
                    {
                        lst.MappedColumns.ForEach(map =>
                        {
                            dtInput.Where(i => !string.IsNullOrEmpty(Convert.ToString(i[map]))).ToList().ForEach(x =>
                            {

                                rowData = new RMLookupEntityCodesAndDates();
                                rowData.Value = x[map];
                                rowData.EffectiveDate = string.IsNullOrEmpty(Convert.ToString(x[lst.EffectiveDateColumn])) ? Convert.ToDateTime(objRMLookupDataMassageInfo.DefaultDateForEffectiveDateColumn.ToString(longDateFormat)) : Convert.ToDateTime(x[lst.EffectiveDateColumn]);
                                rowData.KnowledgeDate = Convert.ToDateTime(x[lst.KnowledgeDateColumn]);
                                lookupValues.Add(rowData);
                            });

                        });
                    }

                    //lookupValues = lookupValues.Distinct().ToList();
                    lookupValues = lookupValues.DistinctBy(l => new { l.Value, l.EffectiveDate, l.KnowledgeDate }).ToList<RMLookupEntityCodesAndDates>();

                    objEntityTypeInfo.SearchColumn = "entity_code";
                    objEntityTypeInfo.ArchiveData = lookupValues;
                    lstEntityTypeInfo.Add(objEntityTypeInfo);
                });

                int rowCount = 0;
                XElement entityTypeXML = new XElement("root", from item in lstEntityTypeInfo.Where(x => x != null)
                                                              select new XElement("row",
                                                                 new XAttribute("ref", "R" + (++rowCount).ToString()),
                                                                 new XAttribute("entity_type_id", item.EntityType),
                                                                 new XAttribute("colsToFetch", string.Join(",", item.AttributesToFetch)),
                                                                 new XAttribute("searchColumn", item.SearchColumn),
                                                                from val in item.ArchiveData
                                                                select
                                                              new XElement("value",
                                                              new XAttribute("val", val.Value),
                                                              new XAttribute("ed", val.EffectiveDate.ToString("yyyy-MM-dd HH:mm:ss")),
                                                              new XAttribute("kd", val.KnowledgeDate.ToString("yyyy-MM-dd HH:mm:ss"))
                                                              )
                                                     ));

                rowCount = 0;

                XElement mappingXML = new XElement("root", from map in lstLookupAttributeInfo.Where(x => false && x != null && (x.ParentDisplayName != null || x.ParentId > 0 || x.ParentRealName != null))
                                                           select new XElement("row",
                                                              new XAttribute("ref", "R" + (++rowCount).ToString()),
                                                                new XElement("attr",
                                                                   new XAttribute("name", objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME ? map.AttributeRealName : (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME ? map.AttributeDisplayName : map.AttributeId.ToString())),
                                                                    new XAttribute("parentET", objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME ? map.ParentRealName : (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME ? map.ParentDisplayName : map.ParentId.ToString())),
                                                                    new XAttribute("parentAttr", objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME ? map.ParentAttributeRealName : (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME ? map.ParentAttributeDisplayName : map.ParentAttributeId.ToString()))
                                                               )
                                                               )
                                                     );


                mLogger.Debug("Before GetArchiveData");
                result = GetArchiveData(entityTypeXML.ToString(), mappingXML.ToString(), (int)objRMLookupDataMassageInfo.InputType, objRMLookupDataMassageInfo.GetDisplayNames, objRMLookupDataMassageInfo.GetWorkflowData, objRMLookupDataMassageInfo.GetDraftData, false, requireDayBestCopy, mDBCon);
                mLogger.Debug("After GetArchiveData");


                //Start Massaging Logic
                int tabCount = 0;
                if (result != null)
                {
                    lstLookupAttributeInfo.ForEach(attr =>
                    {
                        if (result.Tables.Count > tabCount)
                        {
                            DataTable output = result.Tables[tabCount];
                            string joinCol = "entity_code";
                            string assignCol = "entity_code";

                            if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME)
                                assignCol = attr.ParentAttributeRealName;
                            else if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME)
                                assignCol = attr.ParentAttributeDisplayName;
                            else if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.ID)
                                assignCol = attr.ParentAttributeId.ToString();

                            mLogger.Debug("Dictionary Start for attributes : " + attr);
                            Dictionary<string, Dictionary<string, Dictionary<string, DataRow>>> outputDataDic = new Dictionary<string, Dictionary<string, Dictionary<string, DataRow>>>();
                            foreach (DataRow dr in output.Rows)
                            {
                                Dictionary<string, Dictionary<string, DataRow>> innerDictionary = new Dictionary<string, Dictionary<string, DataRow>>();
                                Dictionary<string, DataRow> innerMostDictionary = new Dictionary<string, DataRow>();

                                if (!outputDataDic.TryGetValue(Convert.ToString(dr[joinCol]), out innerDictionary))
                                {
                                    innerDictionary = new Dictionary<string, Dictionary<string, DataRow>>();
                                    outputDataDic.Add(Convert.ToString(dr[joinCol]), innerDictionary);
                                }

                                string loadingDateTimeString = Convert.ToDateTime(dr["loading_time"]).ToShortDateString();
                                if (requireDayBestCopy)
                                    loadingDateTimeString = Convert.ToDateTime(dr["loading_time"]).ToString("yyyy-MM-dd HH:mm:ss");

                                if (!innerDictionary.TryGetValue(loadingDateTimeString, out innerMostDictionary))
                                {
                                    innerMostDictionary = new Dictionary<string, DataRow>();
                                    innerDictionary.Add(loadingDateTimeString, innerMostDictionary);
                                }
                                if (!innerMostDictionary.ContainsKey(Convert.ToDateTime(dr["knowledge_date"]).ToString("yyyy-MM-dd HH:mm:ss")))
                                    innerMostDictionary.Add(Convert.ToDateTime(dr["knowledge_date"]).ToString("yyyy-MM-dd HH:mm:ss"), dr);
                            }
                            mLogger.Debug("Dictionary End for attributes : " + attr);

                            mLogger.Debug("Massaging Start for attributes : " + attr);

                            attr.MappedColumns.ForEach(map =>
                            {
                                mLogger.Debug("Massaging Start for mapped column  : " + map);

                                Dictionary<string, Dictionary<string, DataRow>> innerDictionary = new Dictionary<string, Dictionary<string, DataRow>>();
                                Dictionary<string, DataRow> innerMostDictionary = new Dictionary<string, DataRow>();
                                DataRow op = null;
                                foreach (DataRow ip in dtInput)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(ip[map])))
                                    {
                                        if (outputDataDic.TryGetValue(Convert.ToString(ip[map]), out innerDictionary))
                                        {
                                            if (string.IsNullOrEmpty(Convert.ToString(ip[attr.EffectiveDateColumn])))
                                            {
                                                string loadingDateTimeString = objRMLookupDataMassageInfo.DefaultDateForEffectiveDateColumn.ToShortDateString();
                                                if (requireDayBestCopy)
                                                    loadingDateTimeString = objRMLookupDataMassageInfo.DefaultDateForEffectiveDateColumn.ToString("yyyy-MM-dd HH:mm:ss");

                                                if (innerDictionary.TryGetValue(loadingDateTimeString, out innerMostDictionary))
                                                {
                                                    if (innerMostDictionary.TryGetValue(Convert.ToDateTime(ip[attr.KnowledgeDateColumn]).ToString("yyyy-MM-dd HH:mm:ss"), out op))
                                                    {
                                                        ip[map] = op[assignCol];
                                                    }
                                                    else
                                                    {
                                                        ip[map] = DBNull.Value;
                                                        mLogger.Debug("Knowledge date not found in the inner most dictionary for the entity code : " + Convert.ToString(ip[map]) + " with loading_time : " + Convert.ToDateTime(ip[attr.EffectiveDateColumn]).ToShortDateString() + " and knowledge_date : " + Convert.ToDateTime(ip[attr.KnowledgeDateColumn]).ToString("yyyy-MM-dd HH:mm:ss") + "  from the list of : " + string.Join(",", innerMostDictionary.Keys));
                                                    }
                                                }
                                                else
                                                {
                                                    ip[map] = DBNull.Value;
                                                    mLogger.Debug("Loading time not found in the inner dictionary for the entity code : " + Convert.ToString(ip[map]) + " with loading_time : " + objRMLookupDataMassageInfo.DefaultDateForEffectiveDateColumn.ToShortDateString() + "  from the list of : " + string.Join(",", innerDictionary.Keys));
                                                }
                                            }
                                            else
                                            {
                                                string loadingDateTimeString = Convert.ToDateTime(ip[attr.EffectiveDateColumn]).ToShortDateString();
                                                if (requireDayBestCopy)
                                                    loadingDateTimeString = Convert.ToDateTime(ip[attr.EffectiveDateColumn]).ToString("yyyy-MM-dd HH:mm:ss");

                                                if (innerDictionary.TryGetValue(loadingDateTimeString, out innerMostDictionary))
                                                {
                                                    if (innerMostDictionary.TryGetValue(Convert.ToDateTime(ip[attr.KnowledgeDateColumn]).ToString("yyyy-MM-dd HH:mm:ss"), out op))
                                                    {
                                                        ip[map] = op[assignCol];
                                                    }
                                                    else
                                                    {
                                                        ip[map] = DBNull.Value;
                                                        mLogger.Debug("Knowledge date not found in the inner most dictionary for the entity code : " + Convert.ToString(ip[map]) + " with loading_time : " + Convert.ToDateTime(ip[attr.EffectiveDateColumn]).ToShortDateString() + " and knowledge_date : " + Convert.ToDateTime(ip[attr.KnowledgeDateColumn]).ToString("yyyy-MM-dd HH:mm:ss") + "  from the list of : " + string.Join(",", innerMostDictionary.Keys));
                                                    }
                                                }
                                                else
                                                {
                                                    ip[map] = DBNull.Value;
                                                    mLogger.Debug("Loading time not found in the inner dictionary for the entity code : " + Convert.ToString(ip[map]) + " with loading_time : " + Convert.ToDateTime(ip[attr.EffectiveDateColumn]).ToShortDateString() + "  from the list of : " + string.Join(",", innerDictionary.Keys));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ip[map] = DBNull.Value;
                                            mLogger.Debug("Entity Code not found in the output dictionary : " + Convert.ToString(ip[map]) + " from the list of : " + string.Join(",", outputDataDic.Keys));
                                        }
                                    }
                                }
                                mLogger.Debug("Massaging End for mapped column  : " + map);
                            });

                            //attr.MappedColumns.ForEach(map =>
                            //{
                            //    var assign = from ip in dtInput.Where(i => !string.IsNullOrEmpty(Convert.ToString(i[map])))
                            //                 join op in output.AsEnumerable()
                            //                 on ip[map] equals op[joinCol]
                            //                 where Convert.ToDateTime(ip[attr.EffectiveDateColumn]).ToShortDateString() == Convert.ToDateTime(op["loading_time"]).ToShortDateString()
                            //                 && Convert.ToDateTime(ip[attr.KnowledgeDateColumn]).ToString("yyyy-MM-dd HH:mm:ss") == Convert.ToDateTime(op["knowledge_date"]).ToString("yyyy-MM-dd HH:mm:ss")
                            //                 select AssignValues(ip, op, map, assignCol);

                            //    assign.Count();
                            //});
                        }

                        tabCount++;

                        mLogger.Debug("Massaging End for attributes : " + attr);

                    });
                }
                //End Massaging Logic

            }
            catch (Exception ex)
            {
                mLogger.Error("RMLookupDataMassage -> ConvertLookupEntityCodesToValuesForAuditViewForArchive -> Error: " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMLookupDataMassage -> ConvertLookupEntityCodesToValuesForAuditViewForArchive -> End");
            }

        }

        private void ConvertLookupValuesToEntityCodesForAuditViewForArchive(IEnumerable<DataRow> dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lstLookupAttributeInfo, RDBConnectionManager mDBCon)
        {
        }

        internal DataSet GetData(string entityTypeXML, string mappingXML, int inputType, bool getDisplayNames, bool getWorkflowData, bool getDraftData, RDBConnectionManager mDBCon)
        {
            DataSet ds = null;
            string query = "EXEC REFM_GetMassagedData '" + entityTypeXML.Replace("'", "''") + "', '" + mappingXML.Replace("'", "''") + "', " + inputType +
                ", " + getDisplayNames + ", " + getWorkflowData + ", " + getDraftData + " ";

            if (mDBCon != null)
                ds = CommonDALWrapper.ExecuteSelectQuery(query, mDBCon);
            else
                ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);

            return ds;
        }

        internal DataSet GetArchiveData(string entityTypeXML, string mappingXML, int inputType, bool getDisplayNames, bool getWorkflowData, bool getDraftData, bool requireAuditDatesAsString, bool requireDayBestCopy, RDBConnectionManager mDBCon)
        {
            DataSet ds = null;
            string query = "EXEC REFM_GetMassagedDataForArchive '" + entityTypeXML.Replace("'", "''") + "', '" + mappingXML.Replace("'", "''") + "', " + inputType + ", " + getDisplayNames + ", " + getWorkflowData + ", " + getDraftData + ", " + requireAuditDatesAsString + ", " + requireDayBestCopy + " ";

            if (mDBCon != null)
                ds = CommonDALWrapper.ExecuteSelectQuery(query, mDBCon);
            else
                ds = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);

            return ds;
        }

        private bool AssignValues(DataRow inputRow, DataRow outputRow, string assignToColumn, string assignFromColumn)
        {
            inputRow[assignToColumn] = outputRow[assignFromColumn];
            return true;
        }

        public DataSet GetMassageLookupAttributeInfo(string entityTypeName, string attributeName, bool isDisplayName, RDBConnectionManager mDBCon = null)
        {

            mLogger.Error("RMLookupDataMassage -> GetMassageLookupAttributeInfo-> start");
            DataSet ds = new DataSet();

            try
            {
                if (mDBCon == null)
                    ds = CommonDALWrapper.ExecuteSelectQuery("REFM_GetMassagedData '<root><row ref= \"R1\" entity_type_id=\"" + entityTypeName + "\" colsToFetch=\"" + attributeName + "\" searchColumn=\"entity_code\"></row></root>','<root></root>',0," + isDisplayName, ConnectionConstants.RefMaster_Connection);
                else
                    ds = CommonDALWrapper.ExecuteSelectQuery("REFM_GetMassagedData '<root><row ref= \"R1\" entity_type_id=\"" + entityTypeName + "\" colsToFetch=\"" + attributeName + "\" searchColumn=\"entity_code\"></row></root>','<root></root>',0," + isDisplayName, mDBCon);
            }
            catch (Exception ex)
            {
                mLogger.Error("RMLookupDataMassage -> GetMassageLookupAttributeInfo-> error " + ex.Message);
            }
            finally
            {
                mLogger.Error("RMLookupDataMassage -> GetMassageLookupAttributeInfo-> end");
            }
            return ds;
        }

        #region Get Lookup Data Sets

        public DataSet GetLookupData(DataTable dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lstLookupAttributeInfo, RDBConnectionManager mDBCon = null, string excludePrefix = "")
        {
            mLogger.Debug("RMLookupDataMassage -> GetLookupData -> Start");
            DataSet ds = null;
            try
            {
                if (!objRMLookupDataMassageInfo.IsArchive && !objRMLookupDataMassageInfo.IsAuditView && !objRMLookupDataMassageInfo.IsEntityCodeToValues)
                    ds = GetLookupDataEntityCodesFromValues(dtInput.AsEnumerable(), objRMLookupDataMassageInfo, lstLookupAttributeInfo, excludePrefix, mDBCon);

                return ds;
            }
            catch (Exception ex)
            {
                mLogger.Error("RMLookupDataMassage -> GetLookupData -> Error: " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMLookupDataMassage -> GetLookupData -> End");
            }
        }

        public DataSet GetLookupData(IEnumerable<DataRow> dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lstLookupAttributeInfo, RDBConnectionManager mDBCon = null, string excludePrefix = "")
        {
            mLogger.Debug("RMLookupDataMassage -> GetLookupData -> Start");
            DataSet ds = null;
            try
            {
                if (!objRMLookupDataMassageInfo.IsArchive && !objRMLookupDataMassageInfo.IsAuditView && !objRMLookupDataMassageInfo.IsEntityCodeToValues)
                    ds = GetLookupDataEntityCodesFromValues(dtInput, objRMLookupDataMassageInfo, lstLookupAttributeInfo, excludePrefix, mDBCon);

                return ds;
            }
            catch (Exception ex)
            {
                mLogger.Error("RMLookupDataMassage -> GetLookupData -> Error: " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMLookupDataMassage -> GetLookupData -> End");
            }
        }

        public DataTable GetLookupDataForValues(RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lstLookupAttributeInfo, List<object> valuesToSearch, RDBConnectionManager mDBCon = null, string excludePrefix = "")
        {
            mLogger.Debug("RMLookupDataMassage -> GetLookupData -> Start");
            List<RMAttributeInfoPerEntityType> lstEntityTypeInfo = new List<RMAttributeInfoPerEntityType>();
            DataTable dt = null;
            try
            {

                lstLookupAttributeInfo.ForEach(lst =>
                {
                    List<object> values = new List<object>();
                    RMAttributeInfoPerEntityType objEntityTypeInfo = new RMAttributeInfoPerEntityType();
                    if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME)
                        objEntityTypeInfo.EntityType = lst.ParentRealName;
                    else if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME)
                        objEntityTypeInfo.EntityType = lst.ParentDisplayName;
                    else if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.ID)
                        objEntityTypeInfo.EntityType = lst.ParentId.ToString();

                    string fetchCol = lst.ParentAttributeRealName;
                    string searchCol = lst.ParentAttributeRealName;

                    if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME)
                    {
                        fetchCol = lst.ParentAttributeDisplayName;
                        searchCol = lst.ParentAttributeDisplayName;
                    }
                    if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.ID)
                    {
                        fetchCol = lst.ParentAttributeId.ToString();
                        searchCol = lst.ParentAttributeId.ToString();
                    }

                    objEntityTypeInfo.AttributesToFetch = new HashSet<string>() { fetchCol };

                    objEntityTypeInfo.SearchColumn = searchCol;
                    if (valuesToSearch != null && valuesToSearch.Count > 0)
                        objEntityTypeInfo.ValuesToSearch = valuesToSearch.SRMToHashSet();
                    lstEntityTypeInfo.Add(objEntityTypeInfo);
                });

                int rowCount = 0;
                XElement entityTypeXML = new XElement("root", from item in lstEntityTypeInfo.Where(x => x != null)
                                                              select new XElement("row",
                                                                 new XAttribute("ref", "R" + (++rowCount).ToString()),
                                                                 new XAttribute("entity_type_id", item.EntityType),
                                                                 new XAttribute("colsToFetch", string.Join(",", item.AttributesToFetch)),
                                                                 new XAttribute("searchColumn", item.SearchColumn),
                                                                from val in item.ValuesToSearch
                                                                select
                                                              new XElement("value", val)
                                                     ));

                rowCount = 0;

                XElement mappingXML = new XElement("root", from map in lstLookupAttributeInfo.Where(x => false && x != null && (x.ParentDisplayName != null || x.ParentId > 0 || x.ParentRealName != null))
                                                           select new XElement("row",
                                                              new XAttribute("ref", "R" + (++rowCount).ToString()),
                                                                new XElement("attr",
                                                                    new XAttribute("name", objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME ? map.AttributeRealName : (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME ? map.AttributeDisplayName : map.AttributeId.ToString())),
                                                                    new XAttribute("parentET", objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME ? map.ParentRealName : (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME ? map.ParentDisplayName : map.ParentId.ToString())),
                                                                    new XAttribute("parentAttr", objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME ? map.ParentAttributeRealName : (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME ? map.ParentAttributeDisplayName : map.ParentAttributeId.ToString()))
                                                               )
                                                               )
                                                     );


                DataSet result = GetData(entityTypeXML.ToString(), mappingXML.ToString(), (int)objRMLookupDataMassageInfo.InputType, objRMLookupDataMassageInfo.GetDisplayNames, objRMLookupDataMassageInfo.GetWorkflowData, objRMLookupDataMassageInfo.GetDraftData, mDBCon);

                if (result != null && result.Tables.Count > 0)
                    dt = result.Tables[0];


                return dt;
            }
            catch (Exception ex)
            {
                mLogger.Error("RMLookupDataMassage -> GetLookupData -> Error: " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMLookupDataMassage -> GetLookupData -> End");
            }
        }

        private DataSet GetLookupDataEntityCodesFromValues(IEnumerable<DataRow> dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lstLookupAttributeInfo, string excludePrefix, RDBConnectionManager mDBCon = null)
        {
            mLogger.Debug("RMLookupDataMassage -> GetLookupDataEntityCodesFromValues -> Start");
            DataSet result = null;
            List<RMAttributeInfoPerEntityType> lstEntityTypeInfo = new List<RMAttributeInfoPerEntityType>();
            try
            {

                lstLookupAttributeInfo.ForEach(lst =>
                {
                    List<object> values = new List<object>();
                    RMAttributeInfoPerEntityType objEntityTypeInfo = new RMAttributeInfoPerEntityType();
                    if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME)
                        objEntityTypeInfo.EntityType = lst.ParentRealName;
                    else if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME)
                        objEntityTypeInfo.EntityType = lst.ParentDisplayName;
                    else if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.ID)
                        objEntityTypeInfo.EntityType = lst.ParentId.ToString();

                    string fetchCol = lst.ParentAttributeRealName;
                    string searchCol = lst.ParentAttributeRealName;

                    if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME)
                    {
                        fetchCol = lst.ParentAttributeDisplayName;
                        searchCol = lst.ParentAttributeDisplayName;
                    }
                    if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.ID)
                    {
                        fetchCol = lst.ParentAttributeId.ToString();
                        searchCol = lst.ParentAttributeId.ToString();
                    }

                    objEntityTypeInfo.AttributesToFetch = new HashSet<string>() { fetchCol };

                    if (dtInput != null && dtInput.Count() > 0)
                    {
                        if (!string.IsNullOrEmpty(excludePrefix))
                        {
                            values.AddRange(dtInput.Where(i => !string.IsNullOrEmpty(Convert.ToString(i[lst.MappedColumns[0]])) && !Convert.ToString(i[lst.MappedColumns[0]]).StartsWith(excludePrefix)).Select(inp => inp[lst.MappedColumns[0]]).Distinct().ToList<object>());
                        }
                        else
                        {
                            values.AddRange(dtInput.Where(i => !string.IsNullOrEmpty(Convert.ToString(i[lst.MappedColumns[0]]))).Select(inp => inp[lst.MappedColumns[0]]).Distinct().ToList<object>());
                        }
                    }

                    objEntityTypeInfo.SearchColumn = searchCol;
                    objEntityTypeInfo.ValuesToSearch = values.SRMToHashSet();
                    lstEntityTypeInfo.Add(objEntityTypeInfo);
                });

                int rowCount = 0;
                XElement entityTypeXML = new XElement("root", from item in lstEntityTypeInfo.Where(x => x != null)
                                                              select new XElement("row",
                                                                 new XAttribute("ref", "R" + (++rowCount).ToString()),
                                                                 new XAttribute("entity_type_id", item.EntityType),
                                                                 new XAttribute("colsToFetch", string.Join(",", item.AttributesToFetch)),
                                                                 new XAttribute("searchColumn", item.SearchColumn),
                                                                from val in item.ValuesToSearch
                                                                select
                                                              new XElement("value", val)
                                                     ));

                rowCount = 0;

                XElement mappingXML = new XElement("root", from map in lstLookupAttributeInfo.Where(x => false && x != null && (x.ParentDisplayName != null || x.ParentId > 0 || x.ParentRealName != null))
                                                           select new XElement("row",
                                                              new XAttribute("ref", "R" + (++rowCount).ToString()),
                                                                new XElement("attr",
                                                                    new XAttribute("name", objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME ? map.AttributeRealName : (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME ? map.AttributeDisplayName : map.AttributeId.ToString())),
                                                                    new XAttribute("parentET", objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME ? map.ParentRealName : (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME ? map.ParentDisplayName : map.ParentId.ToString())),
                                                                    new XAttribute("parentAttr", objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME ? map.ParentAttributeRealName : (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME ? map.ParentAttributeDisplayName : map.ParentAttributeId.ToString()))
                                                               )
                                                               )
                                                     );


                result = GetData(entityTypeXML.ToString(), mappingXML.ToString(), (int)objRMLookupDataMassageInfo.InputType, objRMLookupDataMassageInfo.GetDisplayNames, objRMLookupDataMassageInfo.GetWorkflowData, objRMLookupDataMassageInfo.GetDraftData, mDBCon);

                return result;
            }
            catch (Exception ex)
            {
                mLogger.Error("RMLookupDataMassage -> GetLookupDataEntityCodesFromValues -> Error: " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMLookupDataMassage -> GetLookupDataEntityCodesFromValues -> End");
            }
        }

        public DataSet GetLookupDataArchive(IEnumerable<DataRow> dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lstLookupAttributeInfo, RDBConnectionManager mDBCon = null, int attributeIndex = -1, string separator = null)
        {
            mLogger.Debug("RMLookupDataMassage -> ConvertLookupEntityCodesToValuesForAuditViewForArchive -> Start");
            DataSet result = null;
            List<RMAttributeInfoPerEntityType> lstEntityTypeInfo = new List<RMAttributeInfoPerEntityType>();

            try
            {
                lstLookupAttributeInfo.ForEach(lst =>
                {
                    List<RMLookupEntityCodesAndDates> lookupValues = new List<RMLookupEntityCodesAndDates>();
                    RMLookupEntityCodesAndDates rowData = null;

                    RMAttributeInfoPerEntityType objEntityTypeInfo = new RMAttributeInfoPerEntityType();

                    if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME)
                        objEntityTypeInfo.EntityType = lst.ParentRealName;
                    else if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME)
                        objEntityTypeInfo.EntityType = lst.ParentDisplayName;
                    else if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.ID)
                        objEntityTypeInfo.EntityType = lst.ParentId.ToString();

                    string fetchCol = lst.ParentAttributeRealName;

                    if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME)
                    {
                        fetchCol = lst.ParentDisplayName;
                    }
                    if (objRMLookupDataMassageInfo.InputType == RMLookupInputType.ID)
                    {
                        fetchCol = lst.ParentAttributeId.ToString();
                    }

                    objEntityTypeInfo.AttributesToFetch = new HashSet<string>() { fetchCol };

                    if (dtInput != null && dtInput.Count() > 0)
                    {
                        lst.MappedColumns.ForEach(map =>
                        {
                            dtInput.Where(i => !string.IsNullOrEmpty(Convert.ToString(i[map]))).ToList().ForEach(x =>
                            {
                                object val = x[map];
                                rowData = new RMLookupEntityCodesAndDates();
                                if (!string.IsNullOrEmpty(separator) && attributeIndex >= 0 && Convert.ToString(val).Split(new string[] { separator }, StringSplitOptions.None).Length >= attributeIndex)
                                {
                                    val = Convert.ToString(val).Split(new string[] { separator }, StringSplitOptions.None)[attributeIndex];
                                }

                                rowData.Value = val;
                                rowData.EffectiveDate = Convert.ToDateTime(x[lst.EffectiveDateColumn]);
                                rowData.KnowledgeDate = Convert.ToDateTime(x[lst.KnowledgeDateColumn]);
                                lookupValues.Add(rowData);
                            });

                        });
                    }

                    lookupValues = lookupValues.Where(x => !string.IsNullOrEmpty(Convert.ToString(x.Value))).DistinctBy(l => new { l.Value, l.EffectiveDate, l.KnowledgeDate }).ToList<RMLookupEntityCodesAndDates>();

                    objEntityTypeInfo.SearchColumn = "entity_code";
                    objEntityTypeInfo.ArchiveData = lookupValues;
                    lstEntityTypeInfo.Add(objEntityTypeInfo);
                });

                int rowCount = 0;
                XElement entityTypeXML = new XElement("root", from item in lstEntityTypeInfo.Where(x => x != null)
                                                              select new XElement("row",
                                                                 new XAttribute("ref", "R" + (++rowCount).ToString()),
                                                                 new XAttribute("entity_type_id", item.EntityType),
                                                                 new XAttribute("colsToFetch", string.Join(",", item.AttributesToFetch)),
                                                                 new XAttribute("searchColumn", item.SearchColumn),
                                                                from val in item.ArchiveData
                                                                select
                                                              new XElement("value",
                                                              new XAttribute("val", val.Value),
                                                              new XAttribute("ed", val.EffectiveDate),
                                                              new XAttribute("kd", val.KnowledgeDate)
                                                              )
                                                     ));

                rowCount = 0;

                XElement mappingXML = new XElement("root", from map in lstLookupAttributeInfo.Where(x => false && x != null && (x.ParentDisplayName != null || x.ParentId > 0 || x.ParentRealName != null))
                                                           select new XElement("row",
                                                              new XAttribute("ref", "R" + (++rowCount).ToString()),
                                                                new XElement("attr",
                                                                   new XAttribute("name", objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME ? map.AttributeRealName : (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME ? map.AttributeDisplayName : map.AttributeId.ToString())),
                                                                    new XAttribute("parentET", objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME ? map.ParentRealName : (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME ? map.ParentDisplayName : map.ParentId.ToString())),
                                                                    new XAttribute("parentAttr", objRMLookupDataMassageInfo.InputType == RMLookupInputType.REALNAME ? map.ParentAttributeRealName : (objRMLookupDataMassageInfo.InputType == RMLookupInputType.DISPLAYNAME ? map.ParentAttributeDisplayName : map.ParentAttributeId.ToString()))
                                                               )
                                                               )
                                                     );


                result = GetArchiveData(entityTypeXML.ToString(), mappingXML.ToString(), (int)objRMLookupDataMassageInfo.InputType, objRMLookupDataMassageInfo.GetDisplayNames, objRMLookupDataMassageInfo.GetWorkflowData, objRMLookupDataMassageInfo.GetDraftData, false, true, mDBCon);

                return result;

            }
            catch (Exception ex)
            {
                mLogger.Error("RMLookupDataMassage -> ConvertLookupEntityCodesToValuesForAuditViewForArchive -> Error: " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMLookupDataMassage -> ConvertLookupEntityCodesToValuesForAuditViewForArchive -> End");
            }

        }

        #endregion

        #region Security Lookup Methods

        public void MassageSecurityLookup(DataTable dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lstLookupAttributeInfo)
        {
            if (objRMLookupDataMassageInfo.IsEntityCodeToValues)
            {
                ConvertSecurityIdsToValue(dtInput.AsEnumerable(), objRMLookupDataMassageInfo, lstLookupAttributeInfo);
            }
        }

        public void MassageSecurityLookup(IEnumerable<DataRow> dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lstLookupAttributeInfo)
        {
            if (objRMLookupDataMassageInfo.IsEntityCodeToValues)
            {
                ConvertSecurityIdsToValue(dtInput, objRMLookupDataMassageInfo, lstLookupAttributeInfo);
            }
        }

        public void MassageSecurityLookupForAudit(IEnumerable<DataRow> dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, RMLookupAttributeInfo lookupAttributeInfo)
        {
            if (objRMLookupDataMassageInfo.IsEntityCodeToValues)
            {
                ConvertSecurityIdsToValueForAudit(dtInput, objRMLookupDataMassageInfo, lookupAttributeInfo);
            }
        }

        private void ConvertSecurityIdsToValueForAudit(IEnumerable<DataRow> dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, RMLookupAttributeInfo lookupAttributeInfo)
        {
            mLogger.Debug("RMLookupDataMassage -> ConvertSecurityIdsToValueForAudit -> Start");
            DataSet result = new DataSet();
            List<RMAttributeInfoPerEntityType> lstEntityTypeInfo = new List<RMAttributeInfoPerEntityType>();
            const string NAValue = "Not Applicable";
            DataTable dtSec = null;

            try
            {
                //Start Get Data
                List<string> values = new List<string>();
                lookupAttributeInfo.MappedColumns.ForEach(map =>
                {
                    values.AddRange(dtInput.Where(i => !string.IsNullOrEmpty(Convert.ToString(i[map])) && Convert.ToString(i[map]) != NAValue)
                    .Select(d => Convert.ToString(d[map])).Distinct().ToList());
                });

                values = values.Distinct().ToList();
                dtSec = GetSecurityDataBySecId(lookupAttributeInfo.ParentId, lookupAttributeInfo.ParentAttributeId, values);

                if (dtSec != null)
                    result.Tables.Add(dtSec.Copy());

                //End Get Data

                //Start Massaging Logic
                if (result != null && result.Tables.Count > 0)
                {
                    DataTable output = result.Tables[0];
                    string joinCol = "security_id";
                    string assignCol = "attribute_name";

                    lookupAttributeInfo.MappedColumns.ForEach(map =>
                    {
                        var assign = from ip in dtInput.Where(i => !string.IsNullOrEmpty(Convert.ToString(i[map])) && Convert.ToString(i[map]) != NAValue)
                                     join op in output.AsEnumerable()
                                     on ip[map] equals op[joinCol]
                                     select AssignValues(ip, op, map, assignCol);

                        assign.Count();
                    });

                }
                //End Massaging Logic

            }

            catch (Exception ex)
            {
                mLogger.Error("RMLookupDataMassage -> ConvertSecurityIdsToValueForAudit -> Error: " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMLookupDataMassage -> ConvertSecurityIdsToValueForAudit -> End");
            }
        }

        private void ConvertSecurityIdsToValue(IEnumerable<DataRow> dtInput, RMLookupDataMassageInfo objRMLookupDataMassageInfo, List<RMLookupAttributeInfo> lstLookupAttributeInfo)
        {
            mLogger.Debug("RMLookupDataMassage -> ConvertSecurityIdsToValue -> Start");
            DataSet result = new DataSet();
            DataTable secTable = null;
            List<RMAttributeInfoPerEntityType> lstEntityTypeInfo = new List<RMAttributeInfoPerEntityType>();
            int count = 0;

            try
            {
                //Start Get Data
                lstLookupAttributeInfo.ForEach(lst =>
                {
                    count++;
                    List<string> values = new List<string>();
                    lst.MappedColumns.ForEach(map =>
                    {
                        values.AddRange(dtInput.Where(i => !string.IsNullOrEmpty(Convert.ToString(i[map])))
                        .Select(d => Convert.ToString(d[map])).Distinct().ToList());
                    });
                    secTable = GetSecurityDataBySecId(lst.ParentId, lst.ParentAttributeId, values);
                    if (secTable != null)
                    {
                        secTable.TableName = "SecTable_" + count.ToString();
                        result.Tables.Add(secTable.Copy());
                    }
                });
                //End Get Data

                //Start Massaging Logic
                int tabCount = 0;
                if (result != null && result.Tables.Count > 0)
                {
                    lstLookupAttributeInfo.ForEach(attr =>
                    {
                        if (result.Tables.Count > tabCount)
                        {
                            DataTable output = result.Tables[tabCount];
                            string joinCol = "security_id";
                            string assignCol = "attribute_name";

                            attr.MappedColumns.ForEach(map =>
                            {
                                var assign = from ip in dtInput.Where(i => !string.IsNullOrEmpty(Convert.ToString(i[map])))
                                             join op in output.AsEnumerable()
                                             on ip[map] equals op[joinCol]
                                             select AssignValues(ip, op, map, assignCol);

                                assign.Count();
                            });
                        }

                        tabCount++;

                    });
                }
                //End Massaging Logic

            }

            catch (Exception ex)
            {
                mLogger.Error("RMLookupDataMassage -> ConvertSecurityIdsToValue -> Error: " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMLookupDataMassage -> ConvertSecurityIdsToValue -> End");
            }
        }

        private DataTable GetSecurityDataBySecId(int secTypeId, int secAttributeID, List<string> values)
        {
            mLogger.Debug("RMLookupDataMassage -> GetSecurityDataBySecIds -> Start");
            try
            {
                string valuesToSearch = string.Join(",", values);

                DataTable dt = new DataTable();

                string query = @" 
                    Declare 
                        @attribute_name VARCHAR(MAX), 
                        @sql VARCHAR(MAX), 
                        @viewName VARCHAR(MAX) = 'ivpsecmaster.dbo.vwCommonAttributes' 

                    select @attribute_name = ''

                    SELECT TOP 1 @attribute_name = tem.display_name 
                    FROM ivp_secm_attribute_details att
                    INNER JOIN ivp_secm_template_details tem
                    on att.attribute_id = tem.attribute_id
                    WHERE att.attribute_id = " + secAttributeID + @" AND att.is_active = 1 
                    
                    select @viewName = ISNULL('ivpsecmaster.dbo.m' + view_name, @viewName)
                    from ivpsecmaster.dbo.ivp_secm_sectype_master 
                    where sectype_id = " + secTypeId + @" and is_active = 1 and is_complete = 1 
                   

                    select @sql = ' select [Security Id] security_id, [' + @attribute_name + '] attribute_name 
                            from ' + @viewName + '  
                            where [Security Id] in (SELECT distinct item from ivprefmaster.dbo.REFM_GetList2Table(''" + valuesToSearch + @"'','','')) 
                            order by attribute_name ' 

                    exec (@sql) ";

                DataSet secValuesDs = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.SecMaster_Connection);

                if (secValuesDs != null && secValuesDs.Tables.Count > 0)
                    dt = secValuesDs.Tables[0];

                return dt;
            }
            catch (Exception ex)
            {
                mLogger.Error("RMLookupDataMassage -> GetSecurityDataBySecIds -> Error: " + ex.Message);
                throw ex;
            }
            finally
            {
                mLogger.Debug("RMLookupDataMassage -> GetSecurityDataBySecIds -> Start");
            }
        }

        #endregion

        #endregion
    }

}
