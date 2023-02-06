var syncStatus = (function () {
    var syncStatus;

    var dateFilterTextStatic = {};
    dateFilterTextStatic[0] = "Today";
    dateFilterTextStatic[1] = "Since Yesterday";
    dateFilterTextStatic[2] = "This Week";
    dateFilterTextStatic[3] = "Any Time";

    function SRMDownstreamSyncStatus() {
    }
    syncStatus = new SRMDownstreamSyncStatus();
    var data;
    var table;
    var downstreamSyncStatusInfo;
    var dateFilterValues;
    var srmDatePickerId;
    var isReady = false;
    SRMDownstreamSyncStatus.prototype.init = function (data) {
        downstreamSyncStatusInfo = eval("(" + data + ")");
        var todayFilter = new Date().format(downstreamSyncStatusInfo.RADShortDateFormat);
        var todayFilterServer = new Date().format(downstreamSyncStatusInfo.ServerShortDateFormat);

        var startDate = todayFilter;
        var endDate = todayFilter;
        var marked = '0';
        var checked = '0';
        var serverStartDate = todayFilterServer;
        var serverEndDate = todayFilterServer;

        dateFilterValues = { marked: marked, checked: checked, startDate: startDate, serverStartDate: serverStartDate, endDate: endDate, serverEndDate: serverEndDate };

        $(document).ready(function () {
            InitializeDatePicker();
            refreshGrid();
            isReady = true;
            $("#SMDownstreamRefreshButton").click(refreshGrid);
        });
        $(window).resize(resize);
    }
    function resize() {

        $(".dataTables_scrollBody").css("max-height", $("#dialogContainer").height() - 75);
        $(".dataTables_scrollBody>div:nth-child(2)").css("height", $("#dialogContainer").height() - 75);

        //$("#wrapper").css("min-height", $(window).height() + 5);
        console.log("resize");
    }

    function initTableWithData() {
        //var height = $(document).height() - $('.srm_panelSections:first').height() - $('.srm_panelSections:nth-of-type(3)').height();
        //height -= 66;
        //var data2;
        //if (data.length == 0) {
        //    data2 = [{
        //        EndTime: "",
        //        FailureMessage: "",
        //        SetupName: "",
        //        StartTime: "",
        //        Status: "",
        //        reports: null,
        //        setup_status_id: null
        //    }];
        //}
        //if (table == null && data.length == 0) {
        //    return "<div>hi</div>";
        //}

        if (table == null)
            table = $('#tasks').DataTable({
                processing: true,
                data: data.length == 0 ? data2 : data,
                //"bScrollInfinite": true,
                //"bScrollCollapse": true,
                // "sScrollY": "200px",
                ordering: false,
                scrollY: 200,
                scroller: { loadingIndicator: true },
                scrollCollapse: true,
                searching: true,
                deferRender: true,
                drawCallback: function (settings, json) {
                    resize();
                },
                "columns": [{
                    "className": 'status-control',
                    "orderable": false,
                    "data": "Status",
                    render: function (data, type, row) {
                            if (data.toLowerCase() == 'passed')
                                return '<div><i class="fa fa-check" style="background:#6DC49C"></i></div>';
                            else if (data.toLowerCase() == 'failed')
                                return '<div><i class="fa fa-close" style="background:#DF9592"></i></div>';
                            else if (data.toLowerCase() == 'inprogress')
                                return '<div><div class="inProgressContainer"><div></div></div></div>';
                            else if (data.toLowerCase() == 'queued')
                                return '<div><div class="inProgressContainer queuedContainer"><div></div></div></div>';
							else if (data.toLowerCase() == 'loader execution pending' || data.toLowerCase() == 'loader execution inprogress')
								return '<div class="loaderText">' + data + '</div>';
                            else
                                return '<div><i class="fa fa-check" style="background:#6d6d6d"></i></div>';
                        
                    },
                    searchable: false,
                    "orderable": false
                },
                {
                    "data": "SetupName",
                    render: function (data, type, row) {
                        if (data.length == 0) {
                            return "<div></div>";
                        } else {
                            return "<div>" + data + "<span class='fa fa-caret-right setup-caret'></span></div>";
                        }
                    },
                    searchable: true,
                    "orderable": false
                },
                {
                    "data": "StartTime",
                    render: cellDataFormat,
                    searchable: true,
                    "orderable": false
                },
                {
                    "data": "EndTime",
                    render: cellDataFormat,
                    searchable: true,
                    "orderable": false
                },
                {
                    "data": "Status",
                    render: function (data, type, row) {
                        if (data.toLowerCase() == 'passed')
                            return '<div class="passedText">' + data + '</div>';
                        else if (data.toLowerCase() == 'failed')
                            return '<div class="failedText">' + data + '</div>';
                        else if (data.toLowerCase() == 'inprogress')
                            return '<div class="inProgressText">' + data + '</div>';
                        else if (data.toLowerCase() == 'queued')
                            return '<div class="queuedText">' + data + '</div>';
                        else
                            return '<div class="notProcessedText">' + data + '</div>';
                    },
                    searchable: false,
                    "orderable": false
                },
                {
                    "data": null,
                    render: () => {

                            return '<div><i class="fa fa-info info_icon"></i></div>';
                        

                    },
                    searchable: false,
                    "orderable": false
                }
                ],
                columnDefs: [
                    { width: 18, targets: 0 }
                ]
            });
        else {

            table.clear();
            table.rows.add(data);
            table.draw();
            initTableListeners();
        }
        setTimeout(function () {
            if (data.length > 0) {
                $("#SMDownstreamDownloadButton").css('display', 'inline-block');
                $("#SetupStatusSearch").detach().prependTo(".srm_panelTopSections").css('display', 'block');;
                resize();
            }
        }, 300);
        initTableListeners();

    }
    function cellDataFormat(data, type, row) {
        return "<div>" + data + "</div>";
    }
    function format(d) {
        //var tableHeader = $("table#tasks tr:first-child > td");
        //var num = $(tableHeader[0]).outerWidth();
        //console.log(num);
        //num = 0;
        var html = '<table id="child" cellpadding="4" cellspacing="0" border="0" style="padding-left:' + 0 + 'px;" width="100%">' +
            '<tbody> ';
        var statusHtml;

        for (i = 0; i < d.length; i++) {
            if (d[i].Status.toLowerCase() == 'passed')
                statusHtml = '<div class="passedText">' + d[i].Status + '</div>';
            else if (d[i].Status.toLowerCase() == 'failed')
                statusHtml = '<div class="failedText">' + d[i].Status + '</div>';
            else if (d[i].Status.toLowerCase() == 'inprogress')
                statusHtml = '<div class="inProgressText">' + d[i].Status + '</div>';
            else if (d[i].Status.toLowerCase() == 'queued')
                statusHtml = '<div class="queuedText">' + d[i].Status + '</div>';
            else
                statusHtml = '<div class="notProcessedText">' + d[i].Status + '</div>';
            html += '<tr>' +
                (d[i].Status.toLowerCase() == 'passed' ?
                    '<td class="status-control"><div><i class="fa fa-check" style="background:#6DC49C"></i></div></td>' :
                    '<td class="status-control"><div><i class="fa fa-close" style="background:#DF9592"></i></div></td>') +
                '<td><div>' + d[i].ReportName + '</div></td>' +
                '<td><div>' + d[i].StartTime + '</div></td>' +
                '<td><div>' + d[i].EndTime + '</div></td>' +
                '<td>' + statusHtml + '</td>' +
                '<td ><div><i class="fa fa-info info_icon"></i></div></td>' +
                '</tr>';
        }
        html += '</tbody></table>';

        return html;
    }
    function associateReportData(d, row) {
        row.next().find('tbody>tr').each(function (index, item) {
            var data = d.reports[index];
            if (data != undefined) {
                data.SetupName = d.SetupName;
                $(item).data(data);
            }

        });
    }

    function download(data, filename, type) {
        var file = new Blob([data], { type: type });
        if (window.navigator.msSaveOrOpenBlob) // IE10+
            window.navigator.msSaveOrOpenBlob(file, filename);
        else { // Others
            var a = document.createElement("a"),
                url = URL.createObjectURL(file);
            a.href = url;
            a.download = filename;
            document.body.appendChild(a);
            a.click();
            setTimeout(function () {
                document.body.removeChild(a);
                window.URL.revokeObjectURL(url);
            }, 0);
        }
    }

    function search(keyword) {
        var keywords = [keyword.toLowerCase()];
        $.fn.dataTable.ext.search.push(
            function (settings, item, dataIndex) {
                var d = data[dataIndex];
                for (var i = 0; i < keywords.length; i++) {
                    if (d.SetupName.toLowerCase().includes(keywords[i]))
                        return true;
                    if (d.StartTime.toLowerCase().includes(keywords[i]))
                        return true;
                    if (d.EndTime.toLowerCase().includes(keywords[i]))
                        return true;
                    if (d.Status.toLowerCase().includes(keywords[i]))
                        return true;
                    if (d.reports != null)
                        for (var j = 0; j < d.reports.length; j++) {
                            if (d.reports[j].ReportName.toLowerCase().includes(keywords[i]))
                                return true;
                            if (d.reports[j].StartTime.toLowerCase().includes(keywords[i]))
                                return true;
                            if (d.reports[j].EndTime.toLowerCase().includes(keywords[i]))
                                return true;
                            if (d.reports[j].Status.toLowerCase().includes(keywords[i]))
                                return true;
                        }

                }
                return false;
            }
        )
        table.draw();
        $.fn.dataTable.ext.search.pop();
    }
    function initTableListeners() {
        // Add event listener for opening and closing details
        /*$('#tasks>tbody>tr').unbind('click').on('click', 'td', onRowClickListerner);*/
        $('#tasks').on('click', 'tbody tr', onRowClickListerner);
        $('#search_filter input').unbind('keyup').on('keyup', function (e) {
            e.stopPropagation();
            console.log(this.value);
            //table.search(this.value).draw();
        });
        //table.columns().every(function () {
        //    $('input', this.footer()).on('keyup', function () {
        //        search(this.value);
        //    });
        //});

        $('input')
            .off()
            .on('keyup', function () {
                search(this.value);
            });
        $("#closeMessage").unbind("click").on('click', function () {
            $("#dialogContainer").css("display", 'none');
        });

        $("#dialogContainer").unbind("click").click(function (e) {
            var target = $(e.target);
            if (target.attr("id") == "dialogContainer") {
                $("#dialogContainer").css("display", 'none');
                e.stopPropagation();
            }

        });
        $("#exportMessage").unbind("click").on('click', function () {
            var text = $("#report_message").text();
            var data = $("#report_message").data();
            if (data.isSetup)
                download(text, data.SetupName + ".txt", "text/plain");
            else
                download(text, data.SetupName + "_" + data.ReportName + ".txt", "text/plain");
        });
        $("#SMDownstreamDownloadButton").unbind("click").on('click', function () {
            var exportData = 'SETUP NAME,SETUP START DATE,SETUP END DATE,SETUP STATUS,REPORT NAME,REPORT START TIME,REPORT END TIME,REPORT STATUS\n';
            data.forEach(function (item, index) {
                var taskInfo = '"' + item.SetupName + '",' + item.StartTime + ',' + item.EndTime + ',"' + item.Status + '","';
                item.reports.forEach(function (report, index) {
                    exportData += taskInfo + report.ReportName + '",' + report.StartTime + ',' + report.EndTime + ',"' + report.Status + '"\n';
                });
            });
            download(exportData, "Downstream Sync Status.csv", "text/csv");
        });

    }
    function initReportsListener() {
        $(".info_icon").unbind('click').click(onReportInfoButtonClick);
    }

    function onReportInfoButtonClick(e) {
        if ($("#dialogContainer").css("display") == 'block')
            $("#dialogContainer").css("display", 'none');
        var tr = $(this).closest('tr');
        if (Object.keys(tr.data()).length == 0) {
            e.stopPropagation();
            var parentData = table.row(tr).data();

            $("#report_message").data({
                SetupName: parentData.SetupName,
                StartTime: parentData.StartTime,
                EndTime: parentData.EndTime,
                Status: parentData.Status,
                Reports: parentData.reports,
                isSetup: true,
                FailureMessage: parentData.FailureMessage
            });

            //var block_status_ids = [];
            //for (var index in parentData.reports) {
            //    block_status_ids.push(parentData.reports[index].BlockId);
            //}
            //getReportMessage(block_status_ids);
            OnSuccessReportMessage(null);
            return;
        }
        var block_status_id = tr.data().BlockId;
        $("#report_message").data({
            ReportName: tr.data().ReportName,
            SetupName: tr.data().SetupName,
            ProcessId: tr.data().ProcessId,
            StartTime: tr.data().StartTime,
            EndTime: tr.data().EndTime,
            Status: tr.data().Status,
            isSetup: false
        });
        getReportMessage([block_status_id]);
    }

    function onRowClickListerner() {
        var tr = $(this).closest('tr');
        //var row = table.row(tr);
        var row = table.row($(this));

        if (row.child.isShown()) {
            // close row
            row.child.hide();
            tr.removeClass('shown');
            tr.find('.setup-caret').addClass("fa-caret-right").removeClass("fa-caret-down");
        } else {
            // open row
            row.child(format(row.data().reports)).show();
            tr.addClass('shown');
            tr.find('.setup-caret').removeClass("fa-caret-right").addClass("fa-caret-down");
            var t = $("table#tasks tr:first-child > td");
            var adjustment = 0;
            for (var i = 0; i < 6; i++) {
                if (i == 0)
                    adjustment = 30;
                else if (i == 1)
                    adjustment = -30;
                else adjustment = 0;
                $(row.child().children().find("tr:first-child >td")[i]).width($(t[i]).width() + adjustment);
                if (i == 1) {
                    $(row.child().children().find("tr>td:nth-child(2)>div")).width($(t[i]).width() + adjustment);
                }
            }
            associateReportData(row.data(), tr);
            initReportsListener();

        }
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
            var error = updateDateObject();
            if (error != undefined && error.length > 0)
                return error;
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
        refreshGrid();
    }
    function refreshGrid() {
        updateDateObject();
        $("#dialogContainer").css("display", 'none');
        $("#SMDownstreamDownloadButton").css('display', 'none');
        UpdateDownstreamSyncStatus();
    }

    function updateDateObject() {
        var modifiedText = smdatepickercontrol.getResponse($("#" + srmDatePickerId));
        if (!isReady)
            return;
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
    }


    function getReportMessage(block_status_ids) {
        var param = {};
        param.blockStatusId = block_status_ids;
        param.resultDateTimeFormat = downstreamSyncStatusInfo.RADLongDateFormat;
        CallCommonServiceMethod('GetReportStatusMessage', param, OnSuccessReportMessage, OnFailure, null, false);
    }

    function UpdateDownstreamSyncStatus() {
        var param = {};
        param.dateFormat = downstreamSyncStatusInfo.RADShortDateFormat;
        param.resultDateTimeFormat = downstreamSyncStatusInfo.RADLongDateFormat;
        param.startDate = dateFilterValues.startDate;
        param.endDate = dateFilterValues.endDate;
        param.selDateOption = dateFilterValues.marked;
        param.CustomRadioOption = dateFilterValues.checked;
        CallCommonServiceMethod('GetAllDownstreamSyncStatus', param, OnSuccessTaskList, OnFailure, null, false);
    }

    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }


    function OnSuccessTaskList(result) {
        data = result.d;
        //if (data.length != 0)
        //    for (var i = 0; i < 10; i++) {
        //        data.push(data[0]);
        //    }

        if (data.length == 0 && table==null) {

            var elePresent = document.getElementById('dataNull');
            if (elePresent) {
                $("#dataNull").remove();
            }

                $('body').append('<div id="dataNull" style="text-align: center;font-size:16px;">No Data Available </div>');
 
        } else {

            $("#tasks").css('display', 'table');
            initTableWithData();
            resize();
        }

    }

    function OnSuccessReportMessage(result) {
        $("#dialogContainer").css("display", "block");
        //$("#report_message").height(-100 + $(document).height() / 2);
        $("#report_message").width(-40 + $(document).width());

        var data = $("#report_message").data();

        var title = data.SetupName + " - ";
        if (!data.isSetup)
            title += data.ReportName + " - ";
        title += data.StartTime + " - ";
        if (data.EndTime != null && data.EndTime != undefined && data.EndTime.trim() != '')
            title += data.EndTime + " - ";
        title += data.Status;
        $("#dialog .message-title").html(title);



        var messages = "";

        if (data.isSetup) {
            data.Reports.forEach(function (item, index) {
                if (index != 0)
                    messages += "\n";
                messages += "<span class='msg_span'>" + item.ReportName + " :</span>    " + item.StartTime + " - ";
                if (item.EndTime != null && item.EndTime != undefined && item.EndTime.trim() != '')
                    messages += item.EndTime + " - ";
                messages += item.Status;
                //if (item.Status.toLowerCase() == "failed") {
                //    messages += "\n";
                //    var messageData = result.d.filter((value, index) =>
                //        value.block_status_id == item.BlockId
                //    );
                //    messageData.forEach(function (item, index) {
                //        if (index != 0)
                //            messages += "\n";
                //        messages += "\t<span class='msg_span'>" + item.created_on + r" :</span>    " + item.message;
                //    });
                //}
            });
            //if (messages.trim() == '')
            //    messages = "No Report configured for this setup";
            if (data.FailureMessage != null && data.FailureMessage != "") {
                messages += "\n\n";
                messages += "<span class='msg_span'>Failure Message:</span>\n" + data.FailureMessage;
            }

        } else if (result != null && result != undefined && result.d.length > 0)
            result.d.forEach(function (item, index) {
                if (index != 0)
                    messages += "\n";
                messages += "<span class='msg_span'>" + item.created_on + " :</span>    " + item.message;
            });
        else
            messages = "No Messages Present";
        $("#report_message").html(messages);
    }

    function OnFailure(result) {
        console.log(result);
    }
    return syncStatus;
})();