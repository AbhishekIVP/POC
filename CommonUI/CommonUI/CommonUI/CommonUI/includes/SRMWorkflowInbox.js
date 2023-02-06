var srmWorkFlowInbox = (function () {

    function SRMWorkFlowInbox() {

        this._controlIdInfo = null;
        this._securityInfo = null;
        this._pageViewModelInstance = null;
        this._moduleID_moduleName_map = [];
        this._moduleID = 0;
        this._isBindingsApplied = false;
        this._username = "";
        this._dateFormat = "";
        this._dateTimeFormat = "";
    }
    
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    var srmWorkFlowInbox = new SRMWorkFlowInbox();


    /////////////////
    // VIEW MODELS //
    /////////////////
    //function listViewModel(data) {
    //    var self = this;
    //    self.actionName = data.actionName;
    //    self.actionCount = data.actionCount;
    //};

    function pageViewModel(data) {
        var self = this;
        self.subModuleList = ko.observableArray([]);
        for (let i in data) {
            self.subModuleList.push(new chainViewModel(data[i]));
        }
    };

    function chainViewModel(data) {
        var self = this;
        self.workflowType = data.workflowType;
        self.pendingCount = data.pendingCount;// > 9 ? data.pendingCount : '0' + data.pendingCount;
        self.myRequestsCount = data.myRequestsCount;// > 9 ? data.myRequestsCount : '0' + data.myRequestsCount;
        self.rejectedCount = data.rejectedCount;// > 9 ? data.rejectedCount : '0' + data.rejectedCount;
        //self.actionTypeList = ko.observableArray([]);
        //for (let i in data.srmWorkflowInboxActionType) {
        //    self.actionTypeList.push(new listViewModel(data.srmWorkflowInboxActionType[i]));
        //}
        self.onClickCount = function (vm, event) {
            var ele = $(event.target);
            if (parseInt(ele.text()) == 0)
                return false;
            var workflowType = ele.attr('clickedSubType');
            var inboxType = ele.attr('clickedActionType');
            var moduleId = srmWorkFlowInbox._moduleID;
            SRMWorkflowInboxDetails.Initializer(workflowType, inboxType, moduleId, srmWorkFlowInbox._username, srmWorkFlowInbox._dateFormat, refreshWorkflowInbox);
        }
    };


    //Init functions
    function init(username) {
        srmWorkFlowInbox._username = username;

        var listofTabsToBindFunctionsWith = CommonModuleTabs.ModuleList();

        for (i in listofTabsToBindFunctionsWith) {
            let item = listofTabsToBindFunctionsWith[i];
            srmWorkFlowInbox._moduleID_moduleName_map[item.displayName.toLowerCase()] = item.moduleId;
            switch (item.displayName.toLowerCase().trim()) {
                case "securities":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initSecMaster });
                    break;
                case "refdata":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initRefMaster });
                    break;
                case "funds":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initFundMaster });
                    break;
                case "parties":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initPartyMaster });
                    break;
            }
        }
    };

    function initSecMaster() {
        srmWorkFlowInbox._moduleID = srmWorkFlowInbox._moduleID_moduleName_map["securities"];
        getData();
        SRMWorkflowInboxDetails.ChangeModule(srmWorkFlowInbox._moduleID);
    }

    function initRefMaster() {
        srmWorkFlowInbox._moduleID = srmWorkFlowInbox._moduleID_moduleName_map["refdata"];
        getData();
        SRMWorkflowInboxDetails.ChangeModule(srmWorkFlowInbox._moduleID);
    }

    function initFundMaster() {
        srmWorkFlowInbox._moduleID = srmWorkFlowInbox._moduleID_moduleName_map["funds"];
        getData();
        SRMWorkflowInboxDetails.ChangeModule(srmWorkFlowInbox._moduleID);
    }

    function initPartyMaster() {
        srmWorkFlowInbox._moduleID = srmWorkFlowInbox._moduleID_moduleName_map["parties"];
        getData();
        SRMWorkflowInboxDetails.ChangeModule(srmWorkFlowInbox._moduleID);
    }

    function refreshWorkflowInbox() {
        CommonModuleTabs.callbackObj[CommonModuleTabs.currentSelected]();
    }

    function getData() {
        var params = {};
        params.moduleId = srmWorkFlowInbox._moduleID;
        params.userName = srmWorkFlowInbox._username;
        CallCommonServiceMethod('GetWorkFlowInboxData', params, OnSuccess_GetWorkFlowInboxData, function () { SecMasterJSCommon.SMSCommons.onUpdated(); }, null, false);
    }

    function setScreenSize() {
        let height = $(window).height() - 10;
        $("#SRMWorkflowInboxParent").css('display', height);
        $("#SRMWorkflowInboxGrid").css('margin-top', (height - $("#SRMWorkflowInboxGrid").height() - 50) / 2);
    }

    function applyColors() {
        $(".SRMWorkflowInboxContentRow .SRMWorkflowInboxContentItem")[0].css
    }


    //////////////////////////////////
    // Call Common Service Functions //
    //////////////////////////////////
    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        SecMasterJSCommon.SMSCommons.onUpdating();
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }


    //////////////////////////////////
    // On Success Service Functions //
    //////////////////////////////////
    function OnSuccess_GetWorkFlowInboxData(data) {
        SecMasterJSCommon.SMSCommons.onUpdated();
        //console.log(data.d);
        if (!srmWorkFlowInbox._isBindingsApplied) {
            srmWorkFlowInbox._pageViewModelInstance = new pageViewModel(data.d);
            srmWorkFlowInbox._isBindingsApplied = true;
            ko.applyBindings(srmWorkFlowInbox._pageViewModelInstance, document.getElementById("SRMWorkflowInboxParent"));
        }
        else {
            srmWorkFlowInbox._pageViewModelInstance.subModuleList.removeAll();
            var self = srmWorkFlowInbox._pageViewModelInstance.subModuleList;
            //TODO
            for (var i in data.d) {
                let currData = data.d[i];
                self.push(new chainViewModel(currData));
            }
        }
        setScreenSize();

    }

    //prototypes
    SRMWorkFlowInbox.prototype.Initializer = function Initializer(username, dateFormat,dateTimeFormat) {
        init(username);
        srmWorkFlowInbox._dateFormat = dateFormat;
        srmWorkFlowInbox._dateTimeFormat = dateTimeFormat;
    }

    return srmWorkFlowInbox;
})();