var smdatepickercontrol = (function () {

    var SMDatePickerControl = function () {
        this.counter = 0;
        this.pivotVsDivid = [];
        this.obj = [];
    }

    var smdatepickercontrol = new SMDatePickerControl();


    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });
    if (path.endsWith('/App_Dynamic_Resource'))
        path = path.replace('/App_Dynamic_Resource', '')
    smdatepickercontrol.path = path;

    var clientformat;
    var clientShortFormat;
    var serverformat;
    var serverShortformat;

    var onSuccessGetServerFormat = function (e) {
        serverformat = e.d.longFormat;
        serverShortformat = e.d.shortFormat;
        clientformat = e.d.clientLongFormat;
        clientShortFormat = e.d.clientShortFormat;
    }

    $.ajax({
        async: false,
        data: JSON.stringify({}),
        type: "POST",
        url: smdatepickercontrol.path + "/BaseUserControls/Service/CommonService.svc/GetServerFormat",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: onSuccessGetServerFormat
    });

    //2 calenders
    var _dateOptions = {
        0: "Today",
        1: "Since Yesterday",
        3: "Anytime",
        2: "This Week",
        4: "Custom Date",
        5: "Last Days",
        6: "Next Days"


    }

    //extraRadioOptions
    var _extraRadioOptions = {
        0: "Outside",
        1: "On"
    }
    //1 calender
    //    var _dateOptionsSingleCalender = {
    //        0: "Today",
    //        1: "Since Yesterday",
    //        2: "Custom Date"

    //    }

    var _lstNxtFreqOptions = {
        0: "Hours",
        1: "Days",
        2: "Weeks",
        3: "Months",
        4: "Quaters",
        5: "Years"
    }

    var years = {};
    for (var i = 1990; i < 2025; i++)
        years[i] = i;
    var months = {
        0: "Jan",
        1: "Feb",
        2: "Mar",
        3: "Apr",
        4: "May",
        5: "Jun",
        6: "Jul",
        7: "Aug",
        8: "Sep",
        9: "Oct",
        10: "Nov",
        11: "Dec"
    }
    var quaters = {
        1: "Quater1",
        2: "Quater2",
        3: "Quater3",
        4: "Quater4"
    }

    var isChosenStartDate = false;
    var isChosenEndDate = false;


    var docHandler = function (e) {
        e.stopPropagation();
        if (!$(e.target).hasClass('smdd_container')) {
            if (($(e.target).children('.smdd_container').length != 0) || ($('body').find('.smdd_container').length != 0)) {
                var selectsmdd = $('.smdd_container');
                $.each(selectsmdd, function (ii, ee) {
                    var errormsg = '';
                    if ($(ee).css('display') == 'block') {

                        errormsg = smdatepickercontrol.obj[$(ee)[0].id.split('_')[1]].applyCallback();
                        if (errormsg == '' || errormsg == false || errormsg == undefined) {
                            $('#' + $(ee)[0].id + ' .validationSummary').text('');
                            $('#' + $(ee)[0].id + ' .smselectcon').hide();
                            $(ee).hide();
                        }
                        else { $('#' + $(ee)[0].id + ' .validationSummary').text(errormsg); }
                    }
                });
            }
        }
        else if ($(e.target).hasClass('smdd_container')) {
            $('#' + $(e.target)[0].id + ' .smselectcon').hide();
        }
    }
    //calenderType -> 0 (2 calenders)  1-> (1 calender)
    SMDatePickerControl.prototype.init = function ($object) {
        if (typeof $object.StartDateCalender == "undefined" || $object.StartDateCalender == null || $object.StartDateCalender == "-1" || $object.StartDateCalender == "") {
            $object.StartDateCalender = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(new Date().setHours(0, 0, 0, 0)), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
            isChosenStartDate = false;
        }
        else
            isChosenStartDate = true;

        if (typeof $object.EndDateCalender == "undefined" || $object.EndDateCalender == null || $object.EndDateCalender == "-1" || $object.EndDateCalender == "") {
            $object.EndDateCalender = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
            isChosenStartDate = false;
        }
        else
            isChosenEndDate = true;

        if ($object.calenderType == 0) {
            createHTML($object);

        }
        else if ($object.calenderType == 1) {
            createHTMLforSingleCalender($object);

        }
        else if ($object.calenderType == 2)
            createHTMLforNoCalender($object);

        var startDateObject = Date.parseInvariant($object.StartDateCalender, clientShortFormat);
        var endDateObject = Date.parseInvariant($object.EndDateCalender, clientShortFormat);

        if (startDateObject == null)
            startDateObject = Date.parseInvariant($object.StartDateCalender, clientformat);
        if (endDateObject == null)
            endDateObject = Date.parseInvariant($object.EndDateCalender, clientformat);

        var serverStartDate;
        var serverEndDate;
        var serverStartDateOriginal;
        var serverEndDateOriginal;
        var serverStartDateLongDate;

        if (startDateObject != null) {
            serverStartDate = new Date(new Date(startDateObject).setHours(23, 59, 59, 999)).format(serverformat);
            serverStartDateOriginal = startDateObject.format(serverShortformat);
            serverStartDateLongDate = startDateObject.format(serverformat);
        }
        if (endDateObject != null) {
            serverEndDate = endDateObject.format(serverformat);
            serverEndDateOriginal = endDateObject.format(serverShortformat);
        }

        var objInfo = {
            serverStartDate: serverStartDate,
            serverEndDate: serverEndDate,
            serverStartDateOriginal: serverStartDateOriginal,
            serverEndDateOriginal: serverEndDateOriginal,
            serverStartDateLongDate: serverStartDateLongDate
        };

        return objInfo;


    }
    SMDatePickerControl.prototype.SetValuesInCalender = function ($object) {
        // $($object.calenderID).show();
        attachEventHandlers($object, $object.calenderID);
        $("#" + $object.calenderID).find(".smdd_custom_section").attr('selectedRadioOption', $object.selectedCustomRadioOption);
        $($('.smdd_date_option')[$object.selectedTopOption]).trigger('click');
        if ($object.selectedTopOption == 4)
            $($('[name=custom_' + $object.calenderID + ']')[$object.selectedCustomRadioOption]).trigger('click');
        return false;

    }
    SMDatePickerControl.prototype.DisposeCalender = function () {
        smdatepickercontrol.counter = 0;
        smdatepickercontrol.pivotVsDivid = [];
        smdatepickercontrol.obj = [];
        return false;

    }
    SMDatePickerControl.prototype.Show = function ($object) {
        $('#' + $object.calenderID).css('display', '');
        return false;

    }
    SMDatePickerControl.prototype.Hide = function ($object) {
        $('#' + $object.calenderID).css('display', 'none');
        return false;

    }
    SMDatePickerControl.prototype.getResponse = function ($id) {
        var selDateOption = parseInt($id.attr("selectedDate"));
        var selRadioOption = parseInt($id.children(".smdd_custom_section").attr('selectedRadioOption'));
        var startDate = $id.attr("selectedStartDate");
        var endDate = $id.attr("selectedEndDate");
        var displayText = $id.attr("DisplayText");
        var counter = $id[0].id.split('_')[1];
        var calenderType = smdatepickercontrol.obj[counter].calenderType;
        var selectedText = '';
        var obj;
        if (calenderType == 0 || calenderType == 2)//double calender  ; no  calender
        {
            // if (calenderType == 0)
            selectedText = _dateOptions[selDateOption];
            //            if (calenderType == 2)
            //                selectedText = _dateOptionsSingleCalender[selDateOption];
            if (selDateOption == 3) { //Any
                startDate = null;
                endDate = null;
            }
            if (selDateOption == 4) { //customdate
                if (selRadioOption == 0 || selRadioOption == 4) //from / On
                    endDate = null;
                else if (selRadioOption == 2) //prior
                {
                    endDate = startDate;
                    startDate = null;
                }
            }
            if (selDateOption == 5) { //Last 15 days
                var NtimeText = $id.children(".smdd_lastNtime_section").find('#lastNtimeid_' + counter).val();
                var NtimeValue = $id.children(".smdd_lastNtime_section").find('#ddllastNtime_' + counter)[0].value;
                obj = setCalender(0, $id, counter, -(NtimeText), NtimeValue);

            }
            if (selDateOption == 6) { //Next 15 days
                var NtimeText = $id.children(".smdd_nextNtime_section").find('#nextNtimeid_' + counter).val();
                var NtimeValue = $id.children(".smdd_nextNtime_section").find('#ddlnextNtime_' + counter)[0].value;
                obj = setCalender(1, $id, counter, NtimeText, NtimeValue);

            }
        }
        else if (calenderType == 1)  //Single calender
        {
            // selectedText = _dateOptionsSingleCalender[selDateOption];
            selectedText = _dateOptions[selDateOption];
        }



        if (obj == undefined)
            NtimeOption = undefined;
        else {
            NtimeOption = obj.NtimeOption;
            NtimeText = obj.NtimeText;
            startDate = obj.NStartDay;
            endDate = obj.NEndDay;
        }


        smdatepickercontrol.selectedText = selectedText;
        smdatepickercontrol.selectedStartDate = startDate;
        smdatepickercontrol.selectedEndDate = endDate;
        smdatepickercontrol.selRadioOption = selRadioOption;
        smdatepickercontrol.selDateOption = selDateOption;
        smdatepickercontrol.NtimeText = NtimeText;
        smdatepickercontrol.NtimeValue = NtimeValue;
        smdatepickercontrol.NtimeOption = NtimeOption;
        smdatepickercontrol.DisplayText = displayText;

        var startDateObject = new Date(smdatepickercontrol.selectedStartDate);
        var endDateObject = new Date(smdatepickercontrol.selectedEndDate);

        var serverStartDate;
        var serverEndDate;
        var serverStartDateOriginal;
        var serverEndDateOriginal;
        var serverStartDateLongDate;

        if (startDateObject != null) {
            serverStartDate = new Date(new Date(startDateObject).setHours(23, 59, 59, 999)).format(serverformat);
            serverStartDateOriginal = startDateObject.format(serverShortformat);
            serverStartDateLongDate = startDateObject.format(serverformat);
        }
        if (endDateObject != null) {
            serverEndDate = endDateObject.format(serverformat);
            serverEndDateOriginal = endDateObject.format(serverShortformat);
        }

        var objInfo = {
            selectedText: smdatepickercontrol.selectedText,
            selectedStartDate: smdatepickercontrol.selectedStartDate,
            selectedEndDate: smdatepickercontrol.selectedEndDate,
            selRadioOption: smdatepickercontrol.selRadioOption,
            selDateOption: smdatepickercontrol.selDateOption,
            NtimeText: smdatepickercontrol.NtimeText,
            NtimeValue: smdatepickercontrol.NtimeValue,
            NtimeOption: smdatepickercontrol.NtimeOption,
            DisplayText: smdatepickercontrol.DisplayText,
            serverStartDate: serverStartDate,
            serverEndDate: serverEndDate,
            serverStartDateOriginal: serverStartDateOriginal,
            serverEndDateOriginal: serverEndDateOriginal,
            serverStartDateLongDate: serverStartDateLongDate
        };

        return objInfo;
    }

    function createHTML($object) {
        var container = "";
        if (!$object.hasOwnProperty("container") || $object.container === "undefined") {
            container = $("body");
        }
        else {
            container = $object.container;
        }
        var counter;

        var obj = {};
        obj[id] = $object.pivotElement;

        if ($object.calenderID != undefined) {
            $("#" + $object.calenderID).remove();
            var index = $object.calenderID.split('_')[1];

            smdatepickercontrol.pivotVsDivid.splice(index, 1);
            smdatepickercontrol.obj.splice(index, 1);

            counter = $object.calenderID.split('_')[1];
            smdatepickercontrol.counter++;

            smdatepickercontrol.pivotVsDivid.splice(index, 0, obj);
            smdatepickercontrol.obj.splice(index, 0, $object);
        }
        else {
            counter = smdatepickercontrol.counter++;
            smdatepickercontrol.pivotVsDivid.push(obj);
            smdatepickercontrol.obj.push($object);

        }
        var id = "smdd_" + (counter);

        smdatepickercontrol.calenderID = id;

        var firstBtnBackground = "";
        var width = '', width2 = '';
        if ($object.dateOptions.length == 7) { width2 = '30%'; }
        else { width2 = '40%'; }
        width = ($object.dateOptions.length * 90) + 'px';
        if ($object.lefttimePicker && $object.righttimePicker)
            width = '690px';
        else if ($object.lefttimePicker || $object.righttimePicker)
            width = '620px';
        else
            width = '550px'
        if ($object.selectedTopOption != undefined)
            var html = "<div id='" + id + "' class='smdd_container' style='display:none;width:" + width + ";' selectedDate='" + $object.selectedTopOption + "'>";
        else
            var html = "<div id='" + id + "' class='smdd_container' style='display:none;;width:" + width + ";'>";
        //Left Section
        html += "<div class='smdd_top_section' style='margin:0 auto;width:auto'>";
        //change in Top Section
        if ($object.LstNxtFreqOptions == 0)//hourly
        { _dateOptions[5] = "Last Days"; _dateOptions[6] = "Next Days" }
        if ($object.LstNxtFreqOptions == 1)//daily
        { _dateOptions[5] = "Last Days"; _dateOptions[6] = "Next Days" }
        if ($object.LstNxtFreqOptions == 2)//weekly
        { _dateOptions[5] = "Last Days"; _dateOptions[6] = "Next Days" }
        if ($object.LstNxtFreqOptions == 3)//monthly
        { _dateOptions[5] = "Last Months"; _dateOptions[6] = "Next Months" }
        if ($object.LstNxtFreqOptions == 4)//quaterly
        { _dateOptions[5] = "Last Quater"; _dateOptions[6] = "Next Quater" }
        if ($object.LstNxtFreqOptions == 5)//yearly
        { _dateOptions[5] = "Last Year"; _dateOptions[6] = "Next Year" }

        for (var i = 0; i < $object.dateOptions.length; i++) {
            //            if (i === 0) {
            //                firstBtnBackground = "style='color:white'";
            //            }
            //            else {
            //                firstBtnBackground = "";
            //            }
            firstBtnBackground = "";
            html += "<button " + firstBtnBackground + " class='smdd_date_option' option='" + $object.dateOptions[i] + "'>" + _dateOptions[$object.dateOptions[i]] + "</button>";
        }
        html += "</div>";
        //Custom date section
        if ($object.selectedCustomRadioOption == "" || $object.selectedCustomRadioOption == undefined)
            if ($object.extraRadioOption == "" || $object.extraRadioOption == undefined)
                html += "<div class='smdd_custom_section' style='visibility: hidden;text-align: center;' selectedRadioOption='0'><div class='radioClass'><input type='radio' name='custom_" + id + "' value='From' checked> From</div><div class='radioClass'><input type='radio' name='custom_" + id + "' value='Between'>Between</div><div class='radioClass'><input type='radio' name='custom_" + id + "' value='Prior'>Prior</div></div>";
            else {
                html += "<div class='smdd_custom_section' style='visibility: hidden;text-align: center;' selectedRadioOption='0'><div class='radioClass'><input type='radio' name='custom_" + id + "' value='From' checked> From</div><div class='radioClass'><input type='radio' name='custom_" + id + "' value='Between'>Between</div><div class='radioClass'><input type='radio' name='custom_" + id + "' value='Prior'>Prior</div>";
                for (var radioOpt = 0; radioOpt < $object.extraRadioOption.length; radioOpt++)
                    html += "<div class='radioClass'><input type='radio' name='custom_" + id + "' value='" + _extraRadioOptions[$object.extraRadioOption[radioOpt]] + "'>" + _extraRadioOptions[$object.extraRadioOption[radioOpt]] + "</div>";
                html += "</div>";
            }

        else
            if ($object.extraRadioOption == "" || $object.extraRadioOption == undefined)
                html += "<div class='smdd_custom_section' style='visibility: hidden;text-align: center;' selectedRadioOption='" + $object.selectedCustomRadioOption + "'><div class='radioClass'><input type='radio' name='custom_" + id + "' value='From' > From</div><div class='radioClass'><input type='radio' name='custom_" + id + "' value='Between'>Between</div><div class='radioClass'><input type='radio' name='custom_" + id + "' value='Prior'>Prior</div></div>";
            else {
                html += "<div class='smdd_custom_section' style='visibility: hidden;text-align: center;' selectedRadioOption='" + $object.selectedCustomRadioOption + "'><div class='radioClass'><input type='radio' name='custom_" + id + "' value='From' > From</div><div class='radioClass'><input type='radio' name='custom_" + id + "' value='Between'>Between</div><div class='radioClass'><input type='radio' name='custom_" + id + "' value='Prior'>Prior</div>";
                for (var radioOpt = 0; radioOpt < $object.extraRadioOption.length; radioOpt++)
                    html += "<div class='radioClass'><input type='radio' name='custom_" + id + "' value='" + _extraRadioOptions[$object.extraRadioOption[radioOpt]] + "'>" + _extraRadioOptions[$object.extraRadioOption[radioOpt]] + "</div>";
                html += "</div>";
            }
        //Last days section
        // html += "<div class='smdd_lastNtime_section' class='lastNtimeDiv' style='visibility: hidden;' selectedlastNtimeOption='0'>Last <div id='lastNtimeid_" + (counter) + "' class='editableDiv' contenteditable='true'>15</div><select id='ddllastNtime_" + (counter) + "' style='width: 150px'><option value='1'>Days</option><option value='2'>Weeks</option><option value='3'>Months</option><option value='5'>Years</option></select></div>";
        //todo : in other two html sahil neetika
        html += "<div class='smdd_lastNtime_section' class='lastNtimeDiv' " + (($object.selectedNtimeOption != undefined) ? "style='visibility: hidden;' " : "style='' ") + " selectedlastNtimeOption='0'>Last <div class='divSpinner'><input id='lastNtimeid_" + (counter) + "' class='ui-spinner-input spinnerInput'  ></div><select id='ddllastNtime_" + (counter) + "'><option value='1'>Days</option><option value='2'>Weeks</option><option value='3'>Months</option><option value='5'>Years</option></select></div>";

        //Next days section
        //html += "<div class='smdd_nextNtime_section' class='nextNtimeDiv' style='visibility: hidden;' selectednextNtimeOption='0'>Next <div id='nextNtimeid_" + (counter) + "' class='editableDiv' contenteditable='true'>15</div><select id='ddlnextNtime_" + (counter) + "' style='width: 150px'><option value='1'>Days</option><option value='2'>Weeks</option><option value='3'>Months</option><option value='5'>Years</option></select></div>";
        html += "<div class='smdd_nextNtime_section' class='nextNtimeDiv'  style='visibility: hidden;' selectednextNtimeOption='0'>Next <div class='divSpinner'><input id='nextNtimeid_" + (counter) + "' class='ui-spinner-input spinnerInput'  ></div><select id='ddlnextNtime_" + (counter) + "'><option value='1'>Days</option><option value='2'>Weeks</option><option value='3'>Months</option><option value='5'>Years</option></select></div>";

        //Middle Section
        html += "<div class='smdd_middle_section'></div><div class='smdd_middle_section_overlay smdd_section_overlay'></div>";

        //Right Section
        html += "<div class='smdd_right_section'></div><div class='smdd_right_section_overlay  smdd_section_overlay'></div>";

        html += "<div class='validationSummary'></div>";
        html += "</div>";

        container.append(html);
        // defaultTime: $object.StartDateCalender.split(' ')[1],
        $('#' + id).find('.smdd_middle_section').datetimepicker({
            format: $object.dateFormat,
            inline: true,
            timepicker: $object.lefttimePicker,
            //defaultDate: new Date($object.StartDateCalender.split(' ')[0]),
            defaultDate: (typeof $object.StartDateCalender == "undefined" || $object.StartDateCalender == null || $object.StartDateCalender == "-1" || $object.StartDateCalender == "") ?
                com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate)) : $object.StartDateCalender,
            defaultTime: $object.StartDateCalender.split(' ')[1],

            onSelectDate: function (ct, $i) {
                $("#" + id).attr("selectedStartDate", ct);
            },
            onGenerate: function (ct, $i) {
                $("#" + id).attr("selectedStartDate", ct);
            }
        });

        $('#' + id).find('.smdd_right_section').datetimepicker({
            format: $object.dateFormat,
            inline: true,
            timepicker: $object.righttimePicker,
            //defaultDate: new Date($object.EndDateCalender.split(' ')[0]),
            defaultDate: (typeof $object.StartDateCalender == "undefined" || $object.StartDateCalender == null || $object.StartDateCalender == "-1" || $object.StartDateCalender == "") ?
                com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate)) : $object.StartDateCalender,
            defaultTime: $object.EndDateCalender.split(' ')[1],
            onSelectDate: function (ct, $i) {
                $("#" + id).attr("selectedEndDate", ct);
            },
            onGenerate: function (ct, $i) {
                $("#" + id).attr("selectedEndDate", ct);
            }
        });
        $('#' + id + ' .xdsoft_calendar').addClass('changedCalender');

        if ($object.hasOwnProperty('ready') && typeof $object.ready === 'function') {
            $object.ready($(smdatepickercontrol));
        }
        $("#lastNtimeid_" + (counter)).spinner({ min: 1 });
        $("#nextNtimeid_" + (counter)).spinner({ min: 1 });

        if ($object.selectedTopOption == 5) {
            if (($object.selectedNtimeText == "" || $object.selectedNtimeText == undefined)) { $('#lastNtimeid_' + counter).val('15'); $('#nextNtimeid_' + counter).val('15'); }
            else { $('#lastNtimeid_' + counter).val($object.selectedNtimeText); smselect.setOptionByText($('#smselect_ddllastNtime_' + counter), $object.selectedNtimeOption); }
        }
        else if ($object.selectedTopOption == 6) {
            if (($object.selectedNtimeText == "" || $object.selectedNtimeText == undefined)) { $('#nextNtimeid_' + counter).val('15'); $('#lastNtimeid_' + counter).val('15'); }
            else { $('#nextNtimeid_' + counter).val($object.selectedNtimeText); smselect.setOptionByText($('#smselect_ddlnextNtime_' + counter), $object.selectedNtimeOption); }
        }
        else { $('#lastNtimeid_' + counter).val('15'); $('#nextNtimeid_' + counter).val('15'); }
        //Attach Event Handlers
        attachEventHandlers($object, id)
        if ($object.selectedTopOption != undefined)
            $("#" + id + " button[option = '" + $object.selectedTopOption + "']").click();
    }

    function createHTMLforSingleCalender($object) {
        var container = "";
        if (!$object.hasOwnProperty("container") || $object.container === "undefined") {
            container = $("body");
        }
        else {
            container = $object.container;
        }
        var counter;



        var obj = {};
        obj[id] = $object.pivotElement;

        if ($object.calenderID != undefined) {
            $("#" + $object.calenderID).remove();
            var index = $object.calenderID.split('_')[1];
            smdatepickercontrol.pivotVsDivid.splice(index, 1);
            smdatepickercontrol.obj.splice(index, 1);
            counter = $object.calenderID.split('_')[1];
            smdatepickercontrol.counter++;

            smdatepickercontrol.pivotVsDivid.splice(index, 0, obj);
            smdatepickercontrol.obj.splice(index, 0, $object);
        }
        else {
            counter = smdatepickercontrol.counter++;
            smdatepickercontrol.pivotVsDivid.push(obj);
            smdatepickercontrol.obj.push($object);

        }
        var id = "smdd_" + (counter);

        var firstBtnBackground = "";
        if ($object.selectedTopOption != undefined)
            var html = "<div id='" + id + "' class='smdd_container' style='display:none;' selectedDate='" + $object.selectedTopOption + "'>";
        else
            var html = "<div id='" + id + "' class='smdd_container' style='display:none;'>";
        width = ($object.dateOptions.length * 90) + 'px';
        //Left Section
        html += "<div class='smdd_top_section' style='margin-bottom:30px !important;margin:0 auto;width:" + width + "'>";
        for (var i = 0; i < $object.dateOptions.length; i++) {
            //            if (i === 0) {
            //                firstBtnBackground = "style='color:white'";
            //            }
            //            else {
            //                firstBtnBackground = "";
            //            }
            //     html += "<button " + firstBtnBackground + " class='smdd_date_option' option='" + $object.dateOptions[i] + "'>" + _dateOptionsSingleCalender[$object.dateOptions[i]] + "</button>";
            html += "<button " + firstBtnBackground + " class='smdd_date_option' option='" + $object.dateOptions[i] + "'>" + _dateOptions[$object.dateOptions[i]] + "</button>";

        }
        html += "</div>";
        //Custom date section
        html += "<div class='smdd_custom_section' style='visibility:hidden' selectedRadioOption='0'><input type='radio' name='custom' value='From' checked> From<input type='radio' name='custom' value='Between'>Between<input type='radio' name='custom' value='Prior'>Prior</div>";
        //Last days section
        html += "<div class='smdd_lastNtime_section' class='lastNtimeDiv' style='display:none' selectedlastNtimeOption='0'>Last <div id='lastNtimeid_" + (counter) + "' class='editableDiv' contenteditable='true'>15</div><select id='ddllastNtime_" + (counter) + "' style='width: 150px'><option value='1'>Days</option><option value='2'>Weeks</option><option value='3'>Months</option><option value='5'>Years</option></select></div>";
        //Next days section
        html += "<div class='smdd_nextNtime_section' class='nextNtimeDiv' style='display:none' selectednextNtimeOption='0'>Next <div id='nextNtimeid_" + (counter) + "' class='editableDiv' contenteditable='true'>15</div><select id='ddlnextNtime_" + (counter) + "' style='width: 150px'><option value='1'>Days</option><option value='2'>Weeks</option><option value='3'>Months</option><option value='5'>Years</option></select></div>";
        //Middle Section
        html += "<div class='smdd_middle_section'></div><div class='smdd_middle_section_overlay smdd_section_overlay'></div>";

        //        //Right Section
        //        html += "<div class='smdd_right_section'></div><div class='smdd_right_section_overlay  smdd_section_overlay'></div>";

        //        html += "<div><button class='smdd_green_btn smdd_apply_btn'>Apply</button></div>";
        html += "</div>";

        container.append(html);
        $('#' + id).find('.smdd_middle_section').datetimepicker({
            format: $object.dateFormat,
            inline: true,
            timepicker: $object.lefttimePicker,
            onSelectDate: function (ct, $i) {
                $("#" + id).attr("selectedStartDate", ct);
            },
            onGenerate: function (ct, $i) {
                $("#" + id).attr("selectedStartDate", ct);
            }
        });

        //        $('#' + id).find('.smdd_right_section').datetimepicker({
        //            format: $object.dateFormat,
        //            inline: true,
        //            timepicker: false,
        //            onSelectDate: function (ct, $i) {
        //                $("#" + id).attr("selectedEndDate", ct);
        //            },
        //            onGenerate: function (ct, $i) {
        //                $("#" + id).attr("selectedEndDate", ct);
        //            }
        //        });

        if ($object.hasOwnProperty('ready') && typeof $object.ready === 'function') {
            $object.ready($(smdatepickercontrol));
        }

        //Attach Event Handlers
        attachEventHandlersforSingleCalender($object, id)
        if ($object.selectedTopOption != undefined)
            $("#" + id + " button[option = '" + $object.selectedTopOption + "']").click();
    }

    function createHTMLforNoCalender($object) {
        var counter;
        var obj = {};
        obj[id] = $object.pivotElement;
        var id = $object.calenderID;
        if ($object.calenderID != undefined) {
            $("#" + $object.calenderID).remove();
            var index = $object.calenderID.split('_')[1];
            smdatepickercontrol.pivotVsDivid.splice(index, 1);
            smdatepickercontrol.obj.splice(index, 1);
            counter = $object.calenderID.split('_')[1];
            smdatepickercontrol.counter++;
            id = $object.calenderID;
            smdatepickercontrol.pivotVsDivid.splice(index, 0, obj);
            smdatepickercontrol.obj.splice(index, 0, $object);
        }
        else {
            counter = smdatepickercontrol.counter++;
            smdatepickercontrol.pivotVsDivid.push(obj);
            smdatepickercontrol.obj.push($object);
            id = "smdd_" + (counter);
        }
        var container = "";
        //ToDo : if smdd2 exists in body , destroy it 
        if (!$object.hasOwnProperty("container") || $object.container === "undefined") {
            container = $("body");
        }
        else {
            container = $object.container;
        }
        var firstBtnBackground = "";
        var width = '', width2 = '';
        if ($object.dateOptions.length == 7) { width2 = '30%'; }
        else { width2 = '40%'; }
        width = ($object.dateOptions.length * 95) + 'px';
        if ($object.selectedTopOption != undefined)
            var html = "<div id='" + id + "' class='smdd_container' style='display:none;height:130px;' DisplayText='' selectedDate='" + $object.selectedTopOption + "'>";
        else
            var html = "<div id='" + id + "' class='smdd_container' style='display:none;height:130px;'>";
        //Left Section
        html += "<div class='smdd_top_section' style='margin:0 auto;width:" + width + "'>";

        //change in Top Section
        if ($object.LstNxtFreqOptions == 3)//monthly
        { _dateOptions[5] = "Last Months"; _dateOptions[6] = "Next Months" }
        if ($object.LstNxtFreqOptions == 4)//quaterly
        { _dateOptions[5] = "Last Quater"; _dateOptions[6] = "Next Quater" }
        if ($object.LstNxtFreqOptions == 5)//yearly
        { _dateOptions[5] = "Last Year"; _dateOptions[6] = "Next Year" }

        for (var i = 0; i < $object.dateOptions.length; i++) {
            //            if (i === 0) {
            //                firstBtnBackground = "style='color:white'";
            //            }
            //            else {
            //                firstBtnBackground = "";
            //            }
            html += "<button " + firstBtnBackground + " class='smdd_date_option' option='" + $object.dateOptions[i] + "'>" + _dateOptions[$object.dateOptions[i]] + "</button>";
        }
        html += "</div>";

        if ($object.LstNxtFreqOptions == 3)//monthly
        {
            var btwnSelect1ddl = "<div><select id=btwnSelect1ddl_" + $object.calenderID + ">";
            for (var i in months)
                btwnSelect1ddl += "<option value=" + i + ">" + months[i] + "</option>";
            btwnSelect1ddl += "</select></div>";

            var btwnTimeSelect1ddl = "<div><select id=btwnTimeSelect1ddl_" + $object.calenderID + ">";
            for (var i in years)
                btwnTimeSelect1ddl += "<option value=" + i + ">" + years[i] + "</option>";
            btwnTimeSelect1ddl += "</select></div>";

            var btwnSelect2ddl = "<div><select id=btwnSelect2ddl_" + $object.calenderID + ">";
            for (var i in months)
                btwnSelect2ddl += "<option value=" + i + ">" + months[i] + "</option>";
            btwnSelect2ddl += "</select></div>";

            var btwnTimeSelect2ddl = "<div><select id=btwnTimeSelect2ddl_" + $object.calenderID + ">";
            for (var i in years)
                btwnTimeSelect2ddl += "<option value=" + i + ">" + years[i] + "</option>";
            btwnTimeSelect2ddl += "</select></div>";

            var fromSelect1ddl = "<div><select id=fromSelect1ddl_" + $object.calenderID + ">";
            for (var i in months)
                fromSelect1ddl += "<option value=" + i + ">" + months[i] + "</option>";
            fromSelect1ddl += "</select></div>";

            var fromTimeSelect1ddl = "<div><select id=fromTimeSelect1ddl_" + $object.calenderID + ">";
            for (var i in years)
                fromTimeSelect1ddl += "<option value=" + i + ">" + years[i] + "</option>";
            fromTimeSelect1ddl += "</select></div>";

            var priorSelect1ddl = "<div><select id=priorSelect1ddl_" + $object.calenderID + ">";
            for (var i in months)
                priorSelect1ddl += "<option value=" + i + ">" + months[i] + "</option>";
            priorSelect1ddl += "</select></div>";
            var priorTimeSelect1ddl = "<div><select id=priorTimeSelect1ddl_" + $object.calenderID + ">";
            for (var i in years)
                priorTimeSelect1ddl += "<option value=" + i + ">" + years[i] + "</option>";
            priorTimeSelect1ddl += "</select></div>";

        }
        else if ($object.LstNxtFreqOptions == 4)//quaterly
        {
            var btwnSelect1ddl = "<div><select id=btwnSelect1ddl_" + $object.calenderID + ">";
            for (var i in quaters)
                btwnSelect1ddl += "<option value=" + i + ">" + quaters[i] + "</option>";
            btwnSelect1ddl += "</select></div>";

            var btwnTimeSelect1ddl = "<div><select id=btwnTimeSelect1ddl_" + $object.calenderID + ">";
            for (var i in years)
                btwnTimeSelect1ddl += "<option value=" + i + ">" + years[i] + "</option>";
            btwnTimeSelect1ddl += "</select></div>";

            var btwnSelect2ddl = "<div><select id=btwnSelect2ddl_" + $object.calenderID + ">";
            for (var i in quaters)
                btwnSelect2ddl += "<option value=" + i + ">" + quaters[i] + "</option>";
            btwnSelect2ddl += "</select></div>";

            var btwnTimeSelect2ddl = "<div><select id=btwnTimeSelect2ddl_" + $object.calenderID + ">";
            for (var i in years)
                btwnTimeSelect2ddl += "<option value=" + i + ">" + years[i] + "</option>";
            btwnTimeSelect2ddl += "</select></div>";

            var fromSelect1ddl = "<div><select id=fromSelect1ddl_" + $object.calenderID + ">";
            for (var i in quaters)
                fromSelect1ddl += "<option value=" + i + ">" + quaters[i] + "</option>";
            fromSelect1ddl += "</select></div>";

            var fromTimeSelect1ddl = "<div><select id=fromTimeSelect1ddl_" + $object.calenderID + ">";
            for (var i in years)
                fromTimeSelect1ddl += "<option value=" + i + ">" + years[i] + "</option>";
            fromTimeSelect1ddl += "</select></div>";

            var priorSelect1ddl = "<div><select id=priorSelect1ddl_" + $object.calenderID + ">";
            for (var i in quaters)
                priorSelect1ddl += "<option value=" + i + ">" + quaters[i] + "</option>";
            priorSelect1ddl += "</select></div>";
            var priorTimeSelect1ddl = "<div><select id=priorTimeSelect1ddl_" + $object.calenderID + ">";
            for (var i in years)
                priorTimeSelect1ddl += "<option value=" + i + ">" + years[i] + "</option>";
            priorTimeSelect1ddl += "</select></div>";

        }
        else if ($object.LstNxtFreqOptions == 5)//yearly
        {
            var btwnSelect1ddl = "<div style='display:none'><select id=btwnSelect1ddl_" + $object.calenderID + ">";
            btwnSelect1ddl += "</select></div>";

            var btwnTimeSelect1ddl = "<div><select id=btwnTimeSelect1ddl_" + $object.calenderID + ">";
            for (var i in years)
                btwnTimeSelect1ddl += "<option value=" + i + ">" + years[i] + "</option>";
            btwnTimeSelect1ddl += "</select></div>";

            var btwnSelect2ddl = " <div style='display:none'><select id=btwnSelect2ddl_" + $object.calenderID + ">";
            btwnSelect2ddl += "</select></div>";

            var btwnTimeSelect2ddl = "<div><select id=btwnTimeSelect2ddl_" + $object.calenderID + ">";
            for (var i in years)
                btwnTimeSelect2ddl += "<option value=" + i + ">" + years[i] + "</option>";
            btwnTimeSelect2ddl += "</select></div>";

            var fromSelect1ddl = "<div style='display:none'><select id=fromSelect1ddl_" + $object.calenderID + ">";
            fromSelect1ddl += "</select></div>";

            var fromTimeSelect1ddl = "<div><select id=fromTimeSelect1ddl_" + $object.calenderID + ">";
            for (var i in years)
                fromTimeSelect1ddl += "<option value=" + i + ">" + years[i] + "</option>";
            fromTimeSelect1ddl += "</select></div>";

            var priorSelect1ddl = "<div style='display:none'><select id=priorSelect1ddl_" + $object.calenderID + ">";
            priorSelect1ddl += "</select></div>";

            var priorTimeSelect1ddl = "<div><select id=priorTimeSelect1ddl_" + $object.calenderID + ">";
            for (var i in years)
                priorTimeSelect1ddl += "<option value=" + i + ">" + years[i] + "</option>";
            priorTimeSelect1ddl += "</select></div>";

        }
        //Custom date section
        //Custom date section
        if ($object.selectedCustomRadioOption == "" || $object.selectedCustomRadioOption == undefined)
            html += "<div class='smdd_custom_section' style='visibility: hidden;width:70%' selectedRadioOption='0'><div class='radioClass' ><div  class='selectddl'><input type='radio' name='custom_" + id + "' value='From' checked> From</div> <div class='selectddl'>" + fromSelect1ddl + "</div><div class='selectddl'>" + fromTimeSelect1ddl + "</div></div><br/><div class='radioClass' ><div  class='selectddl'><input type='radio' name='custom_" + id + "' value='Between'>Between</div> <div class='selectddl'>" + btwnSelect1ddl + "</div><div class='selectddl'>" + btwnTimeSelect1ddl + "</div>to<div class='selectddl'>" + btwnSelect2ddl + "</div><div class='selectddl'>" + btwnTimeSelect2ddl + "</div></div><div class='radioClass' ><div  class='selectddl'><input type='radio' name='custom_" + id + "' value='Prior'>Prior to</div> <div class='selectddl'>" + priorSelect1ddl + "</div><div class='selectddl'>" + priorTimeSelect1ddl + "</div></div></div>";
        else
            html += "<div class='smdd_custom_section' style='visibility: hidden;width:70%' selectedRadioOption='" + $object.selectedCustomRadioOption + "'><div class='radioClass' ><div  class='selectddl'><input type='radio' name='custom_" + id + "' value='From' checked> From</div> <div class='selectddl'>" + fromSelect1ddl + "</div><div class='selectddl'>" + fromTimeSelect1ddl + "</div></div><br/><div class='radioClass' ><div  class='selectddl'><input type='radio' name='custom_" + id + "' value='Between'>Between</div> <div class='selectddl'>" + btwnSelect1ddl + "</div><div class='selectddl'>" + btwnTimeSelect1ddl + "</div>to<div class='selectddl'>" + btwnSelect2ddl + "</div><div class='selectddl'>" + btwnTimeSelect2ddl + "</div></div><div class='radioClass' ><div  class='selectddl'><input type='radio' name='custom_" + id + "' value='Prior'>Prior to</div> <div class='selectddl'>" + priorSelect1ddl + "</div><div class='selectddl'>" + priorTimeSelect1ddl + "</div></div></div>";


        // html += "<div class='smdd_custom_section' style='visibility: hidden;width:70%' selectedRadioOption='0'><div class='radioClass' ><div  class='selectddl'><input type='radio' name='custom_" + id + "' value='From' checked> From</div> <div class='selectddl'>" + fromSelect1ddl + "</div><div class='selectddl'>" + fromTimeSelect1ddl + "</div></div><br/><div class='radioClass' ><div  class='selectddl'><input type='radio' name='custom_" + id + "' value='Between'>Between</div> <div class='selectddl'>" + btwnSelect1ddl + "</div><div class='selectddl'>" + btwnTimeSelect1ddl + "</div>to<div class='selectddl'>" + btwnSelect2ddl + "</div><div class='selectddl'>" + btwnTimeSelect2ddl + "</div></div><div class='radioClass' ><div  class='selectddl'><input type='radio' name='custom_" + id + "' value='Prior'>Prior to</div> <div class='selectddl'>" + priorSelect1ddl + "</div><div class='selectddl'>" + priorTimeSelect1ddl + "</div></div></div>";
        //        //Last days section
        //        html += "<div class='smdd_lastNtime_section' class='lastNtimeDiv' style='visibility: hidden;top:-40px;' selectedlastNtimeOption='0'>Last <div id='lastNtimeid_" + (counter) + "' class='editableDiv' contenteditable='true'>15</div><select id='ddllastNtime_" + (counter) + "' style='width: 150px'><option value='" + $object.LstNxtFreqOptions + "'>" + _lstNxtFreqOptions[$object.LstNxtFreqOptions] + "</option></select></div>";
        //        //Next days section
        //        html += "<div class='smdd_nextNtime_section' class='nextNtimeDiv' style='visibility: hidden;top:-62px;' selectednextNtimeOption='0'>Next <div id='nextNtimeid_" + (counter) + "' class='editableDiv' contenteditable='true'>15</div><select id='ddlnextNtime_" + (counter) + "' style='width: 150px'><option value='" + $object.LstNxtFreqOptions + "'>" + _lstNxtFreqOptions[$object.LstNxtFreqOptions] + "</option></select></div>";

        html += "<div class='smdd_lastNtime_section' class='lastNtimeDiv' style='visibility: hidden;' selectedlastNtimeOption='0'>Last <div class='divSpinner'><input id='lastNtimeid_" + (counter) + "' class='ui-spinner-input spinnerInput'  ></div><select id='ddllastNtime_" + (counter) + "' style='width: 150px'><option value='" + $object.LstNxtFreqOptions + "'>" + _lstNxtFreqOptions[$object.LstNxtFreqOptions] + "</option></select></div>";
        html += "<div class='smdd_nextNtime_section' class='nextNtimeDiv' style='visibility: hidden;' selectednextNtimeOption='0'>Next <div class='divSpinner'><input id='nextNtimeid_" + (counter) + "' class='ui-spinner-input spinnerInput'  ></div><select id='ddlnextNtime_" + (counter) + "' style='width: 150px'><option value='" + $object.LstNxtFreqOptions + "'>" + _lstNxtFreqOptions[$object.LstNxtFreqOptions] + "</option></select></div>";


        //        html += "<div><button class='smdd_green_btn smdd_apply_btn'>Apply</button></div>";
        html += "</div>";

        container.append(html);

        if ($object.hasOwnProperty('ready') && typeof $object.ready === 'function') {
            $object.ready($(smdatepickercontrol));
        }

        $("#lastNtimeid_" + (counter)).spinner({ min: 1 });
        $("#nextNtimeid_" + (counter)).spinner({ min: 1 });

        //Attach Event Handlers
        attachEventHandlersForNoCalender($object, id)
        if ($object.selectedTopOption != undefined)
            $("#" + id + " button[option = '" + $object.selectedTopOption + "']").click();
    }



    //type- last (0) , next(1) ;text - no of days ; value - month / day / week
    function setCalender(type, id, counter, text, value) {

        var StartDay, EndDay;
        if (type == 0) {//last

            if (text == 0) {
                text = "-15";
                $('#lastNtimeid_' + counter).val('15');
            }

            var promise = getServerDate();
            promise.success(function (data) {
                var serverDate = data.d;
                var today = new Date(new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt")));
                if (value == 1 || value.toUpperCase() == "DAYS") { text = parseInt(text, 10) + 1; NtimeOption = "days"; StartDay = new Date(today.getFullYear(), today.getMonth(), (today.getDate() + parseInt(text, 10))); text = -(parseInt(text, 10) - 1) }
                else if (value == 2 || value.toUpperCase() == "WEEKS") { NtimeOption = "weeks"; StartDay = new Date(today.getFullYear(), today.getMonth(), (today.getDate() + (7 * parseInt(text, 10)))); text = -(parseInt(text, 10)) }
                else if (value == 3 || value.toUpperCase() == "MONTHS") { NtimeOption = "months"; StartDay = new Date(today.getFullYear(), today.getMonth() + parseInt(text, 10), today.getDate()); text = -(parseInt(text, 10)) }
                else if (value == 4 || value.toUpperCase() == "QUATERS") { NtimeOption = "quaters"; StartDay = new Date(today.getFullYear(), today.getMonth() + (3 * parseInt(text, 10)), today.getDate()); text = -(parseInt(text, 10)) }
                else if (value == 5 || value.toUpperCase() == "YEARS") { NtimeOption = "years"; StartDay = new Date(today.getFullYear() + parseInt(text, 10), today.getMonth(), today.getDate()); text = -(parseInt(text, 10)) }
                StartDay = new Date(StartDay.setHours(0, 0, 0, 0));
                // var yesterday = today.setDate(today.getDate() - 1);
                EndDay = new Date(new Date(today).setHours(23, 59, 59, 999));

            });
            return { NtimeText: text, NtimeOption: NtimeOption, NEndDay: EndDay, NStartDay: StartDay }

        }
        else //next
        {

            if (text == 0) {
                text = "15";
                $('#nextNtimeid_' + counter).val('15');
            }

            var promise = getServerDate();
            promise.success(function (data) {
                var serverDate = data.d;
                var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                if (value == 1 || value.toUpperCase() == "DAYS") { text = parseInt(text, 10) - 1; NtimeOption = "days"; StartDay = new Date(today.getFullYear(), today.getMonth(), (today.getDate() + parseInt(text, 10))); text = parseInt(text, 10) + 1 }
                else if (value == 2 || value.toUpperCase() == "WEEKS") { NtimeOption = "weeks"; StartDay = new Date(today.getFullYear(), today.getMonth(), (today.getDate() + (7 * parseInt(text, 10)))) }
                else if (value == 3 || value.toUpperCase() == "MONTHS") { NtimeOption = "months"; StartDay = new Date(today.getFullYear(), today.getMonth() + parseInt(text, 10), today.getDate()) }
                else if (value == 4 || value.toUpperCase() == "QUATERS") { NtimeOption = "quaters"; StartDay = new Date(today.getFullYear(), today.getMonth() + (3 * parseInt(text, 10)), today.getDate()) }
                else if (value == 5 || value.toUpperCase() == "YEARS") { NtimeOption = "years"; StartDay = new Date(today.getFullYear() + parseInt(text, 10), today.getMonth(), today.getDate()) }
                EndDay = new Date(StartDay.setHours(23, 59, 59, 999));
                //  var tomorow = today.setDate(today.getDate() + 1);
                StartDay = new Date(new Date(today).setHours(0, 0, 0, 0));

            });
            return { NtimeText: text, NtimeOption: NtimeOption, NEndDay: EndDay, NStartDay: StartDay }
        }

    }
    function setDateFunc(freq, customOption, val1, val2, val3, val4) {
        var StartDay, EndDay;

        if (freq == 3) { //monthly
            if (customOption == 0) { //From
                StartDay = new Date(val1, val2, 1)
                EndDay = null;
            }
            else if (customOption == 1) { //Between
                if (val1 < val3) {
                    StartDay = new Date(val1, val2, 1);
                    EndDay = new Date(new Date(val3, parseInt(val4, 10) + 1, 0).setHours(23, 59, 59, 999));
                }
                else if (val1 > val3) {
                    StartDay = new Date(val3, val4, 1);
                    EndDay = new Date(new Date(val1, parseInt(val2, 10) + 1, 0).setHours(23, 59, 59, 999));
                }
                else if (val1 == val3) {
                    if (val2 < val4) {
                        StartDay = new Date(val1, val2, 1);
                        EndDay = new Date(new Date(val1, parseInt(val4, 10) + 1, 0).setHours(23, 59, 59, 999));
                    }
                    else if (val2 > val4) {
                        StartDay = new Date(val1, val4, 1);
                        EndDay = new Date(new Date(val1, parseInt(val2, 10) + 1, 0).setHours(23, 59, 59, 999));
                    }
                    else if (val2 == val4) {
                        StartDay = new Date(val1, val4, 1);
                        EndDay = new Date(new Date(val1, parseInt(val2, 10) + 1, 0).setHours(23, 59, 59, 999));
                    }
                }

            }
            else if (customOption == 2) { //Prior
                StartDay = null;
                EndDay = new Date(new Date(val1, parseInt(val2, 10) + 1, 0).setHours(23, 59, 59, 999));

            }
        }
        else if (freq == 4) { //quaterly
            if (customOption == 0) { //From
                StartDay = new Date(val1, (3 * val2) - 3, 1)
                EndDay = null;
            }
            else if (customOption == 1) { //Between
                if (val1 < val3) { var yr1 = val1; var yr2 = val3; var qm1 = val2; var qm2 = val4; }
                else if (val1 > val3) { var yr1 = val3; var yr2 = val1; var qm1 = val4; var qm2 = val2; }
                else if (val1 == val3) {
                    var yr1 = val1; var yr2 = val3;
                    if (val4 < val2) { var qm1 = val4; var qm2 = val2; }
                    else { var qm1 = val2; var qm2 = val4; }
                }
                //   StartDay = new Date(yr1, (qm1 * 2) + (qm2 - 2) + 1, 1);
                StartDay = new Date(yr1, (3 * qm1) - 3, 1);
                EndDay = new Date(new Date(yr2, (qm2 * 3), 0).setHours(23, 59, 59, 999));

            }
            else if (customOption == 2) { //Prior
                StartDay = null;
                //    EndDay = new Date(new Date(val1, (val2 * 2) + (val2 - 2) + 2, 0).setHours(23, 59, 59, 999));

                EndDay = new Date(new Date(val1, ((parseInt(val2, 10) - 1) * 3), 0).setHours(23, 59, 59, 999));
            }
        }
        else if (freq == 5) { //yearly
            if (customOption == 0) { //From
                StartDay = new Date(val1, 0, 1);
                EndDay = null;
            }
            else if (customOption == 1) { //Between

                StartDay = new Date(val1, 0, 1); //jan
                EndDay = new Date(new Date(val3, 12, 0).setHours(23, 59, 59, 999)); //dec

            }
            else if (customOption == 2) { //Prior
                StartDay = null;
                EndDay = new Date(new Date(val1, 0, 0).setHours(23, 59, 59, 999));


            }
        }

        return { StartDay: StartDay, EndDay: EndDay }

    }
    function attachEventHandlers($object, id) {
        var counter = id.split('_')[1];
        //Click handler for the pivot element
        $object.pivotElement.unbind("click", {
            name: "dateControl"
        }).bind("click", {
            name: "dateControl"
        }, function (e) {
            $(".smdd_container").hide();
            $(".smdd_container").each(function (index, element) {
                if ($(element).attr("id") !== id) {
                    $(element).hide();
                }
                else {
                    $object.pivotElement = $(e.target);
                    var windowHeight = $(window).height();
                    var windowWidth = $(window).width();
                    var top = "";
                    var left = "";
                    var choosenOption = parseInt($("#" + id).attr("selectedDate"));
                    if (choosenOption == 3)
                        $("#" + id).find(".smdd_top_section").siblings('.xdsoft_datetimepicker').find('.xdsoft_current').addClass('xdsoft_current_Custom');
                    if (($object.pivotElement.offset().top + $("#" + id).height()) > windowHeight) {
                        top = $object.pivotElement.offset().top - ($("#" + id).height() + 2);
                    }
                    else {
                        top = $object.pivotElement.offset().top + 22;
                    }

                    if (($object.pivotElement.offset().left + $("#" + id).width()) > windowWidth) {
                        left = ($object.pivotElement.offset().left + $object.pivotElement.width()) - $("#" + id).width();
                    }
                    else {
                        left = $object.pivotElement.offset().left - 30;
                    }

                    $("#" + id).css("left", left + "px");
                    $("#" + id).css("top", top + "px");
                    $("#" + id).find(".smdd_middle_section_overlay").css('top', '80px');
                    $("#" + id).find(".smdd_right_section_overlay").css('top', '80px');
                    $("#" + id).find(".smdd_section_overlay").width('260px');

                    $("#" + id).find(".smdd_middle_section_overlay").css('left', '30px');
                    if ($object.lefttimePicker && $object.righttimePicker) {
                        $("#" + id).find(".smdd_section_overlay").width('325px');
                        $("#" + id).find(".smdd_right_section_overlay").css('left', '600px');
                    } else {
                        if ($object.lefttimePicker) {
                            $("#" + id).find(".smdd_middle_section_overlay").width('325px');
                            $("#" + id).find(".smdd_right_section_overlay").css('left', '600px');
                        }
                        else if ($object.righttimePicker) {
                            $("#" + id).find(".smdd_right_section_overlay").width('325px');
                            $("#" + id).find(".smdd_right_section_overlay").css('left', '550px');
                        }
                        else
                            $("#" + id).find(".smdd_right_section_overlay").css('left', '550px');
                    }



                    $("#" + id).toggle();
                }

                e.stopPropagation();
            })
        });

        if ($object.hasOwnProperty("applyCallback") && typeof $object.applyCallback === "function") {
            //            $object.applyCallback();
            $(document).unbind("click", docHandler).bind("click", docHandler);
        }
        $(document).unbind("click", docHandler).bind("click", docHandler);
        $("#" + id).find(".smdd_top_section").find("button").unbind("click").bind("click", function (event) {
            var target = $(event.target);
            target.addClass("dateSelected");
            var counter = $("#" + id)[0].id.split('_')[1];
            $("#" + id).attr("selectedDate", target.attr("option"))
            var choosenOption = parseInt(target.attr("option"));
            $('#' + id + ' .smselectcon').hide();
            //To change the background-color of all the other Btns
            $(this).siblings("button").css("background-color", "white");
            $(this).siblings("button").css("color", "#4197ce");
            //To change the background-color of the selected Btn
            $(this).css("background-color", "#4197ce");
            $(this).css("color", "white");
            $(this).parents(".smdd_top_section").siblings('.xdsoft_datetimepicker').find('.xdsoft_current,.xdsoft_current_Custom').removeClass('xdsoft_current_Custom');
            switch (choosenOption) {
                case 0: //Today
                    $(this).parents(".smdd_top_section").siblings(".smdd_lastNtime_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_nextNtime_section").css('visibility', 'hidden');

                    var promise = getServerDate();
                    promise.success(function (data) {
                        var serverDate = data.d;
                        var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                        var endofToday = new Date(today.setHours(23, 59, 59, 999));
                        var today = new Date(today.setHours(0, 0, 0, 0));

                        $("#" + id).find('.smdd_middle_section').datetimepicker({ value: today });
                        $("#" + id).find('.smdd_right_section').datetimepicker({ value: endofToday });
                    });
                    $(this).parents(".smdd_top_section").siblings(".smdd_middle_section_overlay").show();
                    $(this).parents(".smdd_top_section").siblings(".smdd_right_section_overlay").show();
                    //  $(this).parents(".smdd_top_section").siblings('.smdd_custom_section').attr('selectedRadioOption', '1') //between
                    break;
                case 1: //Since yesterday
                    $(this).parents(".smdd_top_section").siblings(".smdd_lastNtime_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_nextNtime_section").css('visibility', 'hidden');

                    var promise = getServerDate();
                    promise.success(function (data) {
                        var serverDate = data.d;
                        var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                        var yesterday = new Date(today);
                        yesterday.setDate(today.getDate() - 1);
                        var yesterday = new Date(yesterday.setHours(0, 0, 0, 0));
                        var endofToday = new Date(today.setHours(23, 59, 59, 999));
                        $("#" + id).find('.smdd_middle_section').datetimepicker({ value: yesterday });
                        $("#" + id).find('.smdd_right_section').datetimepicker({ value: endofToday });
                    });
                    $(this).parents(".smdd_top_section").siblings(".smdd_middle_section_overlay").show();
                    $(this).parents(".smdd_top_section").siblings(".smdd_right_section_overlay").show();
                    //       $(this).parents(".smdd_top_section").siblings('.smdd_custom_section').attr('selectedRadioOption', '1') //between
                    break;
                case 2: //This week
                    $(this).parents(".smdd_top_section").siblings(".smdd_lastNtime_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_nextNtime_section").css('visibility', 'hidden');

                    var promise = getServerDate();
                    promise.success(function (data) {
                        var serverDate = data.d;
                        var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                        var current = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                        $("#" + id).find('.smdd_right_section').datetimepicker({ value: today });
                        var weekstart = current.getDate() - current.getDay() + 1;
                        var monday = new Date(current.setDate(weekstart));
                        var monday = new Date(monday.setHours(0, 0, 0, 0));
                        var endofToday = new Date(today.setHours(23, 59, 59, 999));
                        $("#" + id).find('.smdd_middle_section').datetimepicker({ value: monday });
                        $("#" + id).find('.smdd_right_section').datetimepicker({ value: endofToday });
                    });
                    $(this).parents(".smdd_top_section").siblings(".smdd_middle_section_overlay").show();
                    $(this).parents(".smdd_top_section").siblings(".smdd_right_section_overlay").show();

                    //       $(this).parents(".smdd_top_section").siblings('.smdd_custom_section').attr('selectedRadioOption', '1') //between
                    break;
                case 3: //Any
                    $(this).parents(".smdd_top_section").siblings(".smdd_lastNtime_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_nextNtime_section").css('visibility', 'hidden');

                    var promise = getServerDate();
                    promise.success(function (data) {
                        var serverDate = data.d;
                        var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));

                    });
                    $(this).parents(".smdd_top_section").siblings('.xdsoft_datetimepicker').find('.xdsoft_current').addClass('xdsoft_current_Custom');
                    $(this).parents(".smdd_top_section").siblings(".smdd_middle_section_overlay").show();
                    $(this).parents(".smdd_top_section").siblings(".smdd_right_section_overlay").show();
                    break;
                case 4: //custom date
                    $(this).parents(".smdd_top_section").siblings(".smdd_nextNtime_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_lastNtime_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").css('visibility', 'visible');
                    $(this).parents(".smdd_top_section").siblings(".smdd_middle_section_overlay").hide();
                    $(this).parents(".smdd_top_section").siblings(".smdd_right_section_overlay").show();

                    var radioOption = $("#" + id).find(".smdd_custom_section").attr('selectedRadioOption');

                    if (typeof $object.StartDateCalender == "undefined" || $object.StartDateCalender == null || $object.StartDateCalender == "-1" || $object.StartDateCalender == "") {
                        $object.StartDateCalender = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
                        isChosenStartDate = false;
                    }
                    else
                        isChosenStartDate = true;

                    if (typeof $object.EndDateCalender == "undefined" || $object.EndDateCalender == null || $object.EndDateCalender == "-1" || $object.EndDateCalender == "") {
                        $object.EndDateCalender = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                        isChosenStartDate = false;
                    }
                    else
                        isChosenEndDate = true;

                    if (radioOption == 0) {
                        $(this).parents(".smdd_custom_section").attr('selectedRadioOption', '0');
                        $($(this).parents(".smdd_top_section").siblings('.xdsoft_datetimepicker')[1]).find('.xdsoft_current').addClass('xdsoft_current_Custom');
                        $("#" + id).find("input[name = custom_" + id + "][value = From]").prop('checked', true);

                        $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").siblings(".smdd_middle_section_overlay").hide();
                        $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").siblings(".smdd_right_section_overlay").show();

                    }
                    else if (radioOption == 1) {
                        $(this).parents(".smdd_custom_section").attr('selectedRadioOption', '1');
                        $($(this).parents(".smdd_top_section").siblings('.xdsoft_datetimepicker')[1]).find('.xdsoft_current,.xdsoft_current_Custom').removeClass('xdsoft_current_Custom');
                        $("#" + id).find("input[name = custom_" + id + "][value = Between]").prop('checked', true);

                        $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").siblings(".smdd_middle_section_overlay").hide();
                        $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").siblings(".smdd_right_section_overlay").hide();

                    }

                    else if (radioOption == 2) {
                        $(this).parents(".smdd_custom_section").attr('selectedRadioOption', '2');
                        $($(this).parents(".smdd_top_section").siblings('.xdsoft_datetimepicker')[1]).find('.xdsoft_current').addClass('xdsoft_current_Custom');
                        $("#" + id).find("input[name = custom_" + id + "][value = Prior]").prop('checked', true);

                        $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").siblings(".smdd_middle_section_overlay").hide();
                        $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").siblings(".smdd_right_section_overlay").show();

                    }
                    else if (radioOption == 3) { //Outside
                        $(this).parents(".smdd_custom_section").attr('selectedRadioOption', '3');
                        $($(this).parents(".smdd_top_section").siblings('.xdsoft_datetimepicker')[1]).find('.xdsoft_current,.xdsoft_current_Custom').removeClass('xdsoft_current_Custom');
                        $("#" + id).find("input[name = custom_" + id + "][value = Outside]").prop('checked', true);
                        $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").siblings(".smdd_middle_section_overlay").hide();
                        $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").siblings(".smdd_right_section_overlay").hide();
                    }
                    else if (radioOption == 4) {  //On
                        $(this).parents(".smdd_custom_section").attr('selectedRadioOption', '4');
                        $($(this).parents(".smdd_top_section").siblings('.xdsoft_datetimepicker')[1]).find('.xdsoft_current,.xdsoft_current_Custom').removeClass('xdsoft_current_Custom');
                        $("#" + id).find("input[name = custom_" + id + "][value = On]").prop('checked', true);
                        $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").siblings(".smdd_middle_section_overlay").hide();
                        $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").siblings(".smdd_right_section_overlay").show();
                    }
                    if (radioOption == 0 || radioOption == 4) { //from and on

                        if (!isChosenStartDate) {
                            Ntimetext = 14;

                            var promise = getServerDate();
                            promise.success(function (data) {
                                var serverDate = data.d;
                                var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                                var StartDay = new Date(today.getFullYear(), today.getMonth(), (today.getDate() - Ntimetext));
                                var StartDay = new Date(StartDay.setHours(0, 0, 0, 0));
                                var today = new Date(today.setHours(23, 59, 59, 999));

                                //    $("#" + id).find('.smdd_right_section').datetimepicker({ value: today });
                                $("#" + id).find('.smdd_middle_section').datetimepicker({ value: StartDay });
                            });
                        }
                        else if (isChosenStartDate) {
                            var today = Date.parseInvariant($object.StartDateCalender, clientShortFormat);

                            if (today == null)
                                today = Date.parseInvariant($object.StartDateCalender, clientformat);

                            var StartDay = new Date(today.getFullYear(), today.getMonth(), today.getDate());
                            var StartDay = new Date(StartDay.setHours(today.getHours(), today.getMinutes(), today.getSeconds(), 0));
                            $("#" + id).find('.smdd_middle_section').datetimepicker({ value: StartDay });
                        }
                    }
                    else if (radioOption == 1 || radioOption == 3) {  //between and outside

                        if (!isChosenStartDate) {

                            Ntimetext = 14;

                            var promise = getServerDate();
                            promise.success(function (data) {
                                var serverDate = data.d;
                                var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                                var StartDay = new Date(today.getFullYear(), today.getMonth(), (today.getDate() - Ntimetext));
                                var StartDay = new Date(StartDay.setHours(0, 0, 0, 0));
                                var today = new Date(today.setHours(23, 59, 59, 999));
                                $("#" + id).find('.smdd_right_section').datetimepicker({ value: today });
                                $("#" + id).find('.smdd_middle_section').datetimepicker({ value: StartDay });

                            });
                        }
                        else if (isChosenStartDate) {
                            var today = Date.parseInvariant($object.StartDateCalender, clientShortFormat);
                            var end = Date.parseInvariant($object.EndDateCalender, clientShortFormat);

                            if (today == null)
                                today = Date.parseInvariant($object.StartDateCalender, clientformat);
                            if (end == null)
                                end = Date.parseInvariant($object.EndDateCalender, clientformat);

                            var StartDay = new Date(today.getFullYear(), today.getMonth(), today.getDate());
                            var StartDay = new Date(StartDay.setHours(today.getHours(), today.getMinutes(), today.getSeconds(), 0));

                            var EndDay = new Date(end.getFullYear(), end.getMonth(), end.getDate());
                            var EndDay = new Date(EndDay.setHours(end.getHours(), end.getMinutes(), end.getSeconds(), 0));


                            $("#" + id).find('.smdd_right_section').datetimepicker({ value: EndDay });
                            $("#" + id).find('.smdd_middle_section').datetimepicker({ value: StartDay });
                        }

                    }
                    else { //prior
                        if (!isChosenStartDate) {

                            var promise = getServerDate();
                            promise.success(function (data) {
                                var serverDate = data.d;
                                var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                                var today = new Date(today.setHours(23, 59, 59, 999));

                                $("#" + id).find('.smdd_middle_section').datetimepicker({ value: StartDay });
                            });
                        }
                        else if (isChosenStartDate) {
                            var end = Date.parseInvariant($object.EndDateCalender, clientShortFormat);

                            if (end == null)
                                end = Date.parseInvariant($object.EndDateCalender, clientformat);

                            var EndDay = new Date(end.getFullYear(), end.getMonth(), end.getDate());
                            var EndDay = new Date(EndDay.setHours(23, 59, 59, 999));

                            $("#" + id).find('.smdd_middle_section').datetimepicker({ value: EndDay });
                        }
                        //   $("#" + id).find('.smdd_right_section').datetimepicker({ value: today });
                    }
                    break;
                case 5: //Last 15 days
                    $('#lastNtimeid_' + counter).val('15');
                    smselect.setOptionByText($('#smselect_ddllastNtime_' + counter), 'Days');
                    var Ntimetext;
                    if ($object.selectedNtimeText == undefined || $object.selectedNtimeText == '') {
                        Ntimetext = 14;

                        var promise = getServerDate();
                        promise.success(function (data) {
                            var serverDate = data.d;
                            var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                            var StartDay = new Date(today.getFullYear(), today.getMonth(), (today.getDate() - Ntimetext));
                            var StartDay = new Date(StartDay.setHours(0, 0, 0, 0));
                            var today = new Date(today.setHours(23, 59, 59, 999));
                            $("#" + id).find('.smdd_right_section').datetimepicker({ value: today });
                            $("#" + id).find('.smdd_middle_section').datetimepicker({ value: StartDay });
                        });
                    }
                    else {
                        var obj = setCalender(0, id, counter, -($object.selectedNtimeText), $object.selectedNtimeOption)
                        $("#" + id).find('.smdd_right_section').datetimepicker({ value: obj.NEndDay });
                        $("#" + id).find('.smdd_middle_section').datetimepicker({ value: obj.NStartDay });
                    }

                    $(this).parents(".smdd_top_section").siblings(".smdd_nextNtime_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_lastNtime_section").css('visibility', 'visible');
                    $(this).parents(".smdd_top_section").siblings(".smdd_middle_section_overlay").show();
                    $(this).parents(".smdd_top_section").siblings(".smdd_right_section_overlay").show();
                    break;
                case 6:
                    //Next 15 days
                    $('#lastNtimeid_' + counter).val('15');
                    smselect.setOptionByText($('#smselect_ddlnextNtime_' + counter), 'Days');
                    var Ntimetext;
                    if ($object.selectedNtimeText == undefined || $object.selectedNtimeText == '') {
                        Ntimetext = 14;

                        var promise = getServerDate();
                        promise.success(function (data) {
                            var serverDate = data.d;
                            var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                            var EndDay = new Date(today.getFullYear(), today.getMonth(), (today.getDate() + Ntimetext));
                            var today = new Date(today.setHours(0, 0, 0, 0));
                            var EndDay = new Date(EndDay.setHours(23, 59, 59, 999));
                            $("#" + id).find('.smdd_middle_section').datetimepicker({ value: today });
                            $("#" + id).find('.smdd_right_section').datetimepicker({ value: EndDay });
                        });

                    }
                    else {
                        var obj = setCalender(1, id, counter, $object.selectedNtimeText, $object.selectedNtimeOption)
                        $("#" + id).find('.smdd_right_section').datetimepicker({ value: obj.NEndDay });
                        $("#" + id).find('.smdd_middle_section').datetimepicker({ value: obj.NStartDay });
                    }

                    $(this).parents(".smdd_top_section").siblings(".smdd_lastNtime_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_nextNtime_section").css('visibility', 'visible');
                    $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_middle_section_overlay").show();
                    $(this).parents(".smdd_top_section").siblings(".smdd_right_section_overlay").show();
                    break;

            }

            return false;
        });

        $("#" + id).find(".smdd_custom_section").find("input[name = custom_" + id + "]").unbind("click").bind("click", function (event) {
            var target = $(event.target);
            if ($("#" + id).find("input[name = custom_" + id + "][value = From]").prop('checked')) {

                if ($object.StartDateCalender == undefined || $object.StartDateCalender == "" || $object.StartDateCalender == null) {
                    Ntimetext = 14;

                    var promise = getServerDate();
                    promise.success(function (data) {
                        var serverDate = data.d;
                        var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                        var StartDay = new Date(today.getFullYear(), today.getMonth(), (today.getDate() - Ntimetext));
                        var StartDay = new Date(StartDay.setHours(0, 0, 0, 0));
                        var today = new Date(today.setHours(23, 59, 59, 999));
                        $("#" + id).find('.smdd_middle_section').datetimepicker({ value: StartDay });
                    });
                }
                else {
                    var startDateObject = Date.parseInvariant($object.StartDateCalender, clientShortFormat);
                    var endDateObject = Date.parseInvariant($object.EndDateCalender, clientShortFormat);

                    if (startDateObject == null)
                        startDateObject = Date.parseInvariant($object.StartDateCalender, clientformat);
                    if (endDateObject == null)
                        endDateObject = Date.parseInvariant($object.EndDateCalender, clientformat);

                    var StartDay = new Date(startDateObject.setHours(0, 0, 0, 0));
                    var today = new Date(endDateObject.setHours(23, 59, 59, 999));

                    $("#" + id).find('.smdd_middle_section').datetimepicker({ value: StartDay });
                }

                $(this).parents(".smdd_custom_section").attr('selectedRadioOption', '0');
                $(this).parents(".smdd_custom_section").siblings(".smdd_middle_section_overlay").hide();
                $(this).parents(".smdd_custom_section").siblings(".smdd_right_section_overlay").show();
                $($(this).parents(".smdd_custom_section").siblings('.xdsoft_datetimepicker')[1]).find('.xdsoft_current').addClass('xdsoft_current_Custom');

            }
            else if ($("#" + id).find("input[name = custom_" + id + "][value = Between]").prop('checked')) {

                if ($object.StartDateCalender == undefined || $object.StartDateCalender == "" || $object.StartDateCalender == null) {
                    Ntimetext = 14;

                    var promise = getServerDate();
                    promise.success(function (data) {
                        var serverDate = data.d;
                        var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                        var StartDay = new Date(today.getFullYear(), today.getMonth(), (today.getDate() - Ntimetext));
                        var StartDay = new Date(StartDay.setHours(0, 0, 0, 0));
                        var today = new Date(today.setHours(23, 59, 59, 999));
                        $("#" + id).find('.smdd_right_section').datetimepicker({ value: today });
                        $("#" + id).find('.smdd_middle_section').datetimepicker({ value: StartDay });
                    });
                }
                else {
                    var startDateObject = Date.parseInvariant($object.StartDateCalender, clientShortFormat);
                    var endDateObject = Date.parseInvariant($object.EndDateCalender, clientShortFormat);

                    if (startDateObject == null)
                        startDateObject = Date.parseInvariant($object.StartDateCalender, clientformat);
                    if (endDateObject == null)
                        endDateObject = Date.parseInvariant($object.EndDateCalender, clientformat);

                    var StartDay = new Date(startDateObject.setHours(0, 0, 0, 0));
                    var today = new Date(endDateObject.setHours(23, 59, 59, 999));

                    $("#" + id).find('.smdd_right_section').datetimepicker({ value: today });
                    $("#" + id).find('.smdd_middle_section').datetimepicker({ value: StartDay });
                }

                $(this).parents(".smdd_custom_section").attr('selectedRadioOption', '1');
                $(this).parents(".smdd_custom_section").siblings(".smdd_middle_section_overlay").hide();
                $(this).parents(".smdd_custom_section").siblings(".smdd_right_section_overlay").hide();

            }
            else if ($("#" + id).find("input[name = custom_" + id + "][value = Prior]").prop('checked')) {
                if ($object.EndDateCalender == undefined || $object.EndDateCalender == "" || $object.EndDateCalender == null) {

                    Ntimetext = 14;

                    var promise = getServerDate();
                    promise.success(function (data) {
                        var serverDate = data.d;
                        var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                        var today = new Date(today.setHours(23, 59, 59, 999));
                        $("#" + id).find('.smdd_middle_section').datetimepicker({ value: today });
                    });
                }
                else {
                    var endDateObject = Date.parseInvariant($object.EndDateCalender, clientShortFormat);

                    if (endDateObject == null)
                        endDateObject = Date.parseInvariant($object.EndDateCalender, clientformat);

                    var today = new Date(endDateObject.setHours(23, 59, 59, 999));

                    $("#" + id).find('.smdd_middle_section').datetimepicker({ value: today });
                }
                //      $("#" + id).find('.smdd_right_section').datetimepicker({ value: today });


                $(this).parents(".smdd_custom_section").attr('selectedRadioOption', '2');
                $(this).parents(".smdd_custom_section").siblings(".smdd_middle_section_overlay").hide();
                $(this).parents(".smdd_custom_section").siblings(".smdd_right_section_overlay").show();
                $($(this).parents(".smdd_custom_section").siblings('.xdsoft_datetimepicker')[1]).find('.xdsoft_current').addClass('xdsoft_current_Custom');


            }
            else if ($("#" + id).find("input[name = custom_" + id + "][value = Outside]").prop('checked')) {

                if ($object.StartDateCalender == undefined || $object.StartDateCalender == "" || $object.StartDateCalender == null) {
                    Ntimetext = 14;

                    var promise = getServerDate();
                    promise.success(function (data) {
                        var serverDate = data.d;
                        var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                        var StartDay = new Date(today.getFullYear(), today.getMonth(), (today.getDate() - Ntimetext));
                        var StartDay = new Date(StartDay.setHours(0, 0, 0, 0));
                        var today = new Date(today.setHours(23, 59, 59, 999));
                        $("#" + id).find('.smdd_right_section').datetimepicker({ value: today });
                        $("#" + id).find('.smdd_middle_section').datetimepicker({ value: StartDay });
                    });
                }
                else {
                    var startDateObject = Date.parseInvariant($object.StartDateCalender, clientShortFormat);
                    var endDateObject = Date.parseInvariant($object.EndDateCalender, clientShortFormat);

                    if (startDateObject == null)
                        startDateObject = Date.parseInvariant($object.StartDateCalender, clientformat);
                    if (endDateObject == null)
                        endDateObject = Date.parseInvariant($object.EndDateCalender, clientformat);

                    var StartDay = new Date(startDateObject.setHours(0, 0, 0, 0));
                    var today = new Date(endDateObject.setHours(23, 59, 59, 999));

                    $("#" + id).find('.smdd_right_section').datetimepicker({ value: today });
                    $("#" + id).find('.smdd_middle_section').datetimepicker({ value: StartDay });
                }

                $(this).parents(".smdd_custom_section").attr('selectedRadioOption', '3');
                $(this).parents(".smdd_custom_section").siblings(".smdd_middle_section_overlay").hide();
                $(this).parents(".smdd_custom_section").siblings(".smdd_right_section_overlay").hide();

            }
            else if ($("#" + id).find("input[name = custom_" + id + "][value = On]").prop('checked')) {

                if ($object.StartDateCalender == undefined || $object.StartDateCalender == "" || $object.StartDateCalender == null) {
                    Ntimetext = 14;

                    var promise = getServerDate();
                    promise.success(function (data) {
                        var serverDate = data.d;
                        var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                        var StartDay = new Date(today.getFullYear(), today.getMonth(), (today.getDate() - Ntimetext));
                        var StartDay = new Date(StartDay.setHours(0, 0, 0, 0));
                        var today = new Date(today.setHours(23, 59, 59, 999));
                        $("#" + id).find('.smdd_middle_section').datetimepicker({ value: StartDay });
                    });
                }
                else {
                    var startDateObject = Date.parseInvariant($object.StartDateCalender, clientShortFormat);
                    var endDateObject = Date.parseInvariant($object.EndDateCalender, clientShortFormat);

                    if (startDateObject == null)
                        startDateObject = Date.parseInvariant($object.StartDateCalender, clientformat);
                    if (endDateObject == null)
                        endDateObject = Date.parseInvariant($object.EndDateCalender, clientformat);

                    var StartDay = new Date(startDateObject.setHours(0, 0, 0, 0));
                    var today = new Date(endDateObject.setHours(23, 59, 59, 999));
                    $("#" + id).find('.smdd_middle_section').datetimepicker({ value: StartDay });
                }
                //   $("#" + id).find('.smdd_right_section').datetimepicker({ value: today });


                $(this).parents(".smdd_custom_section").attr('selectedRadioOption', '4');
                $(this).parents(".smdd_custom_section").siblings(".smdd_middle_section_overlay").hide();
                $(this).parents(".smdd_custom_section").siblings(".smdd_right_section_overlay").show();
                $($(this).parents(".smdd_custom_section").siblings('.xdsoft_datetimepicker')[1]).find('.xdsoft_current').addClass('xdsoft_current_Custom');

            }
            event.stopPropagation();

        });

        $("#" + id).find(".smdd_lastNtime_section").find("#lastNtimeid_" + counter).unbind("click").bind("click", function (event) {
            event.stopPropagation();
        });
        $("#" + id).find(".smdd_nextNtime_section").find("#nextNtimeid_" + counter).unbind("click").bind("click", function (event) {
            event.stopPropagation();
        });

        //lastNtimeText - 15 ; lastNtimeValue- days / month
        $('#smdd_' + counter).unbind('click').click(function (e) {
            if ($("#" + id).children(".smdd_lastNtime_section").css('visibility') == 'visible') {
                var lastNtimeText = $("#" + id).children(".smdd_lastNtime_section").find('#lastNtimeid_' + counter).val()
                var lastNtimeValue = $("#" + id).children(".smdd_lastNtime_section").find('#ddllastNtime_' + counter)[0].value;
                var retobj = setCalender(0, $("#" + id), counter, -(lastNtimeText), lastNtimeValue);
                if ($("#" + id).find('.smdd_right_section').length != 0) {
                    $("#" + id).find('.smdd_right_section').datetimepicker({ value: retobj.NEndDay });
                    $("#" + id).find('.smdd_middle_section').datetimepicker({ value: retobj.NStartDay });
                }
                else {
                    $("#" + id).attr("selectedStartDate", retobj.NStartDay);
                    $("#" + id).attr("selectedEndDate", retobj.NEndDay);
                }

            }
            else if ($("#" + id).children(".smdd_nextNtime_section").css('visibility') == 'visible') {
                var nextNtimeText = $("#" + id).children(".smdd_nextNtime_section").find('#nextNtimeid_' + counter).val();
                var nextNtimeValue = $("#" + id).children(".smdd_nextNtime_section").find('#ddlnextNtime_' + counter)[0].value;
                var retobj = setCalender(1, $("#" + id), counter, nextNtimeText, nextNtimeValue);
                if ($("#" + id).find('.smdd_middle_section').length != 0) {
                    $("#" + id).find('.smdd_middle_section').datetimepicker({ value: retobj.NStartDay });
                    $("#" + id).find('.smdd_right_section').datetimepicker({ value: retobj.NEndDay });
                }
                else {
                    $("#" + id).attr("selectedStartDate", retobj.NStartDay);
                    $("#" + id).attr("selectedEndDate", retobj.NEndDay);
                }
            }
            e.stopPropagation();
        });
        $('#smselect_ddllastNtime_' + counter + ', ' + '#smselect_ddlnextNtime_' + counter).unbind('change').change(function (e) {
            if ($("#" + id).children(".smdd_lastNtime_section").css('visibility') == 'visible') {
                var lastNtimeText = $("#" + id).children(".smdd_lastNtime_section").find('#lastNtimeid_' + counter).val();
                var lastNtimeValue = $("#" + id).children(".smdd_lastNtime_section").find('#ddllastNtime_' + counter)[0].value;
                var retobj = setCalender(0, $("#" + id), counter, -(lastNtimeText), lastNtimeValue);
                if ($("#" + id).find('.smdd_right_section').length != 0) {
                    $("#" + id).find('.smdd_right_section').datetimepicker({ value: retobj.NEndDay });
                    $("#" + id).find('.smdd_middle_section').datetimepicker({ value: retobj.NStartDay });
                }
                else {
                    $("#" + id).attr("selectedStartDate", retobj.NStartDay);
                    $("#" + id).attr("selectedEndDate", retobj.NEndDay);
                }
            }
            else {
                var nextNtimeText = $("#" + id).children(".smdd_nextNtime_section").find('#nextNtimeid_' + counter).val();
                var nextNtimeValue = $("#" + id).children(".smdd_nextNtime_section").find('#ddlnextNtime_' + counter)[0].value;
                var retobj = setCalender(1, $("#" + id), counter, nextNtimeText, nextNtimeValue);
                if ($("#" + id).find('.smdd_middle_section').length != 0) {
                    $("#" + id).find('.smdd_middle_section').datetimepicker({ value: retobj.NStartDay });
                    $("#" + id).find('.smdd_right_section').datetimepicker({ value: retobj.NEndDay });
                }
                else {
                    $("#" + id).attr("selectedStartDate", retobj.NStartDay);
                    $("#" + id).attr("selectedEndDate", retobj.NEndDay);
                }
            }


        });

    }
    function attachEventHandlersforSingleCalender($object, id) {
        var counter = id.split('_')[1];
        //Click handler for the pivot element
        $object.pivotElement.unbind("click").bind("click", function (e) {
            $(".smdd_container").each(function (index, element) {
                if ($(element).attr("id") !== id) {
                    $(element).hide();
                }
                else {
                    $object.pivotElement = $(e.target);
                    var windowHeight = $(window).height();
                    var windowWidth = $(window).width();
                    var top = "";
                    var left = "";
                    if (($object.pivotElement.offset().top + $("#" + id).height()) > windowHeight) {
                        top = $object.pivotElement.offset().top - ($("#" + id).height() + 2);
                    }
                    else {
                        top = $object.pivotElement.offset().top + 22;
                    }

                    if (($object.pivotElement.offset().left + $("#" + id).width()) > windowWidth) {
                        left = $object.pivotElement.offset().left - ($("#" + id).width() - $object.pivotElement.width());
                    }
                    else {
                        left = $object.pivotElement.offset().left - 30;
                    }

                    $("#" + id).css("left", left + "px");
                    $("#" + id).css("top", top + "px");

                    $("#" + id).find(".smdd_middle_section_overlay").css('top', '70px');
                    if (!$object.lefttimePicker) {
                        $("#" + id).find(".smdd_middle_section_overlay").css('left', '30px');
                        $("#" + id).find(".smdd_section_overlay").width('250px');

                    }
                    else {
                        $("#" + id).find(".smdd_middle_section_overlay").css('left', '30px');
                        $("#" + id).find(".smdd_section_overlay").width('325px');


                    }





                    //                    $("#" + id).find(".smdd_middle_section_overlay").css('top', '70px');
                    //                    $("#" + id).find(".smdd_middle_section_overlay").css('left', '15px');

                    $("#" + id).toggle();
                }

                e.stopPropagation();
            })
        });

        if ($object.hasOwnProperty("applyCallback") && typeof $object.applyCallback === "function") {
            //$(document).bind("click", $object.applyCallback);
            $(document).unbind("click", docHandler).bind("click", docHandler);
        }

        $("#" + id).find(".smdd_top_section").find("button").unbind("click").bind("click", function (event) {
            var target = $(event.target);
            target.addClass("dateSelected");
            var counter = $("#" + id)[0].id.split('_')[1];
            $("#" + id).attr("selectedDate", target.attr("option"))
            var choosenOption = parseInt(target.attr("option"));

            //To change the background-color of all the other Btns
            $(this).siblings("button").css("background-color", "white");
            $(this).siblings("button").css("color", "#4197ce");
            //To change the background-color of the selected Btn
            $(this).css("background-color", "#4197ce");
            $(this).css("color", "white");

            switch (choosenOption) {
                case 0: //Today


                    var promise = getServerDate();
                    promise.success(function (data) {
                        var serverDate = data.d;
                        var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                        var today = new Date(today.setHours(0, 0, 0, 0));
                        var endofToday = new Date(new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt")).setHours(23, 59, 59, 999));
                        $("#" + id).find('.smdd_middle_section').datetimepicker({ value: today });


                    });
                    $(this).parents(".smdd_top_section").siblings(".smdd_middle_section_overlay").show();

                    break;
                case 1: //Since yesterday

                    var promise = getServerDate();
                    promise.success(function (data) {
                        var serverDate = data.d;
                        var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                        var yesterday = new Date(today);
                        yesterday.setDate(today.getDate() - 1);
                        var yesterday = new Date(yesterday.setHours(0, 0, 0, 0));
                        var endofToday = new Date(new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt")).setHours(23, 59, 59, 999));
                        $("#" + id).find('.smdd_middle_section').datetimepicker({ value: yesterday });

                    });
                    $(this).parents(".smdd_top_section").siblings(".smdd_middle_section_overlay").show();

                    break;

                case 4: //custom date
                    $(this).parents(".smdd_top_section").siblings(".smdd_middle_section_overlay").hide();

                    var startDateObject = Date.parseInvariant($object.StartDateCalender, clientShortFormat);
                    var endDateObject = Date.parseInvariant($object.EndDateCalender, clientShortFormat);

                    if (startDateObject == null)
                        startDateObject = Date.parseInvariant($object.StartDateCalender, clientformat);
                    if (endDateObject == null)
                        endDateObject = Date.parseInvariant($object.EndDateCalender, clientformat);

                    var StartDay = new Date(startDateObject.setHours(0, 0, 0, 0));
                    var today = new Date(endDateObject.setHours(23, 59, 59, 999));

                    $("#" + id).find('.smdd_middle_section').datetimepicker({ value: StartDay });
                    break;


            }

            return false;
        });
    }

    function attachEventHandlersForNoCalender($object, id) {
        var counter = id.split('_')[1];
        //Click handler for the pivot element
        $object.pivotElement.unbind("click").bind("click", function (e) {
            $(".smdd_container").each(function (index, element) {
                if ($(element).attr("id") !== id) {
                    $(element).hide();
                }
                else {
                    var windowHeight = $(window).height();
                    var windowWidth = $(window).width();
                    var top = "";
                    var left = "";
                    if (($object.pivotElement.offset().top + $("#" + id).height()) > windowHeight) {
                        top = $object.pivotElement.offset().top - ($("#" + id).height() + 2);
                    }
                    else {
                        top = $object.pivotElement.offset().top + 22;
                    }

                    if (($object.pivotElement.offset().left + $("#" + id).width()) > windowWidth) {
                        left = $object.pivotElement.offset().left - ($("#" + id).width() - $object.pivotElement.width());
                    }
                    else {
                        left = $object.pivotElement.offset().left - 30;
                    }

                    $("#" + id).css("left", left + "px");
                    $("#" + id).css("top", top + "px");
                    $("#" + id).toggle();
                }

                e.stopPropagation();
            })
        });

        if ($object.hasOwnProperty("applyCallback") && typeof $object.applyCallback === "function") {
            //            $object.applyCallback();
            $(document).unbind("click", docHandler).bind("click", docHandler);
        }
        $(document).unbind("click", docHandler).bind("click", docHandler);
        $("#" + id).find(".smdd_top_section").find("button").unbind("click").bind("click", function (event) {
            var target = $(event.target);
            target.addClass("dateSelected");
            var counter = $("#" + id)[0].id.split('_')[1];
            $("#" + id).attr("selectedDate", target.attr("option"))
            var choosenOption = parseInt(target.attr("option"));
            $('#' + id + ' .smselectcon').hide();
            //To change the background-color of all the other Btns
            $(this).siblings("button").css("background-color", "white");
            $(this).siblings("button").css("color", "#4197ce");
            //To change the background-color of the selected Btn
            $(this).css("background-color", "#4197ce");
            $(this).css("color", "white");

            switch (choosenOption) {
                case 0: //Today
                    $(this).parents(".smdd_top_section").siblings(".smdd_lastNtime_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_nextNtime_section").css('visibility', 'hidden');

                    var promise = getServerDate();
                    promise.success(function (data) {
                        var serverDate = data.d;
                        var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                        $("#" + id).attr("selectedStartDate", today);
                        $("#" + id).attr("selectedEndDate", today);
                    });

                    $(this).parents(".smdd_top_section").siblings(".smdd_middle_section_overlay").show();
                    $(this).parents(".smdd_top_section").siblings(".smdd_right_section_overlay").show();
                    break;
                case 1: //Since yesterday
                    $(this).parents(".smdd_top_section").siblings(".smdd_lastNtime_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_nextNtime_section").css('visibility', 'hidden');

                    var promise = getServerDate();
                    promise.success(function (data) {
                        var serverDate = data.d;
                        var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                        var yesterday = new Date(today);
                        yesterday.setDate(today.getDate() - 1);
                        $("#" + id).attr("selectedStartDate", yesterday);
                        $("#" + id).attr("selectedEndDate", today);
                    });
                    $(this).parents(".smdd_top_section").siblings(".smdd_middle_section_overlay").show();
                    $(this).parents(".smdd_top_section").siblings(".smdd_right_section_overlay").show();
                    break;
                case 2: //This week
                    $(this).parents(".smdd_top_section").siblings(".smdd_lastNtime_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_nextNtime_section").css('visibility', 'hidden');

                    var promise = getServerDate();
                    promise.success(function (data) {
                        var serverDate = data.d;
                        var current = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                        var weekstart = current.getDate() - current.getDay() + 1;
                        var monday = new Date(current.setDate(weekstart));
                        var monday = new Date(monday.setHours(0, 0, 0, 0));
                        $("#" + id).attr("selectedStartDate", today);
                        $("#" + id).attr("selectedEndDate", monday);
                    });
                    $(this).parents(".smdd_top_section").siblings(".smdd_middle_section_overlay").show();
                    $(this).parents(".smdd_top_section").siblings(".smdd_right_section_overlay").show();
                    break;
                case 3: //Any
                    $(this).parents(".smdd_top_section").siblings(".smdd_lastNtime_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_nextNtime_section").css('visibility', 'hidden');


                    var promise = getServerDate();
                    promise.success(function (data) {
                        var serverDate = data.d;
                        var today = new Date(new Date(serverDate).format("yyyy-MM-dd hh:mm:ss tt"));
                        $("#" + id).attr("selectedEndDate", today);
                    });

                    $(this).parents(".smdd_top_section").siblings(".smdd_middle_section_overlay").show();
                    $(this).parents(".smdd_top_section").siblings(".smdd_right_section_overlay").show();
                    break;
                case 4: //custom date
                    $(this).parents(".smdd_top_section").siblings(".smdd_nextNtime_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_lastNtime_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").css('visibility', 'visible');
                    var radioOption = $("#" + id).find(".smdd_custom_section").attr('selectedRadioOption');
                    if (radioOption == 0) {
                        $(this).parents(".smdd_custom_section").attr('selectedRadioOption', '0');
                        $("#" + id).find("input[name = custom_" + id + "][value = From]").prop('checked', true);
                    }
                    else if (radioOption == 1) {
                        $(this).parents(".smdd_custom_section").attr('selectedRadioOption', '1');
                        $("#" + id).find("input[name = custom_" + id + "][value = Between]").prop('checked', true);
                    }
                    else {
                        $(this).parents(".smdd_custom_section").attr('selectedRadioOption', '2');
                        $("#" + id).find("input[name = custom_" + id + "][value = Prior]").prop('checked', true);

                    }

                    break;
                case 5: //Last 15 days
                    $('#lastNtimeid_' + counter).val('15');
                    $(this).parents(".smdd_top_section").siblings(".smdd_nextNtime_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_lastNtime_section").css('visibility', 'visible');
                    var lastNtimeText = $("#" + id).children(".smdd_lastNtime_section").find('#lastNtimeid_' + counter).val();
                    var lastNtimeValue = $("#" + id).children(".smdd_lastNtime_section").find('#ddllastNtime_' + counter)[0].value;
                    var retobj = setCalender(0, $("#" + id), counter, -(lastNtimeText), lastNtimeValue);
                    $("#" + id).attr("selectedStartDate", retobj.NStartDay);
                    $("#" + id).attr("selectedEndDate", retobj.NEndDay);

                    break;
                case 6:
                    //Next 15 days
                    $('#nextNtimeid_' + counter).val('15');

                    $(this).parents(".smdd_top_section").siblings(".smdd_lastNtime_section").css('visibility', 'hidden');
                    $(this).parents(".smdd_top_section").siblings(".smdd_nextNtime_section").css('visibility', 'visible');
                    $(this).parents(".smdd_top_section").siblings(".smdd_custom_section").css('visibility', 'hidden');
                    var nextNtimeText = $("#" + id).children(".smdd_nextNtime_section").find('#nextNtimeid_' + counter).val();
                    var nextNtimeValue = $("#" + id).children(".smdd_nextNtime_section").find('#ddlnextNtime_' + counter)[0].value;
                    var retobj = setCalender(1, $("#" + id), counter, nextNtimeText, nextNtimeValue);

                    $("#" + id).attr("selectedStartDate", retobj.NStartDay);
                    $("#" + id).attr("selectedEndDate", retobj.NEndDay);

                    break;

            }

            return false;
        });

        $("#" + id).find(".smdd_custom_section").find("input[name = custom_" + id + "]").unbind("click").bind("click", function (event) {
            var target = $(event.target);
            if ($("#" + id).find("input[name = custom_" + id + "][value = From]").prop('checked')) {
                $(this).parents(".smdd_custom_section").attr('selectedRadioOption', '0');
            }
            else if ($("#" + id).find("input[name = custom_" + id + "][value = Between]").prop('checked')) {
                $(this).parents(".smdd_custom_section").attr('selectedRadioOption', '1');
            }
            else {
                $(this).parents(".smdd_custom_section").attr('selectedRadioOption', '2');
            }
            event.stopPropagation();

        });

        $("#" + id).find(".smdd_lastNtime_section").find("#lastNtimeid_" + counter).unbind("click").bind("click", function (event) {
            event.stopPropagation();
        });
        $("#" + id).find(".smdd_nextNtime_section").find("#nextNtimeid_" + counter).unbind("click").bind("click", function (event) {
            event.stopPropagation();
        });

        //lastNtimeText - 15 ; lastNtimeValue- days / month
        $('#smdd_' + counter).unbind('click').click(function (e) {
            var selectsmdd = $('.smdd_container');
            var freqOption, customOption;
            var ddlMonQuarSelectedIndex = '', ddlYearSelectedIndex, ddlYear2SelectedIndex, ddlMonQuar2SelectedIndex = '', textddlMonQuarSelected, textddlMonQuar2Selected;
            $.each(selectsmdd, function (ii, ee) {
                if ($(ee).css('display') == 'block') {
                    freqOption = smdatepickercontrol.obj[$(ee)[0].id.split('_')[1]].LstNxtFreqOptions;
                }
            });
            if ($("#" + id).children(".smdd_lastNtime_section").css('visibility') == 'visible') {
                $("#" + id).attr("DisplayText", null);
                var lastNtimeText = $("#" + id).children(".smdd_lastNtime_section").find('#lastNtimeid_' + counter).val();
                var lastNtimeValue = $("#" + id).children(".smdd_lastNtime_section").find('#ddllastNtime_' + counter)[0].value;
                var retobj = setCalender(0, $("#" + id), counter, -(lastNtimeText), lastNtimeValue);
                if ($("#" + id).find('.smdd_right_section').length != 0) {
                    $("#" + id).find('.smdd_right_section').datetimepicker({ value: retobj.NEndDay });
                    $("#" + id).find('.smdd_middle_section').datetimepicker({ value: retobj.NStartDay });
                }
                else {
                    $("#" + id).attr("selectedStartDate", retobj.NStartDay);
                    $("#" + id).attr("selectedEndDate", retobj.NEndDay);
                }

            }
            else if ($("#" + id).children(".smdd_nextNtime_section").css('visibility') == 'visible') {
                $("#" + id).attr("DisplayText", null);
                var nextNtimeText = $("#" + id).children(".smdd_nextNtime_section").find('#nextNtimeid_' + counter).val();
                var nextNtimeValue = $("#" + id).children(".smdd_nextNtime_section").find('#ddlnextNtime_' + counter)[0].value;
                var retobj = setCalender(1, $("#" + id), counter, nextNtimeText, nextNtimeValue);
                if ($("#" + id).find('.smdd_middle_section').length != 0) {
                    $("#" + id).find('.smdd_middle_section').datetimepicker({ value: retobj.NStartDay });
                    $("#" + id).find('.smdd_right_section').datetimepicker({ value: retobj.NEndDay });
                }
                else {
                    $("#" + id).attr("selectedStartDate", retobj.NStartDay);
                    $("#" + id).attr("selectedEndDate", retobj.NEndDay);
                }
            }
            else if ($("#" + id).children(".smdd_custom_section").css('visibility') == 'visible') {

                if ($("#" + id).children(".smdd_custom_section").attr('selectedRadioOption') == '0') { //From
                    //Left Parameter
                    customOption = 0;
                    //month
                    if (freqOption != 5) { ddlMonQuarSelectedIndex = smselect.getSelectedOption($("#smselect_fromSelect1ddl_" + id))[0].value; textddlMonQuarSelected = smselect.getSelectedOption($("#smselect_fromSelect1ddl_" + id))[0].text; }
                    //year
                    ddlYearSelectedIndex = smselect.getSelectedOption($("#smselect_fromTimeSelect1ddl_" + id))[0].value;
                    $("#" + id).attr("DisplayText", "after " + textddlMonQuarSelected + " " + ddlYearSelectedIndex);
                }
                else if ($("#" + id).children(".smdd_custom_section").attr('selectedRadioOption') == '1') { //Between
                    //Left Parameter
                    customOption = 1;
                    //month
                    if (freqOption != 5) { ddlMonQuarSelectedIndex = smselect.getSelectedOption($("#smselect_btwnSelect1ddl_" + id))[0].value; textddlMonQuarSelected = smselect.getSelectedOption($("#smselect_btwnSelect1ddl_" + id))[0].text; }
                    //year
                    ddlYearSelectedIndex = smselect.getSelectedOption($("#smselect_btwnTimeSelect1ddl_" + id))[0].value;
                    //Right Parameter
                    //month
                    if (freqOption != 5) { ddlMonQuar2SelectedIndex = smselect.getSelectedOption($("#smselect_btwnSelect2ddl_" + id))[0].value; textddlMonQuar2Selected = smselect.getSelectedOption($("#smselect_btwnSelect2ddl_" + id))[0].text; }
                    //year
                    ddlYear2SelectedIndex = smselect.getSelectedOption($("#smselect_btwnTimeSelect2ddl_" + id))[0].value;
                    $("#" + id).attr("DisplayText", "Between " + textddlMonQuarSelected + " " + ddlYearSelectedIndex + " to " + textddlMonQuar2Selected + " " + ddlYear2SelectedIndex);
                }
                else if ($("#" + id).children(".smdd_custom_section").attr('selectedRadioOption') == '2') { //Prior
                    //Left Parameter
                    customOption = 2;
                    //month
                    if (freqOption != 5) { ddlMonQuarSelectedIndex = smselect.getSelectedOption($("#smselect_priorSelect1ddl_" + id))[0].value; textddlMonQuarSelected = smselect.getSelectedOption($("#smselect_fromSelect1ddl_" + id))[0].text; }
                    //year
                    ddlYearSelectedIndex = smselect.getSelectedOption($("#smselect_priorTimeSelect1ddl_" + id))[0].value;
                    $("#" + id).attr("DisplayText", "before " + textddlMonQuarSelected + " " + ddlYearSelectedIndex);
                }


                var retobj = setDateFunc(freqOption, customOption, ddlYearSelectedIndex, ddlMonQuarSelectedIndex, ddlYear2SelectedIndex, ddlMonQuar2SelectedIndex);
                $("#" + id).attr("selectedStartDate", retobj.StartDay);
                $("#" + id).attr("selectedEndDate", retobj.EndDay);
            }
            e.stopPropagation();
        });
        $('#smselect_ddllastNtime_' + counter + ', ' + '#smselect_ddlnextNtime_' + counter).unbind('change').change(function (e) {
            if ($("#" + id).children(".smdd_lastNtime_section").css('display') == 'block') {
                var lastNtimeText = $("#" + id).children(".smdd_lastNtime_section").find('#lastNtimeid_' + counter).val();
                var lastNtimeValue = $("#" + id).children(".smdd_lastNtime_section").find('#ddllastNtime_' + counter)[0].value;
                var retobj = setCalender(0, $("#" + id), counter, -(lastNtimeText), lastNtimeValue);
                if ($("#" + id).find('.smdd_right_section').length != 0) {
                    $("#" + id).find('.smdd_right_section').datetimepicker({ value: retobj.NEndDay });
                    $("#" + id).find('.smdd_middle_section').datetimepicker({ value: retobj.NStartDay });
                }
                else {
                    $("#" + id).attr("selectedStartDate", retobj.NStartDay);
                    $("#" + id).attr("selectedEndDate", retobj.NEndDay);
                }
            }
            else {
                var nextNtimeText = $("#" + id).children(".smdd_nextNtime_section").find('#nextNtimeid_' + counter).val();
                var nextNtimeValue = $("#" + id).children(".smdd_nextNtime_section").find('#ddlnextNtime_' + counter)[0].value;
                var retobj = setCalender(1, $("#" + id), counter, nextNtimeText, nextNtimeValue);
                if ($("#" + id).find('.smdd_middle_section').length != 0) {
                    $("#" + id).find('.smdd_middle_section').datetimepicker({ value: retobj.NStartDay });
                    $("#" + id).find('.smdd_right_section').datetimepicker({ value: retobj.NEndDay });
                }
                else {
                    $("#" + id).attr("selectedStartDate", retobj.NStartDay);
                    $("#" + id).attr("selectedEndDate", retobj.NEndDay);
                }
            }


        });

    }
    return smdatepickercontrol;

})();

