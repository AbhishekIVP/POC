function TopPanelSearchText(source, eventArgs) {
    var searchTxtControl = findSearchTextBox('txtTopSearchText');
    if (searchTxtControl != null) {
        if (searchTxtControl.value.indexOf(" : ") > -1)
            searchTxtControl.value = searchTxtControl.value.substring(searchTxtControl.value.indexOf(" : ") + 3);
    }
    OpenSecurityDetailsPopUp(eventArgs._value);
}

function OpenSecurityDetailsPopUp(sec_id) {
    SecMasterJSCommon.SMSCommons.openWindowForSecurity(false, sec_id, true, true, SecMasterJSCommon.SMCreateUpdateOpenByDefault.None, 0, null);
}

function findSearchTextBox(controlId) {
    var searchControl = null;
    var inputCtrl = document.getElementsByTagName("INPUT");
    for (var i = 0; i < inputCtrl.length; i++)
        if (inputCtrl[i].id.search(controlId) > -1) {
            searchControl = inputCtrl[i];
        }
    return searchControl;
}

//ReportName must be the last parameter
function SetIdOrUrl(identifier, application, url, extraParameters, event, ReportName) //For switching among the pages of lite amd main application
{
    var target = null;
    event = event || window.event;
    if (event != null)
        target = event.srcElement || event.target;
    var secondaryID = "";
    var customReportFrameworkId = "";
    var id = "", createSecurity = "";
    var extraParamsString = '';
    var fromTopMenu = false;

    if ($(target).hasClass('isChildren') || $(target).parent().hasClass('isChildren')) {
        fromTopMenu = true;
        leftMenu.setLinkColor($(event.currentTarget), true);
    }
    else if ($(target).hasClass('single') || $(target).parent().hasClass('single')) {
        fromTopMenu = true;
        $('.colorBlue').removeClass('colorBlue');
        $(event.currentTarget).addClass('colorBlue');
    }

    if (fromTopMenu) {
        $('#top-LeftMenuDiv').addClass('fully-collapsed');
        $('#top-OnHoverOverLay').hide();
    }

    if (extraParameters != null && extraParameters != undefined && extraParameters instanceof Array && extraParameters.length > 0) {
        for (var i = 0; i < extraParameters.length; i++) {
            eval(extraParameters[i].Value);
            extraParamsString += '&' + extraParameters[i].Key + '=' + objEval;
            if (SecMasterJSCommon.SMSCommons.contains(extraParamsString, "&secondaryID="))
                //if (extraParamsString.indexOf("&secondaryID=") != -1) 
                secondaryID = objEval;

            if (SecMasterJSCommon.SMSCommons.contains(extraParamsString, "&customReportFrameworkId="))
                customReportFrameworkId = objEval;
            if (SecMasterJSCommon.SMSCommons.contains(extraParamsString, "&id="))
                id = objEval;
            if (SecMasterJSCommon.SMSCommons.contains(extraParamsString, "&createSecurity="))
                createSecurity = objEval;
        }
    }


    if (application == 'main') //if page is for main application
    {
        if (sessionStorage['previous'] == 'main') {
            if (fromTopMenu) {
                $('.primaryTab').removeClass('hidden');
                $('#primaryFrame').prop('src', url + "?identifier=" + identifier + extraParamsString);
                $('#primaryFrame').on('load', function (e) {
                    $('.primaryTab').addClass('hidden');
                });
                leftMenu.createTab(identifier, '', '', '');
            }
            else {
                if (secondaryID != "")
                    SetViewWithSecondaryID(identifier, secondaryID);
                else if (id != "")
                    SetViewWithID(identifier, id)
                else if (createSecurity != "")
                    SetViewForCreateSecurity(createSecurity);
                else
                    SetViewForLeftMenu(identifier);
                return true;
            }
        }
        else if (sessionStorage['previous'] == 'lite') {
            if (target != null && target != undefined)
                target.href = '#';
            onUpdating();
            try {
                //                var path = window.location.protocol + '//' + window.location.host;
                //                var pathname = window.location.pathname.split('/');

                //                $.each(pathname, function (ii, ee) {
                //                    if ((ii !== 0) && (ii !== pathname.length - 1))
                //                        path = path + '/' + ee;
                //                });
                //                var s = url + "?identifier=" + identifier + extraParamsString;
                //                window.location.href = path + '/SMHomeNew.aspx?mode=7&url=' + s;
                leftMenu.createTab(identifier, url + "?identifier=" + identifier + extraParamsString, (typeof ReportName === 'string' && ReportName !== '' ? GetGUID() : ''), (typeof ReportName === 'string' ? ReportName : ''));
            }
            catch (e) {
                onUpdated();
                return false;
            }
            return false;
        }
        else if (sessionStorage['previous'] == 'iago') {
            if (target != null && target != undefined)
                target.href = '#';
            onUpdating();
            try {
                //                var path = window.location.protocol + '//' + window.location.host;
                //                var pathname = window.location.pathname.split('/');

                //                $.each(pathname, function (ii, ee) {
                //                    if ((ii !== 0) && (ii !== pathname.length - 1))
                //                        path = path + '/' + ee;
                //                });
                //                var s = url + "?identifier=" + identifier + extraParamsString;
                //                window.location.href = path + '/SMHomeNew.aspx?mode=7&url=' + s;
                window.location.href = url + "?identifier=" + identifier + extraParamsString;
            }
            catch (e) {
                onUpdated();
                return false;
            }
            return false;
        }
        else {
            if (fromTopMenu) {
                sessionStorage['previous'] = 'main';
                //                var path = window.location.protocol + '//' + window.location.host;
                //                var pathname = window.location.pathname.split('/');

                //                $.each(pathname, function (ii, ee) {
                //                    if ((ii !== 0) && (ii !== pathname.length - 1))
                //                        path = path + '/' + ee;
                //                });
                //                var s = url + "?identifier=" + identifier + extraParamsString;
                //                window.location.href = path + '/SMHomeNew.aspx?mode=7&url=' + s;
                //                window.location.href = url + "?identifier=" + identifier + extraParamsString;
                leftMenu.createTab(identifier, url + "?identifier=" + identifier + extraParamsString, (typeof ReportName === 'string' && ReportName !== '' ? GetGUID() : ''), (typeof ReportName === 'string' ? ReportName : ''));
            }
        }

    }
    else if (application == 'lite') //if aspx page from lite
    {
        onUpdating();
        try {
            if (sessionStorage['previous'] === 'iago') {
                var path = window.location.protocol + '//' + window.location.host;
                var pathname = window.location.pathname.split('/');

                $.each(pathname, function (ii, ee) {
                    if ((ii !== 0) && (ii !== pathname.length - 1))
                        path = path + '/' + ee;
                });
                var s = url + "?identifier=" + identifier + extraParamsString;
                window.location.href = path + '/SMHomeNew.aspx?url=' + s;
            }
            else {
                //                $('#primaryFrame').prop('src', url + "?identifier=" + identifier + extraParamsString);
                leftMenu.createTab(identifier, url + "?identifier=" + identifier + extraParamsString, (typeof ReportName === 'string' && ReportName !== '' ? GetGUID() : ''), (typeof ReportName === 'string' ? ReportName : ''));
            }
        }
        catch (e) {
            onUpdated();
            return false;
        }
        return false;
    }

    else if (application == 'iago') //if aspx page from iago
    {
        onUpdating();
        try {
            var path = window.location.protocol + '//' + window.location.host;
            var pathname = window.location.pathname.split('/');

            $.each(pathname, function (ii, ee) {
                if ((ii !== 0) && (ii !== pathname.length - 1))
                    path = path + '/' + ee;
            });
            window.location.href = path + '/Home.aspx#/page/' + identifier;
        }
        catch (e) {
            onUpdated();
            return false;
        }
        return false;
    }

}

function getQuerystring(key, default_) {
    if (default_ == null) default_ = "";
    key = key.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regex = new RegExp("[\\?&]" + key + "=([^&#]*)");
    var qs = regex.exec(window.location.href);
    if (qs == null)
        return default_;
    else
        return qs[1];
}

function SetIFrame(url) {
    var iframe = document.getElementById('iFMain');
    iframe.src = url;
    onUpdatingHomeLite();
    return false;
}

function SetIFrameFromFrame(url) {
    var iframe = parent.document.getElementById('iFMain');
    iframe.src = url;
    //return false;
}



function SetIFrameControlValue(controlName, controlValue) {
    var iframe = document.getElementById('iFMain');
    var childDOMDocument = (iframe.contentWindow || iframe.contentDocument);
    if (childDOMDocument.document)
        childDOMDocument = childDOMDocument.document;
    var element = childDOMDocument.getElementById(controlName);
    if (element)
        element.value = controlValue;
}

function SetView(identifier) {
    document.getElementById('identifier').value = identifier;
    //    if (event.srcElement != null) {
    //        document.getElementById('hdnAccordionSelected').value = event.srcElement.id;
    //    }
    //    __doPostBack('', '');
    return false;
}
function SetViewWithID(identifier, id) {
    //    document.getElementById(uIdentifierId).value = identifier;
    document.getElementById('identifier').value = identifier;
    document.getElementById('id').value = id;
    //document.getElementById('hdnAccordionSelected').value = event.srcElement.id;
    //__doPostBack('', '');
    return false;
}

function SetViewWithIDForCustomReports(identifier, id) {
    document.getElementById('identifier').value = identifier;
    //document.getElementById('id').value = id;
    document.getElementById('ctl00_hdnIdForCustomReportFramework').value = id;
    //document.getElementById('hdnAccordionSelected').value = event.srcElement.id;
    //__doPostBack('', '');
    return false;
}

function SetViewForLeftMenu(identifier) {
    //    document.getElementById(uIdentifierId).value = identifier;
    document.getElementById('identifier').value = identifier;
    if (event.srcElement != null) {
        document.getElementById('hdnAccordionSelected').value = event.srcElement.id;
    }
    //__doPostBack('', '');
    return true;
}

var gSearchPageId;
function SetViewForSearch(identifier) {
    ClearFields();
    if (gSearchPageId == 'Search' && identifier == 'Search') {
        document.getElementById('identifier').value = 'MainPanel';
        gSearchPageId = 'MainPanel';
    }
    else {
        document.getElementById('identifier').value = identifier;
        gSearchPageId = identifier;
    }
    if (event.srcElement != null) {
        document.getElementById('hdnAccordionSelected').value = event.srcElement.id;
    }
    //__doPostBack('', '');
    return true;
}

function SetViewWithControlID(identifier, id) {
    document.getElementById('identifier').value = identifier;
    var dropDown = document.getElementById(id);
    document.getElementById('id').value = dropDown.value;
    var selectedIndex = dropDown.selectedIndex;
    document.getElementById('secondaryID').value = dropDown.options[selectedIndex].text;
}
function SetViewWithIDAndSecondaryID(identifier, id, secondaryID) {
    document.getElementById('identifier').value = identifier;
    document.getElementById('id').value = id;
    document.getElementById('secondaryID').value = secondaryID;
}
function SetViewWithSecondaryID(identifier, secondaryID) {
    //    document.getElementById(uIdentifierId).value = identifier;
    document.getElementById('identifier').value = identifier;
    document.getElementById('secondaryID').value = secondaryID;
}
function ClearFields() {
    document.getElementById('identifier').value = '';
    var masterId = document.getElementById('masterId');
    if (masterId != null)
        masterId.value = '';
    var primaryName = document.getElementById('primaryName');
    if (primaryName != null)
        primaryName.value = '';
    var secondaryName = document.getElementById('secondaryName');
    if (secondaryName != null)
        secondaryName.value = '';
    var id = document.getElementById('id');
    if (id != null)
        id.value = '';
    var commandName = document.getElementById('commandName');
    if (commandName != null)
        commandName.value = '';
    var secondaryID = document.getElementById('secondaryID');
    if (secondaryID != null)
        secondaryID.value = '';
    //Reset Event of Attribute Management
    document.onmousemove = null;
    document.onmousedown = null;
    document.onmouseup = null;
}
function SetViewWithIDForFeedName(FeedNameID, txtBox, feedID, vendorId, feedType, vendorName, vendorTypeId) {
    var feedName = document.getElementById(FeedNameID).value;
    feedName = document.getElementById(txtBox).value;

    //var radio = document.getElementsByName(feedType);
    //    for (var ii = 0; ii < 3; ii++) {
    //        if (radio[0].cells[ii].children[0].checked)
    //            var selectedfeedType = radio[0].cells[ii].children[0].value;
    //    }
    var rblCells = $('#' + feedType).find('td');
    for (var count = 0; count < 3; count++) {
        if (rblCells.eq(count).children().eq(0).prop('checked').toString() == "true") {
            var selectedfeedType = rblCells.eq(count).children().eq(0)[0].value;
            break;
        }
    }
    var wizardType = "";
    var identifier = "";
    switch (selectedfeedType) {
        case "1": wizardType = "BULKUpstreamWizard"; identifier = "FeedSummary"; break;
        case "2": wizardType = "BULKMergableUpstreamWizard"; identifier = "FeedSummary"; break;
        case "3": wizardType = "MergedUpstreamWizard"; identifier = "FeedMerging"; break;
    }
    if (vendorTypeId == "3") {
        wizardType = "IdentifierWizard";
        identifier = "IdentifierFeedSummary";
    }
    var id = feedID + '|' + vendorId + '|' + feedName + '|' + selectedfeedType + '|' + wizardType + '|' + vendorName + '|' + vendorTypeId;
    document.getElementById('identifier').value = identifier;
    document.getElementById('id').value = id;

}

function SetViewWithIDForFeedName(FeedNameID, txtBox, feedID, vendorId, feedType, vendorName, vendorTypeId, module) {
    var feedName = document.getElementById(FeedNameID).value;
    feedName = document.getElementById(txtBox).value;

    //    var radio = document.getElementById(feedType);
    //    for (var ii = 0; ii < 3; ii++) {
    //        if (radio[0].cells[ii].childNodes[0].checked)
    //            var selectedfeedType = radio[0].cells[ii].childNodes[0].value;
    //    }
    var rblCells = $('#' + feedType).find('td');
    for (var count = 0; count < 3; count++) {
        if (rblCells.eq(count).children().eq(0).prop('checked').toString() == "true") {
            var selectedfeedType = rblCells.eq(count).children().eq(0)[0].value;
            break;
        }
    }
    var wizardType = "";
    var identifier = "";
    switch (selectedfeedType) {
        case "1": wizardType = "BULKUpstreamWizard"; identifier = "FeedSummary"; break;
        case "2": wizardType = "BULKMergableUpstreamWizard"; identifier = "FeedSummary"; break;
        case "3": wizardType = "MergedUpstreamWizard"; identifier = "FeedMerging"; break;
    }
    if (vendorTypeId == "3" && module != "CA") {
        wizardType = "IdentifierWizard";
        identifier = "IdentifierFeedSummary";
    }
    var id = feedID + '|' + vendorId + '|' + feedName + '|' + selectedfeedType + '|' + wizardType + '|' + vendorName + '|' + vendorTypeId + '|' + module;
    document.getElementById('identifier').value = identifier;
    document.getElementById('id').value = id;

}
/*************************************************************************************************
Generic Methods
*************************************************************************************************/
var ErrorMessage;
ErrorMessage += "\n\n|||||| JAVASCRIPT ERROR ||||||\n\n";
ErrorMessage += "There was an error on this page\n\n";
ErrorMessage += "Error Source & description: \n\n";
/************************************************************************************************
*   Function Name     : SetVisibility 
*   Author            : Manish Sharma
*   Description       : Sets the visibility of the Control
*   Page used in      : Generic
*   Parameters        : cntrlID - Control Client Id
*                       visFlg  - Visibility Flag (True Or False)
*   Modification Log  : 
*************************************************************************************************/
function SetVisibility(cntrlID, visFlg) {
    try {
        if (visFlg == true)
            document.getElementById(cntrlID).style.display = '';
        else if (visFlg == false)
            document.getElementById(cntrlID).style.display = 'none';
    }
    catch (err) {
        alert(ErrorMessage + 'SetVisibility \n\n ' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : AddIteminDDL 
*   Author            : Manish Sharma
*   Description       : Adding an item in Drop Down List
*   Page used in      : Generic
*   Parameters        : cntrlID - Control Client Id
*                       Text  - Text for item
*                       Value - Value for item
*   Modification Log  : 
*************************************************************************************************/
function AddIteminDDL(cntrlId, text, value) {
    try {
        var opt = document.createElement("option");
        document.getElementById(cntrlId).options.add(opt);
        opt.text = text;
        opt.value = value;
        SetValue(cntrlId, value);
    }
    catch (err) {
        alert(ErrorMessage + 'AddIteminDDL\n\n' + cntrlId + Text + Value + err.description);
    }
}
/************************************************************************************************
*   Function Name     : SetValue 
*   Author            : Manish Sharma
*   Description       : Sets the value of the Control
*   Page used in      : Generic
*   Parameters        : cntrlID     - Control Client Id
*                       cntrlValue  - Value to be set
*   Modification Log  : 
*************************************************************************************************/
function SetValue(cntrlID, cntrlValue) {
    try {
        document.getElementById(cntrlID).value = cntrlValue;
    }
    catch (err) {
        alert(ErrorMessage + 'SetValue \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : SetText 
*   Author            : Manish Sharma
*   Description       : Sets the text of the Control
*   Page used in      : Generic
*   Parameters        : cntrlID     - Control Client Id
*                       cntrlText   - Text to be set
*   Modification Log  : 
*************************************************************************************************/
function SetText(cntrlID, cntrlText) {
    try {
        var control = $("#" + cntrlID);

        if ($(control).is('input'))
            $(control).val(cntrlText);

        else
            $(control).text(cntrlText);
    }
    catch (err) {
        alert(ErrorMessage + 'SetText \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : CntrEnable 
*   Author            : Manish Sharma
*   Description       : Enables the control
*   Page used in      : Generic
*   Parameters        : cntrlID     - Control Client Id
*                       flg         - Value to be set
*   Modification Log  : 
*************************************************************************************************/

function CntrEnable(cntrlID, flg) {
    try {
        document.getElementById(cntrlID).disabled = flg;
    }
    catch (err) {
        alert(ErrorMessage + 'CntrEnable \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : GetRadiobuttonCheck 
*   Author            : Manish Sharma
*   Description       : returns the index of the checked item in radio button
*   Page used in      : Generic
*   Parameters        : rblClientID     - Control Client Id
*   Modification Log  : 
*************************************************************************************************/
function GetRadiobuttonCheck(rblClientID) {
    //For Browser Compatibility
    // btnCount = document.getElementById(rblClientID).rows[0].cells.length;
    btnCount = document.getElementById(rblClientID).rows[0].cells.length;
    try {
        //        var selectedValue = 0;
        //        var btnCount = $("#" + rblClientID).find("input[type='radio']");
        //        btnCount.each(function (index) {
        //            if ($(this).prop('checked')) {
        //                selectedValue = index;
        //                return false;
        //            }
        //   });
        var selectedValue = 0;
        for (var i = 0; i < btnCount; i++) {
            if (document.getElementById(rblClientID + '_' + i).checked == true) {
                selectedValue = i;
            }
        }
        return selectedValue;
    }
    catch (err) {
        alert(ErrorMessage + 'GetRadiobuttonCheck \n\n' + err.description);
    }
}

/************************************************************************************************
*   Function Name     : SetToolTipOfControl 
*   Author            : Shubham Mittal
*   Description       : Set The tooltip text for a control
*   Page used in      : Generic
*   Parameters        : cntrlIDs - Control Behaviour Id
*                       cntrlValues -  Text to be set.
*   Modification Log  : 
*************************************************************************************************/
function SetToolTipOfControl(cntrlIDs, cntrlValues) {
    try {
        var arraycntrlIDs = cntrlIDs.split('|');
        var arraycntrlValues = cntrlValues.split('|');
        var count = arraycntrlIDs.length;
        for (var i = 0; i < count; i++) {
            document.getElementById(arraycntrlIDs[i]).title = arraycntrlValues[i];
        }
    }
    catch (err) {
        alert(ErrorMessage + 'SetToolTipOfControl\n\n' + err.description);
    }

}

/************************************************************************************************
*   Function Name     : SetAllValue 
*   Author            : Manish Sharma
*   Description       : Sets the value of multiple Controls
*   Page used in      : Generic
*   Parameters        : cntrlIDs     - pipe ('|') separated Control Client Id
*                       cntrlValues  - pipe ('|') separated Value to be set
*   Modification Log  : 
*************************************************************************************************/
function SetAllValue(cntrlIDs, cntrlValues) {
    try {
        var arraycntrlIDs = cntrlIDs.split('|');
        var arraycntrlValues = cntrlValues.split('|');
        var count = arraycntrlIDs.length;
        for (var i = 0; i < count; i++) {
            SetValue(arraycntrlIDs[i], arraycntrlValues[i]);
        }
    }
    catch (err) {
        alert(ErrorMessage + 'SetAllValue \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : SetAllText 
*   Author            : Manish Sharma
*   Description       : Sets the text of multiple Control
*   Page used in      : Generic
*   Parameters        : cntrlIDs    - pipe ('|') separated Control Client Id
*                       cntrlTexts  - pipe ('|') separated text to be set
*   Modification Log  : 
*************************************************************************************************/

function SetAllText(cntrlIDs, cntrlTexts) {
    try {
        var arraycntrlIDs = cntrlIDs.split('|');
        var arraycntrlTexts = cntrlTexts.split('|');
        var count = arraycntrlIDs.length;
        for (var i = 0; i < count; i++) {
            SetText(arraycntrlIDs[i], arraycntrlTexts[i]);
        }
    }
    catch (err) {
        alert(ErrorMessage + 'SetAllText \n\n' + err.description);
    }
}

function SetMessagePopup(cntrlIDs, outcntrlIds) {
    try {
        var arraycntrlIDs = cntrlIDs.split('|');
        var arraycntrlTexts = outcntrlIds.split('|');
        var count = arraycntrlIDs.length;
        for (var i = 0; i < count; i++) {
            // GetObject(arraycntrlIDs[i]).innerText = GetObject(arraycntrlTexts[i]).innerText;

            var control = $("#" + arraycntrlIDs[i]);
            var inControl = $("#" + arraycntrlTexts[i]);
            var value;

            if ($(inControl).is('input'))
                value = $(inControl).val();
            else
                value = $(inControl).textNBR();

            if ($(control).is('input'))
                $(control).val(value);
            else
                $(control).text(value);
        }
    }
    catch (err) {
        alert(ErrorMessage + 'SetMessagePopup \n\n' + err.description);
    }
}

/************************************************************************************************
*   Function Name     : ClearDDL 
*   Author            : Manish Sharma
*   Description       : Clears the items from the drop down list
*   Page used in      : Generic
*   Parameters        : cntrlID     - Control Id
*   Modification Log  : 
*************************************************************************************************/
function ClearDDL(cntrlID) {
    try {
        var count = document.getElementById(cntrlID).options.length;
        for (var i = count; i > -1; i--) {
            document.getElementById(cntrlID).options.remove(i);
        }
    }
    catch (err) {
        alert(ErrorMessage + 'ClearDDL \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : ClearAllDDL 
*   Author            : Manish Sharma
*   Description       : Clears the items from multiple drop down list
*   Page used in      : Generic
*   Parameters        : cntrlIDs     - pipe ('|') separated Control Client Id
*   Modification Log  : 
*************************************************************************************************/
function ClearAllDDL(cntrlIDs) {
    try {
        var arraycntrlIDs = cntrlIDs.split('|');
        var count = arraycntrlIDs.length;
        for (var i = 0; i < count; i++) {
            ClearDDL(arraycntrlIDs[i]);
        }
    }
    catch (err) {
        alert(ErrorMessage + 'ClearAllDDL\n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : EnableAllCntrls 
*   Author            : Manish Sharma
*   Description       : Enables multiple controls
*   Page used in      : Generic
*   Parameters        : cntrlIDs     - pipe ('|') separated Control Client Id
*   Modification Log  : 
*************************************************************************************************/
function EnableAllCntrls(cntrlIDs) {
    try {
        var arraycntrlIDs = cntrlIDs.split('|');
        var count = arraycntrlIDs.length;
        for (var i = 0; i < count; i++) {
            CntrEnable(arraycntrlIDs[i], false);
        }
    }
    catch (err) {
        alert(ErrorMessage + 'EnableAllCntrls\n\n' + err.description);
    }

}
/************************************************************************************************
*   Function Name     : DisableAllCntrls 
*   Author            : Manish Sharma
*   Description       : Disables multiple controls
*   Page used in      : Generic
*   Parameters        : cntrlIDs     - pipe ('|') separated Control Client Id
*   Modification Log  : 
*************************************************************************************************/
function DisableAllCntrls(cntrlIDs) {
    try {
        var arraycntrlIDs = cntrlIDs.split('|');
        var count = arraycntrlIDs.length;
        for (var i = 0; i < count; i++) {
            CntrEnable(arraycntrlIDs[i], true);
        }
    }
    catch (err) {
        alert(ErrorMessage + 'DisableAllCntrls \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : SetAllVisible 
*   Author            : Manish Sharma
*   Description       : Set the visibility of multiple controls to True
*   Page used in      : Generic
*   Parameters        : cntrlIDs     - pipe ('|') separated Control Client Id
*   Modification Log  : 
*************************************************************************************************/
function SetAllVisible(cntrlIDs) {
    try {
        var arraycntrlIDs = cntrlIDs.split('|');
        var count = arraycntrlIDs.length;
        for (var i = 0; i < count; i++) {
            SetVisibility(arraycntrlIDs[i], true);
        }
    }
    catch (err) {
        alert(ErrorMessage + 'SetAllVisible \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : SetAllInvisible 
*   Author            : Manish Sharma
*   Description       : Set the visibility of multiple controls to False
*   Page used in      : Generic
*   Parameters        : cntrlIDs     - pipe ('|') separated Control Client Id
*   Modification Log  : 
*************************************************************************************************/
function SetAllInvisible(cntrlIDs) {
    try {
        var arraycntrlIDs = cntrlIDs.split('|');
        var count = arraycntrlIDs.length;
        for (var i = 0; i < count; i++) {
            SetVisibility(arraycntrlIDs[i], false);
        }
    }
    catch (err) {
        alert(ErrorMessage + 'SetAllInvisible \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : HideShowModalPopup 
*   Author            : Manish Sharma
*   Description       : Hide/Show modal pop up
*   Page used in      : Generic
*   Parameters        : modalpopupID     - Control Behaviour Id
*                       flg  -  Visibility Flag
*   Modification Log  : 
*************************************************************************************************/
function HideShowModalPopup(modalpopupID, flg) {
    try {
        if (flg)
            $find(modalpopupID).show();
        else
            $find(modalpopupID).hide();
    }
    catch (err) {
        alert(ErrorMessage + 'HideShowModalPopup\n\n' + err.description);
    }

}

//To disable parent iframes also incase of status screens
function HideShowModalPopup_setAlldivOverlay(modalpopupID, flg, gridIdToRefresh) {
    try {
        if (flg) {
            //Add a div and added class of modalbackground in it
            $(window.frameElement).parents('.divWrapperContainer').append("<div id='disableBehindDivs' class='modalBackground' style='width:100%;height:10%;'></div>");

        }
        else {
            $find(modalpopupID).hide();
            //Remove  div and added class of modalbackground in it
            $(window.frameElement).parents('.divWrapperContainer').find("div").remove('#disableBehindDivs');
        }
    }
    catch (err) {
        alert(ErrorMessage + 'HideShowModalPopup\n\n' + err.description);
    }

}
/************************************************************************************************
*   Function Name     : SetRblValue 
*   Author            : Manish Sharma
*   Description       : Sets the value of the Radio button list
*   Page used in      : Generic
*   Parameters        : rblCntrlID     - Radion button Control Client Id
*                       SelectedValue  - Value to be set
*   Modification Log  : 
*************************************************************************************************/
function SetRblValue(rblCntrlID, SelectedValue) {
    try {
        document.getElementById(rblCntrlID + '_' + SelectedValue).checked = true;
    }
    catch (err) {
        alert(ErrorMessage + 'SetRblValue \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : SetAllRblValue 
*   Author            : Manish Sharma
*   Description       : Sets the value of multiple Radio button list
*   Page used in      : Generic
*   Parameters        : cntrlIDs     - pipe ('|') separated Control Client Id
*                       cntrlVals    - pipe ('|') separated Control Value to be set
*   Modification Log  : 
*************************************************************************************************/
function SetAllRblValue(cntrlIDs, cntrlVals) {
    try {
        var arraycntrlIDs = cntrlIDs.split('|');
        var arraycntrlValues = cntrlVals.split('|');
        var count = arraycntrlIDs.length;
        for (var i = 0; i < count; i++) {
            SetRblValue(arraycntrlIDs[i], arraycntrlValues[i]);
        }
    }
    catch (err) {
        alert(ErrorMessage + 'SetAllRblValue \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : ClearCheckBoxList 
*   Author            : Manish Sharma
*   Description       : Clears the selection of the check box list
*   Page used in      : Generic
*   Parameters        : cntrlID     - Control Id
*   Modification Log  : 
*************************************************************************************************/

function ClearCheckBoxList(cntrlID) {
    try {
        var count = document.getElementById(cntrlID).cells.length;
        for (var i = 0; i < count; i++) {
            document.getElementById(cntrlID + '_' + i).checked = false;
        }
    }
    catch (err) {
        alert(ErrorMessage + 'ClearCheckBoxList \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : setCheckListValue 
*   Author            : Manish Sharma
*   Description       : Sets the value of the  check box list
*   Page used in      : Generic
*   Parameters        : cntrlID     - Control Client Id
*                       cntrlValues  - Values to be set
*   Modification Log  : 
*************************************************************************************************/
function setCheckListValue(cntrlID, cntrlValues) {
    try {
        var arrayValues = cntrlValues.split('|');
        var count = document.getElementById(cntrlID).cells.length;
        for (var i = 0; i < count; i++) {
            if (arrayValues[i] == 1) {
                document.getElementById(cntrlID).cells[i].children[0].checked = true;
            }
            else {
                document.getElementById(cntrlID).cells[i].children[0].checked = false;
            }
        }
    }
    catch (err) {
        alert(ErrorMessage + 'setCheckListValue \n\n' + err.description);
    }

}
/************************************************************************************************
*   Function Name     : setCheckBoxValue 
*   Author            : Manish Sharma
*   Description       : Checks / Unchecks the Check box control
*   Page used in      : Generic
*   Parameters        : cntrlID     - Control Client Id
*                       cntrlValue  - Value to be set
*   Modification Log  : 
*************************************************************************************************/
function setCheckBoxValue(cntrlID, cntrlValue) {
    try {
        if (cntrlValue == 1) {
            document.getElementById(cntrlID).checked = true;
        }
        else {
            document.getElementById(cntrlID).checked = false;
        }

    }
    catch (err) {
        alert(ErrorMessage + 'setCheckListValue \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : ClearCntrlsValue 
*   Author            : Manish Sharma
*   Description       : Clears the value of Control
*   Page used in      : Generic
*   Parameters        : cntrlIDs     - pipe ('|') separated Control Client Id
*   Modification Log  : 
*************************************************************************************************/
function ClearCntrlsValue(cntrlIDs) {
    try {
        var arraycntrlIDs = cntrlIDs.split('|');
        var count = arraycntrlIDs.length;
        for (var i = 0; i < count; i++) {
            SetValue(arraycntrlIDs[i], '');
        }

    }
    catch (err) {
        alert(ErrorMessage + 'ClearCntrlsValue \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : ClearCntrlText 
*   Author            : Manish Sharma
*   Description       : Clears the text of Control
*   Page used in      : Generic
*   Parameters        : cntrlIDs     - pipe ('|') separated Control Client Id
*   Modification Log  : 
*************************************************************************************************/
function ClearCntrlText(cntrlIDs) {
    try {
        var arraycntrlIDs = cntrlIDs.split('|');
        var count = arraycntrlIDs.length;
        for (var i = 0; i < count; i++) {
            SetText(arraycntrlIDs[i], '');
        }

    }
    catch (err) {
        alert(ErrorMessage + 'ClearCntrlsValue \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : AddItemsinDDL 
*   Author            : Manish Sharma
*   Description       : Adds multiple items in Drop down list
*   Page used in      : Generic
*   Parameters        : cntrlId     - Control Client Id
*                       TextList    - list of the item text
*                       ValueList   - list of the item value
*   Modification Log  : 
*************************************************************************************************/
function AddItemsinDDL(cntrlId, TextList, ValueList) {
    try {
        var arrayTextList = TextList.split('|');
        var arrayValueList = ValueList.split('|');
        var count = arrayTextList.length;
        for (var i = 0; i < count; i++) {
            AddIteminDDL(cntrlId, arrayTextList[i], arrayValueList[i]);
        }
    }
    catch (err) {
        alert(ErrorMessage + 'AddItemsinDDL\n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : RemoveValueDDL 
*   Author            : Manish Sharma
*   Description       : removes the value from drop down list
*   Page used in      : Generic
*   Parameters        : cntrlID         - Control Client Id
*                       SelectedValue   - Value to be removed
*   Modification Log  : 
*************************************************************************************************/
function RemoveValueDDL(cntrlID, SelectedValue) {
    var selectedIndex = GetElementIndex(cntrlID, SelectedValue);
    if (selectedIndex > -1) {
        document.getElementById(cntrlID).options.remove(selectedIndex);
    }
}
/************************************************************************************************
*   Function Name     : GetElementIndex 
*   Author            : Manish Sharma
*   Description       : returns the index of the selected value
*   Page used in      : Generic
*   Parameters        : cntrlID         - Control Client Id
*                       SelectedValue   - value whose index is desired
*   Modification Log  : 
*************************************************************************************************/
function GetElementIndex(cntrlID, SelectedValue) {
    var index = -1;
    var count = document.getElementById(cntrlID).options.length;
    for (var i = 0; i < count; i++) {
        if (document.getElementById(cntrlID).options[i].value == SelectedValue) {
            index = i;
            break;
        }
        continue;
    }
    return index;
}
/************************************************************************************************
*   Function Name     : CompareTimeFromCurrent 
*   Author            : Manish Sharma
*   Description       : Compares the time from current time and returns error in case 
*                       provided time is greater than current time
*   Page used in      : Generic
*   Parameters        : inputTime - time to be validated
*                       lblTime  - label of the control
*   Modification Log  : 
*************************************************************************************************/
function CompareTimeFromCurrent(startdate, inputTime, lblTime) {
    var todaysdate = new Date();
    try {
        if (!com.ivp.rad.rscriptutils.RSValidators.checkDates(null, startdate, inputTime, null, true))
            return '? Please enter ' + lblTime + 'greater than or equal to Current Time\n ';
        else {
            return '';
        }
    }
    catch (err) {
        alert(ErrorMessage + 'CompareDate \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : CompareDateFromTodaysDate 
*   Author            : Manish Sharma
*   Description       : Compares the date from current date and returns error in case the
*                       provided date is greater than current date
*   Page used in      : Generic
*   Parameters        : inputDate - Date to be validated
*                       lblDate  - label of the control
*   Modification Log  : 
*************************************************************************************************/
function CompareDateFromTodaysDate(inputDate, lblDate) {
    try {
        if (!CheckDateWithCurrentDate(inputDate)) {
            return '? Please enter ' + lblDate + 'less than or equal to Todays Date\n ';
        }
        else {
            return '';
        }
    }
    catch (err) {
        alert(ErrorMessage + 'CompareDate \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : CompareDateFromTodaysDateForGreater 
*   Author            : Manish Sharma
*   Description       : Compares the date from current date and returns error in case the
*                       provided date is Less than current date
*   Page used in      : Generic
*   Parameters        : inputDate - Date to be validated
*                       lblDate  - label of the control
*   Modification Log  : 
*************************************************************************************************/
function CompareDateFromTodaysDateForGreater(inputDate, lblDate) {
    try {
        if (!com.ivp.rad.rscriptutils.RSValidators.checkDates(null, inputDate, null, null, true)) {
            return '? Please enter ' + lblDate + 'greater than or equal to Todays Date\n ';
        }
        else {
            return '';
        }
    }
    catch (err) {
        alert(ErrorMessage + 'CompareDate \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : CompareDate 
*   Author            : Manish Sharma
*   Description       : Compares start date and end date and returns error if end date is less
*                       than start date
*   Page used in      : Generic
*   Parameters        : startdate - start date 
*                       enddate  - end date
*   Modification Log  : 
*************************************************************************************************/
function CompareDate(startdate, enddate) {
    try {
        if (!com.ivp.rad.rscriptutils.RSValidators.checkDates(null, startdate, null, enddate, false)) {
            return '? Please enter End Date greater than or equal to Start Date\n ';
        }
        else {
            return '';
        }
    }
    catch (err) {
        alert(ErrorMessage + 'CompareDate \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : RequirefieldValidator 
*   Author            : Manish Sharma
*   Description       : Checks for the value in controls
*   Page used in      : Generic
*   Parameters        : cntrlIDs - pipe ('|') separated Control Client Id
*   Modification Log  : 
*************************************************************************************************/
function RequirefieldValidator(cntrlIDs) {
    try {
        var arraycntrlIDs = cntrlIDs.split('|');
        var errmsg = '';
        var count = arraycntrlIDs.length;
        for (var i = 0; i < count; i++) {
            if ((document.getElementById(arraycntrlIDs[i]).type == 'text' || document.getElementById(arraycntrlIDs[i]).type == 'textarea') && document.getElementById(arraycntrlIDs[i]).value == '') {
                errmsg += '? ' + document.getElementById(arraycntrlIDs[i]).title + '\n ';
            }
            else if (document.getElementById(arraycntrlIDs[i]).type == 'select-one' && document.getElementById(arraycntrlIDs[i]).value == '--Select One--') {
                errmsg += '? ' + document.getElementById(arraycntrlIDs[i]).title + '\n ';
            }
        }
        return errmsg;

    }
    catch (err) {
        alert(ErrorMessage + 'RequirefieldValidator \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : isValidTime 
*   Author            : Manish Sharma
*   Description       : Validates Time
*   Page used in      : Generic
*   Parameters        : value - time string
*   Modification Log  : 
*************************************************************************************************/
function isValidTime(value) {
    var colonCount = 0;
    var hasMeridian = false;
    for (var i = 0; i < value.length; i++) {
        var ch = value.substring(i, i + 1);
        if ((ch < '0') || (ch > '9')) {
            if ((ch != ':') && (ch != ' ') && (ch != 'a') && (ch != 'A') && (ch != 'p') && (ch != 'P') && (ch != 'm') && (ch != 'M')) {
                return false;
            }
        }
        if (ch == ':') { colonCount++; }
        if ((ch == 'p') || (ch == 'P') || (ch == 'a') || (ch == 'A')) { hasMeridian = true; }
    }
    if ((colonCount < 1) || (colonCount > 2)) { return false; }
    var hh = value.substring(0, value.indexOf(":"));
    if ((parseFloat(hh) < 0) || (parseFloat(hh) > 23)) { return false; }
    if (hasMeridian) {
        if ((parseFloat(hh) < 1) || (parseFloat(hh) > 12)) { return false; }
    }
    if (colonCount == 2) {
        var mm = value.substring(value.indexOf(":") + 1, value.lastIndexOf(":"));
    } else {
        var mm = value.substring(value.indexOf(":") + 1, value.length);
    }
    if ((parseFloat(mm) < 0) || (parseFloat(mm) > 59)) { return false; }
    if (colonCount == 2) {
        var ss = value.substring(value.lastIndexOf(":") + 1, value.length);
    } else {
        var ss = "00";
    }
    if ((parseFloat(ss) < 0) || (parseFloat(ss) > 59)) { return false; }
    return true;
}
/************************************************************************************************
*   Function Name     : PanelVisibility 
*   Author            : Manish Sharma
*   Description       : sets the visibility of the panel
*   Page used in      : Generic
*   Parameters        : cntrlID - Control Client Id
*                       Visibility - visibility flag    
*   Modification Log  : 
*************************************************************************************************/
function PanelVisibility(cntrlID, Visibility) {
    try {
        if (trim(Visibility.toLowerCase()) == 'true')
            $find(cntrlID).collapsePanel();
        else
            $find(cntrlID).expandPanel();
    }
    catch (err) {
        alert(ErrorMessage + 'PanelVisibility \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : AllPanelVisibility 
*   Author            : Manish Sharma
*   Description       : sets the visibility of multiple panel
*   Page used in      : Generic
*   Parameters        : cntrlID - Control Client Id
*                       Visibility - visibility flag 
*   Modification Log  : 
*************************************************************************************************/
//function AllPanelVisibility(cntrlsIDList, Visibility) {
//    try {
//        var arraycntrlsID = cntrlsIDList.split('|');
//        var count = arraycntrlsID.length;
//        var i;
//        if (trim(Visibility.toLowerCase()) == 'true')
//            for (i = 0; i < count; i++)
//                $find(arraycntrlsID[i]).collapsePanel();
//        else
//            for (i = 0; i < count; i++)
//                $find(arraycntrlsID[i]).expandPanel();
//    }
//    catch (err) {
//        alert(ErrorMessage + 'AllPanelVisibility \n\n' + err.description);
//    }
//}
/************************************************************************************************
*   Function Name     : ClearListBox 
*   Author            : Manish Sharma
*   Description       : Clears the List box
*   Page used in      : Generic
*   Parameters        : lsbCntrl - Control Client Id
*   Modification Log  : 
*************************************************************************************************/
function ClearListBox(lsbCntrl) {
    try {
        var length = document.getElementById(lsbCntrl).options.length;
        for (var i = 0; i < length; i++) {
            document.getElementById(lsbCntrl).remove(i);
        }
    }
    catch (Error) {
        alert(ErrorMessage + 'ClearListBox \n\n' + err.description);
    }
}
/************************************************************************************************
*   Function Name     : ChangeToolTip 
*   Author            : Manish Sharma
*   Description       : changes the tool tip for a control
*   Page used in      : Generic
*   Parameters        : cntrlID - Control Client Id
*                       tooltip - tool tip 
*   Modification Log  : 
*************************************************************************************************/
function ChangeToolTip(cntrlID, tooltip) {
    try {
        document.getElementById(cntrlID).title = tooltip;
    }
    catch (Error) {
        alert(ErrorMessage + 'ChangeToolTip \n\n' + err.description);
    }
}
/*************************************************************************************************
FlowSetup Javascript Functions  
*************************************************************************************************/
function SetTaskPanel(panelID, btnID, hidbtnName, hidVisibility, btnName) {
    try {
        SetVisibility(panelID, true);
        SetValue(btnID, btnName);
        SetValue(hidbtnName, btnName);
        SetValue(hidVisibility, '1');
    }
    catch (err) {
        alert(ErrorMessage + 'SetTaskPane \n\n' + err.description);
    }
}
function SetTaskTrigger(rblID, preTaskLabel, perTaskrdl,
    modalpopUpId, recRblID, popContentPanel, btnViewSchedule, ischained) {
    try {
        var selectedValue = GetRadiobuttonCheck(rblID);
        switch (selectedValue) {
            case 0:
                if (ischained = true) {
                    SetVisibility(preTaskLabel, false);
                    SetVisibility(perTaskrdl, false);
                }
                SetVisibility(btnViewSchedule, false);
                $find(modalpopUpId).hide();
                break;
            case 1:
                if (ischained = true) {
                    SetVisibility(preTaskLabel, false);
                    SetVisibility(perTaskrdl, false);
                }
                SetRecurrence(recRblID, popContentPanel, modalpopUpId);
                $find(modalpopUpId).hide();
                $find(modalpopUpId).show();
                SetVisibility(btnViewSchedule, true);
                break;
            case 2:
                if (ischained = true) {
                    SetVisibility(preTaskLabel, true);
                    SetVisibility(perTaskrdl, true);
                }
                SetVisibility(btnViewSchedule, false);
                $find(modalpopUpId).hide();
                break;
            default:
                break;
        }
    }
    catch (err) {
        alert(ErrorMessage + 'SetTaskTrigger \n\n' + err.description);
    }

}
function SetRecurrencePattern(rblID, lblIntervalID, lblDaysofWeekId, chklDaysofWeekId) {

    try {
        var selectedValue = GetRadiobuttonCheck(rblID);
        switch (selectedValue) {
            case 0:
                SetText(lblIntervalID, 'Daily Interval :');
                SetVisibility(chklDaysofWeekId, false);
                SetVisibility(lblDaysofWeekId, false);

                break;
            case 1:
                SetText(lblIntervalID, 'Weekly Interval :');
                SetVisibility(chklDaysofWeekId, true);
                SetVisibility(lblDaysofWeekId, true);
                break;
            case 2:
                SetText(lblIntervalID, 'Monthly Interval :');
                SetVisibility(chklDaysofWeekId, false);
                SetVisibility(lblDaysofWeekId, false);
                break;
            default:
                break;

        }
    }
    catch (err) {
        alert(ErrorMessage + 'SetRecurrencePattern \n\n' + err.description);
    }
}
function SetNoEndDate(chkboxID, txtEndDateID) {
    try {
        if (document.getElementById(chkboxID).checked == true) {
            SetValue(txtEndDateID, '');
            CntrEnable(txtEndDateID, true);
            //  CntrEnable(calEndDateID,true);
        }
        else {
            SetValue(txtEndDateID, '');
            CntrEnable(txtEndDateID, false);
            //  CntrEnable(calEndDateID,false);
        }
    }
    catch (err) {
        alert(ErrorMessage + 'SetNoEndDate\n\n' + err.description);
    }
}
function SetRecurrence(rblID, ContentPanelID, popupID) {
    try {
        var selectedValue = GetRadiobuttonCheck(rblID);
        switch (selectedValue) {
            case 0:
                SetVisibility(ContentPanelID, true);
                //document.getElementById(ContentPanelID).style.height = '190px';
                break;
            case 1:
                SetVisibility(ContentPanelID, false);
                //document.getElementById(ContentPanelID).style.height = '0px';
                break;
            default:
                break;

        }
        $find(popupID).hide();
        $find(popupID).show();
    }
    catch (err) {
        alert(ErrorMessage + 'SetRecurrence \n\n ' + err.description);
    }
}
function ValidateTaskDetailScheduleData(CntrlIDs, CntrllblIds, CntrlErrorIds, CntrlTypes,
 rblstRecurrence, rblstRecurrencePattern, chkNoEndDate, lblValidationSummary, txtIsValidated) {
    try {
        var arrayCntrlsID = CntrlIDs.split('|');
        var arrayCntrllblID = CntrllblIds.split('|');
        var arrayCntrlErrorIDs = CntrlErrorIds.split('|');
        var arrayCntrlTypes = CntrlTypes.split('|');
        var count = arrayCntrlsID.length;
        var errorcount = 0;
        var errmsgs = '';
        var recurrencetype = GetRadiobuttonCheck(rblstRecurrence);
        var recurrencepattern = GetRadiobuttonCheck(rblstRecurrencePattern);
        var chkDaysofweek = '';
        var lblStartDate = '';
        var lblEndDate = ''
        SetText(lblValidationSummary, '');
        for (var i = 0; i < count; i++) {
            var startdate;
            if (arrayCntrlTypes[i] == 'DaysOfWeek' && recurrencepattern == 1 && recurrencetype == 0) {
                //chkDaysofweek = GetRadiobuttonCheck(arrayCntrlsID[i]);

                // Check which day of week is selected
                chkDaysofweek = '';
                btnCount = document.getElementById(arrayCntrlsID[i]).cells.length;
                // var selectedValue = 0;
                for (var cellCount = 0; cellCount < btnCount; cellCount++) {
                    if (document.getElementById(arrayCntrlsID[i] + '_' + cellCount).checked == true) {
                        chkDaysofweek = cellCount;
                    }
                }

                //check if any day of week is selected
                if (chkDaysofweek === '') {
                    SetAllVisible(arrayCntrlErrorIDs[i]);
                    errmsgs += '? Please select atleast one day of the week\n';
                    errorcount++;
                }
                else {
                    SetAllInvisible(arrayCntrlErrorIDs[i]);
                }
            }
            if (arrayCntrlTypes[i] == 'StartDate' || arrayCntrlTypes[i] == 'Time'
            || (arrayCntrlTypes[i] == 'EndDate' && recurrencetype == 0 && document.getElementById(chkNoEndDate).checked == false)) {
                if (arrayCntrlTypes[i] == 'StartDate') {
                    lblStartDate = arrayCntrlsID[i];
                    startdate = (document.getElementById(arrayCntrlsID[i]).value);
                }
                else if (arrayCntrlTypes[i] == 'EndDate')
                    lblEndDate = arrayCntrlsID[i];
                if (document.getElementById(arrayCntrlsID[i]).value == '') {
                    SetAllVisible(arrayCntrlErrorIDs[i]);
                    errmsgs += '? Please enter ' + (document.getElementById(arrayCntrllblID[i]).innerText).replace(':', '') + '\n'
                    errorcount++;
                }
                else {
                    SetAllInvisible(arrayCntrlErrorIDs[i]);
                }
                if ((arrayCntrlTypes[i] == 'StartDate' || arrayCntrlTypes[i] == 'EndDate') && errorcount == 0) {
                    msg = CompareDateFromTodaysDateForGreater(document.getElementById(arrayCntrlsID[i]).value, (document.getElementById(arrayCntrllblID[i]).innerText).replace(':', ''));
                    if (msg != '') {
                        SetAllVisible(arrayCntrlErrorIDs[i]);
                        errmsgs += msg;
                        errorcount++;
                    }
                    else {
                        SetAllInvisible(arrayCntrlErrorIDs[i]);
                    }
                }
                if (arrayCntrlTypes[i] == 'Time' && errorcount == 0) {
                    isvalid = isValidTime(document.getElementById(arrayCntrlsID[i]).value)
                    if (isvalid != true) {
                        errorcount++;
                        SetAllVisible(arrayCntrlErrorIDs[i]);
                        errmsgs += '? Please enter valid ' + (document.getElementById(arrayCntrllblID[i]).innerText).replace(':', '') + '\n'
                    }
                    else {
                        msg = CompareTimeFromCurrent(startdate, document.getElementById(arrayCntrlsID[i]).value, (document.getElementById(arrayCntrllblID[i]).innerText).replace(':', ''));
                        if (msg != '') {
                            SetAllVisible(arrayCntrlErrorIDs[i]);
                            errmsgs += msg;
                            errorcount++;
                        }
                        else {
                            SetAllInvisible(arrayCntrlErrorIDs[i]);
                        }
                    }
                }
                if (arrayCntrlTypes[i] == 'EndDate' && errorcount == 0) {
                    msg = CompareDate(document.getElementById(lblStartDate).value, document.getElementById(lblEndDate).value);
                    if (msg != '') {
                        SetAllVisible(arrayCntrlErrorIDs[i]);
                        errmsgs += msg;
                        errorcount++;
                    }
                    else {
                        SetAllInvisible(arrayCntrlErrorIDs[i]);
                    }
                }
            }
            if (arrayCntrlTypes[i] == 'Text' && recurrencetype == 0) {
                if (document.getElementById(arrayCntrlsID[i]).value == '') {
                    SetAllVisible(arrayCntrlErrorIDs[i]);
                    errmsgs += '? Please enter ' + (document.getElementById(arrayCntrllblID[i]).innerText).replace(':', '') + '\n'
                    errorcount++;
                }
                else {
                    SetAllInvisible(arrayCntrlErrorIDs[i]);
                }

            }
            if (arrayCntrlTypes[i] == 'TextNoofRec' && recurrencetype == 0) {
                if (document.getElementById(arrayCntrlsID[i]).value == '') {
                    SetAllVisible(arrayCntrlErrorIDs[i]);
                    errmsgs += '? Please enter ' + (document.getElementById(arrayCntrllblID[i]).innerText).replace(':', '') + '\n'
                    errorcount++;
                }
                else if (document.getElementById(arrayCntrlsID[i]).value < 1 || document.getElementById(arrayCntrlsID[i]).value > 1439) {
                    SetAllVisible(arrayCntrlErrorIDs[i]);
                    errmsgs += '? Please enter ' + (document.getElementById(arrayCntrllblID[i]).innerText).replace(':', '') + ' between 1 and 1439' + '\n'
                    errorcount++;
                }
                else {
                    SetAllInvisible(arrayCntrlErrorIDs[i]);
                }
            }
        }
        if (errorcount > 0) {
            SetText(lblValidationSummary, errmsgs);
            SetText(txtIsValidated, '');
            return false;
        }
        else {
            SetText(lblValidationSummary, errmsgs);
            SetText(txtIsValidated, '1');
            HideShowModalPopup('modalScheduledJob', false);
        }
    }
    catch (err) {
        alert(ErrorMessage + 'ValidateTaskDetailScheduleData \n\n' + err.description);
    }
}
function ValidateFlowTaskDetails(cntrlsID, lblIDs, errorLblIDs, rblsID,
 lblValidationSummary, ddlPreTask, lblPreTaskError, lblPreTask, txtIsValidated) {
    try {
        var arraycntrlsID = cntrlsID.split('|');
        var arraylblID = lblIDs.split('|');
        var arrayerrorLblIDs = errorLblIDs.split('|');
        var count = arraycntrlsID.length;
        var errorcount = 0;
        var errmsgs = '';
        var selectedvalue = GetRadiobuttonCheck(rblsID);
        for (var i = 0; i < count; i++) {
            if (document.getElementById(arraycntrlsID[i]).value == '-1') {
                SetAllVisible(arrayerrorLblIDs[i]);
                errmsgs += '? Please select ' + (document.getElementById(arraylblID[i]).innerText).replace(':', '') + '\n'
                errorcount++;
            }
            else {
                SetAllInvisible(arrayerrorLblIDs[i]);
            }
        }
        if (selectedvalue == '1') {
            if (document.getElementById(txtIsValidated).value == '') {
                errmsgs += '? Please update Schedule Information.\n';
                errorcount++;
            }
        }
        if (selectedvalue == '2') {
            if (document.getElementById(ddlPreTask).value == '-1') {
                SetAllVisible(lblPreTaskError);
                errmsgs += '? Please select ' + (document.getElementById(lblPreTask).innerText).replace(':', '') + '\n'
                errorcount++;
            }
            else {
                SetAllInvisible(lblPreTaskError);
            }

        }
        else {
            SetAllInvisible(lblPreTaskError);
        }
        if (errorcount > 0) {
            SetText(lblValidationSummary, errmsgs);
            return false;
        }
        else {
            SetText(lblValidationSummary, errmsgs);
            return true;
        }
    }
    catch (err) {
        alert(ErrorMessage + 'ValidateFlowTaskDetails \n\n' + err.description);
    }
}
/*************************************************************************************************
Vendor Prioritization Flow SetUp 
************************************************************************************************/
function ValidateVendorFlowTaskDetails(cntrlsID, lblIDs, errorLblIDs, rblsID,
 lblValidationSummary, txtIsValidated) {
    try {
        var arraycntrlsID = cntrlsID.split('|');
        var arraylblID = lblIDs.split('|');
        var arrayerrorLblIDs = errorLblIDs.split('|');
        var count = arraycntrlsID.length;
        var errorcount = 0;
        var errmsgs = '';
        var selectedvalue = GetRadiobuttonCheck(rblsID);
        for (var i = 0; i < count; i++) {
            if (document.getElementById(arraycntrlsID[i]).value == '-1') {
                SetAllVisible(arrayerrorLblIDs[i]);
                errmsgs += '? Please select ' + (document.getElementById(arraylblID[i]).innerText).replace(':', '') + '\n'
                errorcount++;
            }
            else {
                SetAllInvisible(arrayerrorLblIDs[i]);
            }
        }
        if (selectedvalue == '1') {
            if (document.getElementById(txtIsValidated).value == '') {
                errmsgs += '? Please update Schedule Information.\n';
                errorcount++;
            }
        }
        if (errorcount > 0) {
            SetText(lblValidationSummary, errmsgs);
            return false;
        }
        else {
            SetText(lblValidationSummary, errmsgs);
            return true;
        }
    }
    catch (err) {
        alert(ErrorMessage + 'ValidateFlowTaskDetails \n\n' + err.description);
    }
}
function setControlValueFromShuttleControl(modalError, lblError, txtCntrlID, rblstCntrlId, pnlCntrlId, pnlTaskDetails, trDisplayMessage, shuttleCntrlId, taskName) {
    if (document.getElementById(shuttleCntrlId).children[2].value == '') {
        document.getElementById(pnlTaskDetails).style.display = 'none';
        document.getElementById(lblError).innerText = "Select some Tasks";
        $find(modalError).show();
        return false;
    }
    else {
        var selectedItems = document.getElementById(shuttleCntrlId).getElementsByTagName('select')[1].getElementsByTagName('option');
        var selectedString = '';
        var newElement;
        try {
            for (var i = 0; i < selectedItems.length; i++) {
                selectedString += selectedItems[i].value + '|';
                if (i == 0)
                    SetText(taskName, selectedItems[i].text);

            }
            if (selectedItems.length <= 1) {
                document.getElementById(trDisplayMessage).style.display = 'none';
            }
            else {
                document.getElementById(trDisplayMessage).style.display = '';
            }
            document.getElementById(txtCntrlID).value = selectedString.substring(0, selectedString.length - 1);
            //document.getElementById(rblstCntrlId).cells[0].children[0].checked = true;
            $('#' + rblstCntrlId).find('td').eq(0)[0].children[0].checked = true;
            document.getElementById(pnlCntrlId).style.display = 'none';
            document.getElementById(pnlTaskDetails).style.display = '';
        }
        catch (err) {
            alert(ErrorMessage + 'setControlValueFromShuttleControl \n\n' + err.description);
        }
    }
}
function SetTaskTriggerForSecurityType(rblID, btnCount, modalpopUpId, recRblID, recRblCount, popContentPanel, btnViewSchedule) {
    try {
        var selectedValue = GetRadiobuttonCheck(rblID);
        switch (selectedValue) {
            case 0:
                SetVisibility(btnViewSchedule, false);
                $find(modalpopUpId).hide();
                break;
            case 1:
                SetRecurrence(recRblID, popContentPanel, modalpopUpId);
                $find(modalpopUpId).hide();
                $find(modalpopUpId).show();
                SetVisibility(btnViewSchedule, true);
                break;
            case 2:
                $find(modalpopUpId).hide();
                SetVisibility(btnViewSchedule, false);
                break;
            default:
                break;
        }
    }
    catch (err) {
        alert(ErrorMessage + 'SetTaskTrigger \n\n' + err.description);
    }
}
function ValidateTaskDetailsControlForSecurityType(cntrlsID, errorLblIDs, errorMsgs, rblsID) {
    var arraycntrlsID = cntrlsID.split('|');
    var arrayerrorLblIDs = errorLblIDs.split('|');
    var arrayerrorMsgs = errorMsgs.split('|');
    var count = arraycntrlsID.length;
    var errorcount = 0;
    var selectedvalue = GetRadiobuttonCheck(rblsID);
    for (var i = 0; i < count; i++) {
        if (document.getElementById(arraycntrlsID[i]).value == '-1') {
            SetText(arrayerrorLblIDs[i], arrayerrorMsgs[i]);
            errorcount++;
        }
        else {
            SetText(arrayerrorLblIDs[i], '');
        }
    }
    if (errorcount > 0)
        return false;
    else
        return true;
}
/*************************************************************************************************
Task Status Javascript Functions
*************************************************************************************************/

function SetTaskStatusDatePattern(lblStartDate, txtStartDate, lblStartDateError,
lblEndDate, txtEndDate, lblEndDateError, rblstDate, rblstDateCount, lblValidationSummary, enddate, startdate) {
    try {
        //var selectedValue = GetRadiobuttonCheck(rblstDate);
        if (startdate == null || startdate == undefined)
            startdate = enddate;
        //   var selectedValue = getCheckedInput(rblstDate);
        var selectedValue = getSelectedInput(rblstDate);
        switch (parseInt(selectedValue)) {
            case 0:

                SetText(lblStartDate, '');
                SetValue(txtStartDate, startdate);
                SetText(lblEndDate, 'to');
                SetValue(txtEndDate, enddate);
                ChangeToolTip(txtStartDate, 'Enter a Start Date');
                ChangeToolTip(txtEndDate, 'Enter an End Date');
                SetAllVisible(lblStartDate + '|' + txtStartDate + '|' + lblEndDate + '|' + txtEndDate);
                break;
            case 1:
                SetText(lblStartDate, '');
                SetValue(txtStartDate, startdate);
                SetText(lblEndDate, '');
                SetValue(txtEndDate, '');
                ChangeToolTip(txtStartDate, 'Enter From Date');
                ChangeToolTip(txtEndDate, '');
                SetAllInvisible(lblStartDate + '|' + lblEndDate + '|' + txtEndDate);
                SetAllVisible(lblStartDate + '|' + txtStartDate);
                break;
            case 2:
                SetText(lblStartDate, '');
                SetValue(txtStartDate, enddate);
                SetText(lblEndDate, '');
                SetValue(txtEndDate, '');
                ChangeToolTip(txtStartDate, 'Enter Prior To Date');
                ChangeToolTip(txtEndDate, '');
                SetAllInvisible(lblStartDate + '|' + lblEndDate + '|' + txtEndDate);
                SetAllVisible(txtStartDate);
                break;
            default:
                break;
        }
        SetAllInvisible(lblStartDateError + "|" + lblEndDateError);
        SetText(lblValidationSummary, '');

    }
    catch (err) {
        alert(ErrorMessage + 'SetDatePattern \n\n' + err.description);
    }
}


function getSelectedInput(rblClientID) {
    //$("#" + rblClientID).find("tr td input").not(this).prop('checked', false);

    //console.log(instance);
    var selectedIndex;
    var btnCount;
    var version = jQuery.fn.jquery.lastIndexOf('.');

    if ((jQuery.fn.jquery.split('.').length - 1) > 1) {
        version = jQuery.fn.jquery.substring(0, version);
        version = parseFloat(version);
    }
    else
        version = parseFloat(jQuery.fn.jquery);

    //    if (version < 1.8)
    //        btnCount = $("#" + rblClientID).find("input[type='select']");
    //    else
    //        btnCount = $("#" + rblClientID).find('[type="select"]');

    //    btnCount = $("#" + rblClientID +' :selected').t
    //    btnCount.each(function (index) {

    btnCount = $("#" + rblClientID).find("tr td input:checked");

    return parseInt($(btnCount).val());

    //    btnCount.each(function (index) {
    //        if ($(this).prop('selected', true)) {
    //            selectedIndex = index;
    //            return false;
    //        }
    //    });
    //return selectedIndex;
}

function replaceRadioBtnWithImage(targetDiv) {
    var radioBtn = $(targetDiv).find('input[type="radio"]');

    radioBtn.each(function () {
        $(this).hide(); //Hide the default CheckBox

        //Add title to each label.
        var labelElement = $(this).next();
        labelElement.attr("title", labelElement.text())

        //var imageIcon = $("<span class='pull-right glyphicon glyphicon-ok' style='color: #000'><img src='App_Themes/Aqua/images/dropDownCheckImageSmall.png'></span>").insertAfter(labelElement); //New CheckBox Image
        //imageIcon.hide(); //Hide that image by default.

        $(this).parents('tr').addClass('exceptionManagerDropDownRow');
    });
}

//$('.displayDropDownSelection').click(function () { alert("Click"); });

//$(document).click(function (event) {
//    if (!$(event.target).hasClass('displayDropDownSelection')) {
//        $('.CommonDropDownRows').hide();
//    }

//});
function bindClickDropDownRowForRadioBtn(targetDiv) {
    var parentTable = $(targetDiv).find('table');
    var allRadioBtn = parentTable.find("input[type='radio']");
    var allImages = $(targetDiv).find("tr td span img");
    var allTexts = $(targetDiv).find("tr td label");
    var selectedText;

    $(targetDiv).find(".exceptionManagerDropDownRow").each(function (ii, ee) {

        $(ee).unbind('click').click(function () {
            //var imageIcon = $(this).find("img");
            //var imageDiv = $(this).find(".glyphicon-ok");
            var currentRadioBtn = $(this).find("input[type='radio']");

            if (currentRadioBtn.prop("checked") == true) {
                currentRadioBtn.prop("checked", false);
                //                imageIcon.hide();
                $(this).find("td label").css('color', '#525252');
            }
            else {
                allRadioBtn.prop("checked", false);
                //allImages.hide();
                allTexts.css('color', '#525252');
                currentRadioBtn.prop("checked", true);
                $(this).find("td label").css('color', '#a8a6a6');
                //                imageDiv.show();
                //                imageIcon.show();
                $("input[type='radio']:checked").each(function () {
                    var idVal = $(this).attr("id");
                    selectedText = $("label[for='" + idVal + "']").text();
                });
                // $('.displayDropDownSelection').text(selectedText);
                $('[id$="lblddselection"]').text(selectedText);
                //$('.CommonDropDownRows').hide();
            }
            $('.CommonDropDownRows').hide();
        });
    });
}


function realTimeSecurityStatusPageLoad() {
    $('[id$="lblddselection"]').unbind('click').click(function () {
        if (($('.CommonDropDownRows').css('display')) == "none") {
            replaceRadioBtnWithImage($("#ctl00_cphMain_BackgroundSecurityStatus_ddllstDate"));
            bindClickDropDownRowForRadioBtn($("#ctl00_cphMain_BackgroundSecurityStatus_ddllstDate"));
            $('.CommonDropDownRows').show(300);
        }
        else if (($('.CommonDropDownRows').css('display')) == "block")
            $('.CommonDropDownRows').hide();

    });
}



function ValidateTaskStatusInput(txtStartDate, lblStartDate, lblStartDateError, txtEndDate, lblEndDate,
lblEndDateError, lblValidationSummary,
rblstDate, rblstDateCount, isLongDate) {

    //  var selectedValue = GetRadiobuttonCheck(rblstDate);
    var selectedValue = getCheckedInput(rblstDate);
    var errorCount = 0;
    var errormsg = '';
    var msg = '';
    var isvalid = 'true'
    var startDate = $('#' + lblStartDate);
    var endDate = $('#' + lblEndDate);
    if (typeof isLongDate == "undefined")
        isLongDate = false;
    try {

        switch (selectedValue) {
            case 0:
                errormsg = CompareDateFromTodaysDate(document.getElementById(txtStartDate).value, startDate.textNBR().replace(':', ''));
                if (errormsg != '') {
                    SetAllText(lblValidationSummary, errormsg);
                    SetVisibility(lblValidationSummary, true);
                    return false;
                }
                else {
                    errormsg = CompareDateFromTodaysDate(document.getElementById(txtEndDate).value, endDate.textNBR().replace(':', ''));
                    if (errormsg != '') {
                        SetAllText(lblValidationSummary, errormsg);
                        SetVisibility(lblValidationSummary, true);
                        return false;
                    }
                    else {
                        if (!isLongDate)
                            errormsg = CompareDate(document.getElementById(txtStartDate).value, document.getElementById(txtEndDate).value);
                        else
                            errormsg = CompareDateUS(document.getElementById(txtStartDate).value, document.getElementById(txtEndDate).value);
                        if (errormsg != '') {
                            SetAllText(lblValidationSummary, errormsg);
                            SetVisibility(lblValidationSummary, true);
                            return false;
                        }
                        else {
                            SetVisibility(lblValidationSummary, false);
                            return true;
                        }
                    }
                }
                break;
            case 1:
                errormsg = CompareDateFromTodaysDate(document.getElementById(txtStartDate).value, startDate.textNBR().replace(':', ''));
                break;
            case 2:
                errormsg = CompareDateFromTodaysDate(document.getElementById(txtEndDate).value, endDate.textNBR().replace(':', ''));
                break;
        }
        if (errormsg != '') {
            SetAllText(lblValidationSummary, errormsg);
            SetVisibility(lblValidationSummary, true);
            return false;
        }
        else {
            SetVisibility(lblValidationSummary, false);
            return true;
        }
    }
    catch (err) {
        alert(ErrorMessage + 'ValidateTaskStatusInput \n\n' + err.description);
    }

}


function ValidateNewTemplate_LayoutManagement(txtTemplateName, lblTemplateName, lblTemplateType, ddlTemplatetype, ddlGroupName, lblGroupName, lblError) {
    var isvalid = 'true';
    var errormsg = '';
    var txtTemplate = document.getElementById(txtTemplateName);
    txtTemplate.value = $.trim(txtTemplate.value);
    if (txtTemplate.value == '') {
        isvalid = 'false';
        // errormsg = '  ' + '? Please enter ' + (document.getElementById(lblTemplateName).innerText).replace(':', '') + '.' + '\n';
        errormsg = '  ' + '● Please enter ' + $('#' + lblTemplateName).textNBR().replace(':', '') + '.' + '\n';

    }
    if (document.getElementById(ddlTemplatetype).selectedIndex == 0) {
        isvalid = 'false';
        // errormsg += '  ' + '? Please enter ' + (document.getElementById(lblTemplateType).innerText).replace(':', '') + '.' + '\n';
        errormsg += '  ' + '● Please enter ' + $('#' + lblTemplateType).textNBR().replace(':', '') + '.' + '\n';
    }
    if (document.getElementById(ddlGroupName).selectedIndex == 0) {
        isvalid = 'false';
        //errormsg += '  ' + '? Please enter ' + (document.getElementById(lblGroupName).innerText).replace(':', '') + '.';
        errormsg += '  ' + '● Please enter ' + $('#' + lblGroupName).textNBR().replace(':', '') + '.';
    }
    if (errormsg != '') {
        SetAllText(lblError, errormsg);
        SetVisibility(lblError, true);
        return false;
    }
    else {
        SetVisibility(lblError, false);
        return true;
    }
}


/*************************************************************************************************
Request Status Javascript Functions
*************************************************************************************************/
function SetRequestStatusDatePattern(lblStartDate, txtStartDate, lblStartDateError,
lblEndDate, txtEndDate, lblEndDateError, rblstDate, rblstDateCount, lblValidationSummary, now) {
    try {
        //var selectedValue = GetRadiobuttonCheck(rblstDate);
        var selectedValue = getCheckedInput(rblstDate);
        switch (selectedValue) {
            case 0:
                SetText(lblStartDate, 'Start Date : ');
                SetText(lblEndDate, 'End Date : ');
                ChangeToolTip(txtStartDate, 'Enter a Start Date');
                ChangeToolTip(txtEndDate, 'Enter an End Date');
                SetAllVisible(lblStartDate + '|' + txtStartDate + '|' + lblEndDate + '|' + txtEndDate);
                break;

            case 1:
                SetText(lblStartDate, 'From Date : ');
                SetText(lblEndDate, '');
                ChangeToolTip(txtStartDate, 'Enter From Date');
                ChangeToolTip(txtEndDate, '');
                SetAllInvisible(lblEndDate + '|' + txtEndDate);
                SetAllVisible(lblStartDate + '|' + txtStartDate);
                break;
            case 2:
                SetText(lblStartDate, '');
                SetText(lblEndDate, 'Prior To Date : ');
                ChangeToolTip(txtStartDate, '');
                ChangeToolTip(txtEndDate, 'Enter Prior To Date');
                SetAllVisible(lblEndDate + '|' + txtEndDate);
                SetAllInvisible(lblStartDate + '|' + txtStartDate);
                break;
            default:
                break;
        }
        SetAllInvisible(lblStartDateError + "|" + lblEndDateError);
        SetText(lblValidationSummary, '');

    }
    catch (err) {
        alert(ErrorMessage + 'SetDatePattern \n\n' + err.description);
    }
}

//innerText replaced with text() for browser compatibility
//function ValidateRequestStatusInput(txtStartDate, lblStartDate, lblStartDateError, txtEndDate, lblEndDate,
//lblEndDateError, lblValidationSummary, rblstDate, rblstDateCount) {

//    var selectedValue = GetRadiobuttonCheck(rblstDate);
//    var errorCount = 0;
//    var errormsg = '';
//    var msg = '';
//    var isvalid = 'true'
//    var startDate = $("#" + lblStartDate);
//    var endDate = $("#" + lblEndDate);
//    try {

//        switch (selectedValue) {
//            case 0:
//                errormsg = CompareDateFromTodaysDate(document.getElementById(txtStartDate).value, startDate.text().replace(':', ''));
//                if (errormsg != '') {
//                    SetAllText(lblValidationSummary, errormsg);
//                    SetVisibility(lblValidationSummary, true);
//                    return false;
//                }
//                else {
//                    errormsg = CompareDateFromTodaysDate(document.getElementById(txtEndDate).value, endDate.text().replace(':', ''));
//                    if (errormsg != '') {
//                        SetAllText(lblValidationSummary, errormsg);
//                        SetVisibility(lblValidationSummary, true);
//                        return false;
//                    }
//                    else {
//                        errormsg = CompareDate(document.getElementById(txtStartDate).value, document.getElementById(txtEndDate).value);
//                        if (errormsg != '') {
//                            SetAllText(lblValidationSummary, errormsg);
//                            SetVisibility(lblValidationSummary, true);
//                            return false;
//                        }
//                        else {
//                            SetVisibility(lblValidationSummary, false);
//                            return true;
//                        }
//                    }
//                }
//                break;
//            case 1:
//                errormsg = CompareDateFromTodaysDate(document.getElementById(txtStartDate).value, startDate.text().replace(':', ''));
//                break;
//            case 2:
//                errormsg = CompareDateFromTodaysDate(document.getElementById(txtEndDate).value, endDate.text().replace(':', ''));
//                break;
//        }
//        if (errormsg != '') {
//            SetAllText(lblValidationSummary, errormsg);
//            return false;
//        }
//        else {
//            return true;
//        }
//    }
//    catch (err) {
//        alert(ErrorMessage + 'ValidateRequestStatusInput \n\n' + err.description);
//    }
//}

//function ValidateSelectionAttributeLevelAudit(txtStartDate, lblStartDate, lblStartDateError, txtEndDate, lblEndDate,
//lblEndDateError, lblValidationSummary, rblstDate, rblstDateCount, lblCheckAttributes, AttributeShuttle) {
//    var errormsg;
//    if (ValidateRequestStatusInput(txtStartDate, lblStartDate, lblStartDateError, txtEndDate, lblEndDate, lblEndDateError, lblValidationSummary, rblstDate, rblstDateCount)) {
//          var children = $("#"+AttributeShuttle).children(); //For browser compatibility
//        //if (GetObject(AttributeShuttle).children[2].value == '')
//        if(children[2].value =='')
//        {
//            SetVisibility(lblCheckAttributes, true);
//            return false;
//        }
//        else {
//            SetVisibility(lblCheckAttributes, false);
//            return true;
//        }
//    }
//    else {
//        //if (GetObject(AttributeShuttle).children[2].value == '') {
//        if ($("#" + AttributeShuttle).children()[2].value == '') {
//            SetVisibility(lblCheckAttributes, true);
//        }
//        else {
//            SetVisibility(lblCheckAttributes, false);
//        }
//        return false;
//    }
//}


/*************************************************************************************************
Set the correct Accordion Index javascript functions 
*************************************************************************************************/

var id;
var intevalId;
function setAccordionIndex(_id) {
    id = _id;
    intevalId = window.setTimeout(SetAccordion, 300);
}

function SetAccordion(sender, args) {
    if (id != '') {
        var toSelectLinkButton = $get(id);
        var parent = toSelectLinkButton;
        var accordion;
        var index;
        var selected = false;
        if (toSelectLinkButton != null) {
            var elementForIndex = $(toSelectLinkButton.parentNode.parentNode.parentNode).prev()[0];
            if (elementForIndex == null) {
                elementForIndex = $(toSelectLinkButton.parentNode.parentNode.parentNode.parentNode).prev()[0];
            }

            while (parent != null && elementForIndex != null) {
                if (parent.nodeName.toUpperCase() == 'DIV') {
                    accordion = $find(parent.id + '_AccordionExtender');
                    if (accordion != null) {
                        index = elementForIndex.getAttribute('_index');
                        accordion.set_SelectedIndex(index);
                        selected = true;
                        toSelectLinkButton.className = 'thirdItemSelected';
                        try {
                            elementForIndex = $(parent.parentNode.parentNode).prev()[0];
                        }
                        catch (e) {
                        }
                    }
                }
                parent = parent.parentNode;
            }
        }
    }
    window.clearTimeout(intevalId);
}


/*************************************************************************************************
Bulk Edit Javascript Functions
*************************************************************************************************/
////innerText replaced with text() for Browser compatibility,moved to editbulksecurities
//function ValidateBulkEditInput(lblStartDate, txtStartDate, lblEndDate, txtEndDate, lblValidationSummary, gridSecurities, lblValidate, gridAttributes, lblWarning, lblValidateSecuritySelection) {
//    var errormsg;
//    var startDate = $("#" + lblStartDate);
//    var endDate = $("#" + lblEndDate);
//    SetVisibility(lblValidateSecuritySelection, false);
//    errormsg = CompareDateFromTodaysDate(document.getElementById(txtStartDate).value, startDate.text().replace(':', ''));
//    if (errormsg != '') {
//        SetAllText(lblValidationSummary, errormsg);
//        SetVisibility(lblValidationSummary, true);
//        SetVisibility(lblValidate, false);
//        return false;
//    }
//    else {
//        errormsg = CompareDateFromTodaysDate(document.getElementById(txtEndDate).value, endDate.text().replace(':', ''));
//        if (errormsg != '') {
//            SetAllText(lblValidationSummary, errormsg);
//            SetVisibility(lblValidationSummary, true);
//            SetVisibility(lblValidate, false);
//            return false;
//        }
//        else {
//            errormsg = CompareDate(document.getElementById(txtStartDate).value, document.getElementById(txtEndDate).value);
//            if (errormsg != '') {
//                SetAllText(lblValidationSummary, errormsg);
//                SetVisibility(lblValidationSummary, true);
//                SetVisibility(lblValidate, false);
//                return false;
//            }
//            else {
//                SetVisibility(lblValidationSummary, false);
//                gridObj = GetObject(gridSecurities);
//                var isSelected = false;
//                for (var rows = 1; rows < gridObj.rows.length; rows++) {
//                    //checkBoxChecked = gridObj.rows[rows].cells[0].children[0].checked; //FBC
//                    checkBoxChecked = $(gridObj.rows[rows].cells[0]).children()[0].checked;
//                    if (checkBoxChecked) {
//                        isSelected = true;
//                        break;
//                    }
//                }
//                if (!isSelected) {
//                    SetAllText(lblValidate, "Please Select atleast one Security");
//                    SetVisibility(lblValidate, true);
//                    return false;
//                }

//                gridObj = GetObject(gridAttributes);
//                errormsg = '';
//                for (var rows = 1; rows < gridObj.rows.length; rows++) {
//                //FBC
////                    if (gridObj.rows[rows].cells[2].children[4].id.indexOf("rdlstAttributeValue") != -1) {
////                        valueEntered = GetRadiobuttonCheck(gridObj.rows[rows].cells[2].children[4].id)
//                    if (gridObj.rows[rows].cells[2].children()[4].id.indexOf("rdlstAttributeValue") != -1) {
//                        valueEntered = GetRadiobuttonCheck($(gridObj.rows[rows].cells[2]).children()[4].id)
//                        if (valueEntered == '2')
//                            valueEntered = '';
//                        else if (valueEntered == '0')
//                            valueEntered = 'False';
//                    }
//                    else {
//                        //                        valueEntered = gridObj.rows[rows].cells[2].children[4].value;//FBC
//                        valueEntered = $(gridObj.rows[rows].cells[2]).children()[4].value;
//                    }
//                    if (valueEntered == '') {
//                        errormsg = errormsg + '\n' + $(gridObj.rows[rows].cells[1]).text();
//                    }
//                }
//                if (errormsg != '') {
//                    SetAllText(lblWarning, "The Following Attribute(s) have null value(s) : \n" + errormsg + "\n\n Do you wish to continue?\n");
//                    $find('modalWarning').show();
//                    return false;
//                }
//                return true;
//            }
//        }
//    }
//}

//Moved to EditMultipleSecurities.js
//function ValidateSelectionBulkEdit(gridSecurities, cbxRuleType, lblValidateSecuritySelection, lblValidate, lblCheckAttributes, lblValidationSummary) {
//    SetVisibility(lblCheckAttributes, false);
//    SetVisibility(lblValidationSummary, false);
//    SetVisibility(lblValidate, false);
//    SetVisibility(lblValidateSecuritySelection, false);
//    gridObj = GetObject(gridSecurities);
//    var isSelected = false;
//    for (var rows = 1; rows < gridObj.rows.length; rows++) {
//        //        checkBoxChecked = gridObj.rows[rows].cells[0].children[0].checked;//FBC
//        checkBoxChecked = $(gridObj.rows[rows].cells[0]).children()[0].checked;
//        if (checkBoxChecked) {
//            isSelected = true;
//            break;
//        }
//    }
//    if (!isSelected) {
//        SetAllText(lblValidateSecuritySelection, "Please Select atleast one Security");
//        SetVisibility(lblValidateSecuritySelection, true);
//        return false;
//    }
//    else {
//        isSelected = false;
//        checkBoxList = GetObject(cbxRuleType);
//        checkBoxItems = checkBoxList.getElementsByTagName("INPUT");
//        for (var items = 1; items < checkBoxItems.length; items++) {
//            if (checkBoxItems[items].checked)
//                isSelected = true;
//            else
//                isSelected = false;
//        }
//        if (!isSelected) {
//            SetAllText(lblValidateSecuritySelection, "Please Select atleast one Rule Type");
//            SetVisibility(lblValidateSecuritySelection, true);
//            return false;
//        }
//    }
//}

//function ValidateSelection(lblCheckAttributes, EditBulkAttributeShuttle, panelAttributeValues, lblValidateSecuritySelection) {
//    var errormsg;
//    SetVisibility(lblValidateSecuritySelection, false);
////    if (GetObject(EditBulkAttributeShuttle).children[2].value == '') { //FBC
//    if ($('#'+EditBulkAttributeShuttle).children()[2].value == '') {
//        errormsg = "No Attributes Selected.";
//        SetAllText(lblCheckAttributes, errormsg);
//        SetVisibility(lblCheckAttributes, true);
//        SetVisibility(panelAttributeValues, false);
//        return false;
//    }
//    else {
//        SetVisibility(lblCheckAttributes, false);
//        //        SetVisibility(panelAttributeValues, true);
//        return true;
//    }
//}


//function EnableShowReportForMissingMandatoryReport(ddlTemplates, btnShowReport) {
//    if (GetObject(ddlTemplates).value == '-1')
//        GetObject(btnShowReport).disabled = true;
//    else
//        GetObject(btnShowReport).disabled = false;
//}



//var gridId;
//function CheckBoxSelectionBulkEdit(gridID) {
//    gridId = gridID;
//    window.setTimeout(GridSecuritiesBulkEdit, 800);
//}
//function GridSecuritiesBulkEdit() {
//    var grid = $get(gridId);
//    FindChildNodeByTagName(grid.rows[0].cells[0], 'INPUT').click();
//    SelectAllRowsAccrossPages();
//}

//function SelectAllRowsAccrossPages() {
//    var clickElement = document.getElementById(gridId + '_chkSelectAllSelectCurrentRows_selectAll');
//    if (clickElement != null)
//        clickElement.click();
//}

//function FindChildNodeByTagName(element, nodeName) {
//    while (element != null) {
//        if (element.nodeName.toUpperCase() == nodeName) {
//            return element;
//        }
//        element = element.children[0];
//    }
//}


//function validateTimeSeriesDate(txtStartDate, lblStartDate, txtEndDate, lblEndDate, lblValidationSummary) {
//    var errormsg = '';
//    var startDate = $('#' + lblStartDate);
//    var endDate = $('#' + endDate);
//    try {
//        errormsg = CompareDateFromTodaysDate(document.getElementById(txtStartDate).value, startDate.text().replace(':', ''));
//        if (errormsg != '') {
//            SetAllText(lblValidationSummary, '? Please enter Effective Start Date less than Todays Date\n ');
//            SetVisibility(lblValidationSummary, true);
//            return false;
//        }
//        else {
//            var startDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertStringToDateTime(document.getElementById(txtStartDate).value, com.ivp.rad.rscriptutils.DateTimeFormat.shorDate);
//            var currentDate = new Date();
//            if (startDate.format('yyyyMMdd') == currentDate.format('yyyyMMdd')) {
//                SetAllText(lblValidationSummary, '? Please enter Effective Start Date less than Todays Date\n ');
//                SetVisibility(lblValidationSummary, true);
//                return false;
//            }
//            else {
//                errormsg = CompareDateFromTodaysDate(document.getElementById(txtEndDate).value,endDate.text().replace(':', ''));
//                if (errormsg != '') {
//                    SetAllText(lblValidationSummary, errormsg);
//                    SetVisibility(lblValidationSummary, true);
//                    return false;
//                }
//                else {
//                    errormsg = CompareDate(document.getElementById(txtStartDate).value, document.getElementById(txtEndDate).value);
//                    if (errormsg != '') {
//                        SetAllText(lblValidationSummary, errormsg);
//                        SetVisibility(lblValidationSummary, true);
//                        return false;
//                    }
//                    else {
//                        SetVisibility(lblValidationSummary, false);
//                        return true;
//                    }
//                }
//            }
//        }
//        if (errormsg != '') {
//            SetAllText(lblValidationSummary, errormsg);
//            return false;
//        }
//        else {
//            return true;
//        }
//    }
//    catch (err) {
//        alert(ErrorMessage + 'ValidateRequestStatusInput \n\n' + err.description);
//    }
//}



function ToggleDisplayForCommonConfiguration(chkConfig, trConfig) {
    if (GetObject(chkConfig).checked)
        SetVisibility(trConfig, true);
    else
        SetVisibility(trConfig, false);
}
function OpenWindow() {
    var url = 'SMImpactSecurities.aspx?identifier=ImpactedSecurities';
    var window_dimensions = "toolbars=no,menubar=no,location=no,scrollbars=yes,resizable=yes,status=no,height=750px,width=1250px";
    window.open(url, "ImpactedSecurities", window_dimensions);
    return false;
}



function getCheckedInput(rblClientID) {
    var selectedIndex;
    var btnCount;
    var version = jQuery.fn.jquery.lastIndexOf('.');

    if ((jQuery.fn.jquery.split('.').length - 1) > 1) {
        version = jQuery.fn.jquery.substring(0, version);
        version = parseFloat(version);
    }
    else
        version = parseFloat(jQuery.fn.jquery);

    if (version < 1.8)
        btnCount = $("#" + rblClientID).find("input[type='radio']");
    else
        btnCount = $("#" + rblClientID).find('[type="radio"]');

    btnCount.each(function (index) {
        if ($(this).prop('selected')) {
            selectedIndex = index;
            return false;
        }
    });
    return selectedIndex;
}

//USED for reloading single screen on btn click in iframe
function refreshParentPageFromIframe() {
    var url = window.location.href;
    var btnId = '';
    var queryString = url.split('?');
    if (queryString.length > 0) {
        var paramAndValueCollection = queryString[1].split('&');
        if (paramAndValueCollection.length > 0) {
            for (var i = 0; i < paramAndValueCollection.length; i++) {
                var paramAndValue = paramAndValueCollection[i].split('=');
                if (paramAndValue[0] == 'postbackBtn') {
                    btnId = paramAndValue[1];
                }
            }
        }
    }
    var btn = window.parent.document.getElementById(btnId);
    $(btn).click();
    return false;
}

//used in common rule when opened on single screen
function attachHandlerToCollapsiblePanelSS(collapsePanelId, gridId, subtractHeight, subtractWidth) {
    var collapsiblePanel = $find(collapsePanelId);
    if (collapsiblePanel != null && collapsiblePanel != undefined)
        collapsiblePanel.add_expanded(expandHandlerSS);
    //resizeGridFinal(gridId, topPanelId, middlePanelId, bottomPanelId, subtractWidth, subtractHeight, true, false);
}

function expandHandlerSS(sender, args) {
    $find($("[id$=_xlGridAttributeGroupRules]").attr('id')).refreshWithCache();
}
//added by Arunabh for temporarily fixing the disabling issue of drop down is Bsym Control in case of market sector not handled
function EnableBSYMcontrol() {
    //    var elFoo = window.frames['bsymCreateSecurity_iframeContainer'].document.getElementById('ddlMarketSector');
    //    elFoo.disabled = false;


    //    var iframe = document.getElementById('actContentToGet');
    //    var frameDoc = iframe.contentDocument || iframe.contentWindow.document;
    //    var el = frameDoc.getElementById(elementID);
    //    el.parentNode.removeChild(el);

}


function ValidateDateTimeInputUS(txtStartDate, lblStartDate, lblStartDateError, txtEndDate, lblEndDate,
lblEndDateError, lblValidationSummary,
rblstDate, rblstDateCount) {

    var selectedValue = GetRadiobuttonCheck(rblstDate);
    var errorCount = 0;
    var errormsg = '';
    var msg = '';
    var isvalid = 'true'

    try {

        switch (selectedValue) {
            case 0: //between dates
                errormsg = CompareDateFromTodaysDateUS(document.getElementById(txtStartDate).value, (document.getElementById(lblStartDate).innerText).replace(':', ''));
                if (errormsg != '') {
                    SetAllText(lblValidationSummary, errormsg);
                    SetVisibility(lblValidationSummary, true);
                    return false;
                }
                else {
                    errormsg = CompareDateFromTodaysDateUS(document.getElementById(txtEndDate).value, (document.getElementById(lblEndDate).innerText).replace(':', ''));
                    if (errormsg != '') {
                        SetAllText(lblValidationSummary, errormsg);
                        SetVisibility(lblValidationSummary, true);
                        return false;
                    }
                    else {
                        errormsg = CompareDateUS(document.getElementById(txtStartDate).value, document.getElementById(txtEndDate).value);
                        if (errormsg != '') {
                            SetAllText(lblValidationSummary, errormsg);
                            SetVisibility(lblValidationSummary, true);
                            return false;
                        }
                        else {
                            SetVisibility(lblValidationSummary, false);
                            return true;
                        }
                    }
                }
                break;
            case 1: //from date
                errormsg = CompareDateFromTodaysDateUS(document.getElementById(txtStartDate).value, (document.getElementById(lblStartDate).innerText).replace(':', ''));
                break;
            case 2: //prior to
                errormsg = CompareDateFromTodaysDateUS(document.getElementById(txtEndDate).value, (document.getElementById(lblEndDate).innerText).replace(':', ''));
                break;
            case 3:
                break;
        }
        if (errormsg != '') {
            SetAllText(lblValidationSummary, errormsg);
            SetVisibility(lblValidationSummary, true);
            return false;
        }
        else {
            SetVisibility(lblValidationSummary, false);
            return true;
        }
    }
    catch (err) {
        alert(ErrorMessage + 'ValidateTaskStatusInput \n\n' + err.description);
    }

}

function CompareDateFromTodaysDateUS(webServicePath, inputDate, lblDate) {
    try {
        //check if start date is greator than end date; returns true if start date is greator than end date
        var res = ExecuteSynchronously(webServicePath, 'CompareDate', { startDate: inputDate, endDate: new Date(), setServerDate: true });
        if (res.d) {
            return '● Please enter ' + lblDate + ' less than or equal to Todays Date\n ';
        }
        else {
            return '';
        }
    }
    catch (err) {
        alert(ErrorMessage + 'CompareDate \n\n' + err.description);
    }
}


function CompareDateUS(webServicePath, startdate, enddate) {
    try {
        //check if start date is greator than end date;returns true if start date is greator than end date
        var res = ExecuteSynchronously(webServicePath, 'CompareDate', { startDate: startdate, endDate: enddate, setServerDate: false });
        if (res.d) {
            return '● Please enter End Date greater than or equal to Start Date\n ';
        }
        else {
            return '';
        }
    }
    catch (err) {
        alert(ErrorMessage + 'CompareDate \n\n' + err.description);
    }
}
//Moved to ViewReport.js
//function SetReportViewerDatePattern(lblStartDate, txtStartDate, lblStartDateError,
//            lblEndDate, txtEndDate, lblEndDateError, rblstDate, rblstDateCount, lblValidationSummary, now) {
//    SetRequestStatusDatePattern(lblStartDate, txtStartDate, lblStartDateError,
//            lblEndDate, txtEndDate, lblEndDateError, rblstDate, rblstDateCount, lblValidationSummary, now);
//    var selectedValue = GetRadiobuttonCheck(rblstDate);
//    GetObject(lblValidationSummary).style.color = "DarkOliveGreen";
//    if (selectedValue == 2)
//        SetText(lblValidationSummary, 'Audit will be displayed for 30 days prior to the selected date including the selected date.');
//    else if (selectedValue == 1)
//        SetText(lblValidationSummary, 'Audit will be displayed for next 30 days or upto current date (whichever is less) from the selected date including the selected date.');
//    else
//        SetText(lblValidationSummary, 'Selected date range should be less than or equal to 30 days.');
//}

//Moved to ViewReport.js
//function ValidateDateInputForReportViewer(txtStartDate, lblStartDate, txtEndDate, lblEndDate, lblValidationSummary, rblstDate, flagDoPostBack, postBackElementId) {
//    if (GetObject(txtStartDate) != null && GetObject(txtEndDate) != null) {
//        var selectedValue = GetRadiobuttonCheck(rblstDate);
//        var startDate = $('#' + lblStartDate);
//        var endDate = $('#' + lblEndDate);
//        var errormsg = '';
//        switch (selectedValue) {
//            case 0:
//                errormsg = CompareDateFromTodaysDate(document.getElementById(txtStartDate).value, startDate.text().replace(':', ''));
//                if (errormsg != '') {
//                    SetAllText(lblValidationSummary, errormsg);
//                    SetVisibility(lblValidationSummary, true);
//                    GetObject(lblValidationSummary).style.color = "Red";
//                    return false;
//                }
//                else {
//                    errormsg = CompareDateFromTodaysDate(document.getElementById(txtEndDate).value, endDate.text().replace(':', ''));
//                    if (errormsg != '') {
//                        SetAllText(lblValidationSummary, errormsg);
//                        SetVisibility(lblValidationSummary, true);
//                        GetObject(lblValidationSummary).style.color = "Red";
//                        return false;
//                    }
//                    else {
//                        errormsg = CompareDate(document.getElementById(txtStartDate).value, document.getElementById(txtEndDate).value);
//                        if (errormsg != '') {
//                            SetAllText(lblValidationSummary, errormsg);
//                            SetVisibility(lblValidationSummary, true);
//                            GetObject(lblValidationSummary).style.color = "Red";
//                            return false;
//                        }
//                    }
//                }
//                break;
//            case 1:
//                errormsg = CompareDateFromTodaysDate(document.getElementById(txtStartDate).value, startDate.text().replace(':', ''));
//                break;
//            case 2:
//                errormsg = CompareDateFromTodaysDate(document.getElementById(txtEndDate).value, endDate.text().replace(':', ''));
//                break;
//        }
//        if (errormsg != '') {
//            SetAllText(lblValidationSummary, errormsg);
//            GetObject(lblValidationSummary).style.color = "Red";
//            return false;
//        }
//        else if (selectedValue == 0) {
//            var startDate = new Date();
//            var endDate = new Date();
//            startDate = Date.parseInvariant(GetObject(txtStartDate).value, com.ivp.rad.rscriptutils.RSCultureManager.GetCultureInfo().ShortDateFormat.toString());
//            startDate = startDate.setDate(startDate.getDate() + 29);
//            endDate = Date.parseInvariant(GetObject(txtEndDate).value, com.ivp.rad.rscriptutils.RSCultureManager.GetCultureInfo().ShortDateFormat.toString());
//            endDate = endDate.setDate(endDate.getDate());

//            if (startDate < endDate) {
//                GetObject(lblValidationSummary).style.color = "Red";
//                SetText(lblValidationSummary, 'Selected date range is greater than 30 days.');
//                return false;
//            }
//            else {
//                SetText(lblValidationSummary, '');
//                //                if (flagDoPostBack)
//                //                    __doPostBack(postBackElementId, '');
//                //return true;
//            }
//        }
//        else {
//            //            if (flagDoPostBack)
//            //                __doPostBack(postBackElementId, '');
//            //return true;
//        }
//    }
//    else {
//        //        if (flagDoPostBack)
//        //            __doPostBack(postBackElementId, '');
//        //return true;
//    }
//}

