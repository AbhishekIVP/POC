(function ($) {

    $.ruleTextEngine = function (element, options) {
        var plugin = this;
        var keywords = ['If', 'Then', 'END', 'else'],
                operators = ['*', '+', '-', '/', 'Min', 'Max', 'AddDays', 'AddMonths', 'AddYears', 'ConvertDateToISO', 'TrimEnd', 'TrimStart', 'Concat', 'PrependString', 'Replace', 'RegexExtract', 'abs', 'round', 'reciprocal', 'AppendString', 'power', 'ToLower', 'ToUpper', 'Substring'],
                comparisonop = [' &gt;= ', '!=', '&gt;', '&lt;=', '&lt;', '==', '<=', '<', '>', '>=', 'Equals', 'NotEquals', 'EndsWith', 'Contains', 'StartsWith', 'eq', 'lteq', 'gteq', 'nteq', 'RegexMatch', 'In', 'IsEmpty'],
                aggFunc = ["AVERAGE On", "CONCAT On", "DifferenceOn", "MAX On", "MIN On", "PRODUCT On", "SUM On", "WEIGHTED-AVERAGE On", "AggregationKeyOn"],
                ruleIds = Array();

        plugin.clientGuid;

        var defaults = {
            //serviceUrl: "http://localhost:63520/Resources/Services/RADXRuleEditorService.svc",
            hasFormattingSection: true,
            ruleText: '',
            ExternalFunction: function (state) { },
        };



        plugin.settings = {}

        var $element = $(element),
                element = element;

        var generateUI = function () {
            $element.append('' +
            		'<div id = "engine">' +
                    '	<div id = "resizable1" class = "resizable resizable1">' +
                    '		<div id="minimizeHandle-lft">Rule Editor</div>' +
                    '		<div id ="resizable1-inner" >' +
                    '			<div id="minimize-lft"></div> <div id="closebtn-lft" class = "fa fa-close"></div>' +
                    '			<textarea placeholder="Enter rule here.." id="ruleTxt"></textarea>' +
                    '			<div id = "autcompleteContainer" >' +
                    '				<div id="autocompleteLst" style="overflow-x:hidden;" ></div>' +
                    '			</div>' +
                    '		</div>' +
                    '	</div>' +
                    '	<div id = "resizable2" class = "resizable resizable2">' +
                    '		<div id="minimizeHandle-rgt">Formatted Rule</div>' +
                    '		<div id ="resizable2-inner" >' +
                    '			<div id="minimize-rgt"></div>' +
                    '			<div id="prettyCode" ><pre></pre></div>' +
                    '		</div>' +
                    '	</div>' +
            //'	<div>' +

            //'	</div>' +
            //'	<input type="button" id="backspaceBtn"  value="<--" />' +
                    '</div>' +
                    '');

            if (plugin.settings.hasFormattingSection === false) {
                $element.find('#resizable2').remove();
            }

        };
        plugin.init = function () {
            plugin.settings = $.extend({}, defaults, options);
            plugin.settings.textNow = "";
            generateUI();
            $(plugin.settings.grammarInfo.Columns).each(function (i, e) {
                ruleIds.push(e.ColumnName);
            });
            //var result = JSON.parse(plugin.initRuleEngine());
            plugin.clientGuid = plugin.settings.clientGuid;
            var rule = '';
            if (plugin.settings.ruleText.length > 0) {
                rule = plugin.settings.ruleText;

            }
            //var result1 = plugin.getNextTokens(plugin.clientGuid, rule);
            //fillTokens(result1.d);
            $("#ruleTxt").focus();
            $("#ruleTxt").val(rule);
            $("#ruleTxt").trigger('x');
        }
        plugin.FillColumns = function () {
            ruleIds = [];
            plugin.settings = $.extend({}, defaults, options);
            $(plugin.settings.grammarInfo.Columns).each(function (i, e) {
                ruleIds.push(e.ColumnName);
            });
            $(plugin.settings.grammarInfo.RADXRuleCustomOpInfo).each(function (i, e) {
                if (e.Key != null) {
                    ruleIds.push(e.Key);
                }
            });
            return ruleIds;
        }

        function fillTokens(tokens) {
            $('#autocompleteLst').empty();
            // tokens.sort();
            $(tokens).each(function (i, e) {
                e = SyntaxHighlightToken(e);
                $('#autocompleteLst').append('<div class="autoToken" style="cursor:pointer">' + e + '</div>');
                //$('#autocompleteLst').append('<option class="autoToken">' + e + '</option>');
            });
            //$("select").attr("size", function() { return this.options.length; });
        }

        //for highlighting tokens in dropdown
        function SyntaxHighlightToken(m) {
            if (keywords.indexOf(m) > -1) {
                return "<span class='highlight-keywords' title='" + m + "'>" + m + "</span>";
            }
            else if (operators.indexOf(m) > -1) {
                return "<span class='highlight-operators'>" + m + "</span>";
            } else if (comparisonop.indexOf(m) > -1) {
                return "<span class='highlight-comparisonop'>" + m + "</span>";
            }
            else if (aggFunc.indexOf(m) > -1) {
                return "<span class='highlight-aggFunc'>" + m + "</span>";
            }
            else if (ruleIds.indexOf(m) > -1) {
                return "<span class='highlight-ruleIds'>" + m + "</span>";
            }
            else
                return m;
        }

        //for highlighting tokens in rule output
        function syntaxHighlight(input) {
            var tabCount = 0;

            var output = input.replace(/(\w+)\b/g, function (m) {
                if (keywords.indexOf(m) > -1) {
                    return "<span class='highlight-keywords'>" + m + "</span>";
                }
                else if (operators.indexOf(m) > -1) {
                    return "<span class='highlight-operators'>" + m + "</span>";
                } else if (comparisonop.indexOf(m) > -1) {
                    return "<span class='highlight-comparisonop'>" + m + "</span>";
                }
                else if (aggFunc.indexOf(m) > -1) {
                    return "<span class='highlight-aggFunc'>" + m + "</span>";
                }
                else if (ruleIds.indexOf(m) > -1) {
                    return "<span class='highlight-ruleIds'>" + m + "</span>";
                }
                else
                    return m;
            });
            output = output.replace(/"(.*?)"/g, function (m) {
                return "<span class='highlight-string'>" + m + "</span>";
            });
            var finalStr = autoIndent(output);

            if (finalStr) {
                return finalStr.replace("undefined", "");
            }
        }



        $element.on("click", "#autocompleteLst .autoToken", function (e) {
            selectionMode = false;
            var position = $('#ruleTxt')[0].selectionStart;
            var ruleLength = $('#ruleTxt').val().length;
            if (ruleLength - position > 1) {
                var a = $('#ruleTxt').val();
                var b = $(this).text();
                var final = '';
                if (plugin.remainingtext != null && plugin.remainingtext.length > 0 && b.length >= plugin.remainingtext.length) {
                    final = [a.slice(0, position).trim().slice(0, a.slice(0, position).trim().length - plugin.remainingtext.length), b + ' ', a.slice(position).trim()].join('');
                }
                else {
                    final = [a.slice(0, position).trim(), ' ' + b + '  ', a.slice(position).trim()].join('');
                }
                $('#ruleTxt').val(final);
                var nextTokens = plugin.getNextTokens(plugin.clientGuid, a.slice(0, position).trim() + ' ' + b + ' ');
                if (nextTokens.d.length === 1 && nextTokens.d[0] && nextTokens.d[0].indexOf("Invalid Token") === -1) {
                    lastCaseInsesitiveToken = nextTokens.d[0];
                }

                fillTokens(nextTokens.d);
                showAutoComplete(position + b.length + 2);
                if ($('#ruleTxt')[0].setSelectionRange) {
                    $('#ruleTxt')[0].focus();
                    $('#ruleTxt')[0].setSelectionRange(position + b.length + 1, position + b.length + 1);
                }
            }
            else {
                if ($(this).text() == 'Enter Number' || $(this).text() == 'Enter String' || $(this).text() == 'Invalid Rule Text') {

                }
                else {
                    var text = $(this).text();
                    if (plugin.remainingtext != null && plugin.remainingtext.length > 0) {
                        if (text.trim().toLowerCase().startsWith(plugin.remainingtext.trim().toLowerCase()))
                            $('#ruleTxt').val($('#ruleTxt').val().slice(0, $('#ruleTxt').val().length - plugin.remainingtext.length));
                    }
                    $('#ruleTxt').val($('#ruleTxt').val() + text + ' '); //append clicked token
                    $('#ruleTxt').trigger('x').focus();
                }
            }
        });


        var lastCaseInsesitiveToken = "";
        $element.on("keydown", function () {
            //console.log(event.which);
            if (event.which == KEY_TAB) {
                event.preventDefault();
            }
            //            if (selectionMode == true) {
            //                if ((event.which == KEY_DOWN || event.which == KEY_UP || event.which == KEY_ENTER || event.which == KEY_TAB)) {
            //                    event.preventDefault();
            //                }
            //            }
            if ((event.which == KEY_DOWN || event.which == KEY_UP || event.which == KEY_TAB || event.which == KEY_ENTER)) {
                event.preventDefault();
            }
        });

        var selectionMode = false;
        var KEY_DOWN = 40, KEY_UP = 38, KEY_TAB = 9, KEY_ENTER = 13, KEY_SPACE = 32, KEY_ESC = 27;
        $element.on("x keyup paste", "#ruleTxt", function (event) {

            if (selectionMode == true) {
                switch (event.which) {
                    case KEY_DOWN:
                        event.preventDefault();
                        if ($(".autoToken.selected").index() < $('#autocompleteLst .autoToken').length - 1) {
                            $("#autocompleteLst .autoToken.selected").eq(0).removeClass('selected').next().addClass('selected');
                            if ($('#autocompleteLst .selected').offset().top - $('#autocompleteLst').offset().top > $('#autocompleteLst').height()) {
                                $('#autocompleteLst')[0].scrollTop = $('#autocompleteLst')[0].scrollTop + $('#autocompleteLst').height() / 2;
                            }
                        }
                        return;
                    case KEY_UP:
                        event.preventDefault();
                        if ($(".autoToken.selected").index() > 0) {
                            $("#autocompleteLst .autoToken.selected").eq(0).removeClass('selected').prev().addClass('selected');
                            if ($('#autocompleteLst .selected').offset().top - $('#autocompleteLst').offset().top < 0) {
                                $('#autocompleteLst')[0].scrollTop = $('#autocompleteLst')[0].scrollTop - $('#autocompleteLst').height() / 2;
                            }
                        }
                        return;
                }
                if (event.which === KEY_ENTER) {
                    event.preventDefault();
                }
            }

            selectionMode = false;
            if (event.which === KEY_DOWN) {
                if ($('#autcompleteContainer').is(':visible') && $("#autocompleteLst .autoToken").find('.selected').length === 0 && selectionMode === false) {
                    $("#autocompleteLst .autoToken").eq(0).addClass('selected').focus();
                    selectionMode = true;
                    return;
                }
            }

            if (event.which === KEY_ESC) {
                var currentBlockText = $element.find("#ruleTxt").val()
                if (currentBlockText.toLowerCase().trim().endsWith("end")) {
                    plugin.settings.closeRuleEditor.call(this, currentBlockText, plugin.settings.blockid, plugin.settings.jsplumbId, currentBlockText);
                }
                else {
                    plugin.settings.closeRuleEditor.call(this, plugin.settings.prependText + currentBlockText, plugin.settings.blockid, plugin.settings.jsplumbId, currentBlockText);
                }
                plugin.settings.prependText = "";
                $element.hide();
                return;
            }

            var text;

            if (event.which === KEY_ENTER) {
                e.preventDefault();
            }
            if (event.which === KEY_TAB) {
                var end = $('#ruleTxt').getSelection().end;
                text = $('#ruleTxt').val().substring(0, end);
                var append = $('#ruleTxt').val().substring(end);
                if (text.trim().length == 0) {
                    if ($('#autcompleteContainer').find('.selected').length > 0) {
                        if ($($('#autcompleteContainer').find('.selected')[0]).find('span').length > 0)
                            correctedText = $($($('#autcompleteContainer').find('.selected')[0]).find('span')[0]).text();
                        else
                            correctedText = $($('#autcompleteContainer').find('.selected')[0]).text();
                    }
                    if (correctedText == "If") {
                        plugin.settings.appendControls.call(this, plugin.settings.blockid, $('#ruleTxt').val(), plugin.settings.jsplumbId, true);
                        return;
                    }
                }

                text = plugin.autoCorrect($('#ruleTxt').val().substring(0, end))
                $('#ruleTxt').val(text + " " + append);

                var nextTokens = plugin.getNextTokens(plugin.clientGuid, text);
                if (nextTokens.d.length === 1 && nextTokens.d[0] && nextTokens.d[0].indexOf("Invalid Token") === -1) {
                    lastCaseInsesitiveToken = nextTokens.d[0];
                }

                fillTokens(nextTokens.d);
                showAutoComplete(end);
                if (event.which === KEY_SPACE) {
                    if (getLastWord($('#ruleTxt').val()).toLowerCase().trim() === lastCaseInsesitiveToken.toLowerCase()) {
                        $('#ruleTxt').val(replaceLastWord($('#ruleTxt').val(), lastCaseInsesitiveToken));
                        lastCaseInsesitiveToken = "";
                        showAutoComplete(end);
                        selectionMode = false;
                    }
                }
                //prettyPrint();
                //showAutoComplete(end);
            }
            if (event.ctrlKey) {
                if (event.keyCode == 65 || event.keyCode == 97) { // 'A' or 'a'
                    console.log("controll pressed");
                    event.preventDefault();
                    return false;
                }
            }
            else {
                var keyPressed = event.which;
                if (event.which != 8 && !(event.which > 95 && event.which < 106) && !(event.which > 47 && event.which < 58) && !(event.keyCode == 17))
                    doTheThing();
            }
            var keyPressed = event.which;
            if (event.which == 8 || (event.which > 95 && event.which < 106) || (event.which > 47 && event.which < 58) && !(event.keyCode == 17)) {
                plugin.settings.textNow = $('#ruleTxt').val();
                setTimeout(function (keyPressed) {
                    var newText = $('#ruleTxt').val();
                    if (newText == plugin.settings.textNow) {
                        doTheThing();
                    }
                }, 1500);
                return;
            }
        });

        function doTheThing() {
            if ($('#ruleTxt').val() != null) {
                if ($('#ruleTxt').val().substring(0, $('#ruleTxt').getSelection().end).trim().toLowerCase() == 'if' || $('#ruleTxt').val().substring(0, $('#ruleTxt').getSelection().end).trim().toLowerCase() == 'then') {
                    if ($('#ruleTxt').val().substring(0, $('#ruleTxt').getSelection().end).trim().toLowerCase() == 'if') {
                        plugin.settings.appendControls.call(this, plugin.settings.blockid, $('#ruleTxt').val(), plugin.settings.jsplumbId, true);
                    }
                    else {
                        plugin.settings.appendControls.call(this, plugin.settings.blockid, $('#ruleTxt').val(), plugin.settings.jsplumbId, false);
                    }
                }
                else {
                    var end = $('#ruleTxt').getSelection().end;
                    var nextTokens = plugin.getNextTokens(plugin.clientGuid, $('#ruleTxt').val().substring(0, end));
                    if (nextTokens.d.length === 1 && nextTokens.d[0] && nextTokens.d[0].indexOf("Invalid Token") === -1) {
                        lastCaseInsesitiveToken = nextTokens.d[0];
                    }

                    fillTokens(nextTokens.d);
                    showAutoComplete(end);
                    //                        prettyPrint();
                    //                        showAutoComplete(end);
                }
            }
        }

        var getLastWord = function (input) {
            var lastWord = input.trim().split(' ').slice(-1)[0];
            return lastWord;
        };

        var replaceLastWord = function (input, word) {
            input = input.trim();
            input = input.substring(0, input.lastIndexOf(' '));
            input = input + ' ' + word + ' ';
            return input;
        };

        $element.on("click", "#minimize-rgt", function () {
            //$('#resizable1').width("100%");
            $("#minimize-rgt , #minimize-lft").hide();
            $('#resizable2-inner').slideUp("fast"); //hide();
            $('#minimizeHandle-rgt').show();
            // $(this).hide();
        });

        $element.on("click", "#minimizeHandle-rgt", function () {
            //$('#resizable2').show();
            //$('#resizable2').animate({width: "48%"}, 200);
            //$('#resizable1').width("48%");
            $(this).hide();
            $('#resizable2-inner').slideDown("fast");
            $("#minimize-rgt , #minimize-lft").show();
        });

        $element.on("click", "#minimize-lft", function () {
            hideAutoComplete();
            //$('#resizable2').width("100%");
            $("#minimize-rgt , #minimize-lft").hide();
            $('#resizable1-inner').slideUp("fast");
            //$(this).closest('.resizable').hide();
            $('#minimizeHandle-lft').show();
            $(this).hide();
        });

        $element.on("click", "#closebtn-lft", function () {
            //$element.hide();
            var currentBlockText = $element.find("#ruleTxt").val()
            //currentBlockText = currentBlockText.replace("&&", ' ] && [');
            //currentBlockText = currentBlockText.replace("||", ' ] || [');
            if (currentBlockText.toLowerCase().trim().endsWith("end")) {
                plugin.settings.closeRuleEditor.call(this, currentBlockText, plugin.settings.blockid, plugin.settings.jsplumbId, currentBlockText);
            }
            else {
                plugin.settings.closeRuleEditor.call(this, plugin.settings.prependText + currentBlockText, plugin.settings.blockid, plugin.settings.jsplumbId, currentBlockText);
            }
            plugin.settings.prependText = "";
            $element.hide();
            //$element.removeClass("");
        });

        plugin.setPrependText = function (prependText) {
            plugin.settings.prependText = prependText;
        }

        $element.on("click", "#minimizeHandle-lft", function () {
            //$('#resizable1').show();
            //$('#resizable1').animate({width: "48%"}, 200);
            //$('#resizable2').width("48%");
            $(this).hide();
            $('#resizable1-inner').slideDown("fast");
            $("#minimize-rgt , #minimize-lft").show();
        });

        function moveCaretToEnd(el) {
            el.focus();
            if (typeof el.selectionStart === "number") {
                el.selectionStart = el.selectionEnd = el.value.length;
            } else if (typeof el.createTextRange !== "undefined") {
                var range = el.createTextRange();
                range.collapse(false);
                range.select();
            }
        };

        function moveCaretToNextSpace(el) {
            //            el.focus();
            //            if (typeof el.selectionStart == "number") {
            //                el.selectionStart = el.selectionEnd = $(el).val().indexOf(' ', el.selectionStart) + 1;
            //            } else if (typeof el.createTextRange != "undefined") {
            //                var range = el.createTextRange();
            //                range.collapse(false);
            //                range.select();
            //            }
        };

        $element.on("click", '#resizeHandle-ruleTxt', function () {
            $('#ruleTxt').toggle({ slide: 'left', duration: 100 });
            $(this).toggleClass('collapse');
            $('#prettyCode').toggleClass('fullSize');
            hideAutoComplete();

        });

        $element.on("click", '#resizeHandle-prettyCode', function () {
            $('#prettyCode').toggle({ slide: 'right', duration: 100 });
            $(this).toggleClass('collapse');
            $('#ruleTxt').toggleClass('fullSize');
            hideAutoComplete();

        });
        //$(document).on("click",function(){ hideAutoComplete();});
        $element.on("click", '#ruleTxt, #prettyCode', function () {
            hideAutoComplete();
        });

        var hideAutoComplete = function () {
            selectionMode = false;
            $('#autcompleteContainer').hide();
        };

        var showAutoComplete = function (end) {
            if ($('#autocompleteLst .autoToken').length > 0) {
                var pos = $('#ruleTxt').getCaretPosition(end);
                // if (pos.left > 10) {
                $('#autcompleteContainer').css({
                    left: $('#ruleTxt')[0].offsetLeft + Math.max(pos.left, 26),
                    top: $('#ruleTxt')[0].offsetTop + pos.top + 12 //+ Math.max(pos.top,47)
                }).show();
                //}
            }
            else {
                $('#autcompleteContainer').hide();
            }
        };

        var prettyPrint = function () {
            $('#prettyCode pre').html(syntaxHighlight(autoIndent($('#ruleTxt').val())));
        };

        function autoIndent(output) {
            var finalStr = "";
            var words = output.split(' ');
            var tabCount = 0;
            for (var i = 0; i < words.length; i++) {
                if (words[i] == " ") {
                    continue;
                }
                //console.log(words[i]);
                if (words[i] == "if") {
                    // console.log("true");
                    words[i] = words[i] + "\n\t";
                }
                if (words[i] == "Then") {
                    words[i] = "\n" + words[i] + "\n\t";
                }
                if (words[i] == "&&" || words[i] == "||" || words[i] == ';') {
                    if (words[i + 1] != "END") {
                        words[i] = words[i] + "\n\t";
                    }
                    else {
                        words[i] = words[i] + "\n";
                    }
                }
                finalStr += words[i] + " ";
            }
            return finalStr;
        };

        plugin.getNextTokens = function (clientGuid, input) {
            var currentPrependText = plugin.settings.prependText;
            if (input != "") {
            }
            input = currentPrependText + input;
            var intellisense = JSON.parse($.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: plugin.settings.serviceUrl + "/GetAutoComplete",
                data: JSON.stringify({ cacheKey: clientGuid, ruleText: input }),
                dataType: "json",
                async: false,
                success: function (response) {

                },
                error: function (response) {
                    console.log("err>", response);
                }
            }).responseText);
            if (plugin.settings.grammarInfo.RuleTypeJson == "AggregationRule" && plugin.settings.prependText.trim() == "Then") {
                intellisense.d.splice(intellisense.d.length - 3, 0, "if");
            }

            plugin.settings.lastToken = intellisense.d[intellisense.d.length - 1];
            var orignialText = "";
            if (plugin.settings.lastToken.indexOf("~^~") > -1) {
                orignialText = plugin.settings.lastToken.split("~^~")[0];
                plugin.settings.lastToken = plugin.settings.lastToken.split("~^~")[1];
            }
            intellisense.d.splice(intellisense.d.length - 1, 1);
            if ($.inArray("Invalid Rule Text", intellisense.d) == -1) {
                intellisense.d = jQuery.grep(intellisense.d, function (a) {
                    return a !== 'Else' && a !== 'ElseIf';
                });
                if ($('#ruleTxt').val().trim().length > 0) {
                    intellisense.d = jQuery.grep(intellisense.d, function (a) {
                        return a !== 'Else' && a !== 'ElseIf';
                    });
                }
                plugin.settings.Intellisense = intellisense.d;
            }
            if ($.inArray("]", intellisense.d) != -1) {
                if ($("#" + plugin.settings.blockid).closest(".ruleEditorBlocks").attr("isifblock")) {
                    var index = $.inArray("]", intellisense.d)
                    intellisense.d.splice(index, 1, "AND", "OR");
                }
            }
            if ($.inArray("}", intellisense.d) != -1) {
                var index = $.inArray("}", intellisense.d);
                intellisense.d.splice(index, 1);
            }
            if ($.inArray("Invalid Rule Text", intellisense.d) == -1) {
                plugin.remainingtext = intellisense.d[intellisense.d.length - 2];
            }
            plugin.ruleText = intellisense.d[intellisense.d.length - 1];

            if (plugin.remainingtext == null) {
                var startPos = $('#ruleTxt')[0].selectionStart;
                var endPos = $('#ruleTxt')[0].selectionEnd;
                if (!$('#ruleTxt')[0].value.substring(0, startPos).endsWith(' ')) {
                    if (!plugin.isNumeric(plugin.settings.lastToken) && !$('#ruleTxt')[0].value.substring(0, startPos).trim().endsWith("-"))
                        $('#ruleTxt')[0].value = $('#ruleTxt')[0].value.substring(0, startPos) + ' ' + $('#ruleTxt')[0].value.substring(endPos, $('#ruleTxt')[0].value.length);
                    else
                        $('#ruleTxt')[0].value = $('#ruleTxt')[0].value.substring(0, startPos) + $('#ruleTxt')[0].value.substring(endPos, $('#ruleTxt')[0].value.length);

                    if ($('#ruleTxt')[0].setSelectionRange) {
                        $('#ruleTxt')[0].focus();
                        $('#ruleTxt')[0].setSelectionRange(startPos + 1, startPos + 1);
                    }
                }
            }
            if (orignialText.trim().length > 0)
                $('#ruleTxt')[0].value = $('#ruleTxt')[0].value.replace(orignialText, plugin.settings.lastToken);
            intellisense.d = intellisense.d.slice(0, intellisense.d.length - 2);
            return intellisense;
        };
        plugin.remainingtext;
        plugin.ruleText;

        plugin.isNumeric = function (obj) {
            return !jQuery.isArray(obj) && (obj - parseFloat(obj) + 1) >= 0;
        }

        plugin.autoCorrect = function (ruleText) {
            if (plugin.settings.lastToken != null && plugin.settings.lastToken != "") {
                var textToCorrect = ruleText.substr(ruleText.lastIndexOf(plugin.settings.lastToken) + plugin.settings.lastToken.length);
                //var autoCorrect = new nlpResources();
                var correctedText = "";
                if (textToCorrect.indexOf('"') > -1) {
                    return ruleText;
                }
                if ($('#autcompleteContainer').find('.selected').length > 0) {
                    if ($($('#autcompleteContainer').find('.selected')[0]).find('span').length > 0)
                        correctedText = $($($('#autcompleteContainer').find('.selected')[0]).find('span')[0]).text();
                    else
                        correctedText = $($('#autcompleteContainer').find('.selected')[0]).text();
                }
                else {
                    correctedText = plugin.settings.Intellisense[0];
                }
                return ruleText.substr(0, ruleText.lastIndexOf(plugin.settings.lastToken) + plugin.settings.lastToken.length) + " " + correctedText;
            }
            else {
                return ruleText;
            }
        }

        plugin.autoCorrectText = function (s1, s2, dj) {

            function distance(s1, s2) {
                if (typeof (s1) != "string" || typeof (s2) != "string") return 0;
                if (s1.length == 0 || s2.length == 0)
                    return 0;
                s1 = s1.toLowerCase(), s2 = s2.toLowerCase();
                var matchWindow = (Math.floor(Math.max(s1.length, s2.length) / 2.0)) - 1;
                var matches1 = new Array(s1.length);
                var matches2 = new Array(s2.length);
                var m = 0; // number of matches
                var t = 0; // number of transpositions

                //debug helpers
                //console.log("s1: " + s1 + "; s2: " + s2);
                //console.log(" - matchWindow: " + matchWindow);

                // find matches
                for (var i = 0; i < s1.length; i++) {
                    var matched = false;

                    // check for an exact match
                    if (s1[i] == s2[i]) {
                        matches1[i] = matches2[i] = matched = true;
                        m++
                    }

                        // check the "match window"
                    else {
                        // this for loop is a little brutal
                        for (k = (i <= matchWindow) ? 0 : i - matchWindow;
        		        (k <= i + matchWindow) && k < s2.length && !matched;
			        k++) {
                            if (s1[i] == s2[k]) {
                                if (!matches1[i] && !matches2[k]) {
                                    m++;
                                }

                                matches1[i] = matches2[k] = matched = true;
                            }
                        }
                    }
                }

                if (m == 0)
                    return 0.0;

                // count transpositions
                var k = 0;

                for (var i = 0; i < s1.length; i++) {
                    if (matches1[k]) {
                        while (!matches2[k] && k < matches2.length)
                            k++;
                        if (s1[i] != s2[k] && k < matches2.length) {
                            t++;
                        }

                        k++;
                    }
                }

                //debug helpers:
                //console.log(" - matches: " + m);
                //console.log(" - transpositions: " + t);
                t = t / 2.0;
                return (m / s1.length + m / s2.length + (m - t) / m) / 3;
            }

            var jaro;
            (typeof (dj) == 'undefined') ? jaro = distance(s1, s2) : jaro = dj;
            var p = 0.1; //
            var l = 0 // length of the matching prefix
            while (s1[l] == s2[l] && l < 4)
                l++;

            return jaro + l * p * (1 - jaro);
        }

        plugin.getGeneratedCode = function (ruleText) {

            var result = JSON.parse($.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: plugin.settings.serviceUrl + "/GetGeneratedCode",
                data: JSON.stringify({ "cacheKey": plugin.clientGuid, "ruleText": ruleText != null ? ruleText : $('#ruleTxt').val() }),
                dataType: "json",
                async: false,
                success: function (response) {

                },
                error: function (response) {
                    console.log("err>", response);
                }
            }).responseText);
            return result.d;
        };

        plugin.ResetSettings = function (prependText, blockid, currentBlockText) {
            $("#ruleTxt").val(currentBlockText);
            plugin.settings.ruleText = "";
            plugin.settings.blockid = blockid;
            plugin.settings.prependText = prependText;
            return plugin.clientGuid;
        }

        plugin.getClientGuid = function () {
            return plugin.clientGuid;
        }

        plugin.PrettifyText = function () {
            return syntaxHighlight(autoIndent($('#ruleTxt').val()));
        };

        plugin.Destroy = function () {
            $element.unbind();
            if ($element.find('#engine') != null)
                $element.find('#engine').unbind();
            if ($element.find('resizable1') != null)
                $element.find('resizable1').unbind();
            if ($element.find('minimizeHandle-lft') != null)
                $element.find('minimizeHandle-lft').unbind();
            if ($element.find('resizable1-inner') != null)
                $element.find('resizable1-inner').unbind();
            if ($element.find('minimize-lft') != null)
                $element.find('minimize-lft').unbind();
            if ($element.find('ruleTxt') != null)
                $element.find('ruleTxt').unbind();
            if ($element.find('autcompleteContainer') != null)
                $element.find('autcompleteContainer').unbind();
            if ($element.find('autocompleteLst') != null)
                $element.find('autocompleteLst').unbind();
            if ($element.find('resizable2') != null)
                $element.find('resizable2').unbind();
            if ($element.find('minimizeHandle-rgt') != null)
                $element.find('minimizeHandle-rgt').unbind();
            if ($element.find('resizable2-inner') != null)
                $element.find('resizable2-inner').unbind();
            if ($element.find('minimize-rgt') != null)
                $element.find('minimize-rgt').unbind();
            if ($element.find('prettyCode') != null)
                $element.find('prettyCode').unbind();
            $element.html('');
            $element.removeData('ruleTextEngine');
        };

        plugin.initRuleEngine = function () {
            return $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: plugin.settings.serviceUrl + "/InitRuleEngine",
                data: JSON.stringify({ "info": plugin.settings.grammarInfo }),
                //data:JSON.stringify({"a":"a"}),
                async: false,
                success: function (response) {
                },
                error: function (response) {
                    console.log("err>", response);
                }
            }).responseText;
        };

        plugin.init();
    }

    $.fn.ruleTextEngine = function (options) {
        return this.each(function () {
            if (undefined == $(this).data('ruleTextEngine')) {
                var plugin = new $.ruleTextEngine(this, options);
                $(this).data('ruleTextEngine', plugin);
            }
        });
    }

})(jQuery);