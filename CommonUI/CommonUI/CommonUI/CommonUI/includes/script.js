function SaveProfileSecMaster() {
    var selectedLanguage = $('#tdLanguage').children('select:first').val();
    var selectedLandingPage = $('#tdLandingPage').children('select:first').val();
    var radAPI = new com.ivp.rad.rradapi.RRadAPI();
    onUpdating();
    radAPI.SaveUserProfileNew('Aqua', selectedLanguage, selectedLandingPage, Function.createDelegate(null, onSuccessSaveProfile), Function.createDelegate(null, onFailureSaveProfile));
    //    var hiddenThemeName = window.document.getElementById('themeName');
    //    hiddenThemeName.value = com.ivp.rad.rscriptutils.RSProfileManager._themeName;
    //    var iDashFrame = document.getElementById('iDashboardFrame');
    //    if (iDashFrame != null) {
    //        var loc = iDashFrame.contentWindow.location;
    //        eval('loc.reload();');
    //        iDashFrame.style.display = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    //    }
    //    var iReportViewer = document.getElementById('iFrameMainReportViewerContainer');
    //    if (iReportViewer != null) {
    //        var btnThemePostBack = document.getElementById('btnThemePostBack');
    //        btnThemePostBack.click();
    //    }
}
function onSuccessSaveProfile(stringResult, eventArgs) {
    var selectedLanguage = $('#tdLanguage').children('select:first').val();
    var oldLanguage = $('#cultureName').val();
    if (oldLanguage !== selectedLanguage) {
        window.location.href = getDefaultLoginUrl();
    }
    else
        onUpdated();
}

function onFailureSaveProfile(args) {
    var ex = args;
    alert('WebService Error: Code :' + ex.get_statusCode() + ' : ' + ex.get_message());
}

function GetGUID() {
    var objDate = new Date();
    var str = objDate.toString();
    str = objDate.getDate().toString() + objDate.getMonth().toString() + objDate.getFullYear().toString() + objDate.getHours().toString() + objDate.getMinutes().toString() + objDate.getSeconds().toString() + objDate.getMilliseconds().toString() + eval('Math.round(Math.random() * 10090)').toString();
    return str;
}

/* Check Uncheck all Checkboxes */
function CheckAll() {
    count = document.frm.elements.length;
    for (i = 0; i < count; i++) {
        if (document.frm.elements[i].checked == 0) {
            document.frm.elements[i].checked = 1;
        }
    }
}

function disableShuttleControls(ddlindex, shuttle) {
    var i = document.getElementById(ddlindex).selectedIndex;
    var itemstring = document.getElementById(ddlindex).options.item(i).innerText;

    if (itemstring == "DEFAULT") {
        document.getElementById('ctl00_cphMain_CreateWorkFlow_SecTypeShuttle_btnRemove').disabled = true;
        document.getElementById('ctl00_cphMain_CreateWorkFlow_SecTypeShuttle_btnRemoveAll').disabled = true;
        document.getElementById('ctl00_cphMain_CreateWorkFlow_SecTypeShuttle_btnMove').disabled = true;
        document.getElementById('ctl00_cphMain_CreateWorkFlow_SecTypeShuttle_btnMoveAll').disabled = true;

        //document.getElementById('ctl00_cphMain_CreateWorkFlow_SecTypeShuttle_btnRemove')).di;

        // var btnRemove = document.getElementById('ctl00_cphMain_ReportingSetupConfigure_SecTypeShuttle_btnRemove');
        //      var btnRemoveAll = document.getElementById('ctl00_cphMain_ReportingSetupConfigure_RShuttle1_btnRemoveAll')
        //document.getElementById(shuttle).children[0].cells[1].disabled=true;
        // document.getElementById(shuttle).children[1].disabled=true;
        // document.getElementById(shuttle).children[2].disabled=true;

    }

}

function UncheckAll() {
    count = document.frm.elements.length;
    for (i = 0; i < count; i++) {
        if (document.frm.elements[i].checked == 1)
        { document.frm.elements[i].checked = 0; }

    }
}

/* Show Hide Panel */
function hidePane() {
    document.getElementById('tdLeft').style.display = 'none';
    document.getElementById('collapse').style.display = 'none';
    document.getElementById('expand').style.display = '';
}
function showPane() {
    document.getElementById('tdLeft').style.display = '';
    document.getElementById('collapse').style.display = '';
    document.getElementById('expand').style.display = 'none';
}

/* Show Hide Panel */
function showText(tdToShow, aShowText, aHideText) {
    document.getElementById(tdToShow).style.display = '';
    document.getElementById(aShowText).style.display = 'none';
    document.getElementById(aHideText).style.display = '';
}
function hideText(tdToHide, aShowText, aHideText) {
    document.getElementById(aShowText).style.display = '';
    document.getElementById(tdToHide).style.display = 'none';
    document.getElementById(aHideText).style.display = 'none';
}

/* Show Hide Header*/
function showHeader() {
    document.getElementById('header').style.display = '';
    $('[id$=_linkShowHead]').css('display', 'none');
    $('[id$=_linkHideHead]').css('display', '');
    document.getElementById('divTopMenuMain').style.top = '55px';
    $get('idtopMenuTools').style.top = '55px';
    $get('idtopMenuToolsDiv').style.top = '75px';
}
function hideHeader() {
    document.getElementById('header').style.display = 'none';
    $('[id$=_linkShowHead]').css('display', '');
    $('[id$=_linkHideHead]').css('display', 'none');
    document.getElementById('divTopMenuMain').style.top = '1px';
    $get('idtopMenuTools').style.top = '2px';
    $get('idtopMenuToolsDiv').style.top = '20px';
}
function showHeaderLite() {
    document.getElementById('header').style.display = '';
    if (document.getElementById('ctl00_linkShowHead') != null)
        document.getElementById('ctl00_linkShowHead').style.display = 'none';
    if (document.getElementById('linkShowHead') != null)
        document.getElementById('linkShowHead').style.display = 'none';
    if (document.getElementById('ctl00_linkHideHead') != null)
        document.getElementById('ctl00_linkHideHead').style.display = '';
    if (document.getElementById('linkHideHead') != null)
        document.getElementById('linkHideHead').style.display = '';
    document.getElementById('divTopMenuMain').style.top = '55px';
    $get('idtopMenuTools').style.top = '55px';
    $get('idtopMenuToolsDiv').style.top = '75px';
}
function hideHeaderLite() {
    document.getElementById('header').style.display = 'none';
    if (document.getElementById('linkShowHead') != null)
        document.getElementById('linkShowHead').style.display = '';
    if (document.getElementById('ctl00_linkShowHead') != null)
        document.getElementById('ctl00_linkShowHead').style.display = '';
    if (document.getElementById('linkHideHead') != null)
        document.getElementById('linkHideHead').style.display = 'none';
    if (document.getElementById('ctl00_linkHideHead') != null)
        document.getElementById('ctl00_linkHideHead').style.display = 'none';
    document.getElementById('divTopMenuMain').style.top = '1px';
    $get('idtopMenuTools').style.top = '2px';
    $get('idtopMenuToolsDiv').style.top = '20px';
}
/* Show Hide Header*/
/* show hide Theme popup */
function showThemePopup() {
    //Added to handle theme popup in mgmt dashboard
    var iDashFrame = $get('iDashboardFrame');
    if (iDashFrame != null) {
        iDashFrame.contentWindow.document.getElementById('slvControl').style.display = "none";
    }
    var scrollHeight = document.body.scrollHeight;
    var scrollWidth = document.body.scrollWidth;
    $get('disableDiv').style.height = scrollHeight + "px";
    $get('disableDiv').style.width = scrollWidth + "px";
    document.getElementById('themePopup').style.display = '';
    document.getElementById('disableDiv').style.display = '';
    var language = $get('tdLanguage').children[0].value;
    $get('cultureName').value = language;
}
function cancelThemePopup() {
    document.getElementById('themePopup').style.display = 'none';
    document.getElementById('disableDiv').style.display = 'none';
    //Added to handle theme popup in mgmt dashboard
    var iDashFrame = $get('iDashboardFrame');
    if (iDashFrame != null) {
        iDashFrame.contentWindow.document.getElementById('slvControl').style.display = "";
    }
    var themeName = $get('themeName').value.trim();
    if (themeName != '')
        com.ivp.rad.rscriptutils.RSProfileManager.changeTheme(null, themeName);
}
function hideThemePopup() {
    document.getElementById('themePopup').style.display = 'none';
    //    if ($get('disableDiv') != null)
    //        document.getElementById('disableDiv').style.display = 'none';
}
function applyTheme(themeButton, themeName) {
    com.ivp.rad.rscriptutils.RSProfileManager.changeTheme(themeButton, themeName);
    return false;
}
function SaveProfileSettings(themeButton, themeName) {
    com.ivp.rad.rscriptutils.RSProfileManager.saveProfile();
    ClearFields();
    hideThemePopup();
    return false;
}


/* show hide Theme popup */

/*--  hiding the get sum popup  --*/
function closepopup(id) {
    if (id != undefined)
        document.getElementById(id).style.visibility = 'hidden';
}

//function blockError() { return true; } window.onerror = blockError;

function showThumb(obj) {
    id = obj.id;

    if (id == 'ctl00_cphTop_ctl00_rdAqua') {
        document.getElementById('divRdUnselected').style.display = 'none';
        document.getElementById('ctl00_cphTop_ctl00_AquaThumb').style.display = '';
        document.getElementById('ctl00_cphTop_ctl00_SunsetThumb').style.display = 'none';
        document.getElementById('ctl00_cphTop_ctl00_sunsetBlueThumb').style.display = 'none';
        document.getElementById('ctl00_cphTop_ctl00_greyThemeThumb').style.display = 'none';
        document.getElementById('ctl00_cphTop_ctl00_chocThemeThumb').style.display = 'none';
    }
    else if (id == 'ctl00_cphTop_ctl00_rdSunset') {
        document.getElementById('divRdUnselected').style.display = 'none';
        document.getElementById('ctl00_cphTop_ctl00_chocThemeThumb').style.display = '';
        document.getElementById('ctl00_cphTop_ctl00_blueThemeThumb').style.display = 'none';
        document.getElementById('ctl00_cphTop_ctl00_woodyThumb').style.display = 'none';
        document.getElementById('ctl00_cphTop_ctl00_sunsetBlueThumb').style.display = 'none';
        document.getElementById('ctl00_cphTop_ctl00_greyThemeThumb').style.display = 'none';
    }
    //    else if (id == 'ctl00_cphTop_ctl00_rdGrey') {
    //        document.getElementById('divRdUnselected').style.display = 'none';
    //        document.getElementById('ctl00_cphTop_ctl00_blueThemeThumb').style.display = 'none';
    //        document.getElementById('ctl00_cphTop_ctl00_woodyThumb').style.display = 'none';
    //        document.getElementById('ctl00_cphTop_ctl00_sunsetBlueThumb').style.display = 'none';
    //        document.getElementById('ctl00_cphTop_ctl00_greyThemeThumb').style.display = '';
    //        document.getElementById('ctl00_cphTop_ctl00_chocThemeThumb').style.display = 'none';
    //    }
    //    else if (id == 'ctl00_cphTop_ctl00_rdSunsetBlue') {
    //        document.getElementById('divRdUnselected').style.display = 'none';
    //        document.getElementById('ctl00_cphTop_ctl00_blueThemeThumb').style.display = 'none';
    //        document.getElementById('ctl00_cphTop_ctl00_woodyThumb').style.display = 'none';
    //        document.getElementById('ctl00_cphTop_ctl00_sunsetBlueThumb').style.display = '';
    //        document.getElementById('ctl00_cphTop_ctl00_greyThemeThumb').style.display = 'none';
    //        document.getElementById('ctl00_cphTop_ctl00_chocThemeThumb').style.display = 'none';
    //    }
    //    else if (id == 'ctl00_cphTop_ctl00_rdWoody') {
    //        document.getElementById('divRdUnselected').style.display = 'none';
    //        document.getElementById('ctl00_cphTop_ctl00_blueThemeThumb').style.display = 'none';
    //        document.getElementById('ctl00_cphTop_ctl00_woodyThumb').style.display = '';
    //        document.getElementById('ctl00_cphTop_ctl00_sunsetBlueThumb').style.display = 'none';
    //        document.getElementById('ctl00_cphTop_ctl00_greyThemeThumb').style.display = 'none';
    //        document.getElementById('ctl00_cphTop_ctl00_chocThemeThumb').style.display = 'none';
    //    }
    //    else {
    //        document.getElementById('divRdUnselected').style.display = '';
    //        document.getElementById('ctl00_cphTop_ctl00_blueThemeThumb').style.display = 'none';
    //        document.getElementById('ctl00_cphTop_ctl00_woodyThumb').style.display = 'none';
    //        document.getElementById('ctl00_cphTop_ctl00_sunsetBlueThumb').style.display = 'none';
    //        document.getElementById('ctl00_cphTop_ctl00_greyThemeThumb').style.display = 'none';
    //        document.getElementById('ctl00_cphTop_ctl00_chocThemeThumb').style.display = 'none';
    //    }
}
function hideAllThumb() {
    document.getElementById('ctl00_cphTop_ctl00_AquaThumb').style.display = 'none';
    document.getElementById('ctl00_cphTop_ctl00_SunsetThumb').style.display = 'none';
}


function MaxLength(id, length) {
    var txt = document.getElementById(id);

    if (txt.value.length > length) {
        txt.value = txt.value.substring(0, length);
    }
}

/*====  Script to toggle between Menu and Reports Starts here ====*/
function toggleMenu(toggleItem, divToShow, tbToShow, divToHide, tbToHide) {
    if (toggleItem == 'menu') {
        document.getElementById(tbToShow).style.display = '';
        document.getElementById(tbToHide).style.display = 'none';
        document.getElementById(divToShow).className = 'menuTabActive';
        document.getElementById(divToHide).className = 'menuTabDisabled';
    }
    else if (toggleItem == 'report') {
        document.getElementById(tbToHide).style.display = 'none';
        document.getElementById(tbToShow).style.display = '';
        document.getElementById(divToHide).className = 'menuTabDisabledChanged';
        document.getElementById(divToShow).className = 'menuTabActiveChanged';
    }

}

function addRow(id, text, text2, colIndex) {
    var iButton;
    var iButtonNew;
    var length;
    var flag;
    var a = document.getElementById(id);
    a.insertRow(a.rows.length);
    var i = a.rows.length - 1;
    var j = a.children[0].children[i];
    length = text.split('|').length;
    j.insertCell(0);
    j.children[0].innerHTML = '[';
    for (i = 0; i < length; i++) {
        j.insertCell(i + 1);
        if (document.getElementById(text.split('|')[i]).length > 1) {
            j.children[i + 1].innerHTML = document.getElementById(text.split('|')[i]).options[document.getElementById(text.split('|')[i]).selectedIndex].innerHTML;
            if (j.children[i + 1].innerHTML == "IsNULL")
                flag = true;
            else
                flag = false;
            j.children[0].innerHTML += j.children[i + 1].innerHTML + ' ';
            document.getElementById(text.split('|')[i]).selectedIndex = 0;

        }
        else {
            if (flag == false) {
                j.children[i + 1].innerHTML = document.getElementById(text.split('|')[i]).value;
                j.children[0].innerHTML += ' ' + j.children[i + 1].innerHTML;
                document.getElementById(text.split('|')[i]).value = '';
            }
        }
    }

    j.insertCell(i + 1);
    if (document.getElementById(text2.split('|')[0]).style.visibility == 'visible') {
        j.children[i + 1].innerHTML = document.getElementById(text2.split('|')[0]).options[document.getElementById(text2.split('|')[0]).selectedIndex].innerHTML;
    }
    else {
        j.children[i + 1].innerHTML = document.getElementById(text2.split('|')[1]).value;
    }
    document.getElementById(text2.split('|')[0]).selectedIndex = 0;
    document.getElementById(text2.split('|')[1]).value = '';
    j.children[0].innerHTML += ' ' + j.children[i + 1].innerHTML;
    i++;

    j.children[0].innerHTML = j.children[0].innerHTML.replace(/^\s+|\s+$/g, '') + ']';
    length = colIndex.split('|').length;
    for (var k = 0; k < length; k++, i++) {
        iButton = document.getElementById(a.children[0].children[1].children[colIndex.split('|')[k]].children[0].id);
        iButtonNew = iButton.cloneNode(true);
        j.insertCell(i + 1).appendChild(iButtonNew);
    }

    //**************************Adding Row data in Hidden field 

    var hiddenXMLID = id.substr(0, id.lastIndexOf('_')) + '_hiddenXML';
    if (!document.getElementById(hiddenXMLID).innerHTML == "") {
        document.getElementById(hiddenXMLID).innerHTML = "|";
    }
    for (var rowcnt = 2; rowcnt < a.rows.length; rowcnt++) {
        document.getElementById(hiddenXMLID).innerText += a.rows[rowcnt].cells[0].innerText;
    }
    return false;
}

function addRule(flag, id, parsedXML, colIndex) {
    text = document.getElementById("txtRule").value;
    //var iButton;
    //var iButtonNew;

    var length;
    var xmlString = "";
    if (flag == "True") {
        xmlString = '<%=Cache["xml"]%>';
    }
    else {
        xmlString = document.getElementById("hdn").value;
        xmlString = replaceAll(xmlString, "&lt", "<");
        xmlString = replaceAll(xmlString, "&gt", ">");



    }
    var columnName = '<%=Cache["columnName"]%>';
    var a = document.getElementById(id);
    a.insertRow(a.rows.length);
    var i = a.rows.length - 1;
    var j = a.children[0].children[i];
    length = text.split('|').length;
    var index = xmlString.lastIndexOf("</Table>");
    var begin;

    begin = xmlString.substring(0, index + 8);
    begin = begin + "<Table>";


    for (i = 0; i < length; i++) {
        j.insertCell(i);
        j.children[i].innerHTML = text.split('|')[i];
        begin = begin + "<" + columnName.split(',')[i] + ">" + text.split('|')[i] + "</" + columnName.split(',')[i] + ">";

    }
    begin = begin + "</Table>" + xmlString.substring(index + 8, xmlString.length);
    if (flag == "True") {
        begin = begin.substring(index + 8, begin.length);
        begin = "<NewDataSet>" + begin;

    }


    var regEx = new RegExp('<', 'gi');
    begin = begin.replace(regEx, '&lt');
    var regEx1 = new RegExp('>', 'gi');
    begin = begin.replace(regEx1, '&gt');
    document.getElementById("hdn").value = begin;
    length = colIndex.split('|').length;
    for (var k = 0; k < length; k++, i++) {
        var temp = ind;
        var iButton = document.getElementById(a.children[0].children[1].children[colIndex.split('|')[k]].children[0].id);
        var iButtonNew = iButton.cloneNode(true);
        //iButtonNew = document.createElement("INPUT");
        var attribute = iButtonNew.attributes;

        if (k == 0) {
            iButtonNew.id = "d" + a.rows.length;
            iButtonNew.onclick = function () { deleterow(iButtonNew.id); return false; };
        }
        else {
            iButtonNew.id = "u" + a.rows.length;
            iButtonNew.onclick = function () { updateRow(iButtonNew.id); return false; };
        }

        j.insertCell(i).appendChild(iButtonNew);
        ind++;
    }
    return false;
}

//**********************************************************************************************
//This method will select all dropdowns from grid based on value changed by master dropdown
//**********************************************************************************************   

function SelectAllDropDowns(dropDownListMain, dropDownListGrid, row) {
    if (document.getElementById(row.id).style.display == '')
        dropDownListGrid.value = dropDownListMain.value;
}

//**************************************FEED SUMMARY PAGE***************************************
//**********************************************************************************************

function CallUpDateFeedFieldModalPopup(modalFieldDetails, FieldName, FieldDescription, FieldStartIndex, FieldEndIndex, FieldPosition, Mandatory, Persist, Validation, AllowTrim, FieldXPath, RemoveWhiteSpace
         , VFieldName, VFieldDescription, VFieldStartIndex, VFieldEndIndex, VFieldPosition, VMandatory, VPersist, VValidation, VAllowTrim, VFieldXPath, VRemoveWhiteSpace) {

    document.getElementById(FieldName).value = VFieldName;
    document.getElementById(FieldDescription).value = VFieldDescription;
    document.getElementById(FieldStartIndex).value = VFieldStartIndex;
    document.getElementById(FieldEndIndex).value = VFieldEndIndex;
    document.getElementById(FieldPosition).value = VFieldPosition;
    document.getElementById(Mandatory).value = VMandatory;
    document.getElementById(Persist).value = VPersist;
    document.getElementById(Validation).value = VValidation;
    document.getElementById(AllowTrim).value = VAllowTrim;
    document.getElementById(FieldXPath).value = VFieldXPath;
    document.getElementById(RemoveWhiteSpace).value = VRemoveWhiteSpace;
    $find(modalFieldDetails).show();
}

function CallFeedFieldModalPopup(modalFieldDetails, FieldName, FieldDescription, FieldStartIndex, FieldEndIndex, FieldPosition, Mandatory, Persist, Validation, AllowTrim, FieldXPath, RemoveWhiteSpace) {

    document.getElementById(FieldName).value = '';
    document.getElementById(FieldDescription).value = '';
    document.getElementById(FieldStartIndex).value = '';
    document.getElementById(FieldEndIndex).value = '';
    document.getElementById(FieldPosition).value = '';
    document.getElementById(Mandatory).checked = false;
    document.getElementById(Persist).checked = false;
    //document.getElementById(Validation).checked = 'false';
    document.getElementById(AllowTrim).checked = false;
    document.getElementById(FieldXPath).value = '';
    document.getElementById(RemoveWhiteSpace).checked = false;
    $find(modalFieldDetails).show();
}


//******************************************
//Calling Page method using AJAX and javascript
//******************************************

function CallPageMethod() {
    PageMethods.MyFirstPageMethod(onSucceeded, onFailed);
}

function CallParametersPageMethod() {
    PageMethods.MyFirstParameterPageMethod("This is a Demo", onSucceeded, onFailed);
}

function onSucceeded(result, userContext, methodName) {
    //$get('div1').innerHTML=result;
    alert(result);
}

function onFailed(error, userContext, methodName) {
    alert("An error occurred");
}

//******************************************
// Calling the Custom Control from Loading Setup page (for Adding, Updating and Deleting)
//******************************************

function ShowModalForCustomLoading(
            modalpopUpId,
            hiddenFunc, hiddenFunc_Val,
            btnID, btn_Val, btn_Tip,
            hiddenCustomID, hiddenCustomID_Val,
            hiddenTaskMasterID, hiddenTaskMasterID_Val,
            hiddenTransDetailsID, hiddenTransDetailsID_Val,
            txtClassName, txtClassName_Val,
            txtAssemblyPath, txtAssemblyPath_Val,
            radLoadingType, radLoadingType_Val,
            radClassType, radClassType_Val,
            txtExecSeq, txtExecSeq_Val,
            panelClass, lblAssemblyPath, lblClassName, lblClassSequence, identityCol, hdnID, hdnFeedSource) {

    if (identityCol != "") {
        document.getElementById(hdnID).value = identityCol;
    }
    document.getElementById(lblAssemblyPath).innerHTML = '';
    document.getElementById(lblClassName).innerHTML = '';
    document.getElementById(lblClassSequence).innerHTML = '';

    //document.getElementById(radClassType).cells[0].children[0].checked = true;
    //document.getElementById(radClassType).cells[1].children[0].checked = false;
    $('#' + radClassType).find('td').eq(0)[0].children[0].checked = true;
    $('#' + radClassType).find('td').eq(1)[0].children[0].checked = false;
    if (GetObject(hdnFeedSource).value != '1') {
        //document.getElementById(radLoadingType).cells[0].children[0].checked = true;
        //document.getElementById(radLoadingType).cells[1].children[0].checked = false;
        $('#' + radLoadingType).find('td').eq(0)[0].children[0].checked = true;
        $('#' + radLoadingType).find('td').eq(1)[0].children[0].checked = false;
    }
    document.getElementById(txtClassName).value = '';
    document.getElementById(txtAssemblyPath).value = '';
    document.getElementById(txtExecSeq).value = '';
    document.getElementById(hiddenFunc).value = hiddenFunc_Val;
    document.getElementById(hiddenTaskMasterID).value = hiddenTaskMasterID_Val;
    document.getElementById(hiddenTransDetailsID).value = hiddenTransDetailsID_Val;

    if (btn_Val == 'Save') {
        document.getElementById(hiddenCustomID).value = '0';
        document.getElementById(txtClassName).value = '';
        document.getElementById(txtAssemblyPath).value = '';
        document.getElementById(txtExecSeq).value = '';
        if (GetObject(hdnFeedSource).value != '1') {
            //document.getElementById(radLoadingType).cells[0].children[0].checked = true;
            $('#' + radLoadingType).find('td').eq(0)[0].children[0].checked = true;
        }
        //document.getElementById(radClassType).cells[0].children[0].checked = true;
        $('#' + radClassType).find('td').eq(0)[0].children[0].checked = true;
    }
    else {
        document.getElementById(hiddenCustomID).value = hiddenCustomID_Val;
        document.getElementById(txtClassName).value = txtClassName_Val;
        document.getElementById(txtAssemblyPath).value = txtAssemblyPath_Val;
        document.getElementById(txtExecSeq).value = txtExecSeq_Val;

        if (GetObject(hdnFeedSource).value != '1') {
            if (radLoadingType_Val == 'PRE') {
                //document.getElementById(radLoadingType).cells[0].children[0].checked = true;
                //document.getElementById(radLoadingType).cells[1].children[0].checked = false;
                $('#' + radLoadingType).find('td').eq(0)[0].children[0].checked = true;
                $('#' + radLoadingType).find('td').eq(1)[0].children[0].checked = false;
            }
            else {
                //document.getElementById(radLoadingType).cells[0].children[0].checked = false;
                //document.getElementById(radLoadingType).cells[1].children[0].checked = true;
                $('#' + radLoadingType).find('td').eq(0)[0].children[0].checked = false;
                $('#' + radLoadingType).find('td').eq(1)[0].children[0].checked = true;
            }
        }
        //        else
        //        {
        //            document.getElementById(radLoadingType).cells[0].children[0].checked = true;
        //        }

        if (radClassType_Val == '1') {
            //document.getElementById(radClassType).cells[0].children[0].checked = true;
            //document.getElementById(radClassType).cells[1].children[0].checked = false;
            $('#' + radClassType).find('td').eq(0)[0].children[0].checked = true;
            $('#' + radClassType).find('td').eq(1)[0].children[0].checked = false;
        }
        else if (radClassType_Val == '2') {
            //document.getElementById(radClassType).cells[0].children[0].checked = false;
            //document.getElementById(radClassType).cells[1].children[0].checked = true;
            $('#' + radClassType).find('td').eq(0)[0].children[0].checked = false;
            $('#' + radClassType).find('td').eq(1)[0].children[0].checked = true;
        }
        else {
            //document.getElementById(radClassType).cells[0].children[0].checked = false;
            //document.getElementById(radClassType).cells[1].children[0].checked = true;
            $('#' + radClassType).find('td').eq(0)[0].children[0].checked = false;
            $('#' + radClassType).find('td').eq(1)[0].children[0].checked = true;
        }
    }

    if (btn_Val == 'Details')
        document.getElementById(btnID).style.visibility = 'hidden';
    else {
        document.getElementById(btnID).style.visibility = 'visible';
        document.getElementById(btnID).value = btn_Val;
    }

    DecideCustomLoading(radClassType, panelClass);


    var doHide;
    if (btn_Val == 'Details' || btn_Val == 'Delete')
        doHide = true;
    else
        doHide = false;

    document.getElementById(radClassType).disabled = doHide;
    document.getElementById(txtClassName).disabled = doHide;
    document.getElementById(txtAssemblyPath).disabled = doHide;
    document.getElementById(txtExecSeq).disabled = doHide;

    $find(modalpopUpId).show();
}

function DecideCustomLoading(radCustom, panelClass) {
    //if (document.getElementById(radCustom).cells[0].children[0].checked == true)
    if ($('#' + radCustom).find('td').eq(0)[0].children[0].checked == true)
        document.getElementById(panelClass).style.display = 'none';
    else
        document.getElementById(panelClass).style.display = '';
}

//******************************************
// Calling the Subscription Panel
//******************************************

function OpenSubscriptionPanel(
panelSub, drpTasks,
taskName, taskName_Val,
lstSuccess, lstSuccess_Val,
lstFailure, lstFailure_Val,
taskMasterID, taskMasterID_Val,
btnSubscription, btnSubscription_Text,
hiddenFunc, hiddenFunc_Val, modalError) {
    if (document.getElementById(drpTasks).selectedIndex == 0) {
        $find(modalError).show();
        return false;
    }
    document.getElementById(panelSub).style.visibility = 'visible';
    document.getElementById(hiddenFunc).value = hiddenFunc_Val;

    var lenSuccess = document.getElementById(lstSuccess).length;
    var lenFailure = document.getElementById(lstFailure).length;

    for (var i = 0; i < lenSuccess; i++)
        document.getElementById(lstSuccess)[i].selected = false;
    for (var j = 0; j < lenFailure; j++)
        document.getElementById(lstFailure)[j].selected = false;

    if (hiddenFunc_Val == 'AddSubscription') {
        document.getElementById(drpTasks).disabled = true;
        var ddlLength = document.getElementById(taskMasterID_Val).length;
        for (var i = 0; i < ddlLength; i++)
            if (document.getElementById(taskMasterID_Val)[i].selected == true) {
                document.getElementById(taskMasterID).value = trim(GetObject(taskMasterID_Val)[i].value);
                document.getElementById(taskName).innerText = GetObject(taskName_Val)[i].innerHTML;
            }
    }
    else {
        var arrSuccess = new Array();
        arrSuccess = lstSuccess_Val.toString().split('|');
        var arrFailure = new Array();
        arrFailure = lstFailure_Val.toString().split('|');

        for (var i = 0; i < lenSuccess; i++)
            for (var j = 0; j < arrSuccess.length; j++)
                if (document.getElementById(lstSuccess)[i].innerText == arrSuccess[j])
                    document.getElementById(lstSuccess)[i].selected = true;

        for (var i = 0; i < lenFailure; i++)
            for (var j = 0; j < arrFailure.length; j++)
                if (document.getElementById(lstFailure)[i].innerText == arrFailure[j])
                    document.getElementById(lstFailure)[i].selected = true;

        document.getElementById(drpTasks).disabled = false;
        document.getElementById(taskMasterID).value = trim(GetObject(taskName_Val)[GetObject(taskName_Val).selectedIndex].value);
        document.getElementById(taskName).innerText = GetObject(taskName_Val)[GetObject(taskName_Val).selectedIndex].innerHTML;
    }

    document.getElementById(btnSubscription).value = btnSubscription_Text;

    var doHide;
    if (hiddenFunc_Val == 'DeleteSubscription')
        doHide = true;
    else
        doHide = false;

    document.getElementById(lstSuccess).disabled = doHide;
    document.getElementById(lstFailure).disabled = doHide;
    return false;
}
//******************************************
// Calling the Subscription Panel
//******************************************

function OpenSubscriptionPanelUpdate(
panelSub, drpTasks,
taskName, taskName_Val,
lstSuccess, lstSuccess_Val,
lstFailure, lstFailure_Val,
taskMasterID, taskMasterID_Val,
btnSubscription, btnSubscription_Text,
hiddenFunc, hiddenFunc_Val) {
    document.getElementById(panelSub).style.visibility = 'visible';
    document.getElementById(hiddenFunc).value = hiddenFunc_Val;

    var lenSuccess = document.getElementById(lstSuccess).length;
    var lenFailure = document.getElementById(lstFailure).length;

    for (var i = 0; i < lenSuccess; i++)
        document.getElementById(lstSuccess)[i].selected = false;
    for (var j = 0; j < lenFailure; j++)
        document.getElementById(lstFailure)[j].selected = false;

    if (hiddenFunc_Val == 'AddSubscription') {
        document.getElementById(drpTasks).disabled = true;
        var ddlLength = document.getElementById(taskMasterID_Val).length;
        for (var i = 0; i < ddlLength; i++)
            if (document.getElementById(taskMasterID_Val)[i].selected == true) {
                document.getElementById(taskMasterID).value = taskMasterID_Val;
                document.getElementById(taskName).innerText = taskName_Val;
            }
    }
    else {
        var arrSuccess = new Array();
        arrSuccess = lstSuccess_Val.toString().split('|');
        var arrFailure = new Array();
        arrFailure = lstFailure_Val.toString().split('|');

        for (var i = 0; i < lenSuccess; i++)
            for (var j = 0; j < arrSuccess.length; j++)
                if (document.getElementById(lstSuccess)[i].innerText == arrSuccess[j])
                    document.getElementById(lstSuccess)[i].selected = true;

        for (var i = 0; i < lenFailure; i++)
            for (var j = 0; j < arrFailure.length; j++)
                if (document.getElementById(lstFailure)[i].innerText == arrFailure[j])
                    document.getElementById(lstFailure)[i].selected = true;

        document.getElementById(drpTasks).disabled = false;
        document.getElementById(taskMasterID).value = taskMasterID_Val;
        document.getElementById(taskName).innerText = taskName_Val;
    }

    document.getElementById(btnSubscription).value = btnSubscription_Text;

    var doHide;
    if (hiddenFunc_Val == 'DeleteSubscription')
        doHide = true;
    else
        doHide = false;

    document.getElementById(lstSuccess).disabled = doHide;
    document.getElementById(lstFailure).disabled = doHide;
    return false;
}

function CloseSubscriptionPanel(panelSub, ddlTasks) {
    document.getElementById(panelSub).style.visibility = 'hidden';
    document.getElementById(ddlTasks).disabled = false;
}

function ValidateSearch(textSearch) {
    if (trim(document.getElementById(textSearch).value) == '') {
        alert("Please enter the search value");
        return false;
    }
    else
        return true;
}

function ShowAlertPopup(modalpoupid) {
    $find(modalpoupid).show();
}

function ValidateCustomPopupLoading(txtClassAssemblyPath, txtClassName, txtClassSequence, lblClassAssemblyPath, lblClassName, lblClassSequence, panelClass, gridCustom, btnCustom, hdnID, classType) {
    document.getElementById(lblClassAssemblyPath).innerHTML = '';
    document.getElementById(lblClassName).innerHTML = '';
    document.getElementById(lblClassSequence).innerHTML = '';
    var radioList = document.getElementById(classType);



    var gridObj = document.getElementById(gridCustom);
    var txtValue = document.getElementById(txtClassSequence).value.replace(/^\s+|\s+$/g, '');
    if (parseInt(txtValue) <= 0) {
        document.getElementById(lblClassSequence).innerText = '* Sequence should be greater than 0.';
        return false;
    }

    var classTypeText = ''
    var i;
    for (i = 0; i < radioList.getElementsByTagName('INPUT').length; i++) {
        if (radioList.getElementsByTagName('INPUT')[i].checked == true)
            classTypeText = radioList.getElementsByTagName('INPUT')[i].value;
    }

    document.getElementById(lblClassSequence).innerText = '';

    if (document.getElementById(btnCustom).value.toUpperCase() == "UPDATE") {
        for (var count = 1; count < gridObj.rows.length; count++) {
            if (gridObj.rows[count].cells[0].innerText.replace(/^\s+|\s+$/g, '') == txtValue && gridObj.rows[count].cells[3].innerText.replace(/^\s+|\s+$/g, '') == classTypeText) {
                if (document.getElementById(hdnID).value != '') {
                    if (trim(document.getElementById(hdnID).value) != count) {
                        document.getElementById(lblClassSequence).innerText = '* Duplicate Sequence.';
                        return false;
                    }
                }
            }
        }
    }
    else if (document.getElementById(btnCustom).value.toUpperCase() == "ADD") {
        for (var count = 1; count < gridObj.rows.length; count++) {
            if (gridObj.rows[count].cells[0].innerText.replace(/^\s+|\s+$/g, '') == txtValue && gridObj.rows[count].cells[3].innerText.replace(/^\s+|\s+$/g, '') == classTypeText) {
                document.getElementById(lblClassSequence).innerText = '* Duplicate Sequence.';
                return false;
            }
        }
    }
    return (ValidateCustomPopup(txtClassAssemblyPath, txtClassName, txtClassSequence, lblClassAssemblyPath, lblClassName, lblClassSequence, panelClass));
}

function HideShowCheckExistingCheckBox(chkSecurity, trChkExisting, chkExisting) {
    var chckSecurity = GetObject(chkSecurity);
    if (chckSecurity.checked == true) {
        document.getElementById(chkExisting).checked = false;
        document.getElementById(trChkExisting).style.display = '';
    }
    else
        document.getElementById(trChkExisting).style.display = 'none';
}

function ValidateCustomPopup(txtClassAssemblyPath, txtClassName, txtClassSequence, lblClassAssemblyPath, lblClassName, lblClassSequence, panelClass) {
    document.getElementById(lblClassAssemblyPath).innerHTML = '';
    document.getElementById(lblClassName).innerHTML = '';
    document.getElementById(lblClassSequence).innerHTML = '';

    if (document.getElementById(panelClass).style.display == '') {
        if (trim(document.getElementById(txtClassAssemblyPath).value) == '') {
            document.getElementById(lblClassAssemblyPath).innerHTML = '* Please Enter assembly Path';
            return false;
        }
        else if (trim(document.getElementById(txtClassName).value) == '') {
            document.getElementById(lblClassName).innerHTML = '* Please Enter a Class Name';
            return false;
        }
        else if (trim(document.getElementById(txtClassSequence).value) == '') {
            document.getElementById(lblClassSequence).innerHTML = '* Please Enter a Class Sequence';
            return false;
        }
        else if (parseInt(trim(document.getElementById(txtClassSequence).value)) <= 0) {
            document.getElementById(lblClassSequence).innerHTML = '* Class Sequence should be greater than 0';
            return false;
        }
        else
            return true;
    }
    else {
        document.getElementById(txtClassAssemblyPath).value = '';
        if (trim(document.getElementById(txtClassName).value) == '') {
            document.getElementById(lblClassName).innerHTML = '* Please Enter a Class Name';
            return false;
        }
        else if (trim(document.getElementById(txtClassSequence).value) == '') {
            document.getElementById(lblClassSequence).innerHTML = '* Please Enter a Class Sequence';
            return false;
        }
        else if (parseInt(trim(document.getElementById(txtClassSequence).value)) <= 0) {
            document.getElementById(lblClassSequence).innerHTML = '* Class Sequence should be greater than 0';
            return false;
        }
        else
            return true;
    }
}
/*-------------------------------------------------------*/
//Secmaster Core:

function checkShuttleSelection(SecTypeShuttle, lblError, modalError) {
    if (GetObject(SecTypeShuttle).children[2].value == '') {
        GetObject(lblError).innerText = '● Select some Security types';
        $find(modalError).show();
        return false;
    }
    else
        return true;
}

function hidepopUp(modalAddTab) {
    $find(modalAddTab).hide();
}

function CallTabPopUp(modalAddTab, txtTabName, lblTabNameError) {
    GetObject(txtTabName).value = '';
    GetObject(lblTabNameError).innerText = '';
    $find(modalAddTab).show();
    GetObject(txtTabName).focus();
}

/*************************************************************************************************
function FillModalDropDownList :Written and commented By shiv mohan , Date :07Aug08
Modified and commented By : 
Description: returns Expression names separated with |[pipe] symbol from XML saved in hiddenExpressionTableXML 
Parameters:-
hiddenExpressionXMLId ->client Id of hiddenExpressionTableXML text box
*************************************************************************************************/
function GetExpressionXML(hiddenExpressionXMLId) {
    var expressionsXML = new Array();
    var strExpressionRow = GetObject(hiddenExpressionXMLId).value;
    strExpressionRow = strExpressionRow.replace(/&lts/g, '<');
    strExpressionRow = strExpressionRow.replace(/&gts/g, '>');
    var openingIndex = 0;
    var closingIndex = 0;
    while (openingIndex < strExpressionRow.lastIndexOf('<Display_Name>')) {
        expressionsXML.push(strExpressionRow.substring(strExpressionRow.indexOf('<Display_Name>', openingIndex + 1)
                        + '<Display_Name>'.length, strExpressionRow.indexOf('</Display_Name>', closingIndex + 1)));
        openingIndex = strExpressionRow.indexOf('<Display_Name>', openingIndex + 1);
        closingIndex = strExpressionRow.indexOf('</Display_Name>', closingIndex + 1);
    }
    return expressionsXML;

} //End of the function GetExpressionXML  

//************code to add new row in XML*********************
function UpdateXML(colNames, hdnExpressionXMLId, strdummyRow) {
    var xmlString = GetObject(hdnExpressionXMLId).value;
    xmlString = xmlString.replace(/&lts/g, '<');
    xmlString = xmlString.replace(/&gts/g, '>');
    var checkFlagForNoRow = false; //Flag is true if Dataset is blank
    var begin = '';
    var ColumnCount = colNames.split('|').length;
    var ExpressionsXML = '';
    if (xmlString == '<NewDataSet />' || xmlString == '') {
        checkFlagForNoRow = true;
        begin += '<NewDataSet><Table>';

        for (rowIndex = 0; rowIndex < ColumnCount; rowIndex++)
            begin += '<' + colNames.split('|')[rowIndex] + '>' + GetEscapeSequence(strdummyRow[rowIndex].toString())
                    + '</' + colNames.split('|')[rowIndex] + '>';
        begin += '</Table></NewDataSet>';
    }
    else {
        var index = xmlString.lastIndexOf('</Table>');
        if (index != -1)
            begin = xmlString.substring(0, index + 8);
        else
            begin = xmlString.substring(0, 12);
        begin = begin + '<Table>';
        for (rowIndex = 0; rowIndex < ColumnCount; rowIndex++) {
            begin += '<' + colNames.split('|')[rowIndex] + '>'
                         + GetEscapeSequence(strdummyRow[rowIndex].toString())
                         + '</' + colNames.split('|')[rowIndex] + '>';
        }
        begin = begin + '</Table>' + xmlString.substring(index + 8, xmlString.length);
    }
    begin = begin.replace(/</g, '&lts');
    begin = begin.replace(/>/g, '&gts');

    GetObject(hdnExpressionXMLId).value = begin;
}

function checkTextBox(txtWorkFlowName, modalError, lblError, message) {
    var lblObj = GetObject(lblError);
    lblObj.innerText = '';
    var objWorkFlowName = GetObject(txtWorkFlowName);
    if (trim(objWorkFlowName.value) == '') {
        objWorkFlowName.focus();
        lblObj.innerText = message;
        $find(modalError).show();
        return false;
    }
    else
        return true;
}

function checkLevelName(txtWorkFlowNameNew, lblAddWorkFlowError, message) {
    var lblObj = GetObject(lblAddWorkFlowError);
    lblObj.innerText = '';
    if (trim(GetObject(txtWorkFlowNameNew).value) == '') {
        lblObj.innerText = message;
        lblObj.style.color = 'red';
        return false;
    }
    else
        return true;
}

function checkUserCount(UserShuttle, lblErrorUser, hdnIsUserCountStatic, hdnUserCount) {
    var lblObj = GetObject(lblErrorUser);
    lblObj.innerText = '';
    var IsUserStatic = trim(GetObject(hdnIsUserCountStatic).value);
    var userCount = trim(GetObject(hdnUserCount).value);
    var rightUser = GetObject(UserShuttle).children[2].value.split('~~~~').length - 1;
    if (IsUserStatic == "1" && userCount != rightUser) {
        lblObj.innerText = '● You cannot update this workflow as some requests are pending';
        return false;
    }
    if (rightUser > 9) {
        lblObj.innerText = '● You can not assign more than 9 Group to one Level';
        return false;
    }
    else
        return true;
}


function updateReportAttribute(modalAttributeUpdate, txtAttributeNameU, lblUpdateError, lblName, rowIndex, hiddenRowId, hiddenName, rbldataTypeUpdateId, selectedRadio) {
    GetObject(lblUpdateError).innerText = ''; // Cleans the Error label
    var name = trim(GetObject(lblName).innerText);
    var radioButtons = document.getElementById(rbldataTypeUpdateId);
    //    for (var x = 0; x < radioButtons.cells.length; x++) {

    //        if (trim(radioButtons.cells[x].children[0].value) == trim(selectedRadio)) {
    //            radioButtons.cells[x].children[0].checked = true;
    //        }
    //        radioButtons.disabled = true;

    //    }
    var rblCells = $(radioButtons).find('td');
    for (var count = 0; count < rblCells.length; count++) {
        if (trim(rblCells.eq(count)[0].children[0].value) == trim(selectedRadio)) {
            rblCells.eq(count)[0].children[0].checked = true;
        }
        radioButtons.disabled = true;
    }

    $('#UpdateAttribReportDataTypeLabel').text(selectedRadio);
    $('#UpdateAttribReportDataTypeLabel').show();
    $('#' + rbldataTypeUpdateId).hide();

    GetObject(txtAttributeNameU).value = name; // Set the name in TextBox
    GetObject(hiddenName).value = name; //to hold the Attribute Name
    GetObject(hiddenRowId).value = rowIndex; //to hold the rowIndex
    $find(modalAttributeUpdate).show(); // Show the PopUp
    GetObject(txtAttributeNameU).focus(); // Set Focus in Textbox
}

function saveUpdateAttribute(gridAttributeDetails, rblDataTypeUpdateId, hiddenXML, hiddenRowId, hiddenName, txtAttributeNameU, modalAttributeUpdate, lblUpdateError) {
    var updatedname = trim(GetObject(txtAttributeNameU).value);
    var radioButtons = document.getElementById(rblDataTypeUpdateId);
    var oldName = trim(GetObject(hiddenName).value);
    var selectedDataType;
    var selectedDataTypeId;
    var gridObj = GetObject(gridAttributeDetails);
    var length = gridObj.rows.length;
    var count;
    var rowIndex;
    //    for (var x = 0; x < radioButtons.cells.length; x++) {
    //        if (radioButtons.cells[x].children[0].checked == true) {
    //            //  selectedDataTypeId=radioButtons.cells[x].children[0].value;
    //            selectedDataType = radioButtons.cells[x].innerText;
    //        }

    //    }
    var rblCells = $(radioButtons).find('td');
    for (var count = 0; count < rblCells.length; count++) {
        if (rblCells.eq(count).children().eq(0).prop('checked').toString() == "true") {
            selectedDataType = rblCells.eq(count)[0].innerText;
        }
    }
    if (updatedname.toLowerCase() == 'action identifier') {
        GetObject(lblUpdateError).innerText = '● This Attribute Name Is Not Allowed';
        return false;

    }

    if (updatedname.toLowerCase() != oldName.toLowerCase())
        for (count = 1; count < length; count++)
            if (trim(gridObj.rows[count].cells[0].innerText.toLowerCase()) == updatedname.toLowerCase()) {
                GetObject(lblUpdateError).innerText = '● Attribute Name already exist';
                return false;
            }

    var rowID = trim(GetObject(hiddenRowId).value);
    for (rowIndex = 0; rowIndex < length; rowIndex++)
        if (trim(gridObj.rows[rowIndex].cells[4].innerText) == rowID)
            break;
    gridObj.rows[rowIndex].cells[0].innerText = updatedname;
    gridObj.rows[rowIndex].cells[1].innerText = selectedDataType;


    var strExpressionRow = GetObject(hiddenXML).value;
    strExpressionRow = strExpressionRow.replace(/&lts/g, '<');
    strExpressionRow = strExpressionRow.replace(/&gts/g, '>');

    //deleting row from XML
    var tableArray = strExpressionRow.split('<Table>');
    var finalxml = '';

    for (var i = 0; i < tableArray.length; i++) {
        finalxml += '<Table>';
        if (i != rowIndex)
            finalxml += tableArray[i];
        else {
            var startindex = tableArray[i].indexOf('<report_attribute_name>') + 23;
            var Endindex = tableArray[i].indexOf('</report_attribute_name>');
            var firsthalf = tableArray[i].substring(0, startindex);
            var Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + updatedname + Secondhalf;


            startindex = tableArray[i].indexOf('<is_modified>') + 13;
            Endindex = tableArray[i].indexOf('</is_modified>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + '1' + Secondhalf;

            startindex = tableArray[i].indexOf('<data_type_name>') + 16;
            Endindex = tableArray[i].indexOf('</data_type_name>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + selectedDataType + Secondhalf;

            //            startindex = tableArray[i].indexOf('<attribute_data_type_id>') + 24;
            //            Endindex = tableArray[i].indexOf('</attribute_data_type_id>');
            //            firsthalf = tableArray[i].substring(0, startindex);
            //            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            //            tableArray[i] = firsthalf + selectedDataTypeId + Secondhalf;

            finalxml += tableArray[i];
        }
    }
    if (finalxml.indexOf('<Table>') == 0)
        finalxml = finalxml.substring(7);
    if (finalxml.lastIndexOf('</NewDataSet>') == -1)
        finalxml += '</NewDataSet>';

    finalxml = finalxml.replace(/</g, '&lts');
    finalxml = finalxml.replace(/>/g, '&gts');
    GetObject(hiddenXML).value = finalxml;
    $find(modalAttributeUpdate).hide();
    return false;
}

function deleteReportAttribute(gridAttributeDetails, deleteRowID, hiddenXML, delColindex, trgridAttributeDetails) {
    var expressionsXML = '';
    var strExpressionRow = GetObject(hiddenXML).value;
    var gridObj = GetObject(gridAttributeDetails);
    var length = gridObj.rows.length;
    strExpressionRow = strExpressionRow.replace(/&lts/g, '<');
    strExpressionRow = strExpressionRow.replace(/&gts/g, '>');
    //deleting selected row from table (by default this HTML table will have 2 rows)
    var rowIndex = 0;
    for (rowIndex = 1; rowIndex < length; rowIndex++)
        if (parseInt(trim(gridObj.rows[rowIndex].cells[delColindex].innerText)) == parseInt(deleteRowID))
            break;
    gridObj.deleteRow(rowIndex);

    //deleting row from XML
    var tableArray = strExpressionRow.split('<Table>');
    var finalxml = '';
    if (tableArray.length <= 2)
        finalxml = '<NewDataSet />';
    else {
        for (var i = 0; i < tableArray.length; i++)
            if (i != rowIndex)
                finalxml += '<Table>' + tableArray[i];
        if (finalxml.indexOf('<Table>') == 0)
            finalxml = finalxml.substring(7);
        if (finalxml.lastIndexOf('</NewDataSet>') == -1)
            finalxml += '</NewDataSet>';
    }
    finalxml = finalxml.replace(/</g, '&lts');
    finalxml = finalxml.replace(/>/g, '&gts');
    GetObject(hiddenXML).value = finalxml;
    if (gridObj.rows.length <= 2)
        GetObject(trgridAttributeDetails).style.display = 'none';
} //End of function


function addAttributePopUp(modalAddAttribute, txtAttributeName, lblAddAttributeError, rblAttributeDataTypeID) {
    GetObject(txtAttributeName).value = '';
    var dataType = $('#addAttribDataTypeSaver').attr('datatype');
    $('#AddAttribReportDataTypeLabel').text(dataType);
    var radioButtons = document.getElementById(rblAttributeDataTypeID);
    //radioButtons.cells[0].children[0].checked = true;
    $(radioButtons).find('td').eq(0)[0].children[0].checked = true;
    GetObject(lblAddAttributeError).innerText = '';

    var rdl = $('#' + rblAttributeDataTypeID).children().eq(0).children().find('label');
    for (var index = 0; index < rdl.length; index++) {
        if (rdl.eq(index).text().toLowerCase() == dataType.toLowerCase()) {
            var rrr = rdl.eq(index).parent().find('input');
            rrr.prop('checked', true);
            break;
        }
    }

    $find(modalAddAttribute).show();
    GetObject(txtAttributeName).focus();
}

function addAttributeSave(gridAttributeDetails,
                          txtAttributeName, rblAttributeDataTypeID, rblAttributeDataTypeUpdateID,
                          hiddenXML,
                          colNames,
                          modalAddAttribute,
                          lblAddAttributeError,
                          modalAttributeUpdate,
                          txtAttributeNameU,
                          lblUpdateError,
                          hiddenRowId,
                          hiddenName,
                          trgridAttributeDetails, hiddenSavedAttributes) {
    var gridObj = document.getElementById(gridAttributeDetails);
    var length = gridObj.rows.length;
    var count;
    var selectedDataType;
    var selectedDataTypeId;
    var name = trim(document.getElementById(txtAttributeName).value);
    var radioButtons = document.getElementById(rblAttributeDataTypeID);
    //    for (var x = 0; x < radioButtons.cells.length; x++) {
    //        if (radioButtons.cells[x].children[0].checked == true) {

    //            selectedDataTypeId = selectedDataType = radioButtons.cells[x].children[0].value;
    //            selectedDataType = radioButtons.cells[x].innerText;
    //        }

    //    }
    var rblCells = $(radioButtons).find('td');
    for (var count = 0; count < rblCells.length; count++) {
        if (rblCells.eq(count).children().eq(0).prop('checked').toString() == "true") {
            selectedDataTypeId = selectedDataType = rblCells.eq(count)[0].children[0].value;
            selectedDataType = rblCells.eq(count)[0].innerText;
        }
    }

    if (name == '') {
        GetObject(lblAddAttributeError).innerText = '● Please enter Attribute Name';
        return false;
    }
    if (name.toLowerCase() == 'action identifier') {
        GetObject(lblAddAttributeError).innerText = '● This Attribute Name Is Not Allowed';
        return false;

    }
    for (count = 1; count < length; ++count)
        if (trim(gridObj.rows[count].cells[0].innerText.toLowerCase()) == name.toLowerCase()) {
            GetObject(lblAddAttributeError).innerText = '● Attribute Name already exist';
            return false;
        }

    var strdummyRow = new Array();
    var deleteRowIndex = trim(gridObj.rows[length - 1].cells[4].innerText);
    deleteRowIndex = parseInt(deleteRowIndex) + 1;
    gridObj.insertRow(length);
    var gridRowObj = gridObj.rows[length];
    strdummyRow.push('0');

    var but;
    var newBut;
    but = gridObj.rows[length - 1].cells[0];
    newBut = but.cloneNode(true);
    // newBut.children[0].innerText = name;
    // gridRowObj.insertCell(0).appendChild(newBut);
    gridRowObj.insertCell(0).innerText = name;
    gridRowObj.cells[0].align = 'left';
    strdummyRow.push(name);


    var data;
    var newButData;
    data = gridObj.rows[length - 1].cells[1];
    newButData = data.cloneNode(true);
    // newButData.children[0].innerText = selectedDataType;
    //  gridRowObj.insertCell(1).appendChild(newButData);
    gridRowObj.insertCell(1).innerText = selectedDataType;
    gridRowObj.cells[1].align = 'left';
    strdummyRow.push(selectedDataType);


    //   var AttributedataId;
    //    var newButDataId;
    //   AttributedataId=gridObj.rows[length - 1].cells[2];
    //     newButDataId = AttributedataId.cloneNode(true);
    // newButDataId.children[0].innerText = selectedDataTypeId;
    //   gridRowObj.insertCell(2).appendChild(newButDataId);
    // gridRowObj.cells[2].align = 'left';
    //    strdummyRow.push(selectedDataTypeId);
    //    gridRowObj.cells[2].style.display = 'none';



    var iButton;
    var iButtonNew;
    iButton = gridObj.rows[1].cells[2].children[0];
    iButtonNew = iButton.cloneNode(true);
    iButtonNew.disabled = false;

    iButtonNew.onclick = function () { updateReportAttributeMan(modalAttributeUpdate, txtAttributeNameU, rblAttributeDataTypeUpdateID, gridRowObj.cells[1].innerText, lblUpdateError, gridRowObj.cells[0].innerText, deleteRowIndex, hiddenRowId, hiddenName, hiddenSavedAttributes); return false; };
    gridRowObj.insertCell(2).appendChild(iButtonNew);
    gridRowObj.cells[2].align = 'center';

    var iButton1;
    var iButtonNew1;
    iButton1 = gridObj.rows[1].cells[3].children[0];
    iButtonNew1 = iButton1.cloneNode(true);
    iButtonNew1.disabled = false;

    iButtonNew1.onclick = function () { deleteReportAttribute(gridAttributeDetails, deleteRowIndex, hiddenXML, '4', trgridAttributeDetails); return false; };
    gridRowObj.insertCell(3).appendChild(iButtonNew1);
    gridRowObj.cells[3].align = 'center';

    //adding new column for RowID
    gridRowObj.insertCell(4);
    gridRowObj.cells[4].innerText = deleteRowIndex;
    strdummyRow.push(deleteRowIndex);
    gridRowObj.className = 'normalRow';
    gridRowObj.cells[4].style.display = 'none';

    strdummyRow.push('0');

    gridRowObj.className = 'normalRow';

    UpdateXML(colNames, hiddenXML, strdummyRow);
    $find(modalAddAttribute).hide();
    GetObject(trgridAttributeDetails).style.display = '';
    return false;
}

function updateReportAttributeMan(modalAttributeUpdate, txtAttributeNameU, rblAttributeDataTypeUpdateID, selectedDataTypeId, lblUpdateError, Name, rowIndex, hiddenRowId, hiddenName, hiddenSavedAttributes) {
    GetObject(lblUpdateError).innerText = ''; // Cleans the Error label

    var radioButtons = document.getElementById(rblAttributeDataTypeUpdateID);
    var savedAttr = document.getElementById(hiddenSavedAttributes).value;
    //    for (var x = 0; x < radioButtons.cells.length; x++) {
    //        if (trim(radioButtons.cells[x].children[0].value) == trim(selectedDataTypeId)) {
    //            radioButtons.cells[x].children[0].checked = true;
    //        }
    //    }
    var rblCells = $(radioButtons).find('td');
    for (var count = 0; count < rblCells.length; count++) {
        if (trim(rblCells.eq(count)[0].children[0].value) == trim(selectedDataTypeId)) {
            rblCells.eq(count)[0].children[0].checked = true;
        }
    }
    radioButtons.disabled = false;
    //    var arrSuccess = new Array();
    //        arrSuccess = savedAttr.split('|');
    //        for(var x=0;x<arrSuccess.length;x++)
    //        {
    //        if(arrSuccess[x]==name)
    //        radioButtons.disabled=true;
    //        }

    $('#UpdateAttribReportDataTypeLabel').hide();
    $('#' + rblAttributeDataTypeUpdateID).show();

    var name = trim(Name);
    GetObject(txtAttributeNameU).value = name; // Set the name in TextBox

    GetObject(hiddenName).value = name; //to hold the Attribute Name
    GetObject(hiddenRowId).value = rowIndex; //to hold the rowIndex
    $find(modalAttributeUpdate).show(); // Show the PopUp
    GetObject(txtAttributeNameU).focus(); // Set Focus in Textbox
}

// For Task Manager ~ Task Status
var value = 5000;


function displayresult(rvalue) {
    var lblObj = document.getElementById('lblShowMessage');
    if (rvalue.split('|')[1] == "TRUE") {
        lblObj.innerHTML = rvalue.split('|')[0];
        lblObj.style.display = '';
        setTimeout("CallServer('')", value);
    }
    else {
        lblObj.innerHTML = '';
        lblObj.style.display = 'none';
        return;
    }
    lblObj.style.color = "RED";
}


function CallTaskPanel() {
    setTimeout("CallServer('')", 100);








    return false;
    // based on condition recurse will be called
}

/*****************************************************
* ypSlideOutMenu
* 3/04/2001
*
* a nice little script to create exclusive, slide-out
* menus for ns4, ns6, mozilla, opera, ie4, ie5 on 
* mac and win32. I've got no linux or unix to test on but 
* it should(?) work... 
*
* Revised:
* - 08/29/2002 : added .hideAll()
* - 04/15/2004 : added .writeCSS() to support more 
*                than 30 menus.
*
* --youngpup--
*****************************************************/
ypSlideOutMenu.Registry = []
ypSlideOutMenu.aniLen = 420
ypSlideOutMenu.hideDelay = 100
ypSlideOutMenu.minCPUResolution = 10
// constructor
function ypSlideOutMenu(id, dir, left, top, width, height) {
    this.ie = document.all ? 1 : 0
    this.ns4 = document.layers ? 1 : 0
    this.dom = document.getElementById ? 1 : 0
    if (this.ie || this.ns4 || this.dom) {
        this.id = id
        this.dir = dir
        this.orientation = dir == "left" || dir == "right" ? "h" : "v"
        this.dirType = dir == "right" || dir == "down" ? "-" : "+"
        this.dim = this.orientation == "h" ? width : height
        this.hideTimer = false
        this.aniTimer = false
        this.open = false
        this.over = false
        this.startTime = 0
        this.gRef = "ypSlideOutMenu_" + id
        eval(this.gRef + "=this")
        ypSlideOutMenu.Registry[id] = this
        var d = document
        var strCSS = "";
        strCSS += '#' + this.id + 'Container { visibility:hidden; '
        strCSS += 'left:' + left + 'px; '
        strCSS += 'top:' + top + 'px; '
        strCSS += 'overflow:hidden; z-index:10000; }'
        strCSS += '#' + this.id + 'Container, #' + this.id + 'Content { position:absolute; '
        strCSS += 'width:' + width + 'px; '
        strCSS += 'height:' + height + 'px; '
        strCSS += 'clip:rect(0 ' + width + ' ' + height + ' 0); '
        strCSS += '}'
        this.css = strCSS;
        this.load()
    }
}
ypSlideOutMenu.writeCSS = function () {
    document.writeln('<style type="text/css">');
    for (var id in ypSlideOutMenu.Registry) {
        document.writeln(ypSlideOutMenu.Registry[id].css);
    }
    document.writeln('</style>');
}
ypSlideOutMenu.prototype.load = function () {
    var d = document
    var lyrId1 = this.id + "Container"
    var lyrId2 = this.id + "Content"
    var obj1 = this.dom ? d.getElementById(lyrId1) : this.ie ? d.all[lyrId1] : d.layers[lyrId1]
    if (obj1) var obj2 = this.ns4 ? obj1.layers[lyrId2] : this.ie ? d.all[lyrId2] : d.getElementById(lyrId2)
    var temp
    if (!obj1 || !obj2) window.setTimeout(this.gRef + ".load()", 100)
    else {
        this.container = obj1
        this.menu = obj2
        this.style = this.ns4 ? this.menu : this.menu.style
        this.homePos = eval("0" + this.dirType + this.dim)
        this.outPos = 0
        this.accelConst = (this.outPos - this.homePos) / ypSlideOutMenu.aniLen / ypSlideOutMenu.aniLen
        // set event handlers.
        if (this.ns4) this.menu.captureEvents(Event.MOUSEOVER | Event.MOUSEOUT);
        this.menu.onmouseover = new Function("ypSlideOutMenu.showMenu('" + this.id + "')")
        this.menu.onmouseout = new Function("ypSlideOutMenu.hideMenu('" + this.id + "')")
        //set initial state
        this.endSlide()
    }
}
ypSlideOutMenu.showMenu = function (id) {
    var reg = ypSlideOutMenu.Registry
    var obj = ypSlideOutMenu.Registry[id]
    if (obj.container) {
        obj.over = true
        //for (menu in reg) if (id != menu) ypSlideOutMenu.hide(menu)
        if (obj.hideTimer) { reg[id].hideTimer = window.clearTimeout(reg[id].hideTimer) }
        obj.showcount++;
        if (!obj.open && !obj.aniTimer) reg[id].startSlide(true)
    }
    if (obj.ns4) obj.menu.routeEvent(Event.MOUSEOVER);
}

ypSlideOutMenu.hideMenu = function (id) {
    var obj = ypSlideOutMenu.Registry[id]
    if (obj.container) {
        if (obj.hideTimer) window.clearTimeout(obj.hideTimer)
        obj.showcount--;
        obj.hideTimer = window.setTimeout("ypSlideOutMenu.hide('" + id + "')", ypSlideOutMenu.hideDelay);
    }
    if (obj.ns4) obj.menu.routeEvent(Event.MOUSEOUT);
}
ypSlideOutMenu.hideAll = function () {
    var reg = ypSlideOutMenu.Registry
    for (menu in reg) {
        ypSlideOutMenu.hide(menu);
        if (menu.hideTimer) window.clearTimeout(menu.hideTimer);
    }
}

ypSlideOutMenu.hide = function (id) {
    var obj = ypSlideOutMenu.Registry[id]
    obj.over = false
    if (obj.hideTimer) window.clearTimeout(obj.hideTimer)
    obj.hideTimer = 0
    if (obj.open && !obj.aniTimer) obj.startSlide(false)
}

ypSlideOutMenu.prototype.startSlide = function (open) {
    this[open ? "onactivate" : "ondeactivate"]()
    this.open = open
    if (open) this.setVisibility(true)
    this.startTime = (new Date()).getTime()
    this.aniTimer = window.setInterval(this.gRef + ".slide()", ypSlideOutMenu.minCPUResolution)
}
ypSlideOutMenu.prototype.slide = function () {
    var elapsed = (new Date()).getTime() - this.startTime
    if (elapsed > ypSlideOutMenu.aniLen) this.endSlide()
    else {
        var d = Math.round(Math.pow(ypSlideOutMenu.aniLen - elapsed, 2) * this.accelConst)
        if (this.open && this.dirType == "-") d = -d
        else if (this.open && this.dirType == "+") d = -d
        else if (!this.open && this.dirType == "-") d = -this.dim + d
        else d = this.dim + d
        this.moveTo(d)
    }
}
ypSlideOutMenu.prototype.endSlide = function () {
    this.aniTimer = window.clearTimeout(this.aniTimer)
    this.moveTo(this.open ? this.outPos : this.homePos)
    if (!this.open) this.setVisibility(false)
    if ((this.open && !this.over) || (!this.open && this.over)) {
        this.startSlide(this.over)
    }
}
ypSlideOutMenu.prototype.setVisibility = function (bShow) {
    var s = this.ns4 ? this.container : this.container.style
    s.visibility = bShow ? "visible" : "hidden"
}
ypSlideOutMenu.prototype.moveTo = function (p) {
    this.style[this.orientation == "h" ? "left" : "top"] = this.ns4 ? p : p + "px"
}
ypSlideOutMenu.prototype.getPos = function (c) {
    return parseInt(this.style[c])
}
ypSlideOutMenu.prototype.onactivate = function () { }
ypSlideOutMenu.prototype.ondeactivate = function () { }


var myMenu1 = new ypSlideOutMenu("menu1", "up", -1000, 689, 340, 300, null)
myMenu1.onactivate = function () { repositionMenu(myMenu1, -691); }



function repositionMenu(menu, offset) {
    var newLeft = getWindowWidth() / 2 + offset;
    menu.container.style ? menu.container.style.left = newLeft + "px" : menu.container.left = newLeft;
}
function getWindowWidth() {
    return window.innerWidth ? window.innerWidth : document.body.offsetWidth;
}


ypSlideOutMenu.writeCSS();

function hideCalendar(cb) {
    cb.hide();
}

function SetLeftMenuClickBit(value) {
    var elemrnt = document.getElementById('hdnIsLeftMenuClick');
    if (elemrnt != null)
        elemrnt.value = value;
}

function resetLeftHeight() {

    var browser = navigator.appName;
    if (browser == "Microsoft Internet Explorer") {
        var heightOfBody = document.body.parentElement.clientHeight;
        var heightOfHeader = document.getElementById('tdTop').offsetHeight;
        var heightOfFooter = document.getElementById('tdBottom').offsetHeight;
        var heightToBeDecreased = heightOfHeader + heightOfFooter;
        var flatHeightLeftPanel = 0;
        flatHeightLeftPanel = heightOfBody - heightToBeDecreased - 4;
        var actualLeftPanelHeight = document.getElementById('ctl00_upLeft').scrollHeight;
        var actualMainPanelHeight = document.getElementById('ctl00_upMain').scrollHeight;
        if (flatHeightLeftPanel > actualLeftPanelHeight && flatHeightLeftPanel > actualMainPanelHeight) {
            document.getElementById('ctl00_upLeft').style.height = flatHeightLeftPanel + 'px';
        }
        else {
            if (actualLeftPanelHeight > actualMainPanelHeight) {
                document.getElementById('ctl00_upLeft').style.height = actualLeftPanelHeight + 'px';
            }
            else {
                document.getElementById('ctl00_upLeft').style.height = actualMainPanelHeight + 'px';
            }
        }
    }
    else if (browser == "Netscape") {
        var heightOfBody = window.innerHeight;
        var heightOfHeader = document.getElementById('tdTop').offsetHeight;
        var heightOfFooter = document.getElementById('tdBottom').offsetHeight;
        var heightToBeDecreased = heightOfHeader + heightOfFooter;
        var heightApplicable = heightOfBody - heightToBeDecreased;
        document.getElementById('ctl00_upLeft').style.height = heightApplicable + 'px';
    }
    else if (browser == "Opera") {
        var heightOfBody = window.innerHeight;
        var heightOfHeader = document.getElementById('tdTop').offsetHeight;
        var heightOfFooter = document.getElementById('tdBottom').offsetHeight;
        var heightToBeDecreased = heightOfHeader + heightOfFooter;
        document.getElementById('ctl00_upLeft').style.height = heightOfBody - heightToBeDecreased;
    }
}

function AdjustReportViewerHeight() {
    if (document.getElementById('iFrameMainReportViewerContainer') != null) {
        //resetLeftHeight();
        var controlPanelHeight;
        if (document.getElementById('ctl00_upMain') != null && document.getElementById('tdMain') != null) {
            document.getElementById('ctl00_upMain').style.height = document.getElementById('tdMain').clientHeight;
            controlPanelHeight = document.getElementById('ctl00_upMain').clientHeight;
        }
        else
            controlPanelHeight = document.body.parentElement.clientHeight;

        if (document.getElementById('reportSettings') != null)
            controlPanelHeight = controlPanelHeight - document.getElementById('reportSettings').clientHeight;

        if (document.getElementById('iFrameMainReportViewerContainer').contentWindow.document.getElementById('ReportFramereportViewer') != null)
            document.getElementById('iFrameMainReportViewerContainer').contentWindow.document.getElementById('ReportFramereportViewer').style.height = controlPanelHeight + "px";

        if (document.getElementById('iFrameMainReportViewerContainer').contentWindow.document.getElementById('reportViewer') != null)
            document.getElementById('iFrameMainReportViewerContainer').contentWindow.document.getElementById('reportViewer').style.height = controlPanelHeight + "px";

        if (document.getElementById('reportViewer') != null)
            document.getElementById('reportViewer').style.height = controlPanelHeight + "px";

        if (document.getElementById('iFrameMainReportViewerContainer') != null)
            document.getElementById('iFrameMainReportViewerContainer').style.height = controlPanelHeight + 50 + "px";
    }
}

function resetLeftHeightHome() {
    //Maximize();
    var tdTopId = 'tdTop';
    var tdLeftId = 'tdLeft';
    var tdMiddleId = 'tdMiddle';
    var tdMainId = 'tdMain';
    var tdBottomId = 'tdBottom';
    var upMainId = 'ctl00_upMain';
    var aspNetFormId = "aspnetForm";

    var TdTop = document.getElementById(tdTopId);
    var TdLeft = document.getElementById(tdLeftId);
    var TdMiddle = document.getElementById(tdMiddleId);
    var TdMain = document.getElementById(tdMainId);
    var TdBottom = document.getElementById(tdBottomId);
    var UpMain = document.getElementById(upMainId);
    var FormElement = document.getElementById(aspNetFormId);
    //var windowHeight = 910;

    var ViewPortHeight = 0;
    if (screen.width == 1280 && screen.height == 768) {
        (document.documentElement.clientHeight < 583) ? (ViewPortHeight = 583) : (ViewPortHeight = document.documentElement.clientHeight);
    }
    else if (screen.width == 1280 && screen.height == 960) {
        (document.documentElement.clientHeight < 775) ? (ViewPortHeight = 775) : (ViewPortHeight = document.documentElement.clientHeight);
    }
    else if (screen.width == 1280 && screen.height == 1024) {
        (document.documentElement.clientHeight < 839) ? (ViewPortHeight = 839) : (ViewPortHeight = document.documentElement.clientHeight);
    }
    else if (screen.width == 1400 && screen.height == 1050) {
        (document.documentElement.clientHeight < 865) ? (ViewPortHeight = 865) : (ViewPortHeight = document.documentElement.clientHeight);
    }
    else if (screen.width == 1600 && screen.height == 900) {
        (document.documentElement.clientHeight < 715) ? (ViewPortHeight = 715) : (ViewPortHeight = document.documentElement.clientHeight);
    }
    else if (screen.width == 1600 && screen.height == 1200) {
        (document.documentElement.clientHeight < 1015) ? (ViewPortHeight = 1015) : (ViewPortHeight = document.documentElement.clientHeight);
    }
    else if (screen.width == 1920 && screen.height == 1080) {
        (document.documentElement.clientHeight < 895) ? (ViewPortHeight = 895) : (ViewPortHeight = document.documentElement.clientHeight);
    }
    else if (screen.width == 1920 && screen.height == 1200) {
        (document.documentElement.clientHeight < 1015) ? (ViewPortHeight = 1015) : (ViewPortHeight = document.documentElement.clientHeight);
    }
    else {
        (document.documentElement.clientHeight < 583) ? (ViewPortHeight = 583) : (ViewPortHeight = document.documentElement.clientHeight);
    }


    var MainHeight = 0;
    var TopHeight = 0;
    if (TdTop != null) {
        TopHeight = TdTop.clientHeight;
    }
    var BottomHeight = 0;
    if (TdBottom != null) {
        BottomHeight = TdBottom.clientHeight;
    }

    if (FormElement.offsetHeight < ViewPortHeight) {
        if (FormElement != null) {
            FormElement.style.height = ViewPortHeight + "px";
        }
    }

    MainHeight = ViewPortHeight - TopHeight - BottomHeight - 3;
    if (TdLeft != null) {
        TdLeft.style.height = MainHeight.toString() + 'px';
    }
    if (parseInt(TdMain.offsetHeight) < MainHeight && parseInt(UpMain.offsetHeight) < MainHeight && FormElement.offsetHeight < ViewPortHeight) {
        if (TdMain != null) {
            TdMain.style.height = MainHeight.toString() + 'px';
        }
        if (UpMain != null) {
            UpMain.style.height = MainHeight.toString() + 'px';
        }
        if (FormElement != null) {
            FormElement.style.height = ViewPortHeight + "px";
        }
    }

}

function inkMenuOverLite() {
    var target = $('#idtopMenuTools');
    var position = target.offset();
    var idtopMenuToolsDiv = $('#idtopMenuToolsDiv');
    idtopMenuToolsDiv.css('top', position.top + target.outerHeight());
    idtopMenuToolsDiv.css('left', position.left - (idtopMenuToolsDiv.outerWidth() - target.outerWidth()));
    idtopMenuToolsDiv.css('display', '');
}

function inkMenuOver() {
    document.getElementById('idtopMenuToolsDiv').style.display = '';
}
function inkMenuMouseOut() {
    document.getElementById('idtopMenuToolsDiv').style.display = 'none';
}
function inkMenuClick() {
    document.getElementById('idtopMenuToolsDiv').style.display = '';
}

function resizeGridFinal(gridID, panelTopId, panelMiddleId, panelBottomId, subWidth, subHeight, isAlignRows) {
    var viewPortHeight = viewportSize.getHeight();
    var viewPortWidth = $(window).width();

    //div[id*='T_']
    var tdTop = $('#tdTop');
    var tdBottom = $('#tdBottom');
    var grid = $('#' + gridID);
    var upperHeaderDiv = grid.find("div[id='" + gridID + "_upperHeader_Div']");
    var headerDiv = grid.find("div[id='" + gridID + "_headerDiv']");
    var footerDiv = grid.find("div[id='" + gridID + "_footer_Div']");
    var bodyDiv = grid.find("div[id='" + gridID + "_body_Div']");
    var innerBodyDiv = grid.find("div[id='" + gridID + "_bodyDiv']");
    var frozenheaderDiv = grid.find("div[id='" + gridID + "_frozen_headerDiv']");
    var scrollerMainHorizontal = grid.find("div[id='" + gridID + "_scrollerMainHorizontal']");
    var scrollerMainVertical = grid.find("div[id='" + gridID + "_scrollerMainVertical']");
    var frozenBodyDiv = grid.find("div[id='" + gridID + "_frozen_bodyDiv']");
    var selectAllOptionDiv = grid.find("div[id='" + gridID + "_selectAllOption_Div']");
    var selectAllOptionFrozenDiv = grid.find("div[id='" + gridID + "_selectAllOptionFrozen_Div']");
    var headerDivTable = grid.find("table[id='" + gridID + "_headerDiv_Table']");
    var bodyDivTable = grid.find("table[id='" + gridID + "_bodyDiv_Table']");
    var panelTop = $('#' + panelTopId);
    var panelMiddle = $('#' + panelMiddleId);
    var panelBottom = $('#' + panelBottomId);

    var panelTopHeight = 0;
    if (panelTop != null && panelTop != undefined && panelTop.length > 0) {
        panelTopHeight = panelTop[0].offsetHeight;
    }

    var panelBottomHeight = 0;
    if (panelBottom != null && panelBottom != undefined && panelBottom.length > 0) {
        panelBottomHeight = panelBottom[0].offsetHeight;
    }

    var tdTopHeight = 0
    if (tdTop != null && tdTop != undefined && tdTop.length > 0) {
        tdTopHeight = tdTop[0].offsetHeight;
    }
    var tdBottomHeight = 0
    if (tdBottom != null && tdBottom != undefined && tdBottom.length > 0) {
        tdBottomHeight = tdBottom[0].offsetHeight;
    }

    var upperHeaderDivHeight = 0
    if (upperHeaderDiv != null && upperHeaderDiv != undefined && upperHeaderDiv.length > 0) {
        upperHeaderDivHeight = upperHeaderDiv.height();
    }

    var headerDivHeight = 0
    if (headerDiv != null && headerDiv != undefined && headerDiv.length > 0) {
        headerDivHeight = headerDiv.height();
    }

    var footerDivHeight = 0
    if (footerDiv != null && footerDiv != undefined && footerDiv.length > 0) {
        footerDivHeight = footerDiv.height();
    }

    var scrollerMainHorizontalHeight = 0
    if (scrollerMainHorizontal != null && scrollerMainHorizontal != undefined && scrollerMainHorizontal.length > 0) {
        scrollerMainHorizontalHeight = scrollerMainHorizontal.height();
        var divEmpty = scrollerMainHorizontal.next();
        if (divEmpty != null && divEmpty != undefined && divEmpty.length > 0 && divEmpty[0].tagName.toUpperCase() == 'DIV' && divEmpty.html() == '') {
            divEmpty.css('display', 'none');
        }
    }

    var frozenHeaderRowCellHeight = 0
    if (headerDivTable != null && headerDivTable != undefined && headerDivTable.length > 0) {
        frozenHeaderRowCellHeight = headerDivTable.find('tr:eq(0)').find('th:eq(0)').height();
    }

    if (frozenheaderDiv != null && frozenheaderDiv != undefined && frozenheaderDiv.length > 0) {
        frozenheaderDiv.find('th:eq(0)').height(frozenHeaderRowCellHeight);
    }

    if (frozenBodyDiv != null && frozenBodyDiv != undefined && frozenBodyDiv.length > 0) {
        frozenBodyDiv.find('table').find('tbody').children('tr').each(function (index) {
            var bodyRows = $(bodyDivTable[0].rows[index]);
            var tr = $(this);
            if (isAlignRows != undefined && isAlignRows != null && isAlignRows) {
                if ((tr.attr('class') != null && tr.attr('class') != undefined && tr.attr('class') != '') && (tr.attr('class').toLowerCase() == 'xlnormalrow' || tr.attr('class').toLowerCase() == 'xlalternatingrow')) {// || $(this).attr('class').toLowerCase() == 'groupfooter' || $(this).attr('class').toLowerCase() == 'groupedrow'
                    if (tr.outerHeight() != $(bodyDivTable[0].rows[index]).outerHeight())
                        $(tr[0].cells[0]).outerHeight(bodyDivTable[0].rows[index].cells[0].offsetHeight);
                }
            }
            //            if ((bodyRows.attr('class') != null && bodyRows.attr('class') != undefined && bodyRows.attr('class') != '') && bodyRows.attr('class').toLowerCase() == 'groupedrow') {               
            //                var bodyCells = bodyRows[0].cells[0];
            //                bodyCells.colSpan = bodyCells.colSpan + 1;
            //            }
        });
    }


    var selectAllOptionDivHeight = 0;
    if (selectAllOptionDiv != null && selectAllOptionDiv != undefined && selectAllOptionDiv.length > 0) {
        selectAllOptionDivHeight = (selectAllOptionDiv.css('display').toLowerCase() == 'none' ? 0 : selectAllOptionDiv.height());
        //selectAllOptionDiv.width(innerBodyDiv.width() - 20);
    }
    if (selectAllOptionFrozenDiv != null && selectAllOptionFrozenDiv != undefined && selectAllOptionFrozenDiv.length > 0) {
        selectAllOptionFrozenDiv.height(selectAllOptionDivHeight);
        selectAllOptionFrozenDiv.width(frozenBodyDiv.width() - 20);
    }

    var gridHeight = viewPortHeight - tdTopHeight - tdBottomHeight - upperHeaderDivHeight - headerDivHeight - footerDivHeight - panelTopHeight - panelBottomHeight - scrollerMainHorizontalHeight - selectAllOptionDivHeight - 5 - subHeight;
    if (innerBodyDiv != null && innerBodyDiv != undefined && innerBodyDiv.length > 0) {
        innerBodyDiv.height(gridHeight);
    }
    var frozenBodyDivWidth = 0;
    if (frozenBodyDiv != null && frozenBodyDiv != undefined && frozenBodyDiv.length > 0) {
        frozenBodyDiv.height(gridHeight);
        //  frozenBodyDivWidth = frozenBodyDiv.width();
    }

    var scrollerMainVerticalWidth = 0;
    //   if (scrollerMainVertical != null && scrollerMainVertical != undefined && scrollerMainVertical.length > 0) {
    //        //scrollerMainVerticalWidth = scrollerMainVertical.width();
    //   scrollerMainVertical.height(gridHeight + headerDivHeight + selectAllOptionDivHeight);
    // }
    if (gridHeight <= 300) {
        $("#table").height(upperHeaderDivHeight + headerDivHeight + footerDivHeight + panelTopHeight + panelBottomHeight + scrollerMainHorizontalHeight + selectAllOptionDivHeight + subHeight + 300 + 5);
    }

    var grid = $find(gridID);
    if (grid != null)
        grid.get_GridInfo().Height = gridHeight + 'px'; //viewPortHeight - tdTopHeight - tdBottomHeight - panelTopHeight - panelBottomHeight - subHeight;  
}

//If Grid is in a panel, then use this
function resizeGridFinalCustom(panelId, gridID, panelTopId, panelMiddleId, panelBottomId, subWidth, subHeight, isAlignRows) {
    var panel = $('#' + panelId)
    var viewPortHeight = panel.height();
    var viewPortWidth = panel.width();

    //div[id*='T_']
    var grid = $('#' + gridID);
    var upperHeaderDiv = grid.find("div[id='" + gridID + "_upperHeader_Div']");
    var headerDiv = grid.find("div[id='" + gridID + "_headerDiv']");
    var footerDiv = grid.find("div[id='" + gridID + "_footer_Div']");
    var bodyDiv = grid.find("div[id='" + gridID + "_body_Div']");
    var innerBodyDiv = grid.find("div[id='" + gridID + "_bodyDiv']");
    var frozenheaderDiv = grid.find("div[id='" + gridID + "_frozen_headerDiv']");
    var scrollerMainHorizontal = grid.find("div[id='" + gridID + "_scrollerMainHorizontal']");
    var scrollerMainVertical = grid.find("div[id='" + gridID + "_scrollerMainVertical']");
    var frozenBodyDiv = grid.find("div[id='" + gridID + "_frozen_bodyDiv']");
    var selectAllOptionDiv = grid.find("div[id='" + gridID + "_selectAllOption_Div']");
    var selectAllOptionFrozenDiv = grid.find("div[id='" + gridID + "_selectAllOptionFrozen_Div']");
    var headerDivTable = grid.find("table[id='" + gridID + "_headerDiv_Table']");
    var bodyDivTable = grid.find("table[id='" + gridID + "_bodyDiv_Table']");
    var panelTop = $('#' + panelTopId);
    var panelMiddle = $('#' + panelMiddleId);
    var panelBottom = $('#' + panelBottomId);

    var panelTopHeight = 0;
    if (panelTop != null && panelTop != undefined && panelTop.length > 0) {
        panelTopHeight = panelTop.height();
    }

    var panelBottomHeight = 0;
    if (panelBottom != null && panelBottom != undefined && panelBottom.length > 0) {
        panelBottomHeight = panelBottom.height();
    }

    var upperHeaderDivHeight = 0
    if (upperHeaderDiv != null && upperHeaderDiv != undefined && upperHeaderDiv.length > 0) {
        upperHeaderDivHeight = upperHeaderDiv.height();
    }

    var headerDivHeight = 0
    if (headerDiv != null && headerDiv != undefined && headerDiv.length > 0) {
        headerDivHeight = headerDiv.height();
    }

    var footerDivHeight = 0
    if (footerDiv != null && footerDiv != undefined && footerDiv.length > 0) {
        footerDivHeight = footerDiv.height();
    }

    var scrollerMainHorizontalHeight = 0
    if (scrollerMainHorizontal != null && scrollerMainHorizontal != undefined && scrollerMainHorizontal.length > 0) {
        scrollerMainHorizontalHeight = scrollerMainHorizontal.height();
        var divEmpty = scrollerMainHorizontal.next();
        if (divEmpty != null && divEmpty != undefined && divEmpty.length > 0 && divEmpty[0].tagName.toUpperCase() == 'DIV' && divEmpty.html() == '') {
            divEmpty.css('display', 'none');
        }
    }

    var frozenHeaderRowCellHeight = 0
    if (headerDivTable != null && headerDivTable != undefined && headerDivTable.length > 0) {
        frozenHeaderRowCellHeight = headerDivTable.find('tr:eq(0)').find('th:eq(0)').height();
    }

    if (frozenheaderDiv != null && frozenheaderDiv != undefined && frozenheaderDiv.length > 0) {
        frozenheaderDiv.find('th:eq(0)').height(frozenHeaderRowCellHeight);
    }

    if (frozenBodyDiv != null && frozenBodyDiv != undefined && frozenBodyDiv.length > 0) {
        frozenBodyDiv.find('table').find('tbody').children('tr').each(function (index) {
            var bodyRows = $(bodyDivTable[0].rows[index]);
            var tr = $(this);
            if (isAlignRows != undefined && isAlignRows != null && isAlignRows) {
                if ((tr.attr('class') != null && tr.attr('class') != undefined && tr.attr('class') != '') && (tr.attr('class').toLowerCase() == 'xlnormalrow' || tr.attr('class').toLowerCase() == 'xlalternatingrow')) {// || $(this).attr('class').toLowerCase() == 'groupfooter' || $(this).attr('class').toLowerCase() == 'groupedrow'
                    if (tr.outerHeight() != $(bodyDivTable[0].rows[index]).outerHeight())
                        $(tr[0].cells[0]).outerHeight(bodyDivTable[0].rows[index].cells[0].offsetHeight);
                }
            }
            //            if ((bodyRows.attr('class') != null && bodyRows.attr('class') != undefined && bodyRows.attr('class') != '') && bodyRows.attr('class').toLowerCase() == 'groupedrow') {               
            //                var bodyCells = bodyRows[0].cells[0];
            //                bodyCells.colSpan = bodyCells.colSpan + 1;
            //            }
        });
    }


    var selectAllOptionDivHeight = 0;
    if (selectAllOptionDiv != null && selectAllOptionDiv != undefined && selectAllOptionDiv.length > 0) {
        selectAllOptionDivHeight = (selectAllOptionDiv.css('display').toLowerCase() == 'none' ? 0 : selectAllOptionDiv.height());
        //selectAllOptionDiv.width(innerBodyDiv.width() - 20);
    }
    if (selectAllOptionFrozenDiv != null && selectAllOptionFrozenDiv != undefined && selectAllOptionFrozenDiv.length > 0) {
        selectAllOptionFrozenDiv.height(selectAllOptionDivHeight);
        selectAllOptionFrozenDiv.width(frozenBodyDiv.width() - 20);
    }

    var gridHeight = viewPortHeight - upperHeaderDivHeight - headerDivHeight - footerDivHeight - panelTopHeight - panelBottomHeight - scrollerMainHorizontalHeight - selectAllOptionDivHeight - subHeight - 22;
    if (innerBodyDiv != null && innerBodyDiv != undefined && innerBodyDiv.length > 0) {
        innerBodyDiv.height(gridHeight);
    }
    var frozenBodyDivWidth = 0;
    if (frozenBodyDiv != null && frozenBodyDiv != undefined && frozenBodyDiv.length > 0) {
        frozenBodyDiv.height(gridHeight);
        //  frozenBodyDivWidth = frozenBodyDiv.width();
    }

    var grid = $find(gridID);
    if (grid != null)
        grid.get_GridInfo().Height = gridHeight;

    panel.find(".slimScrollDiv").css("height", "auto");
    panel.find(".horizonslimScrollDiv").css("height", "auto");
}

function resizeGridMultipleGrids(gridID, panelTopId, panelMiddleId, panelBottomId, subWidth, subHeight) {

    var viewPortHeight = $(window).height();
    var viewPortWidth = $(window).width();
    //var viewPortHeight = $(".divNewContainer").height(); //--->
    //var viewPortWidth = $(".divNewContainer").width();

    //div[id*='T_']
    var tdTop = $('#tdTop');
    var tdBottom = $('#tdBottom');
    var grid = $('#' + gridID);
    var upperHeaderDiv = grid.find("div[id='" + gridID + "_upperHeader_Div']");
    var headerDiv = grid.find("div[id='" + gridID + "_headerDiv']");
    var footerDiv = grid.find("div[id='" + gridID + "_footer_Div']");
    var bodyDiv = grid.find("div[id='" + gridID + "_body_Div']");
    var innerBodyDiv = grid.find("div[id='" + gridID + "_bodyDiv']");
    var frozenheaderDiv = grid.find("div[id='" + gridID + "_frozen_headerDiv']");
    var scrollerMainHorizontal = grid.find("div[id='" + gridID + "_scrollerMainHorizontal']");
    var scrollerMainVertical = grid.find("div[id='" + gridID + "_scrollerMainVertical']");
    var frozenBodyDiv = grid.find("div[id='" + gridID + "_frozen_bodyDiv']");
    var selectAllOptionDiv = grid.find("div[id='" + gridID + "_selectAllOption_Div']");
    var selectAllOptionFrozenDiv = grid.find("div[id='" + gridID + "_selectAllOptionFrozen_Div']");
    var headerDivTable = grid.find("table[id='" + gridID + "_headerDiv_Table']");
    var bodyDivTable = grid.find("table[id='" + gridID + "_bodyDiv_Table']");
    var panelTop = $('#' + panelTopId);
    var panelMiddle = $('#' + panelMiddleId);
    var panelBottom = $('#' + panelBottomId);

    var panelTopHeight = 0;
    if (panelTop != null && panelTop != undefined && panelTop.length > 0) {
        panelTopHeight = panelTop.height();
    }

    var panelBottomHeight = 0;
    if (panelBottom != null && panelBottom != undefined && panelBottom.length > 0) {
        panelBottomHeight = panelBottom.height();
    }

    var tdTopHeight = 0
    if (tdTop != null && tdTop != undefined && tdTop.length > 0) {
        tdTopHeight = tdTop.height();
    }
    var tdBottomHeight = 0
    if (tdBottom != null && tdBottom != undefined && tdBottom.length > 0) {
        tdBottomHeight = tdBottom.height();
    }

    var upperHeaderDivHeight = 0
    if (upperHeaderDiv != null && upperHeaderDiv != undefined && upperHeaderDiv.length > 0) {
        upperHeaderDivHeight = upperHeaderDiv.height();
    }

    var headerDivHeight = 0
    if (headerDiv != null && headerDiv != undefined && headerDiv.length > 0) {
        headerDivHeight = headerDiv.height();
    }

    var footerDivHeight = 0
    if (footerDiv != null && footerDiv != undefined && footerDiv.length > 0) {
        footerDivHeight = footerDiv.height();
    }

    var scrollerMainHorizontalHeight = 0
    if (scrollerMainHorizontal != null && scrollerMainHorizontal != undefined && scrollerMainHorizontal.length > 0) {
        scrollerMainHorizontalHeight = scrollerMainHorizontal.height();
        var divEmpty = scrollerMainHorizontal.next();
        if (divEmpty != null && divEmpty != undefined && divEmpty.length > 0 && divEmpty[0].tagName.toUpperCase() == 'DIV' && divEmpty.html() == '') {
            divEmpty.css('display', 'none');
        }
    }

    var frozenHeaderRowCellHeight = 0
    if (headerDivTable != null && headerDivTable != undefined && headerDivTable.length > 0) {
        frozenHeaderRowCellHeight = headerDivTable.find('tr:eq(0)').find('th:eq(0)').height();
    }

    if (frozenheaderDiv != null && frozenheaderDiv != undefined && frozenheaderDiv.length > 0) {
        frozenheaderDiv.find('th:eq(0)').height(frozenHeaderRowCellHeight);
    }

    if (frozenBodyDiv != null && frozenBodyDiv != undefined && frozenBodyDiv.length > 0) {
        frozenBodyDiv.find('table').find('tbody').children('tr').each(function (index) {
            var bodyRows = $(bodyDivTable[0].rows[index]);
            var tr = $(this);
            if ((tr.attr('class') != null && tr.attr('class') != undefined && tr.attr('class') != '') && (tr.attr('class').toLowerCase() == 'xlnormalrow' || tr.attr('class').toLowerCase() == 'xlalternatingrow')) {// || $(this).attr('class').toLowerCase() == 'groupfooter' || $(this).attr('class').toLowerCase() == 'groupedrow'
                $(tr[0].cells[0]).outerHeight(bodyDivTable[0].rows[index].cells[0].offsetHeight);
            }
            //            if ((bodyRows.attr('class') != null && bodyRows.attr('class') != undefined && bodyRows.attr('class') != '') && bodyRows.attr('class').toLowerCase() == 'groupedrow') {
            //                //                var bodyCells = $(bodyRows[0].cells[0]);
            //                //                var colSpan = parseInt(bodyCells.attr('colSpan')) + 1;
            //                //                bodyCells.attr('colSpan', colSpan);
            //                var bodyCells = bodyRows[0].cells[0];
            //                bodyCells.colSpan = bodyCells.colSpan + 1;
            //            }
        });
    }

    var selectAllOptionDivHeight = 0;
    if (selectAllOptionDiv != null && selectAllOptionDiv != undefined && selectAllOptionDiv.length > 0) {
        selectAllOptionDivHeight = (selectAllOptionDiv.css('display').toLowerCase() == 'none' ? 0 : selectAllOptionDiv.height());
        selectAllOptionDiv.width(innerBodyDiv.width() - 20);
    }
    if (selectAllOptionFrozenDiv != null && selectAllOptionFrozenDiv != undefined && selectAllOptionFrozenDiv.length > 0) {
        selectAllOptionFrozenDiv.height(selectAllOptionDivHeight);
        selectAllOptionFrozenDiv.width(frozenBodyDiv.width() - 20);
    }

    var gridHeight = viewPortHeight - tdTopHeight - tdBottomHeight - upperHeaderDivHeight - headerDivHeight - footerDivHeight - panelTopHeight - panelBottomHeight - scrollerMainHorizontalHeight - selectAllOptionDivHeight - subHeight - 42;

    if (gridHeight <= 300) {
        gridHeight = 300;
    }

    if (innerBodyDiv != null && innerBodyDiv != undefined && innerBodyDiv.length > 0) {
        var innerBodyDivHeight = innerBodyDiv.height();
        if (innerBodyDivHeight >= gridHeight)
            innerBodyDiv.height(gridHeight);
        else
            gridHeight = innerBodyDivHeight;
    }
    var frozenBodyDivWidth = 0;
    if (frozenBodyDiv != null && frozenBodyDiv != undefined && frozenBodyDiv.length > 0) {
        frozenBodyDiv.height(gridHeight);
        //  frozenBodyDivWidth = frozenBodyDiv.width();
    }

    var scrollerMainVerticalWidth = 0;
    if (scrollerMainVertical != null && scrollerMainVertical != undefined && scrollerMainVertical.length > 0) {
        //scrollerMainVerticalWidth = scrollerMainVertical.width();
        scrollerMainVertical.height(gridHeight + headerDivHeight + selectAllOptionDivHeight - 5);
    }

    if (gridHeight <= 300) {
        $("#table").height(upperHeaderDivHeight + headerDivHeight + footerDivHeight + panelTopHeight + panelBottomHeight + scrollerMainHorizontalHeight + selectAllOptionDivHeight + subHeight + 300 + 5);
    }

    var grid = $find(gridID);
    if (grid != null)
        grid.get_GridInfo().Height = gridHeight;
}

