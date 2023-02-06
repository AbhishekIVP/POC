<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CASourcePrioritization.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.CASourcePrioritization" %>

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
    <form id="commonUICASourcePrioritizationForm" runat="server">

        <asp:ScriptManager ID="CASourcePrioritizationScriptManager" runat="server" ScriptMode="debug">
            <Scripts>
                <asp:ScriptReference Name="com.ivp.rad.raduicontrols.Scripts.RADCommonScripts.js"
                    Assembly="RADUIControls" />
                <asp:ScriptReference Name="com.ivp.rad.raduicontrols.Scripts.RADUIScripts.debug.js"
                    Assembly="RADUIControls" />
                <asp:ScriptReference Name="com.ivp.rad.raduicontrols.Scripts.RScriptUtils.debug.js"
                    Assembly="RADUIControls" />
            </Scripts>
        </asp:ScriptManager>
        

        <div id="CASourcePrioritizationBody">
            <!-- Beginning of Top Panel -->
            <asp:Panel ID="panelTop" runat="server">
                <div id="CASP_Top">
                    <%-- Top Div for Sectype Name and Save & Reset Buttons --%>
                    <div id="CASP_TopDiv1">
                        <%-- Sectype Name --%>
                        <div id="CASP_CorpActionTypeNameParentDiv">
                            <div id="CASP_CorpActionTypeName">
                            </div>
                        </div>

                        <%-- Save & Reset Buttons --%>
                        <div id="CASP_TopButtonsParentDiv">
                            <div id="CASP_TopSaveButtonDiv" class="CASP_CommonGreenBtnStyle CASP_TopButtonCSSClass">Save</div>
                            <div id="CASP_TopResetButtonDiv" class="CASP_CommonTransparentBtnStyle CASP_TopButtonCSSClass" onclick="document.location.reload(true)">Reset</div>
                        </div>
                    </div>

                    <%-- Top Div for Entity Types for Creation --%>
                    <%--<div id="CASP_TopDiv2">
                    
                    </div>--%>
                </div>
            </asp:Panel>
            <!-- End of Top Panel -->

            <!-- Beginning of Middle Panel -->
            <asp:Panel ID="panelMiddle" runat="server">
                <div id="CASP_Main">

                    <%--Exception Related Checkboxes--%>
                    <div id="CASP_OptionsDiv">
                        <div class="CASP_Options_CSSCLass">
                            Show exception for missing values in first priority vendor :
                            <input id="CASP_Option1" type="checkbox" name="CASP_Option1Name" value="showExceptionFirstPriorityVendor" />
                        </div>
                        <div class="CASP_Options_CSSCLass">
                            Delete previous exceptions :
                            <input id="CASP_Option2" type="checkbox" name="CASP_Option2Name" value="deleteException" />
                        </div>
                        <div class="CASP_Options_CSSCLass">
                            Show exception for missing values in all vendors :
                            <input id="CASP_Option3" type="checkbox" name="CASP_Option3Name" value="showExceptionAllVendor" />
                        </div>
                        <div class="CASP_Options_CSSCLass">
                            Run Calculated Fields :
                            <input id="CASP_Option4" type="checkbox" name="CASP_Option4Name" value="runCalculatedFields" />
                        </div>
                    </div>

                    <%-- TABS --%>
                    <div id="CASP_TabsSuperParentDiv">

                        <%--Left Arrow--%>
                        <%--<div id="CASP_TabsLeftArrowDiv">
                            <i class="fa fa-chevron-left"></i>
                        </div>--%>

                        <%--TABS Parent Div--%>
                        <div id="CASP_TabsParentDiv" data-bind="foreach: tabListing">
                            <div data-bind="text: tabName, attr: { id: 'Tab_' + tabID, class: 'TabNameDiv ' + selectedTabNameClassName() }, click: caSourcePrioritization.onClickTab">
                            </div>
                        </div>


                        <%--Right Arrow--%>
                        <%--<div id="CASP_TabsRightArrowDiv">
                            <i class="fa fa-chevron-right"></i>
                        </div>--%>
                    </div>

                    <%-- GRID Begins --%>
                    <div id="CASP_TabsGridsSuperParentDiv" data-bind="foreach: tabListing">

                        <div data-bind="visible: tabIsSelected(), attr: { id: 'TabGrid_' + tabID }" class="CASP_TabGridParentDiv scroll">
                            <%--Column Header--%>
                            <div class="CASP_TableRow CASP_TableHeaderRow">
                                <%--Attribute--%>
                                <div class="CASP_HeaderColumn CASP_TableCellWidthFirst CASP_Ellipses">
                                    Attribute
                                </div>


                                <%--Priority--%>
                                <div data-bind="foreach: columnNameList" class="CASP_TablePartialRow">
                                    <div data-bind="text: columnName" class="CASP_HeaderColumn CASP_TableCellWidth CASP_Ellipses">
                                    </div>
                                </div>
                            </div>

                            <%--Select ALL--%>
                            <div class="CASP_TableRow CASP_TableRow1">
                                <div id="CASP_SelectAll" class="CASP_TableCell CASP_TableCellWidthFirst">SELECT ALL </div>
                                <div data-bind="foreach: selectAllList" class="CASP_TablePartialRow">
                                    <div data-bind="text: selectAllText, click: caSourcePrioritization.onClickSelectAllPriority, attr: { id: 'CASP_SelectAll_' + tabID + '_' + priority }" class="CASP_TableCell CASP_TableCellWidth CASP_EditableSMSelect">
                                    </div>
                                </div>
                            </div>

                            <%--Actual Grid Data--%>
                            <div data-bind="foreach: tabAttributesDetails">
                                <div class="CASP_TableRow CASP_TableRow1">
                                    <%--Attribute Name--%>
                                    <div class="CASP_TableCell CASP_TableCellWidthFirst CASP_Ellipses" data-bind="text: attributeDisplayName">
                                    </div>

                                    <%--Attribute Vendor Name--%>
                                    <div data-bind="foreach: attributeVendorList" class="CASP_TablePartialRow">
                                        <div data-bind="text: vendorName(), click: caSourcePrioritization.onClickAttributeVendorPriority, attr: { id: 'CASP_' + $parent.attributeID + '_' + vendorPriorityNumber }" class="CASP_TableCell CASP_TableCellWidth CASP_EditableSMSelect">
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                    <%-- GRID Ends --%>


                    <%--For SRFPMInputObjectControl--%>
                    <div id="CASP_SRFPMInputObjectControl1">
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
        <script src="includes/SMSlimscroll.js" type="text/javascript"></script>
        <script src="includes/SMSelect.js" type="text/javascript"></script>
        <script src="includes/RADCommonScripts.js" type="text/javascript"></script>
        <script src="includes/SecMasterScripts.js" type="text/javascript"></script>
        <%--<script src="includes/RScriptUtils.debug.js" type="text/javascript"></script>--%>
        <script src="includes/script.js" type="text/javascript"></script>
        <script src="includes/SRFPMInputObjectControl.js" type="text/javascript"></script>
        <script src="includes/CASourcePrioritization.js" type="text/javascript"></script>

    </form>
</body>
</html>
