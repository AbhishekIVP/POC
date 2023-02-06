var srmMigrationUtility = (function () {

    function SRMMigrationUtility() {
        this._pageViewModelInstance = null;
        this._moduleID_moduleName_map = [];
        this._moduleID = 0;
        this._isBindingsApplied = false;
        this._username = "";
        this._dateInfo = "";
        this._errorMsgDiv = "";
    }
    var supportedFileTypes = ".xls|.xlsx|.xml";
    var supportedFileTypesForBulk = ".xls|.xlsx|.7z|.xml";

    var SRMFileUpload;

    var testData;

    //ID INFOS
    var uploadMasterId = "srmmigration-import-master-button";
    var downloadMasterId = "srmmigration-export-master-button";
    var differenceMasterId = "srmmigration-difference-master-button";
    var listViewParentId = "srmmigration-selected-list-view-parent";
    var uploadParentId = "srmmigration-upload-section";
    var downloadSelectionPlaceholderId = "srmmigration-download-selection-placeholder";
    var uploadPlaceholderId = "srmmigration-upload-selection-placeholder";
    var uploadHelpers = "srm-migration-upload-helpers";
    var bulkDownloadDetailsId = 'srmmigration-bulk-download-details';


    //CLASS
    var moduleItemParent = "srmmigration-modules-item";
    var dispNoneClass = "srmmigration-display-none";
    var dispListViewParent = "srmmigration-display-selected-item-parent";
    var featureListParent = "srmmigration-modules-list";
    var uploadDivClass = "srmmigration-file-upload-div";
    var visibilityHiddenClass = "srm-migration-visibility-hidden";
    var requireMissingTableParent = "srmmigration-upload-require-missing-table";

    //selection toggle
    var selectionClassForXmlExcelToggle = "srmmigration-download-type-children-selected";
    var excelType = "srmmigration-downloadtype-excel";
    var xmlType = "srmmigration-downloadtype-xml";

    //selecton toggle for missing tables
    var selectionClassForRequireMissingToggle = "srmmigration-upload-require-missing-table-children-selected";
    var yesTypeRequireMissing = "srmmigration-upload-require-missing-yes";
    var noTypeRequireMissing = "srmmigration-upload-require-missing-no";

    var bulkButtonClass = "srmmigration-bulk-button-children";
    var bulkUploadSpecificClass = "srmmigration-bulk-upload-specific-css";

    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    var privilegeList = {
        3: ['Config Manager - Upload', 'Config Manager - Compare'],
        6: ['RM - Config Manager - Upload', 'RM - Config Manager - Compare'],
        18: ['FM - Config Manager - Upload', 'FM - Config Manager - Compare'],
        20: ['PM - Config Manager - Upload', 'PM - Config Manager - Compare']
    }

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    var srmMigrationUtility = new SRMMigrationUtility();


    function pageViewModel(data) {
        var self = this;
        self.moduleList = ko.observableArray([]);
        self.checkboxList = ko.observableArray([]);
        self.errorPopupList = ko.observableArray([]);
        self.searchText = ko.observable("");
        self.downloadAllFilePath = ko.observable("");

        for (let i in data) {
            self.moduleList.push(new chainViewModel(data[i]));
        }

        self.SelectAllCheckboxItems = () => {
            ko.utils.arrayForEach(self.checkboxList(), (item) => {
                item.isSelected(true);
            });
        }

        self.UnSelectAllCheckboxItems = () => {
            ko.utils.arrayForEach(self.checkboxList(), (item) => {
                item.isSelected(false);
            });
        }

        self.searchTextForCheckboxes = ko.computed(function () {
            var searchTxt = self.searchText().toLowerCase();
            ko.utils.arrayForEach(self.checkboxList(), function (item) {
                item.itemName.toLowerCase().indexOf(searchTxt) >= 0 ? item.isVisible(true) : item.isVisible(false);
            });
        });

        //BULK DOWNLOAD
        self.isBulkUploadClicked = ko.observable(false);
        self.isBulkDownloadDiffClicked = ko.observable(false);
        //self.isBulkDownloadClicked = ko.observable(false);
        //BULK DOWNLOAD

        self.OnDownloadCheckboxClick = checkboxDownloadClick;
        self.OnUploadCheckboxClick = checkboxUploadClick;

        //Bulk action buttons
        self.onBulkDownloadClick = downloadMasterClick;
        //self.onBulkUploadClick = ko.computed(() => self.isBulkUploadClicked(!self.isBulkUploadClicked()));
        //self.onBulkDownloadDiffClick = ko.computed(() => self.isBulkDownloadDiffClicked(!self.isBulkUploadClicked()));

        self.isAtleastOneCheckboxSelected = ko.computed(function () {
            var result = false;
            ko.utils.arrayForEach(self.checkboxList(), function (item) {
                result = result || item.isSelected();
            });
            return result;
        });

        self.isAtleastOneFeatureSelected = ko.computed(function () {
            var result = false;
            ko.utils.arrayForEach(self.moduleList(), function (item) {
                result = result || item.isSelected();
            });
            return result;
        });

        self.isSearchResultAvailableForCheckboxes = ko.computed(() => {
            var result = false;
            ko.utils.arrayForEach(self.checkboxList(), function (item) {
                result = result || item.isVisible();
            });
            return result;
        });

        self.downloadAllZip = () => {
            downloadFileInIframe(self.downloadAllFilePath());
        };


        self.privilegeForCompare = ko.observable(false);
        self.privilegeForUpload = ko.observable(false);
    }

    function refreshPreviouslyClickedStates() {
        //hide all divs
        $("#" + uploadHelpers).addClass(dispNoneClass);
        //removes the class for upload also
        $(".srmmigration-selection-placeholder-children").addClass(dispNoneClass).removeClass(bulkUploadSpecificClass);

        //unset all selection values
        if (srmMigrationUtility._pageViewModelInstance != null) {
            var self = srmMigrationUtility._pageViewModelInstance.moduleList();
            for (var i in self) {
                self[i].isDownloadClicked(false);
                self[i].isDownloadDiffClicked(false);
                self[i].isUploadClicked(false);
            }
            //refresh search
            srmMigrationUtility._pageViewModelInstance.searchText("");

            //refresh bulk uploads
            srmMigrationUtility._pageViewModelInstance.isBulkUploadClicked(false);
            srmMigrationUtility._pageViewModelInstance.isBulkDownloadDiffClicked(false);
            srmMigrationUtility._pageViewModelInstance.downloadAllFilePath("");
        }

        //refresh top buttons
        $("#" + bulkDownloadDetailsId).addClass(dispNoneClass);
    }

    //also handles upload of master click and children
    function checkboxUploadClick(vm, e) {
        var IsSync = false;
        var requireMissingTables = true;

        var featuresList = [];
        debugger;
        var isSingleUpload = !(srmMigrationUtility._pageViewModelInstance.isBulkUploadClicked() || srmMigrationUtility._pageViewModelInstance.isBulkDownloadDiffClicked());

        var self = srmMigrationUtility._pageViewModelInstance.moduleList();
        //Case for single file upload
        if (isSingleUpload) {
            for (var i in self) {
                if (self[i].isDownloadDiffClicked()) {
                    IsSync = false;
                    featuresList.push(self[i].id);
                    //get require missing tables bit
                    if ($("." + yesTypeRequireMissing, $("#" + uploadHelpers)).hasClass(selectionClassForRequireMissingToggle))
                        requireMissingTables = true;
                    else
                        requireMissingTables = false;
                    break;
                }
                else if (self[i].isUploadClicked()) {
                    IsSync = true;
                    requireMissingTables = false;
                    featuresList.push(self[i].id);
                    break;
                }
            }
        }

            //multiple upload
        else {
            //ko.utils.arrayForEach(self, (item) => {
            //    item.isSelected() ? featuresList.push(item.id) : "";
            //});
            featuresList = [];
            if (srmMigrationUtility._pageViewModelInstance.isBulkDownloadDiffClicked()) {
                IsSync = false;
                if ($("." + yesTypeRequireMissing, $("#" + uploadHelpers)).hasClass(selectionClassForRequireMissingToggle))
                    requireMissingTables = true;
                else
                    requireMissingTables = false;
            }
            else {
                IsSync = true;
                requireMissingTables = false;
            }
        }

        var IsExcel = true;
        var clickContextParent = $("#" + uploadParentId);
        if ($("." + excelType, clickContextParent).hasClass(selectionClassForXmlExcelToggle))
            IsExcel = true;
        else
            IsExcel = false;

        var filePathForUploadedFileOnServer = $(".srmmigration-upload-path-class", $("#" + uploadParentId)).first().attr('filePath');

        var params = {
            features: featuresList,
            isSync: IsSync,
            requireMissing: requireMissingTables,
            moduleID: srmMigrationUtility._moduleID,
            userName: srmMigrationUtility._username,
            dateFormat: srmMigrationUtility._dateInfo.ShortDateFormat,
            dateTimeFormat: srmMigrationUtility._dateInfo.LongDateFormat,
            //timePartFormat: srmMigrationUtility._dateInfo.LongTimePattern,
            timeFormat: srmMigrationUtility._dateInfo.LongTimePattern,
            filePath: filePathForUploadedFileOnServer,
            isExcel: IsExcel,
            isBulkUpload: !isSingleUpload
        };

        CallCommonServiceMethod('UploadMigrationConfiguration', params, OnSuccess_DownloadData, OnFailure, null, false);
    }

    function OnFailure(data) {
        SecMasterJSCommon.SMSCommons.onUpdated();
        displayErrorPopup(data.d);
    }

    function checkboxDownloadClick(vm, e) {
        var listOfIds = [];
        var isAllSelected = true;
        ko.utils.arrayForEach(vm.checkboxList(), (item) => {
            item.isSelected() ? listOfIds.push(item.id) : isAllSelected = false;
        });

        if (isAllSelected)
            listOfIds = [];

        //selected features
        var featuresList = [];
        var self = srmMigrationUtility._pageViewModelInstance.moduleList();
        for (var i in self) {
            if (self[i].isDownloadClicked()) {
                featuresList.push(self[i].id);
                break;
            }
        }

        //fileType
        var isExcel = true;
        var clickContextParent = $("#" + listViewParentId);
        if ($("." + excelType, clickContextParent).hasClass(selectionClassForXmlExcelToggle))
            isExcel = true;
        else
            isExcel = false;

        var params = {
            features: featuresList,
            selectedItems: listOfIds,
            moduleID: srmMigrationUtility._moduleID,
            userName: srmMigrationUtility._username,
            isExcelFile: isExcel
        };
        CallCommonServiceMethod('DownloadMigrationConfiguration', params, OnSuccess_DownloadData, function () {
            SecMasterJSCommon.SMSCommons.onUpdated();
        }, null, false);
    }

    function chainViewModel(data) {
        var self = this;
        self.moduleName = data.name;
        self.id = data.id;
        self.isSelected = ko.observable(ignoreFeatures(data.id) ? false : true);
        self.isDownloadClicked = ko.observable(false);
        self.isDownloadDiffClicked = ko.observable(false);
        self.isUploadClicked = ko.observable(false);

        self.onClickSelectionPopup = setDownloadDetailsDiv;
        self.onClickUploadDiv = setCustomUploadDiv;
        //setUploadDiv;
        self.onSelectionClick = () => {
            if (!ignoreFeatures(self.id))
                self.isSelected(!self.isSelected());
        }
    }

    function checkboxViewModel(data) {
        var self = this;

        self.itemName = data.Text;
        self.id = data.Value;
        self.additionalText = data.AdditionalText;
        self.isSelected = ko.observable(true);
        self.isVisible = ko.observable(true);

        //isSelected par class

        self.onSelectionClick = () => {
            self.isSelected(!self.isSelected());
        }
    }

    function errorPopupListModel(data) {
        var self = this;
        self.ErrorMsg = ((data.SyncStatus != null && data.SyncStatus == 'Failure') && (data.ErrorMsg == null || data.ErrorMsg == "")) ? "Please download status file for errors" : data.ErrorMsg;
        self.SyncStatus = data.SyncStatus;
        self.FeatureDisplayName = data.FeatureDisplayName;
        self.FileName = data.FileName;
        self.FeatureEnum = data.FeatureEnum;
        self.FilePath = data.FilePath;
        self.IsDownloadable = data.IsDownloadable;
        self.downloadFeatureSheet = (vm) => { downloadFileInIframe(vm.FilePath); };
    }

    function populateCheckboxViewModel(data) {
        srmMigrationUtility._pageViewModelInstance.checkboxList.removeAll();
        var self = srmMigrationUtility._pageViewModelInstance.checkboxList;
        for (var i in data) {
            let currData = data[i];
            self.push(new checkboxViewModel(currData));
        }
    }

    function downloadFileAjaxHit(params) {

    }

    function preserveDivs() {
        //preserving Download Div
        var placeholderDivDownload = $("#" + downloadSelectionPlaceholderId);
        var downloadSelectionDiv = $("#" + listViewParentId);
        downloadSelectionDiv.removeClass(dispListViewParent).addClass(dispNoneClass);
        placeholderDivDownload.append(downloadSelectionDiv);

        //preserving Upload Div
        var placeholderUploadDiv = $("#" + uploadPlaceholderId);
        var uploadParentDiv = $("#" + uploadParentId);
        uploadParentDiv.addClass(dispNoneClass);
        placeholderUploadDiv.append(uploadParentDiv);
        refreshPreviouslyClickedStates();
    }

    function setDownloadDetailsDiv(vm, event) {
        if (ignoreFeatures(vm.id))
            return false;
        //set download clicked
        refreshPreviouslyClickedStates();
        //var self = srmMigrationUtility._pageViewModelInstance.moduleList();
        //for (var i in self) {
        //    self[i].isDownloadClicked(false);
        //}

        vm.isDownloadClicked(true);

        findCorrectRelativePositionForDiv(vm, event);

        //ajax request
        var params = {
            moduleID: srmMigrationUtility._moduleID,
            feature: vm.id,
            userName: srmMigrationUtility._username
        }

        $(".srmmigration-action-text-on-click span").text("DOWNLOAD");

        CallCommonServiceMethod('GetSelectableItemsForMigration', params, onSuccess_GetSelectableItemsForMigration, function () { SecMasterJSCommon.SMSCommons.onUpdated(); }, null, false);
    }

    function setToggleButtonForXmlExcel(context) {
        $(".srmmigration-selected-list-view-downloadtype-selection-parent", context).off('click').on('click', 'div', function () {
            if ($("." + excelType, context).hasClass(selectionClassForXmlExcelToggle)) {
                $("." + excelType, context).removeClass(selectionClassForXmlExcelToggle)
                $("." + xmlType, context).addClass(selectionClassForXmlExcelToggle);
            }
            else {
                $("." + xmlType, context).removeClass(selectionClassForXmlExcelToggle)
                $("." + excelType, context).addClass(selectionClassForXmlExcelToggle);
            }
        });

        //by default
        $(".srmmigration-download-type-children", context).removeClass(selectionClassForXmlExcelToggle);
        $("." + excelType, context).addClass(selectionClassForXmlExcelToggle);
    }

    function setToggleButtonForRequireMissingTables(context) {
        $("." + requireMissingTableParent, context).off('click').on('click', 'div', function () {
            var yesEle = $("." + yesTypeRequireMissing, context);
            var noEle = $("." + noTypeRequireMissing, context);

            if (yesEle.hasClass(selectionClassForRequireMissingToggle)) {
                yesEle.removeClass(selectionClassForRequireMissingToggle);
                noEle.addClass(selectionClassForRequireMissingToggle);
            }
            else {
                noEle.removeClass(selectionClassForRequireMissingToggle);
                yesEle.addClass(selectionClassForRequireMissingToggle);
            }
        });

        //by default
        $(".srmmigration-upload-require-missing-table-children", context).removeClass(selectionClassForRequireMissingToggle);
        $("." + yesTypeRequireMissing, context).addClass(selectionClassForRequireMissingToggle);
    }

    function setCloseButtonForUpload() {
        var closeButton = $("#srm-migration-upload-listview-button-parent-close");
        closeButton.off('click').on('click', (event) => {
            event.stopPropagation();
            $("#" + uploadHelpers).addClass(dispNoneClass);
            //reset div for upload
            debugger;
            $("." + uploadDivClass, "#" + uploadParentId).empty().attr("CurrentFilesLoaded", "").attr("isReadOnly", "false").attr("directUploadFile", "true");
            if (srmMigrationUtility._pageViewModelInstance.isBulkUploadClicked() || srmMigrationUtility._pageViewModelInstance.isBulkDownloadDiffClicked())
                SRMFileUpload.init(false, supportedFileTypesForBulk, "srmmigration-file-upload-div", successfulUploadCallback, callCloseButtonOfUpload);
            else
                SRMFileUpload.init(false, supportedFileTypes, "srmmigration-file-upload-div", successfulUploadCallback, callCloseButtonOfUpload);
        });

    }

    function successfulUploadCallback() {
        var context = $("#" + uploadParentId);
        debugger;
        var fileTypeClass = ".srmmigration-filetype-text";
        $("#" + uploadHelpers).removeClass(dispNoneClass);

        var self = srmMigrationUtility._pageViewModelInstance.moduleList();
        for (var i in self) {
            if (self[i].isDownloadDiffClicked()) {
                $(fileTypeClass, context).text("Compare Result File Type");
                $("." + requireMissingTableParent, context).removeClass(dispNoneClass);
                break;
            }
            else if (self[i].isUploadClicked()) {
                $(fileTypeClass, context).text("Upload Result File Type");
                $("." + requireMissingTableParent, context).addClass(dispNoneClass);
                break;
            }
        }

        if (srmMigrationUtility._pageViewModelInstance.isBulkUploadClicked()) {
            $(fileTypeClass, context).text("Upload Result File Type");
            $("." + requireMissingTableParent, context).addClass(dispNoneClass);
        }
        else if (srmMigrationUtility._pageViewModelInstance.isBulkDownloadDiffClicked()) {
            $("." + requireMissingTableParent, context).removeClass(dispNoneClass);
            $(fileTypeClass, context).text("Compare Result File Type");

        }

        setToggleButtonForXmlExcel(context);
        setToggleButtonForRequireMissingTables(context);
        setCloseButtonForUpload();
    }

    function callCloseButtonOfUpload() {
        event.stopPropagation();
        $("#srm-migration-upload-listview-button-parent-close").click();
    }

    function setCustomUploadDiv(vm, event) {
        if (ignoreFeatures(vm.id))
            return false;

        refreshPreviouslyClickedStates();
        findCorrectRelativePositionForDiv(vm, event);

        var placeHolderDiv = getRelativePlaceholderDiv();
        var uploadParentDiv = $("#" + uploadParentId);
        //uploadParentDiv.empty();
        $("." + uploadDivClass).empty().attr("CurrentFilesLoaded", "").attr("isReadOnly", "false").attr("directUploadFile", "true");

        placeHolderDiv.append(uploadParentDiv);
        SRMFileUpload.init(false, supportedFileTypes, "srmmigration-file-upload-div", successfulUploadCallback, callCloseButtonOfUpload);
        uploadParentDiv.removeClass(dispNoneClass);

        var self = srmMigrationUtility._pageViewModelInstance.moduleList();

        var targetDiv = $(event.target);

        if (targetDiv.attr('actionType') == "Diff") {
            debugger;
            vm.isDownloadDiffClicked(true);
            $("#srm-migration-upload-listview-button span").text("Download Delta");
            $(".srmmigration-action-text-on-click span").text("COMPARE");
        }
        else {
            debugger;
            vm.isUploadClicked(true);
            $("#srm-migration-upload-listview-button span").text("Upload");
            $(".srmmigration-action-text-on-click span").text("UPLOAD");
        }

        //close button handler
        $(".srmmigration-close-list-view-parent", uploadParentDiv).off('click').on('click', () => {
            refreshPreviouslyClickedStates();
        });

        animateToUncollapseDiv(placeHolderDiv, placeHolderDiv);
    }

    function onSuccess_GetSelectableItemsForMigration(data) {
        SecMasterJSCommon.SMSCommons.onUpdated();
        data = JSON.parse(data.d);
        populateCheckboxViewModel(data);
        var placeHolderDiv = getRelativePlaceholderDiv();
        var downloadSelectionDiv = $("#" + listViewParentId);

        placeHolderDiv.append(downloadSelectionDiv);
        setToggleButtonForXmlExcel(downloadSelectionDiv);

        //animate in future
        downloadSelectionDiv.removeClass(dispNoneClass).addClass(dispListViewParent);

        var fileTypeClass = ".srmmigration-filetype-text";
        $(fileTypeClass, downloadSelectionDiv).text("Download File Type");

        //close button handler
        $(".srmmigration-close-list-view-parent", downloadSelectionDiv).off('click').on('click', () => {
            refreshPreviouslyClickedStates();
        });

        equalizeHeights();
        animateToUncollapseDiv(placeHolderDiv, placeHolderDiv);
    }

    function animateToUncollapseDiv(parent, child) {
        debugger;
        $("#srmmigration-parent").scrollTop($(parent).prev().offset().top - 40);

        //$("#srmmigration-parent").animate({
        //    scrollTop: $(".srmmigration-modules-item").first().outerWidth();
        //},
        //'fast');
    }

    function onClickDownloadSelection(vm, event) {

    }

    function getRelativePlaceholderDiv() {
        return $("#srmmigration-placeholder-relative-div");
    }

    function createRelativePlaceholderDiv() {
        var elementInDom = getRelativePlaceholderDiv();
        if (elementInDom.length > 0)
            return elementInDom;

        var elm = '<div id="srmmigration-placeholder-relative-div" >' +
          '</div>';
        return elm;
    }

    function findCorrectRelativePositionForDiv(vm, e) {
        var items = $("." + moduleItemParent);
        var item = items.first();
        var parentList = $("#" + featureListParent);

        var rowElementsCount = parseInt(parentList.width() / item.outerWidth());
        var totalItemsCount = items.length;

        //some integer
        var currentClickDiv = $(e.target).closest("." + moduleItemParent);
        var currentClickDivPosition = currentClickDiv.index();
        var placeholderDivPosition = getRelativePlaceholderDiv().index();
        if (placeholderDivPosition && placeholderDivPosition != '-1' && placeholderDivPosition < currentClickDivPosition)
            currentClickDivPosition--;

        var positionForEq = 0;
        for (var i = rowElementsCount - 1; ; i += rowElementsCount) {
            if (i >= currentClickDivPosition) {
                if (i >= totalItemsCount - 1)
                    positionForEq = totalItemsCount - 1;
                else
                    positionForEq = i;
                //positionForEq = Math.min(i, totalItemsCount);
                break;
            }
        }

        var divToAppendNext = items.eq(positionForEq);
        //$(getRelativePlaceholderDiv()).after(divToAppendNext);
        var placeHolderDiv = createRelativePlaceholderDiv();
        $(divToAppendNext).after(placeHolderDiv);
    }

    function removeRelativePlaceholderDiv() {

    }

    //Common Modules initialisation method
    function modulesInitialise(username) {
        srmMigrationUtility._username = username;

        //privilegeCheck();

        var listofTabsToBindFunctionsWith = CommonModuleTabs.ModuleList();

        for (i in listofTabsToBindFunctionsWith) {
            let item = listofTabsToBindFunctionsWith[i];
            srmMigrationUtility._moduleID_moduleName_map[item.displayName.toLowerCase()] = item.moduleId;
            switch (item.displayName.toLowerCase().trim()) {
                case "securities":
                    SRMProductTabs.setCallback({
                        key: item.displayName.toLowerCase(), value: initSecMaster
                    });
                    break;
                case "refdata":
                    SRMProductTabs.setCallback({
                        key: item.displayName.toLowerCase(), value: initRefMaster
                    });
                    break;
                case "funds":
                    SRMProductTabs.setCallback({
                        key: item.displayName.toLowerCase(), value: initFundMaster
                    });
                    break;
                case "parties":
                    SRMProductTabs.setCallback({
                        key: item.displayName.toLowerCase(), value: initPartyMaster
                    });
                    break;
            }
        }
    };

    function initSecMaster() {
        srmMigrationUtility._moduleID = srmMigrationUtility._moduleID_moduleName_map["securities"];
        preserveDivs();
        getInitialScreenData();
    }

    function initRefMaster() {
        srmMigrationUtility._moduleID = srmMigrationUtility._moduleID_moduleName_map["refdata"];
        preserveDivs();
        getInitialScreenData();
    }

    function initFundMaster() {
        srmMigrationUtility._moduleID = srmMigrationUtility._moduleID_moduleName_map["funds"];
        preserveDivs();
        getInitialScreenData();
    }

    function initPartyMaster() {
        srmMigrationUtility._moduleID = srmMigrationUtility._moduleID_moduleName_map["parties"];
        preserveDivs();
        getInitialScreenData();
    }


    //Ajax Calls
    function getInitialScreenData() {
        try {
            $("#srmmigration-parent").addClass(dispNoneClass);
            if (!srmMigrationUtility._moduleID)
                throw "Module Id wasn't provided";

            var params = {
            };
            params.moduleId = parseInt(srmMigrationUtility._moduleID);
            params.username = srmMigrationUtility._username;
            CallCommonServiceMethod('GetMigrationUtilityModulesList', params, OnSuccess_getInitialScreenData, function () { SecMasterJSCommon.SMSCommons.onUpdated(); }, null, false);
        }
        catch (ex) {
            console.log("Error at getInitialScreenData" + ex);
        }
    }


    function getData() {
        //TODO
        var params = {
        };
        params.moduleId = srmMigrationUtility._moduleID;
        params.userName = srmMigrationUtility._username;

    }

    function setScreenSize() {
        let availableHeight = $(window).height() - $(".srm_panelTopSections").height() - 4; //-4 for margin
        $("#srmmigration-parent").css({ "height": availableHeight, "overflow-y": "overlay", "scroll-behavior": "smooth" });
    }


    //Common Service Helper Method
    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        //debugger;
        SecMasterJSCommon.SMSCommons.onUpdating();
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }
    //Common Service Helper Method

    function OnSuccess_getInitialScreenData(data) {
        SecMasterJSCommon.SMSCommons.onUpdated();
        $("#srmmigration-parent").removeClass(dispNoneClass);
        //KO Bindings Check
        if (!srmMigrationUtility._isBindingsApplied) {
            srmMigrationUtility._pageViewModelInstance = new pageViewModel(data.d);
            srmMigrationUtility._isBindingsApplied = true;
            ko.applyBindings(srmMigrationUtility._pageViewModelInstance, document.getElementById("srmmigration-parent"));
        }
        else {
            srmMigrationUtility._pageViewModelInstance.moduleList.removeAll();
            var self = srmMigrationUtility._pageViewModelInstance.moduleList;
            for (var i in data.d) {
                let currData = data.d[i];
                self.push(new chainViewModel(currData));
            }
        }

        setScreenSize();
        applyClickBindingsForBulkButton();
        getPrivilegesForModules();

        //Testing new ref control
        SRMFileUpload = new SRMCustomFileUploader(path + "/SRMFileUpload.aspx", "directUploadFile");
        //SetFileUploadControl('SRMMigrationFileUpload', 'SRMMigrationFileUploadAttachement');
    }

    function ignoreFeatures(id) {
        return false;
        switch (id) {
            //downstream report, ds task, ds sys, transp task
            case 6: case 7: case 10: case 9:

                //
            case 19: case 20: case 22: case 23: case 25:
                return true;
            default: return false;
        }
    }

    function OnSuccess_DownloadData(data) {
        SecMasterJSCommon.SMSCommons.onUpdated();
        var filePath = data.d;

        if (filePath.includes("ž")) {
            displayErrorPopup(filePath.split("ž")[1]);
            return false;
        }

        if (filePath.includes("Ÿ")) {
            srmMigrationUtility._pageViewModelInstance.downloadAllFilePath(filePath.split("Ÿ")[0]);
            displayPopupForUploadResult(filePath.split("Ÿ")[1], true);
            return false;
        }

        if (filePath.includes("¡")) {
            displaySuccessPopup("Configuration is already in sync. There is no difference in the uploaded file and current configuration.");
            return false;
        }

        downloadFileInIframe(filePath);
        //file Delete hit
    }

    function displayPopupForUploadResult(data, parseData) {
        try {
            var errorDataList = data;
            if (parseData != null && parseData)
                errorDataList = JSON.parse(data);
            srmMigrationUtility._pageViewModelInstance.errorPopupList.removeAll();
            var self = srmMigrationUtility._pageViewModelInstance.errorPopupList;
            var isSyncSuccesful = true;
            var alreadyInSync = true;
            for (var i in errorDataList) {
                let currData = errorDataList[i];
                if (currData.FeatureEnum != -1) {
                    isSyncSuccesful = isSyncSuccesful && (currData.SyncStatus == "Success");
                    alreadyInSync = alreadyInSync && (currData.SyncStatus == "Already in Sync");
                    self.push(new errorPopupListModel(currData));
                }
            }
            if (errorDataList.length == 0) {
                isSyncSuccesful = false;
                alreadyInSync = false;
            }
            $(".srmmigration .popup-content-upload-error").removeClass(dispNoneClass);
            //display this error popup, and remove the previous one, probably add a few states as well
            //handleAllDownloadClickButton -> make a JS function -> onclick

            //errorDataList.length>1 ? true:false : decide both cases TODO

            if (isSyncSuccesful) {
                displaySuccessPopup("", "STATUS", true);
            }
            else if (alreadyInSync) {
                displaySuccessPopup("", "STATUS", true);
            }
            else {
                displaySuccessPopup("", "STATUS", true);
            }
        }
        catch (ex) {
            console.log("Error at displayPopupForUploadResult" + ex);
        }
    }

    function displayErrorPopup(errorMsg, titleText, errorListExists) {
        //disableClickOnScreen();
        disableClickOnScreen();
        var msgPopup = $(".message-popup-container", $(".srmmigration")).first().removeClass(dispNoneClass);
        if (titleText == null)
            titleText = "Error";
        if (errorListExists)
            $(".message-popup-container").addClass(" message-popup-container-error-list");
        $(".title-text", msgPopup).text(titleText);
        $(".popup-content", msgPopup).text(errorMsg);
        $(".message-popup", msgPopup).addClass("failed").removeClass('success').removeClass("status");
        $(".popup-close-btn", msgPopup).off('click').on('click', () => { $(".message-popup-container", $(".srmmigration")).first().addClass(dispNoneClass); $(".srmmigration .popup-content-upload-error").addClass(dispNoneClass); enableClickOnScreen(); });
    }

    function displayStatusPopup(statusMsg, titleText, errorListExists) {
        disableClickOnScreen();
        titleText = "Status";
        var msgPopup = $(".message-popup-container", $(".srmmigration")).first().removeClass(dispNoneClass);
        if (titleText == null)
            titleText = "Error";
        if (errorListExists)
            $(".message-popup-container").addClass(" message-popup-container-error-list");
        $(".title-text", msgPopup).text(titleText);
        $(".popup-content", msgPopup).text(statusMsg);
        $(".message-popup", msgPopup).addClass("status").removeClass('success').removeClass("failed");
        $(".popup-close-btn", msgPopup).off('click').on('click', () => { $(".message-popup-container", $(".srmmigration")).first().addClass(dispNoneClass); $(".srmmigration .popup-content-upload-error").addClass(dispNoneClass); enableClickOnScreen(); });
    }

    //status is now success
    function displaySuccessPopup(successMsg, titleText, errorListExists) {
        disableClickOnScreen();
        var msgPopup = $(".message-popup-container", $(".srmmigration")).first().removeClass(dispNoneClass);
        if (titleText == null)
            titleText = "Status";
        if (errorListExists)
            $(".message-popup-container").addClass(" message-popup-container-error-list");

        $(".title-text", msgPopup).text(titleText);
        $(".popup-content", msgPopup).text(successMsg);
        $(".message-popup", msgPopup).removeClass("failed").addClass('success').removeClass("status");
        $(".popup-close-btn").off('click').on('click', () => { $(".message-popup-container", $(".srmmigration")).first().addClass(dispNoneClass); $(".srmmigration .popup-content-upload-error").addClass(dispNoneClass); enableClickOnScreen(); });
    }

    function disableClickOnScreen() {
        //debugger;
        //var popup = $(".message-popup-container");
        //$(document).on({
        //    'click.disableClickOnScreen': (event) => {
        //        debugger;
        //        var ele = $(event.target)
        //        if (ele.is(popup) || ele.closest(popup).length == 0) {
        //            //$(document).off('click.disableClickOnScreen');
        //        }
        //        else {
        //            return false;
        //        }
        //    }
        //});
        $('#disableDivForPopup').show();
        var scrollHeight = document.body.scrollHeight.toString();
        var scrollWidth = document.body.scrollWidth.toString();
        var windowHeight = document.body.parentNode.offsetHeight.toString();
        $('#disableDivForPopup').height(scrollHeight + 'px');
        $('#disableDivForPopup').width(scrollWidth + 'px');
    }

    function enableClickOnScreen() {
        $('#disableDivForPopup').height(0 + 'px');
        $('#disableDivForPopup').width(0 + 'px');
        $('#disableDivForPopup').hide();
    }

    function downloadFileInIframe(filePath) {
        $("#srmmigration-iframe").attr('src', path + "/SRMFileUpload.aspx?fileonserver=true&fullPath=" + filePath + "");
    }

    function RemoveLastDirectoryPartOf(the_url) {
        var the_arr = the_url.split('/');
        the_arr.pop();
        return (the_arr.join('/'));
    }

    function applyClickBindingsForBulkButton() {
        //TOP BUTTON BINDINDS
        $("#" + uploadMasterId).off('click').on('click', "." + bulkButtonClass, setBulkUploadDetails);
        $("#" + downloadMasterId).off('click').on('click', "." + bulkButtonClass, setBulkDownloadDetails);
        $("#" + differenceMasterId).off('click').on('click', "." + bulkButtonClass, setBulkDownloadDiffDetails);
    }

    //bulk buttons display handlers
    function setBulkDownloadDetails(event) {
        if (!srmMigrationUtility._pageViewModelInstance.isAtleastOneFeatureSelected()) {
            return false;
        }

        refreshPreviouslyClickedStates();
        setToggleButtonForXmlExcel($("#" + bulkDownloadDetailsId));
        $("#" + bulkDownloadDetailsId).removeClass(dispNoneClass);

        //giving names to click events to later unbind them after they are done.
        $(document).off('click').on({
            'click.bulkDownloadClickEvent': (event) => {
                debugger;
                var ele = $(event.target)
                if ((!ele.not("#" + bulkDownloadDetailsId)) || ele.closest("#" + bulkDownloadDetailsId).length == 0) {
                    $("#" + bulkDownloadDetailsId).addClass(dispNoneClass);
                    //unbinding click event
                    $(document).off('click.bulkDownloadClickEvent');
                }
            }
        });
        event.stopPropagation();
    }

    function commonBulkUploadDiff(placeHolderDiv) {
        refreshPreviouslyClickedStates();
        var uploadParentDiv = $("#" + uploadParentId);
        //uploadParentDiv.empty();
        $("." + uploadDivClass).empty().attr("CurrentFilesLoaded", "").attr("isReadOnly", "false").attr("directUploadFile", "true");
        placeHolderDiv.append(uploadParentDiv);
        SRMFileUpload.init(false, supportedFileTypesForBulk, "srmmigration-file-upload-div", successfulUploadCallback, callCloseButtonOfUpload);
        uploadParentDiv.removeClass(dispNoneClass).addClass(bulkUploadSpecificClass);
        $(".srmmigration-close-list-view-parent", uploadParentDiv).off('click').on('click', () => {
            refreshPreviouslyClickedStates();
        });
    }

    function setBulkUploadDetails(event) {
        debugger;
        var placeHolderDiv = $("#" + "srmmigration-bulk-upload-placeholder-upload");
        commonBulkUploadDiff(placeHolderDiv);
        var self = srmMigrationUtility._pageViewModelInstance;
        self.isBulkUploadClicked(!self.isBulkUploadClicked());
        $("#srm-migration-upload-listview-button span").text("Upload");
        $(".srmmigration-action-text-on-click span").text("UPLOAD");
    }

    function setBulkDownloadDiffDetails(event) {
        debugger;
        var placeHolderDiv = $("#" + "srmmigration-bulk-upload-placeholder-download-diff");
        commonBulkUploadDiff(placeHolderDiv);
        var self = srmMigrationUtility._pageViewModelInstance;
        self.isBulkDownloadDiffClicked(!self.isBulkDownloadDiffClicked());
        $("#srm-migration-upload-listview-button span").text("Download Delta");
        $(".srmmigration-action-text-on-click span").text("COMPARE");
    }

    function equalizeHeights_Old() {
        debugger;
        var parentDiv = $(".srmmigration-selected-list-view").first();
        var childDiv = $(".srmmigration-selected-list-item");
        if (childDiv.first() == null || parentDiv == null)
            return false;

        var rowSize = 5;
        var maxHeight = 0;
        for (var i = 0; i < childDiv.length; i++) {

            maxHeight = Math.max(maxHeight, childDiv.eq(i).outerHeight());
            if ((i + 1) % rowSize == 0) {
                for (var j = i - rowSize + 1; j <= i; j++) {
                    childDiv.eq(j).css('height', maxHeight);
                }
                maxHeight = 0;
            }
            else if (childDiv.length - i <= rowSize) {
                for (var j = i; j < childDiv.length; j++) {
                    childDiv.eq(j).css('height', maxHeight);
                }
            }
        }

    }

    function equalizeHeights() {
        debugger;
        var parentDiv = $(".srmmigration-selected-list-view").first();
        var childDiv = $(".srmmigration-selected-list-item");
        if (childDiv.first() == null || parentDiv == null)
            return false;

        var rowSize = 5;
        var maxHeight = 0;

        //initial rows
        var remainder = childDiv.length % rowSize;
        var loop1 = childDiv.length - remainder;
        var loop2 = remainder > 0 ? loop1 + remainder : 0;

        for (var i = 0; i < loop1; i++) {
            maxHeight = Math.max(maxHeight, childDiv.eq(i).outerHeight());

            if (i != 0 && ((i + 1) % rowSize == 0)) {
                for (var j = i - rowSize + 1; j <= i; j++) {
                    childDiv.eq(j).css('height', maxHeight);
                }
                maxHeight = 0;
            }
        }

        maxHeight = 0;

        for (var i = loop1; i < loop2; i++) {
            maxHeight = Math.max(maxHeight, childDiv.eq(i).outerHeight());
        }

        for (var i = loop1; i < loop2; i++) {
            childDiv.eq(i).css('height', maxHeight);
        }
    }

    //bulk buttons final click handlers
    function downloadMasterClick(vm, event) {
        var featuresList = [];
        ko.utils.arrayForEach(vm.moduleList(), (item) => {
            item.isSelected() ? featuresList.push(item.id) : "";
        });

        //fileType
        var isExcel = true;
        var clickContextParent = $("#" + bulkDownloadDetailsId);

        //old
        //if ($("." + excelType, clickContextParent).hasClass(selectionClassForXmlExcelToggle))
        //    isExcel = true;
        //else
        //    isExcel = false;
        debugger;
        var clickedDiv = $(event.target);
        if (clickedDiv.is("#srmmigration-bulk-download-excel-click") || clickedDiv.parent().is("#srmmigration-bulk-download-excel-click"))
            isExcel = true;
        else
            isExcel = false;

        var params = {
            features: featuresList,
            selectedItems: [],
            moduleID: srmMigrationUtility._moduleID,
            userName: srmMigrationUtility._username,
            isExcelFile: isExcel
        };

        debugger;

        CallCommonServiceMethod('DownloadMigrationConfiguration', params, OnSuccess_DownloadData, OnFailure, null, false);
    }

    function getPrivilegesForModules() {
        //debugger;
        srmMigrationUtility._pageViewModelInstance.privilegeForCompare(false);
        srmMigrationUtility._pageViewModelInstance.privilegeForUpload(false);
        var params = {
            userName: srmMigrationUtility._username,
            privilegeName: privilegeList[srmMigrationUtility._moduleID]
        }
        CallCommonServiceMethod('CheckControlPrivilegeForUserMultiple', params, OnSuccess_GetPrivileges, OnFailure, null, false);
    }

    function OnSuccess_GetPrivileges(data) {
        SecMasterJSCommon.SMSCommons.onUpdated();
        var privileges = data.d;
        //first is upload, second is for diff
        if (privileges != null && privileges.length == 2) {
            srmMigrationUtility._pageViewModelInstance.privilegeForCompare(privileges[1].result);
            srmMigrationUtility._pageViewModelInstance.privilegeForUpload(privileges[0].result)
        }
        //first is upload
        //second is compare

        //privileges.forEach(x=>{if(x.name == privilegeList[srmMigrationUtility._moduleID][])});
    }


    //Entry method from aspx
    SRMMigrationUtility.prototype.Initializer = function Initializer(username, dateFormat) {
        modulesInitialise(username);
        srmMigrationUtility._dateInfo = eval("(" + dateFormat + ")");
        $(window).resize(setScreenSize);
        $("div").addClass('srmmigration-border-box');
    }

    SRMMigrationUtility.prototype.testingUploadDiv = function () {
        var list = [];
        for (let i = 0; i < 5; i++) {
            list.push({
                FeatureEnum: i,
                ModuleID: 6,
                ErrorMsg: "This is a text msg." + i,
                IsSyncComplete: i % 2 == 0 ? true : false,
                FilePath: "test",
                FileName: "testFileName" + i,
                FeatureDisplayName: "FeatureDisplayname" + i
            });
        }
        displayPopupForUploadResult(list);
    }



    //SRMMigrationUtility.prototype.fileDataUpload = function (fileData, filePath) {
    //    debugger;
    //}

    //Deprecated Methods
    //for when absolute positioning was being used for popup)
    function openDownloadSelectionPopup(vm, event) {
        //clear all


        //try{
        //    $("." + listViewParentClass).removeClass(dispListViewParent).addClass(dispNoneClass);

        //    //display current
        //    var currentListItem = $("." + listViewParentClass, $(event.target).closest("." + moduleItemParent))
        //    currentListItem.removeClass(dispNoneClass).addClass(dispListViewParent);

        //    var itemParent = $("." + dispListViewParent).offset();
        //    var left = itemParent.left;
        //    var right = itemParent.right;

        //    var featureListDiv = $("#" + featureListParent);
        //    var featureLastDivRightSide = featureListDiv.offset().right;


        //}
        //catch(ex){
        //    console.log("openDownloadSelectionPopup" + ex);
        //}
    }

    //function setUploadDiv(vm, event) {
    //    findCorrectRelativePositionForDiv(vm, event);

    //    var placeHolderDiv = getRelativePlaceholderDiv();
    //    var uploadParentDiv = $("#" + uploadParentId);

    //    placeHolderDiv.append(uploadParentDiv);

    //    //animate in future
    //    uploadParentDiv.removeClass(dispNoneClass);
    //    SetFileUploadControl('srmmigration-upload-section', 'SRMMigrationFileUploadAttachement', 'SRMMigrationFileUpload');
    //}

    //FILE UPLOAD INITIATION CODE - START

    function SetFileUploadControl(parentId, attachmentId, fileUploadId) {
        $('#' + fileUploadId).remove();
        $('#' + parentId).append('<div  style="border:0px;" id="' + fileUploadId + '"><div class="SRMMigrationLabeledInput" style="width: 100%; border:0px;padding: 0px; text-indent: 8px;" id="' + attachmentId + '"> Click here or drop a file to upload</div>');
        if ($('#' + fileUploadId).fileUpload != undefined) {
            $('#' + fileUploadId).fileUpload({
                'parentControlId': fileUploadId,
                'attachementControlId': attachmentId,
                'multiple': false,
                'debuggerDiv': '',
                'returnEvent': function () {
                },
                'deleteEvent': function () {
                }
            });
        }
    }


    //FILE UPLOAD INITIATION CODE - END

    return srmMigrationUtility;

})();
