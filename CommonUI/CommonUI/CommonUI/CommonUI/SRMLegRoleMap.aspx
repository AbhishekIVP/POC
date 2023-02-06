<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMLegRoleMap.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMLegRoleMap" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
    <script src="js/thirdparty/jquery-ui.min.js" type="text/javascript"></script>
    <script src="includes/bootstrap2-toggle.js" type="text/javascript"></script>
    <script src="js/thirdparty/bootstrap.min.js" type="text/javascript"></script>
    <script src="includes/MicrosoftAjax.js" type="text/javascript"></script>
    <script src="js/thirdparty/knockout-3.4.0.js" type="text/javascript"></script>
    <script src="includes/knockout-fast-foreach.min.js" type="text/javascript"></script>
    <script src="includes/MicrosoftAjax.js" type="text/javascript"></script>
    <script src="includes/XMLHttpSyncExecutor.js" type="text/javascript"></script>
    <script src="includes/viewManager.js" type="text/javascript"></script>
    <script src="includes/updatePanel.js" type="text/javascript"></script>
    <script src="includes/ViewPort.js" type="text/javascript"></script>
    <script src="includes/neogrid.client.js" type="text/javascript"></script>
    <script src="includes/xlgrid.loader.js" type="text/javascript"></script>
    <script src="includes/script.js" type="text/javascript"></script>
    <script src="includes/SecMasterScripts.js" type="text/javascript"></script>
    <script src="includes/script.js" type="text/javascript"></script>
    <script src="includes/grid.js" type="text/javascript"></script>
    <script src="includes/jquery.slimscrollHorizontal.js" type="text/javascript"></script>
    <script src="includes/slimScrollHorizontal.js" type="text/javascript"></script>
    <script src="includes/jquery.slimscroll.js" type="text/javascript"></script>
    <script src="includes/ruleEditorScroll.js" type="text/javascript"></script>
    <script src="includes/SMSelect.js" type="text/javascript"></script>
    <script src="includes/RADCustomDateTimePicker.js" type="text/javascript"></script>
    <script src="includes/jquery.datetimepicker.js" type="text/javascript"></script>
    <script src="includes/RefMasterUpgradeScript.js" type="text/javascript"></script>
    <script src="includes/SMSlimscroll.js" type="text/javascript"></script>
    <script src="js/CommonModuleTabs.js" type="text/javascript"></script>
    <link href="css/SRMLegRoleParent.css" rel="stylesheet" />
    <script src="includes/SRmLegRoleMap.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div id="SRMLegRoleParent">
                <%--<div id="SRMLegRoleHeadersParent">HEADERS</div>--%>
                <div id="SRMLegRoleLeft" class="SRMLegRoles SRMLegRolesLeft SRMLegRoleHeight SRMLegRolesWidth SRMLegRoles">
                    <div id="SRMLegRoleLeftHeader" class="SRMLegRoleLeftItemsHeight">
                        <div class="SRMLegRoleLeftHeaderText">LEGS</div>
                    </div>
                    <div id="SRMLegRoleLeftContent" data-bind="foreach: legsList">
                        <div class="SRMLegRoleLeftItems SRMLegRoleLeftItemsHeight">
                            <div class="SRMLegRoleLeftItemsText" data-bind="text: $data"></div>
                        </div>
                    </div>
                </div>
                <div class="SRMArrowParent">
                    <div class="SRMLegArrow SRMLegRoleHeight">
                        <i class="fa fa-angle-left SRMLegarrow"></i>
                    </div>
                </div>
                <div id="SRMLegRoleCenter" class="SRMLegRolesWidth SRMLegRolesCenter SRMLegRoleHeight SRMLegRoles">
                    <div class="SRMLegRoleCenterItems" id="SRMLegRoleCenterItems">
                        <div class="SRMLegRoleItemsCenterText" data-bind="text: partyName"></div>
                    </div>
                </div>
                <div class="SRMArrowParent">
                    <div class="SRMLegArrow SRMLegRoleHeight" style="float: right;">
                        <i class="fa fa-angle-right SRMLegarrow"></i>
                    </div>
                </div>
                <div id="SRMLegRoleRight" class="SRMLegRoles SRMLegRolesRight SRMLegRoleHeight">
                    <div id="SRMLegRoleRightHeader" class="SRMLegRoleRightItemsHeight">
                        <div class="SRMLegRoleRightHeaderText">ROLES</div>
                    </div>
                    <div class="SRMLegRoleRightItemsParent" id="SRMLegRoleRightItemsParent" data-bind="foreach: rolesList">
                        <div class="SRMLegRoleRightItems SRMLegRoleRightItemsHeight">
                            <div class="SRMLegRoleRightItemsText" data-bind="text: $data, attr: {'title': $data }"></div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </form>
</body>
</html>
