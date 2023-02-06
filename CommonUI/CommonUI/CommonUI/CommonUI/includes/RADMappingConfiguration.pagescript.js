
RADMappingConfig = {};
RADMappingConfig.initialize = function (initObj) {
    $.extend(RADMappingConfig, initObj);
    if (!initObj.IsIagoDependent) {
        $(".iago-page-title").css("display", "none");
        var $rMappingTabDiv = $("<div>", {
            id: "RMappingTabDiv",
            class: "RMappingTab"
        });
        $("#" + RADMappingConfig.contentBodyId).empty();
        $("#" + RADMappingConfig.contentBodyId).append($rMappingTabDiv);        
    }
    else {
        $("#RMappingTabDiv").css("display", "none");
        if ($("#pageHeaderTabPanel").data("iago-widget-tabs") != null)
            $("#pageHeaderTabPanel").data("iago-widget-tabs").destroy();
    }


    var obj = new RADMappingConfiguration();
    obj.init();
};


var RADMappingConfiguration = function () {
    
};


RADMappingConfiguration.prototype.init = function (sandBox) {
    var self = this;
    self.ServiceURL = RADMappingConfig.baseUrl + "/Resources/Services/RADMappingService.svc";
    self.AllMappingsInfo = [];
    self.CurrentMappingDetailsId = 0;
    self.CurrentMappingName = "";
    self.CurrentMappingSummaryId = 0;
    self.CurrentMappingLookupValue = "";
    self.CurrentMappingSummaryInfo = {};
    self.RMapEntityTypes = [];
    self.RMapAttributesForEntity = [];
    self.AuthPrivileges = [];
    self.PageLoad();

    
};

RADMappingConfiguration.prototype.PageLoad = function () {
    var self = this;

    
    $.ajax({
        url: self.ServiceURL + '/GetAuthorizationPrivileges', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'

    }).then(function (responseText) {
        if (responseText.d == "admin") {
            self.AuthPrivileges = ["Add Mapping", "Add Mapping Details", "Update Mapping", "Update Mapping Details",
                                                     "Delete Mapping", "Delete Mapping Details"];
        }
        else {
            var ResponseForCreation = [];
            if (responseText.d.length > 0)
                ResponseForCreation = JSON.parse(responseText.d);
            self.AuthPrivileges = [];
            for (var i = 0; i < ResponseForCreation.length; i++) {
                if (ResponseForCreation[i].pageId == "RAD_Mapping") {
                    for (var j = 0; j < ResponseForCreation[i].Privileges.length; j++) {
                        self.AuthPrivileges.push(ResponseForCreation[i].Privileges[j]);
                    }
                }
            }
        }

        $.ajax({
            url: self.ServiceURL + '/GetTagTemplates',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ fileName: 'RMappingSetup.html' })
        }).then(function (responseText) {
            
            // $("#" + RADMappingConfig.contentBodyId).empty();
            $(".RMapSetupMain").remove();
            $("#" + RADMappingConfig.contentBodyId).append(responseText.d);

            var $loaderDiv = $("<div>", {
                id: "RMapSetupLoaderDiv",
                class: "RMapSetupLoader RMapSetupDisplayNone"
            });

            $("#" + RADMappingConfig.contentBodyId).append($loaderDiv);

            //ToDo-GetAuthorizarion Privileges
            var showAllAccounts = true;
            self.ShowHideByPrivilegeType();
            self.GetAllMapping(showAllAccounts);
            self.BindEventsPageLoad();
            self.CalculateHeight();
        });

    });
};

RADMappingConfiguration.prototype.ShowHideByPrivilegeType = function () {

    var self = this;

    if (self.AuthPrivileges.indexOf("Add Mapping") != -1) {
        $(".RMapSetupCreateParent").show();
    }
    else {
        $(".RMapSetupCreateParent").hide();
    }

    if (self.AuthPrivileges.indexOf("Update Mapping") != -1) {
        $(".RMapInfoUpdate").show();
    }
    else {
        $(".RMapInfoUpdate").hide();
    }

    if (self.AuthPrivileges.indexOf("Delete Mapping") != -1) {
        $(".RMapInfoDelete").show();
    }
    else {
        $(".RMapInfoDelete").hide();
    }

    if (self.AuthPrivileges.indexOf("Add Mapping Details") != -1) {
        $(".RMapSetupAddMapParent").show();
    }
    else {
        $(".RMapSetupAddMapParent").hide();
    }

    if (self.AuthPrivileges.indexOf("Add Mapping Details") != -1) {
        $(".RMapSetupAddMapParent").show();
    }
    else {
        $(".RMapSetupAddMapParent").hide();
    }

}

RADMappingConfiguration.prototype.CalculateHeight = function () {
    var self = this;
    var totalHeight = $(".RMapSetupMain").height();
    $(".RMapSetupLeftContentMain").height(totalHeight - ($(".RMapSetupLeftHeaderMain").height() + $(".RMapSetupScrollMain").height()));
}

RADMappingConfiguration.prototype.BindEventsPageLoad = function () {
    var self = this;

    $(".RMapSetupMain").unbind().click(function (event) {

        if ($(event.target).closest(".RMapInfoName,.RMapInfoType ").length > 0) {
            
            //$(event.target).closest(".RMapSetupLeftContentMain").find(".RMapInfoActive").removeClass("RMapInfoActive");
            var currentActiveTile = $(event.target).closest(".RMapSetupLeftContentMain").find(".RMapInfoActive");
            currentActiveTile.find(".RMapSetupArrowRight").remove();
            currentActiveTile.removeClass("RMapInfoActive");
            $(event.target).closest(".RMapInfoParent").find(".RMapInfoChild").addClass("RMapInfoActive");
            $(event.target).closest(".RMapInfoParent").find(".RMapInfoChild").append("<div class=\"fa fa-caret-right RMapSetupArrowRight\"></div>");
            var currentMappingSummaryId = $(event.target).closest(".RMapInfoParent").find(".RMapInfoChild").attr("rmappingsummaryid");
            self.CurrentMappingSummaryId = currentMappingSummaryId;
            self.GetMappingDetailsbyId(currentMappingSummaryId, true);

        }

        else if ($(event.target).closest(".btnRMapDetailUpdate").length > 0) {
            self.OnUpdateMappingDetails(event);
        }
        else if ($(event.target).closest(".btnRMapDetailDelete").length > 0) {
            self.OnDeleteMappingDetails(event);
        }
        else if ($(event.target).closest(".RMapSetupAddMap").length > 0) {
            self.OnAddMappingDetails(event);
        }
        else if ($(event.target).closest(".btnRMapDetailsCancel").length > 0) {
            self.CloseMappingUpateScreen(event);
        }
        else if ($(event.target).closest(".btnRMapDetailsUpdate").length > 0) {
            self.UpdateMappingDetails(event);
        }
        else if ($(event.target).closest(".RMapDetailsRemove").length > 0) {
            self.CloseMappingUpateScreen(event);
        }
        else if ($(event.target).closest(".btnRMapSetupCreate").length > 0) {
            self.OnAddMappingSummary(event);
        }
        else if ($(event.target).closest(".RMapInfoUpdate").length > 0) {
            self.CurrentMappingSummaryId = $(event.target).closest(".RMapInfoChild").attr("rmappingsummaryid");
            self.OnUpdateMappingSummary(event);
        }
        else if ($(event.target).closest(".RMapInfoDelete").length > 0) {
            self.CurrentMappingSummaryId = $(event.target).closest(".RMapInfoChild").attr("rmappingsummaryid");
            self.OnDeleteMappingSummary(event);
        }
        else if ($(event.target).closest(".RMapSetupLeftSearchView").length > 0) {
            if ($('.RMapSetupSearchInput:visible').length) {
                $(".RMapSetupSearchInput").hide("slow", function () {
                    // Animation complete.
                });
            }
            else {
                $(".RMapSetupSearchInput").show("slow", function () {
                });
            }
        }
        else if ($(event.target).closest(".RMapSetupScrollMain").length > 0) {
            self.ScrollData(event);
        }
        $(".RMapSetupSearchInput").unbind("keyup").keyup(function (event) {
            self.SearchMappings($(event.target).val());
        })

    });
}

RADMappingConfiguration.prototype.ScrollData = function (event) {

    if ($(event.target).hasClass("RMapSetupScrollDown")) {
        $(".RMapSetupLeftContentMain").scrollTop($(".RMapSetupLeftContentMain").scrollTop() + 50);
    }
    else if ($(event.target).hasClass("RMapSetupScrollUp")) {
        $(".RMapSetupLeftContentMain").scrollTop($(".RMapSetupLeftContentMain").scrollTop() - 50);
    }

}


RADMappingConfiguration.prototype.SearchMappings = function (mappingName) {

    var self = this;
    var mappingName = mappingName.trim().toLowerCase();
    var length = $(".RMapSetupLeftContentMain").children().length;

    for (var i = 0; i < length; i++) {
        if (($($(".RMapSetupLeftContentMain").children()[i])).find(".RMapInfoName").html().trim().toLowerCase().indexOf(mappingName) != -1) {
            $($(".RMapSetupLeftContentMain").children()[i]).removeClass("RMapSearchDisplayNone");
        }
        else {
            $($(".RMapSetupLeftContentMain").children()[i]).addClass("RMapSearchDisplayNone");
        }
    }


}

RADMappingConfiguration.prototype.OnUpdateMappingSummary = function (event) {
    var self = this;

    self.CurrentMappingSummaryId = $(event.target).closest(".RMapInfoChild").attr("rmappingsummaryid");
    $.ajax({
        url: self.ServiceURL + '/GetMappingDetails',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ mappingSummaryId: self.CurrentMappingSummaryId, isExcecuteGrid: false })
    }).then(function (responseText) {
        var mapSummaryGridInfo = JSON.parse(responseText.d);
        self.CurrentMappingSummaryInfo = mapSummaryGridInfo.RMappingSummary;

        $.ajax({
            url: self.ServiceURL + '/GetTagTemplates',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ fileName: 'RAddMapping.html' })
        }).then(function (responseText) {
            $(".RMapAddMapMain").remove();
            $("#" + RADMappingConfig.contentBodyId).append(responseText.d);
            $(".RMapAddMapContent").find("div[rmapcontrolname='ReplaceExisting']").removeClass("RMapSetupDisplayNone");
            $(".RMapAddMapContent").find(".btnRMapAddMapUpdate")[0].value = "Update Mapping"
            self.PreFillMappingSummaryDetails(event);
            self.BindEventsAddUpdateMapSummary(event, false);
            self.BuildUploadControl();
        });

    });


    

}

RADMappingConfiguration.prototype.PreFillMappingSummaryDetails = function(event){
    var self = this;

    var mappingNameControl = $(event.target).closest(".RMapInfoChild").find(".RMapInfoName");
    var mappingName = mappingNameControl.html().trim();
    $(".RMapAddMapContent").find(".RMapAddMapHeader").html("Update Mapping Summary: " + mappingName);
    var isUserDefined = ($(event.target).closest(".RMapInfoChild").find(".RMapInfoType").html().trim() == "User Defined") ? true : false;
    var mappingDescription = mappingNameControl.attr("rmapmappingdescription") == null ? "" : mappingNameControl.attr("rmapmappingdescription");
    $(".RMapAddMapContent").find("div[rmapcontrolname='MappingName']").find(".RMapAddMapInputValue").html(mappingName);
    $(".RMapAddMapContent").find("div[rmapcontrolname='MappingName']").css({ "pointer-events": "none" });
    $(".RMapAddMapContent").find("div[rmapcontrolname='MappingDescription']").find(".RMapAddMapInputValue").html(mappingDescription);
    if (isUserDefined) {
        $(".RMapAddMapContent").find("div[rmapcontrolname='MappingType']").find("div:contains('User Defined')").addClass("RMapToogleButtonValueHighLight");
        $(".RMapAddMapContent").find("div[rmapcontrolname='MappingType']").find("div:contains('Reference Master')").removeClass("RMapToogleButtonValueHighLight");
        $(".RMapAddMapContent").find("div[rmapcontrolname='UploadFile']").removeClass("RMapSetupDisplayNone")
    }
    else {
        $(".RMapAddMapContent").find("div[rmapcontrolname='MappingType']").find("div:contains('Reference Master')").addClass("RMapToogleButtonValueHighLight");
        $(".RMapAddMapContent").find("div[rmapcontrolname='MappingType']").find("div:contains('User Defined')").removeClass("RMapToogleButtonValueHighLight");
        $(".RMapAddMapContent").find("div[rmapcontrolname='UploadFile']").addClass("RMapSetupDisplayNone");
        $(".RMapAddMapContent").find(".RMapAddMapRefMaster").removeClass("RMapSetupDisplayNone")
        $(".RMapAddMapContent").find("div[rmapcontrolname='EntityType']").removeClass("RMapSetupDisplayNone");
        $(".RMapAddMapContent").find("div[rmapcontrolname='LookupColumn']").removeClass("RMapSetupDisplayNone");
        $(".RMapAddMapContent").find("div[rmapcontrolname='TargetValue']").removeClass("RMapSetupDisplayNone");
        //$(".RMapAddMapContent").find("div[rmapcontrolname='EntityType']").find(".RMapSelectionText").html();
        //self.CurrentMappingSummaryInfo
        self.GetEntityTypes(event,true);
        self.GetEntityTypesAndLookups(event, self.CurrentMappingSummaryInfo.EntityTypeId, true);

    }
    $(".RMapAddMapContent").find("div[rmapcontrolname='MappingType']").css({ "pointer-events": "none" });
}



RADMappingConfiguration.prototype.OnDeleteMappingSummary = function (event) {
    var self = this;
    self.CurrentMappingSummaryId = $(event.target).closest(".RMapInfoChild").attr("rmappingsummaryid");
    var mappingName = $(event.target).closest(".RMapInfoChild").find(".RMapInfoName").html().trim();
    var mainDivtoAppend = $("#" + RADMappingConfig.contentBodyId);    
    self.CommonConfirmationAlert(mainDivtoAppend, mappingName, "mapping");

    $(".RMapAlertFooterButton").unbind().click(function (event) {
        if ($(event.target).html() == "Yes")
            self.DeleteMappingSummary(self.CurrentMappingSummaryId);
        $(".RMapAlertMain").remove();
    });
    
}

RADMappingConfiguration.prototype.DeleteMappingSummary = function (mappingSummaryId) {
    var self = this;

    $.ajax({
        url: self.ServiceURL + '/DeleteMapping',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ mappingId: mappingSummaryId })
    }).then(function (responseText) {
        if (responseText.d) {
            self.GetAllMapping(true);
        }
        else {

        }
    });
}

RADMappingConfiguration.prototype.OnAddMappingSummary = function (event) {
    var self = this;

    $.ajax({
        url: self.ServiceURL + '/GetTagTemplates',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ fileName: 'RAddMapping.html' })
    }).then(function (responseText) {
        $(".RMapAddMapMain").remove();
        $("#" + RADMappingConfig.contentBodyId).append(responseText.d);
        self.BindEventsAddUpdateMapSummary(event, true);
        self.BuildUploadControl();
    });
}

RADMappingConfiguration.prototype.BuildUploadControl = function (event) {
    var self = this;
    $(".RMapUploadFileDoc").empty();
    $(".RMapUploadFileDoc").append("<div style=\"text-indent: 8px;\"> </div>");
    $(".RMapUploadFileDoc").append("<div class=\"fa fa-upload RMapUploadButton\"></div>");
    $(".RMapUploadFileDoc").append("<div class=\"RMapDropDiv\" id=\"RadAttachmentButton\">"+
                                   "<div style=\"display:inline-block;\">DRAG AND DROP&nbsp&nbsp&nbsp&nbsp</div><div style=\"display:inline-block;\">"+
                                   "OR&nbsp&nbsp&nbsp&nbsp</div><div style=\"display:inline-block;text-decoration: underline;\">Browse</div>");

    $(".RMapUploadFileDoc").append("<div class=\"RMapParentDropDrag\" id=\"RMapParentDropDragId\"></div>");
    if ($('#RMapParentDropDragId').fileUpload != undefined) {
        $('#RMapParentDropDragId').fileUpload({
            'parentControlId': 'RMapParentDropDragId',
            'attachementControlId': 'RadAttachmentButton',
            'multiple': false,
            'debuggerDiv': '',
            'deleteEvent': function () {
            }
        });
    }

}

RADMappingConfiguration.prototype.BindEventsAddUpdateMapSummary = function (event, isAddMapSummmary) {
    var self = this;
    $(".RMapAddMapContent").unbind().click(function (event) {

        if ($(event.target).closest(".RMapToogleButtonValue").length > 0) {
            // identify property based on toogle clikk
            //based upon highlight call the event

            var currentControlName = $(event.target).closest(".RMapToggleButtonParent").attr("rmapcontrolname");
            if (currentControlName == "FileType") {
                if (!$(event.target).hasClass("RMapToogleButtonValueHighLight")) {
                    $(event.target).closest(".RMapToogleButtonValueParent").find(".RMapToogleButtonValue").removeClass("RMapToogleButtonValueHighLight");
                    $(event.target).addClass("RMapToogleButtonValueHighLight");
                    var currentFileType = $(event.target).html().trim();
                    if (currentFileType == "CSV") {
                        $(".RMapFileTypeContentMainCSV").removeClass("RMapSetupDisplayNone");
                        $(".RMapFileTypeContentMainCommon").removeClass("RMapSetupDisplayNone");
                    }
                    else {
                        $(".RMapFileTypeContentMainCSV").addClass("RMapSetupDisplayNone");
                        $(".RMapFileTypeContentMainCommon").removeClass("RMapSetupDisplayNone");

                    }
                }
            }
            else if (currentControlName == "MappingType") {
                if (!$(event.target).hasClass("RMapToogleButtonValueHighLight")) {
                    $(event.target).closest(".RMapToogleButtonValueParent").find(".RMapToogleButtonValue").removeClass("RMapToogleButtonValueHighLight");
                    $(event.target).addClass("RMapToogleButtonValueHighLight");
                    var currentMappingType = $(event.target).html().trim();
                    if (currentMappingType == "User Defined") {
                        $(".RMapAddMapUserDefined").removeClass("RMapSetupDisplayNone");
                        $(".RMapAddMapRefMaster").addClass("RMapSetupDisplayNone");
                        $(event.target).closest(".RMapAddMapContent").find("div[rmapcontrolname='UploadFile']").removeClass("RMapSetupDisplayNone");
                        //if ($(event.target).closest(".RMapAddMapContent").find("div[rmapcontrolname='UploadFile']")
                        //                   .find(".RMapSwitch").length > 0)
                            $(event.target).closest(".RMapAddMapContent").find("div[rmapcontrolname='UploadFile']")
                                       .find(".RMapSwitch").find("input")[0].checked = false;
                        $(".RMapAddMapUserDefined").addClass("RMapSetupDisplayNone");
                        $(".RMapAddMapUserDefined").find("div[rmapfiletype='CSV']").removeClass("RMapToogleButtonValueHighLight");
                        $(".RMapFileTypeContentMainCSV").addClass("RMapSetupDisplayNone");
                        $(".RMapFileTypeContentMainCommon").addClass("RMapSetupDisplayNone");
                        //$(event.target).closest(".RMapAddMapContent").find("div[rmapcontrolname='RecordDelimiter']").removeClass("RMapSetupDisplayNone");
                        
                    }
                    else {
                        $(".RMapAddMapUserDefined").addClass("RMapSetupDisplayNone");
                        $(".RMapAddMapRefMaster").removeClass("RMapSetupDisplayNone");                        
                        $(event.target).closest(".RMapAddMapContent").find("div[rmapcontrolname='UploadFile']").addClass("RMapSetupDisplayNone");
                        self.GetEntityTypes(event,false);
                    }
                }
            }
            
        }
        else if ($(event.target).closest(".RMapSlider").length > 0) {

            var currentControlName = $(event.target).closest(".RMapToggleParent").attr("rmapcontrolname");
            if (currentControlName == "UploadFile") {
                if ($(event.target).closest(".RMapSwitch").find("input")[0].checked == true) {

                    $(".RMapAddMapUserDefined").addClass("RMapSetupDisplayNone");
                    $(".RMapAddMapUserDefined").find("div[rmapfiletype='CSV']").removeClass("RMapToogleButtonValueHighLight");
                    $(".RMapFileTypeContentMainCSV").addClass("RMapSetupDisplayNone");
                    $(".RMapFileTypeContentMainCommon").addClass("RMapSetupDisplayNone");

                }
                else {
                    $(".RMapAddMapUserDefined").removeClass("RMapSetupDisplayNone");
                    $(".RMapAddMapUserDefined").find("div[rmapfiletype='CSV']").addClass("RMapToogleButtonValueHighLight");
                    $(".RMapFileTypeContentMainCSV").removeClass("RMapSetupDisplayNone");
                    $(".RMapFileTypeContentMainCommon").removeClass("RMapSetupDisplayNone");
                }
            }

            // identify property based on toogle clikk
            //based upon highlight call the event
            //$($(".RMapSwitch")[1]).find("input")[0].checked

        }

        else if ($(event.target).closest(".btnRMapAddMapUpdate").length > 0) {
            //Validate Data
            //Build MappingSummaryObject
            var isAddMappSummary = $(event.target).closest(".btnRMapAddMapUpdate")[0].value == "Add Mapping" ? true : false;
            var isMappingValid = self.ValidateMappingSummary(event);

            if (isMappingValid) {
                var mappingObject = self.GetMappingSummaryObject(event, isAddMappSummary);

                if (isAddMappSummary) {
                    var mappingObject = self.AddMappingSummary(event, mappingObject);
                }
                else {
                    var mappingObject = self.UpdateMappingSummary(event, mappingObject);
                }

            }

            
        }
        else if ($(event.target).closest(".RMapSelection").length > 0) {
            if ($(event.target).closest(".RMapEntitiesDropDownChild").length > 0) {
                if ($(event.target).closest(".RMapAddMapInputParent").attr("rmapcontrolname") == "EntityType") {
                    self.OnRMapEntitySelected(event);
                }
                else {
                    if ($(event.target).closest(".RMapAddMapInputParent").attr("rmapcontrolname") == "LookupColumn") {
                        var currentAttributeDisplayName = $(event.target).html().trim();
                        var currentAttributeRealName = $(event.target).attr("rmapattributerealname").trim();
                        $(event.target).closest(".RMapAddMapInputParent").find(".RMapSelectionText").html(currentAttributeDisplayName);
                        $(event.target).closest(".RMapAddMapInputParent").find(".RMapSelectionText").attr("rmapattributerealname", currentAttributeRealName);
                        $(".RMapEntitiesMainDiv").remove();
                    }
                    else {
                        self.OnRMapTargetValueSelected(event);
                    }
                }
            }
            else {
                if ($(event.target).closest(".RMapAddMapInputParent").attr("rmapcontrolname") == "EntityType") {
                    self.BindAllEntities(event);
                }
                else if ($(event.target).closest(".RMapAddMapInputParent").attr("rmapcontrolname") == "LookupColumn") {
                    self.BindUniqueAttributesForEntity(event);
                }
                else if ($(event.target).closest(".RMapAddMapInputParent").attr("rmapcontrolname") == "TargetValue") {
                    self.BindAllAttributesForEntity(event);
                }


            }
        }
        else if ($(event.target).closest(".btnRMapAddMapCancel,.RMapAddMapRemove").length > 0) {
            $(".RMapAddMapMain").remove();
        }
        else
        {
            $(".RMapEntitiesMainDiv").remove();
        }
    });
}

RADMappingConfiguration.prototype.OnRMapTargetValueSelected = function (event) {
    var self = this;    
    var textControl = $(event.target).closest(".RMapSelection")
                                     .find(".RMapSelectionText");
    if ($(event.target).closest(".RMapEntitiesDropDownChildParent").find(".RMapDropDownChildTick").hasClass("RMapSetupDisplayNone"))
        $(event.target).closest(".RMapEntitiesDropDownChildParent").find(".RMapDropDownChildTick").removeClass("RMapSetupDisplayNone");
    else
        $(event.target).closest(".RMapEntitiesDropDownChildParent").find(".RMapDropDownChildTick").addClass("RMapSetupDisplayNone");
    var existingItemObjectArray = [];
    $(event.target).closest(".RMapEntitiesMainDiv").find(".RMapEntitiesDropDownChildParent").each(function (index) {
        if (!$(this).find(".RMapDropDownChildTick").hasClass("RMapSetupDisplayNone")) {
            var SelectedAttribute = {};
            SelectedAttribute.SelectedAtrributeId = $(this).find(".RMapEntitiesDropDownChild").attr("rmapattributeid");
            SelectedAttribute.SelectedAttributeRealName = $(this).find(".RMapEntitiesDropDownChild").attr("rmapattributerealname");
            SelectedAttribute.SelectedAttributeDisplayName = $(this).find(".RMapEntitiesDropDownChild").html().trim();
            existingItemObjectArray.push(SelectedAttribute);
        }
    });
    self.SetAttributeTextandText(textControl, existingItemObjectArray);
}

RADMappingConfiguration.prototype.SetAttributeTextandText = function (currentControl, selectValuesArray) {
    var self = this; 
    var rmapAttributeIdRealNameText = "";
    var rmapAttributeIdDisplayNameText = "";
    if (selectValuesArray.length == 0) {
        currentControl.text("--Select--");
        currentControl.attr("rmapattributeidrealname", "");
        currentControl.attr("rmapattributeiddisplayname", "");
        return;
    }
    else {
        var dimensionsValuesArray = [];
        selectValuesArray.forEach(function (currentSelectedValue) {
            dimensionsValuesArray.push(currentSelectedValue.SelectedAttributeDisplayName);
            rmapAttributeIdRealNameText = rmapAttributeIdRealNameText + currentSelectedValue.SelectedAttributeRealName + "~";
            rmapAttributeIdDisplayNameText = rmapAttributeIdDisplayNameText + currentSelectedValue.SelectedAttributeDisplayName + "~";
        });
        currentControl.attr("rmapattributeidrealname", rmapAttributeIdRealNameText);
        currentControl.attr("rmapattributeiddisplayname", rmapAttributeIdDisplayNameText);
        if (selectValuesArray.length <= 2) {
            currentControl.text(dimensionsValuesArray.join());
        }
        else {
            var slicedArray = dimensionsValuesArray.slice(0, 2);
            var selectedText = slicedArray.join();
            selectedText = selectedText + " <span class='RMapNMoreClass'>" + (dimensionsValuesArray.length - 2).toString() + " More </span>";
            currentControl.html(selectedText);
        }
    }
}

RADMappingConfiguration.prototype.OnRMapEntitySelected = function (event) {
    var self = this;

    var currentEntityName = $(event.target).html().trim();
    var currentEntityId = $(event.target).attr("rmapentityid")
    $(event.target).closest(".RMapAddMapInputParent").find(".RMapSelectionText").html(currentEntityName);
    $(event.target).closest(".RMapAddMapInputParent").find(".RMapSelectionText").attr("rmapentityid", currentEntityId);
    $(event.target).closest(".RMapAddMapContent").find("div[rmapcontrolname='LookupColumn']").removeClass("RMapSetupDisplayNone");
    $(event.target).closest(".RMapAddMapContent").find("div[rmapcontrolname='TargetValue']").removeClass("RMapSetupDisplayNone");
    $(".RMapEntitiesMainDiv").remove();
    self.GetEntityTypesAndLookups(event, currentEntityId,false);

}

RADMappingConfiguration.prototype.GetEntityTypesAndLookups = function (event, currentEntityId, isUpdate) {
    var self = this;
    $.ajax({
        url: self.ServiceURL + '/GetAttributesForEntity',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ entityId: currentEntityId })
    }).then(function (responseText) {
        self.RMapAttributesForEntity = responseText.d;
        if (isUpdate && self.CurrentMappingSummaryInfo.IsRefMaster) {
            self.GetRealNameDisplayNameMapping(event, currentEntityId);
        }
        //Show Lookup and Target DropDowns
        //$(event.target).closest(".RMapAddMapContent").find("div[rmapcontrolname='LookupColumn']").removeClass("RMapSetupDisplayNone");
        //$(event.target).closest(".RMapAddMapContent").find("div[rmapcontrolname='TargetValue']").removeClass("RMapSetupDisplayNone");
    });
}

RADMappingConfiguration.prototype.GetRealNameDisplayNameMapping = function (event, currentEntityId) {

    var self = this;
    $.ajax({
        url: self.ServiceURL + '/GetRealNameDisplayNameMapping',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ entityTypeId: currentEntityId })
    }).then(function (responseText) {
        if (responseText.d != "") {
            var realNameDisplayNameMapping = responseText.d;

            var currentLookUpColumn = $.grep(realNameDisplayNameMapping, function (e)
            { return e.Key == self.CurrentMappingSummaryInfo.LookUpColumnPosition; });

            var ValueColumnPositionArray = self.CurrentMappingSummaryInfo.ValueColumnPosition.split(",");
            if (ValueColumnPositionArray.indexOf("") >= 0)
                ValueColumnPositionArray.splice(ValueColumnPositionArray.indexOf(""), 1);

            
            var existingItemObjectArray = [];
            ValueColumnPositionArray.forEach(function (currentItem) {
                var currentColumnMapping = $.grep(realNameDisplayNameMapping, function (e)
                { return e.Key == currentItem; });
                var SelectedAttribute = {};
                SelectedAttribute.SelectedAttributeRealName = currentColumnMapping[0].Key;
                SelectedAttribute.SelectedAttributeDisplayName = currentColumnMapping[0].Value;
                existingItemObjectArray.push(SelectedAttribute);
            });
            
            var targetTextControl = $(".RMapAddMapContent").find("div[rmapcontrolname='TargetValue']").find(".RMapSelectionText");
            self.SetAttributeTextandText(targetTextControl, existingItemObjectArray);
            $(".RMapAddMapContent").find("div[rmapcontrolname='LookupColumn']").find(".RMapSelectionText").html(currentLookUpColumn[0].Value);
            $(".RMapAddMapContent").find("div[rmapcontrolname='LookupColumn']").find(".RMapSelectionText").attr("rmapattributerealname", currentLookUpColumn[0].Key);
            self.bindQTip();
        }
    });

}


RADMappingConfiguration.prototype.BindUniqueAttributesForEntity = function (event) {
    var self = this;

    if (self.RMapAttributesForEntity != null) {
        var uniqueAttributes = $.grep(self.RMapAttributesForEntity.MasterAttributes, function (e)
        { return e.IsUnique == true; });

        if (uniqueAttributes.length > 0) {
            $(".RMapEntitiesMainDiv").remove();

            var $rmapEntitiesMainDiv = $("<div>", {
                class: "RMapEntitiesMainDiv"
            });

            var $currentRowEntityParent = $("<div>", {
                class: "RMapEntitiesDropDownParent"
            });

            for (var i = 0; i < uniqueAttributes.length; i++) {
                if (uniqueAttributes[i].Name != "Entity Code") {
                    var $currentRowEntity = $("<div>", {
                        class: "RMapEntitiesDropDownChild",
                        text: uniqueAttributes[i].Name
                    });
                    $currentRowEntity.attr("rmapattributeid", uniqueAttributes[i].AttributeId)
                    $currentRowEntity.attr("rmapattributerealname", uniqueAttributes[i].AttributeRealName)
                    $currentRowEntityParent.append($currentRowEntity);
                }
            }

            $rmapEntitiesMainDiv.append($currentRowEntityParent);
            //$(".RMapSelection").append($rmapEntitiesMainDiv);
            $(event.target).closest(".RMapSelection").append($rmapEntitiesMainDiv);

        }
    }
}

RADMappingConfiguration.prototype.BindAllAttributesForEntity = function (event) {
    var self = this;

    var rmapAttributeIdRealNameText = $(event.target).attr("rmapattributeidrealname");
    var existingDimesionsArray = [];
    if (rmapAttributeIdRealNameText != null) { 
        var existingDimesionsArray = rmapAttributeIdRealNameText.split("~");
        if (existingDimesionsArray.indexOf("") >= 0)
            existingDimesionsArray.splice(existingDimesionsArray.indexOf(""), 1);
    }     

    if (self.RMapAttributesForEntity != null) {
        $(".RMapEntitiesMainDiv").remove();

        var $rmapEntitiesMainDiv = $("<div>", {
            class: "RMapEntitiesMainDiv"
        });

        var $currentRowEntityParent = $("<div>", {
            class: "RMapEntitiesDropDownParent"
        });

        for (var i = 0; i < self.RMapAttributesForEntity.MasterAttributes.length; i++) {

            if (self.RMapAttributesForEntity.MasterAttributes[i].Name != "Entity Code") {

                var $currentRowParent = $("<div>", {
                    class: "RMapEntitiesDropDownChildParent"
                });

                var $currentRowEntity = $("<div>", {
                    class: "RMapEntitiesDropDownChild",
                    text: self.RMapAttributesForEntity.MasterAttributes[i].Name
                });
                $currentRowEntity.attr("rmapattributeid", self.RMapAttributesForEntity.MasterAttributes[i].AttributeId)
                $currentRowEntity.attr("rmapattributerealname", self.RMapAttributesForEntity.MasterAttributes[i].AttributeRealName)
                var $currentRowEntityTick = $("<div>", {
                    class: "RMapSetupDisplayNone RMapDropDownChildTick fa fa-check"
                });
                $currentRowParent.append($currentRowEntity);
                $currentRowParent.append($currentRowEntityTick);
                if (existingDimesionsArray.indexOf(self.RMapAttributesForEntity.MasterAttributes[i].AttributeRealName) != -1)
                    $currentRowEntityTick.removeClass("RMapSetupDisplayNone");
                $currentRowEntityParent.append($currentRowParent);

            }
        }

        $rmapEntitiesMainDiv.append($currentRowEntityParent);
        //$(".RMapSelection").append($rmapEntitiesMainDiv);
        $(event.target).closest(".RMapSelection").append($rmapEntitiesMainDiv);

    }
}

RADMappingConfiguration.prototype.BindAllEntities = function (event) {
    var self = this;
    
    $(".RMapEntitiesMainDiv").remove();    

    var $rmapEntitiesMainDiv = $("<div>", {
        class: "RMapEntitiesMainDiv"
    });

    var $currentRowEntityParent = $("<div>", {
        class: "RMapEntitiesDropDownParent"
    });

    for (var i = 0; i < self.RMapEntityTypes.length; i++) {

        var $currentRowEntity = $("<div>", {
            class: "RMapEntitiesDropDownChild",
            text: self.RMapEntityTypes[i].EntityTypeDisplayName
        });
        $currentRowEntity.attr("rmapentityid", self.RMapEntityTypes[i].EntityTypeId)
        $currentRowEntityParent.append($currentRowEntity);
    }
    
    $rmapEntitiesMainDiv.append($currentRowEntityParent);
    //$(".RMapSelection").append($rmapEntitiesMainDiv);
    $(event.target).closest(".RMapSelection").append($rmapEntitiesMainDiv);
}

RADMappingConfiguration.prototype.GetEntityTypes = function (event, isUpdate) {
    var self = this;
    $.ajax({
        url: self.ServiceURL + '/GetEntityTypes',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        self.RMapEntityTypes = responseText.d;
        if (isUpdate && self.CurrentMappingSummaryInfo.IsRefMaster) {
            var currentEntity = $.grep(self.RMapEntityTypes, function (e)
            { return e.EntityTypeId == self.CurrentMappingSummaryInfo.EntityTypeId; });
            $(".RMapAddMapContent").find("div[rmapcontrolname='EntityType']").find(".RMapSelectionText").html(currentEntity[0].EntityTypeDisplayName);
            $(".RMapAddMapContent").find("div[rmapcontrolname='EntityType']").find(".RMapSelectionText").attr("rmapentityid", currentEntity[0].EntityTypeId);
        }
    });
}

RADMappingConfiguration.prototype.ValidateMappingSummary = function (event) {
    var self = this;
    var isDataValid = true;
    var rmapAddMapContent = $(event.target).closest(".RMapAddMapContent");
    var isRefMaster = rmapAddMapContent.find("div[rmapcontrolname='MappingType']").find(".RMapToogleButtonValueHighLight").html().trim()
                                      == "User Defined" ? false : true;
    if (isRefMaster)
        isDataValid = self.ValidateRefMappingSummary(event, rmapAddMapContent, isDataValid);
    else
        isDataValid = self.ValidateUserMappingSummary(event, rmapAddMapContent, isDataValid);
    return isDataValid;
}

RADMappingConfiguration.prototype.ValidateRefMappingSummary = function (event, rmapAddMapContent, isDataValid) {

    var self = this;

    if (rmapAddMapContent.find("div[rmapcontrolname='MappingName']").find(".RMapAddMapInputValue").html().trim() =="") {
        isDataValid = false;
        rmapAddMapContent.find("div[rmapcontrolname='MappingName']").find(".RMapAddMapInputValidation").removeClass("RMapSetupDisplayNone");
    }
    else {
        rmapAddMapContent.find("div[rmapcontrolname='MappingName']").find(".RMapAddMapInputValidation").addClass("RMapSetupDisplayNone");
    }
        
    

    var allManditoryControls = rmapAddMapContent.find(".RMapAddMapRefMaster").find("div[ismandiatory='true']");
    var activeManditoryControls = [];
    for (var i = 0; i < allManditoryControls.length; i++) {
        if (!$(allManditoryControls[i]).hasClass("RMapSetupDisplayNone"))
            activeManditoryControls.push(allManditoryControls[i]);
    }

    for (var i = 0; i < activeManditoryControls.length; i++) {

        if ($(activeManditoryControls[i]).find(".RMapSelectionText").html().trim() == "--Select--") {
            if (isDataValid)
                isDataValid = false;
            $(activeManditoryControls[i]).find(".RMapAddMapInputValidation").removeClass("RMapSetupDisplayNone");
            $(activeManditoryControls[i]).find(".RMapAddMapInputValidation").attr("title",
                                            $(activeManditoryControls[i]).attr("rmapcontroldisplayname")
                                            + " can't be blank");
        }
        else
            $(activeManditoryControls[i]).find(".RMapAddMapInputValidation").addClass("RMapSetupDisplayNone");

    }

    return isDataValid;
    

}

RADMappingConfiguration.prototype.ValidateUserMappingSummary = function (event, rmapAddMapContent, isDataValid) {

    var self = this;

    if (rmapAddMapContent.find("div[rmapcontrolname='MappingName']").find(".RMapAddMapInputValue").html().trim() == "") {
        isDataValid = false;
        rmapAddMapContent.find("div[rmapcontrolname='MappingName']").find(".RMapAddMapInputValidation").removeClass("RMapSetupDisplayNone");
    }
    else {
        rmapAddMapContent.find("div[rmapcontrolname='MappingName']").find(".RMapAddMapInputValidation").addClass("RMapSetupDisplayNone");
    }

    var isUploadFile = $(".RMapAddMapContent").find("div[rmapcontrolname='UploadFile']").find(".RMapSwitch").find("input")[0].checked;

    if (isUploadFile) {
        var allManditoryControls = [];

        var fileType = rmapAddMapContent.find("div[rmapcontrolname='FileType']").find(".RMapToogleButtonValueHighLight").html().trim();

        allManditoryControls = (fileType == "CSV") ? rmapAddMapContent.find(".RMapAddMapUserDefined").find("div[ismandiatory='true']")
                                                   : rmapAddMapContent.find(".RMapAddMapUserDefined").find(".RMapFileTypeContentMainCommon").
                                                     find("div[ismandiatory='true']");
        var activeManditoryControls = [];
        for (var i = 0; i < allManditoryControls.length; i++) {
            if (!$(allManditoryControls[i]).hasClass("RMapSetupDisplayNone"))
                activeManditoryControls.push(allManditoryControls[i]);
        }

        for (var i = 0; i < activeManditoryControls.length; i++) {

            if ($(activeManditoryControls[i]).find(".RMapAddMapInputValue").html().trim() == "") {
                if (isDataValid)
                    isDataValid = false;
                $(activeManditoryControls[i]).find(".RMapAddMapInputValidation").removeClass("RMapSetupDisplayNone");
                $(activeManditoryControls[i]).find(".RMapAddMapInputValidation").attr("title",
                                                $(activeManditoryControls[i]).attr("rmapcontroldisplayname")
                                                + " can't be blank");
            }
            else
                $(activeManditoryControls[i]).find(".RMapAddMapInputValidation").addClass("RMapSetupDisplayNone");

        }
    }

    return isDataValid;

}

RADMappingConfiguration.prototype.GetMappingSummaryObject = function (event, isAddMappSummary) {
    var self = this;
    var rmapAddMapContent = $(event.target).closest(".RMapAddMapContent");

    var RMappingSummaryInfo = {};
    if (!isAddMappSummary)
        RMappingSummaryInfo.MappingSummaryID = self.CurrentMappingSummaryId;
    RMappingSummaryInfo.MappingName = rmapAddMapContent.find("div[rmapcontrolname='MappingName']").find(".RMapAddMapInputValue").html().trim();
    RMappingSummaryInfo.MappingDescription = rmapAddMapContent.find("div[rmapcontrolname='MappingDescription']").find(".RMapAddMapInputValue").html().trim();
    RMappingSummaryInfo.IsRefMaster = rmapAddMapContent.find("div[rmapcontrolname='MappingType']").find(".RMapToogleButtonValueHighLight").html().trim()
                                      == "User Defined" ? false : true;
    //RMappingSummaryInfo.AccountName = "";
    

    if (RMappingSummaryInfo.IsRefMaster) {
        RMappingSummaryInfo.EntityTypeId = rmapAddMapContent.find("div[rmapcontrolname='EntityType']").find(".RMapSelectionText").attr("rmapentityid");
        //RMappingSummaryInfo.LookUpColumnPosition = rmapAddMapContent.find("div[rmapcontrolname='LookupColumn']").find(".RMapSelectionText").html().trim();
        //RMappingSummaryInfo.ValueColumnPosition = rmapAddMapContent.find("div[rmapcontrolname='TargetValue']").find(".RMapSelectionText").html().trim();
        RMappingSummaryInfo.LookUpColumnPosition = rmapAddMapContent.find("div[rmapcontrolname='LookupColumn']")
                                                   .find(".RMapSelectionText").attr("rmapattributerealname").trim();
        RMappingSummaryInfo.ValueColumnPosition = self.GetValueColumnPosition(event);


    }
        

    var isFileUpload = rmapAddMapContent.find("div[rmapcontrolname='UploadFile']").find(".RMapSwitch").find("input")[0].checked

    if (isFileUpload) {
        RMappingSummaryInfo.FileType = rmapAddMapContent.find("div[rmapcontrolname='FileType']").find(".RMapToogleButtonValueHighLight").html().trim();
        if (RMappingSummaryInfo.FileType == "CSV") {
            //RMappingSummaryInfo.IsPairedEscape = rmapAddMapContent.find("div[rmapfiletype='PairedEscape']").find(".RMapAddMapInputValue").html().trim();
            //RMappingSummaryInfo.RecordXPath = rmapAddMapContent.find("div[rmapfiletype='MappingDescription']").find(".RMapAddMapInputValue").html().trim()
            RMappingSummaryInfo.RecordDelimiter = rmapAddMapContent.find("div[rmapcontrolname='RecordDelimiter']").find(".RMapAddMapInputValue").html().trim();
            RMappingSummaryInfo.FieldDelimiter = rmapAddMapContent.find("div[rmapcontrolname='FieldDelimiter']").find(".RMapAddMapInputValue").html().trim();
            RMappingSummaryInfo.CommentChar = rmapAddMapContent.find("div[rmapcontrolname='CommentChar']").find(".RMapAddMapInputValue").html().trim();
            RMappingSummaryInfo.PairedEscape = rmapAddMapContent.find("div[rmapcontrolname='PairedEscape']").find(".RMapAddMapInputValue").html().trim();
            RMappingSummaryInfo.SingleEscape = rmapAddMapContent.find("div[rmapcontrolname='SingleEscape']").find(".RMapAddMapInputValue").html().trim();
            RMappingSummaryInfo.IsRemoveQuotes = rmapAddMapContent.find("div[rmapcontrolname='RemoveQuotes']").find(".RMapSwitch").find("input")[0].checked;
        }
        RMappingSummaryInfo.ExcludeRegex = rmapAddMapContent.find("div[rmapcontrolname='ExcludeRegex']").find(".RMapAddMapInputValue").html().trim();
        RMappingSummaryInfo.LookUpColumnPosition = rmapAddMapContent.find("div[rmapcontrolname='LookupValuePosition']").find(".RMapAddMapInputValue").html().trim();
        RMappingSummaryInfo.ValueColumnPosition = rmapAddMapContent.find("div[rmapcontrolname='ValuePosition']").find(".RMapAddMapInputValue").html().trim();
    }
    return RMappingSummaryInfo;
}

RADMappingConfiguration.prototype.GetValueColumnPosition = function(event)
{
    var self = this;
    var rmapAddMapContent = $(event.target).closest(".RMapAddMapContent");
    var rmapAttributeIdRealNameText = rmapAddMapContent.find("div[rmapcontrolname='TargetValue']")
                                                                        .find(".RMapSelectionText").attr("rmapattributeidrealname");
    var ValuesArray = [];
    if (rmapAttributeIdRealNameText != null) {
        var ValuesArray = rmapAttributeIdRealNameText.split("~");
        if (ValuesArray.indexOf("") >= 0)
            ValuesArray.splice(ValuesArray.indexOf(""), 1);
        return ValuesArray.join();
    }
}

RADMappingConfiguration.prototype.UpdateMappingSummary = function (event, mappingObject) {

    var self = this;
    var rmapAddMapContent = $(event.target).closest(".RMapAddMapContent");
    var isFileUpload = rmapAddMapContent.find("div[rmapcontrolname='UploadFile']").find(".RMapSwitch").find("input")[0].checked;
    var isReplaceExisting = rmapAddMapContent.find("div[rmapcontrolname='ReplaceExisting']").find(".RMapSwitch").find("input")[0].checked;
    var cacheKey = "RMapParentDropDragId";

    var mappingObjectString = JSON.stringify(mappingObject);

    $.ajax({
        url: self.ServiceURL + '/UpdateMapping',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({
            isRefM: mappingObject.IsRefMaster, MappingSummaryInfo: mappingObjectString,
            isUpload: isFileUpload, replaceExisting : isReplaceExisting, cacheKey: cacheKey
        })
    }).then(function (responseText) {
        if (responseText.d == "") {
            $(".RMapAddMapMain").remove();
            self.GetAllMapping(true);
        }
        else {
            $(".RMapAddMapMain").find(".RMapAddMapErrorParent").removeClass("RMapSetupDisplayNone");
            $(".RMapAddMapError").html(responseText.d);
        }
    });

}

RADMappingConfiguration.prototype.AddMappingSummary = function (event, mappingObject) {
    var self = this;
    var rmapAddMapContent = $(event.target).closest(".RMapAddMapContent");
    var isFileUpload = rmapAddMapContent.find("div[rmapcontrolname='UploadFile']").find(".RMapSwitch").find("input")[0].checked;
    var cacheKey = "RMapParentDropDragId";

    var mappingObjectString = JSON.stringify(mappingObject);

    $.ajax({
        url: self.ServiceURL + '/AddMapping',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ mappingSummary: mappingObjectString, isRefM : mappingObject.IsRefMaster, isUpload: isFileUpload, cacheKey: cacheKey })
    }).then(function (responseText) {
        if (responseText.d == "") {
            $(".RMapAddMapMain").remove();
            self.GetAllMapping(true);
        }
        else {
            $(".RMapAddMapMain").find(".RMapAddMapErrorParent").removeClass("RMapSetupDisplayNone");
            $(".RMapAddMapError").html(responseText.d);
        }
    });



}


RADMappingConfiguration.prototype.GetAllMapping = function (showAllAccounts) {
    var self = this;

    $.ajax({
        url: self.ServiceURL + '/GetMappingSummary',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ showAllAccounts: showAllAccounts })
    }).then(function (responseText) {
        var mappingSummary = JSON.parse(responseText.d);
        if (mappingSummary.length > 0) {
            self.AllMappingsInfo = mappingSummary;
            self.BindAllMappings(mappingSummary);
        }
        else {
            // no mappings found
        }

    });

}

RADMappingConfiguration.prototype.BindAllMappings = function (mappingSummary) {
    var self = this;
    $(".RMapSetupLeftContentMain").empty();
    var firstMappingId = mappingSummary[0].MappingSummaryID;
    self.CurrentMappingSummaryId = firstMappingId;
    var parentElement = $(".RMapSetupLeftContentMain");
    for (var i = 0; i < mappingSummary.length; i++) {
        var isFirtElement = (i == 0) ? true : false;
        var mappingSummaryElement = null;
        var mappingSummaryElement = $("#RMapInfoParentTemplate").find(".RMapInfoParent").clone();
        parentElement.append(mappingSummaryElement);
        self.BindCurrentMapping(mappingSummaryElement, mappingSummary[i], isFirtElement)
    }
    self.GetMappingDetailsbyId(firstMappingId, true);

}

RADMappingConfiguration.prototype.BindCurrentMapping = function (mappingSummaryElement, currentMappingSummary, isFirtElement) {
    var self = this;   
    if (isFirtElement) {
        mappingSummaryElement.find(".RMapInfoChild").addClass("RMapInfoActive");
        mappingSummaryElement.find(".RMapInfoChild").append("<div class=\"fa fa-caret-right RMapSetupArrowRight\"></div>");
    }
    self.CurrentMappingName = currentMappingSummary.MappingName;
    mappingSummaryElement.find(".RMapInfoChild").attr("rmappingsummaryid", currentMappingSummary.MappingSummaryID);
    mappingSummaryElement.find(".RMapInfoName").html(currentMappingSummary.MappingName);
    mappingSummaryElement.find(".RMapInfoName").attr("title", currentMappingSummary.MappingName);
    var mappingDescrption = (currentMappingSummary.MappingDescription == null) ? "" : currentMappingSummary.MappingDescription;
    mappingSummaryElement.find(".RMapInfoName").attr("rmapmappingdescription", mappingDescrption);
    var rMapInfoType = currentMappingSummary.IsRefMaster == true ? "Reference Master" : "User Defined"
    mappingSummaryElement.find(".RMapInfoType").html(rMapInfoType);
    mappingSummaryElement.find(".RMapInfoType").attr("title", rMapInfoType);

   
}

RADMappingConfiguration.prototype.GetMappingDetailsbyId = function (mappingSummaryId, isExcecuteGrid) {
    var self = this;
    $("#RADMappingDetailsGridDiv").empty();
    $('#RMapSetupLoaderDiv').removeClass("RMapSetupDisplayNone");
    $.ajax({
        url: self.ServiceURL + '/GetMappingDetails',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ mappingSummaryId: mappingSummaryId, isExcecuteGrid: isExcecuteGrid })
    }).then(function (responseText) {
        //if (responseText.d != "") {
        //    var gridInfo = JSON.parse(responseText.d);
        //    gridInfo.GridId = "RADMappingDetailsGridDiv";
        //    neogridloader.create("RADMappingDetailsGridDiv", "Test", gridInfo, "");
        //    if (gridInfo.IsGridHasRecords == false) {
        //        self.OnRADGridRenderComplete();
        //    }
        //}
        //else {
        //    //error occured
        //    self.OnRADGridRenderComplete();
            
        //}

        if (responseText.d != "") {
            var mapSummaryGridInfo = JSON.parse(responseText.d);
            //var gridInfo = JSON.parse(responseText.d);
            var gridInfo = mapSummaryGridInfo.RGridInfo;
            self.CurrentMappingSummaryInfo = mapSummaryGridInfo.RMappingSummary;
            if (self.CurrentMappingSummaryInfo.IsRefMaster)
                $(".RMapSetupMain").find(".RMapSetupAddMapParent").addClass("RMapSetupDisplayNone");
            else
                $(".RMapSetupMain").find(".RMapSetupAddMapParent").removeClass("RMapSetupDisplayNone");
            var headerText = (self.CurrentMappingSummaryInfo.IsRefMaster ? "Reference Master" : "User Defined") + " Mapping details: "
                             + self.CurrentMappingSummaryInfo.MappingName
            $(".RMapSetupRightHeaderLeft").html(headerText);
            gridInfo.GridId = "RADMappingDetailsGridDiv";
            neogridloader.create("RADMappingDetailsGridDiv", "Test", gridInfo, "");

            //if (gridInfo.IsGridHasRecords == false) {
            //    self.OnRADGridRenderComplete();
            //}

        }
        $('#RMapSetupLoaderDiv').addClass("RMapSetupDisplayNone");
        
    });
}

RADMappingConfiguration.prototype.OnRADGridRenderComplete = function (eventType,gridid) {
    var self = this;
    //if ($(document.body).find('#RMapSetupLoaderDiv').length > 0)
    //$(document.body).find('#RMapSetupLoaderDiv').addClass("RMapSetupDisplayNone");
    
    if (eventType == "ClientSideBinding")
        $find("RADMappingDetailsGridDiv").get_GridInfo().Height = ($(".RMapDetailsGridContentParent").height() - 130) + "px";
    

    //$(".btnRMapDetailUpdate").unbind().click(function (event) {
    //    self.OnUpdateMappingDetails(event);
    //});

    //$(".btnRMapDetailDelete").unbind().click(function (event) {
    //    self.OnDeleteMappingDetails(event);
    //});






}

RADMappingConfiguration.prototype.OnUpdateMappingDetails = function (event) {
    var self = this;
    var currentMappingId = $(event.target).closest(".xlneocommonrowcss").attr('idcolumnvalue');
    self.CurrentMappingDetailsId = currentMappingId;
    var currentMappingLookupVal = $(event.target).closest(".xlneocommonrowcss").find("div[columnname='lookup_value']").html().trim();
    self.CurrentMappingLookupValue = currentMappingLookupVal;
    var currentMappingTargetVal = $(event.target).closest(".xlneocommonrowcss").find("div[columnname='target_value']").html().trim();
    $.ajax({
        url: self.ServiceURL + '/GetTagTemplates',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ fileName: 'RUpdateMappingDetails.html' })
    }).then(function (responseText) {
        $(".RMapDetailsUpdateMain").remove();
        $("#" + RADMappingConfig.contentBodyId).append(responseText.d);
        $(".RLookupColmnValue").html(currentMappingLookupVal);
        $(".RTargetColmnValue").html(currentMappingTargetVal);
        $(".RMapDetailsUpdateContent").find(".RMapDetailsHeader").html("Update mapping details for: " + currentMappingLookupVal);
        self.BindEventsUpdateMappings();
    });

}

RADMappingConfiguration.prototype.BindEventsUpdateMappings = function () {
    var self = this;

    $(".RMapDetailsUpdateMain").unbind().click(function (event) {
        
        if ($(event.target).closest(".btnRMapDetailsCancel").length > 0) {
            self.CloseMappingUpateScreen(event);
        }
        else if ($(event.target).closest(".btnRMapDetailsUpdate").length > 0) {
            if ((event.target).closest(".btnRMapDetailsUpdate").value.trim() == "Update")
                self.UpdateMappingDetails(event);
            else
                self.AddMappingDetails(event);
        }

        else if ($(event.target).closest(".RMapDetailsRemove").length > 0) {
            self.CloseMappingUpateScreen(event);
        }
    });
}

RADMappingConfiguration.prototype.UpdateMappingDetails = function (event) {
    var self = this;

    if ($(".RLookupColmnValue").html().trim() != "" && $(".RTargetColmnValue").html().trim() != "") {
        $(".RMapDetailsErrorParent").addClass("RMapSetupDisplayNone");
        $(".RMapDetailsError").html("");

        var mappingDetailsObject = self.GetMappingDetails(event);
        var isLookupChanged = self.CurrentMappingLookupValue.toLowerCase() == mappingDetailsObject.LookupValue.toLowerCase() ? false : true;
        var mappingDetailsObjectString = JSON.stringify(mappingDetailsObject);

        $.ajax({
            url: self.ServiceURL + '/UpdateMappingDetails',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ MappingDetail: mappingDetailsObjectString, isLookupChanged: isLookupChanged })
        }).then(function (responseText) {
            if (responseText.d == "") {
                $(".RMapDetailsErrorParent").addClass("RMapSetupDisplayNone");
                $(".RMapDetailsError").html("");
                self.CloseMappingUpateScreen(event);
                self.GetMappingDetailsbyId(mappingDetailsObject.MappingSummaryID, true);
            }
            else {
                $(".RMapDetailsErrorParent").removeClass("RMapSetupDisplayNone");
                $(".RMapDetailsError").html(responseText.d);
            }
           
        });

    }
    else {
        $(".RMapDetailsErrorParent").removeClass("RMapSetupDisplayNone");
        $(".RMapDetailsError").html("Lookup value and Target Value can't be empty.")

    }

}



RADMappingConfiguration.prototype.GetMappingDetails = function (event) {
    var self = this;
    var rMappingDetailsInfo = {};
    rMappingDetailsInfo.LookupValue = $(".RLookupColmnValue").html().trim()
    rMappingDetailsInfo.TargetValue = $(".RTargetColmnValue").html().trim()
    rMappingDetailsInfo.MappingSummaryID = self.CurrentMappingSummaryId;
    rMappingDetailsInfo.MappingDetailsID = self.CurrentMappingDetailsId;
    return rMappingDetailsInfo;
}



RADMappingConfiguration.prototype.OnDeleteMappingDetails = function (event) {
    var self = this;
    var mainDivtoAppend = $("#" + RADMappingConfig.contentBodyId);
    var currentMappingId = $(event.target).closest(".xlneocommonrowcss").attr('idcolumnvalue');
    self.CurrentMappingDetailsId = currentMappingId;
    var currentMappingLookupVal = $(event.target).closest(".xlneocommonrowcss").find("div[columnname='lookup_value']").html().trim();
    self.CommonConfirmationAlert(mainDivtoAppend, currentMappingLookupVal, "mapping detail");

    $(".RMapAlertFooterButton").unbind().click(function (event) {
        if ($(event.target).html() == "Yes")
            self.DeleteMappingDetails(event);
        $(".RMapAlertMain").remove();
    });

    

    //self.DeleteMappingDetails(event);

}

RADMappingConfiguration.prototype.DeleteMappingDetails = function (event) {
    var self = this;
    $.ajax({
        url: self.ServiceURL + '/DeleteMappingDetail',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ mappingSummaryID: self.CurrentMappingSummaryId, mappingDetailId: self.CurrentMappingDetailsId })
    }).then(function (responseText) {
        self.GetMappingDetailsbyId(self.CurrentMappingSummaryId, true);
    });

}

RADMappingConfiguration.prototype.CloseMappingUpateScreen = function (event) {
    var self = this;
    $(".RMapDetailsUpdateMain").remove();
    $(".RLookupColmnValue").html("");
    $(".RTargetColmnValue").html("");
    $(".RMapDetailsError").html("");
    $(".RMapDetailsErrorParent").addClass("RMapSetupDisplayNone");
}

RADMappingConfiguration.prototype.OnAddMappingDetails = function (event) {
    var self = this;
    $.ajax({
        url: self.ServiceURL + '/GetTagTemplates',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ fileName: 'RUpdateMappingDetails.html' })
    }).then(function (responseText) {
        $(".RMapDetailsUpdateMain").remove();
        $("#" + RADMappingConfig.contentBodyId).append(responseText.d);
        $(".RMapDetailsHeader").html("Add new mapping details for : " + self.CurrentMappingName)
        $(".btnRMapDetailsUpdate")[0].value = "Add New Mapping";
        self.BindEventsUpdateMappings();
    });

    
}

RADMappingConfiguration.prototype.AddMappingDetails = function (event) {
    var self = this;

    if ($(".RLookupColmnValue").html().trim() != "" && $(".RTargetColmnValue").html().trim() != "") {
        $(".RMapDetailsErrorParent").addClass("RMapSetupDisplayNone");
        $(".RMapDetailsError").html("");
        var mappingDetailsObject = self.GetMappingDetails(event);
        var mappingDetailsObjectString = JSON.stringify(mappingDetailsObject);

        

        $.ajax({
            url: self.ServiceURL + '/AddMappingDetails',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ mappingDetails: mappingDetailsObjectString })
        }).then(function (responseText) {
            if (responseText.d == "") {
                $(".RMapDetailsErrorParent").addClass("RMapSetupDisplayNone");
                $(".RMapDetailsError").html("");
                self.CloseMappingUpateScreen(event);
                self.GetMappingDetailsbyId(mappingDetailsObject.MappingSummaryID, true);
            }
            else {
                $(".RMapDetailsErrorParent").removeClass("RMapSetupDisplayNone");
                $(".RMapDetailsError").html(responseText.d)
            }

        });

    }
    else {
        $(".RMapDetailsErrorParent").removeClass("RMapSetupDisplayNone");
        $(".RMapDetailsError").html("Lookup value and Target Value can't be empty.")

    }

}

RADMappingConfiguration.prototype.CommonConfirmationAlert = function (mainDivforAlert, toDeleteItem, module) {

    var self = this;

    //$("#RADBcalenderAlertParentDiv").remove();
    //mainDivforAlert.append("<div id=\"RADBCalenderAlertParentDiv\" class=\"RADBCalenderAlertPopUpParent\"></div>");
    //$("#RADBCalenderAlertParentDiv").append("<div class=\"RADBCalenderAlertUpperDiv\"></div>");
    //$("#RADBCalenderAlertParentDiv").append("<div class=\"RADBCalenderPopUpError\"></div>");
    //$("#RADBCalenderAlertParentDiv").append("<div class=\"fa fa-exclamation-circle RADBCalenderDivLeft RADBCalenderAlert\"></div>");

    //$("#RADBCalenderAlertParentDiv").append("<div class=\"RADBCalenderErrorDivRight\"></div>")
    //$(".RADBCalenderErrorDivRight").append("<div class=\"RADBCalenderErrorDivHeading\">Alert</div>")
    //$(".RADBCalenderErrorDivRight").append("<div class=\"RADBCalenderErrorDivText\">Are You Sure You want to delete " + module + " "
    //    + toDeleteItem.toUpperCase() + "?</div>");
    //$("#RADBCalenderAlertParentDiv").append("<div class=\"RADBCalenderPopUpErrorFooter\"></div>");
    //$(".RADBCalenderPopUpErrorFooter").append("<div class=\"RADBCalenderPopUpErrorYes\">Yes</div><div class=\"RADBCalenderPopUpErrorNo\">No</div>")
    //$("#RADBCalenderAlertParentDiv").css("top", "-200px");
    //$("#RADBCalenderAlertParentDiv").animate({ "top": "0px" });

    $(".RMapAlertMain").remove();

    var $rmapAlertMainDiv = $("<div>", {
        class: "RMapAlertMain"
    });

    var $rmapAlertContentDiv = $("<div>", {
        class: "RMapAlertContent"
    });

    var $rmapAlertHeaderLineDiv = $("<div>", {
        class: "RMapAlertHeaderLine"
    });

    var $rmapAlertHeaderParentDiv = $("<div>", {
        class: "RMapAlertHeaderParent"
    });

    var $rmapAlertHeaderIconDiv = $("<div>", {
        class: "fa fa-exclamation-circle RMapAlertHeaderIcon"
    });

    var $rmapAlertHeaderMesageDiv = $("<div>", {
        class: "RMapAlertHeaderMessage",
        text : "Alert"
    });

    var $rmapAlertMessageBody = $("<div>", {
        class: "RMapAlertMessageBody",
        text: "Are you sure you want to delete " + module + " " + toDeleteItem.toUpperCase() + "?"
    });
    
    var $rmapAlertFooterDiv = $("<div>", {
        class: "RMapAlertFooterMain"
    });

    var $rmapAlertFooterYesParentDiv = $("<div>", {
        class: "RMapAlertFooterParent"
    });

    var $rmapAlertFooterYesButtonDiv = $("<div>", {
        class: "RMapAlertFooterButton",
        text : "Yes"
    });

    var $rmapAlertFooterNoParentDiv = $("<div>", {
        class: "RMapAlertFooterParent"
    });

    var $rmapAlertFooterNoButtonDiv = $("<div>", {
        class: "RMapAlertFooterButton",
        text: "No"
    });

   

    $rmapAlertHeaderParentDiv.append($rmapAlertHeaderIconDiv)
    $rmapAlertHeaderParentDiv.append($rmapAlertHeaderMesageDiv)
    $rmapAlertFooterYesParentDiv.append($rmapAlertFooterYesButtonDiv);
    $rmapAlertFooterNoParentDiv.append($rmapAlertFooterNoButtonDiv);
    $rmapAlertFooterDiv.append($rmapAlertFooterYesParentDiv);
    $rmapAlertFooterDiv.append($rmapAlertFooterNoParentDiv);
    $rmapAlertContentDiv.append($rmapAlertHeaderLineDiv);
    $rmapAlertContentDiv.append($rmapAlertHeaderParentDiv);
    $rmapAlertContentDiv.append($rmapAlertMessageBody);
    $rmapAlertContentDiv.append($rmapAlertFooterDiv);
    $rmapAlertMainDiv.append($rmapAlertContentDiv);
    mainDivforAlert.append($rmapAlertMainDiv);


}

RADMappingConfiguration.prototype.bindQTip = function () {
    var self = this;
    $(".RMapNMoreClass").each(function () {
        var data = [];
        if ($(this).parent().attr("rmapattributeiddisplayname") != null)
            data = $(this).parent().attr("rmapattributeiddisplayname").split("~");
        if (data.indexOf("") >= 0)
            data.splice(data.indexOf(""), 1);
        var visisbleData = $(this).parent().clone().children().remove().end().text().trim().split(",");
        var parentDiv = $("<div>", {
            class: 'RMapQtipParent'
        })

        for (var i = 0; i < data.length; i++) {
            if (visisbleData.indexOf(data[i]) == -1) {
                //var dataTextArray = data[i];
                var parentEle = $("<div>", {
                    class: 'RMapQtipChildParent'
                });
                var childEle = $("<div>", {
                    class: 'RMapQtipChild',
                    text: data[i]
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










