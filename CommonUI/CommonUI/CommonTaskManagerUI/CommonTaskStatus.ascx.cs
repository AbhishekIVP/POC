using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using com.ivp.rad.controls.xlgrid.client.info;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel;
using System.Text;
using com.ivp.rad.RCommonTaskManager;
using com.ivp.rad.dal;
using com.ivp.rad.utils;
using com.ivp.rad.viewmanagement;
using com.ivp.rad.configurationmanagement;
using com.ivp.rad.RCTMUtils;
using Newtonsoft.Json;
using com.ivp.srmcommon;

namespace com.ivp.secm.ui
{
    public partial class CommonTaskStatus : BaseUserControl, ICTMServiceCallback//System.Web.UI.UserControl
    {
        static string DBConnectionId = RADConfigReader.GetConfigAppSettings("CTMDBConnectionId");
        public string imagePath = "app_themes/aqua/images";
        private string JQueryDateFormat { get; set; }
        string strSelectText = "-- ALL --";
        string strSelectValue = "-1";
        Dictionary<string, string> myViewState = new Dictionary<string, string>();
        Boolean isUnsyncdTasksExists = false;
        static List<int> unsyncdClientTaskStatusIds = new List<int>();
        static bool hasPriviledge = false;

        protected override void PageLoadEvent(object sender, EventArgs e)
        {
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.js.Jquery10.jquery.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.js.Jquery10.jquery-ui.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.includes.jquery.slimscroll.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.includes.slimScrollHorizontal.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.includes.ruleEditorScroll.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.includes.TaskStatus.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.includes.SMSlimscroll.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.includes.TaskStatusInfo.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.js.taskStatusMain.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.js.SMSlideMenu.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.js.SMSlideMenu.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.js.viewManager.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.includes.RAD.jquerydatetimepicker.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.includes.RAD.RADCustomDateTimePicker.js", this.GetType().BaseType.Assembly.FullName));
            ScriptManager.GetCurrent(this.Page).Scripts.Add(new ScriptReference("com.ivp.secm.ui.js.SMDatePickerControl.js", this.GetType().BaseType.Assembly.FullName));

            loginName_hf.Value = this.SessionInfo.LoginName;
            clientName_hf.Value = SRMMTConfig.GetClientName();
            servicePath.Value = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + HttpRuntime.AppDomainAppVirtualPath;
            //
            //if (fetchUnsyncdTasks() > 0) { unsyncdTasksFlag_hf.Value = "True"; }


            CommonService.CommonService obj = new CommonService.CommonService();
            hasPriviledge = obj.CheckControlPrivilegeForUser("Common_TaskStatus - btnDelete", SessionInfo.LoginName);

            string script = "RAD_init_common_task_status('" + RADXLGrid.ClientID + "');console.log('common task status page load event');";
            ScriptManager.RegisterStartupScript(this.Page, this.Page.GetType(), this.Page.GetHashCode().ToString(), script, true);
            if (!IsControlPostBack)
            {
                BindGrid();
                BindControls();
                isPostBack_hf.Value = "False";

            }
            else
            {


            }

        }
        private int fetchUnsyncdTasks()
        {
            DuplexChannelFactory<ICTMService> dupFactory = null;
            ICTMService clientProxy = null;
            int unsyncdTasksCount = 0;
            try
            {

                TaskStatusWebMethods cbk = new TaskStatusWebMethods();
                dupFactory = new DuplexChannelFactory<ICTMService>(cbk, "NetTcpBinding_ICTMService");
                dupFactory.Open();
                clientProxy = dupFactory.CreateChannel();
                unsyncdClientTaskStatusIds.Clear();
                unsyncdClientTaskStatusIds = clientProxy.GetUnsyncdTasksClientTaskStatusIds(string.Empty);


                unsyncdTasksCount += unsyncdClientTaskStatusIds.Count;
            }
            catch (EndpointNotFoundException timeProblem)
            {
                Console.WriteLine("The service operation timed out. " + timeProblem.Message);
                dupFactory.Abort();
                //throw;
            }
            catch (Exception ex)
            {

                throw;
            }
            RDBConnectionManager dbConnection = null;
            RHashlist htParams = new RHashlist();
            try
            {

                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
                unsyncdTasksCount += Convert.ToInt32(dbConnection.ExecuteQuery("CTM:CountClientStatusIdNullAndInprogress", null).Tables[0].Rows[0][0]);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }
            if (unsyncdTasksCount > 0) { isUnsyncdTasksExists = true; }
            return unsyncdTasksCount;
        }
        protected void btnGetStatus_Click(object sender, EventArgs e)
        {
            BindGrid();
            isPostBack_hf.Value = "True";
            endDate_hf.Value = txtEndDate.Text;
            startDate_hf.Value = txtStartDate.Text;


        }
        protected void btnGetUnsyncdTasks_Click(object sender, EventArgs e)
        {

            try
            {
                //if (unsyncdClientTaskStatusIds.Count > 0)
                if (isUnsyncdTasksExists == true)
                {
                    string condition = "";
                    if (unsyncdClientTaskStatusIds.Count > 0)
                    {
                        String clientTaskIds = string.Join(",", unsyncdClientTaskStatusIds.Select(x => x.ToString()).ToArray());
                        condition = " client_task_status_id in(" + clientTaskIds.TrimEnd(',') + ") or (client_task_status_id is null  and status = 'inprogress' ) ";
                    }
                    else
                    {
                        condition = " client_task_status_id is null and status = 'inprogress' ";

                    }
                    string con = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId).ConnectionString;// DBConnectionId;//@"Data Source=192.168.0.63\LOC05E;Initial Catalog=IVPSRMTaskManager;User ID=sa;password=sa@123;Min Pool Size=10;Max Pool Size=100;Connect Timeout=10;Application Name=CosmosRecon;";
                    // unsyncdClientTaskStatusIds.Aggregate((i, j) => Convert.ToString(i) + "," + Convert.ToString(j));// "";
                    string sql = "SELECT chained_tasks.chain_name \"Task Group Name\", taskSummary.task_name \"Task Name\",flow.module_name\"Module Name\", taskSummary.task_type_name\"Task Type\",[start_time]  \"Start Time\",[end_time] \"End Time\",[status] \"Status\", [task_status_id] FROM [dbo].[ivp_rad_task_status] taskStatus join [dbo].[ivp_rad_flow] flow on taskStatus.flow_id = flow.flow_id join [dbo].[ivp_rad_task_summary] taskSummary on flow.task_summary_id = taskSummary.task_summary_id  join ivp_rad_chained_tasks chained_tasks on chained_tasks.chain_id = flow.chain_id where " + condition + " order by Cast (start_time as datetime) DESC";
                    //                    string sql = @"SELECT chained_tasks.chain_name, taskSummary.task_name,flow.module_name, taskSummary.task_type_name,CAST([start_time] as varchar(50)) start_time,CAST([end_time] as varchar(50)) end_time,[status], [task_status_id],[log_description],task_master_id,taskStatus.[flow_id]  FROM [IVPSRMTaskManager].[dbo].[ivp_rad_task_status] taskStatus join [IVPSRMTaskManager].[dbo].[ivp_rad_flow] flow on taskStatus.flow_id = flow.flow_id join [IVPSRMTaskManager].[dbo].[ivp_rad_task_summary] taskSummary on flow.task_summary_id = taskSummary.task_summary_id  join ivp_rad_chained_tasks chained_tasks on chained_tasks.chain_id = flow.chain_id where client_task_status_id in(" + clientTaskIds.TrimEnd(',') + ") order by Cast (start_time as datetime) DESC";
                    DataSet ds = new DataSet("ROOT");
                    SqlDataAdapter da = new SqlDataAdapter(sql, con);
                    da.Fill(ds);
                    bindRadXlGrid(ds);

                }
            }
            catch (Exception ex)
            {
                throw;
            }


        }
        private void BindControls()
        {
            BindTaskType();
            BindTaskStatus();
        }
        private void BindTaskStatus()
        {
            ddlTaskStatus.Items.Clear();
            ddlTaskStatus.Items.Add("Failed");
            ddlTaskStatus.Items.Add("Inprogress");
            ddlTaskStatus.Items.Add("Passed");
            ddlTaskStatus.Items.Add("Queued");
            AddNewElement(ddlTaskStatus);//adds --All-- at start
            ddlTaskType.Items.Remove("");
            if (ddlTaskStatus != null)
            {
                foreach (ListItem li in ddlTaskStatus.Items)
                {
                    li.Attributes["title"] = li.Text;
                }
            }
            ddlTaskStatus.SelectedIndex = 0;

        }
        private void BindTaskType()
        {
            RDBConnectionManager dbConnection = null;
            RHashlist htParams = new RHashlist();
            try
            {
                dbConnection = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
                ddlTaskType.DataSource = dbConnection.ExecuteQuery("CTM:TaskTypeNameTaskTypeId", null).Tables[0];
                ddlTaskType.DataTextField = "task_type_name";
                ddlTaskType.DataValueField = "task_type_name";
                //ddlTaskType.dat
                ddlTaskType.DataBind();
                ddlTaskType.Items.Remove("");
                if (ddlTaskType != null)
                {
                    foreach (ListItem li in ddlTaskType.Items)
                    {
                        li.Attributes["title"] = li.Text;
                    }
                }
                AddNewElement(ddlTaskType);//adds --All-- at start
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                RDALAbstractFactory.DBFactory.PutConnectionManager(dbConnection);
                if (htParams != null)
                    htParams = null;
            }
        }
        private string GetText(object data)
        {
            string stringData = Convert.ToString(data);
            if (string.IsNullOrWhiteSpace(stringData))
                return "&nbsp;";
            else
                return stringData;
        }
        private void AddNewElement(DropDownList ddlList)
        {
            ddlList.Items.Insert(0, new ListItem(strSelectText, strSelectValue));
        }
        private List<CustomRowDataInfo> GetCustomRowDataInfo(DataTable dt, int hasColumn)
        {
            List<CustomRowDataInfo> lstCustomRowDataInfo = new List<CustomRowDataInfo>();
            string deleteClassName = "";
            string image = "";
            string deleteTitle = "";

            if (hasPriviledge)
            {
                deleteClassName = "imgBtnDelete";
                image = "/deleteIcon.png";
                deleteTitle = "Click to Delete";

            }
            else
            {
                image = "/deleteIcon_disabled.gif";
                deleteTitle = "You do not have priviledge to delete this task status.";
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                lstCustomRowDataInfo = new List<CustomRowDataInfo>();
                int rowCounter = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    CustomRowDataInfo objCustomRowDataInfo = new CustomRowDataInfo();

                    CustomCellDatainfo cellTaskHistoryGanttChart = new CustomCellDatainfo();
                    cellTaskHistoryGanttChart.Append = false;
                    cellTaskHistoryGanttChart.ColumnName = "task_history_chart";
                    cellTaskHistoryGanttChart.NewChild = "<div class='viewTaskHistoryChart' style='text-align:center;cursor:pointer;'><img data-flow_id='" + dr["flow_id"] + "' src='" + imagePath + "/icons/ico_report.png'></div>";

                    CustomCellDatainfo cellDelete = new CustomCellDatainfo();
                    cellDelete.Append = false;
                    cellDelete.ColumnName = "Delete";
                    cellDelete.NewChild = "<div style=\"text-align:center;cursor:pointer;\"><img data-task_status_id=" + dr["task_status_id"] + "  data-task_name = '" + dr["task_name"] + "' class='" + deleteClassName + "' id=\"imgBtnDelete_" + rowCounter + "\" title=\"" + deleteTitle + "\" src=\"" + imagePath + image + "\" type=\"image\"/></div>";
                    StringBuilder btnDeleteOnClick = new StringBuilder();

                    CustomCellDatainfo cellLogDesc = new CustomCellDatainfo();
                    cellLogDesc.Append = false;
                    cellLogDesc.ColumnName = "log_description";
                    cellLogDesc.NewChild = "<span id=\"lblLogDesc_" + rowCounter + "\">" + Server.HtmlEncode(GetText(dr["log_description"])) + "</span>";


                    CustomCellDatainfo cellRetry = new CustomCellDatainfo();
                    cellRetry.Append = false;
                    cellRetry.ColumnName = "Retry";
                    cellRetry.NewChild = "<div class='retryBtn' style=\"text-align:center;cursor:pointer;\"><img id=\"imgBtnRetry_" + rowCounter + "\" title=\"You Cannot Retry This Request\" style=\"border-right-width: 0px; border-top-width: 0px; border-bottom-width: 0px; border-left-width: 0px;\"  src=\"" + imagePath + "/retry.png\" Text=\"Retry\"/></div>";

                    CustomCellDatainfo cellUndo = new CustomCellDatainfo();
                    cellUndo.Append = true;
                    cellUndo.ColumnName = "undo";
                    cellUndo.NewChild = "";
                    if (Convert.ToBoolean(dr["is_undoable"]) == false)
                    { cellUndo.NewChild = string.Empty; }

                    CustomCellDatainfo TriggerredBy = new CustomCellDatainfo();
                    TriggerredBy.Append = false;
                    TriggerredBy.ColumnName = "Username";
                    String username = "";
                    try
                    {
                        Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(dr["environment_variables"].ToString());

                        if (values != null && values.ContainsKey("username"))
                        {
                            username = values["username"];
                        }
                    }
                    catch (Exception ex) { }
                    TriggerredBy.NewChild = username;

                    CustomCellDatainfo cellStatus = new CustomCellDatainfo();
                    cellStatus.Append = false;
                    cellStatus.ColumnName = "status";
                    string status = GetText(dr["status"]).ToUpper();
                    switch (status)
                    {
                        case "PASSED":
                            cellStatus.NewChild = "<span id=\"lblStatus_" + rowCounter + "\" style='color:green;font-weight:bold;'>" + status + "</span>";
                            cellDelete.NewChild = "<div style=\"text-align:center;\"><img id=\"imgBtnDelete_" + rowCounter + "\" src=\"" + imagePath + "/deleteIcon_disabled.gif\" type=\"image\" title=\"This System Status cannot be Deleted\"/></div>";
                            cellRetry.NewChild = "";
                            cellUndo.NewChild = "<div class='undoBtn' data-task_status_id=" + dr["task_status_id"] + " style=\"text-align:center;cursor:pointer\"><img id=\"imgBtnUndo_" + rowCounter + "\"  style=\"border-right-width: 0px; border-top-width: 0px; border-bottom-width: 0px; border-left-width: 0px;\"  src=\"http://png-4.findicons.com/files/icons/2226/matte_basic/16/undo.png\" /></div>";
                            break;
                        case "FAILED":
                            cellStatus.NewChild = "<span id=\"lblStatus_" + rowCounter + "\" style='color:rgb(236, 58, 58);font-weight:bold;'>" + status + "</span>";

                            if (dr["is_retryable"] != DBNull.Value && Convert.ToBoolean(dr["is_retryable"]) == false)//
                                cellRetry.NewChild = string.Empty;
                            else
                                cellRetry.NewChild = "<div style=\"text-align:center;cursor:pointer;\"><img data-flow_id=" + dr["flow_id"] + " class='retryBtn' id=\"imgBtnRetry_" + rowCounter + "\" title=\"Click To Retry\" style=\"border-right-width: 0px; border-top-width: 0px; border-bottom-width: 0px; border-left-width: 0px;\" src=\"" + imagePath + "/retry.png\" FlowId=\"" + Convert.ToString(dr["flow_id"]) + "\" Text=\"Retry\"/></div>";
                            break;
                        case "INPROGRESS":
                            cellStatus.NewChild = "<span id=\"lblStatus_" + rowCounter + "\" style='color:rgb(20, 98, 213);font-weight:bold;'>" + status + "</span>";
                            cellRetry.NewChild = "";
                            break;
                        case "QUEUED":
                            cellStatus.NewChild = "<span id=\"lblStatus_" + rowCounter + "\" style='color:goldenrod;font-weight:bold;'>" + status + "</span>";
                            cellRetry.NewChild = "";
                            break;
                        default:
                            cellStatus.NewChild = "<span id=\"lblStatus_" + rowCounter + "\" style='color:black;font-weight:bold;'>" + status + "</span>";
                            break;
                    }

                    CustomCellDatainfo cellTaskName = new CustomCellDatainfo();
                    cellTaskName.Append = false;
                    cellTaskName.ColumnName = "task_name";
                    cellTaskName.NewChild = "<span id=\"lblTaskName_" + rowCounter + "\">" + GetText(dr["task_name"]) + "</span>";


                    CustomCellDatainfo cellTaskStatusId = new CustomCellDatainfo();
                    cellTaskStatusId.Append = false;
                    cellTaskStatusId.ColumnName = "task_status_id";
                    cellTaskStatusId.NewChild = "<span id=\"lblTaskStatusId_" + rowCounter + "\">" + GetText(dr["task_status_id"]) + "</span>";


                    CustomCellDatainfo cellViewLog = new CustomCellDatainfo();
                    cellViewLog.Append = true;
                    cellViewLog.ColumnName = "view_log";
                    if (Convert.ToString(dr["log_description"]).Length > 50)
                    {
                        dr["link_message"] = Convert.ToString(dr["log_description"]).Substring(0, 47).Replace("<br>", " ").Replace("<br />", " ").Replace("<br/>", " ").Replace("</br>", " ").Replace("</ br>", " ").Replace("<\\br>", " ").Replace("<\\ br>", " ") + "...";
                    }
                    else
                        dr["link_message"] = Convert.ToString(dr["log_description"]).Replace("<br>", " ").Replace("<br />", " ").Replace("<br/>", " ").Replace("</br>", " ").Replace("</ br>", " ").Replace("<\\br>", " ").Replace("<\\ br>", " ");

                    string js;
                    if (hasColumn == 1)
                        js = "onclick='showLogDetails(\"" + Server.HtmlEncode(System.Text.RegularExpressions.Regex.Replace(Convert.ToString(dr["log_description"]).Trim().Replace("\\", "\\\\").Replace("\"", "<ž>"), @"\r\n?|\n", " ")) + "\",\"" + Convert.ToString(dr["task_name"]) + "\",\"" + Server.HtmlEncode(Convert.ToString(dr["additional_info"]).Replace("\"", "'")) + "\")'";
                    else
                        js = "onclick='showLogDetails(\"" + Server.HtmlEncode(System.Text.RegularExpressions.Regex.Replace(Convert.ToString(dr["log_description"]).Trim().Replace("\\", "\\\\").Replace("\"", "<ž>"), @"\r\n?|\n", " ")) + "\",\"" + Convert.ToString(dr["task_name"]) + "\")'";

                    cellViewLog.NewChild = "<span  " + js + "  title=\"" + Server.HtmlEncode(Convert.ToString(dr["log_description"])) + "\" id=\"imgDetails_" + rowCounter + "\" style=\"text-decoration:underline;color:#1a1a1a;cursor:pointer;\" >" + Server.HtmlEncode(Convert.ToString(dr["link_message"]).Trim()) + "</span>";



                    objCustomRowDataInfo.RowID = Convert.ToString(dr["task_status_id"]);
                    objCustomRowDataInfo.Cells.Add(cellLogDesc);
                    objCustomRowDataInfo.Cells.Add(cellViewLog);
                    objCustomRowDataInfo.Cells.Add(cellDelete);

                    objCustomRowDataInfo.Cells.Add(cellRetry);
                    objCustomRowDataInfo.Cells.Add(cellUndo);
                    objCustomRowDataInfo.Cells.Add(cellStatus);
                    objCustomRowDataInfo.Cells.Add(cellTaskName);
                    objCustomRowDataInfo.Cells.Add(cellTaskStatusId);
                    objCustomRowDataInfo.Cells.Add(TriggerredBy);

                    objCustomRowDataInfo.Cells.Add(cellTaskHistoryGanttChart);


                    objCustomRowDataInfo.Attribute.Add("task_status_id", Convert.ToString(dr["task_status_id"]));
                    //objCustomRowDataInfo.Attribute.Add("task_type_id", Convert.ToString(dr["task_type_id"]));
                    objCustomRowDataInfo.Attribute.Add("log_description", Server.HtmlEncode(Convert.ToString(dr["log_description"])));
                    objCustomRowDataInfo.Attribute.Add("task_master_id", Convert.ToString(dr["task_master_id"]));
                    objCustomRowDataInfo.Attribute.Add("flow_id", Convert.ToString(dr["flow_id"]));

                    lstCustomRowDataInfo.Add(objCustomRowDataInfo);
                    rowCounter++;
                }
            }
            return lstCustomRowDataInfo.Count > 0 ? lstCustomRowDataInfo : null;
        }
        private void BindGrid()
        {
            string whereConditions = "";
            RDBConnectionManager conn = RDALAbstractFactory.DBFactory.GetConnectionManager(DBConnectionId);
            string con = conn.ConnectionString;// DBConnectionId;//@"Data Source=192.168.0.63\LOC05E;Initial Catalog=IVPSRMTaskManager;User ID=sa;password=sa@123;Min Pool Size=10;Max Pool Size=100;Connect Timeout=10;Application Name=CosmosRecon;";

            if (IsPostBack)
            {
                string selectedTopOption = hdnTopOption.Value;
                if (selectedTopOption == "-1")
                {
                    selectedTopOption = "0";
                }
                string CustomRadioOption = hdnCustomRadioOption.Value;
                string startDate = null;
                string endDate = null;

                switch (selectedTopOption)
                {
                    case "0": // Today
                        startDate = DateTime.Now.ToString("MM-dd-yyy");
                        //whereConditions = "where cast([start_time] as datetime) >= cast('" + txtStartDate.Text + "' as datetime) and cast (start_time as datetime) <= cast('" + txtEndDate.Text + " 23:59:59" + "' as datetime)";
                        // changing 'enddate check' to starttime check for enabling inprogress task to be visible on task status screen.
                        break;
                    case "1": // Since Yesterday
                        startDate = DateTime.Now.AddDays(-1).ToString("MM-dd-yyy");
                        break;
                    case "2":// This Week
                        startDate = txtStartDate.Text;
                        break;
                    case "3": // Anytime
                        break;
                    case "4": // Custom Date
                        if (CustomRadioOption == "0") // From
                            startDate = txtStartDate.Text;
                        else if (CustomRadioOption == "1") // Between
                        {
                            startDate = txtStartDate.Text;
                            endDate = txtEndDate.Text; 
                        }
                        else if (CustomRadioOption == "2") // Prior 
                            endDate = txtEndDate.Text;
                        break;
                    default:
                        break;

                }
                if (startDate != null || endDate != null)
                {
                    whereConditions = " where ( 1=1 ";
                    if (startDate != null)
                    {
                        whereConditions += "AND (cast(end_time as datetime)>= cast ('" + startDate + "' as datetime) OR end_time is null)";
                    }
                    if (endDate != null)
                    {
                        whereConditions += "AND cast(start_time as datetime)<= cast ('" + endDate + "' as datetime) ";
                    }
                    whereConditions += ") ";
                }



                string[] TaskTypeWithStatus = hdnGetTaskTypeWithStatus.Value.Split(new string[] { "<$>" }, StringSplitOptions.RemoveEmptyEntries);
                string[] taskType = TaskTypeWithStatus[0].Split(',');
                string[] taskStatus = TaskTypeWithStatus[1].Split(',');
                string taskTypeStr = null;
                foreach (var str in taskType)
                {
                    if (str != null)
                        taskTypeStr += "'" + str + "',";
                }
                taskTypeStr = taskTypeStr.Remove(taskTypeStr.Length - 1);

                string taskStatusStr = null;
                foreach (var str in taskStatus)
                {
                    if (str != null)
                        taskStatusStr += "'" + str + "',";
                }
                taskStatusStr = taskStatusStr.Remove(taskStatusStr.Length - 1);

                if (string.IsNullOrEmpty(taskTypeStr) == false)
                {
                    if (string.IsNullOrEmpty(whereConditions) == true)
                        whereConditions += " where taskSummary.task_type_name IN (" + taskTypeStr + ")";
                    else
                        whereConditions += " and taskSummary.task_type_name IN (" + taskTypeStr + ")";
                }
                if (string.IsNullOrEmpty(taskStatusStr) == false)
                {
                    if (string.IsNullOrEmpty(whereConditions) == true)
                        whereConditions += " where status IN (" + taskStatusStr + ")";
                    else
                        whereConditions += " and status IN (" + taskStatusStr + ")";

                }

            }
            else
            {
                DateTime tmp = DateTime.Now;
                //tmp.ho
                whereConditions = "where (cast(end_time as datetime)>= cast ('" + DateTime.Now.ToString("MM-dd-yyy") + "' as datetime) OR end_time is null)  ";
            }

            DataTable tdt = conn.ExecuteQuery(@"SELECT 1 AS [has_column]
                                FROM sys.tables st
                                INNER JOIN sys.columns sc ON st.object_id = sc.object_id
                                WHERE type = 'U' AND st.name = 'ivp_rad_task_status' AND sc.name = 'additional_info'", RQueryType.Select).Tables[0];

            int hasColumn = 0;
            if (tdt.Rows.Count > 0)
                hasColumn = Convert.ToInt32(Convert.ToString(tdt.Rows[0]["has_column"]));

            string sql;
            if (hasColumn == 1)
                sql = @"SELECT taskSummary.is_undoable,environment_variables, taskSummary.is_retryable,flow.module_name, chained_tasks.chain_name, taskSummary.task_name, taskSummary.task_type_name, start_time,end_time,[status], [task_status_id],[log_description],task_master_id,taskStatus.[flow_id],taskStatus.[additional_info]  FROM [dbo].[ivp_rad_task_status] taskStatus join [dbo].[ivp_rad_flow] flow on taskStatus.flow_id = flow.flow_id join [dbo].[ivp_rad_task_summary] taskSummary on flow.task_summary_id = taskSummary.task_summary_id left join ivp_rad_chained_tasks chained_tasks on chained_tasks.chain_id = flow.chain_id " + whereConditions + " order by Cast (start_time as datetime) DESC , Cast (end_time as datetime) DESC";
            else
                sql = @"SELECT taskSummary.is_undoable,environment_variables, taskSummary.is_retryable,flow.module_name, chained_tasks.chain_name, taskSummary.task_name, taskSummary.task_type_name, start_time,end_time,[status], [task_status_id],[log_description],task_master_id,taskStatus.[flow_id] FROM [dbo].[ivp_rad_task_status] taskStatus join [dbo].[ivp_rad_flow] flow on taskStatus.flow_id = flow.flow_id join [dbo].[ivp_rad_task_summary] taskSummary on flow.task_summary_id = taskSummary.task_summary_id left join ivp_rad_chained_tasks chained_tasks on chained_tasks.chain_id = flow.chain_id " + whereConditions + " order by Cast (start_time as datetime) DESC , Cast (end_time as datetime) DESC";

            RADXLGrid.ClearSerializationData = true;
            RADXLGrid.Visible = true;
            RADXLGrid.UserID = "admin";
            RADXLGrid.CurrentPageID = "CTMTaskStatus";
            RADXLGrid.IdColumnName = "task_status_id";
            RADXLGrid.SessionIdentifier = "abc";
            RADXLGrid.RequireFilter = true;
            DataSet ds = new DataSet("ROOT");

            SqlDataAdapter da = new SqlDataAdapter(sql, con);

            da.Fill(ds);
            ds.Tables[0].Columns.Add("Username");
            ds.Tables[0].Columns.Add("Retry");
            ds.Tables[0].Columns.Add("view_log");
            ds.Tables[0].Columns.Add("undo");
            ds.Tables[0].Columns.Add("Delete");
            ds.Tables[0].Columns.Add("link_message");
            ds.Tables[0].Columns.Add("task_history_chart");

            ds.Tables[0].TableName = "Table";

            if (ds.Tables[0].Rows.Count == 0)
            {
                showNoRecordsDiv_hf.Value = "True";
            }
            else
            {
                showNoRecordsDiv_hf.Value = "False";
            }

            RADXLGrid.ColumnsToHide.Add(new HiddenColumnInfo { ColumnName = "task_status_id", isDefault = true });
            RADXLGrid.ColumnsToHide.Add(new HiddenColumnInfo { ColumnName = "environment_variables", isDefault = true });

            // RADXLGrid.ColumnsToHide.Add(new HiddenColumnInfo { ColumnName = "task_type_id", isDefault = true });
            RADXLGrid.ColumnsToHide.Add(new HiddenColumnInfo { ColumnName = "log_description", isDefault = true });
            RADXLGrid.ColumnsToHide.Add(new HiddenColumnInfo { ColumnName = "task_master_id", isDefault = true });
            RADXLGrid.ColumnsToHide.Add(new HiddenColumnInfo { ColumnName = "flow_id", isDefault = true });
            RADXLGrid.ColumnsToHide.Add(new HiddenColumnInfo { ColumnName = "link_message", isDefault = true });
            RADXLGrid.ColumnsToHide.Add(new HiddenColumnInfo { ColumnName = "is_undoable", isDefault = true });
            RADXLGrid.ColumnsToHide.Add(new HiddenColumnInfo { ColumnName = "is_retryable", isDefault = true });
            RADXLGrid.ColumnsToHide.Add(new HiddenColumnInfo { ColumnName = "Username", isDefault = true });

            if (hasColumn == 1)
                RADXLGrid.ColumnsToHide.Add(new HiddenColumnInfo { ColumnName = "additional_info", isDefault = true });
            //if (!IsPostBack)//get key already added error if removed

            if (RADXLGrid.ColumnNameMapping.ContainsKey("chain_name") == false)
            {
                { RADXLGrid.ColumnNameMapping.Add("chain_name", "Task Group Name"); };
                RADXLGrid.ColumnNameMapping.Add("task_name", "Task Name");
                RADXLGrid.ColumnNameMapping.Add("module_name", "Module Name");
                RADXLGrid.ColumnNameMapping.Add("task_type_name", "Task Type");
                RADXLGrid.ColumnNameMapping.Add("start_time", "Start Time");
                RADXLGrid.ColumnNameMapping.Add("end_time", "End Time");
                RADXLGrid.ColumnNameMapping.Add("status", "Status");
                RADXLGrid.ColumnNameMapping.Add("task_status_id", "Task Id");
                // RADXLGrid.ColumnNameMapping.Add("task_type_id", "Task Type Id");
                RADXLGrid.ColumnNameMapping.Add("log_description", "Log Description");
                RADXLGrid.ColumnNameMapping.Add("view_log", "View Log");
                RADXLGrid.ColumnNameMapping.Add("task_master_id", "Task Master ID");
                RADXLGrid.ColumnNameMapping.Add("flow_id", "Flow ID");
                RADXLGrid.ColumnNameMapping.Add("task_history_chart", "View Task History Chart");
                RADXLGrid.ColumnNameMapping.Add("undo", "Undo");
            }
            RADXLGrid.ColumnsToHide.Add(new HiddenColumnInfo { ColumnName = "undo", isDefault = true });
            RADXLGrid.PageSize = 200;
            RADXLGrid.RequirePaging = true;
            RADXLGrid.DateFormat = SessionInfo.CultureInfo.LongDateFormat; //"MM/dd/yy";
            RADXLGrid.RaiseGridRenderComplete = "TSResizeGrid";
            RADXLGrid.Height = "450px";
            RADXLGrid.HideFooter = false;
            RADXLGrid.DoNotExpand = false;
            RADXLGrid.ItemText = "Number of Task";
            RADXLGrid.DoNotRearrangeColumn = true;
            RADXLGrid.RequireGrouping = true;
            RADXLGrid.RequireFilter = true;
            RADXLGrid.RequireSort = true;
            RADXLGrid.RequireMathematicalOperations = false;
            RADXLGrid.RequireSelectedRows = false;
            RADXLGrid.RequireEditGrid = false;
            RADXLGrid.RequireLayouts = false;
            RADXLGrid.RequireExportToExcel = true;
            RADXLGrid.RequireSearch = true;
            RADXLGrid.RequireFreezeColumns = false;
            RADXLGrid.RequireHideColumns = false;
            RADXLGrid.DoNotExpand = false;
            RADXLGrid.RequireColumnSwap = false;
            RADXLGrid.RequireGroupExpandCollapse = true;
            RADXLGrid.ColumnsNotToExport = new List<string> { "task_status_id", "view_log", "task_master_id", "flow_id", "Retry", "Delete", "link_message", "task_history_chart" };
            RADXLGrid.ColumnDateFormatMapping.Clear();
            RADXLGrid.ColumnDateFormatMapping.Add("Start Time", "start_time");
            RADXLGrid.ColumnDateFormatMapping.Add("End Time", "end_time");
            RADXLGrid.DataSource = ds;
            RADXLGrid.CustomRowsDataInfo = GetCustomRowDataInfo(ds.Tables[0], hasColumn);
            RADXLGrid.DataBind();
        }
        void bindRadXlGrid(DataSet ds)
        {
            List<CustomRowDataInfo> lstcustomRowDataInfo = new List<CustomRowDataInfo>();
            int rowCounter = 0;
            string className = "";
            if (hasPriviledge)
                className = "imgBtnDelete";

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                CustomRowDataInfo objCustomRowDataInfo = new CustomRowDataInfo();
                CustomCellDatainfo cellDelete = new CustomCellDatainfo();
                cellDelete.Append = false;
                cellDelete.ColumnName = "Delete";
                cellDelete.NewChild = "<div style=\"text-align:center;\"><img data-task_status_id=" + dr["task_status_id"] + "   class='" + className + "' id=\"imgBtnDelete_" + rowCounter + "\" title=\"Click to Delete\" src=\"" + imagePath + "/deleteIcon.png\" type=\"image\"/></div>";
                StringBuilder btnDeleteOnClick = new StringBuilder();
                objCustomRowDataInfo.Cells.Add(cellDelete);
                objCustomRowDataInfo.Attribute.Add("task_status_id", Convert.ToString(dr["task_status_id"]));
                objCustomRowDataInfo.RowID = Convert.ToString(dr["task_status_id"]);
                lstcustomRowDataInfo.Add(objCustomRowDataInfo);
                rowCounter++;
            }
            RADXLGrid.ClearSerializationData = true;
            RADXLGrid.Visible = true;
            RADXLGrid.UserID = "admin";
            RADXLGrid.CurrentPageID = "CTMTaskStatus";
            RADXLGrid.IdColumnName = "task_status_id";
            RADXLGrid.SessionIdentifier = "abc";
            RADXLGrid.RequireFilter = true;
            // DataSet ds = new DataSet("ROOT");

            // SqlDataAdapter da = new SqlDataAdapter(sql, con);

            //da.Fill(ds);
            //ds.Tables[0].Columns.Add("Retry");
            //ds.Tables[0].Columns.Add("view_log");
            ds.Tables[0].Columns.Add("Delete");
            //ds.Tables[0].Columns.Add("link_message");
            //ds.Tables[0].Columns.Add("task_history_chart");
            //ds.Tables[0].TableName = "Table";
            //RADXLGrid.ColumnsToHide.Add(new HiddenColumnInfo { ColumnName = "task_status_id", isDefault = true });
            //// RADXLGrid.ColumnsToHide.Add(new HiddenColumnInfo { ColumnName = "task_type_id", isDefault = true });
            //RADXLGrid.ColumnsToHide.Add(new HiddenColumnInfo { ColumnName = "log_description", isDefault = true });
            //RADXLGrid.ColumnsToHide.Add(new HiddenColumnInfo { ColumnName = "task_master_id", isDefault = true });
            //RADXLGrid.ColumnsToHide.Add(new HiddenColumnInfo { ColumnName = "flow_id", isDefault = true });
            //RADXLGrid.ColumnsToHide.Add(new HiddenColumnInfo { ColumnName = "link_message", isDefault = true });
            //if (!IsPostBack)
            //{

            //    RADXLGrid.ColumnNameMapping.Add("task_name", "Task Name");
            //    RADXLGrid.ColumnNameMapping.Add("module_name", "Module Name");
            //    RADXLGrid.ColumnNameMapping.Add("task_type_name", "Task Type");
            //    RADXLGrid.ColumnNameMapping.Add("start_time", "Start Time");
            //    RADXLGrid.ColumnNameMapping.Add("end_time", "End Time");
            //    RADXLGrid.ColumnNameMapping.Add("status", "Status");
            //    RADXLGrid.ColumnNameMapping.Add("task_status_id", "Task Id");
            //    // RADXLGrid.ColumnNameMapping.Add("task_type_id", "Task Type Id");
            //    RADXLGrid.ColumnNameMapping.Add("log_description", "Log Description");
            //    RADXLGrid.ColumnNameMapping.Add("view_log", "View Log");
            //    RADXLGrid.ColumnNameMapping.Add("task_master_id", "Task Master ID");
            //    RADXLGrid.ColumnNameMapping.Add("flow_id", "Flow ID");
            //    RADXLGrid.ColumnNameMapping.Add("task_history_chart", "View Task History Chart");

            //}
            RADXLGrid.PageSize = 200;
            RADXLGrid.RequirePaging = true;
            RADXLGrid.DateFormat = "MM-dd-yy";//
            RADXLGrid.RaiseGridRenderComplete = "TSResizeGrid";
            RADXLGrid.Height = "450px";
            RADXLGrid.HideFooter = false;
            RADXLGrid.DoNotExpand = false;
            RADXLGrid.ItemText = "Number of Task";
            RADXLGrid.DoNotRearrangeColumn = true;
            RADXLGrid.RequireGrouping = true;
            RADXLGrid.RequireFilter = true;
            RADXLGrid.RequireSort = true;
            RADXLGrid.RequireMathematicalOperations = false;
            RADXLGrid.RequireSelectedRows = false;
            RADXLGrid.RequireEditGrid = false;
            RADXLGrid.RequireLayouts = false;
            RADXLGrid.RequireExportToExcel = true;
            RADXLGrid.RequireSearch = true;
            RADXLGrid.RequireFreezeColumns = true;
            RADXLGrid.FrozenColumns = new List<FrozenColumnInfo>() { new FrozenColumnInfo { ColumnName = "Module Name", isDefault = true }, new FrozenColumnInfo { ColumnName = "Task Group Name", isDefault = true }, new FrozenColumnInfo { ColumnName = "Task Name", isDefault = true } };
            RADXLGrid.RequireHideColumns = false;
            RADXLGrid.DoNotExpand = false;
            RADXLGrid.RequireColumnSwap = false;
            RADXLGrid.RequireGroupExpandCollapse = true;
            //RADXLGrid.ColumnDateFormatMapping.Add("Start Time", "start_time");
            //RADXLGrid.ColumnDateFormatMapping.Add("End Time", "end_time");
            //RADXLGrid.ColumnsNotToExport = new List<string> { "task_status_id", "log_description", "task_master_id", "flow_id" };
            RADXLGrid.DataSource = ds;

            RADXLGrid.CustomRowsDataInfo = lstcustomRowDataInfo;
            RADXLGrid.DataBind();


        }

        #region not implemented
        Boolean ICTMServiceCallback.UndoTask(string assemblyLocation, string className, int clientTaskStatusId, string clientName)
        {
            throw new NotImplementedException();
        }
        void ICTMServiceCallback.triggerTask(TaskInfo taskInfo, string guid,string clientName)
        {
            throw new NotImplementedException();
        }
        public List<string> getSubscriberList(string a, string b, string clientName)
        {
            throw new NotImplementedException();
        }
        public List<string> getCalendarList(string a, string b, string clientName)
        {
            throw new NotImplementedException();
        }
        public Boolean deleteStatusFromClient(string assemblyLocation, string className, int clientTaskStatusId, string clientName) { throw new NotImplementedException(); }
        public List<int> getUnsyncdTasksClientTaskStatusIds(string assemblyLocation, string className, List<int> list, string clientName) { throw new NotImplementedException(); }
        public string keepAlive(string clientName)
        {
            throw new NotImplementedException();
        }
        List<string> ICTMServiceCallback.getPrivilegeList(string assemblyLocation, string className, string pageId, string username, string clientName)
        {
            throw new NotImplementedException();
        }
        void ICTMServiceCallback.flowsAdded(string assemblyLocation, string className, List<int> clientTaskStatusId, string clientName)
        {
            throw new NotImplementedException();
        }
        void ICTMServiceCallback.flowsDeleted(string assemblyLocation, string className, List<int> clientTaskStatusId, string clientName)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void KillInprogressTask(string assemblyLocation, string className, List<int> ctmStatusId, string clientName)
        {
            throw new NotImplementedException();
        }

        public DataTable SyncStatus(string assemblyLocation, string className, List<int> ctmStatusId, string clientName)
        {
            throw new NotImplementedException();
        }

        public List<int> isSecureToTrigger(string assemblyLocation, string className, int taskMasterId, string clientName)
        {
            throw new NotImplementedException();
        }
    }

}