using com.ivp.commom.commondal;
using com.ivp.rad.BusinessCalendar;
using com.ivp.rad.common;
using com.ivp.rad.dal;
using com.ivp.rad.data;
using com.ivp.rad.utils;
using com.ivp.srmcommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.ivp.common.srmdownstreamcontroller
{
    public class SRMDownstreamController
    {
        private static IRLogger mLogger = RLogFactory.CreateLogger("SRMDownstreamController");

        public static Dictionary<string, string> GetReportAttributesSchema(int reportId, bool isRefM, out HashSet<string> hiddenAttributes)
        {
            string query = null;
            hiddenAttributes = new HashSet<string>();
            HashSet<string> invalidLengthAttributes = new HashSet<string>();
            string RefSecLookupAttributesWithSecId = SRMCommon.GetConfigFromDB("IgnoreSecurityLookupMassagingInRefReport");
            HashSet<int> attributesWithSecId = new HashSet<int>();
            if (!string.IsNullOrEmpty(RefSecLookupAttributesWithSecId))
            {
                attributesWithSecId = new HashSet<int>(RefSecLookupAttributesWithSecId.Split(',').Select(int.Parse));
            }
            Dictionary<int, string> secLookupsAttributeVsDataType = new Dictionary<int, string>();

            Dictionary<string, string> reportAttributeVsDatatypeAndDecimalPlaces = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (isRefM)
            {
                query = String.Format(@"DECLARE @reportId INT = {0}
                        DECLARE @attributesToShow TABLE(id INT,report_attribute_id INT, decimal_places INT)

                        INSERT INTO @attributesToShow
                        SELECT tab.*,null
                        FROM IVPRefMaster.dbo.ivp_refm_report_configuration rc
                        CROSS APPLY IVPRefMaster.dbo.RefM_GetList2TableWithRowIDWithoutTruncation(rc.attribute_to_show,'|') tab
                        WHERE rc.report_id = @reportId AND  rc.is_active = 1

                        UPDATE ats
                        SET ats.decimal_places = CAST(tab2.item AS INT)
                        FROM @attributesToShow ats
                        INNER JOIN (SELECT tab.id, tab.item
			                        FROM IVPRefMaster.dbo.ivp_refm_report_configuration rc
			                        CROSS APPLY IVPRefMaster.dbo.RefM_GetList2TableWithRowIDWithoutTruncation(rc.decimal_places,'|') tab
			                        WHERE rc.report_id = @reportId AND rc.is_active = 1
			                        ) tab2 ON ats.id = tab2.id

                        SELECT
                        ra.report_attribute_id,
                        ra.report_attribute_name ,
                        ra.attribute_data_type as report_attribute_data_type,
                        ats.decimal_places as report_decimal_places,
                        CASE WHEN eat.data_type = 'VARCHAR' 
								THEN 'VARCHAR(' + eat.data_type_length + ')'
	                        WHEN eat.data_type IN ('VARCHARMAX','FILE')
								THEN 'VARCHAR(MAX)'
	                        WHEN eat.data_type IN ('INT','DATETIME', 'BIT')
								THEN eat.data_type
	                        WHEN eat.data_type = 'DECIMAL'
								THEN 'DECIMAL(' + eat.data_type_length + ')'
							WHEN eat.data_type IN ('LOOKUP', 'SECURITY_LOOKUP') AND ISNULL(amap.lookup_attribute_id,0) = 0
								THEN 'VARCHAR(15)'
							WHEN eat.data_type IN ('LOOKUP', 'SECURITY_LOOKUP') AND ISNULL(amap.lookup_attribute_id,0) != 0
								THEN 								
									CASE WHEN eat2.data_type = 'VARCHAR' 
										THEN 'VARCHAR(' + eat2.data_type_length + ')'
									WHEN eat2.data_type IN ('VARCHARMAX','FILE')
										THEN 'VARCHAR(MAX)'
									WHEN eat2.data_type IN ('INT','DATETIME', 'BIT')
										THEN eat2.data_type
									WHEN eat2.data_type = 'DECIMAL'
										THEN 'DECIMAL(' + eat2.data_type_length + ')'
									WHEN eat2.data_type IN ('LOOKUP', 'SECURITY_LOOKUP')
										THEN 'VARCHAR(MAX)'
									ELSE '' END						
							ELSE ra.attribute_data_type
	                        END
                        as attribute_data_type
                        FROM IVPRefMaster.dbo.ivp_refm_report_attribute ra
                        INNER JOIN @attributesToShow ats
                        ON(ats.report_attribute_id = ra.report_attribute_id)
                        INNER JOIN IVPRefMaster.dbo.ivp_refm_report_attribute_map amap
                        ON (ra.report_attribute_id = amap.report_attribute_id AND amap.is_active = 1 )
                        LEFT JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute eat
                        ON (eat.entity_attribute_id = amap.dependent_attribute_id AND eat.is_active = 1)
                        LEFT JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute eat2
						ON (eat2.entity_attribute_id = amap.lookup_attribute_id AND eat2.is_active = 1)
                        WHERE ra.is_active = 1 ", reportId);
            }
            else
            {
                query = string.Format(@"DECLARE @reportId INT = {0}

						IF(OBJECT_ID('tempdb..#temp_underlyer') IS NOT NULL)
							DROP TABLE #temp_underlyer

						IF(OBJECT_ID('tempdb..#tab') IS NOT NULL)
							DROP TABLE #tab

						CREATE TABLE #tab(id INT, 
							report_attribute_id INT, 
							report_attribute_name VARCHAR(MAX),
							sectype_id INT,
							attribute_id INT,
							is_additional_leg BIT,
							attribute_data_type VARCHAR(100), 
							report_decimal_places INT, 
							reference_type_id INT, 
							reference_attribute_id VARCHAR(MAX), 
							underlyer_attribute VARCHAR(MAX),
							is_visible BIT)

						DECLARE @attributes_to_show VARCHAR(MAX),
								@decimal_places VARCHAR(MAX)

						SELECT @attributes_to_show = attributes_to_show, @decimal_places = decimal_places
						FROM IVPSecMaster.dbo.ivp_secm_report_configuration
						WHERE report_setup_id = @reportId
						
						INSERT INTO #tab(id,report_attribute_id, report_attribute_name, sectype_id, attribute_id, is_additional_leg, reference_attribute_id, is_visible)
						SELECT id,ra.report_attribute_id, ra.report_attribute_name, sectype_id, attribute_id, 0, reference_attribute_id, 1
						FROM IVPRefMaster.dbo.RefM_GetList2TableWithRowIDWithoutTruncation(@attributes_to_show, '|') tab
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_report_attributes ra ON CASE WHEN tab.item LIKE 'A:%' OR tab.item LIKE 'S:%' 
							                        THEN SUBSTRING(SUBSTRING(tab.item,3,LEN(tab.item)-2),1,CHARINDEX(':',SUBSTRING(tab.item,3,LEN(tab.item)-2))-1) 
							                        WHEN tab.item LIKE 'R::%' 
							                        THEN REPLACE(tab.item,'R::','') 
							                        END = ra.report_attribute_id
						INNER JOIN IVPSecMaster.dbo.ivp_secm_report_attribute_mapping ram ON ra.report_attribute_id = ram.report_attribute_id
						
						INSERT INTO #tab(id,report_attribute_id, report_attribute_name, sectype_id, attribute_id, is_additional_leg, reference_attribute_id, is_visible)
						SELECT id,ra.report_attribute_id, ra.report_attribute_name, sectype_id, attribute_id, 1, reference_attribute_id, 1
						FROM IVPRefMaster.dbo.RefM_GetList2TableWithRowIDWithoutTruncation(@attributes_to_show, '|') tab
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_report_attributes ra ON CASE WHEN tab.item LIKE 'A:%' OR tab.item LIKE 'S:%' 
							                        THEN SUBSTRING(SUBSTRING(tab.item,3,LEN(tab.item)-2),1,CHARINDEX(':',SUBSTRING(tab.item,3,LEN(tab.item)-2))-1) 
							                        WHEN tab.item LIKE 'R::%' 
							                        THEN REPLACE(tab.item,'R::','') 
							                        END = ra.report_attribute_id
						INNER JOIN IVPSecMaster.dbo.ivp_secm_report_additional_leg_attribute_mapping ram ON ra.report_attribute_id = ram.report_attribute_id

                        UPDATE t
                        SET report_decimal_places = CAST(tab.item AS INT)
                        FROM IVPRefMaster.dbo.RefM_GetList2TableWithRowIDWithoutTruncation(@decimal_places, '|') tab
                        INNER JOIN #tab t ON tab.id = t.id

						-- ADD HIDDEN ATTRIBUTES
						INSERT INTO #tab(report_attribute_id, report_attribute_name, sectype_id, attribute_id, is_additional_leg, reference_attribute_id, is_visible)
						SELECT ra.report_attribute_id, ra.report_attribute_name, ram.sectype_id, ram.attribute_id, 0, ram.reference_attribute_id, 0
						FROM IVPSecMaster.dbo.ivp_secm_report_attributes ra
						INNER JOIN IVPSecMaster.dbo.ivp_secm_report_attribute_mapping ram ON ra.report_attribute_id = ram.report_attribute_id
						LEFT JOIN #tab t ON ra.report_attribute_id = t.report_attribute_id
						WHERE t.report_attribute_id IS NULL AND report_setup_id = @reportId

						INSERT INTO #tab(report_attribute_id, report_attribute_name, sectype_id, attribute_id, is_additional_leg, reference_attribute_id, is_visible)
						SELECT ra.report_attribute_id, ra.report_attribute_name, ram.sectype_id, ram.attribute_id, 1, ram.reference_attribute_id, 0
						FROM IVPSecMaster.dbo.ivp_secm_report_attributes ra 
						INNER JOIN IVPSecMaster.dbo.ivp_secm_report_additional_leg_attribute_mapping ram ON ra.report_attribute_id = ram.report_attribute_id
						LEFT JOIN #tab t ON ra.report_attribute_id = t.report_attribute_id
						WHERE t.report_attribute_id IS NULL AND report_setup_id = @reportId

                        UPDATE tab
						SET tab.reference_type_id = ram.reference_type_id, 
						tab.reference_attribute_id = CASE WHEN at.attribute_type_name = 'Reference' AND tab.reference_attribute_id <> '-1' THEN CAST(tab.reference_attribute_id AS INT) ELSE NULL END,
						tab.underlyer_attribute = CASE WHEN at.attribute_type_name = 'Identifier' AND tab.reference_attribute_id <> '-1' THEN tab.reference_attribute_id ELSE 'VARCHAR(15)' END,
						tab.attribute_data_type =
                        CASE WHEN at.attribute_type_name IN ('Reference', 'Identifier') AND tab.reference_attribute_id = '-1'
	                        THEN 'VARCHAR(15)'
							WHEN at.attribute_type_name IN ('Reference', 'Identifier') AND tab.reference_attribute_id <> '-1'
	                        THEN NULL
	                        WHEN ass.data_type_name = 'STRING' 
	                        THEN 'VARCHAR(' + eat.data_type_length + ')'
	                        WHEN ass.DB_data_type IN ('DATETIME', 'BIT', 'TIME')
	                        THEN ass.DB_data_type
	                        WHEN ass.data_type_name = 'NUMERIC'
	                        THEN 'NUMERIC(' + CAST(CAST(SUBSTRING(data_type_length,1,CHARINDEX('.',data_type_length)-1) AS INT) + CAST(SUBSTRING(data_type_length,CHARINDEX('.',data_type_length)+1,LEN(data_type_length)) AS INT) AS VARCHAR(MAX)) + ',' + SUBSTRING(data_type_length,CHARINDEX('.',data_type_length)+1,LEN(data_type_length)) + ')'
	                        END
                        FROM #tab tab
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_details eat ON (tab.attribute_id = eat.attribute_id AND tab.is_additional_leg = 0)
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_subtype ass ON eat.attribute_subtype_id = ass.attribute_subtype_id
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_type at ON ass.attribute_type_id = at.attribute_type_id
                        LEFT JOIN IVPSecMaster.dbo.ivp_secm_reference_attribute_mapping ram ON eat.attribute_id = ram.attribute_id
                        WHERE eat.is_active = 1

                        UPDATE tab
						SET tab.reference_type_id = ramm.reference_type_id,
						tab.reference_attribute_id = CASE WHEN ass.display_data_type = 'REFERENCE' AND tab.reference_attribute_id <> '-1' THEN CAST(tab.reference_attribute_id AS INT) ELSE NULL END,
						tab.underlyer_attribute = CASE WHEN ass.display_data_type = 'SECURITY' AND tab.reference_attribute_id <> '-1' THEN tab.reference_attribute_id ELSE 'VARCHAR(15)' END,
						tab.attribute_data_type =
                        CASE WHEN ass.display_data_type = 'STRING' 
	                        THEN 'VARCHAR(' + ad.data_type_length + ')'
	                        WHEN ass.display_data_type = 'TEXT' 
	                        THEN 'VARCHAR(MAX)'
	                        WHEN ass.display_data_type IN ('REFERENCE' ,'SECURITY') AND tab.reference_attribute_id = '-1'
	                        THEN 'VARCHAR(15)'
							WHEN ass.display_data_type IN ('REFERENCE', 'SECURITY') AND tab.reference_attribute_id <> '-1'
	                        THEN NULL
	                        WHEN ass.display_data_type = 'FILE' 
	                        THEN 'VARCHAR(200)'
	                        WHEN ass.DB_data_type IN ('DATETIME', 'BIT', 'TIME')
	                        THEN ass.DB_data_type
	                        WHEN ass.display_data_type = 'NUMERIC'
	                        THEN 'NUMERIC(' + CAST(CAST(SUBSTRING(data_type_length,1,CHARINDEX('.',data_type_length)-1) AS INT) + CAST(SUBSTRING(data_type_length,CHARINDEX('.',data_type_length)+1,LEN(data_type_length)) AS INT) AS VARCHAR(MAX)) + ',' + SUBSTRING(data_type_length,CHARINDEX('.',data_type_length)+1,LEN(data_type_length)) + ')'
	                        END
                        FROM #tab tab
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_additional_legs_attribute_details ad ON (tab.attribute_id = ad.attribute_id AND tab.is_additional_leg = 1)
                        INNER JOIN IVPSecMaster.dbo.ivp_secm_additional_legs_attribute_type ass ON ad.attribute_type_id = ass.attribute_type_id
                        LEFT JOIN IVPSecMaster.dbo.ivp_secm_additional_legs_reference_attribute_mapping ramm ON ad.attribute_id = ramm.attribute_id
                        WHERE ad.is_active = 1

						UPDATE tab
						SET tab.attribute_data_type =
                        CASE WHEN eat.data_type = 'VARCHAR' 
								THEN 'VARCHAR(' + eat.data_type_length + ')'
	                        WHEN eat.data_type IN ('VARCHARMAX','FILE')
								THEN 'VARCHAR(MAX)'
	                        WHEN eat.data_type IN ('INT','DATETIME', 'BIT')
								THEN eat.data_type
	                        WHEN eat.data_type = 'DECIMAL'
								THEN 'DECIMAL(' + eat.data_type_length + ')'
							WHEN eat.data_type IN ('LOOKUP', 'SECURITY_LOOKUP')
								THEN 'VARCHAR(MAX)'
	                        END
						FROM #tab tab
						INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute eat ON tab.reference_type_id = eat.entity_type_id AND tab.reference_attribute_id = eat.entity_attribute_id
						WHERE tab.attribute_data_type IS NULL OR tab.attribute_data_type = ''

						SELECT CASE WHEN ast.data_type_name = 'STRING' 
								THEN 'VARCHAR(' + eat.data_type_length + ')'
	                        WHEN ast.data_type_name IN ('DATE', 'DATETIME', 'BOOLEAN', 'FILE')
								THEN ast.DB_data_type
	                        WHEN ast.data_type_name = 'NUMERIC'
								THEN 'NUMERIC(' + CAST((CAST(SUBSTRING(eat.data_type_length,1,CHARINDEX('.',eat.data_type_length)-1) AS INT) + CAST(SUBSTRING(eat.data_type_length,CHARINDEX('.',eat.data_type_length)+1,LEN(eat.data_type_length)-CHARINDEX('.',eat.data_type_length)+1) AS INT)) AS VARCHAR(MAX)) + ',' + CAST(SUBSTRING(eat.data_type_length,CHARINDEX('.',eat.data_type_length)+1,LEN(eat.data_type_length)-CHARINDEX('.',eat.data_type_length)+1) AS VARCHAR(MAX)) + ')'
	                        END AS underlyer_massaged_attribute, tab.id, tab.attribute_id, tab.sectype_id, tab.is_additional_leg
						INTO #temp_underlyer
						FROM #tab tab
						CROSS APPLY IVPSecMaster.dbo.SECM_GetList2Table(tab.underlyer_attribute, ',') capp
						INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_details eat ON CAST(capp.item AS INT) = eat.attribute_id
						INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_subtype ast ON eat.attribute_subtype_id = ast.attribute_subtype_id
						WHERE tab.underlyer_attribute IS NOT NULL AND tab.underlyer_attribute != 'VARCHAR(15)'

						UPDATE tab
						SET tab.attribute_data_type = MAS.underlyer_massaged_attribute
						FROM #tab tab
						INNER JOIN (SELECT id, underlyer_massaged_attribute
									FROM #temp_underlyer
									GROUP BY id, underlyer_massaged_attribute
									HAVING COUNT(1) = 1
									) MAS ON tab.id = MAS.id AND tab.underlyer_attribute IS NOT NULL

						UPDATE tab
						SET tab.attribute_data_type = 'VARCHAR(MAX)'
						FROM #tab tab
						INNER JOIN (SELECT DISTINCT id
									FROM #temp_underlyer
									GROUP BY id, underlyer_massaged_attribute
									HAVING COUNT(1) > 1
									) MAS ON tab.id = MAS.id

                        SELECT * 
						FROM #tab
						WHERE report_attribute_name IS NOT NULL AND attribute_data_type IS NOT NULL", reportId);
            }

            var ds = CommonDALWrapper.ExecuteSelectQueryObject(query, ConnectionConstants.RefMaster_Connection);
            var dt = ds.Tables[0];

            if (isRefM)
            {
                foreach (ObjectRow row in dt.Rows)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(row["report_attribute_data_type"])) && Convert.ToString(row["report_attribute_data_type"]).Equals("SECURITY_LOOKUP") && !attributesWithSecId.Contains(Convert.ToInt32(row["report_attribute_id"])))
                    {
                        secLookupsAttributeVsDataType.Add(Convert.ToInt32(row["report_attribute_id"]), string.Empty);
                    }
                }

                if (secLookupsAttributeVsDataType.Count > 0)
                {
                    query = string.Format(@"SELECT report_attribute_id,
	                            CASE 
	                            WHEN at.attribute_type_name IN ('Reference', 'Identifier')
		                            THEN 'VARCHAR(MAX)'
	                            WHEN sub.data_type_name = 'STRING' 
	                                THEN 'VARCHAR(' + det.data_type_length + ')'
	                            WHEN sub.DB_data_type IN ('DATETIME', 'BIT', 'TIME')
	                                THEN sub.DB_data_type
	                            WHEN sub.data_type_name = 'NUMERIC'
	                                THEN 'NUMERIC(' + CAST(CAST(SUBSTRING(data_type_length,1,CHARINDEX('.',data_type_length)-1) AS INT) + CAST(SUBSTRING(data_type_length,CHARINDEX('.',data_type_length)+1,LEN(data_type_length)) AS INT) AS VARCHAR(MAX)) + ',' + SUBSTRING(data_type_length,CHARINDEX('.',data_type_length)+1,LEN(data_type_length)) + ')'
	                            END
                             FROM IVPRefMaster.dbo.ivp_refm_report_attribute_map map
                            INNER JOIN IVPRefMaster.dbo.ivp_refm_security_attribute_lookup sal
                            ON (sal.child_entity_attribute_id = map.dependent_attribute_id)
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_details det
                            ON (det.attribute_id = sal.parent_security_attribute_id)
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_subtype sub
                            ON (sub.attribute_subtype_id = det.attribute_subtype_id)
                            INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_type at 
                            ON (sub.attribute_type_id = at.attribute_type_id)
                            WHERE report_attribute_id IN ({0})", string.Join(",", secLookupsAttributeVsDataType.Select(x => x.Key)));

                    var secDt = CommonDALWrapper.ExecuteSelectQueryObject(query, ConnectionConstants.RefMaster_Connection).Tables[0];
                    foreach (ObjectRow row in secDt.Rows)
                    {
                        secLookupsAttributeVsDataType[Convert.ToInt32(row[0])] = Convert.ToString(row[1]);
                    }
                }
            }

            foreach (var attLevel in dt.AsEnumerable().GroupBy(x => Convert.ToString(x["report_attribute_name"])))
            {
                string dataType = null;
                int maxPrecision = -1;
                int maxDecimalPlaces = 0;
                int scaleForMaxPrecision = 0;

                bool firstRow = true;
                foreach (var row in attLevel)
                {
                    if (!isRefM && firstRow)
                    {
                        if (!row.IsNull("is_visible") && !row.Field<bool>("is_visible"))
                            hiddenAttributes.Add(attLevel.Key);
                    }
                    firstRow = false;

                    if (secLookupsAttributeVsDataType.ContainsKey(Convert.ToInt32(row["report_attribute_id"])))
                    {
                        row["attribute_data_type"] = Convert.ToString(secLookupsAttributeVsDataType[Convert.ToInt32(row["report_attribute_id"])]);
                    }

                    var arr = Convert.ToString(row["attribute_data_type"]).ToLower().Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
                    dataType = arr[0];
                    if (arr.Length > 1)
                    {
                        if (arr[0] == "varchar")
                        {
                            if (arr[1] == "max")
                                break;
                            else
                            {
                                int precision = Convert.ToInt32(arr[1]);
                                if (maxPrecision < precision)
                                    maxPrecision = precision;
                            }
                        }
                        else if (arr[0] == "numeric" || arr[0] == "decimal")
                        {
                            int scale = Convert.ToInt32(arr[2]);
                            int precision = Convert.ToInt32(arr[1]) - scale;
                            int reportAttributeScale = 0;

                            if (row["report_decimal_places"] != DBNull.Value)
                                reportAttributeScale = Convert.ToInt32(row["report_decimal_places"]);

                            if (maxPrecision < precision)
                            {
                                maxPrecision = precision;
                                scaleForMaxPrecision = scale;
                            }
                            if (maxDecimalPlaces < reportAttributeScale)
                                maxDecimalPlaces = reportAttributeScale;
                        }
                    }
                }
                if (dataType == "varchar")
                {
                    if (maxPrecision == -1)
                        dataType = "VARCHAR(MAX)";
                    else
                        dataType = "VARCHAR(" + maxPrecision + ")";
                }
                else if (dataType == "numeric" || dataType == "decimal")
                {
                    if (scaleForMaxPrecision < maxDecimalPlaces)
                        maxPrecision += maxDecimalPlaces - scaleForMaxPrecision;

                    var precision = maxPrecision + maxDecimalPlaces;
                    if (precision <= 38)
                        dataType = dataType + "(" + precision + "," + maxDecimalPlaces + ")";
                    else
                        invalidLengthAttributes.Add(attLevel.Key);
                }
                reportAttributeVsDatatypeAndDecimalPlaces.Add(attLevel.Key, dataType);
            }

            if (invalidLengthAttributes.Count > 0)
                throw new Exception("Invalid length for attributes: " + string.Join(",", invalidLengthAttributes));

            return reportAttributeVsDatatypeAndDecimalPlaces;
        }
    }
}
