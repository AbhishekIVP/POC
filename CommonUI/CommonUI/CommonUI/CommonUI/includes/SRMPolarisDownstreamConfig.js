var polarisdownstream = (function () {
    function PolarisDownstream() {
        this._pageViewModelInstance = null;
        this._completeObj = null;
        this._totalReports = null;
        this._availableDateTypes = null;
        this._availableDateTypesEnd = null;
        this._allTimeSeries = [];
        this._availableTransports = null;
        this._availableCalendars = null;
        this._securityInfo = null;
        this._RequiredValidationCount = 0;
        this._selectedReports = null;
        this._selectedSystemName = null;
        this._selectedConnectionName = null;
        this._noReportsConfigured = true;
        this.reportSelectionOpen = false;
        this._selectedSystemIndex = 0;
        this._isClone = false;
        this._subscriberList = null;
    }
    var polarisdownstream = new PolarisDownstream();
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    $(document).click(function (e) {

        if ($(".smselectcon").css('display') === 'block') {
            $(".smselectcon").css('display', 'none');
            $('#PolarisDownstreamAddNewReportBtn').click();
        }

        if (e.target.id != "PolarisDownstremAddNewSystemBtn" && e.target.id != "PolarisDownstreamSaveButton") {
            if (polarisdownstream._selectedReports != null && polarisdownstream._selectedReports != 'undefined' && polarisdownstream._selectedReports.length == 0) {

                $("#PolarisDownstreamSelectedReportDetailsMainContainer").hide();
                if (polarisdownstream.reportSelectionOpen) {
                    polarisdownstream.reportSelectionOpen = false;
                    $("#PolarisDownstreamAddNewReportBtn").click();
                }
                e.stopPropagation();
            }
            else if (polarisdownstream._selectedReports != null && polarisdownstream._selectedReports != 'undefined' && polarisdownstream._selectedReports.length > 0) {

                $("#PolarisDownstreamSelectedReportDetailsMainContainer").show();
                if (polarisdownstream.reportSelectionOpen) {
                    polarisdownstream.reportSelectionOpen = false;
                    $("#PolarisDownstreamAddNewReportBtn").click();
                }
                e.stopPropagation();
            } else {
                e.stopPropagation();
            }
        } else if (e.target.id == 'successSubscribtionListContainer') {
            polarisdownstream._pageViewModelInstance.SelectedFailureEmail($('#successSubscribtionListContainer .hdnTag').val());
        }
        else {
            e.stopPropagation();
        }
    });

    ko.extenders.withDefault = function (target, defaultValue) {
        target.subscribe(function (input) {
            if (!input) target(defaultValue)
        });
        return target
    };

    ko.extenders.required = function (target, overrideMessage) {
        //add some sub-observables to our observable
        target.hasError = ko.observable();
        target.validationMessage = ko.observable();

        //define a function to do validation
        function validate(newValue) {
            if (newValue != 0) {
                target.hasError(newValue ? false : true);
                target.validationMessage(newValue ? "" : overrideMessage || "*");
                if (target.hasError()) {
                    polarisdownstream._RequiredValidationCount += 1;
                }
                else {
                    if (polarisdownstream._RequiredValidationCount > 1)
                        polarisdownstream._RequiredValidationCount -= 1;
                }
            }
        }

        //initial validation
        validate(target());

        //validate whenever the value changes
        target.subscribe(validate);

        //return the original observable
        return target;
    };

    function pageViewModel(data) {

        var self = this;
        //self.completeObject = ko.observableArray(data);
        self.BlockTypes = ko.observableArray();

        self.ReportChainListing = ko.observableArray();
        self.selectedSystemChainItem = null;
        self.selectedReportChainItem = null;
        self.selectedBlockType = ko.observable();
        self.selectedServerName = ko.observable();
        self.selectedDbName = ko.observable();
        self.selectedRealDbName = ko.observable();
        self.selectedUserName = ko.observable();
        self.selectedPassword = ko.observable();
        self.selectedEffectiveDate = ko.observable();
        self.reportTypes = ko.observableArray();
        self.availableTransports = ko.observableArray();
        self.availableCalendars = ko.observableArray();
        self.availableReports = ko.observableArray();
        if (polarisdownstream._noReportsConfigured)
            self.SelectedStartDateValue = ko.observable('').extend({ notify: 'always' });
        else
            self.SelectedStartDateValue = ko.observable('').extend({ notify: 'always' }).extend({ required: true });
        if (polarisdownstream._noReportsConfigured)
            self.SelectedEndDateValue = ko.observable('').extend({ notify: 'always' });
        else
            self.SelectedEndDateValue = ko.observable('').extend({ notify: 'always' }).extend({ required: true });
        self.SelectedCustomStartDate = ko.observable();
        self.SelectedCustomEndDate = ko.observable();
        self.SelectedRequireKnowledgeDateReporting = ko.observable(false).extend({ notify: 'always' });;
        self.SelectedRequireTimeInTSReport = ko.observable(false).extend({ notify: 'always' });;
        self.SelectedRequireDeletedAssetTypes = ko.observable(false).extend({ notify: 'always' });;
        self.SelectedRequireLookupMassagingStartDate = ko.observable(false).extend({ notify: 'always' });;
        self.SelectedRequireLookupMassagingCurrentKnowledgeDate = ko.observable(false).extend({ notify: 'always' });;
        self.SelectedCCAssemblyName = ko.observable();
        self.SelectedCCClassName = ko.observable();
        self.SelectedCCMethodName = ko.observable();
        if (polarisdownstream._noReportsConfigured) {
            self.SelectedTableName = ko.observable();
        }
        else {
            self.SelectedTableName = ko.observable()
                .extend({ required: "" })
                .extend({ withDefault: "some Default" });
        }
        self.SelectedBatchSize = ko.observable()
            .extend({ notify: 'always' })
            .extend({ required: "*" });

        self.SelectedFailureEmail = ko.observable('')
            .extend({ notify: 'always' });

        self.SelectedQueueName = ko.observable().extend({ notify: 'always' });
        self.selectedCalendarName = ko.observable().extend({ required: "*" });
        self.selectedEffectiveDate = ko.observable().extend({ required: "*" });
        self.ccc = ko.observable();
        self.SelectedReportName = ko.observable();
        self.SelectedReportType = ko.observable();
        self.SystemChainListing = ko.observableArray();
        self.BlockTypeChain = ko.observableArray();
        self.SelectedReportId = ko.observable();
        self.SelectedModule = ko.observable();
        self.IsTS = ko.observable(false);

        self.SystemChainListing([]);
        for (var item in data) {
            self.SystemChainListing.push(new SystemChainViewModel(data[item]));
        }
        self.SystemChainListing.valueHasMutated();

        self.ComputedReportName = ko.computed(function () {
            return self.SelectedModule() == "Ref" ? self.SelectedReportName() + " Reference Report" : self.SelectedReportName();
        });
        if (polarisdownstream._noReportsConfigured) {
            self.IsTableNameInvalid = ko.computed(function () {
                return false;
            });
        }
        else {
            self.IsTableNameInvalid = ko.computed(function () {
                if (self.SelectedTableName() != undefined)
                    return self.SelectedTableName().includes("].[") ? false : true;
            });
        }
        self.IsEmailIdInvalid = ko.computed(function () {
            return false;
        });
        self.availableDateTypes = ko.observableArray();
        self.availableDateTypesEnd = ko.observableArray();
        for (key in polarisdownstream._availableTransports) {
            var item = {
                text: polarisdownstream._availableTransports[key],
                value: polarisdownstream._availableTransports[key]
            }
            self.availableTransports.push(item);
        }
        for (key in polarisdownstream._availableCalendars) {
            var item = {
                text: polarisdownstream._availableCalendars[key],
                value: key
            }
            self.availableCalendars.push(item);
        }
        for (key in polarisdownstream._availableDateTypes) {
            var item = {
                text: key,
                value: polarisdownstream._availableDateTypes[key],
            }
            self.availableDateTypes.push(item);
        }
        for (key in polarisdownstream._availableDateTypesEnd) {
            var item = {
                text: key,
                value: polarisdownstream._availableDateTypesEnd[key],
            }
            self.availableDateTypesEnd.push(item);
        }
        self.SelectedTableName.subscribe(function (value, e) {
            var report = getCurrentReportObject();
            if (report != undefined)
                report.Details.TableName = value;
        })
        self.SelectedBatchSize.subscribe(function (value, e) {
            var report = getCurrentReportObject();
            if (report != undefined)
                report.Details.BatchSize = value;
        })
        self.SelectedFailureEmail.subscribe(function (value, e) {
            var report = getCurrentReportObject();
            if (report != undefined)
                report.Details.FailureEmail = value;
        })
        self.SelectedQueueName.subscribe(function (value, e) {
            var report = getCurrentReportObject();
            if (report != undefined)
                report.Details.QueueName = value;
        })

        self.SelectedRequireKnowledgeDateReporting.subscribe(function (value, e) {
            var report = getCurrentReportObject();
            if (report != undefined)
                report.Details.RequireKnowledgeDateReporting = value;
        })
        self.SelectedRequireDeletedAssetTypes.subscribe(function (value, e) {
            var report = getCurrentReportObject();
            if (report != undefined)
                report.Details.RequireDeletedAssetTypes = value;
        })
        self.SelectedRequireTimeInTSReport.subscribe(function (value, e) {
            var report = getCurrentReportObject();
            if (report != undefined)
                report.Details.RequireTimeInTSReport = value;
        })
        self.SelectedCCAssemblyName.subscribe(function (value, e) {
            var report = getCurrentReportObject();
            if (report != undefined)
                report.Details.CCAssemblyName = value;
        })
        self.SelectedCCClassName.subscribe(function (value, e) {
            var report = getCurrentReportObject();
            if (report != undefined)
                report.Details.CCClassName = value;
        })
        self.SelectedCCMethodName.subscribe(function (value, e) {
            var report = getCurrentReportObject();
            if (report != undefined)
                report.Details.CCMethodName = value;
        })
        self.SelectedStartDateValue.subscribe(function (value, e) {
            var report = getCurrentReportObject();
            if (report != undefined)
                report.Details.StartDateValue = value;
        })
        self.SelectedEndDateValue.subscribe(function (value, e) {
            var report = getCurrentReportObject();
            if (report != undefined)
                report.Details.EndDateValue = value;
        })
        self.SelectedCustomStartDate.subscribe(function (value, e) {
            var report = getCurrentReportObject();
            if (report != undefined)
                report.Details.CustomStartDate = value;
        })
        self.SelectedCustomEndDate.subscribe(function (value, e) {
            var report = getCurrentReportObject();
            if (report != undefined)
                report.Details.CustomEndDate = value;
        })
        self.SelectedRequireLookupMassagingStartDate.subscribe(function (value, e) {
            var report = getCurrentReportObject();
            if (report != undefined)
                report.Details.RequireLookupMassagingStartDate = value;
        })
        self.SelectedRequireLookupMassagingCurrentKnowledgeDate.subscribe(function (value, e) {
            var report = getCurrentReportObject();
            if (report != undefined)
                report.Details.RequireLookupMassagingCurrentKnowledgeDate = value;
        })
        self.selectedEffectiveDate.subscribe(function (value, e) {
            var system = getCurrentSystemObject();
            if (system != undefined)
                system.SetupDetails.EffectiveDate = value;
        });
        self.selectedCalendarName
            .subscribe(function (value, e) {
                var system = getCurrentSystemObject();
                if (system != undefined)
                    system.SetupDetails.CalendarName = value;
            });

        $(".PolarisDownstreamChain").css("display", "block");
        CallCustomDatePicker($('[id*="SelectedEffectiveDate"]').prop('id').trim(), polarisdownstream._securityInfo.CustomJQueryDateFormat, function (d) {
            polarisdownstream._pageViewModelInstance.selectedEffectiveDate(d.format("MM/dd/yyyy"));
        }, optionDateTime.DATE, 15, true);

        $('#SelectedEffectiveDate').change(function () {
            var startDate = $(this).datepicker('getDate');
            var selectedStartDay = (startDate.getMonth() + 1) + "/" + startDate.getDate() + "/" + startDate.getFullYear();
            polarisdownstream._pageViewModelInstance.selectedEffectiveDate(selectedStartDay);
            console.log(selectedStartDay);
        })
    };

    function getCurrentReportObject() {
        if (polarisdownstream._pageViewModelInstance.selectedSystemChainItem.systemName() == null) {
            polarisdownstream._selectedSystemName = polarisdownstream._pageViewModelInstance.selectedSystemChainItem.systemName();
        }
        var selectedSystemObject = $.grep(polarisdownstream._completeObj, function (e) { return e.SystemName == polarisdownstream._pageViewModelInstance.selectedSystemChainItem.systemName() });
        var BT = selectedSystemObject[0].BlockType.find(obj => {
            return obj.BlockTypeName == polarisdownstream._pageViewModelInstance.SelectedReportType()
        })
        var report = BT.ReportsAvailable.find(obj => {
            return obj.Id == polarisdownstream._pageViewModelInstance.SelectedReportId() && obj.Module == polarisdownstream._pageViewModelInstance.SelectedModule()
        })
        return report;
    }

    function getCurrentSystemObject() {
        return $.grep(polarisdownstream._completeObj, function (e) { return e.SystemName == polarisdownstream._pageViewModelInstance.selectedSystemChainItem.systemName() })[0];
    }

    function SystemChainViewModel(data) {

        polarisdownstream._selectedSystemName = data.SystemName;
        var self = this;
        self.systemName = ko.observable(data.SystemName);
        self.isSystemSelected = ko.observable(false);
        self.serverName = ko.observable(data.SetupDetails.ServerName).extend({
            required: true
        }),
            self.userName = ko.observable(data.SetupDetails.UserName);
        self.dbName = ko.observable(data.SetupDetails.DbName);
        self.realDbName = ko.observable(data.SetupDetails.RealDbName);
        self.password = ko.observable(data.SetupDetails.Password);
        self.calendarName = ko.observable(data.SetupDetails.CalendarName);
        if (data.SetupDetails.EffectiveDate != null && data.SetupDetails.EffectiveDate != undefined && data.SetupDetails.EffectiveDate.trim() != '') {
        } else {
            data.SetupDetails.EffectiveDate = new Date().format("MM/dd/yyyy");
        }
        self.effectiveDate = ko.observable(data.SetupDetails.EffectiveDate);

        self.ReportTypeChainListing = ko.observableArray();
        self.ReportTypeChainListing([]);
        for (var i in polarisdownstream._allTimeSeries) {
            var f = polarisdownstream._allTimeSeries[i].Type();
            for (var item in polarisdownstream._completeObj[0].BlockType) {
                if (polarisdownstream._completeObj[0].BlockType[item].BlockTypeName == f) {
                    // polarisdownstream._allTimeSeries[item].ReportsAvailable.push(polarisdownstream._completeObj[0].BlockType[item].ReportsAvailable);
                    polarisdownstream._allTimeSeries[i].ReportsAvailable([...polarisdownstream._completeObj[0].BlockType[item].ReportsAvailable]);
                    polarisdownstream._allTimeSeries[i].IsBlockTypeConfigured(polarisdownstream._completeObj[0].BlockType[item].IsBlockTypeConfigured);
                }
            }
            //
            self.ReportTypeChainListing.push(new ReportTypeChainViewModel(polarisdownstream._allTimeSeries[i]));
        }
    };

    function ReportTypeChainViewModel(data) {
        //
        var self = this;
        self.IsActive = ko.observable(false);
        self.IsBlockTypeConfigured = ko.observable(data.IsBlockTypeConfigured());
        self.Type = ko.observable(data.Type());
        self.ReportsAvailable = ko.observableArray(data.ReportsAvailable());
    };
    function ReportChainViewModel(data) {
        var self = this;
        self.isSelected = ko.observable(false);
        self.chainName = ko.observable(data.Name);
        self.chainId = ko.observable(data.Id);
        self.chainBackgroundColor = ko.computed(function () {
            return self.isSelected() ? '#72b9e6' : '#f7faff';
        });
        self.module = ko.observable(data.Module);
        self.requireDeletedRowLabelName = ko.computed(function () {
            return self.module == "Ref" ? "Entities" : "Securities";
        })
        self.chainDetailsData = data.Details;
        self.IsReportConfigured = data.IsReportConfigured;
    };

    $("#PolarisDownstreamSaveCloneButton").unbind('click').bind('click', function (e, f) {

        polarisdownstream._isClone = true;
        polarisdownstream._selectedSystemName = $("#PolarisDownstreamClonedSystemNameInput").val();
        var selectedConnectionName = smselect.getSelectedOption($("#smselect_polarisDownstreamSelectedCloneConnection"))[0].text;
        if (polarisdownstream._selectedSystemName == "" || polarisdownstream._selectedSystemName == null) {
            $("#PolarisDownstreamClonedSystemNameInputValidation").show();
        } else if ($.grep(polarisdownstream._completeObj, function (e) { return e.SystemName.toLowerCase().trim() == polarisdownstream._selectedSystemName.toLowerCase().trim() }).length > 0) {
            PolarisDownstream.prototype.showErrorDiv('Error', 'fail_icon.png', "System with same name already exists");
        } else if (smselect.getSelectedOption($("#smselect_polarisDownstreamSelectedCloneConnection")).length == 0) {
            $("#PolarisDownstreamCloneConnectionInputValidation").show();

        } else if ($.grep(polarisdownstream._completeObj, function (e) { return e.SetupDetails.DbName.toLowerCase().trim() == selectedConnectionName.toLowerCase().trim() }).length > 0) {
            PolarisDownstream.prototype.showErrorDiv('Error', 'fail_icon.png', "Connection already in use.");
        }
        else {
            $("#PolarisDownstreamClonedSystemNameInputValidation").hide();
            $("#PolarisDownstreamCloneConnectionInputValidation").hide();

            var params = {
                connectionName: selectedConnectionName,
            };
            CallCommonServiceMethod('GetSelectedConnectionDetails', params, OnSuccess_GetSelectedConnectionDetails, OnFailure, null, false);
        }
    });

    // Selected Report Click Handler
    PolarisDownstream.prototype.onClickReportChain = function onClickReportChain(obj, event) {

        polarisdownstream._noReportsConfigured = false;
        //Handling for arrow of Selected Report
        if (polarisdownstream._pageViewModelInstance.selectedReportChainItem != null) {
            polarisdownstream._pageViewModelInstance.selectedReportChainItem.isSelected(false);
        }
        polarisdownstream._pageViewModelInstance.selectedReportChainItem = obj;
        polarisdownstream._pageViewModelInstance.selectedReportChainItem.isSelected(true);

        polarisdownstream._pageViewModelInstance.SelectedModule(obj.module());
        if (obj.chainName().startsWith("SM ")) {
            obj.chainName().replace("SM ", '');
        } else if (obj.chainName().startsWith("RM ")) {
            obj.chainName().replace("RM ", '');
        }

        polarisdownstream._pageViewModelInstance.SelectedReportName(obj.chainName());
        polarisdownstream._pageViewModelInstance.SelectedReportId(obj.chainId());
        if (obj.chainDetailsData.TableName == null || obj.chainDetailsData.TableName == "") {
            if (obj.module().toLowerCase() == "ref") {
                polarisdownstream._pageViewModelInstance.SelectedTableName("[references].[ivp_polaris_" +
                    polarisdownstream._pageViewModelInstance.SelectedReportName().replace(/ /g, "_").toLowerCase() + "_staging]");
            } else if (obj.module().toLowerCase() == "sec") {
                polarisdownstream._pageViewModelInstance.SelectedTableName("[taskmanager].[ivp_polaris_" +
                    polarisdownstream._pageViewModelInstance.SelectedReportName().replace(/ /g, "_").toLowerCase() + "_staging]");
            }
        }
        else {
            polarisdownstream._pageViewModelInstance.SelectedTableName(obj.chainDetailsData.TableName);
        }
        if (obj.IsReportConfigured) {
            //if (obj.module().toLowerCase() == "ref") {
            //    $("#PolarisDownstreamSelectedReportRefKnowledgeDateChk").attr("disabled", true);
            //    $("#PolarisDownstreamSelectedReportEffectiveDateChk").attr("disabled", true);
            //}
            //else if (obj.module().toLowerCase() == "sec") {
            //    $("#PolarisDownstreamSelectedReportRefKnowledgeDateChk").removeAttr("disabled");
            //    $("#PolarisDownstreamSelectedReportEffectiveDateChk").removeAttr("disabled");
            //}

            if (obj.chainDetailsData.BatchSize == null) {
                polarisdownstream._pageViewModelInstance.SelectedBatchSize(1000);
            } else {
                polarisdownstream._pageViewModelInstance.SelectedBatchSize(obj.chainDetailsData.BatchSize);
            }

            polarisdownstream._pageViewModelInstance.SelectedRequireKnowledgeDateReporting(obj.chainDetailsData.RequireKnowledgeDateReporting);
            polarisdownstream._pageViewModelInstance.SelectedRequireDeletedAssetTypes(obj.chainDetailsData.RequireDeletedAssetTypes);
            polarisdownstream._pageViewModelInstance.SelectedRequireTimeInTSReport(obj.chainDetailsData.RequireTimeInTSReport);


            if (obj.chainDetailsData.StartDateValue == null || obj.chainDetailsData.StartDateValue == "") {

                ApplyMultiSelect('polarisDownstreamAvailableDateTypesStartItems', false, polarisdownstream._pageViewModelInstance.availableDateTypes(), true, '#PolarisDownstreamSelectedReportStartDateInputDiv', 'Date Types', false, "", false);
                smselect.setOptionByText($("#smselect_polarisDownstreamAvailableDateTypesStartItems"), "LastExtractionDate", false);
                polarisdownstream._pageViewModelInstance.SelectedStartDateValue(smselect.getSelectedOption($("#smselect_polarisDownstreamAvailableDateTypesStartItems"))[0].text);
            } else {
                polarisdownstream._pageViewModelInstance.SelectedStartDateValue(obj.chainDetailsData.StartDateValue);
                ApplyMultiSelect('polarisDownstreamAvailableDateTypesStartItems', false, polarisdownstream._pageViewModelInstance.availableDateTypes(), true, '#PolarisDownstreamSelectedReportStartDateInputDiv', 'Date Types', false, polarisdownstream._pageViewModelInstance.SelectedStartDateValue(), false);
            }
            if (polarisdownstream._pageViewModelInstance.SelectedStartDateValue() == "LastExtractionDate") {
                ApplyMultiSelect('polarisDownstreamAvailableDateTypesEndItems', false, polarisdownstream._pageViewModelInstance.availableDateTypesEnd(), true, '#PolarisDownstreamSelectedReportEndDateLabelInputDiv', 'Date Types', false, polarisdownstream._pageViewModelInstance.SelectedEndDateValue(), false);
                smselect.setOptionByText($("#smselect_polarisDownstreamAvailableDateTypesEndItems"), "Now", false);
                polarisdownstream._pageViewModelInstance.SelectedEndDateValue(smselect.getSelectedOption($("#smselect_polarisDownstreamAvailableDateTypesEndItems"))[0].text);
            }
            else {
                polarisdownstream._pageViewModelInstance.SelectedEndDateValue(obj.chainDetailsData.EndDateValue);
                ApplyMultiSelect('polarisDownstreamAvailableDateTypesEndItems', false, polarisdownstream._pageViewModelInstance.availableDateTypesEnd(), true, '#PolarisDownstreamSelectedReportEndDateLabelInputDiv', 'Date Types', false, polarisdownstream._pageViewModelInstance.SelectedEndDateValue(), false);
            }
            polarisdownstream._pageViewModelInstance.SelectedCustomStartDate(obj.chainDetailsData.CustomStartDate);
            polarisdownstream._pageViewModelInstance.SelectedCustomEndDate(obj.chainDetailsData.CustomEndDate);
            // polarisdownstream._pageViewModelInstance.SelectedRequireKnowledgeDateReporting(obj.chainDetailsData.RequireKnowledgeDateReporting);
            polarisdownstream._pageViewModelInstance.SelectedRequireLookupMassagingStartDate(obj.chainDetailsData.RequireLookupMassagingStartDate);
            polarisdownstream._pageViewModelInstance.SelectedRequireLookupMassagingCurrentKnowledgeDate(obj.chainDetailsData.RequireLookupMassagingCurrentKnowledgeDate);
            polarisdownstream._pageViewModelInstance.SelectedCCAssemblyName(obj.chainDetailsData.CCAssemblyName);
            polarisdownstream._pageViewModelInstance.SelectedCCClassName(obj.chainDetailsData.CCClassName);
            polarisdownstream._pageViewModelInstance.SelectedCCMethodName(obj.chainDetailsData.CCMethodName);
            polarisdownstream._pageViewModelInstance.SelectedQueueName(obj.chainDetailsData.QueueName);
            // polarisdownstream._pageViewModelInstance.SelectedRequireKnowledgeDateReporting(obj.chainDetailsData.RequireKnowledgeDateReporting);
            polarisdownstream._pageViewModelInstance.SelectedReportName(obj.chainName());
        }
        else {
            polarisdownstream._pageViewModelInstance.SelectedBatchSize(1000);
            PolarisDownstream.prototype.EmptySelectedReportDetails();
            setDefaultReportDetails();
        }
        if (polarisdownstream._pageViewModelInstance.SelectedQueueName() == null || polarisdownstream._pageViewModelInstance.SelectedQueueName() == undefined || polarisdownstream._pageViewModelInstance.SelectedQueueName().trim() == '') {
            ApplyMultiSelect('polarisDownstreamAvailableTransportsMultiSelectedItems', false, polarisdownstream._pageViewModelInstance.availableTransports(), true, '#PolarisDownstreamSelectedReportQueueNotificationInputDiv', 'Queues', false, [], true);
            //smselect.setOptionByText($("#smselect_polarisDownstreamAvailableTransportsMultiSelectedItems"), "secmasterqueue", false);
            polarisdownstream._pageViewModelInstance.SelectedQueueName("");
        } else {
            var selectedItems = polarisdownstream._pageViewModelInstance.SelectedQueueName().split(",");
            ApplyMultiSelect('polarisDownstreamAvailableTransportsMultiSelectedItems', false, polarisdownstream._pageViewModelInstance.availableTransports(), true, '#PolarisDownstreamSelectedReportQueueNotificationInputDiv', 'Queues', false, selectedItems, true);
        }

        CallCustomDatePicker($('[id*="InputCustomDateStart"]').prop('id').trim(), polarisdownstream._securityInfo.CustomJQueryDateFormat, null, optionDateTime.DATE, 15, true);
        CallCustomDatePicker($('[id*="InputCustomDateEnd"]').prop('id').trim(), polarisdownstream._securityInfo.CustomJQueryDateFormat, null, optionDateTime.DATE, 15, true);

        convertCheckboxToToggle(PolarisDownstreamSelectedRequireKnowledgeDateReportingChk.id, true);
        convertCheckboxToToggle(PolarisDownstreamSelectedReportRequireDeletedRowChk.id, true);
        if (polarisdownstream._pageViewModelInstance.IsTS()) {
            convertCheckboxToToggle(PolarisDownstreamSelectedReportRemoveTimePartChk.id, true);
            convertCheckboxToToggleKDTD(PolarisDownstreamSelectedReportRefKnowledgeDateChk.id, true);
            convertCheckboxToToggleSDED(PolarisDownstreamSelectedReportEffectiveDateChk.id, true);
        }
        polarisdownstream._pageViewModelInstance.SelectedFailureEmail(obj.chainDetailsData.FailureEmail);

        bindMultiDropdown();
    }

    PolarisDownstream.prototype.EmptySelectedReportDetails = function EmptySelectedReportDetails(obj, event) {
        polarisdownstream._pageViewModelInstance.SelectedBatchSize(1000);
        polarisdownstream._pageViewModelInstance.SelectedFailureEmail('');
        polarisdownstream._pageViewModelInstance.SelectedStartDateValue('');
        polarisdownstream._pageViewModelInstance.SelectedEndDateValue('');
        polarisdownstream._pageViewModelInstance.SelectedCustomStartDate('');
        polarisdownstream._pageViewModelInstance.SelectedCustomEndDate('');
        polarisdownstream._pageViewModelInstance.SelectedRequireTimeInTSReport(false);
        polarisdownstream._pageViewModelInstance.SelectedRequireKnowledgeDateReporting(false);
        polarisdownstream._pageViewModelInstance.SelectedRequireDeletedAssetTypes(false);
        polarisdownstream._pageViewModelInstance.SelectedRequireLookupMassagingStartDate(false);
        polarisdownstream._pageViewModelInstance.SelectedRequireLookupMassagingCurrentKnowledgeDate(false);
        polarisdownstream._pageViewModelInstance.SelectedCCAssemblyName('');
        polarisdownstream._pageViewModelInstance.SelectedCCClassName('');
        polarisdownstream._pageViewModelInstance.SelectedCCMethodName('');
        polarisdownstream._pageViewModelInstance.SelectedQueueName();
        smselect.reset($("#smselect_polarisDownstreamAvailableTransportsMultiSelectedItems"));
        smselect.reset($("#smselect_polarisDownstreamAvailableDateTypesStartItems"));
        smselect.reset($("#smselect_polarisDownstreamAvailableDateTypesEndItems"));
    }

    //System Tab Click Handler
    PolarisDownstream.prototype.onClickSystemChain = function onClickSystemChain(obj, event) {

        let arr = $("#PolarisDownstreamProductsContainer").find(".ActiveSetup");
        arr = arr.toArray();
        for (var item in arr) {
            $(arr[item]).removeClass('ActiveSetup')
        }
        $(event.currentTarget).addClass("ActiveSetup");
        polarisdownstream._selectedSystemName = obj.systemName();
        if (polarisdownstream._pageViewModelInstance.selectedSystemChainItem.systemName() == obj.systemName()) { }
        else {

            if (polarisdownstream._pageViewModelInstance.selectedSystemChainItem != null) {
                polarisdownstream._pageViewModelInstance.selectedSystemChainItem.isSystemSelected(false);
            }

            if (polarisdownstream._IsNewSystem) {
                polarisdownstream._pageViewModelInstance.selectedSystemChainItem = obj;
                polarisdownstream._pageViewModelInstance.selectedSystemChainItem.isSystemSelected(true);
                polarisdownstream._pageViewModelInstance.selectedServerName(obj.serverName());
                polarisdownstream._pageViewModelInstance.selectedDbName(obj.dbName());
                polarisdownstream._pageViewModelInstance.selectedRealDbName(obj.realDbName());
                polarisdownstream._pageViewModelInstance.selectedUserName(obj.userName());
                polarisdownstream._pageViewModelInstance.selectedPassword(obj.password());
                polarisdownstream._pageViewModelInstance.SystemChainListing()[0].effectiveDate(obj.effectiveDate().split(" ")[0]);
                polarisdownstream._pageViewModelInstance.selectedCalendarName(obj.calendarName());
                ApplyMultiSelect('polarisDownstreamAvailableCalendarsMultiSelectedItems', false, polarisdownstream._pageViewModelInstance.availableCalendars(), true, '#SelectedSetupCalender', 'Calendars', false, obj.calendarName(), false);
                polarisdownstream._pageViewModelInstance.reportTypes([]);

                for (var item in obj.ReportTypeChainListing()) {
                    polarisdownstream._pageViewModelInstance.reportTypes.push(new ReportTypeChainViewModel(obj.ReportTypeChainListing()[item]));
                }
                $('.TimeSeriesTab').bind('click', { param1: polarisdownstream._pageViewModelInstance.reportTypes[0] }, polarisdownstream.onClickReportTypeChain);
                $('.TimeSeriesTab')[0].click();
                polarisdownstream._IsNewSystem = false;
            }
            else {
                var params = {
                    selectedSystemName: obj.systemName(),
                };
                CallCommonServiceMethod('GetAllConfigDataInitial', params, OnSuccess_GetAllConfigData, OnFailure, null, false);
            }
        }
    }

    // TimeSeries Tab Click Handler
    PolarisDownstream.prototype.onClickReportTypeChain = function onClickReportTypeChain(obj, event) {
        if (obj.Type == undefined)
            return;
        if (obj.Type() != null || obj.Type() != undefined) polarisdownstream._pageViewModelInstance.selectedBlockType(obj.Type())
        $(event.currentTarget).addClass("TimeSeriesTabIsActive");
        if (obj.Type() == "Time Series")
            polarisdownstream._pageViewModelInstance.IsTS(true);
        else
            polarisdownstream._pageViewModelInstance.IsTS(false);

        //var g = $(".TimeSeriesTabIsActive").map(function () {
        //    return this.innerHTML;
        //}).get();
        let arr = $("#PolarisDownstreamTimeseriesContainer").find(".TimeSeriesTabIsActive");
        arr = arr.toArray();
        for (var item in arr) {
            $(arr[item]).removeClass('TimeSeriesTabIsActive')
        }
        $(event.currentTarget).addClass("TimeSeriesTabIsActive");
        if (obj.Type() != null || obj.Type() != undefined) polarisdownstream._pageViewModelInstance.SelectedReportType(obj.Type());
        obj.IsActive = true;
        ComputedIsActive = ko.computed(function () {
            return obj.IsActive ? '2px solid #9c9696' : '2px solid #dcdadb';
        });
        polarisdownstream._pageViewModelInstance.availableReports([]);
        for (let ob of obj.ReportsAvailable()) {
            for (let ele of obj.ReportsAvailable()) {
                if (ob == ele)
                    continue;
                if (ele.Name === ob.Name) {
                    ele.Name = ob.Module == "Ref" ? "RM " + ele.Name : "SM " + ele.Name;
                    console.log(ob);
                    break;
                }
            }
        }
        for (var item in obj.ReportsAvailable()) {
            polarisdownstream._pageViewModelInstance.availableReports.push(obj.ReportsAvailable()[item]);
        }
        console.log(polarisdownstream._pageViewModelInstance.availableReports());
        let availableReports = polarisdownstream._pageViewModelInstance.availableReports();
        var configuredReports = [];
        var data = [];
        for (var i = 0, len = availableReports.length; i < len; i++) {
            var tempObj = {};
            tempObj.text = availableReports[i].Name;
            tempObj.value = availableReports[i].Id;
            polarisdownstream._pageViewModelInstance.availableReports()[i].Name = availableReports[i].Name;
            polarisdownstream._pageViewModelInstance.availableReports()[i].text = polarisdownstream._pageViewModelInstance.availableReports()[i].Name;
            polarisdownstream._pageViewModelInstance.availableReports()[i].value = availableReports[i].Id;
            polarisdownstream._pageViewModelInstance.availableReports()[i].Id = availableReports[i].Id;
            data.push(tempObj);
            if (availableReports[i].IsReportConfigured == true) {
                configuredReports.push(availableReports[i]);
                obj.IsBlockTypeConfigured(true);
            }
        }
        ApplyMultiSelect('polarisDownstreamAvailableReportsMultiSelectedItems', false, polarisdownstream._pageViewModelInstance.availableReports(), true, '#PolarisDownstreamChainDropDownParentDiv', 'Reports', false, configuredReports, true);
        $('#PolarisDownstreamAddNewReportBtn').click();
    }

    // Add New System Button Click Handler
    $("#PolarisDownstremAddNewSystemBtn").unbind('click').bind('click', function (e, f) {
        polarisdownstream._isClone = false;
        var count = 0;
        if ($(".PolarisDownstreamNewSystemNameInput").val() == "") {
            $("#PolarisDownstreamNewSystemNameInputValidation").show();
            count++;
        } else {
            $("#PolarisDownstreamNewSystemNameInputValidation").hide();
        }
        if (smselect.getSelectedOption($("#smselect_polarisDownstreamAvailableCalendarsMultiSelectedItemsNew")).length == 0) {
            $("#PolarisDownstreamNewSystemCalendarInputValidation").show();
            count++;
        } else {
            $("#PolarisDownstreamNewSystemCalendarInputValidation").hide();
        }

        if ($("#newSystemEffectiveDate").val() == "") {
            $("#PolarisDownstreamNewSystemEffectiveDateInputValidation").show();
            count++;
        } else {
            $("#PolarisDownstreamNewSystemEffectiveDateInputValidation").hide();
        }

        if (smselect.getSelectedOption($("#smselect_polarisDownstreamSelectedConnection")).length == 0) {
            $("#PolarisDownstreamConnectionInputValidation").show();
            count++;
        } else {
            $("#PolarisDownstreamConnectionInputValidation").hide();
        }
        if ($("#PolarisDownstreamNewSystemPasswordInput").val() == "") {
            $("#PolarisDownstreamNewSystemPasswordInputValidation").show();
            count++;
        } else {
            $("#PolarisDownstreamNewSystemPasswordInputValidation").hide();
        }
        if (polarisdownstream._completeObj != null) {
            var systemNameExists = $.grep(polarisdownstream._completeObj, function (e) { return e.SystemName.toLowerCase().trim() == $(".PolarisDownstreamNewSystemNameInput").val().toLowerCase().trim() });
            if (systemNameExists.length > 0) {
                PolarisDownstream.prototype.showErrorDiv('Error', 'fail_icon.png', "System with same name already exists");
                count++;
            }

            var connectionInUse = $.grep(polarisdownstream._completeObj, function (e) { return e.SetupDetails.DbName.toLowerCase().trim() == polarisdownstream._selectedConnectionName.toLowerCase().trim() });
            if (connectionInUse.length > 0) {
                PolarisDownstream.prototype.showErrorDiv('Error', 'fail_icon.png', "Connection already in use.");
                count++;
            }
        }
        if (count == 0) {
            let data = {
                SystemName: $(".PolarisDownstreamNewSystemNameInput").val(),
                SetupDetails: {
                    //ServerName: $("#PolarisDownstreamNewSystemServerName").val(),
                    DbName: polarisdownstream._selectedConnectionName,
                    //UserName: $("#PolarisDownstreamNewSystemUserName").val(),
                    //Password: $("#PolarisDownstreamNewSystemPasswordInput").val(),
                },
                EffectiveDate: $("#newSystemEffectiveDate").val(),
                CalendarType: smselect.getSelectedOption($("#smselect_polarisDownstreamAvailableCalendarsMultiSelectedItemsNew"))[0].value
            }
            CallCommonServiceMethod('SRMDownstreamSyncAddNewSystem', data, OnSuccess_SRMDownstreamSyncAddNewSystem, OnFailure, null, false);
        } else {
            return false;
        }

    });

    // Add New System Icon Click Handler
    $('#PolarisDownstreamNewSystemIconContainer').unbind('click').bind('click', function () {
        let NewSystemIconContainer = $("#PolarisDownstreamNewSystemPanelContainer");
        $("#PolarisDownstreamSaveCloneButton").hide();
        $('#notificationsContainerDiv').hide();
        if (!NewSystemIconContainer.hasClass('Open')) {
            NewSystemIconContainer.addClass('Open');
            NewSystemIconContainer.show();
            $('#PolarisDownstreamNewSystemDetailsContainer').show();
        } else {
            NewSystemIconContainer.removeClass('Open');
            NewSystemIconContainer.hide();
            $('#PolarisDownstreamNewSystemDetailsContainer').hide();
        }
        var params = {};
        CallCommonServiceMethod('SRMDownstreamSyncGetExistingConnections', params, OnSuccess_SRMDownstreamSyncGetExistingConnections, OnFailure, null, false);
    });

    // Download views zip file 
    $('#PolarisDownstreamViewZipDownloadIcon').unbind('click').bind('click', function () {
        var params = {};
        params.systemName = polarisdownstream._completeObj[0].SystemName;
        $get('loadingImg').style.display = '';
        CallCommonServiceMethod('DownloadViewConfigurationForDWH', params, OnSuccess_PolarisDownstreamZipDownload, OnFailure, null, false);
    });

    function OnSuccess_PolarisDownstreamZipDownload(result) {

        $get('loadingImg').style.display = 'none';
        if (result.d.errorMessage != null) {
            PolarisDownstream.prototype.showErrorDiv('Error', 'fail_icon.png', result.d.errorMessage);
        }
        else {
            downloadFileInIframe(result.d.downloadLocation);
        }

    }
    function downloadFileInIframe(filePath) {
        $("#srmdwh-iframe").attr('src', path + "/SRMFileUpload.aspx?fileonserver=true&fullPath=" + filePath + "");
    }

    //Add Selected Dropdown items to selected reports chain
    $('#PolarisDownstreamAddNewReportBtn').unbind('click').bind('click', function () {

        var w = smselect.getSelectedOption($("#smselect_polarisDownstreamAvailableReportsMultiSelectedItems")).length;

        //Empty the Error Msg Div
        $("#PolarisDownstreamFilteringErrorDiv").text("");
        polarisdownstream._pageViewModelInstance.availableReports().map(function (x) {
            x.IsReportConfigured = false;
            return x;
        })
        var selctedSystemObj;
        if (polarisdownstream._isClone) {
            selctedSystemObj = polarisdownstream._completeObj[0];

        }
        else {
            selctedSystemObj = getCurrentSystemObject();
        }
        // if No Report Selected
        if (polarisdownstream._selectedReports.length == 0) {
            $("#PolarisDownstreamFilteringErrorDiv").text("No Report Selected.");
            polarisdownstream._pageViewModelInstance.ReportChainListing([]);
            var selectedBlocktype = selctedSystemObj.BlockType.find(obj => { return obj.BlockTypeName == polarisdownstream._pageViewModelInstance.selectedBlockType() })
            selectedBlocktype.IsBlockTypeConfigured = false;
            polarisdownstream._pageViewModelInstance.SelectedReportName("");
            $("#PolarisDownstreamSelectedReportDetailsMainContainer").hide();
            var blockType = polarisdownstream._pageViewModelInstance.reportTypes().find(obj => { return obj.Type() == selectedBlocktype.BlockTypeName })
            blockType.IsBlockTypeConfigured(false);
        }
        //if All Reports Selected
        else if (polarisdownstream._selectedReports.length > 0 && (polarisdownstream._selectedReports[0] == "-1")) {
            $("#PolarisDownstreamSelectedReportDetailsMainContainer").show();
            polarisdownstream._pageViewModelInstance.ReportChainListing([]);
            for (var item in polarisdownstream._pageViewModelInstance.availableReports()) {
                polarisdownstream._pageViewModelInstance.availableReports()[item].IsReportConfigured = true;
                polarisdownstream._pageViewModelInstance.ReportChainListing.push(new ReportChainViewModel(polarisdownstream._pageViewModelInstance.availableReports()[item]));
            }
            var selectedBlocktype = selctedSystemObj.BlockType.find(obj => { return obj.BlockTypeName == polarisdownstream._pageViewModelInstance.selectedBlockType() })
            selectedBlocktype.IsBlockTypeConfigured = true;
            var blockType = polarisdownstream._pageViewModelInstance.reportTypes().find(obj => { return obj.Type() == selectedBlocktype.BlockTypeName })
            blockType.IsBlockTypeConfigured(true);
            $($('.PolarisDownstreamChain')[0]).children('.PolarisDownstreamChainInfo').click();
        } else {
            console.log(polarisdownstream._selectedReports);
            if (polarisdownstream._selectedReports.length > 0) {
                $("#PolarisDownstreamSelectedReportDetailsMainContainer").show();
                polarisdownstream._pageViewModelInstance.ReportChainListing([]);
                for (var item in polarisdownstream._selectedReports) {
                    let selectedReportObject = polarisdownstream._pageViewModelInstance.availableReports().find(obj => {
                        if (polarisdownstream._selectedReports[item].Id != "" && polarisdownstream._selectedReports[item].Id != undefined)
                            return obj.Id === polarisdownstream._selectedReports[item].Id
                        else
                            return obj.text === polarisdownstream._selectedReports[item];
                    })

                    selectedReportObject.IsReportConfigured = true;
                    polarisdownstream._pageViewModelInstance.ReportChainListing.push(new ReportChainViewModel(selectedReportObject));
                    var selectedBlocktype = selctedSystemObj.BlockType.find(obj => { return obj.BlockTypeName == polarisdownstream._pageViewModelInstance.selectedBlockType() })
                    selectedBlocktype.IsBlockTypeConfigured = true;
                    var blockType = polarisdownstream._pageViewModelInstance.reportTypes().find(obj => { return obj.Type() == selectedBlocktype.BlockTypeName })
                    blockType.IsBlockTypeConfigured(true);
                }
            }
            else {
                var selectedBlocktype = selctedSystemObj.BlockType.find(obj => { return obj.BlockTypeName == polarisdownstream._pageViewModelInstance.selectedBlockType() })
                selectedBlocktype.IsBlockTypeConfigured = false;
            }
            for (i = 0; i < $('.PolarisDownstreamChain').length; i++) {
                $($('.PolarisDownstreamChain')[i]).children('.PolarisDownstreamChainInfo').click();
            }
            $($('.PolarisDownstreamChain')[0]).children('.PolarisDownstreamChainInfo').click();
        }
    });

    function setDefaultReportDetails() {

        if (polarisdownstream._pageViewModelInstance.SelectedQueueName() == "" || polarisdownstream._pageViewModelInstance.SelectedQueueName() == null) {
            var defaultValue = [];
            defaultValue.push(polarisdownstream._pageViewModelInstance.availableTransports()[0]);
            ApplyMultiSelect('polarisDownstreamAvailableTransportsMultiSelectedItems', false, polarisdownstream._pageViewModelInstance.availableTransports(), true, '#PolarisDownstreamSelectedReportQueueNotificationInputDiv', 'Queues', false, [], true);
        } else {
            var selectedItems = polarisdownstream._pageViewModelInstance.SelectedQueueName().split(",");
            ApplyMultiSelect('polarisDownstreamAvailableTransportsMultiSelectedItems', false, polarisdownstream._pageViewModelInstance.availableTransports(), true, '#PolarisDownstreamSelectedReportQueueNotificationInputDiv', 'Queues', false, selectedItems, true);
        }
        //
        ApplyMultiSelect('polarisDownstreamAvailableDateTypesEndItems', false, polarisdownstream._pageViewModelInstance.availableDateTypesEnd(), true, '#PolarisDownstreamSelectedReportEndDateLabelInputDiv', 'Date Types', false, polarisdownstream._pageViewModelInstance.SelectedEndDateValue(), false);
        if (polarisdownstream._pageViewModelInstance.SelectedEndDateValue() == "" || polarisdownstream._pageViewModelInstance.SelectedEndDateValue() == null) {
            smselect.setOptionByText($("#smselect_polarisDownstreamAvailableDateTypesEndItems"), "Now", false);
        }

        ApplyMultiSelect('polarisDownstreamAvailableDateTypesStartItems', false, polarisdownstream._pageViewModelInstance.availableDateTypes(), true, '#PolarisDownstreamSelectedReportStartDateInputDiv', 'Date Types', false, polarisdownstream._pageViewModelInstance.SelectedStartDateValue(), false);
        if (polarisdownstream._pageViewModelInstance.SelectedStartDateValue() == "" || polarisdownstream._pageViewModelInstance.SelectedStartDateValue() == null) {
            smselect.setOptionByText($("#smselect_polarisDownstreamAvailableDateTypesStartItems"), "LastExtractionDate", false);
        }
    }

    $('.calendarStart').click(function () {
        $("#InputCustomDateStart").datepicker("show");
    });
    $('#InputCustomDateStart').change(function () {
        var startDate = $(this).datepicker('getDate');
        var selectedStartDay = (startDate.getMonth() + 1) + "/" + startDate.getDate() + "/" + startDate.getFullYear();
        polarisdownstream._pageViewModelInstance.SelectedCustomStartDate(selectedStartDay);
        console.log(selectedStartDay);
    })
    $('.calendarEnd').click(function () {
        $("#InputCustomDateEnd").datepicker("show");
    });
    $('#InputCustomDateEnd').change(function () {
        var endDate = $(this).datepicker('getDate');
        var selectedEndDay = (endDate.getMonth() + 1) + "/" + endDate.getDate() + "/" + endDate.getFullYear();
        polarisdownstream._pageViewModelInstance.SelectedCustomEndDate(selectedEndDay);
        console.log(selectedEndDay);
    });

    //Numeric values only validation
    PolarisDownstream.prototype.isNumber = function isNumber(evt) {
        var iKeyCode = (evt.which) ? evt.which : evt.keyCode
        if (iKeyCode != 46 && iKeyCode > 31 && (iKeyCode < 48 || iKeyCode > 57))
            return false;
        return true;
    }
    PolarisDownstream.prototype.onClickSchedulerButton = function onClickSchedulerButton(obj, e) {
        e.stopPropagation();
        if ($("#scheduler").is(":visible")) {
            $("#scheduler").hide();
            $(".PolarisDownstreamMain").show();
        } else {
            $("#scheduler").show();
            $(".PolarisDownstreamMain").hide();
        }
    };

    PolarisDownstream.prototype.onClickCloneBtn = function onClickCloneBtn(obj, e) {
        e.stopPropagation();
        let NewSystemIconContainer = $("#PolarisDownstreamNewSystemPanelContainer");
        if (NewSystemIconContainer.hasClass('Open')) {
            NewSystemIconContainer.removeClass('Open');
            NewSystemIconContainer.hide();
            $('#PolarisDownstreamNewSystemDetailsContainer').hide();
        }
        var target = $(e.target);
        var NotificationsContainerDiv = $('#notificationsContainerDiv');
        var notificationsContainer = $('#notificationsContainer');
        var panelTop = parseInt(target.offset().top + 26);
        var panelLeft = parseInt(target.offset().left + target.outerWidth(true) + 7 - (NotificationsContainerDiv.outerWidth(true)) - 66);
        NotificationsContainerDiv.offset({
            'top': panelTop
        });
        if (NotificationsContainerDiv.css('display') == 'none') {
            NotificationsContainerDiv.show();
            $("#PolarisDownstreamSaveCloneButton").show();
        }
        else {
            NotificationsContainerDiv.hide();
            $("#PolarisDownstreamSaveCloneButton").hide();
        }
        // notificationsContainerDiv.show();
        var params = {};
        CallCommonServiceMethod('SRMDownstreamSyncGetExistingConnections', params, OnSuccess_SRMDownstreamSyncGetExistingConnections, OnFailure, null, false);
    };

    $("#PolarisDownstreamTriggerButton").unbind('click').bind('click', function (e, f) {

        var configuredReports = JSON.parse(JSON.stringify(polarisdownstream._completeObj));
        var Params = {
            SetupName: configuredReports[0].SystemName
        };
        CallCommonServiceMethod('SRMDownstreamSyncTriggerReports', Params, OnSuccess_SRMDownstreamSyncTriggerReports, OnFailure, null, false);
    });

    $("#PolarisDownstreamSaveButton").unbind('click').bind('click', function (e, f) {
        $get('loadingImg').style.display = '';
        var selectedSystemObject = $.grep(polarisdownstream._completeObj, function (e) { return e.SystemName == polarisdownstream._selectedSystemName });
        var configuredReports = JSON.parse(JSON.stringify(polarisdownstream._completeObj));
        var obj = polarisdownstream._completeObj;
        for (var item in obj[0].BlockType) {
            var s = obj[0].BlockType[item].ReportsAvailable.filter((report) => {
				if(report.IsReportConfigured && report.Details.BatchSize == '')
                    report.Details.BatchSize = -1;
                return report.IsReportConfigured;
            })
            configuredReports[0].BlockType[item].ReportsAvailable = s;
            console.log(s);
        }
        var configuredBlocks = $.grep(configuredReports[0].BlockType, function (e) { return e.IsBlockTypeConfigured });
        configuredReports[0].BlockType = configuredBlocks;
        var scheduleInfo = validateScheduler();
        if (scheduleInfo != null) {
            scheduleInfo.SetupName = configuredReports[0].SystemName;
            scheduleInfo.SetupDescription = configuredReports[0].SystemName;
        }
        var Params = {
            Systems: configuredReports[0],
            //IsUpdate: true,
            //IsClone: false,
            IsNewSystem: false,
            scheduleInfo: scheduleInfo,
            dateFormat: "MM/dd/yyyy"
        };
        if (polarisdownstream._RequiredValidationCount == 1 && scheduleInfo !== false) {

            CallCommonServiceMethod('SRMDownstreamSyncSaveReports', Params, OnSuccess_SRMDownstreamSyncSaveReports, OnFailure, null, false);
        } else {
            if (scheduleInfo === false) {
                if (!$("#scheduler").is(":visible")) {
                    $("#scheduler").show();
                    $(".PolarisDownstreamMain").hide();
                }
            }
        }
    });

    function OnSuccess_GetSelectedConnectionDetails(result) {
        if (typeof (result.d) != 'undefined') {
            var connectionDetails = result.d;
            let data = {
                SystemName: polarisdownstream._selectedSystemName,
                SetupDetails: {
                    ServerName: connectionDetails.ServerName,
                    DbName: connectionDetails.DbName,
                    RealDbName: connectionDetails.RealDbName,
                    UserName: connectionDetails.UserName,
                    Password: connectionDetails.Password,
                    EffectiveDate: polarisdownstream._pageViewModelInstance.selectedEffectiveDate(),
                    CalendarName: polarisdownstream._pageViewModelInstance.selectedCalendarName()
                }
            }
            $('#notificationsContainerDiv').hide();
            $("#PolarisDownstreamNewSystemPanelContainer").hide();
            polarisdownstream._pageViewModelInstance.SystemChainListing.push(new SystemChainViewModel(data));
            polarisdownstream._IsNewSystem = true;
            let len = ($('.SystemChain').length) - 1;
            $('.SystemChain')[len].click("djvfkfj");
            var configuredReports = JSON.parse(JSON.stringify(polarisdownstream._completeObj));
            configuredReports[0].SystemName = polarisdownstream._selectedSystemName;
            configuredReports[0].SetupDetails.DbName = polarisdownstream._pageViewModelInstance.selectedDbName();
            configuredReports[0].SetupDetails.ServerName = connectionDetails.ServerName;
            configuredReports[0].SetupDetails.UserName = connectionDetails.UserName;
            configuredReports[0].SetupDetails.Password = connectionDetails.Password;
            var obj = polarisdownstream._completeObj;
            for (var item in obj[0].BlockType) {
                var s = obj[0].BlockType[item].ReportsAvailable.filter((report) => {
                    return report.IsReportConfigured;
                })
                configuredReports[0].BlockType[item].ReportsAvailable = s;
                console.log(s);
            }
            var configuredBlocks = $.grep(configuredReports[0].BlockType, function (e) { return e.IsBlockTypeConfigured });
            var scheduleInfo = validateScheduler();
            if (scheduleInfo != null) {
                scheduleInfo.SetupName = configuredReports[0].SystemName;
                scheduleInfo.SetupDescription = configuredReports[0].SystemName;
            }
            configuredReports[0].BlockType = configuredBlocks;
            var Params = {
                Systems: configuredReports[0],
                //IsUpdate: true,
                //IsClone: true,
                IsNewSystem: true,
                scheduleInfo: scheduleInfo
            };
            if (polarisdownstream._RequiredValidationCount == 1 && scheduleInfo !== false) {

                CallCommonServiceMethod('SRMDownstreamSyncSaveReports', Params, OnSuccess_SRMDownstreamSyncSaveCloneReports, OnFailure, null, false);
            } else {
                if (scheduleInfo === false) {
                    if (!$("#scheduler").is(":visible")) {
                        $("#scheduler").show();
                        $(".PolarisDownstreamMain").hide();
                    }
                }
            }
        }
    }
    function OnSuccess_SRMDownstreamSyncSaveReports(result) {
        $get('loadingImg').style.display = 'none';
        if (result.d != "") {
            console.log(result.responseJSON);
            PolarisDownstream.prototype.showErrorDiv('Error', 'fail_icon.png', result.d);
        } else
            PolarisDownstream.prototype.showErrorDiv('Success', 'pass_icon.png', 'Reports saved successfully.');
    }
    function OnSuccess_SRMDownstreamSyncSaveCloneReports(result) {

        if (result.d != "") {
            console.log(result.responseJSON);
            PolarisDownstream.prototype.showErrorDiv('Error', 'fail_icon.png', result.d);
        } else {
            let len = ($('.SystemChain').length) - 1;
            $('.SystemChain')[len].click();
            PolarisDownstream.prototype.showErrorDiv('Success', 'pass_icon.png', 'System Cloned successfully.');
        }
    }

    function OnSuccess_SRMDownstreamSyncTriggerReports() {
        PolarisDownstream.prototype.showErrorDiv('Success', 'pass_icon.png', 'Reports Triggered successfully.');
    }

    function OnSuccess_SRMDownstreamSyncGetExistingConnections(result) {

        if (typeof (result.d) != 'undefined') {
            var arr = result.d;
            var data = [];
            for (key in arr) {
                var item = {
                    text: arr[key],
                    value: arr[key]
                }
                data.push(item);
            }
            ApplyMultiSelect('polarisDownstreamSelectedConnection', false, data, true, '#PolarisDownstreamSelectedConnectionInputDiv', 'Connections', false, "", false);
            ApplyMultiSelect('polarisDownstreamSelectedCloneConnection', false, data, true, '#PolarisDownstreamSelectedCloneConnectionInputDiv', 'Connections', false, "", false);
            CallCustomDatePicker($('[id*="newSystemEffectiveDate"]').prop('id').trim(), polarisdownstream._securityInfo.CustomJQueryDateFormat, function (d) {
                polarisdownstream._pageViewModelInstance.selectedEffectiveDate(d.format("MM/dd/yyyy"));
            }, optionDateTime.DATE, 15, true);
        }
    }

    function OnSuccess_SRMDownstreamSyncAddNewSystem(e) {
        // e.stopPropagation();

        PolarisDownstream.prototype.showErrorDiv('Success', 'pass_icon.png', 'New System added successfully.');
        let data = {
            SystemName: $(".PolarisDownstreamNewSystemNameInput").val(),
            SetupDetails: {
                ServerName: $("#PolarisDownstreamNewSystemServerName").val(),
                DbName: polarisdownstream._selectedConnectionName,
                UserName: $("#PolarisDownstreamNewSystemUserName").val(),
                Password: $("#PolarisDownstreamNewSystemPasswordInput").val(),
                EffectiveDate: $("#newSystemEffectiveDate").val(),
                CalendarName: smselect.getSelectedOption($("#smselect_polarisDownstreamAvailableCalendarsMultiSelectedItemsNew"))[0].value
            }
        }
        $('#PolarisDownstreamNewSystemDetailsContainer').hide();
        $("#PolarisDownstreamNewSystemPanelContainer").hide();
        polarisdownstream._pageViewModelInstance.SystemChainListing.push(new SystemChainViewModel(data));
        let len = ($('.SystemChain').length) - 1;

        for (var item in obj.reportTypes()) {
            polarisdownstream._pageViewModelInstance.reportTypes().map(function (x) {
                x.IsBlockTypeConfigured = false;
                return x;
            })
            polarisdownstream._pageViewModelInstance.reportTypes()[item].ReportsAvailable.map(function (x) {
                x.IsReportConfigured = false;
                return x;
            })
        }
        polarisdownstream._IsNewSystem = true;
        $('.SystemChain')[len].click();
    }
    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }
    function OnSuccess_GetBlockTypes(result) {

        // Render the data on the screen
        if (typeof (result.d) != 'undefined') {

            var array = result.d.split(',');
            for (i = 0; i < array.length; i++) {
                polarisdownstream._allTimeSeries.push({
                    Type: ko.observable(array[i]),
                    ReportsAvailable: ko.observableArray(),
                    IsBlockTypeConfigured: ko.observable(false),
                    IsActive: ko.observable(false)
                })
            }
            init();
        }
    }

    function OnSuccess_GetAllConfigDataInitial(result) {

        if (typeof (result.d) != 'undefined' && result.d.length > 0) {
            //bindMultiDropdown();
            ShowAllParentContainers();
            console.log(result.d);
            polarisdownstream._completeObj = result.d;
            polarisdownstream._pageViewModelInstance = new pageViewModel(result.d);
            ko.applyBindings(polarisdownstream._pageViewModelInstance);
            //Hide Add New System Details Container 
            $("#PolarisDownstreamNewSystemPanelContainer").hide();
            // First System tab is selected on load.
            if (polarisdownstream._pageViewModelInstance.selectedSystemChainItem != null) {
                polarisdownstream._pageViewModelInstance.selectedSystemChainItem.isSystemSelected(false);
            }
            polarisdownstream._pageViewModelInstance.selectedSystemChainItem = polarisdownstream._pageViewModelInstance.SystemChainListing()[0];
            polarisdownstream._pageViewModelInstance.selectedSystemChainItem.isSystemSelected(true);
            polarisdownstream._pageViewModelInstance.selectedServerName(polarisdownstream._pageViewModelInstance.SystemChainListing()[0].serverName());
            polarisdownstream._pageViewModelInstance.selectedDbName(polarisdownstream._pageViewModelInstance.SystemChainListing()[0].dbName());
            polarisdownstream._pageViewModelInstance.selectedRealDbName(polarisdownstream._pageViewModelInstance.SystemChainListing()[0].realDbName());
            polarisdownstream._pageViewModelInstance.selectedUserName(polarisdownstream._pageViewModelInstance.SystemChainListing()[0].userName());
            polarisdownstream._pageViewModelInstance.selectedPassword(polarisdownstream._pageViewModelInstance.SystemChainListing()[0].password());
            polarisdownstream._pageViewModelInstance.selectedCalendarName(polarisdownstream._pageViewModelInstance.SystemChainListing()[0].calendarName());
            polarisdownstream._pageViewModelInstance.selectedEffectiveDate(polarisdownstream._pageViewModelInstance.SystemChainListing()[0].effectiveDate());
            //
            if (polarisdownstream._pageViewModelInstance.selectedCalendarName() == null || polarisdownstream._pageViewModelInstance.selectedCalendarName() == undefined)
                polarisdownstream._pageViewModelInstance.selectedCalendarName(smselect.getSelectedOption($("#smselect_polarisDownstreamAvailableCalendarsMultiSelectedItems"))[0].text);
            else {
                ApplyMultiSelect('polarisDownstreamAvailableCalendarsMultiSelectedItems', false, polarisdownstream._pageViewModelInstance.availableCalendars(), true, '#SelectedSetupCalender', 'Calendars', false, polarisdownstream._pageViewModelInstance.selectedCalendarName(), false);
            }
            //polarisdownstream._pageViewModelInstance.reportTypes(obj.ReportTypeChainListing);
            polarisdownstream._pageViewModelInstance.reportTypes([]);
            //
            var arr = polarisdownstream._pageViewModelInstance.SystemChainListing()[0].ReportTypeChainListing();
            for (var item in arr) {
                polarisdownstream._pageViewModelInstance.reportTypes.push(new ReportTypeChainViewModel(arr[item]));
            }

            $('.TimeSeriesTab').bind('click', { param1: polarisdownstream._pageViewModelInstance.reportTypes[0] }, polarisdownstream.onClickReportTypeChain);
            $('.TimeSeriesTab')[0].click();
            var params = { selectedSystemName: result.d[0].SystemName };
            CallCommonServiceMethod('GetSchedulerData', params, OnSuccess_GetSchedulerData, OnFailure, null, false);
        }
        else {
            ShowOnlyAddNewSystemButton();
        }
    }
    function ShowOnlyAddNewSystemButton(result) {
        $("[id$=panelMiddle]").hide();
        $("#PolarisDownstreamNewSystemPanelContainer").hide();
        $("#PolarisDownstreamTopContainer2").hide();
        $("#PolarisDownstreamTopContainer3").hide();

    }
    function ShowAllParentContainers(result) {
        $("[id$=panelMiddle]").show();
        $("#PolarisDownstreamNewSystemPanelContainer").show();
        $("#PolarisDownstreamTopContainer2").show();
        $("#PolarisDownstreamTopContainer3").show();
    }
    function OnSuccess_GetAllConfigData(result) {

        if (typeof (result.d) != 'undefined' && result.d.length > 0) {
            ShowAllParentContainers();
            console.log(result.d);
            polarisdownstream._completeObj = result.d;
            polarisdownstream._selectedSystemIndex = polarisdownstream._pageViewModelInstance.SystemChainListing().findIndex((obj => obj.systemName() == polarisdownstream._completeObj[0].SystemName));
            polarisdownstream._pageViewModelInstance.SystemChainListing()[polarisdownstream._selectedSystemIndex] = new SystemChainViewModel(polarisdownstream._completeObj[0]);

            //Hide Add New System Details Container 
            $("#PolarisDownstreamNewSystemPanelContainer").hide();
            if (polarisdownstream._pageViewModelInstance.selectedSystemChainItem != null) {
                polarisdownstream._pageViewModelInstance.selectedSystemChainItem.isSystemSelected(false);
            }
            polarisdownstream._pageViewModelInstance.selectedSystemChainItem = polarisdownstream._pageViewModelInstance.SystemChainListing()[polarisdownstream._selectedSystemIndex];
            polarisdownstream._pageViewModelInstance.selectedSystemChainItem.isSystemSelected(true);
            polarisdownstream._pageViewModelInstance.selectedServerName(polarisdownstream._pageViewModelInstance.SystemChainListing()[polarisdownstream._selectedSystemIndex].serverName());
            polarisdownstream._pageViewModelInstance.selectedDbName(polarisdownstream._pageViewModelInstance.SystemChainListing()[polarisdownstream._selectedSystemIndex].dbName());
            polarisdownstream._pageViewModelInstance.selectedRealDbName(polarisdownstream._pageViewModelInstance.SystemChainListing()[polarisdownstream._selectedSystemIndex].realDbName());
            polarisdownstream._pageViewModelInstance.selectedUserName(polarisdownstream._pageViewModelInstance.SystemChainListing()[polarisdownstream._selectedSystemIndex].userName());
            polarisdownstream._pageViewModelInstance.selectedPassword(polarisdownstream._pageViewModelInstance.SystemChainListing()[polarisdownstream._selectedSystemIndex].password());
            //polarisdownstream._pageViewModelInstance.selectedCalendarName(polarisdownstream._pageViewModelInstance.SystemChainListing()[polarisdownstream._selectedSystemIndex].calendarName());
            polarisdownstream._pageViewModelInstance.selectedEffectiveDate(polarisdownstream._pageViewModelInstance.SystemChainListing()[polarisdownstream._selectedSystemIndex].effectiveDate());

            if (polarisdownstream._pageViewModelInstance.selectedCalendarName == null || polarisdownstream._pageViewModelInstance.selectedCalendarName == undefined)
                polarisdownstream._pageViewModelInstance.selectedCalendarName(smselect.getSelectedOption($("#smselect_polarisDownstreamAvailableCalendarsMultiSelectedItems"))[0].text);
            else {
                polarisdownstream._pageViewModelInstance.selectedCalendarName(polarisdownstream._pageViewModelInstance.SystemChainListing()[polarisdownstream._selectedSystemIndex].calendarName());
                ApplyMultiSelect('polarisDownstreamAvailableCalendarsMultiSelectedItems', false, polarisdownstream._pageViewModelInstance.availableCalendars(), true, '#SelectedSetupCalender', 'Calendars', false, polarisdownstream._pageViewModelInstance.selectedCalendarName(), false);
            }
            //polarisdownstream._pageViewModelInstance.reportTypes(obj.ReportTypeChainListing);
            polarisdownstream._pageViewModelInstance.reportTypes([]);
            //

            var arr = polarisdownstream._pageViewModelInstance.SystemChainListing()[polarisdownstream._selectedSystemIndex].ReportTypeChainListing();
            for (var item in arr) {
                polarisdownstream._pageViewModelInstance.reportTypes.push(new ReportTypeChainViewModel(arr[item]));
            }
            $('.TimeSeriesTab').bind('click', { param1: polarisdownstream._pageViewModelInstance.reportTypes[0] }, polarisdownstream.onClickReportTypeChain);
            $('.TimeSeriesTab')[0].click();
            var params = { selectedSystemName: result.d[0].SystemName };
            CallCommonServiceMethod('GetSchedulerData', params, OnSuccess_GetSchedulerData, OnFailure, null, false);
        }
        else {
            ShowOnlyAddNewSystemButton();
        }
    }
    function OnSuccess_GetSchedulerData(result) {
        if (result.d != null) {
            var data = result.d;
            $('#startDateTxt').val(data.StartDate);
            $('#startTimeTxt').val(data.StartTime);
            if (data.RecurrenceType == 'Recurring') {
                enableRecurring();
                $('#intervalTxt').val(data.Interval);
                $('#intervalRecurrenceTxt').val(data.TimeInterval);
                $('#numberOfRecurrenceTxt').val(data.NumberOfRecurrence);
                if (data.NoEndDate == true) {
                    $('#neverEndJobCheckbox').prop('checked', true);
                } else {
                    $('#neverEndJobCheckbox').prop('checked', false);
                    $('#endDateTxt').val(data.EndDate);
                }
                $('.dayOfWeekRow').slideUp();
                $('input:radio[name=recurrencePatternRadioGroup]').prop('checked', false);
                if (data.RecurrencePattern == 'daily')
                    $('#dailyRecurrenceRadio').prop('checked', true);
                else if (data.RecurrencePattern == 'weekly') {
                    $('#weeklyRecurrenceRadio').prop('checked', true);
                    $('.dayOfWeekRow').show();
                    $('input[name=dayOfWeekCheckboxGroup]').prop('checked', false);
                    var days = data.DaysOfWeek.split(",");
                    days.forEach((value) => {
                        $('input[name=dayOfWeekCheckboxGroup][value=' + value + ']').prop('checked', true);
                    });
                }
                else if (data.RecurrencePattern == 'monthly')
                    $('#monthlyRecurrenceRadio').prop('checked', true);
            }
            else {
                if (data.StartDate.trim() != '' && data.StartTime.trim() != '')
                    enableNonRecurring();
                else
                    disableScheduler();
            }
        } else {
            disableScheduler();
        }
    }

    function OnFailure(result) {
        console.log("Service returned an error" + ((result.responseJSON == null || result.responseJSON.Message == null || result.responseJSON.Message.length == 0) ? "" : (" : " + result.responseJSON.Message)) + ((result.responseText == null || result.responseText.length == 0) ? "" : (" : " + result.responseText)));
        $get('loadingImg').style.display = 'none';
        PolarisDownstream.prototype.showErrorDiv('Error', 'fail_icon.png', "Service returned an error" + ((result.responseJSON == null || result.responseJSON.Message == null || result.responseJSON.Message.length == 0) ? "" : (" : " + result.responseJSON.Message)) + ((result.responseText == null || result.responseText.length == 0) ? "" : (" : " + result.responseText)));

    }
    function init() {
        if (typeof ko !== 'undefined') {
            var params = {};
            CallCommonServiceMethod('GetAllConfigDataInitial', params, OnSuccess_GetAllConfigDataInitial, OnFailure, null, false);

        }
    }
    PolarisDownstream.prototype.showErrorDiv = function (errorHeading, srcImageName, errorMessageText) {
        var path = window.location.protocol + '//' + window.location.host;
        var pathname = window.location.pathname.split('/');

        $.each(pathname, function (ii, ee) {
            if ((ii !== 0) && (ii !== pathname.length - 1))
                path = path + '/' + ee;
        });

        path += '/images/icons/';

        $("#downstreamSyncError_ImageURL").attr('src', path + srcImageName);
        $(".downstreamSyncError_popupTitle").html(errorHeading);
        $(".downstreamSyncError_popupMessage").html(errorMessageText);
        if (errorHeading.toLowerCase().trim() == "error") {
            $('.downstreamSyncError_popupContainer').css('border-top', '4px solid rgb(199, 137, 140)');
            $('.downstreamSyncError_popupTitle').css('color', '4px solid #8a8787');
            $('.downstreamSyncError_popupMessageContainer').css('margin-left', '25px');
            $('#downstreamSyncErrorDiv_messagePopUp').show();
            $('#downstreamSyncDisableDiv').show();
            return false;
        }
        else if (errorHeading.toLowerCase().trim() == "success") {
            $('.downstreamSyncError_popupContainer').css('border-top', '4px solid rgb(172, 211, 115)');
            $('.downstreamSyncError_popupTitle').css('color', '4px solid rgb(172, 211, 115)');
            $('.downstreamSyncError_popupMessageContainer').css('margin-left', '72px');
            $('#downstreamSyncErrorDiv_messagePopUp').show();
            $('#downstreamSyncDisableDiv').show();
            return true;
        }
    }
    ////////////////////////////
    // "Initializer" function //
    ////////////////////////////
    PolarisDownstream.prototype.Initializer = function Initializer(securityInfo, availableTransports, availableCalendars, subscriberList) {

        polarisdownstream._securityInfo = eval("(" + securityInfo + ")");
        polarisdownstream._subscriberList = JSON.parse(subscriberList);
        polarisdownstream._availableTransports = JSON.parse(availableTransports);
        polarisdownstream._availableCalendars = JSON.parse(availableCalendars);
        polarisdownstream._availableDateTypes = {
            None: 0,
            //Today: 1,
            //Yesterday: 2,
            //LastBusinessDay: 3,
            //T_Minus_N: 4,
            //CustomDate: 5,
            //Now: 6,
            //FirstBusinessDayOfMonth: 7,
            //FirstBusinessDayOfYear: 8,
            //LastBusinessDayOfMonth: 9,
            //LastBusinessDayOfYear: 10,
            //LastBusinessDayOfPreviousMonth_Plus_N: 11,
            //LastBusinessDayOfPreviousYear_Plus_N: 12,
            //FirstBusinessDayOfMonth_Plus_N: 13,
            //FirstBusinessDayOfYear_Plus_N: 14,
            LastExtractionDate: 100
        };
        polarisdownstream._availableDateTypesEnd = {
            // None: 0,
            //Today: 1,
            //Yesterday: 2,
            //LastBusinessDay: 3,
            //T_Minus_N: 4,
            //CustomDate: 5,
            Now: 6,
            //FirstBusinessDayOfMonth: 7,
            //FirstBusinessDayOfYear: 8,
            //LastBusinessDayOfMonth: 9,
            //LastBusinessDayOfYear: 10,
            //LastBusinessDayOfPreviousMonth_Plus_N: 11,
            //LastBusinessDayOfPreviousYear_Plus_N: 12,
            //FirstBusinessDayOfMonth_Plus_N: 13,
            //FirstBusinessDayOfYear_Plus_N: 14,
            // LastExtractionDate: 100
        };

        var availableCalendars = [];
        for (key in polarisdownstream._availableCalendars) {
            var item = {
                text: polarisdownstream._availableCalendars[key],
                value: key
            }
            availableCalendars.push(item);
        }
        ApplyMultiSelect('polarisDownstreamAvailableCalendarsMultiSelectedItemsNew', false, availableCalendars, true, '#newSystemCalendarName', 'Calendars', false, availableCalendars[0].text, false);

        convertCheckboxToToggle(PolarisDownstreamSelectedRequireKnowledgeDateReportingChk.id, true);
        convertCheckboxToToggle(PolarisDownstreamSelectedReportRequireDeletedRowChk.id, true);
        convertCheckboxToToggle(PolarisDownstreamSelectedReportRemoveTimePartChk.id, true);
        convertCheckboxToToggleKDTD(PolarisDownstreamSelectedReportRefKnowledgeDateChk.id, true);
        convertCheckboxToToggleSDED(PolarisDownstreamSelectedReportEffectiveDateChk.id, true);
        var params = {};
        CallCommonServiceMethod('GetBlockTypes', params, OnSuccess_GetBlockTypes, OnFailure, null, false);

        $(document).ready(function () {
            CallCustomDatePicker('startDateTxt', 'm/d/Y', null, optionDateTime.DATE, 15, false);
            CallCustomDatePicker('endDateTxt', 'm/d/Y', null, optionDateTime.DATE, 15, false);
            CallCustomDatePicker("newSystemEffectiveDate", 'm/d/Y', function (d) { console.log("newSystemEffectiveDate " + d); }, optionDateTime.DATE, 15, true);
            initScheduler();
        });

    }
    function bindMultiDropdown() {
        //
        var subscribeListArray = Array();
        taskWaitSubscribers = [];
        var subscribtionInfo = polarisdownstream._subscriberList;
        $.each(subscribtionInfo, function (key) {
            if (typeof (subscribtionInfo[key]) !== 'undefined') {
                subscribeListArray = subscribeListArray.concat(subscribtionInfo[key]);
            }
        });
        if (typeof subscribeListArray !== 'undefined' && subscribeListArray.length > 0) {
            var items = [];
            function onlyUnique(value, index, self) {
                return self.indexOf(value) === index;
            }
            subscribeListArray = subscribeListArray.filter(onlyUnique);
            for (var i = 0; i < subscribeListArray.length; i++) {
                text = subscribeListArray[i];
                val = subscribeListArray[i];
                items.push({ text: text, value: val });
            }
            taskWaitSubscribers = items;
            smselect.create(
                {
                    id: "inprogressSubscribers",
                    text: "users",
                    container: $('#inprogressSubscribersDiv'),
                    isMultiSelect: true,
                    isRunningText: false,
                    data: [{ options: items, text: '' }],
                    showSearch: true,
                    ready: function (selectelement) {
                        selectelement.css('border', '1px solid rgb(175, 175, 175)');
                        selectelement.css('height', '19px');
                        selectelement.css('width', '140px');
                    }
                });

            //var subscribeListArray = {};
            subscribeListArray = polarisdownstream._subscriberList; //msg.d.split(',');

            if (!isEmpty(subscribeListArray)) {
                $('#dialogMsg').empty();
                $('#dialogMsg').append('<span class="SMTaskGroupMailBox_HeaderContainer" style="text-align: center; margin-bottom: 5%; display : none;"><span style=" width: 44%;display: inline-block;border-radius: 2px; border: 1px solid rgb(72, 163, 221);"><span id="successRadio_span" style="display: inline-block; cursor: pointer; height: 20px; width: 49%; font-family: Lato; font-size: 14px; border-bottom: 2px solid rgb(72, 163, 221); margin-right: 3%; background: rgb(72, 163, 221); color: #fff;"><label style="cursor: pointer"><input type="radio" name="triggerTypeRadioGroup" id="successRadio" value="Success" checked="checked" style="display: none;">Success</label></span><span id="failureRadio_span" style="display: inline-block; height: 20px; border-bottom: 2px solid rgb(255, 255, 255); cursor: pointer; width: 48%; font-family: Lato; font-size: 14px; color: rgb(125, 125, 125);"><label style="cursor: pointer"><input id="failureRadio" type="radio" name="triggerTypeRadioGroup" value="Failure" style="display: none;">Failure</label></span></span></span><span id="SRM_SuccessMailBoxContainer"><span class="subscriptionListContainer"><span class="bodysubscriptionListContainer"><span style="margin-bottom: 2%;"><span style="  width: 10%; display:  inline-block; height: 2em; bottom: 2px; position: relative; display:none;">  To  </span><span id="successSubscribtionListContainer" style=" width: 99% !important; margin-left: 1%; min-height: 2.5em; max-height: 5.8em;border-bottom: none !important;"></span></span><span style="margin-bottom: 1%; display : none;"><span style=" width: 10%; display: inline-block; height: 22px">  Subject  </span><input style="display:none; font-size:11px;outline: none;outline-style: none;border-top: none;border-left: none;border-right: none;border-bottom: solid #b3b3b3 1px;width: 89%;margin-left: 1%; height: 16px;" id="successMailSubject"></span><br><textarea style="display : none; Sfont-size:11px; margin-top: 10px; margin-bottom: 4px;" id="successMailBody" rows="8" cols="75"/></span></span></span><span id="SRM_FailureMailBoxContainer" style="display:none;"><span class="subscriptionListContainer"><span class="bodysubscriptionListContainer"><span style="margin-bottom: 2%;"><span style=" width: 10%; display:  inline-block; height: 2em; bottom: 2px; position: relative;">  To  </span><span id="failureSubscribtionListContainer" style="border-bottom: solid #b3b3b3 1px; width: 89% !important; margin-left: 1%; min-height: 2.5em; max-height: 5.8em; padding-bottom: 1px;"></span></span><span style="margin-bottom: 1%;"><span style=" width: 10%; display: inline-block; height: 22px">  Subject  </span><input style="font-size:11px;outline: none;outline-style: none;border-top: none;border-left: none;border-right: none;border-bottom: solid #b3b3b3 1px;width: 89%;margin-left: 1%; height: 16px;" id="failureMailSubject"></span><br><textarea style="font-size:11px; margin-top: 10px; margin-bottom: 4px;" id="failureMailBody" rows="8" cols="75"/></span></span></span>');

                var obj = {};
                obj.container = $('#successSubscribtionListContainer');
                obj.mailsCollection = subscribeListArray;
                if (polarisdownstream._pageViewModelInstance.SelectedFailureEmail() != null && polarisdownstream._pageViewModelInstance.SelectedFailureEmail().split(",").length > 0) {
                    obj.list = polarisdownstream._pageViewModelInstance.SelectedFailureEmail().split(",");
                } else
                    obj.list = [];
                mailtag.create(obj);


                obj.container = $('#failureSubscribtionListContainer');
                obj.mailsCollection = subscribeListArray;
                if (polarisdownstream._pageViewModelInstance.SelectedFailureEmail() != null && polarisdownstream._pageViewModelInstance.SelectedFailureEmail().split(",").length > 0) {
                    obj.list = polarisdownstream._pageViewModelInstance.SelectedFailureEmail().split(",");
                } else
                    obj.list = [];
                mailtag.create(obj);
            }
        }
    }
    $('#successSubscribtionListContainer').click(function (e) {
        //
        polarisdownstream._pageViewModelInstance.SelectedFailureEmail($('#successSubscribtionListContainer .hdnTag').val());
    });
    function isEmpty(obj) {
        for (var key in obj) {
            if (obj.hasOwnProperty(key))
                return false;
        }
        return true;
    }
    function initScheduler() {
        $("#recurringRadio").click(enableRecurring);
        $("#nonRecurringRadio").click(enableNonRecurring);
        $("#removeSchedulingButton").click(disableScheduler);
    }
    convertCheckboxToToggle = function (checkboxID, allowUpdate) {
        var id = "sm_toggle_" + checkboxID;

        if ($("body").find("#" + id).length != 0) {
            $("body").find("#" + id).remove();
        }
        var checkBox = $("#" + checkboxID);
        checkBox[0].style.display = "none";
        $('#hdnCheckBoxID').val('');
        var flag;
        if (id == "sm_toggle_PolarisDownstreamSelectedReportRequireDeletedRowChk" && polarisdownstream._pageViewModelInstance != null) {
            flag = polarisdownstream._pageViewModelInstance.SelectedRequireDeletedAssetTypes();
        }
        if (id == "sm_toggle_PolarisDownstreamSelectedReportRemoveTimePartChk" && polarisdownstream._pageViewModelInstance != null) {
            flag = polarisdownstream._pageViewModelInstance.SelectedRequireTimeInTSReport();
        }
        if (id == "sm_toggle_PolarisDownstreamSelectedRequireKnowledgeDateReportingChk" && polarisdownstream._pageViewModelInstance != null) {
            flag = polarisdownstream._pageViewModelInstance.SelectedRequireKnowledgeDateReporting();
        }


        //By Default NO is Selected

        var HTML = "<div id='" + id + "' class='sm_toggleContainer' style=\" height: 18px;border-radius: 1px;background-color: rgba(0, 0, 0, 0);display: inline-block;margin-left: 1%;padding: 0px 5px; float:right;\">";
        HTML += "<div class='sm_toggleText'  style=\"background-color: #f9f3f5;font-size: 10px; color: #888;width: 24px; padding-left: 6px; display: inline-block;height: 19px; line-height: 19px; border: 1px solid #4da7dc;\">YES</div>";
        HTML += "<div class='sm_toggleText' style=\"background-color: #f9f3f5;font-size: 10px; color: #888;width: 24px; padding-left: 6px; display: inline-block;height: 19px; line-height: 19px;border: 1px solid #4da7dc;\">NO</div>";
        if (!flag) {
            HTML += "<div class='sm_toggleBtn'  style=\"width: 24px;height: 19px;border-radius: 1px;background-color: #4da7dc;cursor: pointer;color: #fff;padding-left: 6px;position: relative;font-size: 10px;top: -20px;line-height: 19px;margin-left:33px;\">NO</div>";
        }
        else {
            HTML += "<div class='sm_toggleBtn' style=\"width: 25px;height:19 px;border-radius: 1px;background-color: #4da7dc;cursor: pointer;color: #fff;padding-left: 6px;position: relative;font-size: 10px;top: -20px;line-height: 19px;\">YES</div>";
        }
        HTML += "</div>";

        checkBox.after(HTML);

        if (allowUpdate) {
            checkBox.next().unbind('click').bind('click', function (e) {
                if (!checkBox[0].disabled) {
                    // handling for mandatory checkbox
                    var manFlag = true;
                    var showFlag = true;
                    var flag;
                    if (id == "sm_toggle_PolarisDownstreamSelectedReportRequireDeletedRowChk" && polarisdownstream._pageViewModelInstance != null) {
                        flag = polarisdownstream._pageViewModelInstance.SelectedRequireDeletedAssetTypes();
                    }
                    if (id == "sm_toggle_PolarisDownstreamSelectedReportRemoveTimePartChk" && polarisdownstream._pageViewModelInstance != null) {
                        flag = polarisdownstream._pageViewModelInstance.SelectedRequireTimeInTSReport();
                    }
                    if (id == "sm_toggle_PolarisDownstreamSelectedRequireKnowledgeDateReportingChk" && polarisdownstream._pageViewModelInstance != null) {
                        flag = polarisdownstream._pageViewModelInstance.SelectedRequireKnowledgeDateReporting();
                    }
                    var target = $(e.target);
                    if (target.hasClass("sm_toggleText")) {
                        target = target.parent();
                    }
                    if (target.hasClass("sm_toggleBtn")) {
                        target = target.parent();
                    }
                    target = target.find(".sm_toggleBtn");

                    if (flag) {
                        target.animate({
                            "margin-left": "33px"
                        }, function () {
                            $("#" + id).find(".sm_toggleBtn").text("NO");
                            if (id == "sm_toggle_PolarisDownstreamSelectedReportRequireDeletedRowChk") {
                                polarisdownstream._pageViewModelInstance.SelectedRequireDeletedAssetTypes(false);
                            }
                            if (id == "sm_toggle_PolarisDownstreamSelectedReportRemoveTimePartChk") {
                                polarisdownstream._pageViewModelInstance.SelectedRequireTimeInTSReport(false);
                            }
                            if (id == "sm_toggle_PolarisDownstreamSelectedRequireKnowledgeDateReportingChk") {
                                polarisdownstream._pageViewModelInstance.SelectedRequireKnowledgeDateReporting(false);
                            }
                            //checkBox.click();
                        });
                    }
                    else {
                        target.animate({
                            "margin-left": "0px"
                        }, function () {
                            $("#" + id).find(".sm_toggleBtn").text("YES");
                            if (id == "sm_toggle_PolarisDownstreamSelectedReportRequireDeletedRowChk") {
                                polarisdownstream._pageViewModelInstance.SelectedRequireDeletedAssetTypes(true);
                            }
                            if (id == "sm_toggle_PolarisDownstreamSelectedReportRemoveTimePartChk") {
                                polarisdownstream._pageViewModelInstance.SelectedRequireTimeInTSReport(true);
                            }
                            if (id == "sm_toggle_PolarisDownstreamSelectedRequireKnowledgeDateReportingChk") {
                                polarisdownstream._pageViewModelInstance.SelectedRequireKnowledgeDateReporting(true);
                            }
                            //
                            // checkBox.click();
                        });
                    }
                    e.stopPropagation();
                }
            });
        }
        else {
            checkBox.next().off('click');

        }
    }

    convertCheckboxToToggleSDED = function (checkboxID, allowUpdate) {
        var id = "sm_toggle_" + checkboxID;

        if ($("body").find("#" + id).length != 0) {
            $("body").find("#" + id).remove();
        }
        var checkBox = $("#" + checkboxID);
        checkBox[0].style.display = "none";
        $('#hdnCheckBoxID').val('');
        var flag;
        if (polarisdownstream._pageViewModelInstance != null)
            flag = polarisdownstream._pageViewModelInstance.SelectedRequireLookupMassagingStartDate();

        //By Default NO is Selected
        var HTML = "<div id='" + id + "' class='sm_toggleContainer' style=\" height: 18px;border-radius: 1px;background-color: rgba(0, 0, 0, 0);display: inline-block;margin-left: 1%;padding: 0px 5px; float:right;\">";
        HTML += "<div class='sm_toggleText'  style=\"background-color: #f9f3f5;font-size: 10px; color: #888;width: 57px; padding-left: 6px; display: inline-block;height: 21px; line-height: 21px; border: 1px solid #4da7dc;\">Start Date</div>";
        HTML += "<div class='sm_toggleText' style=\"background-color: #f9f3f5;font-size: 10px; color: #888;width: 54px; padding-left: 6px; display: inline-block;height: 21px; line-height: 21px;border: 1px solid #4da7dc;\">End Date</div>";
        if (!flag) {
            HTML += "<div class='sm_toggleBtn'  style=\"width: 54px;height: 22px;border-radius: 1px;background-color: #4da7dc;cursor: pointer;color: #fff;padding-left: 6px;position: relative;font-size: 10px;top: -23px;line-height: 21px;margin-left:66px;\">End Date</div>";
        }
        else {
            HTML += "<div class='sm_toggleBtn' style=\"width: 58px;height: 22px;border-radius: 1px;background-color: #4da7dc;cursor: pointer;color: #fff;padding-left: 6px;position: relative;font-size: 10px;top: -23px;line-height: 21px;\">Start Date</div>";
        }
        HTML += "</div>";

        checkBox.after(HTML);

        if (allowUpdate) {
            checkBox.next().unbind('click').bind('click', function (e) {
                if (!checkBox[0].disabled) {

                    // handling for mandatory checkbox
                    var manFlag = true;
                    var showFlag = true;
                    var flag;
                    if (polarisdownstream._pageViewModelInstance != null)
                        flag = polarisdownstream._pageViewModelInstance.SelectedRequireLookupMassagingStartDate();

                    var target = $(e.target);
                    if (target.hasClass("sm_toggleText")) {
                        target = target.parent();
                    }
                    if (target.hasClass("sm_toggleBtn")) {
                        target = target.parent();
                    }
                    target = target.find(".sm_toggleBtn");

                    if (flag) {
                        target.animate({
                            "margin-left": "66px",
                            width: "54px"
                        }, function () {
                            $("#" + id).find(".sm_toggleBtn").text("End Date");
                            polarisdownstream._pageViewModelInstance.SelectedRequireLookupMassagingStartDate(false);
                            // checkBox.click();
                        });
                    }
                    else {
                        target.animate({
                            "margin-left": "0px",
                            width: "58px"
                        }, function () {
                            $("#" + id).find(".sm_toggleBtn").text("Start Date");
                            polarisdownstream._pageViewModelInstance.SelectedRequireLookupMassagingStartDate(true);
                            // checkBox.click();
                        });
                    }
                    e.stopPropagation();
                }
            });
        }
        else {
            checkBox.next().off('click');

        }

    }
    convertCheckboxToToggleKDTD = function (checkboxID, allowUpdate) {
        var id = "sm_toggle_" + checkboxID;

        if ($("body").find("#" + id).length != 0) {
            $("body").find("#" + id).remove();
        }
        var checkBox = $("#" + checkboxID);
        checkBox[0].style.display = "none";
        $('#hdnCheckBoxID').val('');
        var flag;
        if (polarisdownstream._pageViewModelInstance != null)
            flag = polarisdownstream._pageViewModelInstance.SelectedRequireLookupMassagingCurrentKnowledgeDate();
        //By Default NO is Selected
        var HTML = "<div id='" + id + "' class='sm_toggleContainer' style=\" height: 18px;border-radius: 1px;background-color: rgba(0, 0, 0, 0);display: inline-block;margin-left: 1%;padding: 0px 5px; float:right;\">";
        HTML += "<div class='sm_toggleText'  style=\"background-color: #f9f3f5;font-size: 10px;color: #888;width: 76px;padding-left: 6px;display: inline-block;height: 21px;line-height: 21px;border: 1px solid #4da7dc;\">Knowledge Date</div>";
        HTML += "<div class='sm_toggleText' style=\"background-color: #f9f3f5;font-size: 10px; color: #888;width: 35px; padding-left: 6px; display: inline-block;height: 21px; line-height: 21px;border: 1px solid #4da7dc;\">Today</div>";
        if (!flag) {
            HTML += "<div class='sm_toggleBtn'  style=\"width: 35px;height: 22px;border-radius: 1px;background-color: rgb(77, 167, 220);cursor: pointer;color: rgb(255, 255, 255);padding-left: 6px;position: relative;font-size: 10px;top: -23px;line-height: 21px;margin-left: 85px;\">Today</div>";
        }
        else {
            HTML += "<div class='sm_toggleBtn' style=\"width: 77px;height: 22px;border-radius: 1px;background-color: #4da7dc;cursor: pointer;color: #fff;padding-left: 6px;position: relative;font-size: 10px;top: -23px;line-height: 21px;\">Knowledge Date</div>";
        }
        HTML += "</div>";

        checkBox.after(HTML);

        if (allowUpdate) {
            checkBox.next().unbind('click').bind('click', function (e) {
                if (!checkBox[0].disabled) {
                    // handling for mandatory checkbox
                    var manFlag = true;
                    var showFlag = true;
                    var flag;
                    if (polarisdownstream._pageViewModelInstance != null)
                        flag = polarisdownstream._pageViewModelInstance.SelectedRequireLookupMassagingCurrentKnowledgeDate();
                    var target = $(e.target);
                    if (target.hasClass("sm_toggleText")) {
                        target = target.parent();
                    }
                    if (target.hasClass("sm_toggleBtn")) {
                        target = target.parent();
                    }
                    target = target.find(".sm_toggleBtn");
                    if (flag) {
                        target.animate({
                            "margin-left": "85px",
                            width: "35px"
                        }, function () {
                            $("#" + id).find(".sm_toggleBtn").text("Today");
                            polarisdownstream._pageViewModelInstance.SelectedRequireLookupMassagingCurrentKnowledgeDate(false);
                        });
                    }
                    else {
                        target.animate({
                            "margin-left": "0px",
                            width: "77px"
                        }, function () {
                            $("#" + id).find(".sm_toggleBtn").text("Knowledge Date");
                            polarisdownstream._pageViewModelInstance.SelectedRequireLookupMassagingCurrentKnowledgeDate(true);

                        });
                    }
                    e.stopPropagation();
                }
            });
        }
        else {
            checkBox.next().off('click');

        }

    }

    //////////////////////////////////////////////
    //CODE For MULTI-SELECT DROPDOWN For Filters//
    //////////////////////////////////////////////
    function ApplyMultiSelect(id, isRunningText, data, isSearch, container, text, allSelected, selectedItemsList, isMultiSelect) {
        var selectedItems = [];
        var selectedItem;
        //If no data returned from DB
        if (data.length == 0) {
            var tempObj = {};
            tempObj.text = "";
            tempObj.value = "";
            data.push(tempObj);
            //data.push({ "text": "", "value": "" });
        }
        if (isMultiSelect) {
            if (allSelected) {
                $.each(data, function (key, selectedItemObj) {
                    selectedItems.push(selectedItemObj);
                });
            } else {
                if (typeof (selectedItemsList) != 'undefined' && selectedItemsList.length > 0) {
                    $.each(selectedItemsList, function (key, selectedItemObj) {
                        if (selectedItemObj.text == undefined)
                            selectedItems.push(selectedItemObj);
                        else
                            selectedItems.push(selectedItemObj.text);
                    });
                }
            }
            var data = [{
                options: data,
                text: text,
            }];
        }
        else {
            //selectedItems.push(selectedItemsList);
            selectedItem = selectedItemsList;
            var data = data;
        }

        //To handle Initial Selection, i.e., All Selected
        if (id == 'polarisDownstreamAvailableReportsMultiSelectedItems') {
            polarisdownstream._selectedReports = [];
            if (allSelected) {
                polarisdownstream._selectedReports.push("-1");
            } else {
                polarisdownstream._selectedReports = selectedItems;
            }
            if (selectedItems.length == 0) {
                polarisdownstream._noReportsConfigured = true;
            }
            else {
                polarisdownstream._noReportsConfigured = false;
            }
            polarisdownstream._totalReports = data[0].options.length;
        }


        smselect.create({
            id: id,
            text: text,
            container: $(container),
            isMultiSelect: isMultiSelect,
            select: isMultiSelect ? false : true,
            isRunningText: isRunningText,
            data: data,
            selectedItems: selectedItems,
            showSearch: isSearch,
            selectedText: selectedItem,
            ready: function (selectelement) {
                if (id != 'polarisDownstreamAvailableReportsMultiSelectedItems') {
                    selectelement.css({
                        /*'border': '1px solid #CECECE',*/
                        'border-left': 'none',
                        height: '23px',
                        width: '100%',
                        'border-bottom': '2px solid #9c9696',
                        'vertical-align': 'middle'
                    });
                } else {
                    ddlOnChangeHandler(selectelement);
                }
                //selectelement.find(".smselectrun").css({ height: '27px'});
                selectelement.find(".smselectcon").height(selectelement.find(".smselectcon").height() + 3);
                selectelement.on('change', function (ee) {
                    ddlOnChangeHandler(ee);
                });

                if (!isMultiSelect && typeof (selectedItemsList) != 'undefined' && selectedItemsList != null && selectedItemsList != "")
                    smselect.setOptionByValue(selectelement, selectedItem.toString(), false);
                var target = selectelement;
                if (selectelement[0].id == 'smselect_polarisDownstreamAvailableDateTypesStartItems') {
                    for (var item in smselect.getSelectedOption(target)) {

                        console.log("SelectedStartDate : " + polarisdownstream._pageViewModelInstance.SelectedStartDateValue());
                        var SelectedStartDateVal = smselect.getSelectedOption(target)[0].value;
                        switch (SelectedStartDateVal) {
                            case "4":
                                $("#panelTNDayStart").show();
                                $("#panelCustomDateStart").hide();
                                $(".formattingDivs").hide();
                                smselect.enable($("#smselect_polarisDownstreamAvailableDateTypesEndItems"));
                                break;
                            case "5":
                                $("#panelTNDayStart").hide();
                                $("#panelCustomDateStart").show();
                                $(".formattingDivs").hide();
                                smselect.enable($("#smselect_polarisDownstreamAvailableDateTypesEndItems"));
                                break;
                            case "100":
                                smselect.disable($("#smselect_polarisDownstreamAvailableDateTypesEndItems"));
                                smselect.setOptionByValue($("#smselect_polarisDownstreamAvailableDateTypesEndItems"), "6", false);
                                break;
                            default:
                                $("#panelTNDayStart").hide();
                                $("#panelCustomDateStart").hide();
                                $(".formattingDivs").show();
                                smselect.enable($("#smselect_polarisDownstreamAvailableDateTypesEndItems"));
                                break;
                        }
                    }
                }
                else if (selectelement[0].id == 'smselect_polarisDownstreamAvailableDateTypesEndItems') {
                    for (var item in smselect.getSelectedOption(target)) {

                        console.log("SelectedEndDate : " + polarisdownstream._pageViewModelInstance.SelectedStartDateValue());
                        var SelectedEndtDateVal = smselect.getSelectedOption(target)[0].value;
                        switch (SelectedEndtDateVal) {
                            case "4":
                                $("#panelTNDayEnd").show();
                                $("#panelCustomDateEnd").hide();
                                $(".formattingDivs").hide();
                                break;
                            case "5":
                                $("#panelTNDayEnd").hide();
                                $("#panelCustomDateEnd").show();
                                $(".formattingDivs").hide();
                                break;
                            default:
                                $("#panelTNDayEnd").hide();
                                $("#panelCustomDateEnd").hide();
                                $(".formattingDivs").show();
                                break;
                        }
                    }
                }
            }
        });
    }

    //This function is called when Multi-Select is changed.
    function ddlOnChangeHandler(ee) {
        //
        var target = $(ee.currentTarget);
        if (ee.currentTarget == undefined) {
            target = ee;
        }

        if (target[0].id == 'smselect_polarisDownstreamAvailableReportsMultiSelectedItems') {

            options = target.find('.fa-check');
            var lst = new Array();
            $.each(options, function (ii, ee) {
                lst.push({ value: $($(ee).siblings()[0]).attr('data-value').trim(), text: $($(ee).siblings()[0]).text().trim() });
            });
            var selectedOptions = lst;
            polarisdownstream._selectedReports = [];

            if (selectedOptions.length == polarisdownstream._totalReports) {
                polarisdownstream._selectedReports.push("-1");
            } else {
                for (var item in selectedOptions) {
                    polarisdownstream._selectedReports.push(selectedOptions[item].text)
                }
            }
            polarisdownstream.reportSelectionOpen = true;
        }
        else if (target[0].id == 'smselect_polarisDownstreamAvailableTransportsMultiSelectedItems') {
            if (smselect.getSelectedOption(target).length != 0) {
            }
            var selectedQueues = [];
            for (var item in smselect.getSelectedOption(target)) {

                selectedQueues.push(smselect.getSelectedOption(target)[item].text);
            }

            polarisdownstream._pageViewModelInstance.SelectedQueueName(selectedQueues.join(","))

        }
        else if (target[0].id == 'smselect_polarisDownstreamAvailableCalendarsMultiSelectedItems') {
            if (smselect.getSelectedOption(target) != undefined)
                polarisdownstream._pageViewModelInstance.selectedCalendarName(smselect.getSelectedOption(target)[0].text);
            for (var item in smselect.getSelectedOption(target)) {

                //  polarisdownstream._selectedReports.push(smselect.getSelectedOption(target)[item].text)
            }
        }
        else if (target[0].id == 'smselect_polarisDownstreamAvailableDateTypesStartItems') {

            if (smselect.getSelectedOption(target) != undefined)
                //polarisdownstream._pageViewModelInstance.SelectedStartDate(smselect.getSelectedOption(target)[0].value);
                polarisdownstream._pageViewModelInstance.SelectedStartDateValue(smselect.getSelectedOption(target)[0].text);
            for (var item in smselect.getSelectedOption(target)) {

                console.log("SelectedStartDate : " + polarisdownstream._pageViewModelInstance.SelectedStartDateValue());
                var SelectedStartDateVal = smselect.getSelectedOption(target)[0].value;
                switch (SelectedStartDateVal) {
                    case "4":
                        $("#panelTNDayStart").show();
                        $("#panelCustomDateStart").hide();
                        $(".formattingDivs").hide();
                        break;
                    case "5":
                        $("#panelTNDayStart").hide();
                        $("#panelCustomDateStart").show();
                        $(".formattingDivs").hide();
                        break;
                    case "100":
                        smselect.disable($("#smselect_polarisDownstreamAvailableDateTypesEndItems"));
                        smselect.setOptionByValue($("#smselect_polarisDownstreamAvailableDateTypesEndItems"), "6", false);
                        break;
                    default:
                        $("#panelTNDayStart").hide();
                        $("#panelCustomDateStart").hide();
                        $(".formattingDivs").show();
                        smselect.enable($("#smselect_polarisDownstreamAvailableDateTypesEndItems"));
                        break;
                }
                //  polarisdownstream._selectedReports.push(smselect.getSelectedOption(target)[item].text)
            }
        }
        else if (target[0].id == 'smselect_polarisDownstreamAvailableDateTypesEndItems') {
            //  polarisdownstream._selectedReports = [];
            // polarisdownstream._pageViewModelInstance.SelectedQueueName
            if (smselect.getSelectedOption(target) != undefined)
                //  polarisdownstream._pageViewModelInstance.SelectedEndDate(smselect.getSelectedOption(target)[0].value);
                polarisdownstream._pageViewModelInstance.SelectedEndDateValue(smselect.getSelectedOption(target)[0].text);
            for (var item in smselect.getSelectedOption(target)) {

                console.log("SelectedEndDate : " + polarisdownstream._pageViewModelInstance.SelectedEndDateValue());
                var SelectedEndtDateVal = smselect.getSelectedOption(target)[0].value;
                switch (SelectedEndtDateVal) {
                    case "4":
                        $("#panelTNDayEnd").show();
                        $("#panelCustomDateEnd").hide();
                        $(".formattingDivs").hide();
                        break;
                    case "5":
                        $("#panelTNDayEnd").hide();
                        $("#panelCustomDateEnd").show();
                        $(".formattingDivs").hide();
                        break;
                    default:
                        $("#panelTNDayEnd").hide();
                        $("#panelCustomDateEnd").hide();
                        $(".formattingDivs").show();
                        break;
                }
                //  polarisdownstream._selectedReports.push(smselect.getSelectedOption(target)[item].text)
            }
        }
        else if (target[0].id == 'smselect_polarisDownstreamSelectedConnection') {
            if (smselect.getSelectedOption(target) != undefined) {
                if (polarisdownstream._pageViewModelInstance != null) {
                    polarisdownstream._pageViewModelInstance.selectedDbName(smselect.getSelectedOption(target)[0].text);
                    polarisdownstream._selectedConnectionName = smselect.getSelectedOption(target)[0].text;
                } else {
                    polarisdownstream._selectedConnectionName = smselect.getSelectedOption(target)[0].text;
                }
            }

        }
    }


    ////Getting Data For Multi-Select Filters and Calling Callback function to initialize Multi-Select Filters
    //function getMultiSelectFilters() {
    //    var params = {};

    //    //Call for  Multi-Select Data
    //    CallCommonServiceMethod('GetData', params, OnSuccess_LoadMultiSelectFilters, OnFailure, null, false);
    //}
    function enableRecurring() {
        $('#startDateTxt').datepicker('enable');
        $('input:radio[name=recurrenceTypeRadioGroup]').prop('checked', false);
        $('#recurringRadio').prop('checked', true);
        $('.errorMsg').empty().css('display', 'none');
        $('.parsley-error').removeClass('parsley-error');
        $('#dailyRecurrenceRadio').removeAttr('disabled');
        $('#dailyRecurrenceRadio').prop('checked', true);
        $('#weeklyRecurrenceRadio').removeAttr('disabled');
        $('#monthlyRecurrenceRadio').removeAttr('disabled');
        $('#intervalTxt').removeAttr('disabled');
        $('#numberOfRecurrenceTxt').removeAttr('disabled');
        $('#neverEndJobCheckbox').removeAttr('disabled');
        $('#intervalRecurrenceTxt').removeAttr('disabled');
        $('#dailyRecurrenceRadio').prop('checked', true);
        if (!$('#neverEndJobCheckbox').prop('checked')) {
            $('#endDateTxt').removeAttr('disabled');
            $('#endDateTxt').datepicker('enable');
        }
    }
    function enableNonRecurring() {
        $('#startDateTxt').datepicker('enable');
        $('input:radio[name=recurrenceTypeRadioGroup]').prop('checked', false);
        $('#nonRecurringRadio').prop('checked', true);
        $('.errorMsg').empty().css('display', 'none');
        $('.parsley-error').removeClass('parsley-error');
        $('#dailyRecurrenceRadio').attr('disabled', 'disabled');
        $('#weeklyRecurrenceRadio').attr('disabled', 'disabled');
        $('#monthlyRecurrenceRadio').attr('disabled', 'disabled');
        $('#intervalTxt').attr('disabled', 'disabled');
        $('#numberOfRecurrenceTxt').attr('disabled', 'disabled');
        $('#neverEndJobCheckbox').attr('disabled', 'disabled');
        $('#intervalRecurrenceTxt').attr('disabled', 'disabled');
        $('.dayOfWeekRow').slideUp();
        $('#endDateTxt').attr('disabled', 'disabled');
        $('#endDateTxt').datepicker('disable');
        $('.dayOfWeekRow').slideUp();
        $('#intervalTxt').val("");
        $('#intervalRecurrenceTxt').val("");
        $('#numberOfRecurrenceTxt').val("");
        $('input:radio[name=recurrencePatternRadioGroup]').prop('checked', false);
        $('#neverEndJobCheckbox').prop('checked', false);
    }
    function disableScheduler() {
        enableNonRecurring();
        $('input:radio[name=recurrenceTypeRadioGroup]').prop('checked', false);
        $('#startDateTxt').val("");
        $('#startTimeTxt').val("");
    }
    function validateScheduler() {
        var isRecurring = $("#recurringRadio").is(':checked');
        var RecurringType = $("input[name='recurrenceTypeRadioGroup']:checked").val();
        var isNeverEnd = false;
        var startDate = $("#startDateTxt").val();
        var startTime = $("#startTimeTxt").val();
        var endDate = $("#endDateTxt").val();
        var recurrencePattern = $("input[name='recurrencePatternRadioGroup']:checked").val();
        var interval = $("#intervalTxt").val().trim();
        var recurrenceCount = $("#numberOfRecurrenceTxt").val();
        var timeInterval = $("#intervalRecurrenceTxt").val();
        var dayOfWeek = '';
        if (RecurringType == undefined)
            return null;
        $(".errorMsg").empty().css("display", "none");
        $(".parsley-error").removeClass("parsley-error");
        var invalid = 0;
        if (startDate.trim() == "") {
            invalid++;
            $("#startDateTxt").parent().find(".errorMsg").text("Start Date is required").css("display", "block");
        }
        if (startTime.trim() == "") {
            invalid++;
            $("#startTimeTxt").parent().find(".errorMsg").text("Start Time is required").css("display", "block");
        }
        if (invalid != 0)
            return false;

        if (Date.parseInvariant(startDate + startTime, "MM/dd/yyyyHH:mm:ss") <= Date.now()) {
            invalid++;
            $("#startDateTxt").addClass("parsley-error");
            $("#startTimeTxt").addClass("parsley-error");
            $("#startDateTxt").parent().find(".errorMsg").text("Invalid Start Date").css("display", "block");
            $("#startTimeTxt").parent().find(".errorMsg").text("Invalid Start Time").css("display", "block");
        }
        if (Date.parseInvariant(startTime, "HH:mm:ss") == null) {
            invalid++;
            $("#startTimeTxt").addClass("parsley-error");
            $("#startTimeTxt").parent().find(".errorMsg").text("Invalid Start Time").css("display", "block");
        }



        if (isRecurring) {
            isNeverEnd = $("#neverEndJobCheckbox").is(':checked');
            if (!isNeverEnd) {
                if (Date.parseInvariant(endDate, "MM/dd/yyyy") == null || Date.parseInvariant(endDate, "MM/dd/yyyy") == undefined) {
                    invalid++;
                    $("#endDateTxt").addClass("parsley-error");
                    $("#endDateTxt").parent().find(".errorMsg").text("End date required").css("display", "block");
                } else if (Date.parseInvariant(endDate, "MM/dd/yyyy") < Date.parseInvariant(startDate, "MM/dd/yyyy")) {
                    invalid++;
                    $("#endDateTxt").addClass("parsley-error");
                    $("#endDateTxt").parent().find(".errorMsg").text("End Date should be greater than Start Date").css("display", "block");
                }
            }
            if (interval == "") {
                invalid++;
                $("#intervalTxt").addClass("parsley-error");
                $("#intervalTxt").parent().find(".errorMsg").text("Value required").css("display", "block");
            } else if (isNaN(interval)) {
                invalid++;
                $("#intervalTxt").addClass("parsley-error");
                $("#intervalTxt").parent().find(".errorMsg").text("Invalid input").css("display", "block");
            } else if (interval.trim() == '0') {
                invalid++;
                $("#intervalTxt").addClass("parsley-error");
                $("#intervalTxt").parent().find(".errorMsg").text("Interval should be grater than 0").css("display", "block");
            } else {
                switch (recurrencePattern) {
                    case 'daily':
                        break;
                    case 'weekly':
                        weeklyInterval = interval;
                        var checked = $("input[name='dayOfWeekCheckboxGroup']:checked");
                        checked.each(function (_index, item) {
                            dayOfWeek += $(item).val() + ",";
                        });
                        if (dayOfWeek.length != 0)
                            dayOfWeek = dayOfWeek.slice(0, -1);
                        else {
                            $("input[name='dayOfWeekCheckboxGroup']:first-child").parent().find(".errorMsg").text("Select atleast one day of the week").css("display", "block");
                            invalid++;
                        }
                        break;
                    case 'monthly':
                        break;
                    default:
                        invalid++;
                }
            }


            if (recurrenceCount == "") {
                invalid++;
                $("#numberOfRecurrenceTxt").addClass("parsley-error");
                $("#numberOfRecurrenceTxt").parent().find(".errorMsg").text("Value required").css("display", "block");
            } else if (isNaN(recurrenceCount)) {
                invalid++;
                $("#numberOfRecurrenceTxt").addClass("parsley-error");
                $("#numberOfRecurrenceTxt").parent().find(".errorMsg").text("Invalid input").css("display", "block");
            }
            if (timeInterval == "") {
                invalid++;
                $("#intervalRecurrenceTxt").addClass("parsley-error");
                $("#intervalRecurrenceTxt").parent().find(".errorMsg").text("Value required").css("display", "block");
            } else if (isNaN(timeInterval)) {
                invalid++;
                $("#intervalRecurrenceTxt").addClass("parsley-error");
                $("#intervalRecurrenceTxt").parent().find(".errorMsg").text("Invalid input").css("display", "block");
            }
            var lastTaskDateTime = Date.parseInvariant(startDate + startTime, "MM/dd/yyyyHH:mm:ss");
            var nextDay = Date.parseInvariant(startDate, "MM/dd/yyyy");
            if (nextDay == null)
                return false;
            nextDay.setDate(nextDay.getDate() + 1);
            if (lastTaskDateTime.getTime() + recurrenceCount * timeInterval * 60000 > nextDay.getTime()) {
                invalid++;
                $("#startTimeTxt").addClass("parsley-error");
                $("#startTimeTxt").parent().find(".errorMsg").text("The task should end the same day").css("display", "block");
            }
        } else {
            recurrenceCount = 0;
            timeInterval = 0;
            interval = 0;
        }

        if (invalid == 0) {
            return {
                StartDate: startDate,
                StartTime: startTime,
                EndDate: endDate,
                TimeInterval: parseInt(timeInterval),
                NoEndDate: isNeverEnd,
                Recurring: isRecurring,
                DaysOfWeek: dayOfWeek,
                Interval: interval,
                NumberOfRecurrence: parseInt(recurrenceCount),
                RecurrencePattern: recurrencePattern,
                RecurrenceType: RecurringType
            }
        }
        return false;

    }

    return polarisdownstream;
})();
var mailtag = (function () {
    function MailTag() {
        this.totalWidth = 0;
        this.isModified = false;
        this.intervalid = 0;
        this.emailVsName = new Object();
        this.NameVsEmail = new Object();
        this.subscriptionInfoIntellisenseCollection = new Array();
        var path = window.location.protocol + '//' + window.location.host;
        var pathname = window.location.pathname.split('/');
        for (var ii = 0; ii < pathname.length; ii++) {
            if ((ii !== 0) && (ii !== pathname.length - 1))
                path = path + '/' + pathname[ii];
        }
        this.path = path;
        this.height = 0;
    }

    function validateObject(object) {
        var passed = false;
        if (object.hasOwnProperty('container')) {
            if (object.container instanceof jQuery) {
                if (object.container.length > 0) {
                    if (object.hasOwnProperty('list')) {
                        if (object.list instanceof Array)
                            passed = true;
                        else
                            console.error('List is not of type Array');
                    }
                    else
                        console.error('Object does not contain property "list".');
                }
                else
                    console.error('Container is empty.');
            }
            else
                console.error('Container is not of type jQuery');
        }
        else
            console.error('Object does not contain property "container".');
        return passed;
    }

    function attachHandlers($object) {
        $object.find('.mailtagTextElement').unbind('keyup').keyup(function (e) {
            e.stopPropagation();
            var $this = $(this);
            var list = [];
            var grandParent = $this.parent().parent();
            var value = $this.val().trim();
            //value = value.split("<")[0].trim();

            if (value != '' && value.indexOf(' <') != -1 && value.indexOf('>') != -1) {
                if (value.split(' <')[1].split('>').length > 0)
                    value = value.split(' <')[1].split('>')[0].trim();
            }

            //            if (value.length < 10) {
            //                $this[0].style.height = '1px';
            //                $this[0].style.height = (25 + $this[0].scrollHeight) + 'px';
            //            }
            //            else
            //                $this[0].style.height = mailtag.height + 'px';

            if (e.which === 13) {
                var valuee = $this.val().replaceAll('\r\n', '').replaceAll('\n', '').trim();
                //valuee = valuee.split("<")[0].trim();
                if (valuee !== '' && valuee.indexOf(' <') != -1 && valuee.indexOf('>') != -1) {
                    if (valuee.split(' <')[1].split('>').length > 0)
                        valuee = valuee.split(' <')[1].split('>')[0];
                }

                if (valuee !== '') {
                    var ele = grandParent.find('.hdnTag');
                    if (ele.val().length > 0) {
                        list = ele.val().split(',');
                    }

                    if (mailtag.emailVsName.hasOwnProperty(valuee))
                        list.push(valuee);  //push emailID for entered text

                    var tempo = {};
                    $.each(list, function (ii, ee) {
                        tempo[ee] = '';
                    });

                    var obj = {};
                    obj.container = grandParent;
                    obj.list = Object.keys(tempo);
                    mailtag.create(obj);
                    mailtag.isModified = true;

                }
            }
            else {
                if (mailtag.intervalid !== 0) {
                    clearTimeout(mailtag.intervalid);
                }
                mailtag.intervalid = setTimeout(function () {
                    if (!/[^a-zA-Z .@]/.test(value)) {
                        mailtag.searchAutocomplete(value, 20, $object);
                    }

                }, 100);
            }
        });

        $object.find('.mailtagTextElement').unbind('keydown').keydown(function (e) {
            e.stopPropagation();
            var $this = $(this);
            var value = $this.val();
        });

        $object.find('.mailtagClose').unbind('click').click(function (e) {
            e.stopPropagation();
            var $this = $(this);
            var parent = $this.parent();
            var mailTextName = parent.find('.mailtagText').html();
            var text = '';

            for (var key in mailtag.emailVsName) {
                for (var item = 0; item < mailtag.emailVsName[key].length; item++) {
                    if (key != '' && mailtag.emailVsName[key][item] == mailTextName)
                        text += key + ',';
                }
            }

            //text = mailtag.NameVsEmail[mailTextName];
            if (text.length > 0)
                text = text.substr(0, text.length - 1);


            var ele = $this.parents('.mailtagContainer').find('.hdnTag');
            var arr = ele.val().split(',');

            var hiddenValue = '';
            $.each(arr, function (ii, ee) {
                if (text.indexOf(ee) === -1)
                    hiddenValue += ee + ',';
            });
            hiddenValue = hiddenValue.substr(0, hiddenValue.length - 1);
            //
            if (polarisdownstream._pageViewModelInstance != null)
                polarisdownstream._pageViewModelInstance.SelectedFailureEmail(hiddenValue);

            ele.val(hiddenValue);
            parent.remove();
            mailtag.isModified = true;
        });

        $(document).click(function (e) {
            e.stopPropagation();
            $('.mailtagsearchddl').hide();
        });
        $(".hdnTag").change(function (e) {
            //
        })
    }

    function intellisenseHTML(data, prefixText, $object) {
        var html = '';
        var key = '';
        for (var item = 0; item < data.length; item++) {
            key = data[item].split('<');
            if ((key[0].toLowerCase().startsWith(prefixText.toLowerCase()) || key[1].toLowerCase().startsWith(prefixText.toLowerCase())) && $('#' + $object[0].id + ' .hdnTag').val().indexOf(key[1].split('>')[0]) == -1)
                html += '<div class="mailtagsearchddlItem">' + data[item].replaceAll('<', '&lt;').replaceAll('>', '&gt;') + '</div>';
        }
        //for (key in data) {
        //    if ((key.toLowerCase().startsWith(prefixText.toLowerCase()) || data[key].toLowerCase().startsWith(prefixText.toLowerCase())) && $('#' + $object[0].id + ' .hdnTag').val().indexOf(key) == -1)
        //        html += '<div class="mailtagsearchddlItem">' + data[key] + " &lt;" + key + '&gt;</div>';
        //}
        return html;
    }

    function applySlimScroll(containerSelector, bodySelector) {
        var scrollBodyContainer = $(containerSelector);
        var scrollBody = scrollBodyContainer.find(bodySelector);

        if (scrollBodyContainer.height() < scrollBody.height()) {
            scrollBody.smslimscroll({ height: scrollBodyContainer.height() + 'px', alwaysVisible: true, position: 'right', distance: '2px' });
        }
    }

    function isEmpty(obj) {
        for (var key in obj) {
            if (obj.hasOwnProperty(key))
                return false;
        }
        return true;
    }

    MailTag.prototype.searchAutocomplete = function searchAutocomplete(prefixText, count, $object) {
        var onClickIntellisense = function (e) {
            e.stopPropagation();
            var target = $(e.target);
            var ddlDiv = target.parent().parent();
            var valu = target.text().trim();
            if (ddlDiv.prop('class') === 'smslimScrollDiv')
                ddlDiv = ddlDiv.parent();
            ddlDiv.hide();
            var txtBox = ddlDiv.parent().find('.mailtagTextElement');
            txtBox.val(valu);
            var evnt = $.Event('keyup');
            evnt.which = 13;
            txtBox.trigger(evnt);
        }

        var ddl = $object.find('.mailtagsearchddl');
        var records = intellisenseHTML(mailtag.subscriptionInfoIntellisenseCollection, prefixText, $object);
        if (records != '') {
            ddl.html("<div class='mailtagsearchddlOverflowContainer'>" + records + "</div>");
            if ((($('#' + $object[0].id).offset().left + $('#' + $object[0].id).width()) - ($('#' + $object[0].id + ' .mailtagTextContainer').offset().left + 100)) > 201) {
                ddl.css({
                    'top': $('#' + $object[0].id + ' .mailtagTextElement').offset().top + 20,
                    'left': $('#' + $object[0].id + ' .mailtagTextElement').offset().left
                });
            }
            else {
                ddl.css({
                    'top': $('#' + $object[0].id + ' .mailtagTextElement').offset().top + 20,
                    'left': $('#' + $object[0].id + ' .mailtagTextElement').offset().left - 90
                });
            }
            ddl.show();
            var children = ddl.children();
            children.eq(0).css('padding-top', '4px');
            ddl.off('click', '.mailtagsearchddlItem');
            ddl.on('click', '.mailtagsearchddlItem', onClickIntellisense);
            applySlimScroll(".mailtagsearchddl", ".mailtagsearchddlOverflowContainer");
        }
        else
            ddl.hide();
    }

    MailTag.prototype.create = function (object) {
        //
        String.prototype.replaceAll = function (find, replace) {
            return this.replace(new RegExp(find, 'g'), replace);
        }
        if (validateObject(object)) {
            var html = '';
            var hiddenValue = '';
            html += '<input type="hidden" class="hdnTag" value="" />';

            if (isEmpty(mailtag.emailVsName)) {
                mailtag.emailVsName = object.mailsCollection;
            }

            if (mailtag.subscriptionInfoIntellisenseCollection.length === 0) {
                for (var key in mailtag.emailVsName) {
                    for (var item = 0; item < mailtag.emailVsName[key].length; item++) {
                        if (key != '')
                            mailtag.subscriptionInfoIntellisenseCollection.push(mailtag.emailVsName[key][item] + " <" + key + ">");
                    }
                }
            }

            if (object.list[0] == "") {
                object.list = [];
            }
            $.each(object.list, function (ii, ee) {
                for (var item = 0; item < mailtag.emailVsName[ee].length; item++) {
                    var titl = ee.replaceAll('"', '&quot;');
                    html += '<div class="mailtagContent"><div class="fa fa-times mailtagClose"></div><div class="mailtagText' + (ee.length > 25 ? ' OverflowText' : '') + '" title="' + titl + '">' + mailtag.emailVsName[ee][item] + '</div></div>';
                    hiddenValue += ee + ',';
                }
            });
            if (hiddenValue.length > 0)
                hiddenValue = hiddenValue.substr(0, hiddenValue.length - 1);

            html += '<div class="mailtagTextContainer"><input class="mailtagTextElement" type="text" style="border : none;"/></div><div class="mailtagsearchddl"></div>';
            //$('#' + object.container[0].id + ' .mailtagTextContainer').css('width', parseInt($('#successSubscribtionListContainer').css('width')) - parseInt($('#successSubscribtionListContainer .mailtagContent').css('width')) - 35);
            object.container.html(html);

            if (!object.container.hasClass('mailtagContainer'))
                object.container.addClass('mailtagContainer');
            //
            object.container.find('.hdnTag').val(hiddenValue).trigger('change');
            if (polarisdownstream._pageViewModelInstance != null)
                polarisdownstream._pageViewModelInstance.SelectedFailureEmail(hiddenValue);
            var textele = object.container.find('.mailtagTextElement');
            textele.val('');
            mailtag.height = parseInt(textele.css('height'), 10);

            attachHandlers(object.container);

            if (object.hasOwnProperty('ready') && typeof object.ready === 'function')
                object.ready(object.container);
        }
    }

    MailTag.prototype.getIsModified = function getIsModified() {
        return mailtag.isModified;
    }

    var mailtag = new MailTag();
    return mailtag;
})();