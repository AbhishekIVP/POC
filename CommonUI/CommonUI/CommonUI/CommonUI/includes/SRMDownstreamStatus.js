function SRMDownstreamStatusControls(controlIdInfo) {
    this._controlIdInfo = controlIdInfo;
}

SRMDownstreamStatusControls.prototype = {
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
function SMSSRMDownstreamStatusMethods() {
}

SMSSRMDownstreamStatusMethods.prototype = {
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
                    SMSSRMDownstreamStatusMethods.prototype._controls.LblStartDate().text('Start Date : ').css('display', '');
                    SMSSRMDownstreamStatusMethods.prototype._controls.LblEndDate().text('End Date : ').css('display', '');
                    SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().attr('title', 'Enter a Start Date').css('display', '');
                    SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().attr('title', 'Enter an End Date').css('display', '');
                    break;
                case '1':
                    SMSSRMDownstreamStatusMethods.prototype._controls.LblStartDate().text('From Date : ').css('display', '');
                    SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().attr('title', 'Enter From Date').css('display', '');
                    SMSSRMDownstreamStatusMethods.prototype._controls.LblEndDate().text('').css('display', 'none');
                    SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().attr('title', '').css('display', 'none');
                    break;
                case '2':
                    SMSSRMDownstreamStatusMethods.prototype._controls.LblStartDate().text('').css('display', 'none');
                    SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().attr('title', '').css('display', 'none');
                    SMSSRMDownstreamStatusMethods.prototype._controls.LblEndDate().text('Prior To Date : ').css('display', '');
                    SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().attr('title', 'Enter Prior To Date').css('display', '');
                    break;
                default:
                    break;
                    SMSSRMDownstreamStatusMethods.prototype._controls.LblStartDateError().css('display', 'none');
                    SMSSRMDownstreamStatusMethods.prototype._controls.LblEndDateError().css('display', 'none');
                    SMSSRMDownstreamStatusMethods.prototype._controls.LblValidationSummary().text('');
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

                    case $("#ctl00_SRMContentPlaceHolderMiddle_labeldateselectiondiv").attr('id'):
                        SMSSRMDownstreamStatusMethods.prototype.showHideExceptionDate(e);
                        break;

                }
            }
            //            else if (target[0].tagName.toUpperCase() == 'INPUT' && SecMasterJSCommon.SMSCommons.contains(target.attr('id'), SMSSRMDownstreamStatusMethods.prototype._controls.btnGetStatus().attr('id'))) {
            //                SMSSRMDownstreamStatusMethods.prototype.toggleFilters(e);
            //                var inValidDivs = SMSSRMDownstreamStatusMethods.prototype._controls.DivFilterDDLs().find("div[isValid='0']");
            //                if (inValidDivs.length > 0) {
            //                    doPostback = false;
            //                    SMSSRMDownstreamStatusMethods.prototype.isTargetForPostback = false;
            //                }
            //                else {
            //                    doPostback = true;
            //                    SMSSRMDownstreamStatusMethods.prototype.isTargetForPostback = true;
            //                }

            //                return doPostback;
            //            }
        }
    },
    onClickDivExceptionDate: function onClickDivExceptionDate(e) {
        //alert("ll");
        e.stopPropagation();
        SMSSRMDownstreamStatusMethods.prototype.selectDivExceptionDate(e.target);
        if ((e.target.id == SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().prop('id')) && (SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().attr('isClicked') == 0))
            SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().attr('isClicked', '1');
        if ((e.target.id == SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().prop('id')) && (SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().attr('isClicked') == 0))
            SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().attr('isClicked', '1');

        else if (SMSSRMDownstreamStatusMethods.prototype._controls.HdnExceptionDate().val() == '4') {
            if (!(e.target.id == SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().prop('id')) || (e.target.id == SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().prop('id')) || (SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().attr('isclicked') != 0)) {
                if (SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().attr('isClicked') == 1) {
                    SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().customDateTimePicker('hide');
                    SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().attr('isClicked', '0');
                }
            }
            if ((!(e.target.id == SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().prop('id')) || (e.target.id == SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().prop('id')) || (SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().attr('isclicked') != 0))) {
                if (SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().attr('isClicked') == 1) {
                    SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().customDateTimePicker('hide');
                    SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().attr('isClicked', '0');
                }
            }
        }
        else {
            if (SMSSRMDownstreamStatusMethods.prototype._controls.HdnExceptionDate().val() != '4')
                SMSSRMDownstreamStatusMethods.prototype.showHideExceptionDate(e);


        }

    },
    selectDivExceptionDate: function selectDivExceptionDate(element) {
        var target = $(element);
        if (SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().attr('isClicked') == 0 && (SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().attr('isClicked') == 0)) {
            if (target[0].tagName.toUpperCase() == 'TR')
                target = target.find('td:first');
            if (target[0].tagName.toUpperCase() == 'TD') {
                SMSSRMDownstreamStatusMethods.prototype._controls.HdnExceptionDate().val(target.attr('value'));
                if ($('[id$=hdnUrl]').val() == '') {
                    $('#divExceptionDate tr').css('color', 'rgb(0,0,0)');
                    $('#divExceptionDate tr:eq(' + SMSSRMDownstreamStatusMethods.prototype._controls.HdnExceptionDate().val() + ')').css('color', '#48a3dd');
                }
                var table = target.parents('table:first');
                table.find("td[marked='1']").each(function (index) {
                    $(this).attr('marked', "0");
                });
                target.attr('marked', '1');
                if ((SMSSRMDownstreamStatusMethods.prototype._controls.HdnExceptionDate().val() == '4'))
                    SMSSRMDownstreamStatusMethods.prototype._controls.TrExceptionDateCustom().css('display', '');
                else {
                    SMSSRMDownstreamStatusMethods.prototype._controls.TrExceptionDateCustom().css('display', 'none');

                }
            }

        }
    },
    toggleFilters: function toggleFilters(e) {
        var activeDiv = SMSSRMDownstreamStatusMethods.prototype._controls.DivFilterDDLs().find("div[state='1']");
        var flagToggle = true;
        if (activeDiv.length > 0) {
            switch (activeDiv.attr('id')) {

                case 'divExceptionDate':
                    flagToggle = !SMSSRMDownstreamStatusMethods.prototype.showHideExceptionDate(e);
                    break;

            }
        }
        return flagToggle;
    },

    showHideExceptionDate: function showHideExceptionDate(e) {
        var target = $("#ctl00_SRMContentPlaceHolderMiddle_labeldateselectiondiv");
        //        var selectedExceptionDate = objException._controls.DivExceptionDate().find("tbody > tr:lt(5)");
        //        for (var i = 0; i < selectedExceptionDate.length; i++) {
        //            if ($(selectedExceptionDate[i]).find("td").text().trim() == target.text()) {
        //                
        //            }
        //        }

        if (SMSSRMDownstreamStatusMethods.prototype._controls.DivExceptionDate().attr('state') == '0' && SMSSRMDownstreamStatusMethods.prototype.toggleFilters(e)) {
            if ($('[id$=hdnUrl]').val() != '')
                SMSSRMDownstreamStatusMethods.prototype._controls.HdnExceptionDate().val("3");
            SMSSRMDownstreamStatusMethods.prototype.selectDivExceptionDate(SMSSRMDownstreamStatusMethods.prototype._controls.DivExceptionDate().find("td[value='" + SMSSRMDownstreamStatusMethods.prototype._controls.HdnExceptionDate().val() + "']")[0]);

            if ((SMSSRMDownstreamStatusMethods.prototype._controls.DivExceptionDate()).is(":hidden")) {
                (SMSSRMDownstreamStatusMethods.prototype._controls.DivExceptionDate()).show();
                (SMSSRMDownstreamStatusMethods.prototype._controls.DivExceptionDate()).attr('state', '1');
                var obj = new Object();
                obj.select = $("#ctl00_SRMContentPlaceHolderMiddle_ddlTaskStatus");
                smselect.hide($('body').find("#ctl00_SRMContentPlaceHolderMiddle_ddlTaskStatus").next());
                var obj = new Object();
                obj.select = $("#ctl00_SRMContentPlaceHolderMiddle_ddlStatusFilter");
                smselect.hide($('body').find("#ctl00_SRMContentPlaceHolderMiddle_ddlStatusFilter").next());
                var obj = new Object();
                obj.select = ($('body').find("#ctl00_SRMContentPlaceHolderMiddle_ddlSystems"));
                smselect.hide($('body').find("#ctl00_SRMContentPlaceHolderMiddle_ddlSystems").next());
                smselect.hide($('body').find("#ctl00_SRMContentPlaceHolderMiddle_ddlStatusFilter").next());
                smselect.hide($('body').find("#ctl00_SRMContentPlaceHolderMiddle_ddlSystems").next());
            }
            else {
                (SMSSRMDownstreamStatusMethods.prototype._controls.DivExceptionDate()).hide();
                (SMSSRMDownstreamStatusMethods.prototype._controls.DivExceptionDate()).attr('state', '0');
            }
            // SMSSRMDownstreamStatusMethods.prototype._controls.DivExceptionDate().slideToggle("slow");
            //objException._controls.DivExceptionDate().css('display', ''); //[0].className = 'ExceptionDateDropDownsVisible';            
            $("#ctl00_SRMContentPlaceHolderMiddle_labeldateselectiondiv").addClass('selectedOption');
            var positionTop = ($("#ctl00_SRMContentPlaceHolderMiddle_labeldateselectiondiv").outerHeight() + $("#ctl00_SRMContentPlaceHolderMiddle_labeldateselectiondiv").offset().top + 3) + 'px';
            var positionLeft = $("#ctl00_SRMContentPlaceHolderMiddle_labeldateselectiondiv").offset().left;
            SMSSRMDownstreamStatusMethods.prototype._controls.DivExceptionDate().css('top', positionTop);
            SMSSRMDownstreamStatusMethods.prototype._controls.DivExceptionDate().css('left', positionLeft);
            SMSSRMDownstreamStatusMethods.prototype._controls.DivExceptionDate().attr('state', '1');
        }
        else {
            var flagShowValidationMsg = false;
            if (SMSSRMDownstreamStatusMethods.prototype._controls.HdnExceptionDate().val() == '4') {
                var selectedValue = SMSSRMDownstreamStatusMethods.prototype._controls.RblstDate().find("input[type='radio']:checked").first().val();
                var errormsg = '';
                var isDateValid = true;
                switch (selectedValue) {
                    case '0':
                        errormsg = CompareDateFromTodaysDateUS(SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().val(), SMSSRMDownstreamStatusMethods.prototype._controls.LblStartDate().textNBR().replace(':', ''));
                        if (errormsg != '') {
                            SMSSRMDownstreamStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                            isDateValid = false;
                        }
                        else {
                            errormsg = CompareDateFromTodaysDateUS(SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().val(), SMSSRMDownstreamStatusMethods.prototype._controls.LblEndDate().textNBR().replace(':', ''));
                            if (errormsg != '') {
                                SMSSRMDownstreamStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                                isDateValid = false;
                            }
                            else {
                                errormsg = CompareDateUS(SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().val(), SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().val());
                                if (errormsg != '') {
                                    SMSSRMDownstreamStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                                    isDateValid = false;
                                }
                                else {
                                    SMSSRMDownstreamStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', 'none');
                                    isDateValid = true;
                                }
                            }
                        }
                        break;
                    case '1':
                        errormsg = CompareDateFromTodaysDateUS(SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().val(), SMSSRMDownstreamStatusMethods.prototype._controls.LblStartDate().textNBR().replace(':', ''));
                        break;
                    case '2':
                        errormsg = CompareDateFromTodaysDateUS(SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().val(), SMSSRMDownstreamStatusMethods.prototype._controls.LblEndDate().textNBR().replace(':', ''));
                        break;
                }
                if (errormsg != '') {
                    SMSSRMDownstreamStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                    isDateValid = false;
                }
                else {
                    SMSSRMDownstreamStatusMethods.prototype._controls.LblValidationSummary().css('display', 'none');
                    isDateValid = true;
                }
                flagShowValidationMsg = !isDateValid;
            }
            if (!flagShowValidationMsg) {
                var tdExceptionDate = SMSSRMDownstreamStatusMethods.prototype._controls.DivExceptionDate().find("td[marked='1']");
                SMSSRMDownstreamStatusMethods.prototype._controls.HdnExceptionDate().val(tdExceptionDate.attr('value'));


                switch (tdExceptionDate.attr('value')) {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                        target.text(tdExceptionDate.textNBR());
                        SMSSRMDownstreamStatusMethods.prototype._controls.LblStartDate().text('Start Date : ').css('display', '');
                        SMSSRMDownstreamStatusMethods.prototype._controls.LblEndDate().text('End Date : ').css('display', '');
                        SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().css('display', ''); //.attr('title', 'Enter a Start Date')
                        SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().css('display', ''); //.attr('title', 'Enter an End Date')
                        SMSSRMDownstreamStatusMethods.prototype._controls.RblstDate().find("input[type='radio']").first().prop('checked', true);
                        break;
                    case '4':
                        var rdbDateChecked = SMSSRMDownstreamStatusMethods.prototype._controls.RblstDate().find("input[type='radio']:checked");
                        switch (rdbDateChecked.val()) {
                            case '0':
                                target.text(rdbDateChecked.next().textNBR() + ' ' + SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().val() + ' to ' + SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().val());
                                break;
                            case '1':
                                target.text(rdbDateChecked.next().textNBR() + ' ' + SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().val());
                                break;
                            case '2':
                                target.text(rdbDateChecked.next().textNBR() + ' ' + SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().val());
                                break;
                        }
                        break;
                }
                SMSSRMDownstreamStatusMethods.prototype._controls.DivExceptionDate().attr('isValid', '1');
                SMSSRMDownstreamStatusMethods.prototype._controls.DivExceptionDate().css('display', 'none');
                $("#ctl00_SRMContentPlaceHolderMiddle_labeldateselectiondiv").removeClass('selectedOption');
                SMSSRMDownstreamStatusMethods.prototype._controls.DivExceptionDate().attr('state', '0');
            }
            else {
                SMSSRMDownstreamStatusMethods.prototype._controls.DivExceptionDate().attr('isValid', '0');
            }
            return flagShowValidationMsg;
        }
    },
    onClickBtnGetStatus: function onClickBtnGetStatus(e) {
        //DSStopTimer();
        if (SMSSRMDownstreamStatusMethods.prototype._controls.HdnExceptionDate().val() == '4') {
            var selectedValue = SMSSRMDownstreamStatusMethods.prototype._controls.RblstDate().find('[type="radio"]:checked').first().val();
            var errorCount = 0;
            var errormsg = '';
            var msg = '';
            var isvalid = 'true'
            switch (selectedValue) {
                case '0':
                    //  errormsg = SMSSRMDownstreamStatusMethods.prototype.DSCompareDateFromTodaysDate(SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().val(), SMSSRMDownstreamStatusMethods.prototype._controls.LblStartDate().textNBR().replace(':', ''));
                    errormsg = CompareDateFromTodaysDateUS(SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().val(), SMSSRMDownstreamStatusMethods.prototype._controls.LblStartDate().textNBR().replace(':', ''));

                    if (errormsg != '') {
                        SMSSRMDownstreamStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                        return false;
                    }
                    else {
                        errormsg = CompareDateFromTodaysDateUS(SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().val(), SMSSRMDownstreamStatusMethods.prototype._controls.LblEndDate().textNBR().replace(':', ''));
                        if (errormsg != '') {
                            SMSSRMDownstreamStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                            return false;
                        }
                        else {
                            errormsg = CompareDateUS(SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().val(), SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().val());
                            if (errormsg != '') {
                                SMSSRMDownstreamStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                                return false;
                            }
                            else {
                                SMSSRMDownstreamStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', 'none');
                                return true;
                            }
                        }
                    }
                    break;
                case '1':
                    errormsg = CompareDateFromTodaysDateUS(SMSSRMDownstreamStatusMethods.prototype._controls.TxtStartDate().val(), SMSSRMDownstreamStatusMethods.prototype._controls.LblStartDate().textNBR().replace(':', ''));
                    break;
                case '2':
                    errormsg = CompareDateFromTodaysDateUS(SMSSRMDownstreamStatusMethods.prototype._controls.TxtEndDate().val(), SMSSRMDownstreamStatusMethods.prototype._controls.LblEndDate().textNBR().replace(':', ''));
                    break;
            }
            if (errormsg != '') {
                SMSSRMDownstreamStatusMethods.prototype._controls.LblValidationSummary().text(errormsg).css('display', '');
                return false;
            }
            else {
                SMSSRMDownstreamStatusMethods.prototype._controls.LblValidationSummary().css('display', 'none');
                return true;
            }
        }
    }
}

//function CompareDateUS(startdate, enddate, setserverdate) {
//    try {
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



function SMSSRMDownstreamStatusScreen(securityInfo, controlIdInfo1) {
    SMSSRMDownstreamStatusMethods.prototype._securityInfo = eval("(" + securityInfo + ")");
    SMSSRMDownstreamStatusMethods.prototype._controlIdInfo = eval("(" + controlIdInfo1 + ")");
    SMSSRMDownstreamStatusMethods.prototype._controls = new SRMDownstreamStatusControls(SMSSRMDownstreamStatusMethods.prototype._controlIdInfo);

    //CREATE AND REMOVE HANDLERS
    function createHandlers() {
        //Added by Neetika
        if ($("#divFilter").length != 0)
            $("#divFilter").unbind('click').click(SMSSRMDownstreamStatusMethods.prototype.onClickDivFilter);

        if ($("#divExceptionDate").length != 0)
            $("#divExceptionDate").unbind('click').click(SMSSRMDownstreamStatusMethods.prototype.onClickDivExceptionDate);
        if (SMSSRMDownstreamStatusMethods.prototype._controls.RblstDate().length != 0)
            SMSSRMDownstreamStatusMethods.prototype._controls.RblstDate().unbind('click').click(SMSSRMDownstreamStatusMethods.prototype.onClickRblstDate);
    }
    //
    createHandlers();
}

//$(document).click(function (e) {

//    if ($('#smdd_0').css('display') != 'none')
//        filterApply();

//});

$(function () {
    $('.InnerFrame[name=TabsInnerIframe1]').load(function () {


        onUpdated();
    });
    $('.InnerFrame[name=TabsInnerIframe2]').load(function () {


        onUpdated();
    });
    $('.InnerFrame[name=TabsInnerIframe3]').load(function () {


        onUpdated();
    });
});

function initializeDivFilterDate() {

    var obj = {};

    obj.dateOptions = [0, 1, 2, 3, 4];
    obj.dateFormat = 'd/m/Y';
    obj.lefttimePicker = true;
    obj.righttimePicker = false;
    obj.calenderType = 0;
    obj.calenderID = 'smdd_0';
    obj.pivotElement = $('.dateFilterText');
    // obj.pivotElement = $('[id$=labeldateselectiondiv]');
    if ($('[id$=hdnStartDateCalender]').val() == '') {
        obj.StartDateCalender = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
        obj.EndDateCalender = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
    }
    else {
        obj.StartDateCalender = $('[id$=hdnStartDateCalender]').val();
        obj.EndDateCalender = $('[id$=hdnEndDateCalender]').val();
    }
    if ($('[id$=hdnIsSingleSecurity]').val() == 'true') {
        obj.selectedTopOption = 3; //Anytime
        $('.dateFilterText').text('Anytime');
    }
    else if ($('[id$=hdnTopOption]').val() == -1) {
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
        var priorEndDate = modifiedText.selectedEndDate;
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
        if (selectedEndDate != null) {
            priorEndDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(priorEndDate), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
            $('[id$=hdnEndDateCalender]').val(priorEndDate);
        }

        if (selectedText != undefined) {
            if (selectedText.toUpperCase() === "TODAY")
                htmlString = "Today";
            else if (selectedText.toUpperCase() === "SINCE YESTERDAY")
                htmlString = "Since Yesterday";
            else if (selectedText.toUpperCase() === "THIS WEEK")
                htmlString = "This Week";
            else if (selectedText.toUpperCase() === "ANYTIME")
            { prepString = ""; htmlString = "Anytime"; }
            else if (selectedText.toUpperCase() === "CUSTOM DATE") {
                if (selectedRadioOption == 0)
                    htmlString = " After " + selectedStartDate;
                else if (selectedRadioOption == 1)
                    htmlString = " Between " + selectedStartDate + " to " + selectedEndDate;
                else if (selectedRadioOption == 2)
                {
                    //if (CommonModuleTabs.currentSelected.toLowerCase() == "securities") 
                    //    htmlString = " Before " + priorEndDate;
                    //else
                        htmlString = " Before " + selectedEndDate;
                }
            }
            $('[id$=hdnTopOption]').val(selectedDateOption);
            $('[id$=hdnCustomRadioOption]').val(selectedRadioOption);
            $('[id$=hdnFirstTime]').val('0');
            //  $('[id$=labeldateselectiondiv]').text(htmlString);
            $('.dateFilterText').text(htmlString);
            //   SMSDownstreamSystemStatusMethods.prototype._controls.HdnExceptionDate().val(selectedDateOption);
            $('[id*=TextStartDate]').val(serverStartDate);
            debugger;
            $('[id*=TextEndDate]').val(serverEndDate);
            $('[id*=hiddenPriorEndDate]').val(modifiedText.serverEndDate);
        }
        var errormsg = validateDates();
        if (errormsg == '')
            filterApply();
        return errormsg
    }

    obj.ready = function (e) {
    }
    var initObj = smdatepickercontrol.init(obj);
    $('[id*=TextStartDate]').val(initObj.serverStartDateLongDate);
    $('[id*=TextEndDate]').val(initObj.serverEndDateOriginal);
}
function validateDates() {
    checked = $('[id$=hdnCustomRadioOption]').val();
    marked = $('[id$=hdnTopOption]').val();
    serverStartDate = $('[id*=TextStartDate]').val();
    serverEndDate = $('[id*=TextEndDate]').val();
    if (serverStartDate == "")
        serverStartDate = null;
    if (serverEndDate == "")
        serverEndDate = null;
    var errormsg = '';
    if (marked == '4') {
        switch (checked) {

            case '0': //From
                errormsg = CompareDateFromTodaysDateUS(serverStartDate, 'Start Date');
                break;
            case '1':
                errormsg = CompareDateFromTodaysDateUS(serverStartDate, 'Start Date');
                if (errormsg == '')
                    errormsg = CompareDateFromTodaysDateUS(serverEndDate, 'End Date');
                if (errormsg == '')
                    errormsg = errormsg = CompareDateUS(serverStartDate, serverEndDate);
                break;
            case '2':
                errormsg = CompareDateFromTodaysDateUS(serverEndDate, 'End Date');
                break;
        }
    }
    return errormsg;
}
function OnSuccess_GetDownstreamStatusInfo(result) {
}
function OnFailure(result) {

}
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
    var startDate, endDate, checked, marked;
    //  var arr1 = [];
    var obj1 = {};
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    CallCustomDatePicker($('[id*="TextStartDate"]').prop('id').trim(), SMSSRMDownstreamStatusMethods.prototype._securityInfo.CustomJQueryDateTimeFormat, null, optionDateTime.DATETIME, 15, true);
    CallCustomDatePicker($('[id*="TextEndDate"]').prop('id').trim(), SMSSRMDownstreamStatusMethods.prototype._securityInfo.CustomJQueryDateFormat, null, optionDateTime.DATE, 15, true);



    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });


    //if ($("#isPostBack").val() === "False" && $('[id$=hdnUrl]').val() == '') {
    //    onUpdating();
    //    $('.InnerFrame[name=TabsInnerIframe1]').attr('src', path + '/SMDownstreamSystemStatus.aspx?identifier=ExternalSystemStatus&statusscreen=1');
    //    $('.InnerFrame[name=TabsInnerIframe2]').attr('src', path + '/RefMasterLite.aspx?identifier=RefM_ReportSystemTaskStatus&statusscreen=1&systems=All systems&taskstatus=-1');
    //    $('.InnerFrame[name=TabsInnerIframe3]').attr('src', path + '/SMCorporateActionStatus.aspx?identifier=CorporateActionStatus&statusscreen=1');
    //}
    //else {
    //    var hdnUrl = $('[id$=hdnUrl]').val();
    //    formUrlData();
    //    $('.InnerFrame[name=TabsInnerIframe1]').attr('src', path + '/SMDownstreamSystemStatus.aspx?identifier=ExternalSystemStatus' + hdnUrl);
    //    $('.InnerFrame[name=TabsInnerIframe2]').attr('src', path + '/RefMasterLite.aspx?identifier=RefM_ReportSystemTaskStatus&systems=All systems&status=1&taskstatus=-1&startdate=null&enddate=' + endDate + '&statusscreen=1');
    //    //  $('.InnerFrame[name=TabsInnerIframe2]').attr('src', path + '/RefMasterLite.aspx?identifier=RefM_ReportSystemTaskStatus' + hdnUrl);
    //    $('.InnerFrame[name=TabsInnerIframe3]').attr('src', path + '/SMCorporateActionStatus.aspx?identifier=CorporateActionStatus' + hdnUrl);
    //}




    $.fn.textNBR = function () {
        var regEx = new RegExp(String.fromCharCode(160), 'gi');
        var normalSpace = ' ';
        var tempText = this.text().replace(regEx, normalSpace);
        //var result = new RegExp(String.fromCharCode(160), 'gi').test(tempText);
        return tempText;
    };

    //$("#tab1").click(function () {

    //    $('#container2').css('display', 'none');
    //    $('#container3').css('display', 'none');
    //    $('#container1').css('display', 'block');
    //    var grid = $('#InnerFrameId1')[0].contentWindow.document.getElementById('ctl00_cphMain_ExternalSystemStatus_gridExternalSystemStatus1');
    //    if (grid != null)
    //        $('#InnerFrameId1')[0].contentWindow.$find('ctl00_cphMain_ExternalSystemStatus_gridExternalSystemStatus1').refreshWithCache();

    //    $('#tab2').removeClass('TabscssAfter');
    //    $('#tab3').removeClass('TabscssAfter');
    //    $('#tab1').addClass('TabscssAfter');


    //});

    //$("#tab2").click(function () {

    //    $('#container1').css('display', 'none');
    //    $('#container3').css('display', 'none');
    //    $('#container2').css('display', 'block');
    //    //        var grid = $('#InnerFrameId2')[0].contentWindow.document.getElementById('ctl00_cphMain_RefM_ReportSystemTaskStatus_panelReportSystemTaskStatus');
    //    //        if (grid != null) {
    //    //            if ($('#InnerFrameId2')[0].contentWindow.$find('ctl00_cphMain_RefM_ReportSystemTaskStatus_panelReportSystemTaskStatus'))
    //    //                $('#InnerFrameId2')[0].contentWindow.$find('ctl00_cphMain_RefM_ReportSystemTaskStatus_panelReportSystemTaskStatus').refreshWithCache();
    //    //        }

    //    $('#tab1').removeClass('TabscssAfter');
    //    $('#tab3').removeClass('TabscssAfter');
    //    $('#tab2').addClass('TabscssAfter');
    //});
    //$("#tab3").click(function () {

    //    // $find('ctl00_cphMain_CorporateActionStatus_xlGridExternalSystemStatus').refreshWithCache();
    //    $('#container2').css('display', 'none');
    //    $('#container1').css('display', 'none');
    //    $('#container3').css('display', 'block');
    //    var grid = $('#InnerFrameId3')[0].contentWindow.document.getElementById('ctl00_cphMain_CorporateActionStatus_xlGridExternalSystemStatus');
    //    if (grid != null) {
    //        if ($('#InnerFrameId3')[0].contentWindow.$find('ctl00_cphMain_CorporateActionStatus_xlGridExternalSystemStatus'))
    //            $('#InnerFrameId3')[0].contentWindow.$find('ctl00_cphMain_CorporateActionStatus_xlGridExternalSystemStatus').refreshWithCache();
    //    }
    //    $('#tab2').removeClass('TabscssAfter');
    //    $('#tab1').removeClass('TabscssAfter');
    //    $('#tab3').addClass('TabscssAfter');
    //});

    ////    if ($('[id$=hdnIsSingleSecurity]').val())
    ////        $("#tab3").trigger('click');
    ////    else
    //$("#tab1").trigger('click');

    $('[id$=btnGetStatus]').click(function (e) {
        // $('#InnerFrameId1').trigger('click');

        var selectedExternalSystemParam = "";
        var filtersSelectedData = smslidemenu.getAllData("SMDashboardRightFilterDiv"); //$("#SMDashboardRightFilter").rightFilter("getFiltersJSON");
        filtersSelectedDataSelected = filtersSelectedData;
        smslidemenu.hide("SMDashboardRightFilterDiv");
        var SelectedExternalSystems = filtersSelectedData.ExternalSystems.SelectedText;
        var SelectedStatus = filtersSelectedData.Status.SelectedValue;
        var SelectedTaskStatus = filtersSelectedData.TaskStatus.SelectedText;
        var details = formUrlData(SelectedExternalSystems, SelectedStatus, SelectedTaskStatus);
        var errormsg = validateDates();
        if (errormsg == '') {
            onUpdating();
            $('#smdd_0').find('.validationSummary').text('');
            var str = $('[id$=hdnUrl]').val();
            var pos1 = str.indexOf("&");
            var pos2 = str.indexOf("&", pos1 + 1);
            str = str.substr(pos1, pos2);

            var urlString;

            if (CommonModuleTabs.currentSelected.toLowerCase() == "securities") {

                if ($('[id$=hdnIsSingleSecurity]').val() == "true") {
                    urlString = path + '/../SMDownstreamSystemStatus.aspx?identifier=ExternalSystemStatus&systems=' + details.system + '&status=' + details.statusfilter + '&taskstatus=' + details.taskstatus + '&startdate=' + details.startDate + '&enddate=' + details.endDate + '&marked=' + details.marked + '&checked=' + details.checked + '&statusscreen=1&IsSingleSecurity=' + $('[id$=hdnIsSingleSecurity]').val() + str + '&priorEndDate=' + details.priorEndDate;
                }
                else {
                    urlString = path + '/../SMDownstreamSystemStatus.aspx?identifier=ExternalSystemStatus&systems=' + details.system + '&status=' + details.statusfilter + '&taskstatus=' + details.taskstatus + '&startdate=' + details.startDate + '&enddate=' + details.endDate + '&marked=' + details.marked + '&checked=' + details.checked + '&statusscreen=1&priorEndDate=' + details.priorEndDate;

                }
            }

            else if (CommonModuleTabs.currentSelected.toLowerCase() == "refdata") {
                urlString = path + '/../RMHomeInternal.aspx?identifier=RefM_ReportSystemTaskStatus&systems=' + details.system + '&status=' + details.statusfilter + '&taskstatus=' + details.taskstatus + '&startdate=' + details.startDate + '&enddate=' + details.endDate + '&statusscreen=1&ModuleID=' + _moduleID_moduleName_map["refdata"];

            }

            else if (CommonModuleTabs.currentSelected.toLowerCase() == "corpaction") {
                urlString = path + '/../SMCorporateActionStatus.aspx?identifier=CorporateActionStatus&systems=' + details.system + '&status=' + details.statusfilter + '&taskstatus=' + details.taskstatus + '&startdate=' + details.startDate + '&enddate=' + details.endDate + '&statusscreen=1';

            }

            else if (CommonModuleTabs.currentSelected.toLowerCase() == "funds") {
                urlString = path + '/../RMHomeInternal.aspx?identifier=RefM_ReportSystemTaskStatus&systems=' + details.system + '&status=' + details.statusfilter + '&taskstatus=' + details.taskstatus + '&startdate=' + details.startDate + '&enddate=' + details.endDate + '&statusscreen=1&ModuleID=' + _moduleID_moduleName_map["funds"];

            }

            else if (CommonModuleTabs.currentSelected.toLowerCase() == "parties") {
                urlString = path + '/../SMCorporateActionStatus.aspx?identifier=CorporateActionStatus&systems=' + details.system + '&status=' + details.statusfilter + '&taskstatus=' + details.taskstatus + '&startdate=' + details.startDate + '&enddate=' + details.endDate + '&statusscreen=1&ModuleID=' + _moduleID_moduleName_map["parties"];

            }

            CommonModuleTabs.callbackObj[CommonModuleTabs.currentSelected](urlString);

        }
        else { $('#smdd_0').find('.validationSummary').text(errormsg); }
        e.stopPropagation();
    });


    $('#SMDownstreamRefreshButton').unbind('click').click(function (e) {
        filterApply();
        e.stopPropagation();
    });
    $('[id$=ArrowDownApplyToAll]').click(function (e) {
        $('.dateFilterText').trigger('click');
        e.stopPropagation();
    });

    initialiseTabs();

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


    changeDropDown($("#ctl00_SRMContentPlaceHolderMiddle_ddlStatusFilter"));
    changeDropDown($("#ctl00_SRMContentPlaceHolderMiddle_ddlSystems"));
    changeDropDown($("#ctl00_SRMContentPlaceHolderMiddle_ddlTaskStatus"));

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


    getSelectedValue($('#smselect_ctl00_SRMContentPlaceHolderMiddle_ddlSystems'), $('#ctl00_SRMContentPlaceHolderMiddle_ddlSystems'), 'sytem');
    getSelectedValue($('#smselect_ctl00_SRMContentPlaceHolderMiddle_ddlStatusFilter'), $('#ctl00_SRMContentPlaceHolderMiddle_ddlStatusFilter'), 'statusfilter');
    getSelectedValue($('#smselect_ctl00_SRMContentPlaceHolderMiddle_ddlTaskStatus'), $('#ctl00_SRMContentPlaceHolderMiddle_ddlTaskStatus'), 'taskstatus');

    $("#smselect_ctl00_SRMContentPlaceHolderMiddle_ddlTaskStatus").attr('style', 'width:100%')
    $("#smselect_ctl00_SRMContentPlaceHolderMiddle_ddlStatusFilter").attr('style', 'width:100%')
    $("#smselect_ctl00_SRMContentPlaceHolderMiddle_ddlSystems").attr('style', 'width:100%')
    $('#smselect_ctl00_SRMContentPlaceHolderMiddle_ddlStatusFilter').find('a').css({ 'text-decoration': 'none', 'color': '#48a3dd' });
    $('#smselect_ctl00_SRMContentPlaceHolderMiddle_ddlTaskStatus').find('a').css({ 'text-decoration': 'none', 'color': '#48a3dd' });
    $('#smselect_ctl00_SRMContentPlaceHolderMiddle_ddlSystems').find('a').css({ 'text-decoration': 'none', 'color': '#48a3dd' });
    $('#ctl00_SRMContentPlaceHolderMiddle_labeldateselectiondiv').css({ 'text-decoration': 'none' });
    $("#smselect_ctl00_SRMContentPlaceHolderMiddle_ddlTaskStatus").find(".smselectanchorrun").addClass("LinkExceptionFilterItems").css('margin-right', '0px');
    $("#smselect_ctl00_SRMContentPlaceHolderMiddle_ddlStatusFilter").find(".smselectanchorrun").addClass("LinkExceptionFilterItems").css('margin-right', '0px');
    $("#smselect_ctl00_SRMContentPlaceHolderMiddle_ddlSystems").find(".smselectanchorrun").addClass("LinkExceptionFilterItems").css('margin-right', '0px');

});

function setStatusScreenHeight() {
    var height = $(window).height() - $('#container1').offset().top - 30;
    $('.InnerFrame').height(height);
}

function initSlideMenu(moduleID) {
    var parms = {};
    parms.userName = SMSSRMDownstreamStatusMethods.prototype._controls._controlIdInfo.UserName;
    parms.isTempFilterApplied = false;
    parms.productName = 'sec';
    parms.moduleId = moduleID;


    var isExternalSysConf = CallCommonServiceMethod('GetDownstreamStatusInfo', parms, OnSuccess_GetDownstreamStatusInfo, OnFailure, null, false);
    if (isExternalSysConf == true) {

        setStatusScreenHeight();
        $('#SMDownstreamRefreshButton').css('display', 'inline-block');
        $('#DateFilterApplyToAll').css('display', 'inline-block');

    }
    else {
        $('#SMDownstreamRefreshButton').css('display', 'none');
        $('#DateFilterApplyToAll').css('display', 'none');
        window.parent.leftMenu.showNoRecordsMsg("No downstream system is present.", $("#downstreamsystemErrorDiv"));
    }

}

var commonServiceLocation = '/BaseUserControls/Service/CommonService.svc';
var isDownstreamStatusViewAllowedInRefM = false;
var isDownstreamStatusViewAllowedInSecM = false;
var isDownstreamStatusViewAllowedInCorp = false;

function formUrlData(SelectedExternalSystems, SelectedStatus, SelectedTaskStatus) {
    var longDateFormat = $('[id$=hdnLongDateFormat]').val();
    var details = {};
    details.system = SelectedExternalSystems.replace('_', ' ');
    details.statusfilter = SelectedStatus;
    if (SelectedTaskStatus.toLowerCase() == 'any')
        details.taskstatus = -1;
    else
        details.taskstatus = SelectedTaskStatus.replace('_', ' ');
    details.checked = $('[id$=hdnCustomRadioOption]').val();
    details.marked = $('[id$=hdnTopOption]').val();
    details.priorEndDate = $('[id*=hiddenPriorEndDate]').val();
    if ($('[id$=hdnIsSingleSecurity]').val() == 'true' && $('.dateFilterText').text().toLowerCase() == 'anytime') { //Anytime 
        details.startDate = '';
        details.endDate = '';
    }
    else {
        details.startDate = $('[id*=TextStartDate]').val();
        details.endDate = $('[id*=TextEndDate]').val();      
    }
    return details;
}
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
            case "corpaction":
                SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initiateCorpAction });
        }
    }

}

function ExecuteSynchronously(url, method, args) {
    var executor = new Sys.Net.XMLHttpSyncExecutor();
    var request = new Sys.Net.WebRequest();
    request.set_url(url + '/' + method);
    request.set_httpVerb('POST');
    request.get_headers()['Content-Type'] = 'application/json; charset=utf-8';
    request.set_executor(executor);
    request.set_body(Sys.Serialization.JavaScriptSerializer.serialize(args));
    request.invoke();
    if (executor.get_responseAvailable()) {
        return (executor.get_object());
    }
    return (false);
}
function processFilters(filterGroupItems, filterGroupHeaderID, filterGroupHeaderTitle, checkedValuesList, txtId, txtName, filterType, checkerItem, isSearchable, isDateTypeControl, selectAllText, state) {
    var filterItems = [];
    var checkedValuesArray = checkedValuesList.split(',');
    $.each(filterGroupItems, function (index, item) {
        var objFilterItem = {};
        objFilterItem.text = item[txtName];
        objFilterItem.value = item[txtId];
        if (checkedValuesArray.indexOf(item[checkerItem].toString()) > -1)
            objFilterItem.isSelected = "true";
        else
            objFilterItem.isSelected = "false";
        if (isDateTypeControl) {
            objFilterItem.dateFilterType = item.dateFilterType;
            objFilterItem.rangeStartDate = item.rangeStartDate;
            objFilterItem.rangeEndDate = item.rangeEndDate;
            objFilterItem.singleDate = item.singleDate;
            objFilterItem.dateType = item.dateType;
        }
        filterItems.push(objFilterItem);
    });
    var filterDataItem = {};
    if (filterGroupHeaderID != null)
        filterDataItem.identity = filterGroupHeaderID;
    filterDataItem.sectionHeader = filterGroupHeaderTitle;
    if (selectAllText != null)
        filterDataItem.selectAllText = selectAllText;
    filterDataItem.state = state;
    filterDataItem.sectionType = filterType;
    filterDataItem.listItems = filterItems;
    filterDataItem.isSearchable = isSearchable;
    filterDataItem.maxHeight = $(window).height() - 425;
    return filterDataItem;
}
function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
    //  callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);


    var downstreamStatusBasicInfo = ExecuteSynchronously(path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters);
    var allStatus = {};
    allStatus.d = downstreamStatusBasicInfo.d.Status;
    var allTaskStatus = {};
    allTaskStatus.d = downstreamStatusBasicInfo.d.TaskStatus;
    var allExternalSystems = {};
    allExternalSystems.d = downstreamStatusBasicInfo.d.ExternalSystems;
    if (allExternalSystems.d.length == 0) {
        return false;
    }
    else {
        var filterData = [];
        //filterData.push(processFilters(allStatus.d, 'Status', 'State', 'Most Recent', 'StatusId', 'StatusName', 'radio', 'StatusName', false, true, null, "up"));
        //filterData.push(processFilters(allExternalSystems.d, 'ExternalSystems', 'Downstream System', 'All systems', 'ExternalSystemId', 'ExternalSystemName', 'radio', 'ExternalSystemName', false, true, null, "up"));
        //filterData.push(processFilters(allTaskStatus.d, 'TaskStatus', 'Status', 'Any', 'TaskStatusId', 'TaskStatusName', 'radio', 'TaskStatusName', false, true, null, "up"));
        var filtersSelectedDataSelected = smslidemenu.getAllData("SMDashboardRightFilterDiv");
        if (Object.entries(filtersSelectedDataSelected).length != 0) {
            var selectedStatus = filtersSelectedDataSelected.Status.SelectedText;
            var selectedExternalSystem = '';

            // if downstream system selected in filter before module change is undefined
            if (typeof filtersSelectedDataSelected.ExternalSystems.SelectedText == typeof undefined) {
                filterData.push(processFilters(allExternalSystems.d, 'ExternalSystems', 'Downstream System', 'All systems', 'ExternalSystemId', 'ExternalSystemName', 'radio', 'ExternalSystemName', false, true, null, "up"));
            }
            else {
                // check if downstream system selected in filter before module change exists in current module
                try {
                    var existingSystemOnModuleChange = allExternalSystems.d.filter(sys => sys.ExternalSystemName === filtersSelectedDataSelected.ExternalSystems.SelectedText);
                }
                catch (ee) { }
                if (typeof existingSystemOnModuleChange == typeof undefined || existingSystemOnModuleChange.length == 0) {
                    filterData.push(processFilters(allExternalSystems.d, 'ExternalSystems', 'Downstream System', 'All systems', 'ExternalSystemId', 'ExternalSystemName', 'radio', 'ExternalSystemName', false, true, null, "up"));
                }
                else {
                    selectedExternalSystem = filtersSelectedDataSelected.ExternalSystems.SelectedText;
                    filterData.push(processFilters(allExternalSystems.d, 'ExternalSystems', 'Downstream System', selectedExternalSystem, 'ExternalSystemId', 'ExternalSystemName', 'radio', 'ExternalSystemName', false, true, null, "up"));
                }
            }
            var selectedTaskStatus = filtersSelectedDataSelected.TaskStatus.SelectedText;
            filterData.push(processFilters(allStatus.d, 'Status', 'State', selectedStatus, 'StatusId', 'StatusName', 'radio', 'StatusName', false, true, null, "up"));
           
            filterData.push(processFilters(allTaskStatus.d, 'TaskStatus', 'Status', selectedTaskStatus, 'TaskStatusId', 'TaskStatusName', 'radio', 'TaskStatusName', false, true, null, "up"));
        }
        else {
            var urlParams = new URLSearchParams(window.location.search);
            var systemFromCreateUpdate = '';
            if (urlParams.has('systems'))
                systemFromCreateUpdate = urlParams.get('systems');

            filterData.push(processFilters(allStatus.d, 'Status', 'State', 'Most Recent', 'StatusId', 'StatusName', 'radio', 'StatusName', false, true, null, "up"));

            /// fix for 583841 - from create update screen
            if (systemFromCreateUpdate != '')
            {
                filterData.push(processFilters(allExternalSystems.d, 'ExternalSystems', 'Downstream System', systemFromCreateUpdate, 'ExternalSystemId', 'ExternalSystemName', 'radio', 'ExternalSystemName', false, true, null, "up"));
            }
            else
                filterData.push(processFilters(allExternalSystems.d, 'ExternalSystems', 'Downstream System', 'All systems', 'ExternalSystemId', 'ExternalSystemName', 'radio', 'ExternalSystemName', false, true, null, "up"));

            filterData.push(processFilters(allTaskStatus.d, 'TaskStatus', 'Status', 'Any', 'TaskStatusId', 'TaskStatusName', 'radio', 'TaskStatusName', false, true, null, "up"));

        }
        bindSlideMenu(filterData);
        return true;
    }
}

function filterApply() {
    $('[id$=btnGetStatus]').trigger('click');

}

function bindSlideMenu(filterData) {
    var rightFilterContainer = $("#SMDashboardRightFilterContainer");
    rightFilterContainer.empty();

    var objFilterData = {};
    objFilterData.pivotElementId = "SMDashboardRightFilter";
    objFilterData.id = "SMDashboardRightFilterDiv";
    objFilterData.container = rightFilterContainer;
    objFilterData.data = filterData;
    objFilterData.format = SMSSRMDownstreamStatusMethods.prototype._securityInfo.CustomJQueryDateFormat;
    smslidemenu.init(objFilterData, function () { filterApply(); });

    var txtInProgressLimit = $('#txtInProgressLimit');
    var spanInProgressLimit = $('#spanInProgressLimit');
    filtersSelectedDataSelected = smslidemenu.getAllData("SMDashboardRightFilterDiv");
}
function initiateSec(urlString) {
    initSlideMenu(3);
    var filtersSelectedData = smslidemenu.getAllData("SMDashboardRightFilterDiv"); //$("#SMDashboardRightFilter").rightFilter("getFiltersJSON");
    filtersSelectedDataSelected = filtersSelectedData;

    debugger;
    if (Object.entries(filtersSelectedDataSelected).length != 0) {
        smslidemenu.hide("SMDashboardRightFilterDiv");
        var SelectedExternalSystems = '';
        if (typeof filtersSelectedData.ExternalSystems.SelectedText == typeof undefined)
            SelectedExternalSystems = 'All systems';
        else
            SelectedExternalSystems = filtersSelectedData.ExternalSystems.SelectedText;

        var SelectedStatus = filtersSelectedData.Status.SelectedValue;
        var SelectedTaskStatus = filtersSelectedData.TaskStatus.SelectedText;
        var details = formUrlData(SelectedExternalSystems, SelectedStatus, SelectedTaskStatus);
    }
    else
        var details = formUrlData('', '1', '');

    var sec_ids = $('[id$=hdnSingleSecurityValue]').val();
    ////////// get date filters and set /////////////
    var modifiedText = smdatepickercontrol.getResponse($("#smdd_0"));
    details.marked = modifiedText.selDateOption;
    $('[id$=hdnTopOption]').val(modifiedText.selDateOption);
    ///////////////////////////////////////////////////////////////////////

    if (sec_ids != '')
        var pathSecmaster = path + '/../SMDownstreamSystemStatus.aspx?identifier=ExternalSystemStatus&sec_ids=' + sec_ids + '&systems=' + details.system + '&status=' + details.statusfilter + '&taskstatus=' + details.taskstatus + '&startdate=' + details.startDate + '&enddate=' + details.endDate + '&marked=' + details.marked + '&checked=' + details.checked + '&statusscreen=1&issinglesecurity=True&priorEndDate=' + details.priorEndDate;
    else
        var pathSecmaster = path + '/../SMDownstreamSystemStatus.aspx?identifier=ExternalSystemStatus&systems=' + details.system + '&status=' + details.statusfilter + '&taskstatus=' + details.taskstatus + '&startdate=' + details.startDate + '&enddate=' + details.endDate + '&marked=' + details.marked + '&checked=' + details.checked + '&statusscreen=1&priorEndDate=' + details.priorEndDate;
    // var pathSecmaster = path + '/../SMDownstreamSystemStatus.aspx?identifier=ExternalSystemStatus&statusscreen=1';


    //if ((urlString != null || urlString != undefined) && typeof (urlString) == "string") {
    //    pathSecmaster = urlString;
    //}


    $('[id$="lblRunningText"]').text("Status for securities requested");

    var grid = $('#InnerFrameId1')[0].contentWindow.document.getElementById('ctl00_cphMain_ExternalSystemStatus_gridExternalSystemStatus1');
    if (grid != null)
        $('#InnerFrameId1')[0].contentWindow.$find('ctl00_cphMain_ExternalSystemStatus_gridExternalSystemStatus1').refreshWithCache();


    //$('body').find('#ctl00_SRMContentPlaceHolderMiddle_TextStartDate_ContainerDivDateTimePicker').hide();
    //$('body').find('#ctl00_SRMContentPlaceHolderMiddle_TextEndDate_ContainerDivDateTimePicker').hide();
    //$('body').find('.smselectcon').css('display', 'none')
    //SMSSRMRealTimeStatusMethods.prototype.toggleFilters(e);

    if ($("#isPostBack").val() === "False" && $('[id$=hdnUrl]').val() == '') {// for monitor => downstream post status 
        onUpdating();
        $('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathSecmaster);


    }
    else { // for security specific status from create/update
        var hdnUrl = $('[id$=hdnUrl]').val();

        pathSecmaster = path + '/../SMDownstreamSystemStatus.aspx?identifier=ExternalSystemStatus&statusscreen=1' + hdnUrl;
        $('[id$=hdnUrl]').val('');
        $('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathSecmaster);

    }
}

function initiateRef(urlString) {
    initSlideMenu(6);
    var filtersSelectedData = smslidemenu.getAllData("SMDashboardRightFilterDiv"); //$("#SMDashboardRightFilter").rightFilter("getFiltersJSON");
    filtersSelectedDataSelected = filtersSelectedData;
    smslidemenu.hide("SMDashboardRightFilterDiv");
    var SelectedExternalSystems = filtersSelectedData.ExternalSystems.SelectedText;
    var SelectedStatus = filtersSelectedData.Status.SelectedValue;
    var SelectedTaskStatus = filtersSelectedData.TaskStatus.SelectedText;
    var details = formUrlData(SelectedExternalSystems, SelectedStatus, SelectedTaskStatus);
    if (details.checked == '0') {
        details.endDate = '';
    }
    else if (details.checked == '2') {
        details.startDate = '';
    }
    var pathRefMaster = path + '/../RMHomeInternal.aspx?identifier=RefM_ReportSystemTaskStatus&systems=' + details.system + '&status=' + details.statusfilter + '&taskstatus=' + details.taskstatus + '&startdate=' + details.startDate + '&enddate=' + details.endDate + '&marked=' + details.marked + '&checked=' + details.checked + '&statusscreen=1';
    //var pathRefMaster = path + '/../RMHomeInternal.aspx?identifier=RefM_ReportSystemTaskStatus&statusscreen=1&systems=All systems&taskstatus=-1';
    //if ((urlString != null || urlString != undefined) && typeof (urlString) == "string") {
    //    pathRefMaster = urlString;
    //}

    if ($("#isPostBack").val() === "False" && $('[id$=hdnUrl]').val() == '') {
        onUpdating();
        pathRefMaster += "&ModuleID=" + _moduleID_moduleName_map["refdata"];

        //$('.InnerFrame[name=TabsInnerIframe1]').attr('src', path + '/../RefMasterLite.aspx?identifier=RefM_ReportSystemTaskStatus&statusscreen=1&systems=All systems&taskstatus=-1');
        $('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathRefMaster);

    }
    else {
        var hdnUrl = $('[id$=hdnUrl]').val();


        pathRefMaster = path + '/../RMHomeInternal.aspx?identifier=RefM_RealTimebackGroundStatus&RealTimestatusscreen=1&systems=All systems&status=1&taskstatus=-1&startdate=null&enddate=' + endDate + '&statusscreen=1';
        pathRefMaster += "&ModuleID=" + _moduleID_moduleName_map["refdata"];
        $('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathRefMaster);
    }

}

function initiateParty(urlString) {
    initSlideMenu(20);
    var filtersSelectedData = smslidemenu.getAllData("SMDashboardRightFilterDiv"); //$("#SMDashboardRightFilter").rightFilter("getFiltersJSON");
    filtersSelectedDataSelected = filtersSelectedData;
    smslidemenu.hide("SMDashboardRightFilterDiv");
    var SelectedExternalSystems = filtersSelectedData.ExternalSystems.SelectedText;
    var SelectedStatus = filtersSelectedData.Status.SelectedValue;
    var SelectedTaskStatus = filtersSelectedData.TaskStatus.SelectedText;
    var details = formUrlData(SelectedExternalSystems, SelectedStatus, SelectedTaskStatus);
    if (details.checked == '0') {
        details.endDate = '';
    }
    else if (details.checked == '2') {
        details.startDate = '';
    }
    var pathRefMaster = path + '/../RMHomeInternal.aspx?identifier=RefM_ReportSystemTaskStatus&systems=' + details.system + '&status=' + details.statusfilter + '&taskstatus=' + details.taskstatus + '&startdate=' + details.startDate + '&enddate=' + details.endDate + '&marked=' + details.marked + '&checked=' + details.checked + '&statusscreen=1';
    //var pathRefMaster = path + '/../RMHomeInternal.aspx?identifier=RefM_ReportSystemTaskStatus&statusscreen=1&systems=All systems&taskstatus=-1';

    //if ((urlString != null || urlString != undefined) && typeof (urlString) == "string") {
    //    pathRefMaster = urlString;
    //}


    if ($("#isPostBack").val() === "False" && $('[id$=hdnUrl]').val() == '') {
        onUpdating();
        pathRefMaster += "&ModuleID=" + _moduleID_moduleName_map["parties"];

        //$('.InnerFrame[name=TabsInnerIframe1]').attr('src', path + '/../RefMasterLite.aspx?identifier=RefM_ReportSystemTaskStatus&statusscreen=1&systems=All systems&taskstatus=-1');
        $('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathRefMaster);

    }
    else {
        var hdnUrl = $('[id$=hdnUrl]').val();

        pathRefMaster = path + '/../RMHomeInternal.aspx?identifier=RefM_RealTimebackGroundStatus&RealTimestatusscreen=1&systems=All systems&status=1&taskstatus=-1&startdate=null&enddate=' + endDate + '&statusscreen=1';
        pathRefMaster += "&ModuleID=" + _moduleID_moduleName_map["parties"];
        $('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathRefMaster);
    }

}

function initiateFund(urlString) {
    initSlideMenu(18);
    var filtersSelectedData = smslidemenu.getAllData("SMDashboardRightFilterDiv"); //$("#SMDashboardRightFilter").rightFilter("getFiltersJSON");
    filtersSelectedDataSelected = filtersSelectedData;
    smslidemenu.hide("SMDashboardRightFilterDiv");
    var SelectedExternalSystems = filtersSelectedData.ExternalSystems.SelectedText;
    var SelectedStatus = filtersSelectedData.Status.SelectedValue;
    var SelectedTaskStatus = filtersSelectedData.TaskStatus.SelectedText;
    var details = formUrlData(SelectedExternalSystems, SelectedStatus, SelectedTaskStatus);
    if (details.checked == '0') {
        details.endDate = '';
    }
    else if (details.checked == '2') {
        details.startDate = '';
    }
    var pathRefMaster = path + '/../RMHomeInternal.aspx?identifier=RefM_ReportSystemTaskStatus&systems=' + details.system + '&status=' + details.statusfilter + '&taskstatus=' + details.taskstatus + '&startdate=' + details.startDate + '&enddate=' + details.endDate + '&marked=' + details.marked + '&checked=' + details.checked + '&statusscreen=1';
    //var pathRefMaster = path + '/../RMHomeInternal.aspx?identifier=RefM_ReportSystemTaskStatus&statusscreen=1&systems=All systems&taskstatus=-1';

    //if ((urlString != null || urlString != undefined) && typeof (urlString) == "string") {
    //    pathRefMaster = urlString;
    //}


    if ($("#isPostBack").val() === "False" && $('[id$=hdnUrl]').val() == '') {
        onUpdating();
        pathRefMaster += "&ModuleID=" + _moduleID_moduleName_map["funds"];

        //$('.InnerFrame[name=TabsInnerIframe1]').attr('src', path + '/../RefMasterLite.aspx?identifier=RefM_ReportSystemTaskStatus&statusscreen=1&systems=All systems&taskstatus=-1');
        $('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathRefMaster);

    }
    else {
        var hdnUrl = $('[id$=hdnUrl]').val();

        pathRefMaster = path + '/../RMHomeInternal.aspx?identifier=RefM_RealTimebackGroundStatus&RealTimestatusscreen=1&systems=All systems&status=1&taskstatus=-1&startdate=null&enddate=' + endDate + '&statusscreen=1';
        pathRefMaster += "&ModuleID=" + _moduleID_moduleName_map["funds"];
        $('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathRefMaster);
    }

}


function initiateCorpAction(urlString) {
    initSlideMenu(9);
    var filtersSelectedData = smslidemenu.getAllData("SMDashboardRightFilterDiv"); //$("#SMDashboardRightFilter").rightFilter("getFiltersJSON");
    filtersSelectedDataSelected = filtersSelectedData;
    smslidemenu.hide("SMDashboardRightFilterDiv");
    var SelectedExternalSystems = filtersSelectedData.ExternalSystems.SelectedText;
    var SelectedStatus = filtersSelectedData.Status.SelectedValue;
    var SelectedTaskStatus = filtersSelectedData.TaskStatus.SelectedText;
    var details = formUrlData(SelectedExternalSystems, SelectedStatus, SelectedTaskStatus);


    var pathCorpAction = path + '/../SMCorporateActionStatus.aspx?identifier=CorporateActionStatus&systems=' + details.system + '&status=' + details.statusfilter + '&taskstatus=' + details.taskstatus + '&startdate=' + details.startDate + '&enddate=' + details.endDate + '&marked=' + details.marked + '&checked=' + details.checked + '&statusscreen=1';
    //if ((urlString != null || urlString != undefined) && typeof (urlString) == "string") {
    //    pathCorpAction = urlString;
    //}


    var grid = $('#InnerFrameId1')[0].contentWindow.document.getElementById('ctl00_cphMain_CorporateActionStatus_xlGridExternalSystemStatus');
    if (grid != null) {
        if ($('#InnerFrameId1')[0].contentWindow.$find('ctl00_cphMain_CorporateActionStatus_xlGridExternalSystemStatus'))
            $('#InnerFrameId1')[0].contentWindow.$find('ctl00_cphMain_CorporateActionStatus_xlGridExternalSystemStatus').refreshWithCache();
    }
    if ($("#isPostBack").val() === "False" && $('[id$=hdnUrl]').val() == '') {
        onUpdating();

        $('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathCorpAction);
    }
    else {
        var hdnUrl = $('[id$=hdnUrl]').val();


        pathCorpAction = path + '/../SMCorporateActionStatus.aspx?identifier=CorporateActionStatus' + hdnUrl;
        $('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathCorpAction);
    }

}


var dateValue = {
    0: "Today",
    1: "Yesterday",
    2: "This Week",
    3: "Any time"
}
function setDateControl(marked, targetId, hdnExceptionDateID) {
    setTimeout(function () {
        $('#ctl00_SRMContentPlaceHolderMiddle_hdnExceptionDate').val(marked);
        if (marked.toString() !== "4") {
            $("#" + targetId).text(dateValue[marked]);
            $('#divExceptionDate tr').css('color', 'rgb(0,0,0)');
            //   $('#divExceptionDate tr:eq(3)').css('color', '#48a3dd');
            $('#divExceptionDate tr:eq(3)')[0].style.color = "#48a3dd";
        }
    }, 500);
}

