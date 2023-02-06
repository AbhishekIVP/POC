// RScriptUtils.js
//


Type.registerNamespace('com.ivp.rad.rradapi');

////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rradapi.RADWebServiceException

com.ivp.rad.rradapi.RADWebServiceException = function com_ivp_rad_rradapi_RADWebServiceException() {
}
com.ivp.rad.rradapi.RADWebServiceException.prototype = {
    
    get_message: function com_ivp_rad_rradapi_RADWebServiceException$get_message() {
        /// <summary>
        /// Get_messages this instance.
        /// </summary>
        /// <returns type="String"></returns>
        return '';
    },
    
    get_statusCode: function com_ivp_rad_rradapi_RADWebServiceException$get_statusCode() {
        /// <summary>
        /// Get_statuses the code.
        /// </summary>
        /// <returns type="Number" integer="true"></returns>
        return 0;
    }
}


Type.registerNamespace('com.ivp.rad.rscriptutils');

////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rscriptutils.ClearExcept

com.ivp.rad.rscriptutils.ClearExcept = function() { 
    /// <summary>
    /// Clear Except one from...
    /// </summary>
    /// <field name="NONE" type="Number" integer="true" static="true">
    /// Clear All
    /// </field>
    /// <field name="LABEL" type="Number" integer="true" static="true">
    /// Clear except label
    /// </field>
    /// <field name="SELECT" type="Number" integer="true" static="true">
    /// clear except label
    /// </field>
    /// <field name="CHECKBOX" type="Number" integer="true" static="true">
    /// clear except checkbox
    /// </field>
};
com.ivp.rad.rscriptutils.ClearExcept.prototype = {
    NONE: 0, 
    LABEL: 1, 
    SELECT: 2, 
    CHECKBOX: 3
}
com.ivp.rad.rscriptutils.ClearExcept.registerEnum('com.ivp.rad.rscriptutils.ClearExcept', false);


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rscriptutils.BindingControl

com.ivp.rad.rscriptutils.BindingControl = function() { 
    /// <summary>
    /// Control To bind with DropDownInfo
    /// </summary>
    /// <field name="dropDown" type="Number" integer="true" static="true">
    /// The control is DropDown
    /// </field>
    /// <field name="listBox" type="Number" integer="true" static="true">
    /// The Control is List Box
    /// </field>
};
com.ivp.rad.rscriptutils.BindingControl.prototype = {
    dropDown: 0, 
    listBox: 1
}
com.ivp.rad.rscriptutils.BindingControl.registerEnum('com.ivp.rad.rscriptutils.BindingControl', false);


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rscriptutils.DateTimeFormat

com.ivp.rad.rscriptutils.DateTimeFormat = function() { 
    /// <summary>
    /// Date Time Format Enum
    /// </summary>
    /// <field name="longDate" type="Number" integer="true" static="true">
    /// Long Date contains Date and Time.
    /// </field>
    /// <field name="shorDate" type="Number" integer="true" static="true">
    /// Short Date contains only Date.
    /// </field>
    /// <field name="longTime" type="Number" integer="true" static="true">
    /// Long Time contains hours, minutes, seconds and milliseconds.
    /// </field>
    /// <field name="shortTime" type="Number" integer="true" static="true">
    /// Short Time contains hours and minutes.
    /// </field>
};
com.ivp.rad.rscriptutils.DateTimeFormat.prototype = {
    longDate: 0, 
    shorDate: 1, 
    longTime: 2, 
    shortTime: 3
}
com.ivp.rad.rscriptutils.DateTimeFormat.registerEnum('com.ivp.rad.rscriptutils.DateTimeFormat', false);


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rscriptutils.DateTimeOptions

com.ivp.rad.rscriptutils.DateTimeOptions = function() { 
    /// <field name="date" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="time" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="scheduled" type="Number" integer="true" static="true">
    /// </field>
};
com.ivp.rad.rscriptutils.DateTimeOptions.prototype = {
    date: 0, 
    time: 1, 
    scheduled: 2
}
com.ivp.rad.rscriptutils.DateTimeOptions.registerEnum('com.ivp.rad.rscriptutils.DateTimeOptions', false);


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rscriptutils.FilterType

com.ivp.rad.rscriptutils.FilterType = function() { 
    /// <field name="upper" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="lower" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="numbers" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="custom" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="upperLower" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="upperNumbers" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="upperCustom" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="lowerNumbers" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="lowerCustom" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="numbersCustom" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="upperLowerNumbers" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="upperLowerCustom" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="upperNumbersCustom" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="lowerNumbersCustom" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="upperLowerCustomNumbers" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="none" type="Number" integer="true" static="true">
    /// </field>
};
com.ivp.rad.rscriptutils.FilterType.prototype = {
    upper: 0, 
    lower: 1, 
    numbers: 2, 
    custom: 3, 
    upperLower: 4, 
    upperNumbers: 5, 
    upperCustom: 6, 
    lowerNumbers: 7, 
    lowerCustom: 8, 
    numbersCustom: 9, 
    upperLowerNumbers: 10, 
    upperLowerCustom: 11, 
    upperNumbersCustom: 12, 
    lowerNumbersCustom: 13, 
    upperLowerCustomNumbers: 14, 
    none: 15
}
com.ivp.rad.rscriptutils.FilterType.registerEnum('com.ivp.rad.rscriptutils.FilterType', false);


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rscriptutils.RSCommonScripts

com.ivp.rad.rscriptutils.RSCommonScripts = function com_ivp_rad_rscriptutils_RSCommonScripts() {
    /// <summary>
    /// Class containing Common Scripts
    /// </summary>
    /// <field name="_defaulT_VALUE" type="String" static="true">
    /// </field>
    /// <field name="_behaviorID" type="String" static="true">
    /// </field>
    /// <field name="_intervalID" type="Number" integer="true" static="true">
    /// </field>
}
com.ivp.rad.rscriptutils.RSCommonScripts.setDefaultPath = function com_ivp_rad_rscriptutils_RSCommonScripts$setDefaultPath(chkId, txtboxId) {
    /// <summary>
    /// Sets a Defualt Value on chkBox click if set as an attribute named 'defualtVal'
    /// </summary>
    /// <param name="chkId" type="String">
    /// The CHK id.
    /// </param>
    /// <param name="txtboxId" type="String">
    /// The txtbox id.
    /// </param>
    var chk = com.ivp.rad.rscriptutils.RSValidators.getObject(chkId);
    var txtEle = com.ivp.rad.rscriptutils.RSValidators.getObject(txtboxId);
    if (chk.checked) {
        txtEle.value = chk.getAttribute(com.ivp.rad.rscriptutils.RSCommonScripts._defaulT_VALUE).toString();
        txtEle.disabled = true;
    }
    else {
        txtEle.value = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
        txtEle.disabled = false;
    }
}
com.ivp.rad.rscriptutils.RSCommonScripts.setTxtBoxField = function com_ivp_rad_rscriptutils_RSCommonScripts$setTxtBoxField(txtFieldId, password) {
    /// <summary>
    /// Sets the textbox field with a given value.
    /// </summary>
    /// <param name="txtFieldId" type="String">
    /// The TXT field id.
    /// </param>
    /// <param name="password" type="String">
    /// The password.
    /// </param>
    if (password != null) {
        (com.ivp.rad.rscriptutils.RSValidators.getObject(txtFieldId)).value = password;
    }
}
com.ivp.rad.rscriptutils.RSCommonScripts.showPopUp = function com_ivp_rad_rscriptutils_RSCommonScripts$showPopUp(controlBehaviorId) {
    /// <summary>
    /// Shows the pop up.
    /// </summary>
    /// <param name="controlBehaviorId" type="String">
    /// The control's behavior id.
    /// </param>
    var modalPopUp = $find(controlBehaviorId);
    if (modalPopUp != null) {
        com.ivp.rad.rscriptutils.RSCommonScripts._behaviorID = controlBehaviorId;
        modalPopUp.show();
    }
}
com.ivp.rad.rscriptutils.RSCommonScripts.delayedEnableEnterKeyPressOnPopUp = function com_ivp_rad_rscriptutils_RSCommonScripts$delayedEnableEnterKeyPressOnPopUp(controlBehaviorId) {
    /// <param name="controlBehaviorId" type="String">
    /// </param>
    com.ivp.rad.rscriptutils.RSCommonScripts._behaviorID = controlBehaviorId;
    com.ivp.rad.rscriptutils.RSCommonScripts._intervalID = window.setTimeout(Function.createDelegate(null, com.ivp.rad.rscriptutils.RSCommonScripts._enableEnterKeyPressOnPopUp), 200);
}
com.ivp.rad.rscriptutils.RSCommonScripts._enableEnterKeyPressOnPopUp = function com_ivp_rad_rscriptutils_RSCommonScripts$_enableEnterKeyPressOnPopUp() {
    var modalPopUp = $find(com.ivp.rad.rscriptutils.RSCommonScripts._behaviorID);
    if (modalPopUp != null) {
        var popUpInfo;
        if (modalPopUp.get_OkControlID() != null) {
            popUpInfo = modalPopUp.get_PopupControlID() + '|OK|' + modalPopUp.get_OkControlID();
        }
        else if (modalPopUp.get_CancelControlID() != null) {
            popUpInfo = modalPopUp.get_PopupControlID() + '|Cancel|' + modalPopUp.get_CancelControlID();
        }
        else {
            popUpInfo = modalPopUp.get_PopupControlID();
        }
        document.documentElement.setAttribute('popUpInfo', popUpInfo);
        var popUp = document.getElementById(modalPopUp.get_PopupControlID());
        $addHandler(popUp, 'keydown', Function.createDelegate(null, com.ivp.rad.rscriptutils.RSCommonScripts._popUpKeyDownHandler));
        try {
            popUp.getElementsByTagName('input')[0].focus();
        }
        catch ($e1) {
        }
    }
}
com.ivp.rad.rscriptutils.RSCommonScripts._popUpKeyDownHandler = function com_ivp_rad_rscriptutils_RSCommonScripts$_popUpKeyDownHandler(e) {
    /// <summary>
    /// Pop up key down handler.
    /// </summary>
    /// <param name="e" type="Sys.UI.DomEvent">
    /// The e.
    /// </param>
    if (e.keyCode === 13) {
        var args = document.documentElement.getAttribute('popUpInfo').toString().split('|');
        if (args.length === 3 && args[1] === 'OK') {
            document.getElementById(args[2]).click();
        }
        else {
            var collection = document.getElementById(args[0]).getElementsByTagName('input');
            var Ok = null;
            var clicked = false;
            for (var i = 0; i < collection.length; i++) {
                Ok = collection[i];
                if ((Ok.type.toUpperCase() === 'BUTTON' || Ok.type.toUpperCase() === 'SUBMIT') && !Ok.disabled && Ok.style.display === '' && (Ok.style.visibility === 'visible' || Ok.style.visibility === '')) {
                    clicked = true;
                    Ok.click();
                    break;
                }
            }
            if (!clicked && args.length === 3) {
                document.getElementById(args[2]).click();
            }
        }
        e.preventDefault();
        $clearHandlers(document.getElementById(args[0]));
        window.clearTimeout(com.ivp.rad.rscriptutils.RSCommonScripts._intervalID);
    }
}
com.ivp.rad.rscriptutils.RSCommonScripts.hidePopUp = function com_ivp_rad_rscriptutils_RSCommonScripts$hidePopUp(controlBehaviorId) {
    /// <summary>
    /// Hides the pop up.
    /// </summary>
    /// <param name="controlBehaviorId" type="String">
    /// The control's behavior id.
    /// </param>
    var modalPopUp = $find(controlBehaviorId);
    $clearHandlers(document.getElementById(modalPopUp.get_PopupControlID()));
    modalPopUp.hide();
}
com.ivp.rad.rscriptutils.RSCommonScripts.findControl = function com_ivp_rad_rscriptutils_RSCommonScripts$findControl(control, controlToFind) {
    /// <summary>
    /// Finds the control up the hierarchy.
    /// </summary>
    /// <param name="control" type="Object" domElement="true">
    /// The control.
    /// </param>
    /// <param name="controlToFind" type="String">
    /// The control to find.
    /// </param>
    /// <returns type="Object" domElement="true"></returns>
    while (control.nodeName.toLowerCase() !== controlToFind.toLowerCase()) {
        control = control.parentNode;
        if (control == null) {
            break;
        }
    }
    return control;
}
com.ivp.rad.rscriptutils.RSCommonScripts.findChildControl = function com_ivp_rad_rscriptutils_RSCommonScripts$findChildControl(control, controlToFind) {
    /// <summary>
    /// Finds the child control.
    /// </summary>
    /// <param name="control" type="Object" domElement="true">
    /// The control.
    /// </param>
    /// <param name="controlToFind" type="String">
    /// The control to find.
    /// </param>
    /// <returns type="Object" domElement="true"></returns>
    while (control.nodeName.toLowerCase() !== controlToFind.toLowerCase()) {
        control = control.childNodes[0];
        if (control == null) {
            break;
        }
    }
    return control;
}
com.ivp.rad.rscriptutils.RSCommonScripts.clearControls = function com_ivp_rad_rscriptutils_RSCommonScripts$clearControls(ele, except) {
    /// <summary>
    /// Clears the controls.
    /// </summary>
    /// <param name="ele" type="Object" domElement="true">
    /// The ele.
    /// </param>
    /// <param name="except" type="com.ivp.rad.rscriptutils.ClearExcept">
    /// The except.
    /// </param>
    var inputCollection = null;
    switch (except) {
        case com.ivp.rad.rscriptutils.ClearExcept.NONE:
            inputCollection = ele.getElementsByTagName('INPUT');
            for (var i = 0; i < inputCollection.length; i++) {
                if (inputCollection[i].getAttribute('type').toString() === 'text' || inputCollection[i].getAttribute('type').toString() === 'password') {
                    (inputCollection[i]).value = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
                    (inputCollection[i]).disabled = false;
                }
                if (inputCollection[i].getAttribute('type').toString() === 'checkbox') {
                    (inputCollection[i]).checked = false;
                    (inputCollection[i]).disabled = false;
                }
            }
            inputCollection = ele.getElementsByTagName('SELECT');
            for (var i = 0; i < inputCollection.length; i++) {
                (inputCollection[i]).selectedIndex = 0;
                (inputCollection[i]).disabled = false;
            }
            inputCollection = ele.getElementsByTagName('LABEL');
            for (var i = 0; i < inputCollection.length; i++) {
                inputCollection[i].innerText = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
            }
            break;
        case com.ivp.rad.rscriptutils.ClearExcept.CHECKBOX:
            inputCollection = ele.getElementsByTagName('INPUT');
            for (var i = 0; i < inputCollection.length; i++) {
                if (inputCollection[i].getAttribute('type').toString() === 'text') {
                    (inputCollection[i]).value = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
                    (inputCollection[i]).disabled = false;
                }
            }
            inputCollection = ele.getElementsByTagName('SELECT');
            for (var i = 0; i < inputCollection.length; i++) {
                (inputCollection[i]).selectedIndex = 0;
            }
            inputCollection = ele.getElementsByTagName('LABEL');
            for (var i = 0; i < inputCollection.length; i++) {
                inputCollection[i].innerText = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
            }
            break;
        case com.ivp.rad.rscriptutils.ClearExcept.LABEL:
            inputCollection = ele.getElementsByTagName('INPUT');
            for (var i = 0; i < inputCollection.length; i++) {
                if (inputCollection[i].getAttribute('type').toString() === 'text') {
                    (inputCollection[i]).value = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
                    (inputCollection[i]).disabled = false;
                }
                if (inputCollection[i].getAttribute('type').toString() === 'checkbox') {
                    (inputCollection[i]).checked = false;
                    (inputCollection[i]).disabled = false;
                }
            }
            inputCollection = ele.getElementsByTagName('TEXTAREA');
            for (var i = 0; i < inputCollection.length; i++) {
                if (inputCollection[i].getAttribute('type').toString() === 'textarea') {
                    (inputCollection[i]).value = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
                    (inputCollection[i]).disabled = false;
                }
            }
            inputCollection = ele.getElementsByTagName('SELECT');
            for (var i = 0; i < inputCollection.length; i++) {
                (inputCollection[i]).selectedIndex = 0;
            }
            break;
        case com.ivp.rad.rscriptutils.ClearExcept.SELECT:
            inputCollection = ele.getElementsByTagName('INPUT');
            for (var i = 0; i < inputCollection.length; i++) {
                if (inputCollection[i].getAttribute('type').toString() === 'text') {
                    (inputCollection[i]).value = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
                    (inputCollection[i]).disabled = false;
                }
                if (inputCollection[i].getAttribute('type').toString() === 'checkbox') {
                    (inputCollection[i]).checked = false;
                    (inputCollection[i]).disabled = false;
                }
            }
            inputCollection = ele.getElementsByTagName('LABEL');
            for (var i = 0; i < inputCollection.length; i++) {
                inputCollection[i].innerText = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
            }
            break;
    }
}
com.ivp.rad.rscriptutils.RSCommonScripts.disableControlsPanel = function com_ivp_rad_rscriptutils_RSCommonScripts$disableControlsPanel(ele, disable) {
    /// <summary>
    /// Enables/Disables the controls in panel.
    /// </summary>
    /// <param name="ele" type="Object" domElement="true">
    /// The ele.
    /// </param>
    /// <param name="disable" type="Boolean">
    /// if set to <c>true</c> [disable].
    /// </param>
    var inputCollection = ele.getElementsByTagName('INPUT');
    for (var i = 0; i < inputCollection.length; i++) {
        if (inputCollection[i].getAttribute('type').toString() === 'text') {
            (inputCollection[i]).disabled = disable;
        }
        if (inputCollection[i].getAttribute('type').toString() === 'checkbox') {
            (inputCollection[i]).disabled = disable;
        }
        if (inputCollection[i].getAttribute('type').toString() === 'radio') {
            (inputCollection[i]).disabled = disable;
        }
    }
    inputCollection = ele.getElementsByTagName('SELECT');
    for (var i = 0; i < inputCollection.length; i++) {
        (inputCollection[i]).disabled = disable;
    }
    inputCollection = ele.getElementsByTagName('LABEL');
    for (var i = 0; i < inputCollection.length; i++) {
        inputCollection[i].disabled = disable;
    }
}
com.ivp.rad.rscriptutils.RSCommonScripts.createEmptyTable = function com_ivp_rad_rscriptutils_RSCommonScripts$createEmptyTable(element) {
    /// <summary>
    /// Creates the empty table.
    /// </summary>
    /// <param name="element" type="Object" domElement="true">
    /// </param>
    element.style.display = com.ivp.rad.rscriptutils.RSConstants.strinG_NONE;
    com.ivp.rad.rscriptutils.RSCommonScripts._createEmptyTableMessage(element);
}
com.ivp.rad.rscriptutils.RSCommonScripts._createEmptyTableMessage = function com_ivp_rad_rscriptutils_RSCommonScripts$_createEmptyTableMessage(element) {
    /// <summary>
    /// Creates the empty table message.
    /// </summary>
    /// <param name="element" type="Object" domElement="true">
    /// The element.
    /// </param>
    var mainDiv = com.ivp.rad.rscriptutils.RSCommonScripts.findControl(element, com.ivp.rad.rscriptutils.RSConstants.diV_ELEMENT.toLowerCase());
    var ele = document.createElement(com.ivp.rad.rscriptutils.RSConstants.diV_ELEMENT);
    ele.id = 'divId';
    var table = document.createElement(com.ivp.rad.rscriptutils.RSConstants.tablE_ELEMENT);
    var tr = table.insertRow();
    var td = tr.insertCell();
    ele.className = 'dataRow';
    td.innerText = 'There are no tasks defined.';
    ele.appendChild(table);
    mainDiv.appendChild(ele);
}
com.ivp.rad.rscriptutils.RSCommonScripts.showHideEmptyDataTable = function com_ivp_rad_rscriptutils_RSCommonScripts$showHideEmptyDataTable(show) {
    /// <summary>
    /// Shows the hide empty data table.
    /// </summary>
    /// <param name="show" type="Boolean">
    /// if set to <c>true</c> [show].
    /// </param>
    if (show) {
        var el = document.getElementById('divId');
        el.style.display = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    }
    else {
        var el = document.getElementById('divId');
        el.style.display = com.ivp.rad.rscriptutils.RSConstants.strinG_NONE;
    }
}
com.ivp.rad.rscriptutils.RSCommonScripts.emptyColumnName = function com_ivp_rad_rscriptutils_RSCommonScripts$emptyColumnName(textBoxId, chkBoxId) {
    /// <summary>
    /// Empties the name of the column.
    /// </summary>
    /// <param name="textBoxId" type="String">
    /// The text box id.
    /// </param>
    /// <param name="chkBoxId" type="String">
    /// The CHK box id.
    /// </param>
    var txtBx = document.getElementById(textBoxId);
    var regEx = new Array(1);
    regEx[0] = '^[\\w _]*$';
    if ((document.getElementById(chkBoxId)).checked) {
        if (com.ivp.rad.rscriptutils.RSValidators.requiredFieldValidator(txtBx, null, com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY)) {
            com.ivp.rad.rscriptutils.RSValidators.checkStringWithRegExOrString(txtBx, regEx, true, null, 'Column Name is not of desired format.');
        }
    }
}
com.ivp.rad.rscriptutils.RSCommonScripts.showHide = function com_ivp_rad_rscriptutils_RSCommonScripts$showHide(chkId, textBoxId, txtDefault) {
    /// <summary>
    /// Shows / Hides the default value.
    /// </summary>
    /// <param name="chkId" type="String">
    /// The CHK id.
    /// </param>
    /// <param name="textBoxId" type="String">
    /// The text box id.
    /// </param>
    /// <param name="txtDefault" type="String">
    /// The TXT default.
    /// </param>
    var textbox = document.getElementById(textBoxId);
    var checkBox = document.getElementById(chkId);
    var textDefault = document.getElementById(txtDefault);
    if (checkBox.checked) {
        textbox.readOnly = false;
    }
    else {
        textbox.readOnly = true;
        textbox.value = textDefault.value;
    }
}
com.ivp.rad.rscriptutils.RSCommonScripts.append = function com_ivp_rad_rscriptutils_RSCommonScripts$append(originalText, toAddText) {
    /// <summary>
    /// Appends the specified original text.
    /// </summary>
    /// <param name="originalText" type="String">
    /// The original text.
    /// </param>
    /// <param name="toAddText" type="String">
    /// To add text.
    /// </param>
    /// <returns type="String"></returns>
    return originalText + toAddText;
}
com.ivp.rad.rscriptutils.RSCommonScripts.getNumber = function com_ivp_rad_rscriptutils_RSCommonScripts$getNumber(text) {
    /// <summary>
    /// Gets the number from the numbers seperated by commas.
    /// </summary>
    /// <param name="text" type="String">
    /// The text.
    /// </param>
    /// <returns type="String"></returns>
    var numbers = [ '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', '-', '+' ];
    while (text.indexOf(',') > -1) {
        text = text.replace(',', '');
    }
    text = text.trim();
    var j = 0;
    for (var i = 0; i < text.length; i++) {
        if (i !== 0 && (text.charAt(i) === '-' || text.charAt(i) === '+')) {
            return '';
        }
        var textchar = text.charAt(i);
        if (textchar === '.') {
            j++;
        }
        if (j > 1) {
            return '';
        }
        if (!Array.contains(numbers, textchar)) {
            return '';
        }
    }
    return text;
}
com.ivp.rad.rscriptutils.RSCommonScripts.getTextValuePairFromXML = function com_ivp_rad_rscriptutils_RSCommonScripts$getTextValuePairFromXML(xml, text, value) {
    /// <summary>
    /// Gets the text value pair from XML.
    /// </summary>
    /// <param name="xml" type="String">
    /// The XML.
    /// </param>
    /// <param name="text" type="String">
    /// The text.
    /// </param>
    /// <param name="value" type="String">
    /// The value.
    /// </param>
    /// <returns type="Array"></returns>
    var DataText = com.ivp.rad.rscriptutils.RSCommonScripts.parseXMLToFindValues(xml, text, com.ivp.rad.rscriptutils.RSConstants.regeX_CHARACTER_SET);
    var DataValue = com.ivp.rad.rscriptutils.RSCommonScripts.parseXMLToFindValues(xml, value, com.ivp.rad.rscriptutils.RSConstants.regeX_ALPHA_NUMERIC);
    var list = [];
    if (DataText != null && DataValue != null && DataText.length === DataValue.length) {
        var info = null;
        for (var i = 0; i < DataText.length; i++) {
            info = new com.ivp.rad.rscriptutils.DropDownInfo();
            info.text = DataText[i];
            info.value = DataValue[i];
            Array.add(list, info);
        }
    }
    return list;
}
com.ivp.rad.rscriptutils.RSCommonScripts.parseXMLToFindValues = function com_ivp_rad_rscriptutils_RSCommonScripts$parseXMLToFindValues(xml, textToFind, regExPattern) {
    /// <summary>
    /// Parses the XML to find values.
    /// </summary>
    /// <param name="xml" type="String">
    /// The XML.
    /// </param>
    /// <param name="textToFind" type="String">
    /// The text to find.
    /// </param>
    /// <param name="regExPattern" type="String">
    /// The reg ex pattern.
    /// </param>
    /// <returns type="Array" elementType="String"></returns>
    var regEx = new RegExp('<' + textToFind + '>' + regExPattern + '</', 'gi');
    var dataText = xml.match(regEx);
    if (dataText != null) {
        for (var i = 0; i < dataText.length; i++) {
            dataText[i] = com.ivp.rad.rscriptutils.RSCommonScripts._returnsText(dataText[i]);
        }
    }
    return dataText;
}
com.ivp.rad.rscriptutils.RSCommonScripts._returnsText = function com_ivp_rad_rscriptutils_RSCommonScripts$_returnsText(text) {
    /// <summary>
    /// Returns the text.
    /// </summary>
    /// <param name="text" type="String">
    /// The text.
    /// </param>
    /// <returns type="String"></returns>
    return text.split('>')[1].substr(0, text.split('>')[1].length - 2);
}
com.ivp.rad.rscriptutils.RSCommonScripts.generateXML = function com_ivp_rad_rscriptutils_RSCommonScripts$generateXML(xml, key, value) {
    /// <summary>
    /// Generates the XML.
    /// </summary>
    /// <param name="xml" type="String">
    /// The XML (initially send "".
    /// </param>
    /// <param name="key" type="String">
    /// The key.
    /// </param>
    /// <param name="value" type="String">
    /// The value.
    /// </param>
    /// <returns type="String"></returns>
    return com.ivp.rad.rscriptutils.RSCommonScripts.append(xml, '<' + key + '>' + value + '</' + key + '>');
}
com.ivp.rad.rscriptutils.RSCommonScripts.bindDropDown = function com_ivp_rad_rscriptutils_RSCommonScripts$bindDropDown(list, dropDownToBind, control) {
    /// <summary>
    /// Binds the drop down.
    /// </summary>
    /// <param name="list" type="Array">
    /// The list.
    /// </param>
    /// <param name="dropDownToBind" type="Object" domElement="true">
    /// The drop down to bind.
    /// </param>
    /// <param name="control" type="com.ivp.rad.rscriptutils.BindingControl">
    /// The control.
    /// </param>
    com.ivp.rad.rscriptutils.RSCommonScripts.removeOptionsFromDdl(dropDownToBind);
    var option = null;
    var helper = new com.ivp.rad.rscriptutils.RSGUIElementHelper();
    if (control === com.ivp.rad.rscriptutils.BindingControl.dropDown) {
        option = helper.get_option();
        option.text = '--Select One--';
        option.value = '-1';
        dropDownToBind.add(option);
    }
    for (var i = 0; i < list.length; i++) {
        option = helper.get_option();
        option.text = (list[i]).text;
        option.value = (list[i]).value;
        dropDownToBind.add(option);
    }
}
com.ivp.rad.rscriptutils.RSCommonScripts.removeOptionsFromDdl = function com_ivp_rad_rscriptutils_RSCommonScripts$removeOptionsFromDdl(dropDownToBind) {
    /// <summary>
    /// Removes the options from DDL.
    /// </summary>
    /// <param name="dropDownToBind" type="Object" domElement="true">
    /// The drop down to bind.
    /// </param>
    var length = dropDownToBind.options.length;
    for (var i = 0; i < length; i++) {
        dropDownToBind.remove(0);
    }
}
com.ivp.rad.rscriptutils.RSCommonScripts.simpleEncrypt = function com_ivp_rad_rscriptutils_RSCommonScripts$simpleEncrypt(plainText) {
    /// <summary>
    /// Encrypts a string using caesar cipher.
    /// </summary>
    /// <param name="plainText" type="String">
    /// The plain text.
    /// </param>
    /// <returns type="String"></returns>
    var _map = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890';
    var shift = 5;
    var cipherText = new Sys.StringBuilder();
    var pos = 0;
    for (var i = 0; i < plainText.length; i++) {
        pos = _map.indexOf(plainText.charAt(i));
        if (pos >= 0) {
            cipherText.append(_map.charAt((pos + shift) % 62).toString());
        }
        else {
            cipherText.append(plainText.charAt(i).toString());
        }
    }
    return cipherText.toString();
}
com.ivp.rad.rscriptutils.RSCommonScripts.cleanWhitespace = function com_ivp_rad_rscriptutils_RSCommonScripts$cleanWhitespace(node) {
    /// <param name="node" type="Object" domElement="true">
    /// </param>
    var notWhitespace = new RegExp('\\S');
    var childNode = null;
    if (Sys.Browser.name !== 'Microsoft Internet Explorer') {
        for (var i = 0; i < node.childNodes.length; i++) {
            childNode = node.childNodes[i];
            if ((childNode.nodeType === 3) && (!notWhitespace.test(childNode.nodeValue))) {
                node.removeChild(node.childNodes[i]);
                i--;
            }
            if (childNode.nodeType === 1) {
                com.ivp.rad.rscriptutils.RSCommonScripts.cleanWhitespace(childNode);
            }
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rscriptutils.DropDownInfo

com.ivp.rad.rscriptutils.DropDownInfo = function com_ivp_rad_rscriptutils_DropDownInfo() {
    /// <summary>
    /// Drop Down Info
    /// </summary>
    /// <field name="text" type="String">
    /// Text of the DropDown
    /// </field>
    /// <field name="value" type="String">
    /// Value of the DropDown
    /// </field>
}
com.ivp.rad.rscriptutils.DropDownInfo.prototype = {
    text: null,
    value: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rscriptutils.RSConstants

com.ivp.rad.rscriptutils.RSConstants = function com_ivp_rad_rscriptutils_RSConstants() {
    /// <summary>
    /// Class containing all the internal constants
    /// </summary>
    /// <field name="cmD_ARG_ADD" type="String" static="true">
    /// </field>
    /// <field name="cmD_ARG_UPDATE" type="String" static="true">
    /// </field>
    /// <field name="cmD_ARG_DELETE" type="String" static="true">
    /// </field>
    /// <field name="cmD_ARG_DETAILS" type="String" static="true">
    /// </field>
    /// <field name="cmD_ARG_CANCEL" type="String" static="true">
    /// </field>
    /// <field name="cmD_ARG_OK" type="String" static="true">
    /// </field>
    /// <field name="strinG_EMPTY" type="String" static="true">
    /// </field>
    /// <field name="datetimE_EMPTY" type="String" static="true">
    /// </field>
    /// <field name="characteR_EMPTY" type="String" static="true">
    /// </field>
    /// <field name="diV_ELEMENT" type="String" static="true">
    /// </field>
    /// <field name="tablE_ELEMENT" type="String" static="true">
    /// </field>
    /// <field name="tablE_ROW_ELEMENT" type="String" static="true">
    /// </field>
    /// <field name="tablE_CELL_ELEMENT" type="String" static="true">
    /// </field>
    /// <field name="strinG_NONE" type="String" static="true">
    /// </field>
    /// <field name="strinG_NULL" type="String" static="true">
    /// </field>
    /// <field name="strinG_ZERO" type="String" static="true">
    /// </field>
    /// <field name="stylE_DISPLAY_BLOCK" type="String" static="true">
    /// </field>
    /// <field name="strinG_SEPERATOR" type="String" static="true">
    /// </field>
    /// <field name="evenT_CLICK" type="String" static="true">
    /// </field>
    /// <field name="evenT_CHANGE" type="String" static="true">
    /// </field>
    /// <field name="evenT_KEYUP" type="String" static="true">
    /// </field>
    /// <field name="evenT_DOUBLE_CLICK" type="String" static="true">
    /// </field>
    /// <field name="regeX_ALPHA_NUMARIC_SPACE" type="String" static="true">
    /// </field>
    /// <field name="regeX_ALPHA_NUMERIC_SPACE_UNDERSCORE" type="String" static="true">
    /// </field>
    /// <field name="regeX_ALPHA_NUMERIC_SPACE_UNDERSCORE_DOT" type="String" static="true">
    /// </field>
    /// <field name="regeX_ALPHA_NUMERIC_SPACE_UNDERSCORE_DOT_COLON_SLASH" type="String" static="true">
    /// </field>
    /// <field name="regeX_NUMERIC" type="String" static="true">
    /// </field>
    /// <field name="regeX_DECIMAL" type="String" static="true">
    /// </field>
    /// <field name="regeX_CHARACTER_SET" type="String" static="true">
    /// </field>
    /// <field name="regeX_ALPHA_NUMERIC" type="String" static="true">
    /// </field>
    /// <field name="regeX_DECIMAL_DOT" type="String" static="true">
    /// </field>
    /// <field name="interneT_EXPLORER" type="String" static="true">
    /// </field>
    /// <field name="mozillA_FIREFOX" type="String" static="true">
    /// </field>
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rscriptutils.RSCssClassConstants

com.ivp.rad.rscriptutils.RSCssClassConstants = function com_ivp_rad_rscriptutils_RSCssClassConstants() {
    /// <summary>
    /// Class containing CSS Contants
    /// </summary>
    /// <field name="INPUT" type="String" static="true">
    /// </field>
    /// <field name="updatE_CLIENT_BUTTON" type="String" static="true">
    /// </field>
    /// <field name="deletE_CLIENT_DISABLED_BUTTON" type="String" static="true">
    /// </field>
    /// <field name="detailS_CLIENT_BUTTON" type="String" static="true">
    /// </field>
    /// <field name="normaL_ROW" type="String" static="true">
    /// </field>
    /// <field name="alternatinG_ROW" type="String" static="true">
    /// </field>
    /// <field name="deletE_CLIENT_BUTTON" type="String" static="true">
    /// </field>
    /// <field name="HEADER" type="String" static="true">
    /// </field>
    /// <field name="datA_ROW" type="String" static="true">
    /// </field>
    /// <field name="griD_HEADER" type="String" static="true">
    /// </field>
    /// <field name="collapsE_GRID_BUTTON" type="String" static="true">
    /// </field>
    /// <field name="collapsE_GROUP_BUTTON" type="String" static="true">
    /// </field>
    /// <field name="expanD_GROUP_BUTTON" type="String" static="true">
    /// </field>
    /// <field name="expanD_GRID_BUTTON" type="String" static="true">
    /// </field>
    /// <field name="statE_ON_BUTTON" type="String" static="true">
    /// </field>
    /// <field name="statE_OFF_BUTTON" type="String" static="true">
    /// </field>
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rscriptutils.RSCultureManager

com.ivp.rad.rscriptutils.RSCultureManager = function com_ivp_rad_rscriptutils_RSCultureManager() {
    /// <summary>
    /// Culture Manager
    /// </summary>
    /// <field name="_cultureInfo" type="com.ivp.rad.rscriptutils.RSCultureInfo" static="true">
    /// </field>
    /// <field name="_serverCultureInfo" type="com.ivp.rad.rscriptutils.RSCultureInfo" static="true">
    /// </field>
}
com.ivp.rad.rscriptutils.RSCultureManager.GetCultureInfo = function com_ivp_rad_rscriptutils_RSCultureManager$GetCultureInfo() {
    /// <summary>
    /// Gets the culture info.
    /// </summary>
    /// <returns type="com.ivp.rad.rscriptutils.RSCultureInfo"></returns>
    return com.ivp.rad.rscriptutils.RSCultureManager._cultureInfo;
}
com.ivp.rad.rscriptutils.RSCultureManager.GetServerCultureInfo = function com_ivp_rad_rscriptutils_RSCultureManager$GetServerCultureInfo() {
    /// <summary>
    /// Gets the server culture info.
    /// </summary>
    /// <returns type="com.ivp.rad.rscriptutils.RSCultureInfo"></returns>
    return com.ivp.rad.rscriptutils.RSCultureManager._serverCultureInfo;
}
com.ivp.rad.rscriptutils.RSCultureManager.SetCultureInfo = function com_ivp_rad_rscriptutils_RSCultureManager$SetCultureInfo(serializedInfo, serializedServerInfo) {
    /// <summary>
    /// Sets the culture info.
    /// </summary>
    /// <param name="serializedInfo" type="String">
    /// The serialized info.
    /// </param>
    /// <param name="serializedServerInfo" type="String">
    /// The serialized server info.
    /// </param>
    com.ivp.rad.rscriptutils.RSCultureManager._cultureInfo = Sys.Serialization.JavaScriptSerializer.deserialize(serializedInfo);
    com.ivp.rad.rscriptutils.RSCultureManager._serverCultureInfo = Sys.Serialization.JavaScriptSerializer.deserialize(serializedServerInfo);
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rscriptutils.RSCultureInfo

com.ivp.rad.rscriptutils.RSCultureInfo = function com_ivp_rad_rscriptutils_RSCultureInfo() {
    /// <summary>
    /// Culture Info
    /// </summary>
    /// <field name="CultureName" type="String">
    /// </field>
    /// <field name="EnglishName" type="String">
    /// </field>
    /// <field name="ShortDateFormat" type="String">
    /// </field>
    /// <field name="LongDateFormat" type="String">
    /// </field>
    /// <field name="LongTimePattern" type="String">
    /// </field>
    /// <field name="ShortTimePattern" type="String">
    /// </field>
    /// <field name="CurrencySymbol" type="String">
    /// </field>
}
com.ivp.rad.rscriptutils.RSCultureInfo.prototype = {
    CultureName: null,
    EnglishName: null,
    ShortDateFormat: null,
    LongDateFormat: null,
    LongTimePattern: null,
    ShortTimePattern: null,
    CurrencySymbol: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rscriptutils.RMSCustomArray

com.ivp.rad.rscriptutils.RMSCustomArray = function com_ivp_rad_rscriptutils_RMSCustomArray() {
}
com.ivp.rad.rscriptutils.RMSCustomArray.contains = function com_ivp_rad_rscriptutils_RMSCustomArray$contains(array, item) {
    /// <param name="array" type="Array">
    /// </param>
    /// <param name="item" type="Object">
    /// </param>
    /// <returns type="Boolean"></returns>
    return Array.contains(array, item);
}
com.ivp.rad.rscriptutils.RMSCustomArray.indexOf = function com_ivp_rad_rscriptutils_RMSCustomArray$indexOf(array, item) {
    /// <param name="array" type="Array">
    /// </param>
    /// <param name="item" type="Object">
    /// </param>
    /// <returns type="Number" integer="true"></returns>
    return Array.indexOf(array, item);
}
com.ivp.rad.rscriptutils.RMSCustomArray.parse = function com_ivp_rad_rscriptutils_RMSCustomArray$parse(str) {
    /// <param name="str" type="String">
    /// </param>
    /// <returns type="Array"></returns>
    return Array.parse(str);
}
com.ivp.rad.rscriptutils.RMSCustomArray.add = function com_ivp_rad_rscriptutils_RMSCustomArray$add(array, item) {
    /// <param name="array" type="Array">
    /// </param>
    /// <param name="item" type="Object">
    /// </param>
    eval('Array.add(array, item);');
}
com.ivp.rad.rscriptutils.RMSCustomArray.remove = function com_ivp_rad_rscriptutils_RMSCustomArray$remove(array, item) {
    /// <param name="array" type="Array">
    /// </param>
    /// <param name="item" type="Object">
    /// </param>
    eval('Array.remove(array, item);');
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rscriptutils.RSCustomLogger

com.ivp.rad.rscriptutils.RSCustomLogger = function com_ivp_rad_rscriptutils_RSCustomLogger() {
}
com.ivp.rad.rscriptutils.RSCustomLogger.createLog = function com_ivp_rad_rscriptutils_RSCustomLogger$createLog(message) {
    /// <param name="message" type="String">
    /// </param>
    var fso = null;
    var fo = null;
    try {
        fso = new ActiveXObject('Scripting.FileSystemObject');
        if (fso.FileExists('C:\\Log.txt')) {
            fo = fso.OpenTextFile('C:\\Log.txt', 8, true);
        }
        else {
            fo = fso.CreateTextFile('C:\\Log.txt', true);
        }
        fo.WriteLine(new Date().toDateString() + ' ' + new Date().toTimeString() + '=>' + message);
        fo.WriteLine('-----------------------------------');
        fo.Close();
    }
    catch ($e1) {
        alert('Could Not Log');
    }
    finally {
        if (fo != null) {
            fo.Close();
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rscriptutils.RSDateTimeUtils

com.ivp.rad.rscriptutils.RSDateTimeUtils = function com_ivp_rad_rscriptutils_RSDateTimeUtils() {
}
com.ivp.rad.rscriptutils.RSDateTimeUtils.convertStringToDateTime = function com_ivp_rad_rscriptutils_RSDateTimeUtils$convertStringToDateTime(date, format) {
    /// <summary>
    /// Converts the string to date time.
    /// </summary>
    /// <param name="date" type="String">
    /// The date is of current culture format.
    /// </param>
    /// <param name="format" type="com.ivp.rad.rscriptutils.DateTimeFormat">
    /// Datetime format of date passed as parameter.
    /// </param>
    /// <returns type="Date"></returns>
    var dateTime = new Date();
    var sourceFormat = com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(format);
    dateTime = Date.parseInvariant(date, sourceFormat);
    return dateTime;
}
com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString = function com_ivp_rad_rscriptutils_RSDateTimeUtils$convertDateTimeToString(date, format) {
    /// <summary>
    /// Converts the date time to string.
    /// </summary>
    /// <param name="date" type="Date">
    /// DateTime date.
    /// </param>
    /// <param name="format" type="String">
    /// target format of type string.
    /// </param>
    /// <returns type="String"></returns>
    return date.format(format);
}
com.ivp.rad.rscriptutils.RSDateTimeUtils.getISOFormat = function com_ivp_rad_rscriptutils_RSDateTimeUtils$getISOFormat(format) {
    /// <summary>
    /// Gets the ISO format.
    /// </summary>
    /// <param name="format" type="com.ivp.rad.rscriptutils.DateTimeFormat">
    /// The Datetime format.
    /// </param>
    /// <returns type="String"></returns>
    var strFormat = null;
    switch (format) {
        case com.ivp.rad.rscriptutils.DateTimeFormat.longDate:
            strFormat = 'yyyyMMdd HH:mm:ss';
            break;
        case com.ivp.rad.rscriptutils.DateTimeFormat.shorDate:
            strFormat = 'yyyyMMdd';
            break;
    }
    return strFormat;
}
com.ivp.rad.rscriptutils.RSDateTimeUtils.getISOFormattedString = function com_ivp_rad_rscriptutils_RSDateTimeUtils$getISOFormattedString(date, format) {
    /// <summary>
    /// Gets the ISO formatted string.
    /// </summary>
    /// <param name="date" type="String">
    /// The date entered by user of string format.
    /// </param>
    /// <param name="format" type="com.ivp.rad.rscriptutils.DateTimeFormat">
    /// The DateTimeFormat format.
    /// </param>
    /// <returns type="String"></returns>
    var dateTime = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertStringToDateTime(date, format);
    var dateFormat = com.ivp.rad.rscriptutils.RSDateTimeUtils.getISOFormat(format);
    return com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(dateTime, dateFormat);
}
com.ivp.rad.rscriptutils.RSDateTimeUtils.getCulturedDateFromISO = function com_ivp_rad_rscriptutils_RSDateTimeUtils$getCulturedDateFromISO(date, format) {
    /// <summary>
    /// Gets the cultured date from ISO.
    /// </summary>
    /// <param name="date" type="String">
    /// The date user input in string format.
    /// </param>
    /// <param name="format" type="com.ivp.rad.rscriptutils.DateTimeFormat">
    /// The DateTimeFormat format.
    /// </param>
    /// <returns type="String"></returns>
    var strFormat = com.ivp.rad.rscriptutils.RSDateTimeUtils.getISOFormat(format);
    var dateTime = new Date();
    dateTime = Date.parseInvariant(date, strFormat);
    strFormat = com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(format);
    return com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(dateTime, strFormat);
}
com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat = function com_ivp_rad_rscriptutils_RSDateTimeUtils$getCultureFormat(format) {
    /// <summary>
    /// Gets the culture format.
    /// </summary>
    /// <param name="format" type="com.ivp.rad.rscriptutils.DateTimeFormat">
    /// The Datetime format.
    /// </param>
    /// <returns type="String"></returns>
    var dateFormat = null;
    var cultureInfo = com.ivp.rad.rscriptutils.RSCultureManager.GetCultureInfo();
    switch (format) {
        case com.ivp.rad.rscriptutils.DateTimeFormat.longDate:
            dateFormat = cultureInfo.LongDateFormat;
            break;
        case com.ivp.rad.rscriptutils.DateTimeFormat.longTime:
            dateFormat = cultureInfo.LongTimePattern;
            break;
        case com.ivp.rad.rscriptutils.DateTimeFormat.shorDate:
            dateFormat = cultureInfo.ShortDateFormat;
            break;
        case com.ivp.rad.rscriptutils.DateTimeFormat.shortTime:
            dateFormat = cultureInfo.ShortTimePattern;
            break;
    }
    return dateFormat;
}
com.ivp.rad.rscriptutils.RSDateTimeUtils.getServerCultureFormat = function com_ivp_rad_rscriptutils_RSDateTimeUtils$getServerCultureFormat(format) {
    /// <summary>
    /// Gets the server culture format.
    /// </summary>
    /// <param name="format" type="com.ivp.rad.rscriptutils.DateTimeFormat">
    /// The format.
    /// </param>
    /// <returns type="String"></returns>
    var dateFormat = null;
    var cultureInfo = com.ivp.rad.rscriptutils.RSCultureManager.GetServerCultureInfo();
    switch (format) {
        case com.ivp.rad.rscriptutils.DateTimeFormat.longDate:
            dateFormat = cultureInfo.LongDateFormat;
            break;
        case com.ivp.rad.rscriptutils.DateTimeFormat.longTime:
            dateFormat = cultureInfo.LongTimePattern;
            break;
        case com.ivp.rad.rscriptutils.DateTimeFormat.shorDate:
            dateFormat = cultureInfo.ShortDateFormat;
            break;
        case com.ivp.rad.rscriptutils.DateTimeFormat.shortTime:
            dateFormat = cultureInfo.ShortTimePattern;
            break;
    }
    return dateFormat;
}
com.ivp.rad.rscriptutils.RSDateTimeUtils.getServerDateFromISO = function com_ivp_rad_rscriptutils_RSDateTimeUtils$getServerDateFromISO(date, format) {
    /// <summary>
    /// Gets the server date from ISO format.
    /// </summary>
    /// <param name="date" type="String">
    /// The iso formatted date in string format.
    /// </param>
    /// <param name="format" type="com.ivp.rad.rscriptutils.DateTimeFormat">
    /// The DateTime format.
    /// </param>
    /// <returns type="String"></returns>
    var serverFormat = com.ivp.rad.rscriptutils.RSDateTimeUtils.getServerCultureFormat(format);
    var dateTime = Date.parseInvariant(date, com.ivp.rad.rscriptutils.RSDateTimeUtils.getISOFormat(format));
    return com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(dateTime, serverFormat);
}
com.ivp.rad.rscriptutils.RSDateTimeUtils.getISODateFromServerDate = function com_ivp_rad_rscriptutils_RSDateTimeUtils$getISODateFromServerDate(date, format) {
    /// <summary>
    /// Gets the ISO date from server date.
    /// </summary>
    /// <param name="date" type="String">
    /// The date in server format.
    /// </param>
    /// <param name="format" type="com.ivp.rad.rscriptutils.DateTimeFormat">
    /// The DateTime format.
    /// </param>
    /// <returns type="String"></returns>
    var serverFormat = com.ivp.rad.rscriptutils.RSDateTimeUtils.getServerCultureFormat(format);
    var dateTime = Date.parseInvariant(date, serverFormat);
    return com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(dateTime, com.ivp.rad.rscriptutils.RSDateTimeUtils.getISOFormat(format));
}
com.ivp.rad.rscriptutils.RSDateTimeUtils.getServerFormatFromTicks = function com_ivp_rad_rscriptutils_RSDateTimeUtils$getServerFormatFromTicks(date, format) {
    /// <summary>
    /// Gets the server format from ticks.
    /// </summary>
    /// <param name="date" type="String">
    /// The date.
    /// </param>
    /// <param name="format" type="com.ivp.rad.rscriptutils.DateTimeFormat">
    /// The format.
    /// </param>
    /// <returns type="String"></returns>
    var ticks = com.ivp.rad.rscriptutils.RSValidators.returnTicks(date);
    var dateTime = new Date(ticks);
    return dateTime.format(com.ivp.rad.rscriptutils.RSDateTimeUtils.getServerCultureFormat(format));
}
com.ivp.rad.rscriptutils.RSDateTimeUtils.getServerDateFromCulturedDate = function com_ivp_rad_rscriptutils_RSDateTimeUtils$getServerDateFromCulturedDate(date, format) {
    /// <param name="date" type="String">
    /// </param>
    /// <param name="format" type="com.ivp.rad.rscriptutils.DateTimeFormat">
    /// </param>
    /// <returns type="String"></returns>
    return com.ivp.rad.rscriptutils.RSDateTimeUtils.getServerDateFromISO(com.ivp.rad.rscriptutils.RSDateTimeUtils.getISOFormattedString(date, format), format);
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rscriptutils.RSEvalManager

com.ivp.rad.rscriptutils.RSEvalManager = function com_ivp_rad_rscriptutils_RSEvalManager() {
}
com.ivp.rad.rscriptutils.RSEvalManager.rowIndex = function com_ivp_rad_rscriptutils_RSEvalManager$rowIndex(row) {
    /// <summary>
    /// Returns the rowIndex.
    /// </summary>
    /// <param name="row" type="Object" domElement="true">
    /// The row.
    /// </param>
    /// <returns type="Number" integer="true"></returns>
    var el = row;
    var tr = el;
    return tr.rowIndex;
}
com.ivp.rad.rscriptutils.RSEvalManager.sourceIndex = function com_ivp_rad_rscriptutils_RSEvalManager$sourceIndex(el) {
    /// <summary>
    /// Returns the Sources index.
    /// </summary>
    /// <param name="el" type="Object" domElement="true">
    /// The el.
    /// </param>
    /// <returns type="String"></returns>
    var tr = el;
    return tr.sourceIndex;
}
com.ivp.rad.rscriptutils.RSEvalManager.uniqueId = function com_ivp_rad_rscriptutils_RSEvalManager$uniqueId(el) {
    /// <summary>
    /// Returns the Unique id.
    /// </summary>
    /// <param name="el" type="Object" domElement="true">
    /// The el.
    /// </param>
    /// <returns type="String"></returns>
    var tr = el;
    return tr.uniqueID;
}
com.ivp.rad.rscriptutils.RSEvalManager.cellIndex = function com_ivp_rad_rscriptutils_RSEvalManager$cellIndex(el) {
    /// <summary>
    /// Returns the Cells index.
    /// </summary>
    /// <param name="el" type="Object" domElement="true">
    /// The el.
    /// </param>
    /// <returns type="Object"></returns>
    var tr = el;
    return tr.cellIndex;
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rscriptutils.RSGUIElementHelper

com.ivp.rad.rscriptutils.RSGUIElementHelper = function com_ivp_rad_rscriptutils_RSGUIElementHelper() {
}
com.ivp.rad.rscriptutils.RSGUIElementHelper.prototype = {
    
    get_table: function com_ivp_rad_rscriptutils_RSGUIElementHelper$get_table() {
        /// <value type="Object" domElement="true"></value>
        var table = document.createElement('TABLE');
        table.setAttribute('border', '0');
        table.setAttribute('cellSpacing', '0px');
        table.setAttribute('cellPadding', '0px');
        table.setAttribute('width', '100%');
        return table;
    },
    
    get_tBody: function com_ivp_rad_rscriptutils_RSGUIElementHelper$get_tBody() {
        /// <value type="Object" domElement="true"></value>
        var tBody = document.createElement('TBODY');
        return tBody;
    },
    
    get_tHead: function com_ivp_rad_rscriptutils_RSGUIElementHelper$get_tHead() {
        /// <value type="Object" domElement="true"></value>
        var tHead = document.createElement('THEAD');
        return tHead;
    },
    
    get_tableRow: function com_ivp_rad_rscriptutils_RSGUIElementHelper$get_tableRow() {
        /// <value type="Object" domElement="true"></value>
        var tr = document.createElement('TR');
        return tr;
    },
    
    get_tableCell: function com_ivp_rad_rscriptutils_RSGUIElementHelper$get_tableCell() {
        /// <value type="Object" domElement="true"></value>
        var td = document.createElement('TD');
        return td;
    },
    
    get_tableHeaderCell: function com_ivp_rad_rscriptutils_RSGUIElementHelper$get_tableHeaderCell() {
        /// <value type="Object" domElement="true"></value>
        var th = document.createElement('TH');
        return th;
    },
    
    get_textBox: function com_ivp_rad_rscriptutils_RSGUIElementHelper$get_textBox() {
        /// <value type="Object" domElement="true"></value>
        var textBox = document.createElement('TEXTAREA');
        return textBox;
    },
    
    get_checkBox: function com_ivp_rad_rscriptutils_RSGUIElementHelper$get_checkBox() {
        /// <value type="Object" domElement="true"></value>
        var chk = document.createElement('INPUT');
        chk.type = 'checkbox';
        return chk;
    },
    
    get_buttonElement: function com_ivp_rad_rscriptutils_RSGUIElementHelper$get_buttonElement() {
        /// <value type="Object" domElement="true"></value>
        var el_button = document.createElement('INPUT');
        el_button.type = 'button';
        return el_button;
    },
    
    get_selectElement: function com_ivp_rad_rscriptutils_RSGUIElementHelper$get_selectElement() {
        /// <value type="Object" domElement="true"></value>
        var selectElement = document.createElement('SELECT');
        return selectElement;
    },
    
    get_option: function com_ivp_rad_rscriptutils_RSGUIElementHelper$get_option() {
        /// <value type="Object" domElement="true"></value>
        var option = document.createElement('OPTION');
        return option;
    },
    
    get_label: function com_ivp_rad_rscriptutils_RSGUIElementHelper$get_label() {
        /// <value type="Object" domElement="true"></value>
        var el_Label = document.createElement('LABEL');
        return el_Label;
    }
}


////////////////////////////////////////////////////////////////////////////////
// LINQ

LINQ = function LINQ(dataItems) {
    /// <param name="dataItems" type="Array">
    /// </param>
    /// <field name="_items" type="Array">
    /// </field>
    this._items = dataItems;
}
LINQ.prototype = {
    _items: null,
    
    ToArray: function LINQ$ToArray() {
        /// <returns type="Array"></returns>
        return this._items;
    },
    
    Where: function LINQ$Where(clause) {
        /// <param name="clause" type="WhereClause">
        /// </param>
        /// <returns type="LINQ"></returns>
        if (clause == null) {
            throw Error.create('Where Clause is not set.');
        }
        var newArray = [];
        for (var index = 0; index < this._items.length; index++) {
            if (clause(this._items[index], index)) {
                newArray[newArray.length] = this._items[index];
            }
        }
        return new LINQ(newArray);
    },
    
    Select: function LINQ$Select(clause) {
        /// <param name="clause" type="SelectClause">
        /// </param>
        /// <returns type="LINQ"></returns>
        if (clause == null) {
            throw Error.create('Select Clause is not set.');
        }
        var newArray = [];
        for (var i = 0; i < this._items.length; i++) {
            if (clause(this._items[i]) != null) {
                newArray[newArray.length] = clause(this._items[i]);
            }
        }
        return new LINQ(newArray);
    },
    
    OrderBy: function LINQ$OrderBy(clause) {
        /// <param name="clause" type="OrderByClause">
        /// </param>
        /// <returns type="LINQ"></returns>
        var tempArray = [];
        for (var i = 0; i < this._items.length; i++) {
            tempArray[tempArray.length] = this._items[i];
        }
        if (clause != null) {
            tempArray.sort(Function.createDelegate(this, function(a, b) {
                var x = clause(a);
                var y = clause(b);
                return ((x < y) ? -1 : ((x > y) ? 1 : 0));
            }));
        }
        else {
            tempArray.sort();
        }
        return new LINQ(tempArray);
    },
    
    OrderByDescending: function LINQ$OrderByDescending(clause) {
        /// <param name="clause" type="OrderByDescendingClause">
        /// </param>
        /// <returns type="LINQ"></returns>
        var tempArray = [];
        for (var i = 0; i < this._items.length; i++) {
            tempArray[tempArray.length] = this._items[i];
        }
        if (clause != null) {
            tempArray.sort(Function.createDelegate(this, function(a, b) {
                var x = clause(b);
                var y = clause(a);
                return ((x < y) ? -1 : ((x > y) ? 1 : 0));
            }));
        }
        else {
            tempArray.sort();
        }
        return new LINQ(tempArray);
    },
    
    SelectMany: function LINQ$SelectMany(clause) {
        /// <param name="clause" type="SelectManyClause">
        /// </param>
        /// <returns type="LINQ"></returns>
        if (clause == null) {
            throw Error.create('SelectMany Clause is not set.');
        }
        var r = [];
        for (var i = 0; i < this._items.length; i++) {
            r = r.concat(clause(this._items[i]));
        }
        return new LINQ(r);
    },
    
    Count: function LINQ$Count(clause) {
        /// <param name="clause" type="WhereClause">
        /// </param>
        /// <returns type="Number" integer="true"></returns>
        if (clause == null) {
            return this._items.length;
        }
        else {
            var claused = this.Where(clause);
            return claused._items.length;
        }
    },
    
    Distinct: function LINQ$Distinct() {
        /// <returns type="LINQ"></returns>
        var unique = [];
        for (var i = 0; i < this._items.length; i++) {
            if (!Array.contains(unique, this._items[i])) {
                Array.add(unique, this._items[i]);
            }
        }
        return new LINQ(unique);
    },
    
    Any: function LINQ$Any(clause) {
        /// <param name="clause" type="AnyClause">
        /// </param>
        /// <returns type="Boolean"></returns>
        if (clause == null) {
            throw Error.create('Any Clause is not set.');
        }
        for (var index = 0; index < this._items.length; index++) {
            if (clause(this._items[index], index)) {
                return true;
            }
        }
        return false;
    },
    
    All: function LINQ$All(clause) {
        /// <param name="clause" type="AllClause">
        /// </param>
        /// <returns type="Boolean"></returns>
        if (clause == null) {
            throw Error.create('All Clause is not set.');
        }
        for (var index = 0; index < this._items.length; index++) {
            if (!clause(this._items[index], index)) {
                return false;
            }
        }
        return true;
    },
    
    Reverse: function LINQ$Reverse() {
        /// <returns type="LINQ"></returns>
        var retVal = [];
        for (var index = this._items.length - 1; index > -1; index--) {
            retVal[retVal.length] = this._items[index];
        }
        return new LINQ(retVal);
    },
    
    First: function LINQ$First(clause) {
        /// <param name="clause" type="WhereClause">
        /// </param>
        /// <returns type="Object"></returns>
        if (clause != null) {
            return this.Where(clause).First(null);
        }
        else {
            if (this._items.length > 0) {
                return this._items[0];
            }
            else {
                return null;
            }
        }
    },
    
    Last: function LINQ$Last(clause) {
        /// <param name="clause" type="WhereClause">
        /// </param>
        /// <returns type="Object"></returns>
        if (clause != null) {
            return this.Where(clause).Last(null);
        }
        else {
            if (this._items.length > 0) {
                return this._items[this._items.length - 1];
            }
            else {
                return null;
            }
        }
    },
    
    ElementAt: function LINQ$ElementAt(index) {
        /// <param name="index" type="Number" integer="true">
        /// </param>
        /// <returns type="Object"></returns>
        return this._items[index];
    },
    
    Concat: function LINQ$Concat(array) {
        /// <param name="array" type="Array">
        /// </param>
        /// <returns type="LINQ"></returns>
        return new LINQ(this._items.concat(array));
    },
    
    Intersect: function LINQ$Intersect(secondArray, clause) {
        /// <param name="secondArray" type="Array">
        /// </param>
        /// <param name="clause" type="IntersectClause">
        /// </param>
        /// <returns type="LINQ"></returns>
        var clauseMethod = clause;
        if (clauseMethod == null) {
            clauseMethod = Function.createDelegate(this, function(item1, index1, item2, index2) {
                return item1 === item2;
            });
        }
        var result = [];
        for (var a = 0; a < this._items.length; a++) {
            for (var b = 0; b < secondArray.length; b++) {
                if (clauseMethod(this._items[a], a, secondArray[b], b)) {
                    result[result.length] = this._items[a];
                }
            }
        }
        return new LINQ(result);
    },
    
    DefaultIfEmpty: function LINQ$DefaultIfEmpty(defaultValue) {
        /// <param name="defaultValue" type="Object">
        /// </param>
        /// <returns type="LINQ"></returns>
        if (this._items.length === 0) {
            return defaultValue;
        }
        return this;
    },
    
    ElementAtOrDefault: function LINQ$ElementAtOrDefault(index, defaultValue) {
        /// <param name="index" type="Number" integer="true">
        /// </param>
        /// <param name="defaultValue" type="Object">
        /// </param>
        /// <returns type="Object"></returns>
        if (index >= 0 && index < this._items.length) {
            return this._items[index];
        }
        return defaultValue;
    },
    
    FirstOrDefault: function LINQ$FirstOrDefault(defaultValue) {
        /// <param name="defaultValue" type="Object">
        /// </param>
        /// <returns type="Object"></returns>
        return (this.First(null) != null) ? this.First(null) : defaultValue;
    },
    
    LastOrDefault: function LINQ$LastOrDefault(defaultValue) {
        /// <param name="defaultValue" type="Object">
        /// </param>
        /// <returns type="Object"></returns>
        return (this.Last(null) == null) ? this.Last(null) : defaultValue;
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rscriptutils.RSProfileManager

com.ivp.rad.rscriptutils.RSProfileManager = function com_ivp_rad_rscriptutils_RSProfileManager() {
    /// <field name="_themeName" type="String" static="true">
    /// </field>
    /// <field name="_themeButton" type="Object" domElement="true" static="true">
    /// </field>
    /// <field name="_oldThemeName" type="String" static="true">
    /// </field>
}
com.ivp.rad.rscriptutils.RSProfileManager.changeTheme = function com_ivp_rad_rscriptutils_RSProfileManager$changeTheme(newThemeButton, newTheme) {
    /// <summary>
    /// Changes the theme.
    /// </summary>
    /// <param name="newThemeButton" type="Object" domElement="true">
    /// The new theme button.
    /// </param>
    /// <param name="newTheme" type="String">
    /// The new theme.
    /// </param>
    com.ivp.rad.rscriptutils.RSProfileManager._themeName = newTheme;
    com.ivp.rad.rscriptutils.RSProfileManager._themeButton = newThemeButton;
    if (Sys.Browser.name === com.ivp.rad.rscriptutils.RSConstants.interneT_EXPLORER) {
        var callBack = Function.createDelegate(null, com.ivp.rad.rscriptutils.RSProfileManager._loadImages);
        window.setTimeout(callBack, 50);
        var collection = window.document.getElementsByTagName('LINK');
        var anchorElement = null;
        var oldTheme = null;
        var href = null;
        var regEx = null;
        for (var i = 0; i < collection.length; i++) {
            anchorElement = collection[i];
            href = anchorElement.href;
            var hrefSplits = href.split('/');
            if (Sys.Browser.name === com.ivp.rad.rscriptutils.RSConstants.interneT_EXPLORER) {
                if (hrefSplits.length > 2) {
                    oldTheme = hrefSplits[1];
                }
            }
            else if (Sys.Browser.name === com.ivp.rad.rscriptutils.RSConstants.mozillA_FIREFOX) {
                if (hrefSplits.length > 5) {
                    oldTheme = hrefSplits[4];
                }
            }
            if (oldTheme != null) {
                regEx = new RegExp(oldTheme, 'gi');
                href = href.replace(regEx, newTheme);
                anchorElement.href = href;
            }
        }
        collection = window.document.getElementsByTagName('INPUT');
        var inputImageElement = null;
        for (var i = 0; i < collection.length; i++) {
            inputImageElement = collection[i];
            if (inputImageElement.type.toLowerCase() !== 'image') {
                break;
            }
            href = (inputImageElement).src;
            var hrefSplits = href.split('/');
            if (Sys.Browser.name === com.ivp.rad.rscriptutils.RSConstants.interneT_EXPLORER) {
                if (hrefSplits.length > 2) {
                    oldTheme = hrefSplits[1];
                }
            }
            else if (Sys.Browser.name === com.ivp.rad.rscriptutils.RSConstants.mozillA_FIREFOX) {
                if (hrefSplits.length > 5) {
                    oldTheme = hrefSplits[4];
                }
            }
            if (oldTheme != null) {
                regEx = new RegExp(oldTheme, 'gi');
                href = href.replace(regEx, newTheme);
                (inputImageElement).src = href;
            }
        }
        com.ivp.rad.rscriptutils.RSProfileManager._oldThemeName = oldTheme;
        callBack = Function.createDelegate(null, com.ivp.rad.rscriptutils.RSProfileManager._changeTreeTheme);
        window.setTimeout(callBack, 100);
    }
}
com.ivp.rad.rscriptutils.RSProfileManager._changeTreeTheme = function com_ivp_rad_rscriptutils_RSProfileManager$_changeTreeTheme() {
    /// <summary>
    /// Changes the tree theme.
    /// </summary>
    var collection = window.document.getElementsByTagName('A');
    var regExTree = new RegExp('TreeView_ToggleNode');
    var nodeData = null;
    var href = null;
    var regEx = new RegExp(com.ivp.rad.rscriptutils.RSProfileManager._oldThemeName, 'gi');
    var anchorElement = null;
    for (var i = 0; i < collection.length; i++) {
        anchorElement = collection[i];
        href = anchorElement.href;
        if (href.search(regExTree) > 0) {
            nodeData = eval(href.split('(')[1].split(',')[0]);
            for (var j = 0; j < nodeData.images.length; j++) {
                nodeData.images[j] = nodeData.images[j].replace(regEx, com.ivp.rad.rscriptutils.RSProfileManager._themeName);
            }
        }
    }
}
com.ivp.rad.rscriptutils.RSProfileManager._loadImages = function com_ivp_rad_rscriptutils_RSProfileManager$_loadImages() {
    /// <summary>
    /// Loads the images.
    /// </summary>
    var regEx = new RegExp(com.ivp.rad.rscriptutils.RSProfileManager._oldThemeName, 'gi');
    var src = null;
    var imageElement = null;
    var newThemeButton = com.ivp.rad.rscriptutils.RSProfileManager._themeButton;
    var newTheme = com.ivp.rad.rscriptutils.RSProfileManager._themeName;
    var handler = Function.createDelegate(null, com.ivp.rad.rscriptutils.RSProfileManager.onImageError);
    var imgCollection = window.document.getElementsByTagName('IMG');
    for (var i = 0; i < imgCollection.length; i++) {
        imageElement = imgCollection[i];
        src = imageElement.src;
        imageElement.attachEvent('onerror', Function.createDelegate(null, com.ivp.rad.rscriptutils.RSProfileManager.onImageError));
        src = src.replace(regEx, newTheme);
        imageElement.src = src;
    }
    var hiddenThemeName = window.document.getElementById('themeName');
    if (hiddenThemeName.value.trim() === '') {
        hiddenThemeName.value = com.ivp.rad.rscriptutils.RSProfileManager._oldThemeName;
    }
    var tr = null;
    var buttonCollections = null;
    if (newThemeButton != null) {
        tr = newThemeButton.parentNode.parentNode;
        buttonCollections = tr.getElementsByTagName('INPUT');
        for (var i = 0; i < buttonCollections.length; i++) {
            buttonCollections[i].className = 'thumbnails';
        }
        newThemeButton.className = 'selectedThumbnail';
    }
    else {
        tr = window.document.getElementById('themeThumbnailContainer');
        buttonCollections = tr.getElementsByTagName('INPUT');
        regEx = new RegExp(newTheme);
        for (var i = 0; i < buttonCollections.length; i++) {
            if (buttonCollections[i].id.search(regEx) > 0) {
                buttonCollections[i].className = 'selectedThumbnail';
            }
            else {
                buttonCollections[i].className = 'thumbnails';
            }
        }
    }
}
com.ivp.rad.rscriptutils.RSProfileManager.saveProfile = function com_ivp_rad_rscriptutils_RSProfileManager$saveProfile() {
    /// <summary>
    /// Saves the profile.
    /// </summary>
    var cell = document.getElementById('tdLanguage');
    var selectedLanguage = (cell.childNodes[0]).value;
    var tr = null;
    var buttonCollections = null;
    var radAPI = new com.ivp.rad.rradapi.RRadAPI();
    if (com.ivp.rad.rscriptutils.RSProfileManager._themeName == null) {
        tr = window.document.getElementById('themeThumbnailContainer');
        buttonCollections = tr.getElementsByTagName('INPUT');
        for (var i = 0; i < buttonCollections.length; i++) {
            if (buttonCollections[i].className === 'selectedThumbnail') {
                com.ivp.rad.rscriptutils.RSProfileManager._themeName = buttonCollections[i].getAttribute('onclick').toString().split('\'')[1];
                break;
            }
        }
    }
    radAPI.SaveUserProfile(com.ivp.rad.rscriptutils.RSProfileManager._themeName, selectedLanguage, Function.createDelegate(null, com.ivp.rad.rscriptutils.RSProfileManager._succeededCallbackWithContext), Function.createDelegate(null, com.ivp.rad.rscriptutils.RSProfileManager._failedCallbackWithContext));
    var hiddenThemeName = window.document.getElementById('themeName');
    hiddenThemeName.value = com.ivp.rad.rscriptutils.RSProfileManager._themeName;
    var iDashFrame = document.getElementById('iDashboardFrame');
    if (iDashFrame != null) {
        var loc = iDashFrame.contentWindow.location;
        eval('loc.reload();');
        iDashFrame.style.display = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    }
    var iReportViewer = document.getElementById('iFrameMainReportViewerContainer');
    if (iReportViewer != null) {
        var btnThemePostBack = document.getElementById('btnThemePostBack');
        btnThemePostBack.click();
    }
}
com.ivp.rad.rscriptutils.RSProfileManager.onImageError = function com_ivp_rad_rscriptutils_RSProfileManager$onImageError() {
    /// <summary>
    /// Called when [image error].
    /// </summary>
    var img = window.event.srcElement;
    if (img.src.endsWith('png')) {
        img.src = img.src.replace('.png', '.jpg');
    }
    else if (img.src.endsWith('jpg')) {
        img.src = img.src.replace('.jpg', '.png');
    }
    window.event.cancelBubble = true;
}
com.ivp.rad.rscriptutils.RSProfileManager._succeededCallbackWithContext = function com_ivp_rad_rscriptutils_RSProfileManager$_succeededCallbackWithContext(stringResult, eventArgs) {
    /// <summary>
    /// the callback with context when succeeded.
    /// </summary>
    /// <param name="stringResult" type="Object">
    /// The string result.
    /// </param>
    /// <param name="eventArgs" type="Object">
    /// The event args.
    /// </param>
    var cell = document.getElementById('tdLanguage');
    var selectedLanguage = (cell.childNodes[0]).value;
    var oldLanguage = (document.getElementById('cultureName')).value;
    if (oldLanguage !== selectedLanguage) {
        var loc = window.location;
        eval('loc.reload();');
    }
}
com.ivp.rad.rscriptutils.RSProfileManager._failedCallbackWithContext = function com_ivp_rad_rscriptutils_RSProfileManager$_failedCallbackWithContext(args) {
    /// <summary>
    /// the callback with context when failed.
    /// </summary>
    /// <param name="args" type="Object">
    /// The args.
    /// </param>
    var ex = args;
    alert('WebService Error: Code :' + ex.get_statusCode() + ' : ' + ex.get_message());
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.rscriptutils.RSValidators

com.ivp.rad.rscriptutils.RSValidators = function com_ivp_rad_rscriptutils_RSValidators() {
    /// <summary>
    /// Class containing static methods to validate controls
    /// </summary>
}
com.ivp.rad.rscriptutils.RSValidators.getObject = function com_ivp_rad_rscriptutils_RSValidators$getObject(cID) {
    /// <summary>
    /// Gets the object.
    /// </summary>
    /// <param name="cID" type="String">
    /// The c ID.
    /// </param>
    /// <returns type="Object" domElement="true"></returns>
    return document.getElementById(cID);
}
com.ivp.rad.rscriptutils.RSValidators._isTextBoxEmpty = function com_ivp_rad_rscriptutils_RSValidators$_isTextBoxEmpty(ele) {
    /// <summary>
    /// Determines whether [is text box empty] [the specified ele].
    /// </summary>
    /// <param name="ele" type="Object" domElement="true">
    /// The ele.
    /// </param>
    /// <returns type="Boolean"></returns>
    if ((ele).value.trim().length === 0) {
        return true;
    }
    else {
        return false;
    }
}
com.ivp.rad.rscriptutils.RSValidators.createErrorMessage = function com_ivp_rad_rscriptutils_RSValidators$createErrorMessage(errorMessageHolder, message) {
    /// <summary>
    /// Creates the error message.
    /// </summary>
    /// <param name="errorMessageHolder" type="Object" domElement="true">
    /// The errorMessageHolder.
    /// </param>
    /// <param name="message" type="String">
    /// The message.
    /// </param>
    var domElement = new com.ivp.rad.controls.scripts.RADBrowserScripts.RADDOMElement();
    if (message != null && errorMessageHolder != null) {
        errorMessageHolder.style.color = '#ff0000';
        if (errorMessageHolder.nodeName.toLowerCase() === 'ul') {
            var li = document.createElement('li');
            domElement.setInnerContent(li, message);
            errorMessageHolder.appendChild(li);
        }
        else {
            domElement.setInnerContent(errorMessageHolder, message);
        }
        errorMessageHolder.parentNode.style.display = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    }
}
com.ivp.rad.rscriptutils.RSValidators.requiredFieldValidator = function com_ivp_rad_rscriptutils_RSValidators$requiredFieldValidator(ele, errorMessageHolder, message) {
    /// <summary>
    /// Populates the error summary.
    /// </summary>
    /// <param name="ele" type="Object" domElement="true">
    /// The ele.
    /// </param>
    /// <param name="errorMessageHolder" type="Object" domElement="true">
    /// The errorMessageHolder.
    /// </param>
    /// <param name="message" type="String">
    /// The message.
    /// </param>
    /// <returns type="Boolean"></returns>
    if (com.ivp.rad.rscriptutils.RSValidators._isTextBoxEmpty(ele)) {
        if (errorMessageHolder != null) {
            com.ivp.rad.rscriptutils.RSValidators.createErrorMessage(errorMessageHolder, message);
        }
        return false;
    }
    else {
        return true;
    }
}
com.ivp.rad.rscriptutils.RSValidators.createErrorMessageHolder = function com_ivp_rad_rscriptutils_RSValidators$createErrorMessageHolder(obj) {
    /// <summary>
    /// Creates the error message holder.
    /// </summary>
    /// <param name="obj" type="Object" domElement="true">
    /// The obj.
    /// </param>
    /// <returns type="Object" domElement="true"></returns>
    var child = com.ivp.rad.rscriptutils.RSValidators._getErrorMessageHolder(obj);
    if (child != null) {
        obj.removeChild(child);
    }
    var errorMessageHolder = document.createElement('ul');
    obj.appendChild(errorMessageHolder);
    obj.style.display = com.ivp.rad.rscriptutils.RSConstants.strinG_NONE;
    errorMessageHolder.setAttribute('id', com.ivp.rad.rscriptutils.RSValidators._getErrorMessageHolderId(obj));
    return errorMessageHolder;
}
com.ivp.rad.rscriptutils.RSValidators._getErrorMessageHolder = function com_ivp_rad_rscriptutils_RSValidators$_getErrorMessageHolder(obj) {
    /// <summary>
    /// Gets the error message holder id.
    /// </summary>
    /// <param name="obj" type="Object" domElement="true">
    /// The obj.
    /// </param>
    /// <returns type="Object" domElement="true"></returns>
    return com.ivp.rad.rscriptutils.RSValidators.getObject(com.ivp.rad.rscriptutils.RSValidators._getErrorMessageHolderId(obj));
}
com.ivp.rad.rscriptutils.RSValidators._getErrorMessageHolderId = function com_ivp_rad_rscriptutils_RSValidators$_getErrorMessageHolderId(obj) {
    /// <summary>
    /// Gets the error message holder id.
    /// </summary>
    /// <param name="obj" type="Object" domElement="true">
    /// The obj.
    /// </param>
    /// <returns type="String"></returns>
    return obj.id + 'ListId';
}
com.ivp.rad.rscriptutils.RSValidators.checkStringWithRegExOrString = function com_ivp_rad_rscriptutils_RSValidators$checkStringWithRegExOrString(ele, expression, ifRegEx, errorMessageHolder, errorMessage) {
    /// <summary>
    /// Checks the string with reg ex or string.
    /// </summary>
    /// <param name="ele" type="Object" domElement="true">
    /// The ele.
    /// </param>
    /// <param name="expression" type="Array" elementType="String">
    /// The expression.
    /// </param>
    /// <param name="ifRegEx" type="Boolean">
    /// if set to <c>true</c> [if reg ex].
    /// </param>
    /// <param name="errorMessageHolder" type="Object" domElement="true">
    /// The errorMessageHolder.
    /// </param>
    /// <param name="errorMessage" type="String">
    /// The error message.
    /// </param>
    /// <returns type="Boolean"></returns>
    var value = (ele).value;
    var flag = false;
    if (ifRegEx && value.trim().length !== 0) {
        var regEx = new RegExp(expression[0]);
        flag = regEx.test(value);
    }
    else {
        expression[expression.length] = '';
        for (var i = 0; i < expression.length; i++) {
            if (value.trim() === expression[i].trim()) {
                flag = true;
                break;
            }
        }
    }
    if (!flag) {
        if (errorMessageHolder != null) {
            com.ivp.rad.rscriptutils.RSValidators.createErrorMessage(errorMessageHolder, errorMessage);
        }
    }
    return flag;
}
com.ivp.rad.rscriptutils.RSValidators.validateWithCharacters = function com_ivp_rad_rscriptutils_RSValidators$validateWithCharacters(ele, characters, isValidCharacters, errorMessageHolder, errorMessage, filterType) {
    /// <summary>
    /// Validates the with characters.
    /// </summary>
    /// <param name="ele" type="Object" domElement="true">
    /// The ele.
    /// </param>
    /// <param name="characters" type="String">
    /// The characters.
    /// </param>
    /// <param name="isValidCharacters" type="Boolean">
    /// if set to <c>true</c> [is valid characters].
    /// </param>
    /// <param name="errorMessageHolder" type="Object" domElement="true">
    /// The error message holder.
    /// </param>
    /// <param name="errorMessage" type="String">
    /// The error message.
    /// </param>
    /// <param name="filterType" type="com.ivp.rad.rscriptutils.FilterType">
    /// Type of the filter.
    /// </param>
    /// <returns type="Boolean"></returns>
    var value = (ele).value;
    var regEx = new Array(1);
    var flag = false;
    var isCustomCheck = false;
    switch (filterType) {
        case com.ivp.rad.rscriptutils.FilterType.custom:
            regEx[0] = '^([\\W_]*)$';
            break;
        case com.ivp.rad.rscriptutils.FilterType.lower:
            regEx[0] = '^([a-z\\s_]*)$';
            break;
        case com.ivp.rad.rscriptutils.FilterType.upper:
            regEx[0] = '^([A-Z\\s_]*)$';
            break;
        case com.ivp.rad.rscriptutils.FilterType.numbers:
            regEx[0] = '^([0-9\\s_]*)$';
            break;
        case com.ivp.rad.rscriptutils.FilterType.lowerCustom:
            regEx[0] = '^([a-z\\s\\W_]*)$';
            isCustomCheck = true;
            break;
        case com.ivp.rad.rscriptutils.FilterType.lowerNumbers:
            regEx[0] = '^([a-z0-9\\s_]*)$';
            break;
        case com.ivp.rad.rscriptutils.FilterType.lowerNumbersCustom:
            regEx[0] = '^([a-z0-9\\s\\W_]*)$';
            isCustomCheck = true;
            break;
        case com.ivp.rad.rscriptutils.FilterType.numbersCustom:
            regEx[0] = '^([0-9\\s\\W_]*)$';
            isCustomCheck = true;
            break;
        case com.ivp.rad.rscriptutils.FilterType.upperCustom:
            regEx[0] = '^([A-Z\\s\\W_]*)$';
            isCustomCheck = true;
            break;
        case com.ivp.rad.rscriptutils.FilterType.upperLower:
            regEx[0] = '^([a-zA-Z\\s_]*)$';
            break;
        case com.ivp.rad.rscriptutils.FilterType.upperLowerCustom:
            regEx[0] = '^([a-zA-Z\\s\\W_]*)$';
            isCustomCheck = true;
            break;
        case com.ivp.rad.rscriptutils.FilterType.upperLowerCustomNumbers:
            regEx[0] = '^([a-zA-Z0-9\\s\\W_]*)$';
            isCustomCheck = true;
            break;
        case com.ivp.rad.rscriptutils.FilterType.upperLowerNumbers:
            regEx[0] = '^([a-zA-Z0-9\\s_]*)$';
            break;
        case com.ivp.rad.rscriptutils.FilterType.upperNumbers:
            regEx[0] = '^([A-Z0-9\\s_]*)$';
            break;
        case com.ivp.rad.rscriptutils.FilterType.upperNumbersCustom:
            regEx[0] = '^([A-Z0-9\\s\\W_]*)$';
            isCustomCheck = true;
            break;
    }
    if (filterType !== com.ivp.rad.rscriptutils.FilterType.none) {
        flag = com.ivp.rad.rscriptutils.RSValidators.checkStringWithRegExOrString(ele, regEx, true, errorMessageHolder, errorMessage);
        if (!flag) {
            return false;
        }
    }
    if (characters !== com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY && isCustomCheck) {
        var regExCustom = new RegExp('[\\W_]', 'g');
        var specialChars = value.match(regExCustom);
        var chars = com.ivp.rad.rscriptutils.RSValidators._getChartersFromString(characters);
        if (specialChars != null) {
            for (var i = 0; i < specialChars.length; i++) {
                if (Array.contains(chars, specialChars[i])) {
                    if (isValidCharacters) {
                        flag = true;
                    }
                    else {
                        flag = false;
                        break;
                    }
                }
                else {
                    if (isValidCharacters) {
                        flag = false;
                        break;
                    }
                    else {
                        flag = true;
                    }
                }
            }
        }
    }
    if (!flag) {
        if (errorMessageHolder != null) {
            com.ivp.rad.rscriptutils.RSValidators.createErrorMessage(errorMessageHolder, errorMessage);
        }
    }
    return flag;
}
com.ivp.rad.rscriptutils.RSValidators._getChartersFromString = function com_ivp_rad_rscriptutils_RSValidators$_getChartersFromString(characters) {
    /// <summary>
    /// Gets the charters from string.
    /// </summary>
    /// <param name="characters" type="String">
    /// The characters.
    /// </param>
    /// <returns type="Array" elementType="String"></returns>
    var chars = new Array(characters.length);
    for (var i = 0; i < characters.length; i++) {
        chars[i] = characters.charAt(i).toString();
    }
    return chars;
}
com.ivp.rad.rscriptutils.RSValidators.requiredDropDownValidator = function com_ivp_rad_rscriptutils_RSValidators$requiredDropDownValidator(ele, errorMessageHolder, message, control) {
    /// <summary>
    /// Requireds the drop down validator.
    /// </summary>
    /// <param name="ele" type="Object" domElement="true">
    /// The ele.
    /// </param>
    /// <param name="errorMessageHolder" type="Object" domElement="true">
    /// The error message holder.
    /// </param>
    /// <param name="message" type="String">
    /// The message.
    /// </param>
    /// <param name="control" type="com.ivp.rad.rscriptutils.BindingControl">
    /// The control.
    /// </param>
    /// <returns type="Boolean"></returns>
    if (!com.ivp.rad.rscriptutils.RSValidators._isDropDownSelected(ele, control)) {
        if (errorMessageHolder != null) {
            com.ivp.rad.rscriptutils.RSValidators.createErrorMessage(errorMessageHolder, message);
        }
        return false;
    }
    else {
        return true;
    }
}
com.ivp.rad.rscriptutils.RSValidators._isDropDownSelected = function com_ivp_rad_rscriptutils_RSValidators$_isDropDownSelected(ele, control) {
    /// <summary>
    /// Determines whether [is drop down selected] [the specified ele].
    /// </summary>
    /// <param name="ele" type="Object" domElement="true">
    /// The ele.
    /// </param>
    /// <param name="control" type="com.ivp.rad.rscriptutils.BindingControl">
    /// The control.
    /// </param>
    /// <returns type="Boolean"></returns>
    var flag = false;
    if (control === com.ivp.rad.rscriptutils.BindingControl.dropDown) {
        if (ele.selectedIndex === 0) {
            flag = false;
        }
        else {
            flag = true;
        }
    }
    else if (control === com.ivp.rad.rscriptutils.BindingControl.listBox) {
        if (ele.selectedIndex < 0) {
            flag = false;
        }
        else {
            flag = true;
        }
    }
    return flag;
}
com.ivp.rad.rscriptutils.RSValidators.getDateTime = function com_ivp_rad_rscriptutils_RSValidators$getDateTime(dt, option) {
    /// <summary>
    /// Gets the date time.
    /// </summary>
    /// <param name="dt" type="String">
    /// The dt.
    /// </param>
    /// <param name="option" type="com.ivp.rad.rscriptutils.DateTimeOptions">
    /// The option.
    /// </param>
    /// <returns type="String"></returns>
    var ticks = com.ivp.rad.rscriptutils.RSValidators.returnTicks(dt);
    var dtTi = null;
    switch (option) {
        case com.ivp.rad.rscriptutils.DateTimeOptions.date:
            dtTi = new Date(ticks);
            return dtTi.format('MM/dd/yyyy');
        case com.ivp.rad.rscriptutils.DateTimeOptions.time:
            dtTi = new Date(ticks);
            return dtTi.format('hh:mm:ss tt');
        case com.ivp.rad.rscriptutils.DateTimeOptions.scheduled:
            dtTi = new Date(ticks);
            return dtTi.format('MM/dd/yyyy hh:mm:ss tt');
        default:
            return null;
    }
}
com.ivp.rad.rscriptutils.RSValidators.returnTicks = function com_ivp_rad_rscriptutils_RSValidators$returnTicks(dt) {
    /// <summary>
    /// Returns the ticks.
    /// </summary>
    /// <param name="dt" type="String">
    /// The dt.
    /// </param>
    /// <returns type="Number" integer="true"></returns>
    var regEx = new RegExp('[^0-9]');
    var i = 0;
    while (i < dt.length) {
        dt = dt.replace(regEx, '');
        i++;
    }
    return Number.parseInvariant(dt);
}
com.ivp.rad.rscriptutils.RSValidators.checkDates = function com_ivp_rad_rscriptutils_RSValidators$checkDates(errorMessageHolder, startDate, startTime, endDate, ifCurrentDateCheck) {
    /// <summary>
    /// Checks the dates.
    /// </summary>
    /// <param name="errorMessageHolder" type="Object" domElement="true">
    /// The errorMessageHolder.
    /// </param>
    /// <param name="startDate" type="String">
    /// The start date.
    /// </param>
    /// <param name="startTime" type="String">
    /// The start time.
    /// </param>
    /// <param name="endDate" type="String">
    /// The end date.
    /// </param>
    /// <param name="ifCurrentDateCheck" type="Boolean">
    /// [if current date check].
    /// </param>
    /// <returns type="Boolean"></returns>
    var flag = true;
    var dtStartDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertStringToDateTime(startDate, com.ivp.rad.rscriptutils.DateTimeFormat.shorDate);
    var curDate = new Date();
    var startTicks = com.ivp.rad.rscriptutils.RSValidators.returnTicks(Sys.Serialization.JavaScriptSerializer.serialize(dtStartDate));
    var currentTicks = com.ivp.rad.rscriptutils.RSValidators.returnTicks(Sys.Serialization.JavaScriptSerializer.serialize(curDate));
    var endTicks = 0;
    var hours = 0;
    var minutes = 0;
    var seconds = 0;
    var ti = null;
    var time = null;
    if (startTime != null) {
        ti = startTime.split(' ');
        time = ti[0].split(':');
        hours = Number.parseInvariant(time[0]);
        minutes = Number.parseInvariant(time[1]);
        seconds = Number.parseInvariant(time[2]);
    }
    if (((minutes >= 60 || seconds >= 60 || hours > 12) && startTime.endsWith('M')) || (minutes >= 60 || seconds >= 60 || hours > 24)) {
        com.ivp.rad.rscriptutils.RSValidators.createErrorMessage(errorMessageHolder, 'Incorect time format.');
        flag = false;
    }
    else {
        if (ifCurrentDateCheck) {
            if (startTicks < currentTicks) {
                if (dtStartDate.getDate() !== curDate.getDate()) {
                    com.ivp.rad.rscriptutils.RSValidators.createErrorMessage(errorMessageHolder, 'Start Date should be greater than current date.');
                    flag = false;
                }
            }
        }
        if (startTime != null) {
            if (ti.length > 1) {
                switch (ti[1]) {
                    case 'AM':
                        if (hours === 12) {
                            hours = 0;
                        }
                        break;
                    case 'PM':
                        if (hours !== 12) {
                            hours = hours + 12;
                        }
                        break;
                }
            }
            dtStartDate.setHours(hours);
            dtStartDate.setMinutes(minutes);
            dtStartDate.setSeconds(seconds);
            startTicks = com.ivp.rad.rscriptutils.RSValidators.returnTicks(Sys.Serialization.JavaScriptSerializer.serialize(dtStartDate));
            if (ifCurrentDateCheck) {
                if (startTicks < currentTicks) {
                    if (dtStartDate.getDate() === curDate.getDate()) {
                        com.ivp.rad.rscriptutils.RSValidators.createErrorMessage(errorMessageHolder, 'Start time should be greater than current time.');
                        flag = false;
                    }
                }
            }
        }
        var dtEndDate = null;
        if (endDate != null) {
            dtEndDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertStringToDateTime(endDate, com.ivp.rad.rscriptutils.DateTimeFormat.shorDate);
            dtEndDate.setHours(23);
            dtEndDate.setMinutes(59);
            dtEndDate.setSeconds(59);
            endTicks = com.ivp.rad.rscriptutils.RSValidators.returnTicks(Sys.Serialization.JavaScriptSerializer.serialize(dtEndDate));
            if (startTicks > endTicks) {
                com.ivp.rad.rscriptutils.RSValidators.createErrorMessage(errorMessageHolder, 'End Date should be greater than start date.');
                flag = false;
            }
        }
    }
    return flag;
}
com.ivp.rad.rscriptutils.RSValidators.dateComparisonValidator = function com_ivp_rad_rscriptutils_RSValidators$dateComparisonValidator(value, args) {
    /// <summary>
    /// Comparison validator for Dates when called from asp:CustomValidator.
    /// </summary>
    /// <param name="value" type="Object" domElement="true">
    /// The DOMElement of CustomValidator(need not to be passed).
    /// </param>
    /// <param name="args" type="com.ivp.rad.rscriptutils.arguement">
    /// The args(Need not to be passed).
    /// </param>
    /// <returns type="Boolean"></returns>
    var startDateID = value.getAttribute('startDateID').toString();
    var endDateID = value.getAttribute('endDateID').toString();
    if (com.ivp.rad.rscriptutils.RSValidators.checkDates(null, (com.ivp.rad.rscriptutils.RSValidators.getObject(startDateID)).value, null, (com.ivp.rad.rscriptutils.RSValidators.getObject(endDateID)).value, false)) {
        args.IsValid = true;
        return true;
    }
    else {
        args.IsValid = false;
        return false;
    }
}
com.ivp.rad.rscriptutils.RSValidators.clearErrorMessages = function com_ivp_rad_rscriptutils_RSValidators$clearErrorMessages(errorContainer) {
    /// <summary>
    /// Clears the error messages.
    /// </summary>
    /// <param name="errorContainer" type="Object" domElement="true">
    /// The error container.
    /// </param>
    var ele = com.ivp.rad.rscriptutils.RSValidators._getErrorMessageHolder(errorContainer);
    if (ele != null) {
        ele.parentNode.removeChild(ele);
    }
    errorContainer.style.display = com.ivp.rad.rscriptutils.RSConstants.strinG_NONE;
}
com.ivp.rad.rscriptutils.RSValidators.validateForRange = function com_ivp_rad_rscriptutils_RSValidators$validateForRange(element, range, isNumericRange, errorMessageHolder, errorMessage) {
    /// <summary>
    /// Validates for range.
    /// </summary>
    /// <param name="element" type="Object" domElement="true">
    /// The target element.
    /// </param>
    /// <param name="range" type="String">
    /// The range.
    /// </param>
    /// <param name="isNumericRange" type="Boolean">
    /// if set to <c>true</c> [is numeric range].
    /// </param>
    /// <param name="errorMessageHolder" type="Object" domElement="true">
    /// The error message holder.
    /// </param>
    /// <param name="errorMessage" type="String">
    /// The error message.
    /// </param>
    /// <returns type="Boolean"></returns>
    var limits = range.split('-');
    var value = (element).value;
    if (isNumericRange) {
        var lowerLimit = Number.parseInvariant(limits[0]);
        var upperLimit = Number.parseInvariant(limits[1]);
        var intValue = Number.parseInvariant(value);
        if ((intValue >= lowerLimit) && (intValue <= upperLimit)) {
            return true;
        }
        else {
            return false;
        }
    }
    else {
        var regEx = new RegExp('^[' + range + ']$');
        if (regEx.test(value)) {
            return true;
        }
        else {
            return false;
        }
    }
}
com.ivp.rad.rscriptutils.RSValidators.compareValidators = function com_ivp_rad_rscriptutils_RSValidators$compareValidators(element, elementToCompare, isNumeric, ifCheckForEquality, errorMessageHolder, errorMessage) {
    /// <summary>
    /// Compares the validators.
    /// </summary>
    /// <param name="element" type="Object" domElement="true">
    /// The element.
    /// </param>
    /// <param name="elementToCompare" type="Object" domElement="true">
    /// The element to compare.
    /// </param>
    /// <param name="isNumeric" type="Boolean">
    /// if set to <c>true</c> [is numeric].
    /// </param>
    /// <param name="ifCheckForEquality" type="Boolean">
    /// if set to <c>true</c> [if check for equality].
    /// </param>
    /// <param name="errorMessageHolder" type="Object" domElement="true">
    /// The error message holder.
    /// </param>
    /// <param name="errorMessage" type="String">
    /// The error message.
    /// </param>
    /// <returns type="Boolean"></returns>
    var flag = true;
    if (isNumeric) {
        var regEx = new Array(1);
        regEx[0] = '[0-9]';
        if (com.ivp.rad.rscriptutils.RSValidators.checkStringWithRegExOrString(element, regEx, true, null, null) && com.ivp.rad.rscriptutils.RSValidators.checkStringWithRegExOrString(elementToCompare, regEx, true, null, null)) {
            var value = Number.parseInvariant((element).value);
            var valueToCompare = Number.parseInvariant((elementToCompare).value);
            var compareExpression = (ifCheckForEquality) ? value <= valueToCompare : value < valueToCompare;
            if (compareExpression) {
                flag = true;
            }
            else {
                com.ivp.rad.rscriptutils.RSValidators.createErrorMessage(errorMessageHolder, errorMessage);
                flag = false;
            }
        }
        else {
            flag = false;
        }
    }
    return flag;
}


com.ivp.rad.rradapi.RADWebServiceException.registerClass('com.ivp.rad.rradapi.RADWebServiceException');
com.ivp.rad.rscriptutils.RSCommonScripts.registerClass('com.ivp.rad.rscriptutils.RSCommonScripts');
com.ivp.rad.rscriptutils.DropDownInfo.registerClass('com.ivp.rad.rscriptutils.DropDownInfo');
com.ivp.rad.rscriptutils.RSConstants.registerClass('com.ivp.rad.rscriptutils.RSConstants');
com.ivp.rad.rscriptutils.RSCssClassConstants.registerClass('com.ivp.rad.rscriptutils.RSCssClassConstants');
com.ivp.rad.rscriptutils.RSCultureManager.registerClass('com.ivp.rad.rscriptutils.RSCultureManager');
com.ivp.rad.rscriptutils.RSCultureInfo.registerClass('com.ivp.rad.rscriptutils.RSCultureInfo');
com.ivp.rad.rscriptutils.RMSCustomArray.registerClass('com.ivp.rad.rscriptutils.RMSCustomArray');
com.ivp.rad.rscriptutils.RSCustomLogger.registerClass('com.ivp.rad.rscriptutils.RSCustomLogger');
com.ivp.rad.rscriptutils.RSDateTimeUtils.registerClass('com.ivp.rad.rscriptutils.RSDateTimeUtils');
com.ivp.rad.rscriptutils.RSEvalManager.registerClass('com.ivp.rad.rscriptutils.RSEvalManager');
com.ivp.rad.rscriptutils.RSGUIElementHelper.registerClass('com.ivp.rad.rscriptutils.RSGUIElementHelper');
LINQ.registerClass('LINQ');
com.ivp.rad.rscriptutils.RSProfileManager.registerClass('com.ivp.rad.rscriptutils.RSProfileManager');
com.ivp.rad.rscriptutils.RSValidators.registerClass('com.ivp.rad.rscriptutils.RSValidators');
com.ivp.rad.rscriptutils.RSCommonScripts._defaulT_VALUE = 'defaultVal';
com.ivp.rad.rscriptutils.RSCommonScripts._behaviorID = null;
com.ivp.rad.rscriptutils.RSCommonScripts._intervalID = 0;
com.ivp.rad.rscriptutils.RSConstants.cmD_ARG_ADD = 'Add';
com.ivp.rad.rscriptutils.RSConstants.cmD_ARG_UPDATE = 'Update';
com.ivp.rad.rscriptutils.RSConstants.cmD_ARG_DELETE = 'Delete';
com.ivp.rad.rscriptutils.RSConstants.cmD_ARG_DETAILS = 'Details';
com.ivp.rad.rscriptutils.RSConstants.cmD_ARG_CANCEL = 'Cancel';
com.ivp.rad.rscriptutils.RSConstants.cmD_ARG_OK = 'Ok';
com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY = '';
com.ivp.rad.rscriptutils.RSConstants.datetimE_EMPTY = '1/1/1900';
com.ivp.rad.rscriptutils.RSConstants.characteR_EMPTY = '\u0000';
com.ivp.rad.rscriptutils.RSConstants.diV_ELEMENT = 'DIV';
com.ivp.rad.rscriptutils.RSConstants.tablE_ELEMENT = 'TABLE';
com.ivp.rad.rscriptutils.RSConstants.tablE_ROW_ELEMENT = 'TR';
com.ivp.rad.rscriptutils.RSConstants.tablE_CELL_ELEMENT = 'TD';
com.ivp.rad.rscriptutils.RSConstants.strinG_NONE = 'none';
com.ivp.rad.rscriptutils.RSConstants.strinG_NULL = 'null';
com.ivp.rad.rscriptutils.RSConstants.strinG_ZERO = '0';
com.ivp.rad.rscriptutils.RSConstants.stylE_DISPLAY_BLOCK = 'block';
com.ivp.rad.rscriptutils.RSConstants.strinG_SEPERATOR = '\u03a9';
com.ivp.rad.rscriptutils.RSConstants.evenT_CLICK = 'click';
com.ivp.rad.rscriptutils.RSConstants.evenT_CHANGE = 'change';
com.ivp.rad.rscriptutils.RSConstants.evenT_KEYUP = 'keyup';
com.ivp.rad.rscriptutils.RSConstants.evenT_DOUBLE_CLICK = 'dblclick';
com.ivp.rad.rscriptutils.RSConstants.regeX_ALPHA_NUMARIC_SPACE = '^[0-9a-zA-Z\\s]*$';
com.ivp.rad.rscriptutils.RSConstants.regeX_ALPHA_NUMERIC_SPACE_UNDERSCORE = '^[0-9a-zA-Z_\\s]*$';
com.ivp.rad.rscriptutils.RSConstants.regeX_ALPHA_NUMERIC_SPACE_UNDERSCORE_DOT = '^[0-9a-zA-Z_.\\s]*$';
com.ivp.rad.rscriptutils.RSConstants.regeX_ALPHA_NUMERIC_SPACE_UNDERSCORE_DOT_COLON_SLASH = '^[0-9a-zA-Z_:\\\\.\\s]*$';
com.ivp.rad.rscriptutils.RSConstants.regeX_NUMERIC = '^[0-9]*$';
com.ivp.rad.rscriptutils.RSConstants.regeX_DECIMAL = '^[0-9][0-9]*[,][0-9][0-9]*$';
com.ivp.rad.rscriptutils.RSConstants.regeX_CHARACTER_SET = '([\\w\\s\\()/-]*)([\\W]*)([\\.\\s\\w]*)';
com.ivp.rad.rscriptutils.RSConstants.regeX_ALPHA_NUMERIC = '([\\w]*)';
com.ivp.rad.rscriptutils.RSConstants.regeX_DECIMAL_DOT = '^[0-9]*.?[0-9]+$';
com.ivp.rad.rscriptutils.RSConstants.interneT_EXPLORER = 'Microsoft Internet Explorer';
com.ivp.rad.rscriptutils.RSConstants.mozillA_FIREFOX = 'Firefox';
com.ivp.rad.rscriptutils.RSCssClassConstants.INPUT = 'input';
com.ivp.rad.rscriptutils.RSCssClassConstants.updatE_CLIENT_BUTTON = 'updateClientBtn';
com.ivp.rad.rscriptutils.RSCssClassConstants.deletE_CLIENT_DISABLED_BUTTON = 'deleteClientDisabledBtn';
com.ivp.rad.rscriptutils.RSCssClassConstants.detailS_CLIENT_BUTTON = 'detailsClientButton';
com.ivp.rad.rscriptutils.RSCssClassConstants.normaL_ROW = 'normalRow';
com.ivp.rad.rscriptutils.RSCssClassConstants.alternatinG_ROW = 'alternatingRow';
com.ivp.rad.rscriptutils.RSCssClassConstants.deletE_CLIENT_BUTTON = 'deleteClientBtn';
com.ivp.rad.rscriptutils.RSCssClassConstants.HEADER = 'header';
com.ivp.rad.rscriptutils.RSCssClassConstants.datA_ROW = 'dataRow';
com.ivp.rad.rscriptutils.RSCssClassConstants.griD_HEADER = 'gridHeader';
com.ivp.rad.rscriptutils.RSCssClassConstants.collapsE_GRID_BUTTON = 'collapseGridButton';
com.ivp.rad.rscriptutils.RSCssClassConstants.collapsE_GROUP_BUTTON = 'collapseGroupButton';
com.ivp.rad.rscriptutils.RSCssClassConstants.expanD_GROUP_BUTTON = 'expandGroupButton';
com.ivp.rad.rscriptutils.RSCssClassConstants.expanD_GRID_BUTTON = 'expandGridButton';
com.ivp.rad.rscriptutils.RSCssClassConstants.statE_ON_BUTTON = 'onBtn';
com.ivp.rad.rscriptutils.RSCssClassConstants.statE_OFF_BUTTON = 'offBtn';
com.ivp.rad.rscriptutils.RSCultureManager._cultureInfo = null;
com.ivp.rad.rscriptutils.RSCultureManager._serverCultureInfo = null;
com.ivp.rad.rscriptutils.RSProfileManager._themeName = null;
com.ivp.rad.rscriptutils.RSProfileManager._themeButton = null;
com.ivp.rad.rscriptutils.RSProfileManager._oldThemeName = null;

// ---- Do not remove this footer ----
// This script was generated using Script# v0.5.5.0 (http://projects.nikhilk.net/ScriptSharp)
// -----------------------------------
