<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMOverridesStatus.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMOverridesStatus" 
    MasterPageFile="~/CommonUI/SRMMaster.Master" EnableEventValidation="false" %>


<asp:Content ID="contentMiddle" ContentPlaceHolderID="SRMContentPlaceHolderMiddle" runat="server">
    <script src="js/OverridesStatus.js" type="text/javascript"></script>
    <link href="css/overrideStatus.css" rel="stylesheet" type="text/css" />
     <link href="css/thirdparty/bootstrap.min.css" rel="stylesheet" type="text/css" />
  
    <div id="overrideStatusScreenSection">
                    <div id="overrideStatusActionSection" class="overrideStatusActionSectionStyle">
                        <div id="overrideStatusActionList" class="overrideStatusActionListStyle">
                            <div id="overrideStatusDeleteAction" class="overrideStatusAction overrideStatusDisabled">Delete</div>
                            <div id="overrideStatusRefreshButton" class="overrideStatusRefreshButtonParent">
                                <i class="fa fa-refresh btnClassHeader overrideStatusRefreshButtonDiv"></i>
                            </div>
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
</asp:Content>



<asp:Content ID="contentBottom" ContentPlaceHolderID="SRMContentPlaceHolderBottom" runat="server">

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

 </asp:Content>