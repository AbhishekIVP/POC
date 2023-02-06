//(function ($) {
//    'use strict';
//
//    var caretClass = 'textarea-helper-caret'
//    , dataKey = 'textarea-helper'
//
//    // Styles that could influence size of the mirrored element.
//    , mirrorStyles = [
//    // Box Styles.
//                       'box-sizing', 'height', 'width', 'padding-bottom'
//                     , 'padding-left', 'padding-right', 'padding-top'
//
//    // Font stuff.
//                     , 'font-family', 'font-size', 'font-style'
//                     , 'font-variant', 'font-weight'
//
//    // Spacing etc.
//                     , 'word-spacing', 'letter-spacing', 'line-height'
//                     , 'text-decoration', 'text-indent', 'text-transform'
//
//    // The direction.
//                     , 'direction'
//                     ];
//
//    var TextareaHelper = function (elem) {
//        if (elem.nodeName.toLowerCase() !== 'textarea') return;
//        this.$text = $(elem);
//        this.$mirror = $('<div/>').css({ 'position': 'absolute'
//                                   , 'overflow': 'auto'
//                                   , 'white-space': 'pre-wrap'
//                                   , 'word-wrap': 'break-word'
//                                   , 'top': 450
//                                   , 'left': -9999
//        }).insertAfter(this.$text);
//    };
//
//    (function () {
//        this.update = function () {
//
//            // Copy styles.
//            var styles = {};
//            for (var i = 0, style; style = mirrorStyles[i]; i++) {
//                styles[style] = this.$text.css(style);
//            }
//            this.$mirror.css(styles).empty();
//
//            // Update content and insert caret.
//            var caretPos = this.getOriginalCaretPos()
//        , str = this.$text.val()
//        , pre = document.createTextNode(str.substring(0, caretPos))
//        , post = document.createTextNode(str.substring(caretPos))
//        , $car = $('<span/>').addClass(caretClass).css('position', 'absolute').html('&nbsp;');
//            this.$mirror.append(pre, $car, post)
//                  .scrollTop(this.$text.scrollTop());
//        };
//
//        this.destroy = function () {
//            this.$mirror.remove();
//            this.$text.removeData(dataKey);
//            return null;
//        };
//
//        this.caretPos = function () {
//            this.update();
//            var $caret = this.$mirror.find('.' + caretClass)
//        , pos = $caret.position();
//           // pos.top = pos.top + $caret.height() ;
//            if (this.$text.css('direction') === 'rtl') {
//                pos.right = this.$mirror.innerWidth() - pos.left - $caret.width();
//                pos.left = 'auto';
//            }
//
//            return pos;
//        };
//
//        this.height = function () {
//            this.update();
//            this.$mirror.css('height', '');
//            return this.$mirror.height();
//        };
//
//        // XBrowser caret position
//        // Adapted from http://stackoverflow.com/questions/263743/how-to-get-caret-position-in-textarea
//        this.getOriginalCaretPos = function () {
//            var text = this.$text[0];
//            if (text.selectionStart) {
//                return text.selectionStart;
//            } else if (document.selection) {
//                text.focus();
//                var r = document.selection.createRange();
//                if (r == null) {
//                    return 0;
//                }
//                var re = text.createTextRange()
//          , rc = re.duplicate();
//                re.moveToBookmark(r.getBookmark());
//                rc.setEndPoint('EndToStart', re);
//                return rc.text.length;
//            }
//            return 0;
//        };
//
//    }).call(TextareaHelper.prototype);
//
//    $.fn.textareaHelper = function (method) {
//        this.each(function () {
//            var $this = $(this)
//        , instance = $this.data(dataKey);
//            if (!instance) {
//                instance = new TextareaHelper(this);
//                $this.data(dataKey, instance);
//            }
//        });
//        if (method) {
//            var instance = this.first().data(dataKey);
//            return instance[method]();
//        } else {
//            return this;
//        }
//    };
//
//})(jQuery);

/**
* jQuery plugin for getting position of cursor in textarea
 
* @license under Apache license
* @author Bevis Zhao (i@bevis.me, http://bevis.me)
*/

(function ($, window, document, undefined) {
    $(function () {
        var CUSTOM_TOP_OFFSET = 0; // 55;
        var CUSTOM_LEFT_OFFSET = 0;
        var calculator = {
            // key styles
            primaryStyles: ['fontFamily', 'fontSize', 'fontWeight', 'fontVariant', 'fontStyle',
                'paddingLeft', 'paddingTop', 'paddingBottom', 'paddingRight',
                'marginLeft', 'marginTop', 'marginBottom', 'marginRight',
                'borderLeftColor', 'borderTopColor', 'borderBottomColor', 'borderRightColor',
                'borderLeftStyle', 'borderTopStyle', 'borderBottomStyle', 'borderRightStyle',
                'borderLeftWidth', 'borderTopWidth', 'borderBottomWidth', 'borderRightWidth',
                'line-height', 'outline'],
            specificStyle: {
                'word-wrap': 'break-word',
                'overflow-x': 'hidden',
                'overflow-y': 'auto'
            },
            simulator: $('<div id="textarea_simulator" contenteditable="true"/>').css({
                position: 'absolute',
                top: 0,
                left: 0,
                visibility: 'hidden'
            }).appendTo(document.body),
            toHtml: function (text) {
                return text.replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/\n/g, '<br>')
                        .replace(/(\s)/g, '<span style="white-space:pre-wrap;">$1</span>');
            },
            // calculate position
            getCaretPosition: function () {
                var cal = calculator, self = this, element = self[0], elementOffset = self.offset();

                // IE has easy way to get caret offset position
                //if ($.browser.msie) {
                if (navigator.userAgent.match(/msie/i)) {
                    // must get focus first
                    element.focus();
                    var range = document.selection.createRange();
                    return {
                        left: range.boundingLeft - elementOffset.left,
                        top: parseInt(range.boundingTop) - elementOffset.top + element.scrollTop
                                + document.documentElement.scrollTop + parseInt(self.getComputedStyle("fontSize"))
                    };
                }
                cal.simulator.empty();
                // clone primary styles to imitate textarea
                $.each(cal.primaryStyles, function (index, styleName) {
                    self.cloneStyle(cal.simulator, styleName);
                });

                // caculate width and height
                cal.simulator.css($.extend({
                    'width': self.width(),
                    'height': self.height()
                }, cal.specificStyle));

                var value = self.val(), cursorPosition = self.getCursorPosition();
                var beforeText = value.substring(0, cursorPosition),
                        afterText = value.substring(cursorPosition);

                var before = $('<span class="before"/>').html(cal.toHtml(beforeText)),
                        focus = $('<span class="focus"/>'),
                        after = $('<span class="after"/>').html(cal.toHtml(afterText));

                cal.simulator.append(before).append(focus).append(after);
                var focusOffset = focus.offset(), simulatorOffset = cal.simulator.offset();
                // alert(focusOffset.left  + ',' +  simulatorOffset.left + ',' + element.scrollLeft);
                return {
                    top: focusOffset.top - simulatorOffset.top - element.scrollTop + CUSTOM_TOP_OFFSET
                    // calculate and add the font height except Firefox
                    //+ ($.browser.mozilla ? 0 : parseInt(self.getComputedStyle("fontSize"))),
                            + (parseInt(self.getComputedStyle("fontSize"))),
                    left: focus[0].offsetLeft - cal.simulator[0].offsetLeft - element.scrollLeft + CUSTOM_LEFT_OFFSET
                };
            }
        };

        $.fn.extend({
            getComputedStyle: function (styleName) {
                if (this.length == 0)
                    return;
                var thiz = this[0];
                var result = this.css(styleName);
                if (navigator.userAgent.match(/Trident.*rv\:11\./) != null) {
                    $.browser = {};
                    $.browser.msie = "MSIE";
                }
                result = result || ($.browser.msie ?
                        thiz.currentStyle[styleName] :
                        document.defaultView.getComputedStyle(thiz, null)[styleName]);
                return result;
            },
            // easy clone method
            cloneStyle: function (target, styleName) {
                var styleVal = this.getComputedStyle(styleName);
                if (!!styleVal) {
                    $(target).css(styleName, styleVal);
                }
            },
            cloneAllStyle: function (target, style) {
                var thiz = this[0];
                for (var styleName in thiz.style) {
                    var val = thiz.style[styleName];
                    typeof val == 'string' || typeof val == 'number'
                            ? this.cloneStyle(target, styleName)
                            : NaN;
                }
            },
            getCursorPosition: function () {
                var thiz = this[0], result = 0;
                if ('selectionStart' in thiz) {
                    result = thiz.selectionStart;
                } else if ('selection' in document) {
                    var range = document.selection.createRange();
                    if (parseInt($.browser.version) > 6) {
                        thiz.focus();
                        var length = document.selection.createRange().text.length;
                        range.moveStart('character', -thiz.value.length);
                        result = range.text.length - length;
                    } else {
                        var bodyRange = document.body.createTextRange();
                        bodyRange.moveToElementText(thiz);
                        for (; bodyRange.compareEndPoints("StartToStart", range) < 0; result++)
                            bodyRange.moveStart('character', 1);
                        for (var i = 0; i <= result; i++) {
                            if (thiz.value.charAt(i) == '\n')
                                result++;
                        }
                        var enterCount = thiz.value.split('\n').length - 1;
                        result -= enterCount;
                        return result;
                    }
                }
                return result;
            },
            getCaretPosition: calculator.getCaretPosition
        });
    });

})(jQuery, window, document);



