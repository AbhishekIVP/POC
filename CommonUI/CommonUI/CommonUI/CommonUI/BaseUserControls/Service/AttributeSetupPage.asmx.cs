using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace com.ivp.common.CommonUI.CommonUI.BaseUserControls.Service
{
    /// <summary>
    /// Summary description for AttributeSetupPage
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    //[System.Web.Script.Services.ScriptService]
    public class AttributeSetupPage : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string GetAttributeData(int moduleId, int EntityTypeId, int TemplateId, bool isLeg, bool isAdditionalLeg)
        {
            return JsonConvert.SerializeObject(new SRMModelerController.AttributeSetupController().GetAttributeData(moduleId, EntityTypeId, TemplateId, isLeg, isAdditionalLeg));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string GetLegDetails(int moduleId, int TypeId, int templateId)
        {
            //return null;
            return JsonConvert.SerializeObject(new SRMModelerController.AttributeLegController().GetLegDetails(moduleId, TypeId, templateId));
        }

    }
}
