function SRMRealTimeStatusControls(controlIdInfo) {
    this._controlIdInfo = controlIdInfo;
}

SRMRealTimeStatusControls.prototype = {
    _controlIdInfo: null,

    //    _txtTimer: null,
    //    TxtTimer: function get_txtTimer() {
    //        if (this._txtTimer == null || this._txtTimer.length == 0) {
    //            this._txtTimer = $('#' + this._controlIdInfo.TxtTimerId);
    //        }
    //        return this._txtTimer;
    //    },

    _btnGetStatus: null,
    BtnGetStatus: function get_btnGetStatus() {
        if (this._btnGetStatus == null || this._btnGetStatus.length == 0) {
            this._btnGetStatus = $('#' + this._controlIdInfo.BtnGetStatusId);
        }
        return this._btnGetStatus;
    },

    //    _hdnTimer: null,
    //    HdnTimer: function get_hdnTimer() {
    //        if (this._hdnTimer == null || this._hdnTimer.length == 0) {
    //            this._hdnTimer = $('#' + this._controlIdInfo.HdnTimerId);
    //        }
    //        return this._hdnTimer;
    //    },

    _rblstDate: null,
    RblstDate: function get_rblstDate() {
        if (this._rblstDate == null || this._rblstDate.length == 0) {
            this._rblstDate = $('#' + this._controlIdInfo.RblstDateId);
        }
        return this._rblstDate;
    },
    _divExceptionDate: null,
    DivExceptionDate: function get_divExceptionDate() {
        if (this._divExceptionDate == null || this._divExceptionDate.length == 0) {
            this._divExceptionDate = $('#divExceptionDate');
        }
        return this._divExceptionDate;
    },
    _divFilter: null,
    DivFilter: function get_divFilter() {
        if (this._divFilter == null || this._divFilter.length == 0) {
            this._divFilter = $('#' + this._controlIdInfo.DivFilterId);
        }
        return this._divFilter;
    },
    _divFilterDDLs: null,
    DivFilterDDLs: function get_divFilterDDLs() {
        if (this._divFilterDDLs == null || this._divFilterDDLs.length == 0) {
            this._divFilterDDLs = $('#divFilterDDLs');
        }
        return this._divFilterDDLs;
    },
    _lblStartDate: null,
    LblStartDate: function get_lblStartDate() {
        if (this._lblStartDate == null || this._lblStartDate.length == 0) {
            this._lblStartDate = $('#' + this._controlIdInfo.LblStartDateId);
        }
        return this._lblStartDate;
    },
    _hdnExceptionDate: null,
    HdnExceptionDate: function get_hdnExceptionDate() {
        if (this._hdnExceptionDate == null || this._hdnExceptionDate.length == 0) {
            this._hdnExceptionDate = $('#' + this._controlIdInfo.HdnExceptionDateId);
        }
        return this._hdnExceptionDate;
    },
    _trExceptionDateCustom: null,
    TrExceptionDateCustom: function get_trExceptionDateCustom() {
        if (this._trExceptionDateCustom == null || this._trExceptionDateCustom.length == 0) {
            this._trExceptionDateCustom = $('#trExceptionDateCustom');
        }
        return this._trExceptionDateCustom;
    },

    _txtStartDate: null,
    TxtStartDate: function get_txtStartDate() {
        if (this._txtStartDate == null || this._txtStartDate.length == 0) {
            this._txtStartDate = $('#' + this._controlIdInfo.TxtStartDateId);
        }
        return this._txtStartDate;
    },

    _lblStartDateError: null,
    LblStartDateError: function get_lblStartDateError() {
        if (this._lblStartDateError == null || this._lblStartDateError.length == 0) {
            this._lblStartDateError = $('#' + this._controlIdInfo.LblStartDateErrorId);
        }
        return this._lblStartDateError;
    },

    _lblEndDate: null,
    LblEndDate: function get_lblEndDate() {
        if (this._lblEndDate == null || this._lblEndDate.length == 0) {
            this._lblEndDate = $('#' + this._controlIdInfo.LblEndDateId);
        }
        return this._lblEndDate;
    },

    _txtEndDate: null,
    TxtEndDate: function get_txtEndDate() {
        if (this._txtEndDate == null || this._txtEndDate.length == 0) {
            this._txtEndDate = $('#' + this._controlIdInfo.TxtEndDateId);
        }
        return this._txtEndDate;
    },

    _lblEndDateError: null,
    LblEndDateError: function get_lblEndDateError() {
        if (this._lblEndDateError == null || this._lblEndDateError.length == 0) {
            this._lblEndDateError = $('#' + this._controlIdInfo.LblEndDateErrorId);
        }
        return this._lblEndDateError;
    },

    _lblValidationSummary: null,
    LblValidationSummary: function get_lblValidationSummary() {
        if (this._lblValidationSummary == null || this._lblValidationSummary.length == 0) {
            this._lblValidationSummary = $('#' + this._controlIdInfo.LblValidationSummaryId);
        }
        return this._lblValidationSummary;
    },
    _customJQueryDateFormat: null,
    CustomJQueryDateFormat: function get_CustomJQueryDateFormat() {
        if (this._customJQueryDateFormat == null || this._customJQueryDateFormat.length == 0)
            this._customJQueryDateFormat = this._controlIdInfo.CustomJQueryDateFormat;
        return this._customJQueryDateFormat;
    },
    _customJQueryDateTimeFormat: null,
    CustomJQueryDateTimeFormat: function get_CustomJQueryDateTimeFormat() {
        if (this._customJQueryDateTimeFormat == null || this._customJQueryDateTimeFormat.length == 0)
            this._customJQueryDateTimeFormat = this._controlIdInfo.CustomJQueryDateTimeFormat;
        return this._customJQueryDateTimeFormat;
    },

    _productName: null,
    ProductName: function get_ProductName() {
        if (this._productName == null || this._productName.length == 0) {
            this._productName = this._controlIdInfo.ProductName;
        }
        return this._productName;
    },

    _userName: null,
    UserName: function get_UserName() {
        if (this._userName == null || this._userName.length == 0) {
            this._userName = this._controlIdInfo.UserName;
        }
        return this._userName;
    }
}
function SMSSRMRealTimeStatusMethods() {
}

SMSSRMRealTimeStatusMethods.prototype = {
    _securityInfo: null,
    _controlIdInfo: null,
    _controls: null,
    isTargetForPostback: false,
    onClickRblstDate: function onClickRblstDate(e) {
        var target = null;
        if (e.target.tagName.toUpperCase() == 'INPUT' && e.target.type.toUpperCase() == 'RADIO') {
            target = $(e.target);
        }
        if (target != null && target.length > 0) {
            var parentTable = $(getParentElement(target[0], 'TABLE'));
            var selectedValue = parentTable.find('[type="radio"]:checked').first().val();
            switch (selectedValue) {
                case '0':
                    SMSSRMRealTimeStatusMethods.prototype._controls.LblStartDate().text('Start Date : ').css('display', '');
                    SMSSRMRealTimeStatusMethods.prototype._controls.LblEndDate().text('End Date : ').css('display', '');
                    SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().attr('title', 'Enter a Start Date').css('display', '');
                    SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().attr('title', 'Enter an End Date').css('display', '');
                    break;
                case '1':
                    SMSSRMRealTimeStatusMethods.prototype._controls.LblStartDate().text('From Date : ').css('display', '');
                    SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().attr('title', 'Enter From Date').css('display', '');
                    SMSSRMRealTimeStatusMethods.prototype._controls.LblEndDate().text('').css('display', 'none');
                    SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().attr('title', '').css('display', 'none');
                    break;
                case '2':
                    SMSSRMRealTimeStatusMethods.prototype._controls.LblStartDate().text('').css('display', 'none');
                    SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().attr('title', '').css('display', 'none');
                    SMSSRMRealTimeStatusMethods.prototype._controls.LblEndDate().text('Prior To Date : ').css('display', '');
                    SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().attr('title', 'Enter Prior To Date').css('display', '');
                    break;
                default:
                    break;
                    SMSSRMRealTimeStatusMethods.prototype._controls.LblStartDateError().css('display', 'none');
                    SMSSRMRealTimeStatusMethods.prototype._controls.LblEndDateError().css('display', 'none');
                    SMSSRMRealTimeStatusMethods.prototype._controls.LblValidationSummary().text('');
            }
        }
    },
    onClickDivFilter: function onClickDivFilter(e) {
        //alert("m");
        //   var doPostback = true;
        var target = $(e.target);
        if (target.parents("div[id='divFilterDDLs']").length == 0) {
            if (target[0].tagName.toUpperCase() == 'A') {
                e.stopPropagation();
                switch (target.attr('id')) {

                    case $("#ctl00_cphMain_Status_labeldateselectiondiv").attr('id'):
                        SMSSRMRealTimeStatusMethods.prototype.showHideExceptionDate(e);
                        break;

                }
            }
            //            else if (target[0].tagName.toUpperCase() == 'INPUT' && SecMasterJSCommon.SMSCommons.contains(target.attr('id'), SMSSRMRealTimeStatusMethods.prototype._controls.btnGetStatus().attr('id'))) {
            //                SMSSRMRealTimeStatusMethods.prototype.toggleFilters(e);
            //                var inValidDivs = SMSSRMRealTimeStatusMethods.prototype._controls.DivFilterDDLs().find("div[isValid='0']");
            //                if (inValidDivs.length > 0) {
            //                    doPostback = false;
            //                    SMSSRMRealTimeStatusMethods.prototype.isTargetForPostback = false;
            //                }
            //                else {
            //                    doPostback = true;
            //                    SMSSRMRealTimeStatusMethods.prototype.isTargetForPostback = true;
            //                }

            //                return doPostback;
            //            }
        }
    },
    onClickDivExceptionDate: function onClickDivExceptionDate(e) {
        //alert("ll");


        e.stopPropagation();
        //        if ($('#divCustomDateOptions').css('display')=='block')
        //        {
        //         SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().customDateTimePicker('hide');
        //         SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().customDateTimePicker('hide');
        //       //  $("#textEndDate").customDateTimePicker('hide');
        //        }
        SMSSRMRealTimeStatusMethods.prototype.selectDivExceptionDate(e.target);

        if ((e.target.id == SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().prop('id')) && (SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().attr('isClicked') == 0))
            SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().attr('isClicked', '1');
        if ((e.target.id == SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().prop('id')) && (SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().attr('isClicked') == 0))
            SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().attr('isClicked', '1');

        else if (SMSSRMRealTimeStatusMethods.prototype._controls.HdnExceptionDate().val() == '4') {
            if (!(e.target.id == SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().prop('id')) || (e.target.id == SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().prop('id')) || (SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().attr('isclicked') != 0)) {
                if (SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().attr('isClicked') == 1) {
                    SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().customDateTimePicker('hide');
                    SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().attr('isClicked', '0');
                }
            }
            if ((!(e.target.id == SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().prop('id')) || (e.target.id == SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().prop('id')) || (SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().attr('isclicked') != 0))) {
                if (SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().attr('isClicked') == 1) {
                    SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().customDateTimePicker('hide');
                    SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().attr('isClicked', '0');
                }
            }
        }
        else {
            if (SMSSRMRealTimeStatusMethods.prototype._controls.HdnExceptionDate().val() != '4')
                SMSSRMRealTimeStatusMethods.prototype.showHideExceptionDate(e);


        }
    },
    selectDivExceptionDate: function selectDivExceptionDate(element) {
        var target = $(element);
        if (SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().attr('isClicked') == 0 && (SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().attr('isClicked') == 0)) {
            if (target[0].tagName.toUpperCase() == 'TR')
                target = target.find('td:first');
            if (target[0].tagName.toUpperCase() == 'TD') {
                SMSSRMRealTimeStatusMethods.prototype._controls.HdnExceptionDate().val(target.attr('value'));
                $('#divExceptionDate tr').css('color', 'rgb(0,0,0)');
                $('#divExceptionDate tr:eq(' + SMSSRMRealTimeStatusMethods.prototype._controls.HdnExceptionDate().val() + ')').css('color', '#48a3dd');
                var table = target.parents('table:first');
                table.find("td[marked='1']").each(function (index) {
                    $(this).attr('marked', "0");
                });
                target.attr('marked', '1');
                if ((SMSSRMRealTimeStatusMethods.prototype._controls.HdnExceptionDate().val() == '4'))
                    SMSSRMRealTimeStatusMethods.prototype._controls.TrExceptionDateCustom().css('display', '');
                else {
                    SMSSRMRealTimeStatusMethods.prototype._controls.TrExceptionDateCustom().css('display', 'none');

                }

            }
        }
    },
    toggleFilters: function toggleFilters(e) {
        var activeDiv = SMSSRMRealTimeStatusMethods.prototype._controls.DivFilterDDLs().find("div[state='1']");
        var flagToggle = true;
        if (activeDiv.length > 0) {
            switch (activeDiv.attr('id')) {

                case 'divExceptionDate':
                    flagToggle = !SMSSRMRealTimeStatusMethods.prototype.showHideExceptionDate(e);
                    break;

            }
        }
        if (!flagToggle)
            $('[id$="btnGetStatus"]').attr('disabled', true);
        else
            $('[id$="btnGetStatus"]').removeAttr('disabled');
        return flagToggle;
    },

    showHideExceptionDate: function showHideExceptionDate(e) {
        var target = $("#ctl00_cphMain_Status_labeldateselectiondiv");
        //        var selectedExceptionDate = objException._controls.DivExceptionDate().find("tbody > tr:lt(5)");
        //        for (var i = 0; i < selectedExceptionDate.length; i++) {
        //            if ($(selectedExceptionDate[i]).find("td").text().trim() == target.text()) {
        //                
        //            }
        //        }

        if (SMSSRMRealTimeStatusMethods.prototype._controls.DivExceptionDate().attr('state') == '0' && SMSSRMRealTimeStatusMethods.prototype.toggleFilters(e)) {
            SMSSRMRealTimeStatusMethods.prototype.selectDivExceptionDate(SMSSRMRealTimeStatusMethods.prototype._controls.DivExceptionDate().find("td[value='" + SMSSRMRealTimeStatusMethods.prototype._controls.HdnExceptionDate().val() + "']")[0]);

            if ((SMSSRMRealTimeStatusMethods.prototype._controls.DivExceptionDate()).is(":hidden")) {
                (SMSSRMRealTimeStatusMethods.prototype._controls.DivExceptionDate()).show();
                (SMSSRMRealTimeStatusMethods.prototype._controls.DivExceptionDate()).attr('state', '1');

            }
            else {
                (SMSSRMRealTimeStatusMethods.prototype._controls.DivExceptionDate()).hide();
                (SMSSRMRealTimeStatusMethods.prototype._controls.DivExceptionDate()).attr('state', '0');
            }
            // SMSSRMRealTimeStatusMethods.prototype._controls.DivExceptionDate().slideToggle("slow");
            //objException._controls.DivExceptionDate().css('display', ''); //[0].className = 'ExceptionDateDropDownsVisible';            
            $("#ctl00_cphMain_Status_labeldateselectiondiv").addClass('selectedOption');
            var positionTop = ($("#ctl00_cphMain_Status_labeldateselectiondiv").outerHeight() + $("#ctl00_cphMain_Status_labeldateselectiondiv").offset().top + 3) + 'px';
            var positionLeft = $("#ctl00_cphMain_Status_labeldateselectiondiv").offset().left;
            SMSSRMRealTimeStatusMethods.prototype._controls.DivExceptionDate().css('top', positionTop);
            SMSSRMRealTimeStatusMethods.prototype._controls.DivExceptionDate().css('left', positionLeft);
            SMSSRMRealTimeStatusMethods.prototype._controls.DivExceptionDate().attr('state', '1');
        }
        else {
            var flagShowValidationMsg = false;
            if (SMSSRMRealTimeStatusMethods.prototype._controls.HdnExceptionDate().val() == '4') {
                var selectedValue = SMSSRMRealTimeStatusMethods.prototype._controls.RblstDate().find("input[type='radio']:checked").first().val();
                var errormsg = '';
                var isDateValid = true;
                switch (selectedValue) {
                    case '0':
                        errormsg = CompareDateFromTodaysDateUS(SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().val(), SMSSRMRealTimeStatusMethods.prototype._controls.LblStartDate().textNBR().replace(':', ''));
                        if (errormsg != '') {
                            SMSSRMRealTimeStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                            isDateValid = false;
                        }
                        else {
                            errormsg = CompareDateFromTodaysDateUS(SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().val(), SMSSRMRealTimeStatusMethods.prototype._controls.LblEndDate().textNBR().replace(':', ''));
                            if (errormsg != '') {
                                SMSSRMRealTimeStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                                isDateValid = false;
                            }
                            else {
                                errormsg = CompareDateUS(SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().val(), SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().val());
                                if (errormsg != '') {
                                    SMSSRMRealTimeStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                                    isDateValid = false;
                                }
                                else {
                                    SMSSRMRealTimeStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', 'none');
                                    isDateValid = true;
                                }
                            }
                        }
                        break;
                    case '1':
                        errormsg = CompareDateFromTodaysDateUS(SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().val(), SMSSRMRealTimeStatusMethods.prototype._controls.LblStartDate().textNBR().replace(':', ''));
                        break;
                    case '2':
                        errormsg = CompareDateFromTodaysDateUS(SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().val(), SMSSRMRealTimeStatusMethods.prototype._controls.LblEndDate().textNBR().replace(':', ''));
                        break;
                }
                if (errormsg != '') {
                    SMSSRMRealTimeStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                    isDateValid = false;
                }
                else {
                    SMSSRMRealTimeStatusMethods.prototype._controls.LblValidationSummary().css('display', 'none');
                    isDateValid = true;
                }
                flagShowValidationMsg = !isDateValid;
            }
            if (!flagShowValidationMsg) {
                var tdExceptionDate = SMSSRMRealTimeStatusMethods.prototype._controls.DivExceptionDate().find("td[marked='1']");
                SMSSRMRealTimeStatusMethods.prototype._controls.HdnExceptionDate().val(tdExceptionDate.attr('value'));


                switch (tdExceptionDate.attr('value')) {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                        target.text(tdExceptionDate.textNBR());
                        SMSSRMRealTimeStatusMethods.prototype._controls.LblStartDate().text('Start Date : ').css('display', '');
                        SMSSRMRealTimeStatusMethods.prototype._controls.LblEndDate().text('End Date : ').css('display', '');
                        SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().css('display', ''); //.attr('title', 'Enter a Start Date')
                        SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().css('display', ''); //.attr('title', 'Enter an End Date')
                        SMSSRMRealTimeStatusMethods.prototype._controls.RblstDate().find("input[type='radio']").first().prop('checked', true);
                        break;
                    case '4':
                        var rdbDateChecked = SMSSRMRealTimeStatusMethods.prototype._controls.RblstDate().find("input[type='radio']:checked");
                        switch (rdbDateChecked.val()) {
                            case '0':
                                target.text(rdbDateChecked.next().textNBR() + ' ' + SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().val() + ' to ' + SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().val());
                                break;
                            case '1':
                                target.text(rdbDateChecked.next().textNBR() + ' ' + SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().val());
                                break;
                            case '2':
                                target.text(rdbDateChecked.next().textNBR() + ' ' + SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().val());
                                break;
                        }
                        break;
                }
                SMSSRMRealTimeStatusMethods.prototype._controls.DivExceptionDate().attr('isValid', '1');
                SMSSRMRealTimeStatusMethods.prototype._controls.DivExceptionDate().css('display', 'none');
                $("#ctl00_cphMain_Status_labeldateselectiondiv").removeClass('selectedOption');
                SMSSRMRealTimeStatusMethods.prototype._controls.DivExceptionDate().attr('state', '0');
            }
            else {
                SMSSRMRealTimeStatusMethods.prototype._controls.DivExceptionDate().attr('isValid', '0');
            }
            return flagShowValidationMsg;
        }
    },
    onClickBtnGetStatus: function onClickBtnGetStatus(e) {
        //DSStopTimer();
        if (SMSSRMRealTimeStatusMethods.prototype._controls.HdnExceptionDate().val() == '4') {
            var selectedValue = SMSSRMRealTimeStatusMethods.prototype._controls.RblstDate().find('[type="radio"]:checked').first().val();
            var errorCount = 0;
            var errormsg = '';
            var msg = '';
            var isvalid = 'true'
            switch (selectedValue) {
                case '0':
                    //  errormsg = SMSSRMRealTimeStatusMethods.prototype.DSCompareDateFromTodaysDate(SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().val(), SMSSRMRealTimeStatusMethods.prototype._controls.LblStartDate().textNBR().replace(':', ''));
                    errormsg = CompareDateFromTodaysDateUS(SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().val(), SMSSRMRealTimeStatusMethods.prototype._controls.LblStartDate().textNBR().replace(':', ''));

                    if (errormsg != '') {
                        SMSSRMRealTimeStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                        return false;
                    }
                    else {
                        errormsg = CompareDateFromTodaysDateUS(SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().val(), SMSSRMRealTimeStatusMethods.prototype._controls.LblEndDate().textNBR().replace(':', ''));
                        if (errormsg != '') {
                            SMSSRMRealTimeStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                            return false;
                        }
                        else {
                            errormsg = CompareDateUS(SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().val(), SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().val());
                            if (errormsg != '') {
                                SMSSRMRealTimeStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                                return false;
                            }
                            else {
                                SMSSRMRealTimeStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', 'none');
                                return true;
                            }
                        }
                    }
                    break;
                case '1':
                    errormsg = CompareDateFromTodaysDateUS(SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().val(), SMSSRMRealTimeStatusMethods.prototype._controls.LblStartDate().textNBR().replace(':', ''));
                    break;
                case '2':
                    errormsg = CompareDateFromTodaysDateUS(SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().val(), SMSSRMRealTimeStatusMethods.prototype._controls.LblEndDate().textNBR().replace(':', ''));
                    break;
            }
            if (errormsg != '') {
                SMSSRMRealTimeStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                return false;
            }
            else {
                SMSSRMRealTimeStatusMethods.prototype._controls.LblValidationSummary().css('display', 'none');
                return true;
            }
        }
    }
}

//function CompareDateUS(startdate, enddate, setserverdate) {
//    try {SMSSRMRealTimeStatusScreen
//        //check if start date is greator than end date;returns true if start date is greator than end date
//        var res = ExecuteSynchronously('./BaseUserControls/Service/Service.asmx', 'CompareDate', { startDate: startdate, endDate: enddate, setServerDate: setserverdate });
//        if (res.d) {
//            return '● Please enter End Date greater than or equal to Start Date\n ';
//        }
//        else {
//            return '';
//        }
//    }
//    catch (err) {
//        alert(ErrorMessage + 'CompareDate \n\n' + err.description);
//    }
//}



function SMSSRMRealTimeStatusScreen(securityInfo, controlIdInfo1) {
    SMSSRMRealTimeStatusMethods.prototype._securityInfo = eval("(" + securityInfo + ")");
    SMSSRMRealTimeStatusMethods.prototype._controlIdInfo = eval("(" + controlIdInfo1 + ")");
    SMSSRMRealTimeStatusMethods.prototype._controls = new SRMRealTimeStatusControls(SMSSRMRealTimeStatusMethods.prototype._controlIdInfo);

    //CREATE AND REMOVE HANDLERS
    function createHandlers() {
        //Added by Neetika
        //        if ($("#divFilter").length != 0)
        //            $("#divFilter").unbind('click').click(SMSSRMRealTimeStatusMethods.prototype.onClickDivFilter);

        //        if ($("#divExceptionDate").length != 0)
        //            $("#divExceptionDate").unbind('click').click(SMSSRMRealTimeStatusMethods.prototype.onClickDivExceptionDate);
        //        if (SMSSRMRealTimeStatusMethods.prototype._controls.RblstDate().length != 0)
        //            SMSSRMRealTimeStatusMethods.prototype._controls.RblstDate().unbind('click').click(SMSSRMRealTimeStatusMethods.prototype.onClickRblstDate);
    }
    //
    createHandlers();
}

function validateDates() {
    checked = $('[id$=hdnCustomRadioOption]').val();
    marked = $('[id$=hdnTopOption]').val();
    startDate = $('[id*=TextStartDate]').val();
    endDate = $('[id*=TextEndDate]').val();
    if (startDate == "")
        startDate = null;
    if (endDate == "")
        endDate = null;
    var errormsg = '';
    if (marked == '4') {
        switch (checked) {

            case '0': //From
                errormsg = CompareDateFromTodaysDateUS(SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().val(), 'Start Date');
                break;
            case '1':
                errormsg = CompareDateFromTodaysDateUS(SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().val(), 'Start Date');
                if (errormsg == '')
                    errormsg = CompareDateFromTodaysDateUS(SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().val(), 'End Date');
                if (errormsg == '')
                    errormsg = errormsg = CompareDateUS(SMSSRMRealTimeStatusMethods.prototype._controls.TxtStartDate().val(), SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().val());
                break;
            case '2':
                errormsg = CompareDateFromTodaysDateUS(SMSSRMRealTimeStatusMethods.prototype._controls.TxtEndDate().val(), 'End Date');
                break;
        }
    }
    return errormsg;
}
function initializeDivFilterDate() {
    var obj = {};
    obj.dateOptions = [0, 1, 2, 3, 4];
    obj.dateFormat = 'd/m/Y';
    obj.lefttimePicker = true;
    obj.righttimePicker = false;
    obj.calenderType = 0;
    obj.calenderID = 'smdd_0';
    obj.pivotElement = $('[id$=labeldateselectiondiv]');
    if ($('[id$=hdnStartDateCalender]').val() == '') {
        obj.StartDateCalender = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
        obj.EndDateCalender = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
    }
    else {
        obj.StartDateCalender = $('[id$=hdnStartDateCalender]').val();
        obj.EndDateCalender = $('[id$=hdnEndDateCalender]').val();
    }
    if ($('[id$=hdnTopOption]').val() == -1) {
        obj.selectedTopOption = 0; //Today

    }
    else
        obj.selectedTopOption = $('[id$=hdnTopOption]').val();

    obj.selectedCustomRadioOption = $('[id$=hdnCustomRadioOption]').val();
    obj.applyCallback = function () {

        // $("#smdd_1").css('display', 'none');
        var modifiedText = smdatepickercontrol.getResponse($("#smdd_0"));
        var htmlString = "";
        var prepString = "";

        var selectedEndDate = modifiedText.selectedEndDate;
        var selectedStartDate = modifiedText.selectedStartDate;
        var selectedText = modifiedText.selectedText;
        var selectedDateOption = modifiedText.selDateOption;
        var selectedRadioOption = modifiedText.selRadioOption;

        if (selectedText != undefined && selectedText.toUpperCase() === "ANYTIME") {
            var serverEndDate = null;
            var serverStartDate = null;
        }
        else {

            var serverEndDate = modifiedText.serverEndDateOriginal;
            var serverStartDate = modifiedText.serverStartDateLongDate;
        }

        if (selectedStartDate != null) {
            selectedStartDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(selectedStartDate), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
            $('[id$=hdnStartDateCalender]').val(selectedStartDate);
        } if (selectedEndDate != null) {
            selectedEndDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(selectedEndDate), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
            $('[id$=hdnEndDateCalender]').val(selectedEndDate);
        }

        if (selectedText != undefined) {
            if (selectedText.toUpperCase() === "TODAY")
                htmlString = "Today";
            else if (selectedText.toUpperCase() === "SINCE YESTERDAY")
                htmlString = "Yesterday";
            else if (selectedText.toUpperCase() === "THIS WEEK")
                htmlString = "This Week";
            else if (selectedText.toUpperCase() === "ANYTIME")
            { prepString = ""; htmlString = "Anytime"; }
            else if (selectedText.toUpperCase() === "CUSTOM DATE") {
                if (selectedRadioOption == 0)
                    htmlString = " after " + selectedStartDate;
                else if (selectedRadioOption == 1)
                    htmlString = " between " + selectedStartDate + " to " + selectedEndDate;
                else if (selectedRadioOption == 2)
                    htmlString = " before " + selectedEndDate;
            }
            $('[id$=hdnTopOption]').val(selectedDateOption);
            $('[id$=hdnCustomRadioOption]').val(selectedRadioOption);
            $('[id$=hdnFirstTime]').val('0');
            $('[id$=labeldateselectiondiv]').text(htmlString);
            //   SMSDownstreamSystemStatusMethods.prototype._controls.HdnExceptionDate().val(selectedDateOption);
            $('[id*=TextStartDate]').val(serverStartDate);
            $('[id*=TextEndDate]').val(serverEndDate);
        }
        var errormsg = validateDates();

        if (errormsg == '') {
            //formUrlData();
            //var errormsg = validateDates();
            //if (errormsg == '') {

            onUpdating();
            $('#smdd_0').find('.validationSummary').text('');

            var dateString = '&startdate=' + startDate + '&enddate=' + endDate + '&marked=' + marked + '&checked=' + checked;
            CommonModuleTabs.callbackObj[CommonModuleTabs.currentSelected](dateString);

            ////CommonModuleTabs.callbackObj[selectedItem.name.toLowerCase()]();

            //}
            //else { $('#smdd_0').find('.validationSummary').text(errormsg); }
        }

        return errormsg;
    }
    obj.ready = function (e) {
    }
    var initObj = smdatepickercontrol.init(obj);
    $('[id*=TextStartDate]').val(initObj.serverStartDateLongDate);
    $('[id*=TextEndDate]').val(initObj.serverEndDateOriginal);
}
$(document).click(function (e) {

    SMSSRMRealTimeStatusMethods.prototype.toggleFilters(e);

});

$(function () {
    $('.InnerFrame[name=TabsInnerIframe1]').load(function () {


        onUpdated();
    });
    //$('.InnerFrame[name=TabsInnerIframe2]').load(function () {


    //    onUpdated();
    //});

});





$(document).ready(function () {

    initializeDivFilterDate();


    //    adjustCSS()
    //    {
    //        var width = $(window).width();
    //        if(width >= 1680)
    //            $(".tabClass").attr("width", "25%");
    //        if (width >= 1366)
    //            $(".tabClass").attr("width", "25%");
    //    }
    //    adjustCSS();
    var lst;
    var statusfilter;
    var system;
    var taskstatus;
    var date;
    var startDate, endDate;
    //  var arr1 = [];
    var obj1 = {};
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    CallCustomDatePicker($('[id*="TextStartDate"]').prop('id').trim(), SMSSRMRealTimeStatusMethods.prototype._securityInfo.CustomJQueryDateTimeFormat, null, optionDateTime.DATETIME, 15, true);
    CallCustomDatePicker($('[id*="TextEndDate"]').prop('id').trim(), SMSSRMRealTimeStatusMethods.prototype._securityInfo.CustomJQueryDateFormat, null, optionDateTime.DATE, 15, true);

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });


    //
    //}


    initialiseTabs();


    $.fn.textNBR = function () {
        var regEx = new RegExp(String.fromCharCode(160), 'gi');
        var normalSpace = ' ';
        var tempText = this.text().replace(regEx, normalSpace);
        //var result = new RegExp(String.fromCharCode(160), 'gi').test(tempText);
        return tempText;
    };



    //$("#tab1").click(function () {
    //    $('[id$="lblRunningText"]').text("Status for securities requested");
    //    $('#container2').css('display', 'none');

    //    $('#container1').css('display', 'block');
    //    var grid = $('#InnerFrameId1')[0].contentWindow.document.getElementById('ctl00_cphMain_BackgroundSecurityStatus_gridRealTimeSecurityStatus');
    //    if (grid != null)
    //        //     $('#InnerFrameId1')[0].contentWindow.$find('ctl00_cphMain_BackgroundSecurityStatus_gridExternalSystemStatus1').refreshWithCache();
    //        $('#InnerFrameId1')[0].contentWindow.$find('ctl00_cphMain_BackgroundSecurityStatus_gridRealTimeSecurityStatus').refreshWithCache();
    //    $('#tab2').removeClass('TabscssAfter');

    //    $('#tab1').addClass('TabscssAfter');


    //});

    //$("#tab2").click(function () {
    //    $('[id$="lblRunningText"]').text("Status for entities requested");
    //    $('#container1').css('display', 'none');
    //    $('#container2').css('display', 'block');
    //    $('#tab1').removeClass('TabscssAfter');

    //    $('#tab2').addClass('TabscssAfter');
    //});


    //$("#tab1").trigger('click');






    $('#InnerFrameId1').load(function () {
        $(this).contents().click(function (e) {

            $('body').find('#ctl00_cphMain_Status_TextStartDate_ContainerDivDateTimePicker').hide();
            $('body').find('#ctl00_cphMain_Status_TextEndDate_ContainerDivDateTimePicker').hide();
            $('body').find('.smselectcon').css('display', 'none')
            SMSSRMRealTimeStatusMethods.prototype.toggleFilters(e);

        });
    });


    changeDropDown: function changeDropDown(targetId) {
        var obj = new Object();
        obj.select = $(targetId);
        obj.showSearch = false;
        obj.isRunningText = true;
        obj.ready = function (smselect) {
            $(smselect).on('clicked', function (e) {
                $("#divExceptionDate").attr('state', '0');
                $("#divExceptionDate").hide();
                e.stopPropagation();
            });
        }
        smselect.create(obj);
    }



    function getSelectedValue(e, target, filter) {
        var obj = new Object();
        obj.select = target;
        obj.showSearch = false;
        obj.isRunningText = true;
        obj.ready = function (e) {
            e.width('95%');
            e.on('change', function (ee) {

                lst = smselect.getSelectedOption(e);
                obj1[filter] = lst[0]["value"];
                // arr1.push(obj1);
                //(smselect.getSelectedOption(e))[0]["value"]
            });

        }
        smselect.create(obj);
    }




    $('#ctl00_cphMain_Status_labeldateselectiondiv').css({ 'text-decoration': 'none' });


    //Apply Button 

    $('[id$=btnGetStatus]').click(function () {
        //formUrlData();
        //var errormsg = validateDates();
        //if (errormsg == '') {

        //    onUpdating();
        //    $('#smdd_0').find('.validationSummary').text('');

        //    var dateString = '&startdate=' + startDate + '&enddate=' + endDate + '&marked=' + marked + '&checked=' + checked;
        //    CommonModuleTabs.callbackObj[CommonModuleTabs.currentSelected](dateString);

        //    //CommonModuleTabs.callbackObj[selectedItem.name.toLowerCase()]();

        //}
        //else { $('#smdd_0').find('.validationSummary').text(errormsg); }

    });

    //DownArrow along Date Filter in Tabs Div at top
    $("#RealTimeDateFilterDownArrow").click(function (e) {
        $('[id$=labeldateselectiondiv]').click();
        e.stopPropagation();
    });

    //Refresh Button in Tabs Div at top
    $("#RealTimeStatusRefreshButton").click(function (e) {
        $('[id$=labeldateselectiondiv]').click();
        //e.stopPropagation();
    });

    function formUrlData() {
        var longDateFormat = $('[id$=hdnLongDateFormat]').val();
        checked = $('[id$=hdnCustomRadioOption]').val();
        marked = $('[id$=hdnTopOption]').val();
        startDate = $('[id*=TextStartDate]').val();
        endDate = $('[id*=TextEndDate]').val();
        if (startDate == "")
            startDate = null;
        if (endDate == "")
            endDate = null;

        //        if (obj1.length != undefined) {
        //            statusfilter = obj1.statusfilter;
        //            system = obj1.sytem;
        //            taskstatus = obj1.taskstatus
        //        }
        //        else {
        //            system = $('#smselect_ctl00_cphMain_Status_ddlSystems').find('.smselected').attr('data-value');
        //            statusfilter = $('#smselect_ctl00_cphMain_Status_ddlStatusFilter').find('.smselected').attr('data-value');
        //            taskstatus = $('#smselect_ctl00_cphMain_Status_ddlTaskStatus').find('.smselected').attr('data-value');
        //        }

        //        var date;
        //        date = $('#ctl00_cphMain_Status_hdnExceptionDate').val();
        //        var weekDateFilter = new Date().setHours(0, 0, 0, 0);
        //        var weekDateFilter = new Date(weekDateFilter);
        //        var lastWeek = weekDateFilter.getDate() - (weekDateFilter.getDay() - 1);
        //        weekDateFilter.setDate(lastWeek);
        //        weekDateFilter = weekDateFilter.format(longDateFormat);

        //        var todayFilter = new Date();
        //        todayFilter = todayFilter.format(longDateFormat);

        //        var yesterdayFilter = new Date().setHours(0, 0, 0, 0);
        //        var yesterday = new Date(yesterdayFilter);
        //        yesterday.setDate(yesterday.getDate() - 1);
        //        yesterdayFilter = yesterday.format(longDateFormat);

        //        if (date == '0') {
        //            var x = new Date().setHours(0, 0, 0, 0);
        //            var y = new Date(x);
        //            y = y.format(longDateFormat)

        //            startDate = y
        //            endDate = todayFilter;
        //            marked = "0";
        //            checked = "0";
        //        }
        //        else if (date == '1') {
        //            startDate = yesterdayFilter;
        //            endDate = todayFilter;
        //            marked = "1";
        //            checked = "0";
        //        }
        //        else if (date == '2') {
        //            startDate = weekDateFilter;
        //            endDate = todayFilter;
        //            marked = "2";
        //            checked = "0";
        //        }
        //        else if (date == '3') {
        //            startDate = null;
        //            endDate = todayFilter;
        //            marked = "3";
        //            checked = "2";
        //        }
        //        else if (date == '4') {
        //            marked = "4";
        //            if ($("#ctl00_cphMain_Status_rblstDate_0:checked").length == '1') {
        //                startDate = $('#ctl00_cphMain_Status_TextStartDate').val();
        //                endDate = $('#ctl00_cphMain_Status_TextEndDate').val();
        //                checked = "0";
        //            }
        //            else if ($("#ctl00_cphMain_Status_rblstDate_1:checked").length == '1') {
        //                startDate = $('#ctl00_cphMain_Status_TextStartDate').val();
        //                endDate = null;
        //                checked = "1";
        //            }
        //            else if ($("#ctl00_cphMain_Status_rblstDate_2:checked").length == '1') {
        //                startDate = null;
        //                endDate = $('#ctl00_cphMain_Status_TextEndDate').val();
        //                checked = "2";
        //            }
        //        }
    }


    function setStatusScreenHeight() {
        var height = $(window).height() - $('#container1').offset().top - 30;
        $('.InnerFrame').height(height);
    }


    setStatusScreenHeight();
});

//function initialiseTabs() {

//    //$('.InnerFrame[name=TabsInnerIframe1]').attr('src', path + '/../SMRealTimeSecurityStatus.aspx?identifier=BackgroundSecurityStatus&RealTimestatusscreen=1');
//    //    $('.InnerFrame[name=TabsInnerIframe2]').attr('src', path + '/../RMHomeInternal.aspx?identifier=RefM_RealTimebackGroundStatus&RealTimestatusscreen=1');


//    //var initiateTabs = [];
//    //same function 

//    //SRMProductTabs.setCallback({ key: CommonModuleTabs.ModuleList.refmaster.displayName, value: overrideStatus.refMasterInitOverrideStatusScreen });

//    var listofTabsToBindFunctionsWith = CommonModuleTabs.ModuleList();
//    var prodName = SMSSRMRealTimeStatusMethods.prototype._controls._controlIdInfo.ProductName;

//    getViewPrivileges().then(function () {
//        if (prodName.toLowerCase() == "refmaster") {
//            for (i in listofTabsToBindFunctionsWith) {
//                item = listofTabsToBindFunctionsWith[i];
//                if (item.displayName.toLowerCase().trim() == "refmaster" && isRealTimeStatusViewAllowedInRefM)
//                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initiateRef });

//            }
//        }
//            //secmaster
//        else {
//            for (i in listofTabsToBindFunctionsWith) {
//                item = listofTabsToBindFunctionsWith[i];
//                if (item.displayName.toLowerCase().trim() == "secmaster" && isRealTimeStatusViewAllowedInSecM)
//                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initiateSec });
//                else if (item.displayName.toLowerCase().trim() == "refmaster" && isRealTimeStatusViewAllowedInRefM)
//                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initiateRef });

//            }
//        }
//    });


//    //initiateTabs.push({ key: CommonModuleTabs.ModuleList.secmaster.displayName, value: initiateSec });
//    //initiateTabs.push({ key: CommonModuleTabs.ModuleList.refmaster.displayName, value: initiateRef });

//    //initialising both
//    //SRMProductTabs.setAllCallback(initiateTabs);

//    //SRMProductTabs.setCallback({ key: "secmaster", value: initiateSec });
//    //SRMProductTabs.setCallback({ key: "refmaster", value: initiateRef });


////}

//function initialiseTabs() {

//    var listofTabsToBindFunctionsWith = CommonModuleTabs.ModuleList();
//    var prodName = SMSSRMRealTimeStatusMethods.prototype._controls._controlIdInfo.ProductName;

//    for (i in listofTabsToBindFunctionsWith) {
//        item = listofTabsToBindFunctionsWith[i];
//        switch (item.displayName.toLowerCase().trim()) {
//            case "secmaster":
//                SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initiateSec });
//                //draftsStatus._moduleID_moduleName_map["item.displayName.toLowerCase()"] = item.moduleId;
//                break;
//            case "refmaster":
//                SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initiateRef });
//                break;
//                //case "fundmaster":
//                //    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: draftsStatus.fundMasterInitDraftsStatusScreen });
//                //    break;
//                //case "partymaster":
//                //    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: draftsStatus.partyMasterInitDraftsStatusScreen });
//                //    break;
//        }
//    }

//}

var _moduleID_moduleName_map = [];
function initialiseTabs() {

    var listofTabsToBindFunctionsWith = CommonModuleTabs.ModuleList();
    //var prodName = SMSSRMDownstreamStatusMethods.prototype._controls._controlIdInfo.ProductName;

    for (i in listofTabsToBindFunctionsWith) {
        item = listofTabsToBindFunctionsWith[i];
        _moduleID_moduleName_map[item.displayName.toLowerCase()] = item.moduleId;
        switch (item.displayName.toLowerCase().trim()) {
            case "securities":
                SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initiateSec });
                //draftsStatus._moduleID_moduleName_map["item.displayName.toLowerCase()"] = item.moduleId;
                break;
            case "refdata":
                SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initiateRef });
                break;
            case "funds":
                SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initiateFund });
                break;
            case "parties":
                SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initiateParty });
                break;

        }
    }

}


function dummyFunction() {
    alert("works");
}

function datesQueryString() {
    var queryStringDates = '';

    startDate = $('[id*=TextStartDate]').val();
    endDate = $('[id*=TextEndDate]').val();
    checked = $('[id$=hdnCustomRadioOption]').val();
    marked = $('[id$=hdnTopOption]').val();
    if (startDate == "")
        startDate = null;
    if (endDate == "")
        endDate = null;

    if (marked == "4" || marked == 4) {
        //from
        if (checked == "0" || checked == 0) {
            endDate = null;
        }
            //prior
        else if (checked == "2" || checked == 2) {
            startDate = null;
        }
    }

    queryStringDates = "&startdate=" + (startDate == null ? "null" : startDate);
    queryStringDates += "&enddate=" + (endDate == null ? "null" : endDate);
    queryStringDates += "&checked=" + checked;
    queryStringDates += "&marked=" + marked;

    return queryStringDates;
}

function initiateSec() {
    var pathSecmaster = path + '/../SMRealTimeSecurityStatus.aspx?identifier=BackgroundSecurityStatus&RealTimestatusscreen=1';

    //if ((dateFilters != null || dateFilters != undefined) && typeof (dateFilters) == "string") {
    pathSecmaster += datesQueryString();
    //}
    var typeId = $('[id$=hdnTypeId]').val();

    if (typeof (typeId) != 'undefined' && typeId != null && typeId != "")
        pathSecmaster += '&typeId=' + typeId;

    $('[id$="lblRunningText"]').text("Status for securities requested");
    //$('#container2').css('display', 'none');

    //$('#container1').css('display', 'block');
    var grid = $('#InnerFrameId1')[0].contentWindow.document.getElementById('ctl00_cphMain_BackgroundSecurityStatus_gridRealTimeSecurityStatus');
    if (grid != null)
        $('#InnerFrameId1')[0].contentWindow.$find('ctl00_cphMain_BackgroundSecurityStatus_gridRealTimeSecurityStatus').refreshWithCache();


    if ($("#isPostBack").val() === "False") {
        onUpdating();
        $('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathSecmaster);
        //    $('.InnerFrame[name=TabsInnerIframe2]').attr('src', path + '/../RMHomeInternal.aspx?identifier=RefM_RealTimebackGroundStatus&RealTimestatusscreen=1');
    }
}

function initiateRef() {
    var pathRefMaster = path + '/../RMHomeInternal.aspx?identifier=RefM_RealTimebackGroundStatus&RealTimestatusscreen=1';
    //if ((dateFilters != null || dateFilters != undefined) && typeof (dateFilters) == "string") {
    pathRefMaster += datesQueryString();
    //}
    var typeId = $('[id$=hdnTypeId]').val();

    if (typeof (typeId) != 'undefined' && typeId != null && typeId != "")
        pathRefMaster += '&typeId=' + typeId;
    //var pathRefMaster = "App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?identifier=RefM_RealTimebackGroundStatus";
    $('[id$="lblRunningText"]').text("Status for entities requested");

    if ($("#isPostBack").val() === "False") {
        onUpdating();
        //$('.InnerFrame[name=TabsInnerIframe2]').attr('src', path + '/../RMHomeInternal.aspx?identifier=RefM_RealTimebackGroundStatus&RealTimestatusscreen=1');
        pathRefMaster += "&ModuleID=" + _moduleID_moduleName_map["refdata"];
        $('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathRefMaster);


    }

}

function initiateFund(dateFilters) {
    var pathRefMaster = path + '/../RMHomeInternal.aspx?identifier=RefM_RealTimebackGroundStatus&RealTimestatusscreen=1';
    //if ((dateFilters != null || dateFilters != undefined) && typeof (dateFilters) == "string") {
    pathRefMaster += datesQueryString();
    var typeId = $('[id$=hdnTypeId]').val();

    if (typeof (typeId) != 'undefined' && typeId != null && typeId != "")
        pathRefMaster += '&typeId=' + typeId;
    //}
    //var pathRefMaster = "App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?identifier=RefM_RealTimebackGroundStatus";
    $('[id$="lblRunningText"]').text("Status for entities requested");

    if ($("#isPostBack").val() === "False") {
        onUpdating();
        //$('.InnerFrame[name=TabsInnerIframe2]').attr('src', path + '/../RMHomeInternal.aspx?identifier=RefM_RealTimebackGroundStatus&RealTimestatusscreen=1');
        pathRefMaster += "&ModuleID=" + _moduleID_moduleName_map["funds"];
        $('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathRefMaster);


    }

}

function initiateParty(dateFilters) {
    var pathRefMaster = path + '/../RMHomeInternal.aspx?identifier=RefM_RealTimebackGroundStatus&RealTimestatusscreen=1';
    //if ((dateFilters != null || dateFilters != undefined) && typeof (dateFilters) == "string") {
    pathRefMaster += datesQueryString();
    var typeId = $('[id$=hdnTypeId]').val();

    if (typeof (typeId) != 'undefined' && typeId != null && typeId != "")
        pathRefMaster += '&typeId=' + typeId;
    //}
    //var pathRefMaster = "App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?identifier=RefM_RealTimebackGroundStatus";
    $('[id$="lblRunningText"]').text("Status for entities requested");

    if ($("#isPostBack").val() === "False") {
        onUpdating();
        //$('.InnerFrame[name=TabsInnerIframe2]').attr('src', path + '/../RMHomeInternal.aspx?identifier=RefM_RealTimebackGroundStatus&RealTimestatusscreen=1');
        pathRefMaster += "&ModuleID=" + _moduleID_moduleName_map["parties"];
        $('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathRefMaster);


    }

}


var commonServiceLocation = '/BaseUserControls/Service/CommonService.svc';
var isRealTimeStatusViewAllowedInRefM = false;
var isRealTimeStatusViewAllowedInSecM = false;

function getViewPrivileges() {
    return new Promise(function (res, rej) {
        var promiseArr = [];
        var prodName = SMSSRMRealTimeStatusMethods.prototype._controls._controlIdInfo.ProductName;
        var userName = SMSSRMRealTimeStatusMethods.prototype._controls._controlIdInfo.UserName;
        if (prodName.toLowerCase() == "refmaster") {
            var p1 = new Promise(function (resolve, reject) {
                $.ajax({
                    type: 'POST',
                    url: path + commonServiceLocation + "/CheckControlPrivilegeForUser",
                    contentType: "application/json",
                    dataType: "json",
                    data: JSON.stringify({
                        "privilegeName": "topMenu - Real Time Request Status",
                        "userName": userName
                    }),
                    success: function (data) {
                        data = data.d;
                        isRealTimeStatusViewAllowedInRefM = data;
                        resolve();
                    },
                    error: function (ex) {
                        console.log("Real Time Status Privilege cannot be fetched");
                        reject(ex);
                    }
                });
            });
            promiseArr.push(p1);
        }

        else if (prodName.toLowerCase() == "secmaster") {
            var p1 = new Promise(function (resolve, reject) {
                $.ajax({
                    type: 'POST',
                    url: path + commonServiceLocation + "/CheckControlPrivilegeForUser",
                    contentType: "application/json",
                    dataType: "json",
                    data: JSON.stringify({
                        "privilegeName": "topMenu - Real Time Request Status",
                        "userName": userName
                    }),
                    success: function (data) {
                        data = data.d;
                        isRealTimeStatusViewAllowedInRefM = data;
                        resolve();
                    },
                    error: function (ex) {
                        console.log("Real Time Status Privilege cannot be fetched");
                        reject(ex);
                    }
                });
            });
            promiseArr.push(p1);

            var p2 = new Promise(function (resolve, reject) {
                $.ajax({
                    type: 'POST',
                    url: path + commonServiceLocation + "/CheckControlPrivilegeForUser",
                    contentType: "application/json",
                    dataType: "json",
                    data: JSON.stringify({
                        "privilegeName": "Real Time Security Status",
                        "userName": userName
                    }),
                    success: function (data) {
                        data = data.d;
                        isRealTimeStatusViewAllowedInSecM = data;
                        resolve();
                    },
                    error: function (ex) {
                        console.log("Real Time Status Privilege cannot be fetched");
                        reject(ex);
                    }
                });
            });

            promiseArr.push(p2);

        }


        Promise.all(promiseArr)
        .then(function () { res(); })
        .catch(function (ex) { rej(ex); });
    });
}