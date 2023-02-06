using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.ivp.commom.commondal;
using com.ivp.rad.common;
using com.ivp.rad.data;
using com.ivp.rad.RUserManagement;

namespace com.ivp.common
{
    public class SMDataSourceController
    {
        static IRLogger mLogger = RLogFactory.CreateLogger("SMDataSourceController");

        public static List<string> GetBloombergLicenseCategory()
        {
            try
            {
                mLogger.Debug("GetBloombergLicenseCategory -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject("SELECT * FROM [IVPSecMasterVendor].[dbo].[ivp_secmv_bloomberg_license_category]", ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().Select(x => x.Field<string>("bloomberg_license_category_name")).ToList();
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetBloombergLicenseCategory -> End");
            }
        }

        public static List<string> GetBloombergBulkListMapping()
        {
            try
            {
                mLogger.Debug("GetBloombergBulkListMapping -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject("select market_sector + ' - ' + request_field AS bloomberg_bulk_field from IVPRAD.dbo.ivp_rad_bbg_bulklist_mapping", ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().Select(x => x.Field<string>("bloomberg_bulk_field")).ToList();
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetBloombergBulkListMapping -> End");
            }
        }

        public static List<string> GetFileDateTypes()
        {
            try
            {
                mLogger.Debug("GetFileDateTypes -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject("select * from IVPRefMaster.dbo.ivp_refm_date_type WHERE date_type_id BETWEEN 0 AND 10", ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().Select(x => x.Field<string>("date_display_name")).ToList();
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetFileDateTypes -> End");
            }
        }

        public static List<string> GetVendorManagementPreferenceNames()
        {
            try
            {
                mLogger.Debug("GetVendorManagementPreferenceNames -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject("select * from [IVPRefMasterVendor].[dbo].[ivp_rad_vendor_management_master]", ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().Select(x => x.Field<string>("vendor_management_name")).ToList();
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetVendorManagementPreferenceNames -> End");
            }
        }

        public static List<string> GetMarketSectorAttributes()
        {
            try
            {
                mLogger.Debug("GetMarketSectorAttributes -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT td.display_name
FROM IVPSecMaster.dbo.ivp_secm_template_details td
INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_details ad
ON td.attribute_id = ad.attribute_id AND td.template_id = 0
INNER JOIN IVPSecMaster.dbo.ivp_secm_attribute_subtype ast
ON ast.attribute_subtype_id = ad.attribute_subtype_id
WHERE ast.attribute_subtype_id IN(1, 3) AND td.is_active = 1 AND ad.is_active = 1", ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().Select(x => x.Field<string>("display_name")).ToList();
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetMarketSectorAttributes -> End");
            }
        }

        public static List<string> GetFTPTransportNames()
        {
            try
            {
                mLogger.Debug("GetFTPTransportNames -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT * FROM IVPRAD.dbo.ivp_rad_transport_config_details WHERE transport_type_id IN (2,6) AND is_active = 1", ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().Select(x => x.Field<string>("transport_name")).ToList();
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetFTPTransportNames -> End");
            }
        }

        public static ObjectTable GetVendors(List<string> vendorNames)
        {
            try
            {
                mLogger.Debug("GetVendors -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT vm.vendor_id,
			                                                                vm.vendor_name AS [Data Source],
			                                                                vm.vendor_description AS [Data Source Description],
			                                                                vm.vendor_type_id, 
			                                                                vt.vendor_type AS [Data Source Type]
                                                                FROM [IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_master] vm
                                                                INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_type] vt
                                                                ON vm.vendor_type_id = vt.vendor_type_id" + Environment.NewLine
                                                                + (vendorNames != null && vendorNames.Count > 0 ?
                                                                @"INNER JOIN [IVPSecMaster].[dbo].[SECM_GetList2Table]('" + string.Join(",", vendorNames) + @"',',') tab
                                                                ON tab.item = vm.vendor_name" : string.Empty) + Environment.NewLine +
                                                                @"WHERE vm.is_active = 1 AND vm.vendor_id > 0", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetVendors -> End");
            }
        }

        public static ObjectTable GetFeeds(List<string> vendorNames)
        {
            try
            {
                mLogger.Debug("GetFeeds -> Start");

                return CommonDALWrapper.ExecuteSelectQueryObject(@"EXEC IVPRefMaster.dbo.REFM_OpenEncryptionKey
                                                                    SELECT  vm.vendor_id,
		                                                                    vm.vendor_name AS [Data Source],
		                                                                    vm.vendor_description AS [Data Source Description],
		                                                                    vm.vendor_type_id, 
		                                                                    vt.vendor_type AS [Data Source Type],
		                                                                    fs.feed_id,
		                                                                    fs.feed_name AS [Feed Name],
		                                                                    fs.feed_type_id,
		                                                                    fst.feed_source_id,
		                                                                    fst.feed_source_type_name AS [Feed Source Type],
		                                                                    blc.bloomberg_license_category_name AS [Bloomberg License Category],
		                                                                    ft.file_type_id,
		                                                                    CASE WHEN ft.file_type_id = 4 THEN '' ELSE ft.file_type_name END AS [File Type],
		                                                                    CASE 
			                                                                    WHEN ft.file_type_id = 4 THEN NULL
			                                                                    WHEN ft.file_type_id IN (0,1) 
				                                                                    THEN 
					                                                                    CASE 
						                                                                    WHEN ASCII(row_delimiter) = 13 THEN '\r\n' 
						                                                                    WHEN ASCII(row_delimiter) = 10 THEN '\n' 
						                                                                    ELSE row_delimiter
					                                                                    END
			                                                                    ELSE fp.row_delimiter
		                                                                    END AS [Record Delimiter],
		                                                                    CASE WHEN ft.file_type_id = 1 THEN fp.field_delimiter ELSE NULL END AS [Field Delimiter],
		                                                                    CASE 
			                                                                    WHEN ft.file_type_id = 4 THEN NULL
			                                                                    WHEN ft.file_type_id IN (0,1)
				                                                                    THEN
					                                                                    CASE 
						                                                                    WHEN fp.comment_char = 'Æ' THEN ''
						                                                                    ELSE fp.comment_char
					                                                                    END
			                                                                    ELSE NULL
		                                                                    END AS [Comment Char],
		                                                                    CASE 
			                                                                    WHEN ft.file_type_id = 4 THEN NULL 
			                                                                    WHEN ft.file_type_id = 1
				                                                                    THEN
					                                                                    CASE 
						                                                                    WHEN fp.single_escape = 'Æ' THEN ''
						                                                                    ELSE fp.single_escape
					                                                                    END
			                                                                    ELSE NULL
		                                                                    END AS [Single Escape],
		                                                                    CASE 
			                                                                    WHEN ft.file_type_id = 4 THEN NULL 
			                                                                    WHEN ft.file_type_id = 1
				                                                                    THEN
					                                                                    CASE 
						                                                                    WHEN fp.paired_escape = 'Æ' THEN ''
						                                                                    ELSE fp.paired_escape
					                                                                    END
			                                                                    ELSE NULL
		                                                                    END AS [Paired Escape],
		                                                                    CASE WHEN ft.file_type_id IN (2,4) THEN NULL ELSE fp.exclude_regex END AS [Exclude Regex],
		                                                                    CASE WHEN ft.file_type_id = 0 THEN fp.record_length ELSE NULL END AS [Record Length],
		                                                                    CASE WHEN ft.file_type_id = 2 THEN fp.root_xpath ELSE NULL END AS [Root XPath],
		                                                                    CASE WHEN ft.file_type_id = 2 THEN fp.record_xpath ELSE NULL END AS [Record XPath],
		                                                                    CASE WHEN fst.feed_source_id = 4 THEN bbm.market_sector + ' - ' + bbm.request_field ELSE NULL END AS [Bloomberg Bulk Field],
		                                                                    CASE WHEN ft.file_type_id = 4 THEN fs.db_provider_type ELSE NULL END AS [Server Type],
		                                                                    CASE WHEN ft.file_type_id = 4 THEN IVPRefMaster.dbo.REFM_Decrypt(fs.connection_string) ELSE NULL END AS [Connection String],
		                                                                    CASE WHEN ft.file_type_id = 4 THEN fs.column_query ELSE NULL END AS [Loading Criteria]
                                                                    FROM [IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_master] vm
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_type] vt
                                                                    ON vm.vendor_type_id = vt.vendor_type_id
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_summary] fs
                                                                    ON vm.vendor_id = fs.vendor_id
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_type] ftype
                                                                    ON ftype.feed_type_id = fs.feed_type_id
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_source_type] fst
                                                                    ON fst.feed_source_id = fs.source_type
                                                                    INNER JOIN [IVPSecMaster].[dbo].[ivp_rad_file_property] fp
                                                                    ON fs.file_id = fp.file_id
                                                                    LEFT OUTER JOIN [IVPSecMaster].[dbo].[ivp_rad_file_type] ft
                                                                    ON fp.file_type = ft.file_type_id
                                                                    LEFT OUTER JOIN [IVPRAD].[dbo].[ivp_rad_bbg_bulklist_mapping] bbm
                                                                    ON bbm.bulklist_mapping_id = fs.exotic_id
                                                                    LEFT OUTER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_bloomberg_license_category] blc
                                                                    ON blc.bloomberg_license_category_id = fs.bloomberg_license_category_id" + Environment.NewLine
                                                                    + (vendorNames != null && vendorNames.Count > 0 ?
                                                                    @"INNER JOIN [IVPSecMaster].[dbo].[SECM_GetList2Table]('" + string.Join(",", vendorNames) + @"',',') tab
                                                                    ON tab.item = vm.vendor_name" : string.Empty) + Environment.NewLine +
                                                                    //(feedNames != null && feedNames.Count > 0 ?
                                                                    //@"INNER JOIN [IVPSecMaster].[dbo].[SECM_GetList2Table]('" + string.Join(",", feedNames) + @"',',') tab1
                                                                    //ON tab1.item = fs.feed_name" : string.Empty) + Environment.NewLine +
                                                                    @"WHERE vm.is_active = 'TRUE' AND fs.is_active = 'TRUE' AND fp.is_active = 'TRUE'
                                                                    ORDER BY vm.vendor_name, fs.feed_name", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetFeeds -> End");
            }
        }

        public static ObjectTable GetFeedFields(List<string> vendorNames)//, List<string> feedNames
        {
            try
            {
                mLogger.Debug("GetFeedFields -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT  vm.vendor_id,
		                                                                    vm.vendor_name AS [Data Source],
		                                                                    fs.feed_id,
		                                                                    fs.feed_name AS [Feed Name],
                                                                            ffd.field_id,
		                                                                    ffd.field_name AS [Field Name],
		                                                                    ffd.field_description AS [Field Description],
		                                                                    fd.is_primary AS [Is Primary],
		                                                                    ffd.start_index AS [Start Index],
		                                                                    ffd.end_index AS [End Index],
		                                                                    CASE WHEN ffd.field_position IN ('MAPPED')
		                                                                    THEN ffd.field_position
		                                                                    ELSE CAST((CAST(ffd.field_position AS INT) + 1) AS VARCHAR(100))
		                                                                    END AS [Field Position],
		                                                                    ffd.field_x_path AS [Field XPath],
		                                                                    ffd.mandatory AS [Mandatory],
		                                                                    ffd.persistency AS [Persistable],
		                                                                    ffd.allow_trim AS [Allow Trim],
		                                                                    ffd.remove_white_space AS [Remove White Space],
		                                                                    fd.is_bloomberg_mnemonic AS [Is Bloomberg Mnemonic],
		                                                                    fd.for_api AS [For API],
		                                                                    fd.for_ftp AS [For FTP],
		                                                                    fd.for_bulk AS [For Bulk],
                                                                            CAST(1 AS BIT) AS is_update
                                                                    FROM[IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_master] vm
                                                                    INNER JOIN[IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_type] vt
                                                                    ON vm.vendor_type_id = vt.vendor_type_id
                                                                    INNER JOIN[IVPSecMasterVendor].[dbo].[ivp_secmv_feed_summary] fs
                                                                    ON vm.vendor_id = fs.vendor_id
                                                                    INNER JOIN[IVPSecMasterVendor].[dbo].[ivp_secmv_feed_details] fd
                                                                    ON fd.feed_id = fs.feed_id
                                                                    INNER JOIN[IVPSecMaster].[dbo].[ivp_rad_file_field_details] ffd
                                                                    ON ffd.field_id = fd.field_id AND fs.file_id = ffd.file_id" + Environment.NewLine
                                                                    + (vendorNames != null && vendorNames.Count > 0 ?
                                                                    @"INNER JOIN [IVPSecMaster].[dbo].[SECM_GetList2Table]('" + string.Join(",", vendorNames) + @"',',') tab
                                                                    ON tab.item = vm.vendor_name" : string.Empty) + Environment.NewLine +
                                                                    //(feedNames != null && feedNames.Count > 0 ?
                                                                    //@"INNER JOIN [IVPSecMaster].[dbo].[SECM_GetList2Table]('" + string.Join(",", feedNames) + @"',',') tab1
                                                                    //ON tab1.item = fs.feed_name" : string.Empty) + Environment.NewLine +
                                                                    @"WHERE vm.is_active = 'TRUE' AND fs.is_active = 'TRUE' AND fd.is_active = 'TRUE' AND ffd.is_active = 'TRUE'
                                                                    ORDER BY vm.vendor_name, fs.feed_name, fd.is_primary DESC, ffd.field_name", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetFeedFields -> End");
            }
        }

        public static ObjectTable GetFeedFieldsMapping(List<string> vendorNames)//, List<string> feedNames
        {
            try
            {
                mLogger.Debug("GetFeedFieldsMapping -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT *
	                                                                FROM
	                                                                (
		                                                                SELECT  vm.vendor_id,
				                                                                vm.vendor_name AS [Data Source],
				                                                                fs.feed_id,
				                                                                fs.feed_name AS [Feed Name],
				                                                                sm.sectype_name AS [Security Type],
				                                                                ISNULL(st.table_display_name,'') AS [Leg Name],
				                                                                CASE WHEN sectype_attribute_id = -1 THEN 'Security Id'
				                                                                ELSE td.display_name END AS [Attribute Name],
				                                                                ffd.field_name AS [Field Name],fmd.update_blank AS [Update Blank],
				                                                                CASE 
					                                                                WHEN ad.attribute_name = 'underlying_sec_id' THEN iden.display_name 
					                                                                WHEN rfmd.reference_attribute_name = 'entity_code' THEN 'Entity Code'
					                                                                ELSE ea.display_name 
				                                                                END AS [Reference Attribute],
				                                                                format.format AS [Date Format],
				                                                                CAST(0 AS BIT) AS is_additional_leg
		                                                                FROM [IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_master] vm
		                                                                INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_summary] fs
		                                                                ON vm.vendor_id = fs.vendor_id
		                                                                INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_sectype_feed_mapping_master] fmm
		                                                                ON fmm.feed_id = fs.feed_id
		                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_sectype_master] sm
		                                                                ON sm.sectype_id = fmm.sectype_id
		                                                                INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_sectype_feed_mapping_details] fmd
		                                                                ON fmd.sectype_feed_mapping_id = fmm.sectype_feed_mapping_id AND fmd.is_additional_leg = 0
		                                                                LEFT JOIN [IVPSecMaster].[dbo].[ivp_secm_template_master] tm
		                                                                ON tm.sectype_id = fmm.sectype_id AND tm.dependent_id = 'SYSTEM' AND tm.is_active = 1
		                                                                LEFT JOIN [IVPSecMaster].[dbo].[ivp_secm_template_dependent_type] tdt
		                                                                ON tm.dependent_type_id = tdt.dependent_type_id AND tdt.dependent_type_name = 'SYSTEM'
		                                                                LEFT JOIN [IVPSecMaster].[dbo].[ivp_secm_template_details] td
		                                                                ON tm.template_id = td.template_id AND td.attribute_id = fmd.sectype_attribute_id AND td.is_active = 1
		                                                                LEFT JOIN [IVPSecMaster].[dbo].[ivp_secm_attribute_details] ad
		                                                                ON ad.attribute_id = td.attribute_id AND ad.is_active = 1
		                                                                LEFT JOIN [IVPSecMaster].[dbo].[ivp_secm_sectype_table] st
		                                                                ON ad.sectype_table_id = st.sectype_table_id AND st.priority <> 1 AND st.sectype_id <> 0
		                                                                INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_details] fd
		                                                                ON fd.feed_field_id = fmd.feed_field_id
		                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_rad_file_field_details] ffd
		                                                                ON ffd.field_id = fd.field_id
		                                                                LEFT JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_field_format_details] format
		                                                                ON format.format_details_id = fmd.format_details_id
		                                                                LEFT JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_reference_feed_mapping_details] rfmd
		                                                                ON rfmd.sectype_feed_details_id = fmd.sectype_feed_details_id
		                                                                LEFT JOIN [IVPSecMaster].[dbo].[ivp_secm_reference_attribute_mapping] ram
		                                                                ON ram.attribute_id = ad.attribute_id
		                                                                LEFT JOIN [IVPRefMaster].[dbo].[ivp_refm_entity_type] et
		                                                                ON et.entity_type_id = ram.reference_type_id
		                                                                LEFT JOIN [IVPRefMaster].[dbo].[ivp_refm_entity_attribute] ea
		                                                                ON ea.entity_type_id = et.entity_type_id AND ea.attribute_name = rfmd.reference_attribute_name
		                                                                LEFT JOIN
		                                                                (
			                                                                SELECT ad1.attribute_id,
					                                                                   ad1.attribute_name,
					                                                                   td1.display_name
			                                                                FROM [IVPSecMaster].[dbo].[ivp_secm_template_details] td1
			                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_attribute_details] ad1
			                                                                ON (td1.template_id = 0 AND (ad1.sectype_table_id = 1 OR ad1.sectype_table_id = 2) AND td1.attribute_id = ad1.attribute_id)
			                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_attribute_subtype] ast
			                                                                ON (ast.attribute_subtype_id = ad1.attribute_subtype_id)
			                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_attribute_type] at
			                                                                ON (ast.attribute_type_id = at.attribute_type_id)
			                                                                WHERE td1.is_active = 'TRUE' AND ad1.is_active = 'TRUE' AND at.attribute_type_name = 'Default' AND (ast.data_type_name = 'STRING' OR ast.data_type_name = 'NUMERIC')
		                                                                ) iden
		                                                                ON iden.attribute_name = rfmd.reference_attribute_name AND ad.attribute_name = 'underlying_sec_id'" + Environment.NewLine
                                                                    + (vendorNames != null && vendorNames.Count > 0 ?
                                                                    @"INNER JOIN [IVPSecMaster].[dbo].[SECM_GetList2Table]('" + string.Join(",", vendorNames) + @"',',') tab
                                                                    ON tab.item = vm.vendor_name" : string.Empty) + Environment.NewLine +
                                                                    //(feedNames != null && feedNames.Count > 0 ?
                                                                    //@"INNER JOIN [IVPSecMaster].[dbo].[SECM_GetList2Table]('" + string.Join(",", feedNames) + @"',',') tab1
                                                                    //ON tab1.item = fs.feed_name" : string.Empty) + Environment.NewLine +
                                                                    @"WHERE vm.is_active = 'TRUE' AND fs.is_active = 'TRUE' AND fmm.is_active = 'TRUE' AND fmd.is_active = 'TRUE' AND fd.is_active = 'TRUE' AND ffd.is_active = 'TRUE'
		                                                                UNION ALL
		                                                                SELECT  vm.vendor_id,
				                                                                vm.vendor_name AS [Data Source],
				                                                                fs.feed_id,
				                                                                fs.feed_name AS [Feed Name],
				                                                                sm.sectype_name AS [Security Type],
				                                                                ISNULL(al.additional_legs_name,'') AS [Leg Name],
				                                                                CASE WHEN sectype_attribute_id = -1 THEN 'Security Id'
				                                                                ELSE lad.display_name END AS [Attribute Name],
				                                                                ffd.field_name AS [Field Name],fmd.update_blank AS [Update Blank],
				                                                                CASE 
					                                                                WHEN lat.attribute_type_id = 10 THEN iden.display_name 
					                                                                WHEN rfmd.reference_attribute_name = 'entity_code' THEN 'Entity Code'
					                                                                ELSE ea.display_name 
				                                                                END AS [Reference Attribute],
				                                                                format.format AS [Date Format],
				                                                                CAST(1 AS BIT) AS is_additional_leg
		                                                                FROM [IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_master] vm
		                                                                INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_summary] fs
		                                                                ON vm.vendor_id = fs.vendor_id
		                                                                INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_sectype_feed_mapping_master] fmm
		                                                                ON fmm.feed_id = fs.feed_id
		                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_sectype_master] sm
		                                                                ON sm.sectype_id = fmm.sectype_id
		                                                                INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_sectype_feed_mapping_details] fmd
		                                                                ON fmd.sectype_feed_mapping_id = fmm.sectype_feed_mapping_id AND fmd.is_additional_leg = 1
		                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_additional_legs_sectype_mapping] lsm
		                                                                ON lsm.sectype_id = fmm.sectype_id
		                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_additional_legs] al
		                                                                ON al.additional_legs_id = lsm.additional_legs_id
		                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_additional_legs_attribute_details] lad
		                                                                ON lad.additional_legs_id = lsm.additional_legs_id AND lad.attribute_id = fmd.sectype_attribute_id
		                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_additional_legs_attribute_type] lat
		                                                                ON lat.attribute_type_id = lad.attribute_type_id
		                                                                INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_details] fd
		                                                                ON fd.feed_field_id = fmd.feed_field_id
		                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_rad_file_field_details] ffd
		                                                                ON ffd.field_id = fd.field_id
		                                                                LEFT JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_field_format_details] format
		                                                                ON format.format_details_id = fmd.format_details_id
		                                                                LEFT JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_reference_feed_mapping_details] rfmd
		                                                                ON rfmd.sectype_feed_details_id = fmd.sectype_feed_details_id
		                                                                LEFT JOIN [IVPSecMaster].[dbo].[ivp_secm_additional_legs_reference_attribute_mapping] ram
		                                                                ON ram.attribute_id = lad.attribute_id
		                                                                LEFT JOIN [IVPRefMaster].[dbo].[ivp_refm_entity_type] et
		                                                                ON et.entity_type_id = ram.reference_type_id
		                                                                LEFT JOIN [IVPRefMaster].[dbo].[ivp_refm_entity_attribute] ea
		                                                                ON ea.entity_type_id = et.entity_type_id AND ea.attribute_name = rfmd.reference_attribute_name
		                                                                LEFT JOIN
		                                                                (
			                                                                SELECT ad1.attribute_id,
					                                                                   ad1.attribute_name,
					                                                                   td1.display_name
			                                                                FROM [IVPSecMaster].[dbo].[ivp_secm_template_details] td1
			                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_attribute_details] ad1
			                                                                ON (td1.template_id = 0 AND (ad1.sectype_table_id = 1 OR ad1.sectype_table_id = 2) AND td1.attribute_id = ad1.attribute_id)
			                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_attribute_subtype] ast
			                                                                ON (ast.attribute_subtype_id = ad1.attribute_subtype_id)
			                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_attribute_type] at
			                                                                ON (ast.attribute_type_id = at.attribute_type_id)
			                                                                WHERE td1.is_active = 'TRUE' AND ad1.is_active = 'TRUE' AND at.attribute_type_name = 'Default' AND (ast.data_type_name = 'STRING' OR ast.data_type_name = 'NUMERIC')
		                                                                ) iden
		                                                                ON iden.attribute_name = rfmd.reference_attribute_name AND lat.attribute_type_id = 10 " + Environment.NewLine
                                                                    + (vendorNames != null && vendorNames.Count > 0 ?
                                                                    @"INNER JOIN [IVPSecMaster].[dbo].[SECM_GetList2Table]('" + string.Join(",", vendorNames) + @"',',') tab
                                                                    ON tab.item = vm.vendor_name" : string.Empty) + Environment.NewLine +
                                                                    //(feedNames != null && feedNames.Count > 0 ?
                                                                    //@"INNER JOIN [IVPSecMaster].[dbo].[SECM_GetList2Table]('" + string.Join(",", feedNames) + @"',',') tab1
                                                                    //ON tab1.item = fs.feed_name" : string.Empty) + Environment.NewLine +
                                                                    @"WHERE vm.is_active = 'TRUE' AND fs.is_active = 'TRUE' AND fmm.is_active = 'TRUE' AND fmd.is_active = 'TRUE' AND al.is_active = 1 AND lad.is_active = 1 AND fd.is_active = 'TRUE' AND ffd.is_active = 'TRUE'
	                                                                ) p
	                                                                ORDER BY [Data Source], [Feed Name], [Security Type], [Leg Name], [Attribute Name]", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetFeedFieldsMapping -> End");
            }
        }

        public static ObjectSet GetSectypes()
        {
            try
            {
                mLogger.Debug("GetSectypes -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"EXEC IVPSecMasterVendor.[dbo].[SECMV_GetSectypesForDataSourceUploadDownload]", ConnectionConstants.SecMaster_Connection);
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetSectypes -> End");
            }
        }

        public static ObjectTable GetFeedRules()
        {
            try
            {
                mLogger.Debug("GetFeedRules -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT  vm.vendor_id,
		                                                                    vm.vendor_name AS[Data Source],
		                                                                    fs.feed_id,
		                                                                    fs.feed_name AS[Feed Name],
		                                                                    rm.rule_type_id,
		                                                                    CASE WHEN rm.rule_type_id = 4 THEN 'Transformation' ELSE 'Validation' END AS[Rule Type],
		                                                                    xr.rule_name AS[Rule Name],
		                                                                    xr.rule_text AS[Rule Text],
		                                                                    xr.rule_state AS[Rule State],
		                                                                    xr.priority AS[Priority],
		                                                                    xr.rule_set_id,
		                                                                    xr.rule_id
                                                                    FROM[IVPSecMasterVendor].[dbo].[ivp_secmv_rule_mapping] rm
                                                                    INNER JOIN[IVPSecMasterVendor].[dbo].[ivp_secmv_feed_summary] fs
                                                                    ON ((rm.task_in_module = 'u' OR rm.task_in_module = 'o') AND(rm.rule_type_id = 1 OR rm.rule_type_id = 4) AND fs.feed_id = rm.rule_dependent_id)
                                                                    INNER JOIN[IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_master] vm
                                                                    ON(vm.vendor_id = fs.vendor_id)
                                                                    INNER JOIN[IVPSecMaster].[dbo].[ivp_rad_xrule] xr
                                                                    ON xr.rule_set_id = rm.rule_set_id
                                                                    WHERE vm.is_active = 'TRUE' AND fs.is_active = 'TRUE' AND xr.is_active = 1
                                                                    ORDER BY vm.vendor_name, fs.feed_name, [Rule Type], xr.priority", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetFeedRules -> End");
            }
        }

        public static ObjectTable GetFeedSectypeMapping()
        {
            try
            {
                mLogger.Debug("GetFeedSectypeMapping -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT vm.vendor_id,
			                                                                vm.vendor_name AS [Data Source],
			                                                                fs.feed_id,
			                                                                fs.feed_name AS [Feed Name],
			                                                                sm.sectype_id,
			                                                                sm.sectype_name AS [Security Type],
			                                                                fmm.sectype_feed_mapping_id
	                                                                FROM [IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_master] vm
	                                                                INNER JOIN[IVPSecMasterVendor].[dbo].[ivp_secmv_feed_summary] fs
	                                                                ON vm.vendor_id = fs.vendor_id
	                                                                INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_sectype_feed_mapping_master] fmm
	                                                                ON fs.feed_id = fmm.feed_id
	                                                                INNER JOIN [IVPSecMaster].[dbo].[ivp_secm_sectype_master] sm
	                                                                ON sm.sectype_id = fmm.sectype_id
	                                                                WHERE sm.is_active = 1 AND vm.is_active = 1 AND fs.is_active = 1", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetFeedSectypeMapping -> End");
            }
        }

        //public static ObjectTable GetBulkLicenseSetup()
        //{
        //    try
        //    {
        //        mLogger.Debug("GetBulkLicenseSetup -> Start");
        //        return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT  vm.vendor_id,
        //                                                              vm.vendor_name AS [Data Source],
        //                                                              fs.feed_id,
        //                                                              fs.feed_name AS [Feed Name],
        //                                                              ts.task_name AS [Loading Task Name],
        //                                                              ts.task_description AS [Loading Task Description],
        //                                                              ld.file_path AS [Bulk File Path],
        //                                                              dtbulk.date_display_name AS [Bulk File Date Type],
        //                                                              CASE WHEN ld.file_date_type = 5 THEN ld.file_date ELSE NULL END AS [Bulk File Date],
        //                                                              CASE WHEN ld.file_date_type = 4 THEN ld.file_date_days ELSE NULL END AS [Bulk File Business Days],
        //                                                              ld.file_path_diff AS [Diff File Path],
        //                                                              dtdiff.date_display_name AS [Diff File Date Type],
        //                                                              CASE WHEN ld.file_date_type_diff = 5 THEN ld.file_date_diff ELSE NULL END AS [Diff File Date],
        //                                                              CASE WHEN ld.file_date_type_diff = 4 THEN ld.file_date_days_diff ELSE NULL END AS [Diff File Business Days],
        //                                                              ld.check_existing AS [Check Existing Security],
        //                                                              flmd.feed_license_mapping_id,
        //                                                              ts.task_master_id,
        //                                                              ld.loading_details_id,
        //                                                              vm.vendor_type_id,
        //                                                              fs.feed_type_id
        //                                                            FROM [IVPSecMasterVendor].[dbo].[ivp_secmv_loading_task_details] ld
        //                                                            INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_task_summary] ts
        //                                                            ON ld.task_master_id = ts.task_master_id
        //                                                            INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_license_mapping_details] flmd
        //                                                            ON ts.specified_id = flmd.feed_license_mapping_id AND flmd.license_type_id = 1
        //                                                            INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_summary] fs
        //                                                            ON fs.feed_id = flmd.feed_id
        //                                                            INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_master] vm
        //                                                            ON vm.vendor_id = fs.vendor_id
        //                                                            LEFT JOIN [IVPRefMaster].[dbo].[ivp_refm_date_type] dtbulk
        //                                                            ON dtbulk.date_type_id = ld.file_date_type
        //                                                            LEFT JOIN [IVPRefMaster].[dbo].[ivp_refm_date_type] dtdiff
        //                                                            ON dtdiff.date_type_id = ld.file_date_type_diff
        //                                                            WHERE vm.is_active = 1 AND fs.is_active = 1 AND ts.is_active = 1
        //                                                            ORDER BY vm.vendor_name, fs.feed_name", ConnectionConstants.SecMaster_Connection).Tables[0];
        //    }
        //    catch (Exception ex)
        //    {
        //        mLogger.Debug(ex.ToString());
        //        throw;
        //    }
        //    finally
        //    {
        //        mLogger.Debug("GetBulkLicenseSetup -> End");
        //    }
        //}

        public static ObjectTable GetBulkLicenseSetup()
        {
            try
            {
                mLogger.Debug("GetBulkLicenseSetup -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT vm.vendor_id,
		                                                                    vm.vendor_name AS [Data Source],
		                                                                    fs.feed_id,
		                                                                    fs.feed_name AS [Feed Name],
                                                                            flmd.feed_license_mapping_id,
		                                                                    ISNULL(ts.task_master_id,0) AS task_master_id,
		                                                                    ISNULL(ltd.loading_details_id,0) AS loading_details_id
                                                                    FROM [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_license_mapping_details] flmd
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_summary] fs
                                                                    ON fs.feed_id = flmd.feed_id AND flmd.license_type_id = 1
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_master] vm
                                                                    ON vm.vendor_id = fs.vendor_id
                                                                    LEFT JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_task_summary] ts
                                                                    ON flmd.feed_license_mapping_id = ts.specified_id AND ts.task_type_id = 102 AND ts.is_active = 1
                                                                    LEFT JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_loading_task_details] ltd
                                                                    ON ltd.task_master_id = ts.task_master_id AND ltd.is_active = 1
                                                                    WHERE fs.is_active = 1 AND vm.is_active = 1
                                                                    ORDER BY vm.vendor_name, fs.feed_name", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetBulkLicenseSetup -> End");
            }
        }

        //public static ObjectTable GetAPILicenseSetup()
        //{
        //    try
        //    {
        //        mLogger.Debug("GetAPILicenseSetup -> Start");
        //        return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT  vm.vendor_id,
        //                                                              vm.vendor_name AS [Data Source],
        //                                                              fs.feed_id,
        //                                                              fs.feed_name AS [Feed Name],
        //                                                              ts.task_name AS [Import Task Name],
        //                                                              ts.task_description AS [Import Task Description],
        //                                                              ISNULL(vmm.vendor_management_name, 'Default') AS [Vendor Management Preference],
        //                                                              Tbl.Col.value('@value', 'varchar(MAX)') AS [Request Type],
        //                                                              ld.calling_vendor_type AS [Vendor Type],
        //                                                              ld.vendor_identifier_name AS [Vendor Identifier],
        //                                                              STUFF((
        //                                                               SELECT '|' + tdet.display_name
        //                                                               FROM [IVPSecMasterVendor].dbo.[ivp_secmv_api_ftp_task_details] ld1
        //                                                               CROSS APPLY IVPSecMaster.dbo.SECM_GetList2Table(ld.identifier_id, '|') attrs
        //                                                               INNER JOIN IVPSecMaster.dbo.ivp_secm_template_details tdet
        //                                                               ON tdet.template_id = 0 AND tdet.attribute_id = attrs.item AND tdet.is_active = 1 AND ld.api_ftp_details_id = ld1.api_ftp_details_id AND ld1.is_active = 1
        //                                                               FOR XML PATH('')
        //                                                               ), 1, 1, '') AS [Vendor Identifier Mapped Attribute],
        //                                                              Tbl1.Col.value('@value', 'varchar(MAX)') AS [Market Sector],
        //                                                              mtd.display_name AS [Market Sector Mapped Attribute],
        //                                                                    flmd.feed_license_mapping_id
        //                                                            FROM [IVPSecMasterVendor].[dbo].[ivp_secmv_api_ftp_task_details] ld
        //                                                            INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_task_summary] ts
        //                                                            ON ld.task_master_id = ts.task_master_id AND ts.task_type_id = 104
        //                                                            INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_license_mapping_details] flmd
        //                                                            ON ts.specified_id = flmd.feed_license_mapping_id AND flmd.license_type_id = 3
        //                                                            INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_summary] fs
        //                                                            ON fs.feed_id = flmd.feed_id
        //                                                            INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_master] vm
        //                                                            ON vm.vendor_id = fs.vendor_id
        //                                                            LEFT JOIN [IVPRefMasterVendor].[dbo].[ivp_rad_vendor_management_master] vmm
        //                                                            ON vmm.vendor_management_id = ld.vendor_management_id
        //                                                            CROSS APPLY xml_schema.nodes('//RBbgSecurityInfo/Parameter') AS Tbl(Col)
        //                                                            CROSS APPLY xml_schema.nodes('//RBbgSecurityInfo/Parameter/Collection/CollectionItem') AS Tbl1(Col)
        //                                                            LEFT JOIN IVPSecMaster.dbo.ivp_secm_template_details mtd
        //                                                            ON ld.market_sector_mapped_attribute = mtd.attribute_id AND mtd.template_id = 0 AND mtd.is_active = 1
        //                                                            WHERE ld.is_active = 1 AND ts.is_active = 1 AND fs.is_active = 1 AND vm.is_active = 1 AND ld.calling_vendor_type = 'Bloomberg'
        //                                                             AND Tbl.Col.value('@name', 'varchar(MAX)') = 'RequestType' AND Tbl1.Col.value('@name', 'varchar(MAX)') = 'MarketSector'
        //                                                            ORDER BY vm.vendor_name, fs.feed_name", ConnectionConstants.SecMaster_Connection).Tables[0];
        //    }
        //    catch (Exception ex)
        //    {
        //        mLogger.Debug(ex.ToString());
        //        throw;
        //    }
        //    finally
        //    {
        //        mLogger.Debug("GetAPILicenseSetup -> End");
        //    }
        //}

        public static ObjectTable GetAPILicenseSetup()
        {
            try
            {
                mLogger.Debug("GetAPILicenseSetup -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT vm.vendor_id,
		                                                                    vm.vendor_name AS [Data Source],
		                                                                    fs.feed_id,
		                                                                    fs.feed_name AS [Feed Name],
                                                                            flmd.feed_license_mapping_id,
                                                                            ISNULL(ts.task_master_id,0) AS task_master_id
                                                                    FROM [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_license_mapping_details] flmd
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_summary] fs
                                                                    ON fs.feed_id = flmd.feed_id AND flmd.license_type_id = 3
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_master] vm
                                                                    ON vm.vendor_id = fs.vendor_id
                                                                    LEFT JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_task_summary] ts
                                                                    ON flmd.feed_license_mapping_id = ts.specified_id AND ts.task_type_id = 104 AND ts.is_active = 1
                                                                    WHERE fs.is_active = 1 AND vm.is_active = 1
                                                                    ORDER BY vm.vendor_name, fs.feed_name", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetAPILicenseSetup -> End");
            }
        }

        //public static ObjectTable GetFTPLicenseSetup()
        //{
        //    try
        //    {
        //        mLogger.Debug("GetFTPLicenseSetup -> Start");
        //        return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT  vm.vendor_id,
        //                                                              vm.vendor_name AS [Data Source],
        //                                                              fs.feed_id,
        //                                                              fs.feed_name AS [Feed Name],
        //                                                              req.calling_vendor_type AS [Vendor Type],
        //                                                              ISNULL(vmm.vendor_management_name, 'Default') AS [Vendor Management Preference],
        //                                                              req.task_name AS [Request Task Name],
        //                                                              req.task_description AS [Request Task Description],
        //                                                              req.vendor_identifier_name AS [Vendor Identifier],
        //                                                              STUFF((
        //                                                               SELECT '|' + tdet.display_name
        //                                                               FROM [IVPSecMasterVendor].dbo.[ivp_secmv_api_ftp_task_details] ld1
        //                                                               CROSS APPLY IVPSecMaster.dbo.SECM_GetList2Table(req.identifier_id, '|') attrs
        //                                                               INNER JOIN IVPSecMaster.dbo.ivp_secm_template_details tdet
        //                                                               ON tdet.template_id = 0 AND tdet.attribute_id = attrs.item AND tdet.is_active = 1 AND req.api_ftp_details_id = ld1.api_ftp_details_id AND ld1.is_active = 1
        //                                                               FOR XML PATH('')
        //                                                               ), 1, 1, '') AS [Vendor Identifier Mapped Attribute],
        //                                                              TblReq1.Col.value('@value', 'varchar(MAX)') AS [Market Sector],
        //                                                              mtd.display_name AS [Market Sector Mapped Attribute],
        //                                                              TblReq.Col.value('@value', 'varchar(MAX)') AS [Request Transport Type],
        //                                                              req.outgoing_transport AS [Outgoing FTP],
        //                                                              btd.display_name AS [BVAL Mapped Attribute],
        //                                                              res.task_name AS [Response Task Name],
        //                                                              res.task_description AS [Response Task Description],
        //                                                              TblRes.Col.value('@value', 'varchar(MAX)') AS [Response Transport Type],
        //                                                              res.incoming_transport AS [Incoming FTP],
        //                                                              req.feed_license_mapping_id
        //                                                            FROM 
        //                                                            (
        //                                                             SELECT ts.task_name,ts.task_description,flmd.*,ld.*
        //                                                             FROM [IVPSecMasterVendor].[dbo].[ivp_secmv_api_ftp_task_details] ld
        //                                                             INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_task_summary] ts
        //                                                             ON ld.task_master_id = ts.task_master_id AND ts.task_type_id = 105
        //                                                             INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_license_mapping_details] flmd
        //                                                             ON ts.specified_id = flmd.feed_license_mapping_id AND flmd.license_type_id = 2
        //                                                             WHERE ld.is_active = 1 AND ts.is_active = 1 AND ld.calling_vendor_type = 'Bloomberg'
        //                                                            ) req
        //                                                            INNER JOIN
        //                                                            (
        //                                                             SELECT ts.task_name,ts.task_description,flmd.*,ld.*
        //                                                             FROM [IVPSecMasterVendor].[dbo].[ivp_secmv_api_ftp_task_details] ld
        //                                                             INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_task_summary] ts
        //                                                             ON ld.task_master_id = ts.task_master_id AND ts.task_type_id = 108
        //                                                             INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_license_mapping_details] flmd
        //                                                             ON ts.specified_id = flmd.feed_license_mapping_id AND flmd.license_type_id = 2
        //                                                             WHERE ld.is_active = 1 AND ts.is_active = 1 AND ld.calling_vendor_type = 'Bloomberg'
        //                                                            ) res
        //                                                            ON req.feed_license_mapping_id = res.feed_license_mapping_id
        //                                                            INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_summary] fs
        //                                                            ON fs.feed_id = req.feed_id
        //                                                            INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_master] vm
        //                                                            ON vm.vendor_id = fs.vendor_id
        //                                                            LEFT JOIN [IVPRefMasterVendor].[dbo].[ivp_rad_vendor_management_master] vmm
        //                                                            ON vmm.vendor_management_id = req.vendor_management_id
        //                                                            CROSS APPLY req.xml_schema.nodes('//RBbgSecurityInfo/Parameter') AS TblReq(Col)
        //                                                            CROSS APPLY res.xml_schema.nodes('//RBbgSecurityInfo/Parameter') AS TblRes(Col)
        //                                                            CROSS APPLY req.xml_schema.nodes('//RBbgSecurityInfo/Parameter/Collection/CollectionItem') AS TblReq1(Col)
        //                                                            LEFT JOIN IVPSecMaster.dbo.ivp_secm_template_details mtd
        //                                                            ON req.market_sector_mapped_attribute = mtd.attribute_id AND mtd.template_id = 0 AND mtd.is_active = 1
        //                                                            LEFT JOIN IVPSecMaster.dbo.ivp_secm_template_details btd
        //                                                            ON req.bval_mapped_attribute = btd.attribute_id AND btd.template_id = 0 AND mtd.is_active = 1
        //                                                            WHERE fs.is_active = 1 AND vm.is_active = 1 
        //                                                             AND TblReq.Col.value('@name', 'varchar(MAX)') = 'RequestType' AND TblRes.Col.value('@name', 'varchar(MAX)') = 'RequestType' AND TblReq1.Col.value('@name', 'varchar(MAX)') = 'MarketSector'
        //                                                            ORDER BY vm.vendor_name, fs.feed_name", ConnectionConstants.SecMaster_Connection).Tables[0];
        //    }
        //    catch (Exception ex)
        //    {
        //        mLogger.Debug(ex.ToString());
        //        throw;
        //    }
        //    finally
        //    {
        //        mLogger.Debug("GetFTPLicenseSetup -> End");
        //    }
        //}

        public static ObjectTable GetFTPLicenseSetup()
        {
            try
            {
                mLogger.Debug("GetFTPLicenseSetup -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT vm.vendor_id,
		                                                                    vm.vendor_name AS [Data Source],
		                                                                    fs.feed_id,
		                                                                    fs.feed_name AS [Feed Name],
                                                                            flmd.feed_license_mapping_id,
                                                                            ISNULL(ts.task_master_id,0) AS [request_task_master_id],
                                                                            ISNULL(ts1.task_master_id,0) AS [response_task_master_id]
                                                                    FROM [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_license_mapping_details] flmd
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_summary] fs
                                                                    ON fs.feed_id = flmd.feed_id AND flmd.license_type_id = 2
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_master] vm
                                                                    ON vm.vendor_id = fs.vendor_id
                                                                    LEFT JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_task_summary] ts
                                                                    ON flmd.feed_license_mapping_id = ts.specified_id AND ts.task_type_id = 105 AND ts.is_active = 1
                                                                    LEFT JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_task_summary] ts1
                                                                    ON flmd.feed_license_mapping_id = ts1.specified_id AND ts1.task_type_id = 108 AND ts1.is_active = 1    
                                                                    WHERE fs.is_active = 1 AND vm.is_active = 1
                                                                    ORDER BY vm.vendor_name, fs.feed_name", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetFTPLicenseSetup -> End");
            }
        }

        public static ObjectTable GetLoadFromVendorLicenseSetup()
        {
            try
            {
                mLogger.Debug("GetLoadFromVendorLicenseSetup -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT  vm.vendor_id,
		                                                                    vm.vendor_name AS [Data Source],
		                                                                    fs.feed_id,
		                                                                    fs.feed_name AS [Feed Name],
		                                                                    flmd.feed_license_mapping_id,
		                                                                    ISNULL(ts.task_master_id,0) AS task_master_id
                                                                    FROM [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_license_mapping_details] flmd
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_summary] fs
                                                                    ON fs.feed_id = flmd.feed_id AND flmd.license_type_id = 4
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_master] vm
                                                                    ON vm.vendor_id = fs.vendor_id
                                                                    LEFT JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_task_summary] ts
                                                                    ON flmd.feed_license_mapping_id = ts.specified_id AND ts.task_type_id = 119 AND ts.is_active = 1
                                                                    WHERE fs.is_active = 1 AND vm.is_active = 1
                                                                    ORDER BY vm.vendor_name, fs.feed_name", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetLoadFromVendorLicenseSetup -> End");
            }
        }

        public static ObjectTable GetAPIFTPHeaders()
        {
            try
            {
                mLogger.Debug("GetAPIFTPHeaders -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT  vm.vendor_id,
		                                                                    vm.vendor_name AS [Data Source],
		                                                                    fs.feed_id,
		                                                                    fs.feed_name AS [Feed Name],
		                                                                    CASE WHEN ts.task_type_id = 104 THEN 'API' ELSE 'FTP' END AS [License Type],
		                                                                    rh.header_name AS [Header Name],
		                                                                    rh.header_value AS [Header Value]
                                                                    FROM [IVPSecMasterVendor].[dbo].[ivp_secmv_api_ftp_task_details] ld
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_task_summary] ts
                                                                    ON ld.task_master_id = ts.task_master_id AND ts.task_type_id IN (104, 105)
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_license_mapping_details] flmd
                                                                    ON ts.specified_id = flmd.feed_license_mapping_id AND flmd.license_type_id IN (2, 3)
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_bbg_request_headers] rh
                                                                    ON rh.api_ftp_details_id = ld.api_ftp_details_id
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_summary] fs
                                                                    ON fs.feed_id = flmd.feed_id
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_master] vm
                                                                    ON vm.vendor_id = fs.vendor_id
                                                                    WHERE ld.is_active = 1 AND ts.is_active = 1 AND ld.calling_vendor_type = 'Bloomberg'", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetAPIFTPHeaders -> End");
            }
        }

        public static ObjectTable GetAPIFTPLicense()
        {
            try
            {
                mLogger.Debug("GetAPIFTPLicense -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT  vm.vendor_id,
		                                                                    vm.vendor_name AS [Data Source],
		                                                                    fs.feed_id,
		                                                                    fs.feed_name AS [Feed Name],
		                                                                    CASE WHEN ts.task_type_id = 104 THEN 'API' ELSE 'FTP' END AS [License Type]
                                                                    FROM [IVPSecMasterVendor].[dbo].[ivp_secmv_api_ftp_task_details] ld
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_task_summary] ts
                                                                    ON ld.task_master_id = ts.task_master_id AND ts.task_type_id IN (104, 105)
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_license_mapping_details] flmd
                                                                    ON ts.specified_id = flmd.feed_license_mapping_id AND flmd.license_type_id IN (2, 3)
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_summary] fs
                                                                    ON fs.feed_id = flmd.feed_id
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_master] vm
                                                                    ON vm.vendor_id = fs.vendor_id
                                                                    WHERE ld.is_active = 1 AND ts.is_active = 1 AND fs.is_active = 1 AND vm.is_active = 1 AND ld.calling_vendor_type = 'Bloomberg'", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetAPIFTPLicense -> End");
            }
        }

        public static ObjectTable GetBULKLicense()
        {
            try
            {
                mLogger.Debug("GetBULKLicense -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT  vm.vendor_id,
                                                                      vm.vendor_name AS [Data Source],
                                                                      fs.feed_id,
                                                                      fs.feed_name AS [Feed Name],
                                                                      ts.task_name AS [Loading Task Name],
                                                                      ltd.loading_details_id,
                                                                            ts.task_master_id
                                                                    FROM [IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_master] vm
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_summary] fs
                                                                    ON vm.vendor_id = fs.vendor_id
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_license_mapping_details] flmd
                                                                    ON flmd.feed_id = fs.feed_id AND flmd.license_type_id = 1
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_task_summary] ts
                                                                    ON ts.specified_id = flmd.feed_license_mapping_id
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_loading_task_details] ltd
                                                                    ON ltd.task_master_id = ts.task_master_id
                                                                    WHERE vm.is_active = 1 AND fs.is_active = 1 AND ts.is_active = 1 AND ltd.is_active = 1
                                                                    ORDER BY vm.vendor_name, fs.feed_name", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetBULKLicense -> End");
            }
        }

        public static ObjectTable GetCustomClass()
        {
            try
            {
                mLogger.Debug("GetCustomClass -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT  vm.vendor_id,
		                                                                    vm.vendor_name AS [Data Source],
		                                                                    fs.feed_id,
		                                                                    fs.feed_name AS [Feed Name],
		                                                                    ts.task_name AS [Loading Task Name],
		                                                                    cc.custom_class_id,
		                                                                    cc.call_type AS [Call Type],
		                                                                    CASE WHEN cc.class_type = 1 THEN 'Script Executable' ELSE 'Custom Class' END AS [Class Type],
		                                                                    cc.class_name AS [Script-Class Name],
		                                                                    cc.assembly_path AS [Assembly Path],
		                                                                    cc.exec_sequence AS [Sequence Number]
                                                                    FROM [IVPSecMasterVendor].[dbo].[ivp_secmv_vendor_master] vm
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_summary] fs
                                                                    ON vm.vendor_id = fs.vendor_id
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_feed_license_mapping_details] flmd
                                                                    ON flmd.feed_id = fs.feed_id AND flmd.license_type_id = 1
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_task_summary] ts
                                                                    ON ts.specified_id = flmd.feed_license_mapping_id
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_custom_class] cc
                                                                    ON cc.task_master_id = ts.task_master_id
                                                                    INNER JOIN [IVPSecMasterVendor].[dbo].[ivp_secmv_loading_task_details] ltd
                                                                    ON ltd.task_master_id = ts.task_master_id
                                                                    WHERE vm.is_active = 1 AND fs.is_active = 1 AND ts.is_active = 1 AND cc.is_active = 1 AND ltd.is_active = 1
                                                                    ORDER BY vm.vendor_name, fs.feed_name", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetCustomClass -> End");
            }
        }

        public static ObjectTable GetValidationTask()
        {
            try
            {
                mLogger.Debug("GetValidationTask -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT ts.task_name AS [Task Name],
		                                                                    ts.task_description AS [Task Description],
		                                                                    vtd.is_validation AS [Apply Validation],
		                                                                    vtd.is_uniqueness AS [Apply Uniqueness],
		                                                                    vtd.is_alert AS [Apply Alert],
		                                                                    vtd.delete_previous_exceptions AS [Delete Previous Exceptions],
		                                                                    vtd.calendar_id,
		                                                                    CASE WHEN vtd.start_date_type = 11 THEN 'LastRunDate' ELSE sdt.date_display_name END AS [Start Date Type],
			                                                                CASE WHEN vtd.start_date_type = 5 THEN vtd.custom_start_date ELSE NULL END AS [Custom Start Date],
			                                                                CASE WHEN vtd.start_date_type = 4 THEN vtd.start_date_days ELSE NULL END AS [Start Date Business Days],
			                                                                edt.date_display_name AS [End Date Type],
			                                                                CASE WHEN vtd.end_date_type = 5 THEN vtd.custom_end_date ELSE NULL END AS [Custom End Date],
			                                                                CASE WHEN vtd.end_date_type = 4 THEN vtd.end_date_days ELSE NULL END AS [End Date Business Days],
		                                                                    vtd.start_date_type AS start_date_type_id, 
		                                                                    vtd.end_date_type AS end_date_type_id,
		                                                                    vtd.validation_details_id,
		                                                                    ts.task_master_id
                                                                    FROM IVPSecMasterVendor.dbo.ivp_secmv_validation_task_details vtd
                                                                    INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_task_summary ts
                                                                    ON vtd.task_master_id = ts.task_master_id
                                                                    LEFT JOIN [IVPRefMaster].[dbo].[ivp_refm_date_type] sdt
                                                                    ON sdt.date_type_id = vtd.start_date_type AND vtd.start_date_type <> 11
                                                                    LEFT JOIN [IVPRefMaster].[dbo].[ivp_refm_date_type] edt
                                                                    ON edt.date_type_id = vtd.end_date_type
                                                                    WHERE ts.is_active = 1 AND vtd.is_active = 1
                                                                    ORDER BY ts.task_name", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetValidationTask -> End");
            }
        }

        public static ObjectTable GetValidationTaskSectypes()
        {
            try
            {
                mLogger.Debug("GetValidationTaskSectypes -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT ts.task_name AS [Task Name],
		                                                                    sm.sectype_name AS [Security Type]
                                                                    FROM IVPSecMasterVendor.dbo.ivp_secmv_validation_task_details vtd
                                                                    INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_task_summary ts
                                                                    ON vtd.task_master_id = ts.task_master_id
                                                                    CROSS APPLY IVPSecMaster.dbo.SECM_GetList2Table(vtd.sectype_ids, ',') tab
                                                                    INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_master sm
                                                                    ON sm.sectype_id = tab.item
                                                                    WHERE ts.is_active = 1 AND vtd.is_active = 1 AND sm.is_active = 1", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetValidationTaskSectypes -> End");
            }
        }

        public static ObjectSet GetFeedDateFormats()
        {
            try
            {
                mLogger.Debug("GetFeedDateFormats -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT det.format,det.format_details_id
                                                                            FROM IVPSecMasterVendor.dbo.ivp_secmv_field_format_master mas
                                                                            INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_field_format_details det
                                                                            ON mas.format_master_id = det.format_master_id
                                                                            WHERE det.format_master_id = 1

                                                                            SELECT det.format,det.format_details_id
                                                                            FROM IVPSecMasterVendor.dbo.ivp_secmv_field_format_master mas
                                                                            INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_field_format_details det
                                                                            ON mas.format_master_id = det.format_master_id
                                                                            WHERE det.format_master_id = 2

                                                                            SELECT det.format,det.format_details_id
                                                                            FROM IVPSecMasterVendor.dbo.ivp_secmv_field_format_master mas
                                                                            INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_field_format_details det
                                                                            ON mas.format_master_id = det.format_master_id
                                                                            WHERE det.format_master_id = 3

                                                                            SELECT det.format,det.format_details_id
                                                                            FROM IVPSecMasterVendor.dbo.ivp_secmv_field_format_master mas
                                                                            INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_field_format_details det
                                                                            ON mas.format_master_id = det.format_master_id", ConnectionConstants.SecMaster_Connection);
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetFeedDateFormats -> End");
            }
        }

        public static ObjectTable GetReferenceAttributes()
        {
            try
            {
                mLogger.Debug("GetReferenceAttributes -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT et.entity_type_id AS reference_type_id,
		                                                                    ea.entity_attribute_id AS reference_attribute_id,
		                                                                    ea.attribute_name AS reference_attribute_name,
		                                                                    ea.display_name AS reference_display_name
                                                                    FROM IVPRefMaster.dbo.ivp_refm_entity_type et
                                                                    INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute ea
                                                                    ON et.entity_type_id = ea.entity_type_id
                                                                    WHERE et.is_active = 1 AND ea.is_active = 1", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetReferenceAttributes -> End");
            }
        }

        public static ObjectTable GetUnderlierIdentifierAttributes()
        {
            try
            {
                mLogger.Debug("GetUnderlierIdentifierAttributes -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT  attr.attribute_id,attribute_name,display_name 
	                                                                FROM [ivpsecmaster].[dbo].[ivp_secm_attribute_details] attr (NOLOCK)
		                                                                INNER JOIN [ivpsecmaster].[dbo].[ivp_secm_sectype_table] tbl (NOLOCK)
			                                                                ON (tbl.sectype_table_id = attr.sectype_table_id)
		                                                                INNER JOIN [ivpsecmaster].[dbo].[ivp_secm_template_details] tem (NOLOCK)
			                                                                ON (tem.attribute_id = attr.attribute_id AND template_id = 0)
		                                                                INNER JOIN [ivpsecmaster].[dbo].[ivp_secm_template_master] mas (NOLOCK)
			                                                                ON (tem.template_id = mas.template_id AND mas.dependent_id = 'SYSTEM' AND mas.is_active = 1)
		                                                                INNER JOIN [ivpsecmaster].[dbo].[ivp_secm_template_dependent_type] typ (NOLOCK)
			                                                                ON (mas.dependent_type_id = typ.dependent_type_id AND typ.dependent_type_name = 'SYSTEM')
	                                                                AND priority IN (1,2)", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetUnderlierIdentifierAttributes -> End");
            }
        }

        public static ObjectTable GetBBGExoticFieldNames()
        {
            try
            {
                mLogger.Debug("GetExoticFieldNames -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT ltrim(rtrim(market_sector)) + ' - ' + ltrim(rtrim(request_field)) AS exotic_name, ltrim(rtrim(tab.item)) AS exotic_field
                                                                    FROM [IVPRAD].[dbo].[ivp_rad_bbg_bulklist_mapping] bbg
                                                                    CROSS APPLY IVPSecMaster.dbo.SECM_GetList2TableWithRowID(bbg.output_fields, ',') tab", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetExoticFieldNames -> End");
            }
        }

        public static DataTable GetRADTransportsForReportingTask()
        {
            try
            {
                mLogger.Debug("GetRADTransportsForReportingTask -> Start");
                return CommonDALWrapper.ExecuteSelectQuery(@"SELECT transport_type_name, transport_name FROM [ivprad].[dbo].[ivp_rad_transport_type_master] mas INNER JOIN[ivprad].[dbo].[ivp_rad_transport_config_details] det ON mas.transport_type_id = det.transport_type_id WHERE is_active = 1 AND transport_type_name IN('RABBITMQ', 'KAFKAMQ', 'MSMQ', 'FTP', 'WFT', 'SFTP')", ConnectionConstants.SecMaster_Connection).Tables[0];//.AsEnumerable().GroupBy(x=>x.Field<string>("transport_type_name"), StringComparer.OrdinalIgnoreCase).ToDictionary(x=>x.Key, y=>y.Select(z=>z.Field<string>("transport_name")).ToList(), StringComparer.OrdinalIgnoreCase)
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetRADTransportsForReportingTask -> End");
            }

        }

        public static List<string> GetDownstreamSystems()
        {
            try
            {
                mLogger.Debug("GetDownstreamSystems -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT system_name FROM [ivpsecmaster].[dbo].[ivp_secm_downstream_system] WHERE is_active = 1", ConnectionConstants.SecMaster_Connection).Tables[0].AsEnumerable().Select(x => x.Field<string>("system_name")).ToList();
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetDownstreamSystems -> End");
            }

        }

        public static List<string> GetRADEmailIds()
        {
            try
            {
                mLogger.Debug("GetRADEmailIds -> Start");
                return new RUserManagementService().GetAllUsersGDPR().Select(info => info.FullName + " <" + info.EmailId + ">").ToList();
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetRADEmailIds -> End");
            }

        }

        public static ObjectTable GetReportsForDownstreamTasks()
        {
            try
            {
                mLogger.Debug("GetReportsForDownstreamTasks -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT rset.report_name,rset.is_system,rep.report_type,CAST(0 AS BIT) AS is_custom
                                                                FROM [ivpsecmaster].[dbo].[ivp_secm_report_setup] rset
                                                                INNER JOIN [ivpsecmaster].[dbo].[ivp_secm_reports] rep
                                                                ON rep.report_id = rset.report_id
                                                                WHERE rset.is_active = 1
                                                                UNION ALL
                                                                SELECT custom_report_name AS report_name,CAST(0 AS BIT),'Custom Reports',CAST(1 AS BIT) AS is_custom
                                                                FROM [IVPSecMaster].[dbo].[ivp_secm_custom_reports]
                                                                WHERE is_active = 1", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetReportsForDownstreamTasks -> End");
            }

        }

        public static ObjectTable GetDownstreamTasks()
        {
            try
            {
                mLogger.Debug("GetDownstreamTasks -> Start");
                return CommonDALWrapper.ExecuteSelectQueryObject(@"SELECT ts.task_name, ts.task_master_id
                                                                    FROM IVPSecMasterVendor.dbo.ivp_secmv_reporting_task_details rtd
                                                                    INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_task_summary ts
                                                                    ON rtd.task_master_id = ts.task_master_id AND ts.task_type_id = 106
                                                                    WHERE rtd.is_active = 1 AND ts.is_active = 1", ConnectionConstants.SecMaster_Connection).Tables[0];
            }
            catch (Exception ex)
            {
                mLogger.Debug(ex.ToString());
                throw;
            }
            finally
            {
                mLogger.Debug("GetDownstreamTasks -> End");
            }
        }
    }
}
