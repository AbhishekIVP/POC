$(document).ready(function () {
    init();
    initQueryTable();
    InitializeDatePicker();
    refreshGridData();
});

var dateFilterValues;
var dateFilterTextStatic = {};
dateFilterTextStatic[0] = "Today";
dateFilterTextStatic[1] = "Since Yesterday";
dateFilterTextStatic[2] = "This Week";
dateFilterTextStatic[3] = "Any Time";

function init() {
    var todayFilter = new Date().format("dd/MM/yyyy");
    var todayFilterServer = new Date().format("dd/MM/yyyy");
    var startDate = todayFilter;
    var endDate = todayFilter;
    var marked = '0';
    var checked = '0';
    var serverStartDate = todayFilterServer;
    var serverEndDate = todayFilterServer;

    dateFilterValues = { marked: marked, checked: checked, startDate: startDate, serverStartDate: serverStartDate, endDate: endDate, serverEndDate: serverEndDate };
}
var querydata = null;
var dataGrid = null;
function initQueryTable() {
    table = $('#expensiveQuery').DataTable({
        paging: false,
        searching: false,
        bDestroy: true,
        data: dataGrid,
        orderable: true,
       // scrollX: "100%",
        scrollX: "calc(80vw - 100px)",
        // scrollY:"95vw",
        scrollY: "calc(91vh - 100px)",
        //scrollY: "200px",
        scrollCollapse: true,
        scrollInfinite:true,
        //language: { "emptyTable": "No Data Found" },
            columns: [{
                'defaultContent': '<button type="button" id="btn" class="btn btn-sm showQueryText">QueryText</button>'
            },
            { 'data': 'ExecutionCount' },
            { 'data': 'CreationTime' },
            { 'data': 'LastExecutionTime' },
            { 'data': 'LastWorkerTime' },
            { 'data': 'MinWorkerTime' },
            { 'data': 'MaxWorkerTime' },
            { 'data': 'LastPhysicalReads' }, 
            { 'data': 'MinPhysicalReads' },
            { 'data': 'MaxPhysicalReads' },
            { 'data': 'LastLogicalWrites' },
            { 'data': 'MinLogicalWrites' },
            { 'data': 'MaxLogicalWrites' },
            { 'data': 'LastLogicalReads' },
            { 'data': 'MinLogicalReads' },
            { 'data': 'MaxLogicalReads' },
            { 'data': 'LastElapsedTime' },
            { 'data': 'MinElapsedTime' },
            { 'data': 'MaxElapsedTime' },
            {
                'defaultContent': '<div id="downloadIcon" class="downloadFile"></div>'
            }
        ]

    });

    $('.showQueryText').click(function () {
        var dd = table.row($(this).closest('td')).data();
        var param = {};
        param.Id = dd.Id;
        CallCommonServiceMethod('GetQueryTextData', param, OnSuccess_queryClick, OnFailure_queryClick, null, false);
    });

    $('.downloadFile').click(function () {
        var dd = table.row($(this).closest('td')).data();
        console.log(dd);
        var param = {};
        param.Id = dd.Id;
        param.QueryPlan = dd.QueryPlan;
        param.LastExecutionTime = dd.LastExecutionTime;
        CallCommonServiceMethod('DownloadQueryPlanFile', param, OnSuccess_download, OnFailure_download, null, false);
        return false;
    });
}

function OnSuccess_download(data) {
    var filePath = data.d;
    downloadFileInIframe(filePath);
}

function downloadFileInIframe(filePath) {
    $("#expensivequery-iframe").attr('src', path + "/SRMFileUpload.aspx?fileonserver=true&fullPath=" + filePath + "");
}
function OnFailure_download(data) {
    console.log(data);
}
//function ShowQueryPlanXML(res) {
//    $('#divXmlContainer').text(res);
//}

//function FormatXml(data) {
//    var xmlDoc = new DOMParser().parseFromString(data, 'application/xml');
//    var xsltDoc = new DOMParser().parseFromString([
//        // describes how we want to modify the XML - indent everything
//        '<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform">',
//        '  <xsl:strip-space elements="*"/>',
//        '  <xsl:template match="para[content-style][not(text())]">', // change to just text() to strip space in text nodes
//        '    <xsl:value-of select="normalize-space(.)"/>',
//        '  </xsl:template>',
//        '  <xsl:template match="node()|@*">',
//        '    <xsl:copy><xsl:apply-templates select="node()|@*"/></xsl:copy>',
//        '  </xsl:template>',
//        '  <xsl:output indent="yes"/>',
//        '</xsl:stylesheet>',
//    ].join('\n'), 'application/xml');

//    var xsltProcessor = new XSLTProcessor();
//    xsltProcessor.importStylesheet(xsltDoc);
//    var resultDoc = xsltProcessor.transformToDocument(xmlDoc);
//    var resultXml = new XMLSerializer().serializeToString(resultDoc);
//    ShowQueryPlanXML(resultXml);
//    //return resultXml;
//    //document.getElementById('divXmlContainer').innerHTML = resultXml;

//   // return '$("#divXmlContainer").text(' + resultXml+')';
//}


function OnSuccess_queryClick(data) {
    querydata = JSON.parse(data.d);
    $('#myModal').modal();
    $('#divXmlContainer').text(querydata[0].QueryText);
}
function OnFailure_queryClick(data) {
    console.log(data);
}

function InitializeDatePicker() {
    var obj = {};

    obj.dateOptions = [0, 1, 2, 3, 4];
    obj.dateFormat = 'd/m/Y';
    obj.lefttimePicker = false;
    obj.righttimePicker = false;
    obj.calenderType = 0;
    obj.pivotElement = $('.dateFilterPopUp'); //Calender will be displayed when clicked on this div

    //Which date to set in calender   
    obj.StartDateCalender = dateFilterValues.startDate;
    obj.EndDateCalender = dateFilterValues.endDate;

    //Which Dateoption to select by default
    obj.selectedTopOption = dateFilterValues.marked;

    //What option by default to set in Custom Date  { 0 - (from) / 1 - (between) / 2 - (prior) }
    obj.selectedCustomRadioOption = dateFilterValues.checked;

    obj.applyCallback = function () {
        var modifiedText = smdatepickercontrol.getResponse($("#" + srmDatePickerId));

        var dateSettings = {};
        var selectedEndDate = modifiedText.selectedEndDate;
        var selectedStartDate = modifiedText.selectedStartDate;
        var serverEndDate = modifiedText.serverEndDateOriginal;
        var serverStartDate = modifiedText.serverStartDateOriginal;
        var selectedText = modifiedText.selectedText;

        if (selectedStartDate != null) {
            selectedStartDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(selectedStartDate), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
        }
        if (selectedEndDate != null) {
            selectedEndDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(selectedEndDate), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
        }

        if (typeof selectedText !== "undefined") {
            // Set your values in respective IDs
        }
        dateSettings.startDate = selectedStartDate;
        dateSettings.endDate = selectedEndDate;
        dateSettings.serverStartDate = serverStartDate;
        dateSettings.serverEndDate = serverEndDate;
        dateSettings.marked = modifiedText.selDateOption;
        dateSettings.checked = modifiedText.selRadioOption;

        var errorMsg = validateDates(dateSettings);
        if (errorMsg.length > 0)
            return errorMsg;
        dateFilterValues = dateSettings;
        UpdateDateFilter();
        return false;
    }
    obj.ready = function (e) {
        srmDatePickerId = e[0].calenderID;
    }
    smdatepickercontrol.init(obj);

    $(".dateFilterPopUp").click(function (e) {
        var objDate = dateFilterValues;

        var obj = {};
        obj.pivotElement = $('.dateFilterPopUp');
        obj.calenderID = srmDatePickerId;
        obj.selectedTopOption = objDate.marked;
        obj.selectedCustomRadioOption = objDate.checked;
        obj.StartDateCalender = objDate.startDate;
        obj.EndDateCalender = objDate.endDate;
        smdatepickercontrol.SetValuesInCalender(obj);
    });
}

function validateDates(dateSettings) {
    var errormsg = '';
    if (dateSettings.marked.toString() == '4') {
        switch (dateSettings.checked.toString()) {
            case '0': //From
                errormsg = CompareDateFromTodaysDateUS(dateSettings.serverStartDate, 'Start Date ');
                break;
            case '1':
                errormsg = CompareDateFromTodaysDateUS(dateSettings.serverStartDate, 'Start Date ');
                if (errormsg == '')
                    errormsg = CompareDateFromTodaysDateUS(dateSettings.serverEndDate, 'End Date ');
                if (errormsg == '')
                    errormsg = CompareDateUS(dateSettings.serverStartDate, dateSettings.serverEndDate);
                break;
            case '2':
                errormsg = CompareDateFromTodaysDateUS(dateSettings.serverEndDate, 'End Date ');
                break;
        }
    }
    return errormsg;
}

function GetDateFilterValuesForUpdate(objDate) {
    var dateFilterData = {};
    if (objDate.marked == '4') {
        if (objDate.checked == '0') {
            dateFilterData.text = 'From ' + objDate.startDate;
        }
        else if (objDate.checked == '1') {
            dateFilterData.text = objDate.startDate + ' - ' + objDate.endDate;
        }
        else if (objDate.checked == '2') {
            dateFilterData.text = 'Prior To ' + objDate.endDate;
        }
    }
    else
        dateFilterData.text = dateFilterTextStatic[parseInt(objDate.marked)];
    return dateFilterData;
}

function UpdateDateFilter() {
    var objDate = dateFilterValues;
    var dateFilterData = GetDateFilterValuesForUpdate(objDate);
    $(".dateFilterText").html(dateFilterData.text);

    var jsonDate = JSON.stringify(objDate);
    refreshGridData();
}
function refreshGridData() {
    UpdateGridQueryData();
}

function UpdateGridQueryData() {
    var param = {};
    param.dateFormat = "dd/MM/yyyy";
    param.resultDateTimeFormat = "dd/MM/yyyy HH:mm:ss.fff";
    param.startDate = dateFilterValues.startDate;
    param.endDate = dateFilterValues.endDate;
    CallCommonServiceMethod('GetExpensiveQuery', param, OnSuccess, OnFailure, null, false);
}


function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
    callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
}

function OnSuccess(result) {
    if (result.d != "") {
        dataGrid = JSON.parse(result.d);
        $('#noQuery').hide();
        initQueryTable();
    }
    else {
        $('.dataTables_wrapper').css('display', 'none');
        $('#noQuery').show();
    }
}
function OnFailure(result) {
    console.log(result);
}