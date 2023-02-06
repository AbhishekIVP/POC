using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using com.ivp.rad.utils;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using com.ivp.rad.controls.neogrid.client.info;
using System.Drawing;
using System.Web.Services;
using System.Web.Script.Services;
using System.Globalization;
using com.ivp.rad.viewmanagement;
using com.ivp.rad.configurationmanagement;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class SRMGridWriter : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "gridwriter.GridWriterInitializer('" + serializer.Serialize(GetSMSRMGridWriterControlsInfo()) + "','" + serializer.Serialize(GetSMSRMGridWriterInfo()) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), (this.GetType().GetHashCode() + 123).ToString(), script, true);
        }

        public SMSRMGridWriterControlsInfo GetSMSRMGridWriterControlsInfo()
        {
            SMSRMGridWriterControlsInfo controlInfo = new SMSRMGridWriterControlsInfo();
            return controlInfo;
        }

        public SMSRMGridWriterInfo GetSMSRMGridWriterInfo()
        {
            int moduleId = -1;
            string reportName = this.Request.QueryString["ReportName"];
            string tabName = this.Request.QueryString["TabName"];

            if (Request.QueryString != null && !string.IsNullOrEmpty(Request.QueryString["moduleId"]))
                moduleId = Int32.Parse(Request.QueryString["moduleId"]);

            SMSRMGridWriterInfo info = new SMSRMGridWriterInfo();
            info.ReportName = (!string.IsNullOrEmpty(reportName) ? reportName : string.Empty);
            info.TabName = (!string.IsNullOrEmpty(tabName) ? tabName : string.Empty);
            info.UserName = SessionInfo.LoginName;
            info.ModuleId = moduleId;

            return info;
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

    public class SMSRMGridWriterInfo
    {
        public string ReportName { get; set; }
        public string TabName { get; internal set; }
        public string UserName { get; internal set; }
        public int ModuleId { get; set; }
    }

    public class SMSRMGridWriterControlsInfo
    {
    }
}