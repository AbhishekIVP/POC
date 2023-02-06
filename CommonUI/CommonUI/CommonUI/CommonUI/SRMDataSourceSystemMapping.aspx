<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMDataSourceSystemMapping.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMDataSourceSystemMapping" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="includes/jquery-ui.min.js"></script>
    <script type="text/javascript" src="includes/jsplumb.min.js"></script>
    <script src="includes/SecMasterScripts.js" type="text/javascript"></script>
    <script type="text/javascript" src="includes/SRMDataSourceSystemMapping.js"></script>
    <link rel="stylesheet" href="css/SRMDataSourceSystemMapping.css" />
    <%--<script src="includes/SMSlimscroll.js"></script>--%>
</head>
<body>
    <form id="form1" runat="server">
        <%-- <i class="fa fa-plus"></i>
           <div class="srmdatasourcesystemmapping_setarrow">
                    <div class="srmdatasourcesystemmapping_arrow"></div>
                    </div>--%>
        <%-- <div id="SRMDataSourceParent" class=""> --%>
        <div id="srmdatasource_parent_div" class="semdatasourceSystem_mapping_parent_div">

            <div id="SRMDataSourceAndSystemMappingParent" class="srmdatasourcesystemmap_parent">

                <div id="SRMDataSourceAndSystemMappingContainer" class="srmdatasourcesystemmap_container">

                    <div id="srmDataSourceSystemMapping_upstream_container" class="srmDataSourceSystemMapping_upstreamcontainer">
                        <div id="srmDataSourceSystemMapping_upstream" class="horizontal_align_test_up ">
                            <div id="srmDataSourceSystemMapping_upstream_heading" class="srmdatasourcesystemmapping_up_heading_div">
                                <div class="srmDataSourceSystemMapping_up_arrow"></div>
                                <label class="srmDataSourceSystemMapping_align_mid">UPSTREAM</label>
                            </div>
                            <div id="srmDataSourceSystemMapping_upstream_label" class="srmDataSourceSystemMapping_down_label"></div>
                            <div id="srmdatasourcesystemmapping_scrolldiv" class="srmDataSourcesystemMapping_scrolldiv">
                                <div data-bind="foreach: dataSourceList" id="SRMDataSourceAndSystemMappingLeftPanel" class="SRMDataSourceSystemMappingInlineBlock SRMDataSourceSystemMappingPanel middle_align_test">
                                    <div data-bind="attr: { id: 'dataSourceItem_' + dataSourceName }" class="SRMDataSourceItem ">
                                        <div data-bind="text: dataSourceName, attr: { 'dataSourceId': dataSourceId }" class="SRMDataSourceName"></div>
                                        <%--<div data-bind="text: info" class="SRMDataSourceInfo"></div>--%>
                                    </div>
                                </div>

                            </div>
                            <div id="srmdatasourcesystemmapping_scrollbutton_div" class="srmdatasourcesystemmapping_scrollbutton">
                            <i id="srmDataSourceSystemMapping_scrollup_button" class="fa fa-caret-up srmdataSourcemapping_sethover" style="font-size:48px;padding:6px;color:#b4b4b4;"></i>
                            <i id="srmDataSourceSystemMapping_scrolldown_button" class="fa fa-caret-down srmdataSourcemapping_sethover" style="font-size:48px;padding:6px;color:#b4b4b4;"></i>
                                </div>
                        </div>
                    </div>
                    <div id="srmDataSourceSystemMapping_entity_container" class="srmDataSourceSystemMapping_setarrow">
                        <div id="srmDataSourceSystemMapping_entity_type" class="horizontal_align_test_entity ">

                            <div id="srmDataSourceSystemMapping_arrow1" class="srmdatasourcesystemmapping_arrow srmDataSourceSystemMapping_setmargin_right"></div>

                            <div id="SRMDataSourceAndSystemMappingCenterPanel" class="SRMDataSourceSystemMappingInlineBlock SRMDataSourceSystemMappingPanelCenter">
                                <div class="SRMDataSourceType" id="SRMDataSourceType">
                                    <div class="SRMDataSourceEntityName" data-bind="text: entityTypeDisplayName"></div>
                                </div>
                            </div>
                            <div class="srmdatasourcesystemmapping_arrow srmDataSourceSystemMapping_setmargin_left"></div>
                        </div>
                    </div>

                    <div id="srmDataSourceSystemMapping_downstream_container" class="srmDataSourceSystemMapping_downstreamcontainer">
                        <div id="srmDataSourceSystemMapping_downstream" class="horizontal_align_test_down">
                            <div id="srmDataSourceSystemMapping_down_heading" class="srmdatasourcesystemmapping_down_heading_div ">
                                <div class="srmDataSourceSystemMapping_down_arrow"></div>

                                <label class="srmDataSourceSystemMapping_align_mid">DOWNSTREAM</label>
                            </div>

                            <div id="srmDataSourceSystemMapping_downstream_label" class="srmDataSourceSystemMapping_down_label"></div>

                            <%--</div>--%>
                            <div id="srmdatasourcesystemmapping_down_scrolldiv" class="srmDataSourcesystemMapping_scrolldiv">

                                <div data-bind="foreach: systemList" id="SRMDataSourceAndSystemMappingRightPanel" class="SRMDataSourceSystemMappingInlineBlock SRMDataSourceSystemMappingPanel middle_align_test ">
                                    <div data-bind="attr: { id: 'systemItem_' + systemInfoName }" class="SRMSystemItem SRMDataSourceFloatRight">
                                        <div data-bind="text: systemInfoName, attr: { 'systemInfoId': systemInfoId, 'reportTypeId': reportTypeId }" class="SRMSystemName "></div>
                                        <%--<div data-bind="text: info" class="SRMSystemInfo"></div>--%>
                                    </div>
                                </div>
                            </div>
                                <div id="srmdatasourcesystemmapping_down_scrollbutton_div" class="srmdatasourcesystemmapping_scrollbutton">
                            <i id="srmDataSourceSystemMapping_down_scrollup_button" class="fa fa-caret-up srmdataSourcemapping_sethover" style="font-size:48px;padding:6px;color:#b4b4b4;"></i>
                            <i id="srmDataSourceSystemMapping_down_scrolldown_button"  class="fa fa-caret-down srmdataSourcemapping_sethover" style="font-size:48px;padding:6px;color:#b4b4b4;"></i>
                                </div>

                        </div>
                    </div>

                </div>
            </div>

            <div id="mapping_data_grid" class="srmdatasourcesystemmap_partition_grid grid_align_test">
                <div id="SRMDataSourceAndSystemMappingGrid" style="box-shadow: -2px -1px 2px 1px #ececec;">
                    <%--   <div id="SRMDataSourceAndSystemMappingGridHeader" class="SRMDataSourceAndSystemMappingGridHeader">
                        <div id="closeDataSourceAndSystemMappingGrid" class="closeDataSourceAndSystemMappingGrid">
                        </div>
                    </div>--%>
                    <div id="SRMDataSourceAndSystemMappingGridSection">
                        <div id="feed" style="display: none">
                            <div id="feedHeader" class="SRMDataSourceAndMappingGridHeaders">
                                <%--<div class="SRMDataSourceAndMappingGridHeaderColumn">
                                    <div class="SRMDataSourceAndMappingGridLabel">Entity Type:</div>
                                    <div class="SRMDataSourceAndMappingGridValue" id="SRMDataSourceAndMappingEntityType1"></div>
                                </div>--%>
                                <div class="SRMDataSourceAndMappingGridHeaderColumn">
                                    <%--<div class="SRMDataSourceAndMappingGridLabel">Data Source:</div>--%>
                                    <div class="SRMDataSourceAndMappingGridValue" id="SRMDataSourceAndMappingDataSource"></div>
                                </div>
                                <div class="srmdatasourcesystemmapping_set_editdiv">
                                    <button id="srmdatasourcesystemmapping_edit" class="srmdatasourcesystemmapping_edit_button">Edit</button>
                                </div>
                            </div>

                            <div id="feedTable">
                                <div class="ColumnNames rowborder SRMDataSourceCenterAlign" data-bind="chainViewModel">
                                    <div class="SRMDataSourceAndSystemMapping_head_cells">Feed Name</div>
                                    <div class="SRMDataSourceAndSystemMapping_head_cells">Attribute Name</div>
                                    <div class="SRMDataSourceAndSystemMapping_head_cells">Field Name</div>


                                </div>
                                <div id="SRMDataSourceAndMappinglist" data-bind="foreach: DataList">

                                    <div class="row_container SRMDataSourceCenterAlign rowborder">
                                        <div class="SRMDataSourceAndSystemMapping_cells" data-bind="text: Feed_Name"></div>
                                        <%--   <div class="SRMDataSourceAndSystemMapping_space"></div>--%>
                                        <div class="SRMDataSourceAndSystemMapping_cells" data-bind="text: Attribute_Name"></div>
                                        <%--     <div class="SRMDataSourceAndSystemMapping_space"></div>--%>

                                        <div class="SRMDataSourceAndSystemMapping_cells" data-bind="text: Field_Name"></div>


                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="report">
                            <div id="reportHeader" class="SRMDataSourceAndMappingGridHeaders">
                                <%-- <div class="SRMDataSourceAndMappingGridHeaderColumn">
                                    <div class="SRMDataSourceAndMappingGridLabel">Entity Type:</div>
                                    <div class="SRMDataSourceAndMappingGridValue" id="SRMDataSourceAndMappingEntityType2"></div>
                                </div>--%>
                                <div class="SRMDataSourceAndMappingGridHeaderColumn">
                                    <%--<div class="SRMDataSourceAndMappingGridLabel">Report System:</div>--%>
                                    <div class="SRMDataSourceAndMappingGridValue" id="SRMDataSourceAndMappingReportSystem"></div>
                                     
                                </div>
                                <div class="srmdatasourcesystemmapping_set_editdiv">
                                    <button id="srmdatasourcesystemmapping_edit_report" class="srmdatasourcesystemmapping_edit_button">Edit</button>
                                </div>
                            </div>

                            <div id="reportTable">
                                <div id="SRMDataSourceAndMapping_report" class="ColumnNames SRMDataSourceCenterAlign rowborder_report" data-bind="chainViewModel_report">
                                    <%--<div class="head_cells">Report Name</div>--%>
                                    <div class="SRMDataSourceAndSystemMapping_head_cells_report SRMDataSourceAndSystemMapping_cells_Attr_width">Attribute Name</div>
                                    <div class="SRMDataSourceAndSystemMapping_head_cells_report SRMDataSourceAndSystemMapping_cells_RAttr_width">Report Attribute Name</div>

                                </div>
                                  <div id="SRMDataSourceAndMapping_report_case" class="ColumnNames SRMDataSourceCenterAlign rowborder_report" data-bind="chainViewModel_report_case2">
                                    <%--<div class="head_cells">Report Name</div>--%>
                                    <%--<div class="SRMDataSourceAndSystemMapping_head_cells_report SRMDataSourceAndSystemMapping_cells_RAttr_width">Report Name</div>--%>
                                    <div class="SRMDataSourceAndSystemMapping_head_cells_report SRMDataSourceAndSystemMapping_cells_Attr_width">Attribute Name</div>

                                </div>
                                <div id="reportlist" data-bind="foreach: DataList_report">

                                    <div class="row_container rowborder_report SRMDataSourceCenterAlign">
                                       <%-- <div class="cells" data-bind="text: Report_Name"></div>--%>
                                        <div class="SRMDataSourceAndSystemMapping_cells_report" data-bind="text: Attribute_Name"></div>
                                        <div class="SRMDataSourceAndSystemMapping_cells_report" data-bind="text: Report_Attribute_Name"></div>


                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>


        </div>



        <script src="js/thirdparty/knockout-3.4.0.js" type="text/javascript"></script>
    </form>
</body>
</html>

