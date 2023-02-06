
var path = window.location.protocol + '//' + window.location.host;
var pathname = window.location.pathname.split('/');

$.each(pathname, function (ii, ee) {
    if ((ii !== 0) && (ii !== pathname.length - 1))
        path = path + '/' + ee;
});

var SMMultiSelectDropdown = {};

function CallSMSelect(id) {
    var obj = new Object();
    obj.select = $('#' + id);
    obj.isRunningText = false;
    obj.ready = null;
    obj.showSearch = false;
    smselect.create(obj);
    var objSMSelect = $('#smselect_' + id);

    objSMSelect.addClass('CommonInputStyle attrValueCssDropdown gridDropdownCss').css('padding-left', '3px');
    objSMSelect.find('.smselectrun').css('top', '4px');
    objSMSelect.find('.smselectanchorrun').css('border-bottom', '0px').css('color', 'rgb(102, 102, 102)');
    objSMSelect.find('.smselectoptions').css('text-align', 'left');
    objSMSelect.find('.smselectrun2').css('border', 'none').css('width', '97%');
    objSMSelect.find('.smselectanchorrun2').css('color', '#00bcef');
    objSMSelect.find('.smselectimage').css('color', 'rgb(102, 102, 102)');
}

function CallSMSelectVendorManagement(id, ddlType, setWidthFunc, setPosition) {
    var obj = new Object();
    obj.select = $('#' + id);
    obj.isRunningText = false;
    obj.ready = function (e) {
        if (typeof (setWidthFunc) === "function") {
            setDropdownWidth(e);
        }
        if (ddlType == "1")
            setWidth(e);
    }
    obj.showSearch = false;
    smselect.create(obj);
    var objSMSelect = $('#smselect_' + id);

    objSMSelect.addClass('RMLabeledInput').css('padding-left', '7px');
    //objSMSelect.find('.smselectrun').css('top', '2px');
    objSMSelect.find('.smselectoptions').css('text-align', 'left');
    //objSMSelect.find('.smselectrun2').css('width', '97%');
    objSMSelect.find('.smselectimage').css('color', '#818181');
    if (typeof ddlType === "undefined") {
    }
    else if (ddlType == 1) {
        objSMSelect.addClass('RMLabeledInput');
        //objSMSelect.find('.smselectrun').css('top', '2px');
        objSMSelect.find('.smselectanchorrun').css('border-bottom', '0px');
        objSMSelect.find('.smselectoptions').css('text-align', 'left');
        // objSMSelect.find('.smselectrun2').css('border', 'none');
        objSMSelect.find('.smselectanchorrun2').css('color', '#48a3dd');

        objSMSelect.css({
            'border-top-left-radius': '0px',
            'border-bottom-left-radius': '0px',
            'padding-top': '0px',
            'padding-bottom': '0px',
            'width': '200px'
        });

    }
    if (setPosition == 'setPosition') {
        objSMSelect.removeClass('RMLabeledInput');
        objSMSelect.css({
            'padding-left': '0px',
            'width': '95%',
            'border-top': '1px solid rgb(206, 206, 206)',
            'border-right': '1px solid rgb(206, 206, 206)',
            'border-bottom': '1px solid rgb(206, 206, 206)',
            'border-left': 'none',
            'border-image': 'initial',
            'height': '22px',
            'text-indent': '2px'
        });
        objSMSelect.find('.smselectanchorrun2').css('color', '#818181');
        objSMSelect.find('.smselectimage').css('color', '#000000');

    }
}

function InitSMMultiSelect(targetId, container, menuText, groupName, selectedItems, hdnFieldId, setPositionFunc) {
    selectedItems = $.parseJSON(selectedItems);
    var items = [];
    var selectedUsers = selectedItems;
    var text = "";
    var val = "";
    var listLength = $('#' + targetId).find('option').length;
    for (var i = 0; i < listLength; i++) {
        text = $($('#' + targetId).find('option')[i]).text();
        val = $($('#' + targetId).find('option')[i]).val();
        items.push({ text: text, value: val });
    }

    if (container == null || container.length == 0) {
        container = $('#' + targetId).parent();
    }

    var obj = new Object();
    obj.id = targetId;
    obj.text = menuText;
    obj.isMultiSelect = true;
    obj.showSearch = true;
    obj.container = $(container);
    obj.data = [{ options: items, text: groupName }];
    obj.selectedItems = function () {
        var selectedItemsArr = [];
        if (selectedItems.length > 0) {
            $.each(selectedItems, function (key, selectedItemObj) {
                selectedItemsArr.push(selectedItemObj.text);
            });
        }
        return selectedItemsArr;
    }();
    SMMultiSelectDropdown[targetId] = {
        selectedItemsInOrder: selectedItems
    };

    obj.ready = function (e) {
        e.width('95%');
        $(e).css({ 'border': '1px solid #cecece', 'border-left': 'none', 'height': '22px', 'text-indent': '2px' });
        SMMultiSelectUpdateSelectedItemsText($(e).attr("id"), targetId, hdnFieldId);
        e.on('change', function (ee) {
            //$find('SMSSearch').onGetSelectedSecTypeIds();
            SMMultiSelectUpdateSelectedItemsText($(ee.currentTarget).attr("id"), targetId, hdnFieldId);
        });
        if (typeof (setPositionFunc) === "function") {
            setDropdownPosition(e);
        }
    }
    smselect.create(obj);
}

function setDropdownPosition(element) {
    $(element).find(".smselectcon").css("position", "fixed");

}

function setDropdownWidth(element) {
    $(element).css("width", "190px");
}

function setWidth(element) {
    $(element).css("width", "222px");

}
function SMMultiSelectUpdateSelectedItemsText(targetId, actualTargetId, hdnFieldId) {
    var HdnField = $('#' + hdnFieldId);
    var selectedItems = smselect.getSelectedOption($("#" + targetId));
    if (Object.keys(selectedItems).length == Object.keys(SMMultiSelectDropdown[actualTargetId].selectedItemsInOrder).length + 1) {
        $.each(selectedItems, function (key, selectedItemObj) {
            if ($.grep(SMMultiSelectDropdown[actualTargetId].selectedItemsInOrder, function (obj) { return obj.value == selectedItemObj.value; }).length == 0) {
                SMMultiSelectDropdown[actualTargetId].selectedItemsInOrder.push({ text: selectedItemObj.text, value: selectedItemObj.value });
                return false;
            }
        });
    }
    else if (Object.keys(selectedItems).length == Object.keys(SMMultiSelectDropdown[actualTargetId].selectedItemsInOrder).length - 1) {
        $.each(SMMultiSelectDropdown[actualTargetId].selectedItemsInOrder, function (key, selectedItemObj) {
            if ($.grep(selectedItems, function (obj) { return obj.value == selectedItemObj.value; }).length == 0) {
                var index = SMMultiSelectDropdown[actualTargetId].selectedItemsInOrder.indexOf(selectedItemObj);
                SMMultiSelectDropdown[actualTargetId].selectedItemsInOrder.splice(index, 1);
                return false;
            }
        });
    }


    var selectedItemsText = "";
    var selectedItemsValue = "";
    $.each(SMMultiSelectDropdown[actualTargetId].selectedItemsInOrder, function (key, selectedItemObj) {
        selectedItemsText += selectedItemObj.text + "|";
        selectedItemsValue += selectedItemObj.value + "|";
    });

    if (SMMultiSelectDropdown[actualTargetId].selectedItemsInOrder.length > 0) {
        selectedItemsText = selectedItemsText.substring(0, selectedItemsText.length - 1);
        selectedItemsValue = selectedItemsValue.substring(0, selectedItemsValue.length - 1);
    }
    HdnField.val(selectedItemsValue);
    if (selectedItemsText == "")
        selectedItemsText = "--Select One--";
    $(".smselectrun .smselectanchorcontainer a.smselectanchorrun", $("#" + targetId)).text(selectedItemsText);
}

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

    var rowElement = $('#' + e.target.id).parent().parent().parent();
    var rowId = rowElement.prop('id');
    var index = parseInt(FindRowIndex(rowId, gridId));
    var strHeaderValues = HdnHeaderValue.val();
    var strHeaderNames = HdnHeaderName.val();
    var strHeaderValuesFinal = new Sys.StringBuilder();
    var strHeaderNamesFinal = new Sys.StringBuilder();
    var headerValuesList = strHeaderValues.split('ž');
    var headerNamesList = strHeaderNames.split('ž');
    for (var i = 0; i < headerValuesList.length - 1; i++) {
        if (index !== headerValuesList.length - i - 2) {
            strHeaderValuesFinal.append(headerValuesList[i]);
            strHeaderValuesFinal.append('ž');
            strHeaderNamesFinal.append(headerNamesList[i]);
            strHeaderNamesFinal.append('ž');
        }
    }
    HdnHeaderValue.val(strHeaderValuesFinal.toString());
    HdnHeaderName.val(strHeaderNamesFinal.toString());
    rowElement.remove();
    IdentifierGridFormatting(gridId);
}

function InitiateBBGHeaderGrid(lblErrorId, modalErrorId, txtHeaderNameId, txtHeaderValueId, hdnHeaderNameId, hdnHeaderValueId, gridId, gridNewRowId, addRowButtonId) {
    $('#' + addRowButtonId).unbind('click').click(function (e) {
        onClickAddIdentifierButton(e, lblErrorId, modalErrorId, txtHeaderNameId, txtHeaderValueId, hdnHeaderNameId, hdnHeaderValueId, gridNewRowId, gridId);
    });
    PopulateBBGHeaderGrid(txtHeaderNameId, txtHeaderValueId, hdnHeaderNameId, hdnHeaderValueId, addRowButtonId, gridId);
}

function PopulateBBGHeaderGrid(txtHeaderNameId, txtHeaderValueId, hdnHeaderNameId, hdnHeaderValueId, addRowButtonId, gridId) {
    var TxtHeaderName = $('#' + txtHeaderNameId);
    var TxtHeaderValue = $('#' + txtHeaderValueId);
    var HdnHeaderName = $('#' + hdnHeaderNameId);
    var HdnHeaderValue = $('#' + hdnHeaderValueId);
    var AddButton = $('#' + addRowButtonId);

    var HeaderNamesVal = HdnHeaderName.val();
    var HeaderValuesVal = HdnHeaderValue.val();

    ClearGridRows(gridId);

    if (HeaderNamesVal.length > 0) {
        var headerNameList = HeaderNamesVal.split('ž');
        var headerValueList = HeaderValuesVal.split('ž');

        HdnHeaderName.val("");
        HdnHeaderValue.val("");

        for (var index = 0; index < headerNameList.length - 1; index++) {
            TxtHeaderName.val(headerNameList[index]);
            TxtHeaderValue.val(headerValueList[index]);

            if ((TxtHeaderName.val() != "---Select One---" || TxtHeaderName.val() != "") && TxtHeaderName.val().length > 0)
                AddButton.trigger('click');
        }
        //        smselect.setOptionByIndex($('#smselect_' + txtHeaderNameId), 0);
        TxtHeaderName.val("");
        TxtHeaderValue.val("");
    }
}

function onClickAddIdentifierButton(e, lblErrorId, modalErrorId, txtHeaderNameId, txtHeaderValueId, hdnHeaderNameId, hdnHeaderValueId, gridNewRowId, gridId) {
    var LblError = $('#' + lblErrorId);
    var TxtHeaderName = $('#' + txtHeaderNameId);
    var TxtHeaderValue = $('#' + txtHeaderValueId);
    var HdnHeaderName = $('#' + hdnHeaderNameId);
    var HdnHeaderValue = $('#' + hdnHeaderValueId);
    var GridNewRow = $('#' + gridNewRowId);

    var HeaderName = TxtHeaderName.val();
    var HeaderValue = TxtHeaderValue.val();

    if (HeaderName.trim() === '' || HeaderName.trim() === '---Select One---' || HeaderName.length === 0) {
        LblError.text('* Header Name cannot be blank');
        $find(modalErrorId).show();
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
        if (HdnHeaderName.val().toLowerCase().split('ž').indexOf(HeaderName.toLowerCase()) > -1) {
            LblError.text('* Header Name already in list');
            $find(modalErrorId).show();
        }
        else {
            //            TxtHeaderName.val("---Select One---");
            //smselect.setOptionByIndex($('#smselect_' + txtHeaderNameId), 0);
            TxtHeaderValue.val("");
            TxtHeaderName.val("");
            var str = '';
            str += '<div id=\"' + gridId + '_gridRow_' + GridNewRow.attr('countRows') + '\" class=\"identifierGridRowsClass ' + rowClass + '\"><div class=\"SMNewGridBodyElement\" style=\"text-indent: 6px;\">' + HeaderName + '</div><div class=\"SMNewGridBodyElement\" style=\"text-indent: 8px;\">' + HeaderValue + '</div><div class=\"SMNewGridBodyElement\" style=\"width: 0%; border: 0px;\"><span style=\"font-size: 15px; padding-left: 25px; color: rgb(146, 146, 146);\"><i id=\"' + gridId + '_deleteIdentifierButton_' + GridNewRow.attr('countRows') + '\" class=\"fa fa-trash deleteGridRowElemenet\" style=\"cursor: pointer;\"></i></span></div></div>';
            $(str).insertAfter(GridNewRow);
            GridNewRow.attr('countRows', countRows.toString());
            $('.deleteGridRowElemenet').unbind('click').click(function (e) {
                onDeleteGridRowElemenetClick(e, hdnHeaderNameId, hdnHeaderValueId, gridId);
            });
            var strHeaderValues = new Sys.StringBuilder();
            var strHeaderNames = new Sys.StringBuilder();
            strHeaderValues.append(HdnHeaderValue.val());
            strHeaderValues.append(HeaderValue);
            strHeaderValues.append('ž');
            strHeaderNames.append(HdnHeaderName.val());
            strHeaderNames.append(HeaderName);
            strHeaderNames.append('ž');
            HdnHeaderValue.val(strHeaderValues.toString());
            HdnHeaderName.val(strHeaderNames.toString());
            IdentifierGridFormatting(gridId);
        }
    }
}

function CheckTimeFormat(Value) {
    var pattern = /^\d+$/;
    var timeObj = Value.split(':');
    for (var index = 0; index < timeObj.length; index++) {
        if (!pattern.test(timeObj[index]))
            return false;
    }
    return true;
}

function getServerDate() {

    // var dateSet;
    return $.ajax({
        type: "POST",
        url: "./BaseUserControls/Service/Service.asmx/GetServerDate",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        //success: function (r) {
        //    dateSet = r.d;
        //    return dateSet;
        //    //  alert(r.d);
        //},
        //error: function (r) {
        //    // alert(r.responseText);
        //},
        //failure: function (r) {
        //    // alert(r.responseText);
        //}
    });

}

function TabsBarFormatting(TabContainerSelector, subtractWidth) {
    setTimeout(function () {
        var TabsBar = $(TabContainerSelector);
        if (TabsBar.length > 0) {
            for (var index = 0; index < TabsBar.length; index++) {
                if (TabsBar.eq(index).hasClass('ajax__tab_container') && TabsBar.eq(index).children().length > 0) {
                    var HeaderTabDiv = TabsBar.eq(index).children().eq(0);
                    HeaderTabDiv.css('display', 'inline-block');
                    var HeaderTabDivWidth = HeaderTabDiv.width();

                    var viewPortWidth = $(window).width();
                    var WizardMenuWidth = 0;
                    var WizardMenu = $('#divLeftPanel');
                    if (WizardMenu.length > 0)
                        WizardMenuWidth = WizardMenu.width();

                    var TabsBarWidth = viewPortWidth - WizardMenuWidth - subtractWidth;

                    if (HeaderTabDivWidth > TabsBarWidth) {
                        TabsBarWidth = TabsBarWidth - 82;
                        HeaderTabDiv.css('overflow', 'hidden');
                        HeaderTabDiv.width(TabsBarWidth);
                        HeaderTabDiv.css('margin-left', '0px');

                        var divTabHeaderLeftArrow = $('<div class="SMTabHeaderLeftArrow" scrollindex="0" style="position: relative; top: 3px; padding-right: 10px;"></div>');
                        var divTabHeaderRightArrow = $('<div class="SMTabHeaderRightArrow" scrollindex="0" style="position: relative; top: 3px;"></div>');

                        HeaderTabDiv.before(divTabHeaderLeftArrow);
                        HeaderTabDiv.after(divTabHeaderRightArrow);

                        $('.SMTabHeaderLeftArrow').unbind('click').click(function () {
                            HeaderTabDiv.scrollLeft(HeaderTabDiv.scrollLeft() - 100);
                        });
                        $('.SMTabHeaderRightArrow').unbind('click').click(function () {
                            HeaderTabDiv.scrollLeft(HeaderTabDiv.scrollLeft() + 100);
                        });
                        HeaderTabDivWidth = TabsBarWidth + 70;
                    }
                    else {
                        HeaderTabDiv.css('display', 'block');
                        HeaderTabDivWidth = TabsBarWidth;
                    }

                    if (TabsBar.eq(index).width() < 600)
                        TabsBar.eq(index).css("width", (HeaderTabDivWidth) + "px");
                }
            }
        }
    }, 1000);
}

function OnFailure(result) {
}

var RuleDivId = null;
var BtnSaveId = null;
var HdnRuleClassInfoId = null;

function RuleEditorRecon(sectypeId, RuleTypeRAD, ruleDivId, btnSaveId, hdnRuleClassInfoId, dbType) {
    RuleDivId = ruleDivId;
    BtnSaveId = btnSaveId;
    HdnRuleClassInfoId = hdnRuleClassInfoId;

    var parameters = {
    };
    parameters.sectypeId = sectypeId;
    parameters.ruleType = RuleTypeRAD;
    parameters.dbType = dbType;
    callService('POST', path + '/BaseUserControls/Service/SMDashboardService.svc', 'PrepareRuleGrammarInfoRecon', parameters, onSuccess_RuleEditorInitiator, OnFailure, null, null, false);
}

function RuleEditorInitiatorWithoutText(CurrentPageId, columnTypeId, ReportType, LoginName, Module, RuleTypeRAD, ruleDivId, btnSaveId, hdnRuleClassInfoId) {
    RuleDivId = ruleDivId;
    BtnSaveId = btnSaveId;
    HdnRuleClassInfoId = hdnRuleClassInfoId;

    var parameters = {
    };
    parameters.moduleType = CurrentPageId;
    parameters.columnID = columnTypeId;
    parameters.rptType = ReportType;
    parameters.userName = LoginName;
    parameters.module = Module;
    parameters.ruleTypeRAD = RuleTypeRAD;
    callService('POST', path + '/BaseUserControls/Service/SMDashboardService.svc', 'PrepareRuleGrammarInfoNew', parameters, onSuccess_RuleEditorInitiator, OnFailure, null, null, false);
}

function RuleEditorInitiatorModeler(SecurityTypeId, AttributeId, AttributeName, RuleTypeSM, lstReferenceAttributeName, GetDataSet, ruleDivId, btnSaveId, hdnRuleClassInfoId, module) {
    RuleDivId = ruleDivId;
    BtnSaveId = btnSaveId;
    HdnRuleClassInfoId = hdnRuleClassInfoId;
    attr_id_Split = AttributeId.split('<$>');
    attr_id = attr_id_Split[0];
    is_additional_leg = false;
    if (attr_id_Split.length > 1)
        is_additional_leg = (attr_id_Split[1] == "true");

    var parameters = {
    };
    parameters.secTypeId = SecurityTypeId;
    parameters.attributeId = attr_id;
    parameters.displayName = AttributeName;
    parameters.ruleType = RuleTypeSM;
    parameters.lstReferenceAttributeName = lstReferenceAttributeName;
    parameters.getDataSet = GetDataSet;
    if (module.toLowerCase() == "sm") {
        parameters.is_additional_leg = is_additional_leg;
        callService('POST', path + '/BaseUserControls/Service/SMDashboardService.svc', 'PrepareRuleGrammarInfoModeler', parameters, onSuccess_RuleEditorInitiator, OnFailure, null, null, false);
    }
    else if (module.toLowerCase() == "ca") {
        callService('POST', path + '/BaseUserControls/Service/SMDashboardService.svc', 'PrepareRuleGrammarInfoModelerCorpType', parameters, onSuccess_RuleEditorInitiator, OnFailure, null, null, false);
    }
}

function RuleEditorInitiatorRealTime(secTypeTableId, tableType, selectedIndex, secTypeID, externalVendorId, userName, lstFieldNames, ruleDivId, btnSaveId, hdnRuleClassInfoId) {
    var lstFields = JSON.parse(lstFieldNames);
    RuleDivId = ruleDivId;
    BtnSaveId = btnSaveId;
    HdnRuleClassInfoId = hdnRuleClassInfoId;

    var parameters = {
    };
    parameters.SecTypeTableId = secTypeTableId;
    parameters.TableType = tableType;
    parameters.SelectedIndex = selectedIndex;
    parameters.SecTypeID = secTypeID;
    parameters.ExternalVendorId = externalVendorId;
    parameters.UserName = userName;
    parameters.LstFieldNames = lstFields;
    callService('POST', path + '/BaseUserControls/Service/SMDashboardService.svc', 'PrepareRuleGrammarInfoRealTime', parameters, onSuccess_RuleEditorInitiator, OnFailure, null, null, false);
}

function RuleEditorInitiatorRealTimeForContracts(VendorId, ruleDivId, btnSaveId, hdnRuleClassInfoId) {
    RuleDivId = ruleDivId;
    BtnSaveId = btnSaveId;
    HdnRuleClassInfoId = hdnRuleClassInfoId;

    var parameters = {
    };
    parameters.VendorId = VendorId;
    callService('POST', path + '/BaseUserControls/Service/SMDashboardService.svc', 'PrepareRuleGrammarInfoRealTimeContracts', parameters, onSuccess_RuleEditorInitiator, OnFailure, null, null, false);
}

function RuleEditorInitiatorRealTimeForFacilities(VendorId, ruleDivId, btnSaveId, hdnRuleClassInfoId) {
    RuleDivId = ruleDivId;
    BtnSaveId = btnSaveId;
    HdnRuleClassInfoId = hdnRuleClassInfoId;

    var parameters = {
    };
    parameters.VendorId = VendorId;
    callService('POST', path + '/BaseUserControls/Service/SMDashboardService.svc', 'PrepareRuleGrammarInfoRealTimeFacilities', parameters, onSuccess_RuleEditorInitiator, OnFailure, null, null, false);
}

function RuleEditorInitiatorBasket(flagBindRuleEditor, RuleTypeRAD, DDLBasketSelectedValue, DdlBasketUniqueId, lstReferenceAttributeName, ruleDivId, btnSaveId, hdnRuleClassInfoId, SecurityTypeId) {
    RuleDivId = ruleDivId;
    BtnSaveId = btnSaveId;
    HdnRuleClassInfoId = hdnRuleClassInfoId;
    sectype_table_id_Split = DDLBasketSelectedValue.split('<$>');
    sectype_table_id = parseInt(sectype_table_id_Split[0]);
    is_additional_leg = false;
    if (sectype_table_id_Split.length > 1)
        is_additional_leg = (sectype_table_id_Split[1] == "true");

    var parameters = {
    };
    parameters.flagBindRuleEditor = flagBindRuleEditor;
    parameters.RuleTypeRAD = RuleTypeRAD;
    parameters.DDLBasketSelectedValue = sectype_table_id;
    parameters.DdlBasketUniqueId = parseInt(sectype_table_id);
    parameters.lstReferenceAttributeName = lstReferenceAttributeName;
    parameters.SecurityTypeId = SecurityTypeId;
    parameters.is_additional_leg = is_additional_leg;


    callService('POST', path + '/BaseUserControls/Service/SMDashboardService.svc', 'PrepareRuleGrammarInfoBasket', parameters, onSuccess_RuleEditorInitiatorBasket, OnFailure, null, null, false);
}

function onSuccess_RuleEditorInitiatorBasket(result) {
    if (result.d != null) {
        if (typeof ($("#" + RuleDivId).data('ruleEngine')) !== "undefined")
            $("#" + RuleDivId).ruleEngine().data('ruleEngine').Destroy();
        $("#" + RuleDivId).empty();
        $("#" + RuleDivId).ruleEngine({ grammarInfo: result.d, serviceUrl: path + "/BaseUserControls/Service/RADXRuleEditorService.svc", ExternalFunction: RuleCompleteHandlerBasket });
    }
}

function RuleEditorInitiatorCommonPush(selectedAttribute, userName, RuleTypeSM, lstReferenceAttributeName, RuleTypeRAD, getDataSet, ruleDivId, btnSaveId, hdnRuleClassInfoId) {
    RuleDivId = ruleDivId;
    BtnSaveId = btnSaveId;
    HdnRuleClassInfoId = hdnRuleClassInfoId;

    var parameters = {
    };
    parameters.selectedAttribute = selectedAttribute;
    parameters.userName = userName;
    parameters.RuleTypeSM = RuleTypeSM;
    parameters.lstReferenceAttributeName = lstReferenceAttributeName;
    parameters.RuleTypeRAD = RuleTypeRAD;
    parameters.getDataSet = getDataSet;
    callService('POST', path + '/BaseUserControls/Service/SMDashboardService.svc', 'PrepareRuleGrammarInfoCommonPush', parameters, onSuccess_RuleEditorInitiator, OnFailure, null, null, false);
}

function onSuccess_RuleEditorInitiator(result) {
    if (result.d != null) {
        $('#' + BtnSaveId).show();
        var RuleDiv = $("#" + RuleDivId);
        RuleDiv.empty();
        RuleDiv.RRuleEditor({ grammarInfo: result.d, serviceUrl: path + "/BaseUserControls/Service/RADXRuleEditorService.svc" }); //, ExternalFunction: RuleCompleteHandler });
    }
}

function SetRuleEditorTextBasket(ruleDivId, HdnRuleTextId) {
    if (typeof $("#" + ruleDivId).data('ruleEngine') === "undefined") {
        window.setTimeout(function () { SetRuleEditorTextBasket(ruleDivId, HdnRuleTextId); }, 500);
    } else {
        var regEx = new RegExp(String.fromCharCode(160), 'gi');
        var normalSpace = ' ';

        var ruleText = $("#" + HdnRuleTextId).val().replace(regEx, normalSpace);

        var grammarInfo = $("#" + ruleDivId).data('ruleEngine').settings.grammarInfo;
        $("#" + ruleDivId).ruleEngine().data('ruleEngine').Destroy();
        $("#" + ruleDivId).ruleEngine({ grammarInfo: grammarInfo, serviceUrl: path + "/BaseUserControls/Service/RADXRuleEditorService.svc", ruleText: ruleText, ExternalFunction: RuleCompleteHandlerBasket });
    }
}

function SetRuleEditorTextNew(ruleDivId, HdnRuleTextId) {
    var RuleDiv = $("#" + ruleDivId);
    if (typeof RuleDiv.data('radWidgetRRuleEditor') === "undefined") {
        window.setTimeout(function () { SetRuleEditorTextNew(ruleDivId, HdnRuleTextId); }, 500);
    } else {
        var ruleText = $("#" + HdnRuleTextId).val().replaceNBR();
        ruleText = $("<div />").html(ruleText).text();
        $("#" + RuleDivId).data('radWidgetRRuleEditor').changeRuleText(ruleText);
    }
}

function RuleCompleteHandlerNew() {
    var objRRuleEditor = $("#" + RuleDivId).data('radWidgetRRuleEditor');
    if (objRRuleEditor.isRuleComplete()) {
        var ruleClass = objRRuleEditor.getGeneratedCode();
        var ruleText = objRRuleEditor.saveRule();
        $('#' + HdnRuleClassInfoId).val($('<div/>').text(ruleClass[0] + "||$$||" + ruleClass[1] + "||$$||" + ruleText).html());
        return true;
    }
    else
        return false;
}

function RuleCompleteHandlerBasket(state) {
    setTimeout(function () {
        if (state) {
            $('#' + BtnSaveId).show();
            var ruleClass = $("#" + RuleDivId).ruleEngine().data('ruleEngine').getGeneratedCode();
            var ruleText = $("#ruleTxt").val();
            $('#' + HdnRuleClassInfoId).val($('<div/>').text(ruleClass[0] + "||$$||" + ruleClass[1] + "||$$||" + ruleText).html());
        }
        else
            $('#' + BtnSaveId).hide();
    }, 200);
}

function RuleCompleteHandler(state) {
    //    setTimeout(function () {
    //        if (state) {
    //            $('#' + BtnSaveId).show();
    //            //            var ruleText = $("#ruleTxt").val();
    //            var objRRuleEditor = $("#" + RuleDivId).data('RRuleEditor');
    //            var ruleClass = objRRuleEditor.getGeneratedCode();
    //            var ruleText = objRRuleEditor.saveRule();
    //            $('#' + HdnRuleClassInfoId).val($('<div/>').text(ruleClass[0] + "||$$||" + ruleClass[1] + "||$$||" + ruleText).html());
    //        }
    //        else
    //            $('#' + BtnSaveId).hide();
    //    }, 200);
}

function callService(httpMethod, serviceServerString, methodName, parameters, ajaxSuccess, ajaxError, response, userContext, isCrossDomain) {
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
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: ajaxSuccess,
                error: ajaxError,
                param1: userContext
            };
        }
    }
    $.ajax(options);
}

//AUTOCOMPLETE
//Neetika
//function autoCompleteSuccess(data) {
//    var newSet = new Array();
//    var jason;
//    $.each(data.d, function (index, value) {
//        jason = new Object();
//        jason.label = value.split('<@>')[0];
//        jason.id = value.split('<@>')[1];
//        newSet[index] = jason;
//    });
//    $(this)[0].param1.response(newSet);
//    //$(this)[0].response(newSet);
//}

function autoCompleteSuccess(data) {
    var newSet = new Array();
    var jason;
    var columnCount;

    $.each(data.d, function (index, value) {
        if (value.indexOf('<@>') > 0) {
            jason = new Object();
            jason.label = value.split('<@>')[0];
            jason.id = value.split('<@>')[1];
        }
        else { // seperated by comma used incase of multiple columns in ref ddl
            if (index == 0) {
                columnCount = value.split(',').length;

                jason = new Object();
                //jason["EntityCode"] = value.split(',')[0];
                for (var i = 0; i < columnCount ; i++) {
                    jason[value.split(',')[i]] = value.split(',')[i];
                }
            }
            else {
                jason = new Object();
                var i = 0;
                for (var k in Object.keys(newSet[0])) {
                    jason[Object.keys(newSet[0])[k]] = value.split(',')[i];
                    i++;
                }

            }

        }
        newSet[index] = jason;
    });
    $(this)[0].param1.response(newSet);
    //$(this)[0].response(newSet);
}

function CallJqueryAutoCompleteService(id, parametersList, serviceServerString, methodName, autoCompleteSuccessJquery, autoCompleteError, autoCompleteSelect, autoCompleteChange, autoCompleteOpen, autoCompleteClose, renderData) {
    var target = $('#' + id);
    if (autoCompleteChange != null && autoCompleteChange != undefined)
        target.unbind('change').change(autoCompleteChange);
    else
        target.unbind('change').change(function (e) {
            var list = target.autocomplete("widget");
            var targetValText = target.val().toLowerCase();
            if (targetValText == '') {
                target.val('');
            }
            else {
                if (list.attr('selection') == undefined || (list.attr('selection') != undefined && list.attr('selection') != '1')) {
                    var li = $(list).find('li');
                    var a = $(li[0]).find('a');
                    var firstMatchText = a.textNBR();
                    var text = firstMatchText.toLowerCase();

                    if (text.startsWith(targetValText)) {
                        target.val(firstMatchText);
                    }
                    else {
                        target.val('');
                    }
                }
            }
        });
    target.autocomplete({
        source: function (request, response) {
            var parameters = {
            };
            parameters['term'] = request.term;
            for (var param in parametersList) {
                parameters[param] = parametersList[param];
            }
            var a = new Object();
            a.userContext = $(this)[0].element.attr('id');
            a.response = response;
            callService('POST', serviceServerString, methodName, parameters, autoCompleteSuccessJquery, autoCompleteError, response, a, false);
        },
        messages: {
            noResults: '',
            results: function () { }
        },
        select: autoCompleteSelect,
        minLength: 0,
        cacheLength: 1,
        delay: 0,
        open: autoCompleteOpen,
        close: autoCompleteClose
    }).unbind('click').click(function () { if ($(this).val() == null || $(this).val() == '') $(this).autocomplete("search", $('#' + id).val()) });

    if (renderData != null && renderData != undefined)
        target.data("autocomplete")._renderItem = renderData;

    if (target.val() == null || target.val() == '')
        target.autocomplete("search", $('#' + id).val());
}

//Neetika
//function CallAutoCompleteService(id, dateTimeFormat, serviceServerString, methodName, autoCompleteError, autoCompleteSelect, autoCompleteChange, autoCompleteOpen) {//, autoCompleteFocus, autoCompleteFocusOut, autoCompleteOpen
//    var target = $('#' + id);
//    if (autoCompleteChange != null && autoCompleteChange != undefined)
//        target.unbind('change').change(autoCompleteChange);
//    target.autocomplete({
//        source: function (request, response) {
//            var parameters = {
//            };
//            parameters['term'] = request.term;
//            parameters['contextKey'] = $(this)[0].element.attr('RTI') + '@' + $(this)[0].element.attr('RAN') + '@' + dateTimeFormat;
//            var a = new Object();
//            a.userContext = $(this)[0].element.attr('id');
//            a.response = response;
//            callService('POST', serviceServerString, methodName, parameters, autoCompleteSuccess, autoCompleteError, response, a, false);
//        },
//        messages: {
//            noResults: '',
//            results: function () { }
//        },
//        select: autoCompleteSelect,
//        minLength: 0,
//        cacheLength: 1,
//        delay: 0,
//        open: autoCompleteOpen
//        //position: { collision: "flip flip", my: "left bottom", at: "left bottom" }
//        //        ,
//        //        position: { my: "left top", at: "left bottom", collision: 'flip' }
//    }).unbind('click').click(function () { if ($(this).val() == null || $(this).val() == '') $(this).autocomplete("search", $('#' + id).val()) })
//    .data("autocomplete")._renderItem = function (ul, item) {
//        ul.removeAttr('selection');
//        return $("<li></li>")
//   .data("item.autocomplete", item)
//   .append("<a   title='" + item.label + "' EC='" + item.id + "' >" + (item.label) + "</a>").appendTo(ul)
//    };
//    if (target.val() == null || target.val() == '')
//        target.autocomplete("search", $('#' + id).val());
//}


function CallAutoCompleteService(id, dateTimeFormat, serviceServerString, methodName, autoCompleteError, autoCompleteSelect, autoCompleteChange, autoCompleteOpen) {//, autoCompleteFocus, autoCompleteFocusOut, autoCompleteOpen
    var target = $('#' + id);
    if (autoCompleteChange != null && autoCompleteChange != undefined)
        target.unbind('change').change(autoCompleteChange);
    target.autocomplete({
        source: function (request, response) {
            var parameters = {
            };
            parameters['term'] = request.term;
            if ($(this)[0].element.attr('AD') != undefined)
                parameters['contextKey'] = $(this)[0].element.attr('RTI') + '@' + $(this)[0].element.attr('RAN') + '|' + $(this)[0].element.attr('AD').split('<@>')[1] + '|0' + '@' + dateTimeFormat; //MASTER
            else if ($(this)[0].element.attr('AI') != undefined)
                parameters['contextKey'] = $(this)[0].element.attr('RTI') + '@' + $(this)[0].element.attr('RAN') + '|' + $(this)[0].element.attr('AI') + '|' + $(this)[0].element.attr('AdditionalLeg') + '@' + dateTimeFormat; //LEGS
            else
                parameters['contextKey'] = $(this)[0].element.attr('RTI') + '@' + $(this)[0].element.attr('RAN') + '|' + $(this)[0].element.attr('AID') + '@' + dateTimeFormat; //Exception Manager

            var a = new Object();
            a.userContext = $(this)[0].element.attr('id');
            a.response = response;
            callService('POST', serviceServerString, methodName, parameters, autoCompleteSuccess, autoCompleteError, response, a, false);
        },
        messages: {
            noResults: '',
            results: function () { }
        },
        select: function (e, i) {
            if ($(e.toElement).parent().prop('id') != 'divEntityCodeId')
                autoCompleteSelect(e, i);
            else {

                // var uniqueTabId = "RTS_" + e.toElement.attributes["ec"].value;
                var queryStr = "&entityTypeId=&entityDisplayName=&entityCode=" + $(e.toElement).html() + "&entityTypeId" + e.target.attributes["rti"].value;;// + "&tabIframeId=" + uniqueTabId + "&checkpriviledge=1";
                var url = path + '/App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?pageIdentifier=UpdateEntityFromSearch&CalledFrom=RefMasterExternal';
                SecMasterJSCommon.SMSCommons.createTab('RefM_EditEntity', url + queryStr, SecMasterJSCommon.SMSCommons.getGUID(), 'Edit Entity');
            }
        },
        minLength: 0,
        cacheLength: 1,
        delay: 0,
        open: autoCompleteOpen
        //position: { collision: "flip flip", my: "left bottom", at: "left bottom" }
        //        ,
        //        position: { my: "left top", at: "left bottom", collision: 'flip' }
    }).unbind('click').click(function () { if ($(this).val() == null || $(this).val() == '') $(this).autocomplete("search", $('#' + id).val()) })
    $.ui.autocomplete.prototype._renderMenu = function (ul, items) {
        var self = this;
        var colheader = '';
        Object.keys(items[0])
  .forEach(function eachKey(key) {
      if (items[0][key] != undefined)
          colheader += '<th class="ref_ddl_th">' + items[0][key].split('|')[0] + '</th>';

  });
        ul.append("<table class='ref-ddl_table'><thead class='headCls invisible-scrollbar' style='display:block;'><tr class='ref_ddl_headers'>" + colheader + "</tr></thead><tbody class='bodyCls mostly-customized-scrollbar'></tbody></table>");
        $.each(items, function (index, item) {
            if (index != 0)//to avoid column header
                self._renderItemData(ul, ul.find("table tbody"), item);
        });
    };
    $.ui.autocomplete.prototype._renderItemData = function (ul, table, item) {
        return this._renderItem(table, item).data("ui-autocomplete-item", item);
    };
    $.ui.autocomplete.prototype._renderItem = function (table, item) {
        var tr = $("<tr class='ui-menu-item ref_ddl_rows' role='presentation'></tr>");
        var colval;
        for (var key in Object.keys(item)) {
            if (item[Object.keys(item)[key]] != undefined)
                if (Object.keys(item)[key].toLowerCase() == 'entity code|entity_code')
                    colval += "<td class='ref_ddl_columns'><div id=\"divEntityCodeId\"  style=\"color:#48a3dd;text-decoration:underline;cursor:pointer;\"><a EC='" + item["Entity Code|entity_code"] + "'>" + item[Object.keys(item)[key]] + "</a></div></td>";
                else if (Object.keys(item).indexOf('label') > -1 && Object.keys(item).indexOf('value') > -1 && item.label != undefined) {
                    if (Object.keys(item)[key].toLowerCase() == 'label')
                        colval += "<td class='ref_ddl_columns' ><a EC='" + item.id + "'>" + item.label + "</a></td>";
                }
                else
                    colval += "<td class='ref_ddl_columns'><a  EC='" + item["Entity Code|entity_code"] + "'>" + item[Object.keys(item)[key]] + '</a></td>';
        }
        tr.append(colval)
      .appendTo(table);
        return tr;
    };

    if (target.val() == null || target.val() == '')
        target.autocomplete("search", $('#' + id).val());
}

function autoCompleteSuccessFiltered(data) {
    var newSet = new Array();
    var jason;
    var target = $('#' + $(this)[0].userContext);
    var attributeName = $('#' + $(this)[0].userContext).attr('AN');
    if (data != null && data.d != null && data.d.length > 0) {
        var tabId = $(this)[0].param1.userContext;
        //var tabId = $(this)[0].userContext;
        var selectedConditionalFilterInfo = eval("(" + data.d + ")")[tabId][0];
        if (selectedConditionalFilterInfo != null) {
            $(this)[0].param1.conditionalFilterInfo.StringSearch = null;
            //$(this)[0].conditionalFilterInfo.StringSearch = null;
            if (selectedConditionalFilterInfo.SearchResult != null && selectedConditionalFilterInfo.SearchResult.length > 0) {
                $.each(selectedConditionalFilterInfo.SearchResult, function (index, value) {
                    if (value.indexOf('<@>') > 0) {
                        jason = new Object();
                        jason.label = value.split('<@>')[0];
                        jason.id = value.split('<@>')[1];
                    }
                    else { // seperated by comma used incase of multiple columns in ref ddl
                        if (index == 0) {
                            columnCount = value.split(',').length;

                            jason = new Object();
                            //jason["EntityCode"] = value.split(',')[0];
                            for (var i = 0; i < columnCount ; i++) {
                                jason[value.split(',')[i]] = value.split(',')[i];
                            }
                        }
                        else {
                            jason = new Object();
                            var i = 0;
                            for (var k in Object.keys(newSet[0])) {
                                jason[Object.keys(newSet[0])[k]] = value.split(',')[i];
                                i++;
                            }

                        }

                    }
                    newSet[index] = jason;
                });
                $(this)[0].param1.response(newSet);
                //$(this)[0].response(newSet);
            }
        }

        var groupDictionary = $(this)[0].param1.groupDictionary;
        for (var attributeNameKey in groupDictionary) {
            var conditionalFilterInfo = groupDictionary[attributeNameKey];
            var tabType = conditionalFilterInfo.TabType;
            if ((tabType == SMSecurityAction.SingleInfo && conditionalFilterInfo.SingleInfoHasConstituent) || tabType == SMSecurityAction.MutipleInfo) {
                if (tabId == conditionalFilterInfo.TabElementId) {
                    conditionalFilterInfo.Value = '';
                    conditionalFilterInfo.EntityCode = '';
                }
            }
        }
    }
}

function CallAutoCompleteServiceFiltered(id, dateTimeFormat, serviceServerString, methodName, autoCompleteError, autoCompleteSelect, _securityInfo, userContext, autoCompleteFilteredOpen, isFullSearch) {//, autoCompleteFocus, autoCompleteFocusOut, autoCompleteOpen
    var target = $('#' + id);
    target.unbind('change').change(autoCompleteChange);
    target.autocomplete({
        source: function (request, response) {
            //target.autocomplete('option').isFullSearch
            var a = new Object();
            a.userContext = userContext; //$(this)[0].element.attr('id')
            a.response = response;
            var attributeName = target.attr('AN');
            var groupDictionary = _securityInfo.DictConditionalFilter[target.attr('GN')];
            var groupList = [];
            var selectedConditionalFilterInfo = groupDictionary[attributeName];
            var selectedTabType = selectedConditionalFilterInfo.TabType;
            var selectedTabElementId = selectedConditionalFilterInfo.TabElementId;
            var txtBox = $('#' + target.attr('replacedWith'));
            var lstSplitIds = txtBox.prop('id').split('_');
            for (var attributeNameKey in groupDictionary) {
                var conditionalFilterInfo = groupDictionary[attributeNameKey];
                if ((selectedTabType == SMSecurityAction.SingleInfo && selectedConditionalFilterInfo.SingleInfoHasConstituent) || selectedTabType == SMSecurityAction.MutipleInfo) {
                    if (selectedTabElementId == conditionalFilterInfo.TabElementId) {
                        var attributeElementId = '';
                        for (var i = 0; i < lstSplitIds.length - 1; i++) {
                            attributeElementId += lstSplitIds[i] + '_';
                        }
                        attributeElementId += conditionalFilterInfo.ColumnNumber;
                        if (attributeElementId != txtBox.prop('id')) {
                            var attributeElement = $('#' + attributeElementId);
                            conditionalFilterInfo.Value = attributeElement.val();
                            conditionalFilterInfo.EntityCode = attributeElement.attr('EC');
                        }
                        else {
                            conditionalFilterInfo.Value = target.val();
                            conditionalFilterInfo.EntityCode = target.attr('EC');
                        }
                    }
                }
                if (attributeName.toLowerCase() == attributeNameKey.toLowerCase()) {
                    conditionalFilterInfo.StringSearch = request.term;
                    a.conditionalFilterInfo = conditionalFilterInfo;
                }
                groupList[groupList.length] = conditionalFilterInfo;
            }

            a.groupDictionary = groupDictionary;

            var parameters = {
            };
            parameters['userIdentifier'] = _securityInfo.SessionIdentifier;
            parameters['actionType'] = SMConditionalFilterActionType.Filter;
            parameters['attributeName'] = attributeName;
            parameters['lstConditionalFilterInfo'] = groupList;
            parameters['dateTimeFormat'] = dateTimeFormat;


            callService('POST', serviceServerString, methodName, parameters, autoCompleteSuccessFiltered, autoCompleteError, response, a, false);
        },
        select: function (e, i) {
            if ($(e.toElement).parent().prop('id') != 'divEntityCodeId')
                autoCompleteSelect(e, i);
            else {

                // var uniqueTabId = "RTS_" + e.toElement.attributes["ec"].value;
                var queryStr = "&entityTypeId=&entityDisplayName=&entityCode=" + $(e.toElement).html() + "&entityTypeId" + e.target.attributes["rti"].value;;// + "&tabIframeId=" + uniqueTabId + "&checkpriviledge=1";
                var url = path + '/App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?pageIdentifier=UpdateEntityFromSearch&CalledFrom=RefMasterExternal';
                SecMasterJSCommon.SMSCommons.createTab('RefM_EditEntity', url + queryStr, SecMasterJSCommon.SMSCommons.getGUID(), 'Edit Entity');
            }
        },
        minLength: 0,
        cacheLength: 1,
        delay: 0,
        open: autoCompleteFilteredOpen,
        isFullSearch: ((isFullSearch == null) ? false : isFullSearch)
        //        ,
        //        position: { my: "left bottom", at: "left top", collision: 'flip' }
    }).unbind('click').click(function () { if ($(this).val() == null || $(this).val() == '') $(this).autocomplete("search", $('#' + id).val()) })
    $.ui.autocomplete.prototype._renderMenu = function (ul, items) {
        var self = this;
        var colheader = '';
        Object.keys(items[0])
  .forEach(function eachKey(key) {
      if (items[0][key] != undefined)
          colheader += '<th class="ref_ddl_th">' + items[0][key].split('|')[0] + '</th>';

  });
        ul.append("<table class='ref-ddl_table'><thead class='headCls invisible-scrollbar' style='display:block;'><tr class='ref_ddl_headers'>" + colheader + "</tr></thead><tbody class='bodyCls mostly-customized-scrollbar'></tbody></table>");
        $.each(items, function (index, item) {
            if (index != 0)//to avoid column header
                self._renderItemData(ul, ul.find("table tbody"), item);
        });
    };
    $.ui.autocomplete.prototype._renderItemData = function (ul, table, item) {
        return this._renderItem(table, item).data("ui-autocomplete-item", item);
    };
    $.ui.autocomplete.prototype._renderItem = function (table, item) {
        var tr = $("<tr class='ui-menu-item ref_ddl_rows' role='presentation'></tr>");
        var colval;
        for (var key in Object.keys(item)) {
            if (item[Object.keys(item)[key]] != undefined)
                if (Object.keys(item)[key].toLowerCase() == 'entity code|entity_code')
                    colval += "<td class='ref_ddl_columns'><div id=\"divEntityCodeId\"  style=\"color:#48a3dd;text-decoration:underline;cursor:pointer;\"><a EC='" + item["Entity Code|entity_code"] + "'>" + item[Object.keys(item)[key]] + "</a></div></td>";
                else if (Object.keys(item).indexOf('label') > -1 && Object.keys(item).indexOf('value') > -1 && item.label != undefined) {
                    if (Object.keys(item)[key].toLowerCase() == 'label')
                        colval += "<td class='ref_ddl_columns' ><a EC='" + item.id + "'>" + item.label + "</a></td>";
                }
                else
                    colval += "<td class='ref_ddl_columns'><a  EC='" + item["Entity Code|entity_code"] + "'>" + item[Object.keys(item)[key]] + '</a></td>';
        }
        tr.append(colval)
      .appendTo(table);
        return tr;
    };
    if (target.val() == null || target.val() == '')
        target.autocomplete("search", $('#' + id).val());
}
//function CallAutoCompleteServiceFiltered(id, dateTimeFormat, serviceServerString, methodName, autoCompleteError, autoCompleteSelect, _securityInfo, userContext, autoCompleteFilteredOpen, isFullSearch) {//, autoCompleteFocus, autoCompleteFocusOut, autoCompleteOpen
//    var target = $('#' + id);
//    target.unbind('change').change(autoCompleteChange);
//    target.autocomplete({
//        source: function (request, response) {
//            //target.autocomplete('option').isFullSearch
//            var a = new Object();
//            a.userContext = userContext; //$(this)[0].element.attr('id')
//            a.response = response;
//            var attributeName = target.attr('AN');
//            var groupDictionary = _securityInfo.DictConditionalFilter[target.attr('GN')];
//            var groupList = [];
//            var selectedConditionalFilterInfo = groupDictionary[attributeName];
//            var selectedTabType = selectedConditionalFilterInfo.TabType;
//            var selectedTabElementId = selectedConditionalFilterInfo.TabElementId;
//            var txtBox = $('#' + target.attr('replacedWith'));
//            var lstSplitIds = txtBox.prop('id').split('_');
//            for (var attributeNameKey in groupDictionary) {
//                var conditionalFilterInfo = groupDictionary[attributeNameKey];
//                if ((selectedTabType == SMSecurityAction.SingleInfo && selectedConditionalFilterInfo.SingleInfoHasConstituent) || selectedTabType == SMSecurityAction.MutipleInfo) {
//                    if (selectedTabElementId == conditionalFilterInfo.TabElementId) {
//                        var attributeElementId = '';
//                        for (var i = 0; i < lstSplitIds.length - 1; i++) {
//                            attributeElementId += lstSplitIds[i] + '_';
//                        }
//                        attributeElementId += conditionalFilterInfo.ColumnNumber;
//                        if (attributeElementId != txtBox.prop('id')) {
//                            var attributeElement = $('#' + attributeElementId);
//                            conditionalFilterInfo.Value = attributeElement.val();
//                            conditionalFilterInfo.EntityCode = attributeElement.attr('EC');
//                        }
//                        else {
//                            conditionalFilterInfo.Value = target.val();
//                            conditionalFilterInfo.EntityCode = target.attr('EC');
//                        }
//                    }
//                }
//                if (attributeName.toLowerCase() == attributeNameKey.toLowerCase()) {
//                    conditionalFilterInfo.StringSearch = request.term;
//                    a.conditionalFilterInfo = conditionalFilterInfo;
//                }
//                groupList[groupList.length] = conditionalFilterInfo;
//            }

//            a.groupDictionary = groupDictionary;

//            var parameters = {
//            };
//            parameters['userIdentifier'] = _securityInfo.SessionIdentifier;
//            parameters['actionType'] = SMConditionalFilterActionType.Filter;
//            parameters['attributeName'] = attributeName;
//            parameters['lstConditionalFilterInfo'] = groupList;
//            parameters['dateTimeFormat'] = dateTimeFormat;


//            callService('POST', serviceServerString, methodName, parameters, autoCompleteSuccessFiltered, autoCompleteError, response, a, false);
//        },
//        select: autoCompleteSelect,
//        minLength: 0,
//        cacheLength: 1,
//        delay: 0,
//        open: autoCompleteFilteredOpen,
//        isFullSearch: ((isFullSearch == null) ? false : isFullSearch)
//        //        ,
//        //        position: { my: "left bottom", at: "left top", collision: 'flip' }
//    }).unbind('click').click(function () { if ($(this).val() == null || $(this).val() == '') $(this).autocomplete("search", $('#' + id).val()) })
//    .data("autocomplete")._renderItem = function (ul, item) {
//        ul.removeAttr('selection');
//        return $("<li></li>")
//   .data("item.autocomplete", item)
//   .append("<a title='" + (item.label) + "' EC='" + item.id + "' >" + item.label + "</a>").appendTo(ul)
//    };
//    if (target.val() == null || target.val() == '')
//        target.autocomplete("search", $('#' + id).val());
//}


//function autoCompleteSuccess(data) {
//    var newSet = new Array();
//    var jason;
//    $.each(data.d, function (index, value) {
//        jason = new Object();
//        jason.label = value.split('-')[0];
//        jason.id = value.split('-')[1];
//        newSet[index] = jason;
//    });
//    response(newSet);
//}
//function autoCompleteError(data) {
//}
//function CallAutoComplete1() {
//    $('#txtRefHidden').focus(autoCompleteFocus);
//    $('#txtRefHidden').autocomplete({
//        source: function (request, response) {
//            var parameters = {};
//            parameters['term'] = request.term;
//            parameters['contextKey'] = $(this)[0].element.attr('context_key');
//            $.each(parameters, function (key, value) {
//                parameters[key] = JSON.stringify(value);
//            });
//            $.ajax({
//                type: "GET",
//                url: "http://localhost:1503/Services/SMCreateUpdateNew.svc/GetReferenceDataValues",
//                processData: true,
//                data: parameters,
//                contentType: "application/json; charset=utf-8",
//                dataType: "json",
//                success: function (data) {
//                    var newSet = new Array();
//                    var jason;
//                    $.each(data.d, function (index, value) {
//                        jason = new Object();
//                        jason.label = value.split('-')[0];
//                        jason.id = value.split('-')[1];
//                        newSet[index] = jason;
//                    });
//                    response(newSet);
//                },
//                error: function (data) {
//                },
//                userContext: response
//            })
//        },
//        select: function (e,i) {
//            e.target.entity_code = i.item.id;
//        },
//        minLength: 0,
//        cacheLength: 0
//    });
//}
//

//ACCORDION
function callAccordion(id, resizeAccordion) {
    var accordion = $('#' + id);
    var accordiopanels = accordion.children();
    var accHeaders = accordiopanels.filter(':even');
    var accBodies = accordiopanels.filter(':odd');
    accHeaders.each(function (index) {
        var header = $(this);
        var body = $(accBodies[index]);
        var headerChildDiv = header.children('div');
        var arrowDiv = headerChildDiv.children('div');
        headerChildDiv.click(function (e) {
            if (arrowDiv[0].className == 'CUAccordionRightArrow')
                arrowDiv[0].className = 'CUAccordionDownArrow';
            else if (arrowDiv[0].className == 'CUAccordionDownArrow')
                arrowDiv[0].className = 'CUAccordionRightArrow';
            body.toggle();
        });
    })
    //    $('#' + id).accordion({ autoHeight: false, collapsible: true, active: true, change: resizeAccordion });
    //    $('#' + id + ' .ui-accordion-content').show();
}
function accordionChangeIndex(id, index, eventHandler) {
    //    var accordion = $('#' + id);
    //    if (eventHandler != undefined && eventHandler != null)
    //        accordion.on("accordionchange", eventHandler);
    //    accordion.accordion('activate', parseInt(index));
}
//

//TAB
function callTab(id) {
    $('#' + id).tabs();
}


function tabChangeIndex(id, index) {
    $('#' + id).tabs('option', 'active', index);
}
//



//DATETIME

var optionDateTime = {
};
optionDateTime.DATE = "DateOnly";
optionDateTime.TIME = "TimeOnly";
optionDateTime.DATETIME = "DateTime";
optionDateTime.DATERANGE = "DateRange";


function getFormat(format, option) {
    var currentFormat = null;
    var arrFormat = format.split(' ');
    switch (option) {
        case optionDateTime.DATE:
            if (arrFormat != null && arrFormat.length > 0)
                currentFormat = arrFormat[0];
            else
                currentFormat = null;
            break;
        case optionDateTime.TIME:
            if (arrFormat != null && arrFormat.length > 1)
                currentFormat = arrFormat[1];
            else
                currentFormat = null;
            break;
            break;
        case optionDateTime.DATETIME:
            currentFormat = format;
            break;
    }
    return currentFormat;
}

/*  
Time formats:

**24 hr format**
H:i - hours and minutes 
H:i:s - hours minutes seconds
H:i:s a- hours minutes seconds am/pm

**12 hr format**
h:i - hours and minutes 
h:i:s - hours minutes seconds
h:i:s a- hours minutes seconds am/pm

format- shows the date in the given format in the target textbox
E.g- d/m/Y H:i a shows the date as 12/30/2014 20:13 pm
d/m/Y H:i A shows the date as 12/30/2014 20:13 PM

formatTime- show the time in given format in datepicker dropdown
if 12 hr format selected then time is shown from 12-11:59 and den again from 12:00 to 11:59 in same dropdown considering the step size given
*/
function CallCustomDatePicker(id, datetimeFormat, onSelectDate, option, stepSize, am, isToShow) {
    //option: date,time,datetime
    var target = $('#' + id);
    switch (option) {
        case optionDateTime.DATE:
            target.customDateTimePicker({
                timepicker: false,
                //dayOfWeekStart: 1,
                lang: 'en',
                formatdate: datetimeFormat,
                onChangeDateTime: onSelectDate,
                format: datetimeFormat,
                step: 15,
                type: option,
                zIndexStyle: 100006
            });

            break;
        case optionDateTime.TIME:
            datetimeFormat = datetimeFormat.replace('h', 'H');
            target.customDateTimePicker({
                datepicker: false,
                format: datetimeFormat,
                formatTime: datetimeFormat,
                onChangeTime: onSelectDate,
                onChangeDateTime: onSelectDate,
                step: 15,
                type: option,
                zIndexStyle: 100006
            });
            //target.datetimepicker();
            break;
        default:
            var formats = datetimeFormat.split(' ');
            var dateFormat = formats[0];
            var timeFormat = formats[1];
            target.customDateTimePicker({
                // dayOfWeekStart: 1,
                lang: 'en',
                format: datetimeFormat,
                formatTime: timeFormat,
                onChangeDateTime: onSelectDate,
                step: 15,
                formatdate: datetimeFormat + ((am == undefined || am == null || am == '' || am == false) ? '' : ' A'),
                type: option,
                zIndexStyle: 100006
            });
            //target.datetimepicker();
            break;
    }
    if (isToShow)
        target.customDateTimePicker('show');
}


function onCloseDatePicker(date, dateContext) {

}
function CallDatePicker(id, JQueryDateFormat, onSelectDate) {
    var target = $('#' + id);
    target.datepicker({
        dateFormat: JQueryDateFormat,
        onSelect: onSelectDate,
        showOtherMonths: true,
        selectOtherMonths: true,
        dayNamesMin: ['SUN', 'MON', 'TUE', 'WED', 'THU', 'FRI', 'SAT'],
        beforeShow: onShowDatePicker,
        onChangeMonthYear: onChangeDatePicker,
        changeYear: true,
        yearRange: "1753:9999"
    });
    target.unbind('keyup').keyup(onKeyUpDatePicker);
    target.unbind('change').change(dateChange);
    //    target.unbind('click').click(function (e) {
    //        target.datepicker('show');
    //    });
    //    target.unbind('dblclick').dblclick(function (e) {
    //        target.datepicker('show');
    //    });
}
function dateChange(e) {
    var target = $(e.target);
    if (target.attr('AD') != undefined && target.attr('AD').split('<@>')[9] == "1") {
        var attributeDetails = target.attr('AD').split('<@>');
        var attributeName = attributeDetails[2];
        var divQVBody = _controls.DivQVBody().find("div[id='divQVB_" + attributeName + "']");
        divQVBody.text(target.val());
    }

}
function onKeyUpDatePicker(e) {
    //    var target = $(e.target).datepicker('hide');
    //    datePickerInterval = setTimeout(function () { showDatePicker(target) }, 1);
}
function showDatePicker(target) {
    clearTimeout(datePickerInterval);
}
function onShowDatePicker(input, datePicker) {
    datePickerInterval = setTimeout(function () { onShowDatePickerTimer(input, datePicker) }, 0);
}
function onChangeDatePicker(year, month, datePicker) {
    var input = $(this);
    datePickerInterval = setTimeout(function () { onShowDatePickerTimer(input, datePicker) }, 0);
}
function onShowDatePickerTimer(input, datePicker) {
    datePicker.dpDiv.css('display', '');
    datePicker.dpDiv.removeClass();
    var firstChildDiv = datePicker.dpDiv.find('div');
    firstChildDiv.find('div').removeClass();
    datePicker.dpDiv.find('a').removeClass();
    datePicker.dpDiv.find('span').removeClass();
    datePicker.dpDiv.find('table').removeClass();
    datePicker.dpDiv.find('th').removeClass();
    datePicker.dpDiv.find('tr').removeClass();
    datePicker.dpDiv.find('.ui-datepicker-current-day').attr('current', '1');
    datePicker.dpDiv.find('.ui-datepicker-today').attr('today', '1');
    datePicker.dpDiv.find('td').removeClass();
    datePicker.dpDiv.find('thead').removeClass();
    datePicker.dpDiv.find('tbody').removeClass();


    datePicker.dpDiv.addClass('CUDatePicker');
    var childDiv = datePicker.dpDiv.children('div');
    childDiv.addClass('CUDateHeader');
    childDiv.children('div').addClass('CUDateHeaderText');
    childDiv.children('a').each(function (index) {

        $(this).children('span').remove();
        if (index == 0) {
            $(this).addClass('CUDatePrevA');
            $(this).append('<input type="button" class="CUDatePrevInput">');
        }
        else if (index == 1) {
            $(this).addClass('CUDateNextA');
            $(this).append('<input type="button" class="CUDateNextInput">');
        }
    });

    $('.CUDatePrevInput').unbind('click').click(function () { datePicker.dpDiv.css('display', 'none'); });
    $('.CUDateNextInput').unbind('click').click(function () { datePicker.dpDiv.css('display', 'none'); });

    var table = datePicker.dpDiv.children('table');
    table.attr('cellPadding', '0').attr('cellSpacing', '0').attr('border', '0').addClass('CUDateTable');
    var tableRows = table.find('tr');

    var currentMonth = datePicker.selectedMonth + 1;
    var prevMonth = currentMonth - 1;
    var nextMonth = currentMonth + 1;
    tableRows.each(function (rowIndex) {
        if (rowIndex == 0) {
            var th = $(this).find('th');
            th.addClass('CUDateDaysHeader').attr('align', 'center');
            th.first().addClass('CUDateDaysHeaderLeft');
        }
        else {
            $(this).addClass('CUDateDaysBodyRow');
            var tableCells = $(this).find('td');
            tableCells.last().addClass('CUDateDaysBodyRightCol');
            if (rowIndex == 1) {
                tableCells.addClass('CUDateDaysBodyTopRow');
                var lastMonthDatesA = $(this).find('a');
                var lastMonthDateIndex;
                switch (prevMonth) {
                    case 1:
                    case 3:
                    case 5:
                    case 7:
                    case 8:
                    case 10:
                    case 12:
                        lastMonthDatesA.each(function (cellIndex) {
                            var date = parseInt($(this).textNBR());
                            if (date == 31) {
                                lastMonthDateIndex = cellIndex;
                                return false;
                            }
                        });
                        break;
                    case 4:
                    case 6:
                    case 9:
                    case 11:
                        lastMonthDatesA.each(function (cellIndex) {
                            var date = parseInt($(this).textNBR());
                            if (date == 30) {
                                lastMonthDateIndex = cellIndex;
                                return false;
                            }
                        });
                        break;
                    case 2:
                        var leapYear = false;
                        if (datePicker.selectedYear % 100 == 0) {
                            if ((datePicker.selectedYear / 100) % 4 == 0) {
                                leapYear = true;
                            }
                            else {
                                var date = parseInt($(this).textNBR());
                                leapYear = false;
                            }
                        }
                        else {
                            if (datePicker.selectedYear % 4 == 0) {
                                var date = parseInt($(this).textNBR());
                                leapYear = true;
                            }
                            else {
                                leapYear = false;
                            }
                        }
                        if (leapYear) {
                            lastMonthDatesA.each(function (cellIndex) {
                                var date = parseInt($(this).textNBR());
                                if (date == 29) {
                                    lastMonthDateIndex = cellIndex;
                                    return false;
                                }
                            });
                        }
                        else {
                            lastMonthDatesA.each(function (cellIndex) {
                                var date = parseInt($(this).textNBR());
                                if (date == 28) {
                                    lastMonthDateIndex = cellIndex;
                                    return false;
                                }
                            });
                        }
                        break;
                }
                lastMonthDatesA.each(function (cellIndex) {
                    if (cellIndex >= 0 && cellIndex <= lastMonthDateIndex) {
                        $(this).addClass('CUDatePickerDateA CUDatePickerDateAOtherMonth');
                        $(this).parents('td:first').css('background-color', '#ededed');
                    }
                    else {
                        $(this).addClass('CUDatePickerDateA');
                    }
                });
            }
            else if (rowIndex == tableRows.length - 1) {
                tableCells.addClass('CUDateDaysBodyBottomRow');
                if (tableCells.length > 0)
                    $(tableCells[tableCells.length - 1]).addClass('CUDateDaysBodyLastCell');

                var nextMonthDatesA = $(this).find('a');
                var nextMonthDateIndex;
                nextMonthDatesA.each(function (cellIndex) {
                    var date = parseInt($(this).textNBR());
                    if (date == 1) {
                        nextMonthDateIndex = cellIndex;
                        return false;
                    }
                });
                nextMonthDatesA.each(function (cellIndex) {
                    if (cellIndex >= 0 && cellIndex < nextMonthDateIndex) {
                        $(this).addClass('CUDatePickerDateA');
                    }
                    else {
                        $(this).addClass('CUDatePickerDateA CUDatePickerDateAOtherMonth');
                        $(this).parents('td:first').css('background-color', '#ededed');
                    }
                });
            }
            else {
                $(this).find('a').addClass('CUDatePickerDateA');
            }
            $(this).find('td').attr('align', 'center');
        }
    });
    input = $(input);
    var inputLength = input.width() + parseInt(input.css('border-left-width') != null ? input.css('border-left-width').replace('px', '') : 0) + parseInt(input.css('border-right-width') != null ? input.css('border-right-width').replace('px', '') : 0) + parseInt(input.css('padding-left') != null ? input.css('padding-left').replace('px', '') : 0) + parseInt(input.css('padding-right') != null ? input.css('padding-right').replace('px', '') : 0);
    datePicker.dpDiv.width('216');
    datePicker.dpDiv.css('left', (input.offset().left + inputLength - datePicker.dpDiv.width()).toString() + 'px').css('z-index', '3');
    firstChildDiv.css('width', datePicker.dpDiv.width() + 'px');
    firstChildDiv.first().before($('<input type="button" class="CUDateTopArrow" />').css('left', (datePicker.dpDiv.width() - 15 - 15).toString() + 'px'));
    clearTimeout(datePickerInterval);
}
//
function LoadIPAddressNew() {
    var ipAddress = [];
    var allIPAddresses = LoadIPAddressString();
    if (allIPAddresses != null && allIPAddresses != '') {
        ipAddress = allIPAddresses.split('|');
    }
    return ipAddress;
    //    try {
    //        var ipAddress = new com.ivp.rad.raduiscripts.RIPAddressController();
    //        var allIPAddresses = ipAddress.getIPAddress();
    //    }
    //    catch ($e1) {
    //    }
    //    return allIPAddresses;
}

function LoadIPAddress(hdnSapiAddressID) {
    var HiddnSAPIAddress = null;
    if (hdnSapiAddressID != null && hdnSapiAddressID != '')
        HiddnSAPIAddress = GetObject(hdnSapiAddressID);
    var pipeSeparatedIPAddressList = '';
    var result = ExecuteSynchronously('./BaseUserControls/Service/Service.asmx', 'GetIPAddress', {}).d;
    if (result != null && result.length > 0) {
        if (result[0] == '0') {
            pipeSeparatedIPAddressList = sessionStorage['ipaddress'];
            if (pipeSeparatedIPAddressList != undefined && pipeSeparatedIPAddressList != null && pipeSeparatedIPAddressList != '')
                pipeSeparatedIPAddressList = pipeSeparatedIPAddressList;
            else {
                var ipAddress = new com.ivp.rad.raduiscripts.RIPAddressController();
                var allIPAddresses = ipAddress.getIPAddress();
                for (var i = 0; i < allIPAddresses.length; i++) {
                    if (i == 0)
                        pipeSeparatedIPAddressList = allIPAddresses[i].toString();
                    else
                        pipeSeparatedIPAddressList = pipeSeparatedIPAddressList + "|" + allIPAddresses[i].toString();
                }
                sessionStorage['ipaddress'] = pipeSeparatedIPAddressList;
            }
        }
        else if (result[0] == '1') {
            pipeSeparatedIPAddressList = result[1];
        }
    }
    if (HiddnSAPIAddress != null)
        HiddnSAPIAddress.value = pipeSeparatedIPAddressList;

    return pipeSeparatedIPAddressList;
}

function LoadIPAddressString() {
    return LoadIPAddress(null);
}

function getParentElement(element, parentTagName) {
    var parentElement = null;
    if (element != null) {
        parentElement = element.parentNode;
        if (parentElement != null) {
            while (parentElement.tagName.trim().toLowerCase() != parentTagName.trim().toLowerCase()) {
                parentElement = parentElement.parentNode;
            }
        }
    }
    return parentElement;
}

function IsStringNullOrEmpty(value) {
    var flag = false;
    if (value == null || value == '')
        flag = true;
    return flag;
}

function contains(main, toFind) {
    var regEx = null;
    regEx = new RegExp(toFind, 'gi');
    return regEx.test(main);
}

function isNullOrEmpty(value) {
    var flag = true;
    if (value != null && value != SecMasterJSCommon.SMSConstants.strinG_EMPTY) {
        flag = false;
    }
    return flag;
}

function checkJqueryStringWithRegExOrString(element, expressionArray, ifRegEx, isCaseSensitive) {
    var flag = false;
    if (element.length != 0) {
        var jQueryElement = $(element[0]);
        var value = null;
        if (jQueryElement[0].nodeName.toLowerCase() == 'input')
            value = jQueryElement.val();
        else
            value = jQueryElement.textNBR();

        if (ifRegEx && !isNullOrEmpty(value)) {
            var regEx = isCaseSensitive ? new RegExp(expressionArray[0].toString()) : new RegExp(expressionArray[0].toString(), "i");
            flag = regEx.test(value);
        }
        else {
            expressionArray[expressionArray.length] = '';
            for (var i = 0; i < expressionArray.length; i++)
                if ((isCaseSensitive && value == expressionArray[i].toString()) || (!isCaseSensitive && value.toLowerCase() == expressionArray[i].toString().toLowerCase())) {
                    flag = true;
                    break;
                }
            expressionArray.splice(expressionArray.length - 1, 1);
        }
    }
    return flag;
}

function ConvertShortDateFormatLiteralToJQuery(input) {
    input = YearMapping(input);
    input = MonthMapping(input);
    input = DayMapping(input);
    return input;
}

function YearMapping(input) {
    var regex = null;
    regex = new RegExp('yyyy', 'g');
    if (regex.test(input)) {
        input = input.replace('yyyy', 'yy');
        return input;
    }
    regex = new RegExp('yy', 'g');
    if (regex.test(input)) {
        input = input.replace('yy', 'y');
        return input;
    }
    return input;
}

function MonthMapping(input) {
    var regex = null;
    regex = new RegExp('MMMM', 'g');
    if (regex.test(input)) {
        input = input.replace('MMMM', 'MM');
        return input;
    }
    regex = new RegExp('MMM', 'g');
    if (regex.test(input)) {
        input = input.replace('MMM', 'M');
        return input;
    }
    regex = new RegExp('MM', 'g');
    if (regex.test(input)) {
        input = input.replace('MM', 'mm');
        return input;
    }
    regex = new RegExp('M', 'g');
    if (regex.test(input)) {
        input = input.replace('M', 'm');
        return input;
    }
    return input;
}

function DayMapping(input) {
    var regex = null;
    regex = new RegExp('dddd', 'g');
    if (regex.test(input)) {
        input = input.replace('dddd', 'DD');
        return input;
    }
    regex = new RegExp('ddd', 'g');
    if (regex.test(input)) {
        input = input.replace('ddd', 'D');
        return input;
    }
    return input;
}

var gsMonthNames = new Array('January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December');

var gsDayNames = new Array('Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday');

Date.prototype.format = function (f) {
    if (!this.valueOf())
        return ' ';

    var d = this;

    return f.replace(/(yyyy|yy|mmmm|mmm|mm|m|dddd|ddd|dd|d|hh|tfh|nn|ss|fff|tt|a\/p)/gi,
                function ($1) {
                    switch ($1.toLowerCase()) {
                        case 'yyyy': return d.getFullYear();
                        case 'yy': return d.getYear();
                        case 'mmmm': return gsMonthNames[d.getMonth()];
                        case 'mmm': return gsMonthNames[d.getMonth()].substr(0, 3);
                        case 'mm': return (d.getMonth() + 1).zf(2);
                        case 'm': return (d.getMonth() + 1);
                        case 'dddd': return gsDayNames[d.getDay()];
                        case 'ddd': return gsDayNames[d.getDay()].substr(0, 3);
                        case 'dd': return d.getDate().zf(2);
                        case 'd': return d.getDate();
                        case 'hh': return ((h = d.getHours() % 12) ? h : 12).zf(2);
                        case 'tfh': return d.getHours().zf(2);
                        case 'nn': return d.getMinutes().zf(2);
                        case 'ss': return d.getSeconds().zf(2);
                        case 'fff': return d.getMilliseconds();
                        case 'a/p': return d.getHours() < 12 ? 'a' : 'p';
                        case 'tt': return d.getHours() < 12 ? 'AM' : 'PM';

                    }
                }
            );
}
String.prototype.zf = function (l) {
    var initial = '';
    for (var i = l; i > this.length; i--)
        initial = '0' + initial;
    return initial + this;
}
Number.prototype.zf = function (l) {
    return this.toString().zf(l);
}


function ToggleVisibility(id, iconCell) {
    var control = $("#" + id);
    var expandCollapseCell = $("#" + iconCell);
    if (control.is(":visible")) {
        control.hide();
        $("#" + iconCell).removeClass("GridCollapseButton").addClass("GridExpandButton");
    }
    else {
        control.show();

        $("#" + iconCell).removeClass("GridExpandButton").addClass("GridCollapseButton");
    }
}


function KeyValuePair() {
    this.Key = {
    };
    this.Value = {
    };
};
KeyValuePair.prototype = {
    Key: {}, Value: {}
};

function WriteToFile(hdnMessage, eventMessage) {
    //return null;
    var fso;
    try {
        var txtMessage;
        if (hdnMessage == null) {
            txtMessage = eventMessage;
        }
        else {
            txtMessage = document.getElementById(hdnMessage).value;
            document.getElementById(hdnMessage).value = '';
        }
        fso = new ActiveXObject("Scripting.FileSystemObject");
        if (fso.DriveExists("c")) {
            if (!fso.FolderExists("c:\\SecurityMaster"))
                fso.CreateFolder("c:\\SecurityMaster\\");
            var aFile;
            if (fso.FileExists("c:\\SecurityMaster\\Log.txt"))
                aFile = fso.OpenTextFile("c:\\SecurityMaster\\Log.txt", 8, false);
            else
                aFile = fso.CreateTextFile("C:\\SecurityMaster\\Log.txt", true);

            //got the file object in aFile;
            if (txtMessage.split(';').length == 0) {
                aFile.WriteLine(txtMessage);
            }
            else {
                for (i = 0; i < txtMessage.split(';').length; i++) {
                    msgLine = txtMessage.split(';')[i];
                    aFile.WriteLine(msgLine);
                }
            }

            //aFile.WriteLine(new Date());
            aFile.Close();
        }
    }
    catch (E) {
    }
    finally {
        fso = null;
    }
}

function SearchGrid(searchText, gridParentId) {
    //value int he search textbox
    searchText = document.getElementById(searchText).value.toLowerCase();
    //body object
    var objGrid = document.getElementById(gridParentId).childNodes[0].childNodes[0];



    for (var rowCounter = 1; rowCounter < objGrid.childNodes.length; rowCounter++) {
        var row = objGrid.childNodes[rowCounter];

        if (row.nodeName.toLowerCase() == "tr") {
            if (row.innerText.toLowerCase().indexOf(searchText, 0) > -1) {
                row.style.display = "block";
            }
            else {
                row.style.display = "none";
            }

        }
    }

}

function GetSeparatorStringFromArray(lstValues, seperator) {
    var strResult = '';
    if (lstValues != null && lstValues != undefined && lstValues instanceof Array && lstValues.length > 0) {

        for (var i = 0; i < lstValues.length; i++) {
            if (i < lstValues.length - 1)
                strResult += lstValues[i] + seperator;
            else
                strResult += lstValues[i];
        }
    }
    return strResult;
}

$.fn.numericFilter = function (option) {
    //    var target = $(this);
    //    var suffix = null;
    //    var prefix = null;
    //    var onKeyPressText = null;
    //    var onKeyDownText = null;
    //    var onKeyUpText = null;
    //    var onFocusOutText = null;
    //    var noOfValidComma = 0;

    //    if (option != undefined && option != null) {
    //        if (option.suffix != undefined && option.suffix != null)
    //            suffix = isNaN(parseInt(option.suffix)) ? null : parseInt(option.suffix);
    //        if (option.prefix != undefined && option.prefix != null)
    //            prefix = isNaN(parseInt(option.prefix)) ? null : parseInt(option.prefix);
    //        noOfValidComma = prefix % 3 == 0 ? ((prefix / 3) - 1) : ((prefix / 3) + 1);
    //    }
    //    if (suffix == null)
    //        suffix = isNaN(parseInt(target.attr('suffix'))) ? null : parseInt(target.attr('suffix'));
    //    if (prefix == null)
    //        prefix = isNaN(parseInt(target.attr('prefix'))) ? null : parseInt(target.attr('prefix'));
    //    if (suffix != null && prefix != null) {
    //        if (target[0].onkeypress != undefined && target[0].onkeypress != null) {
    //            onKeyPressText = target[0].onkeypress.toString().replace('function onkeypress()', '').replace('this', 'target[0]');
    //            //target[0].onfocusout = null;
    //        }

    //        if (target[0].onkeydown != undefined && target[0].onkeydown != null) {
    //            onKeyDownText = target[0].onkeydown.toString().replace('function onkeydown()', '').replace('this', 'target[0]');
    //            //target[0].onfocusout = null;
    //        }

    //        if (target[0].onkeyup != undefined && target[0].onkeyup != null) {
    //            onKeyUpText = target[0].onkeyup.toString().replace('function onkeyup()', '').replace('this', 'target[0]');
    //            //target[0].onfocusout = null;
    //        }

    //        if (target[0].onfocusout != undefined && target[0].onfocusout != null) {
    //            onFocusOutText = target[0].onfocusout.toString().replace('function onfocusout()', '').replace('this', 'target[0]');
    //            target[0].onfocusout = null;
    //        }

    //        target.unbind('keypress', keyPress).keypress(keyPress);
    //        target.unbind('keydown', keyDown).keydown(keyDown);
    //        target.unbind('keyup', keyUp).keyup(keyUp);
    //        target.unbind('focusout', focusOut).focusout(focusOut);
    //    }
    //    function getCaretPosition() {
    //        var caretPos = 0;
    //        if (document.selection) {
    //            var range = document.selection.createRange();
    //            range.moveStart('character', -target[0].value.length);
    //            caretPos = range.text.length;
    //        }
    //        else if (target[0].selectionStart || target[0].selectionStart == '0')
    //            caretPos = target[0].selectionStart;
    //        return (caretPos);
    //    }

    //    function checkWhenNoDecimal(isMinus, caretPosition, targetValue) {

    //        var returnVal = true;
    //        var caretPositionLocal = caretPosition;
    //        if (isMinus)
    //            caretPositionLocal = caretPosition - 1;

    //        var prefixText = targetValue.substring(0, caretPositionLocal);
    //        var suffixText = targetValue.substring(caretPositionLocal, targetValue.length);
    //        if (suffixText.length <= suffix && prefixText.length <= prefix)
    //            returnVal = true;
    //        else
    //            returnVal = false;

    //        return returnVal;
    //    }

    //    function focusOut(e) {
    //        validate(e);
    //        if (onFocusOutText != null && onFocusOutText != '')
    //            eval(onFocusOutText);
    //    }

    //    function keyUp(e) {
    //        validate(e);
    //    }

    //    function validate(e) {
    //        if ((e.type == 'keyup' && e.ctrlKey && e.keyCode == 86) || e.type == 'focusout' || e.type == 'blur') {
    //            var value = target.val();

    //            var matchInvalidChars = value.match(new RegExp('[^0-9-.,]', 'gi'));
    //            if (matchInvalidChars != null && matchInvalidChars.length > 0) {
    //                target.val('');
    //                value = '';
    //                return;
    //            }

    //            var isMinus = value.charAt(0).indexOf('-') != -1;
    //            var minusMatch = value.match(new RegExp('[-]', 'gi'));
    //            var decimalMatch = value.match(new RegExp('[.]', 'gi'));

    //            if ((isMinus && minusMatch.length > 1) || (!isMinus && minusMatch != null && minusMatch.length > 0))// checks if value contains minus at invalid position
    //            {
    //                target.val('');
    //                value = '';
    //                return;
    //            }

    //            if (decimalMatch != null && decimalMatch.length > 1) {
    //                target.val('');
    //                value = '';
    //                return;
    //            }

    //            if (value == '0' || value == '')
    //            { }
    //            else {
    //                var valueSections = value.split('.');
    //                if (valueSections[0].replace('-', '').replace(/^0+/, '') == '') {// if all digits before decimal were 0s
    //                    value = '0' + ((valueSections.length > 1 && valueSections[1] != '') ? ('.' + valueSections[1]) : '');
    //                }
    //                else {
    //                    value = value.replace('-', '').replace(/^0+/, '');
    //                }
    //                value = (isMinus ? '-' : '') + value;
    //                target.val(value);
    //            }
    //            //  value = (isMinus ? '-' : '') + ((value == '0' || value == '') ? value : (value.split('.')[0].replace('-', '').replace(/^0+/, '') == '') ? ('0' + ((value.split('.').length > 1 && value.split('.')[1] != '') ? ('.' + value.split('.')[1]) : '')) : value.replace('-', '').replace(/^0+/, ''));

    //            if (target.val().indexOf('.') != -1) {
    //                value = value.replace(/0+$/, '');
    //                target.val(value);
    //            }

    //            var indexOfDecimal = target.val().indexOf('.');
    //            if (indexOfDecimal != -1) {
    //                var suffixText = target.val().split('.')[1];
    //                if (suffixText == null || suffixText == '') {
    //                    value = target.val().replace('.', '');
    //                    target.val(value);
    //                }
    //            }

    //            if (value != '') {
    //                value = parseFloat(value.replace(/,+/g, ''));
    //                if (isNaN(value)) {
    //                    value = '0';
    //                }
    //                target.val(value);
    //            }

    //            //if ((!(isMinus && minusMatch.length > 1) && e.type != 'focusout' && value == '') || value == '0.' || value == '-0' || value == '-0.')

    //            var caretPos = indexOfDecimal != -1 ? indexOfDecimal : target.val().length;
    //            var targetValue = target.val().replace('-', '').replace('.', '');

    //            if (!checkWhenNoDecimal(isMinus, caretPos, targetValue))
    //                target.val('');

    //            var value = target.val();
    //            value = ConvertToCurrencyFormat(value.replace(/,+/g, ''));
    //            target.val(value);
    //        }
    //    }

    //    function keyDown(e) {
    //        var charcode = e.keyCode;
    //        var targetValue = target.val();
    //        var decimalPos = targetValue.indexOf('.');
    //        var pos = getCaretPosition();
    //        var decimalCaretPos = decimalPos + 1;
    //        var isMinus = target.val().indexOf('-') != -1;
    //        var targetValue = target.val().replace('-', '');
    //        var returnValue = true;
    //        if (decimalPos > -1) {
    //            if (charcode == 46)//delete
    //            {
    //                if (decimalPos == pos) {
    //                    var value = targetValue.replace('.', '');

    //                    returnValue = checkWhenNoDecimal(isMinus, targetValue.length, value);
    //                }
    //            }
    //            else if (charcode == 8)//backspace
    //            {
    //                if (decimalCaretPos == pos) {
    //                    var value = targetValue.replace('.', '');

    //                    returnValue = checkWhenNoDecimal(isMinus, targetValue.length, value);
    //                }
    //            }
    //        }

    //        if (!returnValue) {
    //            e.preventDefault();
    //        }

    //    }

    //    function keyPress(e) {
    //        // var charCode = e.keyCode;
    //        var charCode = e.which;
    //        var returnVal = true;
    //        var pos = getCaretPosition();

    //        var decimalPos = target.val().indexOf('.');
    //        var currentCommaLength = target.val().match(new RegExp('[,]', 'gi'));
    //        var targetValue = target.val().replace('-', '').replace(/,+/g, '');
    //        var isMinus = target.val().indexOf('-') != -1;
    //        var isComma = false;

    //        //ON KEYPRESS
    //        // (48-57)-digits
    //        //46 decimal,
    //        //45 '-'
    //        //44 ','
    //        if (charCode == 44 || charCode == 45 || charCode == 46 || (charCode >= 48 && charCode <= 57)) {
    //            if (charCode == 45) {
    //                if (isMinus)
    //                    returnVal = false;
    //                else if (pos == 0)
    //                    returnVal = true;
    //                else
    //                    returnVal = false;
    //            }
    //            else {

    //                if (decimalPos == -1) {
    //                    if (charCode != 46) { //if digits
    //                        if (charCode == 44) {
    //                            if (currentCommaLength != null && currentCommaLength != undefined && currentCommaLength.length >= noOfValidComma)
    //                                returnVal = false;
    //                        }
    //                        else if (targetValue.replace(/,+/, '').length >= prefix)
    //                            returnVal = false;
    //                    }
    //                    else { // if decimal           

    //                        returnVal = checkWhenNoDecimal(isMinus, pos, targetValue);
    //                    }
    //                }
    //                else {
    //                    if (charCode == 46)
    //                        returnVal = false;
    //                    else {
    //                        var value = targetValue.split('.');
    //                        if (pos < decimalPos + 1) { //if inetergral part to be modified
    //                            if (value[0].replace(/,+/, '').length >= prefix)
    //                                returnVal = false;
    //                        }
    //                        else if (pos > decimalPos) {
    //                            if (value[1].replace(/,+/, '').length >= suffix)
    //                                returnVal = false;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        else
    //            returnVal = false;
    //        if (!returnVal) {
    //            e.preventDefault();
    //        }
    //        else {
    //            //            if (targetValue != null && targetValue != '') {
    //            //                var val = ConvertToCurrencyFormat(targetValue.replace(',', ''));
    //            //                target.val(val);
    //            //            }
    //        }
    //    }
}

$(document).keypress(onkeyPressDocument);
function onkeyPressDocument(e) {
    if (e.keyCode == 13) {
        var target = $(e.target);
        if (target.length > 0 && target.prop('tagName').toUpperCase().toUpperCase() == 'INPUT' && target.prop('type').toUpperCase() == 'SUBMIT') {
        }
        else {
            var btnEnterDisable = $('#btnEnterDisable');
            if (btnEnterDisable.length > 0)
                return WebForm_FireDefaultButton(event, btnEnterDisable.prop('id'));
        }
    }
}

function IsJqueryAttributeNodeNullOrEmpty(jQueryElement, attributeName) {
    var flag = false;
    if (jQueryElement != null && jQueryElement.length != 0) {
        if (jQueryElement.attr(attributeName) != null && jQueryElement.attr(attributeName) != undefined && jQueryElement.attr(attributeName) != '') {
            flag = false;
        }
        else
            flag = true;
    }
    else {
        flag = true;
    }
    return flag;
}

$.fn.textNBR = function () {
    var regEx = new RegExp(String.fromCharCode(160), 'gi');
    var normalSpace = ' ';
    var tempText = this.text().replace(regEx, normalSpace);
    //var result = new RegExp(String.fromCharCode(160), 'gi').test(tempText);
    return tempText;
};

$.fn.valueNBR = function () {
    var regEx = new RegExp(String.fromCharCode(160), 'gi');
    var normalSpace = ' ';
    var tempValue = this.val().replace(regEx, normalSpace);
    //var result = new RegExp(String.fromCharCode(160), 'gi').test(tempText);
    return tempValue;
};

String.prototype.replaceNBR = function () {
    var regEx = new RegExp(String.fromCharCode(160), 'gi');
    var normalSpace = ' ';
    return this.replace(regEx, normalSpace);
};

$.fn.selectRange = function (start, end) {
    return this.each(function () {
        if (this.setSelectionRange) {
            this.focus();
            this.setSelectionRange(start, end);
        } else if (this.createTextRange) {
            var range = this.createTextRange();
            range.collapse(true);
            range.moveEnd('character', end);
            range.moveStart('character', start);
            range.select();
        }
    });
};

function getScrollBarWidthCommon() {
    var inner = document.createElement('p');
    inner.style.width = "100%";
    inner.style.height = "200px";

    var outer = document.createElement('div');
    outer.style.position = "absolute";
    outer.style.top = "0px";
    outer.style.left = "0px";
    outer.style.visibility = "hidden";
    outer.style.width = "200px";
    outer.style.height = "150px";
    outer.style.overflow = "hidden";
    outer.appendChild(inner);

    document.body.appendChild(outer);
    var w1 = inner.offsetWidth;
    outer.style.overflow = 'scroll';
    var w2 = inner.offsetWidth;
    if (w1 == w2) w2 = outer.clientWidth;
    document.body.removeChild(outer);
    return (w1 - w2);
};
function getWidthForRuleEditor() {
    var width = $("[id$=divNewContainer]").width();
    var scrollBarWidth = getScrollBarWidthCommon();
    var leftPanelWizard = $('#divLeftPanel');
    var leftPanelWizardWidth = 0;
    if (leftPanelWizard.length > 0) {
        leftPanelWizardWidth = leftPanelWizard.width();
    }
    var widthComputed = width - scrollBarWidth - leftPanelWizardWidth - 19;
    if (widthComputed < 1200)
        return 1200;
    else
        return widthComputed;
}

function ruleEditorWidth() {
    return $('#aaa').parent().width();
}

function isElementDisabled(element) {
    if (element.attr('disabled') == undefined || (element.attr('disabled') != undefined && (element.attr('disabled') != 'disabled' || element.attr('disabled') == 'false')))
        return false;
    else
        return true;
}

function outerHTML(element) {
    var outerHTML = '';
    if (element != null) {
        if (element.nodeName.toUpperCase() !== '#TEXT') {
            changeHTML(element);
            updateHTML(element);
            outerHTML = element.outerHTML;
        }
    }
    return outerHTML;
}

function innerHTML(element) {
    var innerHTML = '';
    if (element != null) {
        if (element.nodeName.toUpperCase() !== '#TEXT') {
            changeHTML(element);
            updateHTML(element);
            innerHTML = element.innerHTML;
        }
    }
    return innerHTML;
}

function updateHTML(parent) {
    for (var i = 0; i < parent.childNodes.length; i++) {
        var element = parent.childNodes[i];
        if (element != null) {
            if (element.nodeName.toUpperCase() !== '#TEXT') {
                changeHTML(element);
                updateHTML(element);
            }
        }
    }
}

function changeHTML(element) {
    switch (element.tagName.toUpperCase()) {
        case 'INPUT':
            switch (element.type.toUpperCase()) {
                case 'RADIO':
                case 'CHECKBOX':
                    if (element.checked) {
                        element.setAttribute('checked', 'checked');
                    }
                    else {
                        element.removeAttribute('checked');
                    }
                    break;
                case 'TEXT':
                case 'HIDDEN':
                case 'BUTTON':
                case 'SUBMIT':
                    element.defaultValue = element.value;
                    break;
            }
            break;
        case 'SELECT':
            for (var j = 0; j < element.options.length; j++) {
                if (element.options[j].selected) {
                    element.options[j].setAttribute('selected', 'selected');
                }
                else {
                    element.options[j].removeAttribute('selected');
                }
            }
            break;
    }
}

$.fn.outerHTML = function () {
    var target = $(this);
    return outerHTML(target[0]);
}
$.fn.innerHTML = function () {
    var target = $(this);
    return innerHTML(target[0]);
}



function isAnyCategoryAvailable(key) {
    var chkCategories = $('#divChkCategory_' + key);
    return chkCategories.length > 0 ? true : false;
}

function getSelectedCategories(key) {
    var categoryDiv = $('#divCategory_' + key);
    var lstLicenseCategorySelecetd = [];
    var lstCheckedCategories = categoryDiv.find('div[class=exceptionManagerChecked]');
    for (var mkt = 0; mkt < lstCheckedCategories.length; mkt++) {
        var marketSectorChkBox = $(lstCheckedCategories[mkt]);
        var idTemp = marketSectorChkBox.prop('id');
        var categoryNamePart = idTemp.split('_')[1];
        var categoryId = idTemp.split('_')[2]
        var category = categoryDiv.find('div[id$=divCategory_' + categoryNamePart + '_' + categoryId + ']').text().trim();
        var feedId = marketSectorChkBox.attr('feedId');

        var categoryInfo = {
        };
        categoryInfo.CategoryName = category;
        categoryInfo.LstFeedId = feedId.split(',');
        categoryInfo.CategoryId = categoryId;
        lstLicenseCategorySelecetd.push(categoryInfo);
    }
    return lstLicenseCategorySelecetd;
}

function getDefaultLoginUrl() {
    var hdnDefaultLoginUrl = $('[id*=hdnDefaultLoginUrl]');
    return hdnDefaultLoginUrl.length > 0 ? hdnDefaultLoginUrl.val() : 'SMSearch.aspx?identifier=Search';
}

function repositionPopup(div) {
    var viewPortHeight = document.documentElement.clientHeight;
    var viewPortWidth = document.documentElement.clientWidth;
    var divHeight = div.offsetHeight;
    var divWidth = div.offsetWidth;
    if (divHeight > viewPortHeight)
        $(div).outerHeight(viewPortHeight);
    div.style.top = ((viewPortHeight - divHeight) / 2).toString() + "px";
    div.style.left = ((viewPortWidth - divWidth) / 2).toString() + "px";
}

function ConvertCurrencyToISOFormat(input) {
    var output = '';
    if (!isNullOrEmpty(input) && input.constructor == String) {
        output = input.replace(new RegExp(',', 'g'), '');
    }
    return output
}

function ConvertToCurrencyFormat(input) {
    if (isNullOrEmpty(input))
        return input;

    var lstIndexOfDecimal = input.lastIndexOf('.');
    var beforeDecimal = '';
    var afterDecimal = '';
    var output = '';
    if (lstIndexOfDecimal != -1) {
        beforeDecimal = input.substring(0, lstIndexOfDecimal).replace('.', '');
        afterDecimal = input.substring(lstIndexOfDecimal);
    }
    else
        beforeDecimal = input;

    var counter = 1;
    for (var i = 0; i < beforeDecimal.length; i++) {
        var char = beforeDecimal.charAt(i);
        if (isNaN(parseInt(char)))
            continue;

        output += char;
    }

    beforeDecimal = output;
    output = '';

    for (var i = (beforeDecimal.length - 1) ; i >= 0; i--) {
        var char = beforeDecimal.charAt(i);
        if (counter != 1 && counter != beforeDecimal.length && counter % 3 == 0)
            output = ',' + char + output;
        else
            output = char + output;
        counter++;
    }
    output += afterDecimal;
    return output;
}

function OpenNewSecMasterWindow(searchTxtClientID, validate) {
    var searchText = document.getElementById(searchTxtClientID).value;
    if (searchText != '' && (event.keyCode == 13 || validate == false)) {
        var childWindow = [];
        event.returnValue = false;
        event.cancel = true;
        var randomnumber = Math.floor(Math.random() * 11);
        //For Lite:appended mode=nw
        var URL = 'SMSearch.aspx?identifier=Search&searchText=' + encodeURI(searchText) + '&UniquePageIdentifier=' + randomnumber + '&mode=nw';
        var properties = 'toolbars=no,menubar=no,location=no,scrollbars=yes,resizable=yes,status=no,height=700px,width=1000px';

        childWindow[childWindow.length] = window.open(URL, '', properties);
        //        if (!TestParentWindowOpen()) {
        //            childWindow[childWindow.length] = window.open(URL, '', properties);
        //        }
    }
}


function compareDatesWithTime(date1, date2) {
    /*returns -1 if date1 is less than date2
    returns 0 if date1 is equal to date2
    returns 1 if date1 is greator than date2
    */

    var d1 = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertStringToDateTime(date1, com.ivp.rad.rscriptutils.DateTimeFormat.longDate)//new Date(date1);
    var d2 = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertStringToDateTime(date2, com.ivp.rad.rscriptutils.DateTimeFormat.longDate)//new Date(date2);
    var tick1 = d1.getTime();
    var tick2 = d2.getTime();
    if (tick1 < tick2)
        return -1;
    else if (tick1 == tick2)
        return 0;
    else
        return 1;
}

function disableDivs(zindex) {
    var disableDiv = $get('disableDiv');
    disableDiv.style.display = '';
    var scrollHeight = document.body.scrollHeight;
    var scrollWidth = document.body.scrollWidth;
    windowHeight = document.body.parentNode.clientHeight;
    if (windowHeight > scrollHeight)
        scrollHeight = windowHeight;
    disableDiv.style.height = scrollHeight + "px";
    disableDiv.style.width = scrollWidth + "px";
    if (zindex != null && zindex != '') {
        disableDiv.setAttribute('pzindex', zindex);
        disableDiv.style.zIndex = zindex;
    }
}

function enableDivs() {
    var disableDiv = $get('disableDiv');
    if (disableDiv.getAttributeNode('pzindex') != null) {
        disableDiv.style.zIndex = disableDiv.getAttributeNode('pzindex').value;
        disableDiv.removeAttribute('pzindex');
    }
    disableDiv.style.display = 'none';
}

function Select2JSCall(targetElementID) {
    $('[id$="' + targetElementID + '"]').select2({ minimumResultsForSearch: Infinity });
    $('#select2-results-1').smslimscroll({ height: '150px', railVisible: true, alwaysVisible: true });
}

function SelectizeJSCall(targetElementID) {
    targetElementID.selectize();
    targetElementID.next().children('div:nth-child(2)').find('div').smslimscroll({ height: '200px', railVisible: true, alwaysVisible: true, position: 'right', distance: '10px' });
}

/*Edited By Sahil*/
function SetIframeSrc(id, src) {
    document.getElementById(id).src = src;
    $('#' + id).unbind('load').bind('load', function () {
        if (src.indexOf('SMDeleteSecurity.aspx') === 0) {
            document.getElementById(id).height = 600; /*Height Settings for Chrome*/
        }
        if (src.indexOf('SMLookUp.aspx') === 0) {
            document.getElementById(id).height = 300; /*Height Settings for Chrome*/
        }
        else {
            document.getElementById(id).height = document.getElementById(id).contentWindow.document.getElementsByTagName("body")[0].offsetHeight + 10; /*Height Settings for Chrome*/
        }
        $('#' + id).contents().find('[id$="divWrapperContainer"]').unbind('DOMSubtreeModified').bind('DOMSubtreeModified', function () { /*Height Settings for IE*/
            document.getElementById(id).height = $(this).outerHeight(true) + 10;
        });

        $('#' + id).contents().find('[id$="UnderlyingSecurity"]').unbind('DOMSubtreeModified').bind('DOMSubtreeModified', function () { /*Height Settings for IE Derived Security*/
            document.getElementById(id).height = $(this).outerHeight(true) + 10;
        });
    });
}

/*Edited By Sahil*/
function SetBlankIframeURL(iframeId) {
    document.getElementById(iframeId).src = '';
}

function getTopMostParent() {
    var secMparent = window.parent;
    while ((secMparent.leftMenu == null || typeof secMparent.leftMenu === 'undefined')) {
        if (secMparent.parent != secMparent)
            secMparent = secMparent.parent;
        else
            break;
    }
    return secMparent;

    //    var parent = window.parent;
    //    while (!(parent === parent.parent)) {
    //        if (parent.parent == null)
    //            break;
    //        parent = parent.parent;
    //    }
    //    return parent;
}

function getIdentifierMarketSectorString(identifierValue) {
    var obj = SecMasterJSCommon.SMSCommons.getMarketSector(identifierValue);
    return obj.IdentifierValue.toUpperCase() + ((obj.MarketSector === '') ? '' : ' ' + obj.MarketSector);
}

function getServerDate() {
    // var dateSet;
    return $.ajax({
        type: "POST",
        url: path + "/BaseUserControls/Service/CommonService.asmx/GetServerDate",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        //success: function (r) {
        //    dateSet = r.d;
        //    return dateSet;
        //    //  alert(r.d);
        //},
        //error: function (r) {
        //    // alert(r.responseText);
        //},
        //failure: function (r) {
        //    // alert(r.responseText);
        //}
    });

}

//; (function ($) {
//    //Global plugin settings
//    var settings = {
//        'animationSpeed': 100, //The speed in which the tabs will animate/scroll
//        'closable': false, //Make tabs closable
//        'resizable': false, //Alow resizing the tabs container
//        'resizeHandles': 'e,s,se', //Resizable in North, East and NorthEast directions
//        'loadLastTab': false, //When tabs loaded, scroll to the last tab - default is the first tab
//        'easing': 'swing' //The easing equation
//    }

//    $.fn.scrollabletab = function (options) {
//        //Check if scrollto plugin is available - (pasted the plugin at the end of this plugin)
//        //if(!$.fn.scrollTo) return alert('Error:\nScrollTo plugin not available.');

//        return this.each(function () {
//            var o = $.extend({}, settings, options), //Extend the options if any provided
//			$tabs = $(this),
//			$tabsNav = $tabs.find('.ui-tabs-nav'),
//			$nav; //will save the refrence for the wrapper having next and previous buttons

//            //Adjust the css class
//            //$tabsNav.removeClass('ui-corner-all').addClass('ui-corner-top');
//            $tabs.css({ 'padding': 2, 'position': 'relative' });
//            //$tabsNav.css('position','inherit');

//            //Wrap inner items
//            $tabs.wrap('<div id="stTabswrapper" class="stTabsMainWrapper" style="position:relative"/>').find('.ui-tabs-nav').css('overflow', 'hidden').wrapInner('<div class="stTabsInnerWrapper" style="width:30000px"><span class="stWidthChecker"/></div>');

//            var $widthChecker = $tabs.find('.stWidthChecker'),
//				$itemContainer = $tabs.find('.stTabsInnerWrapper'),
//				$tabsWrapper = $tabs.parents('#stTabswrapper').width($tabs.outerWidth(true));
//            //Fixing safari bug
//            if ($.browser.safari) {
//                $tabsWrapper.width($tabs.width() + 6);
//            }
//            //alert($tabsWrapper.width());
//            if (o.resizable) {
//                if (!!$.fn.resizable) {
//                    $tabsWrapper.resizable({
//                        minWidth: $tabsWrapper.width(),
//                        maxWidth: $tabsWrapper.width() * 2,
//                        minHeight: $tabsWrapper.height(),
//                        maxHeight: $tabsWrapper.height() * 2,
//                        handles: o.resizeHandles,
//                        alsoResize: $tabs,
//                        //start : function(){  },
//                        resize: function () {
//                            $tabs.trigger('resized');
//                        }
//                        //stop: function(){ $tabs.trigger('scrollToTab',$tabsNav.find('li.ui-tabs-selected')); }
//                    });
//                }
//                else {
//                    alert('Error:\nCannot be resizable because "jQuery.resizable" plugin is not available.');
//                }
//            }


//            //Add navigation icons
//            //Total height of nav/2 - total height of arrow/2
//            var arrowsTopMargin = (parseInt(parseInt($tabsNav.innerHeight(true) / 2) - 8)),
//				arrowsCommonCss = { 'cursor': 'pointer', 'z-index': 1000, 'position': 'absolute', 'top': 3, 'height': $tabsNav.outerHeight() - ($.browser.safari ? 2 : 1) };
//            $tabsWrapper.prepend(
//			  $nav = $('<div/>')
//			  		.disableSelection()
//					.css({ 'position': 'relative', 'z-index': 3000, 'display': 'none' })
//					.append(
//						$('<span/>')
//							.disableSelection()
//							.attr('title', 'Previous tab')
//							.css(arrowsCommonCss)
//							.addClass('ui-state-active ui-corner-tl ui-corner-bl stPrev stNav')
//							.css('left', 3)
//							.append($('<span/>').disableSelection().addClass('ui-icon ui-icon-carat-1-w').html('Previous tab').css('margin-top', arrowsTopMargin))
//							.click(function () {
//							    //Check if disabled
//							    if ($(this).hasClass('ui-state-disabled')) return;
//							    //Just select the previous tab and trigger scrollToTab event
//							    prevIndex = $tabsNav.find('li.ui-tabs-selected').prevAll().length - 1
//							    //Now select the tab
//							    $tabsNav.find('li').eq(prevIndex).find('a').trigger('click');
//							    return false;
//							}),
//						$('<span/>')
//							.disableSelection()
//							.attr('title', 'Next tab')
//							.css(arrowsCommonCss)
//							.addClass('ui-state-active ui-corner-tr ui-corner-br stNext stNav')
//							.css({ 'right': 3 })
//							.append($('<span/>').addClass('ui-icon ui-icon-carat-1-e').html('Next tab').css('margin-top', arrowsTopMargin))
//							.click(function () {
//							    //Just select the previous tab and trigger scrollToTab event
//							    nextIndex = $tabsNav.find('li.ui-tabs-selected').prevAll().length + 1
//							    //Now select the tab
//							    $tabsNav.find('li').eq(nextIndex).find('a').trigger('click');
//							    return false;
//							})
//					)
//			);

//            //Bind events to the $tabs
//            $tabs
//			.bind('tabsremove', function () {
//			    $tabs.trigger('scrollToTab').trigger('navHandler').trigger('navEnabler');
//			})
//			.bind('addCloseButton', function () {
//			    //Add close button if require
//			    if (!o.closable) return;
//			    $(this).find('.ui-tabs-nav li').each(function () {
//			        if ($(this).find('.ui-tabs-close').length > 0) return; //Already has close button
//			        var closeTopMargin = parseInt(parseInt($tabsNav.find('li:first').innerHeight() / 2, 10) - 8);
//			        $(this).disableSelection().append(
//						$('<span style="float:left;cursor:pointer;margin:' + closeTopMargin + 'px 2px 0 -11px" class="ui-tabs-close ui-icon ui-icon-close" title="Close this tab"></span>')
//							.click(function () {
//							    $tabs.tabs('remove', $(this).parents('li').prevAll().length);
//							    //If one tab remaining than hide the close button
//							    if ($tabs.tabs('length') == 1) {
//							        $tabsNav.find('.ui-icon-close').hide();
//							    }
//							    else {
//							        $tabsNav.find('.ui-icon-close').show();
//							    }
//							    //Call the method when tab is closed (if any)
//							    if ($.isFunction(o.onTabClose)) {
//							        o.onTabClose();
//							    }
//							    return false;
//							})
//					);
//			        //Show all close buttons if any hidden
//			        $tabsNav.find('.ui-icon-close').show();
//			    });
//			})
//			.bind('tabsadd', function (event) {
//			    //Select it on Add
//			    $tabs.tabs('select', $tabs.tabs('length') - 1);
//			    //Now remove the extra span added to the tab (not needed)
//			    $lastTab = $tabsNav.find('li:last');
//			    if ($lastTab.find('a span').length > 0) $lastTab.find('a').html($lastTab.find('a span').html());
//			    //Move the li to the innerwrapper
//			    $lastTab.appendTo($widthChecker);
//			    //Scroll the navigation to the newly added tab and also add close button to it
//			    $tabs
//					.trigger('addCloseButton')
//					.trigger('bindTabClick')
//					.trigger('navHandler')
//					.trigger('scrollToTab');
//			})//End tabsadd
//			.bind('addTab', function (event, label, content) {
//			    //Generate a random id
//			    var tabid = 'stTab-' + (Math.floor(Math.random() * 10000));
//			    //Append the content to the body
//			    $('body').append($('<div id="' + tabid + '"/>').append(content));
//			    //Add the tab
//			    $tabs.tabs('add', '#' + tabid, label);
//			})//End addTab
//			.bind('bindTabClick', function () {
//			    //Handle scroll when user manually click on a tab
//			    $tabsNav.find('a').click(function () {
//			        var $liClicked = $(this).parents('li');
//			        var navWidth = $nav.find('.stPrev').outerWidth(true);
//			        //debug('left='+($liClicked.offset().left)+' and tabs width = '+ ($tabs.width()-navWidth));
//			        if (($liClicked.position().left - navWidth) < 0) {
//			            $tabs.trigger('scrollToTab', [$liClicked, 'tabClicked', 'left'])
//			        }
//			        else if (($liClicked.outerWidth() + $liClicked.position().left) > ($tabs.width() - navWidth)) {
//			            $tabs.trigger('scrollToTab', [$liClicked, 'tabClicked', 'right'])
//			        }
//			        //Enable or disable next and prev arrows
//			        $tabs.trigger('navEnabler');
//			        return false;
//			    });
//			})
//            //Bind the event to act when tab is added
//			.bind('scrollToTab', function (event, $tabToScrollTo, clickedFrom, hiddenOnSide) {
//			    //If tab not provided than scroll to the last tab
//			    $tabToScrollTo = (typeof $tabToScrollTo != 'undefined') ? $($tabToScrollTo) : $tabsNav.find('li.ui-tabs-selected');
//			    //Scroll the pane to the last tab
//			    var navWidth = $nav.is(':visible') ? $nav.find('.stPrev').outerWidth(true) : 0;
//			    //debug($tabToScrollTo.prevAll().length)

//			    offsetLeft = -($tabs.width() - ($tabToScrollTo.outerWidth(true) + navWidth + parseInt($tabsNav.find('li:last').css('margin-right'), 10)));
//			    offsetLeft = (clickedFrom == 'tabClicked' && hiddenOnSide == 'left') ? -navWidth : offsetLeft;
//			    offsetLeft = (clickedFrom == 'tabClicked' && hiddenOnSide == 'right') ? offsetLeft : offsetLeft;
//			    //debug(offsetLeft);
//			    var scrollSettings = { 'axis': 'x', 'margin': true, 'offset': { 'left': offsetLeft }, 'easing': o.easing || '' }
//			    //debug(-($tabs.width()-(116+navWidth)));
//			    $tabsNav.scrollTo($tabToScrollTo, o.animationSpeed, scrollSettings);
//			})
//			.bind('navEnabler', function () {
//			    setTimeout(function () {
//			        //Check if last or first tab is selected than disable the navigation arrows
//			        var isLast = $tabsNav.find('.ui-tabs-selected').is(':last-child'),
//						isFirst = $tabsNav.find('.ui-tabs-selected').is(':first-child'),
//						$ntNav = $tabsWrapper.find('.stNext'),
//						$pvNav = $tabsWrapper.find('.stPrev');
//			        //debug('isLast = '+isLast+' - isFirst = '+isFirst);
//			        if (isLast) {
//			            $pvNav.removeClass('ui-state-disabled');
//			            $ntNav.addClass('ui-state-disabled');
//			        }
//			        else if (isFirst) {
//			            $ntNav.removeClass('ui-state-disabled');
//			            $pvNav.addClass('ui-state-disabled');
//			        }
//			        else {
//			            $ntNav.removeClass('ui-state-disabled');
//			            $pvNav.removeClass('ui-state-disabled');
//			        }
//			    }, o.animationSpeed);
//			})
//            //Now check if tabs need navigation (many tabs out of sight)
//			.bind('navHandler', function () {
//			    //Check the width of $widthChecker against the $tabsNav. If widthChecker has bigger width than show the $nav else hide it
//			    if ($widthChecker.width() > $tabsNav.width()) {
//			        $nav.show();
//			        //Put some margin to the first tab to make it visible if selected
//			        $tabsNav.find('li:first').css('margin-left', $nav.find('.stPrev').outerWidth(true));
//			    }
//			    else {
//			        $nav.hide();
//			        //Remove the margin from the first element
//			        $tabsNav.find('li:first').css('margin-left', 0);
//			    }
//			})
//			.bind('tabsselect', function () {
//			    //$tabs.trigger('navEnabler');
//			})
//			.bind('resized', function () {
//			    $tabs.trigger('navHandler');
//			    $tabs.trigger('scrollToTab', $tabsNav.find('li.ui-tabs-selected'));
//			})
//            //To add close buttons to the already existing tabs
//			.trigger('addCloseButton')
//			.trigger('bindTabClick')
//            //For the tabs that already exists
//			.trigger('navHandler')
//			.trigger('navEnabler');

//            //Select last tab if option is true
//            if (o.loadLastTab) {
//                setTimeout(function () { $tabsNav.find('li:last a').trigger('click') }, o.animationSpeed);
//            }
//        });

//        //Just for debuging
//        function debug(obj)
//        { console.log(obj) }
//    }
//})(jQuery);



///**
//* jQuery.ScrollTo - Easy element scrolling using jQuery.
//* Copyright (c) 2007-2009 Ariel Flesler - aflesler(at)gmail(dot)com | http://flesler.blogspot.com
//* Dual licensed under MIT and GPL.
//* Date: 5/25/2009
//* @author Ariel Flesler
//* @version 1.4.2
//*
//* http://flesler.blogspot.com/2007/10/jqueryscrollto.html
//*/
//; (function (d) { var k = d.scrollTo = function (a, i, e) { d(window).scrollTo(a, i, e) }; k.defaults = { axis: 'xy', duration: parseFloat(d.fn.jquery) >= 1.3 ? 0 : 1 }; k.window = function (a) { return d(window)._scrollable() }; d.fn._scrollable = function () { return this.map(function () { var a = this, i = !a.nodeName || d.inArray(a.nodeName.toLowerCase(), ['iframe', '#document', 'html', 'body']) != -1; if (!i) return a; var e = (a.contentWindow || a).document || a.ownerDocument || a; return d.browser.safari || e.compatMode == 'BackCompat' ? e.body : e.documentElement }) }; d.fn.scrollTo = function (n, j, b) { if (typeof j == 'object') { b = j; j = 0 } if (typeof b == 'function') b = { onAfter: b }; if (n == 'max') n = 9e9; b = d.extend({}, k.defaults, b); j = j || b.speed || b.duration; b.queue = b.queue && b.axis.length > 1; if (b.queue) j /= 2; b.offset = p(b.offset); b.over = p(b.over); return this._scrollable().each(function () { var q = this, r = d(q), f = n, s, g = {}, u = r.is('html,body'); switch (typeof f) { case 'number': case 'string': if (/^([+-]=)?\d+(\.\d+)?(px|%)?$/.test(f)) { f = p(f); break } f = d(f, this); case 'object': if (f.is || f.style) s = (f = d(f)).offset() } d.each(b.axis.split(''), function (a, i) { var e = i == 'x' ? 'Left' : 'Top', h = e.toLowerCase(), c = 'scroll' + e, l = q[c], m = k.max(q, i); if (s) { g[c] = s[h] + (u ? 0 : l - r.offset()[h]); if (b.margin) { g[c] -= parseInt(f.css('margin' + e)) || 0; g[c] -= parseInt(f.css('border' + e + 'Width')) || 0 } g[c] += b.offset[h] || 0; if (b.over[h]) g[c] += f[i == 'x' ? 'width' : 'height']() * b.over[h] } else { var o = f[h]; g[c] = o.slice && o.slice(-1) == '%' ? parseFloat(o) / 100 * m : o } if (/^\d+$/.test(g[c])) g[c] = g[c] <= 0 ? 0 : Math.min(g[c], m); if (!a && b.queue) { if (l != g[c]) t(b.onAfterFirst); delete g[c] } }); t(b.onAfter); function t(a) { r.animate(g, j, b.easing, a && function () { a.call(this, n, b) }) } }).end() }; k.max = function (a, i) { var e = i == 'x' ? 'Width' : 'Height', h = 'scroll' + e; if (!d(a).is('html,body')) return a[h] - d(a)[e.toLowerCase()](); var c = 'client' + e, l = a.ownerDocument.documentElement, m = a.ownerDocument.body; return Math.max(l[h], m[h]) - Math.min(l[c], m[c]) }; function p(a) { return typeof a == 'object' ? a : { top: a, left: a} } })(jQuery);

$(document).ready(function () {
    $("table.mycontextMenu").parent('div').css('box-shadow', '2px 4px 5px #CDCDCD');
    $("table.mycontextMenu").parent('div').css('border', '1px solid #CDCDCD');
});