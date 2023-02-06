using com.ivp.commom.commondal;
using com.ivp.refmaster.refmasterwebservices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SRMModelerController
{
    public class AttributeSetupController
    {
        public List<AttributeSetupInfo> GetAttributeData(int moduleId, int EntityTypeID, int templateId, bool isLeg, bool isAdditionalLeg)
        {
            if (moduleId == 3)
            {
                List<AttributeSetupInfo> SecAttrList = new List<AttributeSetupInfo>();
                DataSet SecAttr = null;

                if (!isLeg)
                {
                    SecAttr = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"select td.display_name,asub.data_type_name,ad.is_enforced_mandatory, refm.reference_type_id, refm.reference_attribute_name, refm.reference_attribute_id, refm.reference_display_attributes,ad.data_type_length, ad.is_cloneable, ad.attribute_description,
                                                                                STUFF((SELECT ',' + tag_name
			                                                                                FROM [IVPSecMaster].[dbo].[ivp_secm_tags]
			                                                                                WHERE tag_type_id = 1 AND id = ad.attribute_id
			                                                                                FOR XML PATH(''), TYPE)
			                                                                                .value('.', 'NVARCHAR(MAX)'),1,1,'') as tag_name
                                                                                from IVPSecMaster.dbo.ivp_secm_sectype_table st
                                                                                join IVPSecMaster.dbo.ivp_secm_attribute_details ad on ad.sectype_table_id=st.sectype_table_id
                                                                                JOIN IVPSecMaster.dbo.ivp_secm_template_details td on ad.attribute_id=td.attribute_id
                                                                                join IVPSecMaster.dbo.ivp_secm_attribute_subtype asub on asub.attribute_subtype_id=ad.attribute_subtype_id
                                                                                left join IVPSecMaster.dbo.ivp_secm_reference_attribute_mapping refm on refm.attribute_id=ad.attribute_id
                                                                                where ((st.sectype_id = 0)or (st.sectype_id={0} and priority = 1)) and ad.is_active=1 and td.template_id={1} and td.is_active=1
                                                                                order by td.display_name", EntityTypeID, templateId), ConnectionConstants.SecMaster_Connection);

                }
                else if (isLeg && !isAdditionalLeg)
                {
                    SecAttr = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"select td.display_name,asub.data_type_name,ad.is_enforced_mandatory, refm.reference_type_id, refm.reference_attribute_name, refm.reference_attribute_id, refm.reference_display_attributes,ad.data_type_length, ad.is_cloneable, ad.attribute_description,
                                                                                    STUFF((SELECT ',' + tag_name
			                                                                                    FROM [IVPSecMaster].[dbo].[ivp_secm_tags]
			                                                                                    WHERE tag_type_id = 1 AND id = ad.attribute_id
			                                                                                    FOR XML PATH(''), TYPE)
			                                                                                    .value('.', 'NVARCHAR(MAX)'),1,1,'') as tag_name
                                                                                    from IVPSecMaster.dbo.ivp_secm_sectype_table st
                                                                                    join IVPSecMaster.dbo.ivp_secm_attribute_details ad on ad.sectype_table_id=st.sectype_table_id
                                                                                    JOIN IVPSecMaster.dbo.ivp_secm_template_details td on ad.attribute_id=td.attribute_id
                                                                                    join IVPSecMaster.dbo.ivp_secm_attribute_subtype asub on asub.attribute_subtype_id=ad.attribute_subtype_id
                                                                                    left join IVPSecMaster.dbo.ivp_secm_reference_attribute_mapping refm on refm.attribute_id=ad.attribute_id
                                                                                    where st.sectype_table_id = {0} and ad.is_active=1 and td.template_id={1} and td.is_active=1 order by td.display_name", EntityTypeID, templateId), ConnectionConstants.SecMaster_Connection);
                }
                else
                {
                    SecAttr = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"select ad.display_name, at.display_data_type as 'data_type_name', ad.is_enforced_mandatory, refm.reference_type_id, refm.reference_attribute_name, refm.reference_attribute_id, refm.reference_display_attributes, ad.data_type_length, ad.is_cloneable, ad.attribute_description,
                                                                                    NULL as tag_name
                                                                                    from IVPSecMaster.dbo.ivp_secm_additional_legs_attribute_details ad
                                                                                    inner join IVPSecMaster.dbo.ivp_secm_additional_legs_attribute_type at ON ad.attribute_type_id = at.attribute_type_id
                                                                                    left join IVPSecMaster.dbo.ivp_secm_additional_legs_reference_attribute_mapping refm on refm.attribute_id=ad.attribute_id
                                                                                    where ad.additional_legs_id = {0} and ad.is_active=1 order by ad.display_name", EntityTypeID, templateId), ConnectionConstants.SecMaster_Connection);
                }

                Dictionary<int, Dictionary<string, string>> entityTypeVsEntityAttrRealNameVsDisplayName = new Dictionary<int, Dictionary<string, string>>();
                Dictionary<int, string> entityAttributeIdVsName = new Dictionary<int, string>();
                var entityTypeIdVsName = new RMRefMasterAPI().GetAllMainEntityType().Tables[0].AsEnumerable().ToDictionary(x => Convert.ToInt32(x["entity_type_id"]), y => Convert.ToString(y["entity_display_name"]));
                foreach (var row in new RMRefMasterAPI().GetAllEntityAttributes_Curve().Tables[0].AsEnumerable())
                {
                    var typeId = Convert.ToInt32(row["entity_type_id"]);
                    entityAttributeIdVsName[Convert.ToInt32(row["entity_attribute_id"])] = Convert.ToString(row["display_name"]);
                    if (!entityTypeVsEntityAttrRealNameVsDisplayName.ContainsKey(typeId))
                        entityTypeVsEntityAttrRealNameVsDisplayName[typeId] = new Dictionary<string, string>();
                    entityTypeVsEntityAttrRealNameVsDisplayName[typeId][Convert.ToString(row["attribute_name"])] = Convert.ToString(row["display_name"]);
                }

                SecAttrList = SecAttr.Tables[0].AsEnumerable().Select(dataRow =>
                {
                    var man = Convert.ToString(dataRow["is_enforced_mandatory"]);
                    var refTypeId = Convert.ToString(dataRow["reference_type_id"]);
                    var refTypeId2 = 0;
                    var refType = string.Empty;

                    if (!string.IsNullOrEmpty(refTypeId))
                    {
                        refTypeId2 = Convert.ToInt32(refTypeId);
                        refType = entityTypeIdVsName[refTypeId2];
                    }

                    var refAttrId = Convert.ToString(dataRow["reference_attribute_id"]);
                    var refDisAttrs = Convert.ToString(dataRow["reference_display_attributes"]);
                    if (!string.IsNullOrEmpty(refDisAttrs))
                    {
                        refDisAttrs = string.Join(",", refDisAttrs.Split(',').Select(realName => entityTypeVsEntityAttrRealNameVsDisplayName[refTypeId2][realName]));
                    }

                    return new AttributeSetupInfo
                    {
                        AttrName = Convert.ToString(dataRow["display_name"]),
                        DataType = (!string.IsNullOrEmpty(refTypeId)) ? "REFERENCE" : Convert.ToString(dataRow["data_type_name"]),
                        ReferenceType = refType,
                        ReferenceAttribute = (!string.IsNullOrEmpty(refAttrId)) ? entityAttributeIdVsName[Convert.ToInt32(dataRow["reference_attribute_id"])] : string.Empty,
                        ReferenceLegAttribute = string.Empty,
                        ReferenceDisplayAttribute = refDisAttrs,
                        Length = (!string.IsNullOrEmpty(refTypeId) || (Convert.ToString(dataRow["data_type_length"])=="0")) ? string.Empty: Convert.ToString(dataRow["data_type_length"]),
                        Mandatory = (!string.IsNullOrEmpty(man)) ? Convert.ToBoolean(man) : false,
                        IsCloneable = dataRow.Field<bool>("is_cloneable"),
                        Tags = new string[] { dataRow.Field<string>("tag_name") },
                        AttrDescription = Convert.ToString(dataRow["attribute_description"])
                    };
                }).ToList();

                return SecAttrList;
            }
            else if (moduleId == 9) { }
            else //(moduleId == 6 || )
            {
                List<AttributeSetupInfo> RefAttrList = new List<AttributeSetupInfo>();
                //RMAttributeSetupController attributeSetupController = new RMAttributeSetupController();
                //DataSet dsAttr = attributeSetupController.GetAllAttributeByEntityTypeId(EntityTypeID);                
                //Assembly RefMController = Assembly.Load("RefMController");
                //Type attributeSetupController = null;
                //attributeSetupController = RefMController.GetType("com.ivp.refmaster.controller.RMAttributeSetupController");

                //MethodInfo getAttributes = attributeSetupController.GetMethod("GetAllAttributeByEntityTypeId");
                //var attributeObj = Activator.CreateInstance(attributeSetupController);
                //DataSet dsAttr = (DataSet)getAttributes.Invoke(attributeObj, new object[] { EntityTypeID});
                DataSet dsAttr = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"select ea.*,et.entity_display_name,ceat.display_name,rt.tag_name from                                             IVPRefMaster.dbo.ivp_refm_entity_attribute ea 
                                            left join IVPRefMaster.dbo.ivp_refm_entity_attribute_lookup al
                                            on(al.child_entity_attribute_id = ea.entity_attribute_id)
                                            left join IVPRefMaster.dbo.ivp_refm_entity_type et
                                            on(et.entity_type_id = al.parent_entity_type_id)
                                            LEFT JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute ceat
                                            ON(ceat.entity_attribute_id = al.lookup_attribute_id)
                                            left join IVPRefMaster.dbo.ivp_refm_tags_mapping tm
                                            on(tm.attribute_id = ea.entity_attribute_id)
                                            left join IVPRefMaster.dbo.ivp_refm_tags rt
                                            on(rt.tag_id = tm.tag_id)
                                            where ea.entity_type_id = {0} 
                                            and (et.is_active IS NULL OR et.is_active = 1)
                                            and (ea.is_active=1 )
                                            and (al.is_active IS NULL OR al.is_active = 1) order by ceat.display_name", EntityTypeID), ConnectionConstants.RefMaster_Connection);


                RefAttrList = dsAttr.Tables[0].AsEnumerable().Select(dataRow => new AttributeSetupInfo
                {                
                    AttrName = dataRow.Field<string>("display_name"),
                    DataType = dataRow.Field<string>("data_type"),
                    LookupType = dataRow.Field<string>("entity_display_name"),
                    LookupAttribute = dataRow.Field<string>("display_name"),
                    DefaultValue = dataRow.Field<string>("default_value"),
                    SearchViewPosition = Convert.ToString(dataRow["search_view_position"]),
                    Length = dataRow.Field<string>("data_type").ToUpper() == "DECIMAL" ? (Convert.ToInt32(Convert.ToString(dataRow["data_type_length"]).Split(',')[0]) - Convert.ToInt32(Convert.ToString(dataRow["data_type_length"]).Split(',')[1]) + "." + Convert.ToString(dataRow["data_type_length"]).Split(',')[1]) : Convert.ToString(dataRow["data_type_length"]),
                    Mandatory = !dataRow.Field<bool>("is_nullable"),
                    IsCloneable = dataRow.Field<bool>("is_clonable"),
                    Encrypted = dataRow.Field<bool>("is_encrypted"),
                    VisibleInSearch = dataRow.Field<bool>("is_search_view"),
                    Tags = new string[] { dataRow.Field<string>("tag_name") },
                    RestrictedChar = dataRow.Field<string>("restricted_chars")                        
            }).ToList();

            return RefAttrList;
        }

            return null;
        }
}
}
