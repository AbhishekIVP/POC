using com.ivp.rad.vendorapi.reuters;
using com.ivp.rad.viewmanagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class SRMRuleCatalog : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            var moduleName = this.Request.QueryString["Module"];
            var subModuleName = this.Request.QueryString["SubModule"];
            
            if (this.Request.QueryString["id"] != null && !string.IsNullOrEmpty(this.Request.QueryString["id"]))
            {
                hdnId.Value = this.Request.QueryString["id"];
            }
            if (this.Request.QueryString["moduleName"] != null && !string.IsNullOrEmpty(this.Request.QueryString["moduleName"]))
            {
                moduleName = this.Request.QueryString["moduleName"];
            }

            RuleCataLogRunTimeInfo c = new RuleCataLogRunTimeInfo();
            c.moduleNameFromUrl = moduleName;
            c.subModuleNameFromUrl = subModuleName;
            c.userName = SessionInfo.LoginName;
            string script = "srmRuleCatalog.SMRuleCataLogRunTimeStatus('" + serializer.Serialize(c) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), (this.GetType().GetHashCode() + 123).ToString(), script, true);
        }
        public class RuleCataLogRunTimeInfo
        {
            public string moduleNameFromUrl { get; set; }
            public string subModuleNameFromUrl { get; set; }

            public string userName { get; set; }
        }
        protected void smMaster_AsyncPostBackError(object sender, AsyncPostBackErrorEventArgs e)
        {
            StringBuilder str = new StringBuilder();
            string serializedObject = null;
            str.Append("<DIV>");
            str.Append("<DIV id=\"alertMsg\">");
            str.Append(e.Exception.Message);
            str.Append("</DIV>");

            BinaryFormatter serializer = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            try
            {
                serializer.Serialize(stream, e.Exception);
                string serializedData = Convert.ToBase64String(stream.ToArray(),
                    Base64FormattingOptions.InsertLineBreaks);
                serializedObject = ReplaceNonSerializableCharacters(serializedData);
            }
            catch (Exception ex)
            {
                if (e.Exception.InnerException == null)
                    serializedObject = ReplaceNonSerializableCharacters("The Exception object could not be serialized. <br />" +
                      ex.Message + "<br /> The original exception was : " + e.Exception.Message);
                else
                    serializedObject = ReplaceNonSerializableCharacters("The Exception object could not be serialized. <br />" +
                          ex.Message + "<br /> The original exception was : " + e.Exception.InnerException.Message);
            }
            finally
            {
                stream.Close();
            }
            str.Append("<DIV id=\"serObj\" style=\"display:none\">");
            str.Append(serializedObject);
            str.Append("</DIV>");
            smMaster.AsyncPostBackErrorMessage = str.ToString();
        }

        private string ReplaceNonSerializableCharacters(string Message)
        {
            Regex regEx = new Regex("'", RegexOptions.None);
            Message = regEx.Replace(Message, "\"");

            regEx = new Regex("->", RegexOptions.None);
            Message = regEx.Replace(Message, "<br/>");

            return Message;
        }
    }
}