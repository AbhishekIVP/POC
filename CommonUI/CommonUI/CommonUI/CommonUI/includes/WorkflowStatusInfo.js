
var WorkflowStatusService = {};

WorkflowStatusService.GetGridData = {};
WorkflowStatusService.GetGridData.MethodName = "GetGridData";
WorkflowStatusService.GetGridData.Parameters = {};
WorkflowStatusService.GetGridData.Parameters.inputObject = "inputObject";

var WorkflowActionMapping = {};
WorkflowActionMapping["Approve"] = 0;
WorkflowActionMapping["Reject"] = 1;
WorkflowActionMapping["Delete"] = 2;
WorkflowActionMapping["Suppress"] = 3;
WorkflowActionMapping["Revoke"] = 4;

var WorkflowRequestTextMapping = {};
WorkflowRequestTextMapping["Approve"] = "Approved";
WorkflowRequestTextMapping["Reject"] = "Rejected";
WorkflowRequestTextMapping["Suppress"] = "Suppressed";
WorkflowRequestTextMapping["Delete"] = "Deleted";
WorkflowRequestTextMapping["Revoke"] = "Revoked";

var WorkflowActionGridType = {};
WorkflowActionGridType["AllRequests"] = "AllRequests";
WorkflowActionGridType["MyRequests"] = "MyRequests";
WorkflowActionGridType["RejectedRequests"] = "RejectedRequests";
WorkflowActionGridType["RequestsPending"] = "RequestsPending";

var WorkFlowRejectedRequestsGridId = "workflowRejectedRequestsGrid";
var WorkFlowRequestsPendingGridId = "workflowRequestsPendingGrid";

var WorkflowTabs = {};
WorkflowTabs["SecMaster"] = "SecMaster";
WorkflowTabs["RefMaster"] = "RefMaster";

function SMSWorkflowStatusControls(_controlInfo) {
    this._controlInfo = _controlInfo;
}
SMSWorkflowStatusControls.prototype = {
    _controlInfo: null,

    _txtAttributeName: null,
    TxtAttributeName: function get_txtAttributeName() {
        if (this._txtAttributeName == null || this._txtAttributeName.length == 0)
            this._txtAttributeName = $("#txtAttributeName");
        return this._txtAttributeName;
    },

    _ddlAction: null,
    DdlAction: function get_ddlActionId() {
        if (this._ddlAction == null || this._ddlAction.length == 0)
            this._ddlAction = $("#" + this._controlInfo.DdlActionId);
        return this._ddlAction;
    }
}