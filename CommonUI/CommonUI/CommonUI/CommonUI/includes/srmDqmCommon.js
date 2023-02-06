var srmDQMStatus = {};
srmDQMStatus._pageViewModelInstance = null; // //       NOT GETTING
srmDQMStatus._chainViewModelInstance = null;
srmDQMStatus._taskViewModelInstance = null;
srmDQMStatus._protoObject = {};
srmDQMStatus._chartContainerId = 'srmDQMChartContainer';
var testData = {};

srmDQMStatus.registerSrmDqmClient = function (moduleId, protoObject) {                      //not used here
    srmDQMStatus._protoObject[moduleId] = protoObject;
};

srmDQMStatus.pageViewModel = function (data) {
    var self = this;

    self.chainListing = ko.observableArray();
    if (typeof (data) != 'undefined') {
        for (var item in data) {
            self.chainListing.push(new srmDQMStatus.chainViewModel(data[item], item));
        }
    }
};
//top view, pushes data to the chainview


srmDQMStatus.chainViewModel = function (data, chainIndex) {
    var self = this;
    self.taskListing = ko.observableArray();
    self.chainName = '';
    self.chainDuration = '';
    self.chainIndex = chainIndex;                   //probably what is selected
    self.showTasks = ko.observable(false);          //collapsed or not

    self.chainName = data.ChainName;
    self.chainTime = data.ChainTime;
    self.chainDate = data.ChainDate;
    self.chainAvgDuration = data.ChainAvgDuration;
    self.chainDuration = data.ChainDuration;
    self.warningCount = parseInt(data.WarningCount);            //as integer displayed
    self.state = data.State;
    self.chainStatusIconStyle = ko.computed(function () {       //modifies text style depending upon state
        var state = '#9eb395';
        if (self.state == 'Warning')
            state = '#eab671'
        else if (self.state == 'Failed')
            state = '#ce9196'
        return state;
    });

    self.chainInfoBackgroundColor = ko.computed(function () {       //modifies backgrnd style depending upon state
        var state = '#eef3ec';
        if (self.state == 'Warning')
            state = '#f0ebe7'
        else if (self.state == 'Failed')
            state = '#f1ebeb'
        return state;
    });
    if (typeof (data) != 'undefined') {                             //population taskdetails array by adding all the items inside
        for (var item in data.TaskDetails) {
            self.taskListing.push(new srmDQMStatus.taskViewModel(data.TaskDetails[item]));
        }
    }
};

srmDQMStatus.taskViewModel = function (data) {
    var self = this;
    self.taskName = data.TaskName;
    self.taskDuration = data.Duration;
    self.taskType = data.TaskType;
    self.moduleId = data.ModuleId;          //some don't have moduleID in json.
};





srmDQMStatus.init = function () {
    var dataTest = {};
    $.ajax({
        type: 'POST',
        url: setPath() + "/GetChainData",                       //TODO
        contentType: "application/json",
        dataType: "json",
        data: "",
        //data: JSON.stringify({ username: overrideStatus._username, attrInfo: overrideData }),
        success: function (data) {
            dataTest = data;
            console.log(dataTest);

            //AJAX Call

            if (typeof ko !== 'undefined') {                                                                //if ko object exists, then srmDQM status is currently pageview
                srmDQMStatus._pageViewModelInstance = new srmDQMStatus.pageViewModel(data.d);
                console.log(dataQualityJson);
                ko.applyBindings(srmDQMStatus._pageViewModelInstance);

                //$("#srmDQMBody").width()

            }

        },
        error: function () {
            console.log("Chain Data cannot be fetched");
        }
    });
    //if (typeof ko !== 'undefined') {                                                                //if ko object exists, then srmDQM status is currently pageview
    //    srmDQMStatus._pageViewModelInstance = new srmDQMStatus.pageViewModel(dataQualityJson);
    //    console.log(dataQualityJson);
    //    ko.applyBindings(srmDQMStatus._pageViewModelInstance);

    //    //$("#srmDQMBody").width()
    //}
    

    srmDQMStatus.refresh();
    
};

srmDQMStatus.refresh = function () {
    $($('.srmDQMChain')[0]).children('.srmDQMChainName').click();                               //jquery class selector, ARRAY WHY?, select's all cildren     //-

    $('#srmDQMBody').smslimscroll({
        height: $(window).height() - 40 + 'px', width: $("#srmDQMBody").width() + 20 + 'px', alwaysVisible: false, position: 'right', distance: '15px'
    });



    $('#srmDQMChartContainer').smslimscroll({
        height: $(window).height() - 40 + 'px', width: $("#srmDQMChartContainer").width() + 50 + 'px', alwaysVisible: false, position: 'right'
    });


    ///testing a service call
    $.ajax({
        type: 'POST',
        url: setPath() + "/GetChartData",                       //TODO
        contentType: "application/json",
        dataType: "json",
        data: "",
        //data: JSON.stringify({ username: overrideStatus._username, attrInfo: overrideData }),
        success: function (data) {
            testData = data;
            console.log(testData.d.seriesData.chartData[0].Key);
            
            
        },
        error: function () {
            console.log("Chart Data cannot be fetched");
        }
    });
}
    
srmDQMStatus.onClickTask = function (obj, event) {
    $('.srmDQMChartArrow').css('top', $(event.target).offset().top-30);
    $('#sqmDQMChart').css('left', '550px');
    $('#sqmDQMChart').css('top', $($(event.target).parents('.srmDQMChain')[0]).offset().top);
    $('#sqmDQMChart').css('display', 'block');
   
    console.log(parseInt(obj.moduleId));
    //console.log(obj.moduleId);
    srmDQMStatus._protoObject[parseInt(obj.moduleId)].manageTaskClickEvent(1, 1, 1, srmDQMStatus._chartContainerId, obj);                    //RM/SM linked  //pass whole object or some part
                                                                                                                                            
};

srmDQMStatus.onClickChain = function (obj, event) {
    for (var item in srmDQMStatus._pageViewModelInstance.chainListing())
        srmDQMStatus._pageViewModelInstance.chainListing()[item].showTasks(false);                  //uncollapse all
    obj.showTasks(true);                                                                            //collapse current    
    var chainIndex = obj.chainIndex;
    $('.srmDQMChartArrow').css('top', $(event.target).offset().top + 5 - 30);
    $('.srmDQMWarningIcon').css('height', '36px');
    $($('.srmDQMWarningIcon')[chainIndex]).css('height', $($('.srmDQMChain')[chainIndex]).height());
 
    srmDQMStatus.bindChainLevelChart(obj);
};

srmDQMStatus.bindChainLevelChart = function (obj) {
    var chartDivID = 'srmDQMChart_' + GetGUID();            //GetGUID?
    var chartDivID2 = 'srmDQMChart_' + GetGUID();
    $('#srmDQMChartContainer').html('<div id = ' + chartDivID + ' class="srmDQMChart"></div><div id = ' + chartDivID2 + ' class="srmDQMChart"></div>');         //dynamically adding these graphs to the HTML
    srmDQMStatus.bindLineChart(chartDivID, chainChartConfigJson.Title, chainChartConfigJson.SubTitle, chainChartConfigJson.YAxisText, chainChartConfigJson.LegendLayout, chainChartConfigJson.LegendAlign, chainChartConfigJson.LegendVerticalAlign, chainChartConfigJson.PlotStartPoint, chainLineGraph1DataJson.SeriesData); // chainLineGraph1DataJson.SeriesData);
    srmDQMStatus.bindLineChart(chartDivID2, chainChartConfigJson.Title, chainChartConfigJson.SubTitle, chainChartConfigJson.YAxisText, chainChartConfigJson.LegendLayout, chainChartConfigJson.LegendAlign, chainChartConfigJson.LegendVerticalAlign, chainChartConfigJson.PlotStartPoint, chainLineGraph2DataJson.SeriesData);
   
};

srmDQMStatus.applySlimScroll = function (containerSelector, bodySelector) {         //scroll logic
    var scrollBodyContainer = $(containerSelector);
    var scrollBody = scrollBodyContainer.find(bodySelector);

    if (scrollBodyContainer.height() < scrollBody.height()) {
        scrollBody.smslimscroll({ height: scrollBodyContainer.height() + 'px', alwaysVisible: true, position: 'right', distance: '2px' });
    }
};

//before it was changed
//srmDQMStatus.bindLineChart = function (containerID, title, subtitle, yAxisText, legendLayout, legendAlign, legendVerticalAlign, plotStartPoint, seriesData) {

//new with width 200px
srmDQMStatus.bindLineChart = function (containerID, title, subtitle, yAxisText, legendLayout, legendAlign, legendVerticalAlign, plotStartPoint, seriesData) {

    Highcharts.chart(containerID, {
        chart: {
            type: 'spline',
            height: 300
        },
        credits: {
            enabled: false
        },
        title: {
            text: title
        },
        subtitle: {
            text: subtitle
        },
        xAxis: {
            //x axis title maybe?
            title: {
                text: "Date",        //
                offset: 30
            },
            type: 'date',           //
            lineWidth: 1,
            tickWidth: 1,
            labels: {
                format: '{value:%e %b %y}'
            }
        },
        yAxis: {
            title: {
                text: yAxisText,        //
                offset: 50
            },
            lineWidth: 1,
            opposite: false,
            offset: 0,
                             //no clue why this is used?
          
            labels: {
                style: {
                    color: '#b5b5b6'
                }
            },
            gridLineColor: '#f2f2f2'
        },
        tooltip: {
            headerFormat: '<b>{series.name}</b><br>',
            pointFormat: '{point.x:%e %b 20%y}<br>Time Taken: {point.y:.f} mins'
            
        },
        exporting: {
            enabled: false
        },
        credits: false,
        //        legend: {
        //            enabled: false
        //        },
        scrollbar: {
            enabled: false
        },
        legend: {
            enabled: false
        },
        rangeSelector: {
            selected: 1,
            buttons: [
                        {
                            type: 'all',
                            text: 'All'
                        }
            ],
            buttonTheme: {
                visibility: 'hidden'
            },
            labelStyle: {
                visibility: 'hidden'
            },
            inputEnabled: false
        },
        navigator: {
            top: 480 // Reposition the navigator based on the top of the chart
        },
        responsive: {
            rules:  [{
                condition: {
                    maxHeight: 200
                }
            }]
        },
        plotOptions: {
           
        },
        series: seriesData
    });
}








function initializeDivFilterDate() {
    var obj = {};

    obj.dateOptions = [4]; //Which options to be shown on calender
    obj.dateFormat = 'd/m/Y';
    obj.timePicker = false;  //to show time in calender
    obj.calenderType = 0;
    obj.lefttimePicker = false;
    obj.righttimePicker = false;
    obj.pivotElement = $('[id$=DqmDatesFilter]'); //Calender will be displayed when clicked on this div

    //Which date to set in calender   
    obj.StartDateCalender = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
    obj.EndDateCalender = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));

    //Which Dateoption to select by default
    obj.selectedTopOption = 4;

    //What text by default to set in Last/Next (?)  Days
    //obj.selectedNtimeText = $('[id$=hdnvar2]').val();

    //What option by default to set in Last/Next 15  (?) -- { Days/Weeks/Months/Years }
    //obj.selectedNtimeOption = $('[id$=hdnvar3]').val();

    //What option by default to set in Custom Date  { 0 - (from) / 1 - (between) / 2 - (prior) }
    obj.selectedCustomRadioOption = $('[id$=hdnvar4]').val();               //unsure

    obj.applyCallback = function () {


        var modifiedText = smdatepickercontrol.getResponse($("#smdd_0"));
        var htmlString = "";
        var prepString = "";

        var selectedEndDate = modifiedText.selectedEndDate;
        var selectedStartDate = modifiedText.selectedStartDate;
        var selectedText = modifiedText.selectedText;
        var selectedDateOption = modifiedText.selDateOption;
        var selectedRadioOption = modifiedText.selRadioOption;

        if (selectedStartDate != null) {
            selectedStartDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(selectedStartDate), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));

        } if (selectedEndDate != null) {
            selectedEndDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(selectedEndDate), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));

        }

        if (selectedText != undefined) {

            // Set your values in respective IDs 
            //if (selectedStartDate || selectedEndDate)
            //    srmDQMStatus.init();
            var htmlString = selectedStartDate;
            $('[id$=DqmDatesFilterText]').text(htmlString);
            //srmDQMStatus.init();
            $('#smdd_0').hide();   //unsure?
            srmDQMStatus.refresh();
            
        

        }
        return false;
    }
    obj.ready = function (e) {

        //Chnage the Drop down to smselect ;  

        //changeDropDown($('#ddllastNtime_0'), false);
        //changeDropDown($('#ddlnextNtime_0'), false);
        

    }
    smdatepickercontrol.init(obj);
}
//////////////////////////////////////////////////////
//CODE FOR SMDatePickerControl FOR FIltring on Dates//
//////////////////////////////////////////////////////
function initializeDivFilterDate2() {
    /*	
        Date Options

        0: "Today",
        1: "Since Yesterday",
        3: "Anytime",
        2: "This Week",
        4: "Custom Date",
        5: "Last Days",
        6: "Next Days" 
 
	    Calender Types 

	    0: 2 calenders
	    1: 1 calender
	    2: no calender
	
   	    Id of calender is 'smdd_#' 
	    Id of dropdowns 'ddlnextNtime_#' , 'ddllastNtime_#'
	    { # starts with 0 }

    */
    var obj = {};
    obj.dateOptions = [0, 1, 2, 3, 4];      //Which options to be shown on calender
    obj.dateFormat = 'd/m/Y';
    obj.timePicker = false;
    obj.lefttimePicker = false;
    obj.righttimePicker = false;
    obj.calenderType = 0;
    obj.calenderID = 'smdd_0';
    obj.pivotElement = $('#DqmDatesFilter');    //Calender will be displayed when clicked on this div

    obj.StartDateCalender = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
    obj.EndDateCalender = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));

    obj.selectedTopOption = 0; // Which Dateoption to select by default

    obj.selectedCustomRadioOption = 0;
    obj.applyCallback = function () {
        

        var modifiedText = smdatepickercontrol.getResponse($("#smdd_0"));
        var htmlString = "";
        var prepString = "";

        var selectedEndDate = modifiedText.selectedEndDate;
        var selectedStartDate = modifiedText.selectedStartDate;
        var selectedText = modifiedText.selectedText;
        var selectedDateOption = modifiedText.selDateOption;
        var selectedRadioOption = modifiedText.selRadioOption;

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
                htmlString = "Since Yesterday";
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
            $('[id$=DqmDatesFilterText]').text(htmlString);
            //   SMSDownstreamSystemStatusMethods.prototype._controls.HdnExceptionDate().val(selectedDateOption);
            $('[id*=TextStartDate]').val(selectedStartDate);
            $('[id*=TextEndDate]').val(selectedEndDate);
            
        }
        
        var errormsg = validateDates();
        return errormsg
    }

    obj.ready = function (e) {
    }
    smdatepickercontrol.init(obj);
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
                errormsg = CompareDateFromTodaysDateUS(SMSCommonStatusMethods.prototype._controls.TxtStartDate().val(), 'Start Date');
                break;
            case '1':
                errormsg = CompareDateFromTodaysDateUS(SMSCommonStatusMethods.prototype._controls.TxtStartDate().val(), 'Start Date');
                if (errormsg == '')
                    errormsg = CompareDateFromTodaysDateUS(SMSCommonStatusMethods.prototype._controls.TxtEndDate().val(), 'End Date');
                if (errormsg == '')
                    errormsg = errormsg = CompareDateUS(SMSCommonStatusMethods.prototype._controls.TxtStartDate().val(), SMSCommonStatusMethods.prototype._controls.TxtEndDate().val());
                break;
            case '2':
                errormsg = CompareDateFromTodaysDateUS(SMSCommonStatusMethods.prototype._controls.TxtEndDate().val(), 'End Date');
                break;
        }
    }
    return errormsg;
}





//Function to set path for a AJAX call hit for the demo service written
function setPath() {
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });
    path = path + "/BaseUserControls/Service/CommonService.svc";
    //console.log(path);
    return path;
}





//after the html has loaded
/*
$(document).ready(function () {
    srmDQMStatus.init();

});
*/

function Initializer() {
    initializeDivFilterDate();
    srmDQMStatus.init();
    
   
}