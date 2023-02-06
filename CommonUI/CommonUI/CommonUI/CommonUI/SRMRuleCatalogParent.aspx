<%@ Page Title="" Language="C#" MasterPageFile="~/CommonUI/SRMMaster.Master" AutoEventWireup="true" CodeBehind="SRMRuleCatalogParent.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMRuleCatalogParent" %>

<asp:Content ID="contentFilterSection" ContentPlaceHolderID="ContentPlaceHolderFilterSection" runat="server">
    <div style="height: 35px; padding-top: 5px;">
        <div id="SRMRuleCatalogFilterSection" class="RC_FilterSection" style="display: inline-block; float: right;">
            <div style="float: right;">
                <i id="SRMRuleCatalogRightFilter" class="fa fa-filter btnClassHeader" style="display: inline-block; margin-right: 14px; margin-top: 5px; font-size: 18px; color: rgb(210, 210, 210);"></i>
            </div>
            <div id="SRMRuleCatalogRightFilterContainer">
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="SRMContentPlaceHolderMiddle" runat="server">
    <div id="container1" style="width: 100%;">
        <%--Commented By Dhruv--%>
        <%--<iframe name="TabsInnerIframe1" class="InnerFrame" id="InnerFrameId1" style="width:100%; border: none; padding-top: 0px;position:relative; height:720px;"></iframe>--%>
        <div id="SRFPMRuleCatalogueParentDiv" style="font-size: 0px;">
            <div id="RC_LeftMainDiv" class="RC_MainDivs">

                <%--MultiSelect for Section Type--%>
                <div id="RC_LeftTopDiv">
                    <div id="RC_SectionTypeSMSelectDiv">
                    </div>
                    <div id="RC_LeftPaneSearchParentDiv">
                        <i class="fa fa-search notify-icon"></i>
                        <div id="RC_LeftPaneSearchDiv">
                            <input id="RC_LeftPaneSearchInputBox" autocomplete="off" />
                        </div>
                    </div>
                </div>

                <%--Section Data Start--%>
                <div id="RC_SectionData" class="scroll">
                    <div id="RC_SubModulesParentDiv" data-bind="foreach: typeListing">
                        <div class="RC_TypeRow" data-bind="visible: typeToShow(), style: { background: typeBackgroundColor(), color: typeTextColor() }">

                            <div class="RC_TypeInfo">
                                <%--Type Name--%>
                                <div data-bind="text: typeName, click: htmlBulkUploadGrid.onClickType, attr: { title: typeName }" class="RC_TypeName SRMRuleCatalogParentEllipses">
                                </div>
                                <%--Parent Type--%>
                                <div data-bind="text: parentType, visible: parentTypetoShow, attr: { title: parentType }" class="RC_ParentType SRMRuleCatalogParentEllipses"></div>
                            </div>

                            <%--List of Rule Types--%>
                            <div class="RC_TypeData">
                                <div data-bind="foreach: ruleTypeListing, visible: typeIsSelected()" class="RC_TypeDataLeft">
                                    <div data-bind="text: ruleTypeName, click: htmlBulkUploadGrid.onClickRuleType, style: { background: ruleTypeBackgroundColor() }" class="RC_RuleTypeName SRMRuleCatalogParentEllipses">
                                    </div>
                                </div>
                                <div data-bind="foreach: ruleTypeListing, visible: typeIsSelected()" class="RC_TypeDataRight">
                                    <div class="RC_RightCaret">
                                        <div data-bind="visible: ruleTypeIsSelected()"><i class="fa fa-caret-right"></i></div>
                                    </div>
                                </div>
                            </div>

                        </div>

                    </div>
                </div>
                <%--Section Data End--%>
            </div>


            <div id="RC_RightMainDiv" class="RC_MainDivs">
                <div id="SMAttributeRuleContainer">
                    <div id="SMRuleSetup_Header_Contents">
                        <div id="SMRuleSetup_TypeName">
                        </div>
                        <div class="SMRuleSetup_Expand_Collapse SMRuleSetup_hideContents">
                            <div class="SMRuleSetup_SearchPanel">
                                <i class="fa fa-search notify-icon" id="SMRuleSetup_searchIcon" style="display: inline-block; position: relative; color: #a9a9a9; font-size: 15px;"></i>
                                <input id="SMRuleSetup_SearchOnRules" type="text" placeholder="Search" />
                            </div>
                            <div id="SMRuleSetup_Mapped_Types" class="SMRuleSetup_Mapped_Types">
                                Mapped Types
                            </div>
                            <div class="SMRuleSetup_Collapse">
                                Collapse All
                            </div>
                        </div>
                    </div>

                    <div class="SMRuleSetup_Scrollable">

                        <div id="SMRuleSetup_RulesContentId" class="SMRuleSetup_RulesContent" data-bind="foreach: rules">
                            <div class="SMRuleSetup_Rules" iscollapsed="0" data-bind="visible: ruletoShow()">
                                <div class="SMRuleSetup_arrow SMRuleSetup_hideContents" style="display: inline-block; cursor: pointer; left: 14px; position: relative; width: 98%; margin-top: 5px; margin-bottom: 5px;" data-bind="visible: attributeToDisplay">
                                    <%--data-bind="visible: attributeLevelToShow"--%>
                                    <div class="SMRuleSetup_Attr_Ruletype_Container">
                                        <i class="fa fa-caret-right" style="width: 8px; display: none;" aria-hidden="true"></i>
                                        <div class="SMRuleSetup_attrname" data-bind="text: attributeToDisplay, attr: { title: attributeToDisplay }">
                                            <%--data-bind="text: attributeToDisplay"--%>
                                        </div>
                                        <div style="font-weight: bold; margin-left: 3px; margin-right: 3px; display: inline-block;">
                                        </div>

                                    </div>
                                    <div class="SMRuleSetup_Depending_Dependent_RuleContainer" data-bind="visible: dependingDependentToShow, attr: { 'depending': depending, 'dependent': dependent }">
                                        <%--visible: dependingDependentToShow,--%>
                                        <div class="SMRuleSetup_Depending_Container">
                                            <div class="SMRuleSetup_Depending_Label" data-bind="visible: depending">Derived Using :</div>
                                            <div class="SMRuleSetup_Depending" data-bind="text: dependingToShow, attr: { title: depending }"></div>
                                            <div class="SMRuleSetup_Depending_More" data-bind="text: dependingToShowMore"></div>
                                            <%--, click: $parent.ShowDepending--%>
                                        </div>
                                        <div class="SMRuleSetup_Dependent_Container">
                                            <div class="SMRuleSetup_Dependent_Label" data-bind="visible: dependent">Affects :</div>
                                            <div class="SMRuleSetup_Dependent" data-bind="text: dependentToShow, attr: { title: dependent }"></div>
                                            <div class="SMRuleSetup_Dependent_More" data-bind="text: dependentToShowMore"></div>
                                            <%--, click: $parent.ShowDependent--%>
                                        </div>
                                    </div>
                                </div>
                                <div class="SMRuleSetup_hidden_contents" data-bind="foreach: ruletypes">

                                    <div class="SMRuleSetup_ruletypename" data-bind="text: ruleTypeName">
                                    </div>
                                    <div class="SMRuleSetup_rulecount">
                                        (<span data-bind="text: attrRules().length"></span>)
                                    </div>
                                    <hr class="SMRuleSetup_CatalogHr" />
                                    <div data-bind="foreach: attrRules">
                                        <div class="SMRuleSetup_AttributeRulesBinding" data-bind="attr: { 'rulegroupname': ruleGroupName, 'rulename': ruleName, 'priority': priority, 'ruleState': ruleState ? 'true' : 'false', 'ruletext': ruleText, 'commonRuleSectypes': commonRuleSectypes }, visible: specificRuleToShow()">
                                            <br />
                                            <div class="SMRuleSetup_ruleObjectContainer">
                                                <div class="SMRuleSetup_ruleandgroup">
                                                    <div class="SMRuleSetup_rulegroupname" data-bind="visible: ruleGroupName">
                                                        <div class="SMRuleSetup_rulegroupname" data-bind="text: ruleGroupName">
                                                        </div>
                                                        <div class="SMRuleSetup_colon">
                                                            :
                                                        </div>
                                                    </div>
                                                    <div class=" SMRuleSetup_rulename" data-bind="text: ruleName">
                                                    </div>
                                                </div>
                                                <div class="SMRuleSetup_pud">
                                                    <div class="SMRuleSetup_isCommonRule" data-bind="visible: isCommonRule">
                                                        <div class="SMRuleSetup_ApllicableOn">Applicable On</div>
                                                        <div class="SMRuleSetup_ApllicableOnCount" data-bind="text: isCommonRuleCount"></div>
                                                    </div>
                                                    <div class="SMRuleSetup_priority_Container" data-bind="visible: ($parent.ruleTypeName != 'Conditional Filter')">
                                                        <div class="SMRuleSetup_priority_icon">
                                                        </div>
                                                        <div class="SMRuleSetup_priority_catalog" data-bind="text: priority">
                                                        </div>
                                                    </div>

                                                    <!-- State button start -->
                                                    <div class="SMRuleSetup_change_state" data-bind="visible: ($parent.ruleTypeName != 'Conditional Filter')">
                                                        <div data-bind="css: { 'SMRuleSetup_RuleStateON': ruleState, 'SMRuleSetup_RuleStateOFF': !ruleState }, text: ruleState ? 'ON' : 'OFF'">
                                                        </div>
                                                    </div>
                                                    <!-- State button end-->
                                                </div>
                                                <%--   </div>--%>
                                                <div class="SMRuleSetup_ShowRule">
                                                    <div class="SMRuleSetup_RuleTextWrapper">
                                                        <%-- <label style="display: inline-block;">
                                                        Rule :
                                                    </label>--%>
                                                        <div class="SMRuleSetup_RuleText" data-bind="text: ($parent.ruleTypeName != 'Conditional Filter') ? ('Rule : ' + ruleText) : ruleText">
                                                        </div>
                                                    </div>
                                                    <!-- edited -->
                                                </div>

                                            </div>
                                            <hr class="SMRuleSetup_hr" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="SMRuleSetup_NullAttributeRuleContainerForSearch">
                        </div>
                    </div>
                </div>
                <div id="SMRuleSetup_NullAttributeRuleContainer">
                </div>
            </div>

            <div class="SMRuleSetup_Outer_Div_Pointer">
                <div class="SMRuleSetup_Inner_Div_Pointer">
                </div>
            </div>
            <div class="SMRuleSetup_attrContainerDiv">
            </div>
        </div>

        <div id="SRFPMRuleCatalogueCommonErrorMsgDiv">
        </div>

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
    <asp:HiddenField ClientIDMode="Static" ID="hdnId" runat="server" />
    <asp:HiddenField ClientIDMode="Static" ID="hdnModuleName" runat="server" />

    <%--<script src="includes/updatePanel.js" type="text/javascript"></script>--%>
    <script type="text/javascript" src="includes/ruleEditorScroll.js"></script>
    <script type="text/javascript" src="includes/bootstrap2-toggle.js"></script>
    <script type="text/javascript" src="includes/SRMRuleCatalogParent.js"></script>
    <%--<script src="includes/SRM_RuleCatalogRender.js" type="text/javascript"></script>--%>
    <script type="text/javascript" src="js/CommonModuleTabs.js"></script>
    <script type="text/javascript" src="js/thirdparty/knockout-3.4.0.js"></script>
    <script src="includes/knockout-fast-foreach.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="includes/XMLHttpSyncExecutor.js"></script>
    <script src="includes/RScriptUtils.debug.js" type="text/javascript"></script>
    <script type="text/javascript" src="includes/SMSlideMenu.js"></script>
    <script type="text/javascript" src="js/thirdparty/bootstrap.min.js"></script>
    <script src="includes/neogrid.client.js" type="text/javascript"></script>
    <script src="includes/xlgrid.loader.js" type="text/javascript"></script>
    <script type="text/javascript" src="includes/bootstrap2-toggle.js"></script>
    <script src="includes/SecMasterScripts.js" type="text/javascript"></script>

    <link href="css/thirdparty/font-awesome.min.css" rel="stylesheet" />
    <link href="css/SMCss/SMSelect.css" rel="stylesheet" />
    <link href="css/SRMRuleCatalog.css" rel="stylesheet" />
    <link href="css/SRMRuleCatalogParent.css" rel="stylesheet" />
    <link href="css/SMSlideMenu.css" rel="stylesheet" />

</asp:Content>
