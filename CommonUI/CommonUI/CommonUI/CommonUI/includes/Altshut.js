var altshut = (function () {
    function altShut() {
        this.counter = 0;
        this.multiCollection = new Object();
    }
    var altshut = new altShut();

    altShut.prototype.create = function create($object) {
        String.prototype.replaceAll = function (find, replace) {
            return this.replace(new RegExp(find, 'g'), replace);
        }

        if ($object.data instanceof Array && $object.data.length > 0 && $object.data[0].hasOwnProperty('text') && $object.data[0].hasOwnProperty('value') && $object.container instanceof jQuery)
            altshut.initMultiSelect($object);
        else {
            console.error('altshut: Object is of invalid type.');
        }
        return;
    }

    altShut.prototype.initMultiSelect = function initMultiSelect($object) {
        var uniqueId = 'altshut_' + ($object.hasOwnProperty('id') ? $object.id : altshut.counter + 1);
        var heading = ($object.hasOwnProperty('heading') ? $object.heading : '');

        var html = '<div id="' + uniqueId + '" class="altshut altshutmulti"><div class="altshuthead altshut' + heading.toLowerCase() + '"><div class="altshutheadicon altshuticon' + heading.toLowerCase() + '"></div><div class="altshutheadtext altshutheadtext' + heading.toLowerCase() + '" title="' + heading + '">' + heading + '</div></div><div class="altshutcon"><div class="altshutbar"><input type="text" class="altshuttext" placeholder="Search..." autocomplete="off" /></div><div class="altshutoptions altshutmultioptions"><div class="altshutsection"><div class="altshutmultioptioncon"><div class="altshutsectionbar altshuthidebar"></div><div class="altshutmulticon altshutallmulticon selectall"><div class="altshutallmultioption selectall" data-value="All" title="Select All">All</div></div></div></div><div class="altshutsection">';
        var checkedValueArr = {};
        var hasSelected = false;
        if ($object.hasOwnProperty('selectedItems') && $object.selectedItems.length > 0)
            hasSelected = true;
        $.each($object.data, function (iii, eee) {
            var flag = false;
            if (hasSelected && $.inArray(eee.text, $object.selectedItems) > -1)
                flag = true;
            html += '<div class="altshutmulticon"><div class="altshutmultioption' + (flag ? ' altshuted' : '') + '" data-value="' + eee.value + '" title="' + eee.text + '">' + eee.text + '</div><div class="altshutcheck fa';
            if (flag) {
                html += ' fa-check';
                checkedValueArr[eee.value] = eee.text;
            }
            html += '"></div></div>';
        });
        html += '</div></div></div><div class="altshutdonebutton">Done</div></div>';

        $($object.container).html(html);
        var $altshut = $('#' + uniqueId);
        var checks = $altshut.find('.fa-check');
        var options = $altshut.find('.altshutmultioption');
        var totalOptions = options.length;

        if (totalOptions > 5)
            $altshut.find('.altshutsection').eq(1).smslimscroll({ height: '160px', width: '100%', railVisible: true, alwaysVisible: true });

        if ($object.hasOwnProperty('selectedItems') && $object.selectedItems.length > 0) {
            var text;
            if (checks.length === totalOptions) {
                $altshut.find('.altshutallmultioption').addClass('altshutallmultioptionchecked');
            }
        }
        altshut.attachEventHandlers($altshut);
        altshut.multiCollection[uniqueId] = { counter: checks.length, dictionary: checkedValueArr, heading: ($object.hasOwnProperty('heading') ? $object.heading : ''), text: $object.text };
        altshut.counter++;

        if ($object.hasOwnProperty('showSearch') && !$object.showSearch)
            $altshut.find('.altshutbar').hide();
        else if (!$object.hasOwnProperty('showSearch') && $altshut.find('.altshutoption').length < 21)
            $altshut.find('.altshutbar').hide();

        altshut.createNoEdit($altshut);

        if ($object.hasOwnProperty('ready') && typeof $object.ready === 'function')
            $object.ready($altshut);
    }

    altShut.prototype.attachEventHandlers = function attachEventHandlers($altshut) {
        $altshut.find('.altshutbar').click(function (e) { e.stopPropagation(); });
        var $textbox = $altshut.find('.altshuttext');

        $altshut.find('.altshutoptions').on('click', '.altshutmulticon', function (e) {
            e.stopPropagation();
            var insertFlag = false;
            var opt;
            var allChk;
            var smsel;
            var dicKey;
            var val;
            var chk = $(this).find('.altshutcheck');
            if (chk.length > 0) {
                smsel = $(chk).parent().parent().parent().parent().parent();
                if (smsel.hasClass('altshutcon'))
                    smsel = smsel.parent();
                opt = $(chk).siblings('.altshutmultioption');
                dicKey = { value: opt.attr('data-value'), text: opt.html().trim() };
            }
            else {
                allChk = $(this).find('.altshutallmultioption.selectall');
                smsel = $(allChk).parent().parent().parent().parent().parent().parent();
                dicKey = { value: 'All', text: 'All' };
                if (!allChk.hasClass('altshutallmultioptionchecked'))
                    insertFlag = true;
            }
            allChk = smsel.find('.altshutallmultioption.selectall');
            var checks = smsel.find('.altshutcheck');
            var uniqueId = smsel.prop('id');
            var obj = altshut.multiCollection[uniqueId];

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
                if (dicKey.value !== 'All') {
                    if (!obj.dictionary.hasOwnProperty(dicKey.value)) {
                        obj.dictionary[dicKey.value] = dicKey.text;
                        opt.addClass('altshuted');
                    }
                }
            }

            else {
                if (dicKey.value !== 'All') {
                    if (obj.dictionary.hasOwnProperty(dicKey.value)) {
                        delete obj.dictionary[dicKey.value];
                        opt.removeClass('altshuted');
                    }
                }
            }

            if (dicKey.value === 'All') {
                if (insertFlag) {
                    var options = smsel.find('.altshutmulticon');
                    obj.dictionary = {};

                    $.each(options, function (index, value) {
                        var check = $(value).find('.altshutcheck');
                        if (check.length > 0) {
                            $(check).addClass('fa-check');
                            $(check).prev().addClass('altshuted');
                            var opt = $(value).find('.altshutmultioption');
                            dicKey = { value: opt.attr('data-value'), text: opt.html().trim() };

                            if (!obj.dictionary.hasOwnProperty(dicKey.value))
                                obj.dictionary[dicKey.value] = dicKey.text;
                        }
                    });
                    obj.counter = $(checks).length;
                    allChk.addClass('altshutallmultioptionchecked');
                }
                else {
                    allChk.removeClass('altshutallmultioptionchecked');
                    $(checks).removeClass('fa-check');
                    smsel.find('.altshuted').removeClass('altshuted');
                    obj.counter = 0;
                    obj.dictionary = {};
                }
            }
            else {
                if (obj.counter === $(checks).length) {
                    allChk.addClass('altshutallmultioptionchecked');
                }

                else {
                    allChk.removeClass('altshutallmultioptionchecked');
                }
            }

            smsel.trigger('change');
        });

        $textbox.keyup(function (e) {
            e.stopPropagation();
            var con = $(this).parent().next();
            if (con.hasClass('altshutoptions'))
                options = con;
            else
                options = con.find('.altshutoptions');
            var value = $(this).val().trim().toLowerCase();
            if (options.css('display') === 'none') {
                $(options).toggle();
            }

            var opts = options.find('.altshutmultioption');
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

        $textbox.keydown(function (e) {
            if (e.which === 13 || e.which === 9) {
                e.preventDefault();
                return false;
            }
        });

        $altshut.on('change', function () { });

        $altshut.find('.altshutdonebutton').click(function (e) {
            e.stopPropagation();
            altshut.createNoEdit($altshut);
        });
    }

    altShut.prototype.createNoEdit = function createNoEdit($altshut) {
        var uniqueId = $altshut.prop('id');
        var obj = altshut.multiCollection[uniqueId];

        var html = '';
        if (obj.counter > 0) {
            html += '<div class="altshutnonedit"><div class="altshutnoeditscroll">';
            for (var key in obj.dictionary) {
                html += '<div class="altshutstaticmulticon"><div class="altshutstatic" title="' + obj.dictionary[key] + '" data-value="' + key + '">' + obj.dictionary[key] + '</div></div>';
            }
            html += '</div></div><div class="altshuteditbutton" title="Edit">Edit</div>';
            $altshut.find('.altshutnonedit').remove();
            $altshut.find('.altshuteditbutton').remove();
            $altshut.append(html);
            $altshut.find('.altshutcon').hide();
            $altshut.find('.altshutdonebutton').hide();
            if (obj.counter > 5)
                $altshut.find('.altshutnoeditscroll').smslimscroll({ height: '210px', width: '100%', railVisible: true, alwaysVisible: true });

            $altshut.find('.altshuteditbutton').unbind('click').click(function (e) {
                e.stopPropagation();
                $altshut.find('.altshutcon').show();
                $altshut.find('.altshutdonebutton').show();
                $altshut.find('.altshutnonedit').hide();
                $altshut.find('.altshuteditbutton').hide();
            });
        }
    }

    altShut.prototype.getSelectedOption = function getSelectedOption($altshut) {
        var options;
        if ($altshut instanceof jQuery && $altshut.is('select'))
            options = $altshut.next().find('.altshuted');
        else
            options = $altshut.find('.altshuted');
        var lst = new Array();
        $.each(options, function (ii, ee) {
            lst.push({ value: $(ee).attr('data-value').trim(), text: $(ee).text().trim() });
        });
        return lst;
    }

    altShut.prototype.getAllOptions = function getAllOptions($altshut) {
        var options;
        if ($altshut instanceof jQuery && $altshut.is('select'))
            options = $altshut.next().find('.altshutmultioption');
        else
            options = $altshut.find('.altshutmultioption');
        var lst = new Array();
        $.each(options, function (ii, ee) {
            lst.push({ value: $(ee).attr('data-value').trim(), text: $(ee).text().trim() });
        });
        return lst;
    }

    altShut.prototype.disable = function disable($altshut) {
        if ($altshut instanceof jQuery && $altshut.is('select'))
            $altshut.next().find('.altshutanchorrun').addClass('altshutdisabled');
        else
            $altshut.find('.altshutanchorrun').addClass('altshutdisabled');
    }

    altShut.prototype.enable = function enable($altshut) {
        if ($altshut instanceof jQuery && $altshut.is('select'))
            $altshut.next().find('.altshutanchorrun').removeClass('altshutdisabled');
        else
            $altshut.find('.altshutanchorrun').removeClass('altshutdisabled');
    }

    altShut.prototype.triggerEvent = function triggerEvent(elem, evt) {
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

    return altshut;
})();