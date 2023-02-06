using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using System.Data;
using com.ivp.rad.BusinessCalendar;
using com.ivp.rad.viewmanagement;
using com.ivp.secm;
//using com.ivp.secm.core;
using com.ivp.rad.utils;
using com.ivp.refmaster.refmasterwebservices;
using com.ivp.common;
using System.Reflection;
using System.Globalization;

namespace com.ivp.common.CommonUI.CommonUI
{
    public partial class SRMDownstreamStatus : BasePage
    {

        private string productName = string.Empty;

        #region Declarations
        DataSet mDsSystems = null;

        public enum SRMDownstreamStatusTypes
        {
            /// <summary>
            /// Success Staus
            /// </summary>
            Success = 1,
            /// <summary>
            /// Failed Status
            /// </summary>
            Failure = 2,
            /// <summary>
            /// INPROGRESS status
            /// </summary>
            In_Progress = 3,
            /// <summary>
            /// NOTPROCESSED status
            /// </summary>
            Not_Processed = 4
        }
        public class SRMDownstreamStatusControlInfo
        {
            // public string DivExceptionDateId { get; set; }
            public string HdnExceptionDateId { get; set; }
            public string LabeldateselectiondivId { get; set; }
            // public string TxtTimerId { get; set; }
            public string BtnGetStatusId { get; set; }
            // public string HdnTimerId { get; set; }
            public string RblstDateId { get; set; }
            public string LblStartDateId { get; set; }
            public string TxtStartDateId { get; set; }
            public string LblStartDateErrorId { get; set; }
            public string LblEndDateId { get; set; }
            public string TxtEndDateId { get; set; }
            public string LblEndDateErrorId { get; set; }
            public string LblValidationSummaryId { get; set; }

            public string ProductName { get; set; }

            public string UserName { get; set; }
            public string moduleIdToDisplay { get; set; }

        }

        public class SRMDownstreamStatusInfo
        {
            public string CustomJQueryDateTimeFormat { get; set; }
            public string CustomJQueryDateFormat { get; set; }
            public string ServerShortDateFormat { get; set; }
            public string ServerLongDateFormat { get; set; }
        }
        #endregion

        #region INFO AND CONTROL IDS TO BE SEND TO SCRIPTS
        private SRMDownstreamStatusInfo GetCommonStatusInfo()
        {
            SRMDownstreamStatusInfo securityInfo = new SRMDownstreamStatusInfo();
            securityInfo.CustomJQueryDateTimeFormat = SMUiUtils.ConvertDateTimeFormatToCustomDatePickerFormat(SessionInfo.CultureInfo.LongDateFormat, SMDateTimeOptions.DATETIME);
            securityInfo.CustomJQueryDateFormat = SMUiUtils.ConvertDateTimeFormatToCustomDatePickerFormat(SessionInfo.CultureInfo.ShortDateFormat, SMDateTimeOptions.DATE);
            securityInfo.ServerShortDateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            securityInfo.ServerLongDateFormat = CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern;
            return securityInfo;
        }
        private SRMDownstreamStatusControlInfo GetCommonStatusControlInfo()
        {
            SRMDownstreamStatusControlInfo controlInfo = new SRMDownstreamStatusControlInfo();
            controlInfo.LabeldateselectiondivId = labeldateselectiondiv.ClientID;
            //controlInfo.DivExceptionDateId = divExceptionDate.ClientID;
            controlInfo.HdnExceptionDateId = hdnExceptionDate.ClientID;
            //controlInfo.TxtTimerId = txtTimer.ClientID;
            controlInfo.BtnGetStatusId = btnGetStatus.ClientID;
            //controlInfo.HdnTimerId = hdnTimer.ClientID;
            controlInfo.RblstDateId = rblstDate.ClientID;
            controlInfo.LblStartDateId = lblStartDate.ClientID;
            controlInfo.TxtStartDateId = TextStartDate.ClientID;
            controlInfo.LblStartDateErrorId = lblStartDateError.ClientID;
            controlInfo.LblEndDateId = lblEndDate.ClientID;
            controlInfo.TxtEndDateId = TextEndDate.ClientID;
            controlInfo.LblEndDateErrorId = lblEndDateError.ClientID;
            controlInfo.LblValidationSummaryId = lblValidationSummary.ClientID;
            controlInfo.ProductName = RConfigReader.GetConfigAppSettings("ProductName");
            controlInfo.UserName = SessionInfo.LoginName;
            return controlInfo;
        }
        #endregion
        protected void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);
            hdnLongDateFormat.Value = SessionInfo.CultureInfo.LongDateFormat;

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string script = "var objSMSCommonStatusScreen = new SMSSRMDownstreamStatusScreen('" + serializer.Serialize(GetCommonStatusInfo()) + "','" + serializer.Serialize(GetCommonStatusControlInfo()) + "');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), (this.GetType().GetHashCode() + 123).ToString(), script, true);

            bool isPostBack = IsPostBack;
            if (!isPostBack)
            {
                if (this.Request.QueryString["issinglesecurity"] != null)
                {
                    string path = this.Request.Url.ToString().Substring(this.Request.Url.ToString().IndexOf("SRMDownstreamStatus&") + 19);
                    hdnUrl.Value = path;
                    if (!string.IsNullOrEmpty(this.Request.QueryString["sec_ids"]))
                        hdnSingleSecurityValue.Value = this.Request.QueryString["sec_ids"];
                    hdnIsSingleSecurity.Value = "true"; //Add it
                    if (this.Request.QueryString["marked"] != null)
                    {
                        ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), "SetDateCtrl", "setDateControl('" + this.Request.QueryString["marked"] + "','" + labeldateselectiondiv.ClientID + "','" + hdnExceptionDate.ClientID + "');", true);
                    }
                }


                TextStartDate.Text = RCalenderUtils.FormatDate(DateTime.Today, SessionInfo.CultureInfo.LongDateFormat);
                TextEndDate.Text = (RCalenderUtils.FormatDate(DateTime.Now, SessionInfo.CultureInfo.ShortDateFormat)).Split(' ')[0];
                productName = RConfigReader.GetConfigAppSettings("ProductName");
                // if (productName.ToLower() == "secmaster")
                // BindSystems();
                // else

                //{
                // BindSystemsRef();
                //  }

                //  BindTaskStatus();
            }
        }
        public void btnGetStatus_Click(object sender, EventArgs e)
        {

        }

        private void BindSystemsRef()
        {
            DataTable dtRefDownstreamSystems = null;
            RMRefMasterAPI refMAPI = new RMRefMasterAPI();
            dtRefDownstreamSystems = refMAPI.GetEntityDownstreamSystems();
            List<string> l = new List<string>();
            for (int i = 0; i < dtRefDownstreamSystems.Rows.Count; i++)
            {
                if (!(l.Contains(dtRefDownstreamSystems.Rows[i]["report_system_name"].ToString(), StringComparer.CurrentCultureIgnoreCase)))
                    l.Add(dtRefDownstreamSystems.Rows[i]["report_system_name"].ToString());
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("system_name", typeof(string));
            for (int i = 0; i < l.Count; i++)
                dt.Rows.Add(l[i]);

            if (dtRefDownstreamSystems != null)
            {

                ddlSystems.DataTextField = "system_name";
                ddlSystems.DataValueField = "system_name";

                ddlSystems.DataSource = dt;
                ddlSystems.DataBind();
                //ddlSystems.Items.Add(new ListItem("--ALL--", "0"));
                //AddNewElement(ddlSystems, "All systems");
                ddlSystems.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All systems", "All systems"));
                ddlSystems.SelectedIndex = 0;
            }
        }

        private void BindSystems()
        {
            //productName = RConfigReader.GetConfigAppSettings("ProductName");
            DataTable dtRefDownstreamSystems = null;
            DataTable dtSecDownstreamSystems = null;


            //reflection
            //SMRefDataInterfaceManager objSMRefDataInterfaceManager = new SMRefDataInterfaceManager();
            //dtRefDownstreamSystems = objSMRefDataInterfaceManager.GetEntityDownstreamSystems();

            Assembly RefMAPI = Assembly.Load("RefMAPI");
            Type RMRefMasterAPI = null;
            RMRefMasterAPI = RefMAPI.GetType("com.ivp.refmaster.refmasterwebservices.RMRefMasterAPI");
            object RMRefMasterAPIObj = null;
            RMRefMasterAPIObj = Activator.CreateInstance(RMRefMasterAPI);
            MethodInfo getEntityDownstreamSystems = RMRefMasterAPI.GetMethod("GetEntityDownstreamSystems");
            //DataTable dt = new SMBulkEditController().GetExternalSystems();
            dtRefDownstreamSystems = (DataTable)getEntityDownstreamSystems.Invoke(RMRefMasterAPIObj, new object[] { });



            dtSecDownstreamSystems = WorkflowController.SMGetDownStreamSystems(this.SessionInfo.LoginName);
            // new SMSearchController().GetExternalSystemDetails(this.SessionInfo.LoginName);


            List<string> l = new List<string>();
            if (dtSecDownstreamSystems != null)
                for (int i = 0; i < dtSecDownstreamSystems.Rows.Count; i++)
                {
                    if (!(l.Contains(dtSecDownstreamSystems.Rows[i]["system_name"].ToString(), StringComparer.CurrentCultureIgnoreCase)))
                        l.Add(dtSecDownstreamSystems.Rows[i]["system_name"].ToString());
                }
            for (int i = 0; i < dtRefDownstreamSystems.Rows.Count; i++)
            {
                if (!(l.Contains(dtRefDownstreamSystems.Rows[i]["report_system_name"].ToString(), StringComparer.CurrentCultureIgnoreCase)))
                    l.Add(dtRefDownstreamSystems.Rows[i]["report_system_name"].ToString());
            }

            DataTable dt = new DataTable();
            dt.Columns.Add("system_name", typeof(string));
            for (int i = 0; i < l.Count; i++)
                dt.Rows.Add(l[i]);





            if (dtSecDownstreamSystems != null)
            {

                ddlSystems.DataTextField = "system_name";
                ddlSystems.DataValueField = "system_name";

                ddlSystems.DataSource = dt;
                ddlSystems.DataBind();
                //ddlSystems.Items.Add(new ListItem("--ALL--", "0"));
                //AddNewElement(ddlSystems, "All systems");
                ddlSystems.Items.Insert(0, new System.Web.UI.WebControls.ListItem("All systems", "All systems"));
                ddlSystems.SelectedIndex = 0;
            }
        }

        private void BindTaskStatus()
        {
            try
            {
                ddlTaskStatus.Items.Add(SRMDownstreamStatusTypes.Failure.ToString());
                ddlTaskStatus.Items.Add(SRMDownstreamStatusTypes.Success.ToString());
                ddlTaskStatus.Items.Add(SRMDownstreamStatusTypes.In_Progress.ToString());
                ddlTaskStatus.Items.Add(SRMDownstreamStatusTypes.Not_Processed.ToString().Replace('_', ' '));
                AddNewElement(ddlTaskStatus, "Any");
                ddlTaskStatus.SelectedIndex = 0;
            }
            catch (Exception ex) { throw ex; }
            finally
            {

            }
        }
        private void AddNewElement(DropDownList ddlList, string text)
        {
            ddlList.Items.Insert(0, new System.Web.UI.WebControls.ListItem(text, "-1"));
        }



    }
}