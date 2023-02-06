using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.ivp.rad.viewmanagement;
using System.Web.Script.Serialization;
using com.ivp.srmcommon;

namespace com.ivp.secm.ui
{
    public partial class TaskManagerControl : BaseUserControl//System.Web.UI.UserControl
    {
        public string calendarNames { get; set; }
        public string subscribeList { get; set; }
        //protected void Page_Load(object sender, EventArgs e)
        protected override void PageLoadEvent(object sender, EventArgs e)
        {
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.js.Jquery10.jquery.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.js.Jquery10.jquery-ui.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.js.jsPlumb.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.includes.SMSelect.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.includes.jquery.slimscroll.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.includes.knockout-3.4.0.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.includes.slimScrollHorizontal.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.includes.ruleEditorScroll.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.includes.SMSlimscroll.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.js.taskManagerMain.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.includes.KOMappingLibrary.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.includes.SMTaskSetup.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.includes.RAD.jquerydatetimepicker.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.includes.RAD.RADCustomDateTimePicker.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.js.SRMMailTags.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.js.moment.js", this.GetType().BaseType.Assembly.FullName));
            //RCommonTaskManager.RCTMUtils rctm = new RCommonTaskManager.RCTMUtils();
            //rctm.AddTaskStatus(108, 1, new RCTMUtils.TaskStatusInfo() { Status = RCTMUtils.TaskStatus.INPROGRESS});
            loginName_hf.Value = this.SessionInfo.UserName;
            clientName_hf.Value = SRMMTConfig.GetClientName();
            servicePath.Value = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpRuntime.AppDomainAppVirtualPath;
            var longDateFormat = SessionInfo.CultureInfo.ShortDateFormat + " " + SessionInfo.CultureInfo.LongTimePattern;
            var shortDateFormat = SessionInfo.CultureInfo.ShortDateFormat;
            string script = "javascript: smtaskManagerMain.RAD_init_common_task_manager('" + shortDateFormat + "','" + longDateFormat + "');";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), this.Page.GetHashCode().ToString(), script, true);
            hdn_SetShortDateFormat.Value = SMUiUtils.ConvertDateTimeFormatToCustomDatePickerFormat(SessionInfo.CultureInfo.ShortDateFormat, SMDateTimeOptions.DATE); //"YYYY/MM/dd"
        }
    }
}