var SMScreenerSecurityView = {
    _windowHeight: 0,
    _windowWidth: 0,
    _secData: {},
    _secName : "",
    _chartLastPriceData : [],
    _chartVolumeData : [],
    _commonServiceLocation: "/BaseUserControls/Service/CommonService.svc",
    _path: "",
    setPath: function () {
        var path = window.location.protocol + '//' + window.location.host;
        var pathname = window.location.pathname.split('/');

        $.each(pathname, function (ii, ee) {
            if ((ii !== 0) && (ii !== pathname.length - 1))
                path = path + '/' + ee;
        });

        SMScreenerSecurityView._path = path;
    },
    _smScreenerSecurityViewPageModel: {},
    controls: {
        smScreenerSecurityView_container: function () {
            return $("#smScreenerSecurityView_container");
        },
        smScreenerSecurityView_splineChart: function () {
            return $("#smScreenerSecurityView_splineChart");
        },
        smScreenerSecurityView_barChart: function () {
            return $("#smScreenerSecurityView_barChart");
        }
    },
    preInit: function(){
        SMScreenerSecurityView._secName = sv_serverSideValue.securityName;
    },
    init: function () {
        if (typeof ko !== 'undefined') {
            SMScreenerSecurityView.preInit();
            SMScreenerSecurityView.setPath();
            
            SMScreenerSecurityView._windowHeight = $(window).height();
            SMScreenerSecurityView._windowWidth = $(window).width();

            SMScreenerSecurityView.controls.smScreenerSecurityView_container().width(SMScreenerSecurityView._windowWidth - 150);
            SMScreenerSecurityView.controls.smScreenerSecurityView_splineChart().width(SMScreenerSecurityView._windowWidth - 400);
            SMScreenerSecurityView.controls.smScreenerSecurityView_barChart().width(SMScreenerSecurityView._windowWidth - 400);

            SMScreenerSecurityView.getSecurityData(SMScreenerSecurityView._secName, SMScreenerSecurityView.onSuccessGetSecurityData);
            //SMScreenerSecurityView.bindBarChart();
        }
    },
    onSuccessGetSecurityData: function(data){
        var finalData = data.d.securityData;
        for(var item in finalData)
        {
            if(finalData[item].Key.indexOf("Security Name") > -1)
            {
                SMScreenerSecurityView._secData["SecurityName"] = finalData[item].Value;
            }
            else{
                if(!SMScreenerSecurityView._secData.hasOwnProperty("AttributeList"))
                {
                    SMScreenerSecurityView._secData["AttributeList"] = [];
                }
                
                var k = finalData[item].Key;
                var v = finalData[item].Value;
                SMScreenerSecurityView._secData["AttributeList"].push({ "AttributeName" : k, "AttributeValue" : v});
            }
        }

        SMScreenerSecurityView._chartLastPriceData = JSON.parse(data.d.securityHistoricalLastPriceData);
        
        SMScreenerSecurityView._chartVolumeData = JSON.parse(data.d.securityHistoricalVolumneData);

        SMScreenerSecurityView.bindLineChart();

        //Initialize Knockout View Model 
        SMScreenerSecurityView._smScreenerSecurityViewPageModel = new SMScreenerSecurityView.pageViewModel(SMScreenerSecurityView._secData);
        ko.applyBindings(SMScreenerSecurityView._smScreenerSecurityViewPageModel, SMScreenerSecurityView.controls.smScreenerSecurityView_container()[0]);

        SMScreenerSecurityView.controls.smScreenerSecurityView_container().smslimscroll({ "height" : ( (SMScreenerSecurityView._windowHeight - 20) + "px"), railVisible: true, alwaysVisible: true });
    },
    pageViewModel: function (data) {
        var self = this;

        self.singleSectionAttributeCount = 4;

        self.SecurityName = ko.observable();
        if (typeof data !== 'undefined' && typeof data.SecurityName !== 'undefined') {
            self.SecurityName(data.SecurityName);
        }

        self.AttributeList = ko.observableArray();
        if (typeof data !== 'undefined' && typeof data.AttributeList !== 'undefined' && data.AttributeList.length > 0) {
            for (var item in data.AttributeList) {
                self.AttributeList.push(new SMScreenerSecurityView.attributeViewModel(data.AttributeList[item]));
            }
        }

        self.AttributeFirstSection = ko.computed(function () {
            return self.AttributeList().slice(0, 4);
        });

        self.AttributeSecondSection = ko.computed(function () {
            return self.AttributeList().slice(4, 8);
        });

        self.AttributeThirdSection = ko.computed(function () {
            return self.AttributeList().slice(8, 12);
        });
    },
    attributeViewModel: function (data) {
        var self = this;

        self.AttributeName = ko.observable();
        if (typeof data !== 'undefined' && typeof data.AttributeName !== 'undefined') {
            self.AttributeName(data.AttributeName);
        }

        self.AttributeValue = ko.observable();
        if (typeof data !== 'undefined' && typeof data.AttributeValue !== 'undefined') {
            self.AttributeValue(data.AttributeValue);
        }
    },
    bindLineChart: function () {
        var chart = new Highcharts.StockChart('smScreenerSecurityView_splineChart',{
            chart: {
                renderTo: 'smScreenerSecurityView_splineChart'
            },
            credits: {
                enabled: false
            },
            xAxis: {
                type: 'date',
                lineWidth: 2,
                tickWidth: 2,
                labels: {
                    format: '{value:%e %b %y}'
                }
            },
            yAxis: [{
                title: {
                    text: 'Last Price',
                    offset: 70
                },
                lineWidth: 1,
                opposite: false,
                offset: 0,
                height:180,
                min: 0,
                labels: {
                    style: {
                        color: '#b5b5b6'
                    }
                },
                gridLineColor: '#f2f2f2'
            },
            {
                title: {
                    text: 'Volume',
                    offset: 70
                },
                lineWidth: 1,
                top:250,
                offset:0,
                height: 180,
                opposite:false,
                labels: {
                    style: {
                        color: '#b5b5b6'
                    }
                },
                gridLineColor: '#f2f2f2'
            }
            ],
            tooltip: {
                formatter: function () {
                    return Highcharts.dateFormat('%Y-%m-%d', this.x) + '<br/>' + Highcharts.numberFormat(this.y, 2);
                }
            },
            exporting: {
                enabled: false
            },
            legend: {
                enabled: false
            },
            scrollbar: {
                enabled: true
            },
            rangeSelector: {
                selected: 1,
                buttons: [
                            {
                                type: 'all',
                                text: 'All'
                            }
                        ],
                buttonTheme: {
                    visibility: 'hidden'
                },
                labelStyle: {
                    visibility: 'hidden'
                },
                inputEnabled: false
            },
            navigator: {
                top: 480 // Reposition the navigator based on the top of the chart
            },
            plotOptions: {
                areaspline: {
                    fillOpacity: 0.2
                }
            },
            series: [{
                data: SMScreenerSecurityView._chartLastPriceData,
                color: "rgba(45, 118, 188, 0.498039)",
                yAxis: 0,
                type: 'areaspline'
            },
            {
                yAxis: 1,
                color: '#cbaccb',
                data: SMScreenerSecurityView._chartVolumeData,
                type: 'column'
            }]
        });
    },
    getSecurityData: function(secName, success){
        $.ajax({
            type: 'POST',
            url: SMScreenerSecurityView._path + SMScreenerSecurityView._commonServiceLocation + "/GetSecurityData",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ "secName" : secName }),
            success: function (data) {
                success(data);
            },
            error: function () {
                console.log("Security Data Cannot be Fetched");
            }
        });
    }
}

$(document).ready(function () {
    SMScreenerSecurityView.init();
});

var tempData = {
    "SecurityName" : "IBM US",
    "AttributeList" : [
        { "AttributeName": "Issuer", "AttributeValue": "New York Times Company" },
        { "AttributeName": "Shares Outstanding", "AttributeValue": "943,212,551" },
        { "AttributeName": "High Price", "AttributeValue": "176.28" },
        { "AttributeName": "Low Price", "AttributeValue": "174.75" },
        { "AttributeName": "Close Price", "AttributeValue": "175.72" },
        { "AttributeName": "Bid Price", "AttributeValue": "175.81" },
        { "AttributeName": "Ask Price", "AttributeValue": "175.82" },
        { "AttributeName": "Volume", "AttributeValue": "3,815,537" },
        { "AttributeName": "GICS Sector", "AttributeValue": "Information Technology" },
        { "AttributeName": "GICS Industry Group", "AttributeValue": "Software & Services" },
        { "AttributeName": "GICS Industry", "AttributeValue": "IT Services" },
        { "AttributeName": "GICS Sub-Industry", "AttributeValue": "IT Consulting & Other Services" }
    ]
}

//SMScreenerSecurityView._chartLastPriceData = [
//[1460592000000, 355],
//[1460678400000, 358],
//[1460937600000, 358],
//[1461024000000, 365],
//[1461110400000, 367]
//];

//SMScreenerSecurityView._chartVolumeData = [
//[1460592000000, 833300],
//[1460678400000, 1108100],
//[1460937600000, 842000],
//[1461024000000, 834200],
//[1461110400000, 610100]
//];

//[
//        [Date.UTC(2015, 12, 29, 0, 0, 0), 8.160000000],
//        [Date.UTC(2015, 12, 30, 0, 0, 0), 3.830000000],
//        [Date.UTC(2015, 12, 31, 0, 0), 4.280000000],
//        [Date.UTC(2016, 1, 1, 0, 0, 0), 8.609999999],
//        [Date.UTC(2016, 1, 4, 0, 0, 0), 8.609999999],
//        [Date.UTC(2016, 1, 5, 0, 0, 0), 2.940000000],
//        [Date.UTC(2016, 1, 6, 0, 0, 0), 2.940000000],
//        [Date.UTC(2016, 1, 7, 0, 0, 0), 9.060000000],
//        [Date.UTC(2016, 1, 8, 0, 0, 0), 7.710000000],
//        [Date.UTC(2016, 2, 18, 0, 0, 0), 2.040000000]
//    ]