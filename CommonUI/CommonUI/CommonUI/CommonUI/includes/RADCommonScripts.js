/***************************************************************************************************
ViewManager Scripts
***************************************************************************************************/
function RAD_SetView(identifier) {
    document.getElementById('identifier').value = identifier;
}

function RAD_SetViewWithID(identifier, id) {
    document.getElementById('identifier').value = identifier;
    document.getElementById('id').value = id;
}

function RAD_SetViewWithViewID(identifier, id, viewId) {
    document.getElementById('identifier').value = identifier;
    document.getElementById('id').value = id;
    document.getElementById('ViewId').value = viewId;
}
function RAD_SetViewWithControlID(identifier, id) {
    document.getElementById('identifier').value = identifier;
    var dropDown = document.getElementById(id);
    document.getElementById('id').value = dropDown.value;
    var selectedIndex = dropDown.selectedIndex;
    document.getElementById('secondaryID').value = dropDown.options[selectedIndex].text;
}

function RAD_SetViewWithIDAndSecondaryID(identifier, id, secondaryID) {
    document.getElementById('identifier').value = identifier;
    document.getElementById('id').value = id;
    document.getElementById('secondaryID').value = secondaryID;
}

function RAD_SetViewWithIDSecondaryIDAndViewID(identifier, id, secondaryID, viewId) {
    document.getElementById('identifier').value = identifier;
    document.getElementById('id').value = id;
    document.getElementById('secondaryID').value = secondaryID;
    document.getElementById('ViewId').value = viewId;
}

function RAD_ClearFields() {
    try {
        document.getElementById('identifier').value = '';
        document.getElementById('masterId').value = '';
        document.getElementById('primaryName').value = '';
        document.getElementById('secondaryName').value = '';
        document.getElementById('id').value = '';
        document.getElementById('commandName').value = '';
        document.getElementById('secondaryID').value = '';
    } catch (err) { }
}

/***************************************************************************************************
Update Panel Scripts
***************************************************************************************************/
function RAD_onUpdating() {
    $get('disableDiv').style.display = '';
    $get('loadingImg').style.display = '';
}

function RAD_onUpdated() {
    $get('disableDiv').style.display = 'none';
    $get('loadingImg').style.display = 'none';
    document.getElementById('identifier').value = "";
}
function RAD_onUpdatedLeft() {
    $get('disableDiv').style.display = 'none';
    $get('loadingImg').style.display = 'none';
    RAD_ClearFields();
}

/***************************************************************************************************
Disable Select Scripts
***************************************************************************************************/
function RAD_disabletext(e) {
    return false
}

function RAD_reEnable() {
    return true
}

//if the browser is IE4+
//document.onselectstart = new Function("return false")

//if the browser is NS6
if (window.sidebar) {
    document.onmousedown = disableselect
    document.onclick = reEnable
}

/***************************************************************************************************
Grid Scripts
***************************************************************************************************/
//function for RExtGridView
function RAD_TglRow(ctl) {
    var row = ctl.parentNode.parentNode;
    var tbl = row.parentNode;
    var crow = tbl.rows[row.rowIndex + 1];
    var ihExp = ctl.parentNode.getElementsByTagName('input').item(0);

    tbl = tbl.parentNode;

    var expandClass = tbl.attributes.getNamedItem('expandClass').value;
    var collapseClass = tbl.attributes.getNamedItem('collapseClass').value;
    var expandText = tbl.attributes.getNamedItem('expandText').value;
    var collapseText = tbl.attributes.getNamedItem('collapseText').value;


    if (crow.style.display == 'none') {
        crow.style.display = '';
        ctl.innerHTML = collapseText;
        ctl.className = collapseClass;
        ctl.title = "Click to collapse";
        ihExp.value = '1';
    }
    else {
        crow.style.display = 'none';
        ctl.innerHTML = expandText;
        ctl.className = expandClass;
        ctl.title = "Click to expand.";
        ihExp.value = '';
    }
}
function RAD_ToggleRow(expandCollapseCell) {
    var tr = expandCollapseCell.parentElement;
    if (expandCollapseCell.className == "expandGroupButton") {
        expandCollapseCell.className = "collapseGroupButton";
        tr.nextSibling.style.display = "";
    }
    else {
        expandCollapseCell.className = "expandGroupButton";
        tr.nextSibling.style.display = "none";
    }
}
function RAD_SetGridCommand(commandName) {
    document.getElementById('commandName').value = commandName;
}

function RAD_CheckNumerics(gridID) {
    var grid = document.getElementById(gridID);

    var textBoxes = grid.getElementsByTagName('input');
    var reqTextBoxes = [];
    var index = 0;
    for (var i = 0; i < textBoxes.length; i++) {
        if (textBoxes[i].type == 'text') {
            reqTextBoxes[index] = textBoxes[i];
            index++;
        }
    }
    var txtPageIndex = reqTextBoxes[0];
    var txtPageSize = reqTextBoxes[1];

    if (event.keyCode < 48 || (event.keyCode > 57 && event.keyCode < 96) || event.keyCode > 105) {
        if (event.srcElement.id == txtPageIndex.id) {
            txtPageIndex.value = '';
        }
        else {
            txtPageSize.value = '';
        }
    }
}

/***************************************************************************************************
Calendar Scripts
***************************************************************************************************/
function RAD_checkDate(txtDateId, lblDateId) {
    var regExp = /^(0[1-9]?|1[012]?)\/(0[1-9]?|[12][0-9]?|3[01]?)\/(19?|20?)\d\d$/;
    var flag = true;

    var sDateArray = new Array(3);

    if ($get(txtDateId).value.length == 0) {
        errorMsg = "Date Required";
        flag = false;
    }
    else {
        if (!regExp.test($get(txtDateId).value)) {
            errorMsg = "Incorrect Date";
            flag = false;
        }
    }

    sDateArray = $get(txtDateId).value.split("/");
    iMonth = parseInt(sDateArray[0]);
    iDay = parseInt(sDateArray[1]);
    iYear = parseInt(sDateArray[2]);

    if (iMonth == 2) {
        if (LeapYear(iYear)) {
            if (iDay > 29) {
                flag = false;
                errorMsg = "Incorrect Date";
            }
        }
        else {
            if (iDay > 28) {
                flag = false;
                errorMsg = "Incorrect Date";
            }
        }
    }


    if (!flag) {
        $get(lblDateId).style.color = 'red';
        $get(lblDateId).innerHTML = errorMsg;
        return false;
    }
    return true;
}
function RAD_IsDateValid(sender, args) {
    var iDay;
    var iMonth;
    var iYear;

    var sDateArray = new Array(3);

    // parse into 3 elements
    sDateArray = args.Value.split("/");
    iMonth = sDateArray[0];
    iDay = (sDateArray[1]);
    iYear = (sDateArray[2]);

    if (iMonth == 4 || iMonth == 6 || iMonth == 9 || iMonth == 11) {
        if (iDay > 30) {
            args.IsValid = false;
            return;
        }
    }
    if (iMonth == 1 || iMonth == 3 || iMonth == 5 || iMonth == 7 || iMonth == 8 || iMonth == 10 || iMonth == 12) {
        if (iDay > 31) {
            args.IsValid = false;
            return;
        }
    }

    // handle February and Leap year
    if (iMonth == 2) {
        if (LeapYear(iYear)) {
            if (iDay > 29) {
                args.IsValid = false;
                return;
            }
        }
        else {
            if (iDay > 28) {
                args.IsValid = false;
                return;
            }
        }
    }
    else {
        args.IsValid = true;
    }
}
/***************************************************************************************************
IFrame Scripts
***************************************************************************************************/
function RAD_iUploadFrameLoad(btnUploadClientId) {
    if (window.iUploadFrame.document.body.innerHTML.indexOf('Success') != -1) {
        document.getElementById(btnUploadClientId).disabled = false;
    }
    else
        document.getElementById(btnUploadClientId).disabled = true;
}
function RAD_iUploadMatrixFrameLoad(btnUploadClientId) {
    if (window.iUploadMatrixFrame.document.body.innerHTML.indexOf('Success') != -1) {
        document.getElementById(btnUploadClientId).disabled = false;
    }
    else
        document.getElementById(btnUploadClientId).disabled = true;
}

/***************************************************************************************************
RWUCPbAdapter Scripts
***************************************************************************************************/
function RAD_toggleBtnPBFund(btnAddNewPBId, txtPBNameId, ddlPBNameId) {
    if (document.getElementById(btnAddNewPBId).value == "Add New PB/Fund") {
        document.getElementById(ddlPBNameId).style.display = 'none';
        document.getElementById(txtPBNameId).style.display = '';
        document.getElementById(btnAddNewPBId).value = "Select PB/Fund";
    }
    else if (document.getElementById(btnAddNewPBId).value == "Select PB/Fund") {
        document.getElementById(ddlPBNameId).style.display = '';
        document.getElementById(txtPBNameId).style.display = 'none';
        document.getElementById(btnAddNewPBId).value = "Add New PB/Fund";
    }
    return false;
}

function RAD_addDDLPB(ddlPBNameId, txtPBNameId) {
    if (document.getElementById(ddlPBNameId).value == "--Select One--" || document.getElementById(ddlPBNameId).value == "-1")
        document.getElementById(txtPBNameId).value = "";
    else
        document.getElementById(txtPBNameId).value = document.getElementById(ddlPBNameId).value;
    return false;
}

/***************************************************************************************************
RWUCSchedulableJobmanager Scripts
***************************************************************************************************/
/* Check for enteris in No. of retries and time interval of reteries */
function RAD_checkText(txtNoOfRetriesId, txtTimeIntervalId, lblNoOfRetriesId, lblTimeIntervalId) {
    if (document.getElementById(txtNoOfRetriesId).value != '') {
        if (document.getElementById(txtNoOfRetriesId).value == '0') {
            if (document.getElementById(txtTimeIntervalId).value > '0') {
                document.getElementById(lblTimeIntervalId).innerHTML = "Value can't be greater than 0";
                document.getElementById(lblNoOfRetriesId).innerHTML = "";
                return false;
            }
        }
        else if (document.getElementById(txtTimeIntervalId).value == '') {
            document.getElementById(lblTimeIntervalId).innerHTML = "Please enter the value";
            document.getElementById(lblNoOfRetriesId).innerHTML = "";
            return false;
        }

    }
    if (document.getElementById(txtTimeIntervalId).value != '') {
        if (document.getElementById(txtTimeIntervalId).value == '0') {
            if (document.getElementById(txtNoOfRetriesId).value > '0') {
                document.getElementById(lblNoOfRetriesId).innerHTML = "Value can't be greater than 0";
                document.getElementById(lblTimeIntervalId).innerHTML = "";
                return false;
            }
        }
        else if (document.getElementById(txtNoOfRetriesId).value == '') {
            document.getElementById(lblNoOfRetriesId).innerHTML = "Please enter the value";
            document.getElementById(lblTimeIntervalId).innerHTML = "";
            return false;
        }
    }
    return true;
}

/***************************************************************************************************
RWUCScheduledJobManager Scripts
***************************************************************************************************/
function RAD_validateScheduling(txtNoOfRecurrencesId, txtTimeIntervalId, lblValidNoOfRecurrencesId, txtStartTimeId) {
    var hr;
    var min;
    var sec;
    var duration;
    if (document.getElementById(txtNoOfRecurrencesId) != null) {
        var noOfRecurrences = document.getElementById(txtNoOfRecurrencesId).value;
        var timeInterval = document.getElementById(txtTimeIntervalId).value;
        var startTimeArray = document.getElementById(txtStartTimeId).value.split(":");
        var validNoOfRecurrences;
        var noOfMins;

        if (!(noOfRecurrences == "" || timeInterval == "" || startTimeArray == "")) {
            hr = startTimeArray[0];
            min = startTimeArray[1];
            sec = startTimeArray[2].split(" ");
            duration = sec[1];

            switch (duration) {
                case "AM":
                    noOfMins = (12 - hr) * 60 + 12 * 60 - min;
                    break;
                case "PM":
                    noOfMins = (12 - hr) * 60 - min;
                    break;
            }

            validNoOfRecurrences = (noOfMins) / timeInterval;
            if ((noOfMins) % timeInterval > 0)
                validNoOfRecurrences += 1;

            if (noOfRecurrences <= validNoOfRecurrences) {
                document.getElementById(lblValidNoOfRecurrencesId).innerHTML = "";
                return true;
            }
            else {
                document.getElementById(lblValidNoOfRecurrencesId).innerHTML = "* In 1 day, No of Recurrences of Job is not possible according to given time interval value and start time value";
                return false;
            }
        }
    }
    return true;
}
/***************************************************************************************************
Common Scripts
***************************************************************************************************/

function RAD_MaxLength(id, length) {
    var txt = document.getElementById(id);

    if (txt.value.length > length) {
        txt.value = txt.value.substring(0, length);
    }
}
