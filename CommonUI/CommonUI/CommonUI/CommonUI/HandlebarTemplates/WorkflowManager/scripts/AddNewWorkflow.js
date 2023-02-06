$(function () {
    
    $('.workflow-back-icon').on('click', function () {
        workflowManger.getAllWorkflowDetails();
    })

    var onWorkflowModuleChanged = function (e) {
        //console.log('e...', e);
        var moduleId = smselect.getSelectedOption($('#smselect_newWorkflowModuleDropdown'))[0].value;
        var filteredActionTypes = [];
        console.log("action types", workflowManger._workflowActionTypes);
        workflowManger._workflowActionTypes.forEach(function (actionType) {
            if (actionType.moduleId == 0 || actionType.moduleId == moduleId) {
                filteredActionTypes.push({
                    "text": actionType.text,
                    "value": actionType.value
                });
            }
        })
        
        workflowManger.createSMSelectDropDown(workflowManger.controls.newWorkflowActionTypeDropdown(), filteredActionTypes, false, 150, null, null, null, [], false, null, null);
    }

    //dropDownId, smdata, showSearch, width, callback, heading, onChangeHandler, selectedItems, isMulti, multiText, allText
    workflowManger.createSMSelectDropDown(workflowManger.controls.newWorkflowModuleDropdown(), workflowManger._workflowModules, false, 150, null, null, onWorkflowModuleChanged, [], false, null, null);    
    smselect.setOptionByText($('#smselect_newWorkflowModuleDropdown'), "Security Master", false)
    

    var isInvalid = function () {
        
        $('.newWorkflowErroMessage').css('display', 'none');
        $('.newWorkflowErroMessage').html('');
        var isError = false;

        if($('#newWorkflowName').val() == ''){
            $('.newWorkflowErroMessage').append('<div>Please enter workflow name<div>');
            isError = true;
        }
        if ($('#newWorkflowName').val().length > 200) {
            $('.newWorkflowErroMessage').append('<div>Workflow name can not be greater than 200 characters.<div>');
            isError = true;
        }
        if(smselect.getSelectedOption($('#smselect_newWorkflowModuleDropdown')).length == 0){
            $('.newWorkflowErroMessage').append('<div>Please select module.<div>');
            isError = true;
        }
        if(smselect.getSelectedOption($('#smselect_newWorkflowActionTypeDropdown')).length == 0){
            $('.newWorkflowErroMessage').append('<div>Please select action type.<div>');
            isError = true;
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
            workflowIsCreate= false, 
            workflowIsUpdate= false, 
            workflowIsTimeSeries= false, 
            raiseForNonEmptyValue= false,
            workflowIsDelete= false;

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
            $('.newWorkflowErroMessage').css('display', 'block');
        }
    })

    
})