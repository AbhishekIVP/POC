<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MergeDuplicate.aspx.cs" Inherits="com.ivp.secm.ui.SMMergeDuplicate" 
    MasterPageFile="~/CommonUI/SRMMaster.Master" EnableEventValidation="false" %>

<asp:Content ID="filterContent" ContentPlaceHolderID="ContentPlaceHolderFilterSection" runat="server">

<div id="smmergeduplicateSaveFilterParent" class="smmergeduplicateHorizontalAlign" style="height: 100%; text-align: right;vertical-align: top;
    padding-top: 5px;float:right">
    <div id="smmergeduplicateSavedFilters" class=" smmergeduplicateHorizontalAlign smmergeduplicateAttributeDropDown smmergeduplicateRunningTextDropDown"
        style="border: none; margin-top: 3px;">
    </div>
    <input type="button" id="smmergeduplicateSaveBtn" class=" smmergeduplicateHorizontalAlign CommonOrangeBtnStyle smmergeduplicatePrimaryBtnStyle smmergeduplicatePrimaryBtnAsTextStyle smmergeduplicateSaveBtn"
        value="Save" />
</div>

</asp:Content>    

   
<asp:Content ID="contentMiddle" ContentPlaceHolderID="SRMContentPlaceHolderMiddle" runat="server">

    <!--CSS FILES-->
    <link href="css/MergeDuplicate.css" rel="stylesheet" type="text/css" />
    <!--JS FILES-->
    
    <script src="js/MergeDuplicate.js" type="text/javascript"></script>
   <%-- <form id="commonUIMergeDuplicatesForm" runat="server">--%>
        <!--Filter Selection Div-->
        <div id="smmergeduplicateFilterSelectionDiv" class="smmergeduplicateFilterSelectionDivStyle"
            is_expanded="true">
            <%--Dropdown Div --%>
            <div>
                <i id="smmergeduplicateRightFilterIcon" class="fa fa-caret-down smmergeduplicateRightFilterStyle"
                    style="visibility: hidden;"></i>
            </div>

            <!--End Choose Product Div-->
            <div class="smmergeduplicateFilterOuterDiv" style="margin-top:0.5px">
                <div id="smmergedFindDuplicatesContainer" class="smmergeTopSection">
                    <div class="smmergedParentSecTypeOption" style='text-align: center;top: -15px;position: relative;'>
                        <div class="smmergeduplicateText ">Find Duplicates In
                        </div>
                        <div id="smmergeduplicateSectypeDropDown" class=" smmergeduplicateAttributeDropDown smmergeduplicateRunningTextDropDown" >
                        </div>
                    </div>
                    <div class="smmergedTopFilterBackground smmergeduplicateHorizontalAlign smmergeduplicateRowStyle" >
                        <div id="smmergeduplicatePrimaryAttributeDropDown" class="smmergeduplicateHorizontalAlign smmergeduplicateAttributeDropDown">
                        </div>
                        <div id="smmergeduplicatePrimaryMatchTypeDropDown" class="smmergeduplicateHorizontalAlign smmergeduplicateAttributeDropDown">
                        </div>
                        <%-- <div class="smmergeduplicateHorizontalAlign" style="width:' + smMergeDuplicate.getInputElementWidth(isApproxMatch) + 'px;" ><input type="text" id = ' + inputWeightId + ' value="' + selectedWeight + "%" + '" class="smmergeduplicateAttributeInput smmergeduplicateFocusOut" /></div>' + toleranceHTML + '</div>--%>
                        <div class="smmergeduplicateHorizontalAlign" style="z-index:10;position:relative">
                            <input type="text" value="100%" id="smmergeduplicatePrimaryWeightText" class="smmergeduplicateAttributeInput smmergeduplicateFocusOut" />
                        </div>


                        <div id="toleranceDropDown" class='smmergeduplicateHorizontalAlign smmergeduplicateToleranceContainer' style='width: 202px; color: #9a9a9a;'>
                            <div class='smmergeduplicateHorizontalAlign smmergeduplicatePlusMinusContainer'>
                                <div class='smmergeduplicatePlusSign'>+</div>
                                <div class='smmergeduplicateMinusSign'>-</div>
                            </div>
                            <div class='smmergeduplicateHorizontalAlign'>
                                <input class='smergeduplicateToleranceInput' type='text' id="toleranceInput" value="0" /></div>
                            <div id="toleranceDropdown" class='smmergeduplicateHorizontalAlign'></div>
                        </div>
                        <div id="toleranceDropDownDummy" class='smmergeduplicateHorizontalAlign' style="background:rgb(122, 118, 118) !important;border-left: 1px solid rgb(199, 196, 196);width: 225px; margin-left: -225px;background: white;position: relative; height: 37px;opacity: 0;">
                        </div>

                    </div>
                    <div class="smmergeduplicateHorizontalAlign">
                        <div id="smmergeduplicateAddAttributeBtn" class="smmergeduplicateHorizontalAlign CommonIcon smmergeduplicateAddAttributeBtnStyle fa  fa-plus-circle"
                            style="margin-top: 10px;">
                        </div>
                    </div>
                    <div id="smmergeduplicateErrorMsgTextFirstPage" class="smmergeduplicateMsgTextStyle smmergeduplicateErrorColor" style="display:none; position: relative;width: 300px;margin: 0 auto;top: 20px;">Error!</div>
                   <%-- <div class="smmergeduplicateFilterRow smmergeduplicateAttrDivStyle">
                            <div id="smmergeduplicateErrorMsgTextFirstPage" class="smmergeduplicateMsgTextStyle smmergeduplicateErrorColor"></div>
                        </div>--%>
                </div>

                <div id="smmergedBottomDisplayPart">
                    <div id="smmergedFindDuplicatesSearchCriteriaContainer" style="display: inline-flex; /*width: 78%*/">
                        <div class="smmergeduplicateFilterRow" style="margin-top: 20px; width: 100%">
                               <div class="smmergeduplicateText smmergeduplicateMatchCriteriaHeading">
                        Match Criteria <span class="smmergeduplicateHorizontalAlign" style='margin-left: 85%;'>
                        </span>
                    </div>
                            <div id="smmergeduplicateAttributeSelectionDiv" class="smmergeduplicateAttrDivStyle">
                                <%--<div class="smmergeduplicateTextForMatch smmergeduplicateMatchCriteriaHeading">
                                    Match Criteria <span class="smmergeduplicateHorizontalAlign" style='margin-left: 85%;'></span>
                                </div>--%>
                                <div id="smmergeduplicateAttributesContainer" row_number="0">
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="smmergeduplicateFilterInnerDiv">
                        <%--<div>
                    <i id="smmergeduplicateRightFilterIcon" class="fa fa-chevron-circle-right smmergeduplicateRightFilterStyle"
                        style="visibility: hidden;"></i>
                </div>--%>

                        <%--<div class="smmergeduplicateFilterRow" style="border-bottom: 1px solid #dcdcdc; line-height: 1.5;">
                    <div id="smmergeduplicateHeading" class="smmergeduplicateHorizontalAlign" style="color: #838383;
                        font-size: 20px;">
                        SEARCH DUPLICATES</div>
                    <%--<div id="smmergeduplicateSavedFilters" class="smmergeduplicateHorizontalAlign smmergeduplicateAttributeDropDown smmergeduplicateRunningTextDropDown"
                    style="border: none; margin-top: 3px;">
                    </div>
                    <input type="button" id="smmergeduplicateSaveBtn" class=" smmergeduplicateHorizontalAlign CommonOrangeBtnStyle smmergeduplicatePrimaryBtnStyle smmergeduplicatePrimaryBtnAsTextStyle smmergeduplicateSaveBtn"
                        value="Save" />--%>
                        <%-- </div>--%>
                        <%--<div class="smmergeduplicateFilterRow" style='text-align: center;'>
                    <span class="smmergeduplicateText smmergeduplicateHorizontalAlign">Find Duplicates In
                    </span>
                    <div id="smmergeduplicateSectypeDropDown" class="smmergeduplicateHorizontalAlign smmergeduplicateAttributeDropDown smmergeduplicateRunningTextDropDown">
                    </div>
                </div>
                <div class="smmergeduplicateFilterRow" style="margin-top: 20px;">
                    <div class="smmergeduplicateText smmergeduplicateMatchCriteriaHeading">
                        Match Criteria <span class="smmergeduplicateHorizontalAlign" style='margin-left: 85%;'>
                        </span>
                    </div>
                    <div id="smmergeduplicateAttributeSelectionDiv" class="smmergeduplicateAttrDivStyle">
                        <div class="smmergeduplicateHorizontalAlign smmergeduplicateRowStyle">
                            <div id="smmergeduplicatePrimaryAttributeDropDown" class="smmergeduplicateHorizontalAlign smmergeduplicateAttributeDropDown">
                            </div>
                            <div id="smmergeduplicatePrimaryMatchTypeDropDown" class="smmergeduplicateHorizontalAlign smmergeduplicateAttributeDropDown">
                            </div>
                            <div class="smmergeduplicateHorizontalAlign">
                                <input type="text" value="100%" id="smmergeduplicatePrimaryWeightText" class="smmergeduplicateAttributeInput smmergeduplicateFocusOut" />
                            </div>
                        </div>
                        <div class="smmergeduplicateHorizontalAlign">
                            <div id="smmergeduplicateAddAttributeBtn" class="smmergeduplicateHorizontalAlign CommonIcon smmergeduplicateAddAttributeBtnStyle fa  fa-plus-circle"
                                style="margin-top: 10px;">
                            </div>
                        </div>
                        <div id="smmergeduplicateAttributesContainer" row_number="0">
                        </div>
                    </div>
                </div>--%>
                        <div class="smmergeduplicateFilterRow smmergeduplicateAttrDivStyle">
                            <div id='smmergeduplicateErrorMsgText' class="smmergeduplicateMsgTextStyle">
                            </div>
                        </div>
                        <%--<div class="smmergeduplicateFilterRow " style="margin-top: 30px;text-align: right;">
                    <div class="smmergeduplicateHorizontalAlign smmergedUnderlineChildren">
                        <p style="font-size: 14px;">
                            Show only matches above confidence
                            <input type="text" id="smmergeduplicateFilterMatchWeightText" class="smmergeduplicateAttributeInput smmergeduplicateFocusOut smmergeduplicateFinalWeightTextStyle"
                                value="40%" style="height: 22px; width: 50px;" /></p>
                    </div>
                    <div class="smmergeduplicateHorizontalAlign">
                        <input type="button" id="smmergeduplicateFindDuplicatesBtn" class="CommonGreenBtnStyle smmergeduplicatePrimaryBtnStyle"
                            value="Run" style="margin-top: 11%;" />
                    </div>
                </div>--%>
                    </div>
                    <div class="smmergeduplicateFilterRowBottom " style="text-align: right; margin-right: 7.5%;">
                        <div class="smmergeduplicateHorizontalAlign smmergedUnderlineChildren">
                            <p style="font-size: 14px;">
                                Show only matches above confidence
                            <input type="text" id="smmergeduplicateFilterMatchWeightText" class="smmergeduplicateAttributeInput smmergeduplicateFocusOut smmergeduplicateFinalWeightTextStyle"
                                value="80%" style="height: 22px; width: 50px;" />
                            </p>
                        </div>
                        <div class="smmergeduplicateHorizontalAlign">
                            <input type="button" id="smmergeduplicateFindDuplicatesBtn" class="CommonGreenBtnStyle smmergeduplicatePrimaryBtnStyle"
                                value="Run" style="margin-top: 11%;padding-left: 20px;
    padding-right: 20px;" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!--End Filter Selection Div-->
        <!--Grid Div-->
        <div id="smmergeduplicatesGrid" class="smmergeduplicateCenterAlign" style="display: none;">
            <div class="smmergeduplicateGridDivHeader">
                <!--<div class="smmergeduplicateHeading smmergeduplicateHorizontalAlign">DUPES</div>-->
                <div class="smmergeduplicateHorizontalAlign" style="margin-left: 92%;">
                    <input type="button" id="smmergeduplicateMergeBtn" value="View Dupes" class="smmergeduplicateSecondaryBtn smmergeduplicateMergeBtn CommonGreenBtnStyle" />
                </div>
            </div>
            <div id="smmergeduplicateGridsContainer">
            </div>
        </div>
        <!--End Grid Div-->
        <!--Merge Dupes Div-->
        <div id="smmergeduplicateMergeDupesDivContainer" class="smmergeduplicateMergeDupesDivContainerStyle smmergeduplicateCenterAlign smmergeduplicateDetailsGrid">
            <input type="button" id="smmergeduplicateMergeDupesOption" class="smmergeduplicateHorizontalAlign CommonGreenBtnStyle smmergeduplicatePrimaryBtn smmergeduplicateMergeSecMergeBtn"
                value="Merge" style="display: none;" />
            <div id="smmergeduplicateMergeDupesCloseBtn" class="smmergeduplicateHorizontalAlign smmergeduplicateMergeDuplicateCloseBtn">
                <i class="fa fa-times"></i>
            </div>
            <div class="smmergeduplicateMergeDupesDiv">
            </div>
            <div id='smmergeduplicateMergeOptionContainer' class='smmergeduplicateMergeOptionContainerStyle'
                style="display: none;">
                <div>
                    <div class='smmergeduplicateCheckboxContainer'>
                        <input type='checkbox' id='smmergeduplicateDeleteOriginalCheckbox' name='smmergeduplicateDeleteOriginalChkbox'>
                        <div class='smmergeduplicateHorizontalAlign smmergeduplicateCheckBoxText'>
                            Delete Original
                        </div>
                    </div>
                    <div class='smmergeduplicateCheckboxContainer'>
                        <input type='checkbox' id='smmergeduplicateCopyTimeseriesCheckbox' name='smmergeduplicateCopyTimeseriesChkbox'>
                        <div class='smmergeduplicateHorizontalAlign smmergeduplicateCheckBoxText'>
                            Copy Timeseries
                        </div>
                    </div>
                </div>
                <div class='smmergeduplicateMergeOptionRow'>
                    <div class='smmergeduplicateHorizontalAlign smmergeduplicateMergeOptionLeftText'>
                        Merge In
                    </div>
                    <div id='smmergeduplicateMergeOptionSecurityDdn' class='smmergeduplicateHorizontalAlign'>
                    </div>
                </div>
                <div class='smmergeduplicateOrOption'>
                    OR
                </div>
                <div class='smmergeduplicateMergeOptionRow'>
                    <div class='smmergeduplicateHorizontalAlign smmergeduplicateMergeOptionLeftText'>
                        Create New in
                    </div>
                    <div id='smmergeduplicateMergeOptionSectypeDdn' class='smmergeduplicateHorizontalAlign'>
                    </div>
                </div>
                <div class='smmergeduplicateErrorDiv smmergeduplicateMergeOptionLeftText'>
                </div>
                <input type='button' class='CommonGreenBtnStyle smmergeduplicateMergeOptionOkBtn fa fa-arrow-circle-right'
                    value='Ok' id='smmergeduplicateMergeOptionOkBtn' />
                <div id="smmergeduplicateLegsList" class="smmergeduplicateLegsListStyle" style="display: none;">
                </div>
            </div>
            <div id='divDeleteEntities' class='smmergeduplicateDeleteOptionsContainer' style="display: none;">
                <div>
                    <div class='smmergeduplicateCheckboxContainer'>
                        <div id="divCloseDeleteOptions" class="smmergeduplicateHorizontalAlign smDeleteOptionsCloseBtn">
                            <i class="fa fa-times"></i>
                        </div>
                        <div style="position: relative; top: 4px;">
                            One or more entities to be deleted are being referenced. Select an appropriate option
                        before proceeding.
                        </div>
                        </br>
                    <div>
                        <!--<div>
                            <input type='radio' id='rdReplaceReferences_1' name='smmergeduplicateDeleteEntityOptions'
                                checked='checked'>Delete entities and replace references</div>
                        </br>-->
                        <div>
                            <input type='radio' id='rdDontReplaceReferences_2' name='smmergeduplicateDeleteEntityOptions'>Delete
                            entities and remove references
                        </div>
                        </br>
                        <div>
                            <input type='radio' id='rdDontDeleteEntities_3' name='smmergeduplicateDeleteEntityOptions'>Do
                            not delete entities
                        </div>
                    </div>
                        <input type='button' class='CommonGreenBtnStyle fa fa-arrow-circle-right' style="margin-left: 300px;"
                            value='Ok' id='btnDeleteOptions' />
                    </div>
                </div>
            </div>
        </div>
        <!--End Merge Dupes Div-->

        <!--Delete Security Popup-->
        <div id="smmergeduplicateDeleteSecurityPopup" class="smmergeduplicateDeleteSecurityPopupStyle" style="display: none;">
            <div id="smmergeduplicateDeleteSecurityPopupCloseBtn" class="smmergeduplicateDeleteSecurityPopupCloseBtnStyle">
                <i class="fa fa-times"></i>
            </div>
            <iframe id="smmergeduplicateDeleteSecurityPopupIframe" class="smmergeduplicateDeleteSecurityPopupIframeStyle" src=""></iframe>
        </div>
        <!--Delete Security Popup End-->

        <div id="loadingImg" class="loadingMsg" style="display: none; z-index: 10000;">
            <asp:Image ID="Image1" runat="server" src="css/images/ajax-working.gif" />
        </div>
        <div id="disableDiv" class="alertBG" style="display: none; z-index: 9999;" align="center">
        </div>
        <!--Hidden Fields-->
        <input type="hidden" id="hdnUsername" runat="server" />
        <input type="hidden" id="hdnSelectedFilter" runat="server" value="0" />
        <script src="includes/left_menu.js" type="text/javascript"></script>
        <script src="includes/RScriptUtils.debug.js" type="text/javascript"></script>
        <script src="includes/RefMasterUpgradeScript.js" type="text/javascript"></script>
        <script src="includes/script.js" type="text/javascript"></script>
        <script src="ScriptIncludes/SecMasterJSCommon.debug.js" type="text/javascript"></script>
        <script src="ScriptIncludes/RefMasterJSCommon.debug.js" type="text/javascript"></script>
        <script src="includes/SecMasterScripts.js" type="text/javascript"></script>
        <script src="includes/updatePanel.js" type="text/javascript"></script>
        <script src="includes/jquery.slimscroll1.js" type="text/javascript"></script>
        <script src="includes/jquery.slimscrollHorizontal.js" type="text/javascript"></script>
        <script src="includes/SMSlimscroll.js" type="text/javascript"></script>
        <script src="includes/SMSelect.js" type="text/javascript"></script>
        <script src="js/MergeDuplicate.js" type="text/javascript"></script>
    <%--</form>--%>

</asp:Content>
