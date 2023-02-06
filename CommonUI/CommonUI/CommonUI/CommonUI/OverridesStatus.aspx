<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OverridesStatus.aspx.cs"
    Inherits="Reconciliation.CommonUI.OverridesStatus" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!--CSS FILES-->
    <link href="css/AquaStyles.css" rel="stylesheet" type="text/css" />
    <link href="css/thirdparty/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="css/thirdparty/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="css/SMCss/SMSelect.css" rel="stylesheet" type="text/css" />
    <link href="css/CommonModuleTabs.css" rel="stylesheet" type="text/css" />
<%--    <link href="css/grid.css" rel="stylesheet" type="text/css" />
    <link href="css/xlneogrid.css" rel="stylesheet" type="text/css" />
    <link href="css/neogrid.css" rel="stylesheet" type="text/css" />--%>
    <link href="css/Z_IagoCommonThemes.css" rel="stylesheet" type="text/css" />
    <link href="css/Customjquery.datetimepicker.css" rel="stylesheet" type="text/css" />
    <link href="css/jquery.datetimepicker.css" rel="stylesheet" type="text/css" />
    <link href="css/RADDateTimePicker.css" rel="stylesheet" type="text/css" />
    <link href="css/overrideStatus.css" rel="stylesheet" type="text/css" />

    <!--JS FILES-->
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
    <script src="js/OverridesStatus.js" type="text/javascript"></script>
</head>
<body>
    <form id="commonUIOverrideStatusForm" name="commonUIOverrideStatus" runat="server">
        <asp:ScriptManager ID="commonUIOverrideStatus" runat="server" ScriptMode="Release"
            EnableViewState="true" AllowCustomErrorsRedirect="false" AsyncPostBackTimeout="20000"
            EnablePageMethods="true" LoadScriptsBeforeUI="true">
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
        <div>
            <asp:Panel ID="panelTop" runat="server" class="os_panelSections">
                <div id="os_moduleTabs">
                </div>
            </asp:Panel>
            <asp:Panel ID="panelMiddle" runat="server" class="os_panelSections">
                <div id="overrideStatusScreenSection">
                    <div id="overrideStatusActionSection" class="overrideStatusActionSectionStyle">
                        <div id="overrideStatusActionList" class="overrideStatusActionListStyle">
                            <div id="overrideStatusDeleteAction" class="overrideStatusAction overrideStatusDisabled">Delete</div>
                        </div>
                    </div>
                    <div id="overrideStatusNoGridSection" style="display: none;">
                        <div class="overrideStatusNoGridSection"></div>
                    </div>
                    <div id="overrideStatusNeoGrid">
                    </div>
                </div>
                <div id="overridesFromSearchScreenSection">
                    <div class="overrideStatusActionBtns overrideStatusHorizontalAlign">
                        <div id="oversideStatusViewOverrides" class="overrideStatusHorizontalAlign overrideStatusActionGreen overrideStatusAction">View Overrides</div>
                        <div id="overridesStatusSaveOverride" class="overrideStatusHorizontalAlign overrideStatusAction">Save</div>
                    </div>
                    <div id="overrideStatusAddAttributeSection" class="">
                        <div class="overrideStatusTitle overrideStatusHorizontalAlign">ADD ATTRIBUTES</div>
                        <div class="overrideStatusAddAttributeSectionStyle">
                            <div>
                                <div class="overrideStatusRow">
                                    <div class="overrideStatusColumn overrideStatusAttrName"></div>
                                    <div class="overrideStatusColumn overrideStatusAttrExpiryDateTitle">EXPIRES ON</div>
                                </div>
                            </div>
                            <div>
                                <div class="overrideStatusRow">
                                    <div id="overrideStatusAttrDropDown" class="overrideStatusColumn overrideStatusAttrName"></div>
                                    <div id="overrideStatusAttrDate" class="overrideStatusColumn overrideStatusAttrExpiryDate"></div>
                                    <div class="overrideStatusColumn overrideStatusAddIcon" data-bind="click: onClickAddRow"><i class="fa fa-plus-circle"></i></div>
                                    <div id="overrideErrorId" style="display:none"></div>
                                </div>
                            </div>
                            <div id="overrideStatusAttributeListSection" class="overrideStatusAttrListStyle">
                                <div data-bind="foreach: attributeList" class="overrideStatusAttrSlimscroll">
                                    <div class="overrideStatusRow">
                                        <div data-bind="text: attributeName" class="overrideStatusColumn overrideStatusAttrName overrideStatusAttrColumn"></div>
                                        <div data-bind="text: attributeExpiry" class="overrideStatusColumn overrideStatusAttrExpiryDate overrideStatusAttrColumn"></div>
                                        <div class="overrideStatusColumn overrideStatusDeleteIcon" data-bind="click: onClickDeleteRow"><i class="fa fa-trash-o"></i></div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div id="overrideStatusSecurityDataGridSection">
                        <div id="overrideStatusGridTitle" class="overrideStatusTitle"></div>
                        <div id="overrideStatusSecurityDataNeoGrid">
                        </div>
                    </div>
                </div>
            </asp:Panel>
            <asp:Panel ID="panelBottom" runat="server" class="os_panelSections">
                <!--Error Popup-->
                <div id="os_messagePopup" class="overrideStatusMessagePopupStyle" style="display: none;">
                    <div class="fa fa-times overrideStatusPopupCloseBtn">
                    </div>
                    <div class="overrideStatusUpperSection">
                        <div class="overrideStatuspopupImage overrideStatusHorizontalAlign">
                            <img />
                        </div>
                        <div class="overrideStatuspopupTitle overrideStatusHorizontalAlign">
                        </div>
                    </div>
                    <div class="overrideStatusMessage">
                    </div>
                </div>
                <div id="loadingImg" class="loadingMsg" style="display: none; z-index: 10000;">
                    <asp:Image ID="Image1" runat="server" src="css/images/ajax-working.gif" />
                </div>
                <div id="disableDiv" class="alertBG" style="display: none; z-index: 9999;" align="center"></div>
            </asp:Panel>
        </div>
    </form>
</body>
</html>
