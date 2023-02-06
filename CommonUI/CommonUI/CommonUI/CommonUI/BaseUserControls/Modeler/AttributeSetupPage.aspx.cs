using com.ivp.rad.viewmanagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace com.ivp.common.CommonUI.CommonUI.BaseUserControls.Modeler
{
    public partial class AttributeSetupPage : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            int moduleId;
            int EntityTypeId;
            int TemplateId;
            bool isLeg;
            bool isAdditionalLeg;
            if (Request.QueryString["module"] != null && Request.QueryString["TypeId"] != null && Request.QueryString["templateId"] != null)
            {
                moduleId = Convert.ToInt32(Request.QueryString["module"]);
                hdnmoduleId.Value = moduleId.ToString();
                EntityTypeId = Convert.ToInt32(Request.QueryString["TypeId"]);
                hdnEntityTypeId.Value = EntityTypeId.ToString();
                TemplateId= Convert.ToInt32(Request.QueryString["templateId"]);
                hdnTemplateId.Value = TemplateId.ToString();
                isLeg = Convert.ToBoolean(Request.QueryString["isLeg"]);
                hdnIsLeg.Value = isLeg.ToString();
                isAdditionalLeg = Convert.ToBoolean(Request.QueryString["isAdditionalLeg"]);
                hdnAdditionalLeg.Value = isAdditionalLeg.ToString();

            }
        }
    }
}

