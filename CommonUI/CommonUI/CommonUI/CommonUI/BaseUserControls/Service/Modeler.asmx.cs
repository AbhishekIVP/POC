using Newtonsoft.Json;
using SRMModelerController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace com.ivp.common.CommonUI.CommonUI.BaseUserControls.Service
{
    /// <summary>
    /// Summary description for Modeler
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Modeler : System.Web.Services.WebService
    {
       

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string GetAllTypes(int moduleId)
        {
            return JsonConvert.SerializeObject(new SRMModelerController.SRMModelerController().GetAllTypes(moduleId));
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string GetAllConfigInfo(int moduleId,int EntityTypeId)
        {
            return JsonConvert.SerializeObject(new SRMModelerController.SRMModelerController().GetAllConfigInfo(moduleId,EntityTypeId));
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string GetAllUsers()
        {
            return JsonConvert.SerializeObject(new SRMModelerController.SRMModelerController().GetAllUsers());
        }


        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string GetAllGroups()
        {
            return JsonConvert.SerializeObject(new SRMModelerController.SRMModelerController().GetAllGroups());
        }
    }
}
