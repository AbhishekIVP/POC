var draftsStatus = {
    _path: "",
    _windowHeight: "",
    _windowWidth: "",
    _username: "",
    _moduleID: 0,
    _fromScreenName: "",
    _selectedRowsCount: "",
    _selectedSecuritiesOrEntitesFromGrid: "",
    _cultureShortDate: "",
    _cultureLongDate: "",
    _checkedRowsDraftsStatus: [],
    _prodName: "",
    _commonServiceLocation: "/BaseUserControls/Service/CommonService.svc",
    _isDraftsViewAllowedInSecM: false,
    _isDraftsViewAllowedInRefM: false,
    _moduleID_moduleName_map: [],
    _ajaxHitCounts: 0,
    _isUserSpecific : false,
    _sectypeList : "",
    _isFromDashboard: false,
    _deletePrivilege: false,
    _editPrivilege:false,
    privilegeList : {
        3: ['Edit Security Drafts', 'Delete Security Drafts'],
        6: ['RM - Drafts - Edit Entity', 'RM - Delete Drafts'],
        18: ['FM - Drafts - Edit Entity', 'FM - Delete Drafts'],
        20: ['PM - Drafts - Edit Entity', 'PM - Delete Drafts']
    },
    getPrivilegesForModules: function () {
        draftsStatus._deletePrivilege = false;
        draftsStatus._editPrivilege = false;
        $("#draftsStatusDeleteAction").css('visibility', 'hidden');
        var params = {
            userName: draftsStatus._username,
            privilegeName: draftsStatus.privilegeList[draftsStatus._moduleID]
        }
        draftsStatus.CallCommonServiceMethod('CheckControlPrivilegeForUserMultiple', params, draftsStatus.OnSuccess_GetPrivileges, OnFailure, null, false);
    },
      
    CallCommonServiceMethod: function (methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
    callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    },

    OnSuccess_GetPrivileges: function(data){
        var privileges = data.d;
        debugger;
        //first is edit, second is for delete
        if (privileges != null && privileges.length == 2) {
            if (privileges[1].result){
                $("#draftsStatusDeleteAction").css('visibility', 'visible');
                draftsStatus._deletePrivilege = true;
            }
            if (privileges[0].result) {
                draftsStatus._editPrivilege = true;
            }
        }
    },
    //selectedRows: [],
    setPath: function () {
        var path = window.location.protocol + '//' + window.location.host;
        var pathname = window.location.pathname.split('/');

        $.each(pathname, function (ii, ee) {
            if ((ii !== 0) && (ii !== pathname.length - 1))
                path = path + '/' + ee;
        });

        draftsStatus._path = path;
    },

    setProductDraftsStatusScreen: function () {
        //TODO init function
    },

    getViewPrivileges: function () {
        //TODO
    },

    init: function () {
        //TODO
        draftsStatus.setPath();
        draftsStatus.initializeValues();

        var listofTabsToBindFunctionsWith = CommonModuleTabs.ModuleList();
        var prodName = draftsStatus._prodName;

        for (i in listofTabsToBindFunctionsWith) {
            item = listofTabsToBindFunctionsWith[i];
            draftsStatus._moduleID_moduleName_map[item.displayName.toLowerCase()] = item.moduleId;
            //draftsStatus._moduleID = item.moduleId;
            switch (item.displayName.toLowerCase().trim()) {
                case "securities":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: draftsStatus.secMasterInitDraftsStatusScreen });
                    //draftsStatus._moduleID_moduleName_map["item.displayName.toLowerCase()"] = item.moduleId;
                    break;
                case "refdata":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: draftsStatus.refMasterInitDraftsStatusScreen });
                    break;
                case "funds":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: draftsStatus.fundMasterInitDraftsStatusScreen });
                    break;
                case "parties":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: draftsStatus.partyMasterInitDraftsStatusScreen });
                    break;
            }
        }

    },

    initializeValues: function () {
        draftsStatus._username = os_serverSideValues.Username;
        draftsStatus._fromScreenName = os_serverSideValues.FromScreenName;
        draftsStatus._prodName = os_serverSideValues.ProductName;
        draftsStatus._windowHeight = $(window).height();
        draftsStatus._windowWidth = $(window).width();
        draftsStatus._cultureShortDate = os_serverSideValues.CultureShortDateFormat;
        draftsStatus._cultureLongDate = os_serverSideValues.CultureLongDateFormat;
        //draftsStatus._selectedSecuritiesOrEntitesFromGrid = draftsStatus._selectedSecIDsOrEntityCodesFromSearch;
        draftsStatus._moduleID = os_serverSideValues.ModuleID;
        draftsStatus._selectedProduct = os_serverSideValues.SelectedProduct;
        draftsStatus._isUserSpecific = os_serverSideValues.isUserSpecific;
        draftsStatus._sectypeList = os_serverSideValues.sectypeList;
        draftsStatus._isFromDashboard = os_serverSideValues.isFromDashboard;
    },
    initializeDraftsStatusScreen: function () {
        //TODO CommonModuletabs init changes
    },

    //Click handler for Secmaster
    secMasterInitDraftsStatusScreen: function (gridHeight) {
        draftsStatus._ajaxHitCounts = 0;
        draftsStatus.destroyGrid();
        draftsStatus._moduleID = draftsStatus._moduleID_moduleName_map["securities"];
        draftsStatus.getDraftsStatusGridSM(gridHeight);
        draftsStatus.createSecMHandlersDraftsStatusScreen();
        draftsStatus.getPrivilegesForModules();
    },
    //Click handler for Refmaster
    refMasterInitDraftsStatusScreen: function (gridHeight) {
        draftsStatus._ajaxHitCounts = 0;
        draftsStatus.destroyGrid();
        draftsStatus._moduleID = draftsStatus._moduleID_moduleName_map["refdata"];
        ////See later
        draftsStatus.getDraftsStatusGridRM(gridHeight);//, draftsStatus._moduleID_moduleName_map["refmaster"]);
        draftsStatus.createRefMHandlersDraftsStatusScreen();
        draftsStatus.getPrivilegesForModules();
    },

    fundMasterInitDraftsStatusScreen: function (gridHeight) {
        draftsStatus._ajaxHitCounts = 0;
        draftsStatus.destroyGrid();
        draftsStatus._moduleID = draftsStatus._moduleID_moduleName_map["funds"];
        ////See later
        draftsStatus.getDraftsStatusGridRM(gridHeight);//, draftsStatus._moduleID_moduleName_map["fundmaster"]);
        draftsStatus.createRefMHandlersDraftsStatusScreen();
        draftsStatus.getPrivilegesForModules();
    },
    partyMasterInitDraftsStatusScreen: function (gridHeight) {
        draftsStatus._ajaxHitCounts = 0;
        draftsStatus.destroyGrid();
        draftsStatus._moduleID = draftsStatus._moduleID_moduleName_map["parties"];
        ////See later
        draftsStatus.getDraftsStatusGridRM(gridHeight);//, draftsStatus._moduleID_moduleName_map["partymaster"]);
        draftsStatus.createRefMHandlersDraftsStatusScreen();
        draftsStatus.getPrivilegesForModules();
    },
    destroyGrid: function () {
        draftsStatus.controls.draftsStatusNeoGrid().html("");
    },

    createSecMHandlersDraftsStatusScreen: function () {
        if (draftsStatus.controls.draftsStatusNeoGrid().length != 0)
            draftsStatus.controls.draftsStatusNeoGrid().unbind('click').click(draftsStatus.onClickGrid);
        if (draftsStatus.controls.draftsStatusDeleteAction().length != 0)
            draftsStatus.controls.draftsStatusDeleteAction().unbind('click').click(draftsStatus.onClickDeleteBtnSM);
        if (draftsStatus.controls.draftsStatusRefreshButton().length != 0)
            draftsStatus.controls.draftsStatusRefreshButton().unbind('click').click(draftsStatus.refreshGridSM);


    },
    createRefMHandlersDraftsStatusScreen: function () {
        if (draftsStatus.controls.draftsStatusNeoGrid().length != 0)
            draftsStatus.controls.draftsStatusNeoGrid().unbind('click').click(draftsStatus.onClickGrid);
        if (draftsStatus.controls.draftsStatusDeleteAction().length != 0)
            draftsStatus.controls.draftsStatusDeleteAction().unbind('click').click(draftsStatus.onClickDeleteBtnRM);
        if (draftsStatus.controls.draftsStatusRefreshButton().length != 0)
            draftsStatus.controls.draftsStatusRefreshButton().unbind('click').click(draftsStatus.refreshGridRM);
    },

    getDraftsStatusGridSM: function (gridHeight) {
        onServiceUpdating();

        var currPageID = "s" + GetGUID();
        var viewKey = "s" + GetGUID();
        var sessionID = "s" + GetGUID();
        var customRowInfo = "";

        var tempObj = $.extend({}, draftsStatus.gridObj);
        tempObj = draftsStatus.initializeGridPropertiesSM(tempObj, customRowInfo, currPageID, viewKey, sessionID, gridHeight);
        //, gridInfo: tempObj
        $.ajax({
            type: 'POST',
            url: draftsStatus._path + draftsStatus._commonServiceLocation + "/GetDraftsDataSM",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ username: draftsStatus._username, divID: draftsStatus.controls.draftsStatusNeoGrid().attr("id"), currPageID: currPageID, viewKey: viewKey,  sessionID: sessionID, dateFormat: draftsStatus._cultureLongDate, jsonGridInfo: JSON.stringify(tempObj), isFromDashboard : draftsStatus._isFromDashboard,sectypeList : draftsStatus._sectypeList, isUserSpecific: draftsStatus._isUserSpecific  }),
            success: function (data) {
                if (parseInt(data.d) > 0) {
                    draftsStatus.controls.draftsStatusNoGridSection().hide();
                    xlgridloader.create(draftsStatus.controls.draftsStatusNeoGrid().attr("id"), "Test", tempObj, "");
                    $("#draftsStatusNeoGrid_MessageBoxID").css('min-height', 0);
                    draftsStatus.onGridRenderComplete();
                }
                else {
                    //                    console.log("1");
                    draftsStatus.controls.draftsStatusNoGridSection().show();
                    parent.leftMenu.showNoRecordsMsg('No Records to display.', draftsStatus.controls.draftsStatusNoGridSectionCls());
                    draftsStatus.controls.draftsStatusNoGridSectionCls().width("100%");
                }

                onServiceUpdated();
            },
            error: function () {
                console.log("Drafts Data cannot be fetched");
            }
        });
    },
    getDraftsStatusGridRM: function (gridHeight, moduleId) {
        onServiceUpdating();

        var currPageID = GetGUID();
        var viewKey = GetGUID();
        var sessionID = GetGUID();
        var customRowInfo = "";

        var tempObj = $.extend({}, draftsStatus.gridObj);
        tempObj = draftsStatus.initializeGridPropertiesRM(tempObj, customRowInfo, currPageID, viewKey, sessionID, gridHeight);


        // var moduleId = 

        $.ajax({
            type: 'POST',
            url: draftsStatus._path + draftsStatus._commonServiceLocation + "/GetDraftsDataRM",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ username: draftsStatus._username, divID: draftsStatus.controls.draftsStatusNeoGrid().attr("id"), currPageID: currPageID, viewKey: viewKey, sessionID: sessionID, dateFormat: draftsStatus._cultureShortDate, ModuleId: draftsStatus._moduleID, jsonGridInfo: JSON.stringify(tempObj) }),
            success: function (data) {
                if (parseInt(data.d) > 0) {
                    draftsStatus.controls.draftsStatusNoGridSection().hide();
                    xlgridloader.create(draftsStatus.controls.draftsStatusNeoGrid().attr("id"), "Test", tempObj, "");
                    $("#draftsStatusNeoGrid_MessageBoxID").css('min-height', 0);
                    draftsStatus.onGridRenderComplete();
                }
                else {
                    //   console.log("1");
                    draftsStatus.controls.draftsStatusNoGridSection().show();
                    parent.leftMenu.showNoRecordsMsg('No Records to display.', draftsStatus.controls.draftsStatusNoGridSectionCls());
                    draftsStatus.controls.draftsStatusNoGridSectionCls().width("100%");
                }

                onServiceUpdated();
            },
            error: function () {
                console.log("Drafts Data cannot be fetched");
            }
        });
    },

    initializeGridPropertiesSM: function (tempObj, customRowInfo, currPageID, viewKey, sessionID, gridHeight) {
        tempObj.GridId = draftsStatus.controls.draftsStatusNeoGrid().attr("id");
        tempObj.UserId = draftsStatus._username;
        tempObj.ViewKey = viewKey;
        tempObj.CurrentPageId = currPageID;
        tempObj.SessionIdentifier = sessionID;
        tempObj.IdColumnName = "sec_id";
        tempObj.ColumnsToHide = [{ "columnName": "sectype_id", "isDefault": true }, { "columnName": "template_id", "isDefault": true }, { "columnName": "sectype_description", "isDefault": true }, { "columnName": "row_keys", "isDefault": true }, { "columnName": "cloned_from", "isDefault": true }, { "columnName": "delete_existing", "isDefault": true }, { "columnName": "copy_time_series", "isDefault": true }];
        tempObj.DateFormat = draftsStatus._cultureLongDate;
        //tempObj.CustomRowsDataInfo = customRowInfo;
        if (gridHeight !== undefined) {
            tempObj.Height = gridHeight + "px";
        }
        return tempObj;
    },
    initializeGridPropertiesRM: function (tempObj, customRowInfo, currPageID, viewKey, sessionID, gridHeight) {
        tempObj.GridId = draftsStatus.controls.draftsStatusNeoGrid().attr("id");
        tempObj.UserId = draftsStatus._username;
        tempObj.ViewKey = viewKey;
        tempObj.CurrentPageId = currPageID;
        tempObj.SessionIdentifier = sessionID;
        tempObj.IdColumnName = "entity_code";
        tempObj.ColumnsToHide = [{ "columnName": "draft_id", "isDefault": true }, { "columnName": "is_active", "isDefault": true }, { "columnName": "entity_type_id", "isDefault": true }, { "columnName": "last_modified_by", "isDefault": true }];
        tempObj.DateFormat = draftsStatus._cultureLongDate;
        //tempObj.ColumnDateFormatMapping = [{ "Effective Start Date": draftsStatus._cultureShortDate }];
        //tempObj.CustomRowsDataInfo = customRowInfo;
        if (gridHeight !== undefined) {
            tempObj.Height = gridHeight + "px";
        }
        return tempObj;
    },
    onClickGrid: function (event) {
        event = event || window.event;
        var src = $(event.target);

        if (src.prop('id') != null && (src.prop('tagName').toUpperCase() == 'DIV' || src.prop('tagName').toUpperCase() == 'SPAN')) {
            if (src.prop('id').toLowerCase().indexOf('divsecurityid') != -1) {
                var securityId = src.attr('id').split('divSecurityId')[1];
                var cloned_from = src.attr('cloned_from');
                var delete_existing = src.attr('delete_existing');
                var copy_time_series = src.attr('copy_time_series');
                var effective_date = src.attr('effective_date');

                //SecMasterJSCommon.SMSCommons.openWindowForSecurityWithHiglight(true, securityId, "", "", true, true, SecMasterJSCommon.SMCreateUpdateOpenByDefault.None, 0, null);

                if (draftsStatus._editPrivilege)
                    SecMasterJSCommon.SMSCommons.openSecurity(true, securityId, effective_date, true, true, true, '', cloned_from, delete_existing, copy_time_series, SecMasterJSCommon.SMCreateUpdateOpenByDefault.None, 0, null, 3);
                else {
                    SecMasterJSCommon.SMSCommons.openSecurity(false, securityId, effective_date, true, true, false, '', cloned_from, delete_existing, copy_time_series, SecMasterJSCommon.SMCreateUpdateOpenByDefault.None, 0, null, 3);
                }

                //SecMasterJSCommon.SMSCommons.openWindowForDraftedSecurity(true, securityId, true, true, SecMasterJSCommon.SMCreateUpdateOpenByDefault.None, 0, null, true);
            }
            else if (src.prop('id').toLowerCase().indexOf('diventitycode') != -1) {
                debugger;
                var entityCode = src.attr('id').split('divEntityCode')[1];
                //Open Create Page
                if (draftsStatus._editPrivilege) {
                    var containerPage = "App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?";
                    var queryString = "entityCode=" + entityCode + "&";
                    queryString += "entityTypeId=" + src.attr('entity_type_id').toLowerCase() + "&";
                    queryString += "entityDisplayName=" + src.attr('entity_type_id').toLowerCase() + "&";
                    queryString += "effectiveDate=" + src.attr('effectiveDate').toLowerCase() + "&";
                    var uniqueId = "unique_edit_" + GetGUID();
                    var url = containerPage + queryString.trim() + "pageIdentifier=DraftEntity&tabFrameId=" + uniqueId + "&RMTabUniqueID=" + uniqueId;
                    RefMasterJSCommon.RMSCommons.createTab("RefM_ViewEntity", url, uniqueId, entityCode);
                }
                else {
                    debugger;
                    var containerPage = "App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?";
                    var queryString = "entityCode=" + entityCode + "&";
                    queryString += "entityTypeId=" + src.attr('entity_type_id').toLowerCase() + "&";
                    queryString += "entityDisplayName=" + src.attr('entity_type_id').toLowerCase() + "&";
                    queryString += "effectiveDate=" + src.attr('effectiveDate').toLowerCase() + "&";
                    var uniqueId = "unique_edit_" + GetGUID();
                    var url = containerPage + queryString.trim() + "pageIdentifier=DraftEntity&ViewDraftModule=true&tabFrameId=" + uniqueId + "&RMTabUniqueID=" + uniqueId;
                    RefMasterJSCommon.RMSCommons.createTab("RefM_ViewEntity", url, uniqueId, entityCode);
                }
            }
        }
    },
    onGridRenderComplete: function () {
        var grid = $find(draftsStatus.controls.draftsStatusNeoGrid().attr("id"));
        resizeGridFinal(draftsStatus.controls.draftsStatusNeoGrid().attr("id"), os_serverSideControls.TopPanel, os_serverSideControls.MiddlePanel, os_serverSideControls.BottomPanel, 0, 210);

        if (grid != null) {
            this.onGridServiceCompleted = Function.createDelegate(this, draftsStatus.getCheckedRowsInfoDraftsStatus);
            grid.eventHandlerManager.addServiceCompletedEventHandler(this.onGridServiceCompleted);
        }
    },
    getCheckedRowsInfoDraftsStatus: function (info) {
        //Code for enabling/disabling the delete button
        draftsStatus._selectedRowsCount = info.serializedInfo.CheckedRowIndices.length;

        //multiple request handling
        //initially disabled delete button
        draftsStatus.controls.draftsStatusDeleteAction().addClass("draftsDisabled");



        //if (draftsStatus._selectedRowsCount > 0) {
        //draftsStatus.controls.draftsStatusDeleteAction().removeClass("draftsDisabled");
        //}
        //else {
        //    if (!draftsStatus.controls.draftsStatusDeleteAction().hasClass("draftsDisabled")) {
        //        draftsStatus.controls.draftsStatusDeleteAction().addClass("draftsDisabled");
        //    }
        //}

        //binding of selection
        if (info.eventType === com.ivp.rad.controls.neogrid.scripts.EventType.evenT_ROW_WITH_CHECKBOX_SELECTED || info.eventType === com.ivp.rad.controls.neogrid.scripts.EventType.evenT_ROW_WITH_CHECKBOX_SELECTED_ALL || info.eventType === com.ivp.rad.controls.neogrid.scripts.EventType.evenT_ROW_WITH_CHECKBOX_UNSELECTED || info.eventType === com.ivp.rad.controls.neogrid.scripts.EventType.evenT_ROW_WITH_CHECKBOX_UNSELECTED_ALL || info.eventType === com.ivp.rad.controls.neogrid.scripts.EventType.evenT_ROW_WITH_GROUP_HEADER_CHECKBOX_SELECTED || info.eventType === com.ivp.rad.controls.neogrid.scripts.EventType.evenT_ROW_WITH_GROUP_HEADER_CHECKBOX_UNSELECTED) {

            draftsStatus._ajaxHitCounts++;

            var cacheKey = $find(draftsStatus.controls.draftsStatusNeoGrid().attr("id")).get_CacheKey();
            $.ajax({
                url: draftsStatus._path + draftsStatus._commonServiceLocation + '/DraftsStatusGetCheckedRowsData',
                type: 'POST',
                data: JSON.stringify({ 'cacheKey': cacheKey }),
                contentType: "application/json; charset=utf-8",
                dataType: 'json',
                success: function (data) {
                    draftsStatus._ajaxHitCounts--;
                    var selectedData = data.d;
                    if (selectedData !== null && selectedData !== undefined) {
                        var secIDArr = selectedData.split("|");
                        if (secIDArr)
                            draftsStatus._checkedRowsDraftsStatus = secIDArr.filter(secID => secID.length > 0);



                        //multiple request handling

                        if (draftsStatus._ajaxHitCounts == 0 && draftsStatus._checkedRowsDraftsStatus.length > 0) {
                            draftsStatus.controls.draftsStatusDeleteAction().removeClass("draftsDisabled");
                        }
                    }
                },
                error: function (e) {
                    console.log(e.message);
                }
            });
        }
    },
    onClickDeleteBtnSM: function () {
        //if (!overrideStatus.controls.overrideStatusDeleteAction().hasClass("overrideStatusDisabled")) {
        
        var deleteInfo = [];
        deleteInfo = draftsStatus._checkedRowsDraftsStatus;
        //  }

        $.ajax({
            type: 'POST',
            url: draftsStatus._path + draftsStatus._commonServiceLocation + "/SMDeleteDrafts",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ deleteInfo: deleteInfo, username: draftsStatus._username }),
            success: function (data) {
                if (data.d.toLowerCase() === "success") {
                    draftsStatus.refreshGridSM();
                    onServiceUpdated();
                    draftsStatus.displayPopup("Drafts deleted successfully", "success");
                }
                else {
                    draftsStatus.displayPopup("Error occurred while deleting drafts", "failure");
                }
            },
            error: function (ex) {
                draftsStatus.displayPopup(ex, "failure");
            }
        });

    },
    onClickDeleteBtnRM: function () {

        var deleteInfo = [];
        deleteInfo = draftsStatus._checkedRowsDraftsStatus;

        $.ajax({
            type: 'POST',
            url: draftsStatus._path + draftsStatus._commonServiceLocation + "/RMDeleteDrafts",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ deleteInfo: deleteInfo, username: draftsStatus._username }),
            success: function (data) {
                if (data.d.toLowerCase() === "success") {
                    draftsStatus.refreshGridRM();
                    onServiceUpdated();
                    draftsStatus.displayPopup("Drafts deleted successfully", "success");
                }
                else {
                    draftsStatus.displayPopup("Error occurred while deleting drafts", "failure");
                }
            },
            error: function (ex) {
                draftsStatus.displayPopup(ex, "failure");
            }
        });

    },
    refreshGridSM: function (gridHeight) {
        console.log("refreshed");
        if (gridHeight !== undefined) {
            draftsStatus.secMasterInitDraftsStatusScreen(gridHeight);
        }
        else {
            draftsStatus.secMasterInitDraftsStatusScreen();
        }
    },

    refreshGridRM: function (gridHeight) {
        if (gridHeight !== undefined) {
            if (draftsStatus._moduleID == 6)
                draftsStatus.refMasterInitDraftsStatusScreen(gridHeight);
            if (draftsStatus._moduleID == 18)
                draftsStatus.fundMasterInitDraftsStatusScreen(gridHeight);
            if (draftsStatus._moduleID == 20)
                draftsStatus.partyMasterInitDraftsStatusScreen(gridHeight);
        }
        else {
            if (draftsStatus._moduleID == 6)
                draftsStatus.refMasterInitDraftsStatusScreen(gridHeight);
            if (draftsStatus._moduleID == 18)
                draftsStatus.fundMasterInitDraftsStatusScreen(gridHeight);
            if (draftsStatus._moduleID == 20)
                draftsStatus.partyMasterInitDraftsStatusScreen(gridHeight);
        }
    },
    displayPopup: function (message, status) {
        var imageSrc = "";
        var borderTop = "";
        var title = "";
        var color = "";
        switch (status) {
            case "success":
                imageSrc = "images/icons/pass_icon.png";
                borderTop = "4px solid #ACD373";
                title = "Success";
                color = "#ACD373";
                break;
            case "failure":
                imageSrc = "images/icons/fail_icon.png";
                borderTop = "4px solid #C7898C";
                title = "Failure";
                color = "#C7898C";
                break;
            case "alert":
                imageSrc = "images/icons/alert_icon.png";
                borderTop = "4px solid #F4AD02";
                title = "Alert";
                color = "#F4AD02";
                break;
        }

        var left = (draftsStatus._windowWidth / 2) - 200;
        var msgPopup = $("#os_messagePopup");
        msgPopup.find(".draftspopupTitle").text(title).css("color", color);
        msgPopup.find(".draftsMessage").text(message);
        msgPopup.find("img").attr("src", imageSrc);
        msgPopup.css("border-top", borderTop);
        msgPopup.css("left", left + "px");
        msgPopup.css("top", "-" + msgPopup.height() + "px");
        msgPopup[0].style.display = "";
        msgPopup.animate({ top: "0px" }, 500);

        msgPopup.find(".draftsPopupCloseBtn").unbind("click").bind("click", function (e) {
            msgPopup.animate({ top: "-" + msgPopup.height() + "px" }, 500, function () {
                msgPopup[0].style.display = "none";
            });
        });
    },
    controls: {
        draftsStatusNeoGrid: function () {
            return $("#draftsStatusNeoGrid");
        },

        draftsStatusDeleteAction: function () {
            return $("#draftsStatusDeleteAction");
        },
        draftsStatusRefreshButton: function () {
            return $("#draftsStatusRefreshButton");
        },


        draftsStatusNoGridSection: function () {
            return $("#draftsNoGridSection");
        },
        draftsStatusNoGridSectionCls: function () {
            return $(".draftsNoGridSection");
        }

    },






    createSMSelectDropDown: function (dropDownId, smdata, showSearch, width, callback, heading, onChangeHandler, selectedItems, isMulti, multiText) {
        var obj = {};
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
            if (typeof onChangeHandler === "function") {
                e.on('change', function (ee) {
                    onChangeHandler(ee);
                });
            }
        }
        smselect.create(obj);

        $("#smselect_" + dropDownId.attr("id")).find(".smselect").css('text-align', 'left');
        $("#smselect_" + dropDownId.attr("id")).find(".smselectanchorcontainer2").css('width', '86%');

        if (typeof callback === "function") {
            callback();
        }
    },
    gridObj: {
        "SelectRecordsAcrossAllPages": true,
        "ViewKey": "", //to be overriden
        "CacheGriddata": true,
        "CurrentPageId": "", //to be overriden
        "SessionIdentifier": "", //to be overriden
        "UserId": "",
        "GridId": "", //to be overriden
        "CheckBoxInfo": {},
        "Height": "400px",
        "ColumnsWithoutClientSideFunctionality": [],
        "ColumnsNotToSum": [],
        "ColumnsToHide": [], //to be overriden
        "RequireEditGrid": false,
        "RequireEditableRow": false,
        "IdColumnName": "row_id", //to be overriden
        "TableName": "draftsTable",
        "PageSize": 100,
        "RequirePaging": false,
        "RequireInfiniteScroll": true,
        "CollapseAllGroupHeader": false,
        "GridTheme": 2,
        "HideFooter": false,
        "DoNotExpand": false,
        "ItemText": "Security Drafts",
        "DoNotRearrangeColumn": true,
        "RequireGrouping": true,
        "RequireMathematicalOperations": false,
        "RequireSelectedRows": true,
        "RequireExportToExcel": true,
        "RequireSearch": true,
        "RequireFreezeColumns": false,
        "RequireHideColumns": true,
        "RequireColumnSwap": false,
        "RequireGroupExpandCollapse": true,
        "RequireResizing": true,
        "RequireLayouts": false,
        "RequireViews": false,
        "RequireRuleBasedColoring": false,
        "RequireGroupHeaderCheckbox": false,
        "ShowRecordCountOnHeader": false,
        "ShowAggregationOnHeader": false,
        "RequireFilter": true,
        "CssExportRows": "xlneoexportToExcel"
    }




}