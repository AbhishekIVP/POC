<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMWorkflowInbox.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMWorkflowInbox"
    MasterPageFile="~/CommonUI/SRMMaster.Master" EnableEventValidation="false" %>

<asp:Content ID="contentFilterSection" ContentPlaceHolderID="ContentPlaceHolderFilterSection" runat="server">
    <div class="grid-filter-buttons hidden">
        <i id="gridRefereshBtn" class="fa fa-refresh grid-filter-button" title="Click to refresh grid" onclick="SRMWorkflowInboxDetails.RefreshClickHandler();"></i>
        <i id="filterPivot" class="fa fa-filter grid-filter-button" title="Filter"></i>
        <div id="rightFilterDiv" class="smslidemenu_filterDiv" state="open"></div>
    </div>
</asp:Content>

<asp:Content ID="contentMiddle" ContentPlaceHolderID="SRMContentPlaceHolderMiddle" runat="server">
    <script src="includes/SMSlideMenu.js" type="text/javascript"></script>
    <script src="includes/SRMWorkflowInbox.js" type="text/javascript"></script>
    <script src="includes/SRMWorkflowInboxJS/SRMWorkflowInboxDetails.js" type="text/javascript"></script>
    <script src="includes/SRMUniquenessFailure.js" type="text/javascript"></script>
    <link href="css/thirdparty/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="css/SRMWorkflowInbox.css" rel="stylesheet" />
    <link href="css/SRMUniquenessFailure.css" rel="stylesheet" />
    <link href="css/SMSlideMenu.css" rel="stylesheet" />

    <div id="SRMWorkflowInboxParent" class="SRMWorkflowInboxParent">
        <div id="SRMWorkflowInboxGrid" class="SRMWorkflowInboxGrid">
            <div class="SRMWorkflowInboxHeaderParent SRMWorkflowInboxCommonParent">
                <div class="SRMWorkflowInboxHeaderRow SRMWorkflowInboxRow">
                    <div class="SRMWorkflowInboxColumn SRMWorkflowInboxHeaderColumn">
                        <div class="SRMWorkflowInboxHeaderItem SRMWorkflowInboxItem">
                            <div class="SRMWorkflowInboxHeaderText SRMWorkflowInboxContentText">PENDING AT ME</div>
                        </div>
                    </div>
                    <div class="SRMWorkflowInboxColumn SRMWorkflowInboxHeaderColumn">
                        <div class="SRMWorkflowInboxHeaderItem SRMWorkflowInboxItem">
                            <div class="SRMWorkflowInboxHeaderText SRMWorkflowInboxContentText">REJECTED REQUESTS</div>
                        </div>
                    </div>
                    <div class="SRMWorkflowInboxColumn SRMWorkflowInboxHeaderColumn">
                        <div class="SRMWorkflowInboxHeaderItem SRMWorkflowInboxItem">
                            <div class="SRMWorkflowInboxHeaderText SRMWorkflowInboxContentText">MY REQUESTS</div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="SRMWorkflowInboxContentParent SRMWorkflowInboxCommonParent" data-bind="foreach: subModuleList">
                <div class="SRMWorkflowInboxContentRow SRMWorkflowInboxRow">
                    <div class="SRMWorkflowInboxColumn">
                        <div class="SRMWorkflowInboxContentItem SRMWorkflowInboxItem">
                            <div class="SRMWorkflowInboxContentText SRMWorkflowInboxSubTypeText ">
                                <div class="SRMWorkflowInboxContentTextInline SRMWorkflowInboxContentTextLeft SRMWorkflowInboxSubTypeTextWhiteFont" data-bind="text: workflowType"></div>
                                <div class="SRMWorkflowInboxContentText SRMWorkflowInboxSubTypeText SRMWorkflowInboxSubTypeTextWhiteFont SRMWorkflowInboxContentTextRight " data-bind="text: pendingCount > 9 ? pendingCount : '0' + pendingCount, click: onClickCount, attr: { 'clickedSubType': workflowType, 'clickedActionType': 'Pending At Me' }"></div>
                            </div>
                        </div>
                    </div>
                    <div class="SRMWorkflowInboxColumn">
                        <div class="SRMWorkflowInboxContentItem SRMWorkflowInboxItem">
                            <div class="SRMWorkflowInboxContentText SRMWorkflowInboxSubTypeText ">
                                <div class="SRMWorkflowInboxContentTextInline SRMWorkflowInboxContentTextLeft SRMWorkflowInboxSubTypeTextWhiteFont" data-bind="text: workflowType"></div>
                                <div class="SRMWorkflowInboxContentText SRMWorkflowInboxSubTypeText SRMWorkflowInboxSubTypeTextWhiteFont SRMWorkflowInboxContentTextRight" data-bind="text: rejectedCount > 9 ? rejectedCount : '0' + rejectedCount, click: onClickCount, attr: { 'clickedSubType': workflowType, 'clickedActionType': 'Rejected Requests' } "></div>
                            </div>
                        </div>
                    </div>
                    <div class="SRMWorkflowInboxColumn">
                        <div class="SRMWorkflowInboxContentItem SRMWorkflowInboxItem">
                            <div class="SRMWorkflowInboxContentText SRMWorkflowInboxSubTypeText ">
                                <div class="SRMWorkflowInboxContentTextInline SRMWorkflowInboxContentTextLeft SRMWorkflowInboxSubTypeTextWhiteFont" data-bind="text: workflowType"></div>
                                <div class="SRMWorkflowInboxContentText SRMWorkflowInboxSubTypeText SRMWorkflowInboxSubTypeTextWhiteFont SRMWorkflowInboxContentTextRight" data-bind="text: myRequestsCount > 9 ?
        myRequestsCount : '0' + myRequestsCount, click: onClickCount, attr: { 'clickedSubType': workflowType, 'clickedActionType': 'My Requests' }">
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>

    <div id="SRMWorkflowInboxDetails" class="SRMWorkflowInboxDetails hidden">
        <div class="top-div">
            <%--<i class="fa fa-close"  onclick="SRMWorkflowInboxDetails.CloseDetailsGrid();"></i>--%>
            <div title="Close" class="close-details-grid-btn" onclick="SRMWorkflowInboxDetails.CloseDetailsGrid();">
                <span class="fa fa-caret-left btn-icon"></span><span class="btn-content">Back</span>
            </div>
        </div>
        <div class="inbox-sections" data-bind="foreach: sectionsList">
            <div class="inbox-section" data-bind="text: sectionName, attr: { 'data-SectionId': sectionId }, css: { 'active-section': isActive() }, click: clickHandler"></div>
        </div>
        <div class="inbox-header"></div>
        <div class="grid-header-row">
            <%--<div class="grid-header grid-select-all">
                <input type="checkbox" data-bind="checked: SelectAll"/>
                <div>Select All</div>
            </div>--%>
            <div class="grid-search">
                <i class="fa fa-search search-icon"></i>
                <input type="text" data-bind="value: SearchText, valueUpdate: 'afterkeydown'" placeholder="Enter search text" />
            </div>
            <div class="workflow-actions" data-bind="foreach: workflowActionsList">
                <div class="workflow-action" data-bind="attr: { 'class': 'workflow-action ' + ActionName }">
                    <div data-bind="text: ActionName, click: onActionBtnClick"></div>
                    <div class="action-popup" data-bind="css: { 'hidden': PopupHidden() }">
                        <i class="fa fa-close" title="Close Action Popup" data-bind="click: onPopupCancel"></i>
                        <div class="action-remarks">
                            <textarea data-bind="textInput: ActionRemarks" placeholder="Enter remarks here.."></textarea>
                        </div>
                        <div class="action-error" data-bind="html: ActionError"></div>
                        <div class="action-popup-buttons">
                            <%--<div class="workflow-action cancel-action-btn" data-bind="click: onPopupCancel">Close</div>--%>
                            <div class="workflow-action" data-bind="click: onPopupAction">Continue</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="inbox-grid" data-bind="foreach: workflowsList">
            <div class="inbox-grid-row" data-bind="css: { 'is-filtered-out': IsFilteredOut() }">
                <div class="row-checkbox">
                    <input type="checkbox" data-bind="checked: IsSelected" />
                </div>
                <div class="info-panel">
                    <div class="info-container">
                        <div class="primary-info-container">
                            <div class="primary-info instrument-id" data-bind="text: InstrumentId, attr: { 'title': InstrumentId }, click: InstrumentIdClickHandler"></div>
                            <div class="primary-info type-name" data-bind="text: TypeName, attr: { 'title': TypeName }"></div>
                            <div class="primary-info primary-identifier-value" data-bind="text: PrimaryIdentifierValue, attr: { 'title': PrimaryIdentifierValue }"></div>
                        </div>
                        <div class="meta-info-container">
                            <div class="meta-info effective-on" data-bind="attr: { 'title': 'Effective from : ' + SRMWorkflowInboxDetails.ConvertDate(EffectiveStartDate) }">
                                Effective from : <span data-bind="text: SRMWorkflowInboxDetails.ConvertDate(EffectiveStartDate)"></span>
                            </div>
                            <div class="meta-info requested-by" data-bind="attr: { 'title': 'Requested By : ' + RequestedBy + ' on ' + SRMWorkflowInboxDetails.ConvertDateTime(RequestedOn, true) }">
                                Requested By : <span data-bind="text: RequestedBy"></span>on <span data-bind="    text: SRMWorkflowInboxDetails.ConvertDateTime(RequestedOn, true)"></span>
                            </div>
                            <div class="meta-info view-action-history" title="View Action History" data-bind="click: ActionHistoryClickHandler"></div>
                        </div>
                    </div>
                </div>
                <div class="attribute-info-panel" data-bind="foreach: WorkflowAttributes">
                    <div class="attribute">
                        <div class="attribute-name" data-bind="text: AttributeName, attr: { 'title': AttributeName }"></div>
                        <div class="attribute-value" data-bind="text: AttributeValue, attr: { 'title': AttributeValue }"></div>
                    </div>
                </div>
            </div>
        </div>
        <div class="workflow-action-history hidden">
            <div class="top-div action-history-popup-header">
                <div class="action-history-popup-title">Action History</div>
                <i class="fa fa-close" title="Close Action History" onclick="SRMWorkflowInboxDetails.CloseActionHistory();"></i>
            </div>
            <div class="action-history-iframe-container">
                <iframe class="action-history-iframe"></iframe>
            </div>
        </div>
        <div class="message-popup-container hidden">
            <div class="message-popup" data-bind="attr: { 'class': 'message-popup ' + messageType() }">
                <div class="popup-title-bar">
                    <div class="popup-title">
                        <div class="title-image"></div>
                        <div class="title-text" data-bind="text: messageTitle"></div>
                    </div>
                    <div class="popup-close-btn fa fa-close" onclick="SRMWorkflowInboxDetails.CloseMessagePopup();"></div>
                </div>
                <div class="popup-content" data-bind="html: messageContent"></div>
                <div id="popupFailureGrid" class="popup-failure-grid hidden"></div>
            </div>
        </div>
        <div id="uniquenessInfoPopup" class="uniqueness-info-popup hidden"></div>
    </div>

</asp:Content>
