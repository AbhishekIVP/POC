var srmDqmSM = function () {
    var sm1 = '';
    function DqmSM() { }
    
    DqmSM.prototype.manageTaskClickEvent = function (chainId,flowId,date,chartContainerID, obj) {
        console.log('sm executed');
        var prefixDivIds = "srmDqmSM";      //for prefixing div IDs

        var chartObj = $('#' + chartContainerID);


        if (obj.taskType == "Loading") {
            //All the HTML div IDs
            
            var chartDivId1 = prefixDivIds + obj.taskType + "Chart" + GetGUID();
            var chartDivId2 = prefixDivIds + obj.taskType + "Chart" + GetGUID();
            var chartDivId3 = prefixDivIds + obj.taskType + "Chart" + GetGUID();
            var fileFormatDivId = prefixDivIds + obj.taskType + "FileFormat";
            var timeTakenDivId = prefixDivIds + obj.taskType + "TimeTaken";         //should have linked this to the graph
            var fileSizeDivId = prefixDivIds + obj.taskType + "FileSize";          
            var tabId = prefixDivIds + obj.taskType + "Tabs";
            var recordProcessed = prefixDivIds + obj.taskType + "RecordProcessed";      //TODO

            //array containing list of tabs
            var tabsLoadingTaskArray = [];
            for (var item in taskLoadingLineGraphDataJsonArray.Tabs) {
                tabsLoadingTaskArray.push(taskLoadingLineGraphDataJsonArray.Tabs[item].name);
                console.log(tabsLoadingTaskArray);
            }

            //console.log(tabsLoadingTaskArray)

            //section 1
            chartObj.html('<div class=fileFormat id = ' + fileFormatDivId + '></div><div class=timeTaken id = ' + timeTakenDivId + '></div><div id=' + chartDivId1 + '></div>');

            //section 2
            chartObj.append('<div id = ' + tabId + '></div><div class=fileSize id = ' + fileSizeDivId + '></div><div id = '+ chartDivId2 + '></div>');
    
            //section 3
            chartObj.append('<div class=recordProcessed id = ' + recordProcessed + '></div><div id = ' + chartDivId3 + '></div>');

            
            DqmSM.prototype.bindFileAndTimeChart(fileFormatDivId, timeTakenDivId);

            DqmSM.prototype.bindLineChart(chartDivId1, taskLoadingChartConfigJson.Title, taskLoadingChartConfigJson.SubTitle, taskLoadingChartConfigJson.YAxisText, taskLoadingChartConfigJson.LegendLayout, taskLoadingChartConfigJson.LegendAlign, taskLoadingChartConfigJson.LegendVerticalAlign, taskLoadingChartConfigJson.PlotStartPoint, taskLoadingLineGraphDataJson.SeriesData);
            console.log(chartDivId1);
            DqmSM.prototype.bindTabs(tabId, tabsLoadingTaskArray, chartDivId2, fileSizeDivId,taskLoadingLineGraphDataJsonArray, taskLoadingChartConfigJson);
            //calls to bind the second chart inside
            $('#'+ recordProcessed).html('<b>Record Processed</b>');
            DqmSM.prototype.bindStackedChart(chartDivId3, taskLoadingStackedChartConfigJson.Title, taskLoadingStackedChartConfigJson.Subtitle, taskLoadingStackedChartConfigJson.YAxisText, taskLoadingStackedChartConfigJson.LegendLayout, taskLoadingStackedChartConfigJson.LegendAlign, taskLoadingStackedChartConfigJson.LegendVerticalAlign, taskLoadingStackedChartConfigJson.PlotStartPoint, taskLoadingColumnGraphDataJson.SeriesData);
        }

        else if (obj.taskType == "Update Core") {
            var taskType = "UpdateCore";    //used because Chart having issue reading spaced divIDs
            //chart Container IDs
            var chartDivId1 = prefixDivIds + taskType + "Chart" + GetGUID();
            var chartDivId2 = prefixDivIds + taskType + "Chart" + GetGUID();
            var chartDivId3 = prefixDivIds + taskType + "Chart" + GetGUID();
            var chartDivId4 = prefixDivIds + taskType + "Chart" + GetGUID();

            var prioritisationDivId = prefixDivIds + taskType + "Prioritisation";
            var feedMappingDivId = prefixDivIds + taskType + "FeedMapping";
            var timeTakenDivId = prefixDivIds + taskType + "TimeTaken";
            var securitiesUpdatedDivId = prefixDivIds + taskType + "SecuritiesUpdated";
            var exceptionsGeneratedDivId = prefixDivIds + taskType + "ExceptionsGenerated";
            
            var tabId = prefixDivIds + taskType + "Tabs";  //For Tabs


            //section 1

            chartObj.html('<div class = "parentDivPrioriandFM"> <div class="left" id= ' + prioritisationDivId + '></div><div class ="right" id=' + feedMappingDivId + '></div></div>');
            chartObj.append('<div class=timeTaken id = ' + timeTakenDivId + '></div>');
            chartObj.append('<div id=' + chartDivId1 + '></div>');
            chartObj.append('<div class=timeTaken id = ' + securitiesUpdatedDivId + '></div>');
            chartObj.append('<div id=' + chartDivId2 + '></div>');
            chartObj.append('<div class=timeTaken id = ' + exceptionsGeneratedDivId + '></div>');
            chartObj.append('<div id=' + chartDivId3 + '></div>');
            chartObj.append('<div id=' + tabId + '></div>');
            chartObj.append('<div id=' + chartDivId4 + '></div>');

            //in config of JSON
            $('#' + prioritisationDivId).html('<b>Prioritisation Updated</b> : ' + taskUpdateCoreChartConfigJson.PrioritisationUpdated);
            $('#' + feedMappingDivId).html('<b>Feed Mapping Updated</b> : ' + taskUpdateCoreChartConfigJson.FeedMappingUpdated);

            //in data
            
            $('#' + timeTakenDivId).html('<b>' + taskUpdateCoreLineGraph1DataJson.info + '</b> : ' + taskUpdateCoreLineGraph1DataJson.SubInfo1 + ' <span class="downUpInFields">' + taskUpdateCoreLineGraph1DataJson.SubInfo2 + '</span');
            $('#' + securitiesUpdatedDivId).html('<b>' + taskUpdateCoreLineGraph2DataJson.info + '</b> : ' + taskUpdateCoreLineGraph2DataJson.SubInfo1 + ' <span class="downUpInFields">' + taskUpdateCoreLineGraph2DataJson.SubInfo2 + '</span');
            $('#' + exceptionsGeneratedDivId).html('<b>' + columnExceptionGraphDataJson.info + '</b> : ' + columnExceptionGraphDataJson.SubInfo1 + ' <span class="downUpInFields">' + columnExceptionGraphDataJson.SubInfo2 + '</span');

            //array containing list of tabs
            var tabsUpdateCoreTaskArray = [];
            for (var item in taskUpdateCoreLineGraphDataJsonArray.Tabs) {
                tabsUpdateCoreTaskArray.push(taskUpdateCoreLineGraphDataJsonArray.Tabs[item].name);
                console.log(tabsUpdateCoreTaskArray);
            }

            DqmSM.prototype.bindLineChart(chartDivId1, taskLoadingChartConfigJson.Title, taskLoadingChartConfigJson.SubTitle, taskLoadingChartConfigJson.YAxisText, taskLoadingChartConfigJson.LegendLayout, taskLoadingChartConfigJson.LegendAlign, taskLoadingChartConfigJson.LegendVerticalAlign, taskLoadingChartConfigJson.PlotStartPoint, taskLoadingLineGraphDataJson.SeriesData);

            DqmSM.prototype.bindLineChart(chartDivId2, taskLoadingChartConfigJson.Title, taskLoadingChartConfigJson.SubTitle, taskLoadingChartConfigJson.YAxisText, taskLoadingChartConfigJson.LegendLayout, taskLoadingChartConfigJson.LegendAlign, taskLoadingChartConfigJson.LegendVerticalAlign, taskLoadingChartConfigJson.PlotStartPoint, taskLoadingLineGraphDataJson.SeriesData);
            DqmSM.prototype.bindExceptionsColumnChart(chartDivId3, 1, columnExceptionGraphDataJson.SeriesData);
            DqmSM.prototype.bindTabsUC(tabId, tabsUpdateCoreTaskArray, chartDivId4, taskUpdateCoreLineGraphDataJsonArray, taskUpdateCoreTabsGraphConfigJson);
            
        }
    };


    //needs to be made generic
    DqmSM.prototype.bindTabsUC = function (tabId, tabsLoadingTaskArray, chartDivId2, taskData, taskConfig) {
        var tabsObj = $('#' + tabId);
        tabsObj.append('<ul class="tabListContainer">');


        for (var i in tabsLoadingTaskArray) {
            tabsObj.append('<li class="tabLinks">' + tabsLoadingTaskArray[i] + '</li>');
        }
        tabsObj.append('</ul>');

        //$(".tabLinks")[0].click();

        //Display the graph when one of the options is clicked

        $(".tabLinks").bind("click", function () {
            for (var i in tabsLoadingTaskArray) {
                console.log($(this).text());
                if ($(this).text() == tabsLoadingTaskArray[i]) {
                    $(".tabLinks").removeClass('tabSelected');
                    $(this).addClass('tabSelected');
                    DqmSM.prototype.bindTabChartUC(i, chartDivId2, taskData, taskConfig);
                    break;
                }
            }

        });
        $("li").css("list-style-type", "none");
        $("li").css("display", "inline-block");
        $(".tabLinks")[0].click();
    }

    DqmSM.prototype.bindTabChartUC = function (positionInJsonArray, chartDivId, taskData, taskConfig) {
        var currentData = taskData.Tabs[positionInJsonArray];
        DqmSM.prototype.bindLineChartUC(chartDivId, taskConfig.Title, taskConfig.SubTitle, taskConfig.YAxisText, taskConfig.LegendLayout, taskConfig.LegendAlign, taskConfig.LegendVerticalAlign, taskConfig.PlotStartPoint, currentData.data.SeriesData);

    }


    
    DqmSM.prototype.bindTabs = function (tabId, tabsLoadingTaskArray, chartDivId2, fileSizeDivId, taskData, taskConfig) {
        var tabsObj = $('#' + tabId);
        var abc = "";
        abc += '<div class="tabListContainer">';
        for (var i in tabsLoadingTaskArray) {
            abc = abc + '<div class="tabLinks">' + tabsLoadingTaskArray[i] + '</div>';
        }
        abc += '</div>';

        tabsObj.html(abc);

        var fileSizeObj = $('#' + fileSizeDivId);
        var fileSizeContainer = "FileSizeContainer";
        fileSizeObj.html('<div id=' + fileSizeContainer + '></div>');


        $(".tabLinks").bind("click", function () {
            for (var i in tabsLoadingTaskArray) {
                //console.log($(this).text());
                if ($(this).text() == tabsLoadingTaskArray[i]) {
                    $(".tabLinks").removeClass('tabSelected');      //removes all classes
                    $(this).addClass('tabSelected');
                    DqmSM.prototype.bindTabChart(i, chartDivId2, fileSizeContainer, taskData, taskConfig);
                    break;
                }
            }

        });
  
        $(".tabLinks")[0].click();          //select the first option
    
    }

    DqmSM.prototype.bindTabChart = function (positionInJsonArray, chartDivId, fileSizeDivId , taskData, taskConfig) {
        var fileSizeObj = $('#' + fileSizeDivId);
        var currentData = taskData.Tabs[positionInJsonArray];
        
        //should link it another function
        fileSizeObj.html('<b>' + currentData.data.fileSizeName + '</b> : ' + currentData.data.fileSize + '<span class=downUpInFields> (' + currentData.data.fileSizeStatus + ' ' + currentData.data.fileSizePerc + ')</span>');
       
        console.log(taskConfig);
        console.log(currentData.data.SeriesData);
        DqmSM.prototype.bindLineChart(chartDivId, taskConfig.Title, taskConfig.SubTitle, taskConfig.YAxisText, taskConfig.LegendLayout, taskConfig.LegendAlign, taskConfig.LegendVerticalAlign, taskConfig.PlotStartPoint, currentData.data.SeriesData);
        
    }
    
    DqmSM.prototype.bindFileAndTimeChart = function (fileFormatDivId, timeTakenDivId) {
        //console.log(taskLoadingLineGraphDataJson.fileName + ':' + taskLoadingLineGraphDataJson.fileType);
        $("#" + fileFormatDivId).html('<b>' + taskLoadingLineGraphDataJson.fileName + ' : </b>' + taskLoadingLineGraphDataJson.fileType);
        $("#" + timeTakenDivId).html('<b>' + taskLoadingLineGraphDataJson.TotalTimeTaken + ' : </b>' + taskLoadingLineGraphDataJson.Info1 + '<span class="downUpInFields"> (' + taskLoadingLineGraphDataJson.SubInfo1 + ')</span>');
    }

    
    DqmSM.prototype.bindLineChartUC = function (containerID, title, subtitle, yAxisText, legendLayout, legendAlign, legendVerticalAlign, plotStartPoint, seriesData) {
        Highcharts.chart(containerID, {

            title: {
                text: 'Solar Employment Growth by Sector, 2010-2016'
            },

            subtitle: {
                text: 'Source: thesolarfoundation.com'
            },

            yAxis: {
                title: {
                    text: 'Number of Employees'
                }
            },
            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'middle'
            },

            plotOptions: {
                series: {
                    pointStart: 2010
                }
            },
            series: [{
                name: 'Installation',
                data: [43934, 52503, 57177, 69658, 97031, 119931, 137133, 154175]
            }, {
                name: 'Manufacturing',
                data: [24916, 24064, 29742, 29851, 32490, 30282, 38121, 40434]
            }, {
                name: 'Sales & Distribution',
                data: [11744, 17722, 16005, 19771, 20185, 24377, 32147, 39387]
            }, {
                name: 'Project Development',
                data: [null, null, 7988, 12169, 15112, 22452, 34400, 34227]
            }, {
                name: 'Other',
                data: [12908, 5948, 8105, 11248, 8989, 11816, 18274, 18111]
            }]
            
        });
        

    }

    

    DqmSM.prototype.bindLineChart = function (containerID, title, subtitle, yAxisText, legendLayout, legendAlign, legendVerticalAlign, plotStartPoint, seriesData) {
        Highcharts.chart(containerID, {
            chart: {
                type: 'spline'
            },
            title: {
                text: title
            },
            subtitle: {
                text: subtitle
            },
            xAxis: {
                type: 'datetime',
                dateTimeLabelFormats: { // don't display the dummy year
                    month: '%e. %b',
                    year: '%b'
                },
                title: {
                    text: 'Date'
                }
            },
            yAxis: {
                title: {
                    text: yAxisText
                }
            },
            tooltip: {
                headerFormat: '<b>{series.name}</b><br>',
                pointFormat: '{point.x:%e. %b}: {point.y:.2f} m'
            },
            exporting: {
                enabled: false
            },
            credits: false,
            //        legend: {
            //            enabled: false
            //        },
            scrollbar: {
                enabled: true
            },
            legend: {
                layout: legendLayout,
                align: legendAlign,
                verticalAlign: legendVerticalAlign
            },
            plotOptions: {
                series: {
                    pointStart: parseInt(plotStartPoint)
                }
            },
            series: seriesData
        });
    }

    DqmSM.prototype.bindStackedChart = function (containerID, title, subtitle, yAxisText, legendLayout, legendAlign, legendVerticalAlign, plotStartPoint, seriesData) {
        Highcharts.chart(containerID, {
            chart: {
                type: 'column'
            },
            title: {
                text: title
            },
            subtitle: {
                text: subtitle
            },
            xAxis: {
                categories: ['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas']
            },
            yAxis: {
                title: {
                    text: yAxisText
                }
            },
                    
            scrollbar: {
                enabled: true
            },
            legend: {
                layout: legendLayout,
                align: legendAlign,
                verticalAlign: legendVerticalAlign
            },
            tooltip: {
                headerFormat: '<b>{point.x}</b><br/>',
                pointFormat: '{series.name}: {point.y}<br/>Total: {point.stackTotal}'
            },
            plotOptions: {
                column: {
                    stacking: 'normal',
                    
                }
            },
            series: seriesData
        });
    }

    DqmSM.prototype.bindExceptionsColumnChart = function (containerID, pntWdth, seriesData) {
        Highcharts.chart(containerID, {
            chart: {
                type: 'column'
                //,
                //renderTo: containerId,
                //zoomType: 'x',
                //width: windowWidth,
                //height: 400

            },
            title: {
                text: ''
            },
            exporting: {
                enabled: false
            },
            colors: ['#6792BF', '#A859AA', '#E8DBB9', '#7AA6C1', '#939789', '#aa9b87', '#a2ddcc', '#ccd9de', '#f0c8a1', '#eda6a6'],
            legend: {
                color: '#A0A0A0',
                itemMarginTop: 10,
                itemMarginBottom: 10,
                enabled: true,
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'top',
                fontSize: '11px',
                fontWeight: 'normal',
                itemStyle: {
                    font: '9pt Trebuchet MS, Verdana, sans-serif',
                    color: '#A0A0A0'
                },
                itemHoverStyle: {
                    color: '#CCC'
                },
                itemSelectedStyle: {
                    color: '#FFF'
                }
            },
            credits: false,
            xAxis: {
                type: 'datetime',
                lineColor: '#b5b5b6',
                tickColor: '#b5b5b6',
                gridLineColor: 'rgb(241, 241, 241)',
                title: {
                    text: '',
                    offset: 30
                },
                top: '2%',
                labels: {
                    formatter: function () {
                        return Highcharts.dateFormat('%e %b %y', this.value);
                    },
                    style: {
                        color: '#b5b5b6',
                        fontSize: '10px'
                    }
                }
            },
            yAxis: {
                labels: {
                    align: 'right',
                    x: -3,
                    style: {
                        color: '#b5b5b6',
                        fontSize: '10px'
                    }

                },
                title: {
                    text: 'Exceptions Generated',
                    style: {
                        fontSize: '10px',
                        color: '#b5b5b6'
                    }
                },
                height: '100%',
                tickInterval: 60,
                lineColor: '#b5b5b6',
                tickColor: '#b5b5b6',
                gridLineColor: 'rgb(241, 241, 241)'

            },
            plotOptions: {
                column: {
                    stacking: 'percent',
                    pointWidth: pntWdth
                }
            },
            rangeSelector: {
                selected: 0,
                buttons: [{
                    type: 'all',
                    text: 'All'
                }],
                buttonTheme: {
                    visibility: 'hidden'
                },
                labelStyle: {
                    visibility: 'hidden'
                },
                inputEnabled: false
            },
            navigator: {
                series: {
                    color: '#287BFF',
                    fillOpacity: 0.4
                },
                xAxis: {
                    dateTimeLabelFormats: {
                        day: '%b %Y',
                        week: '%b %Y',
                        month: '%b %Y',
                        year: '%b %Y'
                    }
                }
            },
            scrollbar: {
                enabled: false,
                height: 4
            },
            rangeSelector: {
                selected: 4
            },
            series: seriesData
        });
    };

    return new DqmSM();

}();

srmDQMStatus.registerSrmDqmClient(3, srmDqmSM);