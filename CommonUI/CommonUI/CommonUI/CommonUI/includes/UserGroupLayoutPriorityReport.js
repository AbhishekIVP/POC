var UserGroupLayoutReport = function () {
    function UserGroupLayoutPriorityReportClass() {
        this._path = "";
        this._username = "";
        this._commonServiceLocation = "/BaseUserControls/Service/CommonService.svc";
        this._moduleName_moduleID_map = [];
        this._moduleID = 0;
        this.guid = '';
    }
    const GridDivId = "srm_UserGroupLayoutReport_RadNeoClientGridDiv";
    var userGroupLayoutReport = new UserGroupLayoutPriorityReportClass();
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');
    UserGroupLayoutPriorityReportClass.prototype.gridObj = {
        "SelectRecordsAcrossAllPages": true,
        "ViewKey": "", //to be overriden
        "CacheGriddata": true,
        "CurrentPageId": "", //to be overriden
        "SessionIdentifier": "", //to be overriden
        "UserId": "",
        "CheckBoxInfo": {},
        "Height": "400px",
        "ColumnsWithoutClientSideFunctionality": [],
        "ColumnsNotToSum": [],
        "RequireEditGrid": false,
        "RequireEditableRow": false,
        "IdColumnName": "row_id", //to be overriden
        "TableName": "Table",
        "PageSize": 100,
        "RequirePaging": false,
        "RequireInfiniteScroll": true,
        "CollapseAllGroupHeader": false,
        "GridTheme": 2,
        "HideFooter": false,
        "DoNotExpand": false,
        "ItemText": "Number of Records",
        "DoNotRearrangeColumn": true,
        "RequireGrouping": true,
        "RequireMathematicalOperations": false,
        "RequireSelectedRows": true,
        "RequireExportToExcel": true,
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
    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });
    controls = function (name) {
        switch (name) {
            case 'GridDiv': return $('#' + GridDivId);
                break;
        }
    }
    function initReport() {
        let userinfo = UserGroupLayoutReportSessionInfo;
        userGroupLayoutReport._username = userinfo.username;
        userGroupLayoutReport._path = path;
        userGroupLayoutReport.guid = userinfo.guid;
        init();

    }
   

    function GetReport(paramModuleId) {
        params = {};
        params.userName = userGroupLayoutReport._username;
        params.shortDateFormat = UserGroupLayoutReportSessionInfo.clientShortDate;
        params.guid = userGroupLayoutReport.guid;
        params.gridDivId = GridDivId;
        params.moduleId = paramModuleId;

        CallCommonServiceMethod('GetUserGroupLayoutPriorityReport', params, BindRADGrid, OnFailure, null, true);

    }

    function BindRADGrid(ResponseData) {
        try {
            if (ResponseData.d == "[]" || ResponseData.d == null || ResponseData.d == "null" || ResponseData.d == "" || typeof (ResponseData.d) == 'undefined') {
                controls('GridDiv').css('display', 'none');
                $("#srm_mandatorydatareport_NoRecordsDiv").css('display', 'block');// show no reports to display image
                onUpdated();
                return;
            }
            $("#srm_mandatorydatareport_NoRecordsDiv").css('display', 'none');
            controls('GridDiv').css('display', 'block');

            createRADGridDiv();
            var tempObj = $.extend({}, userGroupLayoutReport.gridObj);
            tempObj = initializeGridPropertiesSM(tempObj, $(window).height() - 270);
            xlgridloader.create(GridDivId, "Test", tempObj, "");
            setIsUpdatedOnGridBind();
            // onUpdated();
        }
        catch (ee) {
            onUpdated();
        }
    }
    function setIsUpdatedOnGridBind() {
        if (controls('GridDiv').children().length < 1) {
            window.setTimeout(setIsUpdatedOnGridBind, 1000);
        }
        else {
            onUpdated();
        }
    }

    function initializeGridPropertiesSM(tempObj, gridHeight) {
        tempObj.GridId = GridDivId;
        tempObj.UserId = userGroupLayoutReport._username;
        tempObj.ViewKey = userGroupLayoutReport.guid;
        tempObj.CurrentPageId = userGroupLayoutReport.guid;
        tempObj.SessionIdentifier = userGroupLayoutReport.guid;
        tempObj.IdColumnName = "row_id";
        tempObj.DateFormat = UserGroupLayoutReportSessionInfo.clientLongDate;
        //tempObj.CustomRowsDataInfo = customRowInfo;
        if (gridHeight !== undefined) {
            tempObj.Height = gridHeight + "px";
        }
        return tempObj;
    }

    /////////////
    ////////////////////////////
    // Top Bar Init functions //
    ////////////////////////////
    function init() {
        var listofTabsToBindFunctionsWith = CommonModuleTabs.ModuleList();

        for (i in listofTabsToBindFunctionsWith) {
            let item = listofTabsToBindFunctionsWith[i];
            userGroupLayoutReport._moduleName_moduleID_map[item.displayName.toLowerCase()] = parseInt(item.moduleId);
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
        //SRMProductTabs.executeBind();
    };

    function initSecMaster() {
        clearRadGridDiv();
        GetReport(3);
    }

    function initRefMaster() {
        clearRadGridDiv();
        GetReport(6);
    }

    function initFundMaster() {
        clearRadGridDiv();
        GetReport(18);
    }

    function initPartyMaster() {
        clearRadGridDiv();
        GetReport(20);
    }
   

    function clearRadGridDiv() {
        controls('GridDiv').remove();
    }
    function createRADGridDiv() {
        if (controls('GridDiv').length == 0)
            $('#UserGroupLayoutReportContainer').append(" <div id='" + GridDivId+"'></div>");
    }
    function OnFailure(result) {
        onUpdated();
        var exception = {};
        try {
            exception = JSON.parse(result.responseText);
        }
        catch (ee) {
            exception.Message = 'Error while retrieving data';
        }
        showErrorDiv('Alert', 'fail_icon.png', exception.Message);
    }
    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }
    function showErrorDiv(errorHeading, srcImageName, errorMessageText) {
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
            $('.workFlowDetailsError_popupMessage').css('height', '100px');
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

    onUpdating = function SecMasterJSCommon_SMSCommons$onUpdating() {
        eval('IsPageReloading = true;');
        try {
            com.ivp.rad.rscriptutils.RSValidators.getObject('disableDiv').style.display = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
            com.ivp.rad.rscriptutils.RSValidators.getObject('loadingImg').style.display = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
            var scrollHeight = $(window).height();
            var scrollWidth = document.body.scrollWidth.toString();
            var windowHeight = document.body.parentNode.offsetHeight.toString();
            com.ivp.rad.rscriptutils.RSValidators.getObject('disableDiv').style.height = scrollHeight + 'px';
            com.ivp.rad.rscriptutils.RSValidators.getObject('disableDiv').style.width = scrollWidth + 'px';
        }
        catch (ex) {
            throw ex;
        }
    }
   
    onUpdated = function SecMasterJSCommon_SMSCommons$onUpdated() {
        eval('IsPageReloading = false;');
        try {
            com.ivp.rad.rscriptutils.RSValidators.getObject('disableDiv').style.display = SecMasterJSCommon.SMSConstants.strinG_NONE;
            com.ivp.rad.rscriptutils.RSValidators.getObject('loadingImg').style.display = SecMasterJSCommon.SMSConstants.strinG_NONE;
            var _inputElement = (com.ivp.rad.rscriptutils.RSValidators.getObject('identifier'));
            if (_inputElement != null) {
                _inputElement.value = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
            }
        }
        catch (ex) {
            throw ex;
        }
    }
   
    UserGroupLayoutPriorityReportClass.prototype.Initializer = function () {
        initReport();
    }

    return userGroupLayoutReport;
}();