var htmlBulkUploadGrid = (function () {
    var htmlBulkUploadGrid;
    String.prototype.replaceAll = function (find, replace) {
        return this.replace(new RegExp(find, 'g'), replace);
    }

    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');
    var leftMenuPath = ""
    if (typeof (window.parent.leftMenu) !== "undefined")
        leftMenuPath = window.parent.leftMenu;
    else if (typeof (window.parent.parent.leftMenu) !== "undefined")
        leftMenuPath = window.parent.parent.leftMenu;


    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    function HtmlBulkUploadGrid() {
        this._controlIdInfo = null;
        this._securityInfo = null;
        this._pageViewModelInstance = null;
        this._pageViewModelInstanceRules = null;

        //To Store current ModuleID
        this._currentModuleID = -1;

        //To Store if KO has been applied.
        this._koBindingApplied = false;
        this._koBindingAppliedRules = false;

        //To Search after some certain time.
        this._intervalId = 0;
    }

    htmlBulkUploadGrid = new HtmlBulkUploadGrid();


    /////////////////
    // VIEW MODELS //
    /////////////////

    // Page View Model
    function pageViewModel(data) {
        var self = this;

        self.typeListing = ko.observableArray();

        if (typeof (data) != 'undefined') {
            ////Uncomment This
            ////No Keys for Current Filters
            //if (typeof (window.parent.leftMenu) !== "undefined") {
            if (typeof data.List == 'undefined' || data.List.length === 0) {
                //NO FEED MESSAGE
                var noTypeMessage = getNoTypeMessage();
                $("#" + htmlBulkUploadGrid._controlIdInfo.LblErrorId).text(noTypeMessage);
                $find(htmlBulkUploadGrid._controlIdInfo.ModalErrorId).show();

                $("#RC_RightMainDiv").hide();
                ////Uncomment This
                //$("#UniquenessSetupMain").hide();
                //window.parent.leftMenu.showNoRecordsMsg("No keys found matching your search criteria.", $("#UniquenessSetupMainErrorDiv"));

                ////CSS for smNoRecordsContainer div created inside UniquenessSetupMainErrorDiv div
                //var pd_left = ($("#UniquenessSetupMainErrorDiv").width() - $("#smNoRecordsContainer").width() - 2) / 2;
                //var pd_top = (($("#UniquenessSetupMainErrorDiv").height() - $("#smNoRecordsContainer").height() - 2) / 2) - 20;
                //$("#UniquenessSetupMainErrorDiv #smNoRecordsContainer").css({ "padding-left": pd_left, "padding-top": pd_top });
            }
            else {
                ////Uncomment This
                //$("#UniquenessSetupMain").show();
                //window.parent.leftMenu.hideNoRecordsMsg();

                $("#RC_RightMainDiv").show();

                for (var item in data.List) {
                    self.typeListing.push(new typeViewModel(data.List[item]));
                }
            }
            //}
        }
    };

    // Type View Model
    function typeViewModel(data) {
        var self = this;

        self.typeName = data.Type;
        self.typeID = data.TypeId;
        self.typeIsSelected = ko.observable(false);


        self.parentType = data.ParentType;
        self.parentTypetoShow = true;
        if (self.parentType == null) {
            self.parentTypetoShow = false;
        }

        //To Show
        self.typeToShow = ko.observable(true);

        //Background Color
        self.typeBackgroundColor = ko.computed(function () {
            return self.typeIsSelected() ? '#566b86' : '#d9e5f4';
        });

        //Text Color
        self.typeTextColor = ko.computed(function () {
            return self.typeIsSelected() ? '#ffffff' : '#3e4d5e';
        });

        self.ruleTypeListing = ko.observableArray();

        for (var item in data.RuleTypes) {
            self.ruleTypeListing.push(new ruleTypeViewModel(data.RuleTypes[item], this));
        }
    }

    // Rule Type View Model
    function ruleTypeViewModel(data, ParentObject) {
        var self = this;

        self.ruleTypeName = data;
        self.parentObject = ParentObject;
        self.ruleTypeIsSelected = ko.observable(false);

        //Background Color
        self.ruleTypeBackgroundColor = ko.computed(function () {
            return self.ruleTypeIsSelected() ? '#495f77' : '#566b86';
        });
    }


    ////RightPane View Models
    function rulesPageViewModel(data) {
        var self = this;

        self.rules = ko.observableArray();

        if (typeof (data.rules) != 'undefined') {

            if (data.rules.length === 0) {
                toShowEmptyRuleSet(false);
            }
            else {
                for (var item in data.rules) {
                    self.rules.push(new rulesViewModel(data.rules[item]));
                }
            }
        }
    }

    //Rules View Model
    function rulesViewModel(data) {
        var self = this;

        self.attrName = data.attrName;
        self.basketName = data.basketName;
        self.typeNameToShow = data.typeName;

        self.ruletypes = ko.observableArray();


        for (var item in data.ruletypes) {
            self.ruletypes.push(new rightRuleTypeViewModel(data.ruletypes[item]));
        }


        self.section = data.section;
        self.dependent = data.dependent;
        self.depending = data.depending;


        self.dependentToShow = ko.computed(function () {
            if (self.dependent != null) {
                var numOfDependent = self.dependent.split(',').length;
                var dependentVariable = "";
                if (numOfDependent <= 2) {
                    dependentVariable = self.dependent;
                }
                else
                    dependentVariable = self.dependent.split(',')[0] + ' , ' + self.dependent.split(',')[1];

                if (dependentVariable.length > 40)
                    return dependentVariable.substring(0, 40) + '...';
                else
                    return dependentVariable;
            }
        });

        self.dependingToShow = ko.computed(function () {
            if (self.depending != null) {
                var numOfDepending = self.depending.split(',').length;
                var dependingVariable = "";

                if (numOfDepending <= 2) {
                    dependingVariable = self.depending;
                }
                else
                    dependingVariable = self.depending.split(',')[0] + ' , ' + self.depending.split(',')[1];

                if (dependingVariable.length > 40)
                    return dependingVariable.substring(0, 40) + '...';
                else
                    return dependingVariable;
            }

        });

        self.dependentToShowMore = ko.computed(function () {
            if (self.dependent != null) {
                var numOfDependent = self.dependent.split(',').length;
                if (numOfDependent > 2) {
                    return ' +' + (numOfDependent - 2);
                }
            }
            else
                return;
        });


        self.dependingToShowMore = ko.computed(function () {
            if (self.depending != null) {
                var numOfDepending = self.depending.split(',').length;
                if (numOfDepending > 2) {
                    return ' +' + (numOfDepending - 2);
                }
            }
            else
                return;
        });


        //To Show
        self.ruletoShow = ko.observable(true);

        //self.attributeLevelToShow = ko.computed(function () {
        //    var section = self.section;
        //    var ruleTypeName = self.ruletypes.ruleTypeName;
        //    var flag = false;

        //    if (section.toLowerCase().indexOf('modeler') != -1) {
        //        if ((ruleTypeName.toLowerCase().indexOf('mnemonic') != -1) || (ruleTypeName.toLowerCase().indexOf('calculated') != -1) || (ruleTypeName.toLowerCase().indexOf('validation') != -1) || (ruleTypeName.toLowerCase().indexOf('alert') != -1)) {
        //            flag = true;
        //        }
        //    }
        //    else if (section.toLowerCase().indexOf('upstream') != -1) {
        //        if (ruleTypeName.toLowerCase().indexOf('transformation') != -1) {
        //            flag = true;
        //        }
        //    }
        //    else if (section.toLowerCase().indexOf('downstream') != -1) {
        //        if (ruleTypeName.toLowerCase().indexOf('transformation') != -1) {
        //            flag = true;
        //        }
        //    }
        //    return flag;
        //});

        self.dependingDependentToShow = ko.computed(function () {

            if (self.section.toLowerCase() == "modeler") {
                if (self.attrName == null) {
                    return false;
                }
                return true;
            }
            else
                return true;
        });

        self.attributeToDisplay = ko.computed(function () {
            if (self.attrName != null && self.attrName != "")
                return self.attrName;
            else if (self.basketName != null && self.basketName != "")
                return self.basketName;
            else
                return;
        });

    }

    //Right Rule Type View Model
    function rightRuleTypeViewModel(data) {
        var self = this;

        self.ruleTypeName = data.ruleTypeName;
        self.attrRules = ko.observableArray();

        for (var item in data.attrRules) {
            self.attrRules.push(new RuleViewModel(data.attrRules[item]));
        }
    }


    //Rule View Model
    function RuleViewModel(data) {
        var self = this;

        self.ruleGroupName = data.ruleGroupName;
        self.ruleName = data.ruleName;
        self.priority = data.priority;
        self.ruleText = data.ruleText;
        self.ruleState = data.ruleState == "True" ? true : false;
        self.isCommonRule = data.isCommonRule;
        self.commonRuleSectypes = data.commonRuleSectypes;

        self.isCommonRuleCount = ko.computed(function () {
            if (self.commonRuleSectypes != null) {
                var numOfcommonRuleSectypes = self.commonRuleSectypes.split(',').length;
                return numOfcommonRuleSectypes;
            }
            else
                return;
        });

        self.specificRuleToShow = ko.observable(true);
    }

    //Creates No Type Message Based on Module and Section Selected. Also For Right Filter Data.
    function getNoTypeMessage(rightFilter) {
        var result = "";
        var currentModuleId = htmlBulkUploadGrid._currentModuleID;

        var targetSMSelect = $("#smselect_sectionValues");
        var sectionId;
        if (typeof rightFilter != 'undefined' && rightFilter) {
            sectionId = 2;
        }
        else {
            sectionId = smselect.getSelectedOption(targetSMSelect)[0].value;
        }

        if (sectionId == 0) {   //Upstream
            result = "No Feeds are Configured";
        }
        else if (sectionId == 1) { //Downstream
            result = "No Reports are Configured";
        }
        else if (sectionId == 2) {  //Modeler
            if (currentModuleId == 3) //Sec
                result = "No Security Type is Configured.";
            else if (currentModuleId == 6) //Ref
                result = "No Reference Type is Configured.";
            else if (currentModuleId == 9) //Corp
            {
                if (typeof rightFilter != 'undefined' && rightFilter) {
                    result = "No Corporate Action Type is Configured.";
                }
                else {
                    result = "No Rules are Configured.";
                }
            }
            else if (currentModuleId == 20) //Party
                result = "No Party Type is Configured.";
            else if (currentModuleId == 18) //Fund
                result = "No Fund Type is Configured.";
        }

        return result;
    }

    HtmlBulkUploadGrid.prototype.initialiseTabs = function initialiseTabs() {
        var listofTabsToBindFunctionsWith = CommonModuleTabs.ModuleList();

        for (i in listofTabsToBindFunctionsWith) {
            item = listofTabsToBindFunctionsWith[i];
            switch (item.displayName.toLowerCase().trim()) {
                case "securities":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initiateSec });
                    break;

                case "refdata":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initiateRef });
                    break;

                case "corpaction":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initiateCorp });
                    break;
                case "parties":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initiateParty });
                    break;

                case "funds":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: initiateFunds });
                    break;

            }
        }
    }

    function initiateSec(urlString) {

        htmlBulkUploadGrid._currentModuleID = 3;

        var params = {};
        params.ModuleID = htmlBulkUploadGrid._currentModuleID;
        params.UserName = htmlBulkUploadGrid._securityInfo.UserName;

        CallCommonServiceMethod('GetRuleCatalogFilterDataForModuleID', params, OnSuccess_GetModuleBasedTypesData, OnFailure, null, false);

        //var params = {};
        //params.ModuleID = htmlBulkUploadGrid._currentModuleID;
        //params.TabID = 0;
        //params.UserName = htmlBulkUploadGrid._securityInfo.UserName;
        //CallCommonServiceMethod('GetRulesForModuleIDAndTabID', params, OnSuccess_GetSectionData, OnFailure, null, false);
        ////Set First Option in Section Multi-Select
        //setFirstOptionInSectionMultiSelect();

        ////Commented By Dhruv
        //var pathSecmaster = path + '/../CommonUI/SRMRuleCatalog.aspx?Module=SECURITY_MASTER&&SubModule=UPSTREAM' + '&id=' + $('#hdnId').val() + '&moduleName=' + $('#hdnModuleName').val();
        //SRMIframeResize();
        //$('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathSecmaster);
    }

    function initiateRef(urlString) {

        htmlBulkUploadGrid._currentModuleID = 6;

        var params = {};
        params.ModuleID = htmlBulkUploadGrid._currentModuleID;
        params.UserName = htmlBulkUploadGrid._securityInfo.UserName;

        CallCommonServiceMethod('GetRuleCatalogFilterDataForModuleID', params, OnSuccess_GetModuleBasedTypesData, OnFailure, null, false);

        //var params = {};
        //params.ModuleID = htmlBulkUploadGrid._currentModuleID;
        //params.TabID = 0;
        //params.UserName = htmlBulkUploadGrid._securityInfo.UserName;
        //CallCommonServiceMethod('GetRulesForModuleIDAndTabID', params, OnSuccess_GetSectionData, OnFailure, null, false);
        ////Set First Option in Section Multi-Select
        //setFirstOptionInSectionMultiSelect();

        ////Commented By Dhruv
        //var pathRefMaster = path + '/../CommonUI/SRMRuleCatalog.aspx?Module=REFERENCE_MASTER&&SubModule=UPSTREAM' + '&id=' + $('#hdnId').val() + '&moduleName=' + $('#hdnModuleName').val();
        //SRMIframeResize();
        //$('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathRefMaster);
    }

    function initiateCorp(urlString) {

        htmlBulkUploadGrid._currentModuleID = 9;

        var params = {};
        params.ModuleID = htmlBulkUploadGrid._currentModuleID;
        params.UserName = htmlBulkUploadGrid._securityInfo.UserName;

        CallCommonServiceMethod('GetRuleCatalogFilterDataForModuleID', params, OnSuccess_GetModuleBasedTypesData, OnFailure, null, false);

        //var params = {};
        //params.ModuleID = htmlBulkUploadGrid._currentModuleID;
        //params.TabID = 0;
        //params.UserName = htmlBulkUploadGrid._securityInfo.UserName;
        //CallCommonServiceMethod('GetRulesForModuleIDAndTabID', params, OnSuccess_GetSectionData, OnFailure, null, false);
        ////Set First Option in Section Multi-Select
        //setFirstOptionInSectionMultiSelect();

        ////Commented By Dhruv
        //var pathRefMaster = path + '/../CommonUI/SRMRuleCatalog.aspx?Module=CORPORATE_ACTIONS&&SubModule=UPSTREAM' + '&id=' + $('#hdnId').val() + '&moduleName=' + $('#hdnModuleName').val();
        //SRMIframeResize();
        //$('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathRefMaster);
    }

    function initiateParty(urlString) {

        htmlBulkUploadGrid._currentModuleID = 20;

        var params = {};
        params.ModuleID = htmlBulkUploadGrid._currentModuleID;
        params.UserName = htmlBulkUploadGrid._securityInfo.UserName;

        CallCommonServiceMethod('GetRuleCatalogFilterDataForModuleID', params, OnSuccess_GetModuleBasedTypesData, OnFailure, null, false);

        //var params = {};
        //params.ModuleID = htmlBulkUploadGrid._currentModuleID;
        //params.TabID = 0;
        //params.UserName = htmlBulkUploadGrid._securityInfo.UserName;
        //CallCommonServiceMethod('GetRulesForModuleIDAndTabID', params, OnSuccess_GetSectionData, OnFailure, null, false);
        ////Set First Option in Section Multi-Select
        //setFirstOptionInSectionMultiSelect();

        ////Commented By Dhruv
        //var pathRefMaster = path + '/../CommonUI/SRMRuleCatalog.aspx?Module=PARTY_MASTER&&SubModule=UPSTREAM' + '&id=' + $('#hdnId').val() + '&moduleName=' + $('#hdnModuleName').val();
        //SRMIframeResize();
        //$('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathRefMaster);
    }

    function initiateFunds(urlString) {
        htmlBulkUploadGrid._currentModuleID = 18;

        var params = {};
        params.ModuleID = htmlBulkUploadGrid._currentModuleID;
        params.UserName = htmlBulkUploadGrid._securityInfo.UserName;

        CallCommonServiceMethod('GetRuleCatalogFilterDataForModuleID', params, OnSuccess_GetModuleBasedTypesData, OnFailure, null, false);

        //var params = {};
        //params.ModuleID = htmlBulkUploadGrid._currentModuleID;
        //params.TabID = 0;
        //params.UserName = htmlBulkUploadGrid._securityInfo.UserName;
        //CallCommonServiceMethod('GetRulesForModuleIDAndTabID', params, OnSuccess_GetSectionData, OnFailure, null, false);
        ////Set First Option in Section Multi-Select
        //setFirstOptionInSectionMultiSelect();

        ////Commented By Dhruv
        //var pathRefMaster = path + '/../CommonUI/SRMRuleCatalog.aspx?Module=FUND_MASTER&&SubModule=UPSTREAM' + '&id=' + $('#hdnId').val() + '&moduleName=' + $('#hdnModuleName').val();
        //SRMIframeResize();
        //$('.InnerFrame[name=TabsInnerIframe1]').attr('src', pathRefMaster);
    }

    //Set First Option in Section Multi-Select (Used on click of Sec/Ref/Corp/etc)
    function setFirstOptionInSectionMultiSelect(preventTrigger) {

        if (typeof preventTrigger == 'undefined') {
            preventTrigger = true;
        }

        var targetSMSelect = $("#smselect_sectionValues");
        smselect.setOptionByIndex(targetSMSelect, 0, preventTrigger);

        //Clear The Search text    
        $("#RC_LeftPaneSearchInputBox").val("");
    }

    ////Commented By Dhruv
    //function SRMIframeResize(tabId) {
    //    //var iframeHeight, width;
    //    //iframeHeight = $(window).height() - iframeSubtractHeight[tabId];
    //    var width = $(window).width();
    //    var height = $(window).height() - 40;
    //    $("#container1").css('height', height);
    //    $("#InnerFrameId1").height(height).outerWidth(width);
    //}


    //////////////////////////////////
    // Call Common Service Fuctions //
    //////////////////////////////////
    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }


    /////////////////////////////////////////////////
    // Callback Functions for COMMON SERVICE Calls //
    /////////////////////////////////////////////////

    //Gets Types Data for Each Module
    function OnSuccess_GetModuleBasedTypesData(result) {
        if (typeof result.d != 'undefined' && result.d.length > 0) {
            var filterData = [];
            var filterList = [];
            var selectedList = '';

            for (var item in result.d) {
                var tempObj = {};
                tempObj.text = result.d[item].Value;
                tempObj.value = result.d[item].Key;

                filterList.push(tempObj);
                if (parseInt(item) == 0) {
                    selectedList += tempObj.value;
                }
                else {
                    selectedList += ',' + tempObj.value;
                }
            }

            if (htmlBulkUploadGrid._currentModuleID == 3) {
                filterData.push(processFilters(filterList, 'SecurityCB', 'Security Types', selectedList, 'value', 'text', 'checkbox', 'value', true, false, "Select All", "down"));
            } else if (htmlBulkUploadGrid._currentModuleID == 6) {
                filterData.push(processFilters(filterList, 'EntityCB', 'Entity Types', selectedList, 'value', 'text', 'checkbox', 'value', true, false, "Select All", "down"));
            } else if (htmlBulkUploadGrid._currentModuleID == 9) {
                filterData.push(processFilters(filterList, 'CorpCB', 'Corporate Action Types', selectedList, 'value', 'text', 'checkbox', 'value', true, false, "Select All", "down"));
            } else if (htmlBulkUploadGrid._currentModuleID == 18) {
                filterData.push(processFilters(filterList, 'FundCB', 'Fund Types', selectedList, 'value', 'text', 'checkbox', 'value', true, false, "Select All", "down"));
            } else if (htmlBulkUploadGrid._currentModuleID == 20) {
                filterData.push(processFilters(filterList, 'RoleCB', 'Roles', selectedList, 'value', 'text', 'checkbox', 'value', true, false, "Select All", "down"));
            }

            bindSlideMenu(filterData);

            //Apply CSS to Right Filter
            //SRMRuleCatalog_ApplyDynamicCSS_RightFilter();

            var target = $("#smselect_sectionValues");

            var types = [];
            types = getRightFilterTypesList();

            var params = {};
            params.ModuleID = htmlBulkUploadGrid._currentModuleID;
            //params.TabID = parseInt(smselect.getSelectedOption(target)[0].value);
            params.TabID = 2;
            params.UserName = htmlBulkUploadGrid._securityInfo.UserName;
            params.TypeIDs = types;

            CallCommonServiceMethod('GetRulesForModuleIDAndTabID', params, OnSuccess_GetSectionData, OnFailure, null, false);

            //Set First Option in Section Multi-Select
            setFirstOptionInSectionMultiSelect();

            $("#SRFPMRuleCatalogueParentDiv").show();
            if (leftMenuPath != "")
                leftMenuPath.hideNoRecordsMsg();
            $("#SRFPMRuleCatalogueCommonErrorMsgDiv").hide();

            //Enable Click on Right Filter Icon
            $("#SRMRuleCatalogFilterSection").css('pointer-events', 'auto');
        }
        else {
            if (htmlBulkUploadGrid._pageViewModelInstance != null && typeof htmlBulkUploadGrid._pageViewModelInstance.typeListing != 'undefined') {

                //Remove All Items from list on Left
                //htmlBulkUploadGrid._pageViewModelInstance.typeListing.removeAll();
                //Hide Right Div
                //$("#RC_RightMainDiv").hide();

                //Disable Click on Right Filter Icon
                $("#SRMRuleCatalogFilterSection").css('pointer-events', 'none');
            }

            //NO Types Message
            var noTypeMessage = getNoTypeMessage(true);
            //$("#" + htmlBulkUploadGrid._controlIdInfo.LblErrorId).text(noTypeMessage);
            //$find(htmlBulkUploadGrid._controlIdInfo.ModalErrorId).show();

            //Common Error Msg
            //if (typeof (window.parent.leftMenu) !== "undefined") {
            $("#SRFPMRuleCatalogueParentDiv").hide();
            $("#SRFPMRuleCatalogueCommonErrorMsgDiv").show();
            if (leftMenuPath != "")
                leftMenuPath.showNoRecordsMsg(noTypeMessage, $("#SRFPMRuleCatalogueCommonErrorMsgDiv"));

            //CSS for smNoRecordsContainer div created inside UniquenessSetupMainErrorDiv div
            //var pd_left = ($("#SRFPMRuleCatalogueCommonErrorMsgDiv").width() - $("#smNoRecordsContainer").width() -2) / 2;
            //var pd_top = (($("#SRFPMRuleCatalogueCommonErrorMsgDiv").height() - $("#smNoRecordsContainer").height() - 2) / 2) - 20;
            //$("#SRFPMRuleCatalogueCommonErrorMsgDiv #smNoRecordsContainer").css({ "padding-left": pd_left, "padding-top": pd_top
            //});
            //}
        }
    }

    //Gets Data for Each Section for a specific module
    function OnSuccess_GetSectionData(result) {
        if (typeof result.d != 'undefined') {

            var internalObj = JSON.parse(result.d);

            if (!htmlBulkUploadGrid._koBindingApplied) {
                //Called on Load

                if (typeof ko !== 'undefined') {
                    htmlBulkUploadGrid._pageViewModelInstance = new pageViewModel(internalObj);

                    ko.applyBindings(htmlBulkUploadGrid._pageViewModelInstance, $("#RC_SectionData")[0]);

                    if (typeof internalObj.List != 'undefined' && internalObj.List.length > 0) {
                        //Trigger Click on First Type
                        $("#RC_SubModulesParentDiv .RC_TypeName")[0].click();
                    }
                }

                htmlBulkUploadGrid._koBindingApplied = true;
            }
            else {
                //Called when on Change of SMSelect fires a Service Call

                htmlBulkUploadGrid._pageViewModelInstance.typeListing.removeAll();

                if (typeof internalObj.List == 'undefined' || internalObj.List.length == 0) {
                    //NO FEED MESSAGE
                    var noTypeMessage = getNoTypeMessage();
                    $("#" + htmlBulkUploadGrid._controlIdInfo.LblErrorId).text(noTypeMessage);
                    $find(htmlBulkUploadGrid._controlIdInfo.ModalErrorId).show();

                    $("#RC_RightMainDiv").hide();
                }
                else {
                    $("#RC_RightMainDiv").show();

                    var data = internalObj.List;

                    for (var item in data) {
                        htmlBulkUploadGrid._pageViewModelInstance.typeListing.push(new typeViewModel(data[item]));
                    }

                    //Trigger Click on First Type
                    if ($("#RC_SubModulesParentDiv .RC_TypeName").length > 0) {
                        $("#RC_SubModulesParentDiv .RC_TypeName")[0].click();
                    }
                }
            }
        }
    }

    function renderRuleCatalog(viewModelObj) {
        onServiceUpdated();

        var data = null;

        data = viewModelObj.d;
        data = data.replaceNBR();
        data = data.replace(/(\r\n|\n|\r)/gm, ' ').replace(/\t/g, ' ');
        data = JSON.parse(data);

        if (data.rules.length > 0 && typeof data.rules[0].sectypes != "undefined" && typeof data.rules[0].section != "undefined") {
            if (data.rules[0].section.toLowerCase() == 'upstream' || data.rules[0].section.toLowerCase() == 'downstream') {
                $('#SMRuleSetup_Mapped_Types').attr('sectypes', data.rules[0].sectypes);
                $('#SMRuleSetup_Mapped_Types').show();
            }
            else {
                $('#SMRuleSetup_Mapped_Types').attr('sectypes', '');
                $('#SMRuleSetup_Mapped_Types').hide();
            }
        }

        //if (typeof isFiltered != "undefined" && isFiltered != true)
        //    srmRuleCatalog.rulescollection = data.rules;

        if (typeof data != "undefined" && data.rules.length == 0) {
            toShowEmptyRuleSet(false);
        }
        else if (typeof data != "undefined" && data.rules.length > 0) {
            tohideEmptyRuleSet();
            isCollapsible(data);
            if (!htmlBulkUploadGrid._koBindingAppliedRules) {
                if (typeof ko !== 'undefined') {
                    htmlBulkUploadGrid._pageViewModelInstanceRules = new rulesPageViewModel(data);

                    ko.applyBindings(htmlBulkUploadGrid._pageViewModelInstanceRules, $("#SMAttributeRuleContainer")[0]);
                }

                htmlBulkUploadGrid._koBindingAppliedRules = true;


                //for (var item in data.rules) {
                //    srmRuleCatalog.viewModelObjToRenderParent.rules.push(data.rules[item]);
                //}

                ////srmRuleCatalog.viewModelObj = ko.mapping.fromJS(data);
                //ko.applyBindings(srmRuleCatalog.viewModelObjToRenderParent, document.getElementById('SMAttributeRuleContainer'));
                //srmRuleCatalog.isApplyBinding = true;
            }
            else {
                htmlBulkUploadGrid._pageViewModelInstanceRules.rules.removeAll();

                for (var item in data.rules) {
                    htmlBulkUploadGrid._pageViewModelInstanceRules.rules.push(new rulesViewModel(data.rules[item]));
                }



                //srmRuleCatalog.viewModelObjToRenderParent.rules.removeAll();

                //for (var item in data.rules) {
                //    srmRuleCatalog.viewModelObjToRenderParent.rules.push(data.rules[item]);
                //}

            }
        }
    }

    //This function is called if any Service Call returns an error
    function OnFailure(result) {
        $("#" + htmlBulkUploadGrid._controlIdInfo.LblErrorId).text("Service returned an error" + ((result.responseJSON == null || result.responseJSON.Message == null || result.responseJSON.Message.length == 0) ? "" : (" : " + result.responseJSON.Message)) + ((result.responseText == null || result.responseText.length == 0) ? "" : (" : " + result.responseText)));
        $find(htmlBulkUploadGrid._controlIdInfo.ModalErrorId).show();
    }

    ////////////////////
    // Event Handlers //
    ////////////////////

    //Function called when a Type Name is clicked
    HtmlBulkUploadGrid.prototype.onClickType = function onClickType(obj, event) {
        var target = event.currentTarget;

        //Set IsSelected to false for all Types
        for (var item in htmlBulkUploadGrid._pageViewModelInstance.typeListing()) {
            htmlBulkUploadGrid._pageViewModelInstance.typeListing()[item].typeIsSelected(false);
        }

        obj.typeIsSelected(true);

        //Trigger Click of First Rule Type
        $(target).closest('.RC_TypeRow').find(".RC_RuleTypeName")[0].click();

    }

    //Function called when a Rule Type Name is clicked
    HtmlBulkUploadGrid.prototype.onClickRuleType = function onClickRuleType(obj, event) {
        //Set IsSelected to false for all Rule Types
        for (var item1 in htmlBulkUploadGrid._pageViewModelInstance.typeListing()) {
            for (var item2 in htmlBulkUploadGrid._pageViewModelInstance.typeListing()[item1].ruleTypeListing()) {
                htmlBulkUploadGrid._pageViewModelInstance.typeListing()[item1].ruleTypeListing()[item2].ruleTypeIsSelected(false);
            }
        }

        obj.ruleTypeIsSelected(true);

        //Call Common Service Method to fetch Data and give to Lakshya's Method
        var targetSMSelect = $("#smselect_sectionValues");

        var params = {};
        params.module = htmlBulkUploadGrid._currentModuleID;
        params.tab = parseInt(smselect.getSelectedOption(targetSMSelect)[0].value);
        params.ruleType = obj.ruleTypeName;
        params.typeId = obj.parentObject.typeID;
        params.userName = htmlBulkUploadGrid._securityInfo.UserName;

        onServiceUpdating();
        CallCommonServiceMethod('GetRules', params, renderRuleCatalog, OnFailure, null, false);
    }

    //Function to Handle Search in Left Pane
    $("#RC_LeftPaneSearchParentDiv #RC_LeftPaneSearchInputBox").on('keyup', function () {
        var searchText = "";
        searchText = $("#RC_LeftPaneSearchParentDiv #RC_LeftPaneSearchInputBox").val();

        if (htmlBulkUploadGrid._intervalId !== 0) {
            clearTimeout(htmlBulkUploadGrid._intervalId);
        }
        htmlBulkUploadGrid._intervalId = setTimeout(function () {
            if (searchText == $("#RC_LeftPaneSearchParentDiv #RC_LeftPaneSearchInputBox").val()) {

                if (htmlBulkUploadGrid._pageViewModelInstance.typeListing().length > 0) {
                    for (var item in htmlBulkUploadGrid._pageViewModelInstance.typeListing()) {

                        var typeObj = htmlBulkUploadGrid._pageViewModelInstance.typeListing()[item];

                        if (typeObj.typeName.toLowerCase().indexOf(searchText.toLowerCase()) != -1) {
                            typeObj.typeToShow(true);
                        }
                        else {
                            typeObj.typeToShow(false);
                        }

                    }

                    //Show Data For First Visible Type
                    //htmlBulkUploadGrid._pageViewModelInstance.typeListing()[0];
                    var allRows = $("#RC_SubModulesParentDiv .RC_TypeRow");
                    var firstVisibleRow = null;

                    var len = allRows.length;
                    for (var item = 0; item < len; item++) {
                        if (allRows[item].style.display != "none") {
                            firstVisibleRow = allRows[item];
                            break;
                        }
                    }

                    //Click on First
                    if (firstVisibleRow != null) {
                        $(".RC_TypeName", firstVisibleRow)[0].click();
                    }

                }
            }
        }, 100);
    });


    //////////////////////////////////////////
    // EVENT HANDLERS FOR RIGHT SIDE DIV    //
    //////////////////////////////////////////

    //Function to Handle Search in Right Pane
    $("#SMRuleSetup_SearchOnRules").on('keyup', function () {
        var sText = "";
        var flag = false;
        var attrFlag = false;
        sText = $("#SMRuleSetup_SearchOnRules").val().toLowerCase().replace(/\s+/g, '');
        var count = 0;
        var actualRulesCount = 0;
        var hiddenRulesCount = 0;

        if (htmlBulkUploadGrid._pageViewModelInstanceRules.rules().length > 0) {
            for (var item in htmlBulkUploadGrid._pageViewModelInstanceRules.rules()) {

                var ruleObj = htmlBulkUploadGrid._pageViewModelInstanceRules.rules()[item];
                var section = ruleObj.section;
                var ruleTypeName = "";

                if (ruleObj.attrName != null && ruleObj.attrName != "" && ruleObj.attrName.toLowerCase().replace(/\s+/g, '').indexOf(sText) !== -1) {
                    flag = true;
                    ruleObj.ruletoShow(true);
                    //continue;
                }
                if (ruleObj.basketName != null && ruleObj.basketName != "" && ruleObj.basketName.toLowerCase().replace(/\s+/g, '').indexOf(sText) !== -1) {
                    flag = true;
                    ruleObj.ruletoShow(true);
                    // continue;
                }
                if (ruleObj.dependent != null && ruleObj.dependent != "" && ruleObj.dependent.toLowerCase().replace(/\s+/g, '').indexOf(sText) !== -1) {
                    flag = true;
                    ruleObj.ruletoShow(true);
                    // continue;
                }
                if (ruleObj.depending != null && ruleObj.depending != "" && ruleObj.depending.toLowerCase().replace(/\s+/g, '').indexOf(sText) !== -1) {
                    flag = true;
                    ruleObj.ruletoShow(true);
                    // continue;
                }
                if (ruleObj.ruletypes().length > 0) {
                    //flag = false;
                    ruleObj.ruletypes().forEach(function (rulesiterator, i) {
                        ruleTypeName = rulesiterator.ruleTypeName;
                        if (rulesiterator.ruleTypeName != null && rulesiterator.ruleTypeName != "" && rulesiterator.ruleTypeName.toLowerCase().replace(/\s+/g, '').indexOf(sText) !== -1) {
                            flag = true;
                        }
                        if (section.toLowerCase().indexOf('modeler') != -1) {
                            if ((ruleTypeName.toLowerCase().indexOf('mnemonic') != -1) || (ruleTypeName.toLowerCase().indexOf('calculated') != -1) || (ruleTypeName.toLowerCase().indexOf('validation') != -1) || (ruleTypeName.toLowerCase().indexOf('alert') != -1) || (ruleTypeName.toLowerCase().indexOf('basket') != -1)) {
                                attrFlag = true;
                            }
                        }
                        else if (section.toLowerCase().indexOf('upstream') != -1) {
                            if (ruleTypeName.toLowerCase().indexOf('transformation') != -1) {
                                attrFlag = true;
                            }
                        }
                        else if (section.toLowerCase().indexOf('downstream') != -1) {
                            if (ruleTypeName.toLowerCase().indexOf('transformation') != -1) {
                                attrFlag = true;
                            }
                        }
                        rulesiterator.attrRules().forEach(function (child, i) {
                            if (!attrFlag)
                                actualRulesCount++;
                            if (child.ruleGroupName != null && child.ruleGroupName != "" && child.ruleGroupName.toLowerCase().replace(/\s+/g, '').indexOf(sText) !== -1) {
                                flag = true;
                                child.specificRuleToShow(true);
                            }
                            else if (child.ruleName != null && child.ruleName != "" && child.ruleName.toLowerCase().replace(/\s+/g, '').indexOf(sText) !== -1) {
                                flag = true;
                                child.specificRuleToShow(true);
                            }
                            else if (child.ruleText != null && child.ruleText != "" && child.ruleText.toLowerCase().replace(/\s+/g, '').indexOf(sText) !== -1) {
                                flag = true;
                                child.specificRuleToShow(true);
                            }
                            else {
                                if (!attrFlag) {
                                    child.specificRuleToShow(false);
                                    hiddenRulesCount++;
                                }
                            }
                        });
                    });
                    if (!flag) {
                        ruleObj.ruletoShow(false);
                        count++;
                    }
                    else
                        ruleObj.ruletoShow(true);
                }
                flag = false;
            }
            if (count == htmlBulkUploadGrid._pageViewModelInstanceRules.rules().length) {
                toShowEmptyRuleSet(true);
            }
            else {
                tohideEmptyRuleSet(ruleTypeName);
            }
            if (!attrFlag && hiddenRulesCount > 0) {
                $('.SMRuleSetup_rulecount span').html(actualRulesCount - hiddenRulesCount);
            }
            else if (!attrFlag && hiddenRulesCount == 0) {
                $('.SMRuleSetup_rulecount span').html(actualRulesCount);
            }
        }
    });

    $('#SMRuleSetup_Mapped_Types').on('click', function (e) {
        var sectypes = $('#SMRuleSetup_Mapped_Types').attr('sectypes');
        if (sectypes != null && sectypes != "") {
            onclickDataRender(sectypes.split(','), e, "SMRuleSetup_Mapped_Types");
        }

    });

    //Collapsible Click
    $(document).click(function (e) {
        var $header = $(e.target);
        if ($header.attr('class') == "SMRuleSetup_attrname") {
            $header = $header.closest(".SMRuleSetup_arrow");
            //getting the next element

            var $parent = $header.parent();
            var $content = $parent.find(".SMRuleSetup_hidden_contents");

            if ($parent.find(".fa-caret-right").length > 0) {
                $parent.find(".fa-caret-right").removeClass("fa-caret-right").addClass("fa-caret-down");
            }
            else
                $parent.find(".fa-caret-down").removeClass("fa-caret-down").addClass("fa-caret-right");
            //open up the content needed - toggle the slide- if visible, slide up, if not slidedown.

            if ($parent.attr("isCollapsed") == 0) {
                $parent.attr("isCollapsed", 1);
                $parent.addClass('SMRuleSetup_NotSelectedRule').removeClass('SMRuleSetup_SelectedRule');
            }
            else {
                $parent.attr("isCollapsed", 0);
                $parent.addClass('SMRuleSetup_SelectedRule').removeClass('SMRuleSetup_NotSelectedRule');
            }

            $content.slideToggle();
        }
        else if ($header.attr('class') == "SMRuleSetup_Depending_More") {
            $header = $header.closest(".SMRuleSetup_Depending_Dependent_RuleContainer");
            var depending = $header.attr('depending');
            if (depending != null && depending.split(',').length > 2)
                onclickDataRender(depending.split(','), e, 'SMRuleSetup_Depending_More');
        }
        else if ($header.attr('class') == "SMRuleSetup_Dependent_More") {
            $header = $header.closest(".SMRuleSetup_Depending_Dependent_RuleContainer");
            var dependent = $header.attr('dependent');
            if (dependent != null && dependent.split(',').length > 2)
                onclickDataRender(dependent.split(','), e, 'SMRuleSetup_Dependent_More');
        }
        else if ($header.attr('class') == "SMRuleSetup_ApllicableOnCount") {
            $header = $header.closest(".SMRuleSetup_AttributeRulesBinding");
            var commonRuleSectypes = $header.attr('commonRuleSectypes');
            onclickDataRender(commonRuleSectypes.split(','), e, 'SMRuleSetup_ApllicableOnCount');
        }
        else if ($header.hasClass("RC_RuleTypeName")) {
            var headingtext = $header.parent().parent().siblings().find('.RC_TypeName').text();
            $("#SMRuleSetup_TypeName").html(headingtext);
            $(".SMRuleSetup_Outer_Div_Pointer").css('display', 'none');
            $('.SMRuleSetup_attrContainerDiv').css('display', 'none');
            $("#SMRuleSetup_SearchOnRules").css('display', 'none');
        }
        else if ($header.attr('id') == "SMRuleSetup_searchIcon") {
            $("#SMRuleSetup_SearchOnRules").css('display', 'inline-block');
            $("#SMRuleSetup_SearchOnRules").focus();
            $("#SMRuleSetup_SearchOnRules").val('');
        }
        else {
            $(".SMRuleSetup_Outer_Div_Pointer").css('display', 'none');
            $('.SMRuleSetup_attrContainerDiv').css('display', 'none');
            $("#SMRuleSetup_SearchOnRules").css('display', 'none');
        }
    });

    function onclickDataRender(data, e, element) {
        var html = "";
        $(".SMRuleSetup_attrContainerDiv").empty();

        if (data.length > 2 && (element == "SMRuleSetup_Depending_More" || element == "SMRuleSetup_Dependent_More")) {
            data = data.splice(2, data.length);
        }
        html += "<table border='0' cellpadding='0' cellspacing='0' width='100%'>";
        for (var i = 0; i < data.length; i++) {
            if (data[i].length > 20)
                html += '<tr><td class="SMRuleSetUp_Attr" title="' + data[i] + '">' + data[i].substring(0, 19) + '...' + '</td>';
            else
                html += '<tr><td class="SMRuleSetUp_Attr" title="' + data[i] + '">' + data[i] + '</td>';
        }
        html += '</table>';

        $(".SMRuleSetup_attrContainerDiv").append(html);

        var popUpOffset = $(e.target).closest('.' + element).offset();
        $(".SMRuleSetup_attrContainerDiv").css({ top: popUpOffset.top + 30, left: (popUpOffset.left - 30), position: 'absolute' });

        if (element == "SMRuleSetup_Mapped_Types")
            $(".SMRuleSetup_Outer_Div_Pointer").css({ top: popUpOffset.top + 10, left: (popUpOffset.left) + 30, position: 'absolute' });
        else
            $(".SMRuleSetup_Outer_Div_Pointer").css({ top: popUpOffset.top + 10, left: (popUpOffset.left), position: 'absolute' });

        $('.SMRuleSetup_Outer_Div_Pointer').css('display', 'block');
        $('.SMRuleSetup_attrContainerDiv').css('display', 'block');
        e.stopPropagation();
    }

    function toShowEmptyRuleSet(isSearch) {
        var noResultsDiv;
        //if (typeof window.parent.leftMenu != "undefined") {
        if (typeof isSearch != "undefined" && isSearch == false) {
            var noResultsDiv = $("[id$='SMRuleSetup_NullAttributeRuleContainer']");
            $('#SMAttributeRuleContainer').hide();
            if (leftMenuPath != "")
                leftMenuPath.showNoRecordsMsg("No Rules found", noResultsDiv);
        }
        else if (typeof isSearch != "undefined" && isSearch == true) {
            var noResultsDiv = $("[id$='SMRuleSetup_NullAttributeRuleContainerForSearch']");
            $('#SMRuleSetup_RulesContentId').hide();
            if (leftMenuPath != "")
                leftMenuPath.showNoRecordsMsg("No Rules found matching your search criteria.", noResultsDiv);
            //}
        }
        $('.SMRuleSetup_Collapse').addClass('SMRuleSetup_CollapseDisable').removeClass('SMRuleSetup_CollapseEnable');
    }

    function tohideEmptyRuleSet(ruleTypeName) {
        $('#SMAttributeRuleContainer').show();
        $('#SMRuleSetup_RulesContentId').show();
        //if (typeof window.parent.leftMenu != "undefined") {
        if (leftMenuPath != "")
            leftMenuPath.hideNoRecordsMsg();
        //}
        $('#SMRuleSetup_NullAttributeRuleContainer').hide();
        $("[id$='SMRuleSetup_NullAttributeRuleContainerForSearch']").hide();
        if (typeof ruleTypeName != "undefined" && ruleTypeName.toLowerCase().replace(/\s+/g, '').indexOf("conditional") != -1)
            $('.SMRuleSetup_Collapse').addClass('SMRuleSetup_CollapseDisable').removeClass('SMRuleSetup_CollapseEnable');
        else
            $('.SMRuleSetup_Collapse').addClass('SMRuleSetup_CollapseEnable').removeClass('SMRuleSetup_CollapseDisable');
    }


    /////////////////////
    // Bind Slide Menu //
    /////////////////////

    function bindSlideMenu(filterData) {
        var rightFilterContainer = $("#SRMRuleCatalogRightFilterContainer");
        rightFilterContainer.empty();

        var objFilterData = {};
        objFilterData.pivotElementId = "SRMRuleCatalogRightFilter";
        objFilterData.id = "SMRRuleCatalogRightFilterDiv";
        objFilterData.container = rightFilterContainer;
        objFilterData.data = filterData;
        smslidemenu.init(objFilterData, function () { filterApply(); });

        smslidemenu.hide("SMRRuleCatalogRightFilterDiv");
        filtersSelectedDataSelected = smslidemenu.getAllData("SMRRuleCatalogRightFilterDiv");
    }

    function filterApply() {
        var defaultDateParam = "Any Time";
        var selectedTilesParam = "";
        var selectedCorpActionsParam = "";
        var selectedExceptionTypesParam = "";
        var selectedSecTypesParam = "";
        var selectedExternalSystemParam = "";
        var userSpecificParam = false;

        var target = $("#smselect_sectionValues");

        var types = [];
        types = getRightFilterTypesList();

        //srmRuleCatalog.GetData(types, rulesTypes, runTimeButtonText);
        //Fetch Data Again
        var params = {};
        params.ModuleID = htmlBulkUploadGrid._currentModuleID;
        params.TabID = parseInt(smselect.getSelectedOption(target)[0].value);
        params.UserName = htmlBulkUploadGrid._securityInfo.UserName;
        params.TypeIDs = types;

        CallCommonServiceMethod('GetRulesForModuleIDAndTabID', params, OnSuccess_GetSectionData, OnFailure, null, false);

        //Set First Option in Section Multi-Select
        //setFirstOptionInSectionMultiSelect();

        smslidemenu.hide("SMRRuleCatalogRightFilterDiv");
    }

    function processFilters(filterGroupItems, filterGroupHeaderID, filterGroupHeaderTitle, checkedValuesList, txtId, txtName, filterType, checkerItem, isSearchable, isDateTypeControl, selectAllText, state) {
        var filterItems = [];
        var checkedValuesArray = checkedValuesList.split(',');
        $.each(filterGroupItems, function (index, item) {
            var objFilterItem = {};
            objFilterItem.text = item[txtName];
            objFilterItem.value = item[txtId];
            if (checkedValuesArray.indexOf(item[checkerItem].toString()) > -1)
                objFilterItem.isSelected = "true";
            else
                objFilterItem.isSelected = "false";
            if (isDateTypeControl) {
                objFilterItem.dateFilterType = item.dateFilterType;
                objFilterItem.rangeStartDate = item.rangeStartDate;
                objFilterItem.rangeEndDate = item.rangeEndDate;
                objFilterItem.singleDate = item.singleDate;
                objFilterItem.dateType = item.dateType;
            }
            filterItems.push(objFilterItem);
        });
        var filterDataItem = {};
        if (filterGroupHeaderID != null)
            filterDataItem.identity = filterGroupHeaderID;
        filterDataItem.sectionHeader = filterGroupHeaderTitle;
        if (selectAllText != null)
            filterDataItem.selectAllText = selectAllText;
        filterDataItem.state = state;
        filterDataItem.sectionType = filterType;
        filterDataItem.listItems = filterItems;
        filterDataItem.isSearchable = isSearchable;
        filterDataItem.maxHeight = $(window).height() - 425;
        return filterDataItem;
    }

    function getRightFilterTypesList() {
        var filtersSelectedData = smslidemenu.getAllData("SMRRuleCatalogRightFilterDiv");

        var types = [];
        if (htmlBulkUploadGrid._currentModuleID == 3) {
            if (filtersSelectedData.SecurityCB.SelectedValue != "") {
                for (var i = 0; i < filtersSelectedData.SecurityCB.SelectedValue.split(",").length; i++) {
                    types.push(parseInt(filtersSelectedData.SecurityCB.SelectedValue.split(",")[i]));
                }
            }
        }
        else if (htmlBulkUploadGrid._currentModuleID == 6) {
            if (filtersSelectedData.EntityCB.SelectedValue != "") {
                for (var i = 0; i < filtersSelectedData.EntityCB.SelectedValue.split(",").length; i++) {
                    types.push(parseInt(filtersSelectedData.EntityCB.SelectedValue.split(",")[i]));
                }
            }
        }
        else if (htmlBulkUploadGrid._currentModuleID == 9) {
            if (filtersSelectedData.CorpCB.SelectedValue != "") {
                for (var i = 0; i < filtersSelectedData.CorpCB.SelectedValue.split(",").length; i++) {
                    types.push(parseInt(filtersSelectedData.CorpCB.SelectedValue.split(",")[i]));
                }
            }
        }
        else if (htmlBulkUploadGrid._currentModuleID == 18) {
            if (filtersSelectedData.FundCB.SelectedValue != "") {
                for (var i = 0; i < filtersSelectedData.FundCB.SelectedValue.split(",").length; i++) {
                    types.push(parseInt(filtersSelectedData.FundCB.SelectedValue.split(",")[i]));
                }
            }
        }
        else if (htmlBulkUploadGrid._currentModuleID == 20) {
            if (filtersSelectedData.RoleCB.SelectedValue != "") {
                for (var i = 0; i < filtersSelectedData.RoleCB.SelectedValue.split(",").length; i++) {
                    types.push(parseInt(filtersSelectedData.RoleCB.SelectedValue.split(",")[i]));
                }
            }
        }
        return types;
    }

    function isCollapsible(data) {
        var section = "";
        var ruleTypeName = "";
        var flag = false;
        $('.SMRuleSetup_Collapse').text('Collapse All');

        for (var t = 0; t < data.rules.length; t++) {
            section = data.rules[t].section;
            for (var m = 0; m < data.rules[t].ruletypes.length; m++) {
                ruleTypeName = data.rules[t].ruletypes[m].ruleTypeName;
                if (section.toLowerCase().indexOf('modeler') != -1) {
                    if ((ruleTypeName.toLowerCase().indexOf('mnemonic') != -1) || (ruleTypeName.toLowerCase().indexOf('calculated') != -1) || (ruleTypeName.toLowerCase().indexOf('validation') != -1) || (ruleTypeName.toLowerCase().indexOf('alert') != -1) || (ruleTypeName.toLowerCase().indexOf('basket') != -1)) {
                        flag = true;
                    }
                }
                else if (section.toLowerCase().indexOf('upstream') != -1) {
                    if (ruleTypeName.toLowerCase().indexOf('transformation') != -1) {
                        flag = true;
                    }
                }
                else if (section.toLowerCase().indexOf('downstream') != -1) {
                    if (ruleTypeName.toLowerCase().indexOf('transformation') != -1) {
                        flag = true;
                    }
                }
                if (flag) {
                    $('.SMRuleSetup_Collapse').addClass('SMRuleSetup_CollapseEnable').removeClass('SMRuleSetup_CollapseDisable');
                }
                else {
                    $('.SMRuleSetup_Collapse').addClass('SMRuleSetup_CollapseDisable').removeClass('SMRuleSetup_CollapseEnable');
                }
            }
        }
    }


    $(".SMRuleSetup_Collapse").unbind("click").click(function (e) {

        var $this = $(e.target);
        $this = $this.closest(".SMRuleSetup_Collapse");


        if ($this.text().trim() == "Collapse All") {
            var collapsed = $(".SMRuleSetup_Rules[isCollapsed='0']");
            collapsed.attr("isCollapsed", 1);

            collapsed.find(".fa-caret-down").removeClass("fa-caret-down").addClass("fa-caret-right");
            var $content = collapsed.find(".SMRuleSetup_hidden_contents");
            $content.slideToggle(300);
            collapsed.addClass('SMRuleSetup_NotSelectedRule').removeClass('SMRuleSetup_SelectedRule');
            $('.SMRuleSetup_Collapse').html('Expand All');

        }
        else {
            var collapsed = $(".SMRuleSetup_Rules[isCollapsed='1']");
            collapsed.attr("isCollapsed", 0);
            collapsed.addClass('SMRuleSetup_SelectedRule').removeClass('SMRuleSetup_NotSelectedRule');
            collapsed.find(".fa-caret-right").removeClass("fa-caret-right").addClass("fa-caret-down");
            var $content = collapsed.find(".SMRuleSetup_hidden_contents");
            $content.slideToggle(300);
            $('.SMRuleSetup_Collapse').html('Collapse All');

        }
        e.stopPropagation();
    });


    //////////////////////////////////////////
    // CODE For Single DROPDOWN For Section //
    //////////////////////////////////////////
    function ApplySMSelect(id, isRunningText, data, isSearch, container, selectedItem) {

        smselect.create(
                    {
                        id: id,
                        container: $(container),
                        isRunningText: isRunningText,
                        data: data,
                        selectedText: typeof selectedItem != typeof undefined && typeof selectedItem.text != typeof undefined ? selectedItem.text : "",
                        showSearch: isSearch,
                        ready: function (selectelement) {
                            selectelement.css({ /*'border': '1px solid #CECECE',*/ 'border-left': 'none', height: '22px', width: '200px', 'vertical-align': 'middle' });
                            //selectelement.find(".smselectrun").css({ height: '27px'});
                            //if (data.length > 5) {
                            //    selectelement.find(".smselectcon").height(selectelement.find(".smselectcon").height() + 3);
                            //}
                            selectelement.on('change', function (ee) {
                                ddlOnChangeHandler(ee);
                            });

                            smselect.setOptionByIndex(selectelement, 0);
                        }
                    });
    }

    //This function is called when a Multi-Select is changed.
    function ddlOnChangeHandler(ee) {
        var target = $(ee.currentTarget);

        if (htmlBulkUploadGrid._koBindingApplied) {

            //Clear The Search text    
            $("#RC_LeftPaneSearchInputBox").val("");

            if (target[0].id == 'smselect_sectionValues') {

                var types = [];

                if (htmlBulkUploadGrid._securityInfo.TypeID != null && htmlBulkUploadGrid._securityInfo.TypeID > 0) {
                    types.push(htmlBulkUploadGrid._securityInfo.TypeID);
                }
                else {
                    types = getRightFilterTypesList();
                }
                var params = {};
                params.ModuleID = htmlBulkUploadGrid._currentModuleID;
                params.TabID = parseInt(smselect.getSelectedOption(target)[0].value);
                params.UserName = htmlBulkUploadGrid._securityInfo.UserName;
                params.TypeIDs = types;

                CallCommonServiceMethod('GetRulesForModuleIDAndTabID', params, OnSuccess_GetSectionData, OnFailure, null, false);
            }
        }
    }

    //init function to initialise SMSelect
    function init() {

        var sectionList = [{ "text": "MODELER", "value": "2" }, { "text": "UPSTREAM", "value": "0" }, { "text": "DOWNSTREAM", "value": "1" }];

        ApplySMSelect('sectionValues', false, sectionList, false, "#RC_SectionTypeSMSelectDiv");
    }

    ////////////////////////////
    // "Initializer" function // 
    ////////////////////////////
    HtmlBulkUploadGrid.prototype.Initializer = function Initializer(controlInfo, securityInfo) {

        //Set Control ID Info & Security Info
        htmlBulkUploadGrid._controlIdInfo = eval("(" + controlInfo + ")");
        htmlBulkUploadGrid._securityInfo = eval("(" + securityInfo + ")");

        init();

        //Hide Error Div Initially
        $("#SRFPMRuleCatalogueCommonErrorMsgDiv").hide();

        if (htmlBulkUploadGrid._securityInfo.ModuleID != null && htmlBulkUploadGrid._securityInfo.ModuleID > 0 && htmlBulkUploadGrid._securityInfo.TypeID != null && htmlBulkUploadGrid._securityInfo.TypeID > 0) {

            //Set ModuleID
            htmlBulkUploadGrid._currentModuleID = htmlBulkUploadGrid._securityInfo.ModuleID;

            //Set TypeID
            var types = [];
            types.push(htmlBulkUploadGrid._securityInfo.TypeID);

            //Hide Top Panel
            $("[id$='SRMPanelTop']").hide();

            //Get Data
            var params = {};

            params.ModuleID = htmlBulkUploadGrid._currentModuleID;
            //params.TabID = parseInt(smselect.getSelectedOption(target)[0].value);
            params.TabID = 2;
            params.UserName = htmlBulkUploadGrid._securityInfo.UserName;
            params.TypeIDs = types;

            CallCommonServiceMethod('GetRulesForModuleIDAndTabID', params, OnSuccess_GetSectionData, OnFailure, null, false);

            //Set First Option in Section Multi-Select
            setFirstOptionInSectionMultiSelect();

        }
        else {
            htmlBulkUploadGrid.initialiseTabs();
        }

        //Apply CSS
        SRMRuleCatalog_ApplyDynamicCSS();
    }


    ///////////////////////
    // Apply Dynamic CSS //
    ///////////////////////

    //On Resize Function
    $(window).resize(function () {
        SRMRuleCatalog_ApplyDynamicCSS();
    })

    //Actual Dynamic CSS
    function SRMRuleCatalog_ApplyDynamicCSS() {
        $("#SRFPMRuleCatalogueParentDiv").height($(window).height() - $("#srm_moduleTabs").height() - 15);
        $("#SRFPMRuleCatalogueCommonErrorMsgDiv").height($(window).height() - $("#srm_moduleTabs").height() - 15);
        $("#SRFPMRuleCatalogueCommonErrorMsgDiv").width($(window).width() - 5);
        $('#SMAttributeRuleContainer').height($("#SRFPMRuleCatalogueParentDiv").height());

        $('#SMRuleSetup_RulesContentId').height(($("#SRFPMRuleCatalogueParentDiv").height() - $("#SMRuleSetup_Header_Contents").height()));

        $("#RC_SectionData").height($("#SRFPMRuleCatalogueParentDiv").height() - $("#RC_LeftTopDiv").height());
        $('#RC_RightMainDiv').width($(window).outerWidth() - 310);
    }

    //function SRMRuleCatalog_ApplyDynamicCSS_RightFilter() {
    //    $("#SMRRuleCatalogRightFilterDiv .smslimScrollDiv").height($("#SMRRuleCatalogRightFilterDiv").height() -  200);
    //    $("#SMRRuleCatalogRightFilterDiv .smslidemenu_itemListDiv").height($("#SMRRuleCatalogRightFilterDiv").height() - 200);
    //}

    return htmlBulkUploadGrid;
})();

////Commented By Dhruv
//$(function () {
//    htmlBulkUploadGrid.initialiseTabs();
//})

