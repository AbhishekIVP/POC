//gridobject
var gridObj = {
    "SelectRecordsAcrossAllPages": true,
    "ViewKey": "", //to be overriden
    "CacheGriddata": true,
    "CurrentPageId": "",
    "SessionIdentifier": "",
    "UserId": "",
    "GridId": "", //to be overriden
    "CheckBoxInfo": {},
    "Height": "450px",
    "ColumnsWithoutClientSideFunctionality": [],
    "ColumnsNotToSum": [],
    "ColumnsToHide": [],
    "CheckBoxInfo":null,
    "RequireEditGrid": false,
    "RequireEditableRow": false,
    "IdColumnName": "api_call_id", //to be overriden
    "TableName": "DBLocks",
    "PageSize": 100,
    "RequirePaging": false,
    "RequireInfiniteScroll": false,
    "CollapseAllGroupHeader": false,
    "GridTheme": 2,
    "HideFooter": false,
    "DoNotExpand": false,
    "ItemText": "",
    "DoNotRearrangeColumn": true,
    "RequireGrouping": true,
    "RequireMathematicalOperations": false,
    "RequireSelectedRows": false,
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
};



var dbLocks = (function () {
    function DBLocks() {

    }

    var _selectedStartDate;
    var _selectedEndDate;

    function GetGUID() {
        var objDate = new Date();
        var str = objDate.toString();
        str = objDate.getDate().toString() + objDate.getMonth().toString() + objDate.getFullYear().toString() + objDate.getHours().toString() + objDate.getMinutes().toString() + objDate.getSeconds().toString() + objDate.getMilliseconds().toString() + eval('Math.round(Math.random() * 10090)').toString();
        return str;
    }

    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    var dbLocks = new DBLocks();

    DBLocks.prototype.init = function init() {
        //direct ajax hit
        //Load SMDatePickerControl
        initializeDivFilterDate();
        callNeoGrid();
        //apply click binding
        $("#DBRefreshButton").off('click').on('click', () => { callNeoGrid(); });
        //$("#DBLocksApplyButton").click();

    }

    function callNeoGrid() {
        onServiceUpdating();
        var currPageID = "s" + GetGUID();
        var viewKey = "s" + GetGUID();
        var sessionID = "s" + GetGUID();
        var tempObj = $.extend({}, gridObj);
        var customRowInfo = "";
        tempObj = initializeGridProperties(tempObj, customRowInfo, currPageID, viewKey, sessionID);


        $.ajax({
            type: 'POST',
            url: path + "/BaseUserControls/Service/CommonService.svc/GetDBLocksData",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({
                divID: "DBLocksNeoGrid",
                currPageID: currPageID,
                viewKey: viewKey,
                sessionID: sessionID,
                jsonGridInfo: JSON.stringify(tempObj),
                startDate: _selectedStartDate,
                endDate: _selectedEndDate
            }),
            success: function (data) {
                var data = data.d;
                if (parseInt(data.rowCount) > 0) {
                    $("#DBLocksNoGridSection").hide();
                    $("#DBLocksNeoGrid").show();
                    xlgridloader.create("DBLocksNeoGrid", "Test", tempObj, "");
                    $("#DBLocksNeoGrid_MessageBoxID").css('min-height', 0);
                    onGridRenderComplete();

                    //logic for top row showing current lock
                    populateCurrentLockRow(data);
                }
                else {
                    $("#DBLocksNeoGrid").hide();
                    $("#DBLocksNoGridSection").show();
                    if (parent.leftMenu != null)
                        parent.leftMenu.showNoRecordsMsg('No Records to display.', $(".DBLocksNoGridSection"));
                    else {
                        $("#DBLocksNoGridSection").hide();
                        $("#DBLocksNeoGrid").show();
                        xlgridloader.create("DBLocksNeoGrid", "Test", tempObj, "");
                        $("#DBLocksNeoGrid_MessageBoxID").css('min-height', 0);
                    }
                    $(".DBLocksNoGridSection").width("100%");
                }
                onServiceUpdated();
         },
            error: function (err) {
                onServiceUpdated();
                console.log(err);
            }
        });
    }

    function populateCurrentLockRow(data) {
        if (data.ifLockExistsCurrently) {
            //set img source
            //debugger;
            var finalPath = "/css/images/icons/LockIcon.png";
            $("#DBLocksLockImage").attr('src',path + finalPath);

            $("#DBLocksCurrent").css('display', 'block');
            $("#DBLocksWhenCurrentLockAcquired").text(data.whenAcquired);
            $("#DBLocksWhenCurrentUser").text(data.userName);

            if (data.machineName != "")
                $("#DBLocksWhenCurrentMachineName").text("(" + data.machineName + ")");


            //lower grid


            $("#DBLocksCurrentPopup").css('display', 'block');
            $("#DBLocksFirstAttempt").text(data.firstAttempt);
            $("#DBLocksIdentifier").text(data.identifier);
            $("#DBLocksStackTrace").text(data.stackTrace.substr(0, 50) + "...");

            //making clickable and css
            $("#DBLocksStackTrace").css({'text-decoration':'underline','cursor':'pointer','color':'#48a3dd'});
            $("#DBLocksStackTrace").off('click').on('click', (event) => {
                displayPopup(data.stackTrace, "success");
            });

            $("#DBLocksProcessId").text(data.processId);
            $("#DBLocksProcessName").text(data.processName);

            // $("#DBLocksViewDetails")


        }
        else {
            $("#DBLocksCurrent").css('display', 'none');
            $(".DBLocksCurrent").text("");
        }
    }

    function onGridRenderComplete() {
        var grid = $find("DBLocksNeoGrid");

        if (grid != null) {
            this.onGridServiceCompleted = Function.createDelegate(this, applyPopupBindings);
            grid.eventHandlerManager.addServiceCompletedEventHandler(this.onGridServiceCompleted);
        }
    }

    function applyPopupBindings() {

        //apply click bindings
        $(".DataUploadClickablePopup").off('click').on('click', (event) => {
            displayPopup($(event.target).attr('message'), "success");
        });
    }

    function initializeGridProperties(tempObj, customRowInfo, currPageID, viewKey, sessionID) {
        tempObj.GridId = "DBLocksNeoGrid";
        tempObj.UserId = "admin";
        tempObj.ViewKey = viewKey;
        tempObj.CurrentPageId = currPageID;
        tempObj.SessionIdentifier = sessionID;
        tempObj.IdColumnName = "id";
        //tempObj.height = $(window).height() - 200;
        //}
        return tempObj;
    }

    ///////////////////////////////////////////////////////
    //CODE FOR SMDatePickerControl For Filtering on Dates//
    ///////////////////////////////////////////////////////
    function initializeDivFilterDate() {
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
        obj.pivotElement = $('#DBLocksDatesFilter');    //Calender will be displayed when clicked on this div

        obj.StartDateCalender = (new Date()).toLocaleDateString();
        obj.EndDateCalender = (new Date()).toLocaleDateString();

        obj.selectedTopOption = 0; // Which Dateoption to select by default

        _selectedStartDate = obj.StartDateCalender;   //Set to Default Value, i.e. Today
        _selectedEndDate = obj.EndDateCalender;       //Set to Default Value, i.e. Today

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
                selectedStartDate = (new Date(selectedStartDate)).toLocaleDateString();
                $('[id$=hdnStartDateCalender]').val(selectedStartDate);

                // Updating Object Variable
                _selectedStartDate = selectedStartDate;
                //
            }
            else {
                // Updating Object Variable
                _selectedStartDate = null;
            }

            if (selectedEndDate != null) {
                selectedEndDate = (new Date(selectedEndDate)).toLocaleDateString();
                $('[id$=hdnEndDateCalender]').val(selectedEndDate);

                // Updating Object Variable
                _selectedEndDate = selectedEndDate;
                //
            }
            else {
                // Updating Object Variable
                _selectedEndDate = null;
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
                $('[id$=DBLocksDatesFilterText]').text(htmlString);
                //   SMSDownstreamSystemStatusMethods.prototype._controls.HdnExceptionDate().val(selectedDateOption);
                $('[id*=TextStartDate]').val(selectedStartDate);
                $('[id*=TextEndDate]').val(selectedEndDate);
            }
            var errormsg = validateDates();
            if (!errormsg)
                callNeoGrid();
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

    function displayPopup(message, status) {
        $("#copySuccess").hide();
        var imageSrc = "";
        var borderTop = "";
        var title = "";
        var color = "";
        switch (status) {
            case "success":
                imageSrc = "images/icons/pass_icon.png";
                borderTop = "4px solid #ACD373";
                title = "Success";
                color = "#ACD373";
                break;
            case "failure":
                imageSrc = "images/icons/fail_icon.png";
                borderTop = "4px solid #C7898C";
                title = "Failure";
                color = "#C7898C";
                break;
            case "alert":
                imageSrc = "images/icons/alert_icon.png";
                borderTop = "4px solid #F4AD02";
                title = "Alert";
                color = "#F4AD02";
                break;
        }

        var left = ($(window).width() / 2) - 300;
        var msgPopup = $("#os_messagePopup");
        msgPopup.find(".draftspopupTitle").text(title).css("color", color);
        msgPopup.find(".DBLocks").text(message);
        msgPopup.find("img").attr("src", imageSrc);
        //msgPopup.css("border-top", borderTop);
        msgPopup.css("left", left + "px");
        msgPopup.css("top", "-" + msgPopup.height() + "px");
        msgPopup[0].style.display = "";
        msgPopup.animate({ top: ($(window).height() / 2) - msgPopup.height() + 20 }, 500);

        //click to copy
        $(".copyText").off('click').on('click', () =>
                {
            let str = $(".allowTextCopy").text();
            const e1 = document.createElement('textarea');
            e1.value = str;
            document.body.appendChild(e1);
            e1.select();
            document.execCommand('copy');
            document.body.removeChild(e1);
            $("#copySuccess").show();
            setTimeout(() => { $("#copySuccess").hide(); }, 5000);
        }

            );

        msgPopup.find(".draftsPopupCloseBtn").unbind("click").bind("click", function (e) {
            msgPopup.animate({ top: "-" + msgPopup.height() + "px" }, 500, function () {
                msgPopup[0].style.display = "none";
            });
        });
    }


    return dbLocks;
})();



$(document).ready(function () {

    dbLocks.init();
});