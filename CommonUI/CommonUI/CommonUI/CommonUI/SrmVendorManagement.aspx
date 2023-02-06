<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SrmVendorManagement.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SrmVendorManagement" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <!-- CSS TODO -->
    <link href="css/SrmVendorManagement.css" rel="stylesheet" type="text/css" />
    <link href="css/thirdparty/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="css/SMCss/SMSelect.css" rel="stylesheet" type="text/css" />
</head>


<body>
    <form id="srmVendorManagementForm" runat="server">
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
        <div class="srmVMHeaderFilters">
            <div class="srmVMdataSource">
                <div id="BloomBerg" vendortype="1">Bloomberg</div>
                <div id="Reuters" vendortype="2">Reuters</div>
                <div id="MarkIt">MarkIt</div>
            </div>
            <div class="MarginSet">
                <div class="srmVMDataRequestMethod">
                    <div id="FTP">FTP</div>
                    <div id="SAPI">SAPI</div>
                    <div id="HeavyLite">HeavyLite</div>
                    <div id="GlobalApi">GlobalApi</div>
                </div>
                <div id="UpdatePrefParent">
                    <div id="SavePreference">Save</div>
                </div>
            </div>

        </div>

        <div class="srmVMBody">

            <div id="PreferenceBody">
                <div id="LeftBodyMenu">
                    <div id="PreferenceHeader">
                        <div id="PreferenceHeaderText">PREFERENCE LIST</div>
                        <i class=" fa fa-plus-circle PreferenceSelectorClass" id="AddNewPreference"></i></div>
                    <div id="PreferencesList" class="scroll"></div>
                    <div id="PreferenceBottom">

                        <%--<div id="AddNewPreference" class="PreferenceSelectorClass">Add New</div>--%>
                    </div>

                </div>
                <div id="RightBodyMenu">
                    <div id="PreferenceSelector">
                        <div id="NamePreference">
                            <input id="NamePreferenceInput" placeholder="Enter preference text..." />
                        </div>
                    </div>
                    <%--<div id="srmSaveChanges">
            SAVE
        </div>--%>
                    <div class="HeadingStyle">
                        Configurations
                    </div>
                    <div class="srmVMFixedConfigurations">
                        <div id="srmVMFixedConfigTable">
                        </div>
                    </div>

                    <div class="HeadingStyle" id="HeadingText">
                        Headers
                    </div>
                    <%--Dropdown Menu--%>
                    <div id="srmVMRequestType">
                        <div id="srmVMRequestTypeText" class="CommonLabelStyle attrNameCss">Request Type</div>
                        <div id="srmVMRequestTypeDD"></div>
                    </div>



                    <%--<div class = "srmVMRequestBody">
            <%--<div id="srmVMRequestType">

            <%--</div>
            <div id="srmVMHeaderInput"> 
            <div id="HeaderName">Header Name:</div> 
            <div><input id="HeaderNameInput" /></div>
            <div id="HeaderValue">Value:</div>
            <div><input id="HeaderValueInput" /></div>
            </div>--%>

                    <%--<div id ="srmVMHeaderTable">
                    --%>

                    <!-- TRYING -->
                    <div runat="server" id="trCustomBbgHeaderGrid">
                        <div class="CustomBbgHeaderGrid" style="width: 100%" align="center" colspan="4">
                            <asp:Panel ID="panelBloombergHeaders" runat="server">
                                <div id="bbgHeaderGrid" class="SMNewGridTable" style="width: 50%; box-shadow: 0px 0px 9px -3px #aaaaaa; margin: 0 auto;">
                                    <div style="border-bottom: 0; width: 100%; height: 30px;">
                                        <div class="SMNewGridHeaderRow">
                                            <div class="SMNewGridHeaderElement">
                                                Header Name
                                            </div>
                                            <div class="SMNewGridHeaderElement" style="border-right: none;">
                                                Header Value
                                            </div>
                                            <div class="SMNewGridHeaderElement" style="width: 0%; border: 0px;">
                                            </div>
                                        </div>
                                    </div>
                                    <div id="newGridRowDiv" countrows="0">
                                        <div class="SMNewGridBodyElement">
                                            <asp:TextBox ID="txtHeaderName" runat="server" CssClass="CommonInputStyle attrValueCssDropdown gridDropdownCss" placeholder="Enter Header Name ... "
                                                Style="border: none !important;">
                                            </asp:TextBox>
                                        </div>
                                        <div class="SMNewGridBodyElement">
                                            <asp:TextBox ID="txtHeaderValue" runat="server" CssClass="CommonInputStyle attrValueCssText gridDropdownCss"
                                                placeholder="Enter Header Value ... " Style="border: none !important;"></asp:TextBox>
                                        </div>
                                        <div class="SMNewGridBodyElement" style="width: 0%; border: 0px;">
                                            <span id="addRowButton" style="font-size: 16px; cursor: pointer; padding-left: 13px;">+</span>
                                        </div>
                                    </div>
                                    <div id="FixedDivForSlimScrollVMHeaders">
                                    <div id="GridRowParent">

                                    </div>
                                        </div>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div id="testScroll"></div>
        <div id="testScroll2"></div>
        <!-- TEST -->
        <asp:Panel ID="panelBottom" runat="server">
            <div id="AddButtonPopup" class="AddButtonPopup" style="display: none; font-size: 12px !important; width: auto; height: 350px; background: white; z-index: 10000; position: fixed; padding-top: 6px; border: 1px solid rgb(210, 210, 210); border-top-width: 0px; border-bottom-right-radius: 7px; border-bottom-left-radius: 7px;">
                <%--                     <div id="AddButtonText">Preference Name</div>--%>
                <%--<input id ="AddButtonPopupInput"/>--%>
                
                <div><div id="PreferenceToCloneFrom" style="margin-left: 10px;  margin-bottom: 10px;">Clone From:</div></div>
                <div id="SMSelectAddButtonContainer" style="margin-left: 10px; margin-bottom: 12px;"></div>
                    
                <div id="AddButtonSavePreferenceParent"></div>
                <div id="AddButtonSavePreference" style="color:white;margin-left : 65%; width:25%; background:#43d9c6; text-align:center; padding:3px; cursor:pointer; border-radius:5px" >Continue</div>
            </div>
            <div class="PanelsContainerDiv">
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
            </div>
        </asp:Panel>
        <input type="hidden" runat="server" id="hdnHeaderName" />
        <input type="hidden" runat="server" id="hdnHeaderValue" />

        <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>

        <script src="includes/SecMasterScripts.js" type="text/javascript"></script>
        <script src="js/thirdparty/jquery-ui.min.js" type="text/javascript"></script>
        <script src="includes/SrmVM.js" type="text/javascript"></script>
        <%--<script src="includes/SrmVMDataJson.js" type="text/javascript"></script>--%>
        <script src="includes/MicrosoftAjax.js" type="text/javascript"></script>
        <script src="includes/SMSelect.js" type="text/javascript"></script>
        <script src="includes/SMSlimscroll.js" type="text/javascript"></script>
    </form>
</body>



</html>
