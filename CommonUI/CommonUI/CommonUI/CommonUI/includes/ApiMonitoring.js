var apimonitoring = (function () {

    function ApiMonitoring() {

        this._controlIdInfo = null;

        this._pageViewModelInstance = null; // //       NOT GETTING

        //this._chainIndex = 0;p

        this._iosocket = null;
        this._firstTimeConnectionToNode = true;

        this._selectedUrl = [];
        this._selectedMethod = [];
        this._selectedClientMachine = [];

        this._totalUrl = [];
        this._totalMethod = [];
        this._totalClientMachine = [];

        this._selectedStartDate = "";
        this._selectedEndDate = "";

        this._isMonitoring = false;

        this._selectedAPIUniqueId = -1;

        this._RequestBodyFileContent = "";
        this._ResponseBodyFileContent = "";

        this._RequestBodyFileFullPath = "";
        this._ResponseBodyFileFullPath = "";

        this._RequestBodyFileName = "";
        this._ResponseBodyFileName = "";

        this._ResponseViewLogContent = "";

        this._isDDLBindFirstTime = true;

        //this._RequestBodyFileContentType = "";
        //this._ResponseBodyFileContentType = "";

        //this._UrlData = [];
        //this._MethodData = [];
        //this._ClientMachineData = [];
    }

    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    var apimonitoring = new ApiMonitoring();

    function pageViewModel(data) {
        var self = this;

        //Selected Item
        self.selectedChainItem = null;

        self.chainListing = ko.observableArray();

        if (typeof (data) != 'undefined') {
            //No API Calls For Current Filters
            if (typeof (window.parent.leftMenu) !== "undefined") {
                if (data.length === 0) {
                    $(".ApiMonitoringMain").hide();
                    window.parent.leftMenu.showNoRecordsMsg("No data found matching your search criteria.", $("#ApiMonitoringMainErrorDiv"));

                    //CSS for smNoRecordsContainer div created inside ApiMonitoringMainErrorDiv div
                    var pd_left = ($("#ApiMonitoringMainErrorDiv").width() - $("#smNoRecordsContainer").width() - 2) / 2;
                    var pd_top = (($("#ApiMonitoringMainErrorDiv").height() - $("#smNoRecordsContainer").height() - 2) / 2) - 20;
                    $("#ApiMonitoringMainErrorDiv #smNoRecordsContainer").css({ "padding-left": pd_left, "padding-top": pd_top });

                }
                else {
                    $(".ApiMonitoringMain").show();
                    window.parent.leftMenu.hideNoRecordsMsg();

                    for (var item in data) {
                        self.chainListing.push(new chainViewModel(data[item]));//, item));
                    }

                    $(".ApiMonitoringChain").css("display", "block");

                }
            }


            ////No API Calls For Current Filters
            //if (data.length > 0) {

            //    $(".ApiMonitoringMain").css({ "display": "block" });
            //    $("#ApiMonitoringMainErrorDiv").css({ "display": "none" });

            //    for (var item in data) {
            //        self.chainListing.push(new chainViewModel(data[item]));//, item));
            //    }
            //}
            //else {

            //    $(".ApiMonitoringMain").css({ "display": "none" });
            //    $("#ApiMonitoringMainErrorDiv").css({ "display": "block" });

            //    //$(".ApiMonitoringChainParentDiv").text("Sorry, we couldn't find any matches. Please try again.");
            //}

        }

    };

    function chainViewModel(data) {
        var self = this;

        self.isSelected = ko.observable(false);

        self.chainApiUniqueId = data.ApiUniqueId;
        self.chainName = data.ChainName;
        self.chainURL = data.ChainURL;
        self.chainMethod = data.ChainMethod;
        self.chainClientMachine = data.ChainClientMachine;
        self.chainClientIP = data.ChainClientIP;
        self.chainPort = data.ChainPort;
        self.threadId = data.ThreadId;

        //Change Timezone to Client Machine's Timezone
        //Date Culture Change
        //self.chainStartDateTime = new Date(data.ChainStartDateTime + " UTC").format('MM/dd/yyyy tfh:nn:ss.fff');
        //self.chainEndDateTime = ko.observable(new Date(data.ChainEndDateTime + " UTC").format('MM/dd/yyyy tfh:nn:ss.fff'));
        self.chainStartDateTime = new Date(data.ChainStartDateTime + " UTC").format(com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate) + ' tfh:nn:ss.fff');
        self.chainEndDateTime = ko.observable(new Date(data.ChainEndDateTime + " UTC").format(com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate) + ' tfh:nn:ss.fff'));

        //self.chainStartDateTime = data.ChainStartDateTime;
        //self.chainEndDateTime = ko.observable(data.ChainEndDateTime);

        self.chainTimeTaken = ko.computed(function () {
            var displayString = '';
            if (self.chainEndDateTime() != "NA") {

                displayString += 'Duration : ';

                //Hour
                if (data.ChainTimeTaken[0] != 0) {
                    displayString += data.ChainTimeTaken[0];
                    displayString += (data.ChainTimeTaken[0] == 1) ? ' hr ' : ' hrs ';
                }

                //Minutes
                if (data.ChainTimeTaken[1] != 0) {
                    displayString += data.ChainTimeTaken[1];
                    displayString += (data.ChainTimeTaken[1] == 1) ? ' min ' : ' mins ';
                }

                //Seconds
                if (data.ChainTimeTaken[2] != 0) {
                    displayString += data.ChainTimeTaken[2];
                    displayString += (data.ChainTimeTaken[2] == 1) ? ' sec ' : ' secs ';
                }

                //Milli Seconds
                if (data.ChainTimeTaken[3] != 0) {
                    displayString += data.ChainTimeTaken[3];
                    displayString += (data.ChainTimeTaken[3] == 1) ? ' ms' : ' ms';
                }
            }
            return displayString;

        });

        self.chainDateTimeDisplayString = ko.computed(function () {
            var displayString = '';

            if (self.chainEndDateTime() != "NA") {
                displayString = self.chainStartDateTime + ' to ' + self.chainEndDateTime();
            }
            else {
                displayString = 'Requested At : ' + self.chainStartDateTime;
            }
            return displayString;
        });

        self.chainDetailsDataFormatRequest = data.ChainDetailsDataFormatRequest;
        self.chainDetailsDataFormatResponse = data.ChainDetailsDataFormatResponse;

        self.chainDetailsData = data.ChainDetailsData;

        self.chainBackgroundColor = ko.computed(function () {
            //return self.isSelected() ? '#72b9e6' : '#ffffff';
            //return self.isSelected() ? '#72b9e6' : 'url("css/images/chain_texture.png")';
            return self.isSelected() ? '#72b9e6' : '#f6f6f6';
        });

        self.chainURLColor = ko.computed(function () {
            return self.isSelected() ? '#ffffff' : '#72b9e6';
        });

        self.chainTextColor = ko.computed(function () {
            return self.isSelected() ? '#ffffff' : '#000000';
        });

        // STATE
        ////Check why state is not binding.
        //self.state = data.ChainState;
        ////self.chainDetailsData = data.ChainDetailsData;
        //self.chainStatusIconStyle = ko.computed(function () {       //modifies text style depending upon state
        //    var state = '#9eb395';  //For Success
        //    if (self.state == 'Other') {
        //        state = '#eab671'
        //    }
        //    return state;
        //});
        //self.chainInfoBackgroundColor = ko.computed(function () {       //modifies backgrnd style depending upon state
        //    var state = '#eef3ec';  //For Success 
        //    if (self.state == 'Other')
        //        state = '#f0ebe7'
        //    return state;
        //});
    };

    function chainViewModelNode(data) {
        var self = this;

        self.isSelected = ko.observable(false);

        self.chainApiUniqueId = data.ApiUniqueId;
        self.chainName = data.ChainName;
        self.chainURL = data.ChainURL;
        self.chainMethod = data.ChainMethod;
        self.chainClientMachine = data.ChainClientMachine;
        self.chainClientIP = data.ChainClientIP;
        self.chainPort = data.ChainPort;
        self.threadId = data.ThreadId;

        //Change Timezone to Client Machine's Timezone
        //Date Culture Change
        //self.chainStartDateTime = new Date(data.ChainStartDateTime + " UTC").format('MM/dd/yyyy tfh:nn:ss.fff');
        self.chainStartDateTime = new Date(data.ChainStartDateTime + " UTC").format(com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate) + ' tfh:nn:ss.fff');
        
        //self.chainStartDateTime = data.ChainStartDateTime;

        self.chainEndDateTime = ko.observable("");

        self.chainTimeTaken = ko.observable("");

        self.chainDateTimeDisplayString = ko.observable('Requested At : ' + self.chainStartDateTime);

        self.chainDetailsDataFormatRequest = data.ChainDetailsDataFormatRequest;
        self.chainDetailsDataFormatResponse = "APICNO";

        self.chainDetailsData = {};
        self.chainDetailsData.RequestHeaderData = data.ChainDetailsData.RequestHeaderData;
        self.chainDetailsData.RequestBodyDataFileLocation = data.ChainDetailsData.RequestBodyDataFileLocation;
        self.chainDetailsData.RequestBodyDataFileName = data.ChainDetailsData.RequestBodyDataFileName;
        //self.chainDetailsData.RequestBodyDataToShow = data.ChainDetailsData.RequestBodyDataToShow;
        self.chainDetailsData.RequestBodyErrorMsg = data.ChainDetailsData.RequestBodyErrorMsg;
        self.chainDetailsData.ResponseHeaderData = "APICNO";
        self.chainDetailsData.ResponseBodyDataFileLocation = "APICNO";
        self.chainDetailsData.ResponseBodyDataFileName = "APICNO";
        //self.chainDetailsData.ResponseBodyDataToShow = "APICNO";
        self.chainDetailsData.ResponseBodyErrorMsg = "APICNO";

        self.chainBackgroundColor = ko.computed(function () {
            //return self.isSelected() ? '#72b9e6' : '#ffffff';
            //return self.isSelected() ? '#72b9e6' : 'url("css/images/chain_texture.png")';
            return self.isSelected() ? '#72b9e6' : '#f6f6f6';
        });

        self.chainURLColor = ko.computed(function () {
            return self.isSelected() ? '#ffffff' : '#72b9e6';
        });

        self.chainTextColor = ko.computed(function () {
            return self.isSelected() ? '#ffffff' : '#000000';
        });

    };


    ///////////////////////////////////////////////
    //Callback Functions for COMMON SERVICE Calls//
    ///////////////////////////////////////////////

    // This function is called on Success Service Call on CLICK of Apply Filters Button
    function OnSuccess_LoadApiCallsDataInitial(result) {

        if (typeof ko !== 'undefined') {                                                                //if ko object exists, then ApiMonitoring is currently pageview

            //  
            //    NOTE : The "ApiMonitoringApiCallsData" in the line below is 
            //           the name of the variable inside "ApiMonitoringJson.js"
            //
            apimonitoring._pageViewModelInstance = new pageViewModel(result.d);

            ko.applyBindings(apimonitoring._pageViewModelInstance);

            //$("#ApiMonitoringChainSuperParentDiv").height($('#ApiMonitoringMainLeftColumn').height() - 78);            


            // First Chain Item is selected on load.
            $($('.ApiMonitoringChain')[0]).children('.ApiMonitoringChainInfo').click();
        }
    };

    // This function is called on Success Service Call on CLICK of Start Capturing Button
    function OnSuccess_LoadApiCallsDataLater(result) {

        // Empty the observable array and clear selected item
        apimonitoring._pageViewModelInstance.chainListing.removeAll();
        apimonitoring._pageViewModelInstance.selectedChainItem = null;
        $("#ApiMonitoringSelectedApiDetails").css({ 'display': 'none' });

        // Enter the data in observable array again
        if (typeof (result.d) != 'undefined') {

            //No API Calls For Current Filters
            if (typeof (window.parent.leftMenu) !== "undefined") {
                if (result.d.length === 0) {
                    $(".ApiMonitoringMain").hide();
                    window.parent.leftMenu.showNoRecordsMsg("No data found matching your search criteria.", $("#ApiMonitoringMainErrorDiv"));

                    //CSS for smNoRecordsContainer div created inside ApiMonitoringMainErrorDiv div
                    var pd_left = ($("#ApiMonitoringMainErrorDiv").width() - $("#smNoRecordsContainer").width() - 2) / 2;
                    var pd_top = (($("#ApiMonitoringMainErrorDiv").height() - $("#smNoRecordsContainer").height() - 2) / 2) - 20;
                    $("#ApiMonitoringMainErrorDiv #smNoRecordsContainer").css({ "padding-left": pd_left, "padding-top": pd_top });

                }
                else {
                    $(".ApiMonitoringMain").show();
                    window.parent.leftMenu.hideNoRecordsMsg();

                    for (var item in result.d) {
                        apimonitoring._pageViewModelInstance.chainListing.push(new chainViewModel(result.d[item]));
                    }

                    //$("#ApiMonitoringChainSuperParentDiv").height($('#ApiMonitoringMainLeftColumn').height() - 78);
                    $(".ApiMonitoringChain").css("display", "block");

                    // First Chain Item is selected on load.
                    $($('.ApiMonitoringChain')[0]).children('.ApiMonitoringChainInfo').click();

                }
            }

            ////No API Calls For Current Filters
            //if (result.d.length > 0) {

            //    $(".ApiMonitoringMain").css({ "display": "block" });
            //    $("#ApiMonitoringMainErrorDiv").css({ "display": "none" });

            //    for (var item in result.d) {
            //        apimonitoring._pageViewModelInstance.chainListing.push(new chainViewModel(result.d[item]));
            //    }
            //}
            //else {

            //    $(".ApiMonitoringMain").css({ "display": "none" });
            //    $("#ApiMonitoringMainErrorDiv").css({ "display": "block" });

            //    //$(".ApiMonitoringChainParentDiv").text("Sorry, we couldn't find any matches. Please try again.");
            //}
        }
    }

    // This function is called on Success Service Call on CLICK of View Logs Tab
    function OnSuccess_LoadApiCallLogData(result) {

        // Render the data on the screen
        if (typeof (result.d) != 'undefined') {

            var displayString = "";

            if (result.d.ApiCallLogItemsList.length > 0) {

                for (var item in result.d.ApiCallLogItemsList) {
                    var tempString = "";

                    //Date Culture Change
                    tempString += new Date(result.d.ApiCallLogItemsList[item].LogTime + " UTC").format(com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate) + ' tfh:nn:ss.fff');;
                    tempString += " : ";
                    tempString += result.d.ApiCallLogItemsList[item].LogMessage;

                    displayString += tempString;
                    displayString += "\n";
                }

                $("#ApiMonitoringSelectedApiDetailsContainer2CopyToClipboardIconBB").css('pointer-events', 'auto');
            }
            else {
                $("#ApiMonitoringSelectedApiDetailsContainer2CopyToClipboardIconBB").css('pointer-events', 'none');
                displayString += "No Logs for this API Call."
            }

            $("#ApiMonitoringSelectedApiDetailsContainer3Text").text(displayString);
        }

        //Show Response Header Content Div
        $('#ApiMonitoringSelectedApiDetailsResponseViewLogsContainer').css('display', 'block');

        //Add Selected Class to Header
        $('#ApiMonitoringSelectedApiDetailsResponseSubTabViewLogsContainer').addClass('SelectedSubTabItem');

        //Copy Data into Global Variable for Copy Button
        apimonitoring._ResponseViewLogContent = displayString;

        //Show ONLY Copy Icon
        $("#ApiMonitoringSelectedApiDetailsContainer2CopyToClipboardIconBB").css("display", "inline-block");
    }

    //This function is called if any Service Call returns an error
    function OnFailure(result) {
        $("#" + apimonitoring._controlIdInfo.LblErrorId).text("Service returned an error" + ((result.responseJSON == null || result.responseJSON.Message == null || result.responseJSON.Message.length == 0) ? "" : (" : " + result.responseJSON.Message)) + ((result.responseText == null || result.responseText.length == 0) ? "" : (" : " + result.responseText)));
        $find(apimonitoring._controlIdInfo.ModalErrorId).show();
    }

    /*
    //Edit this if we need to show Some Additional Details below the chain item itself.
    srmDQMStatus.onClickTask = function (obj, event) {
        $('.srmDQMChartArrow').css('top', $(event.target).offset().top);
        $('#sqmDQMChart').css('left', '550px');
        $('#sqmDQMChart').css('top', $($(event.target).parents('.srmDQMChain')[0]).offset().top);
        $('#sqmDQMChart').css('display', 'block');
        //dataQualityStatus.bindChart(obj);
        console.log(parseInt(obj.moduleId));
        //console.log(obj.moduleId);
        srmDQMStatus._protoObject[parseInt(obj.moduleId)].manageTaskClickEvent(1, 1, 1, srmDQMStatus._chartContainerId, obj);                    //RM/SM linked  //pass whole object or some part
    
    };
    */

    //function OnSuccess_LoadApiCallDetailsJSONData(result)
    //{
    //    //We hv to first Remove Class n CSS
    //    $("#ApiMonitoringSelectedApiDetails .ApiMonitoringSelectedApiDetailsContainerTextDivs:nth-child(2)").removeClass("scroll");
    //    $("#ApiMonitoringSelectedApiDetails .ApiMonitoringSelectedApiDetailsContainerTextDivs .renderjson:first-child").removeClass("scroll");
    //    $("#ApiMonitoringSelectedApiDetails .ApiMonitoringSelectedApiDetailsContainerTextDivs:nth-child(2)").css({ "overflow": "", "height": "", "width": "" });
    //    //REQUEST Object
    //    //Call the JSON Library here
    //    document.getElementById("ApiMonitoringSelectedApiDetailsContainer1Text").appendChild(renderjson
    //        .set_icons('+', '-')
    //        .set_show_to_level(2)               // Special Case : If we specify the String "all", then the entire JSON is displayed completely expanded.
    //        .set_max_string_length(100)(JSON.parse(result.d.ChainDetailsData.ChainDetailsRequestData)));
    //    //.set_max_string_length(100)([{ "a": "b" }, {"c":"d"}]));
    //    //RESPONSE Object
    //    //Call the JSON Library here
    //    document.getElementById("ApiMonitoringSelectedApiDetailsContainer2Text").appendChild(renderjson
    //        .set_icons('+', '-')
    //        .set_show_to_level(2)               // Special Case : If we specify the String "all", then the entire JSON is displayed completely expanded.
    //        .set_max_string_length(100)(JSON.parse(result.d.ChainDetailsData.ChainDetailsResponseData)));
    //    //.set_max_string_length(100)([{ "a": "b" }, {"c":"d"}]));
    //    $("#ApiMonitoringSelectedApiDetails").css("display", "block");
    //    //Apply Slim Scroll
    //    //We hv to first remove class
    //    //$("#ApiMonitoringSelectedApiDetails .ApiMonitoringSelectedApiDetailsContainerTextDivs:nth-child(2)").removeClass("scroll");
    //    //$("#ApiMonitoringSelectedApiDetails .ApiMonitoringSelectedApiDetailsContainerTextDivs .renderjson:first-child").removeClass("scroll");
    //    $("#ApiMonitoringSelectedApiDetails .ApiMonitoringSelectedApiDetailsContainerTextDivs .renderjson:first-child").addClass("scroll");
    //}

    function LoadBodyContentJSONData(idContainerDiv, bodyContent) {
        //We hv to first Remove Class n CSS
        $("#ApiMonitoringSelectedApiDetails #" + idContainerDiv + ":nth-child(2)").removeClass("scroll");
        $("#ApiMonitoringSelectedApiDetails #" + idContainerDiv + " .renderjson:first-child").removeClass("scroll");
        $("#ApiMonitoringSelectedApiDetails #" + idContainerDiv + ":nth-child(2)").css({ "overflow": "", "height": "", "width": "" });

        //Call the JSON Library here
        document.getElementById(idContainerDiv).appendChild(renderjson
            .set_icons('+', '-')
            .set_show_to_level(2)               // Special Case : If we specify the String "all", then the entire JSON is displayed completely expanded.
            .set_max_string_length(100)(JSON.parse(bodyContent)));
        //.set_max_string_length(100)([{ "a": "b" }, {"c":"d"}]));

        $("#ApiMonitoringSelectedApiDetails").css("display", "block");

        $("#ApiMonitoringSelectedApiDetails #" + idContainerDiv + " .renderjson:first-child").addClass("scroll");
    }

    //function OnSuccess_LoadApiCallDetailsXMLData(result)
    //{
    //    //We hv to first Remove Class n CSS
    //    $("#ApiMonitoringSelectedApiDetails .ApiMonitoringSelectedApiDetailsContainerTextDivs:nth-child(2)").removeClass("scroll");
    //    $("#ApiMonitoringSelectedApiDetails .ApiMonitoringSelectedApiDetailsContainerTextDivs .renderjson:first-child").removeClass("scroll");
    //    $("#ApiMonitoringSelectedApiDetails .ApiMonitoringSelectedApiDetailsContainerTextDivs:nth-child(2)").css({ "overflow": "", "height": "", "width": "" });
    //    //REQUEST Object
    //    //Call the XML Library here
    //    LoadXMLString('ApiMonitoringSelectedApiDetailsContainer1Text',
    //                    result.d.ChainDetailsData.ChainDetailsRequestData,
    //                    1       //Here 1 is the number of levels, we want expanded initially.
    //    );        
    //    //RESPONSE Object
    //    //Call the XML Library here
    //    LoadXMLString('ApiMonitoringSelectedApiDetailsContainer2Text',
    //                    result.d.ChainDetailsData.ChainDetailsResponseData,
    //                    1       //Here 1 is the number of levels, we want expanded initially.
    //    );
    //    $("#ApiMonitoringSelectedApiDetails").css("display", "block");
    //    //Apply Slim Scroll
    //    $("#ApiMonitoringSelectedApiDetails .ApiMonitoringSelectedApiDetailsContainerTextDivs:nth-child(2)").addClass("scroll");
    //    $("#ApiMonitoringSelectedApiDetails .ApiMonitoringSelectedApiDetailsContainerTextDivs:nth-child(2)").css({"overflow": "auto", "height": "270px", "width" : "90%"});
    //}

    function LoadBodyContentXMLData(idContainerDiv, bodyContent) {
        //We hv to first Remove Class n CSS
        $("#ApiMonitoringSelectedApiDetails #" + idContainerDiv + ":nth-child(2)").removeClass("scroll");
        $("#ApiMonitoringSelectedApiDetails #" + idContainerDiv + " .renderjson:first-child").removeClass("scroll");
        $("#ApiMonitoringSelectedApiDetails #" + idContainerDiv + ":nth-child(2)").css({ "overflow": "", "height": "", "width": "" });

        //REQUEST Object
        //Call the XML Library here
        SM_LoadXMLString(idContainerDiv,
                        bodyContent,
                        1       //Here 1 is the number of levels, we want expanded initially.
        );

        $("#ApiMonitoringSelectedApiDetails").css("display", "block");

        //Apply Slim Scroll
        $("#ApiMonitoringSelectedApiDetails #" + idContainerDiv + ":nth-child(2)").addClass("scroll");
        $("#ApiMonitoringSelectedApiDetails #" + idContainerDiv + ":nth-child(2)").css({ "overflow": "auto", "height": "270px", "width": "100%" });

        $("#ApiMonitoringSelectedApiDetails #" + idContainerDiv + " div:nth-child(2)").css({ "left": "0px" });
    }

    //Function to Load Request/Response Body Content
    function OnSuccess_LoadApiCallDetailsBodyContent(result) {

        if (result.d.RequestOrResponse == "Request") {

            if (result.d.BodyDataToShow) {
                //Store String For Copy
                apimonitoring._RequestBodyFileContent = result.d.BodyFileContent;

                if (result.d.DataFormat == "JSON") {
                    LoadBodyContentJSONData('ApiMonitoringSelectedApiDetailsContainer1Text', result.d.BodyFileContent);
                }
                else if (result.d.DataFormat == "XML") {
                    LoadBodyContentXMLData('ApiMonitoringSelectedApiDetailsContainer1Text', result.d.BodyFileContent);
                }

                //Enable Copy Icon for Request Body
                $("#ApiMonitoringSelectedApiDetailsContainer1CopyToClipboardIcon").css('pointer-events', 'auto');

            }
            else {
                $('#ApiMonitoringSelectedApiDetailsContainer1Text').text(result.d.BodyErrorMsg);

                //Disable Copy Icon for Request Body
                $("#ApiMonitoringSelectedApiDetailsContainer1CopyToClipboardIcon").css('pointer-events', 'none');

                //Can Highlight the Download Icon

            }

            if (result.d.FileExist) {
                //Enable Download Icon for Request Body and set Global Variables for the File
                $("#ApiMonitoringSelectedApiDetailsContainer1DownloadIcon").css('pointer-events', 'auto');
                //apimonitoring._RequestBodyFileFullPath = obj.chainDetailsData.RequestBodyDataFileLocation;
                //apimonitoring._RequestBodyFileName = obj.chainDetailsData.RequestBodyDataFileName;
            }
            else {
                $("#ApiMonitoringSelectedApiDetailsContainer1DownloadIcon").css('pointer-events', 'none');
                apimonitoring._RequestBodyFileFullPath = "";
                apimonitoring._RequestBodyFileName = "";
            }
        }
        else if (result.d.RequestOrResponse == "Response") {

            if (result.d.BodyDataToShow) {
                //Store String For Copy
                apimonitoring._ResponseBodyFileContent = result.d.BodyFileContent;

                if (result.d.DataFormat == "JSON") {
                    LoadBodyContentJSONData('ApiMonitoringSelectedApiDetailsContainer2Text', result.d.BodyFileContent);
                }
                else if (result.d.DataFormat == "XML") {
                    LoadBodyContentXMLData('ApiMonitoringSelectedApiDetailsContainer2Text', result.d.BodyFileContent);
                }

                //Enable Copy Icon for Response Body
                $("#ApiMonitoringSelectedApiDetailsContainer2CopyToClipboardIcon").css('pointer-events', 'auto');
            }
            else {
                $('#ApiMonitoringSelectedApiDetailsContainer2Text').text(result.d.BodyErrorMsg);

                //Disable Copy Icon for Response Body
                $("#ApiMonitoringSelectedApiDetailsContainer2CopyToClipboardIcon").css('pointer-events', 'none');

                //Can Highlight the Download Icons

            }

            if (result.d.FileExist) {
                //Enable Download Icon for Response Body and set Global Variables for the File
                $("#ApiMonitoringSelectedApiDetailsContainer2DownloadIcon").css('pointer-events', 'auto');
                //apimonitoring._ResponseBodyFileFullPath = obj.chainDetailsData.ResponseBodyDataFileLocation;
                //apimonitoring._ResponseBodyFileName = obj.chainDetailsData.ResponseBodyDataFileName;
            }
            else {
                $("#ApiMonitoringSelectedApiDetailsContainer2DownloadIcon").css('pointer-events', 'none');
                apimonitoring._ResponseBodyFileFullPath = "";
                apimonitoring._ResponseBodyFileName = "";
            }
        }
    }

    //Function to Show Details in Right Div Based on Selection of a Chain in Left Div
    ApiMonitoring.prototype.onClickChain = function onClickChain(obj, event) {

        ////////////////
        // TEST Logic //
        ////////////////
        /*
        var chainIndex = obj.chainIndex;
        var abc="";
        if (chainIndex == 0)
            abc = "yellow";
        else if (chainIndex == 1)
            abc = "red";
        else if (chainIndex == 2)
            abc = "green";
    
        $('#ApiMonitoringMainRightColumn').css('background-color', abc);
        */

        ///////////////////
        // ACTUAL Logic 2//
        ///////////////////

        //var ApiUniqueId = obj.chainApiUniqueId;
        //var chainDetailsDataFormat = obj.chainDetailsDataFormat;

        //Handling for isSelected for color and arrow
        if (apimonitoring._pageViewModelInstance.selectedChainItem != null) {
            apimonitoring._pageViewModelInstance.selectedChainItem.isSelected(false);
        }
        apimonitoring._pageViewModelInstance.selectedChainItem = obj;
        apimonitoring._pageViewModelInstance.selectedChainItem.isSelected(true);

        //Set Selected APIUniqueId for Incoming from Node
        apimonitoring._selectedAPIUniqueId = obj.chainApiUniqueId;

        //Empty the Text Divs of Request Body & Response Body
        document.getElementById("ApiMonitoringSelectedApiDetailsContainer1Text").innerHTML = "";
        document.getElementById("ApiMonitoringSelectedApiDetailsContainer2Text").innerHTML = "";

        //Newline Character Handling
        //var tempStringRequestHeader = obj.chainDetailsData.RequestHeaderData.replace(/(?:\r\n|\r|\n)/g, '<br />');

        //Show Request Body Header
        if (obj.chainDetailsData.RequestHeaderData != "") {
            $('#ApiMonitoringSelectedApiDetailsContainer1HeaderText').text(obj.chainDetailsData.RequestHeaderData);
        }
        else {
            $('#ApiMonitoringSelectedApiDetailsContainer1HeaderText').text("No request header available. Please check server logs for more details.");
        }

        //Check if Request Body is Empty, i.e., NULL Case, i.e. API Call OVER, But Not Completed
        if (obj.chainDetailsData.RequestBodyDataFileLocation != "") {

            //if (obj.chainDetailsData.RequestBodyDataToShow) {
            //Service Call to get contents of the file, if required
            //Global String for Copy is set in onSuccess function

            var params = {};
            params.InputObject = {};
            params.InputObject.DataFormat = obj.chainDetailsDataFormatRequest;
            params.InputObject.RequestOrResponse = "Request";
            params.InputObject.BodyFileLocation = obj.chainDetailsData.RequestBodyDataFileLocation;

            //Call Service
            CallCommonServiceMethod('GetFileContent', params, OnSuccess_LoadApiCallDetailsBodyContent, OnFailure, null, false);

            //}
            //else {
            //    $('#ApiMonitoringSelectedApiDetailsContainer1Text').text(obj.chainDetailsData.RequestBodyErrorMsg);

            //    //Disable Copy Icon for Request Body
            //    $("#ApiMonitoringSelectedApiDetailsContainer1CopyToClipboardIcon").css('pointer-events', 'none');

            //    //Can Highlight the Download Icon

            //}

            ////Enable Download Icon for Request Body and set Global Variables for the File
            //$("#ApiMonitoringSelectedApiDetailsContainer1DownloadIcon").css('pointer-events', 'auto');
            apimonitoring._RequestBodyFileFullPath = obj.chainDetailsData.RequestBodyDataFileLocation;
            apimonitoring._RequestBodyFileName = obj.chainDetailsData.RequestBodyDataFileName;
            ////apimonitoring._RequestBodyFileContentType = obj.chainDetailsData.RequestBodyDataFileContentType;

        }
        else {

            $('#ApiMonitoringSelectedApiDetailsContainer1Text').text(obj.chainDetailsData.RequestBodyErrorMsg);

            //Disable Copy & Download Icons for Request Body
            $("#ApiMonitoringSelectedApiDetailsContainer1CopyToClipboardIcon").css('pointer-events', 'none');
            $("#ApiMonitoringSelectedApiDetailsContainer1DownloadIcon").css('pointer-events', 'none');

            //Clear Request Body Global Variables
            apimonitoring._RequestBodyFileContent = "";
            apimonitoring._RequestBodyFileFullPath = "";
            apimonitoring._RequestBodyFileName = "";
            //apimonitoring._RequestBodyFileContentType = "";
        }

        //Show Response Body Header
        //////////////////////////////NOTE : NULL EXCEPTION HANDLING TO BE DONE ////////////////////////////
        ////////////////////////SHOW SOME MESSAGE IF RESPONSE HEADER IS NOT AVAILABLE///////////////////////

        //Newline Character Handling
        //var tempStringResponseHeader = obj.chainDetailsData.ResponseHeaderData.replace(/(?:\r\n|\r|\n)/g, '<br />');

        if (obj.chainDetailsData.ResponseHeaderData != "") {
            $('#ApiMonitoringSelectedApiDetailsContainer2HeaderText').text(obj.chainDetailsData.ResponseHeaderData);
        }
        else {
            $('#ApiMonitoringSelectedApiDetailsContainer2HeaderText').text("No response header available. Please check server logs for more details.");
        }


        //Check if Response Body is Empty, i.e., NULL Case, i.e. ((API Call OVER, But Not Completed) OR (API Call NOT OVER))
        if (obj.chainDetailsData.ResponseBodyDataFileLocation != "") {

            //To Check if API Call OVER, But Not Completed
            if (obj.chainDetailsData.ResponseBodyDataFileLocation != "APICNO") {
                //if (obj.chainDetailsData.ResponseBodyDataToShow) {
                //Service Call to get contents of the file, if required
                //Global String for Copy is set in onSuccess function

                var params = {};
                params.InputObject = {};
                params.InputObject.DataFormat = obj.chainDetailsDataFormatResponse;
                params.InputObject.RequestOrResponse = "Response";
                params.InputObject.BodyFileLocation = obj.chainDetailsData.ResponseBodyDataFileLocation;

                //Call Service
                CallCommonServiceMethod('GetFileContent', params, OnSuccess_LoadApiCallDetailsBodyContent, OnFailure, null, false);
                //}
                //else {
                //    $('#ApiMonitoringSelectedApiDetailsContainer2Text').text(obj.chainDetailsData.ResponseBodyErrorMsg);

                //    //Disable Copy Icon for Response Body
                //    $("#ApiMonitoringSelectedApiDetailsContainer2CopyToClipboardIcon").css('pointer-events', 'none');

                //    //Can Highlight the Download Icons

                //}

                ////Enable Download Icon for Response Body and set Global Variables for the File
                //$("#ApiMonitoringSelectedApiDetailsContainer2DownloadIcon").css('pointer-events', 'auto');
                apimonitoring._ResponseBodyFileFullPath = obj.chainDetailsData.ResponseBodyDataFileLocation;
                apimonitoring._ResponseBodyFileName = obj.chainDetailsData.ResponseBodyDataFileName;
                ////apimonitoring._ResponseBodyFileContentType = obj.chainDetailsData.ResponseBodyDataFileContentType;
            }
                //If API Call NOT OVER
            else {
                $('#ApiMonitoringSelectedApiDetailsContainer2Text').text("API Call Execution Not Over Yet.");
            }
        }
        else {

            $('#ApiMonitoringSelectedApiDetailsContainer2Text').text(obj.chainDetailsData.ResponseBodyErrorMsg);

            //Disable Copy & Download Icons for Response Body
            $("#ApiMonitoringSelectedApiDetailsContainer2CopyToClipboardIcon").css('pointer-events', 'none');
            $("#ApiMonitoringSelectedApiDetailsContainer2DownloadIcon").css('pointer-events', 'none');

            //Clear Response Body Global Variables
            apimonitoring._ResponseBodyFileContent = "";
            apimonitoring._ResponseBodyFileFullPath = "";
            apimonitoring._ResponseBodyFileName = "";
            apimonitoring._ResponseViewLogContent = "";
            //apimonitoring._ResponseBodyFileContentType = "";
        }

        $("#ApiMonitoringSelectedApiDetails").css({ 'display': 'block' });

        $('#ApiMonitoringSelectedApiDetailsRequestSubTabHeaderContainer').click();
        $('#ApiMonitoringSelectedApiDetailsResponseSubTabHeaderContainer').click();


        //var params = {};
        //params.InputObject = {};
        //params.InputObject.ApiUniqueId = ApiUniqueId;

        ////Check what is the format of the data and call the appropriate library accordingly
        //if (chainDetailsDataFormat == "JSON") {
        //    CallCommonServiceMethod('GetApiCallDetailsData', params, OnSuccess_LoadApiCallDetailsJSONData, OnFailure, null, false);
        //}
        //else if (chainDetailsDataFormat == "XML") {
        //    CallCommonServiceMethod('GetApiCallDetailsData', params, OnSuccess_LoadApiCallDetailsXMLData, OnFailure, null, false);
        //}

        /*
        ///////////////////
        // ACTUAL Logic 2//
        ///////////////////
        var chainDetailsDataFormat = obj.chainDetailsDataFormat;
        
        var chainDetailsData = obj.chainDetailsData;

        //Empty the Output Div
        document.getElementById("ApiMonitoringSelectedApiDetailsText").innerHTML = "";

        //Check what is the format of the data and call the appropriate library accordingly
        if (chainDetailsDataFormat == "JSON") {

            //Call the JSON Library here
            document.getElementById("ApiMonitoringSelectedApiDetailsText").appendChild(renderjson
                .set_icons('+', '-')
                .set_show_to_level(2)               // Special Case : If we specify the String "all", then the entire JSON is displayed completely expanded.
                .set_max_string_length(100)(chainDetailsData));
            //.set_max_string_length(100)([{ "a": "b" }, {"c":"d"}]));
        }
        else if (chainDetailsDataFormat == "XML") {
            //Call the XML Library here
            LoadXMLString('ApiMonitoringSelectedApiDetailsText',
                            chainDetailsData,
                            1       //Here 1 is the number of levels, we want expanded initially.
            );
        }
        */

    };


    ///////////////////////////////////////////////////
    // Code for Download and copy to Clipboard Icons //
    ///////////////////////////////////////////////////

    // Function to download data to a file
    // Note : Tested to be working properly in Chrome, FireFox and IE10.
    //        In Safari, the data gets opened in a new tab and one would have to manually save this file.
    //function downloadFile(data, filename, type) {
    //    var file = new Blob([data], { type: type });
    //    if (window.navigator.msSaveOrOpenBlob)                      // For IE10+
    //        window.navigator.msSaveOrOpenBlob(file, filename);
    //    else {                                                      // For Other Browsers
    //        var a = document.createElement("a"),
    //                url = URL.createObjectURL(file);
    //        a.href = url;
    //        a.download = filename;
    //        document.body.appendChild(a);
    //        a.click();
    //        setTimeout(function () {
    //            document.body.removeChild(a);
    //            window.URL.revokeObjectURL(url);
    //        }, 0);
    //    }
    //}

    //Function to copy data to clipboard
    function copyToClipboard(dataToCopy) {
        var $temp = $("<textarea>");
        $("body").append($temp);
        $temp.html(dataToCopy).select();
        document.execCommand("copy");
        $temp.remove();

        /*
        //This Also Works
        // create hidden text element
        var target = document.createElement("textarea");
        target.style.position = "absolute";
        target.style.left = "-9999px";
        target.style.top = "0";
        //target.id = targetId;
        document.body.appendChild(target);
        target.textContent = dataToCopy;
        
        // select the content
        var currentFocus = document.activeElement;
        target.focus();
        target.setSelectionRange(0, target.value.length);

        // copy the selection
        var succeed;
        try {
            succeed = document.execCommand("copy");
        } catch (e) {
            succeed = false;
        }

        // restore original focus
        if (currentFocus && typeof currentFocus.focus === "function") {
            currentFocus.focus();
        }
        
        target.textContent = "";

        return succeed;
        */
    }

    //Function to Remove the last part of the Directory
    function RemoveLastDirectoryPartOf(the_url) {
        var the_arr = the_url.split('/');
        the_arr.pop();
        return (the_arr.join('/'));
    }

    // On Click Function for Download Icons
    $('#ApiMonitoringSelectedApiDetailsContainer1DownloadIcon').unbind('click').bind('click', function () {
        //downloadFile(apimonitoring._RequestBodyFileContent, "Request Object\.txt", "plain/text");

        var pathWithoutCommonUI = RemoveLastDirectoryPartOf(path);

        window.open(pathWithoutCommonUI + "/Download.aspx?fileonserver=true&name=" + apimonitoring._RequestBodyFileName + "&fullPath=" + apimonitoring._RequestBodyFileFullPath + "&contentType=" + "text/plain");

    });
    $('#ApiMonitoringSelectedApiDetailsContainer2DownloadIcon').unbind('click').bind('click', function () {
        //downloadFile(apimonitoring._ResponseBodyFileContent, "Response Object\.txt", "plain/text");

        var pathWithoutCommonUI = RemoveLastDirectoryPartOf(path);

        window.open(pathWithoutCommonUI + "/Download.aspx?fileonserver=true&name=" + apimonitoring._ResponseBodyFileName + "&fullPath=" + apimonitoring._ResponseBodyFileFullPath + "&contentType=" + "text/plain");
    });

    // On Click Function for Copy Icons
    $('#ApiMonitoringSelectedApiDetailsContainer1CopyToClipboardIcon').unbind('click').bind('click', function () {
        copyToClipboard(apimonitoring._RequestBodyFileContent);
    });

    $('#ApiMonitoringSelectedApiDetailsContainer2CopyToClipboardIcon').unbind('click').bind('click', function () {

        if ($(".SelectedSubTabItem.ResponseSubTabItem")[0].id == "ApiMonitoringSelectedApiDetailsResponseSubTabBodyContainer") {
            copyToClipboard(apimonitoring._ResponseBodyFileContent);
        }
    });

    $('#ApiMonitoringSelectedApiDetailsContainer2CopyToClipboardIconBB').unbind('click').bind('click', function () {

        if ($(".SelectedSubTabItem.ResponseSubTabItem")[0].id == "ApiMonitoringSelectedApiDetailsResponseSubTabViewLogsContainer") {
            copyToClipboard(apimonitoring._ResponseViewLogContent);
        }
    });

    //////////////////////////////////////////////
    //CODE For MULTI-SELECT DROPDOWN For Filters//
    //////////////////////////////////////////////
    function ApplyMultiSelect(id, isRunningText, data, isSearch, container, text, allSelected, selectedItemsList) {
        var selectedItems = [];

        //If no data returned from DB
        if (data.length == 0) {
            var tempObj = {};
            tempObj.text = "";
            tempObj.value = "";
            data.push(tempObj);
            //data.push({ "text": "", "value": "" });
        }

        if (allSelected) {
            $.each(data, function (key, selectedItemObj) {
                selectedItems.push(selectedItemObj.text);
            });
        }
        else {
            if (typeof (selectedItemsList) != 'undefined' && selectedItemsList.length > 0) {
                $.each(selectedItemsList, function (key, selectedItemObj) {
                    selectedItems.push(selectedItemObj);
                });
            }
        }

        //To handle Initial Selection, i.e., All Selected
        if (id == 'ApiMonoitoringUrlFilterMultiSelectedItems') {
            apimonitoring._selectedUrl = [];
            if (allSelected) {
                apimonitoring._selectedUrl.push("-1");
            }
            else {
                apimonitoring._selectedUrl = selectedItems;
            }
            apimonitoring._totalUrl = data.length;
        } else if (id == 'ApiMonoitoringMethodFilterMultiSelectedItems') {
            apimonitoring._selectedMethod = [];
            if (allSelected) {
                apimonitoring._selectedMethod.push("-1");
            }
            else {
                apimonitoring._selectedMethod = selectedItems;
            }
            apimonitoring._totalMethod = data.length;
        } else if (id == 'ApiMonoitoringClientMachineFilterMultiSelectedItems') {
            apimonitoring._selectedClientMachine = [];
            if (allSelected) {
                apimonitoring._selectedClientMachine.push("-1");
            }
            else {
                apimonitoring._selectedClientMachine = selectedItems;
            }
            apimonitoring._totalClientMachine = data.length;
        }

        smselect.create(
        {
            id: id,
            text: text,
            container: $(container),
            isMultiSelect: true,
            isRunningText: isRunningText,
            data: [{ options: data, text: text }],
            selectedItems: selectedItems,
            showSearch: isSearch,
            ready: function (selectelement) {
                selectelement.css({ /*'border': '1px solid #CECECE',*/ 'border-left': 'none', height: '23px', width: '200px', 'vertical-align': 'middle' });
                //selectelement.find(".smselectrun").css({ height: '27px'});
                selectelement.find(".smselectcon").height(selectelement.find(".smselectcon").height() + 3);
                selectelement.on('change', function (ee) {
                    ddlOnChangeHandler(ee);
                });

            }
        });
    }

    //This function is called when a Multi-Select is changed.
    function ddlOnChangeHandler(ee) {
        var target = $(ee.currentTarget);

        if (target[0].id == 'smselect_ApiMonoitoringUrlFilterMultiSelectedItems') {

            apimonitoring._selectedUrl = [];

            if (smselect.getSelectedOption(target).length == apimonitoring._totalUrl) {
                apimonitoring._selectedUrl.push("-1");
            }
            else {
                for (var item in smselect.getSelectedOption(target)) {
                    apimonitoring._selectedUrl.push(smselect.getSelectedOption(target)[item].value)
                }
            }

        } else if (target[0].id == 'smselect_ApiMonoitoringMethodFilterMultiSelectedItems') {

            apimonitoring._selectedMethod = [];
            if (smselect.getSelectedOption(target).length == apimonitoring._totalMethod) {
                apimonitoring._selectedMethod.push("-1");
            }
            else {
                for (var item in smselect.getSelectedOption(target)) {
                    apimonitoring._selectedMethod.push(smselect.getSelectedOption(target)[item].value)
                }
            }

        } else if (target[0].id == 'smselect_ApiMonoitoringClientMachineFilterMultiSelectedItems') {

            apimonitoring._selectedClientMachine = [];
            if (smselect.getSelectedOption(target).length == apimonitoring._totalClientMachine) {
                apimonitoring._selectedClientMachine.push("-1");
            }
            else {
                for (var item in smselect.getSelectedOption(target)) {
                    apimonitoring._selectedClientMachine.push(smselect.getSelectedOption(target)[item].value)
                }
            }
        }
    }

    //Getting Data For Multi-Select Filters and Calling Callback function to initialize Multi-Select Filters
    function getMultiSelectFilters() {
        var params = {};

        //Call for ALL Multi-Select Filter Data
        CallCommonServiceMethod('GetFiltersData', params, OnSuccess_LoadMultiSelectFilters, OnFailure, null, false);
    }

    //Callback Function for Service Call to get Multi-Select Filters Data
    function OnSuccess_LoadMultiSelectFilters(result) {

        if (apimonitoring._isDDLBindFirstTime == true) {
            ApplyMultiSelect('ApiMonoitoringUrlFilterMultiSelectedItems', false, result.d.URLFiltersData, true, '#ApiMonoitoringUrlFilterMultiSelect', 'URL', true);
            ApplyMultiSelect('ApiMonoitoringMethodFilterMultiSelectedItems', false, result.d.MethodFiltersData, true, '#ApiMonoitoringMethodFilterMultiSelect', 'Method', true);
            ApplyMultiSelect('ApiMonoitoringClientMachineFilterMultiSelectedItems', false, result.d.ClientMachineFiltersData, true, '#ApiMonoitoringClientMachineFilterMultiSelect', 'ClientMachine', true);

            init();

            apimonitoring._isDDLBindFirstTime = false;
        }
        else {
            if (apimonitoring._selectedUrl[0] != "-1") {
                ApplyMultiSelect('ApiMonoitoringUrlFilterMultiSelectedItems', false, result.d.URLFiltersData, true, '#ApiMonoitoringUrlFilterMultiSelect', 'URL', false, apimonitoring._selectedUrl);
            }
            else {
                ApplyMultiSelect('ApiMonoitoringUrlFilterMultiSelectedItems', false, result.d.URLFiltersData, true, '#ApiMonoitoringUrlFilterMultiSelect', 'URL', true);
            }

            if (apimonitoring._selectedMethod[0] != "-1") {
                ApplyMultiSelect('ApiMonoitoringMethodFilterMultiSelectedItems', false, result.d.MethodFiltersData, true, '#ApiMonoitoringMethodFilterMultiSelect', 'Method', false, apimonitoring._selectedMethod);
            }
            else {
                ApplyMultiSelect('ApiMonoitoringMethodFilterMultiSelectedItems', false, result.d.MethodFiltersData, true, '#ApiMonoitoringMethodFilterMultiSelect', 'Method', true);
            }

            if (apimonitoring._selectedClientMachine[0] != "-1") {
                ApplyMultiSelect('ApiMonoitoringClientMachineFilterMultiSelectedItems', false, result.d.ClientMachineFiltersData, true, '#ApiMonoitoringClientMachineFilterMultiSelect', 'ClientMachine', false, apimonitoring._selectedClientMachine);
            }
            else {
                ApplyMultiSelect('ApiMonoitoringClientMachineFilterMultiSelectedItems', false, result.d.ClientMachineFiltersData, true, '#ApiMonoitoringClientMachineFilterMultiSelect', 'ClientMachine', true);
            }
        }
    }

    ///////////////////////////////////////////////////////
    //CODE FOR SMDatePickerControl For Filtering on Dates//
    ///////////////////////////////////////////////////////
    function initializeDivFilterDate() {
        /*	
            Date Options
    
            0: "Today",
            1: "Since Yesterday",
            3: "Anytime",
            2: "This Week",
            4: "Custom Date",
            5: "Last Days",
            6: "Next Days" 
     
            Calender Types 
    
            0: 2 calenders
            1: 1 calender
            2: no calender
        
            Id of calender is 'smdd_#' 
            Id of dropdowns 'ddlnextNtime_#' , 'ddllastNtime_#'
            { # starts with 0 }
    
        */
        var obj = {};
        obj.dateOptions = [0, 1, 2, 3, 4];      //Which options to be shown on calender
        obj.dateFormat = 'd/m/Y';
        obj.timePicker = false;
        obj.lefttimePicker = false;
        obj.righttimePicker = false;
        obj.calenderType = 0;
        obj.calenderID = 'smdd_0';
        obj.pivotElement = $('#ApiMonitoringDatesFilter');    //Calender will be displayed when clicked on this div

        obj.StartDateCalender = new Date().format(com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
        obj.EndDateCalender = new Date().format(com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));

        obj.selectedTopOption = 0; // Which Dateoption to select by default

        apimonitoring._selectedStartDate = obj.StartDateCalender;   //Set to Default Value, i.e. Today
        apimonitoring._selectedEndDate = obj.EndDateCalender;       //Set to Default Value, i.e. Today

        obj.selectedCustomRadioOption = 0;
        obj.applyCallback = function () {

            var modifiedText = smdatepickercontrol.getResponse($("#smdd_0"));
            var htmlString = "";
            var prepString = "";

            var selectedEndDate = modifiedText.selectedEndDate;
            var selectedStartDate = modifiedText.selectedStartDate;
            var selectedText = modifiedText.selectedText;
            var selectedDateOption = modifiedText.selDateOption;
            var selectedRadioOption = modifiedText.selRadioOption;

            if (selectedStartDate != null) {
                //Date Culture Change
                selectedStartDate = new Date(selectedStartDate).format(com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                $('[id$=hdnStartDateCalender]').val(selectedStartDate);

                // Updating Object Variable
                apimonitoring._selectedStartDate = selectedStartDate;
                //
            }
            else {
                // Updating Object Variable
                apimonitoring._selectedStartDate = null;
            }

            if (selectedEndDate != null) {
                //Date Culture Change
                selectedEndDate = new Date(selectedEndDate).format(com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                $('[id$=hdnEndDateCalender]').val(selectedEndDate);

                // Updating Object Variable
                apimonitoring._selectedEndDate = selectedEndDate;
                //
            }
            else {
                // Updating Object Variable
                apimonitoring._selectedEndDate = null;
            }
            if (selectedText != undefined) {
                if (selectedText.toUpperCase() === "TODAY")
                    htmlString = "Today";
                else if (selectedText.toUpperCase() === "SINCE YESTERDAY")
                    htmlString = "Since Yesterday";
                else if (selectedText.toUpperCase() === "THIS WEEK")
                    htmlString = "This Week";
                else if (selectedText.toUpperCase() === "ANYTIME")
                { prepString = ""; htmlString = "Anytime"; }
                else if (selectedText.toUpperCase() === "CUSTOM DATE") {
                    if (selectedRadioOption == 0)
                        htmlString = " after " + selectedStartDate;
                    else if (selectedRadioOption == 1)
                        htmlString = " between " + selectedStartDate + " to " + selectedEndDate;
                    else if (selectedRadioOption == 2)
                        htmlString = " before " + selectedEndDate;
                }
                $('[id$=hdnTopOption]').val(selectedDateOption);
                $('[id$=hdnCustomRadioOption]').val(selectedRadioOption);
                $('[id$=hdnFirstTime]').val('0');
                $('[id$=ApiMonitoringDatesFilterText]').text(htmlString);
                //   SMSDownstreamSystemStatusMethods.prototype._controls.HdnExceptionDate().val(selectedDateOption);
                $('[id*=TextStartDate]').val(selectedStartDate);
                $('[id*=TextEndDate]').val(selectedEndDate);
            }
            var errormsg = validateDates();
            return errormsg
        }

        obj.ready = function (e) {
        }
        smdatepickercontrol.init(obj);
    }
    function validateDates() {
        checked = $('[id$=hdnCustomRadioOption]').val();
        marked = $('[id$=hdnTopOption]').val();
        startDate = $('[id*=TextStartDate]').val();
        endDate = $('[id*=TextEndDate]').val();
        if (startDate == "")
            startDate = null;
        if (endDate == "")
            endDate = null;
        var errormsg = '';
        if (marked == '4') {
            switch (checked) {

                case '0': //From
                    errormsg = CompareDateFromTodaysDateUS(SMSCommonStatusMethods.prototype._controls.TxtStartDate().val(), 'Start Date');
                    break;
                case '1':
                    errormsg = CompareDateFromTodaysDateUS(SMSCommonStatusMethods.prototype._controls.TxtStartDate().val(), 'Start Date');
                    if (errormsg == '')
                        errormsg = CompareDateFromTodaysDateUS(SMSCommonStatusMethods.prototype._controls.TxtEndDate().val(), 'End Date');
                    if (errormsg == '')
                        errormsg = errormsg = CompareDateUS(SMSCommonStatusMethods.prototype._controls.TxtStartDate().val(), SMSCommonStatusMethods.prototype._controls.TxtEndDate().val());
                    break;
                case '2':
                    errormsg = CompareDateFromTodaysDateUS(SMSCommonStatusMethods.prototype._controls.TxtEndDate().val(), 'End Date');
                    break;
            }
        }
        return errormsg;
    }

    /////////////////////////////////////////////////////////
    //CODE For Display Message when Clicked on Disabled Div//
    /////////////////////////////////////////////////////////

    function OnClickDisabled() {
        $("#" + apimonitoring._controlIdInfo.LblErrorId).text("Please Stop Capturing first.");
        $find(apimonitoring._controlIdInfo.ModalErrorId).show();
    }

    /////////////////////////////////////////////////
    //CODE For Calling COMMON SERVICE with our Data//
    /////////////////////////////////////////////////
    $('#ApiMonitoringFiltersButton').unbind('click').bind('click', function () {

        //Empty the Error Msg Div
        $("#ApiMonitoringFilteringErrorDiv").text("");

        if (apimonitoring._selectedUrl.length == 0 || apimonitoring._selectedMethod.length == 0 || apimonitoring._selectedClientMachine.length == 0) {
            $("#ApiMonitoringFilteringErrorDiv").text("Please select atleast one option in each filter.");
        }
        else {
            //Reset Selected APIUniqueId
            apimonitoring._selectedAPIUniqueId = -1;

            var params = {};
            params.InputObject = {};
            params.InputObject.selectedUrl = apimonitoring._selectedUrl;
            params.InputObject.selectedMethod = apimonitoring._selectedMethod;
            params.InputObject.selectedClientMachine = apimonitoring._selectedClientMachine;

            //Date Culture Change
            //var selectedStartDateTemp = new Date(apimonitoring._selectedStartDate);
            //var selectedEndDateTemp = new Date(apimonitoring._selectedEndDate);
            var selectedStartDateTemp = apimonitoring._selectedStartDate != null ? Date.parseInvariant(apimonitoring._selectedStartDate, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate)) : null;
            var selectedEndDateTemp = apimonitoring._selectedEndDate!= null ? Date.parseInvariant(apimonitoring._selectedEndDate, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate)) : null;

            var selectedStartDate = selectedStartDateTemp != null ? new Date(selectedStartDateTemp.toUTCString()).format(com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate)) : null;
            var selectedEndDate = selectedEndDateTemp != null ? new Date(selectedEndDateTemp.toUTCString()).format(com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate)) : null;

            params.InputObject.selectedStartDate = selectedStartDate;
            params.InputObject.selectedEndDate = selectedEndDate;

            CallCommonServiceMethod('GetApiCallsData', params, OnSuccess_LoadApiCallsDataLater, OnFailure, null, false);

            //$(".ApiMonitoringChain").css("display", "block");

            //The below call is just to test OnFailure
            //Do NOT Uncomment Otherwise
            //CallCommonServiceMethod('GetApiCallsData', params, OnFailure, OnFailure, null, false);

            getMultiSelectFilters();
        }
    });

    $('#ApiMonitoringCapturingButton').unbind('click').bind('click', function () {

        if ($('#ApiMonitoringCapturingButton').val() == "Start Capturing") {
            //Empty the Error Msg Div
            $("#ApiMonitoringFilteringErrorDiv").text("");

            if (apimonitoring._selectedUrl.length == 0 || apimonitoring._selectedMethod.length == 0 || apimonitoring._selectedClientMachine.length == 0) {
                $("#ApiMonitoringFilteringErrorDiv").text("Please select atleast one option in each filter.");
            }
            else {
                //Reset Selected APIUniqueId
                apimonitoring._selectedAPIUniqueId = -1;

                var params = {};
                params.InputObject = {};
                params.InputObject.selectedUrl = apimonitoring._selectedUrl;
                params.InputObject.selectedMethod = apimonitoring._selectedMethod;
                params.InputObject.selectedClientMachine = apimonitoring._selectedClientMachine;

                //var selectedStartDateTemp = new Date(apimonitoring._selectedStartDate);
                //var selectedEndDateTemp = new Date(apimonitoring._selectedEndDate);
                var selectedStartDateTemp = apimonitoring._selectedStartDate != null ? Date.parseInvariant(apimonitoring._selectedStartDate, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate)) : null;
                var selectedEndDateTemp = apimonitoring._selectedEndDate != null ? Date.parseInvariant(apimonitoring._selectedEndDate, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate)) : null;

                var selectedStartDate = selectedStartDateTemp != null ? new Date(selectedStartDateTemp.toUTCString()).format(com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate)) : null;
                var selectedEndDate = selectedEndDateTemp != null ? new Date(selectedEndDateTemp.toUTCString()).format(com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate)) : null;

                params.InputObject.selectedStartDate = selectedStartDate;
                params.InputObject.selectedEndDate = selectedEndDate;

                CallCommonServiceMethod('GetApiCallsData', params, OnSuccess_LoadApiCallsDataLater, OnFailure, null, false);

                //$(".ApiMonitoringChain").css("display", "block");

                //Estabilish Connection to Node Server and Start Listening
                setUpNodeConnection();

                //Disable All Filters and the Apply Filters & Reset Button
                $("#ApiMonitoringFiltersSubContainer").css('pointer-events', 'none');
                $("#ApiMonitoringFiltersButton").css('pointer-events', 'none');


                //SHOW message in Modal Error if Clicked on Filters or Apply Button
                $(".ApiMonitoringFiltersContainer").bind('click', OnClickDisabled);
                $("#ApiMonitoringFiltersButtonContainer").bind('click', OnClickDisabled);



                getMultiSelectFilters();

                //Remove and Add Class
                $('#ApiMonitoringCapturingButton').removeClass("CommonOrangeBtnStyle");
                $('#ApiMonitoringCapturingButton').addClass("ApiMonitoringRedBtnStyle");

                //Change Value
                $('#ApiMonitoringCapturingButton').val("Stop Capturing");
            }
        }
        else if ($('#ApiMonitoringCapturingButton').val() == "Stop Capturing") {

            //Disconnect Connection to Node Server
            disconnectNodeConnection();

            //Enable All Filters and the Apply Filters Button
            $("#ApiMonitoringFiltersSubContainer").css('pointer-events', 'auto');
            $("#ApiMonitoringFiltersButton").css('pointer-events', 'auto');

            //NOT show message in Modal Error if Clicked on Filters or Apply Button
            $(".ApiMonitoringFiltersContainer").unbind('click', OnClickDisabled);
            $("#ApiMonitoringFiltersButtonContainer").unbind('click', OnClickDisabled);


            //Remove and Add Class
            $('#ApiMonitoringCapturingButton').removeClass("ApiMonitoringRedBtnStyle");
            $('#ApiMonitoringCapturingButton').addClass("CommonOrangeBtnStyle");

            //Change Value
            $('#ApiMonitoringCapturingButton').val("Start Capturing");

        }
    });


    //$('#ApiMonitoringStartCapturingButton').unbind('click').bind('click', function () {

    //    //Empty the Error Msg Div
    //    $("#ApiMonitoringFilteringErrorDiv").text("");

    //    if (apimonitoring._selectedUrl.length == 0 || apimonitoring._selectedMethod.length == 0 || apimonitoring._selectedClientMachine.length == 0) {
    //        $("#ApiMonitoringFilteringErrorDiv").text("Please select atleast one option in each filter.");
    //    }
    //    else {
    //        //Reset Selected APIUniqueId
    //        apimonitoring._selectedAPIUniqueId = -1;

    //        var params = {};
    //        params.InputObject = {};
    //        params.InputObject.selectedUrl = apimonitoring._selectedUrl;
    //        params.InputObject.selectedMethod = apimonitoring._selectedMethod;
    //        params.InputObject.selectedClientMachine = apimonitoring._selectedClientMachine;
    //        params.InputObject.selectedStartDate = apimonitoring._selectedStartDate;
    //        params.InputObject.selectedEndDate = apimonitoring._selectedEndDate;

    //        CallCommonServiceMethod('GetApiCallsData', params, OnSuccess_LoadApiCallsDataLater, OnFailure, null, false);

    //        //$(".ApiMonitoringChain").css("display", "block");

    //        //Estabilish Connection to Node Server and Start Listening
    //        setUpNodeConnection();

    //        //Disable All Filters and the Apply Filters & Reset Button
    //        $("#ApiMonitoringFilteringContainer").css('pointer-events', 'none');

    //        //SHOW message in Modal Error if Clicked on Filters or Apply Button
    //        $("#ApiMonitoringFilteringSuperContainer").bind('click', OnClickDisabled);

    //        getMultiSelectFilters();
    //    }        
    //});

    //$('#ApiMonitoringStopCapturingButton').unbind('click').bind('click', function () {

    //    //Disconnect Connection to Node Server
    //    disconnectNodeConnection();

    //    //Enable All Filters and the Apply Filters Button
    //    $("#ApiMonitoringFilteringContainer").css('pointer-events', 'auto');

    //    //NOT show message in Modal Error if Clicked on Filters or Apply Button
    //    $("#ApiMonitoringFilteringSuperContainer").unbind('click', OnClickDisabled);
    //});

    $('#ApiMonitoringSelectedApiDetailsRequestSubTabHeaderContainer').unbind('click').bind('click', function () {
        //Check if not already clicked
        if (!$('#ApiMonitoringSelectedApiDetailsRequestSubTabHeaderContainer').hasClass('SelectedSubTabItem')) {

            //Remove SelectedSubtabItem class from all RequestSubTabItem
            $('.RequestSubTabItem').removeClass('SelectedSubTabItem');

            //Hide Request Body Content Div
            $('#ApiMonitoringSelectedApiDetailsRequestBodyContainer').css('display', 'none');

            //Show Request Header Content Div
            $('#ApiMonitoringSelectedApiDetailsRequestHeaderContainer').css('display', 'block');

            //Add Selected Class to Header
            $('#ApiMonitoringSelectedApiDetailsRequestSubTabHeaderContainer').addClass('SelectedSubTabItem');

            //Hide Download & Copy Icons
            $("#ApiMonitoringSelectedApiDetailsContainer1Icons").css('display', 'none');
        }
    });

    $('#ApiMonitoringSelectedApiDetailsRequestSubTabBodyContainer').unbind('click').bind('click', function () {
        //Check if not already clicked
        if (!$('#ApiMonitoringSelectedApiDetailsRequestSubTabBodyContainer').hasClass('SelectedSubTabItem')) {

            //Remove SelectedSubtabItem class from all RequestSubTabItem
            $('.RequestSubTabItem').removeClass('SelectedSubTabItem');

            //Hide Request Header Content Div
            $('#ApiMonitoringSelectedApiDetailsRequestHeaderContainer').css('display', 'none');

            //Show Request Body Content Div
            $('#ApiMonitoringSelectedApiDetailsRequestBodyContainer').css('display', 'block');

            //Add Selected Class to Body
            $('#ApiMonitoringSelectedApiDetailsRequestSubTabBodyContainer').addClass('SelectedSubTabItem');

            //Show Download & Copy Icons
            $("#ApiMonitoringSelectedApiDetailsContainer1Icons").css('display', 'inline-block');
        }
    });

    $('#ApiMonitoringSelectedApiDetailsResponseSubTabHeaderContainer').unbind('click').bind('click', function () {
        //Check if not already clicked
        if (!$('#ApiMonitoringSelectedApiDetailsResponseSubTabHeaderContainer').hasClass('SelectedSubTabItem')) {

            //Remove SelectedSubtabItem class from all ResponseSubTabItem
            $('.ResponseSubTabItem').removeClass('SelectedSubTabItem');

            //Hide Other Response Containers            
            $('#ApiMonitoringSelectedApiDetailsResponseBodyContainer').css('display', 'none');
            $('#ApiMonitoringSelectedApiDetailsResponseViewLogsContainer').css('display', 'none');

            //Show Response Header Content Div
            $('#ApiMonitoringSelectedApiDetailsResponseHeaderContainer').css('display', 'block');

            //Add Selected Class to Header
            $('#ApiMonitoringSelectedApiDetailsResponseSubTabHeaderContainer').addClass('SelectedSubTabItem');

            //Hide Download & Both Copy Icons
            $("#ApiMonitoringSelectedApiDetailsContainer2DownloadIcon").css('display', 'none');
            $("#ApiMonitoringSelectedApiDetailsContainer2CopyToClipboardIcon").css('display', 'none');
            $("#ApiMonitoringSelectedApiDetailsContainer2CopyToClipboardIconBB").css("display", "none");
        }
    });

    $('#ApiMonitoringSelectedApiDetailsResponseSubTabBodyContainer').unbind('click').bind('click', function () {
        //Check if not already clicked
        if (!$('#ApiMonitoringSelectedApiDetailsResponseSubTabBodyContainer').hasClass('SelectedSubTabItem')) {

            //Remove SelectedSubtabItem class from all ResponseSubTabItem
            $('.ResponseSubTabItem').removeClass('SelectedSubTabItem');

            //Hide Other Response Containers            
            $('#ApiMonitoringSelectedApiDetailsResponseHeaderContainer').css('display', 'none');
            $('#ApiMonitoringSelectedApiDetailsResponseViewLogsContainer').css('display', 'none');

            //Show Response Body Content Div
            $('#ApiMonitoringSelectedApiDetailsResponseBodyContainer').css('display', 'block');

            //Add Selected Class to Body
            $('#ApiMonitoringSelectedApiDetailsResponseSubTabBodyContainer').addClass('SelectedSubTabItem');

            //Show Download & Body Copy Icons & Hide Log Copy Icon
            $("#ApiMonitoringSelectedApiDetailsContainer2DownloadIcon").css('display', 'inline-block');
            $("#ApiMonitoringSelectedApiDetailsContainer2CopyToClipboardIcon").css('display', 'inline-block');
            $("#ApiMonitoringSelectedApiDetailsContainer2CopyToClipboardIconBB").css("display", "none");

        }
    });

    $('#ApiMonitoringSelectedApiDetailsResponseSubTabViewLogsContainer').unbind('click').bind('click', function () {
        //Check if not already clicked
        if (!$('#ApiMonitoringSelectedApiDetailsResponseSubTabViewLogsContainer').hasClass('SelectedSubTabItem')) {

            //Remove SelectedSubtabItem class from all ResponseSubTabItem
            $('.ResponseSubTabItem').removeClass('SelectedSubTabItem');

            //Hide Other Response Containers            
            $('#ApiMonitoringSelectedApiDetailsResponseHeaderContainer').css('display', 'none');
            $('#ApiMonitoringSelectedApiDetailsResponseBodyContainer').css('display', 'none');

            //Hide Download & Copy Icons
            $("#ApiMonitoringSelectedApiDetailsContainer2DownloadIcon").css('display', 'none');
            $("#ApiMonitoringSelectedApiDetailsContainer2CopyToClipboardIcon").css('display', 'none');

            //Call to Common Service to Get API Call Log Data from DB
            var params = {};
            params.InputObject = {};
            params.InputObject.ApiCallId = parseInt(apimonitoring._selectedAPIUniqueId);

            CallCommonServiceMethod('GetApiCallLogData', params, OnSuccess_LoadApiCallLogData, OnFailure, null, false);

        }
    });

    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }


    ////////////////////////////
    //CODE For Node Connection//
    ////////////////////////////
    //Setting Up Connection to Node Server
    function setUpNodeConnection() {
		var nodeURL = window.location.protocol + '//' + window.location.host.split(":")[0] + ':' + $("#APIMonitoringSocketUrl").val().split(":")[2]
        apimonitoring._iosocket = io.connect(nodeURL, { reconnect: true });

        // Very Imp
        if (apimonitoring._firstTimeConnectionToNode) {
            apimonitoring._firstTimeConnectionToNode = false;
        }
            //If Connecting to NODE Server again, i.e., Clicked on Start Monitoring again after Stopping
        else {
            apimonitoring._iosocket.connect();
        }

        apimonitoring._iosocket.removeListener("connect");
        apimonitoring._iosocket.on('connect', function () {

            apimonitoring._iosocket.removeListener("ApiCallDataPostToClients");
            apimonitoring._iosocket.on('ApiCallDataPostToClients', function (data) {
                //Insert
                if (!data.data.IsApiCallOver) {
                    FilterDataIncomingFromNodeServer(data.data);
                }
                    //Update
                else if (data.data.IsApiCallOver) {
                    //Find in chainListing
                    var matchedItem = ko.utils.arrayFirst(apimonitoring._pageViewModelInstance.chainListing(), function (item) {
                        return item.chainApiUniqueId == data.data.ApiUniqueId;
                    });
                    if (matchedItem != null) {
                        //matchedItem.chainEndDateTime(data.data.ChainEndDateTime);
                        matchedItem.chainEndDateTime(new Date(data.data.ChainEndDateTime + " UTC").format(com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate) + ' tfh:nn:ss.fff'));

                        var displayString = '';
                        if (matchedItem.chainEndDateTime() != "NA") {
                            displayString += 'Duration : ';

                            //Hour
                            if (data.data.ChainTimeTaken[0] != 0) {
                                displayString += data.data.ChainTimeTaken[0];
                                displayString += (data.data.ChainTimeTaken[0] == 1) ? ' hr ' : ' hrs ';
                            }

                            //Minutes
                            if (data.data.ChainTimeTaken[1] != 0) {
                                displayString += data.data.ChainTimeTaken[1];
                                displayString += (data.data.ChainTimeTaken[1] == 1) ? ' min ' : ' mins ';
                            }

                            //Seconds
                            if (data.data.ChainTimeTaken[2] != 0) {
                                displayString += data.data.ChainTimeTaken[2];
                                displayString += (data.data.ChainTimeTaken[2] == 1) ? ' sec ' : ' secs ';
                            }

                            //Milli Seconds
                            if (data.data.ChainTimeTaken[3] != 0) {
                                displayString += data.data.ChainTimeTaken[3];
                                displayString += (data.data.ChainTimeTaken[3] == 1) ? ' ms' : ' ms';
                            }
                        }

                        matchedItem.chainTimeTaken(displayString);

                        var displayString1 = '';

                        if (matchedItem.chainEndDateTime() != "NA") {
                            displayString1 = matchedItem.chainStartDateTime + ' to ' + matchedItem.chainEndDateTime();
                        }
                        else {
                            displayString1 = 'Requested At : ' + matchedItem.chainStartDateTime;
                        }

                        matchedItem.chainDateTimeDisplayString(displayString1);

                        matchedItem.chainDetailsDataFormatResponse = data.data.ChainDetailsDataFormatResponse;
                        matchedItem.chainDetailsData.ResponseHeaderData = data.data.ChainDetailsData.ResponseHeaderData;
                        matchedItem.chainDetailsData.ResponseBodyDataFileLocation = data.data.ChainDetailsData.ResponseBodyDataFileLocation;
                        matchedItem.chainDetailsData.ResponseBodyDataFileName = data.data.ChainDetailsData.ResponseBodyDataFileName;
                        //matchedItem.chainDetailsData.ResponseBodyDataToShow = data.data.ChainDetailsData.ResponseBodyDataToShow;

                        //Check if Selected API_Call is this one only, then load response
                        if (apimonitoring._selectedAPIUniqueId == data.data.ApiUniqueId) {

                            //Show Response Body Header
                            //////////////////////////////NOTE : NULL EXCEPTION HANDLING TO BE DONE ////////////////////////////
                            ////////////////////////SHOW SOME MESSAGE IF RESPONSE HEADER IS NOT AVAILABLE///////////////////////
                            //Newline Character Handling
                            //var tempStringResponseHeader = obj.chainDetailsData.ResponseHeaderData.replace(/(?:\r\n|\r|\n)/g, '<br />');
                            if (obj.chainDetailsData.ResponseHeaderData != "") {
                                $('#ApiMonitoringSelectedApiDetailsContainer2HeaderText').text(obj.chainDetailsData.ResponseHeaderData);
                            }
                            else {
                                $('#ApiMonitoringSelectedApiDetailsContainer2HeaderText').text("No response header available. Please check server logs for more details.");
                            }

                            //Check if Response Body is Empty, i.e., NULL Case, i.e. API Call OVER, But Not Completed
                            if (matchedItem.chainDetailsData.ResponseBodyDataFileLocation != "") {

                                //if (matchedItem.chainDetailsData.ResponseBodyDataToShow) {
                                //Service Call to get contents of the file, if required
                                //Global String for Copy is set in onSuccess function

                                var params = {};
                                params.InputObject = {};
                                params.InputObject.DataFormat = matchedItem.chainDetailsDataFormatResponse;
                                params.InputObject.RequestOrResponse = "Response";
                                params.InputObject.BodyFileLocation = matchedItem.chainDetailsData.ResponseBodyDataFileLocation;

                                //Call Service
                                CallCommonServiceMethod('GetFileContent', params, OnSuccess_LoadApiCallDetailsBodyContent, OnFailure, null, false);
                                //}
                                //else {
                                //    $('#ApiMonitoringSelectedApiDetailsContainer2Text').text(obj.chainDetailsData.ResponseBodyErrorMsg);

                                //    //Disable Copy Icon for Response Body
                                //    $("#ApiMonitoringSelectedApiDetailsContainer2CopyToClipboardIcon").css('pointer-events', 'none');

                                //    //Can Highlight the Download Icons

                                //}

                                ////Enable Download Icon for Response Body and set Global Variables for the File
                                //$("#ApiMonitoringSelectedApiDetailsContainer2DownloadIcon").css('pointer-events', 'auto');
                                apimonitoring._ResponseBodyFileFullPath = matchedItem.chainDetailsData.ResponseBodyDataFileLocation;
                                apimonitoring._ResponseBodyFileName = matchedItem.chainDetailsData.ResponseBodyDataFileName;
                                ////apimonitoring._ResponseBodyFileContentType = obj.chainDetailsData.ResponseBodyDataFileContentType;
                            }
                            else {

                                $('#ApiMonitoringSelectedApiDetailsContainer2Text').text(matchedItem.chainDetailsData.RequestBodyErrorMsg);

                                //Disable Copy & Download Icons for Response Body
                                $("#ApiMonitoringSelectedApiDetailsContainer2CopyToClipboardIcon").css('pointer-events', 'none');
                                $("#ApiMonitoringSelectedApiDetailsContainer2DownloadIcon").css('pointer-events', 'none');

                                //Clear Response Body Global Variables
                                apimonitoring._ResponseBodyFileContent = "";
                                apimonitoring._ResponseBodyFileFullPath = "";
                                apimonitoring._ResponseBodyFileName = "";
                                apimonitoring._ResponseViewLogContent = "";
                                //apimonitoring._ResponseBodyFileContentType = "";
                            }

                        }
                    }
                }

                $(".ApiMonitoringChain").css("display", "block");
            });
        });
    }

    //Disconnecting Connection to Node Server
    function disconnectNodeConnection() {
        //apimonitoring._iosocket.emit('end');
        apimonitoring._iosocket.disconnect();
    }

    //Filtering Data Incoming From Node Server
    function FilterDataIncomingFromNodeServer(data) {

        //var CSDT = new Date(data.ChainStartDateTime);
        var CSDT = data.ChainStartDateTime != null ? Date.parseInvariant(data.ChainStartDateTime, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate)) : null;
        
        CSDT.setHours(0, 0, 0, 0);

        //Filtering on Date
        //if ((CSDT >= (new Date(apimonitoring._selectedStartDate))) && (CSDT <= (new Date(apimonitoring._selectedEndDate)))) {
        if ((apimonitoring._selectedStartDate == null || CSDT >= (Date.parseInvariant(apimonitoring._selectedStartDate, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate)))) && (apimonitoring._selectedEndDate == null || CSDT <= (Date.parseInvariant(apimonitoring._selectedEndDate, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate))))) {

            //Filtering on URL
            if (apimonitoring._selectedUrl.length > 0 && (apimonitoring._selectedUrl[0] == "-1" || ko.utils.arrayFirst(apimonitoring._selectedUrl, function (item) { return item == data.ChainURL; }))) {

                //Filtering on Method
                if (apimonitoring._selectedMethod.length > 0 && (apimonitoring._selectedMethod[0] == "-1" || ko.utils.arrayFirst(apimonitoring._selectedMethod, function (item) { return item == data.ChainMethod; }))) {

                    //Filtering on ClientMachine
                    if (apimonitoring._selectedClientMachine.length > 0 && (apimonitoring._selectedClientMachine[0] == "-1" || ko.utils.arrayFirst(apimonitoring._selectedClientMachine, function (item) { return item == data.ChainClientMachine; }))) {

                        //Appending Data to our Observable Array
                        apimonitoring._pageViewModelInstance.chainListing.unshift(new chainViewModelNode(data));
                    }
                }
            }
        }
    }


    /////////////////////////////////////////////////////////////
    //init function to Load Initial Data as per Default Filters//
    /////////////////////////////////////////////////////////////
    function init() {

        // Get data from Service Call
        var params = {};
        params.InputObject = {};
        params.InputObject.selectedUrl = apimonitoring._selectedUrl;
        params.InputObject.selectedMethod = apimonitoring._selectedMethod;
        params.InputObject.selectedClientMachine = apimonitoring._selectedClientMachine;

        //Date Culture Change
        //params.InputObject.selectedStartDate = apimonitoring._selectedStartDate;
        //params.InputObject.selectedEndDate = apimonitoring._selectedEndDate;
        var selectedStartDateTemp = apimonitoring._selectedStartDate!= null ? Date.parseInvariant(apimonitoring._selectedStartDate, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate)) : null;
        var selectedEndDateTemp = apimonitoring._selectedEndDate != null ? Date.parseInvariant(apimonitoring._selectedEndDate, com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate)) : null;

        var selectedStartDate = selectedStartDateTemp != null ? new Date(selectedStartDateTemp.toUTCString()).format(com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate)) : null;
        var selectedEndDate = selectedEndDateTemp != null ? new Date(selectedEndDateTemp.toUTCString()).format(com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate)) : null;

        params.InputObject.selectedStartDate = selectedStartDate;
        params.InputObject.selectedEndDate = selectedEndDate;

        CallCommonServiceMethod('GetApiCallsData', params, OnSuccess_LoadApiCallsDataInitial, OnFailure, null, false);
    }

    ////////////////////////////
    // "Initializer" function //
    ////////////////////////////
    ApiMonitoring.prototype.Initializer = function Initializer(controlInfo) {
        //Hide Error Msg Div on Load
        if (typeof (window.parent.leftMenu) !== "undefined") {
            window.parent.leftMenu.hideNoRecordsMsg();
        }
        $('#ApiMonitoringBody').height(window.innerHeight);
        apimonitoring._controlIdInfo = eval("(" + controlInfo + ")");

        getMultiSelectFilters();

        //ApplyMultiSelect('ApiMonoitoringUrlFilterMultiSelectedItems', false, UrlData, true, '#ApiMonoitoringUrlFilterMultiSelect', 'URL', true);
        //ApplyMultiSelect('ApiMonoitoringMethodFilterMultiSelectedItems', false, MethodData, true, '#ApiMonoitoringMethodFilterMultiSelect', 'Method', true);
        //ApplyMultiSelect('ApiMonoitoringClientMachineFilterMultiSelectedItems', false, ClientMachineData, true, '#ApiMonoitoringClientMachineFilterMultiSelect', 'ClientMachine', true);

        //Load SMDatePickerControl
        initializeDivFilterDate();

        //Set height of Panel Top
        $('#ApiMonitoringTop').height($(window).width() > 1400 ? 54 : 89);

        $('#ApiMonitoringBody').height($(window).height());

        //Setting Height of Area below the Filters
        $(".ApiMonitoringMainColumns").height($('#ApiMonitoringBody').height() - $('#ApiMonitoringTop').height() - 2);
        $(".ApiMonitoringMain").height($('#ApiMonitoringBody').height() - $('#ApiMonitoringTop').height() - 2);
        $("#ApiMonitoringMainErrorDiv").height($('#ApiMonitoringBody').height() - $('#ApiMonitoringTop').height() - 2);

        $("#ApiMonitoringSelectedApiDetailsContainer1").height($(".ApiMonitoringMainColumns").height() / 2);
        $("#ApiMonitoringSelectedApiDetailsContainer2").height($(".ApiMonitoringMainColumns").height());
        $("#ApiMonitoringChainSuperParentDiv").height($('#ApiMonitoringMainLeftColumn').height() - 30);

        //Body Heights
        $('#ApiMonitoringSelectedApiDetailsContainer1Text').height($("#ApiMonitoringSelectedApiDetailsContainer1").height() - 70);
        $('#ApiMonitoringSelectedApiDetailsContainer2Text').height($("#ApiMonitoringSelectedApiDetailsContainer2").height() - 70);

        //Header Heights
        $('#ApiMonitoringSelectedApiDetailsContainer1HeaderText').height($("#ApiMonitoringSelectedApiDetailsContainer1").height() - 70);
        $('#ApiMonitoringSelectedApiDetailsContainer2HeaderText').height($("#ApiMonitoringSelectedApiDetailsContainer2").height() - 70);

        //init
        //init();        

        //Initializing Slim Scrolls
        //initializeSlimScrolls();        

    }

    $(window).resize(function () {
        //Set height of Panel Top
        $('#ApiMonitoringTop').height($(window).width() > 1400 ? 54 : 89);

        $('#ApiMonitoringBody').height($(window).height());

        //Setting Height of Area below the Filters
        $(".ApiMonitoringMainColumns").height($('#ApiMonitoringBody').height() - $('#ApiMonitoringTop').height() - 2);
        $(".ApiMonitoringMain").height($('#ApiMonitoringBody').height() - $('#ApiMonitoringTop').height() - 2);
        $("#ApiMonitoringMainErrorDiv").height($('#ApiMonitoringBody').height() - $('#ApiMonitoringTop').height() - 2);

        $("#ApiMonitoringSelectedApiDetailsContainer1").height($(".ApiMonitoringMainColumns").height() / 2);
        $("#ApiMonitoringSelectedApiDetailsContainer2").height($(".ApiMonitoringMainColumns").height());
        $("#ApiMonitoringChainSuperParentDiv").height($('#ApiMonitoringMainLeftColumn').height() - 30);

        //Body Heights
        $('#ApiMonitoringSelectedApiDetailsContainer1Text').height($("#ApiMonitoringSelectedApiDetailsContainer1").height() - 70);
        $('#ApiMonitoringSelectedApiDetailsContainer2Text').height($("#ApiMonitoringSelectedApiDetailsContainer2").height() - 70);

        //Header Heights
        $('#ApiMonitoringSelectedApiDetailsContainer1HeaderText').height($("#ApiMonitoringSelectedApiDetailsContainer1").height() - 70);
        $('#ApiMonitoringSelectedApiDetailsContainer2HeaderText').height($("#ApiMonitoringSelectedApiDetailsContainer2").height() - 70);

        //CSS for smNoRecordsContainer div created inside ApiMonitoringMainErrorDiv div
        var pd_left = ($("#ApiMonitoringMainErrorDiv").width() - $("#smNoRecordsContainer").width() - 2) / 2;
        var pd_top = (($("#ApiMonitoringMainErrorDiv").height() - $("#smNoRecordsContainer").height() - 2) / 2) - 20;
        $("#ApiMonitoringMainErrorDiv #smNoRecordsContainer").css({ "padding-left": pd_left, "padding-top": pd_top });

    })


    return apimonitoring;
})();