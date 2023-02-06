var caSourcePrioritizationMain = (function () {

    function CASourcePrioritizationMain() {
        this._controlIdInfo = null;
        this._securityInfo = null;
        this._pageViewModelInstance = null;
    }

    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    var caSourcePrioritizationMain = new CASourcePrioritizationMain();
    

    /////////////////
    // VIEW MODELS // 
    /////////////////

    // Page View Model 
    function pageViewModel(data) {
        var self = this;

        self.corpactionListing = ko.observableArray();

        if (typeof (data) != 'undefined') {

            if (data.length > 0) {
                for (var item in data) {
                    {
                        self.corpactionListing.push(new chainViewModel(data[item]));
                    }
                }
            }
        }

    };
    
    //Chain View Model
    function chainViewModel(data) {
        var self = this;

        self.corpActionTypeName = data.CorpActionTypeName;
        self.corpActionTypeID = data.CorpActionTypeID;

        self.showConfigure = false;

        //Data Source Status
        self.dataSourceStatus = "Data Source not available.";
        if (data.Status.toLowerCase().trim() == "true") {
            self.dataSourceStatus = "Data Source available.";
            self.showConfigure = true;
        }

        //Priority Status
        self.priorityStatus = data.IsPrioritized;
    };

    //////////////////////////////////
    // Call Common Service Fuctions //
    //////////////////////////////////
    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }


    /////////////////////////////////////////////////
    // Callback Functions for COMMON SERVICE Calls //
    /////////////////////////////////////////////////

    //This Function is called after Service Call on Load.
    function OnSuccess_GetCorpActionTypesData(result) {

        if (typeof result.d != 'undefined' && result.d.length > 0) {
            //Apply Knockout
            if (typeof ko !== 'undefined') {

                caSourcePrioritizationMain._pageViewModelInstance = new pageViewModel(result.d);

                ko.applyBindings(caSourcePrioritizationMain._pageViewModelInstance);
            }
        }
        else {
            //Show Common Error Msg

        }
    }

    //This function is called if any Service Call returns an error - CHANGED FINAL
    function OnFailure(result) {
        $("#" + caSourcePrioritizationMain._controlIdInfo.LblErrorId).text("Service returned an error" + ((result.responseJSON == null || result.responseJSON.Message == null || result.responseJSON.Message.length == 0) ? "" : (" : " + result.responseJSON.Message)) + ((result.responseText == null || result.responseText.length == 0) ? "" : (" : " + result.responseText)));
        $find(caSourcePrioritizationMain._controlIdInfo.ModalErrorId).show();
    }


    ///////////////////////////
    // Event Based Functions //
    ///////////////////////////

    //Function called when Tab is changed.
    //CASourcePrioritizationMain.prototype.onClickTab = function onClickTab(obj, event) {

    //    //Set tabIsSelected to false for all tabs.
    //    for (var item in caSourcePrioritizationMain._pageViewModelInstance.tabListing()) {
    //        caSourcePrioritizationMain._pageViewModelInstance.tabListing()[item].tabIsSelected(false);
    //    }

    //    //Set true for this tab.
    //    obj.tabIsSelected(true);
    //};
    

    ///////////////////////////////////////////////////////////////
    // init function to Load Initial Data as per Default Filters //
    ///////////////////////////////////////////////////////////////
    function init() {
        // Get data from Service Call
        var params = {};
        
        CallCommonServiceMethod('CASPM_GetCorpActionTypesData', params, OnSuccess_GetCorpActionTypesData, OnFailure, null, false);
    }
    

    ////////////////////////////
    // "Initializer" function // - SectypeID Change Required
    ////////////////////////////
    CASourcePrioritizationMain.prototype.Initializer = function Initializer(controlInfo, securityInfo) {

        //Set Control ID Info & Security Info
        caSourcePrioritizationMain._controlIdInfo = eval("(" + controlInfo + ")");
        caSourcePrioritizationMain._securityInfo = eval("(" + securityInfo + ")");

        //Set Height of Entire Body of the Page
        $('#CASourcePrioritizationMainBody').height($(window).height());

        //Setting Height of Area below the Filters
        $("#CASPM_Main").height($('#CASourcePrioritizationMainBody').height() - $('#CASPM_Top').height() - 15);

        init();
    }

    // WINDOW Resize Function 
    $(window).resize(function () {

        //Set Height of Entire Body of the Page
        $('#CASourcePrioritizationMainBody').height($(window).height());

        //Setting Height of Area below the Filters
        $("#CASPM_Main").height($('#CASourcePrioritizationMainBody').height() - $('#CASPM_Top').height() - 15);

    })

    return caSourcePrioritizationMain;
})();