var mailtag = (function () {
    function MailTag() {
        this.totalWidth = 0;
        this.isModified = false;
        this.intervalid = 0;
        this.emailVsName = new Object();
        this.NameVsEmail = new Object();
        this.subscriptionInfoIntellisenseCollection = new Array();

        var path = window.location.protocol + '//' + window.location.host;
        var pathname = window.location.pathname.split('/');

        for (var ii = 0; ii < pathname.length; ii++) {
            if ((ii !== 0) && (ii !== pathname.length - 1))
                path = path + '/' + pathname[ii];
        }
        this.path = path;

        this.height = 0;
    }

    function validateObject(object) {
        var passed = false;
        if (object.hasOwnProperty('container')) {
            if (object.container instanceof jQuery) {
                if (object.container.length > 0) {
                    if (object.hasOwnProperty('list')) {
                        if (object.list instanceof Array)
                            passed = true;
                        else
                            console.error('List is not of type Array');
                    }
                    else
                        console.error('Object does not contain property "list".');
                }
                else
                    console.error('Container is empty.');
            }
            else
                console.error('Container is not of type jQuery');
        }
        else
            console.error('Object does not contain property "container".');
        return passed;
    }

    function attachHandlers($object) {
        $object.find('.mailtagTextElement').unbind('keyup').keyup(function (e) {
            e.stopPropagation();
            var $this = $(this);
            var list = [];
            var grandParent = $this.parent().parent();
            var value = $this.val().trim();
            //value = value.split("<")[0].trim();

            if (value != '' && value.indexOf(' <') != -1 && value.indexOf('>') != -1) {
                if (value.split(' <')[1].split('>').length > 0)
                    value = value.split(' <')[1].split('>')[0].trim();
            }

            //            if (value.length < 10) {
            //                $this[0].style.height = '1px';
            //                $this[0].style.height = (25 + $this[0].scrollHeight) + 'px';
            //            }
            //            else
            //                $this[0].style.height = mailtag.height + 'px';

            if (e.which === 13) {
                var valuee = $this.val().replaceAll('\r\n', '').replaceAll('\n', '').trim();
                //valuee = valuee.split("<")[0].trim();
                if (valuee !== '' && valuee.indexOf(' <') != -1 && valuee.indexOf('>') != -1) {
                    if (valuee.split(' <')[1].split('>').length > 0)
                        valuee = valuee.split(' <')[1].split('>')[0];
                }

                if (valuee !== '') {
                    var ele = grandParent.find('.hdnTag');
                    if (ele.val().length > 0) {
                        list = ele.val().split(',');
                    }

                    if (mailtag.emailVsName.hasOwnProperty(valuee))
                        list.push(valuee);  //push emailID for entered text

                    var tempo = {};
                    $.each(list, function (ii, ee) {
                        tempo[ee] = '';
                    });

                    var obj = {};
                    obj.container = grandParent;
                    obj.list = Object.keys(tempo);
                    mailtag.create(obj);
                    mailtag.isModified = true;
                }
            }
            else {
                if (mailtag.intervalid !== 0) {
                    clearTimeout(mailtag.intervalid);
                }
                mailtag.intervalid = setTimeout(function () {
                    if (!/[^a-zA-Z .@]/.test(value)) {
                        mailtag.searchAutocomplete(value, 20, $object);
                    }

                }, 100);
            }
        });

        $object.find('.mailtagTextElement').unbind('keydown').keydown(function (e) {
            e.stopPropagation();
            var $this = $(this);
            var value = $this.val();
        });

        $object.find('.mailtagClose').unbind('click').click(function (e) {
            e.stopPropagation();
            var $this = $(this);
            var parent = $this.parent();
            var mailTextName = parent.find('.mailtagText').html();
            var text = '';

            for (var key in mailtag.emailVsName) {
                for (var item = 0; item < mailtag.emailVsName[key].length; item++) {
                    if (key != '' && mailtag.emailVsName[key][item] == mailTextName)
                        text += key + ',';
                }
            }

            //text = mailtag.NameVsEmail[mailTextName];
            if (text.length > 0)
                text = text.substr(0, text.length - 1);


            var ele = $this.parents('.mailtagContainer').find('.hdnTag');
            var arr = ele.val().split(',');

            var hiddenValue = '';
            $.each(arr, function (ii, ee) {
                if (text.indexOf(ee) === -1)
                    hiddenValue += ee + ',';
            });
            hiddenValue = hiddenValue.substr(0, hiddenValue.length - 1);

            ele.val(hiddenValue);
            parent.remove();
            mailtag.isModified = true;
        });

        $(document).click(function (e) {
            e.stopPropagation();
            $('.mailtagsearchddl').hide();
        });
        $(".hdnTag").change(function (e) {
            debugger;
        })
    }

    function intellisenseHTML(data, prefixText, $object) {
        var html = '';
        var key = '';
        for (var item = 0; item < data.length; item++) {
            key = data[item].split('<');
            if ((key[0].toLowerCase().startsWith(prefixText.toLowerCase()) || key[1].toLowerCase().startsWith(prefixText.toLowerCase())) && $('#' + $object[0].id + ' .hdnTag').val().indexOf(key[1].split('>')[0]) == -1)
                html += '<div class="mailtagsearchddlItem">' + data[item].replaceAll('<', '&lt;').replaceAll('>', '&gt;') + '</div>';
        }
        //for (key in data) {
        //    if ((key.toLowerCase().startsWith(prefixText.toLowerCase()) || data[key].toLowerCase().startsWith(prefixText.toLowerCase())) && $('#' + $object[0].id + ' .hdnTag').val().indexOf(key) == -1)
        //        html += '<div class="mailtagsearchddlItem">' + data[key] + " &lt;" + key + '&gt;</div>';
        //}
        return html;
    }

    function applySlimScroll(containerSelector, bodySelector) {
        var scrollBodyContainer = $(containerSelector);
        var scrollBody = scrollBodyContainer.find(bodySelector);

        if (scrollBodyContainer.height() < scrollBody.height()) {
            scrollBody.smslimscroll({ height: scrollBodyContainer.height() + 'px', alwaysVisible: true, position: 'right', distance: '2px' });
        }
    }

    function isEmpty(obj) {
        for (var key in obj) {
            if (obj.hasOwnProperty(key))
                return false;
        }
        return true;
    }

    MailTag.prototype.searchAutocomplete = function searchAutocomplete(prefixText, count, $object) {
        var onClickIntellisense = function (e) {
            e.stopPropagation();
            var target = $(e.target);
            var ddlDiv = target.parent().parent();
            var valu = target.text().trim();
            if (ddlDiv.prop('class') === 'smslimScrollDiv')
                ddlDiv = ddlDiv.parent();
            ddlDiv.hide();
            var txtBox = ddlDiv.parent().find('.mailtagTextElement');
            txtBox.val(valu);
            var evnt = $.Event('keyup');
            evnt.which = 13;
            txtBox.trigger(evnt);
        }

        var ddl = $object.find('.mailtagsearchddl');
        var records = intellisenseHTML(mailtag.subscriptionInfoIntellisenseCollection, prefixText, $object);
        if (records != '') {
            ddl.html("<div class='mailtagsearchddlOverflowContainer'>" + records + "</div>");
            if ((($('#' + $object[0].id).offset().left + $('#' + $object[0].id).width()) - ($('#' + $object[0].id + ' .mailtagTextContainer').offset().left + 100)) > 201) {
                ddl.css({
                    'top': $('#' + $object[0].id + ' .mailtagTextElement').offset().top + 20,
                    'left': $('#' + $object[0].id + ' .mailtagTextElement').offset().left
                });
            }
            else {
                ddl.css({
                    'top': $('#' + $object[0].id + ' .mailtagTextElement').offset().top + 20,
                    'left': $('#' + $object[0].id + ' .mailtagTextElement').offset().left - 90
                });
            }
            ddl.show();
            var children = ddl.children();
            children.eq(0).css('padding-top', '4px');
            ddl.off('click', '.mailtagsearchddlItem');
            ddl.on('click', '.mailtagsearchddlItem', onClickIntellisense);
            applySlimScroll(".mailtagsearchddl", ".mailtagsearchddlOverflowContainer");
        }
        else
            ddl.hide();
    }

    MailTag.prototype.create = function (object) {
        String.prototype.replaceAll = function (find, replace) {
            return this.replace(new RegExp(find, 'g'), replace);
        }
        if (validateObject(object)) {
            var html = '';
            var hiddenValue = '';
            html += '<input type="hidden" class="hdnTag" value="" />';

            if (isEmpty(mailtag.emailVsName)) {
                mailtag.emailVsName = object.mailsCollection;
            }

            if (mailtag.subscriptionInfoIntellisenseCollection.length === 0) {
                for (var key in mailtag.emailVsName) {
                    for (var item = 0; item < mailtag.emailVsName[key].length; item++) {
                        if (key != '')
                            mailtag.subscriptionInfoIntellisenseCollection.push(mailtag.emailVsName[key][item] + " <" + key + ">");
                    }
                }
            }

            ////Reversing the order for NameVsEmail     
            //if (isEmpty(mailtag.NameVsEmail)) {
            //    for (var key in object.mailsCollection) {
            //        mailtag.NameVsEmail[mailtag.emailVsName[key]] = key;
            //    }
            //}

            $.each(object.list, function (ii, ee) {
                for (var item = 0; item < mailtag.emailVsName[ee].length; item++) {
                    var titl = ee.replaceAll('"', '&quot;');
                    html += '<div class="mailtagContent"><div class="fa fa-times mailtagClose"></div><div class="mailtagText' + (ee.length > 25 ? ' OverflowText' : '') + '" title="' + titl + '">' + mailtag.emailVsName[ee][item] + '</div></div>';
                    hiddenValue += ee + ',';
                }
            });
            if (hiddenValue.length > 0)
                hiddenValue = hiddenValue.substr(0, hiddenValue.length - 1);

            html += '<div class="mailtagTextContainer"><input class="mailtagTextElement" type="text" style="border : none;"/></div><div class="mailtagsearchddl"></div>';
            //$('#' + object.container[0].id + ' .mailtagTextContainer').css('width', parseInt($('#successSubscribtionListContainer').css('width')) - parseInt($('#successSubscribtionListContainer .mailtagContent').css('width')) - 35);
            object.container.html(html);

            if (!object.container.hasClass('mailtagContainer'))
                object.container.addClass('mailtagContainer');

            object.container.find('.hdnTag').val(hiddenValue).trigger('change');
            var textele = object.container.find('.mailtagTextElement');
            textele.val('');
            mailtag.height = parseInt(textele.css('height'), 10);

            attachHandlers(object.container);

            if (object.hasOwnProperty('ready') && typeof object.ready === 'function')
                object.ready(object.container);
        }
    }

    MailTag.prototype.getIsModified = function getIsModified() {
        return mailtag.isModified;
    }

    var mailtag = new MailTag();
    return mailtag;
})();