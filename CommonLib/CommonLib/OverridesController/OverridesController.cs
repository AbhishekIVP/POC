using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.ivp.rad.utils;
using com.ivp.rad.common;
using System.Data;
using com.ivp.commom.commondal;
using System.Xml.Linq;
using com.ivp.refmaster.refmasterwebservices;
using com.ivp.refmaster.common;
using System.Runtime.Serialization;
using com.ivp.rad.dal;
using com.ivp.srmcommon;
using com.ivp.rad.viewmanagement;

namespace com.ivp.common
{
    public static class OverridesController
    {
        #region private member variables
        static IRLogger mLogger = RLogFactory.CreateLogger("OverridesController");
        #endregion

        public static DataSet GetOverridesDataSM(string username)
        {
            mLogger.Debug("OverridesController : Start -> GetOverridesDataSM");
            RHashlist mHList = new RHashlist();
            try
            {
                DataSet overridesDS = CommonDALWrapper.ExecuteSelectQuery("EXEC [SECM_GetOverrideStatusDetails] '" + username + "'", ConnectionConstants.SecMaster_Connection);
                return overridesDS;
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
                mLogger.Debug("OverridesController : End -> GetOverridesDataSM");
            }
        }

        private static DataSet GetOverridableAttributes(DataSet input, List<string> selectedSecIds)
        {
            if (input != null && input.Tables.Count >= 3 && input.Tables[2].Rows.Count > 0 && selectedSecIds != null && selectedSecIds.Count > 0)
            {
                HashSet<string> shortCodes = new HashSet<string>();
                foreach (var item in selectedSecIds)
                {
                    shortCodes.Add(item.Substring(0, 4));
                }

                HashSet<string> finalAttributeIds = null;

                DataSet dsSecId = CommonDALWrapper.ExecuteSelectQuery(string.Format("select sectype_id, sectype_short_name from IVPSecMaster.dbo.ivp_secm_sectype_master where sectype_short_name in ({0})", string.Join(",", shortCodes.Select(x => "'" + x + "'"))), ConnectionConstants.SecMaster_Connection);

                HashSet<string> secTypeIds = new HashSet<string>();
                if (dsSecId != null && dsSecId.Tables.Count > 0 && dsSecId.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow item in dsSecId.Tables[0].Rows)
                    {
                        secTypeIds.Add(Convert.ToString(item["sectype_id"]));
                    }
                }

                foreach (string secTypeId in secTypeIds)
                {
                    HashSet<string> attrIdsWithOverrideConfigured = new HashSet<string>();
                    DataTable dt = CommonDALWrapper.ExecuteSelectQuery(@"DECLARE @sectype_id INT = " + secTypeId + @",
		                                                                        @system_template_id INT = 0

                                                                        SELECT @system_template_id = template_id
                                                                        FROM IVPSecMaster.dbo.ivp_secm_template_master
                                                                        WHERE sectype_id = @sectype_id AND dependent_type_id = 1 AND dependent_id = 'SYSTEM'

                                                                        SELECT attr.attribute_id, tdet.display_name
                                                                        FROM
                                                                        (
	                                                                        SELECT fmdet.sectype_attribute_id AS 'attribute_id'
	                                                                        FROM IVPSecMasterVendor.dbo.ivp_secmv_sectype_feed_mapping_master fmmas
	                                                                        INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_sectype_feed_mapping_details fmdet
		                                                                        ON fmdet.sectype_feed_mapping_id = fmmas.sectype_feed_mapping_id AND fmdet.is_active = 1 AND ISNULL(fmdet.is_additional_leg, 0) = 0 AND fmmas.sectype_id = @sectype_id AND fmmas.is_active = 1
	                                                                        INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_feed_summary fsum
		                                                                        ON fsum.feed_id = fmmas.feed_id AND fsum.is_active = 1
	                                                                        INNER JOIN IVPSecMasterVendor.dbo.ivp_secmv_vendor_master vmas
		                                                                        ON vmas.vendor_id = fsum.vendor_id AND vmas.is_active = 1
	                                                                        UNION
	                                                                        SELECT rules.attribute_id AS 'attribute_id'
	                                                                        FROM ( 
	                                                                        SELECT DISTINCT srule.sectype_id, srule.attribute_id
	                                                                        FROM IVPSecMaster.dbo.ivp_secm_sectype_rule_mapping srule
	                                                                        INNER JOIN IVPSecMaster.dbo.ivp_rad_xrule xru
		                                                                        ON xru.rule_set_id = srule.ruleset_id AND (srule.ruletype_id = 1 OR srule.ruletype_id = 2) AND ISNULL(srule.is_additional_leg, 0) = 0 AND xru.is_active = 1 AND xru.rule_state = 1
	                                                                        )rules
	                                                                        WHERE rules.sectype_id = @sectype_id
                                                                        ) attr
                                                                        INNER JOIN IVPSecMaster.dbo.ivp_secm_template_details tdet
	                                                                        ON tdet.template_id = @system_template_id AND tdet.attribute_id = attr.attribute_id", ConnectionConstants.SecMaster_Connection).Tables[0];

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            attrIdsWithOverrideConfigured.Add(Convert.ToString(row["attribute_id"]));
                        }
                    }

                    if (finalAttributeIds == null)
                    {
                        finalAttributeIds = attrIdsWithOverrideConfigured;
                    }
                    else
                    {
                        finalAttributeIds.IntersectWith(attrIdsWithOverrideConfigured);
                    }
                }

                DataTable attributeTable = input.Tables[2];
                HashSet<DataRow> rowsToRemove = new HashSet<DataRow>();

                foreach (DataRow row in attributeTable.Rows)
                {
                    string attrId = Convert.ToString(row["attribute_id"]);
                    if (!finalAttributeIds.Contains(attrId))
                    {
                        rowsToRemove.Add(row);
                    }
                }

                if (rowsToRemove.Count > 0)
                {
                    foreach (var item in rowsToRemove)
                    {
                        attributeTable.Rows.Remove(item);
                    }
                }

            }
            return input;
        }

        public static DataSet GetOverridesSecutiyDataSM(string username, List<string> selectedSecIds, bool getOverrideAttributes = false)
        {
            mLogger.Debug("OverridesController : Start -> GetOverridesSecutiyDataSM");
            RHashlist mHList = new RHashlist();
            try
            {
                DataSet overridesDS = CommonDALWrapper.ExecuteSelectQuery("EXEC SECM_GetOverrideSecurityDataDetails @secid_list = '" + String.Join(",", selectedSecIds) + "' ,@username = '" + username + "'", ConnectionConstants.SecMaster_Connection);
                if (getOverrideAttributes)
                    overridesDS = GetOverridableAttributes(overridesDS, selectedSecIds);
                return overridesDS;
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
                mLogger.Debug("OverridesController : End -> GetOverridesSecutiyDataSM");
            }
        }

        public static DataSet GetOverridesEntityDataRM(List<string> selectedEntityIds, int entityTypeID)
        {
            mLogger.Debug("OverridesController : Start -> GetOverridesEntityDataRM");
            RHashlist mHList = new RHashlist();
            try
            {
                DataSet overridesDS = new DataSet();
                RMRefMasterAPI obj = new RMRefMasterAPI();
                //Method to fetch Grid Data
                DataSet searchDs = obj.SearchEntityData(entityTypeID, selectedEntityIds);
                overridesDS.Tables.Add(searchDs.Tables[0].Copy());
                //Method to fetch Attributes
                DataSet attrDs = new RMAttributeOverride().GetOverridableAttributes(entityTypeID);
                overridesDS.Tables.Add(attrDs.Tables[0].Copy());

                return overridesDS;
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
                mLogger.Debug("OverridesController : End -> GetOverridesEntityDataRM");
            }
        }

        public static DataSet GetOverridesDataRM(string username, string dateCulture, string dateCultureLong, int ModuleId = 0)
        {
            mLogger.Debug("OverridesController : Start -> GetOverridesDataRM");
            RHashlist mHList = new RHashlist();
            try
            {
                string query = @"
                    IF EXISTS(SELECT * FROM tempdb.dbo.sysobjects WHERE ID = OBJECT_ID(N'tempdb..#eTypesTemplate')) 
		            BEGIN DROP TABLE #eTypesTemplate END

                    CREATE TABLE #eTypesTemplate(entity_type_id INT,entity_display_name VARCHAR(MAX), template_id INT)
	                INSERT INTO #eTypesTemplate (entity_type_id,entity_display_name) 
	                SELECT DISTINCT et.entity_type_id, et.entity_display_name
                    FROM IVPRefMaster.dbo.ivp_refm_entity_type etyp
                    INNER JOIN dbo.REFM_GetUserEntityTypes('" + username + @"', " + ModuleId + @") et
                    ON et.entity_type_id = etyp.entity_type_id
                    INNER JOIN IVPRefMaster.dbo.ivp_refm_overridden_attributes eC
                    ON(etyp.entity_code = SUBSTRING(eC.entity_code, 0, 5))

                    UPDATE eT
                    SET eT.template_id = dbo.REFM_GetUserTemplate(eT.entity_type_id, '" + username + @"', 'UPDATE')
                    FROM #eTypesTemplate eT
		

                    SELECT oattr.entity_code AS[Entity Code], etyp.entity_display_name AS[Entity Type], display_name AS[Attribute Name], locked_until, 
                                                    oattr.last_modified_by AS[Overridden By],
                                                    oattr.last_modified_on
                                                    FROM IVPRefMaster.dbo.ivp_refm_overridden_attributes oattr
                                                    INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute eattr
                                                    ON(oattr.attribute_id = eattr.entity_attribute_id)
                                                    INNER JOIN #eTypesTemplate etyp
								                    ON(etyp.entity_type_id = eattr.entity_type_id)
                                                    INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute_tab atab
                                                    ON atab.template_id = etyp.template_id AND atab.entity_attribute_id = eattr.entity_attribute_id AND atab.is_visible = 1
                                                    WHERE oattr.in_workflow = 0 and(locked_until is null or CONVERT(DATE, locked_until) >= CONVERT(DATE, GETDATE()))


                    IF EXISTS(SELECT * FROM tempdb.dbo.sysobjects WHERE ID = OBJECT_ID(N'tempdb..#eTypesTemplate')) 
		                    BEGIN DROP TABLE #eTypesTemplate END ";
                

                //WHERE data_type <> 'Lookup'";
                DataSet overridesDS = CommonDALWrapper.ExecuteSelectQuery(query, ConnectionConstants.RefMaster_Connection);
                overridesDS.Tables[0].Columns.Add("row_id", typeof(int));
                overridesDS.Tables[0].Columns.Add("Expiring On", typeof(string));
                overridesDS.Tables[0].Columns.Add("Overridden On", typeof(string));
                if (overridesDS.Tables.Count > 0 && overridesDS != null)
                {
                    List<string> userNameColumnsList = new List<string>();
                    userNameColumnsList.Add("Overridden By");
                    SRMCommonRAD.ConvertLoginNameToDisplayName(overridesDS.Tables[0].AsEnumerable(), userNameColumnsList, false, false);
                }

                int a = 0;
                foreach (DataRow row in overridesDS.Tables[0].Rows)
                {
                    row["row_id"] = a;
                    a++;
                    row["Expiring On"] = string.IsNullOrEmpty(Convert.ToString(row["locked_until"])) ? "Never" : Convert.ToDateTime(row["locked_until"]).ToString(dateCulture);
                    row["Overridden On"] = string.IsNullOrEmpty(Convert.ToString(row["last_modified_on"])) ? "" : Convert.ToDateTime(row["last_modified_on"]).ToString(dateCultureLong);
                }
                return overridesDS;
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
                mLogger.Debug("OverridesController : End -> GetOverridesDataRM");
            }
        }

        public static string SMDeleteOverride(Dictionary<int, string> deleteInfo)
        {
            mLogger.Debug("OverridesController : Start -> SMDeleteOverride");
            RHashlist mHList = new RHashlist();
            string response = "error";
            RDBConnectionManager con = CommonDALWrapper.GetConnectionManager(ConnectionConstants.SecMaster_Connection, true, IsolationLevel.Serializable);
            try
            {
                string query = string.Format(@"DECLARE @override_ids VARCHAR(MAX) = '{0}'
		                , @user_name VARCHAR(MAX) = '{1}'

                SELECT item AS override_id
				INTO #override_ids
				FROM IVPSecMaster.dbo.SECM_GetList2Table(@override_ids, ',')
				
				SELECT DISTINCT tmas.sectype_name, tdet.display_name
                INTO #temp_readonly
                FROM [IVPSecMaster].[dbo].[ivp_secm_rule_vendor_override] ovvr
				INNER JOIN #override_ids ids
				ON ovvr.override_id = ids.override_id
                INNER JOIN IVPSecMaster.dbo.ivp_secm_sectype_master tmas
                ON tmas.sectype_short_name = SUBSTRING(ovvr.secid, 1, 4) AND tmas.is_active = 1
                INNER JOIN IVPSecMaster.dbo.ivp_secm_template_details tdet
                ON tdet.attribute_id = ovvr.attrid AND tdet.template_id = IVPSecMaster.dbo.SECM_GetUserTemplate(tmas.sectype_id, @user_name) AND tdet.is_active = 1 AND tdet.is_read_only = 1

                IF EXISTS(SELECT 1 FROM #temp_readonly)
	                SELECT * FROM #temp_readonly
                ELSE
	                DELETE ovvr
					FROM [IVPSecMaster].[dbo].[ivp_secm_rule_vendor_override] ovvr
					INNER JOIN #override_ids ids
					ON ovvr.override_id = ids.override_id", string.Join(",", deleteInfo.Select(x => x.Key)), new RCommon().SessionInfo.LoginName);

                DataSet overridesDS = CommonDALWrapper.ExecuteSelectQuery(query, con);

                if (overridesDS != null && overridesDS.Tables.Count > 0 && overridesDS.Tables[0] != null && overridesDS.Tables[0].Rows.Count > 0)
                    throw new Exception("Overrides deletion failed as below " + (overridesDS.Tables[0].Rows.Count > 1 ? "attributes are" : "attribute is") + " read-only - " + string.Join(", ", overridesDS.Tables[0].AsEnumerable().Select(x => "Attribute : " + Convert.ToString(x["display_name"]) + " in Security type : " + Convert.ToString(x["sectype_name"]))));

                con.CommitTransaction();
                response = "Success";
            }
            catch (Exception Ex)
            {
                con.RollbackTransaction();
                mLogger.Error(Ex.ToString());
                response = Ex.Message;
            }
            finally
            {
                if (mHList != null)
                {
                    mHList.Clear();
                    mHList = null;
                }
                CommonDALWrapper.PutConnectionManager(con);
                mLogger.Debug("OverridesController : End -> SMDeleteOverride");
            }
            return response;
        }

        public static string SMSaveOverride(string username, string uniqueId, Dictionary<string, List<SMOverrideAttributesInfo>> attrInfo)
        {
            mLogger.Debug("OverridesController : Start -> SMSaveOverride");
            RHashlist mHList = new RHashlist();
            try
            {
                XElement SecTypeNodeXml = null;
                XElement NodeXml = null;
                XDocument SecTypedoc;
                String OverridenAttrXmlstring = string.Empty;

                NodeXml = new XElement("xml");
                IEnumerable<XElement> MemberList;

                foreach (var key in attrInfo.Keys)
                {
                    var DicKey = key.Split('|');
                    var sec_id = DicKey[0];
                    var sectype_id = DicKey[1];

                    MemberList = null;
                    foreach (var val in attrInfo.Values)
                    {
                        var AttrListXml = val.AsEnumerable().Select(
                                     i => new XElement("Attr",
                                                  new XAttribute("id", i.AttributeId), (string.IsNullOrEmpty(i.ExpiresOn) || i.ExpiresOn.ToLower().Equals("never")) ? null : new XAttribute("expirydt", i.ExpiresOn), new XAttribute("createdby", i.OverrideCreatedBy), new XAttribute("createdon", (String.IsNullOrEmpty(i.OverrideCreatedOn) ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : i.OverrideCreatedOn))
                                                  ));
                        if (MemberList != null)
                            MemberList.Last().Add(AttrListXml);
                        else
                            MemberList = AttrListXml;
                    }
                    SecTypeNodeXml = new XElement("SecId", new XAttribute("sectype_id", sectype_id), new XAttribute("sec_id", sec_id), MemberList);
                    NodeXml.Add(SecTypeNodeXml);
                }
                SecTypedoc = new XDocument(new XElement(NodeXml));
                OverridenAttrXmlstring = SecTypedoc.ToString(SaveOptions.DisableFormatting);

                DataSet dsOutput = CommonDALWrapper.ExecuteSelectQuery("EXEC SECM_SaveOverridenAttributes '" + OverridenAttrXmlstring.Trim() + "'," + true, ConnectionConstants.SecMaster_Connection);
                if (dsOutput.Tables.Count > 0 && Convert.ToInt32(dsOutput.Tables[0].Rows[0]["Status"]) == 1)
                {
                    return ("1|" + Convert.ToString(dsOutput.Tables[0].Rows[0]["Message"]));
                }
                else
                {
                    return ("0|" + Convert.ToString(dsOutput.Tables[0].Rows[0]["Message"]));
                }
            }
            catch (Exception Ex)
            {
                mLogger.Error(Ex.ToString());
                return ("0|" + Ex.ToString());
            }
            finally
            {
                if (mHList != null)
                {
                    mHList.Clear();
                    mHList = null;
                }
                mLogger.Debug("CommonServiceController : End -> SMSaveOverride");
            }
        }
    }

    [DataContract]
    public class SMOverrideAttributesInfo
    {
        [DataMember]
        public string AttributeId { get; set; }
        [DataMember]
        public string AttributeLabelId { get; set; }
        [DataMember]
        public string ExpiresOn { get; set; }
        [DataMember]
        public string OverrideCreatedBy { get; set; }
        [DataMember]
        public string OverrideCreatedOn { get; set; }
    }
}
