
var path = window.location.protocol + '//' + window.location.host;
var pathname = window.location.pathname.split('/');

$.each(pathname, function (ii, ee) {
    if ((ii !== 0) && (ii !== pathname.length - 1))
        path = path + '/' + ee;
});
//path = path.substring(0, path.length - 9);

String.prototype.replaceAll = function (find, replace) {
    return this.replace(new RegExp(find, 'g'), replace);
}

var dateFilterTextStatic = {};
dateFilterTextStatic[0] = "Today";
dateFilterTextStatic[1] = "Since Yesterday";
dateFilterTextStatic[2] = "This Week";
dateFilterTextStatic[3] = "Any Time";

var TopBarTabsMapping = {};
TopBarTabsMapping["Securities"] = "Secmaster";
TopBarTabsMapping["CorpAction"] = "Corpaction";
TopBarTabsMapping["Operational Analytics"] = "ManagementAnalytics";
TopBarTabsMapping["RefData"] = "Refmaster";
TopBarTabsMapping["Funds"] = "Funds";
TopBarTabsMapping["Parties"] = "Parties";

var DashboardTabs = {};
DashboardTabs.Secmaster = "Secmaster";
DashboardTabs.Corpaction = "Corpaction";
DashboardTabs.Refmaster = "Refmaster";
DashboardTabs.Funds = "Funds";
DashboardTabs.Parties = "Parties";
DashboardTabs.ManagementAnalytics = "ManagementAnalytics";

var DemoDashboardMapping = {};
DemoDashboardMapping.Secmaster = false;
DemoDashboardMapping.Corpaction = true;
DemoDashboardMapping.Refmaster = false;
DemoDashboardMapping.Funds = false;
DemoDashboardMapping.Parties = false;
DemoDashboardMapping.ManagementAnalytics = true;

var TabModuleMapping = {};
TabModuleMapping[DashboardTabs.Secmaster] = null;
TabModuleMapping[DashboardTabs.Corpaction] = null;
TabModuleMapping[DashboardTabs.ManagementAnalytics] = null;
TabModuleMapping[DashboardTabs.Refmaster] = 6;
TabModuleMapping[DashboardTabs.Funds] = 18;
TabModuleMapping[DashboardTabs.Parties] = 20;

var saveDateFilter = {};
saveDateFilter[DashboardTabs.Secmaster] = true;
saveDateFilter[DashboardTabs.Refmaster] = true;
saveDateFilter[DashboardTabs.Funds] = true;
saveDateFilter[DashboardTabs.Parties] = true;

var DateFilterId = {}
DateFilterId[DashboardTabs.Secmaster] = "SecDateFilter";
DateFilterId[DashboardTabs.Refmaster] = "RefDateFilter";
DateFilterId[DashboardTabs.Funds] = "FMDateFilter";
DateFilterId[DashboardTabs.Parties] = "PMDateFilter";

var DateFilterNeedsRefreshing = {};
DateFilterNeedsRefreshing[DashboardTabs.Secmaster] = false;
DateFilterNeedsRefreshing[DashboardTabs.Refmaster] = false;
DateFilterNeedsRefreshing[DashboardTabs.Funds] = false;
DateFilterNeedsRefreshing[DashboardTabs.Parties] = false;

var HasDateFilter = {};
HasDateFilter[DashboardTabs.Secmaster] = true;
HasDateFilter[DashboardTabs.Corpaction] = false;
HasDateFilter[DashboardTabs.Refmaster] = true;
HasDateFilter[DashboardTabs.Funds] = true;
HasDateFilter[DashboardTabs.Parties] = true;
HasDateFilter[DashboardTabs.ManagementAnalytics] = false;

var DashboardInitialized = {};
DashboardInitialized[DashboardTabs.Secmaster] = false;
DashboardInitialized[DashboardTabs.Corpaction] = false;
DashboardInitialized[DashboardTabs.Refmaster] = false;
DashboardInitialized[DashboardTabs.Funds] = false;
DashboardInitialized[DashboardTabs.Parties] = false;
DashboardInitialized[DashboardTabs.ManagementAnalytics] = false;

var iframeDashboard = {};
iframeDashboard[DashboardTabs.Secmaster] = "iframeSecMDashboard";
iframeDashboard[DashboardTabs.Corpaction] = "iframeCorpDashboard";
iframeDashboard[DashboardTabs.Refmaster] = "iframeRefMDashboard";
iframeDashboard[DashboardTabs.Funds] = "iframeFMDashboard";
iframeDashboard[DashboardTabs.Parties] = "iframePMDashboard";
iframeDashboard[DashboardTabs.ManagementAnalytics] = "iframeManagementAnalyticsDashboard";

var DashboardAspxName = {};
DashboardAspxName[DashboardTabs.Secmaster] = "SMDashboard.aspx";
DashboardAspxName[DashboardTabs.Corpaction] = "CADashboard.aspx";
DashboardAspxName[DashboardTabs.Refmaster] = "RMDashboard.aspx";
DashboardAspxName[DashboardTabs.Parties] = "RMDashboard.aspx";
DashboardAspxName[DashboardTabs.Funds] = "RMDashboard.aspx";
DashboardAspxName[DashboardTabs.ManagementAnalytics] = "SMManagementAnalyticsDashboard.aspx";

var DashboardPageIdentifier = {};
DashboardPageIdentifier[DashboardTabs.Secmaster] = "SMDashboard";
DashboardPageIdentifier[DashboardTabs.Corpaction] = "SMDashboard";
DashboardPageIdentifier[DashboardTabs.Refmaster] = null;
DashboardPageIdentifier[DashboardTabs.Parties] = null;
DashboardPageIdentifier[DashboardTabs.Funds] = null;
DashboardPageIdentifier[DashboardTabs.ManagementAnalytics] = "ManagementAnalyticsDashboard";

var iframeSubtractHeight = {};
iframeSubtractHeight[DashboardTabs.Secmaster] = 40;
iframeSubtractHeight[DashboardTabs.Corpaction] = 40;
iframeSubtractHeight[DashboardTabs.Refmaster] = 40;
iframeSubtractHeight[DashboardTabs.Parties] = 40;
iframeSubtractHeight[DashboardTabs.Funds] = 40;
iframeSubtractHeight[DashboardTabs.ManagementAnalytics] = 60;

var DashboardFiltersSectionMapping = {};
DashboardFiltersSectionMapping[DashboardTabs.Secmaster] = "SMDashboardFilterSection";
DashboardFiltersSectionMapping[DashboardTabs.Corpaction] = "CADashboardFilterSection";
DashboardFiltersSectionMapping[DashboardTabs.Refmaster] = "RefDashboardFilterSection";
DashboardFiltersSectionMapping[DashboardTabs.Parties] = "PMDashboardFilterSection";
DashboardFiltersSectionMapping[DashboardTabs.Funds] = "FMDashboardFilterSection";
DashboardFiltersSectionMapping[DashboardTabs.ManagementAnalytics] = "ManagementAnalyticsDashboardFilterSection";

var DashboardDivContainersMapping = {};
DashboardDivContainersMapping[DashboardTabs.Secmaster] = "SMDashBoardDivContainer";
DashboardDivContainersMapping[DashboardTabs.Corpaction] = "CADashBoardDivContainer";
DashboardDivContainersMapping[DashboardTabs.Refmaster] = "RefDashBoardDivContainer";
DashboardDivContainersMapping[DashboardTabs.Parties] = "PMDashBoardDivContainer";
DashboardDivContainersMapping[DashboardTabs.Funds] = "FMDashBoardDivContainer";
DashboardDivContainersMapping[DashboardTabs.ManagementAnalytics] = "ManagementAnalyticsDivContainer";

var DashboardSaveButtonId = {};
DashboardSaveButtonId[DashboardTabs.Secmaster] = "SMDashboardSaveButton";
DashboardSaveButtonId[DashboardTabs.Corpaction] = null;
DashboardSaveButtonId[DashboardTabs.Refmaster] = "RefDashboardSaveButton";
DashboardSaveButtonId[DashboardTabs.Parties] = "PMDashboardSaveButton";
DashboardSaveButtonId[DashboardTabs.Funds] = "FMDashboardSaveButton";
DashboardSaveButtonId[DashboardTabs.ManagementAnalytics] = null;

var DashboardRefreshButtonId = {};
DashboardRefreshButtonId[DashboardTabs.Secmaster] = "SMDashboardRefreshButton";
DashboardRefreshButtonId[DashboardTabs.Corpaction] = null;
DashboardRefreshButtonId[DashboardTabs.Refmaster] = "RefDashboardRefreshButton";
DashboardRefreshButtonId[DashboardTabs.Parties] = "PMDashboardRefreshButton";
DashboardRefreshButtonId[DashboardTabs.Funds] = "FMDashboardRefreshButton";
DashboardRefreshButtonId[DashboardTabs.ManagementAnalytics] = null;

var DashboardRightFilterId = {};
DashboardRightFilterId[DashboardTabs.Secmaster] = "SMDashboardRightFilter";
DashboardRightFilterId[DashboardTabs.Corpaction] = "CADashboardRightFilter";
DashboardRightFilterId[DashboardTabs.Refmaster] = "RefDashboardRightFilter";
DashboardRightFilterId[DashboardTabs.Parties] = "PMDashboardRightFilter";
DashboardRightFilterId[DashboardTabs.Funds] = "FMDashboardRightFilter";
DashboardRightFilterId[DashboardTabs.ManagementAnalytics] = null;