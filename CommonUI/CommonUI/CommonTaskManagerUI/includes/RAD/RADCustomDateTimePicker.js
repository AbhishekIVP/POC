$(function () {
    $.widget("RAD.customDateTimePicker", {
        options: {
            format: '',
            type: 'DateTime', //DateOnly,TimeOnly,DateRange
            serviceOptions: false,
            CalendarID: '', //optional
            step: 60,
            onSelectDate: function () { },
            onSelectTime: function () { },
            onChangeMonth: function () { },
            onChangeYear: function () { },
            onChangeDateTime: function () { },
            onSelectRightDate: function () { },
            onSelectRightTime: function () { },
            onChangeRightMonth: function () { },
            onChangeRightYear: function () { },
            onChangeRightDateTime: function () { },
            onShow: function () { },
            onClose: function () { },
            onGenerate: function () { },
            onRightGenerate: function () { },
            zIndexStyle: '',
            //onRightShow: function () { },
            // onRightClose: function () { },
            globalVars: {
                idOfControl: '',
                flagCreated: false,
                rqdFormat: '',
                commonFormat: 'd.m.Y',
                customOptions: '',
                MainDOMElement: '',
                LeftCalendarOptions: {},
                RightCalendarOptions: {},
                url: '', //http://localhost/DatePickerService.svc',
                selectedOne: '',
                previousN: 0,
                caseChoiceToggle: false,
                previousDate: '',
                previousStartDate: '',
                previousEndDate: '',
                SpinInterval: setTimeout(function () { }, 500)
            }
        },
        _create: function () {
            CalendarOptions = {};

            //setting the default format
            if (this.options.format == '') {
                switch (this.options.type) {
                    case 'DateTime':
                        this.options.format = 'm-d-y H:i';
                        break;
                    case 'DateOnly':
                    case 'DateRange':
                        this.options.format = 'm-d-y';
                        break;
                    case 'TimeOnly':
                        this.options.format = 'H:i';
                        break;

                }
            }
            else {
                switch (this.options.type) {
                    case 'DateTime':
                        break;
                    case 'DateOnly':
                    case 'DateRange':
                        this.options.globalVars.LeftCalendarOptions.timepicker = false;
                        this.options.globalVars.RightCalendarOptions.timepicker = false;
                        break;
                    case 'TimeOnly':
                        this.options.globalVars.LeftCalendarOptions.datepicker = false;
                        this.options.globalVars.RightCalendarOptions.datepicker = false;
                        break;

                }
            }

            if (this.options.CalendarID == '')
                this.options.serviceOptions = false;
            else
                this.options.serviceOptions = true;

            var date = new Date();
            this.options.globalVars.previousStartDate = date.dateFormat(this.options.globalVars.commonFormat);
            this.options.globalVars.previousEndDate = date.dateFormat(this.options.globalVars.commonFormat);
            //this.options.globalVars.type = this.options.type;
            this.options.globalVars.rqdFormat = this.options.format;
            this.options.globalVars.customOptions = this.options;

            //setting attributes
            this.options.globalVars.RightCalendarOptions.format = this.options.globalVars.rqdFormat;
            this.options.globalVars.RightCalendarOptions.inline = true;
            this.options.globalVars.RightCalendarOptions.opened = true;
            this.options.globalVars.RightCalendarOptions.step = this.options.step;
            this.options.globalVars.RightCalendarOptions.className = "RightCalendarActualClass";
            //this.options.globalVars.RightCalendarOptions.onChangeDate = this.options.onChangeRightDate;
            //this.options.globalVars.RightCalendarOptions.onChangeTime = this.options.onChangeRightTime;
            this.options.globalVars.RightCalendarOptions.onChangeMonth = this.options.onChangeRightMonth;
            this.options.globalVars.RightCalendarOptions.onChangeYear = this.options.onChangeRightYear;
            this.options.globalVars.RightCalendarOptions.onChangeDateTime = this.options.onChangeRightDateTime;
            //this.options.globalVars.RightCalendarOptions.onShow = this.options.onRightShow;
            //this.options.globalVars.RightCalendarOptions.onClose = this.options.onRightClose;

            this.options.globalVars.RightCalendarOptions.onGenerate = function (ct) {
                jQuery(this).find('.xdsoft_current').addClass('xdsoft_endDate');
                //this.options.onRightGenerate();
            };

            if (this.options.type == 'TimeOnly') {
                this.options.globalVars.LeftCalendarOptions.onGenerate = function (ct) {
                    jQuery(this).find('.xdsoft_time_box').addClass('xdsoft_customTimeBox');
                    jQuery(this).find('.xdsoft_prev').addClass('xdsoft_custom_prev');
                    jQuery(this).find('.xdsoft_next').addClass('xdsoft_custom_next');
                    //this.options.onGenerate();
                }
            }
            else {
                this.options.globalVars.LeftCalendarOptions.onGenerate = function (ct) {
                    //  this.options.onGenerate();
                }
            }

            this.options.globalVars.LeftCalendarOptions.format = this.options.globalVars.rqdFormat;
            this.options.globalVars.LeftCalendarOptions.inline = true;
            this.options.globalVars.LeftCalendarOptions.opened = true;
            this.options.globalVars.LeftCalendarOptions.step = this.options.step;
            this.options.globalVars.LeftCalendarOptions.className = "LeftCalendarActualClass";
            //this.options.globalVars.LeftCalendarOptions.onChangeDate = this.options.onChangeRightDate;
            //this.options.globalVars.LeftCalendarOptions.onChangeTime = this.options.onChangeRightTime;
            this.options.globalVars.LeftCalendarOptions.onChangeMonth = this.options.onChangeMonth;
            this.options.globalVars.LeftCalendarOptions.onChangeYear = this.options.onChangeYear;
            this.options.globalVars.LeftCalendarOptions.onChangeDateTime = this.options.onChangeDateTime;
            //this.options.globalVars.LeftCalendarOptions.onShow = this.options.onShow;
            //this.options.globalVars.LeftCalendarOptions.onClose = this.options.onClose;

            this.options.globalVars.MainDOMElement = (this.element)[0];
            /*this._on(this.element, {
            //click: createOptions
            });*/

            $(this.element).bind("click", function () {
                //this.style.color = "blue";
                $.RAD.customDateTimePicker.prototype.selectedOne = "";
                //$.RAD.customDateTimePicker.prototype.glo
                var collection = $(".ContainerDivDateTimePicker");
                var id;
                for (var i = 0; i < collection.length; i++) {
                    id = collection[i].getAttribute("ControlID") + "_ContainerDivDateTimePicker";
                    if ($("#" + id)[0] != null && $("#" + id)[0].style.display == "") {
                        $("#" + id)[0].style.display = "none";
                        $("#" + collection[i].getAttribute("ControlID")).customDateTimePicker("option", "onClose")();
                        $.RAD.customDateTimePicker.prototype._SetDisplayNone(collection[i].getAttribute("ControlID"), ($("#" + collection[i].getAttribute("ControlID")).customDateTimePicker('option', 'type') == "DateTime" && $("#" + collection[i].getAttribute("ControlID")).customDateTimePicker('option', 'CalendarID') != '') ? true : false);
                    }
                }

                $.RAD.customDateTimePicker.prototype._createCustomDiv(this);
            });

            $(document).bind("click", function (e) {
                var collection = $(".ContainerDivDateTimePicker");
                var id;
                for (var i = 0; i < collection.length; i++) {
                    id = collection[i].getAttribute("ControlID") + "_ContainerDivDateTimePicker";
                    if ($(e.target).closest(".ContainerDivDateTimePicker")[0] != null || $(e.target).closest(".LastSubOptionsDivClass")[0] != null || $(e.target).closest(".FirstSubOptionsDivClass")[0] != null || e.target.id == $("#" + collection[i].getAttribute("ControlID")).customDateTimePicker("option", "globalVars").idOfControl) //|| e.target.id == $.RAD.customDateTimePicker.prototype.globalVars.idOfControl
                        ;
                    else {
                        if ($("#" + id)[0] != null && $("#" + id)[0].style.display == "") {
                            $("#" + id)[0].style.display = "none";
                            $("#" + collection[i].getAttribute("ControlID")).customDateTimePicker("option", "onClose")();
                            $.RAD.customDateTimePicker.prototype._SetDisplayNone(collection[i].getAttribute("ControlID"), ($("#" + collection[i].getAttribute("ControlID")).customDateTimePicker('option', 'type') == "DateTime" && $("#" + collection[i].getAttribute("ControlID")).customDateTimePicker('option', 'CalendarID') != '') ? true : false);
                            //apply T-n operation too
                            // $.RAD.customDateTimePicker.prototype._CasesOnClick();
                        }
                    }
                }


            });

        },

        _createCustomDiv: function (e) {
            var variables = $("#" + e.id).customDateTimePicker("option", "globalVars"); //$.RAD.customDateTimePicker.prototype.globalVars;

            variables.idOfControl = e.id;
            if (!variables.flagCreated) {
                var ContainerDiv = document.createElement("DIV");
                ContainerDiv.className = "ContainerDivDateTimePicker";
                ContainerDiv.setAttribute("id", variables.idOfControl + "_ContainerDivDateTimePicker");
                ContainerDiv.setAttribute("ControlID", variables.idOfControl);
                ContainerDiv.style.position = "absolute";

                var MainDiv = document.createElement("div");

                MainDiv.style.height = "217px";
                MainDiv.className = "MainDivContainingChoices";
                MainDiv.setAttribute("id", variables.idOfControl + "_MainDivContainingChoices");

                //in that case we need 2 calendars
                if (variables.customOptions.type == "DateRange") {
                    ContainerDiv.style.width = (175 + 500) + "px";
                    MainDiv.style.width = 175 + "px";
                    ContainerDiv.style.height = "217px";

                }
                else if (variables.customOptions.type == "DateTime") {
                    if (variables.customOptions.serviceOptions == true) {
                        ContainerDiv.style.width = (175 + 320) + "px";
                        MainDiv.style.width = 175 + "px";
                    }
                    else {
                        ContainerDiv.style.width = "320px";
                        MainDiv.style.width = "0px";
                    }
                    ContainerDiv.style.height = "217px";

                }
                else if (variables.customOptions.type == "DateOnly") {
                    if (variables.customOptions.serviceOptions == true) {
                        ContainerDiv.style.width = (175 + 250) + "px";
                        MainDiv.style.width = (175) + "px";
                    }
                    else {
                        ContainerDiv.style.width = 250 + "px";
                        MainDiv.style.width = "0px";

                    }
                    ContainerDiv.style.height = "217px";

                }
                else if (variables.customOptions.type == "TimeOnly") {
                    MainDiv.style.width = "0px";
                    ContainerDiv.style.width = 100 + "px";
                    ContainerDiv.style.height = "187px";

                }



                var NormalOptionsDiv = document.createElement("DIV");
                NormalOptionsDiv.className = "NormalOptionsDivClass";


                // if service options is set true //and type is dateOnly or DateTime
                if (variables.customOptions.serviceOptions == true && (variables.customOptions.type == "DateOnly" || variables.customOptions.type == "DateTime")) {
                    /*First Option*/
                    var InnerChoice;
                    var Choices = ["Today", "Yesterday", "Tomorrow", "Next Business Day"];

                    //for all of them
                    for (var i = 0; i < Choices.length; i++) {
                        InnerChoice = document.createElement("DIV");
                        InnerChoice.className = "InnerChoices"
                        InnerChoice.setAttribute("title", Choices[i]);
                        InnerChoice.innerText = Choices[i];
                        NormalOptionsDiv.appendChild(InnerChoice);
                    }


                    /*Second Option*/
                    InnerChoice = document.createElement("DIV");
                    InnerChoice.className = "InnerChoices";
                    //InnerChoice.setAttribute("title", Choices[i]);

                    /* SubInnerChoice = document.createElement("DIV");
                    SubInnerChoice.className = "LastSubInnerChoices";
                    SubInnerChoice.innerText = "Last";
                    InnerChoice.appendChild(SubInnerChoice);*/

                    var LastSubInnerChoice = document.createElement("DIV");
                    LastSubInnerChoice.className = "LastOptionsSubInnerChoices";
                    LastSubInnerChoice.innerText = "Last Business Day";
                    LastSubInnerChoice.setAttribute("title", "Last Business Day");

                    InnerChoice.appendChild(LastSubInnerChoice);
                    /*$(LastSubInnerChoice).bind("click", function (e) {
                    //if already there //then remove
                    if ($(SubDropDown)[0].style.display == "")
                    $(SubDropDown)[0].style.display = "none";
                    else
                    $(SubDropDown)[0].style.display = "";
                    $(FirstSubDropDown)[0].style.display = "none"; //other dropdown always gone
                    $(SubDropDown)[0].style.top = (e.target.offsetTop + 16) + "px";
                    $(SubDropDown)[0].style.left = (e.target.offsetLeft - 8) + "px";
                    // SubOptionsDivClass
                    });*/
                    //making a dropdown for last
                    var SubDropDown = document.createElement("DIV");
                    SubDropDown.className = "LastSubOptionsDivClass";
                    SubDropDown.setAttribute("id", variables.idOfControl + "_LastSubOptionsDivClass");
                    SubDropDown.style.display = "none";
                    var SubOptionChoices = ["Last Business Day", "Last Day of Previous Month", "Last Day of Previous Quarter", "Last Day of Previous Year"];

                    var SubDropDownOptions = document.createElement("DIV");
                    //choices are made here
                    for (var i = 0; i < SubOptionChoices.length; i++) {
                        SubDropDownOptions = document.createElement("DIV");
                        SubDropDownOptions.className = "LastSubOptionsOfSubDropDown";
                        SubDropDownOptions.innerText = SubOptionChoices[i];
                        SubDropDown.appendChild(SubDropDownOptions);
                    }
                    //e.parentElement.appendChild(SubDropDown);
                    ContainerDiv.appendChild(SubDropDown);
                    //binding click to it
                    $(SubDropDown).bind("click", function (e) {
                        $.RAD.customDateTimePicker.prototype._clickOnOptionsMainDiv(e);
                    });
                    NormalOptionsDiv.appendChild(InnerChoice);

                    /*Fourth Option*/
                    InnerChoice = document.createElement("DIV");
                    InnerChoice.className = "InnerChoices";

                    var FirstSubInnerChoice = document.createElement("DIV");
                    FirstSubInnerChoice.className = "FirstOptionsSubInnerChoices";
                    FirstSubInnerChoice.innerText = "First Day of Current Month";
                    FirstSubInnerChoice.setAttribute("title", "First Day of Current Month");
                    InnerChoice.appendChild(FirstSubInnerChoice);
                    /* $(FirstSubInnerChoice).bind("click", function (e) {
                    if ($(FirstSubDropDown)[0].style.display == "")
                    $(FirstSubDropDown)[0].style.display = "none";
                    else
                    $(FirstSubDropDown)[0].style.display = "";
                    $(SubDropDown)[0].style.display = "none";
                    $(FirstSubDropDown)[0].style.top = (e.target.offsetTop + 16) + "px";
                    $(FirstSubDropDown)[0].style.left = (e.target.offsetLeft - 8) + "px";
                    });*/
                    //making a dropdown for last
                    var FirstSubDropDown = document.createElement("DIV");
                    FirstSubDropDown.className = "FirstSubOptionsDivClass";
                    FirstSubDropDown.setAttribute("id", variables.idOfControl + "_FirstSubOptionsDivClass");
                    FirstSubDropDown.style.display = "none";
                    var SubOptionChoices = ["First Day of Current Month", "First Day of Previous Month", "First Day of Current Quarter", "First Day of Previous Quarter", "First Day of Current Year", "First Day of Previous Year"];
                    var SubDropDownOptions;
                    for (var i = 0; i < SubOptionChoices.length; i++) {
                        SubDropDownOptions = document.createElement("DIV");
                        SubDropDownOptions.className = "FirstSubOptionsOfSubDropDown";
                        SubDropDownOptions.innerText = SubOptionChoices[i];
                        FirstSubDropDown.appendChild(SubDropDownOptions);
                    }
                    //e.parentElement.appendChild(FirstSubDropDown);
                    ContainerDiv.appendChild(FirstSubDropDown);
                    //binding click to it
                    $(FirstSubDropDown).bind("click", function (e) {
                        $.RAD.customDateTimePicker.prototype._clickOnOptionsMainDiv(e);
                    });

                    NormalOptionsDiv.appendChild(InnerChoice);
                    /*Third option*/
                    InnerChoice = document.createElement("DIV");
                    InnerChoice.className = "InnerChoices";
                    InnerChoice.setAttribute("ID", "TMinusNChoiceDiv");
                    InnerChoice.setAttribute("title", "T + n business days");

                    //InnerChoice.innerText = "T - n";

                    var LabelsInInnerChoice = document.createElement("DIV");
                    LabelsInInnerChoice.innerText = "T + ";
                    LabelsInInnerChoice.className = "LabelInTMinusN";
                    InnerChoice.appendChild(LabelsInInnerChoice);

                    var InputDiv = document.createElement("DIV");
                    InputDiv.className = "InputInTMinusN";
                    var InputInInnerChoice = document.createElement("INPUT");
                    //InputInInnerChoice.className = "InputInTMinusN";
                    InputInInnerChoice.setAttribute("id", variables.idOfControl + "_InputInTMinusNTB");
                    InputInInnerChoice.setAttribute("style", "width: 25%;height: 11px;text-align: left;text-indent: 2px;");
                    InputDiv.appendChild(InputInInnerChoice);
                    InnerChoice.appendChild(InputDiv);
                    NormalOptionsDiv.appendChild(InnerChoice);

                    LabelsInInnerChoice = document.createElement("DIV");
                    LabelsInInnerChoice.className = "EndingLabelInTMinusN";
                    LabelsInInnerChoice.innerText = "business days";
                    InnerChoice.appendChild(LabelsInInnerChoice);

                    MainDiv.appendChild(NormalOptionsDiv);

                }
                else if (variables.customOptions.type == "DateRange") {

                    var DateRangeOptionsDiv = document.createElement("DIV");
                    DateRangeOptionsDiv.className = "DateRangeOptionsDiv";

                    var Choices = ["This Month", "Last Month", "Last 7 Days", "Last 15 Days", "Last Year", "Last 3 Years", "Last 5 Years"];

                    for (var i = 0; i < Choices.length; i++) {
                        InnerChoice = document.createElement("DIV");
                        InnerChoice.className = "InnerChoices";
                        InnerChoice.innerText = Choices[i];
                        InnerChoice.setAttribute("title", Choices[i]);
                        DateRangeOptionsDiv.appendChild(InnerChoice);
                    }
                    MainDiv.appendChild(DateRangeOptionsDiv);
                }
                //options div is appended here

                $(ContainerDiv).bind("click", function (e) {
                    $.RAD.customDateTimePicker.prototype._clickOnOptionsMainDiv(e);
                });

                //MainDiv.appendChild(TextBoxDiv);*/

                //T+n div
                var TPlusNDiv = document.createElement("DIV");
                TPlusNDiv.className = "T_PLUS_N_DivClass";
                TPlusNDiv.style.display = "none";

                var TPlusNDivHeader = document.createElement("DIV");
                TPlusNDivHeader.innerText = "Please Select the date from the calendar";
                TPlusNDivHeader.className = "TPlusNDivHeaderClass";
                TPlusNDiv.appendChild(TPlusNDivHeader);

                //T Div
                var TDiv = document.createElement("DIV");
                TDiv.className = "TDivClass";
                var T_Label = document.createElement("LABEL");
                T_Label.innerText = "T";
                T_Label.className = "TLabelClass";
                var T_Input = document.createElement("INPUT");
                T_Input.className = "TInputClass";
                TDiv.appendChild(T_Label);
                TDiv.appendChild(T_Input);
                TPlusNDiv.appendChild(TDiv);

                var NDiv = document.createElement("DIV");
                NDiv.className = "NDivClass";
                var N_Label = document.createElement("LABEL");
                N_Label.className = "NLabelClass";
                N_Label.innerText = "n";
                var N_Input = document.createElement("INPUT");
                N_Input.className = "NInputClass";
                NDiv.appendChild(N_Label);
                NDiv.appendChild(N_Input);
                TPlusNDiv.appendChild(NDiv);

                MainDiv.appendChild(TPlusNDiv);

                ContainerDiv.appendChild(MainDiv);

                var date = new Date();
                var leftDate = date.dateFormat(variables.rqdFormat);
                var rightDate = date.dateFormat(variables.rqdFormat);
                var LeftCalendarDiv = document.createElement("DIV");
                var LeftInnerDiv = document.createElement("DIV");
                LeftInnerDiv.className = "LeftCalendarInnerDiv";
                LeftInnerDiv.setAttribute("id", variables.idOfControl + "_LeftCalendarInnerDiv");
                LeftCalendarDiv.appendChild(LeftInnerDiv);
                LeftCalendarDiv.className = "LeftCalendarClass";
                LeftCalendarDiv.style.height = "217px";
                //LeftCalendarDiv.style.width = "300px";
                ContainerDiv.appendChild(LeftCalendarDiv);
                LeftCalendarDiv.style.display = "";
                variables.LeftCalendarOptions.onSelectDate = function (ct, $i) {

                    ct.setSeconds(0);
                    ct.setMinutes(0); ct.setHours(0);
                    leftDate = ct.dateFormat(variables.rqdFormat);
                    if (variables.customOptions.type == "DateRange") {
                        variables.previousStartDate = ct.dateFormat(variables.commonFormat);
                        variables.RightCalendarOptions.minDate = ct;
                        variables.RightCalendarOptions.value = variables.previousEndDate;
                        variables.RightCalendarOptions.format = variables.commonFormat;
                        $(RightInnerDiv).datetimepicker(variables.RightCalendarOptions);
                        $(variables.MainDOMElement).val(leftDate + " - " + rightDate);
                    }
                    else {
                        $(variables.MainDOMElement).val(leftDate);
                        $("#" + variables.idOfControl + "_InputInTMinusNTB").spinner("value", 0);
                    }
                    //$(LeftFromTextBox).val(ct.dateFormat(variables.rqdFormat));

                    variables.customOptions.onSelectDate(ct, $i);
                    $(".TTextBoxClass").val(ct.dateFormat(variables.rqdFormat));

                };
                variables.LeftCalendarOptions.onSelectTime = function (dateTime, $i) {
                    variables.RightCalendarOptions.minTime = dateTime;
                    variables.RightCalendarOptions.defaultTime = dateTime;
                    //setting the seconds 0
                    dateTime.setSeconds(0);
                    $(RightInnerDiv).datetimepicker(variables.RightCalendarOptions);
                    //$(LeftFromTextBox).val();
                    leftDate = dateTime.dateFormat(variables.rqdFormat);
                    if (variables.customOptions.type == "DateRange")
                        $(variables.MainDOMElement).val(leftDate + " - " + rightDate);
                    else
                        $(variables.MainDOMElement).val(leftDate);

                    variables.customOptions.onSelectTime(dateTime, $i);

                };

                if (variables.customOptions.type != 'DateRange') {
                    //in case text box is not empty
                    if ($(e).val() != '') {
                        // var variables = $("#" + id).customDateTimePicker('option', 'globalVars');
                        var myDate = Date.parseInvariant($(e).val(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                        if (myDate == null) {
                            myDate = Date.parseInvariant($(e).val(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
                        }
                        if (myDate == null) {
                            myDate = new Date(Date.parse($(e).val()));
                        }
                        //$(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat));
                        variables.LeftCalendarOptions.value = myDate.dateFormat(variables.commonFormat);
                        variables.LeftCalendarOptions.format = variables.commonFormat;
                        //variables.LeftCalendarOptions.defaultSelect = true;
                        //$("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);
                    }
                }

                $(LeftInnerDiv).datetimepicker(variables.LeftCalendarOptions);



                if (variables.customOptions.type == "DateRange") {
                    var RightCalendarDiv = document.createElement("DIV");
                    var RightInnerDiv = document.createElement("DIV");
                    RightInnerDiv.className = "RightCalendarInnerDiv"
                    RightInnerDiv.setAttribute("id", variables.idOfControl + "_RightCalendarInnerDiv");
                    RightCalendarDiv.appendChild(RightInnerDiv);
                    RightCalendarDiv.className = "RightCalendarClass";
                    RightCalendarDiv.style.height = "217px";
                    // RightCalendarDiv.style.width = "300px";
                    ContainerDiv.appendChild(RightCalendarDiv);
                    RightCalendarDiv.style.display = "";
                    variables.RightCalendarOptions.onSelectDate = function (ct, $i) {
                        variables.previousEndDate = ct.dateFormat(variables.commonFormat);
                        variables.LeftCalendarOptions.maxDate = ct;
                        ct.setSeconds(0);
                        ct.setMinutes(0);
                        ct.setHours(0);
                        rightDate = ct.dateFormat(variables.rqdFormat);
                        variables.LeftCalendarOptions.value = variables.previousStartDate;
                        variables.LeftCalendarOptions.format = variables.commonFormat;
                        //variables.LeftCalendarOptions.sDate = ct;
                        $(LeftInnerDiv).datetimepicker(variables.LeftCalendarOptions);
                        //$(RightToTextBox).val(ct.dateFormat(variables.rqdFormat));
                        $(variables.MainDOMElement).val(leftDate + " - " + rightDate); //setting the dateTime
                        //$(".xdsoft_current")[6].style.backgroundColor = "#ff8000";
                        variables.customOptions.onSelectRightDate(ct, $i);
                    };
                    variables.RightCalendarOptions.onSelectTime = function (dateTime, $i) {
                        variables.LeftCalendarOptions.maxTime = dateTime;
                        rightTime = dateTime.dateFormat(variables.rqdFormat);
                        dateTime.setSeconds(0);
                        //variables.LeftCalendarOptions.defaultTime = time;
                        $(variables.MainDOMElement).val(leftDate + " - " + rightDate);
                        $(LeftInnerDiv).datetimepicker(variables.LeftCalendarOptions);
                        variables.customOptions.onSelectRightTime(dateTime, $i);
                    };

                    $(RightInnerDiv).datetimepicker(variables.RightCalendarOptions);
                }
                //e.parentElement.appendChild(ContainerDiv);
                document.body.appendChild(ContainerDiv);


                //ContainerDiv.style.left = $(e).offset().left + "px";
                //ContainerDiv.style.top = ($(e).offset().top + $(e).outerHeight()) + "px";


                //calculations for setting offsetLeft and offsetTop
                var SUM = ($(e).offset().left + (parseInt(ContainerDiv.style.width.replace("px", ""))));
                //console.log("Difference " + SUM);
                if ($(window).outerWidth() >= SUM) {
                    ContainerDiv.style.left = $(e).offset().left + "px";
                }
                else {
                    var diff = (SUM - $(window).outerWidth());
                    ContainerDiv.style.left = ($(e).offset().left - diff) + "px";
                }
                //for top calculations
                var heightOfContainer = (parseInt(ContainerDiv.style.height.replace("px", "")));
                SUM = ($(e).offset().top + heightOfContainer + $(e).outerHeight());
                if ($(window).outerHeight() >= SUM)
                    ContainerDiv.style.top = ($(e).offset().top + $(e).outerHeight()) + "px";
                else //we need to flip it to top
                    ContainerDiv.style.top = ($(e).offset().top - heightOfContainer) + "px";




                if ($("#" + e.id).customDateTimePicker("option", "zIndexStyle") == '')
                    ContainerDiv.style.zIndex = "9999";
                else
                    ContainerDiv.style.zIndex = $("#" + e.id).customDateTimePicker("option", "zIndexStyle");

                variables.flagCreated = true;
            }
            else {
                var ContainerDiv = $("#" + variables.idOfControl + "_ContainerDivDateTimePicker")[0];
                if ($("#" + variables.idOfControl + "_ContainerDivDateTimePicker")[0].style.display == "none") {
                    $("#" + variables.idOfControl + "_ContainerDivDateTimePicker")[0].style.display = "";

                    if (variables.customOptions.type != 'DateRange') {
                        //in case text box is not empty
                        if ($(e).val() != '') {
                            // var variables = $("#" + id).customDateTimePicker('option', 'globalVars');
                            var myDate = Date.parseInvariant($(e).val(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                            if (myDate == null) {
                                myDate = Date.parseInvariant($(e).val(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
                            }
                            if (myDate == null) {
                                myDate = new Date(Date.parse($(e).val()));
                            }
                            //$(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat));
                            variables.LeftCalendarOptions.value = myDate.dateFormat(variables.commonFormat);
                            variables.LeftCalendarOptions.format = variables.commonFormat;
                            //variables.LeftCalendarOptions.defaultSelect = true;
                            $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);
                        }
                    }

                    var SUM = ($(e).offset().left + (parseInt(ContainerDiv.style.width.replace("px", ""))));
                    //console.log("Difference " + SUM);
                    if ($(window).outerWidth() >= SUM) {
                        ContainerDiv.style.left = $(e).offset().left + "px";
                    }
                    else {
                        var diff = (SUM - $(window).outerWidth());
                        ContainerDiv.style.left = ($(e).offset().left - diff) + "px";
                    }

                    //for top calculations
                    var heightOfContainer = (parseInt(ContainerDiv.style.height.replace("px", "")));
                    SUM = ($(e).offset().top + heightOfContainer + $(e).outerHeight());
                    if ($(window).outerHeight() >= SUM)
                        ContainerDiv.style.top = ($(e).offset().top + $(e).outerHeight()) + "px";
                    else //we need to flip it to top
                        ContainerDiv.style.top = ($(e).offset().top - heightOfContainer) + "px";


                    $("#" + variables.idOfControl + "_InputInTMinusNTB").spinner("value", variables.previousN);
                }
                else
                    $("#" + variables.idOfControl + "_ContainerDivDateTimePicker")[0].style.display = "none";

            }

            $("#" + variables.idOfControl + "_InputInTMinusNTB").spinner({
                spin: function (event, ui) {
                    var id = $(event.target).closest(".ContainerDivDateTimePicker")[0].getAttribute("ControlID");
                    var GlobalVariables = $("#" + id).customDateTimePicker('option', 'globalVars');
                    //clear previous Interval
                    window.clearTimeout(GlobalVariables.SpinInterval);
                    GlobalVariables.SpinInterval = setTimeout(function () {
                        $.RAD.customDateTimePicker.prototype._CasesOnClick(event);
                    }, 500);

                    if ($($("#" + variables.idOfControl + "_MainDivContainingChoices").children()[0]).children(".SelectedInnerChoices").length > 0)
                        $($("#" + variables.idOfControl + "_MainDivContainingChoices").children()[0]).children(".SelectedInnerChoices")[0].className = "InnerChoices";
                    //change the display of both
                    $(event.target).closest(".InnerChoices")[0].className = "SelectedInnerChoices";
                    $.RAD.customDateTimePicker.prototype._SetDisplayNone(variables.idOfControl, (GlobalVariables.customOptions.type == "DateTime" && GlobalVariables.customOptions.CalendarID != '') ? true : false);

                }
            }).val(variables.previousN);


            //onShow Event is called
            $("#" + e.id).customDateTimePicker("option", "onShow")();

            /*changing calendar*/
            // $(".xdsoft_time xdsoft_current")[0].className = "xdsoft_time xdsoft_current CurrentDateTimeSelection";
        },

        _clickOnOptionsMainDiv: function (e) {
            var id = $(e.target).closest(".ContainerDivDateTimePicker")[0].getAttribute("ControlID");
            var variables = $("#" + id).customDateTimePicker('option', 'globalVars');
            var isDateRange = false;
            if (variables.customOptions.type == "DateTime" && variables.customOptions.CalendarID != '')
                isDateRange = true;
            switch (e.target.className) {
                case "LastOptionsSubInnerChoices":
                    //if already there //then remove
                    if ($("#" + variables.idOfControl + "_LastSubOptionsDivClass")[0].style.display == "")
                        $("#" + variables.idOfControl + "_LastSubOptionsDivClass")[0].style.display = "none";
                    else
                        $("#" + variables.idOfControl + "_LastSubOptionsDivClass")[0].style.display = "";
                    $("#" + variables.idOfControl + "_FirstSubOptionsDivClass")[0].style.display = "none"; //other dropdown always gone
                    $("#" + variables.idOfControl + "_LastSubOptionsDivClass")[0].style.top = (e.target.offsetTop + 16) + "px";
                    $("#" + variables.idOfControl + "_LastSubOptionsDivClass")[0].style.left = (e.target.offsetLeft - 8) + "px";

                    break;
                case "FirstOptionsSubInnerChoices":
                    //if already there //then remove
                    if ($("#" + variables.idOfControl + "_FirstSubOptionsDivClass")[0].style.display == "")
                        $("#" + variables.idOfControl + "_FirstSubOptionsDivClass")[0].style.display = "none";
                    else
                        $("#" + variables.idOfControl + "_FirstSubOptionsDivClass")[0].style.display = "";
                    $("#" + variables.idOfControl + "_LastSubOptionsDivClass")[0].style.display = "none"; //other dropdown always gone
                    $("#" + variables.idOfControl + "_FirstSubOptionsDivClass")[0].style.top = (e.target.offsetTop + 16) + "px";
                    $("#" + variables.idOfControl + "_FirstSubOptionsDivClass")[0].style.left = (e.target.offsetLeft - 8) + "px";

                    break;
                case "InnerChoices":
                    //change the one already selected
                    if ($($("#" + variables.idOfControl + "_MainDivContainingChoices").children()[0]).children(".SelectedInnerChoices").length > 0)
                        $($("#" + variables.idOfControl + "_MainDivContainingChoices").children()[0]).children(".SelectedInnerChoices")[0].className = "InnerChoices";
                    //change the display of both
                    e.target.className = "SelectedInnerChoices";
                    $.RAD.customDateTimePicker.prototype._SetDisplayNone(variables.idOfControl, isDateRange);
                    $.RAD.customDateTimePicker.prototype._SwitchCases(e);
                    break;
                case "LastSubOptionsOfSubDropDown":
                    //change the one already selected
                    if ($($("#" + variables.idOfControl + "_MainDivContainingChoices").children()[0]).children(".SelectedInnerChoices").length > 0)
                        $($("#" + variables.idOfControl + "_MainDivContainingChoices").children()[0]).children(".SelectedInnerChoices")[0].className = "InnerChoices";
                    // e.target.parentNode.className = "SelectedInnerChoices";
                    $("#" + variables.idOfControl + "_ContainerDivDateTimePicker").find(".LastOptionsSubInnerChoices")[0].innerText = e.target.innerText;
                    $("#" + variables.idOfControl + "_ContainerDivDateTimePicker").find(".LastOptionsSubInnerChoices")[0].setAttribute("title", e.target.innerText);
                    $("#" + variables.idOfControl + "_LastSubOptionsDivClass")[0].style.display = "none";
                    $("#" + variables.idOfControl + "_ContainerDivDateTimePicker").find(".LastOptionsSubInnerChoices").parent()[0].className = "SelectedInnerChoices";
                    $.RAD.customDateTimePicker.prototype._SwitchCases(e);
                    $.RAD.customDateTimePicker.prototype._SetDisplayNone(variables.idOfControl, isDateRange);
                    break;
                case "FirstSubOptionsOfSubDropDown":
                    //change the one already selected
                    if ($($("#" + variables.idOfControl + "_MainDivContainingChoices").children()[0]).children(".SelectedInnerChoices").length > 0)
                        $($("#" + variables.idOfControl + "_MainDivContainingChoices").children()[0]).children(".SelectedInnerChoices")[0].className = "InnerChoices";
                    //e.target.parentNode.className = "SelectedInnerChoices";
                    $("#" + variables.idOfControl + "_ContainerDivDateTimePicker").find(".FirstOptionsSubInnerChoices")[0].innerText = e.target.innerText;
                    $("#" + variables.idOfControl + "_ContainerDivDateTimePicker").find(".FirstOptionsSubInnerChoices")[0].setAttribute("title", e.target.innerText);
                    $("#" + variables.idOfControl + "_FirstSubOptionsDivClass")[0].style.display = "none";
                    $("#" + variables.idOfControl + "_ContainerDivDateTimePicker").find(".FirstOptionsSubInnerChoices").parent()[0].className = "SelectedInnerChoices";
                    $.RAD.customDateTimePicker.prototype._SwitchCases(e);
                    $.RAD.customDateTimePicker.prototype._SetDisplayNone(variables.idOfControl, isDateRange);
                    break;

                default:
                    //for the cases having parentNames as any of above
                    if (e.target.parentNode.className == "InnerChoices") {
                        if ($($("#" + variables.idOfControl + "_MainDivContainingChoices").children()[0]).children(".SelectedInnerChoices").length > 0)
                            $($("#" + variables.idOfControl + "_MainDivContainingChoices").children()[0]).children(".SelectedInnerChoices")[0].className = "InnerChoices";
                        //change the display of both
                        e.target.parentNode.className = "SelectedInnerChoices";
                        $.RAD.customDateTimePicker.prototype._SetDisplayNone(variables.idOfControl, isDateRange);
                        $.RAD.customDateTimePicker.prototype._SwitchCases(e);
                    }
                    break;

            }



        },
        _SetDisplayNone: function (id, isDateTime) {
            /* $(".TTextBoxClass")[0].style.display = "none";
            $(".NTextBoxClass")[0].style.display = "none";
            */
            //var id = $(e.target).closest(".ContainerDivDateTimePicker")[0].getAttribute("ControlID");
            //var variables = $("#" + id).customDateTimePicker('option', 'globalVars');
            if (isDateTime) {
                $("#" + id + "_LastSubOptionsDivClass")[0].style.display = "none";
                $("#" + id + "_FirstSubOptionsDivClass")[0].style.display = "none";

            }
        },

        _SwitchCases: function (e) {
            var id = $(e.target).closest(".ContainerDivDateTimePicker")[0].getAttribute("ControlID");
            var variables = $("#" + id).customDateTimePicker('option', 'globalVars');
            var startDate = new Date();
            var endDate = new Date();
            variables.selectedOne = e.target.innerText;
            switch (variables.selectedOne) {
                case "Today":
                    window.clearTimeout(variables.SpinInterval);
                    variables.caseChoiceToggle = false;
                    $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat));
                    variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                    variables.LeftCalendarOptions.format = variables.commonFormat;
                    //variables.LeftCalendarOptions.defaultSelect = true;
                    $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);
                    break;

                case "Yesterday":
                    window.clearTimeout(variables.SpinInterval);
                    variables.caseChoiceToggle = false;
                    var date = new Date();
                    startDate.setDate(date.getDate() - 1);
                    endDate.setDate(date.getDate() - 1);
                    $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat)); // + " - " + endDate.dateFormat(variables.rqdFormat)
                    variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                    variables.LeftCalendarOptions.format = variables.commonFormat;
                    //variables.LeftCalendarOptions.defaultSelect = true;
                    $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);

                    break;

                case "Last 7 Days":
                    startDate.setDate(endDate.getDate() - 6);
                    variables.previousStartDate = startDate.dateFormat(variables.commonFormat);
                    variables.previousEndDate = endDate.dateFormat(variables.commonFormat);

                    $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat) + " - " + endDate.dateFormat(variables.rqdFormat));
                    variables.LeftCalendarOptions.format = variables.commonFormat;
                    variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                    //variables.LeftCalendarOptions.defaultSelect = true;
                    $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);

                    variables.RightCalendarOptions.value = endDate.dateFormat(variables.commonFormat);
                    variables.RightCalendarOptions.format = variables.commonFormat;
                    //variables.LeftCalendarOptions.defaultSelect = true;
                    $("#" + variables.idOfControl + "_RightCalendarInnerDiv").datetimepicker(variables.RightCalendarOptions);
                    break;

                case "Last 15 Days":
                    startDate.setDate(endDate.getDate() - 14);
                    $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat) + " - " + endDate.dateFormat(variables.rqdFormat));
                    variables.previousStartDate = startDate.dateFormat(variables.commonFormat);
                    variables.previousEndDate = endDate.dateFormat(variables.commonFormat);

                    variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                    variables.LeftCalendarOptions.format = variables.commonFormat;
                    //variables.LeftCalendarOptions.defaultSelect = true;
                    $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);

                    variables.RightCalendarOptions.value = endDate.dateFormat(variables.commonFormat);
                    variables.RightCalendarOptions.format = variables.commonFormat;
                    //variables.LeftCalendarOptions.defaultSelect = true;
                    $("#" + variables.idOfControl + "_RightCalendarInnerDiv").datetimepicker(variables.RightCalendarOptions);
                    break;

                case "Last Month":
                    var currentDate = new Date();
                    var currentYear = currentDate.getFullYear();
                    if (currentDate.getMonth() == 1)
                        currentYear = currentDate.getFullYear() - 1;

                    startDate = new Date(currentYear, currentDate.getMonth() - 1, 1);
                    endDate = new Date(currentYear, currentDate.getMonth(), 0);
                    variables.previousStartDate = startDate.dateFormat(variables.commonFormat);
                    variables.previousEndDate = endDate.dateFormat(variables.commonFormat);

                    $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat) + " - " + endDate.dateFormat(variables.rqdFormat));
                    variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                    variables.LeftCalendarOptions.format = variables.commonFormat;
                    //variables.LeftCalendarOptions.defaultSelect = true;
                    $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);

                    variables.RightCalendarOptions.value = endDate.dateFormat(variables.commonFormat);
                    variables.RightCalendarOptions.format = variables.commonFormat;
                    //variables.LeftCalendarOptions.defaultSelect = true;
                    $("#" + variables.idOfControl + "_RightCalendarInnerDiv").datetimepicker(variables.RightCalendarOptions);
                    break;

                case "This Month":
                    var currentDate = new Date();
                    startDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
                    endDate = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0);
                    variables.previousStartDate = startDate.dateFormat(variables.commonFormat);
                    variables.previousEndDate = endDate.dateFormat(variables.commonFormat);

                    $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat) + " - " + endDate.dateFormat(variables.rqdFormat));
                    variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                    variables.LeftCalendarOptions.format = variables.commonFormat;
                    //variables.LeftCalendarOptions.defaultSelect = true;
                    $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);

                    variables.RightCalendarOptions.value = endDate.dateFormat(variables.commonFormat);
                    variables.RightCalendarOptions.format = variables.commonFormat;
                    //variables.LeftCalendarOptions.defaultSelect = true;
                    $("#" + variables.idOfControl + "_RightCalendarInnerDiv").datetimepicker(variables.RightCalendarOptions);
                    break;


                case "Last Year":
                    var lastYear = startDate.getFullYear() - 1;
                    startDate = new Date(lastYear, 0, 1);
                    endDate = new Date(lastYear, 12, 0);
                    $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat) + " - " + endDate.dateFormat(variables.rqdFormat));

                    variables.previousStartDate = startDate.dateFormat(variables.commonFormat);
                    variables.previousEndDate = endDate.dateFormat(variables.commonFormat);
                    variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                    variables.LeftCalendarOptions.format = variables.commonFormat;
                    //variables.LeftCalendarOptions.defaultSelect = true;
                    $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);

                    variables.RightCalendarOptions.value = endDate.dateFormat(variables.commonFormat);
                    variables.RightCalendarOptions.format = variables.commonFormat;
                    //variables.LeftCalendarOptions.defaultSelect = true;
                    $("#" + variables.idOfControl + "_RightCalendarInnerDiv").datetimepicker(variables.RightCalendarOptions);
                    break;

                case "Last 3 Years":
                    var lastYear = startDate.getFullYear() - 1;
                    var last3rdYear = startDate.getFullYear() - 3;
                    startDate = new Date(last3rdYear, 0, 1);
                    endDate = new Date(lastYear, 12, 0);
                    variables.previousStartDate = startDate.dateFormat(variables.commonFormat);
                    variables.previousEndDate = endDate.dateFormat(variables.commonFormat);
                    $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat) + " - " + endDate.dateFormat(variables.rqdFormat));

                    variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                    variables.LeftCalendarOptions.format = variables.commonFormat;
                    //variables.LeftCalendarOptions.defaultSelect = true;
                    $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);

                    variables.RightCalendarOptions.value = endDate.dateFormat(variables.commonFormat);
                    variables.RightCalendarOptions.format = variables.commonFormat;
                    //variables.LeftCalendarOptions.defaultSelect = true;
                    $("#" + variables.idOfControl + "_RightCalendarInnerDiv").datetimepicker(variables.RightCalendarOptions);
                    break;

                case "Last 5 Years":
                    var lastYear = startDate.getFullYear() - 1;
                    var last5thYear = startDate.getFullYear() - 5;
                    startDate = new Date(last5thYear, 0, 1);
                    endDate = new Date(lastYear, 12, 0);
                    variables.previousStartDate = startDate.dateFormat(variables.commonFormat);
                    variables.previousEndDate = endDate.dateFormat(variables.commonFormat);
                    $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat) + " - " + endDate.dateFormat(variables.rqdFormat));

                    variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                    variables.LeftCalendarOptions.format = variables.commonFormat;
                    //variables.LeftCalendarOptions.defaultSelect = true;
                    $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);

                    variables.RightCalendarOptions.value = endDate.dateFormat(variables.commonFormat);
                    variables.RightCalendarOptions.format = variables.commonFormat;
                    //variables.LeftCalendarOptions.defaultSelect = true;
                    $("#" + variables.idOfControl + "_RightCalendarInnerDiv").datetimepicker(variables.RightCalendarOptions);
                    break;

                case "Custom Range":
                    // $(".TextBoxesDiv")[0].style.display = "";
                    $(".T_PLUS_N_DivClass")[0].style.display = "none";
                    //$(".LeftCalendarClass")[0].style.display = "";
                    //$(".RightCalendarClass")[0].style.display = ""; //change the display of both
                    $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat) + " - " + endDate.dateFormat(variables.rqdFormat));
                    break;

                case "Tomorrow":
                    window.clearTimeout(variables.SpinInterval);
                    variables.caseChoiceToggle = false;
                    var date = new Date();
                    startDate.setDate(date.getDate() + 1);
                    endDate.setDate(date.getDate() + 1);
                    $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat)); // + " - " + endDate.dateFormat(variables.rqdFormat)
                    variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                    variables.LeftCalendarOptions.format = variables.commonFormat;
                    //variables.LeftCalendarOptions.defaultSelect = true;
                    $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);
                    break;

                case "T - n":
                    //$(".TextBoxesDiv")[0].style.display = "none";
                    /*$(".TTextBoxClass")[0].style.display = "";
                    $(".NTextBoxClass")[0].style.display = "";
                    */
                    //$(".LeftCalendarClass")[0].style.display = "";
                    break;

                case "Last Business Day":
                    window.clearTimeout(variables.SpinInterval);
                    variables.caseChoiceToggle = false;
                    $.ajax({
                        type: "GET",
                        url: variables.url + "/GetLastBusinessDay?CalendarID=" + variables.customOptions.CalendarID,
                        processData: false,
                        success: function (result, status, xhr) {
                            var startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                            if (startDate == null) {
                                startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
                            }
                            if (startDate == null) {
                                startDate = new Date(Date.parse(result));
                            }
                            endDate = startDate;
                            $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat)); // + " - " + endDate.dateFormat(variables.rqdFormat)
                            variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                            variables.LeftCalendarOptions.format = variables.commonFormat;
                            //variables.LeftCalendarOptions.defaultSelect = true;
                            $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);
                        },
                        error: function (result) {
                            debugger;
                        },
                        complete: function (xhr, status) {

                        },
                        dataType: "json"
                    });
                    break;

                case "Next Business Day":
                    window.clearTimeout(variables.SpinInterval);
                    variables.caseChoiceToggle = false;
                    $.ajax({
                        type: "GET",
                        url: variables.url + "/GetNextBusinessDay?calendarID=" + variables.customOptions.CalendarID, //format=" + variables.rqdFormat + "&
                        processData: false,
                        success: function (result, status, xhr) {
                            var startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                            if (startDate == null) {
                                startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
                            }
                            if (startDate == null) {
                                startDate = new Date(Date.parse(result));
                            }
                            endDate = startDate;
                            $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat)); // + " - " + endDate.dateFormat(variables.rqdFormat)
                            variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                            variables.LeftCalendarOptions.format = variables.commonFormat;
                            //variables.LeftCalendarOptions.defaultSelect = true;
                            $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);
                        },
                        error: function (result) {
                            debugger;
                        },
                        complete: function (xhr, status) {

                        },
                        dataType: "json"
                    });
                    break;

                case "First Day of Current Month":
                    window.clearTimeout(variables.SpinInterval);
                    variables.caseChoiceToggle = false;
                    $.ajax({
                        type: "GET",
                        url: variables.url + "/GetFirstDayOfCurrentMonth?calendarID=" + variables.customOptions.CalendarID + "&finalDate=" + startDate.dateFormat(variables.commonFormat),
                        processData: false,
                        success: function (result, status, xhr) {
                            var startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                            if (startDate == null) {
                                startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
                            }
                            if (startDate == null) {
                                startDate = new Date(Date.parse(result));
                            }
                            endDate = startDate;
                            $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat)); // + " - " + endDate.dateFormat(variables.rqdFormat)
                            variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                            variables.LeftCalendarOptions.format = variables.commonFormat;
                            //variables.LeftCalendarOptions.defaultSelect = true;
                            $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);
                        },
                        error: function (result) {
                            debugger;
                        },
                        complete: function (xhr, status) {

                        },
                        dataType: "json"
                    });
                    break;

                case "First Day of Previous Month":
                    window.clearTimeout(variables.SpinInterval);
                    variables.caseChoiceToggle = false;
                    $.ajax({
                        type: "GET",
                        url: variables.url + "/GetFirstDayOfPreviousMonth?calendarID=" + variables.customOptions.CalendarID + "&finalDate=" + startDate.dateFormat(variables.commonFormat),
                        processData: false,
                        success: function (result, status, xhr) {
                            var startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                            if (startDate == null) {
                                startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
                            }
                            if (startDate == null) {
                                startDate = new Date(Date.parse(result));
                            }
                            endDate = startDate;
                            $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat)); // + " - " + endDate.dateFormat(variables.rqdFormat)]
                            variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                            variables.LeftCalendarOptions.format = variables.commonFormat;
                            //variables.LeftCalendarOptions.defaultSelect = true;
                            $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);
                        },
                        error: function (result) {
                            debugger;
                        },
                        complete: function (xhr, status) {

                        },
                        dataType: "json"
                    });
                    break;

                case "Last Day of Previous Month":
                    window.clearTimeout(variables.SpinInterval);
                    variables.caseChoiceToggle = false;
                    $.ajax({
                        type: "GET",
                        url: variables.url + "/GetLastDayOfPreviousMonth?calendarID=" + variables.customOptions.CalendarID + "&finalDate=" + startDate.dateFormat(variables.commonFormat),
                        processData: false,
                        success: function (result, status, xhr) {
                            var startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                            if (startDate == null) {
                                startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
                            }
                            if (startDate == null) {
                                startDate = new Date(Date.parse(result));
                            }
                            endDate = startDate;
                            $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat)); // + " - " + endDate.dateFormat(variables.rqdFormat)
                            variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                            variables.LeftCalendarOptions.format = variables.commonFormat;
                            //variables.LeftCalendarOptions.defaultSelect = true;
                            $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);
                        },
                        error: function (result) {
                            debugger;
                        },
                        complete: function (xhr, status) {

                        },
                        dataType: "json"
                    });
                    break;

                case "First Day of Current Quarter":
                    window.clearTimeout(variables.SpinInterval);
                    variables.caseChoiceToggle = false;
                    $.ajax({
                        type: "GET",
                        url: variables.url + "/GetFirstDayOfCurrentQuarter?calendarID=" + variables.customOptions.CalendarID + "&finalDate=" + startDate.dateFormat(variables.commonFormat), // + "&format=" + variables.rqdFormat
                        processData: false,
                        success: function (result, status, xhr) {
                            var startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                            if (startDate == null) {
                                startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
                            }
                            if (startDate == null) {
                                startDate = new Date(Date.parse(result));
                            }
                            endDate = startDate;
                            $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat)); // + " - " + endDate.dateFormat(variables.rqdFormat)
                            variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                            variables.LeftCalendarOptions.format = variables.commonFormat;
                            //variables.LeftCalendarOptions.defaultSelect = true;
                            $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);
                        },
                        error: function (result) {
                            debugger;
                        },
                        complete: function (xhr, status) {

                        },
                        dataType: "json"
                    });

                    break;

                case "First Day of Previous Quarter":
                    window.clearTimeout(variables.SpinInterval);
                    variables.caseChoiceToggle = false;
                    $.ajax({
                        type: "GET",
                        url: variables.url + "/GetFirstDayOfPreviousQuarter?calendarID=" + variables.customOptions.CalendarID + "&finalDate=" + startDate.dateFormat(variables.commonFormat), //+ "&format=" + variables.rqdFormat
                        processData: false,
                        success: function (result, status, xhr) {
                            var startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                            if (startDate == null) {
                                startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
                            }
                            if (startDate == null) {
                                startDate = new Date(Date.parse(result));
                            }
                            endDate = startDate;
                            $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat)); // + " - " + endDate.dateFormat(variables.rqdFormat)
                            variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                            variables.LeftCalendarOptions.format = variables.commonFormat;
                            //variables.LeftCalendarOptions.defaultSelect = true;
                            $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);
                        },
                        error: function (result) {
                            debugger;
                        },
                        complete: function (xhr, status) {

                        },
                        dataType: "json"
                    });
                    break;

                case "Last Day of Previous Quarter":
                    window.clearTimeout(variables.SpinInterval);
                    variables.caseChoiceToggle = false;
                    $.ajax({
                        type: "GET",
                        url: variables.url + "/GetLastDayOfPreviousQuarter?calendarID=" + variables.customOptions.CalendarID + "&finalDate=" + startDate.dateFormat(variables.commonFormat), // + "&format=" + variables.rqdFormat
                        processData: false,
                        success: function (result, status, xhr) {
                            var startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                            if (startDate == null) {
                                startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
                            }
                            if (startDate == null) {
                                startDate = new Date(Date.parse(result));
                            }
                            endDate = startDate;
                            $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat)); // + " - " + endDate.dateFormat(variables.rqdFormat)
                            variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                            variables.LeftCalendarOptions.format = variables.commonFormat;
                            //variables.LeftCalendarOptions.defaultSelect = true;
                            $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);
                        },
                        error: function (result) {
                            debugger;
                        },
                        complete: function (xhr, status) {

                        },
                        dataType: "json"
                    });
                    break;

                case "First Day of Current Year":
                    window.clearTimeout(variables.SpinInterval);
                    variables.caseChoiceToggle = false;
                    $.ajax({
                        type: "GET",
                        url: variables.url + "/GetFirstDayOfCurrentYear?calendarID=" + variables.customOptions.CalendarID + "&finalDate=" + startDate.dateFormat(variables.commonFormat),
                        processData: false,
                        success: function (result, status, xhr) {
                            var startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                            if (startDate == null) {
                                startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
                            }
                            if (startDate == null) {
                                startDate = new Date(Date.parse(result));
                            }
                            endDate = startDate;
                            $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat)); // + " - " + endDate.dateFormat(variables.rqdFormat)
                            variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                            variables.LeftCalendarOptions.format = variables.commonFormat;
                            //variables.LeftCalendarOptions.defaultSelect = true;
                            $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);
                        },
                        error: function (result) {
                            debugger;
                        },
                        complete: function (xhr, status) {

                        },
                        dataType: "json"
                    });
                    break;

                case "First Day of Previous Year":
                    window.clearTimeout(variables.SpinInterval);
                    variables.caseChoiceToggle = false;
                    $.ajax({
                        type: "GET",
                        url: variables.url + "/GetFirstDayOfPreviousYear?calendarID=" + variables.customOptions.CalendarID + "&finalDate=" + startDate.dateFormat(variables.commonFormat),
                        processData: false,
                        success: function (result, status, xhr) {
                            var startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                            if (startDate == null) {
                                startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
                            }
                            if (startDate == null) {
                                startDate = new Date(Date.parse(result));
                            }
                            endDate = startDate;
                            $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat)); // + " - " + endDate.dateFormat(variables.rqdFormat)
                            variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                            variables.LeftCalendarOptions.format = variables.commonFormat;
                            //variables.LeftCalendarOptions.defaultSelect = true;
                            $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);
                        },
                        error: function (result) {
                            debugger;
                        },
                        complete: function (xhr, status) {

                        },
                        dataType: "json"
                    });
                    break;

                case "Last Day of Previous Year":
                    window.clearTimeout(variables.SpinInterval);
                    variables.caseChoiceToggle = false;
                    $.ajax({
                        type: "GET",
                        url: variables.url + "/GetLastDayOfPreviousYear?calendarID=" + variables.customOptions.CalendarID + "&finalDate=" + startDate.dateFormat(variables.commonFormat),
                        processData: false,
                        success: function (result, status, xhr) {
                            var startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                            if (startDate == null) {
                                startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
                            }
                            if (startDate == null) {
                                startDate = new Date(Date.parse(result));
                            }
                            endDate = startDate;
                            $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat)); // + " - " + endDate.dateFormat(variables.rqdFormat)
                            variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                            variables.LeftCalendarOptions.format = variables.commonFormat;
                            //variables.LeftCalendarOptions.defaultSelect = true;
                            $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);
                        },
                        error: function (result) {
                            debugger;
                        },
                        complete: function (xhr, status) {

                        },
                        dataType: "json"
                    });
                    break;

            }

        },

        show: function () {
            //$(a[0]).show();
            //alert("yeh");
            $("#" + this.element[0].id)[0].click();
            //ContainerDiv.style.left = $("#" + this.element[0].id).offset().left + "px";
            //ContainerDiv.style.top = ($("#" + this.element[0].id).offset().top + 22) + "px";
        },

        hide: function () {
            //alert("I am sorry");
            var elementID = this.element[0].id;
            var id = elementID + "_ContainerDivDateTimePicker";
            if ($("#" + id)[0] != null && $("#" + id)[0].style.display == "") {
                $("#" + id)[0].style.display = "none";
                $("#" + elementID).customDateTimePicker("option", "onClose")();
                $.RAD.customDateTimePicker.prototype._SetDisplayNone(elementID, ($("#" + elementID).customDateTimePicker('option', 'type') == "DateTime" && $("#" + elementID).customDateTimePicker('option', 'CalendarID') != '') ? true : false);
            }
        },

        _CasesOnClick: function (e) {
            var id = $(e.target).closest(".ContainerDivDateTimePicker")[0].getAttribute("ControlID");
            var variables = $("#" + id).customDateTimePicker('option', 'globalVars');

            variables.previousN = 0;

            //if ($($(".MainDivContainingChoices").children()[0]).children(".SelectedInnerChoices")[0] != null && $($(".MainDivContainingChoices").children()[0]).children(".SelectedInnerChoices")[0].getAttribute("id") == "TMinusNChoiceDiv") {
            variables.selectedOne = "TPlusN";
            var startDate;
            if ($("#" + variables.idOfControl).val() == "") //if nothing selected previously....today's date
            {
                startDate = new Date();
                variables.previousDate = startDate.dateFormat(variables.rqdFormat);
            }
            else if (variables.caseChoiceToggle) //if we are on the same control again //take previousDate
            {
                var startDate = Date.parseInvariant(variables.previousDate, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                if (startDate == null) {
                    startDate = Date.parseInvariant(variables.previousDate, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
                }
                if (startDate == null) {
                    startDate = new Date(variables.previousDate);
                }
            }
            else if (!variables.caseChoicesToggle) //else take the one in textbox
            {
                var startDate = Date.parseInvariant($("#" + variables.idOfControl).val(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                if (startDate == null) {
                    startDate = Date.parseInvariant($("#" + variables.idOfControl).val(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
                }
                if (startDate == null) {
                    startDate = new Date(Date.parse($("#" + variables.idOfControl).val()));
                }
                variables.previousDate = startDate.dateFormat(variables.rqdFormat);
            }
            variables.caseChoiceToggle = true;
            variables.previousN = $("#" + variables.idOfControl + "_InputInTMinusNTB").spinner("value");
            $.ajax({
                type: "GET",
                url: variables.url + "/GetTPlusNDay?calendarID=" + variables.customOptions.CalendarID + "&finalDate=" + startDate.dateFormat(variables.commonFormat) + "&numberOfDays=" + variables.previousN, // $(".NTextBoxClass").val(), //startDate.dateFormat(variables.commonFormat)
                processData: false,
                success: function (result, status, xhr) {
                    var startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                    if (startDate == null) {
                        startDate = Date.parseInvariant(result, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
                    }
                    if (startDate == null) {
                        startDate = new Date(Date.parse(result));
                    }
                    endDate = startDate;
                    $(variables.MainDOMElement).val(startDate.dateFormat(variables.rqdFormat)); // + " - " + endDate.dateFormat(variables.rqdFormat)
                    variables.LeftCalendarOptions.value = startDate.dateFormat(variables.commonFormat);
                    variables.LeftCalendarOptions.format = variables.commonFormat;
                    //variables.LeftCalendarOptions.defaultSelect = true;
                    $("#" + variables.idOfControl + "_LeftCalendarInnerDiv").datetimepicker(variables.LeftCalendarOptions);
                },
                error: function (result) {
                    debugger;
                },
                complete: function (xhr, status) {

                },
                dataType: "json"
            });
            // } 


            //}
        }

    });  //widget ends here

    /* $("#dateTimePickerTextBox").customDateTimePicker({ type: 'DateTime', zIndexStyle: 1029, format: 'm.d.Y H:i:s', step: 5, CalendarID: 10, onSelectDate: function (ct, $i) { alert(ct); }, onShow: function () { console.log("OnShow Clicked!!"); }, onClose: function () { console.log("OnClose Clicked!!"); } }); //, onSelectDate: function () { alert("I am selecting date"); }, onSelectTime: function () { alert("I am selecting time"); }, OnChangeYear: function () { alert("Changing year"); }, onChangeMonth: function () { alert("changing month"); }, onChangeDateTime: function () { alert("Changing DateTime"); } 
    $("#dateTimePickerSecond").customDateTimePicker({ type: 'DateTime', format: 'm.d.Y H:i', step: 5, onClose: function () { console.log("OnClose Clicked!!"); }, onShow: function () { console.log("OnShow Clicked!!"); } });
    $("#dateTimePickerSecond").customDateTimePicker("show", "dateTimePickerSecond");
    //$("#dateTimePickerSecond").customDateTimePicker("hide", "dateTimePickerSecond");
    */

});

//1 start date // exact month
//0 end date //exact month +1