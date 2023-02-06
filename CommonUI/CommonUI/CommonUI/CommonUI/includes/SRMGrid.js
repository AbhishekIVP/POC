var srmGrid = (function () {

    function SRMGrid() {
        this._controlIdInfo = null;
        this._securityInfo = null;
        this._pageViewModelInstance = null;
        this._moduleID_moduleName_map = [];
        this._moduleID = 0;
        this._isBindingsApplied = false;
        this._username = "";
        this._dateFormat = "";
        this._templateId = "";
    }

    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    var srmGrid = new SRMGrid();

    //prototypes
    SRMGrid.prototype.Initializer = function Initializer(templateId, moduleID) {
        srmGrid._templateId = templateId;
        srmGrid._moduleID = moduleID;
        init();
    }

    function init() {
        getData();
        var totalWidth = $(window).width();
        $(".SRMGridCommon").css('width', (totalWidth * 0.6 - 62) / 3);
        $(".SRMGridCommon2").css('width', (totalWidth * 0.4 - 62) / 3);
    }

    function getData() {
        var params = {};
        params.templateId = srmGrid._templateId;
        params.moduleId = srmGrid._moduleID;
        CallCommonServiceMethod('GetTemplateDetails', params, OnSuccess_GetGridData, OnFailure, null, false);
    }

    function OnSuccess_GetGridData(data) {
        var res = JSON.parse(data.d);
        srmGrid._pageViewModelInstance = new pageViewModel(res);
        ko.applyBindings(srmGrid._pageViewModelInstance);

        //apply scroll
        let height = $("#SRMGridHeader").height();
        $("#SRMGridData").height($(window).height() - height - 5);
        $("#SRMGridData").css('overflow-y', 'auto');
    }

    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }

    function pageViewModel(data) {
        var self = this;
        self.templateList = ko.observableArray(data);
    };

    //function chainViewModel(data) {
    //    var self = this;
    //    self.AttributeName = data.AttributeName;
    //    self.TabName = data.TabName;
    //    self.SubTabName = data.SubTabName;
    //    self.
    //}



    return srmGrid;
})();

