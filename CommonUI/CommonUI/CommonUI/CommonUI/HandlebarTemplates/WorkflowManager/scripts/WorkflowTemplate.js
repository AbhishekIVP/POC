var search_terms_for_subject = ["@instance", "@primaryAttribute"];
var search_terms_for_bulk_subject = ["@instance", "@requestCount"];
var availableAttributeDropdownID;
var cuurentPrioritySelected;
var cuurentRuleSetIdSelected;
var SelectedPrimaryDisplayAttribute;
var SelectedOtherDisplayAttributes;
var allSelectedPrimaryAndOtherAttributes;
var availableAttributes;
var selectedActionID;
var isAnyOtherActionHighlighted = false;
var allowUpdate = true;

var DataAttributesDictionary = {
    "Security Type": "-1",
    "Security ID": "-2",
    "Requested By": "-3",
    "Requested Time": "-4",
    "Approved By": "-5",
    "Approved At": "-6",
    "Approver Comments": "-7",
    "Rejected By": "-8",
    "Rejected Time": "-9",
    "Rejection Comments": "-10",
    "Cancelled By": "-11",
    "Cancellation Time": "-12",
    "Cancellation Comments": "-13",
    "Entity Type": "-14",
    "Entity Code": "-15"

}

function showSuggestionForWorkflowActionSubject(input, value) {
    var suggestionDiv, particularSuggestion, i, val = value;
    closeAllLists();
    if (value.endsWith('@')) {
        suggestionDiv = document.createElement("DIV");
        suggestionDiv.setAttribute("class", "autocomplete-items-for-action");

        var container = input.parentNode;
        container.appendChild(suggestionDiv);

        for (i = 0; i < search_terms_for_subject.length; i++) {
            particularSuggestion = document.createElement("DIV");
            particularSuggestion.setAttribute("class", "selected-item");
            particularSuggestion.innerHTML = search_terms_for_subject[i].substr(0, val.length);
            particularSuggestion.innerHTML += search_terms_for_subject[i].substr(val.length);
            particularSuggestion.innerHTML += "<input type='hidden' value='" + search_terms_for_subject[i] + "'>";
            particularSuggestion.addEventListener("click", function (e) {
                var initialValue = input.value;
                var initialValueArray = initialValue.split("");
                initialValueArray[initialValueArray.length - 1] = this.getElementsByTagName("input")[0].value;
                input.value = initialValueArray.join("");
                lastValue = input.value;

                closeAllLists();
                var textAreaValue = $('#' + input.id).val();
                for (var i = 0; i < workflowManger._pageViewModelActionsPopUp.actionsState().length; i++) {
                    if (workflowManger._pageViewModelActionsPopUp.actionsState()[i].SubjectLineTextBoxID() == input.id)
                        workflowManger._pageViewModelActionsPopUp.actionsState()[i].Subject(textAreaValue);
                }
            });
            suggestionDiv.appendChild(particularSuggestion);
        }
    }
    else
        closeAllLists();
}

function showSuggestionForWorkflowActionBulkSubject(input, value) {
    var suggestionDiv, particularSuggestion, i, val = value;
    closeAllLists();
    if (value.endsWith('@')) {
        suggestionDiv = document.createElement("DIV");
        suggestionDiv.setAttribute("class", "autocomplete-items-for-action");

        var container = input.parentNode;
        container.appendChild(suggestionDiv);

        for (i = 0; i < search_terms_for_bulk_subject.length; i++) {
            particularSuggestion = document.createElement("DIV");
            particularSuggestion.setAttribute("class", "selected-item");
            particularSuggestion.innerHTML = search_terms_for_bulk_subject[i].substr(0, val.length);
            particularSuggestion.innerHTML += search_terms_for_bulk_subject[i].substr(val.length);
            particularSuggestion.innerHTML += "<input type='hidden' value='" + search_terms_for_bulk_subject[i] + "'>";
            particularSuggestion.addEventListener("click", function (e) {
                var initialValue = input.value;
                var initialValueArray = initialValue.split("");
                initialValueArray[initialValueArray.length - 1] = this.getElementsByTagName("input")[0].value;
                input.value = initialValueArray.join("");
                lastValue = input.value;

                closeAllLists();
                var textAreaValue = $('#' + input.id).val();
                for (var i = 0; i < workflowManger._pageViewModelActionsPopUp.actionsState().length; i++) {
                    if (workflowManger._pageViewModelActionsPopUp.actionsState()[i].BulkSubjectLineTextBoxID() == input.id)
                        workflowManger._pageViewModelActionsPopUp.actionsState()[i].BulkSubject(textAreaValue);
                }
            });
            suggestionDiv.appendChild(particularSuggestion);
        }
    }
    else
        closeAllLists();
}

function closeAllLists(elmnt) {
    var x = document.getElementsByClassName("autocomplete-items-for-action");

    for (var i = 0; i < x.length; i++) {
        console.log(x[i]);
        if (elmnt != x[i]) {
            x[i].parentNode.removeChild(x[i]);

        }
    }
}

function highlightActionDiv(input) {
    if (!isAnyOtherActionHighlighted) {
        document.getElementById(input.id).style.border = "1px solid #5cade1";
    }
}

function unhighlightActionDiv(input) {
    if (input == undefined) {
        $("*[id*='actnDIV']").each(function (i, el) {
            el.style.border = "1px solid transparent";
        });
    }
    else if (!isAnyOtherActionHighlighted)
        document.getElementById(input.id).style.border = "1px solid transparent";
}

$(function () {
    $('.workflowCheck').unbind('click').on('click', function () {
        //console.log($(this))
        $('.workflowCheck').removeClass('selectedWorkflowTemplate');
        $(this).addClass('selectedWorkflowTemplate');
        $('[id^=workflowRuleIframePanel_]').html('');

        $('.workflow-setup-ruleEditortemplate-panel').css('display', 'none');
        $('.workflow-setup-template-panel').css('display', 'none');

        $('[id^=workflowSetupIframePanel_]').html('');

        var rad_workflow_instance_id = $(this).closest('.workflow-setup-card').parent().data('radworkflowinstanceid').toString();
        var module_id = $(this).parent().parent().parent().data('moduleid').toString();
        var guid = $('#hdnGuid').val().toString();
        var isworkflowRulesTemplate = $('#workflowRulesTemplate').css('display');

        if (isworkflowRulesTemplate == "none") {
            var workflow_instance_id = $('.workflow-top-container').data('instanceid');
            debugger;
            // Save Rule Data In Collection for Default
            workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'btn_SaveRuleInRADWorkFlowClick', { hdnRuleClassInfoValue: "", module: module_id, ruleInfo: "", Guid: guid, rad_workflow_id: rad_workflow_instance_id, updateCase: workflowManger._isUpdateRule, workFlowInstanceID: workflow_instance_id }, workflowManger.getWorkflowRulesTemporary, workflowManger.FailureCallback);
            $('.workflowRulesViewTemplate').css('display', 'block');
        }

    });

    $('.workflow-template-eye').unbind('click').on('click', function () {

        console.log('.workflow-rule-editor-template-eye');

        if (event.detail == undefined || event.detail == 1) {
            $('.workflow-template-eye').removeClass('selectedWorkflowTemplateEyeIcon');
            $(this).addClass('selectedWorkflowTemplateEyeIcon');
            $('[id^=workflowRuleIframePanel_]').html('');

            $('#newWorkflowTemplateRADModalContent').html('');

            $('.workflow-setup-template-panel').css('display', 'none');
            $('.workflow-setup-template-panel').html('');

            $('.workflow-setup-ruleEditortemplate-panel').css('display', 'none');
            $('.workflow-setup-ruleEditortemplate-panel').html('');

            var index = $(this).data('index') + 1;
            var workflowTemplatePanel = "";
            workflowTemplatePanel = "#workflowSetupIframePanel_" + Math.ceil(index / 4);

            var newWorkflowTemplate = {
                workFlowName: $(this).parent().parent().find('.workflow-template-title').attr('title'), //$('#newWorkflowTemplateName').val(),
                identifier: "workflowSetupIframePanel_" + Math.ceil(index / 4),  //"workflowSetupIframePanel_"
                baseUrl: workflowManger._path,
                user: workflowManger._userName,
                moduelid: $('.workflow-top-container').data('moduleid'),
                IsEditable: false,
                showLandingScrrenIcon: false
            }



            //$('#newWorkflowTemplateRADModal').modal('toggle');
            radworkflow.handlers.openWorkflow(newWorkflowTemplate);
            $(workflowTemplatePanel).slideDown("slow");
            $(this).closest('.workflow-setup-card').parent().nextAll('.workflow-Default-Rule-Editor-container:first').find('.workflowSetup_editableTemplateContainer').show();

            //$(workflowTemplatePanel).append('<h1>Opened Template: ' + $(this).data('name') + '</h1>');
            //$(workflowTemplatePanel).append('<h1>TODO: Add Rad Workflow in Iframe </h1>');
            if (typeof event != "undefined")
                event.stopPropagation();
            else
                return false;
        }
    });

    $('.workflow-rule-editor-template-eye').unbind('click').on('click', function () {
        console.log('.workflow-rule-editor-template-eye');

        if (event.detail == undefined || event.detail == 1) {
            $('.workflow-rule-editor-template-eye').removeClass('selectedWorkflowTemplateEyeIcon');
            $(this).addClass('selectedWorkflowTemplateEyeIcon');
            $('[id^=workflowRuleIframePanel_]').html('');
            $('#newWorkflowTemplateRADModalContent').html('');

            $('.workflow-setup-template-panel').css('display', 'none');
            $('.workflow-setup-template-panel').html('');

            $('.workflow-setup-ruleEditortemplate-panel').css('display', 'none');
            $('.workflow-setup-ruleEditortemplate-panel').html('');

            $('.workflowSetup_editableTemplateContainer').hide();

            var index = $(this).data('index') + 1;
            var workflowTemplatePanel = "";

            workflowTemplatePanel = "#workflowSetupRuleEditorIframePanel_" + Math.ceil(index / 4);
            identifierId = "";

            var newWorkflowTemplate = {
                workFlowName: $(this).parent().parent().find('.workflow-template-title').attr('title'), //$('#newWorkflowTemplateName').val(),
                identifier: "workflowSetupRuleEditorIframePanel_" + Math.ceil(index / 4),  //"workflowSetupIframePanel_"
                baseUrl: workflowManger._path,
                user: workflowManger._userName,
                moduelid: $('.workflow-top-container').data('moduleid'),
                IsEditable: false,
                showLandingScrrenIcon: false
            }

            //$('#newWorkflowTemplateRADModal').modal('toggle');
            radworkflow.handlers.openWorkflow(newWorkflowTemplate);
            $(workflowTemplatePanel).slideDown("slow");
            $(this).closest('.workflow-setup-card').parent().nextAll('.workflowRule-Editor-container:first').find('.workflowSetup_editableTemplateContainer').show();

            //$(workflowTemplatePanel).append('<h1>Opened Template: ' + $(this).data('name') + '</h1>');
            //$(workflowTemplatePanel).append('<h1>TODO: Add Rad Workflow in Iframe </h1>');
            if (typeof event != "undefined")
                event.stopPropagation();
            else
                return false;
        }
    });

    $('.SRMShowRADWorkFlowRule').unbind('click').on('click', function () {
        console.log('SRMShowRADWorkFlowRule click');
        // console.log(event.detail);
        if (event.detail == undefined || event.detail == 1) {
            $('.workflowRule-setup-panel').css('display', 'none');

            // $('.workflowRule-setup-panel').slideUp("slow");
            $('.workflowSetup_editableTemplateContainer').hide();

            $('.workflowRule-setup-panel').html('');
            $(this).closest('.workflow-rules-setup-card').css('border-bottom', 'none');
            var index = $(this).data('index') + 1;
            var workflowTemplatePanel = "#workflowRuleIframePanel_" + Math.ceil(index);

            //  var instanceId = $('.workflow-top-container').data('instanceid');

            var newWorkflowTemplate = {
                workFlowName: $(this).data('name'), //$('#newWorkflowTemplateName').val(),
                identifier: "workflowRuleIframePanel_" + Math.ceil(index),
                baseUrl: workflowManger._path,
                user: workflowManger._userName,
                moduelid: $('.workflow-top-container').data('moduleid'),
                IsEditable: false,
                showLandingScrrenIcon: false
            }

            radworkflow.handlers.openWorkflow(newWorkflowTemplate);
            $(workflowTemplatePanel).slideDown("slow");

            $(this).closest('.WorkflowRuleSetID').next('.workflowRule-setup-panel-top_container').find('.workflowSetup_editableTemplateContainer').show();
            event.stopPropagation();
        }
        //$(workflowTemplatePanel).css('display', 'block');
        if (typeof event != "undefined")
            event.stopPropagation();
        else
            return false;
    });

    $('.SRMWorkFlowRuleDeleteIcon').unbind('click').on('click', function () {
        console.log("delete button clicked.");

        $('.workflow-template-eye').removeClass('selectedWorkflowTemplateEyeIcon');
        $('.workflow-rule-editor-template-eye').removeClass('selectedWorkflowTemplateEyeIcon');
        $('.workflowCheck').removeClass('selectedWorkflowTemplate');


        var instanceId = $('.workflow-top-container').data('instanceid');
        var self = $(this);


        workflowManger.getSpecificWorkflowPendingRequestDetails(instanceId).then(function (data) {
            workflowManger._isPendingRequestWorkFlow = JSON.parse(data);

            if (!workflowManger._isPendingRequestWorkFlow) {
                if ($('.workflow-rules-setup-card').length > 1) {
                    var rule_set_id = self.closest('.WorkflowRuleSetID').data('rulesetid');
                    var is_Default = self.closest('.WorkflowRuleSetID').data('isdefaultworkflow');
                    self.closest('.WorkflowRuleSetID').remove();

                    var element = workflowManger._workflowStateRule.filter(function (data) {
                        return data.rulesetid != rule_set_id;
                    });
                    workflowManger._workflowStateRule = element;

                    workflowManger.deleteRuleInWorkFlow(rule_set_id, is_Default);
                }
                else {
                    workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'You cannot delete all rules for the workflow.');
                }
            }
            else {
                workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'You cannot delete rule as Workflow has pending requests.');
            }
        });

    });

    $('.SRMWorkFlowRuleUpdateIcon').unbind('click').on('click', function () {
        console.log("update button clicked.");
        debugger;
        $('.workflow-template-eye').removeClass('selectedWorkflowTemplateEyeIcon');
        $('.workflow-rule-editor-template-eye').removeClass('selectedWorkflowTemplateEyeIcon');
        $('.workflowCheck').removeClass('selectedWorkflowTemplate');

        $('#hdnRuleTxtUpdate').val('');
        $('#hdnRuleSet').val('');
        $('#hdnPriority').val('');

        var instanceId = $('.workflow-top-container').data('instanceid');
        var self = $(this);

        workflowManger.getSpecificWorkflowPendingRequestDetails(instanceId).then(function (data) {
            workflowManger._isPendingRequestWorkFlow = JSON.parse(data);

            if (!workflowManger._isPendingRequestWorkFlow) {
                $('[id^=workflowRuleIframePanel_]').html('');
                $('[id^=workflowRuleIframePanel_]').hide();

                $('[id^=workflowSetupIframePanel_]').html('');
                $('[id^=workflowSetupIframePanel_]').hide();
                debugger;
                $('.workflowCheck').removeClass('selectedWorkflowTemplate');
                var rule_set_id = self.closest('.WorkflowRuleSetID').data('rulesetid');
                var is_Default = self.closest('.WorkflowRuleSetID').data('isdefaultworkflow');

                var element = workflowManger._workflowStateRule.filter(function (data) {
                    return data.rulesetid != rule_set_id;
                });
                workflowManger._workflowStateRule = element;

                if (is_Default == true) {
                    $('#SMWorkFlowSetup_btnAddDefaultRule').trigger("click");
                }
                else {
                    var priority = self.closest('.WorkflowRuleSetID').data('priority');
                    var rule_text = self.closest('.WorkflowRuleSetID').data('ruletext');
                    var rule_set_id = self.closest('.WorkflowRuleSetID').data('rulesetid');


                    if ($('#selectedTypeList li').length == 0) {
                        workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'Please select atleast one type to add rule.');
                        return false;
                    }
                    else {
                        workflowManger._isUpdateRule = true;
                        $('#workflowRulesTemplate').slideToggle();
                        $('.workflow-setup-ruleEditortemplate-panel').css('display', 'none');
                        $('.workflow-setup-ruleEditortemplate-panel').html('');
                        $('.workflowCheck').removeClass('selectedWorkflowTemplate');
                        $('.workflow-template-eye').removeClass('selectedWorkflowTemplateEyeIcon');
                        $('.workflow-rule-editor-template-eye').removeClass('selectedWorkflowTemplateEyeIcon');
                        $('[id^=workflowRuleIframePanel_]').html('');

                        $('#hdnRuleTxtUpdate').val(rule_text);
                        $('#txtPriority').val(priority);
                        $('#hdnRuleSet').val(rule_set_id);
                        $('#hdnPriority').val(priority);

                        SetRuleEditorTextNew();

                        var radInstanceId = self.closest('.WorkflowRuleSetID').data('radworkflowinstanceid');
                        $('#WorkFlowTemplateViewForRule #' + radInstanceId).find('.workflowCheck').addClass('selectedWorkflowTemplate');
                        $('#WorkFlowTemplateViewForRule #' + radInstanceId).find('.workflow-rule-editor-template-eye').trigger('click');
                    }
                }
                // set for sending isupdate bit when updated
                workflowManger._isUpdateRule = true;
            }
            else {
                workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'You cannot update rule as Workflow has pending requests.');
            }
        });
    });

    function SetRuleEditorTextNew() {

        ruleDivId = 'RRuleEditor';
        HdnRuleTextId = 'hdnRuleTxtUpdate';

        if (typeof $("#" + ruleDivId).data('ruleEngine') === "undefined") {
            window.setTimeout(function () { SetRuleEditorTextNew(ruleDivId, HdnRuleTextId); }, 500);
        } else {

            var regEx = new RegExp(String.fromCharCode(160), 'gi');
            var normalSpace = ' ';

            var ruleText = $("#" + HdnRuleTextId).val().replace(regEx, normalSpace);
            ruleText = ' {' + ruleText + '} END';

            var grammarInfo = $("#" + ruleDivId).data('ruleEngine').settings.grammarInfo;
            $("#" + ruleDivId).ruleEngine().data('ruleEngine').Destroy();
            $("#" + ruleDivId).ruleEngine({ grammarInfo: grammarInfo, serviceUrl: workflowManger._path + "/RADXRuleEditorService.svc", ruleText: ruleText, ExternalFunction: RuleCompleteHandler });
        }
    }

    //View Model
    function popUpPageViewModelActions(data) {
        var self = this;
        self.actionsState = ko.observableArray();

        if (typeof (data) != 'undefined' && typeof (data.Actions) != 'undefined') {
            for (var item in data.Actions) {
                self.actionsState.push(new ActionsDataDetails(data.Actions[item]));
            }
        }
    }

    function popUpPageViewModel(data) {
        var self = this;
        self.module = ko.observable(moduleForWorflow);
        self.statesRuleState = ko.observableArray();


        if (typeof (data.statesRuleState) != 'undefined') {
            for (var item in data.statesRuleState) {
                self.statesRuleState.push(new rulesDataDetails(data.statesRuleState[item]));
            }
        }
    }

    //function
    function ActionsDataDetails(data) {
        var self = this;

        self.checkboxForEachAction = ko.observable(data.CheckBoxForEachAction);

        self.ActionName = data.ActionName;

        self.keepApplicationURLInTheFooter = ko.observable(data.KeepApplicationURLInTheFooter);

        self.sendConsolidatedEmailForBulkAction = ko.observable(data.SendConsolidatedEmailForBulkAction);

        self.To = ko.observable(data.To);

        self.KeepCreatorInCC = ko.observable(data.KeepCreatorInCC);

        self.PrimaryDisplayAttribute = ko.observable(workflowManger._primaryDisplayAttributeList);


        if (moduleForWorflow == 'sec')
            self.DataSectionAttributes = ko.observableArray(["Security Type", "Security ID"]);
        if (moduleForWorflow == 'ref')
            self.DataSectionAttributes = ko.observableArray(["Entity Type", "Entity Code"]);


        self.MailBodyTitle = ko.observable(data.MailBodyTitle);

        self.MailBodyContent = ko.observable(data.MailBodyContent);

        //how to get the url dynamically.
        self.ApplicationURL = ko.computed(function () {
            //if (self.sendConsolidatedEmailForBulkAction())
            //    return "http://localhost:3039/SMHomeNew.aspx";
            //else {
            //    return "http://localhost:3039/SMHomeNew.aspx";
            //}
            return window.location.origin;
        })

        self.actionId = ko.computed(function () {
            var id = self.ActionName.replace(/\s/g, "").concat("actN");
            return id;
        })

        self.EachActionState = ko.computed(function () {
            var id = self.ActionName.replace(/\s/g, "").concat("actnDIV");
            return id;
        })

        self.availableAttributeDropdownId = ko.computed(function () {
            return self.ActionName.replace(/\s/g, "").concat("dropdown");
        });

        self.SubjectLineTextBoxID = ko.computed(function () {
            return self.ActionName.replace(/\s/g, "").concat("textArea");
        });

        self.BulkSubjectLineTextBoxID = ko.computed(function () {
            return self.ActionName.replace(/\s/g, "").concat("textAreaBulk");
        });

        self.ElementWithId = function () {
            return $('#' + self.availableAttributeDropdownId());
        }

        self.DataSectionColumns = ko.computed(function () {
            if (data.DataSectionAttributes != null) {
                for (var i = 0; i < data.DataSectionAttributes.length; i++) {
                    var attribute = data.DataSectionAttributes[i];
                    switch (parseInt(attribute)) {
                        case -1:
                            break;
                        case - 2:
                            break;
                        case -3:
                            self.DataSectionAttributes.push("Requested By");
                            break;
                        case -4:
                            self.DataSectionAttributes.push("Requested Time");
                            break;
                        case -5:
                            self.DataSectionAttributes.push("Approved By");
                            break;
                        case -6:
                            self.DataSectionAttributes.push("Approved At");
                            break;
                        case -7:
                            self.DataSectionAttributes.push("Approver Comments");
                            break;
                        case -8:
                            self.DataSectionAttributes.push("Rejected By");
                            break;
                        case -9:
                            self.DataSectionAttributes.push("Rejected Time");
                            break;
                        case -10:
                            self.DataSectionAttributes.push("Rejection Comments");
                            break;
                        case -11:
                            self.DataSectionAttributes.push("Cancelled By");
                            break;
                        case -12:
                            self.DataSectionAttributes.push("Cancellation Time");
                            break;
                        case -13:
                            self.DataSectionAttributes.push("Cancellation Comments");
                            break;
                        case 14:
                            break;
                        case 15:
                            break;
                        default:
                            for (var j = 0; j < allSelectedPrimaryAndOtherAttributes.length; j++) {
                                if (attribute == allSelectedPrimaryAndOtherAttributes[j].AttributeId) {
                                    self.DataSectionAttributes.push(allSelectedPrimaryAndOtherAttributes[j].AttributeName);
                                    break;
                                }
                            }
                            break;
                    }

                }
            }

        })

        self.Subject = ko.observable(data.Subject);

        self.AllowUpdate = ko.observable(allowUpdate);

        self.BulkSubject = ko.observable(data.BulkSubject);

        self.keepApplicationURLInTheFooterCheckBoxID = ko.computed(function () {
            var finalString = "keepApplicationURLInTheFooter_checkbox_"
                + self.ActionName.replace(/ /g, "_");
            return finalString;
        });

        self.sendConsolidatedEmailForBulkActionCheckBoxID = ko.computed(function () {
            var finalString = "sendConsolidatedEmailForBulkAction_checkbox_"
                + self.ActionName.replace(/ /g, "_");
            return finalString;
        });

        self.checkboxForEachActionCheckBoxID = ko.computed(function () {
            var finalString = "checkboxForEachAction_checkbox_"
                + self.ActionName.replace(/ /g, "_");
            return finalString;
        })

        self.RemoveClickedAttribute = function (obj, event) {
            const index = self.DataSectionAttributes.indexOf($(event.target).parent().children()[0].textContent);
            switch ($(event.target).parent().children()[0].textContent) {
                case "Security Type":
                case "Entity Type":
                case "Security ID":
                case "Entity Code":
                    break;

                default:
                    if (index > -1) {
                        self.DataSectionAttributes.splice(index, 1);
                    }
                    break;
            }
        }

        self.ShowTrashIcon = function (obj, event) {
            switch ($(event.target).text().trim().split('\n')[0]) {
                case "Security Type":
                case "Entity Type":
                case "Security ID":
                case "Entity Code":
                    break;

                default:
                    {
                        if ($(event.target).children().length > 0)
                            $(event.target).children()[1].style.display = 'block';
                        if ($(event.target).children().length == 0)
                            $(event.target).next().show()
                    }
            }

        }

        self.RemoveTrashIcon = function (obj, event) {
            if ($('.CrossIcon:visible').length > 0)
                $('.CrossIcon:visible')[0].style.display = 'none';
        }

        self.onViewEmailTemplate = function (obj, event) {
            console.log("view email template button clicked.");
            isAnyOtherActionHighlighted = false;
            selectedActionID = event.currentTarget.previousElementSibling;
            unhighlightActionDiv();
            highlightActionDiv(selectedActionID);
            isAnyOtherActionHighlighted = true;

            $('.ViewTemplateMailContainer').css('display', 'none');

            var element = $("body").find("#" + $(this)[0].actionId());
            var popupwidth = $('.ActionsSummary').width();
            var popUpHeight = $('.ActionsSummary').height() - 10;

            element.css({ top: '7%', left: '50.5%', position: 'absolute' });

            element.css('width', '49%');
            element.css('display', 'block');
            element.css('background-color', '#f8f8f8');
            element.css('height', '93%');
            element.css('z-index', '1');
            event.stopPropagation();
        }
    }

    function rulesDataDetails(data) {
        var self = this;
        self.stateName = data.stateName;
        self.mandatoryData = data.mandatoryData;
        self.uniquenessValidation = data.uniquenessValidation;
        self.validations = data.validations;
        self.primaryKeyValidation = data.primaryKeyValidation;
        self.alerts = data.alerts;
        self.basketValidation = data.basketValidation;
        self.basketAlert = data.basketAlert;
        //self.isStateChecked = ko.observable();

        //if (data.isStateChecked != "undefined")
        //    self.isStateChecked(data.isStateChecked);
        //else
        //    self.isStateChecked(false);

        //self.stateNameCheckBoxId = ko.computed(function () {
        //    var finalString = "allState_checkbox_" + self.stateName.replace(/ /g, "_");
        //    return finalString;
        //});

        self.mandatoryDataCheckBoxID = ko.computed(function () {
            var finalString = "mandatoryData_checkbox_"
                + self.stateName.replace(/ /g, "_");
            return finalString;
        });

        self.uniquenessValidationCheckBoxID = ko.computed(function () {
            var finalString = "uniquenessValidation_checkbox_"
                + self.stateName.replace(/ /g, "_");
            return finalString;
        });

        self.validationsCheckBoxID = ko.computed(function () {
            var finalString = "validations_checkbox_"
                + self.stateName.replace(/ /g, "_");
            return finalString;
        });

        self.primaryKeyValidationCheckBoxID = ko.computed(function () {
            var finalString = "primaryKeyValidation_checkbox_"
                + self.stateName.replace(/ /g, "_");
            return finalString;
        });

        self.alertsCheckBoxID = ko.computed(function () {
            var finalString = "alerts_checkbox_"
                + self.stateName.replace(/ /g, "_");
            return finalString;
        });

        self.basketValidationCheckBoxID = ko.computed(function () {
            var finalString = "basketValidation_checkbox_"
                + self.stateName.replace(/ /g, "_");
            return finalString;
        });

        self.basketAlertCheckBoxID = ko.computed(function () {
            var finalString = "basketAlert_checkbox_"
                + self.stateName.replace(/ /g, "_");
            return finalString;
        });
    }

    $('.SRMWorkFlowRuleStatePopUpIcon').unbind('click').on('click', function (event) {
        console.log("rule state button clicked.");
        debugger;
        ruleSetId = $(this).closest('.workflow-rules-setup-card').parent().data('rulesetid');
        var moduleId = $('.workflow-top-container').data('moduleid');

        if (moduleId == 3)
            moduleForWorflow = 'sec';
        else
            moduleForWorflow = 'ref';

        $('#hdnRuleSet').val(ruleSetId);
        templatename = $(this).closest('.workflow-rules-setup-card').parent().data('radtemplatename');
        instanceId = $('.workflow-top-container').data('instanceid');
        $(this).closest('.workflow-rules-setup-card').find('.SRMShowRADWorkFlowRule').trigger('click');

        if (typeof templatename != "undefined" && templatename != "")
            workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'GetStageNameFromRAD', { templatename: templatename }, bindToStatesArray, workflowManger.FailureCallback);


        var popUpOffset = $(event.target).offset();
        var popupwidth = $('.WorkFlowRightDiv').width() - 200;
        if (moduleForWorflow == 'sec') {
            $("#RulestatePopupContainer").css({ top: popUpOffset.top + 40, left: (popUpOffset.left - popupwidth + 50), position: 'absolute' });
            $('#RulestatePopupContainer').css('width', popupwidth);
        }
        else {
            $("#RulestatePopupContainer").css({ top: popUpOffset.top + 40, left: (popUpOffset.left - popupwidth + 200), position: 'absolute' });
            $('#RulestatePopupContainer').css('width', popupwidth - 137);
        }

        $('#RulestatePopupContainer').css('background-color', 'white');
        $('#workflowSetup_disableDiv').css('display', 'block');
        $('#RulestatePopupContainer').css('display', 'block');
        $('#popupcaretsymbol').css({ top: popUpOffset.top + 20, left: (popUpOffset.left + 10), position: 'absolute' });
        $('#popupcaretsymbol').css('display', 'block');

        event.stopPropagation();

    });

    $('#CloseEmailPopUp').unbind('click').on('click', function (event) {
        $('.autocomplete-items-for-action').hide();
        $('#EmailstatePopupContainer').hide();
        $('#popupcaretsymbol').css('display', 'none');
        $('#workflowSetup_disableDiv').css('display', 'none');
    });

    $('.SRMWorkFlowEditEmailTemplate').unbind('click').on('click', function (event) {
        workflowManger._emailTemplatefromUI = true;
        console.log("edit email template button clicked.");
        instanceId = $('.workflow-top-container').data('instanceid');
        var moduleId = $('.workflow-top-container').data('moduleid');

        if (moduleId == 3)
            moduleForWorflow = 'sec';
        else
            moduleForWorflow = 'ref';
        if (instanceId != 0) {
            ruleSetId = $(this).closest('.workflow-rules-setup-card').parent().data('rulesetid');
            radWorkflowInstanceId = $(this).closest('.workflow-rules-setup-card').parent().data('radworkflowinstanceid');;


            templatename = $(this).closest('.workflow-rules-setup-card').parent().data('radtemplatename');
            workflowname = $('.workflow-top-container').data('name');

            smselect.hide($('body').find("#smselect_primaryAttributeDropdown"));
            smselect.hide($('body').find("#smselect_otherAttributeDropdown"));

            $(".WorkflowTemplateName").val(templatename);
            $(".WorkflowName").val(workflowname);

            rulePriority = $(this).closest('.workflow-rules-setup-card').parent().data('priority');

            var workflowDetails = [{
                "RulePriority": rulePriority,
                "WorkflowName": workflowname
            }];

            //if (typeof templatename != "undefined" && templatename != "") {
            if (typeof workflowname != "undefined" && workflowname != "" && rulePriority != "") {
                workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'GetWorkflowEmailActions', { moduleId: moduleId, workflowDetails: workflowDetails }, bindToActionsArray, workflowManger.FailureCallback);
            }

            var popUpOffset = $(event.target).offset();
            var popupwidth = $('.workflow-details').width() - 50;
            var popupHeight = $('.WorkFlowRightDiv').height() - 25;

            $("#EmailstatePopupContainer").css({ top: popUpOffset.top - 45, marginLeft: '20px', position: 'absolute' });
            $('#EmailstatePopupContainer').css('width', '98%');
            $('#EmailstatePopupContainer').css('height', '90%');
            $('#EmailstatePopupContainer').css('background-color', 'white');
            $('#workflowSetup_disableDiv').css('display', 'block');
            $('#EmailstatePopupContainer').css('display', 'block');
            $('#EmailstatePopupContainer').css('border-radius', '2px');
            $('#EmailstatePopupContainer').css('box-shadow', '0px 0px 1px 1px lightgrey');

            event.stopPropagation();
        }
        else {
            workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'You cannot edit email template as workflow is not configured/saved.');
        }
    });

    $('.SaveConfiguration').unbind('click').on('click', function (event) {
        var error = false;
        var saveConfigurationForActions = [];
        if (workflowManger._emailTemplatefromUI) {
            var len = workflowManger._pageViewModelActionsPopUp.actionsState().length;
        }
        else
            var len = workflowManger._pageViewModelActionsPopUp_update.actionsState().length;
        for (var i = 0; i < len; i++) {
            if (workflowManger._emailTemplatefromUI)
                var selectedAction = workflowManger._pageViewModelActionsPopUp.actionsState()[i];
            else
                var selectedAction = workflowManger._pageViewModelActionsPopUp_update.actionsState()[i];

            if (selectedAction.checkboxForEachAction()) {
                var adc = {};
                adc.KeepApplicationURLInTheFooter = selectedAction.keepApplicationURLInTheFooter();
                adc.sendConsolidatedEmailForBulkAction = selectedAction.sendConsolidatedEmailForBulkAction();
                adc.KeepCreatorInCC = selectedAction.KeepCreatorInCC();
                adc.ActionName = selectedAction.ActionName;
                adc.To = selectedAction.To();
                if (adc.To != "") {
                    var validRegex = new RegExp(/^(\s?[^\s,]+@[^\s,]+\.[^\s,]+\s?,)*(\s?[^\s,]+@[^\s,]+\.[^\s,]+)$/);
                    const result = validRegex.test(adc.To);
                    if (!result) {
                        workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'Please configure valid email id\'s');
                        error = true;
                    }
                }
                adc.Subject = selectedAction.Subject();
                adc.BulkSubject = selectedAction.BulkSubject();
                adc.MailBodyTitle = selectedAction.MailBodyTitle();
                adc.MailBodyContent = selectedAction.MailBodyContent();
                adc.DataSectionAttributes = [];

                for (var j = 0; j < selectedAction.DataSectionAttributes().length; j++) {
                    var attribute = selectedAction.DataSectionAttributes()[j];
                    adc.DataSectionAttributes.push(parseInt(DataAttributesDictionary[attribute]));
                }

                saveConfigurationForActions.push(adc);
            }


        }

        var workflowName = $('.workflow-top-container').data('name');
        if (workflowManger._pageViewModelActionsPopUp_update != null && workflowManger._pageViewModelActionsPopUp_update.RulePriority != null && !workflowManger._emailTemplatefromUI) {
            cuurentPrioritySelected = workflowManger._pageViewModelActionsPopUp_update.RulePriority;
        }


        var selectedActionsInfo = [{
            "WorkflowName": workflowName,
            "RulePriority": cuurentPrioritySelected,
            "SaveConfigurationForActions": saveConfigurationForActions
        }
        ];

        if (!error) {
            if (workflowName != null && cuurentPrioritySelected != null) {
                workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'SaveWorkflowEmailAction', { selectedActionsInfo: selectedActionsInfo }, SaveWorkflowEmailActionCallback, workflowManger.FailureCallback);
            }

        }
        else {
            $('#EmailstatePopupContainer').hide();
        }
    })

    function SaveWorkflowEmailActionCallback(result) {
        if (result.d[0].Message == 'Passed') {
            workflowManger.showErrorDiv('Success', 'pass_icon.png', 'Workflow Updated successfully.');
            $('#EmailstatePopupContainer').hide();
            workflowManger._workflowAction = [];
            cuurentRuleSetIdSelected = null;
        }
        else {
            if (result.d[0].Message.indexOf('pending request') != -1) {
                workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'You cannot update workflow as it has pending request.');
                $('#EmailstatePopupContainer').hide();
                workflowManger._workflowAction = [];
            }
            else {
                workflowManger.showErrorDiv('Alert', 'fail_icon.png', result.d[0].Message);
                $('#EmailstatePopupContainer').hide();
            }
        }
    }

    function bindToActionsArray(ActionData) {
        var ActionData = ActionData.d[0];
        workflowManger._actionData = ActionData;

        if (workflowManger._workflowAction.length == 0) {
            workflowManger.toAddinCollectionAction = true;
            BindDataToUIAction(ActionData);
        }

        else if (workflowManger._workflowAction.length > 0) {
            var element = workflowManger._workflowAction.filter(function (data) {
                if (data.RuleSetId == ActionData.RuleSetId && data.Actions.length == ActionData.Actions.length) {

                    let actionToAdd = ActionData.Actions.filter(o1 => !data.Actions.some(o2 => o1.ActionName === o2.ActionName));
                    let actionToRemove = data.Actions.filter(o1 => !ActionData.Actions.some(o2 => o1.ActionName === o2.ActionName));

                    if (actionToAdd.length == 0 && actionToRemove.length == 0)
                        return data.RuleSetId == ruleSetId;

                    //if same number of actions are added and same number of actions are removed at same time.
                    else {
                        for (var i = 0; i < actionToAdd.length; i++) {
                            data.Actions.push(actionToAdd[i]);
                            workflowManger._pageViewModelActionsPopUp.actionsState.push(new ActionsDataDetails(actionToAdd[i]));
                        }
                        for (var i = 0; i < actionToRemove.length; i++) {
                            var index = data.Actions.indexOf(actionToRemove[i]);
                            data.Actions.splice(index, 1);
                            let actionToDeleteFromObservableArray = new ActionsDataDetails(actionToRemove[i]);
                            for (var j = 0; j < workflowManger._pageViewModelActionsPopUp.actionsState().length; j++) {
                                if (workflowManger._pageViewModelActionsPopUp.actionsState()[j].ActionName == actionToDeleteFromObservableArray.ActionName) {
                                    var index1 = j;
                                    break;
                                }
                            }
                            workflowManger._pageViewModelActionsPopUp.actionsState.splice(index1, 1);
                        }
                        return data.RuleSetId == ruleSetId;
                    }

                }

                else {
                    if (data.RuleSetId == ActionData.RuleSetId) {
                        // new action is added
                        //existig action is removed
                        let actionAdded = ActionData.Actions.filter(o1 => !data.Actions.some(o2 => o1.ActionName === o2.ActionName));
                        let result1 = data.Actions.filter(o1 => !ActionData.Actions.some(o2 => o1.ActionName === o2.ActionName));

                        if (actionAdded.length > 0) {
                            for (var i = 0; i < actionAdded.length; i++) {
                                data.Actions.push(actionAdded[i]);
                                workflowManger._pageViewModelActionsPopUp.actionsState.push(new ActionsDataDetails(actionAdded[i]));
                            }
                        }

                        if (result1.length > 0) {
                            for (var i = 0; i < result1.length; i++) {
                                var index = data.Actions.indexOf(result1[i])
                                data.Actions.splice(index, 1);
                                let actionToDelete;
                                actionToDelete = new ActionsDataDetails(result1[i]);
                                debugger;
                                for (var j = 0; j < workflowManger._pageViewModelActionsPopUp.actionsState().length; j++) {
                                    if (workflowManger._pageViewModelActionsPopUp.actionsState()[j].ActionName == actionToDelete.ActionName) {
                                        var index1 = j;
                                        break;
                                    }
                                }
                                workflowManger._pageViewModelActionsPopUp.actionsState.splice(index1, 1);

                            }
                        }
                        return data.RuleSetId == ruleSetId;
                    }
                    //return data.RuleSetId == ruleSetId;
                }
                //   return data.RulePriority == rulePriority;
                //  return data.RadWorkflowId == radWorkflowInstanceId;

            });

            if (element.length > 0) {
                workflowManger.toAddinCollectionAction = false;
                BindDataToUIAction(element[0]);
            }
            else {
                workflowManger.toAddinCollectionAction = true;
                BindDataToUIAction(ActionData);
            }
        }

    }

    function BindDataToUIAction(ActionData) {
        ActionData.Actions.sort(function (a, b) {
            var textA = a.ActionName.toUpperCase();
            var textB = b.ActionName.toUpperCase();
            return (textA < textB) ? -1 : (textA > textB) ? 1 : 0;
        });
        ActionData.Actions.forEach(function (Action) {
            var a = Action.ActionName;
            if (a == 'Creator initiates the request') {
                var item = Action;
                const index = ActionData.Actions.indexOf(item);
                ActionData.Actions.splice(index, 1);
                ActionData.Actions.unshift(item);
            }
        })
        ActionData.Actions.forEach(function (Action) {
            var a = Action.ActionName;
            if (a == 'Creator cancels the request') {
                var element = Action;
                ActionData.Actions.push(ActionData.Actions.splice(ActionData.Actions.indexOf(element), 1)[0]);
            }
        })

        if (workflowManger.toAddinCollectionAction) {
            workflowManger._workflowAction.push(ActionData);
        }


        SelectedPrimaryDisplayAttribute = smselect.getSelectedOption($('#smselect_primaryAttributeDropdown'));
        SelectedOtherDisplayAttributes = smselect.getSelectedOption($('#smselect_otherAttributeDropdown'));
        allSelectedPrimaryAndOtherAttributes = JSON.parse(JSON.stringify(workflowManger._primaryAttributeList.concat(workflowManger._otherAttributeList)));

        if (typeof ActionData != "undefined" && ActionData.Actions.length > 0) {
            if (!workflowManger.__koBindingAppliedActions) {
                if (typeof ko !== 'undefined') {
                    cuurentRuleSetIdSelected = ActionData.RuleSetId;
                    cuurentPrioritySelected = ActionData.RulePriority;

                    workflowManger._pageViewModelActionsPopUp = new popUpPageViewModelActions(ActionData);

                    ko.applyBindings(workflowManger._pageViewModelActionsPopUp, $("#EmailstatePopupContainer")[0]);
                }

                workflowManger.__koBindingAppliedActions = true;

                if (SelectedPrimaryDisplayAttribute.length != 0) {
                    var PrimaryDisplayAttributeText;
                    PrimaryDisplayAttributeText = JSON.parse(JSON.stringify(SelectedPrimaryDisplayAttribute[0]));
                    var len = ActionData.Actions.length;
                    while (len > 0) {
                        workflowManger._pageViewModelActionsPopUp.actionsState()[len - 1].PrimaryDisplayAttribute(PrimaryDisplayAttributeText);
                        len--;
                    }
                    workflowManger._primaryDisplayAttributeList = SelectedPrimaryDisplayAttribute[0]
                }


            }
            else {
                if (ActionData.RuleSetId != cuurentRuleSetIdSelected) {
                    workflowManger._pageViewModelActionsPopUp.actionsState.removeAll();

                    for (var item in ActionData.Actions) {
                        workflowManger._pageViewModelActionsPopUp.actionsState.push(new ActionsDataDetails(ActionData.Actions[item]));
                    }
                    cuurentRuleSetIdSelected = ActionData.RuleSetId;
                    cuurentPrioritySelected = ActionData.RulePriority;
                }

            }

            for (var i = 0; i < workflowManger._primaryAndOtherDisplayAttributeList.length; i++) {
                DataAttributesDictionary[workflowManger._primaryAndOtherDisplayAttributeList[i].text] = workflowManger._primaryAndOtherDisplayAttributeList[i].value;
            }
            var availableAttributeList = [{ "value": "", "text": "--Add more attributes--" }];
            workflowManger._primaryAndOtherDisplayAttributeList.forEach(function (d) {
                availableAttributeList.push({
                    "value": d.value,
                    "text": d.text
                })
            })

            if (workflowManger._primaryAndOtherDisplayAttributeList.length > 0) {
                debugger;
                var len3 = ActionData.Actions.length;
                while (len3 > 0) {
                    var availableAttributesForAction = JSON.parse(JSON.stringify(availableAttributeList));

                    var actionName = workflowManger._pageViewModelActionsPopUp.actionsState()[len3 - 1].ActionName;

                    if (actionName.toLowerCase().includes('initiates'))
                        availableAttributesForAction.push({ "value": "", "text": "Requested By" }, { "value": "", "text": "Requested Time" })
                    if (actionName.toLowerCase().includes('approve'))
                        availableAttributesForAction.push({ "value": "", "text": "Approved By" }, { "value": "", "text": "Approved At" }, { "value": "", "text": "Approver Comments" })
                    if (actionName.toLowerCase().includes('reject'))
                        availableAttributesForAction.push({ "value": "", "text": "Rejected By" }, { "value": "", "text": "Rejected Time" }, { "value": "", "text": "Rejection Comments" })
                    if (actionName.toLowerCase().includes('cancel'))
                        availableAttributesForAction.push({ "value": "", "text": "Cancelled By" }, { "value": "", "text": "Cancellation Time" }, { "value": "", "text": "Cancellation Comments" })

                    workflowManger.createSMSelectDropDown(workflowManger._pageViewModelActionsPopUp.actionsState()[len3 - 1].ElementWithId(), availableAttributesForAction, true, 180, null, null, AddAttributesInTheDataSection, [], false, null, null);
                    len3--;
                }

            }

            var instanceId = $('.workflow-top-container').data('instanceid');
            //var allowUpdate = false;
            debugger;

            workflowManger.getSpecificWorkflowPendingRequestDetails(instanceId).then(function (data) {
                debugger;
                workflowManger._isPendingRequestWorkFlow = JSON.parse(data);
                if (workflowManger._isPendingRequestWorkFlow) {
                    allowUpdate = false;
                    $('#ActionStateErrorPopUp').css('display', 'inline-block');
                    for (var i = 0; i < workflowManger._pageViewModelActionsPopUp.actionsState().length; i++)
                        workflowManger._pageViewModelActionsPopUp.actionsState()[i].AllowUpdate(false);
                }
                if (!workflowManger._isPendingRequestWorkFlow) {
                    $('#ActionStateErrorPopUp').css('display', 'none');
                }


                for (var checkbox in workflowManger._pageViewModelActionsPopUp.actionsState()) {
                    var actionIterator = workflowManger._pageViewModelActionsPopUp.actionsState()[checkbox];
                    //convertCheckBoxToToggleAction(actionIterator.keepApplicationURLInTheFooterCheckBoxID(), allowUpdate, actionIterator.actionName);
                    //convertCheckBoxToToggleAction(actionIterator.sendConsolidatedEmailForBulkActionCheckBoxID(), allowUpdate, actionIterator.actionName);
                    convertCheckBoxToToggle(actionIterator.keepApplicationURLInTheFooterCheckBoxID(), allowUpdate, actionIterator.actionName, true);
                    convertCheckBoxToToggle(actionIterator.sendConsolidatedEmailForBulkActionCheckBoxID(), allowUpdate, actionIterator.actionName, true);
                }
            });
        }
        workflowManger.toAddinCollectionAction = false;
        $('.ViewEmailTemplate')[0].click();

    }

    function AddAttributesInTheDataSection(attribute) {
        let result = attribute.target.id.substr(9);
        let result1 = result.replace("dropdown", "");
        console.log("add attribute in data section callback called- " + attribute.target.innerText);

        for (var i = 0; i < workflowManger._pageViewModelActionsPopUp.actionsState().length; i++) {
            let correspondingActionName = workflowManger._pageViewModelActionsPopUp.actionsState()[i].ActionName.replace(/\s/g, "");
            if (correspondingActionName == result1) {
                if (!workflowManger._pageViewModelActionsPopUp.actionsState()[i].DataSectionAttributes().includes(attribute.target.innerText) && !attribute.target.innerText.includes("Add more attributes")) {
                    workflowManger._pageViewModelActionsPopUp.actionsState()[i].DataSectionAttributes.push(attribute.target.innerText);
                }

            }
        }
        $('#' + attribute.target.id).css("width", "180px");
        $('#' + attribute.target.id).css("float", "right");
        smselect.reset($("#" + attribute.target.id));
    }

    function bindToStatesArray(StageData) {
        var StageData = StageData.d;
        workflowManger._stateData = StageData;

        if (workflowManger._workflowStateRule.length == 0) {
            workflowManger.toAddinCollection = true;
            if (instanceId == 0)
                workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'GetStateRuleData', { ruleSetId: ruleSetId, templatename: templatename }, BindDataToUI, workflowManger.FailureCallback);
            else
                workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'GetWorkFlowRuleStateDataForUpdate', { ruleSetId: ruleSetId, instanceId: instanceId, templatename: templatename }, BindDataToUI, workflowManger.FailureCallback);
        }
        else if (workflowManger._workflowStateRule.length > 0) {

            var element = workflowManger._workflowStateRule.filter(function (data) {
                return data.rulesetid == ruleSetId;
            });
            if (element.length > 0) {
                workflowManger.toAddinCollection = false;
                BindDataToUI(element[0]);
            }
            else {
                workflowManger.toAddinCollection = true;
                if (instanceId == 0)
                    workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'GetStateRuleData', { ruleSetId: ruleSetId, templatename: templatename }, BindDataToUI, workflowManger.FailureCallback);
                else
                    workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'GetWorkFlowRuleStateDataForUpdate', { ruleSetId: ruleSetId, instanceId: instanceId, templatename: templatename }, BindDataToUI, workflowManger.FailureCallback);
            }
        }
    }

    function EditDataFromCollection() {
        var StageData = workflowManger._stateData;
        var elementForCollection = [];
        var element = workflowManger._workflowStateRule.filter(function (data) {
            return data.rulesetid == ruleSetId;
        });
        var obj = {};
        if (StageData.length < element[0].statesRuleState.length) {
            for (var item = 0; item < element[0].statesRuleState.length; item++) {
                for (var stage = 0; stage < StageData.length; stage++) {
                    if (StageData[stage] == element[0].statesRuleState[item].stateName)
                        elementForCollection.push(element[0].statesRuleState[item]);

                }
            }
            for (var incol in workflowManger._workflowStateRule) {
                if (workflowManger._workflowStateRule[incol].rulesetid == ruleSetId) {
                    workflowManger._workflowStateRule[incol].statesRuleState = elementForCollection;
                }
            }
        }
        else if (StageData.length > element[0].statesRuleState.length) {

            for (var stage = 0; stage < StageData.length; stage++) {
                var flag = false;
                for (var item in element[0].statesRuleState) {
                    if (element[0].statesRuleState[item].stateName.toLowerCase() == StageData[stage].toLowerCase()) {
                        flag = true;
                        break;
                    }
                }
                if (!flag) {
                    var info = {};
                    info.stateName = StageData[stage];
                    info.mandatoryData = false;
                    info.uniquenessValidation = false;
                    info.primaryKeyValidation = false;
                    info.validations = false;
                    info.alerts = false;
                    info.basketValidation = false;
                    info.basketAlert = false;
                    elementForCollection.push(info);
                }
            }

            for (var incol in workflowManger._workflowStateRule) {
                if (workflowManger._workflowStateRule[incol].rulesetid == ruleSetId) {
                    workflowManger._workflowStateRule[incol].statesRuleState = workflowManger._workflowStateRule[incol].statesRuleState.concat(elementForCollection);
                }
            }
        }
        return element[0].statesRuleState;
    }

    function BindDataToUI(data) {
        if (workflowManger.toAddinCollection) {
            data = data.d;
            data = JSON.parse(data);
            var obj = {};
            for (var item in data.statesRuleState) {
                for (var key in data.statesRuleState[item]) {
                    if (key == "stateName" && data.statesRuleState[item][key].toLowerCase() == 'start') {
                        obj = data.statesRuleState[item];
                    }
                }
            }

            data.statesRuleState.sort(function (a, b) {
                var textA = a.stateName.toUpperCase();
                var textB = b.stateName.toUpperCase();
                return (textA < textB) ? -1 : (textA > textB) ? 1 : 0;
            });

            var element = data.statesRuleState.filter(function (ele) {
                return ele.stateName.toLowerCase() != 'start';
            });

            data.statesRuleState = element;
            data.statesRuleState.unshift(obj);
            workflowManger._workflowStateRule.push(data);
        }

        // Call to check valid states 
        data.statesRuleState = EditDataFromCollection();


        if (typeof data != "undefined" && data.statesRuleState.length > 0) {
            if (!workflowManger.__koBindingApplied) {
                if (typeof ko !== 'undefined') {
                    workflowManger._pageViewModelStateRulesPopUp = new popUpPageViewModel(data);

                    ko.applyBindings(workflowManger._pageViewModelStateRulesPopUp, $("#RulestatePopupContainer")[0]);
                }

                workflowManger.__koBindingApplied = true;

            }
            else {
                workflowManger._pageViewModelStateRulesPopUp.statesRuleState.removeAll();

                for (var item in data.statesRuleState) {
                    workflowManger._pageViewModelStateRulesPopUp.statesRuleState.push(new rulesDataDetails(data.statesRuleState[item]));
                }
            }

            var instanceId = $('.workflow-top-container').data('instanceid');
            var allowUpdate = false;

            workflowManger.getSpecificWorkflowPendingRequestDetails(instanceId).then(function (data) {
                workflowManger._isPendingRequestWorkFlow = JSON.parse(data);

                if (!workflowManger._isPendingRequestWorkFlow) {
                    allowUpdate = true;
                    $('#RuleStateErrorPopUp').css('display', 'none');
                }

                if (!allowUpdate)
                    $('#RuleStateErrorPopUp').css('display', 'inline-block');


                for (var checkbox in workflowManger._pageViewModelStateRulesPopUp.statesRuleState()) {
                    var stateRuleStateIterator = workflowManger._pageViewModelStateRulesPopUp.statesRuleState()[checkbox];
                    convertCheckBoxToToggle(stateRuleStateIterator.mandatoryDataCheckBoxID(), allowUpdate, stateRuleStateIterator.stateName);
                    convertCheckBoxToToggle(stateRuleStateIterator.uniquenessValidationCheckBoxID(), allowUpdate, stateRuleStateIterator.stateName);
                    convertCheckBoxToToggle(stateRuleStateIterator.primaryKeyValidationCheckBoxID(), allowUpdate, stateRuleStateIterator.stateName);
                    convertCheckBoxToToggle(stateRuleStateIterator.validationsCheckBoxID(), allowUpdate, stateRuleStateIterator.stateName);
                    convertCheckBoxToToggle(stateRuleStateIterator.alertsCheckBoxID(), allowUpdate, stateRuleStateIterator.stateName);
                    convertCheckBoxToToggle(stateRuleStateIterator.basketValidationCheckBoxID(), allowUpdate, stateRuleStateIterator.stateName);
                    convertCheckBoxToToggle(stateRuleStateIterator.basketAlertCheckBoxID(), allowUpdate, stateRuleStateIterator.stateName);
                }
            });
        }

        workflowManger.toAddinCollection = false;
    }

    function convertCheckBoxToToggle(checkboxID, allowUpdate, stateName, isForAction = false) {
        var id = "sm_toggle_" + checkboxID;
        if ($("body").find("#" + id).length === 0) {
            var checkBox = $("#" + checkboxID);
            checkBox[0].style.display = "none";

            //By Default NO is Selected
            var HTML = "<div id='" + id + "' class='sm_toggleContainer'>";
            if (!isForAction) {
                HTML += "<div class='sm_toggleText'>YES</div>";
                HTML += "<div class='sm_toggleText'>NO</div>";
                if (!checkBox.is(":checked")) {
                    HTML += "<div class='sm_toggleBtn' style='margin-left:30px;'>NO</div>";
                }
                else {
                    HTML += "<div class='sm_toggleBtn'>YES</div>";
                }
            }
            else {
                HTML += "<div class='sm_toggleText'>YES</div>";
                HTML += "<div class='sm_toggleText'>NO</div>";
                if (!checkBox.is(":checked")) {
                    HTML += "<div class='sm_toggleBtn' style='margin-left:30px;background-color:lightskyblue;'>NO</div>";
                }
                else {
                    HTML += "<div class='sm_toggleBtn' style='background-color:lightskyblue;'>YES</div>";
                }
            }
            HTML += "</div>";

            checkBox.after(HTML);

            if (allowUpdate) {
                checkBox.next().unbind('click').bind('click', function (e) {
                    var target = $(e.target);
                    if (target.hasClass("sm_toggleText")) {
                        target = target.parent();
                    }
                    if (target.hasClass("sm_toggleBtn")) {
                        target = target.parent();
                    }
                    target = target.find(".sm_toggleBtn");
                    checkboxFields = id.split('_');
                    if (target.css("margin-left") !== "30px") {
                        target.animate({ "margin-left": "30px" }, function () {
                            if (!isForAction) {
                                $("#" + id).find(".sm_toggleBtn").text("NO");
                                SaveRuleToggleChange(false, stateName, checkboxFields);
                            }
                            else {
                                $("#" + id).find(".sm_toggleBtn").text("NO");
                            }
                            checkBox.click();
                        });
                    }
                    else {
                        target.animate({ "margin-left": "0px" }, function () {
                            if (!isForAction) {
                                $("#" + id).find(".sm_toggleBtn").text("YES");
                                SaveRuleToggleChange(true, stateName, checkboxFields);
                            }
                            else {
                                $("#" + id).find(".sm_toggleBtn").text("YES");
                            }
                            checkBox.click();
                        });
                    }
                    e.stopPropagation();
                });
            }
        }
    }

    function SaveRuleToggleChange(value, stateName, checkboxFields) {
        checkboxval = checkboxFields[2];
        for (var item in workflowManger._workflowStateRule) {
            if (workflowManger._workflowStateRule[item].rulesetid == $('#hdnRuleSet').val()) {
                for (var ineritem in workflowManger._workflowStateRule[item].statesRuleState) {
                    if (workflowManger._workflowStateRule[item].statesRuleState[ineritem].stateName == stateName) {
                        for (var key in workflowManger._workflowStateRule[item].statesRuleState[ineritem]) {
                            if (key == checkboxval) {
                                workflowManger._workflowStateRule[item].statesRuleState[ineritem][key] = value;
                            }
                        }
                    }
                }
            }
        }

    }

    $('.workflowSetup_editableTemplate').unbind('click').on('click', function () {

        console.log("Edit Clicked");


        var radInstanceId = $(this).closest('.workflowRule-setup-panel-top_container').prev('.WorkflowRuleSetID').data('radworkflowinstanceid');
        var radInstanceName = $(this).closest('.workflowRule-setup-panel-top_container').prev('.WorkflowRuleSetID').data('radtemplatename');
        if (typeof radInstanceId == "undefined")
            radInstanceId = $('.selectedWorkflowTemplateEyeIcon').closest('.workflow-setup-card').parent().data('radworkflowinstanceid');
        if (typeof radInstanceName == "undefined")
            radInstanceName = $('.selectedWorkflowTemplateEyeIcon').closest('.workflow-setup-card').parent().data('radtemplatename');


        workflowManger.getSpecificRADWorkflowPendingRequestDetails(radInstanceId).then(function (data) {
            workflowManger._isPendingRequestRADWorkFlow = JSON.parse(data);

            if (!workflowManger._isPendingRequestRADWorkFlow) {
                $('#newWorkflowTemplateRADModal').modal('toggle');
                $('[id^=workflowSetupIframePanel_]').html('');
                $('[id^=workflowRuleIframePanel_]').html('');
                $('#newWorkflowTemplateRADModalContent').html('');
                $('#newWorkflowTemplateRADModalContent').height($(window).height() - 120);
                $('#newWorkflowTemplateRADModalContent').width($(window).width() - 60);


                var openWorkflowTemplate = {
                    workFlowName: radInstanceName,
                    identifier: "newWorkflowTemplateRADModalContent",
                    baseUrl: workflowManger._path,
                    user: workflowManger._userName,
                    moduelid: $('.workflow-top-container').data('moduleid'),
                    IsEditable: true,
                    showLandingScrrenIcon: false
                }

                $("#newWorkflowTemplateRADModal").one('shown.bs.modal', function () {
                    radworkflow.handlers.openWorkflow(openWorkflowTemplate);
                })


                //setTimeout(function () {
                //    radworkflow.handlers.openWorkflow(openWorkflowTemplate);
                //},5000)
            }
            else {
                workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'Template cannot be edited as it has pending requests.');
            }

        });
    });

    $(document).unbind('click').on('click', function (e) {
        var ViewMailTemplateContainer = false;

        $('.ViewTemplateMailContainer').each(function (index) {
            if ($($('.ViewTemplateMailContainer')[index]).css('display') !== 'none' && !(e.target.hasClass == "MailHeaderSection" || $(e.target).parents(".MailHeaderSection").length) && !(e.target.hasClass == "MailBodySection" || $(e.target).parents(".MailBodySection").length))

                if (e.target.nodeName == '#document' || e.target.className == '') {

                }
                else if (e.target.className == 'selected-item') {

                }
                else if (e.target.className == 'MailHeaderSection' || e.target.className == 'MailBodySection') {
                    if ($('.autocomplete-items').css('display') != 'none')
                        $('.autocomplete-items').css('display', 'none');
                }
                else
                    ViewMailTemplateContainer = true;
        });

        var $header = $(e.target);
        if ($header.closest('[id^=workflowSetupIframePanel]').hasClass('workflow-setup-template-panel')) {
            $(".workflowRule-setup-panel").slideUp("slow");
        }
        else if ($header.closest('[id^=workflowSetupIframePanel]').hasClass('workflow-setup-template-panel')) {
            $(".workflowRule-setup-panel").slideUp("slow");
        }
        else if ($header.hasClass("workflowRule-setup-panel")) {
            $(".workflow-setup-template-panel").slideUp("slow");
        }
        else if ($header.hasClass("workflow-setup-template-panel")) //|| $header.hasClass("radAddWorkFlowBodyPlumb")
        {
            $(".workflowRule-setup-panel").slideUp("slow");

        }
        else if ($header.hasClass("SRMShowRADWorkFlowRule")) {
            event.stopPropagation();
        }

        else if ($('#RulestatePopupContainer').css('display') !== 'none' && $header[0].id == "workflowSetup_disableDiv") {
            $('#RulestatePopupContainer').hide();
            $('#hdnRuleSet').val('');
            $('#popupcaretsymbol').css('display', 'none');
            $('#workflowSetup_disableDiv').css('display', 'none');
        }

        else if ($('#EmailstatePopupContainer').css('display') !== 'none' && $('.autocomplete-items-for-action').css('display') != 'none') {
            $('.autocomplete-items-for-action').hide();
        }




    });

});


$(document).ready(function () {
    if (workflowManger.toSelectFirstTime) {
        var configuredWorkFlowLength = $('.SRMShowRADWorkFlowRule').length;
        if (configuredWorkFlowLength > 0) {
            $('.SRMShowRADWorkFlowRule:first').click();
        }
        workflowManger.toSelectFirstTime = false;
    }


    workflowManger.adjustScreenSize();
});