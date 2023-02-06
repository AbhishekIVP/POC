var smslidemenu = (function () {
    var smSlideMenu = function () {
        this.counter = 0;
    }

    var smslidemenu = new smSlideMenu();

    var dateFormat;

    //Slide Menu Properties
    //pivotElementId - id of the element on whose click the Slide Menu will slide
    //id - the 'id' of the slide menu
    //direction - the slide direction of the slide menu
    //data - data to give to the slide menu(it's and array of objects)
    //Eg of data: [{ "sectionHeader": "Security Types", listItems: typeList, "selectAllText": "All Security Types", "state": "up", "sectionType": "checkbox", selectAllItems: isAllAssetTypeSelected, sectionOpenCallback: objReconBreakGrid.fetchReconciledSectype }, { "sectionHeader": "Attributes", listItems: securityAttributes, "selectAllText": "All Attributes", "state": "up", "sectionType": "checkbox", selectAllItems: isAllAttributesSelected, sectionOpenCallback: objReconBreakGrid.fetchSectypeAttributes, isSearchable: true }
    //In each object
    //  -> sectionHeader - the title of each section that would appear in the slide menu
    //  -> listItems - an array of objects with 2 properties 'text','value' and 'isSelected(to select that item or not)' for each list item
    //      in case of date list the object contains
    //      * dateFilterType(range/single/null) - the type of date filter you want
    //      * dateType(between/from/priorTo/normal) - the type of date
    //      * rangeStartDate - value of the start date in case of range 'dateFilterType'
    //      * rangeEndDate - value of the end date in case of range 'dateFilterType'
    //      * singleDate - value of the date value in case of single 'dateFilterType'
    //      * text, value and isSelected as usual
    //  -> selectAllText - text of the first item in the list that would allow selecting all items on click and similarly deselect all items on click. If not specified then it would not be appended.
    //  -> state(up/down) - to show the list open or close by default
    //  -> sectionType(checkbox/radio) - what type of list to show in the slide menu checkbox or radio
    //  -> selectAllItems(true/false) - to select all items by default or not(would work only with checkbox list)
    //  -> sectionOpenCallback - a function that would executed each time the section is opened
    //  -> sectionCloseCallback - a function that would executed each time the section is closed
    //  -> isSearchable - to show seach on top of the section to allow searching from the list items 

    //Function that initialises the Slide Menu
    smSlideMenu.prototype.init = function ($object, applyCallback, saveCallback) {
        //Set the Id property of the Slide Menu Control
        if ($object.id === undefined) {
            $object.id = "smslidemenu_" + (smslidemenu.counter++);
        }
        else if ($object.id !== undefined && $object.id instanceof jQuery) {
            $object.id = $object.id.attr('id') + "_" + (smslidemenu.counter++);
        }

        if ($object.format)
            dateFormat = $object.format;
        else
            dateFormat = 'm/d/Y';

        //To Set the direction of the Slide Menu Control
        if ($object.slideDirection === undefined) {
            $object.slideDirection = "right";
        }

        //To Check if jQuery syntax is used
        if ($object.pivotElementId instanceof jQuery) {
            $object.pivotElementId = $object.pivotElementId.attr('id');
        }

        if ($object.data instanceof Array && $object.data.length > 0 && $object.pivotElementId !== undefined) {
            smslidemenu.create($object, applyCallback, saveCallback);
        }
        else {
            console.error("SMSlideMenu Object is of Invalid Type");
        }

    }

    //Function to attach event on the pivot element on which the Slide Menu appears
    smSlideMenu.prototype.createHandlers = function ($object) {
        $("#" + $object.pivotElementId).unbind('click').bind('click', function (e) {
            e.stopPropagation();
            if ($("#" + $object.id).attr("state") === "open") {
                $("#" + $object.id).attr("state", "close");
                var animateObj = {};
                animateObj[$object.slideDirection] = "-350px";
                $("#" + $object.id).animate(animateObj);
            }
            else {
                $("#" + $object.id).attr("state", "open");
                var top = $("#" + $object.pivotElementId).offset().top;
                $("#" + $object.id).css("top", top + 20);
                $("#" + $object.id).css("height", $(window).height() - (top + 20));
                var animateObj = {};
                animateObj[$object.slideDirection] = "0px";
                $("#" + $object.id).animate(animateObj);
            }
        });
    }

    //Create the Slide Menu HTML and attach events to it
    smSlideMenu.prototype.create = function ($object, applyCallback, saveCallback) {
        var menuHTML = "<div id='" + $object.id + "' class='smslidemenu_filterDiv' state='close' style='" + $object.slideDirection + " : -350px;'>";
        menuHTML += "</div>";
        $('body').append(menuHTML);

        smslidemenu.bindSlideMenu($object.data, $("#" + $object.id), applyCallback, saveCallback);
        smslidemenu.createHandlers($object);
    }

    //Function to Bind The Slide Menu for the First Time 
    smSlideMenu.prototype.bindSlideMenu = function (sectionItemArrOfObjects, parentDiv, applyCallback, saveCallback) {
        var parentDivID = parentDiv.attr("id");
        parentDiv.empty();
        var divHTML = "<div class='smslidemenu_filterTopHeadSection'><div class='smslidemenu_filterHeading'>Filters</div>";
        if (saveCallback !== undefined && typeof saveCallback === "function") {
            divHTML += "<button id='smSlideMenuSaveBtn' class='smslidemenu_btnGreen smslidemenu_saveBtn'><span>Save</span></button>";
        }
        divHTML += "<button id='smSlideMenuApplyBtn' class='smslidemenu_btnGreen smslidemenu_applyBtn'><span>Apply</span></button></div>";
        parentDiv.append(divHTML);
        $("#smSlideMenuApplyBtn").unbind('click').bind('click', function (e) {
            e.stopPropagation();
            if (typeof applyCallback !== "undefined") {
                applyCallback();
            }
        });

        if (saveCallback !== undefined && typeof saveCallback === "function") {
            $("#smSlideMenuSaveBtn").unbind('click').bind('click', function (e) {
                e.stopPropagation();
                saveCallback();
            });
        }

        for (var i = 0; i < sectionItemArrOfObjects.length; i++) {
            var sName = "";
            if (sectionItemArrOfObjects[i].sectionName !== undefined && sectionItemArrOfObjects[i].sectionName !== "") {
                sName = sectionItemArrOfObjects[i].sectionName;
            }
            else {
                sName = sectionItemArrOfObjects[i].sectionHeader;
            }

            var rightMenuSectionID = ("rightMenuSection_" + parentDivID + "_" + i);
            var html = "";
            html += "<div id='" + rightMenuSectionID + "' identifier='" + sectionItemArrOfObjects[i].identity + "' class='filterSection' section_name='" + sName + "' section_type='" + sectionItemArrOfObjects[i].sectionType + "' >";
            if (sectionItemArrOfObjects[i].state === "down") {
                html += "<div class='smslidemenu_filterHeader'>" + sectionItemArrOfObjects[i].sectionHeader + "<span class='smslidemenu_itemListArrowIcon'><i class='fa fa-angle-down'></i></span></div>";
            }
            else if (sectionItemArrOfObjects[i].state === "up") {
                html += "<div class='smslidemenu_filterHeader'>" + sectionItemArrOfObjects[i].sectionHeader + "<span class='smslidemenu_itemListArrowIcon'><i class='fa fa-angle-right'></i></span></div>";
            }



            var dateIds = [];
            var itemLisID = ("itemList_" + parentDivID + "_" + i);
            var searchInputID = ("searchInput_" + parentDivID + "_" + i);
            if (sectionItemArrOfObjects[i].isSearchable === true || sectionItemArrOfObjects[i].isSearchable === "true") {
                html += "<div id='" + itemLisID + "' class='smslidemenu_itemListDiv' style='margin-top:25px;'>";
                html += "<i class='fa fa-search smslidemenu_searchIcon'></i><input type='text' id='" + searchInputID + "' placeholder='Search' class='smslidemenu_searchInput' />"
            }
            else {
                html += "<div id='" + itemLisID + "' class='smslidemenu_itemListDiv'>";
            }

            if (sectionItemArrOfObjects[i].selectAllText !== undefined) {
                sectionItemArrOfObjects[i].listItems.unshift({ "text": sectionItemArrOfObjects[i].selectAllText, "value": "-1", "isSelected": "false" })
            }

            if (sectionItemArrOfObjects[i].sectionType === "customHTML") {
                html += "<div class='smslidemenu_customhtmlstyle'>" + sectionItemArrOfObjects[i].customHTML + "</div>";
            }

            if (sectionItemArrOfObjects[i].listItems !== undefined && sectionItemArrOfObjects[i].listItems.length > 0) {
                $.each(sectionItemArrOfObjects[i].listItems, function (ii, ee) {
                    if (sectionItemArrOfObjects[i].selectAllText !== undefined && ee.text === sectionItemArrOfObjects[i].selectAllText && ii === 0) {
                        html += "<div class='smslidemenu_filterItem filterSelectAll' isselected='false'>";
                        html += "<div class='smslidemenu_filterItemCheckbox'>";
                        html += "<i class='fa fa-check-circle'></i>";
                        html += "</div>";
                        html += "<div class='smslidemenu_filterItemText' value='" + ee.value + "'>";
                        html += ee.text;
                        html += "</div>";
                        html += "</div>";
                    }
                    else if (sectionItemArrOfObjects[i].listItems.length === sectionItemArrOfObjects[i].listItems.filter(function (item) { if (item.isSelected === "true") { return item; } }).length && ii === 0) {
                        //If all items are selected in the list
                        html += "<div class='smslidemenu_filterItem filterSelectAll' isselected='false'>";
                        html += "<div class='smslidemenu_filterItemCheckbox'>";
                        html += "<i class='fa fa-check-circle'></i>";
                        html += "</div>";
                        html += "<div class='smslidemenu_filterItemText' value='" + ee.value + "'>";
                        html += ee.text;
                        html += "</div>";
                        html += "</div>";
                    }
                    else {
                        if (ee.isSelected !== undefined && ee.isSelected === "true") {
                            html += "<div class='smslidemenu_filterItem filterOptions' isselected='true'>";
                            html += "<div class='smslidemenu_filterItemCheckbox smslidemenu_selectedItem'>";
                            html += "<i class='fa fa-check-circle'></i>";
                            html += "</div>";
                        }
                        else {
                            html += "<div class='smslidemenu_filterItem filterOptions' isselected='false'>";
                            html += "<div class='smslidemenu_filterItemCheckbox'>";
                            html += "<i class='fa fa-check-circle'></i>";
                            html += "</div>";
                        }

                        if (ee.dateFilterType === "range") {
                            html += "<div class='smslidemenu_filterItemText' value='" + ee.value + "' range_start_date = '" + ee.rangeStartDate + "' range_end_date = '" + ee.rangeEndDate + "' date_type='" + ee.dateType + "'>";
                            html += ee.text;
                            html += " <input size='11' type='text' date_type='range_start_date' class='smslidemenu_filterDate' id='range_date_" + i + "_instance_" + ii + "a' value='" + ee.rangeStartDate + "'/> - <input type='text' date_type='range_end_date' size='11' class='smslidemenu_filterDate' id='range_date_" + i + "_instance_" + ii + "b' value='" + ee.rangeEndDate + "'/>";
                            html += "</div>";

                            dateIds.push("range_date_" + i + "_instance_" + ii + "a");
                            dateIds.push("range_date_" + i + "_instance_" + ii + "b");
                        }
                        else if (ee.dateFilterType === "single") {
                            html += "<div class='smslidemenu_filterItemText' value='" + ee.value + "' single_date = '" + ee.singleDate + "' date_type='" + ee.dateType + "'>";
                            html += ee.text;
                            html += " <input size='11' date_type='single_date' class='smslidemenu_filterDate' type='text' id='single_date_" + i + "_instance_" + ii + "a' value='" + ee.singleDate + "'/>";
                            html += "</div>";
                            dateIds.push("single_date_" + i + "_instance_" + ii + "a");
                        }
                        else {
                            html += "<div class='smslidemenu_filterItemText' value='" + ee.value + "' >";
                            html += ee.text;
                            html += "</div>";
                        }
                        html += "</div>";
                    }
                });
            }
            html += "</div>";
            html += "</div>";

            parentDiv.append(html);

            for (var k = 0; k < dateIds.length; k++) {
                CallCustomDatePicker(dateIds[k], dateFormat, null, optionDateTime.DATE, 15, false);
            }

            var mHeight = "200px";
            if (sectionItemArrOfObjects[i].maxHeight !== undefined) {
                mHeight = sectionItemArrOfObjects[i].maxHeight + "px";
            }

            $('#' + itemLisID).smslimscroll({
                height: mHeight,
                wheelStep: 1
            });

            if (sectionItemArrOfObjects[i].state === "up") {
                $('#' + itemLisID).parent(".smslimScrollDiv").css('display', 'none');
            }
            else if (sectionItemArrOfObjects[i].state === "down") {
                $('#' + itemLisID).parent(".smslimScrollDiv").css('display', '');
            }

            //Function to perform Search on a particular section
            if (sectionItemArrOfObjects[i].isSearchable === true || sectionItemArrOfObjects[i].isSearchable === "true") {
                $('#' + searchInputID).unbind().on('input', function (e) {
                    var searchText = $(this).val();
                    //var itemList = $(".filterOptions");
                    var itemList = $(this).parent().parent().find(".filterOptions");

                    for (var i = 0; i < itemList.length; i++) {
                        var itemText = itemList[i].innerText;
                        if (itemText.toLowerCase().indexOf(searchText.toLowerCase()) !== -1) {
                            itemList[i].style.display = "";
                        }
                        else {
                            itemList[i].style.display = "none";
                        }
                    }
                });
            }

            $('#' + rightMenuSectionID).find(".smslidemenu_filterHeader").unbind('click').bind('click', function (e) {
                e.stopPropagation();
                $(this).parent().siblings(".filterSection").find(".smslimScrollDiv").slideUp(function () {
                    var uArr = $(this).parent(".filterSection").attr('id').split('_');
                    var dNo = uArr[uArr.length - 1];
                    var closeCall = sectionItemArrOfObjects[dNo].sectionCloseCallback;
                    if ($(this).is(":hidden")) {
                        $(this).siblings(".smslidemenu_filterHeader").find("i").removeClass("fa fa-angle-down").addClass("fa fa-angle-right");
                        if (closeCall !== undefined) {
                            closeCall();
                        }
                    }
                });

                var uArr = $(e.target).parent(".filterSection").attr('id').split('_');
                var divNo = uArr[uArr.length - 1];
                var iListID = ("itemList_" + parentDivID + "_" + divNo);
                $("#" + iListID).parent(".smslimScrollDiv").slideToggle(function () {
                    var openCall = sectionItemArrOfObjects[divNo].sectionOpenCallback;
                    var closeCall = sectionItemArrOfObjects[divNo].sectionCloseCallback;
                    if ($(this).is(":hidden")) {
                        $(this).siblings(".smslidemenu_filterHeader").find("i").removeClass("fa fa-angle-down").addClass("fa fa-angle-right");
                        if (closeCall !== undefined) {
                            closeCall();
                        }
                    }
                    else {
                        $(this).siblings(".smslidemenu_filterHeader").find("i").removeClass("fa fa-angle-right").addClass("fa fa-angle-down");
                        if (openCall !== undefined) {
                            openCall();
                        }
                    }
                });
            });

            if (sectionItemArrOfObjects[i].sectionType === "radio") {
                $('#' + rightMenuSectionID).find(".smslidemenu_filterItem").unbind('click').bind('click', function (e) {
                    e.stopPropagation();
                    var target = $(e.target);
                    if (!target.hasClass('smslidemenu_filterItem')) {
                        target = $(e.target).parents(".smslidemenu_filterItem");
                    }

                    if (e.target.tagName === "INPUT") {
                        return;
                    }

                    target.parent().find(".smslidemenu_filterItemCheckbox").removeClass("smslidemenu_selectedItem");
                    target.parent().find(".smslidemenu_filterItem").attr('isselected', 'false');
                    if (target.attr('isselected') === "false") {
                        target.children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
                        target.attr('isselected', 'true');
                    }
                    else {
                        target.children(".smslidemenu_filterItemCheckbox").removeClass("smslidemenu_selectedItem");
                        target.attr('isselected', 'false');
                    }
                });
            }
            else {
                $('#' + rightMenuSectionID).find(".smslidemenu_filterItem").unbind('click').bind('click', function (e) {
                    e.stopPropagation();
                    var target = $(e.target);
                    if (!target.hasClass('smslidemenu_filterItem')) {
                        target = $(e.target).parents(".smslidemenu_filterItem");
                    }

                    if (e.target.tagName === "INPUT") {
                        return;
                    }

                    if (target.attr('isselected') === "false") {
                        target.children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
                        target.attr('isselected', 'true');
                    }
                    else {
                        target.children(".smslidemenu_filterItemCheckbox").removeClass("smslidemenu_selectedItem");
                        target.attr('isselected', 'false');
                    }

                    if (target.parent().find('.filterSelectAll').length !== 0) {
                        var allChildrens = target.parent().children('div').length - 1;
                        var selectedChildrens = target.parent().children('div[isSelected="true"]').not('.filterSelectAll').length;
                        if (allChildrens === selectedChildrens) {
                            target.parent().children('div').first().children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
                            target.parent().children('div').first().attr('isselected', 'true');
                        }
                        else {
                            target.parent().children('div').first().children(".smslidemenu_filterItemCheckbox").removeClass("smslidemenu_selectedItem");
                            target.parent().children('div').first().attr('isselected', 'false');
                        }
                    }
                });
            }

            if (sectionItemArrOfObjects[i].selectAllText !== undefined) {
                $('#' + rightMenuSectionID).find(".filterSelectAll").unbind('click').bind('click', function (e) {
                    e.stopPropagation();
                    var target = $(e.target);
                    if (!target.hasClass('filterSelectAll')) {
                        target = $(e.target).parents(".filterSelectAll");
                    }

                    if (target.attr('isselected') === "false") {
                        target.children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
                        target.attr('isselected', 'true');

                        target.siblings(".smslidemenu_filterItem").children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
                        target.siblings(".smslidemenu_filterItem").attr('isselected', 'true');
                    }
                    else {
                        target.children(".smslidemenu_filterItemCheckbox").removeClass("smslidemenu_selectedItem");
                        target.attr('isselected', 'false');

                        target.siblings(".smslidemenu_filterItem").children(".smslidemenu_filterItemCheckbox").removeClass("smslidemenu_selectedItem");
                        target.siblings(".smslidemenu_filterItem").attr('isselected', 'false');
                    }
                });
            }

            if ((sectionItemArrOfObjects[i].listItems !== undefined && (sectionItemArrOfObjects[i].listItems.length - 1) === sectionItemArrOfObjects[i].listItems.filter(function (item) { if (item.isSelected === "true") { return item; } }).length)) {
                var selectAllEle = $('#' + rightMenuSectionID).find(".filterSelectAll");
                selectAllEle.children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
                selectAllEle.attr('isselected', 'true');

            }

            if (sectionItemArrOfObjects[i].selectAllItems === true || sectionItemArrOfObjects[i].selectAllItems === "true") {
                var selectAllEle = $('#' + rightMenuSectionID).find(".filterSelectAll");
                if (selectAllEle.length !== 0 && (selectAllEle.attr("isSelected") !== "true")) {
                    selectAllEle.click();
                }
                else {
                    var target = $('#' + rightMenuSectionID).find(".smslidemenu_filterItem");
                    target.children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
                    target.attr('isselected', 'true');
                }

            }

            if (sectionItemArrOfObjects[i].selectAllText !== undefined) {
                sectionItemArrOfObjects[i].listItems.splice(0, 1);
            }
        }
    }

    //Function to Bind Any Section of the Slide Menu later after the Slide Menu has been attached to the DOM
    smSlideMenu.prototype.changeRightMenuContent = function (contentArray, rightMenuSection, selectAllText, state, sectionType, selectAllItems, isSearchable, bindComplete, maxHeight, parentDivID) {
        rightMenuSection.find(".smslidemenu_itemListDiv").empty();
        var dateIds = [];
        var html = '';
        var underscoreArr = rightMenuSection.attr('id').split('_');
        var sectionNumber = rightMenuSection.attr('id').split('_')[underscoreArr.length - 1];
        var searchInputID = ("searchInput_" + parentDivID + "_" + sectionNumber);
        var itemListID = ("itemList_smslidemenu_" + parentDivID + "_" + sectionNumber);

        if (isSearchable === true || isSearchable === "true") {
            rightMenuSection.find(".smslidemenu_itemListDiv").css('margin-top', '25px');
            html += "<i class='fa fa-search smslidemenu_searchIcon'></i><input type='text' id='" + searchInputID + "' placeholder='Search' class='smslidemenu_searchInput' />"
        }

        if (selectAllText !== undefined) {
            contentArray.unshift({ "text": selectAllText, "value": "-1", "isSelected": "false" })
        }
        $.each(contentArray, function (ii, ee) {
            if (selectAllText !== undefined && ee.text === selectAllText && ii === 0) {
                html += "<div class='smslidemenu_filterItem filterSelectAll' isselected='false'>";
                html += "<div class='smslidemenu_filterItemCheckbox'>";
                html += "<i class='fa fa-check-circle'></i>";
                html += "</div>";
                html += "<div class='smslidemenu_filterItemText' value='" + ee.value + "'>";
                html += ee.text;
                html += "</div>";
                html += "</div>";
            }
            else if (contentArray.length === contentArray.filter(function (item) { if (item.isSelected === "true") { return item; } }).length && ii === 0) {
                //If all items are selected in the list
                html += "<div class='smslidemenu_filterItem filterSelectAll' isselected='false'>";
                html += "<div class='smslidemenu_filterItemCheckbox'>";
                html += "<i class='fa fa-check-circle'></i>";
                html += "</div>";
                html += "<div class='smslidemenu_filterItemText' value='" + ee.value + "'>";
                html += ee.text;
                html += "</div>";
                html += "</div>";
            }
            else {
                if (ee.isSelected !== undefined && ee.isSelected === "true") {
                    html += "<div class='smslidemenu_filterItem filterOptions' isselected='true'>";
                    html += "<div class='smslidemenu_filterItemCheckbox smslidemenu_selectedItem'>";
                    html += "<i class='fa fa-check-circle'></i>";
                    html += "</div>";
                }
                else {
                    html += "<div class='smslidemenu_filterItem filterOptions' isselected='false'>";
                    html += "<div class='smslidemenu_filterItemCheckbox'>";
                    html += "<i class='fa fa-check-circle'></i>";
                    html += "</div>";
                }

                if (ee.dateFilterType === "range") {
                    html += "<div class='smslidemenu_filterItemText' value='" + ee.value + "' range_start_date = '" + ee.rangeStartDate + "' range_end_date = '" + ee.rangeEndDate + "' date_type='" + ee.dateType + "'>";
                    html += ee.text;
                    html += " <input size='11' type='text' date_type='range_start_date' class='smslidemenu_filterDate' id='range_date_" + i + "_instance_" + ii + "a' value='" + ee.rangeStartDate + "'/> - <input type='text' date_type='range_end_date' size='11' class='smslidemenu_filterDate' id='range_date_" + i + "_instance_" + ii + "b' value='" + ee.rangeEndDate + "'/>";
                    html += "</div>";

                    dateIds.push("range_date_" + i + "_instance_" + ii + "a");
                    dateIds.push("range_date_" + i + "_instance_" + ii + "b");
                }
                else if (ee.dateFilterType === "single") {
                    html += "<div class='smslidemenu_filterItemText' value='" + ee.value + "' single_date = '" + ee.singleDate + "' date_type='" + ee.dateType + "'>";
                    html += ee.text;
                    html += " <input size='11' date_type='single_date' class='smslidemenu_filterDate' type='text' id='single_date_" + i + "_instance_" + ii + "a' value='" + ee.singleDate + "'/>";
                    html += "</div>";
                    dateIds.push("single_date_" + i + "_instance_" + ii + "a");
                }
                else {
                    html += "<div class='smslidemenu_filterItemText' value='" + ee.value + "' >";
                    html += ee.text;
                    html += "</div>";
                }
                html += "</div>";
            }
        });
        rightMenuSection.find(".smslidemenu_itemListDiv").append(html);

        for (var k = 0; k < dateIds.length; k++) {
            CallCustomDatePicker(dateIds[k], dateFormat, null, optionDateTime.DATE, 15, false);
        }

        var mHeight = "200px";
        if (maxHeight !== undefined && maxHeight !== null) {
            mHeight = sectionItemArrOfObjects[i].maxHeight + "px";
        }

        $('#' + itemListID).smslimscroll({
            height: mHeight,
            wheelStep: 1
        });

        //Function to perform Search on a particular section
        if (isSearchable === true || isSearchable === "true") {
            $('#' + searchInputID).unbind().on('input', function (e) {
                var searchText = $(this).val();
                //var itemList = $(".filterOptions");
                var itemList = $(this).parent().parent().find(".filterOptions");

                for (var i = 0; i < itemList.length; i++) {
                    var itemText = itemList[i].innerText;
                    if (itemText.toLowerCase().indexOf(searchText.toLowerCase()) !== -1) {
                        itemList[i].style.display = "";
                    }
                    else {
                        itemList[i].style.display = "none";
                    }
                }
            });
        }

        if (state === "up") {
            $('#' + itemListID).parent(".smslimScrollDiv").css('display', 'none');
        }
        else if (state === "down") {
            $('#' + itemListID).parent(".smslimScrollDiv").css('display', '');
        }

        if (sectionType === "radio") {
            $('#' + itemListID).find(".smslidemenu_filterItem").unbind('click').bind('click', function (e) {
                e.stopPropagation();
                var target = $(e.target);
                if (!target.hasClass('smslidemenu_filterItem')) {
                    target = $(e.target).parents(".smslidemenu_filterItem");
                }

                if (e.target.tagName === "INPUT") {
                    return;
                }

                target.parent().find(".smslidemenu_filterItemCheckbox").removeClass("smslidemenu_selectedItem");
                target.parent().find(".smslidemenu_filterItem").attr('isselected', 'false');
                if (target.attr('isselected') === "false") {
                    target.children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
                    target.attr('isselected', 'true');
                }
                else {
                    target.children(".smslidemenu_filterItemCheckbox").removeClass("smslidemenu_selectedItem");
                    target.attr('isselected', 'false');
                }
            });
        }
        else {
            $('#' + itemListID).find(".smslidemenu_filterItem").unbind('click').bind('click', function (e) {
                e.stopPropagation();
                var target = $(e.target);
                if (!target.hasClass('smslidemenu_filterItem')) {
                    target = $(e.target).parents(".smslidemenu_filterItem");
                }

                if (e.target.tagName === "INPUT") {
                    return;
                }

                if (target.attr('isselected') === "false") {
                    target.children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
                    target.attr('isselected', 'true');
                }
                else {
                    target.children(".smslidemenu_filterItemCheckbox").removeClass("smslidemenu_selectedItem");
                    target.attr('isselected', 'false');
                }

                if (target.parent().find('.filterSelectAll').length !== 0) {
                    var allChildrens = target.parent().children('div').length - 1;
                    var selectedChildrens = target.parent().children('div[isSelected="true"]').not('.filterSelectAll').length;
                    if (allChildrens === selectedChildrens) {
                        target.parent().children('div').first().children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
                        target.parent().children('div').first().attr('isselected', 'true');
                    }
                    else {
                        target.parent().children('div').first().children(".smslidemenu_filterItemCheckbox").removeClass("smslidemenu_selectedItem");
                        target.parent().children('div').first().attr('isselected', 'false');
                    }
                }
            });
        }

        if (selectAllText !== undefined) {
            $('#' + itemListID).find(".filterSelectAll").unbind('click').bind('click', function (e) {
                e.stopPropagation();
                var target = $(e.target);
                if (!target.hasClass('filterSelectAll')) {
                    target = $(e.target).parents(".filterSelectAll");
                }

                if (target.attr('isselected') === "false") {
                    target.children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
                    target.attr('isselected', 'true');

                    target.siblings(".smslidemenu_filterItem").children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
                    target.siblings(".smslidemenu_filterItem").attr('isselected', 'true');
                }
                else {
                    target.children(".smslidemenu_filterItemCheckbox").removeClass("smslidemenu_selectedItem");
                    target.attr('isselected', 'false');

                    target.siblings(".smslidemenu_filterItem").children(".smslidemenu_filterItemCheckbox").removeClass("smslidemenu_selectedItem");
                    target.siblings(".smslidemenu_filterItem").attr('isselected', 'false');
                }
            });
        }

        if ((contentArray !== undefined && (contentArray.length - 1) === contentArray.filter(function (item) { if (item.isSelected === "true") { return item; } }).length)) {
            var selectAllEle = rightMenuSection.find(".filterSelectAll");
            selectAllEle.children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
            selectAllEle.attr('isselected', 'true');
        }

        if (selectAllItems === true || selectAllItems === "true") {
            var selectAllEle = rightMenuSection.find(".filterSelectAll");
            if (selectAllEle.length !== 0 && (selectAllEle.attr("isselected") !== "true")) {
                selectAllEle.click();
            }
            else {
                var target = rightMenuSection.find(".smslidemenu_filterItem");
                target.children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
                target.attr('isselected', 'true');
            }

        }

        //        if (sectionType === "radio") {
        //            $('#itemList_' + rightMenuSection.attr('id').split('_')[1]).find(".smslidemenu_filterItem").unbind('click').bind('click', function (e) {
        //                var target = $(e.target);
        //                if (!target.hasClass('smslidemenu_filterItem')) {
        //                    target = $(e.target).parents(".smslidemenu_filterItem");
        //                }

        //                if (e.target.tagName === "INPUT") {
        //                    return;
        //                }

        //                target.parent().find(".smslidemenu_filterItemCheckbox").removeClass("smslidemenu_selectedItem");
        //                target.parent().find(".smslidemenu_filterItem").attr('isselected', 'false');
        //                if (target.attr('isselected') === "false") {
        //                    target.children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
        //                    target.attr('isselected', 'true');
        //                }
        //                else {
        //                    target.children(".smslidemenu_filterItemCheckbox").removeClass("smslidemenu_selectedItem");
        //                    target.attr('isselected', 'false');
        //                }
        //            });
        //        }
        //        else {
        //            $('#itemList_' + rightMenuSection.attr('id').split('_')[1]).find(".smslidemenu_filterItem").unbind('click').bind('click', function (e) {
        //                var target = $(e.target);
        //                if (!target.hasClass('smslidemenu_filterItem')) {
        //                    target = $(e.target).parents(".smslidemenu_filterItem");
        //                }

        //                if (e.target.tagName === "INPUT") {
        //                    return;
        //                }

        //                if (target.attr('isselected') === "false") {
        //                    target.children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
        //                    target.attr('isselected', 'true');
        //                }
        //                else {
        //                    target.children(".smslidemenu_filterItemCheckbox").removeClass("smslidemenu_selectedItem");
        //                    target.attr('isselected', 'false');
        //                }
        //            });
        //        }

        //        if (selectAllText !== undefined) {
        //            $('#itemList_' + rightMenuSection.attr('id').split('_')[1]).find(".filterSelectAll").unbind('click').bind('click', function (e) {
        //                e.stopPropagation();
        //                var target = $(e.target);
        //                if (!target.hasClass('filterSelectAll')) {
        //                    target = $(e.target).parents(".filterSelectAll");
        //                }

        //                if (target.attr('isselected') === "false") {
        //                    target.children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
        //                    target.attr('isselected', 'true');

        //                    target.siblings(".smslidemenu_filterItem").children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
        //                    target.siblings(".smslidemenu_filterItem").attr('isselected', 'true');
        //                }
        //                else {
        //                    target.children(".smslidemenu_filterItemCheckbox").removeClass("smslidemenu_selectedItem");
        //                    target.attr('isselected', 'false');

        //                    target.siblings(".smslidemenu_filterItem").children(".smslidemenu_filterItemCheckbox").removeClass("smslidemenu_selectedItem");
        //                    target.siblings(".smslidemenu_filterItem").attr('isselected', 'false');
        //                }
        //            });
        //        }

        //        if (selectAllItems === true || selectAllItems === "true") {
        //            var selectAllEle = rightMenuSection.find(".filterSelectAll");
        //            if (selectAllEle.length !== 0) {
        //                selectAllEle.click();
        //            }
        //            else {
        //                var target = rightMenuSection.find(".smslidemenu_filterItem");
        //                target.children(".smslidemenu_filterItemCheckbox").addClass("smslidemenu_selectedItem");
        //                target.attr('isselected', 'true');
        //            }

        //        }

        if (selectAllText !== undefined) {
            contentArray.splice(0, 1);
        }

        if (bindComplete !== undefined && typeof bindComplete === "function") {
            bindComplete();
        }
    }

    //Returns an array of Objects Containing the 'text' and 'value' property. In case of Dates 
    //-> 'rangeStartDate' - For Date of Type Between the start date value
    //-> 'rangeEndDate' - For Date of Type Between the end date value
    //-> 'singleDate' - For Date of Type From or PriorTo the date value
    //-> 'dateType' - Type of Date(Between, From and Prior To)
    smSlideMenu.prototype.getSelectedItems = function (sectionIdentifier) {
        var selectedItemsArray = [];
        var listItems = $("#" + sectionIdentifier).find(".smslidemenu_itemListDiv").find(".filterOptions");
        $.each(listItems, function (i, e) {
            if ($(e).attr('isselected') === "true") {
                var text = $(e).find(".smslidemenu_filterItemText").text();
                var value = $(e).find(".smslidemenu_filterItemText").attr("value");
                if ($(e).find(".smslidemenu_filterItemText").attr("range_start_date") !== undefined) {
                    var range_start_date = $(e).find(".smslidemenu_filterItemText").find('input[date_type="range_start_date"]').val();
                }
                if ($(e).find(".smslidemenu_filterItemText").attr("range_end_date") !== undefined) {
                    var range_end_date = $(e).find(".smslidemenu_filterItemText").find('input[date_type="range_end_date"]').val();
                }
                if ($(e).find(".smslidemenu_filterItemText").attr("single_date") !== undefined) {
                    var single_date = $(e).find(".smslidemenu_filterItemText").find('input[date_type="single_date"]').val();
                }
                var date_type = $(e).find(".smslidemenu_filterItemText").attr("date_type");
                var header = $("#" + sectionIdentifier).find(".smslidemenu_filterHeader").text().toLowerCase();
                selectedItemsArray.push({ text: text, value: value, rangeStartDate: range_start_date, rangeEndDate: range_end_date, singleDate: single_date, dateType: date_type, headerText: header });
            }
        });
        return selectedItemsArray;
    }

    //Returns an array of selected items 'text'. Doesn't work for date types
    smSlideMenu.prototype.getSelectedItemsText = function (sectionIdentifier) {
        var selectedItemsArray = [];
        var listItems = $("#" + sectionIdentifier).find(".smslidemenu_itemListDiv").find(".filterOptions");
        $.each(listItems, function (i, e) {
            if ($(e).attr('isselected') === "true") {
                var text = $(e).find(".smslidemenu_filterItemText").text();
                selectedItemsArray.push(text);
            }
        });
        return selectedItemsArray;
    }

    //Returns an array of selected items 'value'. Doesn't work for date types
    smSlideMenu.prototype.getSelectedItemsValue = function (sectionIdentifier) {
        var selectedItemsArray = [];
        var listItems = $("#" + sectionIdentifier).find(".smslidemenu_itemListDiv").find(".filterOptions");
        $.each(listItems, function (i, e) {
            if ($(e).attr('isselected') === "true") {
                var value = $(e).find(".smslidemenu_filterItemText").attr("value");
                selectedItemsArray.push(value);
            }
        });
        return selectedItemsArray;
    }

    smSlideMenu.prototype.getAllData = function (slideMenuId) {
        var selectedItemsObj = {};
        var sections = $("#" + slideMenuId).find(".filterSection");
        $.each(sections, function (i, e) {
            var selectedItemsObject = {};
            var tempArrText = [], tempArrValue = [];
            var sectionID = $(e).attr('id');
            var sectionIdentifier = $(e).attr('identifier');
            var sectionType = $(e).attr('section_type');
            if (sectionType === "checkbox") {
                var listItems = $("#" + sectionID).find(".smslidemenu_itemListDiv").find(".filterOptions");
                $.each(listItems, function (i, e) {
                    if ($(e).attr('isselected') === "true") {
                        var text = $(e).find(".smslidemenu_filterItemText").text();
                        var value = $(e).find(".smslidemenu_filterItemText").attr("value");
                        tempArrText.push(text);
                        tempArrValue.push(value);
                    }
                });
                selectedItemsObject = {};
                selectedItemsObject.SelectedValue = tempArrValue.join(",");
                selectedItemsObject.SelectedText = tempArrText.join(",");
                selectedItemsObject.IsAllSelected = (listItems.length === tempArrText.length) ? true : false;
            }
            else if (sectionType === "radio") {
                var range_start_date, range_end_date, single_date, date_type;
                var listItems = $("#" + sectionID).find(".smslidemenu_itemListDiv").find(".smslidemenu_filterItem");
                $.each(listItems, function (i, e) {
                    if ($(e).attr('isselected') === "true") {
                        var text = $(e).find(".smslidemenu_filterItemText").text();
                        var value = $(e).find(".smslidemenu_filterItemText").attr("value");
                        if ($(e).find(".smslidemenu_filterItemText").attr("range_start_date") !== undefined) {
                            range_start_date = $(e).find(".smslidemenu_filterItemText").find('input[date_type="range_start_date"]').val();
                        }
                        if ($(e).find(".smslidemenu_filterItemText").attr("range_end_date") !== undefined) {
                            range_end_date = $(e).find(".smslidemenu_filterItemText").find('input[date_type="range_end_date"]').val();
                        }
                        if ($(e).find(".smslidemenu_filterItemText").attr("single_date") !== undefined) {
                            single_date = $(e).find(".smslidemenu_filterItemText").find('input[date_type="single_date"]').val();
                        }
                        date_type = $(e).find(".smslidemenu_filterItemText").attr("date_type");
                        tempArrText.push(text);
                        tempArrValue.push(value);
                    }
                });
                selectedItemsObject = {};
                selectedItemsObject.SelectedValue = tempArrValue[0];
                selectedItemsObject.SelectedText = tempArrText[0];
                selectedItemsObject.RangeStartDate = range_start_date;
                selectedItemsObject.RangeEndDate = range_end_date;
                selectedItemsObject.SingleDate = single_date;
                selectedItemsObject.DateType = date_type;
            }
            selectedItemsObj[sectionIdentifier] = selectedItemsObject;
        });
        return selectedItemsObj;
    }

    smSlideMenu.prototype.show = function (slideMenuId, direction) {
        if ($("#" + slideMenuId).attr("state") === "close") {
            $("#" + slideMenuId).attr("state", "open");
            var animateObj = {}, dir;
            if (direction === undefined) {
                dir = "right";
            }
            else {
                dir = direction;
            }
            animateObj[dir] = "0px";
            $("#" + slideMenuId).animate(animateObj);
        }
    }

    smSlideMenu.prototype.hide = function (slideMenuId, direction) {
        if ($("#" + slideMenuId).attr("state") === "open") {
            $("#" + slideMenuId).attr("state", "close");
            var animateObj = {}, dir;
            if (direction === undefined) {
                dir = "right";
            }
            else {
                dir = direction;
            }
            animateObj[dir] = "-350px";
            $("#" + slideMenuId).animate(animateObj);
        }
    }

    //Removes the Slide Menu from DOM
    smSlideMenu.prototype.destroy = function (targetDiv) {
        $("#" + targetDiv).remove();
        if (smslidemenu.counter > 0) {
            smslidemenu.counter--;
        }
    }

    //Removes a section from Slide Menu
    smSlideMenu.prototype.destroySection = function (sectionID) {
        $("#" + parentDiv).find("#" + sectionID).remove();
    }

    return smslidemenu;
})();