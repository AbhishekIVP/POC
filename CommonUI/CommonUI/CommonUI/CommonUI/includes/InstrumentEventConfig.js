var instrumentEventConfig = (function () {
    var instrumentEventConfig;

    function InstrumentEventConfigClass() {
    }

    instrumentEventConfig = new InstrumentEventConfigClass();

    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    var instanceData;

    var configData;
    var isDisplayAttributeConfigured = false;
    var onDisplayAttribute = -1;
    var isDisplayLegConfigured = false;
    var onDisplayLeg = -1;
    var transportNames;

    InstrumentEventConfigClass.prototype.init = function (obj) {
        var objInfo = eval("(" + obj + ")");
        instanceData = objInfo;
        if (objInfo.moduleId == 0)
            return;

        $(document).ready(function () {
            getAllTransportNames();
            initHeader();
        });

    }

    function setConfigChange(action, config) {
        config.delete = false;
        config.update = false;
        switch (action) {
            case 'delete':
                config.delete = true;
                if (config.ConfigId == 0) {
                    config.ConfigId = -1;
                }
                break;
            case 'update':
                config.update = true;
                if (config.ConfigId == 0)
                    //insert then
                    config.ConfigId = -1;
                break;
        }
    }

    function resizeListener() {
        $(".configTabContent").height($(".configTabContainerDiv").parent().height() - $(".configTabContainerDiv").height() - 2);
    }

    function initHeader() {
        $("#mainContainer").css('display', 'flex');
        $(".configTab ").first().html(tabName[instanceData.moduleId]);
        resizeListener();
        $(window).unbind('resize').resize(resizeListener);
        $("#InstrumentTitle").html(instrumentNamePrefix[instanceData.moduleId] + ": " + instanceData.instrumentTypeName);
        $(".configTab").unbind('click').on('click', function (e) {
            if (!validateConfigUI())
                return;
            var selected = $(e.target);
            $(".configTab").removeClass("configTabSelected");
            $(".configTabContent").css("display", 'none');
            selected.addClass("configTabSelected");
            switch (selected.text()) {
                case 'Attribute Level':
                    $("#AttributeLevel").css('display', 'flex');
                    break;
                case 'Leg Level':
                    $("#LegLevel").css('display', 'flex');
                    break;
                default:
                    $("#InstrumentLevel").css('display', 'flex');
            }
        });
        $('.backButton').unbind('click').click(function () {
            if (isPendingChanges()) {
                showErrorDiv('Alert', 'alert_icon.png', "Changes made are not saved. Do you want to go back?",
                    function () { location.replace(path + '/InstrumentListEventScreen.aspx?identifier=Common_EventNotification');});
            }else
            location.replace(path + '/InstrumentListEventScreen.aspx?identifier=Common_EventNotification');
        });
        $("#saveButton").unbind('click').click(saveInstrumentTypeEventConfig);

        $(document).unbind('click', documentClickListener).click(documentClickListener);
    }

    function documentClickListener(e) {
        var visibleDropDown = $('.multiSelectTagDropDown:visible');
        if ($(e.target).closest(".multiSelectTagDropDown").length == 0)
            visibleDropDown.css('display', 'none');
    }

    function initQueue(tabID, queueList, listener) {
        var transportContainerHtml = "";
        var transportContainer = $(tabID + " .multiSelectTagContainer");
        transportNames.forEach(function (item) {
            if (queueList.includes(item))
                transportContainerHtml += String.format(transportContainerItem, item, "selectedMultiTag");
        });
        if (transportContainerHtml == "")
            transportContainerHtml = '<div style="display:inline-block">Add a Queue</div>';
        transportContainerHtml += "<div class='multiSelectAdd'>+</div>";
        transportContainer.html(transportContainerHtml);

        var dropdown = $(tabID + " .multiSelectTagDropDown>div");
        transportContainer.find(".multiSelectAdd").unbind('click').click(function (e) {
            var dropDownContainer = $(e.target).closest(".multiSelectTagWidget").find(".multiSelectTagDropDown");
            if (dropDownContainer.css('display') == 'block')
                dropDownContainer.css('display', 'none');
            else {
                dropDownContainer.css('display', 'block');
                dropDownContainer.css('left', e.target.offsetLeft - 65);
            }
            e.stopPropagation();
        });
        var dropDownContainerHtml = "";
        transportNames.forEach(function (item) {
            dropDownContainerHtml += String.format(transportContainerItem, item, queueList.includes(item) ? "selectedMultiTag" : "");
        });
        dropdown.html(dropDownContainerHtml);

        $(tabID + " .multiSelectTagDropDown .multiSelectTag").unbind('click').click(listener);
        $(tabID + " .multiSelectTagContainer .selectedMultiTag span").unbind('click').click(listener);
    }

    function validateConfigUI() {
        var root = $(".configTabContent:visible");
        var selectedQueue = root.find(".selectedMultiTag").length;
        var selectedActions = root.find(".selectedAction").length;
        if (selectedActions > 0 && selectedQueue == 0) {
            showErrorDiv('Error', 'fail_icon.png', "Select atleast one queue for selected actions");
            return false;
        } if (selectedActions == 0 && selectedQueue > 0) {
            showErrorDiv('Error', 'fail_icon.png', "Select atleast one action for selected queue");
            return false;
        }
        return true;
    }
    //region: Instrument Level
    function initInstrumentLevel() {
        var actionList = $("#InstrumentLevel .actionsContainer");
        var html = "";
        instrumentLevelActions.forEach(function (item) {
            html += String.format(actionItem,
                configData.SelectedInstrumentActions.includes(item) ? "selectedAction" : "",
                item.toString(),
                actionTypeName[item]);
        });
        if (configData.SelectedInstrumentActions.length == instrumentLevelActions.length) {
            $("#InstrumentLevel .selectAllDiv").addClass("allSelected");
        } else
            $("#InstrumentLevel .selectAllDiv").removeClass("allSelected");
        actionList.html(html);

        initQueue("#InstrumentLevel", configData.SelectedInstrumentQueue, InstrumentQueueSelectListener);
        initInstrumentLevelListeners();
    }
    function InstrumentQueueSelectListener(e) {
        var target = $(e.target).closest(".multiSelectTag");
        var queue = target.text().slice(0, -1);
        if (target.hasClass("selectedMultiTag")) {
            configData.SelectedInstrumentQueue = configData.SelectedInstrumentQueue.filter(function (item) { return item != queue; });
        } else {
            configData.SelectedInstrumentQueue.push(queue);
        }
        if (configData.ConfigId!=0)
            setConfigChange('update', configData);
        initQueue("#InstrumentLevel", configData.SelectedInstrumentQueue, InstrumentQueueSelectListener);

        e.stopPropagation();
    }
    function initInstrumentLevelListeners() {
        $("#InstrumentLevel .action").unbind('click').click(function (e) {
            var target = $(e.target).closest(".action");
            var eventId = getActionIdFor(target);
            if (isActionSelected(target)) {
                target.removeClass('selectedAction');
                configData.SelectedInstrumentActions = configData.SelectedInstrumentActions.filter(function (item) {
                    return item != eventId;
                });
                if (configData.SelectedInstrumentActions.length == 0)
                    setConfigChange('delete', configData);
                else
                    setConfigChange('update', configData);
                $("#InstrumentLevel .selectAllDiv").removeClass("allSelected");
            } else {
                target.addClass('selectedAction');
                configData.SelectedInstrumentActions.push(eventId);
                if (configData.SelectedInstrumentActions.length == instrumentLevelActions.length) {
                    $("#InstrumentLevel .selectAllDiv").addClass("allSelected");
                }

                setConfigChange('update', configData);

            }
        });
        $("#InstrumentLevel .selectAllDiv").unbind('click').click(function (e) {
            var target = $(e.target).closest(".selectAllDiv");
            if (target.hasClass('allSelected')) {
                target.removeClass("allSelected");
                $("#InstrumentLevel .action").removeClass('selectedAction');
                configData.SelectedInstrumentActions = [];
                setConfigChange('delete', configData);
            } else {
                target.addClass("allSelected");
                $("#InstrumentLevel .action").addClass('selectedAction');
                configData.SelectedInstrumentActions = Array.from(instrumentLevelActions);
                setConfigChange('update', configData);
            }
        });
    }

    //region: Attribute Level
    function initAttributeLevel() {
        
        var html = "";
        

        var configured = '';
        var available = '';
        configData.AttributeLevel.forEach(function (data, index) {
            if (data.SelectedAttributeLegActions.length > 0) {
                configured += String.format(configuredAttributeLegListItem,
                    index, data.DisplayName, "");
                if (onDisplayAttribute == -1) {
                    onDisplayAttribute = index;
                    isDisplayAttributeConfigured = true;
                }
            } else {
                available += String.format(availableAttributeLegListItem, index, data.DisplayName, '');
            }
        });
        if (onDisplayAttribute == -1 && configData.AttributeLevel.length > 0)
            onDisplayAttribute = 0;
        $("#AttributeLevel .configuredAttributeLegList").html(configured);
        $("#AttributeLevel .availableAttributeLegList").html(available);
        if (configData.AttributeLevel.length == 0) {
            $("#AttributeLevel .placeholderForSelection").html("No Attribute to configure");
            return;
        }
        if (onDisplayAttribute != -1) {
            $("#AttributeLevel .configuredAttributeLegList li,#AttributeLevel .availableAttributeLegList li").removeClass('selectedAttributeLeg');
            $('#AttributeLevel .configuredAttributeLegList li[configIndex="' + onDisplayAttribute +
                '"],#AttributeLevel .availableAttributeLegList li[configIndex="' + onDisplayAttribute + '"]').addClass('selectedAttributeLeg');
            initQueue("#AttributeLevel", configData.AttributeLevel[onDisplayAttribute].SelectedAttributeLegQueue, AttributeQueueSelectListener);
            $("#AttributeLevel .actionConfig").css('display', 'block');
            $("#AttributeLevel .placeholderForSelection").css('display', 'none');
        }

        var actionList = $("#AttributeLevel .actionsContainer");
        attributeLegLevelActions.forEach(function (item) {
            html += String.format(actionItem,
                onDisplayAttribute != -1 && configData.AttributeLevel[onDisplayAttribute].SelectedAttributeLegActions.includes(item) ? 'selectedAction' : '',
                item.toString(),
                actionTypeName[item]);
        });
        actionList.html(html);

        initAttributeLevelListeners();
    }
    function AttributeQueueSelectListener(e) {
        var target = $(e.target).closest(".multiSelectTag");
        var queue = target.text().slice(0, -1);
        if (target.hasClass("selectedMultiTag")) {
            target.removeClass('selectedMultiTag');
            configData.AttributeLevel[onDisplayAttribute].SelectedAttributeLegQueue =
                configData.AttributeLevel[onDisplayAttribute].SelectedAttributeLegQueue.filter(function (item) { return item != queue; });
        } else {
            target.addClass('selectedMultiTag');
            configData.AttributeLevel[onDisplayAttribute].SelectedAttributeLegQueue.push(queue);
        }
        if (configData.AttributeLevel[onDisplayAttribute].ConfigId != 0)
            setConfigChange('update', configData.AttributeLevel[onDisplayAttribute]);
        initQueue("#AttributeLevel", configData.AttributeLevel[onDisplayAttribute].SelectedAttributeLegQueue, AttributeQueueSelectListener);
        e.stopPropagation();
    }
    function initAttributeLevelListeners() {
        //list item selection
        $("#AttributeLevel .configuredAttributeLegList li,#AttributeLevel .availableAttributeLegList li").unbind('click').click(function (e) {
            var target = $(e.target).closest("li");
            var index = getConfigIndexOf(target);
            if (index == onDisplayAttribute)
                return;
            if (!validateConfigUI())
                return;
            onDisplayAttribute = index;
            $("#AttributeLevel .actionConfig").css('display', 'block');
            $("#AttributeLevel .placeholderForSelection").css('display', 'none');
            initQueue("#AttributeLevel", configData.AttributeLevel[onDisplayAttribute].SelectedAttributeLegQueue, AttributeQueueSelectListener);
            $("#AttributeLevel .configuredAttributeLegList li,#AttributeLevel .availableAttributeLegList li").removeClass('selectedAttributeLeg');
            target.addClass('selectedAttributeLeg');
            if (target.parent().hasClass('configuredAttributeLegList')) {
                isDisplayAttributeConfigured = true;
                if (configData.AttributeLevel[onDisplayAttribute].SelectedAttributeLegActions.length == attributeLegLevelActions.length) {
                    $("#AttributeLevel .action").addClass('selectedAction');
                    $("#AttributeLevel .selectAllDiv").addClass('allSelected');
                } else {
                    $("#AttributeLevel .action").each(function (index, item) {
                        var action = $(item);
                        if (configData.AttributeLevel[onDisplayAttribute].SelectedAttributeLegActions.includes(getActionIdFor(action)))
                            action.addClass('selectedAction');
                        else
                            action.removeClass('selectedAction');
                    });
                    $("#AttributeLevel .selectAllDiv").removeClass('allSelected');
                }
            } else {
                $("#AttributeLevel .action").removeClass('selectedAction');
                $("#AttributeLevel .selectAllDiv").removeClass('allSelected');
                isDisplayAttributeConfigured = false;
            }

            initAttributeLevelListeners();
        });
        //clicking on delete button of configured list
        $("#AttributeLevel .configuredAttributeLegList li i").unbind('click').click(function (e) {
            e.stopPropagation();
            var target = $(e.target).closest("li");
            var index = getConfigIndexOf(target);
            var callback = function () {
                configData.AttributeLevel[index].SelectedAttributeLegActions = [];
                configData.AttributeLevel[index].SelectedAttributeLegQueue = [];
                if (index == onDisplayAttribute) {
                    target.removeClass("allSelected");
                    $("#AttributeLevel .action").removeClass('selectedAction');
                    setDisplayAttributeConfigured(false);
                    initQueue("#AttributeLevel", configData.AttributeLevel[onDisplayAttribute].SelectedAttributeLegQueue, AttributeQueueSelectListener);
                } else {
                    $("#AttributeLevel .configuredAttributeLegList li[configindex=" + index + "]").remove();
                    html = String.format(availableAttributeLegListItem,
                        index,
                        configData.AttributeLevel[index].DisplayName,
                        '');
                    $("#AttributeLevel .availableAttributeLegList").prepend(html).scrollTop(0);
                    initAttributeLevelListeners();
                }
                setConfigChange('delete', configData.AttributeLevel[index]);
            };
            showErrorDiv('Alert', 'alert_icon.png', "Do you want to delete event configuration for Attribute: "+configData.AttributeLevel[index].DisplayName,callback);
        });
        //selecting a action
        $("#AttributeLevel .action").unbind('click').click(function (e) {
            var target = $(e.target).closest(".action");
            var eventId = getActionIdFor(target);
            if (isActionSelected(target)) {
                target.removeClass('selectedAction');
                configData.AttributeLevel[onDisplayAttribute].SelectedAttributeLegActions =
                    configData.AttributeLevel[onDisplayAttribute].SelectedAttributeLegActions.filter(function (item) {
                        return item != eventId;
                    });
                if (configData.AttributeLevel[onDisplayAttribute].SelectedAttributeLegActions.length == 0) {
                    setDisplayAttributeConfigured(false);
                    setConfigChange('delete', configData.AttributeLevel[onDisplayAttribute]);
                }
                else {
                    setDisplayAttributeConfigured(true);
                    setConfigChange('update', configData.AttributeLevel[onDisplayAttribute]);
                }
                $("#AttributeLevel .selectAllDiv").removeClass("allSelected");
            } else {
                target.addClass('selectedAction');
                configData.AttributeLevel[onDisplayAttribute].SelectedAttributeLegActions.push(eventId);
                if (configData.AttributeLevel[onDisplayAttribute].SelectedAttributeLegActions.length == attributeLegLevelActions.length) {
                    $("#AttributeLevel .selectAllDiv").addClass("allSelected");
                }
                setConfigChange('update', configData.AttributeLevel[onDisplayAttribute]);
                setDisplayAttributeConfigured(true);
            }
        });
        //clicking on selectAll
        $("#AttributeLevel .selectAllDiv").unbind('click').click(function (e) {
            var target = $(e.target).closest(".selectAllDiv");
            if (target.hasClass('allSelected')) {
                target.removeClass("allSelected");
                $("#AttributeLevel .action").removeClass('selectedAction');
                configData.AttributeLevel[onDisplayAttribute].SelectedAttributeLegActions = [];
                setDisplayAttributeConfigured(false);
                setConfigChange('delete', configData.AttributeLevel[onDisplayAttribute]);
            } else {
                target.addClass("allSelected");
                $("#AttributeLevel .action").addClass('selectedAction');
                configData.AttributeLevel[onDisplayAttribute].SelectedAttributeLegActions = Array.from(attributeLegLevelActions);
                setDisplayAttributeConfigured(true);
                setConfigChange('update', configData.AttributeLevel[onDisplayAttribute]);
            }

        });

        $("#AttributeLevel .availableAttributeSearch input").on("change paste keyup", function () {
            var text = this.value.trim().toLowerCase();
            var listItems = $("#AttributeLevel .availableAttributeLegList>li");
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
    function setDisplayAttributeConfigured(isConfiguredNow) {
        if (isDisplayAttributeConfigured == isConfiguredNow)
            return;
        var html;
        isDisplayAttributeConfigured = isConfiguredNow;
        if (isConfiguredNow) {
            $("#AttributeLevel .availableAttributeLegList li[configindex=" + onDisplayAttribute + "]").remove();
            html = String.format(configuredAttributeLegListItem,
                onDisplayAttribute,
                configData.AttributeLevel[onDisplayAttribute].DisplayName,
                'selectedAttributeLeg');
            $("#AttributeLevel .configuredAttributeLegList").prepend(html).scrollTop(0);
        } else {
            $("#AttributeLevel .configuredAttributeLegList li[configindex=" + onDisplayAttribute + "]").remove();
            html = String.format(availableAttributeLegListItem,
                onDisplayAttribute,
                configData.AttributeLevel[onDisplayAttribute].DisplayName,
                'selectedAttributeLeg');
            $("#AttributeLevel .availableAttributeLegList").prepend(html).scrollTop(0);
        }
        initAttributeLevelListeners();
    }

    //region: Leg Level
    function initLegLevel() {
        var html = "";
       

        var configured = '';
        var available = '';
        configData.LegLevel.forEach(function (data, index) {
            if (data.SelectedAttributeLegActions.length > 0) {
                configured += String.format(configuredAttributeLegListItem,
                    index, data.DisplayName, '');
                if (onDisplayLeg == -1 && configData.AttributeLevel.length > 0) {
                    onDisplayLeg = 0;
                    isDisplayLegConfigured = true;
                }
            } else {
                available += String.format(availableAttributeLegListItem, index, data.DisplayName, '');
            }
        });
        if (onDisplayLeg == -1 && configData.LegLevel.length > 0)
            onDisplayLeg = 0;
        $("#LegLevel .configuredAttributeLegList").html(configured);
        $("#LegLevel .availableAttributeLegList").html(available);
        if (configData.LegLevel.length == 0) {
            $("#LegLevel .placeholderForSelection").html("No Leg to configure");
            return;
        }
        if (onDisplayLeg != -1) {
            $("#LegLevel .configuredAttributeLegList li,#LegLevel .availableAttributeLegList li").removeClass('selectedAttributeLeg');
            $('#LegLevel .configuredAttributeLegList li[configIndex="' + onDisplayLeg +
                '"],#LegLevel .availableAttributeLegList li[configIndex="' + onDisplayLeg + '"]').addClass('selectedAttributeLeg');
            $("#LegLevel .actionConfig").css('display', 'block');
            $("#LegLevel .placeholderForSelection").css('display', 'none');
            initQueue("#LegLevel", configData.LegLevel[onDisplayLeg].SelectedAttributeLegQueue, LegQueueSelectListener);
        }

        var actionList = $("#LegLevel .actionsContainer");
        attributeLegLevelActions.forEach(function (item) {
            html += String.format(actionItem,
                onDisplayLeg != -1 && configData.LegLevel[onDisplayLeg].SelectedAttributeLegActions.includes(item) ? 'selectedAction' : '',
                item.toString(),
                actionTypeName[item]);
        });
        actionList.html(html);

        initLegLevelListeners();
    }
    function LegQueueSelectListener(e) {
        var target = $(e.target).closest(".multiSelectTag");
        var queue = target.text().slice(0, -1);
        if (target.hasClass("selectedMultiTag")) {
            target.removeClass('selectedMultiTag');
            configData.LegLevel[onDisplayLeg].SelectedAttributeLegQueue =
                configData.LegLevel[onDisplayLeg].SelectedAttributeLegQueue.filter(function (item) { return item != queue; });
        } else {
            target.addClass('selectedMultiTag');
            configData.LegLevel[onDisplayLeg].SelectedAttributeLegQueue.push(queue);
        }
        if (configData.LegLevel[onDisplayLeg].ConfigId != 0)
            setConfigChange('update', configData.LegLevel[onDisplayLeg]);

        initQueue("#LegLevel", configData.LegLevel[onDisplayLeg].SelectedAttributeLegQueue, LegQueueSelectListener);
        e.stopPropagation();
    }
    function initLegLevelListeners() {
        //list item selection
        $("#LegLevel .configuredAttributeLegList li,#LegLevel .availableAttributeLegList li").unbind('click').click(function (e) {
            var target = $(e.target).closest("li");
            var index = getConfigIndexOf(target);
            if (index == onDisplayLeg)
                return;
            if (!validateConfigUI())
                return;
            onDisplayLeg = index;
            $("#LegLevel .actionConfig").css('display', 'block');
            $("#LegLevel .placeholderForSelection").css('display', 'none');
            initQueue("#LegLevel", configData.LegLevel[onDisplayLeg].SelectedAttributeLegQueue, LegQueueSelectListener);
            $("#LegLevel .configuredAttributeLegList li,#LegLevel .availableAttributeLegList li").removeClass('selectedAttributeLeg');
            target.addClass('selectedAttributeLeg');
            if (target.parent().hasClass('configuredAttributeLegList')) {
                isDisplayLegConfigured = true;
                if (configData.LegLevel[onDisplayLeg].SelectedAttributeLegActions.length == attributeLegLevelActions.length) {
                    $("#LegLevel .action").addClass('selectedAction');
                    $("#LegLevel .selectAllDiv").addClass('allSelected');
                } else {
                    $("#LegLevel .action").each(function (index, item) {
                        var action = $(item);
                        if (configData.LegLevel[onDisplayLeg].SelectedAttributeLegActions.includes(getActionIdFor(action)))
                            action.addClass('selectedAction');
                        else
                            action.removeClass('selectedAction');
                    });
                    $("#LegLevel .selectAllDiv").removeClass('allSelected');
                }
            } else {
                $("#LegLevel .action").removeClass('selectedAction');
                $("#LegLevel .selectAllDiv").removeClass('allSelected');
                isDisplayLegConfigured = false;
            }

            initLegLevelListeners();
        });
        //clicking on delete button of configured list
        $("#LegLevel .configuredAttributeLegList li i").unbind('click').click(function (e) {
            e.stopPropagation();
            var target = $(e.target).closest("li");
            var index = getConfigIndexOf(target);
            var callback = function () {
                configData.LegLevel[index].SelectedAttributeLegActions = [];
                configData.LegLevel[index].SelectedAttributeLegQueue = [];
                if (index == onDisplayLeg) {
                    target.removeClass("allSelected");
                    $("#LegLevel .action").removeClass('selectedAction');
                    setDisplayLegConfigured(false);
                    initQueue("#LegLevel", configData.LegLevel[onDisplayLeg].SelectedAttributeLegQueue, LegQueueSelectListener);
                } else {
                    $("#LegLevel .configuredAttributeLegList li[configindex=" + index + "]").remove();
                    html = String.format(availableAttributeLegListItem,
                        index,
                        configData.LegLevel[index].DisplayName,
                        '');
                    $("#LegLevel .availableAttributeLegList").prepend(html).scrollTop(0);
                    initLegLevelListeners();
                }
            };

            showErrorDiv('Alert', 'alert_icon.png', "Do you want to delete event configuration for Leg: " + configData.LegLevel[index].DisplayName, callback);
            setConfigChange('delete', configData.LegLevel[index]);
        });
        //selecting a action
        $("#LegLevel .action").unbind('click').click(function (e) {
            var target = $(e.target).closest(".action");
            var eventId = getActionIdFor(target);
            if (isActionSelected(target)) {
                target.removeClass('selectedAction');
                configData.LegLevel[onDisplayLeg].SelectedAttributeLegActions =
                    configData.LegLevel[onDisplayLeg].SelectedAttributeLegActions.filter(function (item) {
                        return item != eventId;
                    });
                if (configData.LegLevel[onDisplayLeg].SelectedAttributeLegActions.length == 0) {
                    setDisplayLegConfigured(false);
                    setConfigChange('delete', configData.LegLevel[onDisplayLeg]);
                }
                else {
                    setDisplayLegConfigured(true);
                    setConfigChange('update', configData.LegLevel[onDisplayLeg]);
                }
                $("#LegLevel .selectAllDiv").removeClass("allSelected");
            } else {
                target.addClass('selectedAction');
                configData.LegLevel[onDisplayLeg].SelectedAttributeLegActions.push(eventId);
                if (configData.LegLevel[onDisplayLeg].SelectedAttributeLegActions.length == attributeLegLevelActions.length) {
                    $("#LegLevel .selectAllDiv").addClass("allSelected");
                }
                setConfigChange('update', configData.LegLevel[onDisplayLeg]);
                setDisplayLegConfigured(true);
            }
        });
        //clicking on selectAll
        $("#LegLevel .selectAllDiv").unbind('click').click(function (e) {
            var target = $(e.target).closest(".selectAllDiv");
            if (target.hasClass('allSelected')) {
                target.removeClass("allSelected");
                $("#LegLevel .action").removeClass('selectedAction');
                configData.LegLevel[onDisplayLeg].SelectedAttributeLegActions = [];
                setDisplayLegConfigured(false);
                setConfigChange('delete', configData.LegLevel[onDisplayLeg]);
            } else {
                target.addClass("allSelected");
                $("#LegLevel .action").addClass('selectedAction');
                configData.LegLevel[onDisplayLeg].SelectedAttributeLegActions = Array.from(attributeLegLevelActions);
                setDisplayLegConfigured(true);
                setConfigChange('update', configData.LegLevel[onDisplayLeg]);
            }

        });
        $("#LegLevel .availableAttributeSearch input").on("change paste keyup", function () {
            var text = this.value.trim().toLowerCase();
            var listItems = $("#LegLevel .availableAttributeLegList>li");
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
    function setDisplayLegConfigured(isConfiguredNow) {
        if (isDisplayLegConfigured == isConfiguredNow)
            return;
        var html;
        isDisplayLegConfigured = isConfiguredNow;
        if (isConfiguredNow) {
            $("#LegLevel .availableAttributeLegList li[configindex=" + onDisplayLeg + "]").remove();
            html = String.format(configuredAttributeLegListItem,
                onDisplayLeg,
                configData.LegLevel[onDisplayLeg].DisplayName,
                'selectedAttributeLeg');
            $("#LegLevel .configuredAttributeLegList").prepend(html).scrollTop(0);
        } else {
            $("#LegLevel .configuredAttributeLegList li[configindex=" + onDisplayLeg + "]").remove();
            html = String.format(availableAttributeLegListItem,
                onDisplayLeg,
                configData.LegLevel[onDisplayLeg].DisplayName,
                'selectedAttributeLeg');
            $("#LegLevel .availableAttributeLegList").prepend(html).scrollTop(0);
        }
        initLegLevelListeners();
    }

    function getActionIdFor(target) { return parseInt(target.attr('actionId')); }
    function isActionSelected(target) { return target.hasClass('selectedAction'); }
    function getConfigIndexOf(target) { return parseInt(target.attr('configIndex')); }

    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, false, false);
    }

    function getAllTransportNames() {

        CallCommonServiceMethod('getTransportNames', null, function (result) {
            transportNames = result.d;
            getInstrumentTypeEventConfig();
        }, null);
    }
    function getInstrumentTypeEventConfig() {
        var param = {};
        param.moduleId = instanceData.moduleId;
        param.instrumentTypeId = instanceData.instrumentTypeId;
        CallCommonServiceMethod("GetEventConfigForInstrumentType", param, onSuccessGetEventConfigForInstrumentType, onFailure);
    }

    function saveInstrumentTypeEventConfig() {

        if (!validateConfigUI())
            return;

        if (configData == null)
            return;
        var param = {};
        param.config = prepareSaveConfig();
        console.log(param.config);
        param.moduleId = instanceData.moduleId;
        param.instrumentTypeId = instanceData.instrumentTypeId;
        param.username = instanceData.username;
        CallCommonServiceMethod("SaveEventInstrumentTypesConfigList", param, onSuccessSaveEventConfigForInstrumentType, onFailureSaveEventConfigForInstrumentType);
    }

    function prepareSaveConfig() {
        var finalConfig = {};
        if (configData.delete || configData.update) {
            finalConfig.ConfigId = configData.ConfigId;
            finalConfig.SelectedInstrumentActions = configData.SelectedInstrumentActions;
            finalConfig.SelectedInstrumentQueue = configData.SelectedInstrumentQueue;
        } else {
            finalConfig.ConfigId = 0;
            finalConfig.SelectedInstrumentActions = [];
            finalConfig.SelectedInstrumentQueue = [];
        }
        finalConfig.AttributeLevel = configData.AttributeLevel.filter(function (item) {
            return item.ConfigId < 0 || item.delete || item.update;;
        });
        finalConfig.LegLevel = configData.LegLevel.filter(function (item) {
            return item.ConfigId < 0 || item.delete || item.update;
        });
        return finalConfig;
    }

    function isPendingChanges() {
        if (configData.delete || configData.update)
            return true;
        var test = configData.AttributeLevel.find(function (item) { return item.ConfigId < -1 || item.delete || item.update; });
        if (test != null && test != undefined)
            return true;
        test = configData.LegLevel.find(function (item) { return item.ConfigId < -1 || item.delete || item.update; });
        if (test != null && test != undefined)
            return true;
        return false;
    }

    function onSuccessGetEventConfigForInstrumentType(result) {
        validateConfigUI();
        result = result.d;
        if (result == null)
            return;
        configData = result;
        console.log(result);
        initInstrumentLevel();
        initAttributeLevel();
        initLegLevel();
    }
    function onSuccessSaveEventConfigForInstrumentType() {
        showErrorDiv('Success', 'pass_icon.png', "Saved Event configurations");
        getInstrumentTypeEventConfig();
    }

    function onFailureSaveEventConfigForInstrumentType() {
    }

    function onFailure() { }

    return instrumentEventConfig;
})();

function showErrorDiv(errorHeading, srcImageName, errorMessageText,callback) {
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