var tag = (function () {
    function Tag() {
        this.totalWidth = 0;
        this.isModified = false;
        this.intervalid = 0;

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
        $object.find('.tagTextElement').unbind('keyup').keyup(function (e) {
            e.stopPropagation();
            var $this = $(this);
            var list = [];
            var grandParent = $this.parent().parent();
            var value = $this.val();

            //            if (value.length < 10) {
            //                $this[0].style.height = '1px';
            //                $this[0].style.height = (25 + $this[0].scrollHeight) + 'px';
            //            }
            //            else
            //                $this[0].style.height = tag.height + 'px';

            if (e.which === 13) {
                var valuee = $this.val().replaceAll('\r\n', '').replaceAll('\n', '');
                if (valuee !== '') {
                    var ele = grandParent.find('.hdnTag');
                    if (ele.val().length > 0) {
                        list = ele.val().split(',');
                    }
                    list.push(valuee);

                    var tempo = {};
                    $.each(list, function (ii, ee) {
                        tempo[ee] = '';
                    });

                    var obj = {};
                    obj.container = grandParent;
                    obj.list = Object.keys(tempo);
                    tag.create(obj);
                    tag.isModified = true;
                }
            }
            else {
                if (tag.intervalid !== 0) {
                    clearTimeout(tag.intervalid);
                }
                tag.intervalid = setTimeout(function () {
                    tag.searchAutocomplete(value, 20, $object);
                }, 100);
            }
        });

        $object.find('.tagTextElement').unbind('keydown').keydown(function (e) {
            e.stopPropagation();
            var $this = $(this);

            var value = $this.val();
        });

        $object.find('.tagClose').unbind('click').click(function (e) {
            e.stopPropagation();
            var $this = $(this);
            var parent = $this.parent();
            var text = parent.find('.tagText').html();

            var ele = $this.parents('.tagContainer').find('.hdnTag');
            var arr = ele.val().split(',');

            var hiddenValue = '';
            $.each(arr, function (ii, ee) {
                if (ee !== text)
                    hiddenValue += ee + ',';
            });
            hiddenValue = hiddenValue.substr(0, hiddenValue.length - 1);

            ele.val(hiddenValue);
            parent.remove();
            tag.isModified = true;
        });

        $(document).click(function (e) {
            e.stopPropagation();
            $('.tagsearchddl').hide();
        });
    }

    function intellisenseHTML(data) {
        var html = '';
        //        data.push('ashdgkajsd');
        //        data.push('ashdgkajsd');
        //        data.push('ashdgkajsd');
        //        data.push('ashdgkajsd');
        //        data.push('ashdgkajsd');
        //        data.push('ashdgkajsd');
        //        data.push('ashdgkajsd');
        //        data.push('ashdgkajsd');
        //        data.push('ashdgkajsd');
        //        data.push('ashdgkajsd');
        //        data.push('ashdgkajsd');
        //        data.push('ashdgkajsd');
        for (var c = 0; c < data.length; c++) {
            html += '<div class="tagsearchddlItem">' + data[c] + '</div>';
        }
        return html;
    }

    function applySlimScroll(containerSelector, bodySelector) {
        var scrollBodyContainer = $(containerSelector);
        var scrollBody = scrollBodyContainer.find(bodySelector);

        if (scrollBodyContainer.height() < scrollBody.height()) {
            scrollBody.smslimscroll({ height: scrollBodyContainer.height() + 'px', alwaysVisible: true, position: 'right', distance: '2px' });
        }
    }

    Tag.prototype.searchAutocomplete = function searchAutocomplete(prefixText, count, $object) {
        var args = { count: count, "prefix": prefixText };

        var onClickIntellisense = function (e) {
            e.stopPropagation();
            var target = $(e.target);
            var ddlDiv = target.parent().parent();
            var valu = target.text().trim();
            if (ddlDiv.prop('class') === 'smslimScrollDiv')
                ddlDiv = ddlDiv.parent();
            ddlDiv.hide();
            var txtBox = ddlDiv.parent().find('.tagTextElement');
            txtBox.val(valu);
            var evnt = $.Event('keyup');
            evnt.which = 13;
            txtBox.trigger(evnt);
        }

        var onSuccess = function (e) {
            if (e.d.length > 0) {
                var ddl = $object.find('.tagsearchddl');
                ddl.html("<div class='tagsearchddlOverflowContainer'>" + intellisenseHTML(e.d) + "</div>");
                ddl.show();
                var children = ddl.children();
                children.eq(0).css('padding-top', '4px');
                ddl.off('click', '.tagsearchddlItem');
                ddl.on('click', '.tagsearchddlItem', onClickIntellisense);
                applySlimScroll(".tagsearchddl", ".tagsearchddlOverflowContainer");
            }
            else
                $('.tagsearchddl').hide();
        }

        if (prefixText.length >= 2) {
            $.ajax({
                data: JSON.stringify(args),
                type: "POST",
                url: tag.path + '/BaseUserControls/Service/Service.asmx/GetTagIntellisense',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: onSuccess
            });
        }
        else
            $('.tagsearchddl').hide();
    }

    Tag.prototype.create = function (object) {
        String.prototype.replaceAll = function (find, replace) {
            return this.replace(new RegExp(find, 'g'), replace);
        }
        if (validateObject(object)) {
            var html = '';
            var hiddenValue = '';
            html += '<input type="hidden" class="hdnTag" value="" />';

            $.each(object.list, function (ii, ee) {
                var titl = ee.replaceAll('"', '&quot;');
                html += '<div class="tagContent"><div class="fa fa-times tagClose"></div><div class="tagText' + (ee.length > 25 ? ' OverflowText' : '') + '" title="' + titl + '">' + ee + '</div></div>';
                hiddenValue += ee + ',';
            });
            if (hiddenValue.length > 0)
                hiddenValue = hiddenValue.substr(0, hiddenValue.length - 1);

            html += '<div class="tagTextContainer"><textarea class="tagTextElement"></textarea></div><div class="tagsearchddl"></div>';
            object.container.html(html);

            object.container.find('.hdnTag').val(hiddenValue);
            var textele = object.container.find('.tagTextElement');
            textele.val('');
            tag.height = parseInt(textele.css('height'), 10);

            attachHandlers(object.container);

            if (object.hasOwnProperty('ready') && typeof object.ready === 'function')
                object.ready(object.container);
        }
    }

    Tag.prototype.getIsModified = function getIsModified() {
        return tag.isModified;
    }

    var tag = new Tag();
    return tag;
})();