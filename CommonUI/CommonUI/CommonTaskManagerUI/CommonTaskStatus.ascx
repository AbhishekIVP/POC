<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommonTaskStatus.ascx.cs"
    Inherits="com.ivp.secm.ui.CommonTaskStatus" %>
<!--script src="http://code.jquery.com/jquery-1.10.1.min.js" type="text/javascript"></script-->
<%--<link rel="stylesheet" href="http://code.jquery.com/ui/1.10.3/themes/smoothness/jquery-ui.css" />
<link href="css/CommonTaskManagerStyle.css" rel="stylesheet" type="text/css" />--%>

<script type="text/javascript">
  // CSS Styling for RM Grid width
    $(document).ready(() => {
        let gridContainingTable = $('#RMContentBody');
        //check if table exists
        if (gridContainingTable.length > 0) {
            gridContainingTable.css('table-layout', 'fixed');
        }
    });
</script>

<div id="dialog">
    <p id='dialogMsg' style='color: rgb(66, 66, 66);'>
    </p>
</div>
<asp:HiddenField ID="servicePath" runat="server" />
<asp:HiddenField ID="loginName_hf" runat="server" />
<asp:HiddenField ID="clientName_hf" runat="server" />
<asp:HiddenField ID="myViewState_hf" runat="server" />
<asp:HiddenField ID="unsyncdTasksFlag_hf" runat="server" />
<asp:HiddenField ID="isPostBack_hf" runat="server" />
<asp:HiddenField ID="startDate_hf" runat="server" />
<asp:HiddenField ID="endDate_hf" runat="server" />
<asp:HiddenField ID="showNoRecordsDiv_hf" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdnGetTaskTypeWithStatus" runat="server" ClientIDMode="Static" />
<asp:HiddenField ID="hdnCustomRadioOption" runat="server" Value="0" />
<asp:HiddenField ID="hdnTopOption" runat="server" Value="-1" />
<asp:HiddenField ID="hdnStartDateCalender" runat="server" Value="-1" />
<asp:HiddenField ID="hdnEndDateCalender" runat="server" Value="-1" />
<asp:HiddenField ID="hdnHtmlString" runat="server" Value="" ClientIDMode="Static" />
<asp:HiddenField ID="txtEndDateShort" runat="server" Value="" ClientIDMode="Static" />




<div id="lightbox">
</div>
<div id="preLoader">
</div>
<div id="taskGanttChartPopup" style="top: 100px; left: 20%;" class="ctm-popup">
    <div class="popupBody">
        <div class="cross">
        </div>
        <div id="chartTabs">
            <ul>
                <li><a href="#graphTab">Task Duration Chart</a></li>
                <li><a href="#highChartTest">Start Time - End Time Chart</a></li>
            </ul>
            <div id="graphTab">
                <div id="highchart" style="min-width: 900px; height: 500px; margin: 0 auto">
                </div>
            </div>
            <div id="highChartTest">
                <div id="highchart3" style="min-width: 900px; height: 500px; margin: 0 auto">
                </div>
            </div>
        </div>
    </div>
</div>
<asp:Panel ID="panelTop" runat="server" CssClass="panelTopBorderBottom">
    <div style="display: none !important;">
        <asp:RadioButtonList ID="rblstDate" runat="server" RepeatDirection="Horizontal" ToolTip="Select Date Format to Search">
            <asp:ListItem Text="panelBetweenDates" Value="0">Between Dates</asp:ListItem>
            <asp:ListItem Text="panelFromDate" Value="1" Selected="True">From Date</asp:ListItem>
            <%--<asp:ListItem Text="panelPriorDate" Value="2">Prior To Date</asp:ListItem>--%>
        </asp:RadioButtonList>
        <div id="ctm_trigger_as_of_date_container">
            <label for="ctm_trigger_as_of_date">
                As of Date
            </label>
            <input type="text" id="ctm_trigger_as_of_date" />
        </div>
    </div>
    <div style="float: right; margin-top: 5px; display: none !important;">
        <asp:Label ID="lblTaskType" runat="server" Text="Task Type : " Style="display: none;"></asp:Label>
        <asp:DropDownList ID="ddlTaskType" runat="server" CssClass="input" ToolTip="Select a Task Type"
            AutoPostBack="false" EnableViewState="true" Style="display: none !important;">
        </asp:DropDownList>
        <asp:Label ID="lblTaskStatus" EnableViewState="true" runat="server" Text="Task Status : " Style="display: none !important;"></asp:Label>
        <asp:DropDownList ID="ddlTaskStatus" runat="server" CssClass="input" ToolTip="Select a Task Status"
            AutoPostBack="false" EnableViewState="true" Style="display: none !important;">
        </asp:DropDownList>
        <asp:Button ID="btnGetStatus" CssClass="cts-button" runat="server" Text="Get Tasks"
            OnClick="btnGetStatus_Click" Style="display: none;" />&nbsp;
        <input type="button" name="viewGlobalTaskHistoryChart" value="View Task Chart" id="viewGlobalTaskHistoryChart"
            class="cts-button" style="display: none !important;" />
        <asp:Button ID="btnGetUnsyncedTasks" CssClass="cts-button cts-redBtn" runat="server"
            Text="Unsynced Tasks" OnClick="btnGetUnsyncdTasks_Click" Style="display: none !important;" />&nbsp;

    </div>
    <div style="display: none;">
        <asp:Panel ID="panelDates" runat="server">
            <div id="startDateContainer">
                <asp:Label ID="lblStartDate" runat="server" Text="Start Date : "></asp:Label>&nbsp;
                <asp:TextBox ID="txtStartDate" runat="server" EnableViewState="true" CssClass="cts-calendar"
                    ToolTip=""></asp:TextBox><%-- # SessionInfo.CultureInfo.ShortDateFormat EnableViewState="true"--%>
                <asp:Label ID="lblStartDateError" runat="server" Font-Size="Small" ForeColor="Red"
                    Text="<sup>*</sup>" Style="display: none"></asp:Label>
            </div>
            <div id="endDateContainer" style="display: none">
                <asp:Label ID="lblEndDate" runat="server" Text="End Date : "></asp:Label>&nbsp;
                <asp:TextBox ID="txtEndDate" runat="server" EnableViewState="true" CssClass="cts-calendar"
                    ToolTip=""><%--# SessionInfo.CultureInfo.ShortDateFormat  EnableViewState="true"--%>
                </asp:TextBox>
                <asp:Label ID="lblEndDateError" runat="server" Font-Size="Small" ForeColor="Red"
                    Text="<sup>*</sup>" Style="display: none"></asp:Label>
            </div>
        </asp:Panel>
        <%--<asp:Label ID="lblValidationSummary" runat="server" Font-Size="X-Small" ForeColor="Red"></asp:Label>&nbsp;--%>
    </div>
    <div id="SRMTaskStatus_FiltersContainer">
        <div style="float: right; bottom: 5px; position: relative; font-size: 14px;">
            <i id="SMDashboardRefreshButton" class="fa fa-refresh btnClassHeader" style="display: inline-block; margin-right: 14px; margin-top: 4px; color: rgb(210, 210, 210);"></i>
        </div>
        <div id="SMDasboardFilterSection" class="DashboardFilterSection" style="display: inline-block; float: right; bottom: 8px; position: relative;">
            <div style="float: right;">
                <i id="SMDashboardRightFilter" class="fa fa-filter btnClassHeader" style="display: inline-block; margin-right: 14px; margin-top: 5px; font-size: 18px; color: rgb(210, 210, 210);"></i>
            </div>
            <div id="SMDashboardRightFilterContainer">
            </div>
        </div>
        <div class="dashboardFilterUI dateFilterPopUp" id="DateFilterApplyToAll" style="display: inline-block; color: #48a3dd !important; float: right; padding: 6px 10px 0px 0px; font-family: Lato; text-align: right; font-size: 13px; margin-right: 8px; padding-top: 4px; padding-right: 4px; bottom: 8px; position: relative;">
            <span class="dateFilterText">Today</span><i id="ArrowDownApplyToAll" class="fa fa-chevron-down"
                style="margin-left: 3px; font-size: 13px;"></i>
        </div>
    </div>
</asp:Panel>
<div id="taskStatus_Border_display" style="width: 100%; height: 1px; box-shadow: -1px 0px 14px 7px rgba(0, 0, 0, 0.34); display: none;">
</div>
<div id="xlGridContainer" style="margin-top: 0px;">
    <div>
    </div>
    <radxlGrid:RADXLGrid runat="server" ID="RADXLGrid" DateFormat="MM/dd/yyyy hh:mm:ss tt"></radxlGrid:RADXLGrid>
</div>
<div id="taskStatus_EmptyGridContainer">
</div>
