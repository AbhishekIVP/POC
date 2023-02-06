$(function () {
    $.widget("iago-widget.checkBox", {
        options: {
            CheckboxInfo: [{
                "Text": "checkbox1",
                "Value": "checkbox1",
                "Checked": false
            }, {
                "Text": "checkbox2",
                "Value": "checkbox2",
                "Checked": false
            }],
            SelectAll: true,
            RadioValue: "checkbox2",
            Layout: "Default",
            Type: "Checkbox",
            displayName: "Text",
            value: "Value",
            isSelected: "Checked",
            showSearch: true,
            searchBoxClass: "",
            searchBoxPlaceHolder: "Search to Filter",
            minItemForSearch: 8,
            maxHeight:'200px'
        },
        _create: function () {
            function PCheckboxViewModel(eleId) {
                var self = this;
                self.identifier = eleId;
                self.Layout = ''; //["Default", "BootstrapButton"];
                self.Type = ''; //["Checkbox", "Radio"];
                self.CheckboxInfo = ko.observableArray([]);
                self.RadioValue = ko.observable();
                self.SelectAll = ko.observable();
                self.SelectAllValue = ko.observable();
                self.allSelected = ko.observable(false);
                self.AllChecked = ko.computed({
                    read: function () {
                        var firstUnchecked = ko.utils.arrayFirst(self.CheckboxInfo(), function (item) {
                            return item.Checked() != true;
                        });
                        return firstUnchecked == null;
                    },
                    write: function (value) {
                        $search = $("#" + self.identifier).find('.checkBoxWidgetSearchBox');
                        ko.utils.arrayForEach(self.CheckboxInfo(), function (item) {

                            if ($search.length == 0 || ($search.length > 0 && (item.Text().toLowerCase().search($search.val().toLowerCase()) > -1) || $search.val().trim() == ""))
                                item.Checked(value);
                        });
                    }
                });
            }

            this.render = function (ele, options) {
                ele.addClass('checkBoxWidget');
                if (options.Layout.toLowerCase() === "default") {
                    options.SelectAll = true;
                    if (options.SelectAll === true && options.Type.toLowerCase() === "checkbox") {
                        $('<label class="selectAll" ><input type="checkbox" class="custom-Checkbox " data-bind="checked: AllChecked"></input><span class="custom-Checkbox">All</span></label>').appendTo(ele);
                        $('<br />').appendTo(ele);
                        $('#'+options.identifier+' .selectAll').show();
                    }
                    else if (options.SelectAll === true && options.Type.toLowerCase() === "radio") {
                        $('#'+options.identifier+' .selectAll').next('br').hide();
                        $('#'+options.identifier+' .selectAll').hide();
                    }
                    if (options.CheckboxInfo.length > options.minItemForSearch && options.showSearch == true) {
                        $("<input type='textbox' placeholder='" + options.searchBoxPlaceHolder + "' class='checkBoxWidgetSearchBox " + options.searchBoxClass + "' />").appendTo(ele);
                    }
                    var ListDiv = $('<div data-bind="foreach: CheckboxInfo" class="checkBoxWidgetContainer" style="max-height:' + options.maxHeight + ';">').appendTo(ele);
                    var indItem = '';
                    if (options.Type.toLowerCase() === "checkbox") {
                        indItem = '<label><input type="checkbox" data-bind="attr : {searchText:Text().toLowerCase()}, value:Value, checked: Checked" ></input>';
                    }
                    else if (options.Type.toLowerCase() === "radio") {
                        indItem = '<label><input type="radio" data-bind="attr : {searchText:Text().toLowerCase()}, checked: $root.RadioValue, value: Value" name = "' + options.identifier + '_radio" class="custom-Checkbox" ></input>';
                    }
                    $(indItem + '<span class="custom-Checkbox" data-bind="attr : {searchText:Text().toLowerCase()},text: Text"></span></label>').appendTo(ListDiv);
                    //$('<br />').appendTo(ListDiv);
                }
                else if (options.Layout.toLowerCase() === "bootstrapbutton") {
                    var BtnListDiv = $('<div class="btn-group " data-toggle="buttons" data-bind="foreach: CheckboxInfo" >').appendTo(ele);
                    var label = $('<label class="btn btn-default custom-btn" ></label>').appendTo(BtnListDiv);
                    if (options.Type.toLowerCase() === "checkbox") {
                        $('<input type="checkbox" data-bind="attr : {searchText:Text().toLowerCase()}, value: value, checked: isSelected"></input>').appendTo(label);
                    }
                    else if (options.Type.toLowerCase() === "radio") {
                        $('<input type="radio" data-bind="attr : {checked: isSelected, searchText:Text().toLowerCase()}, value: value" name="' + options.identifier + '_radio"></input>').appendTo(label);
                    }
                    $('<span data-bind="attr : {searchText:displayName().toLowerCase()},text: displayName"></span>').appendTo(label);
                    //$('<br />').appendTo(BtnListDiv);
                }
            }

            this.options.identifier = this.element.attr("id");
            this.viewModel = new PCheckboxViewModel(this.options.identifier);
            this.setJson(this.options);
            this.render(this.element, this.options);
            ko.applyBindings(this.viewModel, this.element[0]);
            $(".checkBoxWidgetSearchBox").unbind().keyup(function () {
                $(this).next().find('[searchText]').parent().addClass('goHide');
                $(this).next().find('[searchText*="' + $(this).val().toLowerCase() + '"]').parent().removeClass('goHide');
                if ($(this).val().trim() == "") {
                    $(this).next().find('.goHide').removeClass('goHide');
                }
            });
			if(iago.core != null)
				iago.core.registerWidget({ id: this.options.identifier, widget: "checkBox" });
        },
        getCurrentOptionInfo: function () {
            /* TODO */
            return {};
        },
        getJson: function () {
            return ko.toJS(this.viewModel);
        },
        getSelectedValue: function () {
            var initJSON = this.getJson();
            if (this.options.Type.toLowerCase() === "radio") {
                return initJSON.RadioValue;
            }
            else {
                var FinalData = '';
                $.map(initJSON.CheckboxInfo, function (item) {
                    if (item.Checked === true) {
                        FinalData = FinalData + item.Value + ",";
                    }
                });
                return FinalData.substr(0, FinalData.length - 1);
            }
        },
        getSelectedText: function () {
            var initJSON = this.getJson();
            if (this.options.Type.toLowerCase() === "radio") {
                return initJSON.RadioValue;
            }
            else {
                var FinalData = '';
                $.map(initJSON.CheckboxInfo, function (item) {
                    if (item.Checked === true) {
                        FinalData = FinalData + item.Text + ",";
                    }
                });
                return FinalData.substr(0, FinalData.length - 1);
            }
        },
        getViewModel: function () {
            return this.viewModel;
        },
        _Item: function (data, that) {
            this.Text = ko.observable(data[that.displayName]);
            this.Value = ko.observable(data[that.value]);
            this.Checked = ko.observable(data.Checked);
        },
        setJson: function (optionData) {
            var that = this;
            if (optionData === undefined) return;
            this.viewModel.CheckboxInfo($.map(optionData.CheckboxInfo, function (item) { return new that._Item(item, that.options); }));
            this.viewModel.RadioValue(optionData.RadioValue);
            this.viewModel.SelectAll(optionData.SelectAll);
        },


        _setOptions: function () {
            this._superApply(arguments);
        },

        _setOption: function (key, value) {
            var that = this;
            switch (key) {

                case "info":            //$(someid).checkBox('option','info',{'Type':'checkbox','SelectAll':true,'RadioValue':'sdcvdsvcsd','CheckboxInfo':[{'Text':'rfwsf','Value':'checkbox1','Checked':true},{'Text':'sdcvdsvcsd','Value':'checkbox2','Checked':false}]}) 
                    {
                        if (value.CheckboxInfo != undefined || value.CheckboxInfo != null)
                            this.viewModel.CheckboxInfo($.map(value.CheckboxInfo, function (item) { return new that._Item(item, that.options); }));

                        if (value.RadioValue != undefined || value.RadioValue != null)
                            this.viewModel.RadioValue = value.RadioValue;

                        if (value.SelectAll != undefined || value.SelectAll != null)
                            this.viewModel.SelectAll = value.SelectAll;

                        if (value.SelectAll === false && value.Type.toLowerCase() === "checkbox") {
                            $('#'+options.identifier+' .selectAll').next('br').hide();
                            $('#'+options.identifier+' .selectAll').hide();
                        }

                        if (value.SelectAll === true && value.Type.toLowerCase() === "checkbox") {
                            $('#'+options.identifier+' .selectAll').next('br').show();
                            $('#'+options.identifier+' .selectAll').show();
                        }
                        break;
                    }
                case "CheckboxInfo":
                    {       //$("#someid").checkBox('option','CheckboxInfo',{"CheckboxInfo":[{'Text':'checkbox1','Value':'checkbox1','Checked':true},{'Text':'checkbox2','Value':'checkbox2','Checked':false}]})
                        this.viewModel.CheckboxInfo($.map(value.CheckboxInfo, function (item) { return new that._Item(item, that.options); }));
                        break;
                    }
                case "SelectAll":
                    {                   //$(someid).checkBox('option','SelectAll',{"SelectAll":false})
                        this.viewModel.SelectAll = value.SelectAll;

                        if (value.SelectAll === false) {
                            $('#'+options.identifier+' .selectAll').next('br').hide();
                            $('#'+options.identifier+' .selectAll').hide();
                        }
                        if (value.SelectAll === true) {
                            $('#'+options.identifier+' .selectAll').next('br').show();
                            $('#'+options.identifier+' .selectAll').show();
                        }
                        break;
                    }
                case "RadioValue":
                    {
                        this.viewModel.RadioValue(value.RadioValue);
                        break;
                    }
            }

            this._super(key, value);
        },
        _destroy: function () {
            this.element.text('');
			if(iago.core != null)
				iago.core.deRegisterWidget({ id: this.options.identifier });
        }
    });
    $('[data-type="iago:checkBox"]').each(function (index, value) {
        if ($(value).attr('data-options-type') === "JSON")
            $(value).checkBox(JSON.parse($(value).attr('data-options')));
    });
});