/// <reference path="/js/jquery-1.6.2.js" />
/// <reference path="/js/jquery-ui-1.8.16.custom.min.js" />
/// <reference path="/includes/SecMasterScripts.js" />

function SMSTaskStatusControls(controlIdInfo) {
    this._controlIdInfo = controlIdInfo;
}

SMSTaskStatusControls.prototype = {
    _controlIdInfo: null,

    _rblstDate: null,
    RblstDate: function get_rblstDate() {
        if (this._rblstDate == null || this._rblstDate.length == 0) {
            this._rblstDate = $('#' + this._controlIdInfo.RblstDateId);
        }
        return this._rblstDate;
    },

    _lblStartDate: null,
    LblStartDate: function get_lblStartDate() {
        if (this._lblStartDate == null || this._lblStartDate.length == 0) {
            this._lblStartDate = $('#' + this._controlIdInfo.LblStartDateId);
        }
        return this._lblStartDate;
    },

    _txtStartDate: null,
    TxtStartDate: function get_txtStartDate() {
        if (this._txtStartDate == null || this._txtStartDate.length == 0) {
            this._txtStartDate = $('#' + this._controlIdInfo.TxtStartDateId);
        }
        return this._txtStartDate;
    },

    _lblStartDateError: null,
    LblStartDateError: function get_lblStartDateError() {
        if (this._lblStartDateError == null || this._lblStartDateError.length == 0) {
            this._lblStartDateError = $('#' + this._controlIdInfo.LblStartDateErrorId);
        }
        return this._lblStartDateError;
    },

    _lblEndDate: null,
    LblEndDate: function get_lblEndDate() {
        if (this._lblEndDate == null || this._lblEndDate.length == 0) {
            this._lblEndDate = $('#' + this._controlIdInfo.LblEndDateId);
        }
        return this._lblEndDate;
    },

    _txtEndDate: null,
    TxtEndDate: function get_txtEndDate() {
        if (this._txtEndDate == null || this._txtEndDate.length == 0) {
            this._txtEndDate = $('#' + this._controlIdInfo.TxtEndDateId);
        }
        return this._txtEndDate;
    },

    _lblEndDateError: null,
    LblEndDateError: function get_lblEndDateError() {
        if (this._lblEndDateError == null || this._lblEndDateError.length == 0) {
            this._lblEndDateError = $('#' + this._controlIdInfo.LblEndDateErrorId);
        }
        return this._lblEndDateError;
    },

    _lblValidationSummary: null,
    LblValidationSummary: function get_lblValidationSummary() {
        if (this._lblValidationSummary == null || this._lblValidationSummary.length == 0) {
            this._lblValidationSummary = $('#' + this._controlIdInfo.LblValidationSummaryId);
        }
        return this._lblValidationSummary;
    },

    _btnGetStatus: null,
    BtnGetStatus: function get_btnGetStatus() {
        if (this._btnGetStatus == null || this._btnGetStatus.length == 0) {
            this._btnGetStatus = $('#' + this._controlIdInfo.BtnGetStatusId);
        }
        return this._btnGetStatus;
    },

    _txtTimer: null,
    TxtTimer: function get_txtTimer() {
        if (this._txtTimer == null || this._txtTimer.length == 0) {
            this._txtTimer = $('#' + this._controlIdInfo.TxtTimerId);
        }
        return this._txtTimer;
    },

    _hdnTimer: null,
    HdnTimer: function get_hdnTimer() {
        if (this._hdnTimer == null || this._hdnTimer.length == 0) {
            this._hdnTimer = $('#' + this._controlIdInfo.HdnTimerId);
        }
        return this._hdnTimer;
    },

    _lblTaskNameToDelete: null,
    LblTaskNameToDelete: function get_lblTaskNameToDelete() {
        if (this._lblTaskNameToDelete == null || this._lblTaskNameToDelete.length == 0) {
            this._lblTaskNameToDelete = $('#' + this._controlIdInfo.LblTaskNameToDeleteId);
        }
        return this._lblTaskNameToDelete;
    },

    _txtDeleteId: null,
    TxtDeleteId: function get_txtDeleteId() {
        if (this._txtDeleteId == null || this._txtDeleteId.length == 0) {
            this._txtDeleteId = $('#' + this._controlIdInfo.TxtDeleteIdId);
        }
        return this._txtDeleteId;
    },

    _txtName: null,
    TxtName: function get_txtName() {
        if (this._txtName == null || this._txtName.length == 0) {
            this._txtName = $('#' + this._controlIdInfo.TxtNameId);
        }
        return this._txtName;
    },

    _txtDescription: null,
    TxtDescription: function get_txtDescription() {
        if (this._txtDescription == null || this._txtDescription.length == 0) {
            this._txtDescription = $('#' + this._controlIdInfo.TxtDescriptionId);
        }
        return this._txtDescription;
    },

    _lblPanelHeader: null,
    LblPanelHeader: function get_lblPanelHeader() {
        if (this._lblPanelHeader == null || this._lblPanelHeader.length == 0) {
            this._lblPanelHeader = $('#' + this._controlIdInfo.LblPanelHeaderId);
        }
        return this._lblPanelHeader;
    },

    _imgBtnHlp1: null,
    ImgBtnHlp1: function get_imgBtnHlp1() {
        if (this._imgBtnHlp1 == null || this._imgBtnHlp1.length == 0) {
            this._imgBtnHlp1 = $('#' + this._controlIdInfo.ImgBtnHlp1Id);
        }
        return this._imgBtnHlp1;
    },

    _gridTaskStatusDetails1: null,
    GridTaskStatusDetails1: function get_gridTaskStatusDetails1() {
        if (this._gridTaskStatusDetails1 == null || this._gridTaskStatusDetails1.length == 0) {
            this._gridTaskStatusDetails1 = $('#' + this._controlIdInfo.GridTaskStatusDetails1Id);
        }
        return this._gridTaskStatusDetails1;
    },

    _panelLogDetails: null,
    PanelLogDetails: function get_panelLogDetails() {
        if (this._panelLogDetails == null || this._panelLogDetails.length == 0) {
            this._panelLogDetails = $('#' + this._controlIdInfo.PanelLogDetailsId);
        }
        return this._panelLogDetails;
    },

    _panelDeleteTask: null,
    PanelDeleteTask: function get_panelDeleteTask() {
        if (this._panelDeleteTask == null || this._panelDeleteTask.length == 0) {
            this._panelDeleteTask = $('#' + this._controlIdInfo.PanelDeleteTaskId);
        }
        return this._panelDeleteTask;
    },

    _hdnPostBackControl: null,
    HdnPostBackControl: function get_hdnPostBackControl() {
        if (this._hdnPostBackControl == null || this._hdnPostBackControl.length == 0) {
            this._hdnPostBackControl = $('#' + this._controlIdInfo.HdnPostBackControlId);
        }
        return this._hdnPostBackControl;
    },

    _btnOk: null,
    BtnOk: function get_btnOk() {
        if (this._btnOk == null || this._btnOk.length == 0) {
            this._btnOk = $('#' + this._controlIdInfo.BtnOkId);
        }
        return this._btnOk;
    },

    _btnCancelDelete: null,
    BtnCancelDelete: function get_btnCancelDelete() {
        if (this._btnCancelDelete == null || this._btnCancelDelete.length == 0) {
            this._btnCancelDelete = $('#' + this._controlIdInfo.BtnCancelDeleteId);
        }
        return this._btnCancelDelete;
    }
}