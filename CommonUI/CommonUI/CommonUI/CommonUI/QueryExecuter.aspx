<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QueryExecuter.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.QueryExecuter" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="tabLayout">
            <%-- <div class="modal fade" id="addTab" role="dialog">
                <div class="modal-dialog">

                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h4 class="modal-title">Add Tab</h4>
                        </div>
                        <div class="modal-body">
                            <label for='tab_title'>Tab Title</label><br />
                            <input type='text' name='tab_title' id='tab_title'>
                        </div>
                        <div class="modal-footer">
                            <button id="addTabButton" type="button" class="btn btn-default" data-dismiss="modal">Add</button>
                        </div>
                    </div>

                </div>
            </div>

            <button type="button" class="btn btn-info" data-toggle="modal" data-target="#addTab">Add Tab</button>--%>
            <div id="mainTabs" class='tabsContainer'>
            </div>
            <div id="mainContainer" class="tabsContentParent">
            </div>
            <div class="addTabButton">+</div>

            <div class="dialog" id="addTabDialog">
                <div class="dialogContainer">
                    <label for='tab_title'><b>Tab Title</b></label><br />
                    <input type='text' name='tab_title' id='tab_title' /><br />
                    <label id="tab_type_label"><b>Tab Type</b></label>
                    <div class="dialogButtonGroup">
                        <div class="dialogButton" id="addTabCreate">Create</div>
                        <div class="dialogButton" id="addTabCancel">Cancel</div>
                    </div>
                </div>
            </div>
            <script src="js/thirdparty/jquery-1.11.3.min.js"></script>
            <script src="includes/SecMasterScripts.js"></script>
            <link href="css/datatable.css" rel="stylesheet" />
            <script src="includes/datatable.js"></script>
            <script src="includes/QueryExecuter.js"></script>
            <link href="css/QueryExecuter.css" rel="stylesheet" />
    </form>
</body>
</html>
