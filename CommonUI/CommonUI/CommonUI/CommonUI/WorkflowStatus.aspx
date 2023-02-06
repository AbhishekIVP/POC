<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkflowStatus.aspx.cs"
    Inherits="com.ivp.common.CommonUI.CommonUI.WorkflowStatus" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="css/jquery-ui.min.css" rel="stylesheet" type="text/css" />
    <link href="css/RMCommonTheme.css" rel="stylesheet" type="text/css" />
    <%--<link href="css/AquaStyles.css" rel="stylesheet" type="text/css" />--%>
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
    <link href="css/thirdparty/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="css/SMAutoComplete.css" rel="stylesheet" type="text/css" />
    <link href="css/WorkflowStatus.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="commonUIWorkflowStatusForm" runat="server">
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

            <%-- Adding popup for posting security to downstream after final approval in workflow --%>

            <asp:Panel ID="panelTop" runat="server">
                <div id="tabContainer" class="tabClass" style="position: absolute; display: inline-block; top: 0px; left: 0px; background-color: white; display: none;">
                    <%--  ul->padding-left: 16px;margin-bottom: 18px;--%>
                    <ul class="nav nav-tabs" style="border-bottom: none; margin-top: 11px; padding-left: 10px; box-shadow: 2px 0px 10px #DADADA; border-radius: 3px;">
                        <li><a id="SecWorkflowTab" class="tabs WorkflowTabHandler" style="padding-right: 0px; padding-left: 0px; margin-right: 40px; color: #666; font-size: 14px; font-family: Lato; cursor: pointer"
                            selectedtab="SecMaster">Securities</a></li>
                        <li><a id="RefWorkflowTab" class="tabs WorkflowTabHandler" style="padding-right: 0px; padding-left: 0px; margin-right: 40px; color: #666; font-size: 14px; font-family: Lato; cursor: pointer"
                            selectedtab="RefMaster">Reference Data</a></li>
                    </ul>
                </div>
                <div class="WorkflowTabsContainer">
                    <div style="float: right; font-size: 17px;">
                        <i id="workflowRefreshButton" class="fa fa-refresh"></i>
                    </div>
                    <div class="WorkflowTab" targetdivid="workflowAllRequests" tabindex="3" style="border-top-left-radius: 3px; border-bottom-left-radius: 3px; display: none;">
                        <div class="WorkflowTabText">
                            All Requests
                        </div>
                        <div class="WorkflowTabCount" id="WorkflowAllRequestsCount">
                            0
                        </div>
                    </div>
                    <div class="WorkflowTab WorkflowTabActive" targetdivid="workflowMyRequests" tabindex="0"
                        style="border-top-left-radius: 3px; border-bottom-left-radius: 3px;">
                        <div class="WorkflowTabText">
                            My Requests
                        </div>
                        <div class="WorkflowTabCount" id="WorkflowMyRequestsCount">
                            0
                        </div>
                    </div>
                    <div class="WorkflowTab" targetdivid="workflowRejectedRequests" tabindex="1" style="border-right: none; border-left: none;">
                        <div class="WorkflowTabText">
                            Rejected Requests
                        </div>
                        <div class="WorkflowTabCount" id="workflowRejectedRequestsCount">
                            0
                        </div>
                    </div>
                    <div class="WorkflowTab" targetdivid="workflowRequestsPending" tabindex="2" style="border-bottom-right-radius: 3px; border-top-right-radius: 3px;">
                        <div class="WorkflowTabText">
                            Requests pending at me
                        </div>
                        <div class="WorkflowTabCount" id="workflowRequestsPendingCount">
                            0
                        </div>
                    </div>
                    <div class="WorkflowActionButtonsDiv">
                        <div class="WorkflowActionButtonsContainer">
                            <span class="WorkflowActionButtonsGrouped WorkflowActionPending WorkflowActionRevoke">
                                <div class="WorkflowActionButtonsSuperDiv WorkflowActionPending">
                                    <input type="button" id="workflowApproveBtn" class="CommonOrangeBtnStyle WorkflowActionPending WorkflowActionButtons"
                                        tooltip="Click to Approve" value="Approve" action="Approve" />
                                </div>
                                <div class="WorkflowActionButtonsSeparator WorkflowActionPending">
                                </div>
                                <div class="WorkflowActionButtonsSuperDiv WorkflowActionPending">
                                    <input type="button" id="workflowRejectBtn" class="CommonOrangeBtnStyle WorkflowActionPending WorkflowActionButtons"
                                        tooltip="Click to Reject" value="Reject" action="Reject" />
                                </div>
                                <div class="WorkflowActionButtonsSeparator WorkflowActionPending WorkflowExcludeRef">
                                </div>
                                <div class="WorkflowActionButtonsSuperDiv WorkflowActionPending WorkflowExcludeRef">
                                    <input type="button" id="workflowSuppressBtn" class="CommonOrangeBtnStyle WorkflowActionPending WorkflowActionButtons"
                                        tooltip="Click to Suppress" value="Suppress" action="Suppress" />
                                </div>
                                <div class="WorkflowActionButtonsSuperDiv WorkflowActionRevoke">
                                    <input type="button" id="workflowRevokeBtn" class="CommonOrangeBtnStyle WorkflowActionRevoke WorkflowActionButtons"
                                        tooltip="Click to Revoke" value="Revoke" action="Revoke" />
                                </div>
                            </span><span class="WorkflowActionButtonsSeparated WorkflowActionPending WorkflowActionRejected">
                                <div class="WorkflowActionButtonsSuperDiv WorkflowActionPending WorkflowActionRejected">
                                    <input type="button" id="workflowDeleteBtn" class="CommonWhiteBtnStyle WorkflowActionPending WorkflowActionRejected WorkflowActionButtons"
                                        tooltip="Click to Delete" value="Delete" action="Delete" />
                                </div>
                            </span>
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="panelMiddle" runat="server">
                <div id="workflowMyRequests" class="workflowGridDiv">
                    <div id="workflowMyRequestsGridContainer" style="overflow: auto;">
                        <div id="workflowMyRequestsScrollContainer" class="workflowRequestsScrollContainer">
                        </div>
                    </div>
                </div>
                <div id="workflowAllRequests" class="workflowGridDiv">
                    <div id="workflowAllRequestsGridContainer" style="overflow: auto;">
                        <div id="workflowAllRequestsScrollContainer" class="workflowRequestsScrollContainer">
                        </div>
                    </div>
                </div>
                <div id="workflowRejectedRequests" class="workflowGridDiv">
                    <div id="workflowRejectedRequestsGrid" class="WorkflowGridHandler">
                    </div>
                </div>
                <div id="workflowRequestsPending" class="workflowGridDiv">
                    <div id="workflowRequestsPendingGrid" class="WorkflowGridHandler">
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="panelBottom" runat="server">
                <div id="divMoreUsers" style="display: none; height: 350px; background: white; z-index: 10001; position: fixed; padding: 6px 0px; border: 1px solid #E7E7E7; border-radius: 3px; box-shadow: 0 3px 12px rgba(0, 0, 0, .5);">
                    <div id="WorkflowMoreUsersDiv" style="position: relative; width: 100px; vertical-align: top; overflow: hidden; max-height: 306px; white-space: nowrap; margin-left: 6px; margin-right: 5px;">
                    </div>
                </div>
                <div id="panelWorkflowActions" style="display: none; width: 370px; height: 127px; z-index: 10000; position: fixed; padding-top: 14px; box-shadow: 0 1px 6px rgba(0, 0, 0, 0.21);"
                    class="panel_bg">
                    <div class="workflowDownArrowDiv">
                        <span class="fa fa-caret-up workflowDownArrow"></span>
                    </div>
                    <img class="close_img" title="Close" id="imgWorkflowActions" style="cursor: pointer; display: none; float: right; margin: 10px 9px 0px 0px; opacity: 0.2; width: 11px; height: 13px;"
                        src="images/closelabel.png" alt="close" />
                    <div id="workflowActionsPopupHeader" class="panelHead" style="display: none;">
                        Approve
                    </div>
                    <div id="workflowActionsPopupDiv" class="container" style="position: relative; width: 100%;">
                        <textarea id="workflowActionsRemarks" placeholder="Enter remarks here.." style="height: 70px; width: 330px; margin-bottom: 12px; outline: none; font-size: 12px; color: #000000; border: 1px solid #CCCCCC; padding: 3px; border-radius: 5px;"></textarea>
                    </div>
                    <div id="workflowActionsErrorDiv" style="padding-left: 16px; color: red;">
                    </div>
                    <div style="float: right; padding-right: 13px;">
                        <input type="button" id="btnWorkflowActionCancel" class="CommonButtonAsText" tooltip="Click to Cancel"
                            value="Cancel" />
                        <input type="button" id="btnWorkflowAction" class="button" tooltip="Click to Resolve"
                            value="Resolve" />
                    </div>
                </div>
                <div id="panelViewLog" style="display: none; width: 900px; z-index: 10000; position: fixed; padding-top: 0px; padding-left: 20px; box-shadow: rgba(0, 0, 0, 0.207843) 0px 1px 6px;"
                    class="panel_bg">
                    <img class="close_img" title="Close" id="imgViewLog" style="cursor: pointer; display: block; float: right; margin: 10px 9px 0px 0px; opacity: 0.2; width: 11px; height: 13px;"
                        src="images/closelabel.png" alt="close" />
                    <div class="panelHead" style="text-transform: uppercase;">
                        Action History
                    </div>
                    <%--<div id="viewLogPopupTop">
                </div>--%>
                    <div id="viewLogPopupDiv" class="container" style="position: relative; width: 100%; vertical-align: top; overflow: auto; max-height: 380px; margin-top: 8px;">
                    </div>
                </div>
                <div id="panelImpactedSecurities" style="display: none; height: 500px; width: 100%; bottom: 0px; left: 0px; box-shadow: 0 0px 21px rgba(0, 0, 0, .5);"
                    class="panel_bg workflowPanelImpactedtDiv">
                    <img class="close_img" title="Close" id="imgImpactedSecurities" style="cursor: pointer; display: block; float: right; margin: 10px 9px 0px 0px; opacity: 0.2; width: 11px; height: 13px;"
                        src="images/closelabel.png" alt="close" />
                    <div id="impactedSecuritiesPopupHeader" class="panelHead">
                        Impacted Attributes
                    </div>
                    <div id="impactedSecuritiesPopupDiv" class="container" style="position: relative; width: 100%; vertical-align: top; overflow: auto;">
                    </div>
                </div>
                <div id="panelLegData" style="display: none; width: 900px; height: 505px; z-index: 10000; position: fixed; padding-top: 0px; padding-left: 20px; box-shadow: rgba(0, 0, 0, 0.207843) 0px 1px 6px;"
                    class="panel_bg">
                    <img class="close_img" title="Close" id="imgLegData" style="cursor: pointer; display: block; float: right; margin: 10px 9px 0px 0px; opacity: 0.2; width: 11px; height: 13px;"
                        src="images/closelabel.png" alt="close" />
                    <div class="panelHead" style="text-transform: uppercase;">
                        Leg Data
                    </div>
                    <div id="legDataPopupDiv" class="container" style="position: relative; width: 100%; vertical-align: top; overflow: auto; margin-top: 8px;">
                    </div>
                </div>
                <div class="PanelsContainerDiv">
                    <asp:Panel ID="panelSave" runat="server" Style="display: none;"
                        DefaultButton="btnSaveOk">
                        <table width="45%" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="alertSuccess">
                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="successHead">Approved Successfully
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="alertMessage">
                                                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td>
                                                            <div id="divChkListExternalSystemSave" style="background-color: White; width: 440px; margin-top: 10px; margin: auto; margin-top: 10px; padding-left: 34px;">
                                                                <div class="SMPostToDiv" id="postToDivPanel" style="font-weight: normal; font-size: 13px; font-family: lato; padding-bottom: 8px;">
                                                                    Post Approved Entities To:
                                                                </div>
                                                                <table id="chkListExternalSystemSave" border="0" style="display: none; width: 100%;">
                                                                </table>
                                                            </div>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <p id="option_container" style="margin-bottom: 0px !important; margin-left: 0px; font-weight: normal; font-size: 13px; margin-right: 0px; padding-left:34px; color: #666;">
                                                                <input id="posting_status_checkbox" type="checkbox" name="posting_status" style="position: relative; top: 3px; margin-right: 5px;" checked="checked" />
                                                                Go to DownStream Status Screen  
                                                                
                                                                <%--<input type="radio" name="posting_status" style="position: relative; top: 3px;"
                                                                    checked="checked" value="Yes" />
                                                                Yes
                                                                <input type="radio" value="No" name="posting_status" style="position: relative; top: 3px;"/>
                                                                No--%>
                                                            </p>
                                                        </td>
                                                        <td>
                                                            <asp:Button ID="btnSaveOk" Text="Post" ToolTip="Click To Post" CssClass="CommonGreenBtnStyle"
                                                                runat="server" Style="float: right; margin-right: 8px;" />
                                                        </td>
                                                        <td class="alertClose">
                                                            <asp:Button ID="btnClosePost" Text="Close" ToolTip="Close" CssClass="RMCancelBtn"
                                                                runat="server" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>



                    <asp:Panel ID="pnlError" runat="server" Style="display: none;" DefaultButton="btnError">
                        <table width="45%" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="alertUnSuccess">
                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="unSuccessHead">Error
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="alertMessage">
                                                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblError" CssClass="alertLabel" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="alertClose">
                                                            <asp:Button ID="btnError" Text="Close" CssClass="RMCancelBtn" runat="server" CausesValidation="false"
                                                                TabIndex="1" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlSuccess" runat="server" Style="display: none;" DefaultButton="btnOK">
                        <table width="45%" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="alertSuccess">
                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="successHead">Success
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="alertMessage">
                                                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblSuccess" CssClass="alertLabel" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="alertClose">
                                                            <asp:Button ID="btnOK" Text="Close" ToolTip="Click Ok to proceed" CssClass="pageButton"
                                                                runat="server" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>

                    <ajaxToolKit:ModalPopupExtender ID="modalpanelSave" runat="server" TargetControlID="hdnPanelSave"
                        BackgroundCssClass="modalBackground" PopupControlID="panelSave" DropShadow="false"
                        BehaviorID="modalpanelSave" CancelControlID="btnSaveOk">
                    </ajaxToolKit:ModalPopupExtender>
                    <ajaxToolKit:ModalPopupExtender ID="modalError" Y="0" runat="server" TargetControlID="hdnError"
                        BackgroundCssClass="modalBackground" PopupControlID="pnlError" DropShadow="false"
                        BehaviorID="modalError" CancelControlID="btnError">
                    </ajaxToolKit:ModalPopupExtender>
                    <ajaxToolKit:ModalPopupExtender ID="modalSuccess" Y="0" runat="server" TargetControlID="hdnSuccess"
                        BackgroundCssClass="modalBackground" PopupControlID="pnlSuccess" DropShadow="false"
                        BehaviorID="modalSuccess" CancelControlID="btnOK">
                    </ajaxToolKit:ModalPopupExtender>
                    <input id="hdnError" type="hidden" runat="server" />
                    <input id="hdnSuccess" type="hidden" runat="server" />
                    <input id="hdnPanelSave" type="hidden" runat="server" />

                </div>
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
        <script src="includes/SecMasterScripts.js" type="text/javascript"></script>
        <script src="includes/script.js" type="text/javascript"></script>
        <script src="includes/grid.js" type="text/javascript"></script>
        <script src="includes/SMSelect.js" type="text/javascript"></script>
        <script src="includes/RADCustomDateTimePicker.js" type="text/javascript"></script>
        <script src="includes/jquery.datetimepicker.js" type="text/javascript"></script>
        <script src="includes/SMSlimscroll.js" type="text/javascript"></script>
        <script src="js/SMAutocomplete.js" type="text/javascript"></script>
        <script src="includes/WorkflowStatusInfo.js" type="text/javascript"></script>
        <script src="includes/WorkflowStatus.js" type="text/javascript"></script>
    </form>
</body>
</html>
