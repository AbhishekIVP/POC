<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UniquenessSetup.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.UniquenessSetup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="css/jquery-ui.min.css" rel="stylesheet" type="text/css" />
    <link href="css/UniquenessSetup.css" rel="stylesheet" type="text/css" />
    <link href="css/SMCss/SMSelect.css" rel="stylesheet" type="text/css" />
    <link href="css/thirdparty/font-awesome.css" rel="stylesheet" type="text/css" />
    <link href="css/SRMUniquenessFailure.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="commonUIUniquenessSetupForm" runat="server">

        <asp:ScriptManager ID="UniquenessSetupScriptManager" runat="server" ScriptMode="debug">
            <Scripts>
                <asp:ScriptReference Name="com.ivp.rad.raduicontrols.Scripts.RADCommonScripts.js"
                    Assembly="RADUIControls" />
                <asp:ScriptReference Name="com.ivp.rad.raduicontrols.Scripts.RADUIScripts.debug.js"
                    Assembly="RADUIControls" />
                <asp:ScriptReference Name="com.ivp.rad.raduicontrols.Scripts.RScriptUtils.debug.js"
                    Assembly="RADUIControls" />
            </Scripts>
        </asp:ScriptManager>

        <div id="UniquenessSetupBody">
            <!-- Beginning of Top Panel (Filters Container) -->
            <asp:Panel ID="panelTop" runat="server">
                <div id="UniquenessSetupTop">
                    <div id="UniquenessSetupFiltersDiv">
                        <div id="UniquenessSetupSectypeFilterMultiSelect" class="UniquenessSetupFilters">
                            <!-- In this div, we add the multi-select selector. -->
                        </div>
                        <div id="keyNameSearch" class="UniquenessSetupFilters">
                            <i class="fa fa-search notify-icon"></i>
                            <input id="gridSearch" placeholder="Enter a Key Name to search" class="placeholder" />
                        </div>
                    </div>
                    <%--<div id="keysDownloadAll" class="UniquenessSetupDownloadAllKeys" title="Click to Download All Unique Keys">
                        <i class="fa fa-file-excel-o"></i>
                    </div>--%>
                </div>
            </asp:Panel>
            <!-- End of Top Panel (Filters Container) -->

            <!-- Beginning of Middle Panel -->
            <asp:Panel ID="panelMiddle" runat="server">
                <div id="UniquenessSetupMainParentDiv" class="scroll">
                    <div id="UniquenessSetupMainNewKeyParentDiv">
                        <%--Add New Key Label on Top--%>
                        <div id="UniquenessSetupMainAddKeyTab">
                            <div id="UniquenessSetupMainAddKeyLabel">Add New Key</div>
                        </div>

                        <%-- New Key Parent Container --%>
                        <div id="UniquenessSetupMainNewKeyDiv" class="UniquenessSetupNewKeyParentDiv UScenterPadding hideInitially">

                            <%--Key Name and Level--%>
                            <div id="UniquenessSetup_NK_Block_1" class="UniquenessSetup_Block_NameLevel_CSS_Class UniquenessSetup_Block_1 UniquenessSetup_NK_Blocks">
                                <div id="UniquenessSetup_NK_KeyName">
                                    <input id="UniquenessSetup_NK_KeyNameValue" placeholder="Enter Key Name" />
                                </div>
                                <div id="UniquenessSetup_NK_Level_Slide_Div">
                                    <div id="UniquenessSetup_NK_Level_Attribute" class="UniquenessSetup_NK_Levels ToggleBlueSliderButtonCSSClass LeftToggleButton">Attribute</div>
                                    <div id="UniquenessSetup_NK_Level_Leg" class="UniquenessSetup_NK_Levels RightToggleButton">Leg</div>
                                </div>
                            </div>

                            <%--Security Types--%>
                            <div id="UniquenessSetup_NK_Block_2" class="UniquenessSetup_Block_SecTypes_CSS_Class UniquenessSetup_Block_2 UniquenessSetup_NK_Blocks">
                                <div id="UniquenessSetup_NK_Block_2_Header" class="UniquenessSetup_Block_Headers_AddKey">Security Types</div>
                                <div id="UniquenessSetup_NK_Block_2_Info" class="UniquenessSetup_Block_Values"></div>
                            </div>

                            <%--Attributes for Attribuute Level or Leg Name for Leg Level--%>
                            <div id="UniquenessSetup_NK_Block_3" class="UniquenessSetup_Block_AttributeORLeg_CSS_Class UniquenessSetup_Block_3 UniquenessSetup_NK_Blocks">
                                <div id="UniquenessSetup_NK_Block_3_A">
                                    <div id="UniquenessSetup_NK_Block_3_A_Header" class="UniquenessSetup_Block_Headers_AddKey">Attributes</div>
                                    <div id="UniquenessSetup_NK_Block_3_A_Info" class="UniquenessSetup_Block_Values"></div>
                                </div>
                                <div id="UniquenessSetup_NK_Block_3_B" class="hideInitially">
                                    <div id="UniquenessSetup_NK_Block_3_B_Header" class="UniquenessSetup_Block_Headers_AddKey">Leg</div>
                                    <div id="UniquenessSetup_NK_Block_3_B_Info" class="UniquenessSetup_Block_Values"></div>
                                </div>
                            </div>

                            <%--Leg Attributes--%>
                            <div id="UniquenessSetup_NK_Block_4" class="UniquenessSetup_Block_LegAttributes_CSS_Class UniquenessSetup_Block_4 UniquenessSetup_NK_Blocks">
                                <div id="UniquenessSetup_NK_Block_4_B" class="hideInitially">
                                    <div id="UniquenessSetup_NK_Block_4_Header" class="UniquenessSetup_Block_Headers_AddKey">Leg Attributes</div>
                                    <div id="UniquenessSetup_NK_Block_4_Info" class="UniquenessSetup_Block_Values"></div>
                                </div>
                            </div>

                            <%--Check Uniqueness--%>
                            <div id="UniquenessSetup_NK_Block_5" class="UniquenessSetup_Block_CheckUniqueness_CSS_Class UniquenessSetup_Block_5 UniquenessSetup_NK_Blocks">
                                <div id="UniquenessSetup_NK_Block_5_B" class="hideInitially">
                                    <div id="UniquenessSetup_NK_Block_5_Header" class="UniquenessSetup_Block_Headers_AddKey">Check Uniqueness</div>
                                    <div id="UniquenessSetup_NK_Block_5_Info" class="UniquenessSetup_Block_Values">
                                        <div id="UniquenessSetup_NK_CU_Within" class="UniquenessSetup_CU_Options ToggleBlueSliderButtonCSSClass LeftToggleButton">Within Security</div>
                                        <div id="UniquenessSetup_NK_CU_Across" class="UniquenessSetup_CU_Options RightToggleButton">Across Securities</div>
                                    </div>
                                </div>
                            </div>

                             <div id="UniquenessSetup_NK_Block_55" class="UniquenessSetup_Block_CheckUniqueness_CSS_Class UniquenessSetup_Block_55 UniquenessSetup_NK_Blocks">
                                    <div id="UniquenessSetup_NK_Block_55_A" class="">
                                            <div id="UniquenessSetup_NK_Block_55_Header" class="UniquenessSetup_Block_Headers_AddKey">Include Securities in</div>
                                            <div id="Sm_DraftCheckBoxParent"><input type="checkbox" id="Sm_DraftCheckBoxInput" /><span>Drafts</span> </div>
                                            <div id="Sm_WorkflowCheckBoxParent"><input type="checkbox" id="Sm_WorkflowCheckBoxInput" /><span>Workflow</span></div>
                                    </div>
                            </div>

                            <%--null as unique data--%>
                            <div id="UniquenessSetup_NK_Block_56" class="UniquenessSetup_Block_CheckUniqueness_CSS_Class UniquenessSetup_Block_56 UniquenessSetup_NK_Blocks">
                                <div id="UniquenessSetup_NK_Block_56_A" class="">
                                    <div id="UniquenessSetup_NK_Block_56_Header" class="UniquenessSetup_Block_Headers_AddKey">Consider Null As Unique Data</div>
                                    <div id="UniquenessSetup_NK_Block_56_Info" class="UniquenessSetup_Block_Values">
                                        <div id="UniquenessSetup_NK_NullAsUniqueYes" class="UniquenessSetup_CU_Options ToggleBlueSliderButtonCSSClass LeftToggleButton UniquenessNullAsUniqueRadioWidth">Yes</div>
                                        <div id="UniquenessSetup_NK_NullAsUniqueNo" class="UniquenessSetup_CU_Options RightToggleButton UniquenessNullAsUniqueRadioWidth">No</div>
                                    </div>
                                </div>
                            </div>


                            <%--Save or Cancel--%>
                            <div id="UniquenessSetup_NK_Block_6" class="UniquenessSetup_Block_Save_Cancel_CSS_Class UniquenessSetup_Block_6 UniquenessSetup_NK_Blocks">
                                <div id="UniquenessSetup_NK_Block_6_Buttons" class="UniquenessSetup_Block_Buttons_Div UniquenessSetup_Inner_Block_Save_Cancel_CSS_Class">
                                    <div id="UniquenessSetup_NK_Save_Button" class="UniquenessSetup_Block_Buttons_Align UniquenessSetup_Block_Buttons UniquenessSetup_CommonGreenBtnStyle">Save</div>
                                    <div id="UniquenessSetup_NK_Cancel_Button" class="UniquenessSetup_Block_Buttons UniquenessSetup_Block_Buttons_Align UniquenessSetup_CommonTransparentBtnStyle">Cancel</div>
                                </div>
                            </div>
                        </div>

                    </div>
                    <div id="UniquenessSetupMain" class="UScenterPadding scroll hideInitially">

                        <%-- Leg Level Keys Container --%>
                        <div id="UniquenessSetupMainLegLevelKeys" data-bind="visible: chainLegLevelListing().length > 0">
                            <%--<div id="UniquenessSetupMainLegLevelMainLabel" class="UniquenessSetupLegKeysText">LEG LEVEL KEYS</div>--%>
                            <div id="UniquenessSetupMainLabelDivLeg">
                                <div class="UniquenessSetup_Block_1 UniquenessSetup_Leg_Blocks UniquenessLegHeaderTextOverride UniquenessBlock1HeaderUnsetPadding"><span class="UniquenessSetupLegKeysText">LEG LEVEL KEYS</span></div>
                                <div class="UniquenessSetup_Block_2 UniquenessSetup_Leg_Edit_Blocks UniquenessLegHeaderTextOverride">Security Types</div>
                                <div class="UniquenessSetup_Block_3 UniquenessSetup_Leg_Edit_Blocks UniquenessLegHeaderTextOverride">Leg</div>
                                <div class="UniquenessSetup_Block_4 UniquenessSetup_Leg_Edit_Blocks UniquenessLegHeaderTextOverride">Attributes</div>
                                <div class="UniquenessSetup_Block_5 UniquenessSetup_Leg_Edit_Blocks UniquenessLegHeaderTextOverride">Check Uniqueness</div>
                                <div class="UniquenessSetup_Block_55 UniquenessSetup_Leg_Edit_Blocks UniquenessLegHeaderTextOverride">Include Securities in</div>
                                <div class="UniquenessSetup_Block_56 UniquenessSetup_Leg_Edit_Blocks UniquenessLegHeaderTextOverride">Consider Null as Unique Data</div>
                            </div>
                            <div id="UniquenessSetupLegLevelKeysSuperParentDiv" class="scroll">
                                <div id="UniquenessSetupLegLevelKeysParentDiv" data-bind="foreach: chainLegLevelListing">

                                    <%--Chain Item Container--%>
                                    <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_Item_' + chainKeyID }" class="UniquenessSetup_LegLevel_Items UniquenessSetup_Chain_Items">

                                        <%--VIEW Mode--%>
                                        <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_ViewMode_' + chainKeyID }, visible: !chainIsEdit()" class="UniquenessSetup_LegLevel_ViewMode_Class UniquenessSetup_ViewMode_CSS_Class">

                                            <%--Key Name--%>
                                            <div class="UniquenessSetup_Block_1 UniquenessSetup_Leg_Blocks">
                                                <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_ViewMode_Item_' + chainKeyID + '_Block_1', title: chainKeyName() }, text: chainKeyName()" class="UniquenessSetup_LegLevel_ViewMode_Block_1_Class UniquenessSetupEllipses">
                                                </div>
                                            </div>

                                            <%--Security Types--%>
                                            <div class="UniquenessSetup_Block_2 UniquenessSetup_Leg_Blocks">
                                                <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_ViewMode_Item_' + chainKeyID + '_Block_2_Info', title: chainSelectedSectypesTooltipText }, text: chainSelectedSectypesDisplayText" class="UniquenessSetup_Block_Values UniquenessSetupEllipses"></div>
                                            </div>

                                            <%--Leg Name--%>
                                            <div class="UniquenessSetup_Block_3 UniquenessSetup_Leg_Blocks">
                                                <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_ViewMode_Item_' + chainKeyID + '_Block_3_Info', title: chainSelectedLegDisplayText }, text: chainSelectedLegDisplayText" class="UniquenessSetup_Block_Values UniquenessSetupEllipses"></div>
                                            </div>

                                            <%--Leg Attributes--%>
                                            <div class="UniquenessSetup_Block_4 UniquenessSetup_Leg_Blocks">
                                                <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_ViewMode_Item_' + chainKeyID + '_Block_4_Info', title: chainSelectedLegAttributesTooltipText }, text: chainSelectedLegAttributesDisplayText" class="UniquenessSetup_Block_Values UniquenessSetupEllipses"></div>
                                            </div>

                                            <%--Check Uniqueness--%>
                                            <div class="UniquenessSetup_Block_5 UniquenessSetup_Leg_Blocks">
                                                <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_ViewMode_Item_' + chainKeyID + '_Block_5_Info', title: chainIsAcrossSecuritiesDisplayText }, text: chainIsAcrossSecuritiesDisplayText" class="UniquenessSetup_Block_Values"></div>
                                            </div>

                                            <div class="UniquenessSetup_Block_55 UniquenessSetup_Leg_Blocks">
                                                <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_ViewMode_Item_' + chainKeyID + '_Block_55_Info', title: checkInText }, text: checkInText" class="UniquenessSetup_Block_Values"></div>
                                            </div>

                                            <div class="UniquenessSetup_Block_56 UniquenessSetup_Leg_Blocks">
                                                <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_ViewMode_Item_' + chainKeyID + '_Block_56_Info', title: nullAsUniqueText }, text: nullAsUniqueText" class="UniquenessSetup_Block_Values"></div>
                                            </div>

                                            <%--Edit or Delete--%>
                                            <div class="UniquenessSetup_Block_6 UniquenessSetup_Leg_Blocks">
                                                <div class="UniquenessSetup_Block_Buttons_Div">
                                                    <div data-bind="click: uniquenessSetup.onClickEditButton, attr: { id: 'UniquenessSetup_LegLevel_ViewMode_Item_' + chainKeyID + '_Block_6_Info_EditButton' }" class="UniquenessSetup_Block_Icons" title="Edit">
                                                        <i class="fa fa-edit"></i>
                                                    </div>
                                                    <div data-bind="click: uniquenessSetup.onClickDeleteButton, attr: { id: 'UniquenessSetup_LegLevel_ViewMode_Item_' + chainKeyID + '_Block_6_Info_DeleteButton' }" class="UniquenessSetup_Block_Icons" title="Delete">
                                                        <i class="fa fa-trash-o"></i>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>


                                        <%--EDIT Mode--%>
                                        <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_EditMode_' + chainKeyID }, visible: chainIsEdit()" class="UniquenessSetup_LegLevel_EditMode_Class UniquenessSetup_EditMode_CSS_Class">

                                            <%--Key Name--%>
                                            <div class="UniquenessSetup_Block_1 UniquenessSetup_Leg_Edit_Blocks">
                                                <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_EditMode_Item_' + chainKeyID + '_Block_1' }" class="UniquenessSetup_LegLevel_EditMode_Block_1_Class">
                                                    <input data-bind="attr: { id: 'UniquenessSetup_LegLevel_EditMode_Item_' + chainKeyID + '_Block_1_Info' }, textInput: chainKeyName()" />
                                                </div>
                                            </div>

                                            <%--Security Types--%>
                                            <div class="UniquenessSetup_Block_2 UniquenessSetup_Leg_Edit_Blocks">
                                                <div class="UniquenessSetup_Block_Headers">Security Types</div>
                                                <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_EditMode_Item_' + chainKeyID + '_Block_2_Info', title: chainSelectedSectypesTooltipText }, text: chainSelectedSectypesDisplayText" class="UniquenessSetup_Block_Values"></div>
                                            </div>

                                            <%--Leg Name--%>
                                            <div class="UniquenessSetup_Block_3 UniquenessSetup_Leg_Edit_Blocks">
                                                <div class="UniquenessSetup_Block_Headers">Leg</div>
                                                <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_EditMode_Item_' + chainKeyID + '_Block_3_Info' }, text: chainSelectedLegDisplayText" class="UniquenessSetup_Block_Values"></div>
                                            </div>

                                            <%--Leg Attributes--%>
                                            <div class="UniquenessSetup_Block_4 UniquenessSetup_Leg_Edit_Blocks">
                                                <div class="UniquenessSetup_Block_Headers">Attributes</div>
                                                <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_EditMode_Item_' + chainKeyID + '_Block_4_Info' }" class="UniquenessSetup_Block_Values"></div>
                                            </div>

                                            <%--Check Uniqueness--%>
                                            <div class="UniquenessSetup_Block_5 UniquenessSetup_Leg_Edit_Blocks">

                                                <div class="UniquenessSetup_Block_Headers">Check Uniqueness</div>
                                                <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_EditMode_Item_' + chainKeyID + '_Block_5_Info' }, click: uniquenessSetup.onClickToggleCheckUniqueness" class="UniquenessSetup_Block_Values">
                                                    <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_EditMode_Item_' + chainKeyID + '_Block_5_Info_CU_Within' }" class="UniquenessSetup_CU_Options ToggleBlueSliderButtonCSSClass LeftToggleButton ">Within Security</div>
                                                    <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_EditMode_Item_' + chainKeyID + '_Block_5_Info_CU_Across' }" class="UniquenessSetup_CU_Options RightToggleButton ">Across Securities</div>
                                                </div>
                                            </div>

                                             <div class="UniquenessSetup_Block_55 UniquenessSetup_Leg_Edit_Blocks">
                                    <div class="">
                                            <div class="UniquenessSetup_Block_Headers">Include Securities In</div>
                                            <div data-bind="attr: { id: 'Sm_DraftCheckBoxParent' + chainKeyID }"><input type="checkbox"  data-bind="attr: { id: 'Sm_DraftCheckBoxInput' + chainKeyID }, checked:checkInDrafts" /><span>Drafts</span> </div>
                                            <div data-bind="attr: { id: 'Sm_WorkflowCheckBoxParent' + chainKeyID }"><input type="checkbox" data-bind="attr: { id: 'Sm_WorkflowCheckBoxInput' + chainKeyID}, checked:checkInWorkflows " /><span>Workflows</span></div>
                                    </div>
                            </div>

                                            <%--null as unique data--%>
                                            <div class="UniquenessSetup_Block_56 UniquenessSetup_Leg_Edit_Blocks">

                                                <div class="UniquenessSetup_Block_Headers">Consider Null As Unique Data</div>
                                                <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_EditMode_Item_' + chainKeyID + '_Block_56_Info' }, click: uniquenessSetup.onClickToggleNullAsUnique" class="UniquenessSetup_Block_Values">
                                                    <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_EditMode_Item_' + chainKeyID + '_Block_56_NullAsUniqueYes' }" class="UniquenessSetup_CU_Options LeftToggleButton UniquenessNullAsUniqueRadioWidth">Yes</div>
                                                    <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_EditMode_Item_' + chainKeyID + '_Block_56_NullAsUniqueNo' }" class="UniquenessSetup_CU_Options RightToggleButton UniquenessNullAsUniqueRadioWidth">No</div>
                                                </div>
                                            </div>


                                            <%--Save or Cancel--%>
                                            <div class="UniquenessSetup_Block_6 UniquenessSetup_Leg_Edit_Blocks">
                                                <div class="UniquenessSetup_Block_Buttons_Div UniquenessSetup_Inner_Block_Save_Cancel_CSS_Class">
                                                    <div data-bind="click: uniquenessSetup.onClickUpdateButton, attr: { id: 'UniquenessSetup_LegLevel_EditMode_Item_' + chainKeyID + '_Block_6_Info_SaveButton' }" class="UniquenessSetup_Block_Buttons_Align UniquenessSetup_Block_Buttons UniquenessSetup_CommonGreenBtnStyle">Save</div>
                                                    <div data-bind="click: uniquenessSetup.onClickCancelButton, attr: { id: 'UniquenessSetup_LegLevel_EditMode_Item_' + chainKeyID + '_Block_6_Info_CancelButton' }" class="UniquenessSetup_Block_Buttons_Align UniquenessSetup_Block_Buttons UniquenessSetup_CommonTransparentBtnStyle">Cancel</div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <%-- Attribute Level Keys Container --%>
                        <div id="UniquenessSetupMainAttributeLevelKeys" data-bind="visible: chainAttributeLevelListing().length > 0">
<%--                            <div id="UniquenessSetupMainAttributeLevelMainLabel" class="UniquenessSetupAttributeKeysText">ATTRIBUTE LEVEL KEYS</div>--%>
                            <div id="UniquenessSetupMainLabelDivAttribute">
                                <div class="UniquenessSetup_Block_1 UniquenessSetup_Attribute_Edit_Blocks UniquenessLegHeaderTextOverride UniquenessBlock1HeaderUnsetPadding"><span class="UniquenessSetupAttributeKeysText">ATTRIBUTE LEVEL KEYS</span></div>
                                <div class="UniquenessSetup_Block_2 UniquenessSetup_Attribute_Edit_Blocks UniquenessLegHeaderTextOverride">Security Types</div>
                                <div class="UniquenessSetup_Block_3 UniquenessSetup_Attribute_Edit_Blocks UniquenessLegHeaderTextOverride">Attributes</div>
                                <div class="UniquenessSetup_Block_4 UniquenessSetup_Attribute_Edit_Blocks UniquenessLegHeaderTextOverride"> </div>
                                <div class="UniquenessSetup_Block_5 UniquenessSetup_Attribute_Edit_Blocks UniquenessLegHeaderTextOverride"> </div>
                                <div class="UniquenessSetup_Block_55 UniquenessSetup_Attribute_Edit_Blocks UniquenessLegHeaderTextOverride">Include Securities in</div>
                                <div class="UniquenessSetup_Block_56 UniquenessSetup_Attribute_Edit_Blocks UniquenessLegHeaderTextOverride">Consider Null as Unique Data</div>
                            </div>
                            <div id="UniquenessSetupAttributeLevelKeysSuperParentDiv" class="scroll">
                                <div id="UniquenessSetupAttributeLevelKeysParentDiv" data-bind="foreach: chainAttributeLevelListing">

                                    <%--Chain Item Container--%>
                                    <div data-bind="attr: { id: 'UniquenessSetup_AttributeLevel_Item_' + chainKeyID }" class="UniquenessSetup_AttributeLevel_Items UniquenessSetup_Chain_Items">

                                        <%--VIEW Mode--%>
                                        <div data-bind="attr: { id: 'UniquenessSetup_AttributeLevel_ViewMode_' + chainKeyID }, visible: !chainIsEdit()" class="UniquenessSetup_AttributeLevel_ViewMode_Class UniquenessSetup_ViewMode_CSS_Class">

                                            <%--Key Name--%>
                                            <div class="UniquenessSetup_Block_1 UniquenessSetup_Attribute_Blocks">
                                                <div data-bind="attr: { id: 'UniquenessSetup_AttributeLevel_ViewMode_Item_' + chainKeyID + '_Block_1', title: chainKeyName() }, text: chainKeyName()" class="UniquenessSetup_AttributeLevel_ViewMode_Block_1_Class UniquenessSetupEllipses">
                                                </div>
                                            </div>

                                            <%--Security Types--%>
                                            <div class="UniquenessSetup_Block_2 UniquenessSetup_Attribute_Blocks">
                                                <div data-bind="attr: { id: 'UniquenessSetup_AttributeLevel_ViewMode_Item_' + chainKeyID + '_Block_2_Info', title: chainSelectedSectypesTooltipText }, text: chainSelectedSectypesDisplayText" class="UniquenessSetup_Block_Values UniquenessSetupEllipses"></div>
                                            </div>

                                            <%--Attribute Name--%>
                                            <div class="UniquenessSetup_Block_3 UniquenessSetup_Attribute_Blocks">
                                                <div data-bind="attr: { id: 'UniquenessSetup_AttributeLevel_ViewMode_Item_' + chainKeyID + '_Block_3_Info', title: chainSelectedAttributesTooltipText }, text: chainSelectedAttributesDisplayText" class="UniquenessSetup_Block_Values UniquenessSetupEllipses"></div>
                                            </div>

                                            <%--EMPTY--%>
                                            <div class="UniquenessSetup_Block_4 UniquenessSetup_Attribute_Blocks"></div>

                                            <%--EMPTY--%>
                                            <div class="UniquenessSetup_Block_5 UniquenessSetup_Attribute_Blocks"></div>

                                             <div class="UniquenessSetup_Block_55 UniquenessSetup_Attribute_Blocks">
                                                <div data-bind="attr: { id: 'UniquenessSetup_AttributeLevel_ViewMode_Item_' + chainKeyID + '_Block_55_Info', title: checkInText }, text: checkInText" class="UniquenessSetup_Block_Values"></div>
                                            </div>

                                            <div class="UniquenessSetup_Block_56 UniquenessSetup_Attribute_Blocks">
                                                <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_ViewMode_Item_' + chainKeyID + '_Block_56_Info', title: nullAsUniqueText }, text: nullAsUniqueText" class="UniquenessSetup_Block_Values"></div>
                                            </div>


                                            <%--Edit or Delete--%>
                                            <div class="UniquenessSetup_Block_6 UniquenessSetup_Attribute_Blocks">
                                                <div class="UniquenessSetup_Block_Buttons_Div">
                                                    <div data-bind="click: uniquenessSetup.onClickEditButton, attr: { id: 'UniquenessSetup_AttributeLevel_ViewMode_Item_' + chainKeyID + '_Block_6_Info_EditButton' }" class="UniquenessSetup_Block_Icons" title="Edit">
                                                        <i class="fa fa-edit"></i>
                                                    </div>
                                                    <div data-bind="click: uniquenessSetup.onClickDeleteButton, attr: { id: 'UniquenessSetup_AttributeLevel_ViewMode_Item_' + chainKeyID + '_Block_6_Info_DeleteButton' }" class="UniquenessSetup_Block_Icons" title="Delete">
                                                        <i class="fa fa-trash-o"></i>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>


                                        <%--EDIT Mode--%>
                                        <div data-bind="attr: { id: 'UniquenessSetup_AttributeLevel_EditMode_' + chainKeyID }, visible: chainIsEdit()" class="UniquenessSetup_AttributeLevel_EditMode_Class UniquenessSetup_EditMode_CSS_Class">

                                            <%--Key Name--%>
                                            <div class="UniquenessSetup_Block_1 UniquenessSetup_Attribute_Edit_Blocks">
                                                <div data-bind="attr: { id: 'UniquenessSetup_AttributeLevel_EditMode_Item_' + chainKeyID + '_Block_1' }" class="UniquenessSetup_AttributeLevel_EditMode_Block_1_Class">
                                                    <input data-bind="attr: { id: 'UniquenessSetup_AttributeLevel_EditMode_Item_' + chainKeyID + '_Block_1_Info' }, textInput: chainKeyName()" />
                                                </div>
                                            </div>

                                            <%--Security Types--%>
                                            <div class="UniquenessSetup_Block_2 UniquenessSetup_Attribute_Edit_Blocks">
                                                <div class="UniquenessSetup_Block_Headers">Security Types</div>
                                                <div data-bind="attr: { id: 'UniquenessSetup_AttributeLevel_EditMode_Item_' + chainKeyID + '_Block_2_Info', title: chainSelectedSectypesTooltipText }, text: chainSelectedSectypesDisplayText" class="UniquenessSetup_Block_Values"></div>
                                            </div>

                                            <%--Attribute Name--%>
                                            <div class="UniquenessSetup_Block_3 UniquenessSetup_Attribute_Edit_Blocks">
                                                <div class="UniquenessSetup_Block_Headers">Attribute</div>
                                                <div data-bind="attr: { id: 'UniquenessSetup_AttributeLevel_EditMode_Item_' + chainKeyID + '_Block_3_Info' }" class="UniquenessSetup_Block_Values"></div>
                                            </div>

                                            <%--EMPTY--%>
                                            <div class="UniquenessSetup_Block_4 UniquenessSetup_Attribute_Edit_Blocks"></div>

                                            <%--EMPTY--%>
                                            <div class="UniquenessSetup_Block_5 UniquenessSetup_Attribute_Edit_Blocks"></div>

                                             <div class="UniquenessSetup_Block_55 UniquenessSetup_Leg_Edit_Blocks">
                                    <div class="">
                                            <div class="UniquenessSetup_Block_Headers">Include Securities In</div>
                                            <div data-bind="attr: { id: 'Sm_DraftCheckBoxParent' + chainKeyID }"><input type="checkbox"  data-bind="attr: { id: 'Sm_DraftCheckBoxInput' + chainKeyID }, checked: checkInDrafts" /><span>Drafts</span> </div>
                                            <div data-bind="attr: { id: 'Sm_WorkflowCheckBoxParent' + chainKeyID }"><input type="checkbox" data-bind="    attr: { id: 'Sm_WorkflowCheckBoxInput' + chainKeyID }, checked: checkInWorkflows" /><span>Workflows</span></div>
                                    </div>
                            </div>

                                             <div class="UniquenessSetup_Block_56 UniquenessSetup_Attribute_Edit_Blocks">

                                                <div class="UniquenessSetup_Block_Headers">Consider Null As Unique Data</div>
                                                <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_EditMode_Item_' + chainKeyID + '_Block_56_Info' }, click: uniquenessSetup.onClickToggleNullAsUnique" class="UniquenessSetup_Block_Values">
                                                    <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_EditMode_Item_' + chainKeyID + '_Block_56_NullAsUniqueYes' }" class="UniquenessSetup_CU_Options LeftToggleButton UniquenessNullAsUniqueRadioWidth">Yes</div>
                                                    <div data-bind="attr: { id: 'UniquenessSetup_LegLevel_EditMode_Item_' + chainKeyID + '_Block_56_NullAsUniqueNo' }" class="UniquenessSetup_CU_Options RightToggleButton UniquenessNullAsUniqueRadioWidth">No</div>
                                                </div>
                                            </div>


                                            <%--Save or Cancel--%>
                                            <div class="UniquenessSetup_Block_6 UniquenessSetup_Attribute_Edit_Blocks">
                                                <div class="UniquenessSetup_Block_Buttons_Div UniquenessSetup_Inner_Block_Save_Cancel_CSS_Class">
                                                    <div data-bind="click: uniquenessSetup.onClickUpdateButton, attr: { id: 'UniquenessSetup_AttributeLevel_EditMode_Item_' + chainKeyID + '_Block_6_Info_SaveButton' }" class="UniquenessSetup_Block_Buttons UniquenessSetup_Block_Buttons_Align UniquenessSetup_CommonGreenBtnStyle">Save</div>
                                                    <div data-bind="click: uniquenessSetup.onClickCancelButton, attr: { id: 'UniquenessSetup_AttributeLevel_EditMode_Item_' + chainKeyID + '_Block_6_Info_CancelButton' }" class="UniquenessSetup_Block_Buttons UniquenessSetup_Block_Buttons_Align UniquenessSetup_CommonTransparentBtnStyle">Cancel</div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
                <%--No keys found Error Div--%>
                <div id="UniquenessSetupMainErrorDiv"></div>
            </asp:Panel>
            <!-- End of Middle Panel -->

            <!-- Beginning of Bottom Panel -->
            <asp:Panel ID="panelBottom" runat="server">
                <div class="PanelsContainerDiv">
                    <asp:Panel ID="pnlSuccess" runat="server" Style="display: none;" DefaultButton="btnOK">
                        <table width="45%" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="alertSuccess">
                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="successHead">Success
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="alertMessage">
                                                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblSuccess" CssClass="alertLabel" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="alertClose">
                                                            <asp:Button ID="btnOK" Text="Close" ToolTip="Click Ok to proceed" CssClass="pageButton"
                                                                runat="server" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlError" runat="server" Style="display: none;" DefaultButton="btnError">
                        <table width="45%" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="alertUnSuccess">
                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="unSuccessHead">Error
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="alertMessage">
                                                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblError" CssClass="alertLabel" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="alertClose">
                                                            <asp:Button ID="btnError" Text="Close" CssClass="pageButton" runat="server" CausesValidation="false"
                                                                TabIndex="1" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                    <asp:Panel ID="pnlDelete" runat="server" Style='display: none;'>
                        <table width="45%" border="0" cellspacing="0" cellpadding="0">
                            <tr>
                                <td class="alertUnsuccessConfirm">
                                    <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                        <tr>
                                            <td class="unSuccessHead">Delete Key
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="alertMessage">
                                                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                                                    <tr style="display: none;">
                                                        <td>
                                                            <h5>[Key:
                                                        <asp:Label ID="lblDelete" runat="server"></asp:Label>]</h5>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td>
                                                            <asp:Label ID="lblDeleteKey" Font-Bold="true" CssClass="alertLabel" runat="server"
                                                                Text="Are you sure you want to DELETE this Key?">
                                                            </asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="alertUnsuccessConfirmClose">
                                                            <asp:Button ID="btnDeleteYES" Text="Yes" CssClass="CommonContinueButton" runat="server"
                                                                ToolTip="Click to Confirm" />
                                                            <asp:Button ID="btnDeleteNO" Text="No" CssClass="CommonDeleteButtonAsText" runat="server"
                                                                ToolTip="Click to Cancel" TabIndex="1" />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>

                    <ajaxToolKit:ModalPopupExtender ID="modalSuccess" Y="0" runat="server" TargetControlID="hdnSuccess"
                        BackgroundCssClass="modalBackground" PopupControlID="pnlSuccess" DropShadow="false"
                        BehaviorID="modalSuccess" CancelControlID="btnOk">
                    </ajaxToolKit:ModalPopupExtender>
                    <ajaxToolKit:ModalPopupExtender ID="modalError" Y="0" runat="server" TargetControlID="hdnError"
                        BackgroundCssClass="modalBackground" PopupControlID="pnlError" DropShadow="false"
                        BehaviorID="modalError" CancelControlID="btnError">
                    </ajaxToolKit:ModalPopupExtender>
                    <ajaxToolKit:ModalPopupExtender ID="modalDelete" Y="0" runat="server" TargetControlID="hdnDelete"
                        BackgroundCssClass="modalBackground" PopupControlID="pnlDelete" DropShadow="false"
                        BehaviorID="modalDelete">
                    </ajaxToolKit:ModalPopupExtender>

                    <input id="hdnDelete" type="hidden" runat="server" />
                    <input id="hdnSuccess" type="hidden" runat="server" />
                    <input id="hdnError" type="hidden" runat="server" />
                </div>
            </asp:Panel>
            <!-- End of Bottom Panel -->

        </div>
        <div id="divErrorSMUSUniqueness" style="display: none;"></div>
        <div id="divErrorSMUSUniquenessOverlay" class="modalBackground" style="display: none; position: fixed; left: 0px; top: 0px; z-index: 10000;"></div>

        <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/jquery-ui.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/knockout-3.4.0.js" type="text/javascript"></script>
        <script src="includes/SMSlimscroll.js" type="text/javascript"></script>
        <script src="includes/SMSelect.js" type="text/javascript"></script>
        <script src="includes/RADCommonScripts.js" type="text/javascript"></script>
        <script src="includes/SecMasterScripts.js" type="text/javascript"></script>
        <%--<script src="includes/RScriptUtils.debug.js" type="text/javascript"></script>--%>
        <script src="includes/MicrosoftAjax.js" type="text/javascript"></script>
        <script src="includes/script.js" type="text/javascript"></script>
        <script src="includes/UniquenessSetup.js" type="text/javascript"></script>
        <script src="includes/SRMUniquenessFailure.js" type="text/javascript"></script>
    </form>
</body>
</html>
