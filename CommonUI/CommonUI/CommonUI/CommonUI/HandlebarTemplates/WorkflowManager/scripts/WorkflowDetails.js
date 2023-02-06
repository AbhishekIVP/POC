$(function () {

    //Add new workflow button handler    
    $('#addNewWorkflowButton').unbind('click').bind('click', function (event) {
        //console.log('button click');
        // workflowManger.generateAddNewWorkflowView();
        var element = "addNewWorkflowButton";
        $('#newWorkflowName').val('');
        smselect.setOptionByText($('#smselect_newWorkflowModuleDropdown'), workflowManger._workflowModules[0].text, false);

        var popUpOffset = $(event.target).closest('#' + element).offset();
        $(".add-new-workflow-form").css({ top: popUpOffset.top + 30, left: (popUpOffset.left - 150), position: 'absolute', display: 'block' });
        event.stopPropagation();
    });

    $('#exportWorkflowButton').unbind('click').bind('click', function (event) {
        $("#exportWorkflowServerBtn").trigger('click');
        event.stopPropagation();
    });

    //Edit link handler
    $('.hover-link').unbind('click').bind('click', function () {

        debugger;
        //console.log('link clikced...', this, $(this).data('instanceid'));
        //data-typeids="{{workflow.TypeIds}}" data-primaryattributeids="{{workflow.PrimaryAttributeIds}}"
        //data-otherattributeids="{{workflow.OtherAttributeIds}}"
        var workflow = {
            "instanceId": $(this).data('instanceid'), name: $(this).data('name'), moduleId: $(this).data('moduleid'), moduleName: $(this).data('modulename'), workflowActionTypeId: $(this).data('workflowactiontypeid'),
            workflowActionTypeName: $(this).data('workflowactiontypename'), workflowIsCreate: $(this).data('workflowiscreate'), workflowIsUpdate: $(this).data('workflowisupdate'), workflowIsTimeSeries: $(this).data('workflowistimeseries'),
            raiseForNonEmptyValue: $(this).data('raisefornonemptyvalue'), workflowIsDelete: $(this).data('workflowisdelete'), TypeIds: $(this).data('typeids'), PrimaryAttributeIds: $(this).data('primaryattributeids'),
            OtherAttributeIds: $(this).data('otherattributeids')
        }
        //
        workflowManger.generateWorkflowSetupView('update', workflow);
    });


    $(".workflow-card").mouseenter(function () {
        $(this).find(".workflowHeaderRight").css('display', 'inline-block');
    }).mouseleave(function () {
        $(this).find(".workflowHeaderRight").css('display', 'none');
    });

    //<div data-instanceid="1" data-name="Ante volutpat" data-modulename="Reference Master" data-moduleid="6" data-workflowactiontypename="Create Entity Workflow" data-workflowactiontypeid="1" data-workflowiscreate="true" data-workflowisupdate="true"
    //data-workflowistimeseries="true" data-raisefornonemptyvalue="false" data-workflowisdelete="true" class="hover-link pull-right">
    //            <i class="fa fa-arrow-circle-right" title="Update Workflow"></i>                 
    //        </div>

    //TODO: add auto complete select box for selecting workflows      

    var onWorkflowModuleChanged = function (e) {
        //console.log('e...', e);

        var moduleId = smselect.getSelectedOption($('#smselect_newWorkflowModuleDropdown'))[0].value;
        var filteredActionTypes = [];
        console.log("action types", workflowManger._workflowActionTypes);
        workflowManger._workflowActionTypes.forEach(function (actionType) {
            if (actionType.moduleId == 0 || actionType.moduleId == parseInt(moduleId)) {
                filteredActionTypes.push({
                    "text": actionType.text,
                    "value": actionType.value
                });
            }
        })

        if (filteredActionTypes.length > 0) {
            workflowManger.createSMSelectDropDown(workflowManger.controls.newWorkflowActionTypeDropdown(), filteredActionTypes, false, 150, null, null, null, [], false, null, null);
            smselect.setOptionByText($('#smselect_newWorkflowActionTypeDropdown'), filteredActionTypes[0].text, false);
        }
    }

    //dropDownId, smdata, showSearch, width, callback, heading, onChangeHandler, selectedItems, isMulti, multiText, allText
    workflowManger.createSMSelectDropDown(workflowManger.controls.newWorkflowModuleDropdown(), workflowManger._workflowModules, false, 150, null, null, onWorkflowModuleChanged, [], false, null, null);
    smselect.setOptionByText($('#smselect_newWorkflowModuleDropdown'), workflowManger._workflowModules[0].text, false);


    var isInvalid = function () {

        $('.newWorkflowErroMessage').css('display', 'none');
        $('.newWorkflowErroMessage').html('');
        var isError = false;

        if ($('#newWorkflowName').val() == '') {
            $('.newWorkflowErroMessage').append('<div>Please enter workflow name<div>');
            isError = true;
        }
        if ($('#newWorkflowName').val().length > 200) {
            $('.newWorkflowErroMessage').append('<div>Workflow name can not be greater than 200 characters.<div>');
            isError = true;
        }
        if (smselect.getSelectedOption($('#smselect_newWorkflowModuleDropdown')).length == 0) {
            $('.newWorkflowErroMessage').append('<div>Please select module.<div>');
            isError = true;
        }
        if (smselect.getSelectedOption($('#smselect_newWorkflowActionTypeDropdown')).length == 0) {
            $('.newWorkflowErroMessage').append('<div>Please select action type.<div>');
            isError = true;
        }

        if (workflowManger._workflowNameCollection.length > 0 && $('#newWorkflowName').val().trim() != "") {
            var newWorkflowName = $('#newWorkflowName').val().trim().toLowerCase();
            workflowManger._workflowNameCollection.forEach(function (workflowName) {
                if (workflowName == newWorkflowName) {
                    isError = true;
                }
            });
            if (isError)
                $('.newWorkflowErroMessage').append('<div>Workflow name already exists.<div>');
        }

        //console.log('inside validate',isError);

        return isError;
    }

    $('#newWorkflowSubmit').on('click', function () {

        if (!isInvalid()) {

            var Name = $('#newWorkflowName').val(),
            ModuleId = smselect.getSelectedOption($('#smselect_newWorkflowModuleDropdown'))[0].value,
            ModuleName = smselect.getSelectedOption($('#smselect_newWorkflowModuleDropdown'))[0].text,
            WorkflowActionTypeId = smselect.getSelectedOption($('#smselect_newWorkflowActionTypeDropdown'))[0].value,
            WorkflowActionTypeName = smselect.getSelectedOption($('#smselect_newWorkflowActionTypeDropdown'))[0].text,
            UserName = workflowManger._userName,
            workflowIsCreate = false,
            workflowIsUpdate = false,
            workflowIsTimeSeries = false,
            raiseForNonEmptyValue = false,
            workflowIsDelete = false;

            if (ModuleId == 3) {
                workflowIsCreate = false;
                workflowIsUpdate = false;
                workflowIsTimeSeries = false;
                raiseForNonEmptyValue = false;
                workflowIsDelete = false;
            }

            var workflowForSetup = {
                "instanceId": 0, name: Name, moduleId: ModuleId, moduleName: ModuleName, workflowActionTypeId: WorkflowActionTypeId, workflowActionTypeName: WorkflowActionTypeName, workflowIsCreate: workflowIsCreate,
                workflowIsUpdate: workflowIsUpdate, workflowIsTimeSeries: workflowIsTimeSeries, raiseForNonEmptyValue: raiseForNonEmptyValue,
                workflowIsDelete: workflowIsDelete
            }
            workflowManger.generateWorkflowSetupView('save', workflowForSetup);
            //workflowManger.insertWorkflow();
        } else {
            $('.newWorkflowErroMessage').css('display', 'inline-block');
        }
    });

    $(document).click(function (event) {
        var target = $(event.target);
        if ($(".add-new-workflow-form").length !== null && (".add-new-workflow-form").length !== 0) {
            if ($(target).parents(".add-new-workflow-form").length === 0 && $(".workflow-box").length !== 0) {
                if ($(".add-new-workflow-form").css("display") != "none") {
                    $(".add-new-workflow-form").css("display", "none");
                }
            }
        }
    });
})