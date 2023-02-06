
RADTransportConfig = {};
RADTransportConfig.initialize = function (initObj) {
    $.extend(RADTransportConfig, initObj);
    if (!initObj.IsIagoDependent) {        
        
        var $transportTabDiv = $("<div>", {
            id: "RTransportTabDiv",
            class: "RTransportTab"
        });
        $("#" + RADTransportConfig.contentBodyId).empty();
        $("#" + RADTransportConfig.contentBodyId).append($transportTabDiv);
    }

    var obj = new RADTransportConfiguration();
    obj.init();
};

var RADTransportConfiguration = function () {

};

RADTransportConfiguration.prototype.init = function (sandBox) {
    RADTransportConfiguration.instance = this;
    RADTransportConfiguration.instance.LoadTabInfo();
}

RADTransportConfiguration.prototype.LoadTabInfo = function () {

    RADTransportConfiguration.instance.AuthorizationPrivileges = [];
    RADTransportConfiguration.instance.CurrentSelectedTransportType = null;
    RADTransportConfiguration.instance.CurrentSelectedTransportItem = null;
    RADTransportConfiguration.instance.CurrentTransPortDetails = null;
    RADTransportConfiguration.instance.IsCreation = false;
    RADTransportConfiguration.instance.AllTransPortUIInfo =
        RADTransportConfiguration.instance.GetTransportUIInfo();

    RADTransportConfiguration.prototype.tabInfo = {
        tab: [
            {
                id: "FTP",
                name: "FTP",
                isDefault: true
            },
            {
                id: "MSMQ",
                name: "MSMQ"
            },
            {
                id: "WFT",
                name: "WFT"
            },
            {
                id: "SMTP",
                name: "SMTP"
            },
            {
                id: "SFTP",
                name: "SFTP"
            },
            {
                id: "IMAP",
                name: "IMAP"
            },
            {
                id: "MAPI",
                name: "MAPI"
            },
            {
                id: "WMQ",
                name: "WMQ"
            },
            {
                id: "KafkaMQ",
                name: "KafkaMQ"
            },
            {
                id: "SQL",
                name: "SQL"
            },
            {
                id: "S3",
                name: "S3"
            },
            {
                id: "RabbitMQ",
                    name: "RabbitMQ"
            }


        ]
    };
    RADTransportConfiguration.instance.bindTabs();
}

RADTransportConfiguration.prototype.GetTransportUIInfo = function () {

    //Hardcoded values - Todo - can we get this info from service?


    var AllTransPortUIInfo = [
        {
            TransportType: "FTP",
            NonEditableColumn: "TransportName",
            UniquePropertyName: "TransportName",
            UniquePropertyDescription: "File Transfer Protocol",
            LeftMenuColumnsbyOrder: ['TransportName', 'ServerName'],

            //Order use to display the proerties on UI accoring to order
            // Type is used for validations, making the control, for example : bool is used to get checkbox            
            PropertyDetails: [
                                { PropertyName: "TransportName", DisplayName: "Name", Type: "String", Order: 0 },
                                { PropertyName: "ServerName", DisplayName: "Server Name", Type: "String", Order: 1 },
                                { PropertyName: "UserName", DisplayName: "User Name", Type: "String", Order: 2 },
                                { PropertyName: "PassWord", DisplayName: "Password", Type: "Password", Order: 3 },
                                { PropertyName: "PollingInterrval", DisplayName: "Polling Interval", Type: "Numeric", Order: 4 },
                                { PropertyName: "PollingTimeout", DisplayName: "Polling Time Out", Type: "Numeric", Order: 5 },
                                { PropertyName: "PortNumber", DisplayName: "Port Number", Type: "Numeric", Order: 6 },
                                { PropertyName: "EnableSSL", DisplayName: "Enable SSL", Type: "Bool", Order: 7 },
                                { PropertyName: "UsePassive", DisplayName: "UsePassive", Type: "Bool", Order: 8 }
            ]
        },
        {
            TransportType: "MSMQ",
            NonEditableColumn: "TransportName",
            UniquePropertyName: "TransportName",
            UniquePropertyDescription: "Microsoft Message Queue",
            LeftMenuColumnsbyOrder: ['TransportName', 'HandlerClassName'],

            PropertyDetails: [
                               { PropertyName: "TransportName", DisplayName: "Name", Type: "String", Order: 0 },
                               { PropertyName: "InQueue", DisplayName: "In Queue", Type: "String", Order: 1 },
                               { PropertyName: "OutQueue", DisplayName: "Out Queue", Type: "String", Order: 2 },
                               { PropertyName: "TimeOut", DisplayName: "Polling Time Out", Type: "Numeric", Order: 3 },
                               { PropertyName: "IsTransactional", DisplayName: "Is Server", Type: "Bool", Order: 4 },
                               { PropertyName: "PersistMessage", DisplayName: "Is Persistent Message", Type: "Bool", Order: 5 },
                               { PropertyName: "HandlerClassName", DisplayName: "Handler", Type: "String", Order: 6 }

            ]
        },
         {

             TransportType: "WFT",
             NonEditableColumn: "TransportName",
             UniquePropertyName: "TransportName",
             UniquePropertyDescription: "Windows File Transfer",
             LeftMenuColumnsbyOrder: ['TransportName', 'ServerName'],

             PropertyDetails: [
                               { PropertyName: "TransportName", DisplayName: "Name", Type: "String", Order: 0 },
                               { PropertyName: "ServerName", DisplayName: "Server Name", Type: "String", Order: 1 },
                               { PropertyName: "UserName", DisplayName: "User Name", Type: "String", Order: 2 },
                               { PropertyName: "Password", DisplayName: "Password", Type: "Password", Order: 3 },
                               { PropertyName: "DomainName", DisplayName: "Domain Name", Type: "String", Order: 4 },
                               { PropertyName: "PollingInterval", DisplayName: "Polling Interval", Type: "Numeric", Order: 5 },
                               { PropertyName: "PollingTimeout", DisplayName: "Polling Time Out", Type: "Numeric", Order: 6 }

             ]

         },
        {
            TransportType: "SMTP",
            NonEditableColumn: "TransportName",
            UniquePropertyName: "TransportName",
            UniquePropertyDescription: "Simple Mail Transfer Protocol",
            LeftMenuColumnsbyOrder: ['TransportName', 'SMTPServer'],

            //Order use to display the proerties on UI accoring to order
            // Type is used for validations, making the control, for example : bool is used to get checkbox            
            PropertyDetails: [
                                { PropertyName: "TransportName", DisplayName: "Name", Type: "String", Order: 0 },
                                { PropertyName: "DomainName", DisplayName: "Domain Name", Type: "String", Order: 1 },
                                { PropertyName: "SMTPServer", DisplayName: "Server Name", Type: "String", Order: 2 },
                                { PropertyName: "SMTPPort", DisplayName: "Port", Type: "Numeric", Order: 3 },
                                { PropertyName: "SMTPAuthenticationUserName", DisplayName: "User Name", Type: "String", Order: 4 },
                                { PropertyName: "SMTPAuthenticationPassword", DisplayName: "Password", Type: "Password", Order: 5 },
                                { PropertyName: "TimeOut", DisplayName: "Time Out", Type: "Numeric", Order: 6 },
                                { PropertyName: "EnableSSL", DisplayName: "Enable SSL", Type: "Bool", Order: 7 }
            ]
        },
        {
            TransportType: "SFTP",
            NonEditableColumn: "TransportName",
            UniquePropertyName: "TransportName",
            UniquePropertyDescription: "Simple File Transfer Protocol",
            LeftMenuColumnsbyOrder: ['TransportName', 'Host'],

            PropertyDetails: [
                               { PropertyName: "TransportName", DisplayName: "Name", Type: "String", Order: 0 },
                               { PropertyName: "Host", DisplayName: "Server Name", Type: "String", Order: 1 },
                               { PropertyName: "UserName", DisplayName: "User Name", Type: "String", Order: 2 },
                               { PropertyName: "Password", DisplayName: "Password", Type: "Password", Order: 3 },
                               { PropertyName: "PortNumber", DisplayName: "Port", Type: "Numeric", Order: 4 },
                               { PropertyName: "PollingInterval", DisplayName: "Polling Interval", Type: "Numeric", Order: 5 },
                               { PropertyName: "PollingTimeout", DisplayName: "Polling Time out", Type: "Numeric", Order: 6 },
                               { PropertyName: "PrivateKey", DisplayName: "Private Key", Type: "String", Order: 7 }
            ]


        },
        {
            TransportType: "IMAP",
            NonEditableColumn: "TransportName",
            LeftMenuColumnsbyOrder: ['TransportName', 'ImapServer'],
            UniquePropertyName: "TransportName",
            UniquePropertyDescription: "Internet Message Access Protocol",
            PropertyDetails: [
                               { PropertyName: "TransportName", DisplayName: "Name", Type: "String", Order: 0 },
                               { PropertyName: "ImapServer", DisplayName: "Server Name", Type: "String", Order: 1 },
                               { PropertyName: "Username", DisplayName: "User Name", Type: "String", Order: 2 },
                               { PropertyName: "Password", DisplayName: "Password", Type: "Password", Order: 3 },
                               { PropertyName: "Port", DisplayName: "Port", Type: "Numeric", Order: 4 },
                               { PropertyName: "LastDownloadedMailIndex", DisplayName: "Last Downloaded Mail Index", Type: "Numeric", Order: 5 },
                               { PropertyName: "EnableSSL", DisplayName: "Enable SSL", Type: "Bool", Order: 6 }
            ]
        },
        {
            TransportType: "MAPI",
            NonEditableColumn: "TransportName",
            LeftMenuColumnsbyOrder: ['TransportName', 'ServerName'],
            UniquePropertyName: "TransportName",
            UniquePropertyDescription: "Messaging Application Programming Interface",
            PropertyDetails: [
                              { PropertyName: "TransportName", DisplayName: "Name", Type: "String", Order: 0 },
                              { PropertyName: "ServerName", DisplayName: "Server Name", Type: "String", Order: 1 },
                              { PropertyName: "UserName", DisplayName: "User Name", Type: "String", Order: 2 },
                              { PropertyName: "Password", DisplayName: "Password", Type: "Password", Order: 3 },
                              { PropertyName: "Version", DisplayName: "Version", Type: "Numeric", Order: 4 },
                              { PropertyName: "AutoDiscoverUrl", DisplayName: "Auto Discove rUrl", Type: "Bool", Order: 6 }
            ]
        },
        {
            TransportType: "WMQ",
            NonEditableColumn: "TransportName",
            LeftMenuColumnsbyOrder: ['TransportName', 'HostName'],
            UniquePropertyName: "TransportName",
            UniquePropertyDescription: "Windows Messaging Queue",
            PropertyDetails: [
                              { PropertyName: "TransportName", DisplayName: "Name", Type: "String", Order: 0 },
                              { PropertyName: "HostName", DisplayName: "Server Name", Type: "String", Order: 1 },
                              { PropertyName: "Channel", DisplayName: "Channel", Type: "String", Order: 2 },
                              { PropertyName: "QueueManager", DisplayName: "Queue Manager", Type: "String", Order: 3 },
                              { PropertyName: "QueueName", DisplayName: "Queue Name", Type: "String", Order: 4 },
                              { PropertyName: "Port", DisplayName: "Port", Type: "Numeric", Order: 5 }
            ]
        },
        {
            TransportType: "KafkaMQ",
            NonEditableColumn: "TransportName",
            LeftMenuColumnsbyOrder: ['TransportName', 'BrokerList'],
            UniquePropertyName: "TransportName",
            UniquePropertyDescription: "Kafka Messaging Queue",
            PropertyDetails: [
                              { PropertyName: "TransportName", DisplayName: "Name", Type: "String", Order: 0 },
                              { PropertyName: "BrokerList", DisplayName: "Broker List", Type: "String", Order: 1 },
                              { PropertyName: "Topic", DisplayName: "Topic", Type: "String", Order: 2 }
            ]
        }
        ,
        {
            TransportType: "SQL",
            NonEditableColumn: "TransportName",
            LeftMenuColumnsbyOrder: ['TransportName', 'ServerName'],
            UniquePropertyName: "TransportName",
            UniquePropertyDescription: "SQL Server",
            PropertyDetails: [
                               { PropertyName: "TransportName", DisplayName: "Name", Type: "String", Order: 0 },
                                { PropertyName: "ServerName", DisplayName: "Server Name", Type: "String", Order: 1 },
                                { PropertyName: "Authentication", DisplayName: "Authentication", Type: "authenticationtoggle", Order: 2 },
                                { PropertyName: "UserName", DisplayName: "User Name", Type: "String", Order: 3 },
                                { PropertyName: "Password", DisplayName: "Password", Type: "Password", Order: 4 },
                                { PropertyName: "DBName", DisplayName: "DBName", Type: "String", Order: 5 },
                                { PropertyName: "TableName", DisplayName: "Table Name", Type: "String", Order: 6 }
            ]
        },
        {
            TransportType: "S3",
            NonEditableColumn: "TransportName",
            UniquePropertyName: "TransportName",
            UniquePropertyDescription: "Simple Storage Service",
            LeftMenuColumnsbyOrder: ['TransportName', 'BucketName'],

            //Order use to display the proerties on UI accoring to order
            // Type is used for validations, making the control, for example : bool is used to get checkbox            
            PropertyDetails: [
                                { PropertyName: "TransportName", DisplayName: "Name", Type: "String", Order: 0 },
                                { PropertyName: "AccessKey", DisplayName: "Access Key", Type: "String", Order: 1 },
                                { PropertyName: "SecretKey", DisplayName: "Secret Key", Type: "String", Order: 2 },
                                { PropertyName: "RegionName", DisplayName: "Region Name", Type: "String", Order: 3 },
                                { PropertyName: "BucketName", DisplayName: "Bucket Name", Type: "String", Order: 4 },
                                { PropertyName: "PollingInterval", DisplayName: "Polling Interval", Type: "Numeric", Order: 5 },
                                { PropertyName: "PollingTimeout", DisplayName: "Polling Time out", Type: "Numeric", Order: 6 }
            ]
        },
        {
            TransportType: "RabbitMQ",
            NonEditableColumn: "TransportName",
            UniquePropertyName: "TransportName",
            UniquePropertyDescription: "Rabbit Message Queue",
            LeftMenuColumnsbyOrder: ['TransportName', 'HostName'],

            //Order use to display the proerties on UI accoring to order
            // Type is used for validations, making the control, for example : bool is used to get checkbox            
            PropertyDetails: [
                                { PropertyName: "TransportName", DisplayName: "Name", Type: "String", Order: 0 },
                                { PropertyName: "HostName", DisplayName: "Server Name", Type: "String", Order: 1 },
                                { PropertyName: "UserName", DisplayName: "User Name", Type: "String", Order: 2 },
                                { PropertyName: "Password", DisplayName: "Password", Type: "Password", Order: 3 },
                                { PropertyName: "QueueName", DisplayName: "Queue Name", Type: "String", Order: 4 },
                                { PropertyName: "Port", DisplayName: "Port", Type: "Numeric", Order: 5 },
                                { PropertyName: "Durable", DisplayName: "Durable", Type: "Bool", Order: 6 },
                                { PropertyName: "Exclusive", DisplayName: "Exclusive", Type: "Bool", Order: 7 },
                                { PropertyName: "AutoDelete", DisplayName: "Auto Delete", Type: "Bool", Order: 8 },
                                { PropertyName: "Persistent", DisplayName: "Persistent", Type: "Bool", Order: 9 }

                               
            ]
        }
    ];
    return AllTransPortUIInfo;
}

RADTransportConfiguration.prototype.bindTabs = function () {
    if (RADTransportConfig.IsIagoDependent) {
        if ($("#pageHeaderTabPanel").data("iago-widget-tabs") != null)
            $("#pageHeaderTabPanel").data("iago-widget-tabs").destroy();
        $("#pageHeaderTabPanel").tabs({
            tabSchema: RADTransportConfiguration.instance.tabInfo,
            tabClickHandler: RADTransportConfiguration.instance.OnPageLoad,
            tabContentHolder: "contentBody"
        });
    }
    else {
        $("#RTransportTabDiv").empty();
        $("#RTransportTabDiv").toolKittabs({
            tabSchema: { tab: RADTransportConfiguration.prototype.tabInfo.tab },
            tabClickHandler: RADTransportConfiguration.instance.OnPageLoad,
            tabContentHolder: RADTransportConfig.contentBodyId
        });

    }
        
        

    
}

RADTransportConfiguration.prototype.OnPageLoad = function (selectedTranportType, tabContentContainer) {

    RADTransportConfiguration.instance.CurrentSelectedTransportType = selectedTranportType;

    //Adding Spinner
    //$('#RADTransportMainDiv').addClass('spinner');
    var url = RADTransportConfig.baseUrl + "/Resources/Services/RADTransport.svc";
    
    $.ajax({
        url: url + '/GetTagTemplate',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ fileName: 'RADTransportConfiguration.html' })
    }).then(function (responseText) {

        //$('#RADTransportMainDiv').removeClass('spinner');
        //$("#" + RADTransportConfig.contentBodyId).empty();
        $("#RADTransportMainDiv").remove();
        $("#" + RADTransportConfig.contentBodyId).append(responseText.d);

        RADTransportConfiguration.instance.GetAuthorizationPrivileges(selectedTranportType);

        //By default hide save and cancel button
        $("#RADTransportSaveButtonParentDiv").hide();
        $("#RADTransportCancelButtonParentDiv").hide();

        var currentTransportUIInfo = $.grep(RADTransportConfiguration.instance.AllTransPortUIInfo, function (e)
        { return e.TransportType == selectedTranportType; });
        //UI LEVEL CHNAGES :: Adding the label title as per sub level name
        $('#lblRADTransportNameParentId').html(currentTransportUIInfo[0].UniquePropertyDescription)

                
        RADTransportConfiguration.instance.CalculateHeight();
        RADTransportConfiguration.instance.BindEventsCommon();        
    });
}

RADTransportConfiguration.prototype.BindEventsCommon = function () {

    //Create Button
    $("#btnRADCreateTransportId").unbind().click(function (event) {
        RADTransportConfiguration.instance.OnCreateTransportDetail();
    });

    //Save Button
    $("#btnRADTransportSaveButtonId").unbind().click(function (event) {
        RADTransportConfiguration.instance.OnSaveTransportDetail();
    });

    //Camcel Button
    $("#btnRADTransportCancelButtonId").unbind().click(function (event) {
        RADTransportConfiguration.instance.OnCancelTransportDetail();
    });

    //Delete Button
    $("#btnRADTransportDeleteButtonId").unbind().click(function (event) {
        RADTransportConfiguration.instance.OnDeleteTransportDetail();
    });

    //Edit Button
    $("#btnRADTransportEditButtonId").unbind().click(function (event) {
        RADTransportConfiguration.instance.OnEditTransportDetail();
        RADTransportConfiguration.prototype.RegisterOnOffBtnClickHandler(event);
    });

    //search
    $(".RADTransportSearchViewInput").unbind().click(function (event) {
        if ($('#txtRADTransportLeftSectionSearch:visible').length) {
            $("#txtRADTransportLeftSectionSearch").hide("slow", function () {
                // Animation complete.
            });
        }
        else {
            $("#txtRADTransportLeftSectionSearch").show("slow", function () {
            });
        }
    });

    //search keyup
    $("#txtRADTransportLeftSectionSearch").unbind("keyup").keyup(function (event) {
        RADTransportConfiguration.instance.SearchTransport($(event.target).val());
    })

    //scrolling buttons
    $(".RADTranportScrollingDiv").unbind().click(function (event) {
        RADTransportConfiguration.instance.ScrollData(event);
    })
}

RADTransportConfiguration.prototype.ScrollData = function (event) {

    if ($(event.target).hasClass("RADTransportLeftSectionScrollDown")) {
        $(".RADTransportLeftSectionContent").scrollTop($(".RADTransportLeftSectionContent").scrollTop() + 50);
    }
    else if ($(event.target).hasClass("RADTransportLeftSectionScrollUp")) {
        $(".RADTransportLeftSectionContent").scrollTop($(".RADTransportLeftSectionContent").scrollTop() - 50);
    }

}

RADTransportConfiguration.prototype.SearchTransport = function (transportName) {

    var self = this;
    var transportName = transportName.trim().toLowerCase();
    var length = $(".RADTransportLeftSectionContent").children().length;

    for (var i = 0; i < length; i++) {
        if (($($(".RADTransportLeftSectionContent").children()[i])).find(".RADTransportTiles").html().trim().toLowerCase().indexOf(transportName) != -1) {
            $($(".RADTransportLeftSectionContent").children()[i]).removeClass("RADTransportHiddenClass");
        }
        else {
            $($(".RADTransportLeftSectionContent").children()[i]).addClass("RADTransportHiddenClass");
        }
    }
}

RADTransportConfiguration.prototype.CalculateHeight = function () {
    var totalHeight = $("#RADTransportMainDiv").height();
    $("#RADTransportLeftSectionContentDiv").height(totalHeight - 100);
}

RADTransportConfiguration.prototype.GetTransportDetailsbyType = function (selectedTranportType) {
    
    $('#RADTransportMainDiv').addClass('spinner');    
    var url = RADTransportConfig.baseUrl + "/Resources/Services/RADTransport.svc";

    $.ajax({
        url: url + '/GetAllTransport',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ transportType: selectedTranportType })
    }).then(function (responseText) {

        if (responseText.d != null && responseText.d != "" && responseText.d != "[]") {

            var transPortDetails = JSON.parse(responseText.d);
            RADTransportConfiguration.instance.TransPortDetails = transPortDetails;
            //Bind LeftMenu
            var currentSelectedTransportItem = RADTransportConfiguration.instance.BindLeftMenu(selectedTranportType, transPortDetails);
            //Bind default selected transport details
            RADTransportConfiguration.instance.BindTransportDetails(selectedTranportType, currentSelectedTransportItem, transPortDetails);
        }
        $('#RADTransportMainDiv').removeClass('spinner');
    });
}

RADTransportConfiguration.prototype.BindLeftMenu = function (selectedTranportType, transPortDetails) {
    $('#RADTransportLeftSectionContentDiv').empty();
    var currentTransportUIInfo = $.grep(RADTransportConfiguration.instance.AllTransPortUIInfo, function (e)
    { return e.TransportType == selectedTranportType; });

    //setting up the label for transport by default to first server name
    //if (transPortDetails != undefined && Array.isArray(transPortDetails) && transPortDetails.length > 0 && transPortDetails[0].hasOwnProperty("TransportName")) {
    //    $('#lblRADTransportNameParentId').html(transPortDetails[0].TransportName);
    //}
    //else {
    //    $('#lblRADTransportNameParentId').html(currentTransportUIInfo[0].UniquePropertyDescription);
    //}

    if (currentTransportUIInfo.length > 0 && currentTransportUIInfo[0].hasOwnProperty("UniquePropertyDescription") && currentTransportUIInfo[0].UniquePropertyDescription) {
        $('#lblRADTransportNameParentId').html(currentTransportUIInfo[0].UniquePropertyDescription);
    }

    for (var i = 0; i < transPortDetails.length; i++) {
        $('#RADTransportLeftSectionContentDiv')
        .append('<div class="RADTransportTileParent"><div class = "RADTransportTile"><div class="RADTransportTiles" title="'
        + transPortDetails[i][currentTransportUIInfo[0].LeftMenuColumnsbyOrder[0]] + '"  id="RADTransportName' + i + '" name="'
        + transPortDetails[i][currentTransportUIInfo[0].LeftMenuColumnsbyOrder[0]] + '">'
        + transPortDetails[i][currentTransportUIInfo[0].LeftMenuColumnsbyOrder[0]]
        + '</div><div class="RADTransportDesc">' + transPortDetails[i][currentTransportUIInfo[0].LeftMenuColumnsbyOrder[1]] + '</div></div></div>');

        if (i == 0) {
            RADTransportConfiguration.instance.CurrentSelectedTransportItem = transPortDetails[i][currentTransportUIInfo[0].LeftMenuColumnsbyOrder[0]];
            //RADTransportConfiguration.instance.CurrentSelectedTransportItem = transPortDetails[i][currentTransportUIInfo[0].UniquePropertyName];

            $("#RADTransportName" + i).closest(".RADTransportTile").addClass('RADTransportTileActive');
            $("#RADTransportName" + i).closest(".RADTransportTile")
                .append("<div id=\"arr\" class=\"fa fa-caret-right RADTrasnportArrow-Right \"></div>");
        }
    }

    $(".RADTransportTile").unbind().click(function (event) {

        $("#RADTransportSaveButtonParentDiv").hide();
        $("#RADTransportCancelButtonParentDiv").hide();
        RADTransportConfiguration.instance.ShowHideByPrivilegeType();

        $(".RADTrasnportArrow-Right").remove();
        $(".RADTransportTileActive").removeClass("RADTransportTileActive");
        $(event.target).closest(".RADTransportTile").addClass("RADTransportTileActive");
        $(event.target).closest(".RADTransportTile").append("<div id=\"arr\" class=\"fa fa-caret-right RADTrasnportArrow-Right \"></div>");
        var currentSelectedTransportItem = $(event.target).closest(".RADTransportTile").find(".RADTransportTiles").html().trim();
        RADTransportConfiguration.instance.CurrentSelectedTransportItem = currentSelectedTransportItem;

        RADTransportConfiguration.prototype.GetTransportDetails(
            RADTransportConfiguration.instance.CurrentSelectedTransportType,
            RADTransportConfiguration.instance.CurrentSelectedTransportItem);

        //setting the label name on click handler for each type messaging queue
        //$('#lblRADTransportNameParentId').html(RADTransportConfiguration.instance.CurrentSelectedTransportItem);
    });
    return RADTransportConfiguration.instance.CurrentSelectedTransportItem;
}

RADTransportConfiguration.prototype.BindTransportDetails = function (selectedTranportType, currentSelectedTransportItem, transPortDetails) {

    var currentTransportUIInfo = $.grep(RADTransportConfiguration.instance.AllTransPortUIInfo, function (e)
    { return e.TransportType == selectedTranportType; });

    var currentTransportDetails = $.grep(transPortDetails, function (e)
    { return e.TransportName == currentSelectedTransportItem; });

    var currentTransportItemDetails = currentTransportDetails[0];
    RADTransportConfiguration.instance.CurrentTransPortDetails = currentTransportItemDetails;
    RADTransportConfiguration.instance.BindCurrentTransportdetails(selectedTranportType, currentTransportItemDetails);
}

RADTransportConfiguration.prototype.GetTransportDetails = function (selectedTranportType, currentSelectedTransportItem) {
    
    $('#RADTransportMainDiv').addClass('spinner');
    var url = RADTransportConfig.baseUrl + "/Resources/Services/RADTransport.svc";
    $.ajax({
        url: url + '/GetTransportDetails',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ transportName: currentSelectedTransportItem,  transportType: selectedTranportType })
    }).then(function (responseText) {

        $('#RADTransportMainDiv').removeClass('spinner');
        var currentTransportItemDetails = JSON.parse(responseText.d);
        RADTransportConfiguration.instance.CurrentTransPortDetails = currentTransportItemDetails;
        RADTransportConfiguration.instance.BindCurrentTransportdetails(selectedTranportType, currentTransportItemDetails);
    });
}

RADTransportConfiguration.prototype.BindCurrentTransportdetails = function (selectedTranportType, currentTransportItemDetails) {

    var currentTransportUIInfo = $.grep(RADTransportConfiguration.instance.AllTransPortUIInfo, function (e)
    { return e.TransportType == selectedTranportType; });

    var sortedPropertyInfo = currentTransportUIInfo[0].PropertyDetails.sort(function (a, b) {
        return a.Order - b.Order
    });

    $('#RADTranportRightSectionContentParentDiv').empty();
    $('#lblRADTransportNameParentId').html(currentTransportUIInfo[0].UniquePropertyDescription)

 //   currentTransportUIInfo[0]

    for (var propertyInfo in sortedPropertyInfo) {
        if (sortedPropertyInfo.hasOwnProperty(propertyInfo)) {
            var currentPropertyInfo = sortedPropertyInfo[propertyInfo];
            if (currentTransportItemDetails.hasOwnProperty(currentPropertyInfo.PropertyName)) {

                //block display matrix format
                var $currentRowParent = $("<div>", {
                    id: "RADTransport" + currentPropertyInfo.PropertyName + "ParentDiv",
                    class: "RADTransportPropertyParent RADblockMatrix_OverLoad"
                });

                var $currentRowKey = $("<div>", {
                    id: "RADTransport" + currentPropertyInfo.PropertyName + "ChildDivKey",
                    class: "RADTransportPropertyChildKey",
                    text: currentPropertyInfo.DisplayName
                });

                var $currentRowValue = RADTransportConfiguration.instance.GetControlTypeDefault(currentPropertyInfo.PropertyName,
                                      currentTransportItemDetails[currentPropertyInfo.PropertyName], currentPropertyInfo, selectedTranportType);

                var $currentRowValidation = $("<div>", {
                    id: "RADTransport" + currentPropertyInfo.PropertyName + "Validation",
                    class: "RADTransportDisplayNone",
                    title: currentPropertyInfo.DisplayName + " Can't Be Empty",
                    text: "!"
                });

                $currentRowParent.append($currentRowKey).append($currentRowValue).append($currentRowValidation);
                $('#RADTranportRightSectionContentParentDiv').append($currentRowParent);
            }
        }
    }
}

RADTransportConfiguration.prototype.OnCreateTransportDetail = function () {

    //Hide/Show Controls
    $("#RADTransportDeleteButtonParentDiv").hide();
    $("#RADTransportEditButtonParentDiv").hide();
    $("#RADCreateTransportDiv").hide();
    $("#RADTransportSaveButtonParentDiv").show();
    $("#RADTransportCancelButtonParentDiv").show();
    $('#RADTranportRightSectionContentParentDiv').empty();
    RADTransportConfiguration.instance.IsCreation = true;

    // Bind Controls
    var currentTransportUIInfo = $.grep(RADTransportConfiguration.instance.AllTransPortUIInfo, function (e)
    { return e.TransportType == RADTransportConfiguration.instance.CurrentSelectedTransportType; });

    var sortedPropertyInfo = currentTransportUIInfo[0].PropertyDetails.sort(function (a, b) {
        return a.Order - b.Order
    });

    for (var propertyInfo in sortedPropertyInfo) {
        if (sortedPropertyInfo.hasOwnProperty(propertyInfo)) {
            var currentPropertyInfo = sortedPropertyInfo[propertyInfo];

            var $currentRowParent = $("<div>", {
                id: "RADTransport" + currentPropertyInfo.PropertyName + "ParentDiv",
                class: "RADTransportPropertyParent RADblockMatrix_OverLoad"
            });

            var $currentRowKey = $("<div>", {
                id: "RADTransport" + currentPropertyInfo.PropertyName + "ChildDivKey",
                class: "RADTransportPropertyChildKey",
                text: currentPropertyInfo.DisplayName
            });

            var $currentRowValue = RADTransportConfiguration.instance.GetControlTypeonCreate(currentPropertyInfo.PropertyName, currentPropertyInfo);

            var $currentRowValidation = $("<div>", {
                id: "RADTransport" + currentPropertyInfo.PropertyName + "Validation",
                class: "RADTransportDisplayNone",
                title: currentPropertyInfo.DisplayName + " Can't Be Empty",
                text: "!"
            });

            $currentRowParent.append($currentRowKey).append($currentRowValue).append($currentRowValidation);
            $('#RADTranportRightSectionContentParentDiv').append($currentRowParent);
        }
    }

    $(".RADTransportPropertyChildValue").addClass("RADTransportPropertyChildValueEditable");
    $(".RADTransportPropertyChildValuetoogleDiv").addClass("RADTransportPropertyChildValueEditable");
}

RADTransportConfiguration.prototype.OnSaveTransportDetail = function () {
    $('#RADTransportMainDiv').addClass('spinner');
    $("#RADTransportSaveButtonParentDiv").hide();
    $("#RADTransportCancelButtonParentDiv").hide();

    RADTransportConfiguration.instance.ShowHideByPrivilegeType();

    var transportDetails = RADTransportConfiguration.prototype.GetTransportDetailsbyDataValidation();

    if (!$.isEmptyObject(transportDetails)) {

        var transportDetailsString = JSON.stringify(transportDetails);

        var url = RADTransportConfig.baseUrl + "/Resources/Services/RADTransport.svc";
        var stringifiedData = null;

        if (RADTransportConfiguration.instance.IsCreation) {

            url = url + '/CreateTransport';
            stringifiedData = JSON.stringify({
                transportDetails: transportDetailsString,
                transportType: RADTransportConfiguration.instance.CurrentSelectedTransportType
            });
        }
        else {

            url = url + '/UpdateTransport';
            stringifiedData = JSON.stringify({
                transportDetails: transportDetailsString,
                transportType: RADTransportConfiguration.instance.CurrentSelectedTransportType,
                transportName: RADTransportConfiguration.instance.CurrentSelectedTransportItem
            })
        }

        $.ajax({
            url: url,
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: stringifiedData
        }).then(function (responseText) {
            //show the alert with success message

            $("#RADTransportMainDiv").append("<div id=\"RADTransportAlertParentDiv\" class=\"RADTransportAlertPopUpParent\"></div>");
            $("#RADTransportAlertParentDiv").append("<div class=\"RADTransportPopupAlertUpperSuccessDiv\"></div>");
            $("#RADTransportAlertParentDiv").append("<div class=\"RADTransportPopUpError\"></div>")
            $("#RADTransportAlertParentDiv").append("<div class=\"RADTransportDivLeft RADTransportSuccess\"></div>"); //revisit
            $("#RADTransportAlertParentDiv").append("<div class=\"RADTransportErrorDivRight\"></div>")

            if (responseText.d) {
                $(".RADTransportErrorDivRight").append("<div class=\"RADTransportSuccessDivHeading\">Success</div>")
                if (RADTransportConfiguration.instance.IsCreation)
                    $(".RADTransportErrorDivRight").append("<div class=\"RADTransportErrorDivText\">Transport has been created successfully.</div>");
                else
                    $(".RADTransportErrorDivRight").append("<div class=\"RADTransportErrorDivText\">Transport has been updated successfully.</div>");


            }
            else {
                $(".RADTransportErrorDivRight").append("<div class=\"RADTransportSuccessDivHeading\">Alert</div>")
                if (RADTransportConfiguration.instance.IsCreation)
                    $(".RADTransportErrorDivRight").append("<div class=\"RADTransportErrorDivText\">Transport creation failed.</div>");
                else
                    $(".RADTransportErrorDivRight").append("<div class=\"RADTransportErrorDivText\">Transport updation failed.</div>");

            }


            $("#RADTransportAlertParentDiv").css("top", "-200px");
            $("#RADTransportAlertParentDiv").animate({ "top": "0px" });
            setTimeout(function () {
                $("#RADTransportAlertParentDiv").remove()
                if (RADTransportConfiguration.instance.IsCreation)
                    RADTransportConfiguration.instance.OnPageLoad(RADTransportConfiguration.instance.CurrentSelectedTransportType, null);
                else {
                    RADTransportConfiguration.instance.BindCurrentTransportdetails(RADTransportConfiguration.instance.CurrentSelectedTransportType,
                        transportDetails);

                }
            }, 2000);

            $('#RADTransportMainDiv').removeClass('spinner');
        });
    }
    else {

        $('#RADTransportMainDiv').removeClass('spinner');
        $("#RADTransportSaveButtonParentDiv").show();
        $("#RADTransportCancelButtonParentDiv").show();
        $("#RADTransportEditButtonParentDiv").hide();
        $("#RADTransportDeleteButtonParentDiv").hide();

    }
}

RADTransportConfiguration.prototype.GetTransportDetailsbyDataValidation = function () {
    var self = this;
    var transportInputControls = $('.RADTransportPropertyChildValueEditable');
    var transportDetails = {};
    var currentElement = null;
    var currentPropertyName = null;
    var currentPropertyValue = null;
    var currentElementControl = null;
    var isTransportDetailsAvailable = true;
    //var skipUserNameValidation = false;

    for (var i = 0; i < transportInputControls.length; i++) {
        //skipUserNameValidation = false;
        currentElementControl = $('.RADTransportPropertyChildValueEditable')[i];

        //checking if the current control has child elements, for example, input type like password, checkbox can be in parent div 
        if ($(currentElementControl).children() != null && $(currentElementControl).children().length > 0) {
            if ($(currentElementControl).attr("toogleButton") != null) {
                currentPropertyValue = $(currentElementControl).find(".RADSQLAuthSelected").html().trim();
                //skipUserNameValidation = true;
            }

                //case for bool type using new toggle button            
            else if ($(currentElementControl).find(".RADAutoswitch").find("input") != null && $(currentElementControl).find(".RADAutoswitch").find("input").length > 0) {
                //currentPropertyValue = $(currentElementControl).find(".RADAutoswitch").find("input").val() == "on" ? "true" : "false";
                currentPropertyValue = $(currentElementControl).find(".RADAutoswitch").find("input").prop("checked");
            }

            else {
                currentElement = $(currentElementControl).children();
                currentPropertyValue = $(currentElement).val();
            }
        }
        else {
            currentElement = currentElementControl;
            currentPropertyValue = $(currentElement).html();
        }

        var currentPropertyName = $(currentElementControl).attr('radtransportpropertyname');
        var validationControlId = "#RADTransport" + currentPropertyName +"Validation";

        if ((currentPropertyValue == null || currentPropertyValue == "") && $(currentElementControl).attr('skipValidation') == null) {

            if (isTransportDetailsAvailable)
                isTransportDetailsAvailable = false;
            $("" + validationControlId + "").switchClass("RADTransportDisplayNone", "RADTransportRequiredField");
        }
        else {
            $("" + validationControlId + "").switchClass("RADTransportRequiredField", "RADTransportDisplayNone")
            transportDetails[currentPropertyName] = currentPropertyValue;
        }
    }
    //Checking if any controls are present with check box
    if (isTransportDetailsAvailable) {
        var transportCheckControls = $('.RADTransportPropertyChildValueCheckDiv');
        for (var i = 0; i < transportCheckControls.length; i++) {
            currentElementControl = $('.RADTransportPropertyChildValueCheckDiv')[i];
            currentElement = $(currentElementControl).children();
            currentPropertyName = $(currentElementControl).attr('radtransportpropertyname');
            currentPropertyValue = $(currentElement)[0].checked;
            transportDetails[currentPropertyName] = currentPropertyValue;
        }
    }
    else {
        transportDetails = {};
    }
    return transportDetails;
}

RADTransportConfiguration.prototype.GetControlTypeDefault = function (currentPropertyName, currentpropertyValue, currentPropertyInfo, selectedTranportType) {
    var self = this;
    var $currentRowValue = null;
    switch (currentPropertyInfo.Type) {

        case "Numeric":
            $currentRowValue = $("<div>", {
                id: "RADTransport" + currentPropertyInfo.PropertyName + "ChildDivValue",
                class: "RADTransportPropertyChildValue",
                text: currentpropertyValue
            });
            break;

        case "Password":
            $currentRowValue = $("<div>", {
                id: "RADTransport" + currentPropertyInfo.PropertyName + "ChildDivValue",
                class: "RADTransportPropertyChildValue"
            });

            var $passwordTypeInput = $("<input>", {
                id: "RADTransport" + currentPropertyInfo.PropertyName + "ChildDivValueInput",
                type: "password",
                value: currentpropertyValue,
                class: "RADTransportPropertyChildValueInput"
            });

            $passwordTypeInput.css({ "pointer-events": "none" });
            $currentRowValue.append($passwordTypeInput);
            break;

        case "Bool":
            //$currentRowValue = $("<div>", {
            //    id: "RADTransport" + currentPropertyInfo.PropertyName + "ChildDivValue",
            //    class: "RADTransportPropertyChildValueCheckDiv"
            //});

            //var $checkTypeInput = $("<input>", {
            //    id: "RADTransport" + currentPropertyInfo.PropertyName + "ChildDivValueCheckBox",
            //    type: "checkbox",
            //    checked: currentpropertyValue,
            //    class: "RADTransportPropertyChildValueCheckBox"
            //});
            //$checkTypeInput.css({ "pointer-events": "none" });
            //$currentRowValue.append($checkTypeInput);

            $currentRowValue = $("<div>", {
                id: "RADTransport" + currentPropertyInfo.PropertyName + "ChildDivValue",
                class: "RADTransportPropertyChildValuetoogleDiv RADDisableToggle",
                skipValidation: false
            })

            //Appending the on/off toggle button for bool type element
            $currentRowValue.append("<label class=\"RADAutoswitch\"><input type=\"checkbox\"> <span class=\"RADAutoslider round\" style=\"width:53px;\"></span></label>");
            if (currentPropertyInfo != null && currentPropertyInfo != "") {
                if (currentpropertyValue.toString().toLowerCase() == "true") {
                    $currentRowValue.find("input").prop('checked', true);
                }
                else {
                    $currentRowValue.find("input").prop('checked', false);
                }
            }

            break;
        case "authenticationtoggle":
            $currentRowValue = $("<div>", {
                id: "RADTransport" + currentPropertyInfo.PropertyName + "ChildDivValue",
                class: "RADTransportPropertyChildValuetoogleDiv",
                toogleButton: true
            })
            var sqlAuth = $("<div>", {
                class: "RADTransportPropertyChildValueSQLtoogleDiv",
                text: 'SQL Server'
            })

            var windowAuth = $("<div>", {
                class: "RADTransportPropertyChildValueQindowtoogleDiv",
                text: 'Windows'
            })
            if (currentpropertyValue == "SQL Server")
                sqlAuth.addClass("RADSQLAuthSelected");
            else
                windowAuth.addClass("RADSQLAuthSelected");
            $currentRowValue.append(sqlAuth);
            $currentRowValue.append(windowAuth);
            break;
        default:
            $currentRowValue = $("<div>", {
                id: "RADTransport" + currentPropertyInfo.PropertyName + "ChildDivValue",
                class: "RADTransportPropertyChildValue",
                text: currentpropertyValue
            });
    }

    $currentRowValue.attr("radtransportpropertyname", currentPropertyInfo.PropertyName);

    if (currentPropertyInfo.Type.toLowerCase() != "bool") {
        $(".RADTransportPropertyChildValuetoogleDiv").unbind("click").click(function (e) {
            if (!$(e.target).hasClass("RADSQLAuthSelected")) {
                $(e.target).parent().find(".RADSQLAuthSelected").removeClass("RADSQLAuthSelected");
                $(e.target).addClass("RADSQLAuthSelected");

                self.SQLTransportToggle(e);
            }
        });
    }
    return $currentRowValue;
}

RADTransportConfiguration.prototype.SQLTransportToggle = function (e) {
    if ($(e.target).html() == "Windows") {
        $(e.target).parent().attr('skipValidation', 'true');
        $(e.target).closest(".RADTransportPropertyParent").next().find(".RADTransportPropertyChildValue").html("");
        $(e.target).closest(".RADTransportPropertyParent").next().find(".RADTransportPropertyChildValue").addClass("RADDisableClass").attr('skipValidation', 'true');
        $(e.target).closest(".RADTransportPropertyParent").next().next().find(".RADTransportPropertyChildValue")
                                                                        .find(".RADTransportPasswordChildDivValueInput").val("");
        $(e.target).closest(".RADTransportPropertyParent").next().next().find(".RADTransportPropertyChildValue").addClass("RADDisableClass").attr('skipValidation', 'true');
    }
    else {
        $(e.target).parent().removeAttr('skipValidation');
        $(e.target).closest(".RADTransportPropertyParent").next().find(".RADTransportPropertyChildValue").removeClass("RADDisableClass").removeAttr('skipValidation');
        $(e.target).closest(".RADTransportPropertyParent").next().next().find(".RADTransportPropertyChildValue").removeClass("RADDisableClass").removeAttr('skipValidation');
    }
}

RADTransportConfiguration.prototype.GetControlTypeonCreate = function (currentPropertyName, currentPropertyInfo) {
    var self = this;
    var $currentRowValue = null;
    switch (currentPropertyInfo.Type) {

        case "Numeric":
            $currentRowValue = $("<div>", {
                id: "RADTransport" + currentPropertyInfo.PropertyName + "ChildDivValue",
                class: "RADTransportPropertyChildValue",
                text: "",
                tabindex: 1,
                onkeypress : "return event.charCode >= 48 && event.charCode <= 57",
                contenteditable: true
            });
            break;

        case "Password":
            $currentRowValue = $("<div>", {
                id: "RADTransport" + currentPropertyInfo.PropertyName + "ChildDivValue",
                class: "RADTransportPropertyChildValue"
            });

            var $passwordTypeInput = $("<input>", {
                id: "RADTransport" + currentPropertyInfo.PropertyName + "ChildDivValueInput",
                type: "password",
                class: "RADTransportPropertyChildValueInput",
                tabindex: 1
            });
            $currentRowValue.append($passwordTypeInput);
            break;

        case "Bool":
            //$currentRowValue = $("<div>", {
            //    id: "RADTransport" + currentPropertyInfo.PropertyName + "ChildDivValue",
            //    class: "RADTransportPropertyChildValueCheckDiv"
            //})

            //var $checkTypeInput = $("<input>", {
            //    id: "RADTransport" + currentPropertyInfo.PropertyName + "ChildDivValueCheckBox",
            //    type: "checkbox",
            //    class: "RADTransportPropertyChildValueCheckBox",
            //    tabindex: 1
            //});
            //$currentRowValue.append($checkTypeInput);
            $currentRowValue = $("<div>", {
                id: "RADTransport" + currentPropertyInfo.PropertyName + "ChildDivValue",

                class: "RADTransportPropertyChildValuetoogleDiv RADEnableToggle",
                skipValidation: false
            })

            //Appending the on/off toggle button for bool type element
            $currentRowValue.append("<label class=\"RADAutoswitch\"><input type=\"checkbox\"> <span class=\"RADAutoslider round\" style=\"width:53px;\"></span></label>");            
            break;
        case "authenticationtoggle":
            $currentRowValue = $("<div>", {
                id: "RADTransport" + currentPropertyInfo.PropertyName + "ChildDivValue",
                class: "RADTransportPropertyChildValuetoogleDiv",
                toogleButton:true
            })
            var sqlAuth = $("<div>", {
                class: "RADTransportPropertyChildValueSQLtoogleDiv RADSQLAuthSelected",
                text:'SQL Server'
            })
            var windowAuth = $("<div>", {
                class: "RADTransportPropertyChildValueQindowtoogleDiv",
                text: 'Windows'
            })
            $currentRowValue.append(sqlAuth);
            $currentRowValue.append(windowAuth);
            break;
        default:
            $currentRowValue = $("<div>", {
                id: "RADTransport" + currentPropertyInfo.PropertyName + "ChildDivValue",
                class: "RADTransportPropertyChildValue",
                text: "",
                contenteditable: true,
                tabindex: 1
            });
    }

    $currentRowValue.attr("radtransportpropertyname", currentPropertyInfo.PropertyName);

    if (currentPropertyInfo.Type.toLowerCase() != "bool") {
        $(".RADTransportPropertyChildValuetoogleDiv").unbind("click").click(function (e) {
            if (!$(e.target).hasClass("RADSQLAuthSelected")) {
                $(e.target).parent().find(".RADSQLAuthSelected").removeClass("RADSQLAuthSelected");
                $(e.target).addClass("RADSQLAuthSelected");

                self.SQLTransportToggle(e);
            }
        });
    }
    return $currentRowValue;

}

RADTransportConfiguration.prototype.CheckNumeric = function (event) {

    if (event.which >= 48 && event.which <= 57) {
        console.log("true");
        return true;
    }
    else {
        console.log("false");
        return false;
    }
}


RADTransportConfiguration.prototype.OnCancelTransportDetail = function () {
    $("#RADTransportSaveButtonParentDiv").hide();
    $("#RADTransportCancelButtonParentDiv").hide();

    RADTransportConfiguration.instance.ShowHideByPrivilegeType();
    RADTransportConfiguration.instance.BindCurrentTransportdetails(RADTransportConfiguration.instance.CurrentSelectedTransportType,
                                                                    RADTransportConfiguration.instance.CurrentTransPortDetails);
}

RADTransportConfiguration.prototype.OnEditTransportDetail = function () {

    $("#RADTransportDeleteButtonParentDiv").hide();
    $("#RADTransportEditButtonParentDiv").hide();
    $("#RADCreateTransportDiv").hide();
    $("#RADTransportSaveButtonParentDiv").show();
    $("#RADTransportCancelButtonParentDiv").show();
    RADTransportConfiguration.instance.IsCreation = false;

    $(".RADTransportPropertyChildValue").attr('contenteditable', 'true');
    $(".RADTransportPropertyChildValue").addClass("RADTransportPropertyChildValueEditable");
    $(".RADTransportPropertyChildValuetoogleDiv").addClass("RADTransportPropertyChildValueEditable");
    $(".RADTransportPropertyChildValueInput").css({ "pointer-events": "all" });
    $(".RADTransportPropertyChildValueCheckBox").css({ "pointer-events": "all" });

    //Enable Disable On/Off button
    if ($(".RADTransportPropertyChildValuetoogleDiv ").hasClass("RADDisableToggle")) {
        $(".RADTransportPropertyChildValuetoogleDiv ").removeClass("RADDisableToggle");
        $(".RADTransportPropertyChildValuetoogleDiv ").addClass("RADEnableToggle");
    }

    //Add event for numeric types

    var currentTransportUIInfo = $.grep(RADTransportConfiguration.instance.AllTransPortUIInfo, function (e)
    { return e.TransportType == RADTransportConfiguration.instance.CurrentSelectedTransportType; });

    var numericPropertyTypes = $.grep(currentTransportUIInfo[0].PropertyDetails, function (e)
    { return e.Type == "Numeric" });

    for (var numericProperty in numericPropertyTypes) {
        if (numericPropertyTypes.hasOwnProperty(numericProperty)) {
            var currentNumericProperty = numericPropertyTypes[numericProperty];
            var curremtPropertyName = currentNumericProperty.PropertyName;
            var currentNumericElement = $("div[radtransportpropertyname='" + curremtPropertyName + "']");

            currentNumericElement.unbind().on('keypress', function (e) {
                return event.charCode >= 48 && event.charCode <= 57
            });
        }
    }

    var nonEditableProperty = $("div[radtransportpropertyname='" + currentTransportUIInfo[0].NonEditableColumn + "']");
    nonEditableProperty.attr('contenteditable', 'false');
}

RADTransportConfiguration.prototype.OnDeleteTransportDetail = function () {
    $("#RADTransportAlertParentDiv").remove();
    $("#RADTransportMainDiv").append("<div id=\"RADTransportAlertParentDiv\" class=\"RADTransportAlertPopUpParent\"></div>");
    $("#RADTransportAlertParentDiv").append("<div class=\"RADTransportAlertUpperDiv\"></div>");
    $("#RADTransportAlertParentDiv").append("<div class=\"RADTransportPopUpError\"></div>")
    $("#RADTransportAlertParentDiv").append("<div class=\"RADTransportDivLeft RADTransportAlert\"></div>"); //revisit
    $("#RADTransportAlertParentDiv").append("<div class=\"RADTransportErrorDivRight\"></div>")
    $(".RADTransportErrorDivRight").append("<div class=\"RADTransportErrorDivHeading\">Alert</div>")
    $(".RADTransportErrorDivRight").append("<div class=\"RADTransportErrorDivText\">Are You Sure You want to delete "
        + (RADTransportConfiguration.instance.CurrentSelectedTransportItem).toUpperCase() + "?</div>");
    $("#RADTransportAlertParentDiv").append("<div class=\"RADTransportPopUpErrorFooter\"></div>");
    $(".RADTransportPopUpErrorFooter").append("<div class=\"RADTransportPopUpErrorYes\">Yes</div><div class=\"RADTransportPopUpErrorNo\">No</div>")
    $("#RADTransportAlertParentDiv").css("top", "-200px");
    $("#RADTransportAlertParentDiv").animate({ "top": "0px" });
    $(".RADTransportPopUpErrorYes").unbind().click(function (event) {
        $("#RADTransportAlertParentDiv").remove();
        RADTransportConfiguration.prototype.DeleteTransportDetail();
    });
    $(".RADTransportPopUpErrorNo").unbind().click(function (event) {
        $("#RADTransportAlertParentDiv").remove();
    });
}

RADTransportConfiguration.prototype.DeleteTransportDetail = function () {

    $('#RADTransportMainDiv').addClass('spinner');
    var url = RADTransportConfig.baseUrl + "/Resources/Services/RADTransport.svc";
    
    $.ajax({
        url: url + '/DeleteTransport',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({
            transportName: RADTransportConfiguration.instance.CurrentSelectedTransportItem,
            transportType: RADTransportConfiguration.instance.CurrentSelectedTransportType
        })
    }).then(function (responseText) {
        //show the alert with success message
        // call the pageload again
        $("#RADTransportMainDiv").append("<div id=\"RADTransportAlertParentDiv\" class=\"RADTransportAlertPopUpParent\"></div>");
        $("#RADTransportAlertParentDiv").append("<div class=\"RADTransportPopupAlertUpperSuccessDiv\"></div>");
        $("#RADTransportAlertParentDiv").append("<div class=\"RADTransportPopUpError\"></div>")
        $("#RADTransportAlertParentDiv").append("<div class=\"RADTransportDivLeft RADTransportSuccess\"></div>"); //revisit
        $("#RADTransportAlertParentDiv").append("<div class=\"RADTransportErrorDivRight\"></div>");

        if (responseText.d) {
            $(".RADTransportErrorDivRight").append("<div class=\"RADTransportSuccessDivHeading\">Success</div>");
            $(".RADTransportErrorDivRight").append("<div class=\"RADTransportErrorDivText\">Transport has been deleted successfully.</div>");
        }
        else {
            $(".RADTransportErrorDivRight").append("<div class=\"RADTransportSuccessDivHeading\">Alert</div>");
            $(".RADTransportErrorDivRight").append("<div class=\"RADTransportErrorDivText\">Transport deletion failed.</div>");;
        }

        $("#RADTransportAlertParentDiv").css("top", "-200px");
        $("#RADTransportAlertParentDiv").animate({ "top": "0px" });
        setTimeout(function () {
            $("#RADTransportAlertParentDiv").remove()
            RADTransportConfiguration.instance.OnPageLoad(RADTransportConfiguration.instance.CurrentSelectedTransportType, null);
        }, 2000)
        $('#RADTransportMainDiv').removeClass('spinner');
    });
}


RADTransportConfiguration.prototype.GetAuthorizationPrivileges = function (selectedTranportType) {
    
    $('#RADTransportMainDiv').addClass('spinner');
    var url = RADTransportConfig.baseUrl + "/Resources/Services/RADTransport.svc";
    $.ajax({
        url: url + '/GetAuthorizationPrivileges',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'

    }).then(function (responseText) {
        $('#RADTransportMainDiv').removeClass('spinner');
        if (responseText.d == "admin") {
            $("#RADTransportDeleteButtonParentDiv").show();
            $("#RADTransportEditButtonParentDiv").show();
            $("#RADCreateTransportDiv").show();
            RADTransportConfiguration.instance.AuthorizationPrivileges.push('Add New Transport Configuration ',
                                                                            'Add New Transport Configuration',
                                                                            'Delete Transport Configuration',
                                                                            'Update Transport Configuration');
        }
        else {
            var authPrivileges = [];
            if (responseText.d.length > 0)
                authPrivileges = JSON.parse(responseText.d);
            RADTransportConfiguration.instance.AuthorizationPrivileges = []

            for (var i = 0; i < authPrivileges.length; i++) {
                if (authPrivileges[i].pageId == "RAD_RWUCLeftMenu_lnkTransportManager") {
                    for (var j = 0; j < authPrivileges[i].Privileges.length; j++)
                        RADTransportConfiguration.instance.AuthorizationPrivileges.push(authPrivileges[i].Privileges[j])
                }
            }
        }
        RADTransportConfiguration.instance.ShowHideByPrivilegeType();
        RADTransportConfiguration.instance.GetTransportDetailsbyType(selectedTranportType);
        $('#RADTransportMainDiv').removeClass('spinner');
    });
}

RADTransportConfiguration.prototype.ShowHideByPrivilegeType = function () {    
    if (RADTransportConfiguration.instance.AuthorizationPrivileges.indexOf("Add New Transport Configuration ") != -1 ||
        RADTransportConfiguration.instance.AuthorizationPrivileges.indexOf("Add New Transport Configuration") != -1) {
        $("#RADCreateTransportDiv").show();
    }
    else {
        $("#RADCreateTransportDiv").hide();
    }
    if (RADTransportConfiguration.instance.AuthorizationPrivileges.indexOf("Delete Transport Configuration") != -1) {
        $("#RADTransportDeleteButtonParentDiv").show();
    }
    else {
        $("#RADTransportDeleteButtonParentDiv").hide();
    }
    if (RADTransportConfiguration.instance.AuthorizationPrivileges.indexOf("Update Transport Configuration") != -1) {
        $("#RADTransportEditButtonParentDiv").show();
    }
    else {
        $("#RADTransportEditButtonParentDiv").hide();
    }
}


RADTransportConfiguration.prototype.RegisterOnOffBtnClickHandler = function (e) {
    $(".RADAutoslider").unbind("click").click(function (e) {
        if ($(e.target.parentElement).find("input").is(":checked"))
        {
            $(e.target.parentElement).find("input").prop("checked", false);
        }
        else
        {
            $(e.target.parentElement).find("input").prop("checked", true);
        }
        e.preventDefault();
        e.stopPropagation();
    });
}