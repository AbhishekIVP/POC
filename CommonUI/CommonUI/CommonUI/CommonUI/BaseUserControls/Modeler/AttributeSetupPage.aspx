<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AttributeSetupPage.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.BaseUserControls.Modeler.AttributeSetupPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="../../../App_Themes/Aqua/thirdparty/font-awesome.css" rel="stylesheet" />
    <script src="../../js/thirdparty/jquery-1.11.3.min.js"></script>
    <link href="../../css/thirdparty/AttributeSetupPage.css" rel="stylesheet" />
    <link href="../../css/SMCss/SMSelect.css" rel="stylesheet" />
    <link href="../../css/SMCss/Tags.css" rel="stylesheet" />
    <script src="../../includes/SMSelect.js"></script>
    <script src="../../includes/Tags.js"></script>
    <script src="../../js/thirdparty/knockout-3.4.0.js"></script>
    
    <script src="../../includes/KOMappingLibrary.js"></script>
    <%--<script src="../../includes/updatePanel.js"></script>--%>
    <script src="../../includes/MicrosoftAjax.js"></script>
    <script src="../../includes/updatePanel.js"></script>
    <script src="../../includes/AttributeSetupPage.js"></script>
    <script src="../../includes/SMSlimscroll.js"></script>

</head>

<body>
    <form id="form1" runat="server">
        
        <div id="smAttributeContainer">
            
            <!--static part---headers-->
            <div class="SecMasterData">
                <div class="smAttribute_headerSection HideDivInitially<%-- smAttribute_attrInsertionRowStyle--%>" id="SecMasterHeader" data-bind="visible: moduleId() == 3">
                    <div class="smAttribute_horizontalAlign smAttribute_headerColumn AttributeWidthAddRow">Attribute Name</div>
                    <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                    <div class="smAttribute_horizontalAlign smAttribute_headerColumn AttributeWidthAddRow">Data Type</div>
                    <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                    <div class="smAttribute_horizontalAlign smAttribute_headerColumn AttributeWidthAddRow">Reference Entity</div>
                    <div class="smAttribute_horizontalAlign smAttribute_headerColumn AttributeWidthAddRow"<%-- style="width: 10%;"--%>>Display Attribute</div>
                    <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                    <div class="smAttribute_horizontalAlign smAttribute_headerColumn AttributeWidthAddRow" <%--style="width: 10%;"--%>>Reference Leg Attribute</div>
                    <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                    <div class="smAttribute_horizontalAlign smAttribute_headerColumn AttributeWidthAddRow" <%--style="width: 10%;"--%>>Display Attributes</div>
                    <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                    <div class="smAttribute_horizontalAlign smAttribute_headerColumn AttributeWidthAddRowFixed">Attribute Length</div>
                    <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                    <div class="smAttribute_horizontalAlign smAttribute_headerColumn smAttribute_checkboxColumn AttributeWidthAddRowFixed">Mandatory</div>
                    <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                    <div class="smAttribute_horizontalAlign smAttribute_headerColumn smAttribute_checkboxColumn AttributeWidthAddRowFixed">Cloneable</div>
                    <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                    <div class="smAttribute_horizontalAlign smAttribute_headerColumn AttributeWidthAddRow">Tags</div>
                    <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                    <div class="smAttribute_horizontalAlign smAttribute_headerColumn AttributeWidthAddRow" style="text-align: left;">Attribute Description</div>
                </div>
                <div class="smAttribute_headerSection HideDivInitially" id="RPFManagerHeader" data-bind="visible: moduleId() != 3">
                <div class="smAttribute_horizontalAlign smAttribute_headerColumn AttributeWidthAddRow">Attribute Name</div>
                    <%--<div class="smAttribute_horizontalAlign smAttribute_headerColumn"style="width:120px;">Attribute Name</div>--%>
                <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                <div class="smAttribute_horizontalAlign smAttribute_headerColumn AttributeWidthAddRow">Data Type</div>
                <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                <div class="smAttribute_horizontalAlign smAttribute_headerColumn AttributeWidthAddRow" >Lookup Type</div>
                    <%--<div class="smAttribute_horizontalAlign smAttribute_headerColumn" style="width:115px;">Lookup Type</div>--%>
                <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                <div class="smAttribute_horizontalAlign smAttribute_headerColumn AttributeWidthAddRow" >Lookup Attribute</div>
                <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                <div class="smAttribute_horizontalAlign smAttribute_headerColumn AttributeWidthAddRow" >Default Value</div>
                <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
    <%--            <div class="smAttribute_horizontalAlign smAttribute_headerColumn" style="width:100px;">Search View Position</div>--%>
                    <div class="smAttribute_horizontalAlign smAttribute_headerColumn AttributeWidthAddRow" >Search View Position</div>
                <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                <div class="smAttribute_horizontalAlign smAttribute_headerColumn AttributeWidthAddRowFixed">Attribute Length</div>
                <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                <div class="smAttribute_horizontalAlign smAttribute_headerColumn <%--smAttribute_checkboxColumn --%>smAttribute_Unique AttributeWidthAddRowFixed">Mandatory</div>
                <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                <div class="smAttribute_horizontalAlign smAttribute_headerColumn <%--smAttribute_checkboxColumn--%> smAttribute_Unique AttributeWidthAddRowFixed">Cloneable</div>
                <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                <div class="smAttribute_horizontalAlign smAttribute_headerColumn <%--smAttribute_checkboxColumn--%> smAttribute_Unique AttributeWidthAddRowFixed">Encrypted</div>
                <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                <div class="smAttribute_horizontalAlign smAttribute_headerColumn <%--smAttribute_checkboxColumn--%> smAttribute_Unique AttributeWidthAddRow">Visible in Search</div>
                <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                <div class="smAttribute_horizontalAlign smAttribute_headerColumn AttributeWidthAddRow">Tags</div>
                <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                <div class="smAttribute_horizontalAlign smAttribute_headerColumn AttributeWidthAddRow">Restricted Characters</div>
                
            </div>

                <!--static part add new row option-->
                <%--<div id="smAttribute_attrInsertionRow" class="smAttribute_attrInsertionRowStyle">
                    <input type="text" id="smAttribute_attrName" class="smAttribute_horizontalAlign smAttribute_attrNameStyle smAttribute_attrInputStyle AttributeWidthAddRow" placeholder="Attribute Name" data-bind:"event:{Keypress:onKeypress}"/>
                    <div class="smAttribute_horizontalAlign smAttribute_splitter"></div>
                    <div id="smAttribute_dataType" class="smAttribute_horizontalAlign smAttribute_attrRowStyle AttributeWidthAddRow">
                        
                        <div id="datatype"></div>
                    </div>
                    <div class="smAttribute_horizontalAlign smAttribute_splitter"></div>
          
                    <div id="smAttribute_referenceType" class="smAttribute_horizontalAlign smAttribute_attrRowStyle AttributeWidthAddRow" data-bind="visible: moduleId() == 3">
                        
                        <div id="reftype" style="width: 90%"></div>
                    </div>
                        <div  class="smAttribute_horizontalAlign smAttribute_attrRowStyle AttributeWidthAddRow" data-bind="visible: moduleId() != 3">
                        
                        <div id="lookuptype" ></div>
                    </div>
                  
                    <div class="smAttribute_horizontalAlign smAttribute_splitter"></div>
                    <div id="smAttribute_referenceAttribute" class="smAttribute_horizontalAlign smAttribute_attrRowStyle AttributeWidthAddRow" data-bind="visible: moduleId() == 3" >
                       
                        <div id="refattr" style="width: 90%"></div>
                    </div>
                        <div class="smAttribute_horizontalAlign smAttribute_attrRowStyle AttributeWidthAddRow" data-bind="visible: moduleId() != 3">
                        
                        <div id="lookupattr" ></div>
                    </div>
                    <div class="smAttribute_horizontalAlign smAttribute_splitter"></div>
                    <div id="smAttribute_referenceLegAttribute" class="smAttribute_horizontalAlign smAttribute_attrRowStyle AttributeWidthAddRow" data-bind="visible: moduleId() == 3" >
                        
                        <div id="reflegattr" style="width: 90%"></div>
                    </div>
                    <input type="text" id="defaultvalue" class="smAttribute_horizontalAlign smAttribute_attrNameStyle smAttribute_attrInputStyle AttributeWidthAddRow" placeholder="Default Value" data-bind="event: { Keypress: onKeypress }, visible: moduleId() != 3"/>
                    <div class="smAttribute_horizontalAlign smAttribute_splitter"></div>
                    <div id="smAttribute_refdispAttr" class="smAttribute_horizontalAlign smAttribute_attrRowStyle AttributeWidthAddRow" data-bind="visible: moduleId() == 3" >
                        <div id="refdispAttr" style="width: 100%"></div>
                    </div>

                     <input type="text" id="searchviewpos" class="smAttribute_horizontalAlign smAttribute_attrNameStyle smAttribute_attrInputStyle AttributeWidthAddRow" placeholder="Search View Position" data-bind="event: { Keypress: onKeypress }, visible: moduleId() != 3"/>
                    <div class="smAttribute_horizontalAlign smAttribute_splitter"></div>
                  
                                  
                    <div id="smAttribute_attributeLength" class="smAttribute_horizontalAlign smAttribute_attributeLengthStyle AttributeWidthAddRow" 
                        <div class="smAttribute_attrLengthInner" style="display:none;">
                            <div class="smAttribute_stringAttrLengthSection" id="stringlen" style="display: none;">
                                <input type="text" id="smAttribute_stringAttrLength" value="200" class="smAttribute_attrInputStyle" />
                            </div>
                            <div class="smAttribute_numericAttrLengthSection"  id="numlength" style="display: none;">
                                <input type="text" id="smAttribute_numericAttrLengthIntegerPart" value="12" class="smAttribute_horizontalAlign smAttribute_attrInputStyle" style="width: 40%;" />
                                <input type="text" id="smAttribute_numericAttrLengthFractionPart" value="9" class="smAttribute_horizontalAlign smAttribute_attrInputStyle" style="width: 20%;" />
                            </div>
                        </div>
                    </div>
                    <div class="smAttribute_horizontalAlign smAttribute_splitter"></div>

                    <input type="checkbox" id="smAttribute_isMandatory" class="smAttribute_horizontalAlign smAttribute_Unique smAttribute_checkboxColumn AttributeWidthAddRow" checked="" />
                    <div class="smAttribute_horizontalAlign smAttribute_splitter"></div>

                    <input type="checkbox" id="smAttribute_isCloneable" class="smAttribute_horizontalAlign smAttribute_Unique smAttribute_checkboxColumn AttributeWidthAddRow" checked="" />
                    <div class="smAttribute_horizontalAlign smAttribute_splitter"></div>

                    <input type="checkbox" id="smAttribute_isEncrypted" class="smAttribute_horizontalAlign smAttribute_Unique smAttribute_checkboxColumn AttributeWidthAddRow" data-bind="visible: moduleId() != 3" checked="" />
                    <div class="smAttribute_horizontalAlign smAttribute_splitter" data-bind="visible: moduleId() != 3"></div>

                    <input type="checkbox" id="smAttribute_isVisibleInSearch" class="smAttribute_horizontalAlign smAttribute_Unique smAttribute_checkboxColumn AttributeWidthAddRow" data-bind="visible: moduleId() != 3" checked="" />
                    <div class="smattribute_horizontalalign smattribute_splitter" style="display:inline-block;"></div>
                   

                    <div id="smAttribute_Tags" class="smAttribute_horizontalAlign smAttribute_attrNameStyle smAttribute_attrInputStyle AttributeWidthAddRow" style="width:6.1%;display:inline-block;"></div>
                    <div class="smAttribute_horizontalAlign smAttribute_splitter"></div>

              
                    <input type="text" id="smAttribute_attrDescription" class="smAttribute_horizontalAlign smAttribute_attrDescriptionStyle smAttribute_attrInputStyle AttributeWidthAddRow" data-bind="visible: moduleId() == 3" placeholder="Attribute Description" />
                    <input type="text" id="restrictedchar" class="smAttribute_horizontalAlign smAttribute_attrNameStyle smAttribute_attrInputStyle AttributeWidthAddRow" placeholder="Restricted Characters" data-bind="event: { Keypress: onKeypress }, visible: moduleId() != 3"/>
                    <div class="smAttribute_horizontalAlign smAttribute_splitter"></div>

                    
                    <div id="smAttribute_addRowBtn" class="smAttribute_addRowBtnStyle smAttribute_horizontalAlign AttributeIconRow" data-bind="click: onClickAddAttrInfo">
                        <span class="fa-stack fa-sm">
                            <i class="fa fa-circle fa-stack-2x" style="color: #43D9C6;"></i>
                            <i class="fa fa-plus fa-stack-1x fa-inverse"></i>
                        </span>

                    </div>
                </div>--%>

                <%--rendering row from backend--%>
                <div id="smAttribute_addedattributescontainer" class="smAttribute_addedattributescontainerstyle">
                    <div data-bind="foreach: viewmodelobj.data">
                        <div class="smAttribute_attrInsertionRowStyle" data-bind="attr: { id: identifier }">
                            <div class="smAttribute_horizontalAlign <%--smAttribute_attrNameStyle--%> smAttribute_attrInputStyle AttributeWidthAddRow AttrRowColor ">
                                <div class="smAttribute_horizontalAlign <%--smAttribute_attrNameStyle--%> smAttribute_attrInputStyle AttrRowColor RowCenterAlign"  style="padding-left:0.2%;" data-bind="text: AttrName"></div>
                            <%--<input type="text" class="smAttribute_horizontalAlign smAttribute_attrNameStyle smAttribute_attrInputStyle AttributeWidthAddRow AttrRowColor" data-bind="value: AttrName" <%--style="width:120px;"--%>
                                </div>
                            <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                            <div class="smAttribute_horizontalAlign smAttribute_attrRowStyle AttributeWidthAddRow AttrRowColor RowCenterAlign" <%--style="width:6%;"--%> data-bind="attr: { id: DataTypeIdentifier }"></div>
                            <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>

                            <div class="smAttribute_horizontalAlign smAttribute_attrRowStyle AttributeWidthAddRow AttrRowColor RowCenterAlign" <%--style="width:%;--%>">
                                <div class="smAttribute_horizontalAlign" <%--style="width:8%;"--%> data-bind="attr: { id: ReferenceTypeIdentifier }, visible: $parent.moduleId() == 3"></div>
                                <div class="smAttribute_horizontalAlign AttributeWidthAddRow"<%-- style="width:115px;"--%> data-bind="attr: { id: LookupTypeIdentifier }, visible: $parent.moduleId() != 3"></div>
                            </div>

                            <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                            <div class="smAttribute_horizontalAlign smAttribute_attrRowStyle AttributeWidthAddRow AttrRowColor RowCenterAlign" data-bind=" attr: { id: ReferenceAttributeIdentifier }, visible: $parent.moduleId() == 3" <%--style="width: 10%"--%>></div>
                            <div class="smAttribute_horizontalAlign smAttribute_attrRowStyle AttributeWidthAddRow AttrRowColor RowCenterAlign" data-bind=" attr: { id: LookupAttributeIdentifier }, visible: $parent.moduleId() != 3" <%--style="width:100px;"--%>></div>
                            <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>

                            <div class="smAttribute_horizontalAlign smAttribute_attrRowStyle AttributeWidthAddRow AttrRowColor RowCenterAlign" data-bind=" attr: { id: ReferenceLegAttributeIdentifier }, visible: $parent.moduleId() == 3" <%--style="width: 10%"--%>></div>
                            <div class="smAttribute_horizontalAlign smAttribute_attrRowStyle AttributeWidthAddRow AttrRowColor RowCenterAlign" data-bind="value: DefaultValue, visible: $parent.moduleId() != 3" <%--style="width:6%;"--%>></div>
                            <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>

                            <div class="smAttribute_horizontalAlign smAttribute_attrRowStyle AttributeWidthAddRow AttrRowColor RowCenterAlign" data-bind="attr: { id: ReferenceDisplayIdentifier }, visible: $parent.moduleId() == 3"></div>
                            <div class="smAttribute_horizontalAlign smAttribute_attrRowStyle AttributeWidthAddRow AttrRowColor RowCenterAlign" data-bind="value: SearchViewPosition, visible: $parent.moduleId() != 3"></div> 
                            <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>

                            <div class="smAttribute_horizontalAlign smAttribute_attributeLengthStyle smAttribute_attrRowStyle AttributeWidthAddRowFixed AttrRowColor RowCenterAlign" <%--style="width:6%;"--%> data-bind="text: Length"></div>
                            <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>

                            <!--<%--<div class="smAttribute_stringAttrLengthSection">
                            <input type="text" class="smAttribute_attrInputStyle" data-bind="value: additionalLegAttrLength" readonly />
                        </div>--%>-->

                            <%-- <div class="smAttribute_attrLengthInner">
                            <div class="smAttribute_stringAttrLengthSection" data-bind="visible: (additionalLegAttrDataType() === 'STRING')">
                                <input type="text" class="smAttribute_attrInputStyle" data-bind="value: attrStringLength, attr: { id: attrStringAttrID }" style="width: 60%" />
                            </div>
                            <div class="smAttribute_numericAttrLengthSection" data-bind="visible: (additionalLegAttrDataType() === 'NUMERIC')">
                                <input type="text" class="smAttribute_horizontalAlign smAttribute_attrInputStyle" data-bind="value: attrNumericIntegerLength, attr: { id: attrNumericIntegerAttrID }" style="width: 40%;" />
                                <input type="text" class="smAttribute_horizontalAlign smAttribute_attrInputStyle" data-bind="value: attrNumericFractionLength, attr: { id: attrNumericFractionAttrID }" style="width: 10%;" />
                            </div>
                        </div>--%>
                            <div class="smAttribute_horizontalAlign smAttribute_checkboxColumn  AttributeWidthAddRowFixed">
                            <input type="checkbox" class="smAttribute_horizontalAlign smAttribute_checkboxColumn CheckBoxRow AttrRowColor RowCenterAlign" style="width:18%;" data-bind="attr: { /*id: isUniqueCheckBoxID,*/ checked: Mandatory }" />
                            </div>
                            
                            <div class="smAttribute_horizontalAlign smAttribute_checkboxColumn  AttributeWidthAddRowFixed">
                            <input type="checkbox" class="smAttribute_horizontalAlign smAttribute_checkboxColumn  AttrRowColor RowCenterAlign" style="width:18%;" data-bind="attr: { /*id: isCloneableCheckBoxID, */checked: IsCloneable }" />
                            <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                               </div>
                               <div class="smAttribute_horizontalAlign smAttribute_checkboxColumn  AttributeWidthAddRowFixed" data-bind="visible: $parent.moduleId() != 3">
                             <input type="checkbox" class="smAttribute_horizontalAlign smAttribute_checkboxColumn  AttrRowColor RowCenterAlign" style="width:18%;" data-bind="attr: { /*id: isUniqueCheckBoxID,*/ checked: Encrypted }, visible: $parent.moduleId() != 3" />
                            <%--<div class="smAttribute_horizontalAlign smAttribute_splitter" data-bind="visible: $parent.moduleId() != 3"></div>--%>
                                </div>
                            <div class="smAttribute_horizontalAlign smAttribute_checkboxColumn  AttributeWidthAddRowFixed" data-bind="visible: $parent.moduleId() != 3">
                            <input type="checkbox" class="smAttribute_horizontalAlign smAttribute_checkboxColumn AttrRowColor RowCenterAlign" style="width:18%;" data-bind="attr: { /*id: isCloneableCheckBoxID, */checked: VisibleInSearch }, visible: $parent.moduleId() != 3" />
                            <%--<div class="smAttribute_horizontalAlign smAttribute_splitter" data-bind="visible: $parent.moduleId() != 3"></div>--%>
                            </div>
                            <div class="smAttribute_horizontalAlign smAttribute_attrRowStyle AttributeWidthAddRow AttrRowColor RowCenterAlign" data-bind="text: Tags"<%-- style="width:6%"--%>>
                            </div>

                            <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                            <div class="smAttribute_horizontalAlign smAttribute_attrDescriptionStyle AttributeWidthAddRow smAttribute_attrInputStyle AttrRowColor RowCenterAlign" data-bind="visible: $parent.moduleId() == 3">
                                <div class="smAttribute_horizontalAlign smAttribute_attrDescriptionStyle AttributeWidthAddRow smAttribute_attrInputStyle AttrRowColor RowCenterAlign" data-bind="text: AttrDescription"></div>
                               
                             </div>
                            <div class="smAttribute_horizontalAlign smAttribute_attrRowStyle AttributeWidthAddRow AttrRowColor RowCenterAlign" data-bind="value: RestrictedChar, visible: $parent.moduleId() != 3"></div>
                            <%--<div class="smAttribute_horizontalAlign smAttribute_splitter"></div>--%>
                         <%--   <div class="smAttribute_addRowBtnStyle smAttribute_horizontalAlign AttributeIconRow AttributeWidthAddRow AttrRowColor" data-bind="click: $parent.onClickDeleteAttrInfo">
                                <span class="fa-stack fa-lg">
                                    <%--<i class="fa fa-circle fa-stack-2x" style="color: #fff;"></i>--%>
                           <%--         <i class="fa fa-trash-o fa-stack-1x fa-inverse" style="color: #43D9C6;"></i>
                                </span>
                            </div>--%>
                           
                        </div>
                    </div>

                </div>

            </div>
            <%--<input type="button" id="LegAttributePage" value="Next" onclick="RedirectToLegAttribute()" />--%>
            <%--<input type="button" id="UpdateButton" value="Update" onclick="UpdateInfo()" />--%>
            
            </div>
            <input type="hidden" runat="server" id="hdnmoduleId" />
            <input type="hidden" runat="server" id="hdnEntityTypeId" />
            <input type="hidden" runat="server" id="hdnTemplateId" />
            <input type="hidden" runat="server" id="hdnIsLeg" />
            <input type="hidden" runat="server" id="hdnAdditionalLeg" />
        <%--<div id="loadingImg" class="loadingMsg" style="display: none; z-index: 10000;">
            <asp:Image ID="Image2" runat="server" src="css/images/ajax-working.gif" />
        </div>
        <div id="disableDiv" class="alertBG" style="display: none; z-index: 9999;" align="center"></div>--%>
    </form>

</body>
</html>
