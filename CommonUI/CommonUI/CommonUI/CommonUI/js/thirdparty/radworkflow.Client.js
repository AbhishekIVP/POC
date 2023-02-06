/*! grid-toolkit - v1.0.0 - 2018-04-27 7:33:25 */
var radworkflow = {
    baseUrl: "",
    moduelid: null,
    identifier: '',
    IsEditable:true,
    user: '',
    rulrGrammarInfo: {
        Columns: [], DefaultGrouping: null, ElseCount: -1, ThenCount: -1, IfClauseAvailable: true, IsDataTypeAvailable: true, RuleTypeJson: "ConditionalRule",
        RADXRuleCustomOpInfo: {}, RADXRuleCustomOpInfoJson: [], ParametersInfo: [], dataType: 2, dataTypeJson: "Numeric"
    }
}

$.extend(radworkflow, {
    handlers: {
        initialize: function (obj) {
            $.extend(radworkflow, obj);
            var self = this;
            self.getAllWorkFlowsHTML();
        },

        openWorkflow: function (obj) {
            $.extend(radworkflow, obj);
            var self = this;
            radworkflow.addWorkFlow.createnEditWorkFLow(true, radworkflow.workFlowName);
        },

        createWorkflow: function (obj) {
            $.extend(radworkflow, obj);
            var self = this;
            self.saveWorkFLow(radworkflow.workFlowName, "", radworkflow.moduelid)
            //radworkflow.addWorkFlow.createnEditWorkFLow(true, radworkflow.workFlowName);
        },

        //Get All Work floWs HTML From Service.
        getAllWorkFlowsHTML: function () {
            var self = this;
            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc"

            $.ajax({
                url: url + '/GetHtmlTemplate',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivp.rad.RRadWorkflow.Resources.AllWorkFlows.html' })
            }).then(function (responseText) {
                $("#" + radworkflow.identifier).empty().append(responseText.d);
                $(".radAllWorkFlowGridBody").height($("#" + radworkflow.identifier).height() - 100);
                self.allWorkFlowEventHandlers();
                self.getAllWorkFlows();
                self.getModuleDetails();
            });
        },

        //Get All Work FLow From DB.
        getAllWorkFlows: function () {
            var self = this;
            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc"
            $.ajax({
                url: url + '/GetAllWorkFLows',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json'
            }).then(function (responseText) {
                self.allWorkFlowsList = responseText.d;
                if (self.allWorkFlowsList.length > 0) {
                    self.createWorkFlowHTML();
                    $(".radAllWFEmpty").addClass("radDisplayNoneClass");
                    $(".radAllWorkFlowBody").removeClass("radDisplayNoneClass");
                }
                else {
                    $(".radAllWFEmpty").removeClass("radDisplayNoneClass");
                    $(".radAllWorkFlowBody").addClass("radDisplayNoneClass");
                }
            });
        },


        //Get ALl Modules from mkodule master table.
        getModuleDetails: function () {
            var self = this;
            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc";
            $.ajax({
                url: url + "/GetModuleDetails",
                type: "POST",
                contentType: "application/json",
                dataType: "json"
            }).then(function (responseText) {
                self.allModules = $.parseJSON(responseText.d);
            })
        },

        //CReate Work FLow Grid
        createWorkFlowHTML: function () {
            var self = this;
            for (var i = 0; i < self.allWorkFlowsList.length; i++) {
                var divRow = $('<div>', {
                    class: 'radAllWorkFlowDataRow'

                });
                divRow.attr('workflowid', self.allWorkFlowsList[i].WorkflowID);
                divRow.attr('moduleid', self.allWorkFlowsList[i].moduleId);
                var divColumn = $('<div>', {
                    class: 'radWorkFlowHeaderItems radWFName',
                    text: self.allWorkFlowsList[i].WorkflowName
                });
                divColumn.attr("title", self.allWorkFlowsList[i].WorkflowName);
                divRow.append(divColumn);

                divColumn = $('<div>', {
                    class: 'radWorkFlowHeaderItems radWFDesc',
                    text: self.allWorkFlowsList[i].WorkflowDescription
                });
                divRow.append(divColumn);

                divColumn = $('<div>', {
                    class: 'radWorkFlowHeaderItems radWFModuleName',
                    text: self.allWorkFlowsList[i].moduleName
                });
                divRow.append(divColumn);

                divColumn = $('<div>', {
                    class: 'radWorkFlowHeaderItems radWFStates',
                    text: self.allWorkFlowsList[i].WorkflowStates.length
                });
                divRow.append(divColumn);
                $(".radAllWorkFlowGridBody").append(divRow);
                //divRow.append("<div class='radWorkFlowHeaderItems radWFBtns'><div class='radAllWorkFlowAuditParent'><div class='radAllWorkFlowAudit'>Audit</div></div></div>");
                //divRow.append("<div class='radWorkFlowHeaderItems radWFBtns'><div class='radAllWorkFlowEditParent'><div class='radWFTagNames'>" + self.getTagNamesToDisplay(self.allWorkFlowsList[i].Tags,divRow) + "</div><div class='radWFTagNamesEdit fa fa-pencil'></div></div></div>");
                divRow.append("<div class='radWorkFlowHeaderItems radWFBtns'><div class='radAllWorkFlowDeleteParent'><div class='radAllWorkFlowDelete fa fa-trash-o'></div></div></div>");
                divRow.append("<div class='radWorkFlowHeaderItems radWFBtns'><div class='radAllWorkFlowDownloadParent'><div class='radAllWorkFlowDownload fa fa-arrow-circle-o-down'></div></div></div>");
                //if (divRow.attr("tags") != null)
                //    divRow.find(".radWFTagNames").attr("title", divRow.attr("tags"));
            }
        },

        getTagNamesToDisplay: function (tags, divRow) {
            var self = this;
            var text = "";
            var length = tags.length > 2 ? 2 : tags.length;
            if (tags.length > 0)
                text = "";
            for (var i = 0; i < length; i++) {
                text += tags[i] + ",";
            }
            if (text.length > 0)
                text = text.substr(0, text.length - 1);
            if (tags.length)
                divRow.attr('tags', tags.join());
            return text;
        },

        //Bind Module Drop Down
        moduleDropDown:function(event) {
            var self = this;
            var arr = ['Polaris', 'Security Master', 'Ref Master'];
            var div = $("<div>", {
                class: 'radWFModuleParent'
            });
            for (var i = 0; i < self.allModules.length; i++) {
                if (self.allModules[i].ModuleName.toLowerCase().indexOf('rad') == -1) {
                    div.append($("<div>", {
                        class: 'radWFModule',
                        text: self.allModules[i].ModuleName,
                        moduleid: self.allModules[i].ModuleId
                    }))
                }

                //div.append($("<div>", {
                //    class: 'radWFModule',
                //    text: 'OMS',
                //    moduleid:16
                //}))

                //div.append($("<div>", {
                //    class: 'radWFModule',
                //    text: 'Security Master',
                //    moduleid: 3
                //}))

                //div.append($("<div>", {
                //    class: 'radWFModule',
                //    text: 'Ref Master',
                //    moduleid: 6
                //}))

                //div.append($("<div>", {
                //    class: 'radWFModule',
                //    text: 'EDM',
                //    moduleid: 14
                //}))
            }
            $(event.target).closest(".radAddWorkFlowModuleParent").append(div);
        },


        // Event Handlers.
        allWorkFlowEventHandlers: function () {
            var self = this;

            $(".radAllWorkFlowButtonBar").find(".radWFSearch").unbind('keyup').keyup(function (event) {
                var collection = [];
                collection = $(".radAllWorkFlowDataRow");
                var searchText = $(event.target).val().toLowerCase();
                for (var i = 0; i < collection.length; i++) {
                    if ($(collection[i]).find(".radWFName").html().toLowerCase().trim().indexOf(searchText) > -1) {
                        $(collection[i]).removeClass("radDisplayNoneClass");
                    }
                    else {
                        $(collection[i]).addClass("radDisplayNoneClass");
                    }
                }
            });
            $(".radAllWorkFlowButtonBar").unbind('click').click(function (event) {
                if ($(event.target).hasClass("radAddWorkFlow")) {
                    //radworkflow.addWorkFlow.createnEditWorkFLow(false);
                    $(".radWorkflowUploadPopUp").addClass("radDisplayNoneClass");
                    self.createAddWorkFlowPopUp(event);
                }
                else if ($(event.target).hasClass("radSaveWorkFlow")) {
                    if (self.validateWorkflowName($(event.target).closest(".radAddWorkFlowPopUpParent").find(".radWorkFlowNametext").text().trim()))
                        self.saveWorkFLow($(event.target).closest(".radAddWorkFlowPopUpParent").find(".radWorkFlowNametext").text().trim(), $(event.target).closest(".radAddWorkFlowPopUpParent").find("textarea").val().trim(), $(".radWorkFlowModuletext").attr('moduleid'));
                }
                else if ($(event.target).hasClass("radCancelWorkFlow")) {
                    self.removePopUps(event);
                }
                else if ($(event.target).hasClass("radWorkFlowModuletext")) {
                    self.moduleDropDown(event);
                }
                else if ($(event.target).hasClass("radWFModule")) {
                    $(".radWorkFlowModuletext").html($(event.target).html());
                    $(".radWorkFlowModuletext").attr('moduleid', $(event.target).attr("moduleid"));
                    $(".radWFModuleParent").remove()
                }
                else if ($(event.target).hasClass("radUploadWorkFlow")) {
                    self.removePopUps(event);
                    self.uploadPopUp();
                }
                else if ($(event.target).closest(".radAddWorkFlowPopUpParent").length == 0) {
                    self.removePopUps(event);
                }
            });
            $(".radAllWorkFlowBody").unbind('click').click(function (event) {
                if ($(event.target).hasClass("radWFName")) {
                    self.removePopUps(event);
                    var workFlowName = $(event.target).closest(".radAllWorkFlowDataRow").children().first().html();
                    radworkflow.addWorkFlow.createnEditWorkFLow(true, workFlowName);
                }
                else if ($(event.target).hasClass("radAllWorkFlowDelete")) {
                    self.removePopUps(event);
                    var div = radworkflow.utility.allWorkFlowDeleteHandler(event, 'Once deleted, this workflow cannot be recovered.');
                    div.addClass("raddltWorkFlowPopUp");
                    $(event.target).closest(".radAllWorkFlowDataRow").append(div);
                }
                else if ($(event.target).hasClass("radAllWorkFlowAudit")) {
                    self.removePopUps(event);
                    self.allWorkFlowAuditHandler(event);
                }
                else if ($(event.target).hasClass("radDeleteWorkFlowcancelButton")) {
                    $(".raddltWorkFlowPopUp").remove();
                }
                else if ($(event.target).hasClass("radDeleteWorkFlowOKButton")) {
                    self.deleteWorkFlow(event);
                }
                else if ($(event.target).hasClass("radAllWorkFlowDownload")) {
                    self.downLoadWorkflow(event);
                }
                else if ($(event.target).closest(".radAllWorkFlowEditParent").length > 0) {
                    self.createTagPopup(event);
                }
                else if ($(event.target).hasClass("radAddTagBtn")) {
                    $(".radWFAddTagBody").append($("<div>", {
                        class:'radWFaddNewTagParent'
                    }));
                    $(".radWFaddNewTagParent").last().append($("<div>", {
                        class: 'radWFaddNewTag'
                    }));
                    $(".radWFaddNewTag").attr('contenteditable', 'true');
                    $(".radSaveTagBtnParent").removeClass("radDisplayNoneClass");
                }
                else if ($(event.target).hasClass("radWFTagNameDelete")) {
                    var element = $(event.target).closest(".radAllWorkFlowDataRow");
                    $(event.target).parent().remove();
                    self.saveTags(element, true);
                }
                else if ($(event.target).hasClass("radSaveTagBtn")) {
                    self.saveTags($(event.target).closest(".radAllWorkFlowDataRow"), false);
                }
                else if ($(event.target).hasClass("radWorkflowUploadButton")) {
                    self.uploadWorkflow(event);
                }
                else if ($(event.target).hasClass("radWorkflowcross")) {
                    $(".radWorkflowUploadPopUp").addClass("radDisplayNoneClass");
                }
                else if ($(event.target).hasClass("radAllWorkFlowAudit")) {
                    self.getWorkflowAudit();
                }
                else {
                    self.removePopUps(event);
                }
            });
        },

        //Upload Workflow.
        uploadWorkflow: function (event) {
            var self = this;
            if (self.validateUploadWorkflowName($(event.target).closest(".radWorkflowUploadPopUp").find(".radWorkFlowNametext").html().trim()))
            $.ajax({
                url: radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc/UploadWorkflow",
                method: 'POST',
                contentType: "application/json",
                data: JSON.stringify({ cacheKey: "parent1", user: radworkflow.user, workflowName: $(".radWorkFlowNametext").html().trim() }),
                dataType: 'json'
            }).then(function (responseText) {
                if (responseText.d == "") {
                    radworkflow.utility.alertPopUp(true, "Workflow Uploaded Successfully");
                    setTimeout(function () {
                        radworkflow.handlers.getAllWorkFlowsHTML();
                    }, 2000);
                    
                }
                else {
                    $(".radWorkflowUploadPopUp").addClass("radDisplayNoneClass");
                    radworkflow.utility.alertPopUp(false, responseText.d);
                }
            });
        },

        saveTags: function (row,isDelete) {
            var self = this;
            var tags = [];
            var isDuplicateFlag = false;
            for (var i = 0 ; i < $(".radWFTagNamerow").length; i++) {
                if ($($(".radWFTagNamerow")[i]).html().trim() != "") {
                    if (tags.indexOf($($(".radWFTagNamerow")[i]).html().trim()) == -1)
                        tags.push($($(".radWFTagNamerow")[i]).html());
                    else
                        isDuplicateFlag = true;
                }
            }

            for (var i = 0 ; i < $(".radWFaddNewTag").length; i++) {
                if ($($(".radWFaddNewTag")[i]).html().trim() != "") {
                    if (tags.indexOf($($(".radWFaddNewTag")[i]).html().trim()) == -1)
                        tags.push($($(".radWFaddNewTag")[i]).html());
                    else
                        isDuplicateFlag = true;
                }
            }
            if (tags.length > 0 && !isDuplicateFlag) {
                $.ajax({
                    url: radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc/SaveTags",
                    method: 'POST',
                    contentType: "application/json",
                    data: JSON.stringify({ workflowid: row.attr("workflowid"), tags: tags.join(), moduleid: row.attr("moduleid"), user: radworkflow.user }),
                    dataType: 'json'
                }).then(function (responseText) {
                    if (responseText.d != null) {
                        var tags = $.parseJSON(responseText.d);
                        if (isDelete)
                            radworkflow.utility.alertPopUp(true, 'Tags Deleted Successfully');
                        else
                            radworkflow.utility.alertPopUp(true, 'Tags Added Successfully');
                        row.find(".radWFTagNames").html(self.getTagNamesToDisplay(tags, row));
                        if (!isDelete) {
                            $(".radWFAddTagBody").empty();
                            $(".radWFTagPopUpbody").empty();
                            for (var i = 0; i < tags.length; i++) {
                                var divChild = $('<div>', {
                                    class: 'radWFTagNamerowParent'
                                });
                                divChild.append($('<div>', {
                                    class: 'radWFTagNamerow',
                                    text: tags[i]
                                }));
                                divChild.append($('<div>', {
                                    class: 'radWFTagNameDelete fa fa-trash-o',
                                }));
                                $(".radWFTagPopUpbody").append(divChild);
                            }
                        }
                    }
                    else {
                        radworkflow.utility.alertPopUp(true, 'Error occurred while Adding Tags');
                    }
                })
            }
            else if (isDuplicateFlag) {
                radworkflow.utility.alertPopUp(true, 'Duplicate Tags not Allowed');
            }
        },

        //Create Tag Name Popup.
        createTagPopup:function(event) {
            var self = this;
            $(".radWFTagPopUpParent").remove();
            var div = $('<div>', {
                class:'radWFTagPopUpParent'
            });

            div.append($('<div>', {
                class: 'radWFTagPopUpHeader',
                text:'Tags'
            }));

            div.append($('<div>', {
                class: 'radAddTagBtnParent'
            }));

            div.find(".radAddTagBtnParent").append($('<div>', {
                class: 'radAddTagBtn fa fa-plus-circle'
            }));

            div.append($('<div>', {
                class: 'radWFTagPopUpbody'
            }));
            var tags = [];
            if ($(event.target).closest(".radAllWorkFlowDataRow").attr("tags") != null) {
                tags = $(event.target).closest(".radAllWorkFlowDataRow").attr("tags").split(",");
            }

            for (var i = 0 ; i < tags.length; i++) {
                var divChild = $('<div>', {
                    class: 'radWFTagNamerowParent'
                });
                divChild.append($('<div>', {
                    class: 'radWFTagNamerow',
                    text:tags[i]
                }));
                divChild.append($('<div>', {
                    class: 'radWFTagNameDelete fa fa-trash-o',
                }));
                div.find(".radWFTagPopUpbody").append(divChild);
            }
            if (tags.length == 0) {
                div.find(".radWFTagPopUpbody").html("No Tags Attached");
            }

            div.append($('<div>', {
                class: 'radWFAddTagPopUp'
            }));

            

            div.find(".radWFAddTagPopUp").append($('<div>', {
                class: 'radWFAddTagBody'
            }));

            div.find(".radWFAddTagPopUp").append($('<div>', {
                class: 'radSaveTagBtnParent radDisplayNoneClass'
            }));

            div.find(".radSaveTagBtnParent").append($('<div>', {
                class: 'radSaveTagBtn',
                text: 'Save Tag'
            }));

            $(event.target).closest(".radAllWorkFlowDataRow").append(div);
        },


        //Down Load Workflow Method.
        downLoadWorkflow:function(event) {
            var self = this;
            $.ajax({
                url: radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc/DownLoadWorkflow",
                method: 'POST',
                contentType: "application/json",
                data: JSON.stringify({ workFlowName: $(event.target).closest(".radAllWorkFlowDataRow").find(".radWFName").html().toLowerCase().toString() }),
                dataType: 'json'
            }).then(function (responseText) {
                if (responseText.d != "")
                    window.open("App_Dynamic_Resource/RRadWorkflow,com.ivp.rad.RRadWorkflow.Resources.RADGridExportToExcel.aspx?eventData=" + responseText.d + "", "_blank", "width=400,height=1,menubar=0,resizable=1");
            })
        },

        // Remove All Pop Ups
        removePopUps: function (event) {
            $(".raddltWorkFlowPopUp").remove();
            $(".radAddWorkFlowPopUpParent").remove();
            if ($(event.target).closest(".radWorkflowUploadPopUp").length == 0)
                $("#" + radworkflow.identifier).find(".radWorkflowUploadPopUp").addClass("radDisplayNoneClass");
            if ($(event.target).closest(".radWFTagPopUpParent").length == 0)
                $(".radWFTagPopUpParent").remove();
        },

        //Delete Work FLow
        deleteWorkFlow: function (event) {
            var self = this;
            var workFLowName = $(event.target).closest(".radAllWorkFlowDataRow").children().first().html();
            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc"
            $.ajax({
                url: url + '/DeleteWorkFLow',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ workFlowName: workFLowName, user:radworkflow.user })
            }).then(function (responseText) {
                if (responseText.d) {
                    $(".raddltWorkFlowPopUp").remove();
                    radworkflow.utility.alertPopUp(true, 'WorkFlow Deleted Successfully');
                    self.getAllWorkFlowsHTML();
                }
            });
        },

        

        //Create Add Work FLow Pop Up
        createAddWorkFlowPopUp: function (event) {
            var self = this;
            var div = $("<div>", {
                class: 'radAddWorkFlowPopUpParent'
            });
            //div.append($("<div>", {
            //    class: 'radAddWorkFlowNameHeader',
            //    text: 'Create WorkFlow'
            //}));

            //Append Workflow Name Div
            var divrptName = $("<div>", {
                class: 'radAddWorkFlowNameParent'
            });
            divrptName.append($("<div>", {
                class: 'radWorkFlowNamelabel',
                text: 'Work Flow Name'
            }));
            divrptName.append($("<div>", {
                class: 'radWorkFlowNametext',
                tabIndex: 1,
                contenteditable: true
            }));
            div.append(divrptName);
            divrptName.append($("<div>", {
                class: 'radWFValidation radDisplayNoneClass',
                text: "!"
            }));

            //Append Module Name Div
            var divrptName = $("<div>", {
                class: 'radAddWorkFlowModuleParent'
            });
            divrptName.append($("<div>", {
                class: 'radWorkFlowModulelabel',
                text: 'Module Name'
            }));
            divrptName.append($("<div>", {
                class: 'radWorkFlowModuletext',
                text:'Select Module'
            }));
            div.append(divrptName);
            divrptName.append($("<div>", {
                class: 'radWFValidation radDisplayNoneClass',
                text: "!"
            }));

            //Append Workflow Descroiption Div
            divrptName = $("<div>", {
                class: 'radAddWorkFlowDescParent'
            })
            //divrptName.append($("<div>", {
            //    class: 'radWorkFlowDesclabel',
            //    text: 'Work Flow Desc'
            //}));
            divrptName.append($("<div>", {
                class: 'radWorkFlowDesctext'
            }))
            divrptName.find(".radWorkFlowDesctext").append($("<textarea>", { placeholder: "WorkFlow Description" }));
            div.append(divrptName);
            div.append($("<div>", {
                class: 'radAddWorkFlowNameFooter'
            }))
            div.find(".radAddWorkFlowNameFooter").append("<div class='radSaveWorkFlowParent'><div class='radSaveWorkFlow'>Save</div></div>");
            div.find(".radAddWorkFlowNameFooter").append("<div class='radCancelWorkFlowParent'><div class='radCancelWorkFlow'>Cancel</div></div>");
            $(event.target).parent().append(div);
            $(".radWorkFlowNametext").focus();
        },

        saveWorkFLow: function (name, desc,moduleid) {
            var self = this;
            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc";
            radworkflow.addWorkFlow.currentWorkFlow = {};
            radworkflow.addWorkFlow.currentWorkFlow.WorkflowName = name.toString().trim();
            //radworkflow.addWorkFlow.currentWorkFlow.moduleId = desc;
            radworkflow.addWorkFlow.currentWorkFlow.moduleId = radworkflow.moduelid;
            radworkflow.addWorkFlow.currentWorkFlow.WorkflowStates = [];
            radworkflow.moduelid = moduleid;
            //radworkflow.addWorkFlow.createnEditWorkFLow(false, name);
            $.ajax({
                url: url + "/SaveWorkFlow",
                type: 'POST',
                contentType: 'application/json',
                dataType: 'json',
                data: JSON.stringify({ workflow: name.toString().trim(), desc: desc, moduleId: moduleid, userName: radworkflow.user })
            }).then(function (responseText) {
                if (responseText.d) {
                    radworkflow.addWorkFlow.currentWorkFlow.WorkflowID = responseText.d;
                    radworkflow.addWorkFlow.createnEditWorkFLow(false, name);
                    $(".radAddWorkFlowPopUpParent").remove();
                }
            })
        },


        validateUploadWorkflowName:function(name) {
            var self = this;
            if (name == "" || name == null) {
                $(".radWorkFlowNametext").next().removeClass("radDisplayNoneClass");
                $(".radWFValidation").html("!");
                $(".radWFValidation").attr("title", 'Workflow Name can not be empty');
                return false;
            }
            else if (!(!/[^a-z A-Z 0-9]/.test(name))) {
                $(".radWorkFlowNametext").next().removeClass("radDisplayNoneClass");
                $(".radWFValidation").html("!");
                $(".radWFValidation").attr("title", 'Workflow Name can not contain special characters');
                return false;
            }
            else {
                for (var i = 0; i < self.allWorkFlowsList.length; i++) {
                    if (self.allWorkFlowsList[i].WorkflowName.toString().toLowerCase() == name.toString().toLowerCase()) {
                        $(".radWorkFlowNametext").next().html("!");
                        $(".radWorkFlowNametext").next().removeClass("radDisplayNoneClass");
                        $(".radWorkFlowNametext").next().attr("title", 'Workflow Name already exists');
                        return false;
                    }
                }
            }
            $(".radWorkFlowNametext").next().addClass("radDisplayNoneClass");
            return true;
        },

        validateWorkflowName: function (name) {
            var self = this;
            if (name == "" || name == null) {
                $(".radWorkFlowNametext").next().removeClass("radDisplayNoneClass");
                $(".radWFValidation").attr("title", 'Workflow Name can not be empty');
                return false;
            }
            else if (!(!/[^a-z A-Z 0-9]/.test(name))) {
                $(".radWorkFlowNametext").next().removeClass("radDisplayNoneClass");
                $(".radWFValidation").html("!");
                $(".radWFValidation").attr("title", 'Workflow Name can not contain special characters');
                return false;
            }
            else {
                for (var i = 0; i < self.allWorkFlowsList.length; i++) {
                    if (self.allWorkFlowsList[i].WorkflowName.toString().toLowerCase() == name.toString().toLowerCase()) {
                        $(".radWorkFlowNametext").next().removeClass("radDisplayNoneClass");
                        $(".radWorkFlowNametext").next().attr("title", 'Workflow Name already exists');
                        return false;
                    }
                }
            }
            if ($(".radWorkFlowModuletext").html() == 'Select Module') {
                $(".radWorkFlowModuletext").next().removeClass("radDisplayNoneClass");
                $(".radWorkFlowModuletext").next().attr("title", 'Please Select a Module');
                return false;
            }
            return true;
        },

        uploadPopUp:function() {
            var self = this;
            $("#" + radworkflow.identifier).find(".radWorkflowUploadPopUp").removeClass("radDisplayNoneClass");
            $(".radWorkflowDragNDropRegion").empty();
            $(".radAddWorkFlowNameParent").remove();
            $(".radWorkflowHeader").after($('<div>', {
                class: 'radAddWorkFlowNameParent'
            }));
            $(".radAddWorkFlowNameParent").append($('<div>', {
                class: 'radWorkFlowNamelabel',
                text:'Workflow Name'
            }))
            $(".radAddWorkFlowNameParent").append($('<div>', {
                class: 'radWorkFlowNametext',
                tabindex: '1'
            }))
            $(".radWorkFlowNametext").attr("contenteditable", true);
            $(".radAddWorkFlowNameParent").append($('<div>', {
                class: 'radWFValidation radDisplayNoneClass'
            }))
            $(".radWorkflowDragNDropRegion").css({ 'display': 'inline-block' });
            $(".radWorkflowDragNDropRegion").append("<div style=\"text-indent: 8px;\"> </div>");
            //$(".radWorkflowDragNDropRegion").append("<div class=\"fa fa-upload radWFUploadButton\"></div>");

            $(".radWorkflowDragNDropRegion").append("<div class=\"radWFDropDiv\" id=\"attachmentButton\"><div class='radWFUploadButton' style=\"display:inline-block;\">DRAG AND DROP&nbsp&nbsp&nbsp&nbsp</div><div style=\"display:inline-block;\">OR&nbsp&nbsp</div><div style=\"display:inline-block;text-decoration: underline;\">Browse</div>");

            $(".radWorkflowDragNDropRegion").append("<div class=\"radWorkflowParentDropDrag\" id=\"parent1\"></div>");
            if ($('#parent1').fileUpload != undefined) {
                $('#parent1').fileUpload({
                    'parentControlId': 'parent1',
                    'attachementControlId': 'attachmentButton',
                    'multiple': false,
                    'debuggerDiv': '',
                    'deleteEvent': function () {
                    }
                });
            }
        },


        createUploadWorkflowPopUp: function () {
            var self = this;
            var div = $("<div>", {
                class: 'radUploadWorkFlowPopUpParent'
            });
            //div.append($("<div>", {
            //    class: 'radAddWorkFlowNameHeader',
            //    text: 'Create WorkFlow'
            //}));

            //Append Workflow Name Div
            var divrptName = $("<div>", {
                class: 'radAddWorkFlowNameParent'
            });
            divrptName.append($("<div>", {
                class: 'radWorkFlowNamelabel',
                text: 'Work Flow Name'
            }));
            divrptName.append($("<div>", {
                class: 'radWorkFlowNametext',
                tabIndex: 1,
                contenteditable: true
            }));
            div.append(divrptName);
            divrptName.append($("<div>", {
                class: 'radWFValidation radDisplayNoneClass',
                text: "!"
            }));

            //Append Module Name Div
            var divrptName = $("<div>", {
                class: 'radAddWorkFlowModuleParent'
            });
            divrptName.append($("<div>", {
                class: 'radWorkFlowModulelabel',
                text: 'Module Name'
            }));
            divrptName.append($("<div>", {
                class: 'radWorkFlowModuletext',
                text: 'Select Module'
            }));
            div.append(divrptName);
            divrptName.append($("<div>", {
                class: 'radWFValidation radDisplayNoneClass',
                text: "!"
            }));

            //Append Workflow Descroiption Div
            divrptName = $("<div>", {
                class: 'radAddWorkFlowDescParent'
            })
            //divrptName.append($("<div>", {
            //    class: 'radWorkFlowDesclabel',
            //    text: 'Work Flow Desc'
            //}));
            divrptName.append($("<div>", {
                class: 'radWorkFlowDesctext'
            }))
            divrptName.find(".radWorkFlowDesctext").append($("<textarea>", { placeholder: "WorkFlow Description" }));
            div.append(divrptName);
            div.append($("<div>", {
                class: 'radAddWorkFlowNameFooter'
            }))
            div.find(".radAddWorkFlowNameFooter").append("<div class='radSaveWorkFlowParent'><div class='radSaveWorkFlow'>Save</div></div>");
            div.find(".radAddWorkFlowNameFooter").append("<div class='radCancelWorkFlowParent'><div class='radCancelWorkFlow'>Cancel</div></div>");
            $(event.target).parent().append(div);
            $(".radWorkFlowNametext").focus();
        },



        getWorkflowAudit:function(workflowid) {
            var self = this;
            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc";
            $.ajax({
                url: url + "/GetWorkflowAudit",
                type: 'POST',
                contentType: 'application/json',
                dataType: 'json',
                data: JSON.stringify({ workflowid: name, startDate: "", enddate: ""})
            }).then(function (responseText) {
                if (responseText.d) {
                    radworkflow.addWorkFlow.currentWorkFlow.WorkflowID = responseText.d;
                    radworkflow.addWorkFlow.createnEditWorkFLow(false, name);
                    $(".radAddWorkFlowPopUpParent").remove();
                }
            })
        }

    }
})
$.extend(radworkflow, {
    utility: {
        alertPopUp: function (success, msg) {
            var self = this;
            $("#alertParentDiv1").remove();
            //$("#ContentDiv").append("<div id=\"alertParentDiv1\" class=\"ToolKitAlertPopUpParent\"></div>");
            $("#" + radworkflow.identifier).append("<div id=\"alertParentDiv1\" class=\"radworkflowAlertPopUpParent\"></div>");
            if (success)
                $("#alertParentDiv1").append("<div class=\"radworkflowPopupAlertUpperSuccessDiv\"></div>");
            else
                $("#alertParentDiv1").append("<div class=\"radworkflowPopupAlertUpperErrorDiv\"></div>");
            $("#alertParentDiv1").append("<div class=\"radworkflowPopUpError\"></div>")
            $("#alertParentDiv1").append("<div class=\"radworkflowErrorDivLeft radworkflowUserSuccess\"></div>"); //revisit
            $("#alertParentDiv1").append("<div class=\"radworkflowErrorDivRight\"></div>")
            $("#alertParentDiv1").css("top", "-200px");
            $("#alertParentDiv1").animate({ "top": "0px" });
            setTimeout(function () { $("#alertParentDiv1").remove() }, 4000);
            $(".radworkflowErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Result</div>")
            $(".radworkflowErrorDivRight").append("<div class=\"radworkflowErrorDivText\">" + msg + "</div>");
        },

        //Create Delete WorkFlow Confirmation Pop Up. 
        allWorkFlowDeleteHandler: function (event, showText) {
            var self = this;
            var div = $("<div>");

            div.append($('<div>', {
                class: 'radDltWorkFlowPopUpText',
                text: showText
            }));

            var divButtonBar = $("<div>", {
                class: 'radDeleteWorkFlowButtonBar'
            });

            var divButtonParent = $("<div>", {
                class: 'radDeleteWorkFlowOKButtonParent'
            });

            divButtonParent.append($("<div>", {
                class: 'radDeleteWorkFlowOKButton',
                'text': 'Ok'
            }));
            divButtonBar.append(divButtonParent);

            var divButtonParent = $("<div>", {
                class: 'radDeleteWorkFlowCancelButtonParent'
            });

            divButtonParent.append($("<div>", {
                class: 'radDeleteWorkFlowcancelButton',
                'text': 'Cancel'
            }));
            divButtonBar.append(divButtonParent);
            div.append(divButtonBar);
            return div;

        },

        isNotNullorEmpty: function (value) {
            if (value != null && value != "")
                return true;
            else
                return false;
        }
    }
});
$.extend(radworkflow, {
    addWorkFlow: {
        //Create N Edit Work FLow Start Function. Edit Flag indicates for Edit or Create WorkFLow.
        createnEditWorkFLow: function (isEdit, workFlowName, workflowDesc) {
            var self = this;
            self.workFlowName = workFlowName;
            self.jsPlumbInstance = jsPlumb.getInstance();
            self.getAddWorkFlowHTML(isEdit);
        },

        //Get Add Work FLow HTML from Service
        getAddWorkFlowHTML: function (isEdit, workFlowName) {
            var self = this;
            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc";
            $.ajax({
                url: url + "/GetHtmlTemplate",
                type: 'POST',
                contentType: 'application/json',
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivp.rad.RRadWorkflow.Resources.AddWorkFlow.html' })
            }).then(function (responseText) {
                $("#" + radworkflow.identifier).empty().append(responseText.d);
                self.showHideEditActions(radworkflow.IsEditable);
                $(".radCreateWFName").html(self.workFlowName);
                self.eventHandlers();
                self.jsPlumbInstance.setContainer($(".radAddWorkFlowBodyPlumb"));
                $(".radAddWorkFlowBody").height($("#" + radworkflow.identifier).height() - 50);
                if (isEdit) {
                    //$(".radAddNewState").addClass("radDisplayNoneClass");
                    self.getWorkFlowInfo();
                }
                else
                    self.createStartState();

                
            });
        },


   

        createStartState: function () {
            var self = this;
            self.createNewState(event, "Start", "radWFStartState");
            $("div[statename='Start']").find(".radWorkFlowStateNameEdit,.radWorkFlowStateNameDelete").addClass("radDisplayNoneClass");
        },

        //Get WorkFLow Info for a specific Work Flow Info.
        getWorkFlowInfo: function () {
            var self = this;
            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc";
            $.ajax({
                url: url + "/GetWorkflowInfo",
                type: 'POST',
                contentType: 'application/json',
                dataType: 'json',
                data: JSON.stringify({ workflowName: self.workFlowName })
            }).then(function (responseText) {
                if (responseText.d != null) {
                    self.currentWorkFlow = $.parseJSON(responseText.d);
                    radworkflow.rulrGrammarInfo.Columns = [];
                    radworkflow.moduelid = self.currentWorkFlow.moduleId;
                    self.createWorkFlowStateDiagram();
                    if (self.currentWorkFlow.workflowAttibutes != null) {
                        self.bindAllAttributes(self.currentWorkFlow.workflowAttibutes)
                    }
                    if (self.currentWorkFlow.workflowConfig != null) {
                        self.setWorkflowConfig();
                    }

                    if (self.currentWorkFlow.canDelete) {
                        $(".radAddNewStateParent").removeClass("radDisplayNoneClass");
                        $(".radSaveWFParent").removeClass("radDisplayNoneClass");
                        
                    }
                    else {
                        $(".radAddNewStateParent").addClass("radDisplayNoneClass");
                        $(".radSaveWFParent").addClass("radDisplayNoneClass");
                    }


                    if (self.currentWorkFlow.canDelete) {
                        self.jsPlumbInstance.unbind("click").bind("click", function (conn, event) {
                            var stateName = $("#" + conn.sourceId).closest(".radWorkFLowStateItem").find(".radWorkFlowStateName").html().trim();
                            var actionName = $("#" + conn.sourceId).attr("action");
                            self.detachAction(stateName, actionName);
                            self.jsPlumbInstance.detach(conn);
                        });
                        
                    }

                    self.jsPlumbInstance.unbind("connection").bind("connection", function (connInfo, event) {
                        if (connInfo.sourceId.toLowerCase() == "start" && (connInfo.targetId.toLowerCase() == "end" || connInfo.targetId.toLowerCase() == "failed")) {
                            self.jsPlumbInstance.detach(connInfo);
                            return false;
                        }
                    });

                    //self.jsPlumbInstance.unbind("connection").bind("connection", function (connInfo, originalEvent) {
                    //    //init(connInfo.connection);
                    //    self.jsPlumbInstance.connect({
                    //        source: connInfo.sourceId,
                    //        target: connInfo.targetId,
                    //        detachable: true,
                    //        overlays: [
                    //            ["PlainArrow", { width: 7, length: 7, location: 0.97 }]
                    //        ]
                    //    });
                    //});
                    self.hideDeleteUpdateforStates();
                }
            })
        },

        //Deatch action from Action Array of Workflow State 
        detachAction:function(stateName,actionName) {
            var self = this;
            var currentState = self.currentWorkFlow.WorkflowStates.filter(function (state, index) {
                return state.StateName == stateName;
            })[0];
            var index = -1;    
            for (var i = 0; i < currentState.StateActions.length; i++) {
                if (currentState.StateActions[i].Action == actionName) {
                    index = i;
                    break;
                }
            }
            if (index > -1)
                currentState.StateActions.splice(index, 1);
        },

        setWorkflowConfig: function () {
            var self = this;
            if (self.currentWorkFlow.workflowConfig.OneUserOnlyOnce) {
                $(".radWFConfig").attr("config", "OneUserOnlyOnce");
                $(".radWFConfigCircle").first().addClass("radWFConfigSelected");
            }
            else if (self.currentWorkFlow.workflowConfig.NoSameUserAtAdjacentState) {
                $(".radWFConfig").attr("config", "NoSameUserAtAdjacentState");
                $($(".radWFConfigCircle")[1]).addClass("radWFConfigSelected");
            }
            else if (self.currentWorkFlow.workflowConfig.SkipLevel) {
                $(".radWFConfig").attr("config", "SkipLevel");
                $($(".radWFConfigCircle")[2]).addClass("radWFConfigSelected");
            }
            else if (self.currentWorkFlow.workflowConfig.SkipLevelWithAction) {
                $(".radWFConfig").attr("config", "SkipLevelWithAction");
                $(".radWFConfigCircle").last().addClass("radWFConfigSelected");
            }
        },

        //Add all attributes as Columns in Grammar Info  for Rule Editor.
        bindAllAttributes: function (attributesList) {
            var self = this;
            if (attributesList != null) {
                for (var i = 0; i < attributesList.length; i++) {
                    var attr = new self.getGrammarColumnConfig();
                    if (radworkflow.utility.isNotNullorEmpty(attributesList[i].value))
                        attr.ColumnEnumValuesJson = attributesList[i].value.split(',');
                    attr.ColumnName = attributesList[i].name;
                    attr.actualColumnName = attributesList[i].name;
                    attr.DataTypeJson = attributesList[i].dataType;
                    attr.DataType = (attributesList[i].dataType == "Text" ? 1 : (attributesList[i].dataType == "Numeric" ? 2 : 3));
                    radworkflow.rulrGrammarInfo.Columns.push(attr);
                }
            }
            else {
                self.currentWorkFlow.workflowAttibutes = [];
            }
        },


        //Column COnfiguration for RUle Editor Grammar Info.
        getGrammarColumnConfig: function () {
            this.ActionSideApplicability = 1;
            this.ActionSideApplicabilityJson = "RHS";
            this.ColumnEnumValues = Object;
            this.ColumnEnumValuesJson = Array[0];
            this.ColumnName = "Beneficiary Address";
            this.ColumnType = 0;
            this.ColumnTypeJson = "Both";
            this.DataType = 1;
            this.DataTypeJson = "Text";
            this.ExpressionSideApplicability = 0;
            this.ExpressionSideApplicabilityJson = "BOTH";
            this.IsAggregationColumn = false;
            this.IsRhsColumn = true;
            this.IsRhsEnum = false;
            this.IsRhsUserInput = true;
            this.actualColumnName = "Beneficiary_Address";
            this.columnPrefix = "";
        },

        //Create Work Flow State Diagram.
        createWorkFlowStateDiagram: function () {
            var self = this;
            var startItem = '';
            var endItem = '';
            var stateLength = self.currentWorkFlow.WorkflowStates.length;
            var actionLength = 0;
            for (var i = 0; i < stateLength; i++) {
                var className = null;
                if (self.currentWorkFlow.WorkflowStates[i].StateName == "End")
                    className = "radWFEndState";
                else if (self.currentWorkFlow.WorkflowStates[i].StateName == "Failed")
                    className = "radWFFailedState";
                else if (self.currentWorkFlow.WorkflowStates[i].StateName == "Start")
                    className = "radWFStartState";
                $(".radAddWorkFlowBodyPlumb").append(self.createStateDiv(self.currentWorkFlow.WorkflowStates[i], className));
            }
            var state = null;
            for (var i = 0; i < stateLength; i++) {
                actionLength = self.currentWorkFlow.WorkflowStates[i].StateActions.length
                state = self.currentWorkFlow.WorkflowStates[i];
                if (state.StateName == "Start")
                    self.makejsPlumbStateTarget(state.StateName.replace(new RegExp(' ', 'gi'), '_'), true, false);
                else {
                    self.makejsPlumbStateTarget(state.StateName.replace(new RegExp(' ', 'gi'), '_'), false, true);
                }
                for (var j = 0; j < actionLength; j++) {
                    if (state.StateActions[j].Action != "" || state.StateName == "Start") {
                        startItem = state.StateName.replace(new RegExp(' ', 'gi'), '_') + state.StateActions[j].Action.replace(new RegExp(' ', 'gi'), '_');
                        if (state.StateActions[j].NextStateName != "")
                            endItem = state.StateActions[j].NextStateName.replace(new RegExp(' ', 'gi'), '_');
                        else
                            endItem = null;
                        self.jsPlumbConnect(startItem, endItem, true, true, (state.StateActions[j].Position == "" ? "Left" : state.StateActions[j].Position));
                    }
                }
            }
        },

        //Create HTML Div for State Div
        createStateDiv: function (state, className) {
            var self = this;
            var div = $('<div>', {
                class: 'radWorkFLowStateItem'
            });
            if (className != null)
                div.addClass(className);
            div.attr("stateName", state.StateName);
            if (state.stateId != null)
                div.attr("stateId", state.stateId);
            self.setStatePosition(div, state);
            var divChild = $('<div>', {
                class: 'radWorkFlowStateNameSection'
            });
            divChild.append($('<div>', {
                class: 'radWorkFlowStateName',
                id: state.StateName.replace(new RegExp(' ', 'gi'), '_'),
                text: state.StateName
            }));
            if (state.StateName == "Start") {
                divChild.find(".radWorkFlowStateName").attr("action", "");
                divChild.find(".radWorkFlowStateName").attr("state", "Start");
            }

            divChild.append($('<div>', {
                class: 'radWorkFlowStateNameEdit fa fa-pencil-square-o'
            }));
            div.append(divChild);

            divChild.append($('<div>', {
                class: 'radWorkFlowStateNameDelete fa fa-trash-o'
            }));
            div.append(divChild);

            if (state.StateActions.length > 0) {
                self.createActionHTML(state, div);
            }
            if (state.StateName.indexOf('newState') > -1) {
                div.find(".radWorkFlowStateName").attr('contenteditable', true);
                div.find(".radWorkFlowStateName").attr('tabIndex', 1);
            }
            //div.css({ 'top': ((($(".radWorkFLowStateItem").children().length * 100) + 50 ) + 'px') });
            //div.css({ 'left': ((($(".radWorkFLowStateItem").children().length * 100) +  50) + 'px') });
            return div;
        },

        //Set State Div Position
        setStatePosition: function (div, state) {
            var self = this;
            if (self.currentWorkFlow.position != null) {
                if (self.currentWorkFlow.position[state.stateId] != null) {
                    var containerWidth = $(".radAddWorkFlowBodyPlumb").width();
                    var conatinerHeight = $(".radAddWorkFlowBodyPlumb").height();
                    var top = (self.currentWorkFlow.position[state.stateId].positionTop / 100) * conatinerHeight;
                    var left = (self.currentWorkFlow.position[state.stateId].positionLeft / 100) * containerWidth;
                    div.css({ 'top': (top + 'px') });
                    div.css({ 'left': (left + 'px') });
                }
            }
        },

        //Create Action HTML WHICH Acts as connectors for the States
        createActionHTML: function (state, div) {
            var self = this;
            if (state.StateActions.length > 0) {
                //div.find(".radWorkFlowActionNameSectionParent").remove();
                var domCollection = div.find(".radWorkFlowActionNameSection");
                for (var i = 0; i < domCollection.length; i++) {
                    var action = state.StateActions.find(act => act.Action == $(domCollection[i]).attr("action"));
                    if (action == null)
                        self.jsPlumbInstance.remove($(domCollection[i]));
                }
                div.addClass("radWorkFLowStateItemPadding");
                var actiondiv = null;
                var actionDivParent = $('<div>', {
                    class: 'radWorkFlowActionNameSectionParent'
                });
                if (div.find(".radWorkFlowActionNameSectionParent").length > 0)
                    actionDivParent = div.find(".radWorkFlowActionNameSectionParent");
                else
                    div.append(actionDivParent);
                for (var i = 0; i < state.StateActions.length; i++) {
                    if (actionDivParent.find("div[action='" + state.StateActions[i].Action + "']").length == 0) {
                        state.StateActions[i].IsNewAdded = true;
                        if (state.StateActions[i].Action != "") {
                            actiondiv = $('<div>', {
                                class: 'radWorkFlowActionNameSection',
                                id: (state.StateName.replace(new RegExp(' ', 'gi'), '_') + state.StateActions[i].Action.replace(new RegExp(' ', 'gi'), '_'))
                            });
                            actiondiv.attr('action', state.StateActions[i].Action);
                            actiondiv.attr('state', state.StateName);
                            actiondiv.append($('<div>', {
                                class: 'radWorkFlowActionName',
                                text: state.StateActions[i].Action,
                                title: state.StateActions[i].Action
                            }));
                            actiondiv.append($('<div>', {
                                class: 'radWorkFlowActionNameImgs'
                            }));
                            actionDivParent.append(actiondiv);
                        }
                    }
                    else
                        state.StateActions[i].IsNewAdded = false;
                }
            }
        },

        //Add Add Point to State Names ON Left Side. State Name is always a target.
        makejsPlumbStateTarget: function (startItem, isSource, isTarget) {
            var self = this;
            var anchors = ["Left"];
            var maxCon = 8;
            if (startItem == "Start") {
                anchors = ["Right"];
                isSource = true;
                isTarget = false;
                maxCon = 1;
                self.makejsPlumbStateTargetELement(startItem, isSource, isTarget, maxCon, anchors);
            }
            else {
                anchors = ["Left"];
                self.makejsPlumbStateTargetELement(startItem, isSource, isTarget, maxCon, anchors);
                anchors = ["Right"];
                self.makejsPlumbStateTargetELement(startItem, isSource, isTarget, maxCon, anchors);
                anchors = ["Top"];
                self.makejsPlumbStateTargetELement(startItem, isSource, isTarget, maxCon, anchors);
            }
            self.jsPlumbInstance.draggable($("#" + startItem).closest(".radWorkFLowStateItem"), { containment: "parent" });
        },

        makejsPlumbStateTargetELement: function (startItem, isSource, isTarget, maxCon, anchors) {
            var self = this;
            self.jsPlumbInstance.addEndpoint(startItem, {
                isSource: isSource,
                isTarget: isTarget,
                maxConnections: maxCon,
                endpoint: ['Rectangle', { enpointid: startItem, width: 7, height: 7, zIndex: 1, cssClass: 'radCustomEndPoint' }],
                paintStyle: { strokeStyle: 'lightgray', fillStyle: "lightgray" },
                connectorStyle: { strokeStyle: '#a9c0df', lineWidth: 2, dashstyle: "2 1 2 1" },
                anchors: anchors,
                connector: ["Flowchart"],
                deleteEndpointsOnDetach: false,
                dragAllowedWhenFull: false,
                connectionsDetachable: true
            });
        },

        //Connected Source and Target.
        jsPlumbConnect: function (startItem, endItem, isSource, isTarget, anchorEnd) {
            var self = this;
            if (startItem != null) {
                var start = self.jsPlumbInstance.addEndpoint(startItem, {
                    isSource: isSource,
                    isTarget: false,
                    maxConnections: 1,
                    endpoint: ['Rectangle', { width: 7, height: 7, zIndex: 1, cssClass: 'radCustomEndPoint', fillStyle: 'lightgray' }],
                    paintStyle: { strokeStyle: 'lightgray', fillStyle: "lightgray" },
                    connectorStyle: { strokeStyle: '#a9c0df', lineWidth: 2, dashstyle: "2 1 2 1" },
                    anchors: ["Right"],
                    connector: ["Flowchart"],
                    deleteEndpointsOnDetach: false,
                    dragAllowedWhenFull: false,
                    connectionsDetachable: true
                });
            }
            if (endItem != null && $("#" + endItem).length > 0) {
                var end = self.jsPlumbInstance.addEndpoint(endItem, {
                    isSource: false,
                    isTarget: isTarget,
                    maxConnections: 8,
                    endpoint: ['Rectangle', { enpointid: endItem, width: 7, height: 7, cssClass: 'radCustomEndPoint', zIndex: 1 }],
                    paintStyle: { strokeStyle: 'lightgray', fillStyle: "lightgray" },
                    connectorStyle: { strokeStyle: '#a9c0df', lineWidth: 2, dashstyle: "2 1 2 1" },
                    anchors: anchorEnd,
                    connector: ["Flowchart"],
                    deleteEndpointsOnDetach: false,
                    dragAllowedWhenFull: false,
                    connectionsDetachable: true
                });
            }
            if (startItem != null && (endItem != null && $("#" + endItem).length > 0)) {
                self.jsPlumbInstance.connect({
                    source: start,
                    target: end,
                    detachable: true,
                    overlays: [
                        ["PlainArrow", { width: 7, length: 7, location: 0.97 }]
                    ]
                });
            }
            //jsPlumb.draggable($("#" + startItem));
            //jsPlumb.draggable($("#" + endItem));
        },

        //Highlight COnnections on hover of this State
        highlightConnections: function (event) {
            var self = this;
            var targetId = $(event.target).closest(".radWorkFlowStateNameSection").find(".radWorkFlowStateName").html();
            targetId = targetId.replace(new RegExp('[ ]', 'gi'), '_');
            var dummyConn = self.jsPlumbInstance.getConnections({ target: targetId });
            //if (dummyConn.length > 0) {
            //    $.each(dummyConn, function (k, v) {
            //        var position = [];
            //        if (dummyConn[k].endpoints[1].anchor.anchors != null)
            //            position.push(dummyConn[k].endpoints[1].anchor.anchors[0].type);
            //        else
            //            position.push(dummyConn[k].endpoints[1].anchor.type);
            //        self.jsPlumbInstance.detach(dummyConn[k]);
            //        self.jsPlumbConnect(dummyConn[k].sourceId, dummyConn[k].targetId, true, true, position);
            //    });
            //}
            dummyConn = self.jsPlumbInstance.getConnections({ target: targetId });
            if (dummyConn.length > 0) {
                $.each(dummyConn, function (k, v) {
                    if (targetId == "Failed" || targetId == "End")
                        $('#' + dummyConn[k].targetId).closest(".radWorkFlowStateNameSection").addClass('ItemHovered');
                    else
                        $('#' + dummyConn[k].targetId).closest(".radWorkFLowStateItemPadding").addClass('ItemHovered');
                    dummyConn[k].addClass('ItemArrowHovered');
                });
            }

            var actions = $(event.target).closest(".radWorkFLowStateItemPadding").find(".radWorkFlowActionNameSection");
            if (actions.length > 0) {
                for (var i = 0; i < actions.length; i++) {
                    var targetId = $(actions[i]).attr("id");
                    //var dummyConn = self.jsPlumbInstance.getConnections({ source: targetId });
                    //if (dummyConn.length > 0) {
                    //    $.each(dummyConn, function (k, v) {
                    //        var position = [];
                    //        if (dummyConn[k].endpoints[1].anchor.anchors != null)
                    //            position.push(dummyConn[k].endpoints[1].anchor.anchors[0].type);
                    //        else
                    //            position.push(dummyConn[k].endpoints[1].anchor.type);
                    //        self.jsPlumbInstance.detach(dummyConn[k]);
                    //        self.jsPlumbConnect(dummyConn[k].sourceId, dummyConn[k].targetId, true, true, position);
                    //    });
                    //}
                    dummyConn = self.jsPlumbInstance.getConnections({ source: targetId });
                    if (dummyConn.length > 0) {
                        $.each(dummyConn, function (k, v) {
                            //$('#' + dummyConn[k].targetId).closest(".radWorkFLowStateItemPadding").addClass('ItemHovered');
                            dummyConn[k].addClass('ItemArrowHovered');
                        });
                    }
                }
            }
        },

        unhighlightConnections:function() {
            var self = this;
            var targetId = $(event.target).closest(".radWorkFlowStateNameSection").find(".radWorkFlowStateName").html();
            var dummyConn = self.jsPlumbInstance.getConnections({ target: targetId });
            targetId = targetId.replace(new RegExp('[ ]', 'gi'), '_');
            //if (dummyConn.length > 0) {
            //    $.each(dummyConn, function (k, v) {
            //        var position = [];
            //        if (dummyConn[k].endpoints[1].anchor.anchors != null)
            //            position.push(dummyConn[k].endpoints[1].anchor.anchors[0].type);
            //        else
            //            position.push(dummyConn[k].endpoints[1].anchor.type);
            //        self.jsPlumbInstance.detach(dummyConn[k]);
            //        self.jsPlumbConnect(dummyConn[k].sourceId, dummyConn[k].targetId, true, true, position);
            //    });
            //}
            dummyConn = self.jsPlumbInstance.getConnections({ target: targetId });
            if (dummyConn.length > 0) {
                $.each(dummyConn, function (k, v) {
                    if (targetId == "Failed" || targetId == "End")
                        $('#' + dummyConn[k].targetId).closest(".radWorkFlowStateNameSection").removeClass('ItemHovered');
                    else
                        $('#' + dummyConn[k].targetId).closest(".radWorkFLowStateItemPadding").removeClass('ItemHovered');
                    dummyConn[k].removeClass('ItemArrowHovered');
                });
            }

            var actions = $(event.target).closest(".radWorkFLowStateItemPadding").find(".radWorkFlowActionNameSection");
            if (actions.length > 0) {
                for (var i = 0; i < actions.length; i++) {
                    var targetId = $(actions[i]).attr("id");
                    var dummyConn = self.jsPlumbInstance.getConnections({ source: targetId });
                    //if (dummyConn.length > 0) {
                    //    $.each(dummyConn, function (k, v) {
                    //        var position = [];
                    //        if (dummyConn[k].endpoints[1].anchor.anchors != null)
                    //            position.push(dummyConn[k].endpoints[1].anchor.anchors[0].type);
                    //        else
                    //            position.push(dummyConn[k].endpoints[1].anchor.type);
                    //        self.jsPlumbInstance.detach(dummyConn[k]);
                    //        self.jsPlumbConnect(dummyConn[k].sourceId, dummyConn[k].targetId, true, true, position);
                    //    });
                    //}
                    dummyConn = self.jsPlumbInstance.getConnections({ source: targetId });
                    if (dummyConn.length > 0) {
                        $.each(dummyConn, function (k, v) {
                            //$('#' + dummyConn[k].targetId).closest(".radWorkFLowStateItemPadding").addClass('ItemHovered');
                            dummyConn[k].removeClass('ItemArrowHovered');
                        });
                    }
                }
            }
        },

        //Check If State Name already Exists for this Workflow.
        checkDuplicateStateName: function (stateName) {
            var self = this;
            var matchedStates = [];
            matchedStates = self.currentWorkFlow.WorkflowStates.filter((state) => state.StateName.toLowerCase() == stateName);
            if (matchedStates.length == 0)
                return false;
            else
                return true;
        },


        //Add Work Flow Screen Event Handler.
        eventHandlers: function () {
            var self = this;
            $(".radAddWorkFlowMain").unbind("mouseover").on("mouseover", ".radWorkFlowStateNameSection", function (event) {
                if ($(event.target).closest(".raddltStatePopUp").length == 0)
                    self.highlightConnections(event);
            });

            $(".radAddWorkFlowMain").unbind("mouseout").on("mouseout", ".radWorkFlowStateNameSection", function () {
                self.unhighlightConnections(event);
            });


            $(".radAddWorkFlowMain").unbind("click").click(function (event) {
                if ($(event.target).hasClass("radAddNewState")) {
                    self.removePopups();
                    self.createNewStatePopUp();
                    //self.createNewState(event);
                }
                else if ($(event.target).hasClass("radWFStateSave")) {
                    var stateName = $(".radWFStateText").find("input").val();
                    if (!self.checkDuplicateStateName(stateName.toLowerCase())) {
                        self.removePopups();
                        if (stateName.toString().toLowerCase() == "start" || stateName.toString().toLowerCase() == "end" || stateName.toString().toLowerCase() == "failed")
                            radworkflow.utility.alertPopUp(false, "State Name " + stateName + " can not be created");
                        else {
                            if (stateName.toString().trim() == "") {
                                radworkflow.utility.alertPopUp(false, "State Name can not be Empty");
                            }
                            else if (!(!/[^a-z A-Z 0-9]/.test(stateName)))
                                radworkflow.utility.alertPopUp(false, "State Name can not contain special characters");
                            else
                                self.createNewState(event, stateName);
                        }
                    }
                    else {
                        radworkflow.utility.alertPopUp(false, "State Name " + stateName + " already Exists");
                    }
                }
                else if ($(event.target).hasClass("radWorkFlowStateName")) {
                    self.removePopups();
                    if ($(event.target).attr("contenteditable") != null) {
                        $(event.target).addClass("radWorkFlowStateEdit");
                        self.oldstateName = $(event.target).html()
                        $(event.target).html("");
                        $(event.target).append($('<input>', {
                            type: 'text',
                            class: 'radWorkFlowStateNameEdittxt',
                            placeholder: self.oldstateName
                        }));
                    }
                }
                else if ($(event.target).hasClass("radWorkFlowStateNameEdit")) {
                    self.removePopups();
                    if ($(event.target).closest(".radWorkFLowStateItem").attr("statename") != "Start") {
                        radworkflow.rulrGrammarInfo.Columns = [];
                        self.bindAllAttributes(self.currentWorkFlow.workflowAttibutes);
                        self.getCurrentState($(event.target).closest(".radWorkFLowStateItem").attr("statename"));
                    }
                }
                else if ($(event.target).hasClass("radWorkFlowStateNameDelete")) {
                    var div = radworkflow.utility.allWorkFlowDeleteHandler(event, 'Are you sure you want to delete this State');
                    if ($(".radAddWorkFlowBodyPlumb").width() > (event.pageX + 280))
                        div.addClass("raddltStatePopUp");
                    else
                        div.addClass("raddltStatePopUpRight");
                    $(event.target).closest(".radWorkFlowStateNameSection").append(div);
                }
                else if ($(event.target).hasClass("radDeleteWorkFlowOKButton")) {
                    self.deleteState($(event.target).closest(".radWorkFlowStateNameSection").find(".radWorkFlowStateName").html(), event);
                }
                else if ($(event.target).hasClass("radDeleteWorkFlowcancelButton")) {
                    $(".raddltStatePopUp").remove();
                    $(".raddltStatePopUpRight").remove();
                }
                else if ($(event.target).hasClass("radAddAttribute")) {
                    self.createAttributePopUp(event);
                }
                else if ($(event.target).hasClass("radAllWF")) {
                    radworkflow.handlers.getAllWorkFlowsHTML();
                }
                else if ($(event.target).hasClass("radSaveWF")) {
                    self.removePopups();
                    self.saveWorkFlowStateMap(event);
                }
                else if ($(event.target).hasClass("radWFConfig")) {
                    self.populateWFConfig(event);
                }
                else if ($(event.target).closest(".radWFConfigRow").length > 0) {
                    if ($(event.target).closest(".radWFConfigRow").find(".radWFConfigSelected").length > 0) {
                        $(".radWFConfigSelected").removeClass("radWFConfigSelected");
                        $(".radWFConfig").removeAttr("config");
                    }
                    else {
                        $(".radWFConfigSelected").removeClass("radWFConfigSelected");
                        $(event.target).closest(".radWFConfigRow").find(".radWFConfigCircle").addClass("radWFConfigSelected");
                        self.addConfig(event);
                    }
                }
                else if ($(event.target).hasClass("radWFCreateEndState")) {
                    if ($(".radAddWorkFlowBodyPlumb").find(".radWFEndState").length == 0) {
                        self.removePopups();
                        self.createNewState(event, "End", "radWFEndState");
                    }
                    else
                        radworkflow.utility.alertPopUp(false, "State already exists for This Workflow");
                }
                else if ($(event.target).hasClass("radWFCreateFailedState")) {
                    if ($(".radAddWorkFlowBodyPlumb").find(".radWFFailedState").length == 0) {
                        self.removePopups();
                        self.createNewState(event, "Failed", "radWFFailedState");
                    }
                    else
                        radworkflow.utility.alertPopUp(false, "State already exists for This Workflow");
                }
                else if ($(event.target).closest(".raddltStatePopUp").length == 0 && $(event.target).closest(".radWFStatePopup").length == 0 && $(event.target).closest(".radAddAttrPopUPParent").length == 0 && $(event.target).closest(".radWFConfigPopUPParent").length == 0) {
                    self.removePopups();
                }

            });





            $(".radAddWorkFlowMain").unbind("keyup").keyup(function (event) {
                if ($(event.target).hasClass("radWorkFlowStateNameEdittxt")) {
                    if (event.which == 13) {
                        var parent = $(event.target).parent();
                        $(event.target).closest('.radWorkFLowStateItem').attr('statename', $(event.target).val().trim());
                        $(event.target).parent().removeAttr('contenteditable');
                        $(event.target).closest('.radWorkFLowStateItem').find(".radWorkFlowActionNameSection").attr('state', $(event.target).val().trim());
                        var stateName = $(event.target).val().trim();
                        parent.empty().html(stateName).removeClass("radWorkFlowStateEdit");
                        self.updateStateName(stateName)
                    }
                }
            });


        },

        addConfig: function (event) {
            var self = this;
            if ($(event.target).closest(".radWFConfigRow").attr("config") == "OneUserOnlyOnce")
                $(".radWFConfig").attr("config", "OneUserOnlyOnce");
            else if ($(event.target).closest(".radWFConfigRow").attr("config") == "NoSameUserAtAdjacentState")
                $(".radWFConfig").attr("config", "NoSameUserAtAdjacentState");
            else if ($(event.target).closest(".radWFConfigRow").attr("config") == "SkipLevel")
                $(".radWFConfig").attr("config", "SkipLevel");
            else if ($(event.target).closest(".radWFConfigRow").attr("config") == "SkipLevelWithAction")
                $(".radWFConfig").attr("config", "SkipLevelWithAction");
        },

        removePopups: function () {
            $(".radWFStatePopup").remove();
            $(".radAddAttrPopUPParent").remove();
            $(".radWFConfigPopUPParent").addClass("radDisplayNoneClass");
            $(".raddltStatePopUp").remove();
            $(".raddltStatePopUpRight").remove();
        },

        //Populate Workflow Configuration
        populateWFConfig: function (event) {
            var self = this;
            $(".radWFConfigPopUPParent").removeClass("radDisplayNoneClass");
        },


        updateStateName: function (stateName) {
            var self = this;
            for (var i = 0 ; i < self.currentWorkFlow.WorkflowStates.length; i++) {
                if (self.currentWorkFlow.WorkflowStates[i].StateName == self.oldStateName) {
                    self.currentWorkFlow.WorkflowStates[i].StateName = stateName;
                    break;
                }
            }
        },

        //GetCurrentState on Edit CLick;
        getCurrentState: function (stateName) {
            var self = this;
            var state = null;
            for (var i = 0 ; i < self.currentWorkFlow.WorkflowStates.length; i++) {
                if (self.currentWorkFlow.WorkflowStates[i].StateName == stateName) {
                    state = self.currentWorkFlow.WorkflowStates[i];
                    break;
                }
            }
            if (state != null) {
                radworkflow.workflowstates.GetHTML(radworkflow.moduelid, state, self.currentWorkFlow.WorkflowID, state.stateId > 0 ? true : false);
            }
        },

        createNewStatePopUp: function (event) {
            var self = this;
            var divMain = $("<div>", {
                class: 'radWFStatePopup'
            });

            var div = $("<div>", {
                class: 'radWFNewStateParent'
            });
            divMain.append(div);

            div.append($("<div>", {
                class: 'radWFStateLabelparent'
            }));

            div.find(".radWFStateLabelparent").append($("<div>", {
                class: 'radWFStateLabel',
                text: 'State Name'
            }));

            div.find(".radWFStateLabelparent").append($("<div>", {
                class: 'radWFStateText',
            }));
            //div.find(".radWFStateText").attr('tabIndex', 1);
            //div.find(".radWFStateText").attr('contenteditable', true);
            div.find(".radWFStateText").append($("<input>"));
            div.find('input').attr('placeholder', 'New State');
            div.append($("<div>", {
                class: 'radWFStateSave fa fa-arrow-right'
            }));

            $(".radAddWorkFlowBodyPlumb").append(divMain);

            divMain.append($("<div>", {
                class: 'radWFEndStateLabelparent'
            }));
            divMain.find(".radWFEndStateLabelparent").append($("<div>", {
                class: 'radWFEndStateLabel',
                text: 'Create End State'
            }));
            divMain.find(".radWFEndStateLabelparent").append($("<div>", {
                class: 'radWFCreateEndState fa fa-arrow-right'
            }));

            divMain.append($("<div>", {
                class: 'radWFFailedStateLabelparent'
            }));
            divMain.find(".radWFFailedStateLabelparent").append($("<div>", {
                class: 'radWFFailedStateLabel',
                text: 'Create Failed State'
            }));
            divMain.find(".radWFFailedStateLabelparent").append($("<div>", {
                class: 'radWFCreateFailedState fa fa-arrow-right'
            }));


            div.find('input').focus();
        },

        //Create New State
        createNewState: function (event, stateName, className) {
            var self = this;
            self.newState = {};
            self.newState.StateName = stateName;
            self.newState.StateActions = [];
            self.newState.StateActions.push({ Action: '', NextStateName: '', MappedUsers: [], EscalationMappedUsers: [] });
            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc";
            $.ajax({
                url: url + "/SaveState",
                type: "POST",
                contentType: 'application/json',
                dataType: 'json',
                data: JSON.stringify({ stateName: self.newState.StateName, workflowId: self.currentWorkFlow.WorkflowID, user: radworkflow.user })
            }).then(function (responseText) {
                if (responseText.d > 0) {
                    self.newState.stateId = responseText.d;
                    var startItem = self.createStateDiv(self.newState, className);
                    startItem.css({ 'top': '0px' });
                    startItem.css({ 'right': '30px' });
                    $(".radAddWorkFlowBodyPlumb").append(startItem);
                    self.hideDeleteUpdateforStates();
                    var anchors = ["Left"];
                    var maxCon = 8;
                    var source = false;
                    var target = true;
                    startItem = startItem.find(".radWorkFlowStateName");
                    if (self.newState.StateName == "Start") {
                        anchors = ["Right"];
                        maxCon = 1;
                        source = true;
                        target = false;
                        self.makejsPlumbStateTargetELement(startItem, source, target, maxCon, anchors);
                    }
                    else {
                        self.makejsPlumbStateTargetELement(startItem, source, target, maxCon, ["Left"]);
                        self.makejsPlumbStateTargetELement(startItem, source, target, maxCon, ["Right"]);
                        self.makejsPlumbStateTargetELement(startItem, source, target, maxCon, ["Top"]);
                    }

                    //}

                    //self.jsPlumbInstance.addEndpoint(startItem, {
                    //    isSource: source,
                    //    isTarget: target,
                    //    maxConnections: maxCon,
                    //    endpoint: ['Rectangle', { width: 10, height: 10, zIndex: 1, cssClass: 'radCustomEndPoint' }],
                    //    paintStyle: { strokeStyle: 'lightgray', fillStyle: "lightgray" },
                    //    connectorStyle: { strokeStyle: 'lightgray', lineWidth: 2, dashstyle: "2 1 2 1" },
                    //    anchors: anchors,
                    //    connector: ["Flowchart"],
                    //    deleteEndpointsOnDetach: false,
                    //    dragAllowedWhenFull: false
                    //});
                    self.jsPlumbInstance.draggable(startItem.closest(".radWorkFLowStateItem"), { containment: "parent" });
                    radworkflow.addWorkFlow.currentWorkFlow.WorkflowStates.push(self.newState);

                }
                else {
                    radworkflow.utility.alertPopUp("Error occurred while saving State.", false);
                }
            });
            //radworkflow.addWorkFlow.currentWorkFlow.WorkflowStates.push(state);
            //state.StateActions = self.getDummyStateAction();
            //self.createActionNodes(state);
        },



        //Create Action Nodes on Save State
        createActionNodes: function (state) {
            var self = this;
            var startItem = null;
            var endItem = null;
            var actionLength = state.StateActions.length;
            self.createActionHTML(state, $(".radAddWorkFlowBodyPlumb").find(".radWorkFLowStateItem[stateName='" + (state.StateName) + "']"));
            for (var j = 0; j < actionLength; j++) {
                //startItem = state.StateName.replace(new RegExp(' ', 'gi'), '_') + state.StateActions[i].Action.replace(new RegExp(' ', 'gi'), '_');
                if (state.StateActions[j].IsNewAdded) {
                    startItem = state.StateName.replace(new RegExp(' ', 'gi'), '_') + state.StateActions[j].Action.replace(new RegExp(' ', 'gi'), '_');
                    if (radworkflow.utility.isNotNullorEmpty(state.StateActions[j].NextStateName))
                        endItem = state.StateActions[j].NextStateName.replace(new RegExp(' ', 'gi'), '_');
                    self.jsPlumbConnect(startItem, endItem, true, true, ["Left"]);
                }
            }
        },


        getDummyStateAction: function () {
            var self = this;
            var arr = []
            arr.push({ Action: "Passed" });
            arr.push({ Action: "Failed" });
            return arr;
        },

        //Create Attribute (For Rule Editor Grammar) Pop Up
        createAttributePopUp: function (event) {
            var self = this;
            if ($(".radAddAttrPopUPParent").length > 0) {
                $(".radAddAttrPopUPParent").remove();
            }
            else {
                radworkflow.attributes.pageLoad();
            }
        },

        createConnections: function (state) {
            var self = this;
            self.createActionNodes(state);
            $(".radAddWorkFlowMain")[0].style.opacity = 1;
        },


        saveWorkFlowStateMap: function (event) {
            var self = this;
            var allconnetcions = self.jsPlumbInstance.getAllConnections();
            $(allconnetcions).each(function (connection) {
                var connectionObj = this;
                var position = "";
                if (connectionObj.endpoints[1].anchor.anchors != null)
                    position = connectionObj.endpoints[1].anchor.anchors[0].type;
                else
                    position = connectionObj.endpoints[1].anchor.type;
                var conactionName = $($(this)[0].source).attr("action");
                var constateName = $($(this)[0].source).attr("state");
                _.filter(self.currentWorkFlow.WorkflowStates, function (state) {
                    var stateName = state.StateName;
                    if (stateName == constateName) {
                        _.filter(state.StateActions, function (action) {
                            if (action.Action == conactionName) {
                                action.NextStateName = $($(connectionObj)[0].target).closest(".radWorkFLowStateItem").attr('statename');
                                action.Position = position;
                            }
                        })
                    }
                })
            });
            var containerWidth = $(".radAddWorkFlowBodyPlumb").width();
            var conatinerHeight = $(".radAddWorkFlowBodyPlumb").height();
            self.currentWorkFlow.position = {};
            $(self.currentWorkFlow.WorkflowStates).each(function (index) {
                var position = {};
                position.positionLeft = Math.floor(($(".radAddWorkFlowBodyPlumb").find(".radWorkFLowStateItem[statename='" + this.StateName + "']").position().left / containerWidth) * 100);
                position.positionTop = Math.floor(($(".radAddWorkFlowBodyPlumb").find(".radWorkFLowStateItem[statename='" + this.StateName + "']").position().top / conatinerHeight) * 100);
                self.currentWorkFlow.position[this.stateId] = position;
            });

            if (self.currentWorkFlow.workflowConfig == null)
                self.currentWorkFlow.workflowConfig = {};
            self.currentWorkFlow.workflowConfig.OneUserOnlyOnce = false;
            self.currentWorkFlow.workflowConfig.NoSameUserAtAdjacentState = false;
            self.currentWorkFlow.workflowConfig.SkipLevel = false;

            if ($(".radWFConfig").attr("config") != null) {
                self.currentWorkFlow.workflowConfig[$(".radWFConfig").attr("config")] = true;
            }

            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc";
            $.ajax({
                url: url + "/SaveWorkflowActionMapping",
                type: "POST",
                contentType: 'application/json',
                dataType: 'json',
                data: JSON.stringify({ workflow: JSON.stringify(self.currentWorkFlow), user: radworkflow.user })
            }).then(function (responseText) {
                if (responseText.d) {
                    radworkflow.utility.alertPopUp(true, 'Mapping Saved Successfully');
                }
                else {
                    radworkflow.utility.alertPopUp(false, 'Error occurred while saving Mapping');
                }
            });


        },

        deleteState: function (stateName, event) {
            var self = this;
            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkflow.svc"
            var deleteStateName = stateName;
            $.ajax({
                url: url + "/DeleteState",
                contentType: "application/json",
                type: "POST",
                dataType: 'json',
                data: JSON.stringify({ stateName: stateName, workflowId: self.currentWorkFlow.WorkflowID, user: radworkflow.user })
            }).then(function (responseText) {
                if (responseText.d) {
                    self.jsPlumbInstance.remove($(event.target).closest(".radWorkFLowStateItem").find(".radWorkFlowStateName"));
                    var actionElements = $(event.target).closest(".radWorkFLowStateItem").find(".radWorkFlowActionNameSection");
                    for (var i = 0; i < actionElements.length; i++) {
                        self.jsPlumbInstance.remove($(actionElements[i]));
                    }
                    $(event.target).closest(".radWorkFlowStateNameSection").parent().remove();
                    var deleteIndex = 0;
                    for (var i = 0; i < radworkflow.addWorkFlow.currentWorkFlow.WorkflowStates.length; i++) {
                        if (radworkflow.addWorkFlow.currentWorkFlow.WorkflowStates[i].StateName == deleteStateName) {
                            deleteIndex = i;
                            break;
                        }
                    }
                    radworkflow.addWorkFlow.currentWorkFlow.WorkflowStates.splice(deleteIndex, 1);
                }
                else {
                    radworkflow.utility.alertPopUp(false,"This state cannot be Deleted. Workflow not completed.");
                }
            })
        },

        hideDeleteUpdateforStates: function () {
            $("div[statename='Start']").find(".radWorkFlowStateNameEdit,.radWorkFlowStateNameDelete").addClass("radDisplayNoneClass");
            //$("div[statename='End']").find(".radWorkFlowStateNameDelete").addClass("radDisplayNoneClass").prev().addClass("radWFBoundaryStates");
            //$("div[statename='Failed']").find(".radWorkFlowStateNameDelete").addClass("radDisplayNoneClass").prev().addClass("radWFBoundaryStates");
            //if (!radworkflow.IsEditable) {
            //    $(".radWorkFlowStateNameDelete").addClass("radDisplayNoneClass");
            //}
            //else {
            //    $(".radWorkFlowStateNameDelete").removeClass("radDisplayNoneClass");
            //}
        },

        showHideEditActions: function (flag) {
            var self = this;
            if (!flag) {
                $(".radAddWorkFlowHeader").addClass("radDisplayNoneClass");
            }
            else {
                $(".radAddWorkFlowHeader").removeClass("radDisplayNoneClass");
            }
            
        }
    }
});
$.extend(radworkflow, {
    workflowstates: {
        GetHTML: function (moduleId, state, workflowId, AddNewState) {
            var self = this;
            $(".radAddWorkFlowMain")[0].style.opacity = .3
            self.moduleID = moduleId;
            self.StateInfo = state;
            self.workflowId = workflowId;
            self.Info = [];
            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc"
            if (AddNewState) {
                $.ajax({
                    url: url + '/GetStateInfo',
                    type: 'POST',
                    contentType: "application/json",
                    data: JSON.stringify({ stateId: state.stateId }),
                    dataType: 'json'
                }).then(function (responseText) {
                    self.StateInfo = $.parseJSON(responseText.d);
                    if (self.StateInfo.StateName.toLowerCase() == "failed" || self.StateInfo.StateName.toLowerCase() == "end") {
                        self.EditMailFunc();
                    }
                    else
                        self.GetHTMLFUNC(AddNewState);
                });
            }
            else {
                self.StateInfo.stateId = 0
                self.GetHTMLFUNC(AddNewState);
            }
        },
        GetHTMLFUNC: function (AddNewState) {
            var self = this;
            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc"
            $.ajax({
                url: url + '/GetHtmlTemplate',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivp.rad.RRadWorkflow.Resources.StateManagement.html' })
            }).then(function (responseText) {

                self.html = responseText.d;
                $("#" + radworkflow.identifier).append(responseText.d);
                if (!radworkflow.IsEditable || !radworkflow.addWorkFlow.currentWorkFlow.canDelete) {
                    $(".RADWorkFlowStateSaveButton").addClass("radDisplayNoneClass");
                }
                else {
                    $(".RADWorkFlowStateSaveButton").removeClass("radDisplayNoneClass");
                }
                if (self.StateInfo.mailInfo != null && self.StateInfo.mailInfo != "") {
                    $(".RADWorkFlowStateEditMail").show();
                }
                $(".RADWorkFlowStateConfigurationTitle").html(self.StateInfo.StateName);

                //Key Down Event Handler for Escalation Number text box.
                $(".RADWFEscalateText").keydown(function (event) {
                    if (!((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105) || event.keyCode == 8 || event.keyCode == 46)) {
                        return false;
                    }
                })

                //Key Down Event Handler for Minimum Number of Users for approval.
                $(".radWFActionConfig").find("input").keydown(function (event) {
                    if (!((event.keyCode >= 48 && event.keyCode <= 57) || (event.keyCode >= 96 && event.keyCode <= 105) || event.keyCode == 8 || event.keyCode == 46)) {
                        return false;
                    }
                })

                $(".radWFActionConfig").find("input").keyup(function (event) {
                    if (event.keyCode != 8 && event.keyCode != 46) {
                        var selectedAction = $(".RADWorkFlowStateActionItemColor").html();
                        $.each(self.StateInfo.StateActions, function (index,action) {
                            if (action.Action == selectedAction) {
                                action.Config = $(event.target).val();
                            }
                        });
                    }
                })

                $(".RADWorkFlowActionHeaderTitle").html(self.StateInfo.StateName);

                if (self.StateInfo.escalationInfo != null) {
                    if (self.StateInfo.escalationInfo.Duration > 0)
                        $(".RADWFEscalateText").html(self.StateInfo.escalationInfo.Duration);
                }

                if (AddNewState) {
                    if (radworkflow.utility.isNotNullorEmpty(self.StateInfo.escalationInfo.EscalationMail.to) || radworkflow.utility.isNotNullorEmpty(self.StateInfo.mailInfo.to)) {
                        $(".RADWorkFlowStateEditMail").removeClass("RADHidden")
                        $(".RADOffCss").removeClass("RADOffCss")
                        $($(".RADWorkFlowStateMailToggleElement")[0]).addClass("RADOnCSS")
                    }
                    if (self.StateInfo.pubsubInfo.exchangeName != null && self.StateInfo.pubsubInfo.routingKey != null) {
                        $(".RADWorkFlowExchangeName").text(self.StateInfo.pubsubInfo.exchangeName)
                        $(".RADWorkFlowRoutingKeyName").text(self.StateInfo.pubsubInfo.routingKey)
                    }
                }
                if (self.StateInfo.StateActions.length > 0) {
                    $("#RADWorkFlowActionStateConfigurationComponent").show();
                    $("#RADWorkFlowActionComponent").hide();
                }
                else {
                    $("#RADWorkFlowActionStateConfigurationComponent").hide();
                    $("#RADWorkFlowActionComponent").show();
                }
                self.getAllUsers();
            });
        },

        //GetAllUsers
        getAllUsers: function () {
            var self = this;
            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc"
            $.ajax({
                url: url + '/GetAllUsers',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json'
            }).then(function (responseText) {
                self.UsersNGroupList = responseText.d;
                self.UsersList = [];
                self.GroupList = [];
                if (self.StateInfo.StateActions.length > 0) {
                    if (self.StateInfo.StateActions[0].MappedUsers.length > 0) {
                        self.showHideAddNewStateBtn(0);
                        $(".RADWorkFlowAddConditionParent").hide();
                        if (Object.keys(self.StateInfo.StateActions[0].MappedUsers[0].Name).length > 0) {
                            $(".RADWorkFlowStateUserComponentParent").hide();
                            $(".RADWorkFlowStateUserMappedParent").show();
                            $(".radWFActionConfig").removeClass("radDisplayNoneClass");
                        }
                        $(".radWFActionConfig").find("input").val(self.StateInfo.StateActions[0].Config);
                    }
                    else {
                        $(".radWFActionConfig").addClass("radDisplayNoneClass");
                        for (var i = 0 ; i < self.UsersNGroupList.length; i++) {
                            if (self.UsersNGroupList[i].Value == "user") {
                                self.UsersList.push({ Usertext: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                            }
                            if (self.UsersNGroupList[i].Value == "group") {
                                self.GroupList.push({ GroupText: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                            }
                        }
                        self.calculateGroupDivHeight();
                    }

                }
                else {
                    for (var i = 0 ; i < self.UsersNGroupList.length; i++) {
                        if (self.UsersNGroupList[i].Value == "user") {
                            self.UsersList.push({ Usertext: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                        }
                        if (self.UsersNGroupList[i].Value == "group") {
                            self.GroupList.push({ GroupText: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                        }
                    }
                    self.calculateGroupDivHeight();
                }

                $.ajax({
                    url: url + '/GetAllWorkFlowActions',
                    type: 'POST',
                    contentType: "application/json",
                    data: JSON.stringify({ moduleid: self.moduleID }),
                    dataType: 'json'
                }).then(function (responseText) {
                    self.ActionList = responseText.d;
                    self.ActionListItems = [];
                    self.SelectedActionListItems = [];
                    for (var j = 0; j < self.StateInfo.StateActions.length; j++) {
                        //if (self.StateInfo.StateActions[j].Action == self.ActionList[i]) {
                            //count++;
                            //self.ActionListItems.push({ Text: self.ActionList[i], selected: ko.observable(true) })
                            self.SelectedActionListItems.push(self.StateInfo.StateActions[j].Action);
                            //break;
                        //}
                    }
                    for (var i = 0 ; i < self.ActionList.length; i++) {
                        //var count = 0;
                        //for (var j = 0; j < self.StateInfo.StateActions.length; j++) {
                        //    if (self.StateInfo.StateActions[j].Action == self.ActionList[i]) {
                        //        count++;
                        //        self.ActionListItems.push({ Text: self.ActionList[i], selected: ko.observable(true) })
                        //        self.SelectedActionListItems.push(self.ActionList[i])
                        //        break;
                        //    }
                        //}
                        //if (count == 0)
                        if (self.SelectedActionListItems.indexOf(self.ActionList[i]) == -1)
                            self.ActionListItems.push({ Text: self.ActionList[i], selected: ko.observable(false) });
                        else {
                            if (self.ActionListItems.indexOf(self.ActionList[i]) == -1)
                            self.ActionListItems.push({ Text: self.ActionList[i], selected: ko.observable(true) });
                        }
                    }
                    self.BuildViewModel(self.UsersList, self.GroupList, self.ActionListItems, self.SelectedActionListItems);
                })
            });
        },


        BuildViewModel: function (users, group, actionList, selectedActionList) {
            var closure = this;

            closure.State = function () {
                var self = this;
                self.Actions = ko.observableArray(actionList);
                self.SelectedActions = ko.observableArray(selectedActionList);
                self.Users = ko.observableArray(users);
                self.Groups = ko.observableArray(group);
                self.Userquery = ko.observable('');
                self.Groupquery = ko.observable('');
                self.query = ko.observable('');
                self.mappedInfo = ko.observableArray([]);
                self.check = function (val, event) {
                    if (isNaN($(event.target).text())) {
                        $(".RADWFEscalateText").text("")
                    }
                }
                self.PopulateSelectedActions = function (model, event) {
                    if (model.selected() == false) {
                        model.selected(true);
                        self.SelectedActions.push(model.Text)
                    }
                    else {
                        model.selected(false);
                        self.SelectedActions.splice(self.SelectedActions().indexOf(model.Text), 1)
                        var index = -1;
                        for (var i = 0 ; i < closure.StateInfo.StateActions.length; i++) {
                            if (closure.StateInfo.StateActions[i].Action == model.Text) {
                                index = i;
                                break;
                            }
                        }
                        if (index > -1)
                            closure.StateInfo.StateActions.splice(index, 1);
                    }
                    // closure.PopulateSelectedActionsFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.GoToNextPage = function (model, event) {
                    closure.GoToNextPageFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.GetCss = function (model) {
                    if (model.selected() == false) {
                        return 'RADWorkFlowActionComponentItem';
                    }
                    else {
                        return 'RADWorkFlowActionComponentItem radselected';
                    }
                }
                self.GetUserCss = function (model) {
                    if (model.selected() == false) {
                        return 'RADWorkFlowStateUserParentDiv';
                    }
                    else {
                        return 'RADWorkFlowStateUserParentDiv radselect';
                    }
                }
                self.AddAction = function (model, event) {
                    closure.AddActionFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.GetGroupCss = function (model) {
                    if (model.selected() == false) {
                        return 'RADWorkFlowStateGroupParentDiv';
                    }
                    else {
                        return 'RADWorkFlowStateGroupParentDiv select';
                    }
                }
                self.GetGroupCircleCss = function (model) {
                    if (model.selected())
                        return "RADWorkflowCircleSelected fa fa-check";

                }
                self.CancelThisView = function (model, event) {
                    closure.CancelThisViewFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.BackToEarlierPage = function (model, event) {
                    closure.BackToEarlierPageFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.EscalationOn = function (model, event) {
                    closure.EscalationOnFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.EscalationOff = function (model, event) {
                    closure.EscalationOffFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.SaveState = function (model, event) {
                    closure.SaveStateFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.ShowUserAndGroup = function (model, event) {
                    closure.ShowUserAndGroupFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.AddRuleEditor = function (model, event) {
                    closure.AddRuleEditorFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.SelectGroupForAction = function (model, event) {
                    closure.SelectGroupForActionFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.SelectUserForAction = function (model, event) {
                    closure.SelectUserForActionFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.SaveOneItem = function (model, event) {
                    closure.SaveOneItemFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.CancelSelections = function (model, event) {
                    closure.cancelSelection(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.NormalMode = function (model, event) {
                    closure.NormalModeFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.EscalatedMode = function (model, event) {
                    closure.EscalatedModeFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.MailON = function (model, event) {
                    closure.MailONFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.MailOFF = function (model, event) {
                    closure.MailOFFFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.Usersearch = function (value) {
                    closure.UsersearchFunc(value);
                }
                self.Groupsearch = function (value) {
                    closure.GroupsearchFunc(value);
                }
                self.ShowMappedUsers = function (model, event) {
                    $(".RADpopUPforUsersandGroup").empty();
                    $(".RADpopUPforUsersandGroup").show();
                    //closure.ShowMappedUsersFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.ShowMappedGroupsText = function (index) {
                    return closure.ShowMappedGroupsTextFunc(index());
                }
                self.ShowMappedGroups = function (model, event) {
                    $(".RADpopUPforUsersandGroup").empty();
                    $(".RADpopUPforUsersandGroup").show();
                    //closure.ShowMappedGroupsFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.ClearTheDiv = function (model, event) {
                    $(".RADpopUPforUsersandGroup").empty();
                    $(".RADpopUPforUsersandGroup").hide();
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.EditMappedState = function (model, event) {

                    closure.EditMappedStateFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.DeleteMappedState = function (model, event) {
                    closure.DeleteMappedStateFunction(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.AddNewStateMappedUsers = function (model, event) {
                    closure.AddNewStateMappedUsersFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.EditMail = function (model, event) {
                    closure.EditMailFunc(model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }
                
            }
            closure.StateBindings = new closure.State();
            if (closure.StateInfo.StateActions.length > 0)
                for (var i = 0; i < closure.StateInfo.StateActions[0].MappedUsers.length; i++)
                    if (closure.StateInfo.StateActions[0].MappedUsers[i].ruleText == "")
                        closure.StateBindings.mappedInfo.push({ RuleText: "" });
                    else
                        closure.StateBindings.mappedInfo.push({ RuleText: closure.StateInfo.StateActions[0].MappedUsers[i].ruleText });
            ko.applyBindings(closure.StateBindings, $("#RADWorkFlowMainParent")[0]);
            closure.StateBindings.Userquery.subscribe(closure.StateBindings.Usersearch)
            closure.StateBindings.Groupquery.subscribe(closure.StateBindings.Groupsearch)
        },

        EditMailFunc: function (model, event) {
            var self = this;
            radworkflow.mailsubscription.pageLoad(self.StateInfo);
        },

        AddNewStateMappedUsersFunc: function (model, event) {
            var self = this;
            if ($($($(".RADWorkFlowStateUserMapDiv")[0]).find(".RADWorkFlowStateUserMapppedRule")).text() == "" || $($($(".RADWorkFlowStateUserMapDiv")[0]).find(".RADWorkFlowStateUserMapppedRule")).text() == null) {
                var msg = "Can't Add Condition since your first condition doesn't contains any rule"
                self.ShowErrorMessage(msg)
                return;
            }
            else {
                self.StateBindings.Users.splice(0, self.StateBindings.Users().length)
                self.StateBindings.Groups.splice(0, self.StateBindings.Groups().length)
                for (var i = 0 ; i < self.UsersNGroupList.length; i++) {
                    if (self.UsersNGroupList[i].Value == "user") {
                        self.StateBindings.Users.push({ Usertext: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                    }
                    if (self.UsersNGroupList[i].Value == "group") {
                        self.StateBindings.Groups.push({ GroupText: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                    }
                }
                $(".RADWorkFlowSaveOneActionInfo").removeClass("clickable");
                $(".RADWorkFlowAddConditionParent").show();
                $(".RADWorkFlowRuleEditorDivParent").empty();
                $(".RADWorkFlowStateUserMappedParent").hide();
                $(".RADWorkFlowStateAdd").hide()
                $(".RADWorkFlowStateUserComponentParent").show();
                self.AddRuleEditorFunc();
            }
        },

        EditMappedStateFunc: function (model, event) {
            var self = this;
            self.StateBindings.Users.splice(0, self.StateBindings.Users().length)
            self.StateBindings.Groups.splice(0, self.StateBindings.Groups().length)
            index = $($(event.target).closest(".RADWorkFlowStateUserMapDiv")).attr("index")
            self.indexOfEdit = index

            if ($(".RADEscalatedCss").length == 0) {
                var positionOfAction = 0
                for (var i = 0; i < self.StateInfo.StateActions.length; i++) {
                    if (self.StateInfo.StateActions[i].Action == $(".RADWorkFlowStateActionItemColor").text()) {
                        var obj = self.StateInfo.StateActions[i].MappedUsers[index].Name;
                        for (var j = 0; j < Object.keys(obj).length; j++) {
                            if (Object.values(obj)[j] == "true" || Object.values(obj)[j] == true) {
                                //$(".RADpopUPforUsersandGroup").append("<div class='UserListforpopUp' title=" + Object.keys(self.StateInfo.StateActions[i].MappedUsers[$($(event.target).closest(".RADWorkFlowStateUserMapDiv")).attr("index")].Name)[j] + ">" + Object.keys(self.StateInfo.StateActions[i].MappedUsers[$($(event.target).closest(".RADWorkFlowStateUserMapDiv")).attr("index")].Name)[j] + "</div>")
                                self.StateBindings.Users.push({ Usertext: Object.keys(obj)[j], selected: ko.observable(true) })
                            }
                            else if (Object.values(obj)[j] == "false" || Object.values(obj)[j] == false) {
                                self.StateBindings.Groups.push({ GroupText: Object.keys(obj)[j], selected: ko.observable(true) })
                            }
                        }
                        positionOfAction = i;
                        break;
                    }
                }
                for (var i = 0 ; i < self.UsersNGroupList.length; i++) {

                    if (self.UsersNGroupList[i].Value == "user") {
                        var count = 0;
                        for (var j = 0; j < self.StateBindings.Users().length; j++) {
                            if (self.StateBindings.Users()[j].Usertext == self.UsersNGroupList[i].Key) {
                                count++
                            }
                        }
                        if (count == 0)
                            self.StateBindings.Users.push({ Usertext: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                    }
                    else if (self.UsersNGroupList[i].Value == "group") {
                        var count = 0;
                        for (var j = 0; j < self.StateBindings.Groups().length; j++) {
                            if (self.StateBindings.Groups()[j].GroupText == self.UsersNGroupList[i].Key) {
                                count++
                            }
                        }
                        if (count == 0)
                            self.StateBindings.Groups.push({ GroupText: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                    }
                }

                $(".RADWorkFlowAddConditionParent").hide();
                self.checkRuleEditorDiv();
                if (self.StateInfo.StateActions[positionOfAction].MappedUsers[index].ruleText != "" && self.StateInfo.StateActions[positionOfAction].MappedUsers[index].ruleText != null) {
                    $(".RADWorkFlowRuleEditorDivParent").show();
                    $(".RADWorkFlowRuleEditorDivParent").removeClass("radDisplayNoneClass");
                    self.initRuleEngine(self.StateInfo.StateActions[positionOfAction].MappedUsers[index].ruleText);
                }
                else {
                    $(".RADWorkFlowRuleEditorDivParent").hide();
                    $(".RADWorkFlowAddConditionParent").show();
                }
                self.resetButtons(event);
            }
            else {
                var positionOfAction = 0
                for (var i = 0; i < self.StateInfo.StateActions.length; i++) {
                    if (self.StateInfo.StateActions[i].Action == $(".RADWorkFlowStateActionItemColor").text()) {
                        var obj = self.StateInfo.StateActions[i].EscalationMappedUsers[index].Name;
                        for (var j = 0; j < Object.keys(obj).length; j++) {
                            if (Object.values(obj)[j] == "true" || Object.values(obj)[j] == true) {
                                //$(".RADpopUPforUsersandGroup").append("<div class='UserListforpopUp' title=" + Object.keys(self.StateInfo.StateActions[i].MappedUsers[$($(event.target).closest(".RADWorkFlowStateUserMapDiv")).attr("index")].Name)[j] + ">" + Object.keys(self.StateInfo.StateActions[i].MappedUsers[$($(event.target).closest(".RADWorkFlowStateUserMapDiv")).attr("index")].Name)[j] + "</div>")
                                self.StateBindings.Users.push({ Usertext: Object.keys(obj)[j], selected: ko.observable(true) })
                            }
                            else if (Object.values(obj)[j] == "false" || Object.values(obj)[j] == false) {
                                self.StateBindings.Groups.push({ GroupText: Object.keys(obj)[j], selected: ko.observable(true) })
                            }
                        }
                        positionOfAction = i;
                        break;
                    }
                }

                for (var i = 0 ; i < self.UsersNGroupList.length; i++) {

                    if (self.UsersNGroupList[i].Value == "user") {
                        var count = 0;
                        for (var j = 0; j < self.StateBindings.Users().length; j++) {
                            if (self.StateBindings.Users()[j].Usertext == self.UsersNGroupList[i].Key) {
                                count++
                            }
                        }
                        if (count == 0)
                            self.StateBindings.Users.push({ Usertext: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                    }
                    else if (self.UsersNGroupList[i].Value == "group") {
                        var count = 0;
                        for (var j = 0; j < self.StateBindings.Groups().length; j++) {
                            if (self.StateBindings.Groups()[j].GroupText == self.UsersNGroupList[i].Key) {
                                count++
                            }
                        }
                        if (count == 0)
                            self.StateBindings.Groups.push({ GroupText: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                    }
                }
                $(".RADWorkFlowAddConditionParent").hide();
                self.checkRuleEditorDiv();
                if (self.StateInfo.StateActions[positionOfAction].EscalationMappedUsers[index].ruleText != "" && self.StateInfo.StateActions[positionOfAction].EscalationMappedUsers[index].ruleText != null) {
                    $(".RADWorkFlowRuleEditorDivParent").show();
                    self.initRuleEngine(self.StateInfo.StateActions[positionOfAction].EscalationMappedUsers[index].ruleText);
                }
                else {
                    $(".RADWorkFlowAddConditionParent").show();
                }
                //$(".RADWorkFlowRuleEditorDivParent").ruleEngine({
                //    grammarInfo: radworkflow.RuleGrammerInfo,
                //    ruleText: self.StateInfo.StateActions[i].EscalationMappedUsers[$($(event.target).closest(".RADWorkFlowStateUserMapDiv")).attr("index")].ruleText,
                //    serviceUrl: radworkflow.baseUrl + "/Resources/Services/RADXRuleEditorService.svc"
                //});
                self.resetButtons(event);
            }
            //$(".radWFActionConfig").addClass("radDisplayNoneClass");
            self.calculateGroupDivHeight();

        },

        resetButtons:function(event) {
            var self = this;
            $("#prettyCode").addClass("RADHidden");
            if ($(".clickable").length == 0)
                $(".RADWorkFlowSaveOneActionInfo").addClass("clickable")
            $(".RADWorkFlowStateUserMappedParent").hide();
            $(".RADWorkFlowStateUserComponentParent").show();
            $(".RADWorkFlowStateUserComponentParent").attr("index", $($(event.target).closest(".RADWorkFlowStateUserMapDiv")).attr("index"))
            $(".RADWorkFlowStateAdd").hide();
        },

        //Check if Rule Editor Div Exists . If not append.
        checkRuleEditorDiv: function () {
            var self = this;
            if ($(".RADWorkFlowRuleDiv").length == 0) {
                $(".RADWorkFlowRuleEditorDivParent").append('<div class="RADWorkFlowRuleDiv"></div>')
            }
            else {
                $(".RADWorkFlowRuleDiv").empty();
            }
        },

        //Init Rule Engine with Text.
        initRuleEngine:function(ruleText) {
            var self = this;
            if ($(".RADWorkFlowRuleDiv").data("ruleEngine") != undefined)
                $(".RADWorkFlowRuleDiv").data("ruleEngine").Destroy()
            $(".RADWorkFlowRuleDiv").ruleEngine({
                grammarInfo: radworkflow.rulrGrammarInfo,
                ruleText: ruleText,
                serviceUrl: radworkflow.baseUrl + "/Resources/Services/RADXRuleEditorService.svc"
            });
            $(".RADWorkFlowAddConditionParent").hide();
        },

        //Search User Text
        UsersearchFunc: function (searchvalue) {
            var self = this;
            if (searchvalue == "") {
                $(".RADWorkFlowStateUserParentDiv").removeClass("RADHidden")
            }
            else {
                $(".RADWorkFlowStateUserParentDiv").addClass("RADHidden")
                for (var i = 0; i < $(".RADWorkFlowStateUserParentDiv").length; i++) {
                    if ($($(".RADWorkFlowStateUserParentDiv")[i]).text().toLowerCase().indexOf(searchvalue.toLowerCase()) != -1)
                        $($(".RADWorkFlowStateUserParentDiv")[i]).removeClass("RADHidden")
                }
            }
        },

        //Search Group Text
        GroupsearchFunc: function (searchvalue) {
            var self = this;
            if (searchvalue == "") {
                $(".RADWorkFlowStateGroupParentDiv").closest(".RADWorkFlowStateGroupParentDivParent").removeClass("RADHidden")
            }
            else {
                $(".RADWorkFlowStateGroupParentDiv").closest(".RADWorkFlowStateGroupParentDivParent").addClass("RADHidden")
                for (var i = 0; i < $(".RADWorkFlowStateGroupParentDiv").length; i++) {
                    if ($($(".RADWorkFlowStateGroupParentDiv")[i]).text().toLowerCase().indexOf(searchvalue.toLowerCase()) != -1)
                        $($(".RADWorkFlowStateGroupParentDiv")[i]).closest(".RADWorkFlowStateGroupParentDivParent").removeClass("RADHidden")
                }
            }
        },

        DeleteMappedStateFunction: function (model, event) {
            var self = this;
            var index = $(event.target).closest(".RADWorkFlowStateUserMapDiv").attr('index')
            if ($(".RADEscalatedCss").length > 0) {
                for (var i = 0; i < self.StateInfo.StateActions.length; i++) {
                    if (self.StateInfo.StateActions[i].Action == $(".RADWorkFlowStateActionItemColor").text()) {
                        self.StateInfo.StateActions[i].EscalationMappedUsers.splice(index, 1);
                    }
                    self.StateBindings.mappedInfo.splice(parseInt(index), 1)
                    if (self.StateInfo.StateActions[i].EscalationMappedUsers.length == 0) {
                        $(".RADWorkFlowStateUserMappedParent").hide();
                        $(".RADWorkFlowStateAdd").hide();
                        $(".RADWorkFlowStateUserComponentParent").show();
                        $(".RADWorkFlowAddConditionParent").show();
                        self.StateBindings.Users.splice(0, self.StateBindings.Users().length)
                        self.StateBindings.Groups.splice(0, self.StateBindings.Groups().length)
                        for (var i = 0 ; i < self.UsersNGroupList.length; i++) {
                            if (self.UsersNGroupList[i].Value == "user") {
                                self.StateBindings.Users.push({ Usertext: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                            }
                            if (self.UsersNGroupList[i].Value == "group") {
                                self.StateBindings.Groups.push({ GroupText: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                            }
                        }
                        $(".clickable").removeClass("clickable")
                    }
                }
            }
            else {
                for (var i = 0; i < self.StateInfo.StateActions.length; i++) {
                    if (self.StateInfo.StateActions[i].Action == $(".RADWorkFlowStateActionItemColor").text()) {
                        self.StateInfo.StateActions[i].MappedUsers.splice(index, 1);
                    }
                    self.StateBindings.mappedInfo.splice(parseInt(index), 1)
                    if (self.StateInfo.StateActions[i].MappedUsers.length == 0) {
                        $(".RADWorkFlowStateUserMappedParent").hide();
                        $(".RADWorkFlowStateAdd").hide();
                        $(".RADWorkFlowStateUserComponentParent").show();
                        $(".RADWorkFlowAddConditionParent").show();
                        self.StateBindings.Users.splice(0, self.StateBindings.Users().length)
                        self.StateBindings.Groups.splice(0, self.StateBindings.Groups().length)
                        for (var i = 0 ; i < self.UsersNGroupList.length; i++) {
                            if (self.UsersNGroupList[i].Value == "user") {
                                self.StateBindings.Users.push({ Usertext: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                            }
                            if (self.UsersNGroupList[i].Value == "group") {
                                self.StateBindings.Groups.push({ GroupText: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                            }
                        }
                        $(".clickable").removeClass("clickable")
                    }
                }
            }
        },


        //Show Mapped Groups for an Action
        ShowMappedGroupsTextFunc: function (index) {
            var self = this;
            var totalnumberOfGroups = 0
            var numberofGroups = 0;
            var text = '';
            if ($(".RADEscalatedCss").length == 0) {
                for (var i = 0; i < self.StateInfo.StateActions.length; i++) {
                    if (self.StateInfo.StateActions[i].Action == $(".RADWorkFlowStateActionItemColor").text()) {
                        //var index = $($(event.target).closest(".RADWorkFlowStateUserMapDiv")).attr("index");
                        if (self.StateInfo.StateActions[i].MappedUsers.length > 0) {
                            totalnumberOfGroups = Object.keys(self.StateInfo.StateActions[i].MappedUsers[parseInt(index)].Name).length;
                        }
                        numberofGroups = 0;
                        for (var j = 0; j < totalnumberOfGroups; j++) {
                            if (Object.values(self.StateInfo.StateActions[i].MappedUsers[parseInt(index)].Name)[j] == "false" || Object.values(self.StateInfo.StateActions[i].MappedUsers[parseInt(index)].Name)[j] == false) {
                                numberofGroups++;
                                if (numberofGroups < 3)
                                    text += Object.keys(self.StateInfo.StateActions[i].MappedUsers[parseInt(index)].Name)[j] + ",";
                            }
                        }
                        if (numberofGroups > 2)
                            text = (text.substr(0, text.length - 1) + "   " + (numberofGroups - 2) + ' More');
                        else
                            text = text.substr(0, text.length - 1);
                        break;
                    }
                }
                return ("GROUPS : " + text);
            }
            else {
                for (var i = 0; i < self.StateInfo.StateActions.length; i++) {
                    if (self.StateInfo.StateActions[i].Action == $(".RADWorkFlowStateActionItemColor").text()) {
                        if (self.StateInfo.StateActions[i].MappedUsers.length > 0) {
                            totalnumberOfGroups = Object.keys(self.StateInfo.StateActions[i].MappedUsers[parseInt(index)].Name).length;
                        }
                        numberofGroups = 0;
                        for (var j = 0; j < Object.keys(self.StateInfo.StateActions[i].EscalationMappedUsers[parseInt(index)].Name).length; j++) {
                            if (Object.values(self.StateInfo.StateActions[i].EscalationMappedUsers[parseInt(index)].Name)[j] == "false" || Object.values(self.StateInfo.StateActions[i].EscalationMappedUsers[parseInt(index)].Name)[j] == false) {
                                numberofGroups++;
                                if (numberofGroups < 3)
                                    text += Object.keys(self.StateInfo.StateActions[i].MappedUsers[parseInt(index)].Name)[j] + "|";
                            }
                        }
                        if (numberofGroups > 2)
                            text = (text.substr(0, text.length - 1) + "    " + (numberofGroups - 2) + ' More');
                        else
                            text = text.substr(0, text.length - 1);
                        break;
                    }
                }
                return ("GROUPS : " + text);
            }
        },

        EscalatedModeFunc: function (model, event) {
            var self = this;
            if ($(".RADWFEscalateText").html().trim() != "" && $(".RADWFEscalateText").html().trim() > 0) {
                if (!$(event.target).hasClass("RADEscalatedCss")) {
                    $(event.target).addClass("RADEscalatedCss");
                    $($(event.target).closest(".RADWorkFlowTypeToggle").find(".RADNormalCss")).removeClass("RADNormalCss");
                    var count = 0;
                    var actioncount = 0;
                    self.StateBindings.mappedInfo.splice(0, self.StateBindings.mappedInfo().length)
                    for (var i = 0; i < self.StateInfo.StateActions.length; i++) {
                        if (self.StateInfo.StateActions[i].Action == $(".RADWorkFlowStateActionItemColor").text()) {
                            actioncount = i;
                            for (var j = 0; j < self.StateInfo.StateActions[i].EscalationMappedUsers.length; j++) {
                                count++;
                                if (self.StateInfo.StateActions[i].EscalationMappedUsers[j].ruleText == "")
                                    self.StateBindings.mappedInfo.push({ RuleText: "" });
                                else
                                    self.StateBindings.mappedInfo.push({ RuleText: self.StateInfo.StateActions[i].EscalationMappedUsers[j].ruleText });
                            }

                            break;
                        }
                    }
                    if (count == 0) {
                        self.StateBindings.Users().splice(0, self.StateBindings.Users().length)
                        self.StateBindings.Groups().splice(0, self.StateBindings.Groups().length)
                        for (var i = 0 ; i < self.UsersNGroupList.length; i++) {
                            if (self.UsersNGroupList[i].Value == "user") {
                                self.StateBindings.Users.push({ Usertext: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                            }
                            if (self.UsersNGroupList[i].Value == "group") {
                                self.StateBindings.Groups.push({ GroupText: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                            }
                        }
                        $(".radWFActionConfig").addClass("radDisplayNoneClass");
                        $(".RADWorkFlowStateUserComponentParent").show();
                        $(".RADWorkFlowStateUserMappedParent").hide();
                        $(".RADWorkFlowStateAdd").hide();
                        $(".RADWorkFlowAddConditionParent").show();
                        $(".RADWorkFlowRuleEditorDivParent").hide();
                    }
                    else {
                        $(".radWFActionConfig").removeClass("radDisplayNoneClass");
                        $(".RADWorkFlowStateUserComponentParent").hide();
                        $(".RADWorkFlowStateUserMappedParent").show();
                        self.showHideAddNewStateBtn(actioncount);
                        $(".RADWorkFlowAddConditionParent").hide();
                    }
                }
            }
        },

        NormalModeFunc: function (model, event) {
            var self = this;
            if (!$(event.target).hasClass("RADNormalCss")) {
                $(event.target).addClass("RADNormalCss");
                $($(event.target).closest(".RADWorkFlowTypeToggle").find(".RADEscalatedCss")).removeClass("RADEscalatedCss");
                var count = 0;
                var actioncount = 0;
                self.StateBindings.mappedInfo.splice(0, self.StateBindings.mappedInfo().length)
                for (var i = 0; i < self.StateInfo.StateActions.length; i++) {
                    if (self.StateInfo.StateActions[i].Action == $(".RADWorkFlowStateActionItemColor").text()) {
                        actioncount = i;
                        for (var j = 0; j < self.StateInfo.StateActions[i].MappedUsers.length; j++) {
                            if (self.StateInfo.StateActions[i].MappedUsers[j].ruleText == "")
                                self.StateBindings.mappedInfo.push({ RuleText: "" });
                            else
                                self.StateBindings.mappedInfo.push({ RuleText: self.StateInfo.StateActions[i].MappedUsers[j].ruleText });
                            count++;
                        }

                        break;
                    }
                }
                if (count == 0) {
                    self.StateBindings.Users().splice(0, self.StateBindings.Users().length)
                    self.StateBindings.Groups().splice(0, self.StateBindings.Groups().length)
                    for (var i = 0 ; i < self.UsersNGroupList.length; i++) {
                        if (self.UsersNGroupList[i].Value == "user") {
                            self.StateBindings.Users.push({ Usertext: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                        }
                        if (self.UsersNGroupList[i].Value == "group") {
                            self.StateBindings.Groups.push({ GroupText: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                        }
                    }
                    $(".radWFActionConfig").addClass("radDisplayNoneClass");
                    $(".RADWorkFlowStateUserComponentParent").show();
                    $(".RADWorkFlowStateUserMappedParent").hide();
                    $(".RADWorkFlowStateAdd").hide();
                    $(".RADWorkFlowAddConditionParent").show();
                    $(".RADWorkFlowRuleEditorDivParent").hide();
                }
                else {
                    $(".radWFActionConfig").removeClass("radDisplayNoneClass");
                    $(".RADWorkFlowStateUserComponentParent").hide();
                    $(".RADWorkFlowStateUserMappedParent").show();
                    self.showHideAddNewStateBtn(actioncount);
                    $(".RADWorkFlowAddConditionParent").hide();
                }
            }
        },

        MailONFunc: function (model, event) {
            var self = this;
            if (!$(event.target).hasClass("RADOnCSS")) {
                if ($(".RADWorkFlowStateEditMail")[0].style.display == "none") {
                    $(event.target).addClass("RADOnCSS");
                    $($(event.target).closest(".RADWorkFlowStateMailToggle").find(".RADOffCss")).removeClass("RADOffCss");
                    // $("#RADWorkFlowStateComponent")[0].style.opacity = .3;
                    radworkflow.mailsubscription.pageLoad(self.StateInfo);
                }
            }
        },

        calculateGroupDivHeight:function() {
            $(".RADWorkFlowStateGroupParent").height($(".RADWorkFlowStateUserComponentParent").height() - 70 - Math.abs($(".RADWorkFlowRuleEditorDivParent").outerHeight()));
        },

        MailOFFFunc: function (model, event) {
            var self = this;
            if (!$(event.target).hasClass("RADOffCss")) {
                $(event.target).addClass("RADOffCss");
                $($(event.target).closest(".RADWorkFlowStateMailToggle").find(".RADOnCSS")).removeClass("RADOnCSS");
                $(".RADWorkFlowStateEditMail").hide();
                //if (self.StateInfo.escalationInfo.EscalationMail != undefined  || self.StateInfo.mailInfo != undefined) {
                if (self.StateInfo.escalationInfo != null)
                    self.StateInfo.escalationInfo.EscalationMail = {};
                else
                    self.StateInfo.escalationInfo = {};
                self.StateInfo.mailInfo = {}
                //}
            }
        },

        CallBackFunc: function () {
            var self = this;
            if (self.StateInfo.escalationInfo != undefined || self.StateInfo.mailInfo != undefined) {
                if (self.StateInfo.escalationInfo != undefined) {
                    if (self.StateInfo.escalationInfo.EscalationMail != undefined || self.StateInfo.mailInfo != undefined) {
                        if ($(".RADWorkFlowStateEditMail")[0].style.display == "none")
                            $(".RADWorkFlowStateEditMail").show()
                        $(".RADWorkFlowStateEditMail").removeClass("RADHidden");
                        $(".RADOffCss").removeClass("RADOffCss");
                        $($(".RADWorkFlowStateMailToggleElement")[0]).addClass("RADOnCSS");
                    }
                    else {
                        if ($(".RADWorkFlowStateEditMail")[0].style.display != "none") {
                            $(".RADWorkFlowStateEditMail").hide()
                            $(".RADWorkFlowStateEditMail").removeClass("RADHidden")
                            $(".RADOffCss").removeClass("RADOffCss")
                            $($(".RADWorkFlowStateMailToggleElement")[0]).addClass("RADOnCSS")
                        }
                    }
                }
            }
            $("#RADWorkFlowStateComponent")[0].style.opacity = 1;
        },

        AddActionFunc: function (model, event) {
            var self = this;
            $(".RADWorkFlowActionFooterSave").html("Done");
            $("#RADWorkFlowActionStateConfigurationComponent").hide();
            $("#RADWorkFlowActionComponent").show();
        },

        //Save Action Selection for the State.
        GoToNextPageFunc: function (model, event) {
            var self = this;
            if (model.SelectedActions().length > 0) {
                self.currentAction = model.SelectedActions()[0];
                $("#RADWorkFlowActionComponent").hide();
                $("#RADWorkFlowActionStateConfigurationComponent").show();
                $(".RADWorkFlowStateAdd").hide()
                self.calculateGroupDivHeight();
            }
        },

        CancelThisViewFunc: function (model, event) {
            var self = this;
            if (self.StateBindings.SelectedActions().length > 0 || self.StateInfo.StateActions.length > 0) {
                if (self.StateInfo.StateActions.length > 0) {
                    $("#RADWorkFlowActionComponent").hide();
                    $("#RADWorkFlowActionStateConfigurationComponent").show();
                }
                else {
                    $(".RADWorkFlowMainParent").remove();
                    if (self.StateInfo.stateId == 0) {
                        self.StateInfo.StateActions.splice(0, self.StateInfo.StateActions.length);
                    }
                    $(".radAddWorkFlowMain")[0].style.opacity = 1;
                }
                //$("#RADWorkFlowActionComponent").show();
                //$("#RADWorkFlowActionStateConfigurationComponent").hide();
            }
            else {
                $(".RADWorkFlowMainParent").remove();
                if (self.StateInfo.stateId == 0) {
                    self.StateInfo.StateActions.splice(0, self.StateInfo.StateActions.length);
                }
                $(".radAddWorkFlowMain")[0].style.opacity = 1;
            }
        },

        BackToEarlierPageFunc: function (model, event) {
            var self = this;
            $(".RADWorkFlowMainParent").remove();
            if (self.StateInfo.stateId == 0) {
                self.StateInfo.StateActions.splice(0, self.StateInfo.StateActions.length);
            }
            $(".radAddWorkFlowMain")[0].style.opacity = 1;
        },

        EscalationOnFunc: function (model, event) {
            var self = this;
        },

        EscalationOffFunc: function (model, event) {
            var self = this;
        },

        //Save State
        SaveStateFunc: function (model, event) {
            var self = this;
            var count = 0;
            var msg = "";
            //for (var i = 0; i < $(".RADWorkFlowStateActionList").length; i++) {
            //    for (j = 0; j < self.StateInfo.StateActions.length; j++) {
            //        if ($($(".RADWorkFlowStateActionList")[i]).text() == self.StateInfo.StateActions[j].Action) {
            //            count++;
            //            break;
            //        }
            //    }

            //}
            if ($(".RADWorkFlowStateActionListParent").length != self.StateInfo.StateActions.length) {
                count++;
                msg = "Please select all the actions"
            }
            else {
                for (j = 0; j < self.StateInfo.StateActions.length; j++) {
                    if (self.StateInfo.StateActions[j].MappedUsers.length == 0) {
                        count++;
                        msg = "Please select normal user and group mapping for all the actions"
                    }
                }
            }
            if (count == 0) {
                var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc"
                if ($(".RADWorkFlowExchangeName").text() != '' && $(".RADWorkFlowRoutingKeyName").text() != '') {
                    var a = {}
                    a.exchangeName = $(".RADWorkFlowExchangeName").text()
                    a.routingKey = $(".RADWorkFlowRoutingKeyName").text()
                    self.StateInfo.pubsubInfo = a;//{ exchangeName: $(".RADWorkFlowExchangeName").text(), routingKey: $(".RADWorkFlowRoutingKeyName").text() }
                }
                if ($(".RADWFEscalateText").html().trim() != "") {
                    if (self.StateInfo.escalationInfo == null) {
                        self.StateInfo.escalationInfo = {}
                    }
                    self.StateInfo.escalationInfo.Duration = parseInt($(".RADWFEscalateText").html().trim());
                }
                else
                    self.StateInfo.escalationInfo = {}
                for (j = 0; j < self.StateInfo.StateActions.length; j++) {
                    if (self.StateInfo.escalationInfo.Duration == null || self.StateInfo.escalationInfo.Duration == 0) {
                        self.StateInfo.StateActions[j].EscalationMappedUsers = [];
                    }
                }

                $.ajax({
                    url: url + '/SaveWorkflowState',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ stateInfo: JSON.stringify(self.StateInfo), workflowId: self.workflowId, user: radworkflow.user })
                }).then(function (responseText) {
                    if (responseText.d > 0) {
                        //self.StateInfo.stateId = responseText.d;
                        for (var i = 0; i < radworkflow.addWorkFlow.currentWorkFlow.WorkflowStates.length; i++) {
                            if (radworkflow.addWorkFlow.currentWorkFlow.WorkflowStates[i].StateName == self.StateInfo.StateName) {
                                radworkflow.addWorkFlow.currentWorkFlow.WorkflowStates[i] = self.StateInfo;
                            }
                        }
                        $(".RADWorkFlowMainParent").remove();
                        radworkflow.addWorkFlow.createConnections(self.StateInfo);
                    }
                })
            }
            else {
                self.ShowErrorMessage(msg);
                return;
            }
        },

        //Show Error Message.
        ShowErrorMessage: function (message) {
            var self = this;
            $(".radWFStateErrMsg").parent().removeClass("radDisplayNoneClass");
            $(".radWFStateErrMsg").html(message);
        },

        //To Check condition to show and hide Add NEw State Button for Normal Mode.
        showHideAddNewStateBtn:function(stateIndex) {
            var self = this;
            if(self.StateInfo.StateActions[stateIndex].MappedUsers != null && self.StateInfo.StateActions[stateIndex].MappedUsers.length > 1)
                $(".RADWorkFlowStateAdd").show();
            else if(self.StateInfo.StateActions[stateIndex].MappedUsers.length == 1 && !self.checkIsNullorEmpty(self.StateInfo.StateActions[stateIndex].MappedUsers[0].ruleText))
                $(".RADWorkFlowStateAdd").show();
            else
                $(".RADWorkFlowStateAdd").hide();

        },

        checkIsNullorEmpty: function (value) {
            if (value == null || value == "")
                return true
            else
                false;
        },

        ShowUserAndGroupFunc: function (model, event) {
            var self = this;
            if ($(event.target).hasClass("RADWorkFlowStateActionListCaret ") || $(event.target).hasClass("RADWorkFlowStateActionItemColor") || $(event.target).hasClass("RADWorkFlowStateActionListParent")) {
                //do nothing
            }
            else {
                $(".RADWorkFlowNormalType").addClass("RADNormalCss");
                $(".RADWorkFlowEscaledType").removeClass("RADEscalatedCss");
                $(".RADWorkFlowStateActionItemColor").removeClass("RADWorkFlowStateActionItemColor");
                $(".RADWorkFlowStateActionListselect").removeClass("RADWorkFlowStateActionListselect");
                $(".RADWorkFlowStateActionListCaret").addClass("RADHidden");
                $(event.target).addClass("RADWorkFlowStateActionItemColor");
                $(event.target).addClass("RADWorkFlowStateActionListselect");
                $(event.target).closest(".RADWorkFlowStateActionListParent").find(".RADWorkFlowStateActionListCaret").removeClass("RADHidden");
                $(".select").removeClass("select");
                var actionName = $(event.target).text();
                self.currentAction = actionName;
                self.bindUseGroupMapping(actionName);
            }
            self.calculateGroupDivHeight();
        },

        //Group and Action Mapping
        bindUseGroupMapping: function (actionName) {
            var self = this;
            var count = 0;
            if (self.StateInfo.StateActions != null || self.StateInfo.StateActions.length > 0) {
                for (var i = 0; i < self.StateInfo.StateActions.length; i++) {
                    if (self.StateInfo.StateActions[i].Action == actionName) {
                        count = i + 1;
                        break;
                    }
                }
            }
            if (count > 0) {
                $(".RADWorkFlowRuleEditorDivParent").hide();
                $(".RADWorkFlowSaveOneActionInfo").addClass("clickable");
                $(".RADWorkFlowRuleEditorDivParent").empty();
                $(".RADWorkFlowStateUserMappedParent").show();
                self.showHideAddNewStateBtn(count - 1);
                $(".radWFActionConfig").find("input").val(self.StateInfo.StateActions[count - 1].Config);
                //Commented 
                /*if ($(".RADWorkFlowStateUserMapppedRule").text() == "")
                    $(".RADWorkFlowStateAdd").hide();
                else
                    $(".RADWorkFlowStateAdd").show();*/
                $(".RADWorkFlowStateUserComponentParent").hide();
                self.StateBindings.mappedInfo().splice(0, self.StateBindings.mappedInfo().length)
                if ($(".RADEscalatedCss").length == 0) {
                    if (self.StateInfo.StateActions[count - 1].MappedUsers.length > 0) {
                        $(".radWFActionConfig").removeClass("radDisplayNoneClass");
                        $(".RADWorkFlowAddConditionParent").hide();
                        for (var j = 0; j < self.StateInfo.StateActions[count - 1].MappedUsers.length; j++) {
                            if (self.StateInfo.StateActions[count - 1].MappedUsers[j].ruleText == "" && self.StateInfo.StateActions[count - 1].MappedUsers[j].ruleText == undefined)
                                self.StateBindings.mappedInfo.push({ RuleText: "" })
                            else
                                self.StateBindings.mappedInfo.push({ RuleText: self.StateInfo.StateActions[count - 1].MappedUsers[j].ruleText })
                        }
                    }
                    else {
                        $(".radWFActionConfig").addClass("radDisplayNoneClass");
                        $(".RADWorkFlowStateUserMappedParent").hide();
                        $(".RADWorkFlowStateAdd").hide();
                        $(".RADWorkFlowStateUserComponentParent").show();
                        $(".RADWorkFlowAddConditionParent").show();
                        self.StateBindings.Users.splice(0, self.StateBindings.Users().length)
                        self.StateBindings.Groups.splice(0, self.StateBindings.Groups().length)
                        for (var i = 0 ; i < self.UsersNGroupList.length; i++) {
                            if (self.UsersNGroupList[i].Value == "user") {
                                self.StateBindings.Users.push({ Usertext: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                            }
                            if (self.UsersNGroupList[i].Value == "group") {
                                self.StateBindings.Groups.push({ GroupText: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                            }
                        }
                    }
                }
                else {
                    if (self.StateInfo.StateActions[count - 1].EscalationMappedUsers.length > 0) {
                        $(".RADWorkFlowAddConditionParent").hide();
                        $(".radWFActionConfig").removeClass("radDisplayNoneClass");
                        for (var j = 0; j < self.StateInfo.StateActions[count - 1].EscalationMappedUsers.length; j++) {
                            if (self.StateInfo.StateActions[count - 1].EscalationMappedUsers[j].ruleText == "" && self.StateInfo.StateActions[count - 1].EscalationMappedUsers[j].ruleText == undefined)
                                self.StateBindings.mappedInfo.push({ RuleText: "" })
                            else
                                self.StateBindings.mappedInfo.push({ RuleText: self.StateInfo.StateActions[count - 1].EscalationMappedUsers[j].ruleText })
                        }
                    }
                    else {
                        $(".radWFActionConfig").addClass("radDisplayNoneClass");
                        $(".RADWorkFlowStateUserMappedParent").hide();
                        $(".RADWorkFlowStateAdd").hide();
                        $(".RADWorkFlowStateUserComponentParent").show();
                        $(".RADWorkFlowAddConditionParent").show();
                        self.StateBindings.Users.splice(0, self.StateBindings.Users().length)
                        self.StateBindings.Groups.splice(0, self.StateBindings.Groups().length)
                        for (var i = 0 ; i < self.UsersNGroupList.length; i++) {
                            if (self.UsersNGroupList[i].Value == "user") {
                                self.StateBindings.Users.push({ Usertext: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                            }
                            if (self.UsersNGroupList[i].Value == "group") {
                                self.StateBindings.Groups.push({ GroupText: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                            }
                        }
                    }
                }
            }
            else {
                self.StateBindings.mappedInfo().splice(0, self.StateBindings.mappedInfo().length);
                $(".radWFActionConfig").find("input").val("");
                $(".radWFActionConfig").addClass("radDisplayNoneClass");
                $(".RADWorkFlowSaveOneActionInfo").removeClass("clickable");
                $(".RADWorkFlowRuleEditorDivParent").empty();
                $(".RADWorkFlowRuleEditorDivParent").hide();
                $(".RADWorkFlowStateUserMappedParent").hide();
                $(".RADWorkFlowStateAdd").hide();
                $(".RADWorkFlowStateUserComponentParent").show();
                $(".RADWorkFlowAddConditionParent").show();
                self.CleanUsersNGroups(null, event);
            }
        },


        CleanUsersNGroups: function (model, event) {
            var self = this;
            self.StateBindings.Users().splice(0, self.StateBindings.Users().length);
            self.StateBindings.Groups().splice(0, self.StateBindings.Groups().length);
            for (var i = 0 ; i < self.UsersNGroupList.length; i++) {
                if (self.UsersNGroupList[i].Value == "user") {
                    self.StateBindings.Users.push({ Usertext: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                }
                if (self.UsersNGroupList[i].Value == "group") {
                    self.StateBindings.Groups.push({ GroupText: self.UsersNGroupList[i].Key, selected: ko.observable(false) })
                }
            }
        },

        //Initialize Rule Editor.
        AddRuleEditorFunc: function (model, event) {
            var self = this;
            $(".RADWorkFlowRuleEditorDivParent").show();
            if ($(".RADWorkFlowRuleDiv").length == 0) {
                $(".RADWorkFlowRuleEditorDivParent").append('<div class="RADWorkFlowRuleDiv"></div>')
            }
            else {
                $(".RADWorkFlowRuleDiv").empty();
            }
            $(".RADWorkFlowRuleEditorDivParent").removeClass("radDisplayNoneClass");
            if ($(".RADWorkFlowRuleDiv").data("ruleEngine") != undefined)
                $(".RADWorkFlowRuleDiv").data("ruleEngine").Destroy()
            $(".RADWorkFlowRuleDiv").ruleEngine({
                grammarInfo: radworkflow.rulrGrammarInfo,
                ruleText: "",
                serviceUrl: radworkflow.baseUrl + "/Resources/Services/RADXRuleEditorService.svc"
            });
            $("#prettyCode").addClass("RADHidden");
            $(".RADWorkFlowAddConditionParent").hide();
            self.calculateGroupDivHeight();
        },

        //Group Name Select Handler.
        SelectGroupForActionFunc: function (model, event) {
            var self = this;
            if (model.selected() == false) {
                model.selected(true);
                //$(event.target).next().addClass("RADWorkflowCircleSelected").addClass("fa fa-check");
            }
            else {
                model.selected(false);
                $(event.target).next().removeClass("RADWorkflowCircleSelected").removeClass("fa fa-check");
            }
            $(event.target).closest(".RADWorkFlowStateGroupParentDivParent").attr('newAdded', true);
            if ($(".select").length > 0) {
                $(".RADWorkFlowSaveOneActionInfo").addClass("clickable")
            }
            else {
                $(".RADWorkFlowSaveOneActionInfo").removeClass("clickable")
            }
        },


        //Cancel Selections of Groups on Action Mapping Screen
        cancelSelection: function (model, event) {
            var self = this;
            self.bindUseGroupMapping(self.currentAction);
            self.calculateGroupDivHeight();
        },

        //Save Rule For Escalated User Mapping.
        saveEscalatedRuleSet: function (stateActionIndex, mappedUserIndex, ruleInfo, classDocument, updateMappedInfo) {
            var self = this;
            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc";
            $.ajax({
                url: url + '/SaveRuleSet',
                type: 'POST',
                contentType: "application/json",
                data: JSON.stringify({ info: JSON.stringify({ RuleSetId: ruleInfo.RuleSetId, RuleId: ruleInfo.RuleId, Priority: ruleInfo.Priority, Rulename: ruleInfo.Rulename, RuleText: ruleInfo.RuleText, RuleClass: classDocument[0], ClassDocument: classDocument[1], User: radworkflow.user }) }),
                dataType: 'json'
            }).then(function (responseText) {
                self.StateInfo.StateActions[stateActionIndex].EscalationMappedUsers[mappedUserIndex].ruleSetId = responseText.d;
                self.StateInfo.StateActions[stateActionIndex].EscalationMappedUsers[mappedUserIndex].ruleText = $("#ruleTxt").val();
                $(".RADWorkFlowRuleDiv").data("ruleEngine").Destroy()

                if (updateMappedInfo) {
                    self.StateBindings.mappedInfo.splice(0, self.StateBindings.mappedInfo().length)
                    for (var i = 0 ; i < self.StateInfo.StateActions[stateActionIndex].EscalationMappedUsers.length; i++) {
                        self.StateBindings.mappedInfo.push({ RuleText: self.StateInfo.StateActions[stateActionIndex].EscalationMappedUsers[i].ruleText })
                    }
                }
                self.showHideAddNewStateBtn(stateActionIndex);
                $(".RADWorkFlowAddConditionParent").hide();
                self.indexOfEdit = null;
            });
        },

        //Save Rule For User Mapping.
        saveRuleSet: function (stateActionIndex, mappedUserIndex, ruleInfo, classDocument,updateMappedInfo) {
            var self = this;
            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc";
            $.ajax({
                url: url + '/SaveRuleSet',
                type: 'POST',
                contentType: "application/json",
                data: JSON.stringify({ info: JSON.stringify({ RuleSetId: ruleInfo.RuleSetId, RuleId: ruleInfo.RuleId, Priority: ruleInfo.Priority, Rulename: ruleInfo.Rulename, RuleText: ruleInfo.RuleText, RuleClass: classDocument[0], ClassDocument: classDocument[1], User: radworkflow.user }) }),
                dataType: 'json'
            }).then(function (responseText) {
                self.StateInfo.StateActions[stateActionIndex].MappedUsers[mappedUserIndex].ruleSetId = responseText.d;
                self.StateInfo.StateActions[stateActionIndex].MappedUsers[mappedUserIndex].ruleText = $("#ruleTxt").val();
                $(".RADWorkFlowRuleDiv").data("ruleEngine").Destroy()
               
                if (updateMappedInfo) {
                    self.StateBindings.mappedInfo.splice(0, self.StateBindings.mappedInfo().length)
                    for (var i = 0 ; i < self.StateInfo.StateActions[stateActionIndex].MappedUsers.length; i++) {
                        self.StateBindings.mappedInfo.push({ RuleText: self.StateInfo.StateActions[stateActionIndex].MappedUsers[i].ruleText })
                    }
                }
                self.showHideAddNewStateBtn(stateActionIndex);
                $(".RADWorkFlowAddConditionParent").hide();
                self.indexOfEdit = null;
            });
        },

        //Reset the Add New Mapping and Add Rule Buttons to their original State.
        resetAddNewMappingNConditionButtons: function (index) {
            var self = this;
            self.showHideAddNewStateBtn(index);
            $(".RADWorkFlowAddConditionParent").hide();
        },

        //getRuleInfo
        gerRuleInfo: function (ruleInfo) {
            ruleInfo.RuleSetId = 0;
            ruleInfo.RuleId = 0;
            ruleInfo.Priority = 1;
            ruleInfo.Rulename = "Test";
            ruleInfo.RuleText = $("#ruleTxt").val();
            return ruleInfo;
        },



        //Save User and Rule Info Mapping.
        saveUserandRuleInfoMapping: function (model, event) {
            var self = this;
            if (self.StateInfo.StateActions.length == 0) {
                self.StateInfo.StateActions.push({ Action: $(".RADWorkFlowStateActionItemColor").text(), MappedUsers: [], EscalationMappedUsers: [] });
                var a = {};
                for (var j = 0; j < $(".select").length; j++) {
                    if ($($(".select")[j]).closest(".RADWorkFlowStateUserParent").length > 0) {
                        var text = $($(".select")[j]).text();
                        a[text] = "true";
                    }
                    else if ($($(".select")[j]).closest(".RADWorkFlowStateGroupParent").length > 0) {
                        var text = $($(".select")[j]).text();
                        a[text] = "false";
                    }
                }
                self.StateInfo.StateActions[0].MappedUsers.push({ Name: a });
                self.StateInfo.StateActions[0].Config = $(".radWFActionConfig").find('input').val().trim();


                if ($("#ruleTxt").val() != '' && $("#ruleTxt").val() != undefined) {
                    var ruleInfo = {};
                    var editor = $(".RADWorkFlowRuleDiv").data('ruleEngine');
                    var classDocument = editor.getGeneratedCode();
                    ruleInfo = self.gerRuleInfo(ruleInfo);
                    self.saveRuleSet(0, 0, ruleInfo, classDocument, false)
                }
                else {
                    self.resetAddNewMappingNConditionButtons(0);
                }
                self.StateBindings.mappedInfo.push({ RuleText: $("#ruleTxt").val() })
                $(".RADWorkFlowStateUserComponentParent").hide();
                $(".RADWorkFlowStateUserMappedParent").show();
                $(".radWFActionConfig").removeClass("radDisplayNoneClass");
            }
            else {
                count = 0;
                for (var i = 1; i <= self.StateInfo.StateActions.length; i++) {
                    if (self.StateInfo.StateActions[i - 1].Action == $(".RADWorkFlowStateActionItemColor").text()) {
                        count = i;
                        break;
                    }
                }
                
                if (count > 0) {
                    var a = {};
                    for (var j = 0; j < $(".select").length; j++) {
                        if ($($(".select")[j]).closest(".RADWorkFlowStateUserParent").length > 0) {
                            var text = $($(".select")[j]).text();
                            a[text] = true;
                        }
                        else if ($($(".select")[j]).closest(".RADWorkFlowStateGroupParent").length > 0) {
                            var text = $($(".select")[j]).text();
                            a[text] = false;
                        }
                    }
                    self.StateInfo.StateActions[count - 1].Config = $(".radWFActionConfig").find('input').val().trim();
                    if (self.StateInfo.StateActions[count - 1].MappedUsers.length == 0) {
                        self.StateInfo.StateActions[count - 1].MappedUsers.push({ Name: a })
                        if ($("#ruleTxt").val() != '' && $("#ruleTxt").val() != undefined) {
                            var editor = $(".RADWorkFlowRuleDiv").data('ruleEngine');
                            var classDocument = editor.getGeneratedCode();
                            var ruleInfo = {};
                            ruleInfo = self.gerRuleInfo(ruleInfo);
                            self.saveRuleSet(count - 1, 0, ruleInfo, classDocument, false);
                        }
                        else {
                            self.resetAddNewMappingNConditionButtons(0);
                        }
                        self.StateBindings.mappedInfo.push({ RuleText: $("#ruleTxt").val() })
                        $(".radWFActionConfig").removeClass("radDisplayNoneClass");
                    }
                    else if (self.StateInfo.StateActions[count - 1].MappedUsers.length > 0) {
                        if (self.indexOfEdit == null && self.indexOfEdit == undefined) {

                            if ($("#ruleTxt").val() != '' && $("#ruleTxt").val() != undefined) {
                                self.StateInfo.StateActions[count - 1].MappedUsers.push({ Name: a })
                                var editor = $(".RADWorkFlowRuleDiv").data('ruleEngine');
                                var classDocument = editor.getGeneratedCode();
                                var ruleInfo = {};
                                ruleInfo = self.gerRuleInfo(ruleInfo);
                                self.saveRuleSet(count - 1, self.StateInfo.StateActions[count - 1].MappedUsers.length - 1, ruleInfo, classDocument, true);
                                $(".radWFActionConfig").removeClass("radDisplayNoneClass");
                            }
                            else {
                                var c = 0;
                                for (var i = 0; i < self.StateInfo.StateActions[count - 1].MappedUsers.length; i++) {
                                    //if (self.StateInfo.StateActions[count - 1].MappedUsers[i].ruleText != undefined) {
                                    if (!radworkflow.utility.isNotNullorEmpty(self.StateInfo.StateActions[count - 1].MappedUsers[i].ruleText)) {
                                        c++;
                                        break;
                                    }
                                    //}
                                }
                                if (c > 0) {
                                    var msg = "Default Mapping already exists. Please add rule for this mapping";
                                    self.ShowErrorMessage(msg);
                                    return;
                                }
                                else {
                                    self.StateInfo.StateActions[count - 1].MappedUsers.push({ Name: a });
                                    self.StateBindings.mappedInfo.push({ RuleText: $("#ruleTxt").val() });
                                    $(".radWFActionConfig").removeClass("radDisplayNoneClass");
                                }
                            }

                        }
                        else if (self.StateInfo.StateActions[count - 1].MappedUsers[self.indexOfEdit].hasOwnProperty("Name")) {

                            if ($("#ruleTxt").val() != '' && $("#ruleTxt").val() != undefined) {
                                self.StateInfo.StateActions[count - 1].MappedUsers[self.indexOfEdit].Name = a
                                var editor = $(".RADWorkFlowRuleDiv").data('ruleEngine');
                                var classDocument = editor.getGeneratedCode();


                                var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc"
                                if (self.StateInfo.StateActions[count - 1].MappedUsers[self.indexOfEdit].ruleSetId > 0) {
                                    $.ajax({
                                        url: url + '/GetRuleSetDetails',
                                        type: 'POST',
                                        contentType: "application/json",
                                        data: JSON.stringify({ rulesetId: self.StateInfo.StateActions[count - 1].MappedUsers[self.indexOfEdit].ruleSetId }),
                                        dataType: 'json'
                                    }).then(function (responseText) {
                                        var ruleInfo = {};
                                        ruleInfo = self.gerRuleInfo(ruleInfo);
                                        ruleInfo.RuleId = $.parseJSON(responseText.d).Rules[0].RuleID;
                                        ruleInfo.RuleSetId = self.StateInfo.StateActions[count - 1].MappedUsers[self.indexOfEdit].ruleSetId;
                                        self.saveRuleSet(count - 1, self.indexOfEdit, ruleInfo, classDocument, true);
                                    })
                                }
                                else {
                                    var ruleInfo = {};
                                    ruleInfo = self.gerRuleInfo(ruleInfo);
                                    self.saveRuleSet(count - 1, self.indexOfEdit, ruleInfo, classDocument, true);
                                    //self.StateBindings.mappedInfo.splice(0, self.StateBindings.mappedInfo().length);
                                    //for (var i = 0 ; i < self.StateInfo.StateActions[count - 1].MappedUsers.length; i++) {
                                    //    self.StateBindings.mappedInfo.push({ RuleText: self.StateInfo.StateActions[count - 1].MappedUsers[i].ruleText })
                                    //}
                                }
                                $(".radWFActionConfig").removeClass("radDisplayNoneClass");
                            }
                            else {
                                var c = 0;
                                for (var i = 0; i < self.StateInfo.StateActions[count - 1].MappedUsers.length; i++) {
                                    //if (self.StateInfo.StateActions[count - 1].MappedUsers[i].ruleText != undefined) {
                                    if (!radworkflow.utility.isNotNullorEmpty(self.StateInfo.StateActions[count - 1].MappedUsers[i].ruleText) && i != self.indexOfEdit) {
                                        c++;
                                        break;
                                    }
                                    //}
                                }
                                if (c > 0) {
                                    var msg = "Default Mapping already exists. Please add rule for this mapping";
                                    self.ShowErrorMessage(msg);
                                    return;
                                }
                                else {
                                    self.StateInfo.StateActions[count - 1].MappedUsers[self.indexOfEdit].Name = a;
                                    //self.StateBindings.mappedInfo.push({ RuleText: $("#ruleTxt").val() });
                                    //self.StateBindings.mappedInfo()[self.indexOfEdit].RuleText = $("#ruleTxt").val();
                                    self.StateBindings.mappedInfo.splice(0, self.StateBindings.mappedInfo().length);
                                    for (var i = 0 ; i < self.StateInfo.StateActions[count - 1].MappedUsers.length; i++) {
                                        if (i != self.indexOfEdit)
                                            self.StateBindings.mappedInfo.push({ RuleText: self.StateInfo.StateActions[count - 1].MappedUsers[i].ruleText });
                                        else
                                            self.StateBindings.mappedInfo.push({ RuleText: $("#ruleTxt").val() });
                                    }
                                    $(".radWFActionConfig").removeClass("radDisplayNoneClass");
                                }
                            }

                        }
                    }


                }
                else {
                    self.StateInfo.StateActions.push({ Action: $(".RADWorkFlowStateActionItemColor").text(), MappedUsers: [], EscalationMappedUsers: [],Config : $(".radWFActionConfig").find('input').val().trim() });
                    var a = {};
                    for (var j = 0; j < $(".select").length; j++) {
                        if ($($(".select")[j]).closest(".RADWorkFlowStateUserParent").length > 0) {
                            var text = $($(".select")[j]).text();
                            a[text] = true;
                        }
                        else if ($($(".select")[j]).closest(".RADWorkFlowStateGroupParent").length > 0) {
                            var text = $($(".select")[j]).text();
                            a[text] = false;
                        }
                    }
                    self.StateInfo.StateActions[self.StateInfo.StateActions.length - 1].MappedUsers.push({ Name: a })
                    if ($("#ruleTxt").val() != undefined && $("#ruleTxt").val() != "") {
                        var editor = $(".RADWorkFlowRuleDiv").data('ruleEngine');
                        var classDocument = editor.getGeneratedCode();
                        var ruleInfo = {};
                        ruleInfo = self.gerRuleInfo(ruleInfo);
                        self.saveRuleSet(self.StateInfo.StateActions.length - 1, 0, ruleInfo, classDocument, false);
                    }
                    else {
                        self.resetAddNewMappingNConditionButtons(0);
                    }
                    self.StateBindings.mappedInfo.push({ RuleText: $("#ruleTxt").val() });
                    $(".radWFActionConfig").removeClass("radDisplayNoneClass");
                }


                //self.StateBindings.mappedInfo.splice(0, self.StateBindings.mappedInfo().length)
                //self.StateBindings.mappedInfo.push({ RuleText: $("#ruleTxt").val() })
                $(".RADWorkFlowStateUserComponentParent").hide();
                $(".RADWorkFlowStateUserMappedParent").show();
            }
        },

        // Save Escalated User and RUle Info Mapping.
        saveEscalatedUserandRuleInfoMapping: function (model, event) {
            var self = this;
            if (self.StateInfo.StateActions.length == 0) {
                self.StateInfo.StateActions.push({ Action: $(".RADWorkFlowStateActionItemColor").text(), MappedUsers: [], EscalationMappedUsers: [] });
                var a = {};
                for (var j = 0; j < $(".select").length; j++) {
                    if ($($(".select")[j]).closest(".RADWorkFlowStateUserParent").length > 0) {
                        var text = $($(".select")[j]).text();
                        a[text] = "true";
                    }
                    else if ($($(".select")[j]).closest(".RADWorkFlowStateGroupParent").length > 0) {
                        var text = $($(".select")[j]).text();
                        a[text] = "false";
                    }
                }
                self.StateInfo.StateActions[0].EscalationMappedUsers.push({ Name: a });


                if ($("#ruleTxt").val() != '' && $("#ruleTxt").val() != undefined) {
                    var editor = $(".RADWorkFlowRuleDiv").data('ruleEngine');
                    var classDocument = editor.getGeneratedCode();
                    var ruleInfo = {};
                    ruleInfo = self.gerRuleInfo(rule);
                    self.saveEscalatedRuleSet(0, 0, ruleInfo, classDocument, false);
                }
                else {
                    self.resetAddNewMappingNConditionButtons(0);
                }

                self.StateBindings.mappedInfo.push({ RuleText: $("#ruleTxt").val() })
                $(".RADWorkFlowStateUserComponentParent").hide();
                $(".RADWorkFlowStateUserMappedParent").show();
                //$(".RADWorkFlowStateAdd").show();
            }
            else {
                count = 0;
                for (var i = 1; i <= self.StateInfo.StateActions.length; i++) {
                    if (self.StateInfo.StateActions[i - 1].Action == $(".RADWorkFlowStateActionItemColor").text()) {
                        count = i;
                        break;
                    }
                }
                if (count > 0) {

                    var a = {};
                    for (var j = 0; j < $(".select").length; j++) {
                        if ($($(".select")[j]).closest(".RADWorkFlowStateUserParent").length > 0) {
                            var text = $($(".select")[j]).text();
                            a[text] = true;
                        }
                        else if ($($(".select")[j]).closest(".RADWorkFlowStateGroupParent").length > 0) {
                            var text = $($(".select")[j]).text();
                            a[text] = false;
                        }
                    }
                    if (self.StateInfo.StateActions[count - 1].EscalationMappedUsers.length == 0) {
                        self.StateInfo.StateActions[count - 1].EscalationMappedUsers.push({ Name: a })
                        if ($("#ruleTxt").val() != '' && $("#ruleTxt").val() != undefined) {
                            var editor = $(".RADWorkFlowRuleDiv").data('ruleEngine');
                            var classDocument = editor.getGeneratedCode();
                            var ruleInfo = {};
                            ruleInfo = self.gerRuleInfo(ruleInfo);
                            self.saveEscalatedRuleSet(count - 1, 0, ruleInfo, classDocument, false);
                        }
                    }
                    else if (self.StateInfo.StateActions[count - 1].EscalationMappedUsers.length > 0) {
                        if (self.indexOfEdit == null && self.indexOfEdit == undefined) {

                            if ($("#ruleTxt").val() != '' && $("#ruleTxt").val() != undefined) {
                                self.StateInfo.StateActions[count - 1].EscalationMappedUsers.push({ Name: a })
                                var editor = $(".RADWorkFlowRuleDiv").data('ruleEngine');
                                var classDocument = editor.getGeneratedCode();
                                var ruleInfo = {};
                                ruleInfo = self.gerRuleInfo(ruleInfo);
                                self.saveEscalatedRuleSet(count - 1, self.StateInfo.StateActions[count - 1].EscalationMappedUsers.length - 1, ruleInfo, classDocument, true);
                            }
                            else {
                                var c = 0;
                                for (var i = 0; i < self.StateInfo.StateActions[count - 1].EscalationMappedUsers.length; i++) {
                                    if (self.StateInfo.StateActions[count - 1].EscalationMappedUsers[i].ruleText != undefined) {
                                        if (self.StateInfo.StateActions[count - 1].EscalationMappedUsers[i].ruleText != "") {
                                            c++;
                                            break;
                                        }
                                    }
                                }
                                if (c > 0) {
                                    var msg = "Can't add condition becoz other conditions have rule";
                                    self.ShowErrorMessage(msg);
                                    return;
                                }
                                else {
                                    self.StateInfo.StateActions[count - 1].EscalationMappedUsers[i].Name = a;
                                }
                            }
                        }
                        else if (self.StateInfo.StateActions[count - 1].EscalationMappedUsers[self.indexOfEdit].hasOwnProperty("Name")) {
                            if ($("#ruleTxt").val() != '' && $("#ruleTxt").val() != undefined) {
                                self.StateInfo.StateActions[count - 1].EscalationMappedUsers[self.indexOfEdit].Name = a
                                var editor = $(".RADWorkFlowRuleDiv").data('ruleEngine');
                                var classDocument = editor.getGeneratedCode();
                                var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc"

                                if (self.StateInfo.StateActions[count - 1].EscalationMappedUsers[self.indexOfEdit].ruleSetId > 0) {
                                    $.ajax({
                                        url: url + '/GetRuleSetDetails',
                                        type: 'POST',
                                        contentType: "application/json",
                                        data: JSON.stringify({ rulesetId: self.StateInfo.StateActions[count - 1].EscalationMappedUsers[self.indexOfEdit].ruleSetId }),
                                        dataType: 'json'
                                    }).then(function (responseText) {
                                        var ruleInfo = {};
                                        ruleInfo = self.gerRuleInfo(ruleInfo);
                                        ruleInfo.RuleId = $.parseJSON(responseText.d).Rules[0].RuleID;
                                        ruleInfo.RuleSetId = self.StateInfo.StateActions[count - 1].EscalationMappedUsers[self.indexOfEdit].ruleSetId;
                                        self.saveEscalatedRuleSet(count - 1, self.indexOfEdit, ruleInfo, classDocument, true);
                                    })
                                }
                                else {
                                    var ruleInfo = {};
                                    ruleInfo = self.gerRuleInfo(ruleInfo);
                                    self.saveEscalatedRuleSet(count - 1, self.indexOfEdit, ruleInfo, classDocument, true);
                                }
                            }
                            else {
                                var c = 0;
                                for (var i = 0; i < self.StateInfo.StateActions[count - 1].EscalationMappedUsers.length; i++) {
                                    if (self.StateInfo.StateActions[count - 1].EscalationMappedUsers[i].ruleText != undefined) {
                                        if (self.StateInfo.StateActions[count - 1].EscalationMappedUsers[i].ruleText != "") {
                                            c++;
                                            break;
                                        }
                                    }
                                }
                                if (c > 0) {
                                    var msg = "Can't add condition becoz other conditions have rule";
                                    self.ShowErrorMessage(msg);
                                    return;
                                }
                                else {
                                    self.StateInfo.StateActions[count - 1].EscalationMappedUsers[self.indexOfEdit].Name = a;
                                }
                            }
                        }
                    }


                    //self.StateInfo.StateActions[count - 1].EscalationMappedUsers.push({ Name: a })
                }
                else {
                    self.StateInfo.StateActions.push({ Action: $(".RADWorkFlowStateActionItemColor").text(), MappedUsers: [], EscalationMappedUsers: [] });
                    var a = {};
                    for (var j = 0; j < $(".select").length; j++) {
                        if ($($(".select")[j]).closest(".RADWorkFlowStateUserParent").length > 0) {
                            var text = $($(".select")[j]).text();
                            a[text] = true;
                        }
                        else if ($($(".select")[j]).closest(".RADWorkFlowStateGroupParent").length > 0) {
                            var text = $($(".select")[j]).text();
                            a[text] = false;
                        }
                    }
                    self.StateInfo.StateActions[self.StateInfo.StateActions.length - 1].EscalationMappedUsers.push({ Name: a })
                }

                self.StateBindings.mappedInfo().splice(0, self.StateBindings.mappedInfo().length)
                self.StateBindings.mappedInfo.push({ RuleText: $("#ruleTxt").val() })
                $(".RADWorkFlowStateUserComponentParent").hide();
                $(".RADWorkFlowStateUserMappedParent").show();
            }
        },


        //Save User and Rule Info Mapping
        SaveOneItemFunc: function (model, event) {
            var self = this;
            var ruleInfo = {};
            if ($(event.target).hasClass("clickable")) {
                if ($(".select").length == 0) {
                    var msg = "Please select user or group mapping";
                    self.ShowErrorMessage(msg);
                    return;
                }
                $(".radWFStateErrMsg").parent().addClass("radDisplayNoneClass");
                //For Normal Mapped Users
                if ($(".RADNormalCss").length > 0) {
                    self.saveUserandRuleInfoMapping(model, event);
                }
                //Code for Escalated Mapped User.
                else {
                    self.saveEscalatedUserandRuleInfoMapping(model,event)
                }
            }
        }
    }

});
$.extend(radworkflow, {
    mailsubscription: {
        pageLoad: function (workFlowState) {
            var self = this;
            if ($("#RADWorkFlowStateComponent").length > 0)
                $("#RADWorkFlowStateComponent")[0].style.opacity = .3;
            self.AllUserInfo = [];
            self.usersAddedToToListNormal = [];
            self.usersAddedToToListEscalted = [];
            self.WorkFlowState = workFlowState;
            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc";

            $.ajax({
                url: url + '/GetHtmlTemplate',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivp.rad.RRadWorkflow.Resources.MailSubscription.html' })
            }).then(function (responseText) {
                $("#" + radworkflow.identifier).append(responseText.d);
                self.allMailSubscriptionEventHandlers();
                $('.RADWFMailSummerNoteContent').summernote({
                    height: 130,
                    minHeight: 130,
                    maxHeight: 130,
                    focus: true,
                    toolbar: [
                      ['style', ['bold', 'italic', 'underline', 'clear']],
                      ['font', ['strikethrough', 'superscript', 'subscript']],
                      ['fontsize', ['fontsize']],
                      ['color', ['color']],
                      ['para', ['ul', 'ol', 'paragraph']],
                      ['height', ['height']]
                    ]
                });
                self.getAllUsers();
                $(".RADWFMailSubscriptionContent").click(function () {
                    $(".RADWFMailUserAutoSuggestPopup").remove();
                    if ($(event.target).hasClass("RADWFMailTemplate")) {
                        if ($(".radWFMailTemplatePopUp").length > 1)
                            $(".radWFMailTemplatePopUp").remove();
                        else
                            self.createTemplateDropDown();
                    }
                    else {
                        if ($(event.target).closest(".RADWFMailTemplateParent").length == 0)
                            $(".radWFMailTemplatePopUp").remove();
                    }
                    $(".RADWFMailTempPopUp").remove();
                });
            });
        },

        getAllUsers: function () {
            var self = this;
            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc"
            $.ajax({
                url: url + '/GetUserDetails',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json'
            }).then(function (responseText) {
                if (responseText.d != "")
                    self.AllUserInfo = responseText.d;
                if (self.WorkFlowState.mailInfo != null && self.WorkFlowState.mailInfo.to != "") {
                    self.bindExistingMaiInfo(self.WorkFlowState.mailInfo, 'Normal');
                }
            });
        },

        allMailSubscriptionEventHandlers: function () {
            var self = this;
            $(".RADWFMailSubscriptionToText").keyup(function (event) {
                self.autoSuggestUserPopUp(event);
            });

            $(".RADWFMailSubscriptionToText").unbind("focusout").focusout(function (event) {
                //self.autoSuggestUserPopUp(event);
                if ($(event.target).closest(".RADWFMailUserAutoSuggestPopup").length == 0) {
                    if ($(event.target).html().trim().indexOf("@") > -1) {
                        self.userSelectionClickHandler(event);
                    }
                }
            });

            $(".RADWFMailNormalType").click(function (event) {
                $(".RADWFMailEscaledType").removeClass("RADWFMailToggleHighLight");
                $(event.target).addClass("RADWFMailToggleHighLight");
                self.clearMailContentControls(event);
                if (self.WorkFlowState.mailInfo != null && self.WorkFlowState.mailInfo.to != "")
                    self.bindExistingMaiInfo(self.WorkFlowState.mailInfo, 'Normal');
            });

            $(".RADWFMailEscaledType").click(function (event) {
                if ($(".RADWFEscalateText").text() != "") {
                    $(".RADWFMailNormalType").removeClass("RADWFMailToggleHighLight");
                    $(event.target).addClass("RADWFMailToggleHighLight");
                    self.clearMailContentControls(event);
                    if (self.WorkFlowState.escalationInfo != null && self.WorkFlowState.escalationInfo.EscalationMail != null &&
                        self.WorkFlowState.escalationInfo.EscalationMail.to != "")
                        self.bindExistingMaiInfo(self.WorkFlowState.escalationInfo.EscalationMail, 'Escalated');
                }
            });

            $(".RADWFMailButtonBarButton").click(function (event) {
                //Save and Close button click
                var buttonText = $(event.target).html().trim();
                if (buttonText == "Close") {
                    //radworkflow.workflowstates.CallBackFunc();
                    if (self.WorkFlowState.StateName.toLowerCase() == "failed" || self.WorkFlowState.StateName.toLowerCase() == "end") {
                        //self.saveWorkflowState();
                        $(".RADWFMailSubscriptionMain").remove();
                        $(".radAddWorkFlowMain")[0].style.opacity = 1;
                    }
                    else {
                        $(".RADWFMailSubscriptionMain").remove();
                        radworkflow.workflowstates.CallBackFunc();
                    }
                }
                else if (buttonText == "Done") {
                    self.onSaveMailConfiguration(event);
                }

            });
        },

        // Saving Worklfow State with Mail Info Only for ENd and Failed State.
        saveWorkflowState: function () {
            var self = this;
            var url = radworkflow.baseUrl + "/Resources/Services/RADWorkFlow.svc"
            $.ajax({
                url: url + '/SaveWorkflowState',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ stateInfo: JSON.stringify(self.WorkFlowState), workflowId: radworkflow.workflowstates.workflowId, user: radworkflow.user })
            }).then(function (responseText) {
                if (responseText.d > 0) {
                    self.WorkFlowState.stateId = responseText.d;
                    $(".RADWorkFlowMainParent").remove();
                    //radworkflow.addWorkFlow.createConnections(self.StateInfo);
                    $(".RADWFMailSubscriptionMain").remove();
                    $(".radAddWorkFlowMain")[0].style.opacity = 1;
                }
            })
        },

        onSaveMailConfiguration: function (event) {
            var self = this;
            var isMailConfigurationValid = self.validateDataonMailConfiguration(event);
            if (isMailConfigurationValid) {
                self.saveMailConfiguration(event);
               
                if (self.WorkFlowState.StateName.toLowerCase() == "failed" || self.WorkFlowState.StateName.toLowerCase() == "end")
                    self.saveWorkflowState();
                else {
                    radworkflow.workflowstates.CallBackFunc();
                    $(".RADWFMailSubscriptionMain").remove();
                }
            }
        },

        validateDataonMailConfiguration: function (event) {
            var self = this;
            $(".RADWFMailSubscriptionMessage").html("");
            var isMailConfigurationValid = true;
            if ($(event.target).closest(".RADWFMailSubscriptionContent").find("div[radmailcontrol='to']").find(".RADWFMailAddedUserParent").length == 0) {
                $(".RADWFMailSubscriptionMessage").html("Please specify at least one recipient in to list");
                isMailConfigurationValid = false;
                return isMailConfigurationValid;
            }
            else {
                $(".RADWFMailSubscriptionMessage").html("");
            }

            if ($(".RADWFMailSubscriptionSubjectText").html().trim() == "") {
                $(".RADWFMailSubscriptionMessage").html("Please specify the subject");
                isMailConfigurationValid = false;
                return isMailConfigurationValid;
            }
            else {
                $(".RADWFMailSubscriptionMessage").html("");
            }
            return isMailConfigurationValid;
        },

        saveMailConfiguration: function (event, mailType) {
            var self = this;
            var mailInfo = {};
            $(event.target).closest(".RADWFMailSubscriptionContent").find("div[radmailcontrol]").each(function (index) {
                self.getRecipentList($(this), mailInfo);
            });

            //var fromUserInfo = $.grep(self.AllUserInfo, function (e) { return e.Key == radworkflow.user });
            mailInfo.from = radworkflow.user;
            mailInfo.subject = $(".RADWFMailSubscriptionSubjectText").html().trim();
            mailInfo.body = $('.RADWFMailSummerNoteContent').summernote('code');
            mailInfo.includeMappedUsers = $("#RADWCChkIncMappedUsers")[0].checked;
            mailInfo.keepCreatorInCC = $("#RADWCChkKeepCreatorInCC")[0].checked;

            if ($(".RADWFMailNormalType").hasClass("RADWFMailToggleHighLight")) {
                $(".RADWFMailSubscriptionMessage").html("Normal Mail Subscription saved sucessfuly");
                $(".RADWFMailSubscriptionMessage").css("color", "green");
                self.WorkFlowState.mailInfo = mailInfo;
            }
            else {
                $(".RADWFMailSubscriptionMessage").html("Escalated Mail Subscription saved sucessfuly");
                $(".RADWFMailSubscriptionMessage").css("color", "green");
                self.WorkFlowState.escalationInfo = {};
                self.WorkFlowState.escalationInfo.EscalationMail = mailInfo;
            }
        },

        getRecipentList: function (currentEvent, mailInfo) {
            var self = this;
            var usersEmailList = [];
            if (currentEvent.find(".RADWFMailSubscriptionToTextParent").find('.RADWFMailAddedUserParent').length != 0) {
                currentEvent.find(".RADWFMailSubscriptionToTextParent").find('.RADWFMailAddedUserParent').each(function (toIndex) {
                    var userLoginName = $(this).find(".RADWFMailAddedUserChildText").html().trim();
                    var currentUser = ""
                    if ($(".RADWFMailNormalType").hasClass("RADWFMailToggleHighLight")) {
                        //if (self.usersAddedToToListNormal.indexOf(userLoginName) != -1) {
                            currentUser = userLoginName
                        //}
                    }
                    else {
                        //if (self.usersAddedToToListEscalted.indexOf(userLoginName) != -1) {
                            currentUser = userLoginName
                        //}
                    }
                    usersEmailList.push(currentUser);
                });
            }

            if (currentEvent.attr("radmailcontrol").toLowerCase() == "to")
                mailInfo.to = usersEmailList.join();
            else if (currentEvent.attr("radmailcontrol").toLowerCase() == "cc")
                mailInfo.cc = usersEmailList.join();
            else if (currentEvent.attr("radmailcontrol").toLowerCase() == "bcc")
                mailInfo.bcc = usersEmailList.join();
        },

        autoSuggestUserPopUp: function (event) {
            var self = this;
            if (event.keyCode == 40) {
                $(event.target).closest(".RADWFMailSubscriptionToTextParent").find(".RADWFMailUserAutoSuggestPopup").find(".RADWFMailUserAutoSuggestParent").first().addClass("RADWFMailHightlight").focus();
                $(event.target).closest(".RADWFMailSubscriptionToTextParent").find(".RADWFMailUserAutoSuggestPopup").focus();
            }
            else if (event.keyCode == 8) {
                if ($(event.target).html().trim() == "") {
                    $(".RADWFMailAddedUserParent").last().remove();
                }
            }
            else if (event.keyCode == 13) {
                self.CodeForTilesofEmailAddress(event, $(event.target).html().split("<br><br>")[0]);
            }
            else if (event.keyCode == 188) {
                if (radworkflow.utility.isNotNullorEmpty($(event.target).html().split(",")[0]))
                    self.CodeForTilesofEmailAddress(event, $(event.target).html().split(",")[0]);
                else
                    $(".RADWFMailSubscriptionToText").html("");
            }
            else {
                var div = null;
                $(".RADWFMailUserAutoSuggestPopup").remove();

                div = $("<div>", {
                    class: 'RADWFMailUserAutoSuggestPopup'
                });
                var child = $("<div>", {
                    class: 'RADWFMailUserAutoSuggestPopupChild'
                });
                div.append(child);
                for (var currentIndex = 0; currentIndex < self.AllUserInfo.length; currentIndex++) {
                    var currentUserInfo = [];
                    var normalOrescalated = [];
                    if ($(".RADWFMailNormalType").hasClass("RADWFMailToggleHighLight"))
                        normalOrescalated = self.usersAddedToToListNormal;
                    else
                        normalOrescalated = self.usersAddedToToListEscalted;

                    if (self.AllUserInfo[currentIndex].toLowerCase().indexOf($(event.target).html().trim().toLowerCase()) != -1) {
                        //if (normalOrescalated.indexOf(self.AllUserInfo[currentIndex]) == -1) {
                        var divParent = $("<div>", {
                            class: 'RADWFMailUserAutoSuggestParent',
                            tabIndex: 1
                        });

                        var divChild1 = $("<div>", {
                            class: 'RADWFMailUserAutoSuggestNameChild',
                            text: self.AllUserInfo[currentIndex]
                        });
                        divChild1.attr("title", self.AllUserInfo[currentIndex]);

                        divParent.append(divChild1);
                        child.append(divParent);
                        //}
                    }
                }
                $(event.target).closest(".RADWFMailSubscriptionToTextParent").append(div);
                $(event.target).closest(".RADWFMailSubscriptionToTextParent")
                               .find(".RADWFMailUserAutoSuggestPopup")
                               .css({
                                   'left': ($(event.target).closest(".RADWFMailSubscriptionToTextParent")
                                       .find(".RADWFMailSubscriptionToText").position().left + 'px')
                               });
                $(".RADWFMailUserAutoSuggestNameChild,.RADWFMailUserAutoSuggestEmailChild").click(function (event) {
                    self.userSelectionClickHandler(event);
                });

                $(".RADWFMailUserAutoSuggestPopup").unbind("keydown").keydown(function (event) {
                    self.keyDownEventUserAutoSuggestPopUp(event);
                });
            }
        },

        userSelectionClickHandler: function (event) {
            var self = this;
            var userLoginName = "";
            if ($(event.target).closest('.RADWFMailUserAutoSuggestParent').length > 0)
                userLoginName = $(event.target).closest('.RADWFMailUserAutoSuggestParent').find('.RADWFMailUserAutoSuggestNameChild').html().trim();
            else
                userLoginName = $(".RADWFMailSubscriptionToText").html().trim();
            //var userEmailId = $(event.target).closest('.RADWFMailUserAutoSuggestParent').find('.RADWFMailUserAutoSuggestEmailChild').html().trim();
            //var currentUser = {};
            //currentUser["Key"] = userLoginName;
            //currentUser["Value"] = userEmailId;

            var isUserExistsInMailList = false;
            if ($(".RADWFMailNormalType").hasClass("RADWFMailToggleHighLight")) {
                //isUserExistsInMailList = ($.grep(self.usersAddedToToListNormal, function (e) { return e.Key == userLoginName })).length > 0 ? true : false;
                //if (self.usersAddedToToListNormal.indexOf(userLoginName) == -1)
                    self.usersAddedToToListNormal.push(userLoginName);
            }

            else {
                //isUserExistsInMailList = ($.grep(self.usersAddedToToListEscalted, function (e) { return e.Key == userLoginName })).length > 0 ? true : false;
                //if (self.usersAddedToToListEscalted.indexOf(userLoginName) == -1)
                    self.usersAddedToToListEscalted.push(userLoginName);
            }
            self.CodeForTilesofEmailAddress(event, userLoginName);

        },
        CodeForTilesofEmailAddress: function (event, userLoginName) {
            var self = this;
            var divParent = $("<div>", {
                class: 'RADWFMailAddedUserParent'
            });
            var div = $("<div>", {
                class: 'RADWFMailAddedUserChild'
            });
            var divImg = $("<div>", {
                class: 'RADWFMailAddedUserChildIco fa fa-user'
            });
            var divtext = $("<div>", {
                class: 'RADWFMailAddedUserChildText',
                text: userLoginName
            });

            //divtext.attr("title", userEmailId);
            var deleteDiv = $("<div>", {
                class: 'RADWFMailAddedUserChildDeleteIco fa fa-times'
            });
            div.append(divImg);
            div.append(divtext);
            div.append(deleteDiv);
            divParent.append(div)
            $(event.target).closest(".RADWFMailSubscriptionToTextParent").find(".RADWFMailSubscriptionToText").before(divParent);
            $(event.target).closest(".RADWFMailSubscriptionToTextParent").find(".RADWFMailSubscriptionToText").html("");

            var width = $(event.target).closest(".RADWFMailSubscriptionToTextParent").width() - self.getAllUserAddedDivWidth(event);
            if (width > 150)
                $(event.target).closest(".RADWFMailSubscriptionToTextParent").find(".RADWFMailSubscriptionToText").css({ 'width': (width + 'px') });
            else
                $(event.target).closest(".RADWFMailSubscriptionToTextParent").find(".RADWFMailSubscriptionToText").css({ 'width': "100%" });

            $(".RADWFMailUserAutoSuggestPopup").remove();
            $(event.target).closest(".RADWFMailSubscriptionToTextParent").find(".RADWFMailSubscriptionToText").focus();

            $(".RADWFMailAddedUserChildDeleteIco").click(function (event) {
                self.deleteUserAddedInToList(event);
            });
        },
        getAllUserAddedDivWidth: function (event) {
            var width = 0;
            var counter = 0;
            $(event.target).closest(".RADWFMailSubscriptionToTextParent").find(".RADWFMailAddedUserParent").each(function (index) {
                width += $(this).outerWidth();
                counter++;
                if (width > $(event.target).closest(".RADWFMailSubscriptionToTextParent").width()) {
                    counter++;
                    width = $(this).outerWidth();
                }
            })
            return width + counter;
        },

        //Delete Added User on Click of Cross Icon
        deleteUserAddedInToList: function (event) {
            var self = this;
            var userName = $(event.target).prev().html().trim();
            if ($(".RADWFMailNormalType").hasClass("RADWFMailToggleHighLight"))
                self.usersAddedToToListNormal = $.grep(self.usersAddedToToListNormal, function (e) { return e.Key != userName });
            else
                self.usersAddedToToListEscalted = $.grep(self.usersAddedToToListEscalted, function (e) { return e.Key != userName });

            var width = $(event.target).closest(".RADWFMailSubscriptionToTextParent").width() - self.getAllUserAddedDivWidth(event);
            if (width > 150)
                $(event.target).closest(".RADWFMailSubscriptionToTextParent").find(".RADWFMailSubscriptionToText").css({ 'width': (width + 'px') });
            else
                $(event.target).closest(".RADWFMailSubscriptionToTextParent").find(".RADWFMailSubscriptionToText").css({ 'width': "100%" });
            $(event.target).closest(".RADWFMailAddedUserParent").remove();
        },

        keyDownEventUserAutoSuggestPopUp: function (event) {
            var self = this;
            if (event.keyCode == 9) {
                // on tab press
                self.userSelectionClickHandler(event);
                $(event.target).closest(".RADWFMailSubscriptionToTextParent").find(".RADWFMailSubscriptionToText").focus();
                event.preventDefault();
                return false;
            }
            else if (event.keyCode == 40) {
                //down arrow key
                if ($(event.target).next().length > 0) {
                    $(event.target).removeClass("RADWFMailHightlight");
                    $(event.target).next().addClass("RADWFMailHightlight").focus();
                    var scroll = $(event.target).closest(".RADWFMailUserAutoSuggestPopupChild").scrollTop();
                    $(event.target).closest(".RADWFMailUserAutoSuggestPopupChild").scrollTop(scroll + 40);
                    event.preventDefault();
                    return false;
                }
            }
            else if (event.keyCode == 38) {
                //up arrow key
                if ($(event.target).prev().length > 0) {
                    $(event.target).removeClass("RADWFMailHightlight");
                    $(event.target).prev().first().addClass("RADWFMailHightlight").focus();
                    var scroll = $(event.target).closest(".RADWFMailUserAutoSuggestPopupChild").scrollTop();
                    $(event.target).closest(".RADWFMailUserAutoSuggestPopupChild").scrollTop(scroll - 40);
                    event.preventDefault();
                    return false;
                }
            }
        },

        bindExistingMaiInfo: function (mailInfo, mailType) {
            var self = this;
            self.getRecipentInfo(mailInfo);
            $(".RADWFMailSubscriptionSubjectText").html(mailInfo.subject);
            $('.RADWFMailSummerNoteContent').summernote('code', mailInfo.body);
            $("#RADWCChkIncMappedUsers")[0].checked = mailInfo.includeMappedUsers;
            $("#RADWCChkIncMappedUsers")[0].checked = mailInfo.keepCreatorInCC;
            $("#RADWCChkIncMappedUsers")[0].checked = mailInfo.includeMappedUsers;
            $("#RADWCChkKeepCreatorInCC")[0].checked = mailInfo.keepCreatorInCC;

            if (mailType == "Normal") {
                $('.RADWFMailEscaledType').removeClass("RADWFMailToggleHighLight")
                $('.RADWFMailNormalType').addClass("RADWFMailToggleHighLight");
            }
            else {
                $('.RADWFMailNormalType').removeClass("RADWFMailToggleHighLight");
                $('.RADWFMailEscaledType').addClass("RADWFMailToggleHighLight");
            }
        },

        getRecipentInfo: function (mailInfo) {
            var self = this;
            var recipentTypes = ['to', 'cc', 'bcc'];
            for (var i = 0 ; i < recipentTypes.length; i++) {
                var userEmailList = [];
                var userInfoList = [];
                if (radworkflow.utility.isNotNullorEmpty(mailInfo[recipentTypes[i]])) {
                    userEmailList = mailInfo[recipentTypes[i]].split(",");
                    if (userEmailList.indexOf("") >= 0)
                        userEmailList.splice(userEmailList.indexOf(""), 1);
                }

                for (var j = 0; j < userEmailList.length; j++) {
                    //var currentUserInfo = $.grep(self.AllUserInfo, function (e) { return e.Value == userEmailList[j] });
                    userInfoList.push(userEmailList[j]);
                    //var isUserExistsInMailList = false;
                    if ($(".RADWFMailNormalType").hasClass("RADWFMailToggleHighLight")) {
                        //isUserExistsInMailList = ($.grep(self.usersAddedToToListNormal, function (e) { return e.Key == currentUserInfo[0].Key })).length > 0 ? true : false;
                        //if (self.usersAddedToToListNormal.indexOf(userEmailList[j]) == -1)
                            self.usersAddedToToListNormal.push(userEmailList[j]);
                    }
                    else {
                        //isUserExistsInMailList = ($.grep(self.usersAddedToToListEscalted, function (e) { return e.Key == currentUserInfo[0].Key })).length > 0 ? true : false;
                        //if (!isUserExistsInMailList)
                        //    self.usersAddedToToListEscalted.push(currentUserInfo[0]);
                        //if (self.usersAddedToToListEscalted.indexOf(userEmailList[j]) == -1)
                            self.usersAddedToToListEscalted.push(userEmailList[j]);
                    }
                }
                var currentParentControl = $(".RADWFMailSubscriptionMailParent").find("div[radmailcontrol=" + recipentTypes[i] + "]")
                self.bindCurrentRecipentInfo(userInfoList, currentParentControl);
            }
        },

        bindCurrentRecipentInfo: function (userInfoList, currentParentControl) {
            var self = this;
            for (var i = 0; i < userInfoList.length ; i++) {

                var currentUserInfo = userInfoList[i];

                var divParent = $("<div>", {
                    class: 'RADWFMailAddedUserParent'
                });
                var div = $("<div>", {
                    class: 'RADWFMailAddedUserChild'
                });
                var divImg = $("<div>", {
                    class: 'RADWFMailAddedUserChildIco fa fa-user'
                });
                var divtext = $("<div>", {
                    class: 'RADWFMailAddedUserChildText',
                    text: currentUserInfo
                });

                //divtext.attr("title", currentUserInfo.Value);
                var deleteDiv = $("<div>", {
                    class: 'RADWFMailAddedUserChildDeleteIco fa fa-times'
                });
                div.append(divImg);
                div.append(divtext);
                div.append(deleteDiv);
                divParent.append(div)
                currentParentControl.find(".RADWFMailSubscriptionToTextParent").find(".RADWFMailSubscriptionToText").before(divParent);
                currentParentControl.find(".RADWFMailSubscriptionToTextParent").find(".RADWFMailSubscriptionToText").html("");

                var width = currentParentControl.find(".RADWFMailSubscriptionToTextParent").width() - self.getAllUserAddedDivWidthExisting(currentParentControl);
                if (width > 150)
                    currentParentControl.find(".RADWFMailSubscriptionToTextParent").find(".RADWFMailSubscriptionToText").css({ 'width': (width + 'px') });
                else
                    currentParentControl.find(".RADWFMailSubscriptionToTextParent").find(".RADWFMailSubscriptionToText").css({ 'width': "100%" });

                $(".RADWFMailUserAutoSuggestPopup").remove();
                currentParentControl.find(".RADWFMailSubscriptionToTextParent").find(".RADWFMailSubscriptionToText").focus();

                $(".RADWFMailAddedUserChildDeleteIco").click(function (event) {
                    self.deleteUserAddedInToList(event);
                });
            }
        },

        getAllUserAddedDivWidthExisting: function (currentParentControl) {
            var self = this;
            var width = 0;
            var counter = 0;
            currentParentControl.find(".RADWFMailSubscriptionToTextParent").find(".RADWFMailAddedUserParent").each(function (index) {
                width += $(this).outerWidth();
                counter++;
                if (width > currentParentControl.find(".RADWFMailSubscriptionToTextParent").width()) {
                    counter++;
                    width = $(this).outerWidth();
                }
            })
            return width + counter;
        },

        clearMailContentControls: function (event) {
            var self = this;
            $('#RADWCChkIncMappedUsers')[0].checked = false;
            $('#RADWCChkKeepCreatorInCC')[0].checked = false;
            $('.RADWFMailAddedUserParent').remove();
            $('.RADWFMailSubscriptionSubjectText').html("");
            $('.RADWFMailSummerNoteContent').summernote('code', "");
            $(".RADWFMailSubscriptionMessage").html("");
            $(".RADWFMailSubscriptionToText").css({ 'width': "20%" });
        },

        createTemplateDropDown: function () {
            var div = $('<div>', {
                class: 'radWFMailTemplatePopUp'
            });

            //Row Starts
            var divButtonBar = $("<div>", {
                class: 'radWFTemplateRow'
            });

            var divButtonParent = $("<div>", {
                class: 'radWFTemplateRowParent'
            });

            divButtonParent.append($("<div>", {
                class: 'radWFTemplateLabel',
                'text': '{Workflow}'
            }));
            divButtonParent.append($("<div>", {
                class: 'radWFTemplateText',
                'text': 'Workflow Name'
            }));
            divButtonBar.append(divButtonParent);
            div.append(divButtonBar);
            divButtonBar.find(".radWFTemplateText").attr("title", 'Workflow Name');
           
            //Row Starts
            divButtonBar = $("<div>", {
                class: 'radWFTemplateRow'
            });

            var divButtonParent = $("<div>", {
                class: 'radWFTemplateRowParent'
            });
            divButtonParent.append($("<div>", {
                class: 'radWFTemplateLabel',
                'text': '{FromState}'
            }));
            divButtonParent.append($("<div>", {
                class: 'radWFTemplateText',
                'text': 'State on which Action is Performed'
            }));
            divButtonBar.append(divButtonParent);
            div.append(divButtonBar);
            divButtonBar.find(".radWFTemplateText").attr("title", 'State on which Action is Performed');
            //Row Starts
            divButtonBar = $("<div>", {
                class: 'radWFTemplateRow'
            });

            var divButtonParent = $("<div>", {
                class: 'radWFTemplateRowParent'
            });
            divButtonParent.append($("<div>", {
                class: 'radWFTemplateLabel',
                'text': '{ToState}'
            }));
            divButtonParent.append($("<div>", {
                class: 'radWFTemplateText',
                'text': 'Current State'
            }));
            divButtonBar.append(divButtonParent);
            divButtonBar.find(".radWFTemplateText").attr("title", 'State on which Action is Performed');
            div.append(divButtonBar);

            //Row Starts
            divButtonBar = $("<div>", {
                class: 'radWFTemplateRow'
            });

            var divButtonParent = $("<div>", {
                class: 'radWFTemplateRowParent'
            });
            divButtonParent.append($("<div>", {
                class: 'radWFTemplateLabel',
                'text': '{Action}'
            }));
            divButtonParent.append($("<div>", {
                class: 'radWFTemplateText',
                'text': 'Action Performed'
            }));
            divButtonBar.append(divButtonParent);
            divButtonBar.find(".radWFTemplateText").attr("title", 'State on which Action is Performed');
            div.append(divButtonBar);

            //Row Starts
            divButtonBar = $("<div>", {
                class: 'radWFTemplateRow'
            });

            var divButtonParent = $("<div>", {
                class: 'radWFTemplateRowParent'
            });
            divButtonParent.append($("<div>", {
                class: 'radWFTemplateLabel',
                'text': '{User}'
            }));
            divButtonParent.append($("<div>", {
                class: 'radWFTemplateText',
                'text': 'User'
            }));
            divButtonBar.append(divButtonParent);
            divButtonBar.find(".radWFTemplateText").attr("title", 'State on which Action is Performed');
            div.append(divButtonBar);

            for (var i = 0; i < radworkflow.addWorkFlow.currentWorkFlow.workflowAttibutes.length; i++) {
                divButtonBar = $("<div>", {
                    class: 'radWFTemplateRow'
                });

                var divButtonParent = $("<div>", {
                    class: 'radWFTemplateRowParent'
                });
                divButtonParent.append($("<div>", {
                    class: 'radWFTemplateLabel',
                    'text': ( "{" + radworkflow.addWorkFlow.currentWorkFlow.workflowAttibutes[i].name + "}" )
                }));
                divButtonParent.append($("<div>", {
                    class: 'radWFTemplateText',
                    'text': 'Workflow Attribute'
                }));
                divButtonBar.append(divButtonParent);
                divButtonBar.find(".radWFTemplateText").attr("title", 'State on which Action is Performed');
                div.append(divButtonBar);
            }
           
            $(".RADWFMailTemplateParent").append(div);
        }
    }
});
$.extend(radworkflow, {
    attributes: {
        pageLoad: function () {
            var self = this;
            var url = radworkflow.baseUrl + '/Resources/Services/RADWorkFlow.svc'
            $.ajax({
                url: url + '/GetHtmlTemplate',
                type: 'POST',
                contentType: 'text/json',
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivp.rad.RRadWorkflow.Resources.AttributePopUp.html' })
            }).then(function (responseText) {
                $(".radAddAttributeParent").append(responseText.d);
                self.eventHandler();
                self.getAllAttributes()
            });
        },

        eventHandler:function() {
            var self = this;
            $(".radAddAttrPopUPParent").unbind('click').click(function () {
                if ($(event.target).hasClass('radAddWFbtn')) {
                    $(".radAddAttrCreate").removeClass("radDisplayNoneClass");
                    $(".radAddAttrCreate").removeAttr("isedit");
                    $(".radAttNameVal").attr('contenteditable',true).html("");
                    $(".radAttTypeVal").removeAttr("edit").html("Select Data Type");
                    $(".radAttDefVal").html("");
                }
                else if ($(event.target).hasClass('radAttrCancel')) {
                    $(".radAddAttrPopUPParent").remove();
                    //if ($(".radSavedWFAttributes").children().length > 0) {
                    //    $(".radAddAttrCreate").addClass("radDisplayNoneClass");
                    //    $(".radAddAttrCreate").removeAttr("isedit");
                    //    $(".radAttrNameValidator").addClass("radDisplayNoneClass");
                    //    $(".radAttrtypeValidator").addClass("radDisplayNoneClass");
                    //}
                }
                else if ($(event.target).hasClass('radAttrSave')) {
                    self.saveAttribute();
                }
                else if ($(event.target).hasClass('radeditAttr')) {
                    $(".radWFDataTypeParent").remove();
                    self.editAttribute(event)
                }
                else if ($(event.target).hasClass('raddeleteAttr')) {
                    $(".radWFDataTypeParent").remove();
                    self.deleteAttribute(event)
                }
                else if ($(event.target).hasClass('radAttTypeVal')) {
                    if ($(event.target).attr("edit") == null)
                        self.createDataTypeList(event);
                }
                else if ($(event.target).hasClass('radWFDataType')) {
                    $(".radAttTypeVal").html($(event.target).html());
                    $(".radAttrTypeValidator").addClass("radDisplayNoneClass");
                    $(".radWFDataTypeParent").remove();
                }
                else {
                    $(".radWFDataTypeParent").remove();
                }
                event.stopPropagation();
            });
        },

        createDataTypeList:function(event) {
            var self = this;
            var div = $("<div>", {
                class: 'radWFDataTypeParent'
            });
            div.append($("<div>", {
                class: 'radWFDataType',
                text:'Text'

            }))
            div.append($("<div>", {
                class: 'radWFDataType',
                text:'Numeric',
            }))
            div.append($("<div>", {
                class: 'radWFDataType',
                text:'DateTime'
            }))
            $(event.target).closest(".radAddAttrRowParent").append(div);
        },

        editAttribute:function(event) {
            var self = this;
            $(".radAddAttrCreate").removeClass("radDisplayNoneClass");
            $(".radAttNameVal").removeAttr('contenteditable');
            $(".radAttTypeVal").attr("edit", true);
            $(".radAttNameVal").html($(event.target).closest(".radsavedAttRow").find(".radSavedAttrName").html());
            $(".radAttDefVal").html($(event.target).closest(".radsavedAttRow").find(".radSavedAttDefVal").html());
            $(".radAttTypeVal").html($(event.target).closest(".radsavedAttRow").find(".radSavedAttTypeVal").html());
            $(".radAddAttrCreate").attr('isEdit', true);
        },

        deleteAttribute:function() {
            var self = this;
            var url = radworkflow.baseUrl + '/Resources/Services/RADWorkFlow.svc';
            $.ajax({
                url: url + '/DeleteWorkFlowAttribute',
                type: 'POST',
                contentType: 'text/json',
                dataType: 'json',
                data: JSON.stringify({ name: $(event.target).closest(".radsavedAttRow").find(".radSavedAttrName").html(), user: radworkflow.user, workFlowId: radworkflow.addWorkFlow.currentWorkFlow.WorkflowID })
            }).then(function (responseText) {
                if (responseText.d) {
                    radworkflow.utility.alertPopUp(true, 'Attribute deleted succesfully');
                    $(".radSavedWFAttributes").empty();
                    self.getAllAttributes();
                }
                else {
                    radworkflow.utility.alertPopUp(true, 'Error occurred while deleting Attribute');
                }
            })
            
        },

        saveAttribute:function() {
            var self= this;
            var url = radworkflow.baseUrl +  '/Resources/Services/RADWorkFlow.svc';
            var attrInfo = {};
            attrInfo.name = $(".radAttNameVal").html().trim();
            attrInfo.value = $(".radAttDefVal").html().trim();
            attrInfo.dataType = $(".radAttTypeVal").html().trim();
            if (self.checkValidation(attrInfo)) {
                $.ajax({
                    url: url + '/SaveWorkFlowAttribute',
                    type: 'POST',
                    contentType: 'text/json',
                    dataType: 'json',
                    data: JSON.stringify({ parameterInfo: JSON.stringify(attrInfo), user: radworkflow.user, workFlowId: radworkflow.addWorkFlow.currentWorkFlow.WorkflowID })
                }).then(function (responseText) {
                    if (responseText.d) {
                        if (radworkflow.addWorkFlow.currentWorkFlow.workflowAttibutes == null)
                            radworkflow.addWorkFlow.currentWorkFlow.workflowAttibutes = [];
                        radworkflow.addWorkFlow.currentWorkFlow.workflowAttibutes.push(attrInfo);
                        if ($(".radSavedWFAttributes").children().length > 0)
                            $(".radAddAttrCreate").addClass("radDisplayNoneClass");
                        if ($(".radAddAttrCreate").attr('isEdit') == null) {
                            var attr = new self.getGrammarColumnConfig();
                            attr.ColumnName = attrInfo.name;
                            if (radworkflow.utility.isNotNullorEmpty(attrInfo.value))
                                attr.ColumnEnumValuesJson = attrInfo.value.split(',');
                            attr.actualColumnName = attrInfo.name;
                            attr.DataTypeJson = attrInfo.dataType;
                            attr.DataType = (attrInfo.dataType == "Text" ? 1 : (attrInfo.dataType == "Numeric" ? 2 : 3));
                            radworkflow.rulrGrammarInfo.Columns.push(attr);
                            self.creteAttrRow(attrInfo);
                            radworkflow.utility.alertPopUp(true, "Attribute saved successfully");
                        }
                        else {
                            $(".radAddAttrCreate").removeAttr("isedit");
                            self.updateRow(attrInfo);
                            radworkflow.utility.alertPopUp(true, "Attribute updated successfully");
                        }
                        $(".radAttNameVal").html("");
                        $(".radAttDefVal").html("");
                        $(".radAttTypeVal").html("Select Data Type");
                        $(".radAddAttrCreate").addClass("radDisplayNoneClass");
                        $(".radAddAttrCreate").removeAttr('isEdit');
                    }
                    else {
                        radworkflow.utility.alertPopUp(true, "Error ocurred while saving attribute");
                    }
                });
            }
        },

        //Check Validation on Save CLick of Attributes.
        checkValidation: function (attrInfo) {
            var self = this;
            if (!radworkflow.utility.isNotNullorEmpty(attrInfo.name)) {
                $(".radAttrNameValidator").removeClass("radDisplayNoneClass");
                $(".radAttrNameValidator").attr("title", "Attribute Name can not be empty");
                return false;
            }
            else
                $(".radAttrNameValidator").addClass("radDisplayNoneClass");
            if (attrInfo.dataType == "Select Data Type") {
                $(".radAttrTypeValidator").removeClass("radDisplayNoneClass");
                $(".radAttrTypeValidator").attr("title", "Please select a Data Type");
                return false;
            }
            else
                $(".radAttrTypeValidator").addClass("radDisplayNoneClass");
            if ($(".radAddAttrCreate").attr("isedit") == null) {
                for (var i = 0 ; i < radworkflow.addWorkFlow.currentWorkFlow.workflowAttibutes.length; i++) {
                    if (radworkflow.addWorkFlow.currentWorkFlow.workflowAttibutes[i].name == attrInfo.name) {
                        $(".radAttrNameValidator").removeClass("radDisplayNoneClass");
                        $(".radAttrNameValidator").attr("title", "Attribute Name already exists");
                        return false;
                    }
                }
            }
            $(".radAttrNameValidator").addClass("radDisplayNoneClass");
            return true;
        },

        getAllAttributes: function () {
            var self = this;
            var url = radworkflow.baseUrl + '/Resources/Services/RADWorkFlow.svc'
            $.ajax({
                url: url + '/GetAllWorkflowAttributes',
                type: 'POST',
                contentType: 'text/json',
                dataType: 'json',
                data: JSON.stringify({ workFlowId: radworkflow.addWorkFlow.currentWorkFlow.WorkflowID })
            }).then(function (responseText) {
                radworkflow.addWorkFlow.currentWorkFlow.workflowAttibutes = responseText.d
                //radworkflow.rulrGrammarInfo.Columns = [];
                self.bindAllAttributes(responseText.d);
                if (responseText.d.length > 0) {
                    $(".radAddAttrCreate").addClass("radDisplayNoneClass");
                }
            });
        },

        bindAllAttributes: function (attributesList) {
            var self = this;
            for (var i = 0; i < attributesList.length; i++) {
                //var attr = new self.getGrammarColumnConfig();
                //attr.ColumnName = attributesList[i].name;
                //attr.actualColumnName = attributesList[i].name;
                //attr.DataTypeJson = attributesList[i].dataType;
                //attr.DataType = (attributesList[i].dataType == "Text" ? 1 : (attributesList[i].dataType == "Numeric" ? 2 : 3));
                //radworkflow.rulrGrammarInfo.Columns.push(attr);
                self.creteAttrRow(attributesList[i]);
            }
        },

        creteAttrRow: function (attrInfo) {
            var div = $(".radAttrSavedClone").clone();
            div.switchClass("radAttrSavedClone", "radsavedAttRow");
            div.removeClass("radDisplayNoneClass");
            div.attr("attrName", attrInfo.name);
            div.find(".radSavedAttrName").html(attrInfo.name);
            div.find(".radSavedAttDefVal").html(attrInfo.value)
            div.find(".radSavedAttTypeVal").html(attrInfo.dataType);
            $(".radSavedWFAttributes").append(div);
        },

        updateRow: function (attrInfo) {
            var div = $(".radSavedWFAttributes").find(".radsavedAttRow[attrName='" + attrInfo.name + "']");
            div.attr("attrName", attrInfo.name);
            div.find(".radSavedAttrName").html(attrInfo.name);
            div.find(".radSavedAttDefVal").html(attrInfo.value)
            div.find(".radSavedAttTypeVal").html(attrInfo.dataType);
        },
        getGrammarColumnConfig: function () {
            this.ActionSideApplicability = 1;
            this.ActionSideApplicabilityJson = "RHS";
            this.ColumnEnumValues = Object;
            this.ColumnEnumValuesJson = Array[0];
            this.ColumnName = "Beneficiary Address";
            this.ColumnType = 0;
            this.ColumnTypeJson = "Both";
            this.DataType = 1;
            this.DataTypeJson = "Text";
            this.ExpressionSideApplicability = 0;
            this.ExpressionSideApplicabilityJson = "BOTH";
            this.IsAggregationColumn = false;
            this.IsRhsColumn = true;
            this.IsRhsEnum = true;
            this.IsRhsUserInput = true;
            this.actualColumnName = "Beneficiary_Address";
            this.columnPrefix = "";
        }

        
    }
});