var SMAutoComplete = (function () {
    function smAutoComplete() {
        this.counter = 0;
        this.selectedItem = null;
    }

    var SMAutoComplete = new smAutoComplete();

    //Properties
    //1.id
    //2.container
    //3.inputElement (jQuery Object)
    //4.data(Eg. [{ 'label': 'A', 'value':'a'}, { 'label': 'B', 'value':'b'}])
    smAutoComplete.prototype.init = function ($object) {
        var instanceId = "smAutoComplete_" + SMAutoComplete.counter++;

        if ($object.id === undefined) {
            $object.id = instanceId;
        }

        if ($object.container === undefined) {
            $object.container = $("body");
        }

        if ($object.data !== undefined && $object.inputElement !== undefined && $object.inputElement instanceof jQuery) {
            createHTML($object);
        }
    }

    function createHTML($object) {
        SMAutoComplete.destroy($object.id);
        var html = "<div id='" + $object.id + "' class='smAutoCompleteContainerDiv' isHidden='true' style='display:none;'>";
        html += "<div class='smAutoCompleteInner'>";
        for (var item in $object.data) {
            html += "<div sm_auto_val='" + $object.data[item].value + "' class='smAutoCompleteItem' >" + $object.data[item].label + "</div>";
        }
        html += "</div>";
        html += "</div>";
        $object.container.append(html);

        $("#" + $object.id).find(".smAutoCompleteInner").smslimscroll({ height: '200px', railVisible: true, alwaysVisible: true });
        attachHandlers($object);

        if ($object.hasOwnProperty('ready') && typeof $object.ready === 'function')
            $object.ready($("#" + $object.id));
    }

    var docHandler = function (e) {
        e.stopPropagation();
        if (!$(e.target).hasClass('smslimScrollDiv') && $(e.target).parents('.smslimScrollDiv').length === 0) {
            var selects = $('.smAutoCompleteContainerDiv');
            $.each(selects, function (ii, ee) {
                $(ee).hide();
                $(ee).attr('isHidden', 'true');
            });
        }
        else
            e.preventDefault();
    }

    function attachHandlers($object) {
        //Input Element Click Handler
        $object.inputElement.unbind('click').bind('click', function (e) {
            e.stopPropagation();
            if ($("#" + $object.id).attr('isHidden') === 'true') {
                var top = $object.inputElement.offset().top + $object.inputElement.height();
                var left = $object.inputElement.offset().left;
                $("#" + $object.id).css("top", top + "px");
                $("#" + $object.id).css("left", left + "px");
                $("#" + $object.id).show();
                $("#" + $object.id).attr('isHidden', 'false');
            }
            else {
                $("#" + $object.id).hide();
                $("#" + $object.id).attr('isHidden', 'true');
            }
        });

        //List Div Click Handler
        $("#" + $object.id).unbind('click').bind('click', function (e) {
            var currTarget = $(e.target);
            var currentText = currTarget.text();
            var currentValue = currTarget.attr('sm_auto_val');
            SMAutoComplete.selectedItem = currentValue;
            $object.inputElement.val(currentText);
            SMAutoComplete.closeItemsContainer($object.id)
        });

        //Input Element Type Event
        $object.inputElement.on('input', function (e) {
            var searchText = $(this).val();
            var itemList = $(".smAutoCompleteContainerDiv").find(".smAutoCompleteItem");

            for (var i = 0; i < itemList.length; i++) {
                var itemText = itemList[i].innerHTML;
                if (itemText.toLowerCase().indexOf(searchText.toLowerCase()) !== -1) {
                    itemList[i].style.display = "";
                }
                else {
                    itemList[i].style.display = "none";
                }
            }
        });

        //Document Click Handler
        $(document).unbind('click', docHandler).click(docHandler);
    }

    smAutoComplete.prototype.openItemsContainer = function (id) {
        $("#" + id).show();
        $("#" + id).attr('isHidden', 'false');
    }

    smAutoComplete.prototype.closeItemsContainer = function (id) {
        $("#" + id).hide();
        $("#" + id).attr('isHidden', 'true');
    }

    smAutoComplete.prototype.destroy = function (id) {
        $("#" + id).remove();
    }

    return SMAutoComplete;
})();
