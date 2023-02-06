<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkflowActionHistory.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.WorkflowActionHistory" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="css/thirdparty/font-awesome.css" rel="stylesheet" type="text/css" />
    <link href="css/WorkflowActionHistory.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="formWorkflowActionHistoryId" runat="server">

        <asp:ScriptManager ID="smWorkflowActionHistoryScriptManager" runat="server" ScriptMode="debug">
            <Scripts>
                <asp:ScriptReference Name="com.ivp.rad.raduicontrols.Scripts.RADCommonScripts.js"
                    Assembly="RADUIControls" />
                <asp:ScriptReference Name="com.ivp.rad.raduicontrols.Scripts.RADUIScripts.debug.js"
                    Assembly="RADUIControls" />
                <asp:ScriptReference Name="com.ivp.rad.raduicontrols.Scripts.RScriptUtils.debug.js"
                    Assembly="RADUIControls" />
            </Scripts>
        </asp:ScriptManager>

        <%--<div id="WF_AH_Temp1">
            <div id="WF_AH_TempClickDiv">Click to Show Div</div>--%>

        <div id="WFAH_MainBody" class="hideInitially">

            <%--Top Div--%>
            <%--<div id="WFAH_TopDiv">
                    <div id="WFAH_TopDivLabel" class="WFAH_Top_Divs">ACTION HISTORY</div>
                    <!--<div id="WFAH_TopCloseButton" class="WFAH_Top_Divs">
                        <i class="fa fa-times"></i>
                    </div>-->
                </div>--%>

            <%--Main Div--%>
            <div id="WFAH_MainDiv">

                <%--<div id="WFAH_RightBorderDiv">
                    </div>--%>
                <div id="WFAH_InstancesParentDiv" data-bind="visible: instancesListing().length > 0, foreach: instancesListing">

                    <div class="WFAH_InstanceTextParentDiv" data-bind="click: workflowActionHistory.onClickInstance">
                        <div class="WFAH_ArrowDiv">
                            <i class="fa fa-caret-right" aria-hidden="true" data-bind="visible: !toShowInstance()"></i>
                            <i class="fa fa-caret-down" aria-hidden="true" data-bind="visible: toShowInstance()"></i>
                        </div>
                        <div class="WFAH_InstanceText" data-bind="text: instanceText">
                        </div>
                    </div>

                    <div class="WFAH_ActionsParentDivClass" data-bind="visible: toShowInstance(), foreach: actionListing">
                        <div class="WFAH_ActionRowClassParent">
                            <div class="WFAH_ActionRowClass">

                                <div class="WFAH_ActionRowTopClass">

                                    <%--Images - INCOMPLETE--%>
                                    <div class="WFAH_ActionImageClass WFAH_Action_Divs">
                                        <div data-bind="attr: { class: actionImageClass }, style: { color: actionColor }">
                                        </div>
                                    </div>

                                    <%--Main Data--%>
                                    <div class="WFAH_ActionMainCSSClass WFAH_Action_Divs">
                                        <div class="WFAH_ActionMainDataClass" data-bind="click: workflowActionHistory.onClickAction, visible: actionPendingLevel <= 0, style: { borderTop: actionBorderTop, cursor: actionClickable }">

                                            <%--Action Status--%>
                                            <div class="WFAH_ActionCol1 WFAH_Action_InternalDivs WFAH_Ellipses" data-bind="text: actionStatus, style: { color: actionColor }, attr: { title: actionStatus }">
                                            </div>

                                            <%--Action By--%>
                                            <div class="WFAH_ActionCol2 WFAH_Action_InternalDivs">
                                                <div class="WFAH_ActionByClass WFAH_Action_InternalTitle">Action by</div>
                                                <div class="WFAH_ActionByInfoClass WFAH_Action_InternalInfo WFAH_Ellipses" data-bind="text: actionByName, attr: { title: actionByName }"></div>
                                            </div>

                                            <%--Remarks--%>
                                            <div class="WFAH_ActionCol3 WFAH_Action_InternalDivs">
                                                <div class="WFAH_RemarksClass WFAH_Action_InternalTitle">Remarks</div>
                                                <div class="WFAH_RemarksInfoClass WFAH_Action_InternalInfo WFAH_Ellipses" data-bind="text: actionRemarks, attr: { title: actionRemarks }"></div>
                                            </div>

                                            <%--Workflow Level--%>
                                            <div class="WFAH_ActionCol4 WFAH_Action_InternalDivs">
                                                <div class="WFAH_WorkflowStageClass WFAH_Action_InternalTitle">Stage</div>
                                                <div class="WFAH_WorkflowStageInfoClass WFAH_Action_InternalInfo WFAH_Ellipses" data-bind="text: actionWorkflowLevel, attr: { title: actionWorkflowLevel }"></div>
                                                <%--<div class="WFAH_WorkflowLevelClass">Workflow Stage</div>
                                                <div class="WFAH_WorkflowLevelInfoClass" data-bind="text: actionWorkflowLevel"></div>--%>
                                            </div>

                                            <%--Details--%>
                                            <div class="WFAH_ActionCol5 WFAH_Action_InternalDivs">
                                                <div class="WFAH_DetailsInfoClass WFAH_Action_InternalInfo WFAH_Ellipses" data-bind="visible: attrListing().length > 0">Details</div>
                                            </div>
                                        </div>

                                        <div class="WFAH_ActionMainDataPendingClass" data-bind="click: workflowActionHistory.onClickAction, visible: actionPendingLevel > 0, style: { borderTop: actionBorderTop, color: actionColor, cursor: actionClickable }">
                                            <div class="WFAH_ActionColPending1 WFAH_Action_InternalDivs WFAH_Ellipses" data-bind="text: actionPendingText"></div>
                                            <div class="WFAH_ActionColPending2 WFAH_Action_InternalDivs">
                                                <div class="WFAH_DetailsInfoClass WFAH_Action_InternalInfo WFAH_Ellipses" data-bind="visible: attrListing().length > 0">Details</div>
                                            </div>
                                        </div>

                                        <div class="WFAH_ActionRowBottomClass" data-bind="visible: toShow()">
                                            <div class="WFAH_AttrHeaderRow WFAH_AttrRow">
                                                <div class="WFAH_AttrCol1 WFAH_Action_AttrDivs">Attribute Name</div>
                                                <div class="WFAH_AttrCol2 WFAH_Action_AttrDivs">Leg Name</div>
                                                <div class="WFAH_AttrCol3 WFAH_Action_AttrDivs" data-bind="visible: $root.isSec">Leg ID</div>
                                                <div class="WFAH_AttrCol3 WFAH_Action_AttrDivs" data-bind="visible: !$root.isSec">Primary Attribute</div>
                                                <div class="WFAH_AttrCol4 WFAH_Action_AttrDivs">New Value</div>
                                                <div class="WFAH_AttrCol5 WFAH_Action_AttrDivs">Old Value</div>
                                                <div class="WFAH_AttrCol6 WFAH_Action_AttrDivs">Knowledge Date</div>
                                                <div class="WFAH_AttrCol7 WFAH_Action_AttrDivs">Modified By</div>
                                            </div>
                                            <div class="WFAH_AuditData scroll" data-bind="foreach: attrListing">
                                                <div class="WFAH_AttrRow">
                                                    <div class="WFAH_AttrCol1 WFAH_Attr_TD WFAH_Action_AttrDivs WFAH_Ellipses" data-bind="text: attributeName, attr: { title: attributeName }"></div>
                                                    <div class="WFAH_AttrCol2 WFAH_Attr_TD WFAH_Action_AttrDivs WFAH_Ellipses" data-bind="text: typeName, attr: { title: typeName }"></div>
                                                    <div class="WFAH_AttrCol3 WFAH_Attr_TD WFAH_Action_AttrDivs WFAH_Ellipses" data-bind="text: legID, visible: $root.isSec, attr: { title: legID }"></div>
                                                    <div class="WFAH_AttrCol3 WFAH_Attr_TD WFAH_Action_AttrDivs WFAH_Ellipses" data-bind="text: primaryAttribute, visible: !$root.isSec, attr: { title: primaryAttribute }"></div>
                                                    <div class="WFAH_AttrCol4 WFAH_Attr_TD WFAH_Action_AttrDivs WFAH_Ellipses" data-bind="text: newValue, attr: { title: newValue }"></div>
                                                    <div class="WFAH_AttrCol5 WFAH_Attr_TD WFAH_Action_AttrDivs WFAH_Ellipses" data-bind="text: oldValue, attr: { title: oldValue }"></div>
                                                    <div class="WFAH_AttrCol6 WFAH_Attr_TD WFAH_Action_AttrDivs WFAH_Ellipses" data-bind="text: knowledgeDate, attr: { title: knowledgeDate }"></div>
                                                    <div class="WFAH_AttrCol7 WFAH_Attr_TD WFAH_Action_AttrDivs WFAH_Ellipses" data-bind="text: userName, attr: { title: userName }"></div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>

                                    <%--Action Date & Time--%>
                                    <div class="WFAH_ActionDateTimeClass WFAH_Action_Divs">
                                        <div class="WFAH_ActionDateClass" data-bind="text: actionDateDisplayText, visible: actionPendingLevel <= 0">
                                        </div>
                                        <div class="WFAH_ActionTimeClass" data-bind="text: actionTimeDisplayText, visible: actionPendingLevel <= 0">
                                        </div>
                                    </div>

                                </div>

                                <%--<div class="WFAH_ActionRowBottomClass scroll" data-bind="visible: toShow()">
                                    <div class="WFAH_AttrHeaderRow WFAH_AttrRow">
                                        <div class="WFAH_AttrCol1 WFAH_Action_AttrDivs">Attribute Name</div>
                                        <div class="WFAH_AttrCol2 WFAH_Action_AttrDivs">Leg Name</div>
                                        <div class="WFAH_AttrCol3 WFAH_Action_AttrDivs" data-bind="visible: $root.isSec">Leg ID</div>
                                        <div class="WFAH_AttrCol3 WFAH_Action_AttrDivs" data-bind="visible: !$root.isSec">Primary Attribute</div>
                                        <div class="WFAH_AttrCol4 WFAH_Action_AttrDivs">New Value</div>
                                        <div class="WFAH_AttrCol5 WFAH_Action_AttrDivs">Old Value</div>                                    
                                        <div class="WFAH_AttrCol6 WFAH_Action_AttrDivs">Knowledge Date</div>
                                        <div class="WFAH_AttrCol7 WFAH_Action_AttrDivs">User Name</div>
                                    </div>
                                    <div class="WFAH_AuditData" data-bind="foreach: attrListing">
                                        <div class="WFAH_AttrRow">
                                            <div class="WFAH_AttrCol1 WFAH_Attr_TD WFAH_Action_AttrDivs WFAH_Ellipses" data-bind="text: attributeName, attr: { title: attributeName }"></div>
                                            <div class="WFAH_AttrCol2 WFAH_Attr_TD WFAH_Action_AttrDivs WFAH_Ellipses" data-bind="text: typeName, attr: { title: typeName }"></div>
                                            <div class="WFAH_AttrCol3 WFAH_Attr_TD WFAH_Action_AttrDivs WFAH_Ellipses" data-bind="text: legID, visible: $root.isSec, attr: { title: legID }"></div>
                                            <div class="WFAH_AttrCol3 WFAH_Attr_TD WFAH_Action_AttrDivs WFAH_Ellipses" data-bind="html: primaryAttribute, visible: !$root.isSec, attr: { title: primaryAttribute }"></div>
                                            <div class="WFAH_AttrCol4 WFAH_Attr_TD WFAH_Action_AttrDivs WFAH_Ellipses" data-bind="text: newValue, attr: { title: newValue }"></div>
                                            <div class="WFAH_AttrCol5 WFAH_Attr_TD WFAH_Action_AttrDivs WFAH_Ellipses" data-bind="text: oldValue, attr: { title: oldValue }"></div>        
                                            <div class="WFAH_AttrCol6 WFAH_Attr_TD WFAH_Action_AttrDivs WFAH_Ellipses" data-bind="text: knowledgeDate, attr: { title: knowledgeDate }"></div>        
                                            <div class="WFAH_AttrCol7 WFAH_Attr_TD WFAH_Action_AttrDivs WFAH_Ellipses" data-bind="text: userName, attr: { title: userName }"></div>        
                                        </div>
                                    </div>
                                </div>--%>
                            </div>
                        </div>
                    </div>

                </div>
                <%--No Instances Error Div--%>
                <div id="WFAH_InstancesErrorDiv" class="hideInitially">
                    <%--<div class="WFAH_InstancesErrorMsgDiv">
                        No Workflow has ever been raised on this Security.
                    </div>--%>
                </div>

            </div>

        </div>
        <%--</div>--%>

        <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/jquery-ui.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/knockout-3.4.0.js" type="text/javascript"></script>
        <script src="includes/MicrosoftAjax.js" type="text/javascript"></script>
        <script src="includes/RADCommonScripts.js" type="text/javascript"></script>
        <script src="includes/SecMasterScripts.js" type="text/javascript"></script>
        <script src="includes/RScriptUtils.debug.js" type="text/javascript"></script>
        <script src="includes/script.js" type="text/javascript"></script>
        <script src="includes/WorkflowActionHistory.js" type="text/javascript"></script>
    </form>
</body>
</html>
