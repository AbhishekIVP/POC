var srmRuleCatalog = (function () {
    var srmRuleCatalog;
    this._controlIdInfo = null;
    String.prototype.replaceAll = function (find, replace) {
        return this.replace(new RegExp(find, 'g'), replace);
    }
    var runTimeModule;
    var runTimeSubModule;
    var runTimeButtonText;
    var userName;
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');


    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    function SRMRuleCatalog() {
    }
    srmRuleCatalog = new SRMRuleCatalog();

    SRMRuleCatalog.prototype.SMRuleCataLogRunTimeStatus = function SMRuleCataLogRunTimeStatus(controlInfo) {
        $(function () {
            srmRuleCatalog._controlIdInfo = eval("(" + controlInfo + ")");
            runTimeModule = srmRuleCatalog._controlIdInfo.moduleNameFromUrl;
            runTimeSubModule = srmRuleCatalog._controlIdInfo.subModuleNameFromUrl;
            userName = srmRuleCatalog._controlIdInfo.userName;
            var statusGridDiv = $("#GridBindingPanelForRuleCataLog");
            var k = 0;
            var buttons = statusGridDiv[0].parentElement.parentElement.getElementsByClassName('smRuleCataLogButton');
            for (i = 0; i < buttons.length; i++) {
                //if ((buttons[i].children[0].attributes.rule_type.value != runTimeModule)) {

                // buttons[i].children[0].style.display = "none";
                //buttons[i].children[1].style.diplay = "none";
                //  buttons[i].children[1].children[0].style.display = "none";
                if (k == 0) {
                    k++;
                    //  buttons[i].children[0].style.marginLeft = "380px";
                    buttons[i].children[0].style.marginTop = "35px";
                    buttons[i].children[0].style.opacity = 1;
                    // buttons[i].children[1].children[0].style.left = "470.5px";
                } else {

                    buttons[i].children[1].children[0].style.opacity = 0;

                }

            }

            srmRuleCatalog.Init();
        });
    }
    function RefreshStatusGrisData1(types, ruleTypes, runtimeTextButton) {

        var params = {};
        params.module = runTimeModule;
        if (runtimeTextButton != null) {
            params.tab = runtimeTextButton;
        } else {
            params.tab = runTimeSubModule;
        }
        params.ruleTypes = ruleTypes;
        if (types == null || isNaN(types[0]))
            types = [];
        params.types = types;
        params.userName = userName;
        // var resultGrid = CallCommonServiceMethod('GetBulkUploadStatusData', params, onSuccess_GetBulkUploadStatusData, OnFailure, null, false);
        var resultGrid = CallCommonServiceMethod('GetRules', params, onSuccess_GetBulkUploadStatusData, OnFailure, null, false);
        //}

    }
    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }
    SRMRuleCatalog.prototype.GetData = function GetData(types, ruleTypes, runtimeButtonText) {
        RefreshStatusGrisData1(types, ruleTypes, runtimeButtonText);
    }

    SRMRuleCatalog.prototype.Init = function Init() {
        srmRuleCatalog.attachEventHandlers();

        bindGrid();
        if ($('#hdnId').val().trim() != '') {
            var tempIds = [];
            tempIds.push($('#hdnId').val().trim());
            srmRuleCatalog.GetData(tempIds, null);
            $('.srm_panelTopSections', window.parent.document).css('display', 'none');
        }
        else
            srmRuleCatalog.GetData(null, null);
        //srmRuleCatalog.changeLookAndFeel();
    }

    SRMRuleCatalog.prototype.changeLookAndFeel = function changeLookAndFeel() {
        var statusGridDiv = $("#GridBindingPanelForRuleCataLog");
        var k = 0;
        var buttons = statusGridDiv[0].parentElement.parentElement.getElementsByClassName('smRuleCataLogButton');
        for (i = 0; i < buttons.length; i++) {
            if ((buttons[i].children[0].attributes.rule_type.value != runTimeModule)) {

                buttons[i].children[0].style.display = "none";
                //buttons[i].children[1].style.diplay = "none";
                buttons[i].children[1].children[0].style.display = "none";
            } else if (k == 0) {
                k++;
                //  buttons[i].children[0].style.marginLeft = "380px";
                buttons[i].children[0].style.marginTop = "35px";
                buttons[i].children[0].style.opacity = 1;
                // buttons[i].children[1].children[0].style.left = "470.5px";
            } else {

                buttons[i].children[1].children[0].style.opacity = 0;

            }

        }


    }

    function OnFailure(err) {
        console.log(err)
    }

    function onSuccess_GetBulkUploadStatusData(result) {
        if (result.d != null) {
            result = result.d;
            var k = 0;

            var statusGridDiv = $("#GridBindingPanelForRuleCataLog");

            statusGridDiv.empty();

            if (result.rowCount > 0) {
                statusGridDiv.html("<div id='" + result.gridID + "' class='bulkUploadGrid'></div>");

                var gridInfo = {
                    "SelectRecordsAcrossAllPages": null,
                    "ViewKey": result.viewKey,
                    "CacheGriddata": true,
                    "GridId": result.gridID,
                    "CurrentPageId": result.currentPageId,
                    "SessionIdentifier": result.sessionIdentifier,
                    "UserId": srmRuleCatalog._controlIdInfo.userName,
                    "Height": "350px",
                    "ColumnsWithoutClientSideFunctionality": [],
                    "ColumnsNotToSum": [],
                    "RequireEditGrid": false,
                    "RequireEditableRow": false,
                    "IdColumnName": "row_id",
                    "KeyColumns": { 0: "row_keys" },
                    "TableName": "DataTable",
                    "PageSize": 50,
                    "RequirePaging": false,
                    "RequireInfiniteScroll": true,
                    "CollapseAllGroupHeader": false,
                    "GridTheme": 2,
                    "DoNotExpand": false,
                    "ItemText": "Number of Rows",
                    "DoNotRearrangeColumn": true,
                    "RequireGrouping": true,
                    "RequireFilter": true,
                    "RequireSort": true,
                    "RequireMathematicalOperations": false,
                    "RequireSelectedRows": false,
                    "RequireExportToExcel": true,
                    "RequireSearch": true,
                    "RequireFreezeColumns": false,
                    "RequireHideColumns": true,
                    "RequireColumnSwap": true,
                    "RequireGroupExpandCollapse": true,
                    "RequireResizing": true,
                    "RequireLayouts": false,
                    "RequireGroupHeaderCheckbox": false,
                    "RequireRuleBasedColoring": false,
                    "RequireExportToPdf": false,
                    "ShowRecordCountOnHeader": false,
                    "ShowAggregationOnHeader": true,
                    "ColumnsNotToSum": [],
                    "CheckBoxInfo": null,
                    "RaiseGridCallBackBeforeExecute": "",
                    "RaiseGridRenderComplete": "",
                    "DefaultGroupedAndSortedColumns": [],
                    "ColumnsToHide": [{ "columnName": "row_id", "isDefault": true }, { "columnName": "row_keys", "isDefault": true }],
                    "CustomRowsDataInfo": $.parseJSON(result.customInfoScript),
                    "CustomFormatInfoClientSide": {}
                }

                xlgridloader.create(result.gridID, result.gridID, gridInfo, "");
                //onGridRenderComplete();
                //  ResizeGrid("GridPanelMiddle", true, "GridPanelTop", "GridPanelMiddle", "", bulkuploaddocuments._controlIdInfo.DivGridStatusId);
                resizeGridFinal(result.gridID, '', '', '', 0, 250);
                //resizeGridFinal('GridBindingPanelForRuleCataLog', '', 'RuleCataLogMainGrid', '', 0, 210);
            }
            else
                statusGridDiv.html("<div style='border: 1px solid #ccc; width: 99%; padding: 5px; font-weight: bold;'>There are no records in the grid.</div>");
        }
        onServiceUpdated();
    }

    function bindGrid() {
        var parms = {
        };
        parms.userName = userName;
        parms.moduleName = runTimeModule;
        var selectedDataForEnity = "";
        var rulecCatalogBasicInfo = ExecuteSynchronously(path + '/BaseUserControls/Service/CommonService.svc', 'GetRuleCatalogFilterInfo', parms);
        var filterData = [];
        var selectedData = "";
        if (parms.moduleName == "SECURITY_MASTER") {
            if (srmRuleCatalog._controlIdInfo == "SECURITY_MASTER") {
                selectedData = $('#hdnId').val().trim();
            }
            else {
                for (var i = 0; i < rulecCatalogBasicInfo.d.Security.length; i++) {

                    if (i == rulecCatalogBasicInfo.d.Security.length - 1) {
                        selectedData = selectedData + rulecCatalogBasicInfo.d.Security[i].SecurityTypeId;
                    } else {
                        selectedData = selectedData + rulecCatalogBasicInfo.d.Security[i].SecurityTypeId + ",";
                    }
                }
            }
        }
        else {
            if (srmRuleCatalog._controlIdInfo != "SECURITY_MASTER") {
                selectedDataForEnity = $('#hdnId').val().trim();
            }
            else {
                for (var i = 0; i < rulecCatalogBasicInfo.d.Entity.length; i++) {

                    if (i == rulecCatalogBasicInfo.d.Entity.length - 1) {
                        selectedDataForEnity = selectedDataForEnity + rulecCatalogBasicInfo.d.Entity[i].EntityTypeId;
                    } else {
                        selectedDataForEnity = selectedDataForEnity + rulecCatalogBasicInfo.d.Entity[i].EntityTypeId + ",";
                    }
                }
            }
        }
        var selectedDataForRules = "";

        for (var i = 0; i < rulecCatalogBasicInfo.d.Rules.length; i++) {

            if (i == rulecCatalogBasicInfo.d.Rules.length - 1) {
                selectedDataForRules = selectedDataForRules + rulecCatalogBasicInfo.d.Rules[i].RuleName;
            } else {
                selectedDataForRules = selectedDataForRules + rulecCatalogBasicInfo.d.Rules[i].RuleName + ",";
            }
        }


        if (parms.moduleName == "SECURITY_MASTER") {
            filterData.push(processFilters(rulecCatalogBasicInfo.d.Security, 'SecurityCB', 'Security Types', selectedData, 'SecurityTypeId', 'SecurityTypeName', 'checkbox', 'SecurityTypeId', false, false, "Select All", "down"));
        } else {
            filterData.push(processFilters(rulecCatalogBasicInfo.d.Entity, 'EntityCB', 'Entity Types', selectedDataForEnity, 'EntityTypeId', 'EntityTypeName', 'checkbox', 'EntityTypeId', true, false, "Select All", "down"));
        }
        filterData.push(processFilters(rulecCatalogBasicInfo.d.Rules, 'RuleCB', 'Rules Types', selectedDataForRules, 'RuleId', 'RuleName', 'checkbox', 'RuleName', false, false, "Select All", "up"));
        bindSlideMenu(filterData);

    }
    function ExecuteSynchronously(url, method, args) {
        var executor = new Sys.Net.XMLHttpSyncExecutor();
        var request = new Sys.Net.WebRequest();
        request.set_url(url + '/' + method);
        request.set_httpVerb('POST');
        request.get_headers()['Content-Type'] = 'application/json; charset=utf-8';
        request.set_executor(executor);
        request.set_body(Sys.Serialization.JavaScriptSerializer.serialize(args));
        request.invoke();
        if (executor.get_responseAvailable()) {
            return (executor.get_object());
        }
        return (false);
    }
    function processFilters(filterGroupItems, filterGroupHeaderID, filterGroupHeaderTitle, checkedValuesList, txtId, txtName, filterType, checkerItem, isSearchable, isDateTypeControl, selectAllText, state) {
        var filterItems = [];
        var checkedValuesArray = checkedValuesList.split(',');
        $.each(filterGroupItems, function (index, item) {
            var objFilterItem = {};
            objFilterItem.text = item[txtName];
            objFilterItem.value = item[txtId];
            if (checkedValuesArray.indexOf(item[checkerItem].toString()) > -1)
                objFilterItem.isSelected = "true";
            else
                objFilterItem.isSelected = "false";
            if (isDateTypeControl) {
                objFilterItem.dateFilterType = item.dateFilterType;
                objFilterItem.rangeStartDate = item.rangeStartDate;
                objFilterItem.rangeEndDate = item.rangeEndDate;
                objFilterItem.singleDate = item.singleDate;
                objFilterItem.dateType = item.dateType;
            }
            filterItems.push(objFilterItem);
        });
        var filterDataItem = {};
        if (filterGroupHeaderID != null)
            filterDataItem.identity = filterGroupHeaderID;
        filterDataItem.sectionHeader = filterGroupHeaderTitle;
        if (selectAllText != null)
            filterDataItem.selectAllText = selectAllText;
        filterDataItem.state = state;
        filterDataItem.sectionType = filterType;
        filterDataItem.listItems = filterItems;
        filterDataItem.isSearchable = isSearchable;
        filterDataItem.maxHeight = $(window).height() - 425;
        return filterDataItem;
    }
    function bindSlideMenu(filterData) {
        var rightFilterContainer = $("#SMDashboardRightFilterContainer1");
        rightFilterContainer.empty();

        var objFilterData = {};
        objFilterData.pivotElementId = "SMDashboardRightFilter1";
        objFilterData.id = "SMDashboardRightFilterDiv";
        objFilterData.container = rightFilterContainer;
        objFilterData.data = filterData;
        smslidemenu.init(objFilterData, function () { filterApply(); return false; });
        smslidemenu.hide("SMDashboardRightFilterDiv");
        var filtersSelectedDataSelected = smslidemenu.getAllData("SMDashboardRightFilterDiv");
    }
    function filterApply() {
        var defaultDateParam = "Any Time";
        var selectedTilesParam = "";
        var selectedCorpActionsParam = "";
        var selectedExceptionTypesParam = "";
        var selectedSecTypesParam = "";
        var selectedExternalSystemParam = "";
        var userSpecificParam = false;

        var filtersSelectedData = smslidemenu.getAllData("SMDashboardRightFilterDiv");
        var rulesTypes = [];
        for (var i = 0; i < filtersSelectedData.RuleCB.SelectedText.split(",").length; i++) {
            rulesTypes.push(filtersSelectedData.RuleCB.SelectedText.split(",")[i]);
        }

        var types = [];
        if (runTimeModule == "SECURITY_MASTER") {
            for (var i = 0; i < filtersSelectedData.SecurityCB.SelectedValue.split(",").length; i++) {
                types.push(parseInt(filtersSelectedData.SecurityCB.SelectedValue.split(",")[i]));
            }
        } else {

            for (var i = 0; i < filtersSelectedData.EntityCB.SelectedValue.split(",").length; i++) {
                types.push(parseInt(filtersSelectedData.EntityCB.SelectedValue.split(",")[i]));
            }

        }

        srmRuleCatalog.GetData(types, rulesTypes, runTimeButtonText);
        smslidemenu.hide("SMDashboardRightFilterDiv");

        //smDashboard.SMSDashboard(objDashboardInfo, true, false);
    }

    function onGridRenderComplete() {
        var grid = $("#GridBindingPanelForRuleCataLog");

        //resizeGridFinal($("#GridBindingPanelForRuleCataLog").attr("id"), os_serverSideControls.TopPanel, os_serverSideControls.MiddlePanel, os_serverSideControls.BottomPanel, 0, 210);
        resizeGridFinal('GridBindingPanelForRuleCataLog', '', 'RuleCataLogMainGrid', '', 0, 210);
     }

    SRMRuleCatalog.prototype.attachEventHandlers = function attachEventHandlers() {
        $('.smGridBinding').unbind('click').on('click', function (e, preventTrigger) {
            //$smBind.grid[0].children.div2.firstElementChild.style.opacity = 1;
            var idOfEvent = e.target.id;
            for (i = 0; i < $('.smGridBinding').length; i++) {
                if ($('.smGridBinding')[i].id == idOfEvent) {
                    document.getElementById(idOfEvent).style.opacity = 1;
                    document.getElementById(idOfEvent).parentElement.children[1].children[0].style.opacity = 1;
                } else {
                    document.getElementById($('.smGridBinding')[i].id).style.opacity = 0.6;
                    document.getElementById($('.smGridBinding')[i].id).parentElement.children[1].children[0].style.opacity = 0;

                }

            }

            runTimeButtonText = e.target.innerText;

            filterApply();
            //srmRuleCatalog.GetData(null,null);
        });
    }

    return srmRuleCatalog;
})();

