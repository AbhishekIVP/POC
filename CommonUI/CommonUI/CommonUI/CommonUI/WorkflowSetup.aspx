<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkflowSetup.aspx.cs"
    Inherits="Reconciliation.CommonUI.WorkflowSetup" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <!--CSS FILES-->
    <link href="css/thirdparty/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="css/thirdparty/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="css/SMCss/SMSelect.css" rel="stylesheet" type="text/css" />
    <link href="css/CommonModuleTabs.css" rel="stylesheet" type="text/css" />
    <link href="css/Z_IagoCommonThemes.css" rel="stylesheet" type="text/css" />
    <link href="css/Customjquery.datetimepicker.css" rel="stylesheet" type="text/css" />
    <link href="css/jquery.datetimepicker.css" rel="stylesheet" type="text/css" />
    <link href="css/RADDateTimePicker.css" rel="stylesheet" type="text/css" />
    <link href="css/WorkflowSetup.css" rel="stylesheet" type="text/css" />
    <link href="css/AquaStyles.css" rel="stylesheet" type="text/css" />
    <!--JS FILES-->
    <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
    <script src="js/thirdparty/jquery-ui.min.js" type="text/javascript"></script>
    <script src="js/thirdparty/bluebird.min.js" type="text/javascript"></script>
    <script src="includes/bootstrap2-toggle.js" type="text/javascript"></script>
    <script src="js/thirdparty/bootstrap.min.js" type="text/javascript"></script>
    <script src="includes/MicrosoftAjax.js" type="text/javascript"></script>
    <script src="js/thirdparty/knockout-3.4.0.js" type="text/javascript"></script>
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
    <script src="includes/SMSlimscroll.js" type="text/javascript"></script>
    <script src="js/CommonUtils.js" type="text/javascript"></script>
    <script src="js/WorkflowSetup.js" type="text/javascript"></script>
</head>
<body>    
    <form id="commonUIWorkflowSetupForm" runat="server">
        <div id="workflowSetup_container" class="container-fluid workflowSetup_containerStyle">
            <!--Workflow List Screen-->
            <div id="workflowSetup_chooseWorkflowScreen" class="container-fluid" data-bind="css: (workflowList().length > 0) ? 'workflowSetup_chooseWorkflowScreenStyleUpdate' : 'workflowSetup_chooseWorkflowScreenStyleCreate'">
                <div class="row" style="margin: 0px;">
                    <div class="col-sm-12">
                        <div class="row">
                            <div class="col-sm-offset-4 col-sm-4">
                                <div id="workflowSetup_pageText" class="workflowSetup_pageTextStyle">
                                </div>
                            </div>
                            <div class="col-sm-offset-3 col-sm-1">
                                <div class="workflowSetup_sideAddBtn" data-bind="text: (workflowList().length > 0) ? 'Add Workflow' : '', click: (workflowList().length > 0) ? onClickAddNewWorkflow : ''">
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="row">
                                    <div class="col-sm-offset-4 col-sm-4">
                                        <!--Workflow List-->
                                        <div id="workflowSetup_workflowList" data-bind="foreach: workflowList" style="margin-top: 30px;">
                                            <div class="row workflowSetup_updateWorkflowSetupStyle" data-bind="click: onClickUpdateWorkflow, event: { mouseover: onHoverInTile, mouseleave: onHoverOutTile }">
                                                <div class="col-sm-10">
                                                    <div class="workflowSetup_horizontalAlign workflowSetup_updateWorkflowNameTextBox"
                                                        contenteditable data-bind="text: workflowName">
                                                    </div>
                                                    <div class="workflowSetup_horizontalAlign workflowSetup_editIcon" data-bind="click: onClickEditWorkflowName"
                                                        style="color: #fff;">
                                                        <i class="fa fa-pencil-square-o"></i>
                                                    </div>
                                                    <div data-bind="with: workflowSetupInstance" class="workflowSetup_sectypeNames">
                                                        <div class="workflowSetup_horizontalAlign workflowSetup_tileSubtitle" data-bind="text: ($parent.moduleType() === 0 ? sectypesName : entityTypeName)">
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="col-sm-offset-1 col-sm-1" style="padding: 0px;">
                                                    <div class="workflowSetup_tileArrow" style="display: none;">
                                                        <i class="fa fa-arrow-right"></i>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div id="workflowSetup_addNewWorkflow" class="row" data-bind="style: { display: (workflowList().length === 0) ? '' : 'none' }">
                                <div class="col-sm-12">
                                    <div class="row">
                                        <div class="col-sm-offset-4 col-sm-4">
                                            <!--No Workflow is present-->
                                            <div id="workflowSetup_newWorkflowSetup" class="">
                                                <div class="col-sm-offset-2 col-sm-8">
                                                    <input id="workflowSetup_newWorkflowName" class="workflowSetup_horizontalAlign workflowSetup_newWorkflowNameTextBox"
                                                        placeholder="Enter Workflow Name" />
                                                    <div class="workflowSetup_horizontalAlign" data-bind="visible: (showRefWorkflow() === true)">
                                                        <div class="workflowSetup_newWorkflowRow" data-bind="visible: !(isStandAloneRef() === true)">
                                                            <div class="workflowSetup_horizontalAlign workflowSetup_moduleTitle">Module</div>
                                                            <div id="workflowSetup_moduleSelector" module_type="0" data-bind="click: onClickChooseModule" class="workflowSetup_moduleContainer workflowSetup_horizontalAlign">
                                                                <div class="workflowSetup_horizontalAlign workflowSetup_moduleItems workflowSetup_moduleSelected">SecMaster</div>
                                                                <div class="workflowSetup_horizontalAlign workflowSetup_moduleItems">RefMaster</div>
                                                            </div>
                                                        </div>
                                                        <div class="workflowSetup_newWorkflowRow">
                                                            <div class="workflowSetup_horizontalAlign workflowSetup_moduleTitle">Level</div>
                                                            <div id="workflowSetup_workflowTypesDropdown" class="workflowSetup_horizontalAlign workflowSetup_workflowTypesDropdownStyle"></div>
                                                            <div id="workflowSetup_addBtn" class="workflowSetup_addWorkflowBtnStyle" data-bind="click: onClickCreateWorkflow">ADD</div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div id="workflowSetup_errorMsgDiv" class="workflowSetup_errorMsgDivStyle">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!--Attribute Configuration Section-->
            <div id="workflowSetup_attributeContainerScreen" class="container-fluid" style="display: none;">
                <div class="row workflowSetup_topOptions" data-bind="with: workflowSetupInstance">
                    <div class="workflowSetup_horizontalAlign workflowSetup_topSectionOptionsContainer">
                        <div data-bind="css: (wType().toLowerCase() === 'asset level') ? 'col-sm-offset-5 col-sm-2' : 'col-sm-2'">
                            <div class="workflowSetup_sectionTitle">
                                Workflow
                            </div>
                            <div id="workflowSetup_workflowInstanceDropDown" class="workflowSetup_workflowName">
                            </div>
                        </div>
                        <div class="col-sm-2">
                            <div class="workflowSetup_sectionTitle" data-bind="text: assetTypeText"></div>
                            <div data-bind="visible: (mType() === 0)" id="workflowSetup_sectypeDropDown"></div>
                            <div data-bind="visible: (mType() === 1)" id="workflowSetup_entityTypeDropDown"></div>
                        </div>
                        <div class="col-sm-2" data-bind="visible: (wType().toLowerCase().indexOf('attribute') > -1)">
                            <div class="workflowSetup_checkbox">
                                <div class="workflowSetup_checkboxLabel">
                                    Apply on Create
                                </div>
                                <input id="workflowSetup_applyWorkflowOnCreate" type="checkbox" data-bind="checked: applyOnCreate">
                            </div>
                        </div>
                        <div class="col-sm-2"  data-bind="visible: (wType().toLowerCase().indexOf('attribute') > -1)" >
                            <div class="workflowSetup_checkbox">
                                <div class="workflowSetup_checkboxLabel">
                                    Apply on Time Series Update
                                </div>
                                <input id="workflowSetup_applyTimeSeries" type="checkbox" data-bind="checked: applyTimeSeries">
                            </div>
                        </div>
                        <div class="col-sm-3" data-bind="visible: (wType().toLowerCase().indexOf('attribute') > -1)">
                            <div class="workflowSetup_checkbox">
                                <div class="workflowSetup_checkboxLabel">
                                    Apply on Blank Attribute Enrichment
                                </div>
                                <input id="workflowSetup_applyBlankValues" type="checkbox" data-bind="checked: applyOnBlankToNonblank">
                            </div>
                        </div>
                    </div>
                    <div class="workflowSetup_horizontalAlign">
                        <div class="col-sm-2 workflowSetup_SaveBtn">
                            <div id="workflowSetup_saveBtn" class="workflowSetupAction" data-bind="text: workflowMode, click: (workflowMode() === 'Save') ? onClickSaveWorkflow : onClickUpdateWorkflowSetup">
                            </div>
                        </div>
                    </div>
                    <div class="workflowSetup_closeBtn">
                        <div data-bind="click: onClickClose">
                            <i class="fa fa-times"></i>
                        </div>
                    </div>
                </div>
                <div class="row workflowSetup_attributeContainerStyle" id="workflowSetup_attributeContainerSection"
                    data-bind="with: workflowSetupInstance">
                    <div class="col-sm-3 workflowSetup_attributeColumn" data-bind="visible: (wType().toLowerCase().indexOf('attribute') > -1) || (wType().toLowerCase().indexOf('leg') > -1)" >
                        <div class="row workflowSetup_extraMargin">
                            <div class="col-sm-12">
                                <div class="form-group workflowSetup_searchBoxContainerStyle">
                                    <i class="fa fa-search"></i>
                                    <input type="text" data-bind="event: { keyup: onKeyPressAttributeSearchBox }, attr: { placeholder : ( (wType().toLowerCase() === 'attribute' || wType().toLowerCase() === 'attribute level') ? 'Attributes' : 'Legs') }" id="workflowSetup_attributeSearchBox"
                                        class="workflowSetup_attributeSearchBoxStyle" />
                                </div>
                            </div>
                        </div>
                        <div class="row workflowSetup_extraMargin workflowSetup_attrList">
                            <div class="row workflowSetup_selectedAttr" data-bind="style: { display: (selectedAttrSearchResults().length > 0) ? '' : 'none' }">
                                <div class="col-sm-12">
                                    <div class="row" data-bind="visible: !((mType() === 1) && (wType() === 'attribute'))">
                                        <div class="col-sm-12">
                                            <div class="workflowSetup_attributeSectionTitle" style="margin-left: 10px;" data-bind="text : ((wType().toLowerCase() === 'attribute' || wType().toLowerCase() === 'attribute level') ? 'SELECTED ATTRIBUTES' : 'SELECTED LEGS')">
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row workflowSetup_selectedAttributesSectionStyle" id="workflowSetup_selectedAttributesSection">
                                        <div class="col-sm-12">
                                            <div data-bind="foreach: selectedAttrSearchResults" style="width: 90%;">
                                                <div class="workflowSetup_attributeNameStyle workflowSetup_attributeNameSeclectedStyle">
                                                    <div data-bind="text: attributeName, attr: { 'attr_id': attributeID }" class="workflowSetup_horizontalAlign workflowSetup_attributeName">
                                                    </div>
                                                    <div class="workflowSetup_horizontalAlign workflowSetup_attributeRemoveIcon" data-bind="click: onClickRemoveSelectedAttribute, visible: !(($parent.mType() === 1) && ($parent.wType() === 'attribute'))">
                                                        <i class="fa fa-trash-o"></i>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row" data-bind="visible: !((mType() === 1) && (wType() === 'attribute'))">
                                <div class="col-sm-12">
                                    <div class="workflowSetup_attributeSectionTitle" style="margin-left: 10px;" data-bind="text : ((wType().toLowerCase() === 'attribute' || wType().toLowerCase() === 'attribute level') ? 'AVAILABLE ATTRIBUTES' : 'AVAILABLE LEGS')" >
                                    </div>
                                </div>
                            </div>
                            <div class="row" data-bind="visible: !((mType() === 1) && (wType() === 'attribute'))">
                                <div class="col-sm-12">
                                    <div id="workflowSetup_attributesSection" data-bind="foreach: attrSearchResults">
                                        <div class="workflowSetup_attributeNameStyle">
                                            <div data-bind="text: attributeName, click: onClickAttribute, attr: { 'attr_id': attributeID }"
                                                class="workflowSetup_horizontalAlign workflowSetup_attributeName">
                                            </div>
                                            <div class="workflowSetup_horizontalAlign workflowSetup_attributeArrow">
                                                <i class="fa fa-caret-right" aria-hidden="true"></i>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div data-bind="css : ((wType().toLowerCase() === 'asset level') ? 'col-sm-12' : 'col-sm-9')" >
                        <div class="row">
                            <div id="workflowSetup_attributeConfigSection" class="col-sm-12">
                                <div class="workflowSetup_attributeConfigSectionStyle">
                                    <div class="row">
                                        <div class="col-sm-12">
                                            <div class="workflowSetup_toggleBtn" data-bind="click: onClickUserGroupToggle">
                                                <div class="workflowSetup_horizontalAlign workflowSetup_toggleSection workflowSetup_toggleSelected">
                                                    Group
                                                </div>
                                                <div class="workflowSetup_horizontalAlign workflowSetup_toggleSection">
                                                    User
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-offset-2 col-sm-8">
                                            <div class="workflowSetup_attributesConfiguration">
                                                <div style="display: none;" class="workflowSetup_userSection">
                                                    <div class="form-group workflowSetup_userGroupSearchBoxContainer">
                                                        <i class="fa fa-search"></i>
                                                        <input type="text" placeholder="Search Users" data-bind="event: { 'keyup': onKeyPressUserSearchBox }"
                                                            class="workflowSetup_userGroupSearchBox" />
                                                    </div>
                                                    <div class="workflowSetup_userGroupTitle">
                                                        LEVELS
                                                    </div>
                                                    <div id="workflowSetup_userSelectedResults" data-bind="foreach: userSelectedSortedResults"
                                                        class="workflowSetup_userSearchResults">
                                                        <div class="workflowSetup_userGroupRow" data-bind="css: { workflowSetup_userGroupRowAlternate: $index() % 2 }">
                                                            <div class="workflowSetup_horizontalAlign workflowSetup_userNameStyle" data-bind="text: userName">
                                                            </div>
                                                            <div data-bind="foreach: levelList" class="workflowSetup_horizontalAlign">
                                                                <div class="workflowSetup_horizontalAlign workflowSetup_singleLevel" data-bind="text: levelNo, click: function () { $parent.onClickSelectedWorkflowLevel($parent, event, $data) }, css: { workflowSetup_levelOneColor: isLevelOneSelected(levelNo), workflowSetup_levelTwoColor: isLevelTwoSelected(levelNo), workflowSetup_levelThreeColor: isLevelThreeSelected(levelNo), workflowSetup_levelFourColor: isLevelFourSelected(levelNo), workflowSetup_levelFiveColor: isLevelFiveSelected(levelNo) }">
                                                                </div>
                                                            </div>
                                                            <div class="workflowSetup_horizontalAlign workflowSetup_addLevel" data-bind="click: function () { onClickAddLevel($parent, event, $data) }">
                                                                <i class="fa fa-plus" aria-hidden="true"></i>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div id="workflowSetup_userSearchResults" data-bind="foreach: userSearchResults"
                                                        class="workflowSetup_userSearchResults">
                                                        <div class="workflowSetup_userGroupRow" data-bind="css: { workflowSetup_userGroupRowAlternate: $index() % 2 }">
                                                            <div class="workflowSetup_horizontalAlign workflowSetup_userNameStyle" data-bind="text: userName">
                                                            </div>
                                                            <div data-bind="foreach: levelList" class="workflowSetup_horizontalAlign">
                                                                <div class="workflowSetup_horizontalAlign workflowSetup_singleLevel" data-bind="text: levelNo, click: function () { $parent.onClickWorkflowLevel($parent, event, $data) }, css: { workflowSetup_levelOneColor: isLevelOneSelected(levelNo), workflowSetup_levelTwoColor: isLevelTwoSelected(levelNo), workflowSetup_levelThreeColor: isLevelThreeSelected(levelNo), workflowSetup_levelFourColor: isLevelFourSelected(levelNo), workflowSetup_levelFiveColor: isLevelFiveSelected(levelNo) }">
                                                                </div>
                                                            </div>
                                                            <div class="workflowSetup_horizontalAlign workflowSetup_addLevel" data-bind="click: function () { onClickAddLevel($parent, event, $data) }">
                                                                <i class="fa fa-plus" aria-hidden="true"></i>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="workflowSetup_groupSection">
                                                    <div class="form-group workflowSetup_userGroupSearchBoxContainer">
                                                        <i class="fa fa-search"></i>
                                                        <input type="text" placeholder="Search Groups" data-bind="event: { 'keyup': onKeyPressGroupSearchBox }"
                                                            class="workflowSetup_userGroupSearchBox" />
                                                    </div>
                                                    <div class="workflowSetup_userGroupTitle">
                                                        LEVELS
                                                    </div>
                                                    <div id="workflowSetup_groupSelectedResults" data-bind="foreach: groupSelectedSortedResults  "
                                                        class="workflowSetup_groupSearchResults">
                                                        <div class="workflowSetup_userGroupRow" data-bind="css: { workflowSetup_userGroupRowAlternate: $index() % 2 }">
                                                            <div class="workflowSetup_horizontalAlign workflowSetup_userNameStyle" data-bind="text: groupName">
                                                            </div>
                                                            <div data-bind="foreach: levelList" class="workflowSetup_horizontalAlign">
                                                                <div class="workflowSetup_horizontalAlign workflowSetup_singleLevel" data-bind="text: levelNo, click: function () { $parent.onClickSelectedWorkflowLevel($parent, event, $data) }, css: { workflowSetup_levelOneColor: isLevelOneSelected(levelNo), workflowSetup_levelTwoColor: isLevelTwoSelected(levelNo), workflowSetup_levelThreeColor: isLevelThreeSelected(levelNo), workflowSetup_levelFourColor: isLevelFourSelected(levelNo), workflowSetup_levelFiveColor: isLevelFiveSelected(levelNo) }">
                                                                </div>
                                                            </div>
                                                            <div class="workflowSetup_horizontalAlign workflowSetup_addLevel" data-bind="click: function () { onClickAddLevel($parent, event, $data) }">
                                                                <i class="fa fa-plus" aria-hidden="true"></i>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div id="workflowSetup_groupSearchResults" data-bind="foreach: groupSearchResults"
                                                        class="workflowSetup_groupSearchResults">
                                                        <div class="workflowSetup_userGroupRow" data-bind="css: { workflowSetup_userGroupRowAlternate: $index() % 2 }">
                                                            <div class="workflowSetup_horizontalAlign workflowSetup_userNameStyle" data-bind="text: groupName">
                                                            </div>
                                                            <div data-bind="foreach: levelList" class="workflowSetup_horizontalAlign">
                                                                <div class="workflowSetup_horizontalAlign workflowSetup_singleLevel" data-bind="text: levelNo, click: function () { $parent.onClickWorkflowLevel($parent, event, $data) }, css: { workflowSetup_levelOneColor: isLevelOneSelected(levelNo), workflowSetup_levelTwoColor: isLevelTwoSelected(levelNo), workflowSetup_levelThreeColor: isLevelThreeSelected(levelNo), workflowSetup_levelFourColor: isLevelFourSelected(levelNo), workflowSetup_levelFiveColor: isLevelFiveSelected(levelNo) }">
                                                                </div>
                                                            </div>
                                                            <div class="workflowSetup_horizontalAlign workflowSetup_addLevel" data-bind="click: function () { onClickAddLevel($parent, event, $data) }">
                                                                <i class="fa fa-plus" aria-hidden="true"></i>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!--Error Popup-->
            <div id="workflowSetup_messagePopup" class="row" data-bind="with: errorPopup" style="display: none;">
                <div class="col-sm-offset-4 col-sm-4 workflowSetup_popupContainer" data-bind="style: { borderTopColor: borderColor(), borderTopWidth: '4px', borderTopStyle: 'solid' }">
                    <div class="row">
                        <div class="col-sm-offset-11 col-sm-1">
                            <div class="fa fa-times workflowSetup_popupCloseBtn" data-bind="click: onClickClosePopup">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-2">
                            <img data-bind="attr: { src: icon() }" />
                        </div>
                        <div class="col-sm-10">
                            <div class="workflowSetup_popupTitle" data-bind="text: title, style: { color: borderColor() }">
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-offset-2 col-sm-10">
                            <div class="workflowSetup_popupMessage" data-bind="text: message">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div id="loadingImg" class="loadingMsg" style="display: none; z-index: 10000;">
            <asp:Image ID="Image1" runat="server" src="css/images/ajax-working.gif" />
        </div>
        <div id="disableDiv" class="alertBG" style="display: none; z-index: 9999;" align="center">
        </div>
    </form>
</body>
</html>
