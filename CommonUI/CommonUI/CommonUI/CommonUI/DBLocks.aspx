<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DBLocks.aspx.cs" Inherits="SRMMonitoringTool.DBLocks" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="css/neogrid.css" rel="stylesheet" type="text/css" />
    <link href="css/thirdparty/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="css/SMCss/Customjquery.datetimepicker.css" rel="stylesheet" type="text/css" />
    <link href="css/DBLocks.css" rel="stylesheet" type="text/css" />
    <link href="css/SMCss/SMDatePickerControl.css" rel="stylesheet" type="text/css" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <%--<asp:ScriptManager ID="smMaster" runat="server" ScriptMode="Release" EnableViewState="true"
            AllowCustomErrorsRedirect="false" OnAsyncPostBackError="smMaster_AsyncPostBackError"
            AsyncPostBackTimeout="20000" EnablePageMethods="true" LoadScriptsBeforeUI="true">
            <Services>
                <asp:ServiceReference Path="~/CommonUI/BaseUserControls/Service/RADNeoGridService.svc" />
            </Services>
            <Scripts>
                <asp:ScriptReference Path="includes/RADBrowserScripts.debug.js" />
                <asp:ScriptReference Path="includes/RADDragDropScripts.debug.js" />
                <asp:ScriptReference Path="includes/RScriptUtils.debug.js" />
            </Scripts>
        </asp:ScriptManager>--%>
            <asp:ScriptManager ID="smMaster" runat="server" ScriptMode="Release"
                EnableViewState="true" AllowCustomErrorsRedirect="false" AsyncPostBackTimeout="20000"
                EnablePageMethods="true" LoadScriptsBeforeUI="true" OnAsyncPostBackError="smMaster_AsyncPostBackError">
                <Services>
                    <asp:ServiceReference Path="~/CommonUI/BaseUserControls/Service/RADNeoGridService.svc" />
                </Services>
                <Scripts>
                    <asp:ScriptReference Path="ScriptIncludes/SecMasterJSCommon.debug.js" />
                    <asp:ScriptReference Path="ScriptIncludes/RefMasterJSCommon.debug.js" />
                    <asp:ScriptReference Path="includes/RADBrowserScripts.debug.js" />
                    <asp:ScriptReference Path="includes/RADDragDropScripts.debug.js" />
                    <asp:ScriptReference Path="includes/RScriptUtils.debug.js" />
                </Scripts>
            </asp:ScriptManager>

            <div id="DBLocksHeader" style="margin-top: 15px; margin-bottom: 15px;">
                <div id="DBLocksDatesFilter" class="DBLocksFilter" style="display: inline-block; width: 13%; text-align: right; margin-left: 84%; padding-right: 0.5%; color: #4197ce !important; font-size: 14px; cursor: pointer; font-family: Lato; }">
                    <span id="DBLocksDatesFilterText">Today</span>
                    <i class="fa fa-chevron-down" style="padding-left: 5px; font-size: 10px;"></i>
                </div>
                <div id="DBRefreshButton" style="display: inline-block; width: 2%; text-align: left; color: rgb(210, 210, 210); cursor: pointer; font-size: 14px;">
                    <i class="fa fa-refresh"></i>
                </div>
            </div>

            <div id="DBLocksCurrent" style="display:none;">
                <div id="DBLocksCurrentText"> 
                    <img id="DBLocksLockImage"/>
                    Lock acquired since
                    <span id="DBLocksWhenCurrentLockAcquired" class="DBLocksCurrent"></span>
                    by
                    <span id="DBLocksWhenCurrentUser" class="DBLocksCurrent"></span>
                    <span id="DBLocksWhenCurrentMachineName" class="DBLocksCurrent"></span>
                </div>
                <%--<div id="DBLocksCurrentPopup">
                    <div id="DBLocksFirstAttemptHeader" class="DBLocksInline DBLocksGridHeader">First Attempt</div>
                    <div id="DBLocksFirstAttempt" class="DBLocksInline DBLocksGridText"></div>

                    <div id="DBLocksIdentifierHeader" class="DBLocksInline DBLocksGridHeader">Identifier</div>
                    <div id="DBLocksIdentifier" class="DBLocksInline DBLocksGridText"></div>

                    <div id="DBLocksStackTraceHeader" class="DBLocksInline DBLocksGridHeader">Stack Trace</div>
                    <div id="DBLocksStackTrace" class="DBLocksInline DBLocksGridText"></div>

                    <div id="DBLocksProcessIdHeader" class="DBLocksInline DBLocksGridHeader">Process Id</div>
                    <div id="DBLocksProcessId" class="DBLocksInline DBLocksGridText"></div>

                    <div id="DBLocksProcessNameHeader" class="DBLocksInline DBLocksGridHeader">Process Name</div>
                    <div id="DBLocksProcessName" class="DBLocksInline DBLocksGridText"></div>
                </div>--%>

            </div>


            <div id="DBLocksNoGridSection" style="display: none;">
                <div class="DBLocksNoGridSection"></div>
            </div>
            <div id="DBLocksNeoGrid" style="margin-top: 40px"></div>
        </div>

        <div id="loadingImg" class="loadingMsg" style="display: none; z-index: 10000;">
            <asp:Image ID="Image1" runat="server" src="css/images/ajax-working.gif" />
        </div>
        <div id="disableDiv" class="alertBG" style="display: none; z-index: 9999;" align="center"></div>

        <%--popups--%>
        <div id="os_messagePopup" class="DBLocksMessagePopupStyle" style="display: none;">
                    <div class="fa fa-times draftsPopupCloseBtn">
                    </div>
                    <div class="draftsUpperSection">
                    </div>
                    <div class="DBLocks allowTextCopy">
                    </div>
                    <div class="copyTextParent">
                        <span class="copyText">Click here to copy.</span><span id="copySuccess">Copied Successfully</span>
                    </div>
        </div>

        <script src="js/thirdparty/jquery-1.11.3.min.js"></script>
        <script src="js/thirdparty/jquery-ui.js"></script>
        <script src="js/thirdparty/jquery-ui.min.js"></script>
        <script src="includes/MicrosoftAjax.js"></script>
        <script src="js/thirdparty/bootstrap.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/knockout-3.4.0.js" type="text/javascript"></script>
        <script src="includes/knockout-fast-foreach.min.js" type="text/javascript"></script>
        <script src="includes/jquery.slimscrollHorizontal.js" type="text/javascript"></script>
        <script src="includes/slimScrollHorizontal.js" type="text/javascript"></script>
        <script src="includes/jquery.slimscroll.js" type="text/javascript"></script>
        <script src="includes/ruleEditorScroll.js" type="text/javascript"></script>
        <script src="includes/xlgrid.loader.js" type="text/javascript"></script>
        <script src="includes/neogrid.client.js" type="text/javascript"></script>
        <script src="includes/XMLHttpSyncExecutor.js" type="text/javascript"></script>
        <script src="includes/viewManager.js" type="text/javascript"></script>
        <script src="includes/updatePanel.js" type="text/javascript"></script>
        <script src="includes/ViewPort.js" type="text/javascript"></script>
        <script src="includes/SecMasterScripts.js" type="text/javascript"></script>
        <script src="includes/script.js" type="text/javascript"></script>
        <script src="includes/grid.js" type="text/javascript"></script>
        <script src="includes/SMSelect.js" type="text/javascript"></script>
        <script src="includes/RADCustomDateTimePicker.js" type="text/javascript"></script>
        <script src="includes/jquery.datetimepicker.js" type="text/javascript"></script>
        <script src="includes/SMSlimscroll.js" type="text/javascript"></script>
        <script src="includes/DBLocks.js" type="text/javascript"></script>
        <%--<script src="includes/jquery.datetimepicker.js" type="text/javascript"></script>--%>
        <script src="includes/SMDatePickerControl.js" type="text/javascript"></script>
    </form>
</body>
</html>
