<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RecentExpensiveQuery.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.RecentExpensiveQuery"
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
    <script src="includes/RecentExpensiveQuery.js" type="text/javascript"></script>
    <link rel="stylesheet" type="text/css" href="css/SMCss/SMDatePickerControl.css" />
    <link rel="stylesheet" type="text/css" href="css/SMCss/Customjquery.datetimepicker.css" />
    <link rel="Stylesheet" type="text/css" href="css/RADDateTimePicker.css" />
    <link rel="Stylesheet" type="text/css" href="css/RecentExpensiveQuery.css" />
    <%--<link rel="stylesheet" href="css/thirdparty/font-awesome.min.css" />--%>
    <script type="text/javascript" src="js/thirdparty/bootstrap.min.js"></script>
    <link type="text/css" rel="stylesheet" href="css/thirdparty/bootstrap.min.css" />


    <div style="padding: 10px; border: 5px solid grey; margin-top: 20px;" class="container-fluid">
         <div id ="noQuery" class="noQueryContainer">No Records To Display</div>
        <table id="expensiveQuery" class="display">
            <thead>
                <tr>
                    <th>Query_Text</th>
                    <th>Execution_Count</th>
                    <th>Creation_Time</th>
                    <th>Last_Execution_Time</th>
                    <th>Last_Worker_Time(second)</th>
                    <th>Min_Worker_Time(second)</th>
                    <th>Max_Worker_Time(second)</th>
                    <th>Last_Physical_Reads</th>
                    <th>Min_Physical_Reads</th>
                    <th>Max_Physical_Reads</th>
                    <th>Last_Logical_Writes</th>
                    <th>Min_Logical_Writes</th>
                    <th>Max_Logical_Writes</th>
                    <th>Last_Logical_Reads</th>
                    <th>Min_Logical_Reads</th>
                    <th>Max_Logical_Reads</th>
                    <th>Last_Elapsed_Time(second)</th>
                    <th>Min_Elapsed_Time(second)</th>
                    <th>Max_Elapsed_Time(second)</th>
                    <th>Query_Plan</th>
                </tr>
            </thead>
        </table>
    </div>
    <iframe id="expensivequery-iframe" src="" style="display:none !important;"></iframe>
   

<div class="container">
  <!-- Modal -->
  <div class="modal custom fade" id="myModal" role="dialog">
    <div class="modal-dialog">
      <!-- Modal content-->
      <div class="modal-content">
        <%--<div class="modal-header">--%>
          <button type="button" class="close" data-dismiss="modal">&times;</button>
          <%--<h4 class="modal-title">QueryText</h4>--%>
        <%--</div>--%>
        <div class="modal-body">
         <div id="divXmlContainer"></div>
        </div>
      </div>
    </div>
  </div>
</div>

</asp:Content>

