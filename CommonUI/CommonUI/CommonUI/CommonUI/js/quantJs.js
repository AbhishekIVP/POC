	var SRM_jsonDictionary = function () {
	    return {
	        getAllValues: function (jsonDictionary) {
	            var values = [];
	            for (var i = 0; i < jsonDictionary.length; i++) {
	                var item = jsonDictionary[i];
	                values.push(item.Value);
	            }
	            return values;
	        },
	        getAllKeys: function (jsonDictionary) {
	            var keys = [];
	            for (var i = 0; i < jsonDictionary.length; i++) {
	                var item = jsonDictionary[i];
	                keys.push(item.Key);
	            }
	            return keys;
	        },
	        getValue: function (jsonDictionary, key) {
	            for (var i = 0; i < jsonDictionary.length; i++) {
	                var item = jsonDictionary[i];
	                if (item.Key == key)
	                    return item.Value;
	            }
	        },
            getObjectByKey: function(jsonDictionary, key){
                for (var i = 0; i < jsonDictionary.length; i++) {
	                var item = jsonDictionary[i];
	                if (item.Key == key)
	                    return item;
	            }
            }
	    }
	} ();

	var QuantJS = function (baseContainerRef) {
	    var baseContainer;
	    var filters;

	    var encodedQueryTokenSeparator = "§";
	    var encodedQueryParamsSeparator = "‡";
	    var encodedQueryParamBrackets = ["†", "±"];
	    var encodedQueryValueIdentifier = "¦";
	    var encodedQueryPartsSeparator = "‰";

	    var manualEntryKeywordType = "Manual Entry";
	    var manualEntryFilterName = "Enter String";

	    function attachEventHandlers() {
	        $(".query-box", baseContainer).off('click').on('click', function () {
	            $(".query-token-editable", $(this)).focus();
	            $(".token-suggestions", baseContainer).css('display', 'block');
	        });
	        $(".token-suggestion", baseContainer).off('click').on('click', tokenSuggestionClickHandler);
	        $(".query-token", baseContainer).off('click').on('click', queryTokenClickHandler);
	        $(".query-token-editable", baseContainer).off('focus').on('focus', function () { $(".token-suggestions", baseContainer).css('display', 'block'); });
	        $(".query-token-editable", baseContainer).off('keyup').on('keyup', queryTokenKeypressHandler);
	        $(".query-token-editable", baseContainer).off('keydown').on('keydown', queryTokenKeyDownHandler);
	        //$(".query-token-editable", baseContainer).off('keyup').on('keyup', queryTokenKeyUpHandler);
	        $(".query-submit-btn", baseContainer).off('click').on('click', querySubmitClickHandler);
	        $(".query-clear-btn", baseContainer).off('click').on('click', resetQuerySection);
	        //$(".run-query-btn", baseContainer).off('click').on('click', runQueryBtnClickHandler);
	        $(".add-query-section-btn", baseContainer).off('click').on('click', addQuerySectionClickHandler);
	        $(".save-query-btn", baseContainer).off('click').on('click', saveQueryClickHandler);
	        $(".btn-refresh", baseContainer).off('click').on('click', refreshClickHandler);

	        $("#saveQuertBtn").off('click').on('click', saveQuery);
	        $(".btn-email", baseContainer).off('click').on('click', emailClickHandler);
	        $(".complete-query-section .query .text-query", baseContainer).off('click').on('click', queryTabClickHandler);
	        $(document).click(function () {
	            if ($(event.target).parents(".query-box").length == 0 && !$(event.target).hasClass("query-box") && !$(event.target).hasClass("token-suggestion"))
	                $(".token-suggestions").hide();
	        });
	    }

	    function queryTokenKeypressHandler(event) {
	        var allSuggestionElems = $(".token-suggestion", $(this).next());
	        var filteredSuggestions = [];
	        var exactMatches = [];
	        var untrimmedMatches = [];
	        var searchText = $(this).val().trim();
	        var untrimmedText = $(this).val();

	        var allowManualEntry = false;
	        for (var i = 0; i < allSuggestionElems.length; i++) {
	            var suggestionText = $(allSuggestionElems[i]).text();
	            var keywordType = $(allSuggestionElems[i]).attr("keywordType");
	            if (keywordType == manualEntryKeywordType)
	                allowManualEntry = true;

	            if (suggestionText.toLowerCase().indexOf(searchText.toLowerCase()) != -1) {
	                filteredSuggestions.push(suggestionText);
	            }
	            if (suggestionText.toLowerCase() == searchText.toLowerCase()) {
	                exactMatches.push(suggestionText);
	                $(allSuggestionElems).removeClass("active");
	                $(allSuggestionElems[i]).addClass("active");
	            }
	            if (suggestionText.toLowerCase().indexOf(untrimmedText.toLowerCase()) != -1)
	                untrimmedMatches.push(suggestionText);
	        }

	        var parent = $(this).parent();
	        if ($(this).parents(".query-token-parameter").length > 0)
	            parent = $(this).parents(".query-token-parameter");

	        var queryTokenEditable = $(".query-token.editable", baseContainer);

	        if (event.which == 32 && ((filteredSuggestions.length == 1 && exactMatches.length == 1) || (filteredSuggestions.length > 1 && exactMatches.length == 1 && untrimmedMatches.length == 0))) {
	            if (filteredSuggestions.length == 1 && exactMatches.length == 1)
	                addQueryToken(parent, filteredSuggestions[0]);
	            else
	                addQueryToken(parent, exactMatches[0]);
	            $(this).val("");

	            var index = $(".query-box>.query-token", baseContainer).index($(".query-token.editable", baseContainer));
	            if ($(queryTokenEditable).parents(".query-token-parameter").length == 0 && index < $(".query-box>.query-token", baseContainer).length - 1) {
	                editQueryToken($(queryTokenEditable).nextAll(".query-token")[0]);
	            }
	            return;
	        }
	        else if (event.which == 13 && filteredSuggestions.length == 0 && ($(parent).hasClass("query-token-parameter") || allowManualEntry == true)) {
	            addQueryToken(parent, searchText, allowManualEntry);
	            $(this).val("");
	            var index = $(".query-box>.query-token", baseContainer).index($(".query-token.editable", baseContainer));
	            if ($(queryTokenEditable).parents(".query-token-parameter").length == 0 && index < $(".query-box>.query-token", baseContainer).length - 1) {
	                editQueryToken($(queryTokenEditable).nextAll(".query-token")[0]);
	            }
	            return;
	        }

	        for (var i = 0; i < allSuggestionElems.length; i++) {
	            var suggestionElem = allSuggestionElems[i];
	            var suggestionText = $(suggestionElem).text();

	            if (filteredSuggestions.indexOf(suggestionText) != -1)
	                $(suggestionElem).removeClass("filtered-out");
	            else
	                $(suggestionElem).addClass("filtered-out");
	        }

	        var suggestionSections = $(allSuggestionElems).parent();
	        var tokenSuggestionsElemWidth = 0;
	        for (var i = 0; i < suggestionSections.length; i++) {
	            var visibleSuggestions = $(".token-suggestion:not('.hidden'):not('.filtered-out')", suggestionSections[i]);
	            if (visibleSuggestions.length == 0)
	                $(suggestionSections[i]).addClass("hidden");
	            else {
	                $(suggestionSections[i]).removeClass("hidden");
	                tokenSuggestionsElemWidth += ($(suggestionSections[i]).width() + 5);
	            }
	        }
	        $(suggestionSections).parent().width(tokenSuggestionsElemWidth);

	        //	        if (filteredSuggestions.length == 1) {
	        //	            $(allSuggestionElems).removeClass("active");
	        //	            $(".token-suggestion:not('.hidden'):not('.filtered-out')", baseContainer).addClass("active");
	        //	        }

	        adjustTextInputWidth($(this));
	    }

	    function queryTokenKeyUpHandler(event) {

	    }

	    function queryTokenKeyDownHandler(event) {
	        if (event.key == "ArrowUp" || event.key == "ArrowDown" || event.key == "ArrowRight" || event.key == "ArrowLeft") {
	            var suggestions = $(".token-suggestion:not('.hidden'):not('.filtered-out')", baseContainer);
	            var activeSuggestion = $(".token-suggestion.active", baseContainer);

	            if (activeSuggestion.length == 0 && event.key == "ArrowUp")
	                $($(suggestions)[suggestions.length - 1]).addClass("active");
	            else if (activeSuggestion.length == 0 && event.key == "ArrowDown")
	                $($(suggestions)[0]).addClass("active");
	            else if (activeSuggestion.length > 0 && event.key == "ArrowUp" && $(activeSuggestion).prevAll(":not('hidden'):not('filtered-out')").length != 0) {
	                $(activeSuggestion).removeClass("active");
	                var nowActiveSuggestion = $($(activeSuggestion).prevAll(":not('.hidden'):not('.filtered-out')")[0]).addClass("active");
	                scrollSuggestionIntoView(nowActiveSuggestion);
	            }
	            else if (activeSuggestion.length > 0 && event.key == "ArrowDown" && $(activeSuggestion).nextAll(":not('hidden'):not('filtered-out')").length != 0) {
	                $(activeSuggestion).removeClass("active");
	                var nowActiveSuggestion = $($(activeSuggestion).nextAll(":not('.hidden'):not('.filtered-out')")[0]).addClass("active");
	                scrollSuggestionIntoView(nowActiveSuggestion);
	            }
	            else if (activeSuggestion.length > 0 && (event.key == "ArrowRight" || event.key == "ArrowLeft")) {
	                var activeSection = $(activeSuggestion).parent();
	                var activeSuggestionIndex = $(".token-suggestion:not('.hidden'):not('.filtered-out')", activeSection).index(activeSuggestion);

	                var nextSection;
	                if (event.key == "ArrowRight" && $(activeSection).nextAll(".token-suggestion-section:not('.hidden')").length != 0)
	                    nextSection = $(activeSection).nextAll(".token-suggestion-section:not('.hidden')")[0];
	                else if (event.key == "ArrowLeft" && $(activeSection).prevAll(".token-suggestion-section:not('.hidden')").length != 0)
	                    nextSection = $(activeSection).prevAll(".token-suggestion-section:not('.hidden')")[0];

	                var cntSuggestions = $(".token-suggestion:not('.hidden'):not('.filtered-out')", nextSection).length;

	                if (activeSuggestionIndex >= cntSuggestions && cntSuggestions > 0)
	                    activeSuggestionIndex = cntSuggestions - 1;

	                if (cntSuggestions > 0) {
	                    $(activeSuggestion).removeClass("active");
	                    var nowActiveSuggestion = $($(".token-suggestion:not('.hidden'):not('.filtered-out')", nextSection)[activeSuggestionIndex]).addClass("active");
	                    scrollSuggestionIntoView(nowActiveSuggestion);
	                }
	            }
	        }
	        else if (event.key == "Backspace" && $(this).val() == "") {
	            var editableQueryToken = $(this).parent();
	            var prevToken = findPrevQueryToken(editableQueryToken);

	            if ($(editableQueryToken).parents(".query-token-parameter").length > 0 && $(prevToken).parents(".query-token-parameter").length > 0) {
	                if (curFunctionParamN > 0) curFunctionParamN--;
	            }
	            else if ($(editableQueryToken).parents(".query-token-parameter").length > 0 && $(prevToken).parents(".query-token-parameter").length == 0) {
	                if (curFunction != null) {
	                    curFunction = null;
	                    curFunctionParamN = 0;
	                }
	            }

	            if ($(editableQueryToken).prev().length > 0 && $(editableQueryToken).prev()[0].tagName == "BR")
	                $(editableQueryToken).prev().remove();

	            editQueryToken(prevToken);
	            $(".query-token-editable", baseContainer).focus();
	            event.preventDefault();

	            if ($(".query-token:not('.editable')").length == 0)
	                resetPlaceholder();
	        }
	        else if (event.key == "Enter" || event.key == "Tab") {
	            var queryTokenEditable = $(".query-token.editable", baseContainer);
	            var parent = $(this).parent();
	            if ($(this).parents(".query-token-parameter").length > 0)
	                parent = $(this).parents(".query-token-parameter");
	            var activeSuggestion = $(".token-suggestion.active", baseContainer);

	            if (activeSuggestion.length > 0) {
	                addQueryToken(parent, $(activeSuggestion).text());
	                $(this).val("");

	                var index = $(".query-box>.query-token", baseContainer).index($(".query-token.editable", baseContainer));
	                if ($(queryTokenEditable).parents(".query-token-parameter").length == 0 && index < $(".query-box>.query-token", baseContainer).length - 1) {
	                    editQueryToken($(queryTokenEditable).nextAll(".query-token")[0]);
	                }
	            }
	            //	            else if (event.key == "Enter" && $(".query-token-editable", baseContainer).val() == "") {
	            //	                $(queryTokenEditable).before("<br/>");
	            //	            }
	            else if (event.key == "Tab") {
	                var allowManualEntry = false;
	                if (filters != null) {
	                    for (var i = 0; i < filters.length; i++) {
	                        if (filters[i].KeywordType == manualEntryKeywordType) {
	                            allowManualEntry = true;
	                            break;
	                        }
	                    }
	                }

	                if (allowManualEntry || $(parent).hasClass("query-token-parameter")) {
	                    addQueryToken(parent, $(".query-token-editable", queryTokenEditable).val(), allowManualEntry);
	                    $(this).val("");
	                    var index = $(".query-box>.query-token", baseContainer).index($(".query-token.editable", baseContainer));
	                    if ($(queryTokenEditable).parents(".query-token-parameter").length == 0 && index < $(".query-box>.query-token", baseContainer).length - 1) {
	                        editQueryToken($(queryTokenEditable).nextAll(".query-token")[0]);
	                    }
	                }
	            }
	            else if ($(parent).hasClass("query-token-parameter") == false && isFetchingQuantData == false) {
	                $(".query-submit-btn", baseContainer).trigger('click');
	            }

	            event.preventDefault();
	        }
	        else if (event.key == "Escape") {
	            if ($(".token-suggestions", baseContainer).css('display') == 'none')
	                $(".token-suggestions", baseContainer).show();
	            else
	                $(".token-suggestions", baseContainer).hide();
	        }
	        //	        else if (event.key == "Tab") {
	        //                
	        //	        }
	    }

	    function scrollSuggestionIntoView(tokenSuggestion) {
	        var parent = $(tokenSuggestion).parent();
	        var parentHeight = $(parent).height();
	        var tokenSuggestionHeight = $(tokenSuggestion).outerHeight();

	        var parentViewPortTop = $(parent).scrollTop();
	        var parentViewPortBottom = parentViewPortTop + parentHeight;
	        $(parent).css('overflow', 'initial');
	        var tokenSuggestionTop = $(tokenSuggestion).position().top;
	        $(parent).css('overflow', 'auto');
	        var tokenSuggestionBottom = tokenSuggestionTop + tokenSuggestionHeight;

	        if (tokenSuggestionTop < parentViewPortTop) {
	            $(parent).scrollTop(tokenSuggestionTop);
	        }
	        else if (tokenSuggestionBottom > parentViewPortBottom) {
	            $(parent).scrollTop(tokenSuggestionBottom - parentHeight);
	        }
	    }

	    function findPrevQueryToken(tokenRef) {
	        if ($(tokenRef).parents(".query-token-parameter").length > 0) {
	            var queryTokenParam = $(tokenRef).parents(".query-token-parameter")[0];

	            if ($(queryTokenParam).prev().length > 0 && $($(queryTokenParam).prev()).hasClass("query-token-parameter")) {
	                var targetTokenParam = $(queryTokenParam).prev();
	                return $(".query-token", targetTokenParam);
	            }
	            else if ($(queryTokenParam).prev().length == 0 || $($(queryTokenParam).prev()).hasClass("query-token-parameter") == false) {
	                return $(queryTokenParam).parents(".query-token")[0];
	            }
	        }
	        else if ($(tokenRef).prevAll(".query-token").length > 0) {
	            var prevToken = $(tokenRef).prevAll(".query-token")[0];

	            if ($(".query-token-parameter", prevToken).length > 0) {
	                var paramTokens = $(".query-token-parameter", prevToken);
	                return $(".query-token", paramTokens[paramTokens.length - 1]);
	            }

	            return $(tokenRef).prevAll(".query-token")[0];
	        }
	        return null;
	    }

	    function tokenSuggestionClickHandler() {
	        var text = $(this).text();

	        var parent = $(this).parents(".query-token");
	        if ($(this).parents(".query-token-parameter").length > 0)
	            parent = $(this).parents(".query-token-parameter");

	        addQueryToken(parent, text);

	        var queryTokenEditable = $(".query-token.editable", baseContainer);
	        var index = $(".query-box>.query-token", baseContainer).index($(".query-token.editable", baseContainer));
	        if (index < $(".query-box>.query-token", baseContainer).length - 1 && $(queryTokenEditable).parents(".query-token-parameter").length == 0) {
	            editQueryToken($(queryTokenEditable).nextAll('.query-token')[0]);
	        }
	    }

	    function queryTokenClickHandler() {
	        if (!$(this).hasClass("editable") && $(this).parents(".query-token-parameter").length == 0) {
	            if (curFunction != null) {
	                curFunction = null;
	                curFunctionParamN = 0;
	            }
	            editQueryToken($(this));
	        }
	        event.stopPropagation();
	    }

	    var queryIdCounter = 0;
	    function getNewQueryId() {
	        return queryIdCounter++;
	    }

	    function getNewQueryElem(textQuery, encodedQuery, queryId, searchId, isHidden) {
	        var queryElem = $("<div>").addClass("query").attr("queryId", queryId);
	        if (typeof searchId != 'undefined')
	            var textQuery = $("<div>").addClass("text-query").attr('savedSearchID', searchId).text(textQuery);
	        else if (typeof isHidden != 'undefined')
	            var textQuery = $("<div>").addClass("text-query hidden").text(textQuery);
	        else
	            var textQuery = $("<div>").addClass("text-query").text(textQuery);
	        var encodedQuery = $("<div>").addClass("encoded-query hidden").text(encodedQuery);
	        //var runQueryBtn = $("<div>").addClass("btn btn-sm run-query-btn").text("Run");

	        $(queryElem).append(textQuery).append(encodedQuery)//.append(runQueryBtn);

	        return queryElem;
	    }

	    function modifyQueryElem(queryElem, textQuery, encodedQuery) {
	        $(".text-query", queryElem).text(textQuery);
	        $(".encoded-query", queryElem).text(encodedQuery);

	        return queryElem;
	    }

	    var isFetchingQuantData = false;
	    function setIsFetchingQuantData(value) {
	        isFetchingQuantData = value;
	        if (isFetchingQuantData == true) {
	            $("div.loading-screen-placeholder").removeClass("hidden");
	        }
	        else {
	            $("div.loading-screen-placeholder").addClass("hidden");
	        }
	    }

	    function querySubmitClickHandler() {
	        //var query = $(".complete-query-section .query", baseContainer).html();	
	        if (isFetchingQuantData == true)
	            return;
	        var queryEncodedQueryPair = readQuery();
	        var query = queryEncodedQueryPair[0];
	        var encodedQuery = queryEncodedQueryPair[1];
	        if (query == "")
	            return;
	        if ($(this).attr("targetQueryId") != undefined) {
	            var queryElem = $(".complete-query-section .query[queryId='" + $(this).attr("targetQueryId") + "']");
	            modifyQueryElem(queryElem, query, encodedQuery);
	            $(".text-query", queryElem).addClass("hidden");
	        }
	        else {
	            var queryElem = getNewQueryElem(query, encodedQuery, getNewQueryId());
	            $(".complete-query-section", baseContainer).append(queryElem);
	        }

	        $(".buttons-section", baseContainer).removeClass("hidden");
	        attachEventHandlers();

	        callQuantService();

	        $(".result-section", baseContainer).addClass("hidden");
	        $(".complete-query-section .query:last-child", baseContainer).after($(".result-section", baseContainer));
	    }

	    function GetGUID() {
	        var objDate = new Date();
	        var str = objDate.toString();
	        str = objDate.getDate().toString() + objDate.getMonth().toString() + objDate.getFullYear().toString() + objDate.getHours().toString() + objDate.getMinutes().toString() + objDate.getSeconds().toString() + objDate.getMilliseconds().toString() + eval('Math.round(Math.random() * 10090)').toString();
	        return str;
	    }

	    function callQuantService(encodedQuery, onCompletionFunc) {

	        //$('.SRM-quant-container').css('background-color', '#f6f6f6');
	        var curPageId = GetGUID();
	        var viewKey = GetGUID();
	        var sessionId = GetGUID();
	        var query;

	        if (encodedQuery == undefined)
	            query = readCompleteQuery();
	        else
	            query = encodedQuery;

	        setIsFetchingQuantData(true);
	        $.ajax({
	            type: 'POST',
	            url: "BaseUserControls/Service/CommonService.svc/GetQuantData",
	            contentType: "application/json",
	            dataType: "json",
	            data: JSON.stringify({ username: "admin", encodedQuery: query, tokenSeparator: encodedQueryTokenSeparator, paramSeparator: encodedQueryParamsSeparator, functionStart: encodedQueryParamBrackets[0], functionEnd: encodedQueryParamBrackets[1], valueIdentifier: encodedQueryValueIdentifier, divId: radGridClientId, curPageId: curPageId, viewKey: viewKey, sessionId: sessionId, querySeparator: encodedQueryPartsSeparator }),
	            success: function (data) {
	                $('#SRM-savedSearchesTitle').css('display', 'none');
	                var tempObj = $.extend({}, getNeoGridInfoObj("admin", viewKey, curPageId, sessionId));
	                //	                xlgridloader.create(radGridClientId, "Test", tempObj, "");
	                //	                resizeGridFinal(radGridClientId, 'complete-query-section', 'result-section', 'buttons-section', 0, 210);

	                if (onCompletionFunc == undefined) {
	                    $(".result-section", baseContainer).removeClass("hidden");
	                    $(".complete-query-section", baseContainer).removeClass("hidden");
	                    $(".query-section", baseContainer).addClass("hidden");
	                    $(".complete-query-section .query .text-query", baseContainer).removeClass("hidden");
	                    //$(".complete-query-section .query .run-query-btn", baseContainer).removeClass("hidden");
	                }
	                else
	                    onCompletionFunc();

	                xlgridloader.create(radGridClientId, "Test", tempObj, "");
	                resizeGridFinal(radGridClientId, 'complete-query-section', 'result-section', 'buttons-section', 0, 140);
	                $('#' + radGridClientId).unbind('click').click(function (e) {
	                    onClickGrid(e);
	                })
	                setIsFetchingQuantData(false);

	            },
	            error: function () {
	                console.log("Quant Data cannot be fetched");
	            }
	        });
	    }

	    function getNeoGridInfoObj(username, viewKey, curPageId, sessionId) {
	        var obj = {
	            "SelectRecordsAcrossAllPages": true,
	            "ViewKey": viewKey,
	            "CacheGriddata": true,
	            //"GridId": divId,
	            "CurrentPageId": curPageId,
	            "SessionIdentifier": sessionId,
	            "UserId": username,
	            "Height": "400px",
	            "ColumnsNotToSum": [],
	            "RequireEditGrid": false,
	            "IdColumnName": "row_id",
	            "TableName": "TEST",
	            "PageSize": 200,
	            "RequirePaging": true,
	            "DoNotExpand": false,
	            "ItemText": "Number of Records",
	            "DoNotRearrangeColumn": true,
	            "RequireGrouping": true,
	            "RequireGroupExpandCollapse": true,
	            "RequireSelectedRows": true,
	            "RequireExportToExcel": true,
	            "RequireSearch": true,
	            "RequireResizing": true,
	            "RequireLayouts": false,
	            "DateFormat": "yyyy/MM/dd",
	            "RequireFilter": true,
	            "CheckBoxInfo": null,
	            "RightAlignHeaderForNumerics": false,
	            "CssExportRows": "xlneoexportToExcel"
	        };
	        return obj;
	    }

	    function resetQuerySection() {
	        var querySection = $(".query-section", baseContainer);

	        $(querySection).removeClass("hidden");

	        $(".query-token-editable", querySection).val("");
	        var editableQueryTokenHTML = $(".query-token.editable", baseContainer).prop("outerHTML");
	        $(".query-box", querySection).html(editableQueryTokenHTML);
	        resetPlaceholder();

	        filters = universeFilters;
	        fillSuggestionsForFilter(filters);

	        return querySection;
	    }

	    function onClickGrid(event) {
	        event = event || window.event;
	        var src = $(event.target);

	        if (src.prop('id') != null && (src.prop('tagName').toUpperCase() == 'DIV' || src.prop('tagName').toUpperCase() == 'SPAN')) {
	            if (src.prop('id').toLowerCase().indexOf('divsecurityname') != -1) {
	                var securityName = src.attr('id').split('divSecurityName')[1];
	                //alert(securityName);
	                securityName = securityName.replace("&", "%26");

	                window.top.leftMenu.createTab('', 'CommonUI/screenersecurityview.aspx?secname=' + securityName, GetGUID(), securityName);
	                //SecMasterJSCommon.SMSCommons.openWindowForSecurityWithHiglight(true, securityId, "", "", true, true, SecMasterJSCommon.SMCreateUpdateOpenByDefault.None, 0, null);
	            }
	            //	            else if (src.prop('id').toLowerCase().indexOf('diventitycode') != -1) {
	            //	                var entityCode = src.attr('id').split('divEntityCode')[1];
	            //	                //Open Create Page
	            //	                var containerPage = "App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.Container.aspx?";
	            //	                var queryString = "entityCode=" + entityCode + "&";
	            //	                var uniqueId = "unique_edit_" + GetGUID();
	            //	                var url = containerPage + queryString.trim() + "pageIdentifier=ViewEntityFromSearch&tabFrameId=" + uniqueId + "&RMTabUniqueID=" + uniqueId;
	            //	                RefMasterJSCommon.RMSCommons.createTab("RefM_ViewEntity", url, uniqueId, entityCode);
	            //	            }
	        }
	    }

	    function addQuerySectionClickHandler() {
	        var querySection = resetQuerySection();

	        var queryId = getNewQueryId();
	        var dummyQueryElem = getNewQueryElem("", "", queryId, undefined, true);
	        $(dummyQueryElem).append(querySection);
	        //$(".run-query-btn", dummyQueryElem).addClass("hidden");
	        $(".query-submit-btn", querySection).attr("targetQueryId", queryId);

	        $(".complete-query-section", baseContainer).append($("<div class='fa fa-chevron-circle-down' style='color:grey;font-size: 14px;padding-top: 5px;'>"));
	        $(".complete-query-section", baseContainer).append(dummyQueryElem);

	        $(".result-section", baseContainer).addClass("hidden");
	        $(".query-token-editable").focus();
	        $(".complete-query-section .query .text-query", baseContainer).removeClass("hidden");
	        $(".text-query", dummyQueryElem).addClass("hidden");
	    }

	    function saveQuery() {

	        var encodedQuery = readCompleteQuery();
	        var textQuery = readCompleteQueryText();
	        var savedSearchID = readSavedSearchID();
	        var savedSearchName = $('#srm_save_searchName').val();

	        $.ajax({
	            type: 'POST',
	            url: "BaseUserControls/Service/CommonService.svc/InsertUpdateQuantSavedSearch",
	            contentType: "application/json",
	            dataType: "json",
	            data: JSON.stringify({ searchID: savedSearchID, userName: "admin", searchName: savedSearchName, searchQuery: textQuery, searchEncodedQuery: encodedQuery }),
	            success: function (data) {
	                if (data.d.IsSuccess == true)
	                    displayPopup(data.d.Message, "success", getSavedSearches);
	                else
	                    displayPopup(data.d.Message, "failure");
	            },
	            error: function () {
	                displayPopup("Searches could not be saved", "failure");
	                console.log("Searches could not be saved");
	            }
	        });
	    }

	    function saveQueryClickHandler() {


	        $('#srm_quant_savePopup').css('display', '');
	        //$('#srm_save_search_historical')

	        $('.srm_save_section2 .srm_toggle').click(function (e) {
	            if (!($(e.target).hasClass('active'))) {
	                $('.srm_save_section2 .srm_toggle').removeClass('active');
	                $(e.target).addClass('active');
	            }
	            if ($(e.target).text() == 'On') {
	                $('.srm_save_section3').css('display', '');
	            }
	            else
	                $('.srm_save_section3').css('display', 'none');
	        });

	        $('.srm_save_section3 .srm_toggle').click(function (e) {
	            if (!($(e.target).hasClass('active'))) {
	                $('.srm_save_section3 .srm_toggle').removeClass('active');
	                $(e.target).addClass('active');
	            }
	        });

	        //	        $('#srm_save_search_report').change(function () {
	        //	            if ($(this).is(":checked")) {
	        //	                $('.srm_save_section3').css('display', '');
	        //	            }
	        //	            else
	        //	                $('.srm_save_section3').css('display', 'none');
	        //	        });

	        $('#closeSavePopup').click(function () {
	            $('#srm_quant_savePopup').css('display', 'none');
	        });
	    }

	    function refreshClickHandler() {

	        SRM_QuantJS.init(universeFilters);
	        $('#srm_quant_savePopup').css('display', 'none');
	        $('#srm_quant_messagePopup').css('display', 'none');
	        $('#srm_quant_emailPopup').css('display', 'none');
	        $('.buttons-section').addClass('hidden');
	        $('.result-section').addClass('hidden');
	        $('#SRM-savedSearchesTitle').css('display', '');
	        $('#complete-query-section').children().remove();
	        $('.query-section').removeClass('hidden');

	    }

	    function emailClickHandler() {
	        $('#srm_quant_emailPopup').css('display', '');
	        $('#radaddindestroy').click(function () {
	            $('#srm_quant_emailPopup').css('display', 'none');
	        });
	        var html = '';
	        if ($('#complete-query-section').children() != null && $('#complete-query-section').children().length > 0) {
	            for (var index = 0; index < $('#complete-query-section').children('.query').length; index++) {

	                //if ($($('#complete-query-section').children()[index]).attr('class') != 'result-section') {
	                //if (typeof $($('#complete-query-section').find('.text-query')[index])[0] != 'undefined') {
	                //if ($($('#complete-query-section').find('.text-query')[index])[0].innerHTML != '') {
	                html += '<div class="query"><div class="text-query">' + $($('#complete-query-section').find('.text-query')[index])[0].innerHTML + '</div></div>';
	                if (index < $.find('.complete-query-section .text-query').length - 1) {
	                    html += '<div class="fa fa-chevron-circle-down" style="color:grey;font-size: 14px;padding-top: 5px;"></div>';
	                }
	                //}
	                //}
	                //}
	            }
	        }
	        //$('.jqte_editor').append($('#complete-query-section').html());
	        if (html != '') {
	            $('.jqte_editor').html('Hi,</br></br>Please find attached results for ');
	            $('.jqte_editor').append(html);
	        }
	    }

	    function bindDatePicker() {
	        CallCustomDatePicker('fromDate', 'd/m/Y', null, optionDateTime.DATE, 15, false);
	        CallCustomDatePicker('toDate', 'd/m/Y', null, optionDateTime.DATE, 15, false);
	    }

	    function queryTabClickHandler() {
	        var querySection = resetQuerySection();

	        $(".query-submit-btn", querySection).attr("targetQueryId", $(this).parent().attr("queryId"));

	        var encodedQuery = $(".encoded-query", $(this).parent()).text();
	        $($(this).parent()).append(querySection);

	        $(".complete-query-section .query .text-query", baseContainer).removeClass("hidden");
	        $(this).addClass("hidden");
	        //$(".run-query-btn", $(this).parent()).addClass("hidden");
	        $(".result-section", baseContainer).removeClass("hidden");
	        parseQuery(encodedQuery, universeFilters);

	        runQueryBtnClickHandler($(this).parent());
	    }

	    function runQueryBtnClickHandler(queryElem) {
	        //var queryElem = $(this).parent();
	        var prevQueryElems = $(queryElem).prevAll(".query");

	        var encodedQueries = [];

	        for (var i = prevQueryElems.length - 1; i >= 0; i--) {
	            encodedQueries.push($(".encoded-query", prevQueryElems[i]).text());
	        }

	        encodedQueries.push($(".encoded-query", queryElem).text());

	        $(".result-section", baseContainer).addClass("hidden");
	        $(queryElem).after($(".result-section", baseContainer));

	        callQuantService(encodedQueries.join(encodedQueryPartsSeparator), function () {
	            $(".result-section", baseContainer).removeClass("hidden");
	            $(".complete-query-section", baseContainer).removeClass("hidden");
	        });
	    }

	    function readQuery(tokens) {
	        if (tokens == undefined)
	            var tokens = $(".query-box>.query-token", baseContainer);
	        var query = "";
	        var encodedQuery = "";

	        for (var i = 0; i < tokens.length; i++) {
	            var token = tokens[i];

	            //	            if ($(token).hasClass("editable")) {
	            //	                continue;
	            //	            }

	            if ($(".query-token-parameter", token).length > 0) {
	                query += $(token)[0].childNodes[0].nodeValue + " ";
	                encodedQuery += $(token).attr("filterId") + encodedQueryTokenSeparator;
	                var paramTokens = $(".query-token-parameter", token);
	                var params = [];
	                for (var j = 0; j < paramTokens.length; j++) {
	                    //params.push($(paramTokens[j]).text());
	                    if ($(token).attr("keywordType") != "Operator" && $(".params-start", paramTokens[j]).length == 1) {
	                        query += $(".params-start", paramTokens[j]).text();
	                        encodedQuery += encodedQueryParamBrackets[0];
	                    }
	                    if ($(".query-token", paramTokens[j]).length == 1) {
	                        query += $(".query-token", paramTokens[j]).text();

	                        if ($(".query-token", paramTokens[j]).attr("filterId") == $(".query-token", paramTokens[j]).text())
	                            encodedQuery += encodedQueryValueIdentifier + $(".query-token", paramTokens[j]).text() + encodedQueryValueIdentifier;
	                        else
	                            encodedQuery += $(".query-token", paramTokens[j]).attr("filterId");
	                    }
	                    if ($(".param-separator", paramTokens[j]).length == 1) {
	                        query += " " + $(".param-separator", paramTokens[j]).text();
	                        encodedQuery += encodedQueryParamsSeparator;
	                    }
	                    if ($(token).attr("keywordType") != "Operator" && $(".params-end", paramTokens[j]).length == 1) {
	                        query += $(".params-end", paramTokens[j]).text();
	                        encodedQuery += encodedQueryParamBrackets[1];
	                        //encodedQuery += encodedQueryTokenSeparator;
	                    }
	                    query += " ";
	                    //encodedQuery += encodedQueryTokenSeparator;
	                }
	                encodedQuery += encodedQueryTokenSeparator;
	                //	                query += " ";
	                //	                encodedQuery += " "
	            }
	            else {
	                var tokenVal;
	                var tokenFilterId;
	                if ($(token).hasClass("editable")) {
	                    tokenVal = $(".query-token-editable", token).val();
	                    tokenFilterId = tokenVal;
	                }
	                else {
	                    tokenVal = $(token).text();
	                    tokenFilterId = $(token).attr("filterId");
	                }

	                if (tokenVal == "")
	                    continue;

	                query += tokenVal + " ";

	                if (tokenFilterId == tokenVal)
	                    encodedQuery += encodedQueryValueIdentifier + tokenVal + encodedQueryValueIdentifier + encodedQueryTokenSeparator;
	                else
	                    encodedQuery += tokenFilterId + encodedQueryTokenSeparator;
	            }
	        }

	        if (encodedQuery.charAt(encodedQuery.length - 1) == encodedQueryTokenSeparator)
	            encodedQuery = encodedQuery.slice(0, encodedQuery.length - 1);

	        return [query.trim(), encodedQuery.trim()];
	    }

	    function readCompleteQuery() {
	        var completeQuery = "";
	        var encodedQueryElems = $(".complete-query-section .encoded-query", baseContainer)
	        var encodedQueries = [];

	        for (var i = 0; i < encodedQueryElems.length; i++) {
	            //completeQuery += $(encodedQueryElems[i]).text() + encodedQueryPartsSeparator;
	            encodedQueries.push($(encodedQueryElems[i]).text());
	        }

	        completeQuery = encodedQueries.join(encodedQueryPartsSeparator);

	        return completeQuery;
	    }

	    function readCompleteQueryText() {
	        var completeQuery = "";
	        var queryElems = $(".complete-query-section .text-query", baseContainer)
	        var query = [];

	        for (var i = 0; i < queryElems.length; i++) {
	            //completeQuery += $(queryElems[i]).text();
	            query.push($(queryElems[i]).text());
	        }
	        completeQuery = query.join(encodedQueryPartsSeparator);

	        return completeQuery;
	    }

	    function readSavedSearchID() {
	        var savedSearchID = "";
	        var queryElems = $(".complete-query-section .text-query", baseContainer)

	        for (var i = 0; i < queryElems.length; i++) {
	            if ($(queryElems[i]).attr('savedSearchID') != 'undefined' && $(queryElems[i]).attr('savedSearchID') != undefined) {
	                savedSearchID = $(queryElems[i]).attr('savedSearchID');
	                break;
	            }
	        }
	        return savedSearchID;
	    }

	    function editQueryToken(tokenRef) {
	        var editableQueryToken = $(".query-token.editable", baseContainer);

	        if ($(".query-token-editable", editableQueryToken).val() != "" && $(".query-token-editable", editableQueryToken).parents(".query-token-parameter").length == 0) {
	            addQueryToken(editableQueryToken, $(".query-token-editable", editableQueryToken).val());
	        }

	        var result = getFilterListFromQueryToken(tokenRef);
	        filters = result[1];
	        //fillSuggestionsFromFilters(filters);
	        if ($(tokenRef).parents(".query-token-parameter").length > 0) {
	            fillSuggestionsForFilter(filters);
	        }
	        else if (result[0] != undefined) {
	            fillSuggestionsForFilter(result[0]);
	            resetPlaceholder(result[0].HelpText);
	        }
	        else
	            fillSuggestionsForFilter(filters);

	        $(tokenRef).after(editableQueryToken);
	        if ($(".query-token-parameter", tokenRef).length > 0)
	            $(".query-token-editable", editableQueryToken).val($(tokenRef)[0].childNodes[0].nodeValue);
	        else
	            $(".query-token-editable", editableQueryToken).val($(tokenRef).text());

	        if ($(tokenRef).parents(".query-token-parameter").length > 0) {
	            curFunctionParamN = $(tokenRef).parents(".query-token-parameter").index() + 1;
	            var functionTokenRef = $(tokenRef).parents(".query-token");
	            var curFuntionPrecedingFilter = getFilterListFromQueryToken(functionTokenRef)[0];
	            curFunction = findFilterFromFilterName(curFuntionPrecedingFilter.nextFilters(), $(functionTokenRef)[0].childNodes[0].nodeValue);
	        }

	        $(tokenRef).remove();
	        $(".query-token-editable", editableQueryToken).focus();

	        adjustTextInputWidth($(".query-token-editable", editableQueryToken));
	    }

	    function getFunctionParameterInfo(parameters, parametersPrototype) {
	        var paramInfo = {
	            Parameters: [],
	            paramSeparator: "",
	            paramBracket: ["", ""]
	        };

	        if (parametersPrototype.charAt(0) != '{')
	            paramInfo.paramBracket[0] = parametersPrototype.charAt(0);
	        if (parametersPrototype.charAt(parametersPrototype.length - 1) != '}')
	            paramInfo.paramBracket[1] = parametersPrototype.charAt(parametersPrototype.length - 1);
	        if (parameters.length == 1) {
	            paramInfo.Parameters = parameters;
	            return paramInfo;
	        }

	        var proto = parametersPrototype;
	        var paramN = 0;

	        while (paramN < parameters.length) {
	            var startIndex = proto.indexOf('{');
	            var endIndex = proto.indexOf('}');
	            var param = proto.slice(startIndex + 1, endIndex);
	            paramInfo.Parameters.push(SRM_jsonDictionary.getObjectByKey(parameters, param));
	            proto = proto.replace("{" + param + "}", "");
	            paramN++;
	        }

	        paramInfo.paramSeparator = proto;

	        return paramInfo;
	    }

	    var curFunction = null;
	    var curFunctionParamN = 0;
	    var referenceAttributeFilter = null;

	    function addQueryToken(queryTokenRef, tokenText, allowManualEntry) {
	        var queryToken = $("<div>").addClass("query-token").text(tokenText);
	        var addedFilter = null;

	        if ($(queryTokenRef).hasClass("query-token-parameter")) {
	            $("span.param-placeholder", queryTokenRef).after(queryToken);
	            addedFilter = curFunction;
	            if (addedFilter != null) {
	                var paramInfo = getFunctionParameterInfo(addedFilter.Parameters, addedFilter.ParameterPrototype);
	                //if (Object.values(addedFilter.parameters)[curFunctionParamN - 1] == null) {
	                if (SRM_jsonDictionary.getAllValues(paramInfo.Parameters)[curFunctionParamN - 1] == null || SRM_jsonDictionary.getAllValues(paramInfo.Parameters)[curFunctionParamN - 1].length == 0) {
	                    $(queryToken).attr("filterId", $(queryToken).text());
	                }
	                else {
	                    //var filter = searchInFilterLists($(queryToken).text(), Object.values(addedFilter.parameters)[curFunctionParamN - 1]);
	                    var filter = searchInFilterLists($(queryToken).text(), SRM_jsonDictionary.getAllValues(paramInfo.Parameters)[curFunctionParamN - 1]);
	                    $(queryToken).attr("filterId", filter[0].FilterID);
	                }
	            }
	        }
	        else if (allowManualEntry == undefined || allowManualEntry == false) {
	            $(queryTokenRef).before(queryToken);
	            addedFilter = findFilterFromFilterName(filters, tokenText);
	            $(queryToken).attr("filterId", addedFilter.FilterID);
	            $(queryToken).attr("keywordType", addedFilter.KeywordType);
	            if (addedFilter.IsReferenceAttribute == true) {
	                referenceAttributeFilter = addedFilter;
	                //var refValues = referenceAttributeFilter.referenceValues();
	            }
	            if (addedFilter.FilterID == -100) {
	                $(queryToken).attr("filterId", $(queryToken).text());
	            }
	        }
	        else if (allowManualEntry != undefined && allowManualEntry == true) {
	            $(queryTokenRef).before(queryToken);
	            addedFilter = findFilterFromFilterName(filters, manualEntryFilterName);
	            $(queryToken).attr("filterId", $(queryToken).text());
	        }

	        //handling for functions
	        //if (addedFilter != null && addedFilter.hasOwnProperty("parameters")) {
	        if (addedFilter != null && addedFilter.hasOwnProperty("Parameters") && addedFilter.Parameters != null && addedFilter.Parameters.length > 0) {
	            if (curFunction == null) {
	                curFunction = addedFilter;
	                //var params = Object.keys(addedFilter.parameters);
	                var paramInfo = getFunctionParameterInfo(addedFilter.Parameters, addedFilter.ParameterPrototype);
	                var params = SRM_jsonDictionary.getAllKeys(paramInfo.Parameters);
	                for (var i = 0; i < params.length; i++) {
	                    var html = "<span class=\"param-placeholder\">" + params[i] + "</span>";

	                    if (paramInfo.hasOwnProperty("paramBracket") && (i == 0 || i == params.length - 1)) {
	                        if (i == 0)
	                            html = "<span class=\"params-start\">" + paramInfo.paramBracket[0] + "</span>" + html;
	                        if (i == params.length - 1) {
	                            html = html + "<span class=\"params-end\">" + paramInfo.paramBracket[1] + "</span>";
	                        }
	                    }

	                    if (paramInfo.hasOwnProperty("paramSeparator") && params.length > 1 && i < params.length - 1) {
	                        html += "<span class=\"param-separator\">" + paramInfo.paramSeparator + "</span>";
	                    }

	                    var paramElem = $("<div>").addClass("query-token-parameter").html(html);
	                    $(queryToken).append(paramElem);
	                }
	            }
	            //if (curFunctionParamN < Object.values(addedFilter.parameters).length) {
	            if (curFunctionParamN < addedFilter.Parameters.length) {
	                //filters = Object.values(addedFilter.parameters)[curFunctionParamN];
	                var paramInfo = getFunctionParameterInfo(addedFilter.Parameters, addedFilter.ParameterPrototype);
	                filters = paramInfo.Parameters[curFunctionParamN].nextFilters;
	                if (filters != null) {
	                    //fillSuggestionsFromFilters(filters);
	                    fillSuggestionsForFilter(filters);
	                }
	                else {
	                    //fillSuggestionsFromFilters([]);
	                    fillSuggestionsForFilter([]);
	                }

	                if ($(queryTokenRef).hasClass("query-token-parameter")) {
	                    $("span.param-placeholder", $(queryTokenRef).next()).after($(".query-token.editable", baseContainer));
	                    $(".query-token-editable", baseContainer).attr("placeholder", $("span.param-placeholder", $(queryTokenRef).next()).text());
	                    $("span.param-placeholder", $(queryTokenRef).next()).addClass("hidden");
	                }
	                else {
	                    $(".query-token-parameter:nth(" + curFunctionParamN + ") span.param-placeholder", queryToken).after($(".query-token.editable", baseContainer));
	                    $(".query-token-editable", baseContainer).attr("placeholder", $(".query-token-parameter:nth(" + curFunctionParamN + ") span.param-placeholder", queryToken).text());
	                    $(".query-token-parameter:nth(" + curFunctionParamN + ") span.param-placeholder", queryToken).addClass("hidden");
	                }
	                curFunctionParamN++;
	                $(".query-token-editable", baseContainer).val("");
	                $(".query-token-editable", baseContainer).focus();
	                adjustTextInputWidth($(".query-token-editable", baseContainer));
	                return $(queryToken);
	            }
	            $(".query-box", baseContainer).append($(".query-token.editable", baseContainer));
	            $(".query-token-editable", baseContainer).removeAttr("placeholder");
	        }

	        if (addedFilter != null && addedFilter.hasOwnProperty("Filters")) {
	            curFunction = null;
	            curFunctionParamN = 0;
	            filters = addedFilter.nextFilters();

	            var addedFiltersList = fillSuggestionsForFilter(addedFilter);
	            if (referenceAttributeFilter != null && addedFilter.KeywordType == "Operator") {
	                filters = filters.concat(addedFiltersList);
	                referenceAttributeFilter = null;
	            }

	            $(".query-token-editable").attr("placeholder", "");
	            if (addedFilter.hasOwnProperty("HelpText"))
	                $(".query-token-editable", baseContainer).attr("placeholder", addedFilter.HelpText);
	        }
	        $(".query-token-editable", baseContainer).focus();
	        $(".query-token-editable", baseContainer).val("");
	        adjustTextInputWidth($(".query-token-editable", baseContainer));

	        return $(queryToken);
	    }

	    function adjustTextInputWidth(inputText) {
	        var value = $(inputText).val();
	        if (value == "" && $(inputText).attr("placeholder") != undefined) {
	            value = $(inputText).attr("placeholder");
	        }
	        var size = value.length;
	        var charWidth = 10;
	        size = size * charWidth;
	        $(inputText).css('width', size);
	    }

	    function fillSuggestionsForFilter(filter, skipClear) {
	        var parent = $(".token-suggestions", baseContainer);
	        if (skipClear == undefined || skipClear == false)
	            $(parent).html("");
	        $(parent).removeClass("hidden");
	        var nextFilters;

	        if (filter == null) {
	            $(parent).addClass("hidden");
	            return;
	        }

	        if (Array.isArray(filter)) {
	            nextFilters = filter;
	        }
	        else {
	            nextFilters = filter.nextFilters();
	        }

	        if (nextFilters == null || nextFilters.length == 0) {
	            $(parent).addClass("hidden");
	            return;
	        }

	        nextFilters.sort(function (a, b) {
	            if (a.FilterName < b.FilterName) return -1;
	            if (a.FilterName > b.FilterName) return 1;
	            return 0;
	        });

	        var refVals;
	        if (referenceAttributeFilter != null && filter.KeywordType == "Operator") {
	            refVals = referenceAttributeFilter.referenceValues();
	            nextFilters = nextFilters.concat(refVals);
	        }

	        var sectionsMap = {};
	        var totalWidthOfSections = 0;
	        var filterSections = getSuggestionSectionsFromFilterList(nextFilters);
	        if (filterSections.indexOf('Function') > -1 && filterSections.indexOf('Function') == 0)
	            filterSections.reverse();
	        for (var i = 0; i < filterSections.length; i++) {
	            var section = $("<div>").addClass("token-suggestion-section").attr("section", filterSections[i]);
	            sectionsMap[filterSections[i]] = section;
	            $(parent).append(section);
	            if (i < filterSections.length - 1) {
	                var separatorsection = $("<div>").addClass("token-suggestion-section-separator");
	                $(parent).append(separatorsection);
	            }
	            totalWidthOfSections += ($(section).width() + 5);
	        }
	        //	        $(parent).width(totalWidthOfSections);

	        for (var i = 0; i < nextFilters.length; i++) {
	            var filterList = nextFilters[i];
	            var tokenSuggestion = $("<div>").addClass("token-suggestion").attr("keywordType", filterList.KeywordType).text(filterList.FilterName);
	            if (filterList.hasOwnProperty("Hidden") && filterList.Hidden == true)
	                $(tokenSuggestion).addClass("hidden");

	            if (filterList.ParentIndex != null && filterList.ParentIndex != "") {
	                var selectedUniverse = findFilterFromFilterName(universeFilters, $(".query-token[keywordtype='Universe']", $(parent).parents(".query-box")).text());
	                if (filterList.ParentIndex != selectedUniverse.IndexName)
	                    continue;
	            }
	            if (filterList.OperatorType != null && filterList.OperatorType != "" && Array.isArray(filter) == false) {
	                if (filterList.OperatorType != filter.DataType)
	                    continue;
	            }

	            $(sectionsMap[filterList.KeywordType]).append(tokenSuggestion);
	        }

	        for (var i = 0; i < Object.values(sectionsMap).length; i++) {
	            var section = Object.values(sectionsMap)[i];
	            var visibleSuggestions = $(".token-suggestion:not('.hidden'):not('.filtered-out')", section);
	            if (visibleSuggestions.length == 0) {
	                $(section).addClass("hidden");
	                totalWidthOfSections -= ($(section).width() + 5);
	                if ($(section).next().length >= 1)
	                    $(section).next().addClass('hidden');
	            }
	        }
	        $(parent).width(totalWidthOfSections);
	        attachEventHandlers();
	        $(".token-suggestion", parent).removeClass("active");

	        return nextFilters;
	    }

	    function getSuggestionSectionsFromFilterList(filterList) {
	        var sections = [];

	        for (var i = 0; i < filterList.length; i++) {
	            var keywordType = filterList[i].KeywordType;
	            if (sections.indexOf(keywordType) == -1)
	                sections.push(keywordType);
	        }

	        return sections;
	    }

	    //	    function fillSuggestionsFromFilters(filtersList, skipClear) {
	    //	        var parent = $(".token-suggestions", baseContainer);
	    //	        $(parent).css('width', '');
	    //	        if (skipClear == undefined || skipClear == false)
	    //	            $(parent).html("");

	    //	        if (filtersList == null)
	    //	            return;

	    //	        for (var i = 0; i < filtersList.length; i++) {
	    //	            if (Array.isArray(filtersList[i])) {
	    //	                fillSuggestionsFromFilters(filtersList[i], true);
	    //	                continue;
	    //	            }

	    //	            var tokenSuggestion = $("<div>").addClass("token-suggestion").attr("keywordType", filtersList[i].KeywordType).text(filtersList[i].FilterName);
	    //	            if (filtersList[i].hasOwnProperty("Hidden") && filtersList[i].Hidden == true)
	    //	                $(tokenSuggestion).addClass("hidden");
	    //	            $(parent).append(tokenSuggestion);
	    //	        }

	    //	        attachEventHandlers();
	    //	    }

	    function findFilterFromFilterName(filterList, filterName) {
	        for (var i = 0; i < filterList.length; i++) {
	            if (Array.isArray(filterList[i])) {
	                var val = findFilterFromFilterName(filterList[i], filterName);
	                if (val == null)
	                    continue;
	                return val;
	            }

	            if (filterList[i].FilterName == filterName)
	                return filterList[i];
	        }
	        return null;
	    }

	    function getFilterListFromQueryToken(token) {
	        var tokens = [];
	        if ($(token).parents(".query-token-parameter").length > 0) {
	            var queryToken = $(token).parents(".query-token")[0];
	            tokens.push($(token)[0]);
	            tokens.push(queryToken);
	            var prevTokens = $(queryToken).prevAll('.query-token');
	            for (var i = 0; i < prevTokens.length; i++) {
	                tokens.push(prevTokens[i]);
	            }
	        }
	        else
	            tokens = $(token).prevAll('.query-token');

	        var curFilters = universeFilters;
	        var filter;

	        for (var i = tokens.length - 1; i >= 0; i--) {
	            var curToken = tokens[i];
	            if ($(curToken).hasClass("editable"))
	                continue;

	            if ($(".query-token-parameter", curToken).length > 0) {
	                //query += $(token)[0].childNodes[0].nodeValue;
	                var tokenText = $(curToken)[0].childNodes[0].nodeValue;
	                filter = findFilterFromFilterName(curFilters, tokenText);
	            }
	            else if ($(curToken).parents(".query-token-parameter").length > 0) {
	                var paramName = $(".param-placeholder", $(curToken).parent()).text();

	                var paramInfo = getFunctionParameterInfo(filter.Parameters, filter.ParameterPrototype);
	                curFilters = SRM_jsonDictionary.getObjectByKey(paramInfo.Parameters, paramName).nextFilters;

	                //	                filters = paramInfo.Parameters[curFunctionParamN].nextFilters;
	                continue;
	            }
	            else {
	                filter = findFilterFromFilterName(curFilters, $(curToken).text());
	                if (filter == null)
	                    filter = findFilterFromFilterName(curFilters, manualEntryFilterName);
	            }

	            if (curToken == token)
	                return curFilters;

	            curFilters = filter.nextFilters();
	        }

	        return [filter, curFilters];
	    }

	    function parseQuery(encodedQuery, startFilter) {
	        /*
	        
	        */

	        var queryTokenRef = $(".query-token.editable", baseContainer);
	        var curFilter = startFilter;
	        var curFilterList;
	        var simpleTokens = encodedQuery.split(encodedQueryTokenSeparator);
	        var curFunction = null;
	        var curParamN = 0;
	        var paramTokens;

	        if (Array.isArray(startFilter))
	            curFilterList = [startFilter];

	        for (var i = 0; i < simpleTokens.length; i++) {
	            queryToken = simpleTokens[i];

	            if (curFunction != null) {
	                queryToken = paramTokens[curParamN];

	                //if (curParamN < Object.values(curFunction.parameters).length && Object.values(curFunction.parameters)[curParamN] != null)
	                var paramInfo = getFunctionParameterInfo(curFunction.Parameters, curFunction.ParameterPrototype);
	                if (curParamN < paramInfo.Parameters.length && SRM_jsonDictionary.getAllValues(paramInfo.Parameters)[curParamN] != null) {
	                    //curFilterList = Object.values(curFunction.parameters)[curParamN++];
	                    curFilterList = SRM_jsonDictionary.getAllValues(paramInfo.Parameters)[curParamN++];
	                }
	                //else if (curParamN >= Object.values(curFunction.parameters).length) {
	                else if (curParamN >= paramInfo.Parameters.length) {
	                    curFilterList = curFunction.nextFilters();
	                    curFunction = null;
	                    curParamN = 0;
	                    paramTokens = [];
	                    if (i == simpleTokens.length - 1)
	                        break;
	                    continue;
	                }
	                else
	                    curParamN++;
	            }

	            var filter = searchFilterIdInFilterLists(queryToken, curFilterList);
	            var tokenText;
	            var requireManualEntry = false;

	            if (curFunction != null && filter != null) {
	                tokenText = filter.FilterName;
	            }
	            else if (filter != null) {
	                curFilter = filter;
	                curFilterList = curFilter.nextFilters();
	                tokenText = curFilter.FilterName;
	                //1§29§8§30§15§¦0.9¦‡¦1.2¦
	                //if (curFilter.hasOwnProperty("parameters") && curFilter.parameters != null) {
	                if (curFilter.hasOwnProperty("Parameters") && curFilter.Parameters != null && curFilter.Parameters.length > 0) {
	                    curFunction = curFilter;
	                    if (curFilter.KeywordType != "Operator")
	                        paramTokens = simpleTokens[i + 1].replace(encodedQueryParamBrackets[0], "").replace(encodedQueryParamBrackets[1], "").split(encodedQueryParamsSeparator);
	                    else
	                        paramTokens = simpleTokens[i + 1].split(encodedQueryParamsSeparator);

	                    simpleTokens.splice(i + 1, 1);
	                }
	                else
	                    curFunction = null;
	            }
	            else {
	                tokenText = queryToken.slice(1, queryToken.length - 1);
	                //tokenText = tokenText.split("&nbsp;").join(" ");
	                for (var j = 0; j < curFilterList.length; j++) {
	                    if (curFilterList[j].KeywordType == manualEntryKeywordType) {
	                        requireManualEntry = true;
	                        curFilterList = curFilterList[j].nextFilters();
	                        break;
	                    }
	                }
	            }

	            if ($(queryTokenRef).parents(".query-token-parameter").length > 0) {
	                addQueryToken($(queryTokenRef).parents(".query-token-parameter")[0], tokenText, requireManualEntry);
	            }
	            else {
	                addQueryToken(queryTokenRef, tokenText, requireManualEntry);
	            }

	            if (curFunction != null) {
	                i--;
	            }
	        }
	    }

	    function searchFilterIdInFilterLists(filterId, filterLists) {
	        for (var i = 0; i < filterLists.length; i++) {
	            var filterList = filterLists[i];
	            if (Array.isArray(filterList)) {
	                for (var j = 0; j < filterList.length; j++) {
	                    if (filterList[j].Hidden == false && filterList[j].FilterID == filterId) {
	                        return filterList[j];
	                    }
	                }
	            }
	            else if (filterList.Hidden == false && filterList.FilterID == filterId) {
	                return filterList;
	            }
	        }

	        return null;
	    }

	    function searchInFilterLists(searchText, filterLists) {
	        var filteredSuggestions = [];

	        for (var i = 0; i < filterLists.length; i++) {
	            var filterList = filterLists[i];
	            if (Array.isArray(filterList)) {
	                for (var j = 0; j < filterList.length; j++) {
	                    if (filterList[j].FilterName.toLowerCase().indexOf(searchText.toLowerCase()) != -1) {
	                        filteredSuggestions.push(filterList[j]);
	                    }
	                }
	            }
	            else if (filterList.FilterName.toLowerCase().indexOf(searchText.toLowerCase()) != -1) {
	                filteredSuggestions.push(filterList);
	            }
	        }

	        return filteredSuggestions;
	    }

	    function resetPlaceholder(placeholderText) {
	        var text;
	        if (placeholderText == undefined)
	            text = "Query your universe";
	        else
	            text = placeholderText;
	        $(".query-token-editable", baseContainer).attr("placeholder", text);
	        adjustTextInputWidth($(".query-token-editable", baseContainer));
	    }

	    function displayPopup(message, status, closeBtnEvent) {
	        var imageSrc = "";
	        var borderTop = "";
	        var title = "";
	        var color = "";
	        switch (status) {
	            case "success":
	                imageSrc = "/CommonUI/images/icons/pass_icon.png";
	                borderTop = "4px solid #ACD373";
	                title = "Success";
	                color = "#ACD373";
	                break;
	            case "failure":
	                imageSrc = "/CommonUI/images/icons/fail_icon.png";
	                borderTop = "4px solid #C7898C";
	                title = "Failure";
	                color = "#C7898C";
	                break;
	            case "alert":
	                imageSrc = "/CommonUI/images/icons/alert_icon.png";
	                borderTop = "4px solid #F4AD02";
	                title = "Alert";
	                color = "#F4AD02";
	                break;
	        }

	        var left = ($(window).width() / 2) - 200;
	        var msgPopup = $("#srm_quant_messagePopup");
	        msgPopup.find(".srmQuantStatuspopupTitle").text(title).css("color", color);
	        msgPopup.find(".srmQuantStatusMessage").text(message);
	        msgPopup.find("img").attr("src", imageSrc);
	        msgPopup.css("border-top", borderTop);
	        msgPopup.css("left", left + "px");
	        msgPopup.css("top", "-" + msgPopup.height() + "px");
	        msgPopup[0].style.display = "";
	        msgPopup.animate({ top: "0px" }, 500);

	        msgPopup.find(".srmQuantPopupCloseBtn").unbind("click").bind("click", function (e) {
	            msgPopup.animate({ top: "-" + msgPopup.height() + "px" }, 500, function () {
	                msgPopup[0].style.display = "none";
	            });

	            if (typeof closeBtnEvent !== "undefined") {
	                closeBtnEvent();
	            }
	        });
	    }

	    function getSavedSearches() {

	        $.ajax({
	            type: 'POST',
	            url: "BaseUserControls/Service/CommonService.svc/GetQuantSavedSearches",
	            contentType: "application/json",
	            dataType: "json",
	            data: JSON.stringify({ userName: "admin" }),
	            success: function (data) {

	                $('#SRM-savedSearchListDiv').html('');
	                var savedSearchJSON = data.d;

	                $.each(savedSearchJSON, function (index, item) {
	                    item.SearchQuery = item.SearchQuery.replace('‰', '</br>')
	                    var $mainDiv = $('<div id="savedSearchItemDiv_' + item.SearchId + '" class="SRM-savedSearchItem"></div>');
	                    var $seachNameDiv = $('<div class="savedSearchNameItem" id="savedSearchNameItem_' + item.SearchId + '"><span class="SRM-searchNameText" searchCode="' + item.SearchEncodedQuery + '" style="display:block;">' + item.SearchName + '</span></div>');
	                    var $textDiv = $('<div class="savedTextItem" id="savedSearchItem_' + item.SearchId + '"><span class="SRM-searchText" id = "SRM-searchText_' + item.SearchId + '" searchCode="' + item.SearchEncodedQuery + '" style="display:block;">' + item.SearchQuery + '</span></div>');

	                    var $deleteDiv = $('<div class="savedDeleteItem" id="savedDeleteItem_' + item.SearchId + '"></div');
	                    var $deleteIcon = $('<i class="fa fa-trash-o SRM-defaultScreenTrashBin"></i>');
	                    $deleteDiv.append($deleteIcon);

	                    $seachNameDiv.appendTo($mainDiv);
	                    $textDiv.appendTo($mainDiv);
	                    $deleteDiv.appendTo($mainDiv);
	                    $mainDiv.appendTo('#SRM-savedSearchListDiv');

	                    $mainDiv.hover(function () {
	                        $('#savedDeleteItem_' + item.SearchId + ' i').css('visibility', 'visible');
	                        $('#savedDeleteItem_' + item.SearchId + ' i').css('opacity', '1');

	                    }, function () {
	                        $('#savedDeleteItem_' + item.SearchId + ' i').css('visibility', '');
	                        $('#savedDeleteItem_' + item.SearchId + ' i').css('opacity', '');

	                    });

	                    $textDiv.click(function (e) {
	                        //console.log(e);
	                        var encodedQuery = '', query = '', searchId = '';
	                        if ($(e.target).attr('class') == 'SRM-searchText') {
	                            encodedQuery = $(e.target).attr('searchCode');
	                            query = $(e.target).innerHTML().replace('&gt;', '>').replace('&lt;', '<');
	                            searchId = $(e.target).attr('id').split('SRM-searchText_')[1];
	                        }
	                        else {
	                            encodedQuery = $(e.target).find('.SRM-searchText').attr('searchCode');
	                            query = $(e.target).find('.SRM-searchText').innerHTML().replace('&gt;', '>').replace('&lt;', '<');
	                            searchId = $(e.target).find('.SRM-searchText').attr('id').split('SRM-searchText_')[1];
	                        }

	                        callQuantService(encodedQuery);

	                        var find = '&gt;';
	                        var re = new RegExp(find, 'g');
	                        query = query.replace(re, '>');

	                        find = '&lt;';
	                        var re = new RegExp(find, 'g');
	                        query = query.replace(re, '<');

	                        //Check if encoded query is two queries concatenated, loop below statements
	                        if (encodedQuery.indexOf('‰') > 1) {
	                            for (var index = 0; index < encodedQuery.split('‰').length; index++) {

	                                var queryElem = getNewQueryElem(query.split('<br>')[index], encodedQuery.split('‰')[index], getNewQueryId(), searchId);
	                                $(".complete-query-section", baseContainer).append(queryElem);
	                                if (index != encodedQuery.split('‰').length - 1)
	                                    $(".complete-query-section", baseContainer).append($("<div class='fa fa-chevron-circle-down' style='color:grey;font-size: 14px;padding-top: 5px;'>"));

	                                //	                                var queryElem = $("<div>").addClass("query").attr("queryId", queryIdCounter++).text(query.split('‰')[index]);
	                                //	                                var encodedQueryElem = $("<div>").addClass("encoded-query").addClass("hidden").text(encodedQuery.split('‰')[index]);
	                                //	                                $(queryElem).append(encodedQueryElem);
	                                //	                                $(".complete-query-section .add-query-section-btn", baseContainer).before(queryElem);
	                            }
	                        }
	                        else {

	                            var queryElem = getNewQueryElem(query, encodedQuery, getNewQueryId(), searchId);
	                            $(".complete-query-section", baseContainer).append(queryElem);

	                            //	                            var queryElem = $("<div>").addClass("query").attr("queryId", queryIdCounter++).text(query);
	                            //	                            var encodedQueryElem = $("<div>").addClass("encoded-query").addClass("hidden").text(encodedQuery);
	                            //	                            $(queryElem).append(encodedQueryElem);
	                            //	                            $(".complete-query-section .add-query-section-btn", baseContainer).before(queryElem);
	                        }

	                        $(".complete-query-section .query .text-query", baseContainer).removeClass("hidden");
	                        $(".buttons-section", baseContainer).removeClass("hidden");
	                        attachEventHandlers();
	                        //	                        $(".complete-query-section .query", baseContainer).removeClass("hidden");
	                        //	                        attachEventHandlers();

	                        //	                        $(".result-section", baseContainer).removeClass("hidden");
	                        //	                        $(".complete-query-section", baseContainer).removeClass("hidden");
	                        //	                        $(".query-section", baseContainer).addClass("hidden");

	                    });

	                    $deleteDiv.click(function (event) {
	                        var savedSearchID = $(event.target).parents('.SRM-savedSearchItem').find('.SRM-searchText').attr('id').split('SRM-searchText_')[1]

	                        $.ajax({
	                            type: 'POST',
	                            url: "BaseUserControls/Service/CommonService.svc/DeleteQuantSavedSearch",
	                            contentType: "application/json",
	                            dataType: "json",
	                            data: JSON.stringify({ searchID: savedSearchID, userName: "admin" }),
	                            success: function (data) {
	                                if (data.d.IsSuccess == true)
	                                    displayPopup(data.d.Message, "success", getSavedSearches);
	                                else
	                                    displayPopup(data.d.Message, "failure");
	                            },
	                            error: function () {
	                                displayPopup("Searches could not be deleted", "failure");
	                                console.log("Searches could not be deleted");
	                            }
	                        });
	                    });

	                });
	                $('.SRM-savedSearchItem:nth-child(odd)').addClass('backgrnd_watermark_bar');
	                $('.SRM-savedSearchItem:nth-child(even)').addClass('backgrnd_watermark_point');
	            },
	            error: function () {
	                console.log("Saved searches data cannot be fetched");
	            }
	        });

	        //	        var savedSearchJSON = [
	        //                                    {
	        //                                        "SearchId": "820",
	        //                                        "SearchName": "SPX constituents",
	        //                                        "SearchQuery": "SPX constituents with Beta > 1",
	        //                                        "SearchEncodedQuery": "1§29§8§30§9§¦1¦"
	        //                                    },
	        //                                    {
	        //                                        "SearchId": "821",
	        //                                        "SearchName": "SPX constituents",
	        //                                        "SearchQuery": "SPX constituents with Beta between 0.9 and 1.2 and GICS Sector as Financials, Information Technology",
	        //                                        "SearchEncodedQuery": "1§29§8§30§9§¦1¦"
	        //                                    },
	        //                                    {
	        //                                        "SearchId": "822",
	        //                                        "SearchName": "SPX constituents",
	        //                                        "SearchQuery": "SPX constituents with Beta between 0.9 and 1.2 and GICS Sector as Financials, Information Technology AND SPX constituents with Beta between 0.9 and 1.2",
	        //                                        "SearchEncodedQuery": "1§29§8§30§9§¦1¦"
	        //                                    },
	        //                                    {
	        //                                        "SearchId": "823",
	        //                                        "SearchName": "SPX constituents",
	        //                                        "SearchQuery": "SPX constituents with Beta between 0.9 and 1.2 and GICS Sector as Financials, Information Technology",
	        //                                        "SearchEncodedQuery": "1§29§8§30§9§¦1¦"
	        //                                    },
	        //                                    {
	        //                                        "SearchId": "824",
	        //                                        "SearchName": "",
	        //                                        "SearchQuery": "SPX constituents with Beta between 0.9 and 1.2 and GICS Sector as Financials, Information Technology",
	        //                                        "SearchEncodedQuery": "1§29§8§30§9§¦1¦"
	        //                                    },
	        //                                    {
	        //                                        "SearchId": "825",
	        //                                        "SearchName": "",
	        //                                        "SearchQuery": "SPX constituents with Beta between 0.9 and 1.2 and GICS Sector as Financials, Information Technology AND SPX constituents with Beta between 0.9 and 1.2",
	        //                                        "SearchEncodedQuery": "1§29§8§30§9§¦1¦"
	        //                                    }
	        //                                ];


	    }

	    return {
	        init: function (filtersList) {
	            baseContainer = baseContainerRef;
	            bindDatePicker();
	            attachEventHandlers();
	            filters = filtersList;
	            //fillSuggestionsFromFilters(filters);
	            fillSuggestionsForFilter(filters);
	            //$(".result-section", baseContainer).addClass("hidden");
	            resetPlaceholder();
	            getSavedSearches();
	            adjustTextInputWidth($(".query-token-editable", baseContainer));
	        },
	        searchNextFilter: function (rootFilterList, currentFilter) {
	            var filterList = [];
	            var filterIdsToSearch = [];

	            var currentFilterFilters = currentFilter;
	            if (!Array.isArray(currentFilter))
	                currentFilterFilters = currentFilter.Filters;

	            for (var i = 0; i < currentFilterFilters.length; i++) {
	                filterIdsToSearch.push(currentFilterFilters[i].FilterID);
	            }

	            for (var i = 0; i < rootFilterList.length; i++) {
	                var filter = rootFilterList[i];

	                if (filterIdsToSearch.indexOf(filter.FilterID) != -1)
	                    filterList.push(filter);
	            }

	            return filterList;
	        },
	        getFiltersFromService: function (currentFilter, manualEntryFilter) {
	            var data = $.ajax({
	                type: 'POST',
	                url: "BaseUserControls/Service/CommonService.svc/GetReferenceData",
	                contentType: "application/json",
	                dataType: "json",
	                data: JSON.stringify({ attributeName: currentFilter.FilterName }),
	                async: false,
	                error: function () {
	                    console.log("Quant Data cannot be fetched");
	                }
	            });
	            for (var i = 0; i < data.responseJSON.d.length; i++) {
	                var refVal = data.responseJSON.d[i];
	                refVal.FilterID = -100;
	                refVal.Hidden = false;
	                refVal.KeywordType = "REFERENCEVALUE";
	                refVal["nextFilters"] = function () { return manualEntryFilter.nextFilters(); }
	            };

	            return data.responseJSON.d;
	        }
	    }
	}

var SRM_QuantJS = new QuantJS($("#quantModule"));

$(document).ready(function () {
    $.ajax({
        type: 'POST',
        url: "BaseUserControls/Service/CommonService.svc/GetQuantIntellisenseDataSM",
        contentType: "application/json",
        dataType: "json",
        success: function (result) {
            console.log("RECEIVED");

            var manualEntryFilter = null;
            var refAttributes = [];
            for (var i = 0; i < result.d.length; i++) {
                if (result.d[i].IsReferenceAttribute == true)
                    refAttributes.push(result.d[i]);

                result.d[i]["nextFilters"] = function () { return SRM_QuantJS.searchNextFilter(result.d, this); };

                if (result.d[i].KeywordType == "Manual Entry") {
                    result.d[i].Hidden = true;
                    manualEntryFilter = result.d[i];
                }
                else if (result.d[i].KeywordType == "Function")
                    result.d[i].DataType = "NUMERIC";

                if (result.d[i].Parameters != null && result.d[i].Parameters.length > 0) {
                    result.d[i]["ParameterPrototype"] = result.d[i].HelpText;
                    result.d[i].HelpText = "";

                    for (var j = 0; j < result.d[i].Parameters.length; j++) {
                        if (result.d[i].Parameters[j].Value.length > 0)
                            result.d[i].Parameters[j]["nextFilters"] = function () { return SRM_QuantJS.searchNextFilter(result.d, result.d[i].Parameters[j].Value); } ();
                        else
                            result.d[i].Parameters[j]["nextFilters"] = null;
                    }
                }
            }

            for (var i = 0; i < refAttributes.length; i++) {
                refAttributes[i]["referenceValues"] = function () { return SRM_QuantJS.getFiltersFromService(this, manualEntryFilter); };
            }

            universeFilters = [];

            for (var i = 0; i < result.d.length; i++) {
                if (result.d[i].KeyWordLevel == 1)
                    universeFilters.push(result.d[i]);
            }

            SRM_QuantJS.init(universeFilters);
        },
        error: function () {
            console.log("Quant Data cannot be fetched");
        }
    });
});