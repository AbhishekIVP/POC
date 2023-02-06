if (!String.format) {
    String.format = function (format) {
        var args = Array.prototype.slice.call(arguments, 1);
        return format.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
                ? args[number]
                : match
                ;
        });
    };
}
var mainTabCounter = 0;
var mainTabTemplate = '<div class="tab {3}" tab_id="tab-{0}" tabType="{1}">{2}<i>x</i></div>';
var tabTypeRadioTemplate = '<br /><input type="radio" name="tab_type" value="{0}" {1} /><label for="{0}">{0}</label>';

var ExpensiveQuery = '<div class="tabBodyContainer" id="tab-{0}"><iframe src="' + path + '/RecentExpensiveQuery.aspx"/></div>';
var ActivityMonitor = '<div class="tabBodyContainer" id="tab-{0}"><iframe src="' + path + '/ActivityMonitor.aspx"/></div>';
var DeadLock = '<div class="tabBodyContainer" id="tab-{0}"><iframe src= "' + path + '/deadlock.aspx" /></div>';

const tabTypeExecuter = "Executer";
const tabTypeDeadlock = "Deadlock";
const tabTypeLogExpQuery = "LogExpQuery";
const tabTypeActivityMonitor = "Activity Monitor";
var tabTypes = [tabTypeExecuter, tabTypeDeadlock, tabTypeLogExpQuery, tabTypeActivityMonitor];

function getTabContent(tabType) {
    switch (tabType) {
        case tabTypeExecuter:
            return String.format(mainTabExecuterBodyTemplate, mainTabCounter);
        case tabTypeDeadlock:
            return String.format(DeadLock, mainTabCounter);
            break;
        case tabTypeLogExpQuery:
            return String.format(ExpensiveQuery,mainTabCounter);
            break;
        case tabTypeActivityMonitor:
            return String.format(ActivityMonitor, mainTabCounter);
    }
    return String.format(mainTabExecuterBodyTemplate, mainTabCounter);
}

function registerTabContentListener(tab_id, tabType) {
    switch (tabType) {
        case tabTypeExecuter:
            registerExecuterListeners(tab_id);
            break;
    }
}

function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError) {
    callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, false, false);
}

$(document).ready(() => {
    addTab("Query Executer", true, tabTypeExecuter);
    addTab("DeadLock", true, tabTypeDeadlock);
    addTab("LogExpQuery", true, tabTypeLogExpQuery);
    addTab("ActivityMonitor", true, tabTypeActivityMonitor);
    initAddTabDialog();
});
function initAddTabDialog() {
    var tabTypehtml = "";
    tabTypes.forEach((tabtype, index) => {
        tabTypehtml += String.format(tabTypeRadioTemplate, tabtype, index == 0 ? 'checked="checked"' : '');
    });
    $(tabTypehtml).insertAfter("#tab_type_label");
    $(".addTabButton").click((e) => {
        $("#addTabDialog").css('display', 'block');
    });
    $("#addTabDialog").click((e) => {
        if ($(e.target).hasClass('dialog')) {
            $("#addTabDialog").css('display', 'none');
        }
    });
    $("#addTabCancel").click((e) => {
        $("#addTabDialog").css('display', 'none');
    });
    $("#addTabCreate").click((e) => {
        $("#addTabDialog").css('display', 'none');
        var tabTitle = $("#tab_title").val();
        $("#tab_title").val("");
        if (tabTitle.trim() == "")
            tabTitle = "NewTab";
        var tabType = $("input[name='tab_type']:checked").val();
        addTab(tabTitle, true, tabType);
    });
}
function addTab(name, select, tabtype) {
    $("#mainTabs").append(String.format(mainTabTemplate, mainTabCounter, tabtype, name, select ? "tabSelected" : ""));
    $("#mainContainer").append(getTabContent(tabtype));
    var tab_id = "#tab-" + mainTabCounter;
    if (select) {
        $("#mainTabs>.tab").removeClass("tabSelected");
        $("#mainTabs>.tab:last-child").addClass("tabSelected");
        $("#mainContainer>.tabBodyContainer").css('display', 'none');
        $("#mainContainer>" + tab_id).css('display', 'grid');
    } else {
        $("#mainContainer>#tab-" + mainTabCounter).css('display', 'none');
    }
    mainTabCounter++;
    registerMainTabListeners();
    registerTabContentListener(tab_id, tabtype);
}
function registerMainTabListeners() {
    $("#mainTabs>.tab").unbind('click').click((e) => {
        var target = $(e.target).closest('.tab');
        if (target.hasClass('tabSelected'))
            return;
        var tab_id = target.attr('tab_id');
        $("#mainTabs>.tab").removeClass("tabSelected");
        $("#mainContainer>.tabBodyContainer").css('display', 'none');
        target.addClass("tabSelected");
        $("#" + tab_id).css('display', 'grid');
    });
    $("#mainTabs>.tab>i").unbind('click').click((e) => {
        var target = $(e.target).closest('.tab');
        var count = $("#mainTabs>.tab").length;
        if (count == 1) {
            mainTabCounter = 0;
            $("#mainTabs").html("");
            $("#mainContainer").html("");
            addTab("Tab 1", true);
            e.stopPropagation();
            return;
        }
        var tab_id = target.attr('tab_id');
        var tabSelected = target.hasClass("tabSelected");
        target.remove();
        $("#" + tab_id).remove();
        if (tabSelected) {
            tab_id = $("#mainTabs>.tab:first-child").addClass("tabSelected").attr('tab_id');
            $("#" + tab_id).css('display', 'grid');
        }
    });
}

//
// Executer Region 
// Start
//

var mainTabExecuterBodyTemplate = '<div class="tabBodyContainer" id="tab-{0}"><div class="editor-section"><div class="editor-sidebar"><i class="execute-button">Execute</i></div>'
    + '<div class="editor"><textarea></textarea></div></div><div class="result-section"></div></div>';
var editorResultTabTemplate = '<div class="tab" tab_id="{0}-childtab-{1}">{2}</div>';
var editorResultSectionTabContentTemplate = '<div class="tabBodyContainer" id="{0}-childtab-{1}"><table></table></div>';
var editorResultNoRowsSectionTabContentTemplate = '<div class="tabBodyContainer" id="{0}-childtab-{1}"><div class="result-message">No Rows returned</div></div>';

function registerExecuterListeners(tab_id) {
    $(tab_id + " .execute-button").click((e) => {
        var textarea = $(e.target).closest(".editor-section").find("textarea");
        var tabID = "#" + $(e.target).closest(".tabBodyContainer").attr("id");
        var query = textarea.val();
        if (query.trim().length <= 0)
            return;
        console.log("postQuery " + tabID + " with query " + query);
        var param = {};
        param.query = query;
        CallCommonServiceMethod('ExecuteQuery', param, (result) => {
            var data = JSON.parse(result.d);
            console.log("success ");
            console.log(data);
            let keys = Object.keys(data);
            if (keys.length == 0) {
                displayMessageForExecuter(tabID, "Success. No Table Returned.")
                return;
            }
            populateExecuterResult(tabID, data);
        }, (e) => {
            displayMessageForExecuter(tabID, "Error: " + e.responseText);
            console.log(e);
        }, null, false);

    });
}
function displayMessageForExecuter(tabID, message) {
    $(tabID + " .result-section").html("<div class='result-message'>" + message + "</div>");
}
function populateExecuterResult(tabID, tables) {
    $(tabID + " .result-section").html('<div class="tabsContainer"></div>' + '<div class="tabsContentParent"></div>');
    let tableNames = Object.keys(tables);
    tableNames.forEach((tableName, index) => {
        addExecuterResultTab(tabID, tables[tableName], tableName, index);
    });
    $(tabID + " .result-section .tabsContainer>.tab:first-child").addClass("tabSelected");
    var firstTabContainer = $(tabID + " .result-section .tabsContentParent>.tabBodyContainer:first-child");
    firstTabContainer.css('display', 'block');
    firstTabContainer.find("table").DataTable()
        .columns.adjust();
    registerExecuterResultTabListeners(tabID);
}
function addExecuterResultTab(tabID, table, tableName, index) {
    var tab_id = tabID.substring(1);
    $(tabID + " .result-section .tabsContainer").append(String.format(editorResultTabTemplate, tab_id, index, tableName));
    if (table.length == 0) {
        $(tabID + " .result-section .tabsContentParent").append(String.format(editorResultNoRowsSectionTabContentTemplate, tab_id, index));
        return;
    }
    let columns = Object.keys(table[0]);
    $(tabID + " .result-section .tabsContentParent").append(String.format(editorResultSectionTabContentTemplate, tab_id, index));
    var tableColumn = [];
    columns.forEach((column, _) => {
        tableColumn.push({ title: column, data: column });
    });
    $(tabID + "-childtab-" + index + " table").DataTable({
        data: table,
        columns: tableColumn,
        paging: true,
        orderable: true,
        info: false,
        scrollX: '100vw',
        scrollY: 'calc(70vh - 170px)'
    });
}
function registerExecuterResultTabListeners(tabID) {
    $(tabID + " .result-section .tabsContainer>.tab").click((e) => {
        var target = $(e.target).closest('.tab');
        if (target.hasClass('tabSelected'))
            return;
        var tab_id = target.attr('tab_id');
        var tabContainer = target.closest(".result-section").find(".tabsContentParent");
        var selectedTabBodyContainer = tabContainer.find("#" + tab_id);
        target.siblings().removeClass("tabSelected");
        tabContainer.find(".tabBodyContainer").css('display', 'none');
        target.addClass("tabSelected");
        selectedTabBodyContainer.css('display', 'block');
        selectedTabBodyContainer.find("table").DataTable()
            .columns.adjust();
    });
}
//
// Executer Region 
// END
//


