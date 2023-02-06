//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/********************************Custom Dropdown Using SMSelect ********************************/


//Create Multiselect using SMSelect 
/*      listId:  original Dropdown ID,
containerId: parent Div ID of original Dropdown,
text: Text to show with All ,
sectionHeaderClass: Custom Group Header class       */
var createMultiSelect = function (listId, containerId, text, sectionHeaderClass) {
    var ddlId = "#" + listId;

    var obj = new Object();
    obj.id = listId;
    obj.text = text;
    obj.allText = 'All ' + text;
    obj.isMultiSelect = true;
    obj.showSearch = true;
    obj.container = $("#" + containerId);

    var hdnSelectedId = "#hdnSelected" + listId + "Text";
    var sectionHeader = '';
    var arr = [];
    var arrSelected = [];
    var finalArr = [];


    for (var i = 0; i < $(ddlId).find('option').length; i++) {
        var optionElement = $($(ddlId).find('option')[i]);

        if (optionElement.hasClass(sectionHeaderClass)) {
            if (arr.length > 0) {
                finalArr.push({ options: arr, text: sectionHeader });
                arr = [];
                //arrSelected = [];
                sectionHeader = '';
            }
            sectionHeader = optionElement.text();
            continue;
        }
        var objKey = optionElement.text();
        var objValue = optionElement.val();
        arr.push({ text: objKey, value: objValue });
        arrSelected.push(objKey);
    }
    if ($(hdnSelectedId).val() !== "" && $(hdnSelectedId).val() != undefined) {
        var sel = $(hdnSelectedId).val().split(",");
    }
    else {
        sel = arrSelected;
    }

    if (arr.length > 0) {
        finalArr.push({ options: arr, text: sectionHeader });
        arr = [];
        //arrSelected = [];
        sectionHeader = '';
    }
    obj.data = finalArr;
    obj.selectedItems = sel;

    //Override some properties
    obj.ready = function (e) {
        e.width('95%');
        //To keep records of selected item
        e.on('change', function (ee) {
            storeSelectedAttributes(listId);
            var text = $('#smselect_' + listId).find('.smselectanchorrun').text();
            var selected = text.substr(text.length - 8);
            if (selected == "selected") {
                text = text.substr(0, text.length - 8);
                $('#smselect_' + listId).find('.smselectanchorrun').text(text);
            }
            else
                $('#smselect_' + listId).find('.smselectanchorrun').text(text);

        });
        e.ready(function (ee) {
            var text = $('#smselect_' + listId).find('.smselectanchorrun').text();
            var selected = text.substr(text.length - 8);
            if (text.substr(0, 2) == "1 " || text.substr(0, 2) == "1") {
                text = $("#hdnSelected" + listId + "Text").prop('value');
                $('#smselect_' + listId).find('.smselectanchorrun').text(text);
            }
            else if (selected == "selected") {
                text = text.substr(0, text.length - 8);
                $('#smselect_' + listId).find('.smselectanchorrun').text(text);
            }
            else
                $('#smselect_' + listId).find('.smselectanchorrun').text(text);
        });
    }
    smselect.create(obj);
}

//Keep records of select items in Multiselect
var storeSelectedAttributes = function (listId) {
    var multiSelectId = "#smselect_" + listId;
    var hdnSelectedId = "#hdnSelected" + listId;
    var hdnSelectedTextId = hdnSelectedId + "Text";
    var hidden = '';
    var hiddenText = '';
    var selectedAttr = smselect.getSelectedOption($(multiSelectId));
    for (var i = 0; i < selectedAttr.length; i++) {
        hidden += (selectedAttr[i].value) + ',';
        hiddenText += (selectedAttr[i].text) + ',';
    }

    if (hiddenText != undefined && hiddenText != null && hiddenText != "")
        hiddenText = hiddenText.substr(0, hiddenText.length - 1);
    if (hidden != undefined && hidden != null && hidden != "")
        hidden = hidden.substr(0, hidden.length - 1);
    else
        console.log("Nothing Selected,  in" + listId);
    $(hdnSelectedId).val(hidden);
    $(hdnSelectedTextId).val(hiddenText);

    //For Exception Manager
    if (listId == "EntityTypeDDL")
        storeSelectedID(listId, "hdnSelectedEntity");
    if (listId == "EntityAttributeDDL")
        storeSelectedID(listId, "hdnSelectedEntityAttribute");
}

//Store select items id in hiddenfield for Exception Manager specially
var storeSelectedID = function (listId, hdnId) {
    var multiSelectId = "#smselect_" + listId;
    var hdnSelectedId = "#" + hdnId;
    var SelectedId = "#hdnSelected" + listId;

    var hidden = $(SelectedId).val();

    if (hidden == undefined || hidden == null)
        console.log("Nothing Selected in" + listId + " for Exception");
    $(hdnSelectedId).val(hidden);
}

//Single Select DropDown using SMSelect

var createDropdownselect = function (oldDdlId, isSearch, isRunningText) {
    var obj = new Object();
    obj.select = $('[id$="' + oldDdlId + '"]');
    obj.showSearch = isSearch;
    obj.isRunningText = isRunningText;
    var newDdl = $('[id$="' + oldDdlId + '"]')[1];
    obj.ready = function (e) {
        e.on('change', function (ee) {
            var text = $('[id$="' + oldDdlId + '"]').find('.smselectanchorrun').text();
            if (text == "All Exceptions")
                $('[id$="' + oldDdlId + '"]').find('.smselectanchorrun').text("Exceptions");
        });
    };
    smselect.create(obj);
}

// To set item selected in Multiselect

var SetOptionByValueMultiSelect = function (listId, value) {
    var list = "#smselect_" + listId;
    $(list).find(".selectall").click();
    $(list).find(".smselectallmultioptionchecked").click();
    smselect.setOptionByValue($(list), value);
}

//Create Single Select DropDown With Group Header using SMSelect with Custom CSS

var createSingleSmselect = function (oldDdlId, isSearch, isRunningText, customCss) {
    var obj = new Object();
    obj.select = $('[id$="' + oldDdlId + '"]');
    obj.showSearch = isSearch;
    obj.isRunningText = isRunningText;
    var newDdl = $('[id$="' + oldDdlId + '"]')[1];
    obj.ready = function (e) {
        $('div[id$="ddlEntityType"]').find('.smselecttext').unbind('keyup');
        textBoxKeyUp($('div[id$="ddlEntityType"]').find('.smselecttext'));
    };
    smselect.create(obj);
    if (customCss)
        applyCustomCss(oldDdlId);
}

//Apply custom CSS to Group header in Single Select Dropdown

var applyCustomCss = function (ddlid) {
    var optList = $('div[id$="ddlEntityType"]').find('.smselectoptions').find('div');
    for (var i = 0; i < optList.length; i++) {
        var gid = optList[i].getAttribute("data-value");
        if (gid.charAt(0) == 'G') {
            optList[i].className = optList[i].className + " RMGroupDdlHeader";
        }
    }
}

//Override text search of smslect for Grouped SingleSelect

var textBoxKeyUp = function (elem) {

    elem.keyup(function (e) {
        e.stopPropagation();
        var con = $(this).parent().next();
        if (con.hasClass('smselectoptions'))
            options = con;
        else
            options = con.find('.smselectoptions');
        var value = $(this).val().trim().toLowerCase();
        if (options.css('display') === 'none') {
            $(options).toggle();
        }
        var which = e.which;
        var item;
        var foundFirst = false;
        var opts = options.find('.smselectoption');
        $.each(opts, function (ii, ee) {
            if (value !== '') {
                var attrVal = $(ee).attr('data-value');
                var curGroup = '';
                $(ee).removeClass('smselecthover');
                if (attrVal != undefined && attrVal.charAt(0) != 'G') {
                    if ($(ee).text().trim().toLowerCase().indexOf(value) > -1) {
                        if ((which === 13 || which === 9) && !foundFirst) {
                            foundFirst = true;
                            item = $(ee);
                            ee.style.display = "block";
                        }
                        else {
                            ee.style.display = "block";
                        }
                    }
                    else
                        ee.style.display = "none";
                }
            }
            else
                ee.style.display = "block";
        });
        /************To Hide Goup****************/
        var curG = '';
        var flag = false;

        $.each(opts, function (ii, ee) {
            if (value !== '') {
                var attrVal = $(ee).attr('data-value');
                if (attrVal != undefined && attrVal.charAt(0) == 'G') {
                    if (flag) {
                        curG = $(ee);
                        flag = false;
                        curG.css('display', 'block');
                    }
                    else if (curG != '') {
                        curG.css('display', 'none');
                        curG = $(ee);
                    }
                    else
                        curG = $(ee);
                    var nextG = $(ee).next();
                    if (nextG != null && nextG != undefined && nextG.css('display') != 'none') {
                        var nextAttrVal = nextG.attr('data-value');
                        if (nextAttrVal != undefined && nextAttrVal.charAt(0) == 'G') {
                            curG.css('display', 'none');
                        }
                    }
                }
                else if ($(ee).css('display') != 'none') {
                    flag = true;
                }
            }
            else
                ee.style.display = "block";
        });

        if (curG != '' && !flag) {
            //    if (curG.attr('data-value') == '1035')
            curG.css('display', 'none');
        }
        /***************************************/

        if (item instanceof jQuery && item.hasClass('smselectoption'))
            item.trigger('click');
    });

}

      //Date Control


function initDatePickerControl(pivot, options, defaultSelect, calType, count, ctrl) {
	var obj = {};
	var startDate = $('[id$=hdnStart' + ctrl + ']').val();
	var endDate = $('[id$=hdnEnd' + ctrl + ']').val();

	try {
		obj.dateOptions = options;
		obj.dateFormat = 'd/m/Y';
		obj.timePicker = false;
		obj.lefttimePicker = false;
		obj.righttimePicker = false;
		obj.calenderType = calType;
		obj.pivotElement = $('[id$=' + pivot + ']');
		obj.calenderID = 'smdd_' + count;

		if ($('[id$=hdnSelectedOption' + ctrl + ']') != null && $('[id$=hdnSelectedOption' + ctrl + ']').val().trim() != '')
			obj.selectedTopOption = $('[id$=hdnSelectedOption' + ctrl + ']').val();
		else {
			obj.selectedTopOption = defaultSelect;
			$('[id$=hdnSelectedOption' + ctrl + ']').val(defaultSelect);
		}


		if (startDate != null && startDate != "")
			startDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(startDate), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
		else {
			startDate = new Date();
			if (ctrl.indexOf("AsofDate") >= 0)
				startDate.setDate(startDate.getDate() - 15);
			else
				startDate.setDate(startDate.getDate() - 15);
		}

		if (endDate != null && endDate != "")
			endDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(endDate), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
		else {
			endDate = new Date();			
		}
		//What text by default to set in Last/Next (?)  Days
		if ($('[id$=hdnvar2' + ctrl + ']').val() != null && $('[id$=hdnvar2' + ctrl + ']').val() != '')
			obj.selectedNtimeText = $('[id$=hdnvar2' + ctrl + ']').val();

		//What option by default to set in Last/Next 15  (?) -- { Days/Weeks/Months/Years }
		if ($('[id$=hdnvar3' + ctrl + ']').val() != null && $('[id$=hdnvar3' + ctrl + ']').val() != '')
			obj.selectedNtimeOption = $('[id$=hdnvar3' + ctrl + ']').val();

		if ($('[id$=hdnRdbSelected' + ctrl + ']').val() != null && $('[id$=hdnRdbSelected' + ctrl + ']').val() != '')
			obj.selectedCustomRadioOption = $('[id$=hdnRdbSelected' + ctrl + ']').val();

		//Which date to set in calender
		obj.EndDateCalender = endDate;
		obj.StartDateCalender = startDate;
		$('[id$=hdnStart' + ctrl + ']').val(startDate)
		$('[id$=hdnEnd' + ctrl + ']').val(endDate);
	}
	catch (ex) { }
	obj.applyCallback = function () {
		var modifiedText = smdatepickercontrol.getResponse($("#smdd_" + count));
		var selectedEndDate = modifiedText.selectedEndDate;
		var selectedStartDate = modifiedText.selectedStartDate;
		var selectedText = modifiedText.selectedText;
		var selectedRadioOption = modifiedText.selRadioOption;
		var selectedTopOption = modifiedText.selDateOption;
		var selectedNtimeText = modifiedText.NtimeText == undefined || modifiedText.NtimeText == null ? "15" : modifiedText.NtimeText;
		var selectedNtimeOption = modifiedText.NtimeOption == undefined || modifiedText.NtimeOption == null ? "days" : modifiedText.NtimeOption;
		var selectedNtimeValue = modifiedText.NtimeValue == undefined || modifiedText.NtimeValue == null ? "1" : modifiedText.NtimeValue;

		//		startDate = new Date();
		//startDate.setFullYear(1970, 01, 01);
		var htmlString = "";
		var prepString = "";
		if (selectedStartDate != undefined && selectedStartDate != null && selectedStartDate != "")
			selectedStartDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(selectedStartDate), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
		else
			selectedStartDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(startDate), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));

		if (selectedEndDate != undefined && selectedEndDate != null && selectedEndDate != "")
			selectedEndDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(selectedEndDate), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
		else
			selectedEndDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
		if (selectedText.toUpperCase() === "TODAY")
			htmlString = "Today";
		else if (calType == 1)
			htmlString = selectedStartDate;
		else if (selectedText.toUpperCase() === "SINCE YESTERDAY")
			htmlString = "Yesterday";
		else if (selectedText.toUpperCase() === "THIS WEEK")
			htmlString = "This Week";
		else if (selectedText.toUpperCase() === "ANYTIME")
		{ prepString = ""; htmlString = "anytime"; }
		else if (selectedText.toUpperCase() === "CUSTOM DATE") {
			if (selectedRadioOption == 0)
				htmlString = " after " + selectedStartDate;
			else if (selectedRadioOption == 1)
				htmlString = " between " + selectedStartDate + " to " + selectedEndDate;
			else if (selectedRadioOption == 2)
				htmlString = " before " + selectedEndDate;
		}
		else if (selectedText.toUpperCase() === "LAST DAYS")
		{ prepString = " for "; htmlString = " last " + modifiedText.NtimeText + " " + modifiedText.NtimeOption }
		else if (selectedText.toUpperCase() === "NEXT DAYS")
		{ prepString = " for "; htmlString = " next " + modifiedText.NtimeText + " " + modifiedText.NtimeOption }

		$('[id$=hdnvar1' + ctrl + ']').val();
		$('[id$=hdnvar2' + ctrl + ']').val(selectedNtimeText);
		$('[id$=hdnvar3' + ctrl + ']').val(selectedNtimeOption);
		//		$('[id$=hdnvar4' + ctrl + ']').val(selectedNtimeValue);

		$('[id$=hdnString' + ctrl + ']').val(htmlString);
		$('[id$=hdnPrepString' + ctrl + ']').text(prepString);
		$('[id$=hdnSelectedOption' + ctrl + ']').val(selectedTopOption);
		$('[id$=hdnRdbSelected' + ctrl + ']').val(selectedRadioOption);
		$('[id$=hdnStart' + ctrl + ']').val(selectedStartDate);
		$('[id$=hdnEnd' + ctrl + ']').val(selectedEndDate);
		$('[id$=' + pivot + ']').text(htmlString);
		//val= UI date string | SelectedOption | RDB Selected | Start date | End date 
		$('[id$=hdn' + ctrl + ']').val(htmlString + '|' + selectedTopOption + '|' + selectedRadioOption + '|' + selectedStartDate + '|' + selectedEndDate);

		$('#hdnFirstTime' + ctrl).val("1");
		return false;
	}
	obj.ready = function (e) {
		//smselect		ddllastNtime_0 / ddlnextNtime_0		
		changeDropDown($('#ddllastNtime_' + count), false);
		$('#smselect_ddllastNtime_' + count).css('vertical-align', 'bottom');
	}

	smdatepickercontrol.init(obj);
	//obj.applyCallback();
}



function changeDropDown(targetId) {
	var obj = new Object();
	obj.select = $(targetId);
	obj.showSearch = false;
	obj.isRunningText = true;

	obj.ready = function (smselect) {
	}
	smselect.create(obj);
}



/******************************** End of Custom Dropdown using SMSelect **********************************/
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/******************************** To Initialise  New Rad FileUpload Widget *******************************/
var rmUploadFileWidget = function (parentId, attachmentId) {

    $('#' + parentId).remove();
    $('#RMFileUpload').append('<div  style="border:0px;" id="' + parentId + '"><div class="RMLabeledInput" style="width: 100%; border:0px;padding: 0px; text-indent: 8px;" id="' + attachmentId + '"> Click here or drop a file to upload</div>')
    if ($('#' + parentId).fileUpload != undefined) {
        $('#' + parentId).fileUpload({
            'parentControlId': parentId,
            'attachementControlId': attachmentId,
            'multiple': false,
            'debuggerDiv': '',
            'returnEvent': function () {
                //CashMPaymentBlotter.instance.FileUploaded();
            },
            'deleteEvent': function () {
                //alert('deleted'); } 
            }
        });
    }
}

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/**************************************   GRID RESIZE  ***************************************************/
// RADX Grid Resize Width and Height both

var rmRadxGridResize = function (radxGridID, parentID, subWidth, subHeight) {
	//var userAgent = navigator.userAgent.toString().toLowerCase();
	//    if (userAgent.indexOf('chrome') > 0)
	//        subHeight += 50;
	
	if ($('[id$="' + radxGridID + '"]').length > 0 && $('[id$="' + parentID + '"]').length > 0) {
		$('[id$="' + radxGridID + '"]').width($('[id$="' + parentID + '"]').width() - subWidth - 20);
		$('[id$="' + radxGridID + '_gridPanel"]').width($('[id$="' + parentID + '"]').width() - subWidth);
		$('[id$="' + radxGridID + '_gridTablePanel"]').width($('[id$="' + parentID + '"]').width() - subWidth);
		$('[id$="' + radxGridID + '_gridPanel"]').css('margin', '0px auto');

		$('[id$="' + radxGridID + '_gridTablePanel"]').height(document.body.clientHeight);
		// $('[id$="' + radxGridID + '"]').height($('[id$="' + parentID + '"]').height() - subHeight - 50);

		//  $('[id$="' + radxGridID + '_gridTablePanel"]').smslimscroll({ railVisible: true, alwaysVisible: false });
	}
}

// RADX Grid Resize Width only

var rmRadxGridWidthResize = function (radxGridID, parentID, subWidth) {

    if ($('[id$="' + radxGridID + '"]').length > 0 && $('[id$="' + parentID + '"]').length > 0) {
        $('[id$="' + radxGridID + '"]').width($('[id$="' + parentID + '"]').width() - subWidth - 20);
        $('[id$="' + radxGridID + '_gridPanel"]').width($('[id$="' + parentID + '"]').width() - subWidth);
        $('[id$="' + radxGridID + '_gridTablePanel"]').width($('[id$="' + parentID + '"]').width() - subWidth);
        $('[id$="' + radxGridID + '_gridPanel"]').css('margin', '0px auto');
    }
}

// RADX Grid Resize Inline (Only Required height to be assigned)
var rmRadxGridResizeInline = function (radxGridID, parentID, subWidth, subHeight) {
    if ($('[id$="' + radxGridID + '"]').length > 0 && $('[id$="' + parentID + '"]').length > 0) {

        $('[id$="' + radxGridID + '"]').width($('[id$="' + parentID + '"]').width() - subWidth - 20);
        $('[id$="' + radxGridID + '_gridPanel"]').css('margin', '0px auto');
        var requiredHeight = $('[id$="' + radxGridID + '"]').height()+20;
        var maxAvailableHeight = $('[id$="' + parentID + '"]').height() - subHeight-40;
        if (requiredHeight > maxAvailableHeight)
            $('[id$="' + radxGridID + '_gridTablePanel"]').height(maxAvailableHeight);
        else
            $('[id$="' + radxGridID + '_gridTablePanel"]').height(requiredHeight);
    }
}
// Resize grid on Change using grid Properties 
function RMresizeGridFinal(gridID, topPanel, middlePanel, bottomPanel, subWidth, subHeight) {

    var bottomPanelElement = $("[id$='" + bottomPanel + "']");
    var topPanelElement = $("[id$='" + topPanel + "']");
    var middlePanelElement = $("[id$='" + middlePanel + "']");

    if ($(top.document.body).find('.iago-page-title').length > 0)
        subHeight += $(top.document.body).find('.iago-page-title').height() + 20;
    if (topPanelElement.length > 0)
        subHeight += topPanelElement.height();
    if (bottomPanelElement.length > 0)
        subHeight += bottomPanelElement.height();

    var docHeight = $(top).height() - subHeight;
    var docWidth = $(top).width() - subWidth - 12;
    var middlePanelHeight = docHeight;
    if (middlePanelElement.length > 0) {

        middlePanelElement.height(middlePanelHeight);
        middlePanelElement.width(docWidth);

        if (topPanelElement.width() > docWidth) {
            middlePanelElement.width(topPanelElement.width());
            bottomPanelElement.width(topPanelElement.width());
        }
        else {
            middlePanelElement.width(docWidth);
            bottomPanelElement.width(docWidth);
        }
        middlePanelElement.css("margin", "0px auto");
    }
    //findElements inside the grid

    var upperHeaderDiv = $("[id$='upperHeader_Div']");
    var footerDiv = $("[id$='footer_Div']");
    var bodyDiv = $("[id$='body_Div']"); //including frozen div
    var bodyDivHeight = middlePanelHeight - upperHeaderDiv.height() - footerDiv.height();

    upperHeaderDiv.width(docWidth);
    upperHeaderDiv.css('margin', '0px auto');
    bodyDiv.width(docWidth);
    bodyDiv.css('margin', '0px auto');
    footerDiv.width(docWidth);
    footerDiv.css('margin', '0px auto');

    //find and set the height of elements inside bodyDiv
    var headerDiv = $("[id$='" + gridID + "_headerDiv']"); //header
    var frozenHeader = $("[id$='frozen_headerDiv']"); //frozenheader
    var frozenBodyDiv = $("[id$='frozen_bodyDiv']");
    var innerBodyDiv = $("[id$='" + gridID + "_bodyDiv']"); //excluding frozen div
    var bodyDivTable = $("[id$='bodyDiv_Table']");
    var headerDivTable = $("[id$='headerDiv_Table']");
    var scrollHorizontal = $("[id$='horizonslimScrollDiv']");
    var scrollVerticle = $("[id$='scrollerMainVertical']");

    var headerDivHeight = headerDiv.height();
    if (frozenHeader.length > 0)
        frozenHeader.height(headerDivHeight); //set the height of frozenheader equal to header div
    var frozenHeaderDivWidth = frozenHeader.width();
    if (frozenBodyDiv.length > 0)
        frozenBodyDiv.width(frozenHeaderDivWidth); //set the width of frozenbody equal to that for frozen header

    //var innerBodyDivHeight = bodyDivHeight - upperHeaderDiv.height() - scrollHorizontal.height();  //innerBodyDiv.height();
    var innerBodyDivHeight = bodyDivHeight - scrollHorizontal.height();  //innerBodyDiv.height();
    var scrollVerticleWidth = $($('.slimScrollRail')[0]).width();
    if (scrollVerticle.length > 0) {
        scrollVerticleWidth = scrollVerticle.width();
        scrollVerticle.height(bodyDivHeight);
    }
    var innerBodyDivWidth = docWidth - frozenHeaderDivWidth - scrollVerticleWidth;
    // alert(docWidth + "...." + innerBodyDivWidth);
    innerBodyDiv.height(innerBodyDivHeight);
    innerBodyDiv.width(innerBodyDivWidth);

    bodyDivTable.width(innerBodyDivWidth);
    headerDivTable.width(innerBodyDivWidth);


    var grid = $find(gridID);
    if (grid != null) {
        grid.get_GridInfo().Height = innerBodyDivHeight;
        //grid.get_GridInfo().Width = docWidth;
    }
}

// Resize grid on Change using Auto Adjust of grid width and height

function RMresizeGridFinalAuto(gridID, topPanel, middlePanel, bottomPanel, subWidth, subHeight) {
	var bottomPanelElement = $("[id$='" + bottomPanel + "']");
	var topPanelElement = $("[id$='" + topPanel + "']");
	var middlePanelElement = $("[id$='" + middlePanel + "']");
	var bodyDivTable = $("[id$='bodyDiv_Table']");
	var headerDivTable = $("[id$='headerDiv_Table']");

	if ($(top.document.body).find('.iago-page-title').length > 0)
		subHeight += $(top.document.body).find('.iago-page-title').height() + 80;
	if (topPanelElement.length > 0)
		subHeight += topPanelElement.height();
	if (bottomPanelElement.length > 0)
		subHeight += bottomPanelElement.height();

	var docHeight = $(top).height() - subHeight - 60;
	var docWidth = $(top).width() - subWidth - 18;

	var grid = $find(gridID);
	if (grid != null) {
		grid.get_GridInfo().Height = docHeight;
		$('#' + gridID).css('margin', '0px auto');
		$('#' + gridID).width(docWidth);
	}

	$('#' + gridID + '_bodyDiv_Table').width($('#' + gridID + '_bodyDiv').width());
	$('#' + gridID + '_headerDiv_Table').width($('#' + gridID + '_bodyDiv').width());
	if ($('#' + gridID + '_selectAllOption_Div') != null) {
		$('#' + gridID + '_selectAllOption_Div').width($('#' + gridID + '_bodyDiv').width());
		$('#' + gridID + '_selectAllOption_Div').css('position','fixed');
		$('#' + gridID + '_selectAllOption_Div').css('z-index','10');
		$('#' + gridID + '_selectAllOption_Div').css('opacity','0.9');    
	}
}

function RefreshNeoGridWithID(gridID) {
	var grid = $find(gridID);
	if (grid != null)
		grid.refreshGrid();
	else if (gridID != undefined && $find(gridID.id) != null) {
		$find(gridID.id).refreshGrid();
	}
}

var rmRadNeoGridPopupResize = function () {
	//var userAgent = navigator.userAgent.toString().toLowerCase();
	//    if (userAgent.indexOf('chrome') > 0)

	var docHeight = $('[id$="FailedRecordPopUp"]').height() - 20;
	var docWidth = $('[id$="FailedRecordPopUp"]').width() - 50;
	var gridID = 'FailedRecordGrid';
	var grid = $find(gridID);
	if (grid != null) {
		//grid.get_GridInfo().Height = $('[id$="' + parentID + '"]').height() - subHeight - 20;
		$('#' + gridID).css('margin', '0px auto');
		$('#' + gridID).width(docWidth);
	}
}

/**************************************END GRID RESIZE ***************************************************/
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//Change color of status column in grid by Text value

var setStatusColumColor = function (gridId, rowIndex) {
    $('[id$="' + gridId + '"]').find('tr').each(function (index, value) {
        var td = $(value).find('td')[rowIndex];
        if (td != undefined && td != null)
            var tdText = $(td).text().trim().toLowerCase();
        if (tdText == 'passed')
            $(td).find('span').css('color', 'green');
        else if (tdText == 'failed')
            $(td).css('color', 'red');
        else if (tdText == 'inprogress')
            $(td).css('color', '#00bff2');
    });
}
     
//*****************************************************************************************************//
//Refresh Parent Window after changes applied in Database , specially after deletion of leg entity

function rmRefreshParentWindow() {
    try {
        var buttonId = $('[id$="hdnParentButtonId"]').val();
        isSuccessPopupClick = true;
        //Refresh in case of Leg Entity Type Delete
        if (buttonId != "" && window.parent.document.getElementById(buttonId) != null) {
            window.parent.document.getElementById(buttonId).click();
        }
        //Refresh in case of Entity Type Delete
        else {
            window.parent.location.reload();
            //window.opener.document.location.href = window.opener.document.location.href
        }

        //Get iframe attribute (named  unique) value from hidden input with ID hdnTabIframeUniqueVal
        if (document.getElementById("hdnTabIframeUniqueVal") != null) {
            var tabId = document.getElementById("hdnTabIframeUniqueVal").value;

            //Search for iframe and Reload
            if ($(window.opener.top.document).find('iframe[unique=' + tabId + ']') != null)
                $(window.opener.top.document).find('iframe[unique=' + tabId + ']')[0].contentWindow.location.reload();
        }
    }
    catch (e) {
    }
    finally {
        window.close();
    }
}

function getRMLeftMenu() {
    var refMparent = window.parent;
    while ((refMparent.leftMenu == null || refMparent.leftMenu == undefined)) {
        if (refMparent.parent != refMparent)
            refMparent = refMparent.parent;
        else
            break;
    }
    return refMparent.leftMenu;
}

function RM_openTab(identifier, url, uniqueId, uniqueText) {
    if (typeof (getRMLeftMenu()) == 'undefined') {
        window.open(url);
    }
    else {
        getRMLeftMenu().createTab(identifier, url, uniqueId, uniqueText);
    }
}