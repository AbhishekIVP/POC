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

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class WorkflowStatus : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "workflowstatus.SMSWorkflowStatus('" + serializer.Serialize(GetSMWorkflowStatusControlsInfo()) + "','" + serializer.Serialize(GetSMWorkflowStatusInfo()) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), (this.GetType().GetHashCode() + 123).ToString(), script, true);
        }

        public SMWorkflowStatusControlsInfo GetSMWorkflowStatusControlsInfo()
        {
            SMWorkflowStatusControlsInfo controlInfo = new SMWorkflowStatusControlsInfo();
            controlInfo.PanelTopId = panelTop.ClientID;
            controlInfo.PanelMiddleId = panelMiddle.ClientID;
            controlInfo.PanelBottomId = panelBottom.ClientID;
            controlInfo.ModalSuccessId = modalSuccess.BehaviorID;
            controlInfo.ModalErrorId = modalError.BehaviorID;
            controlInfo.ModalPanelSaveId = modalpanelSave.BehaviorID;
            controlInfo.LblSuccessId = lblSuccess.ClientID;
            controlInfo.LblErrorId = lblError.ClientID;

            return controlInfo;
        }

        public SMWorkflowStatusInfo GetSMWorkflowStatusInfo()
        {
            string IsDemoBuild = RConfigReader.GetConfigAppSettings("IsDemoBuild");
            string productName = RConfigReader.GetConfigAppSettings("ProductName");

            SMWorkflowStatusInfo info = new SMWorkflowStatusInfo();
            info.UserName = SessionInfo.LoginName;
            info.WorkflowType = (this.Request.QueryString["workflowType"] != null ? this.Request.QueryString["workflowType"] : string.Empty);
            info.SectypeIds = (this.Request.QueryString["secTypeIds"] != null ? this.Request.QueryString["secTypeIds"] : "-1");
            info.SecurityId = (this.Request.QueryString["secId"] != null ? this.Request.QueryString["secId"] : "");
            info.SelectedTab = (this.Request.QueryString["selectedTab"] != null ? this.Request.QueryString["selectedTab"] : "");
            info.LongDateFormat = SessionInfo.CultureInfo.LongDateFormat;
            info.ShortDateFormat = SessionInfo.CultureInfo.ShortDateFormat;
            info.IsDemoBuild = ((!string.IsNullOrEmpty(IsDemoBuild) && IsDemoBuild.Equals("true", StringComparison.OrdinalIgnoreCase)) ? true : false);
            info.ProductName = ((string.IsNullOrEmpty(productName) || productName.Equals("secmaster", StringComparison.OrdinalIgnoreCase)) ? "SecMaster" : "RefMaster");
            if (info.ProductName.Equals("RefMaster", StringComparison.OrdinalIgnoreCase))
                info.SelectedTab = "RefMaster";

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

    public class SMWorkflowStatusInfo
    {
        public string UserName { get; set; }

        public string WorkflowType { get; set; }

        public string ShortDateFormat { get; set; }

        public string LongDateFormat { get; set; }

        public string SectypeIds { get; set; }

        public string SecurityId { get; set; }

        public bool IsDemoBuild { get; set; }
        public string SelectedTab { get; internal set; }
        public string ProductName { get; internal set; }
    }

    public class SMWorkflowStatusControlsInfo
    {
        public string PanelTopId { get; set; }

        public string PanelMiddleId { get; set; }

        public string PanelBottomId { get; set; }

        public string ModalSuccessId { get; set; }

        public string ModalErrorId { get; set; }

        public string LblSuccessId { get; set; }

        public string LblErrorId { get; set; }

        public string ModalPanelSaveId { get; set; }

    }
}