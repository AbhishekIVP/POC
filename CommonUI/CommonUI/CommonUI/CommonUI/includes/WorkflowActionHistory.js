var workflowActionHistory = (function () {    
    
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');


    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    function WorkflowActionHistory() {
        this._controlIdInfo = null;
        this._securityInfo = null;
        this._instancesPageViewModelInstance = null;
        
        //To Store if KO has been applied.
        this._koBindingAppliedActions = false;

        //To Store if first pending encountered. Until encountered, it stays null. At 1st Pending set to true. Then false.
        this._firstPending = null;
    }

    var workflowActionHistory = new WorkflowActionHistory();


    /////////////////
    // VIEW MODELS //
    /////////////////

    // Instances Page View Model
    function instancesPageViewModel(data) {
        var self = this;

        self.instancesListing = ko.observableArray();

        self.isSec = workflowActionHistory._securityInfo.moduleId == 3 ? true : false;

        if (typeof (data) != 'undefined') {

            if (typeof data == 'undefined' || data.length === 0) {
                //No Instances on Instrument
                //$("#WFAH_InstancesErrorDiv").show();
                if (typeof (window.parent.parent.leftMenu) !== "undefined") {
                    
                    $("#WFAH_InstancesParentDiv").hide();

                    var instrumentString = 'Instrument';
                    if (workflowActionHistory._securityInfo.moduleId == 3) {
                        instrumentString = 'Security';
                    }

                    window.parent.parent.leftMenu.showNoRecordsMsg("No Workflow has ever been raised on this " + instrumentString, $("#WFAH_InstancesErrorDiv"));

                    //CSS for smNoRecordsContainer div created inside UniquenessSetupMainErrorDiv div
                    var pd_left = ($("#WFAH_InstancesErrorDiv").width() - $("#smNoRecordsContainer").width() - 2) / 2 - 40;
                    var pd_top = (($("#WFAH_InstancesErrorDiv").height() - $("#smNoRecordsContainer").height() - 2) / 2) - 20;
                    $("#WFAH_InstancesErrorDiv #smNoRecordsContainer").css({ "padding-left": pd_left, "padding-top": pd_top });
                }
            }
            else {
                //$("#WFAH_InstancesErrorDiv").hide();
                $("#WFAH_InstancesParentDiv").show();
                if (typeof (window.parent.parent.leftMenu) !== "undefined") {
                    window.parent.parent.leftMenu.hideNoRecordsMsg();
                }

                for (var item in data) {
                    self.instancesListing.unshift(new instanceChainViewModel(data[item]));
                }
            }
        }
    };

    // Instance Chain View Model
    function instanceChainViewModel(data) {
        var self = this;

        self.actionListing = ko.observableArray();

        self.instanceText = "Initiated by " + (typeof data.RequestedBy != 'undefined' ? data.RequestedBy : "") + " on " + (typeof data.RequestedOn != 'undefined' ? data.RequestedOn : "");

        self.instanceID = typeof data.InstanceID != 'undefined' ? data.InstanceID : -1;

        self.toShowInstance = ko.observable(false);

        if (typeof (data) != 'undefined' && data.Actions.length > 0) {
            for (var item in data.Actions)
                self.actionListing.unshift(new actionViewModel(data.Actions[item]));
        }
    };

    // Action View Model
    function actionViewModel(data) {
        var self = this;

        self.actionStatus = typeof data.Status != 'undefined' ? data.Status : "";
        
        //self.actionByName = data.ActionByName;
        self.actionByName = typeof data.UserName != 'undefined' ? data.UserName : "";
        //self.actionRemarks = data.ActionRemarks;
        self.actionRemarks = typeof data.Remarks != 'undefined' ? data.Remarks : "";
        //self.actionWorkflowLevel = data.ActionWorkflowLevel;
        self.actionWorkflowLevel = typeof data.LevelName != 'undefined' ? data.LevelName : "";
        //self.actionDateDisplayText = data.ActionDateDisplayText;
        self.actionDateDisplayText = typeof data.ActionDate != 'undefined' ? data.ActionDate : "";
        //self.actionTimeDisplayText = data.ActionTimeDisplayText;
        self.actionTimeDisplayText = typeof data.ActionTime != 'undefined' ? data.ActionTime : "";

        //To Show
        self.toShow = ko.observable(false);
        
        //Action 
        self.actionClickable = "auto";

        //Attributes List
        self.attrListing = ko.observableArray();
        
        if (typeof data.Attributes != 'undefined' && data.Attributes.length > 0) {
            for (var item in data.Attributes) {
                self.attrListing.push(new attributesViewModel(data.Attributes[item]));
            }
            self.actionClickable = "pointer";
        }

        self.actionPendingLevel = 0;
        self.actionPendingText = "";

        if (self.actionStatus.toLowerCase().trim() == 'pending') {

            //Pending at Text
            self.actionPendingText = "Pending at " + (typeof data.LevelName != 'undefined' ? data.LevelName : "");

            if (workflowActionHistory._firstPending == null) {
                self.actionPendingLevel = 1;
                workflowActionHistory._firstPending = true;
            }
            else {
                self.actionPendingLevel = 2;
            }
        }

        //Color
        self.actionColor = ko.computed(function () {
            if (self.actionStatus.toLowerCase().trim() == 'approved')
                return '#b4cca1';
            else if (self.actionStatus.toLowerCase().trim() == 'rejected')
                return '#ea8b8b';
            else if (self.actionStatus.toLowerCase().trim() == 'initiated')
                return '#bdbaba';
            else if (self.actionStatus.toLowerCase().trim() == 'pending') {
                if (self.actionPendingLevel == 1)
                    return '#E0C066';
                else
                    return '#717171';
            }
            else if (self.actionStatus.toLowerCase().trim() == 'canceled')
                return '#696969';
        });

        self.actionBorderTop = '3px solid ' + self.actionColor();

        //INCOMPLETE
        self.actionImageClass = ko.computed(function () {
            if (self.actionStatus.toLowerCase().trim() == 'approved')
                return 'WFAH_ImageCSSClass fa fa-check';
            else if (self.actionStatus.toLowerCase().trim() == 'rejected')
                return 'WFAH_ImageCSSClass fa fa-times';
            else if (self.actionStatus.toLowerCase().trim() == 'initiated')
                return 'WFAH_ImageCSSClass WFAH_ImageBackgroundInitiated';
            else if (self.actionStatus.toLowerCase().trim() == 'pending') 
                return 'WFAH_ImageCSSClass WFAH_ImageBackgroundPending';
            else if (self.actionStatus.toLowerCase().trim() == 'canceled')
                return 'WFAH_ImageCSSClass WFAH_ImageBackgroundCanceled';
            else
                return 'WFAH_ImageCSSClass ';
        });
    }
    
    // Attributes View Model
    function attributesViewModel(data) {
        var self = this;
        
        self.attributeName = typeof data.AttributeName != 'undefined' ? data.AttributeName : "";            // Common - 1
        self.oldValue = typeof data.OldValue != 'undefined' ? data.OldValue : "";                           // Common - 5
        self.newValue = typeof data.NewValue != 'undefined' ? data.NewValue : "";                           // Common - 4
        self.knowledgeDate = typeof data.KnowledgeDate != 'undefined' ? data.KnowledgeDate : "";            // Common - 6
        self.typeName = typeof data.TypeName != 'undefined' ? data.TypeName: "";                            // Common - 2
        self.legID = typeof data.LegID != 'undefined' ? data.LegID : "";                                    // Sec    - 3
        self.primaryAttribute = typeof data.PrimaryAttribute != 'undefined' ? data.PrimaryAttribute: "";    // Ref    - 3
        self.userName = typeof data.UserName != 'undefined' ? data.UserName: "";                            // Common - 7
    }

    /////////////////////////////////////////////////
    // Callback Functions for COMMON SERVICE Calls //
    /////////////////////////////////////////////////

    // Executed after successful retrieval of action history data for a Workflow Request
    function OnSuccess_GetActionHistoryData(result) {
        debugger;
        if (typeof result.d != 'undefined') {

            //var internalObj = JSON.parse(result.d);
            var internalObj = result.d;

            if (!workflowActionHistory._koBindingAppliedActions) {
                //Called on Load
                if (typeof ko !== 'undefined') {
                    workflowActionHistory._instancesPageViewModelInstance = new instancesPageViewModel(internalObj);

                    ko.applyBindings(workflowActionHistory._instancesPageViewModelInstance, $("#WFAH_InstancesParentDiv")[0]);
                    
                    //Click Top Instance
                    if (typeof internalObj!= 'undefined' && internalObj.length > 0) {
                        //Trigger Click on Top Instance
                        $("#WFAH_InstancesParentDiv .WFAH_InstanceTextParentDiv")[0].click();
                    }
                }

                workflowActionHistory._koBindingAppliedActions = true;

                if (workflowActionHistory._securityInfo.moduleId == 3 && workflowActionHistory._securityInfo.instrumentId != "@$@") {
                    window.parent.onServiceUpdated();
                }
            }
            else {
                
                workflowActionHistory._instancesPageViewModelInstance.instancesListing.removeAll();
                workflowActionHistory._firstPending = null;

                if (typeof internalObj == 'undefined' || internalObj.length == 0) {
                    //No Action on Workflow Request               
                    //$("#WFAH_InstancesErrorDiv").show();
                    if (typeof (window.parent.parent.leftMenu) !== "undefined") {

                        $("#WFAH_InstancesParentDiv").hide();

                        var instrumentString = 'Instrument';
                        if (workflowActionHistory._securityInfo.moduleId == 3) {
                            instrumentString = 'Security';
                        }

                        window.parent.parent.leftMenu.showNoRecordsMsg("No Workflow has ever been raised on this " + instrumentString, $("#WFAH_InstancesErrorDiv"));

                        //CSS for smNoRecordsContainer div created inside UniquenessSetupMainErrorDiv div
                        var pd_left = ($("#WFAH_InstancesErrorDiv").width() - $("#smNoRecordsContainer").width() - 2) / 2;
                        var pd_top = (($("#WFAH_InstancesErrorDiv").height() - $("#smNoRecordsContainer").height() - 2) / 2) - 20;
                        $("#WFAH_InstancesErrorDiv #smNoRecordsContainer").css({ "padding-left": pd_left, "padding-top": pd_top });
                    }
                }
                else {
                    //$("#WFAH_InstancesErrorDiv").hide();
                    
                    $("#WFAH_InstancesParentDiv").show();
                    if (typeof (window.parent.parent.leftMenu) !== "undefined") {
                        window.parent.parent.leftMenu.hideNoRecordsMsg();
                    }

                    for (var item in internalObj) {
                        workflowActionHistory._instancesPageViewModelInstance.instancesListing.unshift(new instanceChainViewModel(internalObj[item]));
                    }

                    //Trigger Click on Top Instance
                    $("#WFAH_InstancesParentDiv .WFAH_InstanceTextParentDiv")[0].click();
                }

                if (workflowActionHistory._securityInfo.moduleId == 3 && workflowActionHistory._securityInfo.instrumentId != "@$@") {
                    window.parent.onServiceUpdated();
                }
            }

            //Show Action History Pop-up
            $("#WFAH_MainBody").show();

            if (typeof window.parent != 'undefined') {
                window.parent.postMessage({ "functionName": "resizeActionHistoryIFrame" }, "*");
            }

        }
    }
    
    function OnFailure(result) {
        console.log("Service returned an error" + ((result.responseJSON == null || result.responseJSON.Message == null || result.responseJSON.Message.length == 0) ? "" : (" : " + result.responseJSON.Message)) + ((result.responseText == null || result.responseText.length == 0) ? "" : (" : " + result.responseText)));
        //$("#" + apimonitoring._controlIdInfo.LblErrorId).text("Service returned an error" + ((result.responseJSON == null || result.responseJSON.Message == null || result.responseJSON.Message.length == 0) ? "" : (" : " + result.responseJSON.Message)) + ((result.responseText == null || result.responseText.length == 0) ? "" : (" : " + result.responseText)));
        //$find(apimonitoring._controlIdInfo.ModalErrorId).show();
    }


    /////////////////////////////////
    // Call Common Service Methods //
    /////////////////////////////////
    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }

    //Function to Get Data when screen is opened from Workflow Inbox
    function getWFAHDataOnWorkflowInbox() {
        var params = {};
        params.instanceID = workflowActionHistory._securityInfo.radWorkflowInstanceId;
        params.moduleID = workflowActionHistory._securityInfo.moduleId;
        params.userName = workflowActionHistory._securityInfo.userName;
        params.dateFormat = com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate);
        params.dateTimeFormat = com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate);
        params.timeFormat = com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longTime);
        CallCommonServiceMethod('GetWorkflowActionHistoryByInstance', params, OnSuccess_GetActionHistoryData, OnFailure, null, false);
    }
    
    //Function to Get Data when screen is opened from Instrument C\U
    function getWFAHDataForInstrument() {
        var params = {};
        //Uncomment this line
        params.instrumentID = workflowActionHistory._securityInfo.instrumentId;
        params.dateFormat = com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate);
        params.userName = workflowActionHistory._securityInfo.userName;
        params.moduleID = workflowActionHistory._securityInfo.moduleId;
        params.dateTimeFormat = com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate);
        params.timeFormat = com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longTime);

        //Comment Both lines below
        //params.radWorkflowInstanceId = 1;
        //CallCommonServiceMethod('GetWorkflowActionHistory', params, OnSuccess_GetActionHistoryData, OnFailure, null, false);
        
        //Uncomment this line
        CallCommonServiceMethod('GetWorkflowActionHistoryByInstrument', params, OnSuccess_GetActionHistoryData, OnFailure, null, false);

        if (workflowActionHistory._securityInfo.moduleId == 3 && workflowActionHistory._securityInfo.instrumentId != "@$@") {
            window.parent.onServiceUpdating();
        }

    }

    ////////////////////
    // Event Handlers //
    ////////////////////

    //Function for onClick of Action to show Action Details
    WorkflowActionHistory.prototype.onClickAction = function onClickAction(obj, event) {
        
        if (obj.toShow() == false) {
            //Set toShow for all Actions to false
            //var data = workflowActionHistory._instancesPageViewModelInstance.actionListing();
            //for (var item in data) {
            //    data[item].toShow(false);
            //}
            if (obj.actionClickable == "pointer") {
                //Set toShow of current Object to True
                obj.toShow(true);
            }
        }
        else {
            obj.toShow(false);
        }

        if (typeof window.parent != 'undefined') {
            window.parent.postMessage({ "functionName": "resizeActionHistoryIFrame"}, "*");
        }
    }

    //Function for onClick of Instance to show Instance Action Details
    WorkflowActionHistory.prototype.onClickInstance = function onClickInstance(obj, event) {

        if (obj.toShowInstance() == false) {
            //Set toShow for all Actions to false
            var data = workflowActionHistory._instancesPageViewModelInstance.instancesListing();
            for (var item in data) {
                data[item].toShowInstance(false);
            }

            obj.toShowInstance(true);

            for (var item in obj.actionListing()) {
                obj.actionListing()[item].toShow(false);
            }
        }
        else {
            obj.toShowInstance(false);
        }

        if (typeof window.parent != 'undefined') {
            window.parent.postMessage({ "functionName": "resizeActionHistoryIFrame"}, "*");
        }
    }


    ////Close Button
    //$("#WFAH_TopCloseButton").click(function () {
    //    //Hide Action History Pop-up
    //    $("#WFAH_MainBody").hide();
    //});

    
    //////////
    // init //
    //////////
    function init() {
        setParentSize();
        if (workflowActionHistory._securityInfo.radWorkflowInstanceId > 0 && workflowActionHistory._securityInfo.moduleId > 0) {
            getWFAHDataOnWorkflowInbox();
        }
        else if (workflowActionHistory._securityInfo.moduleId > 0 && workflowActionHistory._securityInfo.instrumentId != "@$@") {
            getWFAHDataForInstrument();
        }
        else {
            console.log("WorkflowActionHistory - Query String Parameters are Incorrect.")
        }
    }


    //////////////////////////
    // Initializer Function //
    //////////////////////////
    WorkflowActionHistory.prototype.Initializer = function initializer(securityInfo) {
        workflowActionHistory._securityInfo = eval("(" + securityInfo + ")");
        init();
    }

    //Apply CSS
    function setParentSize() {
        //$("#WFAH_MainBody").css('max-width', $(window).width() - 10).css('max-height', $(window).height() - 10);
        $("#WFAH_MainBody").css('max-width', $(window).width() - 10);
        //$("#WFAH_MainDiv").css({ 'width': $(window).width() * 0.96, 'height': $(window).height() - 10, 'margin-left': $(window).width() * 0.02 });
        //$("#WFAH_InstancesErrorDiv").width($("#WFAH_MainDiv").width() - 2);
        //$("#WFAH_InstancesErrorDiv").height($("#WFAH_MainDiv").height() - 2);
    }

    ////TEST - Start
    //$("#WF_AH_TempClickDiv").click(function () {
        //var result = {};
        //var arr = [];
        //var item0 = {};
        //item0.ActionStatus = "Rejected";
        //item0.ActionByName = "UserO";
        //item0.ActionRemarks = "blah blah";
        //item0.ActionWorkflowLevel = "01";
        //item0.ActionDateDisplayText = "05/02/2017";
        //item0.ActionTimeDisplayText = "06:02:59";
        //item0.AttributesList = [];
        //var ab1 = {};
        //ab1.AttributeName = "Attr1";
        //ab1.OldValue = "old1";
        //ab1.NewValue = "new0";
        //item0.AttributesList.push(ab1);
        //var ab2 = {};
        //ab2.AttributeName = "Attr2";
        //ab2.OldValue = "old2";
        //ab2.NewValue = "new2";
        //item0.AttributesList.push(ab2);
        //arr.push(item0);
        //var item1 = {};
        //item1.ActionStatus = "Approved";
        //item1.ActionByName = "UserA";
        //item1.ActionRemarks = "Ok";
        //item1.ActionWorkflowLevel = "01";
        //item1.ActionDateDisplayText = "06/02/2017";
        //item1.ActionTimeDisplayText = "06:02:59";
        //item1.AttributesList = [];
        //var ab1 = {};
        //ab1.AttributeName = "Attr1";
        //ab1.OldValue = "old1";
        //ab1.NewValue = "new1";
        //item1.AttributesList.push(ab1);
        //var ab2 = {};
        //ab2.AttributeName = "Attr2";
        //ab2.OldValue = "old2";
        //ab2.NewValue = "new2";
        //item1.AttributesList.push(ab2);        
        //arr.push(item1);
        //var item2 = {};
        //item2.ActionStatus = "Pending";
        //item2.ActionPendingAtName = "UserB";
        //item2.ActionRemarks = "Ok";
        //item2.ActionWorkflowLevel = "02";
        //item2.ActionDateDisplayText = "06/02/2017";
        //item2.ActionTimeDisplayText = "06:02:59";
        //item2.AttributesList = [];
        //arr.push(item2);
        //var item3 = {};
        //item3.ActionStatus = "Pending";
        //item3.ActionPendingAtName = "UserC";
        //item3.ActionRemarks = "Ok";
        //item3.ActionWorkflowLevel = "02";
        //item3.ActionDateDisplayText = "06/02/2017";
        //item3.ActionTimeDisplayText = "06:02:59";
        //item3.AttributesList = [];
        //arr.push(item3);
        //result.d = arr;
    //    workflowActionHistory.OnSuccess_GetActionHistoryData(result);
    //});
    ////TEST - End

    return workflowActionHistory;
})();
