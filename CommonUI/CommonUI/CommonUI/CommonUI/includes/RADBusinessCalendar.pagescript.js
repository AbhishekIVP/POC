
RADCalendarConfig = {};
RADCalendarConfig.initialize = function (initObj) {
    $.extend(RADCalendarConfig, initObj);
    if (!initObj.IsIagoDependent) {        
        
        var $rCalenderTabDiv = $("<div>", {
            id: "RCalenderTabDiv",
            class: "RCalenderTab"
        });
        $("#" + RADCalendarConfig.contentBodyId).empty();
        $("#" + RADCalendarConfig.contentBodyId).append($rCalenderTabDiv);
    }

    var obj = new RADBusinessCalendar();
    obj.init();
};



var RADBusinessCalendar = function () {

};

RADBusinessCalendar.prototype.init = function (sandBox) {
    RADBusinessCalendar.instance = this;
    RADBusinessCalendar.instance.LoadTabInfo();
}

RADBusinessCalendar.prototype.LoadTabInfo = function () {

    RADBusinessCalendar.instance.AuthorizationPrivilegesCalender = [];
    RADBusinessCalendar.instance.AllCalenderDetails = null;
    RADBusinessCalendar.instance.CurrentSelectedTabId = null;
    RADBusinessCalendar.instance.CurrentSelectedCalenderId = null;
    RADBusinessCalendar.instance.CurrentSelectedHolidayId = null;
    RADBusinessCalendar.instance.CurrentSelectedHolidayName = null;
    RADBusinessCalendar.instance.CurrentSelectedExcludeHolidayName = null;
    RADBusinessCalendar.instance.CurrentCalenderDetails = null;
    RADBusinessCalendar.instance.isCreateHoliday = null;
    RADBusinessCalendar.instance.isExcludedHoliday = null;
    RADBusinessCalendar.instance.isGridRowSelected = null;
    RADBusinessCalendar.instance.ServiceAddress = RADCalendarConfig.baseUrl + "/Resources/Services/RADBusinessCalendar.svc";
    RADBusinessCalendar.instance.LoaderDiv = null;
    RADBusinessCalendar.instance.AllEntityTypes = [];
    RADBusinessCalendar.instance.CurrentEntityId = 0;
    RADBusinessCalendar.instance.CurrentEntityName = "";
    RADBusinessCalendar.instance.CurrentAttributeRealName = "";
    RADBusinessCalendar.instance.CurrentAttributeValueId = "";
    RADBusinessCalendar.instance.CurrentEntityIdHoliday = 0;
    RADBusinessCalendar.instance.CurrentStartDateColumn ="";
    RADBusinessCalendar.instance.CurrentHolidayNameColumn ="";
    RADBusinessCalendar.instance.EntityUniqueColumns = [];
    RADBusinessCalendar.instance.CurrentAttributeData = [];
    RADBusinessCalendar.instance.LegsByEntityId = {};
    RADBusinessCalendar.instance.AppDefaultDateTime = "";


    RADBusinessCalendar.prototype.tabInfo = {
        tab: [
                {
                    id: "Calender",
                    name: "Calender",
                    isDefault: true
                },
                {
                    id: "HolidayInc",
                    name: "Holidays"
                }
            ]
    };
    RADBusinessCalendar.instance.BindTabs();
}

RADBusinessCalendar.prototype.BindTabs = function () {

    if (RADCalendarConfig.IsIagoDependent) {
        if ($("#pageHeaderTabPanel").data("iago-widget-tabs") != null)
            $("#pageHeaderTabPanel").data("iago-widget-tabs").destroy();
        $("#RCalenderTabDiv").css("display", "none");
        $("#pageHeaderTabPanel").tabs({
            tabSchema: RADBusinessCalendar.instance.tabInfo,
            tabClickHandler: RADBusinessCalendar.instance.OnPageLoad,
            tabContentHolder: "contentBody"
        });

    }

    else {
        $(".iago-page-title").css("display", "none");
        $("#RCalenderTabDiv").empty();
        $("#RCalenderTabDiv").toolKittabs({
            tabSchema: { tab: RADBusinessCalendar.instance.tabInfo.tab },
            tabClickHandler: RADBusinessCalendar.instance.OnPageLoad,
            tabContentHolder: RADCalendarConfig.contentBodyId
        });

    }

    
}

RADBusinessCalendar.prototype.OnPageLoad = function (selectedTabId, tabContentContainer) {

    RADBusinessCalendar.instance.CurrentSelectedTabId = selectedTabId;

    if (selectedTabId == "Calender") {
        RADBusinessCalendar.instance.OnPageLoadCalender();
    }
    else {
        RADBusinessCalendar.instance.OnPageLoadHolidayIncExc();
    }
}

RADBusinessCalendar.prototype.OnPageLoadHolidayIncExc = function () {

    $.ajax({
        url: RADBusinessCalendar.instance.ServiceAddress + '/GetTagTemplate',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ fileName: 'RADHolidaysBCalendar.html'})
    }).then(function (responseText) {

        //$("#" + RADCalendarConfig.contentBodyId).empty();
        $("#RADBCalenderMainDiv").remove();
        $("#RADHolidayInclMainDiv").remove();
        $("#" + RADCalendarConfig.contentBodyId).append(responseText.d);
        RADBusinessCalendar.instance.GetAuthorizationPrivileges(RADBusinessCalendar.instance.CurrentSelectedTabId);

        var $loaderDiv = $("<div>", {
            id: "RADBCalenderLoaderDiv",
            class: "RADBCalenderLoader RADBCalenderHiddenClass"
        });
        
        $("#" + RADCalendarConfig.contentBodyId).append($loaderDiv);        
        RADBusinessCalendar.instance.CalculateHeight();
        RADBusinessCalendar.instance.BindEventsCommon();
    });
}

RADBusinessCalendar.prototype.GetApplicationDateFormat = function () {
    $.ajax({
        url: RADBusinessCalendar.instance.ServiceAddress + '/GetApplicationDateFormat',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        RADBusinessCalendar.instance.AppDefaultDateTime = responseText.d;
    });
}

RADBusinessCalendar.prototype.OnPageLoadCalender = function () {
    $.ajax({
        url: RADBusinessCalendar.instance.ServiceAddress + '/GetTagTemplate',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ fileName: 'RADBusinessCalendar.html'})
    }).then(function (responseText) {

        //$("#" + RADCalendarConfig.contentBodyId).empty();
        $("#RADBCalenderMainDiv").remove();
        $("#RADHolidayInclMainDiv").remove();
        $("#" + RADCalendarConfig.contentBodyId).append(responseText.d);        
        RADBusinessCalendar.instance.GetAuthorizationPrivileges(RADBusinessCalendar.instance.CurrentSelectedTabId);
       
        $("#RADBCalenderNameParentDiv").html('Calender Details');

        var $loaderDiv = $("<div>", {
            id: "RADBCalenderLoaderDiv",
            class: "RADBCalenderLoader RADBCalenderHiddenClass"
        });

        $("#" + RADCalendarConfig.contentBodyId).append($loaderDiv);
        RADBusinessCalendar.instance.CalculateHeight();
        RADBusinessCalendar.instance.BindEventsCommon();
        RADBusinessCalendar.instance.GetApplicationDateFormat();
    });
}

RADBusinessCalendar.prototype.GetAuthorizationPrivileges = function (currentSelectedTab) {

    RADBusinessCalendar.instance.AuthorizationPrivilegesCalender = [];

    $.ajax({       
        url: RADBusinessCalendar.instance.ServiceAddress + '/GetAuthorizationPrivileges',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'

    }).then(function (responseText) {

        if (responseText.d == "admin") {           
            RADBusinessCalendar.instance.AuthorizationPrivilegesCalender.push('Add Calender', 'Delete Calender',
                                                                               'Update Calender', 'Add Holiday', 'Update Holiday',
                                                                               'Delete Holiday', 'Exclude Holiday');
        }
        else {
            var authPrivileges = [];
            if (responseText.d.length > 0)
                authPrivileges = JSON.parse(responseText.d);
            RADBusinessCalendar.instance.AuthorizationPrivilegesCalender = []

            for (var i = 0; i < authPrivileges.length; i++) {
                for (var j = 0; j < authPrivileges[i].Privileges.length; j++)
                    RADBusinessCalendar.instance.AuthorizationPrivilegesCalender.push(authPrivileges[i].Privileges[j])
            }
        }

        if (currentSelectedTab == "Calender") {
            RADBusinessCalendar.instance.ShowHideByPrivilegeTypeCalender();
        }
        else {
            RADBusinessCalendar.instance.ShowHideByPrivilegeTypeHoliday();
        }

        RADBusinessCalendar.instance.GetAllCalenders();
    });
}

RADBusinessCalendar.prototype.ShowHideByPrivilegeTypeHoliday = function () {

    (RADBusinessCalendar.instance.AuthorizationPrivilegesCalender.indexOf("Add Holiday") != -1)
     ? $("#RADHolidayAddNewIncButtonParentDiv").removeClass("RADBCalenderDisplayNone")
    : $("#RADHolidayAddNewIncButtonParentDiv").addClass("RADBCalenderDisplayNone");

    if (RADBusinessCalendar.instance.isGridRowSelected) {
        (RADBusinessCalendar.instance.AuthorizationPrivilegesCalender.indexOf("Update Holiday") != -1)
         ? $("#RADHolidayEditIncButtonParentDiv").removeClass("RADBCalenderDisplayNone")
        : $("#RADHolidayEditIncButtonParentDiv").addClass("RADBCalenderDisplayNone");

        (RADBusinessCalendar.instance.AuthorizationPrivilegesCalender.indexOf("Delete Holiday") != -1)
         ? $("#RADHolidayIncDeleteButtonParentDiv").removeClass("RADBCalenderDisplayNone")
        : $("#RADHolidayIncDeleteButtonParentDiv").addClass("RADBCalenderDisplayNone");

        if (!RADBusinessCalendar.instance.isExcludedHoliday) {
            (RADBusinessCalendar.instance.AuthorizationPrivilegesCalender.indexOf("Exclude Holiday") != -1)
             ? $("#RADHolidayExcludeButtonParentDiv").removeClass("RADBCalenderDisplayNone")
             : $("#RADHolidayExcludeButtonParentDiv").addClass("RADBCalenderDisplayNone");
            $("#RADHolidayIncDeleteExclButtonParentDiv").addClass("RADBCalenderDisplayNone");
        }
        else {
            $("#RADHolidayExcludeButtonParentDiv").addClass("RADBCalenderDisplayNone");
            (RADBusinessCalendar.instance.AuthorizationPrivilegesCalender.indexOf("Exclude Holiday") != -1)
                ? $("#RADHolidayIncDeleteExclButtonParentDiv").removeClass("RADBCalenderDisplayNone")
                : $("#RADHolidayIncDeleteExclButtonParentDiv").addClass("RADBCalenderDisplayNone");
        }
    }
    else {
        $("#RADHolidayEditIncButtonParentDiv").addClass("RADBCalenderDisplayNone");
        $("#RADHolidayIncDeleteButtonParentDiv").addClass("RADBCalenderDisplayNone");
        $("#RADHolidayExcludeButtonParentDiv").addClass("RADBCalenderDisplayNone");
        $("#RADHolidayIncDeleteExclButtonParentDiv").addClass("RADBCalenderDisplayNone");

    }
}

RADBusinessCalendar.prototype.ShowHideByPrivilegeTypeCalender = function () {

    (RADBusinessCalendar.instance.AuthorizationPrivilegesCalender.indexOf("Add Calender") != -1)
     ? $("#RADCreateBCalenderDiv").removeClass("RADBCalenderDisplayNone")
     : $("#RADCreateBCalenderDiv").addClass("RADBCalenderDisplayNone");

    (RADBusinessCalendar.instance.AuthorizationPrivilegesCalender.indexOf("Delete Calender") != -1)
     ? $("#RADBCalenderDeleteButtonParentDiv").removeClass("RADBCalenderDisplayNone")
     : $("#RADBCalenderDeleteButtonParentDiv").addClass("RADBCalenderDisplayNone");

    (RADBusinessCalendar.instance.AuthorizationPrivilegesCalender.indexOf("Update Calender") != -1)
     ? $("#RADBCalenderEditButtonParentDiv").removeClass("RADBCalenderDisplayNone")
     : $("#RADBCalenderEditButtonParentDiv").addClass("RADBCalenderDisplayNone");
       
}

RADBusinessCalendar.prototype.GetAllCalenders = function () {

    $.ajax({
        url: RADBusinessCalendar.instance.ServiceAddress + '/GetAllCalendars',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {

        if (responseText.d != null && responseText.d != "" && responseText.d != "[]") {

            var calenderDetails = JSON.parse(responseText.d);
            RADBusinessCalendar.instance.AllCalenderDetails = calenderDetails;
            var currentSelectedCalenderId = RADBusinessCalendar.instance.BindLeftMenu(calenderDetails);

            if (RADBusinessCalendar.instance.CurrentSelectedTabId == "Calender") {
                RADBusinessCalendar.instance.GetCurrentCalenderDetails(currentSelectedCalenderId);
            }
            else {

                RADBusinessCalendar.instance.GetCurrentCalenderHolidayDetails(currentSelectedCalenderId);
            }
        }
    });
}

RADBusinessCalendar.prototype.BindLeftMenu = function (calenderDetails) {

    $('#RADBCalenderLeftSectionContentDiv').empty();

    for (var i = 0; i < calenderDetails.length; i++) {

        var currentCalender = calenderDetails[i];

        var $currentTileParent = $("<div>", { class: "RADBCalenderTileParent" });
        var $currentTile = $("<div>", { class: "RADBCalenderTile" });
        var $currentTileName = $("<div>", {
                                id: "RADBCalenderId" + currentCalender.CalendarId,
                                class: "RADBCalenderName",
                                text: currentCalender.CalendarName
        });       

        var $currentTileType = $("<div>", {
            class: "RADBCalenderDesc",
            text: currentCalender.CalendarTypeId == 1 ? "Custom" : "Reference Master"
        });

        $currentTileName.attr("radbcalenderid", currentCalender.CalendarId);  
        $currentTile.append($currentTileName);
        $currentTile.append($currentTileType);
        $currentTileParent.append($currentTile);
        $('#RADBCalenderLeftSectionContentDiv').append($currentTileParent);
        if (i == 0) {
            RADBusinessCalendar.instance.CurrentSelectedCalenderId = currentCalender.CalendarId;
            $("#RADBCalenderId" + currentCalender.CalendarId).closest(".RADBCalenderTile").addClass('RADBCalenderTileActive');
            $("#RADBCalenderId" + currentCalender.CalendarId).closest(".RADBCalenderTile")
                .append("<div id=\"arr\" class=\"fa fa-caret-right RADBCalenderArrow-Right \"></div>");
        }
    }

    $(".RADBCalenderTile").unbind().click(function (event) {

        $("#RADBCalenderSaveButtonParentDiv").addClass("RADBCalenderDisplayNone");
        $("#RADBCalenderCancelButtonParentDiv").addClass("RADBCalenderDisplayNone");

        $("#RADBCalenderDeleteButtonParentDiv").removeClass("RADBCalenderDisplayNone");
        $("#RADBCalenderEditButtonParentDiv").removeClass("RADBCalenderDisplayNone");
                
        $("#RADBCalenderCreateParentDiv").hide();
        $("#RADBCalenderCreateRefMasterParentDiv").hide();
        $("#RADBCalenderDetailsParentDiv").show();
        $("#RADBCalenderNameParentDiv").html('Calendar Details');

        
        if (RADBusinessCalendar.instance.CurrentSelectedTabId == "Calender") {
            RADBusinessCalendar.instance.ShowHideByPrivilegeTypeCalender();
        }
        else {
            RADBusinessCalendar.instance.ShowHideByPrivilegeTypeHoliday();
        }


        $(".RADBCalenderArrow-Right").remove();
        $(".RADBCalenderTileActive").removeClass("RADBCalenderTileActive");
        $(event.target).closest(".RADBCalenderTile").addClass("RADBCalenderTileActive");
        $(event.target).closest(".RADBCalenderTile").append("<div id=\"arr\" class=\"fa fa-caret-right RADBCalenderArrow-Right \"></div>");
        var currentSelectedCalenderId = $(event.target).closest(".RADBCalenderTile").find(".RADBCalenderName").attr('radbcalenderid');
        RADBusinessCalendar.instance.CurrentSelectedCalenderId = currentSelectedCalenderId;

        if (RADBusinessCalendar.instance.CurrentSelectedTabId == "Calender") {
            RADBusinessCalendar.instance.GetCurrentCalenderDetails(currentSelectedCalenderId);
        }
        else {
            RADBusinessCalendar.instance.GetCurrentCalenderHolidayDetails(currentSelectedCalenderId);
        }
    });
    return RADBusinessCalendar.instance.CurrentSelectedCalenderId;
}

RADBusinessCalendar.prototype.GetCurrentCalenderDetails = function (currentSelectedCalenderId) {

    $.ajax({
        url: RADBusinessCalendar.instance.ServiceAddress + '/GetCalendarById',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ calendarId: currentSelectedCalenderId })
    }).then(function (responseText) {
        if (responseText.d != null && responseText.d != "" && responseText.d != "[]") {
            var currentCalenderDetails = JSON.parse(responseText.d);
            RADBusinessCalendar.instance.CurrentCalenderDetails = currentCalenderDetails;
            RADBusinessCalendar.instance.CurrentSelectedCalenderId = currentCalenderDetails.CalendarId;
            RADBusinessCalendar.instance.BindCurrentCalenderDetails(currentCalenderDetails);
        }
    })
}

RADBusinessCalendar.prototype.GetCurrentCalenderHolidayDetails = function (currentSelectedCalenderId) {

    $("#RADHolidayInclGridDiv").empty();    
    $("#RADBCalenderLoaderDiv").removeClass("RADBCalenderHiddenClass");

    var url = RADCalendarConfig.baseUrl + "/Resources/Services/RADBusinessCalendar.svc";
    $.ajax({
        url: url + '/GetHolidaysForCalender',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ calenderId: currentSelectedCalenderId })
    }).then(function (responseText) {
        if (responseText.d != "") {
            var gridInfo = JSON.parse(responseText.d);
            gridInfo.GridId = "RADHolidayInclGridDiv";
            neogridloader.create("RADHolidayInclGridDiv", "Test", gridInfo, "");
        }
        $("#RADBCalenderLoaderDiv").addClass("RADBCalenderHiddenClass");
    });
}

RADBusinessCalendar.prototype.OnRADGridRenderCompleteRef = function () {
    RADBusinessCalendar.instance.CalculateGridHeight();
    $(".RADHolidayAddNewIncButtonParent").addClass("RADBCalenderDisplayNone");
}

RADBusinessCalendar.prototype.OnRADGridRenderComplete = function () {
    var isExcluded = false;    
    RADBusinessCalendar.instance.CalculateGridHeight();
    RADBusinessCalendar.instance.isExcludedHoliday = false;
    RADBusinessCalendar.instance.isGridRowSelected = false;
    if ($($(".xlneocheckedRow")[1]).length > 0) {
        RADBusinessCalendar.instance.isGridRowSelected = true;

        if ($($(".xlneocheckedRow")[1]).find('div[columnname=is_excluded]').length > 0) {
            var isExcludedHoliday = $($(".xlneocheckedRow")[1]).find('div[columnname=is_excluded]').html().trim();
            isExcluded = (isExcludedHoliday == "True") ? true : false;
        }
        if (isExcluded) {
            RADBusinessCalendar.instance.isExcludedHoliday = true;
        }
        else {
            RADBusinessCalendar.instance.isExcludedHoliday = false;
        }
    }
    RADBusinessCalendar.instance.ShowHideByPrivilegeTypeHoliday();
}


RADBusinessCalendar.prototype.BindCurrentCalenderDetails = function (currentCalenderDetails) {

    $("#RADPropCalenderNameValueDiv").html(currentCalenderDetails.CalendarName);
    $("#RADPropCalenderDescValueDiv").html(currentCalenderDetails.CalendarDescription);

    if (currentCalenderDetails.CalendarTypeId == 1) {
        $("#RADPropCalenderTypeCustom")[0].checked = true;
        $("#RADPropCalenderTypeRefMaster")[0].checked = false;
    }
    else {
        $("#RADPropCalenderTypeRefMaster")[0].checked = true;
        $("#RADPropCalenderTypeCustom")[0].checked = false;
    }
    RADBusinessCalendar.instance.SetDefaultWeekHolidays(currentCalenderDetails.DefaultWeekHolidays, false);
}

RADBusinessCalendar.prototype.SetDefaultWeekHolidays = function (defaultWeekHolidays, isEditMode) {
    var defaultWeekHolidaysControls = [];

    if (isEditMode)
        defaultWeekHolidaysControls = $("#RADPropCreateDefWeekHolidaysParentDiv").find('input[type=checkbox]');        
    else        
        defaultWeekHolidaysControls = $("#RADPropDefWeekHolidaysParentDiv").find('input[type=checkbox]');

    var defaultWeekHolidaysArray = new Array(defaultWeekHolidays.length);
    for (var i = 0; i < defaultWeekHolidays.length; i++)
    {
        defaultWeekHolidaysArray[i] = defaultWeekHolidays.charAt(i);
    }

    for (var i = 0; i < defaultWeekHolidaysControls.length; i++)
    {
        var currentCheckBox = defaultWeekHolidaysControls[i];
        if (defaultWeekHolidaysArray.indexOf(i.toString()) == -1)
            $(currentCheckBox)[0].checked = false
        else
            $(currentCheckBox)[0].checked = true;
    }
}

RADBusinessCalendar.prototype.BindEventsCommon = function () {

    $(".RADBCalenderMain,.RADHolidayInclMain").unbind().click(function (event) {

        if ($(event.target).closest(".RADBCalenderScrollingDiv").length > 0) {
            RADBusinessCalendar.instance.ScrollData(event);
        }
        else if ($(event.target).closest("#btnRADCreateBCalenderId").length > 0) {
            RADBusinessCalendar.instance.OnCreateBussinessCalender();

        }
        else if ($(event.target).closest("#btnRADBCalenderCancelButtonId").length > 0) {
            RADBusinessCalendar.instance.OnCancelCreateCalender();
        }
        else if ($(event.target).closest("#btnRADBCalenderSaveButtonId").length > 0) {
            RADBusinessCalendar.instance.SaveBusinessCalender();
        }
        else if ($(event.target).closest("#btnRADBCalenderEditButtonId").length > 0) {
            RADBusinessCalendar.instance.OnEditBusinessCalender();
        }
        else if ($(event.target).closest("#btnRADBCalenderDeleteButtonId").length > 0) {
            RADBusinessCalendar.instance.OnDeleteBusinessCalender();

        }
        else if ($(event.target).closest("#btnRADHolidayIncAddNewButtonId").length > 0) {
            RADBusinessCalendar.instance.isCreateHoliday = true;
            RADBusinessCalendar.instance.OnEditCreateBusinessHoliday();

        }
        else if ($(event.target).closest("#btnRADHolidayIncEditButtonId").length > 0) {
            RADBusinessCalendar.instance.isCreateHoliday = false;
            RADBusinessCalendar.instance.OnEditCreateBusinessHoliday();
        }
        else if ($(event.target).closest("#btnRADHolidayIncDeleteButtonId").length > 0) {
            RADBusinessCalendar.instance.OnDeleteBusinessHoliday();
        }
        else if ($(event.target).closest("#btnRADHolidayIncExcludeButtonId").length > 0) {
            RADBusinessCalendar.instance.OnExcludeBusinessHoliday();
        }
        else if ($(event.target).closest(".RADBCalenderPropertyChildValueCheckBox").length > 0) {
            if ($(event.target).closest(".RADBCalenderPropertyChildValueCheckBox")[0].type == "radio")
                RADBusinessCalendar.instance.ShowHideControlsByCalenderType(event);
        }
        else if ($(event.target).closest(".RCalenderRefEntityTypeSelectionText").length > 0) {
            RADBusinessCalendar.instance.BindAllEntityTypes(event);
        }
        else if ($(event.target).closest(".RCalenderRefUniqueSelectionText").length > 0) {
            RADBusinessCalendar.instance.BindAllUniqueAttributes(event);
        }
        else if ($(event.target).closest(".RCalenderRefHEntityTypeSelectionText").length > 0) {
            if (RADBusinessCalendar.instance.LegsByEntityId != null)
                RADBusinessCalendar.instance.BindLegsDetails(event);
        }
        else if ($(event.target).closest(".RCalenderRefHNameSelectionText").length > 0) {
            if (RADBusinessCalendar.instance.LegsByEntityId != null)
                RADBusinessCalendar.instance.BindLegsStringAttributes(event);
        }
        else if ($(event.target).closest(".RCalenderRefHDateSelectionText").length > 0) {
            if (RADBusinessCalendar.instance.LegsByEntityId != null)
                RADBusinessCalendar.instance.BindLegsDateAttributes(event);
        }
        else if ($(event.target).closest(".RCalenderRefUniqueValueSelectionText").length > 0) {
            RADBusinessCalendar.instance.BindUniqueAttributeData(event);
        }
        else if ($(event.target).closest(".RCalenderEntitiesDropDownChild").length > 0) {
            RADBusinessCalendar.instance.OnSelectEntityType(event);
        }
        else if ($(event.target).closest(".RCalenderUniqueAttrDropDownChild").length > 0) {
            RADBusinessCalendar.instance.OnSelectAttributeName(event);
        }
        else if ($(event.target).closest(".RCalenderUniqueAttrDataDropDownChild").length > 0) {
            RADBusinessCalendar.instance.OnSelectAttributeValue(event);
        }
        else if ($(event.target).closest(".RCalenderRefHEntityDataDropDownChild").length > 0) {
            RADBusinessCalendar.instance.OnSelectRefHolidayEntity(event);
        }
        else if ($(event.target).closest(".RCalenderRefHNameDataDropDownChild").length > 0) {
            RADBusinessCalendar.instance.OnSelectRefHolidayName(event);
        }
        else if ($(event.target).closest(".RCalenderRefHDateDataDropDownChild").length > 0) {
            RADBusinessCalendar.instance.OnSelectRefHolidayDate(event);
        }
        else if ($(event.target).closest("#btnRADHolidayIncDeleteExclButtonId").length > 0) {
            RADBusinessCalendar.instance.OnDeleteExcludedHoliday();
        }        
            //search
        else if ($(event.target).closest(".RADBCalenderSearchViewInput").length > 0) {
            if ($('#txtRADBCalenderLeftSectionSearch:visible').length) {
                $("#txtRADBCalenderLeftSectionSearch").hide("slow", function () {
                    // Animation complete.
                });
            }
            else {
                $("#txtRADBCalenderLeftSectionSearch").show("slow", function () {
                });
            }
        }
        else
        {
            RADBusinessCalendar.instance.ClearDropDowns();
        }

        //search keyup
        $("#txtRADBCalenderLeftSectionSearch").unbind("keyup").keyup(function (event) {
            RADBusinessCalendar.instance.SearchCalender($(event.target).val());
        })
    });
}

RADBusinessCalendar.prototype.SearchCalender = function (calenderName) {

    var self = this;
    var calenderName = calenderName.trim().toLowerCase();
    var length = $(".RADBCalenderLeftSectionContent").children().length;

    for (var i = 0; i < length; i++) {
        if (($($(".RADBCalenderLeftSectionContent").children()[i])).find(".RADBCalenderName").html().trim().toLowerCase().indexOf(calenderName) != -1) {
            $($(".RADBCalenderLeftSectionContent").children()[i]).removeClass("RADBCalenderDisplayNone");
        }
        else {
            $($(".RADBCalenderLeftSectionContent").children()[i]).addClass("RADBCalenderDisplayNone");
        }
    }
}


RADBusinessCalendar.prototype.OnExcludeBusinessHoliday = function () {

    $.ajax({
        url: RADBusinessCalendar.instance.ServiceAddress + '/GetTagTemplate',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ fileName: 'RADHolidaysBCalendarExclude.html'})

    }).then(function (responseText) {

        $("#RADHolidayExcludeMainDiv").remove();
        $("#RADHolidayInclMainDiv").append(responseText.d);

        var checkedRow = $($(".xlneocheckedRow")[1]);
        var holidayDate = $(checkedRow).find('div[columnname=start_date]').html().trim();
        $('#RADPropHolidayExclDateValueDiv').html(holidayDate);
        $("#RADHolidayExcludeHeaderDiv").text("Adding Exclusion Holiday");
        RADBusinessCalendar.instance.BindEventsCommonForExclude();
    });

}

RADBusinessCalendar.prototype.BindEventsCommonForExclude = function () {

    $("#btnRADHolidayExclCancelUpdateButtonId").unbind().click(function () {
        RADBusinessCalendar.instance.OnCancelHolidayExclude();
    });

    $("#btnRADHolidayExclUpdateButtonId").unbind().click(function () {
        RADBusinessCalendar.instance.AddExcludeHolidayForCalender();
    });
}

RADBusinessCalendar.prototype.AddExcludeHolidayForCalender = function () {

    var isValid = RADBusinessCalendar.instance.ValidateDataOnExcludeHoliday();
    var holidayDetails = RADBusinessCalendar.instance.GetHolidayDetailsForExclude();
    var holidayDetailsString = JSON.stringify(holidayDetails);

    $.ajax({
        url: RADBusinessCalendar.instance.ServiceAddress + "/IsHolidayExlusionValid",
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({
            holidayDetails: holidayDetailsString
        })
    }).then(function (responseText) {
        if (responseText.d) {
            $.ajax({
                url: RADBusinessCalendar.instance.ServiceAddress + "/ExcludeHoliday",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ holidayDetails: holidayDetailsString })
            }).then(function (responseText) {
                if (responseText.d) {
                    $("#RADHolidayExcludeMainDiv").remove();
                    RADBusinessCalendar.instance.GetCurrentCalenderHolidayDetails(holidayDetails.CalendarId);
                }
                else {

                }
            });
        }
        else {
            $("#RADHolidayExcludeMessageDiv").removeClass("RADBCalenderDisplayNone");
            $("#RADHolidayExcludeMessageDiv").html("Holiday Exlcusition exists either with same name");
        }
    });
}

RADBusinessCalendar.prototype.GetHolidayDetailsForExclude = function () {

    var holidayDetails = {};
    holidayDetails.CalendarId = RADBusinessCalendar.instance.CurrentSelectedCalenderId;
    holidayDetails.ExcludedHolidayName = $('#RADPropHolidayExclNameValueDiv').text().trim();
    holidayDetails.ExcludedHolidayDescription = $('#RADPropHolidayExclDescValueDiv').text().trim();
    holidayDetails.HolidayDate = $('#RADPropHolidayExclDateValueDiv').text();
    return holidayDetails;
}

RADBusinessCalendar.prototype.ValidateDataOnExcludeHoliday = function () {

    var isDataValid = true;
    if ($('#RADPropHolidayExclNameValueDiv').html() == null || $('#RADPropHolidayExclNameValueDiv').html() == "") {
        $("#RADPropHolidayExclNameValidationDiv").switchClass("RADBCalenderDisplayNone", "RADBCalenderRequiredField")
        isDataValid = false;
    }
    if ($('#RADPropHolidayExclDescValueDiv').html() == null || $('#RADPropHolidayExclDescValueDiv').html() == "") {
        $("#RADPropHolidayExclDescValidationDiv").switchClass("RADBCalenderDisplayNone", "RADBCalenderRequiredField")
        isDataValid = false;
    }
    return isDataValid;
}

RADBusinessCalendar.prototype.OnCancelHolidayExclude = function () {
    $("#RADHolidayExcludeMainDiv").remove();
}

RADBusinessCalendar.prototype.OnEditCreateBusinessHoliday = function () {

    $.ajax({
        url: RADBusinessCalendar.instance.ServiceAddress + '/GetTagTemplate',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ fileName: 'RADHolidaysBCalendarUpdate.html'})
    }).then(function (responseText) {
        $("#RADHolidayUpdateMainDiv").remove();
        $("#RADHolidayInclMainDiv").append(responseText.d);
        RADBusinessCalendar.instance.BindEventsCommonForUpdate();

        if (!RADBusinessCalendar.instance.isCreateHoliday) {
            $("#btnRADHolidayIncUpdateButtonId").attr('value', 'Update');
            RADBusinessCalendar.instance.BindHolidayDetails();
        }
        else {            
            $("#btnRADHolidayIncUpdateButtonId").attr('value', 'Save');
        }
    });
}

RADBusinessCalendar.prototype.GetHolidayDetailsForCreateUpdate = function () {

    var holidayDetailsUI = {};
    holidayDetailsUI.CalendarId = RADBusinessCalendar.instance.CurrentSelectedCalenderId;
    if (!RADBusinessCalendar.instance.isCreateHoliday)
        holidayDetailsUI.HolidayId = RADBusinessCalendar.instance.CurrentSelectedHolidayId;
    holidayDetailsUI.HolidayName = $('#RADPropHolidayNameValueDiv').text().trim();
    RADBusinessCalendar.instance.CurrentSelectedHolidayName = holidayDetailsUI.HolidayName;
    holidayDetailsUI.HolidayDescription = $('#RADPropHolidayDescValueDiv').text().trim();
    holidayDetailsUI.HolidayDate = $('#RADPropHolidayDateChildDivValueInput').val();
    holidayDetailsUI.RecurrencePattern = ($("#RADPropRecurrentValueCheck")[0].checked == true) ? true : false;
    return holidayDetailsUI;
}

RADBusinessCalendar.prototype.CheckHolidayInfoAndCreate = function (holidayDetailsUI) {

    var isValidToCreate = false;
    var holidayDetailsString = JSON.stringify(holidayDetailsUI);

    if (!$("#RADHolidayUpdateMessageDiv").hasClass("RADBCalenderDisplayNone"))
        $("#RADHolidayUpdateMessageDiv").addClass("RADBCalenderDisplayNone");
    $("#RADHolidayUpdateMessageDiv").html("");

    $.ajax({
        url: RADBusinessCalendar.instance.ServiceAddress + "/IsHolidayValid",
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({
            holidayDetails: holidayDetailsString
        })
    }).then(function (responseText) {
        if(responseText.d){
            $.ajax({
                url: RADBusinessCalendar.instance.ServiceAddress + "/CreateHoliday",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ holidayDetails: holidayDetailsString })
            }).then(function (responseText) {
                if (responseText.d) {
                    $("#RADHolidayUpdateMainDiv").remove();
                    RADBusinessCalendar.instance.CommonSucessFailAlert($("#RADHolidayInclMainDiv"), true, true, "Holiday", holidayDetailsUI.CalendarId);
                }
                else {
                    $("#RADHolidayUpdateMainDiv").remove();
                    RADBusinessCalendar.instance.CommonSucessFailAlert($("#RADHolidayInclMainDiv"), false, true, "Holiday", holidayDetailsUI.CalendarId);

                }
            });
        }
        else{
            $("#RADHolidayUpdateMessageDiv").removeClass("RADBCalenderDisplayNone");
            $("#RADHolidayUpdateMessageDiv").html("Holiday exists either with same name or date");
        }
    });
}

RADBusinessCalendar.prototype.CheckHolidayInfoAndUpdate = function (holidayDetailsUI) {

    var isValidToUpdate = false;
    if (!$("#RADHolidayUpdateMessageDiv").hasClass("RADBCalenderDisplayNone"))
        $("#RADHolidayUpdateMessageDiv").addClass("RADBCalenderDisplayNone");
    $("#RADHolidayUpdateMessageDiv").html("");

    $.ajax({
        url: RADBusinessCalendar.instance.ServiceAddress + "/GetHolidayDetailsByName",
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({
            calenderId: RADBusinessCalendar.instance.CurrentSelectedCalenderId,
            holidayName: RADBusinessCalendar.instance.CurrentSelectedHolidayName
        })
    }).then(function (responseText) {

        if (responseText.d == "") {
            isValidToUpdate = true;
        }
        else {
            var holidayDetailsDB = JSON.parse(responseText.d);
            if (holidayDetailsDB.HolidayId == holidayDetailsUI.HolidayId
                && holidayDetailsDB.HolidayName.trim().toLowerCase() == holidayDetailsUI.HolidayName.trim().toLowerCase()) {
                isValidToUpdate = true;
            }
        }
        if (isValidToUpdate) {
            var holidayDetailsString = JSON.stringify(holidayDetailsUI);
            $.ajax({
                url: RADBusinessCalendar.instance.ServiceAddress + "/UpdateHoliday",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ holidayDetails: holidayDetailsString })
            }).then(function (responseText) {
                if (responseText.d) {
                    $("#RADHolidayUpdateMainDiv").remove();
                    RADBusinessCalendar.instance.CommonSucessFailAlert($("#RADHolidayInclMainDiv"), true, false, "Holiday", holidayDetailsUI.CalendarId);
                }
                else {
                    $("#RADHolidayUpdateMainDiv").remove();
                    RADBusinessCalendar.instance.CommonSucessFailAlert($("#RADHolidayInclMainDiv"), false, false, "Holiday", holidayDetailsUI.CalendarId);
                }
            });
        }
        else {
            $("#RADHolidayUpdateMessageDiv").removeClass("RADBCalenderDisplayNone");
            $("#RADHolidayUpdateMessageDiv").html("Holiday Name <b>" + holidayDetailsUI.HolidayName + "</b> already exists for current calender");
        }
    });
}

RADBusinessCalendar.prototype.CreateUpdateHolidayForCalender = function () {

    var isDataValid = false;
    var holidayDetailsUI = {}
    var holidayDetailsDB = {}
    isDataValid = RADBusinessCalendar.instance.ValidateDataOnCreateUpdateHoliday();

    if (isDataValid) {
        holidayDetailsUI = RADBusinessCalendar.instance.GetHolidayDetailsForCreateUpdate();
        if (RADBusinessCalendar.instance.isCreateHoliday) {
            RADBusinessCalendar.prototype.CheckHolidayInfoAndCreate(holidayDetailsUI);
        }
        else {
            RADBusinessCalendar.prototype.CheckHolidayInfoAndUpdate(holidayDetailsUI);
        }
    }   
}

RADBusinessCalendar.prototype.ValidateDataOnCreateUpdateHoliday = function () {

    var isDataValid = true;

    if ($('#RADPropHolidayNameValueDiv').html() == null || $('#RADPropHolidayNameValueDiv').html() == "") {
        $("#RADPropHolidayNameValidationDiv").switchClass("RADBCalenderDisplayNone", "RADBCalenderRequiredField")
        isDataValid = false;
    }
    if ($('#RADPropHolidayDescValueDiv').html() == null || $('#RADPropHolidayDescValueDiv').html() == "") {
        $("#RADPropHolidayDescValidationDiv").switchClass("RADBCalenderDisplayNone", "RADBCalenderRequiredField")
        isDataValid = false;
    }
    if ($('#RADPropHolidayDateChildDivValueInput').val() == null || $('#RADPropHolidayDateChildDivValueInput').val() == "") {
        $("#RADPropHolidayDateValidationDiv").switchClass("RADBCalenderDisplayNone", "RADBCalenderRequiredField")
        isDataValid = false;
    }
    return isDataValid;
}

RADBusinessCalendar.prototype.BindHolidayDetails = function () {

    var checkedRow = $($(".xlneocheckedRow")[1])[0];
    var holidayId = $(checkedRow).attr('idcolumnvalue');
    RADBusinessCalendar.instance.CurrentSelectedHolidayId = holidayId;
    var holidayName = $($(".xlneocheckedRow")[1]).find('div[columnname=holiday_name]').html().trim();
    var holidayDescription = $($(".xlneocheckedRow")[1]).find('div[columnname=holiday_description]').html().trim();
    var holidayDate = $($(".xlneocheckedRow")[1]).find('div[columnname=start_date]').html().trim();
    var isReccurent = $($(".xlneocheckedRow")[1]).find('div[columnname=recurrence_pattern]').html().trim();

    $("#RADPropHolidayNameValueDiv").text(holidayName);
    $("#RADPropHolidayDescValueDiv").text(holidayDescription);
    $("#RADPropHolidayDateChildDivValueInput").val(holidayDate);
    $("#RADPropRecurrentValueCheck")[0].checked = (isReccurent == "true") ? true : false;
    $("#RADHolidayUpdateHeaderDiv").text("Updating Holiday Details for : " + holidayName);
}

RADBusinessCalendar.prototype.BindEventsCommonForUpdate = function () {

    $("#btnRADHolidayIncCancelUpdateButtonId").unbind().click(function (event) {
        RADBusinessCalendar.instance.OnCancelHolidayUpdate();
    });

    $("#btnRADHolidayIncUpdateButtonId").unbind().click(function (event) {
        RADBusinessCalendar.instance.CreateUpdateHolidayForCalender();
    });

    $("#RADPropHolidayDateChildDivValueInput").datepicker({
        changeMonth: true,
        changeYear: true,
        format: RADBusinessCalendar.instance.AppDefaultDateTime
    });
}

RADBusinessCalendar.prototype.OnCancelHolidayUpdate = function () {
    $("#RADHolidayUpdateMainDiv").remove();
}


RADBusinessCalendar.prototype.OnDeleteBusinessHoliday = function () {

    var currentCalenderId = RADBusinessCalendar.instance.CurrentSelectedCalenderId;
    var checkedRow = $($(".xlneocheckedRow")[1])[0];
    var holidayIdTodelete = $(checkedRow).attr('idcolumnvalue');
    var holidayName = $($(".xlneocheckedRow")[1]).find('div[columnname=holiday_name]').html().trim();
    RADBusinessCalendar.prototype.CommonConfirmationAlert($("#RADHolidayInclMainDiv"), holidayName, "holiday");

    $(".RADBCalenderPopUpErrorYes").unbind().click(function (event) {
        $("#RADBCalenderAlertParentDiv").remove();
        RADBusinessCalendar.instance.DeleteBusinessHoliday(currentCalenderId, holidayIdTodelete);
    });

    $(".RADBCalenderPopUpErrorNo").unbind().click(function (event) {
        $("#RADBCalenderAlertParentDiv").remove();
    });
}

RADBusinessCalendar.prototype.OnDeleteExcludedHoliday = function () {
    var currentCalenderId = RADBusinessCalendar.instance.CurrentSelectedCalenderId;
    var excludedHolidayName = $($(".xlneocheckedRow")[1]).find('div[columnname=holiday_exclusion_name]').html().trim();
    RADBusinessCalendar.prototype.CommonConfirmationAlert($("#RADHolidayInclMainDiv"), excludedHolidayName, "excluded holiday");

    $(".RADBCalenderPopUpErrorYes").unbind().click(function (event) {
        $("#RADBCalenderAlertParentDiv").remove();
        RADBusinessCalendar.instance.DeleteExcludedHoliday(currentCalenderId, excludedHolidayName);
    });
    $(".RADBCalenderPopUpErrorNo").unbind().click(function (event) {
        $("#RADBCalenderAlertParentDiv").remove();
    });


    
}

RADBusinessCalendar.prototype.DeleteExcludedHoliday = function (currentCalenderId, excludedHolidayName) {
    $.ajax({
        url: RADBusinessCalendar.instance.ServiceAddress + '/DeleteExcludedHoliday',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({
            calenderId: currentCalenderId,
            excludedHolidayName: excludedHolidayName
        })
    }).then(function (responseText) {
        var isDeleted = responseText.d;
        if (isDeleted) {            
            RADBusinessCalendar.instance.GetCurrentCalenderHolidayDetails(currentCalenderId);
        }
    });
}

RADBusinessCalendar.prototype.DeleteBusinessHoliday = function (currentCalenderId, holidayIdTodelete) {
    $.ajax({
        url: RADBusinessCalendar.instance.ServiceAddress + '/DeleteHoliday',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({
            calenderId: currentCalenderId,
            holidayId: holidayIdTodelete
        })
    }).then(function (responseText) {
        var isDeleted = responseText.d;
        if (isDeleted) {
            RADBusinessCalendar.instance.GetCurrentCalenderHolidayDetails(currentCalenderId);
        }
    });
}

RADBusinessCalendar.prototype.OnDeleteBusinessCalender = function () {   
    
    RADBusinessCalendar.prototype.CommonConfirmationAlert($("#RADBCalenderMainDiv"),
                                                          RADBusinessCalendar.instance.CurrentCalenderDetails.CalendarName, "calender");

    $(".RADBCalenderPopUpErrorYes").unbind().click(function (event) {
        $("#RADBCalenderAlertParentDiv").remove();
        RADBusinessCalendar.instance.DeleteBusinessCalender(RADBusinessCalendar.instance.CurrentSelectedCalenderId);
    });

    $(".RADBCalenderPopUpErrorNo").unbind().click(function (event) {
        $("#RADBCalenderAlertParentDiv").remove();
    });
}

RADBusinessCalendar.prototype.CommonSucessFailAlert = function (mainDivforAlert, isSuccess, isCreation, module, moduleArgOptional) {

    mainDivforAlert.append("<div id=\"RADBCalenderAlertParentDiv\" class=\"RADBCalenderAlertPopUpParent\"></div>");
    $("#RADBCalenderAlertParentDiv").append("<div class=\"RADBCalenderPopupAlertUpperSuccessDiv\"></div>");
    $("#RADBCalenderAlertParentDiv").append("<div class=\"RADBCalenderPopUpError\"></div>");
    $("#RADBCalenderAlertParentDiv").append("<div class=\"RADBCalenderDivLeft RADBCalenderSuccess\"></div>");
    $("#RADBCalenderAlertParentDiv").append("<div class=\"RADBCalenderErrorDivRight\"></div>");


    if (isSuccess) {
        $(".RADBCalenderErrorDivRight").append("<div class=\"RADBCalenderSuccessDivHeading\">Success</div>")
        if (isCreation)
            $(".RADBCalenderErrorDivRight").append("<div class=\"RADBCalenderErrorDivText\">" + module + " has been created successfully.</div>");
        else
            $(".RADBCalenderErrorDivRight").append("<div class=\"RADBCalenderErrorDivText\">" + module + " has been updated successfully.</div>");
    }
    else {
        $(".RADBCalenderErrorDivRight").append("<div class=\"RADBCalenderSuccessDivHeading\">Alert</div>")
        if (isCreation)
            $(".RADBCalenderErrorDivRight").append("<div class=\"RADBCalenderErrorDivText\">" + module + " creation failed.</div>");
        else
            $(".RADBCalenderErrorDivRight").append("<div class=\"RADBCalenderErrorDivText\">" + module + " updation failed.</div>");

    }

    if (isSuccess)
        RADBusinessCalendar.prototype.CommonSucessAlertTimeOut(isCreation, module, moduleArgOptional)
    else
        RADBusinessCalendar.prototype.CommonFailAlertTimeOut(isCreation, module, moduleArgOptional)
}

RADBusinessCalendar.prototype.CommonSucessAlertTimeOut = function (isCreation, module, moduleArgOptional) {
    $("#RADBCalenderAlertParentDiv").css("top", "-200px");
    $("#RADBCalenderAlertParentDiv").animate({ "top": "0px" });
    setTimeout(function () {
        $("#RADBCalenderAlertParentDiv").remove();

        if (module == "Calender") {
            RADBusinessCalendar.instance.OnPageLoadCalender();
        }
        else if (module == "Holiday") {            
                RADBusinessCalendar.instance.GetCurrentCalenderHolidayDetails(moduleArgOptional);
        }
    }, 2000);
}

RADBusinessCalendar.prototype.CommonFailAlertTimeOut = function (isCreation, module, moduleArgOptional) {
    $("#RADBCalenderAlertParentDiv").css("top", "-200px");
    $("#RADBCalenderAlertParentDiv").animate({ "top": "0px" });
    setTimeout(function () {
        $("#RADBCalenderAlertParentDiv").remove();

        if (module == "Calender") {
            RADBusinessCalendar.instance.OnPageLoadCalender();
        }
        else if (module == "Holiday") {
            RADBusinessCalendar.instance.GetCurrentCalenderHolidayDetails(moduleArgOptional);
        }
    }, 2000);
}

RADBusinessCalendar.prototype.CommonConfirmationAlert = function (mainDivforAlert,  toDeleteItem, module) {

    $("#RADBcalenderAlertParentDiv").remove();
    mainDivforAlert.append("<div id=\"RADBCalenderAlertParentDiv\" class=\"RADBCalenderAlertPopUpParent\"></div>");
    $("#RADBCalenderAlertParentDiv").append("<div class=\"RADBCalenderAlertUpperDiv\"></div>");
    $("#RADBCalenderAlertParentDiv").append("<div class=\"RADBCalenderPopUpError\"></div>");
    $("#RADBCalenderAlertParentDiv").append("<div class=\"fa fa-exclamation-circle RADBCalenderDivLeft RADBCalenderAlert\"></div>");

    $("#RADBCalenderAlertParentDiv").append("<div class=\"RADBCalenderErrorDivRight\"></div>")
    $(".RADBCalenderErrorDivRight").append("<div class=\"RADBCalenderErrorDivHeading\">Alert</div>")
    $(".RADBCalenderErrorDivRight").append("<div class=\"RADBCalenderErrorDivText\">Are You Sure You want to delete " + module+" "
        + toDeleteItem.toUpperCase() + "?</div>");
    $("#RADBCalenderAlertParentDiv").append("<div class=\"RADBCalenderPopUpErrorFooter\"></div>");
    $(".RADBCalenderPopUpErrorFooter").append("<div class=\"RADBCalenderPopUpErrorYes\">Yes</div><div class=\"RADBCalenderPopUpErrorNo\">No</div>")
    $("#RADBCalenderAlertParentDiv").css("top", "-200px");
    $("#RADBCalenderAlertParentDiv").animate({ "top": "0px" });

}


RADBusinessCalendar.prototype.DeleteBusinessCalender = function (calenderId) {
    $.ajax({
        url: RADBusinessCalendar.instance.ServiceAddress + '/DeleteCalendar',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({
            calendarId: calenderId
        })
    }).then(function (responseText) {
        var isDeleted = responseText.d;
        $("#RADBCalenderMainDiv").append("<div id=\"RADBCalenderAlertParentDiv\" class=\"RADBCalenderAlertPopUpParent\"></div>");
        $("#RADBCalenderAlertParentDiv").append("<div class=\"RADBCalenderPopupAlertUpperSuccessDiv\"></div>");
        $("#RADBCalenderAlertParentDiv").append("<div class=\"RADBCalenderPopUpError\"></div>")
        $("#RADBCalenderAlertParentDiv").append("<div class=\"RADBCalenderDivLeft RADBCalenderSuccess\"></div>"); //revisit
        $("#RADBCalenderAlertParentDiv").append("<div class=\"RADBCalenderErrorDivRight\"></div>");

        if (isDeleted) {
            $(".RADBCalenderErrorDivRight").append("<div class=\"RADBCalenderSuccessDivHeading\">Success</div>");
            $(".RADBCalenderErrorDivRight").append("<div class=\"RADBCalenderErrorDivText\">Calender has been deleted successfully.</div>");
        }
        else {
            $(".RADBCalenderErrorDivRight").append("<div class=\"RADBCalenderSuccessDivHeading\">Alert</div>");
            $(".RADBCalenderErrorDivRight").append("<div class=\"RADBCalenderErrorDivText\">Calender deletion failed.</div>");
        }
        $("#RADBCalenderAlertParentDiv").css("top", "-200px");
        $("#RADBCalenderAlertParentDiv").animate({ "top": "0px" });
        setTimeout(function () {
            $("#RADBCalenderAlertParentDiv").remove()
            RADBusinessCalendar.instance.OnPageLoadCalender();
        }, 2000);
    });
}

RADBusinessCalendar.prototype.OnEditBusinessCalender = function () {
    RADBusinessCalendar.instance.isEditMode = true;
    RADBusinessCalendar.prototype.ShowHideControlsOnEditandCreate();
    $("#RADBCalenderNameParentDiv").html('Updating Details for Calendar');
    RADBusinessCalendar.instance.BindCalenderDetailsOnEdit(RADBusinessCalendar.instance.CurrentCalenderDetails);
    $("#RADPropCreateCalenderTypeCustom")[0].checked = true;
    $("#RADPropCreateCalenderTypeRef")[0].checked = false;

    if (RADBusinessCalendar.instance.CurrentCalenderDetails.CalendarTypeId == 2)
        RADBusinessCalendar.instance.OnEditRefCalender();
}

RADBusinessCalendar.prototype.OnEditRefCalender = function () {
    $(".RADBCalenderCreateCustomParent").hide();
    $(".RADBCalenderCreateRefMasterParent").show();
    $("#RADPropCreateCalenderTypeCustom")[0].checked = false;
    $("#RADPropCreateCalenderTypeRef")[0].checked = true;
    $("#RADPropCreateCalenderTypeCustom")
    RADBusinessCalendar.instance.BindRefCalenderDetailsOnEdit(RADBusinessCalendar.instance.CurrentCalenderDetails);
    RADBusinessCalendar.instance.ClearDropDowns();
}

RADBusinessCalendar.prototype.BindCalenderSetupDetails = function (currentCalenderDetails) {

    RADBusinessCalendar.instance.CurrentEntityId = currentCalenderDetails.CalenderSetupInfo.EntityTypeCalender
    RADBusinessCalendar.instance.CurrentAttributeRealName = currentCalenderDetails.CalenderSetupInfo.UniqueColumn;
    RADBusinessCalendar.instance.CurrentAttributeValueId = currentCalenderDetails.CalenderSetupInfo.UniqueColumnValue;
    RADBusinessCalendar.instance.CurrentEntityIdHoliday = currentCalenderDetails.CalenderSetupInfo.EntityTypeHoliday;
    RADBusinessCalendar.instance.CurrentStartDateColumn = currentCalenderDetails.CalenderSetupInfo.StartDateColumn;
    RADBusinessCalendar.instance.CurrentHolidayNameColumn = currentCalenderDetails.CalenderSetupInfo.HolidayNameColumn;
    
}

RADBusinessCalendar.prototype.BindRefCalenderDetailsOnEdit = function (currentCalenderDetails) {

    var loaderDiv = $(document.body).find('#RADBCalenderLoaderDiv');
    $(loaderDiv).removeClass("RADBCalenderHiddenClass");

    RADBusinessCalendar.instance.BindCalenderSetupDetails(currentCalenderDetails);

    RADBusinessCalendar.instance.AllEntityTypes = [];
    $.ajax({
        url: RADBusinessCalendar.instance.ServiceAddress + '/GetAllEntityTypes',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        allEntityTypes = responseText.d;
        allEntityTypes.forEach(function (entityItem) {
            var currentEntityInfo = {};
            currentEntityInfo.EntityTypeId = entityItem.EntityTypeId;
            currentEntityInfo.EntityTypeDisplayName = entityItem.EntityTypeDisplayName;
            RADBusinessCalendar.instance.AllEntityTypes.push(currentEntityInfo);
        });

        var currentEntityType =  $.grep(RADBusinessCalendar.instance.AllEntityTypes, function (e)
        { return e.EntityTypeId == currentCalenderDetails.CalenderSetupInfo.EntityTypeCalender });
        $(".RCalenderRefEntityTypeSelectionText").html(currentEntityType[0].EntityTypeDisplayName);

        RADBusinessCalendar.instance.EntityUniqueColumns = [];
        $.ajax({
            url: RADBusinessCalendar.instance.ServiceAddress + '/GetEntityUniqueColumns',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ entityId: currentCalenderDetails.CalenderSetupInfo.EntityTypeCalender })
        }).then(function (responseText) {
            var allAttributeInfo = responseText.d;
            allAttributeInfo.forEach(function (currentAttributeInfo) {
                var currentEntityInfo = {};
                currentEntityInfo.AttributeRealName = currentAttributeInfo.AttributeRealName;
                currentEntityInfo.Name = currentAttributeInfo.Name;
                RADBusinessCalendar.instance.EntityUniqueColumns.push(currentEntityInfo);
            });

            var currentUniqueColumn = $.grep(RADBusinessCalendar.instance.EntityUniqueColumns, function (e)
            { return e.AttributeRealName == currentCalenderDetails.CalenderSetupInfo.UniqueColumn });
            $(".RCalenderRefUniqueSelectionText").html(currentUniqueColumn[0].Name);

            RADBusinessCalendar.instance.CurrentAttributeData = [];
            $.ajax({
                url: RADBusinessCalendar.instance.ServiceAddress + '/GetDataByAttributeName',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({
                    entityId: currentCalenderDetails.CalenderSetupInfo.EntityTypeCalender,
                    entityName: currentEntityType[0].EntityTypeDisplayName, attributeName: currentUniqueColumn[0].Name
                })
            }).then(function (responseText) {
                RADBusinessCalendar.instance.CurrentAttributeData = responseText.d;
                var currentUniqueColumnValue = $.grep(RADBusinessCalendar.instance.CurrentAttributeData, function (e)
                { return e.Key == currentCalenderDetails.CalenderSetupInfo.UniqueColumnValue });
                $(".RCalenderRefUniqueValueSelectionText").html(currentUniqueColumnValue[0].Value);

                RADBusinessCalendar.instance.LegsByEntityId = {};
                $.ajax({
                    url: RADBusinessCalendar.instance.ServiceAddress + '/GetEntityTypesByEntityId',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ entityId: currentCalenderDetails.CalenderSetupInfo.EntityTypeCalender })
                }).then(function (responseText) {
                    RADBusinessCalendar.instance.LegsByEntityId = responseText.d;
                    var currentEntityTypeHoliday = $.grep(RADBusinessCalendar.instance.LegsByEntityId, function (e)
                    { return e.EntityTypeId == currentCalenderDetails.CalenderSetupInfo.EntityTypeHoliday });
                    $(".RCalenderRefHEntityTypeSelectionText").html(currentEntityTypeHoliday[0].Name);

                    var currentHolidayNameColumn = $.grep(currentEntityTypeHoliday[0].Attributes, function (e)
                    { return e.AttributeRealName == currentCalenderDetails.CalenderSetupInfo.HolidayNameColumn });
                    $(".RCalenderRefHNameSelectionText").html(currentHolidayNameColumn[0].Name);

                    

                    var currentHolidayDateColumn = $.grep(currentEntityTypeHoliday[0].Attributes, function (e)
                    { return e.AttributeRealName == currentCalenderDetails.CalenderSetupInfo.StartDateColumn });

                    $(".RCalenderRefHDateSelectionText").html(currentHolidayDateColumn[0].Name);
                    $(loaderDiv).addClass("RADBCalenderHiddenClass");

                });
            });
        });
    });
}

RADBusinessCalendar.prototype.BindCalenderDetailsOnEdit = function (currentCalenderDetails) {
    $("#RADPropCreateCalenderNameValueDiv").html(currentCalenderDetails.CalendarName);
    $("#RADPropCreateCalenderNameValueDiv").css({ "pointer-events": "none" });
    $("#RADPropCreateCalenderDescValueDiv").html(currentCalenderDetails.CalendarDescription);

    if (currentCalenderDetails.CalendarTypeId == 1) {
        $("#RADPropCreateCalenderTypeCustom")[0].checked = true;
    }
    else {
        $("#RADPropCreateCalenderTypeCustom")[0].checked = false;
    }
    RADBusinessCalendar.instance.SetDefaultWeekHolidays(currentCalenderDetails.DefaultWeekHolidays, true);
}


RADBusinessCalendar.prototype.OnCancelCreateCalender = function () {
    RADBusinessCalendar.instance.isEditMode = false;
    $("#RADBCalenderCreateParentDiv").hide();
    $("#RADBCalenderCreateRefMasterParentDiv").hide();
    $("#RADBCalenderDetailsParentDiv").show();

    if (RADBusinessCalendar.instance.CurrentCalenderDetails.CalendarTypeId == 1) {
        $("#RADPropCalenderTypeCustom")[0].checked = true;
        $("#RADPropCalenderTypeRefMaster")[0].checked = false;
    }
    else {
        $("#RADPropCalenderTypeRefMaster")[0].checked = true;
        $("#RADPropCalenderTypeCustom")[0].checked = false;

    }
    $("#RADBCalenderNameParentDiv").html('Calendar Details');

    $("#RADBCalenderSaveButtonParentDiv").addClass("RADBCalenderDisplayNone");
    $("#RADBCalenderCancelButtonParentDiv").addClass("RADBCalenderDisplayNone");
   

    $("#RADBCalenderDeleteButtonParentDiv").removeClass("RADBCalenderDisplayNone");
    $("#RADBCalenderEditButtonParentDiv").removeClass("RADBCalenderDisplayNone");

    RADBusinessCalendar.instance.ShowHideByPrivilegeTypeCalender();


    $(".RADBCalenderRequiredField").switchClass("RADBCalenderRequiredField", "RADBCalenderDisplayNone");
}

RADBusinessCalendar.prototype.OnCreateBussinessCalender = function () {
    RADBusinessCalendar.instance.ShowHideControlsOnEditandCreate();
    RADBusinessCalendar.instance.ClearControlsOnCreate();
}

RADBusinessCalendar.prototype.ShowHideControlsOnEditandCreate = function () {
    $("#RADPropCreateDefWeek0")[0].checked = true;
    $("#RADPropCreateDefWeek6")[0].checked = true;
    $(".RADBCalenderRequiredField").switchClass("RADBCalenderRequiredField", "RADBCalenderDisplayNone");
    $("#RADBCalenderDetailsParentDiv").hide();
    $("#RADBCalenderNameParentDiv").html('Adding Details for New Calendar');
    $("#RADBCalenderCreateParentDiv").show();
    $("#RADBCalenderCreateCustomParentDiv").show();    
    $("#RADBCalenderCreateRefMasterParentDiv").hide();
    $("#RADBCalenderSaveButtonParentDiv").removeClass("RADBCalenderDisplayNone");
    $("#RADBCalenderCancelButtonParentDiv").removeClass("RADBCalenderDisplayNone");
    $("#RADBCalenderDeleteButtonParentDiv").addClass("RADBCalenderDisplayNone");
    $("#RADBCalenderEditButtonParentDiv").addClass("RADBCalenderDisplayNone");
    $("#RADBCalenderUploadFileDocDiv").empty();
    $("#RADPropCreateCalenderTypeCustom")[0].checked = true;
        
    $("#RADBCalenderUploadFileDocDiv").append("<div style=\"text-indent: 8px;\"> </div>");
    $("#RADBCalenderUploadFileDocDiv").append("<div class=\"fa fa-upload RADBCalenderUploadButton\"></div>");
    $("#RADBCalenderUploadFileDocDiv").append("<div class=\"RADBCalenderDropDiv\" id=\"attachmentButton\"><div style=\"display:inline-block;\">DRAG AND DROP&nbsp&nbsp&nbsp&nbsp</div><div style=\"display:inline-block;\">OR&nbsp&nbsp&nbsp&nbsp</div><div style=\"display:inline-block;text-decoration: underline;\">Browse</div>");

    $("#RADBCalenderUploadFileDocDiv").append("<div class=\"RADBCalenderWorkflowParentDropDrag\" id=\"parent1\"></div>");
    if ($('#parent1').fileUpload != undefined) {
        $('#parent1').fileUpload({
            'parentControlId': 'parent1',
            'attachementControlId': 'attachmentButton',
            'multiple': false,
            'debuggerDiv': '',
            'deleteEvent': function () {
            }
        });
    }
}


RADBusinessCalendar.prototype.ValidateDataOnEditandCreate = function () {

    var isDataValid = true;
    if ($('#RADPropCreateCalenderNameValueDiv').html().trim() == null || $('#RADPropCreateCalenderNameValueDiv').html().trim() == "") {
        $("#RADPropCalenderNameValidationDiv").switchClass("RADBCalenderDisplayNone", "RADBCalenderRequiredField")
        isDataValid = false;
    }
    else
        $("#RADPropCalenderNameValidationDiv").switchClass("RADBCalenderRequiredField", "RADBCalenderDisplayNone")

    
    if ($("#RADPropCreateCalenderTypeCustom")[0].checked == true) {
        isDataValid = RADBusinessCalendar.instance.ValidateDataForCustom(isDataValid);
    }
    else {
        isDataValid = RADBusinessCalendar.instance.ValidateDataForRef(isDataValid);
    }
    
    if (isDataValid)       
        $(".RADBCalenderRequiredField").switchClass("RADBCalenderRequiredField", "RADBCalenderDisplayNone")
    return isDataValid;
}

RADBusinessCalendar.prototype.ValidateDataForCustom = function (isDataValid) {

    if ($('#RADPropFileTypeSelect').find(":selected").text() != "--Select--") {
        if ($('#RADPropHolidayNameIndexValueDiv').html().trim() == null || $('#RADPropHolidayNameIndexValueDiv').html().trim() == "") {
            $("#RADPropHolidayNameIndexValidationDiv").switchClass("RADBCalenderDisplayNone", "RADBCalenderRequiredField");
            isDataValid = false;
        }
        else
            $("#RADPropHolidayNameIndexValidationDiv").switchClass("RADBCalenderRequiredField", "RADBCalenderDisplayNone");

        if ($('#RADPropHolidayDateIndexValidationDiv').html().trim() == null || $('#RADPropHolidayDateIndexValidationDiv').html().trim() == "") {
            $("#RADPropHolidayDateIndexValidationDiv").switchClass("RADBCalenderDisplayNone", "RADBCalenderRequiredField")
            isDataValid = false;
        }
        else
            $("#RADPropHolidayDateIndexValidationDiv").switchClass("RADBCalenderRequiredField", "RADBCalenderDisplayNone");

        if ($('#RADPropDateFormatSelect').find(":selected").text() == "--Select--") {
            $("#RADPropDateFormatValidationDiv").switchClass("RADBCalenderDisplayNone", "RADBCalenderRequiredField")
            isDataValid = false;
        }
        else
            $("#RADPropDateFormatValidationDiv").switchClass("RADBCalenderRequiredField", "RADBCalenderDisplayNone");

        if (($('#RADPropFileTypeSelect').find(":selected").text() != "--Select--") && ($('.fuattachmentDiv').length == 0)) {
            $("#RADBCalenderUploadMessageDiv").html("Please upload file");
            isDataValid = false;
        }
        else
            $("#RADBCalenderUploadMessageDiv").switchClass("RADBCalenderRequiredField", "RADBCalenderDisplayNone");

        if (($('#RADPropFileTypeSelect').find(":selected").text() != "--Select--") && ($('.fuattachmentDiv').length > 0)) {

            var acceptedFileExtensions = [];
            if ($('#RADPropFileTypeSelect').find(":selected").text() == "Excel") {
                acceptedFileExtensions.push('xls');
                acceptedFileExtensions.push('xlsx');
            }
            else {
                acceptedFileExtensions.push('csv');
            }
            var CheckFileExtensionObject = RADBusinessCalendar.instance.CheckFileExtension(acceptedFileExtensions);
            if (!CheckFileExtensionObject.IsValidExtension) {
                $("#RADBCalenderUploadMessageDiv").html(CheckFileExtensionObject.ErrorMessage);
                isDataValid = false;
            }
        }
    }
    return isDataValid;

}

RADBusinessCalendar.prototype.ValidateDataForRef = function (isDataValid) {

    if ($('.RCalenderRefEntityTypeSelectionText').html().trim() == "--Select--") {
        $("#RRefEntityTypeValidationDiv").switchClass("RADBCalenderDisplayNone", "RADBCalenderRequiredField")
        isDataValid = false;
    }
    else
        $("#RRefEntityTypeValidationDiv").switchClass("RADBCalenderRequiredField", "RADBCalenderDisplayNone")

    if ($('.RCalenderRefUniqueSelectionText').html().trim() == "--Select--") {
        $("#RRefUniqueValidationDiv").switchClass("RADBCalenderDisplayNone", "RADBCalenderRequiredField")
        isDataValid = false;
    }
    else
        $("#RRefUniqueValidationDiv").switchClass("RADBCalenderRequiredField", "RADBCalenderDisplayNone")

    if ($('.RCalenderRefUniqueValueSelectionText').html().trim() == "--Select--") {
        $("#RRefUniqueValueValidationDiv").switchClass("RADBCalenderDisplayNone", "RADBCalenderRequiredField")
        isDataValid = false;
    }
    else
        $("#RRefUniqueValueValidationDiv").switchClass("RADBCalenderRequiredField", "RADBCalenderDisplayNone")

    if ($('.RCalenderRefHEntityTypeSelectionText').html().trim() == "--Select--") {
        $("#RRefHEntityTypeValidationDiv").switchClass("RADBCalenderDisplayNone", "RADBCalenderRequiredField")
        isDataValid = false;
    }
    else
        $("#RRefHEntityTypeValidationDiv").switchClass("RADBCalenderRequiredField", "RADBCalenderDisplayNone")
    if ($('.RCalenderRefHNameSelectionText').html().trim() == "--Select--") {
        $("#RRefHNameValidationDiv").switchClass("RADBCalenderDisplayNone", "RADBCalenderRequiredField")
        isDataValid = false;
    }
    else
        $("#RRefHNameValidationDiv").switchClass("RADBCalenderRequiredField", "RADBCalenderDisplayNone")

    if ($('.RCalenderRefHDateSelectionText').html().trim() == "--Select--") {
        $("#RRefHDateValidationDiv").switchClass("RADBCalenderDisplayNone", "RADBCalenderRequiredField")
        isDataValid = false;
    }
    else
        $("#RRefHDateValidationDiv").switchClass("RADBCalenderRequiredField", "RADBCalenderDisplayNone")
    return isDataValid;
}

RADBusinessCalendar.prototype.CheckFileExtension = function (acceptedFileExtensions) {

    var CheckFileExtensionObject = {};  
    var fileName = $('.fuattachmentDiv').children()[0].innerHTML;
    var lastIndex = ((fileName.length) - (fileName.lastIndexOf(".")));
    lastIndex = (0 - (lastIndex - 1));
    var fileExtension = fileName.slice(lastIndex);

    if (acceptedFileExtensions.indexOf(fileExtension.toLowerCase()) == -1) {
        CheckFileExtensionObject.IsValidExtension = false;
        var acceptedFileExtensionsText = "";
        for (var i = 0; i < acceptedFileExtensions.length; i++) {
            acceptedFileExtensionsText = acceptedFileExtensionsText + "| " + acceptedFileExtensions[i];
        }
        CheckFileExtensionObject.ErrorMessage = "Please upload files with extension " + acceptedFileExtensionsText;
    }
    else {
        CheckFileExtensionObject.IsValidExtension = true;
        CheckFileExtensionObject.ErrorMessage = "";
    }
    return CheckFileExtensionObject;
}

RADBusinessCalendar.prototype.GetCalenderDetailsForCreation = function () {

    var calendarCreateDetails = {};
    var calendarDetails = {};
    var calenderHolidayFileInfo = {};
    var rCalenderSetupInfo = {}

    calendarDetails.CalendarTypeId = ($("#RADPropCreateCalenderTypeCustom")[0].checked == true) ? 1 : 2;
    calendarDetails.CalendarName = $('#RADPropCreateCalenderNameValueDiv').html().trim();
    calendarDetails.CalendarDescription = $('#RADPropCreateCalenderDescValueDiv').html().trim();
    calendarDetails.DefaultWeekHolidays = RADBusinessCalendar.instance.GetDefaultWeekHolidays();    

    if (calendarDetails.CalendarTypeId == 1 && $('#RADPropFileTypeSelect').find(":selected").text() != "--Select--") {

        calenderHolidayFileInfo.FileType = $('#RADPropFileTypeSelect').find(":selected").val();
        calenderHolidayFileInfo.HolidayNameIndex = $('#RADPropHolidayNameIndexValueDiv').html().trim();
        calenderHolidayFileInfo.HolidayDateIndex = $('#RADPropHolidayDateIndexValueDiv').html().trim();
        calenderHolidayFileInfo.DateFormat = $('#RADPropDateFormatSelect').find(":selected").val();
        calenderHolidayFileInfo.FileName = $('.fuattachmentDiv').children()[0].innerHTML;
        calenderHolidayFileInfo.FilePath = "";
        calenderHolidayFileInfo.IsRecurrent = ($("#RADPropRecurrentValueCheck")[0].checked == true) ? true : false;
        calenderHolidayFileInfo.CacheKey = "parent1";
    }
    else if (calendarDetails.CalendarTypeId == 2) {
        rCalenderSetupInfo.EntityTypeCalender = RADBusinessCalendar.instance.CurrentEntityId;
        rCalenderSetupInfo.UniqueColumn = RADBusinessCalendar.instance.CurrentAttributeRealName;
        rCalenderSetupInfo.UniqueColumnValue = RADBusinessCalendar.instance.CurrentAttributeValueId;
        rCalenderSetupInfo.EntityTypeHoliday = RADBusinessCalendar.instance.CurrentEntityIdHoliday;
        rCalenderSetupInfo.StartDateColumn = RADBusinessCalendar.instance.CurrentStartDateColumn;
        rCalenderSetupInfo.HolidayNameColumn = RADBusinessCalendar.instance.CurrentHolidayNameColumn;
    }
    calendarDetails.CalenderSetupInfo = rCalenderSetupInfo;
    calendarCreateDetails.calendarDetails = calendarDetails;
    calendarCreateDetails.calenderHolidayFileInfo = calenderHolidayFileInfo;
    return calendarCreateDetails

}

RADBusinessCalendar.prototype.GetDefaultWeekHolidays = function () {
    var holidaysString = "";
    if ($('#RADPropCreateDefWeekHolidaysParentDiv').find('input[radbcalenderweekdayid]:checked').length == 0) {
        return "7";
    }
    else {
        var chekedElements = $('#RADPropCreateDefWeekHolidaysParentDiv').find('input[radbcalenderweekdayid]:checked');

        for(var i=0; i<chekedElements.length; i++)
        {
            var currentElement = chekedElements[i];
            holidaysString = holidaysString + $(currentElement).attr('radbcalenderweekdayid');
        }
        return holidaysString;
    }
}

RADBusinessCalendar.prototype.SaveBusinessCalender = function () {

    var calendarCreateDetails = {}
    var isDataValid = RADBusinessCalendar.instance.ValidateDataOnEditandCreate();

    if (isDataValid) {
        var loaderDiv = $(document.body).find('#RADBCalenderLoaderDiv');
        $(loaderDiv).removeClass("RADBCalenderHiddenClass");
        calendarCreateDetails = RADBusinessCalendar.instance.GetCalenderDetailsForCreation();

        if (RADBusinessCalendar.instance.isEditMode)
            calendarCreateDetails.calendarDetails.CalendarId = RADBusinessCalendar.instance.CurrentCalenderDetails.CalendarId;

        var calendarDetailsString = JSON.stringify(calendarCreateDetails.calendarDetails);
        var calenderHolidayFileInfoString = JSON.stringify(calendarCreateDetails.calenderHolidayFileInfo);

        $.ajax({
            url: RADBusinessCalendar.instance.ServiceAddress + '/AddUpdateCalendar',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({
                calendarDetails: calendarDetailsString,
                calenderHolidayFileInfo: calenderHolidayFileInfoString
            })
        }).then(function (responseText) {
            var isCreation = (RADBusinessCalendar.instance.isEditMode) ? false : true;
            $(loaderDiv).addClass("RADBCalenderHiddenClass");
            var isCalenderCreated = responseText.d;
            if (isCalenderCreated) {
                RADBusinessCalendar.instance.CommonSucessFailAlert($("#RADBCalenderMainDiv"), true, isCreation, "Calender");
            }
            else {
                RADBusinessCalendar.instance.CommonSucessFailAlert($("#RADBCalenderMainDiv"), false, isCreation, "Calender");
            }
        });
    }

}

RADBusinessCalendar.prototype.ScrollData = function (event) {

    if ($(event.target).hasClass("RADBCalenderLeftSectionScrollDown")) {
        $(".RADBCalenderLeftSectionContent").scrollTop($(".RADBCalenderLeftSectionContent").scrollTop() + 50);
    }
    else if ($(event.target).hasClass("RADBCalenderLeftSectionScrollUp")) {
        $(".RADBCalenderLeftSectionContent").scrollTop($(".RADBCalenderLeftSectionContent").scrollTop() - 50);
    }
}

RADBusinessCalendar.prototype.CalculateHeight = function () {

    var totalHeight = (RADBusinessCalendar.instance.CurrentSelectedTabId == "Calender") ?
                      $("#RADBCalenderMainDiv").height() : $("#RADHolidayInclMainDiv").height();
    $("#RADBCalenderLeftSectionContentDiv").height(totalHeight - 100);
}

RADBusinessCalendar.prototype.CalculateGridHeight = function () {
    var totalHeight = $("#RADHolidayIncRightSectionDiv").height();
    var gridHeight = totalHeight - $("#RADHolidayIncNameParentDiv").height() - 100;
    $find("RADHolidayInclGridDiv").get_GridInfo().Height = gridHeight + "px"
}

RADBusinessCalendar.prototype.ShowHideControlsByCalenderType = function (event) {
    var calenderTypeId = $(event.target).attr("radpropcreatecalendertypeid");
    if (calenderTypeId == "1") {
        $(".RADBCalenderCreateCustomParent").show();
        $(".RADBCalenderCreateRefMasterParent").hide();
    }
    else {
        $(".RADBCalenderCreateCustomParent").hide();
        $(".RADBCalenderCreateRefMasterParent").show();
        RADBusinessCalendar.instance.GetAllEntityTypes(event);
    }
}

RADBusinessCalendar.prototype.GetAllEntityTypes = function (event) {
    RADBusinessCalendar.instance.AllEntityTypes = [];
    $.ajax({
        url: RADBusinessCalendar.instance.ServiceAddress + '/GetAllEntityTypes',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {        
        allEntityTypes = responseText.d;
        allEntityTypes.forEach(function (entityItem)
        {
            var currentEntityInfo = {};
            currentEntityInfo.EntityTypeId = entityItem.EntityTypeId;
            currentEntityInfo.EntityTypeDisplayName = entityItem.EntityTypeDisplayName;
            RADBusinessCalendar.instance.AllEntityTypes.push(currentEntityInfo);
        });
        
    });
}

RADBusinessCalendar.prototype.BindAllEntityTypes = function (event) {    
    RADBusinessCalendar.instance.ClearDropDowns();
    var $entitiesDropDownMain = $("<div>", {
        class: "RCalenderEntitiesDropDownMain"
    });

    var $currentRowEntityParent = $("<div>", {
        class: "RCalenderEntitiesDropDownParent"
    });

    for (var i = 0; i < RADBusinessCalendar.instance.AllEntityTypes.length; i++) {

        var $currentRowEntity = $("<div>", {
            class: "RCalenderEntitiesDropDownChild",
            text: RADBusinessCalendar.instance.AllEntityTypes[i].EntityTypeDisplayName
        });
        $currentRowEntity.attr("rentitytypeid", RADBusinessCalendar.instance.AllEntityTypes[i].EntityTypeId);
        $currentRowEntityParent.append($currentRowEntity);
    }
    $entitiesDropDownMain.append($currentRowEntityParent);
    $(".RCalenderRefEntityTypeSelection").append($entitiesDropDownMain);
}

RADBusinessCalendar.prototype.BindAllUniqueAttributes = function (event) {    
    RADBusinessCalendar.instance.ClearDropDowns();
    var $uniqueAttrDropDownMain = $("<div>", {
        class: "RCalenderUniqueAttrDropDownMain"
    });

    var $currentRowUniqueAttrParent = $("<div>", {
        class: "RCalenderUniqueAttrDropDownParent"
    });

    for (var i = 0; i < RADBusinessCalendar.instance.EntityUniqueColumns.length; i++) {

        var $currentRowUniqueAttr = $("<div>", {
            class: "RCalenderUniqueAttrDropDownChild",
            text: RADBusinessCalendar.instance.EntityUniqueColumns[i].Name
        });
        $currentRowUniqueAttr.attr("runiqueattributerealname", RADBusinessCalendar.instance.EntityUniqueColumns[i].AttributeRealName);
        $currentRowUniqueAttrParent.append($currentRowUniqueAttr);
    }
    $uniqueAttrDropDownMain.append($currentRowUniqueAttrParent);
    $(".RCalenderRefUniqueSelection").append($uniqueAttrDropDownMain);
}

RADBusinessCalendar.prototype.BindUniqueAttributeData = function (event) {    
    RADBusinessCalendar.instance.ClearDropDowns();
    var $uniqueAttrDataDropDownMain = $("<div>", {
        class: "RCalenderUniqueAttrDataDropDownMain"
    });

    var $currentRowUniqueAttrDataParent = $("<div>", {
        class: "RCalenderUniqueAttrDataDropDownParent"
    });

    for (var i = 0; i < RADBusinessCalendar.instance.CurrentAttributeData.length; i++) {

        var $currentRowUniqueAttrData = $("<div>", {
            class: "RCalenderUniqueAttrDataDropDownChild",
            text: RADBusinessCalendar.instance.CurrentAttributeData[i].Value
        });
        $currentRowUniqueAttrData.attr("rattributevalueid", RADBusinessCalendar.instance.CurrentAttributeData[i].Key);
        $currentRowUniqueAttrDataParent.append($currentRowUniqueAttrData);
    }
    $uniqueAttrDataDropDownMain.append($currentRowUniqueAttrDataParent);
    $(".RCalenderRefUniqueValueSelection").append($uniqueAttrDataDropDownMain);

}

RADBusinessCalendar.prototype.OnSelectEntityType = function (event) {
    var currentControl = $(event.target).closest(".RCalenderRefEntityTypeParent")
                                                     .find(".RCalenderRefEntityTypeSelectionText");
    var entityTypeId = $(event.target).attr("rentitytypeid");
    RADBusinessCalendar.instance.CurrentEntityId = entityTypeId;
    RADBusinessCalendar.instance.CurrentEntityName = $(event.target).html().trim();
    currentControl.attr("rentitytypeid", entityTypeId);
    currentControl.html($(event.target).html().trim());
    $(event.target).closest(".RCalenderEntitiesDropDownMain").remove();
    RADBusinessCalendar.instance.GetEntityUniqueColumns(entityTypeId);
    RADBusinessCalendar.instance.GetLegsByEntityId(entityTypeId);
    RADBusinessCalendar.instance.OnChangeEntityType(event);
    
}

RADBusinessCalendar.prototype.OnSelectAttributeName = function (event) {
    var currentControl = $(event.target).closest(".RCalenderRefUniqueParent")
                                                     .find(".RCalenderRefUniqueSelectionText");    
    currentControl.attr("rentitytypeid", RADBusinessCalendar.instance.CurrentEntityId);
    currentControl.attr("runiqueattributerealname", $(event.target).attr("runiqueattributerealname"));
    RADBusinessCalendar.instance.CurrentAttributeRealName = $(event.target).attr("runiqueattributerealname");
    currentControl.html($(event.target).html().trim());
    $(event.target).closest(".RCalenderUniqueAttrDropDownMain").remove();
    RADBusinessCalendar.instance.GetDataByAttributeName(RADBusinessCalendar.instance.CurrentEntityId, $(event.target).html().trim());
}

RADBusinessCalendar.prototype.OnSelectAttributeValue = function (event) {
    var currentControl = $(event.target).closest(".RCalenderRefUniqueValueParent")
                                                     .find(".RCalenderRefUniqueValueSelectionText");
    currentControl.attr("rentitytypeid", RADBusinessCalendar.instance.CurrentEntityId);
    currentControl.attr("runiqueattributerealname", RADBusinessCalendar.instance.CurrentAttributeRealName);
    currentControl.attr("rattributevalueid", $(event.target).attr("rattributevalueid"));
    RADBusinessCalendar.instance.CurrentAttributeValueId = $(event.target).attr("rattributevalueid");
    currentControl.html($(event.target).html().trim());
    $(event.target).closest(".RCalenderUniqueAttrDataDropDownMain").remove();
}

RADBusinessCalendar.prototype.GetLegsByEntityId = function (entityTypeId) {
    RADBusinessCalendar.instance.LegsByEntityId = {};
    $.ajax({
        url: RADBusinessCalendar.instance.ServiceAddress + '/GetEntityTypesByEntityId',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ entityId: entityTypeId })
    }).then(function (responseText) {
        RADBusinessCalendar.instance.LegsByEntityId = responseText.d;        
    });
}

RADBusinessCalendar.prototype.GetEntityUniqueColumns = function (entityTypeId) {
    RADBusinessCalendar.instance.EntityUniqueColumns = [];
    $.ajax({
        url: RADBusinessCalendar.instance.ServiceAddress + '/GetEntityUniqueColumns',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ entityId: entityTypeId })
    }).then(function (responseText) {        
        var allAttributeInfo = responseText.d;
        allAttributeInfo.forEach(function (currentAttributeInfo) {
            var currentEntityInfo = {};
            currentEntityInfo.AttributeRealName = currentAttributeInfo.AttributeRealName;
            currentEntityInfo.Name = currentAttributeInfo.Name;
            RADBusinessCalendar.instance.EntityUniqueColumns.push(currentEntityInfo);
        });

    });
}

RADBusinessCalendar.prototype.GetDataByAttributeName = function (entityTypeId, attributeName) {
    RADBusinessCalendar.instance.CurrentAttributeData = [];
    $.ajax({
        url: RADBusinessCalendar.instance.ServiceAddress + '/GetDataByAttributeName',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ entityId: entityTypeId, entityName : RADBusinessCalendar.instance.CurrentEntityName, attributeName: attributeName })
    }).then(function (responseText) {
        RADBusinessCalendar.instance.CurrentAttributeData = responseText.d;
    });
}

RADBusinessCalendar.prototype.BindLegsDetails = function () {    
    RADBusinessCalendar.instance.ClearDropDowns();
    var $refHEntityDataDropDownMain = $("<div>", {
        class: "RCalenderRefHEntityDropDownMain"
    });

    var $currentRowRefHEntityDataParent = $("<div>", {
        class: "RCalenderRefHEntityDataDropDownParent"
    });
    var $currentRowRefHEntity = $("<div>", {
        class: "RCalenderRefHEntityDataDropDownChild",
        text: RADBusinessCalendar.instance.LegsByEntityId[0].Name
    });
    $currentRowRefHEntity.attr("rholidayentitytypeid", RADBusinessCalendar.instance.LegsByEntityId[0].EntityTypeId);
    $currentRowRefHEntityDataParent.append($currentRowRefHEntity);
    $refHEntityDataDropDownMain.append($currentRowRefHEntityDataParent);
    $(".RCalenderRefHEntityTypeSelection").append($refHEntityDataDropDownMain);
}

RADBusinessCalendar.prototype.BindLegsStringAttributes = function () {

    var LegsAttributesStringType = $.grep(RADBusinessCalendar.instance.LegsByEntityId[0].Attributes, function (e)
    { return e.Type.toLowerCase() == "string" && e.IsUnique == true;});
    
    RADBusinessCalendar.instance.ClearDropDowns();
    var $refHNameDataDropDownMain = $("<div>", {
        class: "RCalenderRefHNameDropDownMain"
    });

    var $currentRowRefHNameDataParent = $("<div>", {
        class: "RCalenderRefHNameDataDropDownParent"
    });

    for (var i = 0; i < LegsAttributesStringType.length; i++) {
        var $currentRowRefHEntity = $("<div>", {
            class: "RCalenderRefHNameDataDropDownChild",
            text: LegsAttributesStringType[i].Name
        });
        $currentRowRefHEntity.attr("rholidaynameattributerealname", LegsAttributesStringType[i].AttributeRealName);
        $currentRowRefHNameDataParent.append($currentRowRefHEntity);
    }
    $refHNameDataDropDownMain.append($currentRowRefHNameDataParent);
    $(".RCalenderRefHNameSelection").append($refHNameDataDropDownMain);
}

RADBusinessCalendar.prototype.BindLegsDateAttributes = function () {

    var LegsAttributesDateType = $.grep(RADBusinessCalendar.instance.LegsByEntityId[0].Attributes, function (e)
    { return e.Type.toLowerCase() == "datetime"});
    
    RADBusinessCalendar.instance.ClearDropDowns();
    var $refHDateDataDropDownMain = $("<div>", {
        class: "RCalenderRefHDateDropDownMain"
    });

    var $currentRowRefHDateDataParent = $("<div>", {
        class: "RCalenderRefHDateDataDropDownParent"
    });

    for (var i = 0; i < LegsAttributesDateType.length; i++) {
        var $currentRowRefHDate = $("<div>", {
            class: "RCalenderRefHDateDataDropDownChild",
            text: LegsAttributesDateType[i].Name
        });
        $currentRowRefHDate.attr("rholidaydateattributerealname", LegsAttributesDateType[i].AttributeRealName);
        $currentRowRefHDateDataParent.append($currentRowRefHDate);
    }
    $refHDateDataDropDownMain.append($currentRowRefHDateDataParent);
    $(".RCalenderRefHDateSelection").append($refHDateDataDropDownMain);

}

RADBusinessCalendar.prototype.OnSelectRefHolidayEntity = function (event) {
    var currentControl = $(event.target).closest(".RCalenderRefHEntityTypeParent")
                                                     .find(".RCalenderRefHEntityTypeSelectionText");
    RADBusinessCalendar.instance.CurrentEntityIdHoliday = $(event.target).attr("rholidayentitytypeid");
    currentControl.attr("rholidayentitytypeid", RADBusinessCalendar.instance.CurrentEntityIdHoliday);
    currentControl.html($(event.target).html().trim());
    $(event.target).closest(".RCalenderRefHEntityDropDownMain").remove();
}

RADBusinessCalendar.prototype.OnSelectRefHolidayName = function (event) {

    var currentControl = $(event.target).closest(".RCalenderRefHNameMainParent")
                                                     .find(".RCalenderRefHNameSelectionText");
    currentControl.attr("rholidaynameattributerealname", $(event.target).attr("rholidaynameattributerealname"));
    RADBusinessCalendar.instance.CurrentHolidayNameColumn = $(event.target).attr("rholidaynameattributerealname");
    currentControl.html($(event.target).html().trim());
    $(event.target).closest(".RCalenderRefHNameDropDownMain").remove();
}


RADBusinessCalendar.prototype.OnSelectRefHolidayDate = function (event) {

    var currentControl = $(event.target).closest(".RCalenderRefHDateParent")
                                                     .find(".RCalenderRefHDateSelectionText");
    currentControl.attr("rholidaydateattributerealname", $(event.target).attr("rholidaydateattributerealname"));
    RADBusinessCalendar.instance.CurrentStartDateColumn = $(event.target).attr("rholidaydateattributerealname");
    currentControl.html($(event.target).html().trim());
    $(event.target).closest(".RCalenderRefHDateDropDownMain").remove();
}


RADBusinessCalendar.prototype.OnChangeEntityType = function (event) {    
    $(".RCalenderRefUniqueSelectionText").html("--Select--");
    $(".RCalenderRefUniqueSelectionText").removeAttr("rentitytypeid");
    $(".RCalenderRefUniqueSelectionText").removeAttr("runiqueattributerealname");

    $(".RCalenderRefUniqueValueSelectionText").html("--Select--");
    $(".RCalenderRefUniqueValueSelectionText").removeAttr("rentitytypeid");
    $(".RCalenderRefUniqueValueSelectionText").removeAttr("runiqueattributerealname");
    $(".RCalenderRefUniqueValueSelectionText").removeAttr("rattributevalueid");

    $(".RCalenderRefHEntityTypeSelectionText").html("--Select--");
    $(".RCalenderRefHEntityTypeSelectionText").removeAttr("rholidayentitytypeid");

    $(".RCalenderRefHNameSelectionText").html("--Select--");
    $(".RCalenderRefHNameSelectionText").removeAttr("rholidaynameattributerealname");

    $(".RCalenderRefHDateSelectionText").html("--Select--");
    $(".RCalenderRefHDateSelectionText").removeAttr("rholidaydateattributerealname");
}

RADBusinessCalendar.prototype.ClearControlsOnCreate = function (event) {
    $(".RCalenderRefEntityTypeSelectionText").html("--Select--");
    RADBusinessCalendar.instance.OnChangeEntityType();
    RADBusinessCalendar.instance.ClearDropDowns();
    $("#RADPropCreateCalenderNameValueDiv").css({ "pointer-events": "all" });
    $("#RADPropCreateCalenderNameValueDiv").html("");
    $("#RADPropCreateCalenderDescValueDiv").html("");    
    $('#RADPropFileTypeSelect').val("--Select--");
    $("#RADPropHolidayNameIndexValueDiv").html("");
    $("#RADPropHolidayDateIndexValueDiv").html("");    
    $('#RADPropFileTypeSelect').val("--Select--");
    $("#RADPropRecurrentValueCheck")[0].checked = false;
    $("#RADPropCreateDefWeek1")[0].checked = false
    $("#RADPropCreateDefWeek2")[0].checked = false
    $("#RADPropCreateDefWeek3")[0].checked = false
    $("#RADPropCreateDefWeek4")[0].checked = false
    $("#RADPropCreateDefWeek5")[0].checked = false
    $("#RADPropCreateCalenderTypeCustom")[0].checked = true;
}

RADBusinessCalendar.prototype.ClearDropDowns = function (event) {
    $(".RCalenderEntitiesDropDownMain").remove();
    $(".RCalenderUniqueAttrDropDownMain").remove();
    $(".RCalenderUniqueAttrDataDropDownMain").remove();
    $(".RCalenderRefHEntityDropDownMain").remove();
    $(".RCalenderRefHNameDropDownMain").remove();
    $(".RCalenderRefHDateDropDownMain").remove();

}


