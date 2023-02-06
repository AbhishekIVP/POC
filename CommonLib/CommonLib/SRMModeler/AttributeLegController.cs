using com.ivp.commom.commondal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRMModelerController
{
    public class AttributeLegController
    {
        public List<AttributeLegControllerInfo> GetLegDetails(int moduleId, int typeId, int templateId)
        {
            List<AttributeLegControllerInfo> LegList = new List<AttributeLegControllerInfo>();
            DataSet dsLeg = null;
            if (moduleId == 3)
            {
                dsLeg = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"select st.table_display_name,st.underlyer_exists,st.priority,sm.sectype_name,td.display_name, st.sectype_table_id as 'table_id', 0 as [is_additional_leg]
                                                                            from IVPSecMaster.dbo.ivp_secm_sectype_table st 
                                                                            left join (select * from IVPSecMaster.dbo.ivp_secm_attribute_details where is_active = 1) ad on st.primary_attribute_id=ad.attribute_id
                                                                            left join (select distinct attribute_id, display_name from IVPSecMaster.dbo.ivp_secm_template_details where is_active = 1 and template_id={1}) td on ad.attribute_id=td.attribute_id
                                                                            left join (select * from IVPSecMaster.dbo.ivp_secm_sectype_master where is_active = 1) sm on st.underlyer_default_sectype_id=sm.sectype_id
                                                                            where priority<>1 and st.sectype_id= {0}
                                                                            UNION ALL
                                                                            select al.additional_legs_name as 'table_display_name', NULL as 'underlyer_exists',-1 as 'priority', NULL as 'sectype_name',
                                                                            STUFF((SELECT ',' + display_name
			                                                                FROM [IVPSecMaster].[dbo].[ivp_secm_additional_legs_attribute_details]
			                                                                WHERE additional_legs_id = al.additional_legs_id and is_primary = 1
			                                                                FOR XML PATH(''), TYPE)
			                                                                .value('.', 'NVARCHAR(MAX)'),1,1,'') [display_name], al.additional_legs_id as 'table_id', 1 as [is_additional_leg] 
                                                                            from IVPSecMaster.dbo.ivp_secm_additional_legs al 
                                                                            inner join ivp_secm_additional_legs_sectype_mapping sm on al.additional_legs_id = sm.additional_legs_id
                                                                            where sectype_id = {0}", typeId, templateId), ConnectionConstants.SecMaster_Connection);

                //dsLeg = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"select al.additional_legs_name as 'table_display_name', NULL as 'underlyer_exists', NULL as 'sectype_name', -1 as 'priority',al.additional_legs_id, 1 as [is_additional_leg],
                //                                                STUFF((SELECT ',' + display_name
			             //                                                   FROM [IVPSecMaster].[dbo].[ivp_secm_additional_legs_attribute_details]
			             //                                                   WHERE additional_legs_id = al.additional_legs_id and is_primary = 1
			             //                                                   FOR XML PATH(''), TYPE)
			             //                                                   .value('.', 'NVARCHAR(MAX)'),1,1,'') [primary] 
                //                                                from IVPSecMaster.dbo.ivp_secm_additional_legs al 
                //                                                inner join ivp_secm_additional_legs_sectype_mapping sm on al.additional_legs_id = sm.additional_legs_id
                //                                                            where sectype_id = {0} ", typeId), ConnectionConstants.SecMaster_Connection);


                LegList = dsLeg.Tables[0].AsEnumerable().Select(dataRow =>
                {
                    var underlyer = Convert.ToString(dataRow["underlyer_exists"]);
                    return new AttributeLegControllerInfo
                    {
                        LegName = Convert.ToString(dataRow["table_display_name"]),
                        HasUnderlier = (!string.IsNullOrEmpty(underlyer))?Convert.ToBoolean(underlyer):false,
                        UnderlierSecTypeName = Convert.ToString(dataRow["sectype_name"]),
                        MultipleInfo = (dataRow.Field<int>("priority") < 0) ? true : false,
                        PrimaryKey = Convert.ToString(dataRow["display_name"]),
                        LegId=dataRow.Field<int>("table_id"),
                        IsAdditionalLeg=(dataRow.Field<int>("is_additional_leg")==1)?true:false
                    };
                }).ToList();

                return LegList;
            }
            else if (moduleId == 9) { }
            else
            {
                dsLeg = CommonDALWrapper.ExecuteSelectQuery(string.Format(@"select entity_display_name,is_one_to_one,entity_type_id,
                                                                        STUFF((SELECT ',' + display_name
			                                                                        FROM [IVPRefMaster].[dbo].[ivp_refm_entity_attribute]
			                                                                        WHERE entity_type_id = et.entity_type_id and primary_attribute = 1 and is_active=1
			                                                                        FOR XML PATH(''), TYPE)
			                                                                        .value('.', 'NVARCHAR(MAX)'),1,1,'') [primary]
                                                                        from IVPRefMaster.dbo.ivp_refm_entity_type et
                                                                        where structure_type_id=3 and is_active=1 and derived_from_entity_type_id={0}", typeId), ConnectionConstants.RefMaster_Connection);
                LegList = dsLeg.Tables[0].AsEnumerable().Select(dataRow => new AttributeLegControllerInfo
                {
                    LegName = dataRow.Field<string>("entity_display_name"),
                    MultipleInfo = (dataRow.Field<bool>("is_one_to_one") == false) ? true : false,
                    PrimaryKey = Convert.ToString(dataRow["primary"]),
                    LegId = string.IsNullOrEmpty(Convert.ToString(dataRow.Field<int>("entity_type_id"))) ? -1 : dataRow.Field<int>("entity_type_id")
                }).ToList();
                return LegList;
            }
            return null;
        }
    }
}
