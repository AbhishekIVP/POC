var smselect = (function () {
    function smSelect() {
        this.counter = 0;
        this.multiCollection = new Object();
    }
    var smselect = new smSelect();
    var docHandler = function (e) {
        e.stopPropagation();
        if (!$(e.target).hasClass('smslimScrollDiv') && $(e.target).parents('.smslimScrollDiv').length === 0) {
            var selects = $('.smselect');
            $.each(selects, function (ii, ee) {
                $(ee).find('.smselectcon').hide();

                if (!$(ee).hasClass('smselectmulti')) {
                    var text = $(ee).find('.smselecttext');
                    var rundiv = $(ee).find('.smselectrun');

                    var tt = $(ee).attr('overridetext');
                    var hh = $(ee).attr('heading');
                    if (typeof tt === 'undefined' || tt !== '')
                        rundiv.find('a').text(tt);
                    else if ($(ee).find('.smselected').text().trim() !== "")
                        rundiv.find('a').text($(ee).find('.smselected').text().trim());
                    else
                        rundiv.find('a').text(hh);

                }
            });
        }
        else
            e.preventDefault();
    }

    function setTop(target) {
        var viewportHeight = $(window).height();

        var top = parseInt(target.offset().top);
        var height = parseInt(target.height());
        var diff = top + height - viewportHeight;

        //if (diff > 0) {
        //    var cssTop = target.css('top');
        //    cssTop = typeof cssTop !== 'undefined' && cssTop != null && cssTop.indexOf('p') > -1 ? parseInt(cssTop.split('p')[0]) : 0;
        //    if (cssTop !== 0)
        //        target.css('top', (cssTop - diff - 40));
        //}        
        if (diff > 0) {
            var cssTop = parseInt(target.css('top'));
            if (typeof target.attr('oftop') === 'undefined')
                target.attr('oftop', cssTop)
            else
                cssTop = target.attr('oftop');
            if (cssTop !== 0)
                target.css('top', (cssTop - height));
        }
    }

    smSelect.prototype.create = function create($object) {
        String.prototype.replaceAll = function (find, replace) {
            return this.replace(new RegExp(find, 'g'), replace);
        }

        if ($object.select instanceof jQuery && $object.select.is('select')) {
            if ($object.select.hasClass('smselectdone'))
                smselect.destroySelect($object.select);
            smselect.initSelect($object, false);
        }
        else if ($object.hasOwnProperty('isMultiSelect') && $object.isMultiSelect) {
            if ($object.data instanceof Array && $object.data.length > 0 && $object.data[0].hasOwnProperty('text') && $object.data[0].options instanceof Array && $object.data[0].options.length > 0 && $object.data[0].options[0].hasOwnProperty('text') && $object.data[0].options[0].hasOwnProperty('value') && $object.container instanceof jQuery)
                smselect.initMultiSelect($object);
        }

        else if ($object.data instanceof Array && $object.data.length > 0 && $object.data[0].hasOwnProperty('text') && $object.data[0].hasOwnProperty('value') && $object.container instanceof jQuery) {
            if ($object.container.find('.smselect').length > 0)
                smselect.destroyArray($object);
            smselect.initArray($object);
        }
        else {
            console.error('smselect: Object is of invalid type.');
        }
        return;
    }

    smSelect.prototype.initSelect = function initSelect($object, bindOptionsOnly) {
        var id = $object.select.prop('id');
        var uniqueId = (id !== '' ? 'smselect_' + id : 'smselect_' + (smselect.counter + 1));
        var html = '';
        var options = $object.select.find('option');
        var selectedText = '';
        if (!bindOptionsOnly) {
            html = '<div id="' + uniqueId + '" class="smselect" postbackref="' + ($object.hasOwnProperty('postbackref') ? $object.postbackref.replaceAll('"', '&quot;') : '') + '" overridetext="' + ($object.hasOwnProperty('overridetext') ? $object.overridetext : '') + '"><div class="smselectrun' + ($object.hasOwnProperty('textClass') ? ' ' + $object.textClass : '') + '"><div class="smselectanchorcontainer"><a href="#" class="smselectanchorrun" onclick="javascript:return false;"></a></div><div class="fa fa-sort-desc smselectimage"></div></div><div class="smselectcon"><div class="smselectbar"><input type="text" class="smselecttext" placeholder="Search..." autocomplete="off" /></div><div class="smselectoptions">';
            $.each(options, function (ii, ee) {
                var flag = false;
                if ($(ee).is(':selected')) {
                    flag = true;
                    selectedText = $(ee).text();
                }
                html += '<div class="smselectoption' + (flag ? ' smselected smselecthover' : '') + '" data-value="' + $(ee).val() + '" title="' + $(ee).text() + '">' + $(ee).text() + '</div>';
            });
            html += '</div></div></div>';
            $(html).insertAfter($object.select);
        }
        else {
            $.each(options, function (ii, ee) {
                var flag = false;
                if ($(ee).is(':selected')) {
                    flag = true;
                    selectedText = $(ee).text();
                }
                html += '<div class="smselectoption' + (flag ? ' smselected smselecthover' : '') + '" data-value="' + $(ee).val() + '" title="' + $(ee).text() + '">' + $(ee).text() + '</div>';
            });
            $object.select.next('#' + uniqueId).find('.smselectoptions').html(html);
        }
        var $smselect = $object.select.next('#' + uniqueId);
        var opt = $smselect.find('.smselectoptions');
        var optChildren = opt.children();
        if (optChildren.length * optChildren.eq(0).height() > opt.css('max-height').split('p')[0])
            opt.smslimscroll({ height: '250px', railVisible: true, alwaysVisible: true, wheelStep: $object.wheelStep === undefined ? 10 : $object.wheelStep });
        smselect.attachEventHandlers($smselect, false);

        if ($object.hasOwnProperty('overridetext') && $object.overridetext !== '')
            $smselect.find('.smselectanchorrun').text($object.overridetext).prop('title', $object.overridetext);
        else if (selectedText !== '')
            $smselect.find('.smselectanchorrun').text(selectedText).prop('title', selectedText);

        $object.select.addClass('smselectdone');
        smselect.counter++;
        $object.select.hide();
        $object.select.css({ 'visibility': 'hidden' });

        if ($object.hasOwnProperty('showSearch') && !$object.showSearch)
            $smselect.find('.smselectbar').hide();
        else if (!$object.hasOwnProperty('showSearch') && $smselect.find('.smselectoption').length < 21)
            $smselect.find('.smselectbar').hide();

        if ($object.hasOwnProperty('isRunningText') && $object.isRunningText)
            $smselect.find('.smselectimage').hide();
        else {
            $smselect.find('.smselectrun').addClass('smselectrun2');
            $smselect.find('.smselectanchorrun').addClass('smselectanchorrun2');
            $smselect.find('.smselectimage').addClass('smselectimage2');
            $smselect.find('.smselectanchorcontainer').addClass('smselectanchorcontainer2');
        }

        if ($object.hasOwnProperty('ready') && typeof $object.ready === 'function')
            $object.ready($smselect);
    }

    smSelect.prototype.initArray = function initArray($object) {
        var uniqueId = 'smselect_' + ($object.hasOwnProperty('id') ? $object.id : smselect.counter + 1);
        var html = '<div id="' + uniqueId + '" class="smselect" postbackref="' + ($object.hasOwnProperty('postbackref') ? $object.postbackref.replaceAll('"', '&quot;') : '') + '" overridetext="' + ($object.hasOwnProperty('overridetext') ? $object.overridetext : '') + '" heading="' + ($object.hasOwnProperty('heading') ? $object.heading : '') + '"><div class="smselectrun' + ($object.hasOwnProperty('textClass') ? ' ' + $object.textClass : '') + '"><div class="smselectanchorcontainer"><a href="#" class="smselectanchorrun" onclick="javascript:return false;"></a></div><div class="fa fa-sort-desc smselectimage"></div></div><div class="smselectcon"><div class="smselectbar"><input type="text" class="smselecttext" placeholder="Search..." autocomplete="off" /></div><div class="smselectoptions">';
        var options = $object.data;
        var selectedText = $object.selectedText ? $object.selectedText : '';
        $.each(options, function (ii, ee) {
            html += '<div class="smselectoption' + (selectedText === ee.text ? ' smselected smselecthover' : '') + '" data-value="' + ee.value + '" title="' + ee.text + '">' + ee.text + '</div>';
        });
        html += '</div></div></div>';
        $($object.container).append(html);
        var $smselect = $('#' + uniqueId);
        var opt = $smselect.find('.smselectoptions');
        var optChildren = opt.children();
        if (optChildren.length * optChildren.eq(0).height() > opt.css('max-height').split('p')[0])
            opt.smslimscroll({ height: '250px', railVisible: true, alwaysVisible: true });
        smselect.attachEventHandlers($smselect, false);

        if ($object.hasOwnProperty('overridetext') && $object.overridetext !== '')
            $smselect.find('.smselectanchorrun').text($object.overridetext).prop('title', $object.overridetext);
        if ($object.hasOwnProperty('heading') && $object.heading !== '')
            $smselect.find('.smselectanchorrun').text($object.heading).prop('title', $object.heading);
        else if (selectedText !== '')
            $smselect.find('.smselectanchorrun').text(selectedText).prop('title', selectedText);

        smselect.counter++;

        if ($object.hasOwnProperty('showSearch') && !$object.showSearch)
            $smselect.find('.smselectbar').hide();
        else if (!$object.hasOwnProperty('showSearch') && $smselect.find('.smselectoption').length < 21)
            $smselect.find('.smselectbar').hide();

        if ($object.hasOwnProperty('isRunningText') && $object.isRunningText)
            $smselect.find('.smselectimage').hide();
        else {
            $smselect.find('.smselectrun').addClass('smselectrun2');
            $smselect.find('.smselectanchorrun').addClass('smselectanchorrun2');
            $smselect.find('.smselectimage').addClass('smselectimage2');
            $smselect.find('.smselectanchorcontainer').addClass('smselectanchorcontainer2');
        }

        if ($object.hasOwnProperty('ready') && typeof $object.ready === 'function')
            $object.ready($smselect);
    }

    smSelect.prototype.initMultiSelect = function initMultiSelect($object) {
        var uniqueId = 'smselect_' + ($object.hasOwnProperty('id') ? $object.id : smselect.counter + 1);

        var html = '<div id="' + uniqueId + '" class="smselect smselectmulti" postbackref="' + ($object.hasOwnProperty('postbackref') ? $object.postbackref.replaceAll('"', '&quot;') : '') + '"><div class="smselectrun' + ($object.hasOwnProperty('textClass') ? ' ' + $object.textClass : '') + '"><div class="smselectanchorcontainer"><a href="#" class="smselectanchorrun" onclick="javascript:return false;"></a></div><div class="fa fa-sort-desc smselectimage"></div></div><div class="smselectcon"><div class="smselectbar"><input type="text" class="smselecttext" placeholder="Search..." autocomplete="off" /></div><div class="smselectoptions smselectmultioptions"><div class="smselectsection"><div class="smselectmultioptioncon"><div class="smselectsectionbar smselecthidebar"></div><div class="smselectmulticon smselectallmulticon selectall"><div class="smselectallmultioption selectall" data-value="All" title="Select All">All</div></div></div></div>';
        var sections = $object.data;
        var checkedValueArr = new Array();
        var singlesection = false;
        if (sections.length === 1)
            singlesection = true;
        $.each(sections, function (ii, ee) {
            html += '<div class="smselectsection' + (ii === 0 ? ' smselectsectionfirst' : '') + '"><div class="smselectsectionbar">' + ee.text + '</div><div class="smselectmultioptioncon">';
            $.each(ee.options, function (iii, eee) {
                var flag = false;
                if ($object.hasOwnProperty('selectedItems') && $object.selectedItems.length > 0 && $.inArray(eee.text, $object.selectedItems) > -1)
                    flag = true;
                html += '<div class="smselectmulticon"><div class="smselectmultioption' + (flag ? ' smselected' : '') + '" data-value="' + eee.value + '" title="' + eee.text + '">' + eee.text + '</div><div class="smselectcheck fa';
                if (flag) {
                    html += ' fa-check';
                    checkedValueArr.push(eee.value);
                }
                html += '"></div></div>';
            });
            html += '</div></div>';
        });
        html += '</div></div></div>';

        $($object.container).html(html);
        var $smselect = $('#' + uniqueId);
        var checks = $smselect.find('.fa-check');
        var sections = $smselect.find('.smselectsection');
        var totalOptions = 0;
        $.each(sections, function (jj, kk) {
            var optChildren = $(kk).find('.smselectmultioption');
            totalOptions += optChildren.length;

            if (optChildren.length > 5)
                $(kk).find('.smselectmultioptioncon').smslimscroll({ height: '125px', railVisible: true, alwaysVisible: true, wheelStep: $object.wheelStep === undefined ? 10 : $object.wheelStep });
        });


        if ($object.hasOwnProperty('selectedItems') && $object.selectedItems.length > 0) {
            var text;
            if (checks.length === totalOptions) {
                if ($object.hasOwnProperty('allText') && $object.allText !== '')
                    text = $object.allText;
                else
                    text = 'All ' + $object.text + ' selected';
                $smselect.find('.smselectallmultioption').addClass('smselectallmultioptionchecked');
            }
            else if (checks.length === 1)
                text = checks.parent().find('.smselected').html().trim();
            else
                text = $object.selectedItems.length.toString() + ' ' + $object.text + ' selected';
            $smselect.find('.smselectanchorrun').text(text).prop('title', text);
        }
        else
            $smselect.find('.smselectanchorrun').text('Select ' + $object.text).prop('title', 'Select ' + $object.text);

        smselect.attachEventHandlers($smselect, true);
        smselect.multiCollection[uniqueId] = { counter: checks.length, dictionary: checkedValueArr, text: $object.text, allText: ($object.hasOwnProperty('allText') ? $object.allText : '') };
        smselect.counter++;

        if ($object.hasOwnProperty('showSearch') && !$object.showSearch)
            $smselect.find('.smselectbar').hide();
        else if (!$object.hasOwnProperty('showSearch') && $smselect.find('.smselectoption').length < 21)
            $smselect.find('.smselectbar').hide();

        if ($object.hasOwnProperty('isRunningText') && $object.isRunningText)
            $smselect.find('.smselectimage').hide();
        else {
            $smselect.find('.smselectrun').addClass('smselectrun2');
            $smselect.find('.smselectanchorrun').addClass('smselectanchorrun2');
            $smselect.find('.smselectimage').addClass('smselectimage2');
            $smselect.find('.smselectanchorcontainer').addClass('smselectanchorcontainer2');
        }

        if ($object.hasOwnProperty('ready') && typeof $object.ready === 'function')
            $object.ready($smselect);
    }

    smSelect.prototype.attachEventHandlers = function attachEventHandlers($smselect, isMulti) {
        var textClickHandler = function (e) {
            if ($smselect.find('.smselectdisabled').length === 0) {
                var current = $smselect.find('.smselectcon');
                var display = current.css('display');
                var all = $('.smselectcon').hide();
                current.toggle();
                if (display === 'none') {
                    current.show();
                    var options = $smselect.find('.smselectoption');
                    options.show();
                    if (current.find('.smselectbar').css('display') !== 'none')
                        current.find('.smselecttext').focus().css('outline', 'none');

                    setTop(current);
                }
                else
                    current.hide();
            }
            e.stopPropagation();
        }

        $smselect.find('.smselectbar').unbind('click').click(function (e) { e.stopPropagation(); });
        $smselect.find('.smselectrun').unbind('click').click(textClickHandler);
        $smselect.find('.smselectimage').unbind('click').click(textClickHandler);
        var $textbox = $smselect.find('.smselecttext');

        if (isMulti) {
            $smselect.find('.smselectoptions').unbind('click').on('click', '.smselectmulticon', function (e, preventTrigger) {
                e.stopPropagation();
                var insertFlag = false;
                var opt;
                var allChk;
                var smsel;
                var dicKey;
                var val;
                var chk = $(this).find('.smselectcheck');
                if (chk.length > 0) {
                    smsel = $(chk).parent().parent().parent().parent().parent().parent();
                    if (smsel.hasClass('smselectcon'))
                        smsel = smsel.parent();
                    opt = $(chk).siblings('.smselectmultioption');
                    dicKey = opt.attr('data-value');
                }
                else {
                    allChk = $(this).find('.smselectallmultioption.selectall');
                    smsel = $(allChk).parent().parent().parent().parent().parent().parent();
                    dicKey = 'All';
                    if (!allChk.hasClass('smselectallmultioptionchecked'))
                        insertFlag = true;
                }
                allChk = smsel.find('.smselectallmultioption.selectall');
                var hfl = smsel.parent().prev();
                var checks = smsel.find('.smselectcheck');
                var pseudo = smsel.find('.smselectanchorrun');
                var uniqueId = smsel.prop('id');
                var obj = smselect.multiCollection[uniqueId];

                if (chk.length > 0) {
                    if ($(chk).hasClass('fa-check')) {
                        obj.counter--;
                        $(chk).removeClass('fa-check');
                    }

                    else {
                        obj.counter++;
                        $(chk).addClass('fa-check');
                        insertFlag = true;
                    }
                }

                if (insertFlag) {
                    if (dicKey !== 'All') {
                        if ($.inArray(dicKey, obj.dictionary) === -1) {
                            obj.dictionary.push(dicKey);
                            opt.addClass('smselected');
                        }
                    }
                }

                else {
                    if (dicKey !== 'All') {
                        var index = $.inArray(dicKey, obj.dictionary);
                        if (index > -1) {
                            obj.dictionary.splice(index, 1);
                            opt.removeClass('smselected');
                        }
                    }
                }

                if (dicKey === 'All') {
                    if (insertFlag) {
                        var options = smsel.find('.smselectmulticon');
                        obj.dictionary = new Array();

                        $.each(options, function (index, value) {
                            var check = $(value).find('.smselectcheck');
                            if (check.length > 0) {
                                $(check).addClass('fa-check');
                                $(check).prev().addClass('smselected');
                                dicKey = $(value).find('.smselectmultioption').attr('data-value');

                                if ($.inArray(dicKey, obj.dictionary) === -1)
                                    obj.dictionary.push(dicKey);
                            }
                        });
                        obj.counter = $(checks).length;
                        allChk.addClass('smselectallmultioptionchecked');
                    }
                    else {
                        allChk.removeClass('smselectallmultioptionchecked');
                        $(checks).removeClass('fa-check');
                        smsel.find('.smselected').removeClass('smselected');
                        obj.counter = 0;
                        obj.dictionary = new Array();
                    }
                }
                else {
                    if (obj.counter === $(checks).length) {
                        allChk.addClass('smselectallmultioptionchecked');
                    }

                    else {
                        allChk.removeClass('smselectallmultioptionchecked');
                    }
                }

                var selectedItems = smselect.getSelectedOption(smsel);
                if (obj.counter === 1)
                    $(pseudo).text(selectedItems[0].text);

                else if (obj.counter < $(checks).length && obj.counter > 0) {
                    $(pseudo).text(obj.counter + ' ' + obj.text + ' selected');
                }

                else if (obj.counter === $(checks).length) {
                    var text;
                    if (obj.allText !== '')
                        text = obj.allText;
                    else
                        text = 'All ' + obj.text + ' selected';
                    $(pseudo).text(text);
                }

                else if (obj.counter === 0) {
                    $(pseudo).text('Select ' + obj.text);
                }

                hfl.val(JSON.stringify(selectedItems));

                if (typeof preventTrigger === 'undefined' || !preventTrigger)
                    smsel.trigger('change');
            });

            $textbox.keyup(function (e) {
                e.stopPropagation();
                var con = $(this).parent().next();
                if (con.hasClass('smselectoptions'))
                    options = con;
                else
                    options = con.find('.smselectoptions');
                var value = $(this).val().trim().toLowerCase();
                if (options.css('display') === 'none') {
                    $(options).toggle();
                }

                var opts = options.find('.smselectmultioption');
                $.each(opts, function (ii, ee) {
                    if (value !== '') {
                        if ($(ee).text().trim().toLowerCase().indexOf(value) > -1) {
                            $(ee).parent()[0].style.display = "block";
                        }
                        else
                            $(ee).parent()[0].style.display = "none";
                    }
                    else
                        $(ee).parent()[0].style.display = "block";
                });
            });
        }
        else {
            $smselect.find('.smselectoption').unbind('click').click(function (e, preventTrigger) {
                var val = $(this).attr('data-value').trim();
                var smsel = $(this).parents('.smselect');
                var select = smsel.prev();
                var hfl;
                var cont = smsel.parent().parent().find('[id*="smselect_container_"]');
                if (!select.is('select')) {
                    hfl = smsel.parent().find('[type="hidden"]');
                    if (hfl.length > 0)
                        select = smsel.parent().parent().find('select');
                    else {
                        hfl = smsel.parent().parent().find('[id*="hfl_"]');
                        if (cont.length > 0)
                            select = cont.parent().find('select');
                        else
                            select = smsel.parent().find('select');
                    }
                }
                var triggerChangeEvent = !($(this).hasClass('smselected'));
                smsel.find('.smselected').removeClass('smselecthover').removeClass('smselected');
                $(this).addClass('smselected').addClass('smselecthover');

                if (select.is('select')) {
                    select.find('option').filter(function () { if ($(this).val() === val) return $(this); }).prop('selected', true);
                    if ((typeof preventTrigger === 'undefined' || !preventTrigger) && triggerChangeEvent)
                        smselect.triggerEvent(select[0], 'change');
                }
                var par = $(this).parent().parent();
                if (!par.hasClass('smselectcon'))
                    par = par.parent();
                par[0].style.display = "none";

                var text = $smselect.find('.smselecttext');
                var rundiv = $smselect.find('.smselectrun');
                text.val('');

                var tt = smsel.attr('overridetext');
                if (typeof tt === 'undefined' || tt === '')
                    rundiv.find('a').text($(this).text().trim());
                else
                    rundiv.find('a').text(tt);

                e.stopPropagation();

                if (typeof hfl !== 'undefined' && hfl != null) {
                    if (hfl.length > 0) {
                        $('[name*="' + hfl.prop('name') + '"]').val(JSON.stringify(smselect.getSelectedOption(smsel)));
                        eval(smsel.attr('postbackref'));
                    }
                }
                if (typeof preventTrigger === 'undefined' || !preventTrigger)
                    smsel.trigger('change');
            });

            $textbox.keyup(function (e) {
                e.stopPropagation();
                var con = $(this).parent().next();
                if (con.hasClass('smselectoptions'))
                    options = con;
                else
                    options = con.find('.smselectoptions');
                var value = $(this).val().trim().toLowerCase();
                if (options.css('display') === 'none') {
                    $(options).toggle();
                }
                var which = e.which;
                var item;
                var foundFirst = false;
                var opts = options.find('.smselectoption');
                $.each(opts, function (ii, ee) {
                    if (value !== '') {
                        $(ee).removeClass('smselecthover');
                        if ($(ee).text().trim().toLowerCase().indexOf(value) > -1) {
                            if ((which === 13 || which === 9) && !foundFirst) {
                                foundFirst = true;
                                item = $(ee);
                                ee.style.display = "block";
                            }
                            else {
                                ee.style.display = "block";
                            }
                        }
                        else
                            ee.style.display = "none";
                    }
                    else
                        ee.style.display = "block";
                });

                if (item instanceof jQuery && item.hasClass('smselectoption'))
                    item.trigger('click');
            });
        }
        $textbox.keydown(function (e) {
            if (e.which === 13 || e.which === 9) {
                e.preventDefault();
                return false;
            }
        });

        $smselect.on('change', function () { });

        $(document).unbind('click', docHandler).click(docHandler);
    }

    smSelect.prototype.setOptionByText = function setOptionByText($smselect, selectedText, preventTrigger) {
        var options;
        if ($smselect instanceof jQuery && $smselect.is('select'))
            options = $smselect.next().find('.smselectoption');
        else
            if (smselect.multiCollection.hasOwnProperty($smselect.prop('id'))) {
                options = $smselect.find('.smselectmultioption');
            }
            else {
                options = $smselect.find('.smselectoption');
            }
        var item;
        $.each(options, function (ii, ee) {
            if ($(ee).text().trim().toLowerCase() === selectedText.toLowerCase()) {
                item = $(ee);
                return false;
            }
        });
        if (item instanceof jQuery) {
            if (typeof preventTrigger !== 'undefined' || !preventTrigger)
                item.trigger('click', [preventTrigger]);
        }
    }

    smSelect.prototype.setOptionByValue = function setOptionByValue($smselect, selectedValue, preventTrigger) {
        var options;
        if ($smselect instanceof jQuery && $smselect.is('select'))
            options = $smselect.next().find('.smselectoption');
        else
            if (smselect.multiCollection.hasOwnProperty($smselect.prop('id'))) {
                options = $smselect.find('.smselectmultioption');
            }
            else {
                options = $smselect.find('.smselectoption');
            }
        var item;
        $.each(options, function (ii, ee) {
            if ($(ee).attr('data-value').trim().toLowerCase() === selectedValue.toLowerCase()) {
                item = $(ee);
                return false;
            }
        });
        if (item instanceof jQuery) {
            if (smselect.multiCollection.hasOwnProperty($smselect.prop('id'))) {
                if (typeof preventTrigger !== 'undefined' || !preventTrigger)
                    item.parent().trigger('click', [preventTrigger]);
            }
            else {
                if (typeof preventTrigger !== 'undefined' || !preventTrigger)
                    item.trigger('click', [preventTrigger]);
            }
        }
    }

    smSelect.prototype.setOptionByIndex = function setOptionByIndex($smselect, selectedIndex, preventTrigger) {
        var options;
        if ($smselect instanceof jQuery && $smselect.is('select'))
            options = $smselect.next().find('.smselectoption');
        else
            options = $smselect.find('.smselectoption');
        var item;
        $.each(options, function (ii, ee) {
            if (ii === selectedIndex) {
                item = $(ee);
                return false;
            }
        });
        if (item instanceof jQuery) {
            if (smselect.multiCollection.hasOwnProperty($smselect.prop('id'))) {
                if (typeof preventTrigger !== 'undefined' || !preventTrigger)
                    item.parent().trigger('click', [preventTrigger]);
            }
            else {
                if (typeof preventTrigger !== 'undefined' || !preventTrigger)
                    item.trigger('click', [preventTrigger]);
            }
        }
    }

    smSelect.prototype.hideOptionByIndex = function hideOptionByIndex($smselect, selectedIndex) {
        var options;
        if ($smselect instanceof jQuery && $smselect.is('select'))
            options = $smselect.next().find('.smselectoption');
        else
            options = $smselect.find('.smselectoption');
        var item;
        $.each(options, function (ii, ee) {
            if (ii === selectedIndex) {
                item = $(ee);
                return false;
            }
        });
        if (item instanceof jQuery) {
            if (smselect.multiCollection.hasOwnProperty($smselect.prop('id'))) {
                //item.parent().trigger('click');
            }
            else {
                $(item).addClass("smselectdisabledoption");
            }
        }
    }

    smSelect.prototype.showOptionByIndex = function showOptionByIndex($smselect, selectedIndex) {
        var options;
        if ($smselect instanceof jQuery && $smselect.is('select'))
            options = $smselect.next().find('.smselectoption');
        else
            options = $smselect.find('.smselectoption');
        var item;
        $.each(options, function (ii, ee) {
            if (ii === selectedIndex) {
                item = $(ee);
                return false;
            }
        });
        if (item instanceof jQuery) {
            if (smselect.multiCollection.hasOwnProperty($smselect.prop('id'))) {
                //item.parent().trigger('click');
            }
            else {
                $(item).removeClass("smselectdisabledoption");
            }
        }
    }

    smSelect.prototype.getSelectedOption = function getSelectedOption($smselect) {
        var options;
        if ($smselect instanceof jQuery && $smselect.is('select'))
            options = $smselect.next().find('.smselected');
        else
            options = $smselect.find('.smselected');
        var lst = new Array();
        $.each(options, function (ii, ee) {
            lst.push({ value: $(ee).attr('data-value'), text: $(ee).text() });
        });
        return lst;
    }

    smSelect.prototype.reset = function reset($smselect) {
        var options;
        if ($smselect instanceof jQuery && $smselect.is('select'))
            options = $smselect.next().find('.smselected');
        else
            options = $smselect.find('.smselected');
        options.removeClass('smselected');
        options.removeClass('smselecthover');
        $(document).click();

        if ($smselect.hasClass('smselectmulti')) {
            var allChk = $smselect.find('.smselectallmultioption.selectall');
            var checks = $smselect.find('.smselectcheck');
            var uniqueId = $smselect.prop('id');
            var obj = smselect.multiCollection[uniqueId];
            var pseudo = $smselect.find('.smselectanchorrun');

            allChk.removeClass('smselectallmultioptionchecked');
            checks.removeClass('fa-check');
            obj.counter = 0;
            obj.dictionary = new Array();
            $(pseudo).text('Select ' + obj.text);
        }
    }

    smSelect.prototype.disable = function disable($smselect) {
        if ($smselect instanceof jQuery && $smselect.is('select'))
            $smselect.next().find('.smselectanchorrun').addClass('smselectdisabled');
        else
            $smselect.find('.smselectanchorrun').addClass('smselectdisabled');
    }

    smSelect.prototype.enable = function enable($smselect) {
        if ($smselect instanceof jQuery && $smselect.is('select'))
            $smselect.next().find('.smselectanchorrun').removeClass('smselectdisabled');
        else
            $smselect.find('.smselectanchorrun').removeClass('smselectdisabled');
    }

    smSelect.prototype.hide = function hide($smselect) {
        if (typeof $smselect === 'undefined' || $smselect === null)
            $('.smselectcon').hide();
        $smselect.find('.smselectcon').hide();
    }

    smSelect.prototype.hideDropDown = function hideDropDown($smselect) {
        if ($smselect instanceof jQuery && $smselect.is('select'))
            $smselect.next().hide();
        else
            $smselect.hide();
    }

    smSelect.prototype.showDropDown = function showDropDown($smselect) {
        if ($smselect instanceof jQuery && $smselect.is('select'))
            $smselect.next().show();
        else
            $smselect.show();
    }

    smSelect.prototype.destroySelect = function destroySelect($object) {
        $object.next('.smselect').remove();
        $object.removeClass('smselectdone');
        $object.show();
        $object.css({ 'visibility': 'visible' });
        $(document).unbind('click', docHandler);
        smselect.counter--;
    }

    smSelect.prototype.triggerEvent = function triggerEvent(elem, evt) {
        if (elem.addEventListener) {
            var event = document.createEvent('HTMLEvents');
            event.initEvent(evt, true, true);
            elem.dispatchEvent(event);
        }
        else if (elem.attachEvent) {
            var event = document.createEventObject();
            event.eventType = evt;
            elem.fireEvent('on' + evt, event);
        }
    }

    smSelect.prototype.destroyArray = function destroyArray($object) {
        $object.container.find('.smselect').remove();
        $(document).unbind('click', docHandler);
        smselect.counter--;
    }
    return smselect;
})();