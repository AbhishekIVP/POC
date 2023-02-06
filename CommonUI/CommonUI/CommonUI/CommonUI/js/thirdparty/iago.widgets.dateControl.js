$(function () {
    $.widget("iago-widget.dateControl", {
        options: {
            startDate: moment().format('MMM DD YYYY hh:mm a'),
            isOptional: false,
            valueSet: false,
            /*moment().subtract('month',1).format('MMMM DD YYYY')*/
            endDate: moment().format('MMM DD YYYY hh:mm a'),
            format: 'MMM DD YYYY hh:mm a',
            displayFormat: 'MMM DD,YYYY',
            formatter: 'bigDate',
            controlFormat: 'DateRange',
            applyClass: 'btn-iago-green',
            widgetCacheState: 'INHERIT',
            enableTimePicker: false,
            minDate:moment(new Date()).subtract(10,'y'),
            maxDate:moment(new Date()).add(1,'y'),
            timePickerIncrement: 30,
            dateRangeOptions: {
                // 'Today': [moment(), moment()],
                //'Yesterday': [moment().subtract('days', 1), moment().subtract('days', 1)],
                //'Last 7 Days': [moment().subtract('days', 6), moment()],
                //                'Last 30 Days': [moment().subtract('days', 29), moment()],
                //'This Month': [moment().startOf('month'), moment().endOf('month')],
                //                'Last Month': [moment().subtract('month', 1).startOf('month'), moment().subtract('month', 1).endOf('month')],
                //                'Last Year': [moment().subtract('year', 1).startOf('year')],
                //'Last 3 Years': [moment().subtract('year', 3).startOf('year')],
                //'Last 5 Years': [moment().subtract('year', 5).startOf('year')]

            },
            isDisable:false,
            showDropdowns: true,
            asOfDateOptions: {
                //                'Today': [moment(), moment()],
                //                'Yesterday': [moment().subtract('days', 1), moment().subtract('days', 1)]
            },
            onChange: function (startDate, endDate) {}
        },
        _create: function () {
            var self = this;
            /* if ((self.options.widgetCacheState === 'INHERIT' && iago.core.currentSandBox.getPageCacheState() === true) || 
            (self.options.widgetCacheState === true)) {
            self.options = iago.core.currentSandBox.getWidgetOptionInfoFromSession($(self.element).id);
            }*/


            self.options.startDate = moment(self.options.startDate);
            self.options.endDate = moment(self.options.endDate);

            function widgetViewModel() {
                var self = this;
                // Define your viewModel below
                //  self.text = ko.observable('');
            }

            function render(ele, options, self) {

                // Put Rendering Code here(HTML)
                if (options.controlFormat === 'AsOfDate')
                    ele.addClass('AsOfDateFormat');
                $('<i class="glyphicon glyphicon-calendar fa fa-calendar custom-margin-calender" ></i><span class="custom-span" ></span><i class="optionalDate fa fa-times"></i>').appendTo(ele);

                var id = $(ele).attr('id');
                if (options.controlFormat === 'AsOfDate') {
                    self.formatter(options.controlFormat, options.formatter);
                } else if (options.controlFormat === 'DateRange') {
                    self.formatter(options.controlFormat, options.formatter);
                }

                self.baseWidget = $('#' + id).daterangepicker({
                        ranges: options.ranges,
                        startDate: options.startDate,
                        endDate: options.endDate,
                        minDate:options.minDate,
                        maxDate:options.maxDate,
                        showDropdowns: options.showDropdowns,
                        opens:'left',
                        applyClass: options.applyClass,
                        timePicker: options.enableTimePicker,
                        timePickerIncrement: options.timePickerIncrement,
                        singleDatePicker: (options.controlFormat === 'AsOfDate')
                    },
                    function (start, end, label) {
                        if (options.controlFormat === 'AsOfDate') {
                            options.startDate = start;
                            self.formatter(options.controlFormat, options.formatter)
                        } else if (options.controlFormat === 'DateRange') {
                            options.startDate = start;
                            options.endDate = end;
                            self.formatter(options.controlFormat, options.formatter)
                        }
                        if (options.onChange)
                            options.onChange(start, end); //dats from base date picker sent 
                    });
                self.id = id;
                if (options.isOptional) {
                    $('#' + id + ' span.custom-span').addClass('goHide');
                    $('#' + self.id + ' span.custom-span').addClass('goHide');
                    $("#" + id).on('apply.daterangepicker', function () {
                        $('#' + id + ' span.custom-span').removeClass('goHide');
                        $('#' + id + ' i.optionalDate').removeClass('goHide');
                        self.options.valueSet = true;
                    });


                    $('#' + id + ' i.optionalDate').off().on('click', function (evt) {

                        $('#' + self.id + ' span.custom-span').addClass('goHide');
                        self.options.isOptional = true;
                        self.options.valueSet = false;
                        $('#' + self.id + ' i.optionalDate').addClass('goHide');
                        //evt.preventDefault(); 
                        self.baseWidget.data().daterangepicker.hide();
                        evt.stopPropagation();
                    });
                }

            }
            self.element.addClass('date-div');
            if (!self.ranges) {
                if (self.options.controlFormat === 'DateRange') {
                    self.options.ranges = self.options.dateRangeOptions;
                } else if (self.options.controlFormat === 'AsOfDate') {
                    self.options.ranges = self.options.asOfDateOptions;
                }
            }

            render(this.element, self.options, self);
            this.options.identifier = this.element.attr("id");
            if (self.options.controlFormat === 'AsOfDate') {
                $('#' + self.element.attr('id') + "_dateInfo").addClass('opensrighttiptip');
            }
            var viewModel = new widgetViewModel();
           
            if( self.options.isDisable){
                self.disableDateControl(this.options.identifier);
            }
            if(iago.core != null) {
				iago.core.registerWidget({
					id: this.options.identifier,
					widget: "dateControl"
				});
			}
        },
		
        disableDateControl:function(domId)
        {
            $("#"+domId).prop('title','Date Selection Disabled');
            $('#'+domId).click(function(){
              $('#' +domId + "_dateInfo").hide();  
            })
            $('#' +domId + "_dateInfo").hide();
        },
        formatter: function (controlFormat, formatter) {
            var id = $(this.element).attr("id");
            var options = this.options;
            if (options.isOptional) {
                $('#' + id + ' i.optionalDate').addClass('goHide');
            } else {
                $('#' + id + ' span.custom-span').removeClass('goHide');

            }

            if (options.valueSet) {
                $('#' + id + ' span.custom-span').removeClass('goHide');
                $('#' + id + ' i.optionalDate').removeClass('goHide');
            } else {
                $('#' + id + ' i.optionalDate').addClass('goHide');
            }
            if (controlFormat == 'AsOfDate') {
                switch (formatter) {
                case 'bigDate':
                    $('#' + id + ' span.custom-span').html('<div class="fromDate"><span class="dateNumber">' + options.startDate.format("DD") + '</span><span class="dateMonthYear">' + options.startDate.format("MMM YYYY") + '</span></div>');
                    break;
                case 'iago':
                    $('#' + id + ' span.custom-span').html('<span class="iago-old-asofdate">' + options.startDate.format(options.displayFormat) + '</span>');
                    break;
                default:
                    if (typeof (formatter) == 'function') {
                        formatter($('#' + id + ' span.custom-span'), options.startDate);
                    }

                }
            } else if (controlFormat == 'DateRange') {
                switch (formatter) {
                case 'iago':
                    $('#' + id + ' span.custom-span').html('<span class="iago-old-daterange">' + options.startDate.format(options.displayFormat) + ' - ' + options.endDate.format(options.displayFormat) + '</span>');
                break;
                case 'bigDate':
                    var $from='<div class="fromDate"><span class="dateNumber">' + options.startDate.format("DD") + '</span><span class="dateMonthYear">' + options.startDate.format("MMM YYYY") + '</span></div>';
                    var $to='<div class="fromDate"><span class="dateNumber">' + options.endDate.format("DD") + '</span><span class="dateMonthYear">' + options.endDate.format("MMM YYYY") + '</span></div>';    
                        $('#' + id + ' span.custom-span').html($from+'<span class="bigDateHigfin">-</span>'+$to);
                        break;
                    default:
                    if (typeof (formatter) == 'function') {
                        formatter($('#' + id + ' span.custom-span'), options.startDate, options.endDate);
                    }

                }
            }
        },
        /* -->1 Leave as it is */
        getJson: function () {

            if ($('#' + this.id + ' span.custom-span').hasClass('goHide')) {
                return "";
            }
            return this.options;
        },
        getSelectedValue: function () {
            return {};
        },
        getSelectedText: function () {
            return {};
        },

        getViewModel: function () {
            return viewModel;
        },

        getCurrentOptionInfo: function () {
            /* TODO */
            return {};
        },

        setJson: function (optionData) {
            var self = this;
            $.extend(this.options, optionData);
            var options = this.options;
            if (optionData === undefined) {
                return;
            } else {
                this.options.startDate = moment(optionData.startDate);
                this.baseWidget.data('daterangepicker').startDate = this.options.startDate;
                if (optionData.endDate == undefined) {
                    this.baseWidget.data('daterangepicker').endDate = this.options.startDate;
                } else {
                    this.options.endDate = moment(optionData.endDate);
                    this.baseWidget.data('daterangepicker').endDate = this.options.endDate;
                }
                if (this.options.controlFormat == 'AsOfDate') {
                    this.baseWidget.data('daterangepicker').setEndDate(this.baseWidget.data('daterangepicker').endDate);
                } else {
                    this.baseWidget.data('daterangepicker').setStartDate(this.baseWidget.data('daterangepicker').startDate);
                    this.baseWidget.data('daterangepicker').setEndDate(this.baseWidget.data('daterangepicker').endDate);

                }
                options.valueSet = true;
                this.formatter(this.options.controlFormat, this.options.formatter);
                this.options.onChange(this.baseWidget.data('daterangepicker').startDate, this.baseWidget.data('daterangepicker').endDate);
            }

        },

        _setOptions: function () {
            this._superApply(arguments);
        },

        _setOption: function (key, value) {
            this._super(key, value);
        },
        _destroy: function () {
			if(iago.core != null) {
				iago.core.deRegisterWidget({
					id: this.options.identifier
				});
			}
            $.Widget.prototype.destroy;

            this.element
                .removeClass("AsOfDateFormat")
                .removeClass("date-div")
                .text("");

        }
    });
    $('[data-type="iago:dateControl"]').each(function (index, value) {
        if ($(value).attr('data-options-type') === "JSON")
            $(value).dateControl(JSON.parse($(value).attr('data-options')));
    });
});