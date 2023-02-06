var xlgridloader = {
    create: function (gridId, cacheKey, block, clientSideData) {
        var info = block;
        Sys.Application.add_init(function () {


            var columninformation = "[ {name : 'color',type : 'string'},{name:'value':type:'string'}]";
            var gridinfo = $.extend({
                "SelectRecordsAcrossAllPages": null,
                "ViewKey": "test",
                "GridId": gridId,
                "CurrentPageId": "test",
                "SessionIdentifier": null,
                "UserId": "test",
                "CheckBoxInfo": {},
                "ColumnsWithoutClientSideFunctionality": [],
                "ColumnsNotToSum": [],
                //"IdColumnName": null,
                "KeyColumns": {},
                "ColumnsToHide": [],
                "PageSize": 500,
                "CustomRowsDataInfo": [],
                "ItemText": "Number of Attributes",
                "Height": "100%",
                "DefaultGroupedAndSortedColumns": [],
                "EditableRows": [],
                "NonEditableRows": [],
                "EditableColumns": [],
                "ColumnNotToExport": [],
                "ExcelSheetName": "Grid Details",
                "ColumnsCanBeDeleted": [],
                "HideFooter": false,
                "UpperHeaderText": null,
                "DoNotExpand": false,
                "FrozenColumns": [],
                "FrozenRows": [],
                "DoNotRearrangeColumn": false,
                "ColumnNameMapping": {},
                "ExtraRowDataInfo": [],
                "ColumnDateFormatMapping": {},
                //"ColumnCustomFormatMapping": null,
                "RequireGrouping": true,
                "RequireFilter": true,
                "RequireSort": true,
                "RequireMathematicalOperations": false,
                "RequireSelectedRows": true,
                "RequireEditGrid": false,
                "RequireFullScreen": false,
                "RequireRADBalancePopup": false,
                "RADBalancePopupDataSource": null,
                "RequireRADExtraPopup": false,
                "DefaultFilteredColumns": null,
                "LoadPopupDataWithFilterFirstTime": false,
                "LoadPopupDataWithFilter": false,
                "RequireLayouts": false,
                "RequireExportToExcel": true,
                "RequireRuleBasedColoring": false,
                "RequireSliderFilter": false,
                "RequireSearch": true,
                "RequirePaging": true,
                "RequireHideColumns": true,
                "RequireFreezeColumns": true,
                "RequireAbsoluteSort": true,
                "RequireColumnSwap": false,
                "RequireResizing": true,
                "RequireGroupExpandCollapse": true,
                "AutoAdjust": 0,
                "RequireBodyClickClearSelection": true,
                "ClearFilterInfoBeforeBulkFiltering": false,
                "RequirePastingDataOnDataFromExcel": false,
                "RequireTooltipOnData": false,
                "ColumnAutoAdjustMapping": {},
                "ColumnWidths": {},
                "ExtraInfoPopupTitle": null,
                "RequireExportToPdf": false,
                "PdfHeaderText": "hELLO",
                "EmptyGridText": "",
                "MessageBoxHandler": null,
                "ColumnLevelMathematicalOperations": {},
                "ShowRecordCountOnHeader": false,
                "ShowAggregationOnHeader": false,
                "CssSearchTextBox": "xlneogridBtn",
                "CssClearFilter": "xlneoclearFilter",
                "CssClearSort": "xlneoclearSort",
                "CssClearGroup": "xlneoclearGrouping",
                "CssExportVisibleRows": "xlneoexportToExcelVisibleRows",
                "CssExportRows": "xlneoexportToExcel",
                "CssFilter": "xlneofilterIcon",
                "CssFiltered": "xlneofilterDropIcon",
                //"CssFilter": "xlfilterIcon",
                //"CssFiltered": "xlfilterDropIcon",
                "CssRecordSummary": "xlneorecordSummary",
                "CssGroupDropPanel": "xlneogroupDropPanel",
                "CssUpperHeader": "xlneoupperHeader",
                "CssHeader": "xlneoheader",
                "CssGroupDiv": "xlneogroupDiv",
                "CssPrevPage": "xlneoprevPage fa fa-backward fa-lg",
                "CssNextPage": "xlneonextPage fa fa-forward fa-lg",
                "CssPageTextBox": "xlneogridTxtBox",
                "CssStatusBar": "xlneostatusBar",
                "CssPageChange": "input",
                "DateFormat": "yyyyMMdd",
                "CssRemoveColumn": null,
                "CssClearSelection": "clearSelectionBtn",
                "PreviousStartIndex": 0,
                "InfiniteScroll": false,
                "ScrollCount": 0,
                "GroupHeaderInfo": {},
                "CheckGroupHeaderRows": [],
                "RowsToCheck": [],
                "ajaxStart": false,
                "RightAlignHeaderForNumerics": true,
                "RequireInfiniteScroll": true,
                "JsonData": clientSideData,
                "sizeJson": {},
                "IsMasterChildGrid": false,
                "ParentGridId": "",
                "MasterGridId": "",
                "CurrentRowId": "",
                "IsMasterGridSearch": false,
                "ChildGridsToOpen": [],
                "IdColumnNames": [],
                "ParentIDColumnName": "",
                "ColumnsToDisplaySum": [],
                "GridChildLevel": 0,
                "AutoGenerateIdColumn": false,
                "RequireColumnAlign": false,
                "ColumnAlignmentMapping": [],
                "ClientSideGrid":true,
                "ColumnList": [],
                "CollapseAllGroupHeader": true,
                "RequireEditableRow":false,
                "EditableColumnsInfo":[],
                "taggingInfoID" : "",
                "RequireShadow":true,
                "EditGridFocusOut":"",
                "ComputedNewColumns":[],
                "ClearSerializationData":true,
                "RequireTagging":false,
                "ShowTotalRecordCount" : false,
                "CustomHeaderInfo":{},
                "CssAlternatingRow" : "xlneoalternatingrow",
                "CssNormalRow" : "xlneonormalRow",
                "CssSelectedRow" : "xlselectedRow",
                "CssCheckedRow" : "xlneocheckedRow",
                "xlneochildgridParent" : "xlneochildgridParent",
                "xlneoMasterheaderbackground" : "xlneoMasterheaderbackground",
                "GridTheme" : 0,
                "MasterChildEditableColumnInfo" : [],
                 "RequireViews":false,
                "RaiseGridCallBackBeforeExecute":"",
                "RequireRanking":false
            }, info ? info : {});

            if (gridinfo.Height == "100%") {
                //gridinfo.Height = (block.elem.height() - 105) + "px";
            }
//            for (var key in $.parseJSON(clientSideData)[gridinfo.TableName][0]) {
//                if (gridinfo.IsMasterChildGrid);
//                else {
//                    if (gridinfo.AutoGenerateIdColumn || gridinfo.IdColumnName != key)
//                        gridinfo.ColumnList.push(key);

//                    else if (!Array.contains(gridinfo.ColumnsToHide, key))
//                        gridinfo.ColumnList.push(key);

//                    else {
//                        if (!Array.contains(gridinfo.ColumnNameMapping, key))
//                            gridinfo.ColumnList.push(key);
//                        else
//                            gridinfo.ColumnList.push(gridinfo.ColumnNameMapping[key]);
//                    }
//                }
            //            }

            if (clientSideData != "") {
                for (var key in $.parseJSON(clientSideData)[gridinfo.TableName][0]) {
                    if (gridinfo.IsMasterChildGrid);
                    else {
                        if (gridinfo.AutoGenerateIdColumn || gridinfo.IdColumnName != key) {
                            if (Array.contains(gridinfo.ColumnsToHide, key))
                                ;
                            else {
                                if (gridinfo.ColumnNameMapping[key] == null)
                                    gridinfo.ColumnList.push(key);
                                else
                                    gridinfo.ColumnList.push(gridinfo.ColumnNameMapping[key]);
                            }

                        }
                    }
                }
            }
            var comp = Sys.Application.findComponent(gridId);
            if (comp != null) {
                comp.dispose();
                $("#" + gridId).empty();
            }
			$("div[id*='_CustomMessageBoxID']").remove();
            $create(com.ivp.rad.controls.neogrid.scripts.Grid, {
                "CacheKey": cacheKey,
                "DefaultGroupedAndSortedColumns": [],
                "GridInfo": gridinfo,
                "RaiseGridBeginUpdate": null,
                "RaiseGridRenderComplete": null,
                "RaiseGridUpdated": null,
                "RaiseOnColumnRemove": null,
                "RaiseOnEditCommitChange": null,
                "RaiseClickEvent": null,
                "CheckBoxSelectionMode": 0,
            }, null, null, $get(gridId));
        });
        //xlgridloader.contextMenuGrid(block);
    },

    contextMenuGrid: function (block) {
        if (block.gridInfo.contextMenu) {
            $(block.elem[0]).attr("contextmenugrid", JSON.stringify(block.gridInfo.contextMenu));
            $('div.tile[contextmenugrid]').unbind('contextmenu.grid');
            $('div.tile[contextmenugrid]').bind("contextmenu.grid", function (e) {
                var rowclass = ["xlalternatingRow", "xlnormalRow"];
                $("#dv-context-menu").empty();
                if ($(e.target).closest('tr').length > 0 && (jQuery.inArray($(e.target).closest('tr')[0].className, rowclass) != -1)) {
                    var contextmenu = JSON.parse($(e.target).closest('div.tile[contextmenugrid]').attr("contextmenugrid"));
                    for (var i = 0; i < contextmenu.length; i++) {
                        $("#dv-context-menu").append("<div id=\"" + contextmenu[i].name + "\" class=\"contextmenu " + contextmenu[i].iconclass + " f_c_mostimp font_fourteenpx font_weight_l2\" style=\"float:left;padding: 20px;background-position-y: 3px;padding-top: 35px;\" icon=\"" + contextmenu[i].iconclass + "\" command=\"" + contextmenu[i].command + "\">" + contextmenu[i].name + "</div>");
                        xlgridloader.contextMenuAction(contextmenu[i], $(e.target).closest('tr')[0]);
                    }
                    $("#dv-context-menu").css({ "bottom": "1px" });
                }
            });
            $(document).unbind("click.contextgrid");
            $(document).bind("click.contextgrid", function (e) {
                var click = e.which;
                if ($(e.target).is('#dv-context-menu *') || (click == 3)) {
                    return;
                }
                else {
                    xlgridloader.removeGridContextMenus();
                }
            });
        }
    },

    contextMenuAction: function (options, row) {
        new ___.Action({
            "name": options.name,
            "commands": options.command.split(','),
            "on": function (action) {
                $("#" + options.name).click(function () {
                    $('.xlalternatingRow,.xlnormalRow').removeClass("xlrowselected");
                    $(row).addClass("xlrowselected");
                    action(row);
                    xlgridloader.removeGridContextMenus();
                });
            }
        });
    },
    removeGridContextMenus: function () {
        $("#dv-context-menu").animate({
            'bottom': '-1000px'
        }, 500);
        $("#dv-context-menu").empty();
    }
}

function radxlGridRendred() {
    hbps.publish({
        "topic": "web.metro.xlgrid.Rendered",
        "data": this
    });
}


function ClearComponent(gridId) {
    var comp = Sys.Application.findComponent(gridId);
    if (comp != null) {
        ko.cleanNode($("#" + gridId + "_repeatedFrozenDiv")[0]);
        ko.cleanNode($("#" + gridId + "_repeatedBodyDiv")[0]);
        //ko.cleanNode(document.body);
        //comp.dispose();
        $("#" + gridId).parent().remove();
        $("#" + gridId).remove();

    }
}


function ChildGrid(id, clientSideDataFromGrid, parentGridId, MasterGridId, currentRowId, MasterChildMapping, isMasterGridSearch, idcolumnName, idcolumnNames, parentidColumnName, CustomDataList, RaiseGridRenderList, ColumnsToHideList, ColumnNameMappingList, CustomFormatInfoClientSide,gridChildLevel, chilGridMappedWidth, ColumnAlignmentMapping, RequireColumnAlign, MasterChildColumnMap, LastChildMappingList,Requirepaging,Pagesize,childGridDateFormat) {
    var autogenerateidcolumn = false;
    if (idcolumnName.toLowerCase() == "autogenerateidcolumn") {
        idcolumnName = "__ID";
        autogenerateidcolumn = true;
    }
   
    var grid = {
        "SelectRecordsAcrossAllPages": null,
        "ViewKey": "test",
        "GridId": id,
        "CurrentPageId": "test",
        "SessionIdentifier": null,
        "UserId": "test",
        "CheckBoxInfo": null,
        //"Height": "100px",
        "ColumnsWithoutClientSideFunctionality": [],
        "ColumnsNotToSum": [],
        "IdColumnName": idcolumnName,
        "TableName": "Table",
        //"CustomFormatInfoClientSide": { "as_of_date": { "AssemblyName": "RADGridWebTest", "FormatString": "", "ClassName": "RADGridWebTest.CurrencyFormatter"} },
        "ColumnNameMapping": { "total_local": "Total Local", "sector": "Sector" },
        "ColumnDateFormatMapping": { "as_of_date": "dd/M/yyyy" },
        "ColumnWidths": { "sector": "50" },
        //"CustomRowsDataInfo": [{"RowID":"1","StyleAttribute":{},"Attribute":{},"Cells":[{"ColumnName":"security_code","StyleAttribute":{},"Attribute":{},"NewChild":"<img src=\"App_Themes/Aqua/images/add-btn.jpg\"/>","Append":true}]}],
        "RequireEditGrid": false,
        "EditableColumns": ['analyst'],
        "RequireExportToExcel": false,
        "RequireInfiniteScroll": false,
        //"RequireAddClientSideColumn": true,
        //"FrozenColumns": [{ "ColumnName": "sector", "isDefault": true}],
        "RequireAbsoluteSort": true,
        //"RequirePastingDataOnDataFromExcel": true,
        "RequireResizing": true,
        //"RequireAddClientSideColumn": true,
        "ShowRecordCountOnHeader": true,
        //"RaiseGridRenderComplete": "Render",
        "RequirePaging": Requirepaging[0],
        "IsMasterChildGrid": true,
        "ParentGridId": parentGridId,
        "MasterGridId": MasterGridId,
        "CurrentRowId": currentRowId,
        "MasterChildMapping": MasterChildMapping,
        "IsMasterGridSearch": isMasterGridSearch,
        "RequireSearch": false,
        "IdColumnNames": idcolumnNames,
        "AutoGenerateIdColumn": autogenerateidcolumn,
        "ParentIDColumnName": parentidColumnName,
        "RequireGrouping": false,
        "CustomInfoList": CustomDataList,
        "RaiseGridRenderList": RaiseGridRenderList,
        "ColumnsToHideList": ColumnsToHideList,
        "ColumnNameMappingList": ColumnNameMappingList,
        "GridChildLevel": gridChildLevel,
        "RaiseGridRenderComplete": RaiseGridRenderList[0],
        "ChildGridMappedWidth": chilGridMappedWidth,
        "RequireColumnAlign": RequireColumnAlign,
        "ColumnAlignmentMapping": ColumnAlignmentMapping,
        "MasterChildMappedColumnValue": MasterChildColumnMap,
        "LastChildMappingList": LastChildMappingList,
        "PageSize": Pagesize[0],
        "PageSizeMasterChildGrid": Pagesize,
        "RequirePagingMasterChildGrid": Requirepaging,
        "DateFormat": $find(MasterGridId).get_GridInfo().DateFormat,
        "taggingInfoID" : ""

        //"DefaultGroupedAndSortedColumns": [{ "ColumnName": "market value local", "SortOrder": 1, "IsGrouped": true, "IsAbsoluteSorted": false}]

        //"CustomRowsDataInfo": [{ "RowID": "2", "StyleAttribute": {}, "Attribute": {}, "Cells": [{ "ColumnName": "total_local", "StyleAttribute": {}, "Attribute": {}, "NewChild": "<div align=\"center\"><img src=\"App_Themes/Aqua/images/message.jpg\"/><a target=\"_blank\" onclick=\"http://demo.ivp.in/IVPTreasuryRecon/RLBreakManagement.aspx?identifier=BreakManagement&tgt=BreakManagement&pipedInfo=149|320&ReconName=OTC Collateral Recon CITI&nvid=&IsMonthEnd=False&filterInfo=Recon Name|OTC Collateral Recon CITI;\" >0</a></div>", "Append": false}]}]

        //"CustomRowsDataInfo": [{\"RowID\":\"1\",\"StyleAttribute\":{},\"Attribute\":{},\"Cells\":[{\"ColumnName\":\"summary_collateral\",\"StyleAttribute\":{},\"Attribute\":{},\"NewChild\":\"\\u003cdiv align=\\\"center\\\"\\u003e\\u003cimg src=\\\"App_Themes/Aqua/images/message.jpg\\\"/\\u003e\\u003ca target=\\\"_blank\\\" onclick=\\\"OpenPage(\\u0027RLBreakManagement.aspx?identifier=BreakManagement\\u0026tgt=BreakManagement\\u0026pipedInfo=149|320\\u0026ReconName=OTC Collateral Recon CITI\\u0026nvid=\\u0026IsMonthEnd=False\\u0026filterInfo=Recon Name|OTC Collateral Recon CITI~Asset Name|Collateral\\u0026UniquePageIdentifier=hu3443HU\\u0027)\\\" \\u003e0\\u003c/a\\u003e\\u0026nbsp;\\u0026nbsp;\\u0026nbsp;\\u0026nbsp;\\u003cimg src=\\\"App_Themes/Aqua/images/comment.jpg\\\"/\\u003e\\u003ca target=\\\"_blank\\\" href=\\\"http%3a%2f%2fdemo.ivp.in%2fIVPTreasuryRecon%2fRLBreakManagement.aspx%3fidentifier%3dBreakManagement%26tgt%3dBreakManagement%26pipedInfo%3d149%7c320%26ReconName%3dOTC+Collateral+Recon+CITI%26nvid%3d%26IsMonthEnd%3dFalse%26filterInfo%3dRecon+Name%7cOTC+Collateral+Recon+CITI%7eAsset+Name%7cCollateral%26UniquePageIdentifier%3dhu3443HU\\\"\\u003e0\\u003c/a\\u003e\\u0026nbsp;\\u0026nbsp;\\u0026nbsp;\\u0026nbsp;\\u0026nbsp;\\u0026nbsp;\\u003cimg src=\\\"App_Themes/Aqua/images/close.jpg\\\"/\\u003e\\u0026nbsp;\\u003ca target=\\\"_blank\\\" href=\\\"http%3a%2f%2fdemo.ivp.in%2fIVPTreasuryRecon%2fRLBreakManagement.aspx%3fidentifier%3dBreakManagement%26tgt%3dBreakManagement%26pipedInfo%3d149%7c320%26ReconName%3dOTC+Collateral+Recon+CITI%26nvid%3d%26IsMonthEnd%3dFalse%26filterInfo%3dRecon+Name%7cOTC+Collateral+Recon+CITI%7eAsset+Name%7cCollateral%26UniquePageIdentifier%3dhu3443HU\\\"\\u003e2\\u003c/a\\u003e\\u003c/div\\u003e\",\"Append\":false},{\"ColumnName\":\"summary_position\",\"StyleAttribute\":{},\"Attribute\":{},\"NewChild\":\"\\u003cdiv align=\\\"center\\\"\\u003e\\u003cimg src=\\\"App_Themes/Aqua/images/message.jpg\\\"/\\u003e\\u003ca target=\\\"_blank\\\" href=\\\"http%3a%2f%2fdemo.ivp.in%2fIVPTreasuryRecon%2fRLBreakManagement.aspx%3fidentifier%3dBreakManagement%26tgt%3dBreakManagement%26pipedInfo%3d149%7c320%26ReconName%3dOTC+Collateral+Recon+CITI%26nvid%3d%26IsMonthEnd%3dFalse%26filterInfo%3dRecon+Name%7cOTC+Collateral+Recon+CITI%7eAsset+Name%7cCollateral%26UniquePageIdentifier%3dhu3443HU\\\"\\u003e0\\u003c/a\\u003e\\u0026nbsp;\\u0026nbsp;\\u0026nbsp;\\u0026nbsp;\\u003cimg src=\\\"App_Themes/Aqua/images/comment.jpg\\\"/\\u003e\\u003ca target=\\\"_blank\\\" href=\\\"http%3a%2f%2fdemo.ivp.in%2fIVPTreasuryRecon%2fRLBreakManagement.aspx%3fidentifier%3dBreakManagement%26tgt%3dBreakManagement%26pipedInfo%3d149%7c320%26ReconName%3dOTC+Collateral+Recon+CITI%26nvid%3d%26IsMonthEnd%3dFalse%26filterInfo%3dRecon+Name%7cOTC+Collateral+Recon+CITI%7eAsset+Name%7cCollateral%26UniquePageIdentifier%3dhu3443HU\\\"\\u003e1\\u003c/a\\u003e\\u0026nbsp;\\u0026nbsp;\\u0026nbsp;\\u0026nbsp;\\u0026nbsp;\\u0026nbsp;\\u003cimg src=\\\"App_Themes/Aqua/images/close.jpg\\\"/\\u003e\\u0026nbsp;\\u003ca target=\\\"_blank\\\" href=\\\"http%3a%2f%2fdemo.ivp.in%2fIVPTreasuryRecon%2fRLBreakManagement.aspx%3fidentifier%3dBreakManagement%26tgt%3dBreakManagement%26pipedInfo%3d149%7c320%26ReconName%3dOTC+Collateral+Recon+CITI%26nvid%3d%26IsMonthEnd%3dFalse%26filterInfo%3dRecon+Name%7cOTC+Collateral+Recon+CITI%7eAsset+Name%7cCollateral%26UniquePageIdentifier%3dhu3443HU\\\"\\u003e2\\u003c/a\\u003e\\u003c/div\\u003e\",\"Append\":false},{\"ColumnName\":\"shortfall_after_rounding\",\"StyleAttribute\":{},\"Attribute\":{},\"NewChild\":\"\\u003cdiv align=\\\"right\\\"\\u003e\\u003ca target=\\\"_blank\\\" href=\\\"http%3a%2f%2flocalhost%2fCashMaster%2fWebForms%2fPageContainer1.aspx%3fPageId%3d17\\\"\\u003e22050000\\u003c/a\\u003e\\u003c/div\\u003e\",\"Append\":false}]}]
        //"ColumnsToHide": { "ColumnName": "sector", "isDefault": false}]
    };
    //             var comp = Sys.Application.findComponent(id);
    //             if (comp != null) {
    //                 comp.dispose();
    //                 $("#" + id).remove();
    //             }
    //$("#parent").append("<div id=\"" + id + "\"></div>")
    //var clientSideData = "{\"NewDataSet\":{\"xs:schema\":{\"@id\":\"NewDataSet\",\"@xmlns\":\"\",\"@xmlns:xs\":\"http://www.w3.org/2001/XMLSchema\",\"@xmlns:msdata\":\"urn:schemas-microsoft-com:xml-msdata\",\"xs:element\":{\"@name\":\"NewDataSet\",\"@msdata:IsDataSet\":\"true\",\"@msdata:UseCurrentLocale\":\"true\",\"xs:complexType\":{\"xs:choice\":{\"@minOccurs\":\"0\",\"@maxOccurs\":\"unbounded\",\"xs:element\":{\"@name\":\"Table\",\"xs:complexType\":{\"xs:sequence\":{\"xs:element\":[{\"@name\":\"id\",\"@type\":\"xs:int\",\"@minOccurs\":\"0\"},{\"@name\":\"security_code\",\"@type\":\"xs:string\",\"@minOccurs\":\"0\"},{\"@name\":\"as_of_date\",\"@type\":\"xs:dateTime\",\"@minOccurs\":\"0\"},{\"@name\":\"col\",\"@type\":\"xs:string\",\"@minOccurs\":\"0\"},{\"@name\":\"total_local\",\"@type\":\"xs:decimal\",\"@minOccurs\":\"0\"},{\"@name\":\"AC\",\"@type\":\"xs:string\",\"@minOccurs\":\"0\"},{\"@name\":\"sector\",\"@type\":\"xs:string\",\"@minOccurs\":\"0\"},{\"@name\":\"quantity\",\"@type\":\"xs:decimal\",\"@minOccurs\":\"0\"}]}}}}}}},\"Table\":[{\"id\":\"1\",\"as_of_date\":\"2008-01-02T00:00:00+05:30\",\"col\":\"Nokia Corp.\",\"total_local\":\"16166.181200\",\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Textiles\",\"quantity\":\"3700.000000\"},{\"id\":\"2\",\"as_of_date\":\"2008-01-02T00:00:00+05:30\",\"col\":\"Nokia Corp.\",\"total_local\":\"16403.724854\",\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Textiles\",\"quantity\":\"10000.000000\"},{\"id\":\"3\",\"as_of_date\":\"2009-09-01T00:00:00+05:30\",\"col\":\"Nokia Corp.\",\"total_local\":\"16166.181200\",\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":\"3700.000000\"},{\"id\":\"4\",\"as_of_date\":\"2009-09-01T00:00:00+05:30\",\"col\":\"Nokia Corp.\",\"total_local\":\"16403.724854\",\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":\"10000.000000\"},{\"id\":\"5\",\"as_of_date\":\"2009-09-02T00:00:00+05:30\",\"col\":\"Nokia Corp.\",\"total_local\":\"17356.964551\",\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":\"3700.000000\"}]}}";
    //var clientSideData = "{\"Table\":[{\"id\":1,\"security_code\":null,\"as_of_date\":\"2008-01-02T00:00:00\",\"col\":\"Nokia Corp.\",\"total_local\":16166.181200,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Textiles\",\"quantity\":3700.000000},{\"id\":2,\"security_code\":null,\"as_of_date\":\"2008-01-02T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":16403.724854,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Textiles\",\"quantity\":10000.000000},{\"id\":3,\"security_code\":null,\"as_of_date\":\"2009-09-01T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":16166.181200,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":3700.000000},{\"id\":4,\"security_code\":null,\"as_of_date\":\"2009-09-01T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":16403.724854,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":10000.000000},{\"id\":5,\"security_code\":null,\"as_of_date\":\"2009-09-02T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":17356.964551,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":3700.000000},{\"id\":6,\"security_code\":null,\"as_of_date\":\"2009-09-02T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":18242.772501,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":10000.000000},{\"id\":7,\"security_code\":null,\"as_of_date\":\"2009-09-03T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":18320.932026,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":3700.000000},{\"id\":8,\"security_code\":null,\"as_of_date\":\"2009-09-03T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":19009.042354,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":10000.000000},{\"id\":9,\"security_code\":null,\"as_of_date\":\"2009-09-04T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":19681.827285,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":3700.000000},{\"id\":10,\"security_code\":null,\"as_of_date\":\"2009-09-04T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":19622.058237,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":10000.000000},{\"id\":11,\"security_code\":null,\"as_of_date\":\"2009-09-05T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":21326.242390,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":3700.000000},{\"id\":12,\"security_code\":null,\"as_of_date\":\"2009-09-05T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":19928.566178,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":10000.000000},{\"id\":13,\"security_code\":null,\"as_of_date\":\"2009-09-06T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":23537.697185,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":3700.000000},{\"id\":14,\"security_code\":null,\"as_of_date\":\"2009-09-06T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":22074.121766,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":10000.000000},{\"id\":15,\"security_code\":null,\"as_of_date\":\"2009-09-07T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":22800.545587,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":3700.000000},{\"id\":16,\"security_code\":null,\"as_of_date\":\"2009-09-07T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":24372.931325,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":10000.000000},{\"id\":17,\"security_code\":null,\"as_of_date\":\"2009-09-08T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":22006.690019,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":3700.000000},{\"id\":18,\"security_code\":null,\"as_of_date\":\"2009-09-08T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":25292.455149,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":10000.000000},{\"id\":19,\"security_code\":null,\"as_of_date\":\"2009-09-09T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":20929.314606,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":3700.000000},{\"id\":20,\"security_code\":null,\"as_of_date\":\"2009-09-09T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":26058.725002,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":10000.000000},{\"id\":21,\"security_code\":null,\"as_of_date\":\"2009-09-10T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":23084.065432,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":3700.000000},{\"id\":22,\"security_code\":null,\"as_of_date\":\"2009-09-10T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":27897.772649,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":10000.000000},{\"id\":23,\"security_code\":null,\"as_of_date\":\"2009-09-11T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":25238.816259,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":3700.000000},{\"id\":24,\"security_code\":null,\"as_of_date\":\"2009-09-11T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":30196.582208,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":10000.000000},{\"id\":25,\"security_code\":null,\"as_of_date\":\"2009-09-12T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":26089.375796,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":3700.000000},{\"id\":26,\"security_code\":null,\"as_of_date\":\"2009-09-12T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":32495.391767,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":10000.000000},{\"id\":27,\"security_code\":null,\"as_of_date\":\"2009-09-13T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":29151.390128,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":3700.000000},{\"id\":28,\"security_code\":null,\"as_of_date\":\"2009-09-13T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":31575.867943,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":10000.000000},{\"id\":29,\"security_code\":null,\"as_of_date\":\"2009-09-14T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":28017.310746,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":3700.000000},{\"id\":30,\"security_code\":null,\"as_of_date\":\"2009-09-14T00:00:00\",\"col\":\"Nokia Corp.\",\"total local\":31422.613973,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":10000.000000},{\"id\":31,\"security_code\":\"CINDA AMC 2006ZH02 PORTFOLIO DEPOSIT\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"CINDA AMC 2006ZH02 PORTFOLIO DEPOSIT\",\"total local\":-1.346429,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Textiles\",\"quantity\":-150.000000},{\"id\":32,\"security_code\":\"FREEPORT MCMORAN 60 CALL EXP 5/19/07\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"EQOP000091\",\"total local\":470126.817191,\"AC\":\"Equity Option\",\"sector\":\"Textiles\",\"quantity\":52170.000000},{\"id\":33,\"security_code\":\"CEPU PROJECT DEPOSIT\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"CEPU PROJECT DEPOSIT\",\"total local\":86290.907034,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Financial Services\",\"quantity\":10090.000000},{\"id\":34,\"security_code\":\"FOOT LOCKER 25 CALL EXP 1/20/07\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"EQOP000023\",\"total local\":10043.685074,\"AC\":\"Equity Option\",\"sector\":\"Textiles\",\"quantity\":5000.000000},{\"id\":35,\"security_code\":\"FREEPORT MCMORAN 50 PUT EXP 1/20/07\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"EQOP000065\",\"total local\":64328.726381,\"AC\":\"Equity Option\",\"sector\":\"Textiles\",\"quantity\":5120.000000},{\"id\":36,\"security_code\":\"FREEPORT MCMORAN 95 CALL EXP 6/16/07\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"EQOP000084\",\"total local\":5.550000,\"AC\":\"Equity Option\",\"sector\":\"Textiles\",\"quantity\":0.000000},{\"id\":37,\"security_code\":\"GPI TEXTILES DEPOSIT\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"GPI TEXTILES DEPOSIT\",\"total local\":5.550000,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Oil & Gas\",\"quantity\":75000.000000},{\"id\":38,\"security_code\":\"FREEPORT MCMORAN 70 PUT EXP 5/19/07\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"EQOP000058\",\"total local\":-11948.259707,\"AC\":\"Equity Option\",\"sector\":\"Textiles\",\"quantity\":-130000.000000},{\"id\":39,\"security_code\":\"FREEPORT MCMORAN 80 CALL EXP 6/16/07\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"EQOP000085\",\"total local\":828.523822,\"AC\":\"Equity Option\",\"sector\":\"Textiles\",\"quantity\":100.000000},{\"id\":40,\"security_code\":\"FCX 115 CALLS 01/19/2008\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"EQOP000102\",\"total local\":240920.791780,\"AC\":\"Equity Option\",\"sector\":\"Textiles\",\"quantity\":20000.000000},{\"id\":41,\"security_code\":\"VIVA Preferred Shares subscriptions - Deposit\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"VIVA Preferred Shares subscriptions - Deposit\",\"total local\":-348493.979140,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Textiles\",\"quantity\":-60000.000000},{\"id\":42,\"security_code\":\"FREEPORT MCMORAN 70 CALL EXP 4/21/07\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"EQOP000093\",\"total local\":5.550000,\"AC\":\"Equity Option\",\"sector\":\"Textiles\",\"quantity\":4100000.000000},{\"id\":43,\"security_code\":\"DALIAN UNITED CREDIT GUARANTEE\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"DALIAN UNITED CREDIT GUARANTEE\",\"total local\":13274.309425,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Textiles\",\"quantity\":865802.000000},{\"id\":44,\"security_code\":\"FREEPORT MCMORAN 75 CALL EXP 5/19/07\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"EQOP000088\",\"total local\":22074.121766,\"AC\":\"Equity Option\",\"sector\":\"Textiles\",\"quantity\":5000.000000},{\"id\":45,\"security_code\":\"FREEPORT MCMORAN 60 CALL EXP 5/19/07\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"EQOP000091\",\"total local\":4675.688897,\"AC\":\"Equity Option\",\"sector\":\"Textiles\",\"quantity\":4292.000000},{\"id\":46,\"security_code\":\"FREEPORT MCMORAN 55 CALL EXP 3/17/07\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"EQOP000099\",\"total local\":408761.444649,\"AC\":\"Equity Option\",\"sector\":\"Textiles\",\"quantity\":147358.000000},{\"id\":47,\"security_code\":\"FREEPORT MCMORAN 55 CALL EXP 2/17/07\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"EQOP000100\",\"total local\":783.559107,\"AC\":\"Equity Option\",\"sector\":\"Textiles\",\"quantity\":16922.000000},{\"id\":48,\"security_code\":\"TRANSCO DEPOSIT - PHP\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"TRANSCO DEPOSIT - PHP\",\"total local\":31362.844924,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Textiles\",\"quantity\":3700.000000},{\"id\":49,\"security_code\":\"PROJECT CENTURY\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"PROJECT CENTURY\",\"total local\":5.550000,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Textiles\",\"quantity\":150.000000},{\"id\":50,\"security_code\":\"BPI 4 DEPOSIT - PHP\",\"as_of_date\":\"2009-09-15T00:00:00\",\"col\":\"BPI 4 DEPOSIT - PHP\",\"total local\":71115.392357,\"AC\":\"Auction Deposit Receivable\",\"sector\":\"Textiles\",\"quantity\":10000.000000}]}";

    //var clientSideData = "{\"Table\":[{\"id\":1,\"as_of_date\":\"2008-01-02T00:00:00\",\"fx rate\":1.000000,\"strategy_stra\":null,\"analyst\":null,\"market value local\":-37.000000,\"market value book\":-53.000000},{\"id\":2,\"as_of_date\":\"2008-01-02T00:00:00\",\"fx rate\":-1.250000,\"strategy_stra\":null,\"analyst\":null,\"market value local\":1025000.000000,\"market value book\":719273.000000},{\"id\":3,\"as_of_date\":\"2009-09-01T00:00:00\",\"fx rate\":-1.250000,\"strategy_stra\":\"EQ-VOL\",\"analyst\":\"Sanas Gupta\",\"market value local\":-37.000000,\"market value book\":-53.000000},{\"id\":4,\"as_of_date\":\"2009-09-01T00:00:00\",\"fx rate\":-1.250000,\"strategy_stra\":\"Energy\",\"analyst\":\"Tarun Jain\",\"market value local\":1025000.000000,\"market value book\":719273.000000},{\"id\":5,\"as_of_date\":\"2009-09-02T00:00:00\",\"fx rate\":-1.250000,\"strategy_stra\":\"EQ-VOL\",\"analyst\":\"Sanas Gupta\",\"market value local\":-37.000000,\"market value book\":-53.000000}]}";
    var clientSideData = clientSideDataFromGrid.replace('"', '\"');
    xlgridloader.create(id, "Test", grid, clientSideData);
}

function groupedHeaderRowPosition(scrollup, xlgridid) {
    var currentTop = $("#" + xlgridid + "_bodyDiv").offset().top + 22;
    var topElement = null;
    var i = 0;
    var groupHeaderRowCollection = $("#" + xlgridid + "_repeatedBodyDiv").find('div[grouprowid]');

    if (groupHeaderRowCollection.length > 0) {
        if (!scrollup) {
            while ($(groupHeaderRowCollection[i]).offset().top - currentTop <= 0) {
                topElement = groupHeaderRowCollection[i];
                i++;
                if (i > groupHeaderRowCollection.length - 1) {
                    break;
                }
            }
        }
        else {
            i = groupHeaderRowCollection.length - 1
            while (($(groupHeaderRowCollection[i]).offset().top) - (currentTop) >= 0) {
                i--
                topElement = groupHeaderRowCollection[i];
                if (i < 0) {
                    break;
                }
            }
        }
        if (!(scrollup && i == groupHeaderRowCollection.length - 1)) {
            $("#" + xlgridid + "_FixedHeaderRow").empty();
            $("#" + xlgridid + "_FixedHeaderRow").append($(topElement).clone());
        }
    }
}