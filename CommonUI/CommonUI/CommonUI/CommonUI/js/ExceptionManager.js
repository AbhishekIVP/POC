var exceptionManager = {}
exceptionManager._rowWidth = 0;
exceptionManager._count = 0;
exceptionManager._path = '';
exceptionManager._tabSelected = 'All SYSTEMS';
exceptionManager._tagSelected = 'ALL';
exceptionManager._tagNameForService = '-1';
exceptionManager._exceptionStateForService = null;
exceptionManager._listSecurityTypeForService = null;
exceptionManager._listAttributeForService = null;
exceptionManager._listAttributeForServiceRef = null;
exceptionManager._listExceptionTypeForService = null;
exceptionManager._listSectypeTableIdForService = null;
exceptionManager._usernameForService = "";
exceptionManager._moduleIDForService = 0;                                               //Added ModuleID
exceptionManager._startDateForService = "";
exceptionManager._endDateForService = "";
exceptionManager._listSecurityIdForService = "";
exceptionManager._resolvedForService = 0;
exceptionManager._suppressedForService = 0;
exceptionManager._listEntityTypeForService = null;
exceptionManager._listLegsForService = null;
exceptionManager._listEntityCodeForService = null;
exceptionManager._info = {};
exceptionManager._serviceCount = 0;
exceptionManager._prodName = '';
exceptionManager._setValuesForException = null;
exceptionManager._exceptionStateList = [{ value: "1", text: "Open", isSelected: "true" }, { value: "2", text: "Resolved", isSelected: "false" }, { value: "3", text: "UnResolved", isSelected: "false" }, { value: "4", text: "Suppressed", isSelected: "false" }, { value: "5", text: "UnSuppressed", isSelected: "false"}];
exceptionManager._exceptionRefStateList = [{ value: "5", text: "Open", isSelected: "true" }, { value: "8", text: "Resolved", isSelected: "false" }, { value: "4", text: "UnResolved", isSelected: "false" }, { value: "2", text: "Suppressed", isSelected: "false" }, { value: "1", text: "UnSuppressed", isSelected: "false" }]
exceptionManager._windowWidth = "";
exceptionManager._windowHeight = "";
exceptionManager._tagArrowLeft = 90;
exceptionManager._moduleID_moduleName_map = [];
exceptionManager._secModuleId = 0;
exceptionManager._refModuleId = 0;
exceptionManager._fundModuleId = 0;
exceptionManager._partyModuleId = 0;
exceptionManager._serverShortDateFormat = "";
exceptionManager.preInit = function (info) {
    exceptionManager._info = JSON.parse(info);
    //if (exceptionManager._info.ProductName.toLowerCase() == "refmaster")
    //    exceptionManager._tabSelected = exceptionManager._info.ProductName;
    //Initialize Values
    exceptionManager._usernameForService = exceptionManager._info.UserName;
    exceptionManager._serverShortDateFormat = exceptionManager._info.CultureShortDateFormat;
    exceptionManager._windowWidth = $(window).width();
    exceptionManager._windowHeight = $(window).height();
    exceptionManager.setPath();
    exceptionManager.initDateControl();

    //var listofTabsToBindFunctionsWith = CommonModuleTabs.ModuleList();
    var listofTabsToBindFunctionsWith = {};
    $.each(CommonModuleTabs.data, function (k, v) {
        listofTabsToBindFunctionsWith[v.name.toString().toLowerCase()] = { 'moduleId': v.moduleId, 'displayName': v.name }
    });

    var tempListOfTabs = {};
    if (Object.keys(listofTabsToBindFunctionsWith).length > 2)
    {        
        var arrTemp = [],tempTabData = [];
        $.each(Object.keys(listofTabsToBindFunctionsWith), function (k, i) {
            if (listofTabsToBindFunctionsWith[i].moduleId != -1)
                arrTemp.push(listofTabsToBindFunctionsWith[i].moduleId);
        });        
        listofTabsToBindFunctionsWith["allsystems"].moduleId = arrTemp.toString();        
    }
    else
        delete listofTabsToBindFunctionsWith["allsystems"];

    for (i in listofTabsToBindFunctionsWith) {
        item = listofTabsToBindFunctionsWith[i];

        exceptionManager._moduleID_moduleName_map[item.displayName.toLowerCase()] = item.moduleId;
        exceptionManager._secModuleId = exceptionManager._moduleID_moduleName_map["securities"];
        exceptionManager._refModuleId = exceptionManager._moduleID_moduleName_map["refdata"];
        exceptionManager._fundModuleId = exceptionManager._moduleID_moduleName_map["funds"];
        exceptionManager._partyModuleId = exceptionManager._moduleID_moduleName_map["parties"];

        switch (item.displayName.toLowerCase().trim()) {
            case "allsystems":
                SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: exceptionManager.initAllSystem });
                break;
            case "securities":
                SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: exceptionManager.initSecMaster });
                break;
            case "refdata":
                SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: exceptionManager.initRefMaster });
                break;
            case "funds":
                SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: exceptionManager.initFundMaster });
                break;
            case "parties":
                SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: exceptionManager.initPartyMaster });
                break;
        }
    }
    //exceptionManager._moduleIDForService = exceptionManager._info.ModuleID;             //Added ModuleID
    //$("#em_pageContainer").height(exceptionManager._windowHeight);
}

exceptionManager.initSecMaster = function () {
    exceptionManager._moduleIDForService = exceptionManager._moduleID_moduleName_map["securities"];
    exceptionManager._tabSelected = 'Secmaster';
    exceptionManager.init();
    if ($('#detailGrid').length > 0)
        $('#closeDetailsGrid').click();
	$('#em_pageContainer').css('overflow-y','hidden');
}

exceptionManager.initRefMaster = function () {
    exceptionManager._tabSelected = 'Refmaster';
    exceptionManager._moduleIDForService = exceptionManager._moduleID_moduleName_map["refdata"];
    exceptionManager.init();
    if ($('#detailGrid').length > 0)
        $('#closeDetailsGrid').click();
	$('#em_pageContainer').css('overflow-y','hidden');
}

exceptionManager.initPartyMaster = function () {
    exceptionManager._tabSelected = 'Partymaster';
    exceptionManager._moduleIDForService = exceptionManager._moduleID_moduleName_map["parties"];
    exceptionManager.init();
    if ($('#detailGrid').length > 0)
        $('#closeDetailsGrid').click();
	$('#em_pageContainer').css('overflow-y','hidden');
}

exceptionManager.initFundMaster = function () {
    exceptionManager._tabSelected = 'Fundmaster';
    exceptionManager._moduleIDForService = exceptionManager._moduleID_moduleName_map["funds"];	
    exceptionManager.init();
    if ($('#detailGrid').length > 0)
        $('#closeDetailsGrid').click();
	$('#em_pageContainer').css('overflow-y','hidden');
}

exceptionManager.initAllSystem = function () {
    exceptionManager._tabSelected = 'All Systems';
    exceptionManager._moduleIDForService = exceptionManager._moduleID_moduleName_map["allsystems"];	
    exceptionManager.init();
    if ($('#detailGrid').length > 0)
        $('#closeDetailsGrid').click();
	$('#em_pageContainer').css('overflow-y','scroll');
}

exceptionManager.attachTagsSlideClickHandler = function () {
    var tagsContainer = document.getElementsByClassName("DataExceptionsTagsScrollContainer")[0];

    $("#em_tagsLeftClick").unbind("click").bind("click", function () {
        tagsContainer.scrollLeft -= 30;
    });

    $("#em_tagsRightClick").unbind("click").bind("click", function () {
        tagsContainer.scrollLeft += 30;
    });
};
exceptionManager.detailsGridSuccessCallback = function (sessionIdentifierKey) {
    onServiceUpdating();

    //Adjust Datefilter and sidefilter icons
    $('#rightMenuDiv').css('display', 'none')
    $("#dateFilter").css("right", "5px");

    var url = "";
    if (exceptionManager._tabSelected.toLowerCase().trim() == 'secmaster' || exceptionManager._prodName.toLowerCase().trim() == 'secmaster')
        url = "../SMExceptionManager.aspx?identifier=ExceptionManager&mode=nw&SessionIdentifier=" + sessionIdentifierKey;
    else if (exceptionManager._tabSelected.toLowerCase().trim() == 'refmaster' || exceptionManager._prodName.toLowerCase().trim() == 'refmaster'
             || exceptionManager._tabSelected.toLowerCase().trim() == 'partymaster' || exceptionManager._prodName.toLowerCase().trim() == 'partymaster'
             || exceptionManager._tabSelected.toLowerCase().trim() == 'fundmaster' || exceptionManager._prodName.toLowerCase().trim() == 'fundmaster')
        url = '../RMHomeInternal.aspx?identifier=RefM_ExceptionManager&SessionIdentifier=' + sessionIdentifierKey;

    var windowHeight = $(window).height();
    var iframeHeight = windowHeight - 50;
    var topValue = '';

    //if (exceptionManager._info.ProductName.toLowerCase().trim() == 'refmaster') {
    //    topValue = 36;
    //    $('.DataExceptionsTagsContainer').css('margin-top', '40px');
    //}
    //else
    topValue = 37;

    $('.exceptions-details-div').animate({ top: topValue + "px", height: iframeHeight + "px" }, 800, function () {
        //$('.exceptions-details-div').append('<div id="closeDetailsGrid" style="cursor:pointer;position:absolute;right:0px;font-size:22px;padding-right: 10px;">x</div><div style="height:100%;width:100%;"><iframe id = "detailGrid" style="border: none;" src = "' + url + '" width="100%" height = "100%"></iframe></div>');
        $('.exceptions-details-div').append('<div id="closeDetailsGrid" class="EM_backBtnText">Back</div><div style="height:100%;width:100%;"><iframe id = "detailGrid" style="border: none;" src = "' + url + '" width="100%" height = "100%"></iframe></div>');

        $('#closeDetailsGrid').unbind('click').click(function (e) {
            $('.exceptions-details-div').html('');
            $('.exceptions-details-div').animate({ top: "200px", height: "0px" }, 800, function () {
            });

            //Adjust Datefilter and sidefilter icons
            $('#rightMenuDiv').css('display', '')
            $("#dateFilter").css("right", "40px");

            exceptionManager._count = 0;
            exceptionManager.init();
            // $('.exceptions-details-div').slideUp(function () {
            // $('.exceptions-details-div').html('');
            // });
        });

        $("#detailGrid").on("load", function (e) {
            onServiceUpdated();
        });
        // $('.exceptions-details-div').slideDown();
        // $('.exceptions-details-div').append('<div id="closeDetailsGrid" class="EM_backBtnText">Back</div><div style="height:100%;width:100%;"><iframe id = "detailGrid" style="border: none;" src = "' + url + '" width="100%" height = "100%"></iframe></div>');

        // $('#closeDetailsGrid').unbind('click').click(function (e) {
        // $('.exceptions-details-div').slideUp(function () {
        // $('.exceptions-details-div').html('');
        // });

        // });

    });

    //exceptionManager._serviceCount = exceptionManager._serviceCount - 1;
    //if (exceptionManager._serviceCount === 0) {
    //onServiceUpdated();
    //}
};
exceptionManager.setTagsContainerWidth = function (tags) {
    var tLength = tags.length;
    var tagsWidth = 0;
    $.each(tags, function (index, ele) {
        tagsWidth += ($(ele).width() + 15);
    });
    tagsWidth += 5; //For extra buffer

    if (tLength === 1) {
        tagsWidth = 250;
    }

    if (tagsWidth > exceptionManager._windowWidth) {
        $("#em_tagsLeftClick")[0].style.display = "";
        $("#em_tagsRightClick")[0].style.display = "";

        $(".DataExceptionsTagsContainer").css("width", exceptionManager._windowWidth);
        $(".DataExceptionsTagsScrollContainer").css("width", exceptionManager._windowWidth - 70);
        $('.DataExceptionsTagsDiv').css('width', tagsWidth);

        exceptionManager.attachTagsSlideClickHandler();
    }
    else {
        $(".DataExceptionsTagsContainer").css("width", tagsWidth);
        $(".DataExceptionsTagsScrollContainer").css("width", tagsWidth);
        $('.DataExceptionsTagsDiv').css('width', tagsWidth);

        $("#em_tagsLeftClick")[0].style.display = "none";
        $("#em_tagsRightClick")[0].style.display = "none";
    }
};
exceptionManager.setContainersHeight = function () {
    var tabToggleDiv = $("#tabToggleDiv");
    var tabToggleDivHeight = $("#tabToggleDiv").height();

    var DataExceptionsTagsContainer = $("#DataExceptionsTagsContainer");
    var DataExceptionsTagsContainerHeight = DataExceptionsTagsContainer.height();

    var allsystemsDiv = $('#allsystemsDiv');
    allsystemsDiv.height(exceptionManager._windowHeight - (tabToggleDivHeight + DataExceptionsTagsContainerHeight + 100 + $('.srm_panelTopSections').outerHeight()));

    var outerDiv = $("#outer-div");
    outerDiv.height(exceptionManager._windowHeight - (tabToggleDivHeight + DataExceptionsTagsContainerHeight + 110 + $('.srm_panelTopSections').outerHeight()));
};
exceptionManager.FailureCallback = function (err) {    
    console.log(err);
}
exceptionManager.initSlideMenu = function (data, callback) {
    var obj = {};
    obj.pivotElementId = "rightMenuDiv";
    obj.data = data;

    smslidemenu.destroy("smslidemenu_0");
    //$('.smslidemenu_filterDiv').remove();
    smslidemenu.init(obj, callback);
}
exceptionManager.initDateControl = function () {
    var tempDateObj = {
        0: "Today",
        1: "Since Yesterday",
        2: "This Week",
        3: "Anytime",
        4: "Custom Date"
    }

    var obj = {};

    obj.dateOptions = [0, 1, 2, 3, 4];
    obj.dateFormat = exceptionManager._info.ShortDateFormat;
    obj.timePicker = false;
    obj.lefttimePicker = false;
    obj.righttimePicker = false;
    obj.selectedTopOption = 0;
    $("#dateFilterText").val(tempDateObj[2]);

    if (obj.selectedTopOption == 0) {
        exceptionManager._startDateForService = new Date().format(exceptionManager._serverShortDateFormat);
            //com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate)).split(' ')[0];
        exceptionManager._endDateForService = new Date().format(exceptionManager._serverShortDateFormat);
            //com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate)).split(' ')[0];
    }

    if (obj.selectedTopOption == 2) {
        var currDate = new Date();
        var currStartDate = new Date(currDate.getTime()), currEndDate = new Date(currDate.getTime());
        var currDay = currDate.getDay();
        var startDate, endDate;
        if (currDay > 1) {
            currStartDate.setDate(currDate.getDate() - (currDay - 1));
            currStartDate.setHours(0, 0, 0, 0);
            startDate = currStartDate;
            endDate = currEndDate;
        }
        else {
            currStartDate.setHours(0, 0, 0, 0);
            startDate = currStartDate;
            endDate = currEndDate;
        }

        exceptionManager._startDateForService = startDate.format(exceptionManager._serverShortDateFormat);
            //com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(startDate, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate)).split(' ')[0];
        exceptionManager._endDateForService = endDate.format(exceptionManager._serverShortDateFormat);
        //com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(endDate, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate)).split(' ')[0];
    }

    obj.calenderType = 0;
    obj.pivotElement = $('#dateFilter'); //Calender will be displayed when clicked on this div

    obj.StartDateCalender = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
    obj.EndDateCalender = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));

    obj.selectedCustomRadioOption = 0;
    obj.applyCallback = function () {
        var selectedDate = smdatepickercontrol.getResponse($("#smdd_0"));

        var startDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(selectedDate.selectedStartDate), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
        var endDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(selectedDate.selectedEndDate), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));

        if (selectedDate.selectedText.toLocaleLowerCase() === "anytime") {
            exceptionManager._endDateForService = null;
        }
        else if (selectedDate.selectedText.toLocaleLowerCase() === "custom date" && selectedDate.selectedEndDate === null) {
            exceptionManager._endDateForService = null;
        }
        else {
            //exceptionManager._endDateForService = endDate.split(' ')[0];
            exceptionManager._endDateForService = selectedDate.serverEndDateOriginal;
        }

        if (selectedDate.selectedText.toLocaleLowerCase() === "anytime") {
            exceptionManager._startDateForService = null;
        }
        else if (selectedDate.selectedText.toLocaleLowerCase() === "custom date" && selectedDate.selectedStartDate === null) {
            exceptionManager._startDateForService = null;
        }
        else {
            //exceptionManager._startDateForService = startDate.split(' ')[0];
            exceptionManager._startDateForService = selectedDate.serverStartDateOriginal;
        }


        var dateSettings = {};
        dateSettings.startDate = startDate;
        dateSettings.endDate = endDate;
        dateSettings.serverStartDate = selectedDate.serverStartDateOriginal;
        dateSettings.serverEndDate = selectedDate.serverEndDateOriginal;
        dateSettings.marked = selectedDate.selDateOption;
        dateSettings.checked = selectedDate.selRadioOption;

        var errorMsg = exceptionManager.validateDates(dateSettings);
        if (errorMsg.length > 0)
        {
            return errorMsg;
        }
            

        if (selectedDate.selectedText.toLowerCase() === "custom date") {
            var displayText = "";
            switch (selectedDate.selRadioOption) {
                case 0:
                    displayText = "From " + startDate.split(' ')[0];
                    break;
                case 1:
                    displayText = "Between " + startDate.split(' ')[0] + " and " + endDate.split(' ')[0];
                    break;
                case 2:
                    displayText = "Prior To " + endDate.split(' ')[0];
                    break;
            }
            $("#dateFilterText").text(displayText);
        }
        else {
            $("#dateFilterText").text(selectedDate.selectedText);
        }
        exceptionManager._count = 0;
        exceptionManager.init();
        if ($('#detailGrid').length > 0) {
            $('#closeDetailsGrid').click()
            exceptionManager._setValuesForException.objFilterExceptionInfo.StartDate = exceptionManager._startDateForService;
            exceptionManager._setValuesForException.objFilterExceptionInfo.EndDate = exceptionManager._endDateForService;
            exceptionManager._setValuesForException.objFilterExceptionInfo.CustomDateSelected = selectedDate.selRadioOption;
            exceptionManager._setValuesForException.objFilterExceptionInfo.ExceptionDate = selectedDate.selDateOption;
            exceptionManager._setValuesForException.objFilterExceptionInfo.ExceptionState = exceptionManager._exceptionStateForService;
            exceptionManager.callService('POST', exceptionManager._path + '/CommonService.asmx', 'SetValuesForException', exceptionManager._setValuesForException, function(){ exceptionManager.detailsGridSuccessCallback(exceptionManager._setValuesForException.sessionIdentifierKey)}, exceptionManager.FailureCallback);
        }
    }
    smdatepickercontrol.init(obj);
}

exceptionManager.validateDates = function(dateSettings) {
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

exceptionManager.bindRightMenu = function () {
    //Binding Right Side Menu For All Systems
    if (exceptionManager._tabSelected.toLowerCase().trim() == 'all systems') {
        var entityTypes = '';
        var selEntitytypes = [];
        var entitySelectAll = true;
        if (exceptionManager._listEntityTypeForService !== null) {
            selEntitytypes = exceptionManager._listEntityTypeForService;
            entitySelectAll = false;
        }
        exceptionManager.callService('POST', exceptionManager._path + '/ExceptionManagerService.svc', 'GetAllEntityTypes', { selectedEntityTypes: selEntitytypes, userName: exceptionManager._usernameForService, ModuleID: exceptionManager._moduleIDForService }, getEntityTypesSuccessCallback, exceptionManager.FailureCallback, entityTypes)

        function getEntityTypesSuccessCallback(entityTypes) {

            if (exceptionManager._moduleIDForService.toString().indexOf(exceptionManager._secModuleId) > -1)
            {
                var sectypes = '';
                var selSectypes = [];
                var secSelectAll = true;
                if (exceptionManager._listSecurityTypeForService !== null) {
                    selSectypes = exceptionManager._listSecurityTypeForService;
                    secSelectAll = false;
                }
                exceptionManager.callService('POST', exceptionManager._path + '/ExceptionManagerService.svc', 'GetAllSectypes', { selectedSectypes: selSectypes, username: exceptionManager._usernameForService }, getSecTypesSuccessCallback, exceptionManager.FailureCallback, sectypes)

                function getSecTypesSuccessCallback(sectypes) {
                    var selSectype = 0;
                    for (var sec in sectypes.d) {
                        if (sectypes.d[sec].isSelected === "true")
                        { selSectype++; }
                    }
                    var selEntitytype = 0;
                    for (var ref in entityTypes.d) {
                        if (entityTypes.d[ref].isSelected === "true")
                        { selEntitytype++; }
                    }

                    if (selSectype === (sectypes.d.length)) {
                        secSelectAll = true;
                    }
                    if (selEntitytype === (entityTypes.d.length)) {
                        entitySelectAll = true;
                    }

                    var arrSectionAll = [{ "sectionHeader": "Exception State", listItems: exceptionManager._exceptionStateList, "state": "up", "sectionType": "radio", "sectionName": "Exception State" }, { "sectionHeader": "Security Types", listItems: sectypes.d, "selectAllText": "All Security Types", "state": "up", "sectionType": "checkbox", selectAllItems: secSelectAll, isSearchable: true, "sectionName": "Security Type" }, { "sectionHeader": "Entity Types", listItems: entityTypes.d, "selectAllText": "All Entity Types", "state": "up", "sectionType": "checkbox", selectAllItems: entitySelectAll, isSearchable: true, "sectionName": "Entity Type" }];
                    exceptionManager.initSlideMenu(arrSectionAll, exceptionManager.allSectionsSideMenuApplyCallback);
                }
            }
            else
            {
                var selEntitytype = 0;
                for (var ref in entityTypes.d) {
                    if (entityTypes.d[ref].isSelected === "true")
                    { selEntitytype++; }
                }
                if (selEntitytype === (entityTypes.d.length)) {
                    entitySelectAll = true;
                }

                var arrSectionAll = [{ "sectionHeader": "Exception State", listItems: exceptionManager._exceptionStateList, "state": "up", "sectionType": "radio", "sectionName": "Exception State" }, { "sectionHeader": "Entity Types", listItems: entityTypes.d, "selectAllText": "All Entity Types", "state": "up", "sectionType": "checkbox", selectAllItems: entitySelectAll, isSearchable: true, "sectionName": "Entity Type" }];
                exceptionManager.initSlideMenu(arrSectionAll, exceptionManager.allSectionsSideMenuApplyCallback);
            }
        }
    }
    //Binding Right Side Menu For SecMaster
    else if (exceptionManager._tabSelected.toLowerCase().trim() == 'secmaster') {
        var sectypes = '';
        var selSectypes = [];
        var secSelectAll = true;
        if (exceptionManager._listSecurityTypeForService !== null) {
            selSectypes = exceptionManager._listSecurityTypeForService;
            secSelectAll = false;
        }
        exceptionManager.callService('POST', exceptionManager._path + '/ExceptionManagerService.svc', 'GetAllSectypes', { selectedSectypes: selSectypes, username: exceptionManager._usernameForService }, getSecTypesSuccessCallback, exceptionManager.FailureCallback, sectypes, null, null, false)

        function getSecTypesSuccessCallback(sectypes) {
            var secAttr = '', secAttrAll = true;
			if(exceptionManager._listAttributeForService !== null && exceptionManager._listAttributeForService.find(x=> x== '-1') != undefined)
				exceptionManager._listAttributeForService = null;
            if (exceptionManager._listAttributeForService !== null) {
                secAttr = exceptionManager._listAttributeForService.join();
                secAttrAll = false;
            }

            if (exceptionManager._listSecurityTypeForService === null || exceptionManager._listSecurityTypeForService === "-1") {
                selSectypes = sectypes.d.map(function (item) { return item.value; });
            }

            var methodInputParam = {
                secTypeIds: selSectypes.join(","),
                userName: exceptionManager._usernameForService,
                selectedAttributes: exceptionManager._listAttributeForService
            }
            exceptionManager.callService('POST', exceptionManager._path + '/ExceptionManagerService.svc', 'GetAllAttributesBasedOnSectypeSelection', methodInputParam, getAttributesSuccessCallback, exceptionManager.FailureCallback, secAttr, null, null, false)

            function getAttributesSuccessCallback(attributes) {
                //To Select all Sectypes and Attributes if all options come as selected from backend
                var selSectype = 0;
                for (var sec in sectypes.d) {
                    if (sectypes.d[sec].isSelected === "true")
                    { selSectype++; }
                }
                var selAttr = 0;
                for (var attr in attributes.d) {
                    if (attributes.d[attr].isSelected === "true")
                    { selAttr++; }
                }

                if (selSectype === (sectypes.d.length)) {
                    secSelectAll = true;
                }
                if (selAttr === (attributes.d.length)) {
                    secAttrAll = true;
                }

                var arrSectionSec = [{ "sectionHeader": "Exception State", listItems: exceptionManager._exceptionStateList, "state": "up", "sectionType": "radio" }, { "sectionHeader": "Security Types", listItems: sectypes.d, "selectAllText": "All Security Types", "state": "up", "sectionType": "checkbox", selectAllItems: secSelectAll, isSearchable: true, sectionCloseCallback: exceptionManager.sectypeSlideMenuClose }, { "sectionHeader": "Attributes", listItems: attributes.d, "selectAllText": "All Attributes", "state": "up", "sectionType": "checkbox", selectAllItems: secAttrAll, isSearchable: true, sectionOpenCallback: exceptionManager.getAttributesBasedOnSectype}];
                exceptionManager.initSlideMenu(arrSectionSec, exceptionManager.secmSideMenuApplyCallback);
                
                var selectedItems = exceptionManager.getSideMenuItemsSecM();
                exceptionManager._listSecurityTypeForService = selectedItems.sectypeID;
                exceptionManager._listAttributeForService = selectedItems.attrID;
                exceptionManager._listSecMAttributeForService = selectedItems.attrID;
                exceptionManager._exceptionStateForService = selectedItems.exceptionState;
                exceptionManager.computeExceptionState();
            }
        }
    }
    //Binding Right Side Menu For RefMaster/Party/Fund
    else {
        var entityTypes = '';
        var selEntitytypes = [];
        var entitySelectAll = true;
        if (exceptionManager._listEntityTypeForService !== null && typeof (exceptionManager._listEntityTypeForService) !== "undefined") {
            selEntitytypes = exceptionManager._listEntityTypeForService;
            entitySelectAll = false;
        }
        exceptionManager.callService('POST', exceptionManager._path + '/ExceptionManagerService.svc', 'GetAllEntityTypes', { selectedEntityTypes: selEntitytypes, userName: exceptionManager._usernameForService, ModuleID: exceptionManager._moduleIDForService }, getEntityTypeSuccessCallback, exceptionManager.FailureCallback, entityTypes, null, null, false)

        function getEntityTypeSuccessCallback(entityTypes) {
            var entityAttr = '', eTypes = '-1', refAttrAll = true;
			if(exceptionManager._listAttributeForServiceRef !== null && exceptionManager._listAttributeForServiceRef.find(x=> x== '-1') != undefined)
				exceptionManager._listAttributeForServiceRef = null;
            if (exceptionManager._listAttributeForServiceRef !== null) {
                entityAttr = exceptionManager._listAttributeForServiceRef.join();
                refAttrAll = false;
            }

            if (exceptionManager._listEntityTypeForService === null || eTypes === "-1") {
                eTypes = entityTypes.d.map(function (item) { return item.value; }).join();
            }

            var methodInputParam = {
                entityTypeIds: eTypes,
                selectedAttributes: exceptionManager._listAttributeForServiceRef === null ? [] : exceptionManager._listAttributeForServiceRef
            }

            exceptionManager.callService('POST', exceptionManager._path + '/ExceptionManagerService.svc', 'GetAllAttributesBasedOnEntityTypeSelection', methodInputParam, getEntityAttrSuccessCallback, exceptionManager.FailureCallback, entityAttr, null, null, false)

            function getEntityAttrSuccessCallback(entityAttr) {
                //To Select all Entity Types and Attributes if all options come as selected from backend
                var selEntityTypes = 0;
                for (var ref in entityTypes.d) {
                    if (entityTypes.d[ref].isSelected === "true")
                    { selEntityTypes++; }
                }
                var selAttr = 0;
                for (var attr in entityAttr.d) {
                    if (entityAttr.d[attr].isSelected === "true")
                    { selAttr++; }
                }

                if (selEntityTypes === (entityTypes.d.length)) {
                    entitySelectAll = true;
                }
                if (selAttr === (entityAttr.d.length)) {
                    refAttrAll = true;
                }

                var arrSectionRM = [{ "sectionHeader": "Exception State", listItems: exceptionManager._exceptionRefStateList, "state": "up", "sectionType": "radio" }, { "sectionHeader": "Entity Types", listItems: entityTypes.d, "selectAllText": "All Entity Types", "state": "up", "sectionType": "checkbox", selectAllItems: entitySelectAll, isSearchable: true, sectionCloseCallback: exceptionManager.entityTypeSlideMenuClose }, { "sectionHeader": "Attributes", listItems: entityAttr.d, "selectAllText": "All Attributes", "state": "up", "sectionType": "checkbox", selectAllItems: refAttrAll, isSearchable: true, sectionOpenCallback: exceptionManager.getAttributesBasedOnEntityType }];
                exceptionManager.initSlideMenu(arrSectionRM, exceptionManager.refmSideMenuApplyCallback);

                var selectedItems = exceptionManager.getSideMenuItemsRefM();
                exceptionManager._listEntityTypeForService = selectedItems.entityTypeID;
                exceptionManager._listAttributeForServiceRef = selectedItems.attrID;
                exceptionManager._listRefMAttributeForService = selectedItems.attrID;
                exceptionManager._exceptionStateForService = selectedItems.exceptionState;
                //exceptionManager.computeExceptionState();
                exceptionManager.RefcomputeExceptionState();
            }
        }
    }
}
exceptionManager.init = function () {
    onServiceUpdating();

    exceptionManager.bindRightMenu();

    exceptionManager.setContainersHeight();

    var exManagerTypeDiv = $('#exManagerTypeDiv');
    var exceptionTypeDiv = $('#exceptionTypeDiv');
    var allsystemsDiv = $('#allsystemsDiv');
    var frozenHeadDiv = $('#right-section-inner-head');
    
    var sessionIdentifierKey = SecMasterJSCommon.SMSCommons.getGUID();

    //if (exceptionManager._count == 0) {
        //exceptionManager._count = exceptionManager._count + 1;
        //Binding for Tags in SecMaster
        if (exceptionManager._tabSelected.toLowerCase().trim() == 'secmaster') {
            var serviceObj = {
                "objSMExceptionFilterInfo": {
                    "ListSecurityType": exceptionManager._listSecurityTypeForService,
                    "ListAttribute": exceptionManager._listSecMAttributeForService,
                    "ListExceptionType": null,
                    "ListSectypeTableId": null,
                    "UserName": exceptionManager._usernameForService,
                    "StartDate": exceptionManager._startDateForService,
                    "EndDate": exceptionManager._endDateForService,
                    "ListSecurityId": null,
                    "Resolved": exceptionManager._resolvedForService,
                    "Suppressed": exceptionManager._suppressedForService,
                    "TagName": "-1"//exceptionManager._tagNameForService
                }
            }
            var tagsJson = '';
            exceptionManager._serviceCount = exceptionManager._serviceCount + 1;
            exceptionManager.callService('POST', exceptionManager._path + '/ExceptionManagerService.svc', 'SMGetExceptionTagsCountData', serviceObj, tagsSuccessCallback, exceptionManager.FailureCallback, tagsJson);
        }
        //Binding for Tags in RefMaster
        else if (exceptionManager._tabSelected.toLowerCase().trim() == 'refmaster' || exceptionManager._tabSelected.toLowerCase().trim() == 'partymaster'
                    || exceptionManager._tabSelected.toLowerCase().trim() == 'fundmaster') {
            var serviceObj = {
                "objRMExceptionFilterInfo": {
                    "ListEntityType": exceptionManager._listEntityTypeForService,
                    "ListAttribute": exceptionManager._listRefMAttributeForService,
                    "ListExceptionType": null,
                    "ListLegs": null,
                    "UserName": exceptionManager._usernameForService,
                    "StartDate": exceptionManager._startDateForService,
                    "EndDate": exceptionManager._endDateForService,
                    "ListEntityCode": null,
                    "Resolved": exceptionManager._resolvedForService,
                    "Suppressed": exceptionManager._suppressedForService,
                    "TagName": "-1"//exceptionManager._tagNameForService
                },
                "ModuleID": exceptionManager._moduleIDForService
            }
            var tagsJson = '';
            exceptionManager._serviceCount = exceptionManager._serviceCount + 1;
            exceptionManager.callService('POST', exceptionManager._path + '/ExceptionManagerService.svc', 'RMGetExceptionTagsCountData', serviceObj, tagsSuccessCallback, exceptionManager.FailureCallback, tagsJson);
        }
        //Binding for Tags in All Systems
        else {
            var smserviceObj = {
                "ListSecurityType": exceptionManager._listSecurityTypeForService,
                "ListAttribute": null,
                "ListExceptionType": null,
                "ListSectypeTableId": null,
                "UserName": exceptionManager._usernameForService,
                "StartDate": exceptionManager._startDateForService,
                "EndDate": exceptionManager._endDateForService,
                "ListSecurityId": null,
                "Resolved": exceptionManager._resolvedForService,
                "Suppressed": exceptionManager._suppressedForService,
                "TagName": "-1"//exceptionManager._tagNameForService
            }
            var rmserviceObj = {
                "ListEntityType": exceptionManager._listEntityTypeForService,
                "ListAttribute": null,
                "ListExceptionType": null,
                "ListLegs": null,
                "UserName": exceptionManager._usernameForService,
                "StartDate": exceptionManager._startDateForService,
                "EndDate": exceptionManager._endDateForService,
                "ListEntityCode": null,
                "Resolved": exceptionManager._resolvedForService,
                "Suppressed": exceptionManager._suppressedForService,
                "TagName": "-1"//exceptionManager._tagNameForService

            }
            var serviceObj = { objSMExceptionFilterInfo: smserviceObj, objRMExceptionFilterInfo: rmserviceObj, ModuleID: exceptionManager._moduleIDForService };
            var tagsJson = '';
            exceptionManager._serviceCount = exceptionManager._serviceCount + 1;
            exceptionManager.callService('POST', exceptionManager._path + '/ExceptionManagerService.svc', 'GetCommonEMTags', serviceObj, tagsSuccessCallback, exceptionManager.FailureCallback, tagsJson);
        }

        //Success Callback for Tags in all Tabs
        function tagsSuccessCallback(tagsJson) {
            //Adjust arrow to start position while tags are binded            

            if (tagsJson.d.length == 0) {
                $('.DataExceptionsTagsScrollContainer').children('.noexceptionsDiv').remove();
                $('.DataExceptionsTagsScrollContainer').append('<div class="noexceptionsDiv"></div>');
                $('.dashboardDownArrowDiv').css('display', 'none');
            }
            else {
                $('.DataExceptionsTagsScrollContainer').children('.noexceptionsDiv').remove();
                $('.dashboardDownArrowDiv').css('display', '');
            }

            var html = '';
            $.each(tagsJson.d, function (k, v) {
                if (exceptionManager._tagNameForService == "-1" && k == 0) {
                    html += '<div class= "DataExceptionsTag DataExceptionsTagSelected"><div class="DataExceptionsTagTitle" title="' + v.TagName + '">' + v.TagName + '</div><div class="DataExceptionsTagCount" title="' + v.TagCount + '"><span>' + v.TagCount + '</span></div></div>';
                }
                else if (v.TagName == exceptionManager._tagNameForService) {
                    html += '<div class= "DataExceptionsTag DataExceptionsTagSelected"><div class="DataExceptionsTagTitle" title="' + v.TagName + '">' + v.TagName + '</div><div class="DataExceptionsTagCount" title="' + v.TagCount + '"><span>' + v.TagCount + '</span></div></div>';
                }
                else
                    html += '<div class= "DataExceptionsTag"><div class="DataExceptionsTagTitle" title="' + v.TagName + '">' + v.TagName + '</div><div class="DataExceptionsTagCount" title="' + v.TagCount + '"><span>' + v.TagCount + '</span></div></div>';
            });

            //Perform adjustments once html is appended to page
            $('.DataExceptionsTagsDiv').html(html).promise().done(function () {
                exceptionManager.setTagsContainerWidth($('.DataExceptionsTagsDiv').children(".DataExceptionsTag"));
                exceptionManager.adjustArrowLeft();
            });

            $('.DataExceptionsTag').unbind('hover').hover(exceptionManager.tagHoverIn, exceptionManager.tagHoverOut);

            $('.DataExceptionsTag').unbind('click').click(function (e) {
                var self = $(this);

                $('.DataExceptionsTag').removeClass('DataExceptionsTagSelected');
                self.addClass('DataExceptionsTagSelected');
                exceptionManager._tagSelected = self.children('.DataExceptionsTagTitle').text().trim();
                if (exceptionManager._tagSelected.toLowerCase() == "all exceptions")
                    exceptionManager._tagNameForService = "-1";
                else
                    exceptionManager._tagNameForService = exceptionManager._tagSelected;
                exceptionManager.init();
                exceptionManager.adjustArrowLeft();
            });
            exceptionManager._serviceCount = exceptionManager._serviceCount - 1;
            if (exceptionManager._serviceCount === 0) {
                onServiceUpdated();
            }
        }
    //}

    //Bind Data for All Systems
    if (exceptionManager._tabSelected.toLowerCase().trim() == 'all systems') {
        $('#outer-div').css('display', 'none');
        exManagerTypeDiv.css('display', 'none');
        exceptionTypeDiv.css('display', 'none');
        frozenHeadDiv.css('display', 'none');
        allsystemsDiv.css('display', '');

        exManagerTypeDiv.html('');
        exceptionTypeDiv.html('');
        frozenHeadDiv.html('');


        html = '';
        var exceptionsJson = '';
        var smserviceObj = {
            "ListSecurityType": exceptionManager._listSecurityTypeForService,
            "ListAttribute": null,
            "ListExceptionType": null,
            "ListSectypeTableId": null,
            "UserName": exceptionManager._usernameForService,
            "StartDate": exceptionManager._startDateForService,
            "EndDate": exceptionManager._endDateForService,
            "ListSecurityId": null,
            "Resolved": exceptionManager._resolvedForService,
            "Suppressed": exceptionManager._suppressedForService,
            "TagName": exceptionManager._tagNameForService
        }
        var rmserviceObj = {
            "ListEntityType": exceptionManager._listEntityTypeForService,
            "ListAttribute": null,
            "ListExceptionType": null,
            "ListLegs": null,
            "UserName": exceptionManager._usernameForService,
            "StartDate": exceptionManager._startDateForService,
            "EndDate": exceptionManager._endDateForService,
            "ListEntityCode": null,
            "Resolved": exceptionManager._resolvedForService,
            "Suppressed": exceptionManager._suppressedForService,
            "TagName": exceptionManager._tagNameForService

        }
        var serviceObj = { objSMExceptionFilterInfo: smserviceObj, objRMExceptionFilterInfo: rmserviceObj, ModuleID: exceptionManager._moduleIDForService };
        exceptionManager._serviceCount = exceptionManager._serviceCount + 1;
        exceptionManager.callService('POST', exceptionManager._path + '/ExceptionManagerService.svc', 'GetCommonEMData', serviceObj, allDataSuccessCallback, exceptionManager.FailureCallback, exceptionsJson);

        function allDataSuccessCallback(exceptionsJson) {
            //window.parent.leftMenu.hideNoRecordsMsg();
			$("#em_errorMsgContainer").hide();
			html = '';
			$.each(exceptionsJson.d, function (k, v) {
			    //var color = true, textColour = '#498399';
				var textColour = '#498399';
                html += '<div class="outer-system-div"><div class="system-exception-type">' + v.ExceptionType + '</div><div class="system-exception-outer-div">';
                if (exceptionManager._moduleIDForService.toString().indexOf(exceptionManager._secModuleId) > -1)
                {           
					textColour = '#498399';
                    if (v.SecmasterCount == 0)
                        html += '<div class="system-exception-left"><div style="display:inline;vertical-align: sub;"><div class="system-sec-name" style="color:' + textColour + '">Securities</div></div><div class="system-sec-count-zero" extypeid="' + v.ExceptionTypeID + '"></div></div>';
                    else
                        html += '<div class="system-exception-left"><div style="display:inline;vertical-align: sub;"><div class="system-sec-name" style="color:' + textColour + '">Securities</div></div><div class="system-sec-count" style="color:' + textColour + '" extypeid="' + v.ExceptionTypeID + '">' + v.SecmasterCount + '</div></div>'

                    // if (color) {
                        // textColour = '#7d769b';
                        // color = false;
                    // }
                    // else
                    // {
                        // color = true;
                        // textColour = '#498399'
                    // }
                }
                if (exceptionManager._moduleIDForService.toString().indexOf(exceptionManager._refModuleId) > -1) {
                    
					textColour = '#7d769b';
                    if (v.RefmasterCount == 0)
                        html += '<div class="system-exception-right"><div style="display:inline;vertical-align: sub;"><div class="system-ref-name" style="color:' + textColour + '">RefData</div></div><div class="system-ref-count-zero" extypeid="' + v.ExceptionTypeID + '"></div></div>';
                    else
                        html += '<div class="system-exception-right"><div style="display:inline;vertical-align: sub;"><div class="system-ref-name" style="color:' + textColour + '">RefData</div></div><div class="system-ref-count" style="color:' + textColour + '" modId="' + exceptionManager._refModuleId + '" extypeid="' + v.ExceptionTypeID + '">' + v.RefmasterCount + '</div></div>';
                    
					// if (color) {
                        // textColour = '#7d769b';
                        // color = false;
                    // }
                    // else {
                        // color = true;
                        // textColour = '#498399'
                    // }
                }
                if (exceptionManager._moduleIDForService.toString().indexOf(exceptionManager._fundModuleId) > -1) {
				
					textColour = '#6f7bad';
                    if (v.FundCount == 0)
                        html += '<div class="system-exception-right"><div style="display:inline;vertical-align: sub;"><div class="system-ref-name" style="color:' + textColour + '">Funds</div></div><div class="system-ref-count-zero" extypeid="' + v.ExceptionTypeID + '"></div></div>';
                    else
                        html += '<div class="system-exception-right"><div style="display:inline;vertical-align: sub;"><div class="system-ref-name" style="color:' + textColour + '">Funds</div></div><div class="system-ref-count" style="color:' + textColour + '" modId="' + exceptionManager._fundModuleId + '" extypeid="' + v.ExceptionTypeID + '">' + v.FundCount + '</div></div>';
                    
					// if (color) {
                        // textColour = '#7d769b';
                        // color = false;
                    // }
                    // else {
                        // color = true;
                        // textColour = '#498399'
                    // }					
                }
                if (exceptionManager._moduleIDForService.toString().indexOf(exceptionManager._partyModuleId) > -1) {
				
					textColour = '#91837b';
                    if (v.PartyCount == 0)
                        html += '<div class="system-exception-right"><div style="display:inline;vertical-align: sub;"><div class="system-ref-name" style="color:' + textColour + '">Parties</div></div><div class="system-ref-count-zero" extypeid="' + v.ExceptionTypeID + '"></div></div>';
                    else
                        html += '<div class="system-exception-right"><div style="display:inline;vertical-align: sub;"><div class="system-ref-name" style="color:' + textColour + '">Parties</div></div><div class="system-ref-count" style="color:' + textColour + '" modId="' + exceptionManager._partyModuleId + '" extypeid="' + v.ExceptionTypeID + '">' + v.PartyCount + '</div></div>';
                    
					// if (color) {
                        // textColour = '#7d769b';
                        // color = false;
                    // }
                    // else {
                        // color = true;
                        // textColour = '#498399'
                    // }
                }
                html += '</div></div>';

                //if (v.SecmasterCount == 0 && v.RefmasterCount == 0) {
                //    //html += '<div class="system-exception"><div class = "system-sec-count-zero" style="margin-left: 28%;"></div><div class ="system-sec-count-zero-text">No Exceptions</div></div></div>';
                //    html += '<div class="system-exception"><div class="system-exception-left"><div style="width:45%;display:inline-block;"><div class="system-sec-name">SEC</div><div class="system-sec-sub-name">Master</div></div><div class="system-sec-count system-sec-count-zero" extypeid = "' + v.ExceptionTypeID + '"></div> </div><div class="system-exception-right"><div style="width:45%;display:inline-block;"><div class="system-ref-name">REF</div><div class="system-ref-sub-name">Master</div></div><div class="system-ref-count system-ref-count-zero" extypeid = "' + v.ExceptionTypeID + '"></div></div></div></div>';
                //}
                //else if (v.SecmasterCount == 0)
                //    html += '<div class="system-exception"><div class="system-exception-left"><div style="width:45%;display:inline-block;"><div class="system-sec-name">SEC</div><div class="system-sec-sub-name">Master</div></div><div class="system-sec-count-zero" extypeid = "' + v.ExceptionTypeID + '"></div> </div><div class="system-exception-right"><div style="width:45%;display:inline-block;"><div class="system-ref-name">REF</div><div class="system-ref-sub-name">Master</div></div><div class="system-ref-count" extypeid = "' + v.ExceptionTypeID + '">' + v.RefmasterCount + '</div></div></div></div>';
                //else if (v.RefmasterCount == 0)
                //    html += '<div class="system-exception"><div class="system-exception-left"><div style="width:45%;display:inline-block;"><div class="system-sec-name">SEC</div><div class="system-sec-sub-name">Master</div></div><div class="system-sec-count" extypeid = "' + v.ExceptionTypeID + '">' + v.SecmasterCount + '</div> </div><div class="system-exception-right"><div style="width:45%;display:inline-block;"><div class="system-ref-name">REF</div><div class="system-ref-sub-name">Master</div></div><div class="system-ref-count-zero" extypeid = "' + v.ExceptionTypeID + '"></div></div></div></div>';
                //else
                //    html += '<div class="system-exception"><div class="system-exception-left"><div style="width:45%;display:inline-block;"><div class="system-sec-name">SEC</div><div class="system-sec-sub-name">Master</div></div><div class="system-sec-count" extypeid = "' + v.ExceptionTypeID + '">' + v.SecmasterCount + '</div> </div><div class="system-exception-right"><div style="width:45%;display:inline-block;"><div class="system-ref-name">REF</div><div class="system-ref-sub-name">Master</div></div><div class="system-ref-count" extypeid = "' + v.ExceptionTypeID + '">' + v.RefmasterCount + '</div></div></div></div>';
            });

            allsystemsDiv.html('');
            allsystemsDiv.html(html);
            if (exceptionManager._moduleIDForService.split(',').length === 4)
            {
                $('.outer-system-div').css('width', '46%');
                $('.system-exception-right').css('width', '22%');
                $('.system-exception-left').css('width', '22%');                
            }
            else if (exceptionManager._moduleIDForService.split(',').length === 3) {
                $('#allsystemsDiv').css('width', '70%');
                $('.system-exception-right').css('width', '30%');
                $('.system-exception-left').css('width', '30%');
            }
            else if (exceptionManager._moduleIDForService.split(',').length === 2) {
                $('.outer-system-div').css('width', '28%');
                $('.system-exception-right').css('width', '44%');
                $('.system-exception-left').css('width', '44%');
            }

            $('.system-ref-count, .system-sec-count').unbind('click').click(function (e) {
                var Info = {};
                if ($(this).hasClass('system-sec-count')) {
                    exceptionManager._prodName = 'secmaster'
                    Info.ExceptionSectype = "-1";
                }
                else {
                    exceptionManager._prodName = 'refmaster'
                    Info.ExceptionEntityType = "-1";
                }
                Info.TagName = exceptionManager._tagNameForService;
                Info.ExceptionType = this.getAttribute('extypeid');
                Info.ExceptionDate = smdatepickercontrol.getResponse($("#smdd_0")).selDateOption;


                Info.ExceptionAttribute = '-1';
                Info.ShowFilter = 'true';
                Info.ParentPageIdentifier = 'Dashboard';
                Info.UserName = exceptionManager._usernameForService;
                Info.ModuleId = $(this).attr('modId');

                if (Info.ExceptionDate == 4) {
                    Info.CustomDateSelected = smdatepickercontrol.getResponse($("#smdd_0")).selRadioOption;


                    Info.StartDate = exceptionManager._startDateForService;
                    Info.EndDate = exceptionManager._endDateForService;
                    if (Info.CustomDateSelected == 0)
                        Info.EndDate = null;
                    else if (Info.CustomDateSelected == 2)
                        Info.StartDate = null;
                }

                Info.ExceptionFilter = '1';
                if (exceptionManager._resolvedForService == 1 && exceptionManager._suppressedForService == -1)
                    Info.ExceptionFilter = '2';
                if (exceptionManager._resolvedForService == 0 && exceptionManager._suppressedForService == -1)
                    Info.ExceptionFilter = '3';
                if (exceptionManager._resolvedForService == -1 && exceptionManager._suppressedForService == 1)
                    Info.ExceptionFilter = '4';
                if (exceptionManager._resolvedForService == -1 && exceptionManager._suppressedForService == 0)
                    Info.ExceptionFilter = '5';


                // if (datefiltervalues['datefilterdataexceptionstile'].marked === '4') {
                // info.customdateselected = datefiltervalues['datefilterdataexceptionstile'].checked;
                // info.startdate = datefiltervalues['datefilterdataexceptionstile'].startdate;
                // info.enddate = datefiltervalues['datefilterdataexceptionstile'].enddate;
                // }

                var serviceData = { secIds: "", sessionIdentifierKey: sessionIdentifierKey, objFilterExceptionInfo: Info }
                //exceptionManager._serviceCount = exceptionManager._serviceCount + 1;
                exceptionManager._setValuesForException = serviceData;
                exceptionManager.callService('POST', exceptionManager._path + '/CommonService.asmx', 'SetValuesForException', serviceData, function () { exceptionManager.detailsGridSuccessCallback(sessionIdentifierKey) }, exceptionManager.FailureCallback);
            });

            exceptionManager._serviceCount = exceptionManager._serviceCount - 1;
            if (exceptionManager._serviceCount === 0) {
                onServiceUpdated();
            }
        }
    }
    else {
        $('#outer-div').css('display', '');
        exManagerTypeDiv.css('display', '');
        exceptionTypeDiv.css('display', '');
        frozenHeadDiv.css('display', '');
        allsystemsDiv.css('display', 'none');

        var exceptionsJson = '';
        //Bind Data for SecMaster
        if (exceptionManager._tabSelected.toLowerCase().trim() == 'secmaster') {
            var serviceObj = {
                "objSMExceptionFilterInfo": {
                    "ListSecurityType": exceptionManager._listSecurityTypeForService,
                    "ListAttribute": exceptionManager._listSecMAttributeForService,
                    "ListExceptionType": null,
                    "ListSectypeTableId": null,
                    "UserName": exceptionManager._usernameForService,
                    "StartDate": exceptionManager._startDateForService,
                    "EndDate": exceptionManager._endDateForService,
                    "ListSecurityId": null,
                    "Resolved": exceptionManager._resolvedForService,
                    "Suppressed": exceptionManager._suppressedForService,
                    "TagName": exceptionManager._tagNameForService
                }
            }
            exceptionManager._serviceCount = exceptionManager._serviceCount + 1;
            exceptionManager.callService('POST', exceptionManager._path + '/ExceptionManagerService.svc', 'SMGetExceptionCountData', serviceObj, exCountSuccessCallback, exceptionManager.FailureCallback, exceptionsJson);
        }
        //Bind Data for RefMaster
        else {
            var serviceObj = {
                "objRMExceptionFilterInfo": {
                    "ListEntityType": exceptionManager._listEntityTypeForService,
                    "ListAttribute": exceptionManager._listRefMAttributeForService,
                    "ListExceptionType": null,
                    "ListLegs": null,
                    "UserName": exceptionManager._usernameForService,
                    "StartDate": exceptionManager._startDateForService,
                    "EndDate": exceptionManager._endDateForService,
                    "ListEntityCode": null,
                    "Resolved": exceptionManager._resolvedForService,
                    "Suppressed": exceptionManager._suppressedForService,
                    "TagName": exceptionManager._tagNameForService
                },
                "ModuleID": exceptionManager._moduleIDForService
            }
            exceptionManager._serviceCount = exceptionManager._serviceCount + 1;
            exceptionManager.callService('POST', exceptionManager._path + '/ExceptionManagerService.svc', 'RMGetExceptionCountData', serviceObj, exCountSuccessCallback, exceptionManager.FailureCallback, exceptionsJson);
        }

        //Success Callback - Data Grid for SecMaster / RefMaster
        function exCountSuccessCallback(exceptionsJson) {
            var frozenhtml = '';
            html = '';
            var frozenHead = '';

            //var exceptionsLongTextArray = ["First Vendor Value Missing", "Invalid Data Exception", "No Vendor Value Found", "Reference Data Missing", "Underlying Security Missing"]
            $.each(exceptionsJson.d, function (k, v) {
                frozenhtml += '<div class="row-left"><div class="typeName" title="' + v.TypeName + '" >' + v.TypeName.toUpperCase() + '</div><div class="typeExceptionCount" typeid = "' + v.TypeId + '">' + v.ExceptionsCount + '</div></div>';
                exceptionManager._rowWidth = v.Exceptions.length * 120;
                html += '<div class="row-right" style="width:' + exceptionManager._rowWidth + 'px;">';
                $.each(v.Exceptions, function (kk, vv) {
                    if (k == 0)
                        frozenHead += '<div class="exceptionsType" extypeid = "' + vv.ExceptionTypeID + '">' + vv.ExceptionType + '</div>';
                    if (vv.ExceptionCount == 00)
                        html += '<div class="exceptionsTypeDiv"><div class="exceptionsCountZero" style = "color:#a4a4a4" extypeid = "' + vv.ExceptionTypeID + '">' + vv.ExceptionCount + '</div></div>';
                    else
                        html += '<div class="exceptionsTypeDiv"><div class="exceptionsCount" extypeid = "' + vv.ExceptionTypeID + '">' + vv.ExceptionCount + '</div></div>';
                });
                html += '</div>';
            });
            //html += '<div class="exceptions-details-div"></div>';

            exManagerTypeDiv.html('');
            exceptionTypeDiv.html('');
            exManagerTypeDiv.append(frozenhtml);
            exceptionTypeDiv.append(html);
            frozenHeadDiv.html('');
            frozenHeadDiv.append(frozenHead);
            frozenHeadDiv.css('width', exceptionManager._rowWidth)
            $('.right-section').css('width', exceptionManager._rowWidth)
            $('#outer-div').css('width', exceptionManager._rowWidth + $('.left-section').width() + 15);

            if (typeof (window.parent.leftMenu) != "undefined") {
                if (exceptionsJson.d.length === 0) {
                    $("#outer-div").hide();
                    window.parent.leftMenu.showNoRecordsMsg("No exceptions found matching your search criteria.", $("#em_errorMsgContainer"));
                }
                else {
                    $("#outer-div").show();
                    $("#em_errorMsgContainer").hide();
                    //window.parent.leftMenu.hideNoRecordsMsg();
                }
            }

            var exAttr = "-1";

            var listOfAttributes = null;
            if (exceptionManager._moduleIDForService == 3 || exceptionManager._moduleIDForService == "3") {
                listOfAttributes = exceptionManager._listAttributeForService;
            }
            else {
                listOfAttributes = exceptionManager._listAttributeForServiceRef;
            }

            if (listOfAttributes != null && listOfAttributes.length > 0) {
                exAttr = listOfAttributes.join();
            }

            $('.exceptionsCount').unbind('click').click(function (e) {

                var Info = {};
                Info.TagName = exceptionManager._tagNameForService;
                Info.ExceptionType = this.getAttribute('extypeid');
                Info.ExceptionDate = smdatepickercontrol.getResponse($("#smdd_0")).selDateOption;
                
				if (exceptionManager._tabSelected.toLowerCase().trim() == "secmaster")
				{
					Info.ExceptionSectype = $($('.row-left')[$(this).parent().parent().index()]).children('.typeExceptionCount').attr('typeid').trim();
					if (($('#smslidemenu_0').find('#rightMenuSection_smslidemenu_0_1').find('.filterOptions').length != exceptionManager._listSecurityTypeForService.length) && Info.ExceptionSectype == -1)
						Info.ExceptionSectype = exceptionManager._listSecurityTypeForService.join()
				}                    
				else if (exceptionManager._tabSelected.toLowerCase().trim() == "refmaster" || exceptionManager._tabSelected.toLowerCase().trim() == "partymaster" || exceptionManager._tabSelected.toLowerCase().trim() == "fundmaster")
				{
					Info.ExceptionEntityType = $($('.row-left')[$(this).parent().parent().index()]).children('.typeExceptionCount').attr('typeid').trim();
					if (($('#smslidemenu_0').find('#rightMenuSection_smslidemenu_0_1').find('.filterOptions').length != exceptionManager._listEntityTypeForService.length) && Info.ExceptionEntityType == -1)
						Info.ExceptionEntityType = exceptionManager._listEntityTypeForService.join()
				}
				
				Info.ExceptionAttribute = exAttr;
                Info.ShowFilter = 'true';
                Info.ParentPageIdentifier = 'Dashboard';
                Info.UserName = exceptionManager._usernameForService;
                Info.ExceptionState = exceptionManager._exceptionStateForService;
                Info.ModuleId = exceptionManager._moduleIDForService;

                if (Info.ExceptionDate == 4) {
                    Info.CustomDateSelected = smdatepickercontrol.getResponse($("#smdd_0")).selRadioOption;
                    Info.StartDate = exceptionManager._startDateForService;
                    Info.EndDate = exceptionManager._endDateForService;
                    if (Info.CustomDateSelected == 0)
                        Info.EndDate = null;
                    else if (Info.CustomDateSelected == 2)
                        Info.StartDate = null;
                }

                Info.ExceptionFilter = '1';
                if (exceptionManager._resolvedForService == 1 && exceptionManager._suppressedForService == -1)
                    Info.ExceptionFilter = '2';
                if (exceptionManager._resolvedForService == 0 && exceptionManager._suppressedForService == -1)
                    Info.ExceptionFilter = '3';
                if (exceptionManager._resolvedForService == -1 && exceptionManager._suppressedForService == 1)
                    Info.ExceptionFilter = '4';
                if (exceptionManager._resolvedForService == -1 && exceptionManager._suppressedForService == 0)
                    Info.ExceptionFilter = '5';

                // if (datefiltervalues['datefilterdataexceptionstile'].marked === '4') {
                // info.customdateselected = datefiltervalues['datefilterdataexceptionstile'].checked;
                // info.startdate = datefiltervalues['datefilterdataexceptionstile'].startdate;
                // info.enddate = datefiltervalues['datefilterdataexceptionstile'].enddate;
                // }

                var serviceData = { secIds: "", sessionIdentifierKey: sessionIdentifierKey, objFilterExceptionInfo: Info }
                //exceptionManager._serviceCount = exceptionManager._serviceCount + 1;
                exceptionManager._setValuesForException = serviceData;
                exceptionManager.callService('POST', exceptionManager._path + '/CommonService.asmx', 'SetValuesForException', serviceData, function () { exceptionManager.detailsGridSuccessCallback(sessionIdentifierKey) }, exceptionManager.FailureCallback);
                onServiceUpdating();
            });

            $('.typeExceptionCount').unbind('click').click(function (e) {
                var Info = {};
                Info.TagName = exceptionManager._tagNameForService;
                Info.ExceptionType = "-1";
                Info.ExceptionDate = smdatepickercontrol.getResponse($("#smdd_0")).selDateOption;

                if (exceptionManager._tabSelected.toLowerCase().trim() == "secmaster") {
                    Info.ExceptionSectype = $(this).attr('typeid').trim();
                    if (($('#smslidemenu_0').find('#rightMenuSection_smslidemenu_0_1').find('.filterOptions').length != exceptionManager._listSecurityTypeForService.length) && Info.ExceptionSectype == -1)
                        Info.ExceptionSectype = exceptionManager._listSecurityTypeForService.join()
                }
                else if (exceptionManager._tabSelected.toLowerCase().trim() == "refmaster" || exceptionManager._tabSelected.toLowerCase().trim() == "partymaster" || exceptionManager._tabSelected.toLowerCase().trim() == "fundmaster") {
                    Info.ExceptionEntityType = $(this).attr('typeid').trim();
                    if (($('#smslidemenu_0').find('#rightMenuSection_smslidemenu_0_1').find('.filterOptions').length != exceptionManager._listEntityTypeForService.length) && Info.ExceptionEntityType == -1)
                        Info.ExceptionEntityType = exceptionManager._listEntityTypeForService.join()
                }
                //if (exceptionManager._tabSelected.toLowerCase().trim() == "secmaster")
                //    Info.ExceptionSectype = $(this).attr('typeid').trim();
                //else if (exceptionManager._tabSelected.toLowerCase().trim() == "refmaster" || exceptionManager._tabSelected.toLowerCase().trim() == "partymaster" || exceptionManager._tabSelected.toLowerCase().trim() == "fundmaster")
                //    Info.ExceptionEntityType = $(this).attr('typeid').trim();
                Info.ExceptionAttribute = exAttr;
                Info.ShowFilter = 'true';
                Info.ParentPageIdentifier = 'Dashboard';
                Info.UserName = exceptionManager._usernameForService;
                Info.ExceptionState = exceptionManager._exceptionStateForService;
                Info.ModuleId = exceptionManager._moduleIDForService;

                if (Info.ExceptionDate == 4) {
                    Info.CustomDateSelected = smdatepickercontrol.getResponse($("#smdd_0")).selRadioOption;
                    Info.StartDate = exceptionManager._startDateForService;
                    Info.EndDate = exceptionManager._endDateForService;
                    if (Info.CustomDateSelected == 0)
                        Info.EndDate = null;
                    else if (Info.CustomDateSelected == 2)
                        Info.StartDate = null;
                }

                Info.ExceptionFilter = '1';
                if (exceptionManager._resolvedForService == 1 && exceptionManager._suppressedForService == -1)
                    Info.ExceptionFilter = '2';
                if (exceptionManager._resolvedForService == 0 && exceptionManager._suppressedForService == -1)
                    Info.ExceptionFilter = '3';
                if (exceptionManager._resolvedForService == -1 && exceptionManager._suppressedForService == 1)
                    Info.ExceptionFilter = '4';
                if (exceptionManager._resolvedForService == -1 && exceptionManager._suppressedForService == 0)
                    Info.ExceptionFilter = '5';

                // if (datefiltervalues['datefilterdataexceptionstile'].marked === '4') {
                // info.customdateselected = datefiltervalues['datefilterdataexceptionstile'].checked;
                // info.startdate = datefiltervalues['datefilterdataexceptionstile'].startdate;
                // info.enddate = datefiltervalues['datefilterdataexceptionstile'].enddate;
                // }

                var serviceData = { secIds: "", sessionIdentifierKey: sessionIdentifierKey, objFilterExceptionInfo: Info }
                //exceptionManager._serviceCount = exceptionManager._serviceCount + 1;
                exceptionManager._setValuesForException = serviceData;
                exceptionManager.callService('POST', exceptionManager._path + '/CommonService.asmx', 'SetValuesForException', serviceData, function () { exceptionManager.detailsGridSuccessCallback(sessionIdentifierKey) }, exceptionManager.FailureCallback);
                onServiceUpdating();

            });

            if (exceptionManager._tabSelected.toLowerCase() !== 'all systems') {
                exceptionTypeDiv.parent('.slimScrollDiv').css('width', exceptionManager._rowWidth);
                exceptionManager.attachMergeSecScrollbars(exceptionManager._windowWidth);
            }

            exceptionManager._serviceCount = exceptionManager._serviceCount - 1;
            if (exceptionManager._serviceCount === 0) {
                onServiceUpdated();
            }
        }
    }

    //SecMaster / RefMaster Details Grid Iframe Handling


    //Click Handler for All/SecM/RefM System Tabs
    $('.tabToggleClass').unbind('click').click(function (e) {
        if ($('#closeDetailsGrid').length > 0)
            $('#closeDetailsGrid').click();
        $('.tabToggleClass').removeClass('selectedTabClass');
        $(this).addClass('selectedTabClass');
        exceptionManager._prodName = '';
        exceptionManager._tabSelected = $(this).text().trim();
        //        if (exceptionManager._tabSelected.toLowerCase() == "all systems" || exceptionManager._tabSelected.toLowerCase() == "secmaster")
        //            $("#rightMenuDiv").css('display', '');
        //        else
        //            $("#rightMenuDiv").css('display', 'none');
        exceptionManager._count = 0;
        exceptionManager._tagNameForService = "-1";
        exceptionManager.init();
    });

    //Added By Paritosh
    $(window).unbind('message').bind('message', function (e) {
        if (e.originalEvent.data == 'resize') {
            exceptionManager.setTagsContainerWidth($('.DataExceptionsTagsDiv').children(".DataExceptionsTag"));
            if ($('#detailGrid')[0] != undefined)
                $('#detailGrid')[0].contentWindow.postMessage("resize", "*");
        }
    });
}
exceptionManager.setPath = function () {
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });
    exceptionManager._path = path + '/BaseUserControls/Service';
}
exceptionManager.gridScrollBarsFunction = {
    resetScrolls: function (scrollHeight, scrollWidth, changeScrollBarPosition) {
        var gridContainer = $('#outer-div');

        var height = scrollHeight + 57;
        if (!scrollHeight || scrollHeight <= 0) {
            var grid1 = gridContainer.find(".left-section-body").slimscroll({ height: '360px', railVisible: false, alwaysVisible: false, size: '7px' });
            gridContainer.find(".right-section-inner-body").slimscroll({ height: '360px', railVisible: true, alwaysVisible: true, size: '7px' });
            if (typeof scrollWidth !== 'undefined')
                //gridContainer.find(".right-section-inner").slimscrollH({ height: '360px', width: '700px', railVisible: false, alwaysVisible: true, size: '10px' });
                //gridContainer.find(".right-section-inner").slimscrollH({ height: '360px', width: exceptionManager._rowWidth, railVisible: false, alwaysVisible: false, size: '10px' });
				gridContainer.find(".right-section-inner").slimscrollH({ height: (height -20) , width: ($(window).width() - parseInt($('.left-section').css('width'))), railVisible: false, alwaysVisible: false, size: '10px' });
        }

        else {
            gridContainer.find(".left-section-body").css('height', scrollHeight);
            gridContainer.find(".right-section-inner-body").css({ 'height': scrollHeight, 'width': scrollWidth });

            var grid1 = gridContainer.find(".left-section-body").slimscroll({ height: scrollHeight, color: 'transparent', railVisible: false, alwaysVisible: false, size: '7px' });
            var grid2 = gridContainer.find(".right-section-inner-body").slimscroll({ height: scrollHeight, color: '#000', width: scrollWidth, railVisible: true, alwaysVisible: true, size: '7px' });

            gridContainer.find(".left-section-body").parent().height(gridContainer.find(".right-section-inner-body").parent().height());
            if (typeof scrollWidth !== 'undefined') {
                //gridContainer.find(".right-section-inner").slimscrollH({ height: height, width: '100%', railVisible: false, alwaysVisible: false, size: '10px' });
				gridContainer.find(".right-section-inner").slimscrollH({ height: (height -20) , width: ($(window).width() - parseInt($('.left-section').css('width'))), railVisible: false, alwaysVisible: false, size: '10px' });
            }
        }

        if ((typeof (changeScrollBarPosition) !== "undefined") && (changeScrollBarPosition === true)) {
            var verticalScrollBarContainer = $(".right-section-inner");
            var scrollBar = verticalScrollBarContainer.find(".slimScrollBar");
            var scrollRail = verticalScrollBarContainer.find(".slimScrollRail");
            var marginTop = $(".right-section-inner-body").offset().top;
            scrollBar.css("margin-top", (marginTop + "px")).css("position", "fixed");
            scrollRail.css("margin-top", (marginTop + "px")).css("position", "fixed");
            scrollRail.height($(".right-section-inner-body").height());
        }

        exceptionManager.gridScrollBarsFunction.syncScrolls(grid1, grid2);
    },
    syncScrolls: function (grid1, grid2) {
        $(grid1).on('scroll', function () {
            $(grid2).slimScroll({ scrollTo: $(grid1).scrollTop() });

        });

        $(grid2).on('scroll', function () {
            $(grid1).slimScroll({ scrollTo: $(grid2).scrollTop() });
        });
    },
    fixHorizontalScroll: function () {
        var gridContainer = $('#outer-div');
        var module = gridContainer;
        var off = $(module).offset();
        var top = parseInt(off.top) + 78;
        var left = parseInt(off.left) + $(module).width();

        var dataRows = gridContainer.find(".right-section-inner-body");
        var dataRowsHeight = $(dataRows).height();

        var html = '<div id="scrollbar"><div id="inner"><div id="in"></div></div></div>';

        var existingScrollbar = $('#rightSection').find('#scrollbar');

        if ($(existingScrollbar).length > 0)
            $(existingScrollbar).remove();

        gridContainer.find(".right-section-inner-body").append(html);
        $('#in').height($(dataRows)[0].scrollHeight);
        $('#inner').height(dataRowsHeight);

        var inner = $('#inner').slimScroll({ height: dataRowsHeight, railVisible: true, alwaysVisible: true, size: '10px' });
        $('#scrollbar').css({ 'height': dataRowsHeight, 'top': top, 'left': left });

        $(dataRows).parent().find('.slimScrollBar').addClass('importantRule');
        exceptionManager.gridScrollBarsFunction.specialSyncScrolls(gridContainer.find(".left-section-body"), dataRows, inner);
    },
    specialSyncScrolls: function (grid1, grid2, inner) {
        $(grid1).on('scroll', function () {
            $(grid2).slimScroll({ scrollTo: $(grid1).scrollTop(), color: 'transparent', railVisible: false, alwaysVisible: false, size: '10px' });
            $(inner).slimScroll({ scrollTo: $(grid1).scrollTop() });
        });

        $(grid2).on('scroll', function () {
            $(grid1).slimScroll({ scrollTo: $(grid2).scrollTop() });
            $(inner).slimScroll({ scrollTo: $(grid2).scrollTop() });
        });

        $(inner).on('scroll', function () {
            $(grid1).slimScroll({ scrollTo: $(inner).scrollTop() });
            $(grid2).slimScroll({ scrollTo: $(inner).scrollTop(), color: 'transparent', railVisible: false, alwaysVisible: false, size: '10px' });
        });
    }
}
exceptionManager.attachMergeSecScrollbars = function (containerWidth) {
    var windowHeight = $(window).height();

    exceptionManager.gridScrollBarsFunction.resetScrolls((windowHeight - 170), exceptionManager._rowWidth, true);
    //exceptionManager.gridScrollBarsFunction.fixHorizontalScroll();

    //Adjustments
    var gridContainer = $('#outer-div');
    //gridContainer.find(".smmergeduplicateMergeAllSecuritiesContanier").parent().addClass("smmergeduplicateHorizontalAlign");
    var leftWidth = gridContainer.find(".left-section").width();
    var rightWidth = gridContainer.find(".right-section").width();

    /*gridContainer.find("#inner").parent().parent().addClass("smmergeduplicateMergeSecMainScroll");
    gridContainer.find("#inner").parent().parent().css("top", "70px");
    gridContainer.find("#inner").parent().parent().css("left", ($(window).width() - 20) + "px");*/

    /*if ((leftWidth + rightWidth) < $(window).width()) {
    gridContainer.find(".slimScrollBarH").css("visibility", "hidden");
    gridContainer.find(".slimScrollRailH").css("visibility", "hidden");
    }*/
}
exceptionManager.callService = function (httpMethod, serviceServerString, methodName, parameters, ajaxSuccess, ajaxError, response, userContext, isCrossDomain,async) {
    var parametersString = '';
    var options = null;
    if (httpMethod.toUpperCase() == 'GET') {
        $.each(parameters, function (key, value) {
            parameters[key] = JSON.stringify(value, function (key, evaluateObject) {
                return evaluateObject === "" ? "" : evaluateObject
            });
            //parameters[key] = JSON.stringify(value);
        });
        if (response != null && response != undefined) {
            options = {
                type: 'GET',
                url: serviceServerString + '/' + methodName,
                processData: true,
                data: parameters,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: ajaxSuccess,
                error: ajaxError,
                context: userContext
            };
        }
        else {
            if (isCrossDomain) {
                //jQuery.support.cors = true;
                options = {
                    type: 'GET',
                    url: serviceServerString + '/' + methodName,
                    processData: true,
                    data: parameters,
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'jsonp',
                    success: ajaxSuccess,
                    error: ajaxError,
                    param1: userContext
                };
            }
            else {
                options = {
                    type: 'GET',
                    url: serviceServerString + '/' + methodName,
                    processData: true,
                    data: parameters,
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: ajaxSuccess,
                    error: ajaxError,
                    param1: userContext
                };
            }
        }
    }
    if (httpMethod.toUpperCase() == 'POST') {
        //serializedString = JSON.stringify(parameters);
        serializedString = JSON.stringify(parameters, function (key, evaluateObject) {
            return evaluateObject === "" ? "" : evaluateObject
        });
        if (isCrossDomain) {
            jQuery.support.cors = true;
            options = {
                type: 'POST',
                url: serviceServerString + '/' + methodName,
                processData: false,
                data: serializedString,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: ajaxSuccess,
                error: ajaxError,
                param1: userContext,
                beforeSend: function (xhr) {
                    xhr.setRequestHeader('OPTIONS', null);
                }
                //                xhrFields: {
                //                    withCredentials: true
                //                }
            };
        }
        else {
            options = {
                type: 'POST',
                url: serviceServerString + '/' + methodName,
                processData: false,
                data: serializedString,
                contentType: 'application/json;',
                dataType: 'json',
                async: async,
                success: ajaxSuccess,
                error: ajaxError,
                param1: userContext
            };
        }
    }
    $.ajax(options);
}
//Set Resolve and Supress Values based on selected Exception State(Open/Resolve/Unresolve/Suppress/Unsuppress)
exceptionManager.computeExceptionState = function () {
    var eState = exceptionManager._exceptionStateForService;
    exceptionManager._exceptionStateList.filter(function (ele, index) {
        ele["isSelected"] = "false";
    });
    switch (eState) {
        case "1":
            exceptionManager._resolvedForService = 0;
            exceptionManager._suppressedForService = 0;
            exceptionManager._exceptionStateList.filter(function (ele, index) {
                if (ele["value"] === "1")
                    ele["isSelected"] = "true";
            });
            break;
        case "2":
            exceptionManager._suppressedForService = -1;
            exceptionManager._resolvedForService = 1;
            exceptionManager._exceptionStateList.filter(function (ele, index) {
                if (ele["value"] === "2")
                    ele["isSelected"] = "true";
            });
            break;
        case "3":
            exceptionManager._resolvedForService = 0;
            exceptionManager._suppressedForService = -1;
            exceptionManager._exceptionStateList.filter(function (ele, index) {
                if (ele["value"] === "3")
                    ele["isSelected"] = "true";
            });
            break;
        case "4":
            exceptionManager._resolvedForService = -1;
            exceptionManager._suppressedForService = 1;
            exceptionManager._exceptionStateList.filter(function (ele, index) {
                if (ele["value"] === "4")
                    ele["isSelected"] = "true";
            });
            break;
        case "5":
            exceptionManager._resolvedForService = -1;
            exceptionManager._suppressedForService = 0;
            exceptionManager._exceptionStateList.filter(function (ele, index) {
                if (ele["value"] === "5")
                    ele["isSelected"] = "true";
            });
            break;
    }
}

//Set Resolve and Supress Values based on selected Exception State(Open/Resolve/Unresolve/Suppress/Unsuppress)
exceptionManager.RefcomputeExceptionState = function () {
    //    var eState = exceptionManager._exceptionStateForService;
    //    switch (eState) {
    //        case "1":
    //            exceptionManager._suppressedForService = 0;
    //            break;
    //        case "2":
    //            exceptionManager._suppressedForService = 1;
    //            break;
    //        case "4":
    //            exceptionManager._resolvedForService = 0;
    //            break;
    //        case "8":
    //            exceptionManager._resolvedForService = 1;
    //            break;
    //        case "3":
    //            exceptionManager._resolvedForService = 0;
    //            exceptionManager._suppressedForService = 0;
    //            break;
    //    }
    var eState = exceptionManager._exceptionStateForService;
    exceptionManager._exceptionRefStateList.filter(function (ele, index) {
        ele["isSelected"] = "false";
    });
    switch (eState) {
        case "5":
            exceptionManager._resolvedForService = 0;
            exceptionManager._suppressedForService = 0;
            exceptionManager._exceptionRefStateList.filter(function (ele, index) {
                if (ele["value"] === "5")
                    ele["isSelected"] = "true";
            });
            break;
        case "8":
            exceptionManager._resolvedForService = 1;
            exceptionManager._suppressedForService = -1;
            exceptionManager._exceptionRefStateList.filter(function (ele, index) {
                if (ele["value"] === "8")
                    ele["isSelected"] = "true";
            });
            break;
        case "4":
            exceptionManager._resolvedForService = 0;
            exceptionManager._suppressedForService = -1;
            exceptionManager._exceptionRefStateList.filter(function (ele, index) {
                if (ele["value"] === "4")
                    ele["isSelected"] = "true";
            });
            break;
        case "2":
            exceptionManager._resolvedForService = -1;
            exceptionManager._suppressedForService = 1;
            exceptionManager._exceptionRefStateList.filter(function (ele, index) {
                if (ele["value"] === "2")
                    ele["isSelected"] = "true";
            });
            break;
        case "1":
            exceptionManager._resolvedForService = -1;
            exceptionManager._suppressedForService = 0;
            exceptionManager._exceptionRefStateList.filter(function (ele, index) {
                if (ele["value"] === "1")
                    ele["isSelected"] = "true";
            });
            break;
    }
}

//Side Menu All Systems Apply Callback
exceptionManager.allSectionsSideMenuApplyCallback = function () {
    var selectedItems = exceptionManager.getSideMenuItemsAllSystems();
    exceptionManager._listSecurityTypeForService = selectedItems.sectypeID;
    exceptionManager._listEntityTypeForService = selectedItems.entityTypeID;
    exceptionManager._exceptionStateForService = selectedItems.exceptionState;
    exceptionManager.computeExceptionState();
    exceptionManager._count = 0;
    exceptionManager.init();
}
//Side Menu SecMaster Apply Callback
exceptionManager.secmSideMenuApplyCallback = function () {
    var selectedItems = exceptionManager.getSideMenuItemsSecM();
    exceptionManager._listSecurityTypeForService = selectedItems.sectypeID;
    exceptionManager._listAttributeForService = selectedItems.attrID;
    exceptionManager._listSecMAttributeForService = selectedItems.attrID;
    exceptionManager._exceptionStateForService = selectedItems.exceptionState;
    exceptionManager.computeExceptionState();
    exceptionManager._count = 0;
    exceptionManager.init();
}
//Side Menu RefMaster Apply Callback
exceptionManager.refmSideMenuApplyCallback = function () {
    var selectedItems = exceptionManager.getSideMenuItemsRefM();
    exceptionManager._listEntityTypeForService = selectedItems.entityTypeID;
    exceptionManager._listAttributeForServiceRef = selectedItems.attrID;
    exceptionManager._listRefMAttributeForService = selectedItems.attrID;
    exceptionManager._exceptionStateForService = selectedItems.exceptionState;
    //exceptionManager.computeExceptionState();
    exceptionManager.RefcomputeExceptionState();
    exceptionManager._count = 0;
    exceptionManager.init();
}
exceptionManager.getSideMenuItemsAllSystems = function () {
    var returnObj = {};
    var sections = $("#smslidemenu_0").find(".filterSection");
    $.each(sections, function (index, element) {
        var currId = $(element).attr('id');
        switch (index) {
            case 0:
                returnObj.exceptionState = smslidemenu.getSelectedItemsValue(currId)[0];
                break;
            case 1:
                var sIDs = smslidemenu.getSelectedItemsValue(currId);
                if (sIDs.length === 0) {
                    sIDs = ['-2'];
                }
                returnObj.sectypeID = sIDs;
                break;
            case 2:
                var eIDs = smslidemenu.getSelectedItemsValue(currId)
                if (eIDs.length === 0) {
                    eIDs = ['-2'];
                }
                returnObj.entityTypeID = eIDs;
                break;
            default:
                break;
        }
    });
    return returnObj;
}
exceptionManager.getSideMenuItemsSecM = function () {
    var returnObj = {};
    var sections = $("#smslidemenu_0").find(".filterSection");
    $.each(sections, function (index, element) {
        var currId = $(element).attr('id');
        switch (index) {
            case 0:
                returnObj.exceptionState = smslidemenu.getSelectedItemsValue(currId)[0];
                break;
            case 1:
                var sIDs = smslidemenu.getSelectedItemsValue(currId);
                if (sIDs.length === 0) {
                    sIDs = ['-2'];
                }
                returnObj.sectypeID = sIDs;
                break;
            case 2:
                returnObj.attrID = smslidemenu.getSelectedItemsValue(currId);
                break;
            default:
                break;
        }
    });
    return returnObj;
}
exceptionManager.getSideMenuItemsRefM = function () {
    var returnObj = {};
    var sections = $("#smslidemenu_0").find(".filterSection");
    $.each(sections, function (index, element) {
        var currId = $(element).attr('id');
        switch (index) {
            case 0:
                returnObj.exceptionState = smslidemenu.getSelectedItemsValue(currId)[0];
                break;
            case 1:
                returnObj.entityTypeID = smslidemenu.getSelectedItemsValue(currId);
                break;
            case 2:
                returnObj.attrID = smslidemenu.getSelectedItemsValue(currId);
                break;
            default:
                break;
        }
    });
    return returnObj;
}
exceptionManager.sectypeSlideMenuClose = function () {
    var selectedItems = exceptionManager.getSideMenuItemsSecM();
    exceptionManager._listSecurityTypeForService = selectedItems.sectypeID;
};
exceptionManager.entityTypeSlideMenuClose = function () {
    var selectedItems = exceptionManager.getSideMenuItemsSecM();
    exceptionManager._listEntityTypeForService = selectedItems.sectypeID;
}
exceptionManager.getAttributesBasedOnSectype = function () {
    var secAttr = '', secTypes = "-1", secAttrAll = false;
	if(exceptionManager._listAttributeForService !== null && exceptionManager._listAttributeForService.find(x=> x== '-1') != undefined)
		exceptionManager._listAttributeForService = null;
		
    if (exceptionManager._listSecurityTypeForService !== null) {
        secTypes = exceptionManager._listSecurityTypeForService.join();
    }

    var methodInputParam = {
        secTypeIds: secTypes,
        userName: exceptionManager._usernameForService,
        selectedAttributes: exceptionManager._listAttributeForService
    }

    exceptionManager.callService('POST', exceptionManager._path + '/ExceptionManagerService.svc', 'GetAllAttributesBasedOnSectypeSelection', methodInputParam, getAttributesSuccessCallback, exceptionManager.FailureCallback, secAttr, null, null, false)
    function getAttributesSuccessCallback(secAttr) {
        //To Select all SecTypes and Attributes if all options come as selected from backend
        var selAttr = 0;
        for (var attr in secAttr.d) {
            if (secAttr.d[attr].isSelected === "true")
            { selAttr++; }
        }
        if (selAttr === (secAttr.d.length)) {
            secAttrAll = true;
        }
        else if (selAttr === 0) {
            secAttrAll = true;
        }

        smslidemenu.changeRightMenuContent(secAttr.d, $("#rightMenuSection_smslidemenu_0_2"), "All Attributes", "down", null, secAttrAll, true, null, null, 0);
    }
}
exceptionManager.getAttributesBasedOnEntityType = function () {
    var entityAttr = '', eTypes = '-1', refAttrAll = false;
	
	if(exceptionManager._listAttributeForServiceRef !== null && exceptionManager._listAttributeForServiceRef.find(x=> x== '-1') != undefined)
		exceptionManager._listAttributeForServiceRef = null;
		
    if (exceptionManager._listEntityTypeForService !== null) {
        eTypes = exceptionManager._listEntityTypeForService.join();
    }

    var methodInputParam = {
        entityTypeIds: eTypes,
        selectedAttributes: exceptionManager._listAttributeForServiceRef
    }

    exceptionManager.callService('POST', exceptionManager._path + '/ExceptionManagerService.svc', 'GetAllAttributesBasedOnEntityTypeSelection', methodInputParam, getEntityAttrSuccessCallback, exceptionManager.FailureCallback, entityAttr, null, null, false)
    function getEntityAttrSuccessCallback(entityAttr) {
        //To Select all Entity Types and Attributes if all options come as selected from backend
        var selAttr = 0;
        for (var attr in entityAttr.d) {
            if (entityAttr.d[attr].isSelected === "true")
            { selAttr++; }
        }

        if (selAttr === (entityAttr.d.length)) {
            refAttrAll = true;
        }
        else if (selAttr === 0) {
            refAttrAll = true;
        }

        smslidemenu.changeRightMenuContent(entityAttr.d, $("#rightMenuSection_smslidemenu_0_2"), "All Attributes", "down", null, refAttrAll, true, null, null, 0);
    }
}
exceptionManager.tagHoverIn = function (e) {
    var target = $(e.target);
    if (!target.hasClass("DataExceptionsTag")) {
        target = target.parent();
    }
    target.find(".DataExceptionsTagTitle").addClass("EM_tagsHover");
    target.find(".DataExceptionsTagCount").addClass("EM_tagsHover").css("text-decoration", "underline");
};
exceptionManager.tagHoverOut = function (e) {
    var target = $(e.target);
    if (!target.hasClass("DataExceptionsTag")) {
        target = target.parent();
    }
    target.find(".DataExceptionsTagTitle").removeClass("EM_tagsHover");
    target.find(".DataExceptionsTagCount").removeClass("EM_tagsHover").css("text-decoration", "none");
};
exceptionManager.adjustArrowLeft = function () {
    var DataExceptionsTagsDiv = $('.DataExceptionsTagsDiv')
    var selectedDiv = null;
    $.each(DataExceptionsTagsDiv.children(".DataExceptionsTag"), function (index, ele) {
        if ($(ele).hasClass("DataExceptionsTagSelected")) {
            selectedDiv = $(ele);
            return;
        }
    });

    if (selectedDiv !== null) {
        var arrowLeft = (selectedDiv.offset().left - selectedDiv.parent().offset().left) + ((selectedDiv.width() / 2) - 8);
        $('.dashboardDownArrow').css('left', arrowLeft + "px");
    }
};

//$(document).ready(function () {
//    exceptionManager.setPath();
//    exceptionManager.initDateControl();
//    exceptionManager.init();
//});

$(document).click(function (e) {
    var currTarget = $(e.target);
    if (currTarget.closest(".smslidemenu_filterDiv").length === 0) {
        if ($("#smslidemenu_0").css("display") !== "none") {
            smslidemenu.hide("smslidemenu_0");
        }
    }
});