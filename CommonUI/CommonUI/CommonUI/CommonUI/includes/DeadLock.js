$(document).ready(function () {
   // $('#myPopUp').hide();
    init();
    //initTableWithData();
    graphDisplay();
    downLoadFile();

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
//function initTableWithData() {
function graphDisplay() {
    $('.graphDisplay').click(function () {
        clearGraph();
        $("#downloadIcon").css('display', 'block');
        //var dd = table.row($(this).closest('td')).data();
        var dd = this.innerText;
        var xml = $(this).attr('data-xmldata');
        console.log(dd);
        $('#downloadIcon').attr('data-date', dd);
        $('#downloadIcon').attr('data-xmldata', xml);
        var param = {};
        param.dateFormat = "dd/MM/yyyy HH:mm:ss.fff";
        //param.deadLockTime = dd.DeadLockTimeStamp;
        param.deadLockTime = dd;
        CallCommonServiceMethod('GetDeadLockGraphData', param, OnSuccess_GraphClick, OnFailure_graph, null, false);
    });
}

function downLoadFile() {
    $('.downloadDeadlockXml').click(function () {
        //var dd = table.row($(this).closest('td')).data();
        var dd = $('#downloadIcon').attr('data-date');
        var xml = $('#downloadIcon').attr('data-xmldata');
        console.log(dd);
        var param = {};
        //param.DeadLockTimeStamp = dd.DeadLockTimeStamp;
        //param.DeadLockXMLData = dd.DeadLockXMLData;
        param.DeadLockTimeStamp = dd;
        param.DeadLockXMLData = xml;
        CallCommonServiceMethod('DownloadDeadLockXML', param, OnSuccess_downloadfile, OnFailure_downloadfile, null, false);
    });
}
//}

function clearGraph() {
    //$('#myPopUp').hide();
    $('#divCanvasContainer').html('');
    $('.graph-info-icon').remove();
    $('#divCanvasContainer').html('<canvas id="myCanvas" style="z-index: 1" width="1500" height="550">');
    $("#downloadIcon").css('display', 'none');
}

function OnSuccess_downloadfile(data) {
    var filePath = data.d;
    console.log(filePath);
    downloadFileInIframe(filePath);
}

function downloadFileInIframe(filePath) {
    $("#downloaddeadlockfile-iframe").attr('src', path + "/SRMFileUpload.aspx?fileonserver=true&fullPath=" + filePath + "");
}
function OnFailure_downloadfile(data) {
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
    console.log(jsonDate);
    refreshGridData();
}
function refreshGridData() {
    UpdateGridData();
}

function UpdateGridData() {
    var param = {};
    param.dateFormat = "dd/MM/yyyy";
    param.resultDateTimeFormat = "dd/MM/yyyy HH:mm:ss.fff";
    param.startDate = dateFilterValues.startDate;
    param.endDate = dateFilterValues.endDate;
    CallCommonServiceMethod('GetDeadLockData', param, OnSuccess, OnFailure, null, false);
}


function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
    callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
}


function OnSuccess(result) {
    if (result.d != "") {
        dataGrid = JSON.parse(result.d);
        $("#tabstrip").empty();

        var htmlContent = "<ul class='nav nav-tabs' id='myTab' role='tablist'>";
        var DateTimestamp = "";
        var DeadLockXMLData = "";
        $.each(dataGrid, function (index, value) {
            if (index == 0) {
                DateTimestamp = value.DeadLockTimeStamp;
                DeadLockXMLData = value.DeadLockXMLData;
                htmlContent += " <li class='nav-item'>  <a class='nav-link graphDisplay' data-xmldata='" + value.DeadLockXMLData + "' id='tabDate' data-toggle='tab' role='tab' aria-selected='true'>" + value.DeadLockTimeStamp + "</a> </li>";
            }
            else {
                htmlContent += " <li class='nav-item'>  <a class='nav-link  graphDisplay ' data-xmldata='" + value.DeadLockXMLData + "' id='tabDate'  data-toggle='tab' role='tab' aria-selected='false'>" + value.DeadLockTimeStamp + "</a> </li>";
            }
        });
        htmlContent += "</ul>";
        $("#tabstrip").append(htmlContent);
        $(".nav-tabs a:first").tab('show');
        document.getElementById("myTab").click();

        $('#downloadIcon').attr('data-date', DateTimestamp);
        $('#downloadIcon').attr('data-xmldata', DeadLockXMLData);

        var param = {};
        param.dateFormat = "dd/MM/yyyy HH:mm:ss.fff";
        param.deadLockTime = DateTimestamp;
        CallCommonServiceMethod('GetDeadLockGraphData', param, OnSuccess_GraphClick, OnFailure_graph, null, false);


        // initTableWithData();
        graphDisplay();
        console.log(dataGrid);
        $('#noDataContainer').hide();
        clearGraph();
    }
    else {
        $('#noDataContainer').show();
        $("#tabstrip").empty();
        $("#downloadIcon").css('display', 'none');

        $('#divCanvasContainer').html('');
        $('.graph-info-icon').remove();
        $('#divCanvasContainer').html('<canvas id="myCanvas" style="z-index: 1" width="1500" height="550">');
    }
}
function OnFailure(result) {
    console.log(result);
}
function OnSuccess_GraphClick(data) {
    querydata = JSON.parse(data.d);
    console.log(querydata);
    //$('#myPopUp').show();
    //$("#overlay").css('display', 'block');

   $("#downloadIcon").css('display', 'block');
    downLoadFile();
    GenerateGraph();
}
function OnFailure_graph(data) {
    console.log(data);
}

function GetInfoIconButton(top, left, dataContent) {
    return '<a tabindex="0" class="btn graph-info-icon"' +
        ' role="button" data-html="true" data-toggle="popover" data-trigger="hover" title="More Info"' +
        ' data-content="' + dataContent + '"' +
        ' style="z-index: 2; position: absolute; top:' + top + 'px;left:' + left + 'px">' +
        '<i class="fa fa-info" style="color:gray"></i></a >';
}

function DrawArrow(p1, p2, size) {
    var angle = Math.atan2((p2.y - p1.y), (p2.x - p1.x));
    var hyp = Math.sqrt((p2.x - p1.x) * (p2.x - p1.x) + (p2.y - p1.y) * (p2.y - p1.y));
    var canvas = document.getElementById('myCanvas');
    var ctx = canvas.getContext('2d');
    ctx.save();
    ctx.translate(p1.x, p1.y);
    ctx.rotate(angle);

    ctx.beginPath();
    ctx.moveTo(0, 0);
    ctx.lineTo(hyp - size, 0);
    ctx.stroke();

    ctx.fillStyle = 'grey';
    ctx.beginPath();
    ctx.lineTo(hyp - size, size);
    ctx.lineTo(hyp, 0);
    ctx.lineTo(hyp - size, -size);
    ctx.fill();
    ctx.restore();
}
function GetRectangleMoreInfoDataContent(resource) {
    return 'ObjectName: ' + resource.ObjectName +
        '<br/>IndexName: ' + resource.IndexName +
        '<br/>AssociatedObjectId: ' + resource.AssociatedObjectId;
}
function GetEclipseMoreInfoDataContent(process) {
    return 'Statement: ' + process.InputBuff;
}
function GenerateGraph() {
    if (querydata && querydata.Processes && querydata.Resources) {
        var process = querydata.Processes;//Process List
        var resources = querydata.Resources;//Resource List
        if (process.length > 1 && resources.length > 1) {
            var canvas = document.getElementById('myCanvas');
            var context = canvas.getContext('2d');
            //Two deadlock graph
            if ((process.length == 2 && resources.length == 2)) {
                var moreDetailControlContainer = $('#FirstGraph');
                $(moreDetailControlContainer).html('');

                context.scale(2, 1.4);
                context.beginPath();
                context.rect(200, 10, 130, 90);//Top Rectangle
                context.rect(200, 230, 130, 90);//Bottom Rectangle
                context.moveTo(200, 30);//Top Rectangle Line
                context.lineTo(330, 30);
                context.moveTo(200, 250);//Bottom Recatngle Line
                context.lineTo(330, 250);
                context.stroke();

                //Top Rectangle Fill Text
                context.font = "5.5px Arial";
                context.textAlign = "center";
                context.fillText("Key Lock", 260, 20);
                context.fillText("HobtId: " + resources[0].HobtId, 260, 50);
                context.fillText("AssociatedObjectId: " + resources[0].AssociatedObjectId, 260, 60);
                context.fillText("ObjectName: " + resources[0].ObjectName, 260, 70);
                context.fillText("IndexName: " + resources[0].IndexName, 265, 80);

                //add info button
                //--$(moreDetailControlContainer).append(GetInfoIconButton(95, 620, GetRectangleMoreInfoDataContent(resources[0])));

                //Bottom Rectangle fill Text
                context.font = "5.5px Arial";
                context.textAlign = "center";
                context.fillText("Key Lock", 260, 240);
                context.fillText("HobtId: " + resources[1].HobtId, 260, 270);
                context.fillText("AssociatedObjectId: " + resources[1].AssociatedObjectId, 260, 280);
                context.fillText("ObjectName: " + resources[1].ObjectName, 260, 290);
                context.fillText("IndexName: " + resources[1].IndexName, 265, 300);

                //add info button
                //--$(moreDetailControlContainer).append(GetInfoIconButton(315, 620, GetRectangleMoreInfoDataContent(resources[1])));

                /*Eclipse Left*/
                context.beginPath();
                context.ellipse(100, 150, 100, 70, 0, 0, Math.PI * 2);
                context.stroke();
                context.font = "8px Arial";
                context.textAlign = "center";
                context.fillText("ServerProcessId:" + process[0].Spid, 90, 125);
                context.fillText("ServerBatchId:" + process[0].Sbid, 90, 135);
                context.fillText("ExecutionContextId:" + process[0].Ecid, 90, 145);
                context.fillText("TransactionDescriptor:" + process[0].Xdes, 90, 155);
                context.fillText("DeadLockPriority:" + process[0].Priority, 90, 165);
                context.fillText("OwnerId:" + process[0].OwnerId, 90, 175);
                context.fillText("LogUsed:" + process[0].LogUsed, 90, 185);
                //add info button
                $(moreDetailControlContainer).append(GetInfoIconButton(270, 340, GetEclipseMoreInfoDataContent(process[0])));

                //Eclipse Right
                context.beginPath();
                context.ellipse(450, 150, 100, 70, 0, 0, Math.PI * 2);
                context.stroke();
                context.font = "8px Arial";
                context.textAlign = "center";
                context.fillText("ServerProcessId:" + process[1].Spid, 440, 125);
                context.fillText("ServerBatchId:" + process[1].Sbid, 440, 135);
                context.fillText("ExecutionContextId:" + process[1].Ecid, 440, 145);
                context.fillText("TransactionDescriptor:" + process[1].Xdes, 440, 155);
                context.fillText("DeadLockPriority:" + process[1].Priority, 440, 165);
                context.fillText("OwnerId:" + process[1].OwnerId, 440, 175);
                context.fillText("LogUsed:" + process[1].LogUsed, 440, 185);

                //add info button
                $(moreDetailControlContainer).append(GetInfoIconButton(270, 1040, GetEclipseMoreInfoDataContent(process[1])));

                //Arrows
                DrawArrow({ x: 330, y: 20 }, { x: 420, y: 85 }, 10);//Right Top Arrow
                DrawArrow({ x: 200, y: 290 }, { x: 40, y: 210 }, 10);//Left Bottom Arrow
                DrawArrow({ x: 60, y: 85 }, { x: 200, y: 30 }, 10);//Left Top Arrow
                DrawArrow({ x: 490, y: 215 }, { x: 330, y: 290 }, 10);//Right Bottom Arrow


                //cross Line and Arrow Data
                context.beginPath();
                context.moveTo(190, 180);
                context.lineTo(10, 120);
                context.moveTo(180, 110);
                context.lineTo(10, 180);
                context.strokeStyle = '#1a53ff80';//'rgba(100,100,100,0.5)';
                context.stroke();
                context.font = "8px Arial";
                context.textAlign = "center";
                context.fillText("RequestMode: " + process[0].LockMode, 90, 65);
                context.fillText("OwnerMode: " + resources[1].Mode, 110, 260);
                context.fillText("RequestMode: " + process[1].LockMode, 390, 270);
                context.fillText("OwnerMode: " + resources[0].Mode, 390, 50);
            }
            //Three deadlock Graph
            else if (process.length == 3 && resources.length == 3) {
                var moreDetailControlContainer = $('#SecondGraph');
                $(moreDetailControlContainer).html('');

                context.scale(1.8, 1.7);
                context.beginPath();
                context.ellipse(70, 150, 70, 40, 0, 0, Math.PI * 2);//Left Eclipse
                context.rect(130, 20, 120, 70);//Top middle Rectangle
                context.rect(130, 230, 120, 70);//Bottom Rectangle
                context.rect(500, 110, 120, 70);//Right Rectangle
                context.moveTo(130, 40);//Top Rectangle Line
                context.lineTo(250, 40);
                context.moveTo(130, 250);//Bottom Recatngle Line
                context.lineTo(250, 250);
                context.moveTo(500, 130);//Right Rectangle Line
                context.lineTo(620, 130);
                context.stroke();



                //Eclipse Left FillText
                context.font = "6px Arial";
                context.textAlign = "center";
                //context.fillText("LockMode:" + process[0].LockMode, 60, 125);
                //context.fillText("ProcessId:" + process[0].ProcessId, 70, 135);
                //context.fillText("TransactionName:" + process[0].TransactionName, 70, 145);
                //context.fillText("IsolationLevel:" + process[0].IsolationLevel, 70, 155);
                //context.fillText("Xdes:" + process[0].Xdes, 60, 165);

                context.fillText("ServerProcessId:" + process[0].Spid, 70, 125);
                context.fillText("ServerBatchId:" + process[0].Sbid, 60, 135);
                context.fillText("ExecutionContextId:" + process[0].Ecid, 60, 145);
                context.fillText("TransactionDescriptor:" + process[0].Xdes, 70, 155);
                context.fillText("DeadLockPriority:" + process[0].Priority, 70, 165);
                context.fillText("OwnerId:" + process[0].OwnerId, 60, 175);
                context.fillText("LogUsed:" + process[0].LogUsed, 70, 185);


                //Add info button
                $(moreDetailControlContainer).append(GetInfoIconButton(310, 200, GetEclipseMoreInfoDataContent(process[0])));

                //Top rectangle Fill Text
                context.font = "5px Arial";
                context.textAlign = "center";
                context.fillText("Key Lock", 180, 30);
                context.fillText("HobtId: " + resources[0].HobtId, 190, 50);
                context.fillText("AssociatedObjectId: " + resources[0].AssociatedObjectId, 190, 60);
                context.fillText("ObjectName: " + resources[0].ObjectName, 190, 70);
                context.fillText("IndexName: " + resources[0].IndexName, 190, 80);
                //context.fillText("Id: " + resources[0].Id, 190, 60);
                //context.fillText("WaiterId: " + resources[0].ProcessIdwaiter, 190, 70);

                //add info
                //-- $(moreDetailControlContainer).append(GetInfoIconButton(145, 410, GetRectangleMoreInfoDataContent(resources[0])));

                //Bottom Rectangle fill Text
                context.font = "5px Arial";
                context.textAlign = "center";
                context.fillText("Key Lock", 180, 240);
                context.fillText("HobtId: " + resources[1].HobtId, 190, 260);
                context.fillText("AssociatedObjectId: " + resources[1].AssociatedObjectId, 190, 270);
                context.fillText("ObjectName: " + resources[1].ObjectName, 190, 280);
                context.fillText("IndexName: " + resources[1].IndexName, 190, 290);

                //context.fillText("Id: " + resources[1].Id, 190, 270);
                //context.fillText("WaiterId: " + resources[1].ProcessIdwaiter, 190, 280);

                //add info
                //--$(moreDetailControlContainer).append(GetInfoIconButton(500, 410, GetRectangleMoreInfoDataContent(resources[1])));

                //Right top Eclipse Fill text
                context.beginPath();
                context.ellipse(380, 230, 70, 40, 0, 0, Math.PI * 2);
                context.stroke();
                context.font = "6px Arial";
                context.textAlign = "center";
                //context.fillText("LockMode:" + process[1].LockMode, 380, 60);
                //context.fillText("ProcessId: " + process[1].ProcessId, 380, 70);
                //context.fillText("TransactionName: " + process[1].TransactionName, 380, 80);
                //context.fillText("IsolationLevel: " + process[1].IsolationLevel, 380, 90);
                //context.fillText("Xdes: " + process[1].Xdes, 380, 100);

                context.fillText("ServerProcessId:" + process[1].Spid, 380, 55);
                context.fillText("ServerBatchId:" + process[1].Sbid, 380, 65);
                context.fillText("ExecutionContextId:" + process[1].Ecid, 380, 75);
                context.fillText("TransactionDescriptor:" + process[1].Xdes, 380, 85);
                context.fillText("DeadLockPriority:" + process[1].Priority, 380, 95);
                context.fillText("OwnerId:" + process[1].OwnerId, 380, 105);
                context.fillText("LogUsed:" + process[1].LogUsed, 380, 115);

                //add info
                $(moreDetailControlContainer).append(GetInfoIconButton(195, 750, GetEclipseMoreInfoDataContent(process[1])));

                //Right Bottom Eclipse Fill Text
                context.beginPath();
                context.ellipse(380, 80, 70, 40, 0, 0, Math.PI * 2);
                context.stroke();
                context.font = "6px Arial";
                context.textAlign = "center";
                //context.fillText("LockMode:" + process[2].LockMode, 380, 210);
                //context.fillText("ProcessId: " + process[2].ProcessId, 380, 220);
                //context.fillText("TransactionName: " + process[2].TransactionName, 380, 230);
                //context.fillText("IsolationLevel: " + process[2].IsolationLevel, 380, 240);
                //context.fillText("Xdes: " + process[2].Xdes, 380, 250);

                context.fillText("ServerProcessId:" + process[2].Spid, 380, 205);
                context.fillText("ServerBatchId:" + process[2].Sbid, 380, 215);
                context.fillText("ExecutionContextId:" + process[2].Ecid, 380, 225);
                context.fillText("TransactionDescriptor:" + process[2].Xdes, 380, 235);
                context.fillText("DeadLockPriority:" + process[2].Priority, 380, 245);
                context.fillText("OwnerId:" + process[2].OwnerId, 380, 255);
                context.fillText("LogUsed:" + process[2].LogUsed, 380, 265);

                //add info
                $(moreDetailControlContainer).append(GetInfoIconButton(450, 750, GetEclipseMoreInfoDataContent(process[2])));

                //Arrows
                DrawArrow({ x: 250, y: 30 }, { x: 320, y: 60 }, 10);
                DrawArrow({ x: 450, y: 80 }, { x: 500, y: 110 }, 10);
                DrawArrow({ x: 130, y: 250 }, { x: 40, y: 190 }, 10);
                DrawArrow({ x: 10, y: 130 }, { x: 130, y: 40 }, 10);
                DrawArrow({ x: 320, y: 250 }, { x: 250, y: 280 }, 10);
                DrawArrow({ x: 550, y: 180 }, { x: 450, y: 235 }, 10);

                //Right Rectangle Fill Text
                context.beginPath();
                context.font = "5px Arial";
                context.textAlign = "center";
                context.fillText("Key Lock", 550, 120);
                context.fillText("HobtId: " + resources[2].HobtId, 560, 140);
                context.fillText("AssociatedObjectId: " + resources[2].AssociatedObjectId, 560, 150);
                context.fillText("ObjectName: " + resources[2].ObjectName, 560, 160);
                context.fillText("IndexName: " + resources[2].IndexName, 560, 170);

                //context.fillText("Id: " + resources[2].Id, 560, 150);
                //context.fillText("WaiterId: " + resources[2].ProcessIdwaiter, 560, 160);

                //add info
                //--$(moreDetailControlContainer).append(GetInfoIconButton(300, 950, GetRectangleMoreInfoDataContent(resources[2])));

                //cross Line and Arrow Data
                context.beginPath();
                context.moveTo(130, 170);
                context.lineTo(10, 130);
                context.moveTo(130, 130);
                context.lineTo(10, 170);
                context.strokeStyle = '#1a53ff80';//'rgba(100,100,100,0.5)';
                context.stroke();
                context.font = "8px Arial";
                context.textAlign = "center";
                context.fillText("RequestMode: " + process[0].LockMode, 90, 65);
                context.fillText("OwnerMode: " + resources[2].Mode, 90, 220);
                context.fillText("RequestMode: " + process[1].LockMode, 300, 270);
                context.fillText("OwnerMode: " + resources[1].Mode, 500, 220);
                context.fillText("RequestMode: " + process[2].LockMode, 490, 100);
                context.fillText("OwnerMode: " + resources[0].Mode, 290, 40);
            }
        }
        setTimeout(function () { $('.graph-info-icon').popover({ trigger: 'hover' }); }, 500);
    }
}