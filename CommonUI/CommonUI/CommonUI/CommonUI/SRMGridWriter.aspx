<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMGridWriter.aspx.cs"
    Inherits="com.ivp.common.CommonUI.CommonUI.SRMGridWriter" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="css/jquery-ui.min.css" rel="stylesheet" type="text/css" />
    <link href="css/RMCommonTheme.css" rel="stylesheet" type="text/css" />
    <link href="css/AquaStyles.css" rel="stylesheet" type="text/css" />
    <link href="css/SMCommonTheme.css" rel="stylesheet" type="text/css" />
    <link href="css/Z_IagoCommonThemes.css" rel="stylesheet" type="text/css" />
    <%--    <link href="css/grid.css" rel="stylesheet" type="text/css" />
    <link href="css/xlneogrid.css" rel="stylesheet" type="text/css" />
    <link href="css/neogrid.css" rel="stylesheet" type="text/css" />--%>
    <link href="css/Customjquery.datetimepicker.css" rel="stylesheet" type="text/css" />
    <link href="css/jquery.datetimepicker.css" rel="stylesheet" type="text/css" />
    <link href="css/RADDateTimePicker.css" rel="stylesheet" type="text/css" />
    <link href="css/SMCss/SMSelect.css" rel="stylesheet" type="text/css" />
    <link href="css/SMSlideMenu.css" rel="stylesheet" type="text/css" />
    <link href="css/thirdparty/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="css/font-awesome5_0.css" rel="stylesheet" type="text/css" />
    <link href="css/SMAutoComplete.css" rel="stylesheet" type="text/css" />
    <link href="css/toolKitButtonBar.css" rel="stylesheet" />
    <link href="css/taggingPluginCss.css" rel="stylesheet" />
    <link href="css/toolKitButtonBarNew.css" rel="stylesheet" />
    <link href="css/SRMGridWriter.css" rel="stylesheet" type="text/css" />
    <link href="css/iago.widgets.checkbox.css" rel="stylesheet" type="text/css" />
    <link href="css/iago.widgets.rightFilter.css" rel="stylesheet" type="text/css" />
    <link href="css/iago.widgets.inlineFilter.css" rel="stylesheet" />
    <link href="css/bootstrap-switch.css" rel="stylesheet" />
    <link href="css/jquery.contextMenu.css" rel="stylesheet" />
    <link href="css/iago.widget.contextMenu.css" rel="stylesheet" />
    <link href="css/thirdparty/jquery.qtip.css" rel="stylesheet" />
    <link href="css/thirdparty/dateControl.css" rel="stylesheet" />
</head>
<body>
    <form id="commonUISRMGridWriterForm" runat="server">
        <asp:ScriptManager ID="smMaster" runat="server" ScriptMode="Release" EnableViewState="true"
            AllowCustomErrorsRedirect="false" OnAsyncPostBackError="smMaster_AsyncPostBackError"
            AsyncPostBackTimeout="20000" EnablePageMethods="true" LoadScriptsBeforeUI="true">
            <Services>
                <asp:ServiceReference Path="~/CommonUI/BaseUserControls/Service/RADNeoGridService.svc" />
            </Services>
            <Scripts>
                <asp:ScriptReference Path="ScriptIncludes/SecMasterJSCommon.debug.js" />
                <asp:ScriptReference Path="includes/RADBrowserScripts.debug.js" />
                <asp:ScriptReference Path="includes/RADDragDropScripts.debug.js" />
                <asp:ScriptReference Path="includes/RScriptUtils.debug.js" />
            </Scripts>
        </asp:ScriptManager>
        <div>
            <asp:Panel ID="panelTop" runat="server">
                <div class="SRMGridWriterTopBarContainer iago-page-title">
                    <div class="SRMGridWriterTabContainer">
                        <div id="pageHeaderTabPanel" class="pageHeaderTabPanel pageHeaderTabPanelWhiteTheme"></div>
                    </div>
                    <div class="SRMGridWriterRightTabContainer">
                        <div id="pageHeaderCustomDiv"></div>
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="panelMiddle" runat="server">
                <div class="SRMGridWriterContainer">
                    <div id="gridWriterContainer"></div>
                </div>
            </asp:Panel>
            <asp:Panel ID="panelBottom" runat="server">
            </asp:Panel>
            <div id="loadingImg" class="loadingMsg" style="display: none; z-index: 11000;">
                <asp:Image ID="Image1" runat="server" SkinID="imgLoading" />
            </div>
            <div id="disableDiv" class="alertBG" style="display: none; z-index: 10999;" align="center">
            </div>
            <input type="hidden" id="identifier" name="identifier" />
        </div>
        <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/jquery-ui.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/bootstrap.min.js" type="text/javascript"></script>
        <script src="includes/bootstrap2-toggle.js" type="text/javascript"></script>
        <script src="js/thirdparty/knockout-3.4.0.js" type="text/javascript"></script>
        <script src="includes/knockout-fast-foreach.min.js" type="text/javascript"></script>
        <script src="includes/jquery.slimscrollHorizontal.js" type="text/javascript"></script>
        <script src="includes/slimScrollHorizontal.js" type="text/javascript"></script>
        <script src="includes/jquery.slimscroll.js" type="text/javascript"></script>
        <script src="includes/ruleEditorScroll.js" type="text/javascript"></script>
        <script type="text/javascript" src="includes/xlgrid.loader.js"></script>
        <script type="text/javascript" src="includes/neogrid.client.js"></script>
        <script src="includes/XMLHttpSyncExecutor.js" type="text/javascript"></script>
        <script src="includes/viewManager.js" type="text/javascript"></script>
        <script src="includes/updatePanel.js" type="text/javascript"></script>
        <script src="includes/ViewPort.js" type="text/javascript"></script>
        <script src="includes/React Grid/underscore.js"></script>
        <script src="includes/React Grid/moment.js"></script>
        <script src="includes/React Grid/adaptablegridbundle.js"></script>
        <script src="includes/toolKit.ClientNew.js"></script>
        <script src="includes/taggingPlugin.js"></script>
        <script src="js/thirdparty/moment.js"></script>
        <script src="js/thirdparty/iago.widgets.dateControl.js"></script>
        <script src="js/thirdparty/daterangepicker.js"></script>
        <script src="includes/RRuleEditorJsPlumb.js" type="text/javascript"></script>
        <script src="includes/ruleEngineJqueryPlugin.js" type="text/javascript"></script>
        <script src="includes/ruleEngineModularJqueryPlugin.js" type="text/javascript"></script>
        <script src="includes/RangyInputs.js" type="text/javascript"></script>
        <script src="includes/textareaUtil.js" type="text/javascript"></script>
        <script src="includes/bootbox.js"></script>
        <script src="includes/jquery.qtip.min.js"></script>
        <script src="includes/select2.js"></script>
        <script src="includes/jquery.dataTables.js"></script>
        <script src="includes/neogrid.loader.js"></script>
        <script src="includes/iago.widgets.rightFilter.js"></script>
        <script src="includes/bootstrap-switch.js"></script>
        <script src="includes/rad.TagManager.models.script.js"></script>
        <script src="includes/iago.widgets.checkbox.js"></script>
        <script src="includes/jquery.contextMenu.js"></script>
        <script src="includes/iago.widgets.contextMenu.js"></script>
        <script src="includes/iago.widgets.inlineFilter.js"></script>

        <script src="includes/SecMasterScripts.js" type="text/javascript"></script>
        <script src="includes/script.js" type="text/javascript"></script>
        <script src="includes/grid.js" type="text/javascript"></script>
        <script src="includes/SMSelect.js" type="text/javascript"></script>
        <script src="includes/RADCustomDateTimePicker.js" type="text/javascript"></script>
        <script src="includes/jquery.datetimepicker.js" type="text/javascript"></script>
        <script src="includes/SMSlimscroll.js" type="text/javascript"></script>
        <script src="js/SMAutocomplete.js" type="text/javascript"></script>
        <script src="includes/SRMGridWriterInfo.js" type="text/javascript"></script>
        <script src="includes/SRMGridWriter.js" type="text/javascript"></script>
    </form>
</body>
</html>

