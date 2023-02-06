var uniquenessSetup = (function () {

    function UniquenessSetup() {
        this._controlIdInfo = null;
        this._securityInfo = null;
        this._pageViewModelInstance = null;

        this._allSectypesInfo = [];
        this._selectedSectypes = [];
        this._keyIdToBeDeleted = -1;
        //this._selectedAttributes = [];
        //this._selectedLegAttributes = [];
        //this._selectedLeg = {};
    }

    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    var uniquenessSetup = new UniquenessSetup();

    /////////////////
    // VIEW MODELS // - CHANGED FINAL
    /////////////////

    // Page View Model - CHANGED FINAL
    function pageViewModel(data) {
        var self = this;

        //Selected Item
        //self.selectedChainInfo = ko.observable(new chainViewModel({ KeyID: -1, IsMaster: true, KeyName: "", SelectedAttributes: [], SelectedLeg: null, SelectedLegAttributes: [], SelectedSectypes: [] }));

        self.chainListing = ko.observableArray();
        self.chainAttributeLevelListing = ko.observableArray();
        self.chainLegLevelListing = ko.observableArray();

        if (typeof (data) != typeof (undefined)) {
            //No Keys for Current Filters
            if (typeof (window.parent.leftMenu) !== "undefined") {
                if (data.length === 0) {

                    //Uncomment This
                    $("#UniquenessSetupMain").hide();
                    window.parent.leftMenu.showNoRecordsMsg("No keys found matching your search criteria.", $("#UniquenessSetupMainErrorDiv"));

                    //CSS for smNoRecordsContainer div created inside UniquenessSetupMainErrorDiv div
                    var pd_left = ($("#UniquenessSetupMainErrorDiv").width() - $("#smNoRecordsContainer").width() - 2) / 2;
                    var pd_top = (($("#UniquenessSetupMainErrorDiv").height() - $("#smNoRecordsContainer").height() - 2) / 2) - 20;
                    $("#UniquenessSetupMainErrorDiv #smNoRecordsContainer").css({ "padding-left": pd_left, "padding-top": pd_top });
                }
                else {
                    for (var item in data) {
                        if (data[item].KeyID > 0) {
                            self.chainListing.push(new chainViewModel(data[item]));
                            if (data[item].IsMaster) {
                                self.chainAttributeLevelListing.push(new chainViewModel(data[item]));
                            }
                            else {
                                self.chainLegLevelListing.push(new chainViewModel(data[item]));
                            }
                        }
                    }

                    //Uncomment This
                    $("#UniquenessSetupMain").show();
                    window.parent.leftMenu.hideNoRecordsMsg();

                    $(".UniquenessSetupChain").css("display", "block");
                }
            }
        }
    };

    //Chain View Model - Change Background and Text color - CHANGED FINAL
    function chainViewModel(data) {
        var self = this;

        //self.isSelected = ko.observable(false);
        self.chainIsEdit = ko.observable(false);

        self.chainKeyID = data.KeyID;
        self.chainKeyName = ko.observable(data.KeyName);
        self.chainSelectedSectypes = [];

        for (var item1 in uniquenessSetup._allSectypesInfo) {
            for (var item in data.SelectedSectypes) {
                if (uniquenessSetup._allSectypesInfo[item1].SectypeID == data.SelectedSectypes[item]) {
                    self.chainSelectedSectypes.push({ value: uniquenessSetup._allSectypesInfo[item1].SectypeID, text: uniquenessSetup._allSectypesInfo[item1].SectypeName });
                    break;
                }
            }
        }

        self.chainSelectedSectypesDisplayText = ko.computed(function () {
            var sectypes = [];
            ko.utils.arrayForEach(self.chainSelectedSectypes, function (sectype) {
                if (sectypes.length < 3)
                    sectypes.push(sectype.text);
            });

            var str = sectypes.join(", ");

            if (sectypes.length < self.chainSelectedSectypes.length)
                str += " and " + (self.chainSelectedSectypes.length - sectypes.length) + " more.";

            return str;
        });

        self.chainSelectedSectypesTooltipText = ko.computed(function () {
            var sectypes = [];
            ko.utils.arrayForEach(self.chainSelectedSectypes, function (sectype) {
                sectypes.push(sectype.text);
            });

            var str = sectypes.join(", ");
            var replacement = " and";
            str = str.replace(/,([^,]*)$/, replacement + '$1');
            return str;
        });

        self.chainIsMaster = ko.observable(data.IsMaster);
        //self.chainLevel = ko.computed(function () {
        //    return self.chainIsMaster ? "attribute" : "leg";
        //});

        //Selected Attributes
        self.chainSelectedAttributes = [];
        for (var item in data.SelectedAttributes) {
            self.chainSelectedAttributes.push({ text: data.SelectedAttributes[item].AttributeName, value: (data.SelectedAttributes[item].AttributeIDs + "@&@" + data.SelectedAttributes[item].AreAdditionalLegAttributes) });
        }

        self.chainSelectedAttributesDisplayText = ko.computed(function () {
            var attrs = [];
            ko.utils.arrayForEach(self.chainSelectedAttributes, function (attr) {
                if (attrs.length < 3)
                    attrs.push(attr.text);
            });

            var str = attrs.join(", ");

            if (attrs.length < self.chainSelectedAttributes.length)
                str += " and " + (self.chainSelectedAttributes.length - attrs.length) + " more.";

            return str;
        });

        self.chainSelectedAttributesTooltipText = ko.computed(function () {
            var attrs = [];
            ko.utils.arrayForEach(self.chainSelectedAttributes, function (attr) {
                attrs.push(attr.text);
            });

            var str = attrs.join(", ");
            var replacement = " and";
            str = str.replace(/,([^,]*)$/, replacement + '$1');
            return str;
        });

        //Selected Leg
        self.chainSelectedLeg = {};
        if (data.SelectedLeg != null && typeof data.SelectedLeg.LegName != typeof undefined) {
            self.chainSelectedLeg = { text: data.SelectedLeg.LegName, value: (data.SelectedLeg.LegIDs + "@&@" + data.SelectedLeg.AreAdditionalLegs) };
        }

        self.chainSelectedLegDisplayText = self.chainSelectedLeg.text;

        //Selected Leg Attributes
        self.chainSelectedLegAttributes = [];
        for (var item in data.SelectedLegAttributes) {
            self.chainSelectedLegAttributes.push({ text: data.SelectedLegAttributes[item].AttributeName, value: (data.SelectedLegAttributes[item].AttributeIDs + "@&@" + data.SelectedLegAttributes[item].AreAdditionalLegAttributes) });
        }

        self.chainSelectedLegAttributesDisplayText = ko.computed(function () {
            var attrs = [];
            ko.utils.arrayForEach(self.chainSelectedLegAttributes, function (attr) {
                if (attrs.length < 3)
                    attrs.push(attr.text);
            });

            var str = attrs.join(", ");

            if (attrs.length < self.chainSelectedLegAttributes.length)
                str += " and " + (self.chainSelectedLegAttributes.length - attrs.length) + " more.";

            return str;
        });

        self.chainSelectedLegAttributesTooltipText = ko.computed(function () {
            var attrs = [];
            ko.utils.arrayForEach(self.chainSelectedLegAttributes, function (attr) {
                attrs.push(attr.text);
            });

            var str = attrs.join(", ");
            var replacement = " and";
            str = str.replace(/,([^,]*)$/, replacement + '$1');
            return str;
        });


        //Is Across Securities
        self.chainIsAcrossSecurities = ko.observable(data.IsAcrossSecurities);

        self.chainIsAcrossSecuritiesDisplayText = ko.computed(function () {
            return self.chainIsAcrossSecurities() ? 'Across Securities' : 'Within Security';
        });

        //draft-workflow text
        self.checkInDrafts = ko.observable(data.CheckInDrafts);

        self.checkInDrafts_DB = data.CheckInDrafts;

        self.checkInWorkflows = ko.observable(data.CheckInWorkflows);

        self.checkInWorkflows_DB = data.CheckInWorkflows;
        
        self.checkInText = ko.computed(function () {
            var res = "";
            if (!res.length) {
                res += self.checkInDrafts() ? "Drafts" : "";
            }

            if (res.length) {
                res += self.checkInWorkflows() ? " and Workflows" : "";
            }
            else
                res += self.checkInWorkflows() ? "Workflows" : "";
            if (res == "")
                res = "Not set";
            return res;
        });


        //null as unique
        self.nullAsUnique = ko.observable(data.NullAsUnique);

        self.nullAsUnique_DB = data.NullAsUnique;

        self.nullAsUniqueText = ko.computed(() =>  self.nullAsUnique() ? 'Yes' : 'No');

        //self.chainLegSubLevel = data.LegSubLevel;

        //self.chainSelectedSectypesDisplayString = ko.computed(function () {
        //    var displayString = '';

        //    if (self.chainSelectedSectypes() != "") {
        //        //Edit the Logic here
        //        displayString = self.chainStartDateTime + ' to ' + self.chainEndDateTime();
        //    }
        //    else {
        //        displayString = "No Sectype Selected";
        //    }
        //    return displayString;
        //});

        //self.chainBackgroundColor = ko.computed(function () {
        //    return self.isSelected() ? '#72b9e6' : 'url("css/images/chain_texture.png")';
        //});

        //self.chainTextColor = ko.computed(function () {
        //    return self.isSelected() ? '#ffffff' : '#000000';
        //});
    };

    //////////////////////////////////
    // Call Common Service Fuctions // - CHANGED FINAL
    //////////////////////////////////
    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }

    /////////////////////////////////////////////////
    // Callback Functions for COMMON SERVICE Calls //
    /////////////////////////////////////////////////

    // This function is called on Success Service Call on Load - CHANGED FINAL
    function OnSuccess_GetAllSectypes(result) {

        if (result.d != null) {
            //Store All Sectypes Data
            uniquenessSetup._allSectypesInfo = [];
            uniquenessSetup._selectedSectypes = [];
            for (var item in result.d) {
                var tempObj = {};
                tempObj.SectypeID = result.d[item].SectypeID;
                tempObj.SectypeName = result.d[item].SectypeName;

                var tempObj1 = {};
                tempObj1.value = result.d[item].SectypeID;
                tempObj1.text = result.d[item].SectypeName;

                uniquenessSetup._allSectypesInfo.push(tempObj);
                uniquenessSetup._selectedSectypes.push(tempObj1);
            }

            //SMSelect for Sectype Filter
            ApplyMultiSelect('UniquenessSetupSectypeFilterMultiSelectedItems', false, uniquenessSetup._selectedSectypes, true, '#UniquenessSetupSectypeFilterMultiSelect', 'Security Types', true);

            // Get data from Service Call
            var params = {};
            params.selectedSectypes = [];

            for (var item in uniquenessSetup._selectedSectypes) {
                params.selectedSectypes.push(uniquenessSetup._selectedSectypes[item].value);
            }
            CallCommonServiceMethod('GetUniqueKeysForSelectedSectypes', params, OnSuccess_LoadUniqueKeysDataInitial, OnFailure, null, false);
        }
    }

    // This function is called on Success Service Call on Load - CHANGED FINAL
    function OnSuccess_LoadUniqueKeysDataInitial(result) {

        if (typeof ko !== 'undefined') {                                                                //if ko object exists, then UniquenessSetup is currently pageview

            uniquenessSetup._pageViewModelInstance = new pageViewModel(result.d);

            ko.applyBindings(uniquenessSetup._pageViewModelInstance);

            //if (result.d.length === 0) {
            //    //TRIGGER CREATE NEW BUTTON HERE
            //    OnClick_CreateNewUniqueKey();
            //}
            //else {
            //    $('.UniquenessSetupChain')[0].click();
            //}
            // First Chain Item is selected on load.
            //$($('.UniquenessSetupChain')[0]).children('.UniquenessSetupChainInfo').click();
            //$('.UniquenessSetupChain')[0].click();
        }
    };

    // This function is called on Success Service Call on Save or Update - CHANGED FINAL
    function OnSuccess_LoadUniqueKeysDataLater(result) {

        //Hide Add New Key Div (Required if this has been called after new key creation)
        $("#UniquenessSetupMainNewKeyDiv").css("display", "none");

        // Empty the observable array and clear selected item
        uniquenessSetup._pageViewModelInstance.chainListing.removeAll();
        uniquenessSetup._pageViewModelInstance.chainAttributeLevelListing.removeAll();
        uniquenessSetup._pageViewModelInstance.chainLegLevelListing.removeAll();
        //uniquenessSetup._pageViewModelInstance.selectedChainInfo = ko.observable(new chainViewModel({ KeyID: -1, IsMaster: true, KeyName: "", SelectedAttributes: [], SelectedLeg: null, SelectedLegAttributes: [], SelectedSectypes: [] }));

        // Enter the data in observable array again
        if (typeof (result.d) != typeof undefined) {

            if (result.d.length === 0) {
                $("#UniquenessSetupMain").hide();
                window.parent.leftMenu.showNoRecordsMsg("No keys found matching your search criteria.", $("#UniquenessSetupMainErrorDiv"));
            }
            else {
                var data = result.d;

                for (var item in data) {
                    if (data[item].KeyID > 0) {
                        uniquenessSetup._pageViewModelInstance.chainListing.push(new chainViewModel(data[item]));
                        if (data[item].IsMaster) {
                            uniquenessSetup._pageViewModelInstance.chainAttributeLevelListing.push(new chainViewModel(data[item]));
                        }
                        else {
                            uniquenessSetup._pageViewModelInstance.chainLegLevelListing.push(new chainViewModel(data[item]));
                        }
                    }
                }

                $("#UniquenessSetupMain").show();
                window.parent.leftMenu.hideNoRecordsMsg();
            }
        }
    }

    // This function is called on Success Service Call on Chain Sectype Change to populate Common Master attributes List - CHANGED FINAL
    function OnSuccess_GetCommonMasterAttributesForSelectedSectypes(result) {
        var masterAttributesList = [];

        if (result.d.CommonMasterAttributesList.length > 0) {

            var currentChainKeyID = result.d.KeyID;

            for (var item in result.d.CommonMasterAttributesList) {
                var tempObj = {};
                tempObj.text = result.d.CommonMasterAttributesList[item].AttributeName;
                tempObj.value = result.d.CommonMasterAttributesList[item].AttributeIDs + "@&@" + result.d.CommonMasterAttributesList[item].AreAdditionalLegAttributes;
                masterAttributesList.push(tempObj);
            }

            var currentChainSelectedAttributes = [];

            if (currentChainKeyID > 0) {

                for (var item in uniquenessSetup._pageViewModelInstance.chainAttributeLevelListing()) {
                    if (uniquenessSetup._pageViewModelInstance.chainAttributeLevelListing()[item].chainKeyID == currentChainKeyID) {
                        currentChainSelectedAttributes = uniquenessSetup._pageViewModelInstance.chainAttributeLevelListing()[item].chainSelectedAttributes;
                        break;
                    }
                }

                //SMSelect for Attributes Selection
                $('#UniquenessSetup_AttributeLevel_EditMode_Item_' + currentChainKeyID + '_Block_3_Info').html("");
                ApplyMultiSelect('attributeLevelValueDiv_Chain_' + currentChainKeyID, false, masterAttributesList, true, '#UniquenessSetup_AttributeLevel_EditMode_Item_' + currentChainKeyID + '_Block_3_Info', 'Attributes', false, currentChainSelectedAttributes);
            }
            else {
                //New Key Case
                //SMSelect for Attributes Selection
                $('#UniquenessSetup_NK_Block_3_A_Info').html("");
                ApplyMultiSelect('attributeLevelValueDiv_NK', false, masterAttributesList, true, '#UniquenessSetup_NK_Block_3_A_Info', 'Attributes', false, currentChainSelectedAttributes);
            }
        }
    }

    // This function is called on Success Service Call on Chain Sectype Change to populate Common Legs List
    function OnSuccess_GetCommonLegsForSelectedSectypes(result) {
        var legsList = [];

        if (result.d.length > 0) {
            for (var item in result.d) {
                var tempObj = {};
                tempObj.text = result.d[item].LegName;
                tempObj.value = result.d[item].LegIDs + "@&@" + result.d[item].AreAdditionalLegs;
                legsList.push(tempObj);
            }

            //SMSelect for Leg Selection
            $("#UniquenessSetup_NK_Block_3_B_Info").html("");
            ApplySMSelect('legLevelValueDiv_NK', false, legsList, true, '#UniquenessSetup_NK_Block_3_B_Info');
        }
        else {
            //If no legs on selected sectypes
            var tempObj = {};
            tempObj.text = "No Commmon Legs on selected Security Types";
            tempObj.value = "-1@&@-1"
            legsList.push(tempObj);

            //SMSelect for Leg Selection
            $("#UniquenessSetup_NK_Block_3_B_Info").html("");
            ApplySMSelect('legLevelValueDiv_NK', false, legsList, true, '#UniquenessSetup_NK_Block_3_B_Info');

            //$("#UniquenessSetupKeyDetailsInfoDiv_4_B").css("display", "none");
        }
    }

    // This function is called on Success Service Call on Chain Common Leg Change to populate Common Leg Attribute List - CHANGED FINAL
    function OnSuccess_GetCommonLegAttributesForSelectedLegName(result) {
        var legAttributesList = [];

        if (result.d.CommonLegAttributesList.length > 0) {

            var currentChainKeyID = result.d.KeyID;

            for (var item in result.d.CommonLegAttributesList) {
                var tempObj = {};
                tempObj.text = result.d.CommonLegAttributesList[item].AttributeName;
                tempObj.value = result.d.CommonLegAttributesList[item].AttributeIDs + "@&@" + result.d.CommonLegAttributesList[item].AreAdditionalLegAttributes;
                legAttributesList.push(tempObj);
            }

            var currentChainSelectedLegAttributes = [];

            if (currentChainKeyID > 0) {

                for (var item in uniquenessSetup._pageViewModelInstance.chainLegLevelListing()) {
                    if (uniquenessSetup._pageViewModelInstance.chainLegLevelListing()[item].chainKeyID == currentChainKeyID) {
                        currentChainSelectedLegAttributes = uniquenessSetup._pageViewModelInstance.chainLegLevelListing()[item].chainSelectedLegAttributes;
                        break;
                    }
                }

                //SMSelect for Leg Attributes Selection
                $('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_4_Info').html("");
                ApplyMultiSelect('legLevelAttributeValueDiv_Chain_' + currentChainKeyID, false, legAttributesList, true, '#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_4_Info', 'Leg Attributes', false, currentChainSelectedLegAttributes);
            }
            else {
                //New Key Case
                //SMSelect for Leg Attributes Selection
                $('#UniquenessSetup_NK_Block_4_Info').html("");
                ApplyMultiSelect('legLevelAttributeValueDiv_NK', false, legAttributesList, true, '#UniquenessSetup_NK_Block_4_Info', 'Leg Attributes', false, currentChainSelectedLegAttributes);
            }

            //SMSelect for Attributes Selection
            //ApplyMultiSelect('legLevelAttributeValueDivSelectedItems', false, legAttributesList, true, '#legLevelAttributeValueDiv', 'Leg Attributes', false, uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLegAttributes);
        }
        else {
            //If no leg attributes on selected leg
            var tempObj = {};
            tempObj.text = "No Commmon Attributes on Selected Leg";
            tempObj.value = "-1@&@-1"
            legAttributesList.push(tempObj);

            var currentChainSelectedLegAttributes = [];

            //SMSelect for Leg Attributes Selection
            $("#UniquenessSetup_NK_Block_4_Info").html("");
            ApplyMultiSelect('legLevelAttributeValueDiv_NK', false, legAttributesList, true, '#UniquenessSetup_NK_Block_4_Info', 'Leg Attributes', false, currentChainSelectedLegAttributes);
        }
    }

    // This function is called on Success Service Call on Create New Key - CHANGED FINAL
    function OnSuccess_CreateUniqueKey(result) {
        // result.d.status 
        //  0 : Unique Key Validation failed
        // -1 : Data is NOT Unique
        //  1 : Key Successfully Inserted/Updated.
        //  2 : Key Successfully Validated (NOT Inserted/Updated) - NOT for UI

        if (result.d.status == 0) {
            //Display Error Msg
            $("#" + uniquenessSetup._controlIdInfo.LblErrorId).text(result.d.message);
            $find(uniquenessSetup._controlIdInfo.ModalErrorId).show();
            //$("#UniquenessSetupDataValidationErrorMessageDiv").text(result.d.split("@&@")[1]);
        }
        else if (result.d.status == -1) {
            //Invoke Uniqueness Popup
            var pathWithoutCommonUI = RemoveLastDirectoryPartOf(path) + '/';
            UniquenessCheckApp.init(result.d.uniquenessFailurePopupInfo, onClickSecurityIdInSRMUniquenessPopup, onClickCloseSRMUniquenessPopup, 3, 'divErrorSMUSUniqueness', pathWithoutCommonUI);
            $('#divErrorSMUSUniqueness').css('display', '');
            $('#divErrorSMUSUniquenessOverlay').css('display', 'block');
        }
        else {
            //Successfully Create Message
            $("#" + uniquenessSetup._controlIdInfo.LblSuccessId).text("Key Created Successfully.");
            $find(uniquenessSetup._controlIdInfo.ModalSuccessId).show();

            //Fetch all keys again and reload ChainListView
            // Get data from Service Call
            var params = {};
            params.selectedSectypes = [];

            for (var item in uniquenessSetup._selectedSectypes) {
                params.selectedSectypes.push(uniquenessSetup._selectedSectypes[item].value);
            }
            CallCommonServiceMethod('GetUniqueKeysForSelectedSectypes', params, OnSuccess_LoadUniqueKeysDataLater, OnFailure, null, false);
        }
    }

    // This function is called on Success Service Call on Delete Key - CHANGED FINAL
    function OnSuccess_DeleteUniqueKey(result) {
        //Successfully Deleted Message
        $("#" + uniquenessSetup._controlIdInfo.LblSuccessId).text("Key Deleted Successfully.");
        $find(uniquenessSetup._controlIdInfo.ModalSuccessId).show();

        //Fetch all keys again and reload ChainListView
        // Get data from Service Call
        var params = {};
        params.selectedSectypes = [];

        for (var item in uniquenessSetup._selectedSectypes) {
            params.selectedSectypes.push(uniquenessSetup._selectedSectypes[item].value);
        }
        CallCommonServiceMethod('GetUniqueKeysForSelectedSectypes', params, OnSuccess_LoadUniqueKeysDataLater, OnFailure, null, false);
    }

    // This function is called on Success Service Call on Update Key - CHANGED FINAL
    function OnSuccess_UpdateUniqueKey(result) {
        // result.d.status 
        //  0 : Unique Key Validation failed
        // -1 : Data is NOT Unique
        //  1 : Key Successfully Inserted/Updated.
        //  2 : Key Successfully Validated (NOT Inserted/Updated) - NOT for UI

        if (result.d.status == 0) {
            //Display Error Msg
            $("#" + uniquenessSetup._controlIdInfo.LblErrorId).text(result.d.message);
            $find(uniquenessSetup._controlIdInfo.ModalErrorId).show();
            //$("#UniquenessSetupDataValidationErrorMessageDiv").text(result.d.split("@&@")[1]);
        }
        else if (result.d.status == -1) {
            //Invoke Uniqueness Popup
            var pathWithoutCommonUI = RemoveLastDirectoryPartOf(path) + '/';
            UniquenessCheckApp.init(result.d.uniquenessFailurePopupInfo, onClickSecurityIdInSRMUniquenessPopup, onClickCloseSRMUniquenessPopup, 3, 'divErrorSMUSUniqueness', pathWithoutCommonUI);
            $('#divErrorSMUSUniqueness').css('display', '');
            $('#divErrorSMUSUniquenessOverlay').css('display', 'block');
        }
        else {
            //Successfully Updated Message
            $("#" + uniquenessSetup._controlIdInfo.LblSuccessId).text("Key Updated Successfully.");
            $find(uniquenessSetup._controlIdInfo.ModalSuccessId).show();

            //Fetch all keys again and reload ChainListView
            // Get data from Service Call
            var params = {};
            params.selectedSectypes = [];

            for (var item in uniquenessSetup._selectedSectypes) {
                params.selectedSectypes.push(uniquenessSetup._selectedSectypes[item].value);
            }
            CallCommonServiceMethod('GetUniqueKeysForSelectedSectypes', params, OnSuccess_LoadUniqueKeysDataLater, OnFailure, null, false);
        }
    }

    //// This function is called on Success Service Call on Download All Unique Keys - CHANGED FINAL
    //function OnSuccess_DownloadAllUniqueKeys(result) {
    //    if (typeof result != typeof undefined && typeof result.d != typeof undefined) {
    //        if (parseInt(result.d.isSuccess.split("@&@")[0]) == 0) {
    //            //Display Error Msg
    //            $("#" + uniquenessSetup._controlIdInfo.LblErrorId).text(result.d.split("@&@")[1]);
    //            $find(uniquenessSetup._controlIdInfo.ModalErrorId).show();
    //        } else {
    //            var pathWithoutCommonUI = RemoveLastDirectoryPartOf(path);
    //            var fileName = "SecMaster Unique Keys " + (uniquenessSetup._securityInfo.InstanceName != typeof undefined ? uniquenessSetup._securityInfo.InstanceName : "");
    //            window.open(pathWithoutCommonUI + "/Download.aspx?name=" + fileName + "&guid=" + result.d.guidString);
    //        }
    //    }
    //}

    //This function is called if any Service Call returns an error - CHANGED FINAL
    function OnFailure(result) {
        $("#" + uniquenessSetup._controlIdInfo.LblErrorId).text("Service returned an error" + ((result.responseJSON == null || result.responseJSON.Message == null || result.responseJSON.Message.length == 0) ? "" : (" : " + result.responseJSON.Message)) + ((result.responseText == null || result.responseText.length == 0) ? "" : (" : " + result.responseText)));
        $find(uniquenessSetup._controlIdInfo.ModalErrorId).show();
    }


    ///////////////////////////
    // Event Based Functions //
    ///////////////////////////

    ////Function to Show Details in Right Div Based on Selection of a Chain in Left Div - To Be Removed
    //UniquenessSetup.prototype.onClickChain = function onClickChain(obj, event) {
    //    $("#UniquenessSetupDataValidationErrorMessageDiv").text("");
    //    //Handling for isSelected for color and arrow
    //    if (uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainKeyID != obj.chainKeyID) {
    //        uniquenessSetup._pageViewModelInstance.selectedChainInfo().isSelected(false);
    //    }
    //    uniquenessSetup._pageViewModelInstance.selectedChainInfo(obj);
    //    uniquenessSetup._pageViewModelInstance.selectedChainInfo().isSelected(true);
    //    //Decide whether Save Button or Update Button
    //    if (uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainKeyID == -1) {
    //        //Show Save & Hide Update
    //        $("#UniquenessSetupKeySaveButtonDiv").show();
    //        $("#UniquenessSetupKeyUpdateButtonDiv").hide();
    //    }
    //    else {
    //        //Show Update & Hide Save
    //        $("#UniquenessSetupKeyUpdateButtonDiv").show();
    //        $("#UniquenessSetupKeySaveButtonDiv").hide();
    //    }
    //    //Chain Key Name
    //    $("#keyNameValue").val(uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainKeyName());
    //    //Chain Sectype Multi Select
    //    var sectypesList = [];
    //    for (var item in uniquenessSetup._allSectypesInfo) {
    //        var tempObj1 = {};
    //        tempObj1.value = uniquenessSetup._allSectypesInfo[item].SectypeID;
    //        tempObj1.text = uniquenessSetup._allSectypesInfo[item].SectypeName;
    //        sectypesList.push(tempObj1);
    //    }
    //    $("#sectypesValueDiv").html("");
    //    ApplyMultiSelect('sectypesValueDivSelectedItems', false, sectypesList, true, '#sectypesValueDiv', 'Security Types', false, uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedSectypes);
    //    //Chain Level
    //    if (uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainIsMaster() == true) {
    //        $("#levelGroup_attribute").prop("checked", true).change();
    //    }
    //    else {
    //        $("#levelGroup_leg").prop("checked", true).change();
    //    }
    //    var selectedSectypes = [];
    //    for (var item in uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedSectypes) {
    //        selectedSectypes.push(uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedSectypes[item].value);
    //    }
    //    //Get Common Master Attributes For Selected Sectypes
    //    var params = {}
    //    params.userName = uniquenessSetup._securityInfo.UserName;
    //    params.selectedSectypes = selectedSectypes;
    //    CallCommonServiceMethod('GetCommonMasterAttributesForSelectedSectypes', params, OnSuccess_GetCommonMasterAttributesForSelectedSectypes, OnFailure, null, false);
    //    //Get Common Legs For Selected Sectypes
    //    var params1 = {};
    //    params1.selectedSectypes = selectedSectypes;
    //    CallCommonServiceMethod('GetCommonLegsForSelectedSectypes', params1, OnSuccess_GetCommonLegsForSelectedSectypes, OnFailure, null, false);
    //    ////Disable Click on Leg & Attribute Radio
    //    //$("#levelGroup_leg").attr("disabled", true);
    //    //$("#levelGroup_attribute").attr("disabled", true);
    //    $("#UniquenessSetupKeyDetailsInfoDiv_4_A").css("display", "none");
    //    $("#UniquenessSetupKeyDetailsInfoDiv_4_B").css("display", "none");
    //    //$("#UniquenessSetupMainRightColumn").css("display", "inline-block");
    //};

    //Function for Cancel Button in Each Key Div - CHANGED FINAL
    UniquenessSetup.prototype.onClickCancelButton = function onClickCancelButton(obj, event) {
        //Set chainIsEdit to false, which makes View Mode Visible and Hides Edit Mode.
        obj.chainIsEdit(false);
        obj.nullAsUnique(obj.nullAsUnique_DB);
        obj.checkInDrafts(obj.checkInDrafts_DB);
        obj.checkInWorkflows(obj.checkInWorkflows_DB);
    };

    //Function for Update Button in Each Key Div - CHANGED FINAL
    UniquenessSetup.prototype.onClickUpdateButton = function onClickUpdateButton(obj, event) {
        var errorMsg = "";
        var currentChainKeyID = obj.chainKeyID;
        var currentChainIsMaster = obj.chainIsMaster();

        var currentLevelText = currentChainIsMaster ? "AttributeLevel" : "LegLevel";

        var keyData = {};
        keyData.KeyID = currentChainKeyID;

        //Key Name
        if ($("#UniquenessSetup_" + currentLevelText + "_EditMode_Item_" + currentChainKeyID + "_Block_1_Info").val().trim() == "") {
            errorMsg = "Key Name can not be empty.";
        }
        else {
            keyData.KeyName = $("#UniquenessSetup_" + currentLevelText + "_EditMode_Item_" + currentChainKeyID + "_Block_1_Info").val().trim();
            ////Key Selected Sectypes
            //var target = $("#smselect_sectypesValueDiv_Chain_" + currentChainKeyID);
            //if (smselect.getSelectedOption(target).length == 0) {
            //    errorMsg = "Please select at least one Sectype.";
            //}
            //else {
            keyData.SelectedSectypes = [];
            for (var item in obj.chainSelectedSectypes) {
                keyData.SelectedSectypes.push(obj.chainSelectedSectypes[item].value);
            }
            //for (var item in smselect.getSelectedOption(target)) {
            //    keyData.SelectedSectypes.push(parseInt(smselect.getSelectedOption(target)[item].value));
            //}

            //Key Level
            keyData.IsMaster = currentChainIsMaster;
            if (currentChainIsMaster) {
                //Attribute Level
                var targetAttr = $("#smselect_attributeLevelValueDiv_Chain_" + currentChainKeyID);
                if (smselect.getSelectedOption(targetAttr).length == 0) {
                    errorMsg = "Please select at least one Attribute.";
                }
                else {
                    keyData.SelectedAttributes = [];
                    for (var item in smselect.getSelectedOption(targetAttr)) {
                        var tempObj = {};
                        tempObj.AttributeName = smselect.getSelectedOption(targetAttr)[item].text;
                        tempObj.AttributeIDs = smselect.getSelectedOption(targetAttr)[item].value.split("@&@")[0];
                        tempObj.AreAdditionalLegAttributes = smselect.getSelectedOption(targetAttr)[item].value.split("@&@")[1];
                        keyData.SelectedAttributes.push(tempObj);
                    }
                }
            }
            else {
                //Leg Level
                //Selected Leg
                keyData.SelectedLeg = {};
                keyData.SelectedLeg.LegName = obj.chainSelectedLeg.text;
                keyData.SelectedLeg.LegIDs = obj.chainSelectedLeg.value.split("@&@")[0];
                keyData.SelectedLeg.AreAdditionalLegs = obj.chainSelectedLeg.value.split("@&@")[1];
                //keyData.SelectedLeg = obj.chainSelectedLeg;

                //Leg Attributes
                var targetLegAttr = $("#smselect_legLevelAttributeValueDiv_Chain_" + currentChainKeyID);
                if (smselect.getSelectedOption(targetLegAttr).length == 0) {
                    errorMsg = "Please select at least one Leg Attribute.";
                }
                else {
                    keyData.SelectedLegAttributes = [];
                    for (var item in smselect.getSelectedOption(targetLegAttr)) {
                        var tempObj = {};
                        tempObj.AttributeName = smselect.getSelectedOption(targetLegAttr)[item].text;
                        tempObj.AttributeIDs = smselect.getSelectedOption(targetLegAttr)[item].value.split("@&@")[0];
                        tempObj.AreAdditionalLegAttributes = smselect.getSelectedOption(targetLegAttr)[item].value.split("@&@")[1];
                        keyData.SelectedLegAttributes.push(tempObj);
                    }

                    //Across Securities or Within Security
                    if ($("#UniquenessSetup_LegLevel_EditMode_Item_" + currentChainKeyID + "_Block_5_Info_CU_Across").hasClass("ToggleBlueSliderButtonCSSClass")) {
                        keyData.IsAcrossSecurities = true;
                    }
                    else {
                        keyData.IsAcrossSecurities = false;
                    }
                }

            }
            //}
        }

        keyData.CheckInDrafts = obj.checkInDrafts();
        keyData.CheckInWorkflows = obj.checkInWorkflows();
        //null as unique
        //Across Securities or Within Security
        if ($("#UniquenessSetup_LegLevel_EditMode_Item_" + currentChainKeyID + "_Block_56_NullAsUniqueNo").hasClass("ToggleBlueSliderButtonCSSClass")) {
            keyData.NullAsUnique = false;
        }
        else {
            keyData.NullAsUnique = true;
        }



        //Display Validation Error Msg, if any. If not, save the key.
        if (errorMsg == "") {
            // Get data from Service Call
            var params = {};
            params.userName = uniquenessSetup._securityInfo.UserName;
            params.InputObject = keyData;

            CallCommonServiceMethod('UpdateUniqueKey', params, OnSuccess_UpdateUniqueKey, OnFailure, null, false);
        }
        else {
            $("#" + uniquenessSetup._controlIdInfo.LblErrorId).text(errorMsg);
            $find(uniquenessSetup._controlIdInfo.ModalErrorId).show();
        }
    };

    //Function for Edit Button in Each Key Div- CHANGED FINAL
    UniquenessSetup.prototype.onClickEditButton = function onClickEditButton(obj, event) {

        var currentChainKeyID = obj.chainKeyID;
        //Set chainIsEdit to true, which makes Edit Mode Visible and Hides the View Mode.
        obj.chainIsEdit(true);

        //For Sectype Multi Select
        //var sectypesList = [];
        //for (var item in uniquenessSetup._allSectypesInfo) {
        //    var tempObj1 = {};
        //    tempObj1.value = uniquenessSetup._allSectypesInfo[item].SectypeID;
        //    tempObj1.text = uniquenessSetup._allSectypesInfo[item].SectypeName;
        //    sectypesList.push(tempObj1);
        //}

        //Bind Drop Down Lists
        if (obj.chainIsMaster()) {
            //Bind SecTypes DDL
            //$('#UniquenessSetup_AttributeLevel_EditMode_Item_' + currentChainKeyID + '_Block_2_Info').html("");
            //ApplyMultiSelect('sectypesValueDiv_Chain_' + currentChainKeyID, false, sectypesList, true, '#UniquenessSetup_AttributeLevel_EditMode_Item_' + currentChainKeyID + '_Block_2_Info', 'Security Types', false, obj.chainSelectedSectypes);

            //Get Data for Binding Master Attributes DDL
            var selectedSectypes = [];

            for (var item in obj.chainSelectedSectypes) {
                selectedSectypes.push(obj.chainSelectedSectypes[item].value);
            }

            var params = {}
            params.userName = uniquenessSetup._securityInfo.UserName;
            params.selectedSectypes = selectedSectypes;
            params.KeyID = currentChainKeyID;
            CallCommonServiceMethod('GetCommonMasterAttributesForSelectedSectypes', params, OnSuccess_GetCommonMasterAttributesForSelectedSectypes, OnFailure, null, false);
        }
        else {
            //Bind SecTypes DDL
            //$('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_2_Info').html("");
            //ApplyMultiSelect('sectypesValueDiv_Chain_' + currentChainKeyID, false, sectypesList, true, '#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_2_Info', 'Security Types', false, obj.chainSelectedSectypes);

            //Show Leg Name - Commented coz binded using KnockoutJS
            //$('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_3_Info').text(obj.chainSelectedLegDisplayText);

            //Get Data for Binding Leg Attributes DDL
            var selectedLeg = {};
            selectedLeg.LegName = obj.chainSelectedLeg.text;
            selectedLeg.LegIDs = obj.chainSelectedLeg.value.split("@&@")[0];
            selectedLeg.AreAdditionalLegs = obj.chainSelectedLeg.value.split("@&@")[1];

            var params = {};
            params.InputObject = selectedLeg;
            params.KeyID = currentChainKeyID;
            CallCommonServiceMethod('GetCommonLegAttributesForSelectedLegName', params, OnSuccess_GetCommonLegAttributesForSelectedLegName, OnFailure, null, false);


            //Set Check Uniquness Correctly
            if ($('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_5_Info_CU_Within').hasClass("ToggleBlueSliderButtonCSSClass")) {
                $('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_5_Info_CU_Within').removeClass("ToggleBlueSliderButtonCSSClass");
            }
            if ($('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_5_Info_CU_Across').hasClass("ToggleBlueSliderButtonCSSClass")) {
                $('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_5_Info_CU_Across').removeClass("ToggleBlueSliderButtonCSSClass");
            }

            if (obj.chainIsAcrossSecurities()) {
                //Change to Across Securities
                $('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_5_Info_CU_Across').addClass("ToggleBlueSliderButtonCSSClass");
                $("#Sm_DraftCheckBoxInput" + currentChainKeyID + ",#Sm_WorkflowCheckBoxInput" + currentChainKeyID).prop('disabled', false);
            }
            else {
                //Change to Within Security
                $('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_5_Info_CU_Within').addClass("ToggleBlueSliderButtonCSSClass");
                $("#Sm_DraftCheckBoxInput" + currentChainKeyID + ",#Sm_WorkflowCheckBoxInput" + currentChainKeyID).prop('disabled', true);
            }
        }

        //unsetting
        $('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_56_NullAsUniqueYes').removeClass("ToggleBlueSliderButtonCSSClass");
        $('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_56_NullAsUniqueNo').removeClass("ToggleBlueSliderButtonCSSClass");

        //null for unique data
        if(obj.nullAsUnique())
            $('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_56_NullAsUniqueYes').addClass("ToggleBlueSliderButtonCSSClass");
        else
            $('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_56_NullAsUniqueNo').addClass("ToggleBlueSliderButtonCSSClass");
    };

    //Function for Delete Button in Each Key Div - CHANGED FINAL
    UniquenessSetup.prototype.onClickDeleteButton = function onClickDeleteButton(obj, event) {
        //Extract Current Chain Info
        var currentChainKeyID = obj.chainKeyID;

        //
        //$("#" + uniquenessSetup._controlIdInfo.LblSuccessId).text("Key Created Successfully.");
        //$find(uniquenessSetup._controlIdInfo.ModalSuccessId).show();


        //Show Delete Popup
        $find(uniquenessSetup._controlIdInfo.ModalDeleteId).show();
        //$find(uniquenessSetup._controlIdInfo.ModalDeleteId).css('left', ($(window).width() - $("#" + uniquenessSetup._controlIdInfo.ModalDeleteId).width()) / 2);

        //var yesID = uniquenessSetup._controlIdInfo.BtnDeleteYes;
        //var noID = uniquenessSetup._controlIdInfo.BtnDeleteNo;

        uniquenessSetup._keyIdToBeDeleted = obj.chainKeyID;

        $("#" + uniquenessSetup._controlIdInfo.BtnDeleteYes).off().on('click', function (obj, event) {

            var chainKeyID = uniquenessSetup._keyIdToBeDeleted;


            // Get data from Service Call
            var params = {};
            params.userName = uniquenessSetup._securityInfo.UserName;
            params.keyID = chainKeyID;

            CallCommonServiceMethod('DeleteUniqueKey', params, OnSuccess_DeleteUniqueKey, OnFailure, null, false);

            $find(uniquenessSetup._controlIdInfo.ModalDeleteId).hide();

            return false;
        });

        $("#" + uniquenessSetup._controlIdInfo.BtnDeleteNo).off().on('click', function () {
            $find(uniquenessSetup._controlIdInfo.ModalDeleteId).hide();
            return false;
        })

        //// Get data from Service Call
        //var params = {};
        //params.userName = uniquenessSetup._securityInfo.UserName;
        //params.keyID = currentChainKeyID;
        //CallCommonServiceMethod('DeleteUniqueKey', params, OnSuccess_DeleteUniqueKey, OnFailure, null, false);

    };

    //Function for Within/Across Slider Button in Each Leg Level Div - CHANGED FINAL
    UniquenessSetup.prototype.onClickToggleCheckUniqueness = function onClickToggleCheckUniqueness(obj, event) {

        var currentChainKeyID = obj.chainKeyID;

        if ($('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_5_Info_CU_Within').hasClass("ToggleBlueSliderButtonCSSClass")) {
            //Change to Across Securities
            $('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_5_Info_CU_Within').removeClass("ToggleBlueSliderButtonCSSClass");
            $('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_5_Info_CU_Across').addClass("ToggleBlueSliderButtonCSSClass");
            //$("#Sm_DraftCheckBoxInput" + currentChainKeyID + ",#Sm_WorkflowCheckBoxInput" + currentChainKeyID).prop('checked', false);
            $("#Sm_DraftCheckBoxInput" + currentChainKeyID + ",#Sm_WorkflowCheckBoxInput" + currentChainKeyID).prop('disabled', false);
            obj.checkInDrafts(false);
            obj.checkInWorkflows(false);
        }
        else {
            //Change to Within Security
            $('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_5_Info_CU_Across').removeClass("ToggleBlueSliderButtonCSSClass");
            $('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_5_Info_CU_Within').addClass("ToggleBlueSliderButtonCSSClass");
            //$("#Sm_DraftCheckBoxInput" + currentChainKeyID + ",#Sm_WorkflowCheckBoxInput" + currentChainKeyID).prop('checked', false);
            $("#Sm_DraftCheckBoxInput" + currentChainKeyID + ",#Sm_WorkflowCheckBoxInput" + currentChainKeyID).prop('disabled', true);
            obj.checkInDrafts(false);
            obj.checkInWorkflows(false);

        }
    };

    UniquenessSetup.prototype.onClickToggleNullAsUnique = function onClickToggleNullAsUnique(obj, event) {

        var currentChainKeyID = obj.chainKeyID;

        if ($('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_56_NullAsUniqueYes').hasClass("ToggleBlueSliderButtonCSSClass")) {
            //Change to Across Securities
            $('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_56_NullAsUniqueYes').removeClass("ToggleBlueSliderButtonCSSClass");
            $('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_56_NullAsUniqueNo').addClass("ToggleBlueSliderButtonCSSClass");
        }
        else {
            //Change to Within Security
            $('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_56_NullAsUniqueNo').removeClass("ToggleBlueSliderButtonCSSClass");
            $('#UniquenessSetup_LegLevel_EditMode_Item_' + currentChainKeyID + '_Block_56_NullAsUniqueYes').addClass("ToggleBlueSliderButtonCSSClass");
        }
    };

    //NEW Key Functions
    //Function for Leg/Attribute Slider Button in New Key - CHANGED FINAL
    $("#UniquenessSetup_NK_Level_Slide_Div").click(function () {
        if ($('#UniquenessSetup_NK_Level_Attribute').hasClass("ToggleBlueSliderButtonCSSClass")) {
            //Change to Leg Level
            $('#UniquenessSetup_NK_Level_Attribute').removeClass("ToggleBlueSliderButtonCSSClass");
            $('#UniquenessSetup_NK_Level_Leg').addClass("ToggleBlueSliderButtonCSSClass");

            //HIDE OR SHOW DIVS ON RIGHT
            $("#UniquenessSetup_NK_Block_3_A").css("display", "none");

            $("#UniquenessSetup_NK_Block_3_B").css("display", "block");
            $("#UniquenessSetup_NK_Block_4_B").css("display", "block");
            $("#UniquenessSetup_NK_Block_5_B").css("display", "block");
            $("#Sm_DraftCheckBoxInput,#Sm_WorkflowCheckBoxInput").prop('checked', false);
            if ($("#UniquenessSetup_NK_CU_Within").hasClass("ToggleBlueSliderButtonCSSClass"))
                $("#Sm_DraftCheckBoxInput,#Sm_WorkflowCheckBoxInput").prop('disabled', true);
            else
                $("#Sm_DraftCheckBoxInput,#Sm_WorkflowCheckBoxInput").prop('disabled', false);
        }
        else {
            //Change to Attribute Level
            $('#UniquenessSetup_NK_Level_Leg').removeClass("ToggleBlueSliderButtonCSSClass");
            $('#UniquenessSetup_NK_Level_Attribute').addClass("ToggleBlueSliderButtonCSSClass");

            //HIDE OR SHOW DIVS ON RIGHT
            $("#UniquenessSetup_NK_Block_3_A").css("display", "block");

            $("#UniquenessSetup_NK_Block_3_B").css("display", "none");
            $("#UniquenessSetup_NK_Block_4_B").css("display", "none");
            $("#UniquenessSetup_NK_Block_5_B").css("display", "none");

            $("#Sm_DraftCheckBoxInput,#Sm_WorkflowCheckBoxInput").prop('disabled', false);
            $("#Sm_DraftCheckBoxInput,#Sm_WorkflowCheckBoxInput").prop('checked', false);
        }
    });

    //Function for Within/Across Slider Button in New Key - CHANGED FINAL
    $("#UniquenessSetup_NK_Block_5_Info").click(function () {
        if ($('#UniquenessSetup_NK_CU_Within').hasClass("ToggleBlueSliderButtonCSSClass")) {
            //Change to Across Securities
            $('#UniquenessSetup_NK_CU_Within').removeClass("ToggleBlueSliderButtonCSSClass");
            $('#UniquenessSetup_NK_CU_Across').addClass("ToggleBlueSliderButtonCSSClass");
            $("#Sm_DraftCheckBoxInput,#Sm_WorkflowCheckBoxInput").prop('checked', false);
            $("#Sm_DraftCheckBoxInput,#Sm_WorkflowCheckBoxInput").prop('disabled', false);
        }
        else {
            //Change to Within Security
            $('#UniquenessSetup_NK_CU_Across').removeClass("ToggleBlueSliderButtonCSSClass");
            $('#UniquenessSetup_NK_CU_Within').addClass("ToggleBlueSliderButtonCSSClass");
            $("#Sm_DraftCheckBoxInput,#Sm_WorkflowCheckBoxInput").prop('checked', false);
            $("#Sm_DraftCheckBoxInput,#Sm_WorkflowCheckBoxInput").prop('disabled', true);
        }
    });

    $("#UniquenessSetup_NK_Block_56_Info").click(function () {
        if ($('#UniquenessSetup_NK_NullAsUniqueYes').hasClass("ToggleBlueSliderButtonCSSClass")) {
            //Change to Across Securities
            $('#UniquenessSetup_NK_NullAsUniqueYes').removeClass("ToggleBlueSliderButtonCSSClass");
            $('#UniquenessSetup_NK_NullAsUniqueNo').addClass("ToggleBlueSliderButtonCSSClass");
        }
        else {
            //Change to Within Security
            $('#UniquenessSetup_NK_NullAsUniqueNo').removeClass("ToggleBlueSliderButtonCSSClass");
            $('#UniquenessSetup_NK_NullAsUniqueYes').addClass("ToggleBlueSliderButtonCSSClass");
        }
    });

    //Function for Save Button in New Key - CHANGED FINAL
    $("#UniquenessSetup_NK_Save_Button").click(function () {
        var errorMsg = "";

        var keyData = {};
        keyData.KeyID = -1;

        //Key Name
        if ($("#UniquenessSetup_NK_KeyNameValue").val().trim() == "") {
            errorMsg = "Key Name can not be empty.";
        }
        else {
            keyData.KeyName = $("#UniquenessSetup_NK_KeyNameValue").val().trim();

            //Key Selected Sectypes
            var target = $("#smselect_sectypesValueDiv_NK");
            if (smselect.getSelectedOption(target).length == 0) {
                errorMsg = "Please select at least one Sectype.";
            }
            else {
                keyData.SelectedSectypes = [];
                for (var item in smselect.getSelectedOption(target)) {
                    keyData.SelectedSectypes.push(parseInt(smselect.getSelectedOption(target)[item].value));
                }

                //Key Level
                if ($('#UniquenessSetup_NK_Level_Attribute').hasClass("ToggleBlueSliderButtonCSSClass")) {
                    //Attributes Level
                    keyData.IsMaster = true;

                    var targetAttr = $("#smselect_attributeLevelValueDiv_NK");
                    if (smselect.getSelectedOption(targetAttr).length == 0) {
                        errorMsg = "Please select at least one Attribute.";
                    }
                    else {
                        keyData.SelectedAttributes = [];
                        for (var item in smselect.getSelectedOption(targetAttr)) {
                            var tempObj = {};
                            tempObj.AttributeName = smselect.getSelectedOption(targetAttr)[item].text;
                            tempObj.AttributeIDs = smselect.getSelectedOption(targetAttr)[item].value.split("@&@")[0];
                            tempObj.AreAdditionalLegAttributes = smselect.getSelectedOption(targetAttr)[item].value.split("@&@")[1];
                            keyData.SelectedAttributes.push(tempObj);
                        }
                    }
                }
                else {
                    //Leg Level
                    keyData.IsMaster = false;

                    var targetLeg = $("#smselect_legLevelValueDiv_NK");
                    if (smselect.getSelectedOption(targetLeg).length == 0 || smselect.getSelectedOption(targetLeg)[0].value.split("@&@")[0] == "-1") {
                        // Since, its Single Select, there wont be a case that length is zero. Still added for safe side. The second option, where LegIDs is "-1" will occur when the option selected is "No Common Legs on selected Security Types"
                        errorMsg = "Please select a Leg.";
                    }
                    else {
                        keyData.SelectedLeg = {};
                        keyData.SelectedLeg.LegName = smselect.getSelectedOption(targetLeg)[0].text;
                        keyData.SelectedLeg.LegIDs = smselect.getSelectedOption(targetLeg)[0].value.split("@&@")[0];
                        keyData.SelectedLeg.AreAdditionalLegs = smselect.getSelectedOption(targetLeg)[0].value.split("@&@")[1];

                        //Leg Attributes
                        var targetLegAttr = $("#smselect_legLevelAttributeValueDiv_NK");
                        if (smselect.getSelectedOption(targetLegAttr).length == 0 || smselect.getSelectedOption(targetLegAttr)[0].value.split("@&@")[0] == "-1") {
                            //The second option can occur if we select a leg whose name is same in 2 diff sectypes but there are no common attributes in those 2 legs.
                            errorMsg = "Please select at least one Leg Attribute.";
                        }
                        else {
                            keyData.SelectedLegAttributes = [];
                            for (var item in smselect.getSelectedOption(targetLegAttr)) {
                                var tempObj = {};
                                tempObj.AttributeName = smselect.getSelectedOption(targetLegAttr)[item].text;
                                tempObj.AttributeIDs = smselect.getSelectedOption(targetLegAttr)[item].value.split("@&@")[0];
                                tempObj.AreAdditionalLegAttributes = smselect.getSelectedOption(targetLegAttr)[item].value.split("@&@")[1];
                                keyData.SelectedLegAttributes.push(tempObj);
                            }

                            //Across Securities or Within Security
                            if ($("#UniquenessSetup_NK_CU_Across").hasClass("ToggleBlueSliderButtonCSSClass")) {
                                keyData.IsAcrossSecurities = true;
                            }
                            else {
                                keyData.IsAcrossSecurities = false;
                            }
                        }
                    }
                }
            }
        }

        //drafts/workflow changes
        keyData.CheckInDrafts = $("#Sm_DraftCheckBoxInput").prop('checked');
        keyData.CheckInWorkflows = $("#Sm_WorkflowCheckBoxInput").prop('checked');

        if ($("#UniquenessSetup_NK_NullAsUniqueNo").hasClass("ToggleBlueSliderButtonCSSClass")) {
            keyData.NullAsUnique = false;
        }
        else {
            keyData.NullAsUnique = true;
        }

        //Display Validation Error Msg, if any. If not, save the key.
        if (errorMsg == "") {
            // Get data from Service Call
            var params = {};
            params.userName = uniquenessSetup._securityInfo.UserName;
            params.InputObject = keyData;

            CallCommonServiceMethod('CreateUniqueKey', params, OnSuccess_CreateUniqueKey, OnFailure, null, false);
        }
        else {
            $("#" + uniquenessSetup._controlIdInfo.LblErrorId).text(errorMsg);
            $find(uniquenessSetup._controlIdInfo.ModalErrorId).show();
        }
    });

    //Function for Cancel Button in New Key - CHANGED FINAL
    $("#UniquenessSetup_NK_Cancel_Button").click(function () {
        //Hide the New Key Div
        $("#UniquenessSetupMainNewKeyDiv").css("display", "none");
    });

    //Function for onClick on Add New Key Label - CHANGED FINAL
    $("#UniquenessSetupMainAddKeyLabel").click(function () {

        //Set Key Name to Empty
        $("#UniquenessSetup_NK_KeyNameValue").val("");

        //Select Attribute by Default
        if ($('#UniquenessSetup_NK_Level_Leg').hasClass("ToggleBlueSliderButtonCSSClass")) {
            //Change to Attribute Level
            $('#UniquenessSetup_NK_Level_Leg').removeClass("ToggleBlueSliderButtonCSSClass");
            $('#UniquenessSetup_NK_Level_Attribute').addClass("ToggleBlueSliderButtonCSSClass");

            //HIDE OR SHOW DIVS ON RIGHT
            $("#UniquenessSetup_NK_Block_3_A").css("display", "block");

            $("#UniquenessSetup_NK_Block_3_B").css("display", "none");
            $("#UniquenessSetup_NK_Block_4_B").css("display", "none");
            $("#UniquenessSetup_NK_Block_5_B").css("display", "none");
        }

        //Select Within By Default
        if ($('#UniquenessSetup_NK_CU_Across').hasClass("ToggleBlueSliderButtonCSSClass")) {
            //Change to Within Security
            $('#UniquenessSetup_NK_CU_Across').removeClass("ToggleBlueSliderButtonCSSClass");
            $('#UniquenessSetup_NK_CU_Within').addClass("ToggleBlueSliderButtonCSSClass");
        }

        //For SecType Multi Select
        var sectypesList = [];
        for (var item in uniquenessSetup._allSectypesInfo) {
            var tempObj1 = {};
            tempObj1.value = uniquenessSetup._allSectypesInfo[item].SectypeID;
            tempObj1.text = uniquenessSetup._allSectypesInfo[item].SectypeName;

            sectypesList.push(tempObj1);
        }

        $("#UniquenessSetup_NK_Block_2_Info").html("");
        ApplyMultiSelect('sectypesValueDiv_NK', false, sectypesList, true, '#UniquenessSetup_NK_Block_2_Info', 'Security Types', false);

        //For Attributes Multi Select
        var masterAttributesList = [{ text: "Please select a sectype", value: "-1" + "@&@" + "-1" }];
        $("#UniquenessSetup_NK_Block_3_A_Info").html("");
        ApplyMultiSelect('attributeLevelValueDiv_NK', false, masterAttributesList, true, '#UniquenessSetup_NK_Block_3_A_Info', 'Attributes', false);

        //For Legs Multi Select
        var legsList = [{ text: "Please select a sectype", value: "-1" }];
        $("#UniquenessSetup_NK_Block_3_B_Info").html("");
        ApplySMSelect('legLevelValueDiv_NK', false, legsList, true, '#UniquenessSetup_NK_Block_3_B_Info');

        //For Leg Attributes Multi Select
        var legAttributesList = [{ text: "Please select a leg", value: "-1" + "@&@" + "-1" }];
        $("#UniquenessSetup_NK_Block_4_Info").html("");
        ApplyMultiSelect('legLevelAttributeValueDiv_NK', false, legAttributesList, true, '#UniquenessSetup_NK_Block_4_Info', 'Leg Attributes', false);

        //drafts/workflow checkbox
        $("#Sm_DraftCheckBoxInput").prop('checked', false);
        $("#Sm_WorkflowCheckBoxInput").prop('checked', false);

        //null as unique data : select no as default
        if ($('#UniquenessSetup_NK_NullAsUniqueYes').hasClass("ToggleBlueSliderButtonCSSClass")) {
            //Change to Within Security
            $('#UniquenessSetup_NK_NullAsUniqueYes').removeClass("ToggleBlueSliderButtonCSSClass");
            $('#UniquenessSetup_NK_NullAsUniqueNo').addClass("ToggleBlueSliderButtonCSSClass");
        }

        //Show the New Key Div
        $("#UniquenessSetupMainNewKeyDiv").css("display", "block");
    });

    //function OnClick_CreateNewUniqueKey() {
    //    var data = {}
    //    data.KeyID = -1;
    //    data.KeyName = 'Untitled';
    //    data.SelectedSectypes = [];
    //    data.IsMaster = true;
    //    data.SelectedAttributes = [];
    //    data.SelectedLeg = {};
    //    data.SelectedLegAttributes = [];
    //    var newKey = new chainViewModel(data);
    //    uniquenessSetup._pageViewModelInstance.chainListing.unshift(newKey);
    //    //Handling for isSelected for color
    //    uniquenessSetup._pageViewModelInstance.selectedChainInfo().isSelected(false);
    //    uniquenessSetup._pageViewModelInstance.selectedChainInfo(newKey);
    //    $('.UniquenessSetupChain')[0].click();
    //}

    //$("#createNewKeyButtonDiv").on('click', function () {
    //    OnClick_CreateNewUniqueKey();
    //    // First Chain Item is selected on load.
    //    //$($('.UniquenessSetupChain')[0]).children('.UniquenessSetupChainInfo').click();
    //    //$('.UniquenessSetupChain')[0].click();
    //});

    //$('input[type=radio][name=levelGroup]').change(function () {
    //    if (this.value == 'attribute') {
    //        $("#UniquenessSetupKeyDetailsInfoDiv_4_A").css("display", "block");
    //        $("#UniquenessSetupKeyDetailsInfoDiv_4_B").css("display", "none");
    //    }
    //    if (this.value == 'leg') {
    //        $("#UniquenessSetupKeyDetailsInfoDiv_4_A").css("display", "none");
    //        $("#UniquenessSetupKeyDetailsInfoDiv_4_B").css("display", "block");
    //    }
    //});

    //$("#UniquenessSetupKeySaveButtonDiv").on('click', function () {
    //    var errorMsg = "";
    //    var keyData = {};
    //    keyData.KeyID = -1;
    //    //Key Name
    //    if ($("#keyNameValue").val().trim() == "") {
    //        errorMsg = "Key Name can not be empty.";
    //    }
    //    else {
    //        keyData.KeyName = $("#keyNameValue").val();
    //        //Key Selected Sectypes
    //        if (uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedSectypes.length == 0) {
    //            errorMsg = "Please select at least one Sectype.";
    //        }
    //        else {
    //            keyData.SelectedSectypes = [];
    //            for (var item in uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedSectypes) {
    //                keyData.SelectedSectypes.push(parseInt(uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedSectypes[item].value));
    //            }
    //            //Key Level
    //            if (!$('#levelGroup_attribute').prop('checked') && !$('#levelGroup_leg').prop('checked')) {
    //                errorMsg = "Please select one of the Levels.";
    //            }
    //            else {
    //                keyData.IsMaster = $('#levelGroup_attribute').prop('checked') ? true : false;
    //                //Selected Attributes or Selected Leg Details
    //                if (keyData.IsMaster) {         //Master Case
    //                    //Master Attributes
    //                    if (uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedAttributes.length == 0) {
    //                        errorMsg = "Please select at least one Attribute.";
    //                    }
    //                    else {
    //                        //keyData.SelectedAttributes = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedAttributes;
    //                        keyData.SelectedAttributes = [];
    //                        for (var item in uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedAttributes) {
    //                            var tempObj = {};
    //                            tempObj.AttributeName = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedAttributes[item].text;
    //                            tempObj.AttributeIDs = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedAttributes[item].value.split("@&@")[0];
    //                            tempObj.AreAdditionalLegAttributes = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedAttributes[item].value.split("@&@")[1];
    //                            keyData.SelectedAttributes.push(tempObj);
    //                        }
    //                    }
    //                }
    //                else {                          //Leg Case
    //                    //Leg
    //                    if (typeof uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLeg.text == typeof undefined) {
    //                        errorMsg = "Please select a Leg.";
    //                    }
    //                    else {
    //                        keyData.SelectedLeg = {};
    //                        keyData.SelectedLeg.LegName = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLeg.text;
    //                        keyData.SelectedLeg.LegIDs = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLeg.value.split("@&@")[0];
    //                        keyData.SelectedLeg.AreAdditionalLegs = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLeg.value.split("@&@")[1];
    //                        //Leg Attributes
    //                        if (uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLegAttributes.length == 0) {
    //                            errorMsg = "Please select at least one Leg Attribute.";
    //                        }
    //                        else {
    //                            //keyData.SelectedLegAttributes = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLegAttributes;
    //                            keyData.SelectedLegAttributes = [];
    //                            for (var item in uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLegAttributes) {
    //                                var tempObj = {};
    //                                tempObj.AttributeName = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLegAttributes[item].text;
    //                                tempObj.AttributeIDs = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLegAttributes[item].value.split("@&@")[0];
    //                                tempObj.AreAdditionalLegAttributes = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLegAttributes[item].value.split("@&@")[1];
    //                                keyData.SelectedLegAttributes.push(tempObj);
    //                            }
    //                        }
    //                        if (errorMsg == "") {
    //                            //Across Securities or Within Security
    //                            if (!$('#acrossSecGroup_withinSecurity').prop('checked') && !$('#acrossSecGroup_acrossSecurities').prop('checked')) {
    //                                errorMsg = "Please select either Across Securities or Within Security.";
    //                            }
    //                            else {
    //                                keyData.IsAcrossSecurities = $('#acrossSecGroup_acrossSecurities').prop('checked') ? true : false;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    if (errorMsg == "") {
    //        $("#UniquenessSetupDataValidationErrorMessageDiv").text(errorMsg);
    //        // Get data from Service Call
    //        var params = {};
    //        params.userName = uniquenessSetup._securityInfo.UserName;
    //        params.InputObject = keyData;
    //        CallCommonServiceMethod('CreateUniqueKey', params, OnSuccess_CreateUniqueKey, OnFailure, null, false);
    //    }
    //    else {
    //        $("#UniquenessSetupDataValidationErrorMessageDiv").text(errorMsg);
    //    }
    //});

    //$("#UniquenessSetupKeyUpdateButtonDiv").on('click', function () {
    //    var errorMsg = "";
    //    var keyData = {};
    //    keyData.KeyID = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainKeyID;
    //    //Key Name
    //    if ($("#keyNameValue").val().trim() == "") {
    //        errorMsg = "Key Name can not be empty.";
    //    }
    //    else {
    //        keyData.KeyName = $("#keyNameValue").val();
    //        //Key Selected Sectypes
    //        if (uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedSectypes.length == 0) {
    //            errorMsg = "Please select at least one Sectype.";
    //        }
    //        else {
    //            keyData.SelectedSectypes = [];
    //            for (var item in uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedSectypes) {
    //                keyData.SelectedSectypes.push(parseInt(uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedSectypes[item].value));
    //            }
    //            //Key Level
    //            if (!$('#levelGroup_attribute').prop('checked') && !$('#levelGroup_leg').prop('checked')) {
    //                errorMsg = "Please select one of the Levels.";
    //            }
    //            else {
    //                keyData.IsMaster = $('#levelGroup_attribute').prop('checked') ? true : false;
    //                //Selected Attributes or Selected Leg Details
    //                if (keyData.IsMaster) {         //Master Case
    //                    //Master Attributes
    //                    if (uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedAttributes.length == 0) {
    //                        errorMsg = "Please select at least one Attribute.";
    //                    }
    //                    else {
    //                        //keyData.SelectedAttributes = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedAttributes;
    //                        keyData.SelectedAttributes = [];
    //                        for (var item in uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedAttributes) {
    //                            var tempObj = {};
    //                            tempObj.AttributeName = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedAttributes[item].text;
    //                            tempObj.AttributeIDs = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedAttributes[item].value.split("@&@")[0];
    //                            tempObj.AreAdditionalLegAttributes = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedAttributes[item].value.split("@&@")[1];
    //                            keyData.SelectedAttributes.push(tempObj);
    //                        }
    //                    }
    //                }
    //                else {                          //Leg Case
    //                    //Leg
    //                    if (typeof uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLeg.text == typeof undefined) {
    //                        errorMsg = "Please select a Leg.";
    //                    }
    //                    else {
    //                        keyData.SelectedLeg = {};
    //                        keyData.SelectedLeg.LegName = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLeg.text;
    //                        keyData.SelectedLeg.LegIDs = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLeg.value.split("@&@")[0];
    //                        keyData.SelectedLeg.AreAdditionalLegs = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLeg.value.split("@&@")[1];
    //                        //Leg Attributes
    //                        if (uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLegAttributes.length == 0) {
    //                            errorMsg = "Please select at least one Leg Attribute.";
    //                        }
    //                        else {
    //                            //keyData.SelectedLegAttributes = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLegAttributes;
    //                            keyData.SelectedLegAttributes = [];
    //                            for (var item in uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLegAttributes) {
    //                                var tempObj = {};
    //                                tempObj.AttributeName = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLegAttributes[item].text;
    //                                tempObj.AttributeIDs = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLegAttributes[item].value.split("@&@")[0];
    //                                tempObj.AreAdditionalLegAttributes = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLegAttributes[item].value.split("@&@")[1];
    //                                keyData.SelectedLegAttributes.push(tempObj);
    //                            }
    //                        }
    //                        if (errorMsg == "") {
    //                            //Across Securities or Within Security
    //                            if (!$('#acrossSecGroup_withinSecurity').prop('checked') && !$('#acrossSecGroup_acrossSecurities').prop('checked')) {
    //                                errorMsg = "Please select either Across Securities or Within Security.";
    //                            }
    //                            else {
    //                                keyData.IsAcrossSecurities = $('#acrossSecGroup_acrossSecurities').prop('checked') ? true : false;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
    //    if (errorMsg == "") {
    //        $("#UniquenessSetupDataValidationErrorMessageDiv").text(errorMsg);
    //        // Get data from Service Call
    //        var params = {};
    //        params.userName = uniquenessSetup._securityInfo.UserName;
    //        params.InputObject = keyData;
    //        CallCommonServiceMethod('UpdateUniqueKey', params, OnSuccess_UpdateUniqueKey, OnFailure, null, false);
    //    }
    //    else {
    //        $("#UniquenessSetupDataValidationErrorMessageDiv").text(errorMsg);
    //    }
    //});

    //$("#UniquenessSetupKeyDeleteButton").on('click', function () {
    //    //Delete after unable to save New
    //    $("#UniquenessSetupDataValidationErrorMessageDiv").text("");
    //    var chainKeyID = uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainKeyID;
    //    if (chainKeyID != -1) {
    //        // Get data from Service Call
    //        var params = {};
    //        params.userName = uniquenessSetup._securityInfo.UserName;
    //        params.keyID = chainKeyID;
    //        CallCommonServiceMethod('DeleteUniqueKey', params, OnSuccess_DeleteUniqueKey, OnFailure, null, false);
    //    }
    //    else {
    //        //Fetch all keys again and reload ChainListView
    //        // Get data from Service Call
    //        var params = {};
    //        params.selectedSectypes = [];
    //        for (var item in uniquenessSetup._selectedSectypes) {
    //            params.selectedSectypes.push(uniquenessSetup._selectedSectypes[item].value);
    //        }
    //        CallCommonServiceMethod('GetUniqueKeysForSelectedSectypes', params, OnSuccess_LoadUniqueKeysDataLater, OnFailure, null, false);
    //    }
    //});

    //Search on Key Names - CHANGED FINAL
    $("#keyNameSearch #gridSearch").on('keyup', function () {

        //Search String
        var searchString = "";
        searchString = $("#keyNameSearch #gridSearch").val().trimLeft();

        //Filter The Data
        //Clear the Attribute and Leg Chain Listings
        uniquenessSetup._pageViewModelInstance.chainAttributeLevelListing.removeAll();
        uniquenessSetup._pageViewModelInstance.chainLegLevelListing.removeAll();


        if (uniquenessSetup._pageViewModelInstance.chainListing().length > 0) {

            var data = uniquenessSetup._pageViewModelInstance.chainListing();

            for (var item in data) {
                if (data[item].chainKeyID > 0 && data[item].chainKeyName().toLowerCase().indexOf(searchString.toLowerCase()) != -1) {
                    if (data[item].chainIsMaster()) {
                        uniquenessSetup._pageViewModelInstance.chainAttributeLevelListing.push(data[item]);
                    }
                    else {
                        uniquenessSetup._pageViewModelInstance.chainLegLevelListing.push(data[item]);
                    }
                }
            }

            if (uniquenessSetup._pageViewModelInstance.chainAttributeLevelListing().length == 0 && uniquenessSetup._pageViewModelInstance.chainLegLevelListing().length == 0) {
                $("#UniquenessSetupMain").hide();
                window.parent.leftMenu.showNoRecordsMsg("No keys found matching your search criteria.", $("#UniquenessSetupMainErrorDiv"));
            }
            else {
                $("#UniquenessSetupMain").show();
                window.parent.leftMenu.hideNoRecordsMsg();
            }
        }

        //Fetch all keys again and reload ChainListView
        // Get data from Service Call
        //var params = {};
        //params.selectedSectypes = [];
        //params.searchString = searchString;

        //for (var item in uniquenessSetup._selectedSectypes) {
        //    params.selectedSectypes.push(uniquenessSetup._selectedSectypes[item].value);
        //}

        //CallCommonServiceMethod('SearchUniqueKeys', params, OnSuccess_LoadUniqueKeysDataLater, OnFailure, null, false);

    });

    //// On Click Download All Unique Keys Icon
    //$("#keysDownloadAll").unbind('click').bind('click', function () {
    //    var params = {};
    //    CallCommonServiceMethod('DownloadAllUniqueKeys', params, OnSuccess_DownloadAllUniqueKeys, OnFailure, null, false);
    //});

    ///////////////////////////////////////////////////////
    //CODE For Single & Multi Select DROPDOWN For Filters//
    ///////////////////////////////////////////////////////
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
                selectelement.css({ /*'border': '1px solid #CECECE',*/ 'border-left': 'none', height: '22px', width: '180px', 'vertical-align': 'middle', 'text-align': 'left' });
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

    function ApplyMultiSelect(id, isRunningText, data, isSearch, container, text, allSelected, selectedItemsList) {
        var selectedItems = [];

        //If no data returned from DB
        if (data.length == 0) {
            var tempObj = {};
            tempObj.text = "None available";
            tempObj.value = "None available";
            data.push(tempObj);
        }

        if (allSelected) {
            $.each(data, function (key, selectedItemObj) {
                selectedItems.push(selectedItemObj.text);
            });
        }
        else {
            if (typeof (selectedItemsList) != 'undefined' && selectedItemsList.length > 0) {
                $.each(selectedItemsList, function (key, selectedItemObj) {
                    selectedItems.push(selectedItemObj.text);
                });
            }
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
                selectelement.css({ /*'border': '1px solid #CECECE',*/ 'border-left': 'none', height: '22px', width: '180px', 'vertical-align': 'middle', 'text-align': 'left' });
                //selectelement.find(".smselectrun").css({ height: '27px'});
                //if (data.length > 5) {
                //    selectelement.find(".smselectcon").height(selectelement.find(".smselectcon").height() + 3);
                //}                
                selectelement.on('change', function (ee) {
                    ddlOnChangeHandler(ee);
                });

                $(".UniquenessSetup_Leg_Edit_Blocks .UniquenessSetup_Block_Values .smselectrun, .UniquenessSetup_Attribute_Edit_Blocks .UniquenessSetup_Block_Values .smselectrun").click(function (e) {
                    var targetSMSelect = $(e.currentTarget).closest(".smselect");      //event.currentTarget can be ".smselectrun" wala div.
                    var targetScrollParent = targetSMSelect.closest(".scroll");    //Here "scroll" is the class given to the div on which overflow is auto, i.e., whose max-height is fixed.

                    //var actualScrollTop = targetScrollParent.scrollTop();
                    //actualScrollTop = actualScrollTop * -1;

                    var concernedSMSelectCon = targetSMSelect.find(".smselectcon");

                    concernedSMSelectCon.css('top', 'initial');

                    var viewportHeight = $(window).height();

                    var top = targetSMSelect.offset().top; // parseInt(target.offset().top);
                    var height = targetSMSelect.height() + 4 + concernedSMSelectCon.height(); // parseInt(target.height());
                    var diff = top + height - viewportHeight;

                    if (diff > 0) {
                        //var cssTop = top - hei;
                        //cssTop = typeof cssTop !== 'undefined' && cssTop != null && cssTop.indexOf('p') > -1 ? parseInt(cssTop.split('p')[0]) : 0;
                        //if (cssTop !== 0)
                        concernedSMSelectCon.css('top', (top - concernedSMSelectCon.height()));
                    }
                    else {
                        concernedSMSelectCon.css({ top: targetSMSelect.offset().top + targetSMSelect.height() + 4 });
                    }
                });

            }
        });
    }

    //This function is called when a Multi-Select is changed.
    function ddlOnChangeHandler(ee) {
        var target = $(ee.currentTarget);

        // On Change SecTypes in Top Div - CHANGED FINAL
        if (target[0].id == 'smselect_UniquenessSetupSectypeFilterMultiSelectedItems') {

            uniquenessSetup._selectedSectypes = [];
            var selectedSectypes = [];

            for (var item in smselect.getSelectedOption(target)) {
                uniquenessSetup._selectedSectypes.push(smselect.getSelectedOption(target)[item]);
                selectedSectypes.push(smselect.getSelectedOption(target)[item].value);
            }

            var params = {};
            params.selectedSectypes = selectedSectypes;
            CallCommonServiceMethod('GetUniqueKeysForSelectedSectypes', params, OnSuccess_LoadUniqueKeysDataLater, OnFailure, null, false);
        }
            // On Change SecTypes in New Key - CHANGED FINAL
        else if (target[0].id == 'smselect_sectypesValueDiv_NK') {

            var selectedSectypes = [];

            for (var item in smselect.getSelectedOption(target)) {
                selectedSectypes.push(smselect.getSelectedOption(target)[item].value);
            }

            if (selectedSectypes.length > 0) {
                //Get Common Master Attributes For Selected Sectypes
                var params = {}
                params.userName = uniquenessSetup._securityInfo.UserName;
                params.selectedSectypes = selectedSectypes;
                params.KeyID = -1;
                CallCommonServiceMethod('GetCommonMasterAttributesForSelectedSectypes', params, OnSuccess_GetCommonMasterAttributesForSelectedSectypes, OnFailure, null, false);

                //Get Common Legs For Selected Sectypes
                var params1 = {};
                params1.selectedSectypes = selectedSectypes;
                CallCommonServiceMethod('GetCommonLegsForSelectedSectypes', params1, OnSuccess_GetCommonLegsForSelectedSectypes, OnFailure, null, false);
            }
            else {
                //If no sectype is selected
                //For Attributes Multi Select
                var masterAttributesList = [{ text: "Please select a sectype", value: "-1" + "@&@" + "-1" }];
                $("#UniquenessSetup_NK_Block_3_A_Info").html("");
                ApplyMultiSelect('attributeLevelValueDiv_NK', false, masterAttributesList, true, '#UniquenessSetup_NK_Block_3_A_Info', 'Attributes', false);

                //For Legs Multi Select
                var legsList = [{ text: "Please select a sectype", value: "-1" + "@&@" + "-1" }];
                $("#UniquenessSetup_NK_Block_3_B_Info").html("");
                ApplySMSelect('legLevelValueDiv_NK', false, legsList, true, '#UniquenessSetup_NK_Block_3_B_Info');

                ////For Leg Attributes Multi Select
                //var legsList = [{ text: "Please select a leg", value: "Please select a leg" }];
                //$("#UniquenessSetup_NK_Block_4_Info").html("");
                //ApplyMultiSelect('legLevelAttributeValueDiv_NK', false, legsList, true, '#UniquenessSetup_NK_Block_4_Info', 'Leg Attributes', false);

            }
        }
            //// On change SecTypes
            //else if (target[0].id.startsWith("")) {

            //}
        else if (target[0].id == 'smselect_legLevelValueDiv_NK') {

            var selectedLeg = {};
            for (var item in smselect.getSelectedOption(target)) {
                toShow = true;
                selectedLeg.LegName = smselect.getSelectedOption(target)[item].text;
                selectedLeg.LegIDs = smselect.getSelectedOption(target)[item].value.split("@&@")[0];
                selectedLeg.AreAdditionalLegs = smselect.getSelectedOption(target)[item].value.split("@&@")[1];
            }

            //uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLeg = { text: selectedLeg.LegName, value: (selectedLeg.LegIDs + "@&@" + selectedLeg.AreAdditionalLegs) };

            //Get Common Leg Attributes For Selected Leg Name
            var params = {};
            params.InputObject = selectedLeg;
            params.KeyID = -1;
            if (selectedLeg.LegIDs != "-1") {
                CallCommonServiceMethod('GetCommonLegAttributesForSelectedLegName', params, OnSuccess_GetCommonLegAttributesForSelectedLegName, OnFailure, null, false);
            }
            else {
                //For Leg Attributes Multi Select
                var legsList = [{ text: "Please select a leg", value: "Please select a leg" }];
                $("#UniquenessSetup_NK_Block_4_Info").html("");
                ApplyMultiSelect('legLevelAttributeValueDiv_NK', false, legsList, true, '#UniquenessSetup_NK_Block_4_Info', 'Leg Attributes', false);
            }
            //$("#legLevelInfoCommonAttributesSuperDiv").css("display", "inline-block");

        }
        else if (target[0].id == 'smselect_attributeLevelValueDivSelectedItems') {

            uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedAttributes = [];

            for (var item in smselect.getSelectedOption(target)) {

                //var tempObj = {};

                //tempObj.AttributeName = smselect.getSelectedOption(target)[item].text;
                //tempObj.AttributeIDs = smselect.getSelectedOption(target)[item].value.split("@&@")[0];
                //tempObj.AreAdditionalLegAttributes = smselect.getSelectedOption(target)[item].value.split("@&@")[1];

                uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedAttributes.push(smselect.getSelectedOption(target)[item]);
            }
        }
        else if (target[0].id == 'smselect_legLevelAttributeValueDivSelectedItems') {

            uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLegAttributes = [];

            for (var item in smselect.getSelectedOption(target)) {

                //var tempObj = {};

                //tempObj.AttributeName = smselect.getSelectedOption(target)[item].text;
                //tempObj.AttributeIDs = smselect.getSelectedOption(target)[item].value.split("@&@")[0];
                //tempObj.AreAdditionalLegAttributes = smselect.getSelectedOption(target)[item].value.split("@&@")[1];

                uniquenessSetup._pageViewModelInstance.selectedChainInfo().chainSelectedLegAttributes.push(smselect.getSelectedOption(target)[item]);
            }
        }
    }

    $("#UniquenessSetupMainParentDiv, #UniquenessSetupMain, #UniquenessSetupLegLevelKeysSuperParentDiv, #UniquenessSetupAttributeLevelKeysSuperParentDiv").scroll(function () {
        $('.smselectcon').hide();
    });


    /////////////////////////////////////////////////////////
    //CODE For Display Message when Clicked on Disabled Div// - CHANGED FINAL
    /////////////////////////////////////////////////////////

    function OnClickDisabled() {
        $("#" + uniquenessSetup._controlIdInfo.LblErrorId).text("Please click on Save or Cancel to continue.");
        $find(uniquenessSetup._controlIdInfo.ModalErrorId).show();
    }


    ///////////////////////////////////////////////////////////////
    // init function to Load Initial Data as per Default Filters // - CHANGED FINAL
    ///////////////////////////////////////////////////////////////
    function init() {

        // Get data from Service Call
        var params = {};

        CallCommonServiceMethod('UniquenessSetupGetAllSectypes', params, OnSuccess_GetAllSectypes, OnFailure, null, false);
    }


    ////////////////////////////
    // "Initializer" function // 
    ////////////////////////////
    UniquenessSetup.prototype.Initializer = function Initializer(controlInfo, securityInfo) {

        //Hide Error Msg Div on Load
        if (typeof (window.parent.leftMenu) !== "undefined") {
            window.parent.leftMenu.hideNoRecordsMsg();
        }

        //Set Control ID Info & Security Info
        uniquenessSetup._controlIdInfo = eval("(" + controlInfo + ")");
        uniquenessSetup._securityInfo = eval("(" + securityInfo + ")");

        //Set height of Entire Body of the Page
        $('#UniquenessSetupBody').height($(window).height());

        //Setting Height of Area below the Filters - Change Required
        //$("#UniquenessSetupMainParentDiv").height($('#UniquenessSetupBody').height() - $('#UniquenessSetupTop').height() - 5);
        //$("#UniquenessSetupMain").height($('#UniquenessSetupBody').height() - $('#UniquenessSetupTop').height() - $("#UniquenessSetupMainNewKeyParentDiv").height() - 15);
        $("#UniquenessSetupMain").css("max-height", $('#UniquenessSetupBody').height() - $('#UniquenessSetupTop').height() - $("#UniquenessSetupMainNewKeyParentDiv").height() - 15);
        $("#UniquenessSetupMainParentDiv").css({ "min-height": $("#UniquenessSetupMainNewKeyParentDiv").height() + 275, "max-height": $('#UniquenessSetupBody').height() - $('#UniquenessSetupTop').height() - 5 });
        //$("#UniquenessSetupMainErrorDiv").height($('#UniquenessSetupBody').height() - $('#UniquenessSetupTop').height() - 2);

        //Temporarily - Debugging - CHANGED FINAL
        init();
    }

    /////////////////////
    // Extra Functions //
    /////////////////////

    // WINDOW Resize Function 
    $(window).resize(function () {

        //Set height of Entire Body of the Page
        $('#UniquenessSetupBody').height($(window).height());

        //Setting Height of Area below the Filters
        //$("#UniquenessSetupMainParentDiv").height($('#UniquenessSetupBody').height() - $('#UniquenessSetupTop').height() - 5);
        //$("#UniquenessSetupMain").height($('#UniquenessSetupBody').height() - $('#UniquenessSetupTop').height() - $("#UniquenessSetupMainNewKeyParentDiv").height() - 15);
        $("#UniquenessSetupMain").css("max-height", $('#UniquenessSetupBody').height() - $('#UniquenessSetupTop').height() - $("#UniquenessSetupMainNewKeyParentDiv").height() - 15);
        $("#UniquenessSetupMainParentDiv").css({ "min-height": $("#UniquenessSetupMainNewKeyParentDiv").height() + 275, "max-height": $('#UniquenessSetupBody').height() - $('#UniquenessSetupTop').height() - 5 });
        //$("#UniquenessSetupMainErrorDiv").height($('#UniquenessSetupBody').height() - $('#UniquenessSetupTop').height() - 2);
    })

    //Function to Remove the last part of the Directory
    function RemoveLastDirectoryPartOf(the_url) {
        var the_arr = the_url.split('/');
        the_arr.pop();
        return (the_arr.join('/'));
    }

    function onClickSecurityIdInSRMUniquenessPopup(securityId) {
        //open Create Update Screen in new window
        window.parent.SecMasterJSCommon.SMSCommons.openSecurity(true, securityId, '', true, true, false, '', '', false, false, window.parent.SecMasterJSCommon.SMCreateUpdateOpenByDefault.None, 0, "", 3);
    }

    function onClickCloseSRMUniquenessPopup() {
        $('#divErrorSMUSUniqueness').css('display', 'none');
        $('#divErrorSMUSUniquenessOverlay').css('display', 'none');
    }

    return uniquenessSetup;
})();