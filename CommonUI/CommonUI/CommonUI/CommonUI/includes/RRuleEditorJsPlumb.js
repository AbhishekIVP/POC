$(function () {
    $.widget("rad-widget.RRuleEditor", {
        options: {
            "existingBlocks": [],
            "ruleText": "",
            "grammarInfo": {},
            "currentBlockTextObj": {},
            "ExternalFunction": function (state) { },
            "serviceUrl": "http://localhost:63520/Resources/Services/RADXRuleEditorService.svc"
        },
        _create: function () { // building the floating Menu and creating the Blocks
            var self = this;
            self.options.idCount = 0;
            self.options.spaceCounter = 0;
            self.options.blockidCheck = "";
            self.options.setRuleText = "";
            self.options.ifelseflag = true;
            self.reAttachedBlocks = false;
            self.options.breakChain = false;
            if (self.options.existingRules == "" && self.options.existingRules == null) {
                self.options.ruleText = "";
            }
            else {
                self.options.ruleText = self.options.ruleText.replace("END", "");
                self.options.existingRules = self.options.ruleText.replace("END", "");
            }
            self.options.ActionCounter = 0;
            self.options.clientGUID = "";
            self.options.ConditionCounter = 0;
            self.options.CLientX = 0;
            self.options.IFBlockIDsStack = [];
            self.options.IFContentIDsStack = [];
            self.options.ElseBlockIdsStack = [];
            self.element.innerHTML = "";
            self.options.CounterArray = [];
            self.options.Counter = 0;
            self.options.isEditMode = false;
            self.options.currentBlockTextObj = {};
            self.options.blockRuleTextMap = [];
            self.keywords = ['if', 'If', 'Then', 'END', 'else', 'Else'];
            self.operators = ['Min', 'Max', 'AddDays', 'AddMonths', 'AddYears', 'IsNull', 'Mod', 'ConvertDateToISO', 'TrimEnd', 'TrimStart', 'Concat', 'PrependString', 'Replace', 'RegexExtract', 'abs', 'round', 'reciprocal', 'AppendString', 'power', 'ToLower', 'ToUpper', 'Substring'];
            self.comparisonop = [' &gt;= ', '&gt;', '&lt;=', '&lt;', 'Equals', 'NotEquals', 'EndsWith', 'Contains', 'StartsWith', 'eq', 'lteq', 'gteq', 'nteq', 'RegexMatch', 'In', 'IsEmpty'];
            self.aggFunc = ["InToNumber", "AVERAGE On:", "CONCAT On:", "DifferenceOn", "MAX On:", "MIN On:", "PRODUCT On:", "SUM On:", "WEIGHTED-AVERAGE On:", "AggregationKeyOn", "Aggregate On:", "NextBusinessDay", "PreviousBusinessDay", "NextEndOfMonth", "PreviousEndOfMonth", "NextEndOfQuarter", "PreviousEndOfQuarter", "PreviousEndofWeek", "PreviousEndofYear", "FirstBussinessDayOfTheMonth", "LastBussinessDayOfTheMonth", "AddBusinessDays"];
            self.ruleIds = Array();
            self.logicalOperators = ['*', '+', '-', '/', '==', '<=', '<', '>', '>=', '!=', '=', '&&', '||', 'AND', 'OR', '!','%'];
            self.brackets = ['{', '[', '(', ')', ']', '}'];
            self.colon = [';', ','];
            //self.element.append("<div id=\"canvas\"></div>")
            //            $(self.settings.grammarInfo.Columns).each(function (i, e) {
            //                ruleIds.push(e.ColumnName);
            //            });
            self.element.append("<div id=\"RRuleEditorBlocksParent\" class=\"RRuleEditorBlocksParent\"></div>")
            //            self.element.find("#RRuleEditorBlocksParent").append("<div id=\"RRuleEditorFloatingMenu\" class=\"RRuleEditorFloatingMenu\"></div>");
            //            self.element.find("#RRuleEditorFloatingMenu").append("<div id=\"RRuleEditorFloatingMenuTabs\" class=\"RRuleEditorFloatingMenuTabs\"></div>")
            //            self.element.find("#RRuleEditorFloatingMenuTabs").append("<div id=\"RRuleEditorFloatingMenuTabsNames\" class=\"RRuleEditorFloatingMenuTabsNames\">B</div>")
            //            self.element.find("#RRuleEditorFloatingMenu").append("<div id=\"RRuleEditorFloatingMenuBlocks\" class=\"RRuleEditorFloatingMenuBlocks\">BLOCKS</div>")
            //            self.element.find("#RRuleEditorFloatingMenu").append("<div id=\"RRuleEditorFloatingMenuBlockType\" class=\"RRuleEditorFloatingMenuBlockType\"></div>")
            //self.element.find("#RRuleEditorBlocksParent").append("<div id=\"RRuleEditorFloatingMenuHeading\" class=\"RRuleEditorFloatingMenu\">BLOCKS</div>");
            self.element.find("#RRuleEditorBlocksParent").append("<div id=\"RRuleEditorFloatingMenuBlocks\" class=\"RRuleEditorFloatingMenuBlocksCentre\"></div>")
            for (var i = 0; i < self.options.existingBlocks.length; i++) {
                self.element.find("#RRuleEditorFloatingMenuBlocks").append("<div class=\"RRuleEditorConditionBlock\"><div class=\"innerText\">" + self.options.existingBlocks[i] + "</div></div>")
            }

            self.element.append("<div id=\"canvas\"></div>")
            self.element.find("#canvas").innerHTML = "";
            self.element.append("<div id=\"prettyText\" class=\"RRuleEditorPrettyText\"><div id=\"RRuleEditorPrettyTextHeader\"><div id=\"fullscreenHeader\" class= \"fullscreenIcon\"></div><div id=\"btnEditorSaveRule\" class=\"RuleEditorSaveButton\">Save</div></div><div id=\"RRuleEditorBlockIDContent\" class=\"RRuleEditorBlockIDContentContent\"></div><div id=\"RRuleEditorPrettyTextContent\" class=\"RRuleEditorPrettyTextContent\"><div tabindex=\"1\" id = \"ruleEditorBlock0\" class=\"ruleEditorFirstBlock ruleEditorBlocks ruleBlocks\"></div></div></div>")
            //            if (self.options.existingRules["blockName"] != undefined) {
            //                self._createExistingRule(self.options.existingRules);
            //            }

            //else
            //self._jsPlumbReady();

            self.element.on("click", ".RRuleEditorblock", function (event) {
                self._showOptionsOnEachBlock(event);
            })

            self.element.on("focusout", ".RRuleEditorRuleTextEditMode", function (event) {
                self._updateBlockNameinprettyText(event);
            })

            self.element.on("click", ".RRuleEditorRuleText", function (event) {
                self._editRuleEditorBlockText(event);
            });

            self.element.on("mouseover", ".RRuleEditorblock", function (event) {
                self._hightLightRuleTextBlock(event);
                // self._showOptionsOnEachBlock(event);
            })

            self.element.on("mouseout", ".RRuleEditorblock", function (event) {
                self._unhightLightRuleTextBlock(event);
            })

            self.element.on("click", ".RRuleEditorDeleteBlock", function (event) {
                self._deleteEachBlock(event);
            })

            self.element.on("click", "#fullscreenHeader", function (event) {
                self._showFullScreen(event);
            })

            self.element.find(".RRuleEditorPrettyText").on("mouseover", ".PrettyTextBlocks", function (event) {
                self._highlightBlock(event);
            });

            self.element.find(".RRuleEditorPrettyText").on("mouseout", ".PrettyTextBlocks", function (event) {
                self._removehighlightBlock(event);
            });
            self.element.find(".RRuleEditorPrettyText").on("mouseover", ".prettyTextRule", function (event) {
                self._showRuleTextDiv(event);
            });
            self.element.find(".RRuleEditorPrettyText").on("mouseout", ".prettyTextRule", function (event) {
                self._removeRuleTextDiv(event);
            });

            self._registerEventHandlers();
            self._initRulePlugin();

        },


        _registerEventHandlers: function () {
            var self = this;
            self.element.click(function (event) {
                if (!$(event.target).hasClass("showHideContent") && !$(event.target).hasClass("ruleEditorFirstBlock") && !$(event.target).hasClass("ruleEditorEditingBlock") && !$(event.target).hasClass("ruleEditorActionBlocks")
                    && !$(event.target).hasClass("ruleEditorParenthesisBlock") && !$(event.target).hasClass("showHideContent") && !$(event.target).hasClass("showHideContent") && !$(event.target).hasClass("showHideContent")
                    && !$(event.target).hasClass("ruleEditorAddDeleteBlock") && !$(event.target).closest("div").hasClass("autoToken")) {
                    if ($("#ruleTxt").length > 0) {
                        self.destroyRuleEditor();
                    }
                }
            });
            self.element.on("click", ".ruleEditorFirstBlock,.ruleEditorEditingBlock,.ruleEditorActionBlocks,.ruleEditorParenthesisBlock", function () {
                if (!$(event.target).hasClass("showHideContent")) {
                    if (!$(event.target).hasClass("addBlock") && !$(event.target).hasClass("deleteBlock")) {
                        if ($(event.target).closest(".ruleEditorEditingBlock").length > 0) {
                            if (!$(event.target).hasClass("ruleEditorIFBlock") && $(event.target).attr('id') != "ruleTxt")
                                self._initiateRuleEditor($(event.target).closest(".ruleEditorEditingBlock"));
                        }
                        else if ($(event.target).hasClass("ruleEditorParenthesisBlock")) {
                            if ($("#ruleTxt").length > 0) {
                                self.destroyRuleEditor();
                            }
                        }
                        else {
                            if (!$(event.target).hasClass("ruleEditorIFBlock") && $(event.target).attr('id') != "ruleTxt")
                                self._initiateRuleEditor($(event.target));
                        }
                    }
                    return false;
                }
            });
            self.element.on("keydown", ".ruleEditorFirstBlock,.ruleEditorEditingBlock,.ruleEditorActionBlocks,.ruleEditorParenthesisBlock", function () {
                //self._setFocusandOpenRuleEditor(event);
                if (event.shiftKey && event.keyCode == 9) {//shift tab
                    alert('hi');
                    if ($(".RRuleEditorPrettyTextContent").find('.RADHighlightBackground').length > 0) {
                        var currentControl = actualControl = $($(".RRuleEditorPrettyTextContent").find('.RADHighlightBackground')[0]);
                        if (currentControl.parent().hasClass("ruleEditorIfActionBlocks"))
                            currentControl = currentControl.parent();
                        while (true) {
                            if (currentControl.prev().length == 0) {
                                if (currentControl.parent() == null) {
                                    break;
                                }
                                else {
                                    currentControl = currentControl.parent();
                                }
                            }
                            if ($(".RRuleEditorPrettyTextContent").has(currentControl).length == 0) {
                                actualControl = $($(".RRuleEditorPrettyTextContent").find('.RADHighlightBackground')[0]);
                                actualControl.addClass('RADHighlightBackground');
                                actualControl.focus();
                                break;
                            }
                            if ($(currentControl.prev()).hasClass('RRuleEditorPrettyTextContent')) {
                                break;
                            }
                            if ($(currentControl.prev()).attr('isjustparent')) {
                                var focusControl = $(currentControl.prev()).find('.ruleEditorActionBlocks').last();
                                focusControl.addClass('RADHighlightBackground');
                                focusControl.focus();
                                break;
                            }
                            if ($(currentControl.prev()).hasClass('ruleEditorActionBlocks')) {
                                var focusControl = currentControl.prev();
                                focusControl.addClass('RADHighlightBackground');
                                focusControl.focus();
                                break;
                            }
                            if ($(currentControl.prev()).hasClass('ruleEditorIfActionBlocks')) {
                                var focusControl = currentControl.prev().find(".ruleEditorEditingBlock");
                                focusControl.addClass('RADHighlightBackground');
                                focusControl.focus();
                                break;
                            }
                            currentControl = currentControl.prev();
                        }
                        actualControl.removeClass("RADHighlightBackground");
                    }
                }
                else if (event.which === 9) {//tab

                    if ($(".RRuleEditorPrettyTextContent").find('.RADHighlightBackground').length > 0) {
                        var currentControl = actualControl = $(".RRuleEditorPrettyTextContent").find('.RADHighlightBackground');
                        if (currentControl.parent().hasClass("ruleEditorIfActionBlocks"))
                            currentControl = currentControl.parent();
                        while (true) {
                            if (currentControl.next().length == 0) {
                                if (currentControl.parent() == null) {
                                    break;
                                }
                                else {
                                    currentControl = currentControl.parent();
                                }
                            }
                            if ($(".RRuleEditorPrettyTextContent").has(currentControl).length == 0) {
                                actualControl = $($(".RRuleEditorPrettyTextContent").find('.RADHighlightBackground')[0]);
                                actualControl.focus();
                                actualControl.addClass('RADHighlightBackground');
                                break;
                            }
                            if ($(currentControl.next()).hasClass('ruleEditorActionBlocks')) {
                                var focusControl = currentControl.next();
                                focusControl.addClass('RADHighlightBackground');
                                focusControl.focus();
                                break;
                            }
                            if ($(currentControl.next()).attr('isjustparent')) {
                                var focusControl = $(currentControl.next()).find('.ruleEditorIfActionBlocks').first();
                                focusControl.addClass('RADHighlightBackground');
                                focusControl.focus();
                                break;
                            }
                            currentControl = currentControl.next();
                        }
                        actualControl.removeClass("RADHighlightBackground");
                    }
                    else {
                        //todo first focus
                    }
                }

                else if (event.keyCode == 13) {//enter
                    if ($(".RRuleEditorPrettyTextContent").find('.RADHighlightBackground').length > 0) {
                        var currentControl = $(".RRuleEditorPrettyTextContent").find('.RADHighlightBackground');
                        if (currentControl.parent().hasClass("ruleEditorIfActionBlocks"))
                            currentControl = currentControl.parent();
                        if (currentControl.attr('isifblock') && currentControl.attr('id') != "ruleTxt") {
                            self._initiateRuleEditor(currentControl.find(".ruleEditorEditingBlock"));
                        }
                        else {
                            if (currentControl.attr('id') != "ruleTxt")
                                self._initiateRuleEditor(currentControl);
                        }
                    }
                    //self._initiateRuleEditor($(event.target));
                }
            });
            self.element.on("click", ".ruleEditorAddDeleteBlock", function () {
                if (event.target.className == "addBlock fa fa-plus-circle") {
                    self._addElseBlock($(event.target));
                }
                else {
                    self._addDeleteIfElseBlock($(event.target));
                }
            })
            self.element.on("click", ".RuleEditorSaveButton", function () {
                //self._saveRule();
                //self.isRuleComplete();
                self.getGeneratedCode();
            });

            self.element.on("click", ".showHideContent", function () {
                self._showHideRegions(event);
            })
            self.element.find("#RRuleEditorPrettyTextContent").children().first().focus();

            //            self.element.find(".ruleEditorFirstBlock,.ruleEditorEditingBlock,.ruleEditorActionBlocks").click(function (event) {
            //                self._initiateRuleEditor(event);
            //            });
        },


        changeRuleText: function (ruleText) {
            var self = this;
            $("#RRuleEditorPrettyTextContent").empty();
            self.options.ruleText = ruleText;
            self._createExistingRuleNew();
        },

        _destroy: function () {
            var self = this;
            self.element.html("");
            self.element.unbind("click");
            self.element.unbind("keydown");
        },

        _showHideRegions: function (event) {
            var self = this;
            if (event.target.className == "showHideContent fa fa-caret-down") {
                event.target.className = "showHideContent fa fa-caret-right";
                self._hideContent(event, true);
            }
            else if (event.target.className == "showHideContent fa fa-caret-right") {
                event.target.className = "showHideContent fa fa-caret-down";
                self._hideContent(event, false);
            }
        },

        _hideContent: function (event, toBeHidden) {
            var self = this;
            var id = $(event.target).closest(".showHideContent").attr('id');
            var stopIndex = 0;
            var element = $(event.target).closest(".ruleEditorActionBlocks,.ruleEditorFirstBlock").parent();
            var length = element.children().length
            for (var i = 1; i < length; i++) {

                if (toBeHidden)
                    element.children()[i].style.display = "none";
                else {
                    element.children()[i].style.display = "";
                }
            }
        },

        highlighFaultBlock: function () {
            var self = this;
            var rule = self.saveRule();

            var correctRuleText = JSON.parse($.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: self.options.serviceUrl + "/GetCorrectRuleText",
                data: JSON.stringify({ cacheKey: self.options.clientGUID, currentRuleText: rule }),
                dataType: "json",
                async: false,
                success: function (response) {

                },
                error: function (response) {
                    console.log("err>", response);
                    return false;
                }
            }).responseText).d;
            if (correctRuleText.trim().endsWith("END") || correctRuleText.trim().endsWith("end"))
                return true;
            else {
                var ruleTextTokens = self._splitRuleTextNew(correctRuleText);
                var element = $(".RRuleEditorPrettyTextContent").children();
                for (var i = 0; i < element.length; i++) {
                    if ($(element[i]).attr("isJustParent") && ruleTextTokens.length > 0) {
                        self._clearArrayItems("", $(element[i]).children(), ruleTextTokens)
                    }
                }
            }
        },
        _clearArrayItems: function (blockid, element, ruleArray) {
            var self = this;
            var length = element.length;
            for (var i = 0; i < length; i++) {
                if ($(element[i]).attr("isJustParent") && ruleArray.length > 0) {
                    self._clearArrayItems(blockid, $(element[i]).children(), ruleArray);
                }
                if (blockid != "") {
                    if ($(element[i]).find("#" + blockid).length == 0 && $(element[i]).attr("id") != blockid) {
                        if (!$(element[i]).attr('toSkip')) {
                            if ($(element[i]).attr("currentRuleText") != null) {
                                if ($(element[i]).attr("iselseblock")) {
                                    if ($(element[i + 2]).attr("currentRuleText") != null) {
                                        if ($(element[i]).attr("currentRuleText").trim() == "Accept  ;" || $(element[i]).attr("currentRuleText").trim() == "Accept ;") {
                                            self._clearError(element[i], ruleArray);
                                        }
                                        else if ($(element[i]).attr("currentRuleText").trim() == "Reject  ;" || $(element[i]).attr("currentRuleText").trim() == "Reject ;") {
                                            self._clearError(element[i], ruleArray);
                                        }
                                        else {
                                            self._clearError(element[i], ruleArray);
                                        }
                                    }
                                    else {
                                        i += 3;
                                    }
                                }
                                else {
                                    if ($(element[i]).attr("currentRuleText").trim() == "Accept  ;" || $(element[i]).attr("currentRuleText").trim() == "Accept ;") {
                                        self._clearError(element[i], ruleArray);
                                    }
                                    else if ($(element[i]).attr("currentRuleText").trim() == "Reject  ;" || $(element[i]).attr("currentRuleText").trim() == "Reject ;") {
                                        self._clearError(element[i], ruleArray);
                                    }
                                    else {
                                        self._clearError(element[i], ruleArray);
                                    }
                                }
                            }
                        }
                    }
                }
                else {
                    if (!$(element[i]).attr('toSkip')) {
                        if ($(element[i]).attr("currentRuleText") != null) {
                            if ($(element[i]).attr('isifblock')) {
                                self._clearError(element[i], ruleArray);
                            }
                            else {
                                if ($(element[i]).attr("currentRuleText").trim() == "Accept  ;" || $(element[i]).attr("currentRuleText").trim() == "Accept ;") {
                                    self._clearError(element[i], ruleArray);
                                }
                                else if ($(element[i]).attr("currentRuleText").trim() == "Reject  ;" || $(element[i]).attr("currentRuleText").trim() == "Reject ;") {
                                    self._clearError(element[i], ruleArray);
                                }
                                else {
                                    self._clearError(element[i], ruleArray);
                                }
                            }
                        }
                    }
                }
            }
        },

        _clearError: function (element, ruleArray) {
            if (ruleArray.length > 0) {
                ruleArray.splice(0, 1);
                if (ruleArray[0] == ";")
                    ruleArray.splice(0, 1);
                if (ruleArray[0].key == "{" && ruleArray.length == 1)
                    ruleArray.splice(0, 1);
                if (ruleArray.length > 0) {

                }
                else {
                    $(element).addClass('RADHighlightErrorBackground');
                }
            }
        },

        saveRule: function () {
            var self = this;
            self._getBlocksToSkip();
            var element = $("#RRuleEditorPrettyTextContent").find("div[isifblock='true']");
            self._getRuleTextBySkippingElse("");
            if ($(".RRuleEditorPrettyTextContent").find(".ruleEditorFirstBlock").length == 0) {
                if (self.options.grammarInfo.RuleTypeJson === "AggregationRule") {
                    return "Then  " + self.options.setRuleText + "  END";
                }
                else {
                    return "Then { " + self.options.setRuleText + " } END";
                }
            }
            else
                return self.options.setRuleText + " END";
        },

        _getRuleTextBySkippingElse: function (blockid) {
            var self = this;
            var element = $(".RRuleEditorPrettyTextContent").children();
            var length = element.length;
            self.options.setRuleText = "";
            for (var i = 0; i < element.length; i++) {
                if ($(element[i]).attr("isJustParent")) {
                    self._getRuleTextBySkippingElseLoop(blockid, $(element[i]).children())
                }
            }

        },

        _getRuleTextBySkippingElseLoop: function (blockid, element) {
            var self = this;
            var length = element.length;
            for (var i = 0; i < length; i++) {
                if ($(element[i]).attr("isJustParent")) {
                    self._getRuleTextBySkippingElseLoop(blockid, $(element[i]).children())
                }
                if (blockid != "") {
                    if ($(element[i]).find("#" + blockid).length == 0 && $(element[i]).attr("id") != blockid) {
                        if (!$(element[i]).attr('toSkip')) {
                            if ($(element[i]).attr("currentRuleText") != null) {
                                if ($(element[i]).attr("iselseblock")) {
                                    if ($(element[i + 2]).attr("currentRuleText") != null) {
                                        if ($(element[i]).attr("currentRuleText").trim() == "Accept  ;" || $(element[i]).attr("currentRuleText").trim() == "Accept ;") {
                                            self.options.ruleText += "Accept";
                                        }
                                        else if ($(element[i]).attr("currentRuleText").trim() == "Reject  ;" || $(element[i]).attr("currentRuleText").trim() == "Reject ;") {
                                            self.options.ruleText += "Reject";
                                        }
                                        else {
                                            self.options.ruleText += $(element[i]).attr("currentRuleText");
                                        }
                                    }
                                    else {
                                        i += 3;
                                    }
                                }
                                else {
                                    if ($(element[i]).attr("currentRuleText").trim() == "Accept  ;" || $(element[i]).attr("currentRuleText").trim() == "Accept ;") {
                                        self.options.ruleText += "Accept";
                                    }
                                    else if ($(element[i]).attr("currentRuleText").trim() == "Reject  ;" || $(element[i]).attr("currentRuleText").trim() == "Reject ;") {
                                        self.options.ruleText += "Reject";
                                    }
                                    else {
                                        self.options.ruleText += $(element[i]).attr("currentRuleText");
                                    }
                                }
                            }
                        }
                    }
                }
                else {
                    if (!$(element[i]).attr('toSkip')) {
                        if ($(element[i]).attr("currentRuleText") != null) {
                            if ($(element[i]).attr('isifblock')) {
                                //self.options.ruleText += "If { [ ";
                                self.options.setRuleText += $(element[i]).attr("currentRuleText");
                            }
                            else {
                                if ($(element[i]).attr("currentRuleText").trim() == "Accept  ;" || $(element[i]).attr("currentRuleText").trim() == "Accept ;") {
                                    self.options.setRuleText += "Accept";
                                }
                                else if ($(element[i]).attr("currentRuleText").trim() == "Reject  ;" || $(element[i]).attr("currentRuleText").trim() == "Reject ;") {
                                    self.options.setRuleText += "Reject";
                                }
                                else {
                                    self.options.setRuleText += $(element[i]).attr("currentruletext");
                                }
                            }
                        }
                    }
                }
            }
        },

        _addElseBlock: function (target) {
            var self = this;
            var ifblockid = target.parent().parent().attr('id');
            var spacecounter = target.parent().parent().attr('spacecounter');
            if (target.closest(".ruleEditorAddDeleteBlock").parent().hasClass('ruleEditorFirstBlock')) {
                element = $(".ruleEditorFirstBlock").parent().children().last();
            }
            else {
                element = $("#RRuleEditorPrettyTextContent").find("div[ifBlockIDBrac='" + ifblockid + "']");
            }
            self.options.idCount++;
            var div = $("<div>");
            var id = "ruleEditorBlock" + self.options.idCount;
            var elseBlockid = id;
            div.attr('id', id);
            div.attr('tabindex', 1);
            div.attr('isElseBlock', true);
            div.attr('ifBlockID', ifblockid);
            div.attr("currentRuleText", " else ");
            div.addClass("ruleEditorParenthesisBlock ruleEditorBlocks");
            //div.html(self._appendSpace(self.options.spaceCounter) + "else");
            //            div.append("<div style=\"display:inline-block\">" + self._appendSpace(spacecounter) + "else" + "<\div>");
            div.append("<div class=\"ruleEditorElseBlock\" style=\"display:inline-block\">" + "else" + "<\div>");
            div.css({ 'margin-left': '20px' });
            if (self.options.grammarInfo.RuleTypeJson == "TransformationRule")
                div.append("<div class=\"ruleEditorAddDeleteBlock\"><div class=\"deleteBlock fa fa-trash-o\" ></div></div>");
            else
                div.append("<div class=\"ruleEditorAddDeleteBlock\"><div class=\"deleteBlock\" ></div></div>");
            div.insertAfter(element);
            element = element.next();

            self.options.idCount++;
            div = $("<div>");
            var id = "ruleEditorBlock" + self.options.idCount;
            div.attr('id', id);
            div.attr('tabindex', 1);
            div.attr("currentRuleText", " { ");
            div.addClass("ruleEditorParenthesisBlock ruleEditorBlocks");
            div.html("{");
            div.css({ 'margin-left': (45) + 'px' });
            div.insertAfter(element);
            element = element.next();

            self.options.idCount++;
            div = $("<div>");
            var id = "ruleEditorBlock" + self.options.idCount;
            div.attr('id', id);
            div.attr('tabindex', 1);
            div.attr("spaceCounter", parseInt(spacecounter) + 1);
            div.css({ 'margin-left': (50) + 'px' });
            div.addClass("ruleEditorActionBlocks ruleEditorBlocks ruleBlocks");
            div.insertAfter(element);
            element = element.next();

            self.options.idCount++;
            div = $("<div>");
            var id = "ruleEditorBlock" + self.options.idCount;
            div.attr('id', id);
            div.attr('elseBlockID', elseBlockid);
            div.attr('tabindex', 1);
            div.attr("currentRuleText", " } ");
            div.addClass("ruleEditorParenthesisBlock ruleEditorBlocks");
            div.html("}");
            div.css({ 'margin-left': (50) + 'px' });
            div.attr("spaceCounter", parseInt(spacecounter));
            div.insertAfter(element);
            element = element.next();
            target.remove();
        },

        _addDeleteIfElseBlock: function (target) {
            var self = this;
            if (target.parent().parent().attr('isElseBlock')) {
                self._deleteElseBlock(target);
            }
            else if (target.parent().parent().attr('isifblock')) {
                self._deleteIfElseBlock(target);
                if (self.options.grammarInfo.RuleTypeJson != "ValidationRule" && self.options.grammarInfo.RuleTypeJson != "AggregationRule") {
                    self._appendEmptyBlockBeforeClosingBrac();
                }
            }
        },

        _deleteIfElseBlock: function (target) {
            var self = this;
            var flag = false;
            var element = target.closest("div[isjustparent]");
            if (target.closest(".ruleEditorBlocks").hasClass("ruleEditorFirstBlock")) {
                $(".RRuleEditorPrettyTextContent").empty();
                $(".RRuleEditorPrettyTextContent").append("<div tabindex=\"1\" id = \"ruleEditorBlock0\" class=\"ruleEditorFirstBlock ruleEditorBlocks ruleBlocks\"></div>");
                self._initiateRuleEditor($(".ruleEditorFirstBlock"));
            }
            else {
                element.remove();
            }
            if (self.options.grammarInfo.RuleTypeJson == "ValidationRule") {
                self._appendEmptyBlockBeforeClosingBrac();
            }
        },

        _deleteElseBlock: function (target) {
            var elseBlockId = target.parent().parent().attr('id');
            var ifBlockId = target.parent().parent().attr('ifblockid');
            var element = target.parent().parent();
            var nextElement;
            while (element.attr('elseBlockID') != elseBlockId) {
                nextElement = element.next();
                element.remove();
                element = nextElement;
            }
            element.remove();
            if ($("#" + ifBlockId).children().last().find('.addBlock').length == 0) {
                $("#" + ifBlockId).children().last().prepend("<div class=\"addBlock fa fa-plus-circle\"></div>");
            }
        },

        _setFocusandOpenRuleEditor: function (event) {
            var self = this;
            var target = $(event.target)
            if (event.keyCode == 38) {
                if (target.hasClass("ruleEditorBlocks")) {
                    target.prev().focus();
                }
            }
            else if (event.keyCode == 40) {
                if (target.hasClass("ruleEditorBlocks")) {
                    target.next().focus();
                    console.log("A");
                    if (target.hasClass("ruleEditorActionBlocks")) {
                        var a = 1;
                    }
                }
            }
            else if (event.keyCode == 13) {
                //$(event.target).blur();
                var target = $(event.target);
                if (target.hasClass("ruleEditorActionBlocks")) {
                    var a = 1;
                }
                if ($(event.target).attr('isifblock') && $(event.target).attr('id') != "ruleTxt") {
                    self._initiateRuleEditor($(event.target).find(".ruleEditorEditingBlock"));
                }
                else {
                    if ($(event.target).attr('id') != "ruleTxt")
                        self._initiateRuleEditor($(event.target));
                }
            }
        },

        destroyRuleEditor: function () {
            var self = this;
            var currentBlockText = $("#ruleTxt").val()
            var editor = $("#ruleEditorDiv").data('ruleTextEngine');
            if (currentBlockText.toLowerCase().trim().endsWith("end")) {
                self._ruleEditorClose(currentBlockText, plugin.settings.blockid, editor.settings.jsplumbId, currentBlockText);
            }
            else {
                self._ruleEditorClose(editor.settings.prependText + currentBlockText, editor.settings.blockid, editor.settings.jsplumbId, currentBlockText);
            }
        },

        _initiateRuleEditor: function (target) {
            var highlightControls = $(".RRuleEditorPrettyTextContent").find(".RADHighlightBackground,.RADHighlightErrorBackground");
            if (highlightControls != null && highlightControls.length > 0) {
                for (var i = 0; i < highlightControls.length; i++) {
                    $(highlightControls[0]).removeClass("RADHighlightBackground");
                    $(highlightControls[0]).removeClass("RADHighlightErrorBackground");
                }
            }
            var self = this;
            //if ($(event.target).closest('.ruleEditorFirstBlock').children().length == 0) {
            if ($("#ruleTxt").length > 0) {
                self.destroyRuleEditor();
            }
            if (!(target.hasClass("ruleEditorParenthesisBlock") && target.hasClass("ruleEditorIFBlock") && target.parent().hasClass("ruleEditorIFBlock") && target.hasClass("ruleEditorEndParenthesisBlock"))) {
                if ($("#ruleTxt").length == 0) {
                    self.options.setRuleText = "";
                    if (target.closest(".ruleEditorFirstBlock").length > 0) {
                        //self.options.ruleText = "If { [ ";
                        if (target.hasClass('ruleEditorFirstBlock')) {
                            if (self.options.grammarInfo.RuleTypeJson === "AggregationRule") {
                                self.options.ruleText = "Then ";
                            }
                            else {
                                self.options.ruleText = "Then { ";
                            }
                            self._getRuleTextForEditing(target.closest(".ruleBlocks").attr("id"));
                        }
                        else {
                            self.options.ruleText = "If { ";
                            self.options.setRuleText = target.closest(".ruleEditorFirstBlock").attr('actualruletext');
                        }

                        //self._getRuleTextForEditing($(event.target).attr("id"));

                        if (self.options.setRuleText != "") {

                        }
                    }
                    else {
                        self.options.ruleText = "";
                        self._getRuleTextForEditing(target.closest(".ruleBlocks").attr("id"));
                        if (self.options.grammarInfo.RuleTypeJson === "AggregationRule" && self.options.ruleText.indexOf("{") > -1) {
                            self.options.ruleText = self.options.ruleText.substr(0, self.options.ruleText.lastIndexOf("{"));
                        }
                    }
                    //$(event.target).closest(".ruleEditorBlocks").html("");
                    //$(event.target).closest(".ruleEditorBlocks").append("<div id = \"ruleEditorDiv\"></div>")
                    var element = target.closest(".ruleBlocks");
                    element.html("");
                    element.append("<div id = \"ruleEditorDiv\"></div>");
                    if ($("#" + element.attr("id")).hasClass("ruleEditorEditingBlock")) {
                        if (!$("#" + element.attr("id")).hasClass("ruleEditorEditingBlockWidth")) {
                            $("#" + element.attr("id")).addClass("ruleEditorEditingBlockWidth");
                        }
                    }
                    $("#ruleEditorDiv").ruleTextEngine({ ruleText: self.options.setRuleText,
                        grammarInfo: self.options.grammarInfo,
                        prependText: self.options.ruleText,
                        closeRuleEditor: self._ruleEditorClose,
                        appendControls: self._appendControls,
                        // jsplumbId: self.element.attr('id'),
                        blockid: element.attr("id"),
                        clientGuid: self.options.clientGUID,
                        hasFormattingSection: false,
                        jsplumbId: $(self.element).attr("id"),
                        serviceUrl: self.options.serviceUrl
                    });
                }
            }
        },

        _getRuleTextForEditing: function (blockid) {
            var self = this;
            self.options.breakChain = false;
            var element = $(".RRuleEditorPrettyTextContent").children();
            if ($(".RRuleEditorPrettyTextContent").find(".ruleEditorFirstBlock").length == 0) {
                if (self.options.grammarInfo.RuleTypeJson === "AggregationRule") {
                    self.options.ruleText = "Then ";
                }
                else {
                    self.options.ruleText = "Then { ";
                }
            }
            for (var i = 0; i < element.length; i++) {
                if ($(element[i]).attr("isJustParent")) {
                    self._getRuleText(blockid, $(element[i]).children())
                }
            }
        },

        _getRuleText: function (blockid, element) {
            var self = this;
            var length = element.length;
            for (var i = 0; i < length; i++) {
                if ($(element[i]).attr("isJustParent")) {
                    self._getRuleText(blockid, $(element[i]).children())
                }
                if (!self.options.breakChain) {
                    if ($(element[i]).find("#" + blockid).length == 0 && $(element[i]).attr("id") != blockid) {
                        if (!$(element[i]).attr('toSkip')) {
                            if ($(element[i]).attr("currentRuleText") != null) {
                                if ($(element[i]).attr("currentRuleText").trim() == "Accept  ;" || $(element[i]).attr("currentRuleText").trim() == "Accept ;") {
                                    self.options.ruleText += "Accept ";
                                }
                                else if ($(element[i]).attr("currentRuleText").trim() == "Reject  ;" || $(element[i]).attr("currentRuleText").trim() == "Reject ;") {
                                    self.options.ruleText += "Reject ";
                                }
                                else {
                                    self.options.ruleText += $(element[i]).attr("currentRuleText");
                                }
                            }
                        }
                    }
                    else {
                        if (!$(element[i]).attr('toSkip')) {
                            if ($(element[i]).attr("currentRuleText") != null) {
                                if ($(element[i]).attr('isifblock')) {
                                    self.options.ruleText += "If { ";
                                    self.options.setRuleText = $(element[i]).attr("actualruletext");
                                }
                                else {
                                    if ($(element[i]).attr("actualruletext").trim() == "Accept  ;" || $(element[i]).attr("currentRuleText").trim() == "Accept ;") {
                                        self.options.setRuleText = "Accept ";
                                    }
                                    else if ($(element[i]).attr("actualruletext").trim() == "Reject  ;" || $(element[i]).attr("currentRuleText").trim() == "Reject ;") {
                                        self.options.setRuleText = "Reject ";
                                    }
                                    else {
                                        self.options.setRuleText = $(element[i]).attr("actualruletext");
                                    }
                                }
                            }
                            else {
                                if ($(element[i]).attr('isifblock')) {
                                    self.options.ruleText += $(element[i]).attr("currentRuleText");
                                    self.options.setRuleText = $(element[i]).attr("actualruletext");
                                }
                                else {
                                    if ($(element[i]).attr("actualruletext")) {
                                        if ($(element[i]).attr("actualruletext").trim() == "Accept  ;" || $(element[i]).attr("currentRuleText").trim() == "Accept ;") {
                                            self.options.setRuleText = "Accept ";
                                        }
                                        else if ($(element[i]).attr("actualruletext").trim() == "Reject  ;" || $(element[i]).attr("currentRuleText").trim() == "Reject ;") {
                                            self.options.setRuleText = "Reject ";
                                        }
                                        else {
                                            self.options.setRuleText = $(element[i]).attr("actualruletext");
                                        }
                                    }
                                    else {
                                        self.options.setRuleText = $(element[i]).attr("actualruletext");
                                    }
                                }
                            }
                        }
                        self.options.breakChain = true;
                        break;
                    }
                }
            }
        },

        _appendControls: function (blockid, ruleText, jsPlumbId, isIfBlock) {
            var self = $("#" + jsPlumbId).data("radWidgetRRuleEditor");
            self.options.spaceCounter++;
            if ($("#" + blockid).find("#ruleEditorDiv").data('ruleTextEngine') != undefined) {
                $("#" + blockid).find("#ruleEditorDiv").data('ruleTextEngine').Destroy();
                $("#ruleEditorDiv").remove();
            }
            self._appendIforThenBlock(blockid, ruleText, isIfBlock)
            $("#" + blockid).closest('.ruleEditorBlocks').focus();
            //self._appendinLeftSideBlock($("#" + blockid).closest('.ruleEditorBlocks').index());
            self._openRuleEditorByDefault(blockid);
        },


        _appendinLeftSideBlock: function (index) {
            var parent = $(".RRuleEditorBlockIDContentContent");
            var ifblockid = "ruleEditorContent" + parent.children().length;
            if (index == 0) {
                $(".RRuleEditorBlockIDContentContent").append("<div id=\"ruleEditorContent" + parent.children().length + "\" class=\"showHideContent fa fa-minus-square\"></div>");
                $(".RRuleEditorBlockIDContentContent").append("<div id=\"ruleEditorContent" + parent.children().length + "\" class=\"showHideContent\"></div>");
                $(".RRuleEditorBlockIDContentContent").append("<div id=\"ruleEditorContent" + parent.children().length + "\" class=\"showHideContent\"></div>");
                $(".RRuleEditorBlockIDContentContent").append("<div id=\"ruleEditorContent" + parent.children().length + "\" class=\"showHideContent\"></div>");
                $(".RRuleEditorBlockIDContentContent").append("<div id=\"ruleEditorContent" + parent.children().length + "\" class=\"showHideContent\"></div>");
                $(".RRuleEditorBlockIDContentContent").append("<div id=\"ruleEditorContent" + parent.children().length + "\" class=\"showHideContent\"></div>");
                $(".RRuleEditorBlockIDContentContent").append("<div id=\"ruleEditorContent" + parent.children().length + "\" class=\"showHideContent\"></div>");
                $(".RRuleEditorBlockIDContentContent").append("<div startBlockId=\"" + ifblockid + "\" id=\"ruleEditorContent" + parent.children().length + "\" class=\"showHideContent\"></div>");
            }
            else {

                var element = $($(".RRuleEditorBlockIDContentContent").children()[index]);
                index--;

                for (var i = 0; i < 8; i++) {
                    var div = $('<div>');
                    div.attr('id', "ruleEditorContent" + parent.children().length);
                    div.addClass("showHideContent");
                    if (i == 0) {
                        div.addClass("fa fa-minus-square");
                    }
                    if (i == 7) {
                        div.attr('startBlockId', ifblockid);
                    }

                    div.insertAfter($(parent.children()[index + i]));
                }
            }
        },

        _openRuleEditorByDefault: function (blockid) {
            var self = this;
            self.options.ruleText = "";
            self.options.setRuleText = "";
            self._getRuleTextForEditing(blockid);
            $("#" + blockid).find(".ruleEditorEditingBlock").append("<div id = \"ruleEditorDiv\"></div>");
            $("#ruleEditorDiv").ruleTextEngine({ ruleText: self.options.setRuleText,
                grammarInfo: self.options.grammarInfo,
                prependText: self.options.ruleText,
                closeRuleEditor: self._ruleEditorClose,
                appendControls: self._appendControls,
                // jsplumbId: self.element.attr('id'),
                blockid: $("#" + blockid).find(".ruleEditorEditingBlock").attr('id'),
                clientGuid: self.options.clientGUID,
                hasFormattingSection: false,
                jsplumbId: $(self.element).attr("id"),
                serviceUrl: self.options.serviceUrl
            });
        },
        _appendIforThenBlock: function (blockid, ruleText, isIfBlock) {
            var self = this;
            var element = $("#" + blockid);
            element.attr("spaceCounter", self.options.spaceCounter);
            if (isIfBlock) {
                element.html("");
                self._appendIfBlock(element);
                element.attr('isIfBlock', true);
                element.removeClass("ruleEditorActionBlocks");
                element.addClass("ruleEditorIfActionBlocks");
            }
            else {
                //element.html("Then");
                self._appendThenBlock(element);
                element.attr('isThenBlock', true);
            }
            self._appendBlockDetails(element);
        },

        _appendThenBlock: function (element) {
            var self = this;
            element.attr("currentRuleText", "Then { ");
            element.append("<div class=\"ruleEditorIFBlock\">" + self._appendSpace(self.options.spaceCounter) + "Then (</div>");
            element.append("<div id = \"ruleEditorEditingBlock" + self.options.spaceCounter + "\" class=\"ruleEditorEditingBlock ruleBlocks ruleEditorEditingBlockWidth\"></div>");
            element.append("<div class=\"ruleEditorEndParenthesisBlock\">)</div>");
            element.append("<div class=\"ruleEditorAddDeleteBlock\"><div class=\"deleteBlock fa fa-trash-o\" ></div></div>");
        },

        _appendIfBlock: function (element) {
            var self = this;
            var div = $("<div>");
            div.attr("spaceCounter", parseInt(self.options.spaceCounter));
            div.attr("isJustParent", true);
            if (self.options.idCount > 0)
                div.css({ 'margin-left': "45px" });
            element.wrap(div);
            element.attr("currentRuleText", "If { ");
            element.attr("actualruletext", "");
            //            element.append("<div class=\"ruleEditorIFBlock\">" + self._appendSpace(self.options.spaceCounter) + "if (</div>");
            if (element.hasClass("ruleEditorFirstBlock")) {
                element.append("<div class=\"showHideContent fa fa-caret-down\" ></div>");
                element.append("<div class=\"ruleEditorIFBlock\"> if </div>");
            }
            else {
                element.append("<div class=\"showHideContent fa fa-caret-down\" ></div>");
                element.append("<div class=\"ruleEditorIFBlock\"> if </div>");
                element.css({ 'margin-left': ((self.options.spaceCounter - 1) * 0) + 'px' });
            }
            //            element.append("<div id = \"ruleEditorEditingBlock" + self.options.spaceCounter + "\" class=\"ruleEditorEditingBlock ruleBlocks ruleEditorEditingBlockWidth\"></div>");
            element.append("<div id = \"ruleEditorEditingBlock" + $(".ruleEditorEditingBlock").length + "\" class=\"ruleEditorEditingBlock ruleBlocks ruleEditorEditingBlockWidth\"></div>");
            element.append("<div class=\"ruleEditorEndParenthesisBlock\"></div>");
            element.append("<div class=\"ruleEditorAddDeleteBlock\"><div class=\"deleteBlock fa fa-trash-o\" ></div></div>");
        },

        _appendBlockDetails: function (element) {

            var target = element;
            var self = this;
            var div = $("<div>");
            self.options.idCount++;
            var id = "ruleEditorBlock" + self.options.idCount;
            div.attr('id', id);
            div.attr('tabindex', 1);
            //div.attr('toSkip', true);
            div.attr("currentRuleText", " {  ");
            div.addClass("ruleEditorParenthesisBlock ruleEditorBlocks");
            //            div.html(self._appendSpace(self.options.spaceCounter) + "{");
            div.css({ 'margin-left': (45) + 'px' });
            div.html("{");

            div.insertAfter(element);
            element = element.next();

            div = $("<div>");
            self.options.idCount++;
            var id = "ruleEditorBlock" + self.options.idCount;
            div.attr('id', id);
            div.attr('tabindex', 1);
            div.attr("spaceCounter", parseInt(self.options.spaceCounter) + 1);
            div.css({ 'margin-left': (2 * 25) + 'px' });
            div.addClass("ruleEditorActionBlocks ruleEditorBlocks ruleBlocks");
            div.insertAfter(element);
            element = element.next();

            div = $("<div>");
            self.options.idCount++;
            var id = "ruleEditorBlock" + self.options.idCount;
            div.attr('id', id);
            div.attr('tabindex', 1);
            div.attr('ifBlockIDBrac', target.attr('id'));
            //div.attr('toSkip', true);
            div.attr("currentRuleText", " } ");
            div.addClass("ruleEditorParenthesisBlock ruleEditorBlocks");
            //            div.html(self._appendSpace(self.options.spaceCounter) + "}");
            div.html("}");
            div.css({ 'margin-left': (45) + 'px' });
            div.insertAfter(element);
            element = element.next();

            if (self.options.grammarInfo.RuleTypeJson != "AggregationRule") {
                div = $("<div>");
                self.options.idCount++;
                var id = "ruleEditorBlock" + self.options.idCount;
                var elseBlockid = id;
                div.attr('id', id);
                div.attr('tabindex', 1);
                div.attr('isElseBlock', true);
                div.attr('ifBlockID', target.attr('id'));
                div.attr("currentRuleText", " else ");
                div.addClass("ruleEditorParenthesisBlock ruleEditorBlocks");
                //div.html(self._appendSpace(self.options.spaceCounter) + "else");
                //div.append("<div style=\"display:inline-block\">" + self._appendSpace(self.options.spaceCounter) + "else" + "<\div>");
                div.append("<div class=\"ruleEditorElseBlock\" style=\"display:inline-block\">" + "else" + "<\div>");
                div.css({ 'margin-left': (20) + 'px' });
                if (self.options.grammarInfo.RuleTypeJson == "TransformationRule")
                    div.append("<div class=\"ruleEditorAddDeleteBlock\"><div class=\"deleteBlock fa fa-trash-o\" ></div></div>");
                else
                    div.append("<div class=\"ruleEditorAddDeleteBlock\"><div class=\"deleteBlock\" ></div></div>");
                div.insertAfter(element);
                element = element.next();

                div = $("<div>");
                self.options.idCount++;
                var id = "ruleEditorBlock" + self.options.idCount;
                div.attr('id', id);
                div.attr('tabindex', 1);
                div.attr("currentRuleText", " { ");
                div.addClass("ruleEditorParenthesisBlock ruleEditorBlocks");
                div.html("{");
                div.css({ 'margin-left': (45) + 'px' });
                div.insertAfter(element);
                element = element.next();

                div = $("<div>");
                self.options.idCount++;
                var id = "ruleEditorBlock" + self.options.idCount;
                div.attr('id', id);
                div.attr('tabindex', 1);
                div.attr("spaceCounter", parseInt(self.options.spaceCounter) + 1);
                div.css({ 'margin-left': (2 * 25) + 'px' });
                div.addClass("ruleEditorActionBlocks ruleEditorBlocks ruleBlocks");
                div.insertAfter(element);
                element = element.next();

                div = $("<div>");
                self.options.idCount++;
                var id = "ruleEditorBlock" + self.options.idCount;
                div.attr('id', id);
                div.attr('elseBlockID', elseBlockid);
                div.attr('tabindex', 1);
                div.attr("currentRuleText", " } ");
                div.addClass("ruleEditorParenthesisBlock ruleEditorBlocks");
                div.attr("spaceCounter", parseInt(self.options.spaceCounter));
                div.html("}");
                div.css({ 'margin-left': (45) + 'px' });
                div.insertAfter(element);
                element = element.next();
            }
        },

        _getBlocksToSkip: function () {
            var self = this;
            var element = $(".RRuleEditorPrettyTextContent").children();
            var length = element.length;
            self.options.setRuleText = "";
            for (var i = 0; i < element.length; i++) {
                if ($(element[i]).attr("isJustParent")) {
                    self._getBlocksToSkipLoop($(element[i]).children())
                }
            }

        },

        _getBlocksToSkipLoop: function (element) {
            var self = this;
            var length = element.length;
            for (var i = 0; i < length; i++) {
                if ($(element[i]).attr("isJustParent")) {
                    self._getBlocksToSkipLoop($(element[i]).children())
                }
                if (!$(element[i]).attr('toSkip')) {
                    if ($(element[i]).html().trim() == "") {
                        if ($(element[i]).prev().html().trim() == "{" && $(element[i]).next().html().trim() == "}" && $(element[i]).prev().prev().children().first().hasClass("ruleEditorElseBlock")) {
                            $(element[i]).prev().prev().attr('toSkip', true);
                            $(element[i]).prev().attr('toSkip', true);
                            $(element[i]).next().attr('toSkip', true);
                        }
                    }
                    else {
                        if (($(element[i]).html().trim() == "{" || $(element[i]).html().trim() == "}") && self.options.grammarInfo.RuleTypeJson == "AggregationRule") {
                            $(element[i]).attr('toSkip', true);
                        }
                    }
                }
            }
        },
        changeRuleText: function (ruleText) {
            var self = this;
            $("#RRuleEditorPrettyTextContent").empty();
            self.options.ruleText = ruleText;
            if (ruleText.trim() != "")
                self._createExistingRuleNew();
            else {
                $("#RRuleEditorPrettyTextContent").append("<div tabindex=\"1\" id = \"ruleEditorBlock0\" class=\"ruleEditorFirstBlock ruleEditorBlocks ruleBlocks\"></div>");
                self._initiateRuleEditor($(".ruleEditorFirstBlock"));
            }
        },

        _createExistingRuleNew: function () {
            var self = this;
            var existingRule = self.options.ruleText;
            var ruleTextTokens = self._splitRuleTextNew(existingRule);
            $("#RRuleEditorPrettyTextContent").empty();
            var element = null;
            var elemnetIF = null;
            var elementText = null;
            var elementClosingBrace = null;
            var editingBlock = 0;
            var counter = 0;
            var parentelement = null;
            var parentElementArray = [];
            var spaceCnt = 0;
            self.options.idCount = 0;
            for (var i = 0; i < ruleTextTokens.length; i++) {
                if (ruleTextTokens[i].key.trim().toLowerCase() != "end") {
                    element = null;
                    elemnetIF = null;
                    elementText = null;
                    elementClosingBrace = null;
                    var length = self.options.idCount;
                    self.options.idCount++;
                    if (ruleTextTokens[i].key.trim() != "") {
                        if (ruleTextTokens[i].key.trim() != "{" && ruleTextTokens[i].key.trim() != "}" && ruleTextTokens[i].key.trim().toLowerCase() != "else") {
                            if (ruleTextTokens[i].value.trim().toLowerCase().startsWith("if")) {
                                if (parentelement != null)
                                    parentElementArray.push(parentelement);
                                parentelement = $("<div>");
                                spaceCnt++;
                                parentelement.attr("spacecounter", spaceCnt);
                                parentelement.attr("isjustparent", "true");
                                if (spaceCnt > 1)
                                    parentelement.css({ 'margin-left': "45px" });
                                element = $("<div>");
                                self.options.ifelseflag = "if";
                                element.attr('id', "ruleEditorBlock" + length);
                                self.options.IFBlockIDsStack.push("ruleEditorBlock" + length);
                                //self.options.ElseBlockIdsStack = [];
                                elemnetIF = $("<div>");
                                elementText = $("<div>");
                                elementClosingBrace = $("<div>");
                                //element.attr('id', "ruleEditorBlock" + counter);
                                element.attr('currentruletext', ruleTextTokens[i].value);
                                element.attr('isifblock', true);
                                element.attr('actualruletext', ruleTextTokens[i].key);
                                elemnetIF.addClass("ruleEditorIFBlock");
                                //                            elemnetIF.html(self._appendSpace(ruleTextTokens[i].spaceCounter) + "if (");
                                //                            elemnetIF.html(self._appendSpace(ruleTextTokens[i].spaceCounter) + self._syntaxHighlight("if") + " (");
                                elemnetIF.html(self._syntaxHighlight("if"));
                                //elemnetIF.css({ 'margin-left': (ruleTextTokens[i].spaceCounter * 30) + 'px' });
                                element.append("<div class=\"showHideContent fa fa-caret-down\" ></div>");
                                element.append(elemnetIF);
                                element.attr('spaceCounter', ruleTextTokens[i].spaceCounter);
                                elementText.append(self._syntaxHighlight(ruleTextTokens[i].key));
                                elementText.attr("id", "ruleEditorEditingBlock" + editingBlock);
                                editingBlock++;
                                elementText.addClass("ruleEditorEditingBlock ruleBlocks");
                                element.append(elementText);
                                elementClosingBrace.addClass("ruleEditorEndParenthesisBlock");
                                //elementClosingBrace.html(")");
                                element.append(elementClosingBrace);
                                element.append("<div class=\"ruleEditorAddDeleteBlock\"><div class=\"deleteBlock fa fa-trash-o\" ></div></div>");
                                //element.append("<div class=\"ruleEditorAddDeleteBlock\"><div class=\"deleteBlock fa fa-trash\" ></div></div>");
                                if (i == 0) {
                                    element.addClass("ruleEditorFirstBlock ruleEditorBlocks ruleBlocks ruleEditorIfActionBlocks");
                                    $("#RRuleEditorPrettyTextContent").append(parentelement);
                                    parentelement.append(element);
                                }
                                else {
                                    element.addClass("ruleEditorIfActionBlocks ruleEditorBlocks ruleBlocks");
                                    if (parentElementArray.length > 0) {
                                        var ele = parentElementArray[parentElementArray.length - 1];
                                        ele.append(parentelement);
                                        parentelement.append(element);
                                    }
                                    else {
                                        $("#RRuleEditorPrettyTextContent").append(parentelement);
                                        parentelement.append(element);
                                    }
                                }
                                //self._appendContentBlock(true, element.attr('id'), false);
                            }
                            else if (ruleTextTokens[i].value.trim().toLowerCase().startsWith("then")) {
                                if (parentelement != null)
                                    parentElementArray.push(parentelement);
                                parentelement = $("<div>");
                                parentelement.attr("spacecounter", spaceCnt);
                                parentelement.attr("isjustparent", "true");
                                if (i == 0)
                                    $("#RRuleEditorPrettyTextContent").append(parentelement);
                            }
                            else {
                                if (ruleTextTokens[i].value.trim() != ";") {
                                    if (parentelement == null) {
                                        parentelement = $("<div>");
                                    }
                                    parentelement.attr("spacecounter", spaceCnt);
                                    parentelement.attr("isjustparent", "true");
                                    element = $("<div>");
                                    element.attr('id', "ruleEditorBlock" + length);
                                    //element.attr('id', "ruleEditorBlock" + counter);
                                    element.addClass("ruleEditorActionBlocks ruleEditorBlocks ruleBlocks")
                                    if (ruleTextTokens[i].value.trim().endsWith(";"))
                                        element.attr('currentRuleText', ruleTextTokens[i].value + " ");
                                    else
                                        element.attr('currentRuleText', ruleTextTokens[i].value + " ; ");
                                    element.attr('spaceCounter', ruleTextTokens[i].spaceCounter);
                                    element.attr('actualruletext', ruleTextTokens[i].key + " ");
                                    element.append(self._syntaxHighlight(ruleTextTokens[i].key));
                                    element.css({ "margin-left": '50px' });
                                    parentelement.append(element);
                                    if (i == 0)
                                        $("#RRuleEditorPrettyTextContent").append(parentelement);
                                    //self._appendContentBlock(false, element.attr('id'), false);
                                }
                            }
                        }
                        else {
                            if (existingRule.trim().toLowerCase().startsWith("then")) {
                                counter++;
                                continue;
                            }
                            element = $("<div>");
                            element.attr('id', "ruleEditorBlock" + length);
                            //element.attr('id', "ruleEditorBlock" + counter);
                            element.addClass("ruleEditorParenthesisBlock ruleEditorBlocks")
                            element.attr("currentruletext", " " + ruleTextTokens[i].value + " ");
                            if (ruleTextTokens[i].value.toLowerCase().indexOf("else") > -1)
                                element.append("<div class=\"ruleEditorElseBlock\" style=\"display:inline-block\">" + ruleTextTokens[i].value + "<\div>");
                            else
                                element.html(ruleTextTokens[i].value);
                            element.css({ "margin-left": '45px' });
                            if ((self.options.ifelseflag == "none" || self.options.ifelseflag == "else") && ruleTextTokens[i].key.trim().toLowerCase() == "}") {
                                parentelement.append(element);
                                if (parentElementArray.length > 0) {
                                    parentelement = parentElementArray[parentElementArray.length - 1];
                                    //parentelement.append(element);
                                    parentElementArray.length = parentElementArray.length - 1;
                                }
                            }
                            else if (self.options.ifelseflag == "if" && ruleTextTokens[i].key.trim().toLowerCase() == "}") {
                                parentelement.append(element);
                                if (ruleTextTokens[i + 1] != null && ruleTextTokens[i + 1].key.trim().toLowerCase() != "else") {
                                    if (parentElementArray.length > 0) {
                                        parentelement = parentElementArray[parentElementArray.length - 1];
                                        //parentelement.append(element);
                                        parentElementArray.length = parentElementArray.length - 1;
                                    }
                                }
                            }
                            else {
                                parentelement.append(element);

                            }
                            if (ruleTextTokens[i].key.trim().toLowerCase() == "}") {
                                if (self.options.ifelseflag == "else") {
                                    self.options.ifelseflag = "none";
                                    element.attr("elseblockid", self.options.ElseBlockIdsStack[self.options.ElseBlockIdsStack.length - 1]);
                                    //self.options.ElseBlockIdsStack = [];
                                    self.options.ElseBlockIdsStack.splice(self.options.ElseBlockIdsStack.length - 1, 1);
                                    //self.options.IFBlockIDsStack.splice(self.options.IFBlockIDsStack.length - 1, 1);
                                    //self._appendContentBlock(false, null, true);
                                }
                                else if (self.options.ifelseflag == "if") {
                                    self.options.ifelseflag = "none";
                                    element.attr("ifblockidbrac", self.options.IFBlockIDsStack[self.options.IFBlockIDsStack.length - 1]);
                                    if (ruleTextTokens[i + 1] != null && ruleTextTokens[i].key.trim().toLowerCase() != "else") {
                                        self.options.IFBlockIDsStack.splice(self.options.IFBlockIDsStack.length - 1, 1);
                                    }
                                    //self.options.IFBlockIDsStack.splice(self.options.IFBlockIDsStack.length - 1, 1);
                                    //self._appendContentBlock(false, null, false);
                                }
                                else if (self.options.ifelseflag == "none") {
                                    element.attr("ifblockidbrac", self.options.IFBlockIDsStack[self.options.IFBlockIDsStack.length - 1]);
                                    //self.options.IFBlockIDsStack.splice(self.options.IFBlockIDsStack.length - 1, 1);
                                    // if (ruleTextTokens[i + 1] != null && ruleTextTokens[i].key.trim().toLowerCase() != "else") {
                                    if (ruleTextTokens[i].key.trim().toLowerCase() != "else") {
                                        self.options.IFBlockIDsStack.splice(self.options.IFBlockIDsStack.length - 1, 1);
                                    }
                                }
                            }
                            else if (ruleTextTokens[i].key.trim().toLowerCase() == "else") {
                                self.options.ifelseflag = "else";
                                self.options.ElseBlockIdsStack.push("ruleEditorBlock" + length);
                                element.attr("ifblockid", element.prev().attr("ifblockidbrac"));
                                element.attr("isElseBlock", true);
                                element.attr("currentruletext", " " + ruleTextTokens[i].key + " ");
                                element.css({ "margin-left": '20px' });
                                if (self.options.grammarInfo.RuleTypeJson == "TransformationRule")
                                    element.append("<div class=\"ruleEditorAddDeleteBlock\"><div class=\"deleteBlock fa fa-trash-o\" ></div></div>");
                                else
                                    element.append("<div class=\"ruleEditorAddDeleteBlock\"><div class=\"deleteBlock\" ></div></div>");
                                //self._appendContentBlock(false, null, false);
                            }
                            else if (ruleTextTokens[i].key.trim().toLowerCase() == "{") {
                                //self._appendContentBlock(false, null, false);
                            }
                        }
                    }
                    counter++
                }
            }

            self._appendAddDeleteBlocks();
        },

        _appendContentBlock: function (isIfBlock, blockid, isClosingBrac) {
            var self = this;
            var parent = $(".RRuleEditorBlockIDContentContent");
            var div = $('<div>')
            div.attr('id', "ruleEditorContent" + parent.children().length)
            if (isIfBlock) {
                self.options.IFContentIDsStack.push("ruleEditorContent" + parent.children().length);
            }
            div.addClass('showHideContent');
            if (isIfBlock) {
                div.addClass('fa fa-minus-square');
                div.height($("#" + blockid).height());
            }
            if (isClosingBrac) {
                div.attr("startblockid", self.options.IFContentIDsStack[self.options.IFContentIDsStack.length - 1]);
                self.options.IFContentIDsStack.splice(self.options.IFContentIDsStack.length - 1, 1);
            }
            parent.append(div);
        },

        _appendAddDeleteBlocks: function () {
            var self = this;
            var element = $(".RRuleEditorPrettyTextContent").children();
            for (var i = 0; i < element.length; i++) {
                if ($(element[i]).attr("isJustParent")) {
                    self.appendBlockAddAndDelete($(element[i]).children())
                }
            }
        },


        appendBlockAddAndDelete: function (element) {
            var self = this;
            var length = element.length;
            for (var i = 0; i < length; i++) {
                if ($(element[i]).attr('isifblock')) {
                    if ($("#RRuleEditorPrettyTextContent").find('div[ifblockid="' + $(element[i]).attr("id") + '"]').length > 0) {
                        //$(element[i]).append("<div class=\"ruleEditorAddDeleteBlock\"><div class=\"deleteBlock fa fa-trash\" ></div></div>");
                    }
                    else {
                        $(element[i]).find(".ruleEditorAddDeleteBlock").prepend("<div class=\"addBlock fa fa-plus-circle\"></div>");
                    }
                }
                else if ($(element[i]).attr('isElseBlock')) {
                    if ($("#RRuleEditorPrettyTextContent").find('div[elseblockid="' + $(element[i]).attr("id") + '"]').length > 0) {
                        //$(element[i]).append("<div class=\"ruleEditorAddDeleteBlock\"><div class=\"deleteBlock fa fa-trash\" ></div></div>");
                    }
                }
                else if ($(element[i]).attr("isJustParent")) {
                    self.appendBlockAddAndDelete($(element[i]).children())
                }
            }
        },

        _splitRuleTextAggreagte: function (existingRule) {
            var self = this;
            var RuleTextSplitByOpeningBrace = [];
            var RuleTextSplitByClosingBrace = [];
            var RuleTextSplitByThen = existingRule.split("Then");
            var RuleTextFinalTokens = [];
            if (RuleTextSplitByThen[0].trim() == "") {
                RuleTextFinalTokens.push("Then");
                for (var i = 0; i < RuleTextSplitByThen.length; i++) {
                    if (RuleTextSplitByThen[i].trim() != "")
                        RuleTextFinalTokens.push(RuleTextSplitByThen[i]);
                }
            }
            else {
                var RuleTextSplitByBrace = RuleTextSplitByThen[0].split(/{(?=(?:(?:[^"]*"){2})*[^"]*$)/);
                for (var i = 0; i < RuleTextSplitByBrace.length; i++) {
                    if (RuleTextSplitByBrace[i].indexOf("}") != -1) {
                        RuleTextSplitByClosingBrace = RuleTextSplitByBrace[i].split(/}(?=(?:(?:[^"]*"){2})*[^"]*$)/);
                        if (RuleTextSplitByClosingBrace.length > 0) {
                            for (var j = 0; j < RuleTextSplitByClosingBrace.length; j++) {
                                if (RuleTextSplitByClosingBrace[j].trim() != "") {
                                    RuleTextFinalTokens.push(RuleTextSplitByClosingBrace[j]);
                                    if (RuleTextSplitByClosingBrace[j].trim().toLowerCase() == "then" || RuleTextSplitByClosingBrace[j].trim().toLowerCase() == "else") {
                                        RuleTextFinalTokens.push("{");
                                    }
                                    else {
                                        RuleTextFinalTokens.push("}");
                                    }
                                }
                                else if (RuleTextSplitByClosingBrace[j].trim() == "" && j != RuleTextSplitByClosingBrace.length - 1) {
                                    RuleTextFinalTokens.push("}");
                                }
                            }
                        }
                    }
                    else {
                        RuleTextFinalTokens.push(RuleTextSplitByBrace[i]);
                        RuleTextFinalTokens.push("{");
                    }
                }
                RuleTextFinalTokens.push("Then");
                RuleTextFinalTokens.push("{");
                RuleTextFinalTokens.push(RuleTextSplitByThen[1]);
                RuleTextFinalTokens.push("}");
            }

            return RuleTextFinalTokens;
        },

        _splitRuleTextNew: function (existingRule) {
            var self = this;
            if (self.options.grammarInfo.RuleTypeJson === "AggregationRule") {
                return self._getRuleTextDictionary(self._splitRuleTextAggreagte(existingRule));
            }
            else {
                var RuleTextSplitByOpeningBrace = [];
                var RuleTextFinalTokens = [];
                RuleTextSplitByOpeningBrace = existingRule.split(/{(?=(?:(?:[^"]*"){2})*[^"]*$)/);
                for (var i = 0; i < RuleTextSplitByOpeningBrace.length; i++) {
                    var RuleTextSplitByClosingBrace = [];
                    var RuleTextSplitBySemiColon = [];
                    if (RuleTextSplitByOpeningBrace[i].indexOf("}") != -1) {
                        RuleTextSplitByClosingBrace = RuleTextSplitByOpeningBrace[i].split(/}(?=(?:(?:[^"]*"){2})*[^"]*$)/);
                    }
                    else {
                        //                    if (RuleTextSplitByOpeningBrace[i] != " ") {
                        //                        RuleTextFinalTokens.push(RuleTextSplitByOpeningBrace[i]);
                        //                        RuleTextFinalTokens.push("{");
                        //                    }
                        if (RuleTextSplitByOpeningBrace[i].indexOf(";") > -1) {
                            var split = RuleTextSplitByOpeningBrace[i].split(";");
                            for (var x = 0; x < split.length; x++) {
                                RuleTextFinalTokens.push(split[x]);
                                if (split[x].trim() != "") {
                                    if (x == split.length - 1)
                                        RuleTextFinalTokens.push("{");
                                }
                            }
                        }
                        else {
                            if (RuleTextSplitByOpeningBrace[i] != " ") {
                                RuleTextFinalTokens.push(RuleTextSplitByOpeningBrace[i]);
                                if (i < RuleTextSplitByOpeningBrace.length - 1)//not the last statement of rule
                                    RuleTextFinalTokens.push("{");
                            }
                        }
                        //                    if (i != RuleTextSplitByOpeningBrace.length - 1)
                        //                        RuleTextFinalTokens.push("}");
                    }
                    if (RuleTextSplitByClosingBrace.length > 0) {
                        for (var j = 0; j < RuleTextSplitByClosingBrace.length; j++) {
                            if (RuleTextSplitByClosingBrace[j].indexOf(";") != -1) {
                                RuleTextSplitBySemiColon = RuleTextSplitByClosingBrace[j].split(";");
                                if (RuleTextSplitBySemiColon.length > 0) {
                                    for (var k = 0; k < RuleTextSplitBySemiColon.length; k++) {
                                        if (RuleTextSplitBySemiColon[k].trim() != "") {
                                            RuleTextFinalTokens.push(RuleTextSplitBySemiColon[k].trim());
                                            if (k != RuleTextSplitBySemiColon.length - 1)
                                                RuleTextFinalTokens.push(";");
                                        }
                                    }
                                    if (j != RuleTextSplitBySemiColon.length - 1)
                                        RuleTextFinalTokens.push("}");
                                }
                            }
                            else {
                                if (RuleTextSplitByClosingBrace[j].trim() != "") {
                                    RuleTextFinalTokens.push(RuleTextSplitByClosingBrace[j]);
                                    if (RuleTextSplitByClosingBrace[j].trim().toLowerCase() == "then" || RuleTextSplitByClosingBrace[j].trim().toLowerCase() == "else") {
                                        RuleTextFinalTokens.push("{");
                                    }
                                }
                                else if (RuleTextSplitByClosingBrace[j].trim() == "" && j != RuleTextSplitByClosingBrace.length - 1) {
                                    RuleTextFinalTokens.push("}");
                                }

                                if ((self.options.grammarInfo.RuleTypeJson == "ValidationRule" || self.options.grammarInfo.RuleTypeJson == "FilterRule") && j != RuleTextSplitByClosingBrace.length - 1 && RuleTextSplitByClosingBrace[j].trim() != "") {
                                    RuleTextFinalTokens.push("}");
                                }
                            }
                            //if (j != RuleTextSplitByClosingBrace.length - 1)
                            //RuleTextFinalTokens.push("}");
                        }
                        //                    if (i != RuleTextSplitByOpeningBrace.length - 1)
                        //                        RuleTextFinalTokens.push("{");
                    }
                }
                return self._getRuleTextDictionary(RuleTextFinalTokens);
            }
        },

        _getRuleTextDictionary: function (RuleTextFinalTokens) {
            var self = this;
            var prependText = "";
            var dictionaryKey = "";
            var dictionary = [];
            var spaceCounter = 1;
            for (var i = 0; i < RuleTextFinalTokens.length; i++) {
                if (RuleTextFinalTokens[i].trim().toLowerCase() == "if") {
                    prependText += RuleTextFinalTokens[i];
                }
                else if (RuleTextFinalTokens[i].trim().toLowerCase() == "}" || RuleTextFinalTokens[i].trim().toLowerCase() == "{") {
                    if (prependText.toLowerCase().indexOf("if") != -1) {
                        if (RuleTextFinalTokens[i].trim().toLowerCase() == "{") {
                            prependText += RuleTextFinalTokens[i] + " ";
                        }
                        else {
                            //prependText += " " + RuleTextFinalTokens[i];
                        }
                    }
                    else {
                        var value = self._syntaxHighlight(RuleTextFinalTokens[i].trim());
                        if (RuleTextFinalTokens[i].trim().toLowerCase() == "{") {
                            dictionary.push({ "key": RuleTextFinalTokens[i].trim(), "value": value, "spaceCounter": spaceCounter });
                            spaceCounter++;
                        }
                        else {
                            spaceCounter--;
                            dictionary.push({ "key": RuleTextFinalTokens[i].trim(), "value": value, "spaceCounter": spaceCounter });
                        }

                    }
                }
                else if (RuleTextFinalTokens[i].trim().toLowerCase() == "then") { //|| RuleTextFinalTokens[i].trim().tLowerCase() == "else") {
                    if (i == 0) {
                        prependText += RuleTextFinalTokens[i];
                        dictionary.push({ "key": prependText, "value": prependText, "spaceCounter": spaceCounter });
                    }
                    else {
                        prependText += " } " + RuleTextFinalTokens[i];
                        dictionary.push({ "key": dictionaryKey, "value": prependText, "spaceCounter": spaceCounter });
                    }
                    dictionarykey = "";
                    prependText = "";
                }
                else if (RuleTextFinalTokens[i].trim().toLowerCase() == "else") {
                    var value = self._syntaxHighlight(RuleTextFinalTokens[i].trim());
                    dictionary.push({ "key": "else", "value": value, "spaceCounter": spaceCounter });
                }
                else {
                    if (prependText.toLowerCase().indexOf('if') > -1) {
                        dictionaryKey = RuleTextFinalTokens[i].trim(); //.replace("[", "").replace("]", "");
                        prependText += RuleTextFinalTokens[i].trim(); //.replace("[", "").replace("]", "");
                    }
                    else {
                        //                        dictionary.push({ "key": RuleTextFinalTokens[i].trim().replace("[", "").replace("]", ""), "value": RuleTextFinalTokens[i].trim().replace("[", "").replace("]", ""), "spaceCounter": spaceCounter });
                        dictionary.push({ "key": RuleTextFinalTokens[i].trim(), "value": RuleTextFinalTokens[i].trim(), "spaceCounter": spaceCounter });
                    }
                }
            }
            return dictionary;
        },

        isRuleComplete: function () {
            var self = this;
            if ($("#ruleEditorDiv").length > 0) {
                self._ruleEditorClose("", $("#ruleEditorDiv").closest(".ruleEditorActionBlocks,.ruleEditorFirstBlock").attr('id'), $(self.element).attr("id"), $("#ruleTxt").val());
            }
            self._getBlocksToSkip();
            var ruleText = self.saveRule();
            ruleText = ruleText.substr(0, ruleText.lastIndexOf("END"));
            var intellisense = JSON.parse($.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: self.options.serviceUrl + "/GetAutoComplete",
                data: JSON.stringify({ cacheKey: self.options.clientGUID, ruleText: ruleText  }),
                dataType: "json",
                async: false,
                success: function (response) {

                },
                error: function (response) {
                    console.log("err>", response);
                }
            }).responseText);
            for (var i = 0; i < intellisense.d.length; i++) {
                if (intellisense.d[i] == "END") {
                    return true;
                }
            }
            $('div[toskip="true"]').removeAttr("toskip");
            return false;
        },

        getGeneratedCode: function () {
            var self = this;
            //var ruleText = self.saveRule() + " END";

            var ruleText = self.saveRule();
            var result = JSON.parse($.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: self.options.serviceUrl + "/GetGeneratedCode",
                data: JSON.stringify({ "cacheKey": self.options.clientGUID, "ruleText": ruleText }),
                dataType: "json",
                async: false,
                success: function (response) {

                },
                error: function (response) {
                    console.log("err>", response);
                }
            }).responseText);
            return result.d;
        },

        _showRuleTextDiv: function (event) {
            $(".rulePrettyTextHover").remove();
            var left = $("#RRuleEditorBlockIDContent").width() + 10;
            var self = this;
            var text = "";
            var index = $(event.target).closest(".PrettyTextTokens").index();
            if ($(event.target).closest(".PrettyTextTokens").attr('isCondition') == "true") {
                text = $(event.target).closest(".PrettyTextTokens").attr("blocktext");
                text = text.replace("If ( [", "");
                text = text.replace("] )", "");
                text = text.replace("[", "");
                text = text.replace("]", "");
                $(".RRuleEditorPrettyText").append("<div class=\"rulePrettyTextHover\">" + text + "</div>");
            }
            else if ($(event.target).closest(".PrettyTextTokens").attr('isAction') == "true") {
                text = text.replace("] ;", "");
                text = text.replace("[", "");
                $(".RRuleEditorPrettyText").append("<div class=\"rulePrettyTextHover\">" + text + "</div>");
            }
            var top = (60 + ((index + 1) * 25)) + "px";
            $(".rulePrettyTextHover").css({ 'top': top });
            $(".rulePrettyTextHover").css({ 'left': (left + 'px') });

        },

        _hightLightRuleTextBlock: function (event) {
            var self = this;
            var blockid = $(event.target).closest(".RRuleEditorblock").attr('id');
            var block = $(".RRuleEditorBlockIDContentContent").find('div[blockid="' + blockid + '"]');
            var index = block.parent().index();
            block.parent().addClass("rulePrettyTextBlockHover");
            $($("#RRuleEditorPrettyTextContent").children()[index]).addClass("rulePrettyTextRuleHover");
        },

        _unhightLightRuleTextBlock: function (event) {
            var self = this;
            var blockid = $(event.target).closest(".RRuleEditorblock").attr('id');
            var block = $(".RRuleEditorBlockIDContentContent").find('div[blockid="' + blockid + '"]');
            var index = block.parent().index();
            block.parent().removeClass("rulePrettyTextBlockHover");
            $($("#RRuleEditorPrettyTextContent").children()[index]).removeClass("rulePrettyTextRuleHover");
        },

        _removeRuleTextDiv: function () {
            $(".rulePrettyTextHover").remove();
        },

        _highlightBlock: function (event) {
            var blockid = $(event.target).attr("blockid");
            var element = $("#" + blockid);
            if (element.attr('isconditionblock') == "true") {
                element.addClass("RRuleEditorConditionBlockHover");
            }
            else {
                element.addClass("RRuleEditorActionblockHover");
            }
        },

        _removehighlightBlock: function (event) {
            var blockid = $(event.target).attr("blockid");
            var element = $("#" + blockid);
            if (element.attr('isconditionblock') == "true") {
                element.removeClass("RRuleEditorConditionBlockHover");
            }
            else {
                element.removeClass("RRuleEditorActionblockHover");
            }
        },

        _updateBlockNameinprettyText: function (event) {
            var text = $(event.target).val();
            var parent = $(event.target).parent();
            parent.html(text);
            parent.attr("title", text);
            if ($("#RRuleEditorBlockIDContent").length > 0) {
                var blockid = parent.closest(".RRuleEditorblock").attr('id');
                var element = $("#RRuleEditorBlockIDContent").find('div[blockid="' + blockid + '"]')
                element.html(text);
            }
        },

        _editRuleEditorBlockText: function (event) {
            if (event.target.className != "RRuleEditorRuleTextEditMode") {
                var text = $(event.target).html();
                $(event.target).html("");
                $(event.target).append("<input class=\"RRuleEditorRuleTextEditMode\" value = \"" + text + "\"/>")
            }
        },

        _showOptionsOnEachBlock: function (event) {
            //alert("hello");
            if ($(event.target).closest(".RRuleEditorRuleText").length == 0) {
                var self = this;
                if (self.element.find(".RRuleEditorBlockOptionsParent").length > 0) {
                    self.element.find(".RRuleEditorBlockOptionsParent").remove();
                }
                if ($(event.target).find(".RRuleEditorBlockOptionsParent").length == 0) {
                    $(event.target).append("<div class=\"RRuleEditorBlockOptionsParent\"></div>")
                    $(event.target).find(".RRuleEditorBlockOptionsParent").append("<div class=\"RRuleEditorDeleteBlock fa fa-trash-o\"></div>")
                    $(event.target).find(".RRuleEditorBlockOptionsParent").append("<div class=\"RRuleEditorConfigBlock fa fa-cog\"></div>")
                }
            }
        },

        _removeOptionsOnEachBlock: function (event) {
            var self = this;
            if (self.element.find(".RRuleEditorBlockOptionsParent").length > 0) {
                if ($(event.target).closest(".RRuleEditorBlockOptionsParent").length == 0) {
                    self.element.find(".RRuleEditorBlockOptionsParent").remove();
                }
            }
        },

        _showFullScreen: function (event) {
            if (event.target.className == "fullscreenIcon fa fa-chevron-circle-up") {
                event.target.className = "fullscreenIcon fa fa-chevron-circle-down";
                $(event.target).closest('.RRuleEditorPrettyText').animate({ 'height': '96%' }, 500);
            }
            else if (event.target.className == "fullscreenIcon fa fa-chevron-circle-down") {
                event.target.className = "fullscreenIcon fa fa-chevron-circle-up";
                $(event.target).closest('RRuleEditorPrettyText');
                $(event.target).closest('.RRuleEditorPrettyText').animate({ 'height': '30%' }, 500);
            }

        },

        //
        _deleteEachBlock: function (event) {
            // alert("hrrlo");
            var self = this;
            var targetAttribute = [];
            if ($(event.target).parent().parent().attr("target") != null) {
                targetAttribute = $(event.target).parent().parent().attr("target").split(',');

                for (var i = 0; i < targetAttribute.length; i++) {
                    $("#" + $(event.target).parent().parent().attr("target").split(',')[i]).removeAttr("source")
                }
            }
            self.instance.detachAllConnections($(event.target).parent().parent(), [])
            while ($(event.target).parent().parent().next().hasClass("jsplumb-endpoint"))//removing the endpoints
            {
                $(event.target).parent().parent().next().remove();
            }
            $(event.target).parent().parent().remove();
            // jsplumb.empty($(event.target).parent().parent().attr('id'));
        },
        // to set source and target of reattached blocks
        _setSourceAndTargetOfReattachedBlocks: function (conn) {
            $("#" + conn.originalTargetId).removeAttr("source");
            //$("#" + conn.newTargetId).attr({ "source": conn.newSourceId });
            var targetsToBeRemoved = [];
            targetsToBeRemoved = $("#" + conn.originalSourceId).attr("target").split(",").splice(conn.originalTargetId, 1);
            $("#" + conn.originalSourceId).removeAttr("target");
            var targetReset = "";
            for (var i = 0; i < targetsToBeRemoved.length; i++) {
                targetReset += targetsToBeRemoved[i] + ","

            }
            // targetReset += conn.newTargetId;
            $("#" + conn.originalSourceId).attr("target", targetReset);
        },

        // to set the source and target of each connected block
        _setSourceAndTargetOfConnectedBlocks: function (conn) {
            var source = conn.source;
            var target = conn.target;
            //if ($("#" + conn.targetId).attr("isactionblock") == "true") {
            if ($(conn.sourceEndpoint.canvas).hasClass("_false")) {
                $(target).attr("thenorelseblock", "else");
            }
            if ($(conn.sourceEndpoint.canvas).hasClass("_true")) {
                $(target).attr("thenorelseblock", "then");
            }
            // }
            if ($(source).attr("target") == null) {
                $(source).attr("target", target.id);
            }
            else {
                if ($(conn.sourceEndpoint.canvas).hasClass("_false")) {
                    var existingTarget = $(source).attr("target") + "," + conn.target.id;
                }
                else {
                    var existingTarget = conn.target.id + "," + $(source).attr("target");
                }
                $(source).attr("target", existingTarget);

            }
            if ($(target).attr("source") == null) {
                $(target).attr("source", source.id);
            }
            else {
                var existingSource = $(target).attr("source") + "," + conn.source.id;
                $(target).attr("source", existingSource);
            }
        },

        // TO Open Rule Editor
        _openruleEditor: function (event) {
            var self = this;
            self._setruleEditorPosition(event);
            var sourceid = $(event.target).closest(".RRuleEditorblock").attr("source");
            self.options.ruleText = "If { ";
            if (sourceid != null) {
                //                prependText = $("#" + sourceid).attr('ruleText');
                self.options.ruleText = $("#" + sourceid).attr('ruleText');
                var elseBlockid = "";
                if ($("#" + sourceid).attr("target") != null) {
                    var targets = $("#" + sourceid).attr("target").split(',');
                    if (targets.length > 1) {
                        elseBlockid = targets[1];
                    }
                }
                self._getRuleTextFromSource(sourceid, $(event.target).closest(".RRuleEditorblock").attr('id'), elseBlockid);
            }

            if ($("#ruleEditor").data('ruleTextEngine') == undefined) {
                $("#ruleEditor").ruleTextEngine({ ruleText: "",
                    grammarInfo: self.options.grammarInfo,
                    prependText: self.options.ruleText,
                    closeRuleEditor: self._ruleEditorClose,
                    // jsplumbId: self.element.attr('id'),
                    blockid: $(event.target).closest(".RRuleEditorblock").attr("id"),
                    hasFormattingSection: false,
                    jsplumbId: $(self.element).attr("id")
                });
                var left = $(document).width() - self.options.CLientX - 300;
                $("#ruleEditor").animate({ 'width': left }, 500);
                $("#autcompleteContainer").css({ 'display': 'none' });
            }
            else {
                var clientGuid = "";
                if ($(event.target).closest(".RRuleEditorblock").attr('currentBlockText') == null) {
                    if ($(event.target).closest(".RRuleEditorblock").attr("IsConditionBlock")) {
                        if ($(event.target).closest(".RRuleEditorblock").attr("thenorelseblock") == "then") {
                            self.options.ruleText = self.options.ruleText + " Then { If { ";
                        }
                        else {
                            self.options.ruleText = self.options.ruleText + " Else { If { ";
                        }
                    }
                    else {
                        if ($(event.target).closest(".RRuleEditorblock").attr("thenorelseblock") == "then") {
                            self.options.ruleText = self.options.ruleText + " Then { ";
                        }
                        else if ($(event.target).closest(".RRuleEditorblock").attr("thenorelseblock") == "else") {
                            self.options.ruleText = self.options.ruleText + " Else { [";
                        }
                        else {
                            self.options.ruleText = self.options.ruleText + " ";
                        }
                    }
                    var currentBlockText = "";
                    if ($(event.target).closest(".RRuleEditorblock").attr('currentBlockText') != null) {
                        currentBlockText = currentBlockText + $(event.target).closest(".RRuleEditorblock").attr('currentBlockText')
                    }
                    if (self.options.currentBlockTextObj.hasOwnProperty(currentBlockText)) {
                        delete self.options.currentBlockTextObj[currentBlockText];
                    }
                    clientGuid = $("#ruleEditor").data('ruleTextEngine').ResetSettings(self.options.ruleText, $(event.target).closest(".RRuleEditorblock").attr('id'), currentBlockText);
                    // clientGuid = $("#ruleEditor").data('ruleEngine').ResetSettings(self.options.ruleText, $(event.target).closest(".RRuleEditorblock").attr('id'));
                    if (currentBlockText != "") {
                        self.options.isEditMode = true;
                    }
                    $("#ruleEditor").data('ruleTextEngine').getNextTokens(clientGuid, currentBlockText);
                    $("#ruleEditor").css({ 'width': '0px' });
                    $("#ruleEditor").show();
                    var left = $(document).width() - self.options.CLientX - 300;
                    $("#ruleEditor").animate({ 'width': left }, 500);
                }
                else {
                    self._openRuleEditorInEditMode(event);
                }
            }
            $(event.target).closest(".RRuleEditorblock").css({ 'z-index': 100 });
            self.element.find("#prettyText").show();
        },

        _openRuleEditorInEditMode: function (event) {
            var self = this;
            var currentBlockText = "";
            var sourceid = null;
            var actualElement = $(event.target).closest(".RRuleEditorblock");
            if (actualElement.attr('isfirstblock') == null) {
                sourceid = actualElement.attr('isfirstblock');
                if (actualElement.attr('IsConditionBlock')) {
                    if (actualElement.attr("thenorelseblock") == "then") {
                        self.options.ruleText = self.options.ruleText + " Then { If { ";
                    }
                    else {
                        self.options.ruleText = self.options.ruleText + " Else { If { ";
                    }
                }
                else {
                    if (actualElement.attr("thenorelseblock") == "then") {
                        self.options.ruleText = self.options.ruleText + " Then { ";
                    }
                    else if (actualElement.attr("thenorelseblock") == "else") {
                        self.options.ruleText = self.options.ruleText + " Else { ";
                    }
                    else {
                        self.options.ruleText = self.options.ruleText + " ";
                    }
                }
            }
            if ($(event.target).closest(".RRuleEditorblock").attr('currentBlockText') != null) {
                currentBlockText = currentBlockText + $(event.target).closest(".RRuleEditorblock").attr('currentBlockText')
            }
            if (self.options.currentBlockTextObj.hasOwnProperty(currentBlockText.trim())) {
                delete self.options.currentBlockTextObj[currentBlockText.trim()];
            }
            clientGuid = $("#ruleEditor").data('ruleTextEngine').ResetSettings(self.options.ruleText, $(event.target).closest(".RRuleEditorblock").attr('id'), currentBlockText);
            // clientGuid = $("#ruleEditor").data('ruleEngine').ResetSettings(self.options.ruleText, $(event.target).closest(".RRuleEditorblock").attr('id'));
            if (currentBlockText != "") {
                self.options.isEditMode = true;
            }
            $("#ruleEditor").data('ruleTextEngine').getNextTokens(clientGuid, currentBlockText);
            $("#ruleEditor").css({ 'width': '0px' });
            $("#ruleEditor").show();
            var left = $(document).width() - self.options.CLientX - 300;
            $("#ruleEditor").animate({ 'width': left }, 500);
        },

        _setruleEditorPosition: function (event) {
            var blockId = $(event.target).closest(".RRuleEditorblock");
            var clientx = event.clientX;
            this.options.CLientX = (clientx - 18);
            var clienty = (event.clientY + 13) + 'px';
            $("#ruleEditor").css({ 'top': clienty });
            if ((clientx + $("#ruleEditor").width()) < $(document).width()) {
                var width = (clientx - 18) + "px";
                $("#ruleEditor").css({ 'left': width });
            }
            else if ((((clientx + $("#ruleEditor").width()) - $(document).width())) < 0) {
                var width = (($(document).width() - (clientx + $("#ruleEditor").width())) / 2) + "px";
                $("#ruleEditor").css({ 'left': width });
            }
            else {
                $("#ruleEditor").css({ 'left': (((clientx + $("#ruleEditor").width()) - $(document).width()) + "px") });
            }
        },

        _getRuleTextFromSource: function (sourceid, currentBlockId, elseBlockid) {
            var self = this;

            if ($("#" + sourceid).attr("target") != null) {
                var targetIds = $("#" + sourceid).attr("target").split(',');
                for (var i = 0; i < targetIds.length; i++) {
                    if (targetIds[i] != currentBlockId && targetIds[i] != elseBlockid) {
                        //if ($("#" + targetIds[i]).attr("") == "else") {
                        //   self.options.ruleText = ' ' + self.options.ruleText + ' ' + $("#" + targetIds[i]).attr('ruleText');
                        //}
                        //self.options.ruleText = ' ' + self.options.ruleText + ' ' + $("#" + targetIds[i]).attr('ruleText');
                        self.options.ruleText = $("#" + targetIds[i]).attr('ruleText');
                        self._getRuleTextFromSource(targetIds[i], currentBlockId, elseBlockid);
                    }
                    else if (targetIds[i] == currentBlockId) {
                        break;
                    }
                }
            }
        },

        _ruleEditorClose: function (ruleText, blockid, jsPlumbId, currentBlockText) {

            $("#" + blockid).addClass("RADHighlightBackground");
            var self = $("#" + jsPlumbId).data("radWidgetRRuleEditor");
            if (ruleText.toLowerCase().trim().endsWith("end")) {
                var grammarInfo = self.options.grammarInfo;
                var element = self.element;
                self._destroy();
                //element.RRuleEditor({ grammarInfo: grammarInfo,  ruleText: ruleText });
                self.options.ruleText = ruleText;
                self._create();
            }
            else {
                var element = $("#" + blockid).closest('.ruleEditorBlocks');
                if ($(".RRuleEditorPrettyTextContent").find('div[isJustParent="true"]').length == 0) {
                    if (element.closest('div[isJustParent="true"]').length == 0) {
                        var div = $("<div>");
                        div.attr("spaceCounter", parseInt(self.options.spaceCounter));
                        div.attr("isJustParent", true);
                        if (self.options.idCount > 0)
                            div.css({ 'margin-left': "45px" });
                        element.wrap(div);
                        element.removeClass("ruleEditorFirstBlock");
                        element.addClass("ruleEditorActionBlocks");
                        element.css({ 'margin-left': "50px" });
                    }
                }
                var counter = element.attr('spaceCounter');
                if (element.attr('isIfBlock') || element.attr('isThenBlock')) {
                    if (element.attr('isIfBlock')) {
                        element.attr('currentRuleText', 'If { ' + currentBlockText + ' } Then ');
                        element.attr('actualRuleText', currentBlockText);
                    }
                    else if (element.attr('isThenBlock')) {
                        element.attr('currentRuleText', 'Then { ' + currentBlockText + ' }');
                        element.attr('actualRuleText', currentBlockText);
                    }
                    else {
                        if (self.options.grammarInfo.RuleTypeJson != "AggregationRule") {
                            if (currentBlockText.indexOf(";") != -1) {
                                element.attr('currentRuleText', currentBlockText.split(';')[0] + ";");
                                element.attr('actualRuleText', currentBlockText.split(';')[0] + ";");
                                self._appendActionBlocks(element, currentBlockText);
                            }
                            else {
                                element.attr('currentRuleText', currentBlockText.trim() != "" ? currentBlockText + " ; " : currentBlockText);
                                element.attr('actualRuleText', currentBlockText.trim() != "" ? currentBlockText + "" : currentBlockText);
                                if (currentBlockText != "") {
                                    //self._appendEmptyBlock(element);
                                }
                            }
                        }
                        else {
                            element.attr('currentRuleText', currentBlockText.trim() != "" ? currentBlockText + " ; " : currentBlockText);
                            element.attr('actualRuleText', currentBlockText.trim() != "" ? currentBlockText + "" : currentBlockText);
                            if (currentBlockText != "") {
                                //self._appendEmptyBlock(element);
                            }
                        }
                    }
                }
                else {
                    if (self.options.grammarInfo.RuleTypeJson == "ValidationRule" || self.options.grammarInfo.RuleTypeJson == "FilterRule") {
                        element.attr('currentRuleText', currentBlockText.trim() != "" ? currentBlockText : currentBlockText);
                        element.attr('actualRuleText', currentBlockText.trim() != "" ? currentBlockText : currentBlockText);
                        if (currentBlockText != "") {
                            //self._appendEmptyBlock(element);
                        }
                    }
                    else if (self.options.grammarInfo.RuleTypeJson != "AggregationRule") {
                        if (currentBlockText.indexOf(";") != -1) {
                            element.attr('currentRuleText', currentBlockText.split(';')[0] + ";");
                            element.attr('actualRuleText', currentBlockText.split(';')[0]);
                            self._appendActionBlocks(element, currentBlockText);
                        }
                        else {
                            element.attr('currentRuleText', currentBlockText.trim() != "" ? currentBlockText + " ; " : currentBlockText);
                            element.attr('actualRuleText', currentBlockText.trim() != "" ? currentBlockText + "" : currentBlockText);
                            if (currentBlockText != "") {
                                //self._appendEmptyBlock(element);
                            }
                        }
                    }
                    else {
                        if (currentBlockText.trim().endsWith(';')) {
                            element.attr('currentRuleText', currentBlockText);
                            element.attr('actualRuleText', currentBlockText);
                        }
                        else {
                            element.attr('currentRuleText', currentBlockText.trim() != "" ? currentBlockText + " ; " : currentBlockText);
                            element.attr('actualRuleText', currentBlockText.trim() != "" ? currentBlockText + "" : currentBlockText);
                        }
                        if (currentBlockText != "") {
                            //self._appendEmptyBlock(element);
                        }
                    }
                }
                var appendElement = $("#" + blockid);
                if (currentBlockText.trim() == "") {
                    element.attr('toSkip', true);
                }
                else {
                    element.removeAttr('toSkip');
                }
                if (appendElement.hasClass("ruleEditorEditingBlock")) {
                    appendElement.append(self._syntaxHighlight(currentBlockText));
                }
                else {
                    if (self.options.grammarInfo.RuleTypeJson != "AggregationRule") {
                        if (currentBlockText.indexOf(';') != -1) {
                            appendElement.append(self._syntaxHighlight(currentBlockText.split(';')[0]));
                            //appendElement.css({ 'margin-left': (counter * 30) + 'px' });
                        }
                        else {
                            if (currentBlockText.trim() != "")
                                appendElement.append(self._syntaxHighlight(currentBlockText));
                            //appendElement.css({ 'margin-left': (counter * 30) + 'px' });
                        }
                    }
                    else {
                        if (currentBlockText.trim() != "")
                            appendElement.append(self._syntaxHighlight(currentBlockText));
                        //appendElement.css({ 'margin-left': (counter * 30) + 'px' });
                    }
                }
                if ($("#" + blockid).find("#ruleEditorDiv").data('ruleTextEngine') != undefined) {
                    self.options.clientGUID = $("#" + blockid).find("#ruleEditorDiv").data('ruleTextEngine').getClientGuid();
                    $("#" + blockid).find("#ruleEditorDiv").data('ruleTextEngine').Destroy();
                    $("#ruleEditorDiv").remove();
                }
                element.focus();
                if ($("#" + blockid).hasClass("ruleEditorEditingBlock")) {
                    /*$("#" + blockid).css({ 'width': '' });*/
                    $("#" + blockid).removeClass("ruleEditorEditingBlockWidth");
                }
                self.options.blockidCheck = blockid;
                if (self.options.grammarInfo.RuleTypeJson != "ValidationRule" && self.options.grammarInfo.RuleTypeJson != "AggregationRule") {
                    self._appendEmptyBlockBeforeClosingBrac();
                }
                self._setHeightAcctoBlock();
            }
            $("#" + blockid).focus();
        },

        _setHeightAcctoBlock: function () {
            var self = this;
            var height = $("#" + self.options.blockidCheck).closest(".ruleEditorBlocks").height();
            var index = $("#" + self.options.blockidCheck).closest(".ruleEditorBlocks").index();
            $($(".RRuleEditorBlockIDContentContent").children()[index]).height(height);
        },

        _appendEmptyBlockBeforeClosingBrac: function () {
            var self = this;
            var elemenCollection = $("#RRuleEditorPrettyTextContent").find("div[ifblockidbrac]");
            for (var i = 0; i < elemenCollection.length; i++) {
                //if ($(elemenCollection[i]).prev().hasClass("ruleEditorActionBlocks") && $(elemenCollection[i]).prev().html() != "") {
                if ($(elemenCollection[i]).prev().html() != "") {
                    self._appendEmptyBlock($(elemenCollection[i]), true, $("#" + $(elemenCollection[i]).attr("ifblockidbrac")).attr("spacecounter"));
                }
            }
            elemenCollection = $("#RRuleEditorPrettyTextContent").find("div[elseblockid]");
            for (var i = 0; i < elemenCollection.length; i++) {
                //if ($(elemenCollection[i]).prev().hasClass("ruleEditorActionBlocks") && $(elemenCollection[i]).prev().html() != "") {
                if ($(elemenCollection[i]).prev().html() != "") {
                    self._appendEmptyBlock($(elemenCollection[i]), true);
                }
            }
            if ($("#RRuleEditorPrettyTextContent").find("div[ifblockidbrac]").length == 0 && $("#RRuleEditorPrettyTextContent").find("div[elseblockid]").length == 0) {
                if ($(".ruleEditorBlocks").last().html() != "") {
                    self._appendEmptyBlock($(".ruleEditorBlocks").last(), false);
                }
            }
        },

        _appendEmptyBlock: function (element, appendBefore, spaceCounter) {
            var self = this;
            self.options.idCount++;
            spaceCounter = element.prev().attr('spacecounter') - 1;
            var div = $('<div>');
            div.attr('id', "ruleEditorBlock" + self.options.idCount)
            div.attr('spacecounter', element.prev().attr('spacecounter'));
            div.attr('tabindex', 1);
            div.attr('currentRuleText', "");
            div.attr('spacecounter', parseInt(spaceCounter) + 1);
            div.css({ 'margin-left': "50px" });
            div.attr('actualRuleText', "");
            div.addClass("ruleEditorActionBlocks");
            div.addClass("ruleEditorBlocks");
            div.addClass("ruleBlocks");
            if (appendBefore)
                div.insertBefore(element);
            else
                div.insertAfter(element);


            //            if (self.options.blockidCheck != "") {
            //                if ($("#" + self.options.blockidCheck).closest(".ruleEditorBlocks").length > 0) {
            //                    if ($("#" + self.options.blockidCheck).closest(".ruleEditorBlocks").attr('isifblock') == null) {
            //                        var index = element.index() - 1;
            //                        var parent = $(".RRuleEditorBlockIDContentContent");
            //                        div = $('<div>');
            //                        div.attr('id', "ruleEditorContent" + parent.children().length);
            //                        div.addClass("showHideContent");
            //                        div.insertAfter($(parent.children()[index - 1]));
            //                    }

            //                }
            //            }
            //            else {
            //                var index = element.index() - 1;
            //                var parent = $(".RRuleEditorBlockIDContentContent");
            //                div = $('<div>');
            //                div.attr('id', "ruleEditorContent" + parent.children().length);
            //                div.addClass("showHideContent");
            //                div.insertAfter($(parent.children()[index - 1]));
            //            }
        },


        _appendActionBlocks: function (element, currentText) {
            self = this;
            var currentRuleText = currentText.split(';');
            currentRuleText.splice(0, 1);

            var index = element.index() - 1;
            var parent = $(".RRuleEditorBlockIDContentContent");
            var div;
            for (var i = 0; i < currentRuleText.length; i++) {
                if (currentRuleText[i].trim() != "") {
                    self.options.idCount++;
                    div = $('<div>');
                    div.attr('id', "ruleEditorBlock" + self.options.idCount);
                    div.attr('spacecounter', element.attr('spacecounter'));
                    div.attr('tabindex', 1);
                    div.attr('currentRuleText', currentRuleText[i] + " ; ");
                    div.attr('actualRuleText', currentRuleText[i]);
                    div.addClass("ruleEditorActionBlocks");
                    div.addClass("ruleEditorBlocks");
                    div.addClass("ruleBlocks");
                    div.html(self._syntaxHighlight(currentRuleText[i]));
                    div.css({ 'margin-left': "50px" });
                    div.insertAfter(element);
                    element = div;

                    div = $('<div>');
                    div.attr('id', "ruleEditorContent" + parent.children().length);
                    div.addClass("showHideContent");
                    div.insertAfter($(parent.children()[index + i]));
                }
            }


            for (var i = 0; i < currentRuleText.length; i++) {

            }
            //self._appendEmptyBlock(element);
        },

        _populateBlockandRuleTextMapp: function (ruleText, blockid, isEditMode) {
            var self = this;
            if (!isEditMode) {
                self.options.blockRuleTextMap.push({ "blockid": blockid, "ruleText": ruleText, isfound: false });
            }
            else {
                for (var i = 0; i < self.options.blockRuleTextMap.length; i++) {
                    if (self.options.blockRuleTextMap[i].blockid == blockid) {
                        self.options.blockRuleTextMap[i].ruleText = ruleText;
                        break;
                    }
                }
            }
        },

        _resetFoundFlaginTextMap: function () {
            var self = this;
            for (var i = 0; i < self.options.blockRuleTextMap.length; i++) {
                self.options.blockRuleTextMap[i].isfound = false;
            }
        },

        _getRuleTextforBlockId: function (ruleText) {
            var self = this;
            var blockid = "";
            for (var i = 0; i < self.options.blockRuleTextMap.length; i++) {
                if (self.options.blockRuleTextMap[i].ruleText == ruleText && !self.options.blockRuleTextMap[i].isfound) {
                    blockid = self.options.blockRuleTextMap[i].blockid;
                    self.options.blockRuleTextMap[i].isfound = true;
                    break;
                }
            }
            return blockid;
        },

        _createPrettyText: function (ruleText, currentBlockText, jsPlumbId) {
            var instance = $("#" + jsPlumbId).data("radWidgetRRuleEditor");
            var self = instance;
            self.options.CounterArray = [];
            var RuleTextSplitByOpeningBrace = [];
            var RuleTextFinalTokens = [];
            $("#RRuleEditorPrettyTextContent").html("");
            $("#RRuleEditorBlockIDContent").html("");
            if (ruleText != "If { } Then ") {
                RuleTextSplitByOpeningBrace = ruleText.split("{");
                for (var i = 0; i < RuleTextSplitByOpeningBrace.length; i++) {
                    var RuleTextSplitByClosingBrace = [];
                    var RuleTextSplitBySemiColon = [];
                    if (RuleTextSplitByOpeningBrace[i].indexOf("}") != -1) {
                        RuleTextSplitByClosingBrace = RuleTextSplitByOpeningBrace[i].split("}");
                    }

                    else {
                        if (RuleTextSplitByOpeningBrace[i] != " ") {
                            RuleTextFinalTokens.push(RuleTextSplitByOpeningBrace[i]);
                            //RuleTextFinalTokens.push("{");
                        }
                        if (i != RuleTextSplitByOpeningBrace.length - 1)
                            RuleTextFinalTokens.push("{");

                    }
                    if (RuleTextSplitByClosingBrace.length > 0) {
                        for (var j = 0; j < RuleTextSplitByClosingBrace.length; j++) {
                            if (RuleTextSplitByClosingBrace[j].indexOf("] ;  [") != -1) {
                                RuleTextSplitBySemiColon = RuleTextSplitByClosingBrace[j].split(";");
                                if (RuleTextSplitBySemiColon.length > 0) {
                                    for (var k = 0; k < RuleTextSplitBySemiColon.length; k++) {
                                        if (RuleTextSplitBySemiColon[k] != " ") {
                                            RuleTextFinalTokens.push(RuleTextSplitBySemiColon[k].trim());
                                            //if (k != RuleTextSplitBySemiColon.length - 1)
                                            //RuleTextFinalTokens.push(";");
                                        }
                                    }
                                    if (j != RuleTextSplitBySemiColon.length - 1)
                                        RuleTextFinalTokens.push("}");
                                }
                            }
                            else {
                                if (RuleTextSplitByClosingBrace[j] != " ") {
                                    RuleTextFinalTokens.push(RuleTextSplitByClosingBrace[j]);
                                }
                                if (j != RuleTextSplitByClosingBrace.length - 1)
                                    RuleTextFinalTokens.push("}");

                            }
                        }
                        if (i != RuleTextSplitByOpeningBrace.length - 1)
                            RuleTextFinalTokens.push("{");
                    }

                }
            }

            RuleTextFinalTokens = $("#" + jsPlumbId).data("radWidgetRRuleEditor")._indentRule(RuleTextFinalTokens);
            var prependText = "";
            var blockid = "";
            var counter = 0;
            var flag = false;
            for (var i = 0; i < RuleTextFinalTokens.length; i++) {
                if (RuleTextFinalTokens[i].trim() != ")") {
                    blockid = "";
                }
                flag = false;
                if (RuleTextFinalTokens[i].toLowerCase().trim() == "if" || RuleTextFinalTokens[i].trim() == "(" || RuleTextFinalTokens[i].trim() == ")") {
                    prependText = prependText + RuleTextFinalTokens[i];
                }
                else if (RuleTextFinalTokens[i - 1].trim() == "(") {
                    prependText = prependText + RuleTextFinalTokens[i];
                }
                else {
                    if (RuleTextFinalTokens[i].trim() != ")") {
                        prependText = RuleTextFinalTokens[i]
                    }
                    flag = true;
                }
                if (RuleTextFinalTokens[i].trim() == ")") {
                    flag = true;
                }
                if (flag) {
                    $("#RRuleEditorPrettyTextContent").append("<div id=\"Tokens" + counter + "\"class=\"PrettyTextTokens\"></div>");
                    $("#RRuleEditorBlockIDContent").append("<div id=\"Blocks" + counter + "\"class=\"PrettyTextTokens\"></div>");
                }
                var splitTokenWithOpeningSquare = RuleTextFinalTokens[i].replace("[", "");
                var splitTokenWithClosingSquareWithColon = splitTokenWithOpeningSquare.replace("] ;", "");
                var lastIndex = splitTokenWithClosingSquareWithColon.lastIndexOf("]");
                splitTokenWithClosingSquare = splitTokenWithClosingSquareWithColon.slice(0, lastIndex) + splitTokenWithClosingSquareWithColon.slice(lastIndex).replace("]", ""); // splitTokenWithClosingSquareWithColon.replace("]", "");
                if (instance.options.currentBlockTextObj[splitTokenWithClosingSquare.trim()] != undefined) {
                    blockid = instance._getRuleTextforBlockId(splitTokenWithClosingSquare.trim());
                    //blockid = instance.options.currentBlockTextObj[splitTokenWithClosingSquare.trim()];
                }
                if (flag) {
                    if (blockid != "") {
                        if ($("#" + blockid).attr("isconditionblock") == "true") {
                            $("#Blocks" + counter).prepend("<div  class=\"PrettyTextBlocks PrettyTextBlocksCondition\"  blockid = \"" + blockid + "\">" + $("#" + blockid).find(".RRuleEditorRuleText").html() + "</div>");
                            $("#Tokens" + counter).attr("isCondition", "true");
                            $("#Tokens" + counter).attr("blockText", prependText);
                        }
                        else {
                            $("#Blocks" + counter).prepend("<div  class=\"PrettyTextBlocks PrettyTextBlocksAction\"  blockid = \"" + blockid + "\">" + $("#" + blockid).find(".RRuleEditorRuleText").html() + "</div>");
                            $("#Tokens" + counter).attr("isAction", "true");
                            $("#Tokens" + counter).attr("blockText", prependText);
                        }
                        $("#Blocks" + counter).addClass("BlockIdText");
                        $("#Tokens" + counter).addClass("prettyTextRule");
                        $("#Tokens" + counter).attr("isCondition", "true");
                    }
                    else {
                        $("#Blocks" + counter).prepend("<div class=\"PrettyTextBlocksEmpty\">" + "</div>");
                    }

                    $("#Tokens" + counter).append(self._appendSpace(self.options.CounterArray[counter]) + $("#" + jsPlumbId).data("radWidgetRRuleEditor")._syntaxHighlight(prependText));

                }

                if (flag) {
                    counter++;
                    prependText = "";
                }
            }
        },
        _indentRule: function (RuleTextFinalTokens) {
            var self = this;
            var counter = 0;
            self.options.CounterArray = [];
            // $("#RRuleEditorPrettyTextContent").append(RuleTextFinalTokens[i] + "<br/>");
            for (var i = 0; i < RuleTextFinalTokens.length; i++) {
                if (RuleTextFinalTokens[i] == " ") {
                    //  continue;
                }
                //console.log(words[i]);
                if (i != 0) {
                    if (RuleTextFinalTokens[i - 1].trim() == "if" || RuleTextFinalTokens[i - 1].trim() == "If") {
                        // console.log("true");
                        //++counter;
                        //self.options.CounterArray.push(counter);
                        RuleTextFinalTokens[i] = "(";
                    }
                    else if (RuleTextFinalTokens[i - 1].trim() == "else" || RuleTextFinalTokens[i - 1].trim() == "Else") {
                        self.options.CounterArray.push(counter);
                        RuleTextFinalTokens[i] = "\n" + RuleTextFinalTokens[i];
                    }
                    else if (RuleTextFinalTokens[i - 1].trim() == "Then") {
                        //++counter;
                        self.options.CounterArray.push(counter);
                        RuleTextFinalTokens[i] = "\n" + RuleTextFinalTokens[i];
                    }
                    //                        if (RuleTextFinalTokens[i - 1].trim() == "&&" || RuleTextFinalTokens[i - 1].trim() == "||" || RuleTextFinalTokens[i - 1].trim() == ';') {
                    //                            if (RuleTextFinalTokens[i + 1] != "END") {
                    //                                RuleTextFinalTokens[i] = RuleTextFinalTokens[i] + "\n\t";
                    //                            }
                    //                            else {
                    //                                RuleTextFinalTokens[i] = RuleTextFinalTokens[i] + "\n";
                    //                            }
                    //                        }
                    else if (RuleTextFinalTokens[i - 1].trim() == "{") {
                        ++counter;
                        RuleTextFinalTokens[i] = "\n" + RuleTextFinalTokens[i];
                        self.options.CounterArray.push(counter);
                    }
                    else if (RuleTextFinalTokens[i].trim() == "}") {
                        if (RuleTextFinalTokens[i - 2].trim() == "(") {
                            RuleTextFinalTokens[i] = ")"
                        }
                        else {
                            RuleTextFinalTokens[i] = "\n" + RuleTextFinalTokens[i];
                            --counter;
                            self.options.CounterArray.push(counter);
                        }
                    }
                    else if (RuleTextFinalTokens[i - 1].trim() == "}" || RuleTextFinalTokens[i - 1].trim() == ")") {

                        RuleTextFinalTokens[i] = "\n" + RuleTextFinalTokens[i];
                        self.options.CounterArray.push(counter);
                    }
                    else if (RuleTextFinalTokens[i].trim() == "Else") {

                        RuleTextFinalTokens[i] = "\n" + RuleTextFinalTokens[i];
                        //--counter;
                        self.options.CounterArray.push(counter);
                    }
                    else {
                        if (RuleTextFinalTokens[i - 1] != "(") {
                            RuleTextFinalTokens[i] = "\n" + RuleTextFinalTokens[i];
                            self.options.CounterArray.push(counter);
                        }
                    }
                }
                else {
                    self.options.CounterArray.push(counter);
                }
                //  finalStr += words[i] + " ";
            }
            return RuleTextFinalTokens;
        },
        _appendSpace: function (counter) {
            var spaces = "";
            for (var i = 0; i < counter; i++) {
                spaces += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            }
            return spaces;
        },

        _getRuleIds: function () {
            var self = this;
            var ruleIds = [];
            $(self.options.grammarInfo.Columns).each(function (i, e) {
                ruleIds.push(e.ColumnName);
            });
            $(self.options.grammarInfo.RADXRuleCustomOpInfo).each(function (i, e) {
                if (e.Key) {
                    ruleIds.push(e.Key);
                }
            });
            return ruleIds;
        },

        _syntaxHighlight: function (input) {
            var self = this;
            var tabCount = 0;
            var self = this;
            self.ruleIds = [];
            if ($("#ruleEditorDiv").data('ruleTextEngine') != undefined) {
                self.ruleIds = $("#ruleEditorDiv").data('ruleTextEngine').FillColumns();
            }
            else {
                self.ruleIds = self._getRuleIds();
            }
            var output = [];
            // output = input.replace(/(\w+)\b/g, function (m) {
            var parentDiv = "";
            output = input.trim().split(" ");
            var columnName = "";
            var aggregateFuncs = "";
            for (var j = 0; j < output.length; j++) {
                if (self.keywords.indexOf(output[j]) > -1) {
                    //return "<span class='highlight-keywords'>" + output[j] + "</span>";
                    parentDiv += "<span class='highlight-keywords'>" + output[j] + "</span>&nbsp;";
                }
                else if (self.operators.indexOf(output[j]) > -1) {
                    //  return "<span class='highlight-operators'>" + output[j] + "</span>";
                    parentDiv += "<span class='highlight-ruleIds'>" + columnName + "</span>&nbsp;<span class='highlight-operators'>" + output[j] + "</span>&nbsp;";
                    columnName = "";
                } else if (self.comparisonop.indexOf(output[j]) > -1) {
                    // return "<span class='highlight-comparisonop'>" + output[j] + "</span>";
                    if (columnName != "") {
                        parentDiv += "<span class='highlight-ruleIds'>" + columnName + "</span>&nbsp;<span class='highlight-comparisonop'>" + output[j] + "</span>&nbsp;";
                        columnName = "";
                    }
                    else {
                        parentDiv += "<span class='highlight-comparisonop'>" + output[j] + "</span>";
                    }
                }
                else if (self.aggFunc.indexOf(output[j]) > -1) {
                    // return "<span class='highlight-aggFunc'>" + output[j] + "</span>";
                    parentDiv += "<span class='highlight-ruleIds'>" + columnName + "</span>&nbsp;<span class='highlight-aggFunc'>" + output[j] + "</span>";
                    columnName = "";
                }
                else if (self.logicalOperators.indexOf(output[j]) > -1) {
                    //  return "<span class='highlight-ruleIds'>" + columnName + "</span>&nbsp;<span class='highlight-operators'>" + output[j] + "</span>";
                    parentDiv += "<span class='highlight-ruleIds'>" + columnName + "</span>&nbsp;<span class='highlight-operators'>" + output[j] + "</span>&nbsp;"
                    columnName = "";
                }
                else if (self.logicalOperators.indexOf(output[j]) > -1) {
                    //  return "<span class='highlight-ruleIds'>" + columnName + "</span>&nbsp;<span class='highlight-operators'>" + output[j] + "</span>";
                    parentDiv += "<span class='highlight-ruleIds'>" + columnName + "</span>&nbsp;<span class='highlight-operators'>" + output[j] + "</span>&nbsp;"
                    columnName = "";
                }
                else if (self.colon.indexOf(output[j]) > -1) {
                    //  return "<span class='highlight-ruleIds'>" + columnName + "</span>&nbsp;<span class='highlight-operators'>" + output[j] + "</span>";
                    parentDiv += "<span class='highlight-ruleIds'>" + columnName + "</span>&nbsp;<span class='highlight-operators'>" + output[j] + "</span>&nbsp;"
                    columnName = "";
                }
                else if (self.brackets.indexOf(output[j]) > -1) {
                    // return output[j];
                    if (output[j] == "]") {
                        parentDiv += "<span class='highlight-ruleIds'>" + columnName + "</span>";
                        columnName = "";
                    }
                    if (output[j] == "(") {
                        parentDiv += "<span class='highlight-ruleIds'>" + columnName + "</span>";
                        parentDiv += output[j] + "&nbsp;";
                        columnName = "";
                    }
                    else if (output[j] == ")") {
                        parentDiv += "<span class='highlight-ruleIds'>" + columnName + "</span>";
                        parentDiv += output[j] + "&nbsp;";
                        columnName = "";
                    }
                    else if (output[j] == "[") {
                        parentDiv += output[j] + "&nbsp;";
                    }
                    else if (output[j] == "]") {
                        parentDiv += "&nbsp;" + output[j] + "&nbsp;";
                    }
                    else {
                        parentDiv += output[j];
                    }
                }
                else if (output[j].endsWith("&nbsp;If") || output[j].endsWith("&nbsp;Then") || output[j].endsWith("&nbsp;Else")) {
                    parentDiv += "<span class='highlight-operators'>" + output[j] + "</span>";
                }
                else if (output[j].endsWith("&nbsp;{") || output[j].endsWith("&nbsp;}")) {
                    parentDiv += output[j];
                }
                else if (output[j].indexOf("\"") > -1) {
                    parentDiv += "&nbsp;" + output[j];
                }
                else if (self.ruleIds.length > 0) {
                    var div = "";
                    //  var columnName = "";

                    //columnName = columnName.trim() + output[j];
                    var flag = true;
                    for (var i = 0; i < self.ruleIds.length; i++) {
                        if (self.ruleIds[i].toLowerCase().indexOf((columnName.trim().toLowerCase() + " " + output[j]).trim().toLowerCase()) != -1) {
                            columnName = columnName.trim() + " " + output[j];
                            flag = false;
                            // div = "<span class='highlight-ruleIds'>" + columnName + "</span>";
                        }
                    }
                    for (var i = 0; i < self.aggFunc.length; i++) {
                        if (self.aggFunc[i].indexOf((columnName.trim() + " " + output[j]).trim()) != -1) {
                            columnName = columnName.trim() + " " + output[j];
                            if ($.inArray(columnName, self.aggFunc) != -1) {
                                parentDiv += "<span class='highlight-aggFunc'>" + columnName + "</span>";
                                columnName = "";
                                flag = false;
                                break;
                            }
                            // div = "<span class='highlight-ruleIds'>" + columnName + "</span>";
                        }
                    }
                    if (flag && j != output.length - 1) {
                        parentDiv += "" + output[j] + "";
                    }
                    if (j == output.length - 1) {
                        //return "<span class='highlight-ruleIds'>" + columnName + "</span>";
                        if (columnName.trim() != "")
                            parentDiv += "<span class='highlight-ruleIds'>" + columnName + "</span>";
                        else
                            parentDiv += "<span class='highlight-ruleIds'>" + output[j] + "</span>";

                    }
                    // return "<span class='highlight-ruleIds'>" + columnName + "</span>"; ;
                }

                //});
            }

            return parentDiv;
        },

        //Dummy Grammar Info For Rule Editor
        _ruleEditorGrammarInfo: function () {
            var grammarInfo = {
                Columns: [{ ActionSideApplicability: 1, ActionSideApplicabilityJson: "RHS", ColumnEnumValues: {}, ColumnEnumValuesJson: [], ColumnName: "As of Date",
                    ColumnType: 0, ColumnTypeJson: "Both", DataType: 3, DataTypeJson: "DateTime", ExpressionSideApplicability: 0, ExpressionSideApplicabilityJson: "BOTH",
                    IsAggregationColumn: false, IsRhsColumn: true, IsRhsEnum: false, IsRhsUserInput: true, actualColumnName: "as_of_date", columnPrefix: ""
                },
                        { ActionSideApplicability: 1, ActionSideApplicabilityJson: "RHS", ColumnEnumValues: Object, ColumnEnumValuesJson: Array[0], ColumnName: "Knowledge Date",
                            ColumnType: 0, ColumnTypeJson: "Both", DataType: 3, DataTypeJson: "DateTime", ExpressionSideApplicability: 0, ExpressionSideApplicabilityJson: "BOTH",
                            IsAggregationColumn: false, IsRhsColumn: true, IsRhsEnum: false, IsRhsUserInput: true, actualColumnName: "knowledge_date", columnPrefix: ""
                        }
                ],
                DefaultGrouping: null, ElseCount: -1, IfClauseAvailable: true, IsDataTypeAvailable: true,
                RADXRuleCustomOpInfo: {}, RADXRuleCustomOpInfoJson: [], ParametersInfo: [], dataType: 2, dataTypeJson: "Numeric"
            }
            return grammarInfo;
        },

        _initRulePlugin: function () {
            var self = this;
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: self.options.serviceUrl + "/InitRuleEngine",
                data: JSON.stringify({ "info": self.options.grammarInfo }),
                //data:JSON.stringify({"a":"a"}),
                async: false,
                success: function (response) {
                    self.options.clientGUID = response.d;
                    if (self.options.ruleText != "") {
                        self._createExistingRuleNew();
                        if (self.options.grammarInfo.RuleTypeJson != "ValidationRule" && self.options.grammarInfo.RuleTypeJson != "AggregationRule") {
                            self._appendEmptyBlockBeforeClosingBrac();
                        }
                    }
                    if (self.options.existingRules == "" && $(event.target).attr('id') != "ruleTxt") {
                        self._initiateRuleEditor($(".ruleEditorFirstBlock"));
                    }
                    else {
                        //self._initiateRuleEditor($(".ruleEditorFirstBlock").find(".ruleEditorEditingBlock"));
                    }
                },
                error: function (response) {
                    console.log("err>", response);
                }
            });
        }


    });
});