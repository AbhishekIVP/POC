$(function () {
    $.widget("iago-widget.contextMenu", {
        options: {
            className:'addSpaceSeperatedClassesHere',
            MenuItems: {},
            callback: function (a, b) {
                console.log(a);
                console.log(b);
            },
            triggerOn: "right", hoverWait: '200',
            contextOn: function (opt) {
                
            },
            contextOff: function (opt) {
                
            },
            autoHide: true

        },
        _create: function () {
            var self = this;
            self.options.identifier = this.element.attr("id");
            self.setJson(self.options);
//            iago.core.registerWidget({id:this.options.identifier , widget:"contextMenu"});
        },
        getJson: function () {
            return JSON.parse(ko.toJSON(this.viewModel));
        },
        _normalize: function (menuItems) {
            return menuItems;
        },

        setJson: function (optionData) {
            if (optionData === undefined) return;
            $.contextMenu(
                {
                    className:this.options.className,
                    selector: "#" + this.options.identifier,
                    callback: optionData.callback,
                    delay: optionData.hoverWait,
                    autoHide: optionData.autoHide,
                    items: this._normalize(optionData.MenuItems),
                    events: {
                        show: optionData.contextOn,
                        hide: optionData.contextOff
                    }
                }
            );
            },

         getCurrentOptionInfo: function () {
                /* TODO */
                return {};
            },

        _setOptions: function () {
            this._superApply(arguments);
        },

        _setOption: function (key, value) {
            this._super(key, value);
        },
        _destroy: function () {
            //this.element.html('');
            var that = this;
            $.contextMenu('destroy');
            iago.core.deRegisterWidget({ id: this.options.identifier });
        },

        _refresh: function () { this._trigger("change"); }
    });
    $.each($("div[data-type='iago:contextMenu']"), function (index, value) {
        if ($(value).attr('data-options-type') === "JSON")
            $(value).contextMenu(JSON.parse($(value).attr('data-options')));
        if ($(value).attr('data-options-type') === "Global")
            $(value).contextMenu(Global[$(value).attr('data-options')]);
    });
});