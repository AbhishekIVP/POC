
RADDataPrivilegesConfig = {};
RADDataPrivilegesConfig.initialize = function (initObj) {
    $.extend(RADDataPrivilegesConfig, initObj);    
    if (!initObj.IsIagoDependent) {
        
        }

    var obj = new RADDataPrivileges();
    obj.init();
};


var RADDataPrivileges = function () {

};

RADDataPrivileges.prototype.init = function (sandBox) {
    RADDataPrivileges.instance = this;
    RADDataPrivileges.instance.OnPageLoad();
}

/* Functional Group Screen Starts */

RADDataPrivileges.prototype.OnPageLoad = function () {
    var self = this;
    self.IsValid = false;
    RADDataPrivileges.instance.AllHierarchies = [];
    RADDataPrivileges.instance.CommonDataForCreateHierarchy = {};
    RADDataPrivileges.instance.AllRolesInfo = [];
    RADDataPrivileges.instance.AllModulesInfo = [];
    RADDataPrivileges.instance.CommonEntities = ["Groups", "Roles"]
    RADDataPrivileges.instance.ServiceURL = RADDataPrivilegesConfig.baseUrl + "/Resources/Services/RADUserManagement.svc";
    RADDataPrivileges.instance.Privileges = [];

    var url = RADDataPrivilegesConfig.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAuthorizationPrivileges', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'

    }).then(function (responseText) {
        if (responseText.d == "admin") {
            RADDataPrivileges.instance.Privileges = ["Add Data Privilege", "Update Data Privilege", "Delete Data Privilege", "Add Hierarchy", "View Data Privilege", "Delete Hierarchy", "View Data Hierarchy"];
        }
        else {
            var ResponseForCreation = [];
            if (responseText.d.length > 0)
                ResponseForCreation = JSON.parse(responseText.d);
            Priviliges = []
            for (var i = 0; i < ResponseForCreation.length; i++) {
                if (ResponseForCreation[i].pageId == "RAD_Data_Privileges") {
                    for (var j = 0; j < ResponseForCreation[i].Privileges.length; j++) {
                        RADDataPrivileges.instance.Privileges.push(ResponseForCreation[i].Privileges[j]);
                    }
                }
            }
        }

        $.ajax({
            url: RADDataPrivileges.instance.ServiceURL + "/GetTagTemplates",
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ fileName: 'RADCreateFunctionalGroup.html' })

        }).then(function (responseText) {
            $("#" + RADDataPrivilegesConfig.contentBodyId).empty();
            $("#" + RADDataPrivilegesConfig.contentBodyId).append(responseText.d);
            if (RADDataPrivileges.instance.Privileges.indexOf("Add Data Privilege") == -1) {
                $(".RADAddFuncGroup").addClass("RADCreateHierachyDisplayNone");
            }
            else {
                $(".RADAddFuncGroup").removeClass("RADCreateHierachyDisplayNone");
            }
            if (RADDataPrivileges.instance.Privileges.indexOf("View Data Hierarchy") == -1) {
                $(".RADAddHierarchy").addClass("RADCreateHierachyDisplayNone");
            }
            else {
                $(".RADAddHierarchy").removeClass("RADCreateHierachyDisplayNone");
            }
            $("#RADFuncGroupsContentDiv").height(($("#RADFuncGroupsMainDiv").height() - ($("#RADFuncGroupsHeaderDiv").height())));
            $(".RADExistingDataPrivileges").height($("#RADFuncGroupsContentDiv").height());
            RADDataPrivileges.instance.BindEventsCommon();
            RADDataPrivileges.prototype.GetModuleDetails();
        });
    });
}

RADDataPrivileges.prototype.BindEventsCommon = function () {
    var self = this;
    $("#btnAddHierarchyDiv").unbind("click").click(function (event) {
        RADDataPrivileges.instance.GetHierarchyPageLoad();

    });
    $("#btnAddFuncGroupDiv").unbind("click").click(function (event) {
        RADDataPrivileges.instance.OnAddFunctionalGroup();
    });
    $("#RADFuncGroupsMainDiv").unbind().click(function (event) {
        if ($(event.target).closest(".RADDataprivilegeSearch").length > 0) {
            //do nothing
        }
        else if ($(event.target).closest(".RADFuncGroupsFilter").length > 0) {
            RADDataPrivileges.instance.DataPrivilegeFilterPageLoad(event);
        }
        else if ($(event.target).closest(".RADDimensionDropDownChild,.RADDimensionDropDownChildSelectAll, .RADDimensionDropDownChildUnSelectAll").length > 0) {
            //called when any of the value in the dropdown is selected
            RADDataPrivileges.instance.SelectDimensionValuesDropDown(event);
        }
        else if ($(event.target).closest(".RADModulesDropDownChild,.RADModulesDropDownChildSelectAll,.RADModulesDropDownChildUnSelectAll").length > 0) {
            RADDataPrivileges.instance.SelectModulesDropDown(event);
        }
        else if (($(event.target).closest(".RADFuncGroupCreateEntitySelectText").length > 0)
                 || ($(event.target).closest(".RADExistingDataPrivilegeEntitySelect").length > 0 &&
                      $(event.target).closest(".RADExistingDataPrivilegeParent").attr("isEditable") == "true")) {
            //called when any of the dropdown is selected
            RADDataPrivileges.instance.SelectBindingType(event);
        }        
        else if (($(event.target).closest(".RADFuncGroupModuleNamesSelectionText").length > 0)
                 || ($(event.target).closest(".RadDataPrivilegeModuleValuesSelectText").length > 0 &&
                     $(event.target).closest(".RADExistingDataPrivilegeParent").attr("isEditable") == "true")) {
            var filteredModules = RADDataPrivileges.prototype.GetModulesForHierarchy(event, RADDataPrivileges.instance.AllModulesInfo);
            if (filteredModules != null)
                RADDataPrivileges.instance.BindModuleDetails(event, filteredModules);
        }
        else if (($(event.target).closest(".btnRADSaveFuncGroup").length > 0) || ($(event.target).closest(".RADSavePrivlegeBtn").length > 0)) {
            //called when saving data privilege    
            RADDataPrivileges.instance.OnSaveDataPrivilege(event);
        }
        else if (($(event.target).closest(".RADCancelPrivlegeBtn").length > 0)) {
            //called on cancel of data privilege
            RADDataPrivileges.instance.OnCancelDataPrivilege(event);
        }
        else if ($(event.target).closest(".RADDataPvgEditBtn").length > 0) {
            //called when edit data privilege is called
            $(".RADFuncGroupsCreateContent").addClass("RADCreateHierachyDisplayNone");
            $(".RADExistingDataPrivileges").height(($("#RADFuncGroupsMainDiv").height() - ($("#RADFuncGroupsHeaderDiv").height())));
            $(event.target).closest(".RADExistingDataPrivilegeParent").find(".RADDataPvgEditBtnParent").addClass("RADCreateHierachyDisplayNone");
            $(event.target).closest(".RADExistingDataPrivilegeParent").find(".RADDataPvgDeleteBtnParent").addClass("RADCreateHierachyDisplayNone");
            RADDataPrivileges.instance.OnEditDataPrivilege(event);
        }
        else if (($(event.target).closest(".btnRADCancelFuncGroup").length > 0)) {
            $("#RADFuncGroupCreateEntitiesContentDiv").empty();
            $("#RADFuncGroupHierarcyNameSelectionTextDiv").text("Select Hierarchy");
            $("#RADFuncGroupNameValueCreateDiv").text("");
            $(".RADFuncGroupsCreateContent").addClass("RADCreateHierachyDisplayNone");
            $(".RADExistingDataPrivileges").height(($("#RADFuncGroupsMainDiv").height() - ($("#RADFuncGroupsHeaderDiv").height())));
        }
        else {
            $(".RADFunGroupDimensionsMainDiv").remove();
            $(".RADFunGroupModulesMainDiv").remove();
            $("#RADHierarchyMainDiv").remove();
            $(".RADAllHierarchiesMainDiv").remove();
            if (!$(event.target).hasClass("RADDataPvgDeleteBtn") && !$(event.target).hasClass("RADDeleteBtnPrivilege"))
                $(".RADDeletePrivilege").remove();

        }
    });
}

RADDataPrivileges.prototype.GetModulesForHierarchy = function (event, allModules) {
    var filteredModules = [];
    var selectedHierarchy = $(event.target).closest(".RADFuncGroupsCreateContent,.RADExistingDataPrivilegeParent")
                                                  .find(".RADFuncGroupHierarcyNameSelectionText,.RadHierarchyNameText").text().trim();
    if (selectedHierarchy != "Select Hierarchy") {
        var currentHierarchyDetails = $.grep(RADDataPrivileges.instance.AllHierarchies, function (e)
        { return e.HierarchyName == selectedHierarchy; });

        currentHierarchyDetails[0].Modules.forEach(function (currentModule) {
            var currentModuleInfo = $.grep(allModules, function (e)
            { return e.ModuleName == currentModule; });
            filteredModules.push(currentModuleInfo[0]);
        });
        return filteredModules;
    }
    else {
        var messageDiv = $(event.target).closest('.RADFuncGroupsCreateContent').find('.RADFuncGroupsCreateContentMessage');
        messageDiv.text("Please select hierarchy");
        messageDiv.removeClass("RADCreateHierachyDisplayNone");
    }
    
}

RADDataPrivileges.prototype.SearchCurrentDropDown = function (event) {
    var currentInputText = $(event.target).text().trim().toLowerCase();
    var parentControls = $(event.target).closest(".RADDimensionDropDownParent").find(".RADDimensionDropDownChildParent");

    for (var i = 0; i < parentControls.length; i++) {
        if ($(parentControls[i]).find(".RADDimensionDropDownChild").text().trim().toLowerCase().indexOf(currentInputText) == -1) {
            $(parentControls[i]).addClass("RADCreateHierachyDisplayNone");
        }
        else {
            $(parentControls[i]).removeClass("RADCreateHierachyDisplayNone");
        }
    }
    if (currentInputText != "")
        $(event.target).closest(".RADDimensionDropDownParent").find(".RADDimensionDropDownChildSelectAll").addClass("RADCreateHierachyDisplayNone");
    else
        $(event.target).closest(".RADDimensionDropDownParent").find(".RADDimensionDropDownChildSelectAll").removeClass("RADCreateHierachyDisplayNone");
}

RADDataPrivileges.prototype.SelectBindingType = function (event) {

    var entityAttributeName = $(event.target).closest(".RADFuncGroupCreateEntitySelectText,.RADExistingDataPrivilegeEntitySelect")
                                          .attr("radentityattributenametext").trim().split("|^");

   
    var currentControl = $(event.target).closest(".RADSelectedEntityParent");

    

    if (RADDataPrivileges.instance.CommonEntities.indexOf(entityAttributeName[0]) == -1) {
        var entityAttributeId = $(event.target).closest(".RADFuncGroupCreateEntitySelectText,.RADExistingDataPrivilegeEntitySelect")
                                         .attr("radentityattributeid").trim().split("|^");
        var entitySearchRequest = {};
        entitySearchRequest.EntityId = entityAttributeId[0];
        entitySearchRequest.EntityName = entityAttributeName[0];
        entitySearchRequest.AttributeId = entityAttributeId[1];
        entitySearchRequest.AttributeName = entityAttributeName[1];
        RADDataPrivileges.instance.BindDimensionValues(currentControl, entitySearchRequest);
    }
    else {
        RADDataPrivileges.instance.BindCommonEntityValues(currentControl, entityAttributeName[0]);
    }
}

RADDataPrivileges.prototype.GetModuleDetails = function () {

    $.ajax({
        url: RADDataPrivileges.instance.ServiceURL + '/GetModuleAPIMap',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        var allModules = JSON.parse(responseText.d);
        RADDataPrivileges.instance.AllModulesInfo = allModules;
        //Get All Roles
        $.ajax({
            url: RADDataPrivileges.instance.ServiceURL + '/GetAllRoles',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json'
        }).then(function (responseText) {
            var allRoles = JSON.parse(responseText.d);
            RADDataPrivileges.instance.AllRolesInfo = allRoles;
            RADDataPrivileges.instance.GetAllHierarchiesPageLoad();
            RADDataPrivileges.instance.GetAllDataPrivileges();
        });
    });
}

RADDataPrivileges.prototype.GetAllHierarchiesPageLoad = function () {
    $.ajax({
        url: RADDataPrivileges.instance.ServiceURL + '/GetAllHierarchies',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        if (responseText.d != "") {
            var allHierarchies = JSON.parse(responseText.d);
            RADDataPrivileges.instance.AllHierarchies = allHierarchies;
        }
    });
}

RADDataPrivileges.prototype.BindModuleDetails = function (event, allModules) {

    var currentControl = $(event.target).closest(".RADFuncGroupModuleNamesSelection,.RadDataPrivilegeModuleValuesSelect");
    var textControl = currentControl.find(".RADFuncGroupModuleNamesSelectionText,.RadDataPrivilegeModuleValuesSelectText");

    var selectedModulesArray = [];
    var currentSelectedModules = textControl.attr("radselectedvalues");
    if (currentSelectedModules != null) {
        if (currentSelectedModules == "ALL" || currentSelectedModules == "ALL|^") {
            allModules.forEach(function (currentModule) {
                selectedModulesArray.push(currentModule.ModuleName);
            });
        }
        else {
            selectedModulesArray = currentSelectedModules.split("|^");
            if (selectedModulesArray.indexOf("") >= 0)
                selectedModulesArray.splice(selectedModulesArray.indexOf(""), 1);
        }
    }

    $(".RADFunGroupModulesMainDiv").remove();

    var $modulesMainDiv = $("<div>", {
        class: "RADFunGroupModulesMainDiv"
    });
    var $currentRowModulesParent = $("<div>", {
        class: "RADModulesDropDownParent"
    });

    var $selectAllRowDiv = $("<div>", {
        class: "RADModulesDropDownChildSelectAll",
        text: "Select All"
    });

    if (selectedModulesArray.length != allModules.length)
        $selectAllRowDiv.attr("isselectedmode", "false");
    else {
        $selectAllRowDiv.attr("isselectedmode", "true");
        $selectAllRowDiv.addClass("RADModulesDropDownChildUnSelectAll");
    }

    $currentRowModulesParent.append($selectAllRowDiv);

    for (var i = 0; i < allModules.length; i++) {
        if (allModules[i] != null) {
            var $currentParent = $("<div>", {
                class: "RADModulesDropDownChildParent"
            });
            var $currentRowModule = $("<div>", {
                class: "RADModulesDropDownChild",
                text: allModules[i].ModuleName
            });
            $currentRowModule.attr("radmoduleid", allModules[i].ModuleId);
            var $currentRowModuleTick = $("<div>", {
                class: "RADCreateHierachyDisplayNone RADModulesDropDownChildTick fa fa-check"
            });
            $currentParent.append($currentRowModule);
            $currentParent.append($currentRowModuleTick);

            if (selectedModulesArray.indexOf(allModules[i].ModuleName) != -1)
                $currentRowModuleTick.removeClass("RADCreateHierachyDisplayNone");
            $currentRowModulesParent.append($currentParent);
        }
    }
    $modulesMainDiv.append($currentRowModulesParent);
    currentControl.append($modulesMainDiv);
}

RADDataPrivileges.prototype.OnAddFunctionalGroup = function () {
    /// <summary>
    /// 1. This method is called when we are adding the functional group
    /// 2. It will give the default control, for selecting the hierarchy and the data privilege name
    /// </summary>
    var self = this;
    self.IsValid = false;
    $("#RADFuncGroupsCreateContentDiv").removeClass("RADCreateHierachyDisplayNone");
    $(".RADPermissiveClassRadio").css({ "pointer-events": "all" });
    $(".RADDataPrivIsPermissiveRadio").find(".RADPermissiveClassRadio")[0].checked = true;

    $("#RADFuncGroupsContentDiv").height(($("#RADFuncGroupsMainDiv").height() - ($("#RADFuncGroupsHeaderDiv").height())));
    $(".RADExistingDataPrivileges").height(($("#RADFuncGroupsMainDiv").height() - ($("#RADFuncGroupsHeaderDiv").height())) - $("#RADFuncGroupsCreateContentDiv").height());

    $("#RADFuncGroupHierarcyNameSelectionDiv").unbind("click").click(function (event) {
        if ($(event.target).closest(".RADHierarchyDropDownChild").length > 0) {
            $("#RADFuncGroupCreateEntitiesContentDiv").empty();
            RADDataPrivileges.instance.OnHierarchyNameSelected(event);
        }
        else {
            RADDataPrivileges.instance.OnHierarchySelection();
        }
    });
}

RADDataPrivileges.prototype.OnHierarchySelection = function () {
    /// <summary>
    /// 1. This method will bind all the hierarchies, when the hierarchies dropdown is selected, during add functional group
    /// </summary>
    $.ajax({
        url: RADDataPrivileges.instance.ServiceURL + '/GetAllHierarchies',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        if (responseText.d != "") {
            var allHierarchies = JSON.parse(responseText.d);
            RADDataPrivileges.instance.AllHierarchies = allHierarchies;
            RADDataPrivileges.instance.BindAllHierarchies(allHierarchies);
        }
        else {
            //failed message
        }
    });
}

RADDataPrivileges.prototype.BindAllHierarchies = function (allHierarchies) {

    var hierarcyNames = [];
    allHierarchies.forEach(function (hierarchyItem) { hierarcyNames.push(hierarchyItem.HierarchyName); });
    $(".RADAllHierarchiesMainDiv").remove();
    $(".RADFunGroupDimensionsMainDiv").remove();

    var $hierarchiesMainDiv = $("<div>", {
        class: "RADAllHierarchiesMainDiv"
    });

    var $currentRowHierarchyParent = $("<div>", {
        class: "RADHierarchyDropDownParent"
    });

    for (var i = 0; i < hierarcyNames.length; i++) {

        var $currentRowHierarchy = $("<div>", {
            class: "RADHierarchyDropDownChild",
            text: hierarcyNames[i]
        });
        $currentRowHierarchyParent.append($currentRowHierarchy);
    }
    $hierarchiesMainDiv.append($currentRowHierarchyParent);
    $(".RADFuncGroupHierarcyNameSelection,.RADDataPrivFilterHierarchySelect").append($hierarchiesMainDiv);
}

RADDataPrivileges.prototype.OnHierarchyNameSelected = function (event, isFilteredMode) {
    /// <summary>
    /// 1. This method is called when we are selecting and hierarchy during add functional group
    /// 2. This method will populate controls, based upon the attributes selected in the hierarchy selected
    /// </summary>

    var messageDiv = $(event.target).closest('.RADFuncGroupsCreateContent').find('.RADFuncGroupsCreateContentMessage');
    messageDiv.text("");
    messageDiv.addClass("RADCreateHierachyDisplayNone");
    var currentHierarchyName = $(event.target).text();
    $("#RADFuncGroupHierarcyNameSelectionTextDiv").text(currentHierarchyName);
    $(".RADAllHierarchiesMainDiv").remove();
    RADDataPrivileges.instance.GetCommonDataForCreateHierarchy(currentHierarchyName);
}

RADDataPrivileges.prototype.GetCommonDataForCreateHierarchy = function (currentHierarchyName) {
    RADDataPrivileges.instance.CommonDataForCreateHierarchy = {};
    var self = this;
    self.AllEntitiesForHierarchy = [];
    $.ajax({
        url: RADDataPrivileges.instance.ServiceURL + '/GetCommonDataForCreateHierarchy',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        var commonData = responseText.d;
        for (var j = 0; j < commonData.length; j++) {
            RADDataPrivileges.instance.CommonDataForCreateHierarchy[commonData[j].Key] = commonData[j].Value;
        }
        RADDataPrivileges.prototype.BindCommonDataForCreateHierarchy();
        var currentHierarchyDetails = $.grep(RADDataPrivileges.instance.AllHierarchies, function (e)
        { return e.HierarchyName == currentHierarchyName; });

        for (var i = 0; i < currentHierarchyDetails[0].HierarchyEntitiesInfo.length; i++) {
            var currentEntityId = currentHierarchyDetails[0].HierarchyEntitiesInfo[i].EntityTypeId;
            var currentEntityName = currentHierarchyDetails[0].HierarchyEntitiesInfo[i].EntityTypeName;
            var currentEntityAttributeInfo = currentHierarchyDetails[0].HierarchyEntitiesInfo[i].AttributesInfo;
            for (var j = 0; j < currentEntityAttributeInfo.length; j++) {
                var currentAttributeInfo = currentEntityAttributeInfo[j];
                //if (self.AllEntitiesForHierarchy.indexOf(currentEntityName) == -1)
                //    self.AllEntitiesForHierarchy.push(currentEntityName);
                if (self.AllEntitiesForHierarchy.indexOf(currentEntityId) == -1)
                    self.AllEntitiesForHierarchy.push(currentEntityId);
                RADDataPrivileges.instance.BindDimensionData(currentHierarchyName, currentEntityName, currentEntityId, currentAttributeInfo);
            }
        }
    });
}

RADDataPrivileges.prototype.BindCommonDataForCreateHierarchy = function () {

    for (var i = 0; i < RADDataPrivileges.instance.CommonEntities.length; i++) {
        var $entityContentDiv = $("<div>", {
            class: "RADFuncGroupCreateEntityContent"
        });
        var $entityNameDiv = $("<div>", {
            class: "RADFuncGroupCreateEntityName",
            text: RADDataPrivileges.instance.CommonEntities[i]
        });
        $entityNameDiv.attr("commonattrs", true);
        $entityNameDiv.attr("radentityattributename", RADDataPrivileges.instance.CommonEntities[i]);
        var $entitySelectDivParent = $("<div>", {
            class: "RADSelectedEntityParent"
        });
        var $entitySelectDiv = $("<div>", {
            id: "RADFuncGroupCreateEntitySelect" + RADDataPrivileges.instance.CommonEntities[i],
            class: "RADFuncGroupCreateEntitySelect"
        });
        var $entitySelectText = $("<div>", {
            class: "RADFuncGroupCreateEntitySelectText",
            text: "--Select--"
        });
        $entitySelectText.attr("radentityattributenametext", RADDataPrivileges.instance.CommonEntities[i]);
        $entitySelectDiv.append($entitySelectText);
        $entitySelectDivParent.append($entitySelectDiv);

        $entityContentDiv.append($entityNameDiv).append($entitySelectDivParent);
        $("#RADFuncGroupCreateEntitiesContentDiv").append($entityContentDiv);
    }
}

RADDataPrivileges.prototype.BindDimensionData = function (hierarchyName, entityName, entityid, attributeInfo) {

    var $entityContentDiv = $("<div>", {
        class: "RADFuncGroupCreateEntityContent"
    });

    var $entityNameDiv = $("<div>", {
        class: "RADFuncGroupCreateEntityName",
        text: attributeInfo.AttributeDisplayName
    });

    $entityNameDiv.attr("radentityid", entityid);
    $entityNameDiv.attr("radentityname", entityName);
    $entityNameDiv.attr("isLookUp", attributeInfo.IsLookUp);
    $entityNameDiv.attr("isUnique", attributeInfo.isUnique);
    $entityNameDiv.attr("radmappedattribute", attributeInfo.MappedAttributeName);
    $entityNameDiv.attr("radentityattributename", entityName + "|^" + attributeInfo.AttributeName);
    $entityNameDiv.attr("radentityattributeid", entityid + "|^" + attributeInfo.AttributeId);
    if (attributeInfo.IsLookUp) {
        $entityNameDiv.attr("parentAttribute", attributeInfo.LookupEntity.Attribute);
        $entityNameDiv.attr("parentEntity", attributeInfo.LookupEntity.EntityType);
    }

    var $entitySelectDivParent = $("<div>", {
        class: "RADSelectedEntityParent"
    });

    var $entitySelectDiv = $("<div>", {
        id: "RADFuncGroupCreateEntitySelect" + attributeInfo.AttributeDisplayName,
        class: "RADFuncGroupCreateEntitySelect"
    });

    var $entitySelectText = $("<div>", {
        class: "RADFuncGroupCreateEntitySelectText",
        text: "--Select--"
    });
    $entitySelectText.attr("radentityattributenametext", entityName + "|^" + attributeInfo.AttributeName);
    $entitySelectText.attr("radentityattributeid", entityid + "|^" + attributeInfo.AttributeId);
    $entitySelectDiv.append($entitySelectText);
    $entitySelectDivParent.append($entitySelectDiv);

    $entityContentDiv.append($entityNameDiv).append($entitySelectDivParent);
    $("#RADFuncGroupCreateEntitiesContentDiv").append($entityContentDiv);
}


RADDataPrivileges.prototype.SelectDimensionCommonDropDown = function (event) {

    var textControl = $(event.target).closest(".RADSelectedEntityParent,.RADDataPrivFilteUserSelectParent")
                               .find(".RADFuncGroupCreateEntitySelectText,.RADExistingDataPrivilegeEntitySelect,.RADDataPrivFilterUserSelectText");
    var selectedDimension = $(event.target).text();

    if ($(event.target).hasClass("RADDimensionDropDownChildSelectAll") && $(event.target).attr("isselectedmode") == "false") {
        //Select All Case
        $(event.target).attr("isselectedmode", "true");
        $(event.target).addClass("RADDimensionDropDownChildUnSelectAll");
        $(event.target).closest(".RADFunGroupDimensionsMainDiv").find(".RADDimensionDropDownChildTick").removeClass("RADCreateHierachyDisplayNone");
        textControl.attr("radselectedvalues", "ALL");
        textControl.attr("radselectedkeyvaluepair", "ALL");
        textControl.text("ALL");

    }
    else if ($(event.target).hasClass("RADDimensionDropDownChildSelectAll") && $(event.target).attr("isselectedmode") == "true") {
        //Unselect ALL case
        $(event.target).attr("isselectedmode", "false");
        $(event.target).removeClass("RADDimensionDropDownChildUnSelectAll");
        $(event.target).closest(".RADFunGroupDimensionsMainDiv").find(".RADDimensionDropDownChildTick").addClass("RADCreateHierachyDisplayNone");
        textControl.attr("radselectedvalues", "");
        //textControl.attr("radselectedkeyvaluepair", "ALL");
        textControl.text("--Select--");
    }
    else if (!$(event.target).closest(".RADDimensionDropDownChildParent")
                             .find(".RADDimensionDropDownChildTick").hasClass("RADCreateHierachyDisplayNone")) {
        //unselect one
        textControl.text("");

        $(event.target).closest(".RADDimensionDropDownChildParent").find(".RADDimensionDropDownChildTick").addClass("RADCreateHierachyDisplayNone");
        var existingItemObjectArray = [];
        $(event.target).closest(".RADFunGroupDimensionsMainDiv").find(".RADDimensionDropDownChildSelectAll").attr("isselectedmode", "false");
        $(event.target).closest(".RADFunGroupDimensionsMainDiv").find(".RADDimensionDropDownChild").each(function (index) {
            if (!$(this).closest(".RADDimensionDropDownChildParent").find(".RADDimensionDropDownChildTick").hasClass("RADCreateHierachyDisplayNone")) {
                existingItemObjectArray.push($(this).text().trim());
            }
        });
        RADDataPrivileges.instance.SetCommonDimensionsTextandAttributeText(textControl, existingItemObjectArray);

    }
    else {
        textControl.text("");
        $(event.target).closest(".RADDimensionDropDownChildParent").find(".RADDimensionDropDownChildTick").removeClass("RADCreateHierachyDisplayNone");
        var existingItemObjectArray = [];
        $(event.target).closest(".RADFunGroupDimensionsMainDiv").find(".RADDimensionDropDownChild").each(function (index) {
            if (!$(this).closest(".RADDimensionDropDownChildParent").find(".RADDimensionDropDownChildTick").hasClass("RADCreateHierachyDisplayNone")) {
                existingItemObjectArray.push($(this).text().trim());
            }
        });
        RADDataPrivileges.instance.SetCommonDimensionsTextandAttributeText(textControl, existingItemObjectArray);

    }
}

RADDataPrivileges.prototype.SetCommonDimensionsTextandAttributeText = function (currentControl, itemsArray) {
    var radDropDownsText = "";
    if (currentControl.text() == "ALL") {
        currentControl.text("ALL");
        currentControl.attr("radselectedvalues", "ALL");
        return;
    }
    else if (itemsArray.length == 0) {
        currentControl.text("--Select--");
        currentControl.attr("radselectedvalues", "");
        return;
    }
    else {
        var dimensionsValuesArray = [];
        itemsArray.forEach(function (currentItem) {
            dimensionsValuesArray.push(currentItem);
            radDropDownsText = radDropDownsText + currentItem + "|^";
        });
        currentControl.attr("radselectedvalues", radDropDownsText);
        if (itemsArray.length <= 2) {
            currentControl.text(dimensionsValuesArray.join());
        }
        else {
            var slicedArray = dimensionsValuesArray.slice(0, 2);
            var selectedText = slicedArray.join();
            selectedText = selectedText + " <span class='RADNMoreClass'>" + (dimensionsValuesArray.length - 2).toString() + " More </span>";
            currentControl.text(selectedText);
        }
    }
}

RADDataPrivileges.prototype.SelectEntityAttributeDropDown = function (event) {

    var textControl = $(event.target).closest(".RADSelectedEntityParent")
                                         .find(".RADFuncGroupCreateEntitySelectText,.RADExistingDataPrivilegeEntitySelect");    
    if ($(event.target).hasClass("RADDimensionDropDownChildSelectAll") && $(event.target).attr("isselectedmode") == "false") {
        //Select All Case
        $(event.target).attr("isselectedmode", "true");
        $(event.target).addClass("RADDimensionDropDownChildUnSelectAll");
        $(event.target).closest(".RADFunGroupDimensionsMainDiv").find(".RADDimensionDropDownChildTick").removeClass("RADCreateHierachyDisplayNone");
        textControl.attr("radselectedvalues", "ALL");
        textControl.attr("radselectedkeyvaluepair", "ALL");
        textControl.text("ALL");    
    }
    else if ($(event.target).hasClass("RADDimensionDropDownChildSelectAll") && $(event.target).attr("isselectedmode") == "true") {
        //Unselect ALL case
        $(event.target).attr("isselectedmode", "false");
        $(event.target).removeClass("RADDimensionDropDownChildUnSelectAll");
        $(event.target).closest(".RADFunGroupDimensionsMainDiv").find(".RADDimensionDropDownChildTick").addClass("RADCreateHierachyDisplayNone");
        textControl.attr("radselectedvalues", "");
        textControl.attr("radselectedkeyvaluepair", "ALL");
        textControl.text("--Select--");
    }
    else if (!$(event.target).closest(".RADDimensionDropDownChildParent")
                                .find(".RADDimensionDropDownChildTick").hasClass("RADCreateHierachyDisplayNone")) {
        //unselect one
        //this below line is usefull for getting the text and attibute currectly, from SetDropDownsAttributeTextandText function
        textControl.text("");
        $(event.target).closest(".RADDimensionDropDownChildParent").find(".RADDimensionDropDownChildTick").addClass("RADCreateHierachyDisplayNone");
        var existingItemObjectArray = [];
        $(event.target).closest(".RADFunGroupDimensionsMainDiv").find(".RADDimensionDropDownChildSelectAll").attr("isselectedmode", "false");
        $(event.target).closest(".RADFunGroupDimensionsMainDiv").find(".RADDimensionDropDownChild").each(function (index) {
            if (!$(this).closest(".RADDimensionDropDownChildParent").find(".RADDimensionDropDownChildTick").hasClass("RADCreateHierachyDisplayNone")) {
                var SelectedDimesion = {};
                SelectedDimesion.SelectedDimensionId = $(this).attr("raduniqueattributeid");
                SelectedDimesion.SelectedDimensionValue = $(this).text().trim();
                existingItemObjectArray.push(SelectedDimesion);
            }
        });
        RADDataPrivileges.instance.SetDropDownsAttributeTextandText(textControl, existingItemObjectArray);
    }
    else {
        //this below line is usefull for getting the text and attibute currectly, from SetDropDownsAttributeTextandText function
        textControl.text("");
        $(event.target).closest(".RADDimensionDropDownChildParent").find(".RADDimensionDropDownChildTick").removeClass("RADCreateHierachyDisplayNone");
        var existingItemObjectArray = [];
        $(event.target).closest(".RADFunGroupDimensionsMainDiv").find(".RADDimensionDropDownChild").each(function (index) {
            if (!$(this).closest(".RADDimensionDropDownChildParent").find(".RADDimensionDropDownChildTick").hasClass("RADCreateHierachyDisplayNone")) {
                var SelectedDimesion = {};
                SelectedDimesion.SelectedDimensionId = $(this).attr("raduniqueattributeid");
                SelectedDimesion.SelectedDimensionValue = $(this).text().trim();
                existingItemObjectArray.push(SelectedDimesion);
            }
        });
        RADDataPrivileges.instance.SetDropDownsAttributeTextandText(textControl, existingItemObjectArray);
    }
}

RADDataPrivileges.prototype.SelectDimensionValuesDropDown = function (event) {

    currentSelectedDimensionsArray = [];
    var textControl = $(event.target).closest(".RADSelectedEntityParent,.RADDataPrivFilteUserSelectParent")
                                     .find(".RADFuncGroupCreateEntitySelectText,.RADDataPrivFilterUserSelectText");
    var selectedDimension = $(event.target).text();

    if ($(event.target).attr("radisfilteredbylookup") == null) {
        RADDataPrivileges.instance.SelectDimensionCommonDropDown(event);
    }
    else {
        RADDataPrivileges.instance.SelectEntityAttributeDropDown(event);
    }
}

RADDataPrivileges.prototype.SelectModulesDropDown = function (event) {

    var textControl = $(event.target).closest(".RADFuncGroupModuleNamesSelectionParent,.RadDataPrivilegeModuleValuesParent")
                                     .find(".RADFuncGroupModuleNamesSelectionText,.RadDataPrivilegeModuleValuesSelectText");
    var selectedModule = $(event.target).text();
    var selectionMode = "";

    var existingItemObjectArray = [];
    var existingItemIdsArray = [];

    if ($(event.target).hasClass("RADModulesDropDownChildSelectAll") && $(event.target).attr("isselectedmode") == "false") {
        //Select All Case
        $(event.target).attr("isselectedmode", "true");
        $(event.target).addClass("RADModulesDropDownChildUnSelectAll");
        $(event.target).closest(".RADFunGroupModulesMainDiv").find(".RADModulesDropDownChildTick").removeClass("RADCreateHierachyDisplayNone");
        selectionMode = "SelectAll";
        $(event.target).closest(".RADFunGroupModulesMainDiv").find(".RADModulesDropDownChild").each(function (index) {
            if (!$(this).closest(".RADModulesDropDownChildParent").find(".RADModulesDropDownChildTick").hasClass("RADCreateHierachyDisplayNone")) {
                existingItemObjectArray.push($(this).text().trim());
                existingItemIdsArray.push($(this).attr("radmoduleid"));
            }
        });
        textControl.attr("radselectedvalues", "ALL");
        textControl.attr("radselectedmoduleids", "ALL");

    }
    else if ($(event.target).hasClass("RADModulesDropDownChildSelectAll") && $(event.target).attr("isselectedmode") == "true") {
        //Unselect ALL case
        $(event.target).attr("isselectedmode", "false");
        $(event.target).removeClass("RADModulesDropDownChildUnSelectAll");
        $(event.target).closest(".RADFunGroupModulesMainDiv").find(".RADModulesDropDownChildTick").addClass("RADCreateHierachyDisplayNone");
        selectionMode = "UnSelectAll";
    }
    else if (!$(event.target).closest(".RADModulesDropDownChildParent")
                             .find(".RADModulesDropDownChildTick").hasClass("RADCreateHierachyDisplayNone")) {
        //unselect one
        existingItemObjectArray = [];
        existingItemIdsArray = [];
        $(event.target).closest(".RADModulesDropDownChildParent").find(".RADModulesDropDownChildTick").addClass("RADCreateHierachyDisplayNone");
        $(event.target).closest(".RADFunGroupModulesMainDiv").find(".RADModulesDropDownChildSelectAll").attr("isselectedmode", "false");
        $(event.target).closest(".RADFunGroupModulesMainDiv").find(".RADModulesDropDownChild").each(function (index) {
            if (!$(this).closest(".RADModulesDropDownChildParent").find(".RADModulesDropDownChildTick").hasClass("RADCreateHierachyDisplayNone")) {
                existingItemObjectArray.push($(this).text().trim());
                existingItemIdsArray.push($(this).attr("radmoduleid"));
            }
        });
    }

    else {
        $(event.target).closest(".RADModulesDropDownChildParent").find(".RADModulesDropDownChildTick").removeClass("RADCreateHierachyDisplayNone");
        existingItemObjectArray = [];
        existingItemIdsArray = [];
        $(event.target).closest(".RADFunGroupModulesMainDiv").find(".RADModulesDropDownChild").each(function (index) {
            if (!$(this).closest(".RADModulesDropDownChildParent").find(".RADModulesDropDownChildTick").hasClass("RADCreateHierachyDisplayNone")) {
                existingItemObjectArray.push($(this).text().trim());
                existingItemIdsArray.push($(this).attr("radmoduleid"));
            }
        });
    }
    RADDataPrivileges.instance.SetModuleDropDownAttributeTextandText(textControl, existingItemObjectArray, existingItemIdsArray);
    if ($(event.target).closest(".RADFuncGroupsCreateContentChild,.RADExistingDataPrivilegeChild")
                       .find(".RADFuncGroupCreateEntityContent,.RADExistingDataPrivilegeContent").length > 0) {
        RADDataPrivileges.instance.SetRolesByModules(event, existingItemObjectArray, existingItemIdsArray, selectionMode);
    }
}

RADDataPrivileges.prototype.SetModuleDropDownAttributeTextandText = function (currentControl, selectValuesArray, selectIdsArray) {
    var radDropDownsText = "";
    if (currentControl.text() == "ALL" && selectValuesArray.length == 0) {
        currentControl.text("ALL");
        currentControl.attr("radselectedvalues", "ALL");
        currentControl.attr("radselectedmoduleids", "ALL");
        return;
    }
    else if (selectValuesArray.length == 0) {
        currentControl.text("ALL");
        currentControl.attr("radselectedvalues", "");
        currentControl.attr("radselectedmoduleids", "");
        return;
    }
    else {
        var dimensionsValuesArray = [];
        selectValuesArray.forEach(function (currentItem) {
            dimensionsValuesArray.push(currentItem);
            radDropDownsText = radDropDownsText + currentItem + "|^";
        });
        currentControl.attr("radselectedvalues", radDropDownsText);

        radDropDownsText = "";
        selectIdsArray.forEach(function (currentItem) {
            radDropDownsText = radDropDownsText + currentItem + "|^";
        });
        currentControl.attr("radselectedmoduleids", radDropDownsText);

        if (selectValuesArray.length <= 2) {
            currentControl.text(dimensionsValuesArray.join());
        }
        else {
            var slicedArray = dimensionsValuesArray.slice(0, 2);
            var selectedText = slicedArray.join();
            selectedText = selectedText + " <span class='RADNMoreClass'>" + (dimensionsValuesArray.length - 2).toString() + " More </span>";
            currentControl.text(selectedText);
        }
    }
}

RADDataPrivileges.prototype.SetRolesByModules = function (event, existingItemObjectArray, existingItemIdsArray, selectionMode) {

    var rolesParentControl = $(event.target).closest(".RADFuncGroupsCreateContentChild,.RADExistingDataPrivilegeChild")
                                            .find(".RADFuncGroupCreateEntityContent,.RADExistingDataPrivilegeContent")
                                            .find("div[radentityattributename='Roles']").next();
    var filteredRoles = [];

    existingItemIdsArray.forEach(function (moduleId) {
        var roleForModule = $.grep(RADDataPrivileges.instance.AllRolesInfo, function (e)
        { return e.ModuleId == moduleId; });

        roleForModule.forEach(function (currentRole) {
            filteredRoles.push(currentRole.RoleName);
        });
    });
    RADDataPrivileges.instance.BindRolesByModules(rolesParentControl, filteredRoles);
}

RADDataPrivileges.prototype.BindRolesByModules = function (rolesParentControl, filteredRoles) {
    var valuesForAttributeText = [];
    var textControl = rolesParentControl.find(".RADFuncGroupCreateEntitySelectText,.RADExistingDataPrivilegeEntitySelect");
    var selectedRolesArray = [];

    var currentSelectedRoles = textControl.attr("radselectedvalues");
    if (currentSelectedRoles != null) {
        if (currentSelectedRoles == "ALL" || currentSelectedRoles == "ALL|^")
            selectedRolesArray = filteredRoles;
        else {
            selectedRolesArray = currentSelectedRoles.split("|^");
            if (selectedRolesArray.indexOf("") >= 0)
                selectedRolesArray.splice(selectedRolesArray.indexOf(""), 1);
        }
    }

    selectedRolesArray.forEach(function (currentitem) {
        if (filteredRoles.indexOf(currentitem) >= 0)
            valuesForAttributeText.push(currentitem);
    });

    RADDataPrivileges.instance.SetCommonDimensionsTextandAttributeText(textControl, valuesForAttributeText);
}

RADDataPrivileges.prototype.GetRolesbyModuleIds = function (moduleIdsArray) {

    var filteredRoles = [];
    moduleIdsArray.forEach(function (currentModuleId) {
        var rolesInfo = $.grep(RADDataPrivileges.instance.AllRolesInfo, function (e)
        { return e.ModuleId == currentModuleId; });

        rolesInfo.forEach(function (currentRoleInfo) {
            filteredRoles.push(currentRoleInfo.RoleName);
        });
    });

    return filteredRoles;
}

RADDataPrivileges.prototype.GetFilteredRoles = function (currentControl, dimensionData) {

    /*
    var selectedModuleIds = [];
    var currentModuleControl = $(currentControl).closest(".RADFuncGroupsCreateContent,.RADExistingDataPrivilegeChild")
                                        .find("div[radentityattributename='Modules']");

    var currentSelectedModules = currentModuleControl.attr("radselectedmoduleids");
    if (currentSelectedModules != null) {

        if (currentSelectedModules == "ALL" || currentSelectedModules == "ALL|^") {

            RADDataPrivileges.instance.AllModulesInfo.forEach(function (currentItem) {
                selectedModuleIds.push(currentItem.ModuleId);
            });
        }
        else {
            selectedModuleIds = currentSelectedModules.split("|^");
            if (selectedModuleIds.indexOf("") >= 0)
                selectedModuleIds.splice(selectedModuleIds.indexOf(""), 1);
        }
        dimensionData = RADDataPrivileges.prototype.GetRolesbyModuleIds(selectedModuleIds);
    }
    return dimensionData;*/

    var selectedModuleIds = [];
    var currentModuleControl = $(currentControl).closest(".RADFuncGroupsCreateContent,.RADExistingDataPrivilegeChild")
                                        .find("div[radentityattributename='Modules']");
    var currentSelectedModules = currentModuleControl.attr("radselectedmoduleids");

    if (currentSelectedModules != null) {
        selectedModuleIds = currentSelectedModules.split("|^");
        if (selectedModuleIds.indexOf("") >= 0)
            selectedModuleIds.splice(selectedModuleIds.indexOf(""), 1);
    }
    else {

        var currentHierarchyName = $(currentControl).closest(".RADFuncGroupsCreateContent,.RADExistingDataPrivilegeChild")
                                   .find(".RADFuncGroupHierarcyNameSelectionText,.RadHierarchyNameText").text().trim();

        var currentHierarchyDetails = $.grep(RADDataPrivileges.instance.AllHierarchies, function (e)
        { return e.HierarchyName == currentHierarchyName; });

        var moduleIdsText = RADDataPrivileges.instance.GetModuleIdsByNames(currentHierarchyDetails[0].Modules);
        selectedModuleIds = moduleIdsText.split("|^");
        if (selectedModuleIds.indexOf("") >= 0)
            selectedModuleIds.splice(selectedModuleIds.indexOf(""), 1);


    }
    dimensionData = RADDataPrivileges.instance.GetRolesbyModuleIds(selectedModuleIds);
    return dimensionData;



}

RADDataPrivileges.prototype.BindCommonEntityValues = function (currentControl, currentEntityName) {

    var textControl = currentControl.find(".RADFuncGroupCreateEntitySelectText,.RADExistingDataPrivilegeEntitySelect,.RADDataPrivFilterUserSelectText")
    var dimensionData = RADDataPrivileges.instance.CommonDataForCreateHierarchy[currentEntityName];

    //if (currentEntityName == "Roles" && $(currentControl).closest(".RADFuncGroupsCreateContent,.RADExistingDataPrivilegeChild")
    //                                    .find("div[radentityattributename='Modules']").attr("radselectedvalues") != null) {
    //    dimensionData = RADDataPrivileges.prototype.GetFilteredRoles(currentControl, dimensionData);

    if (currentEntityName == "Roles")
        dimensionData = RADDataPrivileges.instance.GetFilteredRoles(currentControl, dimensionData);
    
    var selectedDimesionsArray = [];
    var currentSelectedDimensions = textControl.attr("radselectedvalues");
    if (currentSelectedDimensions != null) {
        if (currentSelectedDimensions == "ALL" || currentSelectedDimensions == "ALL|^")
            selectedDimesionsArray = dimensionData;
        else {
            selectedDimesionsArray = currentSelectedDimensions.split("|^");
            if (selectedDimesionsArray.indexOf("") >= 0)
                selectedDimesionsArray.splice(selectedDimesionsArray.indexOf(""), 1);
        }
    }
    $(".RADAllHierarchiesMainDiv").remove();
    $(".RADFunGroupDimensionsMainDiv").remove();

    var $dimensionsMainDiv = $("<div>", {
        class: "RADFunGroupDimensionsMainDiv"
    });
    var $currentRowDimensionParent = $("<div>", {
        class: "RADDimensionDropDownParent"
    });

    var $searchParentDiv = $("<div>", {
        class: "RADDataprivilegeSearchParent"
    });

    var $searchDiv = $("<div>", {
        class: "RADDataprivilegeSearch"
    });

    var $selectAllRowDiv = $("<div>", {
        class: "RADDimensionDropDownChildSelectAll",
        text: "Select All"
    });

    if (selectedDimesionsArray.length != dimensionData.length)
        $selectAllRowDiv.attr("isselectedmode", "false");
    else {
        $selectAllRowDiv.attr("isselectedmode", "true");
        $selectAllRowDiv.addClass("RADDimensionDropDownChildUnSelectAll");
    }

    $searchDiv.attr("contenteditable", true);
    $searchParentDiv.append($searchDiv);
    $currentRowDimensionParent.append($searchParentDiv);
    $currentRowDimensionParent.append($selectAllRowDiv);

    for (var i = 0; i < dimensionData.length; i++) {
        if (dimensionData[i] != null) {
            var $currentParent = $("<div>", {
                class: "RADDimensionDropDownChildParent"
            });
            var $currentRowDimesion = $("<div>", {
                class: "RADDimensionDropDownChild",
                text: dimensionData[i]
            });
            var $currentRowDimesionTick = $("<div>", {
                class: "RADCreateHierachyDisplayNone RADDimensionDropDownChildTick fa fa-check"
            });
            $currentParent.append($currentRowDimesion);
            $currentParent.append($currentRowDimesionTick);

            if (selectedDimesionsArray.indexOf(dimensionData[i]) != -1)
                $currentRowDimesionTick.removeClass("RADCreateHierachyDisplayNone");
            $currentRowDimensionParent.append($currentParent);
        }
    }
    $dimensionsMainDiv.append($currentRowDimensionParent);
    if (currentControl.closest(".RADFuncGroupCreateEntityContent").length > 0)
        $dimensionsMainDiv.css({ 'left': (currentControl.closest(".RADFuncGroupCreateEntityContent,.RADExistingDataPrivilegeContent,.RADDataPrivFilteUserSelectParent").offset().left + 25 + 'px') });
    else
        $dimensionsMainDiv.css({ 'left': (currentControl.closest(".RADFuncGroupCreateEntityContent,.RADExistingDataPrivilegeContent,.RADDataPrivFilteUserSelectParent").offset().left + 'px') });
    currentControl.append($dimensionsMainDiv);
    $(".RADDataprivilegeSearch").unbind("keyup").keyup(function (event) {
        RADDataPrivileges.instance.SearchCurrentDropDown(event);
    })
}

RADDataPrivileges.prototype.BindDimensionValues = function (currentControl, entitySearchRequest) {

    $(currentControl).addClass("RADDLPdimesionSpinner");
    $("#RADHierarchyMainDiv").css("pointer-events", "none");
    $("#RADFuncGroupsMainDiv").css("pointer-events", "none");

    var hierarchyName = currentControl.closest(".RADFuncGroupsCreateContent,.RADExistingDataPrivilegeParent")
                                         .find("#RADFuncGroupHierarcyNameSelectionTextDiv,.RadHierarchyNameText").text().trim();
    entitySearchRequest.HierarchyName = hierarchyName;
    var apiInfo = RADDataPrivileges.instance.GetApiInfobyHierarchyName(hierarchyName, entitySearchRequest.EntityId);
    var entitySearchRequestText = JSON.stringify(entitySearchRequest);

    $.ajax({
        url: RADDataPrivileges.instance.ServiceURL + '/GetAllDimensions',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({
            entitySearchRequest: entitySearchRequestText, apiType: apiInfo.APiType,
            apiPath: apiInfo.APiPath
        })
    }).then(function (responseText) {
        $(currentControl).removeClass("RADDLPdimesionSpinner");
        $("#RADHierarchyMainDiv").css("pointer-events", "");
        $("#RADFuncGroupsMainDiv").css("pointer-events", "");
        var dimensionData = responseText.d;
        RADDataPrivileges.instance.BindDimensionDropDownData(currentControl, dimensionData);
    });
}

RADDataPrivileges.prototype.BindDimensionDropDownData = function (currentControl, dimensionData) {

    var textControl = currentControl.find(".RADFuncGroupCreateEntitySelectText,.RADExistingDataPrivilegeEntitySelect");
    var selectedDimensionsText = textControl.attr("radselectedvalues");
    var existingDimesionsArray = [];

    if (selectedDimensionsText != null) {
        if (selectedDimensionsText == "ALL~ALL" || selectedDimensionsText == "ALL~ALL|^") {
            dimensionData.forEach(function (dimensionItem) { existingDimesionsArray.push(dimensionItem.Key); });
        }
        else {      
            var selectedDimensionItem = selectedDimensionsText.split("|^");
            if (selectedDimensionItem.indexOf("") >= 0)
                selectedDimensionItem.splice(selectedDimensionItem.indexOf(""), 1);
                
            selectedDimensionItem.forEach(function (currentItem) {
                var currentDimensionItem = currentItem;
                var currentDimensionKeyValue = currentDimensionItem.split("~");
                existingDimesionsArray.push(currentDimensionKeyValue[0]);

            });
        }
    }
    RADDataPrivileges.instance.BindDropDownControl(currentControl, dimensionData, existingDimesionsArray);
}

RADDataPrivileges.prototype.BindDropDownControl = function (currentControl, dimensionData, existingSelectedDimesionsArray) {

    $(".RADFunGroupDimensionsMainDiv").remove();

    var $dimensionsMainDiv = $("<div>", {
        class: "RADFunGroupDimensionsMainDiv"
    });

    var $currentRowDimensionParent = $("<div>", {
        class: "RADDimensionDropDownParent"
    });

    var $searchParentDiv = $("<div>", {
        class: "RADDataprivilegeSearchParent"
    });

    var $searchDiv = $("<div>", {
        class: "RADDataprivilegeSearch"
    });

    var $selectAllRowDiv = $("<div>", {
        class: "RADDimensionDropDownChildSelectAll",
        text: "Select All"
    });
    if (dimensionData.length != existingSelectedDimesionsArray.length)
        $selectAllRowDiv.attr("isselectedmode", "false");
    else {
        $selectAllRowDiv.attr("isselectedmode", "true");
        $selectAllRowDiv.addClass("RADDimensionDropDownChildUnSelectAll");
    }
    $selectAllRowDiv.attr("radisfilteredbylookup", "false");
    $selectAllRowDiv.attr("raduniqueattributeid", "SelectAll");

    $searchDiv.attr("contenteditable", true);
    $searchParentDiv.append($searchDiv);
    $currentRowDimensionParent.append($searchParentDiv);
    $currentRowDimensionParent.append($selectAllRowDiv);

    for (var i = 0; i < dimensionData.length; i++) {
        var currentDimesionKey = dimensionData[i].Key;
        var currentDimesionValue = dimensionData[i].Value;

        var $currentParent = $("<div>", {
            class: "RADDimensionDropDownChildParent"
        });

        var $currentRowDimesion = $("<div>", {
            class: "RADDimensionDropDownChild",
            text: currentDimesionValue
        });

        $currentRowDimesion.attr("radisfilteredbylookup", "false");
        $currentRowDimesion.attr("raduniqueattributeid", currentDimesionKey);

        var $currentRowDimesionTick = $("<div>", {
            class: "RADCreateHierachyDisplayNone RADDimensionDropDownChildTick fa fa-check"
        });

        $currentParent.append($currentRowDimesion);
        $currentParent.append($currentRowDimesionTick);

        if (existingSelectedDimesionsArray.indexOf(currentDimesionKey) != -1)
            $currentRowDimesionTick.removeClass("RADCreateHierachyDisplayNone");
        $currentRowDimensionParent.append($currentParent);
    }

    $dimensionsMainDiv.append($currentRowDimensionParent);
    if (currentControl.closest(".RADFuncGroupCreateEntityContent").length > 0)
        $dimensionsMainDiv.css({ 'left': (currentControl.closest(".RADFuncGroupCreateEntityContent,.RADExistingDataPrivilegeContent").offset().left + 25 + 'px') });
    else
        $dimensionsMainDiv.css({ 'left': (currentControl.closest(".RADFuncGroupCreateEntityContent,.RADExistingDataPrivilegeContent").offset().left + 'px') });
    currentControl.append($dimensionsMainDiv);

    $(".RADDataprivilegeSearch").unbind("keyup").keyup(function (event) {
        RADDataPrivileges.instance.SearchCurrentDropDown(event);
    })
}

RADDataPrivileges.prototype.OnEditDataPrivilege = function (event) {
    var self = this;
    self.IsValid = false;
    $(".RADExistingDataPrivilegeParent").attr("isEditable", "false");
    $(".RADExistingDataPrivilegeParent").find(".RADExistingDataPrivilegeFooter").addClass("RADCreateHierachyDisplayNone");
    $(".RADExistingDataPrivilegeParent").find(".RADPermissiveClassRadio").css({ "pointer-events": "none" });
    
    $(".RADExistingDataPrivilegeFooter").addClass("RADCreateHierachyDisplayNone");
    $(".RADDataPvgEditBtnParent").removeClass("RADCreateHierachyDisplayNone");
    $(".RADDataPvgDeleteBtnParent").removeClass("RADCreateHierachyDisplayNone");


    $(event.target).closest(".RADExistingDataPrivilegeParent").attr("isEditable", "true");
    $(event.target).closest(".RADExistingDataPrivilegeParent").find(".RADExistingDataPrivilegeFooter").removeClass("RADCreateHierachyDisplayNone");
    $(event.target).closest(".RADExistingDataPrivilegeParent").find(".RADPermissiveClassRadio").css({ "pointer-events": "all" });
    $(event.target).closest(".RADExistingDataPrivilegeParent").find(".RADDataPvgEditBtnParent").addClass("RADCreateHierachyDisplayNone");
    $(event.target).closest(".RADExistingDataPrivilegeParent").find(".RADDataPvgDeleteBtnParent").addClass("RADCreateHierachyDisplayNone");


    self.AllEntitiesForHierarchy = [];
    var entityNames = $(event.target).closest(".RADExistingDataPrivilegeParent").find("div[radentityname]");

    //entityNames.each(function (index) {
    //    if (self.AllEntitiesForHierarchy.indexOf($(entityNames[index]).attr("radentityname").trim()) == -1)
    //        self.AllEntitiesForHierarchy.push($(entityNames[index]).attr("radentityname").trim());
    //});

    entityNames.each(function (index) {
        if (self.AllEntitiesForHierarchy.indexOf($(entityNames[index]).attr("radentityattributeid").trim().split("|^")[0]) == -1)
            self.AllEntitiesForHierarchy.push($(entityNames[index]).attr("radentityattributeid").trim().split("|^")[0]);
    });

    RADDataPrivileges.instance.CommonDataForCreateHierarchy = {};

    $.ajax({
        url: RADDataPrivileges.instance.ServiceURL + '/GetCommonDataForCreateHierarchy',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        var commonData = responseText.d;
        for (var j = 0; j < commonData.length; j++) {
            RADDataPrivileges.instance.CommonDataForCreateHierarchy[commonData[j].Key] = commonData[j].Value;
        }
    });
}


RADDataPrivileges.prototype.OnCancelDataPrivilege = function (event) {
    var self = this;
    $(event.target).closest(".RADExistingDataPrivilegeParent").attr("isEditable", "false");
    $(event.target).closest(".RADExistingDataPrivilegeFooter").addClass("RADCreateHierachyDisplayNone");
    $(".RADDataPvgEditBtnParent").removeClass("RADCreateHierachyDisplayNone");
    $(".RADDataPvgDeleteBtnParent").removeClass("RADCreateHierachyDisplayNone");
    var dataPrivilegeElement = $(event.target).closest(".RADExistingDataPrivilegeParent");
    var privilegeName = dataPrivilegeElement.find(".RadDataPrivilegeNameText").text();
    dataPrivilegeElement.find(".RADExistingDataPrivilegebody").empty();
    var dataPrivilege = null;
    for (var i = 0; i < self.allDataPrivileges.length; i++) {
        if (self.allDataPrivileges[i].DataPrivilegeName == privilegeName) {
            dataPrivilege = self.allDataPrivileges[i];
            break;
        }
    }
    self.BindDataPrivilege(dataPrivilegeElement, dataPrivilege);
}

RADDataPrivileges.prototype.OnSaveDataPrivilege = function (event) {
    var self = this;
    var dataPrivilegeName = $(event.target).closest(".RADFuncGroupsCreateContent,.RADExistingDataPrivilegeParent")
                            .find(".RADFuncGroupNameValueCreate,.RadDataPrivilegeNameText").text().trim();
    var hierarchyName = $(event.target).closest(".RADFuncGroupsCreateContent,.RADExistingDataPrivilegeParent")
                        .find(".RADFuncGroupHierarcyNameSelectionText,.RadHierarchyNameText").text().trim();

    var dataPrivilegeInfo = {};
    dataPrivilegeInfo.DataPrivilegeName = dataPrivilegeName;
    dataPrivilegeInfo.HierarchyDetail = {};
    dataPrivilegeInfo.HierarchyDetail.HierarchyName = hierarchyName;
    dataPrivilegeInfo.HierarchyDetail.HierarchyEntitiesInfo = [];

    dataPrivilegeInfo.IsRestrictive = ($(event.target).closest(".RADExistingDataPrivilegeChild,.RADFuncGroupsCreateContent").find('input[type=radio]:checked').parent().hasClass("RADDataPrivIsPermissiveRadio") ? false : true)
    self.GetDataPrivilegeInfotoSave(dataPrivilegeInfo, event);


    var serviceURL = "";
    if ($(event.target).hasClass("RADSavePrivlegeBtn"))
        //edit case
        serviceURL = RADDataPrivileges.instance.ServiceURL + '/ModifyDataPrivilege'
    else
        serviceURL = RADDataPrivileges.instance.ServiceURL + '/SaveDataPrivilege'

    var existingDataPrivilesNames = [];
    $(".RadDataPrivilegeNameText").each(function (index) {
        existingDataPrivilesNames.push($(this).text().trim().toLowerCase());
    })

    if (RADDataPrivileges.instance.ValidateDataOnSaveDataPrivilege(event, dataPrivilegeInfo, existingDataPrivilesNames, self)) {
        $("#RADFuncGroupsMainDiv").addClass("RADDLPSaveSpinner");
        $("#RADHierarchyMainDiv").css("pointer-events", "none");
        $("#RADFuncGroupsMainDiv").css("pointer-events", "none");
        $.ajax({
            url: serviceURL,
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ DataPrivilegeInfo: JSON.stringify(dataPrivilegeInfo) })
        }).then(function (responseText) {
            $("#RADFuncGroupsMainDiv").removeClass("RADDLPSaveSpinner");
            $("#RADHierarchyMainDiv").css("pointer-events", "");
            $("#RADFuncGroupsMainDiv").css("pointer-events", "");
            if (responseText.d) {
                self.OnPageLoad();
            }
            else {
                //false case    
            }
        });
    }
}

RADDataPrivileges.prototype.ValidateDataOnSaveDataPrivilege = function (event, dataPrivilegeInfo, existingDataPrivilesNames, self) {

    var isDataValid = true;
    var messageDiv = $(event.target).closest('.RADFuncGroupsCreateContent').find('.RADFuncGroupsCreateContentMessage');
    messageDiv.text("");
    messageDiv.addClass("RADCreateHierachyDisplayNone");

    if (dataPrivilegeInfo.DataPrivilegeName == "") {
        isDataValid = false;
        messageDiv.text("Please enter data privilege name");
    }
    else if ((existingDataPrivilesNames.indexOf(dataPrivilegeInfo.DataPrivilegeName.toLowerCase()) >= 0) && ($(event.target).hasClass("btnRADSaveFuncGroup"))) {
        isDataValid = false;
        messageDiv.text("Data privilege name with '" + dataPrivilegeInfo.DataPrivilegeName + "' already exists");
    }
    else if (dataPrivilegeInfo.Roles.length == 0) {
        isDataValid = false;
        messageDiv.text("Please select roles");
    }
    else if (dataPrivilegeInfo.Groups.length == 0) {
        isDataValid = false;
        messageDiv.text("Please select groups");
    }
    else if (self.IsValid == false) {
        isDataValid = false;
        messageDiv.text("Please select attribute values of entities");
    }
    if (isDataValid == false)
        messageDiv.removeClass("RADCreateHierachyDisplayNone");
    else {
        messageDiv.text("");
        messageDiv.addClass("RADCreateHierachyDisplayNone");
    }
    return isDataValid;
}

RADDataPrivileges.prototype.GetDataPrivilegeInfotoSave = function (dataPrivilegeInfo, event) {
    var self = this;
    var currentHierarchy = $(event.target).closest(".RADFuncGroupsCreateContent,.RADExistingDataPrivilegeParent")
                                                  .find(".RADFuncGroupHierarcyNameSelectionText,.RadHierarchyNameText").text().trim();

    var currentHierarchyDetails = $.grep(RADDataPrivileges.instance.AllHierarchies, function (e)
    { return e.HierarchyName == currentHierarchy; });

    $(event.target).closest(".RADFuncGroupsCreateContentChild,.RADExistingDataPrivilegeParent").find(".RADFuncGroupsCreateBodyMain,.RADExistingDataPrivilegeChild").find("div[commonattrs]").each(function (index) {
        if ($(this).attr("radentityattributename").toLowerCase() == "users") {

            if ($(this).closest(".RADFuncGroupCreateEntityContent,.RADExistingDataPrivilegeContent")
                                      .find(".RADFuncGroupCreateEntitySelectText,.RADExistingDataPrivilegeEntitySelect")
                                      .attr("radselectedvalues") != null) {
                dataPrivilegeInfo.Users = $(this).closest(".RADFuncGroupCreateEntityContent,.RADExistingDataPrivilegeContent")
                                          .find(".RADFuncGroupCreateEntitySelectText,.RADExistingDataPrivilegeEntitySelect")
                                          .attr("radselectedvalues").split("|^");
                var index = dataPrivilegeInfo.Users.indexOf("");
                if (index >= 0) {
                    dataPrivilegeInfo.Users.splice(index, 1);
                }
            }
            else {
                dataPrivilegeInfo.Users = [];
            }
        }
        else if ($(this).attr("radentityattributename").toLowerCase() == "groups") {

            if ($(this).closest(".RADFuncGroupCreateEntityContent,.RADExistingDataPrivilegeContent")
                                       .find(".RADFuncGroupCreateEntitySelectText,.RADExistingDataPrivilegeEntitySelect")
                                       .attr("radselectedvalues") != null) {
                dataPrivilegeInfo.Groups = $(this).closest(".RADFuncGroupCreateEntityContent,.RADExistingDataPrivilegeContent")
                                           .find(".RADFuncGroupCreateEntitySelectText,.RADExistingDataPrivilegeEntitySelect")
                                           .attr("radselectedvalues").split("|^");
                var index = dataPrivilegeInfo.Groups.indexOf("");
                if (index >= 0) {
                    dataPrivilegeInfo.Groups.splice(index, 1);
                }
            }
            else {
                dataPrivilegeInfo.Groups = [];
            }
        }
        else if ($(this).attr("radentityattributename").toLowerCase() == "roles") {
            if ($(this).closest(".RADFuncGroupCreateEntityContent,.RADExistingDataPrivilegeContent")
                                                     .find(".RADFuncGroupCreateEntitySelectText,.RADExistingDataPrivilegeEntitySelect")
                                                     .attr("radselectedvalues") != null) {
                dataPrivilegeInfo.Roles = $(this).closest(".RADFuncGroupCreateEntityContent,.RADExistingDataPrivilegeContent")
                                                     .find(".RADFuncGroupCreateEntitySelectText,.RADExistingDataPrivilegeEntitySelect")
                                                     .attr("radselectedvalues").split("|^");
                var index = dataPrivilegeInfo.Roles.indexOf("");
                if (index >= 0) {
                    dataPrivilegeInfo.Roles.splice(index, 1);
                }
            }
            else {
                dataPrivilegeInfo.Roles = [];
            }
        }
    })

    var moduleControl = $(event.target).closest(".RADFuncGroupsCreateContent,.RADExistingDataPrivilegeParent").find("div[radentityattributename='Modules']");
    var selectedModulesText = moduleControl.attr("radselectedvalues");

    if (selectedModulesText != null) {
        var selectedModules = selectedModulesText.split("|^");
        var index = selectedModules.indexOf("");
        if (index >= 0) {
            selectedModules.splice(index, 1);
        }
        dataPrivilegeInfo.Modules = selectedModules;
    }
    else {
        dataPrivilegeInfo.Modules = [];
        dataPrivilegeInfo.Modules.push("ALL");
        //dataPrivilegeInfo.Modules = currentHierarchyDetails[0].Modules;
    }
    var entityNames = $(".RADFuncGroupCreateEntityContent").find("div[radentityname]");

    for (var i = 0; i < self.AllEntitiesForHierarchy.length; i++) {
        var EntityTypeInfo = {};
        EntityTypeInfo.EntityTypeId = self.AllEntitiesForHierarchy[i];

        var currentEntityInfo = $.grep(currentHierarchyDetails[0].HierarchyEntitiesInfo, function (e)
        { return e.EntityTypeId == EntityTypeInfo.EntityTypeId; });
        EntityTypeInfo.EntityTypeName = currentEntityInfo[0].EntityTypeName;
        EntityTypeInfo.ApiSource = currentEntityInfo[0].ApiSource;
        //EntityTypeInfo.APiPath = apiInfo.APiPath;
        EntityTypeInfo.AttributesInfo = [];
        if ($(event.target).hasClass("RADSavePrivlegeBtn")) {
            //Edit Case
            RADDataPrivileges.instance.getHierarchyInfoForFuncGroupEdit(EntityTypeInfo, event);
        }
        else {
            self.getHierarchyInfoForFuncGroup(EntityTypeInfo);
        }
        dataPrivilegeInfo.HierarchyDetail.HierarchyEntitiesInfo.push(EntityTypeInfo);
    }
}

RADDataPrivileges.prototype.getHierarchyInfoForFuncGroupEdit = function (EntityTypeInfo, event) {
    var self = this;
    var entityAttrInfo = $(event.target).closest(".RADExistingDataPrivilegeParent").find("div[radentityname='" + EntityTypeInfo.EntityTypeName + "']");
    var AttributesInfo = {};
    entityAttrInfo.each(function (index) {
        AttributesInfo = {};
        AttributesInfo.AttributeId = $(entityAttrInfo[index]).attr("radentityattributeid").split("|^")[1];
        AttributesInfo.AttributeName = $(entityAttrInfo[index]).attr("radentityattributename").split("|^")[1];
        AttributesInfo.AttributeDisplayName = $(entityAttrInfo[index]).text().trim();
        AttributesInfo.IsLookUp = ($(entityAttrInfo[index]).attr("isLookUp") == 'true');

        AttributesInfo.AttributeValues = {};      
        self.IsValid = true;

        if (AttributesInfo.IsLookUp) {
            AttributesInfo.LookupEntity = {};
            AttributesInfo.LookupEntity.Attribute = $(entityAttrInfo[index]).attr("parentAttribute")
            AttributesInfo.LookupEntity.EntityType = $(entityAttrInfo[index]).attr("parentEntity")
        }
        if ($(entityAttrInfo[index]).closest(".RADExistingDataPrivilegeContent")
                             .find(".RADExistingDataPrivilegeEntitySelect").attr("radselectedvalues") != null) {
            var selectedValues = $(entityAttrInfo[index]).closest(".RADExistingDataPrivilegeContent")
                                 .find(".RADExistingDataPrivilegeEntitySelect").attr("radselectedvalues");

            if (selectedValues == "ALL" || selectedValues == "ALL|^") {
                AttributesInfo.AttributeValues["ALL"] = "ALL";
            }
            else {
                var attrValues = selectedValues.split("|^");
                if (attrValues.indexOf("") >= 0)
                    attrValues.splice(attrValues.indexOf(""), 1);
                attrValues.forEach(function (currentItem) {
                    var currentDimensionItem = currentItem;
                    var currentDimensionKeyValue = currentDimensionItem.split("~");
                    AttributesInfo.AttributeValues[currentDimensionKeyValue[0]] = currentDimensionKeyValue[1];
                });
            }           
        }
        EntityTypeInfo.AttributesInfo.push(AttributesInfo); 
    });
}

RADDataPrivileges.prototype.getHierarchyInfoForFuncGroup = function (EntityTypeInfo) {
    var self = this;
    var entityAttrInfo = $(".RADFuncGroupCreateEntityContent").find("div[radentityname='" + EntityTypeInfo.EntityTypeName + "']");
    var AttributesInfo = {};
    entityAttrInfo.each(function (index) {
        AttributesInfo = {};
        AttributesInfo.AttributeId = $(this).closest(".RADFuncGroupCreateEntityContent").find(".RADFuncGroupCreateEntityName").attr("radentityattributeid").split("|^")[1];
        AttributesInfo.AttributeName = $(this).closest(".RADFuncGroupCreateEntityContent").find(".RADFuncGroupCreateEntityName").attr("radentityattributename").split("|^")[1];
        AttributesInfo.AttributeDisplayName = $(this).closest(".RADFuncGroupCreateEntityContent").find(".RADFuncGroupCreateEntityName").text().trim();
        AttributesInfo.IsLookUp = ($(this).attr("isLookUp") == 'true');

        AttributesInfo.AttributeValues = {};
        self.IsValid = true;
        if (AttributesInfo.IsLookUp) {            
            AttributesInfo.LookupEntity = {};
            AttributesInfo.LookupEntity.Attribute = $(this).closest(".RADFuncGroupCreateEntityContent").find(".RADFuncGroupCreateEntityName").attr("parentAttribute")
            AttributesInfo.LookupEntity.EntityType = $(this).closest(".RADFuncGroupCreateEntityContent").find(".RADFuncGroupCreateEntityName").attr("parentEntity");
        }       

        if ($(this).closest(".RADFuncGroupCreateEntityContent").find(".RADFuncGroupCreateEntitySelectText").attr("radselectedvalues") != null) {
            var selectedValues = $(this).closest(".RADFuncGroupCreateEntityContent").find(".RADFuncGroupCreateEntitySelectText").attr("radselectedvalues");
            if (selectedValues == "ALL" || selectedValues == "ALL|^") {
                AttributesInfo.AttributeValues["ALL"] = "ALL";
            }
            else {
                var attrValues = selectedValues.split("|^");
                if (attrValues.indexOf("") >= 0)
                    attrValues.splice(attrValues.indexOf(""), 1);
                attrValues.forEach(function (currentItem) {
                    var currentDimensionItem = currentItem;
                    var currentDimensionKeyValue = currentDimensionItem.split("~");
                    AttributesInfo.AttributeValues[currentDimensionKeyValue[0]] = currentDimensionKeyValue[1];
                });
            }
        }
        EntityTypeInfo.AttributesInfo.push(AttributesInfo);
    });
}

//GetAllDataPrivilegesMethod
RADDataPrivileges.prototype.GetAllDataPrivileges = function () {
    var self = this;
    $.ajax({
        url: RADDataPrivileges.instance.ServiceURL + '/GetAllDataPrivileges',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        var dataPrivileges = $.parseJSON(responseText.d);
        if (dataPrivileges.length > 0) {
            self.allDataPrivileges = dataPrivileges;
            self.BindAllDataPrivileges(dataPrivileges);
            self.showHideEditDeletePrivilegeButtons();
            self.bindQTip();
            $(".RADExistingDataPrivileges").unbind(".click").click(function (event) {
                self.existingDataPrivilegeCLickHandler(event);
            });
        }
        else {
            RADDataPrivileges.instance.OnAddFunctionalGroup();

        }
    })
}


RADDataPrivileges.prototype.showHideEditDeletePrivilegeButtons = function () {
    var self = this;
    if (self.Privileges.indexOf("Update Data Privilege") == -1) {
        $(".RADDataPvgEditBtnParent").addClass("RADCreateHierachyDisplayNone");
    }
    else {
        $(".RADDataPvgEditBtnParent").removeClass("RADCreateHierachyDisplayNone");
    }
    if (self.Privileges.indexOf("Delete Data Privilege") == -1) {
        $(".RADDataPvgDeleteBtnParent").addClass("RADCreateHierachyDisplayNone");
    }
    else {
        $(".RADDataPvgDeleteBtnParent").removeClass("RADCreateHierachyDisplayNone");
    }
}

RADDataPrivileges.prototype.bindQTip = function () {
    var self = this;
    $(".RADNMoreClass").each(function () {
        var data = [];
        if ($(this).parent().attr("radselectedvalues") != null)
            data = $(this).parent().attr("radselectedvalues").split("|^");
        else
            data = $(this).parent().attr("radselectedvalues").split("|^");
        var visisbleData = $(this).parent().clone().children().remove().end().text().trim().split(",");
        var parentDiv = $("<div>", {
            class: 'RADQtipParent'
        })

        for (var i = 0; i < data.length; i++) {
            if (visisbleData.indexOf(data[i]) == -1) {
                var dataTextArray = data[i].split('~');
                var parentEle = $("<div>", {
                    class: 'RADqtipParent'
                });
                var childEle = $("<div>", {
                    class: 'RADqtipChild',
                    text: dataTextArray[1]
                });
                parentEle.append(childEle);
                parentDiv.append(parentEle);
            }
        }
        $(this).qtip({                 // run tag popup
            content: parentDiv,
            show: 'mouseover',
            overwrite: false,
            //hide: 'mouseout',
            style: {
                classes: 'qtip-light qtip-shadow'
            }, position: {
                my: 'top center',  // Position my top left...
                at: 'bottom center'  // at the bottom right of...
            }
        });
    });
}

RADDataPrivileges.prototype.BindAllDataPrivileges = function (dataPrivileges) {
    var self = this;
    var parentElement = $(".RADExistingDataPrivileges");
    for (var i = 0; i < dataPrivileges.length; i++) {
        var dataPrivilegeElement = null;
        var dataPrivilegeElement = $(".RADExistingDataPrivilegesTemplate").find(".RADExistingDataPrivilegeParent").clone();
        parentElement.append(dataPrivilegeElement);
        self.BindDataPrivilege(dataPrivilegeElement, dataPrivileges[i])
    }
}

RADDataPrivileges.prototype.GetModuleIdsByNames = function (moduleNames) {
    var moduleIds = [];
    var moduleIdsText = "";

    if (moduleNames.length == 1 && (moduleNames[0] == "ALL|^" || moduleNames[0] == "ALL"))
        return "ALL";

    moduleNames.forEach(function (currentModule) {
        var currentModuleInfo = $.grep(RADDataPrivileges.instance.AllModulesInfo, function (e)
        { return e.ModuleName == currentModule; });
        moduleIds.push(currentModuleInfo[0].ModuleId);
    });
    moduleIdsText = moduleIds.join("|^");
    return moduleIdsText;

}
RADDataPrivileges.prototype.BindDataPrivilege = function (dataPrivilegeElement, dataPrivileges) {
    var self = this;
    dataPrivilegeElement.find(".RadDataPrivilegeText").html("Data Privilege Name : <span class=\"RadDataPrivilegeNameText\">" + dataPrivileges.DataPrivilegeName + "</span>");
    dataPrivilegeElement.find(".RadDataPrivilegeHierarchyText").html("Hierarchy Name : <span class=\"RadHierarchyNameText\">" + dataPrivileges.HierarchyDetail.HierarchyName + "</span>");
    dataPrivilegeElement.find(".RadDataPrivilegeModule").find(".RadDataPrivilegeModuleText").text("Modules");

    dataPrivilegeElement.find(".RadDataPrivilegeModule").find(".RadDataPrivilegeModuleValuesSelectText").text(dataPrivileges.Modules.join());
    if (dataPrivileges.Modules.length == 1 && dataPrivileges.Modules[0] == "ALL") {
        //do nothing
    }
    else {
        dataPrivilegeElement.find(".RadDataPrivilegeModule").find(".RadDataPrivilegeModuleValuesSelectText").attr("radselectedvalues", dataPrivileges.Modules.join("|^"));
        var moduleIdsAttributeText = RADDataPrivileges.instance.GetModuleIdsByNames(dataPrivileges.Modules);
        dataPrivilegeElement.find(".RadDataPrivilegeModule").find(".RadDataPrivilegeModuleValuesSelectText").attr("radselectedmoduleids", moduleIdsAttributeText);

    }

    
    dataPrivilegeElement.attr("dataPrivilegeName", dataPrivileges.DataPrivilegeName);
    dataPrivilegeElement.attr("isEditable", "false");

    if (dataPrivilegeElement.find(".RADDataPrivIsPermissiveRadio").find(".RADPermissiveClassRadio").length == 0) {
        dataPrivilegeElement.find(".RADDataPrivIsPermissiveRadio").append("<input type=\"radio\" class=\"RADPermissiveClassRadio\"  name=\"" + dataPrivileges.DataPrivilegeName + "\"/>");
        dataPrivilegeElement.find(".RADDataPrivIsRestrictiveRadio").append("<input type=\"radio\" class=\"RADPermissiveClassRadio\"  name=\"" + dataPrivileges.DataPrivilegeName + "\"/>");
    }
    $(".RADPermissiveClassRadio").css({ "pointer-events": "none" });

    if (dataPrivileges.IsRestrictive)
        dataPrivilegeElement.find('.RADDataPrivIsRestrictiveParent').find('.RADPermissiveClassRadio')[0].checked = true;
    else
        dataPrivilegeElement.find('.RADDataPrivIsPermissiveParent').find('.RADPermissiveClassRadio')[0].checked = true;
    //Groups
    var groupsprivilegeContent = $("<div>", {
        class: "RADExistingDataPrivilegeContent"
    });
    dataPrivilegeElement.find(".RADExistingDataPrivilegebody").append(groupsprivilegeContent);

    groupsprivilegeContent.append($("<div>", {
        class: "RADExistingDataPrivilegeEntityName",
        text: 'Groups'
    }));
    groupsprivilegeContent.find(".RADExistingDataPrivilegeEntityName").attr("commonattrs", "true");
    groupsprivilegeContent.find(".RADExistingDataPrivilegeEntityName").attr("radentityattributename", "Groups");

    groupsprivilegeContent.append($("<div>", {
        class: "RADSelectedEntityParent"
    }));

    groupsprivilegeContent.find(".RADSelectedEntityParent").append($("<div>", {
        class: "RADFuncGroupCreateEntitySelect"
    }));

    groupsprivilegeContent.find(".RADFuncGroupCreateEntitySelect").append($("<div>", {
        class: "RADExistingDataPrivilegeEntitySelect",
        html: (self.getUsersText(dataPrivileges.Groups))
    }));
    groupsprivilegeContent.find(".RADExistingDataPrivilegeEntitySelect").attr("radentityattributenametext", "Groups")
    groupsprivilegeContent.find(".RADExistingDataPrivilegeEntitySelect").attr("radselectedvalues", self.getUsersAttributeText(dataPrivileges.Groups));
    //Roles
    var rolesprivilegeContent = $("<div>", {
        class: "RADExistingDataPrivilegeContent"
    });
    dataPrivilegeElement.find(".RADExistingDataPrivilegebody").append(rolesprivilegeContent);

    rolesprivilegeContent.append($("<div>", {
        class: "RADExistingDataPrivilegeEntityName",
        text: 'Roles'
    }));
    rolesprivilegeContent.find(".RADExistingDataPrivilegeEntityName").attr("commonattrs", "true");
    rolesprivilegeContent.find(".RADExistingDataPrivilegeEntityName").attr("radentityattributename", "Roles");


    rolesprivilegeContent.append($("<div>", {
        class: "RADSelectedEntityParent"
    }));

    rolesprivilegeContent.find(".RADSelectedEntityParent").append($("<div>", {
        class: "RADFuncGroupCreateEntitySelect"
    }));

    rolesprivilegeContent.find(".RADFuncGroupCreateEntitySelect").append($("<div>", {
        class: "RADExistingDataPrivilegeEntitySelect",
        html: (self.getUsersText(dataPrivileges.Roles))
    }));
    rolesprivilegeContent.find(".RADExistingDataPrivilegeEntitySelect").attr("radentityattributenametext", "Roles")
    rolesprivilegeContent.find(".RADExistingDataPrivilegeEntitySelect").attr("radselectedvalues", self.getUsersAttributeText(dataPrivileges.Roles));


    self.BindHierarchyEntries(dataPrivileges, dataPrivilegeElement);
}

RADDataPrivileges.prototype.getUsersText = function (data) {
    if (data.length > 2)
        return data[0] + ',' + data[1] + " <span class='RADNMoreClass'>" + (data.length - 2) + " More </span>";
    else {
        var dataText = ""
        for (var i = 0; i < data.length; i++) {
            if (i == data.length - 1)
                dataText += data[i];
            else
                dataText += data[i] + ",";
        }
        return dataText;
    }
}

RADDataPrivileges.prototype.getUsersAttributeText = function (data) {
    var dataText = ""
    for (var i = 0 ; i < data.length; i++) {
        dataText += data[i] + "|^";
    }
    return dataText;
}

RADDataPrivileges.prototype.getEntityText = function (data, islookup) {
    var count = 0;
    var text = "";
    if (Object.keys(data.AttributeValues).length == 1 && data.AttributeValues["ALL"] != null) {
        // this is the case where either unique or lookup is selected explicitly as select all from dropdown
        text = "ALL";
        return text;
    }
    else {
        //this is the case where either some values of unique or lookup is selected in dropdwon
        for (var o in data.AttributeValues) {
            if (data.AttributeValues.hasOwnProperty(o)) {
                count++;
                if (count > 2) {
                    text = text.substr(0, text.length - 1);
                    text += " <span class='RADNMoreClass'>" + (Object.keys(data.AttributeValues).length - 2) + " More </span>";
                    break;
                } else
                    text += data.AttributeValues[o] + ",";
            }
        }
    }
    if (text.endsWith(","))
        text = text.substr(0, text.length - 1);
    return text;
}

RADDataPrivileges.prototype.getEntityAttributeText = function (data, islookup) {

    var text = "";
    for (var o in data.AttributeValues) {
        if (data.AttributeValues.hasOwnProperty(o)) {
            text += o + "~" + data.AttributeValues[o] + "|^";
        }
    }   
    return text;
}

RADDataPrivileges.prototype.BindHierarchyEntries = function (dataPrivileges, dataPrivilegeElement) {

    var self = this;

    for (var j = 0; j < dataPrivileges.HierarchyDetail.HierarchyEntitiesInfo.length; j++) {
        for (var i = 0; i < dataPrivileges.HierarchyDetail.HierarchyEntitiesInfo[j].AttributesInfo.length; i++) {

            var attrInfo = dataPrivileges.HierarchyDetail.HierarchyEntitiesInfo[j].AttributesInfo[i];
            var privilegeContent = $("<div>", {
                class: "RADExistingDataPrivilegeContent"
            });
            dataPrivilegeElement.find(".RADExistingDataPrivilegebody").append(privilegeContent);

            privilegeContent.append($("<div>", {
                class: "RADExistingDataPrivilegeEntityName",
                text: dataPrivileges.HierarchyDetail.HierarchyEntitiesInfo[j].AttributesInfo[i].AttributeDisplayName
            }));
            privilegeContent.find(".RADExistingDataPrivilegeEntityName").attr("title", dataPrivileges.HierarchyDetail.HierarchyEntitiesInfo[j].EntityTypeName
                                                                                      + "_" + attrInfo.AttributeName)
            privilegeContent.find(".RADExistingDataPrivilegeEntityName").attr("radentityname", dataPrivileges.HierarchyDetail.HierarchyEntitiesInfo[j].EntityTypeName);
            privilegeContent.find(".RADExistingDataPrivilegeEntityName").attr("isLookUp", attrInfo.IsLookUp);
            privilegeContent.find(".RADExistingDataPrivilegeEntityName").attr("isUnique", attrInfo.isUnique);
            privilegeContent.find(".RADExistingDataPrivilegeEntityName").attr("radmappedattribute", attrInfo.MappedAttributeName);
            privilegeContent.find(".RADExistingDataPrivilegeEntityName").attr("radentityattributename", dataPrivileges.HierarchyDetail.HierarchyEntitiesInfo[j].EntityTypeName + "|^" + attrInfo.AttributeName);
            privilegeContent.find(".RADExistingDataPrivilegeEntityName").attr("radentityattributeid", dataPrivileges.HierarchyDetail.HierarchyEntitiesInfo[j].EntityTypeId + "|^" + attrInfo.AttributeId);
            if (attrInfo.IsLookUp) {
                privilegeContent.find(".RADExistingDataPrivilegeEntityName").attr("parentAttribute", attrInfo.LookupEntity.Attribute);
                privilegeContent.find(".RADExistingDataPrivilegeEntityName").attr("parentEntity", attrInfo.LookupEntity.EntityType);
            }
            privilegeContent.append($("<div>", {
                class: "RADSelectedEntityParent"
            }));

            privilegeContent.find(".RADSelectedEntityParent").append($("<div>", {
                class: "RADFuncGroupCreateEntitySelect"
            }));

            privilegeContent.find(".RADFuncGroupCreateEntitySelect").append($("<div>", {
                class: "RADExistingDataPrivilegeEntitySelect",
                html: self.getEntityText(attrInfo, attrInfo.IsLookUp)
            }));
            privilegeContent.find(".RADExistingDataPrivilegeEntitySelect").attr("radselectedvalues", self.getEntityAttributeText(attrInfo, attrInfo.IsLookUp));
            privilegeContent.find(".RADExistingDataPrivilegeEntitySelect").attr("radentityattributenametext", dataPrivileges.HierarchyDetail.HierarchyEntitiesInfo[j].EntityTypeName + "|^" + attrInfo.AttributeName);
            privilegeContent.find(".RADExistingDataPrivilegeEntitySelect").attr("radentityattributeid", dataPrivileges.HierarchyDetail.HierarchyEntitiesInfo[j].EntityTypeId + "|^" + attrInfo.AttributeId);

        }
    }
}


RADDataPrivileges.prototype.existingDataPrivilegeCLickHandler = function (event) {
    var self = this;
    if ($(event.target).hasClass("RADDataPvgDeleteBtn")) {
        self.createDeletePrivilegePopUp(event);
    }
    else if ($(event.target).hasClass("RADDeleteBtnPrivilege")) {
        var dataPrivilegeName = $(event.target).closest(".RADExistingDataPrivilegeParent").attr("dataPrivilegeName");
        self.DeleteDataPrivilege(dataPrivilegeName)
    }
}

RADDataPrivileges.prototype.DeleteDataPrivilege = function (dataPrivilegeName) {
    var self = this;
    $.ajax({
        url: RADDataPrivileges.instance.ServiceURL + '/DeleteDataPrivilege',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ DataPrivilege: dataPrivilegeName })
    }).then(function (reponseText) {
        if (reponseText.d) {
            self.OnPageLoad();
        }
    })
}

RADDataPrivileges.prototype.createDeletePrivilegePopUp = function (event) {
    $(".RADDeletePrivilege").remove();
    var dataPrivilegeName = $(event.target).closest(".RADExistingDataPrivilegeParent").attr("dataPrivilegeName");
    var deleteDiv = $('<div>', {
        class: 'RADDeletePrivilege'
    })
    deleteDiv.append($('<div>', {
        class: 'RADDeletePrivilegeText',
        text: 'Are you sure you want to Delete the Privilege'
    }));

    var deleteBtnParent = $('<div>', {
        class: 'RADDeleteBtnPrivilegeParent'
    })
    deleteDiv.append(deleteBtnParent);
    deleteBtnParent.append($('<div>', {
        class: 'RADDeleteBtnPrivilege',
        text: 'Delete'
    }));
    $(event.target).closest(".RADExistingDataPrivilegeParent").append(deleteDiv);
}

RADDataPrivileges.prototype.GetApiInfobyHierarchyName = function (hierarchyName, entityId) {

    var ApiInfo = {};

    var currentHierarchyDetails = $.grep(RADDataPrivileges.instance.AllHierarchies, function (e)
    { return e.HierarchyName == hierarchyName; });

    var currentEntityInfo = $.grep(currentHierarchyDetails[0].HierarchyEntitiesInfo, function (e)
    { return e.EntityTypeId == entityId; });

    ApiInfo["APiType"] = currentEntityInfo[0].ApiSource;

    var moduleInfoByApiSource = $.grep(RADDataPrivileges.instance.AllModulesInfo, function (e)
    { return e.APISource == currentEntityInfo[0].ApiSource; });

    ApiInfo["APiPath"] = moduleInfoByApiSource[0].APIPath;
    return ApiInfo;
}

RADDataPrivileges.prototype.SetDropDownsAttributeTextandText = function (currentControl, selectValuesArray) {
   
    var radDropDownsText = "";
    if (currentControl.text() == "ALL") {
        currentControl.text("ALL");
        currentControl.attr("radselectedvalues", "ALL");
        return;
    }
    else if (selectValuesArray.length == 0) {
        currentControl.text("--Select--");
        currentControl.attr("radselectedvalues", "");
        return;
    }
    else {
        var dimensionsValuesArray = [];
        selectValuesArray.forEach(function (currentSelectedValue) {
            dimensionsValuesArray.push(currentSelectedValue.SelectedDimensionValue);
            radDropDownsText = radDropDownsText + currentSelectedValue.SelectedDimensionId + "~"
                                                + currentSelectedValue.SelectedDimensionValue + "|^";
        });
        currentControl.attr("radselectedvalues", radDropDownsText);

        if (selectValuesArray.length <= 2) {
            currentControl.text(dimensionsValuesArray.join());
        }
        else {
            var slicedArray = dimensionsValuesArray.slice(0, 2);
            var selectedText = slicedArray.join();
            selectedText = selectedText + " <span class='RADNMoreClass'>" + (dimensionsValuesArray.length - 2).toString() + " More </span>";
            currentControl.text(selectedText);
        }

    }
}

/* Functional Group Screen Ends */


/* Hierarchy Screen  Starts*/

RADDataPrivileges.prototype.showHideHierarchyPrivileges = function () {
    var self = this;
    if (self.Privileges.indexOf("Add Hierarchy") == -1) {
        $(".RADHierarchyCreationMain").addClass("RADCreateHierachyDisplayNone");
        $(".RADHierarchyHeaderRightNew").addClass("RADCreateHierachyDisplayNone");
    }
    else {
        $(".RADHierarchyCreationMain").removeClass("RADCreateHierachyDisplayNone");
        $(".RADHierarchyHeaderRightNew").removeClass("RADCreateHierachyDisplayNone");
    }
    if (self.Privileges.indexOf("Delete Hierarchy") == -1) {
        $(".RADDataPvgDeleteBtnParent").addClass("RADCreateHierachyDisplayNone");
    }
    else {
        $(".RADDataPvgDeleteBtnParent").removeClass("RADCreateHierachyDisplayNone");
    }

}

RADDataPrivileges.prototype.GetHierarchyPageLoad = function () {
    var self = this;
    if ($("#" + RADDataPrivilegesConfig.contentBodyId).find("#RADHierarchyMainDiv").length == 0) {
        RADDataPrivileges.instance.ServiceURL = RADDataPrivilegesConfig.baseUrl + "/Resources/Services/RADUserManagement.svc";
        RADDataPrivileges.instance.AllEntityTypes = null;
        RADDataPrivileges.instance.SelectedEntiesInfo = [];

        $.ajax({
            url: RADDataPrivileges.instance.ServiceURL + "/GetTagTemplates",
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ fileName: 'RADCreateHierarchy.html' })
        }).then(function (responseText) {
            $("#" + RADDataPrivilegesConfig.contentBodyId).append(responseText.d);
            self.showHideHierarchyPrivileges();
            //RADDataPrivileges.instance.GetAllEntities();
            RADDataPrivileges.instance.AddEntityAttributeControl();
            RADDataPrivileges.instance.GetAllHierarchies();
            RADDataPrivileges.instance.BindEventsCommonHierarchy();
        });
    }
}

RADDataPrivileges.prototype.BindModuleDetaisForHierarchy = function (event, allModules) {

    var currentControl = $(event.target).closest(".RADHierarchyModuleNamesSelection");
    var textControl = currentControl.find(".RADHierarchyModuleNamesSelectionText");

    var selectedModulesArray = [];
    var currentSelectedModules = textControl.attr("radselectedvalues");
    if (currentSelectedModules != null) {
        if (currentSelectedModules == "ALL" || currentSelectedModules == "ALL|^") {
            allModules.forEach(function (currentModule) {
                selectedModulesArray.push(currentModule.ModuleName);

            });
        }
        else {
            selectedModulesArray = currentSelectedModules.split("|^");
            if (selectedModulesArray.indexOf("") >= 0)
                selectedModulesArray.splice(selectedModulesArray.indexOf(""), 1);
        }
    }

    $(".RADFunGroupModulesMainDiv").remove();

    var $modulesMainDiv = $("<div>", {
        class: "RADFunGroupModulesMainDiv"
    });
    var $currentRowModulesParent = $("<div>", {
        class: "RADModulesDropDownParent"
    });

    var $selectAllRowDiv = $("<div>", {
        class: "RADModulesDropDownChildSelectAll",
        text: "Select All"
    });

    if (selectedModulesArray.length != allModules.length)
        $selectAllRowDiv.attr("isselectedmode", "false");
    else {
        $selectAllRowDiv.attr("isselectedmode", "true");
        $selectAllRowDiv.addClass("RADModulesDropDownChildUnSelectAll");
    }

    $currentRowModulesParent.append($selectAllRowDiv);

    for (var i = 0; i < allModules.length; i++) {
        if (allModules[i] != null) {
            var $currentParent = $("<div>", {
                class: "RADModulesDropDownChildParent"
            });
            var $currentRowModule = $("<div>", {
                class: "RADModulesDropDownChild",
                text: allModules[i].ModuleName
            });

            $currentRowModule.attr("radmoduleid", allModules[i].ModuleId);

            var $currentRowModuleTick = $("<div>", {
                class: "RADCreateHierachyDisplayNone RADModulesDropDownChildTick fa fa-check"
            });
            $currentParent.append($currentRowModule);
            $currentParent.append($currentRowModuleTick);

            if (selectedModulesArray.indexOf(allModules[i].ModuleName) != -1)
                $currentRowModuleTick.removeClass("RADCreateHierachyDisplayNone");
            $currentRowModulesParent.append($currentParent);
        }
    }
    $modulesMainDiv.append($currentRowModulesParent);
    currentControl.append($modulesMainDiv);



}

RADDataPrivileges.prototype.GetAllHierarchies = function () {

    $.ajax({
        url: RADDataPrivileges.instance.ServiceURL + '/GetAllHierarchies',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {

        if (responseText.d != "") {

            var allHierarchies = JSON.parse(responseText.d);
            RADDataPrivileges.instance.BindHierarchies(allHierarchies);
        }
        else {

        }
    });
}

RADDataPrivileges.prototype.BindHierarchies = function (allHierarchies) {

    $("#RADCreatedHierarchiesMainDiv").empty();
    for (var hierarchItem in allHierarchies) {
        var currentHierachy = allHierarchies[hierarchItem];

        var $currentRowParent = $("<div>", {
            class: "RADCreatedHierarchyMain"
        });

        var $currentRowHeader = $("<div>", {
            class: "RADCreatedHierarchyHeaderParent"
        });

        var $currentRowExpand = $("<div>", {
            class: "fa fa-caret-right RADCreatedHierarchyExpandArrow"
        });

        var $currentRowHierarchyName = $("<div>", {
            class: "RADCreatedHierarchyName",
            text: "Hierarchy Name"
        });

        var $currentRowHierarchyNameValue = $("<div>", {
            class: "RADCreatedHierarchyNameValue",
            text: currentHierachy.HierarchyName
        });

        $currentRowHierarchyNameValue.attr("title", currentHierachy.HierarchyName);

        var $currentRowHierarchyModulesName = $("<div>", {
            class: "RADCreatedHierarchyName",
            text: "Modules"
        });

        var $currentRowHierarchyModulesNameValue = $("<div>", {
            class: "RADCreatedHierarchyModuleNamesValue",
            text: currentHierachy.Modules.join()
        });

        $currentRowHierarchyModulesNameValue.attr("title", currentHierachy.Modules.join());

        var $currentRowDeleteDiv = $("<div>", {
            class: "RADCreatedHierarchyNameDelete"
        });

        var $currentRowDeleteButton = $("<div>", {
            class: "fa fa-trash-o btnRADDeleteHierarchy"
        });

        if (!currentHierachy.IsDataPrivilegeExists)
            $currentRowDeleteDiv.append($currentRowDeleteButton);
        $currentRowHeader.append($currentRowExpand).append($currentRowHierarchyName).append($currentRowHierarchyNameValue)
                         .append($currentRowHierarchyModulesName).append($currentRowHierarchyModulesNameValue).append($currentRowDeleteDiv);

        var $currentRowAtributesMainDiv = $("<div>", {
            class: "RADCreatedHierarchyDimensionsMain RADCreateHierachyDisplayNone"
        });

        for (var currentEntityItem in currentHierachy.HierarchyEntitiesInfo) {

            for (var attrubiteItem in currentHierachy.HierarchyEntitiesInfo[currentEntityItem].AttributesInfo) {
                var currentAttribute = currentHierachy.HierarchyEntitiesInfo[currentEntityItem].AttributesInfo[attrubiteItem];

                var $currentRowAttributeParent = $("<div>", {
                    class: "RADDimensionParent"
                });

                var $currentRowAttributeItem = $("<div>", {
                    class: "RADDimension",
                    text: currentAttribute.AttributeDisplayName
                });

                $currentRowAttributeItem.attr("title", currentHierachy.HierarchyEntitiesInfo[currentEntityItem].EntityTypeName + "_"
                                                      + currentAttribute.AttributeName);
                $currentRowAttributeParent.append($currentRowAttributeItem);
                $currentRowAtributesMainDiv.append($currentRowAttributeParent);
            }
        }

        $currentRowParent.append($currentRowHeader).append($currentRowAtributesMainDiv);
        $("#RADCreatedHierarchiesMainDiv").append($currentRowParent);
    }
}

RADDataPrivileges.prototype.AddEntityAttributeControl = function () {

    var $currentRowParent = $("<div>", {
        class: "RADDimensionsSelectedParent"
    });

    var $selectedDimensionDiv = $("<div>", {
        class: "RADSelectedDimension"
    });

    var $selectedDimensionSelect = $("<div>", {
        class: "RADDimensionSelect"
    });

    var $selectedDimensionSelectText = $("<div>", {
        class: "RADDimensionSelectText",
        text: "--Select--"
    });

    $selectedDimensionSelect.append($selectedDimensionSelectText);
    $selectedDimensionDiv.append($selectedDimensionSelect)

    var $selectedZoneDiv = $("<div>", {
        class: "RADSelectedZone"
    });

    var $selectedZoneSelect = $("<div>", {
        class: "RADZoneSelect"
    });

    var $selectedZoneSelectText = $("<div>", {
        class: "RADZoneSelectText",
        text: "--Select--"
    });

    $selectedZoneSelect.append($selectedZoneSelectText);
    $selectedZoneDiv.append($selectedZoneSelect);

    var $selectedDimensionKeyParentDiv = $("<div>", {
        class: "RADSelectedDimensionKeyParent"
    });

    var $selectedDimensionKeyDiv = $("<div>", {
        class: "RADSelectedDimensionKey RADCreateHierachyDisplayNone"
    });

    $selectedDimensionKeyDiv.attr("contenteditable", true);
    $selectedDimensionKeyParentDiv.append($selectedDimensionKeyDiv);

    var $selectedDimensionDeleteDiv = $("<div>", {
        class: "RADSelectedDimensionDelete"
    });

    var $btnDeleteDimensionsId = $("<div>", {
        class: "fa fa-trash btnRADDeleteDimensions"
    });

    $selectedDimensionDeleteDiv.append($btnDeleteDimensionsId);
    $currentRowParent.append($selectedDimensionDiv).append($selectedZoneDiv).append($selectedDimensionKeyParentDiv).append($selectedDimensionDeleteDiv);
    $("#RADDimensionsSelectedMainDiv").append($currentRowParent);

    $(".RADDimensionSelect").unbind("click").click(function (event) {
        if ($(event.target).closest(".RADEntityDropDownChild").length > 0) {
            RADDataPrivileges.instance.SelectEntityTypeDropDown(event);
        }
        else {
            var mainparent = $(event.target).closest(".RADDimensionsSelectedParent");
            RADDataPrivileges.instance.BindEntitiesData(mainparent);
        }
    });

    $(".RADZoneSelect").unbind("click").click(function (event) {
        if ($(event.target).next().hasClass("RADCreateHierachyDisplayNone")) {
            $(event.target).next().removeClass("RADCreateHierachyDisplayNone");
        }
        else if ($(event.target).hasClass("RADAttributeDropDownChild")) {
            var isLookup = $(event.target).attr("radislookupattribute");
            //var isUnique = $(event.target).attr("radisuniqueattribute");            
            //var parentattribute = $(event.target).attr("radparentattribute");
            //var parententitytype = $(event.target).attr("radparententitytype");
            var attributeTextControl = $(event.target).closest(".RADZoneSelect").find(".RADZoneSelectText");
            attributeTextControl.text($(event.target).text());
            attributeTextControl.attr("radislookupattribute", isLookup);
            attributeTextControl.attr("radattributeid", $(event.target).attr("radattributeid"));
            if (isLookup) {
                attributeTextControl.attr("radparententitytypeid", $(event.target).attr("radparententitytypeid"));
                attributeTextControl.attr("radparentattributeid", $(event.target).attr("radparentattributeid"));
            }
            //attributeTextControl.attr("radisuniqueattribute", isUnique);
            var mainparent = $(event.target).closest(".RADDimensionsSelectedParent");
            RADDataPrivileges.instance.AttributeSelectionChanged(mainparent);
        }
    })

    $btnDeleteDimensionsId.unbind("click").click(function (event) {
        RADDataPrivileges.instance.DeleteCurrentEntitySelection(event);
    });
}

RADDataPrivileges.prototype.DeleteCurrentEntitySelection = function (event) {

    /*
    var isAddedEntityAttribuetControl = ($("#RADDimensionsSelectedMainDiv").find(".RADDimensionsSelectedParent").length == 1) ? true : false;
    if ($(event.target).parent().parent().find(".RADSelectedDimensionKey").attr("radentityattributevalue") != null) {

        var entityAttributeValue = $(event.target).parent().parent().find(".RADSelectedDimensionKey").attr("radentityattributevalue").split("|^");
        var entityName = entityAttributeValue[0];
        var isLookup = $(event.target).parent().parent().find(".RADSelectedDimensionKey").attr("radislookupattribute");
        var isUnique = $(event.target).parent().parent().find(".RADSelectedDimensionKey").attr("radisuniqueattribute");

        var existingSelectedInfo = $.grep(RADDataPrivileges.instance.SelectedEntiesInfo, function (e)
        { return e.EntityName == entityName; });

        if (existingSelectedInfo[0].IsLookUpSelected == true && isLookup == "true")
            existingSelectedInfo[0].IsLookUpSelected = false;
        if (existingSelectedInfo[0].IsUniqueSelected == true && isUnique == "true")
            existingSelectedInfo[0].IsUniqueSelected = false;

        if (existingSelectedInfo[0].IsLookUpSelected == false && existingSelectedInfo[0].IsUniqueSelected == false) {
            var index = RADDataPrivileges.instance.SelectedEntiesInfo.indexOf(existingSelectedInfo[0]);
            RADDataPrivileges.instance.SelectedEntiesInfo.splice(index, 1);
        }

    }

    $(event.target).parent().parent().remove();
    if (isAddedEntityAttribuetControl)
        RADDataPrivileges.instance.AddEntityAttributeControl();
        */

    var isAddedEntityAttribuetControl = ($("#RADDimensionsSelectedMainDiv").find(".RADDimensionsSelectedParent").length == 1) ? true : false;
    $(event.target).parent().parent().remove();
    if (isAddedEntityAttribuetControl)
        RADDataPrivileges.instance.AddEntityAttributeControl();


}

RADDataPrivileges.prototype.GetAttributeForEntity = function (selectedEntityTypeId, mainParent) {    

    var selectedEntityInfo = $.grep(RADDataPrivileges.instance.AllEntityTypes, function (e)
    { return e.EntityTypeId == selectedEntityTypeId; });
    var currentControl = mainParent.find(".RADZoneSelect");
    RADDataPrivileges.instance.BindAttributesData(currentControl, selectedEntityInfo[0]);

}

RADDataPrivileges.prototype.GetAllSelectedAttibutesDisplayName = function () {

    var attributeDisplayNameControls = $(".RADSelectedDimensionKey");
    var attributeDisplayNames = [];

    for (var i = 0; i < attributeDisplayNameControls.length; i++) {
        var currentControl = attributeDisplayNameControls[i];
        if ($(currentControl).attr("radentityattributevalue") != null)
            attributeDisplayNames.push($(currentControl).attr("radentityattributevalue"));
    }
    return attributeDisplayNames;
}

RADDataPrivileges.prototype.BindAttributesData = function (currentControl, currentEntityInfo) {   

    var $attributesMainDiv = $("<div>", {
        class: "RADAttributesMainDiv RADCreateHierachyDisplayNone"
    });

    var $currentRowattributesParent = $("<div>", {
        class: "RADAttributesDropDownParent"
    });

    var currentEntityName = currentEntityInfo.EntityTypeName;
    var currentAttrubutesInfo = currentEntityInfo.AttributesInfo;

    for (var i = 0; i < currentAttrubutesInfo.length; i++) {

        var $currentRowAttribute = $("<div>", {
            class: "RADAttributeDropDownChild",
            text: currentAttrubutesInfo[i].AttributeName
        });
        $currentRowAttribute.attr("radattributeid", currentAttrubutesInfo[i].AttributeId);
        $currentRowAttribute.attr("radislookupattribute", currentAttrubutesInfo[i].IsLookUp);
        if (currentAttrubutesInfo[i].IsLookUp && currentAttrubutesInfo[i].LookupEntity != null) {
            $currentRowAttribute.attr("radparententitytypeid",
                (currentAttrubutesInfo[i].LookupEntity.EntityType == null) ? "" : currentAttrubutesInfo[i].LookupEntity.EntityType);
            $currentRowAttribute.attr("radparentattributeid",
                (currentAttrubutesInfo[i].LookupEntity.Attribute == null) ? "" : currentAttrubutesInfo[i].LookupEntity.Attribute);
        }
        $currentRowattributesParent.append($currentRowAttribute);  
    }

    $attributesMainDiv.append($currentRowattributesParent);
    currentControl.append($attributesMainDiv);
}

RADDataPrivileges.prototype.BuildSelectedEntityInfo = function (entityName) {

    var existingSelectedInfo = $.grep(RADDataPrivileges.instance.SelectedEntiesInfo, function (e)
    { return e.EntityName == entityName; });

    if (existingSelectedInfo != null && existingSelectedInfo.length > 0) {

    }
    else {
        var selectedEntityInfo = {};
        selectedEntityInfo.EntityName = entityName;
        //by default making it as false
        selectedEntityInfo.IsLookUpSelected = false;
        selectedEntityInfo.IsUniqueSelected = false;
        RADDataPrivileges.instance.SelectedEntiesInfo.push(selectedEntityInfo);
    }
}

RADDataPrivileges.prototype.AttributeSelectionChanged = function (event) {

    $(".RADEntitiesMainDiv").remove();
    var attributeId = event.find(".RADZoneSelectText").attr("radattributeid");
    var attributetext = event.find(".RADZoneSelectText").text().trim();
    var islookupattribute = event.find(".RADZoneSelectText").attr("radislookupattribute");
    //var isuniqueattribute = event.find(".RADZoneSelectText").attr("radisuniqueattribute");
    //var parentattribute = event.find(".RADZoneSelectText").attr("radparentattribute");
    //var parententitytype = event.find(".RADZoneSelectText").attr("radparententitytype");
    //var dimensionText = event.find(".RADDimensionSelectText").text().trim();
    var entityTypeId = event.find(".RADDimensionSelectText").attr("radentitytypeid");
    var entityTypeName = event.find(".RADDimensionSelectText").text().trim();
    var entityApiSource = event.find(".RADDimensionSelectText").attr("radentityapisource");


    if ((attributetext != "--Select--" && attributetext != "") && (entityTypeName != "--Select--" && entityTypeName != "")) {
        event.find(".RADSelectedDimensionKey").removeClass("RADCreateHierachyDisplayNone");
        event.find(".RADSelectedDimensionKey").text(entityTypeName + "-" + attributetext);
        event.find(".RADSelectedDimensionKey").attr("tabindex", 1);
        event.find(".RADSelectedDimensionKey").focus();
        event.find(".RADSelectedDimensionKey").attr("title", entityTypeName + "_" + attributetext);
        event.find(".RADSelectedDimensionKey").attr("radentityattributevalue", entityTypeName + "|^" + attributetext);
        event.find(".RADSelectedDimensionKey").attr("radentityattributeid", entityTypeId + "|^" + attributeId);
        event.find(".RADSelectedDimensionKey").attr("radislookupattribute", islookupattribute);
        if (islookupattribute == "true") {
            event.find(".RADSelectedDimensionKey").attr("radparentattributeid", event.find(".RADZoneSelectText").attr("radparentattributeid"));
            event.find(".RADSelectedDimensionKey").attr("radparententitytypeid", event.find(".RADZoneSelectText").attr("radparententitytypeid"));
        }
        //event.find(".RADSelectedDimensionKey").attr("radisuniqueattribute", isuniqueattribute);
        
        event.find(".RADSelectedDimensionKey").attr("radentityapisource", entityApiSource);
    }

    //var existingSelectedInfo = $.grep(RADDataPrivileges.instance.SelectedEntiesInfo, function (e)
    //{ return e.EntityName == dimensionText; });

    //if (existingSelectedInfo != null && existingSelectedInfo.length > 0) {
    //    if (existingSelectedInfo[0].IsLookUpSelected == false && islookupattribute == "true")
    //        existingSelectedInfo[0].IsLookUpSelected = true;

    //    if (existingSelectedInfo[0].IsUniqueSelected == false && isuniqueattribute == "true")
    //        existingSelectedInfo[0].IsUniqueSelected = true;
    //}
    event.find(".RADAttributesMainDiv").addClass("RADCreateHierachyDisplayNone");

}

RADDataPrivileges.prototype.BindEntitiesData = function (currentControl) {
    $(currentControl).addClass("RADDLPHierddSpinner");
    $("#RADHierarchyMainDiv").css("pointer-events", "none");
    $("#RADFuncGroupsMainDiv").css("pointer-events", "none");
    var selectedModuleInfo = RADDataPrivileges.instance.GetSelectedModulesApiSource(currentControl);
    var selectedModuleInfoString = JSON.stringify(selectedModuleInfo);

    $.ajax({
        url: RADDataPrivileges.instance.ServiceURL + '/GetEntityTypesInfo',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ allModuleApiMap: selectedModuleInfoString })
    }).then(function (responseText) {
        $(currentControl).removeClass("RADDLPHierddSpinner");
        $("#RADHierarchyMainDiv").css("pointer-events", "");
        $("#RADFuncGroupsMainDiv").css("pointer-events", "");
        if (responseText.d != null && responseText.d != "") {
            RADDataPrivileges.instance.AllEntityTypes =  JSON.parse(responseText.d);

            $(".RADEntitiesMainDiv").remove();

            var $entitiesMainDiv = $("<div>", {
                class: "RADEntitiesMainDiv"
            });

            var $currentRowEntityParent = $("<div>", {
                class: "RADEntityDropDownParent"
            });

            for (var i = 0; i < RADDataPrivileges.instance.AllEntityTypes.length; i++) {

                var $currentRowEntity = $("<div>", {
                    class: "RADEntityDropDownChild",
                    text: RADDataPrivileges.instance.AllEntityTypes[i].EntityTypeName,
                    title: RADDataPrivileges.instance.AllEntityTypes[i].EntityTypeName
                });
                $currentRowEntity.attr("radentitytypeid", RADDataPrivileges.instance.AllEntityTypes[i].EntityTypeId);
                $currentRowEntity.attr("radentityapisource", RADDataPrivileges.instance.AllEntityTypes[i].ApiSource);
                $currentRowEntityParent.append($currentRowEntity);
            }
            $entitiesMainDiv.append($currentRowEntityParent);
            currentControl.find(".RADDimensionSelect").append($entitiesMainDiv);
        }
    });
}

RADDataPrivileges.prototype.GetSelectedModulesApiSource = function (currentControl) {
    var selectedModulesInfo = [];
    if (currentControl.closest(".RADHierarchyMainNew").find(".RADHierarchyModuleNamesSelectionText").attr("radselectedvalues") != null) {
        var selectedModuleIds = currentControl.closest(".RADHierarchyMainNew").find(".RADHierarchyModuleNamesSelectionText").attr("radselectedmoduleids");
        var selectedModuleIdsArray = selectedModuleIds.split("|^");
        if (selectedModuleIdsArray.indexOf("") >= 0)
            selectedModuleIdsArray.splice(selectedModuleIdsArray.indexOf(""), 1);
        selectedModuleIdsArray.forEach(function (currentModuleId) {
            var currentModuleInfo = $.grep(RADDataPrivileges.instance.AllModulesInfo, function (e)
            { return e.ModuleId == currentModuleId; });
            selectedModulesInfo.push(currentModuleInfo[0]);
        });
        return selectedModulesInfo;
    }
    else {
        var messageDiv = $(event.target).closest(".RADHierarchyMainNew").find(".RADHierarchyHeaderMessage");
        messageDiv.text("Please select modules");
        messageDiv.removeClass("RADCreateHierachyDisplayNone");
    }    
}

RADDataPrivileges.prototype.SelectEntityTypeDropDown = function (event) {
    var selectedText = $(event.target);
    var mainParent = $(event.target).closest(".RADDimensionsSelectedParent");
    var selectedEntityName = $(event.target).text().trim();
    var radEntityApiSource = $(event.target).attr("radentityapisource");
    var selectedEntityTypeId = $(event.target).attr("radentitytypeid");
    mainParent.find(".RADDimensionSelectText").text(selectedEntityName);
    mainParent.find(".RADDimensionSelectText").attr("radentityapisource", radEntityApiSource);
    mainParent.find(".RADDimensionSelectText").attr("radentitytypeid", selectedEntityTypeId);
    mainParent.find(".RADZoneSelectText").text("--Select--");
    mainParent.find(".RADEntitiesMainDiv").remove();
    mainParent.find(".RADAttributesMainDiv").remove();
    RADDataPrivileges.instance.GetAttributeForEntity(selectedEntityTypeId, mainParent);
}

RADDataPrivileges.prototype.BindEventsCommonHierarchy = function () {
    $("#RADHierarchyMainDiv").unbind("click").click(function (event) {
        RADDataPrivileges.instance.SelectFunctionByEventType(event);
    });
}

RADDataPrivileges.prototype.SelectFunctionByEventType = function (event) {

    if ($(event.target).closest(".btnRADDeleteHierarchy").length > 0) {
        RADDataPrivileges.instance.OnDeleteHierarchy(event);
    }

    else if ($(event.target).closest(".RADCreatedHierarchyHeaderParent").length > 0) {
        RADDataPrivileges.instance.ShowHideDimesions(event);
    }

    else if ($(event.target).closest(".btnRADAddDimensions").length > 0) {
        RADDataPrivileges.instance.AddEntityAttributeControl();
    }

    else if ($(event.target).closest(".btnRADSaveHierarchy").length > 0) {
        RADDataPrivileges.instance.SelectedEntiesInfo = [];
        RADDataPrivileges.instance.OnCreateHierarchy(event);
    }
    else if ($(event.target).closest(".RADHierarchyModuleNamesSelectionText").length > 0) {
        RADDataPrivileges.instance.BindModuleDetaisForHierarchy(event, RADDataPrivileges.instance.AllModulesInfo);
    }
    else if ($(event.target).closest(".RADModulesDropDownChild,.RADModulesDropDownChildSelectAll,.RADModulesDropDownChildUnSelectAll").length > 0) {
        var messageDiv = $(event.target).closest(".RADHierarchyMainNew").find(".RADHierarchyHeaderMessage");
        messageDiv.text("");
        messageDiv.addClass("RADCreateHierachyDisplayNone");
        RADDataPrivileges.instance.SelectHierachyModulesDropDown(event);
    }
    else if ($(event.target).closest(".RADDimensionSelectText").length == 0 && $(event.target).closest(".RADZoneSelectText").length == 0) {
        $(".RADEntitiesMainDiv").remove();
        $(".RADAttributesMainDiv").addClass("RADCreateHierachyDisplayNone");
        $(".RADFunGroupModulesMainDiv").remove();

    }

}

RADDataPrivileges.prototype.OnDeleteHierarchy = function (event) {

    var hierarchyToDelete = $(event.target).parent().parent().find(".RADCreatedHierarchyNameValue").text();

    $.ajax({
        url: RADDataPrivileges.instance.ServiceURL + '/DeleteHierarchy',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ hierarchyName: hierarchyToDelete })
    }).then(function (responseText) {

        if (responseText.d) {
            $(event.target).parent().parent().remove();
        }
        else {
        }
    });
}

RADDataPrivileges.prototype.OnCreateHierarchy = function (event) {
    var isDataValid = RADDataPrivileges.instance.ValidateDataOnCreate(event);
    if (isDataValid) {
        var messageDiv = $(event.target).closest(".RADHierarchyMainNew").find(".RADHierarchyHeaderMessage");
        messageDiv.text("");
        messageDiv.addClass("RADCreateHierachyDisplayNone");
        var hierarchyObject = RADDataPrivileges.instance.GetHierarchyDetailsForCreate(event);
        //RADDataPrivileges.instance.CreateHierarchy(hierarchyObject);
        RADDataPrivileges.instance.ValidateHierarchyCreation(hierarchyObject, event);
    }
}

RADDataPrivileges.prototype.ValidateHierarchyCreation = function (hierarchyObject, event) {

    var isHierarchyValid = false;
    var hierarchyObjectString = JSON.stringify(hierarchyObject);
    $("#RADHierarchyMainDiv").addClass("RADDLPHierSaveSpinner")
    $("#RADHierarchyMainDiv").css("pointer-events", "none");
    $("#RADFuncGroupsMainDiv").css("pointer-events", "none");

    $.ajax({
        url: RADDataPrivileges.instance.ServiceURL + '/ValidateHierarchy',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ hierarchyInfo: hierarchyObjectString })
    }).then(function (responseText) {

        $("#RADHierarchyMainDiv").removeClass("RADDLPHierSaveSpinner")
        $("#RADHierarchyMainDiv").css("pointer-events", "");
        $("#RADFuncGroupsMainDiv").css("pointer-events", "");

        if (responseText.d == true) {
            RADDataPrivileges.instance.CreateHierarchy(hierarchyObject);
        }
        else {
            var messageDiv = $(event.target).closest(".RADHierarchyMainNew").find(".RADHierarchyHeaderMessage");
            messageDiv.text("Hierarchy creation is marked as invalid");
            messageDiv.removeClass("RADCreateHierachyDisplayNone");
        }

    });

}

RADDataPrivileges.prototype.CreateHierarchy = function (hierarchyObject) {
    var hierarchyObjectString = JSON.stringify(hierarchyObject);
    $("#RADHierarchyMainDiv").addClass("RADDLPHierSaveSpinner")
    $("#RADHierarchyMainDiv").css("pointer-events", "none");
    $("#RADFuncGroupsMainDiv").css("pointer-events", "none");

    $.ajax({
        url: RADDataPrivileges.instance.ServiceURL + '/CreateHierarchy',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ hierarchyDetails: hierarchyObjectString })
    }).then(function (responseText) {
        $("#RADHierarchyMainDiv").removeClass("RADDLPHierSaveSpinner")
        $("#RADHierarchyMainDiv").css("pointer-events", "");
        $("#RADFuncGroupsMainDiv").css("pointer-events", "");

        if (responseText.d == true) {
            //sucess
            //var messageDiv = $(event.target).closest(".RADHierarchyMainNew").find(".RADHierarchyHeaderMessage");
            //messageDiv.text("Hierarchy creation is marked as invalid");
            //messageDiv.removeClass("RADCreateHierachyDisplayNone");
            $(".RADHierarchyHeaderMessage").text("Hierarchy created sucessfully");
            $(".RADHierarchyHeaderMessage").css("color", "#34D4BA");
            $(".RADHierarchyHeaderMessage").removeClass("RADCreateHierachyDisplayNone");
        }
        else {
            $(".RADHierarchyHeaderMessage").text("Hierarchy creation failed");
            $(".RADHierarchyHeaderMessage").removeClass("RADCreateHierachyDisplayNone");
        }
        setTimeout(function () {
            $("#RADDimensionsSelectedMainDiv").empty();;
            $("#RADHierarchyNameValueDiv").text("");
            $(".RADHierarchyHeaderMessage").addClass("RADCreateHierachyDisplayNone");
            $(".RADHierarchyHeaderMessage").css("color", "#34D4BA");
            $(".RADHierarchyModuleNamesSelectionText").text("--Select--");
            $(".RADHierarchyModuleNamesSelectionText").attr("radselectedvalues", "");
            RADDataPrivileges.instance.AddEntityAttributeControl();
            RADDataPrivileges.instance.GetAllHierarchies();
        }, 2000);
    });

}

RADDataPrivileges.prototype.ValidateDataOnCreate = function (event) {

    var isDataValid = true;
    var messageDiv = $(event.target).closest(".RADHierarchyMainNew").find(".RADHierarchyHeaderMessage");
    messageDiv.text("");
    messageDiv.addClass("RADCreateHierachyDisplayNone");
    if ($("#RADHierarchyNameValueDiv").text().trim() == "") {
        isDataValid = false;
        messageDiv.text("Hierarchy name can't be empty");
    }

    else if ($("#RADHierarchyNameValueDiv").text().trim() != "") {
        var existingHierarchyInfo = $.grep(RADDataPrivileges.instance.AllHierarchies, function (e)
        { return e.HierarchyName.toLowerCase() == $("#RADHierarchyNameValueDiv").text().trim().toLowerCase(); });
        if(existingHierarchyInfo!=null && existingHierarchyInfo.length > 0){
            isDataValid = false;
            messageDiv.text("Hierarchy name already exists");
        }
    }

    else if ($(event.target).closest(".RADHierarchyCreationMain").find(".RADHierarchyModuleNamesSelectionText").text().trim() == "--Select--") {
        isDataValid = false;
        messageDiv.text("Please select the modules");
    }

    else {

        var entityControls = $(".RADDimensionSelectText");
        for (var i = 0; i < entityControls.length; i++) {
            if (($(entityControls[i]).text().trim() == "--Select--") && isDataValid) {
                messageDiv.text("Please select entities");
                isDataValid = false;
                break;
            }
        }

        var attributeControls = $(".RADZoneSelectText");
        for (var i = 0; i < attributeControls.length; i++) {
            if (($(attributeControls[i]).text().trim() == "--Select--") && isDataValid) {
                isDataValid = false;
                messageDiv.text("Please select attributes");
                break;
            }
        }

    }

    if (isDataValid == false)
        messageDiv.removeClass("RADCreateHierachyDisplayNone");
    else {
        messageDiv.text("");
        messageDiv.addClass("RADCreateHierachyDisplayNone");
    }

    return isDataValid;
}

RADDataPrivileges.prototype.GetHierarchyDetailsForCreate = function (event) {

    var hierarchyObject = {};
    hierarchyObject.HierarchyName = $("#RADHierarchyNameValueDiv").text().trim();
    hierarchyObject.HierarchyEntitiesInfo = [];
    hierarchyObject.Modules = [];
    var entityAtributeValuesControl = $(".RADSelectedDimensionKey");
    var selectedModulesText = $(event.target).closest(".RADHierarchyCreationMain").find(".RADHierarchyModuleNamesSelectionText").attr("radselectedvalues");
    if (selectedModulesText != null) {
        if (selectedModulesText == "ALL")
            hierarchyObject.Modules.push("ALL")
        else {
            var selectedModules = selectedModulesText.split("|^");
            var index = selectedModules.indexOf("");
            if (index >= 0) {
                selectedModules.splice(index, 1);
            }
            hierarchyObject.Modules = selectedModules;
        }
    }
    else {
        //Error message
    }

    for (var i = 0; i < entityAtributeValuesControl.length; i++) {

        var currentControl = entityAtributeValuesControl[i];
        var currentControlEntityAtrribute = $(currentControl).attr('radentityattributevalue');
        var currentEntityAtrribute = currentControlEntityAtrribute.split("|^");
        var currentEntityAtrributeId = $(currentControl).attr('radentityattributeid').split("|^");
        

        var EntityTypeInfo = {};
        EntityTypeInfo.EntityTypeId = currentEntityAtrributeId[0];
        EntityTypeInfo.EntityTypeName = currentEntityAtrribute[0];
        EntityTypeInfo.ApiSource = $(currentControl).attr('radentityapisource');
        EntityTypeInfo.AttributesInfo = [];

        var AttributeInfo = {};
        AttributeInfo.AttributeId = currentEntityAtrributeId[1];
        AttributeInfo.AttributeName = currentEntityAtrribute[1];
        AttributeInfo.AttributeDisplayName = $(currentControl).text().trim();
        AttributeInfo.IsLookUp = $(currentControl).attr('radislookupattribute');
        //AttributeInfo.IsUnique = $(currentControl).attr('radisuniqueattribute');
        
        if (AttributeInfo.IsLookUp) {
            var LookUpEntity = {};
            LookUpEntity.EntityType = $(currentControl).attr('radparententitytypeid');
            LookUpEntity.Attribute = $(currentControl).attr('radparentattributeid');
            AttributeInfo.LookupEntity = LookUpEntity;
        }
        

        var existingEntityTypeInfo = $.grep(hierarchyObject.HierarchyEntitiesInfo, function (e)
        { return e.EntityTypeName == EntityTypeInfo.EntityTypeName; });

        var isEntityTypeExist = (existingEntityTypeInfo != null && existingEntityTypeInfo.length > 0) ? true : false;

        if (isEntityTypeExist) {
            
            var existingAttributeInfo = $.grep(existingEntityTypeInfo[0].AttributesInfo, function (e)
            { return e.AttributeName == AttributeInfo.AttributeName; });

            var isAttributeExist = (existingAttributeInfo != null && existingAttributeInfo.length > 0) ? true : false;

            if (!isAttributeExist)
                existingEntityTypeInfo[0].AttributesInfo.push(AttributeInfo)
        }
        else {
            EntityTypeInfo.AttributesInfo.push(AttributeInfo);
            hierarchyObject.HierarchyEntitiesInfo.push(EntityTypeInfo);
        }
    }
    return hierarchyObject;
}



RADDataPrivileges.prototype.ShowHideDimesions = function (event) {

    var isDimensionsHidden = $(event.target).closest(".RADCreatedHierarchyHeaderParent").attr('radisdimesnionshidden');

    if (isDimensionsHidden == "true") {
        $(event.target).closest(".RADCreatedHierarchyHeaderParent").attr("radisdimesnionshidden", "false");
        $(event.target).parent().find(".RADCreatedHierarchyExpandArrow").switchClass("fa-caret-right", "fa-caret-down");
        $(event.target).closest(".RADCreatedHierarchyHeaderParent").next().removeClass("RADCreateHierachyDisplayNone");
    }
    else {
        $(event.target).closest(".RADCreatedHierarchyHeaderParent").attr("radisdimesnionshidden", "true");
        $(event.target).parent().find(".RADCreatedHierarchyExpandArrow").switchClass("fa-caret-down", "fa-caret-right");
        $(event.target).closest(".RADCreatedHierarchyHeaderParent").next().addClass("RADCreateHierachyDisplayNone");
    }
}

/* Hierarchy Screen  Ends*/

/* Data Privilege Filter */

RADDataPrivileges.prototype.DataPrivilegeFilterPageLoad = function (event) {

    $.ajax({
        url: RADDataPrivileges.instance.ServiceURL + "/GetTagTemplates",
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ fileName: 'RADDataPrivilegeFilter.html' })

    }).then(function (responseText) {
        $("#" + RADDataPrivilegesConfig.contentBodyId).append(responseText.d);


        RADDataPrivileges.instance.BindFilterEventsCommon();

        //var currentControl = $(event.target).closest(".RADSelectedEntityParent");
        //RADDataPrivileges.instance.BindCommonEntityValues(currentControl, "Users");
    });
}

RADDataPrivileges.prototype.BindFilterEventsCommon = function () {

    $(".RADDataPrivFilterMainParent").unbind().click(function (event) {

        if ($(event.target).closest(".RADDataprivilegeSearch").length > 0) {
            //do nothing
        }

        else if ($(event.target).closest(".RADDimensionDropDownChild,.RADDimensionDropDownChildSelectAll, .RADDimensionDropDownChildUnSelectAll").length > 0) {
            //called when any of the value in the dropdown is selected

            if ($(event.target).closest('.RADDataPrivFilteUserSelectParent').find('.RADDataPrivFilterUserSelect').length > 0)
                RADDataPrivileges.instance.SelectDimensionCommonDropDown(event);
            else
                RADDataPrivileges.instance.SelectDimensionValuesDropDownFilter(event);
        }

        else if ($(event.target).closest(".RADHierarchyDropDownChild").length > 0) {
            //RADDataPrivileges.instance.OnHierarchyNameSelected(event, true);
            RADDataPrivileges.instance.OnHierarchyNameSelectedFiltered(event);
        }

        else if ($(event.target).closest(".RADDataPrivFilterContentHierarchyParent").length > 0) {
            RADDataPrivileges.instance.OnHierarchySelection();
        }

        else if ($(event.target).closest(".RADFuncGroupCreateEntitySelectText").length > 0) {

            RADDataPrivileges.instance.BindDimensionDataForFilter(event);

        }

        else if ($(event.target).closest(".RADDataPrivFilteUserSelectParent").length > 0) {

            RADDataPrivileges.instance.CommonDataForCreateHierarchy = {};
            $.ajax({
                url: RADDataPrivileges.instance.ServiceURL + '/GetCommonDataForCreateHierarchy',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json'
            }).then(function (responseText) {
                var commonData = responseText.d;
                for (var j = 0; j < commonData.length; j++) {
                    RADDataPrivileges.instance.CommonDataForCreateHierarchy[commonData[j].Key] = commonData[j].Value;
                }
                var currentControl = $(event.target).closest(".RADDataPrivFilteUserSelectParent");
                RADDataPrivileges.instance.BindCommonEntityValues(currentControl, "Groups");
            });
        }

        else if ($(event.target).closest(".RADDataPrivFilterApply").length > 0) {
            //Apply  click
            //RADDataPrivileges.instance.GetDataPrivilegeInfoForFilter(event);
            RADDataPrivileges.instance.OnApplyDataPrivilegeFilter(event);
        }
        else if ($(event.target).closest(".RADDataPrivFilterClose").length > 0) {
            $('.RADDataPrivFilterMainParent').remove();
        }
        else {
            $(".RADFunGroupDimensionsMainDiv").remove();
            $(".RADAllHierarchiesMainDiv").remove();

        }
    });
}

RADDataPrivileges.prototype.OnHierarchyNameSelectedFiltered = function (event) {

    //Get All Entities
    // Bing Event for all Entities
    var self = this;
    self.AllEntitiesForHierarchy = [];
    $(".RADAllHierarchiesMainDiv").remove();
    $(".RADFuncGroupCreateEntitiesContentFilter").empty();

    var currentHierarchySelected = $(event.target).text().trim();
    $(".RADDataPrivFilterHierarchySelectText").text(currentHierarchySelected);
    $(".RADAllHierarchiesMainDiv").remove();
    var currentHierarchyDetails = $.grep(RADDataPrivileges.instance.AllHierarchies, function (e)
    { return e.HierarchyName == currentHierarchySelected; });

    for (var i = 0; i < currentHierarchyDetails[0].HierarchyEntitiesInfo.length; i++) {
        var currentEntityId = currentHierarchyDetails[0].HierarchyEntitiesInfo[i].EntityTypeId;
        var currentEntityName = currentHierarchyDetails[0].HierarchyEntitiesInfo[i].EntityTypeName;
        var currentEntityAttributeInfo = currentHierarchyDetails[0].HierarchyEntitiesInfo[i].AttributesInfo;
        for (var j = 0; j < currentEntityAttributeInfo.length; j++) {
            var currentAttributeInfo = currentEntityAttributeInfo[j];
            //if (self.AllEntitiesForHierarchy.indexOf(currentEntityName) == -1)
            //    self.AllEntitiesForHierarchy.push(currentEntityName
            if (self.AllEntitiesForHierarchy.indexOf(currentEntityId) == -1)
                self.AllEntitiesForHierarchy.push(currentEntityId);
            RADDataPrivileges.instance.BindDimensionControlsForFilter(currentHierarchySelected, currentEntityId, currentEntityName, currentAttributeInfo);
        }
    }
}

RADDataPrivileges.prototype.BindDimensionControlsForFilter = function (hierarchyName, entityId,  entityName, attributeInfo) {

    var $entityContentDiv = $("<div>", {
        class: "RADFuncGroupCreateEntityContent"
    });

    var $entityNameDiv = $("<div>", {
        class: "RADFuncGroupCreateEntityName",
        text: attributeInfo.AttributeDisplayName
    });

    $entityNameDiv.attr("radentityname", entityName);
    $entityNameDiv.attr("isLookUp", attributeInfo.IsLookUp);
    $entityNameDiv.attr("isUnique", attributeInfo.isUnique);
    $entityNameDiv.attr("radmappedattribute", attributeInfo.MappedAttributeName);
    $entityNameDiv.attr("radentityattributename", entityName + "|^" + attributeInfo.AttributeName);
    $entityNameDiv.attr("radentityattributeid", entityId + "|^" + attributeInfo.AttributeId);
    if (attributeInfo.IsLookUp) {
        $entityNameDiv.attr("parentAttribute", attributeInfo.LookupEntity.Attribute);
        $entityNameDiv.attr("parentEntity", attributeInfo.LookupEntity.EntityType);
    }

    var $entitySelectDivParent = $("<div>", {
        class: "RADSelectedEntityParent"
    });

    var $entitySelectDiv = $("<div>", {
        id: "RADFuncGroupCreateEntitySelect" + attributeInfo.AttributeDisplayName,
        class: "RADFuncGroupCreateEntitySelect"
    });

    var $entitySelectText = $("<div>", {
        class: "RADFuncGroupCreateEntitySelectText",
        text: "--Select--"
    });
    $entitySelectText.attr("radentityattributenametext", entityName + "|^" + attributeInfo.AttributeName);
    $entitySelectText.attr("radentityattributeid", entityId + "|^" + attributeInfo.AttributeId);
    $entitySelectDiv.append($entitySelectText);
    $entitySelectDivParent.append($entitySelectDiv)


    $entityContentDiv.append($entityNameDiv).append($entitySelectDivParent);
    $(".RADFuncGroupCreateEntitiesContentFilter").append($entityContentDiv);



}

RADDataPrivileges.prototype.BindDimensionDataForFilter = function (event) {

    var entityAttributeName = $(event.target).closest(".RADFuncGroupCreateEntitySelectText")
                                              .attr("radentityattributenametext").trim().split("|^");
    var entityAttributeId = $(event.target).closest(".RADFuncGroupCreateEntitySelectText")
                                                  .attr("radentityattributeid").trim().split("|^");
    var currentControl = $(event.target).closest(".RADSelectedEntityParent");

    var hierarchyName = currentControl.closest(".RADDataPrivFilterMain")
                                            .find(".RADDataPrivFilterHierarchySelectText").text().trim();

    var entitySearchRequest = {};
    entitySearchRequest.HierarchyName = hierarchyName;
    entitySearchRequest.EntityId = entityAttributeId[0];
    entitySearchRequest.EntityName = entityAttributeName[0];
    entitySearchRequest.AttributeId = entityAttributeId[1];
    entitySearchRequest.AttributeName = entityAttributeName[1];
    var entitySearchRequestText = JSON.stringify(entitySearchRequest);

    var apiInfo = RADDataPrivileges.instance.GetApiInfobyHierarchyName(hierarchyName, entityAttributeId[0]);

    $.ajax({
        url: RADDataPrivileges.instance.ServiceURL + '/GetAllDimensions',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({
            entitySearchRequest: entitySearchRequestText, apiType: apiInfo.APiType,
            apiPath: apiInfo.APiPath
        })
    }).then(function (responseText) {
        var dimensionData = responseText.d;
        RADDataPrivileges.instance.BindDimensionDropDownDataFilter(currentControl, dimensionData);
    });
}

RADDataPrivileges.prototype.BindDimensionDropDownDataFilter = function (currentControl, dimensionData) {

    var textControl = currentControl.find(".RADFuncGroupCreateEntitySelectText");
    var selectedDimensionsText = textControl.attr("radselectedvalues");
    var existingDimesionsArray = [];

    if (selectedDimensionsText != null) {
        if (selectedDimensionsText == "ALL" || selectedDimensionsText == "ALL|^") {
            dimensionData.forEach(function (dimensionItem) { existingDimesionsArray.push(dimensionItem.Value); });
        }
        else {
            existingDimesionsArray = selectedDimensionsText.split("|^");
            if (existingDimesionsArray.indexOf("") >= 0)
                existingDimesionsArray.splice(existingDimesionsArray.indexOf(""), 1);
        }
    }

    RADDataPrivileges.instance.BindDropDownControlFilter(currentControl, selectedDimensionsText, dimensionData, existingDimesionsArray);

}

RADDataPrivileges.prototype.BindDropDownControlFilter = function (currentControl, selectedDimensionsText, dimensionData, existingSelectedDimesionsArray) {

    $(".RADFunGroupDimensionsMainDiv").remove();

    var $dimensionsMainDiv = $("<div>", {
        class: "RADFunGroupDimensionsMainDiv"
    });

    var $currentRowDimensionParent = $("<div>", {
        class: "RADDimensionDropDownParent"
    });

    var $searchParentDiv = $("<div>", {
        class: "RADDataprivilegeSearchParent"
    });

    var $searchDiv = $("<div>", {
        class: "RADDataprivilegeSearch"
    });

    var $selectAllRowDiv = $("<div>", {
        class: "RADDimensionDropDownChildSelectAll",
        text: "Select All"
    });
    if (dimensionData.length != existingSelectedDimesionsArray.length)
        $selectAllRowDiv.attr("isselectedmode", "false");
    else {
        $selectAllRowDiv.attr("isselectedmode", "true");
        $selectAllRowDiv.addClass("RADDimensionDropDownChildUnSelectAll");
    }
    $selectAllRowDiv.attr("radisfilteredbylookup", "false");
    $selectAllRowDiv.attr("raduniqueattributeid", "SelectAll");

    $searchDiv.attr("contenteditable", true);
    $searchParentDiv.append($searchDiv);
    $currentRowDimensionParent.append($searchParentDiv);
    $currentRowDimensionParent.append($selectAllRowDiv);

    for (var i = 0; i < dimensionData.length; i++) {
        var currentDimesionKey = dimensionData[i].Key;
        var currentDimesionValue = dimensionData[i].Value;

        var $currentParent = $("<div>", {
            class: "RADDimensionDropDownChildParent"
        });

        var $currentRowDimesion = $("<div>", {
            class: "RADDimensionDropDownChild",
            text: currentDimesionValue
        });

        $currentRowDimesion.attr("radisfilteredbylookup", "false");
        $currentRowDimesion.attr("raduniqueattributeid", currentDimesionKey);

        var $currentRowDimesionTick = $("<div>", {
            class: "RADCreateHierachyDisplayNone RADDimensionDropDownChildTick fa fa-check"
        });

        $currentParent.append($currentRowDimesion);
        $currentParent.append($currentRowDimesionTick);

        if (existingSelectedDimesionsArray.indexOf(currentDimesionValue) != -1)
            $currentRowDimesionTick.removeClass("RADCreateHierachyDisplayNone");
        $currentRowDimensionParent.append($currentParent);
    }

    $dimensionsMainDiv.append($currentRowDimensionParent);
    if (currentControl.closest(".RADFuncGroupCreateEntityContent").length > 0)
        $dimensionsMainDiv.css({ 'left': (currentControl.closest(".RADFuncGroupCreateEntityContent,.RADExistingDataPrivilegeContent").offset().left + 25 + 'px') });
    else
        $dimensionsMainDiv.css({ 'left': (currentControl.closest(".RADFuncGroupCreateEntityContent,.RADExistingDataPrivilegeContent").offset().left + 'px') });
    currentControl.append($dimensionsMainDiv);

    $(".RADDataprivilegeSearch").unbind("keyup").keyup(function (event) {
        RADDataPrivileges.instance.SearchCurrentDropDown(event);
    })

}

RADDataPrivileges.prototype.SelectHierachyModulesDropDown = function (event) {

    var textControl = $(event.target).closest(".RADHierarchyModuleNamesSelectionParent")
                                         .find(".RADHierarchyModuleNamesSelectionText");
    var selectedModule = $(event.target).text();
    var selectionMode = "";

    var existingItemObjectArray = [];
    var existingItemIdsArray = [];

    if ($(event.target).hasClass("RADModulesDropDownChildSelectAll") && $(event.target).attr("isselectedmode") == "false") {
        //Select All Case
        $(event.target).attr("isselectedmode", "true");
        $(event.target).addClass("RADModulesDropDownChildUnSelectAll");
        $(event.target).closest(".RADFunGroupModulesMainDiv").find(".RADModulesDropDownChildTick").removeClass("RADCreateHierachyDisplayNone");
        selectionMode = "SelectAll";
        $(event.target).closest(".RADFunGroupModulesMainDiv").find(".RADModulesDropDownChild").each(function (index) {
            if (!$(this).closest(".RADModulesDropDownChildParent").find(".RADModulesDropDownChildTick").hasClass("RADCreateHierachyDisplayNone")) {
                existingItemObjectArray.push($(this).text().trim());
                existingItemIdsArray.push($(this).attr("radmoduleid"));
            }
        });
        RADDataPrivileges.instance.SetHierarchyModuleDropDownAttributeTextandText(textControl, existingItemObjectArray, existingItemIdsArray);
        textControl.attr("radselectedvalues", "ALL");
        textControl.attr("radselectedmoduleids", "ALL");



    }
    else if ($(event.target).hasClass("RADModulesDropDownChildSelectAll") && $(event.target).attr("isselectedmode") == "true") {
        //Unselect ALL case
        $(event.target).attr("isselectedmode", "false");
        $(event.target).removeClass("RADModulesDropDownChildUnSelectAll");
        $(event.target).closest(".RADFunGroupModulesMainDiv").find(".RADModulesDropDownChildTick").addClass("RADCreateHierachyDisplayNone");
        selectionMode = "UnSelectAll";
        RADDataPrivileges.instance.SetHierarchyModuleDropDownAttributeTextandText(textControl, existingItemObjectArray, existingItemIdsArray);

    }
    else if (!$(event.target).closest(".RADModulesDropDownChildParent")
                             .find(".RADModulesDropDownChildTick").hasClass("RADCreateHierachyDisplayNone")) {
        //unselect one
        existingItemObjectArray = [];
        existingItemIdsArray = [];
        $(event.target).closest(".RADModulesDropDownChildParent").find(".RADModulesDropDownChildTick").addClass("RADCreateHierachyDisplayNone");
        $(event.target).closest(".RADFunGroupModulesMainDiv").find(".RADModulesDropDownChildSelectAll").attr("isselectedmode", "false");
        $(event.target).closest(".RADFunGroupModulesMainDiv").find(".RADModulesDropDownChild").each(function (index) {
            if (!$(this).closest(".RADModulesDropDownChildParent").find(".RADModulesDropDownChildTick").hasClass("RADCreateHierachyDisplayNone")) {
                existingItemObjectArray.push($(this).text().trim());
                existingItemIdsArray.push($(this).attr("radmoduleid"));
            }
        });
        RADDataPrivileges.instance.SetHierarchyModuleDropDownAttributeTextandText(textControl, existingItemObjectArray, existingItemIdsArray);
    }

    else {
        $(event.target).closest(".RADModulesDropDownChildParent").find(".RADModulesDropDownChildTick").removeClass("RADCreateHierachyDisplayNone");
        existingItemObjectArray = [];
        existingItemIdsArray = [];
        $(event.target).closest(".RADFunGroupModulesMainDiv").find(".RADModulesDropDownChild").each(function (index) {
            if (!$(this).closest(".RADModulesDropDownChildParent").find(".RADModulesDropDownChildTick").hasClass("RADCreateHierachyDisplayNone")) {
                existingItemObjectArray.push($(this).text().trim());
                existingItemIdsArray.push($(this).attr("radmoduleid"));
            }
        });
        RADDataPrivileges.instance.SetHierarchyModuleDropDownAttributeTextandText(textControl, existingItemObjectArray, existingItemIdsArray);
    }



}

RADDataPrivileges.prototype.SetHierarchyModuleDropDownAttributeTextandText = function (currentControl, selectValuesArray, selectIdsArray) {
    var radDropDownsText = "";
    //if (currentControl.text() == "ALL" && selectValuesArray.length == 0) {
    //    currentControl.text("ALL");
    //    currentControl.attr("radselectedvalues", "ALL");
    //    currentControl.attr("radselectedmoduleids", "ALL");
    //    return;
    //}
    if (selectValuesArray.length == 0) {
        currentControl.text("--Select--");
        currentControl.attr("radselectedvalues", "");
        currentControl.attr("radselectedmoduleids", "");
        return;
    }
    else {
        var dimensionsValuesArray = [];
        selectValuesArray.forEach(function (currentItem) {
            dimensionsValuesArray.push(currentItem);
            radDropDownsText = radDropDownsText + currentItem + "|^";
        });
        currentControl.attr("radselectedvalues", radDropDownsText);

        radDropDownsText = "";
        selectIdsArray.forEach(function (currentItem) {
            radDropDownsText = radDropDownsText + currentItem + "|^";
        });
        currentControl.attr("radselectedmoduleids", radDropDownsText);

        if (selectValuesArray.length <= 2) {
            currentControl.text(dimensionsValuesArray.join());
        }
        else {
            var slicedArray = dimensionsValuesArray.slice(0, 2);
            var selectedText = slicedArray.join();
            selectedText = selectedText + " <span class='RADNMoreClass'>" + (dimensionsValuesArray.length - 2).toString() + " More </span>";
            currentControl.html(selectedText);
        }
    }
}



RADDataPrivileges.prototype.SelectDimensionValuesDropDownFilter = function (event) {

    var textControl = $(event.target).closest(".RADSelectedEntityParent")
                                     .find(".RADFuncGroupCreateEntitySelectText,.RADExistingDataPrivilegeEntitySelect");
    var selectedDimension = $(event.target).text();

    if ($(event.target).hasClass("RADDimensionDropDownChildSelectAll") && $(event.target).attr("isselectedmode") == "false") {
        //Select All Case
        $(event.target).attr("isselectedmode", "true");
        $(event.target).addClass("RADDimensionDropDownChildUnSelectAll");
        $(event.target).closest(".RADFunGroupDimensionsMainDiv").find(".RADDimensionDropDownChildTick").removeClass("RADCreateHierachyDisplayNone");
        //textControl.attr("radselectedvalues", "ALL");
        //textControl.text("ALL");
        var existingItemObjectArray = [];
        $(event.target).closest(".RADFunGroupDimensionsMainDiv").find(".RADDimensionDropDownChild").each(function (index) {
            if (!$(this).closest(".RADDimensionDropDownChildParent").find(".RADDimensionDropDownChildTick").hasClass("RADCreateHierachyDisplayNone")) {
                existingItemObjectArray.push($(this).text().trim());
            }
        });
        RADDataPrivileges.instance.SetDropDownsAttributeTextandTextFilter(textControl, existingItemObjectArray);
    }
    else if ($(event.target).hasClass("RADDimensionDropDownChildSelectAll") && $(event.target).attr("isselectedmode") == "true") {
        //Unselect ALL case
        $(event.target).attr("isselectedmode", "false");
        $(event.target).removeClass("RADDimensionDropDownChildUnSelectAll");
        $(event.target).closest(".RADFunGroupDimensionsMainDiv").find(".RADDimensionDropDownChildTick").addClass("RADCreateHierachyDisplayNone");
        textControl.attr("radselectedvalues", "");
        textControl.text("--Select--");
    }
    else if (!$(event.target).closest(".RADDimensionDropDownChildParent")
                             .find(".RADDimensionDropDownChildTick").hasClass("RADCreateHierachyDisplayNone")) {
        //unselect one

        $(event.target).closest(".RADDimensionDropDownChildParent").find(".RADDimensionDropDownChildTick").addClass("RADCreateHierachyDisplayNone");
        var existingItemObjectArray = [];
        $(event.target).closest(".RADFunGroupDimensionsMainDiv").find(".RADDimensionDropDownChildSelectAll").attr("isselectedmode", "false");
        $(event.target).closest(".RADFunGroupDimensionsMainDiv").find(".RADDimensionDropDownChild").each(function (index) {
            if (!$(this).closest(".RADDimensionDropDownChildParent").find(".RADDimensionDropDownChildTick").hasClass("RADCreateHierachyDisplayNone")) {
                existingItemObjectArray.push($(this).text().trim());
            }
        });
        RADDataPrivileges.instance.SetDropDownsAttributeTextandTextFilter(textControl, existingItemObjectArray);

    }
    else {
        $(event.target).closest(".RADDimensionDropDownChildParent").find(".RADDimensionDropDownChildTick").removeClass("RADCreateHierachyDisplayNone");
        var existingItemObjectArray = [];
        $(event.target).closest(".RADFunGroupDimensionsMainDiv").find(".RADDimensionDropDownChild").each(function (index) {
            if (!$(this).closest(".RADDimensionDropDownChildParent").find(".RADDimensionDropDownChildTick").hasClass("RADCreateHierachyDisplayNone")) {
                existingItemObjectArray.push($(this).text().trim());
            }
        });
        RADDataPrivileges.instance.SetDropDownsAttributeTextandTextFilter(textControl, existingItemObjectArray);

    }
}

RADDataPrivileges.prototype.SetDropDownsAttributeTextandTextFilter = function (currentControl, selectValuesArray) {

    var radDropDownsText = "";

    if (selectValuesArray.length == 0) {
        currentControl.text("--Select--");
        currentControl.attr("radselectedvalues", "");
        return;
    }
    else {
        var dimensionsValuesArray = [];
        selectValuesArray.forEach(function (currentItem) {
            dimensionsValuesArray.push(currentItem);
            radDropDownsText = radDropDownsText + currentItem + "|^";
        });
        currentControl.attr("radselectedvalues", radDropDownsText);

        if (selectValuesArray.length <= 2) {
            currentControl.text(dimensionsValuesArray.join());
        }
        else {
            var slicedArray = dimensionsValuesArray.slice(0, 2);
            var selectedText = slicedArray.join();
            selectedText = selectedText + " <span class='RADNMoreClass'>" + (dimensionsValuesArray.length - 2).toString() + " More </span>";
            currentControl.text(selectedText);
        }
    }

}

RADDataPrivileges.prototype.OnApplyDataPrivilegeFilter = function (event) {

    var self = this;

    $(".RADDataPrivMessage").text("");
    var dataPrivilegeInfo = RADDataPrivileges.instance.GetDataPrivilegeInfoForFilter(event);

    $.ajax({
        url: RADDataPrivileges.instance.ServiceURL + '/SearchDataPrivilege',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ dataPrivilegeInfo: JSON.stringify(dataPrivilegeInfo) })
    }).then(function (responseText) {
        if (responseText.d != "") {
            //self.OnPageLoad();
            //var filteredDataPrivileges = JSON.parse(responseText.d);
            var filteredDataPrivileges = $.parseJSON(responseText.d);
            if (filteredDataPrivileges.length > 0) {
                self.allDataPrivileges = filteredDataPrivileges;
                $('.RADDataPrivFilterMainParent').remove();
                $('.RADExistingDataPrivileges').empty();
                self.BindAllDataPrivileges(filteredDataPrivileges);
                self.showHideEditDeletePrivilegeButtons();
                self.bindQTip();
                $(".RADExistingDataPrivileges").unbind(".click").click(function (event) {
                    self.existingDataPrivilegeCLickHandler(event);
                });
            }
            else {
                //No records for search criteria
                $(".RADDataPrivMessage").text("No records for the above search criteria");

            }
        }
        else {
            //No records for search criteria
            $(".RADDataPrivMessage").text("No records for the above search criteria");
        }
    });
}

RADDataPrivileges.prototype.GetDataPrivilegeInfoForFilter = function (event) {

    var self = this;

    var dataPrivilegeInfo = {};
    dataPrivilegeInfo.HierarchyDetail = {};
    dataPrivilegeInfo.HierarchyDetail.HierarchyEntitiesInfo = [];

    var hierarchyName = $(event.target).closest('.RADDataPrivFilterMain').find('.RADDataPrivFilterHierarchySelectText').text().trim() == "--Select--" ?
                        "" : $(event.target).closest('.RADDataPrivFilterMain').find('.RADDataPrivFilterHierarchySelectText').text().trim();
    dataPrivilegeInfo.HierarchyDetail.HierarchyName = hierarchyName;

    dataPrivilegeInfo.Groups = [];
    if ($(event.target).closest('.RADDataPrivFilterMain').find('.RADDataPrivFilterUserSelectText').attr('radselectedvalues') != null) {
        dataPrivilegeInfo.Groups = $(event.target).closest('.RADDataPrivFilterMain').find('.RADDataPrivFilterUserSelectText')
                                                 .attr('radselectedvalues').split("|^");
        var index = dataPrivilegeInfo.Groups.indexOf("");
        if (index >= 0) {
            dataPrivilegeInfo.Groups.splice(index, 1);
        }
    }

    var getEntityInfo = false;

    if (hierarchyName != "") {
        var entitiesTextControls = $('.RADFuncGroupCreateEntitiesContentFilter').find('.RADFuncGroupCreateEntitySelectText');
        for (var i = 0; i < entitiesTextControls.length; i++) {
            var currentControl = entitiesTextControls[i];
            if ($(currentControl).text() != "--Select--") {
                getEntityInfo = true;
                break;
            }
        }

    }

    if (getEntityInfo) {
        for (var i = 0; i < self.AllEntitiesForHierarchy.length; i++) {
            var EntityTypeInfo = {};
            EntityTypeInfo.EntityTypeId = self.AllEntitiesForHierarchy[i];
            var currentHierarchyDetails = $.grep(RADDataPrivileges.instance.AllHierarchies, function (e)
            { return e.HierarchyName == hierarchyName; });

            var currentEntityInfo = $.grep(currentHierarchyDetails[0].HierarchyEntitiesInfo, function (e)
            { return e.EntityTypeId == EntityTypeInfo.EntityTypeId; });
            EntityTypeInfo.EntityTypeName = currentEntityInfo[0].EntityTypeName;
            EntityTypeInfo.ApiSource = currentEntityInfo[0].ApiSource;
            EntityTypeInfo.AttributesInfo = [];
            self.GetEntityInfoForFilter(EntityTypeInfo);
            dataPrivilegeInfo.HierarchyDetail.HierarchyEntitiesInfo.push(EntityTypeInfo);
        }

    }
    return dataPrivilegeInfo;
}

RADDataPrivileges.prototype.GetEntityInfoForFilter = function (EntityTypeInfo) {
    var self = this;
    var entityAttrInfo = $(".RADFuncGroupCreateEntityContent").find("div[radentityname='" + EntityTypeInfo.EntityTypeName + "']");
    var AttributesInfo = {};
    entityAttrInfo.each(function (index) {
        AttributesInfo = {};
        AttributesInfo.AttributeName = $(this).closest(".RADFuncGroupCreateEntityContent").find(".RADFuncGroupCreateEntityName").attr("radentityattributename").split("|^")[1];
        AttributesInfo.AttributeDisplayName = $(this).closest(".RADFuncGroupCreateEntityContent").find(".RADFuncGroupCreateEntityName").text().trim();
        AttributesInfo.IsLookUp = ($(this).attr("isLookUp") == 'true');

        AttributesInfo.AttributeValues = {};
        self.IsValid = true;
        if (AttributesInfo.IsLookUp) {
            AttributesInfo.LookupEntity = {};
            AttributesInfo.LookupEntity.Attribute = $(this).closest(".RADFuncGroupCreateEntityContent").find(".RADFuncGroupCreateEntityName").attr("parentAttribute")
            AttributesInfo.LookupEntity.EntityType = $(this).closest(".RADFuncGroupCreateEntityContent").find(".RADFuncGroupCreateEntityName").attr("parentEntity");
        }

        if ($(this).closest(".RADFuncGroupCreateEntityContent").find(".RADFuncGroupCreateEntitySelectText").attr("radselectedvalues") != null) {
            var selectedValues = $(this).closest(".RADFuncGroupCreateEntityContent").find(".RADFuncGroupCreateEntitySelectText").attr("radselectedvalues");
            if (selectedValues == "ALL" || selectedValues == "ALL|^") {
                AttributesInfo.AttributeValues["ALL"] = "ALL";
            }
            else {
                var attrValues = selectedValues.split("|^");
                if (attrValues.indexOf("") >= 0)
                    attrValues.splice(attrValues.indexOf(""), 1);
                attrValues.forEach(function (currentItem) {
                    var currentAtrributeValue = currentItem;
                    AttributesInfo.AttributeValues[currentAtrributeValue] = currentAtrributeValue;
                });
            }
        }
        EntityTypeInfo.AttributesInfo.push(AttributesInfo);
    });
}











