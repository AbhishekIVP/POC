<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMPolarisDownstreamConfig.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMPolarisDownstreamConfig" MasterPageFile="~/CommonUI/SRMMaster.Master" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolKit" %>


<asp:Content ID="contentMiddle" ContentPlaceHolderID="SRMContentPlaceHolderMiddle" runat="server">
    <link href="css/jquery-ui.min.css" rel="stylesheet" type="text/css" />
    <link href="css/SRMPolarisDownstreamConfig.css" rel="stylesheet" />
    <link href="css/SRMPolarisDownstreamScheduler.css" rel="stylesheet" />
    <link href="css/SMCss/SMSelect.css" rel="stylesheet" type="text/css" />
    <link href="css/SMCss/Customjquery.datetimepicker.css" rel="stylesheet" type="text/css" />
    <link href="css/SMCss/SMDatePickerControl.css" rel="stylesheet" type="text/css" />
    <link href="css/thirdparty/font-awesome.css" rel="stylesheet" type="text/css" />
    <link href="css/SRMMailTags.css" rel="stylesheet" />
    <div>
        <!-- Beginning of Top Panel-->
        <asp:Panel ID="panelTop" runat="server">
            <div id="PolarisDownstreamTopPanelContainerParent">
                <div id="PolarisDownstreamTopContainer1" class="TopPanelContainers">
                    <div id="PolarisDownstreamProductsContainer" data-bind="foreach: SystemChainListing">
                        <button class="SystemChain ProductTabs" data-bind="text: systemName, attr: { title: systemName }, click: polarisdownstream.onClickSystemChain, css: { ActiveSetup: isSystemSelected() }" />
                    </div>
                    <div id="PolarisDownstreamNewSystemIconContainer"><i class="AddBtn fa fa-plus-circle"></i><span>System</span></div>
                </div>
                <div id="PolarisDownstreamNewSystemPanelContainer">
                    <div id="PolarisDownstreamNewSystemPanel" class="">
                        <div id="" style="text-align: center;">
                            <span class="PolarisDownstreamNewSystemNameLabel">System Name : </span>
                            <span id="PolarisDownstreamNewSystemNameInputValidation" class="redColor" style="display: none">*</span>
                            <input class="PolarisDownstreamNewSystemNameInput InputBox" style="width: 15%;" />

                            <span class="PolarisDownstreamNewSystemNameLabel">Connection Name : </span>
                            <span id="PolarisDownstreamConnectionInputValidation" class="redColor" style="display: none">*</span>
                            <div id="PolarisDownstreamSelectedConnectionInputDiv" style="display: inline-block; width: 14%;"></div>

                            <span>Calendar : </span>
                            <span id="PolarisDownstreamNewSystemCalendarInputValidation" class="redColor" style="display: none">*</span>
                            <div id="newSystemCalendarName" style="text-align: center; display: inline-block; width: 150px;">
                            </div>

                            <span>Effective Date : </span>
                            <span id="PolarisDownstreamNewSystemEffectiveDateInputValidation" class="redColor" style="display: none">*</span>
                            <input id="newSystemEffectiveDate" class="normalInputBox DateInput" autocomplete="off" />

                            <div id="PolarisDownstreamNewSystemDetailsContainer" style="text-align: center; display: inline-block">
                                <button id="PolarisDownstremAddNewSystemBtn" class="CommonGreenBtnStyle">Add</button>
                            </div>
                        </div>
                    </div>
                </div>
                <iframe id="srmdwh-iframe" src="" style="display:none;"></iframe>
                <div id="PolarisDownstreamTopContainer2" class="TopPanelContainers">
                    <div id="PolarisDownstreamDetailsContainer" data-bind="using: SystemChainListing">
                        <span class="DetailsTabs">Server Name :
                                <input class="SystemDetailsInput" readonly data-bind="value: selectedServerName(), attr: { title: selectedServerName() }" /></span>
                        <span class="DetailsTabs">Connection Name :
                                <input class="SystemDetailsInput" readonly data-bind="value: selectedDbName(), attr: { title: selectedRealDbName() }" /></span>
                        <%-- <span class="DetailsTabs">User Name :
                                <input class="SystemDetailsInput" data-bind="value: selectedUserName(), attr: { title: selectedUserName() }" /></span>
                        <span class="DetailsTabs">Password :
                                <input type="password" class="SystemDetailsInput" data-bind="value: selectedPassword()" /></span>--%>
                        <div class="DetailsTabs">
                            <span>Calendar</span>
                            <span class="redColor" data-bind='visible: selectedCalendarName.hasError, text: selectedCalendarName.validationMessage'></span>
                            <div id="SelectedSetupCalender" style="width: 150px; display: inline-block;">
                            </div>
                        </div>
                        <div class="DetailsTabs">
                            <span>Effective Date</span>
                            <span class="redColor" data-bind='visible: selectedEffectiveDate.hasError, text: selectedEffectiveDate.validationMessage'></span>

                            <input id="SelectedEffectiveDate" class="normalInputBox DateInput" data-bind="textInput: selectedEffectiveDate, value: selectedEffectiveDate, text: selectedEffectiveDate" />

                        </div>
                    </div>


                    <div id="PolarisDownstreamButtonsContainerContainer">
                        <i id="PolarisDownstreamViewZipDownloadIcon" class="fa fa-download" aria-hidden="true" style="    
                            color: #43D9C6;
                            font-size: 14px;
                            position: relative;
                            top: 3px;
                            right: 2px;
                            cursor: pointer;">
                        </i>
                        <button id="PolarisDownstreamSchedulerBtn" style="font-size: 14px" data-bind="click: polarisdownstream.onClickSchedulerButton"><i class="fa fa-clock-o"></i></button>
                        <button id="PolarisDownstreamCloneBtn" style="font-size: 14px" data-bind="click: polarisdownstream.onClickCloneBtn"><i class="fa fa-clone"></i></button>
                        <input type="button" id="PolarisDownstreamTriggerButton" class="CommonGreenBtnStyle" value="Trigger" />
                        <input type="button" id="PolarisDownstreamSaveButton" class="CommonGreenBtnStyle" value="Save" />
                        <input type="button" id="PolarisDownstreamDeleteButton" class="CommonRedBtnStyle" value="Delete" style="display: none;" />
                    </div>
                    <div id="notificationsContainerDiv" class="notificationsContainerDiv" style="display: none; background: #f1f6f9; padding: 17px;">
                        <div id="notificationsContainer" runat="server">
                            <span class="PolarisDownstreamNewSystemNameLabel">System Name : </span>
                            <span id="PolarisDownstreamClonedSystemNameInputValidation" class="redColor" style="display: none">*</span>
                            <input id="PolarisDownstreamClonedSystemNameInput" class="PolarisDownstreamNewSystemNameInput InputBox" />
                            <span class="PolarisDownstreamNewSystemNameLabel">Connection Name : </span>
                            <span id="PolarisDownstreamCloneConnectionInputValidation" class="redColor" style="display: none">*</span>
                            <div id="PolarisDownstreamSelectedCloneConnectionInputDiv" style="display: inline-block; width: 200px;"></div>
                            <input type="button" id="PolarisDownstreamSaveCloneButton" class="CommonGreenBtnStyle" value="Clone" style="display: none;" />
                        </div>
                    </div>
                </div>
                <div id="PolarisDownstreamTopContainer3" class="TopPanelContainers">
                    <div id="PolarisDownstreamTimeseriesContainer" data-bind="foreach: reportTypes">
                        <span class="TimeSeriesTab" data-bind="click: polarisdownstream.onClickReportTypeChain, attr: { id: Type }, css: { inactiveTimeSeriesTab: $parent.IsActive }"><i class="fa fa-check notConfigured" data-bind="    css: { completeTimeSeriesTab: IsBlockTypeConfigured }"></i>
                            <button class="ProductTabs blueFont " data-bind="text: Type, attr: { title: Type }" />
                        </span>
                    </div>
                </div>
                </div>
            <div id="loadingImg" class="loadingMsg" style="display: none; z-index: 110000;">
            <img id="ctl00_Image1" src="images/ajax-working.gif" style="border-width:0px;" />           
            </div>

        </asp:Panel>

        <!-- Beginning of Middle Panel -->
        <asp:Panel ID="panelMiddle" runat="server">
            <div class="PolarisDownstreamMain">

                <!-- Beginning of Left Panel -->
                <div id="PolarisDownstreamMainLeftColumn" class="PolarisDownstreamMainColumns ">
                    <div id="PolarisDownstreamChainHeaderDiv">
                        <div>
                            <div class="PolarisDownstreamChainHeaderColumn1 ChainHeaderText">REPORTS</div>
                            <div class="PolarisDownstreamChainHeaderColumn2 ChainHeaderText"><i id="PolarisDownstreamAddNewReportBtn" class="AddBtn fa fa-plus-circle"></i></div>
                        </div>
                        <div id="PolarisDownstreamFilteringErrorDiv">
                        </div>
                    </div>
                    <div id="PolarisDownstreamChainSuperParentDiv" class="scroll">
                        <div id="PolarisDownstreamChainDropDownParentDiv" class=""></div>
                        <div class="PolarisDownstreamChainParentDiv" data-bind="foreach: ReportChainListing">
                            <div class="PolarisDownstreamChain" data-bind="click: polarisdownstream.onClickReportChain">
                                <div class="PolarisDownstreamChainInfo" data-bind="style: { background: chainBackgroundColor() }">
                                    <div class="PolarisDownstreamChainSubInfo1">
                                        <div class="PolarisDownstreamChainColumn1 PolarisDownstreamEllipses" data-bind="text: chainName, attr: { title: chainName }"></div>
                                        <span class="PolarisDownstreamChainColumn3 chainRightArrow" data-bind="visible: isSelected()">
                                            <i class="fa fa-caret-right"></i>
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <!-- Beginning of Right Panel -->
                <div id="PolarisDownstreamMainRightColumn" class="PolarisDownstreamMainColumns scroll">
                    <div id="PolarisDownstreamSelectedReportDetailsMainContainer">
                        <!-- Beginning of Right Panel Header -->
                        <div class="PolarisDownstreamSelectedReportDetailsHeaderContainer">
                            <div id="PolarisDownstreamSelectedReportDetailsHeaderContainer1Tab1" data-bind="text: ComputedReportName(), attr: { title: ComputedReportName() }"></div>
                        </div>
                        <!-- Beginning of Right Panel Container1 -->
                        <div id="PolarisDownstreamSelectedReportDetailsContainer1Parent">
                            <div id="PolarisDownstreamSelectedReportDetailsContainer1">
                                <div id="PolarisDownstreamSelectedReportTableNameContainer" class="PolarisDownstreamSelectedReportDetailsContainer1Tab">
                                    <span id="PolarisDownstreamSelectedReportTableNameLabel">Table Name
                                    </span>
                                    <input id="PolarisDownstreamSelectedReportTableNameInput" class="InputBox" data-bind="value: SelectedTableName, text: SelectedTableName, valueUpdate: 'afterkeydown', attr: { title: SelectedTableName }" disabled="disabled" />
                                    <span class="redColor" data-bind='visible: SelectedTableName.hasError, text: SelectedTableName.validationMessage'></span>
                                    <span class="redColor fontSize12" data-bind='visible: IsTableNameInvalid() && !SelectedTableName.hasError, text: IsTableNameInvalid() ? "Invalid Table Name" : ""'></span>
                                </div>
                                <div id="PolarisDownstreamSelectedReportBatchSizeContainer" class="PolarisDownstreamSelectedReportDetailsContainer1Tab">
                                    <span id="PolarisDownstreamSelectedReportBatchSizeLabel">Batch Size</span>
                                    <input id="PolarisDownstreamSelectedReportBatchSizeInput" class="InputBox" data-bind="value: SelectedBatchSize, text: SelectedBatchSize || 1000, valueUpdate: 'afterkeydown'" onkeypress="javascript:return polarisdownstream.isNumber(event)" />
                                    <span class="redColor" data-bind='visible: SelectedBatchSize.hasError, text: SelectedBatchSize.validationMessage'></span>
                                </div>
                                <div id="PolarisDownstreamSelectedReportRequireDeletedRowContainer" class="PolarisDownstreamSelectedReportDetailsContainer1Tab">
                                    <span id="PolarisDownstreamSelectedReportRequireDeletedRow" data-bind=" text: SelectedModule() == 'Ref' ? 'Require Deleted Entities' : 'Require Deleted Securities'"></span>
                                    <div style="width: 72px">
                                        <input id="PolarisDownstreamSelectedReportRequireDeletedRowChk" type="checkbox" data-bind="checked: SelectedRequireDeletedAssetTypes" />
                                    </div>
                                </div>
                                <div id="PolarisDownstreamSelectedReportRequireKnowledgeDateReportingContainer" class="PolarisDownstreamSelectedReportDetailsContainer1Tab">
                                    <span id="PolarisDownstreamSelectedReportRequireKnowledgeDateReporting">Require Knowledge Date Reporting</span>
                                    <div style="width: 76px">
                                        <input id="PolarisDownstreamSelectedRequireKnowledgeDateReportingChk" type="checkbox" data-bind="checked: SelectedRequireKnowledgeDateReporting" />
                                    </div>
                                </div>
                                <!-- ko if: IsTS() -->
                                <div id="PolarisDownstreamSelectedReportRemoveTimePartParent" class="PolarisDownstreamSelectedReportDetailsContainer1Tab">
                                    <span id="PolarisDownstreamSelectedReportRemoveTimePart">Require Intraday Changes</span>
                                    <div style="width: 72px">
                                        <input id="PolarisDownstreamSelectedReportRemoveTimePartChk" type="checkbox" data-bind="checked: SelectedRequireTimeInTSReport" />
                                    </div>
                                </div>
                                <!-- /ko -->
                            </div>
                        </div>
                        <div id="PolarisDownstreamContainersParent">
                            <div class="grid2x2">
                                 <!-- ko if: IsTS() -->
                                <div class="box box1" data-bind="hidden: IsNTS">
                                    <div class="categoryLabel">Reference Attribute</div>
                                    <div class="categoryBody" >
                                        <div id="PolarisDownstreamSelectedReportRefKnowledgeDateChkConatiner">
                                            <span id="PolarisDownstreamSelectedReportRefKnowledgeDateChkConatinerLabel">Attribute values to be massaged</span>
                                            <input id="PolarisDownstreamSelectedReportRefKnowledgeDateChk" type="checkbox" data-bind="checked: SelectedRequireLookupMassagingCurrentKnowledgeDate" />
                                        </div>
                                        
                                        <div id="PolarisDownstreamSelectedReportEffectiveDateChkConatiner" data-bind="hidden: IsDaily">
                                            <span id="PolarisDownstreamSelectedReportEffectiveDateLabel">Attribute values to be massaged</span>
                                            <input id="PolarisDownstreamSelectedReportEffectiveDateChk" type="checkbox" data-bind="checked: SelectedRequireLookupMassagingStartDate" />
                                        </div>
                                       
                                    </div>
                                </div>
                                 <!-- /ko -->
                                <div class="box box2">
                                    <div class="categoryLabel">Modified within</div>
                                    <div id="PolarisDownstreamSelectedReportDateInputConatiner" class="categoryBody">
                                        <%--<div id="PolarisDownstreamSelectedCalendarIdInputContainer" style="display: inline-block; width: 97%">
                                            <span id="PolarisDownstreamSelectedCalendarIdInputLabel">Calendar</span>
                                            <span class="redColor" data-bind='visible: SelectedCalendarName.hasError, text: SelectedCalendarName.validationMessage'></span>
                                            <div id="PolarisDownstreamSelectedCalendarIdInputDiv" style="float: right; width: 34%;">
                                            </div>
                                        </div>--%>
                                        <div id="PolarisDownstreamSelectedReportStartDateInputConatiner" class="PolarisDownstreamSelectedReportDateInputConatiner">
                                            <span id="PolarisDownstreamSelectedReportStartDateLabel" class="PolarisDownstreamSelectedReportDateSpan">Start Date</span>
                                            <div id="PolarisDownstreamSelectedReportStartDateInputDiv" style="float: right; width: 34%;"></div>
                                            <span class="redColor" data-bind='visible: SelectedStartDateValue.hasError, text: SelectedStartDateValue.validationMessage'></span>
                                            <div id="panelCustomDateStart" style="display: none;">
                                                <span class="PolarisDownstreamSelectedReportDateSpan">Custom Date
                                                </span>
                                                <input id="InputCustomDateStart" oninput="customStartDateInput()" data-bind="textInput: SelectedCustomStartDate, value: SelectedCustomStartDate, text: SelectedCustomStartDate" class="InputBox" />
                                                <span class="fa fa-calendar calendarStart"></span>
                                            </div>
                                            <div id="panelTNDayStart" style="display: none;">
                                                <span class="PolarisDownstreamSelectedReportDateSpan">Business Days
                                                </span>
                                                <input id="InputBusinessDaysStart" class="InputBox" value="" onkeypress="javascript:return polarisdownstream.isNumber(event)" />
                                            </div>
                                        </div>
                                        <div id="PolarisDownstreamSelectedReportEndDateInputConatiner" class="PolarisDownstreamSelectedReportDateInputConatiner">
                                            <span id="PolarisDownstreamSelectedReportEndDateLabel" class="PolarisDownstreamSelectedReportDateSpan">End Date</span>
                                            <div id="PolarisDownstreamSelectedReportEndDateLabelInputDiv" style="float: right; width: 34%;"></div>
                                            <span class="redColor" data-bind='visible: SelectedEndDateValue.hasError, text: SelectedEndDateValue.validationMessage'></span>
                                            <div id="panelCustomDateEnd" style="display: none;">
                                                <span class="PolarisDownstreamSelectedReportDateSpan">Custom Date
                                                </span>
                                                <input id="InputCustomDateEnd" class="InputBox" data-bind="value: SelectedCustomEndDate" />
                                                <span class="fa fa-calendar calendarEnd"></span>
                                            </div>
                                            <div id="panelTNDayEnd" style="display: none;">
                                                <span class="PolarisDownstreamSelectedReportDateSpan">Business Days
                                                </span>
                                                <input id="InputBusinessDaysEnd" class="InputBox" onkeypress="javascript:return polarisdownstream.isNumber(event)" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="box box3">
                                    <div class="categoryLabel">Post Loading Custom Class</div>
                                    <div id="PolarisDownstreamSelectedReportPostLoadingCustomClassInputConatiner" class="categoryBody">
                                        <div id="PolarisDownstreamSelectedReportPostLoadingAssemblyPathContainer" style="display: inline-block; width: 100%;">
                                            <span id="PolarisDownstreamSelectedReportPostLoadingAssemblyPathLabel">Assembly Path</span>
                                            <input id="PolarisDownstreamSelectedReportPostLoadingAssemblyPathInput" class=" postLoadingInputBox InputBox enable? EnabledInput : DisabledInput " data-bind="value: SelectedCCAssemblyName" />
                                        </div>
                                        <div id="PolarisDownstreamSelectedReportPostLoadingClassNameContainer" style="display: inline-block; width: 100%;">
                                            <span id="PolarisDownstreamSelectedReportPostLoadingClassNameContainerLabel">Class Name</span>
                                            <input id="PolarisDownstreamSelectedReportPostLoadingClassNameContainerInput" class="postLoadingInputBox InputBox" data-bind="value: SelectedCCClassName" />
                                        </div>
                                        <div id="PolarisDownstreamSelectedReportPostLoadingPathNameContainer" style="display: inline-block; width: 100%;">
                                            <span id="PolarisDownstreamSelectedReportPostLoadingPathNameContainerLabel">Method Name</span>
                                            <input id="PolarisDownstreamSelectedReportPostLoadingPathNameContainerInput" class=" postLoadingInputBox InputBox" data-bind="value: SelectedCCMethodName" />
                                        </div>
                                    </div>
                                </div>
                                <div class="box box4">
                                    <div class="categoryLabel">Notifications</div>
                                    <div class="categoryBody">
                                        <div>
                                            <span id="PolarisDownstreamSelectedReportFailureEmailLabel">Failure email notification to</span>
                                            <%-- <input id="PolarisDownstreamSelectedReportFailureEmailInput" class="InputBox" data-bind="value: SelectedFailureEmail" />--%>
                                            <input id="PolarisDownstreamSelectedReportFailureEmailInput" class="InputBox" style ="display:none;"/>
                                            <span id="dialog" style ="float: right;border-bottom: 2px solid rgb(156, 150, 150);width: 34%;display: inline;height: 33px;overflow-y:auto;">
                                                <span id='dialogMsg' style='color: rgb(66, 66, 66);'>
                                                </span>

                                            </span>
                                            <span class="redColor fontSize12" data-bind='visible: IsEmailIdInvalid(), text: IsEmailIdInvalid() ? "Invalid Email Id" : ""'></span>
                                        </div>
                                        <div id="PolarisDownstreamSelectedReportQueueNotificationInputConatiner">
                                            <span id="PolarisDownstreamSelectedReportQueueNotificationInputLabel">Status notifications on</span>
                                            <div id="PolarisDownstreamSelectedReportQueueNotificationInputDiv" style="float: right; width: 34%;"></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="scheduler">
                <div id="schedulingInfo" style="overflow: hidden;">
                    <div class="leftColumn" style="width: 34em; padding-right: 8%; padding-left: 25%;">
                        <table cellspacing="15" style="border-collapse: collapse; border: 0px">
                            <tr>
                                <td class="DownstreamSetup_shadedLabelSpan">Recurrence Type
                                </td>
                                <td>
                                    <input id="recurringRadio" type="radio" name="recurrenceTypeRadioGroup" value="Recurring" />Recurring
                                <input id="nonRecurringRadio" type="radio" name="recurrenceTypeRadioGroup" value="Non-Recurring" />Non-Recurring
                                </td>
                            </tr>
                            <tr>
                                <td class="DownstreamSetup_tdStyle">
                                    <label class="DownstreamSetup_shadedLabelSpan">Start Date</label>
                                </td>
                                <td class="DownstreamSetup_tdStyle">
                                    <input id="startDateTxt" type="text" class="SMTaskNameSetup_DateAttributeText DownstreamSetup_tdInputStyle" style="width: 100%;" />
                                    <span class="errorMsg"></span>
                                </td>
                            </tr>
                            <tr>
                                <td class="DownstreamSetup_tdStyle">
                                    <label class="DownstreamSetup_shadedLabelSpan">Recurrence Pattern</label>
                                </td>
                                <td class="DownstreamSetup_tdStyle">
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
                                    <input type="checkbox" name="dayOfWeekCheckboxGroup" value="Sunday" />Sunday<br />
                                    <input type="checkbox" name="dayOfWeekCheckboxGroup" value="Monday" />Monday<br />
                                    <input type="checkbox" name="dayOfWeekCheckboxGroup" value="Tuesday" />Tuesday<br />
                                    <input type="checkbox" name="dayOfWeekCheckboxGroup" value="Wednesday" />Wednesday<br />
                                    <input type="checkbox" name="dayOfWeekCheckboxGroup" value="Thursday" />Thursday<br />
                                    <input type="checkbox" name="dayOfWeekCheckboxGroup" value="Friday" />Friday<br />
                                    <input type="checkbox" name="dayOfWeekCheckboxGroup" value="Saturday" />Saturday<br />
                                    <span class="errorMsg"></span>
                                </td>
                            </tr>
                            <tr>
                                <td id="intervalLbl" class="DownstreamSetup_tdStyle">
                                    <label class="DownstreamSetup_shadedLabelSpan">Interval</label>
                                </td>
                                <td class="DownstreamSetup_tdStyle">
                                    <input id="intervalTxt" class="DownstreamSetup_tdInputStyle" data-required="true" style="width: 100%;" />
                                    <span class="errorMsg"></span>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="rightColumn" style="width: 28%;">
                        <table border="0" cellspacing="15" style="border-collapse: collapse;">
                            <tr>
                                <td style="width: 30%;">
                                    <label class="DownstreamSetup_shadedLabelSpan">End date</label>
                                </td>
                                <td>
                                    <input id="endDateTxt" type="text" class="SMTaskNameSetup_DateAttributeText DownstreamSetup_tdInputStyle" style="width: 100%;" />
                                    <span class="errorMsg"></span>
                                </td>
                            </tr>
                            <tr>
                                <td class="DownstreamSetup_tdStyle">
                                    <label class="DownstreamSetup_shadedLabelSpan">Number of Recurrences</label>
                                </td>
                                <td class="DownstreamSetup_tdStyle">
                                    <input class="DownstreamSetup_tdInputStyle" id="numberOfRecurrenceTxt" data-required="true" style="width: 100%;" />
                                    <span class="errorMsg"></span>
                                </td>
                            </tr>
                            <tr>
                                <td class="DownstreamSetup_tdStyle">
                                    <label class="DownstreamSetup_shadedLabelSpan">Start Time (24 hr format)</label>
                                </td>
                                <td class="DownstreamSetup_tdStyle">
                                    <input class="maskedTime DownstreamSetup_tdInputStyle" placeholder="hh:mm:ss" id="startTimeTxt" data-required="true" style="width: 100%;" />
                                    <span class="errorMsg"></span>
                                </td>
                            </tr>
                            <tr>
                                <td class="DownstreamSetup_tdStyle">
                                    <label class="DownstreamSetup_shadedLabelSpan">Time Interval of Recurrence(in mins)</label>
                                </td>
                                <td class="DownstreamSetup_tdStyle">
                                    <input id="intervalRecurrenceTxt" class="DownstreamSetup_tdInputStyle" data-required="true" style="width: 100%;" />
                                    <span class="errorMsg"></span>
                                </td>
                            </tr>
                            <tr>
                                <td class="DownstreamSetup_tdStyle">
                                    <input type="checkbox" name="neverEndJobCheckbox" id="neverEndJobCheckbox" onclick='javascript: if (this.checked) {
    $("#endDateTxt").attr("disabled", "disabled");
    $("#endDateTxt").datepicker("disable");
}
else {
    $("#endDateTxt").removeAttr("disabled");
    $("#endDateTxt").datepicker("enable");
}' /><span style="bottom: 2px; position: relative;">Never End Job</span>
                                </td>
                                <td class="DownstreamSetup_tdStyle">
                                    <div class="removeSchedulingButton" id="removeSchedulingButton">
                                        Remove Scheduling
                                    </div>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
            <div id="downstreamSyncErrorDiv_messagePopUp" style="display: none;">
                <div id="downstreamSyncDisableDiv" class="disableDiv" style="display: none; z-index: 99998;" align="center">
                </div>
                <div class="downstreamSyncError_popupContainer">
                    <div class="downstreamSyncError_popupCloseBtnContainer">
                        <div class="fa fa-times" id="downstreamSyncError_popupCloseBtn" onclick="javascript:$('#downstreamSyncErrorDiv_messagePopUp').hide(); $('#workflowSetup_disableDiv').hide();"></div>
                    </div>
                    <div class="downstreamSyncError_popupHeader">
                        <div class="downstreamSyncError_imageContainer">
                            <img id="downstreamSyncError_ImageURL" />
                        </div>
                        <div class="downstreamSyncError_messageContainer">
                            <div class="downstreamSyncError_popupTitle">
                            </div>
                        </div>
                    </div>
                    <div class="downstreamSyncError_popupMessageContainer">
                        <div class="downstreamSyncError_popupMessage">
                        </div>
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
    <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
    <script src="js/thirdparty/jquery-ui.min.js" type="text/javascript"></script>
    <script src="js/thirdparty/knockout-3.4.0.js" type="text/javascript"></script>
    <script src="js/thirdparty/socket.io.js" type="text/javascript"></script>
    <script src="includes/jquery.slimscroll1.js" type="text/javascript"></script>
    <script src="includes/jquery.slimscrollHorizontal.js" type="text/javascript"></script>
    <script src="includes/SMSlimscroll.js" type="text/javascript"></script>
    <script src="includes/MicrosoftAjax.js" type="text/javascript"></script>
    <script src="includes/SMSelect.js" type="text/javascript"></script>
    <script src="includes/RADCommonScripts.js" type="text/javascript"></script>
    <script src="includes/SecMasterScripts.js" type="text/javascript"></script>
    <script src="includes/RScriptUtils.debug.js" type="text/javascript"></script>
    <script src="includes/script.js" type="text/javascript"></script>
    <script src="includes/jquery.datetimepicker.js" type="text/javascript"></script>
    <script src="includes/SMDatePickerControl.js" type="text/javascript"></script>
    <script src="includes/mapping.js"></script>
    <script src="includes/SRMPolarisDownstreamConfig.js"></script>
    <script src="includes/fileUploadScript.js" type="text/javascript"></script>
</asp:Content>
