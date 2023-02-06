<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApiMonitoring.aspx.cs"
    Inherits="com.ivp.common.CommonUI.CommonUI.ApiMonitoring" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolKit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="css/jquery-ui.min.css" rel="stylesheet" type="text/css" />
    <link href="css/ApiMonitoring.css" rel="stylesheet" type="text/css" />
    <link href="css/SMCss/SMSelect.css" rel="stylesheet" type="text/css" />
    <link href="css/SMCss/Customjquery.datetimepicker.css" rel="stylesheet" type="text/css" />
    <link href="css/SMCss/SMDatePickerControl.css" rel="stylesheet" type="text/css" />
    <link href="css/thirdparty/font-awesome.css" rel="stylesheet" type="text/css" />
    <link href="css/thirdparty/XMLDisplay.css" type="text/css" rel="stylesheet" />
    <link href="css/thirdparty/renderjson.css" type="text/css" rel="stylesheet" />

</head>
<body class="scroll">
    <form id="commonUIApiMonitoringForm" runat="server">

        <asp:ScriptManager ID="smExternalSystemStatus" runat="server" ScriptMode="debug">
            <Scripts>
                <asp:ScriptReference Name="com.ivp.rad.raduicontrols.Scripts.RADCommonScripts.js"
                    Assembly="RADUIControls" />
                <asp:ScriptReference Name="com.ivp.rad.raduicontrols.Scripts.RADUIScripts.debug.js"
                    Assembly="RADUIControls" />
                <asp:ScriptReference Name="com.ivp.rad.raduicontrols.Scripts.RScriptUtils.debug.js"
                    Assembly="RADUIControls" />
            </Scripts>
        </asp:ScriptManager>

        <div class="ApiMonitoringBody" id="ApiMonitoringBody">
            <!-- Beginning of Top Panel (Filters Container) -->
            <asp:Panel ID="panelTop" runat="server">
                <div id="ApiMonitoringTop">
                    <div id="ApiMonitoringFilteringSuperContainer">
                        <div id="ApiMonitoringFilteringContainer">
                            <div class="ApiMonitoringFiltersContainer">
                                <div id="ApiMonitoringFiltersSubContainer">
                                    <!-- div for Refresh Button (Float-Right) may be added here -->
                                    <div id="ApiMonoitoringUrlFilter" class="ApiMonoitoringFilter">
                                        <div id="ApiMonoitoringUrlFilterText" class="ApiMonoitoringFilterText">
                                            URL
                                        </div>
                                        <div id="ApiMonoitoringUrlFilterMultiSelect" class="ApiMonoitoringFilterMultiSelect">
                                            <!-- In this div, we add the multi-select selector. -->


                                        </div>
                                    </div>
                                    <div id="ApiMonoitoringMethodFilter" class="ApiMonoitoringFilter">
                                        <div id="ApiMonoitoringMethodFilterText" class="ApiMonoitoringFilterText">
                                            Method
                                        </div>
                                        <div id="ApiMonoitoringMethodFilterMultiSelect" class="ApiMonoitoringFilterMultiSelect">
                                            <!-- In this div, we add the multi-select selector. -->


                                        </div>
                                    </div>
                                    <div id="ApiMonoitoringClientMachineFilter" class="ApiMonoitoringFilter">
                                        <div id="ApiMonoitoringClientMachineFilterText" class="ApiMonoitoringFilterText">
                                            Client Machine
                                        </div>

                                        <div id="ApiMonoitoringClientMachineFilterMultiSelect" class="ApiMonoitoringFilterMultiSelect">
                                            <!-- In this div, we add the multi-select selector. -->


                                        </div>
                                    </div>

                                    <!--The below div contains the Date Filter-->
                                    <div id="ApiMonitoringDatesFilter" class="ApiMonoitoringFilter">
                                        <span id="ApiMonitoringDatesFilterText">Today</span>
                                        <i class="fa fa-chevron-down" style="padding-left: 20px; font-size: 10px;"></i>
                                    </div>
                                </div>
                            </div>

                            <!-- The div Below contains the Submit Button for Filters -->
                            <div id="ApiMonitoringFiltersButtonDiv">
                                <div id="ApiMonitoringFiltersButtonContainer">
                                    <input type="button" id="ApiMonitoringFiltersButton" class="CommonGreenBtnStyle" value="Apply" />
                                </div>
                                <div id="ApiMonitoringResetButton" onclick="document.location.reload(true)">
                                    <i class="fa fa-refresh" aria-hidden="true"></i>
                                </div>
                                <%--<input type="button" id="ApiMonitoringResetButton" class="ApiMonitoringGreyBtnStyle" value="Reset" onclick="document.location.reload(true)" />--%>
                            </div>
                            <div id="ApiMonitoringLiveCapturingContainer">
                                <div id="ApiMonitoringCapturingButtonDiv" class="ApiMonitoringLiveCapturingButtonDivs">
                                    <input type="button" id="ApiMonitoringCapturingButton" class="ApiMonitoringCapturingButtons CommonOrangeBtnStyle" value="Start Capturing" />
                                </div>
                                <%--<div id="ApiMonitoringStartCapturingButtonDiv" class="ApiMonitoringLiveCapturingButtonDivs">
                                    <input type="button" id="ApiMonitoringStartCapturingButton" class="ApiMonitoringCapturingButtons CommonGreenBtnStyle" value="Start Capturing"/>
                                </div>
                                <div id="ApiMonitoringStopCapturingButtonDiv" class="ApiMonitoringLiveCapturingButtonDivs" style="display:none">
                                    <input type="button" id="ApiMonitoringStopCapturingButton" class="ApiMonitoringCapturingButtons ApiMonitoringRedBtnStyle" value="Stop Capturing"/>
                                </div>--%>
                            </div>
                        </div>
                        <div id="ApiMonitoringFilteringErrorDiv">
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <!-- End of Top Panel (Filters Container) -->

            <!-- Beginning of Middle Panel -->
            <asp:Panel ID="panelMiddle" runat="server">
                <div class="ApiMonitoringMain">
                    <div id="ApiMonitoringMainLeftColumn" class="ApiMonitoringMainColumns ">
                        <%--<div id="ApiMonitoringMainLeftColumnTopDiv">
                            <div id="ApiMonitoringMainLeftColumnTopDivText">
                                <!--API Calls-->
                            </div>
                            <div id="ApiMonitoringLiveCapturingContainer">
                                <div id="ApiMonitoringStartCapturingButtonDiv" class="ApiMonitoringLiveCapturingButtonDivs">
                                    <input type="button" id="ApiMonitoringStartCapturingButton" class="ApiMonitoringCapturingButtons CommonGreenBtnStyle" value="Start Capturing"/>
                                </div>
                                <div id="ApiMonitoringStopCapturingButtonDiv" class="ApiMonitoringLiveCapturingButtonDivs">
                                    <input type="button" id="ApiMonitoringStopCapturingButton" class="ApiMonitoringCapturingButtons ApiMonitoringRedBtnStyle" value="Stop Capturing"/>
                                </div>
                            </div>
                        </div>--%>

                        <div id="ApiMonitoringChainHeaderDiv">
                            <div class="ApiMonitoringChainHeaderColumn1 ChainHeaderText">URL</div>
                            <div class="ApiMonitoringChainHeaderColumn2 ChainHeaderText">METHOD</div>
                            <%--<div class="ApiMonitoringChainColumn3">Client Machine</div>--%>
                            <div class="ApiMonitoringChainColumn3 ChainHeaderText"></div>
                        </div>
                        <div id="ApiMonitoringChainSuperParentDiv" class="scroll">
                            <div class="ApiMonitoringChainParentDiv" data-bind="foreach: chainListing">
                                <div class="ApiMonitoringChain" data-bind="click: apimonitoring.onClickChain">
                                    <%--<div class="ApiMonitoringChainName" data-bind="text: chainName"></div>--%>
                                    <div class="ApiMonitoringChainInfo" data-bind="style: { background: chainBackgroundColor(), color: chainTextColor() }">
                                        <div class="ApiMonitoringChainSubInfo1">
                                            <div class="ApiMonitoringChainColumn1 ApiMonitoringEllipses" data-bind="text: chainURL, style: { color: chainURLColor() }, attr: { title: chainURL }"></div>
                                            <div class="ApiMonitoringChainColumn2 ApiMonitoringEllipses" data-bind="text: chainMethod, attr: { title: chainMethod }"></div>
                                            <div class="ApiMonitoringChainColumn2c ApiMonitoringEllipses" data-bind="text: 'Thread Id : ' + threadId, attr: { title: threadId }"></div>
                                            <%--<div class="ApiMonitoringChainColumn3" data-bind="text: chainClientMachine"></div>--%>
                                        </div>

                                        <div class="ApiMonitoringChainSubInfo2">
                                            <div class="ApiMonitoringChainColumn1 ApiMonitoringEllipses" data-bind="text: chainDateTimeDisplayString(), attr: { title: chainDateTimeDisplayString() }"></div>
                                            <div class="ApiMonitoringChainColumn2a ApiMonitoringEllipses" data-bind="text: chainTimeTaken(), attr: { title: chainTimeTaken() }"></div>
                                            <div class="ApiMonitoringChainColumn2b ApiMonitoringEllipses" data-bind="text: 'Requested From : ' + chainClientMachine, attr: { title: chainClientMachine }"></div>
                                        </div>
                                    </div>

                                    <div class="ApiMonitoringChainColumn3 chainRightArrow" data-bind="visible: isSelected(), style: { color: chainBackgroundColor() }">
                                        <i class="fa fa-caret-right"></i>
                                    </div>
                                </div>
                            </div>
                        </div>


                        <%--<div class="ApiMonitoringChainParentDiv" data-bind="foreach: chainListing">
                            <div class="ApiMonitoringChain" data-bind="click: apimonitoring.onClickChain">
                                <div class="ApiMonitoringRowIcon">
                                    <i class="fa fa-caret-right" style="line-height: 20px;"></i>
                                </div>
                                <div class="ApiMonitoringChainName" data-bind="text: chainName"></div>
                                <div class="ApiMonitoringChainInfo">
                                    <div class="ApiMonitoringChainSubInfo1">
                                        <div class="ApiMonitoringChainSubInfo" data-bind="text: 'URL : ' + chainURL"></div>
                                        <div class="ApiMonitoringChainSubInfo" data-bind="text: 'Method : ' + chainMethod"></div>

                                    </div>
                                    <div class="ApiMonitoringChainSubInfo2">
                                        <div class="ApiMonitoringChainSubInfo" data-bind="text: 'Client Machine : ' + chainClientMachine"></div>
                                        <div class="ApiMonitoringChainSubInfo" data-bind="text: 'Client IP : ' + chainClientIP"></div>
                                        <div class="ApiMonitoringChainSubInfo" data-bind="text: 'Port : ' + chainPort"></div>
                                    </div>
                                    <div class="ApiMonitoringChainSubInfo3">
                                        <div class="ApiMonitoringChainSubInfo" data-bind="text: 'Start Date Time : ' + chainStartDateTime"></div>
                                        <div class="ApiMonitoringChainSubInfo" data-bind="text: 'End Date Time : ' + chainEndDateTime()"></div>
                                        <div class="ApiMonitoringChainSubInfo" data-bind="text: 'Time Taken : ' + chainTimeTaken()"></div>
                                    </div>
                                </div>
                            </div>
                        </div>--%>
                    </div>
                    <div id="ApiMonitoringMainRightColumn" class="ApiMonitoringMainColumns scroll">
                        <div id="ApiMonitoringSelectedApiDetails">

                            <!-- The below Div is the Top-half of Right Column -->
                            <div id="ApiMonitoringSelectedApiDetailsContainer1" class="ApiMonitoringSelectedApiDetailsContainers">
                                <!-- The Request Tab is in the div below -->
                                <div class="ApiMonitoringSelectedApiDetailsTabsContainer">
                                    <div id="ApiMonitoringSelectedApiDetailsContainer1Tab1" class="ApiMonitoringSelectedApiDetailsTabs">
                                        REQUEST
                                    </div>
                                </div>

                                <div id="ApiMonitoringSelectedApiDetailsRequestSubTabContainer" class="SubTabsContainer">
                                    <div id="ApiMonitoringSelectedApiDetailsRequestSubTabHeaderContainer" class="SubTabItemContainer RequestSubTabItem">
                                        Header
                                    </div>
                                    <div id="ApiMonitoringSelectedApiDetailsRequestSubTabBodyContainer" class="SubTabItemContainer RequestSubTabItem">
                                        Body
                                    </div>


                                    <!-- Download and Copy to Clipboard Icons Div -->
                                    <div id="ApiMonitoringSelectedApiDetailsContainer1Icons" class="ApiMonitoringSelectedApiDetailsContainerIconsDiv">
                                        <div id="ApiMonitoringSelectedApiDetailsContainer1DownloadIcon" class="ApiMonitoringSelectedApiDetailsContainerIcons">
                                            <i class="fa fa-download"></i>
                                            <%--<i class="fa fa-download" aria-hidden="true"></i>--%>
                                        </div>
                                        <div id="ApiMonitoringSelectedApiDetailsContainer1CopyToClipboardIcon" class="ApiMonitoringSelectedApiDetailsContainerIcons">
                                            <i class="fa fa-clipboard"></i>
                                            <%--<i class="fa fa-clipboard" aria-hidden="true"></i>--%>
                                        </div>
                                    </div>

                                </div>

                                <div id="ApiMonitoringSelectedApiDetailsRequestHeaderContainer">
                                    <!-- The below div is for Request Header -->
                                    <div id="ApiMonitoringSelectedApiDetailsContainer1HeaderText" class="ApiMonitoringSelectedApiDetailsContainerHeaderTextDivs scroll">
                                    </div>
                                </div>

                                <div id="ApiMonitoringSelectedApiDetailsRequestBodyContainer">
                                    <!--The JSON/XML text for Request gets entered in the div below -->
                                    <div id="ApiMonitoringSelectedApiDetailsContainer1Text" class="ApiMonitoringSelectedApiDetailsContainerTextDivs scroll">
                                    </div>
                                </div>
                            </div>

                            <!-- The below Div is the Bottom-half of Right Column -->
                            <div id="ApiMonitoringSelectedApiDetailsContainer2" class="ApiMonitoringSelectedApiDetailsContainers">
                                <!-- The Response Tab is in the div below -->
                                <div class="ApiMonitoringSelectedApiDetailsTabsContainer">
                                    <div id="ApiMonitoringSelectedApiDetailsContainer2Tab1" class="ApiMonitoringSelectedApiDetailsTabs">
                                        RESPONSE
                                    </div>
                                </div>

                                <div id="ApiMonitoringSelectedApiDetailsResponseSubTabContainer" class="SubTabsContainer">
                                    <div id="ApiMonitoringSelectedApiDetailsResponseSubTabHeaderContainer" class="SubTabItemContainer ResponseSubTabItem">
                                        Header
                                    </div>
                                    <div id="ApiMonitoringSelectedApiDetailsResponseSubTabBodyContainer" class="SubTabItemContainer ResponseSubTabItem">
                                        Body
                                    </div>
                                    <!-- View Logs -->
                                    <div id="ApiMonitoringSelectedApiDetailsResponseSubTabViewLogsContainer" class="SubTabItemContainer ResponseSubTabItem">
                                        Processing Logs
                                    </div>


                                    <!-- Download and Copy to Clipboard Icons Div -->
                                    <div id="ApiMonitoringSelectedApiDetailsContainer2Icons" class="ApiMonitoringSelectedApiDetailsContainerIconsDiv">
                                        <div id="ApiMonitoringSelectedApiDetailsContainer2DownloadIcon" class="ApiMonitoringSelectedApiDetailsContainerIcons">
                                            <i class="fa fa-download"></i>
                                            <%--<i class="fa fa-download" aria-hidden="true"></i>--%>
                                        </div>
                                        <div id="ApiMonitoringSelectedApiDetailsContainer2CopyToClipboardIcon" class="ApiMonitoringSelectedApiDetailsContainerIcons">
                                            <i class="fa fa-clipboard"></i>
                                            <%--<i class="fa fa-clipboard" aria-hidden="true"></i>--%>
                                        </div>
                                        <div id="ApiMonitoringSelectedApiDetailsContainer2CopyToClipboardIconBB" class="ApiMonitoringSelectedApiDetailsContainerIcons" style="display: none">
                                            <i class="fa fa-clipboard"></i>
                                            <%--<i class="fa fa-clipboard" aria-hidden="true"></i>--%>
                                        </div>
                                    </div>
                                </div>

                                <div id="ApiMonitoringSelectedApiDetailsResponseHeaderContainer">
                                    <!-- The below div is for Response Header -->
                                    <div id="ApiMonitoringSelectedApiDetailsContainer2HeaderText" class="ApiMonitoringSelectedApiDetailsContainerHeaderTextDivs scroll">
                                    </div>
                                </div>

                                <div id="ApiMonitoringSelectedApiDetailsResponseBodyContainer">
                                    <!--The JSON/XML text for Response gets entered in the div below -->
                                    <div id="ApiMonitoringSelectedApiDetailsContainer2Text" class="ApiMonitoringSelectedApiDetailsContainerTextDivs scroll">
                                    </div>
                                </div>

                                <div id="ApiMonitoringSelectedApiDetailsResponseViewLogsContainer">
                                    <!--The Log Data text for Response gets entered in the div below -->
                                    <div id="ApiMonitoringSelectedApiDetailsContainer3Text" class="ApiMonitoringSelectedApiDetailsContainerViewLogsTextDivs scroll">
                                    </div>
                                </div>

                            </div>

                        </div>
                    </div>
                </div>

                <div id="ApiMonitoringMainErrorDiv">
                    <%--<div id="ApiMonitoringMainErrorImageDiv">
                        <img src="../../App_Themes/Aqua/images/icons/no-data.png" />
                    </div>
                    <div id="ApiMonitoringMainErrorMessageDiv">
                        Sorry, we couldn't find any matches.
                        <br />
                        Please try again.
                    </div>--%>
                </div>
            </asp:Panel>
            <!-- End of Middle Panel -->

            <!-- Beginning of Bottom Panel -->
            <asp:Panel ID="panelBottom" runat="server">
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

                    <ajaxToolKit:ModalPopupExtender ID="modalError" Y="0" runat="server" TargetControlID="hdnError"
                        BackgroundCssClass="modalBackground" PopupControlID="pnlError" DropShadow="false"
                        BehaviorID="modalError" CancelControlID="btnError">
                    </ajaxToolKit:ModalPopupExtender>

                    <input id="hdnError" type="hidden" runat="server" />
                </div>
            </asp:Panel>
            <!-- End of Bottom Panel -->
            <input type="hidden" value="<%= System.Configuration.ConfigurationManager.AppSettings["NodeJsURl"] %>" id="APIMonitoringSocketUrl" />
        </div>

        <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/jquery-ui.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/knockout-3.4.0.js" type="text/javascript"></script>
        <script src="js/thirdparty/socket.io.js" type="text/javascript"></script>
        <script src="includes/jquery.slimscroll1.js" type="text/javascript"></script>
        <script src="includes/jquery.slimscrollHorizontal.js" type="text/javascript"></script>
        <script src="js/thirdparty/XMLDisplay.js" type="text/javascript"></script>
        <script src="js/thirdparty/renderjson.js" type="text/javascript"></script>
        <script src="includes/SMSlimscroll.js" type="text/javascript"></script>
        <script src="includes/MicrosoftAjax.js" type="text/javascript"></script>
        <script src="includes/SMSelect.js" type="text/javascript"></script>
        <script src="includes/RADCommonScripts.js" type="text/javascript"></script>
        <script src="includes/SecMasterScripts.js" type="text/javascript"></script>
        <script src="includes/RScriptUtils.debug.js" type="text/javascript"></script>
        <script src="includes/script.js" type="text/javascript"></script>
        <script src="includes/jquery.datetimepicker.js" type="text/javascript"></script>
        <script src="includes/SMDatePickerControl.js" type="text/javascript"></script>
        <%--<script src="includes/ApiMonitoringJson.js" type="text/javascript"></script>--%>
        <script src="includes/ApiMonitoring.js" type="text/javascript"></script>
    </form>
</body>

</html>
