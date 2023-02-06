var ExceptionCommonConfig = (function () {
    var configScreen;
    function ExceptionConfigClass() { }

    var moduleId;
    var instrumentTypeId;
    var exceptions = {};
    var username;
    var ExceptionTypeMapping = {
        '1st Vendor Value Missing': 'FirstVendorValueMissing',
        'Alerts': 'Alert',
        'Duplicates': 'Duplicate',
        'Invalid Data': 'InvalidData',
        'No Vendor Value': 'NoVendorValue',
        'Ref Data Missing': 'RefDataMissing',
        'Show As Exception': 'ShowAsException',
        'Validations': 'Validation',
        'Vendor Mismatch': 'VendorMismatch',
        'Value Change': 'ValueChange',
        'Underlier Missing': 'UnderlierMissing'
    };

    configScreen = new ExceptionConfigClass();

    ExceptionConfigClass.prototype.init = function (obj) {
        var objInfo = eval("(" + obj + ")");
        moduleId = objInfo.moduleId;
        instrumentTypeId = objInfo.instrumentTypeId;
        username = objInfo.UserName;
        if (moduleId == 3)
            $("#underlierDiv").css("display", "block");

        $(document).ready(function () {
            getExceptionConfig();
        });
    };
    function createCheckboxHandlers() {
        $(".filterItem").unbind('click').click(function (e) {
            var target = e.target;
            if (!$(target).hasClass('filterItem')) {
                target = $(target).parent();
                if (!$(target).hasClass('filterItem'))
                    target = $(target).parent();
            }
            else
                target = $(target);
            var isChecked = target.children('.filterItemCheckbox').hasClass('selectedItem');
            var text = target.children()[1].textContent;
            if (text != 'Select All')
                exceptions[ExceptionTypeMapping[text]] = !isChecked;
            else {
                Object.keys(exceptions).forEach(function (item, index) {
                    if (moduleId != 3 && item == 'Underlier')
                        exceptions[item] = false;
                    else
                        exceptions[item] = !isChecked;
                });
            }
            validateCheckBoxes();
        });
    }

    function validateCheckBoxes() {
        var isAllSelected = true;
        $("#exceptionList").children().each(function (index, item) {
            if (index == 0)
                return;
            var text = $(item).children()[1].textContent;
            if (moduleId != 3 && text == 'Underlier Missing')
                return;
            if (exceptions[ExceptionTypeMapping[text]]) {
                if (!$(item).children('.filterItemCheckbox').hasClass('selectedItem'))
                    $(item).children('.filterItemCheckbox').addClass('selectedItem');
            } else {
                $(item).children('.filterItemCheckbox').removeClass('selectedItem');
                isAllSelected = false;
            }
        });
        if (isAllSelected)
            $($("#exceptionList").children()[0]).children('.filterItemCheckbox').addClass('selectedItem');
        else
            $($("#exceptionList").children()[0]).children('.filterItemCheckbox').removeClass('selectedItem');
    }

    function getExceptionConfig() {
        $("#exceptionList").css("display", 'none');
        var param = {};
        param.moduleId = moduleId;
        param.instrumentTypeId = instrumentTypeId;
        CallCommonServiceMethod("getExceptionConfigDetails", param, onSuccessExceptionConfig, onFailure);
    }

    function saveConfig() {
        var param = {}
        param.user = username;
        param.configs = [];
        param.configs.push(exceptions);
        param.configs[0].moduleId = moduleId;
        param.configs[0].instrumentTypeId = instrumentTypeId;
        CallCommonServiceMethod("saveExceptionConfigDetails", param, onSaved, onFailure);
    }
    function onSuccessExceptionConfig(result) {
        if (result == null || result.d == null) {
            exceptions = {
                FirstVendorValueMissing: false,
                Alert: false,
                Duplicate: false,
                InvalidData: false,
                NoVendorValue: false,
                RefDataMissing: false,
                ShowAsException: false,
                Validation: false,
                VendorMismatch: false,
                ValueChange: false,
                UnderlierMissing: false
            };
        } else {
            exceptions = result.d;
            delete exceptions.__type;
        }
        validateCheckBoxes();
        $("#exceptionList").css("display", 'inline-block');
        createCheckboxHandlers();
        $("#saveButton").unbind('click').click(saveConfig);
    }

    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, false, false);
    }

    function onSaved(result) {
        console.log(result);
        showErrorDiv('Success', 'pass_icon.png', "Saved exception configuration");
    }
    function onFailure(e) {
        console.log(e);
        showErrorDiv('Error', 'fail_icon.png', e.statusText);
    }
    function showErrorDiv(errorHeading, srcImageName, errorMessageText) {
        var path = window.location.protocol + '//' + window.location.host;
        var pathname = window.location.pathname.split('/');

        $.each(pathname, function (ii, ee) {
            if ((ii !== 0) && (ii !== pathname.length - 1))
                path = path + '/' + ee;
        });

        path += '/images/icons/';

        $("#exceptionConfigError_ImageURL").attr('src', path + srcImageName);
        $(".exceptionConfigError_popupTitle").html(errorHeading);
        $(".exceptionConfigError_popupMessage").html(errorMessageText);
        if (errorHeading.toLowerCase().trim() == "error") {
            $('.exceptionConfigError_popupContainer').css('border-top', '4px solid rgb(199, 137, 140)');
            $('.exceptionConfigError_popupTitle').css('color', '4px solid #8a8787');
            $('.exceptionConfigError_popupMessageContainer').css('margin-left', '25px');
            $('#exceptionConfigErrorDiv_messagePopUp').show();
            $('#exceptionConfig_disableDiv').show();
            return false;
        }
        if (errorHeading.toLowerCase().trim() == "success") {
            $('.exceptionConfigError_popupContainer').css('border-top', '4px solid rgb(172, 211, 115)');
            $('.exceptionConfigError_popupTitle').css('color', '4px solid rgb(172, 211, 115)');
            $('.exceptionConfigError_popupMessageContainer').css('margin-left', '72px');
            $('#exceptionConfigErrorDiv_messagePopUp').show();
            $('#exceptionConfig_disableDiv').show();
            return true;
        }
    }

    return configScreen;
})();