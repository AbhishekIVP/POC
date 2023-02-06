
var dataQualityJson = [
   {
       "ChainName": "Name - Securities in Position",
       "ChainTime": "03.55 AM",
       "ChainDate": "6/1/2017",
       "ChainAvgDuration": "14 mins",
       "ChainDuration": "13 mins 45 secs",
       "State": "Passed",
       "WarningCount":"0",
       "TaskDetails": [
         {
             "TaskName": "Loading Task for Geneva Data Load",
             "TaskType": "Loading",
             "Duration": "36m",
             "ModuleId": "3"
         },
         {
             "TaskName": "FLOW_Equity Common Stock",
             "TaskType": "Update Core",
             "Duration": "36m",
             "ModuleId": "3"
         },
         {
             "TaskName": "FLOW_Corporate Bond",
             "TaskType": "Update Core",
             "Duration": "36m",
             "ModuleId": "3"
         },
         {
             "TaskName": "FLOW_Convertible Bond",
             "TaskType": "Update Core",
             "Duration": "36m",
             "ModuleId": "3"
         },
         {
             "TaskName": "FLOW_Govt Bond",
             "TaskType": "Update Core",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW_ADR",
             "TaskType": "Update Core",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW_Equity Swap",
             "TaskType": "Update Core",
             "Duration": "36m"
         }
      ]
   },
   {
       "ChainName": "Name - Equities SOD Refresh",
       "ChainTime": "04.25 AM",
       "ChainDate": "6/1/2017",
       "ChainAvgDuration": "34 mins",
       "ChainDuration": "35 mins 33 secs",
       "State": "Warning",
       "WarningCount": "10",
       "TaskDetails": [
         {
             "TaskName": "Equity Common Stock Feed - Request",
             "TaskType": "Request",
             "Duration": "36m"
         },
         {
             "TaskName": "Equity Common Stock Feed - Response",
             "TaskType": "Response",
             "Duration": "36m"
         },
         {
             "TaskName": "ADR Feed - Request",
             "TaskType": "Request",
             "Duration": "36m"
         },
         {
             "TaskName": "ADR Feed - Response",
             "TaskType": "Response",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW_Equity Common Stock",
             "TaskType": "Update Core",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW_ADR",
             "TaskType": "Update Core",
             "Duration": "36m"
         }
      ]
     },
     {
       "ChainName": "Name - Bonds SOD Refresh",
       "ChainTime": "05.10 AM",
       "ChainDate": "6/1/2017",
       "ChainAvgDuration": "42 mins",
       "ChainDuration": "41 mins 55 secs",
       "State": "Failed",
       "WarningCount": "10",
       "TaskDetails": [
         {
             "TaskName": "Bonds Feed - Request",
             "TaskType": "Request",
             "Duration": "36m"
         },
         {
             "TaskName": "Bonds Feed - Response",
             "TaskType": "Response",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW_Corporate Bond",
             "TaskType": "Update Core",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW_Convertible Bond",
             "TaskType": "Update Core",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW_Govt Bond",
             "TaskType": "Update Core",
             "Duration": "36m"
         }
      ]
     },
     {
         "ChainName": "Name - Credit Risk data load",
         "ChainTime": "06.00 AM",
         "ChainDate": "6/1/2017",
         "ChainAvgDuration": "1 hour 13 mins",
         "ChainDuration": "1 hour 14 mins",
         "State": "Warning",
         "WarningCount": "10",
         "TaskDetails": [
         {
             "TaskName": "Loading Task for Bloomberg Credit Risk File",
             "TaskType": "Loading",
             "Duration": "36m"
         },
         {
             "TaskName": "Bonds Feed - Response",
             "TaskType": "Response",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW_Issuer",
             "TaskType": "Update Core",
             "Duration": "36m"
         }
      ]
     },
     {
         "ChainName": "Equity Asia 1 Load",
         "ChainTime": "8.00 AM",
         "ChainDate": "6/1/2017",
         "ChainAvgDuration": "24 mins",
         "ChainDuration": "24 mins 30 secs",
         "State": "Warning",
         "WarningCount": "10",
         "TaskDetails": [
         {
             "TaskName": "Loading Task for Equity.out Backoffice File",
             "TaskType": "Loading",
             "Duration": "36m"
         },
         {
             "TaskName": "Loading Task for Equity.px Backoffice File",
             "TaskType": "Loading",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW_Equity Common Stock",
             "TaskType": "Update Core",
             "Duration": "36m"
         }
      ]
     },
     {
         "ChainName": "Equity Asia 2 Load",
         "ChainTime": " 11.30 PM",
         "ChainDate": "6/1/2017",
         "ChainAvgDuration": "44 mins",
         "ChainDuration": "43 mins 30 secs",
         "State": "Warning",
         "WarningCount": "10",
         "TaskDetails": [
         {
             "TaskName": "Loading Task for Equity.out Backoffice File",
             "TaskType": "Loading",
             "Duration": "36m"
         },
         {
             "TaskName": "Loading Task for Equity.px Backoffice File",
             "TaskType": "Loading",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW_Equity Common Stock",
             "TaskType": "Update Core",
             "Duration": "36m"
         }
      ]
     },
     {
         "ChainName": "Equity Europe Load",
         "ChainTime": "01.30 PM",
         "ChainDate": "6/1/2017",
         "ChainAvgDuration": "35 mins",
         "ChainDuration": "36 mins 30 secs",
         "State": "Warning",
         "WarningCount": "10",
         "TaskDetails": [
         {
             "TaskName": "Loading Task for Equity.out Backoffice File",
             "TaskType": "Loading",
             "Duration": "36m"
         },
         {
             "TaskName": "Loading Task for Equity.px Backoffice File",
             "TaskType": "Loading",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW_Equity Common Stock",
             "TaskType": "Update Core",
             "Duration": "36m"
         }
      ]
     },
     {
         "ChainName": "Equity NAMR Load",
         "ChainTime": "06.00 PM",
         "ChainDate": "6/1/2017",
         "ChainAvgDuration": "56 mins",
         "ChainDuration": "54 mins 45 secs",
         "State": "Passed",
         "WarningCount": "10",
         "TaskDetails": [
         {
             "TaskName": "Loading Task for Equity.out Backoffice File",
             "TaskType": "Loading",
             "Duration": "36m"
         },
         {
             "TaskName": "Loading Task for Equity.px Backoffice File",
             "TaskType": "Loading",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW_Equity Common Stock",
             "TaskType": "Update Core",
             "Duration": "36m"
         }
      ]
     },
     {
         "ChainName": "Equity LAMR Load",
         "ChainTime": "08.00 PM",
         "ChainDate": "6/1/2017",
         "ChainAvgDuration": "37 mins",
         "ChainDuration": "37 mins 30 secs",
         "State": "Passed",
         "WarningCount": "10",
         "TaskDetails": [
         {
             "TaskName": "Loading Task for Equity.out Backoffice File",
             "TaskType": "Loading",
             "Duration": "36m"
         },
         {
             "TaskName": "Loading Task for Equity.px Backoffice File",
             "TaskType": "Loading",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW_Equity Common Stock",
             "TaskType": "Update Core",
             "Duration": "36m"
         }
      ]
     },
     {
         "ChainName": "Corporate Actions EOD Data Load",
         "ChainTime": "10.00 PM",
         "ChainDate": "6/1/2017",
         "ChainAvgDuration": "1 hour 1 mins",
         "ChainDuration": "59 mins 45 secs",
         "State": "Passed",
         "WarningCount": "10",
         "TaskDetails": [
         {
             "TaskName": "Request-CASH_DVD",
             "TaskType": "Request",
             "Duration": "36m"
         },
         {
             "TaskName": "Response-CASH_DVD",
             "TaskType": "Response",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW-CASH_DVD",
             "TaskType": "Update Core",
             "Duration": "36m"
         },
         {
             "TaskName": "Request-STK_DVD",
             "TaskType": "Request",
             "Duration": "36m"
         },
         {
             "TaskName": "Response-STK_DVD",
             "TaskType": "Response",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW-STK_DVD",
             "TaskType": "Update Core",
             "Duration": "36m"
         },
         {
             "TaskName": "Request-STK_SPLIT",
             "TaskType": "Request",
             "Duration": "36m"
         },
         {
             "TaskName": "Response-STK_SPLIT",
             "TaskType": "Response",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW-STK_SPLIT",
             "TaskType": "Update Core",
             "Duration": "36m"
         },
         {
             "TaskName": "Request-SPINOFF",
             "TaskType": "Request",
             "Duration": "36m"
         },
         {
             "TaskName": "Response-SPINOFF",
             "TaskType": "Response",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW-SPINOFF",
             "TaskType": "Update Core",
             "Duration": "36m"
         }
      ]
     },


     {
         "ChainName": "Equity Europe Load",
         "ChainTime": "01.30 PM",
         "ChainDate": "6/1/2017",
         "ChainAvgDuration": "35 mins",
         "ChainDuration": "36 mins 30 secs",
         "State": "Warning",
         "WarningCount": "10",
         "TaskDetails": [
         {
             "TaskName": "Loading Task for Equity.out Backoffice File",
             "TaskType": "Loading",
             "Duration": "36m"
         },
         {
             "TaskName": "Loading Task for Equity.px Backoffice File",
             "TaskType": "Loading",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW_Equity Common Stock",
             "TaskType": "Update Core",
             "Duration": "36m"
         }
      ]
     },
     {
         "ChainName": "Equity NAMR Load",
         "ChainTime": "06.00 PM",
         "ChainDate": "6/1/2017",
         "ChainAvgDuration": "56 mins",
         "ChainDuration": "54 mins 45 secs",
         "State": "Passed",
         "WarningCount": "10",
         "TaskDetails": [
         {
             "TaskName": "Loading Task for Equity.out Backoffice File",
             "TaskType": "Loading",
             "Duration": "36m"
         },
         {
             "TaskName": "Loading Task for Equity.px Backoffice File",
             "TaskType": "Loading",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW_Equity Common Stock",
             "TaskType": "Update Core",
             "Duration": "36m"
         }
      ]
     },
     {
         "ChainName": "Equity LAMR Load",
         "ChainTime": "08.00 PM",
         "ChainDate": "6/1/2017",
         "ChainAvgDuration": "37 mins",
         "ChainDuration": "37 mins 30 secs",
         "State": "Passed",
         "WarningCount": "10",
         "TaskDetails": [
         {
             "TaskName": "Loading Task for Equity.out Backoffice File",
             "TaskType": "Loading",
             "Duration": "36m"
         },
         {
             "TaskName": "Loading Task for Equity.px Backoffice File",
             "TaskType": "Loading",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW_Equity Common Stock",
             "TaskType": "Update Core",
             "Duration": "36m"
         }
      ]
     },
     {
         "ChainName": "Corporate Actions EOD Data Load",
         "ChainTime": "10.00 PM",
         "ChainDate": "6/1/2017",
         "ChainAvgDuration": "1 hour 1 mins",
         "ChainDuration": "59 mins 45 secs",
         "State": "Passed",
         "WarningCount": "10",
         "TaskDetails": [
         {
             "TaskName": "Request-CASH_DVD",
             "TaskType": "Request",
             "Duration": "36m"
         },
         {
             "TaskName": "Response-CASH_DVD",
             "TaskType": "Response",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW-CASH_DVD",
             "TaskType": "Update Core",
             "Duration": "36m"
         },
         {
             "TaskName": "Request-STK_DVD",
             "TaskType": "Request",
             "Duration": "36m"
         },
         {
             "TaskName": "Response-STK_DVD",
             "TaskType": "Response",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW-STK_DVD",
             "TaskType": "Update Core",
             "Duration": "36m"
         },
         {
             "TaskName": "Request-STK_SPLIT",
             "TaskType": "Request",
             "Duration": "36m"
         },
         {
             "TaskName": "Response-STK_SPLIT",
             "TaskType": "Response",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW-STK_SPLIT",
             "TaskType": "Update Core",
             "Duration": "36m"
         },
         {
             "TaskName": "Request-SPINOFF",
             "TaskType": "Request",
             "Duration": "36m"
         },
         {
             "TaskName": "Response-SPINOFF",
             "TaskType": "Response",
             "Duration": "36m"
         },
         {
             "TaskName": "FLOW-SPINOFF",
             "TaskType": "Update Core",
             "Duration": "36m"
         }
      ]
     }
]

var chartConfigJson = [{
    "ProductName": "Secmaster",
    "TaskInfo": [{
        "TaskType": "Loading",
        "TaskTypeInfo": [{
            "Text": "File Format"
        }],
        "ChartInfo": [{
            "Info": [{
                "Text": "Total Time Taken"
            }],
            "ChartType": "Line",
            "Title": "Solar Employment Growth by Sector, 2010-2016",
            "SubTitle": "Source: thesolarfoundation.com",
            "YAxisText": "Number of Employees",
            "LegendLayout": "vertical",
            "LegendAlign": "right",
            "LegendVerticalAlign": "middle",
            "SeriesName":"Installation",
            "PlotStartPoint": "2010"
        },
        {
            "Info": [{
                "Text": "File Size"
            }],
            "RequireTabs":"true",
            "ChartType": "Line",
            "Title": "Solar Employment Growth by Sector, 2010-2016",
            "SubTitle": "Source: thesolarfoundation.com",
            "YAxisText": "Number of Employees",
            "LegendLayout": "vertical",
            "LegendAlign": "right",
            "LegendVerticalAlign": "middle",
            "SeriesName": "Installation",
            "PlotStartPoint": "2010"
        },
	    {
		    "Info": [{
		        "Text": "Record Processed"
		    }],
		    "ChartType": "Column",
		    "Title": "Stacked column chart",
		    "SubTitle": "",
		    "YAxisText": "Total fruit consumption",
		    "YAxisMinValue": "0",
		    "XAxisCategories": ['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas'],
		    "PlotOptionsColumnStacking": "percent"
	    }]
},
    {
        "TaskType": "Update Core",
        "TaskTypeInfo": [{
            "Text": "File Format"
        }],
        "ChartInfo": [{
            "Info": [{
                "Text": "Record Processed"
            }],
            "ChartType": "ExceptionColumn",
            "Title": "Stacked column chart",
            "SubTitle": "",
            "YAxisText": "Total fruit consumption",
            "YAxisMinValue": "0",
            "XAxisCategories": ['Apples', 'Oranges', 'Pears', 'Grapes', 'Bananas'],
            "PlotOptionsColumnStacking": "percent"
        }]
    }]
}]

var lineGraphDataJson = {
    "SeriesData": [{
        name: 'Task Loading Data',
        data: [
            [Date.UTC(1970, 9, 21), 0],
            [Date.UTC(1970, 10, 4), 0.28],
            [Date.UTC(1970, 10, 9), 0.25],
            [Date.UTC(1970, 10, 27), 0.2],
            [Date.UTC(1970, 11, 2), 0.28],
            [Date.UTC(1970, 11, 26), 0.28],
            [Date.UTC(1970, 11, 29), 0.47],
            [Date.UTC(1971, 0, 11), 0.79],
            [Date.UTC(1971, 0, 26), 0.72],
            [Date.UTC(1971, 1, 3), 1.02],
            [Date.UTC(1971, 1, 11), 1.12],
            [Date.UTC(1971, 1, 25), 1.2],
            [Date.UTC(1971, 2, 11), 1.18],
            [Date.UTC(1971, 3, 11), 1.19],
            [Date.UTC(1971, 4, 1), 1.85],
            [Date.UTC(1971, 4, 5), 2.22],
            [Date.UTC(1971, 4, 19), 1.15],
            [Date.UTC(1971, 5, 3), 0]
        ]
    }],
    "Info1": "6.3 mins",
    "SubInfo1":"up by 2%"
};

/*class ChartDate
{
    int Year
    int month,
    int date,
}

class ChartData
{
    Dictionary<ChartDate, Value> data = new Dictionary<>;
}
*/

//initial line graph temporary data
var chainLineGraphDataJson = {
    "SeriesData": [{
        name: 'Chain Loading Data',
        data: [
            [Date.UTC(1970, 9, 21), 0],
            [Date.UTC(1970, 10, 4), 0.28],
            [Date.UTC(1970, 10, 9), 0.25],
            [Date.UTC(1970, 10, 27), 0.2],
            [Date.UTC(1970, 11, 2), 0.28],
            [Date.UTC(1970, 11, 26), 0.28],
            [Date.UTC(1970, 11, 29), 0.47],
            [Date.UTC(1971, 0, 11), 0.79],
            [Date.UTC(1971, 0, 26), 0.72],
            [Date.UTC(1971, 1, 3), 1.02],
            [Date.UTC(1971, 1, 11), 1.12],
            [Date.UTC(1971, 1, 25), 1.2],
            [Date.UTC(1971, 2, 11), 1.18],
            [Date.UTC(1971, 3, 11), 1.19],
            [Date.UTC(1971, 4, 1), 1.85],
            [Date.UTC(1971, 4, 5), 2.22],
            [Date.UTC(1971, 4, 19), 1.15],
            [Date.UTC(1971, 5, 3), 0]
        ],
        color: "rgba(45, 118, 188, 0.498039)",
        yAxis: 0
    }],
    "Info1": "6.3 mins",
    "SubInfo1": "up by 2%"
};

var chainLineGraph2DataJson = {
    "SeriesData": [{
        name: 'Chain Loading Data',
        data: [[Date.UTC(2016, 10, 03), 14],
[Date.UTC(2016, 10, 04), 14],
[Date.UTC(2016, 10, 05), 13],
[Date.UTC(2016, 10, 06), 14],
[Date.UTC(2016, 10, 07), 14],
[Date.UTC(2016, 10, 08), 14],
[Date.UTC(2016, 10, 09), 12],
[Date.UTC(2016, 10, 10), 13],
[Date.UTC(2016, 10, 11), 13],
[Date.UTC(2016, 10, 12), 13],
[Date.UTC(2016, 10, 13), 11],
[Date.UTC(2016, 10, 14), 12],
[Date.UTC(2016, 10, 15), 12],
[Date.UTC(2016, 10, 16), 11],
[Date.UTC(2016, 10, 17), 12],
[Date.UTC(2016, 10, 18), 13],
[Date.UTC(2016, 10, 19), 11],
[Date.UTC(2016, 10, 20), 14],
[Date.UTC(2016, 10, 21), 12],
[Date.UTC(2016, 10, 22), 13],
[Date.UTC(2016, 10, 23), 12],
[Date.UTC(2016, 10, 24), 13],
[Date.UTC(2016, 10, 25), 12],
[Date.UTC(2016, 10, 26), 13],
[Date.UTC(2016, 10, 27), 12],
[Date.UTC(2016, 10, 28), 12],
[Date.UTC(2016, 10, 29), 14],
[Date.UTC(2016, 10, 30), 12],
[Date.UTC(2016, 11, 01), 13],
[Date.UTC(2016, 11, 02), 14],
[Date.UTC(2016, 11, 03), 13],
[Date.UTC(2016, 11, 04), 13],
[Date.UTC(2016, 11, 05), 11],
[Date.UTC(2016, 11, 06), 11],
[Date.UTC(2016, 11, 07), 14],
[Date.UTC(2016, 11, 08), 14],
[Date.UTC(2016, 11, 09), 14],
[Date.UTC(2016, 11, 10), 13],
[Date.UTC(2016, 11, 11), 11],
[Date.UTC(2016, 11, 12), 14],
[Date.UTC(2016, 11, 13), 12],
[Date.UTC(2016, 11, 14), 13],
[Date.UTC(2016, 11, 15), 11],
[Date.UTC(2016, 11, 16), 12],
[Date.UTC(2016, 11, 17), 12],
[Date.UTC(2016, 11, 18), 12],
[Date.UTC(2016, 11, 19), 11],
[Date.UTC(2016, 11, 20), 12],
[Date.UTC(2016, 11, 21), 13],
[Date.UTC(2016, 11, 22), 12],
[Date.UTC(2016, 11, 23), 14],
[Date.UTC(2016, 11, 24), 11],
[Date.UTC(2016, 11, 25), 12],
[Date.UTC(2016, 11, 26), 13],
[Date.UTC(2016, 11, 27), 12],
[Date.UTC(2016, 11, 28), 13],
[Date.UTC(2016, 11, 29), 11],
[Date.UTC(2016, 11, 30), 13],
[Date.UTC(2016, 11, 31), 13],
[Date.UTC(2017, 0, 01), 11],
[Date.UTC(2017, 0, 02), 14],
[Date.UTC(2017, 0, 03), 13],
[Date.UTC(2017, 0, 04), 12],
[Date.UTC(2017, 0, 05), 13],
[Date.UTC(2017, 0, 06), 13],
[Date.UTC(2017, 0, 07), 13],
[Date.UTC(2017, 0, 08), 14],
[Date.UTC(2017, 0, 09), 12],
[Date.UTC(2017, 0, 10), 12],
[Date.UTC(2017, 0, 11), 11],
[Date.UTC(2017, 0, 12), 13],
[Date.UTC(2017, 0, 13), 13],
[Date.UTC(2017, 0, 14), 14],
[Date.UTC(2017, 0, 15), 11],
[Date.UTC(2017, 0, 16), 14],
[Date.UTC(2017, 0, 17), 13],
[Date.UTC(2017, 0, 18), 12],
[Date.UTC(2017, 0, 19), 13],
[Date.UTC(2017, 0, 20), 13],
[Date.UTC(2017, 0, 21), 12],
[Date.UTC(2017, 0, 22), 11],
[Date.UTC(2017, 0, 23), 13],
[Date.UTC(2017, 0, 24), 12],
[Date.UTC(2017, 0, 25), 13],
[Date.UTC(2017, 0, 26), 14],
[Date.UTC(2017, 0, 27), 14],
[Date.UTC(2017, 0, 28), 14],
[Date.UTC(2017, 0, 29), 13],
[Date.UTC(2017, 0, 30), 13],
[Date.UTC(2017, 0, 31), 12],
[Date.UTC(2017, 1, 01), 11],
[Date.UTC(2017, 1, 02), 12],
[Date.UTC(2017, 1, 03), 13],
[Date.UTC(2017, 1, 04), 12],
[Date.UTC(2017, 1, 05), 11],
[Date.UTC(2017, 1, 06), 11],
[Date.UTC(2017, 1, 07), 13],
[Date.UTC(2017, 1, 08), 12],
[Date.UTC(2017, 1, 09), 13],
[Date.UTC(2017, 1, 10), 12],
[Date.UTC(2017, 1, 11), 13],
[Date.UTC(2017, 1, 12), 14],
[Date.UTC(2017, 1, 13), 13],
[Date.UTC(2017, 1, 14), 11],
[Date.UTC(2017, 1, 15), 12],
[Date.UTC(2017, 1, 16), 13],
[Date.UTC(2017, 1, 17), 12],
[Date.UTC(2017, 1, 18), 13],
[Date.UTC(2017, 1, 19), 13],
[Date.UTC(2017, 1, 20), 13],
[Date.UTC(2017, 1, 21), 14],
[Date.UTC(2017, 1, 22), 13],
[Date.UTC(2017, 1, 23), 12],
[Date.UTC(2017, 1, 24), 13],
[Date.UTC(2017, 1, 25), 11],
[Date.UTC(2017, 1, 26), 12],
[Date.UTC(2017, 1, 27), 13],
[Date.UTC(2017, 1, 28), 12],
[Date.UTC(2017, 2, 01), 12],
[Date.UTC(2017, 2, 02), 13],
[Date.UTC(2017, 2, 03), 14],
[Date.UTC(2017, 2, 04), 14],
[Date.UTC(2017, 2, 05), 13],
[Date.UTC(2017, 2, 06), 13],
[Date.UTC(2017, 2, 07), 11],
[Date.UTC(2017, 2, 08), 11],
[Date.UTC(2017, 2, 09), 14],
[Date.UTC(2017, 2, 10), 11],
[Date.UTC(2017, 2, 11), 12],
[Date.UTC(2017, 2, 12), 12],
[Date.UTC(2017, 2, 13), 12],
[Date.UTC(2017, 2, 14), 12],
[Date.UTC(2017, 2, 15), 11],
[Date.UTC(2017, 2, 16), 11],
[Date.UTC(2017, 2, 17), 13],
[Date.UTC(2017, 2, 18), 11],
[Date.UTC(2017, 2, 19), 13],
[Date.UTC(2017, 2, 20), 13],
[Date.UTC(2017, 2, 21), 12],
[Date.UTC(2017, 2, 22), 14],
[Date.UTC(2017, 2, 23), 13],
[Date.UTC(2017, 2, 24), 12],
[Date.UTC(2017, 2, 25), 13],
[Date.UTC(2017, 2, 26), 11],
[Date.UTC(2017, 2, 27), 12],
[Date.UTC(2017, 2, 28), 11],
[Date.UTC(2017, 2, 29), 12],
[Date.UTC(2017, 2, 30), 11],
[Date.UTC(2017, 2, 31), 13],
[Date.UTC(2017, 3, 01), 14],
[Date.UTC(2017, 3, 02), 13],
[Date.UTC(2017, 3, 03), 14],
[Date.UTC(2017, 3, 04), 12],
[Date.UTC(2017, 3, 05), 12],
[Date.UTC(2017, 3, 06), 13],
[Date.UTC(2017, 3, 07), 13],
[Date.UTC(2017, 3, 08), 14],
[Date.UTC(2017, 3, 09), 13],
[Date.UTC(2017, 3, 10), 12],
[Date.UTC(2017, 3, 11), 11],
[Date.UTC(2017, 3, 12), 13],
[Date.UTC(2017, 3, 13), 12],
[Date.UTC(2017, 3, 14), 13],
[Date.UTC(2017, 3, 15), 13],
[Date.UTC(2017, 3, 16), 14],
[Date.UTC(2017, 3, 17), 12],
[Date.UTC(2017, 3, 18), 12],
[Date.UTC(2017, 3, 19), 13],
[Date.UTC(2017, 3, 20), 12],
[Date.UTC(2017, 3, 21), 14],
[Date.UTC(2017, 3, 22), 13],
[Date.UTC(2017, 3, 23), 14],
[Date.UTC(2017, 3, 24), 14],
[Date.UTC(2017, 3, 25), 12],
[Date.UTC(2017, 3, 26), 12],
[Date.UTC(2017, 3, 27), 11],
[Date.UTC(2017, 3, 28), 11],
[Date.UTC(2017, 3, 29), 14],
[Date.UTC(2017, 3, 30), 12],
[Date.UTC(2017, 4, 01), 12],
[Date.UTC(2017, 4, 02), 13],
[Date.UTC(2017, 4, 03), 11],
[Date.UTC(2017, 4, 04), 14],
[Date.UTC(2017, 4, 05), 13],
[Date.UTC(2017, 4, 06), 14],
[Date.UTC(2017, 4, 07), 12],
[Date.UTC(2017, 4, 08), 13],
[Date.UTC(2017, 4, 09), 13],
[Date.UTC(2017, 4, 10), 11],
[Date.UTC(2017, 4, 11), 14],
[Date.UTC(2017, 4, 12), 13],
[Date.UTC(2017, 4, 13), 13],
[Date.UTC(2017, 4, 14), 13],
[Date.UTC(2017, 4, 15), 12],
[Date.UTC(2017, 4, 16), 11],
[Date.UTC(2017, 4, 17), 13],
[Date.UTC(2017, 4, 18), 14],
[Date.UTC(2017, 4, 19), 12],
[Date.UTC(2017, 4, 20), 14],
[Date.UTC(2017, 4, 21), 14],
[Date.UTC(2017, 4, 22), 12],
[Date.UTC(2017, 4, 23), 12],
[Date.UTC(2017, 4, 24), 11],
[Date.UTC(2017, 4, 25), 12],
[Date.UTC(2017, 4, 26), 12],
[Date.UTC(2017, 4, 27), 13],
[Date.UTC(2017, 4, 28), 12],
[Date.UTC(2017, 4, 29), 12],
[Date.UTC(2017, 4, 30), 13],
[Date.UTC(2017, 4, 31), 13]
],
        color: "rgba(45, 118, 188, 0.498039)",
        yAxis: 0,
        
    }],
    "Info1": "6.3 mins",
    "SubInfo1": "up by 2%"
};

var chainLineGraph1DataJson = {
    "SeriesData": [{
        name: 'Chain Loading Data',
        data: [
            [Date.UTC(2017, 4, 31), 14],
[Date.UTC(2017, 4, 30), 13],
[Date.UTC(2017, 4, 29), 12],
[Date.UTC(2017, 4, 28), 12],
[Date.UTC(2017, 4, 27), 13],
[Date.UTC(2017, 4, 26), 12],
[Date.UTC(2017, 4, 25), 12],
[Date.UTC(2017, 4, 24), 11],
[Date.UTC(2017, 4, 23), 12],
[Date.UTC(2017, 4, 22), 12],
[Date.UTC(2017, 4, 21), 14],
[Date.UTC(2017, 4, 20), 14],
[Date.UTC(2017, 4, 19), 12],
[Date.UTC(2017, 4, 18), 14],
[Date.UTC(2017, 4, 17), 13],
[Date.UTC(2017, 4, 16), 11],
[Date.UTC(2017, 4, 15), 12],
[Date.UTC(2017, 4, 14), 13],
[Date.UTC(2017, 4, 13), 13],
[Date.UTC(2017, 4, 12), 13],
[Date.UTC(2017, 4, 11), 14],
[Date.UTC(2017, 4, 10), 11],
[Date.UTC(2017, 4, 09), 13],
[Date.UTC(2017, 4, 08), 13],
[Date.UTC(2017, 4, 07), 12],
[Date.UTC(2017, 4, 06), 14],
[Date.UTC(2017, 4, 05), 13],
[Date.UTC(2017, 4, 04), 14],
[Date.UTC(2017, 4, 03), 11],
[Date.UTC(2017, 4, 02), 13],
[Date.UTC(2017, 4, 01), 12],
[Date.UTC(2017, 3, 30), 12],
[Date.UTC(2017, 3, 29), 14],
[Date.UTC(2017, 3, 28), 11],
[Date.UTC(2017, 3, 27), 11],
[Date.UTC(2017, 3, 26), 12],
[Date.UTC(2017, 3, 25), 12],
[Date.UTC(2017, 3, 24), 14],
[Date.UTC(2017, 3, 23), 14],
[Date.UTC(2017, 3, 22), 13],
[Date.UTC(2017, 3, 21), 14],
[Date.UTC(2017, 3, 20), 12],
[Date.UTC(2017, 3, 19), 13],
[Date.UTC(2017, 3, 18), 12],
[Date.UTC(2017, 3, 17), 12],
[Date.UTC(2017, 3, 16), 14],
[Date.UTC(2017, 3, 15), 13],
[Date.UTC(2017, 3, 14), 13],
[Date.UTC(2017, 3, 13), 12],
[Date.UTC(2017, 3, 12), 13],
[Date.UTC(2017, 3, 11), 11],
[Date.UTC(2017, 3, 10), 12],
[Date.UTC(2017, 3, 09), 13],
[Date.UTC(2017, 3, 08), 14],
[Date.UTC(2017, 3, 07), 13],
[Date.UTC(2017, 3, 06), 13],
[Date.UTC(2017, 3, 05), 12],
[Date.UTC(2017, 3, 04), 12],
[Date.UTC(2017, 3, 03), 14],
[Date.UTC(2017, 3, 02), 13],
[Date.UTC(2017, 3, 01), 14],
[Date.UTC(2017, 2, 31), 13],
[Date.UTC(2017, 2, 30), 11],
[Date.UTC(2017, 2, 29), 12],
[Date.UTC(2017, 2, 28), 11],
[Date.UTC(2017, 2, 27), 12],
[Date.UTC(2017, 2, 26), 11],
[Date.UTC(2017, 2, 25), 13],
[Date.UTC(2017, 2, 24), 12],
[Date.UTC(2017, 2, 23), 13],
[Date.UTC(2017, 2, 22), 14],
[Date.UTC(2017, 2, 21), 12],
[Date.UTC(2017, 2, 20), 13],
[Date.UTC(2017, 2, 19), 13],
[Date.UTC(2017, 2, 18), 11],
[Date.UTC(2017, 2, 17), 13],
[Date.UTC(2017, 2, 16), 11],
[Date.UTC(2017, 2, 15), 11],
[Date.UTC(2017, 2, 14), 12],
[Date.UTC(2017, 2, 13), 12],
[Date.UTC(2017, 2, 12), 12],
[Date.UTC(2017, 2, 11), 12],
[Date.UTC(2017, 2, 10), 11],
[Date.UTC(2017, 2, 09), 14],
[Date.UTC(2017, 2, 08), 11],
[Date.UTC(2017, 2, 07), 11],
[Date.UTC(2017, 2, 06), 13],
[Date.UTC(2017, 2, 05), 13],
[Date.UTC(2017, 2, 04), 14],
[Date.UTC(2017, 2, 03), 14],
[Date.UTC(2017, 2, 02), 13],
[Date.UTC(2017, 2, 01), 12],
[Date.UTC(2017, 1, 28), 12],
[Date.UTC(2017, 1, 27), 13],
[Date.UTC(2017, 1, 26), 12],
[Date.UTC(2017, 1, 25), 11],
[Date.UTC(2017, 1, 24), 13],
[Date.UTC(2017, 1, 23), 12],
[Date.UTC(2017, 1, 22), 13],
[Date.UTC(2017, 1, 21), 14],
[Date.UTC(2017, 1, 20), 13],
[Date.UTC(2017, 1, 19), 13],
[Date.UTC(2017, 1, 18), 13],
[Date.UTC(2017, 1, 17), 12],
[Date.UTC(2017, 1, 16), 13],
[Date.UTC(2017, 1, 15), 12],
[Date.UTC(2017, 1, 14), 11],
[Date.UTC(2017, 1, 13), 13],
[Date.UTC(2017, 1, 12), 14],
[Date.UTC(2017, 1, 11), 13],
[Date.UTC(2017, 1, 10), 12],
[Date.UTC(2017, 1, 09), 13],
[Date.UTC(2017, 1, 08), 12],
[Date.UTC(2017, 1, 07), 13],
[Date.UTC(2017, 1, 06), 11],
[Date.UTC(2017, 1, 05), 11],
[Date.UTC(2017, 1, 04), 12],
[Date.UTC(2017, 1, 03), 13],
[Date.UTC(2017, 1, 02), 12],
[Date.UTC(2017, 1, 01), 11],
[Date.UTC(2017, 0, 31), 12],
[Date.UTC(2017, 0, 30), 13],
[Date.UTC(2017, 0, 29), 13],
[Date.UTC(2017, 0, 28), 14],
[Date.UTC(2017, 0, 27), 14],
[Date.UTC(2017, 0, 26), 14],
[Date.UTC(2017, 0, 25), 13],
[Date.UTC(2017, 0, 24), 12],
[Date.UTC(2017, 0, 23), 13],
[Date.UTC(2017, 0, 22), 11],
[Date.UTC(2017, 0, 21), 12],
[Date.UTC(2017, 0, 20), 13],
[Date.UTC(2017, 0, 19), 13],
[Date.UTC(2017, 0, 18), 12],
[Date.UTC(2017, 0, 17), 13],
[Date.UTC(2017, 0, 16), 14],
[Date.UTC(2017, 0, 15), 11],
[Date.UTC(2017, 0, 14), 14],
[Date.UTC(2017, 0, 13), 13],
[Date.UTC(2017, 0, 12), 13],
[Date.UTC(2017, 0, 11), 11],
[Date.UTC(2017, 0, 10), 12],
[Date.UTC(2017, 0, 09), 12],
[Date.UTC(2017, 0, 08), 14],
[Date.UTC(2017, 0, 07), 13],
[Date.UTC(2017, 0, 06), 13],
[Date.UTC(2017, 0, 05), 13],
[Date.UTC(2017, 0, 04), 12],
[Date.UTC(2017, 0, 03), 13],
[Date.UTC(2017, 0, 02), 14],
[Date.UTC(2017, 0, 01), 11],
[Date.UTC(2016, 11, 31), 13],
[Date.UTC(2016, 11, 30), 13],
[Date.UTC(2016, 11, 29), 11],
[Date.UTC(2016, 11, 28), 13],
[Date.UTC(2016, 11, 27), 12],
[Date.UTC(2016, 11, 26), 13],
[Date.UTC(2016, 11, 25), 12],
[Date.UTC(2016, 11, 24), 11],
[Date.UTC(2016, 11, 23), 14],
[Date.UTC(2016, 11, 22), 12],
[Date.UTC(2016, 11, 21), 13],
[Date.UTC(2016, 11, 20), 12],
[Date.UTC(2016, 11, 19), 11],
[Date.UTC(2016, 11, 18), 12],
[Date.UTC(2016, 11, 17), 12],
[Date.UTC(2016, 11, 16), 12],
[Date.UTC(2016, 11, 15), 11],
[Date.UTC(2016, 11, 14), 13],
[Date.UTC(2016, 11, 13), 12],
[Date.UTC(2016, 11, 12), 14],
[Date.UTC(2016, 11, 11), 11],
[Date.UTC(2016, 11, 10), 13],
[Date.UTC(2016, 11, 09), 14],
[Date.UTC(2016, 11, 08), 14],
[Date.UTC(2016, 11, 07), 14],
[Date.UTC(2016, 11, 06), 11],
[Date.UTC(2016, 11, 05), 11],
[Date.UTC(2016, 11, 04), 13],
[Date.UTC(2016, 11, 03), 13],
[Date.UTC(2016, 11, 02), 14],
[Date.UTC(2016, 11, 01), 13],
[Date.UTC(2016, 10, 30), 12],
[Date.UTC(2016, 10, 29), 14],
[Date.UTC(2016, 10, 28), 12],
[Date.UTC(2016, 10, 27), 12],
[Date.UTC(2016, 10, 26), 13],
[Date.UTC(2016, 10, 25), 12],
[Date.UTC(2016, 10, 24), 13],
[Date.UTC(2016, 10, 23), 12],
[Date.UTC(2016, 10, 22), 13],
[Date.UTC(2016, 10, 21), 12],
[Date.UTC(2016, 10, 20), 14],
[Date.UTC(2016, 10, 19), 11],
[Date.UTC(2016, 10, 18), 13],
[Date.UTC(2016, 10, 17), 12],
[Date.UTC(2016, 10, 16), 11],
[Date.UTC(2016, 10, 15), 12],
[Date.UTC(2016, 10, 14), 12],
[Date.UTC(2016, 10, 13), 11],
[Date.UTC(2016, 10, 12), 13],
[Date.UTC(2016, 10, 11), 13],
[Date.UTC(2016, 10, 10), 13],
[Date.UTC(2016, 10, 09), 12],
[Date.UTC(2016, 10, 08), 14],
[Date.UTC(2016, 10, 07), 14],
[Date.UTC(2016, 10, 06), 14],
[Date.UTC(2016, 10, 05), 13],
[Date.UTC(2016, 10, 04), 14],
[Date.UTC(2016, 10, 03), 14]
        ],
        color: "rgba(45, 118, 188, 0.498039)",
        yAxis: 0,
    }],
    "Info1": "6.3 mins",
    "SubInfo1": "up by 2%"
};

////Dummy data for Task: Loading
//var taskLoadingLineGraphDataJson = {
//    "SeriesData": [{
//        name: 'Chain Loading Data',
//        data: [
//            [Date.UTC(1970, 9, 21), 0],
//            [Date.UTC(1970, 10, 4), 0.28],
//            [Date.UTC(1971, 10, 9), 0.25],
//            [Date.UTC(1971, 10, 27), 0.2],
//            [Date.UTC(1972, 11, 2), 0.28],
//            [Date.UTC(1972, 11, 26), 0.28],
//            [Date.UTC(1973, 11, 29), 0.47],
//            [Date.UTC(1973, 0, 11), 0.79],
//            [Date.UTC(1974, 0, 26), 0.72],
//            [Date.UTC(1974, 1, 3), 1.02],
//            [Date.UTC(1975, 1, 11), 1.12],
//            [Date.UTC(1975, 1, 25), 1.2],
//            [Date.UTC(1976, 2, 11), 1.18],
//            [Date.UTC(1976, 3, 11), 1.19],
//            [Date.UTC(1977, 4, 1), 1.85],
//            [Date.UTC(1977, 4, 5), 2.22],
//            [Date.UTC(1978, 4, 19), 1.15],
//            [Date.UTC(1978, 5, 3), 0]
//        ]
//    }],
//    "fileName": "File Format",
//    "fileType": "csv",
//    "Info1": "6.3 mins",
//    "SubInfo1": "up by 2%"
//};


//section 1 of loading page
var taskLoadingLineGraphDataJson = {
    "SeriesData": [{
        name: 'Chain Loading Data',
        data: [
            [Date.UTC(1970, 9, 21), 0],
            [Date.UTC(1970, 10, 4), 0.28],
            [Date.UTC(1971, 10, 9), 0.25],
            [Date.UTC(1971, 10, 27), 0.2],
            [Date.UTC(1972, 11, 2), 0.28],
            [Date.UTC(1972, 11, 26), 0.28],
            [Date.UTC(1973, 11, 29), 0.47],
            [Date.UTC(1973, 0, 11), 0.79],
            [Date.UTC(1974, 0, 26), 0.72],
            [Date.UTC(1974, 1, 3), 1.02],
            [Date.UTC(1975, 1, 11), 1.12],
            [Date.UTC(1975, 1, 25), 1.2],
            [Date.UTC(1976, 2, 11), 1.18],
            [Date.UTC(1976, 3, 11), 1.19],
            [Date.UTC(1977, 4, 1), 1.85],
            [Date.UTC(1977, 4, 5), 2.22],
            [Date.UTC(1978, 4, 19), 1.15],
            [Date.UTC(1978, 5, 3), 0]
        ]
    }],
    "fileName": "File Format",
    "fileType": "csv",
    "Info1": "6.3 mins",
    "SubInfo1": "up by 2%",
    "TotalTimeTaken": "Total Time Taken"
};




//section 2 of loading page
var taskLoadingLineGraphDataJsonArray = {
       
    "Tabs"  :[{   
        "name": "Equity_namr.out",
        "data":    {
            "SeriesData": [{
                name: 'Chart1',
                data: [
                    [Date.UTC(1970, 9, 21), 0],
                    [Date.UTC(1970, 10, 4), 0.28],
                    [Date.UTC(1971, 10, 9), 0.25],
                    [Date.UTC(1971, 10, 27), 0.2],
                    [Date.UTC(1972, 11, 2), 0.28],
                    [Date.UTC(1972, 11, 26), 0.28],
                    [Date.UTC(1973, 11, 29), 0.47],
                    [Date.UTC(1973, 0, 11), 0.79],
                    [Date.UTC(1974, 0, 26), 0.72],
                    [Date.UTC(1974, 1, 3), 1.02],
                    [Date.UTC(1975, 1, 11), 1.12],
                    [Date.UTC(1975, 1, 25), 1.2],
                    [Date.UTC(1976, 2, 11), 1.18],
                    [Date.UTC(1976, 3, 11), 1.19],
                    [Date.UTC(1977, 4, 1), 1.85],
                    [Date.UTC(1977, 4, 5), 2.22],
                    [Date.UTC(1978, 4, 19), 1.15],
                    [Date.UTC(1978, 5, 3), 0]
                ]
            }],
            "fileSizeName": "File Size",
            "fileSize": "130 kb",
            "fileSizeStatus": "down by",
            "fileSizePerc": "2%"
        }
    },
    {
        "name": "Equity_lamr.out",
        "data":    {
            "SeriesData": [{
                name: 'Chart2',
                data: [
                    [Date.UTC(1970, 9, 21), 0],
                    [Date.UTC(1970, 10, 4), 0.28],
                    [Date.UTC(1971, 10, 9), 0.25],
                    [Date.UTC(1971, 10, 27), 0.2],
                    [Date.UTC(1972, 11, 2), 0.28],
                    [Date.UTC(1972, 11, 26), 0.28],
                    [Date.UTC(1973, 11, 29), 0.47],
                    [Date.UTC(1973, 0, 11), 0.79],
                    [Date.UTC(1974, 0, 26), 0.72],
                    [Date.UTC(1974, 1, 3), 1.02],
                    [Date.UTC(1975, 1, 11), 1.12],
                    [Date.UTC(1975, 1, 25), 1.2],
                    [Date.UTC(1976, 2, 11), 1.18],
                    [Date.UTC(1976, 3, 11), 1.19],
                    [Date.UTC(1977, 4, 1), 1.85],
                    [Date.UTC(1977, 4, 5), 2.22],
                    [Date.UTC(1978, 4, 19), 1.15],
                    [Date.UTC(1978, 5, 3), 0]
                ]
            }],
            "fileSizeName": "File Size",
            "fileSize": "130 kb",
            "fileSizeStatus": "down by",
            "fileSizePerc": "2%"
        }
    },
    {
        "name": "Equity_euro.out",
        "data":    {
            "SeriesData": [{
                name: 'Chart3',
                data: [
                    [Date.UTC(1970, 9, 21), 0],
                    [Date.UTC(1970, 10, 4), 0.28],
                    [Date.UTC(1971, 10, 9), 0.25],
                    [Date.UTC(1971, 10, 27), 0.2],
                    [Date.UTC(1972, 11, 2), 0.28],
                    [Date.UTC(1972, 11, 26), 0.28],
                    [Date.UTC(1973, 11, 29), 0.47],
                    [Date.UTC(1973, 0, 11), 0.79],
                    [Date.UTC(1974, 0, 26), 0.72],
                    [Date.UTC(1974, 1, 3), 1.02],
                    [Date.UTC(1975, 1, 11), 1.12],
                    [Date.UTC(1975, 1, 25), 1.2],
                    [Date.UTC(1976, 2, 11), 1.18],
                    [Date.UTC(1976, 3, 11), 1.19],
                    [Date.UTC(1977, 4, 1), 1.85],
                    [Date.UTC(1977, 4, 5), 2.22],
                    [Date.UTC(1978, 4, 19), 1.15],
                    [Date.UTC(1978, 5, 3), 0]
                ]
            }],
            "fileSizeName": "File Size",
            "fileSize": "130 kb",
            "fileSizeStatus": "down by",
            "fileSizePerc": "2%"
        }
    },
    {
        "name": "Equity_asia1.out",
        "data":    {
            "SeriesData": [{
                name: 'Chart4',
                data: [
                    [Date.UTC(1970, 9, 21), 0],
                    [Date.UTC(1970, 10, 4), 0.28],
                    [Date.UTC(1971, 10, 9), 0.25],
                    [Date.UTC(1971, 10, 27), 0.2],
                    [Date.UTC(1972, 11, 2), 0.28],
                    [Date.UTC(1972, 11, 26), 0.28],
                    [Date.UTC(1973, 11, 29), 0.47],
                    [Date.UTC(1973, 0, 11), 0.79],
                    [Date.UTC(1974, 0, 26), 0.72],
                    [Date.UTC(1974, 1, 3), 1.02],
                    [Date.UTC(1975, 1, 11), 1.12],
                    [Date.UTC(1975, 1, 25), 1.2],
                    [Date.UTC(1976, 2, 11), 1.18],
                    [Date.UTC(1976, 3, 11), 1.19],
                    [Date.UTC(1977, 4, 1), 1.85],
                    [Date.UTC(1977, 4, 5), 2.22],
                    [Date.UTC(1978, 4, 19), 1.15],
                    [Date.UTC(1978, 5, 3), 0]
                ]
            }],
            "fileSizeName": "File Size",
            "fileSize": "130 kb",
            "fileSizeStatus": "down by",
            "fileSizePerc": "2%"
        }
    },
    {
        "name": "Equity_asia2.out",
        "data":    {
            "SeriesData": [{
                name: 'Chart5',
                data: [
                    [Date.UTC(1970, 9, 21), 0],
                    [Date.UTC(1970, 10, 4), 0.28],
                    [Date.UTC(1971, 10, 9), 0.25],
                    [Date.UTC(1971, 10, 27), 0.2],
                    [Date.UTC(1972, 11, 2), 0.28],
                    [Date.UTC(1972, 11, 26), 0.28],
                    [Date.UTC(1973, 11, 29), 0.47],
                    [Date.UTC(1973, 0, 11), 0.79],
                    [Date.UTC(1974, 0, 26), 0.72],
                    [Date.UTC(1974, 1, 3), 1.02],
                    [Date.UTC(1975, 1, 11), 1.12],
                    [Date.UTC(1975, 1, 25), 1.2],
                    [Date.UTC(1976, 2, 11), 1.18],
                    [Date.UTC(1976, 3, 11), 1.19],
                    [Date.UTC(1977, 4, 1), 1.85],
                    [Date.UTC(1977, 4, 5), 2.22],
                    [Date.UTC(1978, 4, 19), 1.15],
                    [Date.UTC(1978, 5, 3), 0]
                ]
            }],
            "fileSizeName": "File Size",
            "fileSize": "130 kb",
            "fileSizeStatus": "down by",
            "fileSizePerc": "2%"
        }
    },
    ]};   


var taskLoadingColumnGraphDataJson = {
    "SeriesData": [{
        name: 'John',
        data: [5, 3, 4, 7, 2]
    }, {
        name: 'Jane',
        data: [2, 2, 3, 2, 1]
    }, {
        name: 'Joe',
        data: [3, 4, 4, 2, 5]
    }],
    "Info1": "500",
    "SubInfo1": ""
};


var columnGraphDataJson = {
    "SeriesData":[{
        name: 'John',
        data: [5, 3, 4, 7, 2]
    }, {
        name: 'Jane',
        data: [2, 2, 3, 2, 1]
    }, {
        name: 'Joe',
        data: [3, 4, 4, 2, 5]
    }],
    "Info1": "500",
    "SubInfo1": ""
};

var chainChartConfigJson = {
    "Info": [{
        "Text": "Total Time Taken"
    }],
    "ChartType": "Line",
    "Title": "",
    "SubTitle": "",
    "YAxisText": "Time taken(min)",
    "LegendLayout": "vertical",
    "LegendAlign": "center",
    "LegendVerticalAlign": "top",
    "SeriesName": "Time",
    "PlotStartPoint": "2010"
};


var taskLoadingChartConfigJson = {
    "Info": [{
        "Text": "Total Time Taken..."
    }],
    "ChartType": "Line",
    "Title": "",
    "SubTitle": "",
    "YAxisText": "Time taken(min)",
    "LegendLayout": "vertical",
    "LegendAlign": "center",
    "LegendVerticalAlign": "top",
    "SeriesName": "Time",
    "PlotStartPoint": "2010"
};

var taskLoadingStackedChartConfigJson = {
    "Info": [{
        "Text": "Sample Stacked Chart"
    }],
    "ChartType": "Column",
    "Title": "",
    "SubTitle": "",
    "YAxisText": "Time taken(min)",
    "LegendLayout": "vertical",
    "LegendAlign": "center",
    "LegendVerticalAlign": "top",
    "SeriesName": "Time",
    "PlotStartPoint": "2010"
};
    
//Update Core Graphs

var taskUpdateCoreChartConfigJson = {
    "Info": [{
        "Text": "Total Time Taken..."
    }],
    "ChartType": "Line",
    "Title": "",
    "SubTitle": "",
    "YAxisText": "Time taken(min)",
    "LegendLayout": "vertical",
    "LegendAlign": "center",
    "LegendVerticalAlign": "top",
    "SeriesName": "Time",
    "PlotStartPoint": "2010",
    "PrioritisationUpdated": "true",
    "FeedMappingUpdated": "true"
};

var taskUpdateCoreTabsGraphConfigJson = {
    "Info": [{
        "Text": "Total Time Taken..."
    }],
    "ChartType": "Line",
    "Title": "",
    "SubTitle": "",
    "YAxisText": "Time taken(min)",
    "LegendLayout": "vertical",
    "LegendAlign": "center",
    "LegendVerticalAlign": "top",
    "SeriesName": "Time",
    "PlotStartPoint": "2010"
};

//section 1 update core

var taskUpdateCoreLineGraph1DataJson = {
    "SeriesData": [{
        name: 'Chain Loading Data',
        data: [
            [Date.UTC(1970, 9, 21), 0],
            [Date.UTC(1970, 10, 4), 0.28],
            [Date.UTC(1971, 10, 9), 0.25],
            [Date.UTC(1971, 10, 27), 0.2],
            [Date.UTC(1972, 11, 2), 0.28],
            [Date.UTC(1972, 11, 26), 0.28],
            [Date.UTC(1973, 11, 29), 0.47],
            [Date.UTC(1973, 0, 11), 0.79],
            [Date.UTC(1974, 0, 26), 0.72],
            [Date.UTC(1974, 1, 3), 1.02],
            [Date.UTC(1975, 1, 11), 1.12],
            [Date.UTC(1975, 1, 25), 1.2],
            [Date.UTC(1976, 2, 11), 1.18],
            [Date.UTC(1976, 3, 11), 1.19],
            [Date.UTC(1977, 4, 1), 1.85],
            [Date.UTC(1977, 4, 5), 2.22],
            [Date.UTC(1978, 4, 19), 1.15],
            [Date.UTC(1978, 5, 3), 0]
        ]
    }],
    "info": "Total Time Taken",
    "SubInfo1": "6.3 mins",
    "SubInfo2": "(up by 2%)"
};

//section 2 update core

var taskUpdateCoreLineGraph2DataJson = {
    "SeriesData": [{
        name: 'Chain Loading Data',
        data: [
            [Date.UTC(1970, 9, 21), 0],
            [Date.UTC(1970, 10, 4), 0.28],
            [Date.UTC(1971, 10, 9), 0.25],
            [Date.UTC(1971, 10, 27), 0.2],
            [Date.UTC(1972, 11, 2), 0.28],
            [Date.UTC(1972, 11, 26), 0.28],
            [Date.UTC(1973, 11, 29), 0.47],
            [Date.UTC(1973, 0, 11), 0.79],
            [Date.UTC(1974, 0, 26), 0.72],
            [Date.UTC(1974, 1, 3), 1.02],
            [Date.UTC(1975, 1, 11), 1.12],
            [Date.UTC(1975, 1, 25), 1.2],
            [Date.UTC(1976, 2, 11), 1.18],
            [Date.UTC(1976, 3, 11), 1.19],
            [Date.UTC(1977, 4, 1), 1.85],
            [Date.UTC(1977, 4, 5), 2.22],
            [Date.UTC(1978, 4, 19), 1.15],
            [Date.UTC(1978, 5, 3), 0]
        ]
    }],
    "info": "Securities updated",
    "SubInfo1": "5689",
    "SubInfo2": "(up by 2%)"
};

//section 3 update core
var columnExceptionGraphDataJson =  
{
    "SeriesData": [{
        "type": "column",
        "name": "Alert",
        "stacking": "normal",
        "data": [
			[1490054400000, 5],
			[1490140800000, 5],
			[1490227200000, 5],
			[1490313600000, 5],
			[1490400000000, 3],
			[1490486400000, 4],
			[1490572800000, 5],
			[1490659200000, 5],
			[1490745600000, 5],
			[1490832000000, 5],
			[1490918400000, 5],
			[1491004800000, 3],
			[1491091200000, 12],
			[1491177600000, 5],
			[1491264000000, 5],
			[1491350400000, 5],
			[1491436800000, 5],
			[1491523200000, 5],
			[1491609600000, 3],
			[1491696000000, 5],
			[1491782400000, 2],
			[1491868800000, 2],
			[1491955200000, 2],
			[1492041600000, 12],
			[1492128000000, 5],
			[1492214400000, 5],
			[1492300800000, 5],
			[1492387200000, 5],
			[1492473600000, 5],
			[1492560000000, 3],
			[1492646400000, 5]
		],
        "yAxis": 0
    }, {
        "type": "column",
        "name": "Custom Exception",
        "stacking": "normal",
        "data": [
			[1490054400000, 22],
			[1490140800000, 21],
			[1490227200000, 23],
			[1490313600000, 23],
			[1490400000000, 23],
			[1490486400000, 26],
			[1490572800000, 23],
			[1490659200000, 23],
			[1490745600000, 23],
			[1490832000000, 23],
			[1490918400000, 23],
			[1491004800000, 23],
			[1491091200000, 19],
			[1491177600000, 23],
			[1491264000000, 23],
			[1491350400000, 23],
			[1491436800000, 23],
			[1491523200000, 23],
			[1491609600000, 23],
			[1491696000000, 15],
			[1491782400000, 0],
			[1491868800000, 0],
			[1491955200000, 0],
			[1492041600000, 19],
			[1492128000000, 23],
			[1492214400000, 23],
			[1492300800000, 23],
			[1492387200000, 23],
			[1492473600000, 23],
			[1492560000000, 23],
			[1492646400000, 15]
		],
        "yAxis": 0
    }, {
        "type": "column",
        "name": "Invalid Data Exception",
        "stacking": "normal",
        "data": [
			[1490054400000, 0],
			[1490140800000, 0],
			[1490227200000, 0],
			[1490313600000, 0],
			[1490400000000, 0],
			[1490486400000, 0],
			[1490572800000, 0],
			[1490659200000, 0],
			[1490745600000, 0],
			[1490832000000, 0],
			[1490918400000, 0],
			[1491004800000, 0],
			[1491091200000, 0],
			[1491177600000, 0],
			[1491264000000, 0],
			[1491350400000, 0],
			[1491436800000, 0],
			[1491523200000, 0],
			[1491609600000, 0],
			[1491696000000, 0],
			[1491782400000, 0],
			[1491868800000, 0],
			[1491955200000, 0],
			[1492041600000, 0],
			[1492128000000, 0],
			[1492214400000, 0],
			[1492300800000, 0],
			[1492387200000, 0],
			[1492473600000, 0],
			[1492560000000, 0],
			[1492646400000, 0]
		],
        "yAxis": 0
    }, {
        "type": "column",
        "name": "Reference Data Missing",
        "stacking": "normal",
        "data": [
			[1490054400000, 105],
			[1490140800000, 95],
			[1490227200000, 98],
			[1490313600000, 100],
			[1490400000000, 23],
			[1490486400000, 24],
			[1490572800000, 95],
			[1490659200000, 94],
			[1490745600000, 88],
			[1490832000000, 88],
			[1490918400000, 97],
			[1491004800000, 20],
			[1491091200000, 25],
			[1491177600000, 133],
			[1491264000000, 126],
			[1491350400000, 132],
			[1491436800000, 142],
			[1491523200000, 132],
			[1491609600000, 23],
			[1491696000000, 45],
			[1491782400000, 126],
			[1491868800000, 126],
			[1491955200000, 126],
			[1492041600000, 109],
			[1492128000000, 113],
			[1492214400000, 115],
			[1492300800000, 123],
			[1492387200000, 119],
			[1492473600000, 121],
			[1492560000000, 129],
			[1492646400000, 120]
		],
        "yAxis": 0
    }, {
        "type": "column",
        "name": "Underlying Security Missing",
        "stacking": "normal",
        "data": [
			[1490054400000, 0],
			[1490140800000, 0],
			[1490227200000, 0],
			[1490313600000, 0],
			[1490400000000, 0],
			[1490486400000, 0],
			[1490572800000, 0],
			[1490659200000, 0],
			[1490745600000, 0],
			[1490832000000, 0],
			[1490918400000, 0],
			[1491004800000, 0],
			[1491091200000, 0],
			[1491177600000, 0],
			[1491264000000, 0],
			[1491350400000, 0],
			[1491436800000, 0],
			[1491523200000, 1],
			[1491609600000, 0],
			[1491696000000, 0],
			[1491782400000, 1],
			[1491868800000, 1],
			[1491955200000, 1],
			[1492041600000, 0],
			[1492128000000, 0],
			[1492214400000, 0],
			[1492300800000, 0],
			[1492387200000, 0],
			[1492473600000, 0],
			[1492560000000, 1],
			[1492646400000, 1]
		],
        "yAxis": 0
    }, {
        "type": "column",
        "name": "Value Changed",
        "stacking": "normal",
        "data": [
			[1490054400000, 249],
			[1490140800000, 33],
			[1490227200000, 10],
			[1490313600000, 14],
			[1490400000000, 0],
			[1490486400000, 0],
			[1490572800000, 8],
			[1490659200000, 10],
			[1490745600000, 9],
			[1490832000000, 18],
			[1490918400000, 11],
			[1491004800000, 0],
			[1491091200000, 0],
			[1491177600000, 28],
			[1491264000000, 12],
			[1491350400000, 16],
			[1491436800000, 16],
			[1491523200000, 30],
			[1491609600000, 0],
			[1491696000000, 0],
			[1491782400000, 12],
			[1491868800000, 12],
			[1491955200000, 13],
			[1492041600000, 12],
			[1492128000000, 14],
			[1492214400000, 12],
			[1492300800000, 15],
			[1492387200000, 12],
			[1492473600000, 13],
			[1492560000000, 12],
			[1492646400000, 36]
		],
        "yAxis": 0
    }],
    "Info1": "500",
    "info": "Exception Generated",
    "SubInfo1": "568",
    "SubInfo2": "(up by 2%)"
}

//section 4 tab data



var taskUpdateCoreLineGraphDataJsonArray = {

    "Tabs": [{
        "name": "Security Data Feed",
        "data": {
            "SeriesData": [{
                name: 'Chart1',
                data: [{
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
            }],
            "fileSizeName": "File Size",
            "fileSize": "130 kb",
            "fileSizeStatus": "down by",
            "fileSizePerc": "2%"
        }
    },
    {
        "name": "Pricing Feed",
        "data": {
            "SeriesData": [{
                name: 'Chart2',
                data: [{
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
            }],
            "fileSizeName": "File Size",
            "fileSize": "130 kb",
            "fileSizeStatus": "down by",
            "fileSizePerc": "2%"
        }
    }
    ]
};