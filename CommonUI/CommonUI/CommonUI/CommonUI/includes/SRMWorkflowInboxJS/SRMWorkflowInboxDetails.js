var SRMWorkflowInboxDetails = function () {
    var isDetailsPopupOpen = false;
    var container = null;
    var actionHistoryIFrame = null;
    var inboxSectionEnum = {
        "Pending At Me": 0,
        "Rejected Requests": 1,
        "My Requests": 2
    }
    var inboxSections = [Object.keys(inboxSectionEnum)[0], Object.keys(inboxSectionEnum)[1], Object.keys(inboxSectionEnum)[2]];
    var inboxSectionsVM = null;
    var workflowGridVM = null;
    var gridHeaderVM = null;
    var messagePopupVM = null;
    var gridSearchOnFields = ["InstrumentId", "TypeName", "PrimaryIdentifierValue", "RequestedBy"];
    var selectedTypesFromFilter = [];
    //var ignoreActionsList = ["Edit and Approve"];

    var WorkflowTypeEnum = {
        CREATE: 0,
        UPDATE: 1,
        ATTRIBUTE: 2,
        LEG: 3,
        DELETE: 4
    };

    var currentUser = "";
    var currentModule = "";
    var currentWorkflowType = "";
    var currentDateFormat = "";

    var refreshCallback;

    var gridObj = {
        "SelectRecordsAcrossAllPages": true,
        "CacheGriddata": true,
        "UserId": "",
        "CheckBoxInfo": null,
        "Height": "400px",
        "ColumnsWithoutClientSideFunctionality": [],
        "ColumnsNotToSum": [],
        "RequireEditGrid": false,
        "RequireEditableRow": false,
        "PageSize": 100,
        "RequirePaging": true,
        "RequireInfiniteScroll": false,
        "CollapseAllGroupHeader": false,
        "GridTheme": 2,
        "HideFooter": false,
        "DoNotExpand": false,
        "ItemText": "Security Drafts",
        "DoNotRearrangeColumn": true,
        "RequireGrouping": true,
        "RequireMathematicalOperations": false,
        "RequireSelectedRows": false,
        "RequireExportToExcel": false,
        "RequireSearch": true,
        "RequireFreezeColumns": false,
        "RequireHideColumns": true,
        "RequireColumnSwap": false,
        "RequireGroupExpandCollapse": true,
        "RequireResizing": true,
        "RequireLayouts": false,
        "RequireViews": false,
        "RequireRuleBasedColoring": false,
        "RequireGroupHeaderCheckbox": false,
        "ShowRecordCountOnHeader": false,
        "ShowAggregationOnHeader": false,
        "RequireFilter": true,
        "CssExportRows": "xlneoexportToExcel"
    }

    //VIEW MODELS - START

    function gridHeader() {
        var self = this;
        self.SelectAll = ko.observable(false);
        self.SearchText = ko.observable("");
        self.workflowActionsList = ko.observableArray([]);

        self.SelectAll.subscribe(function (value) {
            for (let i in workflowGridVM.workflowsList()) {
                var workflow = workflowGridVM.workflowsList()[i];
                workflow.IsSelected(value);
            }
        });
        self.SearchText.subscribe(function (value) {
            if (workflowGridVM == null)
                return;

            searchWorkflows(value);
        });
    }

    function workflowsList(inboxData) {
        var self = this;
        self.workflowsList = ko.observableArray(inboxData);
        var filterItems = [];
        for (let i in self.workflowsList()) {
            var workflow = self.workflowsList()[i];

            if (filterItems.find(function (item) { return item.text == workflow.TypeName; }) == undefined)
                filterItems.push(new filterItem(workflow.TypeName));

            addExtraAttributesToWorkflowRow(workflow);
        }

        initRightFilter(filterItems);
    }

    function addExtraAttributesToWorkflowRow(workflow) {
        workflow.IsSelected = ko.observable();
        workflow.IsSelected.subscribe(function () {
            onWorkflowRowSelectionChanged();
        });
        workflow.IsFilteredOut = ko.observable(false);
        workflow.InstrumentIdClickHandler = openInstrument;
        workflow.ActionHistoryClickHandler = actionHistoryClickHandler;
        var workflowAttributes = [];
        var isPrimaryAttrFound = false;
        for (let i in workflow.WorkflowAttributes) {
            var workflowAttribute = workflow.WorkflowAttributes[i];
            if (workflowAttribute.isPrimary && !isPrimaryAttrFound) {
                isPrimaryAttrFound = true;
                workflow.PrimaryIdentifierAttributeName = workflowAttribute.AttributeName;
                workflow.PrimaryIdentifierValue = workflowAttribute.AttributeValue;
            }
            else workflowAttributes.push(workflowAttribute);
        }
        if (!isPrimaryAttrFound) {
            workflow.PrimaryIdentifierAttributeName = "";
            workflow.PrimaryIdentifierValue = "";
        }
        workflow.WorkflowAttributes = workflowAttributes;
    }

    function inboxSectionsList(data) {
        var self = this;
        self.sectionsList = ko.observableArray([]);
        self.selectedSection = null;
        for (let i in data) {
            self.sectionsList.push(new inboxSection(i, data[i], self));
        }
    }

    function inboxSection(sectionId, sectionName, parent) {
        var self = this;
        self.sectionName = sectionName;
        self.sectionId = sectionId;
        self.isActive = ko.observable(false);
        self.parent = parent;
        self.clickHandler = function (inboxSection, event) {
            for (let i in inboxSection.parent.sectionsList())
                inboxSection.parent.sectionsList()[i].isActive(false);
            inboxSection.isActive(true);
        }
        self.isActive.subscribe(function (value) {
            if (value) {
                self.parent.selectedSection = self;
                getInboxData(self.sectionName);
            }
        });
    }

    function workflowActionsList() {
        var self = this;
        self.workflowActionsList = ko.observableArray([]);
    }

    function workflowAction(actionName, actualActionName) {
        var self = this;
        self.ActionName = actionName;
        self.ActualActionName = actualActionName;
        self.ActionRemarks = ko.observable("");
        self.PopupHidden = ko.observable(true);
        self.ActionError = ko.observable("");
        self.onActionBtnClick = function (action, event) {
            for (let i in gridHeaderVM.workflowActionsList())
                gridHeaderVM.workflowActionsList()[i].PopupHidden(true);
            action.PopupHidden(false);
            action.ActionError("");
            action.ActionRemarks("");
        }
        self.onPopupCancel = function (action, event) {
            action.PopupHidden(true);
        }
        self.onPopupAction = onWorkflowAction;
    }

    function messagePopup() {
        var self = this;
        self.messageType = ko.observable("");
        self.messageTitle = ko.observable("");
        self.messageContent = ko.observable("");
    }

    //VIEW MODELS - END

    //HANDLERS - START

    function onWorkflowRowSelectionChanged() {
        if (workflowGridVM == null)
            return;

        //Find the intersecting actions for all the selected workflow rows -->
        gridHeaderVM.workflowActionsList.removeAll();
        var selectedWorkflows = workflowGridVM.workflowsList().filter(function (w) { return w.IsSelected(); });
        var actionsList = [];
        var actionsToRemove = [];
        if (selectedWorkflows.length > 0)
            actionsList = selectedWorkflows[0].PossibleActions.slice();
        for (let i = 1; i < selectedWorkflows.length; i++) {
            let workflow = selectedWorkflows[i];

            for (let j = 0; j < actionsList.length; j++) {
                var matchingAction = workflow.PossibleActions[workflow.PossibleActions.indexOf(actionsList[j])];
                if (matchingAction == undefined)
                    actionsToRemove.push(actionsList[j]);
            }
        }

        if (selectedWorkflows.length > 0 && inboxSectionsVM != null && (inboxSectionsVM.selectedSection.sectionName == "Rejected Requests" || inboxSectionsVM.selectedSection.sectionName == "My Requests"))
            actionsList = ["Cancel"];

        if (actionsList.indexOf("Approve") >= 0) {
            actionsList.splice(actionsList.indexOf("Approve"), 1);
            if (actionsList.indexOf("Edit and Approve") == -1)
                actionsList.push("Approve");
        }
        if (actionsList.indexOf("Edit and Approve") >= 0) {
            actionsList.splice(actionsList.indexOf("Edit and Approve"), 1);
            if (actionsList.indexOf("Approve") == -1)
                actionsList.push("Edit and Approve");
        }

        for (let i in actionsList) {
            var action = actionsList[i];

            if (actionsToRemove.indexOf(action) == -1) {
                var actualActionName = action;
                if (action == "Edit and Approve")
                    action = "Approve";
                gridHeaderVM.workflowActionsList.push(new workflowAction(action, actualActionName));
            }
        }
    }

    function searchWorkflows(searchText) {
        for (let i in workflowGridVM.workflowsList()) {
            var workflow = workflowGridVM.workflowsList()[i];
            workflow.IsFilteredOut(true);

            if (selectedTypesFromFilter.indexOf(workflow.TypeName) == -1)
                continue;

            var isMatch = false;
            for (let j in gridSearchOnFields) {
                var field = gridSearchOnFields[j];
                if (workflow[field].toLowerCase().indexOf(searchText.toLowerCase()) >= 0) {
                    isMatch = true;
                    break;
                }
            }

            //for search in dates
            if (!isMatch && SRMWorkflowInboxDetails.ConvertDateTime(workflow.RequestedOn).indexOf(searchText.toLowerCase()) >= 0)
                isMatch = true;
            if (!isMatch && currentWorkflowType == "CREATE" && SRMWorkflowInboxDetails.ConvertDate(workflow.EffectiveStartDate).indexOf(searchText.toLowerCase()) >= 0)
                isMatch = true;

            if (!isMatch) {
                for (let j in workflow.WorkflowAttributes) {
                    var attribute = workflow.WorkflowAttributes[j];
                    if (attribute.AttributeValue.toLowerCase().indexOf(searchText.toLowerCase()) >= 0) {
                        isMatch = true;
                        break;
                    }
                }
            }

            if (isMatch)
                workflow.IsFilteredOut(false);
        }
    }

    function openInstrument(workflow, event, isFromUniquenessPopup, instrumentId, failureType) {
        if (isFromUniquenessPopup === undefined)
            isFromUniquenessPopup = false;
        var url = "";
        var params = {};
        var identifier = "";
        var tabText = "";
        var uniqueId = "unique_" + SecMasterJSCommon.SMSCommons.getGUID();
        switch (currentModule) {
            case 3: //Security Master
                url = "SMCreateUpdateSecurityNew.aspx";
                var pageTypeAppend = "";
                if (currentWorkflowType.toLowerCase() == "create") {
                    params.identifier = "CreateSecurityNew";
                    pageTypeAppend = "Draft";
                }
                else if (currentWorkflowType.toLowerCase() == "update") {
                    params.identifier = "UpdateSecurityNew";
                }
                switch (inboxSectionsVM.selectedSection.sectionName) {
                    case "My Requests":
                        params.pageType = "View" + pageTypeAppend;
                        break;
                    case "Pending At Me":
                        if (workflow.PossibleActions.indexOf("Edit and Approve") == -1)
                            params.pageType = "View" + pageTypeAppend;
                        else params.pageType = "Edit" + pageTypeAppend;
                        break;
                    case "Rejected Requests":
                        params.pageType = "Edit" + pageTypeAppend;
                        break;
                }
                params.secId = workflow.InstrumentId;
                params.UniqueId = uniqueId;
                params.ModuleId = currentModule;
                params.mode = "nw";
                params.isSecurityUnderWorkflow = "1";
                params.RADWorkflowInstanceId = workflow.RadInstanceId;
                params.btnPageRefreshId = "gridRefereshBtn";
                if (inboxSectionsVM != null && inboxSectionsVM.selectedSection.sectionName == "Rejected Requests")
                    params.IsWorkflowRetrigger = "1";
                if (inboxSectionsVM != null && inboxSectionsVM.selectedSection.sectionName == "My Requests")
                    params.IsMyWorkflowReq = "1";
                identifier = "viewSecurity";
                tabText = "View Security";
                break;
            case 6: //Reference Master
            case 18:    //Fund Master
            case 20:    //Party Master
                url = "App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx";
                if (!isFromUniquenessPopup) {
                    params.entityCode = workflow.InstrumentId;
                    params.entityTypeId = workflow.TypeID;
                    params.pageIdentifier = "WorkflowEntity";
                    params.wfSection = inboxSectionEnum[inboxSectionsVM.selectedSection.sectionName];
                    params.quid = workflow.QueueId;
                    params.wtype = currentWorkflowType;
                    params.effectiveDate = formatDate(workflow.EffectiveStartDate);
                    params.btnPageRefreshId = "gridRefereshBtn";
                }
                else {
                    params.entityCode = instrumentId;
                    params.pageIdentifier = "UpdateEntityFromSearch";
                    if (failureType == "In Workflow")
                        params.pageIdentifier = "WorkflowEntity";
                    else if (failureType == "Draft")
                        params.pageIdentifier = "DraftEntity";

                }

                params.ModuleID = currentModule;
                params.searchType = "";
                params.RMTabUniqueID = uniqueId;
                //params.myrejreq = (inboxSectionsVM != null && inboxSectionsVM.selectedSection.sectionName == "Rejected Requests") ? true : false;
                identifier = "viewEntity";
                tabText = "View Entity";
                break;
        }

        url = makeUrl(url, params);
        SecMasterJSCommon.SMSCommons.createTab(identifier, url, uniqueId, tabText, true);
    }

    function actionHistoryClickHandler(workflow, event) {
        var popupContainer = actionHistoryIFrame.parents(".workflow-action-history");
        var iframeContainer = actionHistoryIFrame.parents(".action-history-iframe-container");
        iframeContainer.height(0).width(0);
        popupContainer.addClass("hidden");

        SecMasterJSCommon.SMSCommons.onUpdating();
        actionHistoryIFrame.attr("src", "");
        var url = makeUrl(
            "WorkflowActionHistory.aspx",
            {
                moduleId: currentModule,
                radWorkflowInstanceId: workflow.RadInstanceId,
                btnPageRefreshId: "gridRefereshBtn"
            }
        );
        actionHistoryIFrame.attr("src", url);
    }

    function onActionHistoryIframeLoad(event) {
        if (actionHistoryIFrame.attr("src") == "")
            return;
        var iframeMaxHeight = 700;
        if (iframeMaxHeight > container[0].getBoundingClientRect().height)
            iframeMaxHeight = container[0].getBoundingClientRect().height;
        var popupContainer = actionHistoryIFrame.parents(".workflow-action-history");
        var iframeContainer = actionHistoryIFrame.parents(".action-history-iframe-container");
        var iframeContentRef = $("form", actionHistoryIFrame[0].contentWindow.document.body);
        popupContainer.removeClass("hidden");
        iframeContainer.height(0).width(0);

        var iframeContentHeight = iframeContentRef.height() + 30;
        var iframeContentWidth = iframeContentRef.width() + 20;
        if (iframeContentHeight > iframeMaxHeight)
            iframeContentHeight = iframeMaxHeight;
        iframeContainer.height(iframeContentHeight).width(iframeContentWidth);

        var containerHeight = container[0].getBoundingClientRect().height;
        var containerWidth = container[0].getBoundingClientRect().width;
        var popupContainerHeight = popupContainer[0].getBoundingClientRect().height;
        var popupContainerWidth = popupContainer[0].getBoundingClientRect().width;
        popupContainer.css('left', ((containerWidth - popupContainerWidth) / 2) + 'px');
        popupContainer.css('top', ((containerHeight - popupContainerHeight) / 2) + 'px');

        SecMasterJSCommon.SMSCommons.onUpdated();
    }

    function refreshClickHandler() {
        getInboxData(inboxSectionsVM.selectedSection.sectionName);
    }

    //HANDLERS - END

    function TypeNameByModuleId(moduleId) {
        if (moduleId == "3")
            return "Security Types";
        if (moduleId == "6")
            return "Entity Types";
        if (moduleId == "18")
            return "Funds";
        if (moduleId == "20")
            return "Parties";
        return "Types";
    }

    function makeUrl(url, parameters) {
        if (parameters == undefined)
            parameters = {};
        var queryParams = [];
        var keys = Object.keys(parameters)
        for (let i in keys) {
            queryParams.push(keys[i] + "=" + parameters[keys[i]]);
        }
        if (queryParams.length > 0)
            url += "?" + queryParams.join("&");

        return url;
    }

    //API Hits - START

    function getInboxData(inboxSectionId) {
        SecMasterJSCommon.SMSCommons.onUpdating();
        var params = {
            moduleId: currentModule,
            userName: currentUser,
            workflowType: currentWorkflowType,
            statusType: inboxSectionId.replace(new RegExp(' ', 'g'), '_').toUpperCase(),
            dateFormat: currentDateFormat
        };
        CallCommonServiceMethod(
            testBit ? "GetWorkflowInboxDataTest" : "GetWorkFlowQueueData",
            params,
            function (data) {
                if (workflowGridVM == null) {
                    workflowGridVM = new workflowsList(data.d);
                    ko.applyBindings(workflowGridVM, $(".inbox-grid", container)[0]);
                }
                else {
                    var filterItems = [];
                    for (let i in data.d) {
                        if (filterItems.find(function (item) { return item.text == data.d[i].TypeName; }) == undefined)
                            filterItems.push(new filterItem(data.d[i].TypeName));
                        addExtraAttributesToWorkflowRow(data.d[i]);
                    }

                    workflowGridVM.workflowsList(data.d);
                    initRightFilter(filterItems);
                }

                gridHeaderVM.SearchText("");
                if (gridHeaderVM.SelectAll() == true)
                    gridHeaderVM.SelectAll(false);
                adjustAttributeColumnsWidth();
                gridHeaderVM.workflowActionsList.removeAll();
                SecMasterJSCommon.SMSCommons.onUpdated();
            },
            failureCallback,
            null, false
        );
    }

    function onWorkflowAction(action, event) {
        var actionCheck = action.ActualActionName;
        if (action.ActionRemarks() == "") {
            if (actionCheck == "Reject" || actionCheck == "Delete" || actionCheck == "Suppress" || actionCheck == "Cancel") {
                let errorString = "* Remarks are mandatory in case of " + actionCheck + " action.";
                action.ActionError(errorString);
                return false;
            }
        }

        var actionInfos = [];
        if (workflowGridVM == null)
            return;

        var selectedWorkflows = workflowGridVM.workflowsList().filter(function (w) { return w.IsSelected(); });
        for (let i in selectedWorkflows) {
            var workflow = selectedWorkflows[i];
            var actionInfo = {
                RadInstanceID: workflow.RadInstanceId,
                InstrumentID: workflow.InstrumentId,
                ClonedFrom: workflow.ClonedFrom,
                CopyTimeSeries: workflow.CopyTimeSeries,
                DeleteExisting: workflow.DeleteExisting,
                EffectiveEndDate: workflow.EffectiveEndDate,
                EffectiveStartDate: workflow.EffectiveStartDate,
                RequestedBy: workflow.RequestedByReal,
                RequestedOn: workflow.RequestedOn,
                TypeID: workflow.TypeID,
                TypeName: workflow.TypeName,
                WorkflowInstanceID: workflow.WorkflowInstanceID,
                WorkflowType: WorkflowTypeEnum[currentWorkflowType],
                QueueID: workflow.QueueId
            };
            actionInfos.push(actionInfo);
        }

        var params = {
            actionInfo: actionInfos,
            actionName: action.ActualActionName,
            remarks: action.ActionRemarks(),
            userName: currentUser,
            moduleID: currentModule,
            performFinalApproval: true
        };
        SecMasterJSCommon.SMSCommons.onUpdating();
        CallCommonServiceMethod(
            testBit ? "TakeActionTest" : "TakeAction",
            params,
            function (data) {
                var errorMsg = "";
                var actionResponses = { true: [], false: [] };
                for (let i in data.d) {
                    var actionResponse = data.d[i];
                    actionResponses[actionResponse.isPassed].push(actionResponse);
                    if (actionResponse.isPassed == false)
                        errorMsg += actionResponse.ErrorMessage + "<br/>";
                }

                if (actionResponses[true].length > 0 && actionResponses[false].length == 0) {
                    var successMessage = "Selected requests ";
                    if (action.ActionName == "Approve")
                        successMessage += "approved";
                    else if (action.ActionName == "Reject")
                        successMessage += "rejected";
                    else if (action.ActionName == "Cancel")
                        successMessage += "cancelled";
                    else
                        successMessage += "processed";
                    action.PopupHidden(true)
                    $(".popup-failure-grid", container).addClass("hidden");
                    showMessagePopup("Success", successMessage, "success");
                    gridHeaderVM.workflowActionsList.removeAll();
                    SecMasterJSCommon.SMSCommons.onUpdated();
                }
                else if (actionResponses[false].length > 0) {
                    action.PopupHidden(true)
                    var viewKey = GetGUID();
                    var currPageID = GetGUID();
                    var sessionID = GetGUID();
                    var tempObj = $.extend({}, gridObj);
                    tempObj.GridId = "popupFailureGrid";
                    tempObj.UserId = currentUser;
                    tempObj.ViewKey = viewKey;
                    tempObj.CurrentPageId = currPageID;
                    tempObj.SessionIdentifier = sessionID;
                    tempObj.IdColumnName = "ID";
                    tempObj.ColumnsToHide = [];
                    tempObj.DateFormat = currentDateFormat;
                    tempObj.TableName = "WorkflowActionErrorInfo";
                    tempObj.Height = (container[0].getBoundingClientRect().height - 200) + "px";
                    var gridParams = {
                        username: currentUser,
                        divID: "popupFailureGrid",
                        currPageID: currPageID,
                        viewKey: viewKey,
                        sessionID: sessionID,
                        dateFormat: currentDateFormat,
                        ModuleId: currentModule,
                        jsonGridInfo: JSON.stringify(tempObj),
                        actionInfo: data.d
                    };
                    CallCommonServiceMethod("BindGridForActionInfo", gridParams, function () {
                        $(".popup-failure-grid", container).removeClass("hidden");
                        showMessagePopup("Failed", "Error occurred for some requests", "failed", 800);
                        xlgridloader.create("popupFailureGrid", "Test", tempObj, "");
                        action.ActionError(errorMsg);
                        gridHeaderVM.workflowActionsList.removeAll();
                        SecMasterJSCommon.SMSCommons.onUpdated();
                        $(".grid-error-link", container).on('click', showUniquenessInfoPopup);
                    }, failureCallback, null, false);
                }
            },
            failureCallback,
            null, false
        );
    }

    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }

    function failureCallback(jqXHR, textStatus, errorThrown) {
        SecMasterJSCommon.SMSCommons.onUpdated();
        console.log("Workflow Inbox : " + textStatus + " - " + errorThrown);
    }

    function showUniquenessInfoPopup(event) {
        var popup = $(".uniqueness-info-popup", container);
        var popupParent = popup.parent();
        popup.removeClass("hidden");
        var infoStr = $(event.target).attr("data-uniqueness-info");
        var infoObj = JSON.parse(infoStr);
        var basePath = path.substr(0, path.indexOf("CommonUI"));
        UniquenessCheckApp.init(
            infoObj,
            function (instrumentId, failureType) {
                if (currentModule == 3)
                    SecMasterJSCommon.SMSCommons.openSecurity(true, instrumentId, '', true, true, false, '', '', false, false, SecMasterJSCommon.SMCreateUpdateOpenByDefault.None, 0, null, currentModule);
                else
                    openInstrument(null, null, true, instrumentId, failureType)
            },
            closeUniquenessInfoPopup,
            currentModule,
            "uniquenessInfoPopup",
            basePath
        );

        popup.css('left', (popupParent.width() - popup.width()) / 2);
        popup.css('top', (popupParent.height() - popup.height()) / 2);

        //$(">div", popup).width("100%");
    }

    function closeUniquenessInfoPopup() {
        $(".uniqueness-info-popup", container).addClass("hidden");
    }

    //API Hits - END

    function initRightFilter(items) {
        var objFilterData = {};
        objFilterData.pivotElementId = "filterPivot";
        objFilterData.id = "rightFilterDiv";
        objFilterData.data = [{
            identity: "TypeFilter",
            isSearchable: true,
            listItems: items,
            maxHeight: 300,
            sectionHeader: TypeNameByModuleId(currentModule),
            sectionType: "checkbox",
            selectAllText: "Select All",
            state: "down"
        }];
        smslidemenu.init(
            objFilterData,
            function () {
                var data = smslidemenu.getAllData("rightFilterDiv");
                if (data != null && data != undefined) {
                    selectedTypesFromFilter = data.TypeFilter.SelectedValue.split(',');
                    searchWorkflows(gridHeaderVM.SearchText());
                }
                smslidemenu.hide("rightFilterDiv");
            },
            null
        );

        var data = smslidemenu.getAllData("rightFilterDiv");
        if (data != null && data != undefined) {
            selectedTypesFromFilter = data.TypeFilter.SelectedValue.split(',');
        }
        smslidemenu.hide("rightFilterDiv");
    }

    function filterItem(itemName) {
        return {
            text: itemName,
            value: itemName,
            isSelected: "true"
        }
    }

    function showMessagePopup(messageTitle, message, messageType, isErrorPopup) {
        messagePopupVM.messageType(messageType);
        messagePopupVM.messageTitle(messageTitle);
        messagePopupVM.messageContent(message);
        $(".message-popup-container", container).removeClass("hidden");
        var messagepopup = $(".message-popup", container);
        var popup = $(".message-popup-container", container);
        if (!isErrorPopup) {
            messagepopup.width(400);
            messagepopup.height('initial');
            var width = container.width();
            var height = container.height();
            var popupWidth = popup.width();
            var popupHeight = popup.height();
            popup.css('left', ((width - popupWidth) / 2) + 'px');
            popup.css('top', ((height - popupHeight) / 2) + 'px');
        }
        else {
            messagepopup.width(container[0].getBoundingClientRect().width - 10);
            messagepopup.height(container[0].getBoundingClientRect().height - 10);
            popup.css('left', '0px');
            popup.css('top', '0px');
        }
    }

    function adjustAttributeColumnsWidth() {
        var gridRow = $(".inbox-grid .inbox-grid-row", container)[0];
        if (gridRow != undefined) {
            var gridRowWidth = gridRow.getBoundingClientRect().width;
            var rowCheckboxWidth = $(".row-checkbox", gridRow)[0].getBoundingClientRect().width;
            var infoPanelWidth = $(".info-panel", gridRow)[0].getBoundingClientRect().width;
            var attributeInfoPanelWidth = gridRowWidth - (rowCheckboxWidth + infoPanelWidth);
            var attributeCellWidth = attributeInfoPanelWidth / 4;

            $(".inbox-grid .attribute-info-panel .attribute", container).width(attributeCellWidth - 16);
        }
    }

    function formatDate(date) {
        if (date == null || date == undefined || date == "")
            return "";
        var dateObj = eval("new " + date.replace(new RegExp("/", "g"), ''));
        var year = dateObj.getFullYear() + "";
        var month = dateObj.getMonth() + 1;
        var d = dateObj.getDate();
        if (month < 10)
            month = "0" + month;
        else month = month + "";
        if (d < 10)
            d = "0" + d;
        else d = d + "";
        return year + month + d;
    }

    function windowPostMessageHandler(event) {
        var resizeActionHistoryIFrame = onActionHistoryIframeLoad;
        var refreshGridCallback = refreshClickHandler;
        try {
            eval(event.data.functionName + "();");
        }
        catch (e)
        { }
    }

    function dateConvertHelper(date, flag) {
        if (date != null && (currentWorkflowType == "CREATE" || flag == true)) {
            var dateObj = eval("new " + date.replace(new RegExp("/", "g"), ''));
            return dateObj;
        }
        else if (date == null && currentWorkflowType == "CREATE")
            return "Today";
        return "N/A";
    }

    return {
        Initializer: function (workflowType, inboxType, moduleId, username, dateFormat, refreshWorkflowInboxFirstPage) {
            isDetailsPopupOpen = true;
            container = $("#SRMWorkflowInboxDetails");
            $(container).removeClass("hidden");

            $(".grid-filter-buttons").removeClass("hidden");

            actionHistoryIFrame = $(".workflow-action-history .action-history-iframe", container);
            window.addEventListener('message', windowPostMessageHandler);

            $(".inbox-header", container).text(workflowType.toLowerCase() + " Workflow Requests");

            currentUser = username;
            currentModule = moduleId;
            currentWorkflowType = workflowType;
            currentDateFormat = dateFormat;

            if (inboxSectionsVM == null) {
                inboxSectionsVM = new inboxSectionsList(inboxSections);
                ko.applyBindings(inboxSectionsVM, $(".inbox-sections", container)[0]);
            }
            var selectedInboxSection = inboxSectionsVM.sectionsList().find(function (sec) { return sec.sectionName == inboxType; });
            selectedInboxSection.isActive(true);

            if (gridHeaderVM == null) {
                gridHeaderVM = new gridHeader();
                ko.applyBindings(gridHeaderVM, $(".grid-header-row", container)[0]);
            }

            if (messagePopupVM == null) {
                messagePopupVM = new messagePopup();
                ko.applyBindings(messagePopupVM, $(".message-popup-container", container)[0]);
            }

            refreshCallback = refreshWorkflowInboxFirstPage;
        },
        ChangeModule: function (moduleId) {
            if (!isDetailsPopupOpen)
                return;
            currentModule = moduleId;
            refreshClickHandler();
        },
        ConvertDate: function (date, flag) {
            var dateObj = dateConvertHelper(date, flag);
            if (dateObj.toLocaleDateString == undefined)    //Duck Typing check for Date object
                return dateObj;
            //return dateObj.toLocaleDateString();
            return dateObj.format(srmWorkFlowInbox._dateFormat);
        },
        ConvertDateTime: function (date, flag) {
            var dateObj = dateConvertHelper(date, flag);
            if (dateObj.toLocaleString == undefined)    //Duck Typing check for Date object
                return dateObj;
            //return dateObj.toLocaleString()
            return dateObj.format(srmWorkFlowInbox._dateTimeFormat);
        },
        CloseDetailsGrid: function () {
            if (workflowGridVM != null)
                workflowGridVM.workflowsList.removeAll();
            if (inboxSectionsVM != null) {
                for (let i in inboxSectionsVM.sectionsList())
                    inboxSectionsVM.sectionsList()[i].isActive(false);
            }
            if (gridHeaderVM != null) {
                gridHeaderVM.workflowActionsList.removeAll();
            }
            $(container).addClass("hidden");
            actionHistoryIFrame.parents(".workflow-action-history").addClass("hidden");
            $(".grid-filter-buttons").addClass("hidden");
            isDetailsPopupOpen = false;
            if (refreshCallback != null)
                refreshCallback();
        },
        CloseActionHistory: function () {
            var iframeContainer = actionHistoryIFrame.parents(".workflow-action-history");
            iframeContainer.addClass("hidden");
        },
        CloseMessagePopup: function () {
            $(".message-popup-container", container).addClass("hidden");
            refreshClickHandler();
        },
        ShowUniquenessInfoPopup: showUniquenessInfoPopup,
        CloseUniquenessInfoPopup: closeUniquenessInfoPopup,
        RefreshClickHandler: refreshClickHandler
    }
}();

var testBit = false;