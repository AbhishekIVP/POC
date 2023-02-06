<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMLandingPage.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMLandingPage"
    MasterPageFile="~/CommonUI/SRMMaster.Master" EnableEventValidation="false" %>


<asp:Content ID="contentMiddle" ContentPlaceHolderID="SRMContentPlaceHolderMiddle" runat="server">

    <script src="includes/SRMLandingPage.js" type="text/javascript"></script>
    <link href="css/SRMLandingPage.css" rel="stylesheet" />

    <div id="SRMLandingPageParent">
        <div class="SRMLandingPageSearchTextParent">
            <div id="SRMLandingPageSearchText">
                <i class="fa fa-search" id="SRMLandingPageSearchIcon"></i>
                <input id="SRMLandingPageSearchTextInputBox" data-bind="textinput: searchText" placeholder="Enter search text" />
                
            </div>
        </div>

        <div class="SRMLandingPageListParent" id="SRMLandingPageListParent">
            <div data-bind="foreach: searchResult" class="SRMLandingPageList" id="SRMLandingPageList">
                <div class="SRMLandingPageListItem" data-bind="attr: { id: 'SRMLandingPageListItem_' + id }">
                    <div class="SRMLandingPageLeftHalf">
                        <div class="SRMLandingPageUpperHalf SRMLandingPageHalves">
                            <div data-bind="text: name, click: $parent.clickedType, attr: { 'tabidentifier': 'typeId', title: name }" class="SRMLandingPageTypeName SRMCLP_Ellipses"></div>
                        </div>
                        <div class="SRMLandingPageLowerHalf SRMLandingPageHalves">
                            <div class="SRMLandingPageItemLeft">
                                <div class="SRMLandingPageCountItemLeft SRMLandingPageUpstreamItem">
                                    <div class="SRMLandingPageCountItemCommonLeft">
                                        <img src="images/icons/wf_upstream.png" />
                                    </div>
                                    <div class="SRMLandingPageCountItemNameLeft SRMLandingPageCountItemCommonLeft">Upstream</div>
                                    <div class="SRMLandingPageCountItemValueLeft SRMLandingPageCountItemCommonLeft" data-bind="text: upstreamCount, click: $parent.clickedType, attr: { 'tabidentifier': 'upstream' }"></div>
                                </div>
                                <div class="SRMLandingPageCountItemLeft SRMLandingPageDownstreamItem">
                                    <div class="SRMLandingPageCountItemCommonLeft">
                                        <img src="images/icons/wf_downstream.png" />
                                    </div>
                                    <div class="SRMLandingPageCountItemNameLeft SRMLandingPageCountItemCommonLeft">Downstream</div>
                                    <div class="SRMLandingPageCountItemValueLeft SRMLandingPageCountItemCommonLeft" data-bind="text: downstreamCount, click: $parent.clickedType, attr: { 'tabidentifier': 'downstream' }"></div>
                                </div>
                            </div>
                        </div>
                    </div>


                    <div class="SRMLandingPageRightHalf">
                        <div class="SRMLandingPageItemRight">
                            <div class="SRMLandingPageCountItemRight SRMLandingPageAttrItem">
                                <div class="SRMLandingPageCountItemNameRight SRMLandingPageCountItemCommonRight">Attributes</div>
                                <div class="SRMLandingPageCountItemValueRight SRMLandingPageCountItemCommonRight" data-bind="text: attrCount, click: $parent.clickedType, attr: { 'tabidentifier': 'attribute' }"></div>
                            </div>
                            <div class="SRMLandingPageCountItemRight SRMLandingPageLegItem">
                                <div class="SRMLandingPageCountItemNameRight SRMLandingPageCountItemCommonRight">Legs</div>
                                <div class="SRMLandingPageCountItemValueRight SRMLandingPageCountItemCommonRight" data-bind="text: legCount, click: $parent.clickedType, attr: { 'tabidentifier': 'leg' }"></div>
                            </div>
                        </div>

                    </div>



                    <%--                    <div class="SRMLandingPageLowerHalf SRMLandingPageHalves">
                        <div class="SRMLandingPageItemUpperHalf SRMLandingPageItem">
                            <div class="SRMLandingPageCountItem SRMLandingPageAttrItem">
                                <div class="SRMLandingPageCountItemIcon SRMLandingPageCountItemCommon">
                                    <img src="images/icons/attributes.png" />
                                </div>
                                <div class="SRMLandingPageCountItemName SRMLandingPageCountItemCommon">Attributes</div>
                                <div class="SRMLandingPageCountItemValue SRMLandingPageCountItemCommon" data-bind="text: attrCount, click: $parent.clickedType, attr: {'tabidentifier':'attribute'}"></div>
                            </div>
                            <div class="SRMLandingPageCountItem SRMLandingPageLegItem">
                                <div class="SRMLandingPageCountItemIcon SRMLandingPageCountItemCommon">
                                    <img src="images/icons/leg.png" />
                                </div>
                                <div class="SRMLandingPageCountItemName SRMLandingPageCountItemCommon">Legs</div>
                                <div class="SRMLandingPageCountItemValue SRMLandingPageCountItemCommon" data-bind="text: legCount, click: $parent.clickedType, attr: { 'tabidentifier': 'leg' }"></div>
                            </div>
                        </div>
                        <div class="SRMLandingPageItemLowerHalf SRMLandingPageItem">
                            <div class="SRMLandingPageCountItem SRMLandingPageUpstreamItem">
                                <div class="SRMLandingPageCountItemIcon SRMLandingPageCountItemCommon">
                                    <img src="images/icons/upstream.jpg" />
                                </div>
                                <div class="SRMLandingPageCountItemName SRMLandingPageCountItemCommon">Upstream</div>
                                <div class="SRMLandingPageCountItemValue SRMLandingPageCountItemCommon" data-bind="text: upstreamCount, click: $parent.clickedType, attr: { 'tabidentifier': 'upstream' }"></div>
                            </div>
                            <div class="SRMLandingPageCountItem SRMLandingPageDownstreamItem">
                                <div class="SRMLandingPageCountItemIcon SRMLandingPageCountItemCommon">
                                    <img src="images/icons/downstream.jpg" />
                                </div>
                                <div class="SRMLandingPageCountItemName SRMLandingPageCountItemCommon">Downstream</div>
                                <div class="SRMLandingPageCountItemValue SRMLandingPageCountItemCommon" data-bind="text: downstreamCount, click: $parent.clickedType, attr: { 'tabidentifier': 'downstream' }"></div>
                            </div>
                        </div>
                    </div>--%>
                </div>
            </div>
        </div>
        <div class="SRMLandingPageNoResult" id="SRMLandingPageNoResult">
        </div>
    </div>


    <div id="SRMLandingPageModules" class="SRMLandingPageModules">
        <div id="SRMTabNamesHeader" class="SRMTabNamesHeader">
            <div id="SRMTypeHeaderText" class="SRMTypeHeaderText"></div>
            <div class="SRMTabDiv">
                <div class="SRMTabItem" id="SRMRoles" style="visibility: hidden;">Roles</div>
                <div class="SRMTabItem" id="SRMTabAttributes">Attributes</div>
                <div class="SRMTabItem" id="SRMTabLegs">Legs</div>
                <div class="SRMTabItem" id="SRMTabMappings">Mappings</div>
                <div class="SRMTabItem" id="SRMTabRules">Rules</div>
                <div class="SRMTabItem" id="SRMTabAccess">Access</div>
                <div class="SRMTabItem" id="SRMTabAssociation" style="visibility: hidden;">Association</div>
            </div>
            <div id="SRMLandingPageCloseButton" class="SRMLandingPageCloseButton">
                <i class="fa fa-close"></i>
            </div>
        </div>
        <div id="iFrameContainer">
            <iframe src="" name="Iframe" class="InnerFrame" id="InnerFrameId1" style="border: none"></iframe>
        </div>
    </div>


</asp:Content>



