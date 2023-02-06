<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskManagerControl.ascx.cs"
    Inherits="com.ivp.secm.ui.TaskManagerControl" %>
<asp:HiddenField ID="servicePath" runat="server" />
<asp:HiddenField ID="loginName_hf" runat="server" />
<asp:HiddenField ID="clientName_hf" runat="server" />

<div id="ctm-container">
    <div class="wrap" style="display: none; position: absolute; top: 40%; left: 45%; z-index: 100000">
        <div class="cube">
            <div class="front">
            </div>
            <div class="back">
                <div style="display: block; padding: 10px; width: 90%; height: 90%;">
                    UI Design:Utsav Beri
                </div>
            </div>
            <div class="top">
            </div>
            <div class="bottom">
            </div>
            <div class="left">
                <div style="display: block; padding: 10px; width: 90%; height: 90%;">
                    WCF Code Design:Rahul Gupta & Utsav Beri
                </div>
            </div>
            <div class="right">
                <div style="display: block; padding: 10px; width: 90%; height: 90%;">
                    Team Lead: Rahul Gupta
                </div>
            </div>
        </div>
    </div>
    <div id="lastRunStatusesPopup" class="ctm-popup">
        <div class="popupBody">
            <div class="cross">
            </div>
            <h3>Last Run Task Status</h3>
            <div id="lastRunTaskStatusTable" style="width: 951px;">
                <div class="thead">
                    <div class="ctm-cell task_name">
                        Task Name
                    </div>
                    <div class="ctm-cell module_name">
                        Module Name
                    </div>
                    <div class="ctm-cell start_time">
                        Start Time
                    </div>
                    <div class="ctm-cell end_time">
                        End Time
                    </div>
                    <div class="ctm-cell status">
                        Status
                    </div>
                    <div class="ctm-cell task_log">
                        Log
                    </div>
                </div>
                <div class="tbody" style="max-height: 160px; width: 100%; overflow-y: auto; overflow-x: hidden; margin-bottom: 15px">
                </div>
            </div>
        </div>
    </div>
    <div id="slidingPanel">
        <div class="cross SMTaskGroupSetup_custom_cross">
            <span style="font-size: 14px; color: #43D9C6;">Cancel</span>
        </div>
        <div id="slidingContent" style="overflow: hidden">
            <div class="ivpCornerImage">
            </div>

            <div class="SMTaskGroupSetup_NewGroupNameContainer">
                <input type="text" id="chainNameTxt" placeholder="UNTITLED GROUP" onkeypress="this.style.width = ((this.value.length + 1) * 8) + 'px';">
            </div>
            <a class="btn" id="addChainBtn">Save</a>
            <%--<a class="btn" id="updateChainBtn" style="display: none;">Update Chain</a>--%>
            <a class="btn" id="updateChainBtn" style="display: none;">Save</a>

            <div class="SMTaskGroupSetup_HeaderContainer" style="text-align: center;">
                <div id="manualRadio_Div" style="display: inline-block; cursor: pointer; height: 20px; border-bottom: 2px solid white; color: #48a3dd !important; font-family: Lato; font-size: 14px; border-bottom: 2px solid #48a3dd; margin-right: 3%;">
                    <label style="cursor: pointer">
                        <input type="radio" name="triggerTypeRadioGroup" id="manualRadio" value="Manual" checked="checked" style="display: none;">Task Configuration
                    </label>
                </div>
                <div id="scheduledRadio_Div" style="display: inline-block; height: 20px; border-bottom: 2px solid white; cursor: pointer; font-family: Lato; font-size: 14px;">
                    <label style="cursor: pointer">
                        <input id="scheduledRadio" type="radio" name="triggerTypeRadioGroup" value="Scheduled" style="display: none;">Scheduling Information
                    </label>
                </div>
                <%--<a class="btn" id="addChainBtn">Save</a>--%>
            </div>

            <div id="toolbarNew_toggle" class="toolbar" style="padding: 23px; text-align: center; padding-top: 0px;">
                <div class="customSelectContainer">
                    <select id="tasksContainerModuleSelect" style="width: 219px; height: 20px;">
                        <option value="all">Module Name</option>
                    </select>
                </div>
                <div class="customSelectContainer">
                    <select id="tasksContainerTaskTypeSelect" style="width: 219px; height: 20px;">
                        <option value="all">Task Type</option>
                    </select>
                </div>
                <i class="fa fa-search notify-icon" style="display: inline-block; bottom: 12px; left: 15px; position: relative; color: #a9a9a9;"></i>
                <input id="tasksContainerSearch" type="text" placeholder="Search by task name..."
                    style="" />
            </div>



            <div id="left1_left2_container">
                <div id="left1">
                    <div id="selectAllTasks">
                    </div>
                    <h3 style="margin-top: 0px; font-size: 1.1em; font-weight: normal">Available Tasks</h3>
                    <div id="tasksContainer">
                    </div>
                </div>
                <div id="left2">
                    <div id="removeSelectedTasks">
                    </div>
                    <h3 style="margin-top: 0px; font-size: 1.1em; font-weight: normal">Selected Tasks</h3>
                    <div id="selectedTasksContainer">
                    </div>
                </div>
            </div>
            <div id="left3" style="padding: 0px 0px 0px 50px; width: 95%;">
                <h3 style="font-size: 1.1em; display: none;">Task Group Name</h3>
                <%--<input type="text" id="chainNameTxt" style="margin-left: 0px; width: 197px; font-size: 13px; padding: 0px 0px 0px 3px;" />--%>
                <%--<h3 style="font-size: 1.1em;">Calendar Name</h3>--%>
                <span style="font-size: 12px;">Calendar Name : </span>
                <select id="calendarSelect" style="width: 205px;">
                    <option value="0">Select Calendar</option>
                </select>
                <h3 style="padding-top: 9px; display: none;">Chain Instance wait(in mins)</h3>
                <input type="text" id="chainSecondInstanceWait" style="margin-left: 0px; width: 197px; font-size: 13px; padding: 0px 0px 0px 3px; margin-bottom: 7px; display: none;" />

                <%--<input id="manualRadio" type="radio" name="triggerTypeRadioGroup" value="Manual"
                    checked="checked" />
                Manual
                <input id="scheduledRadio" type="radio" name="triggerTypeRadioGroup" value="Scheduled" />
                Scheduled<br />--%>
            </div>
            <div id="left4" style="width: 16%; top: 162px; display: none">
                <%--<a class="btn" id="updateChainBtn" style="float: right">Update Chain</a>--%>
            </div>
            <div id="deletePlaceholder" title="Drag selected tasks here to delete.." style="display: none;">
                <div id="deletePlaceholderBg" style="position: absolute; width: 100%; height: 100%;">
                </div>
            </div>
        </div>
        <div id="schedulingInfo" style="overflow: hidden;">
            <div class="ivpCornerImage" style="-webkit-filter: hue-rotate(140deg);">
            </div>
            <div class="left1" style="width: 34em; padding-right: 10%; padding-left: 6%;">
                <table border="0px" cellspacing="15" style="border-collapse: collapse;">
                    <col width="35%" />
                    <tr>
                        <td class="SMTaskGroupSetup_shadedLabelSpan">Recurrence Type
                        </td>

                        <td data-reccurencetypetd>
                            <input id="recurringRadio" type="radio" name="recurrenceTypeRadioGroup" value="recurring"
                                checked="checked" onclick="javascript:
    $('#dailyRecurrenceRadio').removeAttr('disabled');
    $('#weeklyRecurrenceRadio').removeAttr('disabled');
    $('#monthlyRecurrenceRadio').removeAttr('disabled');
    $('#intervalTxt').removeAttr('disabled');
    $('#numberOfRecurrenceTxt').removeAttr('disabled');
    $('#neverEndJobCheckbox').removeAttr('disabled');
    $('#intervalRecurrenceTxt').removeAttr('disabled');
    $('#endDateTxt').removeAttr('disabled');
    $('#endDateTxt').datepicker('enable');" />Recurring
                            <input id="nonRecurringRadio" type="radio" name="recurrenceTypeRadioGroup" value="non-recurring"
                                onclick="javascript:
    $('#dailyRecurrenceRadio').attr('disabled', 'disabled');
    $('#weeklyRecurrenceRadio').attr('disabled', 'disabled');
    $('#monthlyRecurrenceRadio').attr('disabled', 'disabled');
    $('#intervalTxt').attr('disabled', 'disabled');
    $('#numberOfRecurrenceTxt').attr('disabled', 'disabled');
    $('#neverEndJobCheckbox').attr('disabled', 'disabled');
    $('#intervalRecurrenceTxt').attr('disabled', 'disabled');
    $('#endDateTxt').attr('disabled', 'disabled');
    $('#endDateTxt').datepicker('disable');
                            " />
                            Non-Recurring
                        </td>
                    </tr>
                    <tr>
                        <td class="SMTaskGroupSetup_tdStyle">
                            <label class="SMTaskGroupSetup_shadedLabelSpan">Start Date</label>
                        </td>
                        <td class="SMTaskGroupSetup_tdStyle">
                            <%--<input class="maskedDate datepicker" id="startDateTxt" type="text" data-required="true"
                                data-dategtnow />--%>
                            <input id="startDateTxt" type="text" class="SMTaskNameSetup_DateAttributeText SMTaskGroupSetup_tdInputStyle" style="width: 100%;" data-dategtnow />
                        </td>
                    </tr>
                    <tr>
                        <td class="SMTaskGroupSetup_tdStyle">
                            <label class="SMTaskGroupSetup_shadedLabelSpan">Recurrence Pattern</label>
                        </td>
                        <td class="SMTaskGroupSetup_tdStyle">
                            <input type="radio" id="dailyRecurrenceRadio" name="recurrencePatternRadioGroup"
                                value="daily" onclick="javascript: $('.dayOfWeekRow').slideUp();" />Daily
                            <input id="weeklyRecurrenceRadio" type="radio" name="recurrencePatternRadioGroup"
                                value="weekly" onclick="javascript: $('.dayOfWeekRow').show();" />Weekly
                            <input id="monthlyRecurrenceRadio" type="radio" name="recurrencePatternRadioGroup"
                                value="monthly" onclick="javascript: $('.dayOfWeekRow').slideUp();" />Monthly
                        </td>
                    </tr>
                    <tr class="dayOfWeekRow" style="display: none;">
                        <td>Days Of Week
                        </td>
                        <td>
                            <input type="checkbox" name="dayOfWeekCheckboxGroup" value="1" id="Checkbox1" />Sun
                            <input type="checkbox" name="dayOfWeekCheckboxGroup" value="2" id="Checkbox2" />Mon
                            <input type="checkbox" name="dayOfWeekCheckboxGroup" value="4" id="Checkbox3" />Tue
                            <input type="checkbox" name="dayOfWeekCheckboxGroup" value="8" id="Checkbox4" />Wed<br />
                            <input type="checkbox" name="dayOfWeekCheckboxGroup" value="16" id="Checkbox8" />Thur
                            <input type="checkbox" name="dayOfWeekCheckboxGroup" value="32" id="Checkbox9" />Fri
                            <input type="checkbox" name="dayOfWeekCheckboxGroup" value="64" id="Checkbox10" />Sat
                        </td>
                    </tr>
                    <tr>
                        <td id="intervalLbl" class="SMTaskGroupSetup_tdStyle">
                            <label class="SMTaskGroupSetup_shadedLabelSpan">Interval</label>
                        </td>
                        <td class="SMTaskGroupSetup_tdStyle">
                            <input id="intervalTxt" class="SMTaskGroupSetup_tdInputStyle" data-required="true" style="width: 100%;" />
                        </td>
                    </tr>

                </table>
            </div>
            <div class="left2" style="width: 34%;">
                <table border="0" cellspacing="15" style="border-collapse: collapse;">
                    <tr>
                        <td style="width: 30%;">
                            <label class="SMTaskGroupSetup_shadedLabelSpan">End date</label>
                        </td>
                        <td>
                            <%--  <input class="maskedDate datepicker" id="endDateTxt" type="text" data-required="true"
                                data-dategtstartdate />--%>
                            <input id="endDateTxt" type="text" class="SMTaskNameSetup_DateAttributeText SMTaskGroupSetup_tdInputStyle" style="width: 100%;" data-dategtstartdate />

                        </td>
                    </tr>
                    <tr>
                        <td class="SMTaskGroupSetup_tdStyle">
                            <label class="SMTaskGroupSetup_shadedLabelSpan">Number of Recurrences</label>
                        </td>
                        <td class="SMTaskGroupSetup_tdStyle">
                            <input class="SMTaskGroupSetup_tdInputStyle" id="numberOfRecurrenceTxt" data-required="true" style="width: 100%;" />
                        </td>
                    </tr>
                    <tr>
                        <td class="SMTaskGroupSetup_tdStyle">
                            <label class="SMTaskGroupSetup_shadedLabelSpan">Start Time (24 hr format)</label>
                        </td>
                        <td class="SMTaskGroupSetup_tdStyle">
                            <input class="maskedTime SMTaskGroupSetup_tdInputStyle" id="startTimeTxt" data-timegtnow data-required="true" style="width: 100%;" />
                        </td>
                    </tr>

                    <tr>
                        <td class="SMTaskGroupSetup_tdStyle">
                            <label class="SMTaskGroupSetup_shadedLabelSpan">TimeInterval of Recurrence(in mins)</label>
                        </td>
                        <td class="SMTaskGroupSetup_tdStyle">
                            <input id="intervalRecurrenceTxt" class="SMTaskGroupSetup_tdInputStyle" data-required="true" style="width: 100%;" />
                        </td>
                    </tr>
                    <tr>
                        <td class="SMTaskGroupSetup_tdStyle">
                            <input type="checkbox" name="neverEndJobCheckbox" id="neverEndJobCheckbox" onclick='javascript: if (this.checked) {
        $("#endDateTxt").attr("disabled", "disabled");
        $("#endDateTxt").datepicker("disable");
    }
    else {
        $("#endDateTxt").removeAttr("disabled");
        $("#endDateTxt").datepicker("enable");
    }' /><span style="bottom: 2px; position: relative;">Never End Job</span>
                        </td>

                    </tr>
                </table>
                <a class="btn" id="schedulingInfoScheduleBtn" style="float: left; margin: 20px; display: none;">Schedule</a>
                <a id="schedulingInfoCancelBtn" class="btn" style="float: left; margin: 20px; display: none;" onclick="javascript:$('#manualRadio').prop('checked',true);$('#schedulingInfo').slideUp();">Cancel</a>
            </div>
        </div>
    </div>
    <div id="lightbox">
    </div>
    <div id="preLoader">
    </div>
    <%--<div class="triggerOptionBtn" style = 'display:none'>
       <img alt='' src='' style="content: url('css/images/CTMimages/gear blue.png');width: 15px;" />
    </div>--%>
    <%--<div class = "triggerOptions"  style = 'display:none'>
        <div class="triggerOption">All tasks in chain</div>
        <div class="triggerOption">This task only</div>
    </div>--%>
    <div id="configureTasksPopup" style="position: fixed;" class="ctm-popup">
        <div id='taskDetailTooltip' onclick='event.stopPropagation();'>
        </div>
        <div id='taskStatusDetailTooltip'>
        </div>
        <div class="popupBody" id="popupBodyExpand">
            <div class="cross" style="top: 12px;" id="resetManual_Close">
                <span style="font-size: 14px; color: #43D9C6;">Cancel</span>
            </div>
            <div id="showDependancy" style="">
                Dependency Chart
            </div>
            <h2 id="taskNameHeading" style="margin-bottom: 10px; font-size: 16px; color: #626262; font-family: oswald !important; width: 77%; display: inline-block; margin-top: 0px; font-weight: normal;">
                <span style="color: rgb(51, 51, 51); font-size: 18px;">Configure Tasks</span>
            </h2>
            <a class="btn-orange-new" id="configureSaveBtn" style="float: right">Save</a>

            <div style="margin-bottom: 15px">
                <div style="text-align: right; padding: 0px 3px 0px 0px; display: none;">
                    <span id="triggerAsOfDateContainer" style="border-right: 1px solid  rgb(150, 150, 150); padding-right: 3px; margin-right: 3px;">
                        <label for="ctm_trigger_as_of_date">
                            <strong>As of Date </strong>
                        </label>
                        <input type="text" id="ctm_trigger_as_of_date" />
                        <select id="ctm_trigger_as_of_date_info_option">
                            <option value='None' selected="selected">None</option>
                            <option value='Today'>Today</option>
                            <option value="Last Business Day">Last Business Day</option>
                            <option value="Last Business Day">Last Business Day of Month</option>
                            <option value="Last Business day of the previous week">Last Business day of the previous
                                week</option>
                            <option value='Last Business day of the previous month'>Last Business day of the previous
                                month</option>
                            <option value="Last Business day of the previous Quarter">Last Business day of the previous
                                Quarter</option>
                            <option value="First Business day of the current week">First Business day of the current
                                week</option>
                            <option value='First Business day of the current month'>First Business day of the current
                                month</option>
                            <option value="First Business day of the current Quarter">First Business day of the
                                current Quarter</option>
                        </select>
                        <input type="text" id="ctm_trigger_as_of_date_info_value" style="width: 30px" />
                    </span><strong>Allow Parallel </strong>
                    <input type="checkbox" id="allowParallelCheckbox" onclick="javascript: if (this.checked) { $('#maxNoOfParallelContainer').show(); } else { $('#maxNoOfParallelContainer').hide(); }" />
                    <span id="maxNoOfParallelContainer" style="display: none;">Max. no of parallel instances
                        allowed
                        <input style="width: 20px" id="maxParallelInstancesAllowed" /></span> <strong style="border-left: 1px solid rgb(150, 150, 150); padding-left: 6px;">Filewatcher </strong>: Folder Location
                    <input id="filewatcherFolderLocation" />
                    File Regex
                    <input id="filewatcherFileRegex" />
                </div>
                <%--                <div id="taskChart">
                </div>--%>
                <div style="text-align: center; height: 30px;">
                    <div id="manualRadio2_Div" style="display: inline-block; cursor: pointer; height: 20px; border-bottom: 2px solid white; color: #48a3dd !important; font-family: Lato; font-size: 14px; border-bottom: 2px solid #48a3dd; margin-right: 3%;">
                        <label style="cursor: pointer">
                            <input type="radio" name="group1" id="manualRadio2" value="Manual" checked="checked" onclick="javascript: $('#schedulingInfo2SlideBtn, #schedulingInfo2').hide(); $('#configureTasksTable').show();" style="display: none;">Task Configuration
                        </label>
                    </div>
                    <div id="scheduledRadio2_Div" style="display: inline-block; height: 20px; border-bottom: 2px solid white; cursor: pointer; font-family: Lato; font-size: 14px;">
                        <label style="cursor: pointer">
                            <input id="scheduledRadio2" type="radio" name="group1" value="Scheduled" onclick="javascript: $('#schedulingInfo2').show(); $('#configureTasksTable').hide()" style="display: none;">Scheduling Information
                        </label>
                    </div>
                    <%--<a class="btn-orange-new" id="configureSaveBtn" style="float: right">Save</a>--%>
                </div>
                <div id="configureTasksTable" style="width: 1260px;">
                    <div class="thead">
                        <div class="dependencyCheckbox ctm-cell" style="display: none; width: 20px;">
                        </div>
                        <div class="ctm-cell CTMTaskName">
                            Task Name
                        </div>
                        <div class="ctm-cell CTMDependentOn">
                            Dependent</br>On
                        </div>
                        <div class="ctm-cell CTMTimeOut">
                            Timeout</br>(in mins)
                        </div>
                        <div class="ctm-cell CTMProceedOnFail">
                            Proceed</br>on Fail
                        </div>
                        <div class="ctm-cell CTMIsMuted">
                            Is Muted
                        </div>
                        <div class="ctm-cell CTMReRunOnFail">
                            Rerun on</br>Fail
                        </div>
                        <div class="ctm-cell retry_fail_duration" title="Retry Duration(in mins)">
                            Retry Duration</br>(in mins)
                        </div>
                        <div class="ctm-cell CTMNumberOfRetries">
                            Number of</br>Retries
                        </div>
                        <div class="ctm-cell CTMOnFailRunTask">
                            On Fail</br>Run Task
                        </div>
                        <div class="ctm-cell CTMTaskTimeOut">
                            Time out</br>(in mins)
                        </div>
                        <div class="ctm-cell CTMTaskSecondInstanceWait">
                            Task Instance wait</br>(in mins)
                        </div>
                        <div class="ctm-cell CTMTaskWaitSubscription">
                            Task Wait</br>Subscription
                        </div>
                    </div>
                    <div class="tbody" style="width: 100%; overflow-y: auto; overflow-x: hidden; margin-bottom: 15px">
                    </div>
                    <div style="margin-top: 3%;">
                        <span style="display: inline-block;">Calendar Type : </span>
                        <select id="calendarSelect2" style="width: 200px;">
                            <option value="0">Select Calendar</option>
                        </select>
                        <a class="btn" id="editTasks" style="float: right; cursor: pointer;">Edit Tasks</a>
                    </div>
                </div>
            </div>
            <div id="schedulingInfo2SlideBtn" style="display: none;" onclick="javascript:$('#schedulingInfo2').slideToggle(); var isScheduled = ($('#schedulingInfo2').css('display') != 'none'); resizeTaskManagerPopup(isScheduled);">
                SchedulingInfo ▼
            </div>
            <div id="schedulingInfo2" style="overflow: hidden; height: 200px; left: 5%;">
                <div class="ivpCornerImage" style="-webkit-filter: hue-rotate(140deg);">
                </div>
                <div class="left1" style="width: 42em;">
                    <table border="0px" cellspacing="15" style="border-collapse: collapse;">
                        <col width="35%" />
                        <tr>
                            <td class="SMTaskGroupSetup_shadedLabelSpan">Recurrence Type
                            </td>
                            <td data-reccurencetypetd2>
                                <input id="recurringRadio2" type="radio" name="recurrenceTypeRadioGroup2" value="recurring"
                                    checked="checked" onclick="javascript:
    $('#dailyRecurrenceRadio2').removeAttr('disabled');
    $('#weeklyRecurrenceRadio2').removeAttr('disabled');
    $('#monthlyRecurrenceRadio2').removeAttr('disabled');
    $('#intervalTxt2').removeAttr('disabled');
    $('#numberOfRecurrenceTxt2').removeAttr('disabled');
    $('#neverEndJobCheckbox2').removeAttr('disabled');
    $('#intervalRecurrenceTxt2').removeAttr('disabled');
    $('#endDateTxt2').removeAttr('disabled');
    $('#endDateTxt2').datepicker('enable');
    $('#dailyRecurrenceRadio2').trigger('click');
                                            " />
                                Recurring
                                <input id="nonRecurringRadio2" type="radio" name="recurrenceTypeRadioGroup2" value="non-recurring"
                                    onclick="javascript:
    $('#dailyRecurrenceRadio2').attr('disabled', 'disabled');
    $('#weeklyRecurrenceRadio2').attr('disabled', 'disabled');
    $('#monthlyRecurrenceRadio2').attr('disabled', 'disabled');
    $('#intervalTxt2').attr('disabled', 'disabled');
    $('#numberOfRecurrenceTxt2').attr('disabled', 'disabled');
    $('#neverEndJobCheckbox2').attr('disabled', 'disabled');
    $('#intervalRecurrenceTxt2').attr('disabled', 'disabled');
    $('#endDateTxt2').attr('disabled', 'disabled');
    $('#endDateTxt2').datepicker('disable');
                                            " />
                                <!--
                                             $('#dailyRecurrenceRadio').attr('disabled','disabled');
                                $('#weeklyRecurrenceRadio').attr('disabled','disabled');
                                $('#monthlyRecurrenceRadio').attr('disabled','disabled');
                                $('#intervalTxt').attr('disabled','disabled');
                                $('#numberOfRecurrenceTxt').attr('disabled','disabled');
                                $('#neverEndJobCheckbox').attr('disabled','disabled');
                                $('#intervalRecurrenceTxt').attr('disabled','disabled');
                                $('#endDateTxt').attr('disabled','disabled');
                                $('#endDateTxt').datepicker('disable');
                                            -->
                                Non-Recurring
                            </td>
                        </tr>
                        <tr>
                            <td class="SMTaskGroupSetup_tdStyle">
                                <label class="SMTaskGroupSetup_shadedLabelSpan">Start Date</label>
                            </td>
                            <td class="SMTaskGroupSetup_tdStyle">
                                <%--  <input class="maskedDate datepicker" id="startDateTxt2" type="text" data-required="true"
                                    data-dategtnow />--%>
                                <input id="startDateTxt2" type="text" class="SMTaskNameSetup_DateAttributeText SMTaskGroupSetup_tdInputStyle" data-dategtnow />
                            </td>
                        </tr>
                        <tr>
                            <td class="SMTaskGroupSetup_tdStyle">
                                <label class="SMTaskGroupSetup_shadedLabelSpan">Recurrence Pattern</label>
                            </td>
                            <td class="SMTaskGroupSetup_tdStyle">
                                <input id="dailyRecurrenceRadio2" type="radio" name="recurrencePatternRadioGroup2"
                                    value="daily" checked="checked" onclick="javascript: $('.dayOfWeekRow2').slideUp();" />Daily
                                <input id="weeklyRecurrenceRadio2" type="radio" name="recurrencePatternRadioGroup2"
                                    value="weekly" onclick="javascript: $('.dayOfWeekRow2').show();" />Weekly
                                <input id="monthlyRecurrenceRadio2" type="radio" name="recurrencePatternRadioGroup2"
                                    value="monthly" onclick="javascript: $('.dayOfWeekRow2').slideUp();" />Monthly
                            </td>
                        </tr>
                        <tr class="dayOfWeekRow2" style="display: none;">
                            <td>Days Of Week
                            </td>
                            <td>
                                <input type="checkbox" name="dayOfWeekCheckboxGroup2" value="1" id="Checkbox5" />Sun
                                <input type="checkbox" name="dayOfWeekCheckboxGroup2" value="2" id="Checkbox6" />Mon
                                <input type="checkbox" name="dayOfWeekCheckboxGroup2" value="4" id="Checkbox7" />Tue
                                <input type="checkbox" name="dayOfWeekCheckboxGroup2" value="8" id="Checkbox11" />Wed<br />
                                <input type="checkbox" name="dayOfWeekCheckboxGroup2" value="16" id="Checkbox12" />Thur
                                <input type="checkbox" name="dayOfWeekCheckboxGroup2" value="32" id="Checkbox13" />Fri
                                <input type="checkbox" name="dayOfWeekCheckboxGroup2" value="64" id="Checkbox14" />Sat
                            </td>
                        </tr>
                        <tr>
                            <td class="SMTaskGroupSetup_tdStyle">
                                <label class="SMTaskGroupSetup_shadedLabelSpan">Interval</label>
                            </td>
                            <td class="SMTaskGroupSetup_tdStyle">
                                <input id="intervalTxt2" class="SMTaskGroupSetup_tdInputStyle" data-required="true" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="left2" style="width: 34%;">
                    <table border="0" cellspacing="15" style="border-collapse: collapse;">
                        <tr>
                            <td style="width: 30%;">
                                <label class="SMTaskGroupSetup_shadedLabelSpan">End date</label>
                            </td>
                            <td>
                                <%-- <input class="maskedDate datepicker" id="endDateTxt2" type="text" data-dategtstartdate
                                    data-required="true" /> --%>
                                <input id="endDateTxt2" type="text" class="SMTaskNameSetup_DateAttributeText SMTaskGroupSetup_tdInputStyle" style="width: 100%;" data-dategtstartdate />
                            </td>
                        </tr>
                        <tr>
                            <td class="SMTaskGroupSetup_tdStyle">
                                <label class="SMTaskGroupSetup_shadedLabelSpan">Number of Recurrences</label>
                            </td>
                            <td class="SMTaskGroupSetup_tdStyle">
                                <input id="numberOfRecurrenceTxt2" class="SMTaskGroupSetup_tdInputStyle" data-required="true" style="width: 100%" />
                            </td>
                        </tr>
                        <tr>
                            <td class="SMTaskGroupSetup_tdStyle">
                                <label class="SMTaskGroupSetup_shadedLabelSpan">Start Time (24 hr format)</label>
                            </td>
                            <td class="SMTaskGroupSetup_tdStyle">
                                <input class="maskedTime SMTaskGroupSetup_tdInputStyle" id="startTimeTxt2" data-timegtnow data-required="true" style="width: 100%" />
                            </td>
                        </tr>

                        <tr>
                            <td class="SMTaskGroupSetup_tdStyle">
                                <label class="SMTaskGroupSetup_shadedLabelSpan">TimeInterval of Recurrence(in mins)</label>
                            </td>
                            <td class="SMTaskGroupSetup_tdStyle">
                                <input class="SMTaskGroupSetup_tdInputStyle" id="intervalRecurrenceTxt2" data-required="true" style="width: 100%" />
                            </td>
                        </tr>
                        <tr>
                            <td class="SMTaskGroupSetup_tdStyle">
                                <input type="checkbox" name="neverEndJobCheckbox2" id="neverEndJobCheckbox2" onclick='javascript: if (this.checked) {
        $("#endDateTxt2").attr("disabled", "disabled");
        $("#endDateTxt2").datepicker("disable");
    }
    else {
        $("#endDateTxt2").removeAttr("disabled");
        $("#endDateTxt2").datepicker("enable");
    }' /><span style="bottom: 2px; position: relative;">Never End Job</span>
                            </td>
                        </tr>
                        <tr>
                            <td></td>
                            <td>
                                <div class="srmTaskManager_RemoveSchedulingInfo">
                                    <a class="btn" id="removeSchedulingInfobtn" style="float: right; cursor: pointer;">Remove Scheduling Info</a>
                                </div>
                            </td>
                        </tr>
                    </table>
                    <a class="btn" id="schedulingInfoScheduleBtn2" style="float: left; margin: 5px; display: none;">Schedule</a> <a id="schedulingInfoCancelBtn2" class="btn" style="float: left; margin: 5px; display: none;"
                        onclick="javascript:$('#manualRadio2').prop('checked',true);$('#schedulingInfo2').slideUp(); var isScheduled = ($('#schedulingInfo2').css('display') != 'none'); resizeTaskManagerPopup(isScheduled);">Cancel</a>
                </div>

            </div>
            <div style="padding: 6px 0px;">
                <%--                <span>Calendar Type : </span>
                <select id="calendarSelect2" style="width: 200px;">
                    <option value="0">Select Calendar</option>
                </select>--%>
                <%--Manual<input type="radio" name="group1" id="manualRadio2" value="Manual" checked="checked"
                    onclick="javascript: $('#schedulingInfo2SlideBtn, #schedulingInfo2').hide(); " />--%>
                <!-- resizeTaskManagerPopup(false); utsav wrong $('#schedulerForm2')[0].reset();" /-->
                <%-- Scheduled<input id="scheduledRadio2" type="radio" name="group1" value="Scheduled"
                    onclick="javascript: $('#schedulingInfo2').slideDown('slow', 'easeOutBounce');" />--%>
                <!--  resizeTaskManagerPopup(true); -->
                <span style="padding-left: 2px; display: none;">Chain Instance Wait(in mins)</span>
                <input type="text" id="chainSecondInstanceWaitUpdate" style="margin-left: 0px; width: 50px; font-size: 13px; padding: 0px 0px 0px 3px; display: none;" />
                <span style="padding-left: 2px; display: none;">Chain Wait Subscription</span>
                <div style="display: inline-block; vertical-align: top; display: none;">
                    <div id="inprogressSubscribersDiv" style="display: inline-block; vertical-align: top;">
                        <%--<select id="inprogressSubscribers" style="width: 200px;">
                </select>--%>
                    </div>
                </div>
                <%--<a class="btn-blue" id="configureSaveBtn" style="float: right">Save</a>--%>
                <%--<a class="btn" id="subscribeChain" style="float: right">Subscribe</a>--%>
                <%--<a class="btn" id="editTasks" style="float: right">Edit Tasks</a>--%>
            </div>
        </div>
    </div>
    <div id="content" style="padding: 3px 0px 5px 5px !important;">
        <div id="toolbar" class="toolbar">
            <div style="display: inline; margin-left: 33%; position: relative;">
                <%--<div class="customSelectContainer">--%>
                <select id="gridModuleSelect" style="padding: 2px; border-left: 0px; border-top: 0px; border-right: 0px; outline: none; background: #FFFFFF; font-size: 13px; border-bottom: solid 1px #333;">
                    <option value="all">All Module Names</option>
                </select>
                <%--</div>--%>
                <%--<div class="customSelectContainer">--%>
                <select id="gridTaskTypeSelect" style="padding: 2px; margin-left: 4%; border-left: 0px; border-top: 0px; border-right: 0px; outline: none; background: #FFFFFF; font-size: 13px; border-bottom: solid 1px #333;">
                    <option value="all">All Task Types</option>
                </select>
                <select id="gridTaskStatusSelect" style="padding: 2px; margin-left: 2%; display: none;">
                    <option value="all">Select Last Task Status</option>
                    <option value="PASSED">Passed</option>
                    <option value="FAILED">Failed</option>
                    <option value="INPROGRESS">Inprogress</option>
                </select>
            </div>
            <%--</div>--%>
            <%--<input id="gridSearch" placeholder="Search by task name.." />--%>
            <%-- <div id="slidingPanelBtn" onclick="javascript:$('#left3').show();$('#left4').hide();
                    resetAddChainForm();
            $('#lightbox').toggle();$('#slidingPanel').animate({'height':'341px'}).toggle();$('#slidingContent').animate({'height':'341px'}).toggle()">
                <i class="fa fa-plus" aria-hidden="true" style="top: 2px; position: relative;"></i>
            </div>--%>
            <div style="display: inline-block; margin-left: 4%; bottom: 12px; position: relative;">
                <i class="fa fa-search notify-icon" style="display: inline-block; right: 5px; position: relative; color: #a9a9a9; top: 2px;"></i>
                <input id="gridSearch" placeholder="Search by group name or task name..." class="placeholder" style="display: inline-block;">
            </div>
            <a class='btn' id="muteBtn" style="display: inline-block; padding: 6px 1px; cursor: pointer; position: relative; z-index: 1; border: none; display: none;">Mute</a>
            <a class='btn' id="unmuteBtn" style="display: inline-block; padding: 6px 1px; cursor: pointer; position: relative; z-index: 1; border: none; display: none">Un-Mute</a>
            <%--  <div id = "ctm_trigger_as_of_date_container">
            <label for="ctm_trigger_as_of_date" >As of Date </label><input type = "text" id = "ctm_trigger_as_of_date" />
        </div>--%>
        </div>
        <%--        <div id="ctm-gridWrapper">
            <div id="mainContainer" style="position: relative;">
            </div>
        </div>--%>
    </div>
    <div id="TaskBindingAreaContainer">
        <div id="TaskBindingArea">
            <div id="SMTaskGroupContainer">
                <div class="SMTaskGroupSetup_Scrollable">
                    <div class="SMTaskGroupSetup_TaskGroups">
                        <div class="SMTaskGroupSetup_Header">
                            <div class="SMTaskGroupSetup_GroupWrapper">
                                GROUPS
                            </div>
                            <div id="slidingPanelBtn">
                                <i class="fa fa-plus" aria-hidden="true" style="top: 2px; position: relative; right: 0.5px;"></i>
                            </div>
                        </div>
                        <div class="SMTaskGroupSetup_Group_not_found" data-bind="visible: filteredGroups().length == 0">
                            Group(s) Not Found
                        </div>
                        <div class="SMTaskGroupSetup_TaskGroupNames" data-bind="foreach: filteredGroups, visible: filteredGroups().length > 0">
                            <div class="SMTaskNameSetup_Group_Object" data-bind="click: $parent.onClickGroupItem, attr: { isSelected: ($parent.selectedGroup().chain_id() == chain_id()) ? 'true' : 'false' }">
                                <!-- #F5F5F5 -->
                                <div style="width: 90%; margin-left: 3%; display: inline-block;" data-bind="style: { backgroundColor: ($parent.selectedGroup().chain_id() == chain_id()) ? 'rgb(86, 107, 134)' : 'rgb(217, 229, 244)', color: ($parent.selectedGroup().chain_id() == chain_id()) ? 'white' : 'rgb(62, 77, 94)', borderRadius: '2px' }">
                                    <div class="SMTaskGroupSetup_GroupName" data-bind="text: chain_name">
                                    </div>
                                    <div style="display: inline-block;" data-bind="css: { SRMTaskManagerAdjustWidth: (next_scheduled_time() != null) }">
                                        <div class="SMTaskNameSetup_Next_Scheduled_Time" data-bind="visible: (next_scheduled_time() != null)">
                                            <div data-bind="css: { SRMTaskManagerScheduledSelected: $parent.selectedGroup().chain_id() == chain_id(), SRMTaskManagerScheduledNotSelected: $parent.selectedGroup().chain_id() != chain_id() }"></div>
                                            <div style="display: inline-block; padding-left: 2%;">
                                                <div class="SMTaskNameSetup_Next_Scheduled_Time_Text " data-bind="text: (next_scheduled_time() != null) ? ' Next Run Time ' : ''"></div>
                                                <div class="SMTaskNameSetup_Next_Scheduled_Actual_Time" data-bind="text: (next_scheduled_time() != null) ? next_scheduled_time() : ''"></div>
                                            </div>
                                        </div>
                                    </div>
                                    <div style="display: inline-block;" data-bind="css: { SRMTaskManagerAdjustWidth: (next_scheduled_time() == null) }">
                                        <div class="SMTaskNameSetup_Not_Scheduled_Time" data-bind="visible: (next_scheduled_time() == null)">
                                            <div data-bind="css: { SRMTaskManagerNotScheduledSelected: $parent.selectedGroup().chain_id() == chain_id(), SRMTaskManagerNotScheduledNotSelected: $parent.selectedGroup().chain_id() != chain_id() }"></div>
                                        </div>
                                    </div>
                                </div>
                                <div style="width: 3%; display: inline-block;">
                                    <div data-bind="visible: ($parent.selectedGroup().chain_id() == chain_id()), style: { color: ($parent.selectedGroup().chain_id() == chain_id()) ? 'rgb(86, 107, 134)' : '' }">
                                        <i class="fa fa-caret-right SMTaskGroupSetup_arrow" aria-hidden="true"></i>
                                    </div>
                                </div>
                            </div>
                            <%--<hr class="SMTaskGroupSetup_Hr_Bar" />--%>
                        </div>
                        <br />
                    </div>
                </div>

            </div>

            <div id="SMTaskNamesContainer">
                <div class="SMTaskNameSetup_TaskLevel_Contents">
                    <div class="SMTaskGroupSetup_Task_not_found" data-bind="visible: filteredGroups().length == 0">
                        Task(s) Not Found
                    </div>
                    <div class="SMTaskNameSetup_TaskGroupNameWrapper" data-bind="with: selectedGroup, visible: filteredGroups().length > 0">
                        <div class="SMTaskNameSetup_TaskGroupName" id="SMTaskNameSetup_Editable_TaskGroupName">
                            <input id="SMeditable" onkeypress="this.style.width = ((this.value.length + 1) * 8) + 'px';" data-bind="textInput: chain_name" readonly />
                        </div>
                        <%--<div class="SMTaskNameSetup_Next_Scheduled_Time" data-bind="text: next_scheduled_time() > 0 ? ' Next Scheduled Time : ' + next_scheduled_time() : null">
                    </div>--%>
                        <br />
                        <br />
                    </div>
                    <div class="SMTaskNameSetup_ChainLevel" id="SMTaskNameSetup_ChainLevel" data-bind="visible: filteredGroups().length > 0">
                        <div class="SMTaskNameSetup_ChainLevel_Trigger button">
                            Trigger
                        </div>
                        <div class="SMTaskNameSetup_ChainLevel_Subscribe" id="subscribeChain" data-bind="with: selectedGroup">
                            <i class="fa fa-envelope-o" aria-hidden="true" data-bind="style: { color: '#b3b3b3' }"></i>
                            <%--(subscribe_id() ? ((subscribe_id().toString() != '|||||') ? 'black' : '#b3b3b3') :--%>
                        </div>
                        <div class="SMTaskNameSetup_ChainLevel_Configure configure" data-bind="with: selectedGroup">
                            <i class="fa fa-pencil-square-o" id="SMTaskNameSetup_Configure_Chain" aria-hidden="true" data-bind="attr: { 'chain_id': chainIdentifier }"></i>
                        </div>
                        <div class="SMTaskNameSetup_ChainLevel_Delete" data-bind="with: selectedGroup">
                            <i class="fa fa-trash-o" id="SMTaskNameSetup_Delete_Chain" aria-hidden="true" data-bind="attr: { 'chain_id': chainIdentifier }"></i>
                        </div>
                    </div>
                </div>

                <div id="SMTaskNameSetup_Task_Contents" style="padding-left: 5%; padding-top: 1%;" data-bind="visible: filteredGroups().length > 0">
                    <div class="SMTaskNameSetup_Header_Contents">
                        <div class="SMTaskNameSetup_Module_Name">Module Name</div>
                        <div class="SMTaskNameSetup_Task_Name">Task Name</div>
                        <div class="SMTaskNameSetup_Task_Type">Task Type</div>
                        <%--<div class="SMTaskNameSetup_Trigger_Type">Trigger Type</div>--%>
                        <div class="SMTaskNameSetup_Trigger_Task"></div>
                        <div class="SMTaskNameSetup_Delete"></div>
                        <div class="SMTaskNameSetup_Subscribe"></div>
                        <div class="SMTaskNameSetup_Muted"></div>
                        <hr class="SMTaskNameSetup_header_hr">
                    </div>

                    <div class="SMTaskNameSetup_TaskNames_Contents">
                        <div class="SMTaskNameSetup_Scrollable">
                            <div class="SMTaskNameSetup_TaskNameContents">

                                <div style="display: inline-block; width: 100%" data-bind="with: selectedGroup, visible: filteredGroups().length > 0">

                                    <div class="SMTaskNameSetup_TaskNamesBinding" data-bind="foreach: children">
                                        <div class="SMTaskNameSetup_Parent_TaskContainer" data-bind="attr: { 'flow_id': flow_id, 'chain_id': chain_id, 'chain_name': chain_name, 'childrenRow': childrenRow() ? 'true' : 'false', 'task_name': task_name, 'moduleId': $parent.module_id, 'subscribe_id': subscribe_id() ? subscribe_id : '', 'is_muted': is_muted() ? 'true' : 'false' }, 'visible': isVisible ">
                                            <div class="SMTaskNameSetup_Module_Name_Data module_name" data-bind="text: module_name"></div>
                                            <div class="SMTaskNameSetup_Task_Name_Data task_name" data-bind="text: task_name, attr: { title: 'Chain_ID : ' + chain_id() + ', Flow_ID : ' + flow_id() + '  (' + task_name() + ')' }"></div>
                                            <div class="SMTaskNameSetup_Task_Type_Data task_type_name" data-bind="text: task_type_name"></div>
                                            <%--<div class="SMTaskNameSetup_Trigger_Type_Data trigger_type" data-bind="text: $parent.trigger_type"></div>--%>
                                            <div class="SMTaskNameSetup_Trigger_Manual_Task trigger_task button" style="border-radius: 50px; cursor: pointer; bottom: 2px; position: relative;">
                                                Trigger<span class="SMarrow" style="display: none">
                                                    <i class="fa fa-chevron-right SMTaskNameSetup_Trigger_Arrow" aria-hidden="true"></i>
                                                </span>
                                            </div>
                                            <div class="SMTaskNameSetup_Delete_Data delete_ico" data-bind="style: { opacity: childrenRow() ? '0.9' : '0', 'pointer-events': childrenRow() ? '' : 'none' }">
                                                <i class="fa fa-trash-o fa-lg" aria-hidden="true" style="cursor: pointer;"></i>
                                            </div>
                                            <div class="SMTaskNameSetup_Subscribe_Data subscribe" data-bind="style: { color: (subscribe_id() ? ((subscribe_id().toString() != '|||||') ? 'black' : '#b3b3b3') : '#b3b3b3') }">
                                                <i class="fa fa-envelope-o" aria-hidden="true" style="cursor: pointer; display: inline-block"></i>
                                            </div>
                                            <div class="SMTaskNameSetup_Mute_UnMute is_muted mute_unmute" data-bind="css: { 'muted': is_muted(), 'Un-muted': !is_muted() }">
                                                <i class="fa fa-power-off" aria-hidden="true" style="cursor: pointer; display: inline-block;"></i>
                                            </div>
                                            <hr style="opacity: 0.1;" />
                                        </div>
                                    </div>
                                </div>
                                <br />
                            </div>
                        </div>
                    </div>

                </div>
                <div class="SMTaskNameSetup_graph_Container" data-bind="with: selectedGroup, visible: filteredGroups().length > 0">
                    <%--<hr data-bind="visible: children().length > 1" class="SMTaskNameSetup_graph_hr" />--%>
                    <div id="taskChartContainer" data-bind="visible: children().length > 1">
                        <div id="taskChart" style="display: none;">
                            <%--<span id="SMTaskNameSetup_expand_Graph">view:more</span>--%>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>
    <div>

        <div class="SMTaskNameSetup_Outer_Div_Pointer">
            <div class="SMTaskNameSetup_Inner_Div_Pointer">
            </div>
        </div>
        <div class="taskTriggerDtd" targetid="">
            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td class="SMtriggerType" type="0">This task only
                    </td>
                </tr>
                <tr>
                    <td class="SMtriggerType" type="1">This task onwards
                    </td>
                </tr>
            </table>
        </div>
    </div>
</div>

<div id="dialog">
    <p id='dialogMsg'>
    </p>
</div>
<div id="customHandlerPopup" style="top: 35%; left: 35%;" class="ctm-popup">
    <div class="popupBody">
        <div class="cross">
        </div>
        <table cellpadding="10">
            <tr>
                <td>Assembly Name
                </td>
                <td>
                    <input />
                </td>
            </tr>
            <tr>
                <td>Class Name
                </td>
                <td>
                    <input />
                </td>
            </tr>
        </table>
        <div style="height: 20px">
            <a class="btn-blue" style="float: right">Save</a>
        </div>
    </div>
</div>
</div>
<input type="hidden" id="hdn_SetShortDateFormat" runat="server" clientidmode="Static" />
<input type="hidden" id="hdn_removeSchedulingInfobtn" runat="server" clientidmode="Static" value="0" />
<asp:Button ID="btnHdnPostBack" runat="server" Style="display: none;" />
<%--</div>--%>
