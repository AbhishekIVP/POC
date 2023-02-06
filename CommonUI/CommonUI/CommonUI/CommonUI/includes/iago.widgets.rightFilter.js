$(function () {
    $.widget("iago-widget.rightFilter", {
        options: {
            Filters: [
                {
                    "eletype": "div",
                    "id": "filterLeftMenu",
                    "type": "iago:leftMenu",
                    "option-type": "JSON",
                    "options": '{"MenuItems":[{ "MenuID": "1", "Order": "0", "Text": "Home", "Href": "", "Img": "fa-home", "Children": [{ "MenuID": "1", "Order": "0", "Text": "Home without Icon", "Href": "", "Img": "" }, { "MenuID": "2", "Order": "0", "Text": "Bull Horn", "Href": "", "Img": "fa-bullhorn" }, { "MenuID": "3", "Order": "0", "Text": "Camera", "Href": "/Joins.htm", "Img": "fa-camera" }, { "MenuID": "4", "Order": "0", "Text": "Check", "Href": "", "Img": "fa-check" }, { "MenuID": "5", "Order": "0", "Text": "Cutlery", "Href": "", "Img": "fa-cutlery" }, { "MenuID": "6", "Order": "0", "Text": "Users", "Href": "", "Img": "fa-users"}] }, { "MenuID": "2", "Order": "0", "Text": "Bull Horn", "Href": "", "Img": "fa-bullhorn" }, { "MenuID": "3", "Order": "0", "Text": "Camera", "Href": "", "Img": "fa-camera" }, { "MenuID": "4", "Order": "0", "Text": "Check", "Href": "", "Img": "fa-check" }, { "MenuID": "5", "Order": "0", "Text": "Cutlery", "Href": "", "Img": "fa-cutlery", "Children": [{ "MenuID": "2", "Order": "0", "Text": "Bull Horn", "Href": "", "Img": "fa-bullhorn" }, { "MenuID": "3", "Order": "0", "Text": "Camera", "Href": "", "Img": "fa-camera" }, { "MenuID": "4", "Order": "0", "Text": "Check", "Href": "", "Img": "fa-check" }, { "MenuID": "5", "Order": "0", "Text": "Cutlery", "Href": "", "Img": "fa-cutlery" }, { "MenuID": "6", "Order": "0", "Text": "Users", "Href": "", "Img": "fa-users", "Children": [{ "MenuID": "2", "Order": "0", "Text": "Bull Horn", "Href": "", "Img": "fa-bullhorn" }, { "MenuID": "3", "Order": "0", "Text": "Camera", "Href": "", "Img": "fa-camera", "Children": [{ "MenuID": "2", "Order": "0", "Text": "Bull Horn", "Href": "", "Img": "fa-bullhorn" }, { "MenuID": "3", "Order": "0", "Text": "Camera", "Href": "", "Img": "fa-camera" }, { "MenuID": "4", "Order": "0", "Text": "Check", "Href": "", "Img": "fa-check" }, { "MenuID": "5", "Order": "0", "Text": "Cutlery", "Href": "", "Img": "fa-cutlery" }, { "MenuID": "6", "Order": "0", "Text": "Users", "Href": "", "Img": "fa-users"}] }, { "MenuID": "4", "Order": "0", "Text": "Check", "Href": "", "Img": "fa-check" }, { "MenuID": "5", "Order": "0", "Text": "Cutlery", "Href": "", "Img": "fa-cutlery" }, { "MenuID": "6", "Order": "0", "Text": "Users", "Href": "", "Img": "fa-users"}] }] }, { "MenuID": "6", "Order": "0", "Text": "Users", "Href": "", "Img": "fa-users" } ]}'
                },
                {
                    "eletype": "div",
                    "id": "filterLeftMenuAnother",
                    "type": "iago:leftMenu",
                    "option-type": "JSON",
                    "options": '{"MenuItems" : [{ "MenuID": "1", "Order": "0", "Text": "Home", "Href": "", "Img": "fa-home"}]}'
                }
            ],
            customClass: '',
            getData: function (rightFilterInfo) {},
            filterDocked: function (isDocked) {},
            showOnHover: false
        },
        _render: function (ele, options) {
            // Put Rendering Code here(HTML)
            var filterDic = $("#" + options.identifier + "_info");
            $.map(options.Filters, function (item) {
                $("<div id='" + options.identifier + "_" + item["title"] + "Div' class='filterWidgetTitle'><div class='expand-filter'></div>" + item["title"] + "</div>").appendTo(filterDic);
                $("<" + item["eletype"] + " id='" + options.identifier + "_" + item["id"] + "' data-type='" + item["type"] + "' data-options-type='" + item["option-type"] + "' data-options='" + item["options"] + "' class='custom-div'>" + "</" + item["eletype"] + ">").appendTo(filterDic);
            });
            $.map(options.Filters, function (item) {
                $("#" + options.identifier + "_" + item["id"])[item["type"].replace("iago:", "")](((typeof item["options"] == 'string') ? JSON.parse(item["options"]) : item["options"]));
            });
            $('.filterWidgetTitle').unbind().click(function () {
                $(this).next('div').slideToggle();
                $(this).children(":first").toggleClass('collapsed-filter');
            });
        },
		
		 getContentBodyHeightForFullScreen: function () {
            var windowHeight = $(window).height();
            var contentBodyOffsetTop = $("#gridWriterContainer").offset().top;
            //var copyRightDivHeight = $("#CopyrightDiv").height();
            // return (windowHeight - contentBodyOffsetTop - copyRightDivHeight - 10);
			return (windowHeight - contentBodyOffsetTop);
        },
		
        _create: function () {
            var self = this;

            this.options.identifier = this.element.attr("id");
            this.options.currentState = 0;
            this.element.addClass('iago-rightFilter ' + this.options.customClass);
            var rightFilterHeight = 0;
            if (iago.utils != null)
                rightFilterHeight = iago.utils.getContentBodyHeightForFullScreen() - 20;
			else
			    rightFilterHeight = self.getContentBodyHeightForFullScreen();
            var rightFilterHeight = Math.floor(rightFilterHeight);
            var filterIcon = $("<div class='rightFilterDiv'><i class='fa fa-filter custom-filter'></i></div>").appendTo(this.element);
            var filterDic = $("<div id='" + this.options.identifier + "_info' class='iago-rightFilter-information'></div>").appendTo(this.element);
            $('<div class="filterHeader" style="margin: 3px;padding: 4px;overflow: auto;margin: 0px;"><span id="' + this.options.identifier + '_applyData" class="btn btn-xs btn-iago-green" style="float: right;margin: 0 auto;font-weight:bold;" title="Apply Filters"><i class="fa fa-check" ></i> Apply</span>Filters</div>').appendTo(filterDic);

            /* -->1 Leave as it is */
            self._render(this.element, this.options);

            $('.filterWidgetTitle').unbind().click(function () {
                $(this).next('div').slideToggle();
                $(this).children(":first").toggleClass('collapsed-filter');
            });

            $('.iago-rightFilter .rightFilterDiv').click(function () {
                /* if(!($(".iago-page-tile").hasClass('goHide')))
                 {
                     $(".vanilla .iago-rightFilter-information").toggleClass('iago-rightFilterShift');
                    
                 }*/
                $('body').toggleClass('pin-content-div');
                $("#" + self.options.identifier).toggleClass('toggle-filter-icon');
                $("#" + self.options.identifier + "_info").show('slide', {
                    direction: 'right'
                }, function () {
                    //$("#" + self.options.identifier + "_info").fadeTo(100, '0.9', function () {
                    $("#" + self.options.identifier + "_info").fadeTo(100, '1', function () {
                        self.options.currentState = 1;
                        if ($("#" + self.options.identifier).hasClass('toggle-filter-icon')) {
                            if (typeof (self.options.filterDocked) == "function") {
                                self.options.filterDocked($('body').hasClass('pin-content-div'));
                            }
                            return;
                        }

                        var masks = $(".select2-drop-mask");
                        if (masks.length > 0) {
                            var hideMe = true;
                            $.each(masks, function (i, elem) {
                                if ($(elem).css('display') === 'block') {
                                    hideMe = false;
                                }
                            });
                            if (!hideMe) {
                                if (typeof (self.options.filterDocked) == "function") {
                                    self.options.filterDocked($('body').hasClass('pin-content-div'));
                                }
                                return;
                            }
                        }
                        var e = window.event;
                        //$("#" + self.options.identifier + "_info").fadeTo(100, '0.2', function () {
                        $("#" + self.options.identifier + "_info").fadeTo(100, '1', function () {
                            $("#" + self.options.identifier + "_info").hide('slide', {
                                direction: 'right'
                            }, function () {
                                self.options.currentState = 0;
                                if (typeof (self.options.filterDocked) == "function") {
                                    self.options.filterDocked($('body').hasClass('pin-content-div'));
                                }
                            });
                        });
                    });
                });
            });

            $("#" + self.options.identifier + '_applyData').click(function () {

                var filterJson = self.getFiltersJSON();
                if (self.options.getData)
                    self.options.getData(filterJson);

                self.element.trigger("right-filter-action", filterJson);

            });
            if (self.options.showOnHover) {
                $("#" + self.options.identifier).hover(function () {
                    if (self.options.currentState !== 0) return;
                    if ($("#" + self.options.identifier).hasClass('toggle-filter-icon')) return;
                    var e = window.event;
                    $("#" + self.options.identifier + "_info").show('slide', {
                        direction: 'right'
                    }, function () {
                        //$("#" + self.options.identifier + "_info").fadeTo(100, '0.9', function () {
                        $("#" + self.options.identifier + "_info").fadeTo(100, '1', function () {
                            self.options.currentState = 1;
                        });
                    });
                }, function () {
                    if (self.options.currentState !== 1) return;
                    if ($("#" + self.options.identifier).hasClass('toggle-filter-icon')) return;

                    var masks = $(".select2-drop-mask");
                    if (masks.length > 0) {
                        var hideMe = true;
                        $.each(masks, function (i, elem) {
                            if ($(elem).css('display') === 'block') {
                                hideMe = false;
                            }
                        });
                        if (!hideMe) {
                            return;
                        }
                    }
                    var e = window.event;
                    //$("#" + self.options.identifier + "_info").fadeTo(100, '0.2', function () {
                    $("#" + self.options.identifier + "_info").fadeTo(100, '1', function () {
                        $("#" + self.options.identifier + "_info").hide('slide', {
                            direction: 'right'
                        }, function () {
                            self.options.currentState = 0;
                        });
                    });
                });
            }
            if (iago.core != null) {
                iago.core.registerWidget({
                    id: this.options.identifier,
                    widget: "rightFilter"
                });
            }
        },
        getCurrentOptionInfo: function () {
            /* TODO */
            return {};
        },
        getFiltersJSON: function () {
            var temp = {};
            var filters = this.options.Filters;
            for (var i = 0; i < filters.length; i++) {
                temp[filters[i]["id"]] = $("#" + this.options.identifier + "_" + filters[i]["id"])[filters[i]["type"].replace("iago:", "")]('getJson');
                temp[filters[i]["id"]]["SelectedValue"] = $("#" + this.options.identifier + "_" + filters[i]["id"])[filters[i]["type"].replace("iago:", "")]('getSelectedValue');
                temp[filters[i]["id"]]["SelectedText"] = $("#" + this.options.identifier + "_" + filters[i]["id"])[filters[i]["type"].replace("iago:", "")]('getSelectedText');
            }
            return temp;
        },
        setJson: function (optionData) {
            if (optionData === undefined) return;
            //this.viewModel.text(optionData.text);
        },

        getJson: function () {
            return this.options.Filters;
        },
        getViewModel: function () {
            return this.viewModel;
        },

        _setOptions: function () {
            this._superApply(arguments);
        },

        _setOption: function (key, value) {
            switch (key) {
            case "getData":
                this.options.getData = value;
                break;
            case "customClass":
                this.element.removeClass(this.options.customClass);
                this.options.customClass = value;
                this.element.removeClass(this.options.customClass);
                break;

            case "Filters":
                var tempThis = this;
                $.map(this.options.Filters, function (item) {
                    $("#" + tempThis.options.identifier + "_" + item["id"]).prev().remove();
                    $("#" + tempThis.options.identifier + "_" + item["id"])[item["type"].replace("iago:", "")]("destroy");
                    $("#" + tempThis.options.identifier + "_" + item["id"]).remove();
                });
                this.options.Filters = value;
                this._render(this.element, this.options);
                break;
            }
            this._super(key, value);
        },
        _destroy: function () {
            /* Destroy all child widgets */
            $.map(this.options.Filters, function (item) {
                $("#" + item["id"])[item["type"].replace("iago:", "")]("destroy");
            });

            /* Reset current Element to Default state */
            this.element
                .removeClass("iago-rightFilter toggle-filter-icon " + this.options.customClass)
                .attr('data-bind', '')
                .text("");
            if (iago.core != null) {
                iago.core.deRegisterWidget({
                    id: this.options.identifier
                });
            }
        }
    });
    $('[data-type="iago:rightFilter"]').each(function (index, value) {
        if ($(value).attr('data-options-type') === "JSON")
            $(value).rightFilter(JSON.parse($(value).attr('data-options')));
    });
});