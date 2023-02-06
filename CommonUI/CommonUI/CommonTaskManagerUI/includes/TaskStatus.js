/// <reference path="/js/jquery-1.6.2.js" />
/// <reference path="/js/jquery-ui-1.8.16.custom.min.js" />
/// <reference path="/includes/SecMasterScripts.js" />
/// <reference path="/includes/TaskStatusInfo.js" />
//Type.registerNamespace('SecMasterJSPage');
//SecMasterJSPage.SMSTaskStatusHelper = function SecMasterJSPage_SMSTaskStatusHelper() {
//    SecMasterJSPage.SMSTaskStatusHelper.initializeBase(this);
//}
//SecMasterJSPage.SMSTaskStatusHelper.prototype = {
//    dispose: function SecMasterJSPage_SMSTaskStatusHelperDispose() {
//        SecMasterJSPage.SMSTaskStatusHelper.callBaseMethod(this, 'dispose');
//    },
//    updated: function SecMasterJSPage_SMSTaskStatusHelperUpdated() {
//        SecMasterJSPage.SMSTaskStatusHelper.callBaseMethod(this, 'updated');
//    }
//}



function SMSTaskStatusMethods() {
}

SMSTaskStatusMethods.prototype = {
    _securityInfo: null,
    _controlIdInfo: null,
    _controls: null,
    TSIntervalId: null,
    TSSeconds: null,

    onClickRblstDate: function onClickRblstDate(e) {
        var target = null;
        if (e.target.tagName.toUpperCase() == 'INPUT' && e.target.type.toUpperCase() == 'RADIO') {
            target = $(e.target);
        }
        if (target != null && target.length > 0) {
            var parentTable = $(getParentElement(target[0], 'TABLE'));
            var selectedValue = parentTable.find("input:[type='radio']:[checked='checked']").first().val();
            var objDate = new Date();
            switch (selectedValue) {
                case '0':
                    SMSTaskStatusMethods.prototype._controls.LblStartDate().text('Start Date : ').css('display', '');
                    SMSTaskStatusMethods.prototype._controls.LblEndDate().text('End Date : ').css('display', '');
                    SMSTaskStatusMethods.prototype._controls.TxtStartDate().attr('title', 'Enter a Start Date').css('display', '').val(objDate.format(SMSTaskStatusMethods.prototype._securityInfo.CultureShortDateFormat));
                    SMSTaskStatusMethods.prototype._controls.TxtEndDate().attr('title', 'Enter an End Date').css('display', '').val(objDate.format(SMSTaskStatusMethods.prototype._securityInfo.CultureShortDateFormat));
                    break;
                case '1':
                    SMSTaskStatusMethods.prototype._controls.LblStartDate().text('From Date : ').css('display', '');
                    SMSTaskStatusMethods.prototype._controls.LblEndDate().text('').css('display', 'none');
                    SMSTaskStatusMethods.prototype._controls.TxtStartDate().attr('title', 'Enter From Date').css('display', '').val(objDate.format(SMSTaskStatusMethods.prototype._securityInfo.CultureShortDateFormat));
                    SMSTaskStatusMethods.prototype._controls.TxtEndDate().attr('title', '').css('display', 'none').val('');
                    break;
                case '2':
                    SMSTaskStatusMethods.prototype._controls.LblStartDate().text('').css('display', 'none');
                    SMSTaskStatusMethods.prototype._controls.TxtStartDate().attr('title', '').css('display', 'none').val('');
                    SMSTaskStatusMethods.prototype._controls.LblEndDate().text('Prior To Date : ').css('display', '');
                    SMSTaskStatusMethods.prototype._controls.TxtEndDate().attr('title', 'Enter Prior To Date').css('display', '').val(objDate.format(SMSTaskStatusMethods.prototype._securityInfo.CultureShortDateFormat));
                    break;
                default:
                    break;
            }
            SMSTaskStatusMethods.prototype._controls.LblStartDateError().css('display', 'none');
            SMSTaskStatusMethods.prototype._controls.LblEndDateError().css('display', 'none');
            SMSTaskStatusMethods.prototype._controls.LblValidationSummary().text('');
        }
    },

    onClickBtnGetStatus: function onClickBtnGetStatus(e) {
        SMSTaskStatusMethods.prototype.TSStopTimer();
        var selectedValue = SMSTaskStatusMethods.prototype._controls.RblstDate().find("input:[type='radio']:[checked='checked']").first().val();
        var errorCount = 0;
        var errormsg = '';
        var msg = '';
        var isvalid = 'true'
        switch (selectedValue) {
            case '0':
                errormsg = SMSTaskStatusMethods.prototype.TSCompareDateFromTodaysDate(SMSTaskStatusMethods.prototype._controls.TxtStartDate().val(), SMSTaskStatusMethods.prototype._controls.LblStartDate().textNBR().replace(':', ''));
                if (errormsg != '') {
                    SMSTaskStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                    return false;
                }
                else {
                    errormsg = SMSTaskStatusMethods.prototype.TSCompareDateFromTodaysDate(SMSTaskStatusMethods.prototype._controls.TxtEndDate().val(), SMSTaskStatusMethods.prototype._controls.LblEndDate().textNBR().replace(':', ''));
                    if (errormsg != '') {
                        SMSTaskStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                        return false;
                    }
                    else {
                        errormsg = SMSTaskStatusMethods.prototype.TSCompareDate(SMSTaskStatusMethods.prototype._controls.TxtStartDate().val(), SMSTaskStatusMethods.prototype._controls.TxtEndDate().val());
                        if (errormsg != '') {
                            SMSTaskStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                            return false;
                        }
                        else {
                            SMSTaskStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', 'none');
                            return true;
                        }
                    }
                }
                break;
            case '1':
                errormsg = SMSTaskStatusMethods.prototype.TSCompareDateFromTodaysDate(SMSTaskStatusMethods.prototype._controls.TxtStartDate().val(), SMSTaskStatusMethods.prototype._controls.LblStartDate().textNBR().replace(':', ''));
                break;
            case '2':
                errormsg = SMSTaskStatusMethods.prototype.TSCompareDateFromTodaysDate(SMSTaskStatusMethods.prototype._controls.TxtEndDate().val(), SMSTaskStatusMethods.prototype._controls.LblEndDate().textNBR().replace(':', ''));
                break;
        }
        if (errormsg != '') {
            SMSTaskStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
            return false;
        }
        else {
            SMSTaskStatusMethods.prototype._controls.LblValidationSummary().css('display', 'none');
            return true;
        }
    },

    onKeyDownTxtTimer: function onKeyDownTxtTimer(e) {
        if (SMSTaskStatusMethods.prototype.TSIntervalId != null)
            clearInterval(SMSTaskStatusMethods.prototype.TSIntervalId);
        if (e != null && e.target != null) {
            var target = $(e.target);
            if (e.keyCode == 13) {
                SMSTaskStatusMethods.prototype._controls.HdnTimer().val(target.val());
                SMSTaskStatusMethods.prototype.TSSeconds = target.val();
                if (SMSTaskStatusMethods.prototype.TSSeconds == "") {
                    SMSTaskStatusMethods.prototype.TSSeconds = 200;
                    SMSTaskStatusMethods.prototype._controls.HdnTimer().val(200);
                }
                SMSTaskStatusMethods.prototype.TSIntervalId = setInterval("SMSTaskStatusMethods.prototype.TSSetCountDown('" + target.attr('id') + "')", 1000);
            }
        }
        else {
            SMSTaskStatusMethods.prototype._controls.TxtTimer().val(SMSTaskStatusMethods.prototype._controls.HdnTimer().val());
            SMSTaskStatusMethods.prototype.TSSeconds = SMSTaskStatusMethods.prototype._controls.HdnTimer().val();
            SMSTaskStatusMethods.prototype.TSIntervalId = setInterval("SMSTaskStatusMethods.prototype.TSSetCountDown('" + SMSTaskStatusMethods.prototype._controls.TxtTimer().attr('id') + "')", 1000);
        }
    },

    onClickGridTaskStatusDetails1: function onClickGridTaskStatusDetails1(e) {
        var target = $(e.target);
        //        if (target[0].tagName.toUpperCase() == 'INPUT' && target[0].type.toUpperCase() == 'IMAGE') {
        if (contains(target.attr('id'), 'imgDetails_')) {
            var row = $(getParentElement(target[0], 'TR'));
            var lblTaskName = row.find("span:[id*='lblTaskName_']").first();
            var lblLogDescValue = row.attr('log_description');
            SMSTaskStatusMethods.prototype._controls.TxtName().val(lblTaskName.textNBR());
            SMSTaskStatusMethods.prototype._controls.TxtDescription().text(lblLogDescValue);
            var objRegExp1 = new RegExp('\\\\', 'g'); //ESCAPE SEQUENCE FOR \\
            var objRegExp2 = new RegExp("'", 'g');
            var objRegExp3 = new RegExp('\r\n', 'g');
            SMSTaskStatusMethods.prototype._controls.LblPanelHeader().text(lblTaskName.textNBR().replace(objRegExp1, '\\\\').replace(objRegExp2, '\\').replace(objRegExp3, ''));
            com.ivp.rad.rscriptutils.RSCommonScripts.showPopUp(SMSTaskStatusMethods.prototype._controlIdInfo.ModalLogDetailsId);
            return false;
        }
        if (target[0].tagName.toUpperCase() == 'INPUT' && target[0].type.toUpperCase() == 'IMAGE') {
            if (contains(target.attr('id'), 'imgBtnDelete_')) {
                var row = $(getParentElement(target[0], 'TR'));
                var lblTaskName = row.find("span:[id*='lblTaskName_']").first();
                var lblTaskStatusIdValue = row.attr('task_status_id');
                var lblStatus = row.find("span:[id*='lblStatus_']").first();
                switch (lblStatus.textNBR().toUpperCase()) {
                    case 'PASSED':
                        break;
                    case 'FAILED':
                    case 'INPROGRESS':
                    case 'QUEUED':
                        SMSTaskStatusMethods.prototype._controls.TxtDeleteId().val(lblTaskStatusIdValue);
                        SMSTaskStatusMethods.prototype._controls.LblTaskNameToDelete().text(lblTaskName.textNBR());
                        com.ivp.rad.rscriptutils.RSCommonScripts.showPopUp(SMSTaskStatusMethods.prototype._controlIdInfo.ModalDeleteTaskId);
                    default: break;
                }
                return false;
            }
            if (contains(target.attr('id'), 'imgBtnRetry_'))
                SMSTaskStatusMethods.prototype._controls.HdnPostBackControl().val(target.attr('id') + '|' + target.attr('FlowId'));
        }
    },

    TSStopTimer: function TSStopTimer() {
        clearInterval(SMSTaskStatusMethods.prototype.TSIntervalId)
    },

    TSSetCountDown: function TSSetCountDown(targetId) {
        var target = $('#' + targetId);
        SMSTaskStatusMethods.prototype.TSSeconds = SMSTaskStatusMethods.prototype.TSSeconds - 1;
        if (SMSTaskStatusMethods.prototype.TSSeconds <= -1) {
            SMSTaskStatusMethods.prototype.TSSeconds += 1
        }
        target.val(SMSTaskStatusMethods.prototype.TSSeconds);
        if (target.val() == "0") {
            SMSTaskStatusMethods.prototype._controls.BtnGetStatus().click();
            SMSTaskStatusMethods.prototype.TSSeconds = SMSTaskStatusMethods.prototype._controls.HdnTimer().val();
            target.val(SMSTaskStatusMethods.prototype.TSSeconds);
        }
    },

    TSCompareDateFromTodaysDate: function TSCompareDateFromTodaysDate(inputDate, lblDate) {
        try {
            if (!SMSTaskStatusMethods.prototype.TSCheckDateWithCurrentDate(inputDate)) {
                return '● Please enter ' + lblDate + 'less than or equal to Todays Date\n ';
            }
            else {
                return '';
            }
        }
        catch (err) {
            alert(ErrorMessage + 'CompareDate \n\n' + err.description);
        }
    },

    TSCheckDateWithCurrentDate: function TSCheckDateWithCurrentDate(dateToCheck) {
        var dtStartDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertStringToDateTime(dateToCheck, com.ivp.rad.rscriptutils.DateTimeFormat.shorDate);
        var curDate = new Date();
        var startTicks = com.ivp.rad.rscriptutils.RSValidators.returnTicks(Sys.Serialization.JavaScriptSerializer.serialize(dtStartDate));
        var currentTicks = com.ivp.rad.rscriptutils.RSValidators.returnTicks(Sys.Serialization.JavaScriptSerializer.serialize(curDate));

        if (startTicks < currentTicks) {
            return true;
        }
        if (dtStartDate <= curDate) {
            return true;
        }
        return false;
    },

    TSCompareDate: function TSCompareDate(startdate, enddate) {
        try {
            if (!com.ivp.rad.rscriptutils.RSValidators.checkDates(null, startdate, null, enddate, false)) {
                return '● Please enter End Date greater than or equal to Start Date\n ';
            }
            else {
                return '';
            }
        }
        catch (err) {
            alert(ErrorMessage + 'CompareDate \n\n' + err.description);
        }
    }
}

function TSResizeGrid() {
    resizeGridFinal(SMSTaskStatusMethods.prototype._controlIdInfo.GridTaskStatusDetails1Id, SMSTaskStatusMethods.prototype._controlIdInfo.PanelTopTaskStatusId, SMSTaskStatusMethods.prototype._controlIdInfo.PanelMiddleTaskStatusId, '', 0, 0);
}

//UPDATED
function SMSTaskStatus(securityInfo, controlIdInfo) {
    SMSTaskStatusMethods.prototype._securityInfo = eval("(" + securityInfo + ")");
    SMSTaskStatusMethods.prototype._controlIdInfo = eval("(" + controlIdInfo + ")");
    SMSTaskStatusMethods.prototype._controls = new SMSTaskStatusControls(SMSTaskStatusMethods.prototype._controlIdInfo);
    //CREATE AND REMOVE HANDLERS
    function createHandlers() {
        if (SMSTaskStatusMethods.prototype._controls.RblstDate().length != 0)
            SMSTaskStatusMethods.prototype._controls.RblstDate().unbind('click').click(SMSTaskStatusMethods.prototype.onClickRblstDate);
        if (SMSTaskStatusMethods.prototype._controls.BtnGetStatus().length != 0)
            SMSTaskStatusMethods.prototype._controls.BtnGetStatus().unbind('click').click(SMSTaskStatusMethods.prototype.onClickBtnGetStatus);
        if (SMSTaskStatusMethods.prototype._controls.TxtTimer().length != 0)
            SMSTaskStatusMethods.prototype._controls.TxtTimer().unbind('keydown').bind('keydown', SMSTaskStatusMethods.prototype.onKeyDownTxtTimer);
        if (SMSTaskStatusMethods.prototype._controls.GridTaskStatusDetails1().length != 0)
            SMSTaskStatusMethods.prototype._controls.GridTaskStatusDetails1().unbind('click').click(SMSTaskStatusMethods.prototype.onClickGridTaskStatusDetails1);
        //        if (SMSTaskStatusMethods.prototype._controls.GridTaskStatusDetails1().length != 0)
        //            SMSTaskStatusMethods.prototype._controls.GridTaskStatusDetails1().unbind('contextmenu').contextmenu(SMSTaskStatusMethods.prototype.TSResizeGrid);
    }
    createHandlers();
}

//SecMasterJSPage.SMSDownstreamSystemStatusHelper.registerClass('SecMasterJSPage.SMSTaskStatusHelper', Sys.Component);
//if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();