using com.ivp.rad.viewmanagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace com.ivp.common.CommonUI.CommonUI.BaseUserControls.Modeler
{
    public partial class AttributeLegSetupPage : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            int moduleId;
            int typeId;
            int templateId;
            
            if (Request.QueryString["module"] != null && Request.QueryString["TypeId"] != null)
            {
                moduleId = Convert.ToInt32(Request.QueryString["module"]);
                hdnmoduleId.Value = moduleId.ToString();
                typeId = Convert.ToInt32(Request.QueryString["TypeId"]);
                hdnTypeId.Value = typeId.ToString();
                templateId = Convert.ToInt32(Request.QueryString["templateId"]);
                hdnTemplateId.Value = templateId.ToString();
            }
        }
    }
}