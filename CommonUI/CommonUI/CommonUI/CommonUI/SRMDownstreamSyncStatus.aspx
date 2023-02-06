<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMDownstreamSyncStatus.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMDownstreamSyncStatus"
    MasterPageFile="~/CommonUI/SRMMaster.Master" EnableEventValidation="false" %>


<asp:Content ID="contentFilterSection" ContentPlaceHolderID="ContentPlaceHolderFilterSection" runat="server">
    <div style="height: 35px; padding-top: 5px;">
        <div id="SMDasboardFilterSection" class="DashboardFilterSection" style="display: inline-block; float: right;">
            <div style="float: right;">
                <i id="SMDownstreamRefreshButton" class="fa fa-refresh btnClassHeader" style="display: inline-block; margin-right: 14px; margin-top: 5px; color: #949494; font-size: 17px;"></i>
            </div>
            <div style="float: right;">
                <i id="SMDownstreamDownloadButton" class="fa fa-file-excel-o btnClassHeader" style="display: inline-block; margin-right: 14px; margin-top: 5px; color: #949494; font-size: 17px;"></i>
            </div>
        </div>
        
        <div class="dashboardFilterUI dateFilterPopUp" id="DateFilterApplyToAll" style="display: inline-block; color: #48a3dd !important; float: right; padding: 6px 10px 0px 0px; font-family: Lato; text-align: right; font-size: 13px; margin-right: 8px; padding-top: 4px; padding-right: 4px;">
            <span class="dateFilterText">Today</span><i id="ArrowDownApplyToAll" class="fa fa-chevron-down"
                style="margin-left: 3px; font-size: 13px;"></i>
        </div>
    </div>
</asp:Content>
<asp:Content ID="contentMiddle" ContentPlaceHolderID="SRMContentPlaceHolderMiddle" runat="server">
    <link href="css/datatable.css" rel="stylesheet" />
    <script src="includes/datatable.js"></script>
    <script type="text/javascript" src="includes/jquery.slimscrollHorizontal.js"></script>
    <script type="text/javascript" src="includes/jquery.datetimepicker.js"></script>
    <script type="text/javascript" src="includes/RADCustomDateTimePicker.js"></script>
    <script type="text/javascript" src="includes/SMDatePickerControl.js"></script>
    <script src="includes/SRMDownstreamSyncStatus.js"></script>
    <link href="css/SRMDownstreamSyncStatus.css" rel="stylesheet" />
    <link href="css/thirdparty/font-awesome.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="css/SMCss/SMDatePickerControl.css" />
    <link rel="stylesheet" type="text/css" href="css/SMCss/Customjquery.datetimepicker.css" />
    <link rel="Stylesheet" type="text/css" href="css/RADDateTimePicker.css" />


    <div id="wrapperDiv">
       <table id="tasks" class="display row-border">
            <thead>
                <tr>
                    <th></th>
                    <th>SETUP NAME</th>
                    <th>START DATE</th>
                    <th>END DATE</th>
                    <th>STATUS</th>
                    <th> </th>

                </tr>
            </thead>
        </table>
    </div>
    <div id="dialogContainer">
        <div id="dialog">
            <div class="message-title">Messages</div>
            <div class="dialog-top">
                <div id="exportMessage"><i class="fa fa-file-text-o"></i></div>
                <div id="closeMessage"><i class="fa fa-close"></i></div>
            </div>
            <div id="report_message"></div>
        </div>
    </div>
    <div id="SetupStatusSearch" style="display:none">
        <i class="fa fa-search"></i>
        <input placeholder="Search..."/>
    </div>

</asp:Content>




