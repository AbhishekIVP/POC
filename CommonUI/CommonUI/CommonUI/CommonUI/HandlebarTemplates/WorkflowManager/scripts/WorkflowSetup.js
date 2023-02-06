$(function () {
    var SelectedPrimaryDisplayAttribute;
    var primaryAttributeList = [];
    var otherAttributeList = [];
    var AlreadySelectedPrimaryAndOtherAttributes = [];
    var AlreadySelectedPrimaryAttribute;
    var AlreadySelectedOtherDisplayAttributes = [];
    var CurrentSelectedOtherAttributes = [];
    var AttributeInUse;
    //console.log('inside workflow setup');       
    workflowManger.convertCheckBoxToToggle("workflowSetupApplyOnCreate");
    workflowManger.convertCheckBoxToToggle("workflowSetupApplyOnTSUpdate");
    workflowManger.convertCheckBoxToToggle("workflowSetupApplyOnAttrEnrichment");
    workflowManger.convertCheckBoxToToggle("workflowSetupApplyOnDelete");
    var moduleId = $('.workflow-top-container').data('moduleid');
    var selectedTypeIdsforRule = [];
    var type = "";
    var isError = false;
    workflowManger._pageViewModelActionsPopUp_update = null;

    if (moduleId == 3) {
        type = "Secuirities";
    } else if (moduleId == 6) {
        type = "Entities";
    }

    var setPrimaryAttribute = function (attributeList) {

        workflowManger._primaryAttributeList = attributeList.d;
        var passedPrimaryAttributeList = "";



        workflowManger._primaryAttributeList.forEach(function (d) {
            primaryAttributeList.push({
                "value": d.AttributeId.toString(),
                "text": d.AttributeName
            })
        })

        //dropDownId, smdata, showSearch, width, callback, heading, onChangeHandler, selectedItems, isMulti, multiText, allText
        if (workflowManger._primaryAttributeList.length > 0)
            workflowManger.createSMSelectDropDown(workflowManger.controls.primaryAttributeDropdown(), primaryAttributeList, true, 180, null, null, false, [], false, null, null);

        passedPrimaryAttributeList = $('.workflow-top-container').data('primaryattributeids').toString();
        AlreadySelectedPrimaryAndOtherAttributes.push(passedPrimaryAttributeList.trim());
        AlreadySelectedPrimaryAttribute = passedPrimaryAttributeList.trim();
        console.log('first primary ', $('.workflow-top-container').data('primaryattributeids'), passedPrimaryAttributeList, workflowManger._primaryAttributeList[0]);
        if (passedPrimaryAttributeList == "") {
            console.log('first primary ', workflowManger._primaryAttributeList[0]);
            if (workflowManger._primaryAttributeList.length > 0)
                smselect.setOptionByText($('#smselect_primaryAttributeDropdown'), workflowManger._primaryAttributeList[0].AttributeName, false)
        } else {
            smselect.setOptionByValue($('#smselect_primaryAttributeDropdown'), passedPrimaryAttributeList.trim(), false)
        }
        SelectedPrimaryDisplayAttribute = smselect.getSelectedOption($('#smselect_primaryAttributeDropdown'));
    }

    var getPrimaryAttributes = function () {
        if ($('#selectedTypeList li').length > 0) {

            var includeTypeAttribute = $('#selectedTypeList li').length == 1 ? true : false;
            var selectedTypeList = $('#selectedTypeList li');
            var selectedTypeTableIds = "";
            var selectedTypeIds = "";
            var selectedTypes = [];
            selectedTypeIdsforRule = [];

            selectedTypeList.each(function (id, li) {
                //console.log(id, $(li), $(li).attr('id'))
                selectedTypeIds += $(li).attr('id') + "|";
                //set selected typeIDs for Rule Binding.
                selectedTypeIdsforRule.push(parseInt($(li).attr('id')));
            });

            workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'GetPrimaryAttributes', { moduleId: moduleId, typeIds: selectedTypeIds, includeTypeAttribute: includeTypeAttribute }, setPrimaryAttribute, workflowManger.FailureCallback);

            //Bind RuleEditor
            bindRuleEditor();
        } else {
            $('#primaryAttributeDropdown').html('');
        }
    }

    var otherAttributeSelectionChanged = function (e) {
        console.log('mutliselect...', e);
        //if(smselect.getSelectedOption($('#smselect_otherAttributeDropdown').length > 4)){
        //    Console.log("Other Attributes can not be more than 4");
        //}
    }

    var setOtherAttribute = function (attributeList) {
        workflowManger._otherAttributeList = attributeList.d;
        var passedOtherAttributeList = "";

        workflowManger._otherAttributeList.forEach(function (d) {
            otherAttributeList.push({
                "value": d.AttributeId,
                "text": d.AttributeName
            })
        });

        workflowManger.createSMSelectDropDown(workflowManger.controls.otherAttributeDropdown(), [{ 'text': 'Attributes', 'options': otherAttributeList }], true, 215, null, 'Attribute', otherAttributeSelectionChanged, [], true, 'Attributes', 'All');

        passedOtherAttributeList = $('.workflow-top-container').data('otherattributeids').trim();
        if (passedOtherAttributeList.length > 0) {
            var passedOtherAttributeArray = passedOtherAttributeList.split('|');
            passedOtherAttributeArray.forEach(function (d) {
                AlreadySelectedPrimaryAndOtherAttributes.push(d.trim());
                if (d.trim() != "")
                    AlreadySelectedOtherDisplayAttributes.push(d.trim());
                console.log('d..', d);
                smselect.setOptionByValue($('#smselect_otherAttributeDropdown'), d.trim(), false)
            })
            var SelectedOtherDisplayAttributes = smselect.getSelectedOption($('#smselect_otherAttributeDropdown'));

            var AllAttributesText = [];
            AllAttributesText = JSON.parse(JSON.stringify(SelectedPrimaryDisplayAttribute.concat(SelectedOtherDisplayAttributes)));
            workflowManger._primaryAndOtherDisplayAttributeList = AllAttributesText;


        }
    }

    var getOtherAttributes = function () {
        if ($('#selectedTypeList li').length > 0) {

            var includeTypeAttribute = $('#selectedTypeList li').length == 1 ? true : false;
            var selectedTypeList = $('#selectedTypeList li');
            var selectedTypeTableIds = "";
            var selectedTypeIds = "";
            var selectedTypes = [];

            selectedTypeList.each(function (id, li) {
                //console.log(id, $(li), $(li).attr('id'))                
                selectedTypeIds += $(li).attr('id') + "|";
            });

            workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'GetOtherAttributes', { moduleId: moduleId, typeIds: selectedTypeIds, includeTypeAttribute: includeTypeAttribute }, setOtherAttribute, workflowManger.FailureCallback);
        } else {
            $('#otherAttributeDropdown').html('');
        }
    }

    $('.setup-left-container').on('click', '#availableTypeList li', function (e) {

        var workflowActionTypeName = $('.workflow-top-container').data('workflowactiontypename').trim().toLowerCase();
        //In case of ref master only one entity type can be selected
        if ($('#selectedTypeList li').length == 1 && moduleId == 6) {
            //return;
            workflowManger.showErrorDiv('Alert', 'fail_icon.png', workflowActionTypeName.toUpperCase() + ' can be mapped to one Entity Type only.');
            return false;
        }
        else if ($('#selectedTypeList li').length == 1 && moduleId == 18) {
            //return;
            workflowManger.showErrorDiv('Alert', 'fail_icon.png', workflowActionTypeName.toUpperCase() + ' can be mapped to one Fund Type only.');
            return false;
        }
        else if ($('#selectedTypeList li').length == 1 && moduleId == 20) {
            //return;
            workflowManger.showErrorDiv('Alert', 'fail_icon.png', workflowActionTypeName.toUpperCase() + ' can be mapped to one Party Type only.');
            return false;
        }

        var TypeId = $(e.target).attr('id');
        var TypeName = $(e.target).context.innerHTML;
        if (TypeId != "") {
            workflowManger.searchWorflowTypeVsTypeIds(workflowActionTypeName, TypeId);
        }

        if (!workflowManger._isWorkFlowAlreadyMade) {

            var removeType = '#availableTypeList #' + TypeId;
            $(removeType).remove();

            if ($('ul#availableTypeList li').length == 0) {
                $('#availableTypeLabel').css('display', 'none');
            }

            $('#selectedTypeLabel').css('display', 'block');
            $('#selectedTypeList').append('<li id=' + TypeId + '><a id=' + TypeId + ' href="#">' + TypeName + '</a><i class="workFlowSetup_DeleteIcon fa fa-trash-o"></i></li>');

            getPrimaryAttributes();
            getOtherAttributes();

        }
        else {
            workflowManger.showErrorDiv('Alert', 'fail_icon.png', workflowActionTypeName.toUpperCase() + ' is already made for ' + TypeName.toUpperCase());
        }
    });

    $('.setup-left-container').on('click', '#selectedTypeList li i', function (e) {
        //console.log('e', e);

        var TypeId = $(e.target).siblings('a').attr('id');
        var TypeName = $(e.target).siblings('a')[0].innerHTML;
        var instanceId = $('.workflow-top-container').data('instanceid');
        //workflowManger.searchPendingWorflowRequests(instanceId);

        workflowManger.getSpecificWorkflowPendingRequestDetails(instanceId).then(function (data) {
            workflowManger._isPendingRequestWorkFlow = JSON.parse(data);

            if (!workflowManger._isPendingRequestWorkFlow) {

                var removeType = '#selectedTypeList #' + TypeId;
                $(removeType).remove();

                if ($('ul#selectedTypeList li').length == 0) {
                    $('#selectedTypeLabel').css('display', 'none');
                }

                $('#availableTypeLabel').css('display', 'block');
                $('#availableTypeList').append('<li id=' + TypeId + ' ><a id=' + TypeId + ' href="#">' + TypeName + '</a></li>');
                getPrimaryAttributes();
                getOtherAttributes();
            }
            else {
                workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'You cannot delete ' + TypeName + ' as it has pending requests.');
            }
        });
    });

    //This function populate and manipulate data in left panel

    var setAllTypes = function (types) {
        var type = "";

        workflowManger._types = types.d;

        $('.setup-left-container').html('');

        var html = '';
        html += '<div id="SMWorkFlowSetupSearchContainer">';
        html += '<i class="fa fa-search notify-icon" id="SMWorkFlowSetup_searchIcon" style="display: inline-block; position: relative; color: #a9a9a9; font-size: 15px;"></i>';
        html += '<input type="text" id="searchTypeId" placeholder="Search..">';
        html += '</div>';
        html += '<p id="selectedTypeLabel" style="display:none;">Selected Types' + type + '</p>';
        html += '<ul id="selectedTypeList" style="padding-left:0px;"></ul>';
        html += '<p id="availableTypeLabel">Available Types' + type + '</p>';
        html += '<ul id="availableTypeList">';

        types.d.forEach(function (type) {
            html += '<li id=' + type.TypeId + '><a id=' + type.TypeId + ' href="#">' + type.TypeName + '</a></li>';
        });

        html += '</ul>';


        $('.setup-left-container').append(html);

        $('#searchTypeId').on('input', function () {
            // Declare variables
            var input, filter, ul, li, a, i;
            input = document.getElementById('searchTypeId');
            filter = input.value.toUpperCase();
            ul = document.getElementById("availableTypeList");
            li = ul.getElementsByTagName('li');

            // Loop through all list items, and hide those who don't match the search query
            for (i = 0; i < li.length; i++) {
                a = li[i].getElementsByTagName("a")[0];
                if (a.innerHTML.toUpperCase().indexOf(filter) > -1) {
                    li[i].style.display = "";
                } else {
                    li[i].style.display = "none";
                }
            }
        });

        initSelectedTypes();
        workflowManger.adjustScreenSize();
    }

    var getAllTypes = function () {
        workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'GetAllTypes', {
            moduleId: moduleId, userName: workflowManger._userName
        }, setAllTypes, workflowManger.FailureCallback);
    }

    var bindRuleEditor = function () {
        RuleDivId = 'RRuleEditor';
        BtnSaveId = 'btnSaveRule';
        HdnRuleClassInfoId = 'hdnRuleClassInfo';

        workflowManger.callService('POST', workflowManger._path + '/CommonService.svc', 'PrepareRuleGrammarInfo', {
            type_ids: selectedTypeIdsforRule.join(','), moduleId: moduleId
        }, onSuccess_RuleEditorInitiator, workflowManger.FailureCallback);
    }

    function onSuccess_RuleEditorInitiator(result) {
        if (result.d != null) {
            if (typeof ($("#" + RuleDivId).data('ruleEngine')) !== "undefined")
                $("#" + RuleDivId).ruleEngine().data('ruleEngine').Destroy();
            $("#" + RuleDivId).empty();
            $("#" + RuleDivId).ruleEngine({
                grammarInfo: result.d, serviceUrl: workflowManger._path + '/RADXRuleEditorService.svc', ExternalFunction: RuleCompleteHandler
            });
        }
    }

    function RuleCompleteHandler(state) {
        setTimeout(function () {
            if (state) {
                $('#' + BtnSaveId).show();
                var ruleClass = $("#" + RuleDivId).ruleEngine().data('ruleEngine').getGeneratedCode();
                var ruleText = $("#ruleTxt").val();
                $('#' + HdnRuleClassInfoId).val($('<div/>').text(ruleClass[0] + "||$$||" + ruleClass[1] + "||$$||" + ruleText).html());
            }
            else
                $('#' + BtnSaveId).hide();
        }, 200);
    }


    var ShowRuleEditor = function (action) {

        $('#workflowRulesTemplate').html('');
        var ruleEditorPanel = "";
        ruleEditorPanel += "<div ID='panelRuleCreation' class='SMWorkFlowRuleSetup_PanelRuleCreation' ClientIDMode='Static'>";
        ruleEditorPanel += "<div ID='RuleEditorCreation' class='SMWorkFlowRuleSetup_PanelRuleCreation' ClientIDMode='Static'>";
        ruleEditorPanel += "<div title='Click to Cancel' ID='SMWorkFlowRuleSetup_Close_Overlay' class='SMWorkFlowRuleSetup_Close_Overlay'><i class='fa fa-close' id='SMWorkFlowRuleSetup_Close_Icon'></i></div>";

        ruleEditorPanel += "<div id='SMWorkflowRuleSetup_priority_save_container'>";

        ruleEditorPanel += "<div id='SMWorkflowRuleSetup_priority_show'>";
        ruleEditorPanel += "<div ID='lb1Priority' class='SMPanelStyle'>Priority :</div>";
        ruleEditorPanel += "<input type='number' ID='txtPriority' class='input SMWorkflowRuleSetup_textline' ToolTip='Priority Level'";
        ruleEditorPanel += "Width='25px' Style='text-align: center;' MaxLength='3' TabIndex='1' onkeydown='javascript: return event.keyCode == 69 ? false : true' >";
        ruleEditorPanel += "</div>";

        ruleEditorPanel += "<div class='ButtonandErrorDisplay'>";
        ruleEditorPanel += "<input type='button' ID='btnSaveRule' class='SMRuleSetup_SaveButton' value='Save Rule'";
        ruleEditorPanel += "class='button' ToolTip='Click to Save' ClientIDMode='Static'/>";
        ruleEditorPanel += "<input type='button' ID='btnCancel' value='Reset'";
        ruleEditorPanel += "class='button SMWorkflowRuleSetup_ResetButton' ToolTip='Click to Reset Rule' />";
        ruleEditorPanel += "<div id='divErrorSaveRule' class='SMRuleSetup_ErrorPanels'>";
        ruleEditorPanel += "</div>";
        ruleEditorPanel += "<div id='divErrorSavePriority' class='SMRuleSetup_ErrorPanels'>";
        ruleEditorPanel += "</div>";

        ruleEditorPanel += "</div>";
        ruleEditorPanel += "</div>";

        ruleEditorPanel += "<table id='tableRuleCreation' cellpadding='0' cellspacing='0' width='100%' style='table-layout: fixed;'>";
        ruleEditorPanel += "<tr class='ssf_panel'></tr>";
        ruleEditorPanel += "<tr class='ssf_panel'>";

        ruleEditorPanel += "<td class='dataRow' align='left'>";
        ruleEditorPanel += "</td>";
        ruleEditorPanel += "</tr>";

        ruleEditorPanel += "<tr class='ssf_panel'>";
        ruleEditorPanel += "<td class='dataRow' colspan='2'>";
        ruleEditorPanel += "&nbsp;";
        ruleEditorPanel += "</td>";
        ruleEditorPanel += "</tr>";
        ruleEditorPanel += "<tr>";
        ruleEditorPanel += "<td colspan='2'>";
        ruleEditorPanel += "<div id='aaa' style='height: 250px;'>";
        ruleEditorPanel += "<div id='RRuleEditor' ClientIDMode='Static'";
        ruleEditorPanel += "</div>";
        ruleEditorPanel += "</div>";
        ruleEditorPanel += "</td>";
        ruleEditorPanel += "</tr>";

        ruleEditorPanel += "</table>";
        ruleEditorPanel += "</div>";

        ruleEditorPanel += "<div class='WorkFlowTemplateViewForRule_Container'>";

        ruleEditorPanel += "<div class='addNewWorkflowTemplateButton_AddRule_Container' >";
        ruleEditorPanel += "<div id='addNewWorkflowTemplateButton_AddRule' data-container='body' data-toggle='popover' title='New Workflow Template'>";
        ruleEditorPanel += "<i class='fa fa-plus-circle SMWorkFlowSetup_arrowAddTemplate'> </i>";
        ruleEditorPanel += "<input id='SMWorkFlowSetup_addNewTemplate_Add_rule' value='Add New Template' type='butto' class='SMTextNew SMWorkFlowSetup_addNewTemplate' title='Click to Add Template '>";
        ruleEditorPanel += "</div>";
        ruleEditorPanel += "</div>";

        ruleEditorPanel += "<div id='WorkFlowTemplateViewForRule'>";
        ruleEditorPanel += "</div>";

        ruleEditorPanel += "</div>";


        ruleEditorPanel += "</div>";

        $('#workflowRulesTemplate').append(ruleEditorPanel);
        workflowManger.generateWorkflowRuleTemplateView('WorkFlowTemplateViewForRule')
        bindRuleEditorHandlers();

    };

    $("#SMWorkFlowSetup_btnAddRule").unbind("click").click(function (e) {
        var instanceId = $('.workflow-top-container').data('instanceid');
        //workflowManger.searchPendingWorflowRequests(instanceId);
        //$('#workflowRuleSetup_disableDiv').show();

        workflowManger.getSpecificWorkflowPendingRequestDetails(instanceId).then(function (data) {
            workflowManger._isPendingRequestWorkFlow = JSON.parse(data);

            if (!workflowManger._isPendingRequestWorkFlow) {

                if ($('#selectedTypeList li').length == 0) {
                    workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'Please select atleast one type to add rule.');
                }
                else {
                    bindRuleEditor();
                    $('#workflowRulesTemplate').slideToggle();
                    $('.workflow-setup-ruleEditortemplate-panel').css('display', 'none');
                    $('.workflow-setup-ruleEditortemplate-panel').html('');
                    $('.workflowCheck').removeClass('selectedWorkflowTemplate');
                    $('.workflow-template-eye').removeClass('selectedWorkflowTemplateEyeIcon');
                    $('.workflow-rule-editor-template-eye').removeClass('selectedWorkflowTemplateEyeIcon');
                    $('[id^=workflowRuleIframePanel_]').html('');
                    $('.workflow-setup-template-panel').css('display', 'none');
                }
                workflowManger._isUpdateRule = false;
            }
            else {
                workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'You cannot add rules as Workflow has pending requests.');
            }
        });
    });

    function bindRuleEditorHandlers() {

        $("#btnSaveRule").unbind("click").click(function (e) {

            debugger;
            var selectedRADWorkflow = $('.selectedWorkflowTemplate');
            if (selectedRADWorkflow.length > 0) {
                if ($('#txtPriority').val().trim() == "") {
                    workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'Please enter priority.');
                    return false;
                }
                if ($('#txtPriority').val().trim() != "") {

                    var element = $(".WorkflowRuleSetID").filter(function () {
                        return $.trim($(this).data('priority')) == $('#txtPriority').val().trim()
                    });

                    if (element.length > 0) {
                        if (workflowManger._isUpdateRule) {
                            var priorityCheck = element.data('priority');
                            var hdnPriority = $('#hdnPriority').val();
                            if (priorityCheck != hdnPriority) {
                                workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'Priority should be unique for each rule.');
                                return false;
                            }
                        }
                        else {
                            workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'Priority should be unique for each rule.');
                            return false;
                        }
                    }

                    if (parseInt($('#txtPriority').val().trim()) < 1) {
                        workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'Priority cannot be zero or negative.');
                        return false;
                    }

                    var rad_workflow_instance_id = $(selectedRADWorkflow).closest('.workflow-setup-card').parent().data('radworkflowinstanceid').toString();
                    var module_id = $(selectedRADWorkflow).closest('.workflow-setup-card').parent().data('moduleid').toString();
                    var guid = $('#hdnGuid').val().toString();
                    var hdnRuleClassInfo = $("#hdnRuleClassInfo").val();
                    //var ruleName = $('#txtRuleName').val();
                    var priority = $('#txtPriority').val();
                    var ruleSetId = $('#hdnRuleSet').val();
                    var ruleInfoVal = priority + '|' + ruleSetId;
                    var workflow_instance_id = $('.workflow-top-container').data('instanceid');


                    if (typeof workflow_instance_id != "undefined" || workflow_instance_id != null) {
                        workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'btn_SaveRuleInRADWorkFlowClick', {
                            hdnRuleClassInfoValue: hdnRuleClassInfo, module: module_id, ruleInfo: ruleInfoVal, Guid: guid, rad_workflow_id: rad_workflow_instance_id, updateCase: workflowManger._isUpdateRule, workFlowInstanceID: workflow_instance_id
                        }, workflowManger.getWorkflowRulesTemporary, workflowManger.FailureCallback);
                        $("#SMWorkFlowRuleSetup_Close_Icon").trigger('click');
                    }
                }
            }
            else
                workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'Please select one template.');

        });

        $("#SMWorkFlowRuleSetup_Close_Icon").unbind("click").click(function (e) {
            $('#workflowRulesTemplate').hide();
            //$('#workflowRuleSetup_disableDiv').hide();
            workflowManger._isUpdateRule = true;
            $('#txtPriority').val('');
            $('#hdnPriority').val('');
            $('.workflow-template-eye').removeClass('selectedWorkflowTemplateEyeIcon');
            $('.workflow-rule-editor-template-eye').removeClass('selectedWorkflowTemplateEyeIcon');
            $('.workflowCheck').removeClass('selectedWorkflowTemplate');
            $('.workflow-setup-template-panel').css('display', 'none');
            $('.workflow-setup-template-panel').html('');
            $('[id^=workflowRuleIframePanel_]').html('');

            $('.workflow-setup-ruleEditortemplate-panel').css('display', 'none');
            $('.workflow-setup-ruleEditortemplate-panel').html('');
            var configuredWorkFlowLength = $('.SRMShowRADWorkFlowRule').length;
            if (configuredWorkFlowLength > 0) {
                $('.SRMShowRADWorkFlowRule:first').click();
            }

        });

        $('#btnCancel').unbind("click").click(function (e) {
            $('#ruleTxt').val('');
            $('#txtPriority').val('');
            $('#hdnPriority').val('');
            $('.workflowCheck').removeClass('selectedWorkflowTemplate');
            //$('.workflow-rule-editor-template-eye').removeClass('selectedWorkflowTemplateEyeIcon');
            $('#autcompleteContainer').hide();
        });

        $('#SMWorkFlowSetup_addNewTemplate_Add_rule').unbind("click").click(function (e) {
            $('#SMWorkFlowSetup_addNewTemplate').trigger('click');
        });

    }

    $('#SMWorkFlowSetup_addNewTemplate').unbind('click').bind('click', function () {
        $('#newWorkflowTemplateModal').modal('toggle');
        $('.workflow-template-eye').removeClass('selectedWorkflowTemplateEyeIcon');
        $('#newWorkflowTemplateName').val('');
    })

    $('#SMWorkFlowSetup_btnAddDefaultRule').unbind('click').bind('click', function () {
        $('.workflow-setup-box').css('display', 'block');
        $('.workflowRulesViewTemplate').css('display', 'none');
        $('.workflow-template-eye').removeClass('selectedWorkflowTemplateEyeIcon');
        var radworkflowinstanceid = $("#workflowRulesViewTemplate").children().closest('.WorkflowRuleSetID').data('radworkflowinstanceid');
        $('#workflowTemplates > #' + radworkflowinstanceid).find('.workflowCheck').addClass('selectedWorkflowTemplate');
        $('#workflowTemplates > #' + radworkflowinstanceid).find('.workflow-template-eye').trigger('click');
    })



    $('#creatNewWorkflowTemplate').unbind('click').bind('click', function () {
        $('#newWorkflowTemplateModal').modal('toggle');
        $('.workflow-setup-ruleEditortemplate-panel').css('display', 'none');
        $('.workflow-setup-template-panel').css('display', 'none');


        var newWorkflowTemplate = {
            workFlowName: $('#newWorkflowTemplateName').val(),
            identifier: "newWorkflowTemplateRADModalContent",
            baseUrl: workflowManger._path,
            user: workflowManger._userName,
            moduelid: $('.workflow-top-container').data('moduleid'),
            IsEditable: true,
            showLandingScrrenIcon: false,
            serviceCallback: setErrorMsg
        }
        // radworkflow.handlers.createWorkflow(newWorkflowTemplate);
        $('#newWorkflowTemplateRADModal').modal('toggle');
        $("#newWorkflowTemplateRADModal").one('shown.bs.modal', function () {
            radworkflow.handlers.createWorkflow(newWorkflowTemplate);
        });
    });

    $('.close').unbind('click').bind('click', function () {

        $('#newWorkflowTemplateRADModal').modal('toggle');
        $('[id^=workflowRuleIframePanel_]').html('');
        $('.workflow-setup-ruleEditortemplate-panel').css('display', 'none');
        $('.workflow-setup-template-panel').css('display', 'none');

        $('[id^=workflowSetupRuleEditorIframePanel_]').html('');
        $('.workflow-rule-editor-template-eye').removeClass('selectedWorkflowTemplateEyeIcon');

        var configuredWorkFlowLength = $('.SRMShowRADWorkFlowRule').length;
        if (configuredWorkFlowLength > 0) {
            $('.SRMShowRADWorkFlowRule:first').click();
        }
    });

    var isInvalid = function () {
        var selectedPrimaryAttribute = "";
        var selectedOtherAttributes = "";


        smselect.getSelectedOption($('#smselect_otherAttributeDropdown')).forEach(function (item) {
            selectedOtherAttributes += item.value + '|';
        })

        selectedPrimaryAttribute = smselect.getSelectedOption($('#smselect_primaryAttributeDropdown'))[0].value;

        var selectedAttrList = selectedOtherAttributes.split('|');
        var element = selectedAttrList.filter(function (data) {
            return data == selectedPrimaryAttribute;
        });

        if ($('#selectedTypeList li').length == 0) {
            workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'Please select atleast one type.');
        }
        else if (smselect.getSelectedOption($('#smselect_primaryAttributeDropdown')).length == 0) {
            workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'Please select primary attribute.');
        }
        else if (smselect.getSelectedOption($('#smselect_otherAttributeDropdown')).length > 4) {
            workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'Please select only four attributes in other display attributes.');
        }
        else if ($('.WorkflowRuleSetID').length < 1) {
            workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'Please add atleast one rule.');
        }
        else if (element.length > 0) {
            workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'You cannot select same attribute as Primary Display Attribute and Other Display Attribute.');
        }

        else
            return true;

        //console.log('inside validate',isError);
        //return isError;
    }

    $('#workflowSetupSaveBtn').on('click', function () {
        console.log('workflowSetupSaveBtn is clicked');
        isError = false;
        workflowManger._attributeError = false;
        var workflowInstanceId = $('.workflow-top-container').data('instanceid');
        
        if (isInvalid()) {
            var result = validateEmailActionsCollection(workflowInstanceId)
            if (!result) {

                var AllAttributes = primaryAttributeList.concat(otherAttributeList);
                var AllAttributesDict = {};
                for (var i = 0; i < AllAttributes.length; i++) {
                    AllAttributesDict[AllAttributes[i].value] = AllAttributes[i].text;
                }
                for (var key in AllAttributesDict) {
                    if (AttributeInUse == key) {
                        AttributeInUse = AllAttributesDict[key];
                        break;
                    }
                }

                workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'Please remove ' + AttributeInUse + ' from email body and then update the Others Attribute dropdown');
                AttributeInUse = null;
                workflowManger._attributeError = true;

                return;
            }
            validateRuleStateCollection(workflowInstanceId);
        }
    });

    $('#newWorkflowTemplateRADModal').on('hidden.bs.modal', function (e) {
        workflowManger.getWorkflowTemplates(true);
        console.log('modal closed.');

    });

    function popUpPageViewModelActions(data) {
        debugger;
        var self = this;
        self.actionsState = ko.observableArray();
        self.RulePriority = data.RulePriority;

        if (typeof (data.Actions) != 'undefined') {
            for (var item in data.Actions) {
                self.actionsState.push(new ActionsDataDetails(data.Actions[item]));
            }
        }
    }

    function ActionsDataDetails(data) {
        debugger;
        SelectedPrimaryDisplayAttribute = AlreadySelectedPrimaryAttribute;
        SelectedOtherDisplayAttributes = AlreadySelectedOtherDisplayAttributes;
        allSelectedPrimaryAndOtherAttributes = JSON.parse(JSON.stringify(SelectedPrimaryDisplayAttribute.concat(SelectedOtherDisplayAttributes)));
        var self = this;

        self.checkboxForEachAction = ko.observable(data.CheckBoxForEachAction);

        self.ActionName = data.ActionName;

        self.keepApplicationURLInTheFooter = ko.observable(data.KeepApplicationURLInTheFooter);

        self.sendConsolidatedEmailForBulkAction = ko.observable(data.SendConsolidatedEmailForBulkAction);

        self.To = ko.observable(data.To);

        self.KeepCreatorInCC = ko.observable(data.KeepCreatorInCC);

        self.MailBodyTitle = ko.observable(data.MailBodyTitle);

        self.MailBodyContent = ko.observable(data.MailBodyContent);

        self.Subject = ko.observable(data.Subject);

        self.BulkSubject = ko.observable(data.BulkSubject);

        self.availableAttributeDropdownId = ko.computed(function () {
            return self.ActionName.replace(/\s/g, "").concat("dropdown");
        });

        self.ElementWithId = function () {
            return $('#' + self.availableAttributeDropdownId());
        }

        if (moduleId == 3)
            moduleForWorflow = 'sec';
        else
            moduleForWorflow = 'ref';

        if (moduleForWorflow == 'sec')
            self.DataSectionAttributes = ko.observableArray(["Security Type", "Security ID"]);
        if (moduleForWorflow == 'ref')
            self.DataSectionAttributes = ko.observableArray(["Entity Type", "Entity Code"]);

        self.PrimaryDisplayAttribute = ko.observable(smselect.getSelectedOption($('#smselect_primaryAttributeDropdown')));


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
                            for (var j = 0; j < otherAttributeList.length; j++) {
                                if (attribute == otherAttributeList[j].value)
                                    self.DataSectionAttributes.push(otherAttributeList[j].text);
                            }
                            break;
                    }

                }
            }

        })

    }

    var validateEmailActionsCollection = function (instanceId) {
        AttributeInUse = null;
        var moduleId = $('.workflow-top-container').data('moduleid');
        var OtherAttributes = smselect.getSelectedOption($('#smselect_otherAttributeDropdown'));
        var PrimaryAttribute = smselect.getSelectedOption($('#smselect_primaryAttributeDropdown'));
        var OtherAttributesId = [];
        for (let i = 0; i < OtherAttributes.length; i++)
            OtherAttributesId.push(OtherAttributes[i].value);

        var OtherAttributesIdJoin = OtherAttributesId.sort().join(',');
        var AlreadySelectedOtherDisplayAttributesJoin = AlreadySelectedOtherDisplayAttributes.sort().join(',');

        if (instanceId != 0 && (PrimaryAttribute[0].value != AlreadySelectedPrimaryAttribute || !(OtherAttributesIdJoin === AlreadySelectedOtherDisplayAttributesJoin))) {
            debugger;
            workflowManger.rulePriorityVsName_update = [];
            workflowManger._workflowAction_update = [];
            var workflowDetails = [];
            $('.workflow-rules-setup-card').each(function (i, ele) {
                var obj = {};
                obj.workflowName = $('.workflow-top-container').data('name')
                obj.rulePriority = ($(ele).parent().data('priority'));
                workflowManger.rulePriorityVsName_update.push(obj);
            });
            if (workflowManger.rulePriorityVsName_update.length > 0) {
                for (var item in workflowManger.rulePriorityVsName_update) {
                    var wd = {
                        "RulePriority": workflowManger.rulePriorityVsName_update[item].rulePriority,
                        "WorkflowName": workflowManger.rulePriorityVsName_update[item].workflowName
                    };
                    workflowDetails.push(wd);
                }
                workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'GetWorkflowEmailActions', { moduleId: moduleId, workflowDetails: workflowDetails }, function (ActionData) {
                    debugger;
                    if (typeof ActionData != "undefined") {
                        for (var i = 0; i < ActionData.d.length; i++)
                            workflowManger._workflowAction_update.push(ActionData.d[i]);

                        for (var i = 0; i < ActionData.d.length; i++) {
                            var ActionData_update = ActionData.d[i];
                            workflowManger._pageViewModelActionsPopUp_update = new popUpPageViewModelActions(ActionData_update);

                            if (!(OtherAttributesIdJoin === AlreadySelectedOtherDisplayAttributesJoin)) {
                                for (var i = 0; i < OtherAttributes.length; i++)
                                    CurrentSelectedOtherAttributes.push(OtherAttributes[i].value);

                                if (!(CurrentSelectedOtherAttributes.sort().join(',') === AlreadySelectedOtherDisplayAttributesJoin)) {

                                    var MismatchValues = AlreadySelectedOtherDisplayAttributes.filter(function (obj) { return CurrentSelectedOtherAttributes.indexOf(obj) == -1; });
                                    for (var i = 0; i < workflowManger._workflowAction_update.length; i++) {
                                        var Actions = workflowManger._workflowAction_update[i].Actions;
                                        for (var j = 0; j < Actions.length; j++) {
                                            if (Actions[j].DataSectionAttributes != null) {
                                                var DataSectionAttributes = Actions[j].DataSectionAttributes;
                                                for (var k = 0; k < DataSectionAttributes.length; k++) {
                                                    if (MismatchValues.includes(DataSectionAttributes[k]) && MismatchValues != "") {
                                                        AttributeInUse = DataSectionAttributes[k];
                                                        return false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (AttributeInUse == null) {
                                        if (PrimaryAttribute[0].value != AlreadySelectedPrimaryAttribute) {
                                            SaveModifiedPrimaryAttributeValue(PrimaryAttribute, ActionData_update);
                                        }
                                    }
                                    if (AttributeInUse != null)
                                        return false;
                                }
                            }
                            else if (PrimaryAttribute[0].value != AlreadySelectedPrimaryAttribute && OtherAttributesIdJoin === AlreadySelectedOtherDisplayAttributesJoin) {
                                SaveModifiedPrimaryAttributeValue(PrimaryAttribute, ActionData_update);
                            }
                        }
                    }

                }, workflowManger.FailureCallback, '', '', '', false);
            }

        }
        if (AttributeInUse != null)
            return false;
        else
            return true;
    }

    function SaveModifiedPrimaryAttributeValue(PrimaryAttribute, ActionData) {
        debugger;
        var PrimaryAttributesDict = {};
        for (var i = 0; i < primaryAttributeList.length; i++) {
            PrimaryAttributesDict[primaryAttributeList[i].value] = primaryAttributeList[i].text;
        }
        var AlreadySelectedPrimaryAttributeText = PrimaryAttributesDict[AlreadySelectedPrimaryAttribute];

        var PrimaryDisplayAttributeText;
        PrimaryDisplayAttributeText = JSON.parse(JSON.stringify(PrimaryAttribute[0].text));

        var len = ActionData.Actions.length;
        while (len > 0) {
            var selectedAction = workflowManger._pageViewModelActionsPopUp_update.actionsState()[len - 1];
            selectedAction.PrimaryDisplayAttribute(PrimaryDisplayAttributeText);
            if (selectedAction.DataSectionAttributes().includes(AlreadySelectedPrimaryAttributeText)) {
                const index = selectedAction.DataSectionAttributes().indexOf(AlreadySelectedPrimaryAttributeText);
                selectedAction.DataSectionAttributes.splice(index, 0, PrimaryAttribute[0].text);
                selectedAction.DataSectionAttributes.splice(index + 1, 1);
            }
            len--;
        }
        workflowManger._primaryDisplayAttributeList = PrimaryAttribute[0].value;
        var OtherAttributes = smselect.getSelectedOption($('#smselect_otherAttributeDropdown'));
        var allAttributes = PrimaryAttribute.concat(OtherAttributes);
        for (var i = 0; i < allAttributes.length; i++)
            DataAttributesDictionary[allAttributes[i].text] = allAttributes[i].value;

        workflowManger._emailTemplatefromUI = false;

        $('.SaveConfiguration').trigger('click');

        return true;
    }

    var validateRuleStateCollection = function (instanceId) {
        if (instanceId == 0) {
            var collection = workflowManger._workflowStateRule;
            workflowManger.ruleSetIDVsName = [];
            var ruleSetIDVsNameToFetch = [];
            var found = false;

            $('.workflow-rules-setup-card').each(function (i, ele) {
                var obj = {};
                obj.rulesetid = ($(ele).parent().data('rulesetid'));
                obj.templatename = ($(ele).parent().data('radtemplatename'));
                workflowManger.ruleSetIDVsName.push(obj);
            });

            if (collection.length == 0 && workflowManger.ruleSetIDVsName.length > 0) {
                for (var item in workflowManger.ruleSetIDVsName) {
                    workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'GetStateRuleData', { ruleSetId: workflowManger.ruleSetIDVsName[item].rulesetid, templatename: workflowManger.ruleSetIDVsName[item].templatename }, addToCollectionAndSave, workflowManger.FailureCallback);
                }
            }
            else if (collection.length <= workflowManger.ruleSetIDVsName.length) {

                for (var i = 0; i < workflowManger.ruleSetIDVsName.length; i++) {
                    var obj = {};
                    for (var j = 0; j < workflowManger._workflowStateRule.length; j++) {
                        if (workflowManger.ruleSetIDVsName[i].rulesetid != workflowManger._workflowStateRule[j].rulesetid) {
                            found = false;
                        }
                        else {
                            found = true;
                            break;
                        }
                    }
                    if (!found) {
                        obj.rulesetid = workflowManger.ruleSetIDVsName[i].rulesetid;
                        obj.templatename = workflowManger.ruleSetIDVsName[i].templatename;
                        ruleSetIDVsNameToFetch.push(obj);

                    }
                }

                if (ruleSetIDVsNameToFetch.length > 0) {
                    for (var item in ruleSetIDVsNameToFetch) {
                        workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'GetStateRuleData', { ruleSetId: ruleSetIDVsNameToFetch[item].rulesetid, templatename: ruleSetIDVsNameToFetch[item].templatename }, addToCollectionAndSave, workflowManger.FailureCallback);
                    }
                }
                else {
                    addToCollectionAndSave();
                }
            }
        }
        else
            addToCollectionAndSave(); //for updation only
    }

    function addToCollectionAndSave(data) {
        if (typeof data != "undefined") {
            data = data.d;
            data = JSON.parse(data);
            workflowManger._workflowStateRule.push(data);
        }

        var workflowInstanceId = $('.workflow-top-container').data('instanceid');

        if (validToInsertWorkflow(workflowInstanceId)) {

            var selectedTypeIds = "";
            var selectedPrimaryAttribute = "";
            var selectedOtherAttributes = "";


            var selectedTypeList = $('#selectedTypeList li');
            selectedTypeList.each(function (id, li) {
                console.log(id, $(li), $(li).attr('id'))
                selectedTypeIds += $(li).attr('id') + "|";
            });

            smselect.getSelectedOption($('#smselect_otherAttributeDropdown')).forEach(function (item) {
                selectedOtherAttributes += item.value + '|';
            })

            selectedPrimaryAttribute = smselect.getSelectedOption($('#smselect_primaryAttributeDropdown'))[0].value;

            allSelectedAttributes = JSON.parse(JSON.stringify(smselect.getSelectedOption($('#smselect_otherAttributeDropdown')).concat(smselect.getSelectedOption($('#smselect_primaryAttributeDropdown'))[0])));


            var workflowWorkflowIsCreate = $('.workflow-top-container').data('workflowiscreate');
            var workflowWorkflowIsTimeSeries = $('.workflow-top-container').data('workflowistimeseries');
            var workflowRaiseForNonEmptyValue = $('.workflow-top-container').data('raisefornonemptyvalue');
            var workflowWorkflowIsDelete = $('.workflow-top-container').data('workflowisdelete');

            var selectedAttrList = selectedOtherAttributes.split('|');
            var element = selectedAttrList.filter(function (data) {
                return data == selectedPrimaryAttribute;
            });
            if (element.length > 0) {
                workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'You cannot select same attribute as Primary Display Attribute and Other Display Attribute.');
                isError = true;
            }

            else {
                if ($('.workflow-top-container').data('action') == 'save') {
                    console.log('inside save');
                    var workflowName = $('.workflow-top-container').data('name');
                    var workflowModuleId = $('.workflow-top-container').data('moduleid');
                    var workflowModuleName = $('.workflow-top-container').data('modulename');
                    var workflowWorkflowActionTypeId = $('.workflow-top-container').data('workflowactiontypeid');
                    var workflowWorkflowActionTypeName = $('.workflow-top-container').data('workflowactiontypename');


                    workflowManger.insertWorkflow(workflowName, workflowModuleId, workflowModuleName, workflowWorkflowActionTypeId, workflowWorkflowActionTypeName, workflowWorkflowIsCreate,
                        workflowWorkflowIsTimeSeries, workflowRaiseForNonEmptyValue, workflowWorkflowIsDelete, selectedTypeIds, selectedPrimaryAttribute, selectedOtherAttributes);
                }
                else if ($('.workflow-top-container').data('action') == 'update') {

                    console.log('e//', selectedTypeIds, selectedOtherAttributes, selectedPrimaryAttribute);
                    workflowManger.updateWorkflow(workflowInstanceId, selectedTypeIds, selectedPrimaryAttribute, selectedOtherAttributes);
                }
            }
        }
    }

    function validToInsertWorkflow(workflowInstanceId) {
        if (workflowInstanceId == 0) {
            var count = 0;
            if ((workflowManger.ruleSetIDVsName.length == workflowManger._workflowStateRule.length) && (workflowManger.ruleSetIDVsName.length != 0)) {
                for (var i = 0; i < workflowManger.ruleSetIDVsName.length; i++) {
                    for (var j = 0; j < workflowManger._workflowStateRule.length; j++) {
                        if (workflowManger.ruleSetIDVsName[i].rulesetid == workflowManger._workflowStateRule[j].rulesetid) {
                            count++;
                        }
                    }
                }
                if (count == workflowManger._workflowStateRule.length)
                    return true;
                else
                    return false;
            }
            else
                return false;
        }
        else
            return true; //for updation
    }

    var setErrorMsg = function (msg) {

        if (msg.toString().toLowerCase() == 'false') {
            $('#newWorkflowTemplateRADModalContent').html('');
            workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'Template name already exists.');
            $('#newWorkflowTemplateRADModal').modal('toggle');
        }
        else {
            $('[id^=workflowSetupIframePanel_]').html('');
            $('[id^=workflowRuleIframePanel_]').html('');
            $('#newWorkflowTemplateRADModalContent').html('');
        }
    }

    var initSelectedTypes = function () {

        var types = $('.workflow-top-container').data('typeids');
        console.log("inside init selected", types);

        var selectedTypeInOtherWorkFlow = [];
        if (!workflowManger.isEmpty(workflowManger._worflowTypeVsTypeIds)) {
            var workflowActionTypeName = $('.workflow-top-container').data('workflowactiontypename').toLowerCase();
            if (typeof workflowManger._worflowTypeVsTypeIds[workflowActionTypeName] != "undefined") {
                workflowManger._worflowTypeVsTypeIds[workflowActionTypeName].forEach(function (d) {
                    selectedTypeInOtherWorkFlow.push(parseInt(d));
                });
            }
        }

        if (types.length > 0) {
            var selectedTypes = types.split('|');
            var selectedTypesNumberic = [];
            selectedTypes.forEach(function (d) {
                selectedTypesNumberic.push(parseInt(d));
            });

            availableTypeList = $('#availableTypeList li');
            //console.log('init selected types', selectedTypesNumberic, availableTypeList, types);


            availableTypeList.each(function (id, li) {
                if (selectedTypesNumberic.indexOf(parseInt($(li).attr('id'))) != -1) {
                    var TypeId = $(li).attr('id');
                    //var SecTypeId = $(e.target).data('sectypeid');
                    var TypeLink = $(li).context.innerHTML;
                    var removeType = '#availableTypeList #' + TypeId;
                    $(removeType).remove();

                    if ($('ul#availableTypeList li').length == 0) {
                        $('#availableTypeLabel').css('display', 'none');
                    }

                    var listItem = '<li id=' + TypeId + '>' + TypeLink + '<i class="workFlowSetup_DeleteIcon fa fa-trash-o"></i></li>';
                    $('#selectedTypeLabel').css('display', 'block');
                    $('#selectedTypeList').append(listItem);
                }
                if (selectedTypeInOtherWorkFlow.indexOf(parseInt($(li).attr('id'))) != -1) {
                    var TypeId = $(li).attr('id');
                    var removeType = '#availableTypeList #' + TypeId;
                    $(removeType).remove();
                }

            });

            getPrimaryAttributes();
            getOtherAttributes();
        }
        else if (selectedTypeInOtherWorkFlow.length > 0) {
            availableTypeList = $('#availableTypeList li');
            //console.log('init selected types', selectedTypesNumberic, availableTypeList, types);

            availableTypeList.each(function (id, li) {
                if (selectedTypeInOtherWorkFlow.indexOf(parseInt($(li).attr('id'))) != -1) {
                    var TypeId = $(li).attr('id');
                    var removeType = '#availableTypeList #' + TypeId;
                    $(removeType).remove();
                }
            });
        }
    }

    var initWorkflowTemplate = function () {
        workflowManger.generateWorkflowTemplateView('workflowTemplates');
    }

    var initWorkflowSetup = function () {
        if ($('.workflow-top-container').data('workflowactiontypename').indexOf('Leg') == -1 && $('.workflow-top-container').data('workflowactiontypename').indexOf('Attribute') == -1) {
            $('.applyOnCreate').css('display', 'none');
            $('.applyOnTS').css('display', 'none');
            $('.applyOnEnrich').css('display', 'none');
            $('.applyOnDelete').css('display', 'none');

            getAllTypes();
            initWorkflowTemplate();
            ShowRuleEditor();
            //initWorkFlowRuleEditor();
        }
    }

    initWorkflowSetup();

})