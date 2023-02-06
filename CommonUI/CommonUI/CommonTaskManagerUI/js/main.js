var webServicePath = "/CTMServices.asmx";
var GRID_HEIGHT = 580;
$(function () {
    $.ajax({
            type: "POST",
            url: webServicePath + "/GetGridData",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                data = JSON.parse(msg.d); initGrid();
            },
            error: function (e) {
                alert("Error while fetching grid data. Please check inputs !",e);
            }
        });
    
    $.ajax({
            type: "POST",
            url: webServicePath + "/GetChainableTasks",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                chainableTasks = JSON.parse(msg.d); initChainableTasks();
            },
            error: function (e) {
                alert("Error while fetching tasks",e);
            }
        });

    $.ajax({
            type: "POST",
            url: webServicePath + "/GetFlowsJSON",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                flowsJSON = JSON.parse(msg.d);
            },
            error: function (e) {
                alert("Error while fetching flow tasks",e);
            }
        });

    var calendarNamesArray = calendarNames.split(',');
    $.each(calendarNamesArray, function (key, value) {
        $('#calendarSelect')
             .append($("<option></option>")
             .attr("value", value)
             .text(value));
        $('#calendarSelect2')
             .append($("<option></option>")
             .attr("value", value)
             .text(value));
    });

    $(document).on("click", '.threeWayCheckBox', function () {
        if ($(this).hasClass('off')) { $(this).removeClass('off').addClass('and').text('&'); }
        else if ($(this).hasClass('or')) { $(this).removeClass('or').addClass('off').text(''); }
        else if ($(this).hasClass('and')) { $(this).removeClass('and').addClass('or').text('OR'); }
    });

    var $dependencyOfRow = null;
    $(document).on("click", ".dependencyCheckbox .threeWayCheckBox", function () {
        $dependencyOfRow.data("dirty", true);
        if ($(this).hasClass('off') == false) {
            $dependencyOfRow.data("dependant_on_id", ($dependencyOfRow.data("dependant_on_id").toString()).replace("," + $(this).closest('.gridRow').data("flow_id").toString(), ''));
            $dependencyOfRow.data("dependant_on_id", ($dependencyOfRow.data("dependant_on_id").toString()).replace(",O" + $(this).closest('.gridRow').data("flow_id").toString(), ''));
            $dependencyOfRow.data("dependant_on_id", ($dependencyOfRow.data("dependant_on_id").toString()).replace(",&" + $(this).closest('.gridRow').data("flow_id").toString(), ''));
            $dependencyOfRow.data("dependant_on_id", ($dependencyOfRow.data("dependant_on_id").toString()).replace("O" + $(this).closest('.gridRow').data("flow_id").toString(), ''));
            $dependencyOfRow.data("dependant_on_id", ($dependencyOfRow.data("dependant_on_id").toString()).replace("&" + $(this).closest('.gridRow').data("flow_id").toString(), ''));
            $dependencyOfRow.data("dependant_on_id", $dependencyOfRow.data("dependant_on_id").toString() + "," + $(this).text().substring(0, 1) + $(this).closest('.gridRow').data("flow_id").toString());
        }
        else {
            $dependencyOfRow.data("dependant_on_id", ($dependencyOfRow.data("dependant_on_id").toString()).replace("," + $(this).closest('.gridRow').data("flow_id").toString(), ''));
            $dependencyOfRow.data("dependant_on_id", ($dependencyOfRow.data("dependant_on_id").toString()).replace(",O" + $(this).closest('.gridRow').data("flow_id").toString(), ''));
            $dependencyOfRow.data("dependant_on_id", ($dependencyOfRow.data("dependant_on_id").toString()).replace(",&" + $(this).closest('.gridRow').data("flow_id").toString(), ''));
        }
    });

    $(document).on("click", ".dependantOnBtn", function () {
        $dependencyOfRow = $(this).closest('.gridRow');
        console.log("dep of row :"); console.log($dependencyOfRow);
        $('.dependencyCheckbox').find('.threeWayCheckBox').show();
        $('.dependencyCheckbox').find('.threeWayCheckBox').removeClass('and or').addClass('off').text('');
        var dependants = $dependencyOfRow.data('dependant_on_id').toString().split(",");
        for (var i = 0; i < dependants.length; i++) {
            console.log($('#configureTasksTable .tbody').find('[data-flow_id="' + dependants[i].substring(1, dependants[i].toString().length) + '"]').children('.dependencyCheckbox'));
            if (dependants[i].substring(0, 1) == "O") {
                $('#configureTasksTable .tbody').find('[data-flow_id="' + dependants[i].substring(1, dependants[i].toString().length) + '"]').children('.dependencyCheckbox').find('.threeWayCheckBox').removeClass('off').addClass('or').text('OR');
            }
            else if (dependants[i].substring(0, 1) == "&") {
                $('#configureTasksTable .tbody').find('[data-flow_id="' + dependants[i].substring(1, dependants[i].toString().length) + '"]').children('.dependencyCheckbox').find('.threeWayCheckBox').removeClass('off').addClass('and').text('&');
            }
        }
        $('.gridRow').removeClass("dependantSelected");
        $(this).closest('.gridRow').addClass("dependantSelected");
        $('.dependencyCheckbox').show();
        $dependencyOfRow.find('.threeWayCheckBox').hide();

    })

    $(document).on("change", "#configureTasksTable .tbody .gridRow .cell input", function () { $(this).closest('.gridRow').data("dirty", true); console.log("task data dirty :"+$(this).closest('tr').data("dirty")) ;});

    $(document).on("change", "#configureTasksTable .tbody .gridRow .cell select", function () { $(this).closest('.gridRow').data("dirty", true); console.log("task data dirty :"+$(this).closest('tr').data("dirty")) ;});

    $(document).on("keyup", "#configureTasksTable .tbody .gridRow .cell input", function () { $(this).closest('.gridRow').data("dirty", true); console.log("task data dirty :"+$(this).closest('tr').data("dirty")) ;});

    $(document).on("change", "#schedulingInfo2 input", function () { $(this).closest('#schedulingInfo2').data("dirty", true); console.log("scheduler data dirty :"+$(this).closest('#schedulingInfo2').data("dirty")) ;});

    $(document).on("keyup", "#schedulingInfo2  input", function () { $(this).closest('#schedulingInfo2').data("dirty", true); console.log("scheduler data dirty :"+$(this).closest('#schedulingInfo2').data("dirty")) ;});

    $('#lightbox,.cross').click(function () { $('.popup').hide(); $('#lightbox').hide();
    $('#slidingContent').animate({'height':'380px'},'fast').animate({'height':'0px'},function(){
            $('#slidingContent').hide();});
            $('#schedulingInfo').slideUp(); $('#content').css({'-webkit-filter':'blur(0px)'});
            $('#slidingPanelBtn').text($('#slidingPanelBtn').text().substring(0,$('#slidingPanelBtn').text().length-1)+'▼'); 
            $('#lightbox').css('background-color','rgba(0,0,0,.4)');
            });

    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            $('.popup').hide(); $('#lightbox').hide();
            if ($('#slidingContent').height() > 300) {
                $('#slidingContent').animate({ 'height': '380px' }, 'fast').animate({ 'height': '0px' }, function () {
                    $('#slidingContent').toggle();
                });
                $('#schedulingInfo').slideUp(); $('#content').css({ '-webkit-filter': 'blur(0px)' });
                $('#slidingPanelBtn').text($('#slidingPanelBtn').text().substring(0, $('#slidingPanelBtn').text().length - 1) + '▼');
            }
        }
    });

    $("#configureTasksPopup").draggable();

    $('#selectedTasksContainer').sortable({ containment: "#slidingContent" });

    $("#deletePlaceholder").droppable({
        drop: function (event, ui) {
            ui.draggable[0].remove();
            $("#tasksContainer .tasks.selected:contains('" + ui.draggable.text() + "')").first().toggleClass("selected");
            $('#deletePlaceholderBg').toggle();
        },
        over: function (event, ui) { $('#deletePlaceholderBg').fadeToggle('fast'); },
        out: function (event, ui) { $('#deletePlaceholderBg').fadeToggle('fast'); }

    });

    $("#scheduledRadio").click(function () { $('#schedulingInfo').slideDown("slow", "easeOutBounce"); });

    $(document).on("click", "#slidingContent #tasksContainer .tasks", function () {
        $(this).toggleClass("selected");
        if ($(this).is('.selected')) { $("<li data-task_summary_id='" + $(this).data("task_summary_id") + "' data-module_name='" + $(this).data("module_name") + "' data-task_name='" + $(this).data("task_name") + "' data-task_type_name='" + $(this).data("task_type_name") + "'></li>").text($(this).text()).appendTo($("#selectedTasksContainer")); }
        else { $("#selectedTasksContainer li:contains('" + $(this).text() + "')").first().remove(); }
    });

//    $('#schedulingInfoScheduleBtn2').click(function () {
//        var schedulerInfo = null;
//        if ($('#scheduledRadio2').is(':checked')) {
//            $('#schedulerForm2').parsley('validate');
//            if ($('#schedulerForm2').parsley('isValid') == true) {
//               schedulerInfo = {
//                recurrenceType: $('input[name=recurrenceTypeRadioGroup2]:radio').filter(':checked').val(),
//                startDate: $('#startDateTxt2').val(),
//                recurrencePattern: $('input[name=recurrencePatternRadioGroup2]:radio').filter(':checked').val(),
//                daysOfWeek: getDaysOfWeekValue("dayOfWeekCheckboxGroup2"),
//                interval: $('#intervalTxt2').val(),
//                endDate: $('#endDateTxt2').val(),
//                numberOfRecurrence: $('#numberOfRecurrenceTxt2').val(),
//                startTime: $('#startTimeTxt2').val(),
//                neverEndJob: $('#neverEndJobCheckbox2').is(':checked'),
//                timeIntervalOfRecurrence: $('#intervalRecurrenceTxt2').val()
//                }
//                //PageMethods.UpdateChainSchedulingInfo($('#configureTasksTable').data("chain_id").toString(),JSON.stringify(schedulerInfo),function(){location.reload()},function(e){alert("Error:Unable to update schedule");console.log(e);})
//                 $.ajax({
//                    type: "POST",
//                    url: webServicePath + "/UpdateChainSchedulingInfo",
//                    //data: '{"num1":"' + num1 + '","num2":"' + num2 + '"}',
//                    //public static void UpdateChainSchedulingInfo(string chainId, string schedulerInfo)
//                    data:JSON.stringify({chainId:$('#configureTasksTable').data("chain_id").toString(),schedulerInfo:JSON.stringify(schedulerInfo)}),
//                    contentType: "application/json; charset=utf-8",
//                    dataType: "json",
//                    success: function (msg) {
//                    location.reload();
//                     
//                    },
//                    error: function (e) {
//                        alert("Error occured. Please check inputs !"+e);
//                        console.log(e);
//                    }
//                });
//            }
//            
//        }
//        console.log(schedulerInfo);
//    });

    $('#addChainBtn').click(function () {
        var schedulerInfo = null;
        if ($('#scheduledRadio').is(':checked')) {
            $('#schedulerForm').parsley('validate');
            if ($('#schedulerForm').parsley('isValid') == false) {
                $('#schedulingInfo').slideDown("slow", "easeOutBounce");
                return;
            }
            schedulerInfo = {
                recurrenceType: $('input[name=recurrenceTypeRadioGroup]:radio').filter(':checked').val(),
                startDate: $('#startDateTxt').val(),
                recurrencePattern: $('input[name=recurrencePatternRadioGroup]:radio').filter(':checked').val(),
                daysOfWeek: getDaysOfWeekValue("dayOfWeekCheckboxGroup"),
                interval: $('#intervalTxt').val(),
                endDate: $('#endDateTxt').val(),
                numberOfRecurrence: $('#numberOfRecurrenceTxt').val(),
                startTime: $('#startTimeTxt').val(),
                neverEndJob: $('#neverEndJobCheckbox').is(':checked'),
                timeIntervalOfRecurrence: $('#intervalRecurrenceTxt').val()

            }
        }
        
        var selectedTasksInfo = Array();
        $('#selectedTasksContainer li').each(function () {
            selectedTasksInfo.push({
                module_name: $(this).data("module_name"),
                task_name: $(this).data("task_name"),
                task_summary_id: $(this).data("task_summary_id"),
                task_type_name: $(this).data("task_type_name")
            });
        });

        if (selectedTasksInfo.length <= 0) { alert("Please add tasks from 'Available Tasks' list..."); return; }

        var chainInfo = {
            calendar_name: $('#calendarSelect').val(),
        };
        
        $.ajax({
            type: "POST",
            url: webServicePath + "/AddChain",
            data: JSON.stringify('{"chainInfo":"' + JSON.stringify(chainInfo) + '","selectedTasksInfo":"' + JSON.stringify(selectedTasksInfo) + '","schedulerInfo":"'+ JSON.stringify(schedulerInfo)+'"}'),
            data: JSON.stringify({chainInfo: JSON.stringify(chainInfo) ,selectedTasksInfo: JSON.stringify(selectedTasksInfo) ,schedulerInfo: JSON.stringify(schedulerInfo)}),
            contentType: "application/json; charset=utf-8",
            success: function (msg) {
                location.reload();
            },
            error: function (e) {
                alert("Error while adding chain!!",e);
            }
        });

    });

    $(".datepicker").datepicker({ showOn: "button", buttonImage: "http://jqueryui.com/resources/demos/datepicker/images/calendar.gif", buttonImageOnly: true });
    $('.datepicker').datepicker('option', 'dateFormat', 'yy-mm-dd');
    $('.maskedTime').mask("99:99:99");
    $('.maskedDate').mask('9999-99-99');
    
    //form validation
    $('#schedulerForm').parsley({
        validators: {
            timegtnow: function (val) {
                var now = new Date();
                var tmp = new Date(val + " " + $('#startDateTxt').val());
                console.log('parsley time');
                console.log(tmp + " > " + now + " = " + tmp > now);
                return (tmp > now);
            },
            dategtnow: function (val) {
                var now = new Date();
                var tmp = new Date(val);
                now.setHours(0, 0, 0, 0);
                tmp.setHours(0, 0, 0, 0);
                return (tmp >= now);

            },
            dategtstartdate: function(val)
            {
                var startDate = new Date($('#startDateTxt').val());
                var tmp = new Date(val);
                startDate.setHours(0, 0, 0, 0);
                tmp.setHours(0, 0, 0, 0);
                return (tmp >= startDate);
            }
        }
          , messages: {
              timegtnow: "Time should be greater than current time",
              dategtnow: "Date should be greater than today's date",
              dategtstartdate:"End Date should be greater than or equal to start date"
          }
    });

    $('#schedulerForm2').parsley({
        validators: {
            timegtnow2: function (val) {
                var now = new Date();
                var tmp = new Date(val + " " + $('#startDateTxt2').val());
                return (tmp > now);
            },
            dategtnow2: function (val) {
                var now = new Date();
                var tmp = new Date(val);
                now.setHours(0, 0, 0, 0);
                tmp.setHours(0, 0, 0, 0);
                return (tmp >= now);
            },
            dategtstartdate2: function(val)
            {
                var startDate = new Date($('#startDateTxt2').val());
                var tmp = new Date(val);
                startDate.setHours(0, 0, 0, 0);
                tmp.setHours(0, 0, 0, 0);
                return (tmp >= startDate);
            }
        }
          , messages: {
              timegtnow: "Time should be greater than current time",
              dategtnow: "Date should be greater than today's date",
              dategtstartdate:"End Date should be greater than or equal to start date"
          }
    });
    
    $('#taskChart').hover(function(){ $('#taskChart').css('display', 'block');},function(){$('#taskChart').css('display', 'none');});
    
    $('#showDependancy').hover(function () {
        $('#taskChart').css('display', 'block');
        generateDependancyGraphItems(currFlows);
        $('#configureTasksTable .tbody .gridRow').each(function (i, e) {
            var deps = $(e).data('dependant_on_id').split(',');
            $.each(deps, function (j, f) {
                if (f) {
                    jsPlumb.connect({
                        target: $('#flow' + $(e).data('flow_id')),
                        source: $('#flow' + f.substring(1, f.length) + '')
                    });
                    jsPlumb.draggable($('#flow' + $(e).data('flow_id')));
                    jsPlumb.draggable($('#flow' + f.substring(1, f.length) + ''));
                }
            });
        });

    }, function () {
        $('#taskChart').css('display', 'none');
    });

});
//end of document.ready(){}

function fillTasksContainer(data) {
    $('#slidingContent #tasksContainer').empty();
    for (var i = 0; i < data.length; i++) {
        $('#slidingContent #tasksContainer').append("<div class='tasks " + ((i % 2 == 0) ? 'even' : 'odd') + "' data-task_summary_id='" + data[i].task_summary_id + "' data-module_name='" + data[i].module_name + "' data-task_name='" + data[i].task_name + "' data-task_type_name='" + data[i].task_type_name + "'>" + data[i].task_name + " : " + data[i].task_type_name + "</div>");
    }
}

var currFlows = null;
function fillConfigureTasksTable(flows) {
    currFlows = flows;
    $('.dependencyCheckbox').hide();
    $('#configureTasksTable > .tbody').empty();
    $('#taskChart').append('<div style="    height: 70%;    display: inline-block;    width: 10px;"></div>');
    $('#configureTasksTable').data("chain_id", flows[0].chain_id);
    var flowNames ="";
    $(flows).each(function(i,e){flowNames+='<option value="'+e.flow_id+'">'+e.task_name+'</option>'});
    for (var i = 0; i < flows.length; i++) {
        $('#configureTasksTable > .tbody').append('<div class="gridRow ' + (i % 2 == 0 ? 'even' : 'odd') + '" data-dirty=false data-flow_id=' + flows[i].flow_id + ' data-dependant_on_id=' + flows[i].dependant_on_id +'><div style="width:10px;display:none" class="cell dependencyCheckbox"  ><div class="threeWayCheckBox off"></div></div><div class="cell">' +flows[i].task_name + '</div><div class="cell dependantOnBtn" style="padding: 16px 10px;background:url(https://cdn2.iconfinder.com/data/icons/Siena/256/gear%20blue.png);background-repeat: no-repeat;background-size: 28px;background-position: center;cursor: pointer;"></div><div class="cell"><input class="timeout" size=10 value="' +flows[i].timeout + '"></div><div class="cell"><input type=checkbox class="proceed_on_fail" ' + (flows[i].proceed_on_fail == true ? 'checked' : '') +'></div><div class="cell"><input type=checkbox class=is_muted ' + (flows[i].is_muted == true ? 'checked' : '') +'></div><div class="cell"><input type=checkbox class="rerun_on_fail" ' + (flows[i].rerun_on_fail == true ? 'checked' : '') +'> </div><div class="cell"><input size=10 class="fail_retry_duration" value="' + flows[i].fail_retry_duration +'"/></div><div class="cell"><input size=10 class="fail_number_retry" value="' + flows[i].fail_number_retry + '"></div><div class="cell"><select class="on_fail_run_task"><option selected value = "' + flows[i].on_fail_run_task +'">'+(flows[i].on_fail_run_task==0?'':flowsJSON[findIndexOf(flows[i].on_fail_run_task,"flow_id",flowsJSON)].task_name)+'</option>'+flowNames+'</select></div></div>');
    }
    $('#calendarSelect2').val(0);
    $('#allowParallelCheckbox').prop('checked',false);
    $('#maxParallelInstancesAllowed').val(0);
    $('#maxNoOfParallelContainer').hide();
    $.ajax({
        type:"POST",
        url: webServicePath + "/getChainInfo",
        data:JSON.stringify({chainId:flows[0].chain_id}),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var chainInfo = msg.d;
            console.log(chainInfo);
            $('#filewatcherFolderLocation').val(chainInfo.filewatcher_info.split('|')[0]);
            $('#filewatcherFileRegex').val(chainInfo.filewatcher_info.split('|')[1]);
            $('#calendarSelect2').val(chainInfo.calendar_name);
            if(chainInfo.allow_parallel == true){
                $('#allowParallelCheckbox').trigger('click');
                $('#maxParallelInstancesAllowed').val(chainInfo.max_parallel_instances_allowed);
            }
        },
        error: function (e) {
            alert("Unable to fetch Chain Info.",e);
       }
    });

    $.ajax({
        type: "POST",
        url: webServicePath + "/getScheduledJobId",
        data:JSON.stringify({chainId:flows[0].chain_id}),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            if (msg.d != -1) { 
                fillSchedulingInfo2(msg.d);
                $('#schedulingInfo2SlideBtn').show();
            }
            else { 
                $('#manualRadio2').prop('checked', 'true'); 
                $('#schedulingInfo2SlideBtn, #schedulingInfo2').hide(); 
                $('#schedulerForm2')[0].reset(); 
            }
        },
        error: function (e) {
            alert("Error while fetching scheduled jobs",e);
        }
    });

}

function fillSchedulingInfo2(scheduledJobId) {
    $('#schedulerForm2')[0].reset();
    $('#scheduledRadio2').prop('checked', 'true');
    
    $.ajax({
        type: "POST",
        url: webServicePath + "/getSchedulingInfo",
        data:JSON.stringify({scheduledJobId:scheduledJobId}),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            schedulingInfo = JSON.parse(msg.d);
            $('#startDateTxt2').val(eval("new " + schedulingInfo.StartDate.toString().substring(1, schedulingInfo.StartDate.toString().length - 1) + ".format('yyyy-MM-dd')"));
            if (schedulingInfo.RecurrenceType == true) {
                $('#recurringRadio2').trigger('click');
                $('#recurringRadio2').prop('checked', true);
                $('#nonRecurringRadio2').prop('checked', false);
            }
            else {
                $('#nonRecurringRadio2').trigger('click');
                $('#recurringRadio2').prop('checked', false);
            }
                    
            $(":radio[name='recurrencePatternRadioGroup2'][value=" + schedulingInfo.RecurrencePattern + "]").trigger("click");
            $(ExtractDaysOfWeek(schedulingInfo.DaysofWeek)).each(function (i, e) { $('input[name=dayOfWeekCheckboxGroup2]').eq(parseInt(e)).trigger("click") });
            $('#intervalTxt2').val(Math.max(parseInt(schedulingInfo.DaysInterval), parseInt(schedulingInfo.MonthInterval), parseInt(schedulingInfo.WeekInterval)));
            $('#endDateTxt2').val(eval("new " + schedulingInfo.EndDate.toString().substring(1, schedulingInfo.EndDate.toString().length - 1) + ".format('yyyy-MM-dd')"));
            $('#numberOfRecurrenceTxt2').val(schedulingInfo.NoOfRecurrences);
            $('#startTimeTxt2').val(eval("new " + schedulingInfo.StartTime.toString().substring(1, schedulingInfo.StartTime.toString().length - 1) + ".format('HH:mm:ss')"));
            if (schedulingInfo.NoEndDate == true) { $('#neverEndJobCheckbox2').trigger('click') }
            $('#intervalRecurrenceTxt2').val(schedulingInfo.TimeIntervalOfRecurrence);
            $('#startDateTxt2').val(eval("new " + schedulingInfo.StartDate.toString().substring(1, schedulingInfo.StartDate.toString().length - 1) + ".format('yyyy-MM-dd')"));
            $('#schedulerForm2').parsley('destroy');
            $('#schedulingInfo2').data("dirty","false");
       },
       error: function (e) {
            alert("Error while fetching scheduling info!!",e);
       }
   });
}

function findIndexOf(searchText, searchColumn, searchArray) {
    for (var i = 0; i < searchArray.length; i++) {
        if (searchArray[i][searchColumn] == searchText) {
            return i;
        }
    }
    return -1;
}

function findIndexOfContains(searchText, searchColumn, searchArray) {
    for (var i = 0; i < searchArray.length; i++) {
        if (searchArray[i][searchColumn].toString().indexOf(searchText) > -1) {
            return i;
        }
    }
    return -1;
}

function configureSaveBtn_click() {
    var updateFlows = Array();
    var g = new Graph(); 
    var flag = true;
    $("#configureTasksTable .tbody .gridRow").each(function () {
        if($(this).data("dependant_on_id").toString().indexOf("O")>-1 && $(this).data("dependant_on_id").toString().indexOf("&")==-1){
            alert(flowsJSON[findIndexOf($(this).data("flow_id"), "flow_id", flowsJSON)].task_name+" should have 1 or more AND dependants. Please add another AND dependant or modify current dependant to AND dependancy");
            flag = false;
            return false;
        }
        
        var deps = $(this).data("dependant_on_id").split(",");
        for (var i = 0; i < deps.length; i++) {
            if (deps[i] != "") {
                g.addEdge(
                    $(this).data("flow_id").toString(), deps[i].toString().substring(1, deps[i].toString().length)
                    );
            }
        }
         
        if ($(this).data("dirty") == true) {
            console.log("update me" + $(this).data("flow_id") + ">>" + $(this).data("dependant_on_id"));
            updateFlows.push({
                dependant_on_id: $(this).data("dependant_on_id"),
                flow_id: $(this).data("flow_id"),
                timeout: $(this).find('.timeout').val(),
                proceed_on_fail: $(this).find('.proceed_on_fail').is(':checked'),
                is_muted: $(this).find('.is_muted').is(':checked'),
                rerun_on_fail: $(this).find('.rerun_on_fail').is(':checked'),
                fail_retry_duration: $(this).find('.fail_retry_duration').val(),
                fail_number_retry: $(this).find('.fail_number_retry').val(),
                on_fail_run_task:$(this).find('.on_fail_run_task').val()
            });

        };
    });

    if(flag == false){return;}

    if($('#scheduledRadio2').is(':checked')){
        if($('#schedulingInfo2').data("dirty")==true){
            var schedulerInfo = null;
            if ($('#scheduledRadio2').is(':checked')) {
                $('#schedulerForm2').parsley('validate');
                if ($('#schedulerForm2').parsley('isValid') == true) {
                    schedulerInfo = {
                    recurrenceType: $('input[name=recurrenceTypeRadioGroup2]:radio').filter(':checked').val(),
                    startDate: $('#startDateTxt2').val(),
                    recurrencePattern: $('input[name=recurrencePatternRadioGroup2]:radio').filter(':checked').val(),
                    daysOfWeek: getDaysOfWeekValue("dayOfWeekCheckboxGroup2"),
                    interval: $('#intervalTxt2').val(),
                    endDate: $('#endDateTxt2').val(),
                    numberOfRecurrence: $('#numberOfRecurrenceTxt2').val(),
                    startTime: $('#startTimeTxt2').val(),
                    neverEndJob: $('#neverEndJobCheckbox2').is(':checked'),
                    timeIntervalOfRecurrence: $('#intervalRecurrenceTxt2').val()
                    }
                    $.ajax({
                        type: "POST",
                        url: webServicePath + "/UpdateChainSchedulingInfo",
                        data:JSON.stringify({chainId:$('#configureTasksTable').data("chain_id").toString(),schedulerInfo:JSON.stringify(schedulerInfo)}),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (msg) {
                        location.reload();
                        },
                        error: function (e) {
                            alert("Scheduling info not saved. Please check inputs !"+e);
                            console.log(e);
                        }
                    });
                }
                else{return false;}
            }
        }
    }

    var deadlocks = findDeadlock(g)
    if (deadlocks.length > 0) {
        $.each(deadlocks, function (i, e) {
            alert("Error: Deadlock at " + flowsJSON[findIndexOf(e.vert1, "flow_id", flowsJSON)].task_name + " and " + flowsJSON[findIndexOf(e.vert2, "flow_id", flowsJSON)].task_name);
        });
    }
    else {
        var chainInfo={
            chainId: $('#configureTasksTable').data("chain_id"),
            calendar:$('#calendarSelect2').find('option:selected').val(),
            allowParallel:$('#allowParallelCheckbox').is(':checked'),
            maxParallelInstancesAllowed:$('#allowParallelCheckbox').is(':checked')==true?$('#maxParallelInstancesAllowed').val():'0',
            filewatcherInfo:$('#filewatcherFolderLocation').val()+"|"+$('#filewatcherFileRegex').val()
            };
        $.ajax({
            type: "POST",
            url: webServicePath + "/configureTasks",
            data:JSON.stringify({modifiedTasks:JSON.stringify(updateFlows)}),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                updateFlowsJSON(updateFlows);
                $('.popup').hide();
                $('#lightbox').hide();
                $('#dialogMsg').text('Configuration saved Successfully..!!');
                $('#dialog').dialog({ modal: true, buttons: {
                    Ok: function () {
                        $(this).dialog("close");
                    }
                }
                });
                $('#dialog').dialog('option', 'title', 'Configure Tasks');
                $.ajax({
                    type: "POST",
                    url: webServicePath + "/updateChain",
                    data:JSON.stringify({chainInfo:JSON.stringify(chainInfo)}),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (msg) {},
                    error:function(){alert("Unable to update chain info",e);}
                });
            },
            error: function (e) {
                alert("Unable to configure tasks!",e);
            }
        });
    }
}

function updateFlowsJSON(flows) {
    for (var i = 0; i < flows.length; i++) {
        console.log(flows[i]);
        for (var attr in flows[i]) {
            flowsJSON[findIndexOf(flows[i].flow_id, "flow_id", flowsJSON)][attr] = flows[i][attr];
        }
    }
}

function randomDate(start, end) {
    var tmp = new Date(start.getTime() + Math.random() * (end.getTime() - start.getTime()));
    var then = tmp.getFullYear() + '-' + (tmp.getMonth() + 1) + '-' + tmp.getDay();
    then += ' ' + tmp.getHours() + ':' + tmp.getMinutes();
    return new Date(start.getTime() + Math.random() * (end.getTime() - start.getTime()));
}

function ExtractDaysOfWeek(daysOfWeek) {
    var dayOfWeek = Array();
    var valDaysOfWeek = daysOfWeek;
    var count = 0;
    var fixedVal = 64;
    var itemNumber = 6;
    while (valDaysOfWeek > 0) {
        if (valDaysOfWeek >= fixedVal) {
            dayOfWeek.push(itemNumber.toString());
            count++;
            valDaysOfWeek -= fixedVal;
        }
        fixedVal = fixedVal / 2;
        itemNumber -= 1;
    }
    return dayOfWeek;
}

function getDaysOfWeekValue(checkBoxGroupName) {
    var sum = 0;
    $('input[name='+checkBoxGroupName+']:checked').each(function () { sum += parseInt(this.value) });
    return sum;
}

jsPlumb.ready(function () {
    var dynamicAnchors = [[0, 0.5, -1, 0], [1, 0.5, 0, -1]];
    jsPlumb.Defaults.PaintStyle = { lineWidth: 3, strokeStyle: 'rgba(0,0,0,0.4)' };
    jsPlumb.Defaults.Endpoint = ["Dot", { radius: 2, hoverClass: "myEndpointHover"}];
    jsPlumb.Defaults.Overlays = [["Arrow", {
        location: 0.9,
        length: 10,
        width: 10,
        id: 'arrow',
        foldback: 0.9,
        paintStyle: {
            fillStyle: "#000000",
            lineWidth: 5
        }
    }]];
    jsPlumb.Defaults.Anchor = dynamicAnchors;
    jsPlumb.Defaults.Connector = ["Straight"];

});


function initGrid() {

    for (var i = 0; i < data.length; i++) {
        data[i].trigger_task = '<div class="btn3way" > Trigger<div class="btnLeft">All</div><div class ="btnRight">One</div></div>';
        data[i].checkbox = "<input type='checkbox'>";
        for (var j = 0; j < data[i].children.length; j++) {
            data[i].children[j].trigger_task = '<div class="btn3way" > Trigger<div class="btnLeft">All</div><div class ="btnRight">One</div></div>';
            data[i].children[j].checkbox = "<input type='checkbox'>";
        }
    }

    var columns = [
		{ name: 'id', id: 'flow_id', width: 0 },
        { name: '', id: 'last_run_status', width: 2 },
        { name: '<input type=checkbox>', id: 'checkbox', width: 11 },
		{ name: 'Module', id: 'module_name' },
		{ name: 'Task Name', id: 'task_name' },
		{ name: 'Task Type', id: 'task_type_name' },
        { name: 'Trigger Task', id: 'trigger_task', width: 100 },
		{ name: 'Trigger Type', id: 'trigger_type' },
		{ name: 'Configure', id: 'configure' },
		{ name: 'Delete', id: 'delete_ico' },
		{ name: 'Subscribe', id: 'subscribe' },
        { name: 'Muted', id: 'is_muted' },
        { name: 'Chain Id', id: 'chain_id', width: 0 }, { name: "Custom Handler", id: "custom_handler", width: 100 },
        { name: "Next Scheduled Time", type: 'DateTime', id: 'next_scheduled_time', width: 180 },
        { name: 'Subscribe Id', id: 'subscribe_id', width: 0 },
        {name:"Configure Page", id:"configure_page_url",width:100}
	];

    for (var i = 0; i < data.length; i++)
    { filteredData.push(data[i]); }

    generateGrid('#mainContainer', columns, filteredData, GRID_HEIGHT);

    $('.configure_page_url').each(function(i,e){$(e).click(function(){
        window.open($(this).children().eq(0).attr("href"),"","width=800,height=800");})
    });

    $(document).on("click",".gridRow .checkbox input",function(){
        $self = $(this); 
        if($self.prop('checked')==true){
            $('#dialogMsg').empty();
            $('#dialogMsg').text("Select All Tasks in chain?");
            $('#dialog').dialog({ 
                modal: true, width: 'auto',height:'auto', buttons:{
                    "Yes": function () {
                        $self.closest('.gridRowContainer').children('.children').eq(0).children('.row').each(function(i,e){console.log($(e).children('.flow_id').text());$(e).find(' input').prop('checked',true);});//trigger("click");});
                        $(this).dialog("close");
                    },
                    "Select only this task":function(){
                        $(this).dialog("close");
                    }
                }
            }); 
            $('#dialog').dialog('option', 'title', 'Select All Tasks');
        }
        else{
            $('#dialogMsg').empty();
            $('#dialogMsg').text("Un-select All Tasks in chain?");
            $('#dialog').dialog({ 
                modal: true, width: 'auto',height:'auto', buttons:{
                    "Yes": function () {
                        $self.closest('.gridRowContainer').children('.children').eq(0).children('.row').each(function(i,e){console.log($(e).children('.flow_id').text());$(e).find(' input').prop('checked',false);});//trigger("click");});
                        $(this).dialog("close");
                    },
                    "Un-select only this task":function(){
                        $(this).dialog("close");
                    }
                }
            }); 
            $('#dialog').dialog('option', 'title', 'Select All Tasks');
        }
    });
    
    $('body').mouseover(function () {
        $('.btn3way').children('.btnLeft').hide('slide', { direction: 'up' }, 200);
        $('.btn3way').children('.btnRight').hide('slide', { direction: 'down' }, 200);
    });

    $('.btnLeft, .btnRight').mouseover(function (e) {
        $('.btn3way').children('.btnLeft').not($(this)).not($(this).siblings()).hide('slide', { direction: 'up' }, 200);
        $('.btn3way').children('.btnRight').not($(this)).not($(this).siblings()).hide('slide', { direction: 'down' }, 200);
        e.stopPropagation();
    });

    $('#removeSelectedTasks').click(function(){
        $('#selectedTasksContainer').empty();
        $('#tasksContainer .tasks').each(function(i,e){$(e).removeClass('selected');})
    });

    $('.btn3way').hover(
	    function (e) {
            $('.btn3way').children('.btnLeft').hide('slide', { direction: 'up' }, 200);
	        $('.btn3way').children('.btnRight').hide('slide', { direction: 'down' }, 200);
	        $(this).find('.btnLeft').show('slide', { direction: 'left' }, 200);
	        $(this).find('.btnRight').show('slide', { direction: 'right' }, 200);
        },
	    function (e) {
	        $('.btn3way').children('.btnLeft').hide('slide', { direction: 'up' }, 200); ;
	        $('.btn3way').children('.btnRight').hide('slide', { direction: 'down' }, 200);
	        e.stopPropagation();
	    }
   );

    //grid Events
    $('#muteBtn').click(function () {
        var selectedFlows = Array();
        $('#tBody .gridRowContainer .row').each(function () {
            if ($(this).find(' .checkbox input').is(':checked')) {
                selectedFlows.push($(this).find('.flow_id').text());
            }

        });
        if (selectedFlows.length > 0) {
            $.ajax({
                type: "POST",
                url: webServicePath + "/MuteFlows",
                data:JSON.stringify({selectedFlows:JSON.stringify(selectedFlows)}),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    $(selectedFlows).each(function(i,e){
                        $($('.row .flow_id').filter(function(){return $(this).text()==e;})).each(function(i,e){
                            $(e).siblings('.is_muted').addClass('glow-text').removeClass('unmuted').addClass('muted');
                            //setTimeout(function(){$(e).siblings('.is_muted').text('true');},500)
                        });
                    });
                },
                error: function (e) {
                    alert("Error occured while muting flows!",e);
                }
            });
        }
    });

    $(document).on("click", ".custom_handler", function () {
        var $self = $(this);
        var flowId = $self.closest('.row').children('.flow_id').text();
        $('#dialogMsg').empty();
        $('#dialogMsg').append('<table cellpadding="10"><tr><td>Assembly Name </td><td><input id="assemblyName"/> </td></tr><tr><td>Class Name </td><td><input id="className"/> </td>                </tr>            </table>      ');
        $.ajax({
            type: "POST",
            url: webServicePath + "/GetAssemblyInfoByFlowId",
            data:JSON.stringify({flowId:flowId}),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var assemblyInfoPopulate = JSON.parse(msg.d);
                if (assemblyInfoPopulate != null && assemblyInfoPopulate != undefined) {
                    $('#assemblyName').val(assemblyInfoPopulate.split('|')[0]);
                    $('#className').val(assemblyInfoPopulate.split('|')[1]);
                }
                $('#dialog').dialog({ 
                    modal: true, width: 'auto',height:'auto', buttons:
                        {
                        "Save": function () {
                            var assemblyInfo = $('#assemblyName').val() + "|" + $('#className').val();
                            $.ajax({
                                type: "POST",
                                url: webServicePath + "/UpdateFlowSetAssemblyInfoByFlowId",
                                data:JSON.stringify({flowId:flowId,assemblyInfo:assemblyInfo}),
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (msg) {},
                                error: function (e) {
                                    alert("Error while updating Assembly Info!!",e);
                                }
                            });
                            $(this).dialog("close");
                        },
                        "Cancel": function () { $(this).dialog("close"); }
                    }
                }); 
                $('#dialog').dialog('option', 'title', 'Custom Handler');
            },
            error: function (e) {
                alert("Error while fetching Assembly Info!!",e);
               }
            });
        })

        $(document).on("click", ".expandBtn", function () {
            $(this).parent().siblings('.children').slideToggle('fast');
            $(this).toggleClass('collapse');
        });

        $(document).on("click", "#tBody .gridRow .configure", function () {
            var chainId = $(this).siblings('.chain_id').text();
            $('#schedulingInfo2').hide();
            $('#configureTasksPopup').show('slide', { direction: 'left' }); $('#lightbox').show('fast');
            fillConfigureTasksTable(flowsJSON.filter(function (el) { return el.chain_id == chainId }));
        

        });

        $(document).on("click", "#gridHead .checkbox input:checkbox", function () {($('#tBody .checkbox input:checkbox').prop('checked', $(this).is(':checked')))});

        $(document).on("click", ".subscribeEntity", function () { $(this).toggleClass("selected") });

        $(document).on("click", '.children-row .subscribe, .gridRow .cell.subscribe', function () {
            var $self = $(this);
            var flowId = $self.closest('.row').children('.flow_id').text();
            var subscribeListArray = subscribeList.split(',');
            $('#dialogMsg').empty();
            $('#dialogMsg').append('<div class="subscriptionListContainer"><h3>Success</h3><div id="successSubscribtionListContainer"></div><input style="font-size:11px" id="successMailSubject" placeholder="Subject..."/><br><textarea style="font-size:11px" id="successMailBody" rows="4" cols="50" placeholder="Body..."/></div><div class="subscriptionListContainer"><h3>Failure</h3><div id="failureSubscribtionListContainer"></div><input style="font-size:11px"  id="failureMailSubject" placeholder="Subject..."/><br><textarea style="font-size:11px" id="failureMailBody" rows="4" cols="50" placeholder="Body..."/></div>');
            $.each(subscribeListArray, function (key, value) {
                $('#successSubscribtionListContainer')
                 .append("<div class='subscribeEntity "+(key%2==0?"even":"odd")+"'>" + value + "</div>");
                $('#failureSubscribtionListContainer')
                 .append("<div class='subscribeEntity "+(key%2==0?"even":"odd")+"'>" + value + "</div>");
            });

            var subscribeStringPopulate = $self.closest('.row').children('.subscribe_id').text();
            var subscribeArr = subscribeStringPopulate.split('|');
            if (subscribeArr != undefined  ) {
                if(subscribeArr[0] != undefined){
                    $(subscribeArr[0].split(',')).each(function (i, e) { $('#successSubscribtionListContainer .subscribeEntity:contains("' + e + '")').eq(0).addClass('selected') });
                }
                if(subscribeArr[1] != undefined){
                    $(subscribeArr[1].split(',')).each(function (i, e) { $('#failureSubscribtionListContainer .subscribeEntity:contains("' + e + '")').eq(0).addClass('selected') });
                }
                if(subscribeArr[2] !=undefined){$('#successMailSubject').val(decodeURI(subscribeArr[2]))}
                if(subscribeArr[3] !=undefined){$('#successMailBody').val(decodeURI(subscribeArr[3]))}
                if(subscribeArr[4] !=undefined){$('#failureMailSubject').val(decodeURI(subscribeArr[4]))}
                if(subscribeArr[5] !=undefined){$('#failureMailBody').val(decodeURI(subscribeArr[5]))}
            }

            $('#dialog').dialog({ 
                modal: true, width:'auto', buttons:{
                    "Subscribe": function () {
                        var subscribeString = "";
                        $('#successSubscribtionListContainer').children('.selected').each(function (i, e) { subscribeString += $(e).text() + ',' });
                        subscribeString += '|'
                        $('#failureSubscribtionListContainer').children('.selected').each(function (i, e) { subscribeString += $(e).text() + ',' });
                        subscribeString += '|'
                        subscribeString+= encodeURI( $('#successMailSubject').val());
                        subscribeString += '|'
                        subscribeString+=encodeURI( $('#successMailBody').val());
                        subscribeString += '|'
                        subscribeString+= encodeURI($('#failureMailSubject').val());
                        subscribeString += '|'
                        subscribeString+= encodeURI($('#failureMailBody').val());
                        $.ajax({
                            type: "POST",
                            url: webServicePath + "/SubscribeFlow",
                            data:JSON.stringify({flowId:flowId,subscribeString:subscribeString}),
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (msg) {
                                $self.closest('.row').children('.subscribe_id').text(subscribeString);
                                if ($('#successSubscribtionListContainer').find('.selected').length > 0 || $('#failureSubscribtionListContainer').find('.selected').length > 0) {
                                    $self.addClass('subscribed');
                                }
                                else { $self.removeClass('subscribed'); }
                            },
                            error: function (e) {
                                alert("Error occured while subscribing!!",e);
                            }
                        });
                        $(this).dialog("close");
                    },
                    "Cancel": function () { $(this).dialog("close"); }
                }
            }); 
            $('#dialog').dialog('option', 'title', 'Subscribe Tasks');
        });

        $(document).on("click", '#subscribeChain', function () {
            var $self = $(this);
            var chainId = $('#configureTasksTable').data("chain_id");
            var subscribeListArray = subscribeList.split(',');
            $('#dialogMsg').empty();
            $('#dialogMsg').append('<div class="subscriptionListContainer"><h3>Success</h3><div id="successSubscribtionListContainer"></div><input style="font-size:11px" id="successMailSubject" placeholder="Subject..."/><br><textarea style="font-size:11px" id="successMailBody" rows="4" cols="50" placeholder="Body..."/></div><div class="subscriptionListContainer"><h3>Failure</h3><div id="failureSubscribtionListContainer"></div><input style="font-size:11px"  id="failureMailSubject" placeholder="Subject..."/><br><textarea style="font-size:11px" id="failureMailBody" rows="4" cols="50" placeholder="Body..."/></div>');
            $.each(subscribeListArray, function (key, value) {
                $('#successSubscribtionListContainer').append("<div class='subscribeEntity'>" + value + "</div>");
                $('#failureSubscribtionListContainer').append("<div class='subscribeEntity'>" + value + "</div>");
            });
            $.ajax({
            type: "POST",
                url: webServicePath + "/GetChainSubscribeString",
                data:JSON.stringify({chainId:chainId}),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    var subscribeStringPopulate = JSON.parse(msg.d);
                    var subscribeArr = subscribeStringPopulate.split('|');
                    if (subscribeArr != undefined  ) {
                        if(subscribeArr[0] != undefined){
                            $(subscribeArr[0].split(',')).each(function (i, e) { $('#successSubscribtionListContainer .subscribeEntity:contains("' + e + '")').eq(0).addClass('selected') });
                        }
                        if(subscribeArr[1] != undefined){
                            $(subscribeArr[1].split(',')).each(function (i, e) { $('#failureSubscribtionListContainer .subscribeEntity:contains("' + e + '")').eq(0).addClass('selected') });
                        }
                        if(subscribeArr[2] !=undefined){$('#successMailSubject').val(decodeURI(subscribeArr[2]))}
                        if(subscribeArr[3] !=undefined){$('#successMailBody').val(decodeURI(subscribeArr[3]))}
                        if(subscribeArr[4] !=undefined){$('#failureMailSubject').val(decodeURI(subscribeArr[4]))}
                        if(subscribeArr[5] !=undefined){$('#failureMailBody').val(decodeURI(subscribeArr[5]))}
                    }

                    $('#dialog').attr('title', 'Subscribe Chain');
                    $('#dialog').dialog({ modal: true, width: 700, buttons:{
                        "Subscribe": function () {
                            var subscribeString = "";
                            $('#successSubscribtionListContainer').children('.selected').each(function (i, e) { subscribeString += $(e).text() + ',' });
                            subscribeString += '|'
                            $('#failureSubscribtionListContainer').children('.selected').each(function (i, e) { subscribeString += $(e).text() + ',' });
                            subscribeString += '|'
                            subscribeString+= encodeURI( $('#successMailSubject').val());
                            subscribeString += '|'
                            subscribeString+=encodeURI( $('#successMailBody').val());
                            subscribeString += '|'
                            subscribeString+= encodeURI($('#failureMailSubject').val());
                            subscribeString += '|'
                            subscribeString+= encodeURI($('#failureMailBody').val());
                            $.ajax({
                                type: "POST",
                                url: webServicePath + "/SubscribeChain",
                                data:JSON.stringify({chainId:chainId,subscribeString:subscribeString}),
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (msg) {},
                                error: function (e) {
                                    alert("Error occured while subscribing chain!!",e);
                                }
                            });
                            $(this).dialog("close");
                        },
                        "Cancel": function () { $(this).dialog("close"); }
                    }}); 
                    $('#dialog').dialog('option', 'title', 'Subscribe Chain');
                },
                error: function (e) {
                    alert("Error occured while fetching subscription info!!",e);
                }
            });
        });

        $(document).on("click", '.gridRow .trigger_task .btnLeft', function () {
            var flowId = $(this).closest('.row').children('.flow_id').text();
            var chainId = $(this).closest('.row').children('.chain_id').text();
            $.ajax({
                type: "POST",
                url: webServicePath + "/triggerChain",
                data:JSON.stringify({chainId:chainId}),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {alert(msg.d)},
                error: function (e) {
                    alert("Error occured while triggering tasks!",e);
                }
            });
        });

        $(document).on("click", '.children-row .trigger_task .btnLeft', function () {
            var flowId = $(this).closest('.row').children('.flow_id').text();
            var chainId = $(this).closest('.row').children('.chain_id').text();
            $.ajax({
                type: "POST",
                url: webServicePath + "/triggerTaskInChain",
                data:JSON.stringify({chainId:chainId,flowId:flowId}),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {alert("Task has been triggered")},
                error: function (e) {
                    alert("Error occured while triggering tasks!",e);
                }
            });
        });

        $(document).on("click", '.trigger_task .btnRight', function () {
            var flowId = $(this).closest('.row').children('.flow_id').text();
            var chainId = $(this).closest('.row').children('.chain_id').text();
            $.ajax({
                type: "POST",
                url: webServicePath + "/triggerSingleTaskInChain",
                data:JSON.stringify({"chainId":chainId,"flowId":flowId}),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {alert("Task has been triggered")},
                error: function (e) {
                    alert("Error occured while triggering tasks!",e);
                }
            });
        });

        $(document).on("click", '.delete_ico',
            function () {
                var flowId = $(this).closest('.row').children('.flow_id').text();
                var chainId = $(this).closest('.row').children('.chain_id').text();
                
                //simple deletion for row with no children
                if ($(this).closest('.row').siblings('.children').length == 0 && $(this).parent().is('.children-row') == false) {
                    var flows = new Array();
                    flows.push(flowId);
                    $.ajax({
                        type: "POST",
                        url: webServicePath + "/DeleteChain",
                        data:JSON.stringify({chainId:chainId}),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (msg) { 
                            data.splice(findIndexOf(flowId, "flow_id", data), 1);
                            filteredData.splice(findIndexOf(flowId, "flow_id", filteredData), 1);
                            $('.flow_id').filter(function () { return $(this).text() == flowId; }).closest(".gridRowContainer").hide('slide', 'left', function () { $(this).remove(); });
                        },
                        error: function (e) {
                            alert("Error occured while deleting task!!",e);
                        }
                    });
                }

                //if row is children
                else if ($(this).parent().is('.children-row') == true) {
                    var flows = new Array();
                    flows.push(flowId);
                    var parentflowId = $(this).parent().parent().siblings().eq(0).children('.flow_id').first().text();
                    $(this).parent().hide('slide', 'left', 200, function () { $(this).remove(); });
//                    PageMethods.DeleteFlows(flows, function () {
//                        data[findIndexOf(parentflowId, "flow_id", data)]["children"].splice(findIndexOf(flowId, "flow_id", data[findIndexOf(parentflowId, "flow_id", data)]["children"]), 1);

//                    },
//                    function () {
//                        alert("Error: Could not delete.. Try Again..!!")
//                    });

                    $.ajax({
                        type:"POST",
                        url: webServicePath +"/DeleteFlows",
                        data:JSON.stringify({flow_ids:JSON.stringify(flows)}),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (msg) { 
                            data[findIndexOf(parentflowId, "flow_id", data)]["children"].splice(findIndexOf(flowId, "flow_id", data[findIndexOf(parentflowId, "flow_id", data)]["children"]), 1);
                        },
                        error: function (e) {
                            alert("Error occured while deleting task!!",e);
                        }
                    });
                }
                //row head 
                else {
                    var self = $(this);
                    $('#dialogMsg').text('Delete all tasks in chain or Delete only this task??');
                    $('#dialog').dialog({ modal: true, width: 'auto',height:'auto', buttons:{
                        "Delete All Tasks in Chain": function () {
                            var flows = new Array();
                            flows.push(flowId);
                            $.each(self.parent().siblings().eq(0).find('.row'), function () { flows.push($(this).children('.flow_id').text()); })
                            $.ajax({
                                type: "POST",
                                url: webServicePath + "/DeleteChain",
                                data:JSON.stringify({chainId:chainId}),
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (msg) { 
                                    self.parent().parent().hide('slide', 'left', 200, function () { self.remove(); });
                                    data.splice(findIndexOf(flowId, "flow_id", data), 1);
                                    filteredData.splice(findIndexOf(flowId, "flow_id", filteredData), 1);
                                },
                                error: function (e) {
                                    alert("Error occured while deleting chain!!",e);
                                }
                            });
                            $(this).dialog("close");
                        },
                        "Delete only This Task": function () {
                            alert("Cannot Delete Head Node of a chain..");
                            $(this).dialog("close");
                        },
                        "Cancel": function () { $(this).dialog("close") }
                    }
                });
                $('#dialog').dialog('option', 'title', 'Delete Tasks');
            }
        });

        $('#gridModuleSelect, #gridTaskTypeSelect').on('change', function () {
            filteredData = data.filter(function (el) {
                return (($('#gridModuleSelect').val() == "all") ? true : (el.module_name == $('#gridModuleSelect').val() || findIndexOf($('#gridModuleSelect').val(), "module_name", el.children) >= 0)) 
                    && 
                    ($('#gridTaskTypeSelect').val() == "all" ? true : el.task_type_name == $('#gridTaskTypeSelect').val() || findIndexOf($('#gridTaskTypeSelect').val(), "task_type_name", el.children) >= 0)
                    &&
                    ($('#gridSearch').val().length == 0 ? true:((el.task_name.toLowerCase().indexOf($('#gridSearch').val().toLowerCase()) > -1) || (findIndexOfContains($('#gridSearch').val().toLowerCase(), "task_name", el.children) > 0)))
                    ;
            });
            generateGrid('#mainContainer', columns, filteredData, GRID_HEIGHT);
        });

        $('#gridSearch').on('keyup', function () {
            animateGrid = false;
            if($('#gridSearch').val()=="discolights"){
                filteredData = data.filter(function(el){return true;});
                generateGrid('#mainContainer', columns, filteredData, GRID_HEIGHT);
                setInterval(function(){$('#pageHeaderContainer,#pageHeaderContainer *,.trigger_task,.delete_ico,.subscribe,.custom_handler,.btn-blue,#showDependancy,#taskChart *,#taskChart,#slidingPanelBtn,.ivpCornerImage,#slidingContent #tasksContainer .tasks.selected ,.popup,.cross').each(function(i,e){$(e).css('-webkit-filter','hue-rotate('+Math.floor((Math.random()*360)+1)+'deg)')})}, 1000);
            }
            else if($('#gridSearch').val()=="andweallfalldown"){ 
                filteredData = data.filter(function(el){return true;});
                generateGrid('#mainContainer', columns, filteredData, GRID_HEIGHT);$('#tBody > *').each(function(i,e){$(e).css('position','relative').css('z-index',"100000" ).animate({top: "80%",left:((Math.random()*100)-50)+"%"}, 4000,"easeOutBounce").draggable();})
            }
            else if($('#gridSearch').val()=="credits"){
                filteredData = data.filter(function(el){return true;});
                generateGrid('#mainContainer', columns, filteredData, GRID_HEIGHT);$('#lightbox').show();$('.wrap').show();$('.wrap div').each(function(i,e){setTimeout(function(){$(e).show("pulsate",{},800);},i*100+500);})
            }
            else if ($('#gridSearch').val().length > 0) {
                filteredData = data.filter(function (el) {
                return ((el.task_name.toLowerCase().indexOf($('#gridSearch').val().toLowerCase()) > -1) || (findIndexOfContains($('#gridSearch').val().toLowerCase(), "task_name", el.children) > 0)) && (($('#gridModuleSelect').val() == "all") ? true : (el.module_name == $('#gridModuleSelect').val()) || findIndexOf($('#gridModuleSelect').val(), "module_name", el.children) >= 0) && ($('#gridTaskTypeSelect').val() == "all" ? true : el.task_type_name == $('#gridTaskTypeSelect').val() || findIndexOf($('#gridTaskTypeSelect').val(), "task_type_name", el.children) >= 0);});
                generateGrid('#mainContainer', columns, filteredData, GRID_HEIGHT);
            }
            else {
                filteredData = data.filter(function (el) {
                    return (($('#gridModuleSelect').val() == "all") ? true : (el.module_name == $('#gridModuleSelect').val()) || findIndexOf($('#gridModuleSelect').val(), "module_name", el.children) >= 0) && ($('#gridTaskTypeSelect').val() == "all" ? true : el.task_type_name == $('#gridTaskTypeSelect').val() || findIndexOf($('#gridTaskTypeSelect').val(), "task_type_name", el.children) >= 0);
                });
                generateGrid('#mainContainer', columns, filteredData, GRID_HEIGHT);
            }
            animateGrid = true;
        });
}

function initChainableTasks() {
    var tasksData = Array();
    for (var module in chainableTasks) {
        for (var i = 0; i < chainableTasks[module].length; i++) {
            tasksData.push({ task_name: chainableTasks[module][i].task_name, task_type_name: chainableTasks[module][i].task_type_name, module_name: module, task_summary_id: chainableTasks[module][i].task_summary_id });
        }
    }
    fillTasksContainer(tasksData);
    $('#tasksContainerModuleSelect, #tasksContainerTaskTypeSelect').on('change', function () {
        var filteredTasksData = tasksData.filter(function (el) {
            return (($('#tasksContainerModuleSelect').val() == "all") ? true : (el.module_name == $('#tasksContainerModuleSelect').val())) 
            && 
            ($('#tasksContainerTaskTypeSelect').val() == "all" ? true : el.task_type_name == $('#tasksContainerTaskTypeSelect').val())
            &&
            ($('#tasksContainerSearch').val().length == 0 ? true : (el.task_name.toLowerCase().indexOf($('#tasksContainerSearch').val().toLowerCase()) > -1))
            ;
        });
        fillTasksContainer(filteredTasksData);
    });
    $('#tasksContainerSearch').on('keyup', function () {
        if ($('#tasksContainerSearch').val().length > 0) {
            var filteredTasksData = tasksData.filter(function (el) {
                return (el.task_name.toLowerCase().indexOf($('#tasksContainerSearch').val().toLowerCase()) > -1) && (($('#tasksContainerModuleSelect').val() == "all") ? true : (el.module_name == $('#tasksContainerModuleSelect').val())) && ($('#tasksContainerTaskTypeSelect').val() == "all" ? true : el.task_type_name == $('#tasksContainerTaskTypeSelect').val()); ;
            });
            fillTasksContainer(filteredTasksData);
        }
        else {
            var filteredTasksData = tasksData.filter(function (el) {
                return (($('#tasksContainerModuleSelect').val() == "all") ? true : (el.module_name == $('#tasksContainerModuleSelect').val())) && ($('#tasksContainerTaskTypeSelect').val() == "all" ? true : el.task_type_name == $('#tasksContainerTaskTypeSelect').val());
            });
            fillTasksContainer(filteredTasksData);
        }
    });
}

//overload for default window.alert   
function alert(msg)
{
    $('#dialogMsg').html(msg);
    $('#dialog').dialog({ modal: true, buttons: {Ok: function () {$(this).dialog("close");}}}); 
    $('#dialog').dialog('option', 'title', 'Common Task Manager');
}

function alert(msg, e)
{
    $('#dialogMsg').html(msg+"</br> Error details:<div style='max-height: 300px;max-width: 600px;overflow: auto;background-color: whitesmoke;border: 1px solid rgb(158, 158, 158);margin-top: 10px;'>"+e.responseText+"</div>");
    $('#dialog').dialog({ modal: true, buttons: {Ok: function () {$(this).dialog("close");}}}); 
    $('#dialog').dialog('option', 'title', 'Common Task Manager');
}