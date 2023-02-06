function makeDateFormatForDateControl() {
    let shortDateFormatClient = os_serverSideValues.CultureShortDateFormat;
    if (shortDateFormatClient) {
        if (shortDateFormatClient == "dd/MM/yyyy" || shortDateFormatClient == "d/M/yyyy")
            return "d/m/Y";
        else if (shortDateFormatClient == "MM/dd/yyyy" || shortDateFormatClient == "M/d/yyyy")
            return "m/d/Y"
    }
}

var slideDateControl = (function () {
    function sDateControl() {
        this.counter = 0;
    }

    var SlideDateControl = new sDateControl();

    sDateControl.prototype.init = function ($object) {
        if ($object.id === undefined) {
            $object.id = "slideDateControl_" + SlideDateControl.counter++;
        }
        $object.inputId = $object.id + "_inputDateControl";
        $(document).ready(function () {
            if ($object.container !== undefined && $object.container instanceof jQuery && $object.dateFormat !== undefined) {
                createHTML($object);
                //576385 - 
                $("#" + $object.inputId).val(new Date().toLocaleDateString());
            }
            else {
                console.log("Please enter data correctly");
            }
        });
    }

    sDateControl.prototype.getSelectedValue = function (targetDiv) {
        var childs = $("#" + targetDiv).children('div')
        var selectedDiv = null;
        childs.each(function (index, ele) {
            if ($(ele).hasClass("slideDateControl_selected")) {
                selectedDiv = ele;
            }
        });

        if ($(selectedDiv).find('input').length > 0) {
            return $(selectedDiv).find('input').val();
        }
        else {
            return "Never";
        }
    }

    function createHTML($object) {
        $object.container.append(createTemplate($object));
        CallCustomDatePicker($object.inputId, $object.dateFormat, null, optionDateTime.DATE, 15, false);
        attachHandlers($object);
    }

    function attachHandlers($object) {
        $("#" + $object.id).unbind("click").bind("click", function (e) {
            var target = e.target;
            $("#" + $object.id).children('div').removeClass("slideDateControl_selected");

            if (target.tagName === "INPUT") {
                target = target.closest('div');
            }

            if (!$(target).hasClass('slideDateControlContainer')) {
                $(target).addClass("slideDateControl_selected");
            }
        });
    }

    function createTemplate($object) {
        var html_template = "<div id='" + $object.id + "' class='slideDateControlContainer'>";
        html_template += "<div class='overrideStatusHorizontalAlign slideDateControlLeftOption slideDateControl_selected'>Never</div>";
        html_template += "<div class='overrideStatusHorizontalAlign slideDateControlRightOption'><input type='text' id='" + $object.inputId + "' placeholder='Enter Date' /></div>";
        html_template += "</div>";

        return html_template;
    }

    return SlideDateControl;
})();

var overrideStatus = {
    _path: "",
    _windowHeight: "",
    _windowWidth: "",
    _username: "",
    _moduleID: 0,                                               //Added ModuleID
    _fromScreenName: "",
    _selectedRowsCount: "",
    _selectedSecuritiesOrEntitesFromGrid: "",
    _cultureShortDate: "",
    _cultureLongDate: "",
    _checkedRowsOverrideStatus: [],
    _prodName: "",
    _selectedSecIDsFromSearch: [],
    _selectedSecIDsFromGrid: [],
    _commonServiceLocation: "/BaseUserControls/Service/CommonService.svc",
    _attributesListData: [],
    _attrViewModel: null,
    _isOverridesViewAllowedInSecM: false,
    _isOverridesViewAllowedInRefM: false,
    _moduleID_moduleName_map: [],
    _deletePrivilege: false,
    _editPrivilege: false,
    _viewPrivilege: false,
    privilegeList: {
        3: ['Monitor Overrides - Delete Security', 'Monitor Overrides - View Security', 'Monitor Overrides - Edit Security'],
        6: ['RM - Monitor Overrides - Delete Entity', 'RM - Monitor Overrides - View Entity', 'RM - Monitor Overrides - Edit Entity'],
        18: ['FM - Monitor Overrides - Delete Entity', 'FM - Monitor Overrides - View Entity', 'FM - Monitor Overrides - Edit Entity'],
        20: ['PM - Monitor Overrides - Delete Entity', 'PM - Monitor Overrides - View Entity', 'PM - Monitor Overrides - Edit Entity']
    },
    getPrivilegesForModules: function () {
        overrideStatus._deletePrivilege = false;
        overrideStatus._editPrivilege = false;
        overrideStatus._viewPrivilege = false;
        $("#overrideStatusDeleteAction").css('visibility', 'hidden');
        var params = {
            userName: overrideStatus._username,
            privilegeName: overrideStatus.privilegeList[overrideStatus._moduleID]
        }
        overrideStatus.CallCommonServiceMethod('CheckControlPrivilegeForUserMultiple', params, overrideStatus.OnSuccess_GetPrivileges, OnFailure, null, false);
    },
    CallCommonServiceMethod: function (methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    },
    OnSuccess_GetPrivileges: function (data) {
        var privileges = data.d;
        debugger;
        //first is delete,second is view security, third is edit security
        if (privileges != null && privileges.length == 3) {
            if (privileges[2].result) {
                overrideStatus._editPrivilege = true;
            }
            if (privileges[1].result) {
                overrideStatus._viewPrivilege = true;
            }
            if (privileges[0].result) {
                $("#overrideStatusDeleteAction").css('visibility', 'visible');
                overrideStatus._deletePrivilege = true;
            }
        }
    },
    setPath: function () {
        var path = window.location.protocol + '//' + window.location.host;
        var pathname = window.location.pathname.split('/');

        $.each(pathname, function (ii, ee) {
            if ((ii !== 0) && (ii !== pathname.length - 1))
                path = path + '/' + ee;
        });

        overrideStatus._path = path;
    },
    setProductOverrideStatusScreen: function () {
        if (overrideStatus._prodName.toLowerCase() === "secmaster") {
            var isSecMSelected = true;
            var isRefMSelected = false;
            if (overrideStatus._selectedProduct.toLowerCase() === "secmaster") {
                isSecMSelected = true;
            }
            else if (overrideStatus._selectedProduct.toLowerCase() === "refmaster") {
                isRefMSelected = true;
                isSecMSelected = false;
            }

            var privilegeArr = [];
            var moduleValue = 0;

            if (overrideStatus._isOverridesViewAllowedInSecM) {
                privilegeArr.push({ name: 'SecMaster', value: ++moduleValue, callback: overrideStatus.secMasterInitOverrideStatusScreen, isSelected: isSecMSelected });
            }

            if (overrideStatus._isOverridesViewAllowedInRefM) {
                privilegeArr.push({ name: 'RefMaster', value: ++moduleValue, callback: overrideStatus.refMasterInitOverrideStatusScreen, isSelected: isRefMSelected });
            }

            if (privilegeArr.length === 1) {
                privilegeArr[0].isSelected = true;
            }

            return privilegeArr;
        }
        else if (overrideStatus._prodName.toLowerCase() === "refmaster") {
            if (overrideStatus._isOverridesViewAllowedInRefM) {
                return [{ name: 'RefMaster', value: ++moduleValue, callback: overrideStatus.refMasterInitOverrideStatusScreen, isSelected: true }];
            }
            else {
                return [];
            }
        }
    },
    setProductOverridesFromSearchScreen: function () {
        if (overrideStatus._prodName.toLowerCase() === "securities") {
            //return [{ name: 'SecMaster', value: 1, callback: overrideStatus.secMasterInitOverridesFromSearchScreen, isSelected: true }, { name: 'RefMaster', value: 2, callback: function () { }, isSelected: false}];
            overrideStatus.secMasterInitOverridesFromSearchScreen();
        }
        else if (overrideStatus._prodName.toLowerCase() === "refdata") {
            //return [{ name: 'RefMaster', value: 1, callback: function () { }, isSelected: false}];
            overrideStatus.refMasterInitOverridesFromSearchScreen();
        }

        overrideStatus.createHandlersFromSearchScreen()
    },
    createHandlersFromSearchScreen: function () {
        if (overrideStatus.controls.oversideStatusViewOverrides().length != 0)
            overrideStatus.controls.oversideStatusViewOverrides().unbind('click').click(overrideStatus.onClickViewOverrides);
        if (overrideStatus.controls.overridesStatusSaveOverride().length != 0)
            overrideStatus.controls.overridesStatusSaveOverride().unbind('click').click(overrideStatus.onClickSaveOverride);
    },
    getViewPrivileges: function () {
        return new Promise(function (res, rej) {
            var promiseArr = [];

            if (overrideStatus._prodName.toLowerCase() !== "refmaster") {
                var p1 = new Promise(function (resolve, reject) {
                    $.ajax({
                        type: 'POST',
                        url: overrideStatus._path + overrideStatus._commonServiceLocation + "/CheckControlPrivilegeForUser",
                        contentType: "application/json",
                        dataType: "json",
                        data: JSON.stringify({
                            "privilegeName": "Monitor Reference data Overrides",
                            "userName": overrideStatus._username
                        }),
                        success: function (data) {
                            data = data.d;
                            overrideStatus._isOverridesViewAllowedInRefM = data;
                            resolve();
                        },
                        error: function (ex) {
                            console.log("Overrides Privilege cannot be fetched");
                            reject(ex);
                        }
                    });
                });
                promiseArr.push(p1);
            }
            var p2 = new Promise(function (resolve, reject) {
                $.ajax({
                    type: 'POST',
                    url: overrideStatus._path + overrideStatus._commonServiceLocation + "/CheckControlPrivilegeForUser",
                    contentType: "application/json",
                    dataType: "json",
                    data: JSON.stringify({
                        "privilegeName": "Monitor Security data Overrides",
                        "userName": overrideStatus._username
                    }),
                    success: function (data) {
                        data = data.d;
                        overrideStatus._isOverridesViewAllowedInSecM = data;
                        resolve();
                    },
                    error: function (ex) {
                        console.log("Overrides Privilege cannot be fetched");
                        reject(ex);
                    }
                });
            });
            promiseArr.push(p2);

            Promise.all(promiseArr)
                .then(function () { res(); })
                .catch(function (ex) { rej(ex); });
        });
    },
    init: function () {
        //To fetch the url to make ajax calls
        overrideStatus.setPath();
        overrideStatus.initializeValues();


        var listofTabsToBindFunctionsWith = CommonModuleTabs.ModuleList();

        if (overrideStatus._fromScreenName !== null && overrideStatus._fromScreenName.toLowerCase() === "search") {
            //overrideStatus.initializeOverridesFromSearchScreen();
            overrideStatus.controls.overrideStatusScreenSection().css('display', 'none');

            for (i in listofTabsToBindFunctionsWith) {
                item = listofTabsToBindFunctionsWith[i];
                overrideStatus._moduleID_moduleName_map[item.displayName.toLowerCase()] = item.moduleId;

                switch (item.displayName.toLowerCase().trim()) {
                    case "securities":
                        if (overrideStatus._prodName.toLowerCase().trim() == "secmaster") {
                            SRMProductTabs.setCallback({
                                key: item.displayName.toLowerCase(), value: overrideStatus.secMasterInitOverridesFromSearchScreen
                            });
                        }

                        break;
                    case "refdata":
                        if (overrideStatus._prodName.toLowerCase().trim() == "refmaster") {
                            SRMProductTabs.setCallback({
                                key: item.displayName.toLowerCase(), value: overrideStatus.refMasterInitOverridesFromSearchScreen
                            });
                        }
                        break;
                    case "funds":
                        if (overrideStatus._prodName.toLowerCase().trim() == "refmaster") {
                            SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: overrideStatus.refMasterInitOverridesFromSearchScreen });
                        }
                        break;
                    case "parties":
                        if (overrideStatus._prodName.toLowerCase().trim() == "refmaster") {
                            SRMProductTabs.setCallback({
                                key: item.displayName.toLowerCase(), value: overrideStatus.refMasterInitOverridesFromSearchScreen
                            });
                            break;
                        }
                }
            }

            overrideStatus.createHandlersFromSearchScreen();
        }
        else {
            //overrideStatus.initializeOverrideStatusScreen();
            overrideStatus.controls.overridesFromSearchScreenSection().css('display', 'none');
            for (i in listofTabsToBindFunctionsWith) {
                item = listofTabsToBindFunctionsWith[i];
                overrideStatus._moduleID_moduleName_map[item.displayName.toLowerCase()] = item.moduleId;
                //draftsStatus._moduleID = item.moduleId;
                switch (item.displayName.toLowerCase().trim()) {
                    case "securities":
                        SRMProductTabs.setCallback({
                            key: item.displayName.toLowerCase(), value: overrideStatus.secMasterInitOverrideStatusScreen
                        });

                        break;
                    case "refdata":
                        SRMProductTabs.setCallback({
                            key: item.displayName.toLowerCase(), value: overrideStatus.refMasterInitOverrideStatusScreen
                        });
                        break;
                    case "funds":
                        SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: overrideStatus.fundMasterInitOverrideStatusScreen });
                        break;
                    case "parties":
                        SRMProductTabs.setCallback({
                            key: item.displayName.toLowerCase(), value: overrideStatus.partyMasterInitOverrideStatusScreen
                        });
                        break;
                }
            }
        }


    },
    initializeOverrideStatusScreen: function () {
        if (overrideStatus._prodName !== null && overrideStatus._prodName.toLowerCase() === "refmaster") {
            overrideStatus.refMasterInitOverrideStatusScreen();
        }
        else {
            var obj = {
                container: $("#os_moduleTabs"),
                theme: "middleTheme",
                data: overrideStatus.setProductOverrideStatusScreen()
            }
            CommonModuleTabs.init(obj);
        }
    },
    initializeOverridesFromSearchScreen: function () {
        //        var obj = {
        //            container: $("#os_moduleTabs"),
        //            data: overrideStatus.setProductOverridesFromSearchScreen()
        //        }
        //        CommonModuleTabs.init(obj);

        overrideStatus.setProductOverridesFromSearchScreen();
    },
    initializeValues: function () {
        overrideStatus._username = os_serverSideValues.Username;
        overrideStatus._fromScreenName = os_serverSideValues.FromScreenName;
        overrideStatus._prodName = os_serverSideValues.ProductName;
        overrideStatus._windowHeight = $(window).height();
        overrideStatus._windowWidth = $(window).width();
        overrideStatus._cultureShortDate = os_serverSideValues.CultureShortDateFormat;
        overrideStatus._cultureLongDate = os_serverSideValues.CultureLongDateFormat;
        overrideStatus._selectedSecuritiesOrEntitesFromGrid = overrideStatus._selectedSecIDsOrEntityCodesFromSearch;
        overrideStatus._moduleID = os_serverSideValues.ModuleID;
        overrideStatus._selectedProduct = os_serverSideValues.SelectedProduct;
    },
    secMasterInitOverrideStatusScreen: function (gridHeight) {
        overrideStatus.destroyGrid();
        overrideStatus.getoverrideStatusGridSM(gridHeight);
        overrideStatus.createSecMHandlersOverrideStatusScreen();
        overrideStatus._moduleID = overrideStatus._moduleID_moduleName_map["securities"];
        overrideStatus.getPrivilegesForModules();
    },
    secMasterInitOverridesFromSearchScreen: function () {
        overrideStatus.controls.overrideStatusGridTitle().text("SECURITIES WITH OVERRIDES");
        overrideStatus.createSecMHandlersOverrideFromSearchScreen();
        overrideStatus.destroyGrid();
        overrideStatus.getSecurityOverridesDataGridSM();
        overrideStatus._moduleID = overrideStatus._moduleID_moduleName_map["securities"];
        overrideStatus.getPrivilegesForModules();

        var gridHeight = overrideStatus._windowHeight - (500 + $("#SRMPanelTop").height()); //210 is grid Height and 135 is extra buffer
        overrideStatus.controls.overrideStatusAttributeListSection().height(gridHeight);

        var dateObj = {}
        dateObj.container = overrideStatus.controls.overrideStatusAttrDate();
        var returnedDate = makeDateFormatForDateControl();
        dateObj.dateFormat = returnedDate ? returnedDate : "m/d/Y";
        slideDateControl.init(dateObj);

        overrideStatus._attrViewModel = new overrideStatus.attributeViewModel(overrideStatus._attributesListData);
        ko.applyBindings(overrideStatus._attrViewModel, overrideStatus.controls.overrideStatusAddAttributeSection()[0]);
    },
    refMasterInitOverridesFromSearchScreen: function () {
        overrideStatus.createRefMHandlersOverrideFromSearchScreen();
        overrideStatus.controls.overrideStatusGridTitle().text("ENTITIES WITH OVERRIDES");
        overrideStatus.destroyGrid();
        overrideStatus.getEntityOverridesDataGridRM();
        overrideStatus.getPrivilegesForModules();

        var gridHeight = overrideStatus._windowHeight - (500 + $("#SRMPanelTop").height()); //210 is grid Height and 135 is extra buffer
        overrideStatus.controls.overrideStatusAttributeListSection().height(gridHeight);

        var dateObj = {}
        dateObj.container = overrideStatus.controls.overrideStatusAttrDate();
        var returnedDate = makeDateFormatForDateControl();
        dateObj.dateFormat = returnedDate ? returnedDate : "m/d/Y";
        slideDateControl.init(dateObj);

        overrideStatus._attrViewModel = new overrideStatus.attributeViewModel(overrideStatus._attributesListData);
        ko.applyBindings(overrideStatus._attrViewModel, overrideStatus.controls.overrideStatusAddAttributeSection()[0]);
    },
    collapsibleTitleOnClick: function () {
        $(".overrideStatusTitle").each(function (index, ele) {
            $(ele).unbind('click').bind('click', function (e) {
                $(ele).siblings().slideToggle();
            });
        });
    },
    refMasterInitOverrideStatusScreen: function (gridHeight) {
        overrideStatus.destroyGrid();
        overrideStatus._moduleID = overrideStatus._moduleID_moduleName_map["refdata"];
        overrideStatus.getoverrideStatusGridRM(gridHeight);
        overrideStatus.createRefMHandlersOverrideStatusScreen();
        overrideStatus.getPrivilegesForModules();
    },
    fundMasterInitOverrideStatusScreen: function (gridHeight) {
        overrideStatus.destroyGrid();
        overrideStatus._moduleID = overrideStatus._moduleID_moduleName_map["funds"];
        overrideStatus.getoverrideStatusGridRM(gridHeight);
        overrideStatus.createRefMHandlersOverrideStatusScreen();
        overrideStatus.getPrivilegesForModules();
    },
    partyMasterInitOverrideStatusScreen: function (gridHeight) {
        overrideStatus.destroyGrid();
        overrideStatus._moduleID = overrideStatus._moduleID_moduleName_map["parties"];
        overrideStatus.getoverrideStatusGridRM(gridHeight);
        overrideStatus.createRefMHandlersOverrideStatusScreen();
        overrideStatus.getPrivilegesForModules();
    },
    destroyGrid: function () {
        overrideStatus.controls.overrideStatusNeoGrid().html("");
    },
    createSecMHandlersOverrideStatusScreen: function () {
        if (overrideStatus.controls.overrideStatusNeoGrid().length != 0)
            overrideStatus.controls.overrideStatusNeoGrid().unbind('click').click(overrideStatus.onClickGrid);
        if (overrideStatus.controls.overrideStatusDeleteAction().length != 0)
            overrideStatus.controls.overrideStatusDeleteAction().unbind('click').click(overrideStatus.onClickDeleteBtnSM);
        if (overrideStatus.controls.overrideStatusRefreshButton().length != 0)
            overrideStatus.controls.overrideStatusRefreshButton().unbind('click').click(overrideStatus.refreshGridSM);
    },
    createRefMHandlersOverrideStatusScreen: function () {
        if (overrideStatus.controls.overrideStatusNeoGrid().length != 0)
            overrideStatus.controls.overrideStatusNeoGrid().unbind('click').click(overrideStatus.onClickGrid);
        if (overrideStatus.controls.overrideStatusDeleteAction().length != 0)
            overrideStatus.controls.overrideStatusDeleteAction().unbind('click').click(overrideStatus.onClickDeleteBtnRM);
        if (overrideStatus.controls.overrideStatusRefreshButton().length != 0)
            overrideStatus.controls.overrideStatusRefreshButton().unbind('click').click(overrideStatus.refreshGridRM);
    },
    createSecMHandlersOverrideFromSearchScreen: function () {
        if (overrideStatus.controls.overrideStatusSecurityDataNeoGrid().length != 0)
            overrideStatus.controls.overrideStatusSecurityDataNeoGrid().unbind('click').click(overrideStatus.onClickGrid);
        overrideStatus.collapsibleTitleOnClick();
    },
    createRefMHandlersOverrideFromSearchScreen: function () {
        if (overrideStatus.controls.overrideStatusSecurityDataNeoGrid().length != 0)
            overrideStatus.controls.overrideStatusSecurityDataNeoGrid().unbind('click').click(overrideStatus.onClickGrid);
        overrideStatus.collapsibleTitleOnClick();
    },
    onClickDeleteBtnRM: function () {
        if (!overrideStatus.controls.overrideStatusDeleteAction().hasClass("overrideStatusDisabled")) {
            var overrideData = [];
            for (var item in overrideStatus._checkedRowsOverrideStatus) {

                var isEntityCodePresentInOverrideData = false;
                for (var row in overrideData) {
                    if (overrideData[row].EntityCode === overrideStatus._checkedRowsOverrideStatus[item].entityCode) {
                        isEntityCodePresentInOverrideData = true;
                        break;
                    }
                }

                if (isEntityCodePresentInOverrideData === false) {
                    var overrideObj = {};
                    overrideObj["Attributes"] = [];
                    overrideObj["DeleteExisting"] = false;
                    overrideObj["EntityCode"] = overrideStatus._checkedRowsOverrideStatus[item].entityCode;

                    overrideObj["Attributes"].push({ "AttributeName": overrideStatus._checkedRowsOverrideStatus[item].attributeDisplayName, "AttributeValue": "", "DeleteExisting": true, "LockedUntil": null });
                }
                else {
                    overrideObj["Attributes"].push({ "AttributeName": overrideStatus._checkedRowsOverrideStatus[item].attributeDisplayName, "AttributeValue": "", "DeleteExisting": true, "LockedUntil": null });
                }
                overrideData.push(overrideObj);
            }

            $.ajax({
                type: 'POST',
                url: overrideStatus._path + overrideStatus._commonServiceLocation + "/RMDeleteOverride",
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify({ username: overrideStatus._username, attrInfo: overrideData }),
                success: function (data) {
                    if (overrideStatus._fromScreenName !== null && overrideStatus._fromScreenName.toLowerCase() === "search") {
                        overrideStatus.refreshGridRM((overrideStatus._windowHeight - 360));
                    }
                    else {
                        overrideStatus.refreshGridRM();
                    }

                    overrideStatus.displayPopup("Override deleted successfully", "success");
                },
                error: function () {
                    overrideStatus.displayPopup(ex, "failure");
                }
            });
        }
    },
    onClickDeleteBtnSM: function () {
        if (!overrideStatus.controls.overrideStatusDeleteAction().hasClass("overrideStatusDisabled")) {
            var deleteInfo = [];
            for (var item in overrideStatus._checkedRowsOverrideStatus) {
                var tempObj = {};
                tempObj["Key"] = overrideStatus._checkedRowsOverrideStatus[item].overrideID;
                tempObj["Value"] = (overrideStatus._checkedRowsOverrideStatus[item].tableName === undefined) ? "" : overrideStatus._checkedRowsOverrideStatus[item].tableName;
                deleteInfo.push(tempObj);
            }

            $.ajax({
                type: 'POST',
                url: overrideStatus._path + overrideStatus._commonServiceLocation + "/SMDeleteOverride",
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify({ deleteInfo: deleteInfo }),
                success: function (data) {
                    if (data.d.toLowerCase() === "success") {
                        //if (overrideStatus._fromScreenName !== null && overrideStatus._fromScreenName.toLowerCase() === "search") {
                        //    overrideStatus.refreshGridSM((overrideStatus._windowHeight - 360));
                        //}
                        // else {
                        overrideStatus.refreshGridSM();
                        // }
                        onServiceUpdated();
                        overrideStatus.displayPopup("Override deleted successfully", "success");
                    }
                    else {
                        overrideStatus.displayPopup(data.d.length > 0 ? data.d : "Error occurred while deleting overrides", "failure");
                    }
                },
                error: function (ex) {
                    overrideStatus.displayPopup(ex, "failure");
                }
            });
        }
    },
    getoverrideStatusGridSM: function (gridHeight) {
        onServiceUpdating();

        var currPageID = GetGUID();
        var viewKey = GetGUID();
        var sessionID = GetGUID();

        $.ajax({
            type: 'POST',
            url: overrideStatus._path + overrideStatus._commonServiceLocation + "/GetOverridesDataSM",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ username: overrideStatus._username, divID: overrideStatus.controls.overrideStatusNeoGrid().attr("id"), currPageID: currPageID, viewKey: viewKey, sessionID: sessionID, dateFormat: overrideStatus._cultureLongDate }),
            success: function (data) {
                if (parseInt(data.d) > 0) {
                    overrideStatus.controls.overrideStatusNoGridSection().hide();

                    var customRowInfo = "";//JSON.parse(data.d);
                    var tempObj = $.extend({}, overrideStatus.gridObj);
                    tempObj = overrideStatus.initializeGridPropertiesSM(tempObj, customRowInfo, currPageID, viewKey, sessionID, gridHeight);
                    xlgridloader.create(overrideStatus.controls.overrideStatusNeoGrid().attr("id"), "Test", tempObj, "");
                    $("#overrideStatusNeoGrid_MessageBoxID").css('min-height', 0);
                    overrideStatus.onGridRenderComplete();
                }
                else {
                    overrideStatus.controls.overrideStatusNoGridSection().show();
                    parent.leftMenu.showNoRecordsMsg('No Records to display.', overrideStatus.controls.overrideStatusNoGridSectionCls());
                }

                onServiceUpdated();
            },
            error: function () {
                console.log("Overrides Data cannot be fetched");
            }
        });
    },
    getSecurityOverridesDataGridSM: function () {
        onServiceUpdating();

        var currPageID = GetGUID();
        var viewKey = GetGUID();
        var sessionID = GetGUID();

        var secIDs = [];
        for (var item in overrideStatus._selectedSecIDsOrEntityCodesFromSearch) {
            secIDs.push(overrideStatus._selectedSecIDsOrEntityCodesFromSearch[item].split('|')[0]);
        }

        $.ajax({
            type: 'POST',
            url: overrideStatus._path + overrideStatus._commonServiceLocation + "/GetOverridesSecutiyDataSM",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ username: overrideStatus._username, divID: overrideStatus.controls.overrideStatusSecurityDataNeoGrid().attr("id"), currPageID: currPageID, viewKey: viewKey, sessionID: sessionID, selectedSecIds: secIDs, dateFormat: overrideStatus._cultureLongDate }),
            success: function (data) {
                var responseData = data.d;
                var customRowInfo = "";//JSON.parse(data.d.customRowInfo);
                var tempObj = $.extend({}, overrideStatus.gridObj);
                tempObj = overrideStatus.initializeGridPropertiesSecurityDataSM(tempObj, customRowInfo, currPageID, viewKey, sessionID, data.d.gridRowIds);
                xlgridloader.create(overrideStatus.controls.overrideStatusSecurityDataNeoGrid().attr("id"), "Test", tempObj, "");
                $("#overrideStatusNeoGrid_MessageBoxID").css('min-height', 0);
                overrideStatus.onGridRenderComplete();

                var attrList = overrideStatus.massageAttrData(responseData.attributeList);
                overrideStatus.createSMSelectDropDown(overrideStatus.controls.overrideStatusAttrDropDown(), attrList, true, "200", function () {
                    $("#smselect_" + overrideStatus.controls.overrideStatusAttrDropDown().attr("id")).find(".smselectanchorrun").css("font-size", "14px");
                    $("#smselect_" + overrideStatus.controls.overrideStatusAttrDropDown().attr("id")).find(".smselectimage").css("color", "#808080");
                }, "Select Attributes");

                onServiceUpdated();
            },
            error: function () {
                console.log("Overrides Security Data cannot be fetched");
            }
        });
    },
    massageAttrData: function (data) {
        var attrArr = [];
        for (var item in data) {
            var tempObj = {};
            tempObj.text = data[item].attributeName;
            tempObj.value = data[item].attributeID;
            attrArr.push(tempObj);
        }
        return attrArr;
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
    attributeViewModel: function () {
        var self = this;

        self.attributeList = ko.observableArray([]);
        self.onClickAddRow = function (obj, event) {
            var selectedAttr = smselect.getSelectedOption($("#smselect_" + overrideStatus.controls.overrideStatusAttrDropDown().attr("id")));

            var isAlreadyPresent = false;
            //To Check If Attribute already exists or not
            for (var item in overrideStatus._attrViewModel.attributeList()) {
                if (overrideStatus._attrViewModel.attributeList()[item].attributeID() === selectedAttr[0].value) {
                    isAlreadyPresent = true;
                    break;
                }
            }

            if (!isAlreadyPresent) {
                if (overrideStatus._attrViewModel.attributeList().length > 7) {
                    overrideStatus.controls.overrideStatusAttributeListSection().find(".overrideStatusAttrSlimscroll").smslimscroll({ height: overrideStatus.controls.overrideStatusAttributeListSection().height() + 'px' });
                }

                if (selectedAttr.length > 0) {
                    var rowData = {}
                    rowData.attributeID = selectedAttr[0].value;
                    rowData.attributeName = selectedAttr[0].text;
                    rowData.attributeExpiry = slideDateControl.getSelectedValue("slideDateControl_0");
                    if (rowData.attributeExpiry.toLowerCase() === 'never') {
                        overrideStatus._attrViewModel.attributeList.push(new overrideStatus.attributeRowViewModel(rowData));
                        $('#overrideErrorId').css('display', 'none');
                    }
                    else {
                        var expDt = Date.parseInvariant(rowData.attributeExpiry, os_serverSideValues.CultureShortDateFormat);
                        var todayDt = new Date(new Date().setHours(0, 0, 0, 0));
                        if (expDt >= todayDt) {
                            overrideStatus._attrViewModel.attributeList.push(new overrideStatus.attributeRowViewModel(rowData));
                            $('#overrideErrorId').css('display', 'none');
                        }
                        else {
                            $('#overrideErrorId').text("Expiry date cannot be less than Today's date");
                            $('#overrideErrorId').css('display', 'block');
                        }
                    }
                }
            }
        }
    },
    attributeRowViewModel: function (rowData) {
        var self = this;

        self.attributeName = ko.observable(rowData.attributeName);
        self.attributeID = ko.observable(rowData.attributeID);
        self.attributeExpiry = ko.observable(rowData.attributeExpiry);
        self.onClickDeleteRow = function (obj, event) {
            overrideStatus._attrViewModel.attributeList.remove(obj);
        }
    },
    getoverrideStatusGridRM: function (gridHeight) {
        onServiceUpdating();

        var currPageID = GetGUID();
        var viewKey = GetGUID();
        var sessionID = GetGUID();

        $.ajax({
            type: 'POST',
            url: overrideStatus._path + overrideStatus._commonServiceLocation + "/GetOverridesDataRM",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ username: overrideStatus._username, divID: overrideStatus.controls.overrideStatusNeoGrid().attr("id"), currPageID: currPageID, viewKey: viewKey, sessionID: sessionID, dateFormat: overrideStatus._cultureShortDate, ModuleId: overrideStatus._moduleID, dateFormatLong: overrideStatus._cultureLongDate }),
            success: function (data) {
                if (parseInt(data.d) > 0) {
                    overrideStatus.controls.overrideStatusNoGridSection().hide();
                    var customRowInfo = "";//JSON.parse(data.d);
                    var tempObj = $.extend({}, overrideStatus.gridObj);
                    tempObj = overrideStatus.initializeGridPropertiesRM(tempObj, customRowInfo, currPageID, viewKey, sessionID, gridHeight);
                    xlgridloader.create(overrideStatus.controls.overrideStatusNeoGrid().attr("id"), "Test", tempObj, "");
                    $("#overrideStatusNeoGrid_MessageBoxID").css('min-height', 0);
                    overrideStatus.onGridRenderComplete();
                }
                else {
                    overrideStatus.controls.overrideStatusNoGridSection().show();
                    parent.leftMenu.showNoRecordsMsg('No Records to display.', overrideStatus.controls.overrideStatusNoGridSectionCls());
                }

                onServiceUpdated();
            },
            error: function () {
                console.log("Overrides Data cannot be fetched");
            }
        });
    },
    getEntityOverridesDataGridRM: function () {
        onServiceUpdating();

        var currPageID = GetGUID();
        var viewKey = GetGUID();
        var sessionID = GetGUID();

        var entityCodes = [];
        for (var item in overrideStatus._selectedSecIDsOrEntityCodesFromSearch) {
            entityCodes.push(overrideStatus._selectedSecIDsOrEntityCodesFromSearch[item].split('|')[0]);
        }

        $.ajax({
            type: 'POST',
            url: overrideStatus._path + overrideStatus._commonServiceLocation + "/GetOverridesEntityDataRM",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ username: overrideStatus._username, divID: overrideStatus.controls.overrideStatusSecurityDataNeoGrid().attr("id"), currPageID: currPageID, viewKey: viewKey, sessionID: sessionID, selectedEntityIds: entityCodes, entityTypeID: overrideStatus._entityTypeID, dateFormat: overrideStatus._cultureLongDate }),
            success: function (data) {
                var responseData = data.d;
                var customRowInfo = "";//JSON.parse(data.d.customRowInfo);
                var tempObj = $.extend({}, overrideStatus.gridObj);
                tempObj = overrideStatus.initializeGridPropertiesEntityDataRM(tempObj, customRowInfo, currPageID, viewKey, sessionID, data.d.gridRowIds);
                xlgridloader.create(overrideStatus.controls.overrideStatusSecurityDataNeoGrid().attr("id"), "Test", tempObj, "");
                $("#overrideStatusNeoGrid_MessageBoxID").css('min-height', 0);
                overrideStatus.onGridRenderComplete();

                var attrList = overrideStatus.massageAttrData(responseData.attributeList);
                overrideStatus.createSMSelectDropDown(overrideStatus.controls.overrideStatusAttrDropDown(), attrList, true, "200", function () {
                    $("#smselect_" + overrideStatus.controls.overrideStatusAttrDropDown().attr("id")).find(".smselectanchorrun").css("font-size", "14px");
                    $("#smselect_" + overrideStatus.controls.overrideStatusAttrDropDown().attr("id")).find(".smselectimage").css("color", "#808080");
                }, "Select Attributes");

                onServiceUpdated();
            },
            error: function () {
                console.log("Overrides Entity Data cannot be fetched");
            }
        });
    },
    controls: {
        overrideStatusNeoGrid: function () {
            return $("#overrideStatusNeoGrid");
        },
        overrideStatusActionSection: function () {
            return $("#overrideStatusActionSection");
        },
        overrideStatusDeleteAction: function () {
            return $("#overrideStatusDeleteAction");
        },
        overrideStatusActionList: function () {
            return $("#overrideStatusActionList");
        },
        overrideStatusSecurityDataNeoGrid: function () {
            return $("#overrideStatusSecurityDataNeoGrid");
        },
        overridesFromSearchScreenSection: function () {
            return $("#overridesFromSearchScreenSection");
        },
        overrideStatusScreenSection: function () {
            return $("#overrideStatusScreenSection");
        },
        overrideStatusAttributeListSection: function () {
            return $("#overrideStatusAttributeListSection");
        },
        overrideStatusAttrDropDown: function () {
            return $("#overrideStatusAttrDropDown");
        },
        overrideStatusAddAttributeSection: function () {
            return $("#overrideStatusAddAttributeSection");
        },
        overrideStatusAttrDate: function () {
            return $("#overrideStatusAttrDate");
        },
        oversideStatusViewOverrides: function () {
            return $("#oversideStatusViewOverrides");
        },
        overridesStatusSaveOverride: function () {
            return $("#overridesStatusSaveOverride");
        },
        overrideStatusGridTitle: function () {
            return $("#overrideStatusGridTitle");
        },
        overrideStatusNoGridSection: function () {
            return $("#overrideStatusNoGridSection");
        },
        overrideStatusNoGridSectionCls: function () {
            return $(".overrideStatusNoGridSection");
        },
        overrideStatusRefreshButton: function () {
            return $("#overrideStatusRefreshButton");
        }

    },
    initializeGridPropertiesSM: function (tempObj, customRowInfo, currPageID, viewKey, sessionID, gridHeight) {
        tempObj.GridId = overrideStatus.controls.overrideStatusNeoGrid().attr("id");
        tempObj.UserId = overrideStatus._username;
        tempObj.ViewKey = viewKey;
        tempObj.CurrentPageId = currPageID;
        tempObj.SessionIdentifier = sessionID;
        tempObj.IdColumnName = "row_keys";
        tempObj.ColumnsToHide = [{ "columnName": "row_id", "isDefault": true }, { "columnName": "attributeID", "isDefault": true }, { "columnName": "table_name", "isDefault": true }, { "columnName": "id_column", "isDefault": true }];
        tempObj.DateFormat = overrideStatus._cultureLongDate;
        //tempObj.CustomRowsDataInfo = customRowInfo;
        if (gridHeight !== undefined) {
            tempObj.Height = gridHeight + "px";
        }
        return tempObj;
    },
    initializeGridPropertiesSecurityDataSM: function (tempObj, customRowInfo, currPageID, viewKey, sessionID, gridPrimaryColumn) {
        tempObj.GridId = overrideStatus.controls.overrideStatusSecurityDataNeoGrid().attr("id");
        tempObj.UserId = overrideStatus._username;
        tempObj.ViewKey = viewKey;
        tempObj.CurrentPageId = currPageID;
        tempObj.SessionIdentifier = sessionID;
        tempObj.IdColumnName = "row_id";
        tempObj.ColumnsToHide = [{ "columnName": "row_id", "isDefault": true }, { "columnName": "row_keys", "isDefault": true }];
        tempObj.Height = "210px";
        tempObj.DateFormat = overrideStatus._cultureLongDate;
        //tempObj.CustomRowsDataInfo = customRowInfo;

        //For Checking all rows by default
        var checkBoxInfo = {};
        for (var item in gridPrimaryColumn) {
            checkBoxInfo[gridPrimaryColumn[item].toString()] = { "checked": "checked" };
        }
        tempObj.CheckBoxInfo.ItemAttribute = checkBoxInfo;
        return tempObj;
    },
    initializeGridPropertiesEntityDataRM: function (tempObj, customRowInfo, currPageID, viewKey, sessionID, gridPrimaryColumn) {
        tempObj.GridId = overrideStatus.controls.overrideStatusSecurityDataNeoGrid().attr("id");
        tempObj.UserId = overrideStatus._username;
        tempObj.ViewKey = viewKey;
        tempObj.CurrentPageId = currPageID;
        tempObj.SessionIdentifier = sessionID;
        tempObj.IdColumnName = "row_id";
        tempObj.ColumnsToHide = [{ "columnName": "row_id", "isDefault": true }, { "columnName": "row_keys", "isDefault": true }, { "columnName": "Select Entity", "isDefault": true }, { "columnName": "id", "isDefault": true }, { "columnName": "Update", "isDefault": true }, { "columnName": "Is Active", "isDefault": true }, { "columnName": "loading_time", "isDefault": true }, { "columnName": "TableIndex", "isDefault": true }, { "columnName": "entity_type_id", "isDefault": true }, { "columnName": "entity_display_name", "isDefault": true }];
        tempObj.Height = "210px";
        tempObj.DateFormat = overrideStatus._cultureLongDate;
        //tempObj.CustomRowsDataInfo = customRowInfo;

        //For Checking all rows by default
        var checkBoxInfo = {};
        for (var item in gridPrimaryColumn) {
            checkBoxInfo[gridPrimaryColumn[item].toString()] = { "checked": "checked" };
        }
        tempObj.CheckBoxInfo.ItemAttribute = checkBoxInfo;
        return tempObj;
    },
    initializeGridPropertiesRM: function (tempObj, customRowInfo, currPageID, viewKey, sessionID, gridHeight) {
        tempObj.GridId = overrideStatus.controls.overrideStatusNeoGrid().attr("id");
        tempObj.UserId = overrideStatus._username;
        tempObj.ViewKey = viewKey;
        tempObj.CurrentPageId = currPageID;
        tempObj.SessionIdentifier = sessionID;
        tempObj.ColumnsToHide = [{ "columnName": "row_keys", "isDefault": true }];
        tempObj.DateFormat = overrideStatus._cultureLongDate;
        //tempObj.CustomRowsDataInfo = customRowInfo;
        if (gridHeight !== undefined) {
            tempObj.Height = gridHeight + "px";
        }
        return tempObj;
    },
    onGridRenderComplete: function () {
        if (overrideStatus._fromScreenName !== null && overrideStatus._fromScreenName.toLowerCase() === "search") {
            //For Security/Entity Info Grid
            var grid = $find(overrideStatus.controls.overrideStatusSecurityDataNeoGrid().attr("id"));
            if (grid != null) {
                this.onGridServiceCompleted = Function.createDelegate(this, overrideStatus.getCheckedRowsInfoOverridesDataFromSearch);
                grid.eventHandlerManager.addServiceCompletedEventHandler(this.onGridServiceCompleted);
            }

            //For Override Status Grid
            var gridOverrideStatus = $find(overrideStatus.controls.overrideStatusNeoGrid().attr("id"));
            if (gridOverrideStatus != null) {
                this.onGridServiceCompleted = Function.createDelegate(this, overrideStatus.getCheckedRowsInfoOverrideStatus);
                gridOverrideStatus.eventHandlerManager.addServiceCompletedEventHandler(this.onGridServiceCompleted);
            }
        }
        else {
            var grid = $find(overrideStatus.controls.overrideStatusNeoGrid().attr("id"));
            resizeGridFinal(overrideStatus.controls.overrideStatusNeoGrid().attr("id"), os_serverSideControls.TopPanel, os_serverSideControls.MiddlePanel, os_serverSideControls.BottomPanel, 0, 210);

            if (grid != null) {
                this.onGridServiceCompleted = Function.createDelegate(this, overrideStatus.getCheckedRowsInfoOverrideStatus);
                grid.eventHandlerManager.addServiceCompletedEventHandler(this.onGridServiceCompleted);
            }
        }
    },
    getCheckedRowsInfoOverrideStatus: function (info) {
        overrideStatus._selectedRowsCount = info.serializedInfo.CheckedRowIndices.length;
        if (overrideStatus._selectedRowsCount > 0) {
            overrideStatus.controls.overrideStatusDeleteAction().removeClass("overrideStatusDisabled");
        }
        else {
            if (!overrideStatus.controls.overrideStatusDeleteAction().hasClass("overrideStatusDisabled")) {
                overrideStatus.controls.overrideStatusDeleteAction().addClass("overrideStatusDisabled");
            }
        }

        if (info.eventType === com.ivp.rad.controls.neogrid.scripts.EventType.evenT_ROW_WITH_CHECKBOX_SELECTED || info.eventType === com.ivp.rad.controls.neogrid.scripts.EventType.evenT_ROW_WITH_CHECKBOX_SELECTED_ALL || info.eventType === com.ivp.rad.controls.neogrid.scripts.EventType.evenT_ROW_WITH_CHECKBOX_UNSELECTED || info.eventType === com.ivp.rad.controls.neogrid.scripts.EventType.evenT_ROW_WITH_CHECKBOX_UNSELECTED_ALL || info.eventType === com.ivp.rad.controls.neogrid.scripts.EventType.evenT_ROW_WITH_GROUP_HEADER_CHECKBOX_SELECTED || info.eventType === com.ivp.rad.controls.neogrid.scripts.EventType.evenT_ROW_WITH_GROUP_HEADER_CHECKBOX_UNSELECTED) {
            var cacheKey = $find(overrideStatus.controls.overrideStatusNeoGrid().attr("id")).get_CacheKey();
            $.ajax({
                url: overrideStatus._path + overrideStatus._commonServiceLocation + '/OverrideStatusGetCheckedRowsData',
                type: 'POST',
                data: JSON.stringify({ 'cacheKey': cacheKey }),
                contentType: "application/json; charset=utf-8",
                dataType: 'json',
                success: function (data) {
                    var checkedData = data.d;
                    if (checkedData.ruleID !== null && checkedData.ruleID !== undefined) {
                        var ruleIDArr = checkedData.ruleID.split("|");
                        var entityCodeArr = checkedData.entityCode.split("|");
                        var typeIDArr = checkedData.typeID.split("|");
                        var attrIDArr = checkedData.attributeID.split("|");
                        var tableNameArr = checkedData.tableName.split("|");
                        var attributeDisplayNameArr = checkedData.attributeDisplayName.split("|");

                        overrideStatus._checkedRowsOverrideStatus = [];

                        for (var i in ruleIDArr) {
                            var tempRuleID = ruleIDArr[i];
                            var tempEntityCode = entityCodeArr[i];
                            var tempTypeID = typeIDArr[i];
                            var tempAttrID = attrIDArr[i];
                            var tempTableName = tableNameArr[i];
                            var tempAttrDisplayName = attributeDisplayNameArr[i];

                            if (tempRuleID !== "") {
                                overrideStatus._checkedRowsOverrideStatus.push({
                                    typeID: (tempTypeID !== null && tempTypeID !== undefined) ? tempTypeID : "",
                                    attrID: (tempAttrID !== null && tempAttrID !== undefined) ? tempAttrID : "",
                                    overrideID: (tempRuleID !== null && tempRuleID !== undefined) ? tempRuleID : "",
                                    tableName: (tempTableName !== null && tempTableName !== undefined) ? tempTableName : "",
                                    entityCode: (tempEntityCode !== null && tempEntityCode !== undefined) ? tempEntityCode : "",
                                    attributeDisplayName: (tempAttrDisplayName !== null && tempAttrDisplayName !== undefined) ? tempAttrDisplayName : "",
                                });
                            }
                        }
                    }
                },
                error: function (e) {
                    console.log(e.message);
                }
            });
        }
    },
    getCheckedRowsInfoOverridesDataFromSearch: function (info) {
        //overrideStatus._selectedSecuritiesOrEntitesFromGrid = info.serializedInfo.CheckedRowIndices.length;

        if (info.eventType === com.ivp.rad.controls.neogrid.scripts.EventType.evenT_ROW_WITH_CHECKBOX_SELECTED || info.eventType === com.ivp.rad.controls.neogrid.scripts.EventType.evenT_ROW_WITH_CHECKBOX_SELECTED_ALL || info.eventType === com.ivp.rad.controls.neogrid.scripts.EventType.evenT_ROW_WITH_CHECKBOX_UNSELECTED || info.eventType === com.ivp.rad.controls.neogrid.scripts.EventType.evenT_ROW_WITH_CHECKBOX_UNSELECTED_ALL || info.eventType === com.ivp.rad.controls.neogrid.scripts.EventType.evenT_ROW_WITH_GROUP_HEADER_CHECKBOX_SELECTED || info.eventType === com.ivp.rad.controls.neogrid.scripts.EventType.evenT_ROW_WITH_GROUP_HEADER_CHECKBOX_UNSELECTED) {
            var cacheKey = $find(overrideStatus.controls.overrideStatusSecurityDataNeoGrid().attr("id")).get_CacheKey();
            $.ajax({
                url: overrideStatus._path + overrideStatus._commonServiceLocation + '/OverrideDataGetCheckedRowsData',
                type: 'POST',
                data: JSON.stringify({ 'cacheKey': cacheKey, 'productName': overrideStatus._prodName }),
                contentType: "application/json; charset=utf-8",
                dataType: 'json',
                success: function (data) {
                    if (overrideStatus._prodName.toLowerCase() === "secmaster") {
                        overrideStatus._selectedSecuritiesOrEntitesFromGrid = data.d;
                    }
                    else if (overrideStatus._prodName.toLowerCase() === "refmaster") {
                        overrideStatus._selectedSecuritiesOrEntitesFromGrid = data.d;
                    }
                },
                error: function (e) {
                    console.log(e.message);
                }
            });
        }
    },
    onClickGrid: function (event) {
        event = event || window.event;
        var src = $(event.target);

        if (src.prop('id') != null && (src.prop('tagName').toUpperCase() == 'DIV' || src.prop('tagName').toUpperCase() == 'SPAN')) {
            if (src.prop('id').toLowerCase().indexOf('divsecurityid') != -1) {
                var securityId = src.attr('id').split('divSecurityId')[1];

                if (overrideStatus._editPrivilege)
                    SecMasterJSCommon.SMSCommons.openSecurity(true, securityId, '', true, true, false, '', '', false, false, SecMasterJSCommon.SMCreateUpdateOpenByDefault.None, 0, "", 3);
                else if (overrideStatus._viewPrivilege) {
                    SecMasterJSCommon.SMSCommons.openSecurity(false, securityId, '', true, true, false, '', '', false, false, SecMasterJSCommon.SMCreateUpdateOpenByDefault.None, 0, "", 3);
                }
                else { }

            }
            else if (src.prop('id').toLowerCase().indexOf('diventitycode') != -1) {
                if (overrideStatus._editPrivilege) {
                    var entityCode = src.attr('id').split('divEntityCode')[1];
                    //Open Create Page
                    var containerPage = "App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?";
                    var queryString = "entityCode=" + entityCode + "&";
                    var uniqueId = "unique_edit_" + GetGUID();
                    var url = containerPage + queryString.trim() + "pageIdentifier=UpdateEntityFromSearch&tabFrameId=" + uniqueId + "&RMTabUniqueID=" + uniqueId;
                    RefMasterJSCommon.RMSCommons.createTab("RefM_EditEntity", url, uniqueId, entityCode);
                }
                else if (overrideStatus._viewPrivilege) {
                    var entityCode = src.attr('id').split('divEntityCode')[1];
                    //Open Create Page
                    var containerPage = "App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?";
                    var queryString = "entityCode=" + entityCode + "&";
                    var uniqueId = "unique_view_" + GetGUID();
                    var url = containerPage + queryString.trim() + "pageIdentifier=ViewEntityFromSearch&tabFrameId=" + uniqueId + "&RMTabUniqueID=" + uniqueId;
                    RefMasterJSCommon.RMSCommons.createTab("RefM_ViewEntity", url, uniqueId, entityCode);
                }
                else {
                    //Do nothing
                }
            }
        }
    },
    onClickViewOverrides: function () {
        if (overrideStatus._prodName.toLowerCase() === "secmaster") {

            if (!overrideStatus.controls.overrideStatusScreenSection().hasClass("overrideStatusMinimizedDiv")) {
                $("body").append("<div class='overrideStatusOverlay'></div>");
                overrideStatus.controls.overrideStatusScreenSection().addClass('overrideStatusMinimizedDiv');
                overrideStatus.controls.overrideStatusScreenSection().css('display', '');
                overrideStatus.controls.overrideStatusScreenSection().css('left', ((overrideStatus._windowWidth - 1200) / 2));
                overrideStatus.controls.overrideStatusScreenSection().addClass('overrideStatusPopupBorder');
                overrideStatus.controls.overrideStatusScreenSection().animate({ "height": (overrideStatus._windowHeight - 100), "top": "50px" }, function () {
                    overrideStatus.secMasterInitOverrideStatusScreen((overrideStatus._windowHeight - 260));
                    overrideStatus.controls.overrideStatusScreenSection().append("<i class='fa fa-times overrideStatusClosePopup'></i>");

                    //Close Popup Click Handler
                    overrideStatus.controls.overrideStatusScreenSection().find(".overrideStatusClosePopup").unbind('click').bind('click', function () {
                        overrideStatus.controls.overrideStatusScreenSection().animate({ "height": 0, "top": "150px" }, function () {
                            overrideStatus.destroyGrid();
                            overrideStatus.controls.overrideStatusScreenSection().removeClass('overrideStatusMinimizedDiv');
                            overrideStatus.controls.overrideStatusScreenSection().css('display', 'none');
                            overrideStatus.controls.overrideStatusScreenSection().removeClass('overrideStatusPopupBorder');
                            $("body").find(".overrideStatusOverlay").remove();
                        });
                    });
                });
            }
        }
        else if (overrideStatus._prodName.toLowerCase() === "refmaster") {
            if (!overrideStatus.controls.overrideStatusScreenSection().hasClass("overrideStatusMinimizedDiv")) {
                $("body").append("<div class='overrideStatusOverlay'></div>");
                overrideStatus.controls.overrideStatusScreenSection().addClass('overrideStatusMinimizedDiv');
                overrideStatus.controls.overrideStatusScreenSection().css('display', '');
                overrideStatus.controls.overrideStatusScreenSection().css('left', ((overrideStatus._windowWidth - 1200) / 2));
                overrideStatus.controls.overrideStatusScreenSection().addClass('overrideStatusPopupBorder');
                overrideStatus.controls.overrideStatusScreenSection().animate({ "height": (overrideStatus._windowHeight - 100), "top": "50px" }, function () {
                    overrideStatus.refMasterInitOverrideStatusScreen((overrideStatus._windowHeight - 260));
                    overrideStatus.controls.overrideStatusScreenSection().append("<i class='fa fa-times overrideStatusClosePopup'></i>");

                    //Close Popup Click Handler
                    overrideStatus.controls.overrideStatusScreenSection().find(".overrideStatusClosePopup").unbind('click').bind('click', function () {
                        overrideStatus.controls.overrideStatusScreenSection().animate({ "height": 0, "top": "150px" }, function () {
                            overrideStatus.destroyGrid();
                            overrideStatus.controls.overrideStatusScreenSection().removeClass('overrideStatusMinimizedDiv');
                            overrideStatus.controls.overrideStatusScreenSection().css('display', 'none');
                            overrideStatus.controls.overrideStatusScreenSection().removeClass('overrideStatusPopupBorder');
                            $("body").find(".overrideStatusOverlay").remove();
                        });
                    });
                });
            }
        }
    },
    onClickSaveOverride: function () {

        if (overrideStatus._prodName.toLowerCase() === "secmaster") {
            if (overrideStatus._selectedSecuritiesOrEntitesFromGrid.length > 0) {
                var overrideData = [];
                for (var item in overrideStatus._attrViewModel.attributeList()) {
                    var attributeID = overrideStatus._attrViewModel.attributeList()[item].attributeID();
                    var expiresOn = overrideStatus._attrViewModel.attributeList()[item].attributeExpiry();
                    debugger;
                    if (expiresOn.toLowerCase() !== "never") {
                        var currDate = Date.parseInvariant(expiresOn, os_serverSideValues.CultureShortDateFormat);
                        expiresOn = currDate.format(os_serverSideValues.ServerShortDateFormat);
                        expiryDate = expiresOn;
                    }
                    else {
                        expiryDate = null;
                    }


                    //overrideData.push({ "AttributeId": attributeID, "AttributeLabelId": "", "DisplayName": "", "RuleOff": true, "ExpiresOn": expiresOn, "OverrideCreatedOn": "", "OverrideCreatedBy": overrideStatus._username });
                    overrideData.push({ "AttributeId": attributeID, "AttributeLabelId": "", "ExpiresOn": expiryDate, "OverrideCreatedOn": "", "OverrideCreatedBy": overrideStatus._username });
                }

                if (overrideData.length > 0) {
                    var listOfSecurities = [];
                    for (var selectedSec in overrideStatus._selectedSecuritiesOrEntitesFromGrid) {
                        var finalObj = {};
                        finalObj["Key"] = overrideStatus._selectedSecuritiesOrEntitesFromGrid[selectedSec];
                        finalObj["Value"] = overrideData;
                        listOfSecurities.push(finalObj);
                    }

                    $.ajax({
                        type: 'POST',
                        url: overrideStatus._path + overrideStatus._commonServiceLocation + "/SaveBulkOverrideSM",
                        contentType: "application/json",
                        dataType: "json",
                        data: JSON.stringify({ username: overrideStatus._username, uniqueId: GetGUID(), attrInfo: listOfSecurities }),
                        success: function (data) {
                            if (data.d.indexOf("|") !== -1) {
                                var status = data.d.split("|")[0];
                                if (status === "1") {
                                    overrideStatus.displayPopup(data.d.split("|")[1], "success")
                                }
                                else {
                                    overrideStatus.displayPopup(data.d.split("|")[1], "failure")
                                }
                            }
                        },
                        error: function () {
                            console.log("Attributes cannot be saved");
                        }
                    });
                }
                else
                    overrideStatus.displayPopup("Please select atleast one attribute to override", "failure");
            }
            else {
                overrideStatus.displayPopup("Please select atleast one Security Id", "failure")
            }
        }
        else if (overrideStatus._prodName.toLowerCase() === "refmaster") {
            if (overrideStatus._selectedSecuritiesOrEntitesFromGrid.length > 0) {
                var overrideData = [];

                var attrInfo = [];
                for (var item in overrideStatus._attrViewModel.attributeList()) {
                    var attributeName = overrideStatus._attrViewModel.attributeList()[item].attributeName();
                    var expiresOn = overrideStatus._attrViewModel.attributeList()[item].attributeExpiry();
                    if (expiresOn.toLowerCase() !== "never") {
                        var currDate = Date.parseInvariant(expiresOn, os_serverSideValues.CultureShortDateFormat);
                        expiresOn = currDate.format(os_serverSideValues.ServerShortDateFormat);
                        expiryDate = expiresOn;
                    }
                    else {
                        expiryDate = null;
                    }
                    attrInfo.push({ "AttributeName": attributeName, "AttributeValue": "abc", "DeleteExisting": false, "LockedUntil": expiryDate });
                }

                if (attrInfo.length > 0) {
                    for (var selectedSec in overrideStatus._selectedSecuritiesOrEntitesFromGrid) {
                        var overrideObj = {};
                        overrideObj["Attributes"] = attrInfo;
                        overrideObj["DeleteExisting"] = false;
                        if ((overrideStatus._selectedSecuritiesOrEntitesFromGrid[selectedSec]).includes('|')) {
                            var entitycode = (overrideStatus._selectedSecuritiesOrEntitesFromGrid[selectedSec]).split('||')[1];
                            overrideObj["EntityCode"] = entitycode;
                        }
                        else
                            overrideObj["EntityCode"] = overrideStatus._selectedSecuritiesOrEntitesFromGrid[selectedSec];
                        overrideData.push(overrideObj);
                    }

                    $.ajax({
                        type: 'POST',
                        url: overrideStatus._path + overrideStatus._commonServiceLocation + "/SaveBulkOverrideRM",
                        contentType: "application/json",
                        dataType: "json",
                        data: JSON.stringify({ username: overrideStatus._username, attrInfo: overrideData }),
                        success: function (data) {
                            //console.log(data);
                            if (data.d.indexOf("|") !== -1) {
                                var status = data.d.split("|")[0];
                                if (status === "1") {
                                    overrideStatus.displayPopup(data.d.split("|")[1], "success")
                                }
                                else {
                                    overrideStatus.displayPopup(data.d.split("|")[1], "failure")
                                }
                            }
                        },
                        error: function () {
                            console.log("Attributes cannot be saved");
                        }
                    });
                }
                else
                    overrideStatus.displayPopup("Please select atleast one attribute to override", "failure");
            }
            else {
                overrideStatus.displayPopup("Please select atleast one Entity Code", "failure")
            }
        }

    },
    refreshGridRM: function (gridHeight) {
        if (gridHeight !== undefined) {
            if (overrideStatus._moduleID == 6)
                overrideStatus.refMasterInitOverrideStatusScreen(gridHeight);
            if (overrideStatus._moduleID == 18)
                overrideStatus.fundMasterInitOverrideStatusScreen(gridHeight);
            if (overrideStatus._moduleID == 20)
                overrideStatus.partyMasterInitOverrideStatusScreen(gridHeight);
        }
        else {
            if (overrideStatus._moduleID == 6)
                overrideStatus.refMasterInitOverrideStatusScreen(gridHeight);
            if (overrideStatus._moduleID == 18)
                overrideStatus.fundMasterInitOverrideStatusScreen(gridHeight);
            if (overrideStatus._moduleID == 20)
                overrideStatus.partyMasterInitOverrideStatusScreen(gridHeight);
        }
    },
    refreshGridSM: function () {
        if (overrideStatus._fromScreenName !== null && overrideStatus._fromScreenName.toLowerCase() === "search") {
            overrideStatus.secMasterInitOverrideStatusScreen((overrideStatus._windowHeight - 360));
        }
        else {
            overrideStatus.secMasterInitOverrideStatusScreen();
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

        var left = (overrideStatus._windowWidth / 2) - 200;
        var msgPopup = $("#os_messagePopup");
        msgPopup.find(".overrideStatuspopupTitle").text(title).css("color", color);
        msgPopup.find(".overrideStatusMessage").text(message);
        msgPopup.find("img").attr("src", imageSrc);
        msgPopup.css("border-top", borderTop);
        msgPopup.css("left", left + "px");
        msgPopup.css("top", "-" + msgPopup.height() + "px");
        msgPopup[0].style.display = "";
        msgPopup.animate({ top: "0px" }, 500);

        msgPopup.find(".overrideStatusPopupCloseBtn").unbind("click").bind("click", function (e) {
            msgPopup.animate({ top: "-" + msgPopup.height() + "px" }, 500, function () {
                msgPopup[0].style.display = "none";
            });
        });
    },
    gridObj: {
        "SelectRecordsAcrossAllPages": true,
        "ViewKey": "", //to be overriden
        "CacheGriddata": true,
        "CurrentPageId": "", //to be overriden
        "SessionIdentifier": "", //to be overriden
        "UserId": "",
        "CheckBoxInfo": {},
        "Height": "400px",
        "ColumnsWithoutClientSideFunctionality": [],
        "ColumnsNotToSum": [],
        "RequireEditGrid": false,
        "RequireEditableRow": false,
        "IdColumnName": "row_id", //to be overriden
        "TableName": "overrideStatusTable",
        "PageSize": 100,
        "RequirePaging": false,
        "RequireInfiniteScroll": true,
        "CollapseAllGroupHeader": false,
        "GridTheme": 2,
        "HideFooter": false,
        "DoNotExpand": false,
        "ItemText": "Number of Records",
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

//$(document).ready(function () {
//    overrideStatus.init();


//    /*$.ajax({
//        type: 'POST',
//        url: overrideStatus._path + "/CommonUI/BaseUserControls/Service/ExceptionManagerService.svc" + "/SMGetExceptionCountData",
//        contentType: "application/json",
//        dataType: "json",
//        data: JSON.stringify({
//            "objSMExceptionFilterInfo": {
//                "ListSecurityType": null,
//                "ListAttribute": null,
//                "ListExceptionType": null,
//                "ListSectypeTableId": null,
//                "UserName": "admin",
//                "StartDate": "",
//                "EndDate": "",
//                "ListSecurityId": null,
//                "Resolved": 0,
//                "Suppressed": 0,
//                "TagName": "-1"
//            }
//        }),
//        success: function (data) {

//        },
//        error: function () {
//            console.log("Overrides Data cannot be fetched");
//        }
//    });*/
//});

/*No Right Click on the Page*/
function clickIE() {
    if (document.all) { }
}
function clickNS(e) {
    if (document.layers || (document.getElementById && !document.all)) {
        if (e.which == 2 || e.which == 3) { }
    }
}
if (document.layers) {
    document.captureEvents(Event.MOUSEDOWN); document.onmousedown = clickNS;
}
else {
    document.oncontextmenu = clickIE;
}
document.oncontextmenu = new Function("return false")


//"SelectRecordsAcrossAllPages": null,
//        "ViewKey": "",
//        "CacheGriddata": true,
//        "CurrentPageId": "",
//        "SessionIdentifier": "",
//        "UserId": "",
//        "CheckBoxInfo": {},
//        "Height": "400px",
//        "ColumnsWithoutClientSideFunctionality": [],
//        "ColumnsNotToSum": [],
//        "RequireEditGrid": false,
//        "RequireEditableRow": false,
//        "IdColumnName": "row_id",
//        "TableName": "overrideStatusTable",
//        "PageSize": 100,
//        "RequirePaging": false,
//        "RequireInfiniteScroll": true,
//        "CollapseAllGroupHeader": false,
//        "GridTheme": 2,
//        "DoNotExpand": false,
//        "ItemText": "Number of Records",
//        "DoNotRearrangeColumn": true,
//        "RequireGrouping": true,
//        "RequireMathematicalOperations": false,
//        "RequireSelectedRows": true,
//        "RequireEditGrid": false,
//        "RequireExportToExcel": true,
//        "RequireSearch": true,
//        "RequireSort": true,
//        "RequireFreezeColumns": false,
//        "RequireHideColumns": true,
//        "RequireColumnSwap": false,
//        "RequireFilter": true,
//        "RequireGroupExpandCollapse": false,
//        "RequireResizing": true,
//        "RequireLayouts": false,
//        "RequireRuleBasedColoring": false,
//        "RequireGroupHeaderCheckbox": false,
//        "RequireViews": false,
//        "ShowRecordCountOnHeader": false,
//        "ShowAggregationOnHeader": false,
//        "CssExportRows": "xlneoexportToExcel",
//        "DateFormat": "yyyy/MM/dd"