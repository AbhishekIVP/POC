using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.ivp.commom.commondal;
using System.Data;
using com.ivp.rad.RUserManagement;

namespace SRMModelerController
{
    public class SRMTemplateMaster
    {
        public string templateName;
        public string userGroup;
        public int templateId;
    }


    public class SRMTemplateDetails
    {
        public string AttributeName;
        public string TabName;
        public string SubTabName;
        public string Mandatory;
        public string Hidden;
        public string ReadOnly;
    }


    public class SRMAccess
    {
        public string Rule;
        public List<string> Groups;
    }

    public class SRMModelerController
    {
        public List<SRMModelerTypeInfo> GetAllTypes(int moduleId)
        {
            List<SRMModelerTypeInfo> Typelist = new List<SRMModelerTypeInfo>() {
                new SRMModelerTypeInfo() {EntityTypeId=1,Type="hello",Tags= new string[] { "def","hij" },HasTags=true },
                new SRMModelerTypeInfo() {EntityTypeId=2,Type="1234",Tags=new string[] { "def","hij" },HasTags=true },
                new SRMModelerTypeInfo() {EntityTypeId=3,Type="Entity User",Tags=new string[] { "def","hij" },HasTags=true },
                new SRMModelerTypeInfo() {EntityTypeId=4,Type="Entity User 1",Tags=new string[] { "def","hij" },HasTags=true },
                new SRMModelerTypeInfo() {EntityTypeId=5,Type="abckjfle",Tags=new string[] { "def","hij" },HasTags=true },
                new SRMModelerTypeInfo() {EntityTypeId=6,Type="ghtihd",Tags=new string[] { "def","hij" },HasTags=true }
            };

            if (moduleId == 3)
            {
                return Typelist;
            }

            else if (moduleId == 6)
            {
                return Typelist;
            }
            else if (moduleId == 9)
            {
                return Typelist;
            }

            return null;
        }

        public SRMModelerConfigInfo GetAllConfigInfo(int moduleId, int EntityTypeId)
        {
            List<SRMModelerConfigInfo> ConfigAttrList = new List<SRMModelerConfigInfo>() {
                new SRMModelerConfigInfo() {SecTypeName="hello",IsVanilla=false,IsExotic=true,IsBasket=true,SecTypeDesc="this security type is hello.",IsCreateScratch=false,IsCreateExisting=false,
                                                DefaultDate="2016-11-05",Tags = new string[] { "def","hij" },Users=new string[] {"abc@xyz.com","pqr@123.com"},Groups=new string[] {"dev","prod","test"} },
                new SRMModelerConfigInfo() {SecTypeName="1234",IsVanilla=true,IsExotic=false,IsBasket=true,SecTypeDesc="this security type is hello.",IsCreateScratch=false,IsCreateExisting=false,
                                                DefaultDate="2016-11-05",Tags = new string[] { "def","hij" },Users=new string[] {"c@xyz.com","abc@xyz.com"},Groups=new string[] {"dev","prod"}},
                new SRMModelerConfigInfo() {SecTypeName="Entity User",IsVanilla=false,IsExotic=true,IsBasket=true,SecTypeDesc="this security type is hello.",IsCreateScratch=false,IsCreateExisting=false,
                                                DefaultDate="2016-11-05",Tags = new string[] { "def","hij" },Users=new string[] { "abc@xyz.com", "c@xyz.com"},Groups=new string[] {"dev","test"}},
                new SRMModelerConfigInfo() {SecTypeName="Entity User 1",IsVanilla=false,IsExotic=true,IsBasket=true,SecTypeDesc="this security type is hello.",IsCreateScratch=false,IsCreateExisting=false,
                                                DefaultDate="2016-11-05",Tags = new string[] { "def","hij" },Users=new string[] {"abc@123.com","b@xyz.com"},Groups=new string[] {"test"}},
                new SRMModelerConfigInfo() {SecTypeName="abckjfle",IsVanilla=false,IsExotic=true,IsBasket=true,SecTypeDesc="this security type is hello.",IsCreateScratch=false,IsCreateExisting=false,
                                                DefaultDate="2016-11-05",Tags = new string[] { "def","hij" },Users=new string[] {"c@xyz.com","b@xyz.com"},Groups=new string[] {"prod","test"}},
                new SRMModelerConfigInfo() {SecTypeName="ghtihd",IsVanilla=false,IsExotic=true,IsBasket=true,SecTypeDesc="this security type is hello.",IsCreateScratch=false,IsCreateExisting=false,
                                                DefaultDate="2016-11-05",Tags = new string[] { "def","hij" },Users=new string[] { "pqr@123.com", "b@xyz.com"},Groups=new string[] {"dev","prod","test"}}
            };




            if (moduleId == 3)
            {
                SRMModelerConfigInfo returnelement = ConfigAttrList[EntityTypeId - 1];
                return returnelement;

            }
            if (moduleId == 6)
            {

                SRMModelerConfigInfo returnelement = ConfigAttrList[EntityTypeId - 1];
                return returnelement;

            }
            if (moduleId == 9)
            {

                SRMModelerConfigInfo returnelement = ConfigAttrList[EntityTypeId - 1];
                return returnelement;

            }
            return null;
        }
        public List<SRMModelerUserInfo> GetAllUsers()
        {
            List<SRMModelerUserInfo> UserAttrList = new List<SRMModelerUserInfo>() {
                new SRMModelerUserInfo() {FirstName="Anshika",LastName="Dabas",username = "adabas",email = "abc@xyz.com"},
                new SRMModelerUserInfo() { FirstName= "Mukul",LastName="Dabas",username = "adabas",email = "pqr@123.com"},
                new SRMModelerUserInfo() { FirstName= "Anil",LastName= "Dabas",username = "adabas",email = "c@xyz.com"},
                new SRMModelerUserInfo() { FirstName= "Jyoti",LastName= "Dabas",username = "adabas",email = "b@xyz.com"},
                new SRMModelerUserInfo() { FirstName= "Akshita",LastName="Dabas",username = "adabas",email = "abc@123.com"},
                new SRMModelerUserInfo() { FirstName= "Varun",LastName= "Dabas",username = "adabas",email = "c@xyz.com"}
            };

            return UserAttrList;

        }
        public List<SRMModelerGroupInfo> GetAllGroups()
        {
            List<SRMModelerGroupInfo> GroupAttrList = new List<SRMModelerGroupInfo>()
            {
                new SRMModelerGroupInfo() {groupname="dev"},
                new SRMModelerGroupInfo() {groupname="prod"},
                new SRMModelerGroupInfo() {groupname="impl"},
                new SRMModelerGroupInfo() {groupname="test"},
                new SRMModelerGroupInfo() {groupname="hell"},
                new SRMModelerGroupInfo() {groupname="heaven"}


            };

            return GroupAttrList;
        }

        public DataTable Derivatives(int secTypeId)
        {
            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(@"SELECT distinct sectype_name FROM IVPSecMaster.dbo.ivp_secm_sectype_table st
inner join ivpsecmaster.dbo.ivp_secm_sectype_master sm
on st.sectype_id = sm.sectype_id
 WHERE underlyer_exists = 1 AND (underlyer_default_sectype_id = " + secTypeId + @" OR underlyer_default_sectype_id = 0) AND priority = 1 ", ConnectionConstants.SecMaster_Connection);
            DataTable dt = ds.Tables[0];


            return dt;
        }
        public DataTable DerivativesLeft(int secTypeId)
        {
            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(@"SELECT distinct sectype_name FROM IVPSecMaster.dbo.ivp_secm_sectype_table st
inner join ivpsecmaster.dbo.ivp_secm_sectype_master sm
on st.sectype_id = sm.sectype_id
WHERE underlyer_exists = 1 AND (underlyer_default_sectype_id =" + secTypeId + @" OR underlyer_default_sectype_id = 0) AND (priority < 0 or priority >1)", ConnectionConstants.SecMaster_Connection);
            DataTable dt = ds.Tables[0];

            return dt;
        }


        public DataTable ConstituentRight(int secTypeId)
        {

            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(@"DECLARE @underlyer_sectype_id INT;
                  SELECT @underlyer_sectype_id=underlyer_default_sectype_id FROM IVPSecMaster.dbo.ivp_secm_sectype_table WHERE sectype_id = " + secTypeId + @" AND underlyer_exists = 1 and priority = 1;
                        

                  if @underlyer_sectype_id=0
                  select distinct sectype_name from ivpsecmaster.dbo.ivp_secm_sectype_master
                      else
                  select distinct sectype_name from ivpsecmaster.dbo.ivp_secm_sectype_master where sectype_id=@underlyer_sectype_id", ConnectionConstants.SecMaster_Connection);
            DataTable dt = ds.Tables[0];

            return dt;
        }


        public DataTable ConstituentRightMost(int secTypeId)
        {

            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(@"DECLARE @UnderlyerSectypeIdTable TABLE
                                                           (
                                                             underlyer_sectype_id int 
                                                       );

                       insert into @UnderlyerSectypeIdTable (underlyer_sectype_id)(SELECT underlyer_default_sectype_id FROM IVPSecMaster.dbo.ivp_secm_sectype_table st
                          inner join ivpsecmaster.dbo.ivp_secm_sectype_master sm
                          on st.sectype_id = sm.sectype_id WHERE st.sectype_id = " + secTypeId + @" AND (priority < 0 or priority >1) AND underlyer_exists = 1);



                          DECLARE @isZero INT;

                         select @isZero=underlyer_sectype_id from @UnderlyerSectypeIdTable where underlyer_sectype_id=0
                         if @isZero=0
                         select distinct sectype_name from ivpsecmaster.dbo.ivp_secm_sectype_master
                          else
                         select  distinct sectype_name from ivpsecmaster.dbo.ivp_secm_sectype_master where sectype_id IN(select underlyer_sectype_id from  @UnderlyerSectypeIdTable);", ConnectionConstants.SecMaster_Connection);
            DataTable dt = ds.Tables[0];

            return dt;
        }


        public List<string> GetAllowedUsers(int typeId, int moduleId)
        {
            RUserManagementService objUserController = new RUserManagementService();
            List<RUserInfo> radUsers = objUserController.GetAllUsersGDPR();

            DataTable dt = new DataTable();

            switch(moduleId)
            {
                case 3: dt = CommonDALWrapper.ExecuteSelectQuery(@"select allowed_users from ivpsecmaster.dbo.ivp_secm_sectype_master where sectype_id = " + typeId, ConnectionConstants.SecMaster_Connection).Tables[0];
                    break;
                case 6:
                case 18:
                case 20:
                    dt = CommonDALWrapper.ExecuteSelectQuery(@"select allowed_users from ivprefmaster.dbo.ivp_refm_entity_type where entity_type_id = " + typeId, ConnectionConstants.SecMaster_Connection).Tables[0];
                    break;

            }

            string selectedUsers = Convert.ToString(dt.Rows[0]["allowed_users"]);
            if (string.IsNullOrEmpty(selectedUsers))
                return radUsers.Where(x => x.UserName != "admin").OrderBy(x => x.FullName).Select(x => x.FullName).ToList();
            else if (selectedUsers.Length > 0)
            {
                HashSet<string> hshSelectedUsers = new HashSet<string>(selectedUsers.Split(','));
                Dictionary<string, string> allUsers = radUsers.Where(x => x.UserName != "admin").ToDictionary(y => y.UserLoginName, x => x.FullName);

                return allUsers.Where(x => hshSelectedUsers.Contains(x.Key)).Select(x => x.Value).OrderBy(x => x).ToList();
            }

            return new List<string>();
        }

        public List<string> GetAllowedGroups(int typeId, int moduleId)
        {
            DataTable dt = new DataTable();

            switch (moduleId)
            {
                case 3:
                    dt = CommonDALWrapper.ExecuteSelectQuery(@"select allowed_groups from ivpsecmaster.dbo.ivp_secm_sectype_master where sectype_id = " + typeId, ConnectionConstants.SecMaster_Connection).Tables[0];
                    break;
                case 6:
                case 18:
                case 20:
                    dt = CommonDALWrapper.ExecuteSelectQuery(@"select allowed_groups from ivprefmaster.dbo.ivp_refm_entity_type where entity_type_id = " + typeId, ConnectionConstants.SecMaster_Connection).Tables[0];
                    break;

            }

            string selectedGroups = Convert.ToString(dt.Rows[0]["allowed_groups"]);
            if (string.IsNullOrEmpty(selectedGroups))
                return new RUserManagementService().GetAllGroupsGDPR().Select(x => x.GroupName).OrderBy(x => x).ToList();
            else if (selectedGroups.Length > 0)
                return selectedGroups.Split(',').ToList();

            return new List<string>();
        }

        public List<SRMTemplateMaster> GetTemplateInfo(int typeId, int moduleId)
        {
            List<SRMTemplateMaster> temDetails = new List<SRMTemplateMaster>();
            RUserManagementService objUserController = new RUserManagementService();
            List<RUserInfo> radUsers = objUserController.GetAllUsersGDPR();

            Dictionary<string, string> dictLoginNamevsFullName = radUsers.ToDictionary(x => x.UserLoginName, y => y.FullName);

            DataTable dt = new DataTable();

            switch (moduleId)
            {
                case 3:
                    dt = CommonDALWrapper.ExecuteSelectQuery(@"select template_name, dependent_type_name, dependent_id, template_id from ivpsecmaster.dbo.ivp_secm_template_master tm 
inner join ivpsecmaster.dbo.ivp_secm_template_dependent_type dt
on tm.dependent_type_id = dt.dependent_type_id and sectype_id = " + typeId, ConnectionConstants.SecMaster_Connection).Tables[0];
                    break;
                case 6:
                case 18:
                case 20:
                    dt = CommonDALWrapper.ExecuteSelectQuery(@"SELECT template_id,template_name,template_type_id AS 'dependent_id',dependent_id AS 'dependent_type_name' FROM IVPRefMaster.dbo.ivp_refm_entity_template_master
WHERE entity_type_id = " + typeId + "  AND is_active = 1 ", ConnectionConstants.SecMaster_Connection).Tables[0];
                    break;

            }

            foreach (DataRow row in dt.Rows)
            {
                if(Convert.ToString(row["dependent_type_name"]).Equals("USER"))
                {
                    string uName = Convert.ToString(row["dependent_id"]);
                    if(dictLoginNamevsFullName.ContainsKey(uName))
                    {
                        temDetails.Add(new SRMTemplateMaster() { templateId = row.Field<int>("template_id"), templateName = row.Field<string>("template_name"), userGroup = dictLoginNamevsFullName[uName] });
                    }
                }
                else
                    temDetails.Add(new SRMTemplateMaster() { templateId = row.Field<int>("template_id"), templateName = row.Field<string>("template_name"), userGroup = row.Field<string>("dependent_type_name") });

            }

            return temDetails;
        }

        public List<SRMAccess> GetRuleAccessDetails(int typeId, int moduleId)
        {
            List<SRMAccess> accessList = new List<SRMAccess>();
            switch(moduleId)
            {
                case 3:
                    accessList.Add(new SRMAccess() { Rule = "Has Position Equals True", Groups = new List<string>(new string[] { "Ops Manager", "Ops User" }) });
                    accessList.Add(new SRMAccess() { Rule = "Issue Country Equals USA", Groups = new List<string>(new string[] { "NAMR Research" }) });
                    break;
                case 6:
                case 18:
                case 20:
                    accessList.Add(new SRMAccess() { Rule = "Country equals US", Groups = new List<string>(new string[] { "Data Entry - US", "Fund Manager", "US Ops", "Operations Manager" }) });
                    accessList.Add(new SRMAccess() { Rule = "Country not equals US", Groups = new List<string>(new string[] { "Data Entry - Global", "Fund Manager", "Global Ops", "Operations Manager" }) });
                    break;
            }
            return accessList;
        }

        public List<SRMTemplateDetails> GetTemplateDetails(int templateId, int moduleId)
        {
            List<SRMTemplateDetails> temDetails = new List<SRMTemplateDetails>();

            DataTable dt = new DataTable();

            switch (moduleId)
            {
                case 3:
                    dt = CommonDALWrapper.ExecuteSelectQuery(@"select display_name, tab_name, sub_tab_name, is_mandatory, to_show, is_read_only from ivpsecmaster.dbo.ivp_secm_template_details td
inner join ivpsecmaster.dbo.ivp_secm_sub_tabs st on st.sub_tab_id = td.sub_tab_id and template_id = " + templateId + " inner join ivpsecmaster.dbo.ivp_secm_tabs tab on st.tab_id = tab.tab_id order by display_name", ConnectionConstants.SecMaster_Connection).Tables[0];
                    break;
                case 6:
                case 18:
                case 20:
                    dt = CommonDALWrapper.ExecuteSelectQuery(@"SELECT eat.display_name,tnam.entity_tab_name AS 'tab_name','' AS 'sub_tab_name',eatt.is_mandatory,eatt.is_visible AS 'to_show',eatt.is_read_only
FROM IVPRefMaster.dbo.ivp_refm_entity_template_master tmp
INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_tab tab
ON (tab.template_id = tmp.template_id AND tab.is_active = 1 AND tmp.is_active = 1)
INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_tab_name tnam
ON (tnam.entity_tab_id = tab.entity_tab_id AND tnam.is_active = 1)
INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute_tab eatt
ON (eatt.entity_tab_name_id = tnam.entity_tab_name_id AND eatt.is_active = 1)
INNER JOIN IVPRefMaster.dbo.ivp_refm_entity_attribute eat
ON (eat.entity_attribute_id = eatt.entity_attribute_id AND eat.is_active = 1 AND eat.entity_type_id = tmp.entity_type_id)
WHERE  tmp.template_id = " + templateId + " ORDER BY tab_name,eat.display_name", ConnectionConstants.SecMaster_Connection).Tables[0];
                    break;

            }


            foreach (DataRow row in dt.Rows)
            {
                SRMTemplateDetails info = new SRMTemplateDetails();
                info.AttributeName = row.Field<string>("display_name");
                info.Hidden = row.Field<bool>("to_show") ? "No" : "Yes";
                info.Mandatory = row.Field<bool>("is_mandatory") ? "Yes" : "No";
                info.ReadOnly = row.Field<bool>("is_read_only") ? "Yes" : "No";
                info.SubTabName = row.Field<string>("sub_tab_name");
                info.TabName = row.Field<string>("tab_name");

                temDetails.Add(info);

            }

            return temDetails;
        }

        public DataTable SecTypeNameText(int secTypeId)
        {

            DataSet ds = CommonDALWrapper.ExecuteSelectQuery(@"select sectype_name from  ivpsecmaster.dbo.ivp_secm_sectype_master where sectype_id=" + secTypeId, ConnectionConstants.SecMaster_Connection);
            DataTable dt = ds.Tables[0];

            return dt;
        }


    }
}
