var UniquenessCheckApp = {

    mainViewModel: null,
    appliedBinding: false,
    ModuleName: null,
    CallbackInstrumentClick: null,
    CallbackPopupClose: null,
    CUF_DateFormat: null,
    CUF_DateTimeFormat: null,
    init: function (data, callbackInstrumentClick, callbackPopupClose, moduleId, containerDivId, basePathForFlie, isPostBackFriendly, cufDateFormat, cufDateTimeFormat) {
        UniquenessCheckApp.ModuleName = moduleId == 3 ? "Security" : "Entity";
        UniquenessCheckApp.CallbackInstrumentClick = callbackInstrumentClick;
        UniquenessCheckApp.CallbackPopupClose = callbackPopupClose;
        UniquenessCheckApp.CUF_DateFormat = cufDateFormat;
        UniquenessCheckApp.CUF_DateTimeFormat = cufDateTimeFormat;

        if (isPostBackFriendly == null)
            isPostBackFriendly = false;

        if (!UniquenessCheckApp.appliedBinding || isPostBackFriendly) {
            //Read html file and insert in containerDivId
            var htmlFile = basePathForFlie + 'CommonUI/html/SRMUniquenessFailure.html';
            var request = $.ajax({
                url: htmlFile,
                type: "GET",
                dataType: "html"
            }).done(function (dataHTML) {
                $("#" + containerDivId).html(dataHTML);
                document.getElementById("imgDuplicateErrorId").src = basePathForFlie + "CommonUI/css/images/icons/fail_icon.png";
                
                if (moduleId == 3) {
                    $("#divErrorDuplicateInner #UniquenessFailure").text("Duplicate Security Found");
                }
                else {
                    $("#divErrorDuplicateInner #UniquenessFailure").text("Duplicate Entity Found");
                }

                UniquenessCheckApp.mainViewModel = new UniquenessCheckApp.LeftAppViewModel(data);
                ko.applyBindings(UniquenessCheckApp.mainViewModel, document.getElementById("divErrorDuplicateInner"));
                UniquenessCheckApp.appliedBinding = true;

                $('#btnOkDuplicateValues').unbind('click').click(function (e) {
                    //closeDialog('divErrorDuplicate');
                    UniquenessCheckApp.CallbackPopupClose();
                    UniquenessCheckApp.mainViewModel.UniqueKeyLst.removeAll();
                });

                //CSS For Container Div
                $("#" + containerDivId).css({ 'position': 'absolute', 'z-index': '99999', 'top': '0px', 'background-color': 'White', 'box-shadow': '5px 7px 22px -7px #6D6D6D', 'background-image': 'url("images/popup_texture.png")', 'background-repeat': 'repeat', 'min-height': '200px', 'font-family': 'Lato', 'border': '1px solid #D4D4D4', 'border-top': '4px solid #C7898C' });
                $("#" + containerDivId).css('left', ($(window).width() - $("#divErrorDuplicateInner").width()) / 2 - 2);
            });
        }
        else {
            //Remove All
            UniquenessCheckApp.mainViewModel.UniqueKeyLst.removeAll();

            for (var item1 in data) {
                UniquenessCheckApp.mainViewModel.UniqueKeyLst.push(new UniquenessCheckApp.UniqueKeyObjViewModel(data[item1]));
            }

            //CSS For Container Div
            $("#" + containerDivId).css({ 'position': 'absolute', 'z-index': '99999', 'top': '0px', 'background-color': 'White', 'box-shadow': '5px 7px 22px -7px #6D6D6D', 'background-image': 'url("images/popup_texture.png")', 'background-repeat': 'repeat', 'min-height': '200px', 'font-family': 'Lato', 'border': '1px solid #D4D4D4', 'border-top': '4px solid #C7898C' });
            $("#" + containerDivId).css('left', ($(window).width() - $("#divErrorDuplicateInner").width()) / 2 - 2);
        }        
    },

    LeftAppViewModel: function (data) {
        var self = this;
        self.selectedChainItem = null;
        self.UniqueKeyLst = ko.observableArray([]);
        
        self.searchText = ko.observable("");
        for (var item1 in data) {
            self.UniqueKeyLst.push(new UniquenessCheckApp.UniqueKeyObjViewModel(data[item1]));
        }
        self.UniqueKeyLst()[0].isSelected(true);
        self.searchResult = ko.computed(function () {
            var searchTxt = self.searchText().toLowerCase();
            var list = self.UniqueKeyLst();
            if (list.length != 0) {
                var res = ko.utils.arrayFilter(list, function (item) {
                    return item.keyName.toLowerCase().indexOf(searchTxt) >= 0
                });
                if (res.length == 0) {
                    for (let item in list) {
                        list[item].isKeyVisible(true);
                        list[item].isSelected(false);
                    }
                    list[0].isSelected(true);
                }
                else {
                    for (let item in list) {
                        list[item].isKeyVisible(false);
                        list[item].isSelected(false);
                    }
                    for (let item in res) {
                        res[item].isKeyVisible(true);
                        res[item].isSelected(false);
                    }
                    res[0].isSelected(true);
                }
            }

        })
    },
    UniqueKeyObjViewModel: function (data) {
        var self = this;

        var dataKey = typeof data.key != 'undefined' ? data.key : (typeof data.Key != 'undefined' ? data.Key : null);
        var dataValue = typeof data.value != 'undefined' ? data.value : (typeof data.Value != 'undefined' ? data.Value : null);

        self.isKeyVisible = ko.observable(true);
        self.isSelected = ko.observable(false);
        self.keyName = dataKey;
        self.legName = dataValue[0].DuplicateLegName;
        self.rightChainAttributeListing = [];
        self.rightChainDuplicateSecListing = [];
        self.selectedkeyName = ko.observable(dataKey);
        self.selectedlegName = ko.observable(dataValue[0].DuplicateLegName);
        
        self.ModuleName = UniquenessCheckApp.ModuleName;
        self.InstrumentIdHeader = UniquenessCheckApp.ModuleName + (UniquenessCheckApp.ModuleName=='Security' ? " Id" : " Code");
        self.InstrumentTypeHeader = UniquenessCheckApp.ModuleName + " Type";
        
        for (var i = 0; i < dataValue.length; i++) {
            for (var item in dataValue[i].LstSaveDuplicateAttributesInKeyId) {
                self.rightChainAttributeListing.push(new UniquenessCheckApp.RightChainAttributeViewModel(dataValue[i].LstSaveDuplicateAttributesInKeyId[item]));//, item));
            }
        }

        for (var i = 0; i < dataValue.length; i++) {
            for (var item in dataValue[i].LstDuplicateSecurityInfo) {
                self.rightChainDuplicateSecListing.push(new UniquenessCheckApp.RightChainDuplicateSecuritiesViewModel(dataValue[i].LstDuplicateSecurityInfo[item]));//, item));
            }
        }
        self.onClickChain = function (obj, event) {
            //making isselected false of all other objs
            var list = UniquenessCheckApp.mainViewModel.UniqueKeyLst();
            for (let item in list) {
                list[item].isSelected(false);
            }            
            obj.isSelected(true);
        }

    },
    RightChainAttributeViewModel: function (data) {
        var self = this;
        //var dataKey = typeof data.key != 'undefined' ? data.key : (typeof data.Key != 'undefined' ? data.Key : null);
        //var dataValue = typeof data.value != 'undefined' ? data.value : (typeof data.Value != 'undefined' ? data.Value : null);
        
        //if (dataValue.includes("<Ÿ>")) {
        //    try {
        //        dataValue = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(dataValue.split("<Ÿ>")[0]), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
        //    }
        //    catch (ex) { }
        //}
        //else if (dataValue.includes("<ž>")) {
        //    try {
        //        dataValue = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(dataValue.split("<ž>")[0]), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
        //    }
        //    catch (ex) { }
        //}

        var dataKey = typeof data.AttributeDisplayName != "undefined" ? data.AttributeDisplayName : "";
        var dataValue = typeof data.AttributeValue != "undefined" ? data.AttributeValue : "";
        
        var dataType = data.AttributeDataType;
        if (typeof dataType != "undefined" && dataType != null) {
            dataType = dataType.toLowerCase();
            if (dataType == "date" && typeof dataValue != 'undefined' && dataValue != null) {
                dataValue = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(dataValue), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
            }
            else if (dataType == "datetime" && typeof dataValue != 'undefined' && dataValue != null) {
                if (UniquenessCheckApp.ModuleName == "Entity")
                {
                    dataValue = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(dataValue), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.shorDate));
                }
                else
                {
                    dataValue = com.ivp.rad.rscriptutils.RSDateTimeUtils.convertDateTimeToString(new Date(dataValue), com.ivp.rad.rscriptutils.RSDateTimeUtils.getCultureFormat(com.ivp.rad.rscriptutils.DateTimeFormat.longDate));
                }
            }
            else if (dataType == "boolean" || dataType == "bit") {
                if (dataValue != null && (dataValue == "1" || dataValue.toLowerCase()=="true")) {
                    dataValue = "True";
                }
                else if (dataValue != null && (dataValue == "0" || dataValue.toLowerCase() == "false")) {
                    dataValue = "False";
                }
                else if (dataValue == null || dataValue == "") {
                    dataValue = "N/A";
                }
            }
        }
        self.attrName = ko.observable(dataKey);
        self.attrValue = ko.observable(dataValue);

    },
    RightChainDuplicateSecuritiesViewModel: function (data) {
        var self = this;
        var dataKey = typeof data.key != 'undefined' ? data.key : (typeof data.Key != 'undefined' ? data.Key : null);
        var dataValue = typeof data.value != 'undefined' ? data.value : (typeof data.Value != 'undefined' ? data.Value : null);

        self.secId = ko.observable(dataKey);
        self.secName = ko.observable(dataValue.split('|')[1]);
        self.secTypeName = ko.observable(dataValue.split('|')[0]);
        self.failureType = ko.observable(dataValue.split('|')[2]);
        self.onClickDuplicateSecId = function (obj, event) {
            var instrumentId = obj.secId();
            if (!instrumentId.toLowerCase().includes("current")) {
                var instrumentFailureType = obj.failureType();
                //CallBack on Instrument Click
                if (UniquenessCheckApp.ModuleName == "Security") {
                    UniquenessCheckApp.CallbackInstrumentClick(instrumentId);
                }
                else {
                    UniquenessCheckApp.CallbackInstrumentClick(instrumentId, instrumentFailureType);
                }
            }
        }
    }
}