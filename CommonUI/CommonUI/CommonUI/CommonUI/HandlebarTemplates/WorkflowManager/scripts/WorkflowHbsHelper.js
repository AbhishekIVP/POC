//handle bars helper methods

//View: Workflow Setup View
Handlebars.registerHelper('setupButton', function (action, options) {
    if (action == 'update') {
        return '<div id="workflowSetupSaveBtn" class="workflow-setup-action">Update</div>';
    } else if (action == 'save') {
        return '<div id="workflowSetupSaveBtn" class="workflow-setup-action">Save</div>';
    }
});

//View: Workflow Setup View
Handlebars.registerHelper('insertIframePanel', function (index, options) {
    index++;
    if (index % 4 == 0 || index == options.data.root.workflowTemplates.length) {
        var id = "workflowSetupIframePanel_" + Math.ceil(index / 4);
        return '<div class="workflow-Default-Rule-Editor-container"> <div class="workflowSetup_editableTemplateContainer"> <div class="workflowSetup_editableTemplate">Edit Template</div> </div> <div id="' + id + '" class="col-sm-12 workflow-setup-template-panel"></div></div>';
    }
});

Handlebars.registerHelper('workflowStateCount', function (states, options) {
    return states.length;
});

Handlebars.registerHelper('workflowRuleinsertIframePanel', function (index, options) {
    index++;
    debugger;
    var id = "workflowRuleIframePanel_" + Math.ceil(index);

    //if (typeof options.data.root.workflowRulesTemplate[0] != "undefined" && options.data.root.workflowRulesTemplate.length != 0) {
    //    $('.workflow-setup-box').css('display', 'none');
    //    //$('#workflowTemplates').css('display', 'none');
    //    $('.workflowRulesViewTemplate').css('display', 'block');
    //}
    if (index == options.data.root.workflowRulesTemplate.length && typeof options.data.root.workflowRulesTemplate[0] != "undefined") {
        workflowManger.toSelectFirstTime = true;
    }
    return '<div class="workflowRule-setup-panel-top_container"> <div class="workflowSetup_editableTemplateContainer"> <div class="workflowSetup_editableTemplate">Edit Template</div> </div> <div id="' + id + '" class="workflowRule-setup-panel"></div></div>';
});

//View: Workflow Rule Editor Setup View
Handlebars.registerHelper('insertIframePanelInRuleEditor', function (index, options) {
    index++;
    if (index % 4 == 0 || index == options.data.root.workflowTemplates.length) {
        var id = "workflowSetupRuleEditorIframePanel_" + Math.ceil(index / 4);
        return '<div class="workflowRule-Editor-container"> <div class="workflowSetup_editableTemplateContainer"> <div class="workflowSetup_editableTemplate">Edit Template</div> </div> <div id="' + id + '" class="col-sm-12 workflow-setup-ruleEditortemplate-panel"></div></div>';
    }
});