var srmDashboard = (function () {
    var srmDashboard;

    function SRMDashboardClass() {
    }
    srmDashboard = new SRMDashboardClass();

    var dashboardExtraInfo = null;
    var dateFilterValues = {};
    var srmDatePickerId = null;
    var dateFilterInitialized = false;

    var selectedTab = null;
    var defaultDateInFilter = {};

    SRMDashboardClass.prototype.DashboardDateFilterRefreshed = function DashboardDateFilterRefreshed(objDate, isSave) {
        if (isSave || !dateFilterInitialized) {
            for (var tabId in saveDateFilter) {
                if (saveDateFilter.hasOwnProperty(tabId)) {
                    saveDateFilter[tabId] = true;
                }
            }
            saveDateFilter[selectedTab] = false;
            defaultDateInFilter = objDate;
        }
        ApplyDateFilter(objDate, false);
    }

    SRMDashboardClass.prototype.Init = function Init(objInfo) {
        dashboardExtraInfo = eval("(" + objInfo + ")");

        $(".DashboardFilterSection").hide();
        $(".DashboardDivContainer").hide();

        InitializeTabs();

        CreateHandlers();
    }

    function InitializeTabs() {
        var moduleId = -1;
        var demoModules = [];
        for (var tabName in TopBarTabsMapping) {
            if (TopBarTabsMapping.hasOwnProperty(tabName)) {
                if (dashboardExtraInfo.IsDemoBuild && DemoDashboardMapping[TopBarTabsMapping[tabName]]) {
                    demoModules.push({ displayName: tabName, moduleId: moduleId-- });
                }
            }
        }
        if (demoModules.length > 0) {
            CommonModuleTabs.setCustomInit(demoModules);
        }
        for (var tabName in TopBarTabsMapping) {
            if (TopBarTabsMapping.hasOwnProperty(tabName)) {
                SRMProductTabs.setCallback({
                    key: tabName.toLowerCase(), value: (function () {
                        var tab = tabName;
                        return function () { TabSwitchHandler(TopBarTabsMapping[tab]); };
                    })()
                });
            }
        }
    }

    function CreateHandlers() {
        $(document).unbind('click').click(function () {
            window.parent.$(parent.document).trigger('click');
        });

        $(window).unbind('resize').resize(function () {
            for (var tabId in DashboardTabs) {
                if (DashboardTabs.hasOwnProperty(tabId) && DashboardInitialized[tabId]) {
                    SRMIframeResize(tabId);
                }
            }
        });

        $(window).unbind('message').bind('message', function (e) {
            if (e.originalEvent.data == 'resize')
                SRMIframeResize(selectedTab);
        });
    }

    function SRMIframeResize(tabId) {
        var iframeHeight, width;
        iframeHeight = $(window).height() - iframeSubtractHeight[tabId];
        width = $(window).width();

        var iFrame = $("#" + iframeDashboard[tabId]);
        iFrame.height(iframeHeight).outerWidth(width);

        iFrame[0].contentWindow.postMessage("resize", "*");
    }

    function OpenSRMIframe() {
        var tabId = selectedTab;
        var iframe = $("#" + iframeDashboard[tabId]);

        var newPath = path;
        newPath = newPath.substring(0, newPath.length - 9);
        var url = newPath + '/' + DashboardAspxName[tabId] + '?CommonIntegrated=1' + (DashboardPageIdentifier[tabId] != null ? ('&identifier=' + DashboardPageIdentifier[tabId]) : '') + (TabModuleMapping[tabId] != null ? "&moduleID=" + TabModuleMapping[tabId] : "") + (DashboardSaveButtonId[tabId] != null ? ('&SaveId=' + DashboardSaveButtonId[tabId]) : '') + (HasDateFilter[tabId] ? ('&marked=' + dateFilterValues.marked + '&checked=' + dateFilterValues.checked + '&startDate=' + dateFilterValues.startDate + '&endDate=' + dateFilterValues.endDate + '&serverStartDate=' + dateFilterValues.serverStartDate + '&serverEndDate=' + dateFilterValues.serverEndDate + '&dateFilterInitialized=' + dateFilterInitialized.toString()) : '');

        iframe.attr("src", url);
        SRMIframeResize(tabId);

        if (DashboardSaveButtonId[tabId] != null || DashboardRefreshButtonId[tabId] != null || DashboardRightFilterId[tabId] != null || HasDateFilter[tabId]) {
            iframe.off('load').on('load', function () {
                if (HasDateFilter[tabId]) {
                    if (dateFilterInitialized) {
                        if (saveDateFilter[tabId])
                            iframe[0].contentWindow.SaveFilterChanges(defaultDateInFilter);
                    }
                    else
                        InitializeDatePicker();

                    saveDateFilter[tabId] = false;
                    dateFilterInitialized = true;
                }

                var controlsIds = null;
                if (DashboardSaveButtonId[tabId] != null || DashboardRefreshButtonId[tabId] != null || DashboardRightFilterId[tabId] != null)
                    controlsIds = iframe[0].contentWindow.GetControlIds();

                if (DashboardRefreshButtonId[tabId] != null) {
                    $("#" + DashboardRefreshButtonId[tabId]).unbind("click").click(function () {
                        iframe.contents().find('#' + controlsIds.RefreshId).trigger("click");
                    });
                }

                if (DashboardSaveButtonId[tabId] != null) {
                    $("#" + DashboardSaveButtonId[tabId]).unbind("click").click(function () {
                        iframe.contents().find('#' + controlsIds.SaveId).trigger("click");
                    });
                }

                if (DashboardRightFilterId[tabId] != null) {
                    $("#" + DashboardRightFilterId[tabId]).unbind("click").click(function () {
                        iframe.contents().find('#' + controlsIds.FilterId).trigger("click");
                    });
                }
            });
        }
    }

    function TabSwitchHandler(tabId) {
        selectedTab = tabId;

        $(".DashboardFilterSection").hide();
        $(".DashboardDivContainer").hide();

        $("#" + DashboardFiltersSectionMapping[selectedTab]).css('display', 'inline-block');
        $("#" + DashboardDivContainersMapping[selectedTab]).show();

        for (var tabId in DashboardRightFilterId) {
            if (DashboardRightFilterId.hasOwnProperty(tabId) && DashboardRightFilterId[tabId] != null && DashboardInitialized[tabId]) {
                $("#" + iframeDashboard[tabId])[0].contentWindow.SwitchTabs();
            }
        }

        if (!DashboardInitialized[selectedTab]) {
            OpenSRMIframe();

            if (HasDateFilter[selectedTab])
                UpdateDateFilter(false);
            DashboardInitialized[selectedTab] = true;
        }
        else {
            SRMIframeResize(selectedTab);
            if (HasDateFilter[selectedTab] && saveDateFilter[selectedTab]) {
                $('#' + iframeDashboard[selectedTab])[0].contentWindow.SaveFilterChanges(defaultDateInFilter);
                saveDateFilter[selectedTab] = false;
            }
        }
        if (HasDateFilter[selectedTab] && DateFilterNeedsRefreshing[selectedTab])
            UpdateDateFilter(true);
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

            ApplyDateFilter(dateSettings, true);
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

    function UpdateDateFilter(refreshData) {
        var objDate = dateFilterValues;
        DateFilterNeedsRefreshing[selectedTab] = false;

        var dateFilterData = GetDateFilterValuesForUpdate(objDate);

        $('#' + DateFilterId[selectedTab]).find(".dateFilterText").html(dateFilterData.text);
        if (refreshData)
            document.getElementById(iframeDashboard[selectedTab]).contentWindow.UpdateDateChanges(objDate);
    }

    function ApplyDateFilter(objDate, refreshData) {
        //if (refreshData) {
        for (var tabId in DateFilterNeedsRefreshing) {
            if (DateFilterNeedsRefreshing.hasOwnProperty(tabId)) {
                DateFilterNeedsRefreshing[tabId] = true;
            }
        }
        //}

        dateFilterValues = objDate;
        UpdateDateFilter(refreshData);
    }

    return srmDashboard;
})();

function DashboardDateFilterRefreshed(objDate, isSave) {
    srmDashboard.DashboardDateFilterRefreshed(objDate, isSave);
}
