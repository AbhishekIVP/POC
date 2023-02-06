//For Browsers that don't support Array.prototype.includes method
if (!Array.prototype.includes) {
    Array.prototype.includes = function (searchElement /*, fromIndex*/) {
        'use strict';
        if (this == null) {
            throw new TypeError('Array.prototype.includes called on null or undefined');
        }

        var O = Object(this);
        var len = parseInt(O.length, 10) || 0;
        if (len === 0) {
            return false;
        }
        var n = parseInt(arguments[1], 10) || 0;
        var k;
        if (n >= 0) {
            k = n;
        } else {
            k = len + n;
            if (k < 0) { k = 0; }
        }
        var currentElement;
        while (k < len) {
            currentElement = O[k];
            if (searchElement === currentElement ||
         (searchElement !== searchElement && currentElement !== currentElement)) { // NaN !== NaN
                return true;
            }
            k++;
        }
        return false;
    };
}

var workflowSetup = new CommonUtils();
workflowSetup._path = "";
workflowSetup._windowHeight = null;
workflowSetup._windowWidth = null;
workflowSetup._username = "admin";
workflowSetup._sectypeList = [];
workflowSetup._entityTypeList = [];
workflowSetup._commonServiceLocation = "/BaseUserControls/Service/CommonService.svc";
workflowSetup._userList = [];
workflowSetup._groupList = [];
workflowSetup._attrList = [];
workflowSetup._workflowList = [];
workflowSetup._workflowViewModel = null;
workflowSetup._sectypeIDVsName = {};
workflowSetup._entityTypeIDVsName = {};
workflowSetup._userLoginNameVsFirstNameLastName = {};
workflowSetup._setRefWorkflow = true;
workflowSetup._isDemoBuild = false;
workflowSetup._productName = null;
workflowSetup._recentlyCreatedWorkflowInstanceID = -1;
workflowSetup._moduleMapping = {
    "SECMASTER": 0,
    "REFMASTER": 1
};
workflowSetup._workflowTypeMapping = {
    1 : "ATTRIBUTE LEVEL",
    2 : "ASSET LEVEL",
    3 : "LEG LEVEL"
};
workflowSetup._rmWorkflowTypeMapping = [];
workflowSetup._defaultModule = workflowSetup._moduleMapping["SECMASTER"];
workflowSetup._maxSequence = 5;
workflowSetup.setPath = function () {
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    workflowSetup._path = path;
};
workflowSetup.initServerSideValues = function () {
    var info = workflowSetup_info;
    workflowSetup._username = info.Username;
    workflowSetup._cultureShortDate = info.CultureShortDateFormat;
    workflowSetup._setRefWorkflow = true;
    workflowSetup._isDemoBuild = info.IsDemoBuild;
    workflowSetup._productName = info.ProductName;
}
workflowSetup.getAllEntityTypes = function () {
    //Get All Entity Types
    workflowSetup.getEntityTypesList().then(function (data) {
        for (var item in data) {
            workflowSetup._entityTypeIDVsName[data[item].value] = data[item].text;
        }
        workflowSetup.initGetAllWorkflow();
    });
}
workflowSetup.initGetAllWorkflow = function () {
    //Get All Usernames for displaying firstname and lastname w.r.t login name
    workflowSetup.getUsers().then(function (usersData) {
        workflowSetup.massageUserData(usersData);

        if (workflowSetup._productName.toLowerCase() === "refmaster") {
            workflowSetup._defaultModule = workflowSetup._moduleMapping["REFMASTER"];

            //Initialize Page based on Workflows List Returned from DB
            workflowSetup.rmGetAllWorkflows().then(function (data) {
                if (data.length === 0) {
                    workflowSetup.controls.workflowSetup_pageText().text("ADD NEW WORKFLOW");
                }
                else {
                    workflowSetup.controls.workflowSetup_pageText().text("PLEASE SELECT WORKFLOW");
                    for (var item in data) {
                        workflowSetup._workflowViewModel.workflowList.push(new workflowSetup.workflowInstanceViewModel(data[item]));
                    }

                    if (data.length > 6) {
                        workflowSetup.controls.workflowSetup_workflowList().closest(".col-sm-12").smslimscroll({ "height": (6 * 95) + "px" });
                        workflowSetup.controls.workflowSetup_workflowList().closest(".col-sm-12").css("width", "100%");
                    }
                }
                workflowSetup.initModule();
                onServiceUpdated();
            });
        }
        else {
            //Initialize Page based on Workflows List Returned from DB
            workflowSetup.getAllWorkflows(workflowSetup._setRefWorkflow).then(function (data) {
                if (data.length === 0) {
                    workflowSetup.controls.workflowSetup_pageText().text("ADD NEW WORKFLOW");
                }
                else {
                    workflowSetup.controls.workflowSetup_pageText().text("PLEASE SELECT WORKFLOW");
                    for (var item in data) {
                        workflowSetup._workflowViewModel.workflowList.push(new workflowSetup.workflowInstanceViewModel(data[item]));
                    }

                    if (data.length > 6) {
                        workflowSetup.controls.workflowSetup_workflowList().closest(".col-sm-12").smslimscroll({ "height": (6 * 95) + "px" });
                        workflowSetup.controls.workflowSetup_workflowList().closest(".col-sm-12").css("width", "100%");
                    }
                }
                workflowSetup.initModule();
                onServiceUpdated();
            });
        }
    });
}
workflowSetup.populateWorkflowTypeMapping = function () {
    workflowSetup.rmGetWorkflowType().then(function (data) {
        workflowSetup._rmWorkflowTypeMapping = data;
    });
};
workflowSetup.init = function () {
    workflowSetup.setPath();

    onServiceUpdating();

    //Initialize Server Side Values
    workflowSetup.initServerSideValues();

    workflowSetup.populateWorkflowTypeMapping();

    if (workflowSetup._productName.toLowerCase() === "refmaster") {
        workflowSetup.getAllEntityTypes();
    }
    else {
        //Get All Security Types
        workflowSetup.getSecurityTypesList().then(function (data) {
            for (var item in data) {
                workflowSetup._sectypeIDVsName[data[item].value] = data[item].text;
            }
            workflowSetup._sectypeIDVsName[-1] = "All Security Types";
            if (workflowSetup._setRefWorkflow) {
                workflowSetup.getAllEntityTypes();
            }
            else {
                workflowSetup.initGetAllWorkflow();
            }
        });
    }

    //Initialize Knockout View Model
    workflowSetup._workflowViewModel = new workflowSetup.workflowInstanceViewModel();
    ko.applyBindings(workflowSetup._workflowViewModel, workflowSetup.controls.workflowSetup_container()[0]);

    //Set Window Height and Width
    workflowSetup._windowHeight = $(window).height();
    workflowSetup._windowWidth = $(window).width();
};
workflowSetup.initModule = function () {
    if (workflowSetup._defaultModule === 0) {
        workflowSetup.controls.workflowSetup_moduleSelector().children('div:first').click();
    }
    else {
        workflowSetup.controls.workflowSetup_moduleSelector().children('div:last').click();
    }
    smselect.setOptionByIndex($("#smselect_workflowSetup_workflowTypesDropdown"), 0);
};
workflowSetup.massageUserData = function (usersData) {
    for (var item in usersData) {
        var loginName = usersData[item].name.split("|")[0];
        var firstName = usersData[item].name.split("|")[1];
        var lastName = usersData[item].name.split("|")[2];
        workflowSetup._userLoginNameVsFirstNameLastName[loginName] = firstName + " " + lastName;
    }
}
workflowSetup.createWorkflowInit = function (workflowType) {
    if (workflowSetup._workflowViewModel.moduleType() === 0) {
        //For Initializing Sectype Dropdown
        //        if (workflowType[0].text.toLowerCase() === "attribute level")
        //            workflowSetup.initializeSectypeDropdown();
        //        else if (workflowType[0].text.toLowerCase() === "security level") {
        //            workflowSetup.initializeSectypeDropdown();
        //            $('.workflowSetup_attributeName').off('onClickAttribute');
        //        }

        workflowSetup.initializeSectypeDropdown();
    }
    else {
        //For Initializing Entity Dropdown
        workflowSetup.initializeEntityTypeDropdown();
    }

    //For Initializing Workflow Dropdown
    workflowSetup.initWorkflowDetailsSection();

    //Get Users and Groups for Creating Workflow
    workflowSetup.getUsers().then(function (usersData) {
        workflowSetup.getGroups().then(function (groupsData) {
            //Users Data
            var arrMappedDataUsers = ko.utils.arrayMap(usersData, function (item) {
                var name = item.name.split("|")[0];
                item.name = name;
                return new workflowSetup.userViewModel(item);
            });
            workflowSetup._workflowViewModel.workflowSetupInstance().users.push.apply(workflowSetup._workflowViewModel.workflowSetupInstance().users, arrMappedDataUsers);
            if ((usersData.length * 40) > (workflowSetup._windowHeight - 150)) {
                workflowSetup.controls.workflowSetup_userSearchResults().smslimscroll({ "height": workflowSetup._windowHeight - 240 + "px" });
            }

            //Groups Data
            var arrMappedDataGroups = ko.utils.arrayMap(groupsData, function (item) {
                return new workflowSetup.groupViewModel(item);
            });
            workflowSetup._workflowViewModel.workflowSetupInstance().groups.push.apply(workflowSetup._workflowViewModel.workflowSetupInstance().groups, arrMappedDataGroups);
            if ((groupsData.length * 40) > (workflowSetup._windowHeight - 150)) {
                workflowSetup.controls.workflowSetup_groupSearchResults().smslimscroll({ "height": workflowSetup._windowHeight - 240 + "px" });
            }
        });
    });

    //Set Attribute Column Height
    workflowSetup.controls.workflowSetup_attributeContainerSection().find(".workflowSetup_attributeColumn").height(workflowSetup._windowHeight - 110);
};
workflowSetup.updateWorkflowInit = function (data) {
    if (workflowSetup._workflowViewModel.moduleType() === 0) {
        //For Initializing Sectype Dropdown
        workflowSetup.initializeSectypeDropdown(data.sectypeID(), data);
    }
    else {
        //For Initializing Entity Dropdown
        workflowSetup.initializeEntityTypeDropdown(data.entityTypeID(), data);
    }

    //For Initializing Workflow Dropdown
    workflowSetup.initWorkflowDetailsSection();

    //For Initializing Users and Groups
    workflowSetup.getUsers().then(function (usersData) {
        var currentInstance = data;
        workflowSetup.getGroups().then(function (groupsData) {
            var workflowInstance = currentInstance;

            usersData = usersData.filter(function (item) {
                var isPresent = false;
                for (var user in workflowSetup._workflowViewModel.workflowSetupInstance().userSelectedResults()) {
                    if (workflowSetup._workflowViewModel.workflowSetupInstance().userSelectedResults()[user].userLoginName() === item.name.split("|")[0]) {
                        isPresent = true;
                        break;
                    }
                }
                if (!isPresent) {
                    return item;
                }
            });

            //Users Data
            var arrMappedDataUsers = ko.utils.arrayMap(usersData, function (item) {
                var name = item.name.split("|")[0];
                item.name = name;
                return new workflowSetup.userViewModel(item);
            });
            workflowSetup._workflowViewModel.workflowSetupInstance().users.push.apply(workflowSetup._workflowViewModel.workflowSetupInstance().users, arrMappedDataUsers);
            var selectedUsersLength = workflowSetup._workflowViewModel.workflowSetupInstance().userSelectedResults().length;
            var usersHeight = workflowSetup._windowHeight - 240 - (selectedUsersLength * 40);
            if ((usersData.length * 40) > usersHeight) {
                workflowSetup.controls.workflowSetup_userSearchResults().smslimscroll({ "height": usersHeight + "px" });
            }

            groupsData = groupsData.filter(function (item) {
                var isPresent = false;
                for (var group in workflowSetup._workflowViewModel.workflowSetupInstance().groupSelectedResults()) {
                    if (workflowSetup._workflowViewModel.workflowSetupInstance().groupSelectedResults()[group].groupName() === item.name) {
                        isPresent = true;
                        break;
                    }
                }
                if (!isPresent) {
                    return item;
                }
            });
            //Groups Data
            var arrMappedDataGroups = ko.utils.arrayMap(groupsData, function (item) {
                return new workflowSetup.groupViewModel(item);
            });
            workflowSetup._workflowViewModel.workflowSetupInstance().groups.push.apply(workflowSetup._workflowViewModel.workflowSetupInstance().groups, arrMappedDataGroups);
            var selectedGroupsLength = workflowSetup._workflowViewModel.workflowSetupInstance().groupSelectedResults().length;
            var groupsHeight = workflowSetup._windowHeight - 240 - (selectedGroupsLength * 40);
            if ((groupsData.length * 40) > groupsHeight) {
                workflowSetup.controls.workflowSetup_groupSearchResults().smslimscroll({ "height": groupsHeight + "px" });
            }

            onServiceUpdated();
        });
    });

    //Set Attribute Column Height
    workflowSetup.controls.workflowSetup_attributeContainerSection().find(".workflowSetup_attributeColumn").height(workflowSetup._windowHeight - 110);
};
workflowSetup.initializeSectypeDropdown = function (sectypeIDs, workflowInstanceData) {
    var selectedItems = [];
    if (sectypeIDs !== undefined && sectypeIDs !== "-1") {
        selectedItems = sectypeIDs.split(",");
    }

    if (sectypeIDs === "-1") {
        var allSectypeIDs = "-1";
    }
    workflowSetup.getSecurityTypesList(selectedItems).then(function (data) {
        workflowSetup._sectypeList = data;
        var selSectypes = [];
        for (var item in data) {
            //workflowSetup._sectypeIDVsName[data[item].value] = data[item].text;
            selSectypes.push(data[item].text);
        }

        var multiData = [{ options: workflowSetup._sectypeList, text: "Security Types" }];

        var selectedItemsText = [];
        for (var sec in selectedItems) {
            selectedItemsText.push(workflowSetup._sectypeIDVsName[selectedItems[sec]]);
        }
        if (allSectypeIDs === "-1") {
            selectedItemsText = selSectypes;
        }

        if (workflowSetup._workflowViewModel.workflowSetupInstance().workflowMode().toLowerCase() === "save") {
            workflowSetup.createSMSelectDropDown(workflowSetup.controls.workflowSetup_sectypeDropDown(), multiData, true, "150", null, null, workflowSetup.initBasedOnSectype, selSectypes, true, "Sectypes", "All Security Types");
        }
        else {
            //workflowSetup.createSMSelectDropDown(workflowSetup.controls.workflowSetup_sectypeDropDown(), multiData, true, "150", null, "All Security Types", function () { return false; }, selectedItemsText, true, "Security Types");
            if (selectedItemsText.length === workflowSetup._sectypeList.length) {
                var sectypeText = "All Security Types"
            }
            else {
                var sectypeText = selectedItemsText[0] + ((selectedItemsText.length > 1) ? (" + " + selectedItemsText.length + " more") : "");
            }
            workflowSetup.controls.workflowSetup_sectypeDropDown().text(sectypeText);
            workflowSetup.controls.workflowSetup_sectypeDropDown().css("font-size", "14px");
        }
        workflowSetup.initBasedOnSectype(workflowInstanceData);
    });
};
workflowSetup.initializeEntityTypeDropdown = function (entityTypeIDs, workflowInstanceData) {
    var selectedItems = [];
    if (entityTypeIDs !== undefined && entityTypeIDs !== "-1") {
        selectedItems = entityTypeIDs.split(",");
    }

    workflowSetup.getEntityTypesList(selectedItems).then(function (data) {
        workflowSetup._entityTypeList = data;
        var selEntitytypes = [];
        for (var item in data) {
            selEntitytypes.push(data[item].text);
        }

        var selectedItemsText = [];
        for (var sec in selectedItems) {
            selectedItemsText.push(workflowSetup._entityTypeIDVsName[selectedItems[sec]]);
        }

        if (workflowSetup._workflowViewModel.workflowSetupInstance().workflowMode().toLowerCase() === "save") {
            workflowSetup.createSMSelectDropDown(workflowSetup.controls.workflowSetup_entityTypeDropDown(), workflowSetup._entityTypeList, true, "150", null, null, workflowSetup.initBasedOnEntityType, selEntitytypes, false, "Entity Types", "");
            smselect.setOptionByIndex(workflowSetup.controls.workflowSetup_entityTypeDropDown(), 0);
        }
        else {
            var entityTypeText = selectedItemsText[0] + ((selectedItemsText.length > 1) ? (" + " + selectedItemsText.length + " more") : "");

            workflowSetup.controls.workflowSetup_entityTypeDropDown().text(entityTypeText);
            workflowSetup.controls.workflowSetup_entityTypeDropDown().css("font-size", "14px");
            workflowSetup.initBasedOnEntityType(workflowInstanceData);
        }
    });
};
workflowSetup.initWorkflowDetailsSection = function () {
    if (workflowSetup._workflowViewModel.workflowSetupInstance().workflowMode().toLowerCase() === "update") {
        //workflowSetup.createSMSelectDropDown(workflowSetup.controls.workflowSetup_workflowInstanceDropDown(), workflowSetup._workflowList, true, "150", null, "Select Workflow", workflowSetup.initBasedOnWorkflow, [], false);
        var workflowName = workflowSetup._workflowViewModel.workflowName();
        workflowSetup.controls.workflowSetup_workflowInstanceDropDown().text(workflowName);
    }
    else {
        var workflowName = workflowSetup.controls.workflowSetup_newWorkflowName().val().trim();
        workflowSetup.controls.workflowSetup_workflowInstanceDropDown().text(workflowName);
    }
};
workflowSetup.controls = {
    workflowSetup_sectypeDropDown: function () {
        return $("#workflowSetup_sectypeDropDown");
    },
    workflowSetup_saveBtn: function () {
        return $("#workflowSetup_saveBtn");
    },
    workflowSetup_attributeSearchBox: function () {
        return $("#workflowSetup_attributeSearchBox");
    },
    workflowSetup_container: function () {
        return $("#workflowSetup_container");
    },
    workflowSetup_configuredAttributesSection: function () {
        return $("#workflowSetup_configuredAttributesSection");
    },
    workflowSetup_attributesSection: function () {
        return $("#workflowSetup_attributesSection");
    },
    workflowSetup_attributeContainerSection: function () {
        return $("#workflowSetup_attributeContainerSection");
    },
    workflowSetup_attributeConfigSection: function () {
        return $("#workflowSetup_attributeConfigSection");
    },
    workflowSetup_groupSearchResults: function () {
        return $("#workflowSetup_groupSearchResults");
    },
    workflowSetup_userSearchResults: function () {
        return $("#workflowSetup_userSearchResults");
    },
    workflowSetup_chooseWorkflowScreen: function () {
        return $("#workflowSetup_chooseWorkflowScreen");
    },
    workflowSetup_attributeContainerScreen: function () {
        return $("#workflowSetup_attributeContainerScreen");
    },
    workflowSetup_pageText: function () {
        return $("#workflowSetup_pageText");
    },
    workflowSetup_newWorkflowSetup: function () {
        return $("#workflowSetup_newWorkflowSetup");
    },
    workflowSetup_workflowList: function () {
        return $("#workflowSetup_workflowList");
    },
    workflowSetup_workflowInstanceDropDown: function () {
        return $("#workflowSetup_workflowInstanceDropDown");
    },
    workflowSetup_newWorkflowName: function () {
        return $("#workflowSetup_newWorkflowName");
    },
    workflowSetup_errorMsgDiv: function () {
        return $("#workflowSetup_errorMsgDiv");
    },
    workflowSetup_selectedAttributesSection: function () {
        return $("#workflowSetup_selectedAttributesSection");
    },
    workflowSetup_applyWorkflowOnCreate: function () {
        return $("#workflowSetup_applyWorkflowOnCreate");
    },
    workflowSetup_applyTimeSeries: function () {
        return $("#workflowSetup_applyTimeSeries");
    },
    workflowSetup_applyBlankValues: function () {
        return $("#workflowSetup_applyBlankValues");
    },
    workflowSetup_messagePopup: function () {
        return $("#workflowSetup_messagePopup");
    },
    workflowSetup_createBtn: function () {
        return $("#workflowSetup_createBtn");
    },
    workflowSetup_addBtn: function () {
        return $("#workflowSetup_addBtn");
    },
    workflowSetup_addNewWorkflow: function () {
        return $("#workflowSetup_addNewWorkflow");
    },
    workflowSetup_moduleSelector: function () {
        return $("#workflowSetup_moduleSelector");
    },
    workflowSetup_entityTypeDropDown: function () {
        return $("#workflowSetup_entityTypeDropDown");
    },
    workflowSetup_workflowTypesDropdown: function () {
        return $("#workflowSetup_workflowTypesDropdown");
    }
};
workflowSetup.initBasedOnSectype = function (workflowInstanceData) {
    if (workflowSetup._workflowViewModel.workflowSetupInstance().selectedAttributes().length === 0) {
        var sectypeIDs = "-1";
        var selectedSectypes = smselect.getSelectedOption($("#smselect_" + workflowSetup.controls.workflowSetup_sectypeDropDown().attr('id')));
        if (workflowInstanceData !== undefined && !workflowInstanceData.hasOwnProperty('type') && workflowInstanceData.sectypeID() !== "" && workflowInstanceData.sectypeID() !== null && workflowInstanceData.sectypeID() !== undefined) {
            sectypeIDs = workflowInstanceData.sectypeID();
        }
        else {
            if (selectedSectypes.length === 0 || selectedSectypes.length === workflowSetup._sectypeList.length) {
                sectypeIDs = "-1";
            }
            else {
                var tempArr = [];
                for (var item in selectedSectypes) {
                    tempArr.push(selectedSectypes[item].value);
                }
                sectypeIDs = tempArr.join(',');
            }
        }

        workflowSetup._workflowViewModel.workflowSetupInstance().sectypeID(sectypeIDs);
        //Set Attributes Container Height
        workflowSetup.controls.workflowSetup_attributeContainerSection().height(workflowSetup._windowHeight - 90);
        //Initialize Attributes
        if (workflowInstanceData !== undefined && workflowInstanceData.type === "change") {
            workflowSetup.initializeAttributesSection(sectypeIDs);
        }
        else {
            workflowSetup._workflowViewModel.workflowSetupInstance().oldSectypeIDs(sectypeIDs);
            workflowSetup.initializeAttributesSection(sectypeIDs, workflowInstanceData);
        }
    }
    else {
        return false;
    }
}
workflowSetup.initBasedOnEntityType = function (workflowInstanceData) {
    var entityTypeID = "";
    if (workflowSetup._workflowViewModel.workflowSetupInstance().workflowMode() === "Save") {
        var selectedEntityTypes = smselect.getSelectedOption($("#smselect_" + workflowSetup.controls.workflowSetup_entityTypeDropDown().attr('id')));
        entityTypeID = selectedEntityTypes[0].value;
        workflowSetup._workflowViewModel.workflowSetupInstance().entityTypeID();
    }
    else {
        entityTypeID = workflowInstanceData.entityTypeID();
    }

    //Set Attributes Container Height
    workflowSetup.controls.workflowSetup_attributeContainerSection().height(workflowSetup._windowHeight - 90);
    workflowSetup.initializeAttributesSectionRM(entityTypeID, workflowInstanceData);
}
workflowSetup.initializeAttributesSection = function (sectypeIDs, workflowInstanceData) {
    workflowSetup.getAttributesBasedOnSectype(sectypeIDs).then(function (data) {

        if (workflowInstanceData !== undefined) {
            //Selected Attributes in Case of Update Config
            var selectedAttrData = ko.utils.arrayMap(workflowInstanceData.attributeInfo(), function (item) {
                var tempObj = {};
                tempObj.attrName = item.attributeName();
                tempObj.attrID = item.attributeID();
                tempObj.isSelected = true;
                return new workflowSetup.singleAttributeViewModel(tempObj);
            });
        }

        //Populate Attribute Info
        workflowSetup._workflowViewModel.workflowSetupInstance().attributeInfo([]);
        workflowSetup._workflowViewModel.workflowSetupInstance().sectypeID(sectypeIDs);
        var arrMappedData = ko.utils.arrayMap(data, function (item) {
            return new workflowSetup.singleAttributeViewModel(item);
        });
        workflowSetup._workflowViewModel.workflowSetupInstance().attributeInfo.push.apply(workflowSetup._workflowViewModel.workflowSetupInstance().attributeInfo, arrMappedData);

        if (workflowInstanceData !== undefined) {
            workflowSetup._workflowViewModel.workflowSetupInstance().selectedAttributes(selectedAttrData);
        }

        if (data.length > 12) {
            if (workflowSetup._workflowViewModel.workflowSetupInstance().selectedAttributes().length === 0) {
                workflowSetup.controls.workflowSetup_attributesSection().smslimscroll({ height: (workflowSetup._windowHeight - 210) + 'px' });
            }
            else {
                workflowSetup.controls.workflowSetup_attributesSection().smslimscroll({ height: (workflowSetup._windowHeight - 360) + 'px' });
            }
        }

        if (workflowSetup._workflowViewModel.workflowSetupInstance().selectedAttributes().length >= 2) {
            workflowSetup.controls.workflowSetup_selectedAttributesSection().smslimscroll({ height: '115px' });
        }
    });
};
workflowSetup.initializeAttributesSectionRM = function (entityTypeIDs, workflowInstanceData) {
    if (workflowSetup._workflowViewModel.workflowSetupInstance().wType().toLowerCase().indexOf('leg') > -1) {
        workflowSetup.getLegsBasedOnEntityType(entityTypeIDs).then(function (data) {

            var selectedAttributeIds = {};
            if (workflowInstanceData !== undefined && typeof workflowInstanceData.attributeInfo === "function") {
                //Selected Attributes in Case of Update Config
                var selectedAttrData = ko.utils.arrayMap(workflowInstanceData.attributeInfo(), function (item) {
                    selectedAttributeIds[item.attributeID()] = "";
                    var tempObj = {};
                    tempObj.attrName = item.attributeName();
                    tempObj.attrID = item.attributeID();
                    tempObj.isSelected = true;
                    return new workflowSetup.singleAttributeViewModel(tempObj);
                });
            }

            //Populate Leg Info
            workflowSetup._workflowViewModel.workflowSetupInstance().attributeInfo([]);
            workflowSetup._workflowViewModel.workflowSetupInstance().entityTypeID(entityTypeIDs);
            var arrMappedData = ko.utils.arrayMap(data, function (item) {
                if (!selectedAttributeIds.hasOwnProperty(item.attrID)) {
                    return new workflowSetup.singleAttributeViewModel(item);
                }
            });

            arrMappedData = arrMappedData.filter(function (element) {
                return element !== undefined;
            });

            workflowSetup._workflowViewModel.workflowSetupInstance().attributeInfo.push.apply(workflowSetup._workflowViewModel.workflowSetupInstance().attributeInfo, arrMappedData);

            if (workflowInstanceData !== undefined && typeof workflowInstanceData.attributeInfo === "function") {
                workflowSetup._workflowViewModel.workflowSetupInstance().selectedAttributes(selectedAttrData);
            }

            if (data.length > 12) {
                if (workflowSetup._workflowViewModel.workflowSetupInstance().selectedAttributes().length === 0) {
                    workflowSetup.controls.workflowSetup_attributesSection().smslimscroll({ height: (workflowSetup._windowHeight - 210) + 'px' });
                }
                else {
                    workflowSetup.controls.workflowSetup_attributesSection().smslimscroll({ height: (workflowSetup._windowHeight - 360) + 'px' });
                }
            }

            if (workflowSetup._workflowViewModel.workflowSetupInstance().selectedAttributes().length >= 2) {
                workflowSetup.controls.workflowSetup_selectedAttributesSection().smslimscroll({ height: '115px' });
            }

        });
    }
    else {
        workflowSetup.getAttributesBasedOnEntityType(entityTypeIDs).then(function (data) {

            //Populate Attribute Info
            workflowSetup._workflowViewModel.workflowSetupInstance().selectedAttributes([]);
            workflowSetup._workflowViewModel.workflowSetupInstance().entityTypeID(entityTypeIDs);
            var arrMappedData = ko.utils.arrayMap(data, function (item) {
                return new workflowSetup.singleAttributeViewModel(item);
            });
            workflowSetup._workflowViewModel.workflowSetupInstance().selectedAttributes.push.apply(workflowSetup._workflowViewModel.workflowSetupInstance().selectedAttributes, arrMappedData);

            if (data.length > 12) {
                workflowSetup.controls.workflowSetup_selectedAttributesSection().css("max-height", "auto");
                workflowSetup.controls.workflowSetup_selectedAttributesSection().smslimscroll({ height: (workflowSetup._windowHeight - 200) + 'px' });
            }
        });
    }
};
workflowSetup.getSecurityTypesList = function (selectedSectypes) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: workflowSetup._path + workflowSetup._commonServiceLocation + "/GetAllSectypes",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ "selectedSectypes": selectedSectypes }),
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
};
workflowSetup.getEntityTypesList = function (selectedEntityTypes) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: workflowSetup._path + workflowSetup._commonServiceLocation + "/GetAllEntityTypes",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ "selectedEntityTypes": selectedEntityTypes }),
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
};
workflowSetup.rmGetWorkflowType = function () {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: workflowSetup._path + workflowSetup._commonServiceLocation + "/RMGetWorkflowType",
            contentType: "application/json",
            dataType: "json",
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
};
workflowSetup.getAttributesBasedOnSectype = function (sectypeIDs) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: workflowSetup._path + workflowSetup._commonServiceLocation + "/GetAllAttributesBasedOnSectypeSelection",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ "secTypeIds": sectypeIDs, "userName": workflowSetup._username }),
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
};
workflowSetup.getAttributesBasedOnEntityType = function (entityTypeID) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: workflowSetup._path + workflowSetup._commonServiceLocation + "/GetAttributeBasedOnEntityTypeSelection",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ "entityTypeId": entityTypeID }),
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
};
workflowSetup.getLegsBasedOnEntityType = function (entityTypeID) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: workflowSetup._path + workflowSetup._commonServiceLocation + "/GetLegBasedOnEntityTypeSelection",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ "entityTypeId": entityTypeID }),
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
};
workflowSetup.getUsers = function () {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: workflowSetup._path + workflowSetup._commonServiceLocation + "/GetAllUsersList",
            contentType: "application/json",
            dataType: "json",
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
};
workflowSetup.getGroups = function () {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: workflowSetup._path + workflowSetup._commonServiceLocation + "/GetAllGroupsList",
            contentType: "application/json",
            dataType: "json",
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
};
workflowSetup.saveWorkflowConfig = function (workflowInfo, workflowName, isCreate, applyTimeSeries, username, applyBlank) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: workflowSetup._path + workflowSetup._commonServiceLocation + "/SMSaveWorkflow",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ info: workflowInfo, workflowName: workflowName, isCreate: isCreate, applyTimeSeries: applyTimeSeries, username: username, applyBlankToNonBlank: applyBlank }),
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
};
workflowSetup.saveWorkflowConfigRM = function (workflowInfo, workflowName, isCreate, applyTimeSeries, username, applyBlank) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: workflowSetup._path + workflowSetup._commonServiceLocation + "/RMSaveWorkflow",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ info: workflowInfo, workflowName: workflowName, isCreate: isCreate, applyTimeSeries: applyTimeSeries, username: username, applyBlankToNonBlank: applyBlank }),
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
};
workflowSetup.updateWorkflowConfig = function (workflowInfo, workflowName, isCreate, applyTimeSeries, username, applyBlank, instanceId) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: workflowSetup._path + workflowSetup._commonServiceLocation + "/SMUpdateWorkflow",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ info: workflowInfo, workflowName: workflowName, isCreate: isCreate, applyTimeSeries: applyTimeSeries, username: username, instanceId: instanceId, applyBlankToNonBlank: applyBlank }),
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
}
workflowSetup.updateWorkflowConfigRM = function (workflowInfo, workflowName, isCreate, applyTimeSeries, username, applyBlank, instanceId) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: workflowSetup._path + workflowSetup._commonServiceLocation + "/RMUpdateWorkflow",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ info: workflowInfo, workflowName: workflowName, isCreate: isCreate, applyTimeSeries: applyTimeSeries, username: username, instanceId: instanceId, applyBlankToNonBlank: applyBlank }),
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
}
workflowSetup.getAllWorkflows = function (setRefWorkflow) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: workflowSetup._path + workflowSetup._commonServiceLocation + "/SMGetAllWorkflows",
            data: JSON.stringify({ "setRefWorkflow": setRefWorkflow }),
            contentType: "application/json",
            dataType: "json",
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
};
workflowSetup.rmGetAllWorkflows = function () {
    return new Promise(function (resolve, reject) {
        $.ajax({
            type: 'POST',
            url: workflowSetup._path + workflowSetup._commonServiceLocation + "/RMGetAllWorkflows",
            contentType: "application/json",
            dataType: "json",
            success: function (data) {
                resolve(data.d);
            },
            error: function (ex) {
                reject(ex);
            }
        });
    });
};
workflowSetup.workflowInstanceViewModel = function (data) {
    var self = this;

    self.onClickDummyFunction = function (obj, event) {
        event.stopPropagation();
    }

    self.showRefWorkflow = ko.computed(function () {
        return workflowSetup._setRefWorkflow;
    });

    self.isStandAloneRef = ko.computed(function () {
        return (workflowSetup._productName.toLowerCase() === "refmaster") ? true : false;
    });

    self.workflowName = ko.observable();
    if (data != undefined && data.workflowName !== undefined) {
        self.workflowName(data.workflowName);
    }

    self.workflowInstanceID = ko.observable();
    if (data != undefined && data.workflowID !== undefined) {
        self.workflowInstanceID(data.workflowID);
    }

    self.moduleType = ko.observable();
    if (data != undefined && data !== undefined) {
        self.moduleType(workflowSetup._moduleMapping[data.moduleType.toUpperCase()]);
    }

    self.workflowType = ko.observable();
    if (data !== undefined) {
        self.workflowType(workflowSetup._workflowTypeMapping[parseInt(data.workflowType)]);
    }

    self.isWorkflowTextBoxEditable = ko.observable(false);
    self.onClickEditWorkflowName = function (obj, event) {
        var target = $(event.target);
        if (target[0].tagName === 'I') {
            target = target.parent();
        }

        if (!self.isWorkflowTextBoxEditable()) {
            self.isWorkflowTextBoxEditable(true);

            target.prev().removeClass("workflowSetup_updateWorkflowNameTextBox")
            target.prev().addClass("workflowSetup_updateWorkflowTextBoxSelected");
        }
        else {
            self.isWorkflowTextBoxEditable(false);
            self.workflowName(target.prev().text());
            target.prev().addClass("workflowSetup_updateWorkflowNameTextBox")
            target.prev().removeClass("workflowSetup_updateWorkflowTextBoxSelected");
        }
        event.stopPropagation();
    }
    self.onClickWorkflowTile = function (obj, event) {
        var target = $(event.target).find(".workflowSetup_editIcon");

        if (self.isWorkflowTextBoxEditable()) {
            self.isWorkflowTextBoxEditable(false);

            target.prev().addClass("workflowSetup_updateWorkflowNameTextBox")
            target.prev().removeClass("workflowSetup_updateWorkflowTextBoxSelected");
        }
    }
    self.onClickUpdateWorkflow = function (obj, event) {
        //Added By Dhruv
        workflowSetup._recentlyCreatedWorkflowInstanceID = -1;

        if (self.isWorkflowTextBoxEditable() === false) {
            onServiceUpdating();

            if (obj.workflowType().toLowerCase() === "asset level") {
                workflowSetup._maxSequence = 8;
                obj.workflowSetupInstance().maxSequenceForUsers(workflowSetup._maxSequence);
                obj.workflowSetupInstance().maxSequenceForGroups(workflowSetup._maxSequence);
            }
            else {
                workflowSetup._maxSequence = 5;
                obj.workflowSetupInstance().maxSequenceForUsers(workflowSetup._maxSequence);
                obj.workflowSetupInstance().maxSequenceForGroups(workflowSetup._maxSequence);
            }

            workflowSetup.controls.workflowSetup_chooseWorkflowScreen().css('display', 'none');
            workflowSetup.controls.workflowSetup_attributeContainerScreen().css('display', '');
            workflowSetup._workflowViewModel.workflowSetupInstance(obj.workflowSetupInstance());
            workflowSetup._workflowViewModel.workflowName(obj.workflowName());
            workflowSetup._workflowViewModel.workflowInstanceID(obj.workflowInstanceID());
            workflowSetup._workflowViewModel.moduleType(obj.moduleType());
            workflowSetup._workflowViewModel.workflowType(obj.workflowType());
            workflowSetup.updateWorkflowInit(obj.workflowSetupInstance());

            //For Keeping Track of Users and Groups in case of update
            workflowSetup._workflowViewModel.workflowSetupInstance().previouslySelectedUsers(obj.workflowSetupInstance().users());
            workflowSetup._workflowViewModel.workflowSetupInstance().previouslySelectedGroups(obj.workflowSetupInstance().groups());

            //Convert CheckBox to Toggle
            workflowSetup.convertCheckBoxToToggle("workflowSetup_applyWorkflowOnCreate");
            workflowSetup.convertCheckBoxToToggle("workflowSetup_applyTimeSeries");
            workflowSetup.convertCheckBoxToToggle("workflowSetup_applyBlankValues");
        }
        else if (self.isWorkflowTextBoxEditable() === true) {
            return false;
        }
    }
    self.onClickCreateWorkflow = function (obj, event) {
        try {
            var workflowType = smselect.getSelectedOption($("#smselect_workflowSetup_workflowTypesDropdown"));
            var newWorkflowName = workflowSetup.controls.workflowSetup_newWorkflowName().val().trim();
            if (newWorkflowName !== "" && workflowType.length > 0) {
                var moduleType = parseInt(workflowSetup.controls.workflowSetup_moduleSelector().attr("module_type"));
                var isWorkflowNameAlreadyPresent = false;
                workflowSetup._workflowViewModel.workflowList().filter(function (item) {
                    if (item.moduleType() === moduleType && item.workflowName() === newWorkflowName) {
                        isWorkflowNameAlreadyPresent = true;
                        return;
                    }
                })

                if (isWorkflowNameAlreadyPresent) {
                    throw ("Workflow Name already Present");
                }

                workflowSetup.controls.workflowSetup_chooseWorkflowScreen().css('display', 'none');
                workflowSetup.controls.workflowSetup_attributeContainerScreen().css('display', '');
                workflowSetup._workflowViewModel.moduleType(moduleType);

                if (workflowType.length > 0) {
                    var choosenWorkflowType = workflowType[0].text;
                    if (choosenWorkflowType.toLowerCase().indexOf("entity") > -1 || choosenWorkflowType.toLowerCase() === "security level") {
                        choosenWorkflowType = "ASSET LEVEL";
                    }
                    workflowSetup._workflowViewModel.workflowType(choosenWorkflowType);

                    self.workflowSetupInstance(new workflowSetup.workflowSetupViewModel(undefined, choosenWorkflowType));
                    workflowSetup.createWorkflowInit(workflowSetup._workflowViewModel.workflowType());
                    if (choosenWorkflowType.toLowerCase() === "asset level") {
                        workflowSetup._maxSequence = 8;
                        self.workflowSetupInstance().maxSequenceForUsers(workflowSetup._maxSequence);
                        self.workflowSetupInstance().maxSequenceForGroups(workflowSetup._maxSequence);
                    }
                    else {
                        workflowSetup._maxSequence = 5;
                        self.workflowSetupInstance().maxSequenceForUsers(workflowSetup._maxSequence);
                        self.workflowSetupInstance().maxSequenceForGroups(workflowSetup._maxSequence);
                    }

                    //Convert CheckBox to Toggle
                    workflowSetup.convertCheckBoxToToggle("workflowSetup_applyWorkflowOnCreate");
                    workflowSetup.convertCheckBoxToToggle("workflowSetup_applyTimeSeries");
                    workflowSetup.convertCheckBoxToToggle("workflowSetup_applyBlankValues");
                }
            }
            else {
                if (workflowType.length === 0) {
                    workflowSetup.controls.workflowSetup_errorMsgDiv().text("Please select workflow type.");
                    setTimeout(function () {
                        workflowSetup.controls.workflowSetup_errorMsgDiv().text("");
                    }, 2000);
                }
                else {
                    workflowSetup.controls.workflowSetup_errorMsgDiv().text("Please enter template name.");
                    setTimeout(function () {
                        workflowSetup.controls.workflowSetup_errorMsgDiv().text("");
                    }, 2000);
                }
            }
        }
        catch (ex) {
            workflowSetup.controls.workflowSetup_errorMsgDiv().text(ex);
            setTimeout(function () {
                workflowSetup.controls.workflowSetup_errorMsgDiv().text("");
            }, 2000);
        }
    }
    self.onClickAddWorkflow = function (obj, event) {
        var target = $(event.target);
        if (target[0].tagName === 'I') {
            target = target.parent();
        }

        target.next().text("");
        target.next().width(0);
        target.next().animate({ "width": "200px" }, function () {
            target.next().removeClass("workflowSetup_updateWorkflowTextBoxSelected")
            target.next().addClass("workflowSetup_updateWorkflowTextBoxSelected");
        });

        workflowSetup.controls.workflowSetup_addBtn().css('display', 'none');
        workflowSetup.controls.workflowSetup_createBtn().css('display', '');
    }
    self.onClickAddNewWorkflow = function (obj, event) {
        //Added by Dhruv
        workflowSetup._recentlyCreatedWorkflowInstanceID = -1;

        workflowSetup.controls.workflowSetup_addNewWorkflow().css('display', '');
        workflowSetup.controls.workflowSetup_addNewWorkflow().addClass("workflowSetup_addNewWorkflow");
        workflowSetup.controls.workflowSetup_addNewWorkflow().find(".col-sm-offset-4").removeClass("col-sm-offset-4").removeClass("col-sm-4");
        workflowSetup.controls.workflowSetup_addNewWorkflow().find(".col-sm-offset-2").removeClass("col-sm-offset-2").removeClass("col-sm-8");
        workflowSetup.controls.workflowSetup_addNewWorkflow().find(".workflowSetup_addWorkflowBtnStyle").css("margin-right", "20px");
        event.stopPropagation();
    }
    self.onClickChooseModule = function (obj, event) {
        workflowSetup.controls.workflowSetup_moduleSelector().children('div').removeClass("workflowSetup_moduleSelected");
        var target = $(event.target);
        target.addClass("workflowSetup_moduleSelected");
        if (target.text().toLowerCase() === "refmaster") {
            target.parent().attr("module_type", "1");
            var data = workflowSetup._rmWorkflowTypeMapping; //[{ "value": "1", "text": "Entity Level" }, { "value": "2", "text": "Attribute Level" }, { "value": "3", "text": "Leg Level"}];
            workflowSetup.createSMSelectDropDown(workflowSetup.controls.workflowSetup_workflowTypesDropdown(), data, false, "150", null, "Workflow Types", workflowSetup.onChangeWorkflowTypeDropdown, [], false, "Workflow Types", "Workflow Types");
        }
        else {
            target.parent().attr("module_type", "0");
            //var data = [{ "value": "1", "text": "Security Level" }, { "value": "2", "text": "Attribute Level"}];
            var data = [{ "value": "2", "text": "Attribute Level"}];
            workflowSetup.createSMSelectDropDown(workflowSetup.controls.workflowSetup_workflowTypesDropdown(), data, false, "150", null, "Workflow Types", workflowSetup.onChangeWorkflowTypeDropdown, [], false, "Workflow Types", "Workflow Types");
        }
    }

    if (data !== undefined)
        self.workflowSetupInstance = ko.observable(new workflowSetup.workflowSetupViewModel(data.workflowData, self.workflowType()));
    else
        self.workflowSetupInstance = ko.observable();

    self.workflowList = ko.observableArray([]);
    self.errorPopup = ko.observable();

    self.onHoverInTile = function (obj, event) {
        var target = $(event.target);
        if (target.hasClass("workflowSetup_updateWorkflowNameTextBox")) {
            target = target.closest(".workflowSetup_updateWorkflowSetupStyle");
        }
        if (target.hasClass("col-sm-10")) {
            target = target.closest(".workflowSetup_updateWorkflowSetupStyle");
        }
        if (target.hasClass("workflowSetup_sectypeNames")) {
            target = target.closest(".workflowSetup_updateWorkflowSetupStyle");
        }
        if (target.hasClass("workflowSetup_tileSubtitle")) {
            target = target.closest(".workflowSetup_updateWorkflowSetupStyle");
        }

        target.find(".workflowSetup_editIcon").css('color', '#898989');
        target.find(".workflowSetup_tileArrow").css('display', '');
    };
    self.onHoverOutTile = function (obj, event) {
        var target = $(event.target);
        if (target.hasClass("workflowSetup_updateWorkflowNameTextBox")) {
            target = target.closest(".workflowSetup_updateWorkflowSetupStyle");
        }
        if (target.hasClass("col-sm-10")) {
            target = target.closest(".workflowSetup_updateWorkflowSetupStyle");
        }
        if (target.hasClass("workflowSetup_sectypeNames")) {
            target = target.closest(".workflowSetup_updateWorkflowSetupStyle");
        }
        if (target.hasClass("workflowSetup_tileSubtitle")) {
            target = target.closest(".workflowSetup_updateWorkflowSetupStyle");
        }

        target.find(".workflowSetup_editIcon").css('color', '#fff');
        target.find(".workflowSetup_tileArrow").css('display', 'none');
    };

};
workflowSetup.onChangeWorkflowTypeDropdown = function () {
    var workflowType = smselect.getSelectedOption($("#smselect_workflowSetup_workflowTypesDropdown"));
    var choosenWorkflowType = workflowType[0].text;
    if (choosenWorkflowType.toLowerCase().indexOf("entity") > -1 || choosenWorkflowType.toLowerCase() === "security level") {
        workflowSetup._maxSequence = 8;
    }
};
workflowSetup.workflowSetupViewModel = function (data, workflowType) {
    var self = this;

    self.maxSequenceForGroups = ko.observable(workflowSetup._maxSequence);
    self.maxSequenceForUsers = ko.observable(workflowSetup._maxSequence);

    self.mType = ko.computed(function () {
        return workflowSetup._workflowViewModel.moduleType();
    });

    self.wType = ko.observable();
    if (typeof (workflowType) !== "undefined") {
        self.wType(workflowType);
    }

    if (data !== undefined)
        self.workflowMode = ko.observable("Update");
    else
        self.workflowMode = ko.observable("Save");

    if (data !== undefined)
        self.sectypeID = ko.observable(data.sectypeIDs);
    else
        self.sectypeID = ko.observable();

    if (data !== undefined)
        self.entityTypeID = ko.observable(data.entityTypeID);
    else
        self.entityTypeID = ko.observable();

    self.sectypesName = ko.computed(function () {
        if (self.sectypeID() !== undefined && self.sectypeID() !== null) {
            var sectypeID = self.sectypeID().split(',');
            var sectypeString = "", extraSectypeString = ""; ;
            for (var i = 0; i < sectypeID.length; i++) {
                if (i < 3) {
                    sectypeString += workflowSetup._sectypeIDVsName[sectypeID[i]] + ",";
                }
                else {
                    extraSectypeString += workflowSetup._sectypeIDVsName[sectypeID[i]] + ",";
                }
            }
            var moreString = "";
            if (extraSectypeString.length > 1) {
                moreString = extraSectypeString.split(',').length + " more";
                moreString = " + " + moreString;
            }
            var finalString = sectypeString.substring(0, sectypeString.length - 1) + moreString;
            return finalString;
        }
        else {
            return "";
        }
    });

    self.entityTypeName = ko.computed(function () {
        if (self.entityTypeID() !== undefined && self.entityTypeID() !== null) {
            var entityTypeID = self.entityTypeID().split(',');
            var finalString = workflowSetup._entityTypeIDVsName[entityTypeID[0]];
            return finalString;
        }
        else {
            return "";
        }
    });

    //For Identifying Changes in Sectype IDs in case of Update
    self.oldSectypeIDs = ko.observable();

    self.oldEntityTypeID = ko.observable();

    self.attributeInfo = ko.observableArray([]);
    if (data !== undefined) {
        var arrMappedData = ko.utils.arrayMap(data.attributeInfo, function (item) {
            return new workflowSetup.singleAttributeViewModel(item);
        });
        self.attributeInfo().push.apply(self.attributeInfo(), arrMappedData);
    }

    self.attrSearchQuery = ko.observable('');
    self.attrSearchResults = ko.computed(function () {
        var q = self.attrSearchQuery();
        return self.attributeInfo().filter(function (i) {
            return (i.attributeName().toLowerCase().indexOf(q.toLowerCase()) >= 0 && i.isAttributeSelected() === false);
        });
    });
    self.onKeyPressAttributeSearchBox = function (obj, event) {
        var target = $(event.target);
        var text = target.val();
        self.attrSearchQuery(text);
    };
    self.selectedAttributes = ko.observableArray([]);
    self.selectedAttrSearchResults = ko.computed(function () {
        var q = self.attrSearchQuery();
        return self.selectedAttributes().filter(function (i) {
            return (i.attributeName().toLowerCase().indexOf(q.toLowerCase()) >= 0);
        });
    });


    //For Identifying Changes in Attributes in case of Update
    self.newlySelectedAttributes = ko.observableArray([]);

    self.users = ko.observableArray([]);
    //For Keeping track of users in case of update
    self.previouslySelectedUsers = ko.observableArray([]);

    self.groups = ko.observableArray([]);
    //For Keeping track of groups in case of update
    self.previouslySelectedGroups = ko.observableArray([]);

    self.userSearchQuery = ko.observable('');
    self.userSearchResults = ko.computed(function () {
        var q = self.userSearchQuery();
        return self.users().filter(function (i) {
            return i.userName().toLowerCase().startsWith(q.toLowerCase());
        });
    });
    self.userSelectedResults = ko.observableArray([]);
    if (data !== undefined) {
        if (self.wType().toLowerCase() === "asset level") {
            workflowSetup._maxSequence = 8;
        }
        else {
            workflowSetup._maxSequence = 5;
        }

        //To Get Max Level in a Workflow
        var maxSequence = Math.max.apply(Math, data.usersInfo.map(function (item) {
            return item.level;
        }));
        if (maxSequence === -Infinity || maxSequence < workflowSetup._maxSequence) {
            maxSequence = workflowSetup._maxSequence;
        }
        self.maxSequenceForUsers(maxSequence);

        //Users Data
        var arrMappedDataUsers = ko.utils.arrayMap(data.usersInfo, function (item) {
            return new workflowSetup.userViewModel(item, self.maxSequenceForUsers());
        });
        self.userSelectedResults.push.apply(self.userSelectedResults, arrMappedDataUsers);
    }
    self.userSelectedSortedResults = ko.computed(function () {
        return self.userSelectedResults().sort(function (l, r) {
            var leftSelected = null, rightSelected = null;
            for (var level in l.levelList()) {
                if (l.levelList()[level].isSelected() === true) {
                    leftSelected = l.levelList()[level];
                    break;
                }
            }
            for (var level in r.levelList()) {
                if (r.levelList()[level].isSelected() === true) {
                    rightSelected = r.levelList()[level];
                    break;
                }
            }

            if (leftSelected === null || rightSelected === null) {
                return 1;
            }
            else {
                return parseInt(leftSelected.levelNo()) > parseInt(rightSelected.levelNo()) ? 1 : -1;
            }
        });
    });

    self.groupSearchQuery = ko.observable('');
    self.onKeyPressGroupSearchBox = function (obj, event) {
        var target = $(event.target);
        var text = target.val();
        self.groupSearchQuery(text);
    };
    self.groupSearchResults = ko.computed(function () {
        var q = self.groupSearchQuery();
        return self.groups().filter(function (i) {
            return i.groupName().toLowerCase().startsWith(q.toLowerCase());
        });
    });
    self.groupSelectedResults = ko.observableArray([]);
    if (data !== undefined) {
        //To Get Max Level in a Workflow
        var maxSequence = Math.max.apply(Math, data.groupInfo.map(function (item) {
            return item.level;
        }));
        if (maxSequence === -Infinity || maxSequence < workflowSetup._maxSequence) {
            maxSequence = workflowSetup._maxSequence;
        }
        self.maxSequenceForGroups(maxSequence)

        //Groups Data
        var arrMappedDataGroups = ko.utils.arrayMap(data.groupInfo, function (item) {
            return new workflowSetup.groupViewModel(item, self.maxSequenceForGroups());
        });
        self.groupSelectedResults.push.apply(self.groupSelectedResults, arrMappedDataGroups);
    }
    self.groupSelectedSortedResults = ko.computed(function () {
        return self.groupSelectedResults().sort(function (l, r) {
            var leftSelected = null, rightSelected = null;
            for (var level in l.levelList()) {
                if (l.levelList()[level].isSelected() === true) {
                    leftSelected = l.levelList()[level];
                    break;
                }
            }
            for (var level in r.levelList()) {
                if (r.levelList()[level].isSelected() === true) {
                    rightSelected = r.levelList()[level];
                    break;
                }
            }

            if (leftSelected === null || rightSelected === null) {
                return 1;
            }
            else {
                return parseInt(leftSelected.levelNo()) > parseInt(rightSelected.levelNo()) ? 1 : -1;
            }
        });
    });

    self.onKeyPressUserSearchBox = function (obj, event) {
        var target = $(event.target);
        var text = target.val();
        self.userSearchQuery(text);
    };
    self.onClickUserGroupToggle = function (obj, event) {
        var section = "";
        if ($(event.target).hasClass("workflowSetup_toggleSection")) {
            section = $(event.target).text().toLowerCase().trim();
            $(event.target).siblings('div').removeClass("workflowSetup_toggleSelected");
            $(event.target).addClass("workflowSetup_toggleSelected");

            if (section === "user") {
                $(event.target).closest(".workflowSetup_attributeConfigSectionStyle").find(".workflowSetup_userSection").css('display', '');
                $(event.target).closest(".workflowSetup_attributeConfigSectionStyle").find(".workflowSetup_groupSection").css('display', 'none');
            }
            else {
                $(event.target).closest(".workflowSetup_attributeConfigSectionStyle").find(".workflowSetup_userSection").css('display', 'none');
                $(event.target).closest(".workflowSetup_attributeConfigSectionStyle").find(".workflowSetup_groupSection").css('display', '');
            }
        }
    };
    self.onClickSaveWorkflow = function (obj, event) {
        var mData = massageSaveData();
        if (mData !== null) {
            var workflowName = mData.wName;
            var isCreate = mData.isCreate;
            var applyTimeSeries = mData.applyTimeSeries;
            var username = mData.username;
            var info = mData.info;
            var applyBlank = mData.applyBlankValues;

            if (self.mType() === 0) {
                workflowSetup.saveWorkflowConfig(info, workflowName, isCreate, applyTimeSeries, username, applyBlank).then(function (data) {
                    onServiceUpdating();
                    var status = data.split("||")[0];
                    if (status == "0") {
                        var d = {
                            title: "Error",
                            message: data.split("||")[1],
                            status: "error"
                        }
                    }
                    else if (status == "1") {
                        var d = {
                            title: "Success",
                            message: "Workflow Successfully Created",
                            status: "success"
                        }

                        //Added By Dhruv
                        workflowSetup._recentlyCreatedWorkflowInstanceID = parseInt(data.split("@^@")[1]) || -1;
                    }
                    workflowSetup._workflowViewModel.errorPopup(new workflowSetup.errorViewModel(d));
                    workflowSetup.controls.workflowSetup_messagePopup().css('display', '');

                    self.workflowMode("Update");
                    onServiceUpdated();
                });
            }
            else if (self.mType() === 1) {
                workflowSetup.saveWorkflowConfigRM(info, workflowName, isCreate, applyTimeSeries, username, applyBlank).then(function (data) {
                    onServiceUpdating();
                    var status = data.split("||")[0];
                    if (status == "0") {
                        var d = {
                            title: "Error",
                            message: data.split("||")[1],
                            status: "error"
                        }
                    }
                    else if (status == "1") {
                        var d = {
                            title: "Success",
                            message: "Workflow Successfully Created",
                            status: "success"
                        }

                        //Added By Dhruv
                        workflowSetup._recentlyCreatedWorkflowInstanceID = parseInt(data.split("@^@")[1]) || -1;
                    }
                    workflowSetup._workflowViewModel.errorPopup(new workflowSetup.errorViewModel(d));
                    workflowSetup.controls.workflowSetup_messagePopup().css('display', '');

                    self.workflowMode("Update");
                    onServiceUpdated();
                });
            }
        }
    };
    self.onClickUpdateWorkflowSetup = function (obj, event) {
        var mData = massageSaveData();
        if (mData !== null) {
            var workflowName = mData.wName;
            var isCreate = mData.isCreate;
            var applyTimeSeries = mData.applyTimeSeries;
            var username = mData.username;
            var info = mData.info;
            var applyBlank = mData.applyBlankValues;
            var instanceID = mData.instanceID;
            if (self.mType() === 0) {
                workflowSetup.updateWorkflowConfig(info, workflowName, isCreate, applyTimeSeries, username, applyBlank, instanceID).then(function (data) {
                    onServiceUpdating();
                    var status = data.split("||")[0];
                    if (status == "0") {
                        var d = {
                            title: "Error",
                            message: data.split("||")[1],
                            status: "error"
                        }
                    }
                    else if (status == "1") {
                        var d = {
                            title: "Success",
                            message: "Workflow Successfully Updated",
                            status: "success"
                        }
                    }
                    workflowSetup._workflowViewModel.errorPopup(new workflowSetup.errorViewModel(d));
                    workflowSetup.controls.workflowSetup_messagePopup().css('display', '');
                    onServiceUpdated();
                });
            }
            else if (self.mType() === 1) {
                workflowSetup.updateWorkflowConfigRM(info, workflowName, isCreate, applyTimeSeries, username, applyBlank, instanceID).then(function (data) {
                    onServiceUpdating();
                    var status = data.split("||")[0];
                    if (status == "0") {
                        var d = {
                            title: "Error",
                            message: data.split("||")[1],
                            status: "error"
                        }
                    }
                    else if (status == "1") {
                        var d = {
                            title: "Success",
                            message: "Workflow Successfully Updated",
                            status: "success"
                        }
                    }
                    workflowSetup._workflowViewModel.errorPopup(new workflowSetup.errorViewModel(d));
                    workflowSetup.controls.workflowSetup_messagePopup().css('display', '');
                    onServiceUpdated();
                });
            }
        }
    };


    self.applyTimeSeries = ko.observable(true);
    if (data !== undefined) {
        self.applyTimeSeries(data.applyTimeSeries);
    }

    self.applyOnCreate = ko.observable(true);
    if (data !== undefined) {
        self.applyOnCreate(data.applyOnCreate);
    }

    self.applyOnBlankToNonblank = ko.observable(true);
    if (data !== undefined) {
        self.applyOnBlankToNonblank(data.applyOnBlankToNonblank);
    }

    self.assetTypeText = ko.computed(function () {
        var finalAssetText = "Security Type";
        var mType = workflowSetup._workflowViewModel.moduleType();
        if (mType !== undefined && mType === workflowSetup._moduleMapping["SECMASTER"]) {
            finalAssetText = "Security Type";
        }
        else if (mType !== undefined && mType === workflowSetup._moduleMapping["REFMASTER"]) {
            finalAssetText = "Entity Type";
        }
        return finalAssetText;
    });

    self.onClickClose = function (obj, event) {
        //workflowSetup.controls.workflowSetup_chooseWorkflowScreen().css('display', '');
        //workflowSetup.controls.workflowSetup_attributeContainerScreen().css('display', 'none');
        onServiceUpdating();
        location.reload();
        onServiceUpdated();
    }

    var massageSaveData = function () {
        try {
            var workflowInstance = workflowSetup._workflowViewModel.workflowSetupInstance();
            var finalWorkflowInfo = {};
            var workflowName = workflowSetup.controls.workflowSetup_workflowInstanceDropDown().text().trim();
            finalWorkflowInfo.workflowData = {};

            if (workflowInstance.wType().toLowerCase().indexOf("attribute") > -1 || workflowInstance.wType().toLowerCase().indexOf('leg') > -1) {
                //Attribute Info
                var attrInfo = [];
                if (workflowInstance.selectedAttributes().length === 0) {
                    throw "Please Select Attribute";
                }
                else {
                    for (var item in workflowInstance.selectedAttributes()) {
                        var tempObj = {};
                        tempObj.attrName = workflowInstance.selectedAttributes()[item].attributeName();
                        tempObj.attrID = workflowInstance.selectedAttributes()[item].attributeID();
                        attrInfo.push(tempObj);
                    }
                    finalWorkflowInfo.workflowData.attributeInfo = attrInfo;
                }
            }
            //SectypeIDs
            if (workflowInstance.mType() === 0) {
                finalWorkflowInfo.workflowData.sectypeIDs = workflowInstance.sectypeID();
            }
            else if (workflowInstance.mType() === 1) {
                finalWorkflowInfo.workflowData.entityTypeID = workflowInstance.entityTypeID();
            }

            var userInfo = [], minSequenceInUser = 1, maxSequenceInUser = 1, minSequenceInUserOrGroup = 1, maxSequenceInUserOrGroup = 1;
            var groupInfo = [], minSequenceInGroup = 1, maxSequenceInGroup = 1;

            if (workflowInstance.userSelectedResults().length === 0 && workflowInstance.groupSelectedResults().length === 0) {
                throw "Please select workflow level for users or groups"
            }
            else {
                //Selected Users
                for (var item in workflowInstance.userSelectedResults()) {
                    var tempObj = {};
                    tempObj.name = workflowInstance.userSelectedResults()[item].userLoginName();
                    tempObj.level = workflowInstance.userSelectedResults()[item].workflowLevel();
                    userInfo.push(tempObj);

                    if (tempObj.level > maxSequenceInUser) {
                        maxSequenceInUser = parseInt(tempObj.level);
                    }
                    if (tempObj.level > maxSequenceInUserOrGroup) {
                        maxSequenceInUserOrGroup = parseInt(tempObj.level);
                    }
                }

                //Selected Groups
                for (var item in workflowInstance.groupSelectedResults()) {
                    var tempObj = {};
                    tempObj.name = workflowInstance.groupSelectedResults()[item].groupName();
                    tempObj.level = workflowInstance.groupSelectedResults()[item].workflowLevel();
                    groupInfo.push(tempObj);

                    if (tempObj.level > maxSequenceInGroup) {
                        maxSequenceInGroup = parseInt(tempObj.level);
                    }
                    if (tempObj.level > maxSequenceInUserOrGroup) {
                        maxSequenceInUserOrGroup = parseInt(tempObj.level);
                    }
                }

                //If Selections are made from both users and groups
                var userGroupWorkflowIsInSequence = true;
                for (var i = minSequenceInUserOrGroup; i <= maxSequenceInUserOrGroup; i++) {
                    if (!groupInfo.concat(userInfo).map(function (item) {
                        return item.level.toString();
                    }).includes(i.toString())) {
                        userGroupWorkflowIsInSequence = false;
                        break;
                    }
                }

                if (userGroupWorkflowIsInSequence === false) {
                    throw "Please select proper sequence in Users and groups";
                }

                if (workflowInstance.userSelectedResults().length === 0) {
                    finalWorkflowInfo.workflowData.usersInfo = [];
                }
                else {
                    var userWorkflowIsInSequence = true;
                    for (var i = minSequenceInUser; i <= maxSequenceInUser; i++) {
                        if (!userInfo.map(function (item) {
                            return item.level.toString();
                        }).includes(i.toString())) {
                            userWorkflowIsInSequence = false;
                            break;
                        }
                    }

                    if (userWorkflowIsInSequence === true || (userGroupWorkflowIsInSequence === true && userWorkflowIsInSequence === false)) {
                        finalWorkflowInfo.workflowData.usersInfo = userInfo;
                    }
                    else {
                        throw "Please select proper sequence in Users";
                    }
                }

                if (workflowInstance.groupSelectedResults().length === 0) {
                    finalWorkflowInfo.workflowData.groupInfo = [];
                }
                else {
                    var groupWorkflowIsInSequence = true;
                    for (var i = minSequenceInGroup; i <= maxSequenceInGroup; i++) {
                        if (!groupInfo.map(function (item) {
                            return item.level.toString();
                        }).includes(i.toString())) {
                            groupWorkflowIsInSequence = false;
                            break;
                        }
                    }

                    if (groupWorkflowIsInSequence === true || (userGroupWorkflowIsInSequence === true && groupWorkflowIsInSequence === false)) {
                        finalWorkflowInfo.workflowData.groupInfo = groupInfo;
                    }
                    else {
                        throw "Please select proper sequence in Groups";
                    }
                }
            }

            applyTimeSeries = false;
            applyWorkflowOnCreate = false;
            applyBlanktoNonBlank = false;
            if (workflowInstance.wType().toLowerCase() === "asset level") {
                applyWorkflowOnCreate = true;
            }

            if (workflowInstance.wType().toLowerCase() === "attribute level") {
                var applyTimeSeries = workflowSetup.controls.workflowSetup_applyTimeSeries().is(":checked");
                var applyWorkflowOnCreate = workflowSetup.controls.workflowSetup_applyWorkflowOnCreate().is(":checked");
                var applyBlanktoNonBlank = workflowSetup.controls.workflowSetup_applyBlankValues().is(":checked");
            }
            var instanceID = workflowSetup._workflowViewModel.workflowInstanceID();

            //Added By Dhruv
            if (typeof instanceID === typeof undefined) {
                instanceID = workflowSetup._recentlyCreatedWorkflowInstanceID != -1 ? workflowSetup._recentlyCreatedWorkflowInstanceID : 0;
            }

            for (var j in workflowSetup._workflowTypeMapping) {
                if (workflowSetup._workflowTypeMapping[j].toLowerCase().indexOf( workflowInstance.wType().toLowerCase() ) > -1) {
                    finalWorkflowInfo.workflowType = parseInt(j);
                }
            }

            return {
                info: finalWorkflowInfo,
                wName: workflowName,
                isCreate: applyWorkflowOnCreate,
                applyTimeSeries: applyTimeSeries,
                applyBlankValues: applyBlanktoNonBlank,
                username: workflowSetup._username,
                instanceID: instanceID
            }
        }
        catch (ex) {
            var d = {
                title: "Error",
                message: ex.toString(),
                status: "error"
            };
            workflowSetup._workflowViewModel.errorPopup(new workflowSetup.errorViewModel(d));
            workflowSetup.controls.workflowSetup_messagePopup().css('display', '');
            return null;
        }
    }
};
workflowSetup.singleAttributeViewModel = function (data) {
    var self = this;

    self.attributeID = ko.observable(data.attrID);
    self.attributeName = ko.observable(data.attrName);
    if (data.isSelected !== undefined) {
        self.isAttributeSelected = ko.observable(data.isSelected);
    }
    else {
        self.isAttributeSelected = ko.observable(false);
    }
    self.onClickAttribute = function (obj, event) {
        var target = $(event.target);
        target.animate({ "margin-top": "-10px", "opacity": "0" }, 450, function () {
            if (workflowSetup._workflowViewModel.workflowSetupInstance().selectedAttributes().length === 2 || workflowSetup._workflowViewModel.workflowSetupInstance().selectedAttributes().length > 2) {
                //var attrSectionHeight = workflowSetup.controls.workflowSetup_attributesSection().height() - 33;
                //workflowSetup.controls.workflowSetup_attributesSection().closest('.smslimScrollDiv').height(attrSectionHeight);
                //workflowSetup.controls.workflowSetup_attributesSection().height(attrSectionHeight);
                workflowSetup.controls.workflowSetup_selectedAttributesSection().smslimscroll({ height: '115px' });
            }
            /*else {
            workflowSetup.controls.workflowSetup_selectedAttributesSection().smslimscroll({ height: '145px' });
            }*/
            obj.isAttributeSelected(true);
            workflowSetup._workflowViewModel.workflowSetupInstance().attributeInfo.remove(obj); // Add to Selected Attributes List
            workflowSetup._workflowViewModel.workflowSetupInstance().selectedAttributes.push(obj); // Add to Selected Attributes List
            workflowSetup._workflowViewModel.workflowSetupInstance().newlySelectedAttributes.push(obj); //For Tracking Changes in Case of Update

            workflowSetup.controls.workflowSetup_attributesSection().parent().height(340);
            workflowSetup.controls.workflowSetup_attributesSection().height(340);
        });
    };
    self.onClickRemoveSelectedAttribute = function (obj, event) {
        var target = $(event.target).parent().prev(); // closest(".workflowSetup_attributeNameSeclectedStyle");
        target.animate({ "margin-top": "10px", "opacity": "0" }, 450, function () {
            workflowSetup._workflowViewModel.workflowSetupInstance().attributeInfo.push(obj); // Add to Attributes Info List
            obj.isAttributeSelected(false);
            workflowSetup._workflowViewModel.workflowSetupInstance().selectedAttributes.remove(obj); //Remove from Selected Attributes List
            workflowSetup._workflowViewModel.workflowSetupInstance().newlySelectedAttributes.remove(obj); //For Tracking Changes in Case of Update

            if (workflowSetup._workflowViewModel.workflowSetupInstance().selectedAttributes().length > 1 && workflowSetup._workflowViewModel.workflowSetupInstance().selectedAttributes().length == 2) {
                workflowSetup.controls.workflowSetup_selectedAttributesSection().smslimscroll({ destroy: true });
                workflowSetup.controls.workflowSetup_selectedAttributesSection().css("height", "auto");
            }

            if (workflowSetup._workflowViewModel.workflowSetupInstance().selectedAttributes().length === 0) {
                var attrSectionHeight = workflowSetup.controls.workflowSetup_attributesSection().height() + 150;
                workflowSetup.controls.workflowSetup_attributesSection().closest('.smslimScrollDiv').height(attrSectionHeight);
                workflowSetup.controls.workflowSetup_attributesSection().height(attrSectionHeight);
                //var selectedAttrSectionHeight = workflowSetup.controls.workflowSetup_selectedAttributesSection().height() - 33;
                //workflowSetup.controls.workflowSetup_selectedAttributesSection().height(selectedAttrSectionHeight)
            }
        });
    };
};
workflowSetup.userViewModel = function (data, maxSequence) {
    var self = this;

    self.userName = ko.observable(workflowSetup._userLoginNameVsFirstNameLastName[data.name]);
    self.workflowLevel = ko.observable(data.level);
    self.levelList = ko.observableArray([]);
    self.userLoginName = ko.observable(data.name);

    var usersInitArr = []; // [{ 'no': '1', 'isSelected': isLevelSelected('1') }, { 'no': '2', 'isSelected': isLevelSelected('2') }, { 'no': '3', 'isSelected': isLevelSelected('3')}];
    var mSequence = workflowSetup._workflowViewModel.workflowSetupInstance() === undefined ? maxSequence : workflowSetup._workflowViewModel.workflowSetupInstance().maxSequenceForUsers();
    for (var row = 1; row <= mSequence; row++) {
        usersInitArr.push({ 'no': row.toString(), 'isSelected': isLevelSelected(row.toString()) });
    }

    var usersData = ko.utils.arrayMap(usersInitArr, function (item) {
        return new workflowSetup.levelViewModel(item);
    });
    self.levelList.push.apply(self.levelList, usersData);

    self.onClickWorkflowLevel = function (parentObj, event, currentObj) {
        var target = $(event.target);
        var selectedSection = null;
        if (target.closest("#workflowSetup_userSearchResults").parent().hasClass("smslimScrollDiv")) {
            selectedSection = target.closest("#workflowSetup_userSearchResults").parent().prev();
        }
        else {
            selectedSection = target.closest("#workflowSetup_userSearchResults").prev();
        }
        var positionDiff = target.closest("#workflowSetup_userSearchResults").offset().top - selectedSection.offset().top;
        var speed = 300;
        if (positionDiff > 150 && positionDiff < 300) {
            speed = 500;
        }
        else if (positionDiff > 450 && positionDiff < 600) {
            speed = 800;
        }

        target.animate({ "margin-top": "-" + positionDiff + "px", "opacity": "0" }, speed, function () {
            var level = $(event.target).text();
            self.workflowLevel(level);
            $(event.target).siblings("div").removeClass("workflowSetup_highlightLevel");
            $(event.target).addClass("workflowSetup_highlightLevel");

            //Adjust User Section Height
            var userSectionHeight = workflowSetup.controls.workflowSetup_userSearchResults().height() - 43;
            workflowSetup.controls.workflowSetup_userSearchResults().closest('.smslimScrollDiv').height(userSectionHeight);
            workflowSetup.controls.workflowSetup_userSearchResults().height(userSectionHeight);

            for (var i in parentObj.levelList()) {
                if (parentObj.levelList()[i].levelNo() === level) {
                    parentObj.levelList()[i].isSelected(true);
                }
            }

            workflowSetup._workflowViewModel.workflowSetupInstance().userSelectedResults.push(parentObj);
            workflowSetup._workflowViewModel.workflowSetupInstance().users.remove(function (item) {
                return item.userName() == parentObj.userName();
            });
        });
    };

    self.onClickSelectedWorkflowLevel = function (parentObj, event, currentObj) {
        var level = $(event.target).text();
        var target = $(event.target);

        self.workflowLevel(level);

        if (target.hasClass("workflowSetup_levelOneColor")) {
            target.removeClass("workflowSetup_levelOneColor");

            for (var i in parentObj.levelList()) {
                parentObj.levelList()[i].isSelected(false);
            }
            workflowSetup._workflowViewModel.workflowSetupInstance().users.push(parentObj);
            workflowSetup._workflowViewModel.workflowSetupInstance().userSelectedResults.remove(function (item) {
                return item.userName() == parentObj.userName();
            });
        }
        else if (target.hasClass("workflowSetup_levelTwoColor")) {
            target.removeClass("workflowSetup_levelTwoColor");

            for (var i in parentObj.levelList()) {
                parentObj.levelList()[i].isSelected(false);
            }
            workflowSetup._workflowViewModel.workflowSetupInstance().users.push(parentObj);
            workflowSetup._workflowViewModel.workflowSetupInstance().userSelectedResults.remove(function (item) {
                return item.userName() == parentObj.userName();
            });
        }
        else if (target.hasClass("workflowSetup_levelThreeColor")) {
            target.removeClass("workflowSetup_levelThreeColor");

            for (var i in parentObj.levelList()) {
                parentObj.levelList()[i].isSelected(false);
            }
            workflowSetup._workflowViewModel.workflowSetupInstance().users.push(parentObj);
            workflowSetup._workflowViewModel.workflowSetupInstance().userSelectedResults.remove(function (item) {
                return item.userName() == parentObj.userName();
            });
        }
        else if (target.hasClass("workflowSetup_levelFourColor")) {
            target.removeClass("workflowSetup_levelFourColor");

            for (var i in parentObj.levelList()) {
                parentObj.levelList()[i].isSelected(false);
            }
            workflowSetup._workflowViewModel.workflowSetupInstance().users.push(parentObj);
            workflowSetup._workflowViewModel.workflowSetupInstance().userSelectedResults.remove(function (item) {
                return item.userName() == parentObj.userName();
            });
        }
        else if (target.hasClass("workflowSetup_levelFiveColor")) {
            target.removeClass("workflowSetup_levelFiveColor");

            for (var i in parentObj.levelList()) {
                parentObj.levelList()[i].isSelected(false);
            }
            workflowSetup._workflowViewModel.workflowSetupInstance().users.push(parentObj);
            workflowSetup._workflowViewModel.workflowSetupInstance().userSelectedResults.remove(function (item) {
                return item.userName() == parentObj.userName();
            });
        }
        else {
            if (target.siblings("div").hasClass("workflowSetup_levelOneColor")) {
                target.siblings("div").removeClass("workflowSetup_levelOneColor");
                target.addClass("workflowSetup_levelOneColor");
            }
            else if (target.siblings("div").hasClass("workflowSetup_levelTwoColor")) {
                target.siblings("div").removeClass("workflowSetup_levelTwoColor");
                target.addClass("workflowSetup_levelTwoColor");
            }
            else if (target.siblings("div").hasClass("workflowSetup_levelThreeColor")) {
                target.siblings("div").removeClass("workflowSetup_levelThreeColor");
                target.addClass("workflowSetup_levelThreeColor");
            }
            else if (target.siblings("div").hasClass("workflowSetup_levelFourColor")) {
                target.siblings("div").removeClass("workflowSetup_levelFourColor");
                target.addClass("workflowSetup_levelFourColor");
            }
            else if (target.siblings("div").hasClass("workflowSetup_levelFiveColor")) {
                target.siblings("div").removeClass("workflowSetup_levelFiveColor");
                target.addClass("workflowSetup_levelFiveColor");
            }

            for (var i in parentObj.levelList()) {
                if (parentObj.levelList()[i].levelNo() === level) {
                    parentObj.levelList()[i].isSelected(true);
                }
                else {
                    parentObj.levelList()[i].isSelected(false);
                }
            }
            workflowSetup._workflowViewModel.workflowSetupInstance().userSelectedResults.remove(function (item) {
                return item.userName() == parentObj.userName();
            });
            workflowSetup._workflowViewModel.workflowSetupInstance().userSelectedResults.push(parentObj);
        }

        //Adjust User Section Height
        var userSectionHeight = workflowSetup.controls.workflowSetup_userSearchResults().height() + 43;
        workflowSetup.controls.workflowSetup_userSearchResults().closest('.smslimScrollDiv').height(userSectionHeight);
        workflowSetup.controls.workflowSetup_userSearchResults().height(userSectionHeight);
    }

    self.onClickAddLevel = function (parent, event, obj) {
        var target = $(event.target);
        if (target[0].tagName === "I") {
            target = target.closest("div");
        }
        var levelNumber = self.levelList().length + 1;
        //self.levelList.push(new workflowSetup.levelViewModel({ 'no': levelNumber.toString(), 'isSelected': false }));
        for (var user in parent.users()) {
            parent.users()[user].levelList.push(new workflowSetup.levelViewModel({ 'no': levelNumber.toString(), 'isSelected': false }))
        }

        for (var user in parent.userSelectedResults()) {
            parent.userSelectedResults()[parseInt(user)].levelList.push(new workflowSetup.levelViewModel({ 'no': levelNumber.toString(), 'isSelected': false }))
        }
    };

    function isLevelSelected(levelNo) {
        if (data.level == levelNo) {
            return true;
        }
        else {
            return false;
        }
    }
};
workflowSetup.levelViewModel = function (data) {
    var self = this;

    var maxSequence = 5;

    self.levelNo = ko.observable(data.no);
    self.isSelected = ko.observable(data.isSelected);
    self.isLevelOneSelected = function (level) {
        if (parseInt(level()) % maxSequence === 1 && self.isSelected()) {
            return true;
        }
        else {
            return false;
        }
    }
    self.isLevelTwoSelected = function (level) {
        if (parseInt(level()) % maxSequence === 2 && self.isSelected()) {
            return true;
        }
        else {
            return false;
        }
    }
    self.isLevelThreeSelected = function (level) {
        if (parseInt(level()) % maxSequence === 3 && self.isSelected()) {
            return true;
        }
        else {
            return false;
        }
    }
    self.isLevelFourSelected = function (level) {
        if (parseInt(level()) % maxSequence === 4 && self.isSelected()) {
            return true;
        }
        else {
            return false;
        }
    }
    self.isLevelFiveSelected = function (level) {
        if (parseInt(level()) % maxSequence === 0 && self.isSelected()) {
            return true;
        }
        else {
            return false;
        }
    }
};
workflowSetup.groupViewModel = function (data, maxSequence) {
    var self = this;

    self.groupName = ko.observable(data.name);
    self.workflowLevel = ko.observable(data.level);
    self.levelList = ko.observableArray();

    var groupsInitArr = []; // [{ 'no': '1', 'isSelected': isLevelSelected('1') }, { 'no': '2', 'isSelected': isLevelSelected('2') }, { 'no': '3', 'isSelected': isLevelSelected('3')}];
    var mSequence = workflowSetup._workflowViewModel.workflowSetupInstance() === undefined ? maxSequence : workflowSetup._workflowViewModel.workflowSetupInstance().maxSequenceForGroups();
    for (var row = 1; row <= mSequence; row++) {
        groupsInitArr.push({ 'no': row.toString(), 'isSelected': isLevelSelected(row.toString()) });
    }

    var groupsData = ko.utils.arrayMap(groupsInitArr, function (item) {
        return new workflowSetup.levelViewModel(item);
    });
    self.levelList.push.apply(self.levelList, groupsData);

    self.onClickWorkflowLevel = function (parentObj, event, currentObj) {
        var target = $(event.target);
        var selectedSection = null;
        if (target.closest("#workflowSetup_groupSearchResults").parent().hasClass("smslimScrollDiv")) {
            selectedSection = target.closest("#workflowSetup_groupSearchResults").parent().prev();
        }
        else {
            selectedSection = target.closest("#workflowSetup_groupSearchResults").prev();
        }
        var positionDiff = target.closest("#workflowSetup_groupSearchResults").offset().top - selectedSection.offset().top;
        var speed = 300;
        if (positionDiff > 150 && positionDiff < 300) {
            speed = 500;
        }
        else if (positionDiff > 450 && positionDiff < 600) {
            speed = 800;
        }

        target.animate({ "margin-top": "-" + positionDiff + "px", "opacity": "0" }, speed, function () {
            var level = $(event.target).text();
            self.workflowLevel(level);
            $(event.target).siblings("div").removeClass("workflowSetup_highlightLevel");
            $(event.target).addClass("workflowSetup_highlightLevel");

            //Adjust Group Section Height
            var groupSectionHeight = workflowSetup.controls.workflowSetup_groupSearchResults().height() - 43;
            workflowSetup.controls.workflowSetup_groupSearchResults().closest('.smslimScrollDiv').height(groupSectionHeight);
            workflowSetup.controls.workflowSetup_groupSearchResults().height(groupSectionHeight);

            for (var i in parentObj.levelList()) {
                if (parentObj.levelList()[i].levelNo() === level) {
                    parentObj.levelList()[i].isSelected(true);
                }
            }

            workflowSetup._workflowViewModel.workflowSetupInstance().groupSelectedResults.push(parentObj);
            workflowSetup._workflowViewModel.workflowSetupInstance().groups.remove(function (item) {
                return item.groupName() == parentObj.groupName();
            });
        });
    };

    self.onClickSelectedWorkflowLevel = function (parentObj, event, currentObj) {
        var level = $(event.target).text();
        var target = $(event.target);

        self.workflowLevel(level);

        if (target.hasClass("workflowSetup_levelOneColor")) {
            target.removeClass("workflowSetup_levelOneColor");

            for (var i in parentObj.levelList()) {
                parentObj.levelList()[i].isSelected(false);
            }

            workflowSetup._workflowViewModel.workflowSetupInstance().groups.push(parentObj);
            workflowSetup._workflowViewModel.workflowSetupInstance().groupSelectedResults.remove(function (item) {
                return item.groupName() == parentObj.groupName();
            });
        }
        else if (target.hasClass("workflowSetup_levelTwoColor")) {
            target.removeClass("workflowSetup_levelTwoColor");

            for (var i in parentObj.levelList()) {
                parentObj.levelList()[i].isSelected(false);
            }

            workflowSetup._workflowViewModel.workflowSetupInstance().groups.push(parentObj);
            workflowSetup._workflowViewModel.workflowSetupInstance().groupSelectedResults.remove(function (item) {
                return item.groupName() == parentObj.groupName();
            });
        }
        else if (target.hasClass("workflowSetup_levelThreeColor")) {
            target.removeClass("workflowSetup_levelThreeColor");

            for (var i in parentObj.levelList()) {
                parentObj.levelList()[i].isSelected(false);
            }

            workflowSetup._workflowViewModel.workflowSetupInstance().groups.push(parentObj);
            workflowSetup._workflowViewModel.workflowSetupInstance().groupSelectedResults.remove(function (item) {
                return item.groupName() == parentObj.groupName();
            });
        }
        else if (target.hasClass("workflowSetup_levelFourColor")) {
            target.removeClass("workflowSetup_levelFourColor");

            for (var i in parentObj.levelList()) {
                parentObj.levelList()[i].isSelected(false);
            }

            workflowSetup._workflowViewModel.workflowSetupInstance().groups.push(parentObj);
            workflowSetup._workflowViewModel.workflowSetupInstance().groupSelectedResults.remove(function (item) {
                return item.groupName() == parentObj.groupName();
            });
        }
        else if (target.hasClass("workflowSetup_levelFiveColor")) {
            target.removeClass("workflowSetup_levelFiveColor");

            for (var i in parentObj.levelList()) {
                parentObj.levelList()[i].isSelected(false);
            }

            workflowSetup._workflowViewModel.workflowSetupInstance().groups.push(parentObj);
            workflowSetup._workflowViewModel.workflowSetupInstance().groupSelectedResults.remove(function (item) {
                return item.groupName() == parentObj.groupName();
            });
        }
        else {
            if (target.siblings("div").hasClass("workflowSetup_levelOneColor")) {
                target.siblings("div").removeClass("workflowSetup_levelOneColor");
                target.addClass("workflowSetup_levelOneColor");
            }
            else if (target.siblings("div").hasClass("workflowSetup_levelTwoColor")) {
                target.siblings("div").removeClass("workflowSetup_levelTwoColor");
                target.addClass("workflowSetup_levelTwoColor");
            }
            else if (target.siblings("div").hasClass("workflowSetup_levelThreeColor")) {
                target.siblings("div").removeClass("workflowSetup_levelThreeColor");
                target.addClass("workflowSetup_levelThreeColor");
            }
            else if (target.siblings("div").hasClass("workflowSetup_levelFourColor")) {
                target.siblings("div").removeClass("workflowSetup_levelFourColor");
                target.addClass("workflowSetup_levelFourColor");
            }
            else if (target.siblings("div").hasClass("workflowSetup_levelFiveColor")) {
                target.siblings("div").removeClass("workflowSetup_levelFiveColor");
                target.addClass("workflowSetup_levelFiveColor");
            }

            for (var i in parentObj.levelList()) {
                if (parentObj.levelList()[i].levelNo() === level) {
                    parentObj.levelList()[i].isSelected(true);
                }
                else {
                    parentObj.levelList()[i].isSelected(false);
                }
            }
            workflowSetup._workflowViewModel.workflowSetupInstance().groupSelectedResults.remove(function (item) {
                return item.groupName() == parentObj.groupName();
            });
            workflowSetup._workflowViewModel.workflowSetupInstance().groupSelectedResults.push(parentObj);
        }

        //Adjust Group Section Height
        var groupSectionHeight = workflowSetup.controls.workflowSetup_groupSearchResults().height() + 43;
        workflowSetup.controls.workflowSetup_groupSearchResults().closest('.smslimScrollDiv').height(groupSectionHeight);
        workflowSetup.controls.workflowSetup_groupSearchResults().height(groupSectionHeight);
    }

    self.onClickAddLevel = function (parent, event, obj) {
        var target = $(event.target);
        if (target[0].tagName === "I") {
            target = target.closest("div");
        }

        var levelNumber = self.levelList().length + 1;
        //self.levelList.push(new workflowSetup.levelViewModel({ 'no': levelNumber.toString(), 'isSelected': false }));
        for (var group in parent.groups()) {
            parent.groups()[group].levelList.push(new workflowSetup.levelViewModel({ 'no': levelNumber.toString(), 'isSelected': false }))
        }

        for (var group in parent.groupSelectedResults()) {
            parent.groupSelectedResults()[parseInt(group)].levelList.push(new workflowSetup.levelViewModel({ 'no': levelNumber.toString(), 'isSelected': false }))
        }
    };

    function isLevelSelected(levelNo) {
        if (data.level == levelNo) {
            return true;
        }
        else {
            return false;
        }
    }
};
workflowSetup.errorViewModel = function (data) {
    var self = this;

    self.title = ko.observable(data.title);
    self.message = ko.observable(data.message);
    self.icon = function () {
        if (data.status === "success") {
            return "images/icons/pass_icon.png";
        }
        else if (data.status === "error") {
            return "images/icons/fail_icon.png";
        }
        else if (data.status === "alert") {
            return "images/icons/alert_icon.png";
        }
    }
    self.borderColor = function () {
        if (data.status === "success") {
            return "#ACD373";
        }
        else if (data.status === "error") {
            return "#C7898C";
        }
        else if (data.status === "alert") {
            return '#F4AD02';
        }
    }
    self.onClickClosePopup = function (obj, event) {
        workflowSetup.controls.workflowSetup_messagePopup().css('display', 'none');
    }
}
workflowSetup.convertCheckBoxToToggle = function (checkboxID) {
    var id = "sm_toggle_" + checkboxID;
    if ($("body").find("#" + id).length === 0) {
        var checkBox = $("#" + checkboxID);
        checkBox[0].style.display = "none";

        //By Default NO is Selected
        var HTML = "<div id='" + id + "' class='sm_toggleContainer'>";
        HTML += "<div class='sm_toggleText'>YES</div>";
        HTML += "<div class='sm_toggleText'>NO</div>";
        if (!checkBox.is(":checked")) {
            HTML += "<div class='sm_toggleBtn' style='margin-left:30px;'>NO</div>";
        }
        else {
            HTML += "<div class='sm_toggleBtn'>YES</div>";
        }
        HTML += "</div>";

        checkBox.after(HTML);

        checkBox.next().unbind('click').bind('click', function (e) {
            var target = $(e.target);
            if (target.hasClass("sm_toggleText")) {
                target = target.parent();
            }
            if (target.hasClass("sm_toggleBtn")) {
                target = target.parent();
            }
            target = target.find(".sm_toggleBtn");
            if (target.css("margin-left") !== "30px") {
                target.animate({ "margin-left": "30px" }, function () {
                    $("#" + id).find(".sm_toggleBtn").text("NO");
                    checkBox.prop("checked", false);
                });
            }
            else {
                target.animate({ "margin-left": "0px" }, function () {
                    $("#" + id).find(".sm_toggleBtn").text("YES");
                    checkBox.prop("checked", true);
                });
            }
        });
    }
}

$(document).ready(function () {
    workflowSetup.init();

});

$(document).click(function (event) {
    var target = $(event.target);
    if (workflowSetup._workflowViewModel !== null && workflowSetup._workflowViewModel.workflowList().length !== 0) {
        if ($(target).parents(".workflowSetup_addNewWorkflow").length === 0) {
            if (workflowSetup.controls.workflowSetup_addNewWorkflow().css("display") === "block" || workflowSetup.controls.workflowSetup_addNewWorkflow().css("display") === "") {
                workflowSetup.controls.workflowSetup_addNewWorkflow().css("display", "none");
            }
        }
//        else if ($(target).parents(".workflowSetup_addNewWorkflow").length > 0) {
//            if ($('#workflowSetup_subSecMasterMenuSelector').css("display") === "" || $('#workflowSetup_subSecMasterMenuSelector').css("display") === "block" && $(target).text() === "RefMaster") {
//                $('#workflowSetup_subSecMasterMenuSelector').css("display", "none"); 
//                $('#workflowSetup_subRefMasterMenuSelector').css("display", "");
//            }
//            else if ($('#workflowSetup_subRefMasterMenuSelector').css("display") === "" || $('#workflowSetup_subRefMasterMenuSelector').css("display") === "block" && $(target).text() === "SecMaster") {
//                $('#workflowSetup_subRefMasterMenuSelector').css("display", "none");
//                $('#workflowSetup_subSecMasterMenuSelector').css("display", "");
//            }
//        }
    }

});