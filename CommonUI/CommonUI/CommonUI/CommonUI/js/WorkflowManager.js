var workflowManger = {};
workflowManger._path = "";
workflowManger._workflowServiceLocation = "/BaseUserControls/Service/WorkflowService.asmx";
workflowManger._windowHeight = null;
workflowManger._windowWidth = null;
workflowManger._workflowDetails = null;
workflowManger._workflowModules = [];
workflowManger._workflowActionTypes = [];
workflowManger._userName = "";
workflowManger._types = [];
workflowManger._primaryAttributeList = [];
workflowManger._otherAttributeList = [];
workflowManger._workflowTemplates = [];
workflowManger._isTemplateUpdated = false;
workflowManger._workflowRulesTemplate = [];
workflowManger.toSelectFirstTime = false;
workflowManger.loginModuleName = "";
workflowManger.leftMenuPath = "";
workflowManger._isRuleAdded = false;
workflowManger._worflowTypeVsTypeIds = {};
workflowManger._isWorkFlowAlreadyMade = false;
workflowManger._workflowNameCollection = [];
workflowManger._isPendingRequestWorkFlow = false;
workflowManger._isPendingRequestRADWorkFlow = false;
workflowManger._isUpdateRule = false;
workflowManger.__koBindingApplied = false;
workflowManger._pageViewModelStateRulesPopUp = null;
//Added for State Rule popup
workflowManger._workflowStateRule = [];
workflowManger.toAddinCollection = false;
workflowManger.ruleSetIDVsName = [];
workflowManger._stateData = [];

workflowManger._workflowAction = [];

workflowManger.toAddinCollectionAction = false;
workflowManger.rulePriorityVsName = [];
workflowManger.rulePriorityVsName_update = [];
workflowManger._actionData = [];
workflowManger._pageViewModelActionsPopUp = null;
workflowManger._pageViewModelActionsPopUp_update = null;

workflowManger._emailTemplatefromUI = true;

workflowManger.__koBindingAppliedActions = false;

workflowManger._primaryDisplayAttributeList = {'text':''};
workflowManger._otherDisplayAttributeList = [];
workflowManger._primaryAndOtherDisplayAttributeList = [];
workflowManger._dataSectionAttributeList = [{ 'text': 'Security Type' }];

workflowManger._attributeError = false;

//Ajax Call stub
workflowManger.callService = function (httpMethod, serviceServerString, methodName, parameters, ajaxSuccess, ajaxError, response, userContext, isCrossDomain, async = true) {
    var parametersString = '';
    var options = null;
    if (httpMethod.toUpperCase() == 'GET') {
        $.each(parameters, function (key, value) {
            parameters[key] = JSON.stringify(value, function (key, evaluateObject) {
                return evaluateObject === "" ? "" : evaluateObject
            });
            //parameters[key] = JSON.stringify(value);
        });
        if (response != null && response != undefined) {
            options = {
                type: 'GET',
                url: serviceServerString + '/' + methodName,
                processData: true,
                data: parameters,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: ajaxSuccess,
                error: ajaxError,
                context: userContext,
                async: async
            };
        }
        else {
            if (isCrossDomain) {
                //jQuery.support.cors = true;
                options = {
                    type: 'GET',
                    url: serviceServerString + '/' + methodName,
                    processData: true,
                    data: parameters,
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'jsonp',
                    success: ajaxSuccess,
                    error: ajaxError,
                    param1: userContext,
                    async: async
                };
            }
            else {
                options = {
                    type: 'GET',
                    url: serviceServerString + '/' + methodName,
                    processData: true,
                    data: parameters,
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: ajaxSuccess,
                    error: ajaxError,
                    param1: userContext,
                    async: async
                };
            }
        }
    }
    if (httpMethod.toUpperCase() == 'POST') {
        //serializedString = JSON.stringify(parameters);
        serializedString = JSON.stringify(parameters, function (key, evaluateObject) {
            return evaluateObject === "" ? "" : evaluateObject
        });
        if (isCrossDomain) {
            jQuery.support.cors = true;
            options = {
                type: 'POST',
                url: serviceServerString + '/' + methodName,
                processData: false,
                data: serializedString,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: ajaxSuccess,
                error: ajaxError,
                param1: userContext,
                async: async,
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('OPTIONS', null);
                }
            };
        }
        else {
            options = {
                type: 'POST',
                url: serviceServerString + '/' + methodName,
                processData: false,
                data: serializedString,
                contentType: 'application/json;',
                dataType: 'json',
                success: ajaxSuccess,
                error: ajaxError,
                param1: userContext,
                async: async
            };
        }
    }
    //console.log('options', options);
    $.ajax(options);
}

//Error Handling
workflowManger.FailureCallback = function (err) {
    workflowManger.showErrorDiv('Error', 'fail_icon.png', err);
}

//public controls
workflowManger.controls = {
    newWorkflowModuleDropdown: function () {
        return $('#newWorkflowModuleDropdown');
    },
    newWorkflowActionTypeDropdown: function () {
        return $('#newWorkflowActionTypeDropdown');
    },
    primaryAttributeDropdown: function () {
        return $('#primaryAttributeDropdown');
    },
    otherAttributeDropdown: function () {
        return $('#otherAttributeDropdown');
    },
    availableAttributeDropdown: function () {
        return $('#availableAttributeDropdown')
    }

}

//Create SM Select dropdown
workflowManger.createSMSelectDropDown = function (dropDownId, smdata, showSearch, width, callback, heading, onChangeHandler, selectedItems, isMulti, multiText, allText) {
    var obj = {};
    obj.container = dropDownId;
    obj.id = dropDownId.attr("id");
    if (heading !== null) {
        obj.heading = heading;
    }
    if (allText !== null || allText !== undefined) {
        obj.allText = allText;
    }
    obj.data = smdata;
    if (showSearch) {
        obj.showSearch = true;
    }
    if (isMulti) {
        obj.isMultiSelect = true;
        obj.text = multiText;
    }
    if (selectedItems !== undefined && selectedItems.length !== 0) {
        obj.selectedItems = selectedItems;
    }
    obj.isRunningText = false;
    obj.ready = function (e) {
        e.width(width + "px");
        if (typeof onChangeHandler === "function") {
            e.on('change', function (ee) {
                onChangeHandler(ee);
               
            });
            
        }
    }
    smselect.create(obj);

    $("#smselect_" + dropDownId.attr("id")).find(".smselect").css('text-align', 'left');
    $("#smselect_" + dropDownId.attr("id")).find(".smselectanchorcontainer2").css('width', '86%');

    if (typeof callback === "function") {
        callback();
    }
};

//Convert check box to toggle
workflowManger.convertCheckBoxToToggle = function (checkboxID) {
    var id = "sm_toggle_" + checkboxID;
    if ($("body").find("#" + id).length === 0) {
        var checkBox = $("#" + checkboxID);
        checkBox[0].style.display = "none";

        //By Default NO is Selected
        var HTML = "<div id='" + id + "' class='sm_toggleContainer'>";
        HTML += "<div class='sm_toggleText'>YES</div>";
        HTML += "<div class='sm_toggleText'>NO</div>";
        if (!checkBox.is(":checked")) {
            HTML += "<div class='sm_toggleBtn' style='margin-left:30px;'>NO</div>";
        }
        else {
            HTML += "<div class='sm_toggleBtn'>YES</div>";
        }
        HTML += "</div>";

        checkBox.after(HTML);

        checkBox.next().unbind('click').bind('click', function (e) {
            var target = $(e.target);
            if (target.hasClass("sm_toggleText")) {
                target = target.parent();
            }
            if (target.hasClass("sm_toggleBtn")) {
                target = target.parent();
            }
            target = target.find(".sm_toggleBtn");
            if (target.css("margin-left") !== "30px") {
                target.animate({ "margin-left": "30px" }, function () {
                    $("#" + id).find(".sm_toggleBtn").text("NO");
                    checkBox.prop("checked", false);
                });
            }
            else {
                target.animate({ "margin-left": "0px" }, function () {
                    $("#" + id).find(".sm_toggleBtn").text("YES");
                    checkBox.prop("checked", true);
                });
            }
        });
    }
}

//Fill Collection for workflowType VS TypeIDs
workflowManger.saveWorkflowTypeVsTypeId = function (workflows) {
    if (workflows.d.length > 0) {
        var workflowOnTypeIds = [];
        workflows.d.forEach(function (d) {
            workflowOnTypeIds = d.TypeIds.split('|');
            if (workflowManger._worflowTypeVsTypeIds.hasOwnProperty(d.WorkflowActionTypeName.trim().toLowerCase())) {
                workflowOnTypeIds.forEach(function (typeID) {
                    if (typeID.trim() != "")
                        workflowManger._worflowTypeVsTypeIds[d.WorkflowActionTypeName.trim().toLowerCase()].push(typeID.trim());
                });
            }
            else {
                workflowManger._worflowTypeVsTypeIds[d.WorkflowActionTypeName.trim().toLowerCase()] = [];
                workflowOnTypeIds.forEach(function (typeID) {
                    if (typeID.trim() != "")
                        workflowManger._worflowTypeVsTypeIds[d.WorkflowActionTypeName.trim().toLowerCase()].push(typeID.trim());
                });
            }

        });
        console.log("colln : ", workflowManger._worflowTypeVsTypeIds);
    }
}

workflowManger.isEmpty = function (obj) {
    for (var key in obj) {
        if (obj.hasOwnProperty(key))
            return false;
    }
    return true;
}

//Search for existing type ID in collection
workflowManger.searchWorflowTypeVsTypeIds = function (workflowActionTypeName, TypeId) {
    if (!workflowManger.isEmpty(workflowManger._worflowTypeVsTypeIds)) {
        if (typeof workflowManger._worflowTypeVsTypeIds[workflowActionTypeName] != "undefined") {
            for (var type = 0; type < workflowManger._worflowTypeVsTypeIds[workflowActionTypeName].length; type++) {
                if (workflowManger._worflowTypeVsTypeIds[workflowActionTypeName][type] == (TypeId.trim())) {
                    workflowManger._isWorkFlowAlreadyMade = true;
                    return false;
                }
                else
                    workflowManger._isWorkFlowAlreadyMade = false;
            }
        }
        else
            workflowManger._isWorkFlowAlreadyMade = false;
    }
}

//Save WorkFlow Names in Collection
workflowManger.saveWorkflowNameInCollection = function (workflows) {
    if (workflows.d.length > 0) {
        workflows.d.forEach(function (workflowName) {
            workflowManger._workflowNameCollection.push(workflowName.Name.trim().toLowerCase());
        });
    }
}

//Display woorkflow details view
workflowManger.generateDetailsView = function (workflows) {

    console.log('workflows...', workflows);
    //console.log('all workflows', workflows.d);
    workflowManger.saveWorkflowTypeVsTypeId(workflows);
    workflowManger.saveWorkflowNameInCollection(workflows);
    var context = {
    };

    if (workflows.d.length > 0) {

        if (typeof workflowManger.loginModuleName != "undefined" && workflowManger.loginModuleName != "") {
            var moduleBasedTemplates = [];
            if (workflowManger.loginModuleName.toLowerCase().indexOf('ref') != -1) {
                workflows.d.forEach(function (d) {
                    if (d.ModuleName.toLowerCase().indexOf('sec') == -1) {
                        moduleBasedTemplates.push(d);
                    }
                });
                context.workflows = moduleBasedTemplates;
            }
            else
                context.workflows = workflows.d;
        }
        else {
            context.workflows = workflows.d;
        }

        $('.page-title').html('Please Select Workflow');

        context.css = [{
            css: 'HandlebarTemplates/WorkflowManager/css/WorkflowDetails.css'
        }];
        context.scripts = [{
            script: 'HandlebarTemplates/WorkflowManager/scripts/WorkflowDetails.js'
        }];

        $.get('HandlebarTemplates/WorkflowManager/WorkflowDetails.hbs', function (data) {
            var template = Handlebars.compile(data);
            $('#workflowDetails').html(template(context));
            $('#addNewWorkflowButton').css('display', 'inline-block');
        }, 'html');
    }
    else {
        context.css = [{
            css: 'HandlebarTemplates/WorkflowManager/css/WorkflowDetails.css'
        }];
        context.scripts = [{
            script: 'HandlebarTemplates/WorkflowManager/scripts/WorkflowDetails.js'
        }];

        $.get('HandlebarTemplates/WorkflowManager/WorkflowDetails.hbs', function (data) {
            var template = Handlebars.compile(data);
            $('#workflowDetails').html(template(context));
            var element = "addNewWorkflowButton";
            $(".add-new-workflow-form").css({
                top: (($(window).height() / 2) - $('.add-new-workflow-form').height()), left: (($(window).width() / 2) - 150), position: 'absolute', display: 'block', 'box-shadow': 'none'
            });
            $('#addNewWorkflowButton').css('display', 'none');
        }, 'html');

        event.stopPropagation();
    }

}

//Display add new workflow view
//workflowManger.generateAddNewWorkflowView = function () {
//    //console.log('inside new workflow');
//    var context = {};
//    context.css = [{ css: 'HandlebarTemplates/WorkflowManager/css/AddNewWorkflow.css' }];
//    context.scripts = [{ script: 'HandlebarTemplates/WorkflowManager/scripts/AddNewWorkflow.js' }];
//    $.get('HandlebarTemplates/WorkflowManager/AddNewWorkflow.hbs', function (data) {
//        var template = Handlebars.compile(data);
//        $('#workflowDetails').html(template(context));
//    }, 'html');
//}

//Display workflow setup view
workflowManger.generateWorkflowSetupView = function (action, workflow) {
    console.log('generateWorkflowSetupView');

    var context = {
        action: action, workflow: workflow
    };
    context.css = [{
        css: 'HandlebarTemplates/WorkflowManager/css/WorkflowSetup.css'
    }];
    context.scripts = [{
        script: 'HandlebarTemplates/WorkflowManager/scripts/WorkflowSetup.js'
    }];
    //console.log('context',context)
    $.get('HandlebarTemplates/WorkflowManager/WorkflowSetup.hbs', function (data) {
        var template = Handlebars.compile(data);
        $('#workflowDetails').html(template(context));
        if (action.toLowerCase().trim() == "save") {
            $('.add_New_Default_Rule_Container').css('display', 'none');
            $('.workflow-setup-box').css('display', 'block');
        }

        if (action.toLowerCase() == "update") {
            workflowManger.getWorkflowRules(workflow.instanceId, workflow.moduleId);
        }
    }, 'html');
}

//Display workflow template view
workflowManger.generateWorkflowTemplateView = function (id) {
    console.log('generateWorkflowTemplateView');

    console.log('inside generateWorkflowTemplateView', workflowManger._isTemplateUpdated);
    var workflowTemplates = [];
    var moduleId = $('.workflow-top-container').data('moduleid');
    workflowManger._workflowTemplates.forEach(function (d) {
        if (d.moduleId == moduleId) {
            workflowTemplates.push(d)
        }
    })

    var context = {
        workflowTemplates: workflowTemplates
    };
    context.css = [];
    context.scripts = [{
        script: 'HandlebarTemplates/WorkflowManager/scripts/WorkflowTemplate.js'
    }];
    $.get('HandlebarTemplates/WorkflowManager/WorkflowTemplates.hbs', function (data) {
        var template = Handlebars.compile(data);
        $('#' + id).html(template(context));
    }, 'html');
}
//Display workflow rules template view
workflowManger.generateWorkflowRuleTemplateView = function (id) {
    console.log('generateWorkflowRuleTemplateView');
    console.log('inside generateWorkflowRulesTemplateView', workflowManger._isTemplateUpdated);
    var workflowTemplates = [];
    var moduleId = $('.workflow-top-container').data('moduleid');
    workflowManger._workflowTemplates.forEach(function (d) {
        if (d.moduleId == moduleId) {
            workflowTemplates.push(d)
        }
    })

    var context = {
        workflowTemplates: workflowTemplates
    };
    context.css = [];
    context.scripts = [{
        script: 'HandlebarTemplates/WorkflowManager/scripts/WorkflowTemplate.js'
    }];
    $.get('HandlebarTemplates/WorkflowManager/WorkflowRulesEditorTemplate.hbs', function (data) {
        var template = Handlebars.compile(data);
        $('#' + id).html(template(context));
    }, 'html');
}
// Display workflow rule view
workflowManger.generateWorkflowRuleEditorTemplateView = function () {
    console.log('generateWorkflowRuleEditorTemplateView');
    console.log('inside generateWorkflowRuleEditorTemplateView', workflowManger._isTemplateUpdated);
    var workflowRulesTemplate = [];
    var moduleId = $('.workflow-top-container').data('moduleid');
    var defaultCount = 0;

    workflowManger._workflowRulesTemplate.forEach(function (d) {
        if (d.moduleId == moduleId) {
            workflowRulesTemplate.push(d)
        }
        if (d.SRMWorkFlowIsDefault == true) {
            $('.add_New_Default_Rule_Container').css('display', 'none');
            defaultCount++;
        }
    });

    if (workflowManger._workflowRulesTemplate.length == 0) {
        $('.add_New_Default_Rule_Container').css('display', 'none');
        $('.workflow-setup-box').css('display', 'block');
        $('.workflowRulesViewTemplate').css('display', 'none');
    }
    else {
        $('.workflow-setup-box').css('display', 'none');
        $('.workflowRulesViewTemplate').css('display', 'block');
    }

    if (defaultCount == 0)
        $('.add_New_Default_Rule_Container').css('display', 'inline-block');


    workflowRulesTemplate.push(workflowRulesTemplate.shift());
    var context = {
        workflowRulesTemplate: workflowRulesTemplate
    };
    context.css = [];
    context.scripts = [{
        script: 'HandlebarTemplates/WorkflowManager/scripts/WorkflowTemplate.js'
    }];
    $.get('HandlebarTemplates/WorkflowManager/WorkflowRulesTemplate.hbs', function (data) {
        var template = Handlebars.compile(data);
        $('#workflowRulesViewTemplate').html(template(context));
    }, 'html');
}

//Get workflow Modules

workflowManger.setWorkflowModules = function (modules) {
    modules.d.forEach(function (module) {
        workflowManger._workflowModules.push({
            "text": module.ModuleName,
            "value": module.ModuleId
        });
    });
    workflowManger.getWorkflowActionTypes();
}
workflowManger.getWorkflowModules = function () {
    workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'GetWorkflowModules', {}, workflowManger.setWorkflowModules, workflowManger.FailureCallback);
}

//Insert new Workflow
workflowManger.insertCallback = function (result) {
    //console.log('insert result..', result);
    if (result.d.Message == 'Passed') {
        $('#workflowSetupSaveBtn').text('Update');
        $('.workflow-top-container').data('action', 'update');
        $('.workflow-top-container').attr('data-action', 'update');

        var workflow = {
            "instanceId": result.d.InstanceId, name: result.d.name, moduleId: result.d.moduleId, moduleName: result.d.moduleName, workflowActionTypeId: result.d.workflowActionTypeId,
            workflowActionTypeName: result.d.workflowActionTypeName, workflowIsCreate: false, workflowIsUpdate: false, workflowIsTimeSeries: false,
            raiseForNonEmptyValue: false, workflowIsDelete: false, TypeIds: result.d.TypeIds, PrimaryAttributeIds: result.d.PrimaryAttributeIds,
            OtherAttributeIds: result.d.OtherAttributeIds
        }
        //
        workflowManger.generateWorkflowSetupView('update', workflow);

        workflowManger.getWorkflowRules(result.d.InstanceId, result.d.moduleId);

        workflowManger.showErrorDiv('Success', 'pass_icon.png', 'Workflow saved successfully.');

        //workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'getWorkflowActionTypes', {}, workflowManger.setWorkflowActionTypes, workflowManger.FailureCallback);        
        //workflowManger.getWorkflowDetails(result.d.InstanceId);
    } else {
        //$('.newWorkflowErroMessage').append('<div>Error occured while creating workflow<div>');
        //console.log('inser workflow error', result.d);
        //$('.newWorkflowErroMessage').css('display', 'block');
        workflowManger.showErrorDiv('Error', 'fail_icon.png', result.d.Message);
    }
}

workflowManger.insertWorkflow = function (workflowName, workflowModuleId, workflowModuleName, workflowWorkflowActionTypeId, workflowWorkflowActionTypeName, workflowWorkflowIsCreate,
                workflowWorkflowIsTimeSeries, workflowRaiseForNonEmptyValue, workflowWorkflowIsDelete, selectedTypeIds, selectedPrimaryAttribute, selectedOtherAttributes) {

    var guid = $('#hdnGuid').val().toString();
    var newWorkflow = {
        "workflow": {
        },
        "rsc": workflowManger._workflowStateRule
    };

    newWorkflow.workflow.InstanceId = 0;
    newWorkflow.workflow.Name = workflowName;
    newWorkflow.workflow.ModuleId = workflowModuleId;
    newWorkflow.workflow.ModuleName = workflowModuleName;
    newWorkflow.workflow.WorkflowActionTypeId = workflowWorkflowActionTypeId;
    newWorkflow.workflow.WorkflowActionTypeName = workflowWorkflowActionTypeName;
    newWorkflow.workflow.workflowWorkflowIsCreate = workflowWorkflowIsCreate;
    newWorkflow.workflow.workflowWorkflowIsTimeSeries = workflowWorkflowIsTimeSeries;
    newWorkflow.workflow.workflowRaiseForNonEmptyValue = workflowRaiseForNonEmptyValue;
    newWorkflow.workflow.workflowWorkflowIsDelete = workflowWorkflowIsDelete;
    newWorkflow.workflow.TypeIds = selectedTypeIds;
    newWorkflow.workflow.PrimaryAttributeIds = selectedPrimaryAttribute;
    newWorkflow.workflow.OtherAttributeIds = selectedOtherAttributes;
    newWorkflow.workflow.UserName = workflowManger._userName;
    newWorkflow.workflow.Guid = guid;


    console.log('new workflow', newWorkflow);
    workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'InsertWorkflow', newWorkflow, workflowManger.insertCallback, null, workflowManger.FailureCallback);
}


//Update existing workflow
workflowManger.updateWorkflowCallback = function (result) {
    console.log('update result - Inside Update', result);
    if (result.d.Message == 'Passed') {

        var getHeaderData = $('.workflow-top-container');
        var workflow = {
            "instanceId": getHeaderData.data('instanceid'), name: getHeaderData.data('name'), moduleId: getHeaderData.data('moduleid'), moduleName: getHeaderData.data('modulename'), workflowActionTypeId: getHeaderData.data('workflowactiontypeid'),
            workflowActionTypeName: getHeaderData.data('workflowactiontypename'), workflowIsCreate: getHeaderData.data('workflowiscreate'), workflowIsUpdate: getHeaderData.data('workflowisupdate'), workflowIsTimeSeries: getHeaderData.data('workflowistimeseries'),
            raiseForNonEmptyValue: getHeaderData.data('raisefornonemptyvalue'), workflowIsDelete: getHeaderData.data('workflowisdelete'), TypeIds: result.d.types, PrimaryAttributeIds: result.d.primaryAttribute,
            OtherAttributeIds: result.d.otherAttributes
        }
        //
        workflowManger.generateWorkflowSetupView('update', workflow);

        workflowManger.getWorkflowRules(getHeaderData.data('instanceid'), getHeaderData.data('moduleid'));

        workflowManger.showErrorDiv('Success', 'pass_icon.png', 'Workflow Updated successfully.');
    }
    else {
        if (result.d.Message.indexOf('pending request') != -1)
            workflowManger.showErrorDiv('Alert', 'fail_icon.png', 'You cannot update workflow as it has pending request.');
        else
            workflowManger.showErrorDiv('Alert', 'fail_icon.png', result.d.Message);
    }
}

workflowManger.updateWorkflow = function (workflowInstanceId, selectedTypes, selectedPrimaryAttribute, selectedOtherAttributes) {
    var guid = $('#hdnGuid').val().toString();
    var moduleId = $('.workflow-top-container').data('moduleid');
    var actionTypeId = $('.workflow-top-container').data('workflowactiontypeid');

    if (!workflowManger._attributeError) {
        console.log('inside update workflow', workflowInstanceId, selectedTypes, selectedPrimaryAttribute, selectedOtherAttributes)
        workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'UpdateWorkflow',
            {
                workflowInstanceId: workflowInstanceId, types: selectedTypes, primaryAttribute: selectedPrimaryAttribute, otherAttributes: selectedOtherAttributes, userName: workflowManger._userName, Guid: guid, moduleId: moduleId, actionTypeId: actionTypeId, rsc: workflowManger._workflowStateRule
            }, workflowManger.updateWorkflowCallback, workflowManger.FailureCallback);
    }
    
}

//Get workflow details
workflowManger.setWorkflowDetails = function (workflowDetails) {

    var workflow = workflowDetails.d;
    console.log('worklfow details', workflowDetails);
    var workflowForSetup = {
        "instanceId": workflow.InstanceId, name: workflow.Name, moduleId: workflow.ModuleId, moduleName: workflow.ModuleName, workflowActionTypeId: workflow.WorkflowActionTypeId,
        workflowActionTypeName: workflow.WorkflowActionTypeName, workflowIsCreate: workflow.WorkflowIsCreate, workflowIsUpdate: workflow.WorkflowIsUpdate, workflowIsTimeSeries: workflow.WorkflowIsTimeSeries,
        raiseForNonEmptyValue: workflow.RaiseForNonEmptyValue, workflowIsDelete: workflow.WorkflowIsDelete
    }
    workflowManger.generateWorkflowSetupView('update', workflowForSetup);
}

workflowManger.getWorkflowDetails = function (instanceId) {
    workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'GetWorkflowDetails', {
        instanceId: instanceId
    }, workflowManger.setWorkflowDetails, workflowManger.FailureCallback);
}

//Get All Workflow Details
workflowManger.getAllWorkflowDetails = function () {
    workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'GetAllWorkflows', {}, workflowManger.generateDetailsView, workflowManger.FailureCallback);
}

//Get Specific Workflow Having Pending Request
workflowManger.getSpecificWorkflowPendingRequestDetails = function (workflowInstanceId) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: workflowManger._path + '/WorkflowService.asmx' + "/GetSpecificWorkfowPendingRequest",
            contentType: "application/json",
            data: JSON.stringify({ "workflowInstanceId": workflowInstanceId }),
            dataType: "json",
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
};

//Get Specific RAD Workflow Having Pending Request
workflowManger.getSpecificRADWorkflowPendingRequestDetails = function (radWorkflowInstanceId) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: workflowManger._path + '/WorkflowService.asmx' + "/GetSpecificRADWorkfowPendingRequest",
            contentType: "application/json",
            data: JSON.stringify({ "radWorkflowInstanceId": radWorkflowInstanceId }),
            dataType: "json",
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
}

//Get Workflow Action Types
workflowManger.setWorkflowActionTypes = function (actionTypes) {
    actionTypes.d.forEach(function (actionType) {
        if (actionType.ActionTypeName.indexOf('Delete') == -1 && actionType.ActionTypeName.indexOf('Attribute') == -1 && actionType.ActionTypeName.indexOf('Leg') == -1) {
            workflowManger._workflowActionTypes.push({
                "text": actionType.ActionTypeName,
                "value": actionType.ActionTypeId,
                "moduleId": actionType.ModuleId
            });
        }
    });

    workflowManger.getAllWorkflowDetails();
    workflowManger.getWorkflowTemplates(false);

    //console.log("action types", workflowManger._workflowActionTypes)
}

workflowManger.getWorkflowActionTypes = function () {
    workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'GetWorkflowActionTypes', {}, workflowManger.setWorkflowActionTypes, workflowManger.FailureCallback);
}

//Get Workflow Template
workflowManger.setWorkflowTemplates = function (templates) {
    workflowManger._workflowTemplates = templates.d;
    //console.log('workflow templates', workflowManger._workflowTemplates, workflowManger._isTemplateUpdated);
    if (workflowManger._isTemplateUpdated) {
        workflowManger.generateWorkflowTemplateView('workflowTemplates');
        workflowManger.generateWorkflowRuleTemplateView('WorkFlowTemplateViewForRule');
        workflowManger._isTemplateUpdated = false;
    }
}

//Add Newly Added Rule to Screen
workflowManger.setWorkflowRulesTemporary = function (newRuleObj) {
    if (newRuleObj.d == null) {
        $('#SMWorkFlowSetup_btnAddDefaultRule').trigger('click');
    }
    else {
        workflowManger._workflowRulesTemplate = newRuleObj.d;
        workflowManger.generateWorkflowRuleEditorTemplateView();
    }
}

//Get Newly Added Rule to Screen
workflowManger.getWorkflowRulesTemporary = function (message) {
    workflowManger._isUpdateRule = false;
    workflowManger.showErrorDiv('Success', 'pass_icon.png', 'Workflow Rule(s) Updated Successfully.');

    //$('.workflow-top-container').data('instanceid');
    //if (message.d != null && message.d.Message == "Failed")
    //    workflowManger.showErrorDiv('Error', 'fail_icon.png', 'Rule cannot be deleted as workflow has pending requests.');

    var guid = $('#hdnGuid').val().toString();
    if (guid != "") //instanceId !== 0 && moduleId.toString() != "" && 
        workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'getWorkFlowRuleDetails', {
            Guid: guid
        }, workflowManger.setWorkflowRulesTemporary, workflowManger.FailureCallback);
}

//Get Workflow Rule Template
workflowManger.setWorkflowRuleForTemplates = function (templatesRule) {

    workflowManger._workflowRulesTemplate = templatesRule.d;
    //if (workflowManger._isTemplateUpdated) {
    if ($('.workflow-top-container').length == 0) {
        window.setTimeout(() => {
            workflowManger.generateWorkflowRuleEditorTemplateView();
        }, 2000);

    }
    else {
        workflowManger.generateWorkflowRuleEditorTemplateView();
    }
    //workflowManger._isTemplateUpdated = false;
    // }
}

workflowManger.getWorkflowTemplates = function (isTemplateUpdated) {

    workflowManger._isTemplateUpdated = isTemplateUpdated;
    workflowManger.callService('POST', workflowManger._path + '/RADWorkflow.svc', 'GetAllWorkflows', {}, workflowManger.setWorkflowTemplates, workflowManger.FailureCallback);
}

//Get Rule set Info
workflowManger.getWorkflowRules = function (instanceId, moduleId) {

    //workflowManger._isTemplateUpdated = isTemplateUpdated;
    //var instanceId = $('.workflow-top-container').data('instanceid');
    //var moduleId = $('.workflow-top-container').data('moduleid');
    var guid = $('#hdnGuid').val().toString();
    if (instanceId !== 0 && moduleId.toString() != "" && guid != "")
        workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'GetAllWorkflowsRules', {
            instanceId: instanceId, moduleId: moduleId, Guid: guid
        }, workflowManger.setWorkflowRuleForTemplates, workflowManger.FailureCallback);
}


//Get Rule Data To Update
workflowManger.getRuleDataToUpdate = function (ruleSetID, isDefault, radworkflowinstanceId, moduleid) {
    var instanceId = $('.workflow-top-container').data('instanceid');
    var guid = $('#hdnGuid').val().toString();
    if (instanceId == 0 && moduleid.toString() != "" && guid != "") {
        workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'GetWorkflowRuleDataToUpdate', {
            ruleSetID: ruleSetID, Guid: guid
        }, workflowManger.setWorkflowRuleEditorForUpdate, workflowManger.FailureCallback);
    }
}

workflowManger.setWorkflowRuleEditorForUpdate = function (ruleData) {
    $('#SMWorkFlowSetup_btnAddRule').trigger('click');
    $('#ruleTxt').val(ruleData.d.SRMWorkFlowRuleText);
    $('#txtPriority').val(ruleData.d.WorkFlowRulePriority);
    $('.workflowCheck').removeClass('selectedWorkflowTemplate');
    $('#WorkFlowTemplateViewForRule').find('#' + ruleData.d.RadWorkFlowInstanceID + ' .workflowCheck').addClass('selectedWorkflowTemplate');
}

workflowManger.deleteRuleInWorkFlow = function (rule_set_id, is_Default) {
    var instanceId = $('.workflow-top-container').data('instanceid');
    var guid = $('#hdnGuid').val().toString();
    var isUpdateWorkflow = false;
    if (instanceId > 0)
        isUpdateWorkflow = true;

    if (guid != "")
        workflowManger.callService('POST', workflowManger._path + '/WorkflowService.asmx', 'DeleteRuleFromDataBaseandCollection', { Guid: guid, ruleset_id: rule_set_id, isDefault: is_Default, isUpdate: isUpdateWorkflow, workFlowInstanceID: instanceId }, workflowManger.getWorkflowRulesTemporary, workflowManger.FailureCallback);
}


workflowManger.onSuccess = function () {
    console.log("Success");
}

workflowManger.setPath = function () {
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    workflowManger._path = path + '/BaseUserControls/Service';
}

workflowManger.setModule = function () {
    //Set Module
    if (window.parent.leftMenu != null)
        workflowManger.leftMenuPath = window.parent.leftMenu;
    else if (window.parent.parent.leftMenu != null)
        workflowManger.leftMenuPath = window.parent.parent.leftMenu;

    if (typeof workflowManger.leftMenuPath != "undefined" && workflowManger.leftMenuPath != "")
        workflowManger.loginModuleName = workflowManger.leftMenuPath.baseModule;
}

workflowManger.init = function () {

    workflowManger._userName = document.getElementById("UserName").value;
    //console.log('user name', workflowManger._userName);
    workflowManger.setPath();
    workflowManger.setModule();
    workflowManger.getWorkflowModules();
    //Set Window Height and Width
    workflowManger._windowHeight = $(window).height();
    workflowManger._windowWidth = $(window).width();
};

$(function () {
    workflowManger.init();
});


workflowManger.adjustScreenSize = function () {

    var height = $(window).outerHeight();
    var remainingheight = $(window).outerHeight() - ($('.workflow-top-container').outerHeight() + 10);
    $('.SRMWorkFlowLowerDiv').css({
        'height': remainingheight
    });
    $('.setup-left-container').css({
        'height': remainingheight
    });
    $('.WorkFlowRightDiv').css({
        'height': remainingheight
    });
    $('#workflowRulesTemplate').css({
        'height': remainingheight
    });
    $('.workflowDetails').css({
        'height': height - 10
    });
    $('#selectedTypeList').css('max-height', 200);
    $('#selectedTypeList').css('overflow-y', 'auto');
    $('#selectedTypeList').css('overflow-x', 'hidden');

    var availableTypesHeight = ($('#SMWorkFlowSetupSearchContainer').outerHeight() + $('#selectedTypeLabel').outerHeight() + $('#selectedTypeList').outerHeight() + $('#availableTypeLabel').outerHeight() + 10);
    $('#availableTypeList').css('height', (remainingheight - (availableTypesHeight + 40)));
    $('#availableTypeList').addClass('availableTypeList');
}


workflowManger.showErrorDiv = function (errorHeading, srcImageName, errorMessageText) {
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    path += '/images/icons/';

    $("#workFlowDetailsError_ImageURL").attr('src', path + srcImageName);
    $(".workFlowDetailsError_popupTitle").html(errorHeading);
    $(".workFlowDetailsError_popupMessage").html(errorMessageText);
    if (errorHeading.toLowerCase().trim() == "alert") {
        $('.workFlowDetailsError_popupContainer').css('border-top', '4px solid rgb(199, 137, 140)');
        $('.workFlowDetailsError_popupTitle').css('color', '4px solid #8a8787');
        $('.workFlowDetailsError_popupMessageContainer').css('margin-left', '25px');
        $('#workFlowDetailsErrorDiv_messagePopUp').show();
        $('#workflowSetup_disableDiv').show();
        return false;
    }
    else if (errorHeading.toLowerCase().trim() == "success") {
        $('.workFlowDetailsError_popupContainer').css('border-top', '4px solid rgb(172, 211, 115)');
        $('.workFlowDetailsError_popupTitle').css('color', '4px solid rgb(172, 211, 115)');
        $('.workFlowDetailsError_popupMessageContainer').css('margin-left', '72px');
        $('#workFlowDetailsErrorDiv_messagePopUp').show();
        $('#workflowSetup_disableDiv').show();
        return true;
    }
}