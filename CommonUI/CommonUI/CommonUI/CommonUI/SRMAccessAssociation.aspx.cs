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
using com.ivp.rad.culturemanager;
using com.ivp.rad.viewmanagement;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class SRMAccessAssociation : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

        }

        protected override void OnPreRender(EventArgs e)
        {
            var typeId = Convert.ToString(Request.QueryString["typeId"]);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "srmAssociation.Init('" + SessionInfo.LoginName + "','" + typeId + "'); ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "scriptToInitializeDashboard", script, true);
            base.OnPreRender(e);
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
           // smMaster.AsyncPostBackErrorMessage = str.ToString();
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