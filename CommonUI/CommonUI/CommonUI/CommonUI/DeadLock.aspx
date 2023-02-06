<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeadLock.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.DeadLock"
    MasterPageFile="~/CommonUI/SRMMaster.Master" EnableEventValidation="false" %>

<asp:Content ID="contentFilterSection" ContentPlaceHolderID="ContentPlaceHolderFilterSection" runat="server">
    <div style="height: 35px; padding-top: 5px;">
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
    <script src="includes/SMDatePickerControl.js" type="text/javascript"></script>
    <script src="includes/DeadLock.js" type="text/javascript"></script>
    <link rel="stylesheet" type="text/css" href="css/SMCss/SMDatePickerControl.css" />
    <link rel="stylesheet" type="text/css" href="css/SMCss/Customjquery.datetimepicker.css" />
    <link rel="Stylesheet" type="text/css" href="css/RADDateTimePicker.css" />
    <link rel="Stylesheet" type="text/css" href="css/DeadLock.css" />
    <link type="text/css" rel="stylesheet" href="css/thirdparty/bootstrap.min.css" />
    <script src="js/thirdparty/popper.min.js"></script>
    <script type="text/javascript" src="js/thirdparty/bootstrap.min.js"></script>
    <link rel="stylesheet" href="css/thirdparty/font-awesome.min.css" />
    <script type="text/javascript" src="js/thirdparty/bootstrap.min.js"></script>
    <link type="text/css" rel="stylesheet" href="css/thirdparty/bootstrap.min.css" />


    <div id="noDataContainer" style="padding: 10px; border: 5px solid grey; margin-top: 20px; z-index: 1;" class="container-fluid">
        <div id ="noData" class="noDataContainer">No DeadLock Found</div>
        <%--<table id="deadlock" class="display">
            <thead>
                <tr>
                    <th>DeadLockTimeStamp</th>
                    <th>DeadLock Details</th>
                    <th>Download DeadlockFile</th>
                </tr>
            </thead>
        </table>--%>
    </div>
    <div>
        <div id="tabstrip" style="margin-top: 20px;"></div>
    </div>

     <iframe id="downloaddeadlockfile-iframe" src="" style="display:none !important;"></iframe>

   <%-- <div id="myPopUp">
        <div id="graphPopup">--%>
            <%--<button type="button" class="close closeGraph">&times;</button>--%>
            <div>
                <div id="divCanvasContainer">
                    <canvas id="myCanvas" style="z-index: 1" width="1500" height="550"></canvas>
                </div>
                <div id='FirstGraph'></div>
                <div id='SecondGraph'></div>
            </div>

    <div id="downloadIcon" class="downloadDeadlockXml"></div>
</asp:Content>
