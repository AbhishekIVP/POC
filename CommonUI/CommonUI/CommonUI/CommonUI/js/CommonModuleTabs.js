var CommonModuleTabs = (function () {

    function commonModuleTabs() {
        this.count = 0;
        this.itemSelected = null;
        this.callbackObj = {};
        this.data = null;
        this.currentSelected = null;
        this.modulesToBeDisplayed = 0;
        this.objectId = null;
        this.case = 0;
        this._modulesList = {};
    }

    var CommonModuleTabs = new commonModuleTabs();

    commonModuleTabs.prototype.init = function ($object) {
        var instanceId = "commonModuleTabs_" + CommonModuleTabs.count++;

        if ($object.id === undefined) {
            $object.id = instanceId;
        }

        if ($object.container === undefined) {
            $object.container = $("body");
        }

        if ($object.theme === undefined) {
            $object.theme = "leftTheme";
        }

        var selectedCount = 0;
        for (var item in $object.data) {
            if ($object.data[item].isSelected == true) {
                selectedCount++;
            }
        }

        if ($object.data !== undefined) {
            createHTML($object);
        }
        this.objectId = $object.id;
    }

    function createHTML($object) {
        CommonModuleTabs.data = $object.data;

        CommonModuleTabs.destroy($object.id);
        var HTML = "<div id='" + $object.id + "' class='cmt_Container_" + $object.theme + "'>";
        for (var item in $object.data) {
            var isSelectedClass = "", isSelectedBackgroundClass = "", isSelectedTextClass = "";
            if ($object.data[item].isSelected) {
                isSelectedClass = "cmt_Selected cmt_Selected_" + $object.theme;
                isSelectedBackgroundClass = "cmt_Selected_Background_" + $object.theme;
                isSelectedTextClass = "cmt_Selected_Text ";
                var currentValue = null;
                if ($object.data[item].value)
                    currentValue = $object.data[item].value; //currentValue = $object.data[item].value;
            }
            else {
                isSelectedClass = "cmt_NotSelected cmt_NotSelected_" + $object.theme;
                isSelectedBackgroundClass = "cmt_NotSelected_Background_" + $object.theme;
            }

            HTML += "<div class='cmt_ItemContainer cmt_ItemContainer_" + $object.theme + " " + isSelectedClass + "' cmt_module_id='" + $object.data[item].moduleId + "' cmt_value='" + $object.data[item].name.toLowerCase() + "' style='display:none'>";
            HTML += "<div class='cmt_ItemText cmt_ItemText_" + $object.theme + " " + isSelectedTextClass + "' >" + $object.data[item].name + "</div>";
            HTML += "<div class='cmt_Underline_" + $object.theme + " " + isSelectedBackgroundClass + "'></div>";
            HTML += "</div>";
            if ($object.data[item].value)
                CommonModuleTabs.callbackObj[$object.data[item].value.toString()] = $object.data[item].callback;
            else
                CommonModuleTabs.callbackObj[$object.data[item].name.toLowerCase()] = $object.data[item].callback;

        }
        HTML += "</div>";

        $object.container.append(HTML);
        if (typeof (CommonModuleTabs.callbackObj[currentValue]) != "undefined")
            CommonModuleTabs.callbackObj[currentValue]();
        else
            CommonModuleTabs.case = 1;
        attachHandlers($object);
    }

    function attachHandlers($object) {
        //Tab Div Click Handler
        $("#" + $object.id).unbind('click').bind('click', function (e) {
            e.stopPropagation();
            var currTarget = $(e.target);
            if (!currTarget.hasClass("cmt_ItemContainer_" + $object.theme)) {
                currTarget = currTarget.closest(".cmt_ItemContainer_" + $object.theme);
            }
            var currentText = currTarget.text();
            var currentValue = currTarget.attr('cmt_value');
            if (!currentValue)
                return false;

            var tabs = $("#" + $object.id).find(".cmt_ItemContainer_" + $object.theme);
            tabs.each(function (index, ele) {
                $(ele).removeClass("cmt_Selected_" + $object.theme).addClass("cmt_NotSelected_" + $object.theme);
                $(ele).removeClass("cmt_Selected").addClass("cmt_NotSelected");

                $(ele).find(".cmt_ItemText_" + $object.theme).removeClass("cmt_Selected_Text").addClass("cmt_NotSelected_Text");

                $(ele).find(".cmt_Underline_" + $object.theme).removeClass("cmt_Selected_Background_" + $object.theme).addClass("cmt_NotSelected_Background_" + $object.theme);
            });

            currTarget.removeClass("cmt_NotSelected_" + $object.theme).addClass("cmt_Selected_" + $object.theme);
            currTarget.removeClass("cmt_NotSelected").addClass("cmt_Selected");

            currTarget.find(".cmt_ItemText_" + $object.theme).removeClass("cmt_NotSelected_Text").addClass("cmt_Selected_Text");
            //            currTarget.find(".cmt_ItemText_" + $object.theme).removeClass("cmt_NotSelected").addClass("cmt_Selected");

            currTarget.find(".cmt_Underline_" + $object.theme).removeClass("cmt_NotSelected_Background_" + $object.theme).addClass("cmt_Selected_Background_" + $object.theme);

            //Execute the callback function
            //changed
            if (CommonModuleTabs.case == 1) {
                CommonModuleTabs.callbackObj[currentValue.toLowerCase()]();
                CommonModuleTabs.currentSelected = currentValue.toLowerCase();
            }
            else {
                CommonModuleTabs.callbackObj[currentValue]();
            }

        });
    }

    function attachHandlers2($object) {
        //Tab Div Click Handler
        $("#" + $object.id).unbind('click').bind('click', '.cmt_ItemText', function (e) {
            e.stopPropagation();
            var currTarget = $(e.target);
            currTarget = currTarget.closest(".cmt_ItemContainer_" + $object.theme);

            //if (currTarget.length == 0)
            //    return false;
            var currentValue = currTarget.attr('cmt_value');
            //TODO : do it in one go
            var tabs = $("#" + $object.id).find(".cmt_ItemContainer_" + $object.theme);
            tabs.each(function (index, ele) {
                $(ele).removeClass("cmt_Selected_" + $object.theme).addClass("cmt_NotSelected_" + $object.theme);
                $(ele).removeClass("cmt_Selected").addClass("cmt_NotSelected");

                $(ele).find(".cmt_ItemText_" + $object.theme).removeClass("cmt_Selected_Text").addClass("cmt_NotSelected_Text");

                $(ele).find(".cmt_Underline_" + $object.theme).removeClass("cmt_Selected_Background_" + $object.theme).addClass("cmt_NotSelected_Background_" + $object.theme);
            });

            currTarget.removeClass("cmt_NotSelected_" + $object.theme).addClass("cmt_Selected_" + $object.theme);
            currTarget.removeClass("cmt_NotSelected").addClass("cmt_Selected");

            currTarget.find(".cmt_ItemText_" + $object.theme).removeClass("cmt_NotSelected_Text").addClass("cmt_Selected_Text");
            //            currTarget.find(".cmt_ItemText_" + $object.theme).removeClass("cmt_NotSelected").addClass("cmt_Selected");

            currTarget.find(".cmt_Underline_" + $object.theme).removeClass("cmt_NotSelected_Background_" + $object.theme).addClass("cmt_Selected_Background_" + $object.theme);

            //Execute the callback function
            //changed
            if (CommonModuleTabs.case == 1) {
                CommonModuleTabs.callbackObj[currentValue.toLowerCase()]();
                CommonModuleTabs.currentSelected = currentValue.toLowerCase();
            }
            else {
                CommonModuleTabs.callbackObj[currentValue]();
            }

        });
    }

    commonModuleTabs.prototype.destroy = function (id) {
        $("#" + id).remove();
    }

    commonModuleTabs.prototype.getSelectedTab = function (id) {
        var response = {};
        var tabs = $("#" + id).find(".cmt_ItemContainer");
        tabs.each(function (index, ele) {
            if ($(ele).hasClass("cmt_Selected")) {
                response["name"] = $(ele).find(".cmt_ItemText").text();
                response["value"] = $(ele).attr("cmt_value");
            }
        });

        return response;
    }


    commonModuleTabs.prototype.SetSRMProductTabs = function (userName, pageId, specificModuleId, setCustomText, selectedModuleToDisplay) {
        getModulesBasedOnPrivilege(userName, pageId, specificModuleId, selectedModuleToDisplay);
        var mainObj = {
            setCallback: function (obj) {

                CommonModuleTabs.callbackObj[obj.key.toLowerCase()] = obj.value;

                var selectedModule = CommonModuleTabs.data.find(function (i) { if (i.name.toLowerCase() == obj.key) return i; });



                if (selectedModule) {

                    CommonModuleTabs.modulesToBeDisplayed++; //count of modules

                    if (CommonModuleTabs.modulesToBeDisplayed == 1 && selectedModule.name.toLowerCase() != "allsystems") {
                        CommonModuleTabs.data.splice(0, 1);
                        CommonModuleTabs._modulesList.splice(0, 1);
                    }
                    var defaultModuleSelected = CommonModuleTabs.data.find(function (i) { if (i.isSelected == true) return i; });
                    //var isdefaultModuleSelected;
                    if (typeof (defaultModuleSelected) == "undefined") {
                        //isdefaultModuleSelected = false;
                        if (typeof CommonModuleTabs.data != 'undefined' && CommonModuleTabs.data.length > 0)
                            CommonModuleTabs.data[0].isSelected = true;
                    }
                    //else {
                    //    isdefaultModuleSelected = true;
                    //}

                    $(".cmt_ItemContainer[cmt_value='" + selectedModule.name.toLowerCase() + "']").show();

                    let moduleName = "";
                    if (getRMLeftMenu()) {
                        if (getRMLeftMenu().currentModule) {
                            moduleName = getRMLeftMenu().currentModule.toLowerCase();
                        }
                    }

                    if (selectedModule.isSelected) {
                        $(".cmt_ItemContainer").find(".cmt_ItemText").removeClass("cmt_Selected_Text");
                        $(".cmt_ItemContainer[cmt_value='" + selectedModule.name.toLowerCase() + "']").find(".cmt_ItemText").addClass("cmt_Selected_Text");
                        CommonModuleTabs.currentSelected = obj.key.toLowerCase();
                        CommonModuleTabs.callbackObj[obj.key.toLowerCase()]();
                    }
                        //isselected is not sent from db, select the first module by default    
                    else if (!(moduleName == "partymaster" || moduleName == "entitymaster" || moduleName == "fundmaster") && !defaultModuleSelected) {
                        {
                            if (CommonModuleTabs.modulesToBeDisplayed == 1) {
                                CommonModuleTabs.currentSelected = obj.key.toLowerCase();
                                CommonModuleTabs.callbackObj[obj.key.toLowerCase()]();
                                $(".cmt_ItemContainer[cmt_value='" + selectedModule.name.toLowerCase() + "']").find(".cmt_ItemText").addClass("cmt_Selected_Text");
                            }
                        }
                    }
                }
                if (CommonModuleTabs.modulesToBeDisplayed > 1) {
                    if (CommonModuleTabs.objectId != null && $(".srm_panelTopSections").length > 0) {
                        $("#" + CommonModuleTabs.objectId).show();
                        $("[id$=srm_setTextDiv]").css('display', 'none');
                        $("[id$=srm_moduleTabs]").css('display', 'inline-block');
                        document.getElementsByClassName("srm_panelTopSections")[0].style.display = "Block";
                    }
                    else {
                        $("#" + CommonModuleTabs.objectId).show();
                    }
                }
                else {
                    if (CommonModuleTabs.objectId != null && $(".srm_panelTopSections").length > 0) {
                        $("#" + CommonModuleTabs.objectId).hide();
                        $("[id$=srm_moduleTabs]").css('display', 'none')
                        if (setCustomText != '') {
                            $("[id$=srm_setTextDiv]").text(setCustomText);
                            $("[id$=srm_setTextDiv]").css('display', 'inline-block');
                        }
                        //  document.getElementsByClassName("srm_panelTopSections")[0].style.display = "None";
                    }
                    else {
                        $("#" + CommonModuleTabs.objectId).hide();
                    }
                }

            },
            setAllCallback: function (objArr) {
                for (var item in objArr) {
                    CommonModuleTabs.callbackObj[objArr[item].key.toLowerCase()] = objArr[item].value;
                }
                item = 0;
                for (var item in objArr) {
                    var selectedModule = CommonModuleTabs.data.find(function (i) { if (i.name.toLowerCase() == objArr[item].key) return i; });
                    if (selectedModule.isSelected) {
                        CommonModuleTabs.callbackObj[objArr[item].key.toLowerCase()]();
                        break;
                    }
                }
            }

        };
        return mainObj;
    }

    commonModuleTabs.prototype.ModuleList = function () {
        return Object.assign({}, CommonModuleTabs._modulesList);
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


    var privilegeArr = [];
    //var _modulesList = {};
    var getModulesBasedOnPrivilege = function (userName, pageId, specificModuleId, selectedModuleToDisplay) {

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

        if (data != null) {
            CommonModuleTabs._modulesList = data.d;
            let corpActionIndex = CommonModuleTabs._modulesList.findIndex(x=>x.moduleId == 9)
            if (corpActionIndex >= 0) {
                let removedObject = CommonModuleTabs._modulesList.splice(corpActionIndex, 1);
                if (removedObject.length > 0)
                    CommonModuleTabs._modulesList.push(removedObject[0]);
            }
            //console.log(data.d);
            privilegeArr = [];

            //by default selects the first module available
            var firstIsSelected = true;

            //a module is selected
            if (parseInt(selectedModuleToDisplay) != 0) {
                firstIsSelected = false;
            }


            if (specificModuleId != "-1")
                CommonModuleTabs._modulesList = data.d.filter(function (k) {
                    if (k.moduleId == specificModuleId) return k
                });
            for (i in CommonModuleTabs._modulesList) {
                var item = CommonModuleTabs._modulesList[i];

                if (item.moduleId == parseInt(selectedModuleToDisplay))
                    firstIsSelected = true;
                var moduleObj = {
                    name: item.displayName, moduleId: item.moduleId, isSelected: firstIsSelected
                };
                firstIsSelected = false;
                privilegeArr.push(moduleObj);
            }

            var obj = {
                container: $("#srm_moduleTabs"),
                theme: "middleTheme",
                data: privilegeArr
            }
            //TODO Demobuild
            CommonModuleTabs.init(obj);

        }

    }

    commonModuleTabs.prototype.setCustomInit = function (nonPrivilegeModuleList) {
        $("#srm_moduleTabs").html("");
        for (i in nonPrivilegeModuleList) {
            var item = nonPrivilegeModuleList[i];
            var moduleObj = { name: item.displayName, moduleId: item.moduleId, isSelected: false };
            privilegeArr.push(moduleObj);
        }
        var obj = {
            container: $("#srm_moduleTabs"),
            theme: "middleTheme",
            data: privilegeArr
        }
        CommonModuleTabs.init(obj);

    }
    $(window).unbind('message').bind('message', function (e) {
        debugger;
        if (e.originalEvent.data == 'resize') {
            var tempVar = window.frames;
            if (tempVar != null && tempVar.length > 0) {
                //for (var item in tempVar) {
                try {
                    tempVar[0].postMessage("resize", "*");
                }
                catch (ee) {
                }
                //tempVar[item].contentWindow.postMessage("resize", "*");
                // }
            }
        }
    });
    return CommonModuleTabs;
})();


