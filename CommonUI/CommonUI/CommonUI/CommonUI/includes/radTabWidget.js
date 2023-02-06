$(function () {
    $.widget("iago-widget.toolKittabs", {
        options: {
            tabSchema: {
                tab: [
	                    {
	                        id: "t1",
	                        name: "t1",
	                        subtab: [{
	                            id: "t5",
	                            name: "t5",
	                            subtab: [
	                                    {
	                                        id: "t11",
	                                        name: "t11",
	                                        isDefault: true
	                                    },
	                                    {
	                                        id: "t12",
	                                        name: "t12"
	                                    }
	                            ]
	                        },
	                            {
	                                id: "t6",
	                                name: "t6"
	                            },
	                            {
	                                id: "t7",
	                                name: "t7",
	                                isDefault: true,
	                                subtab: [
	                                    {
	                                        id: "t13",
	                                        name: "t13",
	                                        isDefault: true
	                                    },
	                                    {
	                                        id: "t14",
	                                        name: "t14"
	                                    }
	                                ]
	                            }
	                        ]

	                    },
	                    {
	                        id: "t2",
	                        name: "t2"
	                    },
	                    {
	                        id: "t3",
	                        name: "PnL Summary",
	                        isDefault: true,
	                        subtab: [
	                            {
	                                id: "t8",
	                                name: "Portfolio",
	                                isDefault: true
	                            },
	                            {
	                                id: "t9",
	                                name: "Sector"
	                            },
	                            {
	                                id: "t10",
	                                name: "Analyst"
	                            }

	                        ]
	                    },
	                    {
	                        id: "t4",
	                        name: "t4"
	                    }



                ]
            },

            tabClickHandler: function (a, tabContentDiv) {
                tabContentDiv.html("<h1>" + a + "</h1>");
            },
            changeTabHandler: function (tabs) {

            },
            deleteTabs: false,
            deleteTabHandler: function (tabName) {

            },
            enableShadow: true,
            tabContentHolder: ""
        },
        _create: function () {

            var self = this;
            this.tabDivArray = new Array();

            function widgetViewModel() {
                var self = this;
                self.tabSchema = ko.observable();
            }

            var widgetId = this.element.attr("id");
            // var tabDivArray = new Array();

            var tabContentHolder = this.options.tabContentHolder;

            function render(element, options, tabDivArray, manageTabState) {
                var widgetIdentifier = widgetId;
                renderTab(options.tabSchema.tab, 0, undefined, tabDivArray);
                var $tabStrip = $(renderTabStrip(tabDivArray, options.enableShadow,options));
                $tabStrip.appendTo(element);
                var self = this;
                var tabOptions = options;
                //$tabStrip.find('ul li').unbind('click').click(function (event) {
                $tabStrip.unbind('click');

                $tabStrip.on('click', 'ul li', function (event) {
                    var $clickedElem = $(event.target);
                    if ($(event.target).get(0).tagName === 'A') {
                        if ($(event.target).get(0).tagName === 'A') {
                            $clickedElem = $clickedElem.parent();
                        }
                        if (tabOptions.isIagoDependent) {
                            if (iago.global != null && iago.core != null) {
                                iago.global.isTabClicked = true;
                                iago.core.changeLocation($(this).attr("data-link"));
                            }
                        }
                        
                        $("#" + widgetIdentifier).toolKittabs("setSelectedTab", $clickedElem.attr("id"), true);
                    }
                    else if ($(event.target).hasClass("editTabs")) {
                        if (tabOptions.isIagoDependent) {
                            if (iago.global != null)
                                iago.global.isTabClicked = true;
                        }
                        $("#" + widgetIdentifier).toolKittabs("editTab", event);
                    }
                    else if ($(event.target).hasClass("btnAddTab") || $(event.target).hasClass("btnCancelTab")) {
                        $("#" + widgetIdentifier).toolKittabs("doAction", event);
                    }
                    else if ($(event.target).hasClass("btnDeleteTab") || $(event.target).hasClass("btnCancelDelete")) {
                        if (tabOptions.isIagoDependent) {
                            if (iago.global != null)
                                iago.global.isTabClicked = true;
                        }
                        $("#" + widgetIdentifier).toolKittabs("deleteTab", event);
                    }
                    else if ($(event.target).hasClass("deleteTabs")) {
                        if (tabOptions.isIagoDependent) {
                            if (iago.global != null)
                                iago.global.isTabClicked = true;
                        }
                        $("#" + widgetIdentifier).toolKittabs("deleteTabPopUp", event);
                    }
                    return false;

                });
                //if(this.options.deleteTabs)
                //{
                $tabStrip.on('mouseover', 'ul li', function (event) {
                    $(event.target).closest('li').find(".deleteTabs").removeClass("hideTabs");
                    $(event.target).closest('li').find(".editTabs").removeClass("hideTabs");
                });
                $tabStrip.on('mouseout', 'ul li', function (event) {
                    $(event.target).closest('li').find(".deleteTabs").addClass("hideTabs");
                    $(event.target).closest('li').find(".editTabs").addClass("hideTabs");
                });
                //}

                manageTabState(null, widgetId, tabDivArray);
            }

            function renderTab(tabList, level, parentId, tabDivArray) {
                if (tabList === undefined || tabList.length === 0) return;

                var tabStrip = '<ul class="nav nav-tabs" id="' + parentId + '_subtab">';
                var loc = location.hash;
                var locWithoutTabPart = location.hash.split(new RegExp("\/tab"))[0];
                for (var i = 0; i < tabList.length; i++) {
                    var tab = tabList[i];

                    tabStrip = tabStrip + '<li ' + ((tab.isDefault === undefined || tab.isDefault === false) ? "" : "data-is-default=\"\"") + ((tab.isDefault === true) ? ((level > 0) ? ' class="subtab subactive" ' : ' class="active" ') : ((level > 0) ? ' class="subtab" ' : ' class="" ')) + ' data-level="' + level + '" id="' + tab.id + '" data-link="' + locWithoutTabPart + '/tab/' + tab.id + '" ' + ((tab.subtab === undefined || tab.subtab.length === 0) ? "" : " data-has-subtab=\"\"") + ' ><a>' + tab.name;

                    if (self.options.deleteTabs)
                        tabStrip = tabStrip + '</a><div class=\"tabActionsParent\"><div class=\"editTabs hideTabs fa fa-pencil\"></div><div class=\"deleteTabs hideTabs fa fa-times\"></div></div></li>';
                        //tabStrip = tabStrip + '</a><div class=\"deleteTabs hideTabs fa fa-times\"></div></li>';
                    else
                        tabStrip = tabStrip + '</a></li>';

                    renderTab(tab.subtab, level + 1, tab.id, tabDivArray);
                }
                tabStrip = tabStrip + '</ul>';

                if (tabDivArray[level] === undefined) tabDivArray[level] = "";
                tabDivArray[level] = tabDivArray[level] + tabStrip;
            }


            function renderTabStrip(tabDivArray, enableTabShadow,options) {                
                if (options.isIagoDependent)
                   var iagoHeader = $("#" +iago.pageHeaderSpaceId);


                var stripContainer = '<div class="tabbable-custom toolKitTabs nav-justified">';
                if (enableTabShadow)
                    var stripContainer = '<div class="tabbable-custom toolKitTabs nav-justified customTabShadow">';
                for (var i = 0; i < tabDivArray.length; i++) {
                    stripContainer = stripContainer + '<div class="tab-level" id="' + widgetId + '_div_level_' + i + '">' + tabDivArray[i] + "</div>";
                }
                stripContainer = stripContainer + "</div>";

                if (tabContentHolder === "" || tabContentHolder === null || tabContentHolder === undefined) {
                    if (iagoHeader.html().trim() === "") {
                        stripContainer = stripContainer + '<div id="' + widgetId + '_tab_content" class="tab-content" ></div>';
                    } else {
                        stripContainer = stripContainer + '<div id="' + widgetId + '_tab_content" style="margin-top: ' + (iagoHeader.height() + 10) + 'px;" class="tab-content" ></div>';
                    }
                } else {
                    $("#" + tabContentHolder).append('<div id="' + widgetId + '_tab_content"  class="tab-content" ></div>');
                }

                return stripContainer;
            }






            this.options.setJson = function (optionData) {
                if (optionData === undefined) return;
                viewModel.text(optionData.text);
            }
            render(this.element, this.options, this.tabDivArray, this.manageTabState);
            this.options.identifier = this.element.attr("id");
            this.viewModel = new widgetViewModel();
            //ko.applyBindings(this.viewModel, this.element[0]);
            this.element.find('ul').sortable({
                items: "li",
                stop: function (event, ui) {
                    self.changetabOrder(event, ui);
                }
            });

            /* iago.core.addRoute(location.hash+"/tab/:tabid",function() {
	               
            var tabId = this.params["tabid"];
            var $widgetDiv = $("#"+tabId).closest('[type="iago:tabs"]');
            $widgetDiv.tabs("setSelectedTab",tabId,true);
            });*/

            var finalActiveTabId = "";
            for (var i = this.tabDivArray.length - 1; i >= 0; i--) {
                var $tabStrip = $("#" + this.options.identifier + "_div_level_" + i + " ul");
                if ($tabStrip.length === 0) continue;

                if (i === 0) {
                    finalActiveTabId = $tabStrip.find("li.active").attr("id");
                    break;
                } else {
                    finalActiveTabId = $tabStrip.find("li.subactive").attr("id");
                    break;
                }
            }

            if (location.hash === $("#" + finalActiveTabId).attr("data-link")) {
                // if(this.options.isEnabled==true)
                // { 
                this.options.tabClickHandler(finalActiveTabId, $("#" + widgetId + "_tab_content"));
                //  }
            } else {
                var newLocHash = $("#" + finalActiveTabId).attr("data-link");
                if (this.options.isIagoDependent) {
                    if (iago.global != null && iago.core != null) {
                        iago.global.isTabClicked = true;
                        iago.core.pushInitParameterIntoSession(newLocHash, iago.core.getInitParameterFromSession(location.hash));
                        iago.core.changeLocation(newLocHash);
                    }

                }
                
                this.options.tabClickHandler(finalActiveTabId, $("#" + widgetId + "_tab_content"));
            }

            if (this.options.isIagoDependent)
                var $iagoHeader = $("#" + iago.pageHeaderSpaceId);
            //	        	if ($iagoHeader.html().trim() !== "")
            //	        	{
            //	        			$(widgetId+"_tab_content").css("margin-top",($iagoHeader.height()+10));
            //	        	}
            //	        	else
            //	        	{
            //	        			$(widgetId+"_tab_content").css("margin-top",5);	
            //	        	}

            //$("#"+finalActiveTabId).click();
            if (this.options.isIagoDependent) {
                if (iago.core != null) {
                    iago.core.registerWidget({
                        id: this.options.identifier,
                        widget: "toolKittabs"
                    });
                }
            }

            
        },

        changetabOrder: function (event, ui) {
            var self = this;
            self.options.tabSchema.tab = [];
            var element = self.element.find('li');
            for (var i = 0; i < element.length; i++) {
                self.options.tabSchema.tab.push({ id: $(element[i]).attr('id'), name: $(element[i]).find('a').html() });
            }
            self.options.changeTabHandler(self.options.tabSchema.tab);
        },

        getJson: function () {
            return ko.toJSON(viewModel);
        },

        getCurrentOptionInfo: function () {
            /* TODO */
            return {};
        },

        getViewModel: function () {
            return viewModel;
        },

        setJson: function (optionData) {
            if (optionData === undefined) return;
            this.viewModel.text(optionData.text);
        },

        deleteTab: function (event) {
            if ($(event.target).hasClass("btnDeleteTab")) {
                var tabName = $(event.target).closest('li').find(".deleteTabs").attr("tabName");//$(event.target).parent().prev().html();
                var self = this;
                var index = 0;
                var tabId = null;
                var nextTabId = "";
                for (var i = 0; i < self.options.tabSchema.tab.length; i++) {
                    if (self.options.tabSchema.tab[i].name == tabName) {
                        index = i;
                        tabId = self.options.tabSchema.tab[i].id;
                        break;
                    }
                }
                //if (index = self.options.tabSchema.tab.length - 1) {
                //    nextTabId = self.options.tabSchema.tab[0].id;
                //}
                //else {
                //    nextTabId = self.options.tabSchema.tab[index + 1].id;
                //}
                self.options.tabSchema.tab.splice(index, 1);
                nextTabId = self.options.tabSchema.tab[0].id;
                self.options.deleteTabHandler(tabId, tabName, true);
                $(event.target).closest("li").remove();
                $("#" + self.element.attr("id")).toolKittabs("setSelectedTab", nextTabId, true);
            }
            else if ($(event.target).hasClass("btnCancelDelete"))
                $(".tabDeletePopUp").remove();
        },

        deleteTabPopUp: function (event) {
            $(".iago-page-title").css({ 'overflow': '' });
            var tabName = $(event.target).parent().prev().html();
            $(event.target).attr('tabName', tabName);
            var div = $('<div>');
            div.addClass("tabDeletePopUp");
            div.append("<div class=\"tabNameLabelParent\"><div class='tabDeletetext'>Delete the tab</div></div>");
            div.append("<div class=\"parentbtnDeleteTab\"><div class='btnDeleteTab'>Ok</div><div class=\"btnCancelDelete\">Cancel</div></div>");
            $(event.target).closest('li').find(".tabActionsParent").append(div);
            $(event.target).closest('li').find(".inputTabName").val(tabName);
        },

        editTab: function (event) {
            $(".iago-page-title").css({ 'overflow': '' });
            var tabName = $(event.target).parent().prev().html();
            $(event.target).attr('tabName', tabName);
            var div = $('<div>');
            div.addClass("tabEditPopUp");
            div.append("<div class=\"tabNameLabelParent\"><div class='tabNameLabelName'>Tab Name</div><div class='tabNameInput'><input class='inputTabName'/></div><div id=\"tabNameValidation\" title = \"Tab Name Already Exists\" class=\"tabNameValidation hiddenClass\" >!</div></div>");
            div.append("<div class=\"parentbtnAddTab\"><div class='btnAddTab'>Ok</div><div class=\"btnCancelTab\">Cancel</div></div>");
            $(event.target).closest('li').find(".tabActionsParent").append(div);
            $(event.target).closest('li').find(".inputTabName").val(tabName);
        },

        doAction: function (event) {
            var self = this;
            if ($(event.target).hasClass("btnAddTab")) {
                var oldTabName = $(event.target).closest('li').find(".editTabs").attr("tabName");
                var tabName = $(event.target).closest('li').find(".inputTabName").val();
                var tabId = null;
                for (var i = 0; i < self.options.tabSchema.tab.length; i++) {
                    if (self.options.tabSchema.tab[i].name == oldTabName) {
                        index = i;
                        self.options.tabSchema.tab[i].name =
                        tabId = self.options.tabSchema.tab[i].id;
                        break;
                    }
                }
                $(event.target).closest('li').find('a').html(tabName);

                self.options.deleteTabHandler(tabId, tabName, false, oldTabName);
                $(".tabEditPopUp").remove();
            }
            else {
                $(".tabEditPopUp").remove();
            }
        },

        addNewTab: function (tab) {
            var loc = location.hash;
            var locWithoutTabPart = location.hash.split(new RegExp("\/tab"))[0];
            var level = 0;
            this.options.tabSchema.tab.push(tab);
            var ul = this.element.find('ul');
            var li = '';
            li = '<li ' + ((tab.isDefault === undefined || tab.isDefault === false) ? "" : "data-is-default=\"\"") + ((tab.isDefault === true) ? ((level > 0) ? ' class="subtab subactive" ' : ' class="active" ') : ((level > 0) ? ' class="subtab" ' : ' class="" ')) + ' data-level="' + level + '" id="' + tab.id + '" data-link="' + locWithoutTabPart + '/tab/' + tab.id + '" ' + ((tab.subtab === undefined || tab.subtab.length === 0) ? "" : " data-has-subtab=\"\"") + ' ><a>' + tab.name + '</a><div class=\"deleteTabs hideTabs fa fa-times\"></div></li>';;
            ul.append(li);
            if (this.options.isIagoDependent) {
                if (iago.global != null && iago.core != null) {
                    iago.global.isTabClicked = true;
                    iago.core.changeLocation($(this).attr("data-link"));
                }
            }
            
            $("#" + this.element.attr("id")).toolKittabs("setSelectedTab", tab.id, true);
        },

        setSelectedTab: function (selectedTabId, isTabHandlerCallRequired) {
            this.manageTabState($("#" + selectedTabId), this.options.identifier, this.tabDivArray);
            var $widgetDiv = $("#" + selectedTabId).closest('[type="iago:tabs"]');
            var finalActiveTabId = "";
            for (var i = this.tabDivArray.length - 1; i >= 0; i--) {
                var $tabStrip = $("#" + this.options.identifier + "_div_level_" + i + " ul:visible");
                if ($tabStrip.length === 0) continue;

                if (i === 0) {
                    finalActiveTabId = $tabStrip.find("li.active").attr("id");
                    break;
                } else {
                    finalActiveTabId = $tabStrip.find("li.subactive").attr("id");
                    break;
                }

            }



            if (isTabHandlerCallRequired === true)
                this.options.tabClickHandler(finalActiveTabId, $("#" + this.options.identifier + "_tab_content"));


            if (location.hash !== $("#" + finalActiveTabId).attr("data-link")) {
                if (this.options.isIagoDependent) {
                    if (iago.global != null && iago.core != null) {
                        iago.global.isTabClicked = true;
                        iago.core.changeLocation($("#" + finalActiveTabId).data("link"));
                    }
                }
                
            }
            if (this.options.isIagoDependent)
               var $iagoHeader = $("#" +iago.pageHeaderSpaceId);
            //	        	if ($iagoHeader.html().trim() !== "")
            //	        	{
            //	        			$widgetDiv.find(".tab-content").css("margin-top",($iagoHeader.height()+10));
            //	        	}
            //	        	else
            //	        	{
            //	        			$widgetDiv.find(".tab-content").css("margin-top",5);	
            //	        	}

        },

        manageTabState: function manageTabState(selectedTab, widgetId, tabDivArray) {
            if (widgetId === undefined)
                widgetId = this.element.attr("id");

            if (selectedTab == null) {
                //set selectedTab to level 0 default tab or first tab if no default present
                selectedTab = $("#" + widgetId + "_div_level_0 ul li[data-is-default]")[0];
                if (selectedTab === undefined) $("#" + widgetId + "_div_level_0 ul li")[0];
            }


            var $selectedTab = $(selectedTab);
            var selectedTabId = $(selectedTab).attr("id");
            var level = Number.parseInt($selectedTab.attr("data-level"));

            //loop over current level tabs to switch state
            $selectedTab.parent().find("li").each(function (i, elem) {


                var $elemTab = $(elem);
                if (($elemTab).attr("id") === $selectedTab.attr("id")) {
                    (level > 0) ? $elemTab.addClass("subtab subactive") : $elemTab.addClass("active");
                } else {
                    (level > 0) ? $elemTab.removeClass("subactive") : $elemTab.removeClass("active");
                }
            });

            // manage subtab divs
            if ($selectedTab.is("[data-has-subtab]")) {

                var subtabId = $selectedTab.attr("id") + "_subtab";
                var $subTabDiv = $("#" + widgetId + "_div_level_" + (level + 1));
                $subTabDiv.show();
                $subTabDiv.find("ul").each(function (i, elem) {
                    var $elem = $(elem);
                    if ($elem.attr("id") === subtabId) {
                        $elem.show();


                        manageTabState($elem.find("li[data-is-default]")[0], widgetId, tabDivArray);
                    } else
                        $elem.hide();
                });
            } else {
                //hide all level > current level
                for (var i = level + 1; i < tabDivArray.length; i++) {
                    $("#" + widgetId + "_div_level_" + i).hide();
                }

            }

        },

        _setOptions: function () {
            this._superApply(arguments);
        },

        _setOption: function (key, value) {
            this._super(key, value);
        },

        _destroy: function () {
            $("#" + this.element.attr("id") + "_tab_content").html('');
            $(".toolKitAddTab").remove();
            ko.cleanNode(this.element[0]);
            this.element.unbind();
            this.element.html('');
            if (this.options.isIagoDependent) {
                if (iago.core != null) {
                    iago.core.deRegisterWidget({
                        id: this.options.identifier
                    });
                }
            }
            
        }
    });
    $('[data-type="iago:toolKittabs"]').each(function (index, value) {
        if ($(value).attr('data-options-type') === "JSON")
            $(value).WidgetName(JSON.parse($(value).attr('data-options')));
    });
});