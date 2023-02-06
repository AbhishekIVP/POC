<%@ Page Title="" Language="C#" MasterPageFile="~/CommonUI/SRMMaster.Master" AutoEventWireup="true" CodeBehind="SRMDashboard.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMDashboard" %>

<asp:Content ID="ContentFilterSection" ContentPlaceHolderID="ContentPlaceHolderFilterSection" runat="server">
    <div class="SRMDashboardFilterContainer">
        <div id="SMDashboardFilterSection" class="DashboardFilterSection">
            <div style="float: right;">
                <i id="SMDashboardRefreshButton" class="fa fa-refresh btnClassHeader" style="display: inline-block; margin-right: 14px; margin-top: 4px; color: rgb(210, 210, 210);"></i>
            </div>
            <div style="float: right;">
                <i id="SMDashboardRightFilter" class="fa fa-filter btnClassHeader" style="display: inline-block; margin-right: 14px; margin-top: 4px; color: rgb(210, 210, 210);"></i>
            </div>
            <div class="dashboardFilterUI dateFilterPopUp" id="SecDateFilter" style="display: inline-block; float: right; font-size: 13px; margin-right: 8px; padding-top: 4px; padding-right: 4px;">
                <span class="dateFilterText">Today</span><i id="ArrowDownApplyToAll" class="fa fa-chevron-down"
                    style="margin-left: 3px; font-size: 9px;"></i>
            </div>
            <div style="float: right;">
                <i id="SMDashboardSaveButton" class="fa fa-floppy-o btnClassHeader" style="display: none; margin-right: 14px; margin-top: 4px;"></i>
            </div>
            <div id="SMDashboardRightFilterContainer">
            </div>
        </div>
        <div id="CADashboardFilterSection" class="DashboardFilterSection">
            <div id="caDashboardFilterIconDiv" class="caDashboardFilterIconDivStyle caDashboardHorizontalAlign">
                <i id="CADashboardRightFilter" class="fa fa-filter"></i>
            </div>
        </div>
        <div id="RefDashboardFilterSection" class="DashboardFilterSection">
            <div style="float: right;">
                <i id="RefDashboardRefreshButton" class="fa fa-refresh btnClassHeader" style="display: inline-block; margin-right: 14px; margin-top: 4px; color: rgb(210, 210, 210);"></i>
            </div>
            <div style="float: right;">
                <i id="RefDashboardRightFilter" class="fa fa-filter btnClassHeader" style="display: inline-block; margin-right: 14px; margin-top: 4px; color: rgb(210, 210, 210);"></i>
            </div>
            <div class="dashboardFilterUI dateFilterPopUp" id="RefDateFilter" style="display: inline-block; float: right; font-size: 13px; margin-right: 8px; padding-top: 4px; padding-right: 4px;">
                <span class="dateFilterText">Today</span><i id="RefArrowDownApplyToAll" class="fa fa-chevron-down"
                    style="margin-left: 3px; font-size: 9px;"></i>
            </div>
            <div style="float: right;">
                <i id="RefDashboardSaveButton" class="fa fa-floppy-o btnClassHeader" style="display: none; margin-right: 14px; margin-top: 4px;"></i>
            </div>
            <div id="RefDashboardRightFilterContainer">
            </div>
        </div>
        <div id="PMDashboardFilterSection" class="DashboardFilterSection">
            <div style="float: right;">
                <i id="PMDashboardRefreshButton" class="fa fa-refresh btnClassHeader" style="display: inline-block; margin-right: 14px; margin-top: 4px; color: rgb(210, 210, 210);"></i>
            </div>
            <div style="float: right;">
                <i id="PMDashboardRightFilter" class="fa fa-filter btnClassHeader" style="display: inline-block; margin-right: 14px; margin-top: 4px; color: rgb(210, 210, 210);"></i>
            </div>
            <div class="dashboardFilterUI dateFilterPopUp" id="PMDateFilter" style="display: inline-block; float: right; font-size: 13px; margin-right: 8px; padding-top: 4px; padding-right: 4px;">
                <span class="dateFilterText">Today</span><i id="PMArrowDownApplyToAll" class="fa fa-chevron-down"
                    style="margin-left: 3px; font-size: 9px;"></i>
            </div>
            <div style="float: right;">
                <i id="PMDashboardSaveButton" class="fa fa-floppy-o btnClassHeader" style="display: none; margin-right: 14px; margin-top: 4px;"></i>
            </div>
            <div id="PMDashboardRightFilterContainer">
            </div>
        </div>
        <div id="FMDashboardFilterSection" class="DashboardFilterSection">
            <div style="float: right;">
                <i id="FMDashboardRefreshButton" class="fa fa-refresh btnClassHeader" style="display: inline-block; margin-right: 14px; margin-top: 4px; color: rgb(210, 210, 210);"></i>
            </div>
            <div style="float: right;">
                <i id="FMDashboardRightFilter" class="fa fa-filter btnClassHeader" style="display: inline-block; margin-right: 14px; margin-top: 4px; color: rgb(210, 210, 210);"></i>
            </div>
            <div class="dashboardFilterUI dateFilterPopUp" id="FMDateFilter" style="display: inline-block; float: right; font-size: 13px; margin-right: 8px; padding-top: 4px; padding-right: 4px;">
                <span class="dateFilterText">Today</span><i id="FMArrowDownApplyToAll" class="fa fa-chevron-down"
                    style="margin-left: 3px; font-size: 9px;"></i>
            </div>
            <div style="float: right;">
                <i id="FMDashboardSaveButton" class="fa fa-floppy-o btnClassHeader" style="display: none; margin-right: 14px; margin-top: 4px;"></i>
            </div>
            <div id="FMDashboardRightFilterContainer">
            </div>
        </div>
        <div id="ManagementAnalyticsDashboardFilterSection" class="DashboardFilterSection"
            style="display: inline-block; float: right;">
            <div style="float: right;">
                <i id="ManagementAnalyticsDashboardRightFilter" class="fa fa-filter btnClassHeader"
                    style="display: inline-block; margin-right: 14px; margin-top: 4px; color: rgb(210, 210, 210);"></i>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="ContentMiddle" ContentPlaceHolderID="SRMContentPlaceHolderMiddle" runat="server">
    <div id="divDashboardMain" class="cssDivSpecific">
        <div id="SMDashBoardDivContainer" class="DashboardDivContainer">
            <div id="SMDashBoardDiv">
                <iframe id="iframeSecMDashboard" frameborder="0" width="100%"></iframe>
            </div>
        </div>
        <div id="CADashBoardDivContainer" class="DashboardDivContainer">
            <div id="CADashBoardDiv">
                <iframe id="iframeCorpDashboard" frameborder="0" width="100%"></iframe>
            </div>
        </div>
        <div id="RefDashBoardDivContainer" class="DashboardDivContainer">
            <div id="RefDashBoardDiv">
                <iframe id="iframeRefMDashboard" frameborder="0" width="100%"></iframe>
            </div>
        </div>
        <div id="PMDashBoardDivContainer" class="DashboardDivContainer">
            <div id="PMDashBoardDiv">
                <iframe id="iframePMDashboard" frameborder="0" width="100%"></iframe>
            </div>
        </div>
        <div id="FMDashBoardDivContainer" class="DashboardDivContainer">
            <div id="FMDashBoardDiv">
                <iframe id="iframeFMDashboard" frameborder="0" width="100%"></iframe>
            </div>
        </div>
        <div id="ManagementAnalyticsDivContainer" class="DashboardDivContainer">
            <div id="ManagementAnalyticsDiv">
                <iframe id="iframeManagementAnalyticsDashboard" frameborder="0" width="100%"></iframe>
            </div>
        </div>
    </div>
    <script type="text/javascript" src="includes/jquery.slimscrollHorizontal.js"></script>
    <script type="text/javascript" src="includes/jquery.datetimepicker.js"></script>
    <script type="text/javascript" src="includes/RADCustomDateTimePicker.js"></script>
    <script type="text/javascript" src="includes/SMDatePickerControl.js"></script>
    <script type="text/javascript" src="includes/SRMDashboardInfo.js"></script>
    <script type="text/javascript" src="includes/SRMDashboard.js"></script>
    <link rel="stylesheet" type="text/css" href="css/SMCss/SMDatePickerControl.css" />
    <link rel="stylesheet" type="text/css" href="css/SMCss/Customjquery.datetimepicker.css" />
    <link rel="Stylesheet" type="text/css" href="css/RADDateTimePicker.css" />
    <link rel="stylesheet" type="text/css" href="css/SRMDashboard.css" />
</asp:Content>
<asp:Content ID="ContentBottom" ContentPlaceHolderID="SRMContentPlaceHolderBottom" runat="server">
</asp:Content>
