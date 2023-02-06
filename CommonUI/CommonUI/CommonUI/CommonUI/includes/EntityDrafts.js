Type.registerNamespace('SecMasterJSPage');
SecMasterJSPage.EntityDraftsHelper = function SecMasterJSPage_EntityDraftsHelper() {
    SecMasterJSPage.EntityDraftsHelper.initializeBase(this);
}
SecMasterJSPage.EntityDraftsHelper.prototype = {
    _securityDraftsControlInfo: null,

    get_securityDraftsControlInfo: function get_securityDraftsControlInfo() {
        return this._securityDraftsControlInfo;
    },
    set_securityDraftsControlInfo: function set_securityDraftsControlInfo(value) {
        this._securityDraftsControlInfo = value;
        return value;
    },

    dispose: function SecMasterJSPage_EntityDraftsHelperDispose() {

        SecMasterJSPage.EntityDraftsHelper.callBaseMethod(this, 'dispose');
    },
    updated: function SecMasterJSPage_EntityDraftsHelperUpdated() {
        SecMasterJSPage.EntityDraftsHelper.callBaseMethod(this, 'updated');
        //var grid = $find($('[id$=xlGrid]')[0].id)
        var grid = $find(gridId);
        if (grid != null) {
            var serviceCompleted = Function.createDelegate(this, _serviceCompleted);
            grid.eventHandlerManager.addServiceCompletedEventHandler(serviceCompleted);
        }
    }
}
//function EntityDraftsMethodsControls(controlIdInfo) {
//    this._controlIdInfo = controlIdInfo;
//}
//SMSRealTimeStatusControls.prototype = {
//    _controlIdInfo: null,

//    _gridSecurityDrafts: null,
//    GridSecurityDrafts: function get_gridSecurityDrafts() {
//        if (this._gridSecurityDrafts == null || this._gridSecurityDrafts.length == 0) {
//            this._gridSecurityDrafts = $('#xlGrid');
//        }
//        return this._gridExternalSystemStatus1;
//    }
//}

function _serviceCompleted(info) {
    var objArr = [];

    if (info.eventType === com.ivp.rad.controls.xlgrid.scripts.EventType.evenT_GET_CHECKED_ROWS) {
        RowCheckedDetails = Sys.Serialization.JavaScriptSerializer.deserialize((info).output);
        for (var i = 0; i < RowCheckedDetails.length; i++) {
            var rowData = RowCheckedDetails[i].split('¦');
            var obj = new Object();
            obj.secId = rowData[0];
            obj.effectiveDate = rowData[1];
            objArr.push(obj);
        }
        $('#' + contextMenuId).trigger('click', [contextMenuOption, true, objArr]);
    }
}

function EntityDraftsMethods() {
}
EntityDraftsMethods.prototype = {
    _securityInfo: null,
     _controls: null,
    onClickGridSecurityDrafts: function onClickGridSecurityDrafts(e) {
        var target = $(e.target);

        if (target.prop('id') != null) {
            if ((target.prop('id').indexOf('lblSecId') != -1)) {
                if (target.prop('tagName').toUpperCase() == 'SPAN') {
                    var row = $(getParentElement(target[0], 'TR'));
                    var securityId = row.attr('sec_id');
                    var isSecurityEditable = EntityDraftsMethods.prototype._securityInfo.isSecurityEditable;
                    if (isSecurityEditable != null && isSecurityEditable != undefined && isSecurityEditable == "true") {
                        OpenNewTabs(true, securityId, "", "", true, true);
                    }
                    else if (isSecurityEditable != null && isSecurityEditable != undefined && isSecurityEditable == "false") {
                        OpenNewTabs(false, securityId, "", "", true, true);
                    }
                    else if (isSecurityEditable != null && isSecurityEditable != undefined && isSecurityEditable == "false" && isViewPrivilege == "false") {
                        return false;
                    }

                }
            }
        }
    }
}

function EntityDrafts(info, securityInfo) {
    EntityDraftsMethods.prototype._securityInfo = eval("(" + securityInfo + ")");
    var json = JSON.parse(info);
    gridId = json.grid;
    contextMenuId = json.ContextMenu;
    viewId = json.View;
    discardId = json.Discard;
    hiddenSecId = json.hiddenSecId;
    btnPageRefresh = json.btnPageRefresh;
    $('#' + contextMenuId).click(contextHandler);
    function createHandlers()
    {
        if ($('[id$=xlGrid]').length != 0)
            $('[id$=xlGrid]').unbind('click').click(EntityDraftsMethods.prototype.onClickGridSecurityDrafts);
    }
    createHandlers();

}

SecMasterJSPage.EntityDraftsHelper.registerClass('SecMasterJSPage.EntityDraftsHelper', Sys.Component);
if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();

function contextHandler(e, choice, flag, objArr) {
    e.stopPropagation();
    if ((typeof choice !== 'undefined') && (choice === 'discard')) {
        if (flag) {
            if (objArr.length > 0) {
                var secIds = '';
                var count = 0;
                $.each(objArr, function (ii, ee) {
                    secIds = secIds + ee.secId + 'ž';
                    count++;
                });
                $('#' + hiddenSecId).val(secIds);

                $('[id *= "LabelDeleteWarning"]').text("Are you sure you want to Discard " + count + " Draft(s)");
                var panel = $('#panelWarning');
                $('#Div_Overlay').show();
                $(panel).show();
            }

            else {
                var rowIndex = $('#' + contextMenuId + '___rowId').val();
                var grid = $('#' + gridId + '_bodyDiv_Table');
                var row = $(grid[0].rows[rowIndex]);

                var key = row.attr('sec_id');
                if (typeof key !== 'undefined') {
                    $('#' + hiddenSecId).val(key.split('¦')[0]);

                    $('[id *= "LabelDeleteWarning"]').text("Are you sure you want to Discard 1 Draft");
                    var panel = $('#panelWarning');
                    $('#Div_Overlay').show();
                    $(panel).show();
                }
            }
        }
    }

    else if ((typeof choice !== 'undefined') && (choice === 'view')) {
        if (flag) {
            if ((objArr.length > 0) && (objArr.length < 11)) {
                $.each(objArr, function (ii, ee) {
                    SecMasterJSCommon.SMSCommons.openSecurity('true', ee.secId, ee.effectiveDate, true, true, false, btnPageRefresh, '', false, false, SecMasterJSCommon.SMCreateUpdateOpenByDefault.None, 0, null, 3);
                });
            }

            else if (objArr.length > 10) {
                $('[id *= "lblDeleteError"]').text("Cannot view more than 10 entities");
                var panel = $('[id *= "panelError"]');
                $(panel).css({ "top": "45%", "left": "35%", "z-index": "70000" });
                $('#Div_Overlay').show();
                $(panel).show();
            }

            else {
                var rowIndex = $('#' + contextMenuId + '___rowId').val();
                var grid = $('#' + gridId + '_bodyDiv_Table');
                var row = $(grid[0].rows[rowIndex]);

                var key = row.attr('sec_id');
                if (typeof key !== 'undefined') {
                    var keyArr = key.split('¦');
                    SecMasterJSCommon.SMSCommons.openSecurity('true', keyArr[0], keyArr[1], true, true, false, btnPageRefresh, '', false, false, SecMasterJSCommon.SMCreateUpdateOpenByDefault.None, 0, null, 3);
                }
            }
        }
    }

    else {
        if (e.target.value === 'View') {
            var grid = $find(gridId);
            grid.getCheckedRowKeys();
            contextMenuOption = 'view';
        }

        else if (e.target.value === 'Discard Draft') {
            var grid = $find(gridId);
            grid.getCheckedRowKeys();
            contextMenuOption = 'discard';
        }
    }
}

$(document).ready(function () {
    $('#btn_WarningOK').click(function (e) {
        e.stopPropagation();
        var panel = $('#panelWarning');

        $('#Div_Overlay').hide();
        $(panel).hide();
        $('[id *= "btn_Hidden"]').click();
    });
    $('[id *= "btn_WarningCancel"]').click(function (e) {
        e.stopPropagation();
        var panel = $('#panelWarning');
        $('#Div_Overlay').hide();
        $(panel).hide();
    });
    $('#Div_Overlay').click(function (e) {
        var panel = $('#panelWarning');
        $('[id *= "panelError"]').hide();
        $('#Div_Overlay').hide();
        $(panel).hide();
    });
    $('[id *= "btnErrorOK"]').click(function (e) {
        $('#Div_Overlay').hide();
    });
});

$(document).keyup(function (e) {
    if (e.keyCode == 27) {
        $('[id *= "panelError"]').hide();
        $('#Div_Overlay').hide();
        $('#panelWarning').hide();
    }
});
function OpenNewTabs(isEditMode, secId, AttrIdForHighlight, MultiInfoIdForHighlight, newWindow, isNewWindowMode) {
    //SecMasterJSCommon.SMSCommons.openWindowForSecurity(true, secId, true, true, SecMasterJSCommon.SMCreateUpdateOpenByDefault.None, 0, null);
    SecMasterJSCommon.SMSCommons.openWindowForDraftedSecurity(true, secId, true, true, SecMasterJSCommon.SMCreateUpdateOpenByDefault.None, 0, null,true);
}
