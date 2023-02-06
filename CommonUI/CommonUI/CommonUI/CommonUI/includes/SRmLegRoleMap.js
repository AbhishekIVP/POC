var srmLegRoleMap = (function () {
    
    function SRMLegRoleMap() {

        this._controlIdInfo = null;
        this._securityInfo = null;
        this._pageViewModelInstance = null;
        this._moduleID_moduleName_map = [];
        this._moduleID = 0;
        this._isBindingsApplied = false;
        this._typeId = 0;
    }

    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    var srmLegRoleMap = new SRMLegRoleMap();


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
        self.legList = ko.observableArray([]);
        self.roleList = ko.observableArray([]);
        self.entityName = ko.observable(data.entityName);
        for (var i in data.ll) {
            var xyz = data.ll[i]
            self.legList.push(xyz);
        }
        for (var i in data.en) {
            var xyz = data.en[i]
            self.roleList.push(xyz);
        }
    };

    //Init functions
    function init() {
        //var data = {
        //    entityName: "Party",
        //    ll: [
        //        "Leg1","Leg2","Leg3","Leg4","Leg5","Leg6","Leg7"
        //    ],
        //    en: ["Role1", "Role2", "Role3", "Role4", "Role5", "Role6", "Role7", "Role8", "Role9", "Role10", "Role11", "Role12", "Role13", "Role14", "Role15"]     
        //}
        //srmLegRoleMap._pageViewModelInstance = new pageViewModel(data);
        //ko.applyBindings(srmLegRoleMap._pageViewModelInstance, document.getElementById("SRMLegRoleParent"));
        getData();
        
    };

 
    SRMLegRoleMap.prototype.Initializer = function Initializer(typeId) {
        srmLegRoleMap._typeId = typeId;
        //srmLegRoleMap._typeId = 770;
        init();
  
        
    }

    function getData() {
        var params = {};
        params.typeId = srmLegRoleMap._typeId;
        CallCommonServiceMethod('GetRoleData', params, OnSuccess_GetRoleData, OnFailure, null, false);
    }

    function OnSuccess_GetRoleData(result) {
        var data = JSON.parse(result.d);
        srmLegRoleMap._pageViewModelInstance = new pageViewModel(data);
        ko.applyBindings(srmLegRoleMap._pageViewModelInstance, document.getElementById("SRMLegRoleParent"));
        resize();
    }

    function pageViewModel(data){
        var self = this;
        self.partyName = data.Table[0].entity_display_name;
        self.legsList = ko.observableArray();
        self.rolesList = ko.observableArray();
        
        for (var i in data.Table1) {
            var item = data.Table1[i].entity_display_name;
            self.legsList.push(item);
        }

        for (var i in data.Table2) {
            var item = data.Table2[i].entity_display_name;
            self.rolesList.push(item);
        }
    }

    function OnFailure() {
        
    }

    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }

    function resize() {
        var height = $(window).height() - 10;
        $("#SRMLegRoleLeft").css('height', height);
        $("#SRMLegRoleRight").css('height', height);
        $("#SRMLegRoleCenter").css('height', height);
        $(".SRMArrowParent").css('height', height);

        
        
        var leftSideHeight;
        leftSideHeight = $(".SRMLegRoleLeftItemsHeight").last().offset().top + $(".SRMLegRoleLeftItemsHeight").height() - $(".SRMLegRoleLeftItemsHeight").first().offset().top;
        $("#SRMLegRoleLeftHeader").css('margin-top', (($("#SRMLegRoleLeft").height() - leftSideHeight) / 2) - 32 );


        if ($(".SRMLegRoleLeftItemsHeight").length == 1) {
            $(".SRMLegRoleLeftHeaderText").text("No legs exist");
            $("#SRMLegRoleLeftHeader").css('margin-top', parseInt($("#SRMLegRoleLeftHeader").css('margin-top')) + 29);
        }

        $("#SRMLegRoleCenterItems").css('margin-top', ($("#SRMLegRoleCenter").height() - $("#SRMLegRoleCenterItems").height()) / 2);

        var rightSideHeight;
        //20px margin
        let rightSideMargin = 34;
        rightSideHeight = $(".SRMLegRoleRightItemsHeight").last().offset().top + $(".SRMLegRoleRightItemsHeight").height() - $(".SRMLegRoleRightItemsHeight").first().offset().top;
        $("#SRMLegRoleRightHeader").css('margin-top', (($("#SRMLegRoleRight").height() - rightSideHeight - rightSideMargin) / 2) - 15);

        if ($(".SRMLegRoleRightItemsHeight").length == 1) {
            $(".SRMLegRoleRightHeaderText").text("No roles exist");
            $("#SRMLegRoleRightHeader").css('margin-top', parseInt($("#SRMLegRoleRightHeader").css('margin-top')) + 29);
       }

        //arrow centering
        $(".SRMLegArrow").css('margin-top', (($(".SRMArrowParent").height() - $(".SRMLegArrow").height())) / 2);

        if($(".SRMLegRoleRightItemsHeight").length >= 1 && $(".SRMLegRoleLeftItemsHeight").length >= 1)
        {
            $("#SRMLegRoleLeftHeader").css('margin-top', (($("#SRMLegRoleRight").height() - rightSideHeight - rightSideMargin) / 2) - 15);
            $("#SRMLegRoleRightHeader").css('margin-top', (($("#SRMLegRoleRight").height() - rightSideHeight - rightSideMargin) / 2) - 15);

            $("#SRMLegRoleCenterItems").css('margin-top', ($("#SRMLegRoleCenter").height() - $("#SRMLegRoleCenterItems").height()) / 3);
            $(".SRMLegArrow").css('margin-top', (($(".SRMArrowParent").height() - $(".SRMLegArrow").height())) / 3);
        }

        
    }

    return srmLegRoleMap;
})();