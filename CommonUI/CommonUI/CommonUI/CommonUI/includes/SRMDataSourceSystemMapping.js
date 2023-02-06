var dataSourceToSystemWidget = {
    _commonServiceLocation: "/BaseUserControls/Service/CommonService.svc",
    _path: "",
    _pageViewModelInstance: {},
    type_id: "",
    //_tabToOpen: "",
    module_id: "",
    template_id: "",
    set_first_screen_bit:"",
    _gridViewModelInstanceDataSource: {},
    _gridViewModelInstanceSystem: {},
    isDataSourceBinded: false,
    isSystemBinded: false,
    init: function (moduleId, typeId, templateId, setFirstScreenBit) {
        //dataSourceToSystemWidget.applyTabBindings();
        dataSourceToSystemWidget.setPath();
        dataSourceToSystemWidget.setValuesFromQueryString(moduleId, typeId, templateId, setFirstScreenBit); // set moduleid, type, typeid, temp

        //dataSourceToSystemWidget._tabToOpen = selectedTabToDisplay;
        //dataSourceToSystemWidget.selectedTabToDisplay();
        dataSourceToSystemWidget.dataSourceInit();
        //        $("#SRMTabMappings").click();
    },
    setValuesFromQueryString(moduleId, typeId, templateId, setFirstScreenBit)
    {
        dataSourceToSystemWidget.module_id = moduleId;
        dataSourceToSystemWidget.type_id = typeId;
        dataSourceToSystemWidget.template_id = templateId;
        dataSourceToSystemWidget.set_first_screen_bit = setFirstScreenBit;
    },
    setPath: function () {
        var path = window.location.protocol + '//' + window.location.host;
        var pathname = window.location.pathname.split('/');

        $.each(pathname, function (ii, ee) {
            if ((ii !== 0) && (ii !== pathname.length - 1))
                path = path + '/' + ee;
        });

        dataSourceToSystemWidget._path = path;
    },
    applyTabBindings: function () {
        //$("#SRMTabAttributes").off('click').on('click', function () {
        //    dataSourceToSystemWidget.toggleIframe(dataSourceToSystemWidget._path + "/BaseUserControls/Modeler/AttributeSetupPage.aspx?module=20&EntityTypeId=" + dataSourceToSystemWidget.type_id);
        //    $("#SRMTabAttributes").addClass("SRMSelectedTabNamesHeader");
        //});
        $("#SRMTabMappings").off('click').on('click', function () {
            $("svg").show();
            $("#SRMDataSourceParent").show();
            //$("#iFrameContainer").hide();
            dataSourceToSystemWidget.dataSourceInit();
            $(".SRMTabItem").removeClass("SRMSelectedTabNamesHeader");
            $("#SRMTabMappings").addClass("SRMSelectedTabNamesHeader");
        });
        //$("#SRMTabUniqueKeys").off('click').on('click', function () {
        //    dataSourceToSystemWidget.toggleIframe(dataSourceToSystemWidget.setPathForUniqueKeys());
        //    $("#SRMTabUniqueKeys").addClass("SRMSelectedTabNamesHeader");
        //});
        //$("#SRMTabRules").off('click').on('click', function () {
        //    dataSourceToSystemWidget.toggleIframe("http://localhost:59522/CommonUI/SRMRuleCatalogParent.aspx?identifier=SRM_RuleCatalog&specificModuleId=20");
        //    $("#SRMTabRules").addClass("SRMSelectedTabNamesHeader");
        //});
    },
    //selectedTabToDisplay: function () {

    //    switch (dataSourceToSystemWidget._tabToOpen) {
    //        case "attributes": $("#SRMTabAttributes").click(); break;
    //        case "mappings": $("#SRMTabMappings").click(); break;
    //        case "uniquekeys": $("#SRMTabUniqueKeys").click(); break;
    //        case "rules": $("#SRMTabRules").click(); break;
    //        default: $("#SRMTabMappings").click(); break;
    //    }


    //},
    //toggleIframe: function (source) {
    //    $("#SRMDataSourceParent").hide();
    //    $("svg").hide();
    //    $("#iFrameContainer").show();
    //    dataSourceToSystemWidget.resizeIframe();
    //    $("#InnerFrameId1").attr('src', source);
    //    $(".SRMTabItem").removeClass("SRMSelectedTabNamesHeader");
    //},
    //resizeIframe: function () {
    //    var iframeHeight, width;
    //    iframeHeight = $(window).height() - $("#SRMTabNamesHeader").height() - 10;// - iframeSubtractHeight[tabId];
    //    width = $(window).width();
    //    //$("#InnerFrameId1").height(iframeHeight).width(width);
    //}
    dataSourceInit: function () {
        dataSourceToSystemWidget.setPath();
        //dataSourceToSystemWidget.getTypeId();
        dataSourceToSystemWidget.getDataSourceAndSystemInfo();
        //$("#SRMDataSourceAndSystemMappingGrid").hide();
    },
    setScreenSize: function () {
        //$("#SRMDataSourceAndSystemMappingParent").width($(window).width() - 100);
        //$("#SRMDataSourceAndSystemMappingParent").css('margin-top', (($(window).height() - $("#SRMDataSourceAndSystemMappingParent").height()) / 3)-120);
        $("#SRMDataSourceAndSystemMappingContainer").css('height', ($(window).height()) - 12 - 3);
        $("#mapping_data_grid").css('height', ($(window).height()) - 12 - 3);
    

        $(".srmDataSourceSystemMapping_upstreamcontainer").css('height', $("#SRMDataSourceAndSystemMappingContainer").height());
      
        //up_block_margintop = (($(window).height() - $("#srmDataSourceSystemMapping_upstream").outerHeight()) / 2);
        if ($("#srmDataSourceSystemMapping_upstream").outerHeight() > $(window).height()) {
            $(".horizontal_align_test_up").css('margin-top','30.5px');

        }
        else {
            $(".horizontal_align_test_up").css('margin-top', ($("#SRMDataSourceAndSystemMappingContainer").height() - $("#srmDataSourceSystemMapping_upstream").outerHeight()) / 2);
        }
        if ($("#srmDataSourceSystemMapping_downstream").outerHeight() > $(window).height()) {
            $(".horizontal_align_test_down").css('margin-top', '30.5px');

        }
        else {
            $(".horizontal_align_test_down").css('margin-top', ($("#SRMDataSourceAndSystemMappingContainer").height() - $("#srmDataSourceSystemMapping_downstream").outerHeight()) / 2);
        }
        $("#srmDataSourceSystemMapping_entity_type").css('margin-top', ($("#SRMDataSourceAndSystemMappingContainer").height() - $("#srmDataSourceSystemMapping_entity_type").outerHeight()) / 2);
        $("#feedTable").css('height', $("#SRMDataSourceAndSystemMappingContainer").height() - 50);
        $("#reportTable").css('height', $("#SRMDataSourceAndSystemMappingContainer").height() - 50);
        $(".srmdatasourcesystemmapping_arrow").css('margin-top', $("#srmDataSourceSystemMapping_entity_type").outerHeight() / 2 - 10);

        if ($("#SRMDataSourceAndSystemMappingLeftPanel").outerHeight() > $("#SRMDataSourceAndSystemMappingContainer").height() - 70)
      
        {
            $("#srmdatasourcesystemmapping_scrolldiv").css('height', $("#SRMDataSourceAndSystemMappingContainer").height() - $("#srmdatasourcesystemmapping_scrollbutton_div").height()-
                $("#srmDataSourceSystemMapping_upstream_heading").height()-40);

            $("#srmdatasourcesystemmapping_scrolldiv").css('overflow-y', 'auto');
            $("#srmdatasourcesystemmapping_scrollbutton_div").css('display', 'block');

        }
        if ($("#SRMDataSourceAndSystemMappingRightPanel").outerHeight() > $("#SRMDataSourceAndSystemMappingContainer").height() - 70) {

            $("#srmdatasourcesystemmapping_down_scrolldiv").css('height', $("#SRMDataSourceAndSystemMappingContainer").height() - $("#srmdatasourcesystemmapping_down_scrollbutton_div").height() -
               $("#srmDataSourceSystemMapping_down_heading").height() - 40);

             $("#srmdatasourcesystemmapping_down_scrolldiv").css('overflow-y', 'auto');
             $("#srmdatasourcesystemmapping_down_scrollbutton_div").css('display', 'block');
        }
       // $("#srmdatasourcesystemmapping_scrolldiv").css('height', $("#SRMDataSourceAndSystemMappingContainer").height() - 70);
       // $("#srmdatasourcesystemmapping_down_scrolldiv").css('height', $("#SRMDataSourceAndSystemMappingContainer").height() - 70);


        //$("SRMDataSourceType").css('height',20%);
        //$(".srmdatasourcesystemmapping_arrow").css('margin-top', (up_block_margintop + $("#srmDataSourceSystemMapping_upstream").height() / 2));

       // $("#entity_type").css('margin-top', (($(window).height()) / 2));
    },
    //getTypeId: function () {
    //    dataSourceToSystemWidget.type_id = 661;
    //},
//    getSecTypeId: function(){
//        dataSourceToSystemWidget._sec_type_id=17;
//},
    setPath: function () {
        var path = window.location.protocol + '//' + window.location.host;
        var pathname = window.location.pathname.split('/');

        $.each(pathname, function (ii, ee) {
            if ((ii !== 0) && (ii !== pathname.length - 1))
                path = path + '/' + ee;
        });

        dataSourceToSystemWidget._path = path;
    },
    setFirstGrid: function (setFirstScreenBit) {
        if(setFirstScreenBit==0)
        {
            if ($(".SRMDataSourceName").length > 0)
            { $(".SRMDataSourceName").first().click(); }

        }
        else {
            if ($(".SRMSystemName ").length > 0)
            { $(".SRMSystemName ").first().click(); }

        }
        //var lastcell= 
        //$(".row_container").last().css('padding-bottom','10px');
        //$(".row_container").last().css('border-bottom', '5px solid #d6e0e7');
        //$(".row_container").last().css('border-bottom-left-radius', '6px');
        //$(".row_container").last().css('border-bottom-right-radius', '6px');
    }
    ,
    getDataSourceAndSystemInfo: function () {
        $.ajax({
            type: 'POST',
            url: dataSourceToSystemWidget._path + dataSourceToSystemWidget._commonServiceLocation + "/GetDataSourceAndSystemMappingInfo",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ type_id: dataSourceToSystemWidget.type_id,module_id: dataSourceToSystemWidget.module_id }),
            success: function (data) {
                if (data != null) {
                    dataSourceToSystemWidget.onSuccessDataSourceAndSystemInfo(data.d);
                    ko.cleanNode($("#SRMDataSourceAndSystemMappingParent"));
                    if (!ko.dataFor(document.getElementById("SRMDataSourceAndSystemMappingParent"))) {
                        ko.applyBindings(dataSourceToSystemWidget._pageViewModelInstance, document.getElementById("SRMDataSourceAndSystemMappingParent"));
                        dataSourceToSystemWidget.setScreenSize();
                        //dataSourceToSystemWidget.applyJSPlumb();
                        if (data.d.dataSourceInfoWidgetList.length == 0 && data.d.systemInfoWidgetList.length == 0)
                        {
                            dataSourceToSystemWidget.setNoDataHandling();
                            dataSourceToSystemWidget.setCAReportSystemHandling();

                        }
                        else if (data.d.dataSourceInfoWidgetList.length == 0)
                        {
                            dataSourceToSystemWidget.setDataSourceHandling();
                            dataSourceToSystemWidget.applyReportSystemBindings();
                            dataSourceToSystemWidget.set_first_screen_bit = 1;
                        }
                        else if (dataSourceToSystemWidget.module_id == 9 || data.d.systemInfoWidgetList.length == 0) 
                        {
                            dataSourceToSystemWidget.setCAReportSystemHandling();
                            dataSourceToSystemWidget.applyDataSourceClickBindings();
                         }

                        else {
                            dataSourceToSystemWidget.applyDataSourceClickBindings();
                            dataSourceToSystemWidget.applyReportSystemBindings();
                        }
                        dataSourceToSystemWidget.setFirstGrid(dataSourceToSystemWidget.set_first_screen_bit);
                        //click functionj
                    }
                }

            },
            error: function (m) {
                console.log("Data cannot be fetched");
            }
        });
    },
    setNoDataHandling()
    {
        dataSourceToSystemWidget.setDataSourceHandling();
        $("#mapping_data_grid").css('display', 'none');
        $(".srmdatasourcesystemmap_parent").css('width', '99.8%');
        $(".semdatasourceSystem_mapping_parent_div").css('height', $(window).height() - 7);
    }
    ,
    setNoGridDataHandling()
    {
        $("#mapping_data_grid").css('display', 'none');
        $(".srmdatasourcesystemmap_parent").css('width', '99.8%');
        $(".semdatasourceSystem_mapping_parent_div").css('height', $(window).height() - 7);
    },

    onSuccessDataSourceAndSystemInfo: function (data) {
        dataSourceToSystemWidget._pageViewModelInstance = new dataSourceToSystemWidget.pageViewModel(data);

    },
    setCAReportSystemHandling() {
        $("#srmDataSourceSystemMapping_downstream_label").css('display', 'block');
        //$("#SRMDataSourceEntityName").css('line-height', '0');
        $("#srmDataSourceSystemMapping_downstream_label").text("NO REPORTS CONFIGURED");
    },

    setDataSourceHandling() {
        //$("#SRMDataSourceAndMappingEntityType1").text($(".SRMDataSourceEntityName").text());
        //$("#SRMDataSourceAndMappingDataSource").text($(event.target).text());

        $("#srmDataSourceSystemMapping_upstream_label").css('display', 'block');
        //$("#SRMDataSourceEntityName").css('line-height', '0');
        $("#srmDataSourceSystemMapping_upstream_label").text("NO DATA SOURCE CONFIGURED");
    },
    pageViewModel: function (data) {
        var self = this;
        self.dataSourceList = ko.observableArray();
        self.systemList = ko.observableArray();
        self.entityTypeDisplayName = data.entityTypeDisplayName;
      //  for (var i = 0; i < 20; i++) {
            for (var item in data.dataSourceInfoWidgetList)
                self.dataSourceList.push(new dataSourceToSystemWidget.dataSourceChain(data.dataSourceInfoWidgetList[item]));

            for (var item in data.systemInfoWidgetList)
                self.systemList.push(new dataSourceToSystemWidget.systemInfoChain(data.systemInfoWidgetList[item]));
       // }
    },
    dataSourceChain: function (data) {
        var self = this;
        self.dataSourceName = data.dataSourceName;
        self.dataSourceId = data.dataSourceId;
    },
    systemInfoChain: function (data) {
        var self = this;
        self.reportTypeId = data.reportTypeId;
        self.systemInfoName = data.systemInfoName;
        self.systemInfoId = data.systemInfoId;
    },
    applyJSPlumb: function () {
        jsPlumb.ready(function () {
            /*First Instance*/
            var firstInstance = jsPlumb.getInstance();
            firstInstance.importDefaults({
                Connector: ["Straight"],
                Anchors: ["Right", "Left"],
                ConnectionsDetachable: false,
                PaintStyle: { strokeWidth: 3, stroke: '#d1d1d1' },
                Endpoint: ["Dot", {
                    radius: 2
                }],
                Overlays: ["Arrow"],
                //ConnectorOverlays: [ ["Arrow", { width: 1, length: 3, location: 1, id: "arrow" }]  ],
                EndpointStyle: { strokeWidth: 1, stroke: '#d1d1d1', fill: '#d1d1d1' }                
            });



            for (var i in dataSourceToSystemWidget._pageViewModelInstance.dataSourceList()) {
                var name = "dataSourceItem_" + dataSourceToSystemWidget._pageViewModelInstance.dataSourceList()[i].dataSourceName;
                firstInstance.connect({
                    source: name,
                    target: "SRMDataSourceType",
                    scope: "someScope"
                });
            }




            /*Second Instance*/
            var secondInstance = jsPlumb.getInstance();
            secondInstance.importDefaults({
                Connector: ["Straight"],
                Anchors: ["Right", "Left"],
                PaintStyle: { strokeWidth: 3, stroke: '#d1d1d1' },
                Endpoint: ["Dot", {
                    radius: 2
                }],
                Overlays: ["Arrow"],
                EndpointStyle: { strokeWidth: 1, stroke: '#d1d1d1', fill: '#d1d1d1' }

            });


            for (var i in dataSourceToSystemWidget._pageViewModelInstance.systemList()) {
                var name = "systemItem_" + dataSourceToSystemWidget._pageViewModelInstance.systemList()[i].systemInfoName;
                secondInstance.connect({
                    source: "SRMDataSourceType",
                    target: name,
                    scope: "someScope"
                });
            }

        });

    },
    applyDataSourceClickBindings: function () {
        $(".SRMDataSourceName").off('click').on('click', function (event) {
            //console.log($(event.target).attr("dataSourceId") + "_" + dataSourceToSystemWidget.type_id);

            //set entity type name and data source
            $("#SRMDataSourceAndMappingEntityType1").text($(".SRMDataSourceEntityName").text());
            $("#SRMDataSourceAndMappingDataSource").text($(event.target).text());
            dataSourceToSystemWidget.getDataSourceGridData($(event.target).attr("dataSourceId"));
            $(".SRMSelectedText").removeClass('SRMSelectedText');
            $(event.target).addClass('SRMSelectedText');
            //dataSourceToSystemWidget.animateDiv();
            
        });
        $("#srmDataSourceSystemMapping_scrollup_button").off('click').on('click', function () {
            $('#srmdatasourcesystemmapping_scrolldiv').scrollTop($('#srmdatasourcesystemmapping_scrolldiv').scrollTop() - 40);
        });
        $("#srmDataSourceSystemMapping_scrolldown_button").off('click').on('click', function () {
            $('#srmdatasourcesystemmapping_scrolldiv').scrollTop($('#srmdatasourcesystemmapping_scrolldiv').scrollTop() + 40);
        });

        $("#srmDataSourceSystemMapping_down_scrollup_button").off('click').on('click', function () {
            $('#srmdatasourcesystemmapping_down_scrolldiv').scrollTop($('#srmdatasourcesystemmapping_down_scrolldiv').scrollTop() - 40);
        });
        $("#srmDataSourceSystemMapping_down_scrolldown_button").off('click').on('click', function () {
            $('#srmdatasourcesystemmapping_down_scrolldiv').scrollTop($('#srmdatasourcesystemmapping_down_scrolldiv').scrollTop() + 40);
        });


    },
    applyReportSystemBindings: function () {
        $(".SRMSystemItem").off('click').on('click', function (event) {
            //console.log($(event.target).attr("systemInfoId") + "_" + dataSourceToSystemWidget.type_id);

            //set entity type name and report system
            $("#SRMDataSourceAndMappingEntityType2").text($(".SRMDataSourceEntityName").text());
            $("#SRMDataSourceAndMappingReportSystem").text($(event.target).text());
          
            dataSourceToSystemWidget.getSystemGridData($(event.target).attr("systemInfoId"), $(event.target).attr("reportTypeId"));
            //dataSourceToSystemWidget.animateDiv();
            $(".SRMSelectedText").removeClass('SRMSelectedText');
            $(event.target).addClass('SRMSelectedText');
        });
    },
    getDataSourceGridData: function (dataSourceId) {
        var params = {};
        params.type_id = parseInt(dataSourceToSystemWidget.type_id);
        params.data_source_id = parseInt(dataSourceId);
        params.module_id = parseInt(dataSourceToSystemWidget.module_id);
        params.template_id = parseInt(dataSourceToSystemWidget.template_id);
        CallCommonServiceMethod('GetDataSourceTable_Slider', params, dataSourceToSystemWidget.OnSuccess_GetDataDataSource, dataSourceToSystemWidget.OnError, null, false);
    },
    getSystemGridData: function (systemInfoId,reportTypeId) {
        var params = {};
        params.type_id = parseInt(dataSourceToSystemWidget.type_id);
        params.report_system_id = parseInt(systemInfoId);
        params.module_id = parseInt(dataSourceToSystemWidget.module_id);
        params.report_type_id = parseInt(reportTypeId);
        params.template_id = parseInt(dataSourceToSystemWidget.template_id);
        CallCommonServiceMethod('GetReportData', params, dataSourceToSystemWidget.OnSuccess_GetDataSystem, dataSourceToSystemWidget.OnError, null, false);
    },
    OnSuccess_GetDataDataSource: function(result){
        var data = JSON.parse(result.d);
        
        //isDataSourceBinded: false,
        //isSystemBinded: false,
        if (data.length == 0) {
            dataSourceToSystemWidget.setNoGridDataHandling();

        }
        else {
            $("#feed").css('display', 'block');
            $("#report").css('display', 'none');
            if (!dataSourceToSystemWidget.isDataSourceBinded) {
                dataSourceToSystemWidget.isDataSourceBinded = true;
                dataSourceToSystemWidget._gridViewModelInstanceDataSource = new dataSourceToSystemWidget.DataViewModel(data);
                ko.applyBindings(dataSourceToSystemWidget._gridViewModelInstanceDataSource, document.getElementById("feed"));
            }
            else {
                dataSourceToSystemWidget._gridViewModelInstanceDataSource.DataList.removeAll();
                //            dataSourceToSystemWidget._gridViewModelInstanceDataSource.push()
                for (var item in data) {
                    dataSourceToSystemWidget._gridViewModelInstanceDataSource.DataList.push(new dataSourceToSystemWidget.chainViewModel(data[item]));
                }
            }
        }
    },
    OnSuccess_GetDataSystem: function (result) {
        var data = JSON.parse(result.d);
        
        $("#feed").css('display', 'none');
        $("#report").css('display', 'block');

        if (!dataSourceToSystemWidget.isSystemBinded)
        {    dataSourceToSystemWidget.isSystemBinded = true;
            dataSourceToSystemWidget._gridViewModelInstanceSystem = new dataSourceToSystemWidget.DataViewModel_report(data);
            ko.applyBindings(dataSourceToSystemWidget._gridViewModelInstanceSystem, document.getElementById("report"));

        }
        else {
            dataSourceToSystemWidget._gridViewModelInstanceSystem.DataList_report.removeAll();
            if ($(".SRMSelectedText").attr('reportTypeId') == 2 || $(".SRMSelectedText").attr('reportTypeId') == 5) {
                $("#SRMDataSourceAndMapping_report").css('display', 'none');
                $("#SRMDataSourceAndMapping_report_case").css('display', 'block');
                for (var item in data) {
                    dataSourceToSystemWidget._gridViewModelInstanceSystem.DataList_report.push(new dataSourceToSystemWidget.chainViewModel_report_case2(data[item]));
                }
            }
            else {
                $("#SRMDataSourceAndMapping_report").css('display', 'block');
                $("#SRMDataSourceAndMapping_report_case").css('display', 'none');
                for (var item in data) {
                    dataSourceToSystemWidget._gridViewModelInstanceSystem.DataList_report.push(new dataSourceToSystemWidget.chainViewModel_report(data[item]));
                }
            }
        }
    },
    OnError:function(){
        console.log("Error");
    },
    DataViewModel: function(data) {
        var self = this;
        self.DataList = ko.observableArray();
        for (var item in data) {
            self.DataList.push(new dataSourceToSystemWidget.chainViewModel(data[item]));
        }
    },

    chainViewModel: function (data) {
        var self = this;
        self.Feed_Name = data.Feed_Name;
        self.Field_Name = data.Field_Name;
        self.Attribute_Name = data.Attribute_Name;
    },
    DataViewModel_report: function (data) {
        var self = this;
        self.DataList_report = ko.observableArray();
        if ($(".SRMSelectedText").attr('reportTypeId') == 2 || $(".SRMSelectedText").attr('reportTypeId') == 5) {
            for (var item in data) {
                self.DataList_report.push(new dataSourceToSystemWidget.chainViewModel_report_case2(data[item]));
            }
        }
        else {
            for (var item in data) {
                self.DataList_report.push(new dataSourceToSystemWidget.chainViewModel_report(data[item]));
            }
        }
    },
    chainViewModel_report: function (data) {
        var self = this;
        self.Report_Name = data["Report Name"];
        self.Attribute_Name = data["Attribute Name"];
        self.Report_Attribute_Name = data["Report Attribute Name"];
    },
    chainViewModel_report_case2: function (data) {
        var self = this;
        self.Report_Name = data["Report Name"];
        self.Attribute_Name = data["Attribute Name"];
    },
    animateDiv: function () {
        $("#SRMDataSourceAndSystemMappingGrid").show();

        $("#SRMDataSourceAndSystemMappingGrid").animate({
            left: '400px', width: $(window).width() - 420, height: $(window).height() - 30
        }, '500',
        function () {
            $("#SRMDataSourceAndSystemMappingGrid").css('background', 'white');
            $("svg").hide();

            //close button binding
            $("#closeDataSourceAndSystemMappingGrid").off('click').on('click', function () {


                $("#SRMDataSourceAndSystemMappingGrid").animate({
                    width: 0, left: '400px'
                }, '500', function () {
                    $("svg").show();
                    $("#SRMDataSourceAndSystemMappingGrid").css('display', 'none');
                });
                
            })
        });
    },

    setPathForUniqueKeys: function () {
        return dataSourceToSystemWidget._path + "RMHomeInternal.aspx?identifier=RefM_UniqueKeySetup&entityTypeId=" + type_id + "&ModuleId=20";
    }
}

function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
    callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
}




