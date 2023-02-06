using com.ivp.rad.viewmanagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace com.ivp.common.CommonUI.CommonUI.BaseUserControls.Modeler
{
    public partial class ConfigurePage : BasePage
    {
        int moduleId;
        int EntityTypeId;

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            this.Theme = "Aqua";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            if (Request.QueryString["module"] != null && Request.QueryString["EntityTypeId"] != null)
            {
                moduleId = Convert.ToInt32(Request.QueryString["module"]);
                hdnModuleId.Value = moduleId.ToString();
                EntityTypeId = Convert.ToInt32(Request.QueryString["EntityTypeId"]);
                hdnEntityTypeId.Value = EntityTypeId.ToString();
            }
        }
    }
}