<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AttributeLegSetupPage.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.BaseUserControls.Modeler.AttributeLegSetupPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../../App_Themes/Aqua/thirdparty/font-awesome.css" rel="stylesheet" />
    <script src="../../js/thirdparty/jquery-1.11.3.min.js"></script>
    <link href="../../css/thirdparty/AttributeLegSetupPage.css" rel="stylesheet" />
    <script src="../../includes/AttributeLegSetupPage.js"></script>
    <script src="../../js/thirdparty/knockout-3.4.0.js"></script>
    <script src="../../includes/KOMappingLibrary.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="LegAttributePageParent">
            <div id="LegAttributePage" style="visibility:hidden">
                <%--<div class="LegRow">
                <div class="LegColumn">
                    Leg Name :
                </div>
                <div class="LegColumn">
                    <input type="text" id="LegName" value="" />
                </div>
            </div>
            <div class="LegRow">
                <div class="LegColumn">
                    <input type="radio" id="singleRel" name="Relationship" value="SingleRelationship" checked="" />Single Relationship
                </div>
                <div class="LegColumn">
                    <input type="radio" id="multiRel" name="Relationship" value="MultiRelationship" checked="" />Multiple Relationship
                </div>
                <div class="LegColumn">
                    <input type="checkbox" id="constSec" name="ConstituentSec" checked="" />Has Constituent Securities
                </div>
                <div class="LegColumn" style="display:none;" id="onconstseccheck">
                    <select>
                        <option value="test">test</option>
                        <option value="demo">demo</option>
                    </select>
                </div>
            </div>
            <div class="LegRow">
                <div class="LegColumn">
                    Primary Attribute :
                    </div>
                <div class="LegColumn">
                    <select>
                        <option value="attr1">attr1</option>
                        <option value="notes">notes</option>
                        <option value="attr ref 1">attr ref 1</option>
                    </select>
                </div>
            </div>
            <div id="Tr1" class="LegRow" runat="server">
                <div class="LegColumn" style="width: 153px; display: inline-block;">
                    Tags:
                </div>
                <div class="LegColumn" style="width: 364px; display: inline-block;">
                    <div class="tagContainer" id="divTag" runat="server" style="width: 200px; margin-left: 30px; overflow: auto;">
                    </div>
                </div>
            </div>--%>
                   <div class="smLegAttributePage_headerSection LegRow" id="SecMasterHeader" style="margin-top: 2%;">
                       <div class="smAttribute_horizontalAlign LegColumn  AttributeInfoHideShow LegFirst" style="visibility:hidden">
                            <span class="fa-stack fa-lg">
                                <i class="fa fa-plus" style="color: #969292;"></i>
                            </span>
                        </div>
                <div class="LegColumn LegColumnFontHeader SetWidth"></div>
             
                <%--<div class="smAttribute_headerColumn LegColumn SetWidth">Leg Name</div>--%>

                <div class="LegColumn LegColumnFontHeader SetWidth" data-bind="visible: moduleId() == 3">Has Underlier</div>
           
                <div class="LegColumn LegColumnFontHeader SetWidth" data-bind="visible: moduleId() == 3">Underlier Security Type Name</div>
             
                <div class="LegColumn LegColumnFontHeader SetWidth">Multi Info</div>
             
                <div class="LegColumn LegColumnFontHeader SetWidth">Primary Key</div>

            </div>
                <div id="RenderLegInfo" data-bind="foreach: viewmodelobj.data">

                    <div class="LegRow SRMAttributeIFrameMarker ALSBorder" iscollapsed="0">

                        <div class="smAttribute_horizontalAlign LegColumn <%--SetWidth--%> AttributeInfoHideShow" data-bind="click: $parent.onClickShowAttrInfo">
                            <span class="fa-stack fa-lg">
                                <i class="fa fa-plus" style="color: #969292;"></i>
                            </span>
                        </div>

                        <div class="LegColumn" <%--data-bind="attr: { id: identifier }"--%>>
                            <div class="smAttribute_horizontalAlign LegColumn LegNameStyling SetWidth" data-bind="text: LegName"></div>

                        </div>
                        <div class="LegColumn SetWidth"
                            data-bind="visible: viewmodelobj.moduleId() == 3"
                             <%--data-bind="attr: { id: identifier }"--%>>
                            <%--<div class="LegPageColumnHeadingStyle LegSetupheadingstyle" data-bind="visible: viewmodelobj.moduleId() == 3">Has Underlier</div>--%>
                            <%--<div class="ToggleWidthSet">
                                <div class="ToggleParentUnderlier ToggleParent">
                                    <div class="ToggleYes UnderlierYes SelectedClass LegDataStyling"  data-bind="click: $parent.ToggleUnderlier, style: { 'background-color': HasUnderlier() ? '#4da5dd' : 'none', 'color': HasUnderlier() ? 'white' : 'black' }">Yes</div>
                                    <div class="ToggleNo UnderlierNo SelectedClass LegDataStyling" data-bind="click: $parent.ToggleUnderlier, style: { 'background-color': HasUnderlier() ? 'none' : '#4da5dd', 'color': HasUnderlier() ? 'black' : 'white' }">No</div>
                                </div>
                            </div>--%>
                            <div  class="LegDataStyling" data-bind="text: HasUnderlier() ? 'Yes' : 'No', style: { 'color': HasUnderlier() ? 'black' : '#bfbfbf' }"></div>
                            <%--<input type="checkbox" class="smAttribute_horizontalAlign smAttribute_isPrimaryStyle smAttribute_checkboxColumn AttributeWidthAddRow SetWidth" data-bind="attr: { checked: HasUnderlier }, visible: viewmodelobj.moduleId() == 3" />--%>
                        </div>
                        <div class="LegColumn SetWidth" <%--data-bind="attr: { id: identifier }"--%>
                            
                            data-bind="visible: viewmodelobj.moduleId() == 3">
                            <%--<div class="LegPageColumnHeadingStyle LegSetupheadingstyle" data-bind="visible: viewmodelobj.moduleId() == 3">Underlier Security Type Name</div>--%>
                            <div class="LegColumn LegDataStyling" data-bind="text: UnderlierSecTypeName() == '' ? 'None' : UnderlierSecTypeName, style: { 'font-style': UnderlierSecTypeName() == '' ? 'italic' : 'normal' }"></div>

                        </div>
                        <div class="LegColumn  SetWidth" <%--data-bind="attr: { id: identifier }"--%>>
                            <%--<div class="LegPageColumnHeadingStyle LegSetupheadingstyle">Multiple Info.</div>--%>
                           <%-- <div class="ToggleParentMultiInfo ToggleParent">
                                <div class="ToggleYes MultiInfoYes SelectedClass LegDataStyling"  data-bind="click: $parent.ToggleUnderlier, style: { 'background-color': MultipleInfo() ? '#4da5dd' : 'none', 'color': MultipleInfo() ? 'white' : 'black' }">Yes</div>
                                <div class="ToggleNo MultiInfoNo SelectedClass LegDataStyling" data-bind="click: $parent.ToggleUnderlier, style: { 'background-color': MultipleInfo() ? 'none' : '#4da5dd', 'color': MultipleInfo() ? 'black' : 'white' }">No</div>

                            </div>--%>
                            <div class="LegDataStyling" data-bind="text: MultipleInfo() ? 'Yes' : 'No', style: { 'color': MultipleInfo() ? 'black' : '#bfbfbf' }"></div>
                            <%--<input type="checkbox" class="smAttribute_horizontalAlign smAttribute_isPrimaryStyle smAttribute_checkboxColumn AttributeWidthAddRow SetWidth" data-bind="attr: { checked: MultipleInfo }" />--%>
                        </div>
                        <div class="LegColumn  SetWidth" <%--data-bind="attr: { id: identifier }"--%>>
                            <%--<div class="LegPageColumnHeadingStyle LegSetupheadingstyle">Primary Key</div>--%>
                            <div class="LegDataStyling" data-bind="text: PrimaryKey() == '' ? 'None' : PrimaryKey, style: {'font-style': PrimaryKey() == '' ? 'italic':'normal'}"></div>
                        </div>


                    </div>
                    <div class="SMAttributeLegSetupHideInitially SRMAttributeLegIFrameContainer" data-bind="attr: { id: 'SRMAttributeLegIFrameContainer' + LegId() }">
                        <%-- <div class="closeButton">
                            <i class="fa fa-close"></i>
                        </div>--%>
                        <iframe data-bind="attr: { id: 'SRMAttributeLegIFrame' + LegId() }"></iframe>
                    </div>
                </div>
            </div>
            <div class="SRMLegAttributePageNoResult" id="SRMLegAttributePageNoResult" style="display:none"></div>
        </div>

        
        <input type="hidden" runat="server" id="hdnmoduleId" />
        <input type="hidden" runat="server" id="hdnTypeId" />
        <input type="hidden" runat="server" id="hdnTemplateId" />


    </form>
</body>
</html>
