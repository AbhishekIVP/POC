var workflowstatus = (function () {
    function workflowStatus() {
        this._controls = null;
        this._securityInfo = null;
        this._controlIdInfo = null;
        this.gridUpdating = false;
        this.userSessionIdentifier = null;
        this.currentRequestObject = null;
        this.currentRequestText = null;
        this.isFirstTimeBinding = false;
        this.bindAllRequestsData = false;
        this.count = 0;
        this.gridIdVsGridInfo = {};
        this.gridIdVsGridInfo[WorkflowActionGridType.AllRequests] = {};
        this.gridIdVsGridInfo[WorkflowActionGridType.MyRequests] = {};
        this.gridIdVsGridInfo[WorkflowActionGridType.RejectedRequests] = {};
        this.gridIdVsGridInfo[WorkflowActionGridType.RequestsPending] = {};
        this.tabCount = {};
        this.tabCount[WorkflowActionGridType.AllRequests] = 0;
        this.tabCount[WorkflowActionGridType.MyRequests] = 0;
        this.tabCount[WorkflowActionGridType.RejectedRequests] = 0;
        this.tabCount[WorkflowActionGridType.RequestsPending] = 0;
    }
    var workflowstatus = new workflowStatus();

    var selectedTab = WorkflowTabs.SecMaster;
    var initialSelectedTab = WorkflowTabs.SecMaster;
    var secIds = null;

    String.prototype.replaceAll = function (find, replace) {
        return this.replace(new RegExp(find, 'g'), replace);
    }

    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    workflowStatus.prototype.SMSWorkflowStatus = function SMSWorkflowStatus(controlInfo, securityInfo) {
        $(function () {
            workflowstatus.isFirstTimeBinding = true;
            workflowstatus.userSessionIdentifier = GetGUID();
            workflowstatus._controlIdInfo = eval("(" + controlInfo + ")");
            workflowstatus._securityInfo = eval("(" + securityInfo + ")");
            workflowstatus._controls = new SMSWorkflowStatusControls(workflowstatus._controlIdInfo);

            var tabContainer = $("#tabContainer");
            $(".TabscssAfter").removeClass("TabscssAfter");
            selectedTab = WorkflowTabs.SecMaster;

            if (workflowstatus._securityInfo.SelectedTab != null && workflowstatus._securityInfo.SelectedTab.length > 0) {
                selectedTab = workflowstatus._securityInfo.SelectedTab;
                initialSelectedTab = selectedTab;
            }
            tabContainer.find("[selectedtab='" + selectedTab + "']").eq(0).addClass("TabscssAfter");
            if (workflowstatus._securityInfo.ProductName == WorkflowTabs.SecMaster)
                tabContainer.show();
            else
                tabContainer.hide();

            if (workflowstatus._securityInfo.WorkflowType == "MyRequests") {
                workflowstatus.isFirstTimeBinding = false;
                $(".WorkflowTabActive").removeClass("WorkflowTabActive");
                $(".WorkflowTab[targetdivid='workflowMyRequests']").addClass("WorkflowTabActive");
            }
            else if (workflowstatus._securityInfo.WorkflowType == "RejectedRequests") {
                workflowstatus.isFirstTimeBinding = false;
                $(".WorkflowTabActive").removeClass("WorkflowTabActive");
                $(".WorkflowTab[targetdivid='workflowRejectedRequests']").addClass("WorkflowTabActive");
            }
            else if (workflowstatus._securityInfo.WorkflowType == "RequestsPending") {
                workflowstatus.isFirstTimeBinding = false;
                $(".WorkflowTabActive").removeClass("WorkflowTabActive");
                $(".WorkflowTab[targetdivid='workflowRequestsPending']").addClass("WorkflowTabActive");
            }
            else if (workflowstatus._securityInfo.WorkflowType == "AllRequests") {
                workflowstatus.isFirstTimeBinding = false;
                workflowstatus.bindAllRequestsData = true;
                $(".WorkflowTabActive").removeClass("WorkflowTabActive");
                $(".WorkflowTab[targetdivid='workflowAllRequests']").addClass("WorkflowTabActive").show();
                $(".WorkflowTab[tabindex='0']").css({ 'border-top-left-radius': '0px', 'border-bottom-left-radius': '0px' });
            }

            AttachHandlers();
            Init();
        });
    }

    function Init() {
        onServiceUpdating();
        FormatTabDivs();

        workflowstatus.gridUpdating = true;
        BindGrids(true, true, true, workflowstatus.bindAllRequestsData);
        DefaultingActionButtons();
        onInitComplete();
        ExternalSystemBinding();

        //<<<<< --- have to call later --- >>>>

    }

    function onClickPanelSave(e) {
        var systemIdsList = [];
        var target = $(e.target);
        if (target.prop('tagName').toUpperCase() == 'INPUT') {
            if (target.prop('type').toUpperCase() == 'SUBMIT') {
                if (contains(target.prop('id'), 'btnSaveOk')) {
                    $('#chkListExternalSystemSave').find("input:checked").each(function (index) {//ChkListExternalSystem
                        if ($(this).attr('extId') != '-1')
                            systemIdsList.push($(this).attr('extId'));
                    });
                    if (systemIdsList.length > 0) {
                        var params = {};
                        params.inputObject = {};
                        params.inputObject.SecIds = secIds;
                        params.inputObject.systemIds = systemIdsList;
                        params.inputObject.userName = workflowstatus._securityInfo.UserName;
                        params.inputObject.selectedTab = selectedTab;
                        CallCommonServiceMethod('PostToDownstream', params, onSuccess_PostToDownstream, OnFailure, null, false);
                    }

                    //enableDivs();
                    //$('#panelSave').hide();
                    $find(workflowstatus._controlIdInfo.ModalPanelSaveId).hide();

                    var val = $('#posting_status_checkbox').prop('checked').toString().toLowerCase();
                    if (val == 'true') {
                        //alert(selectedTab);

                        if (selectedTab == WorkflowTabs.RefMaster) {
                            var url = 'App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?pageIdentifier=RefM_ReportSystemTaskStatus';
                            window.parent.leftMenu.createTab('', url, GetGUID(), "Entity Downstream Post Status");
                        }
                        else {
                            var url = "SMCommonStatusScreen.aspx?identifier=CommonStatusScreen";
                            window.parent.leftMenu.createTab('ExternalSystemStatus', url, GetGUID(), "Downstream Post Status");
                        }
                    }
                    return false;
                    //popup hide
                }

                else if (contains(target.prop('id'), 'btnClosePost')) {
                    //enableDivs();
                    //$('#panelSave').hide();
                    $find(workflowstatus._controlIdInfo.ModalPanelSaveId).hide();
                    return false;
                }
            }
        }
    }

    function onClickChkListExternalSystem(e) {
        var target = $(e.target);
        var ChkListControl = $('#chkListExternalSystemSave');
        if (target[0].tagName.toUpperCase() == 'INPUT' && target[0].type.toUpperCase() == 'CHECKBOX') {
            if (target.attr('extId') == '-1') {
                if (target.prop('checked') == true) {
                    //     $("label[for='" + target.prop('id') + "']")[0].className = 'CommonGreenBtnStyle';
                    ChkListControl.find("input[type='checkbox']").each(function (index) {
                        $(this).prop('checked', true);
                    });
                }
                else {
                    //         $("label[for='" + target.prop('id') + "']")[0].className = 'uncheckedButton';
                    ChkListControl.find("input[type='checkbox']").each(function (index) {
                        $(this).prop('checked', false);
                    });
                }
            }
            else if (target.attr('extId') != '-1') {
                var totalExternalSystemCount = ChkListControl.find("input[type='checkbox'][extId!='-1']").length;
                var totalCheckedExternalSystemCount = 0;
                ChkListControl.find("input[type='checkbox'][extId!='-1']").each(function (index) {
                    if ($(this).prop('checked') == true) {
                        totalCheckedExternalSystemCount++;
                    }
                });
                if (totalCheckedExternalSystemCount < totalExternalSystemCount) {
                    ChkListControl.find("input[type='checkbox'][extId='-1']").prop('checked', false);
                    //       ChkListControl.find("label:first").prop('class', 'uncheckedButton');
                }
                else if (totalCheckedExternalSystemCount == totalExternalSystemCount) {
                    ChkListControl.find("input[type='checkbox'][extId='-1']").prop('checked', true);
                    //       ChkListControl.find("label:first").prop('class', 'CommonGreenBtnStyle');
                }
            }
        }
    }

    function ExternalSystemBinding() {
        var params = {};
        params.inputObject = {};
        params.inputObject.userName = workflowstatus._securityInfo.UserName;
        params.inputObject.selectedTab = selectedTab;
        //params.inputObject.moduleId = 6;
        //if (!string.IsNullOrEmpty(Request.QueryString["ModuleID"]))
        //    params.moduleId = Request.QueryString["ModuleID"];
        CallCommonServiceMethod('GetExternalSystemInfo', params, onSuccess_GetExternalSystemInfo, OnFailure, null, false);
        
        //onSuccess_GetExternalSystemInfo({ d: { ExternalSystemNameAndIds: ['-1<@>Select All', '1<@>DB1<@>true', '2<@>DB2<@>true'], PostToPrivilege: true } });
    }

    function onSuccess_PostToDownstream(result) {
        if (result.d != null) {
        }
    }

    function onSuccess_GetExternalSystemInfo(result) {
        if (result.d != null) {
            BindExternalSystem(result.d.ExternalSystemNameAndIds, result.d.PostToPrivilege);
        }
    }

    function BindExternalSystem(listExternalSystem, PostToPrivilege) {
        var tableControl = $('#chkListExternalSystemSave');
        tableControl.empty();
        var state = 0;
        var isExternalSystem = (listExternalSystem != null && listExternalSystem.length > 0);
        if (PostToPrivilege) {
            if (isExternalSystem) {
                state = 0;

                var save = 'Save';
                var trHead = $('<tr style="float: left;"></tr>');
                var tdHead = $('<td></td><br/>');
                var tr = $('<tr></tr>');
                var newtr = $('<tr></tr>');
                var innertble = $('<table style="margin: 0px auto; width: 100%;"></table>');
                for (var i = 0; i < listExternalSystem.length; i++) {
                    var extId = listExternalSystem[i].split('<@>')[0];
                    var extName = listExternalSystem[i].split('<@>')[1];
                    var realTimePosting = listExternalSystem[i].split('<@>')[2];

                    if (i == 0) {
                        var chk1 = $("<input type='checkbox'  id='chkListExternalSystem" + save + "_" + i.toString() + "' extId='" + extId + "' realTimePosting='" + realTimePosting + "' />");
                        var lblFor1 = $('<label for=chkListExternalSystem' + save + '_' + i.toString() + '  style="color:#666; padding-left: 3px; padding-right: 3px; bottom: 2px; position: relative; font-weight: normal; font-size: 13px; ">' + extName + '</label>');
                        chk1.attr('checked', true);
                        tdHead.append(chk1);
                        tdHead.append(lblFor1);

                    } else {

                        var td = $('<td></td>');
                        td.css('white-space', 'nowrap');
                        var chk = $("<input type='checkbox'  id='chkListExternalSystem" + save + "_" + i.toString() + "' extId='" + extId + "' realTimePosting='" + realTimePosting + "' />");
                        var lblFor = $('<label for=chkListExternalSystem' + save + '_' + i.toString() + ' style="color:#666; padding-left: 3px; padding-right: 3px; bottom: 2px; position: relative;font-weight: normal; font-size: 13px;">' + extName + '</label>');
                        chk.attr('checked', true);
                        td.append(chk);
                        td.append(lblFor);
                        if ((i - 1) % 3 == 0) {
                            newtr = $('<tr></tr>');
                            newtr.append(td);
                        }
                        else
                            newtr.append(td);
                        innertble.append(newtr);
                    }


                    // var lblFor = $('<label for=chkListExternalSystem' + save + '_' + i.toString() + '>' + extName + '</label>');


                    tr.append(innertble);
                    trHead.append(tdHead);
                    // trHead.append(tr);
                }
                tableControl.append(trHead);
                tableControl.append(tr);
            }
            else {
                state = 1;

                var tr = $('<tr></tr>');
                var td = $('<td>No External system configured.</td>');
                tr.append(td);
                tableControl.append(tr);
            }
        }
        else {
            state = 2;
        }

        $('#chkListExternalSystemSave')[0].style.display = ((state != 2) ? '' : 'none');
    }



    function onInitComplete() {
        if (workflowstatus.gridUpdating) {
            setTimeout(function () { onInitComplete(); }, 1000);
        }
        else {
            onServiceUpdated();
            workflowstatus.isFirstTimeBinding = false;
        }
    }

    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }

    function BindGrids(getMyRequests, getRejectedRequests, getRequestsPending, getAllRequests) {
        if (getAllRequests) {
            workflowstatus.tabCount[WorkflowActionGridType.AllRequests] = 0;
            workflowstatus.gridIdVsGridInfo[WorkflowActionGridType.AllRequests] = {};
            $("#workflowAllRequestsGridContainer").html("<div id='workflowAllRequestsScrollContainer' class='workflowRequestsScrollContainer'></div>");
        }
        if (getMyRequests) {
            workflowstatus.tabCount[WorkflowActionGridType.MyRequests] = 0;
            workflowstatus.gridIdVsGridInfo[WorkflowActionGridType.MyRequests] = {};
            $("#workflowMyRequestsGridContainer").html("<div id='workflowMyRequestsScrollContainer' class='workflowRequestsScrollContainer'></div>");
        }
        if (getRejectedRequests) {
            workflowstatus.tabCount[WorkflowActionGridType.RejectedRequests] = 0;
            WorkFlowRejectedRequestsGridId = "workflowRejectedRequestsGrid_" + GetGUID();
            workflowstatus.gridIdVsGridInfo[WorkflowActionGridType.RejectedRequests] = {};
            $("#workflowRejectedRequests").html("<div id='" + WorkFlowRejectedRequestsGridId + "' class='WorkflowGridHandler'></div>");
        }
        if (getRequestsPending) {
            workflowstatus.tabCount[WorkflowActionGridType.RequestsPending] = 0;
            WorkFlowRequestsPendingGridId = "workflowRequestsPendingGrid_" + GetGUID();
            workflowstatus.gridIdVsGridInfo[WorkflowActionGridType.RequestsPending] = {};
            $("#workflowRequestsPending").html("<div id='" + WorkFlowRequestsPendingGridId + "' class='WorkflowGridHandler'></div>");
        }
        var params = {};
        params.inputObject = {};
        params.inputObject.getAllRequests = getAllRequests;
        params.inputObject.getMyRequests = getMyRequests;
        params.inputObject.getRejectedRequests = getRejectedRequests;
        params.inputObject.getRequestsPending = getRequestsPending;
        params.inputObject.userName = workflowstatus._securityInfo.UserName;
        params.inputObject.rejectedRequestsDivId = WorkFlowRejectedRequestsGridId;
        params.inputObject.requestsPendingDivId = WorkFlowRequestsPendingGridId;
        params.inputObject.userSessionIdentifier = workflowstatus.userSessionIdentifier;
        params.inputObject.longDateFormat = workflowstatus._securityInfo.LongDateFormat;
        params.inputObject.shortDateFormat = workflowstatus._securityInfo.ShortDateFormat;
        params.inputObject.sectypeIds = (initialSelectedTab == selectedTab) ? workflowstatus._securityInfo.SectypeIds : "-1";
        params.inputObject.securityId = (initialSelectedTab == selectedTab) ? workflowstatus._securityInfo.SecurityId : "";
        params.inputObject.selectedTab = selectedTab;
        CallCommonServiceMethod('GetGridData', params, onSuccess_bindGrid, OnFailure, null, false);
    }

    function applySlimScroll(containerSelector, bodySelector) {
        var scrollBodyContainer = $(containerSelector);
        var scrollBody = scrollBodyContainer.find(bodySelector);

        if (scrollBodyContainer.height() < scrollBody.height()) {
            scrollBody.smslimscroll({ height: scrollBodyContainer.height() + 'px', alwaysVisible: true, position: 'right', distance: '2px' });
        }
    }

    function onSuccess_bindGrid(result) {
        if (result.d != null) {
            result = result.d;
            var bindGrid = true;
            var columnToHideGrouped = null;
            var columnToHideUnGrouped = null;
            var isMyRequest = false;
            var isAllRequest = false;
            var myRequestCount = 0;
            var allRequestCount = 0;
            var workflowMyRequestsScrollContainer = $("#workflowMyRequestsScrollContainer");
            var workflowAllRequestsScrollContainer = $("#workflowAllRequestsScrollContainer");
            var workflowGridDiv = $(".workflowGridDiv");
            var gridType = {};

            columnToHideUnGrouped = [{ "columnName": "queue_id", "isDefault": true }, { "columnName": "row_id", "isDefault": true }, { "columnName": "row_keys", "isDefault": true }, { "columnName": "workflow_group_user_name", "isDefault": true }, { "columnName": "Security", "isDefault": true }, { "columnName": "data_type_name", "isDefault": true }, { "columnName": "sectype_table_id", "isDefault": true }, { "columnName": "attribute_id", "isDefault": true }, { "columnName": "entity_type_id", "isDefault": true }, { "columnName": "Entity", "isDefault": true }, { "columnName": "entity_type_table", "isDefault": true }];
            columnToHideGrouped = [{ "columnName": "queue_id", "isDefault": true }, { "columnName": "row_id", "isDefault": true }, { "columnName": "row_keys", "isDefault": true }, { "columnName": "workflow_group_user_name", "isDefault": true }, { "columnName": "Security", "isDefault": true }, { "columnName": "Security Id", "isDefault": true }, { "columnName": "Security Type", "isDefault": true }, { "columnName": "Security Name", "isDefault": true }, { "columnName": "Requested On", "isDefault": true }, { "columnName": "data_type_name", "isDefault": true }, { "columnName": "sectype_table_id", "isDefault": true }, { "columnName": "attribute_id", "isDefault": true }, { "columnName": "entity_type_id", "isDefault": true }, { "columnName": "Entity", "isDefault": true }, { "columnName": "Entity Code", "isDefault": true }, { "columnName": "Entity Type", "isDefault": true }, { "columnName": "Entity Name", "isDefault": true }, { "columnName": "entity_type_table", "isDefault": true }];

            workflowGridDiv.css("visibility", "hidden");
            for (var index = 0; index < workflowGridDiv.length; index++) {
                var workflowGridDivThis = workflowGridDiv.eq(index);
                workflowGridDivThis.attr("displayPrev", workflowGridDivThis.css("display"));
            }
            workflowGridDiv.show();

            for (var key in result) {
                var item = result[key];
                bindGrid = false;
                if (item.isMyRequestGrid || item.isAllRequestGrid) {
                    if (item.isMyRequestGrid) {
                        isMyRequest = true;
                    }
                    if (item.isAllRequestGrid) {
                        isAllRequest = true;
                    }
                    if (item.isDataAvailable) {
                        var RequestsScrollContainer;
                        if (item.isMyRequestGrid) {
                            myRequestCount += item.rowCount;
                            gridType = WorkflowActionGridType.MyRequests;
                            RequestsScrollContainer = workflowMyRequestsScrollContainer;
                        }
                        else if (item.isAllRequestGrid) {
                            allRequestCount += item.rowCount;
                            gridType = WorkflowActionGridType.AllRequests;
                            RequestsScrollContainer = workflowAllRequestsScrollContainer;
                        }

                        var usersGroups = item.usersGroups.split(",");
                        var userGroupsShown = "";
                        var userGroupsHidden = "";
                        var isMoreUsers = false;
                        if (usersGroups.length > 3) {
                            isMoreUsers = true;
                            for (var index = 0; index < usersGroups.length; index++) {
                                if (index < 3)
                                    userGroupsShown += usersGroups[index] + ",";
                                else
                                    userGroupsHidden += usersGroups[index] + ",";
                            }
                            userGroupsShown = userGroupsShown.substr(0, userGroupsShown.length - 1);
                            userGroupsHidden = userGroupsHidden.substr(0, userGroupsHidden.length - 1);
                        }
                        else {
                            userGroupsShown = item.usersGroups;
                        }
                        userGroupsShown = userGroupsShown.replaceAll(",", ", ");

                        RequestsScrollContainer.append("<div class='workflowUserContainer' targetdivid='" + item.gridID + "'>Pending at " + userGroupsShown + (isMoreUsers ? ("<div class='WorkflowMoreUsers' users='" + userGroupsHidden + "'>+ " + (usersGroups.length - 3).toString() + " more</div>") : "") + " (" + item.rowCount + ")</div><div class='WorkflowGridAccordianDiv'><div id='" + item.gridID + "' class='WorkflowGridHandler'></div></div>");
                        bindGrid = true;
                    }
                }
                else {
                    if (item.gridID == WorkFlowRejectedRequestsGridId) {
                        workflowstatus.tabCount[WorkflowActionGridType.RejectedRequests] = item.rowCount;
                        gridType = WorkflowActionGridType.RejectedRequests;
                        $(".WorkflowTab[targetdivid='workflowRejectedRequests'] .WorkflowTabCount").text(item.rowCount);
                    }
                    else if (item.gridID == WorkFlowRequestsPendingGridId) {
                        workflowstatus.tabCount[WorkflowActionGridType.RequestsPending] = item.rowCount;
                        gridType = WorkflowActionGridType.RequestsPending;
                        $(".WorkflowTab[targetdivid='workflowRequestsPending'] .WorkflowTabCount").text(item.rowCount);
                    }
                    if (item.isDataAvailable) {
                        bindGrid = true;
                    }
                    else {
                        $("#" + item.gridID).append("<div class='workflowEmptyGrid'>No data available</div>");
                    }
                }
                if (bindGrid) {
                    var gridInfo = {
                        "SelectRecordsAcrossAllPages": null,
                        "ViewKey": item.viewKey,
                        "CacheGriddata": true,
                        "GridId": item.gridID,
                        "CurrentPageId": item.currentPageId,
                        "SessionIdentifier": item.sessionIdentifier,
                        "UserId": workflowstatus._securityInfo.UserName,
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
                        "ItemText": "Number of Requests",
                        "DoNotRearrangeColumn": true,
                        "RequireGrouping": true,
                        "RequireFilter": true,
                        "RequireSort": true,
                        "RequireMathematicalOperations": false,
                        "RequireSelectedRows": true,
                        "RequireExportToExcel": true,
                        "RequireSearch": true,
                        "RequireFreezeColumns": false,
                        "RequireHideColumns": true,
                        "RequireColumnSwap": true,
                        "RequireGroupExpandCollapse": true,
                        "RequireResizing": true,
                        "RequireLayouts": false,
                        "RequireGroupHeaderCheckbox": true,
                        "RequireRuleBasedColoring": false,
                        "RequireExportToPdf": false,
                        "ShowRecordCountOnHeader": false,
                        "ShowAggregationOnHeader": true,
                        "ColumnsNotToSum": ["row_id", "queue_id", "sectype_table_id", "attribute_id", "entity_type_id"],
                        "CheckBoxInfo": {},
                        "DateFormat": workflowstatus._securityInfo.ShortDateFormat,
                        "RaiseGridCallBackBeforeExecute": "",
                        "RaiseGridRenderComplete": "workflowstatus.gridRenderComplete",
                        "DefaultGroupedAndSortedColumns": (selectedTab == WorkflowTabs.RefMaster) ? [{ "columnName": "Entity", "isGrouped": true }] : [{ "columnName": "Security", "isGrouped": true }],
                        "ColumnsToHide": columnToHideGrouped,
                        "CustomRowsDataInfo": $.parseJSON(item.customInfoScript),
                        "CustomFormatInfoClientSide": (selectedTab == WorkflowTabs.RefMaster) ? { "Entity": { "AssemblyName": "CommonService", "FormatString": "{0:C2}", "ClassName": "CommonService.RMHeaderFormatter" } } : { "Security": { "AssemblyName": "CommonService", "FormatString": "{0:C2}", "ClassName": "CommonService.HeaderFormatter" } }
                    }
                    workflowstatus.gridIdVsGridInfo[gridType][item.gridID] = { gridInfo: gridInfo, columnToHideGrouped: columnToHideGrouped, columnToHideUnGrouped: columnToHideUnGrouped };

                    xlgridloader.create(item.gridID, item.gridID, gridInfo, "");
                    if (selectedTab != WorkflowTabs.RefMaster) {
                        addGroupBySection(item.gridID, 'Security Id', true);
                    }
                    else
                        addGroupBySection(item.gridID, 'Entity Code', true);
                }
            }
            for (var index = 0; index < workflowGridDiv.length; index++) {
                var workflowGridDivThis = workflowGridDiv.eq(index);
                workflowGridDivThis.css("display", workflowGridDivThis.attr("displayPrev"));
            }

            if (isMyRequest || isAllRequest) {
                if (isMyRequest) {
                    workflowstatus.tabCount[WorkflowActionGridType.MyRequests] = myRequestCount;
                    if (myRequestCount == 0) {
                        workflowMyRequestsScrollContainer.append("<div class='workflowEmptyGrid'>No data available</div>");
                    }
                    $(".WorkflowTab[targetdivid='workflowMyRequests'] .WorkflowTabCount").text(myRequestCount);

                    var workflowUserContainer = workflowMyRequestsScrollContainer.find(".workflowUserContainer");
                    if (workflowUserContainer.length > 0) {
                        workflowUserContainer.eq(0).addClass("workflowUserContainerActive");

                        var targetDivId = workflowMyRequestsScrollContainer.find(".workflowUserContainerActive").attr("targetdivid");
                        workflowMyRequestsScrollContainer.find(".WorkflowGridAccordianDiv").hide();
                        $("#" + targetDivId).parent().show();
                    }
                }
                if (isAllRequest) {
                    workflowstatus.tabCount[WorkflowActionGridType.AllRequests] = allRequestCount;
                    if (allRequestCount == 0) {
                        workflowAllRequestsScrollContainer.append("<div class='workflowEmptyGrid'>No data available</div>");
                    }
                    $(".WorkflowTab[targetdivid='workflowAllRequests'] .WorkflowTabCount").text(allRequestCount);

                    var workflowUserContainer = workflowAllRequestsScrollContainer.find(".workflowUserContainer");
                    if (workflowUserContainer.length > 0) {
                        workflowUserContainer.eq(0).addClass("workflowUserContainerActive");

                        var targetDivId = workflowAllRequestsScrollContainer.find(".workflowUserContainerActive").attr("targetdivid");
                        workflowAllRequestsScrollContainer.find(".WorkflowGridAccordianDiv").hide();
                        $("#" + targetDivId).parent().show();
                    }
                }

                $(".workflowUserContainer").unbind("click").click(function (e) {
                    var target = $(e.target);
                    if (!target.hasClass("workflowUserContainer"))
                        target = target.parents(".workflowUserContainer").first();
                    var targetdivid = target.attr("targetdivid");
                    var targetdiv = $("#" + targetdivid).parent();
                    var workflowRequestsScrollContainer = target.closest(".workflowRequestsScrollContainer");

                    workflowRequestsScrollContainer.find(".WorkflowGridAccordianDiv").hide();
                    workflowRequestsScrollContainer.find(".workflowUserContainerActive").removeClass("workflowUserContainerActive");
                    targetdiv.show();
                    target.addClass("workflowUserContainerActive");

                    DefaultingActionButtons();
                    resizeGridsWrapper($(".WorkflowTabActive").attr("targetdivid"));
                });

                AttachMoreUsersClickHandler(".WorkflowMoreUsers");
            }

            AttachGridClickHandler(".WorkflowGridHandler");
            setTimeout(function () {
                workflowGridDiv.css("visibility", "visible");
                workflowstatus.gridUpdating = false;
            }, 2000);
        }
        FormatTabsPositioning();
    }

    function onSuccess_GetLegData(result) {
        if (result.d != null) {
            result = result.d;

            var panelLegData = $('#panelLegData');
            var LegDataPopupDiv = $('#legDataPopupDiv');

            LegDataPopupDiv.html("<div><div id='" + result.gridID + "'></div></div>");


            //setTimeout(function () {
            var panelTop = $(window).height() / 2 - panelLegData.outerHeight() / 2;
            var panelLeft = $(window).width() / 2 - panelLegData.outerWidth() / 2;
            panelLegData.css('top', panelTop + 'px').css('left', panelLeft + 'px');
            var zIndex = panelLegData.css('zIndex') - 1;
            disableDivs(zIndex);
            panelLegData.show();
            //}, 1000);

            SetNeoGridData(result);

            //applySlimScroll('#legDataPopupDiv', 'div:first');
        }
    }

    function SetNeoGridData(item) {
        var gridInfo = {
            "SelectRecordsAcrossAllPages": null,
            "ViewKey": item.viewKey,
            "CacheGriddata": true,
            "GridId": item.gridID,
            "CurrentPageId": item.currentPageId,
            "SessionIdentifier": item.sessionIdentifier,
            "UserId": workflowstatus._securityInfo.UserName,
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
            "ItemText": "Number of Records",
            "DoNotRearrangeColumn": true,
            "RequireGrouping": true,
            "RequireFilter": true,
            "RequireSort": true,
            "RequireMathematicalOperations": false,
            "RequireSelectedRows": true,
            "RequireExportToExcel": true,
            "RequireSearch": true,
            "RequireFreezeColumns": false,
            "RequireHideColumns": true,
            "RequireColumnSwap": true,
            "RequireGroupExpandCollapse": true,
            "RequireResizing": true,
            "RequireLayouts": false,
            "RequireGroupHeaderCheckbox": true,
            "RequireRuleBasedColoring": false,
            "RequireExportToPdf": false,
            "ShowRecordCountOnHeader": false,
            "ShowAggregationOnHeader": true,
            "ColumnsNotToSum": [],
            "CheckBoxInfo": null,
            "RaiseGridCallBackBeforeExecute": "",
            "RaiseGridRenderComplete": "workflowstatus.gridRenderCompleteNeo",
            "DefaultGroupedAndSortedColumns": [],
            "ColumnsToHide": [{ "columnName": "row_keys", "isDefault": true }, { "columnName": "entity_code", "isDefault": true }],
            "CustomRowsDataInfo": []
        }
        xlgridloader.create(item.gridID, item.gridID, gridInfo, "");
    }

    function FormatTabsPositioning() {
        var selectedDivTargetId = $(".WorkflowTabActive").attr("targetdivid");
        var tabMyRequests = $(".WorkflowTab[targetdivid='workflowMyRequests']");
        var tabRejectedRequests = $(".WorkflowTab[targetdivid='workflowRejectedRequests']");
        var tabRequestPending = $(".WorkflowTab[targetdivid='workflowRequestsPending']");
        if (workflowstatus.tabCount[WorkflowActionGridType.RequestsPending] == 0 && parseInt(tabRequestPending.attr("tabindex")) == 0) {
            var tabIndex_0 = $(".WorkflowTab[tabindex='0']");
            var tabIndex_1 = $(".WorkflowTab[tabindex='1']");
            var tabIndex_2 = $(".WorkflowTab[tabindex='2']");

            tabIndex_0.attr("targetdivid", "workflowMyRequests");
            tabIndex_0.find(".WorkflowTabText").text("My Requests");
            tabIndex_0.find(".WorkflowTabCount").text(workflowstatus.tabCount[WorkflowActionGridType.MyRequests]);

            tabIndex_1.attr("targetdivid", "workflowRejectedRequests");
            tabIndex_1.find(".WorkflowTabText").text("Rejected Requests");
            tabIndex_1.find(".WorkflowTabCount").text(workflowstatus.tabCount[WorkflowActionGridType.RejectedRequests]);

            tabIndex_2.attr("targetdivid", "workflowRequestsPending");
            tabIndex_2.find(".WorkflowTabText").text("Requests pending at me");
            tabIndex_2.find(".WorkflowTabCount").text(workflowstatus.tabCount[WorkflowActionGridType.RequestsPending]);
        }
        else if (workflowstatus.tabCount[WorkflowActionGridType.RequestsPending] > 0 && parseInt(tabRequestPending.attr("tabindex")) == 2) {
            var tabIndex_0 = $(".WorkflowTab[tabindex='0']");
            var tabIndex_1 = $(".WorkflowTab[tabindex='1']");
            var tabIndex_2 = $(".WorkflowTab[tabindex='2']");

            tabIndex_1.attr("targetdivid", "workflowMyRequests");
            tabIndex_1.find(".WorkflowTabText").text("My Requests");
            tabIndex_1.find(".WorkflowTabCount").text(workflowstatus.tabCount[WorkflowActionGridType.MyRequests]);

            tabIndex_2.attr("targetdivid", "workflowRejectedRequests");
            tabIndex_2.find(".WorkflowTabText").text("Rejected Requests");
            tabIndex_2.find(".WorkflowTabCount").text(workflowstatus.tabCount[WorkflowActionGridType.RejectedRequests]);

            tabIndex_0.attr("targetdivid", "workflowRequestsPending");
            tabIndex_0.find(".WorkflowTabText").text("Requests pending at me");
            tabIndex_0.find(".WorkflowTabCount").text(workflowstatus.tabCount[WorkflowActionGridType.RequestsPending]);
        }
        if (!workflowstatus.isFirstTimeBinding) {
            $(".WorkflowTabActive").removeClass("WorkflowTabActive");
            $(".WorkflowTab[targetdivid='" + selectedDivTargetId + "']").addClass("WorkflowTabActive");
        }
        FormatTabDivs();
        DefaultingActionButtons();
    }

    function AttachGridClickHandler(gridSelector) {
        $(gridSelector).unbind("click").click(function (e) {
            var target = $(e.target);
            var WorkflowGridHandler = target;
            if (!WorkflowGridHandler.hasClass("WorkflowGridHandler"))
                WorkflowGridHandler = WorkflowGridHandler.parents(".WorkflowGridHandler").first();

            if (target.hasClass("workflowViewLogClick")) {
                var getPending = false;
                switch ($(".WorkflowTabActive").attr("targetdivid")) {
                    case "workflowMyRequests":
                    case "workflowAllRequests":
                        {
                            getPending = true;
                            break;
                        }
                    case "workflowRejectedRequests":
                    case "workflowRequestsPending":
                        {
                            getPending = false;
                            break;
                        }
                }
                var queueId = target.attr("queueId");
                var params = {};
                params.queueId = queueId;
                params.getPending = getPending;
                params.moduleName = selectedTab;
                CallCommonServiceMethod('GetWorkflowRequestLog', params, onSuccess_viewLog, OnFailure, null, false);
            }
            else if (target.hasClass("secIdClick")) {
                var securityId = target.attr("secId");
                if (selectedTab == WorkflowTabs.SecMaster)
                    SecMasterJSCommon.SMSCommons.openWindowForSecurity(true, securityId, true, true, SecMasterJSCommon.SMCreateUpdateOpenByDefault.None, 0, null, 3);
                else if (selectedTab == WorkflowTabs.RefMaster) {
                    var effectiveStartDate = "";
                    if (target.parents("[grouprowid]").length == 1)
                        effectiveStartDate = target.parents("[grouprowid]").next().children("[columnname='Effective Start Date']").text();
                    else if (target.parent().nextAll("[columnname='Effective Start Date']").length == 1)
                        effectiveStartDate = target.parent().nextAll("[columnname='Effective Start Date']").text();

                    var queueIds = target.attr("queueIds");
                    var gridType = target.attr("gridType");

                    var url = 'App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?pageIdentifier=ViewEntityFromSearch&isWorkFlow=true&effectiveStartDate=' + effectiveStartDate.trim() + '&dateFormat=' + workflowstatus._securityInfo.ShortDateFormat + '&entityCode=' + securityId + '&queueIds=' + queueIds + '&gridType=' + gridType;
                    window.parent.leftMenu.createTab('', url, GetGUID(), "View Entity " + securityId);
                }
            }
            else if (target.hasClass("workflowAttributeValueClick")) {
                var queueId = target.attr("queueId");
                var params = {};
                params.queueId = queueId;
                params.userName = workflowstatus._securityInfo.UserName;
                CallCommonServiceMethod('GetLegData', params, onSuccess_GetLegData, OnFailure, null, false);
            }
            else if (target[0].tagName != null && target[0].tagName.toLowerCase() == 'input' && target.prop('type').toLowerCase() == 'checkbox') {
                if (target.hasClass("workflowGridViewCheckbox")) {
                    var gridId = WorkflowGridHandler.prop("id");
                    var gridType = "";
                    var grid = WorkflowGridHandler;

                    switch ($(".WorkflowTabActive").attr("targetdivid")) {
                        case "workflowMyRequests":
                            {
                                gridType = WorkflowActionGridType.MyRequests;
                                break;
                            }
                        case "workflowRejectedRequests":
                            {
                                gridType = WorkflowActionGridType.RejectedRequests;
                                break;
                            }
                        case "workflowRequestsPending":
                            {
                                gridType = WorkflowActionGridType.RequestsPending;
                                break;
                            }
                        case "workflowAllRequests":
                            {
                                gridType = WorkflowActionGridType.AllRequests;
                                break;
                            }
                    }

                    var gridIdVsGridInfo = workflowstatus.gridIdVsGridInfo[gridType][gridId];

                    grid.empty().css("visibility", "hidden");

                    if (target.is(":checked") === false) {
                        gridIdVsGridInfo.gridInfo.DefaultGroupedAndSortedColumns = [];
                        gridIdVsGridInfo.gridInfo.CustomFormatInfoClientSide = {};
                        gridIdVsGridInfo.gridInfo.ColumnsToHide = gridIdVsGridInfo.columnToHideUnGrouped;
                    }
                    else {
                        gridIdVsGridInfo.gridInfo.DefaultGroupedAndSortedColumns = (selectedTab == WorkflowTabs.SecMaster) ? [{ "columnName": "Security", "isGrouped": true }] : [{ "columnName": "Entity", "isGrouped": true }];
                        gridIdVsGridInfo.gridInfo.CustomFormatInfoClientSide = (selectedTab == WorkflowTabs.SecMaster) ? { "Security": { "AssemblyName": "CommonService", "FormatString": "{0:C2}", "ClassName": "CommonService.HeaderFormatter" } } : { "Entity": { "AssemblyName": "CommonService", "FormatString": "{0:C2}", "ClassName": "CommonService.RMHeaderFormatter" } };
                        gridIdVsGridInfo.gridInfo.ColumnsToHide = gridIdVsGridInfo.columnToHideGrouped;
                    }

                    var inputObject = {};
                    inputObject.gridId = gridId;
                    inputObject.gridTypeStr = gridType.toUpperCase();
                    inputObject.userName = workflowstatus._securityInfo.UserName;
                    inputObject.userSessionIdentifier = workflowstatus.userSessionIdentifier;
                    inputObject.isGrouped = target.is(":checked");
                    inputObject.selectedTab = selectedTab;
                    var result = ExecuteSynchronously(path + '/BaseUserControls/Service/CommonService.svc', 'ResetGridData', { inputObject: inputObject }).d;

                    if (result.status) {
                        var parent = grid.parent();
                        parent.html("<div id='" + result.gridID + "' class='WorkflowGridHandler'></div>");
                        grid = parent.find(".WorkflowGridHandler");
                        grid.css("visibility", "hidden");

                        switch ($(".WorkflowTabActive").attr("targetdivid")) {
                            case "workflowMyRequests":
                            case "workflowAllRequests":
                                {
                                    $("[targetdivid='" + gridId + "']").attr("targetdivid", result.gridID);
                                    break;
                                }
                            case "workflowRejectedRequests":
                                {
                                    WorkFlowRejectedRequestsGridId = result.gridID;
                                    break;
                                }
                            case "workflowRequestsPending":
                                {
                                    WorkFlowRequestsPendingGridId = result.gridID;
                                    break;
                                }
                        }

                        gridIdVsGridInfo.gridInfo.GridId = result.gridID;
                        gridIdVsGridInfo.gridInfo.CurrentPageId = result.currentPageId;
                        gridIdVsGridInfo.gridInfo.ViewKey = result.viewKey;
                        gridIdVsGridInfo.gridInfo.SessionIdentifier = result.sessionIdentifier;
                        xlgridloader.create(result.gridID, result.gridID, gridIdVsGridInfo.gridInfo, "");
                        if (selectedTab != WorkflowTabs.RefMaster) {
                            addGroupBySection(result.gridID, 'Security Id', target.is(":checked"));
                        }
                        else
                            addGroupBySection(result.gridID, 'Entity Code', target.is(":checked"));

                        delete workflowstatus.gridIdVsGridInfo[gridType][inputObject.gridId];
                        workflowstatus.gridIdVsGridInfo[gridType][result.gridID] = gridIdVsGridInfo;

                        AttachGridClickHandler("#" + result.gridID);
                        setTimeout(function () {
                            resizeGridsWrapper($(".WorkflowTabActive").attr("targetdivid"));
                            grid.css("visibility", "visible");
                            DefaultingActionButtons();
                        }, 300);
                    }
                }
                else {
                    var WorkflowActionPending = $('.WorkflowActionPending');
                    var WorkflowActionRejected = $('.WorkflowActionRejected');
                    var WorkflowActionRevoke = $('.WorkflowActionRevoke');
                    var WorkflowActionButtonsSeparated = $(".WorkflowActionButtonsSeparated");
                    var WorkflowActionButtonsGrouped = $(".WorkflowActionButtonsGrouped");
                    setTimeout(function () {
                        if (WorkflowGridHandler.find('input[type="checkbox"][isheader="false"]:checked').length > 0) {
                            WorkflowActionPending.removeAttr("disabled");
                            WorkflowActionRejected.removeAttr("disabled");
                            WorkflowActionRevoke.removeAttr("disabled");
                            if (WorkflowActionButtonsSeparated.hasClass("WorkflowActionButtonsSeparatedDisabled"))
                                WorkflowActionButtonsSeparated.removeClass("WorkflowActionButtonsSeparatedDisabled");
                            if (WorkflowActionButtonsGrouped.hasClass("WorkflowActionButtonsGroupedDisabled"))
                                WorkflowActionButtonsGrouped.removeClass("WorkflowActionButtonsGroupedDisabled");
                        }
                        else {
                            WorkflowActionPending.attr("disabled", "disabled");
                            WorkflowActionRejected.attr("disabled", "disabled");
                            WorkflowActionRevoke.attr("disabled", "disabled");
                            if (!WorkflowActionButtonsSeparated.hasClass("WorkflowActionButtonsSeparatedDisabled"))
                                WorkflowActionButtonsSeparated.addClass("WorkflowActionButtonsSeparatedDisabled");
                            if (!WorkflowActionButtonsGrouped.hasClass("WorkflowActionButtonsGroupedDisabled"))
                                WorkflowActionButtonsGrouped.addClass("WorkflowActionButtonsGroupedDisabled");
                        }
                    }, 300);
                }
            }
        });
    }

    function AttachMoreUsersClickHandler(gridSelector) {
        $(gridSelector).unbind("click").click(function (e) {
            var target = $(e.target);
            var divMoreUsers = $("#divMoreUsers");
            var WorkflowMoreUsersDiv = $("#WorkflowMoreUsersDiv");
            var users = target.attr("users").split(",");
            var strHtml = "";

            for (var index = 0; index < users.length; index++) {
                strHtml += "<div class='WorkflowMoreUserSection'>" + users[index] + "</div>";
            }
            WorkflowMoreUsersDiv.html(strHtml);
            var targetOffset = target.offset();
            var windowHeight = $(window).height();
            var panelTop = targetOffset.top + target.outerHeight(true) - 1;
            var panelLeft = parseInt(targetOffset.left + 5);

            divMoreUsers.css('top', panelTop + 'px');
            divMoreUsers.css('left', panelLeft + 'px');
            var zIndex = divMoreUsers.css('zIndex') - 1;
            divMoreUsers.css('display', '');
            divMoreUsers.outerHeight(WorkflowMoreUsersDiv.outerHeight() + 13);

            var divHeight = divMoreUsers.outerHeight(true);
            if (targetOffset.top + divHeight + 18 > windowHeight) {
                panelTop = targetOffset.top - divHeight - 2;
            }
            divMoreUsers.css('top', panelTop + 'px');
            return false;
        });
    }

    workflowStatus.prototype.gridRenderComplete = function gridRenderComplete(eventType, gridId) {
        setTimeout(function () {
            $("#" + gridId + "_footer_Div").hide();
            resizeGridsWrapper($(".WorkflowTabActive").attr("targetdivid"));
        }, 20);
    }

    workflowStatus.prototype.gridRenderCompleteNeo = function gridRenderCompleteNeo(eventType, gridId) {
        setTimeout(function () {
            $("#" + gridId + "_footer_Div").hide();
        }, 20);
    }

    function addGroupBySection(gridId, groupKey, isGrouped) {
        $("#" + gridId + "_footer_Div").hide();
        $("#" + gridId + "_groupingInUpperHeader").hide();
        $("#" + gridId + "_configutation").hide();
        $("#" + gridId + "_groupingInUpperHeader").parent().append('<div class="workflowGroupSecurityDiv"><input class="workflowGridViewCheckbox" ' + ((isGrouped) ? ('checked="checked"') : '') + ' type="checkbox"/><span>Group By ' + groupKey + '</span></div>');
    }

    function resizeGrids(resizeMyRequestsGrids, resizeRejectedRequestsGrid, resizeRequestsPendingGrid, resizeAllRequestsGrids) {
        var gridId = null;
        var gridHeightSubtractor = 30;
        var WorkflowTabsContainer = $(".WorkflowTabsContainer");
        var workflowMyRequestsGridContainer = $('#workflowMyRequestsGridContainer');
        var workflowAllRequestsGridContainer = $('#workflowAllRequestsGridContainer');
        var RequestsGridContainer;
        if (resizeMyRequestsGrids || resizeAllRequestsGrids) {
            if (resizeMyRequestsGrids) {
                RequestsGridContainer = workflowMyRequestsGridContainer;
            }
            else if (resizeAllRequestsGrids) {
                RequestsGridContainer = workflowAllRequestsGridContainer;
            }
            WorkflowTabsContainer.css("padding-bottom", "0px");
            RequestsGridContainer.height($(window).height() - 50);

            var workflowUserContainerActive = RequestsGridContainer.find(".workflowUserContainerActive");
            if (workflowUserContainerActive.length > 0) {
                var workflowUserContainer = RequestsGridContainer.find(".workflowUserContainer");
                var gridDiv = workflowUserContainerActive.next().children(":eq(0)");
                var windowHeight = viewportSize.getHeight();
                var panelTopHeight = $("#" + workflowstatus._controlIdInfo.PanelTopId).height();
                var panelBottomHeight = $("#" + workflowstatus._controlIdInfo.PanelBottomId).height();

                gridId = gridDiv.prop("id");
                var gridBodyHeigth = $("#" + gridId + "_bodyDiv_Table").outerHeight(true);
                var gridHeaderHeigth = $("#" + gridId + "_headerDiv").outerHeight(true);
                var gridUpperHeaderHeigth = $("#" + gridId + "_upperHeader_Div").outerHeight(true);
                var gridFooterHeigth = $("#" + gridId + "_footer_Div").outerHeight(true);

                var gridHeightOriginal = gridBodyHeigth + gridHeaderHeigth + gridUpperHeaderHeigth + gridFooterHeigth;

                gridHeightSubtractor = 30 + (workflowUserContainer.length * (workflowUserContainer.eq(0).outerHeight(true)));
                var gridHeightWithoutSubtractor = windowHeight - (panelTopHeight + panelBottomHeight);
                if (gridHeightWithoutSubtractor - gridHeightOriginal - gridHeightSubtractor - 30 > 0) {
                    gridHeightSubtractor = gridHeightWithoutSubtractor - gridHeightOriginal - 30;
                }
                else if (gridHeightWithoutSubtractor - gridHeightSubtractor < 300) {
                    gridHeightSubtractor = gridHeightWithoutSubtractor - 330;
                }
            }
        }
        else if (resizeRejectedRequestsGrid) {
            WorkflowTabsContainer.css("padding-bottom", "7px");
            gridId = WorkFlowRejectedRequestsGridId;
        }
        else if (resizeRequestsPendingGrid) {
            WorkflowTabsContainer.css("padding-bottom", "7px");
            gridId = WorkFlowRequestsPendingGridId;
        }

        if (gridId != null) {
            resizeGridFinal(gridId, workflowstatus._controlIdInfo.PanelTopId, workflowstatus._controlIdInfo.PanelMiddleId, workflowstatus._controlIdInfo.PanelBottomId, 0, gridHeightSubtractor);

            //var grid = $find(gridId);
            //if (grid != null)
            //    grid.refreshWithCache();

            setTimeout(function () { $("#" + gridId + "_footer_Div").hide(); }, 20);

            if (resizeMyRequestsGrids) {
                setTimeout(function () { applySlimScroll('#workflowMyRequestsGridContainer', '#workflowMyRequestsScrollContainer'); }, 300);
            }
            else if (resizeAllRequestsGrids) {
                setTimeout(function () { applySlimScroll('#workflowAllRequestsGridContainer', '#workflowAllRequestsScrollContainer'); }, 300);
            }
        }
    }

    function FormatTabDivs() {
        var WorkflowTabActive = $(".WorkflowTabActive");
        var targetdivid = WorkflowTabActive.attr("targetdivid");
        var targetdiv = $("#" + targetdivid);

        $(".workflowGridDiv").hide();
        targetdiv.show();
    }

    function DefaultingActionButtons() {
        var WorkflowActionButtonsSeparated = $(".WorkflowActionButtonsSeparated");
        var WorkflowActionButtonsGrouped = $(".WorkflowActionButtonsGrouped");
        var WorkflowActionRevoke = $(".WorkflowActionRevoke");
        var WorkflowActionRejected = $(".WorkflowActionRejected");
        var WorkflowActionPending = $(".WorkflowActionPending");
        var WorkflowActionButtonsDiv = $(".WorkflowActionButtonsDiv");

        WorkflowActionPending.hide().attr("disabled", "disabled");
        WorkflowActionButtonsDiv.hide();

        WorkflowActionRejected.hide().attr("disabled", "disabled");
        WorkflowActionRevoke.hide().attr("disabled", "disabled");
        if (!WorkflowActionButtonsSeparated.hasClass("WorkflowActionButtonsSeparatedDisabled"))
            WorkflowActionButtonsSeparated.addClass("WorkflowActionButtonsSeparatedDisabled");
        if (!WorkflowActionButtonsGrouped.hasClass("WorkflowActionButtonsGroupedDisabled"))
            WorkflowActionButtonsGrouped.addClass("WorkflowActionButtonsGroupedDisabled");

        $(".WorkflowGridHandler").find("input[type='checkbox'][isheader='false']:checked").trigger("click");

        switch ($(".WorkflowTabActive").attr("targetdivid")) {
            case "workflowMyRequests":
                {
                    WorkflowActionRevoke.show();
                    WorkflowActionButtonsDiv.show();
                    break;
                }
            case "workflowRejectedRequests":
                {
                    WorkflowActionRejected.show();
                    WorkflowActionButtonsDiv.show();
                    break;
                }
            case "workflowRequestsPending":
                {
                    WorkflowActionPending.show();
                    WorkflowActionButtonsDiv.show();
                    break;
                }
        }

        $(".WorkflowExcludeRef").hide();
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

    function AttachHandlers() {
        $('#panelSave').unbind('click').click(onClickPanelSave);
        $('#chkListExternalSystemSave').unbind('click').click(onClickChkListExternalSystem);

        $(".WorkflowTabHandler").unbind("click").click(function (e) {
            var target = $(e.target);
            var selectedTabThis = target.attr("selectedtab");
            if (selectedTabThis != selectedTab) {
                workflowstatus.isFirstTimeBinding = true;
                $(".TabscssAfter").removeClass("TabscssAfter");
                target.addClass("TabscssAfter");
                selectedTab = selectedTabThis;
                Init();
            }
        });

        $(".WorkflowTab").unbind("click").click(function (e) {
            var target = $(e.target);
            if (!target.hasClass("WorkflowTab"))
                target = target.parents(".WorkflowTab").first();
            var targetdivid = target.attr("targetdivid");
            var targetdiv = $("#" + targetdivid);

            $(".workflowGridDiv").hide();
            $(".WorkflowTabActive").removeClass("WorkflowTabActive");
            targetdiv.show();
            target.addClass("WorkflowTabActive");

            DefaultingActionButtons();
            resizeGridsWrapper(targetdivid);
        });

        $("#imgWorkflowActions, #btnWorkflowActionCancel").unbind("click").click(function (e) {
            enableDivs();
            $('#panelWorkflowActions').hide();
        });

        $("#imgImpactedSecurities").unbind("click").click(function (e) {
            enableDivs();
            $('#panelImpactedSecurities').hide();

            var alertBG = $('.alertBG');
            alertBG.css('background-color', alertBG.attr("backgroundColorPrev"));
            alertBG.css('opacity', alertBG.attr("opacityPrev"));
        });

        $("#workflowRefreshButton").unbind("click").click(function (e) {
            workflowstatus.isFirstTimeBinding = false;
            Init();
        });

        $("#imgViewLog").unbind("click").click(function (e) {
            enableDivs();
            $('#panelViewLog').hide();
            if (workflowstatus.currentRequestObject != null && workflowstatus.currentRequestObject.guid != null && workflowstatus.currentRequestObject.guid.length > 0) {
                ExecuteSynchronously(path + '/BaseUserControls/Service/CommonService.svc', 'RemoveKey', { guid: workflowstatus.currentRequestObject.guid });
            }
            workflowstatus.currentRequestObject = null;
        });

        $("#imgLegData").unbind("click").click(function (e) {
            enableDivs();
            $('#panelLegData').hide();
        });

        $("#btnWorkflowAction").unbind("click").click(function (e) {
            var target = $(e.target);
            var targetId = target.prop('id');
            var action = target.attr("action");
            var remarks = $('#workflowActionsRemarks').val().trim();

            if ((action == "Reject" || action == "Delete" || action == "Suppress") && remarks.length == 0) {
                $('#panelWorkflowActions').height(123);
                $("#workflowActionsErrorDiv").text("* Remarks are mandatory in case of " + action + " action.").show();
                return false;
            }

            var gridId = null;
            var grid = null;
            var cacheKey = null;
            switch ($(".WorkflowTabActive").attr("targetdivid")) {
                case "workflowMyRequests":
                    {
                        var workflowUserContainerActive = $(".workflowUserContainerActive");
                        if (workflowUserContainerActive.length > 0) {
                            gridId = workflowUserContainerActive.next().children(":eq(0)").prop("id");
                        }
                        break;
                    }
                case "workflowRejectedRequests":
                    {
                        gridId = WorkFlowRejectedRequestsGridId;
                        break;
                    }
                case "workflowRequestsPending":
                    {
                        gridId = WorkFlowRequestsPendingGridId;
                        break;
                    }
            }
            if (gridId != null)
                grid = $find(gridId);
            if (grid != null)
                cacheKey = grid.get_CacheKey();

            enableDivs();
            $('#panelWorkflowActions').hide();

            if (cacheKey != null) {
                onServiceUpdating();
                var rowKeys = ExecuteSynchronously(path + '/BaseUserControls/Service/CommonService.svc', 'GetCheckedRowKeys', { cacheKey: cacheKey, gridType: 4 }).d;

                workflowstatus.currentRequestObject = {};
                workflowstatus.currentRequestObject.userName = workflowstatus._securityInfo.UserName;
                workflowstatus.currentRequestObject.remarks = remarks;
                workflowstatus.currentRequestObject.action = WorkflowActionMapping[action];

                workflowstatus.currentRequestText = WorkflowRequestTextMapping[action];

                var params = {};
                params.rowKeys = rowKeys;
                params.userName = workflowstatus._securityInfo.UserName;
                params.remarks = remarks;
                params.action = WorkflowActionMapping[action];
                params.moduleName = selectedTab;
                CallCommonServiceMethod('WorkflowImpactedSecuritiesCheck', params, onSuccess_impactedSecuritiesCheck, onFailure_impactedSecuritiesCheck, null, false);
            }
            else {
            }
        });

        $(".WorkflowActionButtons").unbind("click").click(function (e) {
            var target = $(e.target);
            var targetId = target.prop('id');
            var action = target.attr("action");
            var panelWorkflowActions = $('#panelWorkflowActions');

            $("#workflowActionsErrorDiv").hide();

            $('#workflowActionsPopupHeader').text(action);
            $('#workflowActionsRemarks').val("");
            $('#btnWorkflowAction').val(action).prop("tooltip", "Click to " + action).attr("action", action);

            var targetOffset = target.offset();
            var panelTop = targetOffset.top + target.outerHeight(true) + 6;
            var panelLeft = parseInt(targetOffset.left + target.outerWidth(true) - panelWorkflowActions.outerWidth(true) + 5);
            //            var panelTop = $(window).height() / 2 - panelWorkflowActions.outerHeight() / 2;
            //            var panelLeft = $(window).width() / 2 - panelWorkflowActions.outerWidth() / 2;
            panelWorkflowActions.css('top', panelTop + 'px').css('left', panelLeft + 'px');
            var zIndex = panelWorkflowActions.css('zIndex') - 1;
            disableDivs(zIndex);
            panelWorkflowActions.height(106).show();
        });

        $(document).unbind("click").click(function (e) {
            var target = $(e.target);
            var targetId = target.prop('id');
            if (targetId != 'divMoreUsers' && targetId != 'WorkflowMoreUsersDiv' && !target.hasClass("WorkflowMoreUsers"))
                $('#divMoreUsers').hide();
        });

        $(window).unbind("resize").resize(function (e) {
            setTimeout(function () {
                resizeGridsWrapper($(".WorkflowTabActive").attr("targetdivid"));
            }, 500);
        });

        $(window).unbind("unload").unload(function (e) {
            ExecuteSynchronously(path + '/BaseUserControls/Service/CommonService.svc', 'ClearGridDataForUser', { userSessionIdentifier: workflowstatus.userSessionIdentifier }).d;
            if (workflowstatus.currentRequestObject != null && workflowstatus.currentRequestObject.guid != null && workflowstatus.currentRequestObject.guid.length > 0) {
                ExecuteSynchronously(path + '/BaseUserControls/Service/CommonService.svc', 'RemoveKey', { guid: workflowstatus.currentRequestObject.guid });
            }
        });
    }

    function resizeGridsWrapper(id) {
        switch (id) {
            case "workflowMyRequests":
                {
                    resizeGrids(true, false, false, false);
                    break;
                }
            case "workflowRejectedRequests":
                {
                    resizeGrids(false, true, false, false);
                    break;
                }
            case "workflowRequestsPending":
                {
                    resizeGrids(false, false, true, false);
                    break;
                }
            case "workflowAllRequests":
                {
                    resizeGrids(false, false, false, true);
                    break;
                }
            default:
                {
                    resizeGrids(true, false, false, false);
                    break;
                }
        }
    }

    function onSuccess_viewLog(result) {
        if (result.d != null) {
            result = result.d;

            var panelViewLog = $('#panelViewLog');
            var viewLogPopupDiv = $('#viewLogPopupDiv');
            //            var viewLogPopupTop = $('#viewLogPopupTop');

            viewLogPopupDiv.html("<div>" + result.html + "</div>");
            //            if (result.htmlTop != null && result.htmlTop.length > 0) {
            //                viewLogPopupTop.html("<div style='text-align: center; padding-bottom: 18px;'>" + result.htmlTop + "</div>");
            //                viewLogPopupTop.show();
            //            }
            //            else {
            //                viewLogPopupTop.hide();
            //            }

            //            viewLogPopupDiv.find(".panelHead").css({ "padding-left": "0px", "padding-bottom": "3px", "text-transform": "uppercase" });

            var panelTop = $(window).height() / 2 - panelViewLog.outerHeight() / 2;
            var panelLeft = $(window).width() / 2 - panelViewLog.outerWidth() / 2;
            panelViewLog.css('top', panelTop + 'px').css('left', panelLeft + 'px');
            var zIndex = panelViewLog.css('zIndex') - 1;
            disableDivs(zIndex);
            panelViewLog.show();

            if (result.isGridAvailable) {
                applySlimScroll('#viewLogPopupDiv', 'div:first');
            }

            AttachMoreUsersClickHandler("#panelViewLog .WorkflowMoreUsers");
        }
    }

    function onSuccess_performAction(result) {
        if (result.d != null) {
            result = result.d;
            var panelImpactedSecurities = $('#panelImpactedSecurities');
            if (panelImpactedSecurities.css("display") != "none")
                panelImpactedSecurities.slideToggle(); //hide();
            onActionComplete(result);
        }
        onServiceUpdated();
    }

    function onSuccess_performAction_RM(result) {
        if (result.d != null) {
            result = result.d;
            var panelImpactedSecurities = $('#panelImpactedSecurities');
            if (panelImpactedSecurities.css("display") != "none")
                panelImpactedSecurities.slideToggle(); //hide();
            onActionComplete_RM(result);
        }
        onServiceUpdated();
    }

    function onSuccess_impactedSecuritiesCheck(result) {
        if (result.d != null) {
            result = result.d;
            if (result.IsActionComplete) {
                onActionComplete(result.handlerResponse);
                if (result.guid != null && result.guid.length > 0) {
                    ExecuteSynchronously(path + '/BaseUserControls/Service/CommonService.svc', 'RemoveKey', { guid: result.guid });
                }
            }
            else if (result.IsImpactedCheck) {
                workflowstatus.currentRequestObject.statusIds = result.statusIds;
                workflowstatus.currentRequestObject.guid = result.guid;

                var panelImpactedSecurities = $('#panelImpactedSecurities');
                var impactedSecuritiesPopupDiv = $('#impactedSecuritiesPopupDiv');
                var alertBG = $('.alertBG');

                impactedSecuritiesPopupDiv.html("<div>" + result.ImpactedHTML + "</div>");

                if (selectedTab == WorkflowTabs.RefMaster) {
                    $('#impactedSecuritiesPopupHeader').text('Impacted Entities');
                }

                var workflowImpactedTablePassed = $('#workflowImpactedTablePassed');
                var workflowImpactedTablePassedHeight = 0;
                if (workflowImpactedTablePassed.length > 0)
                    workflowImpactedTablePassedHeight = workflowImpactedTablePassed.outerHeight();

                var workflowImpactedTableFailed = $('#workflowImpactedTableFailed');
                var workflowImpactedTableFailedHeight = 0;
                if (workflowImpactedTableFailed.length > 0)
                    workflowImpactedTableFailedHeight = workflowImpactedTableFailed.outerHeight();

                //                var panelTop = $(window).height() / 2 - panelImpactedSecurities.outerHeight() / 2;
                //                var panelLeft = $(window).width() / 2 - panelImpactedSecurities.outerWidth() / 2;
                //                panelImpactedSecurities.css('top', panelTop + 'px').css('left', panelLeft + 'px');
                var zIndex = panelImpactedSecurities.css('zIndex') - 1;
                disableDivs(zIndex);
                alertBG.attr("backgroundColorPrev", alertBG.css("background-color")).css('background-color', 'white');
                alertBG.attr("opacityPrev", alertBG.css("opacity")).css('opacity', '0.3');
                panelImpactedSecurities.slideToggle(); //css('display', '');
                //                impactedSecuritiesPopupDiv.outerHeight(workflowImpactedTablePassedHeight + workflowImpactedTableFailedHeight + 10);
                //                panelImpactedSecurities.outerHeight($('#impactedSecuritiesPopupHeader').outerHeight() + impactedSecuritiesPopupDiv.outerHeight() + 10);
                //                panelTop = $(window).height() / 2 - panelImpactedSecurities.outerHeight() / 2;
                //                panelImpactedSecurities.css('top', panelTop + 'px');

                var workflowImpactedScrollContainer = $(".workflowImpactedScrollContainer");
                if (workflowImpactedScrollContainer.length == 1) {
                    workflowImpactedScrollContainer.css("max-height", "330px");
                }
                else if (workflowImpactedScrollContainer.length == 2) {
                    var successSection = workflowImpactedScrollContainer.eq(0);
                    var failureSection = workflowImpactedScrollContainer.eq(1);
                    var successSectionHeight = successSection.find(".workflowImpactedTableContainer").outerHeight(true);
                    var failureSectionHeight = failureSection.find(".workflowImpactedTableContainer").outerHeight(true);
                    if (successSectionHeight < 165 && failureSectionHeight > 165) {
                        successSection.css("max-height", successSectionHeight + "px");
                        failureSection.css("max-height", (330 - successSectionHeight) + "px");
                    }
                    else if (successSectionHeight > 165 && failureSectionHeight < 165) {
                        failureSection.css("max-height", failureSectionHeight + "px");
                        successSection.css("max-height", (330 - failureSectionHeight) + "px");
                    }
                }

                applySlimScroll('#impactedSecuritiesPopupDiv', 'div:first');
                if (workflowImpactedScrollContainer.length > 0) {
                    applySlimScroll('.workflowImpactedScrollContainer:eq(0)', 'div:first');
                }
                if (workflowImpactedScrollContainer.length > 1) {
                    applySlimScroll('.workflowImpactedScrollContainer:eq(1)', 'div:first');
                }

                if (selectedTab == WorkflowTabs.RefMaster) {
                    if ($('#workflowImpactedTablePassed').length > 0)
                        $("#btnPerformActionRM").removeAttr("disabled");
                    else
                        $("#btnPerformActionRM").attr("disabled", "disabled");

                    $("#btnPerformActionRM").unbind("click").click(function (e) {
                        var target = $(e.target);

                        enableDivs();
                        onServiceUpdating();
                        CallCommonServiceMethod('RMWorkflowRequestHandler', workflowstatus.currentRequestObject, onSuccess_performAction_RM, onFailure_performAction, null, false);
                        workflowstatus.currentRequestObject = null;
                        alertBG.css('background-color', alertBG.attr("backgroundColorPrev"));
                        alertBG.css('opacity', alertBG.attr("opacityPrev"));
                    });
                }

                else {
                    if ($('#workflowImpactedTablePassed').length > 0)
                        $("#btnPerformAction").removeAttr("disabled");
                    else
                        $("#btnPerformAction").attr("disabled", "disabled");

                    $("#btnPerformAction").unbind("click").click(function (e) {
                        var target = $(e.target);

                        enableDivs();
                        onServiceUpdating();
                        CallCommonServiceMethod('WorkflowRequestHandler', workflowstatus.currentRequestObject, onSuccess_performAction, onFailure_performAction, null, false);
                        workflowstatus.currentRequestObject = null;
                        alertBG.css('background-color', alertBG.attr("backgroundColorPrev"));
                        alertBG.css('opacity', alertBG.attr("opacityPrev"));
                    });

                }

                $("#impactedSecuritiesPopupDiv .secIdClick").unbind("click").click(function (e) {
                    var target = $(e.target);
                    var securityId = target.attr("secId");
                    SecMasterJSCommon.SMSCommons.openWindowForSecurity(true, securityId, true, true, SecMasterJSCommon.SMCreateUpdateOpenByDefault.None, 0, null, 3);
                });
            }
        }
        onServiceUpdated();
    }

    function onActionComplete(result) {
        if (result.IsSuccess) {
            workflowstatus.gridUpdating = true;
            switch ($(".WorkflowTabActive").attr("targetdivid")) {
                case "workflowMyRequests":
                    {
                        BindGrids(true, false, false, false);
                        break;
                    }
                case "workflowRejectedRequests":
                    {
                        WorkFlowRejectedRequestsGridId = "workflowRejectedRequestsGrid_" + GetGUID();
                        BindGrids(false, true, false, false);
                        break;
                    }
                case "workflowRequestsPending":
                    {
                        WorkFlowRequestsPendingGridId = "workflowRequestsPendingGrid_" + GetGUID();
                        BindGrids(false, false, true, false);
                        break;
                    }
                default:
                    workflowstatus.gridUpdating = false;
                    break
            }

            showPerformActionSuccess(result);
        }
        else {
            $("#" + workflowstatus._controlIdInfo.LblErrorId).html(result.FailureMessage);
            $find(workflowstatus._controlIdInfo.ModalErrorId).show();
        }
    }

    function onActionComplete_RM(result) {

        var isSuccess = true;

        if (result != null && result != undefined) {

            for (var i = 0; i < result.length; i++) {
                if (result[i].Key.IsSuccess != true)
                    isSuccess = false;
            }
        }


        if (isSuccess == true) {
            workflowstatus.gridUpdating = true;
            switch ($(".WorkflowTabActive").attr("targetdivid")) {
                case "workflowMyRequests":
                    {
                        BindGrids(true, false, false, false);
                        break;
                    }
                case "workflowRejectedRequests":
                    {
                        WorkFlowRejectedRequestsGridId = "workflowRejectedRequestsGrid_" + GetGUID();
                        BindGrids(false, true, false, false);
                        break;
                    }
                case "workflowRequestsPending":
                    {
                        WorkFlowRequestsPendingGridId = "workflowRequestsPendingGrid_" + GetGUID();
                        BindGrids(false, false, true, false);
                        break;
                    }
                default:
                    workflowstatus.gridUpdating = false;
                    break
            }

            showPerformActionSuccess_RM(result);
        }
        else {
            $("#" + workflowstatus._controlIdInfo.LblErrorId).html(result.FailureMessage);
            $find(workflowstatus._controlIdInfo.ModalErrorId).show();
        }
    }

    function showPerformActionSuccess(result) {
        if (workflowstatus.gridUpdating) {
            setTimeout(function () { showPerformActionSuccess(result); }, 1000);
        }
        else {
            DefaultingActionButtons();

            if (result.secIds != null && result.secIds.length > 0) {
                secIds = result.secIds;
                //var zIndex = $('#panelSave').css('zIndex') - 1;
                //disableDivs(zIndex);

                var chkListExternalSystemSave = $('#chkListExternalSystemSave');
                var defaultSelectedCheckBoxes = chkListExternalSystemSave.find("input[type='checkbox'][realTimePosting='true']");
                chkListExternalSystemSave.find("input[type='checkbox']").prop("checked", "");
                if (defaultSelectedCheckBoxes.length > 0) {
                    defaultSelectedCheckBoxes.trigger("click"); //.prop("checked", "checked");
                    chkListExternalSystemSave.css('display', 'table');
                    $('#postToDivPanel').css('display', 'block');
                    $('#divChkListExternalSystemSave').width(440);
                }
                else {
                    chkListExternalSystemSave.css('display', 'none');
                    $('#postToDivPanel').css('display', 'none');
                    $('#option_container').css('display', 'none');
                    $('#divChkListExternalSystemSave').width(250);
                    $('#btnSaveOk').css('display', 'none');
                }
                // showAsDialog('panelSave');
                $find(workflowstatus._controlIdInfo.ModalPanelSaveId).show();

            }
            else {
                $("#" + workflowstatus._controlIdInfo.LblSuccessId).text(workflowstatus.currentRequestText + " successfully.");
                $find(workflowstatus._controlIdInfo.ModalSuccessId).show();

            }
        }
    }

    function showPerformActionSuccess_RM(result) {
        if (workflowstatus.gridUpdating) {
            setTimeout(function () { showPerformActionSuccess_RM(result); }, 1000);
        }
        else {
            DefaultingActionButtons();

            //if (result.secIds != null && result.secIds.length > 0) {
            if (result != null && result != undefined && result[0].Key != null && result[0].Key != undefined && result[0].Key.secIds != null && result[0].Key.secIds.length > 0) {
                secIds = result[0].Key.secIds;
                //var zIndex = $('#panelSave').css('zIndex') - 1;
                //disableDivs(zIndex);

                var chkListExternalSystemSave = $('#chkListExternalSystemSave');
                var defaultSelectedCheckBoxes = chkListExternalSystemSave.find("input[type='checkbox'][realTimePosting='true']");
                chkListExternalSystemSave.find("input[type='checkbox']").prop("checked", "");
                if (defaultSelectedCheckBoxes.length > 0) {
                    defaultSelectedCheckBoxes.trigger("click"); //.prop("checked", "checked");
                    chkListExternalSystemSave.css('display', 'table');
                    $('#postToDivPanel').css('display', 'block');
                    $('#divChkListExternalSystemSave').width(440);
                }
                else {
                    chkListExternalSystemSave.css('display', 'none');
                    $('#postToDivPanel').css('display', 'none');
                    $('#option_container').css('display', 'none');
                    $('#divChkListExternalSystemSave').width(250);
                    $('#btnSaveOk').css('display', 'none');
                }
                //showAsDialog('panelSave');
                $find(workflowstatus._controlIdInfo.ModalPanelSaveId).show();
            }
            else {
                $("#" + workflowstatus._controlIdInfo.LblSuccessId).text(workflowstatus.currentRequestText + " successfully.");
                $find(workflowstatus._controlIdInfo.ModalSuccessId).show();
            }
        }
    }

    function showAsDialog(targetDivId) {

        var target = $("#" + targetDivId);
        target.css("z-index", "10005");
        target.css("position", "absolute");
        target.css("top", 0);
        target.css("left", 0);
        var subHeight = (target[0].offsetHeight / 2);
        var subWidth = target[0].offsetWidth / 2;

        var top = ($(document.body).height() / 2) - target.height() / 2;
        var left = ($(document.body).width() / 2) - target.width() / 2;
        target.css("top", top);
        target.css("left", left);
        target.css("display", "");

    }

    function onFailure_performAction(result) {
        var panelImpactedSecurities = $('#panelImpactedSecurities');
        if (panelImpactedSecurities.css("display") != "none")
            panelImpactedSecurities.slideToggle(); //hide();
        $("#" + workflowstatus._controlIdInfo.LblErrorId).text("Failed to perform action. Service returned an error" + ((result.responseJSON == null || result.responseJSON.Message == null || result.responseJSON.Message.length == 0) ? "" : (" : " + result.responseJSON.Message)) + ((result.responseText == null || result.responseText.length == 0) ? "" : (" : " + result.responseText)));
        $find(workflowstatus._controlIdInfo.ModalErrorId).show();
        onServiceUpdated();
    }

    function onFailure_impactedSecuritiesCheck(result) {
        $("#" + workflowstatus._controlIdInfo.LblErrorId).text("Failed to perform action. Service returned an error" + ((result.responseJSON == null || result.responseJSON.Message == null || result.responseJSON.Message.length == 0) ? "" : (" : " + result.responseJSON.Message)) + ((result.responseText == null || result.responseText.length == 0) ? "" : (" : " + result.responseText)));
        $find(workflowstatus._controlIdInfo.ModalErrorId).show();
        onServiceUpdated();
    }

    function OnFailure(result) {
        $("#" + workflowstatus._controlIdInfo.LblErrorId).text("Service returned an error" + ((result.responseJSON == null || result.responseJSON.Message == null || result.responseJSON.Message.length == 0) ? "" : (" : " + result.responseJSON.Message)) + ((result.responseText == null || result.responseText.length == 0) ? "" : (" : " + result.responseText)));
        $find(workflowstatus._controlIdInfo.ModalErrorId).show();
        workflowstatus.gridUpdating = false;
    }

    function GetGUID() {
        var objDate = new Date();
        var str = objDate.toString();
        str = objDate.getDate().toString() + objDate.getMonth().toString() + objDate.getFullYear().toString() + objDate.getHours().toString() + objDate.getMinutes().toString() + objDate.getSeconds().toString() + objDate.getMilliseconds().toString() + eval('Math.round(Math.random() * 10090)').toString();
        return str;
    }

    return workflowstatus;
})();