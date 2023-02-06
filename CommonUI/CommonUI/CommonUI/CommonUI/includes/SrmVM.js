var _controlIdInfo = null;
var _securityInfo = null;

var srmVmStatus = {};
srmVmStatus.tableInstance = null;

srmVmStatus._selectedDataSource = null;
srmVmStatus._selectedDataReqMethod = null;

//Storing the data when you get from the server
srmVmStatus.bloombergJsonData = null;
srmVmStatus.reutersJsonData = null;
srmVmStatus.markITJsonData = null;

srmVmStatus.addNewPreferenceId = "AddNewPreference";
srmVmStatus.savePreferenceId = "SavePreference";

srmVmStatus.fixedTableId = "srmVMFixedConfigTable";
srmVmStatus.PreferencesDisplayId = "PreferencesList";
srmVmStatus.NamePreferenceId = "NamePreferenceInput";

srmVmStatus.prefSelected = false;
srmVmStatus.currPref;
srmVmStatus.currPrefID;
srmVmStatus.vendorPreferences = null;
srmVmStatus.selectedAddClone = {};
srmVmStatus.selectedAddClone.text = null;
srmVmStatus.selectedAddClone.id = null;

var addButtonClicked = false;

srmVmStatus.manualAddTrigger = false;

var objList = [];

srmVmStatus.dropdownSelected = null;

function VendorStringToId(name) {
    if (typeof (name) == "string") {
        var check = name.toLowerCase();
        if (check == "bloomberg")
            return 1;
        else if (check == "reuters")
            return 2;
        else if (check == "markit")
            return 3;
    }
    else if (typeof (name) == "int") {
        if (name == 1)
            return "Bloomberg";
        else if (name == 2)
            return "Reuters";
        else if (name == 3)
            return "MarkIt";
    }
}


function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
    callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
}

srmVmStatus.saveState = function () {
    //Before this is selected save the current state
    if (srmVmStatus._selectedDataSource == "Bloomberg" && srmVmStatus._selectedDataReqMethod) {
        //$('#' + srmVmStatus._selectedDataSource + srmVmStatus._selectedDataSource)

        var Table_Rows = $("#srmVMFixedConfigTable .TableRow");

        for (var i = 0; i < Table_Rows.length; i++) {
            //if (isInteger(parseInt(i)))

            var item = Table_Rows[i];
            var currKey = item.firstChild.innerText;

            var currValue = item.lastChild.innerText;            //as this is an input field

            //editing the current JSON
            srmVmStatus.bloombergJsonData.Configurations[srmVmStatus._selectedDataReqMethod][i].value = currValue;

        }

        //Saves changes in headers
        if (srmVmStatus._selectedDataReqMethod == "FTP") {
            var valueList = [];
            valueList.splice(0, valueList.length);
            var rows = $(".identifierGridRowsClass");
            if (srmVmStatus.dropdownSelected)
                var selectedDropDown = srmVmStatus.dropdownSelected;
            else {
                //var selectedDropDown = $("#srmVMDropDownMenu").find(":selected").text();
                var selectedDropDown = smselect.getSelectedOption($("#smselect_srmVMRequestTypeDropDown"))[0];
            }
            for (var i = 0; i < rows.length; i++) {
                var first = rows[i].children[0].innerText;
                var second = rows[i].children[1].innerText;
                //valueList.push({ "headerName": first, "headerValue": second });
                valueList.push({ "headerName": first, "headerValue": second });
            }

            for (i in srmVmStatus.bloombergJsonData.Headers.FTP) {
                var item = srmVmStatus.bloombergJsonData.Headers.FTP[i].requestType;
                if (item == selectedDropDown) {
                    srmVmStatus.bloombergJsonData.Headers.FTP[i].value = valueList;
                    break;
                }

            }

        }

            //Heavy Lite handling without the dropdown

            //else if (srmVmStatus._selectedDataReqMethod == "HeavyLite") {
            //    var valueList = [];
            //    var rows = $(".identifierGridRowsClass");

            //    for (var i = 0; i < rows.length; i++) {
            //        var first = rows[i].children[0].innerHTML;
            //        var second = rows[i].children[1].innerHTML;
            //        valueList.unshift({ "headerName": first, "headerValue": second });
            //        //valueList.push({ "headerName": first, "headerValue": second });
            //    }


            //    srmVmStatus.bloombergJsonData.Headers["HeavyLite"][0].value = valueList;



            //}
        else if (srmVmStatus._selectedDataReqMethod == "HeavyLite") {
            var valueList = [];
            valueList.splice(0, valueList.length);
            var rows = $(".identifierGridRowsClass");
            if (srmVmStatus.dropdownSelected)
                var selectedDropDown = srmVmStatus.dropdownSelected;
            else {
                //var selectedDropDown = $("#srmVMDropDownMenu").find(":selected").text();
                var selectedDropDown = smselect.getSelectedOption($("#smselect_srmVMRequestTypeDropDown"))[0];
            }
            for (var i = 0; i < rows.length; i++) {
                var first = rows[i].children[0].innerText;
                var second = rows[i].children[1].innerText;
                //valueList.push({ "headerName": first, "headerValue": second });
                valueList.push({ "headerName": first, "headerValue": second });
            }

            for (i in srmVmStatus.bloombergJsonData.Headers.HeavyLite) {
                var item = srmVmStatus.bloombergJsonData.Headers.HeavyLite[i].requestType;
                if (item == selectedDropDown) {
                    srmVmStatus.bloombergJsonData.Headers.HeavyLite[i].value = valueList;
                    break;
                }

            }
        }

    }

    if (srmVmStatus._selectedDataSource == "Reuters" && srmVmStatus._selectedDataReqMethod) {
        //$('#' + srmVmStatus._selectedDataSource + srmVmStatus._selectedDataSource)

        var Table_Rows = $("#srmVMFixedConfigTable .TableRow");

        for (var i = 0; i < Table_Rows.length; i++) {
            //if (isInteger(parseInt(i)))

            var item = Table_Rows[i];
            var currKey = item.firstChild.innerText;
            var currValue = item.lastChild.innerText;            //as this is an input field
            //srmVmStatus.reutersJsonData2 = srmVmStatus.reutersJsonData;
            srmVmStatus.reutersJsonData.Configurations[srmVmStatus._selectedDataReqMethod][i].value = currValue;
        }

        if (srmVmStatus._selectedDataReqMethod == "FTP") {
            var valueList = [];
            valueList.splice(0, valueList.length);
            var rows = $(".identifierGridRowsClass");
            if (srmVmStatus.dropdownSelected)
                var selectedDropDown = srmVmStatus.dropdownSelected;
            else {
                var selectedDropDown = $("#srmVMDropDownMenu").find(":selected").text();
            }
            for (var i = 0; i < rows.length; i++) {
                var first = rows[i].children[0].innerText;
                var second = rows[i].children[1].innerText;
                valueList.push({ "headerName": first, "headerValue": second });
            }

            for (i in srmVmStatus.reutersJsonData.Headers.FTP) {
                var item = srmVmStatus.reutersJsonData.Headers.FTP[i].requestType;
                if (item == selectedDropDown) {
                    srmVmStatus.reutersJsonData.Headers.FTP[i].value = valueList;
                    break;
                }

            }

        }
        //console.log(srmVmStatus.reutersJsonData);
    }

    if (srmVmStatus._selectedDataSource == "MarkIt") {
        var Table_Rows = $("#srmVMFixedConfigTable .TableRow");

        for (var i = 0; i < Table_Rows.length; i++) {
            var item = Table_Rows[i];
            var currKey = item.firstChild.innerText;
            var currValue = item.lastChild.innerText;
            srmVmStatus.markITJsonData.Configurations.SAPI[i].value = currValue;
        }
        console.log(srmVmStatus.markITJsonData);
    }
}

//click for the data source
$(".srmVMdataSource div").unbind('click').bind("click", function () {
    if (!srmVmStatus.prefSelected)
        srmVmStatus.saveState();

    $(".srmVMdataSource").find('.SelectedDataSourceClass').removeClass('SelectedDataSourceClass');
    $(this).addClass('SelectedDataSourceClass');
    //Does not include the focus on what's being clicked.
    srmVmStatus._selectedDataSource = $(this).text();
    //Mapping to data request
    srmVmStatus.dataSourceToReqMethodDefaults($(this).text());
    //how this is selected and told to the below part?
    //track of selected data source
    //srmVmStatus._selectedDataSource = $(this).text();               //Select

});

//click for the request type in the data source
$(".srmVMDataRequestMethod div").unbind('click').bind("click", function (event) {
    if (!srmVmStatus.prefSelected)
        srmVmStatus.saveState();
    $(".srmVMDataRequestMethod div").removeClass('SelectedTabClass');
    $(this).addClass('SelectedTabClass');

    srmVmStatus._selectedDataReqMethod = $(this).text();            //Select

    if (srmVmStatus._selectedDataSource == "Bloomberg") {

        srmVmStatus.bloombergFixedTable();
        srmVmStatus.displayDropDown();

    }
    if (srmVmStatus._selectedDataSource == "Reuters") {
        srmVmStatus.reutersFixedTable();
        //srmVmStatus.prefSelected = true;
        srmVmStatus.displayDropDown();
        //srmVmStatus.prefSelected = false;

    }
    //availableHeightForTables();


});

srmVmStatus.displayDropDown = function () {

    //make the select
    var str;
    var data = [];
    data.splice(0, data.length);
    if ((srmVmStatus._selectedDataReqMethod == "FTP" || srmVmStatus._selectedDataReqMethod == "HeavyLite")) {
        $("#HeadingText").show();
        $("#srmVMRequestTypeText").show();
        $("#bbgHeaderGrid").show();
        if (srmVmStatus._selectedDataSource == "Bloomberg") {

            for (var i in srmVmStatus.bloombergJsonData.Headers[srmVmStatus._selectedDataReqMethod]) {
                var item = srmVmStatus.bloombergJsonData.Headers[srmVmStatus._selectedDataReqMethod][i];
                data.push({ text: item.requestType, value: item.requestType });
            }

            //TO MAKE IT WORK WITHOUT REUTERS

            //$("#srmVMRequestTypeDD").html("");
            //ApplySMSelect('srmVMRequestTypeDropDown', false, data, false, '#srmVMRequestTypeDD', 'none');
            //$("#smselect_srmVMRequestTypeDropDown").show();
            //var selected = smselect.getSelectedOption($("#smselect_srmVMRequestTypeDropDown"))[0];

            //if (selected == null) {
            //    smselect.setOptionByIndex($('#smselect_srmVMRequestTypeDropDown'), 0);

            //}
            //InitiateBBGHeaderGrid("test1", "test2", "txtHeaderName", "txtHeaderValue", "hdnHeaderName", "hdnHeaderValue", "bbgHeaderGrid", "newGridRowDiv", "addRowButton");
        }
        else if (srmVmStatus._selectedDataSource == "Reuters" && srmVmStatus._selectedDataReqMethod == "FTP") {


            for (var i in srmVmStatus.reutersJsonData.Headers[srmVmStatus._selectedDataReqMethod]) {
                var item = srmVmStatus.reutersJsonData.Headers[srmVmStatus._selectedDataReqMethod][i];
                data.push({ text: item.requestType, value: item.requestType });
            }
            //console.log(data);
        }
        //sm select ka data is json like key value pair

        //SHIFTED TO Bloomberg after reuters is removed

        $("#srmVMRequestTypeDD").html("");
        ApplySMSelect('srmVMRequestTypeDropDown', false, data, false, '#srmVMRequestTypeDD', 'none');
        $("#smselect_srmVMRequestTypeDropDown").show();
        var selected = smselect.getSelectedOption($("#smselect_srmVMRequestTypeDropDown"))[0];

        if (selected == null) {
            smselect.setOptionByIndex($('#smselect_srmVMRequestTypeDropDown'), 0);

        }
        InitiateBBGHeaderGrid("test1", "test2", "txtHeaderName", "txtHeaderValue", "hdnHeaderName", "hdnHeaderValue", "bbgHeaderGrid", "newGridRowDiv", "addRowButton");
    }
        //HEAVY LITE NO DROPDOWN OLD
        //else if (srmVmStatus._selectedDataReqMethod == "HeavyLite") {
        //    $("#HeadingText").show();
        //    $("#smselect_srmVMRequestTypeDropDown").hide();
        //    $("#bbgHeaderGrid").show();
        //    if (srmVmStatus._selectedDataSource == "Bloomberg") {
        //        str = '<div id= "srmVMHeavyHeader">';
        //        str += srmVmStatus.bloombergJsonData.Headers[srmVmStatus._selectedDataReqMethod][0].requestType;
        //        str += '</div>'
        //        $("#srmVMRequestTypeDD").html(str);
        //    }
        //}

    else {
        //logic to not display apply slim select
        $("#smselect_srmVMRequestTypeDropDown").hide();
        $("#HeadingText").hide();
        $("#srmVMRequestTypeText").hide();
        $("#bbgHeaderGrid").hide();
        $("#srmVMRequestTypeDD").html("");
    }
    //if (srmVmStatus._selectedDataReqMethod == "Heavy/Lite")
    //    $("#srmVMRequestTypeDD").html(str);




}


srmVmStatus.saveCurrentState = function (dataSource, requestmethod) {
    //also need to write logic to check if nulls aren't sent back, and also those with changes are sent back with some change in a status bit probably
    console.log(srmVmStatus.bloombergJsonData);
    console.log(srmVmStatus.reutersJsonData);
    console.log(srmVmStatus.markITJsonData);
}


$("#srmSaveChanges").unbind('click').bind("click", function (event) {
    //Post Data back to server and keep a boolean value of what all changed and send the corresponding ajax requests
    srmVmStatus.saveCurrentState();
});


function onSuccess_BBData(result) {
    srmVmStatus.bloombergJsonData = result.d;

    srmVmStatus._selectedDataReqMethod = null;
    $(".srmVMDataRequestMethod").children()[0].click();
    //Unsure?
    //srmVmStatus.vendorPreferences = result.d.VendorPreferences;
    //populatePreferences();
    //$(".srmVMdataSource").children()[0].click();

}

function onSuccess_ReutersData(result) {
    srmVmStatus.reutersJsonData = result.d;
    srmVmStatus._selectedDataReqMethod = null;
    srmVmStatus.prefSelected = true;
    $(".srmVMDataRequestMethod").children()[0].click();
    srmVmStatus.prefSelected = false;
}

function onSuccess_MarkItData(result) {
    srmVmStatus.markITJsonData = result.d;
    srmVmStatus._selectedDataReqMethod = null;
    srmVmStatus.markItFixedTable();
    srmVmStatus.displayDropDown();

}
//TODO CSS Handling and show the clicked text with some focus
srmVmStatus.dataSourceToReqMethodDefaults = function (dataSource) {
    if (dataSource == "Bloomberg") {
        $(".srmVMDataRequestMethod div").show();                //make it visible
        $("#HeadingText").show();
        $("#srmVMRequestTypeText").show();
        $("#srmVMRequestTypeText").show();
        //$("#HeavyLite").show();
        //$("#GlobalApi").show();


        //AJAX call
        if (!srmVmStatus.bloombergJsonData) {
            var params = {};

            params.preferenceId = srmVmStatus.currPrefID;
            params.vendorId = VendorStringToId("Bloomberg");
            CallCommonServiceMethod('GetVendorManagementData', params, onSuccess_BBData, OnFailure, null, false);
            //srmVmStatus.bloombergJsonData = BloomBergJson;  
        }
        else {
            srmVmStatus.prefSelected = true;
            srmVmStatus._selectedDataReqMethod = null;
            $(".srmVMDataRequestMethod").children()[0].click();
            srmVmStatus.prefSelected = false;
        }

    }

    else if (dataSource == "Reuters") {
        $(".srmVMDataRequestMethod div").show();
        $("#HeadingText").show();

        $("#srmVMRequestTypeText").show();
        $("#HeavyLite").hide();
        $("#GlobalApi").hide();

        if (!srmVmStatus.reutersJsonData) {
            var params = {};

            params.preferenceId = srmVmStatus.currPrefID;
            params.vendorId = VendorStringToId("Reuters");
            CallCommonServiceMethod('GetVendorManagementData', params, onSuccess_ReutersData, OnFailure, null, false);
        }
        else {
            //No Ajax Call
            srmVmStatus.prefSelected = true;
            srmVmStatus._selectedDataReqMethod = null;
            $(".srmVMDataRequestMethod").children()[0].click();
            srmVmStatus.prefSelected = false;
        }
    }



    else if (dataSource == "MarkIt") {

        $(".srmVMDataRequestMethod div").hide();
        $("#HeadingText").hide();
        $("#srmVMRequestTypeText").hide();

        if (!srmVmStatus.markITJsonData) {
            var params = {};

            params.preferenceId = srmVmStatus.currPrefID;
            params.vendorId = VendorStringToId("MarkIt");
            CallCommonServiceMethod('GetVendorManagementData', params, onSuccess_MarkItData, OnFailure, null, false);
        }
        else {
            srmVmStatus._selectedDataReqMethod = null;
            srmVmStatus.markItFixedTable();
            srmVmStatus.displayDropDown();

        }


    }

}

//display BBG data in the first table
srmVmStatus.bloombergFixedTable = function () {
    var inputTableData = '<div class="TableHeader" id="' + srmVmStatus._selectedDataSource + srmVmStatus._selectedDataReqMethod + '"><div class="TableCell TableHeaderColor">Key</div><div class="TableCell TableHeaderColor">Value</div></div><div id="SlimScrollFixedTableParent"><div id="FixedTableRows">';
    //console.log(srmVmStatus._selectedDataReqMethod);
    var path = srmVmStatus.bloombergJsonData.Configurations[srmVmStatus._selectedDataReqMethod];
    for (item in path) {
        console.log(path[item]);
        inputTableData += '<div class="TableRow"><div class="TableCell">' + path[item].labelName + '</div><div class="TableCell" contenteditable="true">' + Encode(path[item].value) + '</div></div>';
    }
    //find the table ID
    inputTableData += '</div></div>'
    $('#' + srmVmStatus.fixedTableId).html(inputTableData);
}

function Encode(str) {
	var i = str.length,
		a = [];

	while (i--) {
		var iC = str[i].charCodeAt();
		if (iC < 65 || iC > 127 || (iC > 90 && iC < 97)) {
			a[i] = '&#' + iC + ';';
		} else {
			a[i] = str[i];
		}
	}
	return a.join('');
}


//displays reuters data in the second table
srmVmStatus.reutersFixedTable = function () {
    var inputTableData = '<div class="TableHeader"><div class="TableCell TableHeaderColor">Key</div><div class="TableCell TableHeaderColor">Value</div></div><div id="SlimScrollFixedTableParent"><div id="FixedTableRows">';
    //console.log(srmVmStatus._selectedDataReqMethod);
    var path = srmVmStatus.reutersJsonData.Configurations[srmVmStatus._selectedDataReqMethod];
    for (item in path) {
        console.log(path[item]);
        inputTableData += '<div class="TableRow"><div class="TableCell">' + path[item].labelName + '</div><div class="TableCell" contenteditable="true">' + Encode(path[item].value) + '</div></div>';
    }
    //find the table ID
    inputTableData += '</div></div>'
    $('#' + srmVmStatus.fixedTableId).html(inputTableData);
}

srmVmStatus.markItFixedTable = function () {
    var inputTableData = '<div class="TableHeader"><div class="TableCell TableHeaderColor">Key</div><div class="TableCell TableHeaderColor">Value</div></div><div id="SlimScrollFixedTableParent"><div id="FixedTableRows">';
    //different handling than the other 2 above
    var path = srmVmStatus.markITJsonData.Configurations["SAPI"];
    console.log(path);
    for (item in path) {

        inputTableData += '<div class="TableRow"><div class="TableCell">' + path[item].labelName + '</div><div class="TableCell" contenteditable="true">' + Encode(path[item].value) + '</div></div>';
    }
    inputTableData += '</div></div>'
    //find the table ID
    $('#' + srmVmStatus.fixedTableId).html(inputTableData);
}


//Headers ka previously written code

function IdentifierGridFormatting(gridId) {
    var addIdentifierGridDivs = $('#' + gridId).children();
    var rowClass = 'SMNewGridAlternatingRow';
    if (addIdentifierGridDivs.length > 2) {
        for (var index = 2; index < addIdentifierGridDivs.length; index++) {
            if (addIdentifierGridDivs.eq(index).hasClass('SMNewGridNormalRow')) {
                addIdentifierGridDivs.eq(index).removeClass('SMNewGridNormalRow');
            }
            if (addIdentifierGridDivs.eq(index).hasClass('SMNewGridAlternatingRow')) {
                addIdentifierGridDivs.eq(index).removeClass('SMNewGridAlternatingRow');
            }
            addIdentifierGridDivs.eq(index).addClass(rowClass);
            if (index % 2 == 0) {
                rowClass = 'SMNewGridNormalRow';
            }
            else {
                rowClass = 'SMNewGridAlternatingRow';
            }
        }
    }
}

function FindRowIndex(rowId, gridId) {
    var addIdentifierGridDivs = $('#' + gridId).children();
    if (addIdentifierGridDivs.length > 2) {
        for (var index = 2; index < addIdentifierGridDivs.length; index++) {
            if (addIdentifierGridDivs.eq(index).prop('id') == rowId) {
                return (parseInt(index) - 2);
                break;
            }
        }
    }
}

function ClearGridRows(gridId) {
    $("#" + gridId + " .deleteGridRowElemenet").each(function (index, element) {
        $(element).click();
    });
}

function onDeleteGridRowElemenetClick(e, hdnHeaderNameId, hdnHeaderValueId, gridId) {
    var HdnHeaderName = $('#' + hdnHeaderNameId);
    var HdnHeaderValue = $('#' + hdnHeaderValueId);

    var rowElement = $('#' + e.target.id).parent().parent();
    //Because it has icon
    var rowId = rowElement.prop('id');
    var index = parseInt(FindRowIndex(rowId, gridId));

    rowElement.remove();
    IdentifierGridFormatting(gridId);
    //availableHeightForTables();
}

//lblErrorId the error up in SM, txt: name & values, hdn: storing the current values

function InitiateBBGHeaderGrid(lblErrorId, modalErrorId, txtHeaderNameId, txtHeaderValueId, hdnHeaderNameId, hdnHeaderValueId, gridId, gridNewRowId, addRowButtonId) {
    console.log($('#' + addRowButtonId));
    $('#' + addRowButtonId).unbind('click').click(function (e) {
        onClickAddIdentifierButton(e, lblErrorId, modalErrorId, txtHeaderNameId, txtHeaderValueId, hdnHeaderNameId, hdnHeaderValueId, gridNewRowId, gridId);
    });
    PopulateBBGHeaderGrid(txtHeaderNameId, txtHeaderValueId, hdnHeaderNameId, hdnHeaderValueId, addRowButtonId, gridId);
}

function PopulateBBGHeaderGrid(txtHeaderNameId, txtHeaderValueId, hdnHeaderNameId, hdnHeaderValueId, addRowButtonId, gridId) {
    console.log(4);
    var TxtHeaderName = $('#' + txtHeaderNameId);
    var TxtHeaderValue = $('#' + txtHeaderValueId);
    var HdnHeaderName = $('#' + hdnHeaderNameId);
    var HdnHeaderValue = $('#' + hdnHeaderValueId);
    var AddButton = $('#' + addRowButtonId);

    var HeaderNamesVal = HdnHeaderName.val();
    var HeaderValuesVal = HdnHeaderValue.val();

    ClearGridRows(gridId);

    //Check which option is currently selected
    if (srmVmStatus._selectedDataSource == "Bloomberg") {

        //var selected = $("#srmVMDropDownMenu").find(":selected").text();
        var selected = smselect.getSelectedOption($("#smselect_srmVMRequestTypeDropDown"))[0].text;
        srmVmStatus.dropdownSelected = selected;
        //$("#srmVMDropDownMenu").find(":selected").text();


        var j = 0;
        for (var i in srmVmStatus.bloombergJsonData.Headers[srmVmStatus._selectedDataReqMethod]) {

            var item = srmVmStatus.bloombergJsonData.Headers[srmVmStatus._selectedDataReqMethod][i];
            if (selected == item.requestType) {
                j = i;
                break;
            }
        }

        srmVmStatus.manualAddTrigger = true;
        for (var i in srmVmStatus.bloombergJsonData.Headers[srmVmStatus._selectedDataReqMethod][j].value) {


            var item = srmVmStatus.bloombergJsonData.Headers[srmVmStatus._selectedDataReqMethod][j].value[i];
            TxtHeaderName.val(item.headerName);
            TxtHeaderValue.val(item.headerValue);
            AddButton.trigger('click');
            //if (TxtHeaderName.val() != "---Select One---")
            //    AddButton.trigger('click');
        }
        srmVmStatus.manualAddTrigger = false;
    }
    else if (srmVmStatus._selectedDataSource == "Reuters") {
        var selected = smselect.getSelectedOption($("#smselect_srmVMRequestTypeDropDown"))[0].text;
        srmVmStatus.dropdownSelected = selected;
        var j = 0;
        for (var i in srmVmStatus.reutersJsonData.Headers[srmVmStatus._selectedDataReqMethod]) {
            //jkdna

            var item = srmVmStatus.reutersJsonData.Headers[srmVmStatus._selectedDataReqMethod][i];
            if (selected == item.requestType) {
                j = i;
                break;
            }
        }

        srmVmStatus.manualAddTrigger = true;
        for (var i in srmVmStatus.reutersJsonData.Headers[srmVmStatus._selectedDataReqMethod][j].value) {
            //jkdna

            var item = srmVmStatus.reutersJsonData.Headers[srmVmStatus._selectedDataReqMethod][j].value[i];
            TxtHeaderName.val(item.headerName);
            TxtHeaderValue.val(item.headerValue);

            AddButton.trigger('click');

            //if (TxtHeaderName.val() != "---Select One---")
            //    AddButton.trigger('click');
        }
        srmVmStatus.manualAddTrigger = false;
    }


    //if (HeaderNamesVal.length > 0) {


    //    var headerNameList = HeaderNamesVal.split('ž');
    //    var headerValueList = HeaderValuesVal.split('ž');

    //    HdnHeaderName.val("");
    //    HdnHeaderValue.val("");

    //    for (var index = 0; index < headerNameList.length - 1; index++) {
    //        TxtHeaderName.val(headerNameList[index]);
    //        TxtHeaderValue.val(headerValueList[index]);

    //        if (TxtHeaderName.val() != "---Select One---")
    //            AddButton.trigger('click');
    //    }

    //}
}

//DropDown Change OLD
$("#srmVMRequestTypeDD").change(function () {
    //change
    srmVmStatus.saveState();
    srmVmStatus.dropdownSelected = $("#srmVMDropDownMenu").find(":selected").text();
    //
    InitiateBBGHeaderGrid("test1", "test2", "txtHeaderName", "txtHeaderValue", "hdnHeaderName", "hdnHeaderValue", "bbgHeaderGrid", "newGridRowDiv", "addRowButton");
});

function onClickAddIdentifierButton(e, lblErrorId, modalErrorId, txtHeaderNameId, txtHeaderValueId, hdnHeaderNameId, hdnHeaderValueId, gridNewRowId, gridId) {
    console.log(2);
    var LblError = $('#' + lblErrorId);
    var TxtHeaderName = $('#' + txtHeaderNameId);
    var TxtHeaderValue = $('#' + txtHeaderValueId);
    var HdnHeaderName = $('#' + hdnHeaderNameId);
    var HdnHeaderValue = $('#' + hdnHeaderValueId);
    var GridNewRow = $('#' + gridNewRowId);

    var HeaderName = TxtHeaderName.val();
    var HeaderValue = TxtHeaderValue.val();

    if (HeaderName.trim() === '' || HeaderName.length === 0) {

        OnWrongInput("Header Name can't be blank");
    }


        //    else if (TxtHeaderValue.val().trim() === '' || TxtHeaderValue.val().length === 0) {
        //        LblError.text('* Header Value cannot be blank');
        //        $find(modalErrorId).show();
        //    }
    else {
        var rowClass = 'SMNewGridNormalRow';
        var countRows = parseInt(GridNewRow.attr('countRows')) + 1;
        if (countRows % 2 === 0) {
            rowClass = 'SMNewGridNormalRow';
        }
        else {
            rowClass = 'SMNewGridAlternatingRow';
        }

        //Check if headername already exists in the headerlist
        var rows = $(".identifierGridRowsClass");
        var alreadyExists = false;
        for (var i = 0; i < rows.length; i++) {
            if (rows[i].children[0].innerText == HeaderName) {
                alreadyExists = true;
                if (!srmVmStatus.manualAddTrigger)
                    OnWrongInput("Header Name already exists");
                break;
            }
        }


        if (!alreadyExists) {
            TxtHeaderValue.val("");
            TxtHeaderName.val("");
            var str = '';

            str += '<div id=\"' + gridId + '_gridRow_' + GridNewRow.attr('countRows') + '\" class=\"identifierGridRowsClass ' + rowClass + '\"><div class=\"SMNewGridBodyElement\" contenteditable="false" style=\"text-indent: 6px;\">' + HeaderName + '</div><div class=\"SMNewGridBodyElement\" contenteditable="false" style=\"text-indent: 8px;\">' + HeaderValue + '</div><div class=\"SMNewGridBodyElement\" style=\"width: 2%; border: 0px;\"> <i class=\"fa fa-trash deleteGridRowElemenet\" id=\"' + gridId + '_deleteIdentifierButton_' + GridNewRow.attr('countRows') + '\"  style="width:100%;cursor:pointer"></i> </div>';
            //$(str).insertAfter(GridNewRow);
            $("#GridRowParent").append(str);
            GridNewRow.attr('countRows', countRows.toString());
            $('.deleteGridRowElemenet').unbind('click').click(function (e) {                            //??
                onDeleteGridRowElemenetClick(e, hdnHeaderNameId, hdnHeaderValueId, gridId);             //??

            });
            //Instead write to JSON

            //var strHeaderValues = new Sys.StringBuilder();
            //var strHeaderNames = new Sys.StringBuilder();
            //strHeaderValues.append(HdnHeaderValue.val());
            //strHeaderValues.append(HeaderValue);
            //strHeaderValues.append('ž');
            //strHeaderNames.append(HdnHeaderName.val());
            //strHeaderNames.append(HeaderName);
            //strHeaderNames.append('ž');
            //HdnHeaderValue.val(strHeaderValues.toString());
            //HdnHeaderName.val(strHeaderNames.toString());
            IdentifierGridFormatting(gridId);
            //availableHeightForTables();
        }

    }
    //$("GridRowParent").smslimscroll({ height: '500px', alwaysVisible: true, position: 'right', distance: '2px' });
}

var prefList = [];

//Populate Preferences
function populatePreferences(manuallySelectPrefWithoutAjaxCall) {


    $('#' + srmVmStatus.PreferencesDisplayId).html("");

    //Check if the list has been loaded in the global variable after the ajax call
    if (srmVmStatus.vendorPreferences) {
        var str = "";
        prefList.splice(0, prefList.length);
        for (i in srmVmStatus.vendorPreferences) {

            prefList.unshift({ name: srmVmStatus.vendorPreferences[i].name, id: srmVmStatus.vendorPreferences[i].id });
            //ID for default
            if (srmVmStatus.vendorPreferences[i].id == 1) {
                str += '<div class="PreferenceParent"><div class="PreferenceClass" id="prefId' + srmVmStatus.vendorPreferences[i].id + '">' + srmVmStatus.vendorPreferences[i].name + '</div></div>';
            }
            else {

                str += '<div class="PreferenceParent"><div class="PreferenceClass" id="prefId' + srmVmStatus.vendorPreferences[i].id + '">' + srmVmStatus.vendorPreferences[i].name + '</div><div class="removeButton"><i class="fa fa-trash"></i></div></div>';
            }
        }
        $('#' + srmVmStatus.PreferencesDisplayId).append(str);

    }

    $(".PreferenceClass").unbind("click").bind("click", function () {
        addButtonClicked = false;

        var curr = $(this).text();
        if (curr == "Default") {
            //MAKING IT UNEDITABLE
            $("#NamePreferenceInput").prop("readonly", true);
        }
        else {
            $("#NamePreferenceInput").prop("readonly", false);
        }
        //CSS change
        //Change class tabs
        $(this).parent().parent().find('.VMSelectedPreferencesTab').removeClass("VMSelectedPreferencesTab");
        $(this).parent().addClass("VMSelectedPreferencesTab");


        for (var i in prefList) {
            if (curr == prefList[i].name) {

                srmVmStatus.currPref = curr;
                srmVmStatus.currPrefID = prefList[i].id;

                var params = {};
                params.preferenceId = srmVmStatus.currPrefID;
                params.vendorId = 1;                            //Bloomberg
                CallCommonServiceMethod('GetVendorManagementData', params, onSuccess_PreferenceClick, OnFailure, null, false);
                srmVmStatus.setPreferenceName(curr);
                /*  SHIFTED TO ON SUCCESS LOGIC

                srmVmStatus.prefSelected = true;
                $(".srmVMdataSource").children()[0].click();
                srmVmStatus.setPreferenceName(curr);
                srmVmStatus.prefSelected = false;
                */

                //}
                break;
            }
        }
    });
    if (manuallySelectPrefWithoutAjaxCall) {
        var curr = $(".PreferenceClass").first().text();

        //Set text in the field
        srmVmStatus.setPreferenceName(curr);
        srmVmStatus.currPref = curr;


        //Handling default selected case
        if (curr == "Default") {
            //MAKING IT UNEDITABLE
            $("#NamePreferenceInput").prop("readonly", true);
        }
        else {
            $("#NamePreferenceInput").prop("readonly", false);
        }
        //VMSelectedPreferencesTab
        //Select tab
        //change text
        $(".PreferenceClass").first().parent().addClass('VMSelectedPreferencesTab');
    }

    $(".removeButton").unbind('click').bind("click", function () {
        var deletedParent = $(this).parent();



        //prefId ke saath this is returned, need to take out the ID from the string.
        var prefIdToBeDeleted = deletedParent.children()[0].id;
        prefIdToBeDeleted = prefIdToBeDeleted.replace("prefId", "");

        var param = {};
        param.inputData = {};
        param.inputData.VendorManagementId = prefIdToBeDeleted;
        param.inputData.UserName = _securityInfo.UserName;

        CallCommonServiceMethod('DeleteVendorManagementData', param, onSuccess_DeletingPref, OnFailure, null, false);


        //deletedParent.remove();

    });

}

function onSuccess_DeletingPref(result) {

    if (result.d.Status == true) {

        var deletedId = result.d.VendorManagementId;
        for (item in prefList) {
            if (prefList[item].id == deletedId) {
                showSuccessPopup("Deleted :" + prefList[item].name + " Successfully.");
            }
        }

        srmVmStatus.vendorPreferences = result.d.VendorPreferences;

        srmVmStatus.bloombergJsonData = result.d;
        $(".srmVMdataSource").children()[0].click();
        populatePreferences(true);
    }
    else {
        OnWrongInput(result.d.Message);
    }
}



//Save preferences
$('#' + srmVmStatus.savePreferenceId).unbind("click").bind("click", function () {
    var PrefText = $('#' + srmVmStatus.NamePreferenceId).val();

    if (PrefText.trim() === "") {
        OnWrongInput("Preference Name can't be blank");
        $('#' + srmVmStatus.NamePreferenceId).text("");
    }
    else {


        ////check if it already exists in the preference list, two cases either we are overriding a previous one, or making changes to the current selection
        var checkExistence = false;
        var prefCorrespondingPrefId = null;
        for (i in prefList) {
            if (PrefText == prefList[i].name) {
                prefCorrespondingPrefId = prefList[i].id;
                checkExistence = true;
            }
        }

        //Adding new 
        if (srmVmStatus.currPref != PrefText && checkExistence)
            OnWrongInput("This preference name already exists");

        else {
            //TO CHECK IF DEFAULT IS BEING EDITED


            //Save the current data in the local jsons
            srmVmStatus.saveState();
            //Renames
            if (PrefText == srmVmStatus.currPref)
                srmVmStatus.currPrefID = prefCorrespondingPrefId;
            srmVmStatus.currPref = PrefText;


            var param = {};
            param.inputData = {};
            if (!addButtonClicked)
                param.inputData.VendorManagementId = srmVmStatus.currPrefID;
            else
                param.inputData.VendorManagementId = 0;
            param.inputData.VendorManagementName = srmVmStatus.currPref;
            param.inputData.VendorManagementToCloneFrom;
            //Handles new addition or updating
            if (checkExistence) {
                //OnWrongInput(PrefText + " already exists in the preference list");
            }
            else {
                param.inputData.VendorManagementToCloneFromID = 1;
            }
            param.inputData.UserName = _securityInfo.UserName;

            var Configurations = [];
            var Headers = [];
            if (srmVmStatus.bloombergJsonData) {

                if (srmVmStatus.bloombergJsonData.Configurations) {
                    srmVmStatus.bloombergJsonData.Configurations.VendorId = 1;
                    Configurations.push(srmVmStatus.bloombergJsonData.Configurations);

                }
                if (srmVmStatus.bloombergJsonData.Headers) {
                    srmVmStatus.bloombergJsonData.Headers.VendorId = 1;
                    Headers.push(srmVmStatus.bloombergJsonData.Headers);
                }
            }
            if (srmVmStatus.reutersJsonData) {
                if (srmVmStatus.reutersJsonData.Configurations) {
                    srmVmStatus.reutersJsonData.Configurations.VendorId = 2;
                    Configurations.push(srmVmStatus.reutersJsonData.Configurations);
                }
                if (srmVmStatus.reutersJsonData.Headers) {
                    srmVmStatus.reutersJsonData.Headers.VendorId = 2;
                    Headers.push(srmVmStatus.reutersJsonData.Headers);
                }
            }
            if (srmVmStatus.markITJsonData) {
                if (srmVmStatus.markITJsonData.Configurations) {
                    srmVmStatus.markITJsonData.Configurations.VendorId = 3;
                    Configurations.push(srmVmStatus.markITJsonData.Configurations);
                }
                if (srmVmStatus.markITJsonData.Headers) {
                    srmVmStatus.markITJsonData.Headers.VendorId = 3;
                    Headers.push(srmVmStatus.markITJsonData.Headers);
                }
            }

            param.inputData.Configurations = Configurations;

            param.inputData.Headers = Headers;

            CallCommonServiceMethod('SaveVendorManagementData', param, onSuccess_SavingPref, OnFailure, null, false);

        }

        //}
    }
    //$(".removeButton").unbind('click').bind("click", function () {
    //    var deletedParent = $(this).parent();

    //    deletedParent.remove();

    //    //prefId ke saath this is returned, need to take out the ID from the string.
    //    var prefIdToBeDeleted = deletedParent.children()[0].id;
    //    prefIdToBeDeleted = prefIdToBeDeleted.replace("prefId", "");

    //    //AJAX Call.
    //    //Put repopulate in its on success and pass true to display the first entity



    //    showSuccessPopup(deletedParent.children()[0].innerHTML + " deleted successfully.");


    //});
});

function onSuccess_SavingPref(result) {

    // Status handles DB preference name repetition.
    addButtonClicked = false;
    if (result.d.Status == true) {
        srmVmStatus.vendorPreferences = result.d.VendorPreferences;
        showSuccessPopup("Preference Saved Successfully");
        populatePreferences(false);
        srmVmStatus.bloombergJsonData = result.d;
        var curr = srmVmStatus.currPref;

        //Handling default selected case
        if (curr == "Default") {
            //MAKING IT UNEDITABLE
            $("#NamePreferenceInput").prop("readonly", true);
        }
        else {
            $("#NamePreferenceInput").prop("readonly", false);
        }
        //VMSelectedPreferencesTab
        //Select tab
        //change text
        $("#prefId" + srmVmStatus.currPrefID).parent().addClass('VMSelectedPreferencesTab');
        srmVmStatus.reutersJsonData = null;
        srmVmStatus.markITJsonData = null;
        srmVmStatus.prefSelected = true;
        $(".srmVMdataSource").children()[0].click();
    }
    else {
        OnWrongInput(result.d.Message);
    }

}


//Add Preferences
$('#' + srmVmStatus.addNewPreferenceId).unbind().bind("click", function (e) {


    //Start Add Button Popup
    srmVmStatus.selectedAddClone.text = "Default";
    srmVmStatus.selectedAddClone.id = 1;

    //$("#AddButtonPopup").css({ display: 'block', height: '50px', width: '50px', top: '-50px', 'margin-bottom' : '50px'});
    //TO CHANGE

    var target = $(e.target);
    var targetId = target.prop('id')
    var panelTop = parseInt(target.offset().top + 1);
    var panelLeft = parseInt(target.offset().left - 2);

    $('#AddButtonPopup').css('top', panelTop);
    $('#AddButtonPopup').css('left', panelLeft);
    $('#AddButtonPopup').css('width', 210 + 'px');
    $('#AddButtonPopup').css('height', 100 + 'px');
    var zIndex = $('#AddButtonPopup').css('zIndex') - 1;
    $('#AddButtonPopup').css('display', '');
    //$('#divMoreCorpAction').outerHeight($('#dataMCA').outerHeight() + 5);

    var data = [];
    data.splice(0, data.length);

    for (i in prefList) {
        data.unshift({ text: prefList[i].name, value: prefList[i].id });
    }
    ApplySMSelectForAddButton('srmVMAddButton', false, data, false, '#SMSelectAddButtonContainer', 'none');

    var selected = smselect.getSelectedOption($("#smselect_srmVMAddButton"))[0];

    if (selected == null) {
        smselect.setOptionByValue($('#smselect_srmVMAddButton'), srmVmStatus.selectedAddClone.id.toString());
    }


    $("#AddButtonSavePreference").unbind('click').bind('click', function () {

        srmVmStatus.currPref = null;
        srmVmStatus.currPrefID = 0;
        srmVmStatus._selectedDataSource = null;

        //reset to editable incase default was previosly clicked
        $("#NamePreferenceInput").prop("readonly", false);
        srmVmStatus.setPreferenceName("");


        //Function TODO clone from
        //var cloneFrom = "Default";
        var cloneFrom = 1;

        var params = {};
        params.preferenceId = srmVmStatus.selectedAddClone.id;
        params.vendorId = 1;    //BB by default
        srmVmStatus.currPref = srmVmStatus.selectedAddClone.text;
        srmVmStatus.currPrefID = params.preferenceId;       //Selected to that the current data is loaded after add button
        addButtonClicked = true;
        $("#PreferencesList").find('.VMSelectedPreferencesTab').removeClass('VMSelectedPreferencesTab');
        CallCommonServiceMethod('GetVendorManagementData', params, onSuccess_VMPreferenceAdd, OnFailure, null, false);

        //Hide 
        $("#AddButtonPopup").hide();

    });

    //End Add Button Popup






});



function onSuccess_VMPreferenceAdd(result) {
    //showSuccessPopup("New Preference Added")
    srmVmStatus.bloombergJsonData = result.d;
    srmVmStatus.reutersJsonData = null;
    srmVmStatus.markITJsonData = null;
    //Hide the popup

    //Not clicked anymore
    srmVmStatus.selectedAddClone.text = null;
    srmVmStatus.selectedAddClone.id = 0;

    $(".srmVMdataSource").children()[0].click();
}


//set text value field of the preference
srmVmStatus.setPreferenceName = function (str) {
    $('#' + srmVmStatus.NamePreferenceId).val(str);
}

function showSuccessPopup(message) {
    $("#" + _controlIdInfo.LblSuccessId).text(message);
    $find(_controlIdInfo.ModalSuccessId).show();
}



//Applying SM Select
//function ApplySMSelect(id, isRunningText, data, isSearch, container, borderLeft) {
function ApplySMSelect(id, isRunningText, data, isSearch, container, borderLeft) {
    smselect.create(
    {
        id: id,
        container: $(container),
        isRunningText: isRunningText,
        data: data,
        showSearch: isSearch,
        ready: function (selectelement) {
            if (isRunningText) {
                selectelement.css({ width: '120px', 'text-align': 'center' });
                selectelement.find(".smselectanchorrun").css({ color: '#48a3dd', 'border-bottom': '1px solid rgb(0, 191, 242)', 'font-size': '15px' });
                selectelement.find(".smselectcon").css({ 'text-align': 'left' });
            }
            else
                selectelement.css({ 'border': '1px solid #CECECE', 'border-left': 'none', height: '22px', width: '180px' });

            selectelement.on('change', function (ee) {
                var selectedVal = smselect.getSelectedOption($(ee.currentTarget));

                //to check old data isn't populated on click of save change
                if (!srmVmStatus.prefSelected) {
                    srmVmStatus.saveState();
                }

                srmVmStatus.dropdownSelected = selectedVal[0].text;

                // $("#srmVMDropDownMenu").find(":selected").text();
                //
                InitiateBBGHeaderGrid("test1", "test2", "txtHeaderName", "txtHeaderValue", "hdnHeaderName", "hdnHeaderValue", "bbgHeaderGrid", "newGridRowDiv", "addRowButton");
            });
        }
    });
}


function ApplySMSelectForAddButton(id, isRunningText, data, isSearch, container, borderLeft) {
    smselect.create(
    {
        id: id,
        container: $(container),
        isRunningText: isRunningText,
        data: data,
        showSearch: isSearch,
        ready: function (selectelement) {
            if (isRunningText) {
                selectelement.css({ width: '120px', 'text-align': 'center' });
                selectelement.find(".smselectanchorrun").css({ color: '#48a3dd', 'border-bottom': '1px solid rgb(0, 191, 242)', 'font-size': '15px' });
                selectelement.find(".smselectcon").css({ 'text-align': 'left' });
            }
            else
                selectelement.css({ 'border': '1px solid #CECECE', height: '22px', width: '180px' });

            selectelement.on('change', function (ee) {
                var selectedVal = smselect.getSelectedOption($(ee.currentTarget));
                srmVmStatus.selectedAddClone.id = selectedVal[0].value;
                srmVmStatus.selectedAddClone.text = selectedVal[0].text;

            });
        }
    });
}

function OnFailure(result) {
    $("#" + _controlIdInfo.LblErrorId).text("Service returned an error" + ((result.responseJSON == null || result.responseJSON.Message == null || result.responseJSON.Message.length == 0) ? "" : (" : " + result.responseJSON.Message)) + ((result.responseText == null || result.responseText.length == 0) ? "" : (" : " + result.responseText)));
    $find(_controlIdInfo.ModalErrorId).show();

}

function OnWrongInput(message) {
    $("#" + _controlIdInfo.LblErrorId).text(message);
    $find(_controlIdInfo.ModalErrorId).show();

}


function onSuccess_PreferenceClick(result) {
    srmVmStatus.bloombergJsonData = result.d;
    srmVmStatus.prefSelected = true;            //For local save check


    $(".srmVMdataSource").children()[0].click();

    srmVmStatus.prefSelected = false;
    srmVmStatus.reutersJsonData = null;
    srmVmStatus.markITJsonData = null;
    //$(".srmVMdataSource").children()[0].click();
}


function onSuccess_VMData(result) {
    srmVmStatus.bloombergJsonData = result.d;
    srmVmStatus.vendorPreferences = result.d.VendorPreferences;

    //var curr = $(".PreferenceClass").first().text();

    ////Set text in the field
    //srmVmStatus.setPreferenceName(curr);



    ////Handling default selected case
    //if (curr == "Default") {
    //    //MAKING IT UNEDITABLE
    //    $("#NamePreferenceInput").prop("readonly", true);
    //}
    //else {
    //    $("#NamePreferenceInput").prop("readonly", false);
    //}
    ////VMSelectedPreferencesTab
    ////Select tab
    ////change text
    //$(".PreferenceClass").first().addClass('VMSelectedPreferencesTab');

    //clicks bloomberg
    $(".srmVMdataSource").children()[0].click();
    populatePreferences(true);

}

function SMSSRMVendorManagement(controlInfo, securityInfo) {
    $(document).ready(function () {
        _controlIdInfo = eval("(" + controlInfo + ")");
        _securityInfo = eval("(" + securityInfo + ")");

        var params = {};
        params.preferenceId = 0;
        params.vendorId = 1;
        CallCommonServiceMethod('GetVendorManagementData', params, onSuccess_VMData, OnFailure, null, false);

        resizePreferenceList();
        //applySlimScroll("#FixedDivForSlimScrollVMHeaders", "#GridRowParent");
        //Binds clicking outside the popup
        $(document).unbind('click', OnDocumentClick).click(OnDocumentClick);


    });
}

function OnDocumentClick(e) {
    var target = $(e.target);
    var targetId = target.prop('id');
    if (targetId != 'AddNewPreference' && !target.hasClass("AddButtonPopup") && target.closest(".AddButtonPopup").length == 0)
        $('#AddButtonPopup').css('display', 'none');
}

function resizePreferenceList() {
    var listHeight = parseInt($(window).height() - $('#PreferencesList').offset().top - 25);
    console.log(listHeight);
    $('#PreferencesList').height(listHeight);

    //var gridHeight = parseInt($(window).height() - $("#GridRowParent").offset().top - 20);
    //$('#GridRowParent').height(gridHeight);
}

function availableHeightForTables() {
    return;
    //var heightBetweenHeaderAndPrefName = $("#NamePreferenceInput").offset().top - ($(".srmVMHeaderFilters").offset().top + $(".srmVMHeaderFilters").height());
    //var heightPrefNameAndFirstHeading = $(".HeadingStyle").first().offset().top - ($("#NamePreferenceInput").offset().top + $("#NamePreferenceInput").height());
    //var heightFirstHeadingAndFixedTable = $("#srmVMFixedConfigTable").offset().top -($(".HeadingStyle").first().offset().top + $(".HeadingStyle").first().height());
    //var heightAboveFixedTableElements = $(window).height() - $(".srmVMHeaderFilters").height() - $("#NamePreferenceInput").height() - $(".HeadingStyle").height();

    var heightAboveFixedTable = $(window).height() - $("#srmVMFixedConfigTable").offset().top - $(".TableHeader").height();
    var bottomHeight = $(".HeadingStyle").last().height() + $("#srmVMRequestType").height() + ($("#bbgHeaderGrid").offset().top - ($("#srmVMRequestType").offset().top + $("#srmVMRequestType").height())) + $(".SMNewGridHeaderRow").height();
    var availableSpace = parseInt(heightAboveFixedTable - bottomHeight - 90);
    var heightOfSecondTable = $("#GridRowParent").children().length * $("#GridRowParent").children().first().height();
    var heightOfFirstTable = $("#FixedTableRows").children().length * $("#FixedTableRows").children().first().height() + 10;
    if (heightOfFirstTable + heightOfSecondTable > availableSpace) {
        if (heightOfFirstTable < availableSpace / 2) {
            $("#FixedDivForSlimScrollVMHeaders").height(availableSpace - heightOfFirstTable);
            $("#SlimScrollFixedTableParent").height(heightOfFirstTable);
        }
        else if (heightOfSecondTable < availableSpace / 2) {
            $("#SlimScrollFixedTableParent").height(availableSpace - heightOfSecondTable);
            $("#FixedDivForSlimScrollVMHeaders").height(heightOfSecondTable);
        }
        else {
            $("#FixedDivForSlimScrollVMHeaders").height(availableSpace / 2);
            $("#SlimScrollFixedTableParent").height(availableSpace / 2);
        }
        //applySlimScroll("#FixedDivForSlimScrollVMHeaders", "#GridRowParent");
        //applySlimScroll("#SlimScrollFixedTableParent", "#FixedTableRows");

    }
    else {
        //$("#FixedDivForSlimScrollVMHeaders").smslimscroll({ destroy: true });
        //$("#FixedDivForSlimScrollVMHeaders").height(heightOfSecondTable);
        //$("#SlimScrollFixedTableParent").smslimscroll({ destroy: true });
        //$("#SlimScrollFixedTableParent").height(heightOfFirstTable);

    }
}

$(window).resize(function () {

    var selected = smselect.getSelectedOption($("#smselect_srmVMRequestTypeDropDown"))[0];


    smselect.setOptionByText($('#smselect_srmVMRequestTypeDropDown'), selected.toString());
    //availableHeightForTables();
    resizePreferenceList();
})