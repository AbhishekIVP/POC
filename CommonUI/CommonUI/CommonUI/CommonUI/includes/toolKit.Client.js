/*! grid-toolkit - v1.0.0 - 2017-12-12 7:38:47 */
var toolKit = {
    buttonBarHeaderId: "buttonBarHeader",
    toolKitBody: "toolKitBody",
    baseUrl: getBaseUrl(),
    identifier: "",
    tabCounter: 1,
    ShowReport:false,
    tabInfo: getTabInfo(),
    scrollDownContent: 0,
    parametersForProcedure: [],
    TablesCollection: [],
    DBName: "",
    identifierForReport: "",
    reportName: "",
    IsNewReport: false,
    identifierForReport: "test",
    viewKey: "",
    userId: "",
    CurrentPageId: "",
    DBName: "IVPRAD",
    DBConnectionID: "",
    IsFromContextMenu:false,
    DataSourceCollection: [],
    dbIdentifier: "",
    DataSourceAndIdentifier: { "IVPRAD": "radDB" },
    ProcedureViewModel: ProceduresViewModel(),
    TableViewModel: TablesViewModel(),
    ProcedureParamViewModel: ProceduresParamViewModel(),
    CurrentTab: "",
    CurrentTabInfo : {},
    ReportViewModel:ReportsViewModel(),

    filterDetails: [],
    CurrentTab:"",
    reportInfo: {},
    newTabAdded: false,
    tabContainer: "toolKitTabContainer",
    IsConfigure:false,
    tabNameIDMapping: {},
    ListOfTabInfo:[],
    TabReportType:[],
    ReportType: "",
    AssemblyName: "PRadGridReportingToolkit",
    ClassName: "PRadGridReportingToolkit.RadGridReportingToolkit",
    dbIdentifierList: [],
    RequireTagLink: true,
    RequireDataDictionary:true
}

function getBaseUrl() {
    var re = /.*Home.aspx/gi;
    var url = window.location.href;
    var m = re.exec(url);
    if (m === null) return url;
    else return m[0].replace("/Home.aspx", "");
}
function getTabInfo() {
    var a = { tab: [
    {
        id: "df",
        name: "Default Tab",
        isDefault: true
    },
     {
         id: "gfh",
         name: "Default Tab1",
         isDefault: false
     }
    ]
    }
    return a;
}
function ProceduresViewModel() {
    return function () {
        this.ProcedureSource = ko.observableArray();
        this.ProceduresClickHandler = function (event) {
            toolKit.addParametersForProcedure.CreateParametersScreen(event, toolKit.DataSourceCollection);
        }
    }
}

function TablesViewModel() {
    return function () {
        this.TablesSource = ko.observableArray();
        this.TablesClickHandler = function (model, event) {
            //toolKit.addColumnsForTable.CreateColumnsScreen(event, toolKit.TablesCollection);
            $(".dbIdentifierParent").remove();
            $("#globalFilterPopUp").remove();
            $(".tableSourceItemSelected").remove();
            var tableName = $(event.target).text();
            $(".dataSourceHeader").empty();
            $(".dataSourceHeader").animate({ height: "45px" });
            $(".TextParent").addClass("TextParentDisplay");
            $(".DataSourceBody").addClass("DataSourceBodyDisplay");
            $(".addTableSourcePopup").hide("blind", 1000, function () {
                //toolKit.CurrentTabInfo.DataSourceInfo.TableOrViewName = tableName.replace("dbo.","");
                toolKit.addColumns.CreatePopUp($(event.target).text().replace("dbo.", ""));
                $(".saveTabInfo").removeClass("saveTabInfoHidden");
            });
        }
    }
}

function ProceduresParamViewModel() {
    return function () {
        this.ProcedureParameterSourceName = ko.observableArray();
        this.ProceduresParamClickHandler = function (event) {
            toolKit.addParametersForProcedure.CreateGlobalFilter(event);
        }
    }
}
function ReportsViewModel() {
    return function () {
        this.ReportList = ko.observableArray();
       // this.UserMappingString = ko.computed();
        this.ReportClickHandler = function (model, event) {
            alert("hello")
        }
        this.ReportUserMappingClickHandler = function (model, event) {
            toolKit.ExistingReports.ReportUserHandler(model, event);
        }
    }
}
      

$.extend(toolKit, {
    handlers: {
        initialize: function initialize(obj, existingViewInfo) {
            var self = this;
            $.extend(toolKit, obj);
            if (obj.dbIdentifierList != null)
                obj.dbIdentifierList.sort();
            if(toolKit.dbIdentifierList.length > 0)
                toolKit.dbIdentifier = toolKit.dbIdentifierList[0];
            if (toolKit.ShowReport) {
                $("#" + toolKit.identifier).append("<div class=\"dataSourceHeader\"></div>");
                $("#" + toolKit.identifier).append("<div id=\"" + toolKit.tabContainer + "\"></div>");
                $("#" + toolKit.tabContainer).height($("#" + toolKit.identifier).height() - $(".dataSourceHeader").height());
                if ($("#pageHeaderCustomDiv").find("#toolKitRightfilter").length == 0)
                    $("#pageHeaderCustomDiv").append("<div id=\"toolKitRightfilter\" class=\"toolKitRightfilter\"></div>");
                if ($(".iago-page-title").find(".toolKitAddTab").length == 0) {
                    if (!toolKit.ShowReport)
                        $(".iago-page-title").append("<div class=\"fa fa-plus-circle toolKitAddTab\" aria-hidden=\"true\"></div>");
                }
                if ($(".btnGlobalParameter").length == 0) {
                    $("#pageHeaderCustomDiv").append("<div class='btnGlobalParameterParent'><div class='btnGlobalParameter'>Parameter</div></div>");
                }
                if ($("#pageHeaderCustomDiv").find(".btnReportConfig").length == 0 && !toolKit.ShowReport) {
                    $("#pageHeaderCustomDiv").append("<div class='btnReportConfigParent'><div class=\"btnReportConfig\" >All Reports</div></div>");
                    $(".btnReportConfig").unbind("click").click(function () {
                        toolKit.ShowReport = false;
                        toolKit.ExistingReports.CreateReportPopUp();
                    })
                }
                toolKit.handlers.getToolKItInfo();
            }
            else {
                toolKit.ExistingReports.CreateReportPopUp();
                //toolKit.addTabInfo.createPopUp();
                //                toolKit.newTabAdded = true;
                //                self.bindTabs();
            }
            self.buttonBarClickHandler();
        },

        getToolKItInfo: function () {
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                url: url + '/GetToolkitInfo',
                data: JSON.stringify({ reportName: toolKit.reportName, user: iago.user.userName, moduleId: toolKit.moduleid }),
                dataType: "json"
            }).then(function (responseText) {
                if ($.parseJSON(responseText.d).TabsDetails.length > 0) {
                    toolKit.reportInfo = $.parseJSON(responseText.d);
                    toolKit.CurrentTabInfo = toolKit.reportInfo.TabsDetails[0];
                    if (toolKit.CurrentTabInfo.dbIdentifier != "")
                        toolKit.dbIdentifier = toolKit.CurrentTabInfo.dbIdentifier;
                    toolKit.bindTabs.bindTab();
                }
                else {
                    toolKit.reportInfo = $.parseJSON(responseText.d);
                    $("#" + toolKit.identifier).empty();
                    toolKit.addTabInfo.createPopUp();
                }
                //$("#pageHeaderCustomDiv").append("<div class='btnConfigureScreenParent'><div id=\"btnConfigureScreen\" class=\"btnConfigureScreen\">Configure Report</div></div>");
            });
        },

        bindTabs: function () {
            var self = this;
            self.tabInfo = []
            //div.find("ul").append("<li class=\"existingTab\"><div contenteditable=\"true\">" + tabNames[i] + "</div></li>");
            self.tabInfo.push({
                id: "Default",
                name: "Default",
                isDefault: true
            })
            $("#pageHeaderTabPanel").toolKittabs({
                tabSchema: { tab: self.tabInfo },
                tabClickHandler: this.tabClickHandler,
                tabContentHolder: toolKit.tabContainer
            });
        },



        buttonBarClickHandler: function () {
            $("#pageHeaderCustomDiv").unbind("click").click(function (event) {
                if (event.target.className == "btnConfigureScreen") {
                    $(event.target).remove();
                    toolKit.tabContentContainer.empty();
                    if (toolKit.CurrentTabInfo.DataSourceInfo.TableOrViewName != "") {
                        toolKit.addDataSource.createDataSourcePopUp();
                        toolKit.addTables.createTablesdropDown();
                    }
                    else {
                        toolKit.reportType.createPopUp(false);
                    }
                }
                if (event.target.className == "btnGlobalParameter") {
                    if ($("#globalFilterPopUp").length > 0) {
                        $("#globalFilterPopUp").remove();
                    }
                    else {
                        toolKit.globalFilter.createPopUp();
                    }
                }
                if (event.target.className == "btnCalendarName") {
                    toolKit.utility.getAllCalendars(event);
                }
                if (event.target.className == "btnEdit") {
                    toolKit.utility.isUserAdmin();
                }
                if (event.target.className == "btnAddTabRelation") {
                    if (toolKit.reportInfo.TabRelationInfo != null) {
                        if (toolKit.reportInfo.TabRelationInfo.length > 0) {
                            toolKit.ExistingTabRelation.addTabRelation(toolKit.reportInfo.TabsDetails, toolKit.reportInfo.TabRelationInfo, false);
                        }
                        else {
                            toolKit.ExistingTabRelation.addTabRelation(toolKit.reportInfo.TabsDetails, toolKit.reportInfo.TabRelationInfo, true);
                        }
                    }
                    else {
                        toolKit.ExistingTabRelation.addTabRelation(toolKit.reportInfo.TabsDetails, [], true);
                    }
                }
            });

            $(".iago-page-title").click(function (event) {
                if ($(event.target).hasClass("toolKitAddTab")) {
                    toolKit.addWidget.addTab(event);
                }
            })
            $(".saveTabInfo").unbind('click').click(function () {
                if (toolKit.addColumns.ValidationBeforeSavingTab() == true)
                    toolKit.addColumns.getColumnInfo();
            });
            $("#" + toolKit.buttonBarHeaderId).unbind("click").click(function (event) {
                toolKit.eventHandlers.buttonBarHandler(event);
            });
            $("#addGlobalParameters").unbind("click").click(function (event) {
                toolKit.addColumns.CreatePopUp();
            });
            $(".TextParent").unbind("click").click(function (event) {
                toolKit.eventHandlers.HeaderTextHandler(event);
            })
            $(".toolkitAddTabRelation").unbind("click").click(function (event) {
                toolKit.ExistingTabRelation.addTabRelation(toolKit.ListOfTabInfo, [], true);
            })
        },
        bindContextMenu: function (listOfRelatedTabs, gridid) {
            var contextMenuObject = {};

            for (var item in listOfRelatedTabs) {
                if (listOfRelatedTabs[item].IsTwoWay == false) {
                    if (toolKit.CurrentTabInfo.TabName == listOfRelatedTabs[item].PrimaryTabName)
                        contextMenuObject[listOfRelatedTabs[item]["SecondryTabName"]] = { "name": listOfRelatedTabs[item]["ContextMenuOption"].split('~~~')[1] }
                }
                else {
                    if (toolKit.CurrentTabInfo.TabName == listOfRelatedTabs[item].PrimaryTabName)
                        contextMenuObject[listOfRelatedTabs[item]["SecondryTabName"]] = { "name": listOfRelatedTabs[item]["ContextMenuOption"].split('~~~')[0] }
                    else if (toolKit.CurrentTabInfo.TabName == listOfRelatedTabs[item].SecondryTabName)
                        contextMenuObject[listOfRelatedTabs[item]["PrimaryTabName"]] = { "name": listOfRelatedTabs[item]["ContextMenuOption"].split('~~~')[1] }
                }
            }

            $("#" + gridid).contextMenu({ MenuItems: contextMenuObject, contextOn: function (option) {
                toolKit.bindTabs.LoadTabWithContextMenu(option);
            },
                callback: function (a, b) {
                    toolKit.bindTabs.ContextMenuHandler(a, b);
                }
            });
        }
    }
});
$.extend(toolKit, {
    addTabInfo: {
        createPopUp: function () {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + "/GetHtmlTemplate",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.addTabInfo.htm' })
            }).then(function (responseText) {
                self.tabHTML = responseText.d;
                $.ajax({
                    url: toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc/GetTabReportType",
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json'
                }).then(function (responseText) {
                    toolKit.TabReportType = $.parseJSON(responseText.d);
                    $("#pageHeaderTabPanel").append("<div class=\"unTitledTab\">Untitled</div>");
                    self.addTab();
                    //                    $(".rptCatValue").bootstrapToggle({ 'size': 'mini' });
                    //                    bootbox.dialog({
                    //                        message: self.tabHTML,
                    //                        title: 'Add Tab'
                    //                    });
                    //                    $(".divTabInfo").unbind("click").click(function () {
                    //                        self.clickEventHandler(event);
                    //                    })
                    //                    $("#addMultiSelect").bootstrapToggle({ 'size': 'mini' });
                });
            })
        },

        addTab: function () {
            $(".tabNamePopUp").remove();
            var div = $('<div>');
            div.addClass("tabNamePopUp");
            div.append("<div class=\"tabNameLabelParent\"><div class='tabNameLabelName'>Tab Name</div><div class='tabNameInput'><input class='inputTabName'/></div><div id=\"tabNameValidation\" title = \"Tab Name Already Exists\" class=\"tabNameValidation hiddenClass\" >!</div></div>");
            //div.append("<div class=\"tabNameLabelParent\"><div class='tabNameLabel'>is Advanced</div><div class='toggleParentDiv'> <div class=\"isAdvancedOn\">ON</div><div class=\"isAdvancedOff advanceSelected\">OFF</div></div></div>");
            if (toolKit.RequireDataDictionary)
                div.append("<div class=\"tabNameLabelParent\"><div class='tabNameLabel'>Use Dictionary</div><div class='toggleParentDiv'><div class=\"isAdvancedOff advanceSelected\">ON</div> <div class=\"isAdvancedOn\">OFF</div></div></div>");
            div.append("<div class=\"parentbtnAddTab\"><div class='btnAddTab'>Add Tab</div><div class=\"btnCancelTab\">Cancel</div></div>");
            $("#" + toolKit.identifier).append(div);
            div.css({ left: (10 + 'px') });
            toolKit.IsAdvanced = true;
            $(".inputAdvanced").bootstrapToggle({ 'size': 'mini' });
            toolKit.newTabAdded = true;

            $(".inputTabName").unbind("keyup").keyup(function () {
                if ($(".inputTabName").val().length > 0)
                    $("#tabNameValidation").addClass("hiddenClass");
            })
            $(".toggleParentDiv").click(function (e) {
                if ($(e.target).hasClass("isAdvancedOff")) {
                    $(e.target).addClass("advanceSelected");
                    $(e.target).next().removeClass("advanceSelected");
                }
                else {
                    $(e.target).addClass("advanceSelected");
                    $(e.target).prev().removeClass("advanceSelected");
                }
            });
            $(".btnCancelTab").click(function () {
                $(".tabNamePopUp").slideUp("slow", function () {
                    $(".tabNamePopUp").remove();
                });
            });

            $(".btnAddTab").click(function (event) {
                if ($(".advanceSelected").hasClass("isAdvancedOn")) {
                    toolKit.IsAdvanced = true;
                }
                else {
                    toolKit.IsAdvanced = false;
                }
                if (!toolKit.RequireDataDictionary)
                    toolKit.IsAdvanced = true;
                toolKit.addTabInfo.clickEventHandler(event);
            });
            $(".inputTabName").focus();
        },

        clickEventHandler: function (event) {
            var self = this;
            if ($(event.target).hasClass("btnAddTab")) {
                if (!toolKit.utility.IsTabNameIsEmptyorDuplicated($(".inputTabName").val().trim())) {
                    var tabName = $(".inputTabName").val().trim();
                    //toolKit.IsAdvanced = $("#addMultiSelect").bootstrapSwitch('state');
                    //bootbox.hideAll();
                    $("#pageHeaderTabPanel").empty();
                    self.initTabs(tabName);
                    if ($(".iago-page-title").find(".toolKitAddTab").length == 0) {
                        if (!toolKit.ShowReport)
                            $(".iago-page-title").append("<div class=\"fa fa-plus-circle toolKitAddTab\" aria-hidden=\"true\"></div>");
                    }
                    if ($(".btnGlobalParameter").length == 0) {
                        $("#pageHeaderCustomDiv").append("<div class='btnGlobalParameterParent'><div class='btnGlobalParameter'>Parameter</div></div>");
                    }
                    if (toolKit.utility.showMappedTabs()) {
                        if ($(".btnAddTabRelation").length == 0) {
                            $("#pageHeaderCustomDiv").append("<div class='btnAddTabRelationParent'><div class='btnAddTabRelation'>Navigation</div></div>");
                        }
                    }
                    if ($("#pageHeaderCustomDiv").find(".btnReportConfig").length == 0 && !toolKit.ShowReport) {
                        $("#pageHeaderCustomDiv").append("<div class='btnReportConfigParent'><div class=\"btnReportConfig\" >All Reports</div></div>");
                        $(".btnReportConfig").unbind("click").click(function () {
                            toolKit.ShowReport = false;
                            toolKit.ExistingReports.CreateReportPopUp();
                        })
                    }
                }
                else {
                    $("#tabNameValidation").removeClass("hiddenClass");
                }
            }
            else if ($(event.target).hasClass("rptCatValue")) {
                self.getReportTypeDropDown(event);
            }
            else if ($(event.target).hasClass("rptDropwDownChild")) {
                $(event.target).closest(".rptTabTD").find(".rptCatValue").html($(event.target).html());
                $(event.target).parent().remove();
            }
        },

        getReportTypeDropDown: function (event) {
            var self = this;
            var div = $("<div>");
            div.addClass("rptDropDown");
            for (var i = 0; i < toolKit.TabReportType.length; i++) {
                div.append("<div class=\"rptDropwDownChild\">" + toolKit.TabReportType[i] + "</div>");
            }
            $(event.target).parent().append(div);
        },

        initTabs: function (tabName) {
            $(".tabNamePopUp").remove();
            if (toolKit.IsAdvanced)
                $("#" + toolKit.identifier).append("<div class=\"dataSourceHeader landingPage\"></div>");
            else
                $("#" + toolKit.identifier).append("<div class=\"dataSourceHeader\"></div>");
            //$(".dataSourceHeader").height(73);
            $("#" + toolKit.identifier).append("<div id=\"" + toolKit.tabContainer + "\"></div>");
            $("#" + toolKit.tabContainer).height($("#" + toolKit.identifier).height() - $(".dataSourceHeader").height());

            toolKit.newTabAdded = true;
            toolKit.reportInfo.TabsDetails = [];
            toolKit.reportInfo.TabsDetails.push({ tabId: 0, IsAdvanced: toolKit.IsAdvanced, TabName: tabName, TabOrder: toolKit.reportInfo.TabsDetails.length, IsTabSaved: false, ColumnInfo: [],
                DataSourceInfo: { TableOrViewName: "", ProcName: "", ParameterInfo: [] }
            });
            if (toolKit.IsAdvanced) {
                toolKit.reportInfo.TabsDetails[0].IsAdvanced = true;
            }
            else {
                toolKit.reportInfo.TabsDetails[0].IsAdvanced = false;
            }
            toolKit.CurrentTabInfo = toolKit.reportInfo.TabsDetails[toolKit.reportInfo.TabsDetails.length - 1];
            toolKit.CurrentTab = toolKit.CurrentTabInfo.TabName;
            if (toolKit.IsAdvanced) {
                toolKit.addDataSource.createDataSourcePopUp();
            }
            toolKit.bindTabs.bindTab(toolKit.reportInfo);

            toolKit.utility.getGlobalParameters();
        }
    }
});
$.extend(toolKit, {
    eventHandlers: {
        buttonBarHandler: function initialize(event) {
            if ($(event.target).closest("." + toolKit.addTabClass).length > 0) {
                toolKit.addWidget.addTab(event);
            }
            else if ($(event.target).closest("." + toolKit.saveDashBrdClass).length > 0) {
                var a = 1;
            }
            else if ($(event.target).closest("." + toolKit.addWdgtClass).length > 0) {
                toolKit.addWidget.createPopUp();
            }
            else if ($(event.target).closest("." + toolKit.selectDBClass).length > 0) {
                toolKit.addDataSource.createDataSourcePopUp();
            }
            else if ($(event.target).hasClass("procScrollDown")) {
                var scrollDownContent = scrollDownContent + 50;
                $(".ProcSourceBody").scrollTop(scrollDownContent);


            }
            else if ($(event.target).hasClass("procScrollUp")) {
                var scrollDownContent = scrollDownContent - 50;
                $(".ProcSourceBody").scrollTop(scrollDownContent);


            }
        },

        HeaderTextHandler: function (event) {
            if ($(event.target).hasClass("ProcedureText")) {
                toolKit.addTables.createTablesdropDown();
            }
            if ($(event.target).hasClass("QueryText")) {
                toolKit.addProcedure.createProcedureDropDown();
            }

        },
        TabsHeaderHandler: function (event) {
            if ($(event.target).hasClass("dataSourceItem")) {
                //alert($(event.target).text());
                if (!toolKit.CurrentTabInfo.IsTabSaved)
                    $(event.target).parent().append(toolKit.utility.getDbIDentiferDropDown());
            }
            else if ($(event.target).hasClass("dbIdentifierChild")) {
                $(".dataSourceItem").html($(event.target).html());
                toolKit.dbIdentifier = $(event.target).html();
                $(".dbIdentifierParent").remove();
                toolKit.CurrentTabInfo.DataSourceInfo.TableOrViewName = "";
                toolKit.tabContentContainer.empty();
                toolKit.addTables.createTablesdropDown();
            }
            else {
                $(".dbIdentifierParent").remove();
            }
        },
        ProcedureHandler: function (event) {
            if ($(event.target).hasClass("procScrollDown")) {
                toolKit.scrollDownContent = toolKit.scrollDownContent + 50;
                $(".ProcSourceBody").scrollTop(toolKit.scrollDownContent);


            }
            else if ($(event.target).hasClass("procScrollUp")) {
                toolKit.scrollDownContent = toolKit.scrollDownContent - 50;
                $(".ProcSourceBody").scrollTop(toolKit.scrollDownContent);


            }

        },

        TableHandlerKeyUp: function (event, viewModelProc, TablesArray) {
            var domCollection = $(".TableSourceBody").find(".tableSourceItem");
            var length = domCollection.length;
            for (var i = 0; i < length; i++) {
                if ($(domCollection[i]).html().toLowerCase().indexOf($(event.target).val().toLowerCase()) > -1) {
                    $(domCollection[i]).removeClass("formatHidden");
                }
                else {
                    $(domCollection[i]).addClass("formatHidden");
                }
            }
        },
        //        TableHandlerKeyUp: function (event, viewModelProc, TablesArray) {
        //            for (var item in TablesArray) {
        //                if (TablesArray[item].toLowerCase().indexOf($(event.target).val().toLowerCase()) == -1) {
        //                    var index = toolKit.eventHandlers.getIndex(viewModelProc, TablesArray[item])
        //                    if (index > -1) {
        //                        viewModelProc.TablesSource().splice(index, 1);
        //                    }
        //                }
        //                else {
        //                    var index = toolKit.eventHandlers.getIndex(viewModelProc, TablesArray[item])
        //                    if (index == -1) {
        //                        viewModelProc.TablesSource().splice(item, 0, TablesArray[item]);
        //                    }
        //                }
        //            }
        //            //  toolKit.addTables.bindTables(TableArrayForSearch);
        //        },

        getIndex: function (viewModelProc, TablesArray) {
            var self = this;
            var index = -1;
            for (var i = 0; i < viewModelProc.TablesSource().length; i++) {
                index = viewModelProc.TablesSource()[i].tableName.indexOf(TablesArray);
                if (index > -1)
                    return index;
            }
            return index;
        },

        ProcedureHandlerKeyUp: function (event, viewModelProc) {
            if ($(event.target).hasClass("ProcedureSearchTextBox")) {
                //$(".ProcSourceBody").empty();
                var ProceduresArraySearch = toolKit.DataSourceCollection;
                for (var item in ProceduresArraySearch) {
                    if (ProceduresArraySearch[item].toLowerCase().indexOf($(event.target).val().toLowerCase()) == -1) {
                        //ProceduresArraySearch.push(toolKit.DataSourceCollection[item]);
                        if (viewModelProc.ProcedureSource.indexOf(ProceduresArraySearch[item]) > -1) {
                            var index = viewModelProc.ProcedureSource.indexOf(ProceduresArraySearch[item]);
                            viewModelProc.ProcedureSource.splice(index, 1);
                        }
                    }
                    else {
                        if (viewModelProc.ProcedureSource.indexOf(ProceduresArraySearch[item]) == -1) {
                            viewModelProc.ProcedureSource.splice(item, 0, ProceduresArraySearch[item]);
                            // ProceduresArraySearch.splice(item, 1);
                        }
                    }
                }
                //toolKit.addProcedure.test = ["Test11", "Test12", "Test13"];
                //  toolKit.addProcedure.bindProcedures();
                //                var viewModelProc = toolKit.ProcedureViewModel;
                //                viewModelProc.ProcedureSource = ProceduresArraySearch;
                //                ko.cleanNode($(".addProcSourcePopup")[0]);
                //                ko.applyBindings(viewModelProc, $(".addProcSourcePopup")[0]);


            }
        }
    }
});
$.extend(toolKit, {
    addWidget: {
        createPopUp: function () {
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetHtmlTemplate',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.addWidget.htm' })
            }).then(function (responseText) {
                $("#" + toolKit.identifier).append(responseText.d);
            });
        },
        addTab: function (event) {
            $(".tabNamePopUp").remove();
            var div = $('<div>');
            div.addClass("tabNamePopUp");
            div.append("<div class=\"tabNameLabelParent\"><div class='tabNameLabelName'>Tab Name</div><div class='tabNameInput'><input class='inputTabName'/></div><div id=\"tabNameValidation\" title = \"Tab Name Already Exists\" class=\"tabNameValidation hiddenClass\" >!</div></div>");
            if (toolKit.RequireDataDictionary)
                div.append("<div class=\"tabNameLabelParent\"><div class='tabNameLabel'>Use Dictionary</div><div class='toggleParentDiv'><div class=\"isAdvancedOff advanceSelected\">ON</div> <div class=\"isAdvancedOn\">OFF</div></div></div>");
            //div.append("<div class=\"tabNameLabelParent\"><label for=\"lblDiction\">Dictionary</label><input type=\"radio\" id=\"lblDiction\" name=\"gender\" /><label for=\"lblAdvance\">Advance</label><input type=\"radio\" id=\"lblAdvance\" name=\"gender\" /></div>");
            div.append("<div class=\"parentbtnAddTab\"><div class='btnAddTab'>Add Tab</div><div class=\"btnCancelTab\">Cancel</div></div>");
            $("#" + toolKit.identifier).append(div);
            div.css({ left: (($("#pageHeaderTabPanel").width() - 20) + 'px') });
            toolKit.IsAdvanced = true;
            $(".inputAdvanced").bootstrapToggle({ 'size': 'mini' });
            toolKit.newTabAdded = true;

            $(".inputTabName").unbind("keyup").keyup(function () {
                if ($(".inputTabName").val().length > 0)
                    $("#tabNameValidation").addClass("hiddenClass");
            })
            $(".toggleParentDiv").click(function (e) {
                if ($(e.target).hasClass("isAdvancedOff")) {
                    $(e.target).addClass("advanceSelected");
                    $(e.target).next().removeClass("advanceSelected");
                }
                else {
                    $(e.target).addClass("advanceSelected");
                    $(e.target).prev().removeClass("advanceSelected");
                }
            });
            $(".btnCancelTab").click(function () {
                $(".tabNamePopUp").slideUp("slow", function () {
                    $(".tabNamePopUp").remove();
                });
            });
            $(".inputTabName").focus();
            $(".btnAddTab").click(function () {
                if (!toolKit.utility.IsTabNameIsEmptyorDuplicated($(".inputTabName").val().trim())) {
                    $(".dataSourceHeader").css({ 'height': '' });
                    $("#pageHeaderCustomDiv").empty();
                   
                    if ($(".btnGlobalParameter").length == 0) {
                        $("#pageHeaderCustomDiv").append("<div class='btnGlobalParameterParent'><div class='btnGlobalParameter'>Parameter</div></div>");
                    }
                    if (toolKit.utility.showMappedTabs()) {
                        if ($(".btnAddTabRelation").length == 0) {
                            $("#pageHeaderCustomDiv").append("<div class='btnAddTabRelationParent'><div class='btnAddTabRelation'>Navigation</div></div>");
                        }
                    }
                    if ($("#pageHeaderCustomDiv").find(".btnReportConfig").length == 0 && !toolKit.ShowReport) {
                        $("#pageHeaderCustomDiv").append("<div class='btnReportConfigParent'><div class=\"btnReportConfig\" >All Reports</div></div>");
                        $(".btnReportConfig").unbind("click").click(function () {
                            toolKit.ShowReport = false;
                            toolKit.ExistingReports.CreateReportPopUp();
                        })
                    }
                    var tabName = $(".inputTabName").val().trim();
                    var tabId = toolKit.reportInfo.TabsDetails.length;
                    toolKit.reportInfo.TabsDetails.push({});
                    toolKit.reportInfo.TabsDetails[tabId].tabId = 0;
                    toolKit.reportInfo.TabsDetails[tabId].TabName = tabName;
                    if ($(".advanceSelected").hasClass("isAdvancedOn")) {
                        toolKit.reportInfo.TabsDetails[tabId].IsAdvanced = true;
                    }
                    else {
                        toolKit.reportInfo.TabsDetails[tabId].IsAdvanced = false;
                    }
                    if (!toolKit.RequireDataDictionary)
                        toolKit.reportInfo.TabsDetails[tabId].IsAdvanced = true;
                    toolKit.reportInfo.TabsDetails[tabId].TabOrder = tabId;
                    toolKit.reportInfo.TabsDetails[tabId].DataSourceInfo = {};
                    toolKit.reportInfo.TabsDetails[tabId].ColumnInfo = [];
                    toolKit.CurrentTabInfo = toolKit.reportInfo.TabsDetails[toolKit.reportInfo.TabsDetails.length - 1];
                    if (!toolKit.CurrentTabInfo.IsAdvanced) {
                        $(".dataSourceHeader").removeClass("landingPage");
                    }
                    $(".tabNamePopUp").remove();
                    $("#pageHeaderTabPanel").toolKittabs('addNewTab', {
                        id: tabId,
                        name: tabName
                    });
                }
                else {
                    $("#tabNameValidation").removeClass("hiddenClass");
                }
            })
        }
    }

});
$.extend(toolKit, {
    addDataSource: {
        createDataSourcePopUp: function () {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetHtmlTemplate',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.addDataSource.htm' })
            }).then(function (responseText) {
                if ($(".dataSourceHeader").length == 0) {
                    $("#" + toolKit.identifier).prepend("<div class=\"dataSourceHeader\"></div>");
                }
                $(".dataSourceHeader").css({ 'height': '' });
                $(".dataSourceHeader").empty();
                $(".dataSourceHeader").append(responseText.d);
                var dataSource = ["DataBase 01", "DataBase 02", "DataBase 03", "DataBase 04"];
                self.bindDataSource(dataSource);

                //   toolKit.addTables.createTablesdropDown();


                //toolKit.handlers.buttonBarClickHandler();

            });
        },

        bindDataSource: function (dataSourceArray) {
            function DataSourceViewModel() {
                var self = this;
                self.dataSource = ko.observableArray();
                self.ClickOnDataBaseTabs = function (event) {
                    self.ClickOnDBTabs(event);
                }
            }
            var self = this;
            var viewModel = new DataSourceViewModel();
            viewModel.dataSource.push(toolKit.dbIdentifier.toUpperCase());
            ko.applyBindings(viewModel, $(".addDataSourcePopup")[0]);
            self.TabsHeaderHandler();
        },

        TabsHeaderHandler: function () {
            $(".DataSourceBody").unbind("click").click(function (event) {
                toolKit.eventHandlers.TabsHeaderHandler(event);
            });
        }
    }
});
    
    






    
$.extend(toolKit, {
    globalFilter: {

        createPopUp: function (globalFilterCollection) {
            var self = this;
            //self.defaultValuesList = ["Custom","Today",
            //    "Yesterday", "LastDay", "T - N", "CustomDate", "Now",
            //    "FirstDayOfMonth", "FirstDayOfYear", "LastDayOfMonth",
            //    "LastDayOfYear", "LastDayOfPreviousMonth + N",
            //    "LastDayOfPreviousYear + N", "FirstDayOfMonth + N",
            //    "FirstDayOfYear + N", "LastDayOfLastQuarter",
            //    "FirstDayOfCurrentQuarter", "LastDayOfLastQuarter + N",
            //    "FirstDayOfCurrentQuarter + N", "LastDayOfPreviousWeek + N",
            //    "FirstDayOfWeek + N", "LastDayOfPreviousWeek",
            //    "FirstDayOfWeek", "T + N", "LastDayOfQuarter",
            //];
            self.defaultValuesList = ["Custom", "Today",
                "Yesterday", "CustomDate", "Now",
                "FirstDayOfMonth", "FirstDayOfYear", "LastDayOfMonth",
                "LastDayOfYear", "LastDayOfLastQuarter",
                "FirstDayOfCurrentQuarter", "LastDayOfPreviousWeek",
                "FirstDayOfWeek", "LastDayOfQuarter",
            ];

            self.defaultNList = ["T - N", "LastDayOfPreviousMonth + N", "LastDayOfPreviousYear + N", "FirstDayOfMonth + N", "FirstDayOfYear + N",
                "LastDayOfLastQuarter + N", "FirstDayOfCurrentQuarter + N", "LastDayOfPreviousWeek + N", "FirstDayOfWeek + N", "T + N"]
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetHtmlTemplate',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.globalFilter.htm' })
            }).then(function (responseText) {
                self.right = ($("#pageHeaderCustomDiv").width() - $(".btnGlobalParameterParent").width()) + 'px';
                self.globalFilterHtml = responseText.d;
                //                $("#" + toolKit.identifier).append(responseText.d);
                //                $(".globalFilterPopUp").css({ 'right': right });
                self.editGlobalFilterMode = false;
                //globalFilterCollection = [];
                self.getTables()
            });
        },

        getTables: function () {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetTablesFromDatabase',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ databaseName: toolKit.DBName, dbIdentifier: toolKit.dbIdentifier })
            }).then(function (responseText) {
                toolKit.TablesCollection = JSON.parse(responseText.d);
                //  self.bindProcedures(["Test1", "Test2", "Test3", "Test4"]);
                toolKit.globalFilter.bindGLobalFilter();
                toolKit.globalFilter.eventHandlers();
                //                var right = ($("#pageHeaderCustomDiv").width() - $(".btnGlobalParameterParent").width()) + 'px';
                //                $(".globalFilterPopUp").css({ 'right': right });
            });
        },

        eventHandlers: function () {
            var self = this;
            $("#globalFilterPopUp").click(function (event) {
                if (event.target.id == "addSourceTable") {
                    $(".columncollection").remove();
                    var div = $('<div>');
                    div.addClass("tablecollection");
                    toolKit.TablesCollection.sort();
                    for (var i = 0; i < toolKit.TablesCollection.length; i++) {
                        div.append("<div class=\"tableCollectionChild\" title=\"" + toolKit.TablesCollection[i] + "\" >" + toolKit.TablesCollection[i] + "</div>")
                    }
                    $("#addGlobalFilter").append(div);
                    div.css({ 'left': '175px' });
                    div.css({ 'top': '80px' });
                }
                else if (event.target.className == "tableCollectionChild") {
                    self.tableName = $(event.target).html();
                    $("#addSourceTable").val($(event.target).html());
                    $(".tablecollection").remove();
                }
                else if (event.target.id == "addSourceColumn") {
                    if (self.tableName == null) {
                        self.tableName = $(event.target).closest('tr').prev().find(".toolKitTextBox").val().trim();
                    }
                    if (self.tableName != null)
                        self.getColumnNameDropDown(self.tableName);
                }
                else if (event.target.className == "columnCollectionChild") {
                    $("#addSourceColumn").val($(event.target).html());
                    $(".columncollection").remove();
                }
                else if (event.target.className == "btnSaveGlobalFilter") {
                    $(".tablecollection").remove();
                    $(".columncollection").remove();
                    self.saveGlobalFilter();
                    //                    $("#addGlobalFilter").removeClass("addGlobalFilter");
                    //                    $("#addGlobalFilter").addClass("addGlobalFilterHidden");
                }
                else if (event.target.className == "btnCancelGlobalFilter") {
                    //self.saveGlobalFilter();
                    $(".tablecollection").remove();
                    $(".columncollection").remove();
                    $(".addGlobalFilter").css({ 'display': 'none' });
                    $(".btnAddGlobalFilter").removeClass("btnAddGlobalFilterHidden");
                }
                //                else if ($(event.target).hasClass("editGlobalFilter")) {
                //                    self.editGlobalFIlter(event);
                //                }
                //                else if ($(event.target).hasClass("deleteGlobalFilter")) {
                //                    self.deleteGlobalFIlter(event);
                //                }
                else if ($(event.target).hasClass("btnAddGlobalFilter")) {
                    toolKit.globalFilter.addNewGlobalFIlter(event);
                }
                else if ($(event.target).hasClass("defaultValueOption") || $(event.target).hasClass("defaultValueRangeOption")) {
                    var div = $('<div>');
                    div.addClass("defaultValueCollection");
                    for (var i = 0; i < self.defaultValuesList.length; i++) {
                        div.append("<div class=\"defaultValueChild\" title = \"" + self.defaultValuesList[i] + "\">" + self.defaultValuesList[i] + "</div>")
                    }
                    $("#addGlobalFilter").append(div);
                    div.css({ 'left': '107px' });
                    if ($(event.target).hasClass("defaultValueRangeOption")) {
                        div.css({ 'top': '175px' });
                        div.attr("forRangeValue", "true");
                        div.children().first().remove();
                    }
                    else
                        div.css({ 'top': '142px' });
                }
                else if ($(event.target).hasClass("defaultValueRangeButton")) {
                    $(".NGenRangeSelect").removeClass("hiddenClass");
                }
                else if ($(event.target).hasClass("defaultValueChild") && $(".defaultValueCollection").attr("forRangeValue") != null) {
                    $("#txtRangeDefaultValue").val($(event.target).html());
                    $(".ngeneralRangeText").removeClass("hiddenClass");
                    $(".defaultValueCollection").remove();
                }
                else if ($(event.target).hasClass("defaultValueChild")) {

                    if ($(event.target).html() == "Custom") {
                        $("#txtDefaultValue").removeClass("nonEditableTextBox");
                        $("#txtDefaultValue").val("");
                        $("#txtRangeDefaultValue").val("");
                        $(".addDefaultValue").removeAttr("NGeneral")
                        $(".ngeneralText").addClass("hiddenClass");
                        $(".NGenRangeSelect").addClass("hiddenClass");
                        $(".defaultValueRangeButton").addClass("hiddenClass");
                    }
                    else {
                        $("#txtDefaultValue").addClass("nonEditableTextBox");
                        $("#txtDefaultValue").val($(event.target).html());
                        $(".addDefaultValue").attr("NGeneral", "true");
                        $(".ngeneralText").removeClass("hiddenClass");
                       
                        $(".defaultValueRangeButton").removeClass("hiddenClass");
                    }
                    //if (self.defaultNList.indexOf($(event.target).html()) > -1) {
                    //    $(".addDefaultValue").attr("NGeneral", "true");
                    //    $(".ngeneralText").removeClass("hiddenClass");
                    //}
                    //else {
                    //    $(".addDefaultValue").removeAttr("NGeneral")
                    //    $(".ngeneralText").addClass("hiddenClass");
                    //}
                    $(".defaultValueCollection").remove();
                }
                else {
                    $(".defaultValueCollection").remove();
                    $(".tablecollection").remove();
                    $(".columncollection").remove();

                }
            });
        },

        IsParamNameEmptyorDuplicated: function (reportName) {
            var self = this;
            if (reportName == "" || reportName == null) {
                return true;
            }
            else {
                if (toolKit.reportInfo.GlobalFilters != null) {
                    for (var i = 0; i < toolKit.reportInfo.GlobalFilters.length; i++) {
                        if (toolKit.reportInfo.GlobalFilters[i].GlobalParameterName.trim().toLowerCase() == reportName.trim().toLowerCase()) {
                            return true;
                        }
                    }
                }
                else
                    return false;
            }
            return false;
        },

        saveGlobalFilter: function () {
            var self = this;

            var parameterInfo = {
                GlobalParameterName: $("#addFilterName").val(),
                DataSourceTable: $("#addSourceTable").val(),
                DataSourceColumn: $("#addSourceColumn").val(),
                IsMultiSelect: ($('#addMultiSelect').bootstrapSwitch('state') == true ? true : false),
                //DefaultValue: $("#txtDefaultValue").val(),
                IsRightColumn: ($('#addRightFilter').bootstrapSwitch('state') == true ? true : false),
                PossibleValues: $("#txtPossibleValues").val()
            }
            if ($(".addDefaultValue").attr("NGeneral") != null) {
                parameterInfo.DefaultValue = $("#txtDefaultValue").val() + "^^" + $(".ngeneralText").html() + "," + $("#txtRangeDefaultValue").val() + "^^" + $(".ngeneralRangeText").html();
            }
            else
                parameterInfo.DefaultValue = $("#txtDefaultValue").val();
            var index = 0;
            if (toolKit.globalFilter.editGlobalFilterMode) {
                var element = $(".globalFilterPopUp").find('div[globalfiltername="' + parameterInfo.GlobalParameterName + '"]');
                index = element.index();
                element.find(".filterSourceTable").html(parameterInfo.DataSourceTable);
                element.find(".filterSourceColumn").html(parameterInfo.DataSourceColumn);
                element.find("#lblDefaultValue").html(parameterInfo.DefaultValue);
                element.find("#lblPossibleValue").html(parameterInfo.PossibleValues);
                if (parameterInfo.IsMultiSelect) {
                    element.find("#isMultiSelectSwitch").bootstrapToggle({ 'size': 'mini' });
                    element.find("#isMultiSelectSwitch").bootstrapToggle('on');
                    element.find("#isMultiSelectSwitch").bootstrapToggle('disable');
                }
                else {
                    element.find("#isMultiSelectSwitch").bootstrapToggle({ 'size': 'mini' });
                    element.find("#isMultiSelectSwitch").bootstrapToggle('off');
                    element.find("#isMultiSelectSwitch").bootstrapToggle('disable');
                }
                if (parameterInfo.IsRightColumn) {
                    element.find("#isRightFilterColumnSwitch").bootstrapToggle({ 'size': 'mini' });
                    element.find("#isRightFilterColumnSwitch").bootstrapToggle('on');
                    element.find("#isRightFilterColumnSwitch").bootstrapToggle('disable');
                }
                else {
                    element.find("#isRightFilterColumnSwitch").bootstrapToggle({ 'size': 'mini' });
                    element.find("#isRightFilterColumnSwitch").bootstrapToggle('off');
                    element.find("#isRightFilterColumnSwitch").bootstrapToggle('disable');
                }
                toolKit.reportInfo.GlobalFilters[index] = parameterInfo;
            }
            else {
                if (self.IsParamNameEmptyorDuplicated($("#addFilterName").val())) {
                    $(".paramNameValidation").removeClass("hiddenClass");
                    return false;
                }
                toolKit.reportInfo.GlobalFilters.push({
                    "GlobalParameterName": $("#addFilterName").val(), "DataSourceTable": $("#addSourceTable").val(), "DataSourceColumn": $("#addSourceColumn").val(),
                    "isMultiSelect": ($('#addMultiSelect').bootstrapSwitch('state') == true ? true : false), "DefaultValue": $("#txtDefaultValue").val(),
                    "IsRightColumn": ($('#addRightFilter').bootstrapSwitch('state') == true ? true : false), "PossibleValues": $("#txtPossibleValues").val()
                });
                if ($('#addMultiSelect').bootstrapSwitch('state')) {
                    $(".globalFIlter").last().find("#isMultiSelectSwitch").bootstrapToggle({ 'size': 'mini' });
                    $(".globalFIlter").last().find("#isMultiSelectSwitch").bootstrapToggle('on');
                    $(".globalFIlter").last().find("#isMultiSelectSwitch").bootstrapToggle('disable');
                }
                else {
                    $(".globalFIlter").last().find("#isMultiSelectSwitch").bootstrapToggle({ 'size': 'mini' });
                    $(".globalFIlter").last().find("#isMultiSelectSwitch").bootstrapToggle('off');
                    $(".globalFIlter").last().find("#isMultiSelectSwitch").bootstrapToggle('disable');
                }
                if ($('#isRightFilterColumnSwitch').bootstrapSwitch('state')) {
                    $(".globalFIlter").last().find("#isRightFilterColumnSwitch").bootstrapToggle({ 'size': 'mini' });
                    $(".globalFIlter").last().find("#isRightFilterColumnSwitch").bootstrapToggle('on');
                    $(".globalFIlter").last().find("#isRightFilterColumnSwitch").bootstrapToggle('disable');
                }
                else {
                    $(".globalFIlter").last().find("#isRightFilterColumnSwitch").bootstrapToggle({ 'size': 'mini' });
                    $(".globalFIlter").last().find("#isRightFilterColumnSwitch").bootstrapToggle('off');
                    $(".globalFIlter").last().find("#isRightFilterColumnSwitch").bootstrapToggle('disable');
                }
            }
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + "/CreateGlobalParameter",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ parameterInfo: JSON.stringify(parameterInfo), user: iago.user.userName, identifier: toolKit.reportInfo.reportIdentifier })
            }).then(function (responseText) {
                $(".btnAddGlobalFilter").removeClass("btnAddGlobalFilterHidden");
                //toolKit.reportInfo.GlobalFilters.push(parameterInfo);
                $("#addGlobalFilter").hide('blind', 1000);
                $("#addGlobalFilter").addClass("addGlobalFilterHidden").removeClass("addGlobalFilter");
                $("#addFilterName").val("");
                $("#addSourceTable").val("");
                $("#addSourceColumn").val("");
                $("#isMultiSelect").empty();
                $("#isMultiSelect").append("<input id=\"addMultiSelect\" type=\"checkbox\" class=\"toolKitTextBox\" /></div>");

                if (self.editGlobalFilterMode) {
                    self.viewModel.filters()[index].filterName(parameterInfo.GlobalParameterName),
                    self.viewModel.filters()[index].sourceTable(parameterInfo.DataSourceTable),
                    self.viewModel.filters()[index].sourceColumn(parameterInfo.DataSourceColumn),
                    self.viewModel.filters()[index].isMultiSelect(parameterInfo.IsMultiSelect),
                    //self.viewModel.filters()[index].defaultValue(parameterInfo.DefaultValue),
                    self.viewModel.filters()[index].defaultValue(self.getDefaultValue(parameterInfo.DefaultValue)),
                    self.viewModel.filters()[index].defaultRangeValue(self.getdefaultRangeValue(parameterInfo.DefaultValue)),
                    self.viewModel.filters()[index].possibleValues(parameterInfo.PossibleValues),
                    self.viewModel.filters()[index].isRightFilterColumn(parameterInfo.IsRightColumn)
                }
                else {
                    self.viewModel.filters.push({ filterName: ko.observable(parameterInfo.GlobalParameterName),
                        sourceTable: ko.observable(parameterInfo.DataSourceTable),
                        sourceColumn: ko.observable(parameterInfo.DataSourceColumn),
                        isMultiSelect: ko.observable(parameterInfo.IsMultiSelect),
                        defaultValue: ko.observable(parameterInfo.DefaultValue),
                        defaultRangeValue: ko.observable(parameterInfo.DefaultValue),
                        possibleValues: ko.observable(parameterInfo.PossibleValues),
                        isRightFilterColumn: ko.observable(parameterInfo.IsRightColumn)
                    });

                    index = $(".globalFIlter").length - 1;

                }
                var domCollection = $(".globalFIlter");
                $(domCollection[index]).find("#isMultiSelectSwitch").bootstrapToggle({ 'size': 'mini' });
                $(domCollection[index]).find("#isRightFilterColumnSwitch").bootstrapToggle({ 'size': 'mini' });
                $(domCollection[index]).find("#isMultiSelectSwitch").bootstrapToggle('enable');
                $(domCollection[index]).find("#isRightFilterColumnSwitch").bootstrapToggle('enable');
                if (toolKit.reportInfo.GlobalFilters[index].IsMultiSelect) {
                    $(domCollection[index]).find("#isMultiSelectSwitch").bootstrapToggle('on');
                }
                else {
                    $(domCollection[index]).find("#isMultiSelectSwitch").bootstrapToggle('off');
                }
                if (toolKit.reportInfo.GlobalFilters[index].IsRightColumn) {
                    $(domCollection[index]).find("#isRightFilterColumnSwitch").bootstrapToggle('on');
                }
                else {
                    $(domCollection[index]).find("#isRightFilterColumnSwitch").bootstrapToggle('off');
                }

                $(domCollection[index]).find("#isMultiSelectSwitch").bootstrapToggle('disable');
                $(domCollection[index]).find("#isRightFilterColumnSwitch").bootstrapToggle('disable');
                self.editGlobalFilterMode = false
            })
        },

        getColumnNameDropDown: function (tableName) {
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + "/GetColumnOfTableFromDatabase",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ databaseName: toolKit.DBName, dbIdentifier: toolKit.dbIdentifier, tableName: tableName })
            }).then(function (responseText) {
                var colCollection = $.parseJSON(responseText.d);
                var div = $('<div>');
                div.addClass("columncollection");
                for (var i = 0; i < colCollection.length; i++) {
                    div.append("<div class=\"columnCollectionChild\" title=\"" + colCollection[i].ColumnName + "\" >" + colCollection[i].ColumnName + "</div>")
                }
                $("#addGlobalFilter").append(div);
                div.css({ 'left': '175px' });
                div.css({ 'top': '110px' });
            });
        },

        bindGLobalFilter: function () {

            //            var globalFilterCollection = [];
            //            globalFilterCollection.push({ "filterName": "Test", "sourceTable": "Test", "sourceColumn": "Test", "isMultiSelect": true });
            //            globalFilterCollection.push({ "filterName": "Test1", "sourceTable": "Test", "sourceColumn": "Test", "isMultiSelect": true });
            var closure = this;
            function GlobalFilterViewModel() {
                var self = this;
                self.filters = ko.observableArray();
                self.addGlobalFilter = function (model, event) {
                    toolKit.globalFilter.addNewGlobalFIlter(event);
                }
                self.editGlobalFilter = function (model, event) {
                    toolKit.globalFilter.editGlobalFilterMode = true;
                    toolKit.globalFilter.editGlobalFIlter(model, event);
                }
                self.deleteGlobalFilter = function (model, event) {
                    toolKit.globalFilter.editGlobalFilterMode = false;
                    toolKit.globalFilter.deleteGlobalFIlter(model, event);
                }
            }

            this.viewModel = new GlobalFilterViewModel();
            for (var i = 0; i < toolKit.reportInfo.GlobalFilters.length; i++) {
                toolKit.reportInfo.GlobalFilters[i]
                this.viewModel.filters.push({
                    filterName: ko.observable(toolKit.reportInfo.GlobalFilters[i].GlobalParameterName),
                    sourceTable: ko.observable(toolKit.reportInfo.GlobalFilters[i].DataSourceTable),
                    sourceColumn: ko.observable(toolKit.reportInfo.GlobalFilters[i].DataSourceColumn),
                    isMultiSelect: ko.observable(toolKit.reportInfo.GlobalFilters[i].IsMultiSelect),
                    defaultValue: ko.observable(closure.getDefaultValue(toolKit.reportInfo.GlobalFilters[i].DefaultValue)),
                    defaultRangeValue: ko.observable(closure.getdefaultRangeValue(toolKit.reportInfo.GlobalFilters[i].DefaultValue)),
                    possibleValues: ko.observable(toolKit.reportInfo.GlobalFilters[i].PossibleValues),
                    isRightFilterColumn: ko.observable(toolKit.reportInfo.GlobalFilters[i].IsRightColumn)
                });
            }
            if (toolKit.reportInfo.GlobalFilters.length == 0) {
                $("#addGlobalFilter").addClass("addGlobalFilter").removeClass("addGlobalFilterHidden");
                $(".addGlobalFilterHidden").find("#addRightFilter").bootstrapToggle({ 'size': 'mini' });
                $(".addGlobalFilterHidden").find("#addMultiSelect").bootstrapToggle({ 'size': 'mini' });
            }
            $("#" + toolKit.identifier).append(this.globalFilterHtml);
            ko.applyBindings(this.viewModel, $(".globalFilters")[0]);
            $(".globalFilterPopUp").css({ 'right': this.right });
            var domCollection = $(".globalFIlter");
            var length = $(".globalFIlter").length;
            for (var i = 0; i < length; i++) {
                $(domCollection[i]).find("#isMultiSelectSwitch").bootstrapToggle({ 'size': 'mini' });
                $(domCollection[i]).find("#isMultiSelectSwitch").bootstrapToggle('disable');
                $(domCollection[i]).find("#isRightFilterColumnSwitch").bootstrapToggle({ 'size': 'mini' });
                $(domCollection[i]).find("#isRightFilterColumnSwitch").bootstrapToggle('disable');
                if (toolKit.reportInfo.GlobalFilters[i].IsMultiSelect) {
                    $(domCollection[i]).find("#isMultiSelectSwitch").bootstrapToggle('on');
                }
                else {
                    $(domCollection[i]).find("#isMultiSelectSwitch").bootstrapToggle('off');
                }
                if (toolKit.reportInfo.GlobalFilters[i].IsRightColumn) {
                    $(domCollection[i]).find("#isRightFilterColumnSwitch").bootstrapToggle('on');
                }
                else {
                    $(domCollection[i]).find("#isRightFilterColumnSwitch").bootstrapToggle('off');
                }
                if (toolKit.reportInfo.GlobalFilters[i].DefaultValue.indexOf("^^") > -1) {
                    $(domCollection[i]).find(".defaultRangeValueParent").removeClass("hiddenClass");
                    $(domCollection[i]).attr("defaultValue", toolKit.reportInfo.GlobalFilters[i].DefaultValue);
                }
            }
        },

        getDefaultValue: function (defaultValue) {
            if (defaultValue.indexOf("^^") > -1) {
                return "<div class='defaultTextClass'>" + defaultValue.substring(0, defaultValue.indexOf("^^")) + "</div><div class='defaultNGenClass'>" + defaultValue.substring(defaultValue.indexOf("^^") + 2, defaultValue.indexOf(",")) + "</div>";
            }
            return defaultValue;
        },

        getdefaultRangeValue: function (defaultValue) {
            if (defaultValue.indexOf("^^") > -1) {
                return "<div class='defaultTextClass'>" + defaultValue.substring(defaultValue.indexOf(",") + 1, defaultValue.lastIndexOf("^^")) + "</div><div class='defaultNGenClass'>" + defaultValue.substr(defaultValue.lastIndexOf("^^") + 2) + "</div>";
            }
            return "";
        },

        editGlobalFIlter: function (model, event) {
            var self = this;
            self.addNewGlobalFIlter();
            $("#addFilterName").val(model.filterName());
            $("#addSourceTable").val(model.sourceTable());
            $("#addSourceColumn").val(model.sourceColumn());
            $(".toolKitTextBox").addClass("nonEditableTextBox");
            //$("#txtDefaultValue").val(model.defaultValue());
            if ($(event.target).closest(".globalFIlter").attr("defaultValue") != null) {
                
                var defaultValue = $(event.target).closest(".globalFIlter").attr("defaultValue");
                $(".addDefaultValue").attr("NGeneral", "true");
                $("#txtDefaultValue").val(defaultValue.substring(0, defaultValue.indexOf("^^")));
                $("#txtDefaultValue").addClass("nonEditableTextBox");
                $(".ngeneralText").removeClass("hiddenClass");
                $(".ngeneralText").html(defaultValue.substring(defaultValue.indexOf("^^") + 2, defaultValue.indexOf(",")));
                $(".NGenRangeSelect").removeClass("hiddenClass");
                $(".defaultValueRangeButton").removeClass("hiddenClass");
                $("#txtRangeDefaultValue").val(defaultValue.substring(defaultValue.indexOf(",") + 1, defaultValue.lastIndexOf("^^")));
                $(".ngeneralRangeText").html(defaultValue.substr(defaultValue.lastIndexOf("^^") + 2));
            }
            else {
                $("#txtDefaultValue").val(model.defaultValue());
                $(".NGenRangeSelect").addClass("hiddenClass");
                $(".addDefaultValue").removeAttr("NGeneral");
                $(".ngeneralText").addClass("hiddenClass");
            }
          
            $("#txtPossibleValues").val(model.possibleValues());
            $("#txtPossibleValues").val(model.possibleValues());
            if (model.isMultiSelect())
                $("#addGlobalFilter").find("#addMultiSelect").bootstrapToggle('on');
            if (model.isRightFilterColumn())
                $("#addGlobalFilter").find("#addRightFilter").bootstrapToggle('on');
        },

        deleteGlobalFIlter: function (model, event) {
            var self = this;
            var index = 0;
            for (var i = 0; i < toolKit.reportInfo.GlobalFilters.length; i++) {
                if (toolKit.reportInfo.GlobalFilters[i].GlobalParameterName == model.filterName) {
                    index = i;
                    break;
                }
            }
            toolKit.reportInfo.GlobalFilters.splice(index, 1);
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + "/DeleteGloablFilter",
                type: "POST",
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify({ parameterName: model.filterName(), identifier: toolKit.reportInfo.reportIdentifier, user: iago.user.userName })
            }).then(function () {
                $(event.target).closest('.globalFIlter').remove();
            });
        },

        addNewGlobalFIlter: function (event) {
            $("#addGlobalFilter").show("blind", 1000);
            $(".addFilterValueTest").find(".toolKitTextBox").bootstrapToggle({ 'size': 'mini' });
            $("#addGlobalFilter").removeClass("addGlobalFilterHidden");
            $("#addGlobalFilter").addClass("addGlobalFilter");
            $(".btnAddGlobalFilter").addClass("btnAddGlobalFilterHidden");
            $(".addFilterName").val("");
            $(".NGenRangeSelect").addClass("hiddenClass");
            $("#txtRangeDefaultValue").val("");
            $("#txtDefaultValue").val("");
            $("#txtDefaultValue").removeClass("nonEditableTextBox");
            $(".ngeneralRangeText").html(1);
            $(".ngeneralText").addClass("hiddenClass");
            $(".ngeneralText").html(1);
            $(".defaultValueRangeButton").addClass("hiddenClass");
            $("#txtPossibleValues").val("");
            $(".toolKitTextBox").val("");
            $(".paramNameValidation").addClass("hiddenClass");
            $(".toolKitTextBox").removeClass("nonEditableTextBox");
        }
    }
});
$.extend(toolKit, {
    addColumns: {
        CreatePopUp: function (tableName) {
            var self = this;
            self.tableName = tableName;
            self.doChange = true;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + "/GetHtmlTemplate",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.addColumns.htm' })
            }).then(function (responseText) {
                self.appendTableName(toolKit.addColumns.tableName);

                var spinnerValue = $(".inputSectionSpinner").spinner({ min: 0, change: function (event, ui) { self.changeSpinner(event); } })
                toolKit.tabContentContainer.empty();
                self.addColumnsHTML = toolKit.addColumns.tableName
                toolKit.tabContentContainer.append(responseText.d);
                //toolKit.utility.applytaggingPrivilege();
                if (toolKit.RequireTagLink)
                    $(".btnAddLinkColumn").addClass("hiddenClass");
                toolKit.tabContentContainer.css({ 'height': '100%' });
                if (toolKit.CurrentTabInfo.DataSourceInfo.TableOrViewName == self.tableName) {
                    if (toolKit.CurrentTabInfo.ColumnInfo.length > 0) {
                        self.getColumnList();
                        $(".addColumnsUnselectedRegion").height($(".addColumnsBody").height() - 35);
                    }
                    else
                        self.getColumnList();
                }
                else {
                    self.getColumnList();
                }
            });
        },

        changeSpinner: function (event) {
            var self = this;
            $(".addColumnsSelectedRegion").find("[groupApplied]").removeAttr("groupApplied");
            if ($(event.target).closest(".headerSectionGroupSpinner").children().last().css("display") == "inline-block") {
                $(event.target).closest(".headerSectionGroupSpinner").children().last().css({ "display": "none" })
            }
            $(event.target).closest(".headerSectionGroupSpinner").next().attr({ "groupapplied": true });
            if (self.doChange) {
                if ($(".headerSectionAggregation").attr("spinnerinitial") != undefined) {
                    //if(true) {
                    $(".headerSectionAggregation").removeAttr("spinnerinitial");
                    var domCollection = $(".selectedRegionBody").find('tr.columnBodySection');
                    for (var i = 0; i < domCollection.length; i++) {
                        if ($(domCollection[i]).find(".headerSectionOperationDropDown").attr("groupapplied") == undefined) {
                            if ($(domCollection[i]).find(".dataTypeColumn").html().trim().toLowerCase() != "string" && $(domCollection[i]).find(".dataTypeColumn").html().trim().toLowerCase() != "date")
                                $($(domCollection[i]).find(".headerSectionOperationDropDown")).find("#firstOperationDiv").text("SUM");
                        }
                    }
                }
            }
        },

        getColumnList: function () {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetColumnOfTableFromDatabase',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ databaseName: toolKit.DBName, dbIdentifier: toolKit.dbIdentifier, tableName: toolKit.addColumns.tableName })
            }).then(function (responseText) {
                columnsList = $.parseJSON(responseText.d);
                if (toolKit.CurrentTabInfo.DataSourceInfo.TableOrViewName == self.tableName) {
                    self.bindColumnsList(columnsList, toolKit.CurrentTabInfo.ColumnInfo);
                }
                else {
                    self.bindColumnsList(columnsList, []);
                }
                $(".addColumnsUnselectedRegion").height($(".addColumnsBody").height() - 35);
            });
        },

        appendTableName: function (tableName) {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + "/GetHtmlTemplate",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.addColumnDataSource.htm' })
            }).then(function (responseText) {

                $(".dataSourceHeader").append(responseText.d);
                $(".columnDBName").append("<span>DATABASE : </span><span style=\"color:#2c2c2c\">" + toolKit.dbIdentifier.toUpperCase() + "</span>");
                //                $(".columnTableName").append("<span>TABLE : </span>" + "<span style=\"color:#00BFF0\">" + toolKit.CurrentTabInfo.DataSourceInfo.TableOrViewName.toUpperCase() + "</span>");
                $(".columnTableName").append("<span>TABLE : </span>" + "<span style=\"color:#00BFF0\">" + self.tableName.toUpperCase() + "</span>");

                $(".columnDBName").click(function () {
                    self.dbNameClickHandler(event);
                })

                $(".columnTableName").click(function () {
                    self.tableNameClickHandler(event);
                })
                $(".btnAddColumn").click(function (event) {
                    $("#globalFilterPopUp").remove();
                    if ($(event.target).html().trim() === "Next : Column Formatting") {
                        //$(event.target).html("Back:Column Formatting")
                        $(event.target).hide();
                        toolKit.addColumns.createSelectedRegion(event);
                    }
                    else {
                        $(event.target).html("Next : Column Formatting")
                        toolKit.addColumns.showUnSelectedRegion(event);
                    }
                })

            })
        },

        dbNameClickHandler: function (event) {
            var self = this;
            if ($(event.target).hasClass("dbIdentifierChild")) {
                toolKit.dbIdentifier = $(event.target).html();
                toolKit.CurrentTabInfo.DataSourceInfo.TableOrViewName = "";
                self.tableNameClickHandler();
            }
            if (!toolKit.CurrentTabInfo.IsTabSaved)
                $(event.target).parent().append(toolKit.utility.getDbIDentiferDropDown());
        },

        tableNameClickHandler: function () {
            toolKit.tabContentContainer.empty();
            $("#globalFilterPopUp").remove();
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetHtmlTemplate',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.addDataSource.htm' })
            }).then(function (responseText) {
                $(".dataSourceHeader").empty();
                $(".dataSourceHeader").height(73);
                $(".dataSourceHeader").append(responseText.d);
                var dataSource = ["DataBase 01", "DataBase 02", "DataBase 03", "DataBase 04"];
                toolKit.addDataSource.bindDataSource(dataSource);
                toolKit.addTables.createTablesdropDown();
                //toolKit.handlers.buttonBarClickHandler();
            });
        },

        keyUpEventHandler: function () {
            var self = this;
            var collection = $(".unSelectedRegionBody").find(".columnsToSelectParent");
            var length = collection.length;
            var searchText = $(".searchNewColumnsInmput").val();
            for (var i = 0; i < length; i++) {
                if ($(collection[i]).find(".columnsToSelectChild").html().toLowerCase().indexOf(searchText.toLowerCase()) > -1) {
                    $(collection[i]).removeClass("attributeNameHidden");
                }
                else {
                    $(collection[i]).addClass("attributeNameHidden");
                }
            }
        },

        bindColumnsList: function (columnList, AddedColumnList) {
            var closure = this;
            var self = this;
            function AddColumnViewModel() {
                var self = this;
                self.ColumnsList = ko.observableArray();
                self.AddedColumnList = ko.observableArray();
                self.showAddedColumnBody = ko.computed(function () {
                    if (self.AddedColumnList().length > 0)
                        return true;
                    else
                        return false;
                });
                self.addColumn = function (model, event) {
                    toolKit.addColumns.eventHandler(event)
                }
                self.setHeight = ko.computed(function () {
                    if (self.AddedColumnList().length > 0) {
                        if (self.AddedColumnList() > 2)
                            return '40%';
                        else
                            return '60%';
                    }
                    else
                        return '80%';
                });
                self.keyupeventHandler = function (event) {
                    closure.keyUpEventHandler(event);
                }
            }
            this.viewModel = new AddColumnViewModel();
            for (var i = 0; i < columnList.length; i++) {
                this.viewModel.ColumnsList.push({ ColumnName: columnList[i].ColumnName, DataType: closure.getDataTypeText(columnList[i].DataType),
                    checkIfColumnIsSelected: function (index) {
                        return closure.checkIfColumnIsSelected(index());
                    },
                    getOperationTypeText: function (model) {
                        return self.getOperation(model.ColumnName);
                    },
                    getAdvanceOperationTypeText: function (model) {
                        return self.getAdvanceOperation(model.ColumnName);
                    },
                    getRightFilterFlag: function (model) {
                        return self.getRightFilter(model.ColumnName);
                    },
                    getHiddenColumnFlag: function (model) {
                        return self.getHiddenColumn(model.ColumnName);
                    },
                    getTabFilter: function (model) {
                        return self.getTabFilter(model.ColumnName);
                    },
                    getGroupByOrder: function (model) {
                        return self.getGroupByOrder(model.ColumnName);
                    },
                    getFormatter: function (model) {
                        return self.getFormatter(model.ColumnName);
                    }
                });
            }
            if (AddedColumnList != null) {
                for (var i = 0; i < AddedColumnList.length; i++) {
                    this.viewModel.AddedColumnList.push({ "ColumnName": AddedColumnList[i].ColumnName,
                        "DisplayColumnName": AddedColumnList[i].DisplayColumnName,
                        "DataType": closure.getDataTypeText(AddedColumnList[i].DataType),
                        "Unit": "None",
                        "NegativeValue": "Default",
                        "Prefix": (AddedColumnList[i].CustomFormatter != null) ? AddedColumnList[i].CustomFormatter.Prefix : "",
                        "DecimalPlaces": "2",
                        "GroupByOrder": AddedColumnList[i].GroupByOrder,
                        "GroupByOperation": AddedColumnList[i].GroupByOperation,
                        "GroupAdvanceOperation": (AddedColumnList[i].GroupAdvanceOperation == null ? 0 : AddedColumnList[i].GroupAdvanceOperation),
                        "IsColumnHidden": AddedColumnList[i].IsHidden,
                        "IsRightFilter": AddedColumnList[i].IsRightFilter,
                        "IsRangeFilter": AddedColumnList[i].IsRangeFilter,
                        "IsTag": AddedColumnList[i].IsTag,
                        "mappedGlobalParamVal": AddedColumnList[i].mappedGlobalParam,
                        "mappedGlobalParam": function (model) {
                            return self.getGlobalParamText(model);
                        },
                        "addHiddenClass": function (model) {
                            return self.addHiddenClass(model);
                        },
                        "addRightFilterClass": function (model) {
                            return self.addRightFilterClass(model);
                        },
                        "getGorupByOperationText": function (model) {
                            return self.getOperation(model.ColumnName);
                        },
                        "getAdvanceOperationText": function (model) {
                            return self.getAdvanceOperation(model.ColumnName);
                        }
                    });
                }
            }
            //viewModel.AddedColumnList.push({ "ColumnName": "Test", "Unit": "None", "NegativeValue": "Default", "Prefix": "$", "DecimalPlaces": "2" });
            ko.applyBindings(this.viewModel, $("#addColumnsBody")[0])
            $(".selectedRegionBody").find('tbody').sortable({ axis: 'y', containment: 'parent', items: '.columnBodySection', handle: '.toolKithamBurger', start: function (event, ui) {
                $(ui.placeholder).addClass("ui-helper");
            }
            });
            $(".unSelectedRegionBody").append("<div style=\"clear:both;\"></div>");
            var domCollection = $(".selectedRegionBody").find('tbody').find('tr');
            self.doChange = false;
            for (var i = 0; i < domCollection.length; i++) {
                if ($(domCollection[i]).find(".dataTypeColumn").html().toLowerCase() == "string" || $(domCollection[i]).find(".dataTypeColumn").html().toLowerCase() == "date") {
                    $(domCollection[i]).find(".formatterClass").css({ 'background-color': 'lightgray' });
                }
                if (AddedColumnList[i].LinkColumn != null) {
                    if (AddedColumnList[i].LinkColumn.URL == "" || AddedColumnList[i].LinkColumn.URL == null)
                        $(domCollection[i]).find(".lnkColumn").addClass("attributeNameHidden");
                    else {
                        $(domCollection[i]).find(".lnkColumn").removeClass("attributeNameHidden");
                        $(domCollection[i]).attr('url', AddedColumnList[i].LinkColumn.URL);
                        $(domCollection[i]).attr('DisplayText', AddedColumnList[i].LinkColumn.DisplayText);
                        $(domCollection[i]).find(".txtDisplayName").attr('disabled');
                    }
                }
                else {
                    $(domCollection[i]).find(".lnkColumn").addClass("attributeNameHidden");
                }
                var spinnerValue = $(domCollection[i]).find(".inputSectionSpinner").spinner({ min: 0, value: Number.parseInt(self.viewModel.AddedColumnList()[i].GroupByOrder), change: function (event, ui) { self.changeSpinner(event); } });
                spinnerValue.spinner("value", Number.parseInt(self.viewModel.AddedColumnList()[i].GroupByOrder));
                if (Number.parseInt(self.viewModel.AddedColumnList()[i].GroupByOrder) > 0) {
                    $(".headerSectionAggregation").removeAttr("spinnerinitial");
                }
                if (AddedColumnList[i].CustomFormatter != null) {
                    var formatterInfo = AddedColumnList[i].CustomFormatter;
                    var dom = $(domCollection[i]);
                    if (formatterInfo.DataType != null && formatterInfo.DataType != 0) {
                        dom.attr('unit', self.getUnitText(formatterInfo.DataType));
                        dom.attr('ngvalue', self.getNgText(formatterInfo.NegativeValue));
                        dom.attr('dataType', self.getformatterDataType(formatterInfo.DataType));
                        dom.attr('dcmPlace', formatterInfo.DecimalPlaces);
                        dom.attr('prefix', formatterInfo.Prefix);
                    }
                    else {
                        dom.attr('assemblyName', formatterInfo.AssemblyName);
                        dom.attr('className', formatterInfo.ClassName);
                    }
                }
            }
            self.doChange = true;
        },

        checkIfColumnIsSelected: function (index) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (self.viewModel.ColumnsList()[index].ColumnName == toolKit.CurrentTabInfo.ColumnInfo[i].ColumnName) {
                    var div = $("<div>");
                    div.addClass("circleDiv fa fa-check");
                    $($(".unSelectedRegionBody").find(".columnsToSelectParent")[index]).append(div);
                    return "columnsToSelectChildSelected";
                }
            }
        },

        getDataTypeText: function (dataType) {
            switch (dataType) {
                case 0:
                    return 'String';
                case 1:
                    return 'Date';
                case 2:
                    return 'Numeric';
            }
        },

        eventHandler: function (event) {
            $("#globalFilterPopUp").remove();
            $(".dbIdentifierParent").remove();
            if ($(event.target).hasClass("columnsToSelectChild")) {
                toolKit.addColumns.addColumn(event);
            }
            else if (event.target.className == "headerSectionDelete fa fa-trash-o")
                toolKit.addColumns.removeColumnFromSelectedRegion(event);
            else if (event.target.className == "saveTabInfo") {
                //if(toolKit.addColumns.ValidationBeforeSavingTab()==true)
                toolKit.addColumns.getColumnInfo();
            }
            else if (event.target.className == "seacrhAddedColIcon fa fa-search") {
                if ($(event.target).next().find('.searchAddedColumnsInmput').hasClass("addBorder")) {
                    $(event.target).next().find('.searchAddedColumnsInmput').removeClass("addBorder");
                }
                else {
                    $(event.target).next().find('.searchAddedColumnsInmput').addClass("addBorder");
                }
            }
            else if (event.target.className == "searchAddedColumnsInmput") {
                $(event.target).addClass("addBorder");
            }
            else if (event.target.className == "seacrhNewColIcon") {
                if ($(event.target).next().find('.searchNewColumnsInmput').hasClass("addBorder")) {
                    $(event.target).next().find('.searchNewColumnsInmput').removeClass("addBorder");
                }
                else {
                    $(event.target).next().find('.searchNewColumnsInmput').addClass("addBorder");
                }
            }
            else if (event.target.className == "searchNewColumnsInmput") {
                $(event.target).addClass("addBorder");
            }
            else if (event.target.className == "btnAddColumn") {
                toolKit.addColumns.createSelectedRegion(event);
            }
            else if (event.target.className == "btnSelectAllColumn") {
                toolKit.addColumns.SelectAllColumns(event);
            }
            else if (event.target.className == "btnAddColumnAgain") {
                toolKit.addColumns.showUnSelectedRegion(event);
                $(".btnAddColumn").show();
            }
            else if (event.target.className == "addcolumnHeader addcolumnHeaderHidden") {
                toolKit.addColumns.openAddColumnSection();
            }
            else if ($(event.target).attr('id') == "btnAddLinkColumn") {
                toolKit.addColumns.openLinkPopUp(event);
            }
            else if (event.target.className == "headerSectionLnkColumnDiv") {
                toolKit.addColumns.openLinkPopUp(event);
            }
            else if (event.target.className == "btnSaveLink") {
                toolKit.addColumns.addColumnForLink(event);
            }
            else if ($(event.target).attr('id') == "btnAddFxColumn" || $(event.target).hasClass("fxDataColumn")) {
                toolKit.addColumns.OpenTagging(event);
            }
            else if ($(event.target).hasClass("lnkColumn")) {
                toolKit.addColumns.openLinkPopUp(event);
            }
            else {
                toolKit.addColumns.createDropDown(event);
            }
        },
        ValidationBeforeSavingTab: function () {
            if ($(".headerSectionAggregation").attr("spinnerinitial") != undefined) {
                return true;
            }
            else {
                for (var i = 0; i < $(".columnBodySection").length; i++) {
                    if ($($(".columnBodySection")[i]).find("#GroupSpinner").spinner("value") == null)
                        $($(".columnBodySection")[i]).find(".toolkitGroupAlertIcon").css({ "display": "" })
                }
                return false;
            }

        },

        showUnSelectedRegion: function (event) {
            var self = this;
            $(".unSelectedRegionParent").addClass("addColumnHeaderAbs");
            $(".selectedRegionParent").addClass("opacityClass");
            $(".addcolumnHeader").removeClass("addcolumnHeaderHidden");
            $(".unSelectedRegionParent").show("blind", 1000, function () {
            });
            $(".unSelectedRegionParent").css({ 'width': '100%' });
        },



        OpenTagging: function (event) {
            var self = this;
            var columnInfo = self.CreateColumnInfo();
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            var globalParam = toolKit.utility.getRightFIlter();

            $.ajax({
                url: url + "/InitializeTagging",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ ColumnInfoJson: JSON.stringify(columnInfo), tabName: toolKit.CurrentTabInfo.TabName, identifier: toolKit.reportInfo.reportIdentifier, dbIdentifier: toolKit.dbIdentifier, GlobalParam: globalParam })
            }).then(function (responseText) {
                var gridinfo = $.parseJSON(responseText.d);
                gridinfo.taggingInfoID = "popUpDiv";
                if ($("#popUpDiv").length == 0)
                    toolKit.tabContentContainer.append("<div id ='popUpDiv'></div>");
                else
                    $("#" + gridinfo.taggingInfoID).tagging('destroyPlugin');
                $("#" + gridinfo.GridId).empty();
                $("#" + gridinfo.taggingInfoID).tagging(
                {
                    IsBindGrid: false,
                    gridInfo: gridinfo,
                    serviceURL: toolKit.baseUrl + "/Resources/Services/TagManagement.svc",
                    ruleEditorServiceUrl: toolKit.baseUrl + "/Resources/Services/RADXRuleEditorService.svc",
                    actionBy: (toolKit.ShowReport ? iago.user.userName : "All"),
                    getAllTags:toolKit.ShowReport,
                    isPersistantTextBoxRequired: false,
                    pageIdentifier: toolKit.reportInfo.reportIdentifier + toolKit.CurrentTabInfo.TabName,
                    tagHeading: 'Fx Columns',
                    ExternalFunctionForSaveTag: toolKit.addColumns.saveColumnFromTagging
                });
            });
        },
        saveColumnFromTagging: function (tagInfo, isDelete) {
            if (!isDelete) {
                var self = toolKit.addColumns;
                var datatype = 0;
                if (tagInfo.DataTypeDetails.DataType != 0)
                    datatype = 2;
                var i;
                var groupOperation = '';



                var obj = { "ColumnName": tagInfo.TagRealName,
                    "DisplayColumnName": tagInfo.TagName,
                    "DataType": self.getDataTypeText(datatype),
                    "Unit": "None",
                    "NegativeValue": "Default",
                    "Prefix": "",
                    "DecimalPlaces": "2",
                    "GroupByOrder": 0,
                    "GroupByOperation": "None",
                    "GroupAdvanceOperation": "None",
                    "IsColumnHidden": false,
                    "IsTag": true,
                    "IsRightFilter": false,
                    "IsRangeFilter": false,
                    "mappedGlobalParamVal": "",
                    "mappedGlobalParam": function (model) {
                        return self.getGlobalParamText(model);
                    },
                    "addHiddenClass": function (model) {
                        return self.addHiddenClass(model);
                    },
                    "addRightFilterClass": function (model) {
                        return self.addRightFilterClass(model);
                    },
                    "getGorupByOperationText": function (model) {
                        return self.getOperation(model.ColumnName);
                    },
                    "getAdvanceOperationText": function (model) {
                        return self.getAdvanceOperation(model.ColumnName);
                    }
                }

                var columnInfo = self.CreateColumnInfo();
                for (i = 0; i < columnInfo.length; ++i) {
                    if (columnInfo[i].GroupByOrder > 0)
                        groupOperation = 'sum';
                }
                obj.GroupByOrder = 0;
                if (groupOperation.length > 0) {
                    obj.GroupByOperation = 3; //SUM
                }

                toolKit.addColumns.viewModel.AddedColumnList.push(obj);
                var domCollection = $(".selectedRegionBody").find('tbody').find('tr');
                $(domCollection[domCollection.length - 1]).find(".lnkColumn").addClass("attributeNameHidden");
                if (tagInfo.DataTypeDetails.DataType != 0) {
                    var dom = $(domCollection[domCollection.length - 1]);
                    if (tagInfo.DataTypeDetails.DataType != null && tagInfo.DataTypeDetails.DataType != 0) {
                        dom.attr('unit', self.getUnitText(tagInfo.DataTypeDetails.Unit));
                        dom.attr('ngvalue', self.getNgText(tagInfo.DataTypeDetails.NegativeValue));
                        dom.attr('dataType', self.getformatterDataType(tagInfo.DataTypeDetails.DataType + 1));
                        dom.attr('dcmPlace', tagInfo.DataTypeDetails.DecimalPlaces);
                        dom.attr('prefix', tagInfo.DataTypeDetails.Prefix);
                    }
                    else {
                        dom.attr('assemblyName', formatterInfo.AssemblyName);
                        dom.attr('className', formatterInfo.ClassName);
                    }
                }
                else {
                    self.doChange = false;
                    var spinnerValue = $(domCollection[domCollection.length - 1]).find(".inputSectionSpinner").spinner({ min: 0, value: Number.parseInt(obj.GroupByOrder.GroupByOrder), change: function (event, ui) { self.changeSpinner(event); } });
                    spinnerValue.spinner("value", Number.parseInt(obj.GroupByOrder));

                }

                columnInfo = self.CreateColumnInfo();
                toolKit.CurrentTabInfo.ColumnInfo = columnInfo;
                toolKit.newTabAdded = false;
                toolKit.CurrentTabInfo.dbIdentifier = toolKit.dbIdentifier;
                var tabInfo = JSON.stringify(toolKit.CurrentTabInfo);
                toolKit.utility.saveNPreviewTabInfo(tabInfo);
                //rad.TagManager.modalClose('.bootbox-close-button', true);
                self.doChange = true;
            }
            else {
                toolKit.addColumns.deleteTag(tagInfo);
            }
        },

        deleteTag: function (tagName) {
            var self = this;
            var index = -1
            for (var i = 0; i < toolKit.addColumns.viewModel.AddedColumnList().length; i++) {
                if (tagName == toolKit.addColumns.viewModel.AddedColumnList()[i].DisplayColumnName) {
                    index = i;
                    break;
                }
            }
            if (index > -1) {
                toolKit.addColumns.viewModel.AddedColumnList.splice(index, 1);
                columnInfo = self.CreateColumnInfo();
                toolKit.CurrentTabInfo.ColumnInfo = columnInfo;
                toolKit.newTabAdded = false;
                toolKit.CurrentTabInfo.dbIdentifier = toolKit.dbIdentifier;
                var tabInfo = JSON.stringify(toolKit.CurrentTabInfo);
                toolKit.utility.saveNPreviewTabInfo(tabInfo);
                //rad.TagManager.modalClose('.bootbox-close-button', true);
                self.doChange = true;
            }
        },

        addColumnForLink: function (event) {
            var self = this;
            if ($(event.target).closest(".editlnkColumnPopUp").length > 0) {
                index = $(event.target).closest(".editlnkColumnPopUp").attr('index');

                $($(".selectedRegionBody").find('tbody').find('tr')[index]).attr('url', $(event.target).closest(".editlnkColumnPopUp").find(".txtUrlText").val());
                $($(".selectedRegionBody").find('tbody').find('tr')[index]).attr('displayText', $(event.target).closest(".editlnkColumnPopUp").find(".txtDisplayText").val());
                $($(".selectedRegionBody").find('tbody').find('tr')[index]).find(".txtDisplayName").attr('disabled');
                $(event.target).closest(".editlnkColumnPopUp").remove();
            }
            else {
                toolKit.addColumns.viewModel.AddedColumnList.push({ "ColumnName": $(event.target).closest(".lnkColumnPopUp").find(".txtColumnDisplayText").val(),
                    "DisplayColumnName": $(event.target).closest(".lnkColumnPopUp").find(".txtColumnDisplayText").val(),
                    "Unit": "None",
                    "NegativeValue": "Default",
                    "Prefix": "",
                    "DecimalPlaces": "2",
                    "DataType": "String",
                    "GroupByOrder": 0,
                    "GroupByOperation": "None",
                    "GroupAdvanceOperation": "None",
                    "IsColumnHidden": false,
                    "IsTag": false,
                    "IsRightFilter": false,
                    "IsRangeFilter": false,
                    "mappedGlobalParamVal": "",
                    "mappedGlobalParam": function (model) {
                        return self.getGlobalParamText(model);
                    },
                    "addHiddenClass": function (model) {
                        return self.addHiddenClass(model);
                    },
                    "addRightFilterClass": function (model) {
                        return self.addRightFilterClass(model);
                    },
                    "getGorupByOperationText": function (model) {
                        return self.getOperation(model.ColumnName);
                    },
                    "getAdvanceOperationText": function (model) {
                        return self.getAdvanceOperation(model.ColumnName);
                    }
                });
                $(event.target).closest(".lnkColumnPopUp").remove();
                $(".selectedRegionBody").find('tbody').find('tr').last().attr('url', $(event.target).closest(".lnkColumnPopUp").find(".txtUrlText").val());
                $(".selectedRegionBody").find('tbody').find('tr').last().attr('displayText', $(event.target).closest(".lnkColumnPopUp").find(".txtDisplayText").val());
                $(".selectedRegionBody").find('tbody').find('tr').last().find(".txtDisplayName").attr('disabled');
            }
        },

        openLinkPopUp: function (event) {
            this.createLinkPopUp(event);
        },

        createLinkPopUp: function (event) {
            var self = this;
            self.event = event;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + "/GetHtmlTemplate",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.toolKitLnkColumn.htm' })
            }).then(function (responseText) {
                if ($(self.event.target).hasClass("lnkColumn")) {
                    $(self.event.target).closest('td').append(responseText.d);
                }
                else {
                    $(".addColumnsBody").append(responseText.d);
                }
                if ($(self.event.target).hasClass("lnkColumn")) {
                    $(".lnkColumnPopUp").addClass("editlnkColumnPopUp").removeClass("lnkColumnPopUp");
                    $(".txtUrlText").val($(self.event.target).closest('tr').attr("url"));
                    $(".txtDisplayText").val($(self.event.target).closest('tr').attr("displaytext"));
                    $(".txtColumnDisplayText").val($(self.event.target).closest('tr').find(".addedColumnName").html());
                    $(".editlnkColumnPopUp").attr("editMode", "true");
                    $(".editlnkColumnPopUp").attr("index", $(self.event.target).closest('tr').index());
                    $(".txtColumnDisplayText").attr("disabled", true);
                    $(".txtColumnDisplayText").closest(".lnkColumnRow").css({ 'display': 'none' });
                    $(".editlnkColumnPopUp").css({ "left": (($(self.event.target).closest('td').width() + 26) + 'px') });
                }
            });
        },

        openAddColumnSection: function () {
            var self = this;
            if ($(".addcolumnHeader").hasClass("addcolumnHeaderHidden")) {
                $(".selectedRegionParent").hide("blind", 1000, function () {
                    $(".addColumnsSelectedRegion").height($(".addColumnsBody").height() - 80);
                    $(".addcolumnHeader").removeClass("addcolumnHeaderHidden");
                    $(".addColumnsUnselectedRegion").show("blind", 1000);
                });
            }
        },

        createSelectedRegion: function () {

            var height = $(".addColumnsBody").height();
            var self = this;
            $(".unSelectedRegionParent").hide("blind", 1000, function () {
                $(".addColumnsSelectedRegion").height(height - 80);
                $(".addcolumnHeader").addClass("addcolumnHeaderHidden");
                //var spinnerValue = $(".inputSectionSpinner").spinner({ min: 0, change: function (event, ui) { self.changeSpinner(event); } })
                $(".selectedRegionParent").removeClass("opacityClass");
                $(".selectedRegionParent").show();
                $("#pageHeaderCustomDiv").empty();
                
                if ($(".btnGlobalParameter").length == 0) {
                    $("#pageHeaderCustomDiv").append("<div class='btnGlobalParameterParent'><div class='btnGlobalParameter'>Parameter</div></div>");
                }
                if (toolKit.utility.showMappedTabs()) {
                    if ($(".btnAddTabRelation").length == 0) {
                        $("#pageHeaderCustomDiv").append("<div class='btnAddTabRelationParent'><div class='btnAddTabRelation'>Navigation</div></div>");
                    }
                }
                if (toolKit.CurrentTabInfo.IsTabSaved) {
                    if ($(".btnPreview").length == 0) {
                        $("#pageHeaderCustomDiv").append("<div class='btnPreviewParent'><div class='btnPreview'>Preview</div></div>");
                        $(".btnPreview").unbind("click").click(function () {
                            toolKit.addColumns.preview();
                        })
                    }
                }
                if ($("#pageHeaderCustomDiv").find(".saveTabInfo").length == 0) {
                    $("#pageHeaderCustomDiv").append("<div class='saveTabInfoParent'><div class=\"saveTabInfo\" >Save Tab</div></div>");
                    $(".saveTabInfo").unbind("click").click(function () {
                        toolKit.addColumns.getColumnInfo();
                    })
                }
                if ($("#pageHeaderCustomDiv").find(".btnReportConfig").length == 0 && !toolKit.ShowReport) {
                    $("#pageHeaderCustomDiv").append("<div class='btnReportConfigParent'><div class=\"btnReportConfig\" >All Reports</div></div>");
                    $(".btnReportConfig").unbind("click").click(function () {
                        toolKit.ShowReport = false;
                        toolKit.ExistingReports.CreateReportPopUp();
                    })
                }
            });
        },

        createDropDown: function (event) {
            var self = this;
            if (event.target.className == "globalColumnName" || event.target.className == "globalFilterText" || event.target.className == "globalFilterArrow fa fa-caret-down") {
                if ($(event.target).closest('td').prev().find('div').hasClass("rightColumnNameback")) {
                    $(".drpDwnUnit").remove();
                    $(".editlnkColumnPopUp").remove();
                    $(".drpDwnGlobalUnit").remove();
                    $(".drpDownGlobalUnit").remove();
                    $(".drpAdvDwnUnit").remove();
                    var div = $('<div>');
                    div.addClass("drpDwnGlobalUnit");
                    //                div.append("<div class=\"parameterLabel\"><div class=\"parameterLabelText\">Parameter</div><div class=\"parameterLabelDrpDwnParent\"><div class=\"parameterLabelDrpDwn\">Select One</div></div></div>")
                    var div = $('<div>');
                    div.addClass("drpDownGlobalUnit");
                    if ($(event.target).html() != "Parameters")
                        div.append("<div class='drpDwnGlobalUnitChild'>Parameters</div>");
                    for (var i = 0; i < toolKit.reportInfo.GlobalFilters.length; i++) {
                        div.append("<div class='drpDwnGlobalUnitChild'>" + toolKit.reportInfo.GlobalFilters[i].GlobalParameterName + "</div>");
                    }
                    if (toolKit.reportInfo.GlobalFilters.length > 0) {
                        if ($(event.target).closest('tr').find(".dataTypeColumn").html().toLowerCase() == "date") {
                            div.append("<div class=\"rangeSelect\"><div class='rangeSelectLabel'>Range</div><div class='rangeSelectDiv'><div class=\"isAdvancedOff\">ON</div> <div class=\"isAdvancedOn advanceSelected\">OFF</div></div></div>");
                        }
                    }
                    if (event.target.className == "globalColumnName")
                        $(event.target).parent().parent().append(div);
                    else
                        $(event.target).parent().parent().append(div);
                    if (event.target.className == "globalFilterText") {
                        div.find(".parameterLabelDrpDwn").html($(event.target).html())
                    }
                    if ($(event.target).closest('tr').attr('range') != null) {
                        if ($(event.target).closest('tr').attr('range') == "true") {
                            $(".isAdvancedOff").addClass("advanceSelected");
                            $(".isAdvancedOn").removeClass("advanceSelected");
                        }
                    }
                }
            }
            else if (event.target.className == "parameterLabelDrpDwn") {
                $(".drpDownGlobalUnit").remove();
                var div = $('<div>');
                div.addClass("drpDownGlobalUnit");
                if ($(event.target).html() != "Select One")
                    div.append("<div class='drpDwnGlobalUnitChild'>Select One</div>");
                for (var i = 0; i < toolKit.reportInfo.GlobalFilters.length; i++) {
                    div.append("<div class='drpDwnGlobalUnitChild'>" + toolKit.reportInfo.GlobalFilters[i].GlobalParameterName + "</div>");
                }
                div.insertAfter($(event.target));
                var index = $(event.target).closest('tr').index();
                var top = 55 + (34 * (index + 1)) + (10 * index);
                $(".drpDownGlobalUnit").css({ 'top': (top + 'px') });
            }
            else if ($(event.target).closest('.rangeSelect').length > 0) {
                if ($(event.target).hasClass("isAdvancedOff")) {
                    $(event.target).addClass("advanceSelected");
                    $(event.target).next().removeClass("advanceSelected");
                }
                else {
                    $(event.target).addClass("advanceSelected");
                    $(event.target).prev().removeClass("advanceSelected");
                }
            }
            else if ($(event.target).hasClass("drpDwnGlobalUnitChild")) {

                if ($(event.target).html() == "Parameters") {
                    var tdele = $(event.target).closest("td");
                    $(event.target).closest('tr').removeAttr("range");
                    $(event.target).closest("td").empty();
                    tdele.append("<div class=\"globalFilterParent\"><div class=\"globalColumnName\">P</div></div>")
                }
                else {
                    var text = $(event.target).html();
                    if ($(event.target).closest('tr').find(".dataTypeColumn").html().toLowerCase() == "date") {
                        if ($(".isAdvancedOff").hasClass("advanceSelected")) {
                            $(event.target).closest('tr').attr("range", "true");
                        }
                        else {
                            $(event.target).closest('tr').attr("range", "false");
                        }
                    }
                    var tdEle = $(event.target).closest("td");
                    $(event.target).closest("td").empty();
                    tdEle.append("<div class=\"tempParent\"><div class=\"globalFilterText\">" + text + "</div><div class=\"globalFilterArrow fa fa-caret-down\"></div>");
                }

                $(".drpDwnGlobalUnit").remove();
            }
            else if (event.target.className == "drpDwnChild") {
                $(event.target).closest(".drpDwnUnit").prev().html($(event.target).html());
                $(event.target).closest(".drpDwnUnit").prev().attr('enumValue', $(event.target).attr('value'));
                $(event.target).closest("tr").attr('formatterapplied', "true");
                $(event.target).closest(".drpDwnUnit").remove();
            }
            else if ($(event.target).hasClass("drpDwnGlobalUnitChild")) {
                if ($(event.target).html() == "Select One") {
                    var tdele = $(event.target).closest("td");
                    $(event.target).closest("td").empty();
                    tdele.append("<div class=\"globalFilterParent\"><div class=\"globalColumnName\">P</div></div>")
                }
                else {
                    var text = $(event.target).html();
                    var tdEle = $(event.target).closest("td");
                    $(event.target).closest("td").empty();
                    tdEle.append("<div class=\"tempParent\"><div class=\"globalFilterText\">" + text + "</div><div class=\"globalFilterArrow fa fa-caret-down\"></div>");
                }
                $(".drpDwnGlobalUnit").remove();
            }
            else if ($(event.target).hasClass("toolkitFirstOperationDiv")) {
                self.removeDropDowns(event);
                if ($(event.target).closest(".headerSectionOperationDropDown").attr("spinnerinitial") == undefined) {
                    //                if(true) {
                    var div = $('<div>');
                    div.attr('id', 'optionsMainDiv');
                    div.addClass("toolkitOperationsMainDiv");
                    div.append("<div value='0' class='toolKitEachOperation'>None</div>");
                    div.append("<div value='1' class='toolKitEachOperation'>Min</div>");
                    div.append("<div value='2' class='toolKitEachOperation'>Max</div>");
                    div.append("<div value='3' class='toolKitEachOperation'>Count</div>");
                    div.append("<div value='4' class='toolKitEachOperation'>Sum</div>");
                    div.append("<div value='5' class='toolKitEachOperation'>Average</div>");
                    div.css({ 'width': $(event.target).width() });
                    $(event.target).closest('td').append(div);
                }

            }
            else if ($(event.target).hasClass("toolKitEachOperation")) {
                $($(event.target).closest("td")).find(".toolkitFirstOperationDiv").text($(event.target).text());
                $(event.target).closest("table").find(".OperationDropdown").attr('enumvalue', $(event.target).attr('value'));
                $(event.target).parent().remove();
            }
            else if ($(event.target).hasClass("rightColumnName")) {
                self.removeDropDowns(event);
                if ($(event.target).attr('isChecked') == "true") {
                    $(event.target).attr('isChecked', false);
                    $(event.target).removeClass('rightColumnNameback');
                    $(event.target).closest('td').next().empty();
                    $(event.target).closest('td').next().append("<div class=\"globalFilterParent\"><div class=\"globalColumnName\">P</div></div>");
                }
                else {
                    $(event.target).attr('isChecked', true);
                    $(event.target).addClass('rightColumnNameback')
                }
            }
            else if (event.target.className == "formatterClass") {
                $(".drpDwnUnit").remove();
                $(".editlnkColumnPopUp").remove();
                $(".drpAdvDwnUnit").remove();
                $(".drpDwnGlobalUnit").remove();
                $(".toolkitOperationsMainDiv").remove();
                if ($(event.target).closest('tr').find(".dataTypeColumn").html().toLowerCase() != "string" && $(event.target).closest('tr').find(".dataTypeColumn").html().toLowerCase() != "date")
                    self.getFormatterUI(event);
            }
            else if ($(event.target).hasClass("hiddenColumn")) {
                self.removeDropDowns(event);
                if ($(event.target).hasClass("hiddenColumnSelected")) {
                    $(event.target).removeClass("hiddenColumnSelected");
                    $(event.target).closest('tr').removeAttr('hiddenColumn');
                }
                else {
                    $(event.target).addClass("hiddenColumnSelected");
                    $(event.target).closest('tr').attr('hiddenColumn', true);
                }
            }
            else if (event.target.className == "AdvOperationDropdownRpt") {
                if ($(".applyGroupClass").length > 0 && ($(event.target).closest("tr").find(".dataTypeColumn").html().trim().toLowerCase() != "string" && $(event.target).closest("tr").find(".dataTypeColumn").html().trim().toLowerCase() != "date")) {
                    self.removeDropDowns(event);
                    var div = $('<div>');
                    div.addClass("drpAdvDwnUnit");
                    if ($(event.target).html() != "None")
                        div.append("<div value='0' class='drpAdvDwnChild'>None</div>");
                    div.append("<div value='1' class='drpAdvDwnChild'>RunningTotal</div>");
                    div.append("<div value='2' class='drpAdvDwnChild'>Percentage</div>");
                    div.css({ 'width': $(event.target).width() });
                    $(event.target).parent().append(div);
                }
            }
            else if (event.target.className == "drpAdvDwnChild") {
                $(event.target).closest("td").find(".AdvOperationDropdownRpt").html($(event.target).html());
                $(event.target).closest("table").find(".AdvOperationDropdownRpt").attr('enumvalue', $(event.target).attr('value'));
                $(event.target).closest(".drpAdvDwnUnit").remove();
            }
            else if ($(event.target).hasClass("noGroupClass")) {
                $(".drpDwnUnit").remove();
                $(".editlnkColumnPopUp").remove();
                $(".drpAdvDwnUnit").remove();
                $(".drpDwnGlobalUnit").remove();
                $(".toolkitOperationsMainDiv").remove();
                $(event.target).removeClass("noGroupClass");
                $(event.target).addClass("applyGroupClass");
                $(".headerSectionAggregation").removeAttr("spinnerinitial");
            }
            else if ($(event.target).hasClass("applyGroupClass")) {
                $(".drpDwnUnit").remove();
                $(".editlnkColumnPopUp").remove();
                $(".drpAdvDwnUnit").remove();
                $(".drpDwnGlobalUnit").remove();
                $(".toolkitOperationsMainDiv").remove();
                $(event.target).addClass("noGroupClass");
                $(event.target).removeClass("applyGroupClass");
                $(".headerSectionAggregation").attr("spinnerinitial", false);
                if ($(".applyGroupClass").length == 0) {
                    $(".drpDwnUnit").remove();
                }
            }
            else {
                $(".drpDwnUnit").remove();
                if ($(event.target).closest(".editlnkColumnPopUp").length == 0)
                    $(".editlnkColumnPopUp").remove();
                $(".drpAdvDwnUnit").remove();
                $(".drpDwnGlobalUnit").remove();
                $(".drpDwnGlobalUnit").remove();
                $(".toolkitOperationsMainDiv").remove();
                if ($(event.target).closest(".divFormatterUI").length == 0)
                    $(".divFormatterUI").remove();
                if ($(event.target).closest(".lnkColumnPopUp").length == 0)
                    $(".lnkColumnPopUp").remove();
            }
        },


        removeDropDowns: function (event) {
            var self = this;
            $(".drpDwnUnit").remove();
            $(".editlnkColumnPopUp").remove();
            $(".drpAdvDwnUnit").remove();
            $(".drpDwnGlobalUnit").remove();
            $(".drpDownGlobalUnit").remove();
            $(".toolkitOperationsMainDiv").remove();
            if ($(event.target).closest(".divFormatterUI").length == 0)
                $(".divFormatterUI").remove();
        },

        getFormatterUI: function (event) {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + "/GetHtmlTemplate",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.toolKitPopUps.htm' })
            }).then(function (responseText) {
                $(".drpDwnUnit").remove();
                $(".drpAdvDwnUnit").remove();
                $(".drpDwnGlobalUnit").remove();
                $(".divFormatterUI").remove();
                var dom = $(event.target).parent();
                $(event.target).parent().append(responseText.d);
                $(".txtPrefix").attr('disabled', true);
                $(".txtDecimal").removeAttr('disabled');
                $(".divFormatterUI").click(function (event) {
                    self.formatterUIHandlers(event);
                    event.stopPropagation();
                })
                if (dom.attr('assemblyName') != null) {
                    $(".txtAssemblyName").val(dom.attr('assemblyName'))
                }
                if (dom.attr('className') != null) {
                    $(".txtAssemblyName").val(dom.attr('className'))
                }
            });
        },

        formatterUIHandlers: function (event) {
            var self = this;
            if (event.target.className == "btnSaveFormatter") {
                if ($(".btnColor").hasClass("btnNormal")) {
                    $(event.target).closest(".divFormatterUI").closest('tr').attr('formatterapplied', "Normal");
                    var dom = $(".divFormatterUI").parent().parent();
                    dom.removeAttr("assemblyName");
                    dom.removeAttr("className");
                    dom.attr('unit', $(".txtUnit").attr('enumValue'));
                    dom.attr('ngvalue', $(".txtNgValue").attr('enumValue'));
                    dom.attr('dcmPlace', $(".txtDecimal").val());
                    dom.attr('prefix', $(".txtPrefix").val());
                    dom.attr('dataType', $(".txtdataType").html());
                    $(".divFormatterUI").remove();
                }
                else {
                    $(event.target).closest("tr").attr('formatterapplied', "Advanced");
                    var dom = $(".divFormatterUI").parent().parent();
                    dom.removeAttr("unit");
                    dom.removeAttr("ngvalue");
                    dom.removeAttr("dcmPlace");
                    dom.removeAttr("prefix");
                    dom.removeAttr("dataType");
                    dom.attr('assemblyName', $(".txtAssemblyName").val());
                    dom.attr('className', $(".txtClassName").val());
                    $(".divFormatterUI").remove();
                }
            }
            else if (event.target.className == "btnCancelFormatter") {
                $(".divFormatterUI").remove();
            }
            else if (event.target.className == "btnAdvanced") {
                $(".btnAdvanced").addClass('btnColor');
                $(".btnNormal").removeClass('btnColor');
                $(".divNormalFormatterUI").hide();
                $(".divAdvancedFormatterUI").show();
                $(".divAdvancedFormatterUI").removeClass('formatHidden');

            }
            else if (event.target.className == "btnNormal") {
                $(".btnAdvanced").removeClass('btnColor');
                $(".btnNormal").addClass('btnColor');
                $(".divAdvancedFormatterUI").hide();
                $(".divNormalFormatterUI").show();
                $(".divNormalFormatterUI").removeClass('formatHidden');
                var dom = $(event.target).closest('tr');
                if (dom.attr("dataType") != null) {
                    $(".txtUnit").html(self.getUnitText(dom.attr("unit")));
                    $(".txtNgValue").html(self.getNgText(dom.attr("ngvalue")));
                    $(".txtDecimal").val(dom.attr("dcmPlace"));
                    $(".txtPrefix").val(dom.attr("prefix"));
                    $(".txtdataType").html(dom.attr('dataType'));
                }
                else {
                    $(".txtUnit").html("None");
                    $(".txtdataType").html("None");
                    $(".txtNgValue").html("None");
                    $(".txtDecimal").val("");
                    $(".txtPrefix").val("");
                }
            }
            else if (event.target.className == "txtdataType") {
                $(".drpDownData").remove();
                $(".drpDownUnit").remove();
                $(".drpDownUnitNgValue").remove();
                var div = $('<div>');
                div.addClass("drpDownData");
                div.append("<div value='0' class='drpDownChild'>None</div>");
                div.append("<div value='1' class='drpDownChild'>String</div>");
                div.append("<div value='2' class='drpDownChild'>Number</div>");
                div.append("<div value='3' class='drpDownChild'>Currency</div>");
                div.append("<div value='4' class='drpDownChild'>Percentage</div>");
                div.css({ 'width': $(event.target).width() });
                $(".divNormalFormatterUI").append(div);
            }
            else if (event.target.className == "txtUnit") {
                if ($(".txtdataType").html().toLowerCase() != "string" && $(".txtdataType").html().toLowerCase() != "percentage") {
                    $(".drpDownData").remove();
                    $(".drpDownUnit").remove();
                    $(".drpDownUnitNgValue").remove();
                    var div = $('<div>');
                    div.addClass("drpDownUnit");
                    div.append("<div value='0' class='drpDownChild'>None</div>");
                    div.append("<div value='1' class='drpDownChild'>Thousands</div>");
                    div.append("<div value='2' class='drpDownChild'>Millions</div>");
                    div.append("<div value='3' class='drpDownChild'>Billions</div>");
                    div.css({ 'width': $(event.target).width() });
                    $(".divNormalFormatterUI").append(div);
                }
            }
            else if (event.target.className == "txtNgValue") {
                $(".drpDownData").remove();
                $(".drpDownUnit").remove();
                $(".drpDownUnitNgValue").remove();
                var div = $('<div>');
                div.addClass("drpDownUnitNgValue");
                div.append("<div value='0' class='drpDownChild'>Default</div>");
                div.append("<div value='1' class='drpDownChild'>DefaultBrac</div>");
                div.append("<div value='2' class='drpDownChild'>Colored</div>");
                div.append("<div value='3' class='drpDownChild'>ColoredBrac</div>");
                div.css({ 'width': $(event.target).width() });
                $(".divNormalFormatterUI").append(div);
            }
            else if (event.target.className == "drpDownChild") {
                if ($(event.target).closest(".drpDownUnitNgValue").length > 0) {
                    $(event.target).closest("table").find(".txtNgValue").html($(event.target).html());
                    $(event.target).closest("table").find(".txtNgValue").attr('enumValue', $(event.target).attr('value'));
                    $(event.target).closest(".drpDownUnitNgValue").remove();
                }
                else if ($(event.target).closest(".drpDownData").length > 0) {
                    if ($(event.target).html() == "String" || $(event.target).html() == "None") {
                        $(".txtDecimal").attr('disabled', 'true');
                        $(".txtDecimal").val("");
                    }
                    else {
                        $(".txtDecimal").removeAttr("disabled");
                    }
                    if ($(event.target).html() == "Currency") {
                        $(".txtPrefix").removeAttr('disabled');
                    }
                    else {
                        $(".txtPrefix").attr('disabled', 'true');
                        $(".txtPrefix").val("");
                    }
                    $(event.target).closest("table").find(".txtdataType").html($(event.target).html());
                    $(event.target).closest("table").find(".txtdataType").attr('enumValue', $(event.target).attr('value'));
                    $(event.target).closest(".drpDownData").remove();
                }
                else {
                    $(event.target).closest("table").find(".txtUnit").html($(event.target).html());
                    $(event.target).closest("table").find(".txtUnit").attr('enumValue', $(event.target).attr('value'));
                    $(event.target).closest(".drpDownUnit").remove();
                }
            }
            else {
                $(".drpDownData").remove();
                $(".drpDownUnit").remove();
                $(".drpDownUnitNgValue").remove();
            }
        },

        CreateColumnInfo: function () {
            var self = this;
            var columnInfo = [];
            var domCollection = $(".selectedRegionBody").find('tr.columnBodySection');
            for (var i = 0; i < domCollection.length; i++) {
                var obj = {
                    ColumnName: $(domCollection[i]).find(".addedColumnName").html(),
                    DisplayColumnName: $(domCollection[i]).find(".txtDisplayName").val(),
                    IsHidden: ($(domCollection[i]).attr('hiddenColumn') != null ? true : false),
                    IsRightFilter: ($(domCollection[i]).find(".rightColumnName").attr("isChecked") == "true" ? true : false),
                    mappedGlobalParam: $(domCollection[i]).find(".globalFilterText").length > 0 ? $(domCollection[i]).find(".globalFilterText").html() : "",
                    mappedGlobalParam: $(domCollection[i]).find(".globalFilterText").length > 0 ? $(domCollection[i]).find(".globalFilterText").html() : "",
                    ReportName: toolKit.reportName,
                    DataType: self.getDatatype($(domCollection[i]).find(".dataTypeColumn")),
                    AttributeDetail: {
                        AttributeName: $(domCollection[i]).find(".txtDisplayName").val(),
                        AttributeRealName: $(domCollection[i]).find(".addedColumnName").html(),
                        //                        TableName: toolKit.CurrentTabInfo.DataSourceInfo.TableOrViewName
                        TableName: toolKit.addColumns.tableName
                    },
                    GroupAdvanceOperation: $(domCollection[i]).attr('enumvalue')
                }
                if (obj.mappedGlobalParam != "") {
                    obj.IsRightFilter = false;
                }
                if ($(domCollection[i]).attr("formatterapplied") != null) {
                    if ($(domCollection[i]).attr("formatterapplied") == "Normal") {
                        obj.CustomFormatter = {
                            DecimalPlaces: (($(domCollection[i]).attr("dcmPlace") == null || $(domCollection[i]).attr("dcmPlace") == "") ? 0 : $(domCollection[i]).attr("dcmPlace")),
                            Prefix: $(domCollection[i]).attr("prefix"),
                            NegativeValue: $(domCollection[i]).attr("ngvalue"),
                            Unit: ($(domCollection[i]).attr('unit') == null ? "None" : $(domCollection[i]).attr('unit')),
                            DataType: $(domCollection[i]).attr('dataType')//self.getDatatype($(domCollection[i]).find(".dataTypeColumn"))
                        }
                    }
                    else {
                        obj.CustomFormatter = {
                            AssemblyName: $(domCollection[i]).find(".txtDecimalPlace").val(),
                            ClassName: $(domCollection[i]).find(".txtPrefix").val()
                        }
                    }
                }
                if ($(domCollection[i]).find(".headerSectionGroupSpinner").find('div').length > 0) {
                    if ($(domCollection[i]).find(".headerSectionGroupSpinner").find('div').hasClass("applyGroupClass"))
                        obj.GroupByOrder = 1;
                    else
                        obj.GroupByOrder = 0;
                }
                else {
                    obj.GroupByOrder = 0;
                }
                //                if ($(domCollection[i]).attr('url') == null) {
                //                    if ($($(domCollection[i]).find(".headerSectionGroupSpinner")).find("#GroupSpinner").length > 0) {
                //                        obj.GroupByOrder = $($(domCollection[i]).find(".headerSectionGroupSpinner")).find("#GroupSpinner").spinner("value") == null ? 0 : $($(domCollection[i]).find(".headerSectionGroupSpinner")).find("#GroupSpinner").spinner("value")
                //                    }
                //                    else {
                //                        obj.GroupByOrder = 0;
                //                    }
                //                }
                //                else
                //                    obj.GroupByOrder = 0;

                if ($($(domCollection[i]).find(".headerSectionOperationDropDown")).find(".toolkitFirstOperationDiv").text().trim().toLowerCase() == "operation") {
                    obj.GroupByOperation = "None"
                    obj.GroupByOperation = 0;
                }
                else {
                    if (obj.GroupByOrder == 1) {
                        obj.GroupByOperation = $($(domCollection[i]).find(".headerSectionOperationDropDown")).find(".toolkitFirstOperationDiv").text().trim();
                    }
                    else {
                        if (obj.DataType == 0)
                            //obj.GroupByOperation = "None";
                            obj.GroupByOperation = 0;
                        else
                            obj.GroupByOperation = $($(domCollection[i]).find(".headerSectionOperationDropDown")).find(".toolkitFirstOperationDiv").text().trim();
                    }
                }
                if ($(domCollection[i]).attr('url') != null) {
                    obj.LinkColumn = {
                        url: $(domCollection[i]).attr('url'),
                        DisplayText: $(domCollection[i]).attr('displaytext')
                    }
                }
                if ($(domCollection[i]).attr('range') != null) {
                    if ($(domCollection[i]).attr('range') == "false") {
                        obj.IsRangeFilter = false;
                    }
                    else {
                        obj.IsRangeFilter = true;
                    }
                }
                if ($(domCollection[i]).attr('istag') != null) {
                    if ($(domCollection[i]).attr('istag')) {
                        obj.IsTag = true;
                    }
                    else {
                        obj.IsTag = false;
                    }
                }
                if ($($(domCollection[i]).find(".headerSectionAdvOperationDropDown")).find(".AdvOperationDropdownRpt").text().trim().toLowerCase() == "none")
                //obj.GroupAdvanceOperation = "None";
                    obj.GroupAdvanceOperation = 0;
                else
                    obj.GroupAdvanceOperation = $($(domCollection[i]).find(".headerSectionAdvOperationDropDown")).find(".AdvOperationDropdownRpt").text().trim();
                columnInfo.push(obj);
            }
            return columnInfo;
        },
        getColumnInfo: function () {
            var self = this;
            $("#globalFilterPopUp").remove();
            var columnInfo = self.CreateColumnInfo();
            toolKit.CurrentTabInfo.DataSourceInfo.TableOrViewName = self.tableName;
            toolKit.CurrentTabInfo.ColumnInfo = columnInfo;
            toolKit.CurrentTabInfo.dbIdentifier = toolKit.dbIdentifier;
            toolKit.newTabAdded = false;
            var tabInfo = JSON.stringify(toolKit.CurrentTabInfo);
            toolKit.utility.saveNPreviewTabInfo(tabInfo);
        },

        preview: function () {
            var self = this;
            $(".dataSourceHeader").remove();
            $("#globalFilterPopUp").remove();
            toolKit.utility.loadSelectedTab(toolKit.CurrentTabInfo.tabId);
        },

        raiseGridRenderComplete: function (eventType, gridid) {
            $(".dataSourceHeader").remove();
            toolKit.tabContentContainer.height($("#" + toolKit.identifier).height() - 15);
            $find(gridid).get_GridInfo().Height = (toolKit.tabContentContainer.height() - 110) + "px";
            if (eventType == "Filter") {

                var self = this;
                var RightFilterValues = {};
                var RightFilterArray = [];
                if (toolKit.RightFilterValues != null)
                    RightFilterValues = toolKit.RightFilterValues;
                for (var i = 0; i < toolKit.reportInfo.TabsDetails.length; i++) {
                    for (var j = 0; j < toolKit.reportInfo.TabsDetails[i].ColumnInfo.length; j++) {
                        if (toolKit.reportInfo.TabsDetails[i].ColumnInfo[j].IsRightFilter) {
                            if (RightFilterArray.indexOf(toolKit.reportInfo.TabsDetails[i].ColumnInfo[j].ColumnName) == -1)
                                RightFilterArray.push(toolKit.reportInfo.TabsDetails[i].ColumnInfo[j].ColumnName)
                        }
                    }
                }
                if (eventType == "Filter") {
                    for (var i = 0; i < $find(gridid).filter.filteredColumnInfo.length; i++) {
                        var values = [];

                        for (var j = 0; j < $find(gridid).filter.filteredColumnInfo[i].Values.length; j++) {
                            values.push($find(gridid).filter.filteredColumnInfo[i].Values[j]);
                        }
                        if ($find(gridid).filter.filteredColumnInfo[i].ColumnName in RightFilterValues) {
                            toolKit.utility.setRightFilter(RightFilterValues, gridid, $find(gridid).filter);
                        }
                    }

                }
            }
            else if (eventType == "ClientSideBinding" && toolKit.IsRightFilterApplied) {
                toolKit.IsRightFilterApplied = false;
                $find(gridid).filterData(toolKit.rightFilterInfo);
            }
        },

        getDatatype: function (element) {
            if (element.html().toLowerCase() == "string")
                return 0;
            else if (element.html().toLowerCase() == "date")
                return 1;
            else
                return 2;
        },



        getGlobalParamText: function (model) {
            if (model.mappedGlobalParamVal == "") {
                return "<div class=\"globalFilterParent\"><div class=\"globalColumnName\">P</div></div>";
            }
            else {
                return "<div class=\"tempParent\"><div class=\"globalFilterText\">" + model.mappedGlobalParamVal + "</div><div class=\"globalFilterArrow fa fa-caret-down\"></div></div>";
            }
        },

        getformatterDataType: function (value) {
            if (Number.isNaN(parseInt(value))) {
                return value;
            }
            else {
                switch (Number.parseInt(value)) {
                    case 0:
                        return "None";
                    case 1:
                        return "String";
                    case 2:
                        return "Number";
                    case 3:
                        return "Currency";
                    default:
                        return "Percentage";
                }
            }
        },



        getNgText: function (value) {
            if (Number.isNaN(parseInt(value))) {
                return value;
            }
            else {
                switch (Number.parseInt(value)) {
                    case 0:
                        return "DEFAULT";
                    case 1:
                        return "DEFAULTBRAC";
                    case 2:
                        return "COLORED";
                    case 3:
                        return "COLOREDBRAC";
                    default:
                        return "None";
                }
            }
        },

        getUnitText: function (value) {
            if (Number.isNaN(parseInt(value))) {
                return value;
            }
            else {
                switch (Number.parseInt(value)) {
                    case 0:
                        return "None";
                    case 1:
                        return "THOUSANDS";
                    case 2:
                        return "MILLIONS";
                    case 3:
                        return "BILLIONS";
                    default:
                        return "None";
                }
            }
        },

        addHiddenClass: function (model) {
            var self = this;
            if (model.IsColumnHidden)
                return "hiddenColumn hiddenColumnSelected";
            else
                return "hiddenColumn";
        },

        addRightFilterClass: function (model) {
            var self = this;
            if (model.IsRightFilter || model.mappedGlobalParamVal)
                return "rightColumnName rightColumnNameback";
            else
                return "rightColumnName";
        },

        removeColumnFromSelectedRegion: function (event) {
            var index = 0;
            var columnName = "";
            for (var i = 0; i < toolKit.addColumns.viewModel.AddedColumnList().length; i++) {
                columnName = $(event.target).closest(".columnBodySection").find(".addedColumnName").html().trim();
                if (columnName == toolKit.addColumns.viewModel.AddedColumnList()[i].ColumnName) {
                    index = i;
                    break;
                }
            }
            toolKit.addColumns.viewModel.AddedColumnList.splice(index, 1);
            //toolKit.addColumns.viewModel.ColumnsList.push(columnName);
            var collection = $(".columnsToSelectChildSelected");
            for (var i = 0; i < collection.length; i++) {
                if ($(collection[i]).html().trim() == columnName) {
                    $(collection[i]).removeClass("columnsToSelectChildSelected");
                    $(collection[i]).next().remove();
                }
            }
        },

        SelectAllColumns: function () {
            var self = this;
            var div = null;
            for (var i = 0; i < $(".unSelectedRegionBody").children().length; i++) {
                div = $("<div>");
                div.addClass("circleDiv fa fa-check");
                var child = $($(".unSelectedRegionBody").children()[i]).find(".columnsToSelectChild");
                child.addClass("columnsToSelectChildSelected");
                child.closest(".columnsToSelectParent").append(div);
                toolKit.addColumns.viewModel.AddedColumnList.push({ "ColumnName": child.html(),
                    "DisplayColumnName": child.html().replace(/_/g, " ").charAt(0).toUpperCase() + child.html().replace(/_/g, " ").slice(1),
                    "DataType": child.attr("datatype"),
                    "Unit": "None",
                    "NegativeValue": "Default",
                    "Prefix": "",
                    "DecimalPlaces": "2",
                    "GroupByOrder": 0,
                    "GroupByOperation": "None",
                    "GroupAdvanceOperation": "None",
                    "IsColumnHidden": false,
                    "IsTag": false,
                    "IsRightFilter": false,
                    "IsRangeFilter": false,
                    "mappedGlobalParamVal": "",
                    "mappedGlobalParam": function (model) {
                        return self.getGlobalParamText(model);
                    },
                    "addHiddenClass": function (model) {
                        return self.addHiddenClass(model);
                    },
                    "addRightFilterClass": function (model) {
                        return self.addRightFilterClass(model);
                    },
                    "getGorupByOperationText": function (model) {
                        return self.getOperation(model.ColumnName);
                    },
                    "getAdvanceOperationText": function (model) {
                        return self.getAdvanceOperation(model.ColumnName);
                    }
                    //                    "GroupByOrder": $(self.columnList[i]).attr('GroupByOrder'),
                    //                    "GroupByOperation": $(self.columnList[i]).attr('operation'),
                    //                    "GroupAdvanceOperation": $(self.columnList[i]).attr('AdvanceOperation'),
                    //                    "IsColumnHidden": ($(self.columnList[i]).attr('IsColumnHidden') == null ? false : true),
                    //                    "IsRightFilter": ($(self.columnList[i]).attr('IsRightFilter') == null ? false : true),
                    //                    "mappedGlobalParamVal": ($(self.columnList[i]).attr('mappedGlobalParam') == null ? "" : $(self.columnList[i]).attr('mappedGlobalParam')),
                    //                    "mappedGlobalParam": function (model) {
                    //                        return self.getGlobalParamText(model);
                    //                    },
                    //                    "addHiddenClass": function (model) {
                    //                        return self.addHiddenClass(model);
                    //                    },
                    //                    "addRightFilterClass": function (model) {
                    //                        return self.addRightFilterClass(model);
                    //                    }
                });
            }
        },

        addColumn: function (event) {
            var self = this;
            if ($(event.target).closest(".columnsToSelectParent").find(".circleDiv").length > 0) {
                $(event.target).closest(".columnsToSelectParent").find(".circleDiv").remove();
                $(event.target).removeClass("columnsToSelectChildSelected");
                var index = 0;
                var domCollection = $(".selectedRegionBody").find('tbody').find('tr');
                var length = domCollection.length;
                for (var i = 0; i < toolKit.addColumns.viewModel.ColumnsList().length; i++) {
                    columnName = $(event.target).html().trim();
                    if (columnName == toolKit.addColumns.viewModel.AddedColumnList()[i].ColumnName) {
                        index = i;
                        break;
                    }
                }
                toolKit.addColumns.viewModel.AddedColumnList().splice(index, 1);
                for (var i = 0; i < length; i++) {
                    columnName = $(event.target).html().trim();
                    if (columnName == $(domCollection[i]).find(".addedColumnName").html()) {
                        $(domCollection[i]).remove();
                    }
                }
            }
            else {
                var div = $("<div>");
                div.addClass("circleDiv fa fa-check");
                $(event.target).addClass("columnsToSelectChildSelected")
                $(event.target).closest(".columnsToSelectParent").append(div);
                toolKit.addColumns.viewModel.AddedColumnList.push({ "ColumnName": $(event.target).html(),
                    "DisplayColumnName": $(event.target).html().replace(/_/g, " ").charAt(0).toUpperCase() + $(event.target).html().replace(/_/g, " ").slice(1),
                    "DataType": $(event.target).attr("datatype"),
                    "Unit": "None",
                    "NegativeValue": "Default",
                    "Prefix": "",
                    "DecimalPlaces": "2",
                    "GroupByOrder": 0,
                    "GroupByOperation": "None",
                    "GroupAdvanceOperation": "None",
                    "IsColumnHidden": false,
                    "IsTag": false,
                    "IsRightFilter": false,
                    "IsRangeFilter": false,
                    "mappedGlobalParamVal": "",
                    "mappedGlobalParam": function (model) {
                        return self.getGlobalParamText(model);
                    },
                    "addHiddenClass": function (model) {
                        return self.addHiddenClass(model);
                    },
                    "addRightFilterClass": function (model) {
                        return self.addRightFilterClass(model);
                    },
                    "getGorupByOperationText": function (model) {
                        return self.getOperation(model.ColumnName);
                    },
                    "getAdvanceOperationText": function (model) {
                        return self.getAdvanceOperation(model.ColumnName);
                    }
                });

                var domCollection = $(".selectedRegionBody").find('tbody').find('tr');
                $(domCollection[domCollection.length - 1]).find(".lnkColumn").addClass("attributeNameHidden");
                if ($(domCollection[domCollection.length - 1]).find(".dataTypeColumn").html().toLowerCase() == "string" || $(domCollection[domCollection.length - 1]).find(".dataTypeColumn").html().toLowerCase() == "date") {
                    $(domCollection[domCollection.length - 1]).find(".formatterClass").css({ 'background-color': 'lightgray' });
                }
                self.doChange = false;
                var spinnerValue = $(domCollection[domCollection.length - 1]).find(".inputSectionSpinner").spinner({ min: 0, change: function (event, ui) { self.changeSpinner(event); } });
                spinnerValue.spinner("value", 0);
                self.doChange = true;
            }
        },
        getOperation: function (columnName) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].ColumnName == columnName) {
                    return self.getOperationText(toolKit.CurrentTabInfo.ColumnInfo[i].GroupByOperation)
                }
            }
            return "None";
        },

        getOperationText: function (operation) {
            if (Number.isNaN(parseInt(operation))) {
                return operation;
            }
            else {
                switch (parseInt(operation)) {
                    case 0:
                        return "None";
                    case 1:
                        return "MIN";
                    case 2:
                        return "MAX";

                    case 3:
                        return "SUM";
                    case 4:
                        return "AVERAGE";
                    case 5:
                        return "COUNT";
                    default:
                        return "None";
                }
            }
        },

        getAdvanceOperation: function (columnName) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].ColumnName == columnName) {
                    return self.getAdvanceOperationText(toolKit.CurrentTabInfo.ColumnInfo[i].GroupAdvanceOperation);
                }
            }
            return "None";
        },

        getRightFilter: function (columnName) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].ColumnName == columnName) {
                    return toolKit.CurrentTabInfo.ColumnInfo[i].IsRightFilter;
                }
            }
            return false;
        },

        getHiddenColumn: function (columnName) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].ColumnName == columnName) {
                    return toolKit.CurrentTabInfo.ColumnInfo[i].IsHidden;
                }
            }
            return false;
        },

        getTabFilter: function (columnName) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].ColumnName == columnName) {
                    return toolKit.CurrentTabInfo.ColumnInfo[i].mappedGlobalParam;
                }
            }
            return "";
        },

        getGroupByOrder: function (columnName) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].ColumnName == columnName) {
                    return toolKit.CurrentTabInfo.ColumnInfo[i].GroupByOrder;
                }
            }
            return 0;
        },

        getFormatter: function (columnName) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].ColumnName == columnName) {
                    if (toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter != null) {
                        if (toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.DataType != 0) {
                            return "Normal|" + toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.DataType + "|" + toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.NegativeValue + "|"
                                    + toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.DecimalPlaces + "|" + toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.Prefix;
                        }
                        else if (toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.AssemblyName != "") {
                            return "Advanced|" + toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.AssemblyName + "|" + toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.ClassName;
                        }
                        else
                            return ""
                    }
                }
            }
            return "";
        },

        getAdvanceOperationText: function (operation) {
            if (Number.isNaN(parseInt(operation))) {
                return operation;
            }
            else {
                switch (parseInt(operation)) {
                    case 0:
                        return "None";
                    case 1:
                        return "RunningTotal";
                    case 2:
                        return "Percentage";
                    default:
                        return "None";
                }
            }
        }
    }
})
$.extend(toolKit, {
addProcedure: {
        createProcedureDropDown: function () {
            var self = this;
            if ($(".addTableSourcePopup") != null) {
                $(".addTableSourcePopup").remove();
            }
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetHtmlTemplate',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path:'com.ivo.rad.RGridWriterToolKit.Resources.addProcedure.htm' })
            }).then(function (responseText) {
                $("#" + toolKit.identifier).append(responseText.d);
                self.getProcs();

            })

        },
        getProcs: function () {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetProcsForDB',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ databaseName: toolKit.DBName, dbIdentifier: toolKit.dbIdentifier })
            }).then(function (responseText) {
                toolKit.DataSourceCollection = JSON.parse(responseText.d);
                //  self.bindProcedures(["Test1", "Test2", "Test3", "Test4"]);
                self.bindProcedures(toolKit.DataSourceCollection);


            });
        },
        bindProcedures: function (ProceduresArray) {
            //            function ProceduresViewModel() {
            //                var self = this;
            //                self.ProcedureSource = ko.observableArray();

            //            }
            var self = this;
            self.viewModelProc = new toolKit.ProcedureViewModel();
            //toolKit.addProcedure.test = viewModelProc;
            // self.viewModelProc.ProcedureSource = [];
            for (var item in ProceduresArray) {
                self.viewModelProc.ProcedureSource.push(ProceduresArray[item]);
            }
            ko.cleanNode($(".addProcSourcePopup")[0]);
            ko.applyBindings(self.viewModelProc, $(".addProcSourcePopup")[0]);
            self.ProcedureHandler(event, self.viewModelProc);
            //self.ClickProcedure(event);

        },
        ProcedureHandler: function (event, viewModelProc) {

            $(".addProcSourcePopup").unbind("click").click(function (event) {
                toolKit.eventHandlers.ProcedureHandler(event);
            });
            $(".ProcedureSearchTextBox").keyup(function (event) {
                toolKit.eventHandlers.ProcedureHandlerKeyUp(event, viewModelProc);

            });
            //            var self = this;
            //            self.viewModelProc = new toolKit.ProcedureViewModel();
        }


    }


})
$.extend(toolKit, {
    addParametersForProcedure: {
        CreateParametersScreen: function (event, ProceduresArray) {
            var self = this;
            $("#" + toolKit.identifier).append("<div class=\"ProcParamSourceHeading\"><div class=\"ParameterText\">Parameter of</div><div class=\"parametersMainDiv\"></div></div>")

            $(".parametersMainDiv").append("<div class=\"parameterFirstDiv\">" + event + "</div>")

            for (var item in toolKit.DataSourceCollection) {
                $(".parametersMainDiv").append("<div class=\"parameterEachDiv\">" + toolKit.DataSourceCollection[item] + "</div>")
            }
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetHtmlTemplate',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.AddParametersForProcedure.htm' })
            }).then(function (responseText) {
                $(".addProcSourcePopup").remove();
                $("#" + toolKit.identifier).append(responseText.d);
                var a = ["ProcParam01", "ProcParam02", "ProcParam03", "ProcParam04"];


                self.getParametersForProcs(event);

            })

        },
        getParametersForProcs: function (event) {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetParameterForProc',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ databaseName: toolKit.DBName, procName: event, dbIdentifier: toolKit.dbIdentifier })
            }).then(function (responseText) {
                toolKit.parametersForProcedure = JSON.parse(responseText.d);
                self.bindParametersForProcedures(toolKit.parametersForProcedure);


            });
        },
        bindParametersForProcedures: function (ProcParametersArray) {
            var self = this;
            self.viewModelProc = new toolKit.ProcedureParamViewModel();
            
            for (var item in ProcParametersArray) {

                if (ProcParametersArray[item].ParamDataType == 0)
                    ProcParametersArray[item].ParamDataType = "VARCHAR";
                else if (ProcParametersArray[item].ParamDataType == 1)
                    ProcParametersArray[item].ParamDataType = "DATE";
                else if (ProcParametersArray[item].ParamDataType == 2)
                    ProcParametersArray[item].ParamDataType = "NUMERIC";
                self.viewModelProc.ProcedureParameterSourceName.push(ProcParametersArray[item]);
            }
            ko.cleanNode($(".addParamProcSourcePopup")[0]);
            ko.applyBindings(self.viewModelProc, $(".addParamProcSourcePopup")[0]);
            self.ProcedureHandler(event, self.viewModelProc);

        },
        ProcedureHandler: function (event, viewModelProc) {
            var self = this;
            $(".parametersMainDiv").unbind("click").click(function (event) {
                self.procedureDropDownHandler(event, viewModelProc);

            });

        },
        procedureDropDownHandler: function (event, viewModelProc) {
            var self = this;

            if ($(event.target).hasClass("parameterFirstDiv")) {
                if ($(".parametersMainDiv").height() === 25) {
                    $(".parametersMainDiv").height(220)
                }
                else if ($(".parametersMainDiv").height() === 220) {
                    $(".parametersMainDiv").height(25)
                }
            }
            else if ($(event.target).hasClass("parameterEachDiv")) {

                if ($(".parametersMainDiv").height() === 220) {
                    $(".parameterFirstDiv").text($(event.target).text())
                    $(".parametersMainDiv").height(25)
                    $(".parametersMainDiv").scrollTop(0)
                    viewModelProc.ProcedureParameterSourceName([]);
                    // $(".ProcParamSourceHeading").remove();
                    var self = this;
                    var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
                    $.ajax({
                        url: url + '/GetParameterForProc',
                        type: 'POST',
                        contentType: "application/json",
                        dataType: 'json',
                        data: JSON.stringify({ databaseName: toolKit.DBName, procName: $(event.target).text(), dbIdentifier: toolKit.dbIdentifier })
                    }).then(function (responseText) {
                        //  toolKit.DataSourceCollection = JSON.parse(responseText.d);
                        //  self.bindProcedures(["Test1", "Test2", "Test3", "Test4"]);
                        toolKit.parametersForProcedure = JSON.parse(responseText.d);
                        var ProcParametersArray = toolKit.parametersForProcedure;
                        for (var item in ProcParametersArray) {

                            if (ProcParametersArray[item].ParamDataType == 0)
                                ProcParametersArray[item].ParamDataType = "VARCHAR";
                            //    self.viewModelProc.ProcedureParameterSourceName.push("VARCHAR");
                            else if (ProcParametersArray[item].ParamDataType == 1)
                                ProcParametersArray[item].ParamDataType = "DATE";
                            //self.viewModelProc.ProcedureParameterSourceName.push("DATE");
                            else if (ProcParametersArray[item].ParamDataType == 2)
                                ProcParametersArray[item].ParamDataType = "NUMERIC";
                            //self.viewModelProc.ProcedureParameterSourceName.push("NUMERIC");
                            viewModelProc.ProcedureParameterSourceName.push(ProcParametersArray[item]);
                        }

                    });
                }
                else if ($(".parametersMainDiv").height() === 25) {
                    $(".parametersMainDiv").height(220)
                }
            }
        },
        CreateGlobalFilter: function () {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetHtmlTemplate',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.globalFilter.htm' })
            }).then(function (responseText) {
                $("#" + toolKit.identifier).append(responseText.d);
                $("#" + toolKit.identifier).find("#globalFilterPopUp").removeClass("globalFilterPopUp").addClass("globalFilterPopUpParamScreen");

            })

        }
    }

})
$.extend(toolKit, {
    addTables: {
        createTablesdropDown: function () {
            var self = this;
            if ($(".addProcSourcePopup") != null) {
                $(".addProcSourcePopup").remove();
            }
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetHtmlTemplate',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.AddTables.htm' })
            }).then(function (responseText) {
                self.tablesHTML = responseText.d;
                //                toolKit.tabContentContainer.append(responseText.d);
                
                $(".TableSourceBody").height(550);
                self.getTables();
            })

        },
        getTables: function () {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetTablesFromDatabase',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ databaseName: toolKit.DBName, dbIdentifier: toolKit.dbIdentifier })
            }).then(function (responseText) {
                toolKit.TablesCollection = JSON.parse(responseText.d);
                //  self.bindProcedures(["Test1", "Test2", "Test3", "Test4"]);
                self.bindTables(toolKit.TablesCollection);
            });
        },
        bindTables: function (TablesArray) {
            var self = this;
            self.viewModelProc = new toolKit.TableViewModel();
            //toolKit.addProcedure.test = viewModelProc;
            // self.viewModelProc.ProcedureSource = [];
            if (toolKit.CurrentTabInfo.DataSourceInfo.TableOrViewName != "" && toolKit.CurrentTabInfo.DataSourceInfo.TableOrViewName != null) {
                var index = TablesArray.indexOf(toolKit.CurrentTabInfo.DataSourceInfo.TableOrViewName);
                TablesArray.splice(index, 1);
                TablesArray.splice(0, 0, toolKit.CurrentTabInfo.DataSourceInfo.TableOrViewName);
            }
            for (var item in TablesArray) {
                self.viewModelProc.TablesSource.push({ tableName: TablesArray[item],
                    addClass: function (index) {
                        return self.addClass(index());
                    }
                });
            }
            toolKit.tabContentContainer.append(self.tablesHTML);
            ko.cleanNode($(".addTableSourcePopup")[0]);
            
            ko.applyBindings(self.viewModelProc, $(".addTableSourcePopup")[0]);
            self.TableHandler(event, self.viewModelProc, TablesArray);
        },

        addClass: function (index) {
            if (toolKit.TablesCollection[index] == toolKit.CurrentTabInfo.DataSourceInfo.TableOrViewName) {
                return "tableSourceItemSelected";
            }
        },

        TableHandler: function (event, viewModelProc, TablesArray) {
            $(".TableSearchTextBox").keyup(function (event) {
                toolKit.eventHandlers.TableHandlerKeyUp(event, viewModelProc, TablesArray);
            });
        }
    }

})
$.extend(toolKit, {
    addColumnsForTable: {
        CreateColumnsScreen: function (event, TablesArray) {

        }




    }

})
$.extend(toolKit, {
    bindTabs: {
        bindTab: function () {
            var self = this;
            self.getTabs();
            //self.getDataSourceDetails()
        },

        getTabs: function (tabNames) {
            var self = this;
            self.tabInfo = []
            for (var i = 0; i < toolKit.reportInfo.TabsDetails.length; i++) {
                self.tabInfo.push({
                    id: toolKit.reportInfo.TabsDetails[i].tabId,
                    name: toolKit.reportInfo.TabsDetails[i].TabName
                })
            }
            self.tabInfo[0].isDefault = true;
            var tabsObj = $("#pageHeaderTabPanel").data('iagoWidget-toolKittabs');
            if (tabsObj != null)
                $("#pageHeaderTabPanel").toolKittabs('destroy');
            $("#pageHeaderTabPanel").toolKittabs({
                tabSchema: { tab: self.tabInfo },
                tabClickHandler: this.tabClickHandler,
                tabContentHolder: toolKit.tabContainer,
                deleteTabs: (toolKit.ShowReport ? false : true),
                deleteTabHandler: this.deleteTabHandler,
                changeTabHandler: this.changeTabHandler

            });
        },

        getTabNames: function (tabNames) {
            self.tabInfo = []
            for (var i = 0; i < tabNames.length; i++) {
                self.tabInfo.push({
                    id: tabNames[i],
                    name: tabNames[i]
                })
            }
            tabInfo[0].isDefault = true;
            $("#pageHeaderTabPanel").toolKittabs({
                tabSchema: { tab: self.tabInfo },
                tabClickHandler: this.tabClickHandler,
                tabContentHolder: toolKit.tabContainer
            });
        },

        changeTabHandler: function (tabs) {
            //var a = 1;
            //toolKit.utility.showButtons();
            var tabsOrder = [];
            for (var i = 0; i < tabs.length; i++) {
                tabsOrder.push({ Key: Number.parseInvariant(tabs[i].id), Value: i });
            }
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                type: 'POST',
                url: url + "/ReOrderTab",
                dataType: 'json',
                contentType: "application/json",
                data: JSON.stringify({ TabPositions: tabsOrder, identifier: toolKit.reportInfo.reportIdentifier, user: iago.user.userName })
            }).then(function (responseText) {

            })
        },

        deleteTabHandler: function (tabid, tabName, isDelete, oldName) {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            if (isDelete) {
                $.ajax({
                    type: 'POST',
                    url: url + "/DeleteTab",
                    dataType: 'json',
                    contentType: "application/json",
                    data: JSON.stringify({ TabId: tabid, tabName: tabName, identifier: toolKit.reportInfo.reportIdentifier, user: iago.user.userName })
                }).then(function (responseText) {
                    if (responseText.d) {
                        toolKit.utility.alertPopUp(true, "Tab Deleted Successfully");
                        //var removeTab = -1;
                        //for (var i = 0; i < toolKit.bindTabs.tabInfo.length; i++) {
                        //    if (toolKit.bindTabs.tabInfo[i].id == tabid) {
                        //        removeTab = i;
                        //        break;
                        //    }
                        //}
                        //toolKit.bindTabs.tabInfo.splice(removeTab, 1);
                        //$("#pageHeaderTabPanel").toolKittabs('setSelectedTab', toolKit.bindTabs.tabInfo[0].id, true);
                        //$("li#" + tabid).remove();
                    }
                    else {
                        toolKit.utility.alertPopUp(false, "Tab Deletion failed");
                    }
                })
            }
            else {
                var tabInfo = null;
                for (var i = 0; i < toolKit.reportInfo.TabsDetails.length; i++) {
                    if (toolKit.reportInfo.TabsDetails[i].TabName == oldName) {
                        tabInfo = toolKit.reportInfo.TabsDetails[i];
                        break;
                    }
                }
                tabInfo.TabName = tabName;
                $.ajax({
                    type: 'POST',
                    url: url + "/RenameTab",
                    dataType: 'json',
                    contentType: "application/json",
                    data: JSON.stringify({ tabInfo: JSON.stringify(tabInfo), identifier: toolKit.reportInfo.reportIdentifier, user: iago.user.userName, oldTabName : oldName })
                }).then(function (responseText) {
                    if (responseText.d) {
                        toolKit.utility.alertPopUp(true, "Tab Renamed Successfully");
                    }
                    else {
                        toolKit.utility.alertPopUp(false, "Tab Rename failed");
                    }
                })
            }
        },

        getDataSourceDetails: function () {
            var self = this;

            //   if (self.tabInfo.dataSourceInfo.TableOrViewName != "") {
            //toolKit.addColumns.bindColumnsList(self.tabInfo.ColumnInfo);
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                url: url + '/GetToolkitInfo',
                data: JSON.stringify({ identifier: toolKit.identifierForReport + toolKit.reportName, dbIdentifier: toolKit.dbIdentifier }),
                dataType: "json"
            }).then(function (responseText) {
                var toolKitInfo = $.parseJSON(responseText.d);
                toolKit.CurrentTabInfo.TabName = toolKitInfo.TabsDetails[0].TabName;
                if (toolKit.CurrentTabInfo.dbIdentifier != "")
                    toolKit.dbIdentifier = toolKit.CurrentTabInfo.dbIdentifier;
                toolKit.ListOfTabInfo = toolKitInfo.TabsDetails;
                toolKit.actualInfo = toolKitInfo;
                var tabNames = []
                for (var item in toolKit.ListOfTabInfo) {
                    tabNames.push(toolKit.ListOfTabInfo[item].TabName);
                    toolKit.tabNameIDMapping[toolKit.ListOfTabInfo[item].TabName] = toolKit.ListOfTabInfo[item].tabId
                }
                if ($(".btnConfigure").length == 0) {
                    $("#pageHeaderCustomDiv").append("<div class='btnConfigure'>Configure</div>");
                    $(".btnConfigure").unbind('click').click(function (event) {
                        self.openColumnConfig(event);
                    })
                }
                self.getTabNames(tabNames);
            });
        },

        openColumnConfig: function () {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetHtmlTemplate',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.addDataSource.htm' })
            }).then(function (responseText) {
                toolKit.tabContentContainer.empty();
                if ($(".dataSourceHeader").length == 0) {
                    $("#" + toolKit.identifier).prepend("<div class=\"dataSourceHeader\"></div>");
                }
                else {
                    $(".dataSourceHeader").show();
                }
                $(".dataSourceHeader").append(responseText.d);
                $(".TextParent").addClass("TextParentDisplay");
                $(".DataSourceBody").addClass("DataSourceBodyDisplay");
                $(".dataSourceHeader").empty();
                $(".dataSourceHeader").append(responseText);
                var dataSource = ["DataBase 01", "DataBase 02", "DataBase 03", "DataBase 04"];
                toolKit.addDataSource.bindDataSource(dataSource);
                var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
                $.ajax({
                    url: url + "/GetHtmlTemplate",
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.addColumns.htm' })
                }).then(function (responseText) {
                    self.currenttabInfo = self.getTabInfo();
                    toolKit.addColumns.appendTableName(self.currenttabInfo.DataSourceInfo.TableOrViewName);
                    toolKit.tabContentContainer.empty();
                    toolKit.tabContentContainer.height({ height: '100%' });
                    toolKit.tabContentContainer.append(responseText.d);

                    //toolKit.utility.applytaggingPrivilege();

                    if (toolKit.RequireTagLink)
                        $(".btnAddLinkColumn").addClass("hiddenClass");
                    $(".unSelectedRegionParent").hide();
                    $(".selectedRegionParent").show();
                    var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
                    $.ajax({
                        url: url + '/GetColumnOfTableFromDatabase',
                        type: 'POST',
                        contentType: "application/json",
                        dataType: 'json',
                        data: JSON.stringify({ databaseName: toolKit.DBName, dbIdentifier: toolKit.dbIdentifier, tableName: self.currenttabInfo.DataSourceInfo.TableOrViewName })
                    }).then(function (responseText) {
                        columnsList = $.parseJSON(responseText.d);
                        $(".addColumnsUnselectedRegion").height($(".addColumnsBody").height() - 35);
                        toolKit.addColumns.bindColumnsList(columnsList, self.currenttabInfo.ColumnInfo);
                    });
                });
            });
        },

        getTabInfo: function (selectedTabId) {
            var self = this;
            var tabName = 0;
            for (var i = 0; i < self.tabInfo.length; i++) {
                if (self.tabInfo[i].id == selectedTabId) {
                    tabName = self.tabInfo[i].name;
                }
            }
            for (var i = 0; i < toolKit.reportInfo.TabsDetails.length; i++) {
                if (toolKit.reportInfo.TabsDetails[i].TabName == tabName) {
                    return toolKit.reportInfo.TabsDetails[i]; // .DataSourceInfo.TableOrViewName;
                }
            }
        },



        tabClickHandler: function (selectedTabId, tabContentContainer) {
            $("#globalFilterPopUp").remove();
            toolKit.CurrentTabInfo = toolKit.utility.getTabInfo(Number.parseInvariant(selectedTabId))
            if (toolKit.CurrentTabInfo.dbIdentifier != "" && toolKit.CurrentTabInfo.dbIdentifier != null)
                toolKit.dbIdentifier = toolKit.CurrentTabInfo.dbIdentifier;
            toolKit.tabContentContainer = tabContentContainer;
            toolKit.tabContentContainer.css({ 'background-color': '#F2F5F8' });
            toolKit.tabContentContainer.empty();
            toolKit.bindTabs.getSelectedTab(Number.parseInvariant(selectedTabId));
        },


        getSelectedTab: function (tabId) {
            if (toolKit.IsFromContextMenu) {
                toolKit.IsFromContextMenu = false;
                toolKit.utility.loadSelectedTabWithFilter(toolKit.CurrentTabInfo.tabId);
            }
            else if (toolKit.ShowReport) {
                $(".dataSourceHeader").remove();
                toolKit.utility.loadSelectedTab(toolKit.CurrentTabInfo.tabId);
            }
            else {
                $(".btnConfigureScreen").remove();
                $("#pageHeaderCustomDiv").empty();
                if ($(".btnGlobalParameter").length == 0) {
                    $("#pageHeaderCustomDiv").append("<div class='btnGlobalParameterParent'><div class='btnGlobalParameter'>Parameter</div></div>");
                }
                if (toolKit.CurrentTabInfo.IsTabSaved) {
                    if ($(".btnPreview").length == 0) {
                        $("#pageHeaderCustomDiv").append("<div class='btnPreviewParent'><div class='btnPreview'>Preview</div></div>");
                        $(".btnPreview").unbind("click").click(function () {
                            toolKit.addColumns.preview();
                        })
                    }
                }
                if (toolKit.utility.showMappedTabs()) {
                    if ($(".btnAddTabRelation").length == 0) {
                        $("#pageHeaderCustomDiv").append("<div class='btnAddTabRelationParent'><div class='btnAddTabRelation'>Navigation</div></div>");
                    }
                }
                if ($("#pageHeaderCustomDiv").find(".btnReportConfig").length == 0 && !toolKit.ShowReport) {
                    $("#pageHeaderCustomDiv").append("<div class='btnReportConfigParent'><div class=\"btnReportConfig\" >All Reports</div></div>");
                    $(".btnReportConfig").unbind("click").click(function () {
                        toolKit.ShowReport = false;
                        toolKit.ExistingReports.CreateReportPopUp();
                    })
                }
                if (!toolKit.CurrentTabInfo.IsAdvanced) {
                    toolKit.reportType.createPopUp(false);
                }
                else {
                    toolKit.addDataSource.createDataSourcePopUp();
                    toolKit.addTables.createTablesdropDown();
                }
                //}
            }
            //}
        },

        TabsHandler: function (ListOfTabInfo, ListOfTabRelation) {
            $(".toolkitAddTabRelation").unbind("click").click(function (event) {
                if (ListOfTabRelation.length > 0)
                    toolKit.ExistingTabRelation.addTabRelation(ListOfTabInfo, ListOfTabRelation, false);
                else
                    toolKit.ExistingTabRelation.addTabRelation(ListOfTabInfo, ListOfTabRelation, true);
            });
        },

        raiseGridRender: function (eventType, gridid) {
            $find(gridid).get_GridInfo().Height = ($("#" + toolKit.identifier).height() - 110) + "px";
            var self = this;
            var RightFilterValues = {};
            RightFilterValues = toolKit.RightFilterValues;
            if (eventType == "") {
                for (var i = 0; i < $find(gridid).filter.filteredColumnInfo.length; i++) {
                    var values = [];

                    for (var j = 0; j < $find(gridid).filter.filteredColumnInfo[i].Values.length; j++) {
                        values.push($find(gridid).filter.filteredColumnInfo[i].Values[j]);
                    }
                    if (!$find(gridid).filter.filteredColumnInfo[i].ColumnName in RightFilterValues) {
                        RightFilterValues[$find(gridid).filter.filteredColumnInfo[i].ColumnName] = values;
                    }
                }
                self.bindRightFilter(RightFilterValues, gridid, $find(gridid).filter);
            }
        },

        bindContextMenu: function (listOfRelatedTabs, gridid) {
            var contextMenuObject = {};
            var self = this;
            if (listOfRelatedTabs.length > 0) {
                for (var item in listOfRelatedTabs) {
                    if (listOfRelatedTabs[item].IsTwoWay == false) {
                        if (toolKit.CurrentTabInfo.TabName == listOfRelatedTabs[item].PrimaryTabName) {
                            contextMenuObject[listOfRelatedTabs[item]["ContextMenuOption"].split('~~~')[1]] = { "name": listOfRelatedTabs[item]["ContextMenuOption"].split('~~~')[1] }
                            $("#" + gridid).contextMenu({
                                MenuItems: contextMenuObject,
                                contextOn: function (option) {
                                    self.LoadTabWithContextMenu(option);
                                },
                                callback: function (a, b) {
                                    self.ContextMenuHandler(a, b);
                                }
                            });
                        }
                    }
                    else {
                        if (toolKit.CurrentTabInfo.TabName == listOfRelatedTabs[item].PrimaryTabName) {
                            contextMenuObject[listOfRelatedTabs[item]["ContextMenuOption"].split('~~~')[1]] = { "name": listOfRelatedTabs[item]["ContextMenuOption"].split('~~~')[1] }
                            $("#" + gridid).contextMenu({ MenuItems: contextMenuObject, contextOn: function (option) {
                                self.LoadTabWithContextMenu(option);
                            },
                                callback: function (a, b) {
                                    self.ContextMenuHandler(a, b);
                                }
                            });
                        }
                        else if (toolKit.CurrentTabInfo.TabName == listOfRelatedTabs[item].SecondaryTabName) {
                            contextMenuObject[listOfRelatedTabs[item]["ContextMenuOption"].split('~~~')[0]] = { "name": listOfRelatedTabs[item]["ContextMenuOption"].split('~~~')[0] }
                            $("#" + gridid).contextMenu({ MenuItems: contextMenuObject,
                                contextOn: function (option) {
                                    self.LoadTabWithContextMenu(option);
                                },
                                callback: function (a, b) {
                                    self.ContextMenuHandler(a, b);
                                }
                            });
                        }
                    }
                }
            }
        },

        ContextMenuHandler: function (a, b) {
            toolKit.fromTab = toolKit.CurrentTabInfo.TabName;
            toolKit.IsFromContextMenu = true;
            var obj = {};
            var secTabName = ""
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            for (var i = 0; i < toolKit.tabRelation.length; i++) {
                if (!toolKit.tabRelation[i].IsTwoWay) {
                    if (toolKit.tabRelation[i].ContextMenuOption.split('~~~')[1] == b.items[a]["name"] || toolKit.tabRelation[i].ContextMenuOption.split('~~~')[1].indexOf(b.items[a]["name"]) != -1) {
                        for (var item = 0; item < toolKit.tabRelation[i].ColumnMappings.length; item++) {
                            var primcolName = toolKit.tabRelation[i].ColumnMappings[item].split("~~")[0];
                            var colName = toolKit.tabRelation[i].ColumnMappings[item].split("~~")[1];
                            //                        obj[colName] = toolKit.bindTabs.column.parent().find('div[columnname=\"' + primcolName + '\"]').html();
                            obj[colName] = toolKit.bindTabs.column.parent().attr(primcolName);
                            secTabName = toolKit.tabRelation[i].SecondryTabName;
                        }
                        break;
                    }
                }
                else {//from prim to second
                    if (toolKit.tabRelation[i].ContextMenuOption.split('~~~')[0] == b.items[a]["name"] || toolKit.tabRelation[i].ContextMenuOption.split('~~~')[0].indexOf(b.items[a]["name"]) != -1) {
                        for (var item = 0; item < toolKit.tabRelation[i].ColumnMappings.length; item++) {
                            var primcolName = toolKit.tabRelation[i].ColumnMappings[item].split("~~")[0];
                            var colName = toolKit.tabRelation[i].ColumnMappings[item].split("~~")[1];
                            //                        obj[colName] = toolKit.bindTabs.column.parent().find('div[columnname=\"' + primcolName + '\"]').html();
                            obj[colName] = toolKit.bindTabs.column.parent().attr(primcolName);
                            secTabName = toolKit.tabRelation[i].SecondryTabName;
                        }
                        break;
                    } //from second to prim
                    else if (toolKit.tabRelation[i].ContextMenuOption.split('~~~')[1] == b.items[a]["name"] || toolKit.tabRelation[i].ContextMenuOption.split('~~~')[1].indexOf(b.items[a]["name"]) != -1) {
                        for (var item = 0; item < toolKit.tabRelation[i].ColumnMappings.length; item++) {
                            var primcolName = toolKit.tabRelation[i].ColumnMappings[item].split("~~")[1];
                            var colName = toolKit.tabRelation[i].ColumnMappings[item].split("~~")[0];
                            //                        obj[colName] = toolKit.bindTabs.column.parent().find('div[columnname=\"' + primcolName + '\"]').html();
                            obj[colName] = toolKit.bindTabs.column.parent().attr(primcolName);
                            secTabName = toolKit.tabRelation[i].PrimaryTabName;
                        }
                        break
                    }
                }
            }
            var tabId = toolKit.utility.getTabId(secTabName)
            toolKit.filtereData = JSON.stringify(obj);

            toolKit.rightFilterData = "";
            if ($("#toolKitRightfilter").hasClass("iago-rightFilter")) {
                var object = toolKit.bindTabs.getrightFilterData($("#toolKitRightfilter").rightFilter('getFiltersJSON'));
                if (object.length > 0)
                    toolKit.rightFilterData = JSON.stringify(object);
            }
            else {
                toolKit.rightFilterData = [];
            }
            // toolKit.utility.loadSelectedTab(tabId);
            var tabIdUI = toolKit.utility.getUITabId(secTabName);
            $("#pageHeaderTabPanel").toolKittabs('setSelectedTab', tabIdUI, true);
            //  toolKit.bindTabs.tabClickHandler(tabId, $("#pageHeaderTabPanel_tab_content"))
            toolKit.IsContextMenu = true;
            $(".nav-tabs").find("#" + secTabName).click();

        },

        getrightFilterData: function (filterInfo) {
            var self = this;
            var globalParamInfo = [];
            var filterArray = [];
            for (var item in filterInfo) {
                if (item.endsWith("_GlobalFilter")) {
                    //globalFilter = true;
                    globalParamInfo.push({ "ParameterName": item.substr(0, item.indexOf("_GlobalFilter")), "ParameterValue": filterInfo[item][item.substr(0, item.indexOf("_GlobalFilter"))] });
                }
                else {
                    if (filterInfo[item].SelectedText != "") {
                        var selecteData = filterInfo[item].SelectedText.split(',');
                        filterArray.push({ "ColumnName": item.substr(0, item.indexOf("Filter")), "Values": selecteData, "latestOperation": "checkBox" });
                    }
                }
            }
            //toolKit.IsRightFilterApplied = true;
            //toolKit.rightFilterInfo = filterArray;
            var globalFilterDate = toolKit.utility.getSelectedDate();
            if (globalFilterDate != null) {
                globalParamInfo.push(globalFilterDate);
            }
            return globalParamInfo;
            //toolKit.bindTabs.GlobalFilterHandler(globalParamInfo);
        },


        LoadTabWithContextMenu: function (options) {
            var self = this;
            self.column = $(options.$item)
        },

        bindRightFilter: function (RightFilterValues, gridId, filter) {
            var self = this;
            var sourceList = [];
            var filterData = [];
            for (var item in RightFilterValues) {
                sourceList = [];

                for (var value in RightFilterValues[item]) {
                    sourceList.push({
                        Text: RightFilterValues[item][value],
                        Value: RightFilterValues[item][value],
                        Checked: self.CreateRightFilterCheckBox(RightFilterValues[item][value], filter, item)
                    });
                }
                filterData.push({ "eletype": "div", "title": item, "id": item + "Filter", "type": "iago:checkBox", "option-type": "JSON", "options": JSON.stringify({ "Type": "checkbox", "Layout": "default", "CheckboxInfo": sourceList }) })
            }
            $("#toolKitRightfilter").rightFilter({
                "Filters": filterData,
                getData: function (filterInfo) {
                    var filterArray = [];
                    for (var item in filterInfo) {
                        if (filterInfo[item].SelectedText != "") {
                            var columnName = toolKit.bindTabs.getRealColumnName(item.substr(0, item.indexOf("Filter")));
                            var selecteData = filterInfo[item].SelectedText.split(',');
                            filterArray.push({ "ColumnName": columnName, "Values": selecteData, "latestOperation": "checkBox" });
                        }
                    }
                    $find(gridId).filterData(filterArray);
                }
            })
        },
        CreateRightFilterCheckBox: function (checkedColumn, filter, columnName) {
            if (filter == null) {
                return true;
            }
            else {
                for (var i = 0; i < filter.filteredColumnInfo.length; i++) {
                    if (filter.filteredColumnInfo[i].ColumnName == columnName) {
                        if ($find("DefaultGridId").filter.filteredColumnInfo[i].IsDestructive == "t") {
                            if ($find("DefaultGridId").filter.filteredColumnInfo[i].Values.indexOf(checkedColumn) == -1) {
                                return true;
                            }
                            else {
                                return false;
                            }
                        }
                        else {
                            if ($find("DefaultGridId").filter.filteredColumnInfo[i].Values.indexOf(checkedColumn) != -1) {
                                return true;
                            }
                            else {
                                return false;
                            }
                        }
                    }
                }
            }
        },

        getRealColumnName: function (columnName) {
            for (var i = 0; i < toolKit.filterDetails.length; i++) {
                if (toolKit.filterDetails[i].FilterDisplayName == columnName && toolKit.filterDetails[i].IsRightFilter) {
                    return toolKit.filterDetails[i].FilterName;
                }
            }
        },

        createGlobalFilter: function (GlobalFilterList) {
            var self = this;

            var GlobalFilter = [];
            for (var i = 0; i < GlobalFilterList.length; i++) {
                if (!GlobalFilterList[i].IsRightFilter)
                    GlobalFilter.push(GlobalFilterList[i]);
            }
            var GlobalFilterValues = [];
            for (var item in GlobalFilter) {
                GlobalFilterValues = [];
                if (GlobalFilter[item].DataType == 0 || GlobalFilter[item].DataType == 2) {
                    var div = $('<div>');
                    div.addClass('globalFilterParentDiv');
                    div.attr('filterName', GlobalFilter[item].FilterName);
                    div.append("<div class='globalFilterLable'>" + GlobalFilter[item].FilterDisplayName + "</div>");
                    if (GlobalFilter[item].DefaultValue != null || GlobalFilter[item].DefaultValue != "")
                        div.append("<div class='globalFilterInput'>" + GlobalFilter[item].DefaultValue + "</div>");
                    else
                        div.append("<div class='globalFilterInput'>" + "Select One" + "</div>");
                    $("#GlobalFilterDiv").append(div);
                }
                else {
                    $("#toolKitDateControl").attr("columnname", GlobalFilter[item].FilterName)
                    var obj = {
                        onChange: function (startDate, endDate) {
                            if ($("#toolKitDateControl").data("iago-widget-dateControl").options.controlFormat == "DateRange")
                                self.ApplyDateFilter(startDate,endDate);
                            else
                                self.ApplyDateFilter(startDate);
                    },
                        startDate: moment(GlobalFilter[item].DefaultValue).format("MMM DD YYYY"),
                        format: 'MMM DD YYYY',
                        controlFormat: (GlobalFilter[item].IsRangeFilter ? 'DateRange' : 'AsOfDate')
                    }

                    if (GlobalFilter[item].IsRangeFilter) {
                        obj.startDate = moment(GlobalFilter[item].DefaultValue.split(',')[0]).format("MMM DD YYYY")
                        obj.endDate = moment(GlobalFilter[item].DefaultValue.split(',')[1]).format("MMM DD YYYY")
                    }
                    $("#toolKitDateControl").dateControl(obj)
                }
            }
            $("#GlobalFilterDiv").click(function (event) {
                if (event.target.className == "globalFilterInput") {
                    toolKit.bindTabs.showFIlterPopUp(event)
                }
                else if (event.target.className == "drpDwnGblFilterChild") {
                    toolKit.bindTabs.GlobalFilterHandler(event);
                }

            })
        },

        showFIlterPopUp: function (event) {
            filterValues = [];
            for (var i = 0; i < toolKit.filterDetails.length; i++) {
                if (toolKit.filterDetails[i].FilterDisplayName == $(event.target).prev().html() && !toolKit.filterDetails[i].IsRightFilter) {
                    filterDetails = toolKit.filterDetails[i];
                }
            }

            var div = $('<div>');
            div.addClass("drpDwnGblFilter");
            for (var i = 0; i < filterDetails.FilterValues.length; i++) {
                div.append("<div value='" + i + "' class='drpDwnGblFilterChild'>" + filterDetails.FilterValues[i] + "</div>");
            }
            if (filterDetails.DefaultValue != null || filterDetails.DefaultValue != "")
                div.append("<div value='" + filterDetails.FilterValues.length + "' class='drpDwnGblFilterChild'>" + filterDetails.DefaultValue + "</div>");
            $(event.target).closest(".globalFilterParentDiv").append(div);
        },

        ApplyDateFilter: function (startDate, endDate) {
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            var filterInfo = [];
            if(endDate != null)
                filterInfo.push({ "ParameterName": $("#toolKitDateControl").attr("columnname"), "ParameterValue": moment(startDate).format("YYYY-MM-DD") + "," + moment(endDate).format("YYYY-MM-DD") });
            else
                filterInfo.push({ "ParameterName": $("#toolKitDateControl").attr("columnname"), "ParameterValue": moment(startDate).format("YYYY-MM-DD") });
            var useName = (toolKit.ShowReport ? iago.user.userName : "All");
            $.ajax({
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                url: url + '/ApplyFIlter',
                data: JSON.stringify({ tabId: toolKit.CurrentTabInfo.tabId, dbIdentifier: toolKit.dbIdentifier, database: toolKit.DBName, identifier: toolKit.reportInfo.reportIdentifier, filterData: JSON.stringify(filterInfo), user: useName, calendarName : toolKit.reportInfo.CalendarName}),
                dataType: "json"
            }).then(function (responseText) {
                if (responseText.d != null) {
                    var gridinfo = $.parseJSON(responseText.d);
                    gridinfo.RaiseGridRenderComplete = "toolKit.addColumns.raiseGridRenderComplete";
                    gridinfo.taggingInfoID = "popUpDiv";
                    toolKit.tabContentContainer.empty();
                    toolKit.tabContentContainer.append("<div id=\"" + gridinfo.GridId + "\"></div>");
                    if (toolKit.tabRelation.length > 0)
                        toolKit.handlers.bindContextMenu(toolKit.tabRelation, gridinfo.GridId);
                    $("#" + gridinfo.taggingInfoID).empty();
                    if ($("#popUpDiv").length == 0)
                        toolKit.tabContentContainer.append("<div id ='popUpDiv'></div>");
                    else
                        $("#" + gridinfo.taggingInfoID).tagging('destroyPlugin');
                    $("#" + gridinfo.taggingInfoID).tagging(
                    {
                        gridInfo: gridinfo,
                        getAllTags: toolKit.ShowReport,
                        actionBy: (toolKit.ShowReport ? iago.user.userName : "All"),
                        serviceURL: toolKit.baseUrl + "/Resources/Services/TagManagement.svc",
                        ruleEditorServiceUrl: toolKit.baseUrl + "/Resources/Services/RADXRuleEditorService.svc",
                        isPersistantTextBoxRequired: true,
                        pageIdentifier: toolKit.reportInfo.reportIdentifier + toolKit.CurrentTabInfo.TabName,
                        tagHeading: 'Publish Report'
                    });
                }
                else {
                    toolKit.utility.alertPopUp(false, "Error occured while Loading Tab");
                }
            });
        },


        GlobalFilterHandler: function (filterInfo) {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $(".drpDwnGblFilter").remove();
            var useName = (toolKit.ShowReport ? iago.user.userName : "All");
            $.ajax({
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                url: url + '/ApplyFIlter',
                data: JSON.stringify({ tabId: toolKit.CurrentTabInfo.tabId, dbIdentifier: toolKit.dbIdentifier, database: toolKit.DBName, identifier: toolKit.reportInfo.reportIdentifier, filterData: JSON.stringify(filterInfo), user: useName, calendarName: toolKit.reportInfo.CalendarName }),
                dataType: "json"
            }).then(function (responseText) {
                if (responseText.d != null) {
                    var gridinfo = $.parseJSON(responseText.d);
                    gridinfo.RaiseGridRenderComplete = "toolKit.addColumns.raiseGridRenderComplete";
                    gridinfo.taggingInfoID = "popUpDiv";
                    toolKit.tabContentContainer.empty();
                    toolKit.tabContentContainer.append("<div id=\"" + gridinfo.GridId + "\"></div>");
                    if (toolKit.tabRelation.length > 0)
                        toolKit.handlers.bindContextMenu(toolKit.tabRelation, gridinfo.GridId);
                    $("#" + gridinfo.taggingInfoID).empty();
                    if ($("#popUpDiv").length == 0)
                        toolKit.tabContentContainer.append("<div id ='popUpDiv'></div>");
                    else
                        $("#" + gridinfo.taggingInfoID).tagging('destroyPlugin');
                    $("#" + gridinfo.taggingInfoID).tagging(
                    {
                        gridInfo: gridinfo,
                        getAllTags: toolKit.ShowReport,
                        actionBy: (toolKit.ShowReport ? iago.user.userName : "All"),
                        serviceURL: toolKit.baseUrl + "/Resources/Services/TagManagement.svc",
                        ruleEditorServiceUrl: toolKit.baseUrl + "/Resources/Services/RADXRuleEditorService.svc",
                        isPersistantTextBoxRequired: true,
                        pageIdentifier: toolKit.reportInfo.reportIdentifier + toolKit.CurrentTabInfo.TabName,
                        tagHeading: 'Publish Report'
                    });
                }
                else {
                    toolKit.utility.alertPopUp(false, "Error occured while Loading Tab");
                }
            });
        }
    }
});
        
           

            
            
        
    

$.extend(toolKit, {
    reportType: {
        createPopUp: function (isback) {
            var self = this;
            self.DataSetList = [];
            self.isBack = isback;
            self.AddedColumnList = [];
            self.selectedDataSet = [];
            self.SelectedAttributes = 0;
            self.selectedColumns = [];
            self.TotalAttributes = 0;
            if ($(".dataSourceHeader").length == 0) {
                $("#" + toolKit.identifier).prepend("<div class=\"dataSourceHeader\"></div>");
            }
            $(".dataSourceHeader").empty();
            $(".dataSourceHeader").append("<div class=\"columnDataSourceTab\"><div class=\"columnDBNameReport\"><span> DataBase : </span><span style=\"color:#2c2c2c;\">" + toolKit.dbIdentifier + "</span></div><div class='btnColumnFormatting'>Next : ColumnFormatting</div></div>");
            $(".dataSourceHeader").find(".columnDBNameReport").after("<div class=\"dataSetParentClass\"><div class=\"labelParent\"><span class=\"dataSetTypeLabel\">DataSet</span></div><div class=\"messageTypeParent\"><div id=\"messageTypeDropDown\" class=\"dataSetDropDown\"></div></div></div>");
            $(".btnColumnFormatting").unbind().click(function () {
                self.formattingColumnScreen();
            })
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + "/GetHtmlTemplate",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.addReportType.htm' })
            }).then(function (responseText) {
                $("#" + toolKit.tabContainer).height($("#" + toolKit.identifier).height() - $(".dataSourceHeader").height());
                toolKit.tabContentContainer.height($("#" + toolKit.identifier).height() - $(".dataSourceHeader").height());
                toolKit.tabContentContainer.css({ 'background-color': '#F2F5F8' });
                toolKit.tabContentContainer.empty();
                self.reportHTML = responseText.d;
                //                toolKit.tabContentContainer.append(responseText.d);

                if (self.isBack) {
                    $("#pageHeaderCustomDiv").empty();
                   
                    if ($(".btnGlobalParameter").length == 0) {
                        $("#pageHeaderCustomDiv").append("<div class='btnGlobalParameterParent'><div class='btnGlobalParameter'>Parameter</div></div>");
                    }
                    if (toolKit.CurrentTabInfo.IsTabSaved) {
                        if ($(".btnPreview").length == 0) {
                            $("#pageHeaderCustomDiv").append("<div class='btnPreviewParent'><div class='btnPreview'>Preview</div></div>");
                            $(".btnPreview").unbind("click").click(function () {
                                toolKit.addColumns.preview();
                            })
                        }
                    }
                    if (toolKit.utility.showMappedTabs()) {
                        if ($(".btnAddTabRelation").length == 0) {
                            $("#pageHeaderCustomDiv").append("<div class='btnAddTabRelationParent'><div class='btnAddTabRelation'>Navigation</div></div>");
                        }
                    }
                    if ($("#pageHeaderCustomDiv").find(".btnReportConfig").length == 0 && !toolKit.ShowReport) {
                        $("#pageHeaderCustomDiv").append("<div class='btnReportConfigParent'><div class=\"btnReportConfig\" >All Reports</div></div>");
                        $(".btnReportConfig").unbind("click").click(function () {
                            toolKit.ShowReport = false;
                            toolKit.ExistingReports.CreateReportPopUp();
                            //                            window.open("App_Dynamic_Resource/RGridWriterToolKit,com.ivo.rad.RGridWriterToolKit.Resources.RADGridExportToExcel.aspx?eventData=" + eventData.Split('~')[0], "_blank", "width=400,height=1,menubar=0,resizable=1");
                            //window.open("App_Dynamic_Resource/RGridWriterToolKit,com.ivo.rad.RGridWriterToolKit.Resources.RADGridExportToExcel.aspx?eventData=" + "z34miaww0xduj3gsl3usfu4n_excel.xlsx", "_blank", "width=400,height=1,menubar=0,resizable=1");
                        })
                    }
                }
                $(".addColumnsSelectedRegionReport").height($(".selectedRegionParent").height() - 35);
                self.createViewModel();
            });
        },

        //Navigate to Formatting Column Screen.
        formattingColumnScreen: function () {
            var self = this;
            toolKit.tabContentContainer.css({ 'background-color': '' });
            toolKit.addReportColumn.createPopUp(self.AddedColumnList);
        },

        //Creating View Model for the Attribute Type Screen.
        createViewModel: function () {
            var closure = this;
            function ReportViewModel() {
                var self = this;
                self.CategoryList = ko.observableArray();
                self.clickEvent = function (model, event) {
                    closure.clickEventHandler(event, model);
                }
                self.keyDownEvent = function (model, event) {
                    if (event.keyCode == 27) {
                        $(event.target).html("");
                        closure.keyDownEventHandler(event);
                    }
                    else {
                        closure.keyDownEventHandler(event);
                    }
                }
            }
            closure.viewModel = new ReportViewModel();
            if (closure.isBack) {
                closure.bindCategories(closure.categoriesList);
            }
            else {
                closure.getAttributesByCategory();
            }
        },

        //Key Down Event Handler for Search
        keyDownEventHandler: function (event) {
            if (event.target.className == "searchDivInput") {
                var searchText = $(event.target).html().toLowerCase();
                var self = this;
                var domCollection = $(".attributeNameParent");
                for (var i = 0; i < domCollection.length; i++) {
                    if ($(domCollection[i]).children().first().html().trim().toLowerCase().indexOf(searchText) > -1) {
                        $(domCollection[i]).removeClass("attributeNameParentHidden");
                    }
                    else {
                        $(domCollection[i]).addClass("attributeNameParentHidden");
                    }
                }
                var collection = $(".selectorClass");
                for (var j = 0; j < collection.length; j++) {
                    if ($(collection[j]).find(".attributeNameParent").length == $(collection[j]).find(".attributeNameParentHidden").length) {
                        self.viewModel.CategoryList()[j + 1].showCategoryDiv(false);
                    }
                    else {
                        self.viewModel.CategoryList()[j + 1].showCategoryDiv(true);
                    }
                }
            }
        },

        //Click Event Handler For View Model.
        clickEventHandler: function (event, model) {
            var self = this;
            if ($(event.target).closest(".attributeTypeParent").length > 0) {
                var domEle = $(".attributeTypeSelected");
                domEle.removeClass("attributeTypeSelected");
                $(".speechBubbleClass").addClass("sppechBubbleHidden");
                $(event.target).closest(".attributeType").find(".speechBubbleClass").removeClass("sppechBubbleHidden");
                $(event.target).closest(".attributeType").addClass("attributeTypeSelected");
                self.showCategoryAttributes();
            }
            else if ($(event.target).closest(".toggleToolKitDiv").length > 0) {
                self.showSelectedAllColumns(event);
            }
            else if ($(event.target).hasClass("attributeName")) {
                self.selectUnselectColumn(event);
            }
            else if ($(event.target).closest(".searchDiv").length > 0) {
                self.enableSearch(event);
                $(".searchDivInput").focus();
            }
        },

        //Enable Search.Called from Click Event Handler.
        enableSearch: function (event) {
            var self = this;
            if (!$(event.target).closest(".searchDiv").hasClass("searchDivBorder")) {
                $(".searchDivInput").attr("contenteditable", "true");
                $(event.target).closest(".searchDiv").addClass("searchDivBorder");
            }
        },

        //Show Category Attributes Div. Called from Click Event Handler.
        showCategoryAttributes: function () {
            var self = this;
            var className = $(event.target).closest(".attributeTypeParent").find(".categaoryName").html().replace(new RegExp('[ ]', 'gi'), '_').replace("&amp;", "&");
            var domCollection = $(".attributeNameParent").parent().parent();
            if (className == "All_Columns") {
                for (var i = 0; i < domCollection.length; i++) {
                    $(domCollection[i]).removeClass("attributeNameParentHidden");
                }
            }
            else {
                for (var i = 0; i < domCollection.length; i++) {
                    if (!$(domCollection[i]).hasClass(className)) {
                        $(domCollection[i]).addClass("attributeNameParentHidden");
                    }
                    else {
                        $(domCollection[i]).removeClass("attributeNameParentHidden");
                    }
                }
            }


        },

        //Show All or Selected Columns.Called from Click Event Handler.
        showSelectedAllColumns: function (event) {
            var self = this;
            if ($(event.target).hasClass("showAllColumn")) {
                if (!$(event.target).hasClass("selectedColumn")) {
                    $(event.target).addClass("selectedColumn");
                    $(event.target).next().removeClass("selectedColumn");
                    $(".attributeNameParent").removeClass("attributeNameHidden");
                }
            }
            else if ($(event.target).hasClass("showSelectedColumn")) {
                if (!$(event.target).hasClass("selectedColumn")) {
                    $(event.target).addClass("selectedColumn");
                    $(event.target).prev().removeClass("selectedColumn");
                    $(".checkedColumnHidden").closest(".attributeNameParent").addClass("attributeNameHidden");
                }
            }
        },

        //Select Unselect Column . Called from CLick Event Handler.
        selectUnselectColumn: function (event) {
            var self = this;
            if ($(event.target).closest(".attributeNameParent").find(".checkedColumnHidden").length > 0) {
                $(event.target).closest(".attributeNameParent").find(".checkedColumnHidden").removeClass("checkedColumnHidden");
                $(event.target).closest(".attributeNameParent").find(".attributeName").addClass("attributeNameSelected");
                var catName = $(event.target).closest(".attributeNameParent").find(".attributeName").attr("category");
                var attrName = $(event.target).closest(".attributeNameParent").find(".attributeName").html();
                var index = $(event.target).closest(".attributeNameParent").index();
                self.unselectedInArray(catName, index, true);
                self.SelectedAttributes++;
                var element = $(".attributeType").find("div:contains('All Columns')").next()
                var domEle = $(".attributeType").find("div:contains('" + catName + "')").next();
                element.html(self.SelectedAttributes + "/" + self.TotalAttributes);
                domEle.html(self.getCount(catName, attrName, true));
                if (self.selectedDataSet.indexOf($(event.target).closest(".attributeNameParent").find(".attributeName").attr("dataSet")) == -1) {
                    if ($(event.target).closest(".attributeNameParent").find(".attributeName").attr("dataSet") != null && $(event.target).closest(".attributeNameParent").find(".attributeName").attr("dataSet") != "") {
                        self.selectedDataSet.push($(event.target).closest(".attributeNameParent").find(".attributeName").attr("dataSet"));
                    }
                }
                self.AddedColumnList.push({
                    "ColumnName": $(event.target).closest(".attributeNameParent").find(".attributeName").attr("attributerealname"),
                    "DisplayColumnName": $(event.target).closest(".attributeNameParent").find(".attributeName").html(),
                    "DataType": $(event.target).closest(".attributeNameParent").find(".dataTypeClass").html(),
                    "Unit": "None",
                    "NegativeValue": "Default",
                    "Prefix": "",
                    "IsTag": false,
                    "DecimalPlaces": "2",
                    "FormatterDataType": "None",
                    "GroupByOrder": 0,
                    "GroupByOperation": "None",
                    "GroupAdvanceOperation": "None",
                    "IsColumnHidden": false,
                    "AssemblyName": "",
                    "LinkColumn": null,
                    "ClassName": "",
                    "IsRightFilter": false,
                    "IsRangeFilter": false,
                    "TableName": $(event.target).closest(".attributeNameParent").find(".attributeName").attr("tableName"),
                    "Dataset": $(event.target).closest(".attributeNameParent").find(".attributeName").attr("dataSet"),
                    "CategoryName": $(event.target).closest(".attributeNameParent").find(".attributeName").attr("category"),
                    "mappedGlobalParamVal": ""
                });
            }
            else {
                $(event.target).closest(".attributeNameParent").find(".checkedColumnDiv").addClass("checkedColumnHidden");
                $(event.target).closest(".attributeNameParent").find(".attributeName").removeClass("attributeNameSelected");
                var catName = $(event.target).closest(".attributeNameParent").find(".attributeName").attr("category");
                var attrName = $(event.target).closest(".attributeNameParent").find(".attributeName").html();
                var index = $(event.target).closest(".attributeNameParent").index();
                self.unselectedInArray(catName, index, false);
                self.SelectedAttributes--;
                var element = $(".attributeType").find("div:contains('All Columns')").next()
                var domEle = $(".attributeType").find("div:contains('" + catName + "')").next();
                element.html(self.SelectedAttributes + "/" + self.TotalAttributes);
                domEle.html(self.getCount(catName, attrName, false));
                self.removeFromAddedList(attrName)
            }
        },

        removeFromAddedList: function (columnName) {
            var self = this;
            var index = 0;
            for (var i = 0; i < self.AddedColumnList.length; i++) {
                if (self.AddedColumnList[i].DisplayColumnName == columnName) {
                    index = i;
                }
            }
            self.AddedColumnList.splice(index, 1);
            index = -1
            if (toolKit.CurrentTabInfo.ColumnInfo.length > 0) {
                for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                    if (toolKit.CurrentTabInfo.ColumnInfo[i].DisplayColumnName == columnName) {
                        index = i;
                    }
                }
            }
            if (index > -1)
                toolKit.CurrentTabInfo.ColumnInfo.splice(index, 1);
        },

        getCount: function (catName, attrName, selected) {
            var self = this;
            var totalColumns = 0;
            var selectedColumns = 0;
            for (var i = 0; i < self.viewModel.CategoryList().length; i++) {
                if (self.viewModel.CategoryList()[i].CategoryName == catName) {
                    if (self.viewModel.CategoryList()[i].AttributesList != null) {
                        for (var j = 0; j < self.viewModel.CategoryList()[i].AttributesList.length; j++) {
                            totalColumns++;
                            if (self.viewModel.CategoryList()[i].AttributesList[j].AttributeName == attrName) {
                                if (selected) {
                                    self.viewModel.CategoryList()[i].AttributesList[j].IsSelected = true;
                                    selectedColumns++;
                                }
                                else {
                                    self.viewModel.CategoryList()[i].AttributesList[j].IsSelected = false;
                                }
                            }
                            else if (self.viewModel.CategoryList()[i].AttributesList[j].IsSelected || self.checkIfTabSelected(self.viewModel.CategoryList()[i].AttributesList[j].AttributeRealName, self.viewModel.CategoryList()[i].AttributesList[j].TableName)) {
                                selectedColumns++;
                            }

                        }
                    }
                }
            }
            return selectedColumns + "/" + totalColumns;
        },

        unselectedInArray: function (categoryName, index, flag) {
            var self = this;
            for (var i = 0; i < self.categoriesList.length; i++) {
                if (self.categoriesList[i].CategoryName == categoryName) {
                    if (flag) {
                        self.categoriesList[i].Attributes[index].IsSelected = true;
                    }
                    else {
                        self.categoriesList[i].Attributes[index].IsSelected = false;
                    }
                }
            }
        },

        //Get Attributes By Category in Create POP Up Method
        getAttributesByCategory: function () {
            var self = this;
            $.ajax({
                url: toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc/GetAttributesByCategory",
                type: "POST",
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify({ selectedCategory: toolKit.ReportType, AssemblyName: toolKit.AssemblyName, className: toolKit.ClassName})
            }).then(function (reposnseText) {
                if (reposnseText.d != null) {
                    self.categoriesList = $.parseJSON(reposnseText.d);
                    //                else {
                    //                    //self.categoriesList = [];
                    //                    self.getReportData();
                    //                }
                    self.bindCategories(self.categoriesList);
                }
                else {
                    toolKit.utility.alertPopUp(false, "Error occurred while loading dictionary");
                }
            })
        },

        getReportData: function (categoryList) {
            var self = this;
            for (var i = 0; i < categoryList.length; i++) {
                if (columnExits()) {
                    categoryList[i].Attributes.IsSelected = true;
                    self.categoriesList.push(categoryList[i]);
                }
            }
        },

        columnExits: function (columnName) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].ColumnName == columnName)
                    return true;
            }
            return false;
        },

        //Get Selected Column Count to show it in Category Headers.Called from View Model Binding.
        getSelectedColumnCount: function (index) {
            var self = this;
            var selectedColumns = 0;
            var totalColumns = 0;
            if (self.viewModel.CategoryList()[index].CategoryName == "All Columns") {
                for (var i = 0; i < self.viewModel.CategoryList().length; i++) {
                    if (self.viewModel.CategoryList()[i].AttributesList != null) {
                        for (var j = 0; j < self.viewModel.CategoryList()[i].AttributesList.length; j++) {
                            totalColumns++;
                            self.TotalAttributes++
                            if (self.viewModel.CategoryList()[i].AttributesList[j].IsSelected || self.checkIfTabSelected(self.viewModel.CategoryList()[i].AttributesList[j].AttributeRealName, self.viewModel.CategoryList()[i].AttributesList[j].TableName)) {
                                selectedColumns++;
                                self.SelectedAttributes++;
                            }
                        }
                    }
                }
            }
            else {
                for (var i = 0; i < self.viewModel.CategoryList()[index].AttributesList.length; i++) {
                    totalColumns++;

                    if (self.viewModel.CategoryList()[index].AttributesList[i].IsSelected || self.checkIfTabSelected(self.viewModel.CategoryList()[index].AttributesList[i].AttributeRealName, self.viewModel.CategoryList()[index].AttributesList[i].TableName)) {
                        selectedColumns++;
                    }
                }
            }
            return selectedColumns + "/" + totalColumns;
        },

        //Adding Class Name for Each parent div of category attributes list. Called from Apply Bindings. 
        addClass: function (index) {
            var self = this;
            return self.viewModel.CategoryList()[index].CategoryName.replace(new RegExp('[ ]', 'gi'), '_');
        },

        addHiddenClass: function (attrIndex, catName) {
            var self = this;
            for (var i = 0; i < self.viewModel.CategoryList().length; i++) {
                if (self.viewModel.CategoryList()[i].CategoryName == catName) {
                    if (self.viewModel.CategoryList()[i].AttributesList[attrIndex].IsSelected || self.checkIfTabSelected(self.viewModel.CategoryList()[i].AttributesList[attrIndex].AttributeRealName, self.viewModel.CategoryList()[i].AttributesList[attrIndex].TableName)) {
                        return "";
                    }
                    else {
                        return "checkedColumnHidden";
                    }
                }
            }
        },

        addSelectedClass: function (attrIndex, catName) {
            var self = this;
            for (var i = 0; i < self.viewModel.CategoryList().length; i++) {
                if (self.viewModel.CategoryList()[i].CategoryName == catName) {
                    if (self.viewModel.CategoryList()[i].AttributesList[attrIndex].IsSelected) {
                        //self.addtoListColumns(self.viewModel.CategoryList()[i].AttributesList[attrIndex])
                        return "attributeNameSelected";
                    }
                    else if (self.checkIfTabSelected(self.viewModel.CategoryList()[i].AttributesList[attrIndex].AttributeRealName, self.viewModel.CategoryList()[i].AttributesList[attrIndex].TableName)) {
                        //self.addtoColumnList(self.viewModel.CategoryList()[i].AttributesList[attrIndex].AttributeRealName);
                        return "attributeNameSelected";
                    }
                    else {
                        return "";
                    }
                }
            }
        },

        addtoListColumns: function (obj) {
            var self = this;
            if (self.selectedDataSet.indexOf(obj.Dataset) == -1) {
                if (obj.Dataset != null && obj.Dataset != "") {
                    self.selectedDataSet.push(obj.Dataset);
                }
            }
            self.AddedColumnList.push({
                "ColumnName": obj.AttributeRealName,
                "DisplayColumnName": obj.AttributeName,
                "DataType": self.getDataType(obj.DataType),
                "Unit": "None",
                "NegativeValue": "Default",
                "Prefix": "",
                "DecimalPlaces": "2",
                "FormatterDataType": "None",
                "GroupByOrder": 0,
                "GroupByOperation": "None",
                "GroupAdvanceOperation": "None",
                "IsColumnHidden": false,
                "AssemblyName": "",
                "LinkColumn": "",
                "ClassName": "",
                "IsRightFilter": false,
                "IsRangeFilter": false,
                "TableName": obj.TableName,
                "Dataset": obj.Dataset,
                "CategoryName": obj.CategoryName,
                "mappedGlobalParamVal": ""
            });
        },

        addtoColumnList: function (columnName) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (self.selectedDataSet.indexOf(toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.Dataset) == -1) {
                    if (toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.Dataset != null && toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.Dataset != "") {
                        self.selectedDataSet.push(toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.Dataset);
                    }
                }
                if (columnName == toolKit.CurrentTabInfo.ColumnInfo[i].ColumnName) {
                    self.AddedColumnList.push({
                        "ColumnName": toolKit.CurrentTabInfo.ColumnInfo[i].ColumnName,
                        "DisplayColumnName": toolKit.CurrentTabInfo.ColumnInfo[i].DisplayColumnName,
                        "DataType": self.getDataType(toolKit.CurrentTabInfo.ColumnInfo[i].DataType),
                        "Unit": "None",
                        "NegativeValue": "Default",
                        "Prefix": "",
                        "DecimalPlaces": "2",
                        "FormatterDataType": "None",
                        "AssemblyName": "",
                        "ClassName": "",
                        "LinkColumn": toolKit.CurrentTabInfo.ColumnInfo[i].LinkColumn,
                        "GroupByOrder": toolKit.CurrentTabInfo.ColumnInfo[i].GroupByOrder,
                        "GroupByOperation": toolKit.CurrentTabInfo.ColumnInfo[i].GroupByOperation,
                        "GroupAdvanceOperation": toolKit.CurrentTabInfo.ColumnInfo[i].GroupAdvanceOperation,
                        "IsColumnHidden": toolKit.CurrentTabInfo.ColumnInfo[i].IsHidden,
                        "IsRightFilter": toolKit.CurrentTabInfo.ColumnInfo[i].IsRightFilter,
                        "IsRangeFilter": toolKit.CurrentTabInfo.ColumnInfo[i].IsRangeFilter,
                        "mappedGlobalParamVal": toolKit.CurrentTabInfo.ColumnInfo[i].mappedGlobalParam,
                        "TableName": toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.TableName,
                        "Dataset": toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.Dataset,
                        "CategoryName": toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.CategoryName
                    });
                    if (toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter != null) {
                        self.AddedColumnList[i].Unit = toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.Unit;
                        self.AddedColumnList[i].NegativeValue = toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.NegativeValue;
                        self.AddedColumnList[i].Prefix = toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.Prefix;
                        self.AddedColumnList[i].DecimalPlaces = toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.DecimalPlaces;
                        self.AddedColumnList[i].FormatterDataType = toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.DataType;
                        self.AddedColumnList[i].AssemblyName = toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.AssemblyName;
                        self.AddedColumnList[i].ClassName = toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.ClassName;
                    }
                }
            }
        },

        checkIfTabSelected: function (attrName, tableName) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].ColumnName == attrName) {
                    if (toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.TableName == tableName)
                        return true;
                }
            }
            return false;
        },

        getOperation: function (attrName, tableName) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].DisplayColumnName == attrName) {
                    if (toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.TableName == tableName) {
                        return self.getOperationText(toolKit.CurrentTabInfo.ColumnInfo[i].GroupByOperation)
                    }
                }
            }
            return "None";
        },

        getOperationText: function (operation) {
            if (Number.isNaN(parseInt(operation))) {
                return operation;
            }
            else {
                switch (parseInt(operation)) {
                    case 0:
                        return "None";
                    case 1:
                        return "MIN";
                    case 2:
                        return "MAX";

                    case 3:
                        return "SUM";
                    case 4:
                        return "AVERAGE";
                    case 5:
                        return "COUNT";
                    default:
                        return "None";
                }
            }
        },

        getAdvanceOperation: function (attrName, tableName) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].DisplayColumnName == attrName) {
                    if (toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.TableName == tableName) {
                        return self.getAdvanceOperationText(toolKit.CurrentTabInfo.ColumnInfo[i].GroupAdvanceOperation)
                    }
                }
            }
            return "None";
        },

        getRightFilter: function (attrName, tableName) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].DisplayColumnName == attrName) {
                    if (toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.TableName == tableName) {
                        return toolKit.CurrentTabInfo.ColumnInfo[i].IsRightFilter;
                    }
                }
            }
            return false;
        },

        getHiddenColumn: function (attrName, tableName) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].DisplayColumnName == attrName) {
                    if (toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.TableName == tableName) {
                        return toolKit.CurrentTabInfo.ColumnInfo[i].IsHidden;
                    }
                }
            }
            return false;
        },

        getTabFilter: function (attrName, tableName) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].DisplayColumnName == attrName) {
                    if (toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.TableName == tableName) {
                        return toolKit.CurrentTabInfo.ColumnInfo[i].mappedGlobalParam;
                    }
                }
            }
            return "";
        },

        getGroupByOrder: function (attrName, tableName) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].DisplayColumnName == attrName) {
                    if (toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.TableName == tableName) {
                        return toolKit.CurrentTabInfo.ColumnInfo[i].GroupByOrder;
                    }
                }
            }
            return 0;
        },

        getFormatter: function (attrName, tableName) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].DisplayColumnName == attrName) {
                    if (toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.TableName == tableName) {
                        if (toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter != null) {
                            if (toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.DataType != 0) {
                                return "Normal|" + toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.Unit + "|" + toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.NegativeValue + "|"
                                    + toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.DecimalPlaces + "|" + toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.Prefix + '|' + toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.DataType;
                            }
                            else if (toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.AssemblyName != "") {
                                return "Advanced|" + toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.AssemblyName + "|" + toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.ClassName;
                            }
                            else
                                return ""
                        }
                    }
                }
            }
            return "";
        },

        getAdvanceOperationText: function (operation) {
            if (Number.isNaN(parseInt(operation))) {
                return operation;
            }
            else {
                switch (parseInt(operation)) {
                    case 0:
                        return "None";
                    case 1:
                        return "RunningTotal";
                    case 2:
                        return "Percentage";
                    default:
                        return "None";
                }
            }
        },


        getDataType: function (dataType) {
            var self = this;
            if (Number.isNaN(parseInt(dataType))) {
                return dataType;
            }
            else {
                switch (dataType) {
                    case 0:
                        return "String";
                    case 1:
                        return "Date";
                    case 2:
                        return "Numeric";
                }
            }
        },

        getToolTipData: function (element) {
            var self = this;
            var div = $("<div>");
            div.addClass("toolTipClass");
            div.append("<table><tbody></tbody></table>");
            div.find('tbody').append("<tr><td> DataSet </td><td> : " + element.find(".attributeName").attr("dataSet") + "</td></tr>");
            //            div.find('tbody').append("<tr><td> Table Name </td><td> : " + element.find(".attributeName").attr("tablename") + "</td></tr>");
            div.find('tbody').append("<tr><td> Column Name </td><td> : " + element.find(".attributeName").html() + "</td></tr>");
            div.find('tbody').append("<tr><td> Data Type </td><td> : " + element.find(".dataTypeClass").html() + "</td></tr>");
            //            div.find('tbody').append("<div> ColumnName : " + element.find(".attributeName").html() + "</div>");
            //            div.find('tbody').append("<div> Table Name : " + element.find(".dataTypeClass").html() + "</div>");
            return div;
        },

        bindDataSetFilter: function () {
            var self = this;
            var sourceList = [];
            sourceList.push({ 'id': "Select All", 'text': "Select All" });
            for (var i = 0; i < self.DataSetList.length; i++) {
                sourceList.push({ 'id': self.DataSetList[i], 'text': self.DataSetList[i] });
            }
            $("#messageTypeDropDown").inlineFilter({
                filterInfo: {
                    filterPhrase: "{0}",
                    bindingInfo: [{
                        identifier: "messageTypeDropDown",
                        data: sourceList,
                        multiple: false,
                        maxResultsToShow: 1,
                        placeholder: "Select Dataset",
                        class: 'inlineFilterCSS'
                    }]
                }
                    , selectHandler: toolKit.reportType.GetSelectedDataSet
            })
        },

        GetSelectedDataSet: function () {
            var self = toolKit.reportType;
            var dataSet = $("#messageTypeDropDown").inlineFilter("getCurrentStateObjects")["messageTypeDropDown"][0].text;
            if (dataSet == "Select All") {
                $(".attributeNameParent").removeClass("attributeNameHidden");
            }
            else {
                $(".attributeNameParent").removeClass("attributeNameHidden");
                $(".attributeNameParent").find(".attributeName[dataSet!='" + dataSet + "']").parent().addClass("attributeNameHidden");
            }
        },

        //Binding View Model.
        bindCategories: function (categoryList) {
            var self = this;
            self.viewModel.CategoryList().push({ CategoryName: "All Columns",
                getColumnCount: function (index) {
                    return self.getSelectedColumnCount(index());
                },
                addClass: function (index) {
                    return self.addClass(index());
                },
                showCategoryDiv: ko.observable(true)
            });

            for (var i = 0; i < categoryList.length; i++) {
                self.viewModel.CategoryList().push({ CategoryName: categoryList[i].CategoryName,
                    getColumnCount: function (index) {
                        return self.getSelectedColumnCount(index());
                    },
                    addClass: function (index) {
                        return self.addClass(index());
                    },
                    showCategoryDiv: ko.observable(true)
                });
                for (var j = 0; j < categoryList[i].Attributes.length; j++) {
                    if (self.DataSetList.indexOf(categoryList[i].Attributes[j].Dataset) == -1) {
                        self.DataSetList.push(categoryList[i].Attributes[j].Dataset);
                    }
                    if (self.viewModel.CategoryList()[i + 1].AttributesList != null) {

                        self.viewModel.CategoryList()[i + 1].AttributesList.push({ AttributeRealName: categoryList[i].Attributes[j].AttributeRealName, TableName: categoryList[i].Attributes[j].TableName, CategoryName: categoryList[i].CategoryName, AttributeDescription: categoryList[i].Attributes[j].AttributeDescription, AttributeName: categoryList[i].Attributes[j].AttributeName, IsSelected: categoryList[i].Attributes[j].IsSelected,
                            DataType: categoryList[i].Attributes[j].DataType,
                            Dataset: categoryList[i].Attributes[j].Dataset,
                            addHiddenClass: function (catName, index) {
                                return self.addHiddenClass(index(), catName);
                            },
                            addSelectedClass: function (catName, index) {
                                return self.addSelectedClass(index(), catName);
                            },
                            getDataType: function (dataType) {
                                return self.getDataType(dataType);
                            },
                            getOperationTypeText: function (model) {
                                return self.getOperation(model.AttributeName, model.TableName);
                            },
                            getAdvanceOperationTypeText: function (model) {
                                return self.getAdvanceOperation(model.AttributeName, model.TableName);
                            },
                            getRightFilterFlag: function (model) {
                                return self.getRightFilter(model.AttributeName, model.TableName);
                            },
                            getHiddenColumnFlag: function (model) {
                                return self.getHiddenColumn(model.AttributeName, model.TableName);
                            },
                            getTabFilter: function (model) {
                                return self.getTabFilter(model.AttributeName, model.TableName);
                            },
                            getGroupByOrder: function (model) {
                                return self.getGroupByOrder(model.AttributeName, model.TableName);
                            },
                            getFormatter: function (model) {
                                return self.getFormatter(model.AttributeName, model.TableName);
                            }
                        });
                    }
                    else {
                        self.viewModel.CategoryList()[i + 1].AttributesList = [];
                        self.viewModel.CategoryList()[i + 1].AttributesList.push({ AttributeRealName: categoryList[i].Attributes[j].AttributeRealName, TableName: categoryList[i].Attributes[j].TableName, CategoryName: categoryList[i].CategoryName, AttributeDescription: categoryList[i].Attributes[j].AttributeDescription, AttributeName: categoryList[i].Attributes[j].AttributeName, IsSelected: categoryList[i].Attributes[j].IsSelected,
                            DataType: categoryList[i].Attributes[j].DataType,
                            Dataset: categoryList[i].Attributes[j].Dataset,
                            addHiddenClass: function (catName, index) {
                                return self.addHiddenClass(index(), catName);
                            },
                            addSelectedClass: function (catName, index) {
                                return self.addSelectedClass(index(), catName);
                            },
                            getDataType: function (dataType) {
                                return self.getDataType(dataType);
                            },
                            getOperationTypeText: function (model) {
                                return self.getOperation(model.AttributeName, model.TableName);
                            },
                            getAdvanceOperationTypeText: function (model) {
                                return self.getAdvanceOperation(model.AttributeName, model.TableName);
                            },
                            getRightFilterFlag: function (model) {
                                return self.getRightFilter(model.AttributeName, model.TableName);
                            },
                            getHiddenColumnFlag: function (model) {
                                return self.getHiddenColumn(model.AttributeName, model.TableName);
                            },
                            getTabFilter: function (model) {
                                return self.getTabFilter(model.AttributeName, model.TableName);
                            },
                            getGroupByOrder: function (model) {
                                return self.getGroupByOrder(model.AttributeName, model.TableName);
                            },
                            getFormatter: function (model) {
                                return self.getFormatter(model.AttributeName, model.TableName);
                            }
                        });
                    }
                }
            }
            toolKit.tabContentContainer.append(self.reportHTML);
            ko.applyBindings(self.viewModel, $("#addReportType")[0]);
            self.bindDataSetFilter();
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (self.selectedDataSet.indexOf(toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.Dataset) == -1) {
                    if (toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.Dataset != null && toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.Dataset != "") {
                        self.selectedDataSet.push(toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.Dataset);
                    }
                }
                self.AddedColumnList.push({
                    "ColumnName": toolKit.CurrentTabInfo.ColumnInfo[i].ColumnName,
                    "DisplayColumnName": toolKit.CurrentTabInfo.ColumnInfo[i].DisplayColumnName,
                    "DataType": self.getDataType(toolKit.CurrentTabInfo.ColumnInfo[i].DataType),
                    "Unit": "None",
                    "NegativeValue": "Default",
                    "Prefix": "",
                    "DecimalPlaces": "2",
                    "FormatterDataType": "None",
                    "IsTag": toolKit.CurrentTabInfo.ColumnInfo[i].IsTag,
                    "AssemblyName": "",
                    "ClassName": "",
                    "LinkColumn": toolKit.CurrentTabInfo.ColumnInfo[i].LinkColumn,
                    "GroupByOrder": toolKit.CurrentTabInfo.ColumnInfo[i].GroupByOrder,
                    "GroupByOperation": toolKit.CurrentTabInfo.ColumnInfo[i].GroupByOperation,
                    "GroupAdvanceOperation": toolKit.CurrentTabInfo.ColumnInfo[i].GroupAdvanceOperation,
                    "IsColumnHidden": toolKit.CurrentTabInfo.ColumnInfo[i].IsHidden,
                    "IsRightFilter": toolKit.CurrentTabInfo.ColumnInfo[i].IsRightFilter,
                    "IsRangeFilter": toolKit.CurrentTabInfo.ColumnInfo[i].IsRangeFilter,
                    "mappedGlobalParamVal": toolKit.CurrentTabInfo.ColumnInfo[i].mappedGlobalParam,
                    "TableName": toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.TableName,
                    "Dataset": (toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.Dataset == null ? "" : toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.Dataset),
                    "CategoryName": toolKit.CurrentTabInfo.ColumnInfo[i].AttributeDetail.CategoryName
                });
                if (toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter != null) {
                    self.AddedColumnList[i].Unit = toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.Unit;
                    self.AddedColumnList[i].NegativeValue = toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.NegativeValue;
                    self.AddedColumnList[i].Prefix = toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.Prefix;
                    self.AddedColumnList[i].DecimalPlaces = toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.DecimalPlaces;
                    self.AddedColumnList[i].FormatterDataType = toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.DataType;
                    self.AddedColumnList[i].AssemblyName = toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.AssemblyName;
                    self.AddedColumnList[i].ClassName = toolKit.CurrentTabInfo.ColumnInfo[i].CustomFormatter.ClassName;
                }
            }
            var height = $(document).height() - ($(".searchDivParent").height() + $(".CategoryNamesHeader").height() + $(".dataSourceHeader").height() + 34 + $("#header-div").height());
            $(".categoryBodyParent").height(height);
            $(".categoryBodyParent").find(".attributeNameParent").each(function () {
                $(this).qtip({
                    content: self.getToolTipData($(this)),
                    position: { my: 'top center', at: 'bottom center', target: $(this) },
                    show: 'mouseover',
                    hide: 'mouseout',
                    style: {
                        classes: 'qtip-light qtipCustom'
                    }
                });
            });
        }
    }
});

$.extend(toolKit, {
    addReportColumn: {
        createPopUp: function (columnList) {
            this.columnList = columnList;
            var self = this;
            self.doChange = true;

            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + "/GetHtmlTemplate",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.addReportColumn.htm' })
            }).then(function (responseText) {
                $(".btnColumnFormatting").remove();

                $(".dataSourceHeader").children().first().removeClass("columnDataSourceTab");
                $(".dataSourceHeader").children().first().addClass("columnDataSourceRpt");
                $(".dataSourceHeader").find(".dataSetParentClass").remove();
                $(".dataSourceHeader").find(".columnDBNameReport").after("<div class=\"dataSetParentClass\"><div class=\"labelParent\"><span class=\"dataSetTypeLabel\">DataSet</span></div><div class=\"messageTypeParent\"><div id=\"messageTypeDropDown\" class=\"dataSetDropDown dataSetLabel\"></div></div></div>");
                if (toolKit.reportType.selectedDataSet.length > 1) {
                    $("#messageTypeDropDown").html("All");
                }
                else {
                    $("#messageTypeDropDown").html(toolKit.reportType.selectedDataSet[0]);
                }
                $(".dataSourceHeader").find(".columnDBNameReport").addClass("fullWidth");
                $(".columnDataSourceRpt").append("<div class='btnConfigColumn'>Back : Configure Columns</div>");
                $("#pageHeaderCustomDiv").empty();
                
                if ($(".btnGlobalParameter").length == 0) {
                    $("#pageHeaderCustomDiv").append("<div class='btnGlobalParameterParent'><div class='btnGlobalParameter'>Parameter</div></div>");
                }
                if (toolKit.utility.showMappedTabs()) {
                    if ($(".btnAddTabRelation").length == 0) {
                        $("#pageHeaderCustomDiv").append("<div class='btnAddTabRelationParent'><div class='btnAddTabRelation'>Navigation</div></div>");
                    }
                }
                if (toolKit.CurrentTabInfo.IsTabSaved) {
                    if ($(".btnPreview").length == 0) {
                        $("#pageHeaderCustomDiv").append("<div class='btnPreviewParent'><div class='btnPreview'>Preview</div></div>");
                        $(".btnPreview").unbind("click").click(function () {
                            toolKit.addColumns.preview();
                        })
                    }
                }
                if ($("#pageHeaderCustomDiv").find(".btnSaveConfigColumn").length == 0) {
                    $("#pageHeaderCustomDiv").append("<div class='btnSaveConfigColumnParent'><div class=\"btnSaveConfigColumn\" >Save Tab</div></div>");
                    $(".btnSaveConfigColumn").unbind("click").click(function () {
                        self.saveConfigColumn();
                    })
                }
                if ($("#pageHeaderCustomDiv").find(".btnReportConfig").length == 0 && !toolKit.ShowReport) {
                    $("#pageHeaderCustomDiv").append("<div class='btnReportConfigParent'><div class=\"btnReportConfig\" >All Reports</div></div>");
                    $(".btnReportConfig").unbind("click").click(function () {
                        toolKit.ShowReport = false;
                        toolKit.ExistingReports.CreateReportPopUp();
                    })
                }
                toolKit.tabContentContainer.empty();
                toolKit.tabContentContainer.append(responseText.d);
                //toolKit.utility.applytaggingPrivilege();
                if (toolKit.RequireTagLink)
                    $(".btnAddLinkColumn").addClass("hiddenClass");
                $(".selectedRegionParentReport").height(toolKit.tabContentContainer.height());
                $(".btnConfigColumn").unbind("click").click(function () {
                    self.backtoReportConfig();
                })
                //                $(".btnSaveConfigColumn").unbind("click").click(function () {
                //                    self.saveConfigColumn();
                //                })
                self.createViewModel();
                self.bindColumnList();
            });
        },

        changeSpinner: function (event) {
            var self = this;
            $(".addColumnsSelectedRegionReport").find("[groupApplied]").removeAttr("groupApplied");
            if ($(event.target).closest(".headerSectionGroupSpinner").children().last().css("display") == "inline-block") {
                $(event.target).closest(".headerSectionGroupSpinner").children().last().css({ "display": "none" })
            }
            $(event.target).closest(".headerSectionGroupSpinner").next().attr({ "groupapplied": true });
            if (self.doChange) {
                if ($(".headerSectionAggregation").attr("spinnerinitial") != undefined) {
                    $(".headerSectionAggregation").removeAttr("spinnerinitial");
                    var domCollection = $(".selectedRegionBody").find('tr.columnBodySection');
                    for (var i = 0; i < domCollection.length; i++) {
                        if ($(domCollection[i]).find(".headerSectionOperationDropDown").attr("groupapplied") == undefined) {
                            if ($(domCollection[i]).find(".dataTypeColumn").html().trim().toLowerCase() != "string" && $(domCollection[i]).find(".dataTypeColumn").html().trim().toLowerCase() != "date") {
                                $($(domCollection[i]).find(".headerSectionOperationDropDown")).find(".OperationDropdownRpt").text("SUM");
                            }
                        }
                    }
                }
            }
        },

        saveConfigColumn: function () {
            var self = this;
            $("#globalFilterPopUp").remove();
            var columnInfo = toolKit.utility.getReportColumnInfo();
            toolKit.CurrentTabInfo.ColumnInfo = columnInfo;
            toolKit.newTabAdded = false;
            toolKit.CurrentTabInfo.dbIdentifier = toolKit.dbIdentifier;
            toolKit.utility.saveNPreviewTabInfo(JSON.stringify(toolKit.CurrentTabInfo));
        },

        preview: function () {
            var self = this;
            $(".dataSourceHeader").remove();
            $("#globalFilterPopUp").remove();
            toolKit.utility.loadSelectedTab(toolKit.CurrentTabInfo.tabId);
        },

        backtoReportConfig: function () {
            toolKit.CurrentTabInfo.ColumnInfo = toolKit.utility.getReportColumnInfo();
            toolKit.reportType.createPopUp(true);
        },

        createViewModel: function () {
            var closure = this;
            function ReportColumns() {
                var self = this;
                self.AddedColumnList = ko.observableArray();
                self.clickEventHandler = function (model, event) {
                    closure.eventHandler(event);
                }
            }
            closure.viewModel = new ReportColumns();
        },

        eventHandler: function (event) {
            var self = this;
            $("#globalFilterPopUp").remove();
            if (event.target.className == "btnAddLnkColumn") {
                self.openLinkPopUp(event);
            }
            else if ($(event.target).hasClass("lnkColumn")) {
                toolKit.addColumns.openLinkPopUp(event);
            }
            else if ($(event.target).attr('id') == "btnAddFxColumn" || $(event.target).hasClass("fxDataColumn")) {
                self.OpenTagging(event);
            }
            else if (event.target.className == "btnSaveLink") {
                self.addColumnForLink(event);
            }
            else if (event.target.className == "headerSectionDelete fa fa-trash-o") {
                var attributeName = $(event.target).parent().find(".addedColumnName").html();
                var category = $(event.target).parent().attr("category");
                self.unselectAttribute(attributeName, category);
                $(event.target).parent().remove();
            }
            else {
                self.createDropDown(event);
            }
        },

        OpenTagging: function (event) {
            var self = this;
            var columnInfo = toolKit.utility.getReportColumnInfo();
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            var globalParam = toolKit.utility.getRightFIlter();
            $.ajax({
                url: url + "/InitializeTagging",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ ColumnInfoJson: JSON.stringify(columnInfo), tabName: toolKit.CurrentTabInfo.TabName, identifier: toolKit.reportInfo.reportIdentifier, dbIdentifier: toolKit.dbIdentifier, GlobalParam: globalParam })
            }).then(function (responseText) {
                var gridinfo = $.parseJSON(responseText.d);
                gridinfo.taggingInfoID = "popUpDiv";
                if ($("#popUpDiv").length == 0)
                    toolKit.tabContentContainer.append("<div id ='popUpDiv'></div>");
                else
                    $("#" + gridinfo.taggingInfoID).tagging('destroyPlugin');
                $("#" + gridinfo.GridId).empty();
                $("#" + gridinfo.taggingInfoID).tagging(
                {
                    IsBindGrid: false,
                    gridInfo: gridinfo,
                    getAllTags: toolKit.ShowReport,
                    actionBy: (toolKit.ShowReport ? iago.user.userName : "All"),
                    serviceURL: toolKit.baseUrl + "/Resources/Services/TagManagement.svc",
                    ruleEditorServiceUrl: toolKit.baseUrl + "/Resources/Services/RADXRuleEditorService.svc",
                    isPersistantTextBoxRequired: false,
                    pageIdentifier: toolKit.reportInfo.reportIdentifier + toolKit.CurrentTabInfo.TabName,
                    tagHeading: 'Fx Columns',
                    ExternalFunctionForSaveTag: toolKit.addReportColumn.saveColumnFromTagging
                });
            });
        },

        saveColumnFromTagging: function (tagInfo, isDelete) {
            if (!isDelete) {
                var self = toolKit.addColumns;
                var datatype = 0;
                if (tagInfo.DataTypeDetails.DataType != 0)
                    datatype = 2;
                var i;
                var groupOperation = '';



                var obj = { "ColumnName": tagInfo.TagRealName,
                    "DisplayColumnName": tagInfo.TagName,
                    "DataType": self.getDataTypeText(datatype),
                    "Unit": "None",
                    "NegativeValue": "Default",
                    "Prefix": "",
                    "DecimalPlaces": "2",
                    "CategoryName": "Tag",
                    "AssemblyName": "",
                    "IsTag": true,
                    "ClassName": "",
                    "TableName": "",
                    "Dataset": "",
                    "GroupByOrder": 0,
                    "GroupByOperation": "None",
                    "GroupAdvanceOperation": "None",
                    "IsColumnHidden": false,
                    "IsRightFilter": false,
                    "IsRangeFilter": false,
                    "mappedGlobalParamVal": "",
                    "mappedGlobalParam": function (model) {
                        return self.getGlobalParamText(model);
                    },
                    "addHiddenClass": function (model) {
                        return self.addHiddenClass(model);
                    },
                    "addRightFilterClass": function (model) {
                        return self.addRightFilterClass(model);
                    },
                    "getGorupByOperationText": function (model) {
                        return toolKit.addColumns.getOperation(model.ColumnName);
                    },
                    "getAdvanceOperationText": function (model) {
                        return toolKit.addColumns.getAdvanceOperation(model.ColumnName);
                    }
                }

                var columnInfo = toolKit.utility.getReportColumnInfo();
                for (i = 0; i < columnInfo.length; ++i) {
                    if (columnInfo[i].GroupByOrder > 0)
                        groupOperation = 'sum';
                }
                obj.GroupByOrder = 0;
                if (groupOperation.length > 0) {
                    obj.GroupByOperation = 3; //SUM
                }

                toolKit.addReportColumn.viewModel.AddedColumnList.push(obj);
                var domCollection = $(".selectedRegionBody").find('tbody').find('tr');
                if (tagInfo.DataTypeDetails.DataType != 0) {
                    var dom = $(domCollection[domCollection.length - 1]);
                    if (tagInfo.DataTypeDetails.DataType != null && tagInfo.DataTypeDetails.DataType != 0) {
                        dom.attr('unit', self.getUnitText(tagInfo.DataTypeDetails.Unit));
                        dom.attr('ngvalue', self.getNgText(tagInfo.DataTypeDetails.NegativeValue));
                        dom.attr('dataType', self.getformatterDataType(tagInfo.DataTypeDetails.DataType + 1));
                        dom.attr('dcmPlace', tagInfo.DataTypeDetails.DecimalPlaces);
                        dom.attr('prefix', tagInfo.DataTypeDetails.Prefix);
                    }
                    else {
                        dom.attr('assemblyName', formatterInfo.AssemblyName);
                        dom.attr('className', formatterInfo.ClassName);
                    }
                }
                else {
                    self.doChange = false;
                    var spinnerValue = $(domCollection[domCollection.length - 1]).find(".inputSectionSpinner").spinner({ min: 0, value: Number.parseInt(obj.GroupByOrder.GroupByOrder), change: function (event, ui) { self.changeSpinner(event); } });
                    spinnerValue.spinner("value", Number.parseInt(obj.GroupByOrder));

                }

                columnInfo = toolKit.utility.getReportColumnInfo();
                toolKit.CurrentTabInfo.ColumnInfo = columnInfo;
                toolKit.newTabAdded = false;
                toolKit.CurrentTabInfo.dbIdentifier = toolKit.dbIdentifier;
                var tabInfo = JSON.stringify(toolKit.CurrentTabInfo);
                toolKit.utility.saveNPreviewTabInfo(tabInfo);
                //rad.TagManager.modalClose('.bootbox-close-button', true);
                self.doChange = true;
            }
            else {
                toolKit.addReportColumn.deleteTag(tagInfo);
            }
        },

        deleteTag: function (tagName) {
            var self = this;
            var index = -1;
            for (var i = 0; i < toolKit.addReportColumn.viewModel.AddedColumnList().length; i++) {
                if (tagName == toolKit.addReportColumn.viewModel.AddedColumnList()[i].DisplayColumnName) {
                    index = i;
                    break;
                }
            }
            if (index > -1) {
                toolKit.addReportColumn.viewModel.AddedColumnList.splice(index, 1);
                columnInfo = toolKit.utility.getReportColumnInfo();
                toolKit.CurrentTabInfo.ColumnInfo = columnInfo;
                toolKit.newTabAdded = false;
                toolKit.CurrentTabInfo.dbIdentifier = toolKit.dbIdentifier;
                var tabInfo = JSON.stringify(toolKit.CurrentTabInfo);
                toolKit.utility.saveNPreviewTabInfo(tabInfo);
                //rad.TagManager.modalClose('.bootbox-close-button', true);
                self.doChange = true;
            }
        },

        unselectAttribute: function (attributeName, category) {
            var self = this;
            for (var i = 0; i < toolKit.reportType.categoriesList.length; i++) {
                if (toolKit.reportType.categoriesList[i].CategoryName == category) {
                    if (toolKit.reportType.categoriesList[i].Attributes != null) {
                        for (var j = 0; j < toolKit.reportType.categoriesList[i].Attributes.length; j++) {
                            if (toolKit.reportType.categoriesList[i].Attributes[j].AttributeName == attributeName) {
                                toolKit.reportType.categoriesList[i].Attributes[j].IsSelected = false;
                                break;
                            }
                        }
                    }
                }
            }
            toolKit.reportType.removeFromAddedList(attributeName);
        },

        createDropDown: function (event) {
            var self = this;
            //            if (event.target.className == "headerSectionUnitChild") {
            //                if ($(event.target).closest('.columnBodySection').find(".dataTypeColumn").html().toLowerCase() != 'string') {
            //                    $(".drpDwnUnit").remove();
            //                    $(".drpDwnGlobalUnit").remove();
            //                    var div = $('<div>');
            //                    div.addClass("drpDwnUnit");
            //                    div.append("<div value='0' class='drpDwnChild'>None</div>");
            //                    div.append("<div value='1' class='drpDwnChild'>Thousand</div>");
            //                    div.append("<div value='2' class='drpDwnChild'>Millions</div>");
            //                    div.append("<div value='3' class='drpDwnChild'>Billions</div>");
            //                    div.css({ 'width': $(event.target).width() });
            //                    $(event.target).parent().append(div);
            //                }
            //            }
            //            else if (event.target.className == "headerSectionNgValueChild") {
            //                if ($(event.target).closest('.columnBodySection').find(".dataTypeColumn").html().toLowerCase() != 'string') {
            //                    $(".drpDwnUnit").remove();
            //                    $(".drpDwnGlobalUnit").remove();
            //                    var div = $('<div>');
            //                    div.addClass("drpDwnUnit");
            //                    div.append("<div value='0' class='drpDwnChild'>Default</div>");
            //                    div.append("<div value='1' class='drpDwnChild'>DefaultBrac</div>");
            //                    div.append("<div value='2' class='drpDwnChild'>Colored</div>");
            //                    div.append("<div value='3' class='drpDwnChild'>ColoredBrac</div>");
            //                    div.css({ 'width': $(event.target).width() });
            //                    $(event.target).parent().append(div);
            //                }
            //            }
            //            else
            if (event.target.className == "globalColumnName" || event.target.className == "globalFilterText" || event.target.className == "globalFilterArrow fa fa-caret-down") {
                if ($(event.target).closest('td').prev().find('div').hasClass("rightColumnNameback")) {
                    $(".drpDwnUnit").remove();
                    $(".drpDwnGlobalUnit").remove();
                    $(".drpDownGlobalUnit").remove();
                    $(".editlnkColumnPopUp").remove();
                    $(".drpAdvDwnUnit").remove();
                    var div = $('<div>');
                    div.addClass("drpDwnGlobalUnit");
                    //                div.append("<div class=\"parameterLabel\"><div class=\"parameterLabelText\">Parameter</div><div class=\"parameterLabelDrpDwnParent\"><div class=\"parameterLabelDrpDwn\">Select One</div></div></div>")
                    var div = $('<div>');
                    div.addClass("drpDownGlobalUnit");
                    if ($(event.target).html() != "Parameters")
                        div.append("<div class='drpDwnGlobalUnitChild'>Parameters</div>");
                    for (var i = 0; i < toolKit.reportInfo.GlobalFilters.length; i++) {
                        div.append("<div class='drpDwnGlobalUnitChild'>" + toolKit.reportInfo.GlobalFilters[i].GlobalParameterName + "</div>");
                    }
                    if (toolKit.reportInfo.GlobalFilters.length > 0) {
                        if ($(event.target).closest('tr').find(".dataTypeColumn").html().toLowerCase() == "date") {
                            div.append("<div class=\"rangeSelect\"><div class='rangeSelectLabel'>Range</div><div class='rangeSelectDiv'><div class=\"isAdvancedOff\">ON</div> <div class=\"isAdvancedOn advanceSelected\">OFF</div></div></div>");
                        }
                    }
                    if (event.target.className == "globalColumnName")
                        $(event.target).parent().parent().append(div);
                    else
                        $(event.target).parent().parent().append(div);
                    if (event.target.className == "globalFilterText") {
                        div.find(".parameterLabelDrpDwn").html($(event.target).html())
                    }
                    if ($(event.target).closest('tr').attr('range') != null) {
                        if ($(event.target).closest('tr').attr('range') == "true") {
                            $(".isAdvancedOff").addClass("advanceSelected");
                            $(".isAdvancedOn").removeClass("advanceSelected");
                        }
                    }
                }
            }
            else if (event.target.className == "parameterLabelDrpDwn") {
                $(".drpDownGlobalUnit").remove();
                $(".editlnkColumnPopUp").remove();

                var div = $('<div>');
                div.addClass("drpDownGlobalUnit");
                if ($(event.target).html() != "Select One")
                    div.append("<div class='drpDwnGlobalUnitChild'>Select One</div>");
                for (var i = 0; i < toolKit.reportInfo.GlobalFilters.length; i++) {
                    div.append("<div class='drpDwnGlobalUnitChild'>" + toolKit.reportInfo.GlobalFilters[i].GlobalParameterName + "</div>");
                }
                div.insertAfter($(event.target));
                var index = $(event.target).closest('tr').index();
                var top = 55 + (34 * (index + 1)) + (10 * index);
                $(".drpDownGlobalUnit").css({ 'top': (top + 'px') });
            }
            else if ($(event.target).closest('.rangeSelect').length > 0) {
                if ($(event.target).hasClass("isAdvancedOff")) {
                    $(event.target).addClass("advanceSelected");
                    $(event.target).next().removeClass("advanceSelected");
                }
                else {
                    $(event.target).addClass("advanceSelected");
                    $(event.target).prev().removeClass("advanceSelected");
                }
            }
            //            else if (event.target.className == "drpDwnChild") {
            //                $(event.target).closest(".drpDwnUnit").prev().html($(event.target).html());
            //                $(event.target).closest(".drpDwnUnit").prev().attr('enumValue', $(event.target).attr('value'));
            //                $(event.target).closest("tr").attr('formatterapplied', "true");
            //                $(event.target).closest(".drpDwnUnit").remove();
            //            }
            else if ($(event.target).hasClass("drpDwnGlobalUnitChild")) {

                if ($(event.target).html() == "Parameters") {
                    var tdele = $(event.target).closest("td");
                    $(event.target).closest('tr').removeAttr("range");
                    $(event.target).closest("td").empty();
                    tdele.append("<div class=\"globalFilterParent\"><div class=\"globalColumnName\">P</div></div>")
                }
                else {
                    var text = $(event.target).html();
                    if ($(event.target).closest('tr').find(".dataTypeColumn").html().toLowerCase() == "date") {
                        if ($(".isAdvancedOff").hasClass("advanceSelected")) {
                            $(event.target).closest('tr').attr("range", "true");
                        }
                        else {
                            $(event.target).closest('tr').attr("range", "false");
                        }
                    }
                    var tdEle = $(event.target).closest("td");
                    $(event.target).closest("td").empty();
                    tdEle.append("<div class=\"tempParent\"><div class=\"globalFilterText\">" + text + "</div><div class=\"globalFilterArrow fa fa-caret-down\"></div>");
                }

                $(".drpDwnGlobalUnit").remove();
            }
            else if ($(event.target).hasClass("rightColumnName")) {
                self.removeDropDowns(event);
                if ($(event.target).attr('isChecked') == "true") {
                    $(event.target).attr('isChecked', false);
                    $(event.target).removeClass('rightColumnNameback')
                    $(event.target).closest('td').next().empty();
                    $(event.target).closest('td').next().append("<div class=\"globalFilterParent\"><div class=\"globalColumnName\">P</div></div>");
                }
                else {
                    $(event.target).attr('isChecked', true);
                    $(event.target).addClass('rightColumnNameback')
                }
            }
            else if (event.target.className == "OperationDropdownRpt") {
                if ($(".headerSectionAggregation").attr("spinnerinitial") == null) {
                    if ($(event.target).closest('.columnBodySection').find(".dataTypeColumn").html().toLowerCase() != 'string' && $(event.target).closest('.columnBodySection').find(".dataTypeColumn").html().toLowerCase() != 'date') {
                        $(".drpDwnUnit").remove();
                        $(".drpDwnOperation").remove();
                        $(".drpDwnGlobalUnit").remove();
                        $(".drpAdvDwnUnit").remove();
                        $(".drpDownGlobalUnit").remove();
                        $(".editlnkColumnPopUp").remove();
                        var div = $('<div>');
                        div.addClass("drpDwnUnit");
                        div.append("<div value='0' class='drpDwnChild'>None</div>");
                        div.append("<div value='1' class='drpDwnChild'>Min</div>");
                        div.append("<div value='2' class='drpDwnChild'>Max</div>");
                        div.append("<div value='3' class='drpDwnChild'>Count</div>");
                        div.append("<div value='4' class='drpDwnChild'>Sum</div>");
                        div.append("<div value='5' class='drpDwnChild'>Average</div>");
                        div.css({ 'width': $(event.target).width() });
                        $(event.target).parent().append(div);
                    }
                }
            }
            else if (event.target.className == "formatterClass") {
                $(".drpDwnUnit").remove();
                $(".editlnkColumnPopUp").remove();
                $(".drpAdvDwnUnit").remove();
                $(".drpDownGlobalUnit").remove();
                $(".toolkitOperationsMainDiv").remove();
                if ($(event.target).closest('tr').find(".dataTypeColumn").html().toLowerCase() != "string" && $(event.target).closest('tr').find(".dataTypeColumn").html().toLowerCase() != "date")
                    self.getFormatterUI(event);
            }
            else if ($(event.target).hasClass("hiddenColumn")) {
                self.removeDropDowns(event);
                if ($(event.target).hasClass("hiddenColumnSelected")) {
                    $(event.target).removeClass("hiddenColumnSelected");
                    $(event.target).closest('tr').removeAttr('hiddenColumn', true);
                }
                else {
                    $(event.target).addClass("hiddenColumnSelected");
                    $(event.target).closest('tr').attr('hiddenColumn', true);
                }
            }
            else if (event.target.className == "AdvOperationDropdownRpt") {
                if ($(".applyGroupClass").length > 0 && ($(event.target).closest("tr").find(".dataTypeColumn").html().trim().toLowerCase() != "string" && $(event.target).closest("tr").find(".dataTypeColumn").html().trim().toLowerCase() != "date")) {
                    self.removeDropDowns(event);
                    var div = $('<div>');
                    div.addClass("drpAdvDwnUnit");
                    if ($(event.target).html() != "None")
                        div.append("<div value='0' class='drpAdvDwnChild'>None</div>");
                    div.append("<div value='1' class='drpAdvDwnChild'>RunningTotal</div>");
                    div.append("<div value='2' class='drpAdvDwnChild'>Percentage</div>");
                    div.css({ 'width': $(event.target).width() });
                    $(event.target).parent().append(div);
                }
            }
            else if (event.target.className == "drpAdvDwnChild") {
                $(event.target).closest("td").find(".AdvOperationDropdownRpt").html($(event.target).html());
                $(event.target).closest("table").find(".AdvOperationDropdownRpt").attr('enumvalue', $(event.target).attr('value'));
                $(event.target).closest(".drpAdvDwnUnit").remove();
            }
            else if (event.target.className == "drpDwnChild") {
                $(event.target).closest("td").find(".OperationDropdownRpt").html($(event.target).html());
                $(event.target).closest("table").find(".OperationDropdownRpt").attr('enumvalue', $(event.target).attr('value'));
                $(event.target).closest(".drpDwnUnit").remove();
            }
            else if ($(event.target).hasClass("noGroupClass")) {
                $(".drpDwnUnit").remove();
                $(".drpAdvDwnUnit").remove();
                $(".drpDownGlobalUnit").remove();
                $(".editlnkColumnPopUp").remove();
                $(".drpDwnGlobalUnit").remove();
                $(event.target).removeClass("noGroupClass");
                $(event.target).addClass("applyGroupClass");
                $(".headerSectionAggregation").removeAttr("spinnerinitial");
            }
            else if ($(event.target).hasClass("applyGroupClass")) {
                $(".drpDwnUnit").remove();
                $(".drpAdvDwnUnit").remove();
                $(".drpDownGlobalUnit").remove();
                $(".editlnkColumnPopUp").remove();
                $(".drpDwnGlobalUnit").remove();
                $(event.target).addClass("noGroupClass");
                $(event.target).removeClass("applyGroupClass");
                $(".headerSectionAggregation").attr("spinnerinitial", false);
                if ($(".applyGroupClass").length == 0) {
                    $(".drpDwnUnit").remove();
                }
            }
            else {
                $(".drpDwnUnit").remove();
                $(".drpAdvDwnUnit").remove();
                $(".drpDownGlobalUnit").remove();
                $(".drpDownGlobalUnit").remove();
                if ($(event.target).closest(".editlnkColumnPopUp").length == 0)
                    $(".editlnkColumnPopUp").remove();
                $(".drpDwnGlobalUnit").remove();
                if ($(event.target).closest(".lnkColumnPopUp").length == 0)
                    $(".lnkColumnPopUp").remove();
                if ($(event.target).closest(".divFormatterUI").length == 0)
                    $(".divFormatterUI").remove();
            }
        },


        removeDropDowns: function (event) {
            $(".drpDwnUnit").remove();
            $(".drpAdvDwnUnit").remove();
            $(".drpDownGlobalUnit").remove();
            $(".editlnkColumnPopUp").remove();
            $(".drpDownGlobalUnit").remove();
            $(".drpDwnGlobalUnit").remove();
            if ($(event.target).closest(".editlnkColumnPopUp").length == 0)
                $(".editlnkColumnPopUp").remove();
            if ($(event.target).closest(".lnkColumnPopUp").length == 0)
                $(".lnkColumnPopUp").remove();
            if ($(event.target).closest(".divFormatterUI").length == 0)
                $(".divFormatterUI").remove();
        },

        getFormatterUI: function (event) {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + "/GetHtmlTemplate",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.toolKitPopUps.htm' })
            }).then(function (responseText) {
                $(".drpDwnUnit").remove();
                $(".drpAdvDwnUnit").remove();
                $(".drpDwnGlobalUnit").remove();
                $(".divFormatterUI").remove();
                var dom = $(event.target).parent();
                $(event.target).parent().append(responseText.d);
                $(".txtPrefix").attr('disabled', true);
                $(".txtDecimal").removeAttr('disabled');
                $(".divFormatterUI").click(function (event) {
                    self.formatterUIHandlers(event)
                    event.stopPropagation();
                })
                if (dom.attr('assemblyName') != null) {
                    $(".txtAssemblyName").val(dom.attr('assemblyName'))
                }
                if (dom.attr('className') != null) {
                    $(".txtAssemblyName").val(dom.attr('className'))
                }
                var dom = $(event.target).closest('tr');
                if (dom.attr('formatterapplied') != null) {
                    if (dom.attr('formatterapplied') == "Normal") {
                        var dom = $(event.target).closest('tr');
                        $(".txtUnit").html(self.getUnitText(dom.attr("unit")));
                        $(".txtNgValue").html(self.getNgText(dom.attr("ngvalue")));
                        $(".txtDecimal").val((dom.attr("dcmPlace")));
                        $(".txtPrefix").val(dom.attr("prefix"));
                        $(".txtdataType").html(self.getformatterDataType(dom.attr('dataType')));
                    }
                }
            });
        },

        formatterUIHandlers: function (event) {
            var self = this;
            if (event.target.className == "btnSaveFormatter") {
                if ($(".btnColor").hasClass("btnNormal")) {
                    $(event.target).closest(".divFormatterUI").closest('tr').attr('formatterapplied', "Normal");
                    var dom = $(".divFormatterUI").parent().parent();
                    dom.removeAttr("assemblyName");
                    dom.removeAttr("className");
                    dom.attr('unit', $(".txtUnit").attr('enumValue'));
                    dom.attr('ngvalue', $(".txtNgValue").attr('enumValue'));
                    dom.attr('dcmPlace', $(".txtDecimal").val());
                    dom.attr('prefix', $(".txtPrefix").val());
                    dom.attr('dataType', $(".txtdataType").html());
                    $(".divFormatterUI").remove();
                }
                else {
                    $(event.target).closest("tr").attr('formatterapplied', "Advanced");
                    dom.removeAttr("unit");
                    dom.removeAttr("ngvalue");
                    dom.removeAttr("dcmPlace");
                    dom.removeAttr("prefix");
                    dom.removeAttr("dataType");
                    var dom = $(".divFormatterUI").parent().parent();
                    dom.attr('assemblyName', $(".txtAssemblyName").val());
                    dom.attr('className', $(".txtClassName").val());
                    $(".divFormatterUI").remove();
                }
            }
            else if (event.target.className == "btnCancelFormatter") {
                $(".divFormatterUI").remove();
            }
            else if (event.target.className == "btnAdvanced") {
                $(".btnAdvanced").addClass('btnColor');
                $(".btnNormal").removeClass('btnColor');
                $(".divNormalFormatterUI").hide();
                $(".divAdvancedFormatterUI").show();
                $(".divAdvancedFormatterUI").removeClass('formatHidden');

            }
            else if (event.target.className == "btnNormal") {
                if ($(event.target).closest('tr').find(".dataTypeColumn").html().toLowerCase() != "string") {
                    $(".btnAdvanced").removeClass('btnColor');
                    $(".btnNormal").addClass('btnColor');
                    $(".divAdvancedFormatterUI").hide();
                    $(".divNormalFormatterUI").show();
                    $(".divNormalFormatterUI").removeClass('formatHidden');
                    var dom = $(event.target).closest('tr');
                    if (dom.attr("dataType") != null) {
                        $(".txtUnit").html(self.getUnitText(dom.attr("unit")));
                        $(".txtNgValue").html(self.getNgText(dom.attr("ngvalue")));
                        $(".txtDecimal").val(dom.attr("dcmPlace"));
                        $(".txtPrefix").val(dom.attr("prefix"));
                        $(".txtdataType").html(self.getformatterDataType(dom.attr('dataType')));
                    }
                    else {
                        $(".txtUnit").html("None");
                        $(".txtNgValue").html("None");
                        $(".txtdataType").html("None");
                        $(".txtDecimal").val("");
                        $(".txtPrefix").val("");
                    }
                }
            }
            else if (event.target.className == "txtdataType") {
                $(".drpDownData").remove();
                $(".drpDownUnit").remove();
                $(".drpDownUnitNgValue").remove();
                var div = $('<div>');
                div.addClass("drpDownData");
                div.append("<div value='0' class='drpDownChild'>None</div>");
                div.append("<div value='1' class='drpDownChild'>String</div>");
                div.append("<div value='2' class='drpDownChild'>Number</div>");
                div.append("<div value='3' class='drpDownChild'>Currency</div>");
                div.append("<div value='4' class='drpDownChild'>Percentage</div>");
                div.css({ 'width': $(event.target).width() });
                $(".divNormalFormatterUI").append(div);
            }
            else if (event.target.className == "txtUnit") {
                $(".drpDownData").remove();
                $(".drpDownUnit").remove();
                $(".drpDownUnitNgValue").remove();
                var div = $('<div>');
                div.addClass("drpDownUnit");
                div.append("<div value='0' class='drpDownChild'>None</div>");
                div.append("<div value='1' class='drpDownChild'>Thousands</div>");
                div.append("<div value='2' class='drpDownChild'>Millions</div>");
                div.append("<div value='3' class='drpDownChild'>Billions</div>");
                div.css({ 'width': $(event.target).width() });
                $(".divNormalFormatterUI").append(div);
            }
            else if (event.target.className == "txtNgValue") {
                $(".drpDownData").remove();
                $(".drpDownUnit").remove();
                $(".drpDownUnitNgValue").remove();
                var div = $('<div>');
                div.addClass("drpDownUnitNgValue");
                div.append("<div value='0' class='drpDownChild'>Default</div>");
                div.append("<div value='1' class='drpDownChild'>DefaultBrac</div>");
                div.append("<div value='2' class='drpDownChild'>Colored</div>");
                div.append("<div value='3' class='drpDownChild'>ColoredBrac</div>");
                div.css({ 'width': $(event.target).width() });
                $(".divNormalFormatterUI").append(div);
            }
            else if (event.target.className == "drpDownChild") {
                if ($(event.target).closest(".drpDownUnitNgValue").length > 0) {
                    $(event.target).closest("table").find(".txtNgValue").html($(event.target).html());
                    $(event.target).closest("table").find(".txtNgValue").attr('enumValue', $(event.target).attr('value'));
                    $(event.target).closest(".drpDownUnitNgValue").remove();
                }
                else if ($(event.target).closest(".drpDownData").length > 0) {
                    if ($(event.target).html() == "String" || $(event.target).html() == "None") {
                        $(".txtDecimal").attr('disabled', 'true');
                        $(".txtDecimal").val("");
                    }
                    else {
                        $(".txtDecimal").removeAttr("disabled");
                    }
                    if ($(event.target).html() == "Currency") {
                        $(".txtPrefix").removeAttr('disabled');
                    }
                    else {
                        $(".txtPrefix").attr('disabled', 'true');
                        $(".txtPrefix").val("");
                    }
                    $(event.target).closest("table").find(".txtdataType").html($(event.target).html());
                    $(event.target).closest("table").find(".txtdataType").attr('enumValue', $(event.target).attr('value'));
                    $(event.target).closest(".drpDownData").remove();
                }
                else {
                    $(event.target).closest("table").find(".txtUnit").html($(event.target).html());
                    $(event.target).closest("table").find(".txtUnit").attr('enumValue', $(event.target).attr('value'));
                    $(event.target).closest(".drpDownUnit").remove();
                }
            }
            else {
                $(".drpDownData").remove();
                $(".drpDownUnit").remove();
                $(".drpDownUnitNgValue").remove();
            }
        },


        getformatterDataType: function (value) {
            if (Number.isNaN(parseInt(value))) {
                return value;
            }
            else {
                switch (Number.parseInt(value)) {
                    case 0:
                        return "None";
                    case 1:
                        return "String";
                    case 2:
                        return "Number";
                    case 3:
                        return "Currency";
                    default:
                        return "Percentage";
                }
            }
        },

        getNgText: function (value) {
            if (Number.isNaN(parseInt(value))) {
                return value;
            }
            else {
                switch (Number.parseInt(value)) {
                    case 0:
                        return "DEFAULT";
                    case 1:
                        return "DEFAULTBRAC";
                    case 2:
                        return "COLORED";
                    case 3:
                        return "COLOREDBRAC";
                    default:
                        return "None";
                }
            }
        },

        getUnitText: function (value) {
            if (Number.isNaN(parseInt(value))) {
                return value;
            }
            else {
                switch (Number.parseInt(value)) {
                    case 0:
                        return "None";
                    case 1:
                        return "THOUSANDS";
                    case 2:
                        return "MILLIONS";
                    case 3:
                        return "BILLIONS";
                    default:
                        return "None";
                }
            }
        },

        addHiddenClass: function (model) {
            var self = this;
            if (model.IsColumnHidden)
                return "hiddenColumn hiddenColumnSelected";
            else
                return "hiddenColumn";
        },



        addRightFilterClass: function (model) {
            var self = this;
            if (model.IsRightFilter || model.mappedGlobalParamVal)
                return "rightColumnName rightColumnNameback";
            else
                return "rightColumnName";
        },

        bindColumnList: function () {
            var self = this;
            for (var i = 0; i < self.columnList.length; i++) {
                self.viewModel.AddedColumnList.push({ "ColumnName": self.columnList[i].ColumnName,
                    "DisplayColumnName": self.columnList[i].DisplayColumnName,
                    "DataType": self.columnList[i].DataType,
                    "Unit": self.columnList[i].Unit,
                    "NegativeValue": self.columnList[i].NegativeValue,
                    "Prefix": self.columnList[i].Prefix,
                    "DecimalPlaces": self.columnList[i].DecimalPlaces,
                    "CategoryName": self.columnList[i].CategoryName,
                    "IsTag": self.columnList[i].IsTag,
                    "AssemblyName": self.columnList[i].AssemblyName,
                    "ClassName": self.columnList[i].ClassName,
                    "TableName": self.columnList[i].TableName,
                    "Dataset": self.columnList[i].Dataset,
                    "GroupByOrder": self.columnList[i].GroupByOrder,
                    "GroupByOperation": self.columnList[i].GroupByOperation,
                    "GroupAdvanceOperation": self.columnList[i].GroupAdvanceOperation,
                    "IsColumnHidden": self.columnList[i].IsColumnHidden,
                    "mappedGlobalParamVal": self.columnList[i].mappedGlobalParamVal,
                    "IsRightFilter": self.columnList[i].IsRightFilter,
                    "IsRangeFilter": self.columnList[i].IsRangeFilter,
                    "mappedGlobalParam": function (model) {
                        return self.getGlobalParamText(model);
                    },
                    "addHiddenClass": function (model) {
                        return self.addHiddenClass(model);
                    },
                    "addRightFilterClass": function (model) {
                        return self.addRightFilterClass(model);
                    },
                    "getGorupByOperationText": function (model) {
                        return toolKit.addColumns.getOperation(model.ColumnName);
                    },
                    "getAdvanceOperationText": function (model) {
                        return toolKit.addColumns.getAdvanceOperation(model.ColumnName);
                    }
                });
            }
            ko.applyBindings(self.viewModel, $(".selectedRegionParentReport")[0]);
            $(".selectedRegionBody").find('tbody').sortable({ axis: 'y', containment: 'parent', items: '.columnBodySection', handle: '.toolKithamBurger', start: function (event, ui) {
                $(ui.placeholder).addClass("ui-helper");
            }
            });
            $(".addColumnsSelectedRegionReport").height($(".selectedRegionParentReport").height() - $(".selectedRegionHeaderReport").height());
            var domCollection = $(".selectedRegionBody").find('tbody').find('tr');
            self.doChange = false;
            for (var i = 0; i < domCollection.length; i++) {
                if ($(domCollection[i]).find(".dataTypeColumn").html().toLowerCase() == "string" || $(domCollection[i]).find(".dataTypeColumn").html().toLowerCase() == "date") {
                    $(domCollection[i]).find(".formatterClass").css({ 'background-color': 'lightgray' });
                }
                if (self.columnList[i].LinkColumn != null) {
                    if (self.columnList[i].LinkColumn.URL == "" || self.columnList[i].LinkColumn.URL == null)
                        $(domCollection[i]).find(".lnkColumn").addClass("attributeNameHidden");
                    else {
                        $(domCollection[i]).find(".lnkColumn").removeClass("attributeNameHidden");
                        $(domCollection[i]).attr('url', self.columnList[i].LinkColumn.URL);
                        $(domCollection[i]).attr('DisplayText', self.columnList[i].LinkColumn.DisplayText);
                        $(domCollection[i]).find(".txtDisplayName").attr('disabled');
                    }
                }
                else {
                    $(domCollection[i]).find(".lnkColumn").addClass("attributeNameHidden");
                }
                var spinnerValue = $(domCollection[i]).find(".inputSectionSpinner").spinner({ min: 0, value: Number.parseInt(self.viewModel.AddedColumnList()[i].GroupByOrder), change: function (event, ui) { self.changeSpinner(event); } });
                spinnerValue.spinner("value", Number.parseInt(self.viewModel.AddedColumnList()[i].GroupByOrder));
                if (Number.parseInt(self.viewModel.AddedColumnList()[i].GroupByOrder) > 0) {
                    $(".headerSectionAggregation").removeAttr("spinnerinitial");
                }

                if (self.columnList[i].FormatterDataType != "None" || self.columnList[i].AssemblyName != "") {
                    //var formatterInfo = self.columnList[i].CustomFormatter;
                    var dom = $(domCollection[i]);
                    if (self.columnList[i].FormatterDataType != null && self.columnList[i].FormatterDataType != 0) {
                        dom.attr('formatterapplied', "Normal");
                        dom.attr('unit', self.getUnitText(self.columnList[i].Unit));
                        dom.attr('ngvalue', self.getNgText(self.columnList[i].NegativeValue));
                        dom.attr('dataType', self.getformatterDataType(self.columnList[i].FormatterDataType));
                        dom.attr('dcmPlace', self.columnList[i].DecimalPlaces);
                        dom.attr('prefix', self.columnList[i].Prefix);
                    }
                    else {
                        dom.attr('formatterapplied', "Advanced");
                        dom.attr('assemblyName', self.columnList[i].AssemblyName);
                        dom.attr('className', self.columnList[i].ClassName);
                    }
                }

                //                if (self.IsnOtNullorEmpty($(self.columnList[i]).attr("formatterInfo"))) {
                //                    var formatterInfo = $(self.columnList[i]).attr("formatterInfo");
                //                    var dom = $(domCollection[i]);
                //                    var formatterArray = formatterInfo.split('|');
                //                    if (formatterInfo.split("|")[0] == "Normal") {
                //                        dom.attr('unit', formatterArray[1]);
                //                        dom.attr('ngvalue', formatterArray[2]);
                //                        dom.attr('dcmPlace', formatterArray[3]);
                //                        dom.attr('prefix', formatterArray[4]);
                //                        dom.attr('dataType', formatterArray[5]);
                //                    }
                //                    else {
                //                        //var formatterArray = formatterInfo.split('|');
                //                        dom.attr('assemblyName', formatterArray[1]);
                //                        dom.attr('className', formatterArray[2]);
                //                    }
                //                }
            }
            self.doChange = true;
        },

        IsnOtNullorEmpty: function (data) {
            if (data != null && data != "")
                return true;
            return false;
        },

        openLinkPopUp: function (event) {
            this.createLinkPopUp(event);
        },

        getGlobalParamText: function (model) {
            if (model.mappedGlobalParamVal == "") {
                return "<div class=\"globalFilterParent\"><div class=\"globalColumnName\">P</div></div>";
            }
            else {
                return "<div class=\"tempParent\"><div class=\"globalFilterText\">" + model.mappedGlobalParamVal + "</div><div class=\"globalFilterArrow fa fa-caret-down\"></div></div>";
            }
        },

        addColumnForLink: function (event) {
            var self = this;
            if ($(event.target).closest(".editlnkColumnPopUp").length > 0) {
                index = $(event.target).closest(".editlnkColumnPopUp").attr('index');

                $($(".selectedRegionBody").find('tbody').find('tr')[index]).attr('url', $(event.target).closest(".editlnkColumnPopUp").find(".txtUrlText").val());
                $($(".selectedRegionBody").find('tbody').find('tr')[index]).attr('displayText', $(event.target).closest(".editlnkColumnPopUp").find(".txtDisplayText").val());
                $($(".selectedRegionBody").find('tbody').find('tr')[index]).find(".txtDisplayName").attr('disabled');
                $(event.target).closest(".editlnkColumnPopUp").remove();
            }
            else {
                self.viewModel.AddedColumnList.push({ "ColumnName": $(event.target).closest(".lnkColumnPopUp").find(".txtColumnDisplayText").val(),
                    "DisplayColumnName": $(event.target).closest(".lnkColumnPopUp").find(".txtColumnDisplayText").val(),
                    "Unit": "None",
                    "NegativeValue": "Default",
                    "Prefix": "",
                    "DecimalPlaces": "2",
                    "DataType": "String",
                    "GroupByOrder": 0,
                    "GroupByOperation": "None",
                    "GroupAdvanceOperation": "None",
                    "CategoryName": "",
                    "AssemblyName": "",
                    "ClassName": "",
                    "TableName": "",
                    "Dataset": "",
                    "IsColumnHidden": false,
                    "IsTag": false,
                    "IsRightFilter": false,
                    "IsRangeFilter":false,
                    "mappedGlobalParamVal": "",
                    "mappedGlobalParam": function (model) {
                        return self.getGlobalParamText(model);
                    },
                    "addHiddenClass": function (model) {
                        return self.addHiddenClass(model);
                    },
                    "addRightFilterClass": function (model) {
                        return self.addRightFilterClass(model);
                    },
                    "getGorupByOperationText": function (model) {
                        return toolKit.addColumns.getOperation(model.ColumnName);
                    },
                    "getAdvanceOperationText": function (model) {
                        return toolKit.addColumns.getAdvanceOperation(model.ColumnName);
                    }
                });
                $(event.target).closest(".lnkColumnPopUp").remove();
                $(".selectedRegionBody").find('tbody').find('tr').last().attr('url', $(event.target).closest(".lnkColumnPopUp").find(".txtUrlText").val());
                $(".selectedRegionBody").find('tbody').find('tr').last().attr('displayText', $(event.target).closest(".lnkColumnPopUp").find(".txtDisplayText").val());
                $(".selectedRegionBody").find('tbody').find('tr').last().find(".txtDisplayName").attr('disabled');
            }
        },

        createLinkPopUp: function (event) {
            var self = this;
            self.event = event;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + "/GetHtmlTemplate",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.toolKitLnkColumn.htm' })
            }).then(function (responseText) {
                if ($(self.event.target).hasClass("lnkColumn")) {
                    $(self.event.target).closest('td').append(responseText.d);
                }
                else {
                    $(".selectedRegionParentReport").append(responseText.d);
                }
                if ($(self.event.target).hasClass("lnkColumn")) {
                    $(".lnkColumnPopUp").addClass("editlnkColumnPopUp").removeClass("lnkColumnPopUp");
                    $(".txtUrlText").val($(self.event.target).closest('tr').attr("url"));
                    $(".txtDisplayText").val($(self.event.target).closest('tr').attr("displaytext"));
                    $(".txtColumnDisplayText").val($(self.event.target).closest('tr').find(".addedColumnName").html());
                    $(".editlnkColumnPopUp").attr("editMode", "true");
                    $(".editlnkColumnPopUp").attr("index", $(self.event.target).closest('tr').index());
                    $(".txtColumnDisplayText").attr("disabled", true);
                    $(".txtColumnDisplayText").closest(".lnkColumnRow").css({ 'display': 'none' });
                    $(".editlnkColumnPopUp").css({ "left": (($(self.event.target).closest('td').width() + 26) + 'px') });
                }
            });
        }
    }
});
$.extend(toolKit, {
    ExistingTabRelation: {
        addTabRelation: function (ListOfTabInfo, ListOfExistingRelation, isCreatinNew) {
            var self = this;
            ListOfExistingRelation = [];
            for (var i = 0; i < toolKit.reportInfo.TabRelationInfo.length; i++) {
                if (toolKit.reportInfo.TabRelationInfo[i].PrimaryTabName == toolKit.CurrentTabInfo.TabName || toolKit.reportInfo.TabRelationInfo[i].SecondryTabName == toolKit.CurrentTabInfo.TabName)
                    ListOfExistingRelation.push(toolKit.reportInfo.TabRelationInfo[i]);
            }
            self.ListOfExistingRelation = ListOfExistingRelation;
            if (ListOfExistingRelation.length == 0)
                isCreatinNew = true;
            if (ListOfExistingRelation.length == 0 || isCreatinNew) {
                if ($("#toolkitExistingRelationsNewPopUp").length != 0) {
                    $("#toolkitExistingRelationsNewPopUp").slideUp("slow", function () {
                        $("#toolkitExistingRelationsNewPopUp").remove();
                    });
                }

                var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
                $.ajax({
                    url: url + "/GetHtmlTemplate",
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.ExistingTabRelation.htm' })
                }).then(function (responseText) {
                    toolKit.tabContentContainer.prepend(responseText.d);
                    self.newMapping(ListOfTabInfo);
                    $("#TabLinkingMainParent").slideDown("slow", function () {
                        $("#TabLinkingMainParent").css({ "display": "block" });
                    })
                    self.addtabRelationHandler(event, ListOfTabInfo, ListOfExistingRelation);
                });
            }
            else {
                var self = this;
                //  self.tableName = tableName;
                var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
                $.ajax({
                    url: url + "/GetHtmlTemplate",
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.ExistingRelations.htm' })
                }).then(function (responseText) {
                    if ($("#TabLinkingMainParent").length != 0) {
                        $("#toolkitExistingRelationsNewPopUp").slideUp("slow", function () {
                            $("#TabLinkingMainParent").remove();
                        });
                    }
                    //                    if ($("#toolkitExistingRelationsNewPopUp").length != 0) {
                    //                        $("#toolkitExistingRelationsNewPopUp").slideUp("slow", function () {
                    //                            $("#toolkitExistingRelationsNewPopUp").remove();
                    //                            self.create_on_AddTabRelation(responseText, ListOfTabInfo, ListOfExistingRelation, isCreatinNew);
                    //                        });
                    //                    }
                    //                    else {
                    //                        self.create_on_AddTabRelation(responseText, ListOfTabInfo, ListOfExistingRelation, isCreatinNew);
                    //                    }
                    if ($("#toolkitExistingRelationsNewPopUp").length != 0) {
                        $("#toolkitExistingRelationsNewPopUp").slideUp("slow", function () {
                            $("#toolkitExistingRelationsNewPopUp").remove();
                            self.create_on_AddTabRelation(responseText.d, ListOfTabInfo, ListOfExistingRelation, isCreatinNew);
                        });
                    }
                    else {
                        self.create_on_AddTabRelation(responseText.d, ListOfTabInfo, ListOfExistingRelation, isCreatinNew);
                    }
                });
            }
        },

        create_on_AddTabRelation: function (responseText, ListOfTabInfo, ListOfExistingRelation, isCreatinNew) {
            var self = this;
            toolKit.tabContentContainer.prepend(responseText);
            self.existingMapping();
            $($(".toolKitExistingRelationsIntileFormEach")[0]).addClass("tileSelected")
            self.AddNewRelationWithexistingTabs(ListOfTabInfo, ListOfExistingRelation, isCreatinNew);
        },
        DeleteTabRelation: function (event, index) {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            var primaryTabname = $(event.target).closest(".toolKitExistingRelationsIntileFormEach").find("#PrimaryTabNameDiv").html().trim();
            var SecondaryTabname = $(event.target).closest(".toolKitExistingRelationsIntileFormEach").find("#SecondaryTabNameDiv").html().trim();
            $.ajax({
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                url: url + '/DeleteTabRelation',
                data: JSON.stringify({ primaryTab: primaryTabname, secondryTab: SecondaryTabname, identifier: toolKit.reportInfo.reportIdentifier, user: iago.user.userName }),
                dataType: "json"
            }).then(function (responseText) {
                if (responseText.d == true) {
                    $(event.target).closest(".toolKitExistingRelationsIntileFormEach").remove();
                    self.ListOfExistingRelation.splice(index, 1);
                    var actualIndex = 0;
                    for (var i = 0; i < toolKit.reportInfo.TabRelationInfo.length; i++) {
                        if (toolKit.reportInfo.TabRelationInfo[i].PrimaryTabName == primaryTabname && toolKit.reportInfo.TabRelationInfo[i].SecondryTabName == SecondaryTabname) {
                            actualIndex = i;
                            break;
                        }
                    }
                    toolKit.reportInfo.TabRelationInfo.splice(actualIndex, 1);
                    self.tabRelationModel.tabRelations.splice(index, 1);

                    if (self.tabRelationModel.tabRelations().length > 0) {
                        toolKit.reportInfo.TabRelationInfo.splice(actualIndex, 1);
                        if ($(event.target).closest(".toolKitExistingRelationsIntileFormEach").hasClass("tileSelected")) {
                            if (index == self.tabRelationModel.tabRelations().length - 1) {
                                self.getTileData(0);
                                $($(".toolKitExistingRelationsIntileFormParent").children()[0]).addClass("tileSelected");
                            }
                            else {
                                self.getTileData(index);
                                $($(".toolKitExistingRelationsIntileFormParent").children()[index]).addClass("tileSelected");
                            }
                        }
                    }
                    else {
                        toolKit.reportInfo.TabRelationInfo.splice(0, 1);
                        self.addTabRelation(toolKit.reportInfo.TabsDetails, toolKit.reportInfo.TabRelationInfo, true);
                    }
                }
            })
        },


        existingMapping: function () {
            //toolKit.reportInfo.TabRelationInfo;
            var closure = this;
            function tabRelation() {
                var self = this;
                self.tabRelations = ko.observableArray();
                self.columnMapping = ko.observableArray();
                self.tabTileClickHandler = function (model, event) {
                    $(".tileSelected").removeClass("tileSelected");
                    $(event.target).closest(".toolKitExistingRelationsIntileFormEach").addClass("tileSelected");
                    var index = $(event.target).closest(".toolKitExistingRelationsIntileFormEach").index();
                    closure.getTileData(index);
                }
                self.PrimaryTabName = ko.observable();
                self.SecondaryTabName = ko.observable();
                self.FirstPageSecondDropDown = ko.observableArray();
                self.FirstPageFirstDropDown = ko.observableArray();
                self.cnt = ko.observable(1);
                self.repeatDiv1stpage = ko.observableArray([{ values: 1}]);
                self.removeTile = function (model, event) {
                    var index = $(event.target).closest(".toolKitExistingRelationsIntileFormEach").index();
                    closure.DeleteTabRelation(event, index);
                    event.stopPropagation();
                }
                self.remDiv = function (model, event) {
                    var index = $(event.target).closest(".toolKitExistingRelationsColumnName").index();
                    self.columnMapping.splice(index, 1);
                    $(event.target).closest(".toolKitExistingRelationsColumnName").remove();
                }
                self.fillFirstPageSecondDropDown = function (model, event) {
                    var index = $(event.target).closest(".toolKitExistingRelationsColumnName").index();
                    if ($(event.target).attr('id') == "EachMappedColumn") {
                        self.columnMapping()[index].secondaryColumnName($(event.target).html());
                        self.columnMapping()[index].secondaryRealColumnName($(event.target).attr('columnname'));
                        $(event.target).closest(".SecondaryColumnNameDiv").find("#OtherMappedColumns").addClass("attributeNameParentHidden");
                    }
                    else {
                        $(".toolKitOtherColumns").addClass("attributeNameParentHidden");
                        $(".toolKitOtherMappedColumns").addClass("attributeNameParentHidden");
                        $(event.target).closest(".SecondaryColumnNameDiv").find("#OtherMappedColumns").removeClass("attributeNameParentHidden");
                        self.FirstPageSecondDropDown.splice(0, self.FirstPageSecondDropDown().length)
                        for (var item in toolKit.reportInfo.TabsDetails) {
                            if (toolKit.reportInfo.TabsDetails[item].TabName === self.SecondaryTabName()) {
                                for (var col in toolKit.reportInfo.TabsDetails[item].ColumnInfo) {
                                    self.FirstPageSecondDropDown.push({ firstpagedisplayColumnName2: toolKit.reportInfo.TabsDetails[item].ColumnInfo[col].DisplayColumnName, firstpagerealColumnName2: toolKit.reportInfo.TabsDetails[item].ColumnInfo[col].ColumnName });
                                }
                            }
                        }
                    }
                }
                self.fillFirstPageFirstDropDown = function (model, event) {
                    var index = $(event.target).closest(".toolKitExistingRelationsColumnName").index();
                    if ($(event.target).attr('id') == "EachColumn") {
                        self.columnMapping()[index].primaryColumnName($(event.target).html());
                        self.columnMapping()[index].primaryRealColumnName($(event.target).attr('columnname'));
                        $(event.target).closest(".PrimaryColumnNameDiv").find("#OtherColumns").addClass("attributeNameParentHidden");
                    }
                    else {
                        $(".toolKitOtherColumns").addClass("attributeNameParentHidden");
                        $(".toolKitOtherMappedColumns").addClass("attributeNameParentHidden");
                        $(event.target).closest(".PrimaryColumnNameDiv").find("#OtherColumns").removeClass("attributeNameParentHidden");
                        self.FirstPageFirstDropDown.splice(0, self.FirstPageFirstDropDown().length)
                        for (var item in toolKit.CurrentTabInfo.ColumnInfo) {
                            self.FirstPageFirstDropDown.push({ firstpagedisplayColumnName: toolKit.CurrentTabInfo.ColumnInfo[item].DisplayColumnName, firstpagerealColumnName: toolKit.CurrentTabInfo.ColumnInfo[item].ColumnName });
                        }
                    }
                }
                self.firstpageRemove = function (model, event) {
                    if ($(event.target).id = "toolKitExistingDeleteButton") {
                        $("#toolkitExistingRelationsNewPopUp").remove();
                    }
                }
                self.firstpageAddMoreColumnMappings = function (model, event) {
                    self.cnt(self.cnt() + 1);
                    self.repeatDiv1stpage.push({ value: self.cnt() });
                    var obj = {};
                    obj.primaryColumnName = ko.observable();
                    obj.primaryRealColumnName = ko.observable();
                    obj.secondaryColumnName = ko.observable();
                    obj.secondaryRealColumnName = ko.observable();
                    obj.primaryColumnName("All Columns");
                    obj.secondaryColumnName("All Columns");
                    closure.tabRelationModel.columnMapping.push(obj);
                }
                self.firstpageSave = function (model, event) {
                    closure.UpdateTabRelation(event);
                }
            }
            closure.tabRelationModel = new tabRelation();
            for (var i = 0; i < toolKit.reportInfo.TabRelationInfo.length; i++) {
                if (toolKit.reportInfo.TabRelationInfo[i].PrimaryTabName == toolKit.CurrentTabInfo.TabName || toolKit.reportInfo.TabRelationInfo[i].SecondryTabName == toolKit.CurrentTabInfo.TabName)
                    closure.tabRelationModel.tabRelations().push(toolKit.reportInfo.TabRelationInfo[i]);
            }
            closure.getTileData(0);
            ko.applyBindings(closure.tabRelationModel, $("#toolkitExistingRelationsNewPopUp")[0]);
        },

        getTileData: function (indx) {
            var self = this;
            var listTabRelations = [];

            self.tabRelationModel.PrimaryTabName(self.ListOfExistingRelation[indx].PrimaryTabName);
            self.tabRelationModel.SecondaryTabName(self.ListOfExistingRelation[indx].SecondryTabName);
            self.tabRelationModel.columnMapping.splice(0, self.tabRelationModel.columnMapping().length);
            if (self.ListOfExistingRelation[indx].IsQueryFilter == true) {
                $("#queryFilterCheckBox")[0].checked = true
            }
            else {
                $("#queryFilterCheckBox")[0].checked = false
            }
            if (self.ListOfExistingRelation[indx].IsTwoWay == true) {
                $("#toolKitCheckBox")[0].checked = true
            }
            else {
                $("#toolKitCheckBox")[0].checked = false
            }
            for (var i = 0; i < self.ListOfExistingRelation[indx].ColumnMappings.length; i++) {
                var obj = {};
                obj.primaryColumnName = ko.observable();
                obj.secondaryColumnName = ko.observable();
                obj.primaryRealColumnName = ko.observable();
                obj.secondaryRealColumnName = ko.observable();
                obj.primaryColumnName(self.getDisplayColumnName(self.ListOfExistingRelation[indx].ColumnMappings[i].split('~~')[0]));
                obj.secondaryColumnName(self.getDisplayColumnName(self.ListOfExistingRelation[indx].ColumnMappings[i].split('~~')[1]));
                obj.primaryRealColumnName(self.ListOfExistingRelation[indx].ColumnMappings[i].split('~~')[0]);
                obj.secondaryRealColumnName(self.ListOfExistingRelation[indx].ColumnMappings[i].split('~~')[1]);
                self.tabRelationModel.columnMapping.push(obj);
            }
        },

        getDisplayColumnName: function (columnName) {
            var self = this;
            for (var i = 0; i < toolKit.CurrentTabInfo.ColumnInfo.length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].ColumnName == columnName) {
                    return toolKit.CurrentTabInfo.ColumnInfo[i].DisplayColumnName;
                    break;
                }
            }
            return columnName;
        },


        newMapping: function (ListOfTabInfo) {
            var closure = this;
            function AddNewRelation(item) {
                var self = this;
                self.count = ko.observable(1);
                self.removeDiv = function (model, event) {
                    var index = $(event.target).closest(".toolKitExistingRelationsColumnName").index();
                    self.repeatDiv.splice(index, 1);
                    $(event.target).closest(".toolKitExistingRelationsColumnName").remove();
                }
                self.selectedTabInfo = toolKit.CurrentTabInfo.TabName;
                self.listOfTab = ko.observableArray(item);
                self.firstTabname = ko.observable(toolKit.CurrentTabInfo.TabName + " Column");
                self.seconTabName = ko.observable("All Columns");
                self.secondTabName = ko.observable("All Tabs");
                self.TabSelected = function (model, event) {
                    for (var i = 0; i < self.repeatDiv().length; i++) {
                        self.repeatDiv()[i].secondaryColumnName("All Columns")
                    }
                    $(event.target).closest(".toolKitOtherTabs").hide();
                    for (var item in ListOfTabInfo) {
                        if (ListOfTabInfo[item].TabName === $(event.target).text()) {
                            self.seconTabName($(event.target).text() + " Column");
                            return ($(event.target).text());
                        }
                    }

                    self.listOfTab.splice(0, self.listOfTab().length);
                }
                self.TabDropDown = function (model, event) {
                    $($(event.target).siblings()[1]).show()
                }
                self.secondDropDown = ko.observableArray();
                self.fillseconddropdown = function (model, event) {
                    var index = $(event.target).closest(".toolKitExistingRelationsColumnName").index();
                    if ($(event.target).attr('class') == "toolKitEachMappedColumn") {
                        self.repeatDiv()[index].secondaryColumnName($(event.target).html());
                        self.repeatDiv()[index].secondaryRealColumnName($(event.target).attr('columnname'));
                        $(event.target).closest("#listofAllNewColumns").find("#OtherMappedColumns").addClass("attributeNameParentHidden");
                    }
                    else {
                        $(".toolKitOtherColumns").addClass("attributeNameParentHidden");
                        $(".toolKitOtherMappedColumns").addClass("attributeNameParentHidden");
                        $(event.target).closest("#listofAllNewColumns").find("#OtherMappedColumns").removeClass("attributeNameParentHidden");
                        self.secondDropDown.splice(0, self.secondDropDown().length)
                        for (var item in ListOfTabInfo) {
                            if ((ListOfTabInfo[item].TabName + " Column") === self.seconTabName()) {
                                for (var col in ListOfTabInfo[item].ColumnInfo) {
                                    self.secondDropDown.push({ displayColumnName2: ListOfTabInfo[item].ColumnInfo[col].DisplayColumnName, realColumnName2: ListOfTabInfo[item].ColumnInfo[col].ColumnName });
                                }
                            }
                        }
                    }
                }
                self.firstDropDown = ko.observableArray();
                self.fillfirstdropdown = function (model, event) {
                    var index = $(event.target).closest(".toolKitExistingRelationsColumnName").index();
                    if ($(event.target).attr('class') == "toolKitEachColumn") {
                        self.repeatDiv()[index].primaryColumnName($(event.target).html());
                        self.repeatDiv()[index].primaryRealColumnName($(event.target).attr('columnname'));
                        $(event.target).closest("#listofAllColumns").find("#OtherColumns").addClass("attributeNameParentHidden");
                    }
                    else {
                        $(".toolKitOtherColumns").addClass("attributeNameParentHidden");
                        $(".toolKitOtherMappedColumns").addClass("attributeNameParentHidden");
                        $(event.target).closest("#listofAllColumns").find("#OtherColumns").removeClass("attributeNameParentHidden");
                        self.firstDropDown.splice(0, self.firstDropDown().length)
                        for (var item in toolKit.CurrentTabInfo.ColumnInfo) {
                            self.firstDropDown.push({ displayColumnName: toolKit.CurrentTabInfo.ColumnInfo[item].DisplayColumnName, realColumnName: toolKit.CurrentTabInfo.ColumnInfo[item].ColumnName });
                        }
                    }
                }

                self.repeatDiv = ko.observableArray([{ primaryColumnName: ko.observable("All Columns"), primaryRealColumnName: ko.observable("All Columns"), secondaryColumnName: ko.observable("All Columns"), secondaryRealColumnName: ko.observable("All Columns")}]);
                self.AddMoreMappingColumn = function (model, event) {
                    self.count(self.count() + 1);
                    self.repeatDiv.push({ primaryColumnName: ko.observable("All Columns"), primaryRealColumnName: ko.observable("All Columns"), secondaryColumnName: ko.observable("All Columns"), secondaryRealColumnName: ko.observable("All Columns") });
                }
            }
            closure.newRelationModel = new AddNewRelation(ListOfTabInfo);
            ko.applyBindings(closure.newRelationModel, $("#TabLinkingMainParent")[0]);
        },

        AddNewRelationWithexistingTabs: function (ListOfTabInfo, ListOfExistingRelation, isCreatinNew) {
            var self = this;
            $(".toolkitExistingTabsNewAddRelationBtn").unbind("click").click(function (event) {
                self.addTabRelation(ListOfTabInfo, ListOfExistingRelation, true);
            });
        },

        addtabRelationHandler: function (event, ListOfTabInfo, ListOfExistingRelationInfo) {
            var self = this;
            //            $(".ToolKitTabLinkingUpperDiv").unbind("click").click(function (event) {
            //                self.handleClickEvent(event, ListOfTabInfo);
            //            });
            $(".ToolKitTabLinkingUpperDiv").unbind("click").click(function (event) {
                self.handleClickEvent(event, ListOfTabInfo);
            });
            $(".toolKitSaveColumnMappings").unbind("click").click(function (event) {
                self.SaveMapping(event, ListOfTabInfo);
            });
            $(".toolKitCancelColumnMappings").unbind("click").click(function (event) {
                self.RemoveMapping(event, ListOfTabInfo);
            });
            //            $("#firstTabDiv").unbind("click").click(function (event) {
            //                self.ExpandTabDropDown(event);
            //            });
            $(".toolKitCheckBox").unbind("click").click(function (event) {
                self.CreateContextMenuSecondaryDiv(event);
            });
            $(".toolkitAddTabRelationExisting").unbind("click").click(function (event) {
                $("#TabLinkingMainParent").empty();
                var div = $("#TabLinkingMainParent").append(self.getNewDiv(event, ListOfTabInfo, ListOfExistingRelationInfo));
                self.addtabRelationHandler(event, ListOfTabInfo, ListOfExistingRelationInfo);
                self.handleClickEvent(event, ListOfTabInfo);

                $("#TabLinkingMainParentExisting").show("blind", 1000);
            });
            if ($(event.target).closest("[isCreatingNew]").attr("isCreatingNew") == true) {
                self.addTabRelation(ListOfTabInfo, true);
            }

        },

        RemoveMapping: function (event, ListOfTabInfo) {
            $("#TabLinkingMainParent").slideUp("slow", function () {
                $("#TabLinkingMainParent").remove();
            });
        },

        ExpandTabDropDown: function (event) {
            if ($(event.target).closest(".toolKitListOfAllTabs").height() == 30) {
                $(event.target).closest(".toolKitListOfAllTabs").height(60)
            }
            else if ($(event.target).closest(".toolKitListOfAllTabs").height() == 60) {
                $(event.target).closest(".toolKitListOfAllTabs").height(30)
                $(event.target).closest(".toolKitListOfAllTabs").css({ "box-shadow": "none" });
            }
        },
        //        ExpandTabDropDown: function (event) {
        //            if ($(event.target).closest(".toolKitListOfAllTabs").height() == 30) {
        //                $(event.target).closest(".toolKitListOfAllTabs").height(60)
        //            }
        //            else if ($(event.target).closest(".toolKitListOfAllTabs").height() == 60) {
        //                $(event.target).closest(".toolKitListOfAllTabs").height(30)
        //                $(event.target).closest(".toolKitListOfAllTabs").css({ "box-shadow": "none" });
        //            }
        //        },

        CreateContextMenuSecondaryDiv: function (event) {
            if ($(event.target).attr("class") == "toolKitCheckBox") {
                $(event.target).addClass("fa fa-check");
                $("#SecondaryContextMenu").text("Context Menu Value");
            }
            else {
                $(event.target).removeClass("fa fa-check");
                $("#SecondaryContextMenu").text("View" + self.newRelationModel.seconTabName());
            }
        },


        //        handleClickEvent: function (event, ListOfTabInfo) {
        //            var self = this;
        //            if ($(event.target).closest(".toolKitListOfAllTabs") != null) {
        //                if ($(event.target).closest(".toolKitListOfAllTabs").height() >= 75) {
        //                    $(event.target).closest(".toolKitListOfAllTabs").height(30);
        //                    $(event.target).closest(".toolKitListOfAllTabs").css({ "box-shadow": "none" });
        //                    $(event.target).closest(".toolKitListOfAllTabs").scrollTop(0);
        //                    $("#firstTabDiv").html($(event.target).html());
        //                    $("#SecondaryContextMenu").text("View" + " " + $("#firstTabDiv").text())
        //                }
        //                else {
        //                    $(event.target).closest(".toolKitListOfAllTabs").height(75);
        //                    $(event.target).closest(".toolKitListOfAllTabs").css({ "box-shadow": "0px 2px 4px #C5C5C5" });
        //                }
        //            }
        //        },

        handleClickEvent: function (event, ListOfTabInfo) {
            var self = this;
            if ($(event.target).closest(".toolKitListOfAllTabs") != null) {
                if ($(event.target).closest(".toolKitListOfAllTabs").height() >= 75) {
                    $(event.target).closest(".toolKitListOfAllTabs").height(30);
                    $(event.target).closest(".toolKitListOfAllTabs").css({ "box-shadow": "none" });
                    $(event.target).closest(".toolKitListOfAllTabs").scrollTop(0);
                    $("#firstTabDiv").html($(event.target).html());
                    $("#SecondaryContextMenu").text("View" + " " + $("#firstTabDiv").text())
                }
                else {
                    $(event.target).closest(".toolKitListOfAllTabs").height(75);
                    $(event.target).closest(".toolKitListOfAllTabs").css({ "box-shadow": "0px 2px 4px #C5C5C5" });
                }
            }
        },

        SaveMapping: function (event, ListOfTabInfo) {
            var self = this;
            var TabRelationInfo = {};
            var PrimaryTab = $("#SelectedTab").text();
            var SecondaryTab = $("#firstTabDiv").text();
            var isTwoWay = false;
            var isQueryFilter = false;
            var ListOfTabColumnMappings = [];

            for (var item = 0; item < self.newRelationModel.repeatDiv().length; item++) {
                ListOfTabColumnMappings.push(self.newRelationModel.repeatDiv()[item].primaryRealColumnName() + "~~" + self.newRelationModel.repeatDiv()[item].secondaryRealColumnName());
            }
            if ($("#toolKitCheckBox")[0].checked)
                isTwoWay = true;
            else
                isTwoWay = false;
            if ($("#queryFilterCheckBox")[0].checked)
                isQueryFilter = true
            else
                isQueryFilter = false
            var ContextMenu = $("#PrimaryTabContextMenu").text().trim() + "~~~" + $("#SecondaryContextMenu").text();
            TabRelationInfo = { "PrimaryTabName": PrimaryTab, "SecondryTabName": SecondaryTab, "ColumnMappings": ListOfTabColumnMappings, "ContextMenuOption": ContextMenu, "IsTwoWay": isTwoWay, "IsQueryFilter": isQueryFilter }

            if (toolKit.reportInfo.TabRelationInfo == null) {
                toolKit.reportInfo.TabRelationInfo = [];
                toolKit.reportInfo.TabRelationInfo.push(TabRelationInfo);
            }
            else {
                toolKit.reportInfo.TabRelationInfo.push(TabRelationInfo);
            }
            self.SaveTabRelation(TabRelationInfo);
        },

        SaveTabRelation: function (TabRelationInfo) {
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            var RelationInfo = JSON.stringify(TabRelationInfo);
            $.ajax({
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                url: url + '/SaveTabRelation',
                data: JSON.stringify({ RelationInfo: RelationInfo, identifier: toolKit.reportInfo.reportIdentifier, user: iago.user.userName }),
                dataType: "json"
            }).then(function (responseText) {
                $("#TabLinkingMainParent").slideUp("slow", function () {
                    $("#TabLinkingMainParent").remove();
                });
                //$("#ContentDiv").append("<div id=\"alertParentDiv1\" class=\"ToolKitAlertPopUpParent\"></div>");
                $("#" + toolKit.identifier).append("<div id=\"alertParentDiv1\" class=\"ToolKitAlertPopUpParent\"></div>");
                $("#alertParentDiv1").append("<div class=\"ToolKitPopupAlertUpperSuccessDiv\"></div>");
                $("#alertParentDiv1").append("<div class=\"ToolKitPopUpError\"></div>")
                $("#alertParentDiv1").append("<div class=\"ToolKitErrorDivLeft ToolKitUserSuccess\"></div>"); //revisit
                $("#alertParentDiv1").append("<div class=\"ToolKitErrorDivRight\"></div>")
                $("#alertParentDiv1").css("top", "-200px");
                $("#alertParentDiv1").animate({ "top": "0px" });
                setInterval(function () { $("#alertParentDiv1").remove() }, 2700);
                $(".ToolKitErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Result</div>")
                if ($("#TabLinkingMainParent").length > 0) {
                    $("#TabLinkingMainParent").slideUp("slow", function () {
                        $("#TabLinkingMainParent").remove();
                    });
                }
                else {
                    $("#toolkitExistingRelationsNewPopUp").slideUp("slow", function () {
                        $("#toolkitExistingRelationsNewPopUp").remove();
                    });
                }
                if (responseText.d == true) {
                    $(".ToolKitErrorDivRight").append("<div class=\"ToolKitErrorDivText\"> Relation Has Been Created Successfully!!! </div>");
                }
                else {
                    $(".ToolKitErrorDivRight").append("<div class=\"ToolKitErrorDivText\"> Relation Creation Failed!!! </div>");
                }

            });
        },

        UpdateTabRelation: function (event) {
            var ListOfColumnMappings = [];
            var self = this;
            var isQueryFilter = false;
            var index = 0;
            //            if ($(".tileSelected").length) {
            //                var index = $(".tileSelected").index();
            //            }
            //            else {
            //                var index = 0;
            //            }
            for (var i = 0; i < toolKit.reportInfo.TabRelationInfo.length; i++) {
                if (toolKit.reportInfo.TabRelationInfo[i].PrimaryTabName == $(".tileSelected").find(".PrimaryTabNameDiv").html().trim() || toolKit.reportInfo.TabRelationInfo[i].SecondryTabName == $(".tileSelected").find(".SecondaryTabNameDiv").html().trim()) {
                    index = i;
                    break;
                }
            }
            if ($("#queryFilterCheckBox")[0].checked)
                isQueryFilter = true
            else
                isQueryFilter = false

            if ($("#toolKitCheckBox")[0].checked) {
                toolKit.reportInfo.TabRelationInfo[index].IsTwoWay = true;
            }
            else {
                toolKit.reportInfo.TabRelationInfo[index].IsTwoWay = false;
            }
            for (var item = 0; item < self.tabRelationModel.columnMapping().length; item++) {
                ListOfColumnMappings.push(self.tabRelationModel.columnMapping()[item].primaryRealColumnName() + "~~" + self.tabRelationModel.columnMapping()[item].secondaryRealColumnName());
            }

            TabRelationInfo = { "PrimaryTabName": self.tabRelationModel.PrimaryTabName(), "SecondryTabName": self.tabRelationModel.SecondaryTabName(), "ColumnMappings": ListOfColumnMappings, "ContextMenuOption": toolKit.reportInfo.TabRelationInfo[index].ContextMenuOption, "IsTwoWay": toolKit.reportInfo.TabRelationInfo[index].IsTwoWay, "IsQueryFilter": isQueryFilter }
            toolKit.reportInfo.TabRelationInfo.splice(index, 1, TabRelationInfo);
            self.SaveTabRelation(TabRelationInfo);
        }
    }
})

$.extend(toolKit, {
    utility: {
        setRightFilter: function (RightFilterValues, gridId, filter) {
            var self = this;
            var sourceList = [];
            if (self.filterData == null)
                self.filterData = [];
            //self.flag = false;
            for (var item in RightFilterValues) {
                if (!self.checkDateColumn(item)) {
                    self.flag = true;
                    sourceList = [];
                    for (var value in RightFilterValues[item]) {
                        sourceList.push({
                            Text: RightFilterValues[item][value],
                            Value: RightFilterValues[item][value],
                            Checked: (filter == null ? true : self.CreateRightFilterCheckBox(RightFilterValues[item][value], filter, item, gridId))
                        });
                    }

                    self.filterData.push({ "eletype": "div", "title": self.getDisplayColumnName(item), "id": item.replace(/ /g, "_") + "Filter", "type": "iago:checkBox", "option-type": "JSON", "options": JSON.stringify({ "Type": "checkbox", "Layout": "default", "CheckboxInfo": sourceList }) })
                }
            }
            if (self.flag) {
                $("#toolKitRightfilter").rightFilter({
                    "Filters": self.filterData,
                    getData: function (filterInfo) {
                        var globalFilter = false;
                        toolKit.rightFilterInfo = [];
                        var filterArray = [];
                        var globalParamInfo = []
                        for (var item in filterInfo) {
                            if (item.endsWith("_GlobalFilter")) {
                                globalFilter = true;
                                //globalParamInfo.push({ "ParameterName": item.substr(0, item.indexOf("_GlobalFilter")), "ParameterValue": filterInfo[item][item.substr(0, item.indexOf("_GlobalFilter"))] });
                                globalParamInfo.push({ "ParameterName": item.substr(0, item.indexOf("_GlobalFilter")).replace(new RegExp('[_]', 'gi'), ' '), "ParameterValue": filterInfo[item].SelectedText });
                            }
                            else {
                                if (filterInfo[item].SelectedText != "") {
                                    var selecteData = filterInfo[item].SelectedText.replace("<EMPTY>", "").split(',');
                                    filterArray.push({ "ColumnName": item.substr(0, item.indexOf("Filter")).replace(new RegExp('[_]', 'gi'), ' '), "Values": selecteData, "latestOperation": "checkBox" });
                                }
                            }
                        }
                        if (globalFilter) {
                            toolKit.IsRightFilterApplied = true;
                            toolKit.rightFilterInfo = filterArray;
                            var globalFilterDate = self.getSelectedDate();
                            if (globalFilterDate != null) {
                                globalParamInfo.push(globalFilterDate);
                            }
                            toolKit.bindTabs.GlobalFilterHandler(globalParamInfo);
                        }
                        else {
                            $find(gridId).filterData(filterArray);
                        }
                    }
                });
            }
        },

        checkDateColumn: function (columnName) {
            var self = this;
            var length = toolKit.CurrentTabInfo.ColumnInfo.length;
            for (var i = 0; i < length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].ColumnName == columnName && toolKit.CurrentTabInfo.ColumnInfo[i].DataType == 1)
                    return true;
            }
            return false;
        },

        getSelectedDate: function () {
            var self = this;
            var startDate = "";
            var columnName = "";
            var flag = false;
            for (var i = 0; i < toolKit.filterDetails.length; i++) {
                if (toolKit.filterDetails[i].DataType == 1) {
                    if ($("#toolKitDateControl").data("iago-widget-dateControl") != null) {
                        flag = true;
                        if (toolKit.filterDetails[i].IsRangeFilter)
                            startDate = moment($("#toolKitDateControl").dateControl('getJson').startDate).format("YYYY-MM-DD") + ',' + moment($("#toolKitDateControl").dateControl('getJson').endDate).format("YYYY-MM-DD");
                        else
                            startDate = moment($("#toolKitDateControl").dateControl('getJson').startDate).format("YYYY-MM-DD");
                        columnName = toolKit.filterDetails[i].FilterName;
                    }
                    else
                        return null;
                }
            }
            if (flag)
                return { "ParameterName": columnName, ParameterValue: startDate };
            else
                return null;
        },

        createInlineFilterForRightFilter: function (GlobalFilterList) {
            var self = this;
            self.flag = false;
            self.filterData = [];
            var GlobalFilter = [];
            for (var i = 0; i < GlobalFilterList.length; i++) {
                if (!GlobalFilterList[i].IsRightFilter)
                    GlobalFilter.push(GlobalFilterList[i]);
            }
            var sourceList = null;
            for (var item in GlobalFilter) {
                sourceList = [];
                if (GlobalFilter[item].DataType == 0 || GlobalFilter[item].DataType == 2) {
                    self.flag = true;
                    var selectedValues = GlobalFilter[item].DefaultValue.split(",");
                    for (var i = 0; i < GlobalFilter[item].FilterValues.length; i++) {
                        sourceList.push({
                            'Text': GlobalFilter[item].FilterValues[i],
                            'Value': GlobalFilter[item].FilterValues[i],
                            //Checked: (filter == null ? true : self.CreateRightFilterCheckBox(RightFilterValues[item][value], filter, item, gridId))
                            Checked: (selectedValues.indexOf(GlobalFilter[item].FilterValues[i]) > -1 ? true : false)
                        });
                    }
                    self.filterData.push({ "eletype": "div", "title": GlobalFilter[item].FilterDisplayName, "id": GlobalFilter[item].FilterName.replace(new RegExp('[ ]', 'gi'), '_') + "_GlobalFilter", "type": "iago:checkBox", "option-type": "JSON", "options": JSON.stringify({ "Type": "checkbox", "Layout": "default", "CheckboxInfo": sourceList }) })
                    //var multipleFIlter = false;
                }
                else {
                    $("#toolKitDateControl").attr("columnname", GlobalFilter[item].FilterName)
                    var obj = {
                        onChange: function (startDate, endDate) {
                            if ($("#toolKitDateControl").data("iago-widget-dateControl").options.controlFormat == "DateRange")
                                toolKit.bindTabs.ApplyDateFilter(startDate, endDate);
                            else
                                toolKit.bindTabs.ApplyDateFilter(startDate);
                        },
                        startDate: moment(GlobalFilter[item].DefaultValue).format("MMM DD YYYY"),
                        format: 'MMM DD YYYY',
                        controlFormat: (GlobalFilter[item].IsRangeFilter ? 'DateRange' : 'AsOfDate')
                    }

                    if (GlobalFilter[item].IsRangeFilter) {
                        obj.startDate = moment(GlobalFilter[item].DefaultValue.split(',')[0]).format("MMM DD YYYY")
                        obj.endDate = moment(GlobalFilter[item].DefaultValue.split(',')[1]).format("MMM DD YYYY")
                    }
                    $("#toolKitDateControl").dateControl(obj)
                }
            }
        },

        inlineFilterHandler: function () {
            var self = this;
        },

        getDisplayColumnName: function (columnName) {
            var self = this;
            var length = toolKit.CurrentTabInfo.ColumnInfo.length;
            for (var i = 0; i < length; i++) {
                if (toolKit.CurrentTabInfo.ColumnInfo[i].ColumnName == columnName)
                    return toolKit.CurrentTabInfo.ColumnInfo[i].DisplayColumnName;
            }
            return "";
        },

        CreateRightFilterCheckBox: function (checkedColumn, filter, columnName, gridId) {
            if (filter == null) {
                return true;
            }
            else {
                for (var i = 0; i < filter.filteredColumnInfo.length; i++) {
                    if (filter.filteredColumnInfo[i].ColumnName == columnName) {
                        if ($find(gridId).filter.filteredColumnInfo[i].IsDestructive == "t") {
                            if ($find(gridId).filter.filteredColumnInfo[i].Values.indexOf(checkedColumn) == -1) {
                                return true;
                            }
                            else {
                                return false;
                            }
                        }
                        else {
                            if ($find(gridId).filter.filteredColumnInfo[i].Values.indexOf(checkedColumn) != -1) {
                                return true;
                            }
                            else {
                                return false;
                            }
                        }
                    }
                }
            }
        },

        getTabId: function (tabName) {
            for (var i = 0; i < toolKit.reportInfo.TabsDetails.length; i++) {
                if (toolKit.reportInfo.TabsDetails[i].TabName == tabName) {
                    return toolKit.reportInfo.TabsDetails[i].tabId;
                }
            }
        },

        getUITabId: function (tabName) {
            for (var i = 0; i < toolKit.bindTabs.tabInfo.length; i++) {
                if (toolKit.bindTabs.tabInfo[i].name == tabName) {
                    return toolKit.bindTabs.tabInfo[i].id;
                }
            }
        },

        showMappedTabs: function () {
            var self = this;
            var count = 0;
            for (var i = 0; i < toolKit.reportInfo.TabsDetails.length; i++) {
                if (toolKit.reportInfo.TabsDetails[i].IsTabSaved) {
                    count++;
                }
            }
            if (count >= 2)
                return true;
            else
                return false;
        },

        setGlobalFilter: function () {

        },

        getTabInfo: function (selectedTabId) {
            var self = this;
            var tabName = 0;
            for (var i = 0; i < toolKit.bindTabs.tabInfo.length; i++) {
                if (toolKit.bindTabs.tabInfo[i].id == selectedTabId) {
                    tabName = toolKit.bindTabs.tabInfo[i].name;
                }
            }
            for (var i = 0; i < toolKit.reportInfo.TabsDetails.length; i++) {
                if (toolKit.reportInfo.TabsDetails[i].TabName == tabName) {
                    return toolKit.reportInfo.TabsDetails[i]; // .DataSourceInfo.TableOrViewName;
                }
            }
        },

        getGlobalParameters: function () {
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetGlobalParameter',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ identifier: toolKit.reportInfo.reportIdentifier })
            }).then(function (responseText) {
                var filterInfo = $.parseJSON(responseText.d);
                toolKit.reportInfo.GlobalFilters = filterInfo;
            });
        },

        getRightFIlter: function () {
            var self = this;
            var filtersList = [];
            //            for (var i = 0; i < toolKit.filterDetails.length; i++) {
            //                if (!toolKit.filterDetails[i].IsRightFilter && toolKit.filterDetails[i].DataType != 1) {
            //                    filtersList.push(toolKit.filterDetails[i].FilterDisplayName)
            //                }
            //            }
            for (var i = 0; i < toolKit.reportInfo.GlobalFilters.length; i++) {
                if (toolKit.reportInfo.GlobalFilters[i].DataSourceTable != "" || toolKit.reportInfo.GlobalFilters[i].PossibleValues != "") {
                    filtersList.push(toolKit.reportInfo.GlobalFilters[i].GlobalParameterName);
                }
            }
            return filtersList;
        },

        getReportColumnInfo: function () {
            var self = this;
            var columnInfo = [];
            var domCollection = $(".selectedRegionBody").find('tr.columnBodySection');
            for (var i = 0; i < domCollection.length; i++) {
                var obj = {
                    ColumnName: $(domCollection[i]).find(".addedColumnName").html(),
                    DisplayColumnName: $(domCollection[i]).find(".txtDisplayName").val(),
                    IsHidden: ($(domCollection[i]).attr('hiddenColumn') != null ? true : false),
                    //                    CustomFormatter: ($(domCollection[i]).attr('formatterapplied') == "true" ? {
                    //                        DecimalPlaces: $(domCollection[i]).find(".txtDecimalPlace").val(),
                    //                        Prefix: $(domCollection[i]).find(".txtPrefix").val(),
                    //                        NegativeValue: $(domCollection[i]).find(".headerSectionNgValueChild").attr('enumValue'),
                    //                        Unit: $(domCollection[i]).find(".headerSectionUnitChild").attr('enumValue'),
                    //                        DataType: self.getDatatype($(domCollection[i]).find(".dataTypeColumn"))
                    //                    } : null),
                    IsRightFilter: ($(domCollection[i]).find(".rightColumnName").attr("isChecked") == "true" ? true : false),
                    mappedGlobalParam: $(domCollection[i]).find(".globalFilterText").length > 0 ? $(domCollection[i]).find(".globalFilterText").html() : "",

                    GroupByOperation: $($(domCollection[i]).find(".headerSectionOperationDropDown")).find("#firstOperationDiv").text(),
                    ReportName: toolKit.reportName,
                    DataType: self.getDatatype($(domCollection[i]).find(".dataTypeColumn")),
                    AttributeDetail: {
                        AttributeName: $(domCollection[i]).find(".txtDisplayName").val(),
                        AttributeRealName: $(domCollection[i]).find(".addedColumnName").html(),
                        TableName: $(domCollection[i]).attr("tableName"),
                        Dataset: $(domCollection[i]).attr("dataSet"),
                        CategoryName: $(domCollection[i]).attr("category")
                    },
                    GroupAdvanceOperation: $(domCollection[i]).attr('enumvalue')
                }
                if (obj.mappedGlobalParam != "") {
                    obj.IsRightFilter = false;
                }
                if ($(domCollection[i]).attr("formatterapplied") != null) {
                    if ($(domCollection[i]).attr("formatterapplied") == "Normal") {
                        obj.CustomFormatter = {
                            DecimalPlaces: (($(domCollection[i]).attr("dcmPlace") == null || $(domCollection[i]).attr("dcmPlace") == "") ? 0 : $(domCollection[i]).attr("dcmPlace")),
                            Prefix: $(domCollection[i]).attr("prefix"),
                            NegativeValue: $(domCollection[i]).attr("ngvalue"),
                            Unit: ($(domCollection[i]).attr('unit') == null ? "None" : $(domCollection[i]).attr('unit')),
                            DataType: $(domCollection[i]).attr('dataType')//self.getDatatype($(domCollection[i]).find(".dataTypeColumn"))
                        }
                    }
                    else {
                        obj.CustomFormatter = {
                            AssemblyName: $(domCollection[i]).find(".txtDecimalPlace").val(),
                            ClassName: $(domCollection[i]).find(".txtPrefix").val()
                        }
                    }
                }
                if ($(domCollection[i]).find(".headerSectionGroupSpinner").find('div').length > 0) {
                    if ($(domCollection[i]).find(".headerSectionGroupSpinner").find('div').hasClass("applyGroupClass"))
                        obj.GroupByOrder = 1;
                    else
                        obj.GroupByOrder = 0;
                }
                else {
                    obj.GroupByOrder = 0;
                }
                //                if ($(domCollection[i]).attr('url') == null) {
                //                    if ($($(domCollection[i]).find(".headerSectionGroupSpinner")).find("#GroupSpinner").length > 0) {
                //                        obj.GroupByOrder = $($(domCollection[i]).find(".headerSectionGroupSpinner")).find("#GroupSpinner").spinner("value") == null ? 0 : $($(domCollection[i]).find(".headerSectionGroupSpinner")).find("#GroupSpinner").spinner("value")
                //                    }
                //                    else {
                //                        obj.GroupByOrder = 0;
                //                    }
                //                }
                //                else {
                //                    obj.GroupByOrder = 0;
                //                }
                if ($($(domCollection[i]).find(".headerSectionOperationDropDown")).find(".OperationDropdownRpt").text().trim().toLowerCase() == "operation") {
                    obj.GroupByOperation = "None";
                }
                else {
                    if (obj.GroupByOrder == 1) {
                        obj.GroupByOperation = $($(domCollection[i]).find(".headerSectionOperationDropDown")).find(".OperationDropdownRpt").text();
                    }
                    else {
                        if (obj.DataType == 0)
                            obj.GroupByOperation = "None";
                        else
                            obj.GroupByOperation = $($(domCollection[i]).find(".headerSectionOperationDropDown")).find(".OperationDropdownRpt").text();
                    }
                }
                if ($(domCollection[i]).attr('url') != null) {
                    obj.LinkColumn = {
                        URL: $(domCollection[i]).attr('url'),
                        DisplayText: $(domCollection[i]).attr('displaytext')
                    }
                }
                if ($(domCollection[i]).attr('range') != null) {
                    if ($(domCollection[i]).attr('range') == "false") {
                        obj.IsRangeFilter = false;
                    }
                    else {
                        obj.IsRangeFilter = true;
                    }
                }
                if ($(domCollection[i]).attr('istag') != null) {
                    if ($(domCollection[i]).attr('istag')) {
                        obj.IsTag = true;
                    }
                    else {
                        obj.IsTag = false;
                    }
                }
                if ($($(domCollection[i]).find(".headerSectionAdvOperationDropDown")).find(".AdvOperationDropdownRpt").text().trim().toLowerCase() == "none")
                    obj.GroupAdvanceOperation = "None";
                else
                    obj.GroupAdvanceOperation = $($(domCollection[i]).find(".headerSectionAdvOperationDropDown")).find(".AdvOperationDropdownRpt").text();
                columnInfo.push(obj);
            }
            return columnInfo;
        },

        getDatatype: function (element) {
            if (element.html().toLowerCase() == "string")
                return 0;
            else if (element.html().toLowerCase() == "date")
                return 1;
            else
                return 2;
        },

        saveNPreviewTabInfo: function (tabInfo) {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/SaveTabInfo',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ tabInfo: tabInfo, identifier: toolKit.reportInfo.reportIdentifier, user: iago.user.userName })
            }).then(function (responseText) {
                if (responseText.d != "") {
                    var info = (responseText.d).split("~~~");
                    toolKit.CurrentTabInfo.IsTabSaved = false;
                    toolKit.CurrentTabInfo = $.parseJSON(responseText.d);
                    toolKit.CurrentTabInfo.IsTabSaved = true;
                    self.setTabInfoAgain();
                    toolKit.utility.showButtons();
                }

                //$("#ContentDiv").append("<div id=\"alertParentDiv1\" class=\"ToolKitAlertPopUpParent\"></div>");
                $("#" + toolKit.identifier).append("<div id=\"alertParentDiv1\" class=\"ToolKitAlertPopUpParent\"></div>");
                $("#alertParentDiv1").append("<div class=\"ToolKitPopupAlertUpperSuccessDiv\"></div>");
                $("#alertParentDiv1").append("<div class=\"ToolKitPopUpError\"></div>")
                $("#alertParentDiv1").append("<div class=\"ToolKitErrorDivLeft ToolKitUserSuccess\"></div>"); //revisit
                $("#alertParentDiv1").append("<div class=\"ToolKitErrorDivRight\"></div>")
                $("#alertParentDiv1").css("top", "-200px");
                $("#alertParentDiv1").animate({ "top": "0px" });
                setInterval(function () { $("#alertParentDiv1").remove() }, 1000);
                $(".ToolKitErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Result</div>")
                if (toolKit.CurrentTabInfo.IsTabSaved) {
                    $(".ToolKitErrorDivRight").append("<div class=\"ToolKitErrorDivText\"> Tab Saved Successfully!!! </div>");
                }
                else {
                    $(".ToolKitErrorDivRight").append("<div class=\"ToolKitErrorDivText\"> Tab Creation Failed!!! </div>");
                }
            });
        },

        setTabId: function (tabName, tabId) {
            var self = this;
            for (var i = 0; i < toolKit.reportInfo.TabsDetails.length; i++) {
                if (toolKit.reportInfo.TabsDetails[i].ColumnName == tabName) {
                    toolKit.reportInfo.TabsDetails[i].tabId = tabId;
                }
            }
        },

        setTabInfoAgain: function () {
            var self = this;
            for (var i = 0; i < toolKit.reportInfo.TabsDetails.length; i++) {
                if (toolKit.reportInfo.TabsDetails[i].TabName == toolKit.CurrentTabInfo.TabName) {
                    toolKit.reportInfo.TabsDetails[i] = toolKit.CurrentTabInfo;
                }
            }
        },

        saveTabInfo: function (tabInfo) {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/SaveTabInfo',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ tabInfo: tabInfo, identifier: toolKit.reportInfo.reportIdentifier, user: iago.user.userName })
            }).then(function (responseText) {
                toolKit.CurrentTabInfo = $.parseJSON(responseText.d);
                //                var info = (responseText.d).split("~~~");
                //                toolKit.CurrentTabInfo = $.parseJSON(info[1]);
                //                var tabId = info[0];
                //                toolKit.CurrentTabInfo.tabId = tabId;
                toolKit.CurrentTabInfo.IsTabSaved = true
                self.setTabInfoAgain();
                var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
                var useName = (toolKit.ShowReport ? iago.user.userName : "All");
                $.ajax({
                    url: url + '/LoadSelectedTab',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ tabId: tabId, identifier: toolKit.reportInfo.reportIdentifier, dbIdentifier: toolKit.dbIdentifier, database: toolKit.DBName, user: useName, calendarName: toolKit.reportInfo.CalendarName })
                }).then(function (responseText) {
                    var response = responseText.d;
                    var gridinfo = $.parseJSON(response.split("~~~~")[2]);
                    toolKit.filterDetails = $.parseJSON(response.split("~~~~")[0]);
                    var tabRelations = $.parseJSON(response.split("~~~~")[1]);
                    var rightFilter = $.parseJSON(response.split("~~~~")[3]);
                    toolKit.tabNameIDMapping[toolKit.CurrentTabInfo.TabName] = tabId;
                    gridinfo.RaiseGridRenderComplete = "toolKit.addColumns.raiseGridRenderComplete";
                    toolKit.tabContentContainer.empty();
                    toolKit.tabContentContainer.append("<div id=\"GlobalFilterDiv\" class=\"GridGlobalFilterDiv\"></div>");
                    if (toolKit.addColumns.getDateFilter()) {
                        $(".iago-page-title").append("<div id=\"toolKitDateControl\" class=\"toolKitDateControl\"></div>");
                    }
                    toolKit.tabContentContainer.append("<div id=\"" + gridinfo.GridId + "\"></div>");
                    var tabRelation = $.parseJSON(response.split("~~~~")[4]);
                    if (tabRelation.length > 1) {
                        $("#AddTabRelation").css({ "display": "inline-block" });
                    }
                    toolKit.addColumns.TabsHandler(tabRelation, tabRelations);
                    neogridloader.create(gridinfo.GridId, "", gridinfo, "");
                    toolKit.bindTabs.createGlobalFilter(toolKit.filterDetails);
                    toolKit.utility.showButtons();
                    if ($("#pageHeaderCustomDiv").find("#toolKitRightfilterParent").length == 0)
                        $("#pageHeaderCustomDiv").append("<div class='toolKitRightfilterParent'><div id=\"toolKitRightfilter\" class=\"toolKitRightfilter\"></div></div>");
                    toolKit.utility.setRightFilter(rightFilter, gridinfo.GridId);
                });

            });
        },

        showButtons: function () {
            $("#pageHeaderCustomDiv").empty();
            if ($(".btnGlobalParameter").length == 0) {
                $("#pageHeaderCustomDiv").append("<div class='btnGlobalParameterParent'><div class='btnGlobalParameter'>Parameter</div></div>");
            }
            if (toolKit.utility.showMappedTabs()) {
                if ($(".btnAddTabRelation").length == 0) {
                    $("#pageHeaderCustomDiv").append("<div class='btnAddTabRelationParent'><div class='btnAddTabRelation'>Navigation</div></div>");
                }
            }
            if (toolKit.CurrentTabInfo.IsTabSaved) {
                if ($(".btnPreview").length == 0) {
                    $("#pageHeaderCustomDiv").append("<div class='btnPreviewParent'><div class='btnPreview'>Preview</div></div>");
                    $(".btnPreview").unbind("click").click(function () {
                        toolKit.addColumns.preview();
                    })
                }
            }
            if ($("#pageHeaderCustomDiv").find(".saveTabInfo").length == 0) {
                $("#pageHeaderCustomDiv").append("<div class='saveTabInfoParent'><div class=\"saveTabInfo\" >Save Tab</div></div>");
                $(".saveTabInfo").unbind("click").click(function () {
                    if (toolKit.CurrentTabInfo.DataSourceInfo.TableOrViewName != "" && toolKit.CurrentTabInfo.DataSourceInfo.TableOrViewName != null)
                        toolKit.addColumns.getColumnInfo();
                    else
                        toolKit.addReportColumn.saveConfigColumn();
                })
            }
            if ($("#pageHeaderCustomDiv").find(".btnReportConfig").length == 0 && !toolKit.ShowReport) {
                $("#pageHeaderCustomDiv").append("<div class='btnReportConfigParent'><div class=\"btnReportConfig\" >All Reports</div></div>");
                $(".btnReportConfig").unbind("click").click(function () {
                    toolKit.ShowReport = false;
                    toolKit.ExistingReports.CreateReportPopUp();
                })
            }
        },


        loadSelectedTabWithFilter: function (tabId) {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            //var tabInfo = JSON.stringify(toolKit.reportInfo.TabsDetails[0]);
            var useName = (toolKit.ShowReport ? iago.user.userName : "All");
            $.ajax({
                url: url + '/LoadSelectedTabWithFilter',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ tabId: tabId, identifier: toolKit.reportInfo.reportIdentifier, dbIdentifier: toolKit.dbIdentifier, database: toolKit.DBName, filterData: toolKit.filtereData, fromTab: toolKit.fromTab, filterDatas: toolKit.rightFilterData, user: useName, calendarName: toolKit.reportInfo.CalendarName })
            }).then(function (responseText) {
                if (responseText.d != null) {
                    self.filterData = [];
                    $("#pageHeaderCustomDiv").empty();
                    if ($(".btnEditParent").length == 0) {
                        if (!toolKit.ShowReport)
                            $("#pageHeaderCustomDiv").append("<div class='btnEditParent'><div class='btnEdit'>Edit</div></div>");
                    }
                    if ($("#pageHeaderCustomDiv").find(".btnReportConfig").length == 0 && !toolKit.ShowReport) {
                        $("#pageHeaderCustomDiv").append("<div class='btnReportConfigParent'><div class=\"btnReportConfig\" >All Reports</div></div>");
                        $(".btnReportConfig").unbind("click").click(function () {
                            toolKit.ShowReport = false;
                            toolKit.ExistingReports.CreateReportPopUp();
                        })
                    }
                    if ($("#pageHeaderCustomDiv").find("#toolKitDateControlParent").length == 0)
                        $("#pageHeaderCustomDiv").append("<div class='toolKitDateControlParent'><div id=\"toolKitDateControl\" class=\"toolKitDateControl\"></div></div>");
                    if ($("#pageHeaderCustomDiv").find("#toolKitRightfilterParent").length == 0)
                        $("#pageHeaderCustomDiv").append("<div class='toolKitRightfilterParent'><div id=\"toolKitRightfilter\" class=\"toolKitRightfilter\"></div></div>");

                    var response = responseText.d;
                    var gridinfo = $.parseJSON(response.split("~~~~")[2]);

                    gridinfo.RaiseGridRenderComplete = "toolKit.addColumns.raiseGridRenderComplete";
                    toolKit.tabContentContainer.empty();
                    toolKit.tabContentContainer.append("<div id=\"GlobalFilterDiv\" class=\"GridGlobalFilterDiv\"></div>");
                    toolKit.tabContentContainer.append("<div id=\"" + gridinfo.GridId + "\"></div>");

                    var tabRelation = $.parseJSON(response.split("~~~~")[1]);
                    toolKit.tabRelation = tabRelation;


                    var response = responseText.d;
                    toolKit.filterDetails = JSON.parse(response.split('~~~~')[0]);
                    self.gridinfo = JSON.parse(response.split('~~~~')[2]);
                    //$("#pageHeaderCustomDiv").find(".btnConfigure").after("<div id=\"toolKitRightfilter\" class=\"toolKitRightfilter\"></div>");
                    //$("#pageHeaderCustomDiv").append("<div id=\"toolKitDateControl\" class=\"toolKitDateControl\"></div>");

                    //self.gridinfo = JSON.parse(response.split('~~~~')[2]);
                    var filteredValuesForRightFilter = JSON.parse(response.split('~~~~')[3]);
                    var gridinfo = JSON.parse(response.split('~~~~')[2]);
                    gridinfo.RaiseGridRenderComplete = "toolKit.addColumns.raiseGridRenderComplete";
                    var filteredValuesForRightFilter = JSON.parse(response.split('~~~~')[3]);

                    //  if (filterDetails.IsRightFilter == true) {
                    var listOfRelatedTabs = [];

                    listOfRelatedTabs.push({ "PrimaryTabName": "Default", "SecondryTabName": "test", "ContextMenuOption": "View Tab test", "IsTwoWay": true });


                    var GlobalFilter = [];
                    GlobalFilter = toolKit.filterDetails

                    toolKit.utility.createInlineFilterForRightFilter(GlobalFilter);

                    listOfRelatedTabs.push({ "PrimaryTabName": "TabA", "SecondryTabName": "TabB", "ContextMenuOption": "View Tab B" });
                    listOfRelatedTabs.push({ "PrimaryTabName": "TabA", "SecondryTabName": "TabC", "ContextMenuOption": "View Tab C" })

                    //self.bindContextMenu(listOfRelatedTabs, gridinfo.GridId);
                    toolKit.utility.setRightFilter(JSON.parse(response.split('~~~~')[3]), self.gridinfo.GridId)
                    self.tabFilter = $.parseJSON(response.split("~~~~")[0]);
                    if ($.parseJSON(response.split("~~~~")[4]).length > 1) {
                        $("#AddTabRelation").css({ "display": "inline-block" });
                    }
                    toolKit.tabContentContainer.empty();
                    toolKit.tabContentContainer.append("<div id=\"" + gridinfo.GridId + "\"></div>");
                    if (tabRelation.length > 0)
                        toolKit.handlers.bindContextMenu(tabRelation, gridinfo.GridId);
                    if ($("#popUpDiv").length == 0)
                        toolKit.tabContentContainer.append("<div id ='popUpDiv'></div>");
                    else
                        $("#" + gridinfo.taggingInfoID).tagging('destroyPlugin');
                    $("#" + gridinfo.GridId).empty();
                    //toolKit.tabContentContainer.append("<div id ='popUpDiv'></div>");
                    gridinfo.taggingInfoID = "popUpDiv";
                    //$("#" + gridinfo.taggingInfoID).tagging('destroyPlugin');
                    gridinfo.taggingInfoID = "popUpDiv";
                    $("#" + gridinfo.taggingInfoID).tagging(
                    {
                        gridInfo: gridinfo,
                        getAllTags: toolKit.ShowReport,
                        actionBy: (toolKit.ShowReport ? iago.user.userName : "All"),
                        serviceURL: toolKit.baseUrl + "/Resources/Services/TagManagement.svc",
                        ruleEditorServiceUrl: toolKit.baseUrl + "/Resources/Services/RADXRuleEditorService.svc",
                        isPersistantTextBoxRequired: false,
                        pageIdentifier: toolKit.reportInfo.reportIdentifier + toolKit.CurrentTabInfo.TabName,
                        tagHeading: 'Publish Report'
                    });
                }
                else {
                    toolKit.utility.alertPopUp(false, "Error occured while Loading Tab");
                }
            });
        },

        setCurrentTab: function (tabId) {
            var self = this;
            for (var i = 0; i < toolKit.reportInfo.TabsDetails.length; i++) {
                if (toolKit.reportInfo.TabsDetails[i].tabId == tabId) {
                    toolKit.CurrentTabInfo = toolKit.reportInfo.TabsDetails[i];
                }
            }
        },

        loadSelectedTab: function (tabId) {
            var self = this;
            self.tabId = tabId;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            //var tabInfo = JSON.stringify(toolKit.reportInfo.TabsDetails[0]);
            var useName = (toolKit.ShowReport ? iago.user.userName : "All");
            $.ajax({
                url: url + '/LoadSelectedTab',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ tabId: tabId, identifier: toolKit.reportInfo.reportIdentifier, dbIdentifier: toolKit.dbIdentifier, database: toolKit.DBName, user: useName, calendarName: toolKit.reportInfo.CalendarName })
            }).then(function (responseText) {
                if (responseText.d != null) {
                    $("#pageHeaderCustomDiv").empty();
                    //                if ($(".btnGlobalParameter").length == 0) {
                    //                    $("#pageHeaderCustomDiv").append("<div class='btnGlobalParameterParent'><div class='btnGlobalParameter'>Global Parameter</div></div>");
                    //                }
                    if ($(".btnEditParent").length == 0) {
                        if (!toolKit.ShowReport)
                            $("#pageHeaderCustomDiv").append("<div class='btnEditParent'><div class='btnEdit'>Edit</div></div>");
                    }
                    if ($("#pageHeaderCustomDiv").find(".btnReportConfig").length == 0 && !toolKit.ShowReport) {
                        $("#pageHeaderCustomDiv").append("<div class='btnReportConfigParent'><div class=\"btnReportConfig\" >All Reports</div></div>");
                        $(".btnReportConfig").unbind("click").click(function () {
                            toolKit.ShowReport = false;
                            toolKit.ExistingReports.CreateReportPopUp();
                        })
                    }
                    if (toolKit.ShowReport) {
                        if (toolKit.reportInfo.UserMapping != null) {
                            if (toolKit.reportInfo.UserMapping.Export)
                                $("#pageHeaderCustomDiv").append("<div class='btnReportExportParent'><div class=\"btnReportExport\" >Export</div></div>");
                        }
                    }
                    else
                        $("#pageHeaderCustomDiv").append("<div class='btnReportExportParent'><div class=\"btnReportExport\" >Export</div></div>");
                    $(".btnReportExport").unbind("click").click(function () {
                        $(".iago-page-title").css({ 'overflow': '' });
                        var element = document.createElement('DIV');
                        element.id = 'downLoadPopUp';
                        element.className = 'xlNeoDownLoadPopUp';
                        var exportExcel = document.createElement('DIV');
                        exportExcel.className = 'xlNeoExcelDownLoadPopUp';
                        exportExcel.innerText = 'Excel';
                        element.appendChild(exportExcel);
                        var exportPDF = document.createElement('DIV');
                        exportPDF.className = 'xlNeoPDFDownLoadPopUp';
                        exportPDF.innerText = 'PDF';
                        element.appendChild(exportPDF);
                        if (document.getElementById('downLoadPopUp') == null) {
                            $(".btnReportExportParent").append(element);
                        }
                        $(".xlNeoDownLoadPopUp").unbind("click").click(function (event) {
                            if ($(event.target).hasClass("xlNeoExcelDownLoadPopUp")) {
                                toolKit.ExportReport.Export("Excel");

                            }
                            else if ($(event.target).hasClass("xlNeoPDFDownLoadPopUp")) {
                                toolKit.ExportReport.Export("PDF");
                            }
                            $(event.target).closest(".xlNeoDownLoadPopUp").remove();
                        })
                    })
                    if ($("#pageHeaderCustomDiv").find("#toolKitDateControlParent").length == 0)
                        $("#pageHeaderCustomDiv").append("<div class='toolKitDateControlParent'><div id=\"toolKitDateControl\" class=\"toolKitDateControl\"></div></div>");
                    if ($("#pageHeaderCustomDiv").find("#toolKitRightfilterParent").length == 0)
                        $("#pageHeaderCustomDiv").append("<div class='toolKitRightfilterParent'><div id=\"toolKitRightfilter\" class=\"toolKitRightfilter\"></div></div>");

                    var response = responseText.d;
                    var gridinfo = $.parseJSON(response.split("~~~~")[2]);
                    toolKit.reportInfo.TabsDetails = $.parseJSON(response.split("~~~~")[4]);
                    toolKit.utility.setCurrentTab(self.tabId);
                    gridinfo.RaiseGridRenderComplete = "toolKit.addColumns.raiseGridRenderComplete";
                    toolKit.tabContentContainer.empty();
                    toolKit.tabContentContainer.append("<div id=\"GlobalFilterDiv\" class=\"GridGlobalFilterDiv\"></div>");
                    //                toolKit.tabContentContainer.append("<div id=\"toolKitDateControl\" class=\"toolKitDateControl\"></div>");
                    toolKit.tabContentContainer.append("<div id=\"" + gridinfo.GridId + "\"></div>");

                    var tabRelation = $.parseJSON(response.split("~~~~")[1]);
                    toolKit.tabRelation = tabRelation;


                    var response = responseText.d;
                    toolKit.filterDetails = $.parseJSON(response.split("~~~~")[0]);
                    self.gridinfo = JSON.parse(response.split('~~~~')[2]);
                    //$("#pageHeaderCustomDiv").find(".btnConfigure").after("<div id=\"toolKitRightfilter\" class=\"toolKitRightfilter\"></div>");
                    //$("#pageHeaderCustomDiv").append("<div id=\"toolKitDateControl\" class=\"toolKitDateControl\"></div>");

                    //self.gridinfo = JSON.parse(response.split('~~~~')[2]);
                    var filteredValuesForRightFilter = JSON.parse(response.split('~~~~')[3]);
                    var gridinfo = JSON.parse(response.split('~~~~')[2]);
                    gridinfo.RaiseGridRenderComplete = "toolKit.addColumns.raiseGridRenderComplete";
                    var filteredValuesForRightFilter = JSON.parse(response.split('~~~~')[3]);

                    //  if (filterDetails.IsRightFilter == true) {
                    var listOfRelatedTabs = [];

                    listOfRelatedTabs.push({ "PrimaryTabName": "Default", "SecondryTabName": "test", "ContextMenuOption": "View Tab test", "IsTwoWay": true });
                    //listOfRelatedTabs.push({ "PrimaryTabName": "", "SecondryTabName": "TabC", "ContextMenuOption": "View Tab C", "IsTwoWay": false })
                    // toolKit.tabNameIDMapping[toolKit.CurrentTab] = tabId;
                    if (JSON.parse(response.split('~~~~')[1]).length > 0) {
                        //toolKit.handlers.bindContextMenu(JSON.parse(response.split('~~~~')[1]), gridinfo.GridId);
                    }
                    // self.bindContextMenu(JSON.parse(response.split('~~~~')[1]), gridinfo.GridId);
                    // self.bindRightFilter(JSON.parse(response.split('~~~~')[3]), gridinfo.GridId)

                    //self.TabsHandler(JSON.parse(response.split('~~~~')[1]));
                    var GlobalFilter = [];
                    GlobalFilter = toolKit.filterDetails
                    //toolKit.bindTabs.createGlobalFilter(GlobalFilter);
                    toolKit.utility.createInlineFilterForRightFilter(GlobalFilter)
                    //self.bindContextMenu(listOfRelatedTabs, gridinfo.GridId);
                    toolKit.RightFilterValues = JSON.parse(response.split('~~~~')[3]);
                    toolKit.utility.setRightFilter(JSON.parse(response.split('~~~~')[3]), self.gridinfo.GridId)
                    self.tabFilter = $.parseJSON(response.split("~~~~")[0]);
                    if ($.parseJSON(response.split("~~~~")[4]).length > 1) {
                        $("#AddTabRelation").css({ "display": "inline-block" });
                    }
                    toolKit.tabContentContainer.empty();
                    toolKit.tabContentContainer.append("<div id=\"" + gridinfo.GridId + "\"></div>");
                    if (tabRelation.length > 0)
                        toolKit.handlers.bindContextMenu(tabRelation, gridinfo.GridId);
                    //neogridloader.create(gridinfo.GridId, "", gridinfo, "");
                    if ($("#popUpDiv").length == 0)
                        toolKit.tabContentContainer.append("<div id ='popUpDiv'></div>");
                    else
                        $("#" + gridinfo.taggingInfoID).tagging('destroyPlugin');
                    gridinfo.taggingInfoID = "popUpDiv";
                    $("#" + gridinfo.taggingInfoID).tagging(
                    {
                        gridInfo: gridinfo,
                        getAllTags: toolKit.ShowReport,
                        actionBy: (toolKit.ShowReport ? iago.user.userName : "All"),
                        serviceURL: toolKit.baseUrl + "/Resources/Services/TagManagement.svc",
                        ruleEditorServiceUrl: toolKit.baseUrl + "/Resources/Services/RADXRuleEditorService.svc",
                        isPersistantTextBoxRequired: false,
                        pageIdentifier: toolKit.reportInfo.reportIdentifier + toolKit.CurrentTabInfo.TabName,
                        tagHeading: 'Publish Report'
                    });
                }
                else {
                    toolKit.utility.alertPopUp(false, "Error occured while Loading Tab");
                }
            });

        },

        alertPopUp: function (success, msg) {
            var self = this;
            //$("#ContentDiv").append("<div id=\"alertParentDiv1\" class=\"ToolKitAlertPopUpParent\"></div>");
            $("#" + toolKit.identifier).append("<div id=\"alertParentDiv1\" class=\"ToolKitAlertPopUpParent\"></div>");
            if (success)
                $("#alertParentDiv1").append("<div class=\"ToolKitPopupAlertUpperSuccessDiv\"></div>");
            else
                $("#alertParentDiv1").append("<div class=\"ToolKitPopupAlertUpperErrorDiv\"></div>");
            $("#alertParentDiv1").append("<div class=\"ToolKitPopUpError\"></div>")
            $("#alertParentDiv1").append("<div class=\"ToolKitErrorDivLeft ToolKitUserSuccess\"></div>"); //revisit
            $("#alertParentDiv1").append("<div class=\"ToolKitErrorDivRight\"></div>")
            $("#alertParentDiv1").css("top", "-200px");
            $("#alertParentDiv1").animate({ "top": "0px" });
            setTimeout(function () { $("#alertParentDiv1").remove() }, 1000);
            $(".ToolKitErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Result</div>")
            $(".ToolKitErrorDivRight").append("<div class=\"ToolKitErrorDivText\">" + msg + "</div>");
        },

        isUserAdmin: function () {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/IsUserAdmin',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ reportName: toolKit.reportName, user: iago.user.userName, moduleId: toolKit.moduleid })
            }).then(function (responseText) {
                var isAdmin = responseText.d;
                if (isAdmin) {
                    toolKit.utility.editReport();
                }
                else {
                    var div = $("<div>");
                    div.addClass("adminUserPopUp");
                    div.append("<div class=\"adminUserMsg\">Do You Want to Clone Report.</div>");
                    div.append("<div class='adminUserFooter'><div class='btnAdmitParent'><div class='btnAdmit'>Yes</div></div><div class='btnNgParent'><div class='btnNg'>No</div></div></div>");
                    $("#" + toolKit.identifier).append(div);
                    $(".adminUserPopUp").click(function (event) {
                        if (event.target.className == "btnAdmit") {
                            $(".adminUserPopUp").remove();
                            toolKit.utility.cloneReport();
                        }
                        else if (event.target.className == "btnNg") {
                            $(".adminUserPopUp").remove();
                        }
                    });

                }
            })
        },

        cloneReport: function () {
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/CloneReport',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ reportName: toolKit.reportName, user: iago.user.userName, moduleId: toolKit.moduleid })
            }).then(function (responseText) {
                toolKit.reportInfo = $.parseJSON(responseText.d);
                toolKit.utility.editReport();
            });
        },

        editReport: function () {
            toolKit.tabContentContainer.empty();
            $("#pageHeaderCustomDiv").empty();
            if ($(".btnGlobalParameter").length == 0) {
                $("#pageHeaderCustomDiv").append("<div class='btnGlobalParameterParent'><div class='btnGlobalParameter'>Parameter</div></div>");
            }
            if (toolKit.utility.showMappedTabs()) {
                if ($(".btnAddTabRelation").length == 0) {
                    $("#pageHeaderCustomDiv").append("<div class='btnAddTabRelationParent'><div class='btnAddTabRelation'>Navigation</div></div>");
                }
            }
            if (toolKit.CurrentTabInfo.IsTabSaved) {
                if ($(".btnPreview").length == 0) {
                    $("#pageHeaderCustomDiv").append("<div class='btnPreviewParent'><div class='btnPreview'>Preview</div></div>");
                    $(".btnPreview").unbind("click").click(function () {
                        toolKit.addColumns.preview();
                    })
                }
            }
            if ($("#pageHeaderCustomDiv").find(".btnReportConfig").length == 0 && !toolKit.ShowReport) {
                $("#pageHeaderCustomDiv").append("<div class='btnReportConfigParent'><div class=\"btnReportConfig\" >All Reports</div></div>");
                $(".btnReportConfig").unbind("click").click(function () {
                    toolKit.ShowReport = false;
                    toolKit.ExistingReports.CreateReportPopUp();
                })
            }
            if (toolKit.CurrentTabInfo.DataSourceInfo.TableOrViewName != "" && toolKit.CurrentTabInfo.DataSourceInfo.TableOrViewName != null) {
                toolKit.addDataSource.createDataSourcePopUp();
                toolKit.addTables.createTablesdropDown();
            }
            else {
                toolKit.reportType.createPopUp(false);
            }
        },

        IsTabNameIsEmptyorDuplicated: function (tabName) {
            if (tabName == null || tabName == "") {
                return true;
            }
            else {
                if (toolKit.reportInfo.TabsDetails != null) {
                    for (var i = 0; i < toolKit.reportInfo.TabsDetails.length; i++) {
                        if (toolKit.reportInfo.TabsDetails[i].TabName.trim().toLowerCase() == tabName.trim().toLowerCase()) {
                            return true;
                        }
                    }
                }
                else
                    return false;
            }
            return false;
        },

        getDbIDentiferDropDown: function () {
            var div = $('<div>', {
                class: 'dbIdentifierParent'
            });
            for (var i = 0; i < toolKit.dbIdentifierList.length; i++) {
                var divChild = $('<div>', {
                    class: 'dbIdentifierChild',
                    text: toolKit.dbIdentifierList[i]
                });
                div.append(divChild);
            }
            return div;
        },

        applytaggingPrivilege:function() {
            if (toolKit.reportInfo.UserMapping.Tagging)
                $(".btnAddFxColumn").addClass("hiddenClass");
            else
                $(".btnAddFxColumn").removeClass("hiddenClass");
        },

        createSimpleDropwDown: function (dropDownList,sharedList) {
            var self = this;
            var dropdownParent = $("<div>", {
                class: 'toolKitcommondropdownParent',
            });

            var dropdownChild = $("<div>", {
                class: 'toolKitcommondropdownChild',
            });
            dropdownParent.append(dropdownChild);
            for (var i = 0; i < dropDownList.length; i++) {
                var drpElementParent = $("<div>", {
                    class: 'toolKitcommondropdownELeParent'
                });

                var drpElementChild = $("<div>", {
                    class: 'toolKitcommondropdownELe',
                    text: dropDownList[i],
                    title: dropDownList[i]
                });
                if (sharedList.indexOf(dropDownList[i]) > -1) {
                    drpElementChild.addClass("toolKitcommondropdownELeSelected");
                }
                drpElementParent.append(drpElementChild);
                dropdownChild.append(drpElementParent)
            }
            return dropdownParent;
        }
    }
});
$.extend(toolKit, {
    ExistingReports: {
        CreateReportPopUp: function () {
            $("#globalFilterPopUp").remove();
            var self = this;
            self.privilegesList = ["Tagging", "Layout", "Excel","Admin"];
            self.shareRptIdentifier = "";
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + "/GetHtmlTemplate",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.ExistingReports.htm' })
            }).then(function (responseText) {
                var tabWidget = $("#pageHeaderTabPanel").data('iagoWidget-toolKittabs');
                if (tabWidget != null) {
                    tabWidget.destroy();
                }
                $("#pageHeaderCustomDiv").empty();
                $("#" + toolKit.identifier).empty()
                $(".iago-page-title").find(".toolKitAddTab").remove();
                $("#" + toolKit.identifier).append(responseText.d);
                self.CreateReportsList();
            });
        },

        CreateReportsList: function () {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetReportForUser',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ user: iago.user.userName, moduleId: toolKit.moduleid })
            }).then(function (responseText) {
                toolKit.ReportsCollection = JSON.parse(responseText.d);
                //toolKit.ReportsCollection = [{ "ReportName": "ReportOne", "ReportDesc": "ReportOneDescription", "UserMapping": ["ragupta@ivp.in", "ayadav@ivp.in", "prmishra@ivp.in", "rkandle@ivp.in"] }, { "ReportName": "ReportTwo", "ReportDesc": "ReportTwoDescription", "UserMapping": ["ragupta@ivp.in", "ayadav@ivp.in", "prmishra@ivp.in", "rkandle@ivp.in"] }, { "ReportName": "ReportThree", "ReportDesc": "ReportThreeDescription", "UserMapping": ["ragupta@ivp.in", "ayadav@ivp.in", "prmishra@ivp.in", "rkandle@ivp.in"] }, { "ReportName": "ReportFour", "ReportDesc": "ReportFourDescription", "UserMapping": ["ragupta@ivp.in", "ayadav@ivp.in", "prmishra@ivp.in", "rkandle@ivp.in"]}]
                //  self.bindProcedures(["Test1", "Test2", "Test3", "Test4"]);
                self.bindReports(toolKit.ReportsCollection);
                var height = ($("#addReportsBody").height() - $(".selectedRegionHeader").height() - 85) + 'px';
                $(".addColumnsSelectedRegion").css({ "height": height });
                if (toolKit.ReportsCollection.length == 0)
                    $(".noReportsData").removeClass("hiddenClass");
                else {
                    $(".addedcolumnHeaderReport").removeClass("hiddenClass");
                    $(".searchAddedColumnsDivReport").removeClass("hiddenClass");
                }
            });
        },

        bindReports: function (ReportsArray) {
            var self = this;
            self.viewModelReport = new toolKit.ReportViewModel();
            var i = 0;
            for (var item in ReportsArray) {
                if (ReportsArray[item].PublishPath == null)
                    ReportsArray[item].PublishPath = "";
                self.viewModelReport.ReportList.push(ReportsArray[item]);
                self.viewModelReport.ReportList()[i].callFunc = function (index) {
                    //   self.CreateUserMappedString(self.viewModelReport,index,i);
                    var a = "";
                    //var userNames = Object.keys(toolKit.ExistingReports.viewModelReport.ReportList()[index()].UserMapping);
                    var userNames = (toolKit.ExistingReports.viewModelReport.ReportList()[index()].UserMapping);
                    if (userNames.length >= 2) {
                        for (var j = 0; j < 2; j++) {
                            if (userNames[j].Name != "") {
                                a += userNames[j].Name + (j == userNames.length - 1 ? "" : ",");
                            }
                            else {
                                a += "";
                            }
                        }
                        if (a != "" && userNames.length > 2)
                            a += userNames.length - 2 + "more";
                    }
                    else if (userNames.length == 1) {
                        if (userNames[0].Name != "")
                            a += userNames[0].Name;
                        else
                            a = "";
                    }
                    if (a == "") {
                        //  $($(".addedReportMappedUser")[index()]).addClass("fa fa-plus-circle addUserMappingReport");
                        a = "<div class=\"fa fa-user addUserMappingReport\"></div><div class=\"toolkitaddusermappingNew\">Add User</div>";
                    }
                    else {
                        a = "<div class=\"fa fa-user addUserMappingReport\"></div><div class=\"toolkitaddusermappingNew\">" + a + "</div>";
                    }
                    return a;
                }
                i++;
            }

            
            ko.applyBindings(self.viewModelReport, $(".toolKitAddReports")[0]);
            var height = $(document).height() - (34 + $("#header-div").height() + $(".selectedRegionHeader").height() + 50);
            $(".addColumnsSelectedRegionReports").height(height);

            self.ReportHandler(event, self.viewModelReport, ReportsArray);
            $(".columnBodySectionReport").unbind("click").click(function (event) {
                self.reportRowClickHandler(event);
            });
            $(".searchAddedColumnsInmputReport").unbind("keyup").keyup(function (event) {
                self.searchReportByName();
            })
        },

        searchReportByName: function () {
            var self = this;
            var reportCollection = $(".addColumnsSelectedRegionReports").find('tbody tr');
            var length = reportCollection.length;
            var searchText = $(".searchAddedColumnsInmputReport").val();
            for (var i = 0; i < length; i++) {
                if ($(reportCollection[i]).find(".addedReportName").html().toLowerCase().indexOf(searchText.toLowerCase()) > -1) {
                    $(reportCollection[i]).removeClass("attributeNameHidden");
                }
                else {
                    $(reportCollection[i]).addClass("attributeNameHidden");
                }
            }
        },

        CreateUserMappedString: function (viewModelreport, index, i) {
            var self = this;
            var a = "";
            if (toolKit.ExistingReports.viewModelReport.ReportList()[index()].UserMapping.length >= 2) {
                for (var j = 0; j < 2; j++) {
                    if (toolKit.ExistingReports.viewModelReport.ReportList()[index()].UserMapping[j] != "") {
                        a += toolKit.ExistingReports.viewModelReport.ReportList()[index()].UserMapping[j] + ",";
                    }
                    else {
                        a += "";
                    }
                }
                if (a != "")
                    a += toolKit.ExistingReports.viewModelReport.ReportList()[index()].UserMapping.length - 2 + "more";
            }
            else if (toolKit.ExistingReports.viewModelReport.ReportList()[index()].UserMapping.length == 1) {
                if (toolKit.ExistingReports.viewModelReport.ReportList()[index()].UserMapping[0] != "")
                    a += toolKit.ExistingReports.viewModelReport.ReportList()[index()].UserMapping[0];
                else
                    a = "";
            }
            if (a == "") {
                //  $($(".addedReportMappedUser")[index()]).addClass("fa fa-plus-circle addUserMappingReport");
                a = "<div class=\"toolkitaddusermappingNew\">AddUser</div><div class=\"fa fa-user-plus addUserMappingReport\"></div>";
            }
            return a;

            i++;

        },

        ReportHandler: function (event, ReportModel, ReportsArray) {
            var self = this;
            $(".btnAddReportAgain").unbind("click").click(function (event) {
                $(".CalendarListMainParent").remove();
                self.CreateNewReport();
            });
            $(".seacrhAddedColIconReport").unbind("click").click(function (event) {
                self.CreateSearchDiv(event);
            });

        },
        CreateSearchDiv: function (event) {
            if (!$(event.target).hasClass("toolKitBorderClass")) {
                //$(event.target).css({ "float": "left" });
                //$($(event.target).parent().children()[1]).css({ "border-bottom": "1px solid #00BFF0" })
                $(event.target).addClass("toolKitBorderClass");
                $(event.target).next().children().first().css({ "border-bottom": "1px solid #00BFF0" })
                $(event.target).next().children().first().attr('placeholder','Search');
            }
            else {
                $(event.target).css({ "float": "right" });
                $(event.target).removeClass("toolKitBorderClass");
                //$($(event.target).parent().children()[1]).css({ "border-bottom": "0px solid" })
                $(event.target).next().children().first().css({ "border-bottom": "0px solid" })
                $(event.target).next().children().first().removeAttr('placeholder');
            }
        },

        CreateNewReport: function (event) {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetAllReports',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ moduleId: toolKit.moduleid, user: iago.user.userName })
            }).then(function (responseText) {
                toolKit.ExistingReportsCollection = $.parseJSON(responseText.d);
                $("#NewReportPopUpParent").remove();
                //toolKit.tabContentContainer.prepend("<div id=\"NewReportPopUpParent\" class=\"toolKitNewReportPopUp\"></div>");
                $("#" + toolKit.identifier).prepend("<div id=\"NewReportPopUpParent\" class=\"toolKitNewReportPopUp\" style=\"display:none\"></div>");
                $("#NewReportPopUpParent").append("<div><div id=\"NewReportHeading\" class=\"toolKitReportHeading\">CREATE REPORT</div><div id=\"CloneexistingReport\" class=\"toolKitCloneExistingReport\"style=\"display:none\">Clone Existing Report</div></div>")
                $("#NewReportPopUpParent").append("<div id=\"CreatReportLeftPortion\" class=\"CreatReportLeftPortion\"></div>")
                $("#CreatReportLeftPortion").append("<div id=\"ReportNameParent\" class=\"ReportNameParent\"></div>");
                $("#ReportNameParent").append("<div id=\"toolKitReportNameLabel\" class=\"toolKitReportNameLabel\">Report Name</div>")
                $("#ReportNameParent").append("<div id=\"toolKitReportNameText\" class=\"toolKitReportNameText\" contenteditable=\"true\"></div>");
                $("#ReportNameParent").append("<div id=\"rptNameValidation\" title = \"Report Name Already Exists\" class=\"rptNameValidation hiddenClass\" >!</div>");
                $("#CreatReportLeftPortion").append("<div id=\"CalendarNameParent\" class=\"CalendarNameParent hiddenClass\"></div>");
                $("#CalendarNameParent").append("<div id=\"toolKitCalendarNameLabel\" class=\"toolKitCalendarNameLabel\">Calendar Name</div>");
                $("#CalendarNameParent").append("<div id=\"toolKitCalendarNameText\" class=\"toolKitCalendarNameText\"></div>");

                $("#CreatReportLeftPortion").append("<div id=\"ReportDescParent\" class=\"ReportDescParent\"></div>")
                $("#ReportDescParent").append("<div id=\"toolKitReportDescTextParent\" class=\"toolKitReportDescText\"></div>")
                $("#toolKitReportDescTextParent").append("<textarea id=\"toolKitReportDescTextEditable\" class=\"toolKitReportDescTextEditable\" placeholder=\"Report Description\"></textarea>")
                $("#NewReportPopUpParent").append("<div id=\"CreatReportRightPortion\" class=\"CreatReportRightPortion\" style=\"display:none;\"></div>")
                $("#CreatReportRightPortion").append("<div id=\"toolKitReportPopUpSeperator\" class=\"toolKitReportPopUpSeperator\"></div>")
                $("#CreatReportRightPortion").append("<div id=\"toolKitExistingReportParent\" class=\"toolKitExistingReportParent\"></div>")
                $("#toolKitExistingReportParent").append("<div id=\"toolKitExistingReportLabel\" class=\"toolKitExistingReportLabel\">Existing Reports</div>")
                $("#toolKitExistingReportParent").append("<div id=\"toolKitExistingReportDropDownParent\" class=\"toolKitExistingReportDropDownParent\"></div>");
                $("#toolKitExistingReportDropDownParent").append("<div id=\"toolKitExistingReportDropDownFirstDiv\" class=\"toolKitExistingReportDropDownFirstDiv\">None</div>")
                $("#toolKitExistingReportDropDownParent").append("<div id=\"toolKitExistingReportDropDownOptions\" class=\"toolKitExistingReportDropDownOptions\"></div>")
                $("#CreatReportRightPortion").append("<div id=\"toolKitExistingTabReportParent\" class=\"toolKitExistingTabReportParent\"></div>")
                $("#toolKitExistingTabReportParent").append("<div id=\"toolKitExistingReportTabLabel\" class=\"toolKitExistingReportTabLabel\">Existing Tabs</div>")
                $("#toolKitExistingTabReportParent").append("<div id=\"toolKitExistingReportTabsDropDownParent\" class=\"toolKitExistingReportTabsDropDownParent\">None</div>");
                $("#NewReportPopUpParent").append("<div id=\"addReportFooter\" class=\"toolKitAddReportFooter\"></div>");
                $("#addReportFooter").append("<div id=\"addReportSaveBtn\" class=\"toolKitAddReportsaveBtn\">Save</div>");
                $("#addReportFooter").append("<div id=\"addReportCancelBtn\" class=\"toolKitAddReportCancelBtn\">Cancel</div>");
                // $("#addReportFooter").append("<div id=\"CloneexistingReport\" class=\"toolKitCloneExistingReport\">Clone Existing Report</div>")
                $("#NewReportPopUpParent").slideDown("slow", function () {
                    $("#NewReportPopUpParent").css({ "display": "" });
                });
                self.getAllCalendars();
                self.CreateNewReportPopUpHandler();
            });
        },

        getAllCalendars: function () {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetAllCalendar',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json'
            }).then(function (responseText) {
                if (responseText.d != null) {
                    self.calendarList = $.parseJSON(responseText.d);
                    if (self.calendarList.length > 0) {
                        $("#CalendarNameParent").removeClass("hiddenClass");
                        $("#toolKitCalendarNameText").html(self.calendarList[0]);
                    }
                }
            });
        },

        CreateNewReportPopUpHandler: function (event) {
            var self = this;
            $("#CalendarNameParent").click(function (event) {
                $("#CalendarNameParent").click(function (event) {
                    if ($(event.target).hasClass("toolKitCalendarNameText")) {
                        $(".CalendarListMainParent").remove();
                        self.getCalendarDropDown(event);
                        var width = $(".toolKitCalendarNameLabel").width() + 'px';
                        $(".CalendarListMainParent").css({ 'left': width });
                        $(".CalendarListMainParent").css({ 'top': '35px' });
                    }
                    else if ($(event.target).hasClass("CalendarListChild")) {
                        $(".toolKitCalendarNameText").html($(event.target).html());
                        $(event.target).parent().remove();
                    }
                });
            });
            $(".toolKitExistingReportDropDownParent").unbind("click").click(function (event) {
                self.ExpandDropDownInReportPopUp(event);
            });
            $(".toolKitExistingTabReportParent").unbind("click").click(function (event) {
                //self.SetReportNameInDropDown(event);
                self.getTabDetails(event)
            });
            $(".toolKitAddReportCancelBtn").unbind("click").click(function (event) {
                self.RemoveCreateReportPopUp(event);
            });

            $(".toolKitAddReportsaveBtn").unbind("click").click(function (event) {
                self.SaveNewReport(event);
            });
            $(".toolKitCloneExistingReport").unbind("click").click(function (event) {
                self.ShowRightPortion(event);
            });
            $(".toolKitReportNameText").unbind("keyup").keyup(function () {
                if ($(".toolKitReportNameText").text().trim().length > 0) {
                    $("#rptNameValidation").addClass("hiddenClass");
                }
            });
        },

        getCalendarDropDown: function (event) {
            var self = this;
            $(".CalendarListMainParent").remove();
            var divParent = $("<div>", {
                class: 'CalendarListMainParent'
            });
            var div = $("<div>", {
                class: 'CalendarListParent'
            });
            for (var i = 0; i < self.calendarList.length; i++) {
                var divChild = $("<div>", {
                    class: "CalendarListChild",
                    text: self.calendarList[i]
                });
                div.append(divChild);
            }
            divParent.append(div);
            $(event.target).parent().append(divParent);
        },

        getTabDetails: function (event) {
            var self = this;
            if (event.target.className == "toolKitExistingReportTabsDropDownParent") {
                self.getTabsForReport();
            }
            else if (event.target.className == "tabCollectionItem") {
                $("#toolKitExistingReportTabsDropDownParent").html($(event.target).html());
                $(".tabCollection").remove();
            }
        },

        getTabsForReport: function () {
            var self = this;
            var reportName = $(".toolKitExistingReportDropDownFirstDiv").html();
            var div = $("<div>");
            div.addClass("tabCollection");
            for (var key in toolKit.ExistingReportsCollection) {
                if (key == reportName) {
                    var tabsArray = toolKit.ExistingReportsCollection[key];
                    for (var j = 0; j < tabsArray.length; j++) {
                        div.append("<div class='tabCollectionItem'>" + tabsArray[j].TabName + "</div>");
                    }
                    $(".toolKitExistingTabReportParent").append(div);
                    break;
                }
            }
            $(".toolKitExistingTabReportParent").append(div);
        },

        reportRowClickHandler: function (event) {
            var self = this;
            self.counter = 0;
            if (event.target.className == "addedReportName") {
                $("#" + toolKit.identifier).empty();
                $("#" + toolKit.identifier).append("<div id=\"" + toolKit.tabContainer + "\"></div>");
                toolKit.identifierForReport = "";
                toolKit.reportName = $(event.target).html();
                if ($(".iago-page-title").find(".toolKitAddTab").length == 0) {
                    if (!toolKit.ShowReport)
                        $(".iago-page-title").append("<div class=\"fa fa-plus-circle toolKitAddTab\" aria-hidden=\"true\"></div>");
                }
                toolKit.handlers.getToolKItInfo();
            }
            else if (event.target.className == "AddedReportDelete fa fa-trash-o") {
                self.deleteReport(event);
            }
            else if (event.target.className == "PublishReport" || event.target.className == "PublishedReport") {
                $(".toolKitAddReports").css("opacity", "0.2");
                var reportName = $(event.target).closest(".columnBodySectionReport").find(".addedReportName").html();
                var identifier = $(event.target).closest(".columnBodySectionReport").attr("identifier");
                toolKit.PublishReport.BuildLeftMenu(reportName, identifier);

            }
            else if (event.target.className == "displaycalendarName") {
                $("#toolKitUserMappingParent").remove();
                self.getCalendarDropDownList(event);
            }
            else if (event.target.className == "CalendarListChild") {
                $(event.target).closest(".headerReportDesc").find(".displaycalendarName").html("Calendar : " + $(event.target).html());
                self.saveCalendarName($(event.target).closest('tr').attr("identifier"), $(event.target).html());
                $(event.target).parent().remove();
            }
            else {
                $(".CalendarListMainParent").remove();
            }
        },

        saveCalendarName: function (rptName, calName) {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/SaveCalendar',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ reportName: rptName, calendarName: calName })
            }).then(function (responseText) {
                if (!responseText.d) {
                    toolKit.utility.alertPopUp("Save Calendar failed", false);
                }
            });
        },

        getCalendarDropDownList: function (event) {
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetAllCalendar',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json'
            }).then(function (responseText) {
                if (responseText.d != null) {
                    self.calendarList = $.parseJSON(responseText.d);
                    if (self.calendarList.length > 0) {
                        self.getCalendarDropDown(event);
                        $(".CalendarListMainParent").css({ 'width': '200px' });
                        $(".CalendarListMainParent").css({ 'top': '60px' });
                        $(".CalendarListMainParent").css({ 'left': '30%' });
                    }
                }
            });
        },

        deleteReport: function (event) {
            reportName = $(event.target).closest('tr').find(".addedReportName").html();
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/DeleteReport',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ reportIdentifier: reportName + iago.user.userName + toolKit.moduleid, User: iago.user.userName, moduleId: toolKit.moduleid })
            }).then(function (responseText) {
                //toolKit.ExistingReportsCollection = JSON.parse(responseText.d);
                if (responseText.d == true) {
                    $("#NewReportPopUpParent").remove();
                    //$("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"ToolKitAlertPopUpParent\"></div>");
                    $("#" + toolKit.identifier).append("<div id=\"alertParentDiv1\" class=\"ToolKitAlertPopUpParent\"></div>");
                    $("#alertParentDiv").append("<div class=\"ToolKitPopupAlertUpperSuccessDiv\"></div>");
                    $("#alertParentDiv").append("<div class=\"ToolKitPopUpError\"></div>")
                    $("#alertParentDiv").append("<div class=\"ToolKitErrorDivLeft ToolKitUserSuccess\"></div>"); //revisit
                    $("#alertParentDiv").append("<div class=\"ToolKitErrorDivRight\"></div>")
                    $(".ToolKitErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Result</div>")
                    $(".ToolKitErrorDivRight").append("<div class=\"ToolKitErrorDivText\"> Report Has Been Deleted Successfully!!! </div>");
                    $("#alertParentDiv").css("top", "-200px");
                    $("#alertParentDiv").animate({ "top": "0px" });
                    setInterval(function () { $("#alertParentDiv").remove() }, 2700);

                    var reportName = $(event.target).parent().parent().find(".addedReportName").html();
                    $(event.target).closest('tr').remove();
                    var index = 0;
                    for (var i = 0; i < toolKit.ReportsCollection.length; i++) {
                        if (toolKit.ReportsCollection[i].ReportName == reportName) {
                            index = i;
                            break;
                        }
                    }
                    toolKit.ReportsCollection.splice(index, 1);
                }
            });
        },

        ShowRightPortion: function (event) {
            if ($("#CreatReportRightPortion").css("display") == "none") {
                $("#CreatReportRightPortion").show("slide", { direction: 'right' }, 1000);
                $("#NewReportPopUpParent").css({ "width": "800px" });
                $("#CreatReportLeftPortion").css({ "width": "46%" });
            }
            else {
                $("#CreatReportRightPortion").hide("slide", { direction: 'left' }, 1000);
                $("#NewReportPopUpParent").css({ "width": "400px" });
                $("#CreatReportLeftPortion").css({ "width": "100%" });
            }
        },

        RemoveCreateReportPopUp: function (event) {
            $("#NewReportPopUpParent").slideUp("slow", function () {
                $("#NewReportPopUpParent").remove();
            });
        },

        SaveNewReport: function (event) {
            var self = this;
            var ReportInfo = {};
            ReportInfo.ReportName = $("#toolKitReportNameText").text().trim();
            toolKit.reportName = ReportInfo.ReportName;
            if ($(".toolKitCalendarNameText").html() != "") {
                ReportInfo.CalendarName = $(".toolKitCalendarNameText").html();
            }
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            if (!self.IsReportNameEmptyorDuplicated($("#toolKitReportNameText").text().trim())) {
                if ($("#toolKitExistingReportDropDownFirstDiv").html() != "None" && $("#toolKitExistingReportTabsDropDownParent").html() != "None") {
                    $.ajax({
                        url: url + '/CloneTabs',
                        type: 'POST',
                        contentType: "application/json",
                        dataType: 'json',
                        data: JSON.stringify({ oldReportName: $(".toolKitExistingReportDropDownFirstDiv").html(), tabs: self.getTabs(), newReportName: $("#toolKitReportNameText").text().trim(), reportDesc: $("#toolKitReportDescText").text(), user: iago.user.userName, moduleId: toolKit.moduleid })
                    }).then(function (responseText) {
                        $("#" + toolKit.identifier).empty();
                        $("#" + toolKit.identifier).append("<div id=\"" + toolKit.tabContainer + "\"></div>");
                        toolKit.identifierForReport = "";
                        if ($(".iago-page-title").find(".toolKitAddTab").length == 0) {
                            if (!toolKit.ShowReport)
                                $(".iago-page-title").append("<div class=\"fa fa-plus-circle toolKitAddTab\" aria-hidden=\"true\"></div>");
                        }
                        toolKit.handlers.getToolKItInfo();
                    });
                }
                else {
                    ReportInfo.ReportName = $("#toolKitReportNameText").text().trim();
                    toolKit.reportName = ReportInfo.ReportName;
                    ReportInfo.ReportDesc = $("#toolKitReportDescTextEditable").val();
                    ReportInfo.DataBaseName = toolKit.dbIdentifier;
                    var ReportString = JSON.stringify(ReportInfo);
                    $.ajax({
                        url: url + '/CreateReport',
                        type: 'POST',
                        contentType: "application/json",
                        dataType: 'json',
                        data: JSON.stringify({ reportInfo: ReportString, user: iago.user.userName, moduleId: toolKit.moduleid })
                    }).then(function (responseText) {
                        //toolKit.ExistingReportsCollection = JSON.parse(responseText.d);
                        if (responseText.d) {
                            toolKit.reportInfo = ReportInfo;
                            toolKit.reportInfo.TabRelationInfo = [];
                            toolKit.reportInfo.reportIdentifier = toolKit.reportName + iago.user.userName + toolKit.moduleid;
                            $("#" + toolKit.identifier).empty();
                            toolKit.addTabInfo.createPopUp();
                        }
                    });
                }
            }
            else {
                $("#rptNameValidation").removeClass("hiddenClass");
            }
        },

        getTabs: function () {
            var self = this;
            var tabs = [];
            var selectedTabs = [];
            selectedTabs.push($("#toolKitExistingReportTabsDropDownParent").html());
            var reportName = $(".toolKitExistingReportDropDownFirstDiv").html();
            for (var key in toolKit.ExistingReportsCollection) {
                if (key == reportName) {
                    var tabsArray = toolKit.ExistingReportsCollection[key];
                    for (var j = 0; j < tabsArray.length; j++) {
                        if (selectedTabs.indexOf(tabsArray[j].TabName) > -1)
                            tabs.push({ Key: tabsArray[j].tabId, Value: tabsArray[j].TabName });
                    }
                    break;
                }
            }
            return tabs;
        },

        IsReportNameEmptyorDuplicated: function (reportName) {
            var self = this;
            if (reportName == "" || reportName == null) {
                return true;
            }
            else {
                if (toolKit.ReportsCollection != null) {
                    for (var i = 0; i < toolKit.ReportsCollection.length; i++) {
                        if (toolKit.ReportsCollection[i].ReportName.trim().toLowerCase() == reportName.trim().toLowerCase()) {
                            return true;
                        }
                    }
                }
                else
                    return false;
            }
            return false;
        },

        ExpandDropDownInReportPopUp: function (event) {
            var self = this;
            if ($(event.target).hasClass("toolKitExistingReportDropDownFirstDiv")) {
                var div = $("<div>");
                div.addClass("reportCollection");
                for (var i = 0; i < toolKit.ReportsCollection.length; i++) {
                    div.append("<div class='reportCollectionItem'>" + toolKit.ReportsCollection[i].ReportName + "</div>");
                }
                $(".toolKitExistingReportDropDownParent").append(div);
            }
            else if ($(event.target).hasClass("reportCollectionItem")) {
                $(".toolKitExistingReportDropDownFirstDiv").html($(event.target).html());
                var reportName = $(event.target).html();
                $(".reportCollection").remove();

            }
        },

        SetReportNameInDropDown: function (event) {
            var self = this;
            $("#toolKitExistingReportDropDownFirstDiv").text($(event.target).text());
            $(event.target).closest(".toolKitExistingReportDropDownParent").height(20);
            $("#toolKitExistingReportTabsDropDownParent").append("<div id=\"toolKitExistingReportTabDropDownFirstDiv\" class=\"toolKitExistingReportTabDropDownFirstDiv\"></div>")
            $("#toolKitExistingReportTabsDropDownParent").append("<div id=\"toolKitExistingReportTabDropDownOptions\" class=\"toolKitExistingReportTabDropDownOptions\"></div>")

            for (var value in toolKit.ExistingReportsCollection[$(event.target).text()]) {
                $("#toolKitExistingReportTabDropDownOptions").append("<div id=\"toolKitExistingReportEachDropDownOptions\" class=\"toolKitExistingReportEachDropDownOptions\">" + toolKit.ExistingReportsCollection[$(event.target).text()][value] + "</div>")
            }
            $(".toolKitExistingReportTabDropDownFirstDiv").unbind("click").click(function (event) {
                self.ExpandTabDropDownInReportPopUp(event);
            });
        },

        ExpandTabDropDownInReportPopUp: function (event) {
            var self = this;
            if ($(event.target).closest(".toolKitExistingReportTabsDropDownParent").height() == 20) {
                $(event.target).closest(".toolKitExistingReportTabsDropDownParent").height(100);
            }
            else {
                $(event.target).closest(".toolKitExistingReportTabsDropDownParent").height(100);

            }
        },

        userMapClickHandler: function (event) {
            var self = this;

            if (event.target.className == "toolKitExistingUsersDropDown") {
                $(event.target).addClass("selected");
                $($(event.target).closest(".toolKitExistingUser")).addClass("selected");
            }
            else if (event.target.className == "toolKitExistinUsersLabel UserLabelSelectedhover" || event.target.className == "toolKitExistinUsersLabel UserLabelSelected") {
                if ($($(event.target).closest(".toolKitExistingUser").find(".toolKitExistingUsersDropDown")).attr('admin') == "false") {
                    $($(event.target).closest(".toolKitExistingUser").find(".toolKitExistingUsersDropDown")).attr('admin', 'true');
                    //$(event.target).css("background-color", "#fafff0");
                    if ($($(event.target).closest(".toolKitExistingUser").find(".toolKitExistingUsersDropDown")).hasClass("toolKitExistingUsersDropDown")) {
                        $($(event.target).closest(".toolKitExistingUser").find(".toolKitExistingUsersDropDown")).addClass("selected");
                        $($(event.target).closest(".toolKitExistingUser").find(".toolKitExistinUsersLabel")).addClass("UserLabelSelected");
                    }
                }
                else {
                    $($(event.target).closest(".toolKitExistingUser").find(".toolKitExistingUsersDropDown")).attr('admin', 'false');
                    $($(event.target).closest(".toolKitExistingUser").find(".toolKitExistinUsersLabel")).removeClass("UserLabelSelected");
                    //$(event.target).css("background-color", "aliceblue");
                }
            }
            else if (event.target.className == "toolKitExistingUsersDropDown selected") {
                $(event.target).removeClass("selected");
                $($(event.target).closest(".toolKitExistingUser")).removeClass("selected");
                if ($(event.target).attr('admin') == "true") {
                    $(event.target).attr('admin', 'false');
                    $($(event.target).closest(".toolKitExistingUser").find(".toolKitExistinUsersLabel")).css("background-color", "aliceblue");
                }
            }
            else if (event.target.className == "toolKitExistingUserSaveBtn") {
                var selectedValues = "";
                var x = 0;
                if ($($('div[class="toolKitExistingUsersDropDown selected"]')[0]).attr('admin') == "true") {
                    x = 1;
                }
                else {
                    x = 0;
                }
                var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
                //                $(".toolKitExistingUser selected>div").each(function () {
                //                    if ($(this).className == "toolKitExistingUsersDropDown selected") {
                //                        if (x == 0) {
                //                            selectedValues = $(this).attr('title') + ~ ~ +$(this).attr('admin');
                //                            x++;
                //                        }
                //                        else {
                //                            selectedValues = selectedValues + ',' + $(this).attr('title') + ~ ~ +$(this).attr('admin');
                //                        }
                //                    }
                //                });
                if ($('div[class="toolKitExistingUsersDropDown selected"]').length) {
                    selectedValues = $($('div[class="toolKitExistingUsersDropDown selected"]')[0]).attr('title') + "~~" + x;
                }
                for (var i = 1; i < $('div[class="toolKitExistingUsersDropDown selected"]').length; i++) {
                    if ($($('div[class="toolKitExistingUsersDropDown selected"]')[i]).attr('admin') == "true") {
                        x = 1;
                    }
                    else {
                        x = 0;
                    }
                    selectedValues = selectedValues + ',' + $($('div[class="toolKitExistingUsersDropDown selected"]')[i]).attr('title') + "~~" + x;
                }

                $.ajax({
                    url: url + '/AddUserMapping',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ reportIdentifier: self.shareRptIdentifier, user: iago.user.userName, userMapping: selectedValues })
                }).then(function (responseText) {
                    if (responseText.d == true) {
                        $("#toolKitUserMappingParent").remove();
                        $(".rightSideTip").remove();
                        var domEle = $(".headerReportName").find("div:contains('" + toolKit.reportName + "')").parent();
                        var selectedVal = (selectedValues.trim() == "" ? [] : selectedValues.trim().split(","));
                        var list = "";
                        var obj = [];
                        if (selectedVal.length > 0) {
                            for (var i = 0; i < selectedVal.length; i++) {
                                if (i < 2) {
                                    //obj.push({ key: selectedVal[i].split("~~")[0], value: (selectedVal[i].split("~~")[1] == "1" ? true : false) })
                                    obj[selectedVal[i].split("~~")[0]] = (selectedVal[i].split("~~")[1] == "1" ? true : false);
                                    if (i != 0)
                                        list += ("," + selectedVal[i].split("~~")[0]);
                                    else
                                        list += selectedVal[i].split("~~")[0];
                                }
                                else {
                                    //obj.push({ key: selectedVal[i].split("~~")[0], value: (selectedVal[i].split("~~")[1] == "1" ? true : false) })
                                    obj[selectedVal[i].split("~~")[0]] = (selectedVal[i].split("~~")[1] == "1" ? true : false);
                                }
                            }
                        }
                        if (selectedVal.length > 2)
                            list += (" " + selectedVal.length - 2 + "more");
                        if (selectedVal.length > 0)
                            domEle.closest('tr').find(".toolkitaddusermappingNew").html(list);
                        else
                            domEle.closest('tr').find(".toolkitaddusermappingNew").html("AddUser");

                        self.viewModelReport.ReportList()[self.index].UserMapping = obj;
                    }
                    else {
                    }
                });
            }
            else if (event.target.className == "toolKitExistingUserCancelBtn") {
                $("#toolKitUserMappingParent").remove();
            }
        },

        ReportUserHandler: function (model, event) {
            var self = this;
            $(".CalendarListMainParent").remove();
            self.shareRptIdentifier = $(event.target).closest('tr').attr("identifier");
            toolKit.reportName = $(event.target).closest(".columnBodySectionReport").find(".addedReportName").html();
            $(".rightSideTip").remove();
            $("#toolKitUserMappingParent").remove();
            var reportElement = $(event.target).closest(".columnBodySectionReport").find(".addedReportMappedUser");
            //$(event.target).closest(".columnBodySectionReport").find(".addedReportMappedUser").append("<div class=\"rightSideTip\"><div class=\"rightSideTipInside\"></div></div>")
            var self = this;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + "/GetHtmlTemplate",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.UserMapping.htm' })
            }).then(function (responseText) {
                //toolKit.tabContentContainer.prepend(responseText);
                $("#" + toolKit.identifier).prepend(responseText.d);
                $("#toolKitUserMappingParent").slideDown("slow", function () {
                    $("#toolKitUserMappingParent").css({ "display": "" });
                    reportElement.append("<div class=\"rightSideTip\"><div class=\"rightSideTipInside\"></div></div>");
                });
                if (($(event.target).offset().top + 330 - 55) > $(".toolKitAddReports").height()) {
                    var top = ($(event.target).offset().top + 330 - 55) - $(".toolKitAddReports").height();
                    $("#toolKitUserMappingParent").css({ "top": ($(event.target).offset().top - 61 - top + "px") });
                }
                else {
                    $("#toolKitUserMappingParent").css({ "top": ($(event.target).offset().top - 61 + "px") });
                }

                //                $("#toolKitUserMappingParent").css({ "left": ($(event.target).offset().left - 30 + "px") });
                $("#toolKitUserMappingParent").css({ "left": (($(event.target).offset().left + reportElement.width()) + "px") });
                $("#toolKitUserMappingParent").unbind("click").click(function (event) {
                    self.userMapClickHandler(event);
                })

                var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
                $.ajax({
                    url: url + '/GetAllUsers',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json'
                }).then(function (responseText) {
                    var index = $(event.target).closest('tr').index();
                    self.index = index;
                    var user = Object.keys(self.viewModelReport.ReportList()[index].UserMapping);
                    //                    var selectedUsers = "";
                    //                    for (var i = 0; i < user.length; i++) {
                    //                        selectedUsers += user[i] + (i == user.length - 1 ? "" : ",");
                    //                    }
                    var ListOfUsers = JSON.parse(responseText.d);
                    var UserList = [];
                    for (var i = 0; i < ListOfUsers.length; i++) {
                        var x = 0;
                        for (var j = 0; j < user.length; j++) {
                            if (ListOfUsers[i] == user[j]) {
                                if (self.viewModelReport.ReportList()[index].UserMapping[user[j]] == true) {
                                    $("#toolKitExistingUsersParent").append('<div class="toolKitExistingUser selected IsAdmin"><div class="toolKitExistingUsersDropDown selected" id ="toolKitExistingUsersDropDown" admin = "true" title =' + ListOfUsers[i] + ' >' + ListOfUsers[i] + '</div><div id="toolKitExistingUsersLabel" class="toolKitExistinUsersLabel UserLabelSelected" style="background-color:#fafff0;">admin</div></div>');
                                }
                                else {
                                    $("#toolKitExistingUsersParent").append('<div class="toolKitExistingUser selected"><div class="toolKitExistingUsersDropDown selected" id ="toolKitExistingUsersDropDown" admin = "false" title =' + ListOfUsers[i] + ' >' + ListOfUsers[i] + '</div><div id="toolKitExistingUsersLabel" class="toolKitExistinUsersLabel">admin</div></div>');
                                }
                                x++;
                                break;
                            }
                        }
                        if (x == 0) {
                            $("#toolKitExistingUsersParent").append('<div class="toolKitExistingUser"><div class="toolKitExistingUsersDropDown" id ="toolKitExistingUsersDropDown" admin = "false" title =' + ListOfUsers[i] + ' >' + ListOfUsers[i] + '</div><div id="toolKitExistingUsersLabel" class="toolKitExistinUsersLabel">admin</div></div>');
                        }
                    }
                    //                    $("#toolKitExistingUsersDropDown").select2({ data: UserList, multiple: true, placeholder: "Existing Users" });
                    //                    $("#toolKitExistingUsersDropDown").select2("val", user);
                    self.AdduserMappingPopUpHandler();
                });
            });
        },

        AdduserMappingPopUpHandler: function () {
            var self = this;
            $(".toolKitExistingUserCancelBtn").unbind("click").click(function (event) {
                $(".rightSideTip").remove();
                self.RemoveUserMappingPopUp(event);
            });
            $(".toolKitExistingUsersDropDown,.toolKitExistinUsersLabel").hover(function (event) {
                if ($(event.target).closest(".toolKitExistingUser").find(".toolKitExistingUsersDropDown").attr('admin') != "true") {
                    $(event.target).closest(".toolKitExistingUser").find(".toolKitExistinUsersLabel").addClass("UserLabelSelectedhover");
                }
            }, function (event) {
                $(event.target).closest(".toolKitExistingUser").find(".toolKitExistinUsersLabel").removeClass("UserLabelSelectedhover");
                //                if ($(event.target).closest(".toolKitExistingUser").find(".toolKitExistingUsersDropDown").attr('admin') != "true") {
                //                    $(event.target).closest(".toolKitExistingUser").find(".toolKitExistinUsersLabel").removeClass("UserLabelSelectedhover");
                //                }
            });
        },


        RemoveUserMappingPopUp: function () {
            $("#toolKitUserMappingParent").slideUp("slow", function () {
                $("#toolKitUserMappingParent").remove();
            });
        },


        //Click Event for User Mapping to open Pop Up
        ReportUserHandler: function (model, event) {
            var self = this;
            $(".CalendarListMainParent").remove();
            self.shareRptIdentifier = $(event.target).closest('tr').attr("identifier");
            toolKit.reportName = $(event.target).closest(".columnBodySectionReport").find(".addedReportName").html();
            $(".rightSideTip").remove();
            $("#toolKitUserMappingParent").remove();
            var reportElement = $(event.target).closest(".columnBodySectionReport").find(".addedReportMappedUser");
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            self.userAdded = [];
            $.ajax({
                url: url + "/GetHtmlTemplate",
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.UserMapping.htm' })
            }).then(function (responseText) {
                bootbox.dialog({
                    message: responseText.d
                });
                $(".toolKitShareUserParent").closest(".modal-content").addClass("toolKitShareUserDialog");

                var index = $(event.target).closest('tr').index();
                self.index = index;
                //self.sharedUsersList = Object.keys(self.viewModelReport.ReportList()[index].UserMapping);
                self.sharedUsersList = self.viewModelReport.ReportList()[index].UserMapping;
                self.sharedUserNamesList = [];



                var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
                $.ajax({
                    url: url + '/GetAllUsers',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json'
                }).then(function (responseText) {
                    self.listOfUsers = $.parseJSON(responseText.d);
                    self.createSharedUserDiv(self.sharedUsersList,'',false);
                    $(".toolKitShareUserParent").click(function (event) {
                        self.shareUserDivClickHandler(event);
                    });
                    $(".toolKitAddUserOrGroup").keyup(function (event) {
                        self.createAddShareUserPopUp(event);
                    });
                });
            });
        },

        //Create Pop Up of Users and Groups on KeyDown of text box
        createAddShareUserPopUp: function (event) {
            var self = this;
            if (event.keyCode == 40) {
                $(".toolKitUserNGroupPopUp").find(".toolKitUserNGroupChild").first().addClass("toolKitHightlight").focus();
            }
            else if (event.keyCode == 8 && $(".toolKitAddUserOrGroup").val().trim() == "") {
                self.deleteUserOnBackspace(event);
            }
            else {
                var div = null;
                $(".toolKitUserNGroupPopUp").remove();
                div = $("<div>", {
                    class: 'toolKitUserNGroupPopUp'
                });
                var child = $("<div>", {
                    class: 'toolKitUserNGroupPopUpChild'
                });
                div.append(child);

                for (var o in self.listOfUsers) {
                    if (self.listOfUsers.hasOwnProperty(o)) {
                        if (self.userAdded.indexOf(o) == -1 && self.sharedUserNamesList.indexOf(o) == -1) {
                            if (o.toLowerCase().indexOf($(event.target).val().trim().toLowerCase()) != -1) {
                                var divParent = $("<div>", {
                                    class: 'toolKitUserNGroupParent'
                                });

                                var divChild = $("<div>", {
                                    class: 'toolKitUserNGroupChild',
                                    text: o,
                                    tabIndex: 1,
                                    UserorGroup: self.listOfUsers[o]
                                });
                                divParent.append(divChild);
                                child.append(divParent)
                            }
                        }
                    }
                }

                //for (var i = 0; i < self.listOfUsers.length; i++) {
                //    if (self.userAdded.indexOf(self.listOfUsers[i]) == -1 && self.sharedUsersList.indexOf(self.listOfUsers[i]) == -1) {
                //        if (self.listOfUsers[i].toLowerCase().indexOf($(event.target).val().trim().toLowerCase()) != -1) {
                //            var divParent = $("<div>", {
                //                class: 'toolKitUserNGroupParent'
                //            });

                //            var divChild = $("<div>", {
                //                class: 'toolKitUserNGroupChild',
                //                text: self.listOfUsers[i],
                //                tabIndex: 1
                //            });
                //            divParent.append(divChild);
                //            child.append(divParent)
                //        }
                //    }
                //}
                $(".toolKitShareText").append(div);
                $(".toolKitUserNGroupPopUp").css({ 'left': ($(".toolKitAddUserOrGroup").position().left + 'px') });
                $(".toolKitUserNGroupPopUp").unbind("keydown").keydown(function (event) {
                    self.keyupEventHandlerforUserPopUp(event);
                });
                //$(".toolKitUserNGroupPopUp").find(".toolKitUserNGroupChild").first().addClass("toolKitHightlight").focus();
            }
        },


        deleteUserOnBackspace:function(event) {
            var self = this;
            if ($(".toolKitAddSharedEntText").length > 0) {
                var text = $(".toolKitAddSharedEntText").last().html().trim();
                $(".toolKitAddSharedEntParent").last().remove();
                self.userAdded.splice(self.userAdded.indexOf(text), 1);
                var width = $(".toolKitShareText").width() - self.getAllUserDivWidth();
                if (width > 150)
                    $(".toolKitAddUserOrGroup").css({ 'width': (width + 'px') });
                else
                    $(".toolKitAddUserOrGroup").css({ 'width': "100%" });
            }
        },

        keyupEventHandlerforUserPopUp: function (event) {
            var self = this;
            if (event.keyCode == 9) {
                self.createSharedUserDivToAdd(event);
                $(".toolKitAddUserOrGroup").focus();
            }
            else if (event.keyCode == 40) {
                if ($(event.target).parent().next().length > 0) {
                    $(event.target).removeClass("toolKitHightlight");
                    $(event.target).parent().next().children().first().addClass("toolKitHightlight").focus();
                    var scroll = $(".toolKitUserNGroupPopUpChild").scrollTop();
                    $(".toolKitUserNGroupPopUpChild").scrollTop(scroll + 20);
                    event.preventDefault();
                    return false;
                }
            }
            else if (event.keyCode == 38) {
                if ($(event.target).parent().prev().length > 0) {
                    $(event.target).removeClass("toolKitHightlight");
                    $(event.target).parent().prev().children().first().addClass("toolKitHightlight").focus();
                    var scroll = $(".toolKitUserNGroupPopUpChild").scrollTop();
                    $(".toolKitUserNGroupPopUpChild").scrollTop(scroll - 20);
                    event.preventDefault();
                    return false;
                }
            }
            
        },

        //Click Handler on User Pop Up
        shareUserDivClickHandler: function (event) {
            var self = this;
            if ($(event.target).hasClass("toolKitUserNGroupChild")) {
                self.createSharedUserDivToAdd(event);
            }
            else if ($(event.target).hasClass("toolKitAddSharedEntDeleteText")) {
                self.deleteUserAddedInTextBox(event);
            }
            else if ($(event.target).hasClass("toolKitSharedEntDelete")) {
                if ($(event.target).closest(".toolKitUnsavedUsers").length > 0)
                    $(event.target).closest(".toolKitUnsavedUsers").remove();
                else
                    $(event.target).closest(".toolKitSharedEntParent").addClass("toolKitDeleteSharedUser");
            }
            else if ($(event.target).hasClass("toolKitTagPriv")) {
                self.createPrivilegesDropDown(event, $(event.target).closest(".toolKitSharedEntParent").attr("privileges"));
            }
            else if ($(event.target).hasClass("toolKitcommondropdownELe")) {
                
                if ($(event.target).closest(".toolKitShareTextParent").length > 0) {
                    self.addRemovePrivilegesShared(event);
                }
                else {
                    $(event.target).closest(".toolKitSharedEntParent").addClass("toolKitUnsavedUsers");
                    self.addRemovePrivileges(event);
                }
            }
            else if ($(event.target).hasClass("toolKitAddUserBtn")) {
                //var users = [];
                //for (var i = 0; i < $(".toolKitAddSharedEntText").length; i++) {
                //    users.push($($(".toolKitAddSharedEntText")[i]).html().trim());
                //}
                //if (users.length > 0)
                //    self.createSharedUserDiv(users, "toolKitUnsavedUsers");
                //self.userAdded = [];
                //$(".toolKitAddSharedEntParent").remove();
                //$(".toolKitAddUserOrGroup").val("");
                if ($(event.target).closest(".toolKitShareTextParent").find(".toolKitAddSharedEntParent").length > 0)
                self.createPrivilegesDropDownOnShare(event, $(event.target).closest(".toolKitShareTextParent").attr("privileges"));
            }
            else if ($(event.target).hasClass("toolShareRptBtn")) {
                var users = [];
                for (var i = 0; i < $(".toolKitAddSharedEntText").length; i++) {
                    users.push({ Name: $($(".toolKitAddSharedEntText")[i]).html().trim() });
                }
                if (users.length > 0)
                    self.createSharedUserDiv(users, "toolKitUnsavedUsers",true);
                self.userAdded = [];
                $(".toolKitAddSharedEntParent").remove();
                $(".toolKitAddUserOrGroup").val("");
                self.AdduserMapping();
            }
            else {
                $(".toolKitUserNGroupPopUp").remove();
                $(".toolKitCommonClass").remove();
                $(".toolKitCommonClassPriv").remove();
            }
        },

        addRemovePrivilegesShared:function(event) {
            var self = this;
            var privileges = ''
            if ($(event.target).hasClass("toolKitcommondropdownELeSelected")) {
                $(event.target).removeClass("toolKitcommondropdownELeSelected");
                var collection = $(event.target).closest(".toolKitcommondropdownParent").find(".toolKitcommondropdownELeSelected");
                
                for (var i = 0; i < collection.length; i++) {
                    privileges = privileges == "" ? $(collection[i]).html() : privileges + ',' + $(collection[i]).html();
                }
                $(event.target).closest(".toolKitShareTextParent").attr("privileges", privileges);
            }
            else {
                privileges = $(event.target).closest(".toolKitShareTextParent").attr("privileges") == null ? "" : $(event.target).closest(".toolKitShareTextParent").attr("privileges");
                privileges = (privileges == "" ? $(event.target).html() : (privileges + ',' + $(event.target).html()));
                $(event.target).closest(".toolKitShareTextParent").attr("privileges", privileges);
                $(event.target).addClass("toolKitcommondropdownELeSelected");
            }
            if (privileges != '') {
                if (privileges.split(",").length > 1)
                    $(event.target).closest(".toolKitShareTextParent").find(".toolKitAddUserBtn").html(privileges.split(",")[0] + " " + (privileges.split(",").length - 1) + " More");
                else
                    $(event.target).closest(".toolKitShareTextParent").find(".toolKitAddUserBtn").html(privileges);
            }
            else {
                $(event.target).closest(".toolKitShareTextParent").find(".toolKitAddUserBtn").html("Add Privileges");
            }
        },

        addRemovePrivileges:function(event) {
            var self = this;
            var privileges = '';
            if ($(event.target).hasClass("toolKitcommondropdownELeSelected")) {
                $(event.target).removeClass("toolKitcommondropdownELeSelected");
                var collection = $(event.target).closest(".toolKitcommondropdownParent").find(".toolKitcommondropdownELeSelected");
                for (var i = 0; i < collection.length; i++) {
                    privileges = privileges == "" ? $(collection[i]).html() : privileges + ',' + $(collection[i]).html();
                }
                $(event.target).closest(".toolKitSharedEntParent").attr("privileges", privileges);
            }
            else {
                privileges = $(event.target).closest(".toolKitSharedEntParent").attr("privileges") == null ? "" : $(event.target).closest(".toolKitSharedEntParent").attr("privileges");
                privileges = (privileges == "" ? $(event.target).html() : (privileges + ',' + $(event.target).html() ));
                $(event.target).closest(".toolKitSharedEntParent").attr("privileges", privileges);
                $(event.target).addClass("toolKitcommondropdownELeSelected");
            }
      
        },

        //Create Privileges Drop Down
        createPrivilegesDropDown:function(event,sharedList) {
            var self = this;
            sharedList = sharedList == null ? [] : sharedList.split(",");
            var drpDownParent = toolKit.utility.createSimpleDropwDown(self.privilegesList,sharedList);
            $(event.target).closest(".toolKitSharedEntParent").append(drpDownParent);
            drpDownParent.addClass("toolKitCommonClass");
        },

        createPrivilegesDropDownOnShare: function (event,sharedList) {
            var self = this;
            sharedList = sharedList == null ? [] : sharedList.split(",");
            var drpDownParent = toolKit.utility.createSimpleDropwDown(self.privilegesList,sharedList);
            $(event.target).closest(".toolKitShareTextParent").append(drpDownParent);
            drpDownParent.addClass("toolKitCommonClassPriv");
        },

        //AdduserMapping 
        AdduserMapping:function() {
            var self = this;
            var x = 0;
            var selectedValues = [];
            for (var i = 0; i < $(".toolKitSharedEntParent").length; i++) {
                var obj = {};
                obj.Name = $($(".toolKitSharedEntParent")[i]).find(".toolKitSharedEntText").html().trim();
                if ($($(".toolKitSharedEntParent")[i]).attr("privileges") != null) {
                    var priv = $($(".toolKitSharedEntParent")[i]).attr("privileges").split(",");
                    for (var j = 0; j < priv.length; j++) {
                        obj[priv[j]] = true;
                    }
                }
                if ($($(".toolKitSharedEntParent")[i]).attr("isAdmin") == "true") {
                    obj.IsAdmin = true;
                }
                if ($($(".toolKitSharedEntParent")[i]).attr("UserorGroup") == "group") {
                    obj.IsGroup = true;
                }
                selectedValues.push(obj);
            }
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/AddUserMapping',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ reportIdentifier: self.shareRptIdentifier, user: iago.user.userName, userMapping: JSON.stringify(selectedValues) })
            }).then(function (responseText) {
                if (responseText.d == true) {
                    $("#toolKitUserMappingParent").remove();
                    $(".rightSideTip").remove();
                    var domEle = $(".headerReportName").find("div:contains('" + toolKit.reportName + "')").parent();
                    //var selectedVal = (selectedValues.trim() == "" ? [] : selectedValues.trim().split(","));
                    var selectedVal = selectedValues;
                    var list = "";
                    var obj = selectedValues;
                    if (selectedVal.length > 0) {
                        for (var i = 0; i < selectedVal.length; i++) {
                            if (i < 2) {
                                //obj.push({ key: selectedVal[i].split("~~")[0], value: (selectedVal[i].split("~~")[1] == "1" ? true : false) })
                                //obj[selectedVal[i].split("~~")[0]] = (selectedVal[i].split("~~")[1] == "1" ? true : false);
                                if (i != 0)
                                    list += ("," + selectedVal[i].Name);
                                else
                                    list += selectedVal[i].Name;
                            }
                        }
                    }
                    if (selectedVal.length > 2)
                        list += (" " + selectedVal.length - 2 + "more");
                    if (selectedVal.length > 0)
                        domEle.closest('tr').find(".toolkitaddusermappingNew").html(list);
                    else
                        domEle.closest('tr').find(".toolkitaddusermappingNew").html("AddUser");

                    self.viewModelReport.ReportList()[self.index].UserMapping = obj;
                }
            });
        },

        //Delete Added User on Click of Cross Icon
        deleteUserAddedInTextBox:function(event) {
            var text = $(event.target).prev().html().trim();
            $(event.target).closest(".toolKitAddSharedEntParent").remove();
            self.userAdded.splice(self.userAdded.indexOf(text), 1);
            var width = $(".toolKitShareText").width() - self.getAllUserDivWidth();
            if (width > 150)
                $(".toolKitAddUserOrGroup").css({ 'width': (width + 'px') });
            else
                $(".toolKitAddUserOrGroup").css({ 'width': "100%" });
        },


        // Create USer or Group DIv in Text Box on selection
        createSharedUserDivToAdd: function (event) {
            var self = this;
            var text = $(event.target).html().trim();
            self.userAdded.push(text);
            var divParent = $("<div>", {
                class: 'toolKitAddSharedEntParent',
                UserorGroup: $(event.target).attr("UserorGroup").trim(),
                UGName: text
            });
            var div = $("<div>", {
                class: 'toolKitAddSharedEntChild'
            });
            var divImg = $("<div>", {
                class: 'toolKitAddSharedEntImg fa fa-user'
            });
            var divtext = $("<div>", {
                class: 'toolKitAddSharedEntText',
                text:text
            });
            var deleteDiv = $("<div>", {
                class: 'toolKitAddSharedEntDeleteText fa fa-times'
            });
            div.append(divImg);
            div.append(divtext);
            div.append(deleteDiv);
            divParent.append(div)
            $(".toolKitAddUserOrGroup").before(divParent);
            $(".toolKitAddUserOrGroup").val("");
            $(".toolKitUserNGroupPopUp").remove();
            var width = $(".toolKitShareText").width() - self.getAllUserDivWidth();
            if(width > 150)
                $(".toolKitAddUserOrGroup").css({ 'width': (width + 'px') });
            else
                $(".toolKitAddUserOrGroup").css({ 'width': "100%" });

            $(".toolKitAddUserOrGroup").focus();
        },

        //get Width of All User Div in textbox
        getAllUserDivWidth:function(){
            var width = 0;
            var counter = 0;
            $(".toolKitAddSharedEntParent").each(function(index) {
                width += $(this).outerWidth();
                counter++;
                if (width > $(".toolKitShareText").width()) {
                    counter++;
                    width = $(this).outerWidth();
                }
            })
            return width + counter;
        },

        //Create section for ALready Shared Users or Groups.
        createSharedUserDiv: function (sharedUsers, className, addNew) {
            var self = this;
            for (var i = 0; i < sharedUsers.length; i++) {
                var div = $("<div>", {
                    class: 'toolKitSharedEntParent' + (className != null ? (' ' + className) : '')
                    
                });
                if (addNew) {
                    div.attr("UserorGroup", $(".toolKitShareText").find("div[UGName='" + sharedUsers[i].Name + "']").attr("UserorGroup"));
                    if ($(".toolKitShareTextParent").attr("privileges") != null)
                        div.attr("privileges", $(".toolKitShareTextParent").attr("privileges"));
                }
                else {
                    self.sharedUserNamesList.push(sharedUsers[i].Name);
                    var priv = '';
                    for (var o in sharedUsers[i]) {
                        if (o != 'Name' && o != 'IsGroup') {
                            if (sharedUsers[i][o]) {
                                priv += o + ',';
                            }
                        }
                    }
                    priv = priv.substr(0, priv.length - 1);
                    div.attr("UserorGroup", sharedUsers[i].IsGroup ? "group" : "user");
                    if (priv != "")
                        div.attr("privileges", priv);
                }
                var divParent = $("<div>", {
                    class: 'toolKitSharedTextParent'
                });
                var divImg = $("<div>", {
                    class: 'toolKitSharedEntImg fa fa-user'
                });
                divParent.append(divImg);

                divImg = $("<div>", {
                    class: 'toolKitSharedEntText',
                    text: sharedUsers[i].Name
                });
                divParent.append(divImg);
                div.append(divParent);
                
                var privParent = $("<div>", {
                    class: 'toolKitTagPrivParent'
                });

                var privdiv = $("<div>", {
                    class: 'toolKitTagPriv',
                    text:'Add Privileges'
                });
                privParent.append(privdiv);
               

                

                divImg = $("<div>", {
                    class: 'toolKitSharedEntDelete fa fa-times'
                });
                div.append(divImg);
                //divImg = $("<div>", {
                //    class: 'toolKitSharedEntPrivlege'
                //});
                
                //div.append(divImg);
                div.append(privParent);
               
                $(".toolKitShareSection").append(div);
            }
        }

    }
})

$.extend(toolKit, {
    PublishReport: {
        BuildLeftMenu: function (reportname, identifier) {
            var closure = this;
            closure.currentidentifier = identifier;
            closure.currentReportName = reportname;
            closure.EditFlag = false;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                url: url + '/GetLeftMenuJson',
                dataType: "json",
                data: JSON.stringify({ AssemblyName: toolKit.AssemblyName, className: toolKit.ClassName })
            }).then(function (responseText) {
                toolKit.LeftMenuJSON = $.parseJSON(responseText.d);
                toolKit.LeftMenuJSON = toolKit.LeftMenuJSON.MenuItems;
                $.ajax({
                    url: url + '/GetHtmlTemplate',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.LeftMenu.htm' })
                }).then(function (responseText) {
                    var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
                    closure.html = responseText.d;
                    $("#" + toolKit.identifier).append(responseText.d);
                    $("#toolkitLeftMenuBar").css("height", $("#" + toolKit.identifier).height() + 60);
                    $("#toolkitLeftMenu").css("height", $("#" + toolKit.identifier).height() + 60);
                    $(".LeftMenuBar").css("height", $("#" + toolKit.identifier).height() + 20);
                    $.ajax({
                        type: 'POST',
                        contentType: "application/json",
                        dataType: 'json',
                        url: url + '/GetAllPublishPathForUser',
                        data: JSON.stringify({ user: iago.user.userName, moduleId: toolKit.moduleid })
                    }).then(function (response) {
                        for (var i = 0; i < response.d.length; i++) {
                            if (response.d[i].Key == closure.currentReportName) {
                                closure.EditFlag = true;

                                var publishfag = response.d[i].Value.split("|");
                                break;
                            }
                            else {
                                closure.EditFlag = false;
                            }
                        }
                        closure.BuildMenu();
                        closure.addToLeftMenuJson(response.d);

                        if (closure.EditFlag == true) {
                            closure.UpdatedPath = response.d[i].Value;
                            var length = publishfag.length;
                            closure.PublishFunc(closure.ToolkitLeftMenuTabs.MenuItems(), publishfag, length, 0, "");
                        }



                        //closure.BuildMenu();
                    })
                })
            });
        },
        PublishFunc: function (child, event, len, currentLevel, path) {
            var self = this;
            var childArray = [];
            for (var i = 0; i < child.length; i++) {
                if (child[i].Text == event[0].split("~~")[0]) {
                    childArray = child[i].Children;
                    //                    $(".Data_Show").each(function () {
                    //                        if ($(this).html() == event[currentLevel]) {
                    //                            var target = $(this);
                    //                        }
                    //                    })
                    for (var j = 0; j < $(".Data_Show").length; j++) {
                        if ($(".Data_Show")[j].innerText == event[0].split("~~")[0]) {
                            var target = $(".Data_Show")[j];
                        }
                    }
                    self.a = new self.LeftTab();
                    for (var i = 0; i < childArray.length; i++) {
                        self.a.MenuItems.push(self.getNewMenuObj(childArray[i]));
                        if (self.a.MenuItems()[i].hasOwnProperty('IsNewAdded') == false)
                            self.a.MenuItems()[i].IsNewAdded = false;
                    }

                    currentLevel++;
                    self.a.currentLevel(currentLevel);
                    if ($(target).closest(".TollKit_Report").length > 0) {
                        $(target).closest(".TollKit_Report").append($(self.html).find(".LeftMenuBarParent").clone(true));
                        $(target).closest(".LeftMenuBar").find(".LeftMenuBarParent").css("top", $(target).position().top - 6);
                        $(target).closest(".TollKit_Report").find(".LeftMenuBar").parent().addClass("child");
                        $(target).closest(".TollKit_Report").children(".delete_div").hide();
                        if (path == "") {
                            path = path + event[0];
                        }
                        else {
                            path = path + "|" + event[0];
                        }
                        $(target).closest(".LeftMenuBar").find(".LeftMenuBar").attr('path', path);
                        ko.applyBindings(self.a, $(target).closest(".TollKit_Report").find(".LeftMenuBarParent")[0]);
                    }
                    else {
                        $(target).closest(".Data_left_menu").append($(self.html).find(".LeftMenuBarParent").clone(true));
                        $(target).closest(".Data_left_menu").find(".LeftMenuBar").parent().addClass("child");
                        $(target).closest(".LeftMenuBar").find(".LeftMenuBarParent").css("top", $(target).position().top - 6);
                        if (path != "")
                            path = path + "|" + event[0];
                        else
                            path = event[0];
                        $(target).closest(".LeftMenuBar").find(".LeftMenuBar").attr('path', path);
                        ko.applyBindings(self.a, $(target).closest(".Data_left_menu").find(".LeftMenuBarParent")[0]);
                    }
                    if (self.reportIsAdeded || self.EditFlag == true) {
                        $(".AddNewReport").hide();
                        $(".addReportName").css("display", "none");
                    }
                    if (len == 3) {
                        //                        $(".delete_div").hide();
                        //                        if ($(target).parent().hasClass("Folder")) {
                        //                            for (var i = 0; i < $(target).parent().find(".Data_Show").length; i++) {
                        //                                if ($($(target).parent().find(".Data_Show")[i]).html() == event[1]) {
                        //                                    $($(target).parent().find(".Data_Show")[i]).siblings(".delete_div").show();
                        //                                }

                        //                            }
                        //                        }
                        $($(target).closest(".Data_left_menu").find(".LeftMenuBar")[0]).sortable({
                            start: function (e, ui) {
                                // creates a temporary attribute on the element with the old index
                                $(ui.item).attr('data-previndex', ui.item.index());
                            },
                            cancel: (".currentLevel" + $($(target).closest(".Data_left_menu").find(".LeftMenuBar")[0]).attr('currentlevel') + ", .addReportName"),
                            update: function (e, ui) {
                                self.oldIndex = $(ui.item).attr('data-previndex');
                                self.newIndex = ui.item.index();
                                if ($(ui.item).children().find(".Data_Show").length > 0) {
                                    if ($(ui.item).children().find(".Data_Show").length) {
                                        self.UpdatedPath = $(ui.item).children().find(".LeftMenuBar").attr("path").split("~~")[0] + "~~" + (self.newIndex + 1) + "|" + $(ui.item).children().find(".Data_Show").text() + "|" + $(ui.item).children().find(".Data_Show").length;
                                    }
                                    else {
                                        self.UpdatedPath = $(ui.item).children().find(".LeftMenuBar").attr("path").split("~~")[0] + "~~" + (self.newIndex + 1)
                                    }
                                }
                                else {
                                    if ($(e.target).closest(".Data_left_menu").hasClass("Folder")) {
                                        if ($(e.target).closest(".LeftMenuBarParent").find(".LeftMenuBar").attr('path').split("|")[0] != "") {
                                            self.UpdatedPath = $(e.target).closest(".LeftMenuBarParent").find(".LeftMenuBar").attr('path') + "|" + $(ui.item).find(".Data_Show").html() + "|" + (self.newIndex + 1);
                                        }
                                        else {
                                            self.UpdatedPath = $(e.target).closest(".LeftMenuBarParent").find(".LeftMenuBar").attr('path').split("|")[1] + "|" + $(ui.item).find(".Data_Show").html() + "|" + (self.newIndex + 1);
                                        }
                                    }
                                    else {
                                        self.UpdatedPath = $($(e.target).closest(".LeftMenuBar")).attr('path') + "|" + $(ui.item).find(".Data_Show").html() + "|" + (self.newIndex + 1);
                                    }
                                }
                                self.pathFragments = self.UpdatedPath.split("|");
                                self.pathFragmentsLength = self.pathFragments.length;
                                if ($(e.target).closest(".Data_left_menu").hasClass("Folder")) {
                                    self.Update_Folder_On_Publish(self.ToolkitLeftMenuTabs.MenuItems, self.pathFragmentsLength, self.pathFragments, 0, true, self.oldIndex);
                                }
                                else {

                                    if ($(e.target).closest(".Data_left_menu").hasClass("Folder")) {
                                        self.Update_Folder_On_Publish(self.ToolkitLeftMenuTabs.MenuItems, self.pathFragmentsLength, self.pathFragments, 0, true, self.oldIndex);
                                    }
                                    else {
                                        self.SendItToTheRealFunctionWhichDoesTheWork(self.ToolkitLeftMenuTabs.MenuItems, self.pathFragmentsLength, self.pathFragments, 0, true, self.oldIndex);
                                    }
                                }
                            }
                        });
                        break;
                    }
                    else if (len > 3) {

                        event.splice(0, 1);
                        self.PublishFunc(childArray, event, event.length, currentLevel, path)
                        break;
                    }
                }

            }
        },
        addToLeftMenuJson: function (obj) {
            var closure = this;
            closure.counter = obj.length;

            for (var i = 0; i < closure.counter; i++) {
                closure.pathFragments = obj[i].Value.split("|");
                closure.pathFragmentsLength = closure.pathFragments.length;
                if (closure.pathFragments[0] == "") {
                    var data = { "MenuID": closure.pathFragments[2], "Text": closure.pathFragments[1], "Img": "",
                        "Href": "",
                        "Fragment": null,
                        "IgnorePriviledge": true,
                        "Children": [],
                        "IsNewAdded": false
                    };
                    if (closure.currentReportName == obj[i].Key) {
                        data.IsNewAdded = true;
                    }
                    if (!closure.ValidateReportName(toolKit.LeftMenuJSON, closure.pathFragments[1])) {
                        closure.ToolkitLeftMenuTabs.MenuItems.splice(closure.pathFragments[2] - 1, 1);
                        closure.ToolkitLeftMenuTabs.MenuItems.splice(closure.pathFragments[2] - 1, 0, data);
                    }
                    else {
                        closure.ToolkitLeftMenuTabs.MenuItems.splice(closure.pathFragments[2] - 1, 0, data);
                    }
                }
                else {
                    closure.SendItToTheRealFunctionWhichDoesTheWork(closure.ToolkitLeftMenuTabs.MenuItems, closure.pathFragmentsLength, closure.pathFragments, 0, false, 0, obj[i].Key);
                }
            }

        },

        SendItToTheRealFunctionWhichDoesTheWork: function (LeftMenuJSON, Length, Arr, cnt, flag, OldIndex, rptnme) {
            var self = this;
            if (ko.isObservable(LeftMenuJSON)) {
                var LeftMenuJSON1 = LeftMenuJSON()
            }
            else {
                var LeftMenuJSON1 = LeftMenuJSON;
            }
            self.c = 0;
            for (var j = 0; j < LeftMenuJSON1.length; j++) {
                if (Arr[cnt].split("~~")[1] == null) {
                    if (LeftMenuJSON1[j].Text == Arr[cnt]) {
                        cnt++;
                        var data = { "MenuID": Arr[Length - 1], "Text": Arr[Length - 2], "Img": "",
                            "Href": "",
                            "Fragment": null,
                            "IgnorePriviledge": true,
                            "Children": [],
                            "IsNewAdded": false
                        };
                        if (cnt == Length - 2) {
                            if (self.currentReportName == rptnme) {
                                data.IsNewAdded = true;
                            }
                            if (!self.ValidateReportName(LeftMenuJSON, Arr[Length - 2]))
                                LeftMenuJSON1[j].Children.splice(Arr[Length - 1] - 1, 1);
                            LeftMenuJSON1[j].Children.splice(Arr[Length - 1] - 1, 0, data);
                            break;
                        }
                        if (cnt == Length - 1) {
                            if (flag == true) {
                                LeftMenuJSON()[j].Children.splice(OldIndex, 1);
                                data.IsNewAdded = true;
                                LeftMenuJSON().splice(Arr[1] - 1, 0, data);
                                break;
                            }
                        }
                        cnt = self.SendItToTheRealFunctionWhichDoesTheWork(LeftMenuJSON1[j].Children, Length, Arr, cnt, flag, OldIndex, rptnme);
                        break;
                    }
                }
                else {
                    for (var i = 0; i < LeftMenuJSON1.length; i++) {
                        if (LeftMenuJSON1[i].Text == Arr[cnt].split("~~")[0]) {
                            if (flag == true) {
                                LeftMenuJSON1.splice(OldIndex - 1, 1);
                                LeftMenuJSON1.splice(Arr[cnt].split("~~")[1], 0, data)
                                data.IsNewAdded = true;
                                break;
                            }
                            else {
                                cnt = self.AddNewFolderOnPublish(LeftMenuJSON1[i].Children, Length, Arr, cnt, flag, OldIndex, rptnme, data)
                            }
                            self.c++;
                            break;
                        }
                    }
                    if (self.c == 0) {
                        var data = { "MenuID": Arr[cnt].split("~~")[1], "Text": Arr[cnt].split("~~")[0], "Img": "",
                            "Href": "",
                            "Fragment": null,
                            "IgnorePriviledge": true,
                            "Children": [],
                            "IsNewAdded": false,
                            "IsFolder": true
                        };
                        LeftMenuJSON.splice(Arr[cnt].split("~~")[1], 0, data)
                        cnt = self.AddNewFolderOnPublish(LeftMenuJSON1[Arr[cnt].split("~~")[1]].Children, Length, Arr, cnt, flag, OldIndex, rptnme, data)
                        break;

                    }
                    else {
                        break;
                    }

                }
            }
            if (cnt == Length - 3) {
                var data = { "MenuID": Arr[cnt].split("~~")[1], "Text": Arr[cnt].split("~~")[0], "Img": "",
                    "Href": "",
                    "Fragment": null,
                    "IgnorePriviledge": true,
                    "Children": [{ "MenuID": Arr[cnt].split("~~")[1] + "_" + Arr[cnt + 2], "Text": Arr[cnt + 1], "Img": "",
                        "Href": "",
                        "Fragment": null,
                        "IgnorePriviledge": true,
                        "Children": [],
                        "IsNewAdded": self.currentReportName == rptnme ? true : false,
                        "IsMyReport": true
                    }],
                    "IsNewAdded": false,
                    "IsFolder": true
                }
                LeftMenuJSON1.splice(Arr[cnt].split("~~")[1], 0, data);
                cnt++;
                return cnt;
            }

        },
        Update_Folder_On_Publish: function (LeftMenuJSON, Length, Arr, cnt, flag, OldIndex, rptnme) {
            var self = this;
            if (ko.isObservable(LeftMenuJSON)) {
                var LeftMenuJSON1 = LeftMenuJSON()
            }
            else {
                var LeftMenuJSON1 = LeftMenuJSON;
            }
            self.c = 0;
            for (var j = 0; j < LeftMenuJSON1.length; j++) {
                if (Arr[cnt].split("~~")[1] == null) {
                    if (LeftMenuJSON1[j].Text == Arr[cnt]) {
                        cnt++;
                        if (cnt == Length - 1) {
                            var obj = LeftMenuJSON1[OldIndex];
                            LeftMenuJSON1.splice(OldIndex, 1)
                            LeftMenuJSON1.splice(Arr[Length - 1], 0, obj);
                            break;
                        }
                        self.Update_Folder_On_Publish(LeftMenuJSON1[j].Children, Length, Arr, cnt, flag, OldIndex, rptnme);
                        break;
                    }
                }
                else {
                    for (var i = 0; i < LeftMenuJSON1.length; i++) {
                        if (LeftMenuJSON1[i].Text == Arr[cnt].split("~~")[0]) {
                            self.c = self.c + 1;
                            self.Update_Folder_On_Publish(LeftMenuJSON1[i].Children, Length, Arr, cnt + 1, flag, OldIndex, rptnme);
                            break;
                        }
                    }
                    if (self.c == 0) {
                        if (Arr[cnt + 1] != null) {
                            var data = { "MenuID": Arr[cnt].split("~~")[1], "Text": Arr[cnt].split("~~")[0], "Img": "",
                                "Href": "",
                                "Fragment": null,
                                "IgnorePriviledge": true,
                                "Children": [{ "MenuID": Arr[cnt].split("~~")[1] + "_" + Arr[cnt + 2], "Text": Arr[cnt + 1], "Img": "",
                                    "Href": "",
                                    "Fragment": null,
                                    "IgnorePriviledge": true,
                                    "Children": [],
                                    "IsNewAdded": true,
                                    "IsMyReport": true
                                }],
                                "IsNewAdded": false,
                                "IsFolder": true

                            };
                        }
                        else {
                            var data = { "MenuID": Arr[cnt].split("~~")[1], "Text": Arr[cnt].split("~~")[0], "Img": "",
                                "Href": "",
                                "Fragment": null,
                                "IgnorePriviledge": true,
                                "Children": [],
                                "IsNewAdded": false,
                                "IsMyReport": true
                            };
                        }
                        if (cnt == 0) {
                            LeftMenuJSON1.splice(LeftMenuJSON1.length, 1)
                        }
                        LeftMenuJSON1.splice(Arr[cnt].split("~~")[1] - 1, 0, data)
                        break;
                    }
                }
            }
            if (cnt == Length - 2) {
                var data = { "MenuID": Arr[cnt - 1].split("~~")[1] + "_" + Arr[cnt + 1], "Text": Arr[cnt], "Img": "",
                    "Href": "",
                    "Fragment": null,
                    "IgnorePriviledge": true,
                    "Children": [],
                    "IsNewAdded": true,
                    "IsMyReport": true
                }
                LeftMenuJSON1.splice(Arr[Length - 1] - 1, 0, data);


            }

        },
        AddNewFolderOnPublish: function (LeftMenuJSON, Length, Arr, cnt, flag, OldIndex, rptnme, data) {
            var self = this;
            if (cnt == (Length - 3)) {
                LeftMenuJSON.splice(Arr[(Length - 1)] - 1, 0, {
                    "MenuID": Arr[cnt].split("~~")[1] + "_" + Arr[Length - 1],
                    "Text": Arr[Length - 2],
                    "Img": "",
                    "Href": "",
                    "Fragment": null,
                    "IgnorePriviledge": true,
                    "Children": [],
                    "IsNewAdded": self.currentReportName == rptnme ? true : false,
                    "IsMyReport": true

                });
                return (cnt + 1);
            }
            else {
                LeftMenuJSON.splice(Arr[cnt++].split("~~")[1] - 1, 0, {
                    "MenuID": Arr[cnt].split("~~")[1] + "_" + Arr[cnt++].split("~~")[1],
                    "Text": Arr[cnt++].split("~~")[0],
                    "Img": "",
                    "Href": "",
                    "Fragment": null,
                    "IgnorePriviledge": true,
                    "Children": [],
                    "IsNewAdded": false

                });
                self.SendItToTheRealFunctionWhichDoesTheWork(LeftMenuJSON[i].Children, Length, Arr, cnt, flag, OldIndex, rptnme);
                cnt++;
                return cnt;
            }
        },
        BuildMenu: function () {
            var closure = this;
            closure.reportIsAdeded = false;
            closure.UpdatedPath = "";
            closure.oldIndex = "";
            closure.newIndex = "";
            closure.reportname = "";
            closure.ToggleButton = false;
            closure.LeftTab = function () {
                var self = this;
                //                self.editFlag = ko.observable();
                //                if (closure.EditFlag == false) {
                //                    self.editFlag(false);
                //                }
                //                else {
                //                    self.editFlag(true);
                //                }
                self.MenuItems = ko.observableArray([]);
                self.ChildrenFunction = function (parent, model, event) {
                    if (model.IsMyReport) {
                        return false;
                    }
                    if ($(event.target).parent().hasClass("Folder")) {
                        closure.ToggleButton = true;
                    }
                    else {
                        closure.ToggleButton = false;
                    }
                    closure.AddOrRemoveChild(parent, model, event);
                }
                self.arrow = function (data) {
                    return "fa fa-angle-right leFtToggleArrow";
                }
                self.addreport = function (parent, model, event) {
                    closure.AddReportName(parent, model, event);
                    event.stopPropagation();
                    event.preventDefault();
                }

                self.clickMe = function (model, event) {
                    $(event.target).parent().css("display", "none");
                    $(event.target).closest(".LeftMenuBarParent").children(".addReportName").show();
                    if (closure.ToggleButton) {
                        $(event.target).closest(".LeftMenuBarParent").find(".addReportName").find(".Add_New_Folder").hide();
                        closure.ToggleButton = false;
                    }
                    event.stopPropagation();
                    event.preventDefault();
                }

                self.AddNewFolder = function (model, event) {
                    closure.ToggleButton = true;
                    $(event.target).css("color", "#43D9C6");
                    event.stopPropagation();
                    event.preventDefault();
                }
                self.deletereport = function (parent, model, event) {
                    closure.DeleteReportName(parent, model, event);
                }

                self.Sent_new_report_DB = function (model, event) {
                    closure.SaveReportName(model, event);
                }

                self.Remove_thePopUp = function (model, event) {
                    //                    $("#toolkitLeftMenu").slideUp("slow", function () {
                    //                        $("#toolkitLeftMenu").remove();
                    //                        $(".toolKitAddReports").css("opacity", "1");
                    //                    });
                    $('#toolkitLeftMenu').animate({
                        opacity: 0, // animate slideUp
                        marginLeft: '-200px'
                    }, 'slow', 'linear', function () {
                        $(this).remove();
                        $(".toolKitAddReports").css("opacity", "1");
                    });
                }
                self.availabilityCssClass = function (par, parent) {
                    if (par.IsFolder == true) {
                        return "Data_left_menu" + " currentLevel" + parent.currentLevel() + " Folder";
                    }
                    else if (par.IsNewAdded == true) {
                        return "TollKit_Report";
                    }
                    else {
                        return "Data_left_menu" + " currentLevel" + parent.currentLevel();
                    }
                }
                self.currentLevel = ko.observable()
            }

            closure.ToolkitLeftMenuTabs = new closure.LeftTab();
            closure.ToolkitLeftMenuTabs.currentLevel(0)
            for (var i = 0; i < toolKit.LeftMenuJSON.length; i++) {

                closure.ToolkitLeftMenuTabs.MenuItems.push(closure.getNewMenuObj(toolKit.LeftMenuJSON[i]));
                if (toolKit.LeftMenuJSON[i].IsNewAdded == true) {
                    closure.ToolkitLeftMenuTabs.MenuItems()[i].IsNewAdded = true;
                }
                else {
                    closure.ToolkitLeftMenuTabs.MenuItems()[i].IsNewAdded = false;
                }
            }
            $(".LeftMenuBar").sortable({
                start: function (e, ui) {
                    // creates a temporary attribute on the element with the old index
                    $(ui.item).attr('data-previndex', ui.item.index());
                },
                cancel: ".currentLevel0, .addReportName",
                update: function (e, ui) {
                    closure.oldIndex = $(ui.item).attr('data-previndex');
                    closure.newIndex = ui.item.index();

                    if ($(ui.item).children().find(".Data_Show").length > 0) {
                        if ($(ui.item).children().find(".Data_Show").length) {
                            closure.UpdatedPath = $(ui.item).children().find(".LeftMenuBar").attr("path").split("~~")[0] + "~~" + (closure.newIndex + 1) + "|" + $(ui.item).children().find(".Data_Show").text() + "|" + $(ui.item).children().find(".Data_Show").length;
                        }
                        else {
                            closure.UpdatedPath = $(ui.item).children().find(".LeftMenuBar").attr("path").split("~~")[0] + "~~" + (closure.newIndex + 1)
                        }
                    }
                    else {
                        if ($(e.target).closest(".Data_left_menu").hasClass("Folder")) {
                            if ($(e.target).closest(".LeftMenuBarParent").find(".LeftMenuBar").attr('path').split("|")[0] != "") {
                                closure.UpdatedPath = $(e.target).closest(".LeftMenuBarParent").find(".LeftMenuBar").attr('path') + "|" + $(ui.item).find(".Data_Show").html() + "|" + (closure.newIndex + 1);
                            }
                            else {
                                closure.UpdatedPath = $(e.target).closest(".LeftMenuBarParent").find(".LeftMenuBar").attr('path').split("|")[1] + "|" + $(ui.item).find(".Data_Show").html() + "|" + (closure.newIndex + 1);
                            }
                        }
                        else {
                            closure.UpdatedPath = $($(e.target).closest(".LeftMenuBar")).attr('path') + "|" + $(ui.item).find(".Data_Show").html() + "|" + (closure.newIndex + 1);
                        }
                    }
                    closure.pathFragments = closure.UpdatedPath.split("|");
                    closure.pathFragmentsLength = closure.pathFragments.length;
                    if ($(e.target).closest(".Data_left_menu").hasClass("Folder")) {
                        closure.Update_Folder_On_Publish(closure.ToolkitLeftMenuTabs.MenuItems, closure.pathFragmentsLength, closure.pathFragments, 0, true, closure.oldIndex);
                    }
                    else {
                        if ($(e.target).closest(".Data_left_menu").hasClass("Folder")) {
                            closure.Update_Folder_On_Publish(closure.ToolkitLeftMenuTabs.MenuItems, closure.pathFragmentsLength, closure.pathFragments, 0, true, closure.oldIndex);
                        }
                        else {
                            closure.SendItToTheRealFunctionWhichDoesTheWork(closure.ToolkitLeftMenuTabs.MenuItems, closure.pathFragmentsLength, closure.pathFragments, 0, true, closure.oldIndex);
                        }
                    }
                }
            });
            ko.applyBindings(closure.ToolkitLeftMenuTabs, $("#toolkitLeftMenu")[0]);
            //$(".LeftMenuBar").slimscrollNew({ height: '400px', applyborderRadius: true });
            if (closure.reportIsAdeded || closure.EditFlag == true) {
                $(".AddNewReport").hide();
                $(".addReportName").css("display", "none");
            }
        },

        LeftTabChildren: function (child, event, currentLevel) {
            // iss pe ata hain whwn u clicked on a tab having chilren 
            // then its bind a new viewmodel as a new  constructor
            var self = this;
            self.a = new self.LeftTab();
            if (child != null) {
                for (var i = 0; i < child.length; i++) {
                    self.a.MenuItems.push(self.getNewMenuObj(child[i]));
                    if (self.a.MenuItems()[i].hasOwnProperty('IsNewAdded') == false)
                        self.a.MenuItems()[i].IsNewAdded = false;
                }
            }
            currentLevel++;
            self.a.currentLevel(currentLevel);

            if (self.ToggleButton) {
                if ($(event).parent().hasClass("Folder")) {
                    $(event).closest(".Data_left_menu").append($(self.html).find(".LeftMenuBar").parent().clone(true));
                    ko.applyBindings(self.a, $(event).closest(".Data_left_menu").find(".LeftMenuBarParent")[0]);
                }
                else {
                    $(event).closest(".TollKit_Report").append($(self.html).find(".LeftMenuBar").parent().clone(true));
                    ko.applyBindings(self.a, $(event).closest(".TollKit_Report").find(".LeftMenuBarParent")[0]);
                    $(event).closest(".TollKit_Report").find(".LeftMenuBarParent").addClass("child");
                    $(event).closest(".TollKit_Report").find(".delete_div").hide();
                }

                if (self.EditFlag) {
                    $(event).closest(".TollKit_Report").find(".LeftMenuBarParent").find(".AddNewReport").hide();
                }
                if ($(event).parent().hasClass("Folder")) {
                    var path = $(event).closest(".LeftMenuBar").attr('path') + "|" + $(event).text() + "~~" + $(event).closest(".Folder").index();
                    $(event).closest(".Data_left_menu").find(".LeftMenuBar").attr("path", path);
                }
                else {
                    if ($(event).closest(".LeftMenuBar").attr('path') != "")
                        var path = $(event).closest(".LeftMenuBar").attr('path') + "|" + $(event).text() + "~~" + ($(event).closest(".LeftMenuBarParent").find(".Data_Show").length - 1);
                    else
                        var path = $(event).text() + "~~" + ($(event).closest(".LeftMenuBarParent").find(".Data_Show").length - 1);
                    $(event).closest(".TollKit_Report").find(".LeftMenuBar").attr("path", path);
                }

            }
            else {
                $(event.target).closest(".Data_left_menu").append($(self.html).find(".LeftMenuBar").parent().clone(true));
                ko.applyBindings(self.a, $(event.target).closest(".Data_left_menu").find(".LeftMenuBarParent")[0]);
            }
            if ((self.reportIsAdeded || self.EditFlag == true) && !self.ToggleButton) {
                $(".AddNewReport").hide();
                $(".addReportName").css("display", "none");
            }
            if (self.ToggleButton) {
                var dom = event
            }
            else {
                var dom = event.target;
            }
            $($(dom).closest(".Data_left_menu").find(".LeftMenuBar")[0]).sortable({
                start: function (e, ui) {
                    // creates a temporary attribute on the element with the old index
                    $(ui.item).attr('data-previndex', ui.item.index());
                },
                cancel: (".currentLevel" + $($(event.target).closest(".Data_left_menu").find(".LeftMenuBar")[0]).attr('currentlevel') + ", .addReportName"),
                update: function (e, ui) {
                    self.oldIndex = $(ui.item).attr('data-previndex');
                    self.newIndex = ui.item.index();
                    if ($(ui.item).children().find(".Data_Show").length > 0) {
                        if ($(ui.item).children().find(".Data_Show").length) {
                            self.UpdatedPath = $(ui.item).children().find(".LeftMenuBar").attr("path").split("~~")[0] + "~~" + (self.newIndex + 1) + "|" + $(ui.item).children().find(".Data_Show").text() + "|" + $(ui.item).children().find(".Data_Show").length;
                        }
                        else {
                            self.UpdatedPath = $(ui.item).children().find(".LeftMenuBar").attr("path").split("~~")[0] + "~~" + (self.newIndex + 1)
                        }
                    }
                    else {
                        if ($(e.target).closest(".Data_left_menu").hasClass("Folder")) {
                            if ($(e.target).closest(".LeftMenuBarParent").find(".LeftMenuBar").attr('path').split("|")[0] != "") {
                                self.UpdatedPath = $(e.target).closest(".LeftMenuBarParent").find(".LeftMenuBar").attr('path') + "|" + $(ui.item).find(".Data_Show").html() + "|" + (self.newIndex + 1);
                            }
                            else {
                                self.UpdatedPath = $(e.target).closest(".LeftMenuBarParent").find(".LeftMenuBar").attr('path').split("|")[1] + "|" + $(ui.item).find(".Data_Show").html() + "|" + (self.newIndex + 1);
                            }
                        }
                        else {
                            self.UpdatedPath = $($(e.target).closest(".LeftMenuBar")).attr('path') + "|" + $(ui.item).find(".Data_Show").html() + "|" + (self.newIndex + 1);
                        }
                    }
                    self.pathFragments = self.UpdatedPath.split("|");
                    self.pathFragmentsLength = self.pathFragments.length;
                    if ($(e.target).closest(".Data_left_menu").hasClass("Folder")) {
                        self.Update_Folder_On_Publish(self.ToolkitLeftMenuTabs.MenuItems, self.pathFragmentsLength, self.pathFragments, 0, true, self.oldIndex);
                    }
                    else {
                        if ($(e.target).closest(".Data_left_menu").hasClass("Folder")) {
                            self.Update_Folder_On_Publish(self.ToolkitLeftMenuTabs.MenuItems, self.pathFragmentsLength, self.pathFragments, 0, true, self.oldIndex);
                        }
                        else {
                            self.SendItToTheRealFunctionWhichDoesTheWork(self.ToolkitLeftMenuTabs.MenuItems, self.pathFragmentsLength, self.pathFragments, 0, true, self.oldIndex);
                        }
                    }
                }
            });
        },

        UpdateViewModel: function (leftMenuJSON, Items, flag) {
            var self = this;
            self.breakTheLoop = 0;

            for (var i = 0; i < leftMenuJSON.length; i++) {
                if (self.breakTheLoop > 0) {
                    break;
                }
                if (flag == false) {
                    if (leftMenuJSON[i].Text == Items) {
                        self.breakTheLoop = 1;
                        leftMenuJSON.splice(i, 1);
                        break;
                    }
                }
                else {
                    if (leftMenuJSON[i].Text == Items[0].Text) {
                        self.breakTheLoop = 1;
                        leftMenuJSON.push(Items[Items.length - 1]);
                        break;
                    }
                }

                if (leftMenuJSON[i].Children.length > 0) {
                    self.UpdateViewModel(leftMenuJSON[i].Children, Items, flag);
                }

            }


        },

        ValidateReportName: function (validateLeftMenuJson, reportName) {
            var self = this;
            self.x = 0;
            for (var i = 0; i < validateLeftMenuJson.length; i++) {
                if (self.x > 0) {
                    return false;
                }
                if (validateLeftMenuJson[i].Text == reportName) {
                    self.x = 1;
                    break;
                }
                if (validateLeftMenuJson[i].Children != null && validateLeftMenuJson[i].Children.length > 0) {
                    self.ValidateReportName(validateLeftMenuJson[i].Children, reportName);
                }

            }
            if (self.x > 0) {
                return false;
            }
            else {
                return true;
            }

        },

        DeleteReportName: function (parent, model, event) {
            var closure = this;
            closure.reportIsAdeded = false;
            toolKit.reportName = closure.currentReportName;
            if ($(event.target).closest(".LeftMenuBar").attr("path").indexOf("~~") != 1) {
                if ($(event.target).closest(".LeftMenuBar").children().length > 1) {
                    $(event.target).closest(".TollKit_Report").remove();
                }
                else {
                    $(event.target).closest(".LeftMenuBarParent").parent().remove();
                }
            }
            else {
                $(event.target).closest(".TollKit_Report").remove();
            }
            closure.UpdateViewModel(closure.ToolkitLeftMenuTabs.MenuItems(), $(event.target).parent().parent().find(".Data_Show").text(), false);
            //$(".addReportName").css("display", "inline");
            if (closure.EditFlag == true) {
                var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
                $.ajax({
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    url: url + '/DeleteReportPublished',
                    data: JSON.stringify({ reportName: closure.currentReportName }),
                    dataType: "json"
                }).then(function (responseText) {
                    $(".AddNewReport").show();
                    $(".addReportName").css("display", "none");
                    $('div.addedReportName:contains(' + toolKit.reportName + ')').closest(".columnBodySectionReport").find(".PublishedReport").hide();
                    if ($('div.addedReportName:contains(' + toolKit.reportName + ')').closest(".columnBodySectionReport").find(".PublishReprtIcon").find(".PublishTheReport").length == 0)
                        $('div.addedReportName:contains(' + toolKit.reportName + ')').closest(".columnBodySectionReport").find(".PublishReprtIcon").append("<div class = \"PublishTheReport\"><div class=\"PublishReport\" >Publish</div></div>");
                })
            }
            else {
                $(".AddNewReport").show();
                $(".addReportName").css("display", "none");
            }
            event.stopPropagation();
            event.preventDefault();
        },

        AddReportName: function (parent, model, event) {
            var closure = this;
            closure.count = 0;
            $(".addReportName").css("display", "none");
            closure.reportIsAdeded = true;
            closure.reportname = $(event.target).closest(".addReportName").find(".texthere").val();
            $(event.target).closest(".addReportName").find(".texthere").val("");
            if (closure.reportname != "") {
                var data = [{ "MenuID": parent.length, "Text": closure.reportname, "Img": "",
                    "Href": "",
                    "Fragment": null,
                    "IgnorePriviledge": true,
                    "Children": [],
                    "IsNewAdded": true
                }];
                parent.push(data[0]);
                //closure.UpdateViewModel(closure.ToolkitLeftMenuTabs.MenuItems(), parent(), true);
                if (closure.ToggleButton == true) {
                    var event1 = $(event.target).closest(".LeftMenuBarParent").find(".Data_Show")[$(event.target).closest(".LeftMenuBarParent").find(".Data_Show").length - 1];
                    closure.LeftTabChildren([], event1, $($(event.target).closest(".LeftMenuBarParent").find(".LeftMenuBar")[0]).attr("currentlevel"));
                }
                if ($(event.target).closest(".LeftMenuBarParent").find(".LeftMenuBar").attr('path').split("~~")[1] != null) {
                    if ($(event.target).closest(".Data_left_menu").hasClass("Folder")) {
                        if ($(event.target).closest(".LeftMenuBarParent").find(".LeftMenuBar").attr('path').split("|")[0] == "") {
                            closure.UpdatedPath = $(event.target).closest(".LeftMenuBarParent").find(".LeftMenuBar").attr('path').split("|")[1] + "|" + closure.reportname + "|" + ($($(event.target).closest(".LeftMenuBarParent")[0]).find(".Data_left_menu").length + $($(event.target).closest(".LeftMenuBarParent")[0]).find(".TollKit_Report").length);
                        }
                        else {
                            closure.UpdatedPath = $(event.target).closest(".LeftMenuBarParent").find(".LeftMenuBar").attr('path') + "|" + closure.reportname + "|" + ($($(event.target).closest(".LeftMenuBarParent")[0]).find(".Data_left_menu").length + $($(event.target).closest(".LeftMenuBarParent")[0]).find(".TollKit_Report").length);
                        }
                    }
                    else {

                        closure.UpdatedPath = $(event.target).closest(".LeftMenuBarParent").find(".LeftMenuBar").attr('path').split("~~")[0] + "~~" + $(event.target).closest(".LeftMenuBarParent").siblings(".Data_Show").parent().index() + "|" + closure.reportname + "|" + ($($(event.target).closest(".LeftMenuBarParent")[0]).find(".Data_left_menu").length + $($(event.target).closest(".LeftMenuBarParent")[0]).find(".TollKit_Report").length);

                    }
                }
                else {
                    closure.UpdatedPath = $(event.target).closest(".LeftMenuBarParent").find(".LeftMenuBar").attr('path') + "|" + closure.reportname + "|" + ($($(event.target).closest(".LeftMenuBarParent")[0]).find(".Data_left_menu").length + $($(event.target).closest(".LeftMenuBarParent")[0]).find(".TollKit_Report").length);
                }
                if (closure.ToggleButton == false) {
                    $(".AddNewReport").hide();
                    $(".addReportName").css("display", "none");
                }
                //                if (closure.ToggleButton == true) {
                //                    closure.ToggleButton = false;
                //                }
            }
            else if (closure.reportname == "" || closure.reportname == null) {

            }
            else {
                $(".addReportName").css("display", "inline");
            }
        },

        SaveReportName: function (model, event) {
            var closure = this;
            toolKit.reportName = closure.currentReportName;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                url: url + '/SaveReportPublishPath',
                data: JSON.stringify({ reportName: closure.currentReportName, identifier: closure.currentidentifier, path: closure.UpdatedPath, user: iago.user.userName }),
                dataType: "json"
            }).then(function (responseText) {
                $("#toolkitLeftMenu").slideUp("slow", function () {
                    $("#toolkitLeftMenu").remove();
                    $(".toolKitAddReports").css("opacity", "1");
                    $('div.addedReportName:contains(' + toolKit.reportName + ')').closest(".columnBodySectionReport").find(".PublishTheReport").hide();
                    if ($('div.addedReportName:contains(' + toolKit.reportName + ')').closest(".columnBodySectionReport").find(".PublishReprtIcon").find(".PublishedReport").length == 0)
                        $('div.addedReportName:contains(' + toolKit.reportName + ')').closest(".columnBodySectionReport").find(".PublishReprtIcon").append("<div class=\"PublishedReport\"> Published</div>");

                });
            })

        },

        getNewMenuObj: function (menuItem) {
            var self = this;
            var newObj = {};
            for (var o in menuItem) {
                if (menuItem.hasOwnProperty(o)) {
                    if (menuItem[o] instanceof Array) {
                        if (menuItem[o] != null)
                            newObj[o] = self.getChildArray(menuItem[o]);
                        else
                            newObj[o] = menuItem[o];
                    }
                    else
                        newObj[o] = menuItem[o];
                }
            }
            return newObj;
        },

        getChildArray: function (childArray) {
            var self = this;
            var child = [];
            for (var i = 0; i < childArray.length; i++) {
                child.push(self.getNewMenuObj(childArray[i]));
            }
            return child;
        },

        AddOrRemoveChild: function (parent, model, event) {
            var closure = this;
            // check for checking it being clicked for first time
            // problem isske children ki length ki hai
            if ($(event.target).closest(".Data_left_menu").children().length == 2 || $(event.target).closest(".TollKit_Report").children().length == 3) {

                $(event.target).closest(".LeftMenuBar").find(".LeftMenuBarParent").remove();
                var x = $(event.target).closest(".LeftMenuBar").find(".LeftMenuBar");
                if (x.children().length > 0) {
                    x.closest(".Data_left_menu ").find(".leFtToggleArrow")[0].className = "fa fa-angle-right leFtToggleArrow"
                    x.remove();
                }
                if ($(event.target).closest(".LeftMenuBar").attr('path') != "")
                    var path = $(event.target).closest(".LeftMenuBar").attr('path') + "|" + model.Text;
                else
                    var path = model.Text;

                if (closure.ToggleButton == true) {
                    closure.LeftTabChildren(model.Children, $(event.target), parent[0].currentLevel())
                }
                else {
                    if ($(event.target).parent().hasClass("TollKit_Report")) {

                    }
                    else {
                        closure.LeftTabChildren(model.Children, event, parent[0].currentLevel());
                        $(event.target).closest(".LeftMenuBar").find(".LeftMenuBar").attr('path', path);
                    }
                }
                $(event.target).closest(".LeftMenuBar").find(".LeftMenuBarParent").addClass("child");
                $(event.target).closest(".LeftMenuBar").find(".LeftMenuBarParent").css("top", $(event.target).position().top - 6)

                if (closure.ToggleButton == true) {
                    if ($(event.target).parent().hasClass("Folder")) {
                        $(event.target).closest(".Data_left_menu").find(".fa")[0].className = "leFtToggleArrow";
                    }
                    else {
                        $(event.target).closest(".TollKit_Report").find(".fa")[0].className = "leFtToggleArrow";
                    }

                }
                else {
                    $(event.target).closest(".Data_left_menu").find(".fa")[0].className = "leFtToggleArrow";
                }
                if (!closure.EditFlag) {
                    $(".delete_div").hide();
                }
                if (closure.reportIsAdeded == false && closure.EditFlag == false) {
                    $(".addReportName").css("display", "none");
                    $(event.target).closest(".LeftMenuBar").children(".AddNewReport").show();
                }
                if (closure.EditFlag) {
                    $(".AddNewReport").css("display", "none");
                }
            }
            else if ($(event.target).attr('class') == "texthere") {
                // just taki yeh else pe naa jai when clicked here
            }
            else if ($(event.target).attr('class') == "TollKit_Report ui-sortable-handle") {
                // just taki yeh else pe naa jai when clicked here
            }
            else if ($(event.target).attr('class') == "fa fa-floppy-o") {
                // just taki yeh else pe naa jai when clicked here
            }
            else if ($(event.target).parent().hasClass("TollKit_Report")) {
                // just taki yeh else pe naa jai when clicked here
            }
            else {
                // isssmein jab dobara click karte ho ya kahi aur ,it closes all the children tabs
                var e = event.toElement || event.relatedTarget;
                if ($(event.currentTarget).find(e).length || $(event.currentTarget) == e) {
                    $(event.target).closest(".LeftMenuBar").find(".LeftMenuBarParent").remove();
                }
                $(event.target).closest(".Data_left_menu").find(".leFtToggleArrow")[0].className = "fa fa-angle-right leFtToggleArrow";
            }
            event.stopPropagation();
            event.preventDefault();
        }


    }
})

$.extend(toolKit, {
    ExportReport: {
        Export: function (exportType) {
            var closure = this;
            closure.ExportData = [];
            closure.ExportType = exportType;
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/GetHtmlTemplate',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ path: 'com.ivo.rad.RGridWriterToolKit.Resources.ExportReport.htm' })
            }).then(function (responseText) {

                $("#" + toolKit.identifier).append(responseText.d);
                if (closure.ExportType == "Excel") {
                    $("#" + toolKit.identifier).find(".ExportTo").text("Export To Excel")
                    $("#" + toolKit.identifier).find(".PdfFooter").hide();
                    $("#" + toolKit.identifier).find(".UpperFooterClass").addClass("AddPadding2");
                }
                else if (closure.ExportType == "PDF") {
                    $("#" + toolKit.identifier).find(".ExportTo").text("Export To PDF")
                    $("#" + toolKit.identifier).find(".PdfFooter").show();
                }
                closure.ExportBindings();
            })
        },
        ExportBindings: function () {
            var self = this;
            self.BuildHtml = function () {
                var closure = this;
                closure.TabDetails = ko.observableArray([]);
                closure.selectData = function (model, event) {
                    if ($(event.target).hasClass("GroupedChild")) {
                        $(event.target).addClass("selected");
                        self.BuildData(model, event.target);
                        var array = [];
                        for (var i = 0; i < $(event.target).parent().find(".selected").length; i++) {
                            array.push($($(event.target).parent().find(".selected")[i]).text());
                        }
                        var Text = self.ShowCloumnNmae(array.length, array, "Grouping");
                        $(event.target).closest(".GroupDropdown").find(".groupTextshow").text(Text);
                        if ($(event.target).closest(".GroupDropdown").find(".groupTextshow").html().trim() != "Grouping") {
                            $(event.target).closest(".TileGroupColumn").find(".CircleClicked").show();
                            $(event.target).closest(".TileGroupColumn").find(".CircleIcon").hide();
                        }
                        else {
                            $(event.target).closest(".TileGroupColumn").find(".CircleClicked").hide();
                            $(event.target).closest(".TileGroupColumn").find(".CircleIcon").show();
                        }
                    }
                    else if ($(event.target).hasClass("SortChild") || $(event.target).hasClass("asc") || $(event.target).hasClass("dsc")) {
                        $(event.target).addClass("selected");
                        self.BuildData(model, event.target);
                        var array = [];
                        for (var i = 0; i < $(event.target).closest(".SortParent").find(".selected").length; i++) {
                            array.push($($(event.target).closest(".SortParent").find(".selected")[i]).text());
                        }
                        var Text = self.ShowCloumnNmae(array.length, array, "Sorting");
                        $(event.target).closest(".SortDropdown").find(".SortTextshow").text(Text);
                        if ($(event.target).closest(".SortDropdown").find(".SortTextshow").html().trim() != "Sorting") {
                            $(event.target).closest(".TileSortColumn").find(".CircleClicked").show();
                            $(event.target).closest(".TileSortColumn").find(".CircleIcon").hide();
                        }
                        else {
                            $(event.target).closest(".TileSortColumn").find(".CircleClicked").hide();
                            $(event.target).closest(".TileSortColumn").find(".CircleIcon").show();
                        }
                    }
                    else if ($(event.target).hasClass("LayoutChild")) {
                        $(".LayoutChild").removeClass("selected");
                        $(event.target).addClass("selected");
                        self.BuildData(model, event.target);
                        var Text = $(".LayoutChild.selected").html();
                        if (Text == null || Text == "") {
                            $(event.target).closest(".LayoutDropdown").find(".LayoutTextshow").text("Layout");
                        }
                        else {
                            $(event.target).closest(".LayoutDropdown").find(".LayoutTextshow").text(Text);
                        }
                        if ($(event.target).closest(".LayoutDropdown").find(".LayoutTextshow").html().trim() != "Layout") {
                            $(event.target).closest(".TileLayoutColumn").find(".CircleClicked").show();
                            $(event.target).closest(".TileLayoutColumn").find(".CircleIcon").hide();
                        }
                        else {
                            $(event.target).closest(".TileLayoutColumn").find(".CircleClicked").hide();
                            $(event.target).closest(".TileLayoutColumn").find(".CircleIcon").show();
                        }
                    }
                    else if ($(event.target).hasClass("PdfForemat-content-type") || $(event.target).hasClass("PdfForemat-orient-type")) {
                        $(event.target).closest(".PdfForemat-content").find(".selected").removeClass("selected");
                        $(event.target).closest(".PdfForematOrient-content").find(".selected").removeClass("selected");
                        $(event.target).addClass("selected");
                        if ($(event.target).html().trim() == "A4") {
                            $(event.target).closest(".PdfForemat").attr("value", "4")
                        }
                        else if ($(event.target).html().trim() == "A3") {
                            $(event.target).closest(".PdfForemat").attr("value", "3")
                        }
                        else if ($(event.target).html().trim() == "A2") {
                            $(event.target).closest(".PdfForemat").attr("value", "2")
                        }
                        else if ($(event.target).html().trim() == "A1") {
                            $(event.target).closest(".PdfForemat").attr("value", "1")
                        }
                        else if ($(event.target).html().trim() == "A0") {
                            $(event.target).closest(".PdfForemat").attr("value", "0")
                        }
                        else if ($(event.target).html().trim() == "Potrait") {
                            $(event.target).closest(".PdfOrientation").attr("value", "0")
                        }
                        else if ($(event.target).html().trim() == "Landscape") {
                            $(event.target).closest(".PdfOrientation").attr("value", "1")
                        }
                    }
                    $(event.target).closest(".PdfForemat").find(".formatHeaderChild").text("A" + $(event.target).closest(".PdfForemat").attr("value"));
                    $(event.target).closest(".PdfForemat").find(".PdfForemat-content").hide();
                    event.stopPropagation();
                    event.preventDefault();
                }
                closure.TileSelect = function (model, event) {
                    self.TileNumber = $(event.target).index();
                    if ($(event.target).closest(".TextShow").find(".groupTextshow").length > 0) {
                        self.HideOpenedDropdowns();
                        $(event.target).closest(".GroupDropdown").addClass("GroupDropdownAdded");
                        $(event.target).closest(".TileGroupColumn").addClass("TileGroupColumnAdded");
                        $(event.target).closest(".TextShow").siblings(".GroupedParent").show();
                        $(event.target).closest(".TextShow").find(".ArrowforDropdown").show();
                    }
                    else if ($(event.target).closest(".TextShow").find(".SortTextshow").length > 0) {
                        self.HideOpenedDropdowns();
                        $(event.target).closest(".SortDropdown").addClass("SortDropdownAdded");
                        $(event.target).closest(".TileSortColumn").addClass("TileSortColumnAdded");
                        $(event.target).closest(".TextShow").siblings(".SortParent").show();
                        $(event.target).closest(".TextShow").find(".ArrowforDropdown").show();
                    }
                    else if ($(event.target).closest(".TextShow").find(".LayoutTextshow").length > 0) {
                        self.HideOpenedDropdowns();
                        $(event.target).closest(".LayoutDropdown").addClass("LayoutDropdownAdded");
                        $(event.target).closest(".TileLayoutColumn").addClass("TileLayoutColumnAdded");
                        if ($(event.target).closest(".TextShow").siblings(".LayoutParent").find(".LayoutChild").length > 0) {
                            $(event.target).closest(".TextShow").siblings(".LayoutParent").show();
                            $(event.target).closest(".TextShow").find(".ArrowforDropdown").show();
                        }
                    }
                    else if ($(event.target).hasClass("TabName")) {
                        if ($($(event.target).closest(".TileNameColumn").find(".ActualTileNameColumn")).hasClass("Select")) {
                            self.HideOpenedDropdowns();
                            $(event.target).closest(".TileNameColumn").find(".LayoutTextshow").text("Layout");
                            $(event.target).closest(".TileNameColumn").find(".groupTextshow").text("Grouping");
                            $(event.target).closest(".TileNameColumn").find(".SortTextshow").text("Sorting");
                            $(event.target).closest(".TileNameColumn").find(".CircleClicked").hide();
                            $(event.target).closest(".TileNameColumn").find(".CircleIcon").show();
                            $(event.target).closest(".TileNameColumn").find(".GroupDropdown").removeClass("GroupDropdownAdded");
                            $(event.target).closest(".TileNameColumn").find(".TileSortColumn").removeClass("TileSortColumnAdded");
                            $(event.target).closest(".TileNameColumn").find(".SortDropdown").removeClass("SortDropdownAdded");
                            $(event.target).closest(".TileNameColumn").find(".TileGroupColumn").removeClass("TileGroupColumnAdded");
                            $(event.target).closest(".TileNameColumn").find(".LayoutDropdown").removeClass("LayoutDropdownAdded");
                            $(event.target).closest(".TileNameColumn").find(".TileLayoutColumn").removeClass("TileLayoutColumnAdded");
                            $(event.target).closest(".TileNameColumn").find(".ArrowforDropdown").hide();
                            $(event.target).closest(".TileNameColumn").find(".GroupedParent").find(".selected").removeClass("selected");
                            $(event.target).closest(".TileNameColumn").find(".SortParent").find(".selected").removeClass("selected");
                            $(event.target).closest(".TileNameColumn").find(".LayoutParent").find(".selected").removeClass("selected");
                            $($(event.target).closest(".TileNameColumn").find(".ActualTileNameColumn")).removeClass("Select")
                            for (var i = 0; i < self.ExportData.length; i++) {
                                if (parseInt(self.ExportData[i].tabid) == parseInt(toolKit.ExportReport.TabDetailsInfo.TabDetails()[$(event.target).closest(".TileNameColumn").index()].tabId)) {
                                    self.ExportData.splice(i, 1);
                                }
                            }

                            return false;
                        }

                    }
                    else {
                        self.HideOpenedDropdowns();
                    }
                    var j = 0;
                    for (var i = 0; i < self.ExportData.length; i++) {
                        if (parseInt(self.ExportData[i].tabid) == parseInt(toolKit.ExportReport.TabDetailsInfo.TabDetails()[$(event.target).closest(".TileNameColumn").index()].tabId)) {
                            j++;
                        }
                    }
                    if (j == 0) {
                        var data = { tabid: toolKit.ExportReport.TabDetailsInfo.TabDetails()[$(event.target).closest(".TileNameColumn").index()].tabId,
                            LayoutName: "",
                            GroupedandSortedColumns: []
                        }
                        self.ExportData.push(data);
                    }
                    $(event.target).closest(".ActualTileNameColumn").addClass("Select");
                    event.stopPropagation();
                    event.preventDefault();
                }
                closure.NotTile = function (model, event) {
                    if ($($(event.target).parent().parent()).hasClass("PdfForemat") || $($(event.target).parent()).hasClass("PdfOrientation") || $(event.target).hasClass("PdfForemat") || $(event.target).hasClass("PdfOrientation")) {
                        self.HideOpenedDropdowns();
                        $($(event.target).closest(".PdfForemat").find(".PdfForemat-content")[0]).show()
                    }
                    event.stopPropagation();
                    event.preventDefault();
                }
                closure.ascORdsc = function (model, event) {
                    //                    if ($(event.target).hasClass("asc")) {
                    //                        $(event.target).closest(".SortInBetween").find(".SortChild").attr("typeofsort", "ascending");
                    //                        $(event.target).hide();
                    //                        $($(event.target).closest(".SortInBetween").find(".dsc")[0]).show();
                    //                    }
                    //                    else if ($(event.target).hasClass("dsc")) {
                    //                        $(event.target).closest(".SortInBetween").find(".SortChild").attr("typeofsort", "descending");
                    //                        $(event.target).hide();
                    //                        $($(event.target).closest(".SortInBetween").find(".asc")[0]).show();
                    //                    }

                    //                    $(event.target).closest(".SortInBetween").find(".SortChild").addClass("selected");
                    //                    self.BuildData(model, $(event.target).closest(".SortInBetween").find(".SortChild"));
                    event.stopPropagation();
                    event.preventDefault();
                }
                closure.ExportTheData = function (model, event) {
                    var exportType = self.ExportType;
                    if (exportType == "PDF") {
                        var PdfSize = parseInt($(".PdfForemat").attr("value"));
                        var PotraitOrientation = parseInt($(".PdfOrientation").attr("value"));
                    }
                    //                    var PdfSize = parseInt($(".PdfForemat").attr("value"));
                    //                    var PotraitOrientation = parseInt($(".PdfOrientation").attr("value"));
                    for (var i = 0; i < self.ExportData.length; i++) {
                        self.ExportData[i].PdfINfo = {};
                        self.ExportData[i].PdfINfo.PdfPageSize = PdfSize;
                        self.ExportData[i].PdfINfo.PotraitOrientation = PotraitOrientation;
                    }
                    for (var i = 0; i < 2; i++) {
                        if ($(".RadioButtons").find(".CircleClicked")[i].style.display != "none") {
                            if ($($(".RadioButtons").find(".CircleClicked")[i]).closest(".RadioButton1").length > 0) {
                                var data = 0;
                            }
                            else if ($($(".RadioButtons").find(".CircleClicked")[i]).closest(".RadioButton2").length > 0) {
                                var data = 1;
                            }
                        }
                    }
                    self.ExportFinalDATA = {
                        ExportType: ($(".advanceSelected").hasClass("AdvancedWrapper") ? 2 : (data == 0 ? 0 : 1)),
                        tablist: self.ExportData
                    }

                    var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
                    self.ExportFinalDATA.currentTabName = toolKit.CurrentTabInfo.TabName;
                    if (self.ExportFinalDATA.tablist.length > 0) {
                        $.ajax({
                            type: 'POST',
                            contentType: "application/json",
                            dataType: 'json',
                            url: url + '/ExportReport',
                            data: JSON.stringify({ identifier: toolKit.reportInfo.reportIdentifier, dbIdentifier: toolKit.dbIdentifier, database: toolKit.DBName, tabFilterInfo: JSON.stringify(self.ExportFinalDATA), exportType: exportType, Reportname: toolKit.reportInfo.ReportName,calendarName: toolKit.reportInfo.CalendarName }),
                            dataType: "json"
                        }).then(function (responseText) {
                            if (responseText.d) {
                                $('.ExportReport').animate({
                                    opacity: 0, // animate slideUp
                                    marginLeft: '-200px'
                                }, 'slow', 'linear', function () {
                                    $(this).remove();
                                });
                                if (exportType == "Excel") {
                                    window.open("App_Dynamic_Resource/RGridWriterToolKit,com.ivo.rad.RGridWriterToolKit.Resources.RADGridExportToExcel.aspx?eventData=" + toolKit.reportInfo.ReportName + ".xlsx", "_blank", "width=400,height=1,menubar=0,resizable=1");
                                }
                                else {
                                    window.open("App_Dynamic_Resource/RGridWriterToolKit,com.ivo.rad.RGridWriterToolKit.Resources.RADGridExportToExcel.aspx?eventData=" + toolKit.reportInfo.ReportName + ".pdf", "_blank", "width=400,height=1,menubar=0,resizable=1");
                                }
                            }
                        })
                    }
                    event.stopPropagation();
                    event.preventDefault();
                }
                closure.RawDataClicked = function (model, event) {
                    $(event.target).siblings(".circleClickParent").find(".CircleClicked").show();
                    $(event.target).siblings(".circleClickParent").find(".CircleIcon").hide();
                    $(event.target).closest(".RadioButtons").find(".RadioButton2").find(".circleClickParent").find(".CircleIcon").show();
                    $(event.target).closest(".RadioButtons").find(".RadioButton2").find(".circleClickParent").find(".CircleClicked").hide();
                    event.stopPropagation();
                    event.preventDefault();
                }
                closure.CuurentState = function (model, event) {
                    $(event.target).siblings(".circleClickParent").find(".CircleClicked").show();
                    $(event.target).siblings(".circleClickParent").find(".CircleIcon").hide();
                    $(event.target).closest(".RadioButtons").find(".RadioButton1").find(".circleClickParent").find(".CircleIcon").show();
                    $(event.target).closest(".RadioButtons").find(".RadioButton1").find(".circleClickParent").find(".CircleClicked").hide();
                    event.stopPropagation();
                    event.preventDefault();
                }
                closure.SelecttheTile = function (model, event) {
                    var j = 0;
                    for (var i = 0; i < self.ExportData.length; i++) {
                        if (parseInt(self.ExportData[i].tabid) == parseInt(toolKit.ExportReport.TabDetailsInfo.TabDetails()[$(event.target).closest(".WrapTheTile").index()].tabId)) {
                            j++;
                            $($(event.target).closest(".TileName")).removeClass("Select")
                            self.ExportData = $.grep(self.ExportData, function (e) {
                                return e.tabid != self.ExportData[i].tabid;
                            });
                            break;
                        }
                    }
                    if (j == 0) {
                        var data = { tabid: toolKit.ExportReport.TabDetailsInfo.TabDetails()[$(event.target).closest(".WrapTheTile").index()].tabId,
                            LayoutName: "",
                            GroupedandSortedColumns: []
                        }
                        self.ExportData.push(data);
                        $($(event.target).closest(".TileName")).addClass("Select")
                    }
                    event.stopPropagation();
                    event.preventDefault();
                }
                closure.DropdownClose = function (model, event) {
                    self.HideOpenedDropdowns();
                    event.stopPropagation();
                    event.preventDefault();
                }
                closure.goToAdvanced = function (model, event) {
                    self.ExportData = [];
                    $(event.target).siblings(".NormalWrapper").removeClass("advanceSelected");
                    $(event.target).addClass("advanceSelected");
                    $(".ExportAdvanced").show();
                    $(".ExportNormal").hide();
                    if (self.ExportType == "Excel") {
                        $(".UpperFooterClass").hide();
                    }
                    else {
                        $(".RadioButtons").hide();
                        $(".UpperFooterClass").addClass("AddPadding");
                    }
                    $(".TextWritten").addClass("TextWrittenAdvanced");
                    $(".ArrowforDropdown").css({ 'display': 'none' });
                    $(".TileSortColumnAdded").removeClass("TileSortColumnAdded");
                    $(".TileLayoutColumnAdded").removeClass("TileLayoutColumnAdded");
                    $(".TileGroupColumnAdded").removeClass("TileGroupColumnAdded");
                    $(".LayoutDropdownAdded").removeClass("LayoutDropdownAdded");
                    $(".SortDropdownAdded").removeClass("SortDropdownAdded");
                    $(".GroupDropdownAdded").removeClass("GroupDropdownAdded");
                    $(".CircleClicked").css({ 'display': 'none' });
                    $(".CircleIcon").css({ 'display': '' });
                    event.stopPropagation();
                    event.preventDefault();
                }
                closure.goToNormal = function (model, event) {
                    self.ExportData = [];
                    $(event.target).siblings(".AdvancedWrapper").removeClass("advanceSelected");
                    $(event.target).addClass("advanceSelected");
                    $(".TextWritten").removeClass("TextWrittenAdvanced");
                    $(".ExportNormal").show();
                    $(".ExportAdvanced").hide();
                    if (self.ExportType == "Excel") {
                        $(".UpperFooterClass").show();
                    }
                    else {
                        $(".PdfFooter").show();
                        $(".RadioButtons").show();
                        $(".UpperFooterClass").show();
                        $(".UpperFooterClass").removeClass("AddPadding");
                    }
                    event.stopPropagation();
                    event.preventDefault();
                }
                closure.CancelTheData = function (model, event) {
                    $('.ExportReport').animate({
                        opacity: 0, // animate slideUp
                        marginLeft: '-200px'
                    }, 'slow', 'linear', function () {
                        $(this).remove();
                    });
                    event.stopPropagation();
                    event.preventDefault();
                }
            }
            self.TabDetailsInfo = new self.BuildHtml();
            for (var i = 0; i < toolKit.reportInfo.TabsDetails.length; i++) {
                self.TabDetailsInfo.TabDetails.push(toolKit.reportInfo.TabsDetails[i]);
            }
            var url = toolKit.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                url: url + '/GetLayout',
                data: JSON.stringify({ identifier: toolKit.reportInfo.reportIdentifier })
            }).then(function (responseText) {
                var dict = $.parseJSON(responseText.d);
                var array = $.map(dict, function (value, index) {
                    return [value];
                });
                for (var i = 0; i < toolKit.reportInfo.TabsDetails.length; i++) {
                    self.TabDetailsInfo.TabDetails()[i].LayoutNames = array[i];

                }
                ko.applyBindings(self.TabDetailsInfo, $("#Export_Report")[0]);
            })

        },

        HideOpenedDropdowns: function () {
            $(".GroupedParent").hide();
            $(".SortParent").hide();
            $(".LayoutParent").hide();
            $(".PdfForemat-content").hide();
        },

        ShowCloumnNmae: function (Length, array, xyz) {
            if (Length > 2) {
                var groupedColumns = array[0] + "," + array[1] + ".." + (Length - 2) + "more";
            }
            else if (Length == 2) {
                var groupedColumns = array[0] + "," + array[1];
            }
            else if (Length == 1) {
                var groupedColumns = array[0];
            }
            else {
                var groupedColumns = xyz;
            }

            return groupedColumns;
        },

        BuildData: function (model, event) {
            var closure = this;
            var j = 0;
            if ($(event).hasClass("GroupedChild")) {
                for (var i = 0; i < closure.ExportData.length; i++) {
                    if (parseInt(closure.ExportData[i].tabid) == parseInt(toolKit.ExportReport.TabDetailsInfo.TabDetails()[$(event).closest(".TileNameColumn").index()].tabId)) {
                        j++;
                        var x = 0;
                        if (closure.ExportData[i].GroupedandSortedColumns.length > 0) {
                            for (var k = 0; k < closure.ExportData[i].GroupedandSortedColumns.length; k++) {
                                if (closure.ExportData[i].GroupedandSortedColumns[k].ColumnName == $(event).attr("columnNmae")) {
                                    x++;
                                    if (closure.ExportData[i].GroupedandSortedColumns[k].IsGrouped == true) {
                                        closure.ExportData[i].GroupedandSortedColumns.splice(k, 1);
                                        $(event).removeClass("selected");
                                    }
                                    else {
                                        closure.ExportData[i].GroupedandSortedColumns[k].IsGrouped = true;
                                        if ($(event).closest(".TileNameColumn").find(".SortChild").filter(function () {
                                            return this.innerHTML === $(event).text();
                                        }).attr("typeofsort") == "ascending") {
                                            closure.ExportData[i].GroupedandSortedColumns[k].SortOrder = 1;
                                        }
                                        else {
                                            closure.ExportData[i].GroupedandSortedColumns[k].SortOrder = 2;
                                        }
                                    }
                                }
                            }
                        }
                        if (x == 0) {
                            var item = {
                                ColumnName: $(event).attr("columnNmae"),
                                IsGrouped: true,
                                IsRankingColumn: false,
                                SortOrder: 1
                            }
                            closure.ExportData[i].GroupedandSortedColumns.push(item);
                        }
                    }

                }
                if (j == 0) {
                    var data = { tabid: parseInt(toolKit.ExportReport.TabDetailsInfo.TabDetails()[$(event).closest(".TileNameColumn").index()].tabId),
                        LayoutName: "",
                        GroupedandSortedColumns: [{
                            ColumnName: $(event).attr("columnNmae"),
                            IsGrouped: true,
                            IsRankingColumn: false,
                            SortOrder: 1
                        }]
                    }
                    closure.ExportData.push(data);
                }

            }
            else if ($(event).hasClass("SortChild")) {
                for (var i = 0; i < closure.ExportData.length; i++) {
                    if (parseInt(closure.ExportData[i].tabid) == parseInt(toolKit.ExportReport.TabDetailsInfo.TabDetails()[$(event).closest(".TileNameColumn").index()].tabId)) {
                        j++;
                        var x = 0;
                        if (closure.ExportData[i].GroupedandSortedColumns.length > 0) {
                            for (var k = 0; k < closure.ExportData[i].GroupedandSortedColumns.length; k++) {
                                if (closure.ExportData[i].GroupedandSortedColumns[k].ColumnName == $(event).attr("columnNmae")) {
                                    x++;
                                    if (closure.ExportData[i].GroupedandSortedColumns[k].IsGrouped == false) {
                                        if ($(event).siblings(".asc")[0].style.display != "none" && $(event).siblings(".dsc")[0].style.display == "none") {
                                            $(event).attr("typeofsort", "descending");
                                            closure.ExportData[i].GroupedandSortedColumns[k].SortOrder = 2;
                                            $(event).siblings(".dsc").show();
                                            $(event).siblings(".asc").hide();
                                        }
                                        else if ($(event).siblings(".dsc")[0].style.display != "none" && $(event).siblings(".asc")[0].style.display == "none") {
                                            $(event).attr("typeofsort", "ascending");
                                            closure.ExportData[i].GroupedandSortedColumns.splice(k, 1);
                                            $(event).removeClass("selected");
                                            $(event).siblings(".dsc").hide();
                                            $(event).siblings(".asc").hide();
                                        }
                                    }
                                    else {
                                        if (($(event).siblings(".dsc")[0].style.display == "none" && $(event).siblings(".asc")[0].style.display != "none")) {
                                            closure.ExportData[i].GroupedandSortedColumns[k].SortOrder = 2;
                                            $(event).siblings(".dsc").show();
                                            $(event).siblings(".asc").hide();
                                        }
                                        else if (($(event).siblings(".dsc")[0].style.display == "none" && $(event).siblings(".asc")[0].style.display == "none") || ($(event).siblings(".dsc")[0].style.display != "none" && $(event).siblings(".asc")[0].style.display == "none")) {
                                            $(event).siblings(".asc").show();
                                            $(event).siblings(".dsc").hide();
                                        }
                                    }
                                }
                            }
                        }
                        if (x == 0) {
                            var item = {
                                ColumnName: $(event).attr("columnNmae"),
                                IsGrouped: false,
                                IsRankingColumn: false,
                                SortOrder: $(event).attr("typeofsort") == "ascending" ? 1 : 2
                            }
                            closure.ExportData[i].GroupedandSortedColumns.push(item);
                            if ($(event).attr("typeofsort") == "ascending") {
                                $(event).siblings(".asc").show();
                                $(event).siblings(".dsc").hide();
                            }
                            else {
                                $(event).siblings(".dsc").show();
                                $(event).siblings(".asc").hide();
                            }
                        }
                    }

                }
                if (j == 0) {
                    var data = { tabid: parseInt(toolKit.ExportReport.TabDetailsInfo.TabDetails()[$(event).closest(".TileNameColumn").index()].tabId),
                        LayoutName: "",
                        GroupedandSortedColumns: [{
                            ColumnName: $(event).attr("columnNmae"),
                            IsGrouped: false,
                            IsRankingColumn: false,
                            SortOrder: $(event).attr("typeofsort") == "ascending" ? 1 : 2
                        }]
                    }
                    closure.ExportData.push(data);
                    if ($(event).attr("typeofsort") == "ascending") {
                        $(event).siblings(".asc").show();
                        $(event).siblings(".dsc").hide();
                    }
                    else {
                        $(event).siblings(".dsc").show();
                        $(event).siblings(".asc").hide();
                    }
                }
            }
            else if ($(event).hasClass("LayoutChild")) {
                for (var i = 0; i < closure.ExportData.length; i++) {
                    if (parseInt(closure.ExportData[i].tabid) == parseInt(toolKit.ExportReport.TabDetailsInfo.TabDetails()[$(event).closest(".TileNameColumn").index()].tabId)) {
                        j++;
                        if (closure.ExportData[i].LayoutName == $(event).html()) {
                            $(event).removeClass("selected");
                            closure.ExportData[i].LayoutName = "";
                        }
                        else {
                            closure.ExportData[i].LayoutName = $(event).html();
                        }
                    }
                }
                if (j == 0) {
                    var data = { tabid: parseInt(toolKit.ExportReport.TabDetailsInfo.TabDetails()[$(event).closest(".TileNameColumn").index()].tabId),
                        LayoutName: $(event).html(),
                        GroupedandSortedColumns: []
                    }
                }
            }

        }

    }
})

$(function () {
    $.widget("iago-widget.toolKittabs", {
        options: {
            tabSchema: {
                tab: [
	                    {
	                        id: "t1",
	                        name: "t1",
	                        subtab: [{
	                            id: "t5",
	                            name: "t5",
	                            subtab: [
	                                    {
	                                        id: "t11",
	                                        name: "t11",
	                                        isDefault: true
	                                    },
	                                    {
	                                        id: "t12",
	                                        name: "t12"
	                                    }
	                            ]
	                        },
	                            {
	                                id: "t6",
	                                name: "t6"
	                            },
	                            {
	                                id: "t7",
	                                name: "t7",
	                                isDefault: true,
	                                subtab: [
	                                    {
	                                        id: "t13",
	                                        name: "t13",
	                                        isDefault: true
	                                    },
	                                    {
	                                        id: "t14",
	                                        name: "t14"
	                                    }
	                                ]
	                            }
	                        ]

	                    },
	                    {
	                        id: "t2",
	                        name: "t2"
	                    },
	                    {
	                        id: "t3",
	                        name: "PnL Summary",
	                        isDefault: true,
	                        subtab: [
	                            {
	                                id: "t8",
	                                name: "Portfolio",
	                                isDefault: true
	                            },
	                            {
	                                id: "t9",
	                                name: "Sector"
	                            },
	                            {
	                                id: "t10",
	                                name: "Analyst"
	                            }

	                        ]
	                    },
	                    {
	                        id: "t4",
	                        name: "t4"
	                    }



                ]
            },

            tabClickHandler: function (a, tabContentDiv) {
                tabContentDiv.html("<h1>" + a + "</h1>");
            },
            changeTabHandler: function (tabs) {

            },
            deleteTabs: false,
            deleteTabHandler: function (tabName) {

            },
            enableShadow: true,
            tabContentHolder: ""
        },
        _create: function () {

            var self = this;
            this.tabDivArray = new Array();

            function widgetViewModel() {
                var self = this;
                self.tabSchema = ko.observable();
            }

            var widgetId = this.element.attr("id");
            // var tabDivArray = new Array();

            var tabContentHolder = this.options.tabContentHolder;

            function render(element, options, tabDivArray, manageTabState) {
                var widgetIdentifier = widgetId;
                renderTab(options.tabSchema.tab, 0, undefined, tabDivArray);
                var $tabStrip = $(renderTabStrip(tabDivArray, options.enableShadow));
                $tabStrip.appendTo(element);
                var self = this;
                //$tabStrip.find('ul li').unbind('click').click(function (event) {
                $tabStrip.unbind('click');

                $tabStrip.on('click', 'ul li', function (event) {
                    var $clickedElem = $(event.target);
                    if ($(event.target).get(0).tagName === 'A') {
                        if ($(event.target).get(0).tagName === 'A') {
                            $clickedElem = $clickedElem.parent();
                        }
                        if (iago.global != null && iago.core != null) {
                            iago.global.isTabClicked = true;
                            iago.core.changeLocation($(this).attr("data-link"));
                        }
                        $("#" + widgetIdentifier).toolKittabs("setSelectedTab", $clickedElem.attr("id"), true);
                    }
                    else if ($(event.target).hasClass("editTabs")) {
                        if (iago.global != null)
                            iago.global.isTabClicked = true;
                        $("#" + widgetIdentifier).toolKittabs("editTab", event);
                    }
                    else if ($(event.target).hasClass("btnAddTab") || $(event.target).hasClass("btnCancelTab")) {
                        $("#" + widgetIdentifier).toolKittabs("doAction", event);
                    }
                    else if ($(event.target).hasClass("btnDeleteTab") || $(event.target).hasClass("btnCancelDelete")) {
                        if (iago.global != null)
                            iago.global.isTabClicked = true;
                        $("#" + widgetIdentifier).toolKittabs("deleteTab", event);
                    }
                    else if ($(event.target).hasClass("deleteTabs")) {
                        if (iago.global != null)
                            iago.global.isTabClicked = true;
                        $("#" + widgetIdentifier).toolKittabs("deleteTabPopUp", event);
                    }
                    return false;

                });
                //if(this.options.deleteTabs)
                //{
                $tabStrip.on('mouseover', 'ul li', function (event) {
                    $(event.target).closest('li').find(".deleteTabs").removeClass("hideTabs");
                    $(event.target).closest('li').find(".editTabs").removeClass("hideTabs");
                });
                $tabStrip.on('mouseout', 'ul li', function (event) {
                    $(event.target).closest('li').find(".deleteTabs").addClass("hideTabs");
                    $(event.target).closest('li').find(".editTabs").addClass("hideTabs");
                });
                //}

                manageTabState(null, widgetId, tabDivArray);
            }

            function renderTab(tabList, level, parentId, tabDivArray) {
                if (tabList === undefined || tabList.length === 0) return;

                var tabStrip = '<ul class="nav nav-tabs" id="' + parentId + '_subtab">';
                var loc = location.hash;
                var locWithoutTabPart = location.hash.split(new RegExp("\/tab"))[0];
                for (var i = 0; i < tabList.length; i++) {
                    var tab = tabList[i];

                    tabStrip = tabStrip + '<li ' + ((tab.isDefault === undefined || tab.isDefault === false) ? "" : "data-is-default=\"\"") + ((tab.isDefault === true) ? ((level > 0) ? ' class="subtab subactive" ' : ' class="active" ') : ((level > 0) ? ' class="subtab" ' : ' class="" ')) + ' data-level="' + level + '" id="' + tab.id + '" data-link="' + locWithoutTabPart + '/tab/' + tab.id + '" ' + ((tab.subtab === undefined || tab.subtab.length === 0) ? "" : " data-has-subtab=\"\"") + ' ><a>' + tab.name;

                    if (self.options.deleteTabs)
                        tabStrip = tabStrip + '</a><div class=\"tabActionsParent\"><div class=\"editTabs hideTabs fa fa-pencil\"></div><div class=\"deleteTabs hideTabs fa fa-times\"></div></div></li>';
                        //tabStrip = tabStrip + '</a><div class=\"deleteTabs hideTabs fa fa-times\"></div></li>';
                    else
                        tabStrip = tabStrip + '</a></li>';

                    renderTab(tab.subtab, level + 1, tab.id, tabDivArray);
                }
                tabStrip = tabStrip + '</ul>';

                if (tabDivArray[level] === undefined) tabDivArray[level] = "";
                tabDivArray[level] = tabDivArray[level] + tabStrip;
            }


            function renderTabStrip(tabDivArray, enableTabShadow) {


                var iagoHeader = $("#" + iago.pageHeaderSpaceId);

                var stripContainer = '<div class="tabbable-custom toolKitTabs nav-justified">';
                if (enableTabShadow)
                    var stripContainer = '<div class="tabbable-custom toolKitTabs nav-justified customTabShadow">';
                for (var i = 0; i < tabDivArray.length; i++) {
                    stripContainer = stripContainer + '<div class="tab-level" id="' + widgetId + '_div_level_' + i + '">' + tabDivArray[i] + "</div>";
                }
                stripContainer = stripContainer + "</div>";

                if (tabContentHolder === "" || tabContentHolder === null || tabContentHolder === undefined) {
                    if (iagoHeader.html().trim() === "") {
                        stripContainer = stripContainer + '<div id="' + widgetId + '_tab_content" class="tab-content" ></div>';
                    } else {
                        stripContainer = stripContainer + '<div id="' + widgetId + '_tab_content" style="margin-top: ' + (iagoHeader.height() + 10) + 'px;" class="tab-content" ></div>';
                    }
                } else {
                    $("#" + tabContentHolder).append('<div id="' + widgetId + '_tab_content"  class="tab-content" ></div>');
                }

                return stripContainer;
            }






            this.options.setJson = function (optionData) {
                if (optionData === undefined) return;
                viewModel.text(optionData.text);
            }
            render(this.element, this.options, this.tabDivArray, this.manageTabState);
            this.options.identifier = this.element.attr("id");
            this.viewModel = new widgetViewModel();
            //ko.applyBindings(this.viewModel, this.element[0]);
            this.element.find('ul').sortable({
                items: "li",
                stop: function (event, ui) {
                    self.changetabOrder(event, ui);
                }
            });

            /* iago.core.addRoute(location.hash+"/tab/:tabid",function() {
	               
            var tabId = this.params["tabid"];
            var $widgetDiv = $("#"+tabId).closest('[type="iago:tabs"]');
            $widgetDiv.tabs("setSelectedTab",tabId,true);
            });*/

            var finalActiveTabId = "";
            for (var i = this.tabDivArray.length - 1; i >= 0; i--) {
                var $tabStrip = $("#" + this.options.identifier + "_div_level_" + i + " ul");
                if ($tabStrip.length === 0) continue;

                if (i === 0) {
                    finalActiveTabId = $tabStrip.find("li.active").attr("id");
                    break;
                } else {
                    finalActiveTabId = $tabStrip.find("li.subactive").attr("id");
                    break;
                }
            }

            if (location.hash === $("#" + finalActiveTabId).attr("data-link")) {
                // if(this.options.isEnabled==true)
                // { 
                this.options.tabClickHandler(finalActiveTabId, $("#" + widgetId + "_tab_content"));
                //  }
            } else {
                var newLocHash = $("#" + finalActiveTabId).attr("data-link");
                if (iago.global != null && iago.core != null) {
                    iago.global.isTabClicked = true;
                    iago.core.pushInitParameterIntoSession(newLocHash, iago.core.getInitParameterFromSession(location.hash));
                    iago.core.changeLocation(newLocHash);
                }
                this.options.tabClickHandler(finalActiveTabId, $("#" + widgetId + "_tab_content"));
            }


            var $iagoHeader = $("#" + iago.pageHeaderSpaceId);
            //	        	if ($iagoHeader.html().trim() !== "")
            //	        	{
            //	        			$(widgetId+"_tab_content").css("margin-top",($iagoHeader.height()+10));
            //	        	}
            //	        	else
            //	        	{
            //	        			$(widgetId+"_tab_content").css("margin-top",5);	
            //	        	}

            //$("#"+finalActiveTabId).click();
            if (iago.core != null) {
                iago.core.registerWidget({
                    id: this.options.identifier,
                    widget: "toolKittabs"
                });
            }
        },

        changetabOrder: function (event, ui) {
            var self = this;
            self.options.tabSchema.tab = [];
            var element = self.element.find('li');
            for (var i = 0; i < element.length; i++) {
                self.options.tabSchema.tab.push({ id: $(element[i]).attr('id'), name: $(element[i]).find('a').html() });
            }
            self.options.changeTabHandler(self.options.tabSchema.tab);
        },

        getJson: function () {
            return ko.toJSON(viewModel);
        },

        getCurrentOptionInfo: function () {
            /* TODO */
            return {};
        },

        getViewModel: function () {
            return viewModel;
        },

        setJson: function (optionData) {
            if (optionData === undefined) return;
            this.viewModel.text(optionData.text);
        },

        deleteTab: function (event) {
            if ($(event.target).hasClass("btnDeleteTab")) {
                var tabName = $(event.target).closest('li').find(".deleteTabs").attr("tabName");//$(event.target).parent().prev().html();
                var self = this;
                var index = 0;
                var tabId = null;
                var nextTabId = "";
                for (var i = 0; i < self.options.tabSchema.tab.length; i++) {
                    if (self.options.tabSchema.tab[i].name == tabName) {
                        index = i;
                        tabId = self.options.tabSchema.tab[i].id;
                        break;
                    }
                }
                //if (index = self.options.tabSchema.tab.length - 1) {
                //    nextTabId = self.options.tabSchema.tab[0].id;
                //}
                //else {
                //    nextTabId = self.options.tabSchema.tab[index + 1].id;
                //}
                self.options.tabSchema.tab.splice(index, 1);
                nextTabId = self.options.tabSchema.tab[0].id;
                self.options.deleteTabHandler(tabId, tabName, true);
                $(event.target).closest("li").remove();
                $("#" + self.element.attr("id")).toolKittabs("setSelectedTab", nextTabId, true);
            }
            else if ($(event.target).hasClass("btnCancelDelete"))
                $(".tabDeletePopUp").remove();
        },

        deleteTabPopUp: function (event) {
            $(".iago-page-title").css({ 'overflow': '' });
            var tabName = $(event.target).parent().prev().html();
            $(event.target).attr('tabName', tabName);
            var div = $('<div>');
            div.addClass("tabDeletePopUp");
            div.append("<div class=\"tabNameLabelParent\"><div class='tabDeletetext'>Delete the tab</div></div>");
            div.append("<div class=\"parentbtnDeleteTab\"><div class='btnDeleteTab'>Ok</div><div class=\"btnCancelDelete\">Cancel</div></div>");
            $(event.target).closest('li').find(".tabActionsParent").append(div);
            $(event.target).closest('li').find(".inputTabName").val(tabName);
        },

        editTab: function (event) {
            $(".iago-page-title").css({ 'overflow': '' });
            var tabName = $(event.target).parent().prev().html();
            $(event.target).attr('tabName', tabName);
            var div = $('<div>');
            div.addClass("tabEditPopUp");
            div.append("<div class=\"tabNameLabelParent\"><div class='tabNameLabelName'>Tab Name</div><div class='tabNameInput'><input class='inputTabName'/></div><div id=\"tabNameValidation\" title = \"Tab Name Already Exists\" class=\"tabNameValidation hiddenClass\" >!</div></div>");
            div.append("<div class=\"parentbtnAddTab\"><div class='btnAddTab'>Ok</div><div class=\"btnCancelTab\">Cancel</div></div>");
            $(event.target).closest('li').find(".tabActionsParent").append(div);
            $(event.target).closest('li').find(".inputTabName").val(tabName);
        },

        doAction: function (event) {
            var self = this;
            if ($(event.target).hasClass("btnAddTab")) {
                var oldTabName = $(event.target).closest('li').find(".editTabs").attr("tabName");
                var tabName = $(event.target).closest('li').find(".inputTabName").val();
                var tabId = null;
                for (var i = 0; i < self.options.tabSchema.tab.length; i++) {
                    if (self.options.tabSchema.tab[i].name == oldTabName) {
                        index = i;
                        self.options.tabSchema.tab[i].name =
                        tabId = self.options.tabSchema.tab[i].id;
                        break;
                    }
                }
                $(event.target).closest('li').find('a').html(tabName);

                self.options.deleteTabHandler(tabId, tabName, false, oldTabName);
                $(".tabEditPopUp").remove();
            }
            else {
                $(".tabEditPopUp").remove();
            }
        },

        addNewTab: function (tab) {
            var loc = location.hash;
            var locWithoutTabPart = location.hash.split(new RegExp("\/tab"))[0];
            var level = 0;
            this.options.tabSchema.tab.push(tab);
            var ul = this.element.find('ul');
            var li = '';
            li = '<li ' + ((tab.isDefault === undefined || tab.isDefault === false) ? "" : "data-is-default=\"\"") + ((tab.isDefault === true) ? ((level > 0) ? ' class="subtab subactive" ' : ' class="active" ') : ((level > 0) ? ' class="subtab" ' : ' class="" ')) + ' data-level="' + level + '" id="' + tab.id + '" data-link="' + locWithoutTabPart + '/tab/' + tab.id + '" ' + ((tab.subtab === undefined || tab.subtab.length === 0) ? "" : " data-has-subtab=\"\"") + ' ><a>' + tab.name + '</a><div class=\"deleteTabs hideTabs fa fa-times\"></div></li>';;
            ul.append(li);
            if (iago.global != null && iago.core != null) {
                iago.global.isTabClicked = true;
                iago.core.changeLocation($(this).attr("data-link"));
            }
            $("#" + this.element.attr("id")).toolKittabs("setSelectedTab", tab.id, true);
        },

        setSelectedTab: function (selectedTabId, isTabHandlerCallRequired) {
            this.manageTabState($("#" + selectedTabId), this.options.identifier, this.tabDivArray);
            var $widgetDiv = $("#" + selectedTabId).closest('[type="iago:tabs"]');
            var finalActiveTabId = "";
            for (var i = this.tabDivArray.length - 1; i >= 0; i--) {
                var $tabStrip = $("#" + this.options.identifier + "_div_level_" + i + " ul:visible");
                if ($tabStrip.length === 0) continue;

                if (i === 0) {
                    finalActiveTabId = $tabStrip.find("li.active").attr("id");
                    break;
                } else {
                    finalActiveTabId = $tabStrip.find("li.subactive").attr("id");
                    break;
                }

            }



            if (isTabHandlerCallRequired === true)
                this.options.tabClickHandler(finalActiveTabId, $("#" + this.options.identifier + "_tab_content"));


            if (location.hash !== $("#" + finalActiveTabId).attr("data-link")) {
                if (iago.global != null && iago.core != null) {
                    iago.global.isTabClicked = true;
                    iago.core.changeLocation($("#" + finalActiveTabId).data("link"));
                }
            }
            var $iagoHeader = $("#" + iago.pageHeaderSpaceId);
            //	        	if ($iagoHeader.html().trim() !== "")
            //	        	{
            //	        			$widgetDiv.find(".tab-content").css("margin-top",($iagoHeader.height()+10));
            //	        	}
            //	        	else
            //	        	{
            //	        			$widgetDiv.find(".tab-content").css("margin-top",5);	
            //	        	}

        },

        manageTabState: function manageTabState(selectedTab, widgetId, tabDivArray) {
            if (widgetId === undefined)
                widgetId = this.element.attr("id");

            if (selectedTab == null) {
                //set selectedTab to level 0 default tab or first tab if no default present
                selectedTab = $("#" + widgetId + "_div_level_0 ul li[data-is-default]")[0];
                if (selectedTab === undefined) $("#" + widgetId + "_div_level_0 ul li")[0];
            }


            var $selectedTab = $(selectedTab);
            var selectedTabId = $(selectedTab).attr("id");
            var level = Number.parseInt($selectedTab.attr("data-level"));

            //loop over current level tabs to switch state
            $selectedTab.parent().find("li").each(function (i, elem) {


                var $elemTab = $(elem);
                if (($elemTab).attr("id") === $selectedTab.attr("id")) {
                    (level > 0) ? $elemTab.addClass("subtab subactive") : $elemTab.addClass("active");
                } else {
                    (level > 0) ? $elemTab.removeClass("subactive") : $elemTab.removeClass("active");
                }
            });

            // manage subtab divs
            if ($selectedTab.is("[data-has-subtab]")) {

                var subtabId = $selectedTab.attr("id") + "_subtab";
                var $subTabDiv = $("#" + widgetId + "_div_level_" + (level + 1));
                $subTabDiv.show();
                $subTabDiv.find("ul").each(function (i, elem) {
                    var $elem = $(elem);
                    if ($elem.attr("id") === subtabId) {
                        $elem.show();


                        manageTabState($elem.find("li[data-is-default]")[0], widgetId, tabDivArray);
                    } else
                        $elem.hide();
                });
            } else {
                //hide all level > current level
                for (var i = level + 1; i < tabDivArray.length; i++) {
                    $("#" + widgetId + "_div_level_" + i).hide();
                }

            }

        },

        _setOptions: function () {
            this._superApply(arguments);
        },

        _setOption: function (key, value) {
            this._super(key, value);
        },

        _destroy: function () {
            $("#" + this.element.attr("id") + "_tab_content").html('');
            $(".toolKitAddTab").remove();
            ko.cleanNode(this.element[0]);
            this.element.unbind();
            this.element.html('');
            if (iago.core != null) {
                iago.core.deRegisterWidget({
                    id: this.options.identifier
                });
            }
        }
    });
    $('[data-type="iago:toolKittabs"]').each(function (index, value) {
        if ($(value).attr('data-options-type') === "JSON")
            $(value).WidgetName(JSON.parse($(value).attr('data-options')));
    });
});

