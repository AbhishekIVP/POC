var moduleIdInfo = {};
moduleIdInfo[3] = { title: "Security Types", id: "SecurityTypeContainer" };
moduleIdInfo[6] = { title: "Entity Types", id: "EntityTypeContainer" };
moduleIdInfo[18] = { title: "Funds", id: "FundTypeContainer" };
moduleIdInfo[20] = { title: "Parties", id: "PartyTypeContainer" };

if (!String.format) {
    String.format = function (format) {
        var args = Array.prototype.slice.call(arguments, 1);
        return format.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
                ? args[number]
                : match
                ;
        });
    };
}
var moduleListContainer = '<div id="{0}" class="instumentTypeContainer" moduleid="{2}"><div>{1}</div><div class="instrumentTypeListContainer"><ul class="instrumentTypeList"></ul></div></div>';
var listItem = '<li class="{2}" instrumentTypeId="{0}"><div>{1}</div><i class="fa fas fa-trash"></i></li>';

var instrumentListEventScreen = (function () {
    var instrumentListEventScreen;
    function InstrumentListEventScreenClass() {
    }

    instrumentListEventScreen = new InstrumentListEventScreenClass();

    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    var availableModule = [];
    var username;
    InstrumentListEventScreenClass.prototype.init = function (obj) {
        username = eval("(" + obj + ")").username;
        var modulePresent = getModulesByPrevilages(username, getQueryParams("identifier", document.URL), '-1', '0').d;
        if (modulePresent == null || modulePresent.length == 0)
            availableModule = [];
        else {
            modulePresent = modulePresent.filter(function (item) {
                return moduleIdInfo[item.moduleId] != undefined;
            });
            modulePresent.forEach(function (item) { availableModule.push(item.moduleId); });
        }
        $(document).ready(function () {
            registerSearchListener();
            createModuleListContainers();
            CallCommonServiceMethod("GetEventInstrumentTypesConfigList", { moduleIds: availableModule }, onSuccessGetEventInstrumentTypesConfigList, onFailure);
        });
    }

    function createModuleListContainers() {
        var moduleContainer = $("#ModuleContainer");
        availableModule.forEach(function (item) {
            var moduleInfo = moduleIdInfo[item];
            moduleContainer.append(String.format(moduleListContainer, moduleInfo.id, moduleInfo.title, item));
        });
    }

    function registerSearchListener() {
        $("#instrumentSearch input").on("change paste keyup", function () {
            var text = this.value.trim().toLowerCase();

            var listItems = $(".instrumentTypeList>li");
            if (text.length == 0) {
                listItems.css("display", "list-item");
            }

            listItems.each(function (index, item) {
                if ($($(item).children()[0]).innerHTML().toLowerCase().includes(text))
                    $(item).css("display", "list-item");
                else
                    $(item).css("display", "none");
            });
        });
    }

    function registerInstrumentClickListener() {
        $(".instrumentTypeList li").unbind('click').click(function (e) {
            var target = $(e.target).closest('li');
            var instrument_type_id = target.attr("instrumenttypeid");
            var module_id = target.closest('.instumentTypeContainer').attr("moduleId");
            var instrument_type_name = target.children()[0].textContent;
            var url = String.format('{0}/InstrumentEventConfig.aspx?moduleId={1}&&instrumentTypeId={2}&&instrumentTypeName={3}',
                path, module_id, instrument_type_id, instrument_type_name);
            console.log(target);
            location.replace(url);
        });
        $(".instrumentTypeList li i").unbind('click').click(function (e) {
            var target = $(e.target).closest('li');
            var instrument_type_id = target.attr("instrumenttypeid");
            var module_id = target.closest('.instumentTypeContainer').attr("moduleId");
            showErrorDiv('Alert', 'alert_icon.png', "Do you want to delete event configurations for " + target.find('div').text()+"?",
                function () {
                    CallCommonServiceMethod("DeleteEventSettingForInstrumentType", { moduleId: module_id, instrumentTypeId: instrument_type_id }, function () {
                        CallCommonServiceMethod("GetEventInstrumentTypesConfigList", { moduleIds: [3, 6, 18, 20] }, onSuccessGetEventInstrumentTypesConfigList, onFailure);
                    }, null);
                });
            
            e.stopPropagation();
        });
    }

    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, false, false);
    }
    function ExecuteSynchronously(url, method, args) {
        var executor = new Sys.Net.XMLHttpSyncExecutor();
        var request = new Sys.Net.WebRequest();
        request.set_url(url + '/' + method);
        request.set_httpVerb('POST');
        request.get_headers()['Content-Type'] = 'application/json; charset=utf-8';
        request.set_executor(executor);
        request.set_body(Sys.Serialization.JavaScriptSerializer.serialize(args));
        request.invoke();
        if (executor.get_responseAvailable()) {
            return (executor.get_object());
        }
        return (false);
    }

    function getModulesByPrevilages(userName, pageId, specificModuleId, selectedModuleToDisplay) {
        var params = {};
        params.userName = userName;
        params.pageIdentifier = pageId;
        if (typeof getRMLeftMenu() != 'undefined') {
            params.baseModule = getRMLeftMenu().baseModule;
            params.currentModule = getRMLeftMenu().currentModule;
        }
        else {
            params.baseModule = "secmaster";
            params.currentModule = "secmaster";
        }
        var data = ExecuteSynchronously(path + '/BaseUserControls/Service/CommonService.svc', 'GetCommonModuleTypeDetails', params);
        return data;
    }


    function onSuccessGetEventInstrumentTypesConfigList(result) {
        if (result.d == null)
            return;
        var data = result.d;
        data.forEach(function (item) {
            var moduleContainerId = moduleIdInfo[item.moduleId].id;
            var moduleList = $("#" + moduleContainerId + " .instrumentTypeList");
            moduleList.html("");
            item.instrumentTypes.forEach(function (instrumentType) {
                var html = String.format(listItem, instrumentType.instrumentId, instrumentType.instrumentName, instrumentType.isConfigured ? "configuredInstrument" : "");
                moduleList.append(html);
            });
        });
        registerInstrumentClickListener();
    }


    function onFailure() {
    }

    return instrumentListEventScreen;
})();

const getQueryParams = (params, url) => {

    let href = url;
    //this expression is to get the query strings
    let reg = new RegExp('[?&]' + params + '=([^&#]*)', 'i');
    let queryString = reg.exec(href);
    return queryString ? queryString[1] : null;
};

function showErrorDiv(errorHeading, srcImageName, errorMessageText, callback) {
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    path += '/images/icons/';

    $("#eventConfigError_ImageURL").attr('src', path + srcImageName);
    $(".eventConfigError_popupTitle").html(errorHeading);
    $(".eventConfigError_popupMessage").html(errorMessageText);
    $('.eventConfig_btns').hide();
    if (errorHeading.toLowerCase().trim() == "error") {
        $('.eventConfigError_popupContainer').css('border-top', '4px solid rgb(199, 137, 140)');
        $('.eventConfigError_popupTitle').css('color', '4px solid #8a8787');
        $('.eventConfigError_popupMessageContainer').css('margin-left', '25px');
        $('#eventConfigErrorDiv_messagePopUp').show();
        $('#eventConfig_disableDiv').show();
        return false;
    }
    if (errorHeading.toLowerCase().trim() == "success") {
        $('.eventConfigError_popupContainer').css('border-top', '4px solid rgb(172, 211, 115)');
        $('.eventConfigError_popupTitle').css('color', '4px solid rgb(172, 211, 115)');
        $('.eventConfigError_popupMessageContainer').css('margin-left', '72px');
        $('#eventConfigErrorDiv_messagePopUp').show();
        $('#eventConfig_disableDiv').show();
        return true;
    }
    if (errorHeading.toLowerCase().trim() == "alert") {
        $('.eventConfigError_popupContainer').css('border-top', '4px solid rgb(244, 173, 2)');
        $('.eventConfigError_popupTitle').css('color', '4px solid rgb(244, 173, 2)');
        $('.eventConfigError_popupMessageContainer').css('margin-left', '72px');
        $('.eventConfig_btns').show();
        $('#eventConfigErrorDiv_messagePopUp').show();
        $('#eventConfig_disableDiv').show();
        $('.eventConfig_popupOkBtn').unbind('click').click(function (e) {
            callback(e);
            $('#eventConfigErrorDiv_messagePopUp').hide();
            $('#eventConfig_disableDiv').hide();
        });
        return true;
    }
}