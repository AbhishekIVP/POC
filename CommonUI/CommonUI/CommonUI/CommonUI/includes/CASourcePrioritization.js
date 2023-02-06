var caSourcePrioritization = (function () {

    function CASourcePrioritization() {
        this._controlIdInfo = null;
        this._securityInfo = null;
        this._pageViewModelInstance = null;

        // VendorLists
        this._vendorList = [];
        this._vendorListWithSpecialVendors = [];

        this._corpActionTypeID = 0;
    }

    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    var caSourcePrioritization = new CASourcePrioritization();


    /////////////////
    // VIEW MODELS // 
    /////////////////

    // Page View Model 
    function pageViewModel(data) {
        var self = this;

        self.tabListing = ko.observableArray();

        if (typeof (data) != 'undefined') {

            if (data.length > 0) {
                for (var item in data) {
                    {
                        self.tabListing.push(new tabViewModel(data[item]));
                    }
                }

                //$(".CASP_Chain").css("display", "block");
            }
        }

    };

    //Tab View Model
    function tabViewModel(data) {
        var self = this;

        self.tabName = data.TabName;
        self.tabID = data.TabID;

        //Is Tab Selected
        self.tabIsSelected = ko.observable(false);

        //Selected Tab Name Class Name
        self.selectedTabNameClassName = ko.computed(function () {
            return self.tabIsSelected() ? "SelectedTabNameCSSClass" : "";
        });

        //tabPriority
        self.tabPriority = data.TabPriority;

        //Tab Attributes Details
        self.tabAttributesDetails = ko.observableArray();

        for (var item in data.TabAttributesDetails) {
            self.tabAttributesDetails.push(new attributeViewModel(data.TabAttributesDetails[item]));
        }

        //Tab Column List
        self.columnNameList = ko.observableArray();

        for (var item in data.TabColumnNames) {
            self.columnNameList.push({ "columnName": data.TabColumnNames[item] });
        }

        //Tab Select All Text List
        self.selectAllList = ko.observableArray();

        for (var item in data.TabColumnNames) {
            //if (parseInt(item) > 0) {
                self.selectAllList.push(new selectAllViewModel({ "selectAllText": "--Select One--", "priority": parseInt(item) + 1, "tabID": self.tabID }));
            //}
        }
        
        //Enter Tab Details ONLY in tabList - This is to make tabs on top.
        //caSourcePrioritization._pageViewModelInstance.tabsList.push({ "tabName": dataTabName, "tabID": data.TabID });
    }

    //SelectAll View Model
    function selectAllViewModel(data) {
        var self = this;

        self.selectAllText = ko.observable(data.selectAllText);
        self.priority = data.priority;
        self.tabID = data.tabID;

    }

    //Attribute View Model
    function attributeViewModel(data) {
        var self = this;

        self.attributeID = data.AttributeID;

        self.attributeDisplayName = data.AttributeDisplayName;

        self.attributeVendorList = ko.observableArray();

        self.isSpecialVendorSet = ko.observable(false);

        if (data.AttributeVendorList) {
            for (var item in data.AttributeVendorList) {
                self.attributeVendorList.push(new attributeVendorsViewModel(data.AttributeVendorList[item], parseInt(item) + 1));   // +1 because item will be zero-indexed.
            }
        }
    };

    //Attribute Vendors View Model
    function attributeVendorsViewModel(data, priorityNumber) {
        var self = this;

        self.vendorDetails = ko.observable(data.VendorID + "@$@" + data.VendorTypeID);
        self.vendorName = ko.observable(data.VendorName);
        self.vendorPriorityNumber = priorityNumber;
    };

    //////////////////////////////////
    // Call Common Service Fuctions //
    //////////////////////////////////
    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }

    /////////////////////////////////////////////////
    // Callback Functions for COMMON SERVICE Calls //
    /////////////////////////////////////////////////

    // This function is called on Success Service Call on Load 
    function OnSuccess_GetDataForCorpActionType(result) {
        if (typeof result.d != 'undefined') {
            if (typeof result.d.CorpActionTypeName != 'undefined') {
                $("#CASP_CorpActionTypeName").text("[Corporate Action Type: " + result.d.CorpActionTypeName + "]");
            }

            if (typeof result.d.VendorData != 'undefined') {
                OnSuccess_GetAllVendors(result.d.VendorData);

                if (typeof result.d.TabData != 'undefined') {
                    OnSuccess_GetTabData(result.d.TabData, result.d.VendorData.length - 4);  // -4 for Select One, Vendor Mismatch, Value Changed & Show as Exception.
                }
            }
        }
    }

    //This Function is called from OnSuccess_GetDataForCorpActionType. It populates the vendorList for SMSelects on UI.
    function OnSuccess_GetAllVendors(result) {
        if (typeof result != 'undefined') {
            for (var item in result) {
                caSourcePrioritization._vendorListWithSpecialVendors.push({ "text": result[item].VendorName, "value": result[item].VendorID + "@$@" + result[item].VendorTypeID });

                if (result[item].VendorID > 0 || result[item].VendorID == -9) {
                    caSourcePrioritization._vendorList.push({ "text": result[item].VendorName, "value": result[item].VendorID + "@$@" + result[item].VendorTypeID });
                }
            }
        }

        ApplySRFPMInputObjectControl();
    }

    //This Function is called from OnSuccess_GetDataForCorpActionType. It massages the Tab data from Server and pass to Knockout.
    function OnSuccess_GetTabData(tabData, vendorsCount) {

        if (typeof tabData != 'undefined') {

            //Make TabColumnNames and Make TabColumnNames & AttributeVendorList of specific length.
            for (var item in tabData) {
                var isTabMultiInfo = tabData[item].TabPriority < 0 ? true : false;

                if (!isTabMultiInfo) {
                    //////////////////////////////
                    //Master or Single Info Tabs//
                    //////////////////////////////

                    //Tab Attribute Details
                    if (typeof tabData[item].TabAttributesDetails != 'undefined') {

                        for (var item1 in tabData[item].TabAttributesDetails) {
                            
                            if (typeof tabData[item].TabAttributesDetails[item1].AttributeVendorList != 'undefined') {

                                while (tabData[item].TabAttributesDetails[item1].AttributeVendorList.length < vendorsCount) {
                                    var tempObj = {};
                                    tempObj.VendorName = "--Select One--";
                                    tempObj.VendorID = -9;
                                    tempObj.Vendor_Type_ID = 2;

                                    tabData[item].TabAttributesDetails[item1].AttributeVendorList.push(tempObj);
                                }//End of while loop
                            }
                        }//End of for loop
                    }

                    //Tab Column Names
                    tabData[item].TabColumnNames = [];
                    for (var i = 0; i <= vendorsCount; i++) {
                        if (i > 0) {
                            tabData[item].TabColumnNames.push("Priority " + i + " Vendor");
                        }
                    }

                }
                else {
                    //////////////////
                    //MultiInfo Tabs//
                    //////////////////

                    //Tab Attribute Details
                    if (typeof tabData[item].TabAttributesDetails != 'undefined') {

                        for (var item1 in tabData[item].TabAttributesDetails) {

                            if (typeof tabData[item].TabAttributesDetails[item1].AttributeVendorList != 'undefined') {

                                while (tabData[item].TabAttributesDetails[item1].AttributeVendorList.length < 1) {
                                    var tempObj = {};
                                    tempObj.VendorName = "--Select One--";
                                    tempObj.VendorID = -9;
                                    tempObj.Vendor_Type_ID = 2;

                                    tabData[item].TabAttributesDetails[item1].AttributeVendorList.push(tempObj);
                                }//End of while loop
                            }
                        }//End of for loop
                    }

                    //Tab Column Names
                    tabData[item].TabColumnNames = [];
                    tabData[item].TabColumnNames.push("Attribute");
                    tabData[item].TabColumnNames.push("Priority 1 Vendor");
                }
            }//End of for loop
            
            //Apply Knockout
            if (typeof ko !== 'undefined') {

                caSourcePrioritization._pageViewModelInstance = new pageViewModel(tabData);

                ko.applyBindings(caSourcePrioritization._pageViewModelInstance);

                // First Tab is selected on load. Set tabIsSelected to true.
                caSourcePrioritization._pageViewModelInstance.tabListing()[0].tabIsSelected(true);

            }
        }
    }

    //This function is called if any Service Call returns an error - CHANGED FINAL
    function OnFailure(result) {
        $("#" + caSourcePrioritization._controlIdInfo.LblErrorId).text("Service returned an error" + ((result.responseJSON == null || result.responseJSON.Message == null || result.responseJSON.Message.length == 0) ? "" : (" : " + result.responseJSON.Message)) + ((result.responseText == null || result.responseText.length == 0) ? "" : (" : " + result.responseText)));
        $find(caSourcePrioritization._controlIdInfo.ModalErrorId).show();
    }
    

    //////////////////////////////////////
    // Initiate SRFPMInputObjectControl //
    //////////////////////////////////////
    function ApplySRFPMInputObjectControl() {
        var data1 = {};
        data1.listOfItems = caSourcePrioritization._vendorListWithSpecialVendors;
        
        var obj = {};

        obj.id = "CASP_VendorPriority";
        obj.container = "#CASP_SRFPMInputObjectControl1";
        obj.type = "SS";
        obj.callbackFunctionDelegate = callbackVendorPriority;
        obj.data = data1; 
        
        SRFPMInputObjectControl.init(obj);

    }

    ////////////////////////////////////////////////////
    // Callback Functions for SRFPMInputObjectControl //
    ////////////////////////////////////////////////////

    //Callback Function for Vendor Value Change (Attribute & Select All --> Both)
    function callbackVendorPriority(result) {
        var obj = {};

        if (typeof result.obj != 'undefined') {
            obj = result.obj;

            if (typeof result.obj.tabID == 'undefined') {
                //Attribute Vendor Value Change

                if (typeof result.data != 'undefined' && result.data[0].value.split("@$@")[0] != -9) {
                    obj.vendorName(result.data[0].text);
                    obj.vendorDetails(result.data[0].value);
                }
            }
            else {
                //Select All Value Change
                
                if (typeof result.data != 'undefined' && result.data[0].value.split("@$@")[0] != -9) {
                    //Change Text and Details of SelectAll SMSelect.
                    obj.selectAllText(result.data[0].text);
                    //obj.vendorDetails = result.data[0].value;

                    //Select All Priority Index
                    var selectAllPriorityIndex = obj.priority - 1;

                    //Change Text and Details for all Attributes in that tab in that priority.
                    for (var item in caSourcePrioritization._pageViewModelInstance.tabListing()) {
                        if (caSourcePrioritization._pageViewModelInstance.tabListing()[item].tabID == obj.tabID) {

                            for (var item1 in caSourcePrioritization._pageViewModelInstance.tabListing()[item].tabAttributesDetails()) {

                                caSourcePrioritization._pageViewModelInstance.tabListing()[item].tabAttributesDetails()[item1].attributeVendorList()[selectAllPriorityIndex].vendorName(result.data[0].text);

                                caSourcePrioritization._pageViewModelInstance.tabListing()[item].tabAttributesDetails()[item1].attributeVendorList()[selectAllPriorityIndex].vendorDetails(result.data[0].value);
                            }

                            break;
                        }
                    }

                }
            }
        }
    }

    ////Callback Function for Select All Value Change
    //function callbackSelectAllVendorPriority(result) {
    //    var obj = {};
    //    if (typeof result.obj != 'undefined') {
    //        obj = result.obj;
    //        if (typeof result.data != 'undefined' && result.data[0].value != -1) {
    //            //Change Text and Details of SelectAll SMSelect.
    //            obj.vendorName(result.data[0].text);
    //            //obj.vendorDetails = result.data[0].value;
    //            //Select All Priority Index
    //            var selectAllPriorityIndex = obj.priority - 1;
    //            //Change Text and Details for all Attributes in that tab in that priority.
    //            for (var item in caSourcePrioritization._pageViewModelInstance.tabListing()) {
    //                if (caSourcePrioritization._pageViewModelInstance.tabListing()[item].tabID == obj.tabID) {
    //                    for (var item1 in caSourcePrioritization._pageViewModelInstance.tabListing()[item].tabAttributesDetails) {
    //                        caSourcePrioritization._pageViewModelInstance.tabListing()[item].tabAttributesDetails()[item1].attributeVendorList()[selectAllPriorityIndex].vendorName(result.data[0].text);
    //                        caSourcePrioritization._pageViewModelInstance.tabListing()[item].tabAttributesDetails()[item1].attributeVendorList()[selectAllPriorityIndex].vendorDetails(result.data[0].value);
    //                    }
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //}


    ///////////////////////////
    // Event Based Functions //
    ///////////////////////////

    //Function called when Tab is changed.
    CASourcePrioritization.prototype.onClickTab = function onClickTab(obj, event) {

        //Set tabIsSelected to false for all tabs.
        for (var item in caSourcePrioritization._pageViewModelInstance.tabListing()) {
            caSourcePrioritization._pageViewModelInstance.tabListing()[item].tabIsSelected(false);
        }

        //Set true for this tab.
        obj.tabIsSelected(true);
    };

    //Attribute Vendor Click
    CASourcePrioritization.prototype.onClickAttributeVendorPriority = function onClickAttributePriority(obj, event) {

        var dispObj = {};

        dispObj.inputControlID = "CASP_VendorPriority";
        dispObj.callbackFunctionDelegate = callbackVendorPriority;
        dispObj.data = {};

        //Can be removed later, if we initiate SRFPMInputObjectControl 2 times, one for with spl vendors & one without.
        dispObj.data.listOfItems = caSourcePrioritization._vendorListWithSpecialVendors;

        dispObj.data.selectedItem = [{ "text": obj.vendorName(), "value": obj.vendorDetails() }];

        SRFPMInputObjectControl.showInputObject(event, dispObj, obj);
    };

    //Select All Click for Priority
    CASourcePrioritization.prototype.onClickSelectAllPriority = function onClickAttributePriority(obj, event) {
        var dispObj = {};

        dispObj.inputControlID = "CASP_VendorPriority";
        dispObj.callbackFunctionDelegate = callbackVendorPriority;
        dispObj.data = {};

        //Can be removed later, if we initiate SRFPMInputObjectControl 2 times, one for with spl vendors & one without.
        dispObj.data.listOfItems = caSourcePrioritization._vendorList;

        //dispObj.data.selectedItem = [{ "text": obj.vendorName(), "value": obj.vendorDetails() }];

        SRFPMInputObjectControl.showInputObject(event, dispObj, obj);
    };


    ///////////////////////////////////////////////////////////////
    // init function to Load Initial Data as per Default Filters //
    ///////////////////////////////////////////////////////////////
    function init() {
        // Get data from Service Call
        var params = {};
        params.corpActionTypeID = caSourcePrioritization._corpActionTypeID;

        CallCommonServiceMethod('CASP_GetDataForCorpActionType', params, OnSuccess_GetDataForCorpActionType, OnFailure, null, false);
        //CallCommonServiceMethod('CASP_GetAllVendorsForCorpActionType', params, OnSuccess_GetAllVendors, OnFailure, null, false);
    }


    ////////////////////////////
    // "Initializer" function // - SectypeID Change Required
    ////////////////////////////
    CASourcePrioritization.prototype.Initializer = function Initializer(controlInfo, securityInfo) {

        //Set Control ID Info & Security Info
        caSourcePrioritization._controlIdInfo = eval("(" + controlInfo + ")");
        caSourcePrioritization._securityInfo = eval("(" + securityInfo + ")");

        //SectypeID - Debugging
        caSourcePrioritization._corpActionTypeID = 1;

        //Set Height of Entire Body of the Page
        $('#CASourcePrioritizationBody').height($(window).height());

        //Setting Height of Area below the Filters 
        $("#CASP_Main").height($('#CASourcePrioritizationBody').height() - $('#CASP_Top').height() - 15);

        //$(".CASP_TabGridParentDiv").height($("#CASP_Main").height() - 100);
        //$(".CASP_TabGridParentDiv").width($("#CASP_Main").width() - 50);

        init();
    }

    // WINDOW Resize Function 
    $(window).resize(function () {

        //Set Height of Entire Body of the Page
        $('#CASourcePrioritizationBody').height($(window).height());

        //Setting Height of Area below the Filters
        $("#CASP_Main").height($('#CASourcePrioritizationBody').height() - $('#CASP_Top').height() - 15);


        //Change in Each Tab
        $(".CASP_TabGridParentDiv").height($("#CASP_Main").height() - 100);
        $(".CASP_TabGridParentDiv").width($("#CASP_Main").width() - 50);
    })

    return caSourcePrioritization;
})();