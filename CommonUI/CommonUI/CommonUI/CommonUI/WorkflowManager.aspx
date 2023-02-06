<%@ page language="C#" autoeventwireup="true" codebehind="WorkflowManager.aspx.cs" inherits="com.ivp.common.CommonUI.CommonUI.WorkflowManager" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link href="css/thirdparty/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link href="css/thirdparty/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="css/SMCss/SMSelect.css" rel="stylesheet" type="text/css" />
    <link href="css/WorkflowManager.css" rel="stylesheet" type="text/css" />
    <link href="css/thirdparty/radWorkFlow.css" rel="stylesheet" type="text/css" />
    <link href="css/summernote.css" rel="stylesheet" />

    <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
    <script src="js/thirdparty/jquery-ui.min.js" type="text/javascript"></script>
    <script src="js/thirdparty/bootstrap.min.js" type="text/javascript"></script>
    <script src="js/thirdparty/knockout-3.4.0.js"></script>
    <script src="js/thirdparty/underscore.js"></script>
    <script src="js/thirdparty/handlebars.js" type="text/javascript"></script>
    <script src="includes/SMSelect.js" type="text/javascript"></script>
    <script src="includes/SMSlimscroll.js" type="text/javascript"></script>
    <script src="js/thirdparty/jquery.jsPlumb.js" type="text/javascript"></script>
    <script src="js/thirdparty/radworkflow.Client.js" type="text/javascript"></script>
    <script src="js/thirdparty/summernote.js"></script>

    <script src="includes/RRuleEditorJsPlumb.js"></script>
    <script src="includes/ruleEngineJqueryPlugin.js"></script>
    <script src="includes/ruleEngineModularJqueryPlugin.js"></script>
    <script src="includes/RangyInputs.js"></script>
    <script src="includes/textareaUtil.js"></script>


    <script src="HandlebarTemplates/WorkflowManager/scripts/WorkflowHbsHelper.js" type="text/javascript"></script>

    <script src="js/WorkflowManager.js" type="text/javascript"></script>
    <script src="js/WorkFlowRuleSetup.js"></script>
</head>
<body>
    <form runat="server">
        <asp:HiddenField ID="UserName" runat="server" Value="" />
        <input id="hdnGuid" type="hidden" runat="server" clientidmode="Static" />
        <input id="hdnRuleClassInfo" type="hidden" runat="server" clientidmode="Static" />
        <input id="hdnRuleTxtUpdate" type="hidden" runat="server" clientidmode="Static" />
        <input id="hdnRuleSet" type="hidden" runat="server" clientidmode="Static" />
        <input id="hdnPriority" type="hidden" runat="server" clientidmode="Static" />
        <asp:Button ID="exportWorkflowServerBtn" ClientIDMode="Static" runat="server" OnClick="ExportWorkflow_Click" Style="display: none;" />
    </form>

    <div class="main-container">
        <div id="workflowDetails" class="workflow-details" style="overflow: hidden;">
        </div>
        <div id="workFlowDetailsErrorDiv_messagePopUp" style="display: none;">
            <div class="workFlowDetailsError_popupContainer">
                <div class="workFlowDetailsError_popupCloseBtnContainer">
                    <div class="fa fa-times" id="workFlowDetailsError_popupCloseBtn" onclick="javascript:$('#workFlowDetailsErrorDiv_messagePopUp').hide(); $('#workflowSetup_disableDiv').hide();"></div>
                </div>
                <div class="workFlowDetailsError_popupHeader">
                    <div class="workFlowDetailsError_imageContainer">
                        <img id="workFlowDetailsError_ImageURL" />
                    </div>
                    <div class="workFlowDetailsError_messageContainer">
                        <div class="workFlowDetailsError_popupTitle">
                        </div>
                    </div>
                </div>
                <div class="workFlowDetailsError_popupMessageContainer">
                    <div class="workFlowDetailsError_popupMessage">
                    </div>
                </div>
            </div>
        </div>
        <div id="workflowSetup_disableDiv" class="sm_workFlowDisableDiv" style="display: none; z-index: 99998;" align="center">
        </div>


        <div id="popupcaretsymbol" class="Popup_Outer_Div_Pointer">
            <div class="popupcaretstyle">
            </div>
        </div>
        <div id="RulestatePopupContainer" style="display: none;">

            <div id="RuleStatePopup_header" class="RuleStatePopUp_HeaderSection" style="width: 100%">
                <div class="PopupHorizontalAlign PopupHeaderColumn  workflowSetup_StateName ">
                    <div class="PopupHeaderColumn" style="padding-left: 2%;">STAGES</div>
                </div>
                <div class="PopupHorizontalAlign PopupHeaderColumn RuleCheckboxClass RuleCheckboxHeader">
                    <div class="PopupHorizontalAlign PopupHeaderColumn" data-bind="css: { CheckboxStyleMandatory: module() == 'sec', ColumnWidthForRefMaster: module() == 'ref' }">
                        <div class="PopupHorizontalAlign">Mandatory</div>
                    </div>
                    <div class="PopupHorizontalAlign PopupHeaderColumn" data-bind="css: { CheckboxStyleUnique: module() == 'sec', ColumnWidthForRefMaster: module() == 'ref' }">
                        <div class="PopupHorizontalAlign">Uniqueness</div>
                    </div>
                    <div class="PopupHorizontalAlign PopupHeaderColumn CheckboxStyle">
                        <div class="PopupHorizontalAlign">Primary Key</div>
                    </div>
                    <div class="PopupHorizontalAlign PopupHeaderColumn " data-bind="css: { CheckboxStyleValidation: module() == 'sec', ColumnWidthForRefMaster: module() == 'ref' }">
                        <div class="PopupHorizontalAlign">Validations</div>
                    </div>
                    <div id="AlertHeader" class="PopupHorizontalAlign PopupHeaderColumn " data-bind="css: { CheckboxAlertStyle: module() == 'sec', ColumnWidthForRefMaster: module() == 'ref' }">
                        <div class="PopupHorizontalAlign">Alerts</div>
                    </div>
                    <div class="PopupHorizontalAlign PopupHeaderColumn CheckboxStyleBasket" data-bind="visible: module() == 'sec'">
                        <div class="PopupHorizontalAlign">Basket Validations</div>
                    </div>
                    <div class="PopupHorizontalAlign PopupHeaderColumn CheckboxStyleBasket" data-bind="visible:module()== 'ref'">
                        <div class="PopupHorizontalAlign">Group Validation</div>
                    </div>
                    <div class="PopupHorizontalAlign PopupHeaderColumn CheckboxStyleBasket" data-bind="visible: module() == 'sec'">
                        <div class="PopupHorizontalAlign">Basket Alerts</div>
                    </div>
                </div>
            </div>
            <div id="workflowSetup_RuleStatePopUp" class="workflowSetup_RuleStatePopUp" data-bind="foreach: statesRuleState" style="width: 100%;">

                <div class="PopupHorizontalAlign workflowSetup_StateName">
                    <div class="PopupHorizontalAlign" data-bind="text: stateName == 'Start' ? 'Workflow Initiation' : stateName, attr: { title: stateName == 'Start' ? 'Workflow Initiation' : stateName }"></div>
                </div>
                <div class="PopupHorizontalAlign RuleCheckboxClass ToggleButtonStyling">
                    <div class="PopupHorizontalAlign PopupColumn " data-bind="css: { CheckboxStyleMandatory: $parent.module() == 'sec', ColumnWidthForRefMaster: $parent.module() == 'ref' }">
                        <input type="checkbox" data-bind="attr: { id: mandatoryDataCheckBoxID, checked: mandatoryData }, " />
                    </div>
                    <div class="PopupHorizontalAlign PopupColumn " data-bind="css: { CheckboxStyleUnique: $parent.module() == 'sec', ColumnWidthForRefMaster: $parent.module() == 'ref' }">
                        <input type="checkbox" data-bind="attr: { id: uniquenessValidationCheckBoxID, checked: uniquenessValidation }" />
                    </div>
                    <div class="PopupHorizontalAlign PopupColumn CheckboxStyle">
                        <input type="checkbox" data-bind="attr: { id: primaryKeyValidationCheckBoxID, checked: primaryKeyValidation }" />
                    </div>
                    <div class="PopupHorizontalAlign PopupColumn " data-bind="css: { CheckboxStyleValidation: $parent.module() == 'sec', ColumnWidthForRefMaster: $parent.module() == 'ref' }">
                        <input type="checkbox" data-bind="attr: { id: validationsCheckBoxID, checked: validations }" />
                    </div>
                    <div class="PopupHorizontalAlign PopupColumn " data-bind="css: { CheckboxAlertStyle: $parent.module() == 'sec', ColumnWidthForRefMaster: $parent.module() == 'ref' }">
                        <input type="checkbox" data-bind="attr: { id: alertsCheckBoxID, checked: alerts }" />
                    </div>
                    <div class="PopupHorizontalAlign PopupColumn CheckboxStyleBasket">
                        <input type="checkbox" data-bind="attr: { id: basketValidationCheckBoxID, checked: basketValidation }" />
                    </div>
                    <div class="PopupHorizontalAlign PopupColumn CheckboxStyleBasket" data-bind="visible: $parent.module() == 'sec'">
                        <input type="checkbox" data-bind="attr: { id: basketAlertCheckBoxID, checked: basketAlert }" />
                    </div>
                </div>
            </div>
            <div id="RuleStateErrorPopUp">
                ● You cannot edit the rules configured on stages as this workflow has pending request
            </div>
        </div>

        <div id="EmailstatePopupContainer" style="display: none;">
            <i style="display: none" class="fa-solid fa-trash"></i>
            <div id="EmailStatePopup_header" class="EmailStatePopUp_HeaderSection" style="width: 100%">
                
                <div class="WorflowDetails">
                    <span style="display: inline-block; margin-left: 20px; margin-top: 15px;">WORKFLOW</span>
                    <input class="WorkflowName" type="text" value="Default Name" readonly="true" />
                    <span style="display: inline-block;">WORKFLOW TEMPLATE</span>
                    <input class="WorkflowTemplateName" type="text" value="Default Template" readonly="true" />
                    <button class="SaveConfiguration" style="cursor: pointer">Save</button>
                    <span id="CloseEmailPopUp" style="font-size: 15px; float: right; margin-right: -5.5%;cursor: pointer;">x</span>
                </div>

            </div>
            <div class="ActionsSummary">
                <div class="Headings">
                    <div class="Action">Action</div>
                    <div class="ApplicationURL">Keep Application URL in the footer</div>
                    <div class="ConsolidatedEmail">Consolidated Email for Bulk Action</div>
                </div>
                <div id="workflowSetup_EmailStatePopUp" class="workflowSetup_EmailStatePopUp" data-bind="foreach: actionsState">
                    <div class="FirstHalf" style="width: 100%">
                        <input type="checkbox" class="checkboxForEachAction" style="margin-left: 10px; margin-top: 5px" data-bind="checked: checkboxForEachAction,enable:AllowUpdate" />
                        <div data-bind="attr:{id:EachActionState}" style="background-color: white; display: inline-block; margin-left: 10px; height: 35px; width: 86%; margin-bottom: 10px; font-size: 11px; border-radius: 5px; border: 1px solid transparent" onmouseout="unhighlightActionDiv(this)">

                            <div class="PopupHorizontalAlign workflowSetup_ActionName">
                                <div class="PopupHorizontalAlign" data-bind="text: ActionName"></div>
                            </div>


                            <div class="keepApplicatioURLInFooterCheckbox" style="margin-top: 9px">
                                <input type="checkbox" data-bind="checked: keepApplicationURLInTheFooter ,attr:{ id:keepApplicationURLInTheFooterCheckBoxID}" />
                            </div>
                            <div class="SendConsolidatedEmailForBulkActionToggle">
                                <input type="checkbox" data-bind="checked: sendConsolidatedEmailForBulkAction ,attr:{ id : sendConsolidatedEmailForBulkActionCheckBoxID}" />
                            </div>
                        </div>
                        <div class="ViewEmailTemplate" data-bind="click:onViewEmailTemplate">
                            <i class="fa fa-caret-right"></i>
                        </div>
                    </div>

                    <div class="ViewTemplateMailContainer" style="display: none;" data-bind=" attr: { id: actionId}">
                        <div class="MailHeaderSection">
                            <div style="margin-top: 10px">
                                <span style="margin-left: 15px">Mail Template</span>
                                <input type="checkbox" style="float: right; margin-right: 180px;" data-bind="checked: KeepCreatorInCC,enable:AllowUpdate" />
                                <span class="KeepCreatorInCC">Keep Creator in CC</span>
                            </div>

                            <div class="To">
                                <span style="font-weight: bold; font-size: 12px">To</span>
                                <input type="text" style="margin-left: 70px; width: 550px; border: 0px; border-bottom: 1px solid black; background: transparent;" data-bind="value:To,enable:AllowUpdate" />
                            </div>

                            <div class="Subject">
                                <span style="font-weight: bold; font-size: 12px">Subject</span>
                                <div class="SubjectDetails">
                                    <form autocomplete="off" style="width: 550px">
                                        <div class="autocomplete" style="position: relative; display: inline-block;">
                                            <input type="text" class="SubjectLine" placeholder="Enter the subject line" data-bind="value:Subject,attr:{id:SubjectLineTextBoxID},enable:AllowUpdate" onkeyup="showSuggestionForWorkflowActionSubject(this,this.value)" />
                                        </div>
                                    </form>
                                </div>
                            </div>
                            <div class="BulkSubject">
                                <span style="font-weight: bold; font-size: 12px">Bulk Subject</span>
                                <div class="BulkSubjectDetails">
                                    <form autocomplete="off" style="width: 550px">
                                        <div class="autocomplete" style="position: relative; display: inline-block;">
                                            <input type="text" class="BulkUpdateSubjectLine" placeholder="Enter the bulk update subject line" data-bind="value:BulkSubject,enable:AllowUpdate() && sendConsolidatedEmailForBulkAction(),attr:{id:BulkSubjectLineTextBoxID}" onkeyup="showSuggestionForWorkflowActionBulkSubject(this,this.value)" />
                                        </div>
                                    </form>
                                </div>
                            </div>
                        </div>
                        <div id="MailBody" class="MailBodySection">
                            <div style="background-color: white; height: 97%; width: 99%; margin-left: 5px;">
                                <textarea class="TitleLine" data-bind="value:MailBodyTitle,enable:AllowUpdate"></textarea>
                                <textarea class="StaticLine" data-bind="value:MailBodyContent,enable:AllowUpdate"></textarea>

                                <div class="DataSection" data-bind="foreach:DataSectionAttributes">
                                    <div style="min-width: 125px; width: fit-content; display: inline-block; border: 1px solid white; height: 62px; padding: 10px 7px 18px 7px; border-radius: 1px; background-color: #f1f6fa;"
                                        data-bind="event:{mouseover:$parent.ShowTrashIcon,mouseout:$parent.RemoveTrashIcon}">
                                        <div data-bind="text:$data,event:{mouseover:$parent.ShowTrashIcon,mouseout:$parent.RemoveTrashIcon}" style="height: 50%; text-align: center; color: #6d869a; cursor: default"></div>
                                        <div class="CrossIcon" style="display: none; float: right; margin-top: -30px; margin-right: -4px; cursor: default" data-bind="click:$parent.RemoveClickedAttribute,clickBubble: false">x</div>
                                        <div style="text-align: center; margin-top: 10px">--</div>
                                    </div>

                                </div>
                                <div class="FooterSection">
                                    <div style="display: inline-block; width: 65%">
                                        <span data-bind="visible:keepApplicationURLInTheFooter">Application Link:</span>
                                        <a style="border: 0px;" data-bind="visible:keepApplicationURLInTheFooter,href:ApplicationURL,text:ApplicationURL"></a>
                                    </div>
                                    <div style="width: 40%; float: right; display: inline-block; margin-top: -20px; height: 10%; text-align: right;">
                                        <span class="footer" style="border-radius: 5px; width: 125px; height: 31px; border: 1px solid rgb(74, 215, 197); color: rgb(53, 192, 174); margin-right: 15px; padding: 5px 10px 5px 10px">Add more attributes</span>
                                        <div class="workflow-top-component-value1" style="margin-top: -20px">
                                            <div class="available-attribute-dropdown" data-bind="attr: { id: availableAttributeDropdownId}"></div>
                                        </div>
                                    </div>
                                </div>
                                <div style="font-size: 11px; color: red; margin-top: 10px; margin-left: 2px">
                                    ● All the users in the groups mapped to the next stage for this particular action (considering Skip Level Workflow) will automatically receive the email
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div id="ActionStateErrorPopUp">
                    ● You cannot edit the configurations on actions as this workflow has pending request
                </div>
            </div>

        </div>

    </div>
</body>
</html>
