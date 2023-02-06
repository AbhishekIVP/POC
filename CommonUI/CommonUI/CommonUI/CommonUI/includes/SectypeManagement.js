// 200 for string 12,9 for numeric
// MAXIMUM 38 sum of int and decimal
/*************************************************************************************************
Function Name     : SecTypeSetupShowHideLeg 
Author            : Gautam Kanwar
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/

String.prototype.replaceAll = function (find, replace) {
    return this.replace(new RegExp(find, 'g'), replace);
}

function SecTypeSetupShowHideLeg(rblVanillaExotic, dataChkBox, chkLegInfoId) {
    document.getElementById(chkLegInfoId).checked = false;
    var rblObj = GetObject(rblVanillaExotic);
    var count;
    //    var length = rblObj.cells.length;
    //    for (count = 0; count < length; count++) {
    //        if (rblObj.cells[count].children[0].checked) {
    //            GetObject(dataChkBox).style.visibility = 'visible';
    //            break;
    //        }
    //        else {
    //            GetObject(dataChkBox).style.visibility = 'hidden';
    //            break;
    //        }
    //    }
    var rblVanillaCell = $('#' + rblVanillaExotic).find('td').eq(0);
    if (rblVanillaCell.children().eq(0).prop('checked').toString() == "true") {
        $('#' + dataChkBox).css('visibility', 'visible');
    }
    else {
        $('#' + dataChkBox).css('visibility', 'hidden');
    }
}

function AttachWizardHandlers() {
    $("#divLeftPanel").hide();
    $(".navBtnBackNew").unbind("hover").hover(function () {
        $(this).parent().removeClass("navBtnSpan");
        $(this).parent().addClass("navBtnSpanHover");
    }, function () {
        $(this).parent().removeClass("navBtnSpanHover");
        $(this).parent().addClass("navBtnSpan");
    });

    $(".navBtnNextNew").unbind("hover").hover(function () {
        $(this).parent().removeClass("navBtnSpan");
        $(this).parent().addClass("navBtnSpanHover");
    }, function () {
        $(this).parent().removeClass("navBtnSpanHover");
        $(this).parent().addClass("navBtnSpan");
    });

    $(".addAttribHandlerClass").unbind("click").click(function (e) { AddAttribHandler(e); return false; });
    $(".addAttribLegHandlerClass").unbind("click").click(function (e) { AddAttribLegHandler(e); return false; });
    $(".addAttribReportHandlerClass").unbind("click").click(function (e) { AddAttribReportHandler(e); return false; });

    $(document).click(function (e) {
        var commonAttributesPopup = $('#panelCommonAttributes');
        var panelSectypeExpand = $('#panelAddSectypeExpand');
        var target = $(e.target);
        var targetId = target.prop('id');
        if (targetId != 'panelCommonAttributes' && targetId != 'tdApiCategory' && targetId != 'btnCommonAttributes' && targetId != 'arrowCommonAttributes') {
            if (commonAttributesPopup.length > 0) {
                if (commonAttributesPopup.css('display') !== 'none') {
                    commonAttributesPopup.hide();
                }
            }
        }
        if (targetId != 'addSectypeExpand') {// && targetId != 'btnAddAttributes' && targetId != 'arrowAddAttributes') {
            if (panelSectypeExpand.length > 0) {
                if (panelSectypeExpand.css('display') !== 'none') {
                    panelSectypeExpand.hide();
                }
            }
        }
    });

    CommonClickHandlersWizard();

    $(window).unbind('resize').resize(function (e) {
        WizardResizeFunction();
    });
}

function CommonClickHandlersWizard() {
    $("#btnAddAttributes").unbind("click").click(function (e) { AddAttrbPanelOpener(e); });
    $("#arrowAddAttributes").unbind("click").click(function (e) { AddAttrbPanelOpener(e); });

    $(document).click(function (e) {
        var addAttributesPopup = $('#panelAddAttributes');
        var target = $(e.target);
        var targetId = target.prop('id');
        if (targetId != 'panelAddAttributes' && targetId != 'btnAddAttributes' && targetId != 'arrowAddAttributes') {
            if (addAttributesPopup.length > 0) {
                if (addAttributesPopup.css('display') !== 'none') {
                    addAttributesPopup.hide();
                }
            }
        }
    });
}

function ManageCommonConfigHandlers() {
    CommonClickHandlersWizard();
    $(".addAttribMCAHandlerClass").unbind("click").click(function (e) { AddAttribMCAHandler(e); return false; });
}

function AddAttribHandler(e) {
    e.stopPropagation();
    var target = $(e.target);
    $('#addAttribDataTypeSaver').attr('dataType', target.val());
    $('[id$=btnAddAtribManually]').click();
}

function AddAttribLegHandler(e) {
    e.stopPropagation();
    var target = $(e.target);
    $('#addAttribDataTypeSaver').attr('dataType', target.val());
    $('[id$=btnAddAttribute]').click();
}

function AddAttribReportHandler(e) {
    e.stopPropagation();
    var target = $(e.target);
    $('#addAttribDataTypeSaver').attr('dataType', target.val());
    $('[id$=btnAddReportAttributes]').click();
}

function AddAttribMCAHandler(e) {
    e.stopPropagation();
    var target = $(e.target);
    $('#addAttribDataTypeSaver').attr('dataType', target.val());
    $('[id$=btnAdd]').click();
}

function WizardResizeFunction() {
    var ViewPortHeight = $(window).height();
    var TdTopHeight = $("#tdTop").height();
    var TdBottomHeight = $("#tdBottom").height();
    var BlackBarHeight = $(".iago-page-title").height();
    var NavigationBarHeight = 0;
    if ($("div[id$=SecTypeWizardNavigationBar]").css('display') != 'none')
        NavigationBarHeight = $("div[id$=SecTypeWizardNavigationBar]").height() - TdBottomHeight;
    $('#table').css('height', (ViewPortHeight - TdBottomHeight - NavigationBarHeight - TdTopHeight - BlackBarHeight) + 'px');
    $('div[id$=divWrapperContainer]').outerHeight($('#table').outerHeight());
    //$('#top-ContainerDiv').css('height', (ViewPortHeight - TdBottomHeight - TdTopHeight) + 'px');
    $('#divLeftPanel').css('height', (ViewPortHeight - TdBottomHeight - TdTopHeight - BlackBarHeight - 30) + 'px'); // - NavigationBarHeight
    $('#divMainBody').css('height', (ViewPortHeight - TdBottomHeight - NavigationBarHeight - TdTopHeight - BlackBarHeight - $('#divMainBodyFreezed').height() - 5) + 'px');
    $('div[id$=divNewContainer]').css('margin-left', '0px');
}

function CreateWizardMenu(panelWizardId) {
    var panelWizard = $("#" + panelWizardId);
    var str = "";
    var TabButtons;
    var SubTabButtons;
    var GroupClass = "";
    var TabClass = "";
    var SubTabClass = "";
    var TabText = "";
    var SubTabText = "";
    var TabTextHelper = null;
    var SubTabTextHelper = null;
    var indexTemp = 0;
    panelWizard.hide();
    var panelTabs = panelWizard.find('table:eq(0) tbody>tr:eq(1) table tr:eq(0)').children();
    var panelSubTabs = panelWizard.find('table:eq(0) tbody>tr:eq(1) table tr:eq(1) table tr:eq(0)').children();
    for (var index = 0; index < panelTabs.length; index++) {
        TabButtons = panelTabs.eq(index).children().eq(0);
        TabTextHelper = TabButtons.html().split("<BR>");
        TabText = "";
        for (indexTemp = 0; indexTemp < TabTextHelper.length; indexTemp++) {
            TabText += TabTextHelper[indexTemp];
            if (indexTemp != TabTextHelper.length - 1)
                TabText += " ";
        }
        TabTextHelper = TabText.split("<br>");
        TabText = "";
        for (indexTemp = 0; indexTemp < TabTextHelper.length; indexTemp++) {
            TabText += TabTextHelper[indexTemp];
            if (indexTemp != TabTextHelper.length - 1)
                TabText += " ";
        }
        TabButtons.html(TabText);
        TabButtons.prop('title', TabText.replace('&amp;', '&'));
        GroupClass = "UnSelectedTabGroup";
        TabClass = "UnSelectedTab";
        if (TabButtons.attr('class').toLowerCase().indexOf('active') > -1) {
            GroupClass = "SelectedTabGroup";
            TabClass = "SelectedTab";
        }
        str += "<div class=\"" + GroupClass + " bottomBarTabGroup\" IBBTG=\"1\">";
        str += "<div class=\"" + TabClass + " bottomBarTabs\" IBBTH=\"1\">";
        str += TabButtons.parent().html() + "</div>";
        if (panelSubTabs.length > 0 && TabButtons.attr('class').toLowerCase().indexOf('active') > -1) {
            str += "<div IBBToggle=\"1\">";
            for (var index1 = 0; index1 < panelSubTabs.length; index1++) {
                if (index1 % 2 == 0) {
                    SubTabButtons = panelSubTabs.eq(index1).children().eq(0);
                    SubTabTextHelper = SubTabButtons.html().split("<BR>");
                    SubTabText = "";
                    for (indexTemp = 0; indexTemp < SubTabTextHelper.length; indexTemp++) {
                        SubTabText += SubTabTextHelper[indexTemp];
                        if (indexTemp != SubTabTextHelper.length - 1)
                            SubTabText += " ";
                    }
                    SubTabTextHelper = SubTabText.split("<br>");
                    SubTabText = "";
                    for (indexTemp = 0; indexTemp < SubTabTextHelper.length; indexTemp++) {
                        SubTabText += SubTabTextHelper[indexTemp];
                        if (indexTemp != SubTabTextHelper.length - 1)
                            SubTabText += " ";
                    }
                    SubTabButtons.html(SubTabText);
                    SubTabButtons.prop('title', SubTabText.replace('&amp;', '&'));
                    SubTabClass = "UnSelectedSubTab";
                    if (SubTabButtons.attr('class').toLowerCase().indexOf('active') > -1) {
                        SubTabClass = "SelectedSubTab";
                    }
                    if (index1 == 0)
                        str += "<div class=\"" + SubTabClass + " bottomBarSubTabs\" IBBSTH=\"1\" IBBSTHF=\"1\">";
                    else
                        str += "<div class=\"" + SubTabClass + " bottomBarSubTabs\" IBBSTH=\"1\">";
                    str += SubTabButtons.parent().html() + "</div>";
                }
            }
            str += "</div>";
        }
        str += "</div>";
    }
    $("#divLeftPanel").html(str);
    $("#divLeftPanel").css('display', 'inline');
    WizardResizeFunction();
}

function FormatDefaultDivs() {
    setTimeout(function () {
        var panelSecurityMain = $('[id$=panelSecurityMain]');
        var panelSecurityLegs = $('[id$=panelSecurity]');

        panelSecurityMain.css('height', 'auto');
        panelSecurityMain.css('overflow', 'auto');
        panelSecurityLegs.css('height', 'auto');
        panelSecurityLegs.css('overflow', 'auto');
        $('.autocomplete_completionListElement').hide();
    }, 500);
}

function AddAttrbPanelOpener(e) {
    var BtnAddAttributes = $("#btnAddAttributes");
    var PanelAddAttributes = $("#panelAddAttributes");
    var TopPanel = BtnAddAttributes.offset().top + BtnAddAttributes.height() + 5;
    var LeftPopUp = BtnAddAttributes.offset().left + BtnAddAttributes.width() - PanelAddAttributes.width() + 1;
    PanelAddAttributes.css('top', TopPanel.toString() + 'px');
    PanelAddAttributes.css('left', LeftPopUp.toString() + 'px');
    PanelAddAttributes.css('display', 'block');
}

function CollapsiblePanelExpanded(TargetId) {
    if ($("#" + TargetId).hasClass("SMCollapsedClass"))
        $("#" + TargetId).removeClass("SMCollapsedClass");
    if (!$("#" + TargetId).hasClass("SMExpandedClass"))
        $("#" + TargetId).addClass("SMExpandedClass");
}

function CollapsiblePanelCollapsed(TargetId) {
    if ($("#" + TargetId).hasClass("SMExpandedClass"))
        $("#" + TargetId).removeClass("SMExpandedClass");
    if (!$("#" + TargetId).hasClass("SMCollapsedClass"))
        $("#" + TargetId).addClass("SMCollapsedClass");
}

function ValidatePrimaryKey(ddlId, lblId, modalId) {
    GetObject(lblId).innerText = '';
    var lbl = document.getElementById(lblId);
    if (document.getElementById(ddlId).options[document.getElementById(ddlId).selectedIndex].value == "-1") {
        GetObject(lblId).innerText = '● Please select primary field ';
        //  $find(modalId).show();
        return false;
    }

}

/*************************************************************************************************
Function Name     : ClearAndShowPopup 
Author            : Gautam Kanwar
Description       : This function is used to show popup to add common attribute
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/

function ClearAndShowPopup(modalAddAtrib, txtDisplayName, txtDescription, rblDataTypeAdd, rblAttributeType, lblDisplayNameError, lblPanelHeaderAtrib, hdn, txtString, td, txtNumericInt, txtNumericDecimal, lbl, chkBoxCloneable, divTag) {
    var dataType = $('#addAttribDataTypeSaver').attr('datatype');
    GetObject(lblDisplayNameError).innerText = '';
    GetObject(lblPanelHeaderAtrib).innerText = 'Add Master / Identifier Attribute';
    var txtObj = GetObject(txtDisplayName);
    GetObject(txtString).style.display = '';
    GetObject(td).style.display = '';
    GetObject(txtString).value = '200'
    GetObject(txtNumericInt).style.display = 'none';
    GetObject(txtNumericDecimal).style.display = 'none';
    GetObject(lbl).style.display = 'none';
    txtObj.value = '';
    GetObject(txtDescription).value = '';
    $('#' + chkBoxCloneable).prop('checked', true);

    var obj = {};
    obj.container = $('#' + divTag);
    obj.list = [];
    tag.create(obj);

    $('[id$=AddAttribDataTypeLabel]').text(dataType);
    //    GetObject(rblDataTypeAdd).isDisabled = false;
    //  GetObject(rblAttributeType).isDisabled = false;
    //    var rblObj = GetObject(rblDataTypeAdd); 
    //    var rblObjAtt=GetObject(rblAttributeType); 
    //     for (count = 0; count < rblObj.cells.length; count++) {
    //        rblObj.cells[count].children[0].disabled =false;
    //        }
    //        
    //        for (count = 0; count < rblObjAtt.cells.length; count++) {
    //        rblObjAtt.cells[count].children[0].disabled =false;
    //        }

    $('#' + rblDataTypeAdd).find('td').eq(0)[0].children[0].checked = true;
    $('#' + rblAttributeType).find('td').eq(0)[0].children[0].checked = true;
    document.getElementById(hdn).value = '0';

    var rdl = $('#' + rblDataTypeAdd).children().eq(0).children().find('label');
    for (var index = 0; index < rdl.length; index++) {
        if (rdl.eq(index).text().toLowerCase() == dataType.toLowerCase()) {
            var rrr = rdl.eq(index).parent().find('input');
            rrr.prop('checked', true);
            break;
        }
    }
    $('#' + rblDataTypeAdd).click();

    $find(modalAddAtrib).show();
    txtObj.focus();
    return false;
}

function ShowAddReferencePopUp(modalAddRefAttrib, txtDisplayName, ddlEntityType, ddlEntityAttri, lblDisplayNameError) {
    GetObject(lblDisplayNameError).innerText = '';
    var txtObj = GetObject(txtDisplayName);
    txtObj.value = '';

}
/*************************************************************************************************
Function Name     : CheckCommonAttribute 
Author            : Gautam Kanwar
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/

function CheckCommonAttribute(txtDisplayName,
                              lblDisplayNameError,
                              lstExistingAttributes,
                              hdnUpdate,
                              txtStr,
                              txtNumericInt,
                              txtNumericDecimal,
                              lblDec,
                              td,
                              rbl,
                              hdnAttributeSize, hdnDisplayName) {
    var objDispName = GetObject(txtDisplayName);
    var rblObj = GetObject(rbl);
    var dispName = trim(objDispName.value).toLowerCase();
    if (dispName == '') {
        GetObject(lblDisplayNameError).innerText = '● Please enter Attribute Name';
        objDispName.focus();
        return false;
    }
    var dataType;

    var count;
    var lstBoxObj = GetObject(lstExistingAttributes);
    var lstCount = lstBoxObj.length;
    // Check If added Attribute is in Common Attributes.
    for (count = 0; count < lstCount; count++) {
        if (trim(lstBoxObj[count].text.toLowerCase()) == dispName && dispName != trim(GetObject(hdnDisplayName).value.toLowerCase())) {
            GetObject(lblDisplayNameError).innerText = '● Attribute Name is not unique';
            objDispName.focus();
            return false;
        }
    }
    if (trim(GetObject(hdnUpdate).value) == '0') {
        //        for (count = 0; count < rblObj.cells.length; count++) {
        //            if (rblObj.cells[count].children[0].checked) {
        //                dataType = rblObj.cells[count].children[0].value;
        //                break;
        //            }
        //        }
        var rblCells = $('#' + rbl).find('td');
        for (count = 0; count < rblCells.length; count++) {
            if (rblCells.eq(count).children().eq(0).prop('checked').toString() == "true") {
                dataType = rblCells.eq(count).children().eq(0)[0].value;
                break;
            }
        }
        if (dataType.toLowerCase() == 'string') {
            if (trim(GetObject(txtStr).value) == '') {
                GetObject(lblDisplayNameError).innerText = '● Enter Attribute Size';
                return false;
            }
            if (parseInt(trim(GetObject(txtStr).value)) > 8000) {
                GetObject(lblDisplayNameError).innerText = '●Attribute Size Should Be Less Than Or Equal To 8000';
                return false;
            }

        }
        else if (dataType.toLowerCase() == 'numeric') {
            if (trim(GetObject(txtNumericDecimal).value) == '' || trim(GetObject(txtNumericInt).value) == '') {
                GetObject(lblDisplayNameError).innerText = '● Enter Attribute Size';
                return false;
            }
            if (Number(trim(GetObject(txtNumericInt).value)) + Number(trim(GetObject(txtNumericDecimal).value)) > 28) {
                GetObject(lblDisplayNameError).innerText = '● Precision Can Have A Maximum Value Of 28 (Precision is no. of digits before decimal + no. of digits after decimal) ';
                return false;
            }
        }

    }

    else if (trim(GetObject(hdnUpdate).value) == '1') {
        //        for (count = 0; count < rblObj.cells.length; count++) {
        //            if (rblObj.cells[count].children[0].children[0].checked) {
        //                dataType = rblObj.cells[count].children[0].children[0].value;
        //                break;
        //            }
        //        }
        var rblCells = $('#' + rbl).find('td');
        for (count = 0; count < rblCells.length; count++) {
            if (rblCells.eq(count).children().eq(0).children().eq(0).prop('checked').toString() == "true") {
                dataType = rblCells.eq(count)[0].children[0].children[0].value;
                break;
            }
        }
        if (dataType.toLowerCase() == 'string') {
            if ((GetObject(txtStr).value) == '') {
                GetObject(lblDisplayNameError).innerText = '● Enter Attribute Size';
                return false;
            }
            if (parseInt(GetObject(txtStr).value) < parseInt(GetObject(hdnAttributeSize).value)) {
                GetObject(lblDisplayNameError).innerText = '● Attribute Size should be greater than or equal to ' + (GetObject(hdnAttributeSize).value);
                return false;

            }
            if (parseInt(trim(GetObject(txtStr).value)) > 8000) {
                GetObject(lblDisplayNameError).innerText = '●Attribute Size Should Be Less Than Or Equal To 8000';
                return false;
            }
        }
        else if (dataType.toLowerCase() == 'numeric' && (trim(GetObject(txtNumericDecimal).value) == '' || trim(GetObject(txtNumericInt).value) == '')) {
            if (trim(GetObject(txtNumericDecimal).value) == '' || trim(GetObject(txtNumericInt).value) == '') {
                GetObject(lblDisplayNameError).innerText = '● Enter Attribute Size';
                return false;
            }
            if (parseInt(GetObject(txtNumericInt).value) < parseInt((GetObject(hdnAttributeSize).value).split('.')[0])) {
                GetObject(lblDisplayNameError).innerText = '● Attribute Size should be greater than or equal to ' + (GetObject(hdnAttributeSize).value);
                return false;

            }
            if (parseInt(GetObject(txtNumericDecimal).value) < parseInt((GetObject(hdnAttributeSize).value).split('.')[1])) {
                GetObject(lblDisplayNameError).innerText = '● Attribute Size should be greater than or equal to ' + (GetObject(hdnAttributeSize).value);
                return false;

            }
            if (Number(trim(GetObject(txtNumericInt).value)) + Number(trim(GetObject(txtNumericDecimal).value)) > 28) {
                GetObject(lblDisplayNameError).innerText = '● Precision Can Have A Maximum Value Of 28 (Precision is no. of digits before decimal + no. of digits after decimal) ';
                return false;
            }

        }

    }

    return true;
}
function CheckCommonReferenceAttribute(txtDisplayName, ddlEntityName, lblDisplayNameError, lstExistingAttributes, ddlAttributeNames) {
    var objDispName = GetObject(txtDisplayName);
    var dispName = trim(objDispName.value).toLowerCase();
    if (dispName == '') {
        GetObject(lblDisplayNameError).innerText = '● Please enter Attribute Name';
        objDispName.focus();
        return false;
    }
    if (document.getElementById(ddlEntityName).options[document.getElementById(ddlEntityName).selectedIndex].value == "-1") {
        GetObject(lblDisplayNameError).innerText = '● Please select entity type ';
        return false;
    }
    if (document.getElementById(ddlAttributeNames).options[document.getElementById(ddlAttributeNames).selectedIndex].value == "-1") {
        GetObject(lblDisplayNameError).innerText = '● Please select reference attribute';

        return false;
    }
    //   var displayname=trim(document.getElementById(ddlEntityName).options[document.getElementById(ddlEntityName).selectedIndex].text).toLowerCase() ;
    var count;
    var lstBoxObj = GetObject(lstExistingAttributes);
    var lstCount = lstBoxObj.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(lstBoxObj[count].text.toLowerCase()) == dispName && dispName != trim(GetObject(hdnDisplayName).value.toLowerCase())) {
            GetObject(lblDisplayNameError).innerText = '● Attribute Name is not unique';
            objDispName.focus();
            return false;
        }
    }
    return true;



}

/*************************************************************************************************
Function Name     : ValidateUpdate 
Author            : Bhavya Jaitly
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function ValidateUpdate(ddlEntityName, ddlAttributeName, lblErrorUpdate) {
    if (document.getElementById(ddlEntityName).options[document.getElementById(ddlEntityName).selectedIndex].value == "-1") {
        GetObject(lblErrorUpdate).innerText = '● Please select entity type ';
        return false;
    }
    if (document.getElementById(ddlAttributeName).options[document.getElementById(ddlAttributeName).selectedIndex].value == "-1") {
        GetObject(lblErrorUpdate).innerText = '● Please select Reference Attribute ';

        return false;
    }
    return true;
}

/*************************************************************************************************
Function Name     : CallUpdateAtribPopUp 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function CallUpdateAtribPopUp(modalUpdateAtrib,
                              txtDisplayNameUpdate,
                               txtDescriptionUpdate,
                              txtUpdateStringAttribute,
                              txtUpdateNumericIntAttribute,
                              txtUpdateNumericDecimalAttribute,
                              lblUpdateSizeAttribute,
                              tdSizeUpdate,
                              hdnAttributeSize,
                              hdnUpdateAtribRowId,
                              rowId,
                              rblDataType,
                              hdnDisplayName,
                              gridDetailAttributes,
                              lblDisplayNameUpdateError,
                              chkBoxCloneable,
                              hdnCloneable,
                              divTagUpdate,
                              hdnTags,
                              deleteColumnIndex,
                              isLabelForRBL,
                              UpdateAttribDataTypeLabel,
                              updateType) {
    var count;
    var gridObj = GetObject(gridDetailAttributes);
    var length = gridObj.rows.length;
    var dispName;
    var description;
    var lstcount;
    var attributesize;
    var isCloneable;
    var tagValue;
    GetObject(lblDisplayNameUpdateError).innerText = '';

    var regEx = new RegExp(String.fromCharCode(160), 'gi');
    var normalSpace = ' ';

    for (count = 1; count < length; count++) {
        if (trim(gridObj.rows[count].cells[parseInt(deleteColumnIndex)].innerText) == rowId) {
            dispName = trim(gridObj.rows[count].cells[0].innerText);
            description = trim(gridObj.rows[count].cells[1].innerText);
            dataType = trim(gridObj.rows[count].cells[3].innerText);
            attributesize = trim(gridObj.rows[count].cells[9].innerText);

            dispName = dispName.replace(regEx, normalSpace);

            var max = gridObj.rows[count].cells.length - 1;
            while (max > 0) {
                if (typeof gridObj.rows[count].cells[max] !== 'undefined')
                    break;
                max--;
            }

            isCloneable = trim(gridObj.rows[count].cells[max - 1].innerText);
            tagValue = trim(gridObj.rows[count].cells[max].innerText);

            break;
        }
    }
    GetObject(hdnAttributeSize).value = trim(attributesize);
    if (dataType.toLowerCase() == 'string') {

        GetObject(txtUpdateStringAttribute).value = attributesize;
        GetObject(tdSizeUpdate).style.display = '';
        GetObject(txtUpdateStringAttribute).style.display = '';
        GetObject(txtUpdateNumericIntAttribute).style.display = 'none';
        GetObject(txtUpdateNumericDecimalAttribute).style.display = 'none';
        GetObject(lblUpdateSizeAttribute).style.display = 'none';

    }
    else if (dataType.toLowerCase() == 'numeric') {
        var commonCols = attributesize.split('.');
        GetObject(txtUpdateNumericIntAttribute).value = commonCols[0];
        GetObject(txtUpdateNumericDecimalAttribute).value = commonCols[1];
        GetObject(tdSizeUpdate).style.display = '';
        GetObject(txtUpdateStringAttribute).style.display = 'none';
        GetObject(txtUpdateNumericIntAttribute).style.display = '';
        GetObject(txtUpdateNumericDecimalAttribute).style.display = '';
        GetObject(lblUpdateSizeAttribute).style.display = '';
    }
    else {
        GetObject(tdSizeUpdate).style.display = 'none';
        GetObject(txtUpdateStringAttribute).value = '';
        GetObject(txtUpdateNumericIntAttribute).value = '';
        GetObject(txtUpdateNumericDecimalAttribute).value = '';
        GetObject(txtUpdateStringAttribute).style.display = 'none';
        GetObject(txtUpdateNumericIntAttribute).style.display = 'none';
        GetObject(txtUpdateNumericDecimalAttribute).style.display = 'none';
        GetObject(lblUpdateSizeAttribute).style.display = 'none';

    }
    GetObject(hdnUpdateAtribRowId).value = rowId;
    GetObject(txtDisplayNameUpdate).value = dispName;
    GetObject(txtDescriptionUpdate).value = description;
    GetObject(hdnDisplayName).value = dispName;
    var rblObj = GetObject(rblDataType);

    //    for (count = 0; count < rblObj.cells.length; count++) {
    //        if (rblObj.cells[count].innerText == dataType) {
    //            if (rblObj.cells[count].children[0].children.length == 0)
    //                rblObj.cells[count].children[0].checked = true;
    //            else
    //                rblObj.cells[count].children[0].children[0].checked = true;
    //            break;
    //        }

    //    }
    var rblCells = $('#' + rblDataType).find('td');
    for (count = 0; count < rblCells.length; count++) {
        if (rblCells.eq(count).text() == dataType) {
            if (rblCells.eq(count)[0].children[0].children.length == 0)
                rblCells.eq(count)[0].children[0].checked = true;
            else
                rblCells.eq(count)[0].children[0].children[0].checked = true;
            break;
        }
    }
    if (isCloneable.toLowerCase() === 'true')
        $(GetObject(chkBoxCloneable)).prop('checked', true);
    else
        $(GetObject(chkBoxCloneable)).prop('checked', false);

    if (typeof tagValue !== 'undefined') {
        var obj = {};
        obj.container = $('#' + divTagUpdate);
        obj.list = tagValue.length > 0 ? tagValue.split(',') : [];
        tag.create(obj);
    }

    if (isLabelForRBL) {
        $("#" + UpdateAttribDataTypeLabel).text(dataType);
    }

    $('#hdnAttributeName').val(dispName);
    //fetch from DB leg attr
    if (typeof updateType != 'undefined' && updateType == 'UpdateLegAttr') {
        $('#hdnLegName').val($("#" + event.target.id).closest($("[id$='gridLegAttribute']")).closest('tr').prev().find("[id$='lblLevelId']").text());
        getRefDisplayAttributedetails(dispName, null, 'chkBoxMandatoryUpdate2', null, '1');
    }
    //save details for master attributes details
    if (typeof updateType != 'undefined' && updateType == 'UpdateNewNormalAttr')
        getRefDisplayAttributedetails(dispName, null, 'chkBoxMandatoryUpdateNormal', null);
    //fetch from DB for master details
    if (typeof updateType != 'undefined' && updateType == 'UpdateExistingNormalAttr')
        getRefDisplayAttributedetails(dispName, null, 'chkBoxMandatoryUpdate', null, '1');


    $find(modalUpdateAtrib).show();
    GetObject(txtDisplayNameUpdate).focus();
}

var gName;
/*************************************************************************************************
Function Name     : CallUpdateRefAtribPopUp 
Author            : Bhavya Jaitly
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function CallUpdateRefAtribPopUp(modalUpdate,
                                 txtDisplayUpdate,
                                  txtDescriptionUpdate,
                                 hdnUpdateRow,
                                 rowId,
                                 ddlEntityName,
                                 ddlEntityAttributeName,
                                 hdnDisplayName,
                                 gridDetails,
                                 lblUpdateError,
                                 ddlLegAttributeNameId,
                                 chkBoxCloneable,
                                 hdnCloneable,
                                 divRTagUpdate,
                                 objhdnTags,
                                 deleteColumnIndex,
                                 updateType) {
    var count;
    var gridObj = GetObject(gridDetails);
    var length = gridObj.rows.length;
    var dispName;
    var desription;
    var entityName;
    var entityAttribute;
    var entityTypeId;
    var entityAttributeId;
    var isCloneable;
    var tagValue;

    GetObject(lblUpdateError).innerText = '';

    for (count = 1; count < length; count++) {
        if (trim(gridObj.rows[count].cells[parseInt(deleteColumnIndex)].innerText) == rowId) {
            dispName = trim(gridObj.rows[count].cells[0].innerText.replaceNBR());
            desription = trim(gridObj.rows[count].cells[1].innerText.replaceNBR());
            entityName = trim(gridObj.rows[count].cells[5].innerText.replaceNBR());
            entityAttribute = trim(gridObj.rows[count].cells[6].innerText.replaceNBR());
            entityLegAttribute = trim(gridObj.rows[count].cells[7].innerText.replaceNBR());
            entityTypeId = trim(gridObj.rows[count].cells[4].innerText.replaceNBR());
            isCloneable = trim(gridObj.rows[count].cells[13].innerText.replaceNBR());
            tagValue = trim(gridObj.rows[count].cells[14].innerText.replaceNBR());
            entityAttributeId = trim(gridObj.rows[count].cells[8].innerText.replaceNBR());
            break;
        }
    }

    if (entityLegAttribute.indexOf(':') > -1)
        entityAttribute = entityAttribute + ':' + entityLegAttribute.split(':')[1];
    var objddl = GetObject(ddlEntityName);
    length = objddl.length;

    for (count = 1; count < length; count++) {
        if (objddl[count].text == entityName) {
            objddl.selectedIndex = count;
            break;
        }
    }
    //Reference Type is selected here itself (By Name)

    GetObject(hdnUpdateRow).value = rowId;
    GetObject(txtDisplayUpdate).value = dispName;
    GetObject(txtDescriptionUpdate).value = desription;
    GetObject(hdnDisplayName).value = dispName;

    showRefAttributes(objddl[count].value, ddlEntityAttributeName, ddlLegAttributeNameId);

    // Colon separated - attribute Display Names for master and "entityId-entityAttributeId"   -- if it is CurveReferenceAttribute
    if (entityAttribute.split(':').length > 1) {
        gName = entityAttribute.split(':')[0] + ':' + entityTypeId + '-' + entityAttributeId;
    }
    else
        gName = entityAttribute;

    if (isCloneable.toLowerCase() === 'true')
        $(GetObject(chkBoxCloneable)).prop('checked', true);
    else
        $(GetObject(chkBoxCloneable)).prop('checked', false);

    if (typeof tagValue !== 'undefined') {
        var obj = {};
        obj.container = $('#' + divRTagUpdate);
        obj.list = tagValue.length > 0 ? tagValue.split(',') : [];
        tag.create(obj);
    }

    // Update new ref attr
    var smdata = null;
    callRefAttr(entityTypeId).then(function (res) {
        var data = res;
        if (data != null) {
            smdata = data;
        }
        $('#hdnAttributeName').val(dispName);
        if (typeof updateType != 'undefined' && updateType == 'UpdateNewRefAttr')
            getRefDisplayAttributedetails(dispName, 'smtrRefDisplayNameUpdate_dropdown', 'chkBoxUpdateMandatoryRef', smdata);
        //for updating existing ref mster attr 
        if (typeof updateType != 'undefined' && updateType == 'UpdateExistingRefAttr')
            getRefDisplayAttributedetails(dispName, 'smtrRefDisplayName_dropdown_update', 'chkBoxRefMandatory', smdata, '1');

        $find(modalUpdate).show();
        GetObject(txtDisplayUpdate).focus();
    });

}

/*************************************************************************************************
Function Name     : CallUpdateRefAtribSavedPopUp 
Author            : Bhavya Jaitly
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function CallUpdateRefAtribSavedPopUp(modalUpdate,
                                 txtDisplayUpdate,
                                  txtDescriptionUpdate,
                                 hdnUpdateRow,
                                 rowId,
                                 ddlEntityName,
                                 ddlEntityAttributeName,
                                 ddlDetailEntityAttributeId,
                                 hdnDisplayName,
                                 gridDetails,
                                 lblUpdateError,
                                 chkBoxRCloneableUpdate2,
                                 hdnCloneable,
                                 divRTagUpdate2,
                                 objhdnTags,
                                 deleteColumnIndex, entityIndex, entityAttributeIndex) {
    var count;
    var gridObj = GetObject(gridDetails);
    var length = gridObj.rows.length;
    var dispName;
    var description;
    var entityName;
    var entityAttribute;
    var entityTypeId;
    var entityAttributeId;
    var isCloneable;
    var tagValue;

    GetObject(lblUpdateError).innerText = '';
    var ddlDetailEntityAttribute = $get(ddlDetailEntityAttributeId);
    for (count = 1; count < length; count++) {
        if (trim(gridObj.rows[count].cells[parseInt(deleteColumnIndex)].innerText) == rowId) {
            dispName = trim(gridObj.rows[count].cells[0].innerText);
            description = trim(gridObj.rows[count].cells[1].innerText);
            entityName = trim(gridObj.rows[count].cells[parseInt(entityIndex)].innerText);
            entityAttribute = trim(gridObj.rows[count].cells[parseInt(entityAttributeIndex)].innerText);
            entityLegAttribute = trim(gridObj.rows[count].cells[parseInt(entityAttributeIndex) + 1].innerText);
            entityTypeId = trim(gridObj.rows[count].cells[parseInt(entityIndex) + 1].innerText);
            entityAttributeId = trim(gridObj.rows[count].cells[parseInt(entityAttributeIndex) + 2].innerText);
            isCloneable = trim(gridObj.rows[count].cells[12].innerText);


            if (typeof gridObj.rows[count].cells[14] === "undefined")
                tagValue = trim(gridObj.rows[count].cells[13].innerText);
            else
                tagValue = trim(gridObj.rows[count].cells[14].innerText);

            break;
        }
    }

    if (entityLegAttribute.indexOf(':') > -1)
        entityAttribute = entityAttribute + ':' + entityLegAttribute.split(':')[1];

    var objddl = GetObject(ddlEntityName);
    length = objddl.length;

    for (count = 1; count < length; count++) {
        if (objddl[count].text == entityName) {
            objddl.selectedIndex = count;
            break;
        }
    }

    GetObject(hdnUpdateRow).value = rowId;
    GetObject(txtDisplayUpdate).value = dispName;
    GetObject(txtDescriptionUpdate).value = description;
    GetObject(hdnDisplayName).value = dispName;
    showRefAttributes(objddl[count].value, ddlEntityAttributeName, ddlDetailEntityAttributeId);
    if (entityAttribute.split(':').length > 1) {
        gName = entityAttribute.split(':')[0] + ':' + entityTypeId + '-' + entityAttributeId;
    }
    else
        gName = entityAttribute;


    //if (hdnCloneable.toLowerCase() === 'true')
    if (isCloneable.toLowerCase() === 'true')
        $(GetObject(chkBoxRCloneableUpdate2)).prop('checked', true);
    else
        $(GetObject(chkBoxRCloneableUpdate2)).prop('checked', false);

    var obj = {};
    obj.container = $('#' + divRTagUpdate2);
    obj.list = tagValue.length > 0 ? tagValue.split(',') : [];
    tag.create(obj);

    var event_target_id = event.target.id;
    //fetch from DB
    var smdata = null;
    callRefAttr(entityTypeId).then(function (res) {
        var data = res;
        if (data != null) {
            smdata = data;
        }

        $('#hdnAttributeName').val(dispName);
        $('#hdnLegName').val($("#" + event_target_id).closest($("[id$='gridLegAttribute']")).closest('tr').prev().find("[id$='lblLevelId']").text());
        getRefDisplayAttributedetails(dispName, 'sm_ref_saved_update_dropdown', 'chkBoxRMandatoryUpdate2', smdata, '1');

        $find(modalUpdate).show();
        GetObject(txtDisplayUpdate).focus();
    });
}



/*************************************************************************************************
Function Name     : CallUpdateRefLegAtribPopUp 
Author            : Bhavya Jaitly
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function CallUpdateRefLegAtribPopUp(modalUpdate,
                                 txtDisplayUpdate,
                                  txtDescriptionUpdate,
                                 hdnUpdateRow,
                                 rowId,
                                 ddlEntityName,
                                 ddlEntityAttributeName,
                                 ddlDetailEntityAttributeId,
                                 hdnDisplayName,
                                 gridDetails,
                                 lblUpdateError,
                                 chkBoxRCloneableUpdate,
                                 hdnCloneable,
                                 divRTagUpdate,
                                 hdnTags,
                                 deleteColumnIndex) {
    var count;
    var gridObj = GetObject(gridDetails);
    var length = gridObj.rows.length;
    var dispName;
    var description;
    var entityAttribute;
    var entityTypeId;
    var entityAttributeId;
    var entityType;
    var isCloneable;
    var tags;
    var entityLegAttribute;

    GetObject(lblUpdateError).innerText = '';
    var ddlDetailEntityAttribute = $get(ddlDetailEntityAttributeId);

    for (count = 1; count < length; count++) {
        if (trim(gridObj.rows[count].cells[parseInt(deleteColumnIndex)].innerText) == rowId) {
            dispName = trim(gridObj.rows[count].cells[0].innerText);
            description = trim(gridObj.rows[count].cells[1].innerText)
            entityType = trim(gridObj.rows[count].cells[3].innerText);
            entityTypeId = trim(gridObj.rows[count].cells[4].innerText);
            entityAttribute = trim(gridObj.rows[count].cells[5].innerText);
            entityLegAttribute = trim(gridObj.rows[count].cells[6].innerText);
            entityAttributeId = trim(gridObj.rows[count].cells[7].innerText);
            isCloneable = trim(gridObj.rows[count].cells[13].innerText);
            tags = trim(gridObj.rows[count].cells[14].innerText);
            break;
        }
    }
    if (entityLegAttribute.indexOf(':') > -1)
        entityAttribute = entityAttribute + ':' + entityLegAttribute.split(':')[1];

    var objddl = GetObject(ddlEntityName);
    length = objddl.length;

    for (count = 1; count < length; count++) {
        if (objddl[count].text == entityType) {
            objddl.selectedIndex = count;
            break;
        }
    }

    GetObject(hdnUpdateRow).value = rowId;
    GetObject(txtDisplayUpdate).value = dispName;
    GetObject(txtDescriptionUpdate).value = description;
    GetObject(hdnDisplayName).value = dispName;
    showRefAttributes(objddl[count].value, ddlEntityAttributeName, ddlDetailEntityAttributeId);
    if (entityAttribute.split(':').length > 1)
        gName = entityAttribute.split(':')[0] + ':' + entityTypeId + '-' + entityAttributeId;
    else
        gName = entityAttribute;


    if (isCloneable.toLowerCase() === 'true')
        $(GetObject(chkBoxRCloneableUpdate)).prop('checked', true);
    else
        $(GetObject(chkBoxRCloneableUpdate)).prop('checked', false);

    var obj = {};
    obj.container = $('#' + divRTagUpdate);
    obj.list = tags.length > 0 ? tags.split(',') : [];
    tag.create(obj);


    //For fetching data for ref display attributes and Ismandatory

    var smdata = null;
    callRefAttr(entityTypeId).then(function (res) {
        var data = res;
        if (data != null) {
            smdata = data;
        }
        $('#hdnAttributeName').val(dispName);
        getRefDisplayAttributedetails(dispName, 'sm_ref_display_update', 'chkBoxRMandatoryUpdate', smdata);

        $find(modalUpdate).show();
        GetObject(txtDisplayUpdate).focus();
    });
}

/*************************************************************************************************
Function Name     : CallUpdateLegAtribPopUp 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function CallUpdateLegAtribPopUp(modalUpdateAtrib,
                              txtDisplayNameUpdate,
                               txtDescriptionUpdate,
                              txtUpdateStringAttribute,
                              txtUpdateNumericIntAttribute,
                              txtUpdateNumericDecimalAttribute,
                              lblUpdateSizeAttribute,
                              tdSizeUpdate,
                              hdnUpdateAtribRowId,
                              rowId,
                              rblDataType,
                              hdnDisplayName,
                              gridDetailAttributes,
                              lblDisplayNameUpdateError,
                              chkBoxCloneableUpdate,
                              hdnCloneable,
                              divTagUpdate,
                              tags,
                              deleteColumnIndex,
                              isLabelForRBL,
                              UpdateAttribDataTypeLabel) {
    var count;
    var gridObj = GetObject(gridDetailAttributes);
    var length = gridObj.rows.length;
    var dispName;
    var attributesize;
    var description;
    var commonCols;
    var isCloneable;
    var tagValue;

    GetObject(lblDisplayNameUpdateError).innerText = '';

    for (count = 1; count < length; count++) {
        if (trim(gridObj.rows[count].cells[parseInt(deleteColumnIndex)].innerText) == rowId) {
            dispName = trim(gridObj.rows[count].cells[0].innerText);
            description = trim(gridObj.rows[count].cells[1].innerText);
            dataType = trim(gridObj.rows[count].cells[2].innerText);
            attributesize = trim(gridObj.rows[count].cells[8].innerText);
            isCloneable = trim(gridObj.rows[count].cells[13].innerText);
            tagValue = trim(gridObj.rows[count].cells[14].innerText);
            break;
        }
    }
    if (dataType.toLowerCase() == 'string') {

        GetObject(txtUpdateStringAttribute).value = attributesize;
        GetObject(tdSizeUpdate).style.display = '';
        GetObject(txtUpdateStringAttribute).style.display = '';
        GetObject(txtUpdateNumericIntAttribute).style.display = 'none';
        GetObject(txtUpdateNumericDecimalAttribute).style.display = 'none';
        GetObject(lblUpdateSizeAttribute).style.display = 'none';

    }
    else if (dataType.toLowerCase() == 'numeric') {
        commonCols = attributesize.split('.');
        GetObject(txtUpdateNumericIntAttribute).value = commonCols[0];
        GetObject(txtUpdateNumericDecimalAttribute).value = commonCols[1];
        GetObject(tdSizeUpdate).style.display = '';
        GetObject(txtUpdateStringAttribute).style.display = 'none';
        GetObject(txtUpdateNumericIntAttribute).style.display = '';
        GetObject(txtUpdateNumericDecimalAttribute).style.display = '';
        GetObject(lblUpdateSizeAttribute).style.display = '';
    }
    else {
        GetObject(tdSizeUpdate).style.display = 'none';
        GetObject(txtDisplayNameUpdate).value = '';
        GetObject(txtDescriptionUpdate).value = '';
        GetObject(txtUpdateNumericIntAttribute).value = '';
        GetObject(txtUpdateNumericDecimalAttribute).value = '';
        GetObject(txtUpdateStringAttribute).style.display = 'none';
        GetObject(txtUpdateNumericIntAttribute).style.display = 'none';
        GetObject(txtUpdateNumericDecimalAttribute).style.display = 'none';
        GetObject(lblUpdateSizeAttribute).style.display = 'none';

    }

    GetObject(hdnUpdateAtribRowId).value = rowId;
    GetObject(txtDisplayNameUpdate).value = dispName;
    GetObject(txtDescriptionUpdate).value = description;
    GetObject(hdnDisplayName).value = dispName;
    var rblObj = GetObject(rblDataType);

    //    for (count = 0; count < rblObj.cells.length; count++) {
    //        if (rblObj.cells[count].innerText == dataType) {
    //            if (rblObj.cells[count].children[0].children.length == 0)
    //                rblObj.cells[count].children[0].checked = true;
    //            else
    //                rblObj.cells[count].children[0].children[0].checked = true;
    //            break;
    //        }
    //    }
    var rblCells = $('#' + rblDataType).find('td');
    for (count = 0; count < rblCells.length; count++) {
        if (rblCells.eq(count).text() == dataType) {
            if (rblCells.eq(count)[0].children[0].children.length == 0)
                rblCells.eq(count)[0].children[0].checked = true;
            else
                rblCells.eq(count)[0].children[0].children[0].checked = true;
            break;
        }
    }

    //if (hdnCloneable.toLowerCase() === 'true')
    if (isCloneable.toLowerCase() === 'true')
        $(GetObject(chkBoxCloneableUpdate)).prop('checked', true);
    else
        $(GetObject(chkBoxCloneableUpdate)).prop('checked', false);

    var obj = {};
    obj.container = $('#' + divTagUpdate);
    obj.list = tagValue.length > 0 ? tagValue.split(',') : [];
    tag.create(obj);


    if (isLabelForRBL) {
        $("#" + UpdateAttribDataTypeLabel).text(dataType);
    }

    $('#hdnAttributeName').val(dispName);
    getRefDisplayAttributedetails(dispName, null, 'chkBoxMandatoryUpdate', null);

    $find(modalUpdateAtrib).show();
    GetObject(txtDisplayNameUpdate).focus();
}



/*************************************************************************************************
Function Name     : DeleteRowInAtribTable 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
// gridDetailAttributes, deleteRowIndex, hdnLegXML, delColindex
function DeleteRowInAtribTable(grid, deleteRowID, hdnExpressionXMLId, delColindex, trgridSecTypeExisting, trSave, lblAttributeCount, DeleteAttrName) {

    if (delColindex === '10')
        delColindex = '11';

    var expressionsXML = '';
    var gridObj = GetObject(grid);
    var strExpressionRow = GetObject(hdnExpressionXMLId).value;
    var lblCount = GetObject(lblAttributeCount);
    strExpressionRow = strExpressionRow.replace(/&lts/g, '<');
    strExpressionRow = strExpressionRow.replace(/&gts/g, '>');
    //deleting selected row from table (by default this HTML table will have 2 rows)
    var rowIndex = 0;
    for (rowIndex = 1; rowIndex < gridObj.rows.length; rowIndex++) {
        if (parseInt(trim(gridObj.rows[rowIndex].cells[delColindex].innerText)) == parseInt(deleteRowID))
            break;
    }
    if (lblCount != null)
        lblCount.innerHTML = parseInt(lblCount.innerHTML) - 1;

    //rowIndex=rowIndex-1;
    gridObj.deleteRow(rowIndex);

    //Delete From Collection
    if (typeof DeleteAttrName != 'undefined' && DeleteAttrName != null) {
        saveRefDisplayAttributedetails(null, null, null, 'DELETE', null, DeleteAttrName);
    }

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
    GetObject(hdnExpressionXMLId).value = finalxml;
    try {
        if (gridObj.rows.length < 3) {
            GetObject(trgridSecTypeExisting).style.display = 'none';
            GetObject(trSave).style.display = 'none';
        }
    }
    catch (err)
    { }
} //End of function

/*************************************************************************************************
Function Name     : CallAtribPopUp 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function CallAtribPopUp(modalAddAtrib, txtDisplayName, txtDescription, lblTabNameError, rblDataType, txtStringLength, txtNumericIntLength, txtNumericDecimalLength, lblDecimal, tdSize, chkBoxCloneable, divTag, addAttribDataTypeSaver, AddAttribDataTypeLabel) {
    var dataType = $('#' + addAttribDataTypeSaver).attr('datatype');
    GetObject(txtDisplayName).value = '';
    GetObject(txtDescription).value = '';
    GetObject(lblTabNameError).innerText = '';
    $(GetObject(txtStringLength)).val("");
    $(GetObject(txtNumericIntLength)).val("");
    $(GetObject(txtNumericDecimalLength)).val("");
    $('#' + AddAttribDataTypeLabel).text(dataType);

    //GetObject(rblDataType).cells[0].children[0].checked = true;
    $('#' + rblDataType).find('td').eq(0)[0].children[0].checked = true;
    GetObject(txtStringLength).style.display = '';
    GetObject(txtStringLength).value = '200';
    GetObject(txtNumericIntLength).style.display = 'none';
    GetObject(txtNumericDecimalLength).style.display = 'none';
    GetObject(lblDecimal).style.display = 'none';
    GetObject(tdSize).style.display = '';
    $(GetObject(chkBoxCloneable)).prop('checked', true);

    $(GetObject(divTag)).html('');


    var rdl = $('#' + rblDataType).children().eq(0).children().find('label');
    for (var index = 0; index < rdl.length; index++) {
        if (rdl.eq(index).text().toLowerCase() == dataType.toLowerCase()) {
            var rrr = rdl.eq(index).parent().find('input');
            rrr.prop('checked', true);
            break;
        }
    }
    $('#' + rblDataType).click();

    var obj = {};
    obj.container = $('#' + divTag);
    obj.list = [];
    tag.create(obj);

    $find(modalAddAtrib).show();
    GetObject(txtDisplayName).focus();
}
function SetSelectionTextBoxes(modalAddAtrib, rblDataType, txtStringLength, txtNumericIntLength, txtNumericDecimalLength, lblDecimal, tdSize) {
    var rblObj = GetObject(rblDataType);
    var dataType;
    //    for (count = 0; count < rblObj.cells.length; count++)
    //        if (rblObj.cells[count].children[0].checked) {
    //            dataType = rblObj.cells[count].children[0].value;
    //            break;
    //        }
    //    rblObj.cells[count].children[0].checked = true;

    var rblCells = $('#' + rblDataType).find('td');
    for (count = 0; count < rblCells.length; count++) {
        if (rblCells.eq(count).find('input').eq(0).prop('checked').toString() == "true") {
            dataType = rblCells.eq(count).find('input').eq(0).val();
            rblCells.eq(count).find('input').eq(0).css('checked', true);
            break;
        }
    }
    if (dataType.toLowerCase() == 'string') {
        GetObject(txtStringLength).style.display = '';
        GetObject(txtStringLength).value = '200';
        GetObject(txtNumericIntLength).style.display = 'none';
        GetObject(txtNumericDecimalLength).style.display = 'none';
        GetObject(lblDecimal).style.display = 'none';
        GetObject(tdSize).style.display = '';

    }
    else if (dataType.toLowerCase() == 'numeric') {
        GetObject(txtStringLength).style.display = 'none';
        GetObject(txtNumericIntLength).value = '12';
        GetObject(txtNumericDecimalLength).value = '9';
        GetObject(txtNumericIntLength).style.display = '';
        GetObject(txtNumericDecimalLength).style.display = '';
        GetObject(lblDecimal).style.display = '';
        GetObject(tdSize).style.display = '';

    }
    else {
        GetObject(txtStringLength).style.display = 'none';
        GetObject(txtNumericIntLength).style.display = 'none';
        GetObject(txtNumericDecimalLength).style.display = 'none';
        GetObject(lblDecimal).style.display = 'none';
        GetObject(tdSize).style.display = 'none';

    }


}

function CallReferencePopUp(modalAddRefAtt, txtDisplayName, txtDescription, lblError, ddlEntityType, ddlEntityAttribute, trDetailEntityAttributeId, ddlDetailEntityAttributeId, chkBoxRCloneable, divRTag) {
    var obj = {};
    obj.container = $('#' + divRTag);
    obj.list = [];
    tag.create(obj);

    GetObject(txtDisplayName).value = '';
    GetObject(txtDescription).value = '';
    GetObject(lblError).innerText = '';
    GetObject(ddlEntityType).selectedIndex = 0;
    // GetObject(ddlEntityType).value = "-1";
    GetObject(ddlEntityAttribute).selectedIndex = 0;
    $(GetObject(chkBoxRCloneable)).prop('checked', true);
    $find(modalAddRefAtt).show();
    GetObject(txtDisplayName).focus();
    GetObject(trDetailEntityAttributeId).style.display = "none";
    GetObject(ddlDetailEntityAttributeId).selectedIndex = 0;

    $(GetObject(ddlEntityType)).unbind('change').bind('change', function (e) {
        e.stopPropagation();
        $('#hdnTagInfo').val($('#' + divRTag).find('.hdnTag').val());
        var div = $('#DivCreateLegTag');
        if (div.length > 0)
            $('#hdnLegTagInfo').val($(div).find('.hdnTag').val());
    });

    // new leg & master ref attr popup
    getRefDisplayAttributedetails(null, 'smtrRefDisplayName_dropdown', 'chkBoxRMandatory', null);
}
/*************************************************************************************************
Function Name     : AddRowAtrib 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function AddRowAtrib(gridSecTypeExisting,
                     txtDisplayName,
                     txtDescription,
                     colNames,
                     hdnExpressionXMLId,
                     hdnUpdateAtribRowId,
                     txtDisplayNameUpdate,
                      txtDescriptionUpdate,
                     modalUpdateAtrib,
                     modalAddAtrib, lblDisplayNameError, lstExistingAttributes,
                     trgridSecTypeExisting,
                     rblDataTypeAdd,
                     hdnDisplayName,
                     rblDataType,
                     lblDisplayNameUpdateError,
                     hdnAttributeList,
                     lstDetail,
                     trSave, txtStringLength,
                     txtNumericIntLength,
                     txtNumericDecimalLength,
                     txtUpdateStringAttribute,
                     txtUpdateNumericIntAttribute,
                     txtUpdateNumericDecimalAttribute,
                     lblUpdateSizeAttribute,
                     tdSizeUpdate, hdnAttributeSize,
                     lblAttributeCount,
                     chkBoxCloneable,
                     chkBoxCloneableUpdateNormal,
                     divTag,
                     divTagUpdateNormal) {
    var iButton;
    var iButtonNew;
    var strdummyRow = new Array();
    var count;
    var gridObj = GetObject(gridSecTypeExisting);
    var lstBoxObj = GetObject(lstExistingAttributes);
    var lstDetailObj = GetObject(lstDetail);
    var lstCount = lstBoxObj.length;
    var objDispName = GetObject(txtDisplayName);
    var objDescription = GetObject(txtDescription);
    var dispName = trim(objDispName.value.toLowerCase());
    var sizeofAttribute;
    var lblCount = GetObject(lblAttributeCount);
    var objchkBoxCloneable = $(GetObject(chkBoxCloneable));
    var objdivTag = $(GetObject(divTag));

    //Clear Error labels on Modal PopUp
    GetObject(lblDisplayNameError).innerText = '';

    // Check if the entered values are blank or Already existing: then raise error.
    if (trim(objDispName.value) == '') {
        GetObject(lblDisplayNameError).innerText = '● Please Enter Attribute Name';
        objDispName.focus();
        return false;
    }
    // Check If added Attribute is in Common Attributes.
    for (count = 0; count < lstCount; count++) {
        if (trim($(lstBoxObj[count]).textNBR().toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name is in Common Attributes';
            objDispName.focus();
            return false;
        }
    }
    lstCount = lstDetailObj.length;
    // Check If added Attribute is in Details Attributes.
    for (count = 0; count < lstCount; count++) {
        if (trim($(lstDetailObj[count]).textNBR().toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name Exists in Leg';
            objDispName.focus();
            return false;
        }
    }

    // Check If added Attribute is already Added.
    for (count = 2; count < gridObj.rows.length; count++) {
        if (trim($(gridObj.rows[count].cells[0]).textNBR().toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● Attribute Name Already Exists';
            objDispName.focus();
            return false;
        }
    }

    // Check If added Attribute is among List of prohibited Attributes.
    var commonCols = $(GetObject(hdnAttributeList)).valueNBR().split('|');
    lstCount = commonCols.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(commonCols[count].toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name is not Allowed';
            objDispName.focus();
            return false;
        }
    }
    var rblObj = GetObject(rblDataTypeAdd);
    var dataType;
    //    for (count = 0; count < rblObj.cells.length; count++) {
    //        if (rblObj.cells[count].children[0].checked) {
    //            dataType = rblObj.cells[count].children[0].value;
    //            // rblObj.cells[count].children[0].checked = true;
    //            break;
    //        }
    //    }
    var rblCells = $('#' + rblDataTypeAdd).find('td');
    for (count = 0; count < rblCells.length; count++) {
        if (rblCells.eq(count).children().eq(0).prop('checked').toString() == "true") {
            dataType = rblCells.eq(count)[0].children[0].value;
            //rblCells.eq(count)[0].children[0].checked = true;
            break;
        }
    }

    if (dataType.toLowerCase() == 'string') {
        if (trim(GetObject(txtStringLength).value) == '') {
            GetObject(lblDisplayNameError).innerText = '● Please Enter attribute size';
            return false;
        }
        if (parseInt(trim(GetObject(txtStringLength).value)) > 8000) {
            GetObject(lblDisplayNameError).innerText = '● Attribute Size Should Be Less Than Or Equal To 8000';
            return false;
        }
        else {
            var size = Number(trim(GetObject(txtStringLength).value))
            var result = size * 2;
            if (result == 0) {
                GetObject(lblDisplayNameError).innerText = '● Length Of Attribute Should Be Greater Than Zero';
                return false;

            }
            sizeofAttribute = trim(GetObject(txtStringLength).value);
        }

    }
    else if (dataType.toLowerCase() == 'numeric') {
        if (trim(GetObject(txtNumericIntLength).value) == '' || trim(GetObject(txtNumericDecimalLength).value) == '') {
            GetObject(lblDisplayNameError).innerText = '● Please Enter attribute size';
            return false;
        }
        if (Number(trim(GetObject(txtNumericIntLength).value)) + Number(trim(GetObject(txtNumericDecimalLength).value)) > 28) {
            GetObject(lblDisplayNameError).innerText = '● Precision Can Have A Maximum Value Of 28 (Precision is no. of digits before decimal + no. of digits after decimal) ';
            return false;
        }

        else
            sizeofAttribute = trim(GetObject(txtNumericIntLength).value) + '.' + trim(GetObject(txtNumericDecimalLength).value);
    }
    else
        sizeofAttribute = '';

    var delColindex = 2;

    var sectype_table_id = trim(gridObj.rows[gridObj.rows.length - 1].cells[11].innerText);

    //  var sectype_table_id = gridObj.children[1].children[2].cells[9].innerText;
    // Generate Unique Row ID for New Row
    var deleteRowIndex = trim(gridObj.rows[gridObj.rows.length - 1].cells[delColindex].innerText);
    deleteRowIndex = parseInt(deleteRowIndex) + 1;
    gridObj.insertRow(gridObj.rows.length);
    var i = gridObj.rows.length - 1;
    var gridRowObj = gridObj.rows[i];


    strdummyRow.push('0'); // Attribute_ID
    strdummyRow.push('_'); // Attribute_Name

    // Insert Display name
    gridRowObj.insertCell(0);
    gridRowObj.cells[0].innerText = trim(objDispName.value);
    strdummyRow.push(trim(objDispName.value));

    //description
    gridRowObj.insertCell(1);
    gridRowObj.cells[1].innerHTML = '<div class="overflowingDescription">' + trim(objDescription.value) + '</div>';
    gridRowObj.cells[1].innerText = trim(objDescription.value);
    //--->gridRowObj.cells[1].style.display = 'none';
    strdummyRow.push(trim(objDescription.value));


    //adding new column for RowID
    gridRowObj.insertCell(delColindex);
    gridRowObj.children[delColindex].innerText = deleteRowIndex;
    strdummyRow.push(deleteRowIndex);
    gridRowObj.cells[delColindex].style.display = 'none';

    // Insert Data Type
    gridRowObj.insertCell(3);
    gridRowObj.cells[3].innerText = dataType;
    strdummyRow.push(dataType);

    strdummyRow.push(sectype_table_id);
    // False for IsPermanent flag
    strdummyRow.push(false);
    // Insert sectype_table_id


    gridRowObj.insertCell(4);
    gridRowObj.cells[4].innerText = '';
    strdummyRow.push('0'); // Reference Type Id

    gridRowObj.cells[4].style.display = 'none';
    gridRowObj.insertCell(5);
    gridRowObj.cells[5].innerText = '';
    strdummyRow.push(''); //Reference Type

    gridRowObj.insertCell(6);
    gridRowObj.cells[6].innerText = '';
    strdummyRow.push(''); // Reference Attribute

    gridRowObj.insertCell(7);
    gridRowObj.cells[7].innerText = '';
    strdummyRow.push(''); // Reference Leg Attribute

    gridRowObj.insertCell(8);
    gridRowObj.cells[8].innerText = '';
    gridRowObj.cells[8].style.display = 'none';
    strdummyRow.push('0'); // Reference Attribute Id

    var iButton;
    var iButtonNew;

    gridRowObj.insertCell(9);
    gridRowObj.cells[9].innerText = sizeofAttribute;
    // gridRowObj.cells[6].style.display = 'none';
    // strdummyRow.push('0');  

    //    iButton = gridObj.rows[i - 1].children[7];
    //    iButtonNew = iButton.cloneNode(true);
    //    iButtonNew.innerText = sizeofAttribute;
    //   
    //    gridRowObj.insertCell(7).appendChild(iButtonNew);
    //    gridRowObj.cells[10].style.display = 'none';


    // Insert Buttons: Update
    var checkedCloneableValue = $(objchkBoxCloneable).is(":checked");
    var tagValue = $(objdivTag).find('.hdnTag').val();

    iButton = GetObject(gridObj.rows[1].children[10].children[0].id);
    iButtonNew = iButton.cloneNode(true);
    iButtonNew.disabled = false;
    iButtonNew.onclick = function () {
        CallUpdateAtribPopUp(modalUpdateAtrib, txtDisplayNameUpdate, txtDescriptionUpdate, txtUpdateStringAttribute, txtUpdateNumericIntAttribute, txtUpdateNumericDecimalAttribute, lblUpdateSizeAttribute, tdSizeUpdate, hdnAttributeSize, hdnUpdateAtribRowId, deleteRowIndex,
                                    rblDataType, hdnDisplayName, gridSecTypeExisting, lblDisplayNameUpdateError, chkBoxCloneableUpdateNormal, checkedCloneableValue.toString(), divTagUpdateNormal, tagValue, delColindex, false, 'UpdateAttribDataTypeLabel', 'UpdateNewNormalAttr'); return false;
    };
    gridRowObj.insertCell(10).appendChild(iButtonNew);
    gridRowObj.cells[10].align = 'center';


    // for update
    //   gridRowObj.cells[7].style.display = 'none';

    // Insert Buttons: Delete
    var iButton;
    var iButtonNew;
    iButton = GetObject(gridObj.rows[1].children[11].children[0].id);
    iButtonNew = iButton.cloneNode(true);
    iButtonNew.disabled = false;
    iButtonNew.onclick = function () { DeleteRowInAtribTable(gridSecTypeExisting, deleteRowIndex, hdnExpressionXMLId, delColindex, trgridSecTypeExisting, trSave, lblAttributeCount); return false; };

    gridRowObj.insertCell(11).appendChild(iButtonNew);
    gridRowObj.cells[11].align = 'center';


    iButton = gridObj.rows[i - 1].children[12];
    iButtonNew = iButton.cloneNode(true);
    $(iButtonNew).text(sectype_table_id);
    gridRowObj.insertCell(12).appendChild(iButtonNew);
    gridRowObj.cells[12].style.display = 'none';

    gridRowObj.insertCell(13);
    gridRowObj.cells[13].innerText = checkedCloneableValue.toString();
    gridRowObj.cells[13].style.display = 'none';

    gridRowObj.insertCell(14);
    gridRowObj.cells[14].innerText = tagValue;
    gridRowObj.cells[14].style.display = 'none';

    if (lblCount != null)
        lblCount.innerHTML = parseInt(lblCount.innerHTML) + 1;

    gridRowObj.className = 'normalRow';
    strdummyRow.push(false);
    if (sizeofAttribute != '') {
        strdummyRow.push(sizeofAttribute);
    }
    else {
        strdummyRow.push('-1');
    }
    if (checkedCloneableValue)
        strdummyRow.push('true');

    else
        strdummyRow.push('false');

    strdummyRow.push(tagValue);
    saveRefDisplayAttributedetails(txtDisplayName, null, 'chkBoxMandatory', 'ADD');
    $('#hdnAttributeName').val('');

    //************code to add new row in XML*********************
    UpdateXML(colNames, hdnExpressionXMLId, strdummyRow);
    $find(modalAddAtrib).hide();
    GetObject(trgridSecTypeExisting).style.display = '';
    GetObject(trSave).style.display = '';
    return false;
}
/*************************************************************************************************
Function Name     : AddRefRowAtrib 
Author            : Bhavya Jaitly
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function AddRefRowAtrib(gridSecTypeExisting,
                     txtDisplayName,
                     txtDescription,
                     colNames,
                     hdnExpressionXMLId,
                     hdnUpdateAtribRowId,
                     txtDisplayNameUpdate,
                     txtDescriptionUpdate,
                     modalUpdateAtrib,
                     modalAddAtrib,
                     lblDisplayNameError,
                     lstExistingAttributes,
                     trgridSecTypeExisting,
                     ddlEntityType,
                     ddlEntityAttribute,
                     hdnDisplayName,
                     lblDisplayNameUpdateError,
                     hdnAttributeList,
                     lstDetail,
                     trSave, ddlUpdateEntity, ddlUpdateEntityAttribute,
                     lblAttributeCount,
                     ddlLegAttributeNamesId,
                     ddlLegAttributeNameUpdateExistingId,
                     chkBoxRCloneable,
                     chkBoxUpdateCloneableRef,
                     divRTag,
                     divUpdateTagRef) {
    //,hdnReferenceType
    var iButton;
    var iButtonNew;
    var strdummyRow = new Array();
    var count;
    var gridObj = GetObject(gridSecTypeExisting);
    var lstBoxObj = GetObject(lstExistingAttributes);
    var lstDetailObj = GetObject(lstDetail);
    var lstCount = lstBoxObj.length;
    var objDispName = GetObject(txtDisplayName);
    var objDescription = GetObject(txtDescription);
    var dispName = trim(objDispName.value.toLowerCase());
    var lblCount = GetObject(lblAttributeCount);
    var ddlLegAttributeNames = GetObject(ddlLegAttributeNamesId);
    var objchkBoxCloneable = $(GetObject(chkBoxRCloneable));
    var objdivRTag = $(GetObject(divRTag));

    //Clear Error labels on Modal PopUp
    GetObject(lblDisplayNameError).innerText = '';

    // Check if the entered values are blank or Already existing: then raise error.
    if (trim(objDispName.value) == '') {
        GetObject(lblDisplayNameError).innerText = '● Please Enter Attribute Name';
        objDispName.focus();
        return false;
    }


    if (document.getElementById(ddlEntityType).options[document.getElementById(ddlEntityType).selectedIndex].value == "-1") {
        GetObject(lblDisplayNameError).innerText = '● Please select entity type ';
        return false;
    }
    if (document.getElementById(ddlEntityAttribute).options[document.getElementById(ddlEntityAttribute).selectedIndex].value == "-1") {
        GetObject(lblDisplayNameError).innerText = '● Please select reference attribute';
        return false;
    }
    // Check If added Attribute is in Common Attributes.
    for (count = 0; count < lstCount; count++) {
        if (trim(lstBoxObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name is in Common Attributes';
            objDispName.focus();
            return false;
        }
    }
    lstCount = lstDetailObj.length;
    // Check If added Attribute is in Details Attributes.
    for (count = 0; count < lstCount; count++) {
        if (trim(lstDetailObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name Exists in Leg';
            objDispName.focus();
            return false;
        }
    }

    // Check If added Attribute is already Added.
    for (count = 2; count < gridObj.rows.length; count++) {
        if (trim(gridObj.rows[count].cells[0].innerText.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● Attribute Name Already Exists';
            objDispName.focus();
            return false;
        }
    }

    // Check If added Attribute is among List of prohibited Attributes.
    var commonCols = GetObject(hdnAttributeList).value.split('|');
    lstCount = commonCols.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(commonCols[count].toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name is not Allowed';
            objDispName.focus();
            return false;
        }
    }

    var delColindex = 2;
    // Generate Unique Row ID for New Row
    var sectype_table_id = trim(gridObj.rows[gridObj.rows.length - 1].cells[11].innerText);
    var deleteRowIndex = trim(gridObj.rows[gridObj.rows.length - 1].cells[delColindex].innerText);
    deleteRowIndex = parseInt(deleteRowIndex) + 1;
    gridObj.insertRow(gridObj.rows.length);
    var i = gridObj.rows.length - 1;
    var gridRowObj = gridObj.rows[i];

    strdummyRow.push('0'); // Attribute_ID
    strdummyRow.push('_'); // Attribute_Name

    // Insert Display name
    gridRowObj.insertCell(0);
    gridRowObj.cells[0].innerText = trim(objDispName.value);
    strdummyRow.push(trim(objDispName.value));

    //description
    gridRowObj.insertCell(1);
    gridRowObj.cells[1].innerHTML = '<div class="overflowingDescription">' + trim(objDescription.value) + '</div>';
    gridRowObj.cells[1].innerText = trim(objDescription.value);
    //---> gridRowObj.cells[1].style.display = 'none';
    strdummyRow.push(trim(objDescription.value));

    //adding new column for RowID
    gridRowObj.insertCell(delColindex);
    gridRowObj.children[delColindex].innerText = deleteRowIndex;
    strdummyRow.push(deleteRowIndex);
    gridRowObj.cells[delColindex].style.display = 'none';

    // Insert Data Type
    gridRowObj.insertCell(3);
    // gridRowObj.cells[2].innerText =document.getElementById(hdnReferenceType).value;
    gridRowObj.cells[3].innerText = 'STRING';
    strdummyRow.push('STRING');
    // 
    //  strdummyRow.push(document.getElementById(hdnReferenceType).value);
    strdummyRow.push(sectype_table_id);
    // False for IsPermanent flag
    strdummyRow.push(false);

    var isCurveReferenceAttrib = false;

    if (ddlLegAttributeNames.disabled == false && ddlLegAttributeNames.value != null && ddlLegAttributeNames.value != "-1" && $get(ddlEntityAttribute).value != "")
        isCurveReferenceAttrib = true;
    gridRowObj.insertCell(4);
    gridRowObj.insertCell(5);
    gridRowObj.insertCell(6);
    gridRowObj.insertCell(7);
    gridRowObj.insertCell(8);

    if (!isCurveReferenceAttrib) {
        gridRowObj.cells[4].innerText = trim(document.getElementById(ddlEntityType).value);
        strdummyRow.push(document.getElementById(ddlEntityType).value);
        gridRowObj.cells[4].style.display = 'none';

        gridRowObj.cells[5].innerText = trim(document.getElementById(ddlEntityType).options[GetObject(ddlEntityType).selectedIndex].text);
        strdummyRow.push(document.getElementById(ddlEntityType).options[GetObject(ddlEntityType).selectedIndex].text);

        var len = document.getElementById(ddlEntityAttribute).length;
        var selectedEntityType;
        for (var x = 0; x < len; x++) {
            if (document.getElementById(ddlEntityAttribute)[x].selected == true)
                selectedEntityType = trim(document.getElementById(ddlEntityAttribute)[x].text);
        }
        gridRowObj.cells[6].innerText = selectedEntityType;
        strdummyRow.push(selectedEntityType);
        strdummyRow.push("");
        //------------------------------------------------------------------------------------------------------------------------------------------------------------

        gridRowObj.cells[8].innerText = trim(document.getElementById(ddlEntityAttribute).value);
        strdummyRow.push(document.getElementById(ddlEntityAttribute).value);
        gridRowObj.cells[8].style.display = 'none';
    }
    else {
        var detailEntityTypeId = ddlLegAttributeNames.value.split('-')[0];
        var detailEntityAttributeId = ddlLegAttributeNames.value.split('-')[1];
        var detailEntitytAttributeName = ddlLegAttributeNames.options[ddlLegAttributeNames.selectedIndex].text.replace('-', ':');
        var masterEntityAttributeId = $get(ddlEntityAttribute).value;
        var masterEntityAttributeName = $get(ddlEntityAttribute).options[$get(ddlEntityAttribute).selectedIndex].text;
        var masterEntityTypeName = $get(ddlEntityType).options[$get(ddlEntityType).selectedIndex].text;

        gridRowObj.cells[4].innerText = detailEntityTypeId;
        strdummyRow.push(detailEntityTypeId);
        gridRowObj.cells[4].style.display = 'none';

        gridRowObj.cells[5].innerText = masterEntityTypeName
        strdummyRow.push(masterEntityTypeName);

        gridRowObj.cells[6].innerText = masterEntityAttributeName;
        strdummyRow.push(masterEntityAttributeName);

        strdummyRow.push(detailEntitytAttributeName);
        gridRowObj.cells[7].innerText = detailEntitytAttributeName;

        gridRowObj.cells[8].innerText = detailEntityAttributeId;
        strdummyRow.push(detailEntityAttributeId);
        gridRowObj.cells[8].style.display = 'none';
    }


    var iButton;
    var iButtonNew;

    gridRowObj.insertCell(9);
    gridRowObj.cells[9].innerText = '';
    //    iButton = gridObj.rows[i - 1].children[7];
    //    iButtonNew = iButton.cloneNode(true);
    //    iButtonNew.innerText = "";
    //   
    //    gridRowObj.insertCell(7).appendChild(iButtonNew);
    // Insert Buttons: Update
    var cloneableValue = $(objchkBoxCloneable).is(":checked");
    var tagValue = $(objdivRTag).find('.hdnTag').val();

    iButton = GetObject(gridObj.rows[1].children[10].children[0].id);
    iButtonNew = iButton.cloneNode(true);
    iButtonNew.disabled = false;
    iButtonNew.onclick = function () {
        CallUpdateRefAtribPopUp(modalUpdateAtrib, txtDisplayNameUpdate, txtDescriptionUpdate, hdnUpdateAtribRowId, deleteRowIndex,
                                     ddlUpdateEntity, ddlUpdateEntityAttribute, hdnDisplayName, gridSecTypeExisting,
                                     lblDisplayNameUpdateError, ddlLegAttributeNameUpdateExistingId, chkBoxUpdateCloneableRef, cloneableValue.toString(), divUpdateTagRef, tagValue, delColindex, 'UpdateNewRefAttr'); return false;
    };
    gridRowObj.insertCell(10).appendChild(iButtonNew);
    gridRowObj.cells[10].align = 'center';


    // for  update
    // gridRowObj.cells[7].style.display = 'none';

    var iButton;
    var iButtonNew;
    iButton = GetObject(gridObj.rows[1].children[11].children[0].id);
    iButtonNew = iButton.cloneNode(true);
    iButtonNew.disabled = false;
    iButtonNew.onclick = function () { DeleteRowInAtribTable(gridSecTypeExisting, deleteRowIndex, hdnExpressionXMLId, delColindex, trgridSecTypeExisting, trSave, lblAttributeCount); return false; };

    gridRowObj.insertCell(11).appendChild(iButtonNew);
    gridRowObj.cells[11].align = 'center';


    iButton = gridObj.rows[i - 1].children[12];
    iButtonNew = iButton.cloneNode(true);
    $(iButtonNew).text(sectype_table_id);
    gridRowObj.insertCell(12).appendChild(iButtonNew);
    gridRowObj.cells[12].style.display = 'none';


    gridRowObj.insertCell(13);
    gridRowObj.cells[13].innerText = cloneableValue.toString();
    gridRowObj.cells[13].style.display = 'none';

    gridRowObj.insertCell(14);
    gridRowObj.cells[14].innerText = tagValue;
    gridRowObj.cells[14].style.display = 'none';

    if (lblCount != null)
        lblCount.innerHTML = parseInt(lblCount.innerHTML) + 1;

    gridRowObj.className = 'normalRow';
    strdummyRow.push(true);
    strdummyRow.push('-1');
    if (cloneableValue)
        strdummyRow.push('true');

    else
        strdummyRow.push('false');

    strdummyRow.push(tagValue);
    saveRefDisplayAttributedetails(txtDisplayName, 'smtrRefDisplayName_dropdown', 'chkBoxRMandatory', 'ADD');
    $('#hdnAttributeName').val('');

    //************code to add new row in XML*********************
    UpdateXML(colNames, hdnExpressionXMLId, strdummyRow);
    $find(modalAddAtrib).hide();
    GetObject(trgridSecTypeExisting).style.display = '';
    GetObject(trSave).style.display = '';
    // return false;
}


/*************************************************************************************************
Function Name     : UpdateAtribRow 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function UpdateAtribRow(grid,
                        hdnUpdateAtribRowId,
                        txtDisplayNameUpdate,
                        txtDescription,
                        hdnExpressionXMLId,
                        lblDisplayNameUpdateError,
                        modalUpdateAtrib,
                        lstExistingAttributes,
                        rblDataType,
                        hdnDisplayName,
                        hdnAttributeList,
                        lstDetail,
                        txtUpdateStringAttribute,
                        txtUpdateNumericIntAttribute,
                        txtUpdateNumericDecimalAttribute,
                        chkBoxCloneableUpdateNormal,
                        divTagUpdateNormal) {
    var gridObj = GetObject(grid);
    var lstBoxObj = GetObject(lstExistingAttributes);
    var lstDetailObj = GetObject(lstDetail);
    var lstCount = lstBoxObj.length;
    var count;
    var updatedAttributeSize;
    GetObject(lblDisplayNameUpdateError).innerText = '';
    var updatedDispName = trim(GetObject(txtDisplayNameUpdate).value);
    var updatedDispNameL = updatedDispName.toLowerCase();

    var updatedDescription = $("[id$=txtDescriptionUpdate]").val();
    // Check If Attribute name is Empty.
    if (updatedDispName == '') {
        GetObject(lblDisplayNameUpdateError).innerText = '● Enter Attribute Name';
        GetObject(txtDisplayNameUpdate).focus();
        return false;
    }
    // Check If added Attribute is in Common Attributes
    for (count = 0; count < lstCount; count++) {
        if (trim(lstBoxObj[count].text.toLowerCase()) == updatedDispNameL) {
            GetObject(lblDisplayNameUpdateError).innerText = '● Attribute Name Already Exists';
            GetObject(txtDisplayNameUpdate).focus();
            return false;
        }
    }
    // Check If added Attribute is in Details Attributes.
    lstCount = lstDetailObj.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(lstDetailObj[count].text.toLowerCase()) == updatedDispNameL) {
            GetObject(lblDisplayNameUpdateError).innerText = '● This Attribute Name Exists in Leg';
            GetObject(txtDisplayNameUpdate).focus();
            return false;
        }
    }
    // Check If added Attribute is already Added
    for (count = 2; count < gridObj.rows.length; count++) {
        if (updatedDispNameL == trim(gridObj.rows[count].cells[0].innerText.toLowerCase())
            && GetObject(hdnDisplayName).value.toLowerCase() != updatedDispName.toLowerCase()) {
            GetObject(lblDisplayNameUpdateError).innerText = '● Attribute Name Already Exist';
            GetObject(txtDisplayNameUpdate).focus();
            return false;
        }
    }

    // Check If added Attribute is among List of prohibited Attributes.
    var commonCols = GetObject(hdnAttributeList).value.split('|');
    lstCount = commonCols.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(commonCols[count].toLowerCase()) == updatedDispNameL) {
            GetObject(lblDisplayNameUpdateError).innerText = '● This Attribute Name is not Allowed';
            GetObject(txtDisplayNameUpdate).focus();
            return false;
        }
    }

    var rblObj = GetObject(rblDataType);
    var dataType;
    //    for (count = 0; count < rblObj.cells.length; count++)
    //        if (rblObj.cells[count].children[0].checked) {
    //            dataType = rblObj.cells[count].children[0].value;
    //            break;
    //        }
    var rblCells = $('#' + rblDataType).find('td');
    for (count = 0; count < rblCells.length; count++) {
        if (rblCells.eq(count).children().eq(0).prop('checked').toString() == "true") {
            dataType = rblCells.eq(count)[0].children[0].value;
            break;
        }
    }
    var deleteRowID = GetObject(hdnUpdateAtribRowId).value;
    var expressionsXML = '';
    var strExpressionRow = GetObject(hdnExpressionXMLId).value;
    strExpressionRow = strExpressionRow.replace(/&lts/g, '<');
    strExpressionRow = strExpressionRow.replace(/&gts/g, '>');
    //deleting selected row from table (by default this HTML table will have 2 rows)
    var rowIndex = 0;
    for (rowIndex = 1; rowIndex < gridObj.rows.length; rowIndex++) {
        if (parseInt(gridObj.rows[rowIndex].cells[2].innerText) == parseInt(deleteRowID))//---->
            break;
    }

    if (dataType.toLowerCase() == 'string') {
        if (trim(GetObject(txtUpdateStringAttribute).value) == '') {
            GetObject(lblDisplayNameUpdateError).innerText = '● Enter Attribute Size';
            return false;
        }
        if (parseInt(trim(GetObject(txtUpdateStringAttribute).value)) > 8000) {
            GetObject(lblDisplayNameUpdateError).innerText = '● Attribute Size Should Be Less Than Or Equal To 8000';
            return false;
        }
        else {
            gridObj.rows[rowIndex].cells[9].innerText = trim(GetObject(txtUpdateStringAttribute).value); //--->cells[8]

            gridObj.rows[rowIndex].cells[9].align = 'left'; //--->cells[8]
        }
    }
    else if (dataType.toLowerCase() == 'numeric') {
        if (trim(GetObject(txtUpdateNumericDecimalAttribute).value) == '' || trim(GetObject(txtUpdateNumericIntAttribute).value) == '') {
            GetObject(lblDisplayNameUpdateError).innerText = '● Enter Attribute Size';
            return false;
        }
        if (Number(trim(GetObject(txtUpdateNumericIntAttribute).value)) + Number(trim(GetObject(txtUpdateNumericDecimalAttribute).value)) > 28) {
            GetObject(lblDisplayNameUpdateError).innerText = '● Precision Can Have A Maximum Value Of 28 (Precision is no. of digits before decimal + no. of digits after decimal) ';
            return false;
        }

        gridObj.rows[rowIndex].cells[9].innerText = trim(GetObject(txtUpdateNumericIntAttribute).value) + '.' + trim(GetObject(txtUpdateNumericDecimalAttribute).value); //---> cells[8]
        gridObj.rows[rowIndex].cells[9].align = 'left'; //--->cells[8]
    }
    else {
        gridObj.rows[rowIndex].cells[9].innerText = ''; //--->cells[8]
        gridObj.rows[rowIndex].cells[9].align = 'left'; //--->cells[8]
    }

    updatedAttributeSize = trim(gridObj.rows[rowIndex].cells[9].innerText); //--->cells[8]
    gridObj.rows[rowIndex].cells[0].innerText = updatedDispName;
    gridObj.rows[rowIndex].cells[0].align = 'left';

    gridObj.rows[rowIndex].cells[1].innerHTML = '<div class="overflowingDescription">' + updatedDescription + '</div>';
    gridObj.rows[rowIndex].cells[1].align = 'left';

    gridObj.rows[rowIndex].cells[3].innerText = dataType; //--->cells[2]
    gridObj.rows[rowIndex].cells[3].align = 'left'; //--->cells[2]


    var isCloneable = $('#' + chkBoxCloneableUpdateNormal).is(':checked');
    var tagValue = $('#' + divTagUpdateNormal).find('.hdnTag').val();

    gridObj.rows[rowIndex].cells[13].innerText = isCloneable.toString();
    gridObj.rows[rowIndex].cells[14].innerText = tagValue;

    //deleting row from XML
    var tableArray = strExpressionRow.split('<Table>');
    var finalxml = '';

    for (var i = 0; i < tableArray.length; i++) {
        finalxml += '<Table>';
        if (i != rowIndex)
            finalxml += tableArray[i];
        else {
            var startindex = tableArray[i].indexOf('<Display_Name>') + 14;
            var Endindex = tableArray[i].indexOf('</Display_Name>');
            var firsthalf = tableArray[i].substring(0, startindex);
            var Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + updatedDispName + Secondhalf;

            updatedDescription = updatedDescription.replaceAll('<', 'ž').replaceAll('>', 'Ÿ');

            startindex = tableArray[i].indexOf('<Attribute_x0020_Description>') + 29;
            Endindex = tableArray[i].indexOf('</Attribute_x0020_Description>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + updatedDescription + Secondhalf;

            startindex = tableArray[i].indexOf('<Data_Type>') + 11;
            Endindex = tableArray[i].indexOf('</Data_Type>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + dataType + Secondhalf;

            startindex = tableArray[i].indexOf('<attribute_size>') + 16;
            Endindex = tableArray[i].indexOf('</attribute_size>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            if (updatedAttributeSize != '') {
                tableArray[i] = firsthalf + updatedAttributeSize + Secondhalf;
            }
            else {
                tableArray[i] = firsthalf + '-1' + Secondhalf;
            }

            startindex = tableArray[i].indexOf('<is_cloneable>') + 14;
            Endindex = tableArray[i].indexOf('</is_cloneable>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + isCloneable.toString() + Secondhalf;

            startindex = tableArray[i].indexOf('<tags>') + 6;
            Endindex = tableArray[i].indexOf('</tags>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + tagValue + Secondhalf;

            finalxml += tableArray[i];
        }
    }
    if (finalxml.indexOf('<Table>') == 0) { finalxml = finalxml.substring(7); }
    if (finalxml.lastIndexOf('</NewDataSet>') == -1) { finalxml += '</NewDataSet>'; }

    finalxml = finalxml.replace(/</g, '&lts');
    finalxml = finalxml.replace(/>/g, '&gts');
    GetObject(hdnExpressionXMLId).value = finalxml;

    saveRefDisplayAttributedetails(txtDisplayNameUpdate, null, 'chkBoxMandatoryUpdateNormal', 'UPDATE');
    $('#hdnAttributeName').val('');

    $find(modalUpdateAtrib).hide();
} //End of function

/*************************************************************************************************
Function Name     : UpdateRefAtribRow 
Author            : Bhavya Jaitly
Modified By       : 
Description       :
Page used in      : 
Parameters        : 
                        
Modification Log  : 
*************************************************************************************************/
function UpdateRefAtribRow(grid,
                        hdnUpdateAtribRowId,
                        txtDisplayNameUpdate,
                        hdnExpressionXMLId,
                        lblDisplayNameUpdateError,
                        modalUpdateAtrib,
                        lstExistingAttributes,
                        ddlEntity, ddlEntityAttribute,
                        hdnDisplayName,
                        hdnAttributeList,
                        lstDetail, hdnIsRefUpdate,
                        ddlLegAttributeNamesId,
                        chkBoxUpdateCloneableRef,
                        divUpdateTagRef) {

    var gridObj = GetObject(grid);
    var lstBoxObj = GetObject(lstExistingAttributes);
    var lstDetailObj = GetObject(lstDetail);
    var lstCount = lstBoxObj.length;
    var count;
    GetObject(lblDisplayNameUpdateError).innerText = '';
    var updatedDispName = trim(GetObject(txtDisplayNameUpdate).value);
    var updatedDispNameL = updatedDispName.toLowerCase();

    var updatedDescription = $("[id$=txtUpdateExistingRefAttributeDescription]").val();
    var ddlLegAttributeNames = $get(ddlLegAttributeNamesId);

    if (document.getElementById(ddlEntity).options[document.getElementById(ddlEntity).selectedIndex].value == "-1") {
        GetObject(lblDisplayNameUpdateError).innerText = '● Please select entity type ';
        return false;
    }
    if (document.getElementById(ddlEntityAttribute).options[document.getElementById(ddlEntityAttribute).selectedIndex].value == "-1") {
        GetObject(lblDisplayNameUpdateError).innerText = '● Please select reference attribute';

        return false;
    }


    // Check If Attribute name is Empty.
    if (updatedDispName == '') {
        GetObject(lblDisplayNameUpdateError).innerText = '● Enter Attribute Name';
        GetObject(txtDisplayNameUpdate).focus();
        return false;
    }
    // Check If added Attribute is in Common Attributes
    for (count = 0; count < lstCount; count++) {
        if (trim(lstBoxObj[count].text.toLowerCase()) == updatedDispNameL) {
            GetObject(lblDisplayNameUpdateError).innerText = '● Attribute Name Already Exists';
            GetObject(txtDisplayNameUpdate).focus();
            return false;
        }
    }
    // Check If added Attribute is in Details Attributes.
    lstCount = lstDetailObj.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(lstDetailObj[count].text.toLowerCase()) == updatedDispNameL) {
            GetObject(lblDisplayNameUpdateError).innerText = '● This Attribute Name Exists in Leg';
            GetObject(txtDisplayNameUpdate).focus();
            return false;
        }
    }
    // Check If added Attribute is already Added
    for (count = 2; count < gridObj.rows.length; count++) {
        if (updatedDispNameL == trim(gridObj.rows[count].cells[0].innerText.toLowerCase())
            && GetObject(hdnDisplayName).value.toLowerCase() != updatedDispName.toLowerCase()) {
            GetObject(lblDisplayNameUpdateError).innerText = '● Attribute Name Already Exist';
            GetObject(txtDisplayNameUpdate).focus();
            return false;
        }
    }

    // Check If added Attribute is among List of prohibited Attributes.
    var commonCols = GetObject(hdnAttributeList).value.split('|');
    lstCount = commonCols.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(commonCols[count].toLowerCase()) == updatedDispNameL) {
            GetObject(lblDisplayNameUpdateError).innerText = '● This Attribute Name is not Allowed';
            GetObject(txtDisplayNameUpdate).focus();
            return false;
        }
    }

    GetObject(hdnIsRefUpdate).value = '1';
    var dataType;

    var deleteRowID = GetObject(hdnUpdateAtribRowId).value;
    var expressionsXML = '';
    var strExpressionRow = GetObject(hdnExpressionXMLId).value;
    strExpressionRow = strExpressionRow.replace(/&lts/g, '<');
    strExpressionRow = strExpressionRow.replace(/&gts/g, '>');
    //deleting selected row from table (by default this HTML table will have 2 rows)
    var rowIndex = 0;
    for (rowIndex = 1; rowIndex < gridObj.rows.length; rowIndex++) {
        if (parseInt(trim(gridObj.rows[rowIndex].cells[2].innerText)) == parseInt(deleteRowID))//---> 
            break;
    }
    gridObj.rows[rowIndex].cells[0].innerText = updatedDispName;
    gridObj.rows[rowIndex].cells[0].align = 'left';

    gridObj.rows[rowIndex].cells[1].innerHTML = '<div class="overflowingDescription">' + updatedDescription + '</div>';
    gridObj.rows[rowIndex].cells[1].align = 'left';


    gridObj.rows[rowIndex].cells[3].innerText = 'STRING'; //--->cells[2]
    gridObj.rows[rowIndex].cells[3].align = 'left'; //--->cells[2]

    var isCurveReferenceAttrib = false;

    if (ddlLegAttributeNames.disabled == false && ddlLegAttributeNames.value != null && ddlLegAttributeNames.value != "-1" && $get(ddlEntityAttribute).value != "")
        isCurveReferenceAttrib = true;

    if (!isCurveReferenceAttrib) {
        gridObj.rows[rowIndex].cells[4].innerText = trim(document.getElementById(ddlEntity).value); //--->4
        gridObj.rows[rowIndex].cells[5].innerText = trim(document.getElementById(ddlEntity).options[GetObject(ddlEntity).selectedIndex].text); //--->4
        var len = document.getElementById(ddlEntityAttribute).length;
        var selectedEntityType;
        for (var x = 0; x < len; x++) {
            if (document.getElementById(ddlEntityAttribute)[x].selected == true)
                selectedEntityType = trim(document.getElementById(ddlEntityAttribute)[x].text);
        }
        gridObj.rows[rowIndex].cells[6].innerText = selectedEntityType; //--->5
        gridObj.rows[rowIndex].cells[8].innerText = trim(document.getElementById(ddlEntityAttribute).value); //--->6
    }
    else {
        var detailEntityTypeId = trim(ddlLegAttributeNames.value).split('-')[0];
        var detailEntityAttributeId = trim(ddlLegAttributeNames.value).split('-')[1];
        var detailEntitytAttributeName = trim(ddlLegAttributeNames.options[ddlLegAttributeNames.selectedIndex].text).split('-')[1];
        var masterEntityAttributeId = trim($get(ddlEntityAttribute).value);
        var masterEntityAttributeName = trim($get(ddlEntityAttribute).options[$get(ddlEntityAttribute).selectedIndex].text);
        var masterEntityTypeName = trim($get(ddlEntity).options[$get(ddlEntity).selectedIndex].text);

        gridObj.rows[rowIndex].cells[4].innerText = detailEntityTypeId; //--->3
        gridObj.rows[rowIndex].cells[5].innerText = masterEntityTypeName//--->4
        gridObj.rows[rowIndex].cells[6].innerText = masterEntityAttributeName + ':' + detailEntitytAttributeName; //--->5
        //        gridObj.rows[rowIndex].cells[6].innerText = masterEntityAttributeId + ':' + detailEntityAttributeId;
        gridObj.rows[rowIndex].cells[8].innerText = detailEntityAttributeId; //--->7
    }

    var isCloneable = $('#' + chkBoxUpdateCloneableRef).is(':checked');
    gridObj.rows[rowIndex].cells[13].innerText = isCloneable.toString();

    var tagValue = $('#' + divUpdateTagRef).find('.hdnTag').val();
    gridObj.rows[rowIndex].cells[14].innerText = tagValue;

    //deleting row from XML
    var tableArray = strExpressionRow.split('<Table>');
    var finalxml = '';

    for (var i = 0; i < tableArray.length; i++) {
        finalxml += '<Table>';
        if (i != rowIndex)
            finalxml += tableArray[i];
        else {
            var startindex = tableArray[i].indexOf('<Display_Name>') + 14;
            var Endindex = tableArray[i].indexOf('</Display_Name>');
            var firsthalf = tableArray[i].substring(0, startindex);
            var Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + updatedDispName + Secondhalf;

            updatedDescription = updatedDescription.replaceAll('<', 'ž').replaceAll('>', 'Ÿ');

            startindex = tableArray[i].indexOf('<Attribute_x0020_Description>') + 29;
            Endindex = tableArray[i].indexOf('</Attribute_x0020_Description>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + updatedDescription + Secondhalf;


            startindex = tableArray[i].indexOf('<reference_type_id>') + 19;
            Endindex = tableArray[i].indexOf('</reference_type_id>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + trim(gridObj.rows[rowIndex].cells[4].innerText) + Secondhalf; //--->3

            startindex = tableArray[i].indexOf('<reference_type>') + 16;
            Endindex = tableArray[i].indexOf('</reference_type>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + trim(gridObj.rows[rowIndex].cells[5].innerText) + Secondhalf; //--->4

            startindex = tableArray[i].indexOf('<reference_attribute_name>') + 26;
            Endindex = tableArray[i].indexOf('</reference_attribute_name>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + trim(gridObj.rows[rowIndex].cells[6].innerText) + Secondhalf; //--->5

            startindex = tableArray[i].indexOf('<reference_attribute_id>') + 24;
            Endindex = tableArray[i].indexOf('</reference_attribute_id>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + trim(gridObj.rows[rowIndex].cells[8].innerText) + Secondhalf; //--->6

            startindex = tableArray[i].indexOf('<is_cloneable>') + 14;
            Endindex = tableArray[i].indexOf('</is_cloneable>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + isCloneable.toString() + Secondhalf;

            startindex = tableArray[i].indexOf('<tags>') + 6;
            Endindex = tableArray[i].indexOf('</tags>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + tagValue + Secondhalf;

            finalxml += tableArray[i];
        }
    }
    if (finalxml.indexOf('<Table>') == 0) { finalxml = finalxml.substring(7); }
    if (finalxml.lastIndexOf('</NewDataSet>') == -1) { finalxml += '</NewDataSet>'; }

    finalxml = finalxml.replace(/</g, '&lts');
    finalxml = finalxml.replace(/>/g, '&gts');
    GetObject(hdnExpressionXMLId).value = finalxml;

    //save updated values for ref attr
    saveRefDisplayAttributedetails(txtDisplayNameUpdate, 'smtrRefDisplayNameUpdate_dropdown', 'chkBoxUpdateMandatoryRef', 'UPDATE');
    $('#hdnAttributeName').val('');

    $find(modalUpdateAtrib).hide();
}

/*************************************************************************************************
Function Name     : CheckTabName
Author            : Varun Temani
Description       : Checks the uniqueness of Tab name
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function CheckTabName(hiddenTab, txtTabName, lblTabError) {
    var str = GetObject(hiddenTab).value.split('|');
    var TabName = trim(GetObject(txtTabName).value).toLowerCase();
    if (TabName == '') {
        GetObject(lblTabError).innerText = '● Please Enter Tab Name';
        return false;
    }
    var count;
    for (count = 0; count < str.length; count++) {
        if (trim(str[count]).toLowerCase() == TabName) {
            GetObject(lblTabError).innerText = '● Tab name already exists';
            return false;
        }
    }
    return true;
}
/*************************************************************************************************
Function Name     : CheckSubTabName

Author            : Varun Temani
Description       : Checks the uniqueness of SubTab name
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function CheckSubTabName(hiddenTab, txtSubTabName, lblSubTabNameError) {
    var str = GetObject(hiddenTab).value.split('|');
    var SubTabName = trim(GetObject(txtSubTabName).value).toLowerCase();
    if (SubTabName == '') {
        GetObject(lblSubTabNameError).innerText = '● Please Enter SubTab Name';
        return false;
    }
    var count;
    for (count = 0; count < str.length; count++) {
        if (trim(str[count]).toLowerCase() == SubTabName) {
            GetObject(lblSubTabNameError).innerText = '● SubTab name already exists';
            return false;
        }
    }

    return true;
}
/*************************************************************************************************
Function Name     : CallSubTabPopup
Author            : Varun Temani
Description       : Shows the SubTab popup
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function CallSubTab(modalAddSubTab, txtSubTabName, lblSubTabNameError, hdnPassTabID,
                    hdnPassTabRowId, hdnPassTabName, passTabId, passRowTabId, lblTabNameSub, lbltabname) {
    GetObject(txtSubTabName).value = '';
    GetObject(lblSubTabNameError).innerText = '';
    GetObject(lblTabNameSub).innerText = lbltabname;
    GetObject(hdnPassTabName).value = lbltabname;
    $find(modalAddSubTab).show();
    GetObject(txtSubTabName).focus();
    GetObject(hdnPassTabID).value = passTabId;
    GetObject(hdnPassTabRowId).value = passRowTabId;

}
/*************************************************************************************************
Function Name     : TM_CallUpdateTabPopUp
Author            : Varun Temani
Description       : Calls Update Tab popup
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function TM_CallUpdateTabPopUp(modalUpdateTab, lbltabname, txtTabUpdateName, hdnUpdateTabRowId, PassRowTabId, hdnCheckTabName, lblTabUpdateNameError) {
    GetObject(txtTabUpdateName).value = lbltabname;
    GetObject(hdnUpdateTabRowId).value = PassRowTabId;
    GetObject(lblTabUpdateNameError).innerText = '';
    GetObject(hdnCheckTabName).value = lbltabname;
    $find(modalUpdateTab).show();
    GetObject(txtTabUpdateName).focus();
}
/*************************************************************************************************
Function Name     : TM_UpdateTabName
Author            : Varun Temani
Description       : Updates the Tab name
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function TM_UpdateTabName(hdnTabName, txtTabUpdateName, hdnUpdateTabRowId, hdnDisplayName, hdnCheckTabName, lblUpdateTabName, lblTabUpdateNameError) {
    var str = GetObject(hdnTabName).value.split('|');
    var TabName = trim(GetObject(txtTabUpdateName).value).toLowerCase();
    if (TabName == '') {
        GetObject(lblTabUpdateNameError).innerText = '● Please Enter Tab Name';
        return false;
    }
    if (TabName == trim(GetObject(hdnCheckTabName).value).toLowerCase()) {
        GetObject(hdnDisplayName).value = trim(GetObject(txtTabUpdateName).value);
        GetObject(lblUpdateTabName).innerText = trim(GetObject(txtTabUpdateName).value);
        return true;
    }
    var count;
    for (count = 0; count < str.length; count++) {
        if (trim(str[count]).toLowerCase() == TabName) {
            GetObject(lblTabUpdateNameError).innerText = '● Tab name already exists';
            return false;
        }
    }
    GetObject(hdnDisplayName).value = trim(GetObject(txtTabUpdateName).value);
    GetObject(lblUpdateTabName).innerText = trim(GetObject(txtTabUpdateName).value);
    return true;
}
/*************************************************************************************************
Function Name     : TM_CallUpdateSubTabPopUp
Author            : Varun Temani
Description       : Calls Update SubTab popup
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function TM_CallUpdateSubTabPopUp(modalUpdateSubTab, lblSubTabname, txtUpdateSubTabName, hdnRowIdforSubTab, lblUpdateTabName, lblTabname, PassSubTabRowId, hdnCheckSubTabName, lblUpdateSubTabNameError) {
    GetObject(txtUpdateSubTabName).value = trim(lblSubTabname);
    GetObject(hdnRowIdforSubTab).value = PassSubTabRowId;
    GetObject(lblUpdateSubTabNameError).innerText = '';
    GetObject(lblUpdateTabName).innerText = lblTabname;
    GetObject(hdnCheckSubTabName).value = trim(GetObject(txtUpdateSubTabName).value);
    $find(modalUpdateSubTab).show();
    GetObject(txtUpdateSubTabName).focus();
}
/*************************************************************************************************
Function Name     : TM_UpdateSubTabName
Author            : Varun Temani
Description       : Updates the SubTab name
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function TM_UpdateSubTabName(hdnSubTabName, txtUpdateSubTabName, hdnRowIdforSubTab, hdnDisplayName, hdnCheckSubTabName, lblSubTabNameError) {
    var str = GetObject(hdnSubTabName).value.split('|');
    var SubTabName = trim(GetObject(txtUpdateSubTabName).value).toLowerCase();
    if (SubTabName == '') {
        GetObject(lblSubTabNameError).innerText = '● Please Enter Sub Tab Name';
        return false;
    }
    if (SubTabName == trim(GetObject(hdnCheckSubTabName).value).toLowerCase()) {
        GetObject(hdnDisplayName).value = trim(GetObject(txtUpdateSubTabName).value);
        return true;
    }
    var count;
    for (count = 0; count < str.length; count++) {
        if (trim(str[count]).toLowerCase() == SubTabName) {
            GetObject(lblSubTabNameError).innerText = '● Sub Tab Name already exists.';
            return false;
        }
    }
    GetObject(hdnDisplayName).value = trim(GetObject(txtUpdateSubTabName).value);
    return true;
}
/*************************************************************************************************
Function Name     : TM_TabPriorityUp
Author            : Varun Temani
Description       : Changes the Tab priority upside
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function TM_TabPriorityUp(lstTab, hdnTabXml, modalError, lblError) {
    var listTab = document.getElementById(lstTab);
    var idx = listTab.selectedIndex
    if (idx == 0)

        return false

    if (idx == -1) {
        $find(modalError).show();
        GetObject(lblError).innerText = '● Select Tab to ReOrder.';
        //        alert("You must first select the Tab to reorder.");
        return false;
    }
    else {
        var nxidx = idx + (true ? -1 : 1)
        if (nxidx < 0) nxidx = listTab.length - 1
        if (nxidx >= listTab.length) nxidx = 0
        var oldVal = listTab[idx].value
        var oldText = listTab[idx].text
        listTab[idx].value = listTab[nxidx].value
        listTab[idx].text = listTab[nxidx].text
        listTab[nxidx].value = oldVal
        listTab[nxidx].text = oldText
        listTab.selectedIndex = nxidx
    }
    TM_FillPriority(lstTab, hdnTabXml);
    return false;
}
/*************************************************************************************************
Function Name     : TM_TabPriorityDown
Author            : Varun Temani
Description       : Changes the Tab priority down
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function TM_TabPriorityDown(lstTab, hdnTabXml, modalError, lblError) {
    var listTab = document.getElementById(lstTab);
    var idx = listTab.selectedIndex
    var counttab = listTab.length
    if (idx == counttab - 1)
        return false
    if (idx == -1) {
        $find(modalError).show();
        GetObject(lblError).innerText = '● Select Tab to ReOrder.';
        //  alert("● You must first select the Tab to reorder.");
        return false;
    }
    else {
        var nxidx = idx + (false ? -1 : 1)
        if (nxidx < 0) nxidx = listTab.length - 1
        if (nxidx >= listTab.length) nxidx = 0
        var oldVal = listTab[idx].value
        var oldText = listTab[idx].text
        listTab[idx].value = listTab[nxidx].value
        listTab[idx].text = listTab[nxidx].text
        listTab[nxidx].value = oldVal
        listTab[nxidx].text = oldText
        listTab.selectedIndex = nxidx
    }
    TM_FillPriority(lstTab, hdnTabXml);
    return false;
}
/*************************************************************************************************
Function Name     : TM_FillPriority
Author            : Varun Temani
Description       : Fills the Tab priority
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function TM_FillPriority(lstTab, hdnTabXml) {
    var hiddenXml = GetObject(hdnTabXml);
    var xml = (hiddenXml.value.replace(/&lts/g, '<')).replace(/&gts/g, '>');
    var newXml = null;

    //var docElement = new ActiveXObject("Microsoft.XMLDOM");

    //docElement.async = 'false';
    //docElement.loadXML(xml);
    var docElement = window.jQuery(jQuery.parseXML(xml))[0];
    var tabname = GetObject(lstTab);
    var dtTab = docElement.getElementsByTagName("Table");

    var length = tabname.length;
    var rows;
    var tablerow;
    for (rows = 0; rows < length; rows++) {
        var Tabrowid = tabname.options[rows].value;
        for (tablerow = 0; tablerow < dtTab.length; tablerow++) {

            if ($(dtTab[tablerow]).children().eq(2).text() == Tabrowid) {
                $(dtTab[tablerow]).children().eq(4).text(rows + 1);

                break;
            }
        }

    }
    if (docElement.xml == undefined)
        newXml = ((new XMLSerializer()).serializeToString(docElement).replace(/</g, '&lts')).replace(/>/g, '&gts');
    else
        newXml = (docElement.xml.replace(/</g, '&lts')).replace(/>/g, '&gts');
    hiddenXml.value = newXml;
}
/*************************************************************************************************
Function Name     : TM_SubTabPriorityUp
Author            : Varun Temani
Description       : Changes the SubTab priority upside
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function TM_SubTabPriorityUp(lstTab, lstSubTab, hdnXml, modalError, lblError) {

    var listSubTab = document.getElementById(lstSubTab);

    var idx = listSubTab.selectedIndex;
    if (idx == 0)
        return false;
    if (idx == -1) {
        $find(modalError).show();
        GetObject(lblError).innerText = '● Select Sub Tab to ReOrder.';
        //alert("● You must first select the Sub-Tab to reorder.");
        return false;
    }
    else {
        var nxidx = idx + (true ? -1 : 1);
        if (nxidx < 0)
            nxidx = listSubTab.length - 1;
        if (nxidx >= listSubTab.length)
            nxidx = 0;
        var oldVal = listSubTab[idx].value;
        var oldText = listSubTab[idx].text;
        listSubTab[idx].value = listSubTab[nxidx].value;
        listSubTab[idx].text = listSubTab[nxidx].text;
        listSubTab[nxidx].value = oldVal;
        listSubTab[nxidx].text = oldText;
        listSubTab.selectedIndex = nxidx;
    }
    TM_FillSubTabPriority(lstTab, lstSubTab, hdnXml);
    return false;
}
/*************************************************************************************************
Function Name     : TM_SubTabPriorityDown
Author            : Varun Temani
Description       : Changes the SubTab priority down
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function TM_SubTabPriorityDown(lstTab, lstSubTab, hdnSubTab, modalError, lblError) {
    var listTab = document.getElementById(lstTab);
    var listSubTab = document.getElementById(lstSubTab);
    var idx = listSubTab.selectedIndex;
    var countsubtab = listSubTab.length
    if (idx == -1) {
        $find(modalError).show();
        GetObject(lblError).innerText = '● Select Sub Tab to ReOrder.';
        // alert("● You must first select the Sub-Tab to reorder.");
        return false;
    }
    else if (idx == countsubtab - 1)
        return false;
    else {
        var nxidx = idx + (false ? -1 : 1);
        if (nxidx < 0)
            nxidx = listSubTab.length - 1;
        if (nxidx >= listSubTab.length)
            nxidx = 0;
        var oldVal = listSubTab[idx].value;
        var oldText = listSubTab[idx].text;
        listSubTab[idx].value = listSubTab[nxidx].value;
        listSubTab[idx].text = listSubTab[nxidx].text;
        listSubTab[nxidx].value = oldVal;
        listSubTab[nxidx].text = oldText;
        listSubTab.selectedIndex = nxidx;
    }
    TM_FillSubTabPriority(lstTab, lstSubTab, hdnSubTab);
    return false;
}

/*************************************************************************************************
Function Name     : TM_FillSubTabPriority
Author            : Varun Temani
Description       : Fills the SubTab priority
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function TM_FillSubTabPriority(lstTab, lstSubTab, hdnXml) {
    var hiddenXml = GetObject(hdnXml);
    var xml = (hiddenXml.value.replace(/&lts/g, '<')).replace(/&gts/g, '>');
    var newXml = null;

    //    var docElement = new ActiveXObject("Microsoft.XMLDOM");

    //    docElement.async = 'false';
    //    docElement.loadXML(xml);
    var docElement = window.jQuery(jQuery.parseXML(xml))[0];
    var tabname = GetObject(lstTab).value;
    var dt = docElement.getElementsByTagName("Table");
    var lstObj = GetObject(lstSubTab);
    var length = lstObj.length;
    var rows;
    var tablerow;
    for (rows = 0; rows < length; rows++) {
        var rowid = lstObj.options[rows].value;
        for (tablerow = 0; tablerow < dt.length; tablerow++) {
            if ($(dt[tablerow]).children().eq(2).text() == tabname) {
                if ($(dt[tablerow]).children().eq(5).text() == rowid) {
                    $(dt[tablerow]).children().eq(4).text(rows + 1);
                    break;
                }
            }
        }
    }
    if (docElement.xml == undefined)
        newXml = ((new XMLSerializer()).serializeToString(docElement).replace(/</g, '&lts')).replace(/>/g, '&gts');
    else
        newXml = (docElement.xml.replace(/</g, '&lts')).replace(/>/g, '&gts');
    hiddenXml.value = newXml;
}
/*************************************************************************************************
Function Name     : TM_FillSubTabList
Author            : Varun Temani
Description       : Fills the SubTab list according to priority
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function TM_FillSubTabList(lstTabName, lstSubTabName, hdnXml) {

    var tabName = document.getElementById(lstTabName);
    var subTabName = document.getElementById(lstSubTabName);
    var hiddenXml = document.getElementById(hdnXml);

    var selectedTabName = tabName.value;
    var xml = (hiddenXml.value.replace(/&lts/g, '<')).replace(/&gts/g, '>');
    //    var docElement = new ActiveXObject("Microsoft.XMLDOM");

    //    docElement.async = 'false';
    //    docElement.loadXML(xml);
    var docElement = window.jQuery(jQuery.parseXML(xml))[0];
    var Tables = docElement.getElementsByTagName("Table");

    var optionItem;
    var count = 0;
    var optionText = '';
    var optionValue = '';
    // clearing subtabname list
    ClearList(lstSubTabName);
    var subtabsPriority = [];
    var subtabsName = [];
    for (var noofRows = 0; noofRows < Tables.length; noofRows++) {
        if ($(Tables[noofRows]).children().eq(2).text() == selectedTabName) {
            subtabsPriority[parseInt($(Tables[noofRows]).children().eq(4).text() - 1)] = $(Tables[noofRows]).children().eq(5).text();

            subtabsName[parseInt($(Tables[noofRows]).children().eq(4).text() - 1)] = $(Tables[noofRows]).children().eq(1).text();
        }
    }
    for (var i = 0; i < subtabsPriority.length; i++) {
        optionText = subtabsName[i];
        optionValue = subtabsPriority[i];
        optionItem = new Option(optionText, optionValue);
        subTabName.options.add(optionItem);
    }
    return false;
}

/*************************************************************************************************
Function Name     : TM_CheckTabSubTab
Author            : Varun Temani
Description       : Checks if a tab is added or not and whether the added tab has a subtab
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function TM_CheckTabSubTab(modalError, hdnParsedTab, hdnParsedSubTab, lblError1) {

    var hdnTabXml = document.getElementById(hdnParsedTab);
    var hdnSubTabXml = document.getElementById(hdnParsedSubTab);

    var TabXml = (hdnTabXml.value.replace(/&lts/g, '<')).replace(/&gts/g, '>');
    var SubTabXml = (hdnSubTabXml.value.replace(/&lts/g, '<')).replace(/&gts/g, '>');

    //    var docElementTab = new ActiveXObject("Microsoft.XMLDOM");
    //    docElementTab.async = 'false';
    //    docElementTab.loadXML(TabXml);
    var docElementTab = window.jQuery(jQuery.parseXML(TabXml))[0];
    var TablesTab = docElementTab.getElementsByTagName("Table");

    //    var docElementSubTab = new ActiveXObject("Microsoft.XMLDOM");
    //    docElementSubTab.async = 'false';
    //    docElementSubTab.loadXML(SubTabXml);
    var docElementSubTab = window.jQuery(jQuery.parseXML(SubTabXml))[0];
    var TablesSubTab = docElementSubTab.getElementsByTagName("Table");

    tablength = TablesTab.length;

    if (tablength < 4) {
        GetObject(lblError1).innerText = '● Please Add User Defined Tab';
        $find(modalError).show();
        return false;
    }
    subtablength = TablesSubTab.length;

    for (var tabrow = 0; tabrow < tablength; tabrow++) {
        var tabrowid = $(TablesTab[tabrow]).children().eq(2).text();
        var count = 0;
        for (var subtabrow = 0; subtabrow < subtablength; subtabrow++) {
            if ($(TablesSubTab[subtabrow]).children().eq(2).text() == tabrowid)
                count = 1;
        }
        if (count == 0) {
            GetObject(lblError1).innerText = '● No SubTab exists for Tab : ' + $(TablesTab[tabrow]).children().eq(0).text();
            $find(modalError).show();
            return false;
        }
    }
}
/*************************************************************************************************
Function Name     : AddLegRowAtrib 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function AddLegRowAtrib(gridDetailAttributes,
                     txtDisplayName,
                     txtDescription,
                     colNames,
                     hdnLegXML,
                     hdnUpdateAtribRowId,
                     txtDisplayNameUpdate,
                      txtDescriptionUpdate,
                     modalUpdateAtrib,
                     modalAddAtrib,
                     lblDisplayNameError,
                     rblDataTypeAdd,
                     hdnDisplayName,
                     rblDataType,
                     lblDisplayNameUpdateError,
                     hdnAttributeList,
                     lstMaster,
                     lstCommon,
                     lstLeg,
                     trDetailGrid,
                     txtStringLength,
                     txtNumericIntLength,
                     txtNumericDecimalLength,
                     txtUpdateStringAttribute,
                     txtUpdateNumericIntAttribute,
                     txtUpdateNumericDecimalAttribute,
                     lblUpdateSizeAttribute,
                     tdSizeUpdate,
                     chkBoxCloneableUpdate,
                     chkBoxCloneable,
                     divTagUpdate,
                     divTag) {
    var iButton;
    var iButtonNew;
    var strdummyRow = new Array();
    var count;
    var gridObj = GetObject(gridDetailAttributes);
    var lstMasterObj = GetObject(lstMaster);
    var lstCommonObj = GetObject(lstCommon);
    var lstLegObj = GetObject(lstLeg);
    var lstCount;
    var objDispName = GetObject(txtDisplayName);
    var objDescription = GetObject(txtDescription);
    var dispName = trim(objDispName.value.toLowerCase());
    var sizeofAttribute;
    var length;

    delColindex = 11;
    //Clear Error labels on Modal PopUp
    GetObject(lblDisplayNameError).innerText = '';

    // Check if the entered values are blank or Already existing: then raise error.
    if (trim(objDispName.value) == '') {
        GetObject(lblDisplayNameError).innerText = '● Please Enter Attribute Name';
        objDispName.focus();
        return false;
    }

    // Check If added Attribute is in Common Attributes.
    lstCount = lstCommonObj.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(lstCommonObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name is in Common Attributes';
            objDispName.focus();
            return false;
        }
    }

    // Check If added Attribute is in Details Attributes.
    lstCount = lstMasterObj.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(lstMasterObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name Exists in Master Section';
            objDispName.focus();
            return false;
        }
    }

    // Check If added Attribute is already Added.
    length = gridObj.rows.length;
    for (count = 2; count < length; count++) {
        if (trim(gridObj.rows[count].cells[0].innerText.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● Attribute Name Already Exists';
            objDispName.focus();
            return false;
        }
    }

    lstCount = lstLegObj.length;
    // Check If added Attribute is in Details Attributes.
    for (count = 0; count < lstCount; count++) {
        if (trim(lstLegObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name Exists in Leg';
            objDispName.focus();
            return false;
        }
    }

    // Check If added Attribute is among List of prohibited Attributes.
    var commonCols = GetObject(hdnAttributeList).value.split('|');
    length = commonCols.length;
    for (count = 0; count < length; count++) {
        if (trim(commonCols[count].toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name is not Allowed';
            objDispName.focus();
            return false;
        }
    }

    var delColindex = 11;
    var sectype_table_id = trim(gridObj.rows[gridObj.rows.length - 1].cells[11].innerText);

    // Generate Unique Row ID for New Row
    var deleteRowIndex = trim(gridObj.rows[gridObj.rows.length - 1].cells[delColindex].innerText);
    deleteRowIndex = parseInt(deleteRowIndex) + 1;
    gridObj.insertRow(gridObj.rows.length);
    var i = gridObj.rows.length - 1;
    var gridRowObj = gridObj.rows[i];
    var rblObj = GetObject(rblDataTypeAdd);
    var dataType;
    //    length = rblObj.cells.length;
    //    for (count = 0; count < length; count++) {
    //        if (rblObj.cells[count].children[0].checked) {
    //            dataType = rblObj.cells[count].children[0].value;
    //            rblObj.cells[0].children[0].checked = true;
    //            break;
    //        }
    //    }
    var rblCells = $('#' + rblDataTypeAdd).find('td');
    for (count = 0; count < rblCells.length; count++) {
        if (rblCells.eq(count).children().eq(0).prop('checked').toString() == "true") {
            dataType = rblCells.eq(count)[0].children[0].value;
            rblCells.eq(count)[0].children[0].checked = true;
            break;
        }
    }

    if (dataType.toLowerCase() == 'string') {
        if (trim(GetObject(txtStringLength).value) == '') {
            GetObject(lblDisplayNameError).innerText = '● Please Enter attribute size';
            return false;
        }
        else {
            var size = Number(trim(GetObject(txtStringLength).value))
            var result = size * 2;
            if (result == 0) {
                GetObject(lblDisplayNameError).innerText = '● Length Of Attribute Should Be Greater Than Zero';
                return false;

            }
            sizeofAttribute = trim(GetObject(txtStringLength).value);
        }
    }
    else if (dataType.toLowerCase() == 'numeric') {
        if (trim(GetObject(txtNumericIntLength).value) == '' || trim(GetObject(txtNumericDecimalLength).value) == '') {
            GetObject(lblDisplayNameError).innerText = '● Please Enter attribute size';
            return false;
        }
        else {
            var numericIntLength = GetObject(txtNumericIntLength).value;
            var numericDecimalLength = GetObject(txtNumericDecimalLength).value;
            if ((parseInt(numericIntLength) + parseInt(numericDecimalLength)) > 28) {
                GetObject(lblDisplayNameError).innerText = '● Precision Can Have A Maximum Value Of 28 (Precision is no. of digits before decimal + no. of digits after decimal) ';
                return false;
            }
            else
                sizeofAttribute = trim(GetObject(txtNumericIntLength).value) + '.' + trim(GetObject(txtNumericDecimalLength).value);
        }
    }
    else
        sizeofAttribute = '';


    strdummyRow.push('0'); // IsPermanent
    strdummyRow.push('0'); // Attribute_ID
    strdummyRow.push('_'); // Attribute_Name

    // Insert Display name
    gridRowObj.insertCell(0);
    gridRowObj.cells[0].innerText = trim(objDispName.value);
    strdummyRow.push(trim(objDispName.value));

    //insert description
    gridRowObj.insertCell(1);
    gridRowObj.cells[1].innerHTML = '<div class="overflowingDescription">' + trim(objDescription.value) + '</div>';
    //--->gridRowObj.cells[1].style.display = 'none';
    strdummyRow.push(trim(objDescription.value));

    // Insert Data Type
    gridRowObj.insertCell(2);
    gridRowObj.cells[2].innerText = dataType;
    strdummyRow.push(dataType);


    strdummyRow.push(deleteRowIndex);
    strdummyRow.push(sectype_table_id);

    gridRowObj.insertCell(3);
    gridRowObj.cells[3].innerText = '';
    strdummyRow.push('');

    gridRowObj.insertCell(4);
    gridRowObj.cells[4].innerText = '';
    strdummyRow.push('');
    gridRowObj.cells[4].style.display = 'none';

    gridRowObj.insertCell(5);
    gridRowObj.cells[5].innerText = '';
    strdummyRow.push('');

    gridRowObj.insertCell(6);
    gridRowObj.cells[6].innerText = '';
    strdummyRow.push('');

    gridRowObj.insertCell(7);
    gridRowObj.cells[7].innerText = '';
    strdummyRow.push('');
    gridRowObj.cells[7].style.display = 'none';

    var iButton;
    var iButtonNew;

    gridRowObj.insertCell(8);
    gridRowObj.cells[8].innerText = sizeofAttribute;
    //iButton = gridObj.rows[i - 1].children[6];
    //    iButtonNew = iButton.cloneNode(true);
    //    iButtonNew.innerText = sizeofAttribute;
    //   
    //    gridRowObj.insertCell(6).appendChild(iButtonNew);


    // Insert Buttons: Update
    var isCloneable = $('#' + chkBoxCloneable).is(":checked").toString();
    var tags = $('#' + divTag).find('.hdnTag').val();

    iButton = GetObject(gridObj.rows[1].children[9].children[0].id);
    iButtonNew = iButton.cloneNode(true);
    iButtonNew.disabled = false;
    iButtonNew.onclick = function () {
        CallUpdateLegAtribPopUp(modalUpdateAtrib, txtDisplayNameUpdate, txtDescriptionUpdate, txtUpdateStringAttribute, txtUpdateNumericIntAttribute, txtUpdateNumericDecimalAttribute, lblUpdateSizeAttribute, tdSizeUpdate, hdnUpdateAtribRowId, deleteRowIndex,
                                    rblDataType, hdnDisplayName, gridDetailAttributes, lblDisplayNameUpdateError, chkBoxCloneableUpdate, isCloneable, divTagUpdate, tags, delColindex); return false;
    };
    gridRowObj.insertCell(9).appendChild(iButtonNew);
    gridRowObj.cells[9].align = 'center';
    //   gridRowObj.cells[6].style.display = 'none';

    // Insert Buttons: Delete

    iButton = GetObject(gridObj.rows[1].children[10].children[0].id);
    iButtonNew = iButton.cloneNode(true);
    iButtonNew.disabled = false;
    iButtonNew.onclick = function () { DeleteRowInAtribTable(gridDetailAttributes, deleteRowIndex, hdnLegXML, delColindex, trDetailGrid); return false; };

    gridRowObj.insertCell(10).appendChild(iButtonNew);
    gridRowObj.cells[10].align = 'center';

    //adding new column for RowID
    gridRowObj.insertCell(delColindex);
    gridRowObj.children[delColindex].innerText = deleteRowIndex;

    gridRowObj.cells[delColindex].style.display = 'none';

    //adding new column for sectype_table_id
    gridRowObj.insertCell(12);
    gridRowObj.children[12].innerText = sectype_table_id;
    gridRowObj.cells[12].style.display = 'none';

    var objchkBoxCloneable = $(GetObject(chkBoxCloneable));

    gridRowObj.insertCell(13);
    gridRowObj.children[13].innerText = isCloneable;
    gridRowObj.cells[13].style.display = 'none';

    gridRowObj.insertCell(14);

    gridRowObj.children[14].innerText = tags;
    gridRowObj.cells[14].style.display = 'none';

    // False for IsPermanent flag
    gridRowObj.className = 'normalRow';
    strdummyRow.push(false);
    strdummyRow.push(sizeofAttribute);

    if ($(objchkBoxCloneable).is(":checked"))
        strdummyRow.push('true');

    else
        strdummyRow.push('false');

    strdummyRow.push(tags);

    //Save new attrib values
    saveRefDisplayAttributedetails(txtDisplayName, null, 'chkBoxMandatory', 'ADD');
    $('#hdnAttributeName').val('');

    //************code to add new row in XML*********************
    UpdateXML(colNames, hdnLegXML, strdummyRow);
    $find(modalAddAtrib).hide();
    GetObject(trDetailGrid).style.display = '';
    return false;
}


/*************************************************************************************************
Function Name     : CheckIfExistsAttributeName 
Author            : Bhavya Jaitly
Modified By       : 
Description       :
Page used in      : 
Parameters        : 
                        
Modification Log  : 
*************************************************************************************************/
function CheckIfExistsAttributeName(txtAttributeName,
                                    txtDescription,
                                    label,
                                    txtStringSizeUpdateLegExisting,
                                    txtNumericIntSizeUpdateLegExisting,
                                    txtNumericDecimalSizeUpdateLegExisting,
                                    tdSizeUpdateLegExisting,
                                    hdnAttributeSize,
                                    rbl,
                                    divTagUpdate2) {
    var dataType;
    var rblObj = GetObject(rbl);

    GetObject(txtAttributeName).value = trim(GetObject(txtAttributeName).value).replaceNBR();
    if (trim(GetObject(txtAttributeName).value) == '') {
        GetObject(label).innerText = '● Please Enter Attribute Name';
        return false;
    }

    //    for (count = 0; count < rblObj.cells.length; count++) {
    //        if (rblObj.cells[count].children[0].children[0].checked) {
    //            dataType = rblObj.cells[count].children[0].children[0].value;
    //            break;
    //        }
    //    }
    var rblCells = $('#' + rbl).find('td');
    for (count = 0; count < rblCells.length; count++) {
        if (rblCells.eq(count).children().eq(0).children().eq(0).prop('checked').toString() == "true") {
            dataType = rblCells.eq(count)[0].children[0].children[0].value;
            break;
        }
    }

    if (dataType.toLowerCase() == 'string') {
        if (GetObject(txtStringSizeUpdateLegExisting).value.toLowerCase() == '') {
            GetObject(label).innerText = '● Please Enter Attribute Size';
            return false;
        }
        if (parseInt(GetObject(txtStringSizeUpdateLegExisting).value.toLowerCase()) < parseInt(GetObject(hdnAttributeSize).value)) {
            GetObject(label).innerText = '● Attribute Size Should be greater than or equal to ' + GetObject(hdnAttributeSize).value;
            return false;
        }
        if (parseInt(GetObject(txtStringSizeUpdateLegExisting).value.toLowerCase()) > 8000) {
            GetObject(label).innerText = '● Attribute Size Should be less than or equal to ' + '8000';
            return false;
        }


    }
    else if (dataType.toLowerCase() == 'numeric') {
        if (trim(GetObject(txtNumericIntSizeUpdateLegExisting).value) == '' || trim(GetObject(txtNumericDecimalSizeUpdateLegExisting).value) == '') {
            GetObject(label).innerText = '● Please Enter Attribute Size';
            return false;
        }
        if (parseInt(GetObject(txtNumericIntSizeUpdateLegExisting).value.toLowerCase()) < parseInt(GetObject(hdnAttributeSize).value.split('.')[0])) {
            GetObject(label).innerText = '● Length of string before decimal should be greater than or equal to  ' + parseInt(GetObject(hdnAttributeSize).value.split('.')[0]);
            return false;
        }
        if (parseInt(GetObject(txtNumericDecimalSizeUpdateLegExisting).value.toLowerCase()) < parseInt(GetObject(hdnAttributeSize).value.split('.')[1])) {
            GetObject(label).innerText = '● Length of string after decimal should be greater than or equal to   ' + parseInt(GetObject(hdnAttributeSize).value.split('.')[1]);
            return false;
        }
        if (Number(trim(GetObject(txtNumericIntSizeUpdateLegExisting).value)) + Number(trim(GetObject(txtNumericDecimalSizeUpdateLegExisting).value)) > 28) {
            GetObject(label).innerText = '● Precision Can Have A Maximum Value Of 28 (Precision is no. of digits before decimal + no. of digits after decimal) ';
            return false;
        }

    }

    $('#hdnTagInfo').val($('#' + divTagUpdate2).find('.hdnTag').val());
    //Save new attrib values
    saveRefDisplayAttributedetails(txtAttributeName, null, 'chkBoxMandatoryUpdate2', 'ADD');
}

/*************************************************************************************************
Function Name     : AddLegRowRefAtrib 
Author            : Bhavya Jaitly
Modified By       : 
Description       :
Page used in      : 
Parameters        : 
                        
Modification Log  : 
*************************************************************************************************/
function AddLegRowRefAtrib(gridDetailAttributes,
                     txtDisplayName,
                     txtDescription,
                     colNames,
                     hdnLegXML,
                     hdnUpdateAtribRowId,
                     txtDisplayNameUpdate,
                      txtDescriptionUpdate,
                     modalUpdateAtrib,
                     modalAddAtrib,
                     lblDisplayNameError,
                     ddlEntityType,
                     ddlEntityAttribute,
                     ddlDetailEntityAttributeId,
                     hdnDisplayName,
                     ddlEntityTypeUpdate,
                     ddlEntityAttributeUpdate,
                     ddlDetailEntityAttributeUpdateId,
                     lblDisplayNameUpdateError,
                     hdnAttributeList,
                     lstMaster,
                     lstCommon,
                     lstLeg,
                     trDetailGrid,
                     chkBoxRCloneableUpdate,
                     chkBoxRCloneable,
                     divRTagUpdate,
                     divRTag) {
    var iButton;
    var iButtonNew;
    var strdummyRow = new Array();
    var count;
    var gridObj = GetObject(gridDetailAttributes);
    var lstMasterObj = GetObject(lstMaster);
    var lstCommonObj = GetObject(lstCommon);
    var lstLegObj = GetObject(lstLeg);
    var lstCount;
    var objDispName = GetObject(txtDisplayName);
    var objDescription = GetObject(txtDescription);
    var dispName = trim(objDispName.value.toLowerCase());
    var length;
    var ddlDetailEntityAttribute = $get(ddlDetailEntityAttributeId);
    //Clear Error labels on Modal PopUp
    GetObject(lblDisplayNameError).innerText = '';

    // Check if the entered values are blank or Already existing: then raise error.
    if (trim(objDispName.value) == '') {
        GetObject(lblDisplayNameError).innerText = '● Please Enter Attribute Name';
        objDispName.focus();
        return false;
    }

    if (document.getElementById(ddlEntityType).options[document.getElementById(ddlEntityType).selectedIndex].value == "-1") {
        GetObject(lblDisplayNameError).innerText = '● Please select entity type ';
        return false;
    }
    if (document.getElementById(ddlEntityAttribute).options[document.getElementById(ddlEntityAttribute).selectedIndex].value == "-1") {
        GetObject(lblDisplayNameError).innerText = '● Please select reference attribute';

        return false;
    }


    // Check If added Attribute is in Common Attributes.
    lstCount = lstCommonObj.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(lstCommonObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name is in Common Attributes';
            objDispName.focus();
            return false;
        }
    }

    // Check If added Attribute is in Details Attributes.
    lstCount = lstMasterObj.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(lstMasterObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name Exists in Master Section';
            objDispName.focus();
            return false;
        }
    }

    // Check If added Attribute is already Added.
    length = gridObj.rows.length;
    for (count = 2; count < length; count++) {
        if (trim(gridObj.rows[count].cells[0].innerText.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● Attribute Name Already Exists';
            objDispName.focus();
            return false;
        }
    }

    lstCount = lstLegObj.length;
    // Check If added Attribute is in Details Attributes.
    for (count = 0; count < lstCount; count++) {
        if (trim(lstLegObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name Exists in Leg';
            objDispName.focus();
            return false;
        }
    }

    // Check If added Attribute is among List of prohibited Attributes.
    var commonCols = GetObject(hdnAttributeList).value.split('|');
    length = commonCols.length;
    for (count = 0; count < length; count++) {
        if (trim(commonCols[count].toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name is not Allowed';
            objDispName.focus();
            return false;
        }
    }

    var deleteColindex = 11;
    //  var sectype_table_id = trim(gridObj.rows[gridObj.rows.length - 1].cells[12].innerText);
    var sectype_table_id = $(gridObj.rows[gridObj.rows.length - 1]).find('[id$=lblsectype_table_id]').text().trim();
    // Generate Unique Row ID for New Row
    var deleteRowIndex = trim(gridObj.rows[gridObj.rows.length - 1].cells[deleteColindex].innerText);
    deleteRowIndex = parseInt(deleteRowIndex) + 1;
    gridObj.insertRow(gridObj.rows.length);
    var i = gridObj.rows.length - 1;
    var gridRowObj = gridObj.rows[i];

    strdummyRow.push('0'); // IsPermanent
    strdummyRow.push('0'); // Attribute_ID
    strdummyRow.push('_'); // Attribute_Name

    // Insert Display name
    gridRowObj.insertCell(0);
    gridRowObj.cells[0].innerText = trim(objDispName.value);
    strdummyRow.push(trim(objDispName.value));

    //Attribute Description
    gridRowObj.insertCell(1);
    gridRowObj.cells[1].innerHTML = '<div class="overflowingDescription">' + trim(objDescription.value) + '</div>';
    //--->gridRowObj.cells[1].style.display = "none";
    strdummyRow.push(trim(objDescription.value));

    // Insert Data Type
    gridRowObj.insertCell(2);
    gridRowObj.cells[2].innerText = 'STRING';
    //    gridRowObj.cells[1].innerText =document.getElementById(hdnReferenceType).value;
    strdummyRow.push('STRING');

    strdummyRow.push(deleteRowIndex);
    strdummyRow.push(sectype_table_id);

    var isCurveReferenceAttrib = false;

    if (ddlDetailEntityAttribute.disabled == false && ddlDetailEntityAttribute.value != null && ddlDetailEntityAttribute.value != "-1" && $get(ddlEntityAttribute).value != "")
        isCurveReferenceAttrib = true;
    gridRowObj.insertCell(3);
    gridRowObj.insertCell(4);
    gridRowObj.insertCell(5);
    gridRowObj.insertCell(6);
    gridRowObj.insertCell(7);

    if (!isCurveReferenceAttrib) {
        gridRowObj.cells[3].innerText = trim(document.getElementById(ddlEntityType).options[GetObject(ddlEntityType).selectedIndex].text);
        strdummyRow.push(trim(document.getElementById(ddlEntityType).options[GetObject(ddlEntityType).selectedIndex].text));

        gridRowObj.cells[4].innerText = trim(document.getElementById(ddlEntityType).value);
        strdummyRow.push(document.getElementById(ddlEntityType).value);
        gridRowObj.cells[4].style.display = 'none';

        var len = document.getElementById(ddlEntityAttribute).length;
        var selectedEntityType;
        for (var x = 0; x < len; x++) {
            if (document.getElementById(ddlEntityAttribute)[x].selected == true)
                selectedEntityType = document.getElementById(ddlEntityAttribute)[x].text;
        }
        gridRowObj.cells[5].innerText = selectedEntityType;
        strdummyRow.push(selectedEntityType);

        gridRowObj.cells[6].innerText = "";
        strdummyRow.push("");

        gridRowObj.cells[7].innerText = trim(document.getElementById(ddlEntityAttribute).value);
        strdummyRow.push(document.getElementById(ddlEntityAttribute).value);
        gridRowObj.cells[7].style.display = 'none';
    }
    else {
        //ddlEntityType,
        //ddlEntityAttribute,
        //ddlDetailEntityAttributeId,
        var detailEntityTypeId = trim(ddlDetailEntityAttribute.value).split('-')[0];
        var detailEntityAttributeId = trim(ddlDetailEntityAttribute.value).split('-')[1];
        var detailEntitytAttributeName = trim(ddlDetailEntityAttribute.options[ddlDetailEntityAttribute.selectedIndex].text).replace('-', ':');
        var masterEntityAttributeId = trim($get(ddlEntityAttribute).value);
        var masterEntityAttributeName = trim($get(ddlEntityAttribute).options[$get(ddlEntityAttribute).selectedIndex].text);
        var masterEntityTypeName = trim($get(ddlEntityType).options[$get(ddlEntityType).selectedIndex].text);

        gridRowObj.cells[3].innerText = masterEntityTypeName
        strdummyRow.push(masterEntityTypeName);

        gridRowObj.cells[4].innerText = detailEntityTypeId;
        strdummyRow.push(detailEntityTypeId);
        gridRowObj.cells[4].style.display = 'none';

        gridRowObj.cells[5].innerText = masterEntityAttributeName;
        strdummyRow.push(masterEntityAttributeName);

        gridRowObj.cells[6].innerText = detailEntitytAttributeName;
        strdummyRow.push(detailEntitytAttributeName);

        gridRowObj.cells[7].innerText = detailEntityAttributeId;
        strdummyRow.push(detailEntityAttributeId);
        gridRowObj.cells[7].style.display = 'none';
    }



    // Insert Buttons: Update
    var objchkBoxCloneable = $(GetObject(chkBoxRCloneable));
    var divRTag = $(GetObject(divRTag));

    var iButton;
    var iButtonNew;
    gridRowObj.insertCell(8);
    gridRowObj.cells[8].innerText = '';
    //iButton = gridObj.rows[i - 1].children[6];
    //    iButtonNew = iButton.cloneNode(true);
    //    iButtonNew.innerText = "";
    //   
    //    gridRowObj.insertCell(6).appendChild(iButtonNew);

    iButton = GetObject(gridObj.rows[1].children[9].children[0].id);


    iButtonNew = iButton.cloneNode(true);
    iButtonNew.disabled = false;

    iButtonNew.onclick = function () {
        CallUpdateRefLegAtribPopUp(modalUpdateAtrib, txtDisplayNameUpdate, txtDescriptionUpdate, hdnUpdateAtribRowId, deleteRowIndex,
                                    ddlEntityTypeUpdate, ddlEntityAttributeUpdate, ddlDetailEntityAttributeUpdateId,
                                    hdnDisplayName, gridDetailAttributes, lblDisplayNameUpdateError, chkBoxRCloneableUpdate, $(objchkBoxCloneable).is(":checked").toString(), divRTagUpdate, divRTag.find('.hdnTag').val(), deleteColindex); return false;
    };
    gridRowObj.insertCell(9).appendChild(iButtonNew);
    gridRowObj.cells[9].align = 'center';
    //   gridRowObj.cells[6].style.display = 'none';

    // Insert Buttons: Delete
    var iButton;
    var iButtonNew;
    iButton = GetObject(gridObj.rows[1].children[10].children[0].id);
    iButtonNew = iButton.cloneNode(true);
    iButtonNew.disabled = false;
    iButtonNew.onclick = function () { DeleteRowInAtribTable(gridDetailAttributes, deleteRowIndex, hdnLegXML, deleteColindex, trDetailGrid); return false; };

    gridRowObj.insertCell(10).appendChild(iButtonNew);
    gridRowObj.cells[10].align = 'center';

    //adding new column for RowID
    gridRowObj.insertCell(deleteColindex);
    gridRowObj.children[deleteColindex].innerText = deleteRowIndex;

    gridRowObj.cells[deleteColindex].style.display = 'none';
    //adding new column for sectype_table_id
    gridRowObj.insertCell(12);
    gridRowObj.children[12].innerText = sectype_table_id;
    gridRowObj.cells[12].style.display = 'none';


    gridRowObj.insertCell(13);
    gridRowObj.children[13].innerText = $(objchkBoxCloneable).is(":checked").toString();
    gridRowObj.cells[13].style.display = 'none';

    gridRowObj.insertCell(14);
    gridRowObj.children[14].innerText = divRTag.find('.hdnTag').val();
    gridRowObj.cells[14].style.display = 'none';

    // False for IsPermanent flag
    gridRowObj.className = 'normalRow';
    strdummyRow.push(true);
    strdummyRow.push('');

    if ($(objchkBoxCloneable).is(":checked"))
        strdummyRow.push('true');

    else
        strdummyRow.push('false');

    strdummyRow.push(divRTag.find('.hdnTag').val());

    //Add values for new ref attr
    saveRefDisplayAttributedetails(txtDisplayName, 'smtrRefDisplayName_dropdown', 'chkBoxRMandatory', 'ADD');
    $('#hdnAttributeName').val('');

    //************code to add new row in XML*********************
    UpdateXML(colNames, hdnLegXML, strdummyRow);
    $find(modalAddAtrib).hide();
    GetObject(trDetailGrid).style.display = '';
    //$('#hdnAttributeName').val('');
    return false;
}






/*************************************************************************************************
Function Name     : UpdateLegAtribRow 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function UpdateLegAtribRow(grid,
                        hdnUpdateAtribRowId,
                        txtDisplayNameUpdate,
                        txtDescriptionUpdate,
                        hdnExpressionXMLId,
                        lblDisplayNameError,
                        modalUpdateAtrib,
                        rblDataType,
                        hdnDisplayName,
                        hdnAttributeList,
                        lstMaster,
                        lstCommon,
                        lstLeg,
                        txtUpdateStringAttribute,
                        txtUpdateNumericIntAttribute,
                        txtUpdateNumericDecimalAttribute,
                        chkBoxCloneable,
                        divTag) {
    var gridObj = GetObject(grid);
    var lstMasterObj = GetObject(lstMaster);
    var lstCommonObj = GetObject(lstCommon);
    var lstLegObj = GetObject(lstLeg);
    var lstCount;
    var count;
    var updatedAttributeSize;
    GetObject(lblDisplayNameError).innerText = '';
    var objDispName = GetObject(txtDisplayNameUpdate);
    var objDescription = GetObject(txtDescriptionUpdate);
    var updatedDispName = trim(objDispName.value);
    var dispName = updatedDispName.toLowerCase();
    // Check If Attribute name is Empty.
    if (updatedDispName == '') {
        GetObject(lblDisplayNameError).innerText = '● Enter Attribute Name';
        objDispName.focus();
        return false;
    }

    // Check If added Attribute is in Common Attributes.
    lstCount = lstCommonObj.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(lstCommonObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name is in Common Attributes';
            objDispName.focus();
            return false;
        }
    }

    // Check If added Attribute is in Details Attributes.
    lstCount = lstMasterObj.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(lstMasterObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name Exists in Master Section';
            objDispName.focus();
            return false;
        }
    }

    lstCount = lstLegObj.length;
    // Check If added Attribute is in Details Attributes.
    for (count = 0; count < lstCount; count++) {
        if (trim(lstLegObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name Exists in Leg';
            objDispName.focus();
            return false;
        }
    }
    // Check If added Attribute is already Added
    lstCount = gridObj.rows.length;
    for (count = 2; count < lstCount; count++) {
        if (dispName == trim(gridObj.rows[count].cells[0].innerText.toLowerCase())
            && GetObject(hdnDisplayName).value.toLowerCase() != updatedDispName.toLowerCase()) {
            GetObject(lblDisplayNameError).innerText = '● Attribute Name Already Exist';
            objDispName.focus();
            return false;
        }
    }

    // Check If added Attribute is among List of prohibited Attributes.
    var commonCols = GetObject(hdnAttributeList).value.split('|');
    lstCount = commonCols.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(commonCols[count].toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name is not Allowed';
            objDispName.focus();
            return false;
        }
    }

    var rblObj = GetObject(rblDataType);
    var dataType;
    //    for (count = 0; count < rblObj.cells.length; count++)
    //        if (rblObj.cells[count].children[0].checked) {
    //            dataType = rblObj.cells[count].children[0].value;
    //            break;
    //        }
    var rblCells = $('#' + rblDataType).find('td');
    for (count = 0; count < rblCells.length; count++) {
        if (rblCells.eq(count).children().eq(0).prop('checked').toString() == "true") {
            dataType = rblCells.eq(count)[0].children[0].value;
            break;
        }
    }

    var deleteRowID = GetObject(hdnUpdateAtribRowId).value;
    var expressionsXML = '';
    var strExpressionRow = GetObject(hdnExpressionXMLId).value;
    strExpressionRow = strExpressionRow.replace(/&lts/g, '<');
    strExpressionRow = strExpressionRow.replace(/&gts/g, '>');
    //detecting selected row from table (by default this HTML table will have 2 rows)
    var rowIndex = 0;
    for (rowIndex = 1; rowIndex < gridObj.rows.length; rowIndex++) {
        if (parseInt(trim(gridObj.rows[rowIndex].cells[11].innerText)) == parseInt(deleteRowID))
            break;
    }

    if (dataType.toLowerCase() == 'string') {
        if (trim(GetObject(txtUpdateStringAttribute).value) == '') {
            GetObject(lblDisplayNameError).innerText = '● Please enter attribute size';
            return false;
        }
        else {
            gridObj.rows[rowIndex].cells[8].innerText = trim(GetObject(txtUpdateStringAttribute).value);
            gridObj.rows[rowIndex].cells[8].align = 'left';
        }


    }
    else if (dataType.toLowerCase() == 'numeric') {

        if (trim(GetObject(txtUpdateNumericIntAttribute).value) == '' || trim(GetObject(txtUpdateNumericDecimalAttribute).value) == '') {
            GetObject(lblDisplayNameError).innerText = '● Please enter attribute size';
            return false;
        }
        else {
            var numericIntAttributeSize = GetObject(txtUpdateNumericIntAttribute).value;
            var numericDecimalAttributeSize = GetObject(txtUpdateNumericDecimalAttribute).value;
            if ((parseInt(numericIntAttributeSize) + parseInt(numericDecimalAttributeSize)) > 28) {
                GetObject(lblDisplayNameError).innerText = '● Precision Can Have A Maximum Value Of 28 (Precision is no. of digits before decimal + no. of digits after decimal) ';
                return false;
            }
            else {
                gridObj.rows[rowIndex].cells[8].innerText = trim(GetObject(txtUpdateNumericIntAttribute).value) + '.' + trim(GetObject(txtUpdateNumericDecimalAttribute).value);
                gridObj.rows[rowIndex].cells[8].align = 'left';
            }
        }
    }
    else {
        gridObj.rows[rowIndex].cells[8].innerText = '';
        gridObj.rows[rowIndex].cells[8].align = 'left';
    }
    updatedAttributeSize = trim(gridObj.rows[rowIndex].cells[8].innerText);
    //  rowIndex=rowIndex-1;
    gridObj.rows[rowIndex].cells[0].innerText = updatedDispName;
    gridObj.rows[rowIndex].cells[0].align = 'left';

    gridObj.rows[rowIndex].cells[1].innerHTML = '<div class="overflowingDescription">' + trim(objDescription.value) + '</div>';
    gridObj.rows[rowIndex].cells[1].align = 'left';


    gridObj.rows[rowIndex].cells[2].innerText = dataType;
    gridObj.rows[rowIndex].cells[2].align = 'left';

    var isClone = $('#' + chkBoxCloneable).is(':checked').toString();
    gridObj.rows[rowIndex].cells[13].innerText = isClone;

    var tags = $('#' + divTag).find('.hdnTag').val();
    gridObj.rows[rowIndex].cells[14].innerText = tags;

    //deleting row from XML
    var tableArray = strExpressionRow.split('<Table>');
    var finalxml = '';

    for (var i = 0; i < tableArray.length; i++) {
        finalxml += '<Table>';
        if (i != rowIndex)
            finalxml += tableArray[i];
        else {

            var startindex = tableArray[i].indexOf('<Display_Name>') + 14;
            var Endindex = tableArray[i].indexOf('</Display_Name>');
            var firsthalf = tableArray[i].substring(0, startindex);
            var Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + updatedDispName + Secondhalf;

            var descc = trim(objDescription.value);
            descc = descc.replaceAll('<', 'ž').replaceAll('>', 'Ÿ');

            startindex = tableArray[i].indexOf('<Attribute_x0020_Description>') + 29;
            Endindex = tableArray[i].indexOf('</Attribute_x0020_Description>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + descc + Secondhalf;

            startindex = tableArray[i].indexOf('<Data_Type>') + 11;
            Endindex = tableArray[i].indexOf('</Data_Type>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + dataType + Secondhalf;

            startindex = tableArray[i].indexOf('<attribute_size>') + 16;
            Endindex = tableArray[i].indexOf('</attribute_size>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + updatedAttributeSize + Secondhalf;


            startindex = tableArray[i].indexOf('<is_cloneable>') + 14;
            Endindex = tableArray[i].indexOf('</is_cloneable>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + isClone + Secondhalf;

            startindex = tableArray[i].indexOf('<tags>') + 6;
            Endindex = tableArray[i].indexOf('</tags>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + tags + Secondhalf;

            finalxml += tableArray[i];
        }
    }
    if (finalxml.indexOf('<Table>') == 0) { finalxml = finalxml.substring(7); }
    if (finalxml.lastIndexOf('</NewDataSet>') == -1) { finalxml += '</NewDataSet>'; }

    finalxml = finalxml.replace(/</g, '&lts');
    finalxml = finalxml.replace(/>/g, '&gts');
    GetObject(hdnExpressionXMLId).value = finalxml;

    saveRefDisplayAttributedetails(txtDisplayNameUpdate, null, 'chkBoxMandatoryUpdate', 'UPDATE');
    $find(modalUpdateAtrib).hide();
    //$('#hdnAttributeName').val('');
} //End of function


/*************************************************************************************************
Function Name     : ValidateAttributeNameAndDropdowns 
Author            : Bhavya Jaitly
Modified By       : 
Description       :
Page used in      : 
Parameters        : 
                        
Modification Log  : 
*************************************************************************************************/
function ValidateAttributeNameAndDropdowns(txtAttr, ddlEntity, ddlEntityAttribute, labelError, divRTag2) {
    if (trim(GetObject(txtAttr).value) == '') {
        GetObject(labelError).innerText = '● Enter Attribute Name';
        // objDispName.focus();
        return false;
    }
    if (document.getElementById(ddlEntity).options[document.getElementById(ddlEntity).selectedIndex].value == "-1") {
        document.getElementById(labelError).innerText = '* Please select entity type';
        return false;
    }

    if (document.getElementById(ddlEntityAttribute).options[document.getElementById(ddlEntityAttribute).selectedIndex].value == "-1") {
        document.getElementById(labelError).innerText = '* Please select attribute';
        return false;
    }

    var hdn = $('#hdnTagInfo');
    if (hdn.length > 0)
        hdn.val($('#' + divRTag2).find('.hdnTag').val());

    //add ref data in collection
    saveRefDisplayAttributedetails(txtAttr, 'smtrRefDisplayName_LegAttr_dropdown', 'chkBoxRMandatory2', 'ADD');

}

/*************************************************************************************************
Function Name     : UpdateLegRefAtribRow 
Author            : Bhavya Jaitly
Modified By       : 
Description       :
Page used in      : SectypeLegDetails
Parameters        : 
                        
Modification Log  : 
*************************************************************************************************/
function UpdateLegRefAtribRow(grid,
                        hdnUpdateAtribRowId,
                        txtDisplayNameUpdate,
                         txtDescriptionUpdate,
                        hdnExpressionXMLId,
                        lblDisplayNameError,
                        modalUpdateAtrib,
                        ddlEntity, ddlEntityAttribute,
                        ddlDetailEntityAttributeId,
                        hdnDisplayName,
                        hdnAttributeList,
                        lstMaster,
                        lstCommon,
                        lstLeg,
                        chkBoxCloneable,
                        divTag) {
    var gridObj = GetObject(grid);
    var lstMasterObj = GetObject(lstMaster);
    var lstCommonObj = GetObject(lstCommon);
    var lstLegObj = GetObject(lstLeg);
    var lstCount;
    var count;
    GetObject(lblDisplayNameError).innerText = '';
    var objDispName = GetObject(txtDisplayNameUpdate);
    var objDescription = GetObject(txtDescriptionUpdate);
    var updatedDispName = trim(objDispName.value);
    var dispName = updatedDispName.toLowerCase();
    var ddlDetailEntityAttribute = $get(ddlDetailEntityAttributeId);

    // Check If Attribute name is Empty.
    if (updatedDispName == '') {
        GetObject(lblDisplayNameError).innerText = '● Enter Attribute Name';
        objDispName.focus();
        return false;
    }



    // Check If added Attribute is in Common Attributes.
    lstCount = lstCommonObj.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(lstCommonObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name is in Common Attributes';
            objDispName.focus();
            return false;
        }
    }

    // Check If added Attribute is in Details Attributes.
    lstCount = lstMasterObj.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(lstMasterObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name Exists in Master Section';
            objDispName.focus();
            return false;
        }
    }

    lstCount = lstLegObj.length;
    // Check If added Attribute is in Details Attributes.
    for (count = 0; count < lstCount; count++) {
        if (trim(lstLegObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name Exists in Leg';
            objDispName.focus();
            return false;
        }
    }
    // Check If added Attribute is already Added
    lstCount = gridObj.rows.length;
    for (count = 2; count < lstCount; count++) {
        if (dispName == trim(gridObj.rows[count].cells[0].innerText.toLowerCase())
            && GetObject(hdnDisplayName).value.toLowerCase() != updatedDispName.toLowerCase()) {
            GetObject(lblDisplayNameError).innerText = '● Attribute Name Already Exist';
            objDispName.focus();
            return false;
        }
    }

    // Check If added Attribute is among List of prohibited Attributes.
    var commonCols = GetObject(hdnAttributeList).value.split('|');
    lstCount = commonCols.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(commonCols[count].toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name is not Allowed';
            objDispName.focus();
            return false;
        }
    }
    if (document.getElementById(ddlEntity).options[document.getElementById(ddlEntity).selectedIndex].value == "-1") {
        document.getElementById(lblDisplayNameError).innerText = '* Please select entity type';
        return false;
    }
    if (document.getElementById(ddlEntityAttribute).options[document.getElementById(ddlEntityAttribute).selectedIndex].value == "-1") {
        document.getElementById(lblDisplayNameError).innerText = '* Please select attribute';
        return false;
    }
    var dataType;

    var deleteRowID = GetObject(hdnUpdateAtribRowId).value;
    var expressionsXML = '';
    var strExpressionRow = GetObject(hdnExpressionXMLId).value;
    strExpressionRow = strExpressionRow.replace(/&lts/g, '<');
    strExpressionRow = strExpressionRow.replace(/&gts/g, '>');
    //detecting selected row from table (by default this HTML table will have 2 rows)
    var rowIndex = 0;
    for (rowIndex = 1; rowIndex < gridObj.rows.length; rowIndex++) {
        if (parseInt(trim(gridObj.rows[rowIndex].cells[11].innerText)) == parseInt(deleteRowID))
            break;
    }
    gridObj.rows[rowIndex].cells[0].innerText = updatedDispName;
    gridObj.rows[rowIndex].cells[0].align = 'left';

    gridObj.rows[rowIndex].cells[1].innerHTML = '<div class="overflowingDescription">' + trim(objDescription.value) + '</div>';
    gridObj.rows[rowIndex].cells[1].align = 'left';

    gridObj.rows[rowIndex].cells[2].innerText = 'STRING';
    gridObj.rows[rowIndex].cells[2].align = 'left';

    var isCurveReferenceAttrib = false;

    if (ddlDetailEntityAttribute.disabled == false && ddlDetailEntityAttribute.value != null && ddlDetailEntityAttribute.value != "-1" && $get(ddlEntityAttribute).value != "")
        isCurveReferenceAttrib = true;

    if (!isCurveReferenceAttrib) {
        gridObj.rows[rowIndex].cells[3].innerText = trim(document.getElementById(ddlEntity).options[GetObject(ddlEntity).selectedIndex].text);
        gridObj.rows[rowIndex].cells[4].innerText = trim(document.getElementById(ddlEntity).value);
        var selectedEntityType;
        var len = document.getElementById(ddlEntityAttribute).length;
        var selectedEntityType;
        for (var x = 0; x < len; x++) {
            if (document.getElementById(ddlEntityAttribute)[x].selected == true)
                selectedEntityType = trim(document.getElementById(ddlEntityAttribute)[x].text);
        }
        gridObj.rows[rowIndex].cells[5].innerText = selectedEntityType;
        gridObj.rows[rowIndex].cells[7].innerText = trim(document.getElementById(ddlEntityAttribute).value);

    }
    else {
        //ddlEntityType,
        //ddlEntityAttribute,
        //ddlDetailEntityAttributeId,
        var detailEntityTypeId = trim(ddlDetailEntityAttribute.value).split('-')[0];
        var detailEntityAttributeId = trim(ddlDetailEntityAttribute.value).split('-')[1];
        var detailEntitytAttributeName = trim(ddlDetailEntityAttribute.options[ddlDetailEntityAttribute.selectedIndex].text).replace('-', ':');
        var masterEntityAttributeId = trim($get(ddlEntityAttribute).value);
        var masterEntityAttributeName = trim($get(ddlEntityAttribute).options[$get(ddlEntityAttribute).selectedIndex].text);
        var masterEntityTypeName = trim($get(ddlEntity).options[$get(ddlEntity).selectedIndex].text);

        gridObj.rows[rowIndex].cells[4].innerText = detailEntityTypeId;
        gridObj.rows[rowIndex].cells[4].style.display = 'none';
        gridObj.rows[rowIndex].cells[3].innerText = masterEntityTypeName
        gridObj.rows[rowIndex].cells[5].innerText = masterEntityAttributeName;
        gridObj.rows[rowIndex].cells[6].innerText = detailEntitytAttributeName;
        gridObj.rows[rowIndex].cells[7].innerText = detailEntityAttributeId;
        gridObj.rows[rowIndex].cells[7].style.display = 'none';
    }

    var isCloneable = $('#' + chkBoxCloneable).is(':checked').toString();
    gridObj.rows[rowIndex].cells[13].innerText = isCloneable;
    gridObj.rows[rowIndex].cells[13].style.display = 'none';

    var tags = $('#' + divTag).find('.hdnTag').val();
    gridObj.rows[rowIndex].cells[14].innerText = tags;
    gridObj.rows[rowIndex].cells[14].style.display = 'none';


    //deleting row from XML
    var tableArray = strExpressionRow.split('<Table>');
    var finalxml = '';

    for (var i = 0; i < tableArray.length; i++) {
        finalxml += '<Table>';
        if (i != rowIndex)
            finalxml += tableArray[i];
        else {

            var startindex = tableArray[i].indexOf('<Display_Name>') + 14;
            var Endindex = tableArray[i].indexOf('</Display_Name>');
            var firsthalf = tableArray[i].substring(0, startindex);
            var Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + updatedDispName + Secondhalf;

            var descc = trim(objDescription.value).replaceAll('<', 'ž').replaceAll('>', 'Ÿ');
            //Description            
            startindex = tableArray[i].indexOf('<Attribute_x0020_Description>') + 29;
            Endindex = tableArray[i].indexOf('</Attribute_x0020_Description>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + descc + Secondhalf;


            //data type

            startindex = tableArray[i].indexOf('<Data_Type>') + 11;
            Endindex = tableArray[i].indexOf('</Data_Type>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + 'STRING' + Secondhalf;


            //            startindex = tableArray[i].indexOf('<Data_Type>') + 11;
            //            Endindex = tableArray[i].indexOf('</Data_Type>');
            //            firsthalf = tableArray[i].substring(0, startindex);
            //            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            //            tableArray[i] = firsthalf + 'STRING' + Secondhalf;
            //            
            //            startindex = tableArray[i].indexOf('<Data_Type>') + 11;
            //            Endindex = tableArray[i].indexOf('</Data_Type>');
            //            firsthalf = tableArray[i].substring(0, startindex);
            //            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            //            tableArray[i] = firsthalf + 'STRING' + Secondhalf;
            startindex = tableArray[i].indexOf('<reference_type_id>') + 19;
            Endindex = tableArray[i].indexOf('</reference_type_id>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + trim(gridObj.rows[rowIndex].cells[4].innerText) + Secondhalf;

            startindex = tableArray[i].indexOf('<reference_type>') + 16;
            Endindex = tableArray[i].indexOf('</reference_type>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + trim(gridObj.rows[rowIndex].cells[3].innerText) + Secondhalf;

            startindex = tableArray[i].indexOf('<reference_attribute_name>') + 26;
            Endindex = tableArray[i].indexOf('</reference_attribute_name>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + trim(gridObj.rows[rowIndex].cells[5].innerText) + Secondhalf;

            startindex = tableArray[i].indexOf('<reference_leg_attribute_name>') + 30;
            Endindex = tableArray[i].indexOf('</reference_leg_attribute_name>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + trim(gridObj.rows[rowIndex].cells[6].innerText) + Secondhalf;

            startindex = tableArray[i].indexOf('<reference_attribute_id>') + 24;
            Endindex = tableArray[i].indexOf('</reference_attribute_id>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + trim(gridObj.rows[rowIndex].cells[7].innerText) + Secondhalf;

            startindex = tableArray[i].indexOf('<is_cloneable>') + 14;
            Endindex = tableArray[i].indexOf('</is_cloneable>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + isCloneable + Secondhalf;

            startindex = tableArray[i].indexOf('<tags>') + 6;
            Endindex = tableArray[i].indexOf('</tags>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + tags + Secondhalf;

            finalxml += tableArray[i];
        }
    }
    if (finalxml.indexOf('<Table>') == 0) { finalxml = finalxml.substring(7); }
    if (finalxml.lastIndexOf('</NewDataSet>') == -1) { finalxml += '</NewDataSet>'; }

    finalxml = finalxml.replace(/</g, '&lts');
    finalxml = finalxml.replace(/>/g, '&gts');
    GetObject(hdnExpressionXMLId).value = finalxml;

    // save updated value in collection
    saveRefDisplayAttributedetails(txtDisplayNameUpdate, 'sm_ref_display_update', 'chkBoxRMandatoryUpdate', 'UPDATE');
    $find(modalUpdateAtrib).hide();
} //End of function



/*************************************************************************************************
Function Name     : IsSectypeNameEmpty
Author            : Gautam Kanwar
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function IsSectypeNameEmpty(txtSectypeName, modalError, lblError) {
    if (trim(GetObject(txtSectypeName).value) == '') {
        GetObject(lblError).innerText = '● Enter Security Type Name';
        $find(modalError).show();
        return false;
    }
}

/*************************************************************************************************
Function Name     : CallLegAtribPopUp 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function CallLegAtribPopUp(modalAddAtrib,
                           txtDisplayName,
                           txtDescription,
                           lblTabNameError,
                           rblDataType,
                           hdnSecTypeTableId, sectype_table_id,
                           hdnLegName, LegName,
                           hdnIsMultiInfo, IsMultiInfo,
                           hdnIsUnderLier, IsUnderLier, txtStringLength, txtNumericIntLength, txtNumericDecimalLength, lblDecimal, tdSize, chkBoxCloneable2, divTag2) {
    GetObject(txtDisplayName).value = '';
    GetObject(txtDescription).value = '';
    GetObject(lblTabNameError).innerText = '';
    //GetObject(rblDataType).cells[0].children[0].checked = true;
    $('#' + rblDataType).find('td').eq(0)[0].children[0].checked = true;
    $(GetObject(txtStringLength)).val("");
    $(GetObject(txtNumericIntLength)).val("");
    $(GetObject(txtNumericDecimalLength)).val("");

    GetObject(txtStringLength).style.display = '';
    GetObject(txtStringLength).value = '200';
    GetObject(txtNumericIntLength).style.display = 'none';
    GetObject(txtNumericDecimalLength).style.display = 'none';
    GetObject(lblDecimal).style.display = 'none';
    GetObject(tdSize).style.display = '';
    $(GetObject(chkBoxCloneable2)).prop('checked', true);

    $find(modalAddAtrib).show();
    GetObject(txtDisplayName).focus();
    GetObject(hdnSecTypeTableId).value = sectype_table_id;
    GetObject(hdnLegName).value = LegName;
    GetObject(hdnIsMultiInfo).value = IsMultiInfo;
    GetObject(hdnIsUnderLier).value = IsUnderLier;

    var obj = {};
    obj.container = $('#' + divTag2);
    obj.list = [];
    tag.create(obj);
}

/*************************************************************************************************
Function Name     : CallLegRefAtribPopUp 
Author            : Bhavya Jaitly
Modified By       : 
Description       : opens popup 
Page used in      : SectypeLegDetails
Parameters        : 1) modalpopup Id , 2) txtbox id , 3)error label, 4)dropdown of entitytypes 5)dropdownof entitytype attributes 6)hiddenfield 7)sectypetableid 8)
                        
Modification Log  : 
*************************************************************************************************/
function CallLegRefAtribPopUp(modalAddAtrib,
                           txtDisplayName,
                           txtDescription,
                           lblTabNameError,
                           ddlEntityType,
                           ddlEntityAttributeName,
                           ddlDetailEntityAttributeId,
                           trDdlDetailEntityAttributeId,
                           hdnSecTypeTableId, sectype_table_id,
                           hdnLegName, LegName,
                           hdnIsMultiInfo, IsMultiInfo,
                           hdnIsUnderLier, IsUnderLier, chkBoxRCloneable2, divRTag2) {
    GetObject(txtDisplayName).value = '';
    GetObject(txtDescription).value = '';
    GetObject(lblTabNameError).innerText = '';
    GetObject(ddlEntityType).selectedIndex = 0;
    // GetObject(ddlEntityType).value = "-1";
    GetObject(ddlEntityAttributeName).selectedIndex = 0;
    $(GetObject(chkBoxRCloneable2)).prop('checked', true);
    $find(modalAddAtrib).show();
    // GetObject(txtDisplayName).focus();
    GetObject(hdnSecTypeTableId).value = sectype_table_id;
    GetObject(hdnLegName).value = LegName;
    GetObject(hdnIsMultiInfo).value = IsMultiInfo;
    GetObject(hdnIsUnderLier).value = IsUnderLier;
    GetObject(trDdlDetailEntityAttributeId).style.display = "none";
    GetObject(ddlDetailEntityAttributeId).selectedIndex = 0;

    var obj = {};
    obj.container = $('#' + divRTag2);
    obj.list = [];
    tag.create(obj);

    $(GetObject(ddlEntityType)).unbind('change').bind('change', function (e) {
        $('#hdnTagInfo').val($('#' + divRTag2).find('.hdnTag').val());
        var div = $('#DivCreateLegTag');
        if (div.length > 0)
            $('#hdnLegTagInfo').val($(div).find('.hdnTag').val());
    });

    //initialze ref display popup
    getRefDisplayAttributedetails(null, 'smtrRefDisplayName_LegAttr_dropdown', 'chkBoxRMandatory2', null);
}

/*************************************************************************************************
Function Name     : AddLegAtrib 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function AddLegAtrib(txtDisplayName,
txtDesription,
                     lblDisplayNameError,
                     lstCommon,
                     lstMaster,
                     lstLeg,
                     gridDetailAttributes,
                     divTag2) {
    var objDispName = GetObject(txtDisplayName);
    var objDescription = GetObject(txtDesription);
    var dispName = trim(objDispName.value.toLowerCase());
    var count;
    var lstMasterObj = GetObject(lstMaster);
    var lstCommonObj = GetObject(lstCommon);
    var lstLegObj = GetObject(lstLeg);
    var gridObj = GetObject(gridDetailAttributes);
    var lstCount;

    //Clear Error labels on Modal PopUp
    GetObject(lblDisplayNameError).innerText = '';

    // Check if the entered values are blank or Already existing: then raise error.
    if (dispName == '') {
        GetObject(lblDisplayNameError).innerText = '● Please Enter Attribute Name';
        objDispName.focus();
        return false;
    }

    // Check If added Attribute is in Common Attributes.
    lstCount = lstCommonObj.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(lstCommonObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name is in Common Attributes';
            objDispName.focus();
            return false;
        }
    }

    // Check If added Attribute is in Details Attributes.
    lstCount = lstMasterObj.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(lstMasterObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name Exists in Master Section';
            objDispName.focus();
            return false;
        }
    }

    // Check If added Attribute is already Added.
    var length = gridObj.rows.length;
    for (count = 2; count < length; count++) {
        if (trim(gridObj.rows[count].cells[1].innerText.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● Attribute Name Already Exists';
            objDispName.focus();
            return false;
        }
    }

    lstCount = lstLegObj.length;
    // Check If added Attribute is in Details Attributes.
    for (count = 0; count < lstCount; count++) {
        if (trim(lstLegObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name Exists in Leg';
            objDispName.focus();
            return false;
        }
    }

    var hdn = $('#hdnTagInfo');
    if (hdn.length > 0)
        hdn.val($('#' + divTag2).find('.hdnTag').val());

    // save attr in collection
    saveRefDisplayAttributedetails(txtDisplayName, null, 'chkBoxMandatory2', 'ADD');

}

/*************************************************************************************************
Function Name     : AddLegRefAtrib 
Author            : Bhavya Jaitly
Modified By       : 
Description       : validattions 
Page used in      : SectypeLegDetails
Parameters        : 1) txtbox Id , 2) list of common attributes , 3)list of master attributes , 4)list of leg attributes 5)grid id
Modification Log  : 
*************************************************************************************************/
function AddLegRefAtrib(txtDisplayName,
                     lblDisplayNameError,
                     lstCommon,
                     lstMaster,
                     lstLeg,
                     gridDetailAttributes) {
    var objDispName = GetObject(txtDisplayName);
    var dispName = trim(objDispName.value.toLowerCase());
    var count;
    var lstMasterObj = GetObject(lstMaster);
    var lstCommonObj = GetObject(lstCommon);
    var lstLegObj = GetObject(lstLeg);
    var gridObj = GetObject(gridDetailAttributes);
    var lstCount;

    //Clear Error labels on Modal PopUp
    GetObject(lblDisplayNameError).innerText = '';

    // Check if the entered values are blank or Already existing: then raise error.
    if (dispName == '') {
        GetObject(lblDisplayNameError).innerText = '● Please Enter Attribute Name';
        objDispName.focus();
        return false;
    }

    // Check If added Attribute is in Common Attributes.
    lstCount = lstCommonObj.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(lstCommonObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name is in Common Attributes';
            objDispName.focus();
            return false;
        }
    }

    // Check If added Attribute is in Details Attributes.
    lstCount = lstMasterObj.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(lstMasterObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name Exists in Master Section';
            objDispName.focus();
            return false;
        }
    }

    // Check If added Attribute is already Added.
    var length = gridObj.rows.length;
    for (count = 2; count < length; count++) {
        if (trim(gridObj.rows[count].cells[1].innerText.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● Attribute Name Already Exists';
            objDispName.focus();
            return false;
        }
    }

    lstCount = lstLegObj.length;
    // Check If added Attribute is in Details Attributes.
    for (count = 0; count < lstCount; count++) {
        if (trim(lstLegObj[count].text.toLowerCase()) == dispName) {
            GetObject(lblDisplayNameError).innerText = '● This Attribute Name Exists in Leg';
            objDispName.focus();
            return false;
        }
    }
}


/*************************************************************************************************
Function Name     : CheckSaveLeg 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function CheckSaveLeg(gridDetailAttributes,
                      modalError,
                      lblError,
                      txtLegName,
                      hdnLegNames,
                      DivCreateLegTag) {
    var legname = trim(GetObject(txtLegName).value).toLowerCase();

    $('#hdnLegTagInfo').val($('#' + DivCreateLegTag).find('.hdnTag').val());

    // Check If Leg Name is entered
    if (legname == '') {
        GetObject(lblError).innerText = '● Enter Leg Name';
        $find(modalError).show();
        return false;
    }
    var regex = new RegExp(/^\d$/);
    if (regex.test(legname.substring(0, 1))) {
        GetObject(lblError).innerText = '● Leg Name cannot start with numeric values.';
        $find(modalError).show();
        return false;
    }
    // Check If Some attribute is added or not.
    if (GetObject(gridDetailAttributes).rows.length < 3) {
        GetObject(lblError).innerText = '● Add Attributes to Leg';
        $find(modalError).show();
        return false;
    }
    var LegName = GetObject(hdnLegNames).value;
    if (LegName != '') {
        var LegNames = GetObject(hdnLegNames).value.split('|');
        var length = LegNames.length;
        var count;
        for (count = 0; count < length; count++) {
            if (trim(LegNames[count]).toLowerCase() == legname) {
                GetObject(lblError).innerText = '● Leg With This Name Already exists.';
                $find(modalError).show();
                return false;
            }
        }
    }
    return true;
}

/*************************************************************************************************
Function Name     : UpdateLegName 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
//function UpdateLegName(modalUpdateLegName,
//                       txtUpdateLegName,
//                       lblUpdateLegNameError,
//                       hdnSecTypeTableId,
//                       sectype_table_id,
//                       hdnLegName,ddlPrimarySavedLeg,
//                       Leg_Name,primaryfield) {
//                       var ddl=GetObject(ddlPrimarySavedLeg);
//                       var length=ddl.length;
//                       for(var count=0;count<length;++count)
//                       {
//                       if(ddl[count].text==primaryfield)
//                         ddl.selectedIndex = count;
//                         break;
//                       }
//    GetObject(txtUpdateLegName).value = trim(Leg_Name);
//    GetObject(hdnLegName).value = trim(Leg_Name);
//    GetObject(lblUpdateLegNameError).innerText = '';
//    GetObject(hdnSecTypeTableId).value = sectype_table_id;
//    $find(modalUpdateLegName).show();
//    return false;
//}

/*************************************************************************************************
Function Name     : CheckUpdateLeg 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function CheckUpdateLeg(txtUpdateLegName, lblUpdateLegNameError, hdnLegName, hdnLegNames, ddl, divLegTags) {
    var updatedlegName = trim(GetObject(txtUpdateLegName).value).toLowerCase();
    $('#hdnTagInfo').val($('#' + divLegTags).find('.hdnTag').val());

    var regex = new RegExp(/^\d$/);
    if (updatedlegName == '') {
        GetObject(lblUpdateLegNameError).innerText = '● Enter Leg Name';
        return false;
    }
    if (regex.test(updatedlegName.substring(0, 1))) {
        GetObject(lblUpdateLegNameError).innerText = '● Leg Name cannot start with numeric values.';
        return false;
    }
    //    if (GetObject(ddl).disabled == false && GetObject(ddl).selectedIndex == 0) {
    //        GetObject(lblUpdateLegNameError).innerText = '● Select Primary Key';
    //        return false;
    //    }
    if (trim(GetObject(hdnLegName).value).toLowerCase() == updatedlegName) {
        return true;
    }
    var LegNames = GetObject(hdnLegNames).value.split('|');
    var length = LegNames.length;
    var count;
    for (count = 0; count < length; count++) {
        if (trim(LegNames[count]).toLowerCase() == updatedlegName) {
            GetObject(lblUpdateLegNameError).innerText = '● Duplicate Leg Name';
            return false;
        }
    }
}

/*************************************************************************************************
Function Name     : ShowHideddlUnderlierMaster 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function ShowHideddlUnderlierMaster(chkIsUnderlierId, ddlDefaultUnderlierId, ddlUnderlyingIdentifierAttributeId) {
    var chkIsUnderlier = document.getElementById(chkIsUnderlierId);
    var ddlDefaultUnderlier = document.getElementById(ddlDefaultUnderlierId);
    var ddlUnderlyingIdentifierAttribute = document.getElementById(ddlUnderlyingIdentifierAttributeId);
    var underlyingIndex = parseInt(chkIsUnderlier.parentNode.getAttributeNode('underlyingIndex').value);
    var identifierIndex = parseInt(chkIsUnderlier.parentNode.getAttributeNode('identifierIndex').value);

    if (chkIsUnderlier.checked) {
        ddlDefaultUnderlier.style.display = '';
        if (ddlDefaultUnderlier.options.length > 0) {
            ddlDefaultUnderlier.selectedIndex = underlyingIndex;
        }
        if (ddlUnderlyingIdentifierAttribute != null && ddlUnderlyingIdentifierAttribute != undefined) {
            ddlUnderlyingIdentifierAttribute.style.display = '';
            if (ddlUnderlyingIdentifierAttribute.options.length > 0) {
                ddlUnderlyingIdentifierAttribute.selectedIndex = identifierIndex;
            }
        }
    }
    else {
        ddlDefaultUnderlier.style.display = 'none';
        if (ddlDefaultUnderlier.options.length > 0) {
            ddlDefaultUnderlier.selectedIndex = underlyingIndex;
        }
        if (ddlUnderlyingIdentifierAttribute != null && ddlUnderlyingIdentifierAttribute != undefined) {
            ddlUnderlyingIdentifierAttribute.style.display = 'none';
            if (ddlUnderlyingIdentifierAttribute.options.length > 0) {
                ddlUnderlyingIdentifierAttribute.selectedIndex = identifierIndex;
            }
        }
    }
    //    if (GetObject(chkIsUnderlier).checked) {
    //        GetObject(ddlDefaultUnderlier).style.display = '';
    //        if (GetObject(ddlUnderlyingIdentifierAttribute) != null)
    //            GetObject(ddlUnderlyingIdentifierAttribute).style.display = '';
    //    }
    //    else {
    //        GetObject(ddlDefaultUnderlier).style.display = 'none';
    //        if (GetObject(ddlUnderlyingIdentifierAttribute) != null)
    //            GetObject(ddlUnderlyingIdentifierAttribute).style.display = 'none';
    //    }
}

function ShowHideddlUnderlierLeg(chkIsUnderlierId, ddlDefaultUnderlierId) {
    var chkIsUnderlier = document.getElementById(chkIsUnderlierId);
    var ddlDefaultUnderlier = document.getElementById(ddlDefaultUnderlierId);
    if (chkIsUnderlier.checked) {
        ddlDefaultUnderlier.style.display = '';
    }
    else {
        ddlDefaultUnderlier.style.display = 'none';
    }
    //    if (GetObject(chkIsUnderlier).checked) {
    //        GetObject(ddlDefaultUnderlier).style.display = '';
    //        if (GetObject(ddlUnderlyingIdentifierAttribute) != null)
    //            GetObject(ddlUnderlyingIdentifierAttribute).style.display = '';
    //    }
    //    else {
    //        GetObject(ddlDefaultUnderlier).style.display = 'none';
    //        if (GetObject(ddlUnderlyingIdentifierAttribute) != null)
    //            GetObject(ddlUnderlyingIdentifierAttribute).style.display = 'none';
    //    }
}

/*************************************************************************************************
Function Name     : AddMnemonic 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function AddMnemonic(ddlAttributeName, txtDisplayName, lblMnemonicError) {
    var objDdl = GetObject(ddlAttributeName);
    if (objDdl.selectedIndex != -1) {
        var text = trim(GetObject(txtDisplayName).value) + '[' + trim(objDdl[objDdl.selectedIndex].innerText) + ']'
        if (text.length <= 200) {
            GetObject(txtDisplayName).value = text;
            GetObject(lblMnemonicError).innerText = '';
        }
        else
            GetObject(lblMnemonicError).innerText = 'Mnemonic can Not be of more than 200 characters';
        objDdl.selectedIndex = -1;
    }
}

/*************************************************************************************************
Function Name     : ValidateMnemonic 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function ValidateMnemonic(txtDisplayName, ddlAttributeName, lblMnemonicError, hdnAttribute, hdnID) {
    var objlblMnemonicError = GetObject(lblMnemonicError);
    objlblMnemonicError.innerText = '';
    var strName = GetObject(txtDisplayName).value;
    if (strName == '') {
        GetObject(hdnAttribute).value = '';
        GetObject(hdnID).value = '';
        return true;
    }

    var Attribute = '';
    var ID = '';
    var str = '';
    var currChar;
    var count = 0;
    var isOpen = 0;
    var isClose = 0;
    var length = strName.length;

    var objDdl = GetObject(ddlAttributeName);
    var ddlLength = objDdl.length;
    var isValid = false;
    while (count < length) {
        currChar = strName.substring(count, ++count);

        if (currChar != '[' && currChar != ']')
            str += currChar;
        else if (currChar == '[')
            isOpen++;
        else
            isClose++;

        if (isOpen < isClose || isOpen > isClose + 1) {
            GetObject(lblMnemonicError).innerText = 'Incorrect Mnemonic';
            return false;
        }

        if (isOpen == isClose && (currChar == '[' || currChar == ']') && str.length > 0) {
            isValid = true;
            for (var ddlIndex = 0; ddlIndex < ddlLength; ddlIndex++) {
                if (trim(objDdl[ddlIndex].innerText) == str) {
                    isValid = true;
                    Attribute += str + '~';
                    ID += objDdl[ddlIndex].value + '~';
                    str = '';
                    break;
                }
                else
                    isValid = false;
            }
            if (!isValid) {
                GetObject(lblMnemonicError).innerText = 'Incorrect Mnemonic';
                return false;
            }
        }
        else if (strName.substring(count, count + 1) == '[' && str.length > 0) {
            Attribute += str + '~';
            ID += '0~';
            str = '';
        }
    }
    if (isOpen != isClose) {
        GetObject(lblMnemonicError).innerText = 'Incorrect Mnemonic';
        return false;
    }
    if (str.length > 0) {
        Attribute += str + '~';
        ID += '0~';
    }
    GetObject(hdnAttribute).value = Attribute.substring(0, Attribute.length - 1);
    GetObject(hdnID).value = ID.substring(0, ID.length - 1);
    return true;
}

/*************************************************************************************************
Function Name     : ResetMnemonic 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function ResetMnemonic(txtDisplayName, hdnAttribute) {
    GetObject(txtDisplayName).value = GetObject(hdnAttribute).value;
}

/*************************************************************************************************
Function Name     : CheckTxtBoxLength 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function CheckTxtBoxLength(txtDisplayName, lblMnemonicError) {
    var objTxt = GetObject(txtDisplayName);
    var text = objTxt.value;
    if (text.length > 200) {
        objTxt.value = text.substring(0, 200);
        GetObject(lblMnemonicError).innerText = 'Mnemonic can Not be of more than 200 characters';
    }
    else
        GetObject(lblMnemonicError).innerText = '';
    return false;
}

/*************************************************************************************************
Function Name     : UpdateExistingAtrib 
Author            : Vishal Gupta
Description       : 
Page used in      : <Page Name> <Module Name>
Parameters        : 
Modification Log  : 
*************************************************************************************************/
function UpdateExistingAtrib(grid,
                            hdnUpdateAtribRowId,
                            txtDisplayNameUpdate,
                            hdnExpressionXMLId,
                            lblDisplayNameUpdateError,
                            lstExistingAttributes,
                            hdnDisplayName,
                            hdnAttributeList,
                            lstDetail,
                            txtStringSizeUpdateExistingAttribute, txtNumericIntSizeUpdateExistingAttribute,
                            txtNumericDecimalSizeUpdateExistingAttribute,
                            lblDecimalUpdateExistingAttribute,
                            chkBoxCloneableUpdate,
                            tdSizeUpdateExistingAttribute,
                            divTagUpdate) {
    var gridObj = GetObject(grid);
    var lstBoxObj = GetObject(lstExistingAttributes);
    var lstDetailObj = GetObject(lstDetail);
    var lstCount = lstBoxObj.length;
    var count;
    var datatype;
    var updatedDataTypeSize;

    var regEx = new RegExp(String.fromCharCode(160), 'gi');
    var normalSpace = ' ';

    $('#' + hdnDisplayName).val($('#' + hdnDisplayName).val().replace(regEx, normalSpace));

    GetObject(lblDisplayNameUpdateError).innerText = '';
    var updatedDispName = trim(GetObject(txtDisplayNameUpdate).value);
    var updatedDescription = $("[id$=txtDescriptionExistingUpdate]").val();
    var updatedDispNameL = updatedDispName.toLowerCase();
    // Check If Attribute name is Empty.
    if (updatedDispName == '') {
        GetObject(lblDisplayNameUpdateError).innerText = '● Enter Attribute Name';
        GetObject(txtDisplayNameUpdate).focus();
        return false;
    }
    // Check If added Attribute is in Common Attributes
    for (count = 0; count < lstCount; count++) {
        if (trim(lstBoxObj[count].text.toLowerCase()) == updatedDispNameL) {
            GetObject(lblDisplayNameUpdateError).innerText = '● Attribute Name Already Exists';
            GetObject(txtDisplayNameUpdate).focus();
            return false;
        }
    }
    // Check If added Attribute is in Details Attributes.
    lstCount = lstDetailObj.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(lstDetailObj[count].text.toLowerCase()) == updatedDispNameL) {
            GetObject(lblDisplayNameUpdateError).innerText = '● This Attribute Name Exists in Leg';
            GetObject(txtDisplayNameUpdate).focus();
            return false;
        }
    }
    // Check If added Attribute is already Added
    for (count = 2; count < gridObj.rows.length; count++) {
        if (updatedDispNameL == trim(gridObj.rows[count].cells[0].innerText.toLowerCase())
            && GetObject(hdnDisplayName).value.toLowerCase() != updatedDispName.toLowerCase()) {
            GetObject(lblDisplayNameUpdateError).innerText = '● Attribute Name Already Exist';
            GetObject(txtDisplayNameUpdate).focus();
            return false;
        }
    }

    // Check If added Attribute is among List of prohibited Attributes.
    var commonCols = GetObject(hdnAttributeList).value.split('|');
    lstCount = commonCols.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(commonCols[count].toLowerCase()) == updatedDispNameL) {
            GetObject(lblDisplayNameUpdateError).innerText = '● This Attribute Name is not Allowed';
            GetObject(txtDisplayNameUpdate).focus();
            return false;
        }
    }

    var deleteRowID = GetObject(hdnUpdateAtribRowId).value;
    var expressionsXML = '';
    var strExpressionRow = GetObject(hdnExpressionXMLId).value;
    strExpressionRow = strExpressionRow.replace(/&lts/g, '<');
    strExpressionRow = strExpressionRow.replace(/&gts/g, '>');
    //deleting selected row from table (by default this HTML table will have 2 rows)
    var rowIndex = 0;
    for (rowIndex = 1; rowIndex < gridObj.rows.length; rowIndex++) {
        if (parseInt(trim(gridObj.rows[rowIndex].cells[2].innerText)) == parseInt(deleteRowID))
            break;
    }
    datatype = trim(gridObj.rows[rowIndex].cells[3].innerText);
    gridObj.rows[rowIndex].cells[0].innerText = updatedDispName;
    gridObj.rows[rowIndex].cells[0].align = 'left';

    gridObj.rows[rowIndex].cells[1].innerHTML = '<div class="overflowingDescription">' + updatedDescription + '</div>';
    gridObj.rows[rowIndex].cells[1].align = 'left';

    if (datatype.toLowerCase() == 'string') {
        if (GetObject(txtStringSizeUpdateExistingAttribute).value.toLowerCase() == '') {
            GetObject(lblDisplayNameUpdateError).innerText = '● Please Enter Attribute Size';
            return false;
        }
        if (parseInt(trim(GetObject(txtStringSizeUpdateExistingAttribute).value)) > 8000) {
            GetObject(lblDisplayNameUpdateError).innerText = '●Attribute Size Should Be Less Than Or Equal To 8000';
            return false;
        }
        if (parseInt(trim(GetObject(txtStringSizeUpdateExistingAttribute).value).toLowerCase()) < parseInt(trim(gridObj.rows[rowIndex].cells[9].innerText))) {
            GetObject(lblDisplayNameUpdateError).innerText = '● Attribute Size Should be greater than or equal to  ' + parseInt(trim(gridObj.rows[rowIndex].cells[9].innerText));
            return false;
        }
        updatedDataTypeSize = GetObject(txtStringSizeUpdateExistingAttribute).value;

    }
    else if (datatype.toLowerCase() == 'numeric') {
        if (trim(GetObject(txtNumericIntSizeUpdateExistingAttribute).value) == '' || trim(GetObject(txtNumericDecimalSizeUpdateExistingAttribute).value) == '') {
            GetObject(lblDisplayNameUpdateError).innerText = '● Please Enter Attribute Size';
            return false;
        }
        if (parseInt(trim(GetObject(txtNumericIntSizeUpdateExistingAttribute).value).toLowerCase()) < parseInt(trim(gridObj.rows[rowIndex].cells[9].innerText).split('.')[0])) {
            GetObject(lblDisplayNameUpdateError).innerText = '● Length of string before decimal should be greater than or equal to  ' + parseInt(trim(gridObj.rows[rowIndex].cells[9].innerText).split('.')[0]);
            return false;
        }
        if (parseInt(trim(GetObject(txtNumericDecimalSizeUpdateExistingAttribute).value).toLowerCase()) < parseInt(trim(gridObj.rows[rowIndex].cells[9].innerText).split('.')[1])) {
            GetObject(lblDisplayNameUpdateError).innerText = '● Length of string after decimal should be greater than or equal to   ' + parseInt(trim(gridObj.rows[rowIndex].cells[9].innerText).split('.')[1]);
            return false;
        }
        if (Number(trim(GetObject(txtNumericIntSizeUpdateExistingAttribute).value)) + Number(trim(GetObject(txtNumericDecimalSizeUpdateExistingAttribute).value)) > 28) {
            GetObject(lblDisplayNameUpdateError).innerText = '● Precision Can Have A Maximum Value Of 28 (Precision is no. of digits before decimal + no. of digits after decimal) ';
            return false;
        }
        updatedDataTypeSize = GetObject(trim(txtNumericIntSizeUpdateExistingAttribute).value) + '.' + trim(GetObject(txtNumericDecimalSizeUpdateExistingAttribute).value);

    }

    else
        updatedDataTypeSize = '';




    //deleting row from XML
    var tableArray = strExpressionRow.split('<Table>');
    var finalxml = '';

    for (var i = 0; i < tableArray.length; i++) {
        finalxml += '<Table>';
        if (i != rowIndex)
            finalxml += tableArray[i];
        else {

            var startindex = tableArray[i].indexOf('<Display_Name>') + 14;
            var Endindex = tableArray[i].indexOf('</Display_Name>');
            var firsthalf = tableArray[i].substring(0, startindex);
            var Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + updatedDispName + Secondhalf;

            updatedDescription = updatedDescription.replaceAll('<', 'ž').replaceAll('>', 'Ÿ');

            startindex = tableArray[i].indexOf('<Attribute_x0020_Description>') + 29;
            Endindex = tableArray[i].indexOf('</Attribute_x0020_Description>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + updatedDescription + Secondhalf;

            var startindex = tableArray[i].indexOf('<is_cloneable>') + 14;
            var Endindex = tableArray[i].indexOf('</is_cloneable>');
            var firsthalf = tableArray[i].substring(0, startindex);
            var Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + $('#' + chkBoxCloneableUpdate).is(':checked').toString() + Secondhalf;

            var startindex = tableArray[i].indexOf('<tags>') + 6;
            var Endindex = tableArray[i].indexOf('</tags>');
            var firsthalf = tableArray[i].substring(0, startindex);
            var Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + $('#' + divTagUpdate).find('.hdnTag').val() + Secondhalf;

            startindex = tableArray[i].indexOf('<attribute_size>') + 16;
            Endindex = tableArray[i].indexOf('</attribute_size>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            if (updatedDataTypeSize != '') {
                tableArray[i] = firsthalf + updatedDataTypeSize + Secondhalf;
            }
            else {
                tableArray[i] = firsthalf + '-1' + Secondhalf;
            }


            finalxml += tableArray[i];
        }
    }
    if (finalxml.indexOf('<Table>') == 0) { finalxml = finalxml.substring(7); }
    if (finalxml.lastIndexOf('</NewDataSet>') == -1) { finalxml += '</NewDataSet>'; }

    finalxml = finalxml.replace(/</g, '&lts');
    finalxml = finalxml.replace(/>/g, '&gts');
    GetObject(hdnExpressionXMLId).value = finalxml;

    saveRefDisplayAttributedetails(txtDisplayNameUpdate, null, 'chkBoxMandatoryUpdate', 'ADD');

    return true;
} //End of function

/*************************************************************************************************
Function Name     : UpdateExistingRefAtrib 
Author            : Bhavya Jaitly
Modified By       : 
Description       : Update xml for reference data values
Page used in      : SectypeLegDetails
Parameters        : 1) txtbox Id , 2) entitydropdown , 3)hiddenfield , 4) Error Label
Modification Log  : 
*************************************************************************************************/
function UpdateExistingRefAtrib(grid,
                            hdnUpdateAtribRowId,
                            txtDisplayNameUpdate,
                            hdnExpressionXMLId,
                            lblDisplayNameUpdateError,
                            lstExistingAttributes,
                            hdnDisplayName,
                            hdnAttributeList,
                            lstDetail, ddlAttributeName, hdn,
                            ddlLegAttributeNamesId,
                            chkBoxRCloneableUpdate,
                            ddlEntityId,
                            divRTagUpdate) {
    var gridObj = GetObject(grid);
    var lstBoxObj = GetObject(lstExistingAttributes);
    var lstDetailObj = GetObject(lstDetail);
    var lstCount = lstBoxObj.length;
    var count;
    GetObject(lblDisplayNameUpdateError).innerText = '';
    var ddlLegAttributeNames = $get(ddlLegAttributeNamesId);

    var len = document.getElementById(ddlAttributeName).length;

    var updatedDispName = trim(GetObject(txtDisplayNameUpdate).value);
    var updatedDispNameL = updatedDispName.toLowerCase();

    var updatedDescription = $("[id$=txtUpdateAttributeDescription]").val();
    // Check If Attribute name is Empty.
    if (updatedDispName == '') {
        GetObject(lblDisplayNameUpdateError).innerText = '● Enter Attribute Name';
        GetObject(txtDisplayNameUpdate).focus();
        return false;
    }
    if (document.getElementById(ddlAttributeName).options[document.getElementById(ddlAttributeName).selectedIndex].value == "-1") {
        document.getElementById(lblDisplayNameUpdateError).innerText = '* Please select attribute';
        return false;
    }


    // Check If added Attribute is in Common Attributes
    for (count = 0; count < lstCount; count++) {
        if (trim(lstBoxObj[count].text.toLowerCase()) == updatedDispNameL) {
            GetObject(lblDisplayNameUpdateError).innerText = '● Attribute Name Already Exists';
            GetObject(txtDisplayNameUpdate).focus();
            return false;
        }
    }
    // Check If added Attribute is in Details Attributes.
    lstCount = lstDetailObj.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(lstDetailObj[count].text.toLowerCase()) == updatedDispNameL) {
            GetObject(lblDisplayNameUpdateError).innerText = '● This Attribute Name Exists in Leg';
            GetObject(txtDisplayNameUpdate).focus();
            return false;
        }
    }
    // Check If added Attribute is already Added
    for (count = 2; count < gridObj.rows.length; count++) {
        if (updatedDispNameL == trim(gridObj.rows[count].cells[0].innerText.toLowerCase())
            && GetObject(hdnDisplayName).value.toLowerCase() != updatedDispName.toLowerCase()) {
            GetObject(lblDisplayNameUpdateError).innerText = '● Attribute Name Already Exist';
            GetObject(txtDisplayNameUpdate).focus();
            return false;
        }
    }

    // Check If added Attribute is among List of prohibited Attributes.
    var commonCols = GetObject(hdnAttributeList).value.split('|');
    lstCount = commonCols.length;
    for (count = 0; count < lstCount; count++) {
        if (trim(commonCols[count].toLowerCase()) == updatedDispNameL) {
            GetObject(lblDisplayNameUpdateError).innerText = '● This Attribute Name is not Allowed';
            GetObject(txtDisplayNameUpdate).focus();
            return false;
        }
    }

    var deleteRowID = GetObject(hdnUpdateAtribRowId).value;
    var expressionsXML = '';
    var strExpressionRow = GetObject(hdnExpressionXMLId).value;
    strExpressionRow = strExpressionRow.replace(/&lts/g, '<');
    strExpressionRow = strExpressionRow.replace(/&gts/g, '>');
    //deleting selected row from table (by default this HTML table will have 2 rows)
    var rowIndex = 0;
    for (rowIndex = 1; rowIndex < gridObj.rows.length; rowIndex++) {
        if (parseInt(trim(gridObj.rows[rowIndex].cells[2].innerText)) == parseInt(deleteRowID))//--->
            break;
    }
    gridObj.rows[rowIndex].cells[0].innerText = updatedDispName;
    gridObj.rows[rowIndex].cells[0].align = 'left';

    gridObj.rows[rowIndex].cells[1].innerHTML = '<div class="overflowingDescription">' + updatedDescription + '</div>';
    gridObj.rows[rowIndex].cells[1].align = 'left';

    var isCurveReferenceAttrib = false;

    if (ddlLegAttributeNames.disabled == false && ddlLegAttributeNames.value != null && ddlLegAttributeNames.value != "-1" && $get(ddlAttributeName).value != "")
        isCurveReferenceAttrib = true;

    var selectedEntityType;
    var entityAttributeId;

    if (!isCurveReferenceAttrib) {
        for (var x = 0; x < len; x++) {
            if (document.getElementById(ddlAttributeName)[x].selected == true)
                selectedEntityType = document.getElementById(ddlAttributeName)[x].text;
        }
        entityAttributeId = document.getElementById(ddlAttributeName).value;

    }
    else {
        var detailEntityTypeId = ddlLegAttributeNames.value.split('-')[0];
        var detailEntityAttributeId = ddlLegAttributeNames.value.split('-')[1];
        var detailEntitytAttributeName = ddlLegAttributeNames.options[ddlLegAttributeNames.selectedIndex].text.split('-')[1];
        var masterEntityAttributeId = $get(ddlAttributeName).value;
        var masterEntityAttributeName = $get(ddlAttributeName).options[$get(ddlAttributeName).selectedIndex].text;
        var masterEntityTypeName = $get(ddlEntityId).options[$get(ddlEntityId).selectedIndex].text;

        selectedEntityType = masterEntityAttributeName + ':' + detailEntitytAttributeName; // RealNameRequired
        entityAttributeId = detailEntityAttributeId; // Used only to bind grid in UI
    }

    //deleting row from XML
    var tableArray = strExpressionRow.split('<Table>');
    var finalxml = '';
    document.getElementById(hdn).value = selectedEntityType;

    for (var i = 0; i < tableArray.length; i++) {
        finalxml += '<Table>';
        if (i != rowIndex)
            finalxml += tableArray[i];
        else {
            var startindex = tableArray[i].indexOf('<Display_Name>') + 14;
            var Endindex = tableArray[i].indexOf('</Display_Name>');
            var firsthalf = tableArray[i].substring(0, startindex);
            var Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + updatedDispName + Secondhalf;

            updatedDescription = updatedDescription.replaceAll('<', 'ž').replaceAll('>', 'Ÿ');

            startindex = tableArray[i].indexOf('<Attribute_x0020_Description>') + 29;
            Endindex = tableArray[i].indexOf('</Attribute_x0020_Description>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + updatedDescription + Secondhalf;

            startindex = tableArray[i].indexOf('<reference_type_id>') + 19;
            Endindex = tableArray[i].indexOf('</reference_type_id>');
            firsthalf = tableArray[i].substring(0, startindex);
            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + trim(gridObj.rows[rowIndex].cells[4].innerText) + Secondhalf;

            //            startindex = tableArray[i].indexOf('<reference_type>') + 16;
            //            Endindex = tableArray[i].indexOf('</reference_type>');
            //            firsthalf = tableArray[i].substring(0, startindex);
            //            Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            //            tableArray[i] = firsthalf + gridObj.rows[rowIndex].cells[5].innerText + Secondhalf;

            var startindex = tableArray[i].indexOf('<reference_attribute_id>') + 24;
            var Endindex = tableArray[i].indexOf('</reference_attribute_id>');
            var firsthalf = tableArray[i].substring(0, startindex);
            var Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + entityAttributeId + Secondhalf;

            var startindex = tableArray[i].indexOf('<reference_attribute_name>') + 26;
            var Endindex = tableArray[i].indexOf('</reference_attribute_name>');
            var firsthalf = tableArray[i].substring(0, startindex);
            var Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + selectedEntityType + Secondhalf;

            var startindex = tableArray[i].indexOf('<is_cloneable>') + 14;
            var Endindex = tableArray[i].indexOf('</is_cloneable>');
            var firsthalf = tableArray[i].substring(0, startindex);
            var Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + $('#' + chkBoxRCloneableUpdate).is(':checked').toString() + Secondhalf;

            var startindex = tableArray[i].indexOf('<tags>') + 6;
            var Endindex = tableArray[i].indexOf('</tags>');
            var firsthalf = tableArray[i].substring(0, startindex);
            var Secondhalf = tableArray[i].substring(Endindex, tableArray[i].length);
            tableArray[i] = firsthalf + $('#' + divRTagUpdate).find('.hdnTag').val() + Secondhalf;

            finalxml += tableArray[i];
        }
    }
    if (finalxml.indexOf('<Table>') == 0) { finalxml = finalxml.substring(7); }
    if (finalxml.lastIndexOf('</NewDataSet>') == -1) { finalxml += '</NewDataSet>'; }

    finalxml = finalxml.replace(/</g, '&lts');
    finalxml = finalxml.replace(/>/g, '&gts');
    GetObject(hdnExpressionXMLId).value = finalxml;

    //add ref attr
    saveRefDisplayAttributedetails(txtDisplayNameUpdate, 'smtrRefDisplayName_dropdown_update', 'chkBoxRefMandatory', 'ADD');

    return true;
} //End of function




function ValidateNewTemplate(txtTemplateName, lblTemplateName, lblTemplateType, ddlTemplatetype, lblError) {
    var isvalid = 'true';
    var errormsg = '';
    if (document.getElementById(txtTemplateName).value == '') {
        isvalid = 'false';
        errormsg = '  ' + '● Please enter ' + (document.getElementById(lblTemplateName).innerText).replace(':', '') + '.' + '\n';

    }
    if (document.getElementById(ddlTemplatetype).selectedIndex == 0) {
        isvalid = 'false';
        errormsg += '  ' + '● Please enter ' + (document.getElementById(lblTemplateType).innerText).replace(':', '') + '.';
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
var ddls;
var ddl2;
var ddlsRef;
function registerEventForRefAttrib(ddlId) {
    $get(ddlId).onchange = callWebService;

}
function registerEventForRefAttribLeg(ddlId, ddlentityAdd, ddlEntityAttributes) {
    $get(ddlId).onchange = callWebService;
    $get(ddlentityAdd).onchange = callWebServiceForAddLeg(ddlEntityAttributes);

}

function callWebService() {
    var ele = window.event.srcElement;
    showRefAttributes(ele.value, null, null);
}
function callWebServiceForAddLeg(ddl) {
    var ele = window.event.srcElement;
    showRefAttributes(ele.value, ddl, null);

}

function showRefAttributes(entityID, ddlToBind, ddlDetailAttributes) {
    if (ddlToBind != null)
        ddls = ddlToBind;
    if (ddlDetailAttributes != null)
        ddl2 = ddlDetailAttributes;
    com.ivp.secm.api.intermediate.SMRefAttributeAddition.GetRefAttributes(entityID, succedGetReferredAttribCallBack, failureGetReferredAttribCallBack, entityID);
}

function succedGetReferredAttribCallBack(result, args) {
    var entityID = args;
    var arrayData = Sys.Serialization.JavaScriptSerializer.deserialize(result);
    var ddlToAddOptions = $get(ddls);
    ddlToAddOptions.disabled = false;
    var length = ddlToAddOptions.options.length;
    for (var i = 0; i < length; i++) {
        ddlToAddOptions.removeChild(ddlToAddOptions.options[0]);
    }
    var option = null;
    for (var i = 0; i < arrayData.length; i++) {
        option = document.createElement('option');
        option.text = arrayData[i].attributeName;
        //option.innerText = arrayData[i].attributeName;
        option.value = arrayData[i].attributeId;
        ddlToAddOptions.add(option);
    }

    var length = ddlToAddOptions.length;
    if (gName != '') {
        if (gName.split(':').length > 1) {
            //Case when Update button is pressed and Curve attribute is being updated
            //Select the master attribute as it should be..
            var masterAttributeName = gName.split(':')[0];
            for (var count = 1; count < length; count++) {
                if (ddlToAddOptions[count].text.replaceNBR().toLowerCase() == masterAttributeName.toLowerCase()) {
                    ddlToAddOptions.selectedIndex = count;
                    break;
                }
                else if (trim(ddlToAddOptions[count].innerText).replaceNBR().toLowerCase() == trim(masterAttributeName).replaceNBR().toLowerCase()) {
                    ddlToAddOptions.selectedIndex = count;
                    break;
                }
            }
        }
        else {
            for (var count = 1; count < length; count++) {
                if (trim(ddlToAddOptions[count].text).replaceNBR().toLowerCase() == trim(gName).replaceNBR().toLowerCase()) {
                    ddlToAddOptions.selectedIndex = count;
                    break;
                }
                else if (trim(ddlToAddOptions[count].innerText).replaceNBR().toLowerCase() == trim(gName).replaceNBR().toLowerCase()) {
                    ddlToAddOptions.selectedIndex = count;
                    break;
                }
            }
            gName = '';
        }
    }

    //Call service to bind detail attributes -> keep value as detailEntityId-detailEntityAttributeId
    //Only those attributes are send.. that are allowed to be bind..
    //In case of Update button press in grid -> check gName and select the attribute that was selected
    //in success method, if list of attributes is recieved, enable the dropdown and show TR
    //else disable dropdown and hide TR
    var ddlLegAttributes = $get(ddl2);
    var permanent = ddlLegAttributes.getAttribute('permanent');
    if (permanent == null)
        permanent = true;
    else
        permanent = Boolean.parse(permanent);
    if (!permanent || gName.split(':').length > 1)
        com.ivp.secm.api.intermediate.SMRefAttributeAddition.GetDetailEntityAttributes(entityID, successGetDetailEntityAttributes, failureGetReferredAttribCallBack, entityID);
    else {
        ddlLegAttributes.disabled = true;
        SecMasterJSCommon.SMSCommons.getTargetTableRow(ddlLegAttributes).style.display = 'none';
    }
}

function successGetDetailEntityAttributes(result, args) {
    var dictionaryData = result;
    var ddlToAddOptions = $get(ddl2);
    var length = ddlToAddOptions.options.length;
    for (var i = 0; i < length; i++) {
        ddlToAddOptions.removeChild(ddlToAddOptions.options[0]);
    }
    var option = null;
    var permanent = ddlToAddOptions.getAttribute('permanent');
    if (permanent == null)
        permanent = true;
    else
        permanent = Boolean.parse(permanent);
    //ADD this only in case of !permanent
    if (!permanent) {
        option = document.createElement('option');
        option.text = "--Select One--";
        //option.innerText = "--Select One--";
        option.value = "-1";
        ddlToAddOptions.add(option, 0);
    }
    else {
        //$get(ddls).disabled = true;
    }
    if (dictionaryData.length > 0) {
        for (var i = 0; i < dictionaryData.length; i++) {
            if (!permanent || dictionaryData[i].Key.split('-')[0] == gName.split(':')[1].split('-')[0]) {
                option = document.createElement('option');
                option.text = dictionaryData[i].Value;
                //option.innerText = dictionaryData[i].Value;
                option.value = dictionaryData[i].Key;
                ddlToAddOptions.add(option);
            }
        }
        if (gName != '' && gName.split(':').length > 1) {
            var detailAttributeId = gName.split(':')[1];
            ddlToAddOptions.value = detailAttributeId;
        }
        else {
            ddlToAddOptions.value = "-1";
        }
        ddlToAddOptions.disabled = false;
        SecMasterJSCommon.SMSCommons.getTargetTableRow(ddlToAddOptions).style.display = '';
    }
    else {
        ddlToAddOptions.disabled = true;
        SecMasterJSCommon.SMSCommons.getTargetTableRow(ddlToAddOptions).style.display = 'none';

    }
    gName = '';
}

function failureGetReferredAttribCallBack(result, args) {
    alert(result.get_message());
}

/*************************************************************************************************
Function Name     : PutDataInhiddenfield 
Author            : Bhavya Jaitly
Modified By       : 
Description       : Used Validate dropdowns and textbox and then store dropdown value in hiddenfield
Page used in      : SectypeLegDetails
Parameters        : 1) txtbox Id , 2) entitydropdown , 3)hiddenfield , 4) Error Label
Modification Log  : 
*************************************************************************************************/
function PutDataInhiddenfield(txtAttr, ddlId, hdnEntity, labelError, divRTagUpdate2) {
    $('#hdnTagInfo').val($('#' + divRTagUpdate2).find('.hdnTag').val());

    if (document.getElementById(ddlId).options[document.getElementById(ddlId).selectedIndex].value == "-1") {
        document.getElementById(labelError).innerHTML = '* Please select attribute';
        return false;
    }
    else if (trim(GetObject(txtAttr).value) == '') {
        GetObject(labelError).innerHTML = '● Please enter Attribute Name';
        return false;
    }
    else {
        document.getElementById(hdnEntity).value = trim(document.getElementById(ddlId)[GetObject(ddlId).selectedIndex].innerText);
        //Save new attrib values
        saveRefDisplayAttributedetails(txtAttr, 'sm_ref_saved_update_dropdown', 'chkBoxRMandatoryUpdate2', 'ADD');
        return true;
    }

}
//function SetRefDataType(hdn,ddlAttributeNames) {
//document.getElementById(hdn).value= document.getElementById(ddlAttributeNames)[GetObject(ddlAttributeNames).selectedIndex].innerText;
//}

function MoveUpDown(id, direction) {
    var SibilingToSwitch;
    var ListView = document.getElementById(id);
    if (ListView.options.length > 0) {
        var selectedIndex = ListView.selectedIndex;
        //direction 1 is +ve Y axis, 0 in negative Y axis
        if (direction == 1) {
            SibilingToSwitch = ListView[selectedIndex].previousSibling;
            if (SibilingToSwitch != null && SibilingToSwitch.nodeName == "#text")
                SibilingToSwitch = ListView[selectedIndex].previousSibling.previousSibling;
        }
        else {

            SibilingToSwitch = ListView[selectedIndex].nextSibling;
            if (SibilingToSwitch != null && SibilingToSwitch.nodeName == "#text")
                SibilingToSwitch = ListView[selectedIndex].nextSibling.nextSibling;
        }
        //            if(SibilingToSwitch != null)
        //                ListView[selectedIndex].swapNode(SibilingToSwitch);
        if (SibilingToSwitch != null) {
            SibilingToSwitch.selected = true
            tempValue = ListView[selectedIndex].value;
            tempText = ListView[selectedIndex].text;
            ListView[selectedIndex].value = SibilingToSwitch.value;
            ListView[selectedIndex].text = SibilingToSwitch.text;
            SibilingToSwitch.value = tempValue;
            SibilingToSwitch.text = tempText;
        }
    }


    //ListView.
}

function BuildAttribList(lstID, templateId, ruletype, modalattr) {
    var lstbox = document.getElementById(lstID);
    var hdn = "";
    var count = lstbox.options.length;
    if (count > 0) {
        for (var i = 0; i < count; i++) {
            hdn += lstbox.options[i].value + ",";
        }
        com.ivp.secm.ui.Service.SaveAttributeRulePriority(hdn, templateId, ruletype);
    }
    $find(modalattr).hide();
}


/*************************************************************************************************
Function Name     : ClearDDL
Author            : Shreyansh Agarwal
Modified By       : 
Description       : Clear DDL except option[0]
Page used in      : SectypeLegDetails / SectypeMasterDetails
Parameters        : 1) ddl id
Modification Log  : 
*************************************************************************************************/

function ClearDDL(ddlID) {
    var objDDL = $get(ddlID);
    for (; objDDL.options.length > 1;) {
        objDDL.removeChild(objDDL.options[1]);
    }
}

//var hdnDefaultId = null;
function attachHandlerForDefaultManagementGrid(gridId, btnDeleteDefaultNoId) {
    var grid = document.getElementById(gridId);
    //hdnDefaultId = document.getElementById(hdnDefaultIdId);
    var btnDeleteDefaultNo = document.getElementById(btnDeleteDefaultNoId);
    if (grid != null) {
        $(grid).click(onClickGridDefaults);
        //grid.attachEvent('onclick', onClickGridDefaults);
    }
    if (btnDeleteDefaultNo != null) {
        $(btnDeleteDefaultNo).click(onClickDefaultDeleteNo);
        //btnDeleteDefaultNo.attachEvent('onclick', onClickDefaultDeleteNo);
    }
}
function onClickGridDefaults() {
    var target = event.srcElement;
    if (SecMasterJSCommon.SMSCommons.contains(target.id, 'imgRemove')) {
        var row = com.ivp.rad.rscriptutils.RSCommonScripts.findControl(target, 'TR');
        var firstCell = row.cells[0];
        var hdnGridDefaultId = firstCell.children[1];
        var hdnGridDefaultName = row.children[0];
        hdnDefaultId.value = hdnGridDefaultId.value;
        hdnDefaultName.value = hdnGridDefaultName.innerText;
        $find('modalDeleteDefault').show();
        event.returnValue = false;
        return false;
    }
    //if (SecMasterJSCommon.SMSCommons.contains(target.id, 'imgView')) {
    //    var row = com.ivp.rad.rscriptutils.RSCommonScripts.findControl(target, 'TR');
    //    var firstCell = row.cells[0];
    //    var hdnGridDefaultId = firstCell.children[1];
    //    var hdnGridDefaultName = row.children[0];
    //    var hdnGridsectypeId = firstCell.children[3];
    //    hdnDefaultId.value = hdnGridDefaultId.value;
    //    hdnDefaultName.value = hdnGridDefaultName.innerText;
    //    hdnGridsectypeId = hdnGridsectypeId.value;
    //    var path = window.location.protocol + '//' + window.location.host;
    //    var pathname = window.location.pathname.split('/');
    //    $.each(pathname, function (ii, ee) {
    //        if ((ii !== 0) && (ii !== pathname.length - 1))
    //            path = path + '/' + ee;
    //    });
    //    var x = 123;
    //    $('.InnerFrame[name=TabsInnerIframe1]').attr('src', path + '/SMCreateUpdateSecurityNew.aspx?identifier=CreateSecurityNew&pageType=Defaults&sectypeId=' + hdnGridsectypeId + '&defaultId=' + hdnDefaultId.value + '&OpenInDefaults=1');
    //    $('#container1').css('display', 'block');
    //    return false;
    //}
}
function onClickDefaultDeleteNo() {
    $find('modalDeleteDefault').hide();
    hdnDefaultId.value = '';
    hdnDefaultName.value = '';
    event.returnValue = false;
    return false;
}

/*Edited By Sahil*/
function DeleteSectype(contextMenuId, gridId, btnPageRefresh, userName, iframePopupId, modalPopupId) {
    //onUpdating();
    //var rowId = parseInt(document.getElementById(contextMenuId + "___rowId").value);
    //var secTypeId = document.getElementById(gridId).rows[rowId].getAttributeNode('sectype_id').value;
    //var secTypeName = document.getElementById(gridId).rows[rowId].getAttributeNode('sectype_name').value;
    var secTypeId = $('[id$="hdnSectypeId"]').val();
    var secTypeName = $('[id$="hdnSectypeName"]').val();
    var url = 'SMSectypeDeletion.aspx?sectypeId=' + secTypeId + '&secTypeName=' + secTypeName + '&buttonId=' + btnPageRefresh + '&userName=' + userName;
    //var window_dimensions = 'toolbars=no,menubar=no,location=no,scrollbars=yes,resizable=yes,status=yes,height=' + eval('screen.availHeight - 50') + ',width=' + eval('screen.availWidth') + ',top=0, left=0';
    // $('[id$="' + modalPopupId + '"]').show();
    //$find(modalPopupId).show();
    document.getElementById(iframePopupId).src = encodeURI(url);
    return false;
}

function DeleteSectypePopUp(contextMenuId, gridId, userName) {
    var rowId = parseInt(document.getElementById(contextMenuId + "___rowId").value);
    var secTypeId = document.getElementById(gridId).rows[rowId].getAttributeNode('sectype_id').value;
    var secTypeName = document.getElementById(gridId).rows[rowId].getAttributeNode('sectype_name').value;

    $('[id$="hdnSectypeId"]').val(secTypeId);
    $('[id$="hdnSectypeName"]').val(secTypeName);

    $('[id$="hdnButton"]').trigger("click");

    return false;
}

function DeleteAttributePopUp(attributeId, attributeName, secTypeName) {
    $('[id$="hdnIdAttribute"]').val(attributeId);
    $('[id$="hdnSectypeName"]').val(secTypeName);
    $('[id$="hdnAttributeName"]').val(attributeName);

    $('[id$="hdnButton"]').trigger("click");

    return false;
}


/*Edited By Sahil*/
function DeleteAttribute(attributeId, attributeName, secTypeName, isCommon, btnPageRefreshId, userName, iframePopupId, modalPopupId) {
    var url = 'SMSectypeAttributeDeletion.aspx?attributeId=' + attributeId + '&attributeName=' + attributeName + '&secTypeName=' + secTypeName
    + '&isCommon=' + isCommon + '&buttonId=' + btnPageRefreshId + '&userName=' + userName;
    if (iframePopupId == undefined && modalPopupId == undefined) {
        onUpdating();
        var window_dimensions = 'toolbars=no,menubar=no,location=no,scrollbars=yes,resizable=yes,status=yes,height=' + eval('screen.availHeight - 50') + ',width=' + eval('screen.availWidth') + ',top=0, left=0';
        window.open(url, '', window_dimensions);
    }
    else {
        //$find(modalPopupId).show();
        document.getElementById(iframePopupId).src = encodeURI(url);
    }
    return false;
}

function OpenUnderlierDeleteWindow(hdnUnderlierAttributeId, btnPageRefreshId, userName) {
    DeleteAttribute(document.getElementById(hdnUnderlierAttributeId).value, btnPageRefreshId, userName);
    return false;
}

function onClickChkUnderlyingUpdateLeg(hdnUpdateLegStateId, trPrimaryFieldId, ddlPrimaryFieldSavedLegId, ddlDefaultUnderlierUpdateLegId, hdnUnderlierSectypeSelectedIndexId, hdnPrimarySelectedIndexId) {
    var HdnUpdateLegState = document.getElementById(hdnUpdateLegStateId);
    var TrPrimaryField = document.getElementById(trPrimaryFieldId);
    var DdlPrimaryFieldSavedLeg = document.getElementById(ddlPrimaryFieldSavedLegId);
    var DdlDefaultUnderlierUpdateLeg = document.getElementById(ddlDefaultUnderlierUpdateLegId);
    var HdnUnderlierSectypeSelectedIndex = document.getElementById(hdnUnderlierSectypeSelectedIndexId);
    var HdnPrimarySelectedIndex = document.getElementById(hdnPrimarySelectedIndexId);
    var target = null;
    if (event.srcElement.tagName.toUpperCase() == 'INPUT' && event.srcElement.type.toUpperCase() == 'CHECKBOX' && !event.srcElement.disabled) {
        target = event.srcElement;
    }
    if (target != null) {
        if (target.checked)
            switch (HdnUpdateLegState.value) {
                case '0':
                    TrPrimaryField.style.display = '';
                    DdlPrimaryFieldSavedLeg.disabled = true;
                    DdlPrimaryFieldSavedLeg.selectedIndex = 0;
                    DdlDefaultUnderlierUpdateLeg.style.display = '';
                    DdlDefaultUnderlierUpdateLeg.selectedIndex = 0;
                    break;
                case '1':
                    //No Action
                    break;
                case '2':
                    TrPrimaryField.style.display = '';
                    DdlPrimaryFieldSavedLeg.disabled = true;
                    DdlPrimaryFieldSavedLeg.selectedIndex = 0;
                    DdlDefaultUnderlierUpdateLeg.style.display = '';
                    DdlDefaultUnderlierUpdateLeg.selectedIndex = parseInt(HdnUnderlierSectypeSelectedIndex.value);
                    break;
                case '3':
                    TrPrimaryField.style.display = 'none';
                    DdlPrimaryFieldSavedLeg.disabled = true;
                    DdlPrimaryFieldSavedLeg.selectedIndex = 0;
                    DdlDefaultUnderlierUpdateLeg.style.display = '';
                    DdlDefaultUnderlierUpdateLeg.selectedIndex = 0;
                    break;
                case '4':
                    TrPrimaryField.style.display = 'none';
                    DdlPrimaryFieldSavedLeg.disabled = true;
                    DdlPrimaryFieldSavedLeg.selectedIndex = 0;
                    DdlDefaultUnderlierUpdateLeg.style.display = '';
                    DdlDefaultUnderlierUpdateLeg.selectedIndex = parseInt(HdnUnderlierSectypeSelectedIndex.value);;
                    break;
                default: break;
            }
        else
            switch (HdnUpdateLegState.value) {
                case '0':
                    TrPrimaryField.style.display = '';
                    DdlPrimaryFieldSavedLeg.disabled = false;
                    DdlPrimaryFieldSavedLeg.selectedIndex = 0;
                    DdlDefaultUnderlierUpdateLeg.style.display = 'none';
                    DdlDefaultUnderlierUpdateLeg.selectedIndex = 0;
                    break;
                case '1':
                    //No Action
                    break;
                case '2':
                    TrPrimaryField.style.display = '';
                    DdlPrimaryFieldSavedLeg.disabled = true;
                    DdlPrimaryFieldSavedLeg.selectedIndex = 0;
                    DdlDefaultUnderlierUpdateLeg.style.display = 'none';
                    DdlDefaultUnderlierUpdateLeg.selectedIndex = 0;
                    break;
                case '3':
                    TrPrimaryField.style.display = 'none';
                    DdlPrimaryFieldSavedLeg.disabled = true;
                    DdlPrimaryFieldSavedLeg.selectedIndex = 0;
                    DdlDefaultUnderlierUpdateLeg.style.display = 'none';
                    DdlDefaultUnderlierUpdateLeg.selectedIndex = 0;
                    break;
                case '4':
                    TrPrimaryField.style.display = 'none';
                    DdlPrimaryFieldSavedLeg.disabled = true;
                    DdlPrimaryFieldSavedLeg.selectedIndex = 0;
                    DdlDefaultUnderlierUpdateLeg.style.display = 'none';
                    DdlDefaultUnderlierUpdateLeg.selectedIndex = 0;
                    break;
                default: break;
            }
    }
}

function onChangeDdlPrimaryFieldSavedLeg(hdnUpdateLegStateId, trPrimaryFieldId, chkUnderlyingUpdateLegId, ddlDefaultUnderlierUpdateLegId, hdnUnderlierSectypeSelectedIndexId, hdnPrimarySelectedIndexId) {
    var HdnUpdateLegState = document.getElementById(hdnUpdateLegStateId);
    var TrPrimaryField = document.getElementById(trPrimaryFieldId);
    var ChkUnderlyingUpdateLeg = document.getElementById(chkUnderlyingUpdateLegId);
    var DdlDefaultUnderlierUpdateLeg = document.getElementById(ddlDefaultUnderlierUpdateLegId);
    var HdnUnderlierSectypeSelectedIndex = document.getElementById(hdnUnderlierSectypeSelectedIndexId);
    var HdnPrimarySelectedIndex = document.getElementById(hdnPrimarySelectedIndexId);
    var target = event.srcElement;
    if (target.tagName.toUpperCase() == 'SELECT') {
        if (target.selectedIndex == 0)
            switch (HdnUpdateLegState.value) {
                case '0':
                    ChkUnderlyingUpdateLeg.disabled = false;
                    ChkUnderlyingUpdateLeg.parentNode.disabled = false;
                    ChkUnderlyingUpdateLeg.checked = false;
                    DdlDefaultUnderlierUpdateLeg.style.display = 'none';
                    DdlDefaultUnderlierUpdateLeg.selectedIndex = 0;
                    break;
                case '1':
                    ChkUnderlyingUpdateLeg.disabled = true;
                    ChkUnderlyingUpdateLeg.parentNode.disabled = true;
                    ChkUnderlyingUpdateLeg.checked = false;
                    DdlDefaultUnderlierUpdateLeg.style.display = 'none';
                    DdlDefaultUnderlierUpdateLeg.selectedIndex = 0;
                    break;
                case '2':
                case '3':
                case '4':
                    //No Action
                    break;
                default: break;
            }
        else
            switch (HdnUpdateLegState.value) {
                case '0':
                    ChkUnderlyingUpdateLeg.disabled = true;
                    ChkUnderlyingUpdateLeg.parentNode.disabled = true;
                    ChkUnderlyingUpdateLeg.checked = false;
                    DdlDefaultUnderlierUpdateLeg.style.display = 'none';
                    DdlDefaultUnderlierUpdateLeg.selectedIndex = 0;
                    break;
                case '1':
                    ChkUnderlyingUpdateLeg.disabled = true;
                    ChkUnderlyingUpdateLeg.parentNode.disabled = true;
                    ChkUnderlyingUpdateLeg.checked = false;
                    DdlDefaultUnderlierUpdateLeg.style.display = 'none';
                    DdlDefaultUnderlierUpdateLeg.selectedIndex = 0;
                    break;
                case '2':
                case '3':
                case '4':
                    //No Action
                    break;
                default: break;
            }
    }
}

function reloadMainWindow(identifier) {
    window.location = 'SMHome.aspx?identifier=' + identifier
    return false;
}


function SetTags(divTag, hdnTag) {
    $('#' + hdnTag).val($('#' + divTag).find('.hdnTag').val());
}

function bindCommonAttributeEntityDDL(ddlEntityType, divRTag) {
    $(GetObject(ddlEntityType)).unbind('change').bind('change', function (e) {
        $('#hdnRTagsInfo').val($('#' + divRTag).find('.hdnTag').val());
    });
}

function setHiddenVariablesForSectypeSetup(hdnTagInfo, divTag, hdnUsersInfo, hdnGroupsInfo) {
    $('#' + hdnTagInfo).val($('#' + divTag).find('.hdnTag').val());
    var objuser = altshut.getSelectedOption($('#divUsers'));
    var allUsers = altshut.getAllOptions($('#divUsers'));
    var userValue = '';
    var groupValue = '';

    if (objuser.length !== allUsers.length) {
        $.each(objuser, function (ii, ee) {
            userValue += ee.value + ',';
        });
    }
    var objgroup = altshut.getSelectedOption($('#divGroups'));
    var allGroups = altshut.getAllOptions($('#divGroups'));

    if (objgroup.length !== allGroups.length) {
        $.each(objgroup, function (ii, ee) {
            groupValue += ee.value + ',';
        });
    }
    $('#' + hdnUsersInfo).val(userValue.substr(0, userValue.length - 1));
    $('#' + hdnGroupsInfo).val(groupValue.substr(0, groupValue.length - 1));

    var options = $('#ddl_creationDateType').find('option');
    var selected;
    $.each(options, function (ii, ee) {
        if ($(ee).is(':selected')) {
            selected = $(ee).html().trim();
            return;
        }
    });
    $('[id$=hdnCreationDateType]').val(selected);
}

function creationDateChangeHandler(shortDateFormat) {
    $('#ddl_creationDateType').on('change', function (e) {
        e.stopPropagation();
        var options = $(this).find('option');
        var selected;
        var tdDate = $('[id$=tdDate]');
        var tdDateValue = $('[id$=tdDateValue]');
        var tdDays = $('[id$=tdDays]');
        var tdDaysValue = $('[id$=tdDaysValue]');

        $.each(options, function (ii, ee) {
            if ($(ee).is(':selected')) {
                selected = $(ee).html().trim();
                return;
            }
        });

        if (selected === 'Custom') {
            var txtDate = $('[id$=txt_customDate]');
            txtDate.val('');
            tdDate.show();
            tdDateValue.show();
            tdDays.hide();
            tdDaysValue.hide();
        }
        else if (selected === 'T-N days') {
            tdDays.show();
            tdDaysValue.show();
            tdDate.hide();
            tdDateValue.hide();
        }
        else {
            tdDate.hide();
            tdDateValue.hide();

            tdDays.hide();
            tdDaysValue.hide();
        }
    });
}
/* Edited by Lakshya Bhardwaj*/
function createSMSelectDropDown(dropDownId, smdata, showSearch, width, callback, heading, onChangeHandler, selectedItems, isMulti, multiText, headerWidth) {
    var obj = {};
    dropDownId = $("#" + dropDownId);
    obj.container = dropDownId;
    obj.id = dropDownId.attr("id");
    if (heading !== null) {
        obj.heading = heading;
    }
    obj.data = smdata;
    if (showSearch) {
        obj.showSearch = true;
    }
    if (isMulti) {
        obj.isMultiSelect = true;
        obj.text = multiText;
    }
    if (selectedItems !== undefined && selectedItems.length !== 0) {
        obj.selectedItems = selectedItems;
    }
    obj.isRunningText = false;
    obj.ready = function (e) {
        e.width(width + "px");
        $(e).css({ 'border': '1px solid #cecece', 'height': '22px' });
        if (typeof onChangeHandler === "function") {
            e.on('change', function (ee) {
                onChangeHandler(ee);
            });
        }
    }
    smselect.create(obj);

    $("#smselect_" + dropDownId.attr("id")).find(".smselectrun2").css('width', headerWidth);
    //$("#smselect_" + dropDownId.attr("id")).find(".smselectanchorcontainer2").css('width', '86%');
    $("#smselect_" + dropDownId.attr("id")).find(".smselectimage").css('color', '#666666');
    $("#smselect_" + dropDownId.attr("id")).find(".smselecttext").css('width', '97%');
    $("#smselect_" + dropDownId.attr("id")).find(".smselectcon").css('width', '100%');
    $("#smselect_" + dropDownId.attr("id")).find(".smselectsectionbar").html('Reference Display Attributes');



    if (typeof callback === "function") {
        callback();
    }
};

function getRefDisplayAttributedetails(AttrName, refDropdownId, mandatoryid, smdata, fetchFromDB) {
    var Guid = $('#hdnGuid').val().trim();
    var sectypeName = null;
    var legName = null;

    if (AttrName == "null")
        AttrName = null;

    if (refDropdownId == "null")
        refDropdownId = null;

    if (mandatoryid == "null")
        mandatoryid = null;

    if (smdata == "null")
        smdata = null;

    if (typeof $('#hdnLegName').val() != 'undefined' && $('#hdnLegName').val() != "") {
        legName = $('#hdnLegName').val().trim();
    }

    if (typeof $('#hdnSectypeName').val() != 'undefined' && $('#hdnSectypeName').val() != "") {
        sectypeName = $('#hdnSectypeName').val().trim();
    }

    if (refDropdownId != null) {
        if (smdata == null) {
            smdata = [{ "text": "", "value": "" }];
        }
        var finalData = [{ 'text': 'Reference Display Attributes', 'options': smdata }];
        createSMSelectDropDown(refDropdownId, finalData, true, '248px', null, null, null, [], true, 'Reference Display Attributes', '248px');
    }
    if (AttrName != null && typeof fetchFromDB == "undefined") {
        getReferenceMandatoryData(sectypeName, legName, AttrName, Guid).then(function (data) {
            if (data != null) {
                var obj = JSON.parse(data);
                if ($("[id$='" + mandatoryid + "']") != null && ($("[id$='" + mandatoryid + "']").length !== 0)) {
                    $("[id$='" + mandatoryid + "']").attr('checked', obj.IsMandatory);
                    $('#hdnoldMandatoryData').val(obj.IsMandatory);
                }
                if (refDropdownId != null) {
                    for (var item in obj.RefDisplayAttrs) {
                        smselect.setOptionByValue($("#smselect_" + refDropdownId), obj.RefDisplayAttrs[item]);
                    }
                }
            }
        });
    }
    // update from DB
    if (AttrName != null && typeof fetchFromDB != "undefined" && fetchFromDB == '1') {
        getReferenceMandatoryData(sectypeName, legName, AttrName, null).then(function (data) {
            if (data != null) {
                var obj = JSON.parse(data);
                if ($("[id$='" + mandatoryid + "']") != null && ($("[id$='" + mandatoryid + "']").length !== 0)) {
                    $("[id$='" + mandatoryid + "']").attr('checked', obj.IsMandatory);
                    // storing the old state for mandatory attribute
                    $('#hdnoldMandatoryData').val(obj.IsMandatory);
                }
                if (refDropdownId != null) {
                    for (var item in obj.RefDisplayAttrs) {
                        smselect.setOptionByValue($("#smselect_" + refDropdownId), obj.RefDisplayAttrs[item]);
                    }
                }
            }
        });
    }
}

function getReferenceMandatoryData(sectypeName, legName, AttrName, Guid) {
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    serviceLocation = "/BaseUserControls/Service/Service.asmx";

    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: path + serviceLocation + "/getRefDisplayAttributeDetails",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ "sectypeName": sectypeName, "legName": legName, "AttrName": AttrName, "Guid": Guid }),
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
}

function saveRefDisplayAttributedetails(attrNameId, refDropdownId, mandatoryId, action, lblDisplayNameError, DeleteAttrName) {

    var AttrName = null;
    var AttrOldName = null;
    var Guid = $('#hdnGuid').val().trim();
    var sectypeName = null;
    var legName = null;
    var RefDisplayAttrs = [];
    var flag = true;    //checks for attr to be mandatory or not

    if (typeof DeleteAttrName == 'undefined')
        AttrName = $("[id$='" + attrNameId + "']").val().trim();
    else if (DeleteAttrName != 'undefined' && action.toUpperCase() == 'DELETE')
        AttrName = DeleteAttrName;

    if (refDropdownId != null) {
        var refdisplaynames = smselect.getSelectedOption($("#smselect_" + refDropdownId));

        if (refdisplaynames.length >= 1) {
            for (var i = 0; i < refdisplaynames.length; i++) {
                RefDisplayAttrs.push(refdisplaynames[i].value);
            }
        }
    }
    if (mandatoryId != null) {
        var IsMandatory = $("[id$='" + mandatoryId + "']").prop('checked');
    }

    if (typeof $('#hdnLegName').val() != 'undefined' && $('#hdnLegName').val() != "") {
        legName = $('#hdnLegName').val().trim();
    }

    if (typeof $('#hdnSectypeName').val() != 'undefined' && $('#hdnSectypeName').val() != "") {
        sectypeName = $('#hdnSectypeName').val().trim();
    }
    if (typeof $("[id$='hdnAttributeName']").val() != 'undefined' && $("[id$='hdnAttributeName']").val() != "") {
        AttrOldName = $("[id$='hdnAttributeName']").val().trim();
    }

    if (AttrName != null) {
        saveReferenceMandatoryData(sectypeName, legName, AttrOldName, AttrName, Guid, RefDisplayAttrs, IsMandatory, action).then(function (data) {
            if (data != null) {
                if (data.indexOf("FAILURE") !== -1) {
                    $("div[class$='sm_ref_display_attr_show_error']").css('display', 'inline-block');
                    $("div[class$='sm_ref_display_attr_show_error']").text('Attribute with same name already exists.');
                    return false;
                }
            }
        });
    }
};

function saveReferenceMandatoryData(sectypeName, legName, AttrOldName, AttrName, Guid, RefDisplayAttrs, IsMandatory, action) {
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    serviceLocation = "/BaseUserControls/Service/Service.asmx";

    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: path + serviceLocation + "/savedRefDisplayAttributeDetails",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ "sectypeName": sectypeName, "legName": legName, "AttrOldName": AttrOldName, "AttrName": AttrName, "Guid": Guid, "RefDisplayAttrs": RefDisplayAttrs, "IsMandatory": IsMandatory, "action": action }),
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
}



function callRefAttr(entityTypeID) {
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    serviceLocation = "/BaseUserControls/Service/Service.asmx";

    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: path + serviceLocation + "/GetEntityAttribute",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ "entityTypeID": entityTypeID }),
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
}

function sm_renderTemplateErrorInfo(dataToRender) {
    data = dataToRender;
    flag = false;

    for (var j in data[0]) {
        if (j == "SectypeName" && data[0][j] == null) {
            flag = true;
        }
    }
    if (flag)
        data.forEach(function (item) { delete item.SectypeName });

    var html = '<div class="sm_renderTemplateErrorHeaderDiv"><table style="width:100%">';
    html += '<thead><tr class="header">';
    for (var j in data[0]) {
        if (j == "SectypeName")
            j = "Security Type";
        else if (j == "TemplateName")
            j = "Template Name";
        else if (j == "ToShow")
            j = "To Show";
        else if (j == "isReadOnly")
            j = "Is Read Only";

        html += '<th style="width:25%">' + j + '</th>';
    }
    html += '</tr></thead></table></div><div class="sm_renderTemplateErrorBodyDiv"><table style="width:100%;margin-left: 5px;"><tbody>';

    for (var i = 0; i < data.length; i++) {
        html += '<tr>';
        for (var j in data[i]) {
            html += '<td ">' + data[i][j] + '</td>';
        }
    }
    html += '</tbody></table></div>';
    var height = $(window).height();
    var width = $(window).width();
    $('#sm_renderdisableDiv').css({
        'display': 'block',
        'height': height,
        'width': width
    });
    document.getElementById('sm_renderTemplateTableContainer').innerHTML = html;
    $('#sm_renderTemplateErrorInfoContainer').css('display', 'inline-block');
}