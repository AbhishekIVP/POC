<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CASourcePrioritizationMain.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.CASourcePrioritizationMain" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="css/jquery-ui.min.css" rel="stylesheet" type="text/css" />
    <link href="css/CASourcePrioritization.css" rel="stylesheet" type="text/css" />
    <link href="css/SMCss/SMSelect.css" rel="stylesheet" type="text/css" />
    <link href="css/thirdparty/font-awesome.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="commonUICASourcePrioritizationMainForm" runat="server">

        <asp:ScriptManager ID="CASourcePrioritizationMainScriptManager" runat="server" ScriptMode="debug">
            <Scripts>
                <asp:ScriptReference Name="com.ivp.rad.raduicontrols.Scripts.RADCommonScripts.js"
                    Assembly="RADUIControls" />
                <asp:ScriptReference Name="com.ivp.rad.raduicontrols.Scripts.RADUIScripts.debug.js"
                    Assembly="RADUIControls" />
                <asp:ScriptReference Name="com.ivp.rad.raduicontrols.Scripts.RScriptUtils.debug.js"
                    Assembly="RADUIControls" />
            </Scripts>
        </asp:ScriptManager>

        <div id="CASourcePrioritizationMainBody">

            <!-- Beginning of Top Panel -->
            <asp:Panel ID="panelTop" runat="server">
                <div id="CASPM_Top">
                    <div id="CASPM_NumberParentDiv">
                        <%--Number of CorpAction Types Div--%>
                        <div id="CASPM_NumberDiv">
                        </div>
                    </div>

                    <%--Search--%>
                    <div id="CASPM_SearchParentDiv">
                        <div id="CASPM_SearchLabel">Search</div>
                        <div id="CASPM_SearchBarParentDiv">
                            <input id="CASPM_SearchInputBox" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <!-- End of Top Panel -->

            <!-- Beginning of Middle Panel -->
            <asp:Panel ID="panelMiddle" runat="server">
                <div id="CASPM_Main">
                    <div id="CASPM_TableHeaderRow">
                        <div class="CASPM_TableHeaderColumns CASPM_Col1_Width">
                            Corporate Action Type
                        </div>
                        <div class="CASPM_TableHeaderColumns CASPM_Col2_Width">
                            Priority Status
                        </div>
                        <div class="CASPM_TableHeaderColumns CASPM_Col3_Width">
                            Data Source Status
                        </div>
                        <div class="CASPM_TableHeaderColumns CASPM_Col4_Width">
                            Configure
                        </div>
                    </div>

                    <div id="CASPM_GridParentDiv" data-bind="foreach: corpactionListing">
                        <div class="CASPM_TableRow">
                            <div class="CASPM_TableCell CASPM_Col1_Width" data-bind="text: corpActionTypeName">
                            </div>
                            <div class="CASPM_TableCell CASPM_Col2_Width" data-bind="text: priorityStatus">
                            </div>
                            <div class="CASPM_TableCell CASPM_Col3_Width" data-bind="text: dataSourceStatus">
                            </div>
                            <div class="CASPM_TableCell CASPM_ConfigureCol CASPM_Col4_Width">
                                <i class="fa fa-cog" aria-hidden="true" data-bind="visible: showConfigure"></i>
                            </div>
                        </div>
                    </div>

                </div>
            </asp:Panel>
            <!-- End of Middle Panel -->

            <!-- Beginning of Bottom Panel -->
            <asp:Panel ID="panelBottom" runat="server">
                <div class="PanelsContainerDiv">
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
                                                            <asp:Button ID="btnError" Text="Close" CssClass="pageButton" runat="server" CausesValidation="false"
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
                    <%--<asp:Panel ID="pnlDelete" runat="server" Style='display: none;'>
                        <table width="45%" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="alertUnsuccessConfirm">
                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="unSuccessHead">Delete Key
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="alertMessage">
                                                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                    <tr style="display: none;">
                                                        <td>
                                                            <h5>[Key:
                                                        <asp:Label ID="lblDelete" runat="server"></asp:Label>]</h5>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblDeleteKey" Font-Bold="true" CssClass="alertLabel" runat="server"
                                                                Text="Are you sure you want to DELETE this Key?">
                                                            </asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="alertUnsuccessConfirmClose">
                                                            <asp:Button ID="btnDeleteYES" Text="Yes" CssClass="CommonContinueButton" runat="server"
                                                                ToolTip="Click to Confirm" />
                                                            <asp:Button ID="btnDeleteNO" Text="No" CssClass="CommonDeleteButtonAsText" runat="server"
                                                                ToolTip="Click to Cancel" TabIndex="1" />
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

                    <ajaxToolKit:ModalPopupExtender ID="modalSuccess" Y="0" runat="server" TargetControlID="hdnSuccess"
                        BackgroundCssClass="modalBackground" PopupControlID="pnlSuccess" DropShadow="false"
                        BehaviorID="modalSuccess" CancelControlID="btnOk">
                    </ajaxToolKit:ModalPopupExtender>
                    <ajaxToolKit:ModalPopupExtender ID="modalError" Y="0" runat="server" TargetControlID="hdnError"
                        BackgroundCssClass="modalBackground" PopupControlID="pnlError" DropShadow="false"
                        BehaviorID="modalError" CancelControlID="btnError">
                    </ajaxToolKit:ModalPopupExtender>
                    <%--<ajaxToolKit:ModalPopupExtender ID="modalDelete" Y="0" runat="server" TargetControlID="hdnDelete" 
                        BackgroundCssClass="modalBackground" PopupControlID="pnlDelete" DropShadow="false"
                        BehaviorID="modalDelete">
                    </ajaxToolKit:ModalPopupExtender>--%>

                    <%--<input id="hdnDelete" type="hidden" runat="server" />--%>
                    <input id="hdnSuccess" type="hidden" runat="server" />
                    <input id="hdnError" type="hidden" runat="server" />
                </div>
            </asp:Panel>
            <!-- End of Bottom Panel -->
        </div>


        <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/jquery-ui.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/knockout-3.4.0.js" type="text/javascript"></script>
        <script src="includes/RADCommonScripts.js" type="text/javascript"></script>
        <script src="includes/SecMasterScripts.js" type="text/javascript"></script>
        <%--<script src="includes/RScriptUtils.debug.js" type="text/javascript"></script>--%>
        <script src="includes/script.js" type="text/javascript"></script>
        <script src="includes/CASourcePrioritizationMain.js" type="text/javascript"></script>

    </form>
</body>
</html>
