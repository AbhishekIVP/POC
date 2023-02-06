// SecMasterJSCommon.js
//


Type.registerNamespace('SecMasterJSCommon');

////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.SearchPageIdentifier

SecMasterJSCommon.SearchPageIdentifier = function() { 
    /// <field name="CreateSecurity" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="UpdateSecurity" type="Number" integer="true" static="true">
    /// </field>
};
SecMasterJSCommon.SearchPageIdentifier.prototype = {
    CreateSecurity: 0, 
    UpdateSecurity: 1
}
SecMasterJSCommon.SearchPageIdentifier.registerEnum('SecMasterJSCommon.SearchPageIdentifier', false);


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.ListOperation

SecMasterJSCommon.ListOperation = function() { 
    /// <field name="ADD" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="removE_ITEM" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="removE_ALL" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="checK_CONTAINS" type="Number" integer="true" static="true">
    /// </field>
};
SecMasterJSCommon.ListOperation.prototype = {
    ADD: 0, 
    removE_ITEM: 1, 
    removE_ALL: 2, 
    checK_CONTAINS: 3
}
SecMasterJSCommon.ListOperation.registerEnum('SecMasterJSCommon.ListOperation', false);


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.RepeatDirection

SecMasterJSCommon.RepeatDirection = function() { 
    /// <field name="horizontal" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="vertical" type="Number" integer="true" static="true">
    /// </field>
};
SecMasterJSCommon.RepeatDirection.prototype = {
    horizontal: 0, 
    vertical: 1
}
SecMasterJSCommon.RepeatDirection.registerEnum('SecMasterJSCommon.RepeatDirection', false);


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.SMChangeSecurityTypeStatus

SecMasterJSCommon.SMChangeSecurityTypeStatus = function() { 
    /// <field name="SUCCESS" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="FAILURE" type="Number" integer="true" static="true">
    /// </field>
};
SecMasterJSCommon.SMChangeSecurityTypeStatus.prototype = {
    SUCCESS: 1, 
    FAILURE: 0
}
SecMasterJSCommon.SMChangeSecurityTypeStatus.registerEnum('SecMasterJSCommon.SMChangeSecurityTypeStatus', false);


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.SMCreateUpdateOpenByDefault

SecMasterJSCommon.SMCreateUpdateOpenByDefault = function() { 
    /// <field name="None" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="FTP" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="Audit" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="Hierarchy" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="CorpAction" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="VendorData" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="DownstreamStatus" type="Number" integer="true" static="true">
    /// </field>
};
SecMasterJSCommon.SMCreateUpdateOpenByDefault.prototype = {
    None: 0, 
    FTP: 1, 
    Audit: 2, 
    Hierarchy: 3, 
    CorpAction: 4, 
    VendorData: 5, 
    DownstreamStatus: 6
}
SecMasterJSCommon.SMCreateUpdateOpenByDefault.registerEnum('SecMasterJSCommon.SMCreateUpdateOpenByDefault', false);


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.SMSCommons

SecMasterJSCommon.SMSCommons = function SecMasterJSCommon_SMSCommons() {
}
SecMasterJSCommon.SMSCommons.contains = function SecMasterJSCommon_SMSCommons$contains(main, toFind) {
    /// <summary>
    /// Determines whether "main" string contains "toFind" string.
    /// </summary>
    /// <param name="main" type="String">
    /// String to be searched
    /// </param>
    /// <param name="toFind" type="String">
    /// String(pattern) containing the sequence to match.
    /// </param>
    /// <returns type="Boolean"></returns>
    var regEx = new RegExp(toFind, 'gi');
    return regEx.test(main);
}
SecMasterJSCommon.SMSCommons.containsWithinArray = function SecMasterJSCommon_SMSCommons$containsWithinArray(main, toFind, isCaseSensitive) {
    /// <param name="main" type="Array" elementType="String">
    /// </param>
    /// <param name="toFind" type="String">
    /// </param>
    /// <param name="isCaseSensitive" type="Boolean">
    /// </param>
    /// <returns type="Boolean"></returns>
    var flag = false;
    main[main.length] = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
    for (var i = 0; i < main.length; i++) {
        if ((!isCaseSensitive && toFind.trim().toLowerCase() === main[i].trim().toLowerCase()) || (isCaseSensitive && toFind.trim() === main[i].trim())) {
            flag = true;
            break;
        }
    }
    return flag;
}
SecMasterJSCommon.SMSCommons.findIndexInDdl = function SecMasterJSCommon_SMSCommons$findIndexInDdl(valueToSearch, ddlToSearch) {
    /// <summary>
    /// Finds the index of the option element in DropDownList by Value.
    /// </summary>
    /// <param name="valueToSearch" type="String">
    /// String Value to search.
    /// </param>
    /// <param name="ddlToSearch" type="Object" domElement="true">
    /// DropDownList to search.
    /// </param>
    /// <returns type="Number" integer="true"></returns>
    var length = ddlToSearch.options.length;
    for (var i = 0; i < length; i++) {
        if ((ddlToSearch.options[i]).value === valueToSearch.trim()) {
            return i;
        }
    }
    return 0;
}
SecMasterJSCommon.SMSCommons.findIndexInDdlByText = function SecMasterJSCommon_SMSCommons$findIndexInDdlByText(textToSearch, ddlToSearch) {
    /// <summary>
    /// Finds the index of the option element in the DropDownList by Text.
    /// </summary>
    /// <param name="textToSearch" type="String">
    /// The String Text to search.
    /// </param>
    /// <param name="ddlToSearch" type="Object" domElement="true">
    /// The DropDownList to search.
    /// </param>
    /// <returns type="Number" integer="true"></returns>
    var length = ddlToSearch.options.length;
    for (var i = 0; i < length; i++) {
        if ((ddlToSearch.options[i]).text.trim() === textToSearch.trim()) {
            return i;
        }
    }
    return 0;
}
SecMasterJSCommon.SMSCommons.formatXGrid = function SecMasterJSCommon_SMSCommons$formatXGrid(gridToFormat) {
    /// <summary>
    /// Formats the RADX grid with aternation row css.
    /// </summary>
    /// <param name="gridToFormat" type="Object" domElement="true">
    /// The grid to format.
    /// </param>
    var normalRow = SecMasterJSCommon.SMSConstants.clasS_NORMAL_ROW;
    var alternatingRow = SecMasterJSCommon.SMSConstants.clasS_ALTERNATING_ROW;
    for (var i = 1; i < gridToFormat.rows.length; i++) {
        if ((i % 2) === 0) {
            gridToFormat.rows[i].className = alternatingRow;
        }
        else {
            gridToFormat.rows[i].className = normalRow;
        }
    }
}
SecMasterJSCommon.SMSCommons.showError = function SecMasterJSCommon_SMSCommons$showError(divElement, strErrorMsg) {
    /// <summary>
    /// Shows formated error in the divElement.
    /// </summary>
    /// <param name="divElement" type="Object" domElement="true">
    /// The div element to show error.
    /// </param>
    /// <param name="strErrorMsg" type="String">
    /// The error message.
    /// </param>
    var holder = com.ivp.rad.rscriptutils.RSValidators.createErrorMessageHolder(divElement);
    com.ivp.rad.rscriptutils.RSValidators.createErrorMessage(holder, strErrorMsg);
}
SecMasterJSCommon.SMSCommons.clearError = function SecMasterJSCommon_SMSCommons$clearError(divElement) {
    /// <summary>
    /// Clears the error
    /// </summary>
    /// <param name="divElement" type="Object" domElement="true">
    /// The div element to show error.
    /// </param>
    com.ivp.rad.rscriptutils.RSValidators.clearErrorMessages(divElement);
}
SecMasterJSCommon.SMSCommons.bindDropDown = function SecMasterJSCommon_SMSCommons$bindDropDown(list, ddlToBind, requireSelectOne) {
    /// <summary>
    /// Binds the drop down with the list.
    /// </summary>
    /// <param name="list" type="Array">
    /// The list to bind in DropDownList.
    /// </param>
    /// <param name="ddlToBind" type="Object" domElement="true">
    /// The DropDownList to bind.
    /// </param>
    /// <param name="requireSelectOne" type="Boolean">
    /// if set to <c>true</c> select one option element is added.
    /// </param>
    var helper = new com.ivp.rad.rscriptutils.RSGUIElementHelper();
    var option = null;
    if (requireSelectOne) {
        option = helper.get_option();
        option.text = SecMasterJSCommon.SMSConstants.selecT_ONE_TEXT;
        option.value = SecMasterJSCommon.SMSConstants.selecT_ONE_VALUE;
        ddlToBind.add(option);
    }
    if (list.length > 0 && list[0].toString() === SecMasterJSCommon.SMSConstants.strinG_EMPTY) {
        option = helper.get_option();
        option.text = 'None';
        option.value = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
        ddlToBind.add(option);
    }
    else {
        for (var i = 0; i < list.length; i++) {
            option = helper.get_option();
            option.text = list[i].toString();
            option.value = list[i].toString();
            ddlToBind.add(option);
        }
    }
}
SecMasterJSCommon.SMSCommons.cloneDropDown = function SecMasterJSCommon_SMSCommons$cloneDropDown(ddlSource, ddlToBind) {
    /// <param name="ddlSource" type="Object" domElement="true">
    /// </param>
    /// <param name="ddlToBind" type="Object" domElement="true">
    /// </param>
    var helper = new com.ivp.rad.rscriptutils.RSGUIElementHelper();
    var option = null;
    SecMasterJSCommon.SMSCommons.clearSelectElement(ddlToBind);
    for (var i = 0; i < ddlSource.options.length; i++) {
        option = helper.get_option();
        option.text = (ddlSource.options[i]).text;
        option.value = (ddlSource.options[i]).value;
        ddlToBind.add(option);
    }
}
SecMasterJSCommon.SMSCommons.bindDropDownFromDictionary = function SecMasterJSCommon_SMSCommons$bindDropDownFromDictionary(dicValue, dropDownToBind, control, requireSelectOne) {
    /// <summary>
    /// Binds the drop down from the dictionary.
    /// </summary>
    /// <param name="dicValue" type="SecMasterJSCommon.SMSDictionary">
    /// The dictionary to bind in DropDownList.
    /// </param>
    /// <param name="dropDownToBind" type="Object" domElement="true">
    /// The DropDownList to bind.
    /// </param>
    /// <param name="control" type="com.ivp.rad.rscriptutils.BindingControl">
    /// The BindingControl type.
    /// </param>
    /// <param name="requireSelectOne" type="Boolean">
    /// </param>
    var option = null;
    var helper = new com.ivp.rad.rscriptutils.RSGUIElementHelper();
    if (control === com.ivp.rad.rscriptutils.BindingControl.dropDown && requireSelectOne) {
        option = helper.get_option();
        option.text = SecMasterJSCommon.SMSConstants.selecT_ONE_TEXT;
        option.value = SecMasterJSCommon.SMSConstants.selecT_ONE_VALUE;
        dropDownToBind.add(option);
    }
    var $enum1 = dicValue.getEnumerator();
    while ($enum1.moveNext()) {
        var key = $enum1.get_current();
        option = helper.get_option();
        option.value = key;
        option.text = dicValue.get_item(key).toString();
        dropDownToBind.add(option);
    }
}
SecMasterJSCommon.SMSCommons.hideContextMenu = function SecMasterJSCommon_SMSCommons$hideContextMenu(cntMenuId) {
    /// <summary>
    /// Hides the context menu.
    /// </summary>
    /// <param name="cntMenuId" type="String">
    /// The id of the context menu to hide.
    /// </param>
    var ctMenu = $find(cntMenuId + '_Root_0');
    var attributes = ctMenu.get_attributeOverrides();
    var a = new RegExp('postback');
    if (!a.test(attributes)) {
        ctMenu.set_attributeOverrides(attributes + '|postback');
    }
    if (window.event.srcElement != null) {
        window.event.srcElement.setAttribute('postback', '123');
    }
}
SecMasterJSCommon.SMSCommons.hideContextMenuItem = function SecMasterJSCommon_SMSCommons$hideContextMenuItem(cntMenuId, cntMenuItemId) {
    /// <summary>
    /// Hides the context menu item.
    /// </summary>
    /// <param name="cntMenuId" type="String">
    /// The context menu id.
    /// </param>
    /// <param name="cntMenuItemId" type="String">
    /// The id of the context menu item to hide.
    /// </param>
    var cntMenuItem = com.ivp.rad.rscriptutils.RSValidators.getObject(cntMenuId + '_' + cntMenuItemId);
    if (cntMenuItem != null) {
        cntMenuItem.parentNode.parentNode.style.display = SecMasterJSCommon.SMSConstants.strinG_NONE;
    }
}
SecMasterJSCommon.SMSCommons.showHiddenContextMenuItem = function SecMasterJSCommon_SMSCommons$showHiddenContextMenuItem(cntMenuId, cntMenuItemId) {
    /// <summary>
    /// Shows the hidden context menu item.
    /// </summary>
    /// <param name="cntMenuId" type="String">
    /// The context menu id.
    /// </param>
    /// <param name="cntMenuItemId" type="String">
    /// The id of the hidden context menu item to show.
    /// </param>
    var cntMenuItem = com.ivp.rad.rscriptutils.RSValidators.getObject(cntMenuId + '_' + cntMenuItemId);
    if (cntMenuItem != null) {
        cntMenuItem.parentNode.parentNode.style.display = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
    }
}
SecMasterJSCommon.SMSCommons.getOnContextMenuRowId = function SecMasterJSCommon_SMSCommons$getOnContextMenuRowId(cntMenuId) {
    /// <summary>
    /// Gets the OnContextMenu row id.
    /// </summary>
    /// <param name="cntMenuId" type="String">
    /// The context menu id.
    /// </param>
    /// <returns type="String"></returns>
    return (document.getElementById(cntMenuId + '___rowId')).value;
}
SecMasterJSCommon.SMSCommons.getTableRowByRowId = function SecMasterJSCommon_SMSCommons$getTableRowByRowId(grid, rowId) {
    /// <summary>
    /// Gets the table row element by table row id.
    /// ///
    /// </summary>
    /// <param name="grid" type="Object" domElement="true">
    /// The grid object to which the table row belongs.
    /// </param>
    /// <param name="rowId" type="Object">
    /// The row id.
    /// </param>
    /// <returns type="Object" domElement="true"></returns>
    if (String.isInstanceOfType(rowId)) {
        return grid.rows[Number.parseInvariant(rowId)];
    }
    else if (Number.isInstanceOfType(rowId)) {
        return grid.rows[rowId];
    }
    else {
        return null;
    }
}
SecMasterJSCommon.SMSCommons.getTargetTableRow = function SecMasterJSCommon_SMSCommons$getTargetTableRow(targetElement) {
    /// <summary>
    /// Gets the target table row element.
    /// </summary>
    /// <param name="targetElement" type="Object" domElement="true">
    /// The event target element (e.target).
    /// </param>
    /// <returns type="Object" domElement="true"></returns>
    return com.ivp.rad.rscriptutils.RSCommonScripts.findControl(targetElement, 'TR');
}
SecMasterJSCommon.SMSCommons.deleteCurrentRowFromGrid = function SecMasterJSCommon_SMSCommons$deleteCurrentRowFromGrid(grid, targetElement) {
    /// <summary>
    /// Deletes the current row from grid.
    /// </summary>
    /// <param name="grid" type="Object" domElement="true">
    /// The grid object to which the table row belongs.
    /// </param>
    /// <param name="targetElement" type="Object" domElement="true">
    /// The event target element (e.target).
    /// </param>
    grid.deleteRow((com.ivp.rad.rscriptutils.RSCommonScripts.findControl(targetElement, 'TR')).rowIndex);
}
SecMasterJSCommon.SMSCommons.getParentElement = function SecMasterJSCommon_SMSCommons$getParentElement(ele, controlToFind) {
    /// <param name="ele" type="Object" domElement="true">
    /// </param>
    /// <param name="controlToFind" type="String">
    /// </param>
    /// <returns type="Object" domElement="true"></returns>
    return com.ivp.rad.rscriptutils.RSCommonScripts.findControl(ele, controlToFind);
}
SecMasterJSCommon.SMSCommons.getCellIndex = function SecMasterJSCommon_SMSCommons$getCellIndex(ele) {
    /// <param name="ele" type="Object" domElement="true">
    /// </param>
    /// <returns type="Number" integer="true"></returns>
    return (ele).cellIndex;
}
SecMasterJSCommon.SMSCommons.doControlPostback = function SecMasterJSCommon_SMSCommons$doControlPostback(targetElementID) {
    /// <summary>
    /// Does the control postback.
    /// </summary>
    /// <param name="targetElementID" type="String">
    /// The target element ID.
    /// </param>
    eval('__doPostBack(\'' + targetElementID + '\',\'\')');
}
SecMasterJSCommon.SMSCommons.smSelectCall = function SecMasterJSCommon_SMSCommons$smSelectCall(targetElementID, showSearch, isRunningText, readyFunction) {
    /// <param name="targetElementID" type="String">
    /// </param>
    /// <param name="showSearch" type="Boolean">
    /// </param>
    /// <param name="isRunningText" type="Boolean">
    /// </param>
    /// <param name="readyFunction" type="Function">
    /// </param>
    eval('var obj = new Object(); obj.select = $(\'#\' + targetElementID); obj.showSearch = showSearch; obj.isRunningText = isRunningText; obj.ready = readyFunction; smselect.create(obj);');
}
SecMasterJSCommon.SMSCommons.smSetSelectedIndex = function SecMasterJSCommon_SMSCommons$smSetSelectedIndex(targetElementID, index) {
    /// <param name="targetElementID" type="String">
    /// </param>
    /// <param name="index" type="Number" integer="true">
    /// </param>
    eval('smselect.setOptionByIndex($(\'#smselect_\'+targetElementID), index);');
}
SecMasterJSCommon.SMSCommons.smSetSelectedValue = function SecMasterJSCommon_SMSCommons$smSetSelectedValue(targetElementID, value) {
    /// <param name="targetElementID" type="String">
    /// </param>
    /// <param name="value" type="String">
    /// </param>
    eval('smselect.setOptionByValue($(\'#smselect_\'+targetElementID), value);');
}
SecMasterJSCommon.SMSCommons.expandCollapsePanel = function SecMasterJSCommon_SMSCommons$expandCollapsePanel(collapseControlID) {
    /// <summary>
    /// Expand/Collapse the collapsable panel.
    /// </summary>
    /// <param name="collapseControlID" type="String">
    /// The CollapseControlID.
    /// </param>
    var collapseControl = com.ivp.rad.rscriptutils.RSValidators.getObject(collapseControlID);
    if (collapseControl != null) {
        collapseControl.click();
    }
}
SecMasterJSCommon.SMSCommons.getInnerContent = function SecMasterJSCommon_SMSCommons$getInnerContent(ele) {
    /// <summary>
    /// Gets the inner content of the dom element.
    /// </summary>
    /// <param name="ele" type="Object" domElement="true">
    /// The dom element.
    /// </param>
    /// <returns type="String"></returns>
    var value;
    if (ele.nodeName.toLowerCase() === 'input') {
        value = (ele).value;
    }
    else {
        var elemrnt = new com.ivp.rad.controls.scripts.RADBrowserScripts.RADDOMElement();
        value = elemrnt.getInnerContent(ele);
    }
    return value;
}
SecMasterJSCommon.SMSCommons.checkStringWithRegExOrString = function SecMasterJSCommon_SMSCommons$checkStringWithRegExOrString(ele, expression, ifRegEx, isCaseSensitive) {
    /// <summary>
    /// Checks the string with reg ex or string.
    /// </summary>
    /// <param name="ele" type="Object" domElement="true">
    /// The dom element whose inner value has to be matched.
    /// </param>
    /// <param name="expression" type="Array" elementType="String">
    /// The expression to be searched.
    /// </param>
    /// <param name="ifRegEx" type="Boolean">
    /// if the expression is a regEx set to <c>true</c>
    /// </param>
    /// <param name="isCaseSensitive" type="Boolean">
    /// if set to <c>true</c> [is case sensitive].
    /// </param>
    /// <returns type="Boolean"></returns>
    var value;
    if (ele.nodeName.toLowerCase() === 'input') {
        value = (ele).value;
    }
    else {
        var elemrnt = new com.ivp.rad.controls.scripts.RADBrowserScripts.RADDOMElement();
        value = elemrnt.getInnerContent(ele);
    }
    var flag = false;
    if (ifRegEx && value.trim().length !== 0) {
        var regEx = (isCaseSensitive) ? new RegExp(expression[0]) : new RegExp(expression[0], 'i');
        flag = regEx.test(value);
    }
    else {
        expression[expression.length] = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
        for (var i = 0; i < expression.length; i++) {
            if ((isCaseSensitive && value.trim() === expression[i].trim()) || (!isCaseSensitive && value.trim().toLowerCase() === expression[i].trim().toLowerCase())) {
                flag = true;
                break;
            }
        }
    }
    return flag;
}
SecMasterJSCommon.SMSCommons.clearGrid = function SecMasterJSCommon_SMSCommons$clearGrid(gridToClear) {
    /// <summary>
    /// Clears all the rows of the grid.
    /// </summary>
    /// <param name="gridToClear" type="Object" domElement="true">
    /// The grid object to clear.
    /// </param>
    for (; gridToClear.rows.length > 2; ) {
        gridToClear.deleteRow(gridToClear.rows.length - 1);
    }
}
SecMasterJSCommon.SMSCommons.clearSelectElement = function SecMasterJSCommon_SMSCommons$clearSelectElement(objSelectElement) {
    /// <summary>
    /// Clears the shuttle.
    /// </summary>
    /// <param name="objSelectElement" type="Object" domElement="true">
    /// The shuttle object to clear.
    /// </param>
    for (; objSelectElement.options.length > 0; ) {
        objSelectElement.removeChild(objSelectElement.options[0]);
    }
}
SecMasterJSCommon.SMSCommons.clearDOMElement = function SecMasterJSCommon_SMSCommons$clearDOMElement(ele) {
    /// <param name="ele" type="Object" domElement="true">
    /// </param>
    switch (ele.nodeName) {
        case 'TABLE':
            SecMasterJSCommon.SMSCommons.clearGrid(ele);
            break;
        case 'SELECT':
            SecMasterJSCommon.SMSCommons.clearSelectElement(ele);
            break;
        default:
            break;
    }
}
SecMasterJSCommon.SMSCommons.searchMasterDetailGrid = function SecMasterJSCommon_SMSCommons$searchMasterDetailGrid(SearchText, MasterGridClientId, MasterGridServerId, DetailGridServerId, DetailGridSummary, HeaderInnerText) {
    /// <param name="SearchText" type="String">
    /// </param>
    /// <param name="MasterGridClientId" type="String">
    /// </param>
    /// <param name="MasterGridServerId" type="String">
    /// </param>
    /// <param name="DetailGridServerId" type="String">
    /// </param>
    /// <param name="DetailGridSummary" type="String">
    /// </param>
    /// <param name="HeaderInnerText" type="String">
    /// </param>
    var EntityGroupGrid = document.getElementById(MasterGridClientId).children[1];
    var tr = null;
    for (var i = 0; i < EntityGroupGrid.rows.length; i++) {
        tr = EntityGroupGrid.rows[i];
        if (!(tr.id.search(new RegExp(MasterGridServerId, 'i')) > 0 && tr.id.search(new RegExp(DetailGridServerId, 'i')) < 0)) {
            continue;
        }
        tr.style.display = '';
        if (tr.cells[0].className === 'collapseGridButton') {
            eval('RAD_ToggleRow(tr.cells[0]);');
        }
        if (tr.innerText.replace(new RegExp(DetailGridSummary, 'i'), '').search(new RegExp(SearchText, 'i')) < 0) {
            if (tr.cells[0].className != null) {
                if (tr.cells[0].className === 'expandGridButton' || tr.cells[0].className === 'collapseGridButton') {
                    if (tr.nextSibling.innerText.replace(new RegExp(DetailGridSummary, 'i'), '').replace(new RegExp(HeaderInnerText, 'i'), '').search(new RegExp(SearchText, 'i')) > 0) {
                        tr.style.display = '';
                        continue;
                    }
                    else if (SearchText !== '') {
                        tr.style.display = 'none';
                    }
                }
            }
            else if (SearchText !== '') {
                tr.style.display = 'none';
            }
        }
    }
}
SecMasterJSCommon.SMSCommons.openHelpText = function SecMasterJSCommon_SMSCommons$openHelpText(OpenAnimation, id, client_id, lblToShowId, lblWithTextId) {
    /// <param name="OpenAnimation" type="String">
    /// </param>
    /// <param name="id" type="String">
    /// </param>
    /// <param name="client_id" type="String">
    /// </param>
    /// <param name="lblToShowId" type="String">
    /// </param>
    /// <param name="lblWithTextId" type="String">
    /// </param>
    eval('OpenHelpText(\'' + OpenAnimation + '\',\'' + id + '\',\'' + client_id + '\',\'' + lblToShowId + '\',\'' + lblWithTextId + '\')');
}
SecMasterJSCommon.SMSCommons.openWindowToUpdateMultipleSecurity = function SecMasterJSCommon_SMSCommons$openWindowToUpdateMultipleSecurity(secID) {
    /// <param name="secID" type="Array" elementType="String">
    /// </param>
    var objUIService = new com.ivp.secm.ui.Service();
    objUIService.SetValuesToSessionObjectForEditBulkSecurity(secID, Number.parseInvariant(eval('Math.round(Math.random() * 10090)').toString()), Function.createDelegate(null, SecMasterJSCommon.SMSCommons._onSuccessSetValuesToUpdateMultipleSecurities), Function.createDelegate(null, SecMasterJSCommon.SMSCommons.onFailure));
}
SecMasterJSCommon.SMSCommons.openWindowExceptions = function SecMasterJSCommon_SMSCommons$openWindowExceptions(secIds, objFilterExceptionInfo) {
    /// <param name="secIds" type="String">
    /// </param>
    /// <param name="objFilterExceptionInfo" type="FilterInfoForException">
    /// </param>
    var objUIService = new com.ivp.secm.ui.Service();
    var sessionIdentifierKey = SecMasterJSCommon.SMSCommons.getGUID();
    objUIService.SetValuesForException(secIds, sessionIdentifierKey, objFilterExceptionInfo, Function.createDelegate(null, SecMasterJSCommon.SMSCommons._onSuccessSetValuesForException), Function.createDelegate(null, SecMasterJSCommon.SMSCommons.onFailure), sessionIdentifierKey);
}
SecMasterJSCommon.SMSCommons._onSuccessSetValuesForException = function SecMasterJSCommon_SMSCommons$_onSuccessSetValuesForException(result, eventArgs) {
    /// <param name="result" type="Object">
    /// </param>
    /// <param name="eventArgs" type="Object">
    /// </param>
    var url = 'SMExceptionManager.aspx?identifier=ExceptionManager&mode=nw&SessionIdentifier=' + eventArgs;
    SecMasterJSCommon.SMSCommons.createTab('ExceptionManager', url, SecMasterJSCommon.SMSCommons.getGUID(), 'Exception Manager');
}
SecMasterJSCommon.SMSCommons.openWindowExceptionsSameWindow = function SecMasterJSCommon_SMSCommons$openWindowExceptionsSameWindow(secIds, objFilterExceptionInfo) {
    /// <param name="secIds" type="String">
    /// </param>
    /// <param name="objFilterExceptionInfo" type="FilterInfoForException">
    /// </param>
    var objUIService = new com.ivp.secm.ui.Service();
    var sessionIdentifierKey = SecMasterJSCommon.SMSCommons.getGUID();
    objUIService.SetValuesForException(secIds, sessionIdentifierKey, objFilterExceptionInfo, Function.createDelegate(null, SecMasterJSCommon.SMSCommons._onSuccessSetValuesForExceptionSameWindow), Function.createDelegate(null, SecMasterJSCommon.SMSCommons.onFailure), sessionIdentifierKey);
}
SecMasterJSCommon.SMSCommons._onSuccessSetValuesForExceptionSameWindow = function SecMasterJSCommon_SMSCommons$_onSuccessSetValuesForExceptionSameWindow(result, eventArgs) {
    /// <param name="result" type="Object">
    /// </param>
    /// <param name="eventArgs" type="Object">
    /// </param>
    var url = 'SMExceptionManager.aspx?identifier=ExceptionManager&SessionIdentifier=' + eventArgs;
    window.location.href = url;
}
SecMasterJSCommon.SMSCommons.openWindowExceptionsNewTab = function SecMasterJSCommon_SMSCommons$openWindowExceptionsNewTab(secIds, objFilterExceptionInfo) {
    /// <param name="secIds" type="String">
    /// </param>
    /// <param name="objFilterExceptionInfo" type="FilterInfoForException">
    /// </param>
    var objUIService = new com.ivp.secm.ui.Service();
    var sessionIdentifierKey = SecMasterJSCommon.SMSCommons.getGUID();
    objUIService.SetValuesForException(secIds, sessionIdentifierKey, objFilterExceptionInfo, Function.createDelegate(null, SecMasterJSCommon.SMSCommons._onSuccessSetValuesForExceptionNewTab), Function.createDelegate(null, SecMasterJSCommon.SMSCommons.onFailure), sessionIdentifierKey);
}
SecMasterJSCommon.SMSCommons._onSuccessSetValuesForExceptionNewTab = function SecMasterJSCommon_SMSCommons$_onSuccessSetValuesForExceptionNewTab(result, eventArgs) {
    /// <param name="result" type="Object">
    /// </param>
    /// <param name="eventArgs" type="Object">
    /// </param>
    var url = 'SMExceptionManager.aspx?identifier=ExceptionManager&SessionIdentifier=' + eventArgs;
    SecMasterJSCommon.SMSCommons.createTab('ExceptionManager', url, SecMasterJSCommon.SMSCommons.getGUID(), 'Manage Security Exceptions');
}
SecMasterJSCommon.SMSCommons.openViewBulkSecurity = function SecMasterJSCommon_SMSCommons$openViewBulkSecurity(externalVendorId, sectypeId, InstrumentIdType, InstrumentID) {
    /// <param name="externalVendorId" type="Number" integer="true">
    /// </param>
    /// <param name="sectypeId" type="Number" integer="true">
    /// </param>
    /// <param name="InstrumentIdType" type="String">
    /// </param>
    /// <param name="InstrumentID" type="String">
    /// </param>
    var url = 'SMCreateUpdateBulkSecurity.aspx?external_vendor_id=' + externalVendorId + '&sectype_id=' + sectypeId + '&InstrumentIdType=' + InstrumentIdType + '&InstrumentID=' + InstrumentID;
    var window_dimensions = 'toolbars=no,menubar=no,location=no,scrollbars=yes,resizable=yes,status=yes,height=' + eval('screen.availHeight - 50') + ',width=' + eval('screen.availWidth') + ',top=0, left=0';
    window.open(url, SecMasterJSCommon.SMSConstants.strinG_EMPTY, window_dimensions);
}
SecMasterJSCommon.SMSCommons.openWindowToDeleteSecurity = function SecMasterJSCommon_SMSCommons$openWindowToDeleteSecurity(secIds) {
    /// <param name="secIds" type="String">
    /// </param>
    var objUIService = new com.ivp.secm.ui.Service();
    objUIService.SetValuesToServiceObjectForDeleteSecurity(secIds, Function.createDelegate(null, SecMasterJSCommon.SMSCommons._onSuccessSetValuesToServiceObjectDeleteSecurity), Function.createDelegate(null, SecMasterJSCommon.SMSCommons.onFailure));
}
SecMasterJSCommon.SMSCommons._onSuccessSetValuesToUpdateMultipleSecurities = function SecMasterJSCommon_SMSCommons$_onSuccessSetValuesToUpdateMultipleSecurities(result, eventArgs) {
    /// <param name="result" type="Object">
    /// </param>
    /// <param name="eventArgs" type="Object">
    /// </param>
    var url = 'SMEditMulitpleSecurities.aspx?identifier=EditBulkSecurities&UniquePageIdentifier=' + result.toString();
    var window_dimensions = 'toolbars=no,menubar=no,location=no,scrollbars=yes,resizable=yes,status=yes,height=' + eval('screen.availHeight') + ',width=' + eval('screen.availWidth') + ',top=0, left=0';
    SecMasterJSCommon.SMSCommons.createTab('EditBulkSecurities', url, SecMasterJSCommon.SMSCommons.getGUID(), 'Update Multiple Securities');
}
SecMasterJSCommon.SMSCommons._onSuccessSetValuesToServiceObjectDeleteSecurity = function SecMasterJSCommon_SMSCommons$_onSuccessSetValuesToServiceObjectDeleteSecurity(result, eventArgs) {
    /// <param name="result" type="Object">
    /// </param>
    /// <param name="eventArgs" type="Object">
    /// </param>
    var url = 'SMDeleteSecurity.aspx?btnId=btnDeleteSecurityCallback';
    var window_dimensions = 'toolbars=no,menubar=no,location=no,scrollbars=yes,resizable=yes,status=yes,height=' + eval('screen.availHeight - 50') + ',width=' + eval('screen.availWidth') + ',top=0, left=0';
    window.open(url, SecMasterJSCommon.SMSConstants.strinG_EMPTY, window_dimensions);
}
SecMasterJSCommon.SMSCommons.onFailure = function SecMasterJSCommon_SMSCommons$onFailure(result, eventArgs) {
    /// <param name="result" type="Object">
    /// </param>
    /// <param name="eventArgs" type="Object">
    /// </param>
    alert(result.toString());
}
SecMasterJSCommon.SMSCommons.openExternalSystemStatusScreen = function SecMasterJSCommon_SMSCommons$openExternalSystemStatusScreen() {
    var url = 'SMDownstreamSystemStatus.aspx?identifier=ExternalSystemStatus&mode=nw';
    var window_dimensions = 'toolbars=no,menubar=no,location=no,scrollbars=yes,resizable=yes,status=yes,height=' + eval('screen.availHeight - 50') + ',width=' + eval('screen.availWidth') + ',top=0, left=0';
    SecMasterJSCommon.SMSCommons.createTab('', url, SecMasterJSCommon.SMSCommons.getGUID(), 'Downstream Post Status');
}
SecMasterJSCommon.SMSCommons.openRealTimeSecurityStatusScreen = function SecMasterJSCommon_SMSCommons$openRealTimeSecurityStatusScreen() {
    var url = 'SMRealTimeSecurityStatus.aspx?identifier=BackgroundSecurityStatus&mode=nw';
    var window_dimensions = 'toolbars=no,menubar=no,location=no,scrollbars=yes,resizable=yes,status=yes,height=' + eval('screen.availHeight - 50') + ',width=' + eval('screen.availWidth') + ',top=0, left=0';
    window.open(url, SecMasterJSCommon.SMSConstants.strinG_EMPTY, window_dimensions);
}
SecMasterJSCommon.SMSCommons.getSeparatorStringFromArray = function SecMasterJSCommon_SMSCommons$getSeparatorStringFromArray(lstValues, seperator) {
    /// <param name="lstValues" type="Array" elementType="String">
    /// </param>
    /// <param name="seperator" type="String">
    /// </param>
    /// <returns type="String"></returns>
    var strResult = '';
    if (lstValues != null && lstValues.length > 0) {
        for (var i = 0; i < lstValues.length; i++) {
            if (i < lstValues.length - 1) {
                strResult += lstValues[i] + seperator;
            }
            else {
                strResult += lstValues[i];
            }
        }
    }
    return strResult;
}
SecMasterJSCommon.SMSCommons.openWindowToSelectSecurity = function SecMasterJSCommon_SMSCommons$openWindowToSelectSecurity(pageIdentifier, lblId, txtBoxId, isMaster, underyingSecurityTypeId) {
    /// <param name="pageIdentifier" type="SecMasterJSCommon.SearchPageIdentifier">
    /// </param>
    /// <param name="lblId" type="String">
    /// </param>
    /// <param name="txtBoxId" type="String">
    /// </param>
    /// <param name="isMaster" type="Boolean">
    /// </param>
    /// <param name="underyingSecurityTypeId" type="Number" integer="true">
    /// </param>
    var url = String.format('SMSearch.aspx?isMaster={0}&underlying_sectype_id={1}&LabelControl={2}&txtBox={3}&identifier={4}&mode=nw', isMaster.toString(), underyingSecurityTypeId, lblId, txtBoxId, SecMasterJSCommon.SearchPageIdentifier.toString(pageIdentifier)) + '&UniquePageIdentifier=' + eval('Math.round(Math.random() * 10090)');
    var window_dimensions = 'toolbars=no,menubar=no,location=no,scrollbars=yes,resizable=yes,status=yes,height=700px,width=1000px';
    window.open(url, SecMasterJSCommon.SMSConstants.strinG_EMPTY, window_dimensions);
}
SecMasterJSCommon.SMSCommons.openWindowToSelectSecurityFromSearchResults = function SecMasterJSCommon_SMSCommons$openWindowToSelectSecurityFromSearchResults(pageIdentifier, lblId, txtBoxId, isMaster, underyingSecurityTypeId, attributeId, searchText, attribtueName, isExact) {
    /// <param name="pageIdentifier" type="SecMasterJSCommon.SearchPageIdentifier">
    /// </param>
    /// <param name="lblId" type="String">
    /// </param>
    /// <param name="txtBoxId" type="String">
    /// </param>
    /// <param name="isMaster" type="Boolean">
    /// </param>
    /// <param name="underyingSecurityTypeId" type="Number" integer="true">
    /// </param>
    /// <param name="attributeId" type="Number" integer="true">
    /// </param>
    /// <param name="searchText" type="String">
    /// </param>
    /// <param name="attribtueName" type="String">
    /// </param>
    /// <param name="isExact" type="Boolean">
    /// </param>
    var url = String.format('SMSearch.aspx?isMaster={0}&underlying_sectype_id={1}&LabelControl={2}&txtBox={3}&identifier={4}&attrID={5}&searchText={6}&attrName={7}&isExact={8}&mode=nw', isMaster.toString(), underyingSecurityTypeId, lblId, txtBoxId, SecMasterJSCommon.SearchPageIdentifier.toString(pageIdentifier), attributeId, searchText, attribtueName, (isExact) ? '1' : '0') + '&UniquePageIdentifier=' + eval('Math.round(Math.random() * 10090)');
    var window_dimensions = 'toolbars=no,menubar=no,location=no,scrollbars=yes,resizable=yes,status=yes,height=700px,width=1000px';
    window.open(url, SecMasterJSCommon.SMSConstants.strinG_EMPTY, window_dimensions);
}
SecMasterJSCommon.SMSCommons.isNullOrEmpty = function SecMasterJSCommon_SMSCommons$isNullOrEmpty(value) {
    /// <param name="value" type="String">
    /// </param>
    /// <returns type="Boolean"></returns>
    var flag = true;
    if (value != null && value !== SecMasterJSCommon.SMSConstants.strinG_EMPTY) {
        flag = false;
    }
    return flag;
}
SecMasterJSCommon.SMSCommons.makeGroupsInSelectElement = function SecMasterJSCommon_SMSCommons$makeGroupsInSelectElement(selectElementToModify, valueOfGroupOption) {
    /// <param name="selectElementToModify" type="Object" domElement="true">
    /// </param>
    /// <param name="valueOfGroupOption" type="String">
    /// </param>
    for (var i = 0; i < selectElementToModify.options.length; i++) {
        var opt = selectElementToModify.options[i];
        if (opt.value === valueOfGroupOption) {
            opt.disabled = true;
        }
    }
}
SecMasterJSCommon.SMSCommons.manageList = function SecMasterJSCommon_SMSCommons$manageList(lstValue, separator, operation, itemValue) {
    /// <param name="lstValue" type="String">
    /// </param>
    /// <param name="separator" type="String">
    /// </param>
    /// <param name="operation" type="SecMasterJSCommon.ListOperation">
    /// </param>
    /// <param name="itemValue" type="String">
    /// </param>
    /// <returns type="Boolean"></returns>
    switch (operation) {
        case SecMasterJSCommon.ListOperation.ADD:
            if (SecMasterJSCommon.SMSCommons.containsWithinArray(lstValue.split(separator), itemValue, false)) {
                return false;
            }
            lstValue = lstValue + itemValue + separator;
            return true;
            break;
        case SecMasterJSCommon.ListOperation.removE_ITEM:
            if (!SecMasterJSCommon.SMSCommons.containsWithinArray(lstValue.split(separator), itemValue, false)) {
                return false;
            }
            lstValue = separator + lstValue;
            lstValue = lstValue.replace(separator + itemValue + separator, separator.toString());
            lstValue = lstValue.substr(1);
            return true;
            break;
        case SecMasterJSCommon.ListOperation.removE_ALL:
            lstValue = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
            return true;
            break;
        case SecMasterJSCommon.ListOperation.checK_CONTAINS:
            if (SecMasterJSCommon.SMSCommons.containsWithinArray(lstValue.split(separator), itemValue, false)) {
                return true;
            }
            return false;
            break;
        default:
            break;
    }
    return false;
}
SecMasterJSCommon.SMSCommons.findRowIndexByRowId = function SecMasterJSCommon_SMSCommons$findRowIndexByRowId(grid, rowId) {
    /// <param name="grid" type="Object" domElement="true">
    /// </param>
    /// <param name="rowId" type="String">
    /// </param>
    /// <returns type="Number" integer="true"></returns>
    var rowLength = grid.rows.length;
    var rowIndex = -1;
    for (var i = 0; i < rowLength; i++) {
        if (grid.rows[i].id.trim().toLowerCase() === rowId.trim().toLowerCase()) {
            rowIndex = i;
            break;
        }
    }
    return rowIndex;
}
SecMasterJSCommon.SMSCommons.onUpdating = function SecMasterJSCommon_SMSCommons$onUpdating() {
    eval('IsPageReloading = true;');
    try {
        com.ivp.rad.rscriptutils.RSValidators.getObject('disableDiv').style.display = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
        com.ivp.rad.rscriptutils.RSValidators.getObject('loadingImg').style.display = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
        var scrollHeight = document.body.scrollHeight.toString();
        var scrollWidth = document.body.scrollWidth.toString();
        var windowHeight = document.body.parentNode.offsetHeight.toString();
        com.ivp.rad.rscriptutils.RSValidators.getObject('disableDiv').style.height = scrollHeight + 'px';
        com.ivp.rad.rscriptutils.RSValidators.getObject('disableDiv').style.width = scrollWidth + 'px';
    }
    catch (ex) {
        throw ex;
    }
}
SecMasterJSCommon.SMSCommons.onUpdated = function SecMasterJSCommon_SMSCommons$onUpdated() {
    eval('IsPageReloading = false;');
    try {
        com.ivp.rad.rscriptutils.RSValidators.getObject('disableDiv').style.display = SecMasterJSCommon.SMSConstants.strinG_NONE;
        com.ivp.rad.rscriptutils.RSValidators.getObject('loadingImg').style.display = SecMasterJSCommon.SMSConstants.strinG_NONE;
        var _inputElement = (com.ivp.rad.rscriptutils.RSValidators.getObject('identifier'));
        if (_inputElement != null) {
            _inputElement.value = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
        }
    }
    catch (ex) {
        throw ex;
    }
}
SecMasterJSCommon.SMSCommons.isGridNullOrEmpty = function SecMasterJSCommon_SMSCommons$isGridNullOrEmpty(grid, hasDummyRow) {
    /// <param name="grid" type="Object" domElement="true">
    /// </param>
    /// <param name="hasDummyRow" type="Boolean">
    /// </param>
    /// <returns type="Boolean"></returns>
    var flag = true;
    if (hasDummyRow) {
        if (grid != null && grid.rows.length > 2) {
            flag = false;
        }
    }
    else {
        if (grid != null && grid.rows.length > 0) {
            flag = false;
        }
    }
    return flag;
}
SecMasterJSCommon.SMSCommons.isSelectNullOrEmpty = function SecMasterJSCommon_SMSCommons$isSelectNullOrEmpty(select) {
    /// <param name="select" type="Object" domElement="true">
    /// </param>
    /// <returns type="Boolean"></returns>
    var flag = true;
    if (select != null && select.options.length > 0) {
        flag = false;
    }
    return flag;
}
SecMasterJSCommon.SMSCommons.stringToArray = function SecMasterJSCommon_SMSCommons$stringToArray(value, separator) {
    /// <param name="value" type="String">
    /// </param>
    /// <param name="separator" type="String">
    /// </param>
    /// <returns type="Array" elementType="String"></returns>
    var List = null;
    if (!SecMasterJSCommon.SMSCommons.isNullOrEmpty(value)) {
        List = value.split(separator);
    }
    return List;
}
SecMasterJSCommon.SMSCommons.genericParser = function SecMasterJSCommon_SMSCommons$genericParser(xml) {
    /// <param name="xml" type="String">
    /// </param>
    /// <returns type="Object" domElement="true"></returns>
    var xmlDoc = new ActiveXObject('Microsoft.XMLDOM');
    xmlDoc.loadXML(xml);
    xmlDoc.setProperty('SelectionLanguage', 'XPath');
    return xmlDoc;
}
SecMasterJSCommon.SMSCommons.getColumnDataOfGridAsArray = function SecMasterJSCommon_SMSCommons$getColumnDataOfGridAsArray(grid, columnNumber, hasDummyRow) {
    /// <param name="grid" type="Object" domElement="true">
    /// </param>
    /// <param name="columnNumber" type="Number" integer="true">
    /// </param>
    /// <param name="hasDummyRow" type="Boolean">
    /// </param>
    /// <returns type="Array"></returns>
    var list = [];
    if (!hasDummyRow && !SecMasterJSCommon.SMSCommons.isGridNullOrEmpty(grid, hasDummyRow)) {
        for (var i = 1; i < grid.rows.length; i++) {
            list[i - 1] = grid.rows[i].childNodes[columnNumber].innerText.trim();
        }
    }
    if (hasDummyRow && !SecMasterJSCommon.SMSCommons.isGridNullOrEmpty(grid, hasDummyRow)) {
        for (var i = 2; i < grid.rows.length; i++) {
            list[i - 2] = grid.rows[i].childNodes[columnNumber].innerText.trim();
        }
    }
    return list;
}
SecMasterJSCommon.SMSCommons.resetLeftHeight = function SecMasterJSCommon_SMSCommons$resetLeftHeight() {
    eval('resetLeftHeightForSearch();');
}
SecMasterJSCommon.SMSCommons.resetGridSizeException = function SecMasterJSCommon_SMSCommons$resetGridSizeException(GridId, PanelTopId, PanelMiddleId, PanelBottomId, SubtractSize) {
    /// <param name="GridId" type="String">
    /// </param>
    /// <param name="PanelTopId" type="String">
    /// </param>
    /// <param name="PanelMiddleId" type="String">
    /// </param>
    /// <param name="PanelBottomId" type="String">
    /// </param>
    /// <param name="SubtractSize" type="Number" integer="true">
    /// </param>
    var gridPanelId = GridId + '_gridPanel';
    var gridHeaderPanelId = GridId + '_gridHeaderPanel';
    var gridTablePanelId = GridId + '_gridTablePanel';
    var gridScrollId = GridId + '_scroll';
    var gridPagerId = GridId + '_Div_Pager';
    var gridStatusPanelId = GridId + '_gridStatusPanel';
    var tdTopId = 'tdTop';
    var tdBottomId = 'tdBottom';
    var RADDragDropHeaderId = GridId + 'RADDragDropHeader_visualElementHolder';
    var GridPanel = com.ivp.rad.rscriptutils.RSValidators.getObject(gridPanelId);
    var GridHeaderPanel = com.ivp.rad.rscriptutils.RSValidators.getObject(gridHeaderPanelId);
    var GridTablePanel = com.ivp.rad.rscriptutils.RSValidators.getObject(gridTablePanelId);
    var GridScroll = com.ivp.rad.rscriptutils.RSValidators.getObject(gridScrollId);
    var GridPager = com.ivp.rad.rscriptutils.RSValidators.getObject(gridPagerId);
    var GridStatusPanel = com.ivp.rad.rscriptutils.RSValidators.getObject(gridStatusPanelId);
    var PanelTop = com.ivp.rad.rscriptutils.RSValidators.getObject(PanelTopId);
    var PanelMiddle = com.ivp.rad.rscriptutils.RSValidators.getObject(PanelMiddleId);
    var PanelBottom = com.ivp.rad.rscriptutils.RSValidators.getObject(PanelBottomId);
    var TdTop = com.ivp.rad.rscriptutils.RSValidators.getObject(tdTopId);
    var TdBottom = com.ivp.rad.rscriptutils.RSValidators.getObject(tdBottomId);
    var RADDragDropHeader = com.ivp.rad.rscriptutils.RSValidators.getObject(RADDragDropHeaderId);
    var ViewPortHeight = 0;
    if (window.screen.width === 1280 && window.screen.height === 768) {
        ViewPortHeight = (document.documentElement.clientHeight < 583) ? 583 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1280 && window.screen.height === 960) {
        ViewPortHeight = (document.documentElement.clientHeight < 775) ? 775 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1280 && window.screen.height === 1024) {
        ViewPortHeight = (document.documentElement.clientHeight < 839) ? 839 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1400 && window.screen.height === 1050) {
        ViewPortHeight = (document.documentElement.clientHeight < 865) ? 865 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1600 && window.screen.height === 900) {
        ViewPortHeight = (document.documentElement.clientHeight < 715) ? 715 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1600 && window.screen.height === 1200) {
        ViewPortHeight = (document.documentElement.clientHeight < 1015) ? 1015 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1920 && window.screen.height === 1080) {
        ViewPortHeight = (document.documentElement.clientHeight < 895) ? 895 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1920 && window.screen.height === 1200) {
        ViewPortHeight = (document.documentElement.clientHeight < 1015) ? 1015 : document.documentElement.clientHeight;
    }
    else {
        ViewPortHeight = (document.documentElement.clientHeight < 583) ? 583 : document.documentElement.clientHeight;
    }
    var TdTopHeight = 0;
    if (TdTop != null) {
        TdTopHeight = TdTop.offsetHeight;
    }
    var TdBottomHeight = 0;
    if (TdBottom != null) {
        TdBottomHeight = TdBottom.offsetHeight;
    }
    if (RADDragDropHeader != null) {
        RADDragDropHeader.style.display = SecMasterJSCommon.SMSConstants.strinG_NONE;
        RADDragDropHeader.style.height = '0px';
    }
    var PanelTopSearchHeight = PanelTop.offsetHeight;
    var PanelBottomSearchHeight = (PanelBottom == null) ? 0 : PanelBottom.offsetHeight;
    var GridHeaderPanelHeight = (GridHeaderPanel == null) ? 0 : GridHeaderPanel.offsetHeight;
    if (GridScroll != null) {
        GridScroll.style.display = 'none';
    }
    if (GridPager != null) {
        GridPager.style.display = 'none';
    }
    var GridScrollHeight = (GridScroll == null) ? 0 : GridScroll.offsetHeight;
    var GridStatusPanelHeight = (GridStatusPanel == null) ? 0 : GridStatusPanel.childNodes[0].offsetHeight;
    var BlackBar = SecMasterJSCommon.SMSCommons.createElementUsingSelector('.iago-page-title');
    var BlackBarHeight = 0;
    if (BlackBar != null) {
        BlackBarHeight = BlackBar.height();
    }
    var NavigationBar = SecMasterJSCommon.SMSCommons.createElementUsingSelector('div[id$=SecTypeWizardNavigationBar]');
    var NavigationBarHeight = 0;
    if (NavigationBar != null) {
        if (NavigationBar.css('display') !== 'none') {
            NavigationBarHeight = NavigationBar.height() - TdBottomHeight;
        }
    }
    var heightGridTablePanel = ViewPortHeight - TdTopHeight - TdBottomHeight - BlackBarHeight - NavigationBarHeight - PanelTopSearchHeight - PanelBottomSearchHeight - GridHeaderPanelHeight - GridScrollHeight - GridStatusPanelHeight - SubtractSize;
    if (GridTablePanel != null) {
        GridTablePanel.style.styleFloat = 'none';
        GridTablePanel.style.height = heightGridTablePanel.toString() + 'px';
    }
    var table = document.getElementById('table');
    if (table != null) {
        table.style.height = (document.documentElement.clientHeight - (TdBottomHeight + TdTopHeight + BlackBarHeight + NavigationBarHeight) - SubtractSize) + 'px';
    }
    eval('$(\'div[id$=divWrapperContainer]\').outerHeight($(\'#table\').outerHeight());');
}
SecMasterJSCommon.SMSCommons.resetGridSize = function SecMasterJSCommon_SMSCommons$resetGridSize(GridId, PanelTopId, PanelMiddleId, PanelBottomId, SubtractSize) {
    /// <param name="GridId" type="String">
    /// </param>
    /// <param name="PanelTopId" type="String">
    /// </param>
    /// <param name="PanelMiddleId" type="String">
    /// </param>
    /// <param name="PanelBottomId" type="String">
    /// </param>
    /// <param name="SubtractSize" type="Number" integer="true">
    /// </param>
    var gridPanelId = GridId + '_gridPanel';
    var gridHeaderPanelId = GridId + '_gridHeaderPanel';
    var gridTablePanelId = GridId + '_gridTablePanel';
    var gridScrollId = GridId + '_scroll';
    var gridStatusPanelId = GridId + '_gridStatusPanel';
    var tdMainId = 'tdMain';
    var tdLeftId = 'tdLeft';
    var tdMiddleId = 'tdMiddle';
    var tdTopId = 'tdTop';
    var tdBottomId = 'tdBottom';
    var RADDragDropHeaderId = GridId + 'RADDragDropHeader_visualElementHolder';
    var aspNetFormId = 'aspnetForm';
    var upMainId = 'ctl00_upMain';
    var GridPanel = com.ivp.rad.rscriptutils.RSValidators.getObject(gridPanelId);
    var GridHeaderPanel = com.ivp.rad.rscriptutils.RSValidators.getObject(gridHeaderPanelId);
    var GridTablePanel = com.ivp.rad.rscriptutils.RSValidators.getObject(gridTablePanelId);
    var GridScroll = com.ivp.rad.rscriptutils.RSValidators.getObject(gridScrollId);
    var GridStatusPanel = com.ivp.rad.rscriptutils.RSValidators.getObject(gridStatusPanelId);
    var PanelTop = com.ivp.rad.rscriptutils.RSValidators.getObject(PanelTopId);
    var PanelMiddle = com.ivp.rad.rscriptutils.RSValidators.getObject(PanelMiddleId);
    var PanelBottom = com.ivp.rad.rscriptutils.RSValidators.getObject(PanelBottomId);
    var TdMain = com.ivp.rad.rscriptutils.RSValidators.getObject(tdMainId);
    var TdLeft = com.ivp.rad.rscriptutils.RSValidators.getObject(tdLeftId);
    var TdMiddle = com.ivp.rad.rscriptutils.RSValidators.getObject(tdMiddleId);
    var TdTop = com.ivp.rad.rscriptutils.RSValidators.getObject(tdTopId);
    var TdBottom = com.ivp.rad.rscriptutils.RSValidators.getObject(tdBottomId);
    var RADDragDropHeader = com.ivp.rad.rscriptutils.RSValidators.getObject(RADDragDropHeaderId);
    var FormElement = com.ivp.rad.rscriptutils.RSValidators.getObject(aspNetFormId);
    var UpMain = com.ivp.rad.rscriptutils.RSValidators.getObject(upMainId);
    var ViewPortHeight = 0;
    if (window.screen.width === 1280 && window.screen.height === 768) {
        ViewPortHeight = (document.documentElement.clientHeight < 583) ? 583 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1280 && window.screen.height === 960) {
        ViewPortHeight = (document.documentElement.clientHeight < 775) ? 775 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1280 && window.screen.height === 1024) {
        ViewPortHeight = (document.documentElement.clientHeight < 839) ? 839 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1400 && window.screen.height === 1050) {
        ViewPortHeight = (document.documentElement.clientHeight < 865) ? 865 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1600 && window.screen.height === 900) {
        ViewPortHeight = (document.documentElement.clientHeight < 715) ? 715 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1600 && window.screen.height === 1200) {
        ViewPortHeight = (document.documentElement.clientHeight < 1015) ? 1015 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1920 && window.screen.height === 1080) {
        ViewPortHeight = (document.documentElement.clientHeight < 895) ? 895 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1920 && window.screen.height === 1200) {
        ViewPortHeight = (document.documentElement.clientHeight < 1015) ? 1015 : document.documentElement.clientHeight;
    }
    else {
        ViewPortHeight = (document.documentElement.clientHeight < 583) ? 583 : document.documentElement.clientHeight;
    }
    var TdTopHeight = 0;
    if (TdTop != null) {
        TdTopHeight = TdTop.clientHeight;
    }
    var TdBottomHeight = 0;
    if (TdBottom != null) {
        TdBottomHeight = TdBottom.clientHeight;
    }
    if (RADDragDropHeader != null) {
        RADDragDropHeader.style.display = SecMasterJSCommon.SMSConstants.strinG_NONE;
        RADDragDropHeader.style.height = '0px';
    }
    if (FormElement != null && FormElement.offsetHeight < ViewPortHeight) {
        FormElement.style.height = ViewPortHeight + 'px';
    }
    var MainHeight = ViewPortHeight - TdTopHeight - TdBottomHeight;
    var TdMainHeight = MainHeight;
    var PanelTopSearchHeight = PanelTop.offsetHeight;
    var PanelBottomSearchHeight = (PanelBottom == null) ? 0 : PanelBottom.offsetHeight;
    var GridHeaderPanelHeight = (GridHeaderPanel == null) ? 0 : GridHeaderPanel.offsetHeight;
    var GridScrollHeight = (GridScroll == null) ? 0 : GridScroll.offsetHeight;
    var GridStatusPanelHeight = (GridStatusPanel == null) ? 0 : GridStatusPanel.offsetHeight;
    var heightGridTablePanel = TdMainHeight - PanelTopSearchHeight - PanelBottomSearchHeight - GridHeaderPanelHeight - GridScrollHeight - GridStatusPanelHeight - SubtractSize;
    if (TdMain != null && UpMain != null && FormElement != null) {
        if (TdMain.offsetHeight < TdMainHeight && UpMain.offsetHeight < TdMainHeight && FormElement.offsetHeight < ViewPortHeight) {
            TdMain.style.height = TdMainHeight + 'px';
            UpMain.style.height = TdMainHeight + 'px';
            FormElement.style.height = ViewPortHeight + 'px';
        }
    }
    if (GridTablePanel != null) {
        GridTablePanel.style.height = heightGridTablePanel.toString() + 'px';
    }
    if (GridPanel != null) {
        GridPanel.style.height = (MainHeight - PanelTopSearchHeight - PanelBottomSearchHeight) + 'px';
    }
    if (PanelMiddle != null) {
        PanelMiddle.style.height = (MainHeight - PanelTopSearchHeight - PanelBottomSearchHeight - SubtractSize) + 'px';
    }
}
SecMasterJSCommon.SMSCommons.resizeXlGrid = function SecMasterJSCommon_SMSCommons$resizeXlGrid(XlGridId, PanelTopId, PanelMiddleId, PanelBottomId, SubtractHeight, SubtractWidth) {
    /// <param name="XlGridId" type="String">
    /// </param>
    /// <param name="PanelTopId" type="String">
    /// </param>
    /// <param name="PanelMiddleId" type="String">
    /// </param>
    /// <param name="PanelBottomId" type="String">
    /// </param>
    /// <param name="SubtractHeight" type="Number" integer="true">
    /// </param>
    /// <param name="SubtractWidth" type="Number" integer="true">
    /// </param>
    var XlGridDiv = com.ivp.rad.rscriptutils.RSValidators.getObject(XlGridId);
    var PanelTop = com.ivp.rad.rscriptutils.RSValidators.getObject(PanelTopId);
    var PanelMiddle = com.ivp.rad.rscriptutils.RSValidators.getObject(PanelMiddleId);
    var PanelBottom = com.ivp.rad.rscriptutils.RSValidators.getObject(PanelBottomId);
    var tdMainId = 'tdMain';
    var tdLeftId = 'tdLeft';
    var tdMiddleId = 'tdMiddle';
    var tdTopId = 'tdTop';
    var tdBottomId = 'tdBottom';
    var aspNetFormId = 'aspnetForm';
    var upMainId = 'ctl00_upMain';
    var TdMain = com.ivp.rad.rscriptutils.RSValidators.getObject(tdMainId);
    var TdLeft = com.ivp.rad.rscriptutils.RSValidators.getObject(tdLeftId);
    var TdMiddle = com.ivp.rad.rscriptutils.RSValidators.getObject(tdMiddleId);
    var TdTop = com.ivp.rad.rscriptutils.RSValidators.getObject(tdTopId);
    var TdBottom = com.ivp.rad.rscriptutils.RSValidators.getObject(tdBottomId);
    var FormElement = com.ivp.rad.rscriptutils.RSValidators.getObject(aspNetFormId);
    var UpMain = com.ivp.rad.rscriptutils.RSValidators.getObject(upMainId);
    if (XlGridDiv != null) {
        var XlGridUpperHeaderDiv = com.ivp.rad.rscriptutils.RSValidators.getObject(XlGridId + '_upperHeader_Div');
        var DivFrozenBody = com.ivp.rad.rscriptutils.RSValidators.getObject(XlGridId + '_frozen_bodyDiv');
        var DivBody = com.ivp.rad.rscriptutils.RSValidators.getObject(XlGridId + '_bodyDiv');
        var DivHeader = com.ivp.rad.rscriptutils.RSValidators.getObject(XlGridId + '_headerDiv');
        var XlGridFooterDiv = com.ivp.rad.rscriptutils.RSValidators.getObject(XlGridId + '_footer_Div');
        var XlGridHeaderDiv = com.ivp.rad.rscriptutils.RSValidators.getObject(XlGridId + '_headerDiv');
        var XlGridMainBodyDiv = com.ivp.rad.rscriptutils.RSValidators.getObject(XlGridId + '_bodyDiv');
        var XlGridFrozenBodyDiv = com.ivp.rad.rscriptutils.RSValidators.getObject(XlGridId + '_frozen_bodyDiv');
        var XlGridScrollerMainVerticalDiv = com.ivp.rad.rscriptutils.RSValidators.getObject(XlGridId + '_scrollerMainVertical');
        var ViewPortHeight = 0;
        if (window.screen.width === 1024 && window.screen.height === 768) {
            ViewPortHeight = (document.documentElement.clientHeight < 567) ? 567 : document.documentElement.clientHeight;
        }
        else if (window.screen.width === 1280 && window.screen.height === 768) {
            ViewPortHeight = (document.documentElement.clientHeight < 583) ? 583 : document.documentElement.clientHeight;
        }
        else if (window.screen.width === 1280 && window.screen.height === 960) {
            ViewPortHeight = (document.documentElement.clientHeight < 775) ? 775 : document.documentElement.clientHeight;
        }
        else if (window.screen.width === 1280 && window.screen.height === 1024) {
            ViewPortHeight = (document.documentElement.clientHeight < 839) ? 839 : document.documentElement.clientHeight;
        }
        else if (window.screen.width === 1350 && window.screen.height === 810) {
            ViewPortHeight = (document.documentElement.clientHeight < 615) ? 615 : document.documentElement.clientHeight;
        }
        else if (window.screen.width === 1400 && window.screen.height === 1050) {
            ViewPortHeight = (document.documentElement.clientHeight < 865) ? 865 : document.documentElement.clientHeight;
        }
        else if (window.screen.width === 1600 && window.screen.height === 900) {
            ViewPortHeight = (document.documentElement.clientHeight < 715) ? 715 : document.documentElement.clientHeight;
        }
        else if (window.screen.width === 1600 && window.screen.height === 1200) {
            ViewPortHeight = (document.documentElement.clientHeight < 1015) ? 1015 : document.documentElement.clientHeight;
        }
        else if (window.screen.width === 1920 && window.screen.height === 1080) {
            ViewPortHeight = (document.documentElement.clientHeight < 895) ? 895 : document.documentElement.clientHeight;
        }
        else if (window.screen.width === 1920 && window.screen.height === 1200) {
            ViewPortHeight = (document.documentElement.clientHeight < 1015) ? 1015 : document.documentElement.clientHeight;
        }
        else if (window.screen.width === 1680 && window.screen.height === 1050) {
            ViewPortHeight = (document.documentElement.clientHeight < 849) ? 849 : document.documentElement.clientHeight;
        }
        else {
            ViewPortHeight = document.documentElement.clientHeight;
        }
        var ViewPortWidth = 0;
        ViewPortWidth = document.documentElement.clientWidth;
        var TdTopHeight = 0;
        if (TdTop != null) {
            TdTopHeight = TdTop.clientHeight;
        }
        var TdBottomHeight = 0;
        if (TdBottom != null) {
            TdBottomHeight = TdBottom.clientHeight;
        }
        if (FormElement != null && FormElement.offsetHeight < ViewPortHeight) {
            FormElement.style.height = ViewPortHeight + 'px';
        }
        var GridWidth = ViewPortWidth - SubtractWidth;
        var XlGridUpperHeaderDivHeight = 0;
        if (XlGridUpperHeaderDiv != null) {
            XlGridUpperHeaderDivHeight = XlGridUpperHeaderDiv.offsetHeight;
            XlGridUpperHeaderDiv.style.width = GridWidth + 'px';
        }
        var XlGridFooterDivHeight = 0;
        if (XlGridFooterDiv != null) {
            XlGridFooterDivHeight = XlGridFooterDiv.offsetHeight;
            XlGridFooterDiv.style.width = GridWidth + 'px';
        }
        var XlGridHeaderDivHeight = 0;
        if (XlGridHeaderDiv != null) {
            XlGridHeaderDivHeight = XlGridHeaderDiv.clientHeight;
        }
        var XlGridFrozenBodyWidth = 0;
        if (DivFrozenBody != null) {
            XlGridFrozenBodyWidth = DivFrozenBody.clientWidth;
        }
        var PanelTopHeight = ((PanelTop != null) ? PanelTop.offsetHeight : 0);
        var PanelBottomHeight = ((PanelBottom != null) ? PanelBottom.offsetHeight : 0);
        var TdMainHeight = ViewPortHeight - TdTopHeight - TdBottomHeight;
        var GridHeight = TdMainHeight - PanelTopHeight - PanelBottomHeight - XlGridUpperHeaderDivHeight - XlGridFooterDivHeight - SubtractHeight;
        if (TdMain != null && UpMain != null && FormElement != null) {
            if (TdMain.offsetHeight < TdMainHeight && UpMain.offsetHeight < TdMainHeight && FormElement.offsetHeight < ViewPortHeight) {
                TdMain.style.height = TdMainHeight + 'px';
                UpMain.style.height = TdMainHeight + 'px';
                FormElement.style.height = ViewPortHeight + 'px';
            }
        }
        if (XlGridMainBodyDiv != null) {
            XlGridMainBodyDiv.style.height = (GridHeight - XlGridHeaderDivHeight) + 'px';
            XlGridMainBodyDiv.style.width = (GridWidth - XlGridFrozenBodyWidth - 15) + 'px';
        }
        if (DivHeader != null) {
            DivHeader.style.width = (GridWidth - XlGridFrozenBodyWidth - 15) + 'px';
        }
        if (XlGridFrozenBodyDiv != null) {
            XlGridFrozenBodyDiv.style.height = (GridHeight - XlGridHeaderDivHeight) + 'px';
        }
        if (XlGridScrollerMainVerticalDiv != null) {
            XlGridScrollerMainVerticalDiv.style.height = (GridHeight - 10) + 'px';
        }
        if (PanelMiddle != null) {
            PanelMiddle.style.height = (TdMainHeight - PanelTopHeight - PanelBottomHeight - SubtractHeight) + 'px';
        }
        var grid = $find(XlGridId);
        if (grid != null) {
            grid.manageScrolling();
        }
    }
}
SecMasterJSCommon.SMSCommons.addHandlersToCpe = function SecMasterJSCommon_SMSCommons$addHandlersToCpe(CpeId, handler) {
    /// <param name="CpeId" type="String">
    /// </param>
    /// <param name="handler" type="SecMasterJSCommon.CpeComplete">
    /// </param>
    var cpe = $find(CpeId);
    cpe.add_expandComplete(handler);
    cpe.add_collapseComplete(handler);
}
SecMasterJSCommon.SMSCommons.getSelectedIndices = function SecMasterJSCommon_SMSCommons$getSelectedIndices(select) {
    /// <param name="select" type="Object" domElement="true">
    /// </param>
    /// <returns type="Array"></returns>
    var selectedIndices = [];
    var j = 0;
    for (var i = 0; i < select.options.length; i++) {
        if ((select.options[i]).selected) {
            selectedIndices[j] = i;
            j++;
        }
    }
    return selectedIndices;
}
SecMasterJSCommon.SMSCommons.getSelectedIndicesCount = function SecMasterJSCommon_SMSCommons$getSelectedIndicesCount(select) {
    /// <param name="select" type="Object" domElement="true">
    /// </param>
    /// <returns type="Number" integer="true"></returns>
    var count = 0;
    for (var i = 0; i < select.options.length; i++) {
        if ((select.options[i]).selected) {
            count += 1;
        }
    }
    return count;
}
SecMasterJSCommon.SMSCommons.resetMasterDetailGrid = function SecMasterJSCommon_SMSCommons$resetMasterDetailGrid(GridId, PanelTopId, PanelMiddleId, PanelBottomId, SubtractSize) {
    /// <param name="GridId" type="String">
    /// </param>
    /// <param name="PanelTopId" type="String">
    /// </param>
    /// <param name="PanelMiddleId" type="String">
    /// </param>
    /// <param name="PanelBottomId" type="String">
    /// </param>
    /// <param name="SubtractSize" type="Number" integer="true">
    /// </param>
    var tdMainId = 'tdMain';
    var tdLeftId = 'tdLeft';
    var tdMiddleId = 'tdMiddle';
    var tdTopId = 'tdTop';
    var tdBottomId = 'tdBottom';
    var RADDragDropHeaderId = GridId + 'RADDragDropHeader_visualElementHolder';
    var aspNetFormId = 'aspnetForm';
    var Grid = com.ivp.rad.rscriptutils.RSValidators.getObject(GridId);
    var GridContainer = null;
    var THead = null;
    var TBody = null;
    if (Grid != null) {
        GridContainer = Grid.parentNode;
        THead = Grid.childNodes[0];
        TBody = Grid.childNodes[1];
    }
    var PanelTop = com.ivp.rad.rscriptutils.RSValidators.getObject(PanelTopId);
    var PanelMiddle = com.ivp.rad.rscriptutils.RSValidators.getObject(PanelMiddleId);
    var PanelBottom = com.ivp.rad.rscriptutils.RSValidators.getObject(PanelBottomId);
    var TdMain = com.ivp.rad.rscriptutils.RSValidators.getObject(tdMainId);
    var TdLeft = com.ivp.rad.rscriptutils.RSValidators.getObject(tdLeftId);
    var TdMiddle = com.ivp.rad.rscriptutils.RSValidators.getObject(tdMiddleId);
    var TdTop = com.ivp.rad.rscriptutils.RSValidators.getObject(tdTopId);
    var TdBottom = com.ivp.rad.rscriptutils.RSValidators.getObject(tdBottomId);
    var RADDragDropHeader = com.ivp.rad.rscriptutils.RSValidators.getObject(RADDragDropHeaderId);
    var FormElement = com.ivp.rad.rscriptutils.RSValidators.getObject(aspNetFormId);
    var ViewPortHeight = 0;
    if (window.screen.width === 1280 && window.screen.height === 768) {
        ViewPortHeight = (document.documentElement.clientHeight < 583) ? 583 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1280 && window.screen.height === 960) {
        ViewPortHeight = (document.documentElement.clientHeight < 775) ? 775 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1280 && window.screen.height === 1024) {
        ViewPortHeight = (document.documentElement.clientHeight < 839) ? 839 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1400 && window.screen.height === 1050) {
        ViewPortHeight = (document.documentElement.clientHeight < 865) ? 865 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1600 && window.screen.height === 900) {
        ViewPortHeight = (document.documentElement.clientHeight < 715) ? 715 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1600 && window.screen.height === 1200) {
        ViewPortHeight = (document.documentElement.clientHeight < 1015) ? 1015 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1920 && window.screen.height === 1080) {
        ViewPortHeight = (document.documentElement.clientHeight < 895) ? 895 : document.documentElement.clientHeight;
    }
    else if (window.screen.width === 1920 && window.screen.height === 1200) {
        ViewPortHeight = (document.documentElement.clientHeight < 1015) ? 1015 : document.documentElement.clientHeight;
    }
    else {
        ViewPortHeight = (document.documentElement.clientHeight < 583) ? 583 : document.documentElement.clientHeight;
    }
    var TdTopHeight = 0;
    if (TdTop != null) {
        TdTopHeight = TdTop.clientHeight;
    }
    var TdBottomHeight = 0;
    if (TdBottom != null) {
        TdBottomHeight = TdBottom.clientHeight;
    }
    if (RADDragDropHeader != null) {
        RADDragDropHeader.style.display = SecMasterJSCommon.SMSConstants.strinG_NONE;
        RADDragDropHeader.style.height = '0px';
    }
    if (FormElement != null) {
        FormElement.style.height = ViewPortHeight + 'px';
    }
    var MainHeight = ViewPortHeight - TdTopHeight - TdBottomHeight;
    var TdMainHeight = MainHeight;
    var PanelTopSearchHeight = PanelTop.offsetHeight;
    var PanelBottomSearchHeight = (PanelBottom == null) ? 0 : PanelBottom.offsetHeight;
    var THeadHeight = (THead == null) ? 0 : THead.offsetHeight;
    var GridContainerHeight = TdMainHeight - PanelTopSearchHeight - PanelBottomSearchHeight - SubtractSize;
    if (GridContainer != null) {
        GridContainer.style.height = GridContainerHeight.toString() + 'px';
    }
    if (PanelMiddle != null) {
        PanelMiddle.style.height = (MainHeight - PanelTopSearchHeight - PanelBottomSearchHeight) + 'px';
    }
    if (FormElement != null) {
        FormElement.style.height = ViewPortHeight + 'px';
    }
}
SecMasterJSCommon.SMSCommons.hideRADShuttleOrderOption = function SecMasterJSCommon_SMSCommons$hideRADShuttleOrderOption(shuttleId) {
    /// <param name="shuttleId" type="String">
    /// </param>
    var ele = com.ivp.rad.rscriptutils.RSValidators.getObject(shuttleId);
    ((ele.childNodes[0]).rows[0]).cells[3].style.display = SecMasterJSCommon.SMSConstants.strinG_NONE;
}
SecMasterJSCommon.SMSCommons.openWindowEditCompareMultipleSecurities = function SecMasterJSCommon_SMSCommons$openWindowEditCompareMultipleSecurities(secIdsCsv, report_setup_id) {
    /// <param name="secIdsCsv" type="String">
    /// </param>
    /// <param name="report_setup_id" type="Number" integer="true">
    /// </param>
    var url = 'SMEditCompareSecurities.aspx?identifier=CreateEditMultiple&SecIds=' + secIdsCsv + '&&report_setup_id=' + report_setup_id;
    var window_dimensions = 'toolbars=no,menubar=no,location=no,scrollbars=yes,resizable=yes,status=yes,height=' + eval('screen.availHeight - 50') + ',width=' + eval('screen.availWidth') + ',top=0, left=0';
    SecMasterJSCommon.SMSCommons.createTab('', url, SecMasterJSCommon.SMSCommons.getGUID(), 'Edit and Compare Securities');
}
SecMasterJSCommon.SMSCommons.numericOnly = function SecMasterJSCommon_SMSCommons$numericOnly() {
    /// <returns type="Boolean"></returns>
    var textfield = window.event.srcElement;
    var key = 0;
    var keychar;
    if (window.event != null) {
        key = window.event.keyCode;
    }
    keychar = String.fromCharCode(key);
    if ((key === 0) || (key === 8) || (key === 9) || (key === 13) || (key === 27)) {
        return true;
    }
    else if (('0123456789'.indexOf(keychar) > -1)) {
        return true;
    }
    else if (keychar === '.') {
        var text = textfield.value;
        if (text.indexOf(keychar) > -1) {
            return false;
        }
        else {
            return true;
        }
    }
    else {
        return false;
    }
}
SecMasterJSCommon.SMSCommons.loadIPAddress = function SecMasterJSCommon_SMSCommons$loadIPAddress() {
    /// <returns type="String"></returns>
    var pipeSeparatedIPAddressList = '';
    eval('pipeSeparatedIPAddressList = LoadIPAddressString();');
    return pipeSeparatedIPAddressList;
}
SecMasterJSCommon.SMSCommons.getSelectedCategories = function SecMasterJSCommon_SMSCommons$getSelectedCategories(key) {
    /// <param name="key" type="String">
    /// </param>
    /// <returns type="String"></returns>
    var categoriesSerialized = '';
    eval('categoriesSerialized = JSON.stringify(getSelectedCategories(\'' + key + '\'));');
    return categoriesSerialized;
}
SecMasterJSCommon.SMSCommons.showCustomPopup = function SecMasterJSCommon_SMSCommons$showCustomPopup(div) {
    /// <param name="div" type="Object" domElement="true">
    /// </param>
    var viewPortHeight = document.body.clientHeight;
    var viewPortWidth = document.body.clientWidth;
    div.style.display = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
    div.style.position = 'fixed';
    div.style.verticalAlign = 'middle';
    div.style.zIndex = 10001;
    var divHeight = div.offsetHeight;
    var divWidth = div.offsetWidth;
    div.style.top = ((viewPortHeight - divHeight) / 2).toString() + 'px';
    div.style.left = ((viewPortWidth - divWidth) / 2).toString() + 'px';
    document.getElementById('disableDiv').style.display = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
}
SecMasterJSCommon.SMSCommons.hideCustomPopup = function SecMasterJSCommon_SMSCommons$hideCustomPopup(div) {
    /// <param name="div" type="Object" domElement="true">
    /// </param>
    div.style.display = SecMasterJSCommon.SMSConstants.strinG_NONE;
    document.getElementById('disableDiv').style.display = SecMasterJSCommon.SMSConstants.strinG_NONE;
}
SecMasterJSCommon.SMSCommons._generateExtraParametersString = function SecMasterJSCommon_SMSCommons$_generateExtraParametersString(lstExtraParameters) {
    /// <param name="lstExtraParameters" type="Array">
    /// </param>
    /// <returns type="String"></returns>
    var str = null;
    if (lstExtraParameters != null && lstExtraParameters.length > 0) {
        for (var i = 0; i < lstExtraParameters.length; i++) {
            ((lstExtraParameters[i])).Value = 'var objEval = \'' + ((lstExtraParameters[i])).Value + '\';';
        }
        str = Sys.Serialization.JavaScriptSerializer.serialize(lstExtraParameters);
    }
    return str;
}
SecMasterJSCommon.SMSCommons._generateExtraParametersArray = function SecMasterJSCommon_SMSCommons$_generateExtraParametersArray(lstExtraParameters) {
    /// <param name="lstExtraParameters" type="Array">
    /// </param>
    /// <returns type="Array"></returns>
    if (lstExtraParameters != null && lstExtraParameters.length > 0) {
        for (var i = 0; i < lstExtraParameters.length; i++) {
            ((lstExtraParameters[i])).Value = 'var objEval = \'' + ((lstExtraParameters[i])).Value + '\';';
        }
    }
    return lstExtraParameters;
}
SecMasterJSCommon.SMSCommons.createPageSwitchScript = function SecMasterJSCommon_SMSCommons$createPageSwitchScript(script, lstExtraParameters) {
    /// <param name="script" type="String">
    /// </param>
    /// <param name="lstExtraParameters" type="Array">
    /// </param>
    /// <returns type="String"></returns>
    var indexOfNull = script.indexOf('null');
    var indexOfOpeningBracket = script.indexOf('[');
    var indexOfEndingBracket = script.indexOf(']');
    if (indexOfNull === -1 && indexOfOpeningBracket !== -1 && indexOfEndingBracket !== -1) {
        var strExistingParameters = script.substring(script.indexOf('['), script.indexOf(']') + 1);
        var lstExistingExtraParameters = Sys.Serialization.JavaScriptSerializer.deserialize(strExistingParameters);
        var strFinalExtraParams = Sys.Serialization.JavaScriptSerializer.serialize(lstExistingExtraParameters.concat(SecMasterJSCommon.SMSCommons._generateExtraParametersArray(lstExtraParameters)));
        return script.replace(strExistingParameters, strFinalExtraParams);
    }
    else if (indexOfNull !== -1 && indexOfOpeningBracket === -1 && indexOfEndingBracket === -1) {
        return script.replace('null', SecMasterJSCommon.SMSCommons._generateExtraParametersString(lstExtraParameters));
    }
    else {
        return script;
    }
}
SecMasterJSCommon.SMSCommons.compareCreationDateFromCurrentDate = function SecMasterJSCommon_SMSCommons$compareCreationDateFromCurrentDate(dateToCheck) {
    /// <param name="dateToCheck" type="String">
    /// </param>
    /// <returns type="Boolean"></returns>
    var dtDate = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertStringToDateTime(dateToCheck, com.ivp.rad.rscriptutils.DateTimeFormat.shorDate);
    var curDate = new Date();
    var startTicks = com.ivp.rad.rscriptutils.RSValidators.returnTicks(Sys.Serialization.JavaScriptSerializer.serialize(dtDate));
    var currentTicks = com.ivp.rad.rscriptutils.RSValidators.returnTicks(Sys.Serialization.JavaScriptSerializer.serialize(curDate));
    if (startTicks < currentTicks) {
        return true;
    }
    if (dtDate <= curDate) {
        return true;
    }
    return false;
}
SecMasterJSCommon.SMSCommons.openVendorComparisonWindow = function SecMasterJSCommon_SMSCommons$openVendorComparisonWindow(securityId) {
    /// <param name="securityId" type="String">
    /// </param>
    var url = 'SMSecurityVendorData.aspx?securityId=' + securityId;
    var window_dimensions = 'toolbars=no,menubar=no,location=no,scrollbars=yes,resizable=yes,status=yes,height=700px,width=1000px';
    window.open(url, SecMasterJSCommon.SMSConstants.strinG_EMPTY, window_dimensions);
}
SecMasterJSCommon.SMSCommons.createElement = function SecMasterJSCommon_SMSCommons$createElement(element) {
    /// <param name="element" type="Object" domElement="true">
    /// </param>
    /// <returns type="SecMasterJSCommon.WrapperDom"></returns>
    var obj = null;
    eval('obj = $(element);');
    return obj;
}
SecMasterJSCommon.SMSCommons.createElementUsingSelector = function SecMasterJSCommon_SMSCommons$createElementUsingSelector(selector) {
    /// <param name="selector" type="String">
    /// </param>
    /// <returns type="SecMasterJSCommon.WrapperDom"></returns>
    var obj = null;
    eval('obj = $(selector);');
    return obj;
}
SecMasterJSCommon.SMSCommons.nextElementSibling = function SecMasterJSCommon_SMSCommons$nextElementSibling(selector) {
    /// <param name="selector" type="String">
    /// </param>
    /// <returns type="SecMasterJSCommon.WrapperDom"></returns>
    var obj = null;
    eval('obj = $(selector).next();');
    return obj;
}
SecMasterJSCommon.SMSCommons.insertAfter = function SecMasterJSCommon_SMSCommons$insertAfter(htmlString, selector) {
    /// <param name="htmlString" type="String">
    /// </param>
    /// <param name="selector" type="String">
    /// </param>
    eval('$(htmlString).insertAfter($(selector));');
}
SecMasterJSCommon.SMSCommons.removeHtml = function SecMasterJSCommon_SMSCommons$removeHtml(element) {
    /// <param name="element" type="SecMasterJSCommon.WrapperDom">
    /// </param>
    eval('element.remove();');
}
SecMasterJSCommon.SMSCommons.accordion = function SecMasterJSCommon_SMSCommons$accordion(selector, options) {
    /// <param name="selector" type="String">
    /// </param>
    /// <param name="options" type="String">
    /// </param>
    eval('$(selector).accordion({ collapsible: true, active: false });');
}
SecMasterJSCommon.SMSCommons.parseInt = function SecMasterJSCommon_SMSCommons$parseInt(number) {
    /// <param name="number" type="String">
    /// </param>
    /// <returns type="Number" integer="true"></returns>
    var obj = 0;
    eval('obj = parseInt(number);');
    return obj;
}
SecMasterJSCommon.SMSCommons.getChildNode = function SecMasterJSCommon_SMSCommons$getChildNode(element, index) {
    /// <param name="element" type="Object" domElement="true">
    /// </param>
    /// <param name="index" type="Number" integer="true">
    /// </param>
    /// <returns type="Object" domElement="true"></returns>
    var lst = SecMasterJSCommon.SMSCommons.createElement(element).children();
    if (lst != null && lst.length > 0) {
        return lst[index];
    }
    else {
        return null;
    }
}
SecMasterJSCommon.SMSCommons.getAllChildNodes = function SecMasterJSCommon_SMSCommons$getAllChildNodes(element) {
    /// <param name="element" type="Object" domElement="true">
    /// </param>
    /// <returns type="Array"></returns>
    return SecMasterJSCommon.SMSCommons.createElement(element).children();
}
SecMasterJSCommon.SMSCommons.getGUID = function SecMasterJSCommon_SMSCommons$getGUID() {
    /// <returns type="String"></returns>
    var obj = null;
    eval('obj=GetGUID()');
    return obj;
}
SecMasterJSCommon.SMSCommons.openWindowDraftedSecurity = function SecMasterJSCommon_SMSCommons$openWindowDraftedSecurity(secId, isEditMode, effectiveDate, isActivateWhenSave, newWindow, isNewWindowMode, overrideIsActivateWhenSave, btnPageRefreshId, secIdForClone, deleteExistingSecurity, cloneTimeSeries, cuUniqueId) {
    /// <param name="secId" type="String">
    /// </param>
    /// <param name="isEditMode" type="Boolean">
    /// </param>
    /// <param name="effectiveDate" type="String">
    /// </param>
    /// <param name="isActivateWhenSave" type="Boolean">
    /// </param>
    /// <param name="newWindow" type="Boolean">
    /// </param>
    /// <param name="isNewWindowMode" type="Boolean">
    /// </param>
    /// <param name="overrideIsActivateWhenSave" type="Boolean">
    /// </param>
    /// <param name="btnPageRefreshId" type="String">
    /// </param>
    /// <param name="secIdForClone" type="String">
    /// </param>
    /// <param name="deleteExistingSecurity" type="Boolean">
    /// </param>
    /// <param name="cloneTimeSeries" type="Boolean">
    /// </param>
    /// <param name="cuUniqueId" type="String">
    /// </param>
    var newUniqueId = (newWindow) ? ('unique_' + SecMasterJSCommon.SMSCommons.getGUID()) : cuUniqueId;
    var draftType = (isEditMode) ? 'EditDraft' : 'ViewDraft';
    var effectiveDateQueryString = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
    if (effectiveDate != null && effectiveDate !== SecMasterJSCommon.SMSConstants.strinG_EMPTY) {
        effectiveDateQueryString = '&effectiveDate=' + effectiveDate;
    }
    var isActivate = false;
    if (isActivateWhenSave) {
        isActivate = true;
    }
    else {
        if (overrideIsActivateWhenSave) {
            isActivate = true;
        }
        else {
            isActivate = false;
        }
    }
    var url = 'SMCreateUpdateSecurityNew.aspx?identifier=CreateSecurityNew&secId=' + secId + '&pageType=' + draftType + effectiveDateQueryString + '&isActivateWhenSave=' + isActivate.toString() + '&btnPageRefreshId=' + btnPageRefreshId + ('&UniqueId=' + newUniqueId);
    if (secIdForClone != null && secIdForClone !== '') {
        url += '&secIdForClone=' + secIdForClone + '&deleteSecurity=' + deleteExistingSecurity + '&cloneTimeSeries=' + cloneTimeSeries;
    }
    var nw = url + '&mode=nw';
    if (newWindow) {
        SecMasterJSCommon.SMSCommons.createTab('CreateSecurityNew', nw, newUniqueId, '');
    }
    else {
        window.location.href = ((isNewWindowMode) ? nw : url);
    }
}
SecMasterJSCommon.SMSCommons.openWindowCloneSecurity = function SecMasterJSCommon_SMSCommons$openWindowCloneSecurity(secId, newWindow, isNewWindowMode, deleteSecurity, cloneTimeSeries, sectypeId, cuUniqueId) {
    /// <param name="secId" type="String">
    /// </param>
    /// <param name="newWindow" type="Boolean">
    /// </param>
    /// <param name="isNewWindowMode" type="Boolean">
    /// </param>
    /// <param name="deleteSecurity" type="Boolean">
    /// </param>
    /// <param name="cloneTimeSeries" type="Boolean">
    /// </param>
    /// <param name="sectypeId" type="Number" integer="true">
    /// </param>
    /// <param name="cuUniqueId" type="String">
    /// </param>
    var newUniqueId = (newWindow) ? ('unique_' + SecMasterJSCommon.SMSCommons.getGUID()) : cuUniqueId;
    var url = 'SMCreateUpdateSecurityNew.aspx?identifier=CreateSecurityNew&SecIdForClone=' + secId + '&pageType=Clone&deleteSecurity=' + deleteSecurity + '&cloneTimeSeries=' + cloneTimeSeries + '&sectypeId=' + sectypeId + ('&UniqueId=' + newUniqueId);
    var nw = url + '&mode=nw';
    if (newWindow) {
        SecMasterJSCommon.SMSCommons.createTab('CreateSecurityNew', nw, newUniqueId, '');
    }
    else {
        window.location.href = ((isNewWindowMode) ? nw : url);
    }
}
SecMasterJSCommon.SMSCommons.openWindowAsOfSecurity = function SecMasterJSCommon_SMSCommons$openWindowAsOfSecurity(secId, date, knowledgeDate, cuUniqueId) {
    /// <param name="secId" type="String">
    /// </param>
    /// <param name="date" type="String">
    /// </param>
    /// <param name="knowledgeDate" type="String">
    /// </param>
    /// <param name="cuUniqueId" type="String">
    /// </param>
    var newUniqueId = (cuUniqueId == null || cuUniqueId === '') ? ('unique_' + SecMasterJSCommon.SMSCommons.getGUID()) : cuUniqueId;
    var url = 'SMCreateUpdateSecurityNew.aspx?identifier=UpdateSecurityNew&pageType=AsOf&mode=nw&secId=' + secId + '&effectiveDate=' + date + '&knowledgeDate=' + knowledgeDate + ('&UniqueId=' + newUniqueId);
    SecMasterJSCommon.SMSCommons.createTab('UpdateSecurityNew', url, newUniqueId, '');
}
SecMasterJSCommon.SMSCommons.openWindowForSecurityWithHiglight = function SecMasterJSCommon_SMSCommons$openWindowForSecurityWithHiglight(isEditMode, secId, AttrIdForHighlight, MultiInfoIdForHighlight, newWindow, isNewWindowMode, openByDefault, externalVendorId, cuUniqueId) {
    /// <param name="isEditMode" type="Boolean">
    /// </param>
    /// <param name="secId" type="String">
    /// </param>
    /// <param name="AttrIdForHighlight" type="Number" integer="true">
    /// </param>
    /// <param name="MultiInfoIdForHighlight" type="Number" integer="true">
    /// </param>
    /// <param name="newWindow" type="Boolean">
    /// </param>
    /// <param name="isNewWindowMode" type="Boolean">
    /// </param>
    /// <param name="openByDefault" type="SecMasterJSCommon.SMCreateUpdateOpenByDefault">
    /// </param>
    /// <param name="externalVendorId" type="Number" integer="true">
    /// </param>
    /// <param name="cuUniqueId" type="String">
    /// </param>
    var newUniqueId = (newWindow) ? ('unique_' + SecMasterJSCommon.SMSCommons.getGUID()) : cuUniqueId;
    var openByDefaultUrl = '&OpenByDefault=' + openByDefault;
    switch (openByDefault) {
        case SecMasterJSCommon.SMCreateUpdateOpenByDefault.None:
            openByDefaultUrl = '';
            break;
        case SecMasterJSCommon.SMCreateUpdateOpenByDefault.FTP:
            openByDefaultUrl += '&ExternalVendorId=' + externalVendorId;
            break;
        case SecMasterJSCommon.SMCreateUpdateOpenByDefault.Audit:
            break;
        case SecMasterJSCommon.SMCreateUpdateOpenByDefault.Hierarchy:
            break;
        case SecMasterJSCommon.SMCreateUpdateOpenByDefault.CorpAction:
            break;
        case SecMasterJSCommon.SMCreateUpdateOpenByDefault.VendorData:
            break;
    }
    var url = 'SMCreateUpdateSecurityNew.aspx?identifier=UpdateSecurityNew&pageType=' + ((isEditMode) ? 'Edit' : 'View') + '&secId=' + secId + (((AttrIdForHighlight !== 0 && AttrIdForHighlight !== -1) && (MultiInfoIdForHighlight === 0 || MultiInfoIdForHighlight === -1)) ? '&AttrIdForHighlight=' + AttrIdForHighlight : '') + (((MultiInfoIdForHighlight !== 0 && MultiInfoIdForHighlight !== -1) && (AttrIdForHighlight === 0 || AttrIdForHighlight === -1)) ? '&MultiInfoIdForHighlight=' + MultiInfoIdForHighlight : '') + ('&UniqueId=' + newUniqueId) + openByDefaultUrl;
    var nw = url + '&mode=nw';
    if (newWindow) {
        SecMasterJSCommon.SMSCommons.createTab('UpdateSecurityNew', nw, newUniqueId, '');
    }
    else {
        window.location.href = ((isNewWindowMode) ? nw : url);
    }
}
SecMasterJSCommon.SMSCommons.openWindowForCreateSecurity = function SecMasterJSCommon_SMSCommons$openWindowForCreateSecurity(sectypeId, newWindow, isNewWindowMode, cuUniqueId) {
    /// <param name="sectypeId" type="Number" integer="true">
    /// </param>
    /// <param name="newWindow" type="Boolean">
    /// </param>
    /// <param name="isNewWindowMode" type="Boolean">
    /// </param>
    /// <param name="cuUniqueId" type="String">
    /// </param>
    var newUniqueId = (newWindow) ? ('unique_' + SecMasterJSCommon.SMSCommons.getGUID()) : cuUniqueId;
    var url = 'SMCreateUpdateSecurityNew.aspx?identifier=CreateSecurityNew&sectypeId=' + sectypeId + ('&UniqueId=' + newUniqueId);
    var nw = url + '&mode=nw';
    if (newWindow) {
        SecMasterJSCommon.SMSCommons.createTab('CreateSecurityNew', nw, newUniqueId, '');
    }
    else {
        window.location.href = ((isNewWindowMode) ? nw : url);
    }
}
SecMasterJSCommon.SMSCommons.openSecurity = function SecMasterJSCommon_SMSCommons$openSecurity(isEditMode, secId, effectiveDate, newWindow, isNewWindowMode, overrideIsActivateWhenSave, btnPageRefreshId, secIdForClone, deleteExistingSecurity, cloneTimeSeries, openByDefault, externalVendorId, cuUniqueId) {
    /// <param name="isEditMode" type="Boolean">
    /// </param>
    /// <param name="secId" type="String">
    /// </param>
    /// <param name="effectiveDate" type="String">
    /// </param>
    /// <param name="newWindow" type="Boolean">
    /// </param>
    /// <param name="isNewWindowMode" type="Boolean">
    /// </param>
    /// <param name="overrideIsActivateWhenSave" type="Boolean">
    /// </param>
    /// <param name="btnPageRefreshId" type="String">
    /// </param>
    /// <param name="secIdForClone" type="String">
    /// </param>
    /// <param name="deleteExistingSecurity" type="Boolean">
    /// </param>
    /// <param name="cloneTimeSeries" type="Boolean">
    /// </param>
    /// <param name="openByDefault" type="SecMasterJSCommon.SMCreateUpdateOpenByDefault">
    /// </param>
    /// <param name="externalVendorId" type="Number" integer="true">
    /// </param>
    /// <param name="cuUniqueId" type="String">
    /// </param>
    var dict = {};
    var list = [];
    list[0] = secId;
    eval('dict = ExecuteSynchronously(\'./BaseUserControls/Service/Service.asmx\', \'GetSecuritiesActiveState\', { lstSecId: ' + Sys.Serialization.JavaScriptSerializer.serialize(list) + ' }).d;');
    eval('for(var item in dict){ if (dict[item].Is_Active) { SecMasterJSCommon.SMSCommons.openWindowForSecurity(isEditMode, item, newWindow, isNewWindowMode, openByDefault, externalVendorId, cuUniqueId);} else{SecMasterJSCommon.SMSCommons.openWindowDraftedSecurity(item, isEditMode, ((dict[item].Effective_Date != null && dict[item].Effective_Date != \'\') ? dict[item].Effective_Date : effectiveDate), dict[item].Is_Draft, newWindow, isNewWindowMode, overrideIsActivateWhenSave, btnPageRefreshId, secIdForClone, deleteExistingSecurity, cloneTimeSeries, cuUniqueId);}}');
}
SecMasterJSCommon.SMSCommons.openWindowForSecurity = function SecMasterJSCommon_SMSCommons$openWindowForSecurity(isEditMode, secId, newWindow, isNewWindowMode, openByDefault, externalVendorId, cuUniqueId) {
    /// <param name="isEditMode" type="Boolean">
    /// </param>
    /// <param name="secId" type="String">
    /// </param>
    /// <param name="newWindow" type="Boolean">
    /// </param>
    /// <param name="isNewWindowMode" type="Boolean">
    /// </param>
    /// <param name="openByDefault" type="SecMasterJSCommon.SMCreateUpdateOpenByDefault">
    /// </param>
    /// <param name="externalVendorId" type="Number" integer="true">
    /// </param>
    /// <param name="cuUniqueId" type="String">
    /// </param>
    SecMasterJSCommon.SMSCommons.openWindowForSecurityWithHiglight(isEditMode, secId, 0, 0, newWindow, isNewWindowMode, openByDefault, externalVendorId, cuUniqueId);
}
SecMasterJSCommon.SMSCommons.openWindowToCreateDerivative = function SecMasterJSCommon_SMSCommons$openWindowToCreateDerivative(securityId, securityTypeId, newWindow, isNewWindowMode, cuUniqueId) {
    /// <param name="securityId" type="String">
    /// </param>
    /// <param name="securityTypeId" type="String">
    /// </param>
    /// <param name="newWindow" type="Boolean">
    /// </param>
    /// <param name="isNewWindowMode" type="Boolean">
    /// </param>
    /// <param name="cuUniqueId" type="String">
    /// </param>
    var newUniqueId = (newWindow) ? ('unique_' + SecMasterJSCommon.SMSCommons.getGUID()) : cuUniqueId;
    var url = String.format('SMCreateUpdateSecurityNew.aspx?identifier=CreateSecurityNew&pageType=Derivative&SecIdForDerivative={0}&sectypeId={1}', securityId, securityTypeId) + ('&UniqueId=' + newUniqueId);
    var nw = url + '&mode=nw';
    if (newWindow) {
        SecMasterJSCommon.SMSCommons.createTab('CreateSecurityNew', nw, newUniqueId, '');
    }
    else {
        window.location.href = ((isNewWindowMode) ? nw : url);
    }
}
SecMasterJSCommon.SMSCommons.createTab = function SecMasterJSCommon_SMSCommons$createTab(identifier, url, uniqueId, tabText) {
    /// <param name="identifier" type="String">
    /// </param>
    /// <param name="url" type="String">
    /// </param>
    /// <param name="uniqueId" type="String">
    /// </param>
    /// <param name="tabText" type="String">
    /// </param>
    eval('var parent = getTopMostParent(); parent.leftMenu.createTab(\'' + identifier + '\',\'' + url + '\',\'' + uniqueId + '\',\'' + tabText + '\');');
}
SecMasterJSCommon.SMSCommons.getRowForXlGrid = function SecMasterJSCommon_SMSCommons$getRowForXlGrid(gridId, contextMenuId) {
    /// <param name="gridId" type="String">
    /// </param>
    /// <param name="contextMenuId" type="String">
    /// </param>
    /// <returns type="Object" domElement="true"></returns>
    var rowIndex = Number.parseInvariant((document.getElementById(contextMenuId + '___rowId')).value);
    var grid = null;
    grid = document.getElementById(gridId + '_bodyDiv_Table');
    var row = grid.rows[rowIndex];
    return row;
}
SecMasterJSCommon.SMSCommons.getCrossBrowserEventObjectAndTarget = function SecMasterJSCommon_SMSCommons$getCrossBrowserEventObjectAndTarget(objEvent) {
    /// <param name="objEvent" type="Object">
    /// </param>
    /// <returns type="SecMasterJSCommon.CrossBrowserEventAndTarget"></returns>
    var objCrossBrowset = new SecMasterJSCommon.CrossBrowserEventAndTarget();
    objCrossBrowset.objEvent = objEvent;
    objCrossBrowset.target = null;
    eval('objCrossBrowset.target= objCrossBrowset.objEvent.target|| objCrossBrowset.objEvent.srcElement|| objCrossBrowset.objEvent.originalTarget');
    return objCrossBrowset;
}
SecMasterJSCommon.SMSCommons.outerHTML = function SecMasterJSCommon_SMSCommons$outerHTML(element) {
    /// <param name="element" type="Object" domElement="true">
    /// </param>
    /// <returns type="String"></returns>
    var outerhtml = '';
    eval('outerhtml = outerHTML(element)');
    return outerhtml;
}
SecMasterJSCommon.SMSCommons.innerHTML = function SecMasterJSCommon_SMSCommons$innerHTML(element) {
    /// <param name="element" type="Object" domElement="true">
    /// </param>
    /// <returns type="String"></returns>
    var innerhtml = '';
    eval('innerhtml = innerHTML(element)');
    return innerhtml;
}
SecMasterJSCommon.SMSCommons.convertToCurrencyFormat = function SecMasterJSCommon_SMSCommons$convertToCurrencyFormat(input) {
    /// <param name="input" type="String">
    /// </param>
    /// <returns type="String"></returns>
    var output = '';
    eval('output = ConvertToCurrencyFormat(input)');
    return output;
}
SecMasterJSCommon.SMSCommons.convertCurrencyToISOFormat = function SecMasterJSCommon_SMSCommons$convertCurrencyToISOFormat(input) {
    /// <param name="input" type="String">
    /// </param>
    /// <returns type="String"></returns>
    var output = '';
    eval('output = ConvertCurrencyToISOFormat(input)');
    return output;
}
SecMasterJSCommon.SMSCommons.getMarketSector = function SecMasterJSCommon_SMSCommons$getMarketSector(value) {
    /// <param name="value" type="String">
    /// </param>
    /// <returns type="SecMasterJSCommon.IdentifierSection"></returns>
    var marketSector = '';
    var arr = value.split(' ');
    if (arr != null && arr.length > 0) {
        switch (arr[arr.length - 1].toLowerCase()) {
            case 'comdty':
                marketSector = 'Comdty';
                break;
            case 'corp':
                marketSector = 'Corp';
                break;
            case 'curncy':
                marketSector = 'Curncy';
                break;
            case 'equity':
                marketSector = 'Equity';
                break;
            case 'govt':
                marketSector = 'Govt';
                break;
            case 'index':
                marketSector = 'Index';
                break;
            case 'mmkt':
                marketSector = 'MMkt';
                break;
            case 'mtge':
                marketSector = 'Mtge';
                break;
            case 'muni':
                marketSector = 'Muni';
                break;
            case 'pfd':
                marketSector = 'Pfd';
                break;
        }
    }
    var obj = new SecMasterJSCommon.IdentifierSection();
    obj.MarketSector = marketSector;
    if (marketSector !== '') {
        obj.IdentifierValue = value.substring(0, value.toLowerCase().indexOf(marketSector.toLowerCase()) - 1).trim().toUpperCase();
    }
    else {
        obj.IdentifierValue = value.trim();
    }
    return obj;
}
SecMasterJSCommon.SMSCommons.executeSynchronously = function SecMasterJSCommon_SMSCommons$executeSynchronously(url, method, args) {
    /// <param name="url" type="String">
    /// </param>
    /// <param name="method" type="String">
    /// </param>
    /// <param name="args" type="Object">
    /// </param>
    /// <returns type="Object"></returns>
    var result = null;
    eval('result = ExecuteSynchronously(url, method, args).d;');
    return result;
}
SecMasterJSCommon.SMSCommons.disableDivs = function SecMasterJSCommon_SMSCommons$disableDivs(zIndex) {
    /// <param name="zIndex" type="Number" integer="true">
    /// </param>
    eval('disableDivs(zIndex);');
}
SecMasterJSCommon.SMSCommons.enableDivs = function SecMasterJSCommon_SMSCommons$enableDivs() {
    eval('enableDivs();');
}
SecMasterJSCommon.SMSCommons.setIframeSrc = function SecMasterJSCommon_SMSCommons$setIframeSrc(Id, Src) {
    /// <param name="Id" type="String">
    /// </param>
    /// <param name="Src" type="String">
    /// </param>
    eval('SetIframeSrc(\'' + Id + '\',\'' + Src + '\')');
}


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.IdentifierSection

SecMasterJSCommon.IdentifierSection = function SecMasterJSCommon_IdentifierSection() {
    /// <field name="MarketSector" type="String">
    /// </field>
    /// <field name="IdentifierValue" type="String">
    /// </field>
}
SecMasterJSCommon.IdentifierSection.prototype = {
    MarketSector: null,
    IdentifierValue: null
}


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.MyDictionaryElement

SecMasterJSCommon.MyDictionaryElement = function SecMasterJSCommon_MyDictionaryElement() {
    /// <field name="Value" type="String">
    /// </field>
    /// <field name="Key" type="String">
    /// </field>
}
SecMasterJSCommon.MyDictionaryElement.prototype = {
    Value: null,
    Key: null
}


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.SMSDictionary

SecMasterJSCommon.SMSDictionary = function SecMasterJSCommon_SMSDictionary() {
    /// <field name="_smsDictionaryObject" type="Array">
    /// </field>
    this._smsDictionaryObject = [];
}
SecMasterJSCommon.SMSDictionary.getDictFromDelimitedText = function SecMasterJSCommon_SMSDictionary$getDictFromDelimitedText(delimitedText, keyValueSeparator, keyValuePairSeparator) {
    /// <param name="delimitedText" type="String">
    /// </param>
    /// <param name="keyValueSeparator" type="String">
    /// </param>
    /// <param name="keyValuePairSeparator" type="String">
    /// </param>
    /// <returns type="SecMasterJSCommon.SMSDictionary"></returns>
    var dict = new SecMasterJSCommon.SMSDictionary();
    var dictKeyValues = delimitedText.split(keyValuePairSeparator);
    var $enum1 = dictKeyValues.getEnumerator();
    while ($enum1.moveNext()) {
        var keyValuePairString = $enum1.get_current();
        if (keyValuePairString !== '') {
            var keyValuePair = keyValuePairString.split(keyValueSeparator);
            if (keyValuePair.length === 2) {
                dict.set_item(keyValuePair[0], keyValuePair[1]);
            }
        }
    }
    return dict;
}
SecMasterJSCommon.SMSDictionary.prototype = {
    _smsDictionaryObject: null,
    
    get_SMSDictionaryObject: function SecMasterJSCommon_SMSDictionary$get_SMSDictionaryObject() {
        /// <summary>
        /// Gets or sets the SMS dictionary object.
        /// </summary>
        /// <value type="Array"></value>
        return this._smsDictionaryObject;
    },
    set_SMSDictionaryObject: function SecMasterJSCommon_SMSDictionary$set_SMSDictionaryObject(value) {
        /// <summary>
        /// Gets or sets the SMS dictionary object.
        /// </summary>
        /// <value type="Array"></value>
        this._smsDictionaryObject = value;
        return value;
    },
    
    findIndex: function SecMasterJSCommon_SMSDictionary$findIndex(key, isCaseSensitive) {
        /// <summary>
        /// Finds the index of the key in the dictionary.
        /// </summary>
        /// <param name="key" type="String">
        /// The key to search.
        /// </param>
        /// <param name="isCaseSensitive" type="Boolean">
        /// if set to <c>true</c> case sensitive search is performed.
        /// </param>
        /// <returns type="Number" integer="true"></returns>
        var i = -1;
        if (this.get_SMSDictionaryObject() != null && this.get_SMSDictionaryObject().length > 0) {
            for (var j = 0; j < this.get_SMSDictionaryObject().length; j++) {
                if (isCaseSensitive) {
                    if ((this.get_SMSDictionaryObject()[j]).Key.trim() === key.trim()) {
                        i = j;
                    }
                }
                else {
                    if ((this.get_SMSDictionaryObject()[j]).Key.toLowerCase().trim() === key.toLowerCase().trim()) {
                        i = j;
                    }
                }
            }
        }
        return i;
    },
    
    findIndexByValue: function SecMasterJSCommon_SMSDictionary$findIndexByValue(value, isCaseSensitive) {
        /// <summary>
        /// Finds the index by value in the dictionary.
        /// </summary>
        /// <param name="value" type="String">
        /// The value to search.
        /// </param>
        /// <param name="isCaseSensitive" type="Boolean">
        /// if set to <c>true</c> case sensitive search is performed.
        /// </param>
        /// <returns type="Number" integer="true"></returns>
        var i = -1;
        if (this.get_SMSDictionaryObject() != null && this.get_SMSDictionaryObject().length > 0) {
            for (var j = 0; j < this.get_SMSDictionaryObject().length; j++) {
                if (isCaseSensitive) {
                    if ((this.get_SMSDictionaryObject()[j]).Value.trim() === value.trim()) {
                        i = j;
                    }
                }
                else {
                    if ((this.get_SMSDictionaryObject()[j]).Value.toLowerCase().trim() === value.toLowerCase().trim()) {
                        i = j;
                    }
                }
            }
        }
        return i;
    },
    
    add: function SecMasterJSCommon_SMSDictionary$add(key, value) {
        /// <summary>
        /// Adds the specified key-value pair in the dictionary.
        /// </summary>
        /// <param name="key" type="String">
        /// The key.
        /// </param>
        /// <param name="value" type="String">
        /// The value.
        /// </param>
        if (this.get_SMSDictionaryObject() == null) {
            this.set_SMSDictionaryObject([]);
        }
        Array.add(this.get_SMSDictionaryObject(), SecMasterJSCommon.CreateCustomObject.createMyDictionaryElement(key, value));
    },
    
    get_length: function SecMasterJSCommon_SMSDictionary$get_length() {
        /// <summary>
        /// Gets the length of the dictionary.
        /// </summary>
        /// <value type="Number" integer="true"></value>
        if (this.get_SMSDictionaryObject() != null) {
            return this.get_SMSDictionaryObject().length;
        }
        else {
            return 0;
        }
    },
    
    remove: function SecMasterJSCommon_SMSDictionary$remove(key) {
        /// <summary>
        /// Removes the MyDictionaryElement with the specified key.
        /// </summary>
        /// <param name="key" type="String">
        /// The key.
        /// </param>
        if (this.get_SMSDictionaryObject() != null) {
            if (this.findIndex(key, true) !== -1) {
                Array.removeAt(this.get_SMSDictionaryObject(), this.findIndex(key, true));
            }
        }
    },
    
    removeByValue: function SecMasterJSCommon_SMSDictionary$removeByValue(value) {
        /// <summary>
        /// Removes the MyDictionaryElement with the specified value.
        /// </summary>
        /// <param name="value" type="String">
        /// The value.
        /// </param>
        if (this.get_SMSDictionaryObject() != null) {
            if (this.findIndexByValue(value, true) !== -1) {
                Array.removeAt(this.get_SMSDictionaryObject(), this.findIndexByValue(value, true));
            }
        }
    },
    
    clone: function SecMasterJSCommon_SMSDictionary$clone() {
        /// <summary>
        /// Clones this instance of dictionary.
        /// </summary>
        /// <returns type="SecMasterJSCommon.SMSDictionary"></returns>
        if (this.get_SMSDictionaryObject() != null) {
            return SecMasterJSCommon.CreateCustomObject.createSMSDictionary(this.get_SMSDictionaryObject());
        }
        else {
            return new SecMasterJSCommon.SMSDictionary();
        }
    },
    
    containsKey: function SecMasterJSCommon_SMSDictionary$containsKey(key) {
        /// <summary>
        /// Determines whether dictionary contains a MyDictionaryElement with the specified key.
        /// </summary>
        /// <param name="key" type="String">
        /// The key.
        /// </param>
        /// <returns type="Boolean"></returns>
        if (this.get_SMSDictionaryObject() != null && this.get_SMSDictionaryObject().length > 0) {
            if (this.findIndex(key, true) > -1) {
                return true;
            }
        }
        return false;
    },
    
    containsValue: function SecMasterJSCommon_SMSDictionary$containsValue(value) {
        /// <summary>
        /// Determines whether dictionary contains a MyDictionaryElement with the specified value.
        /// </summary>
        /// <param name="value" type="String">
        /// The value.
        /// </param>
        /// <returns type="Boolean"></returns>
        if (this.get_SMSDictionaryObject() != null && this.get_SMSDictionaryObject().length > 0) {
            if (this.findIndexByValue(value, true) > 0) {
                return true;
            }
        }
        return false;
    },
    
    get_keys: function SecMasterJSCommon_SMSDictionary$get_keys() {
        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value type="Array"></value>
        var list = [];
        if (this.get_SMSDictionaryObject() != null) {
            for (var j = 0; j < this.get_SMSDictionaryObject().length; j++) {
                Array.add(list, (this.get_SMSDictionaryObject()[j]).Key);
            }
        }
        return list;
    },
    
    getEnumerator: function SecMasterJSCommon_SMSDictionary$getEnumerator() {
        /// <returns type="SecMasterJSCommon.ObjectEnum"></returns>
        return new SecMasterJSCommon.ObjectEnum(this.get_keys());
    },
    
    toDelimitedText: function SecMasterJSCommon_SMSDictionary$toDelimitedText(keyValueSeparator, keyValuePairSeparator) {
        /// <param name="keyValueSeparator" type="String">
        /// </param>
        /// <param name="keyValuePairSeparator" type="String">
        /// </param>
        /// <returns type="String"></returns>
        var textBuilder = new Sys.StringBuilder();
        var $enum1 = this.getEnumerator();
        while ($enum1.moveNext()) {
            var key = $enum1.get_current();
            textBuilder.append(key);
            textBuilder.append(keyValueSeparator);
            textBuilder.append(this.get_item(key));
            textBuilder.append(keyValuePairSeparator);
        }
        return textBuilder.toString();
    },
    get_item: function SecMasterJSCommon_SMSDictionary$get_item(key) {
        /// <summary>
        /// Gets or sets the value of the MyDictionaryElement with the specified key.
        /// </summary>
        /// <param name="key" type="String">
        /// </param>
        /// <param name="value" type="String">
        /// </param>
        /// <returns type="String"></returns>
        if (this.get_SMSDictionaryObject() != null && this.get_SMSDictionaryObject().length > 0) {
            var index = this.findIndex(key, true);
            if (index >= 0) {
                return (this.get_SMSDictionaryObject()[index]).Value;
            }
            else {
                return SecMasterJSCommon.SMSConstants.strinG_EMPTY;
            }
        }
        else {
            return SecMasterJSCommon.SMSConstants.strinG_EMPTY;
        }
    },
    set_item: function SecMasterJSCommon_SMSDictionary$set_item(key, value) {
        /// <summary>
        /// Gets or sets the value of the MyDictionaryElement with the specified key.
        /// </summary>
        /// <param name="key" type="String">
        /// </param>
        /// <param name="value" type="String">
        /// </param>
        /// <returns type="String"></returns>
        if (this.get_SMSDictionaryObject() != null) {
            var index = this.findIndex(key, true);
            if (index >= 0) {
                (this.get_SMSDictionaryObject()[index]).Value = value;
            }
            else {
                this.add(key, value);
            }
        }
        return value;
    }
}


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.ObjectEnum

SecMasterJSCommon.ObjectEnum = function SecMasterJSCommon_ObjectEnum(list) {
    /// <param name="list" type="Array">
    /// </param>
    /// <field name="_objects" type="Array">
    /// </field>
    /// <field name="_position" type="Number" integer="true">
    /// </field>
    this._position = -1;
    this._objects = list;
}
SecMasterJSCommon.ObjectEnum.prototype = {
    _objects: null,
    
    moveNext: function SecMasterJSCommon_ObjectEnum$moveNext() {
        /// <returns type="Boolean"></returns>
        this._position++;
        return (this._position < this._objects.length);
    },
    
    reset: function SecMasterJSCommon_ObjectEnum$reset() {
        this._position = -1;
    },
    
    get_current: function SecMasterJSCommon_ObjectEnum$get_current() {
        /// <value type="Object"></value>
        if (this._position !== -1 && this._position < this._objects.length) {
            return this._objects[this._position];
        }
        else {
            return null;
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.CreateCustomObject

SecMasterJSCommon.CreateCustomObject = function SecMasterJSCommon_CreateCustomObject() {
}
SecMasterJSCommon.CreateCustomObject.createMyDictionaryElement = function SecMasterJSCommon_CreateCustomObject$createMyDictionaryElement(key, value) {
    /// <summary>
    /// Creates a new instance of MyDictionaryElement.
    /// </summary>
    /// <param name="key" type="String">
    /// The key.
    /// </param>
    /// <param name="value" type="String">
    /// The value.
    /// </param>
    /// <returns type="SecMasterJSCommon.MyDictionaryElement"></returns>
    var dictElement = new SecMasterJSCommon.MyDictionaryElement();
    dictElement.Key = key;
    dictElement.Value = value;
    return dictElement;
}
SecMasterJSCommon.CreateCustomObject.createSMSDictionary = function SecMasterJSCommon_CreateCustomObject$createSMSDictionary(arrayList) {
    /// <summary>
    /// Creates a new instance of SMSDictionary.
    /// </summary>
    /// <param name="arrayList" type="Object">
    /// The array list.
    /// </param>
    /// <returns type="SecMasterJSCommon.SMSDictionary"></returns>
    var dic = new SecMasterJSCommon.SMSDictionary();
    dic.set_SMSDictionaryObject(arrayList);
    return dic;
}


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.SMSConstants

SecMasterJSCommon.SMSConstants = function SecMasterJSCommon_SMSConstants() {
    /// <field name="strinG_VISIBLE" type="String" static="true">
    /// Value = "visible"
    /// </field>
    /// <field name="strinG_HIDDEN" type="String" static="true">
    /// Value = "hidden"
    /// </field>
    /// <field name="strinG_NONE" type="String" static="true">
    /// Value = "none"
    /// </field>
    /// <field name="strinG_EMPTY" type="String" static="true">
    /// Value = ""
    /// </field>
    /// <field name="clasS_NORMAL_ROW" type="String" static="true">
    /// Value = "normalRow"
    /// </field>
    /// <field name="clasS_ALTERNATING_ROW" type="String" static="true">
    /// Value = "alternatingRow"
    /// </field>
    /// <field name="clasS_CHECKED_ROW" type="String" static="true">
    /// Value = "checkedRow"
    /// </field>
    /// <field name="clasS_GROUPED_ROW" type="String" static="true">
    /// Value = "groupedRow"
    /// </field>
    /// <field name="clasS_GROUP_FOOTER" type="String" static="true">
    /// Value = "groupFooter"
    /// </field>
    /// <field name="clasS_HEADER" type="String" static="true">
    /// Value = "header"
    /// </field>
    /// <field name="clasS_UPDATE_CLIENT_BTN" type="String" static="true">
    /// Value = "updateClientBtn"
    /// </field>
    /// <field name="clasS_DELETE_CLIENT_BTN" type="String" static="true">
    /// Value = "deleteClientBtn"
    /// </field>
    /// <field name="clasS_BTN_UPDATE" type="String" static="true">
    /// Value = "btnUpdate"
    /// </field>
    /// <field name="clasS_BTN_DELETE" type="String" static="true">
    /// Value = "btnDelete"
    /// </field>
    /// <field name="selecT_ONE_TEXT" type="String" static="true">
    /// Value = "--Select One--"
    /// </field>
    /// <field name="selecT_ONE_VALUE" type="String" static="true">
    /// Value = "-1"
    /// </field>
    /// <field name="htmL_INPUT" type="String" static="true">
    /// Value = "input"
    /// </field>
    /// <field name="htmL_INPUT_BUTTON" type="String" static="true">
    /// Value = "button"
    /// </field>
    /// <field name="htmL_INPUT_CHECKBOX" type="String" static="true">
    /// Value = "checkbox"
    /// </field>
    /// <field name="htmL_INPUT_FILE" type="String" static="true">
    /// Value = "file"
    /// </field>
    /// <field name="htmL_INPUT_IMAGE" type="String" static="true">
    /// Value = "image"
    /// </field>
    /// <field name="htmL_INPUT_PASSWORD" type="String" static="true">
    /// Value = "password"
    /// </field>
    /// <field name="htmL_INPUT_RADIO" type="String" static="true">
    /// Value = "radio"
    /// </field>
    /// <field name="htmL_INPUT_RESET" type="String" static="true">
    /// Value = "reset"
    /// </field>
    /// <field name="htmL_INPUT_SUBMIT" type="String" static="true">
    /// Value = "submit"
    /// </field>
    /// <field name="htmL_INPUT_TEXT" type="String" static="true">
    /// Value = "text"
    /// </field>
    /// <field name="htmL_INPUT_HIDDEN" type="String" static="true">
    /// Value = "hidden"
    /// </field>
    /// <field name="htmL_SELECT" type="String" static="true">
    /// Value = "select"
    /// </field>
    /// <field name="htmL_OPTION" type="String" static="true">
    /// Value = "option"
    /// </field>
    /// <field name="htmL_TABLE" type="String" static="true">
    /// Value = "table"
    /// </field>
}


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.CheckBoxList

SecMasterJSCommon.CheckBoxList = function SecMasterJSCommon_CheckBoxList(chkBox, direction) {
    /// <param name="chkBox" type="Object" domElement="true">
    /// </param>
    /// <param name="direction" type="SecMasterJSCommon.RepeatDirection">
    /// </param>
    /// <field name="_chkBox" type="Object" domElement="true">
    /// </field>
    /// <field name="_length" type="Number" integer="true">
    /// </field>
    /// <field name="_selectedCount" type="Number" integer="true">
    /// </field>
    /// <field name="_selectedValues" type="Array">
    /// </field>
    /// <field name="_chkBoxList" type="Array">
    /// </field>
    this._chkBox = chkBox;
    if (direction === SecMasterJSCommon.RepeatDirection.vertical) {
        this._length = (this._chkBox).rows.length;
    }
    else {
        this._length = ((this._chkBox).rows[0]).cells.length;
    }
    this._chkBoxList = [];
    if (this._chkBox != null) {
        for (var i = 0; i < this._length; i++) {
            this._chkBoxList[i] = com.ivp.rad.rscriptutils.RSValidators.getObject(this._chkBox.id + '_' + i);
        }
    }
}
SecMasterJSCommon.CheckBoxList.prototype = {
    _chkBox: null,
    _length: 0,
    
    get_length: function SecMasterJSCommon_CheckBoxList$get_length() {
        /// <value type="Number" integer="true"></value>
        return this._length;
    },
    
    _selectedCount: 0,
    
    get_selectedCount: function SecMasterJSCommon_CheckBoxList$get_selectedCount() {
        /// <value type="Number" integer="true"></value>
        this._selectedCount = 0;
        for (var i = 0; i < this._length; i++) {
            if ((this._chkBoxList[i]).checked) {
                this._selectedCount++;
            }
        }
        return this._selectedCount;
    },
    
    _selectedValues: null,
    
    get_selectedValues: function SecMasterJSCommon_CheckBoxList$get_selectedValues() {
        /// <value type="Array"></value>
        var j = 0;
        this._selectedValues = [];
        for (var i = 0; i < this._length; i++) {
            if ((this._chkBoxList[i]).checked) {
                this._selectedValues[j] = (this._chkBoxList[i]).value;
                j++;
            }
        }
        return this._selectedValues;
    },
    
    clearSelected: function SecMasterJSCommon_CheckBoxList$clearSelected() {
        for (var i = 0; i < this._length; i++) {
            (this._chkBoxList[i]).checked = false;
        }
    },
    
    _chkBoxList: null,
    get_item: function SecMasterJSCommon_CheckBoxList$get_item(index) {
        /// <param name="index" type="Number" integer="true">
        /// </param>
        /// <param name="value" type="Object" domElement="true">
        /// </param>
        /// <returns type="Object" domElement="true"></returns>
        return this._chkBoxList[index];
    }
}


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.KeyValuePair

SecMasterJSCommon.KeyValuePair = function SecMasterJSCommon_KeyValuePair() {
    /// <field name="Key" type="String">
    /// </field>
    /// <field name="Value" type="String">
    /// </field>
}
SecMasterJSCommon.KeyValuePair.prototype = {
    Key: null,
    Value: null
}


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.CrossBrowserEventAndTarget

SecMasterJSCommon.CrossBrowserEventAndTarget = function SecMasterJSCommon_CrossBrowserEventAndTarget() {
    /// <field name="objEvent" type="EventWrapper">
    /// </field>
    /// <field name="target" type="Object" domElement="true">
    /// </field>
}
SecMasterJSCommon.CrossBrowserEventAndTarget.prototype = {
    objEvent: null,
    target: null
}


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.SMSSectypeRuleAttributeInfo

SecMasterJSCommon.SMSSectypeRuleAttributeInfo = function SecMasterJSCommon_SMSSectypeRuleAttributeInfo() {
    /// <field name="attributeId" type="Number" integer="true">
    /// </field>
    /// <field name="attributeName" type="String">
    /// </field>
}
SecMasterJSCommon.SMSSectypeRuleAttributeInfo.prototype = {
    attributeId: 0,
    attributeName: null
}


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.SMSecurityTimeSeries

SecMasterJSCommon.SMSecurityTimeSeries = function SecMasterJSCommon_SMSecurityTimeSeries() {
    /// <field name="startDate" type="String">
    /// </field>
    /// <field name="endDate" type="String">
    /// </field>
    /// <field name="attrIdList" type="Array" elementType="String">
    /// </field>
    /// <field name="attrValueList" type="Array" elementType="String">
    /// </field>
    /// <field name="secId" type="String">
    /// </field>
    /// <field name="primaryAttrValue" type="String">
    /// </field>
}
SecMasterJSCommon.SMSecurityTimeSeries.prototype = {
    startDate: null,
    endDate: null,
    attrIdList: null,
    attrValueList: null,
    secId: null,
    primaryAttrValue: null
}


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.SMSecurityTimeSeriesMultiSec

SecMasterJSCommon.SMSecurityTimeSeriesMultiSec = function SecMasterJSCommon_SMSecurityTimeSeriesMultiSec() {
    /// <field name="startDate" type="String">
    /// </field>
    /// <field name="endDate" type="String">
    /// </field>
    /// <field name="attrIdListMaster" type="Array" elementType="String">
    /// </field>
    /// <field name="attrIdListSingle" type="Array" elementType="String">
    /// </field>
    /// <field name="basketListMulti" type="Array" elementType="String">
    /// </field>
    /// <field name="secId" type="String">
    /// </field>
    /// <field name="sectypeId" type="String">
    /// </field>
    /// <field name="effDate" type="String">
    /// </field>
    /// <field name="attrListRequired" type="Boolean">
    /// </field>
    /// <field name="isCreationTimeChecked" type="Boolean">
    /// </field>
}
SecMasterJSCommon.SMSecurityTimeSeriesMultiSec.prototype = {
    startDate: null,
    endDate: null,
    attrIdListMaster: null,
    attrIdListSingle: null,
    basketListMulti: null,
    secId: null,
    sectypeId: null,
    effDate: null,
    attrListRequired: false,
    isCreationTimeChecked: false
}


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.SMChangeSecurityTypeRequestInfo

SecMasterJSCommon.SMChangeSecurityTypeRequestInfo = function SecMasterJSCommon_SMChangeSecurityTypeRequestInfo() {
    /// <field name="ExistingSecurityId" type="String">
    /// </field>
    /// <field name="NewSecurityTypeName" type="String">
    /// </field>
}
SecMasterJSCommon.SMChangeSecurityTypeRequestInfo.prototype = {
    ExistingSecurityId: null,
    NewSecurityTypeName: null
}


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.SMChangeSecurityTypeResponseInfo

SecMasterJSCommon.SMChangeSecurityTypeResponseInfo = function SecMasterJSCommon_SMChangeSecurityTypeResponseInfo() {
    /// <field name="StatusMessage" type="String">
    /// </field>
    /// <field name="Status" type="SecMasterJSCommon.SMChangeSecurityTypeStatus">
    /// </field>
    /// <field name="NewSecurityId" type="String">
    /// </field>
    /// <field name="SecurityKey" type="String">
    /// </field>
    /// <field name="ExistingSecurityId" type="String">
    /// </field>
}
SecMasterJSCommon.SMChangeSecurityTypeResponseInfo.prototype = {
    StatusMessage: null,
    Status: 0,
    NewSecurityId: null,
    SecurityKey: null,
    ExistingSecurityId: null
}


////////////////////////////////////////////////////////////////////////////////
// SecMasterJSCommon.SMSViewManager

SecMasterJSCommon.SMSViewManager = function SecMasterJSCommon_SMSViewManager() {
}
SecMasterJSCommon.SMSViewManager.setView = function SecMasterJSCommon_SMSViewManager$setView(identifier) {
    /// <param name="identifier" type="String">
    /// </param>
    var HdnIdentifier = com.ivp.rad.rscriptutils.RSValidators.getObject('identifier');
    HdnIdentifier.value = identifier;
}
SecMasterJSCommon.SMSViewManager.setViewWithID = function SecMasterJSCommon_SMSViewManager$setViewWithID(identifier, id) {
    /// <param name="identifier" type="String">
    /// </param>
    /// <param name="id" type="String">
    /// </param>
    var HdnIdentifier = com.ivp.rad.rscriptutils.RSValidators.getObject('identifier');
    HdnIdentifier.value = identifier;
    var HdnId = com.ivp.rad.rscriptutils.RSValidators.getObject('id');
    HdnId.value = id;
}
SecMasterJSCommon.SMSViewManager.setViewWithIDForCustomReports = function SecMasterJSCommon_SMSViewManager$setViewWithIDForCustomReports(identifier, id) {
    /// <param name="identifier" type="String">
    /// </param>
    /// <param name="id" type="String">
    /// </param>
    var HdnIdentifier = com.ivp.rad.rscriptutils.RSValidators.getObject('identifier');
    HdnIdentifier.value = identifier;
    var HdnId = com.ivp.rad.rscriptutils.RSValidators.getObject('ctl00_hdnIdForCustomReportFramework');
    HdnId.value = id;
}
SecMasterJSCommon.SMSViewManager.setViewForLeftMenu = function SecMasterJSCommon_SMSViewManager$setViewForLeftMenu(identifier, e) {
    /// <param name="identifier" type="String">
    /// </param>
    /// <param name="e" type="Sys.UI.DomEvent">
    /// </param>
    var HdnIdentifier = com.ivp.rad.rscriptutils.RSValidators.getObject('identifier');
    HdnIdentifier.value = identifier;
    if (e.target != null) {
        var HdnAccordionSelected = com.ivp.rad.rscriptutils.RSValidators.getObject('hdnAccordionSelected');
        HdnAccordionSelected.value = e.target.id;
    }
}
SecMasterJSCommon.SMSViewManager.setViewWithControlID = function SecMasterJSCommon_SMSViewManager$setViewWithControlID(identifier, controlId) {
    /// <param name="identifier" type="String">
    /// </param>
    /// <param name="controlId" type="String">
    /// </param>
    var HdnIdentifier = com.ivp.rad.rscriptutils.RSValidators.getObject('identifier');
    HdnIdentifier.value = identifier;
    var Ddl = com.ivp.rad.rscriptutils.RSValidators.getObject(controlId);
    var HdnId = com.ivp.rad.rscriptutils.RSValidators.getObject('id');
    HdnId.value = Ddl.value;
}
SecMasterJSCommon.SMSViewManager.setViewWithIDAndSecondaryID = function SecMasterJSCommon_SMSViewManager$setViewWithIDAndSecondaryID(identifier, id, secondaryID) {
    /// <param name="identifier" type="String">
    /// </param>
    /// <param name="id" type="String">
    /// </param>
    /// <param name="secondaryID" type="String">
    /// </param>
    var HdnIdentifier = com.ivp.rad.rscriptutils.RSValidators.getObject('identifier');
    var HdnId = com.ivp.rad.rscriptutils.RSValidators.getObject('id');
    var HdnSecondaryID = com.ivp.rad.rscriptutils.RSValidators.getObject('secondaryID');
    HdnIdentifier.value = identifier;
    HdnId.value = id;
    HdnSecondaryID.value = secondaryID;
}
SecMasterJSCommon.SMSViewManager.setViewWithSecondaryID = function SecMasterJSCommon_SMSViewManager$setViewWithSecondaryID(identifier, secondaryID) {
    /// <param name="identifier" type="String">
    /// </param>
    /// <param name="secondaryID" type="String">
    /// </param>
    var HdnIdentifier = com.ivp.rad.rscriptutils.RSValidators.getObject('identifier');
    var HdnSecondaryID = com.ivp.rad.rscriptutils.RSValidators.getObject('secondaryID');
    HdnIdentifier.value = identifier;
    HdnSecondaryID.value = secondaryID;
}
SecMasterJSCommon.SMSViewManager.clearFields = function SecMasterJSCommon_SMSViewManager$clearFields() {
    var HdnMasterId = com.ivp.rad.rscriptutils.RSValidators.getObject('masterId');
    var HdnPrimaryName = com.ivp.rad.rscriptutils.RSValidators.getObject('primaryName');
    var HdnSecondaryName = com.ivp.rad.rscriptutils.RSValidators.getObject('secondaryName');
    var HdnId = com.ivp.rad.rscriptutils.RSValidators.getObject('id');
    var HdnCommandName = com.ivp.rad.rscriptutils.RSValidators.getObject('commandName');
    var HdnSecondaryID = com.ivp.rad.rscriptutils.RSValidators.getObject('secondaryID');
    HdnMasterId.value = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
    HdnPrimaryName.value = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
    HdnSecondaryName.value = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
    HdnId.value = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
    HdnCommandName.value = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
    HdnSecondaryID.value = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
    eval('document.onmousemove = null;');
    eval('document.onmousedown = null;');
    eval('document.onmouseup = null;');
}
SecMasterJSCommon.SMSViewManager.addDocumentHandler = function SecMasterJSCommon_SMSViewManager$addDocumentHandler(EventName, handler) {
    /// <param name="EventName" type="String">
    /// </param>
    /// <param name="handler" type="Sys.UI.DomEventHandler">
    /// </param>
    $addHandler(document.documentElement, EventName, handler);
}
SecMasterJSCommon.SMSViewManager.removeDocumentHandler = function SecMasterJSCommon_SMSViewManager$removeDocumentHandler(EventName, handler) {
    /// <param name="EventName" type="String">
    /// </param>
    /// <param name="handler" type="Sys.UI.DomEventHandler">
    /// </param>
    $removeHandler(document.documentElement, EventName, handler);
}
SecMasterJSCommon.SMSViewManager.setVisibility = function SecMasterJSCommon_SMSViewManager$setVisibility(controlID, visFlg) {
    /// <param name="controlID" type="String">
    /// </param>
    /// <param name="visFlg" type="Boolean">
    /// </param>
    var Element = com.ivp.rad.rscriptutils.RSValidators.getObject(controlID);
    if (visFlg) {
        Element.style.display = SecMasterJSCommon.SMSConstants.strinG_EMPTY;
    }
    else {
        Element.style.display = SecMasterJSCommon.SMSConstants.strinG_NONE;
    }
}
SecMasterJSCommon.SMSViewManager.addIteminDDL = function SecMasterJSCommon_SMSViewManager$addIteminDDL(controlID, text, value) {
    /// <param name="controlID" type="String">
    /// </param>
    /// <param name="text" type="String">
    /// </param>
    /// <param name="value" type="String">
    /// </param>
    var Option = document.createElement('option');
    Option.text = text;
    Option.value = value;
    var Select = com.ivp.rad.rscriptutils.RSValidators.getObject(controlID);
    Select.add(Option);
}
SecMasterJSCommon.SMSViewManager.cntrEnable = function SecMasterJSCommon_SMSViewManager$cntrEnable(cntrlID, flg) {
    /// <param name="cntrlID" type="String">
    /// </param>
    /// <param name="flg" type="Boolean">
    /// </param>
    document.getElementById(cntrlID).disabled = flg;
}
SecMasterJSCommon.SMSViewManager.setText = function SecMasterJSCommon_SMSViewManager$setText(cntrlID, cntrlText) {
    /// <param name="cntrlID" type="String">
    /// </param>
    /// <param name="cntrlText" type="String">
    /// </param>
    document.getElementById(cntrlID).innerText = cntrlText;
}
SecMasterJSCommon.SMSViewManager.clearCheckBoxElement = function SecMasterJSCommon_SMSViewManager$clearCheckBoxElement(chkBoxElementId, chkBoxElementItemCount) {
    /// <param name="chkBoxElementId" type="String">
    /// </param>
    /// <param name="chkBoxElementItemCount" type="Number" integer="true">
    /// </param>
    for (var i = 0; i < chkBoxElementItemCount; i++) {
        (com.ivp.rad.rscriptutils.RSValidators.getObject(chkBoxElementId + '_' + i)).checked = false;
    }
}
SecMasterJSCommon.SMSViewManager.clearCheckBoxList = function SecMasterJSCommon_SMSViewManager$clearCheckBoxList(chkBoxLst) {
    /// <param name="chkBoxLst" type="SecMasterJSCommon.CheckBoxList">
    /// </param>
    for (var i = 0; i < chkBoxLst.get_length(); i++) {
        chkBoxLst.get_item(i).checked = false;
    }
}


SecMasterJSCommon.SMSCommons.registerClass('SecMasterJSCommon.SMSCommons');
SecMasterJSCommon.IdentifierSection.registerClass('SecMasterJSCommon.IdentifierSection');
SecMasterJSCommon.MyDictionaryElement.registerClass('SecMasterJSCommon.MyDictionaryElement');
SecMasterJSCommon.SMSDictionary.registerClass('SecMasterJSCommon.SMSDictionary');
SecMasterJSCommon.ObjectEnum.registerClass('SecMasterJSCommon.ObjectEnum');
SecMasterJSCommon.CreateCustomObject.registerClass('SecMasterJSCommon.CreateCustomObject');
SecMasterJSCommon.SMSConstants.registerClass('SecMasterJSCommon.SMSConstants');
SecMasterJSCommon.CheckBoxList.registerClass('SecMasterJSCommon.CheckBoxList');
SecMasterJSCommon.KeyValuePair.registerClass('SecMasterJSCommon.KeyValuePair');
SecMasterJSCommon.CrossBrowserEventAndTarget.registerClass('SecMasterJSCommon.CrossBrowserEventAndTarget');
SecMasterJSCommon.SMSSectypeRuleAttributeInfo.registerClass('SecMasterJSCommon.SMSSectypeRuleAttributeInfo');
SecMasterJSCommon.SMSecurityTimeSeries.registerClass('SecMasterJSCommon.SMSecurityTimeSeries');
SecMasterJSCommon.SMSecurityTimeSeriesMultiSec.registerClass('SecMasterJSCommon.SMSecurityTimeSeriesMultiSec');
SecMasterJSCommon.SMChangeSecurityTypeRequestInfo.registerClass('SecMasterJSCommon.SMChangeSecurityTypeRequestInfo');
SecMasterJSCommon.SMChangeSecurityTypeResponseInfo.registerClass('SecMasterJSCommon.SMChangeSecurityTypeResponseInfo');
SecMasterJSCommon.SMSViewManager.registerClass('SecMasterJSCommon.SMSViewManager');
SecMasterJSCommon.SMSConstants.strinG_VISIBLE = 'visible';
SecMasterJSCommon.SMSConstants.strinG_HIDDEN = 'hidden';
SecMasterJSCommon.SMSConstants.strinG_NONE = 'none';
SecMasterJSCommon.SMSConstants.strinG_EMPTY = '';
SecMasterJSCommon.SMSConstants.clasS_NORMAL_ROW = 'normalRow';
SecMasterJSCommon.SMSConstants.clasS_ALTERNATING_ROW = 'alternatingRow';
SecMasterJSCommon.SMSConstants.clasS_CHECKED_ROW = 'checkedRow';
SecMasterJSCommon.SMSConstants.clasS_GROUPED_ROW = 'groupedRow';
SecMasterJSCommon.SMSConstants.clasS_GROUP_FOOTER = 'groupFooter';
SecMasterJSCommon.SMSConstants.clasS_HEADER = 'header';
SecMasterJSCommon.SMSConstants.clasS_UPDATE_CLIENT_BTN = 'updateClientBtn';
SecMasterJSCommon.SMSConstants.clasS_DELETE_CLIENT_BTN = 'deleteClientBtn';
SecMasterJSCommon.SMSConstants.clasS_BTN_UPDATE = 'btnUpdate';
SecMasterJSCommon.SMSConstants.clasS_BTN_DELETE = 'btnDelete';
SecMasterJSCommon.SMSConstants.selecT_ONE_TEXT = '--Select One--';
SecMasterJSCommon.SMSConstants.selecT_ONE_VALUE = '-1';
SecMasterJSCommon.SMSConstants.htmL_INPUT = 'input';
SecMasterJSCommon.SMSConstants.htmL_INPUT_BUTTON = 'button';
SecMasterJSCommon.SMSConstants.htmL_INPUT_CHECKBOX = 'checkbox';
SecMasterJSCommon.SMSConstants.htmL_INPUT_FILE = 'file';
SecMasterJSCommon.SMSConstants.htmL_INPUT_IMAGE = 'image';
SecMasterJSCommon.SMSConstants.htmL_INPUT_PASSWORD = 'password';
SecMasterJSCommon.SMSConstants.htmL_INPUT_RADIO = 'radio';
SecMasterJSCommon.SMSConstants.htmL_INPUT_RESET = 'reset';
SecMasterJSCommon.SMSConstants.htmL_INPUT_SUBMIT = 'submit';
SecMasterJSCommon.SMSConstants.htmL_INPUT_TEXT = 'text';
SecMasterJSCommon.SMSConstants.htmL_INPUT_HIDDEN = 'hidden';
SecMasterJSCommon.SMSConstants.htmL_SELECT = 'select';
SecMasterJSCommon.SMSConstants.htmL_OPTION = 'option';
SecMasterJSCommon.SMSConstants.htmL_TABLE = 'table';

// ---- Do not remove this footer ----
// This script was generated using Script# v0.5.5.0 (http://projects.nikhilk.net/ScriptSharp)
// -----------------------------------
