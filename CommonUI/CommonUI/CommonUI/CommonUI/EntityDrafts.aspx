<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EntityDrafts.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.EntityDrafts" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
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
        <div runat="server" id="topPanel">
            <table width="100%" cellpadding="0" cellspacing="0" border="0">
                <tr>
                    <td>
                        <table width="100%" border="0" class="tableBrdr" cellspacing="0" cellpadding="0">
                            <tr>
                                <%-- Modified By Neetika  
                               <td class="headPanel">
                                    Security Drafts
                                </td>--%>
                                <td class="headPanel" style="width: 10%;" align="right">
                                    <%--<asp:ImageButton ID="imgBtnHlp1" runat="server" SkinID="hlpButton" ImageAlign="Right"
                                    Style="vertical-align: middle; padding-left: 5px;" />--%>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
        <div runat="server" id="middlePanel">
            <div id="Div_NoResult" runat="server">
                No Entity Draft Exists</div>
            <div id="Div_Grid" runat="server" style="border: 1px solid silver; margin: auto;
                width: 99%; -webkit-border-radius: 2px; -moz-border-radius: 2px; border-radius: 2px;">
                <radxlGrid:RADXLGrid runat="server" ID="xlGrid" />
            </div>
        </div>
        <input type="hidden" id="lblUniqueSessionIdentifier" runat="server" />
        <input type="hidden" runat="server" id="hiddenSecID" />
        <asp:Button runat="server" ID="btn_Hidden" OnClick="ctmSecurityDrafts_Handler" Style='display: none;' />
        <input type="hidden" runat="server" id="hiddenSuccess" />
        <input type="hidden" runat="server" id="hiddenError" />
        <asp:Panel ID="panelSave" Width="30%" runat="server" Style='display: none;'>
            <table width="100%" border="0" cellspacing="0" cellpadding="0">
                <tr>
                    <td class="alertSuccess">
                        <table width="100%" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="successHead">
                                    Success!
                                </td>
                            </tr>
                            <tr>
                                <td class="alertMessage">
                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblDeleteSuccess" CssClass="alertLabel" runat="server" Text=" Draft Discarded Successfully !"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="alertClose">
                                                <asp:Button ID="btnOK" Text="  OK  " ToolTip="Click Ok to proceed" CssClass="pageButton"
                                                    runat="server" Style="left: 91%;" />
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
        <%--     <asp:Panel ID="panelError" Width="30%" runat="server" Style='display: none; z-index:70000;'>
            <table width="100%" border="0" cellspacing="0" cellpadding="0">
                <tr>
                    <td class="alertSuccess">
                        <table width="100%" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="successHead">
                                    Warning!
                                </td>
                            </tr>
                            <tr>
                                <td class="alertMessage">
                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td>
                                                <asp:Label ID="lblDeleteError" CssClass="alertLabel" runat="server" Text=" Failed to Discard Draft(s)!"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="alertClose">
                                                <asp:Button ID="btnErrorOK" Text="  OK  " ToolTip="Click Ok to proceed" CssClass="pageButton"
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
        </asp:Panel>--%>
        <asp:Panel ID="panelError" runat="server" Style='display: none;' Width="30%" CssClass="errorPopupDiv">
            <div class="popupAlertUpperDiv">
            </div>
            <div class="errorDivWrapper">
                <div class="errorDivLeft">
                    <img src="../../App_Themes/Aqua/images/icons/alert_icon.png" />
                    <%--<i class="fa fa-exclamation-circle"></i>--%>
                </div>
                <div class="errorDivRight">
                    <div class="errorDivHeading">
                        <p>
                            Warning!</p>
                    </div>
                    <div class="errorDivText">
                        <asp:Label ID="lblDeleteError" CssClass="alertLabel" runat="server" Text=" Failed to Discard Draft(s)!"></asp:Label>
                    </div>
                </div>
                <div class="alertClose">
                    <asp:Button ID="btnErrorOK" Text="  OK  " ToolTip="Click Ok to proceed" CssClass="pageButton"
                        runat="server" Style="left: 91%;" />
                </div>
            </div>
        </asp:Panel>
        <%--        <div id="panelWarning" cssclass="errorPopupDiv" style="top: 0; left: 40%; position: fixed;
            width: 30%; display: none; background: white; z-index: 70000;box-shadow:0px 2px 5px rgba(0,0,0,0.5);">
            <div class="popupAlertUpperDiv">
            </div>
            <div class="errorDivWrapper">
                <div class="errorDivLeft">
                    <img src="../../App_Themes/Aqua/images/icons/alert_icon.png" />
                </div>
                <div class="errorDivRight">
                    <img id="img_Warning" src="~/App_Themes/Aqua/images/Exception/error-orange.png" runat="server"
                        style="margin-left: 5px; float: left; display: none;" onclick="javascript:return false;" />
                    <div class="errorDivHeading" id="Div_WarningText">
                        Warning!
                    </div>
                    <div class="errorDivText">
                        <div id="Div_Content">
                            <div id="LabelDeleteWarning">
                            </div>
                        </div>
                    </div>
                </div>
                <div class="alertCloseNew" id="Div_Button">
                    <input type="button" id="btn_WarningOK" value="Yes" style="width: 50px; margin-top: 8px;
                        margin: auto;border:0px;" />
                    <input type="button" id="btn_WarningCancel" value="No" style="width: 50px; margin-top: 8px;
                        margin: auto;border:0px;" />
                </div>
            </div>
        </div>--%>
        <div id="panelWarning" cssclass="errorPopupDiv" style="top: 0; left: 40%; position: fixed;
            width: 30%; display: none; background: white; z-index: 70000; box-shadow: 0px 2px 5px rgba(0,0,0,0.5);">
            <div class="alertUnsuccessConfirm" style="box-shadow: none;">
            </div>
            <div class="errorDivWrapper">
                <div class="errorDivLeft" style="display: none;">
                    <img src="../../App_Themes/Aqua/images/icons/alert_icon.png" />
                </div>
                <div class="errorDivRight">
                    <img id="img_Warning" src="~/App_Themes/Aqua/images/Exception/error-orange.png" runat="server"
                        style="margin-left: 5px; float: left; display: none;" onclick="javascript:return false;" />
                    <div class="alertUnsuccessConfirmHead" id="Div_WarningText">
                        Warning!
                    </div>
                    <div class="alertUnsuccessConfirmLabel">
                        <div id="Div_Content">
                            <div id="LabelDeleteWarning">
                            </div>
                        </div>
                    </div>
                </div>
                <div class="alertUnsuccessConfirmClose" id="Div_Button">
                    <input type="button" id="btn_WarningOK" class="CommonDeleteButtonAsText" value="Yes"
                        style="width: 50px; margin-top: 8px; margin: auto; border: 0px;" />
                    <input type="button" id="btn_WarningCancel" class="CommonContinueButton" value="No"
                        style="width: 50px; margin-top: 8px; margin: auto; border: 0px;" />
                </div>
            </div>
        </div>
        <%--<asp:Panel ID="panelWarning" runat="server" Style='display: none;' Width="30%" CssClass="errorPopupDiv">
                <div class="popupAlertUpperDiv"></div>
                <div class="errorDivWrapper">
                    <div class="errorDivLeft">
                        <img src="../../App_Themes/Aqua/images/icons/alert_icon.png" />
                        <%--<i class="fa fa-exclamation-circle"></i>
                    </div>
                    <div class="errorDivRight">
                       <img id="img_Warning" src="~/App_Themes/Aqua/images/Exception/error-orange.png" runat="server" style="margin-left: 5px;
                    float: right;" onclick="javascript:return false;" />
                        <div class="errorDivHeading">
                            <p>  Delete!</p>
                        </div>
                        <div class="errorDivText">
                            <asp:Label ID="LabelDeleteWarning" runat="server" ></asp:Label>
                        </div>
                    </div>
                    <div class="alertCloseNew">
                      
                    <input type="button" id="btn_WarningOK" value=" Yes " style="width: 50px; margin-top: 8px;
                        margin: auto;" />
                       
                </div>
                         </div>
              
                </asp:Panel>

        --%>
        <div id="Div_Overlay" style="display: none; height: 100%; width: 100%; position: fixed;
            top: 0px; left: 0px; z-index: 60000;">
        </div>
        <ajaxToolKit:ModalPopupExtender ID="modalSave" runat="server" TargetControlID="hiddenSuccess"
            BackgroundCssClass="modalBackground" PopupControlID="panelSave" OkControlID="btnOK"
            BehaviorID="modalSave" Y="0">
        </ajaxToolKit:ModalPopupExtender>
        <ajaxToolKit:ModalPopupExtender ID="modalError" runat="server" TargetControlID="hiddenError"
            BackgroundCssClass="modalBackground" PopupControlID="panelError" OkControlID="btnErrorOK"
            BehaviorID="modalError" Y="0">
        </ajaxToolKit:ModalPopupExtender>
        <radContextMenu:RADContextMenu ID="ctmSecurityDrafts" runat="server" CssClass="mycontextMenu">
            <radContextMenu:ContextMenuItem ID="ctmSecurityDraftsView" Text="View" CssMenuItem="detailsIconBtn"
                OnClientClick="javascript:return false;">
            </radContextMenu:ContextMenuItem>
            <radContextMenu:ContextMenuItem ID="ctmSecurityDraftsDiscard" Text="Discard Draft"
                CssMenuItem="detailsIconBtn" OnClientClick="javascript:return false;">
            </radContextMenu:ContextMenuItem>
        </radContextMenu:RADContextMenu>
        <asp:Button ID="btnPageRefresh" runat="server" Text="btnPageRefresh" Style="display: none;"
            OnClientClick="javascript:window.location.reload();return false;" />
        <customScript:ScriptCustomControl ID="sccSecurityDrafts" runat="server" />
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
