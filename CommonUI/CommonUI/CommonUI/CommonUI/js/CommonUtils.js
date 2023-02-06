function CommonUtils() {
    this.createSMSelectDropDown = function (dropDownId, smdata, showSearch, width, callback, heading, onChangeHandler, selectedItems, isMulti, multiText, allText) {
        var obj = {};
        obj.container = dropDownId;
        obj.id = dropDownId.attr("id");
        if (heading !== null) {
            obj.heading = heading;
        }
        if (allText !== null || allText !== undefined) {
            obj.allText = allText;
        }
        obj.data = smdata;
        if (showSearch) {
            obj.showSearch = true;
        }
        if (isMulti) {
            obj.isMultiSelect = true;
            obj.text = multiText;
        }
        if (selectedItems !== undefined && selectedItems.length !== 0) {
            obj.selectedItems = selectedItems;
        }
        obj.isRunningText = false;
        obj.ready = function (e) {
            e.width(width + "px");
            if (typeof onChangeHandler === "function") {
                e.on('change', function (ee) {
                    onChangeHandler(ee);
                });
            }
        }
        smselect.create(obj);

        $("#smselect_" + dropDownId.attr("id")).find(".smselect").css('text-align', 'left');
        $("#smselect_" + dropDownId.attr("id")).find(".smselectanchorcontainer2").css('width', '86%');

        if (typeof callback === "function") {
            callback();
        }
    };
};