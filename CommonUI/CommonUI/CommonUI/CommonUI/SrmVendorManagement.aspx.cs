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
    public partial class SrmVendorManagement : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "SMSSRMVendorManagement('" + serializer.Serialize(GetSMSRMVendorManagementControlsInfo()) + "','" + serializer.Serialize(GetSMSRMVendorManagementInfo()) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), (this.GetType().GetHashCode() + 123).ToString(), script, true);
        }

        public SMSRMVendorManagementControlsInfo GetSMSRMVendorManagementControlsInfo()
        {
            SMSRMVendorManagementControlsInfo controlInfo = new SMSRMVendorManagementControlsInfo();
            controlInfo.ModalSuccessId = modalSuccess.BehaviorID;
            controlInfo.ModalErrorId = modalError.BehaviorID;
            controlInfo.LblSuccessId = lblSuccess.ClientID;
            controlInfo.LblErrorId = lblError.ClientID;

            return controlInfo;
        }

        public SMSRMVendorManagementInfo GetSMSRMVendorManagementInfo()
        {
            SMSRMVendorManagementInfo info = new SMSRMVendorManagementInfo();
            info.UserName = SessionInfo.LoginName;

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

    public class SMSRMVendorManagementInfo
    {
        public string UserName { get; set; }
    }

    public class SMSRMVendorManagementControlsInfo
    {
        public string ModalSuccessId { get; set; }

        public string ModalErrorId { get; set; }

        public string LblSuccessId { get; set; }

        public string LblErrorId { get; set; }
    }
}