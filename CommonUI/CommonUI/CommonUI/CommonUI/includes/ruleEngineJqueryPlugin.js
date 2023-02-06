
(function ($) {

    $.ruleEngine = function (element, options) {
        var plugin = this;
        var keywords = ['If', 'Then', 'END', 'else'],
                operators = ['*', '+', '-', '/', 'Min', 'Max', 'AddDays', 'AddMonths', 'AddYears', 'ConvertDateToISO', 'TrimEnd', 'TrimStart', 'Concat', 'PrependString', 'Replace', 'RegexExtract', 'abs', 'round', 'reciprocal', 'AppendString', 'power', 'ToLower', 'ToUpper', 'Substring'],
                comparisonop = [' &gt;= ', '!=', '&gt;', '&lt;=', '&lt;', '==', '<=', '<', '>', '>=', 'Equals', 'NotEquals', 'EndsWith', 'Contains', 'StartsWith', 'eq', 'lteq', 'gteq', 'nteq', 'RegexMatch', 'In', 'IsEmpty'],
                aggFunc = ["AVERAGE On", "CONCAT On", "DifferenceOn", "MAX On", "MIN On", "PRODUCT On", "SUM On", "WEIGHTED-AVERAGE On", "AggregationKeyOn"],
                ruleIds = Array();

        plugin.clientGuid;

        var defaults = {
            serviceUrl: "http://localhost:58435/Services/RADXRuleEditorService.svc",
            hasFormattingSection: true,
            ruleText: '',
            ExternalFunction: function (state) { }
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
                    '			<div id="minimize-lft"></div>' +
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
            generateUI();
            plugin.settings = $.extend({}, defaults, options);
            $(plugin.settings.grammarInfo.Columns).each(function (i, e) {
                ruleIds.push(e.ColumnName);
            });
            var result = JSON.parse(plugin.initRuleEngine());
            plugin.clientGuid = result.d;
            var rule = '';
            if (plugin.settings.ruleText.length > 0) {
                rule = plugin.settings.ruleText;

            }
            var result1 = plugin.getNextTokens(plugin.clientGuid, rule);
            fillTokens(result1.d);
            $("#ruleTxt").focus();
            $("#ruleTxt").val(rule);
            $("#ruleTxt").trigger('x');
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
            console.log("click" + new Date().getTime());
            var position = $('#ruleTxt')[0].selectionStart;
            var ruleLength = $('#ruleTxt').val().length;
            if (ruleLength - position > 1) {
                var a = $('#ruleTxt').val();
                var b = $(this).text();
                var final = '';
                if (plugin.remainingtext != null && plugin.remainingtext.length > 0 && b.length >= plugin.remainingtext.length) {
                    //  final = [a.slice(0, position).trim(), b.toLowerCase().replace(plugin.remainingtext, '') + ' ', a.slice(position).trim()].join('');
                    final = [a.slice(0, position).trim().slice(0, a.slice(0, position).trim().length - plugin.remainingtext.length), b + ' ', a.slice(position).trim()].join('');
                }
                else {
                    final = [a.slice(0, position).trim(), ' ' + b + ' ', a.slice(position).trim()].join('');
                }
                $('#ruleTxt').val(final);
                moveCaretToNextSpace($('#ruleTxt')[0]);
                $('#ruleTxt').trigger('x').focus();
            }
            //else if ($('#ruleTxt').val().substr($('#ruleTxt').val().length - 1) === " " || $('#ruleTxt').val().length == $('#ruleTxt')[0].selectionStart)//last letter is space
            // else if ($('#ruleTxt').val().substr($('#ruleTxt').val().length - 1) === " ")//last letter is space
            else {
                if ($(this).text() == 'Enter Number' || $(this).text() == 'Enter String' || $(this).text() == 'Invalid Rule Text') {

                }
                else {
                    var text = $(this).text();
                    if (plugin.remainingtext != null && plugin.remainingtext.length > 0) {
                        //text = text.substring(plugin.remainingtext.length, text.length);
                        $('#ruleTxt').val($('#ruleTxt').val().slice(0, $('#ruleTxt').val().length - plugin.remainingtext.length));
                    }
                    $('#ruleTxt').val($('#ruleTxt').val() + text + ' '); //append clicked token
                    moveCaretToEnd($('#ruleTxt')[0]);
                    $('#ruleTxt').trigger('x').focus();
                }
            }
            //            else {
            //                var someStr = $('#ruleTxt').val();

            //                if (someStr.lastIndexOf(" ") > -1)
            //                    someStr = someStr.substring(0, someStr.lastIndexOf(" ")) + " " + $(this).text() + " ";
            //                else
            //                    someStr = someStr + " " + $(this).text() + " ";
            //                $('#ruleTxt').val(someStr);
            //                moveCaretToEnd($('#ruleTxt')[0]);
            //                $('#ruleTxt').trigger('x').focus();
            //            }
        });


        var lastCaseInsesitiveToken = "";
        $element.on("keydown", function () {
            //console.log(event.which);
            if (event.which == KEY_TAB) {
                event.preventDefault();
            }
            if (selectionMode == true) {
                if ((event.which == KEY_DOWN || event.which == KEY_UP || event.which == KEY_ENTER || event.which == KEY_TAB)) {
                    event.preventDefault();
                }
            }
        });

        var selectionMode = false;
        var KEY_DOWN = 40, KEY_UP = 38, KEY_TAB = 9, KEY_ENTER = 13, KEY_SPACE = 32;
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
                        // case KEY_ENTER:
                        // event.preventDefault();


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

            if ($('#autcompleteContainer').is(':visible')) {
                if (event.which === KEY_TAB || event.which === KEY_ENTER) {
                    event.preventDefault();
                    if ($("#autocompleteLst .autoToken.selected").length > 0) {
                        $("#autocompleteLst .autoToken.selected").eq(0).trigger("click");
                    }
                    else {;
                        $("#autocompleteLst .autoToken").eq(0).trigger("click");
                    }
                    selectionMode = false;
                }
            }
			if(event.which != 8 && !(event.which > 95 && event.which < 106) && !(event.which > 47 && event.which < 58) &&!(event.keyCode == 17) && (event.which != 190 && event.which != 110))
                 doTheThing(event);

            var keyPressed = event.which;
            if(event.which == 8 || (event.which > 95 && event.which < 106) || (event.which > 47 && event.which < 58) &&!(event.keyCode ==17) || event.which != 190 || event.which != 110) {
				var evnt = event;
                plugin.settings.textNow = $('#ruleTxt').val();
                setTimeout(function(keyPressed) {   
                    var newText = $('#ruleTxt').val();
                    if(newText == plugin.settings.textNow) {
                        doTheThing(evnt);
                    }
                },1500);
                return;
            }
        });
		
		
		function doTheThing(event) {
             var nextTokens = plugin.getNextTokens(plugin.clientGuid, $('#ruleTxt').val().substring(0, $('#ruleTxt').getSelection().end));
            if (nextTokens.d.length === 1 && nextTokens.d[0] && nextTokens.d[0].indexOf("Invalid Token") === -1) {
                lastCaseInsesitiveToken = nextTokens.d[0];
            }

            fillTokens(nextTokens.d);
            showAutoComplete();
            if (event.which === KEY_SPACE) {
                if (getLastWord($('#ruleTxt').val()).toLowerCase().trim() === lastCaseInsesitiveToken.toLowerCase()) {
                    $('#ruleTxt').val(replaceLastWord($('#ruleTxt').val(), lastCaseInsesitiveToken));
                    lastCaseInsesitiveToken = "";
                    showAutoComplete();
                    selectionMode = false;
                }
            }
            prettyPrint();
            showAutoComplete();
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

        var showAutoComplete = function () {
            if ($('#autocompleteLst .autoToken').length > 0) {
                var pos = $('#ruleTxt').getCaretPosition();
                // if (pos.left > 10) {
                $('#autcompleteContainer').css({
                    left: $('#ruleTxt')[0].offsetLeft + Math.max(pos.left, 26),
                    top: $('#ruleTxt')[0].offsetTop + pos.top //+ Math.max(pos.top,47)
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
            plugin.lastToken = intellisense.d[intellisense.d.length - 1];
            var orignialText = "";
            if (plugin.lastToken.indexOf("~^~") > -1) {
                orignialText = plugin.lastToken.split("~^~")[0];
                plugin.lastToken = plugin.lastToken.split("~^~")[1];
            }
            plugin.remainingtext = intellisense.d[intellisense.d.length - 3];
            plugin.ruleText = intellisense.d[intellisense.d.length - 2];
            intellisense.d = intellisense.d.slice(0, intellisense.d.length - 3);
            if (intellisense.d.length == 0)
                plugin.settings.ExternalFunction(true);
            else
                plugin.settings.ExternalFunction(false);

            if (orignialText.trim().length > 0)
                $('#ruleTxt')[0].value = $('#ruleTxt')[0].value.replace(orignialText, plugin.lastToken);
            return intellisense;
        };
        plugin.remainingtext;
        plugin.ruleText;

        plugin.getGeneratedCode = function () {
            var result = JSON.parse($.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: plugin.settings.serviceUrl + "/GetGeneratedCode",
                data: JSON.stringify({ "cacheKey": plugin.clientGuid, "ruleText": $('#ruleTxt').val() }),
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
            $element.removeData('ruleEngine');
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

    $.fn.ruleEngine = function (options) {
        return this.each(function () {
            if (undefined == $(this).data('ruleEngine')) {
                var plugin = new $.ruleEngine(this, options);
                $(this).data('ruleEngine', plugin);
            }
        });
    }

})(jQuery);
