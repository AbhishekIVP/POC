var smMergeDuplicate = {
    _path: '',
    _moduleID_moduleName_map: [],
    _serviceASMXLocation: "/BaseUserControls/Service/CommonService.asmx",
    _serviceCommonSVCLocation: "/BaseUserControls/Service/CommonService.svc",
    _serviceDeDuplicationLocation: "/BaseUserControls/Service/DeDuplicationService.svc",
    _smServiceASMXLocation: "../../BaseUserControls/Service/Service.asmx",
    _allAttributesList: [],
    _allRefAttributesList: [],
    _allAttributesListWithDataType: [],
    _allRefAttributesListWithDataType: [],
    _matchType: null,
    _productList: { modules: [], selectedModule: "SecMaster" },
    _windowHeight: null,
    _windowWidth: null,
    _selectedSectype: "-1",
    _selectedDupesSectype: ["0"],
    _selectedAttributes: [],
    _sectypeList: [],
    _selectedSecuritiesFromGrid: [],
    _selectedSectypeNamesFromGrid: [],
    _currectMatchWeight: 100,
    _mergeSecColumnWidth: 250,
    _rowHeightForSlimScroll: 45,
    _filterResponseData: null,
    _duplicateSecuritiesData: null,
    _primaryAttrDropdownWidth: 350,
    _primaryMatchTypeDropDownWidth: 300,
    _toleranceOptionsDropdownWidth: 100,
    _contractedAttrDropDown: 155,
    _contractedMatchTypeDropDown: 150,
    _finalMergeData: [],
    _finalMergeSelectedSectype: '',
    _dicSectypeVsSecId: null,
    _username: null,
    _toleranceOptions: null,
    _isDeDupeAllowedInRefM: null,
    _isDeDupeAllowedInSecM: null,
    _moduleID_moduleName_map: [],
    presetData: null,
    _refMLandingPage: false,
    _fromMatchTypeLandingPage: false,
    setPath: function () {
        var path = window.location.protocol + '//' + window.location.host;
        var pathname = window.location.pathname.split('/');

        $.each(pathname, function (ii, ee) {
            if ((ii !== 0) && (ii !== pathname.length - 1))
                path = path + '/' + ee;
        });

        smMergeDuplicate._path = path;
    },
    massageAttributeList: function () {
        if (smMergeDuplicate._productName.toLowerCase() === "refmaster") {
            var arrList = smMergeDuplicate._allRefAttributesList;
            smMergeDuplicate._allRefAttributesList = [];
            if (arrList != null && arrList != undefined) {
                for (var i = 0; i < arrList.length; i++) {
                    var text = arrList[i].split("|")[1];
                    var value = arrList[i].split("|")[0].split('&&')[0];
                    var dataType = arrList[i].split("|")[0].split('&&')[1];
                    var tempObj = { "text": text, "value": value };
                    var tObj = { "attrId": value, "attrDataType": dataType };
                    smMergeDuplicate._allRefAttributesListWithDataType.push(tObj);
                    smMergeDuplicate._allRefAttributesList.push(tempObj);
                }
            }
        }
        else {
            var arrList = smMergeDuplicate._allAttributesList;
            smMergeDuplicate._allAttributesList = [];
            if (arrList != null && arrList != undefined) {
                for (var i = 0; i < arrList.length; i++) {
                    var text = arrList[i].split("|")[1];
                    var value = arrList[i].split('&&')[0];
                    var dataType = arrList[i].split('&&')[1].split("|")[0];
                    var tempObj = { "text": text, "value": value };
                    var tObj = { "attrId": value, "attrDataType": dataType };
                    smMergeDuplicate._allAttributesListWithDataType.push(tObj);
                    smMergeDuplicate._allAttributesList.push(tempObj);
                }
            }
        }
    },
    hideText: function (targetDiv) {
        setTimeout(function () {
            targetDiv.text("");
        }, 3000);
    },
    hideTextAfterCustomWait: function (targetDiv, interval) {
        setTimeout(function () {
            targetDiv.text("");
        }, interval);
    },
    getViewPrivileges: function () {
        return new Promise(function (res, rej) {
            var promiseArr = [];
            if (smMergeDuplicate._productName.toLowerCase() !== "refmaster") {
                var p1 = new Promise(function (resolve, reject) {
                    $.ajax({
                        type: 'POST',
                        url: smMergeDuplicate._path + smMergeDuplicate._serviceCommonSVCLocation + "/CheckControlPrivilegeForUser",
                        contentType: "application/json",
                        dataType: "json",
                        data: JSON.stringify({
                            //"pageId": "topMenu",
                            "privilegeName": "Search Duplicate Securities",
                            "userName": smMergeDuplicate._username
                        }),
                        success: function (data) {
                            data = data.d;
                            smMergeDuplicate._isDeDupeAllowedInSecM = data;
                            resolve();
                        },
                        error: function (ex) {
                            console.log("DeDuplication Privilege cannot be fetched");
                            reject(ex);
                        }
                    });
                });
                promiseArr.push(p1);
            }

            var p2 = new Promise(function (resolve, reject) {
                $.ajax({
                    type: 'POST',
                    url: smMergeDuplicate._path + smMergeDuplicate._serviceCommonSVCLocation + "/CheckControlPrivilegeForUser",
                    contentType: "application/json",
                    dataType: "json",
                    data: JSON.stringify({
                        //"pageId": "topMenu",
                        "privilegeName": "Search Duplicate Reference Data",
                        //"controlId": "",
                        "userName": smMergeDuplicate._username
                    }),
                    success: function (data) {
                        data = data.d;
                        smMergeDuplicate._isDeDupeAllowedInRefM = data;
                        resolve();
                    },
                    error: function (ex) {
                        console.log("DeDuplication Privilege cannot be fetched");
                        reject(ex);
                    }
                });
            });
            promiseArr.push(p2);

            Promise.all(promiseArr)
            .then(function () { res(); })
            .catch(function (ex) { rej(ex); });
        });
    },
    preInit: function () {
        onServiceUpdating();
        //To fetch the url to make ajax calls
        smMergeDuplicate.setPath();
        //smMergeDuplicate.getViewPrivileges().then(function () {


        //To Set width of products tab at the top
        //smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().find(".smmergeduplicateProductDivStyle").css("width", ((smMergeDuplicate._windowWidth * 45) / 200) + "px");

        //Master page handling
        var listofTabsToBindFunctionsWith = CommonModuleTabs.ModuleList();

        for (i in listofTabsToBindFunctionsWith) {
            item = listofTabsToBindFunctionsWith[i];
            smMergeDuplicate._moduleID_moduleName_map[item.displayName.toLowerCase()] = item.moduleId;

            switch (item.displayName.toLowerCase().trim()) {
                case "securities":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: smMergeDuplicate.secMasterInitMergeDuplicatesScreen });
                    break;
                case "refdata":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: smMergeDuplicate.refMasterInitMergeDuplicatesScreen });
                    break;
            }
        }



    },
    secMasterInitMergeDuplicatesScreen: function () {
        var paddingHeightForFirstScreen = ($(window).height() - $("#srm_moduleTabs").height() - $("#smmergedFindDuplicatesContainer").height() - 30 - 25 - 110) / 2;
        $("#smmergedFindDuplicatesContainer").css({
            "padding-top": paddingHeightForFirstScreen - 65, "padding-bottom": paddingHeightForFirstScreen + 65
        });
        $("#smmergedFindDuplicatesContainer").removeClass('smmergeTopSection');
        $("#smmergedFindDuplicatesContainer").addClass('smmergedFirstPageStyle');
        $("#smmergedBottomDisplayPart").hide();

        if (smMergeDuplicate._presetValue != null && smMergeDuplicate._presetValue != undefined && smMergeDuplicate._presetValue != '')
            $('.smmergeduplicateFilterOuterDiv').css('opacity', '0');

        smMergeDuplicate._windowHeight = $(window).height() - $("#srm_moduleTabs").height() - 2;
        smMergeDuplicate._windowWidth = $(window).width();
        smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().css("height", smMergeDuplicate._windowHeight);
        smMergeDuplicate.init("secmaster");
    },
    refMasterInitMergeDuplicatesScreen: function () {
        var paddingHeightForFirstScreen = ($(window).height() - $("#srm_moduleTabs").height() - $("#smmergedFindDuplicatesContainer").height() - 30 - 25 - 110) / 2;
        $("#smmergedFindDuplicatesContainer").css({
            "padding-top": paddingHeightForFirstScreen - 65, "padding-bottom": paddingHeightForFirstScreen + 65
        });
        $("#smmergedFindDuplicatesContainer").removeClass('smmergeTopSection');
        $("#smmergedFindDuplicatesContainer").addClass('smmergedFirstPageStyle');
        $("#smmergedBottomDisplayPart").hide();

        if (smMergeDuplicate._presetValue != null && smMergeDuplicate._presetValue != undefined && smMergeDuplicate._presetValue != '')
            $('.smmergeduplicateFilterOuterDiv').css('opacity', '0');

        smMergeDuplicate._windowHeight = $(window).height() - $("#srm_moduleTabs").height() - 2;
        smMergeDuplicate._windowWidth = $(window).width();
        smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().css("height", smMergeDuplicate._windowHeight);
        smMergeDuplicate.init("refmaster");
    },

    init: function (prodName) {
        onServiceUpdating();
        smMergeDuplicate._productName = prodName;

        //removes all the previous entries by deleting them manually
        $("#smmergeduplicateAttributesContainer").find(".smmergeduplicateTrashIconPosition").each(function (index, value) {
            //$(index).click();
            $(value).click();
        })

        //To Get all Tolerance Options for all datatypes
        smMergeDuplicate.getToleranceOptions();


        //privilege issue 393811
        if (smMergeDuplicate._productName == 'refmaster')
            smMergeDuplicate._refMLandingPage = true;

        //To bind the Sectype Drop Down and after that also get the Match Type List
        if (smMergeDuplicate._productName.toLowerCase() === 'refmaster') {
            smMergeDuplicate._selectedDupesSectype = -2;
            smMergeDuplicate.bindEntitytypeDropDown(smMergeDuplicate.getAllAttributesList, smMergeDuplicate.getMatchTypeList);
        }
        else {
            smMergeDuplicate._selectedDupesSectype = ["0"];
            smMergeDuplicate.bindSectypeDropDown(smMergeDuplicate.getAllAttributesList, smMergeDuplicate.getMatchTypeList);
        }

        //unset
        //smMergeDuplicate._refMLandingPage = false;

        //To attach focus out event with all inputs that are used for storing weight values
        smMergeDuplicate.onFocusOutInput();
        //To bind saved filter drop down
        smMergeDuplicate.bindSavedFilter();

        smMergeDuplicate.createEventHandlers();

        //Initialise SM Grid
        smMergeDuplicate.sm_grid;
        //setting initially

        //correct input bindings
        applyCorrectInputBinding();


    },
    getSelectedAttributeDataType: function (selectedAttrValue) {
        var attrDataType = null;
        if (smMergeDuplicate._productName.toLowerCase() === "refmaster") {
            for (var item in smMergeDuplicate._allRefAttributesListWithDataType) {
                if (smMergeDuplicate._allRefAttributesListWithDataType[item].attrId == selectedAttrValue) {
                    attrDataType = smMergeDuplicate._allRefAttributesListWithDataType[item].attrDataType;
                    break;
                }
            }
        }
        else {
            for (var item in smMergeDuplicate._allAttributesListWithDataType) {
                if (smMergeDuplicate._allAttributesListWithDataType[item].attrId == selectedAttrValue) {
                    attrDataType = smMergeDuplicate._allAttributesListWithDataType[item].attrDataType;
                    break;
                }
            }
        }

        return attrDataType;
    },

    onChangeMatchType: function (e) {
        var selectedOption = smselect.getSelectedOption($(e.target))[0];
        var attrArrayReturn = smselect.getSelectedOption($("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")));
        if (attrArrayReturn.length > 0) {
            var selectedAttrValue = attrArrayReturn[0].value;
            var options = [];
            var attrDataType = smMergeDuplicate.getSelectedAttributeDataType(selectedAttrValue);

            //            if (selectedOption.text.toLowerCase() === "approximate match") {
            //                for(var i in smMergeDuplicate._toleranceOptions)
            //                {
            //                    var ele = smMergeDuplicate._toleranceOptions[i];
            //                    if (ele.Key === attrDataType) {
            //                        options = ele.Value;
            //                        break;
            //                    }
            //                }
            //            }
            var firstSelectedOption;


            if (selectedOption.text.toLowerCase() === "approximate match" && (attrDataType.toLowerCase() === "date" || attrDataType.toLowerCase() === "time" || attrDataType.toLowerCase() === "numeric" || attrDataType.toLowerCase() === "datetime" || attrDataType.toLowerCase() === "decimal" || attrDataType.toLowerCase() === "int")) {
                if (attrDataType.toLowerCase() == "decimal" || attrDataType.toLowerCase() == "int")
                    attrDataType = "NUMERIC";
                if (attrDataType.toLowerCase() == "datetime")
                    attrDataType = "DATE";
                $("#toleranceDropDown").css('color', '#353839 !important')
                $("#toleranceDropdown .smselectanchorrun2").css("color", "#353839");
                $("#toleranceDropDownDummy").hide();

                if (attrDataType.toLowerCase() === "date")
                    firstSelectedOption = "Days";
                else if (attrDataType.toLowerCase() === "numeric" || attrDataType.toLowerCase() === "decimal")
                    firstSelectedOption = "Percentage";
            }
            else {
                $("#toleranceDropDown").css('color', '#9a9a9a;')
                $("#toleranceDropdown .smselectanchorrun2").css("color", "#9a9a9a;");
                $("#toleranceDropdown .smselectimage").css("color", "#9a9a9a;");
                $("#toleranceDropDownDummy").show();
                firstSelectedOption = "";
            }


            //            var smSelectOption = [];
            //            for (var o in options) {
            //                smSelectOption.push({ "text": options[o].text, "value": options[o].value});
            //            }

            //			if(smSelectOption.length > 0)
            //			{
            //				smMergeDuplicate.createSMSelectDropDown($("#toleranceDropdown "), smSelectOption, true, "150", function () { }, "Options");
            //				smselect.setOptionByText($("#toleranceDropdown"), firstSelectedOption);
            //				$("#toleranceDropdown .smselectanchorrun2").css("color", "#9a9a9a;");
            //				$("#toleranceDropdown .smselectimage").css("color", "#9a9a9a;");
            //			}

            if ($(".smmergedFirstPageStyle"))
                $("#toleranceDropdown .smselectanchorrun2").css("font-size", "18px");
            else
                $("#toleranceDropdown .smselectanchorrun2").css("font-size", "14px");

            if (selectedOption.text.toLowerCase() === "approximate match" && (attrDataType.toLowerCase() === "date" || attrDataType.toLowerCase() === "time" || attrDataType.toLowerCase() === "numeric" || attrDataType.toLowerCase() === "datetime" || attrDataType.toLowerCase() === "decimal" || attrDataType.toLowerCase() == "int")) {
                if (attrDataType.toLowerCase() == "decimal" || attrDataType.toLowerCase() == "int")
                    attrDataType = "NUMERIC";
                if (attrDataType.toLowerCase() == "datetime")
                    attrDataType = "DATE";
                $("#toleranceDropdown .smselectanchorrun2").css("color", "#353839");
                $("#toleranceDropdown .smselectimage").css("color", "#353839");
                if ($(".smmergedFirstPageStyle"))
                    $("#toleranceDropdown .smselectanchorrun2").css("font-size", "18px");
                else
                    $("#toleranceDropdown .smselectanchorrun2").css("font-size", "14px");
            }

        }
            //42//
        else {
            $("#toleranceDropDown").css('color', '#9a9a9a;')
            $("#toleranceDropdown .smselectanchorrun2").css("color", "#9a9a9a;");
            $("#toleranceDropdown .smselectimage").css("color", "#9a9a9a;");
            $("#toleranceDropDownDummy").show();
            firstSelectedOption = "";
            //$("#toleranceInput").val(0);
            //    smselect.reset($("#smselect_toleranceDropdown"));


            if ($(".smmergedFirstPageStyle"))
                $("#toleranceDropdown .smselectanchorrun2").css("font-size", "18px");
            else
                $("#toleranceDropdown .smselectanchorrun2").css("font-size", "14px");
        }
        //42//
    },
    onChangeAttribute: function (e) {
        var target = $(e.target);
        var selectedAttrArray = smselect.getSelectedOption(target);
        if (selectedAttrArray.length > 0) {
            var selectedAttrValue = selectedAttrArray[0].value;
            var options = [];
            var attrDataType = smMergeDuplicate.getSelectedAttributeDataType(selectedAttrValue).toLowerCase();


            //Disabling match type dropdown in case boolean attribute type is selected and only works with exact match type
            if (checkForBooleanType(attrDataType.toLowerCase())) {
                //smselect.setOptionByText($("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown().attr("id")), "Exact Match");
                smselect.setOptionByText($("#smselect_smmergeduplicatePrimaryMatchTypeDropDown"), "Exact Match");
                smselect.disable($("#smselect_smmergeduplicatePrimaryMatchTypeDropDown"));
            }
            else {
                smselect.enable($("#smselect_smmergeduplicatePrimaryMatchTypeDropDown"));
            }

            //reset on change
            smselect.setOptionByText($("#smselect_smmergeduplicatePrimaryMatchTypeDropDown"), "Exact Match");

            if ($(".smmergedFirstPageStyle").length) {
                $("#smmergeduplicatePrimaryWeightText").val("100%");
            }

            if (attrDataType.toLowerCase() == "datetime")
                attrDataType = "DATE";
            if (attrDataType.toLowerCase() == "decimal" || attrDataType.toLowerCase() == "int")
                attrDataType = "NUMERIC";
            var options = [];
            for (var item in smMergeDuplicate._toleranceOptions) {
                if (smMergeDuplicate._toleranceOptions[item].Key.toLowerCase() == attrDataType.toLowerCase()) {
                    options = smMergeDuplicate._toleranceOptions[item].Value;
                    break;
                }
            }

            var smSelectOption = [];
            for (var o in options) {
                smSelectOption.push({ "text": options[o].text, "value": options[o].value });
            }

            if (smSelectOption.length > 0) {
                smMergeDuplicate.createSMSelectDropDown($("#toleranceDropdown "), smSelectOption, true, "150", function () { }, "Options");
                smselect.setOptionByText($("#toleranceDropdown"), smSelectOption[0].text);
                $("#toleranceDropdown .smselectanchorrun2").css("color", "#9a9a9a;");
                $("#toleranceDropdown .smselectimage").css("color", "#9a9a9a;");
            }
        }
    },
    initDropDowns: function () {
        //if (smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().attr("is_expanded") === "true") {
        //!smMergeDuplicate.getAttributesListBasedOnModuleType().length && 
        if (smselect.getSelectedOption($("#smselect_smmergeduplicateSectypeDropDown")).length == 0 || (smMergeDuplicate._refMLandingPage && smMergeDuplicate._fromMatchTypeLandingPage)) {
            if (smMergeDuplicate._refMLandingPage && smMergeDuplicate._fromMatchTypeLandingPage) {
                smselect.reset($("#smselect_smmergeduplicateSectypeDropDown"));
                var totalWidthOfContent = $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectanchorcontainer").width() + $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectimage ").width();
                var totalWidth = $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectrun").width();
                $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectrun").css("padding-left", ((totalWidth - totalWidthOfContent - 2) / 2));
                $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectrun").css("padding-right", ((totalWidth - totalWidthOfContent - 2) / 2));
            }
            smMergeDuplicate._refMLandingPage == false;

            smselect.reset(smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown());
            smselect.disable(smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown());
            //42//
            smselect.disable(smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown());
            if ($(".smmergedFirstPageStyle").length) {
                $("#smmergeduplicatePrimaryWeightText").val("100%");
            }
            $("#smmergeduplicatePrimaryWeightText").prop('disabled', true);
            $("#toleranceDropDownDummy").show();
            $("#smergeduplicateToleranceInput").val(0);
            smselect.disable($("#smselect_toleranceDropdown"));
        }
            //42//
        else {
            $("#smmergeduplicatePrimaryWeightText").prop('disabled', false);
            smselect.enable(smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown());
            smMergeDuplicate.createSMSelectDropDown(smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown(), smMergeDuplicate.getAttributesListBasedOnModuleType(), true, smMergeDuplicate._primaryAttrDropdownWidth.toString(), function () {
                $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")).css("text-align", "center");
                //$("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")).find(".smselectanchorcontainer ").css("width", "95%");
                $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")).find(".smselectanchorcontainer").css("width", "auto");
                //$("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")).find(".smselectanchorcontainer").parent().css("width", "50%");

                $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")).find(".smselectcon").css("margin-left", "20%");

                $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")).find(".smselectanchorrun").css("font-size", "18px");
                $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")).find(".smselectanchorrun").css("color", "#353839");
                $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")).find(".smselectimage").css("float", "left");

                //center sm
                var totalWidthOfContent = $("#smselect_smmergeduplicatePrimaryAttributeDropDown").find(".smselectanchorcontainer").width() + $("#smselect_smmergeduplicatePrimaryAttributeDropDown").find(".smselectimage ").width();
                var totalWidth = $("#smselect_smmergeduplicatePrimaryAttributeDropDown").find(".smselectrun").width();
                $("#smselect_smmergeduplicatePrimaryAttributeDropDown").find(".smselectrun").css("padding-left", ((totalWidth - totalWidthOfContent - 2) / 2));
                $("#smselect_smmergeduplicatePrimaryAttributeDropDown").find(".smselectrun").css("padding-right", ((totalWidth - totalWidthOfContent - 2) / 2));
            }, "Select Attributes", smMergeDuplicate.onChangeAttribute);
        }

        var matchTypeSelected = [];
        for (var item in smMergeDuplicate._matchType) {
            if (smMergeDuplicate._matchType[item].value === "1") {
                matchTypeSelected.push(smMergeDuplicate._matchType[item].text);
            }
        }
        smMergeDuplicate.createSMSelectDropDown(smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown(), smMergeDuplicate._matchType, false, smMergeDuplicate._primaryMatchTypeDropDownWidth.toString(), function () {
            $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown().attr("id")).css("text-align", "center");
            //$("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown().attr("id")).find(".smselectanchorcontainer ").css("width", "95%");
            $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")).find(".smselectanchorcontainer").css("width", "auto");
            //  $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")).find(".smselectanchorcontainer").parent().css("width", "50%");
            $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")).find(".smselectcon").css("margin-left", "20%");
            //$("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown().attr("id")).find(".smselectanchorcontainer ").css("margin-left", "25%");
            $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown().attr("id")).find(".smselectanchorrun").css("font-size", "18px");
            $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown().attr("id")).find(".smselectanchorrun").css("color", "#353839");
            $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown().attr("id")).find(".smselectimage").css("float", "left");

        }, null, smMergeDuplicate.onChangeMatchType, matchTypeSelected);
        smselect.setOptionByText($("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown().attr("id")), "Exact Match");
        //sm select centering
        var totalWidthOfContent = $("#smselect_smmergeduplicatePrimaryMatchTypeDropDown").find(".smselectanchorcontainer").width() + $("#smselect_smmergeduplicatePrimaryMatchTypeDropDown").find(".smselectimage ").width();
        var totalWidth = $("#smselect_smmergeduplicatePrimaryMatchTypeDropDown").find(".smselectrun").width();
        $("#smselect_smmergeduplicatePrimaryMatchTypeDropDown").find(".smselectrun").css("padding-left", ((totalWidth - totalWidthOfContent - 2) / 2));
        $("#smselect_smmergeduplicatePrimaryMatchTypeDropDown").find(".smselectrun").css("padding-right", ((totalWidth - totalWidthOfContent - 2) / 2));

        //Set the width of the input element according to screen size
        smMergeDuplicate.controls.smmergeduplicatePrimaryWeightText().parent().width(smMergeDuplicate.getInputElementWidth());
        //Resizing issue

        var options = [];
        var smSelectOption = [];
        for (var i in smMergeDuplicate._toleranceOptions) {
            var ele = smMergeDuplicate._toleranceOptions[i];
            if (ele.Key === "NUMERIC" || ele.Key === "DECIMAL") {
                if (ele.Key.toLowerCase() == "decimal")
                    ele.Key = "NUMERIC";
                options = ele.Value;
                break;
            }
        }

        for (var o in options) {
            smSelectOption.push({ "text": options[o].text, "value": options[o].value });
        }
        smMergeDuplicate.createSMSelectDropDown($("#toleranceDropdown"), smSelectOption, true, "150", function () { }, "Options");
        smselect.setOptionByText($("#toleranceDropdown"), "Percentage");

        $("#smselect_toleranceDropdown .smselectanchorrun").css('color', '#9a9a9a;');
        $("#smselect_toleranceDropdown .smselectanchorrun").css('font-size', '18px');


        //smMergeDuplicate.controls.smmergeduplicatePrimaryWeightText().parent().width(widthForMatchPercent);
        //}
        //else {
        //    smMergeDuplicate.createSMSelectDropDown(smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown(), smMergeDuplicate._allAttributesList, true, smMergeDuplicate._contractedAttrDropDown, function () {
        //        //$("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")).find(".smselectcon").css("width", smMergeDuplicate._contractedAttrDropDown + "px");
        //        $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")).find(".smselectanchorrun").css("font-size", "18px");
        //        $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")).find(".smselectanchorrun").css("color", "#353839");
        //    }, "Select Attributes");
        //    smMergeDuplicate.createSMSelectDropDown(smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown(), smMergeDuplicate._matchType, false, smMergeDuplicate._contractedMatchTypeDropDown, function () {
        //        //$("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown().attr("id")).find(".smselectcon").css("width", smMergeDuplicate._contractedMatchTypeDropDown + "px");
        //        $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown().attr("id")).find(".smselectanchorrun").css("font-size", "18px");
        //        $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown().attr("id")).find(".smselectanchorrun").css("color", "#353839");
        //    }, "Match Type");

        //   // smMergeDuplicate.controls.smmergeduplicatePrimaryWeightText().parent().width(70);
        //}

    },
    initElementsPosition: function () {
        var innerDivWidth = smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().find(".smmergeduplicateFilterInnerDiv").width();

        //smMergeDuplicate.controls.smmergeduplicateSavedFilters().css("margin-left", (innerDivWidth - (205 + smMergeDuplicate.controls.smmergeduplicateHeading().width())) + "px");
        //smMergeDuplicate.controls.smmergeduplicateFindDuplicatesBtn().parent().css("margin-left", (innerDivWidth - 435) + "px");
    },
    getInputElementWidth: function (isApproxMatch) {
        var widthForMatchPercent = $(window).width() - ($("#smmergeduplicatePrimaryMatchTypeDropDown").offset().left + $("#smmergeduplicatePrimaryMatchTypeDropDown").width()) - $("#toleranceDropDown").width() - 180;
        var outerDivWidth = smMergeDuplicate.controls.smmergeduplicateAttributeSelectionDiv().outerWidth();
        var inputElementWidth = (outerDivWidth - (smMergeDuplicate._primaryAttrDropdownWidth + smMergeDuplicate._primaryMatchTypeDropDownWidth + 40)); // 250 for attr drop down, 200 for match type drop down and 40 for plus sign

        if (isApproxMatch) {
            //widthForMatchPercent += 220;
            //widthForMatchPercent -= $("#toleranceDropDown").width()
        }
        //if ($(".smmergedFirstPageStyle").length)
        //{ }
        //else {
        //    $(window).trigger('resize');
        //}

        return widthForMatchPercent;
    },
    getToleranceOptions: function () {
        $.ajax({
            async:false,
            type: 'POST',
            url: smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + "/GetToleranceOptions",
            contentType: "application/json",
            success: function (data) {
                smMergeDuplicate._toleranceOptions = data.d;
                var tempObj = { "Key": "DATETIME", "Value": [].concat(smMergeDuplicate._toleranceOptions.find(function (i) { return i.Key === "DATE" }).Value, smMergeDuplicate._toleranceOptions.find(function (i) { return i.Key === "TIME" }).Value) };
                smMergeDuplicate._toleranceOptions.push(tempObj);
            },
            error: function (ex) {
                console.log("Tolerance Options cannot be fetched");
            }
        });
    },
    createEventHandlers: function () {
        if (smMergeDuplicate.controls.smmergeduplicateAddAttributeBtn().length !== 0) {
            smMergeDuplicate.controls.smmergeduplicateAddAttributeBtn().unbind('click').bind('click', smMergeDuplicate.onClickAddAttributeBtn);
        }
        //if (smMergeDuplicate.controls.smmergeduplicateChooseProduct().length !== 0) {
        //    smMergeDuplicate.controls.smmergeduplicateChooseProduct().unbind('click').bind('click', smMergeDuplicate.onClickProductTab);
        //}
        if (smMergeDuplicate.controls.smmergeduplicateFindDuplicatesBtn().length !== 0) {
            smMergeDuplicate.controls.smmergeduplicateFindDuplicatesBtn().unbind('click').bind('click', smMergeDuplicate.onClickFindDuplicatesBtn);
        }
        if (smMergeDuplicate.controls.smmergeduplicateSaveBtn().length !== 0) {
            smMergeDuplicate.controls.smmergeduplicateSaveBtn().unbind('click').bind('click', smMergeDuplicate.onClickCreatePreset);
        }
        if (smMergeDuplicate.controls.smmergeduplicateMergeBtn().length !== 0) {
            smMergeDuplicate.controls.smmergeduplicateMergeBtn().unbind('click').bind('click', smMergeDuplicate.onClickMergeDupes);
        }
        if (smMergeDuplicate.controls.smmergeduplicateMergeDupesCloseBtn().length !== 0) {
            smMergeDuplicate.controls.smmergeduplicateMergeDupesCloseBtn().unbind('click').bind('click', smMergeDuplicate.onClickMergeDupesCloseBtn);
        }
        if (smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().length !== 0) {
            smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().unbind('click').bind('click', smMergeDuplicate.onClickRightFilterBtn);
        }
    },
    onClickRightFilterBtn: function () {

        if (parseInt(smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().css("height")) == 30) {

            //bottom arrow show

            $('#srm_moduleTabs').show();
            $('.smmergeduplicateFilterOuterDiv').show();
            $('#smmergeduplicatesGrid').hide();
            smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().css("height", $(window).height());
            $("#smmergeduplicateRightFilterIcon").hide();



            //smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().show();
            //smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().height(smMergeDuplicate._windowHeight);
            //smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().removeClass("smmergeduplicateRightFilterBackground");

            //no longer icon change
            //smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().animate({ marginBottom: '0px' }, 600, function () {
            //    smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().removeClass("fa-chevron-circle-down");
            //    smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().addClass("fa-chevron-circle-up");
            //});

        }
        //else {
        //    var currMarginLeft = parseInt(smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().css("margin-left"));
        //    smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().animate({ marginLeft: "-" + (420) + 'px' }, 600, function () {
        //        smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().removeClass("fa-chevron-circle-left");
        //        smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().addClass("fa-chevron-circle-right");
        //        smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().height(smMergeDuplicate._windowHeight);
        //        smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().addClass("smmergeduplicateRightFilterBackground");
        //    });
        //}
        //    else {
        //        $('#srm_moduleTabs').hide();
        //        $('.smmergeduplicateFilterOuterDiv').hide();
        //        $('#smmergeduplicatesGrid').show();
        //        smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().css("height", '30px');
        //        //var currMarginLeft = parseInt(smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().css("margin-left"));
        //        smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().animate({  }, 600, function () {
        //        smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().removeClass("fa-chevron-circle-up");
        //        smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().addClass("fa-chevron-circle-down");
        //        //smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().height(smMergeDuplicate._windowHeight);
        //        //smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().addClass("smmergeduplicateRightFilterBackground");
        //    });
        //}
    },
    onClickAddAttributeBtn: function (event) {
        smMergeDuplicate.controls.smmergeduplicateHdnSelectedFilter().val("0");
        smMergeDuplicate.createAttributeSelectionDivRow();

        //in case sm select match type was disabled
        smselect.enable($("#smselect_smmergeduplicatePrimaryMatchTypeDropDown"));

        //disable the add button's last div
        $("#toleranceDropDown").css('color', '#9a9a9a;')
        $("#toleranceDropdown .smselectanchorrun2").css("color", "#9a9a9a");
        $("#toleranceDropdown .smselectimage").css("color", "#9a9a9a;");
        if ($(".smmergeTopSection"))
            $("#toleranceDropdown .smselectanchorrun2").css("font-size", "14px");
        else {
            $("#toleranceDropdown .smselectanchorrun2").css("font-size", "22px");
        }

        $("#toleranceDropDownDummy").show();
        firstSelectedOption = "";

        //if ($("#smmergeduplicateAttributesContainer").attr('row_number') > 0) {
        //    $("#smmergedFindDuplicatesContainer").css({ "padding-top": '30px', "padding-bottom": '30px' });
        //    $("#smmergedFindDuplicatesContainer").removeClass('smmergedFirstPageStyle');
        //    $("#smmergedFindDuplicatesContainer").addClass('smmergeTopSection');
        //    $("#smmergedBottomDisplayPart").show();

        //}

        //$("#smmergeduplicateDeleteSecurityPopup").animate({ "padding-top": popupHeight + "px" }, 500, function () {
        //    $("#smmergeduplicateDeleteSecurityPopupIframe").attr("src", url);
        //});

        //$('#smmergeduplicateAttributeSelectionDiv').css('height', $("#smmergeduplicateAttributeSelectionDiv").height() );

        event.stopPropagation();
    },
    onClickProductTab: function (event) {
        var target = $(event.target);
        smMergeDuplicate.controls.smmergeduplicateChooseProduct().find("div").removeClass("smmergeduplicateProductTabSelected");
        target.addClass("smmergeduplicateProductTabSelected");

        event.stopPropagation();
    },
    onClickFindDuplicatesBtn: function (event) {
        //Check for proper sum
        var listOfAttributePercentValues = $("input[id^='smmergeduplicatePrimaryWeightText_']");
        var sum = 0;
        $.each(listOfAttributePercentValues, function (key, value) {
            sum += parseInt(value.value.split("%")[0])
        });
        if (sum == 100) {


            var matchingCriteriaValue = parseInt($("#smmergeduplicateFilterMatchWeightText").val().split("%")[0]);
            if (matchingCriteriaValue <= 0 || matchingCriteriaValue > 100) {
                smMergeDuplicate.controls.smmergeduplicateErrorMsgText().addClass("smmergeduplicateErrorColor");
                smMergeDuplicate.controls.smmergeduplicateErrorMsgText().text("Please select confidence value b/w 1 and 100");
                smMergeDuplicate.hideText(smMergeDuplicate.controls.smmergeduplicateErrorMsgText());
            }



            else {
                smMergeDuplicate.fetchDuplicateSecurities();
                event.stopPropagation();

            }
        }

        else {
            smMergeDuplicate.controls.smmergeduplicateErrorMsgText().addClass("smmergeduplicateErrorColor");
            smMergeDuplicate.controls.smmergeduplicateErrorMsgText().text("Sum of matching criteria % should be equal 100% , it is " + sum + "% right now!");
            smMergeDuplicate.hideText(smMergeDuplicate.controls.smmergeduplicateErrorMsgText());
        }





    },
    onClickCreatePreset: function (e) {
        var top = smMergeDuplicate.controls.smmergeduplicateSaveBtn().offset().top + 30;
        var left = smMergeDuplicate.controls.smmergeduplicateSaveBtn().offset().left;
        left -= 200;
        var presetName = smMergeDuplicate.controls.smmergeduplicateSaveBtn().attr("preset_name");
        if (presetName === undefined) {
            presetName = "";
        }

        var html = "<div id='smmergeduplicateCreatePresetDiv' class='smmergeduplicateCreatePresetDivStyle' style='top:" + top + "px;left:" + left + "px;'>";
        html += "<input type='text' placeholder='Enter Name' id='smmergeduplicatePresetNameInput' class='smmergeduplicatePresetNameStyle' value='" + presetName + "' >";
        html += "<i class='fa fa-arrow-circle-right smmergeduplicatePresetCheckStyle' id='smmergeduplicateCheckBtn'></i>";
        html += "<div>";

        $("#smmergeduplicateCreatePresetDiv").remove();
        $("body").append(html);

        $("#smmergeduplicateCheckBtn").unbind('click').bind('click', function () {
            smMergeDuplicate.controls.smmergeduplicateSaveBtn().attr("preset_name", $("#smmergeduplicatePresetNameInput").val().trim());
            smMergeDuplicate.savePreset();
            $("#smmergeduplicateCreatePresetDiv").hide();
        });

        $("#smmergeduplicatePresetNameInput").unbind('click').bind('click', function (e) {
            e.stopPropagation();
        });

        e.stopPropagation();
    },
    savePreset: function () {
        var obj = smMergeDuplicate.getConfigValues();

        $.ajax({
            type: 'POST',
            url: smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + (smMergeDuplicate._productName == "refmaster" ? "/SaveRMPreset" : "/SavePreset"),
            data: JSON.stringify({ "configValues": obj }),
            contentType: "application/json",
            dataType: "json",
            success: function (data) {
                if (data.d === "success") {
                    var message = "Preset has been successfully created"
                    smMergeDuplicate.controls.smmergeduplicateErrorMsgText().addClass("smmergeduplicateSuccessColor");
                    smMergeDuplicate.controls.smmergeduplicateErrorMsgText().text(message);
                    smMergeDuplicate.hideText(smMergeDuplicate.controls.smmergeduplicateErrorMsgText());
                }
                else {

                    var message;
                    if (data.d)
                        message = data.d;
                    else
                        message = "An error occured while saving preset."
                    smMergeDuplicate.controls.smmergeduplicateErrorMsgText().addClass("smmergeduplicateErrorColor");
                    smMergeDuplicate.controls.smmergeduplicateErrorMsgText().text(message);
                    smMergeDuplicate.hideText(smMergeDuplicate.controls.smmergeduplicateErrorMsgText());
                }

                smMergeDuplicate.bindSavedFilter();
            },
            error: function (data) {
                console.log("Attributes cannot be fetched");

            }
        });
    },
    onClickMergeDupes: function (e) {
        if ((smMergeDuplicate._selectedSecuritiesFromGrid.length > 0)) {
            if (!parseInt(smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().css("margin-left")) < 0) {
                smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().click();
            }
            onServiceUpdating();

            //data: JSON.stringify({ "sectypeNames": smMergeDuplicate._selectedSectypeNamesFromGrid, "secIds": smMergeDuplicate._selectedSecuritiesFromGrid, "username": "admin" }),

            $.ajax({
                method: "POST",
                url: smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + (smMergeDuplicate._productName == "refmaster" ? "/GetDupeEntityData" : "/GetSecurityData"),
                data: JSON.stringify({ "sectypeNames": smMergeDuplicate._selectedSectypeNamesFromGrid, "secIds": smMergeDuplicate._selectedSecuritiesFromGrid, "username": smMergeDuplicate._username }),
                contentType: "application/json",
                dataType: "json",
                data: JSON.stringify({ "secIds": smMergeDuplicate._selectedSecuritiesFromGrid }),
                success: function (data) {
                    smMergeDuplicate._duplicateSecuritiesData = data.d;

                    smMergeDuplicate.controls.smmergeduplicatesGrid().hide();
                    smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().show();
                    smMergeDuplicate.smMergeSecuritiesGrid(smMergeDuplicate._duplicateSecuritiesData, smMergeDuplicate._duplicateSecuritiesData.columns);
                    smMergeDuplicate._finalMergeSelectedSectype = smMergeDuplicate._duplicateSecuritiesData.sectypeIds;
                    smMergeDuplicate._dicSectypeVsSecId = smMergeDuplicate._duplicateSecuritiesData.sectypeVsSecid;
                },
                error: function () {
                    console.log("Error");
                },
                complete: function () {
                    onServiceUpdated();
                    if (smMergeDuplicate.controls.smmergeduplicateMergeDupesOption().length !== 0) {
                        smMergeDuplicate.controls.smmergeduplicateMergeDupesOption().unbind('click').bind('click', smMergeDuplicate.onClickMergeDupesOption);
                    }
                }
            });
        }
        else {

        }

        e.stopPropagation();
    },
    onClickMergeDupesCloseBtn: function (e) {
        smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().fadeOut();
        smMergeDuplicate.controls.smmergeduplicatesGrid().fadeIn();
        smMergeDuplicate.controls.smmergeduplicatesGrid().css('display', 'inline-block');

        e.stopPropagation();
    },
    onClickDuplicatesGridContainer: function (e) {
        e.stopPropagation();
        var target = $(e.target);
        if (target[0].type === "checkbox") {
            var parentContainer = target.closest(".sm_grid_container");
            var siblingContainers = parentContainer.siblings();
            if (siblingContainers.find('input[type="checkbox"]').is(":checked") === true) {
                siblingContainers.find('input[type="checkbox"]').attr("checked", false);
                smMergeDuplicate._selectedSecuritiesFromGrid = [];
                smMergeDuplicate._selectedSectypeNamesFromGrid = [];
            }

            if (target.parent().hasClass("sm_grid_header_col")) {
                parentContainer.find(".sm_grid_row").each(function (ii, ee) {
                    var sId = $(ee).attr("sec_id");
                    var sTypeName = $(ee).attr("sectype_name");

                    if (target[0].checked) {
                        if ($.inArray(sId, smMergeDuplicate._selectedSecuritiesFromGrid) === -1) {
                            smMergeDuplicate._selectedSecuritiesFromGrid.push(sId.trim());
                        }
                        parentContainer.find('input[type="checkbox"]').prop("checked", true);
                    }
                    else {
                        smMergeDuplicate._selectedSecuritiesFromGrid = $.grep(smMergeDuplicate._selectedSecuritiesFromGrid, function (value) {
                            return value != sId;
                        });
                        parentContainer.find('input[type="checkbox"]').prop("checked", false);
                    }
                });
            }
            else {
                var rowInstance = target.closest(".sm_grid_row");
                var securityId = rowInstance.attr("sec_id");
                var securityTypeName = rowInstance.attr("sectype_name");

                if (target[0].checked) {
                    if ($.inArray(securityId, smMergeDuplicate._selectedSecuritiesFromGrid) === -1) {
                        smMergeDuplicate._selectedSecuritiesFromGrid.push(securityId.trim());
                    }
                }
                else {
                    smMergeDuplicate._selectedSecuritiesFromGrid = $.grep(smMergeDuplicate._selectedSecuritiesFromGrid, function (value) {
                        return value != securityId;
                    });
                }

                //handling for unchecking/checking stuff for all
                var gridContainerSelect = target.closest(".sm_grid_grid_container");

                //find no. of total row-divs
                var totalRows = gridContainerSelect.find(".sm_grid_row").length;
                var selectedRows = smMergeDuplicate._selectedSecuritiesFromGrid.length;
                var headerSelectInput = gridContainerSelect.find('.sm_grid_header input[type="checkbox"]')
                if (totalRows == selectedRows)
                    headerSelectInput.prop("checked", true);
                else
                    headerSelectInput.prop("checked", false);
            }
        }

        if (target.hasClass("sm_grid_security")) {
            var secId = target.text().trim();
            if (smMergeDuplicate._productName == 'refmaster') {

                var currentEntityTypeID = smMergeDuplicate._selectedSectype;
                var entityTypeName;

                for (var i = 0; i < smMergeDuplicate._sectypeList.length; i++) {
                    if (smMergeDuplicate._sectypeList[i].value == currentEntityTypeID)
                        entityTypeName = smMergeDuplicate._sectypeList[i].text;
                }

                var containerPage = "App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?";
                var queryString = "entityTypeID=" + currentEntityTypeID + "&entityCode=" + secId + "&entityDisplayName=" + entityTypeName + "&";
                var uniqueId = "unique_dedupe_" + RefMasterJSCommon.RMSCommons.getGUID();
                var url = containerPage + queryString.trim() + "pageIdentifier=ViewEntityFromSearch&tabFrameId=" + uniqueId + "&RMTabUniqueID=" + uniqueId;
                RefMasterJSCommon.RMSCommons.createTab("RefM_ViewEntity", url, uniqueId, "View Entity: " + secId);
            }
            else {
                SecMasterJSCommon.SMSCommons.openWindowForSecurityWithHiglight(true, secId, "", "", true, true, SecMasterJSCommon.SMCreateUpdateOpenByDefault.None, 0, null);
            }
        }
    },
    onClickMergeDupesOption: function (e) {
        if (smMergeDuplicate.controls.smmergeduplicateMergeOptionContainer().length === 1 && smMergeDuplicate.controls.smmergeduplicateMergeOptionContainer().css("display") !== "none") {
            smMergeDuplicate.controls.smmergeduplicateMergeOptionContainer().hide();
        }
        else {
            var topPosition = smMergeDuplicate.controls.smmergeduplicateMergeDupesOption().offset().top + 20;
            var leftPosition = smMergeDuplicate.controls.smmergeduplicateMergeDupesOption().offset().left - 250;

            smMergeDuplicate.controls.smmergeduplicateMergeOptionContainer().css('top', topPosition + 'px');
            smMergeDuplicate.controls.smmergeduplicateMergeOptionContainer().css('left', leftPosition + 'px');
            smMergeDuplicate.controls.smmergeduplicateMergeOptionContainer().css('display', 'block');

            //$("body").append("<div class='smmergeduplicateoverlay'></div>");

            //To get only the securities that are selected
            var secArr = [];
            var secIdArr = [];
            secArr.push({ text: "Select One", value: "0" });
            for (var item in smMergeDuplicate._selectedSecuritiesFromGrid) {
                var tempObj = {};
                tempObj.text = smMergeDuplicate._selectedSecuritiesFromGrid[item];
                tempObj.value = smMergeDuplicate._selectedSecuritiesFromGrid[item];
                secArr.push(tempObj);
                secIdArr.push(tempObj.value);
            }

            //To get the sectype of only the securities that are selected
            var sectypeList = [];
            sectypeList.push({ text: "Select One", value: "0" });
            for (var item in smMergeDuplicate._sectypeList) {
                for (var i in smMergeDuplicate._finalMergeSelectedSectype) {
                    if (smMergeDuplicate._sectypeList[item].value === smMergeDuplicate._finalMergeSelectedSectype[i].toString()) {
                        sectypeList.push(smMergeDuplicate._sectypeList[item]);
                    }
                }
            }

            smMergeDuplicate.createSMSelectDropDown($("#smmergeduplicateMergeOptionSectypeDdn"), sectypeList, true, "150px", function () {
                $("#smselect_smmergeduplicateMergeOptionSectypeDdn").find(".smselectcon").css("width", "150px");
                $("#smselect_smmergeduplicateMergeOptionSectypeDdn").find(".smselectrun").css("width", "150px");
            }, (smMergeDuplicate._productName == "refmaster" ? "Select Entity Type" : "Select Security Type"), function (e) {
                var selectedOption = smselect.getSelectedOption($(e.currentTarget));
                if (selectedOption[0].text !== "Select One") {
                    smselect.disable($("#smselect_smmergeduplicateMergeOptionSecurityDdn"));
                }
                else {
                    smselect.enable($("#smselect_smmergeduplicateMergeOptionSecurityDdn"));
                }
            });
            //smselect.setOptionByText($("#smselect_smmergeduplicateMergeOptionSectypeDdn"), sectypeList[0].text);

            smMergeDuplicate.createSMSelectDropDown($("#smmergeduplicateMergeOptionSecurityDdn"), secArr, true, "150px", function () {
                $("#smselect_smmergeduplicateMergeOptionSecurityDdn").find(".smselectcon").css("width", "150px");
                $("#smselect_smmergeduplicateMergeOptionSecurityDdn").find(".smselectrun").css("width", "150px");
            }, (smMergeDuplicate._productName == "refmaster" ? "Entities" : "Securities"), null);
            smselect.setOptionByText($("#smselect_smmergeduplicateMergeOptionSecurityDdn"), secArr[1].text);

            /*$(".smmergeduplicateoverlay").unbind('click').bind('click', function () {
            $("#smmergeduplicateMergeOptionContainer").hide();
            $(this).hide();
            });
            */

            smMergeDuplicate._finalMergeData = [];
            var tempObj = [];

            //To get the data of the final merged security


            $.each(smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeSecRightRowsContanier").find(".smmergeduplicateMergeSecColumn"), function (innerIndex, innerEle) {
                var secId = "";
                var attributes = [];
                smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateAttrColClass").find(".smmergeduplicateMergeSecAttr").each(function (ii, ee) {
                    var rowNumber = $(ee).attr("row_number");
                    var attrName = $(ee).text().trim();
                    var attrValue = "";
                    smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeSecEditableCell").each(function (iii, eee) {
                        var rNumber = $(eee).attr("row_number");
                        if (rowNumber === rNumber) {
                            attrValue = $(eee).text().trim();
                        }
                    });

                    if ($(innerEle).find("div[row_number=" + rowNumber + "]").hasClass("smmergeduplicateHighlightedCell")) {
                        secId = $(innerEle).attr("sec_id");
                        var attrObj = {}
                        attrObj["Key"] = attrName;
                        attrObj["Value"] = attrValue;

                        attributes.push(attrObj);
                    }
                });
                if (secId != '')
                    tempObj.push({ Key: secId, Value: attributes });
            });
            smMergeDuplicate._finalMergeData = tempObj;

            //For Populating Legs
            if (sectypeList.length === 2) {
                smMergeDuplicate.getLegNamesForSectype(sectypeList[1]).then(function (data) {
                    var LegsHTML = "";
                    //For Binding Leg Names
                    for (var leg in data) {
                        LegsHTML += '<div legrow="' + leg + '" class="smmergeduplicateLegRow">';
                        LegsHTML += '<div class="smmergeduplicateHorizontalAlign smmergeduplicateLegName" legid="' + data[leg].Key + '" >' + data[leg].Value + '</div>';
                        LegsHTML += '<div id="smmergeLegSecurityDropDown_' + leg + '" class="smmergeduplicateHorizontalAlign smmergeduplicateLegSecurityDropDown"></div>';
                        LegsHTML += '</div>';
                    }
                    smMergeDuplicate.controls.smmergeduplicateLegsList().html(LegsHTML);

                    //For Binding DropDowns
                    for (var leg in data) {
                        var currDropDown = smMergeDuplicate.controls.smmergeduplicateLegsList().find('#smmergeLegSecurityDropDown_' + leg);
                        smMergeDuplicate.createSMSelectDropDown(currDropDown, secArr, true, "150px", function () {
                            $("#smselect_" + currDropDown.attr("id")).find(".smselectcon").css("width", "150px");
                            $("#smselect_" + currDropDown.attr("id")).find(".smselectrun").css("width", "150px");
                        }, "Securities", null);
                        smselect.setOptionByText($("#smselect_" + currDropDown.attr("id")), secArr[1].text);
                    }

                    smMergeDuplicate.controls.smmergeduplicateLegsList().css('display', '');
                });
            }

            $("#smmergeduplicateMergeOptionOkBtn").unbind("click").bind("click", function (e) {
                var sectypeIds = smselect.getSelectedOption($("#smselect_smmergeduplicateMergeOptionSectypeDdn"));
                var secIds = smselect.getSelectedOption($("#smselect_smmergeduplicateMergeOptionSecurityDdn"));
                var flag = true;
                var isCreate = false;
                var selectedSectype = "";
                var selectedSecId = "";
                if (sectypeIds.length !== 0) {
                    isCreate = true;
                    selectedSectype = sectypeIds[0].value;
                    for (var item in smMergeDuplicate._dicSectypeVsSecId) {
                        if (smMergeDuplicate._dicSectypeVsSecId[item].Key === parseInt(sectypeIds[0].value)) {
                            selectedSecId = smMergeDuplicate._dicSectypeVsSecId[item].Value[0];
                            break;
                        }
                    }
                }
                else {
                    isCreate = false;
                    selectedSecId = secIds[0].value;
                    for (var item in smMergeDuplicate._dicSectypeVsSecId) {
                        var index = smMergeDuplicate._dicSectypeVsSecId[item].Value.indexOf(secIds[0].value);
                        if (index > -1)
                            selectedSectype = smMergeDuplicate._dicSectypeVsSecId[item].Key;
                        else
                            flag = false;
                        break;
                    }
                }

                if (flag && selectedSecId !== "") {
                    onServiceUpdating();
                    var deleteSec = [], copyTS = false;
                    if (smMergeDuplicate.controls.smmergeduplicateDeleteOriginalCheckbox().is(":checked")) {
                        deleteSec = secIdArr;
                    }
                    if (smMergeDuplicate.controls.smmergeduplicateCopyTimeseriesCheckbox().is(":checked")) {
                        copyTS = true;
                    }

                    var deletionOption = 3;

                    if (smMergeDuplicate._productName == "refmaster") {

                        if (deleteSec.length > 0) {
                            if (!isCreate) {
                                var index = deleteSec.indexOf(selectedSecId);
                                if (index > -1)
                                    deleteSec.splice(index, 1);
                            }
                        }

                        if (deleteSec.length > 0) {
                            $.ajax({
                                method: "POST",
                                url: smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + "/CheckEntityHasDependency",
                                //url: smMergeDuplicate._path + smMergeDuplicate._serviceCommonSVCLocation + "/SetMergedEntityDetails",
                                data: JSON.stringify({ "entityCodes": deleteSec }),
                                contentType: "application/json",
                                dataType: "json",
                                success: function (res) {
                                    var hasDependency = res.d;
                                    if (hasDependency) {
                                        $get('loadingImg').style.display = 'none';
                                        $('#divDeleteEntities').css('display', '');
                                        $("#smmergeduplicateMergeOptionContainer").hide();
                                        $('#divCloseDeleteOptions').unbind('click').click(function () {
                                            $('#divDeleteEntities').css('display', 'none');
                                            onServiceUpdated();
                                        });
                                        $('#btnDeleteOptions').unbind('click').click(function () {

                                            deletionOption = $('input[name=smmergeduplicateDeleteEntityOptions]:checked', '#divDeleteEntities').prop('id').split('_')[1];
                                            if (deletionOption == '3')
                                                deleteSec = [];

                                            $('#divDeleteEntities').css('display', 'none');

                                            $.ajax({
                                                method: "POST",
                                                url: smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + "/SetMergedEntityDetails",
                                                //url: smMergeDuplicate._path + smMergeDuplicate._serviceCommonSVCLocation + "/SetMergedEntityDetails",
                                                data: JSON.stringify({ "isCreate": isCreate, "sectypeId": selectedSectype, "secId": selectedSecId, "attributeData": smMergeDuplicate._finalMergeData, "legNameVsSecId": smMergeDuplicate.fetchLegNameVsSecId(), "deleteSecurities": deleteSec, "copyTS": copyTS, "deletionOption": deletionOption }),
                                                contentType: "application/json",
                                                dataType: "json",
                                                success: function (res) {
                                                    var guid = res.d;

                                                    if (isCreate) {
                                                        var containerPage = "App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?";
                                                        var queryString = "entityTypeID=" + selectedSectype + "&";
                                                        var mergeid = "mergeuniqueid=" + guid + "&";
                                                        var uniqueId = "unique_edit_" + RefMasterJSCommon.RMSCommons.getGUID();
                                                        var url = containerPage + queryString.trim() + mergeid.trim() + "pageIdentifier=CreateEntityFromMergeDupes&tabFrameId=" + uniqueId + "&RMTabUniqueID=" + uniqueId;
                                                        RefMasterJSCommon.RMSCommons.createTab("RefM_CreateEntity", url, uniqueId, "Create Entity");
                                                    }
                                                    else {
                                                        var containerPage = "App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?";
                                                        var queryString = "entityTypeID=" + selectedSectype + "&entityCode=" + selectedSecId + "&";
                                                        var mergeid = "mergeuniqueid=" + guid + "&";
                                                        var uniqueId = "unique_edit_" + RefMasterJSCommon.RMSCommons.getGUID();
                                                        var url = containerPage + queryString.trim() + mergeid.trim() + "pageIdentifier=UpdateEntityFromMergeDupes&tabFrameId=" + uniqueId + "&RMTabUniqueID=" + uniqueId;
                                                        RefMasterJSCommon.RMSCommons.createTab("RefM_UpdateEntity", url, uniqueId, "Edit Entity " + selectedSecId);
                                                    }
                                                    $("#smmergeduplicateMergeOptionContainer").hide();
                                                },
                                                error: function (jqXHR, textStatus, errorThrown) {
                                                    //alert(errorThrown);
                                                    console.log("Error");
                                                },
                                                complete: function () {
                                                    onServiceUpdated();
                                                }
                                            });

                                        });

                                    }

                                    else {

                                        $.ajax({
                                            method: "POST",
                                            url: smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + "/SetMergedEntityDetails",
                                            //url: smMergeDuplicate._path + smMergeDuplicate._serviceCommonSVCLocation + "/SetMergedEntityDetails",
                                            data: JSON.stringify({ "isCreate": isCreate, "sectypeId": selectedSectype, "secId": selectedSecId, "attributeData": smMergeDuplicate._finalMergeData, "legNameVsSecId": smMergeDuplicate.fetchLegNameVsSecId(), "deleteSecurities": deleteSec, "copyTS": copyTS, "deletionOption": deletionOption }),
                                            contentType: "application/json",
                                            dataType: "json",
                                            success: function (res) {
                                                var guid = res.d;

                                                if (isCreate) {
                                                    var containerPage = "App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?";
                                                    var queryString = "entityTypeID=" + selectedSectype + "&";
                                                    var mergeid = "mergeuniqueid=" + guid + "&";
                                                    var uniqueId = "unique_edit_" + RefMasterJSCommon.RMSCommons.getGUID();
                                                    var url = containerPage + queryString.trim() + mergeid.trim() + "pageIdentifier=CreateEntityFromMergeDupes&tabFrameId=" + uniqueId + "&RMTabUniqueID=" + uniqueId;
                                                    RefMasterJSCommon.RMSCommons.createTab("RefM_CreateEntity", url, uniqueId, "Create Entity");
                                                }
                                                else {
                                                    var containerPage = "App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?";
                                                    var queryString = "entityTypeID=" + selectedSectype + "&entityCode=" + selectedSecId + "&";
                                                    var mergeid = "mergeuniqueid=" + guid + "&";
                                                    var uniqueId = "unique_edit_" + RefMasterJSCommon.RMSCommons.getGUID();
                                                    var url = containerPage + queryString.trim() + mergeid.trim() + "pageIdentifier=UpdateEntityFromMergeDupes&tabFrameId=" + uniqueId + "&RMTabUniqueID=" + uniqueId;
                                                    RefMasterJSCommon.RMSCommons.createTab("RefM_UpdateEntity", url, uniqueId, "Edit Entity " + selectedSecId);
                                                }
                                                $("#smmergeduplicateMergeOptionContainer").hide();
                                            },
                                            error: function (jqXHR, textStatus, errorThrown) {
                                                //alert(errorThrown);
                                                console.log("Error");
                                            },
                                            complete: function () {
                                                onServiceUpdated();
                                            }
                                        });


                                    }


                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    //alert(errorThrown);
                                    console.log("Error");
                                },
                                complete: function () {
                                    //onServiceUpdated();
                                }
                            });


                        }
                        else {
                            $.ajax({
                                method: "POST",
                                url: smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + "/SetMergedEntityDetails",
                                //url: smMergeDuplicate._path + smMergeDuplicate._serviceCommonSVCLocation + "/SetMergedEntityDetails",
                                data: JSON.stringify({ "isCreate": isCreate, "sectypeId": selectedSectype, "secId": selectedSecId, "attributeData": smMergeDuplicate._finalMergeData, "legNameVsSecId": smMergeDuplicate.fetchLegNameVsSecId(), "deleteSecurities": deleteSec, "copyTS": copyTS, "deletionOption": deletionOption }),
                                contentType: "application/json",
                                dataType: "json",
                                success: function (res) {
                                    var guid = res.d;
                                    if (isCreate) {
                                        var containerPage = "App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?";
                                        var queryString = "entityTypeID=" + selectedSectype + "&";
                                        var mergeid = "mergeuniqueid=" + guid + "&";
                                        var uniqueId = "unique_edit_" + RefMasterJSCommon.RMSCommons.getGUID();
                                        var url = containerPage + queryString.trim() + mergeid.trim() + "pageIdentifier=CreateEntityFromMergeDupes&tabFrameId=" + uniqueId + "&RMTabUniqueID=" + uniqueId;
                                        RefMasterJSCommon.RMSCommons.createTab("RefM_CreateEntity", url, uniqueId, "Create Entity");
                                    }
                                    else {
                                        var containerPage = "App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?";
                                        var queryString = "entityTypeID=" + selectedSectype + "&entityCode=" + selectedSecId + "&";
                                        var mergeid = "mergeuniqueid=" + guid + "&";
                                        var uniqueId = "unique_edit_" + RefMasterJSCommon.RMSCommons.getGUID();
                                        var url = containerPage + queryString.trim() + mergeid.trim() + "pageIdentifier=UpdateEntityFromMergeDupes&tabFrameId=" + uniqueId + "&RMTabUniqueID=" + uniqueId;
                                        RefMasterJSCommon.RMSCommons.createTab("RefM_UpdateEntity", url, uniqueId, "Edit Entity " + selectedSecId);
                                    }
                                    $("#smmergeduplicateMergeOptionContainer").hide();
                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    //alert(errorThrown);
                                    console.log("Error");
                                },
                                complete: function () {
                                    onServiceUpdated();
                                }
                            });
                        }
                    }
                    else {
                        $.ajax({
                            method: "POST",
                            url: smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + "/MergeSecurities",
                            data: JSON.stringify({ "isCreate": isCreate, "sectypeId": selectedSectype, "secId": selectedSecId, "attributeData": smMergeDuplicate._finalMergeData, "legNameVsSecId": smMergeDuplicate.fetchLegNameVsSecId(), "deleteSecurities": deleteSec, "copyTS": copyTS }),
                            contentType: "application/json",
                            dataType: "json",
                            success: function (data) {
                                var response = data.d;
                                if (response.indexOf("|") !== -1) {
                                    var status = response.split("|");
                                    if (status[0] === "0" && status[1].toLowerCase() === "cannot delete security") {
                                        smMergeDuplicate.setSecurityIDsInSession(deleteSec.join(","), function () {
                                            var url = "../SMDeleteSecurity.aspx?source=deduplication";
                                            var popupHeight = (0.8 * smMergeDuplicate._windowHeight);
                                            var popupWidth = (0.8 * smMergeDuplicate._windowWidth);
                                            $("#smmergeduplicateDeleteSecurityPopup").show();
                                            $("#smmergeduplicateDeleteSecurityPopup").animate({ "height": popupHeight + "px" }, 500, function () {
                                                $("#smmergeduplicateDeleteSecurityPopupIframe").attr("src", url);
                                            });

                                            //Close Btn Click
                                            $("#smmergeduplicateDeleteSecurityPopupCloseBtn").unbind("click").bind("click", function (e) {
                                                $("#smmergeduplicateDeleteSecurityPopup").animate({ "height": "0px" }, 500, function () {
                                                    $("#smmergeduplicateDeleteSecurityPopup").hide();
                                                });
                                            });
                                        });
                                    }
                                }
                                else {
                                    var guid = response;
                                    SecMasterJSCommon.SMSCommons.openWindowMergeSecurity(isCreate, selectedSecId, guid, true, true, '');
                                    $("#smmergeduplicateMergeOptionContainer").hide();
                                }
                            },
                            error: function () {
                                console.log("Error");
                            },
                            complete: function () {
                                onServiceUpdated();
                            }
                        });
                    }
                }
                else {
                    if (smMergeDuplicate._productName == "refmaster")
                        $("#smmergeduplicateMergeOptionContainer").find(".smmergeduplicateErrorDiv").text("Please select Entity of the same Entity type.");
                    else
                        $("#smmergeduplicateMergeOptionContainer").find(".smmergeduplicateErrorDiv").text("Please select Security of the same Security type.");
                }
                e.stopPropagation();
            });
        }

        e.stopPropagation();
    },
    getLegNamesForSectype: function (sectypeID) {
        return new Promise(function (resolve, reject) {
            $.ajax({
                method: "POST",
                url: smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + (smMergeDuplicate._productName == "refmaster" ? "/GetEntityTypeLegNames" : "/GetLegNames"),
                data: JSON.stringify({ "sectypeId": sectypeID.value }),
                contentType: "application/json",
                dataType: "json",
                success: function (data) {
                    resolve(data.d);
                },
                error: function () {
                    console.log("Error");
                }
            })
        });
    },
    fetchLegNameVsSecId: function () {
        var returnArr = [];
        smMergeDuplicate.controls.smmergeduplicateLegsList().children("div").each(function (index, ele) {
            var legName = $(ele).find(".smmergeduplicateLegName").text().trim();
            var secId = smselect.getSelectedOption($(ele).find("#smselect_" + 'smmergeLegSecurityDropDown_' + index))[0].value;
            var tempObj = {};
            tempObj["Key"] = legName;
            tempObj["Value"] = secId;
            returnArr.push(tempObj);
        });

        return returnArr;
    },
    onFocusOutInput: function () {
        $(".smmergeduplicateFocusOut").on("focusout", function (event) {
            var target = $(event.target);
            var currentValue = target.val();
            if (currentValue.indexOf("%") === -1) {
                target.val(currentValue + '%');
            }
        });
    },
    getConfigValues: function () {
        if (smMergeDuplicate.controls.smmergeduplicateHdnSelectedFilter().val() !== "0") {
            return {
                configName: "",
                moduleId: 0,
                sectypeId: smMergeDuplicate._selectedDupesSectype,
                matchConfidence: 0,
                attrList: null,
                entityCodes: null
            };
        }
        else {
            var configName = smMergeDuplicate.controls.smmergeduplicateSaveBtn().attr("preset_name");
            var module_name = smMergeDuplicate.controls.smmergeduplicateChooseProduct().attr("selected_module");
            var finalMatchConfidenceText = parseInt(smMergeDuplicate.controls.smmergeduplicateFilterMatchWeightText().val().split("%")[0]);
            /*var sectypeId = [];
            $.each(smselect.getSelectedOption($("#smselect_" + smMergeDuplicate.controls.smmergeduplicateSectypeDropDown().attr("id"))), function (ii, ee) {
            sectypeId.push(ee.value);
            })*/
            var attrFilterList = smMergeDuplicate.getAttributeFilterList();
            var filteredEntityCodes = '';
            var filteredCodeList = null;
            if (smMergeDuplicate._presetValue != '' && smMergeDuplicate._presetValue != null && smMergeDuplicate._presetValue != undefined) {
                var codes = smMergeDuplicate._presetValue.split('||')[1];
                filteredCodeList = codes.split(',');
            }
            return {
                configName: (configName === undefined ? "" : configName),
                moduleId: 1,
                sectypeId: smMergeDuplicate._selectedDupesSectype,
                matchConfidence: finalMatchConfidenceText,
                attrList: attrFilterList,
                entityCodes: filteredCodeList
            };
        }
    },
    getMatchTypeList: function () {
        $.ajax({
            method: "POST",
            url: smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + "/GetDeDuplicateMatchTypeList",
            success: function (data) {
                smMergeDuplicate._matchType = data.d;
            },
            error: function () {
                console.log("Error");
            },
            complete: function () {
                if (smMergeDuplicate._productName == 'refmaster')
                    smMergeDuplicate._fromMatchTypeLandingPage = true;
                //To Initialisze Primary DropDowns

                smMergeDuplicate.initDropDowns();

                //unset
                smMergeDuplicate._fromMatchTypeLandingPage = false;

                if (smMergeDuplicate._presetValue == null || smMergeDuplicate._presetValue == undefined || smMergeDuplicate._presetValue == '')
                    onServiceUpdated();
            }
        });
    },
    getModuleList: function (callback) {
        $.ajax({
            method: "POST",
            url: smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + "/GetDeDuplicateModuleList",
            success: function (data) {
                if (smMergeDuplicate._productName == "refmaster")
                    smMergeDuplicate._productList.modules = data.d.filter(function (item) { if (item.text.toLowerCase() === "refmaster") return item; });
                else
                    smMergeDuplicate._productList.modules = data.d.splice(0, 2); // Splice for removing corpaction from the modules list

                if (typeof callback === "function") {
                    callback();
                }
            },
            error: function () {
                console.log("Error");
            }
        });
    },
    getAttributeFilterList: function () {
        var finalAttrFilterList = [];
        // parseInt(smMergeDuplicate.controls.smmergeduplicateAttributesContainer().attr("row_number"));
        var noOfRows = smMergeDuplicate.controls.smmergeduplicateAttributesContainer().find('div[row]').length;
        /*for (var i = 1; i <= noOfRows; i++) {
        
        }*/
        $.each(smMergeDuplicate.controls.smmergeduplicateAttributesContainer().find('div[row]'), function (index, element) {
            var rowNumber = parseInt($(element).attr('row'));
            var tempObj = {};
            var attr_id = parseInt(smselect.getSelectedOption($("#smmergeduplicatePrimaryAttributeDropDown_" + rowNumber))[0].value);
            tempObj["attributeId"] = attr_id;
            var match_type_id = parseInt(smselect.getSelectedOption($("#smmergeduplicatePrimaryMatchTypeDropDown_" + rowNumber))[0].value);
            tempObj["matchTypeId"] = match_type_id;
            var weight = parseInt($("#smmergeduplicatePrimaryWeightText_" + rowNumber).val().split("%")[0]);
            tempObj["matchPercentage"] = weight;

            var attrDataType = smMergeDuplicate.getSelectedAttributeDataType(attr_id);

            var ddOptions = [];
            if ((attrDataType === "NUMERIC" || attrDataType === "DATE" || attrDataType === "DATETIME" || attrDataType === "TIME" || attrDataType.toLowerCase() === "decimal" || attrDataType.toLowerCase() == "int") && (match_type_id == "2")) {
                if (attrDataType.toLowerCase() == "decimal" || attrDataType.toLowerCase() == "int")
                    attrDataType = "NUMERIC";
                if (attrDataType.toLowerCase() == "datetime")
                    attrDataType = "DATE";
                //tempObj["toleranceValue"] = parseInt($("#smmergeduplicateToleranceInputRow_" + rowNumber).val());
                tempObj["toleranceValue"] = parseInt($("#smmergeduplicateToleranceInputRow_" + rowNumber).val());
                //var selectedOptionText = $("#smmergeduplicateToleranceDropdownRow_" + rowNumber).val();
                var selectedOptionText = smselect.getSelectedOption($("#smmergeduplicateToleranceDropdownRow_" + rowNumber))[0].text;
                var selectedOptionArr = smMergeDuplicate._toleranceOptions.find(function (dataType) {
                    return dataType.Key === attrDataType;
                });
                var selOption = selectedOptionArr.Value.find(function (item) {
                    if (item.text.toLowerCase() === selectedOptionText.toLowerCase()) {
                        return item.value;
                    }
                });
                tempObj["toleranceOptionSelected"] = parseInt(selOption.value);
            }

            //if ((attrDataType === "NUMERIC" || attrDataType === "DATE" || attrDataType === "DATETIME" || attrDataType === "TIME") && (match_type_id == "2")) {
            //    //tempObj["toleranceValue"] = parseInt($("#smmergeduplicateToleranceInputRow_" + rowNumber).val());
            //    tempObj["toleranceValue"] = parseInt($("#smmergeduplicateToleranceInputRow_" + rowNumber).text());
            //    var selectedOptionText = $("#smmergeduplicateToleranceDropdownRow_" + rowNumber).text();
            //    var selectedOptionArr = smMergeDuplicate._toleranceOptions.find(function (dataType) {
            //        return dataType.Key === attrDataType;
            //    });
            //    var selOption = selectedOptionArr.Value.find(function (item) {
            //        if (item.text.toLowerCase() === selectedOptionText.toLowerCase()) {
            //            return item.value;
            //        }
            //    });
            //    tempObj["toleranceOptionSelected"] = selOption.value;
            //}



            finalAttrFilterList.push(tempObj);
        });

        return finalAttrFilterList;
    },
    createAttributeSelectionDivRow: function () {
        var selectedAttr = smselect.getSelectedOption($("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")));
        var selectedMatchType = smselect.getSelectedOption($("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown().attr("id")));
        var selectedWeight = smMergeDuplicate.controls.smmergeduplicatePrimaryWeightText().val().split("%")[0];

        if (selectedAttr.length !== 0 && selectedMatchType.length !== 0 && (selectedWeight !== "" || selectedWeight !== "%" || selectedWeight !== "0" || selectedWeight !== 0) && ($.inArray(selectedAttr[0].value, smMergeDuplicate._selectedAttributes) === -1) && (smMergeDuplicate._currectMatchWeight - parseInt(selectedWeight) >= 0)) {
            $("#smmergeduplicateErrorMsgTextFirstPage").hide();
            smMergeDuplicate._selectedAttributes.push(selectedAttr[0].value);
            //To Subtract the weight from primary textbox
            smMergeDuplicate._currectMatchWeight = smMergeDuplicate._currectMatchWeight - parseInt(selectedWeight);
            smMergeDuplicate.controls.smmergeduplicatePrimaryWeightText().val(smMergeDuplicate._currectMatchWeight + "%");

            //Reset DropDowns
            smselect.reset($("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")));
            smselect.reset($("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown().attr("id")));
            smselect.setOptionByText($("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown().attr("id")), "Exact Match");

            var maxRow = -1;
            $.each($("#smmergeduplicateAttributesContainer").find('div[row]'), function (index, element) {
                var temp = parseInt($(element).attr('row'));
                if (temp > maxRow)
                    maxRow = temp;
            });

            var rowNumber;

            if (maxRow == -1)
                rowNumber = parseInt(smMergeDuplicate.controls.smmergeduplicateAttributesContainer().attr('row_number')) + 1;
            else
                rowNumber = maxRow + 1;

            //LOGIC TO ANIMATE
            if (rowNumber > 0) {
                $("#smmergedFindDuplicatesContainer").animate({ "padding-top": '30px', "padding-bottom": '0px' }, 500, function () {
                    //$("#smmergedFindDuplicatesContainer").css({ "padding-top": '30px', "padding-bottom": '0px' });
                    $("#smmergedFindDuplicatesContainer").removeClass('smmergedFirstPageStyle');
                    $("#smmergedFindDuplicatesContainer").addClass('smmergeTopSection');
                    $("#smmergeduplicateSectypeDropDown .smselectanchorrun").css('font-size', '20px');
                    smMergeDuplicate.controls.smmergeduplicatePrimaryWeightText().parent().width(smMergeDuplicate.getInputElementWidth());
                    smMergeDuplicate.addAttributeRow(rowNumber, selectedWeight, selectedAttr[0].value, selectedMatchType[0].value);
                    $("#smmergedBottomDisplayPart").show();
                    smMergeDuplicate.controls.smmergeduplicatePrimaryWeightText().parent().width(smMergeDuplicate.getInputElementWidth());
                    $("#smmergeduplicatePrimaryAttributeDropDown .smselectanchorrun").css('font-size', '14px');
                    $("#smmergeduplicatePrimaryMatchTypeDropDown .smselectanchorrun").css('font-size', '14px');
                    $("#toleranceDropdown smselectanchorrun").css('font-size', '14px');

                    var totalWidthOfContent = $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectanchorcontainer").width() + $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectimage ").width();
                    var totalWidth = $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectrun").width();
                    $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectrun").css("padding-left", ((totalWidth - totalWidthOfContent - 2) / 2));
                    $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectrun").css("padding-right", ((totalWidth - totalWidthOfContent - 2) / 2));





                });
            }
            //smMergeDuplicate.addAttributeRow(rowNumber, selectedWeight, selectedAttr[0].value, selectedMatchType[0].value);
        }
        else {

            var message = "Please enter valid values";

            //Error message in case same attribute is added again
            if (selectedAttr[0]) {
                if ($.inArray(selectedAttr[0].value, smMergeDuplicate._selectedAttributes) !== -1) {
                    message = "Matching criteria on " + selectedAttr[0].text + " attribute already exists";
                }
            }


            if ($(".smmergeTopSection").length > 0) {
                smMergeDuplicate.controls.smmergeduplicateErrorMsgText().addClass("smmergeduplicateErrorColor");
                smMergeDuplicate.controls.smmergeduplicateErrorMsgText().text(message);
                smMergeDuplicate.hideText(smMergeDuplicate.controls.smmergeduplicateErrorMsgText());
            }
            else {


                if (parseInt(selectedWeight) <= 0 || parseInt(selectedWeight) > 100)
                    message = "Please enter weight b/w 1 - 100%";
                if (!selectedAttr[0])
                    message = "Please select an attribute value";
                var selectedOptions = smselect.getSelectedOption($('#smselect_smmergeduplicateSectypeDropDown'));
                if (!selectedOptions.length && smMergeDuplicate._productName.toLowerCase() === 'refmaster') {
                    message = "Please select an entity type first";
                }

                var errorMsgFirstPg = $("#smmergeduplicateErrorMsgTextFirstPage");
                errorMsgFirstPg.show();
                errorMsgFirstPg.addClass("smmergeduplicateErrorColor");
                errorMsgFirstPg.text(message);
                setTimeout(function () {
                    errorMsgFirstPg.text(""); errorMsgFirstPg.hide();
                }, 3000);
            }


            //smmergeduplicateErrorMsgTextFirstPage

        }
    },
    createSMSelectDropDown: function (dropDownId, smdata, showSearch, width, callback, heading, onChangeHandler, selectedItems, isMulti, multiText) {
        var obj = {};
        obj.container = dropDownId;
        obj.id = dropDownId.attr("id");
        if (heading !== null) {
            obj.heading = heading;
        }
        obj.data = smdata;
        if (showSearch) {
            obj.showSearch = true;
        }
        if (isMulti) {
            obj.isMultiSelect = true;
            obj.text = multiText;
        }
        if (selectedItems !== undefined && selectedItems.length !== 0) {
            obj.selectedItems = selectedItems;
        }
        obj.isRunningText = false;
        obj.ready = function (e) {
            e.width(width + "px");
            if (typeof onChangeHandler === "function") {
                e.on('change', function (ee) {
                    onChangeHandler(ee);
                });
            }
        }
        smselect.create(obj);

        $("#smselect_" + dropDownId.attr("id")).find(".smselect").css('text-align', 'left');
        $("#smselect_" + dropDownId.attr("id")).find(".smselectanchorcontainer2").css('width', '86%');
        if ($("#smselect_" + dropDownId.attr("id")).find(".smselectanchorcontainer2").parent().parent().attr("id") === "smselect_smmergeduplicatePrimaryMatchTypeDropDown") {
            //center aligning DD
            $("#smselect_" + dropDownId.attr("id")).find(".smselectanchorcontainer2").css('width', 'auto');
            //$("#smselect_" + dropDownId.attr("id")).find(".smselectanchorcontainer2").parent().css('width', '50%');
            //Con being center aligned
            $("#smselect_" + dropDownId.attr("id")).find(".smselectcon").css('margin-left', '20%');

        }

        //first screen setup align
        //var totalWidthOfContent = $("#smselect_" + dropDownId.attr("id")).find(".smselectanchorcontainer").width() + $("#smselect_" + dropDownId.attr("id")).find(".smselectimage ").width();
        //var totalWidth = $("#smselect_" + dropDownId.attr("id")).find(".smselectrun").width();
        //$("#smselect_" + dropDownId.attr("id")).find(".smselectrun").css("padding-left", ((totalWidth - totalWidthOfContent - 2) / 2));
        //$("#smselect_" + dropDownId.attr("id")).find(".smselectrun").css("padding-right", ((totalWidth - totalWidthOfContent - 2) / 2));


        if (typeof callback === "function") {
            callback();
        }
    },
    onClickDeleteAttributeRow: function (event) {
        var target = $(event.target);
        var noOfRows = parseInt(target.parent().parent().parent().attr('row_number'));
        noOfRows = noOfRows - 1;
        target.parent().parent().parent().attr('row_number', noOfRows);
        //target.parent().parent().parent().attr('row_number') = noOfRows;
        //Logic to Animate Back to first screen
        //1 because of match critera row
        if (noOfRows == 0) {

            var paddingHeightForFirstScreen = ($(window).height() - $("#srm_moduleTabs").height() - $("#smmergedFindDuplicatesContainer").height() - 30 - 25 - 110) / 2;
            $("#smmergedFindDuplicatesContainer").animate({
                "padding-top": paddingHeightForFirstScreen - 65, "padding-bottom": paddingHeightForFirstScreen + 65
            }, 500, function () {
                //var paddingHeightForFirstScreen = ($(window).height() - $("#srm_moduleTabs").height() - $("#smmergedFindDuplicatesContainer").height() - 30) / 2;
                //$("#smmergedFindDuplicatesContainer").css({ "padding-top": paddingHeightForFirstScreen, "padding-bottom": paddingHeightForFismmergeduplicateSectypeDropDownrstScreen });
                $("#smmergedFindDuplicatesContainer").removeClass('smmergeTopSection');
                $("#smmergedFindDuplicatesContainer").addClass('smmergedFirstPageStyle');
                //changes
                //$(".smmergedTopFilterBackground .smselectanchorrun").css("font-size", "2px");


                $("#smmergeduplicateSectypeDropDown .smselectanchorrun").css('font-size', '24px');
                $("#smmergeduplicateSectypeDropDown .smselectanchorrun").css('letter-spacing', '1px');

                $("#smmergeduplicatePrimaryAttributeDropDown .smselectanchorrun").css('font-size', '18px');
                $("#smmergeduplicatePrimaryMatchTypeDropDown .smselectanchorrun").css('font-size', '18px');


                var totalWidthOfContent = $("#smselect_smmergeduplicatePrimaryAttributeDropDown").find(".smselectanchorcontainer").width() + $("#smselect_smmergeduplicatePrimaryAttributeDropDown").find(".smselectimage ").width();
                var totalWidth = $("#smselect_smmergeduplicatePrimaryAttributeDropDown").find(".smselectrun").width();
                $("#smselect_smmergeduplicatePrimaryAttributeDropDown").find(".smselectrun").css("padding-left", ((totalWidth - totalWidthOfContent - 2) / 2));
                $("#smselect_smmergeduplicatePrimaryAttributeDropDown").find(".smselectrun").css("padding-right", ((totalWidth - totalWidthOfContent - 2) / 2));



                //$("#toleranceDropdown .smselectanchorrun").css('font-size', '22px');

                $("#smmergedBottomDisplayPart").hide();
                smMergeDuplicate.controls.smmergeduplicatePrimaryWeightText().parent().width(smMergeDuplicate.getInputElementWidth());



            });




        }
        //target.parent().parent().parent().attr('row_number', noOfRows);

        var removeItem = smselect.getSelectedOption($("#" + target.parent().siblings().find(".smselect").attr("id")))[0].value;
        smMergeDuplicate._selectedAttributes = $.grep(smMergeDuplicate._selectedAttributes, function (value) {
            return value != removeItem;
        });

        var removedInputText = parseInt(target.parent().siblings().find(".smmergeduplicateAttributeInput").val());
        var primaryInputText = parseInt(smMergeDuplicate.controls.smmergeduplicatePrimaryWeightText().val());
        smMergeDuplicate._currectMatchWeight += parseInt(removedInputText);
        smMergeDuplicate.controls.smmergeduplicatePrimaryWeightText().val((primaryInputText + removedInputText) + "%");

        var numberOfRows = smMergeDuplicate.controls.smmergeduplicateAttributesContainer().find('div[row]').length;
        if (numberOfRows == 5) {
            smMergeDuplicate.controls.smmergeduplicateAttributesContainer().smslimscroll({
                destroy: true
            });
            smMergeDuplicate.controls.smmergeduplicatePrimaryWeightText().parent().width(smMergeDuplicate.getInputElementWidth());
            smMergeDuplicate.controls.smmergeduplicateAttributesContainer().find(".smslimScrollDiv").height(55 * noOfRows);
        }

        if (target.parent().parent().attr('row') !== undefined) {
            target.parent().parent().remove();
        }

        event.stopPropagation();
    },
    bindSectypeDropDown: function (firstCallback, secondCallback) {
        $.ajax({
            type: 'POST',
            url: smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + "/GetAllSectypes",
            data: JSON.stringify({
                "userName": smMergeDuplicate._username
            }),
            contentType: "application/json",
            dataType: "json",
            success: function (data) {
                var _sectypeList = data.d;
                smMergeDuplicate._sectypeList = _sectypeList;
                var multiSelectData = [{
                    options: _sectypeList, text: "All Security Types"
                }];

                var selectedItems = [];
                for (var item in _sectypeList) {
                    selectedItems.push(_sectypeList[item].text);
                }

                smMergeDuplicate.createSMSelectDropDown(smMergeDuplicate.controls.smmergeduplicateSectypeDropDown(), multiSelectData, true, "220", function (e) {
                    var dropdownRef = $("#smselect_" + smMergeDuplicate.controls.smmergeduplicateSectypeDropDown().attr("id"));
                    dropdownRef.find(".smselectanchorcontainer2").css("width", "auto");
                    dropdownRef.find(".smselectanchorcontainer2").css("height", "25px");

                    dropdownRef.find(".smselectcon").css("height", "200px");

                    //dropdownRef.find(".smselectanchorrun").css("font-size", "16px");
                    dropdownRef.find(".smselectanchorrun").css("font-size", "24px");
                    dropdownRef.find(".smselectanchorrun").css("letter-spacing", "1px");

                    dropdownRef.find(".smselectanchorrun").css("font-family", "oswald");
                    dropdownRef.find(".smselectanchorrun").css("line-height", "20px");
                    dropdownRef.find(".smselectanchorrun").css("color", "#00bff0");
                    dropdownRef.find(".smselectimage").css("color", "#00bff0");
                    dropdownRef.find(".smselectimage").css("float", "left");
                    dropdownRef.find(".smselectoptions ").css("text-align", "left");
                    //dropdownRef.parent().css("margin-left" , "3%");
                    //dropdownRef.parent().css("margin-left", "6%");

                    var tempText = dropdownRef.find(".smselectanchorrun").text().replace("selected", "");
                    dropdownRef.find(".smselectanchorrun").text(tempText);
                }, "All Security Types", smMergeDuplicate.onSectypeChangeHandler, selectedItems, true, "Security Types");
            },
            error: function () {
                console.log("Attributes cannot be fetched");
            },
            complete: function () {
                if (typeof firstCallback === "function") {
                    firstCallback(null, secondCallback);
                }
                var totalWidthOfContent = $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectanchorcontainer").width() + $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectimage ").width();
                var totalWidth = $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectrun").width();
                $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectrun").css("padding-left", ((totalWidth - totalWidthOfContent - 2) / 2));
                $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectrun").css("padding-right", ((totalWidth - totalWidthOfContent - 2) / 2));

                //$("#smselect_" + smMergeDuplicate.controls.smmergeduplicateSectypeDropDown().attr("id")).find(".selectall").click();
            }
        });
        //var _sectypeList = [{ text: 'Equity Common Stock', value: 0 }, { text: 'Corporate Bonds', value: 1 }, { text: 'Equity Options', value: 2}];
    },
    bindEntitytypeDropDown: function (firstCallback, secondCallback) {
        $.ajax({
            type: 'POST',
            url: smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + "/GetAllEntityTypes",
            data: JSON.stringify({
                "userName": smMergeDuplicate._username
            }),
            contentType: "application/json",
            dataType: "json",
            success: function (data) {
                var _sectypeList = data.d;
                smMergeDuplicate._sectypeList = _sectypeList;
                var multiSelectData = [{
                    options: _sectypeList, text: "All Entity Types"
                }];

                var selectedItems = [];
                for (var item in _sectypeList) {
                    selectedItems.push(_sectypeList[item].text);
                }


                smMergeDuplicate.createSMSelectDropDown(smMergeDuplicate.controls.smmergeduplicateSectypeDropDown(), _sectypeList, true, "220", function () {
                    var dropdownRef = $("#smselect_" + smMergeDuplicate.controls.smmergeduplicateSectypeDropDown().attr("id"));

                    //dropdownRef.find(".smselectanchorcontainer2").css("width", "initial");
                    dropdownRef.find(".smselectanchorcontainer2").css("width", "auto");
                    dropdownRef.find(".smselectanchorrun").css("font-family", "oswald");
                    dropdownRef.find(".smselectanchorcontainer2").css("height", "25px");
                    //dropdownRef.find(".smselectanchorrun").css("font-size", "16px");
                    dropdownRef.find(".smselectanchorrun").css("font-size", "24px");
                    dropdownRef.find(".smselectanchorrun").css("letter-spacing", "1px");
                    dropdownRef.find(".smselectanchorrun").css("line-height", "20px");
                    dropdownRef.find(".smselectanchorrun").css("color", "#00bff0");
                    dropdownRef.find(".smselectimage").css("color", "#00bff0");
                    dropdownRef.find(".smselectimage").css("float", "left");
                    dropdownRef.find(".smselectoptions ").css("text-align", "left");

                    var tempText = dropdownRef.find(".smselectanchorrun").text().replace("selected", "");
                    dropdownRef.find(".smselectanchorrun").text(tempText);
                }, "Select Entity Type", smMergeDuplicate.onSectypeChangeHandler, false, "");

                //To Select the first entity by default

                smselect.setOptionByValue($("#smselect_" + smMergeDuplicate.controls.smmergeduplicateSectypeDropDown().attr("id")), smMergeDuplicate._sectypeList[0].value);
                //if (smMergeDuplicate._productName == "refmaster"){
                //    smselect.reset($("#smselect_" + smMergeDuplicate.controls.smmergeduplicateSectypeDropDown().attr("id")));
                //}
            },
            error: function () {
                console.log("Attributes cannot be fetched");
            },
            complete: function () {
                if (typeof firstCallback === "function") {
                    firstCallback(null, secondCallback);
                }
                var totalWidthOfContent = $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectanchorcontainer").width() + $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectimage ").width();
                var totalWidth = $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectrun").width();
                $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectrun").css("padding-left", ((totalWidth - totalWidthOfContent - 2) / 2));
                $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectrun").css("padding-right", ((totalWidth - totalWidthOfContent - 2) / 2));


                //$("#smselect_" + smMergeDuplicate.controls.smmergeduplicateSectypeDropDown().attr("id")).find(".selectall").click();
            }
        });
        //var _sectypeList = [{ text: 'Equity Common Stock', value: 0 }, { text: 'Corporate Bonds', value: 1 }, { text: 'Equity Options', value: 2}];
    },
    onSectypeChangeHandler: function (e) {
        var selectedSectype = smselect.getSelectedOption($(e.currentTarget));
        var optionsLength = $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectmulticon").length;

        if (selectedSectype.length === (optionsLength - 1)) {
            smMergeDuplicate._selectedSectype = "-1";
            smMergeDuplicate._selectedDupesSectype = "0";

            var tempText = $(e.currentTarget).find(".smselectanchorrun").text().replace("selected", "");
            $(e.currentTarget).find(".smselectanchorrun").text(tempText);
        }
        else {
            var selSectypes = [];
            for (var item in selectedSectype) {
                selSectypes.push(selectedSectype[item].value);
            }
            smMergeDuplicate._selectedSectype = selSectypes.toString();
            smMergeDuplicate._selectedDupesSectype = selSectypes;
        }

        if (smMergeDuplicate._selectedDupesSectype.length > 0) {
            //To Get All Attributes List based on Sectype Selection
            smMergeDuplicate.getAllAttributesList(null, smMergeDuplicate.initDropDowns);
        }

        //Center the text
        if ($("#smmergedFindDuplicatesContainer").attr("class") === "smmergedFirstPageStyle") {
            var totalWidthOfContent = $(e.currentTarget).find(".smselectanchorcontainer").width() + $(e.currentTarget).find(".smselectimage ").width();
            var totalWidth = $(e.currentTarget).find(".smselectrun").width();
            if (totalWidth - totalWidthOfContent < 0) {
                $(e.currentTarget).find(".smselectrun").width(totalWidthOfContent + 20);
                $(e.currentTarget).find(".smselectrun").css("padding-left", ((totalWidth - totalWidthOfContent - 2 + 20) / 2));
                $(e.currentTarget).find(".smselectrun").css("padding-right", ((totalWidth - totalWidthOfContent - 2 + 20) / 2));
            }
            else {
                $(e.currentTarget).find(".smselectrun").css("padding-left", ((totalWidth - totalWidthOfContent - 2) / 2));
                $(e.currentTarget).find(".smselectrun").css("padding-right", ((totalWidth - totalWidthOfContent - 2) / 2));
            }

        }

        else {
            var totalWidthOfContent = $(e.currentTarget).find(".smselectanchorcontainer").width() + $(e.currentTarget).find(".smselectimage ").width();
            var totalWidth = $(e.currentTarget).find(".smselectrun").width();
            $(e.currentTarget).find(".smselectrun").css("padding-left", ((totalWidth - totalWidthOfContent - 2) / 2));
            $(e.currentTarget).find(".smselectrun").css("padding-right", ((totalWidth - totalWidthOfContent - 2) / 2));

        }
        smMergeDuplicate.controls.smmergeduplicateHdnSelectedFilter().val("0");
    },
    onAttributeChangeHandler: function (e) {

        //Code to disable approximate match on boolean attr type

        var selectedAttr = smselect.getSelectedOption($(e.currentTarget))[0].value;

        var attrDataType = smMergeDuplicate.getSelectedAttributeDataType(selectedAttr).toLowerCase();
        var parentRowId = $(e.currentTarget).closest(".bottomMarginsForRows").attr("row");


        ////calling the match type
        if ($("#smselect_" + "smmergeduplicatePrimaryMatchTypeDropDown" + "_" + parentRowId).length > 0) {
            //var currSelection =  smselect.getSelectedOption($("#smselect_" + "smmergeduplicatePrimaryMatchTypeDropDown" + "_" +parentRowId))[0].text;
            smselect.setOptionByText($("#smselect_" + "smmergeduplicatePrimaryMatchTypeDropDown" + "_" + parentRowId), "Exact Match");
        }


        //Disabling match type dropdown in case boolean attribute type is selected and only works with exact match type
        if (checkForBooleanType(attrDataType.toLowerCase())) {
            smselect.setOptionByText($("#smselect_" + "smmergeduplicatePrimaryMatchTypeDropDown" + "_" + parentRowId), "Exact Match");
            //smselect.setOptionByText($("#smselect_smmergeduplicatePrimaryMatchTypeDropDown"), "Exact Match");
            smselect.disable($("#smselect_" + "smmergeduplicatePrimaryMatchTypeDropDown" + "_" + parentRowId));
        }
        else {
            smselect.enable($("#smselect_" + "smmergeduplicatePrimaryMatchTypeDropDown" + "_" + parentRowId));
        }


        if ($.inArray(selectedAttr, smMergeDuplicate._selectedAttributes) === -1) {
            //smMergeDuplicate._selectedAttributes.push(selectedAttr);
            return true;
        }
        else {
            e.stopPropagation();
            return false;
        }
    },
    bindSavedFilter: function () {
        var filterList = [];
        var params = {};
        if (smMergeDuplicate._productName == "refmaster") {
            params.userName = smMergeDuplicate._username;
            params.moduleId = 6;
        }
        //var int = JSON.stringify({ userName: smMergeDuplicate._username })
        $.ajax({
            type: 'POST',
            url: smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + (smMergeDuplicate._productName == "refmaster" ? "/GetRMDeDuplicateFilterList" : "/GetDeDuplicateFilterList"),
            contentType: "application/json",
            dataType: "json",
            data: (smMergeDuplicate._productName == "refmaster") ? JSON.stringify({ "userName": smMergeDuplicate._username, "moduleId": 6 }) : {},
            success: function (data) {
                smMergeDuplicate._filterResponseData = data.d;
                var dupesFilterList = Object.keys(smMergeDuplicate._filterResponseData);
                for (var items in dupesFilterList) {
                    var tempObj = {
                    }
                    tempObj.text = smMergeDuplicate._filterResponseData[items].Value["configName"];
                    tempObj.value = smMergeDuplicate._filterResponseData[items].Key;
                    filterList.push(tempObj);
                }
                //Adjust Elements Position
                smMergeDuplicate.initElementsPosition();
            },
            error: function () {
                console.log("Attributes cannot be fetched");

            },
            complete: function () {
                // $("#smselect_smmergeduplicateSavedFilters").show();
                if (smMergeDuplicate._filterResponseData.length === 0) {
                    $("#smselect_smmergeduplicateSavedFilters").hide();
                }

                    //
                else {
                    $("#smselect_smmergeduplicateSavedFilters").show();
                    smMergeDuplicate.createSMSelectDropDown(smMergeDuplicate.controls.smmergeduplicateSavedFilters(), filterList, false, "130", function () {
                        $("#smselect_" + smMergeDuplicate.controls.smmergeduplicateSavedFilters().attr("id")).find(".smselectanchorrun").css("color", "#43D9C6");
                        $("#smselect_" + smMergeDuplicate.controls.smmergeduplicateSavedFilters().attr("id")).find(".smselectanchorrun").css("font-size", "14px");
                        $("#smselect_" + smMergeDuplicate.controls.smmergeduplicateSavedFilters().attr("id")).find(".smselectimage").css("color", "#43D9C6");
                        $("#smselect_" + smMergeDuplicate.controls.smmergeduplicateSavedFilters().attr("id")).find(".smselectcon").css("width", "130px");
                    }, "Saved Searches", smMergeDuplicate.onChangeFilterDropDown);

                    setTimeout(function () {

                        if (smMergeDuplicate._presetValue != '' && smMergeDuplicate._presetValue != null && smMergeDuplicate._presetValue != undefined) {
                            var presetName = smMergeDuplicate._presetValue.split('||')[0];
                            smselect.setOptionByText($("#smselect_" + smMergeDuplicate.controls.smmergeduplicateSavedFilters().attr("id")), presetName);
                        }
                    }, 1500);
                }
            }
        });



    },
    onChangeFilterDropDown: function (e) {
        smMergeDuplicate.controls.smmergeduplicateHdnSelectedFilter().val(smselect.getSelectedOption($(e.currentTarget))[0].value);
        $.each(smMergeDuplicate._filterResponseData, function (ii, ee) {
            if (ee.Key === parseInt(smselect.getSelectedOption($(e.currentTarget))[0].value)) {

                smMergeDuplicate.populateConfigValues(ee.Value);

                //ANIMATE AND PREVIOUS PART
                $("#smmergedFindDuplicatesContainer").animate({
                    "padding-top": '30px', "padding-bottom": '0px'
                }, 500, function () {
                    //$("#smmergedFindDuplicatesContainer").css({ "padding-top": '30px', "padding-bottom": '0px' });
                    $("#smmergedFindDuplicatesContainer").removeClass('smmergedFirstPageStyle');
                    $("#smmergedFindDuplicatesContainer").addClass('smmergeTopSection');
                    $("#smmergeduplicateSectypeDropDown .smselectanchorrun").css('font-size', '20px');
                    //$("#smmergeduplicateSectypeDropDown .smselectanchorrun").css('font-size', '22px');
                    $("#smmergeduplicatePrimaryAttributeDropDown .smselectanchorrun").css('font-size', '14px');
                    $("#smmergeduplicatePrimaryMatchTypeDropDown .smselectanchorrun").css('font-size', '14px');
                    // $("#toleranceDropdown .smselectanchorrun").css('font-size', '14px');

                    smMergeDuplicate.controls.smmergeduplicatePrimaryWeightText().parent().width(smMergeDuplicate.getInputElementWidth());
                    //smMergeDuplicate.addAttributeRow(rowNumber, selectedWeight, selectedAttr[0].value, selectedMatchType[0].value);
                    $("#smmergedBottomDisplayPart").show();
                    //smMergeDuplicate.controls.smmergeduplicatePrimaryWeightText().parent().width(smMergeDuplicate.getInputElementWidth());
                    var totalWidthOfContent = $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectanchorcontainer").width() + $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectimage ").width();
                    var totalWidth = $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectrun").width();
                    $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectrun").css("padding-left", ((totalWidth - totalWidthOfContent - 2) / 2) - 25);
                    $("#smselect_smmergeduplicateSectypeDropDown").find(".smselectrun").css("padding-right", ((totalWidth - totalWidthOfContent - 2) / 2) + 25);

                });
            }
        });

        //smMergeDuplicate.controls.smmergeduplicateSaveBtn().val("Update Preset");
    },
    setConfigValues: function () {
        $.ajax({
            type: 'POST',
            url: smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + "/GetPreset",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ dupes_master_id: smMergeDuplicate.controls.smmergeduplicateHdnSelectedFilter().val() }),
            success: function (data) {
                smMergeDuplicate.populateConfigValues(data.d);
            },
            error: function () {
                console.log("Data cannot be fetched");
            }
        });
    },
    populateConfigValues: function (data) {
        var stringSectypeId = [];
        $.each(data.sectypeId, function (ii, ee) {
            var a = data.sectypeId[ii].toString();
            stringSectypeId.push(a);
        });

        //Set Sectype Dropdown
        smselect.reset($("#smselect_" + smMergeDuplicate.controls.smmergeduplicateSectypeDropDown().attr("id")));
        for (var i = 0; i < stringSectypeId.length; i++) {
            smselect.setOptionByValue($("#smselect_" + smMergeDuplicate.controls.smmergeduplicateSectypeDropDown().attr("id")), stringSectypeId[i]);
        }

        //            smMergeDuplicate.getAllEntityTypeAttributesList(data);
        //        else {

        smMergeDuplicate.getAllAttributesList(data);
        //            if (smMergeDuplicate._productName != "refmaster")
        //            smMergeDuplicate.setPresetValues(data);
        //        }


    },
    setPresetValues: function setPresetValues(data) {
        //Set Final Match Confidence
        smMergeDuplicate.controls.smmergeduplicateFilterMatchWeightText().val(data.matchConfidence + "%");

        //Set Config Name
        smMergeDuplicate.controls.smmergeduplicateSaveBtn().attr("preset_name", data.configName.trim());

        //Set Primary DropDowns Styling
        smMergeDuplicate.initDropDowns();

        //Set Attributes
        smMergeDuplicate.controls.smmergeduplicateAttributesContainer().empty();
        for (var item in data.attrList) {
            var rowNumber = parseInt(item) + 1;
            smMergeDuplicate.addAttributeRow(rowNumber, data.attrList[item].matchPercentage, data.attrList[item].attributeId.toString(), data.attrList[item].matchTypeId.toString(), data.attrList[item].toleranceValue, data.attrList[item].toleranceOptionSelected);

        }

        //Set Primary Input Value
        smMergeDuplicate.controls.smmergeduplicatePrimaryWeightText().val("0%");
        //Set Current Match Weight to Zero
        smMergeDuplicate._currectMatchWeight = 0;

        //Apply Btn Margin Left Setting
        if (smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().attr("is_expanded") !== "true") {
            smMergeDuplicate.controls.smmergeduplicateFilterMatchWeightText().parent().css("margin-left", "3%");
            smMergeDuplicate.controls.smmergeduplicateFindDuplicatesBtn().parent().css("margin-left", "2%");
        }

        if (smMergeDuplicate._presetValue != '' && smMergeDuplicate._presetValue != null && smMergeDuplicate._presetValue != undefined) {
            smMergeDuplicate.fetchDuplicateSecurities();
        }
    },
    getAttributesListBasedOnModuleType: function () {
        var attributesArray = [];
        if (smMergeDuplicate._productName.toLowerCase() === "refmaster") {
            attributesArray = smMergeDuplicate._allRefAttributesList;
        }
        else {
            attributesArray = smMergeDuplicate._allAttributesList;
        }
        return attributesArray;
    },
    addAttributeRow: function (rowNumber, selectedWeight, selectedAttributeValue, selectedMatchTypeValue, toleranceValue, toleranceOptionSelected) {
        //var attrSecWidth = ($(window).width() - $("#smmergedFindDuplicatesSearchCriteriaContainer").width()) / 2;
        $("#smmergedFindDuplicatesSearchCriteriaContainer").css("margin-left", $("#smmergeduplicatePrimaryAttributeDropDown").offset().left + 'px');

        smMergeDuplicate.controls.smmergeduplicateAttributesContainer().attr('row_number', parseInt(smMergeDuplicate.controls.smmergeduplicateAttributesContainer().attr('row_number')) + 1);

        var attrListId = 'smmergeduplicatePrimaryAttributeDropDown_' + rowNumber;
        var matchTypeId = 'smmergeduplicatePrimaryMatchTypeDropDown_' + rowNumber;
        var inputWeightId = 'smmergeduplicatePrimaryWeightText_' + rowNumber;
        var deleteBtnId = 'smmergeduplicateDeleteAttributeRow_' + rowNumber;
        var toleranceInput = 'smmergeduplicateToleranceInputRow_' + rowNumber;
        var toleranceDropdown = 'smmergeduplicateToleranceDropdownRow_' + rowNumber;
        var toleranceDropdownSelectedMain = 'smmergeduplicateToleranceDropdownRow_' + rowNumber;

        var attrDataType = smMergeDuplicate.getSelectedAttributeDataType(selectedAttributeValue);

        //handling for boolean types, exact match
        //var attrDataType = smMergeDuplicate.getSelectedAttributeDataType(selectedAttr[0].value).toLowerCase();

        var ddOptions = [];
        if (attrDataType === "NUMERIC" || attrDataType === "DECIMAL" || attrDataType.toLowerCase() == "int") {
            if (attrDataType.toLowerCase() == "decimal" || attrDataType.toLowerCase() == "int")
                attrDataType = "NUMERIC";
            for (var item in smMergeDuplicate._toleranceOptions) {
                if (smMergeDuplicate._toleranceOptions[item].Key === attrDataType) {
                    ddOptions = smMergeDuplicate._toleranceOptions[item].Value;
                    break;
                }
            }
        }
        else if (attrDataType === "TIME") {
            for (var item in smMergeDuplicate._toleranceOptions) {
                if (smMergeDuplicate._toleranceOptions[item].Key === attrDataType) {
                    ddOptions = smMergeDuplicate._toleranceOptions[item].Value;
                    break;
                }
            }
        }
        else if (attrDataType === "DATE") {
            for (var item in smMergeDuplicate._toleranceOptions) {
                if (smMergeDuplicate._toleranceOptions[item].Key === attrDataType) {
                    ddOptions = smMergeDuplicate._toleranceOptions[item].Value;
                    break;
                }
            }
        }
        else if (attrDataType === "DATETIME") {
            for (var item in smMergeDuplicate._toleranceOptions) {
                if (smMergeDuplicate._toleranceOptions[item].Key === "DATE")// || smMergeDuplicate._toleranceOptions[item].Key === "TIME") 
                {
                    ddOptions = smMergeDuplicate._toleranceOptions[item].Value.concat(ddOptions);
                }
            }
        }

        var toleranceHTML = "<div class='smmergeduplicateHorizontalAlign smmergeduplicateToleranceContainerMatchCriteria' style=' text-align: center; width:202px;'></div>";

        var tolVal = "", tolOpt = "";
        if (toleranceValue !== null && toleranceValue !== undefined) {
            tolVal = toleranceValue;
        }
        else {
            tolVal = $("#toleranceInput").val();
        }
        if (toleranceOptionSelected !== null && toleranceOptionSelected !== undefined) {
            tolOpt = toleranceOptionSelected;
        }
        //  <input class='smergeduplicateToleranceInput' type='text' id='" + toleranceInput + "' value='" + $("#toleranceInput").val() + "' />

        var isApproxMatch = false;
        if (selectedMatchTypeValue === "2") {
            isApproxMatch = true;
            var currentSelectedDropDown = smselect.getSelectedOption($("#toleranceDropdown"))[0].text;
            var plusMinus = "<div class='smmergeduplicateHorizontalAlign smmergeduplicatePlusMinusContainer'><div class='smmergeduplicatePlusSign'>+</div><div class='smmergeduplicateMinusSign'>-</div></div>";
            //toleranceHTML = "<div class='smmergeduplicateHorizontalAlign smmergeduplicateToleranceContainerMatchCriteria' style=' padding-left:24px; width:202px;color:rgb(199, 196, 196)'>" +plusMinus + "<div class='smmergeduplicateHorizontalAlign'><div class='smergeduplicateToleranceInput' type='text' id='" + toleranceInput + "'>" +$("#toleranceInput").val() + "</div></div><div id='" + toleranceDropdownSelectedMain + "' class='smmergeduplicateHorizontalAlign selectedOptionToleranceTextDisplay'>" +currentSelectedDropDown + "</div></div>";
            toleranceHTML = "<div class='smmergeduplicateHorizontalAlign smmergeduplicateToleranceContainer' style='width:202px;'>" + plusMinus + "<div class='smmergeduplicateHorizontalAlign'><input class='smergeduplicateToleranceInput' type='text' id='" + toleranceInput + "' value='" + tolVal + "' /></div><div id='" + toleranceDropdown + "' class='smmergeduplicateHorizontalAlign'></div></div>";
            //toleranceHTML = "<div class='smmergeduplicateHorizontalAlign smmergeduplicateToleranceContainer' style='width:202px;'>" + plusMinus + "<div class='smmergeduplicateHorizontalAlign'><input class='smergeduplicateToleranceInput' type='text' id='" + toleranceInput + "' value='" + tolVal + "' /></div><div id='" + toleranceDropdown + "' class='smmergeduplicateHorizontalAlign'></div></div>";
        }
        else {
            var currentSelectedDropDown = smselect.getSelectedOption($("#toleranceDropdown"))[0].text;
            var plusMinus = "<div class='smmergeduplicateHorizontalAlign smmergeduplicatePlusMinusContainer'><div class='smmergeduplicatePlusSign'>+</div><div class='smmergeduplicateMinusSign'>-</div></div>";
            //toleranceHTML = "<div class='smmergeduplicateHorizontalAlign smmergeduplicateToleranceContainerMatchCriteria' style=' padding-left:24px; width:202px;color:rgb(199, 196, 196)'>" + plusMinus + "<div class='smmergeduplicateHorizontalAlign'><div class='smergeduplicateToleranceInput' type='text' id='" + toleranceInput + "'>" + $("#toleranceInput").val() + "</div></div><div id='" + toleranceDropdownSelectedMain + "' class='smmergeduplicateHorizontalAlign selectedOptionToleranceTextDisplay'>" + currentSelectedDropDown + "</div></div>";
            toleranceHTML = "<div class='smmergeduplicateHorizontalAlign smmergeduplicateToleranceContainer' style='width:202px;color:rgb(199, 196, 196)'>" + plusMinus + "<div class='smmergeduplicateHorizontalAlign'><input disabled class='smergeduplicateToleranceInput' type='text' id='" + toleranceInput + "' value='" + tolVal + "' /></div><div id='" + toleranceDropdown + "' class='smmergeduplicateHorizontalAlign'></div></div>";
        }

        var rowHTML = '<div row="' + rowNumber + '" class="bottomMarginsForRows" ><div class="smmergeduplicateHorizontalAlign smmergeduplicateRowStyle"><div id = ' + attrListId + ' class="smmergeduplicateHorizontalAlign smmergeduplicateAttributeDropDownMatchCriteria"></div><div id = ' + matchTypeId + ' class="smmergeduplicateHorizontalAlign smmergeduplicateAttributeDropDownMatchCriteria smmergeduplicateMatchTypeDropDown"></div><div class="smmergeduplicateHorizontalAlign" style="width:' + smMergeDuplicate.getInputElementWidth(isApproxMatch) + 'px;" ><input type="text" id = ' + inputWeightId + ' value="' + selectedWeight + "%" + '" class="smmergeduplicateAttributeInput smmergeduplicateFocusOut" /></div>' + toleranceHTML + '</div><div class="smmergeduplicateHorizontalAlign"><div id = ' + deleteBtnId + ' class="CommonIcon smmergeduplicateAddAttributeBtnStyle smmergeduplicateTrashIconPosition fa fa-trash-o" style="margin-top: -14px;"></div></div></div>';
        smMergeDuplicate.controls.smmergeduplicateAttributesContainer().append(rowHTML);


        //if (smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().attr("is_expanded") === "true") {
        //Dropdowns for Attributes and Matchtype under Match Criteria
        smMergeDuplicate.createSMSelectDropDown($("#" + attrListId), smMergeDuplicate.getAttributesListBasedOnModuleType(), true, smMergeDuplicate._primaryAttrDropdownWidth.toString(), function () {
            $("#smselect_" + attrListId).find(".smselectanchorrun").css("font-size", "14px");
            $("#smselect_" + attrListId).find(".smselectanchorcontainer ").css("text-align", "center");
            $("#smselect_" + attrListId).find(".smselectimage").hide();
            $("#smselect_" + attrListId).find(".smselectcon").css("margin-left", "20%");
            //smselect.disable($("#smselect_" + attrListId));
        }, "Attributes", smMergeDuplicate.onAttributeChangeHandler);
        smselect.setOptionByValue($("#" + attrListId), selectedAttributeValue);
        smMergeDuplicate.createSMSelectDropDown($("#" + matchTypeId), smMergeDuplicate._matchType, false, smMergeDuplicate._primaryMatchTypeDropDownWidth.toString(), function () {
            $("#smselect_" + matchTypeId).find(".smselectanchorrun").css("font-size", "14px");
            $("#smselect_" + matchTypeId).find(".smselectanchorcontainer ").css("text-align", "center");
            $("#smselect_" + matchTypeId).find(".smselectimage").hide();
            $("#smselect_" + matchTypeId).find(".smselectcon").css("margin-left", "20%");
            //smselect.disable($("#smselect_" + matchTypeId));
        }, "Match Type", smMergeDuplicate.onChangeMatchTypeInRow);
        smselect.setOptionByValue($("#" + matchTypeId), selectedMatchTypeValue);

        //if (isApproxMatch) {
        //Apply Tolerance Dropdown
        smMergeDuplicate.createSMSelectDropDown($("#" + toleranceDropdown), ddOptions, false, smMergeDuplicate._toleranceOptionsDropdownWidth.toString(), function () {
            $("#smselect_" + attrListId).find(".smselectanchorrun").css("font-size", "14px");
            $("#smselect_" + attrListId).find(".smselectimage").hide();
            //smselect.disable($("#smselect_" + attrListId));
        }, "Options");
        //if (toleranceOptionSelected !== null && toleranceOptionSelected !== undefined) {
        //smselect.setOptionByText($("#" +toleranceDropdown), smselect.getSelectedOption($("#toleranceDropdown"))[0].text);

        //}
        //}     
        if (isApproxMatch) {
            if (tolOpt) {
                smselect.setOptionByValue($("#" + toleranceDropdown), tolOpt);
            }
            else {
                smselect.setOptionByText($("#" + toleranceDropdown), smselect.getSelectedOption($("#toleranceDropdown"))[0].text);
            }
        }
        if (!isApproxMatch)
            smselect.disable($("#" + toleranceDropdown));


        //}
        //else {smmergeduplicateFilterSelectionDiv
        //    smMergeDuplicate.createSMSelectDropDown($("#" + attrListId), smMergeDuplicate._allAttributesList, true, smMergeDuplicate._contractedAttrDropDown.toString(), function () {
        //        $("#smselect_" + attrListId).find(".smselectanchorrun").css("font-size", "14px");
        //        $("#smselect_" + attrListId).find(".smselectcon").css("margin-left", "20%");
        //        $("#smselect_" + attrListId).find(".smselectimage").hide();
        //        //smselect.disable($("#smselect_" + attrListId));
        //    }, "Attributes", smMergeDuplicate.onAttributeChangeHandler);
        //    smselect.setOptionByValue($("#" + attrListId), selectedAttributeValue);
        //    smMergeDuplicate.createSMSelectDropDown($("#" + matchTypeId), smMergeDuplicate._matchType, false, smMergeDuplicate._contractedMatchTypeDropDown.toString(), function () {
        //        $("#smselect_" + matchTypeId).find(".smselectanchorrun").css("font-size", "14px");
        //        $("#smselect_" + matchTypeId).find(".smselectcon").css("margin-left", "20%");
        //        $("#smselect_" + matchTypeId).find(".smselectimage").hide();
        //        //smselect.disable($("#smselect_" + matchTypeId));
        //    }, "Match Type");
        //    smselect.setOptionByValue($("#" + matchTypeId), selectedMatchTypeValue);

        //    smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().find(".smmergeduplicateAttributeInput").css("width", "70px");
        //    smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().find(".smmergeduplicateAttributeInput").css("margin-left", "0px");
        //}


        //attrDataType

        //Disabling match type dropdown in case boolean attribute type is selected and only works with exact match type
        if (checkForBooleanType(attrDataType.toLowerCase())) {
            smselect.setOptionByText($("#" + matchTypeId), "Exact Match");
            //smselect.setOptionByText($("#smselect_smmergeduplicatePrimaryMatchTypeDropDown"), "Exact Match");
            smselect.disable($("#" + matchTypeId));
        }
        else {
            smselect.enable($("#" + matchTypeId));
        }


        //To attach focus out event with all inputs that are used for storing weight values
        smMergeDuplicate.onFocusOutInput();

        $("#" + deleteBtnId).unbind('click').bind('click', smMergeDuplicate.onClickDeleteAttributeRow);

        //apply binding of match percent being 100
        $("#" + inputWeightId).on("change", function (event) {
            //var val = parseInt(event.target.value.split("%")[0]);
            var listOfAttributePercentValues = $("input[id^='smmergeduplicatePrimaryWeightText_']");
            var sum = 0;
            $.each(listOfAttributePercentValues, function (key, value) {
                sum += parseInt(value.value.split("%")[0])
            });
            smMergeDuplicate._currectMatchWeight = 100 - sum;
            if (smMergeDuplicate._currectMatchWeight < 0) {
                smMergeDuplicate._currectMatchWeight = 0;
            }
            smMergeDuplicate.controls.smmergeduplicatePrimaryWeightText().val(smMergeDuplicate._currectMatchWeight + "%");
        });

        var noOfRows = smMergeDuplicate.controls.smmergeduplicateAttributesContainer().find('div[row]').length;
        if (noOfRows >= 5) {
            smMergeDuplicate.controls.smmergeduplicateAttributesContainer().smslimscroll({
                height: (55 * 5) + 'px', width: $(".smmergedTopFilterBackground").width() + 50 + 'px', railVisible: true, alwaysVisible: false
            });
        }

        $("#toleranceInput").val("0");

        applyCorrectInputBinding();
        $(window).trigger('resize');
    },
    createProductListDiv: function () {
        var ModuleData = [];
        for (var i = 0; i < smMergeDuplicate._productList.modules.length; i++) {
            var moduleName = smMergeDuplicate._productList.modules[i].text
            if (moduleName.toLowerCase() === "secmaster" && smMergeDuplicate._isDeDupeAllowedInSecM) {
                var tempObj = {};
                tempObj.name = smMergeDuplicate._productList.modules[i].text;
                tempObj.value = smMergeDuplicate._productList.modules[i].value;
                tempObj.isSelected = true;
                tempObj.callback = function () {
                    smMergeDuplicate.init("secmaster");
                };
                ModuleData.push(tempObj);
            }
            else if (moduleName.toLowerCase() === "refmaster" && smMergeDuplicate._isDeDupeAllowedInRefM) {
                var tempObj = {};
                tempObj.name = smMergeDuplicate._productList.modules[i].text;
                tempObj.value = smMergeDuplicate._productList.modules[i].value;
                tempObj.isSelected = false;
                tempObj.callback = function () {
                    smMergeDuplicate.init("refmaster");
                };
                ModuleData.push(tempObj);
            }
        }

        if (ModuleData.length === 1) {
            ModuleData[0].isSelected = true;
        }

        var obj = {
            container: smMergeDuplicate.controls.smmergeduplicateChooseProduct(),
            theme: "middleTheme",
            data: ModuleData
        }
        CommonModuleTabs.init(obj);

        //        var divHTML = "";
        //        var selectedClass = "";
        //        var selectedModule = "";
        //        for (var i = 0; i < smMergeDuplicate._productList.modules.length; i++) {
        //            if (smMergeDuplicate._productList.selectedModule === smMergeDuplicate._productList.modules[i].text) {
        //                selectedClass = "smmergeduplicateProductTabSelected";
        //                selectedModule = smMergeDuplicate._productList.selectedModule;
        //            }
        //            else {
        //                selectedClass = "";
        //            }
        //            divHTML += "<div name='" + smMergeDuplicate._productList.modules[i].text.toLowerCase() + "' class='smmergeduplicateHorizontalAlign smmergeduplicateProductTab " + selectedClass + " '>" + smMergeDuplicate._productList.modules[i].text + "</div>";
        //        }
        //        smMergeDuplicate.controls.smmergeduplicateChooseProduct().append(divHTML);
        //        //Add Attr to the Product Container Div
        //        smMergeDuplicate.controls.smmergeduplicateChooseProduct().attr("selected_module", selectedModule);
    },
    onChangeMatchTypeInRow: function (e) {
        var selectedOption = smselect.getSelectedOption($(e.target))[0];
        var row = e.target.parentElement.parentElement.parentElement.getAttribute("row");

        var attrArrayReturn = smselect.getSelectedOption($("#smselect_smmergeduplicatePrimaryAttributeDropDown_" + row));
        if (attrArrayReturn.length > 0) {
            var selectedAttrValue = attrArrayReturn[0].value;
            var options = [];
            var attrDataType = smMergeDuplicate.getSelectedAttributeDataType(selectedAttrValue);


            //handling for boolean types, exact match
            //var attrDataType = smMergeDuplicate.getSelectedAttributeDataType(selectedAttr[0].value).toLowerCase();

            var ddOptions = [];
            if (attrDataType === "NUMERIC" || attrDataType === "DECIMAL" || attrDataType.toLowerCase() == "int") {
                if (attrDataType.toLowerCase() == "decimal" || attrDataType.toLowerCase() == "int")
                    attrDataType = "NUMERIC";
                for (var item in smMergeDuplicate._toleranceOptions) {
                    if (smMergeDuplicate._toleranceOptions[item].Key === attrDataType) {
                        ddOptions = smMergeDuplicate._toleranceOptions[item].Value;
                        break;
                    }
                }
            }
            else if (attrDataType === "TIME") {
                for (var item in smMergeDuplicate._toleranceOptions) {
                    if (smMergeDuplicate._toleranceOptions[item].Key === attrDataType) {
                        ddOptions = smMergeDuplicate._toleranceOptions[item].Value;
                        break;
                    }
                }
            }
            else if (attrDataType === "DATE") {
                for (var item in smMergeDuplicate._toleranceOptions) {
                    if (smMergeDuplicate._toleranceOptions[item].Key === attrDataType) {
                        ddOptions = smMergeDuplicate._toleranceOptions[item].Value;
                        break;
                    }
                }
            }
            else if (attrDataType === "DATETIME") {
                for (var item in smMergeDuplicate._toleranceOptions) {
                    if (smMergeDuplicate._toleranceOptions[item].Key === "DATE") {
                        ddOptions = smMergeDuplicate._toleranceOptions[item].Value.concat(ddOptions);
                    }
                }
            }





            var inputId = "smmergeduplicateToleranceInputRow_";
            //keep an enable/disable flag by adding a property to the UI
            var toleranceDropdown = "smmergeduplicateToleranceDropdownRow_";
            if (selectedOption.text.toLowerCase() === "approximate match" && ddOptions.length > 0) {
                smMergeDuplicate.createSMSelectDropDown($("#" + toleranceDropdown + row), ddOptions, false, smMergeDuplicate._toleranceOptionsDropdownWidth.toString(), function () {
                    //$("#smselect_" + attrListId).find(".smselectanchorrun").css("font-size", "14px");
                    //$("#smselect_" + attrListId).find(".smselectimage").hide();
                    //smselect.disable($("#smselect_" + attrListId));
                }, "Options");
                smselect.setOptionByValue($("#" + toleranceDropdown + row), ddOptions[0].value);
                $("#" + inputId + row).prop('disabled', false);
            }
            else {
                //disable if enabled
                smselect.disable($("#" + toleranceDropdown + row));
                $("#" + inputId + row).prop('disabled', true);
            }



        }




    },
    getAllAttributesList: function (resultdata, callback) {
        $.ajax({
            type: 'POST',
            url: (smMergeDuplicate._productName == "refmaster") ? smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + "/GetEntityTypeAttributesForDeDupe" : smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + "/GetAttributeBasedOnSecTypeSelectionForDeDupe",
            data: (smMergeDuplicate._productName == "refmaster") ? JSON.stringify({ "entityTypeId": smMergeDuplicate._selectedDupesSectype.toString() }) : JSON.stringify({
                "secTypeIds": smMergeDuplicate._selectedDupesSectype.toString(), "userName": smMergeDuplicate._username
            }),
            contentType: "application/json",
            dataType: "json",
            success: function (data) {
                if (smMergeDuplicate._productName == "refmaster") {
                    smMergeDuplicate._allRefAttributesList = data.d;
                }
                else {
                    smMergeDuplicate._allAttributesList = data.d;
                }
                //To massage attribute list according to smselect
                smMergeDuplicate.massageAttributeList();
                if (resultdata != null && resultdata != undefined)
                    smMergeDuplicate.setPresetValues(resultdata);
            },
            error: function () {
                console.log("Attributes cannot be fetched");
            },
            complete: function () {
                if (typeof callback === "function") {
                    callback();
                }
            }
        });
    },
    //    getAllEntityTypeAttributesList: function (resultdata, callback) {
    //        $.ajax({
    //            type: 'POST',
    //            url: (smMergeDuplicate._productName == "refmaster") ? smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + "/GetEntityTypeAttributesForDeDupe" : smMergeDuplicate._path + smMergeDuplicate._serviceASMXLocation + "/GetAttributeBasedOnSecTypeSelectionForDeDupe",
    //            data: (smMergeDuplicate._productName == "refmaster") ? JSON.stringify({ "entityTypeId": smMergeDuplicate._selectedDupesSectype.toString() }) : JSON.stringify({ "secTypeIds": smMergeDuplicate._selectedDupesSectype.toString(), "userName": smMergeDuplicate._username }),
    //            contentType: "application/json",
    //            dataType: "json",
    //            success: function (data) {
    //                smMergeDuplicate._allAttributesList = data.d;
    //                //To massage attribute list according to smselect
    //                smMergeDuplicate.massageAttributeList();
    //                smMergeDuplicate.setPresetValues(resultdata);
    //            },
    //            error: function () {
    //                console.log("Attributes cannot be fetched");
    //            },
    //            complete: function () {
    //                if (typeof callback === "function") {

    //                    callback();
    //                }
    //            }
    //        });
    //    },
    fetchDuplicateSecurities: function () {
        onServiceUpdating();
        smMergeDuplicate._selectedSecuritiesFromGrid = [];
        $.ajax({
            method: "POST",
            url: smMergeDuplicate._path + smMergeDuplicate._serviceDeDuplicationLocation + (smMergeDuplicate._productName == "refmaster" ? "/FindEntityDupesData" : "/FindDupesData"),
            data: JSON.stringify({
                "configId": smMergeDuplicate.controls.smmergeduplicateHdnSelectedFilter().val(), "configDetails": smMergeDuplicate.getConfigValues(), "userName": smMergeDuplicate._username
            }),
            contentType: "application/json",
            dataType: "json",
            success: function (data) {
                smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().animate({
                    height: (30) + 'px'
                }, 1000, function () {
                    //smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().height(100);
                    //smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().hide();
                    smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().attr("is_expanded", "false");
                    smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().css("visibility", "visible");
                    $('#srm_moduleTabs').hide();
                    $('.smmergeduplicateFilterOuterDiv').hide();
                    smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().width(smMergeDuplicate._windowWidth - 100);
                    smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().css("padding-top", "2.5px");
                    smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().addClass("smmergeduplicateRightFilterBackground");
                    $('#smmergeduplicateRightFilterIcon').show();
                    //smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().animate({ marginTop: "-" + (200) + 'px' }, 100);
                });
                if (data.d.length > 0) {
                    smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().attr("is_expanded", "false");

                    //smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().animate({ width: ((smMergeDuplicate._windowWidth * 45) / 200) + 'px' });
                    //    smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().animate({
                    //        height: (30) + 'px'
                    //        }, 1000, function () {
                    //            //smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().height(100);
                    //            //smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().hide();
                    //        smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().attr("is_expanded", "false");
                    //        smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().css("visibility", "visible");
                    //        $('#srm_moduleTabs').hide();
                    //        $('.smmergeduplicateFilterOuterDiv').hide();
                    //        smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().width(smMergeDuplicate._windowWidth -100);
                    //        smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().css("padding-top", "2.5px");
                    //        smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().addClass("smmergeduplicateRightFilterBackground");
                    //        $('#smmergeduplicateRightFilterIcon').show();
                    //            //smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().animate({ marginTop: "-" + (200) + 'px' }, 100);
                    //});

                    //LEFT SIDE ANIMATION    

                    //smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().animate({ marginLeft: "-" + (smMergeDuplicate._windowWidth - 30) + 'px' }, 1000, function () {
                    //    smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().width(450);
                    //    //smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().hide();
                    //    smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().attr("is_expanded", "false");
                    //    smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().css("visibility", "visible");
                    //    smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().height(smMergeDuplicate._windowHeight);
                    //    smMergeDuplicate.controls.smmergeduplicateRightFilterIcon().addClass("smmergeduplicateRightFilterBackground");
                    //    smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().animate({ marginLeft: "-" + (420) + 'px' }, 100);
                    //});

                    //Hiding/Showing the first pages

                    //  $('#smmergeduplicateFilterSelectionDiv').height(30);



                    //smMergeDuplicate.resizeFilterDiv();
                    smMergeDuplicate.controls.smmergeduplicatesGrid().css("width", "100%");
                    smMergeDuplicate.controls.smmergeduplicatesGrid().show();
                    var securityData = data.d;
                    smMergeDuplicate.controls.smmergeduplicateGridsContainer().empty();

                    for (var sec in securityData) {
                        var obj = {
                        };
                        obj.data = securityData[sec];
                        obj.container = smMergeDuplicate.controls.smmergeduplicateGridsContainer();
                        smMergeDuplicate.sm_grid.init(obj);
                    }

                    smMergeDuplicate.controls.smmergeduplicateGridsContainer().smslimscroll({
                        height: (smMergeDuplicate._windowHeight - 50) + 'px', railVisible: true, alwaysVisible: true
                    });
                    $("#smmergeduplicateGridsContainer").parent().css("margin-top", "15px");
                    /*var obj = {};
                    obj.data = dupes_data;
                    obj.container = smMergeDuplicate.controls.smmergeduplicateGridsContainer();
                    smMergeDuplicate.sm_grid.init(obj);

                    var obj = {};
                    obj.data = dupes_data2;
                    obj.container = smMergeDuplicate.controls.smmergeduplicateGridsContainer();
                    smMergeDuplicate.sm_grid.init(obj);*/
                    var rows = $("#smmergeduplicateAttributesContainer").attr("row_number");
                    var count = 0;
                    var str = "     ";
                    for (var i = 1; ; i++) {
                        if ($("#smmergeduplicatePrimaryWeightText_" + i).length) {
                            var percentIn = $("#smmergeduplicatePrimaryWeightText_" + i).val();
                            if ($("#smselect_smmergeduplicatePrimaryAttributeDropDown_" + i).length && $("#smselect_smmergeduplicatePrimaryMatchTypeDropDown_" + i).length) {
                                count++;
                                var selectedSecurity = smselect.getSelectedOption($("#smselect_smmergeduplicatePrimaryAttributeDropDown_" + i))[0].text;
                                var selectedApprox = smselect.getSelectedOption($("#smselect_smmergeduplicatePrimaryMatchTypeDropDown_" + i))[0].text;

                                str += selectedSecurity + " (" + selectedApprox + ") ";
                                if (i != rows)
                                    str += ", ";
                                if (count == rows)
                                    break;
                            }
                        }

                    }
                    $("#smmergeduplicateRightFilterIcon").text("  Match Criteria: " + str);
                }
                else {
                    //smMergeDuplicate.controls.smmergeduplicateErrorMsgText().addClass("smmergeduplicateErrorColor");
                    //smMergeDuplicate.controls.smmergeduplicateErrorMsgText().text("Oops... We dig too deep... But nothing is found here");
                    //smMergeDuplicate.hideTextAfterCustomWait(smMergeDuplicate.controls.smmergeduplicateErrorMsgText(), 10000);

                    smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().attr("is_expanded", "false");
                    smMergeDuplicate.controls.smmergeduplicatesGrid().css("width", "100%");
                    smMergeDuplicate.controls.smmergeduplicatesGrid().show();
                    var html = "<div id='" + smMergeDuplicate.controls.smmergeduplicatesGrid().id + "' class='sm_grid_container' '></div>";
                    smMergeDuplicate.controls.smmergeduplicatesGrid().append(html);
                    window.parent.leftMenu.showNoRecordsMsg("No duplicates found matching your search criteria.", smMergeDuplicate.controls.smmergeduplicateGridsContainer());
                    smMergeDuplicate.controls.smmergeduplicateGridsContainer().width("100%")
                    $("#smmergeduplicateRightFilterIcon").text("Go back");

                    //smMergeDuplicate.controls.smmergeduplicatesGrid().empty();
                    //window.parent.leftMenu.showNoRecordsMsg("No exceptions found matching your search criteria.", smMergeDuplicate.controls.smmergeduplicateGridsContainer());
                    //$("#smmergeduplicateRightFilterIcon").text("Go back");
                    //smMergeDuplicate.controls.smmergeduplicatesGrid().empty();
                    //smMergeDuplicate.controls.smmergeduplicatesGrid().css("width", "100%");
                    //smMergeDuplicate.controls.smmergeduplicateGridsContainer().show();
                }
            },
            error: function (ex) {
                console.log("Error" + ex);
            },
            complete: function () {
                onServiceUpdated();

                if (smMergeDuplicate.controls.smmergeduplicateGridsContainer().length !== 0) {
                    smMergeDuplicate.controls.smmergeduplicateGridsContainer().unbind('click').bind('click', smMergeDuplicate.onClickDuplicatesGridContainer);
                }
            }
        });
    },

    resizeFilterDiv: function () {
        //For the Filter Selection Container Div
        smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().find(".smmergeduplicateFilterInnerDiv").css("width", "100%");
        smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().find(".smmergeduplicateShiftRight").css("text-align", "center");

        //For Dropdowns in the Filter Selection Div
        smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().find(".smmergeduplicateAttributeDropDown").find(".smselectanchorcontainer").css("margin-left", "0%");
        smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().find(".smmergeduplicateAttributeDropDown").find(".smselect").css("width", smMergeDuplicate._contractedAttrDropDown.toString() + "px");
        smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().find(".smmergeduplicateAttributeDropDown").find(".smselectcon").css("width", smMergeDuplicate._contractedAttrDropDown.toString() + "px");
        smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().find(".smmergeduplicateMatchTypeDropDown").find(".smselect").css("width", smMergeDuplicate._contractedMatchTypeDropDown.toString() + "px");
        smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().find(".smmergeduplicateMatchTypeDropDown").find(".smselectcon").css("width", smMergeDuplicate._contractedMatchTypeDropDown.toString() + "px");
        smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().find(".smmergeduplicateAttributeInput").parent().css("width", "70px");

        //For Primary Attribute Selection DropDown and Match Type DropDown
        //$("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")).css("text-align", "center");

        $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")).css("width", smMergeDuplicate._contractedAttrDropDown.toString() + "px");
        $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryAttributeDropDown().attr("id")).find(".smselectcon").css("width", smMergeDuplicate._contractedAttrDropDown.toString() + "px");
        $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown().attr("id")).css("width", smMergeDuplicate._contractedMatchTypeDropDown.toString() + "px");
        $("#smselect_" + smMergeDuplicate.controls.smmergeduplicatePrimaryMatchTypeDropDown().attr("id")).find(".smselectcon").css("width", smMergeDuplicate._contractedMatchTypeDropDown.toString() + "px");

        //For Sectype DropDown
        $("#smselect_" + smMergeDuplicate.controls.smmergeduplicateSectypeDropDown().attr("id")).css("width", "initial");



        //For Attribute Filter List Div
        smMergeDuplicate.controls.smmergeduplicateAttributeSelectionDiv().css("width", "100%");

        //For Saved Filter DropDown
        smMergeDuplicate.controls.smmergeduplicateSavedFilters().css("margin-left", "45%");
        $("#smselect_" + smMergeDuplicate.controls.smmergeduplicateSavedFilters().attr("id")).css("width", "105px");

        //For Filter Match Weight Text Container Div
        smMergeDuplicate.controls.smmergeduplicateFilterMatchWeightText().parent().css("width", "100%");
        smMergeDuplicate.controls.smmergeduplicateFilterMatchWeightText().parent().css("margin-left", "6%");
        smMergeDuplicate.controls.smmergeduplicateFilterMatchWeightText().parent().css("padding-top", "3px");

        //For Attributes Container Div
        smMergeDuplicate.controls.smmergeduplicateAttributeSelectionDiv().parent().css("margin-left", "5%");

        //For Find Dupes Btn
        smMergeDuplicate.controls.smmergeduplicateFindDuplicatesBtn().parent().css("margin-left", "6%");

        //For inputs in attributes container
        smMergeDuplicate.controls.smmergeduplicatePrimaryWeightText().css("margin-left", "0%");
        //smMergeDuplicate.controls.smmergeduplicatePrimaryWeightText().parent().width(smMergeDuplicate.getInputElementWidth());
        smMergeDuplicate.controls.smmergeduplicateAttributesContainer().find(".smmergeduplicateAttributeInput").css("margin-left", "0%");


        //Add Btn Left Position
        smMergeDuplicate.controls.smmergeduplicateAddAttributeBtn().closest("span").css("margin-left", "60%");

        smMergeDuplicate.controls.smmergeduplicateFilterMatchWeightText().css("margin", "0px");

        smMergeDuplicate.controls.smmergeduplicateFilterSelectionDiv().find(".smmergeduplicatePrimaryBtnStyle").css("width", 125 + "px");

        //Heading Alignment
        smMergeDuplicate.controls.smmergeduplicateHeading().css("position", "absolute");
        smMergeDuplicate.controls.smmergeduplicateHeading().css("top", "0px");

    },

    controls: {
        _smmergeduplicateAttributeSelectionDiv: null,
        smmergeduplicateAttributeSelectionDiv: function () {
            if (this._smmergeduplicateAttributeSelectionDiv == null || this._smmergeduplicateAttributeSelectionDiv.length == 0) {
                this._smmergeduplicateAttributeSelectionDiv = $("#smmergeduplicateAttributeSelectionDiv")
            }
            return this._smmergeduplicateAttributeSelectionDiv;
        },

        _hdnUsername: null,
        hdnUsername: function () {
            if (this._hdnUsername == null || this._hdnUsername.length == 0) {
                this._hdnUsername = $("#hdnUsername");
            }
            return this._hdnUsername;
        },

        _smmergeduplicateAttributesContainer: null,
        smmergeduplicateAttributesContainer: function () {
            if (this._smmergeduplicateAttributesContainer == null || this._smmergeduplicateAttributesContainer.length == 0) {
                this._smmergeduplicateAttributesContainer = $("#smmergeduplicateAttributesContainer");
            }
            return this._smmergeduplicateAttributesContainer;
        },

        _smmergeduplicatePrimaryAttributeDropDown: null,
        smmergeduplicatePrimaryAttributeDropDown: function () {
            if (this._smmergeduplicatePrimaryAttributeDropDown == null || this._smmergeduplicatePrimaryAttributeDropDown.length == 0) {
                this._smmergeduplicatePrimaryAttributeDropDown = $("#smmergeduplicatePrimaryAttributeDropDown");
            }
            return this._smmergeduplicatePrimaryAttributeDropDown;
        },

        _smmergeduplicatePrimaryMatchTypeDropDown: null,
        smmergeduplicatePrimaryMatchTypeDropDown: function () {
            if (this._smmergeduplicatePrimaryMatchTypeDropDown == null || this._smmergeduplicatePrimaryMatchTypeDropDown.length == 0) {
                this._smmergeduplicatePrimaryMatchTypeDropDown = $("#smmergeduplicatePrimaryMatchTypeDropDown");
            }
            return this._smmergeduplicatePrimaryMatchTypeDropDown;
        },

        _smmergeduplicatePrimaryWeightText: null,
        smmergeduplicatePrimaryWeightText: function () {
            if (this._smmergeduplicatePrimaryWeightText == null || this._smmergeduplicatePrimaryWeightText.length == 0) {
                this._smmergeduplicatePrimaryWeightText = $("#smmergeduplicatePrimaryWeightText");
            }
            return this._smmergeduplicatePrimaryWeightText;
        },

        _smmergeduplicateAddAttributeBtn: null,
        smmergeduplicateAddAttributeBtn: function () {
            if (this._smmergeduplicateAddAttributeBtn == null || this._smmergeduplicateAddAttributeBtn.length == 0) {
                this._smmergeduplicateAddAttributeBtn = $("#smmergeduplicateAddAttributeBtn");
            }
            return this._smmergeduplicateAddAttributeBtn;
        },

        _smmergeduplicateChooseProduct: null,
        smmergeduplicateChooseProduct: function () {
            if (this._smmergeduplicateChooseProduct == null || this._smmergeduplicateChooseProduct.length == 0) {
                this._smmergeduplicateChooseProduct = $("#smmergeduplicateChooseProduct");
            }
            return this._smmergeduplicateChooseProduct;
        },

        _smmergeduplicateSectypeDropDown: null,
        smmergeduplicateSectypeDropDown: function () {
            if (this._smmergeduplicateSectypeDropDown == null || this._smmergeduplicateSectypeDropDown.length == 0) {
                this._smmergeduplicateSectypeDropDown = $("#smmergeduplicateSectypeDropDown");
            }
            return this._smmergeduplicateSectypeDropDown;
        },

        _smmergeduplicateFindDuplicatesBtn: null,
        smmergeduplicateFindDuplicatesBtn: function () {
            if (this._smmergeduplicateFindDuplicatesBtn == null || this._smmergeduplicateFindDuplicatesBtn.length == 0) {
                this._smmergeduplicateFindDuplicatesBtn = $("#smmergeduplicateFindDuplicatesBtn");
            }
            return this._smmergeduplicateFindDuplicatesBtn;
        },

        _smmergeduplicateSavedFilters: null,
        smmergeduplicateSavedFilters: function () {
            if (this._smmergeduplicateSavedFilters == null || this._smmergeduplicateSavedFilters.length == 0) {
                this._smmergeduplicateSavedFilters = $("#smmergeduplicateSavedFilters");
            }
            return this._smmergeduplicateSavedFilters;
        },

        _smmergeduplicateFilterSelectionDiv: null,
        smmergeduplicateFilterSelectionDiv: function () {
            if (this._smmergeduplicateFilterSelectionDiv == null || this._smmergeduplicateFilterSelectionDiv.length == 0) {
                this._smmergeduplicateFilterSelectionDiv = $("#smmergeduplicateFilterSelectionDiv");
            }
            return this._smmergeduplicateFilterSelectionDiv;
        },

        _smmergeduplicateGridsContainer: null,
        smmergeduplicateGridsContainer: function () {
            if (this._smmergeduplicateGridsContainer == null || this._smmergeduplicateGridsContainer.length == 0) {
                this._smmergeduplicateGridsContainer = $("#smmergeduplicateGridsContainer");
            }
            return this._smmergeduplicateGridsContainer;
        },

        _smmergeduplicatesGrid: null,
        smmergeduplicatesGrid: function () {
            if (this._smmergeduplicatesGrid == null || this._smmergeduplicatesGrid.length == 0) {
                this._smmergeduplicatesGrid = $("#smmergeduplicatesGrid");
            }
            return this._smmergeduplicatesGrid;
        },

        _smmergeduplicateSaveBtn: null,
        smmergeduplicateSaveBtn: function () {
            if (this._smmergeduplicateSaveBtn == null || this._smmergeduplicateSaveBtn.length == 0) {
                this._smmergeduplicateSaveBtn = $("#smmergeduplicateSaveBtn");
            }
            return this._smmergeduplicateSaveBtn;
        },

        _smmergeduplicateFilterMatchWeightText: null,
        smmergeduplicateFilterMatchWeightText: function () {
            if (this._smmergeduplicateFilterMatchWeightText == null || this._smmergeduplicateFilterMatchWeightText.length == 0) {
                this._smmergeduplicateFilterMatchWeightText = $("#smmergeduplicateFilterMatchWeightText");
            }
            return this._smmergeduplicateFilterMatchWeightText;
        },

        _smmergeduplicateMergeDupesDivContainer: null,
        smmergeduplicateMergeDupesDivContainer: function () {
            if (this._smmergeduplicateMergeDupesDivContainer == null || this._smmergeduplicateMergeDupesDivContainer.length == 0) {
                this._smmergeduplicateMergeDupesDivContainer = $("#smmergeduplicateMergeDupesDivContainer");
            }
            return this._smmergeduplicateMergeDupesDivContainer;
        },

        _smmergeduplicateHdnSelectedFilter: null,
        smmergeduplicateHdnSelectedFilter: function () {
            if (this._smmergeduplicateHdnSelectedFilter == null || this._smmergeduplicateHdnSelectedFilter.length == 0) {
                this._smmergeduplicateHdnSelectedFilter = $("[id$='hdnSelectedFilter']");
            }
            return this._smmergeduplicateHdnSelectedFilter;
        },

        _smmergeduplicateMergeBtn: null,
        smmergeduplicateMergeBtn: function () {
            if (this._smmergeduplicateMergeBtn == null || this._smmergeduplicateMergeBtn.length == 0) {
                this._smmergeduplicateMergeBtn = $("#smmergeduplicateMergeBtn");
            }
            return this._smmergeduplicateMergeBtn;
        },

        _smmergeduplicateMergeDupesCloseBtn: null,
        smmergeduplicateMergeDupesCloseBtn: function () {
            if (this._smmergeduplicateMergeDupesCloseBtn == null || this._smmergeduplicateMergeDupesCloseBtn.length == 0) {
                this._smmergeduplicateMergeDupesCloseBtn = $("#smmergeduplicateMergeDupesCloseBtn");
            }
            return this._smmergeduplicateMergeDupesCloseBtn;
        },

        _smmergeduplicateErrorMsgText: null,
        smmergeduplicateErrorMsgText: function () {
            if (this._smmergeduplicateErrorMsgText == null || this._smmergeduplicateErrorMsgText.length == 0) {
                this._smmergeduplicateErrorMsgText = $("#smmergeduplicateErrorMsgText");
            }
            return this._smmergeduplicateErrorMsgText;
        },

        _smmergeduplicateMergeDupesOption: null,
        smmergeduplicateMergeDupesOption: function () {
            if (this._smmergeduplicateMergeDupesOption == null || this._smmergeduplicateMergeDupesOption.length == 0) {
                this._smmergeduplicateMergeDupesOption = $("#smmergeduplicateMergeDupesOption");
            }
            return this._smmergeduplicateMergeDupesOption;
        },

        _smmergeduplicateRightFilterIcon: null,
        smmergeduplicateRightFilterIcon: function () {
            if (this._smmergeduplicateRightFilterIcon == null || this._smmergeduplicateRightFilterIcon.length == 0) {
                this._smmergeduplicateRightFilterIcon = $("#smmergeduplicateRightFilterIcon");
            }
            return this._smmergeduplicateRightFilterIcon;
        },

        _smmergeduplicateHeading: null,
        smmergeduplicateHeading: function () {
            if (this._smmergeduplicateHeading == null || this._smmergeduplicateHeading.length == 0) {
                this._smmergeduplicateHeading = $("#smmergeduplicateHeading");
            }
            return this._smmergeduplicateHeading;
        },

        _smmergeduplicateMergeOptionContainer: null,
        smmergeduplicateMergeOptionContainer: function () {
            if (this._smmergeduplicateMergeOptionContainer == null || this._smmergeduplicateMergeOptionContainer.length == 0) {
                this._smmergeduplicateMergeOptionContainer = $("#smmergeduplicateMergeOptionContainer");
            }
            return this._smmergeduplicateMergeOptionContainer;
        },

        _smmergeduplicateLegsList: null,
        smmergeduplicateLegsList: function () {
            if (this._smmergeduplicateLegsList == null || this._smmergeduplicateLegsList.length == 0) {
                this._smmergeduplicateLegsList = $("#smmergeduplicateLegsList");
            }
            return this._smmergeduplicateLegsList;
        },

        _smmergeduplicateDeleteOriginalCheckbox: null,
        smmergeduplicateDeleteOriginalCheckbox: function () {
            if (this._smmergeduplicateDeleteOriginalCheckbox == null || this._smmergeduplicateDeleteOriginalCheckbox.length == 0) {
                this._smmergeduplicateDeleteOriginalCheckbox = $("#smmergeduplicateDeleteOriginalCheckbox");
            }
            return this._smmergeduplicateDeleteOriginalCheckbox;
        },

        _smmergeduplicateCopyTimeseriesCheckbox: null,
        smmergeduplicateCopyTimeseriesCheckbox: function () {
            if (this._smmergeduplicateCopyTimeseriesCheckbox == null || this._smmergeduplicateCopyTimeseriesCheckbox.length == 0) {
                this._smmergeduplicateCopyTimeseriesCheckbox = $("#smmergeduplicateCopyTimeseriesCheckbox");
            }
            return this._smmergeduplicateCopyTimeseriesCheckbox;
        }
    },
    setRunningTextMoreSectype: function () {
        $(".runningMore").each(function (ii, ee) {
            $(ee).find(".runningMore").unbind("click").bind('click', function (e) {
                var runningMoreClass = $(ee).find(".runningMore");
                var secNames = $(ee).closest(".sm_grid_title").attr("secnames").split(",");

                if (runningMoreClass.find(".moreSectypeCss").length === 0) {
                    var divElement = document.createElement("div");
                    divElement.className = "moreSectypeCss";
                    var divHeight = 0;
                    for (var i = 3; i < secNames.length; i++) {
                        $(divElement).append("<div>" + selectedSectypes[i].text + "</div>");
                        divHeight += 25;
                    }
                    $(divElement).css('height', divHeight);
                    var leftPos = runningMoreClass.offset().left;
                    var topPos = runningMoreClass.offset().top;
                    $(divElement).css('left', leftPos);
                    $(divElement).css('top', topPos + 15);
                    $(divElement).css('display', 'none');
                    runningMoreClass.append(divElement);
                }
                else {
                    var divElement = runningMoreClass.find(".moreSectypeCss");
                    runningMoreClass.remove(".moreSectypeCss");
                }
                $(divElement).slideToggle();
                e.stopPropagation();
            });
        });
    },
    sm_grid: (function () {
        var SMGrid = function () {
            this.count = 0;
        }

        var sm_grid = new SMGrid();

        SMGrid.prototype.init = function ($object) {
            var instanceId = ++sm_grid.count;
            if (!$object.hasOwnProperty("id")) {
                $object.id = "sm_grid_" + instanceId;
            }

            if (!$object.hasOwnProperty("container")) {
                $object.container = $("body");
            }

            if (!$object.hasOwnProperty("width")) {
                $object.width = "95%";
            }

            if ($object.hasOwnProperty("data") && typeof $object.data === "object" && $object.container instanceof jQuery) {
                createHTML($object);
            }
        }

        function createHTML($object) {

            if (smMergeDuplicate._presetValue != null && smMergeDuplicate._presetValue != undefined && smMergeDuplicate._presetValue != '')
                $('.smmergeduplicateFilterOuterDiv').css('display', 'none');

            var marginWidth = ((100 - parseInt($object.width)) / 100) * smMergeDuplicate._windowWidth;
            var gridWidth = (parseInt($object.width) / 100) * (smMergeDuplicate._windowWidth - marginWidth); //40 for margin from left and right
            var colWidth = Math.ceil(gridWidth / ($object.data.colHeaders.length + 1) - 20);

            var secNames = $object.data.title.split('%')[1].split(',');
            var secNamesText = "";
            if (secNames.length === 1) {
                secNamesText = secNames[0].toString();
            }
            else if (secNames.length === 2) {
                secNamesText = secNames[0].toString() + " and " + secNames[1].toString();
            }
            else if (secNames.length === 3) {
                secNamesText = secNames[0].toString() + ", " + secNames[1].toString() + " and " + secNames[2].toString();
            }
            else if (secNames.length > 3) {
                secNamesText = secNames[0].toString() + ", " + secNames[1].toString() + ", " + secNames[2].toString() + " <span class='runningMore'>+" + (secNames.length - 3) + " more</span>";
            }

            var html = "<div id='" + $object.id + "' class='sm_grid_container' style='width:" + $object.width + "'>";
            html += "<div class='sm_grid_title' secnames = '" + secNames.toString() + "'>" + $object.data.title.split('%')[0] + "% " + secNamesText + "</div>";
            html += "<div class='sm_grid_grid_container'>";
            html += "<div class='sm_grid_header'>";
            if ($object.data.checkboxRequired) {
                html += "<div class='sm_grid_header_col sm_grid_horizontal_align sm_grid_checkbox'><input type='checkbox' /></div>";
            }
            var colHeadersLength = $object.data.colHeaders.length;
            for (var i = 0, z = 0; i < colHeadersLength; i++) {
                if (z === 0) {
                    if (smMergeDuplicate._productName == "refmaster")
                        html += "<div class='sm_grid_header_col sm_grid_col sm_grid_horizontal_align' title='Entity Code' style='width:" + colWidth + "px;'>Entity Code</div>";
                    else
                        html += "<div class='sm_grid_header_col sm_grid_col sm_grid_horizontal_align' title='Security ID' style='width:" + colWidth + "px;'>Security ID</div>";
                    z = 1;
                }
                html += "<div class='sm_grid_header_col sm_grid_col sm_grid_horizontal_align' title='" + $object.data.colHeaders[i] + "' style='width:" + colWidth + "px;'>" + $object.data.colHeaders[i] + "</div>";
            }
            html += "</div>";

            var rowKeys = Object.keys($object.data.mergeSecurities);
            var rowKeysLength = rowKeys.length;
            for (var j = 0; j < rowKeysLength; j++) {
                //                var securityTypeIndex = null;
                //                for (var item in $object.data.mergeSecurities[j].Value) {
                //                    if ($object.data.mergeSecurities[j].Value[item].Key === "Security Type") {
                //                        securityTypeIndex = item;
                //                    }
                //                }

                html += "<div class='sm_grid_row_container'><div class='sm_grid_row' row_id='" + j + "' sec_id='" + $object.data.mergeSecurities[j].Key + "'>";
                if ($object.data.checkboxRequired) {
                    html += "<div class='sm_grid_horizontal_align sm_grid_checkbox'><input type='checkbox' /></div>";
                }
                var column = $object.data.mergeSecurities[rowKeys[j]].Value;
                for (var k = 0, z = 0; k < column.length; k++) {
                    if (z === 0) {
                        html += "<div class='sm_grid_horizontal_align sm_grid_col sm_grid_security' col_name='Security ID' style='width:" + colWidth + "px;'>" + $object.data.mergeSecurities[rowKeys[j]].Key + "</div>"
                        z = 1;
                    }
                    html += "<div class='sm_grid_horizontal_align sm_grid_col' col_name='" + $object.data.colHeaders[k] + "' style='width:" + colWidth + "px;'>" + column[k].Value.Value.replaceAll(" 12:00:00", "") + "</div>"
                }
                html += "</div>";
            }
            html += "</div>";
            html += "</div>";
            html += "</div>";
            html += "</div>";

            $object.container.append(html);

            //Add More Sectype Dropdown to Title
            //smMergeDuplicate.setRunningTextMoreSectype();
            //            if (rowKeys.length > 7) {
            //                $object.container.find(".sm_grid_row_container").smslimscroll({ height: '140px', railVisible: true, alwaysVisible: true, size: '10px' });

            //                $object.container.find(".sm_grid_row_container").parent().find(".smslimScrollRail").css("left", (smMergeDuplicate._windowWidth - 20) + "px");
            //                $object.container.find(".sm_grid_row_container").parent().find(".smslimScrollBar").css("left", (smMergeDuplicate._windowWidth - 20) + "px");
            //            }
        }

        return sm_grid;
    })(),
    smMergeSecuritiesGrid: function (mergeSecuritiesData, attributeColumns) {
        //        var lastElement = mergeSecuritiesData.data[mergeSecuritiesData.data.length - 1];
        //        mergeSecuritiesData.data[mergeSecuritiesData.data.length - 1] = mergeSecuritiesData.data[0];
        //        mergeSecuritiesData.data[0] = lastElement;
        mergeSecuritiesData.data = mergeSecuritiesData.data.splice(0, mergeSecuritiesData.data.length - 1);


        var columns = Object.keys(mergeSecuritiesData.data);
        columns.unshift("Attributes"); //To add item at the start of the string

        var width = smMergeDuplicate._windowWidth;
        var cellWidth = smMergeDuplicate._mergeSecColumnWidth; //160 width of one cell
        var containerWidth = ((columns.length - 1) * cellWidth + 500);
        if (containerWidth > smMergeDuplicate._windowWidth) {
            width = containerWidth;
        }
        var highlightClass = "";
        var attrHeaderClass = "", attrRowClass = "", attrColClass = "";

        var html = "";

        html += "<div class='smmergeduplicateMergeGridForeground' style='z-index: 1001; width:" + width + "px;'>";
        for (var j = 0, mergeCol = false, attrCol = false; j < columns.length; j++) {
            //var attrNames = Object.keys(mergeSecuritiesData[0].Value);
            var attrNames = attributeColumns;

            if (j === 0) {
                attrCol = true;
            }
            else {
                attrCol = false;
            }

            //            if (j === 1) {
            //                mergeCol = true;
            //                //highlightClass = "smmergeduplicateHighlightedCell";
            //            }
            //            else {
            //                //highlightClass = "";
            //                mergeCol = false;
            //            }

            if (attrCol) {
                attrHeaderClass = "smmergeduplicateMergeSecAttrHeader";
                attrRowClass = "smmergeduplicateMergeSecAttr";
                attrColClass = "smmergeduplicateAttrColClass";

                html += "<div class='smmergeduplicateMergeSecLeftContainer smmergeduplicateHorizontalAlign' style='width:" + ((cellWidth) + 62) + "px;' >"; // Left Container Div
                html += "<div class='smmergeduplicateMergeLeftGridHeader'></div>"; // Left Grid Header
                html += "<div class='smmergeduplicateMergeSecLeftRowsContainer'>"; // Left Rows Container 

                html += "<div class='" + attrColClass + " smmergeduplicateMergeSecColumn smmergeduplicateHorizontalAlign' sec_id='' style='width:" + cellWidth + "px;'>";
                for (var i = 0; i < attrNames.length; i++) {
                    html += "<div row_number='" + i + "' class='" + attrRowClass + " smmergeduplicateMergeSecCell smmergeduplicateMergeGridCellText smmergeduplicateHorizontalAlign " + highlightClass + "' style='width:" + (cellWidth + 2) + "px;' title='" + attrNames[i] + "' >" + attrNames[i] + "</div>"; //columns[1] is used to fetch all attributes from the EQST attributes
                }
                html += "</div>";

                html += "</div>"; //Left Rows Container End
                html += "</div>"; //Left Container End

                html += "<div class='smmergeduplicateMergeAllSecuritiesContanier smmergeduplicateHorizontalAlign' attr='abc' style='width:" + containerWidth + "px;' >"; //Right Container
                html += "<div class='smmergeduplicateMergeRightGridHeader'></div>"; // Right Grid Header
                html += "<div class='smmergeduplicateMergeSecRightRowsContanier'>"; //Right Rows Container
            }
            else {
                attrHeaderClass = "";
                attrRowClass = "";
                attrColClass = "";

                //For the second column which contains the final merged security
                //                if (mergeCol) {
                //                    html += "<div class='smmergeduplicateMergeSecFinalMergeColumn smmergeduplicateMergeSecColumn smmergeduplicateHorizontalAlign' sec_id='' style='width:" + cellWidth + "px;'>";
                //                    for (var i = 0; i < attrNames.length; i++) {
                //                        html += "<div row_number='" + i + "' class='smmergeduplicateMergeSecCell smmergeduplicateMergeSecEditableCell smmergeduplicateHorizontalAlign smmergeduplicateMergeGridCellText' style='width:" + cellWidth + "px;' title='" + mergeSecuritiesData.data[columns[j]].Value[i].Value + "' >" + mergeSecuritiesData.data[columns[j]].Value[i].Value + "</div>";
                //                    }
                //                    html += "</div>";

                //                    html += "</div>"; //Left Rows Container End
                //                    html += "</div>"; //Left Container End

                //                    html += "<div class='smmergeduplicateMergeAllSecuritiesContanier smmergeduplicateHorizontalAlign' attr='abc' style='width:" + containerWidth + "px;' >"; //Right Container
                //                    html += "<div class='smmergeduplicateMergeRightGridHeader'></div>"; // Right Grid Header
                //                    html += "<div class='smmergeduplicateMergeSecRightRowsContanier'>"; //Right Rows Container
                //                }
                //                else {
                var firstSecClass = "";
                //                    if (j === 2) {
                if (j === 1) {
                    firstSecClass = "smmergeduplicateFirstSecurity"
                }
                else {
                    firstSecClass = "";
                }

                html += "<div class='" + attrColClass + " smmergeduplicateMergeSecColumn smmergeduplicateHorizontalAlign' sec_id='" + mergeSecuritiesData.data[j - 1].Key + "' style='width:" + cellWidth + "px;'>";
                for (var i = 0; i < attrNames.length; i++) {
                    if (mergeSecuritiesData.data[columns[j]].Value[i].IsHighlighted) {
                        highlightClass = "smmergeduplicateHighlightedCell";
                    }
                    else {
                        highlightClass = "";
                    }

                    html += "<div row_number='" + i + "' class='" + attrRowClass + " " + firstSecClass + " smmergeduplicateMergeSecCell smmergeduplicateMergeSecNonEditableCell smmergeduplicateMergeGridCellText smmergeduplicateHorizontalAlign " + highlightClass + "' style='width:" + cellWidth + "px;' title='" + mergeSecuritiesData.data[columns[j]].Value[i].Value + "' >" + mergeSecuritiesData.data[columns[j]].Value[i].Value + "</div>";
                }
                html += "</div>";
                //                }

                if (j === columns.length - 1) {
                    html += "</div>"; //Right Rows Container
                    html += "</div>"; // Right Container End
                }

            }
        }
        html += "</div>";

        //smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smslimScrollDiv").remove();
        smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeDupesDiv").html(html);
        //Attach Headers
        smMergeDuplicate.addGridHeaders(mergeSecuritiesData);
        //Add Background Row Styling
        smMergeDuplicate.addBackgroundStylingMergeGrid();
        //Add smSlimscroll
        smMergeDuplicate.attachMergeSecScrollbars(containerWidth);
        //Attach event handlers
        smMergeDuplicate.smMergeSecuritiesGridHandlers();

        smMergeDuplicate.adjustGridWidth();
    },
    adjustGridWidth: function () {
        var leftWidth = smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeSecLeftContainer").width();
        var rightWidth = smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeAllSecuritiesContanier").width();

        smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeDupesDiv").width(leftWidth + rightWidth + 100);
        //smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeDupesDiv").css("margin", "10px auto");

        //smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeAllSecuritiesContanier").width(smMergeDuplicate._windowWidth - leftWidth - 300);
        //smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeAllSecuritiesContanier").parent().width(smMergeDuplicate._windowWidth - leftWidth - 300);
        if (leftWidth + rightWidth + 100 > $(window).width()) {
            smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeAllSecuritiesContanier").width($(window).width() - leftWidth - 60);
            smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeAllSecuritiesContanier").parent().width($(window).width() - leftWidth - 60);
        }
        else {
            smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeAllSecuritiesContanier").width($(window).width() - leftWidth - 350);
            smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeAllSecuritiesContanier").parent().width($(window).width() - leftWidth - 350);

        }


        smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeRightGridHeader").width(rightWidth + 100);
    },
    attachMergeSecScrollbars: function (containerWidth) {
        smMergeDuplicate.gridScrollBarsFunction.resetScrolls((smMergeDuplicate._windowHeight - 150), (containerWidth));
        smMergeDuplicate.gridScrollBarsFunction.fixHorizontalScroll();

        //Adjustments
        var gridContainer = smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeDupesDiv");
        gridContainer.find(".smmergeduplicateMergeAllSecuritiesContanier").parent().addClass("smmergeduplicateHorizontalAlign");
        var leftWidth = gridContainer.find(".smmergeduplicateMergeSecLeftContainer").width();
        var rightWidth = gridContainer.find(".smmergeduplicateMergeAllSecuritiesContanier").width();

        gridContainer.find("#inner").parent().parent().addClass("smmergeduplicateMergeSecMainScroll");
        gridContainer.find("#inner").parent().parent().css("top", "70px");
        gridContainer.find("#inner").parent().parent().css("left", (smMergeDuplicate._windowWidth - 20) + "px");

        if ((leftWidth + rightWidth + 100) < smMergeDuplicate._windowWidth) {
            gridContainer.find(".slimScrollBarH").css("visibility", "hidden");
            gridContainer.find(".slimScrollRailH").css("visibility", "hidden");
        }
    },
    addGridHeaders: function (mergeSecuritiesData) {
        var leftHtml = "";
        var rightHtml = "";
        smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeSecColumn").each(function (ii, ee) {
            if (ii === 0) {
                leftHtml += "<div class='smmergeduplicateMergeSecAttrHeader smmergeduplicateHorizontalAlign' sec_id='' style='width:" + (smMergeDuplicate._mergeSecColumnWidth) + "px;margin:0px 15px;' ></div>";
            }
                //            else if (ii === 1) {
                //                if (smMergeDuplicate._productName == "refmaster")
                //                    leftHtml += "<div class='smmergeduplicateMergeSecHeader smmergeduplicatePrimaryHeader smmergeduplicateHorizontalAlign' sec_id='' style='width:" + (smMergeDuplicate._mergeSecColumnWidth + 2) + "px;margin:0px 15px;' >FINAL ENTITY</div>";
                //                else
                //                    leftHtml += "<div class='smmergeduplicateMergeSecHeader smmergeduplicatePrimaryHeader smmergeduplicateHorizontalAlign' sec_id='' style='width:" + (smMergeDuplicate._mergeSecColumnWidth + 2) + "px;margin:0px 15px;' >FINAL SECURITY</div>";
                //            }
            else {
                rightHtml += "<div class='smmergeduplicateMergeSecHeader smmergeduplicateHorizontalAlign' sec_id='" + mergeSecuritiesData.data[ii - 1].Key + "' title='Please select this security to make it Primary' style='width:" + (smMergeDuplicate._mergeSecColumnWidth + 2) + "px;margin:0px 15px;' >" + mergeSecuritiesData.data[ii - 1].Key + "</div>";
            }
        });

        smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeLeftGridHeader").html(leftHtml);
        smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeRightGridHeader").html(rightHtml);

    },
    addBackgroundStylingMergeGrid: function () {
        var html = "";
        smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeGridForeground").find(".smmergeduplicateMergeSecAttr").each(function (ii, ee) {
            var topPosition = $(ee).position().top + parseInt($(ee).css('margin-top').split('px'));
            var leftPosition = $(ee).position().left + 15;

            var noOfColumns = smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeGridForeground").find(".smmergeduplicateMergeSecColumn").length;
            var width = ((smMergeDuplicate._mergeSecColumnWidth + 30) * (2)) - 20;
            var html = "<div class='smmergeduplicateBackgroundRow' style='top:" + topPosition + "px;left:" + leftPosition + "px;width:" + width + "px;'></div>";
            ee.insertAdjacentHTML('beforebegin', html);
        });

        smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeAllSecuritiesContanier").find(".smmergeduplicateFirstSecurity").each(function (ii, ee) {
            var topPosition = $(ee).position().top + parseInt($(ee).css('margin-top').split('px'));
            var leftPosition = $(ee).position().left - 16;

            var noOfColumns = smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeGridForeground").find(".smmergeduplicateMergeSecColumn").length;
            var width = ((smMergeDuplicate._mergeSecColumnWidth + 30) * (noOfColumns - 1)) - 15;
            var html = "<div class='smmergeduplicateBackgroundRow' style='top:" + topPosition + "px;left:" + leftPosition + "px;width:" + width + "px; border-left:1px solid rgb(0,0,0,0);'></div>";
            ee.insertAdjacentHTML('beforebegin', html);
        });
    },
    smMergeSecuritiesGridHandlers: function () {
        //Removed Functionality


        //smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().unbind('click').bind('click', function (e) {
        //    var target = $(e.target);
        //    if (target.hasClass('smmergeduplicateMergeSecNonEditableCell')) {
        //        var rowNumber = target.attr('row_number');
        //        //Remove Highlighted Class from that entire row
        //        smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeSecColumn").find("[row_number='" + rowNumber + "']").removeClass("smmergeduplicateHighlightedCell");

        //        var appendDataTo = smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeSecFinalMergeColumn").find("[row_number='" + rowNumber + "']");

        //        var allColumns = smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeSecColumn")
        //        allColumns.each(function (ii, ee) {
        //            if ($(ee).attr("is_expanded") === "true") {
        //                var rows = $(ee).find("[row_number='" + rowNumber + "']");
        //                rows.each(function (iii, eee) {
        //                    if (!$(eee).hasClass("smmergeduplicateNewRightColumn")) {
        //                        appendDataTo = $(eee);
        //                    }
        //                });
        //            }
        //        });
        //        appendDataTo.text(target.text().trim());

        //        //Add Highlighted Class to selected div
        //        target.addClass("smmergeduplicateHighlightedCell");
        //    }

        //    //            if (target.hasClass('smmergeduplicateMergeSecHeader')) {
        //    //                var selectedSecId = target.attr('sec_id');
        //    //                var column = $(smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find("[sec_id='" + selectedSecId + "']")[1]);

        //    //                //Remove the highlighting from the grid
        //    //                smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateHighlightedCell").removeClass("smmergeduplicateHighlightedCell");

        //    //                //Loop through the selected column and copy values from the selected column to the final column
        //    //                column.find(".smmergeduplicateMergeSecNonEditableCell").each(function (ii, ee) {
        //    //                    $(ee).addClass("smmergeduplicateHighlightedCell");
        //    //                    var currRowText = $(ee).text();
        //    //                    var rowNumber = $(ee).attr("row_number");
        //    //                    smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeSecFinalMergeColumn").find("[row_number='" + rowNumber + "']").text(currRowText);
        //    //                });

        //    //            }
        //});
    },
    contractColumn: function (targetDiv) {
        targetDiv.removeClass("smmergeduplicateSelectedSecurity");
        targetDiv.find(".smmergeduplicateOldLeftColumn").remove();
        targetDiv.find(".smmergeduplicateNewRightColumn").removeClass("smmergeduplicateNewRightColumn");
        targetDiv.width(smMergeDuplicate._mergeSecColumnWidth);
        targetDiv.css("box-shadow", "0px 0px 0px 0px #fff");
        targetDiv.attr("is_expanded", "false");

        //Securities Container Div
        //var securitiesContainer = smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeAllSecuritiesContanier").children();
        //var securitiesContainerWidth = securitiesContainer.width();
        //securitiesContainer.width(securitiesContainerWidth - smMergeDuplicate._mergeSecColumnWidth);
    },
    smMergeSecuritiesCreateDropdown: function (eleId, data) {
        var securityData = [];
        for (var i = 1; i < data.length; i++) {
            var tempObj = {};
            tempObj.text = data[i];
            tempObj.value = i;
            securityData.push(tempObj);
        }
        smMergeDuplicate.createSMSelectDropDown($("#smmergeduplicateMergeSecDropDown"), securityData, true, "150", null, [securityData[0].text], smMergeDuplicate.smMergeSecuritiesOnChangeHandler);
    },
    smMergeSecuritiesOnChangeHandler: function (e) {
        var selectedSecurity = smselect.getSelectedOption($(e.currentTarget))[0].text;
        smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeSecCell").removeClass("smmergeduplicateHighlightedCell");
        smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find("[sec_id='" + selectedSecurity + "']").find(".smmergeduplicateMergeSecNonEditableCell").each(function (index, ele) {
            smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeSecFinalMergeColumn").find("[row_number='" + index + "']").find('input').val($(this).text().trim());
            $(this).addClass("smmergeduplicateHighlightedCell");
        });
    },
    gridScrollBarsFunction: {
        resetScrolls: function (scrollHeight, scrollWidth) {
            var gridContainer = smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeDupesDiv");
            var currentTab = gridContainer.find(".smmergeduplicateMergeAllSecuritiesContanier");
            var currentTabId = $(currentTab).prop('id');

            var height = scrollHeight + 57;
            if (!scrollHeight || scrollHeight <= 0) {
                var grid1 = gridContainer.find(".smmergeduplicateMergeSecLeftRowsContainer").slimscroll({
                    height: '360px', railVisible: false, alwaysVisible: false, size: '10px'
                });
                gridContainer.find("#smmergeduplicateMergeSecRightRowsContanier").slimscroll({
                    height: '360px', railVisible: false, alwaysVisible: false, size: '10px'
                });
                if (typeof scrollWidth !== 'undefined')
                    gridContainer.find(".smmergeduplicateMergeAllSecuritiesContanier").slimscrollH({
                        height: '360px', width: '960px', railVisible: true, alwaysVisible: true, size: '10px'
                    });
            }

            else {
                gridContainer.find(".smmergeduplicateMergeSecLeftRowsContainer").css('height', scrollHeight);
                gridContainer.find("#smmergeduplicateMergeSecRightRowsContanier").css({
                    'height': scrollHeight, 'width': scrollWidth
                });

                var grid1 = gridContainer.find(".smmergeduplicateMergeSecLeftRowsContainer").slimscroll({
                    height: scrollHeight, color: 'transparent', railVisible: false, alwaysVisible: false, size: '10px'
                });
                var grid2 = gridContainer.find(".smmergeduplicateMergeSecRightRowsContanier").slimscroll({
                    height: scrollHeight, color: 'transparent', width: scrollWidth, railVisible: false, alwaysVisible: false, size: '10px'
                });

                gridContainer.find(".smmergeduplicateMergeSecLeftRowsContainer").parent().height(gridContainer.find(".smmergeduplicateMergeSecRightRowsContanier").parent().height());
                if (typeof scrollWidth !== 'undefined') {
                    gridContainer.find(".smmergeduplicateMergeAllSecuritiesContanier").slimscrollH({
                        height: height + 'px', width: scrollWidth + 'px', railVisible: true, alwaysVisible: true, size: '10px'
                    });
                }
            }

            smMergeDuplicate.gridScrollBarsFunction.syncScrolls(grid1, grid2);
        },
        syncScrolls: function (grid1, grid2) {
            $(grid1).on('scroll', function () {
                $(grid2).slimScroll({
                    scrollTo: $(grid1).scrollTop()
                });

            });

            $(grid2).on('scroll', function () {
                $(grid1).slimScroll({
                    scrollTo: $(grid2).scrollTop()
                });
            });
        },
        fixHorizontalScroll: function () {
            var gridContainer = smMergeDuplicate.controls.smmergeduplicateMergeDupesDivContainer().find(".smmergeduplicateMergeDupesDiv");
            var module = gridContainer;
            var off = $(module).offset();
            var top = parseInt(off.top) + 78;
            var left = parseInt(off.left) + $(module).width();

            var dataRows = gridContainer.find(".smmergeduplicateMergeSecRightRowsContanier");
            var dataRowsHeight = $(dataRows).height();

            var html = '<div id="scrollbar"><div id="inner"><div id="in"></div></div></div>';

            var existingScrollbar = $('#rightSection').find('#scrollbar');

            if ($(existingScrollbar).length > 0)
                $(existingScrollbar).remove();

            gridContainer.find(".smmergeduplicateMergeAllSecuritiesContanier").append(html);
            $('#in').height($(dataRows)[0].scrollHeight);
            $('#inner').height(dataRowsHeight);

            var inner = $('#inner').slimScroll({
                height: dataRowsHeight, railVisible: true, alwaysVisible: true, size: '10px'
            });
            $('#scrollbar').css({
                'height': dataRowsHeight, 'top': top, 'left': left
            });

            $(dataRows).parent().find('.slimScrollBar').addClass('importantRule');
            smMergeDuplicate.gridScrollBarsFunction.specialSyncScrolls(gridContainer.find(".smmergeduplicateMergeSecLeftRowsContainer"), dataRows, inner);
        },
        specialSyncScrolls: function (grid1, grid2, inner) {
            $(grid1).on('scroll', function () {
                $(grid2).slimScroll({
                    scrollTo: $(grid1).scrollTop(), color: 'transparent', railVisible: false, alwaysVisible: false, size: '10px'
                });
                $(inner).slimScroll({
                    scrollTo: $(grid1).scrollTop()
                });
            });

            $(grid2).on('scroll', function () {
                $(grid1).slimScroll({ scrollTo: $(grid2).scrollTop() });
                $(inner).slimScroll({
                    scrollTo: $(grid2).scrollTop()
                });
            });

            $(inner).on('scroll', function () {
                $(grid1).slimScroll({ scrollTo: $(inner).scrollTop() });
                $(grid2).slimScroll({
                    scrollTo: $(inner).scrollTop(), color: 'transparent', railVisible: false, alwaysVisible: false, size: '10px'
                });
            });
        }
    },
    setSecurityIDsInSession: function (secIds, callbackFunction) {
        $.ajax({
            method: "POST",
            url: smMergeDuplicate._path + smMergeDuplicate._smServiceASMXLocation + "/SetValuesToServiceObjectForDeleteSecurity",
            data: JSON.stringify({
                "secIds": secIds
            }),
            contentType: "application/json",
            dataType: "json",
            success: function () {
                if (typeof (callbackFunction) === "function")
                    callbackFunction();
            },
            error: function () {
                console.log("Error Setting SecIds in Session");
            }
        });
    }
}

//$(document).ready(function () {
//    smMergeDuplicate.init();
//});

//window.onload = function () {
//    smMergeDuplicate.preInit();
//}

$(document).click(function (e) {
    if (e.target.type !== "checkbox") {
        if ($("#smmergeduplicateCreatePresetDiv").css("display") === "block") {
            $("#smmergeduplicateCreatePresetDiv").hide();
        }

        if ($("#smmergeduplicateMergeOptionContainer").css("display") === "block") {
            $("#smmergeduplicateMergeOptionContainer").hide();
        }
    }
});

$(window).resize(function () {


    var windowWidth = $(window).width();

    $('[id^=smselect_smmergeduplicatePrimaryAttributeDropDown]').width(0.21875 * windowWidth);
    var totalWidthOfContent = $("#smselect_smmergeduplicatePrimaryAttributeDropDown").find(".smselectanchorcontainer").width() + $("#smselect_smmergeduplicatePrimaryAttributeDropDown").find(".smselectimage ").width();
    var totalWidth = $("#smselect_smmergeduplicatePrimaryAttributeDropDown").find(".smselectrun").width();
    $("#smselect_smmergeduplicatePrimaryAttributeDropDown").find(".smselectrun").css("padding-left", ((totalWidth - totalWidthOfContent - 2) / 2));
    $("#smselect_smmergeduplicatePrimaryAttributeDropDown").find(".smselectrun").css("padding-right", ((totalWidth - totalWidthOfContent - 2) / 2));



    $('[id^=smselect_smmergeduplicatePrimaryMatchTypeDropDown]').width(windowWidth * 0.1875);
    var totalWidthOfContent = $("#smselect_smmergeduplicatePrimaryMatchTypeDropDown").find(".smselectanchorcontainer").width() + $("#smselect_smmergeduplicatePrimaryMatchTypeDropDown").find(".smselectimage ").width();
    var totalWidth = $("#smselect_smmergeduplicatePrimaryMatchTypeDropDown").find(".smselectrun").width();
    $("#smselect_smmergeduplicatePrimaryMatchTypeDropDown").find(".smselectrun").css("padding-left", ((totalWidth - totalWidthOfContent - 2) / 2));
    $("#smselect_smmergeduplicatePrimaryMatchTypeDropDown").find(".smselectrun").css("padding-right", ((totalWidth - totalWidthOfContent - 2) / 2));

    smMergeDuplicate.controls.smmergeduplicatePrimaryWeightText().parent().width(smMergeDuplicate.getInputElementWidth());


    //setting width for percentages

    $('[id^=smmergeduplicatePrimaryWeightText_]').parent().width($("#smmergeduplicatePrimaryWeightText").parent().width());
    //handling for resolutions less than 1200 pixel
    var num = 0.025 * windowWidth;
    $('[id^=smmergeduplicatePrimaryWeightText]').css("margin-left", num + "%");
    if (windowWidth < 1200) {
        $('[id^=smmergeduplicatePrimaryWeightText]').css("margin-left", "0%");
    }

    smMergeDuplicate.adjustGridWidth();

    //resizing the match criteria page 
    $(".sm_grid_horizontal_align").not(".sm_grid_checkbox").width(($(window).width() * 0.85) / $(".sm_grid_header").first().children().length - 1);
    $("#smmergeduplicateGridsContainer").height($(window).height() * 0.93);
    $("#smmergeduplicateGridsContainer").parent().height($(window).height() * 0.93);

});

function applyCorrectInputBinding() {
    $("input[id^='smmergeduplicatePrimaryWeightText'],input[id='smmergeduplicateFilterMatchWeightText'],input[id^='toleranceInput'],input[id^='smmergeduplicateToleranceInputRow']").on("keypress", function (event) {
        //backspace, delete
        var regex = new RegExp("[0-9]|%");
        var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
        if (!regex.test(key)) {
            event.preventDefault();
            return false;
        }
    });
}

//$(".smmergedTopFilterBackground").on("click", function () {
//    var selectedOptions = smselect.getSelectedOption($('#smselect_smmergeduplicateSectypeDropDown'));
//    if(!selectedOptions.length && smMergeDuplicate._productName.toLowerCase() === 'refmaster')
//    {
//        alert("Please select an entity type first");
//    }

//})

function checkForBooleanType(dataType) {
    if (dataType.toLowerCase() == "boolean" || dataType.toLowerCase() == "bit")
        return true;
    return false;
}





