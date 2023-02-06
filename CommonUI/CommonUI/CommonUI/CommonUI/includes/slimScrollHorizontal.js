/*! Copyright (c) 2011 Piotr Rochala (http://rocha.la)
* Dual licensed under the MIT (http://www.opensource.org/licenses/mit-license.php)
* and GPL (http://www.opensource.org/licenses/gpl-license.php) licenses.
*
* Version: 0.6.5
* 
*/
(function ($) {

    jQuery.fn.extend({
        slimScrollHorizontal: function (options) {

            var defaults = {
                wheelStep: 2,
                height: 'auto',
                width: 'auto',
                size: '4px',
                color: '#000',
                position: 'bottom',
                distance: '1px',
                start: 'left',
                opacity: .4,
                alwaysVisible: false,
                disableFadeOut: false,
                railVisible: false,
                railColor: '#333',
                railOpacity: '0.2',
                railClass: 'horizonslimScrollRail',
                //railClass: '$(".horizonslimScrollBar")',
                barClass: 'horizonslimScrollBar',
                wrapperClass: 'horizonslimScrollDiv',
                allowPageScroll: false,
                scroll: 0,
                touchScrollStep: 200,
                horizonxlgridid: null,
                scrollclick: function () { },
                leftposition: 0,
                draghorizonscroll: function () { },
                scrollContentLeftPos: 0
            };

            var o = $.extend(defaults, options);

            // do it for every element that matches selector
            this.each(function () {
                var version = checkVersion();
                var isOverPanel, isOverBar, isDragg, queueHide, touchDif,
        barWidth, percentScroll, lastScroll,
        divS = '<div></div>',
        minBarWidth = 30,
        releaseScroll = false;


                // used in event handlers and for better minification
                var me = $(this);
                var wid = me.css('width');
                //ensure we are not binding it again
                if (me.parent().parent().hasClass('horizonslimScrollDiv' + me.attr('id'))) {
                    //check if we should scroll existing instance
                    if (scroll) {

                        $("#" + o.horizonxlgridid + "_headerDiv").css({ width: me[0].style.width != "" ? me[0].style.width : o.width });
                        me.css({ width: me[0].style.width != "" ? me[0].style.width : o.width });
                        me.parent().parent().css({ width: me[0].style.width != "" ? me[0].style.width : o.width });
                        $("#" + o.horizonxlgridid + "_frozen_bodyDiv").css({ height: o.height });
                        me.css({ height: o.height })
                        me.parent().parent().css({ height: o.height })
                        //find bar and rail
                        //me.parent().parent().css('height', me.css('height'));
                        bar = me.parent().parent().find('.horizonslimScrollBar' + me.attr('id'));
                        rail = me.parent().parent().find('.' + o.railClass + me.attr('id'));
                        bar.css({ 'left': o.leftposition + "px" });

                        //scroll by given amount of pixels
                        //                        if ($find(o.horizonxlgridid).get_GridInfo().RequireFullScreen == false) {
                        scrollContent(me.scrollLeft() + (isNaN(parseInt(scroll)) ? 0 : parseInt(scroll)), false, true);
                        //}
                        //me.scrollLeft(o.scrollContentLeftPos);
                    }

                    return;
                }

                // wrap content
                var wrapper = $(divS)
          .addClass(o.wrapperClass + me.attr('id'))
          .css({
              position: 'relative',
              overflow: 'hidden',
              //              width: wid,//o.width,
              //width: o.width,
              width: me[0].style.width != "" ? me[0].style.width : o.width,
              //width:  parseInt(me.children()[0].style.width) > parseInt(me[0].style.width) ? o.width : wid,
              height: o.height
          });

                // update style for the div
                me.css({
                    overflow: 'hidden',
                    //width: o.width,
                    width: me[0].style.width != "" ? me[0].style.width : o.width,
                    //width: parseInt(me.children()[0].style.width) > parseInt(me[0].style.width) ? o.width : wid,
                    height: o.height
                });

                $("#" + o.horizonxlgridid + "_headerDiv").css({ width: me[0].style.width != "" ? me[0].style.width : o.width });
                // create scrollbar rail
                var rail = $(divS)
          .addClass(o.railClass + me.attr('id'))
          .css({
              width: '100%',
              height: o.size,
              position: 'absolute',
              bottom: 0,
              display: (o.alwaysVisible && o.railVisible) ? 'block' : 'none',
              'border-radius': o.size,
              background: o.railColor,
              opacity: o.railOpacity,
              zIndex: 90
          });

                // create scrollbar
                var bar = $(divS)
          .addClass(o.barClass + me.attr('id'))
          .css({
              left: o.leftposition + "px",
              background: o.color,
              height: o.size,
              position: 'absolute',
              bottom: 0,
              opacity: o.opacity,
              display: o.alwaysVisible ? 'block' : 'none',
              'border-radius': o.size,
              BorderRadius: o.size,
              MozBorderRadius: o.size,
              WebkitBorderRadius: o.size,
              zIndex: 99
          });

                // set position
                var posCss = (o.position == 'top') ? { top: o.distance} : { bottom: o.distance };
                rail.css(posCss);
                bar.css(posCss);

                // wrap it
                me.wrap(wrapper);

                // append to parent div
                me.parent().append(bar);
                me.parent().append(rail);

                // make it draggable
                bar.draggable({
                    axis: 'x',
                    containment: 'parent',
                    start: function () { isDragg = true; },
                    stop: function () {
                        isDragg = false;
                        $find(o.horizonxlgridid).gridEngine._grid.set_HorizontalScrollPosition(parseInt(me.parent().parent().find(".horizonslimScrollBar" + me.attr('id')).css('left')));
                        $find(o.horizonxlgridid).gridEngine._grid.set_InnerBodyPosition(parseInt(me.scrollLeft()));
                        $find(o.horizonxlgridid).gridEngine._grid.set_HeaderDivPosition(parseInt(me.scrollLeft()));
                        hideBar();
                    },
                    drag: function (e) {
                        // scroll content
                        scrollContent(0, $(this).position().left, false);
                        o.draghorizonscroll.call(this);
                    }
                });

                // on rail over
                rail.hover(function () {
                    showBar();
                    bar.css({ height: '8px' });
                    rail.css({ height: '8px' });
                }, function () {
                    hideBar();
                });

                // on bar over
                bar.hover(function () {
                    isOverBar = true;
                    showBar();
                    bar.css({ height: '8px' });
                    rail.css({ height: '8px' });
                }, function () {
                    isOverBar = false;
                    hideBar();
                });

                // show on parent mouseover
                me.hover(function () {
                    //isOverPanel = true;
                    showBar();
                    //hideBar();
                }, function () {
                    //isOverPanel = false;
                    hideBar();
                });

                // support for mobile
                me.bind('touchstart', function (e, b) {
                    if (e.originalEvent.touches.length) {
                        // record where touch started
                        touchDif = e.originalEvent.touches[0].pageX;
                    }
                });

                me.bind('touchmove', function (e) {
                    // prevent scrolling the page
                    e.originalEvent.preventDefault();
                    if (e.originalEvent.touches.length) {
                        // see how far user swiped
                        var diff = (touchDif - e.originalEvent.touches[0].pageX) / o.touchScrollStep;
                        // scroll content
                        scrollContent(diff, true);
                    }
                });

                var _onWheel = function (e) {
                    // use mouse wheel only when mouse is over
                    if (!isOverPanel) { return; }

                    var e = e || window.event;

                    var delta = 0;
                    if (e.wheelDelta) { delta = -e.wheelDelta / 120; }
                    if (e.detail) { delta = e.detail / 3; }

                    // scroll content
                    scrollContent(delta, true);

                    // stop window scroll
                    if (e.preventDefault && !releaseScroll) { e.preventDefault(); }
                    if (!releaseScroll) { e.returnValue = false; }
                }

                $("#" + me.attr('id').substring(0, me.attr('id').lastIndexOf('_'))).find(".horizonslimScrollRail" + me.attr('id')).click(function (e) {
                    var pos = 0;
                    o.scrollclick.call(this, me.parent().parent().find(".horizonslimScrollBar" + me.attr('id'))[0], me.parent().parent().find(".horizonslimScrollRail" + me.attr('id'))[0], e.pageX);
                    //                    if (o.horizonxlgridid != null) {
                    //                      $find(o.horizonxlgridid).footer._horizontalScrollClick($(".horizonslimScrollBar")[0], $(".horizonslimScrollRail")[0], e.pageX);
                    //                    }
                    if (e.pageX > (me.parent().parent().find(".horizonslimScrollBar" + me.attr('id')).position().left + me.parent().parent().find(".horizonslimScrollBar" + me.attr('id')).parent().offset().left)) {
                        pos = e.pageX - (me.parent().parent().find(".horizonslimScrollBar" + me.attr('id')).position().left + me.parent().parent().find(".horizonslimScrollBar" + me.attr('id')).parent().offset().left);
                        if (((e.pageX - me.parent().parent().find(".horizonslimScrollBar" + me.attr('id')).parent().offset().left) + me.parent().parent().find(".horizonslimScrollBar" + me.attr('id')).width()) > me.parent().parent().find(".horizonslimScrollRail" + me.attr('id')).width()) {
                            //pos = $(".horizonslimScrollRail").width() - $(".horizonslimScrollBar").width();
                            pos = e.pageX - me.parent().parent().find(".horizonslimScrollBar" + me.attr('id')).parent().offset().left;
                            //$("#" + o.horizonxlgridid + "_bodyDiv").scrollLeft((pos - $(".horizonslimScrollBar").width()) * ($("#" + o.horizonxlgridid + "_bodyDiv").find('table').width() / $(".horizonslimScrollRail").width()));
                            $("#" + o.horizonxlgridid + "_headerDiv").scrollLeft((pos - me.parent().parent().find(".horizonslimScrollBar" + me.attr('id')).width()) * (me.find('table').width() / me.parent().parent().find(".horizonslimScrollRail" + me.attr('id')).width()));
                          //  me.scrollLeft((pos - me.parent().parent().find(".horizonslimScrollBar" + me.attr('id')).width()) * (me.find('table').width() / me.parent().parent().find(".horizonslimScrollRail" + me.attr('id')).width()));
                            me.parent().parent().find(".horizonslimScrollBar" + me.attr('id')).css({ 'left': (pos - me.parent().parent().find(".horizonslimScrollBar" + me.attr('id')).width()) });
                            //$("#" + o.horizonxlgridid + "_headerDiv").scrollLeft((pos - $(".horizonslimScrollBar").width()) * ($("#" + o.horizonxlgridid + "_bodyDiv").find('table').width() / $(".horizonslimScrollRail").width()));
                        }
                        else {
                            me.parent().parent().find(".horizonslimScrollBar" + me.attr('id')).css({ 'left': (me.parent().parent().find(".horizonslimScrollBar" + me.attr('id')).position().left + (pos - me.parent().parent().find(".horizonslimScrollBar" + me.attr('id')).width())) + 'px' });
                            //$("#" + o.horizonxlgridid + "_headerDiv").scrollLeft((pos - $(".horizonslimScrollBar").width()) * ($("#" + o.horizonxlgridid + "_bodyDiv").find('table').width() / $(".horizonslimScrollRail").width()));
                            //$("#" + o.horizonxlgridid + "_bodyDiv").scrollLeft((pos - $(".horizonslimScrollBar").width()) * ($("#" + o.horizonxlgridid + "_bodyDiv").find('table').width() / $(".horizonslimScrollRail").width()));
                            //me.scrollLeft((pos - me.parent().parent().find(".horizonslimScrollBar").width()) * (me.find('table').width() / me.parent().parent().find(".horizonslimScrollRail").width()));
                            //$("#" + o.horizonxlgridid + "_headerDiv").scrollLeft((pos - $(".horizonslimScrollBar").width()) * ($("#" + o.horizonxlgridid + "_bodyDiv").find('table').width() / $(".horizonslimScrollRail").width()));
                        }
                    }
                    else {
                        pos = e.pageX - me.parent().parent().find(".horizonslimScrollBar" + me.attr('id')).parent().offset().left;
                        //$("#" + o.horizonxlgridid + "_bodyDiv").scrollLeft(pos * ($("#" + o.horizonxlgridid + "_bodyDiv").find('table').width() / $(".horizonslimScrollRail").width()));
                        $("#" + o.horizonxlgridid + "_headerDiv").scrollLeft(pos * (me.find('table').width() / me.parent().parent().find(".horizonslimScrollRail" + me.attr('id')).width()));
                        //me.scrollLeft(pos * (me.find('table').width() / me.parent().parent().find(".horizonslimScrollRail" + me.attr('id')).width()));
                        me.parent().parent().find(".horizonslimScrollBar" + me.attr('id')).css({ 'left': pos });
                        //$("#" + o.horizonxlgridid + "_headerDiv").scrollLeft(pos * ($("#" + o.horizonxlgridid + "_bodyDiv").find('table').width() / $(".horizonslimScrollRail").width()));
                    }
                    //$find(o.horizonxlgridid).gridEngine._grid.set_HorizontalScrollPosition(parseInt($(".horizonslimScrollBar").css('left')));
                });


                function getInternetExplorerVersion()
                // Returns the version of Internet Explorer or a -1
                // (indicating the use of another browser).
                {
                    var rv = -1; // Return value assumes failure.
                    if (navigator.appName == 'Microsoft Internet Explorer') {
                        var ua = navigator.userAgent;
                        var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
                        if (re.exec(ua) != null)
                            rv = parseFloat(RegExp.$1);
                    }
                    return rv;
                }
                function checkVersion() {
                    var msg = "You're not using Internet Explorer.";
                    var ver = getInternetExplorerVersion();

                    if (ver > -1) {
                        if (ver >= 8.0)
                            msg = "You're using a recent copy of Internet Explorer."
                        else
                            msg = "You should upgrade your copy of Internet Explorer.";
                    }
                    return ver;
                    //alert(msg);
                }

                function scrollContent(x, isWheel, isJump) {
                    var delta = x;

                    if (bar.css('left') == 'auto') {
                        bar.css('left', '0px');
                    }
                    if (isWheel) {
                        // move bar with mouse wheel
                        delta = parseInt(bar.css('left')) + x * parseInt(o.wheelStep) / 100 * bar.outerWidth();

                        // move bar, make sure it doesn't go out
                        var maxLeft = me.outerWidth() - bar.outerWidth();
                        delta = Math.min(Math.max(delta, 0), maxLeft);

                        // scroll the scrollbar
                        bar.css({ left: delta + 'px' });
                    }

                    // calculate actual scroll amount
                    percentScroll = parseInt(bar.css('left')) / (me.outerWidth() - bar.outerWidth());
                    delta = percentScroll * (me[0].scrollWidth - me.outerWidth());

                    if (isJump) {
                        delta = x;
                        var offsetLeft = delta / me[0].scrollWidth * me.outerWidth();
                        bar.css({ left: offsetLeft + 'px' });
                    }

                    // scroll content
                    me.scrollLeft(delta);
                    if (me.find(".xlMathematialFooterBody").length > 0) {
                        if ((delta - (me.find(".xlMathematialFooterBody").parent().parent().position().left)) == 0) {
                            //$("#text").val(me.position().left);
                            me.find(".xlMathematialFooterBody").css('left', (me.position().left));
                        }
                        else {
                            me.find(".xlMathematialFooterBody").css('left', (-1) * (delta - (me.position().left)));
                            //$("#text1").val((-1) * (delta - (me.position().left)));
                        }
                    }

                    //                    if ((delta - (me.find(".xlMathematialFooterBody").parent().parent().position().left)) == 0) {
                    //                        $("#text").val(me.find(".xlMathematialFooterBody").parent().parent().position().left);
                    //                        me.find(".xlMathematialFooterBody").css('left', (me.find(".xlMathematialFooterBody").parent().parent().position().left));
                    //                    }
                    //                    else {
                    //                        me.find(".xlMathematialFooterBody").css('left', (-1) * (delta - (me.find(".xlMathematialFooterBody").parent().parent().position().left)));
                    //                        $("#text1").val((-1) * (delta - (me.find(".xlMathematialFooterBody").parent().parent().position().left)));
                    //                    }

                    //alert($("#xlGid_headerDiv").attr('id'));
                    //                    if (o.horizonxlgridid != null) {
                    //                        $("#" + o.horizonxlgridid + "_headerDiv").scrollLeft(delta);
                    //                    }
                    // ensure bar is visible
                    showBar();

                    // trigger hide when scroll is stopped
                    hideBar();
                }

                var attachWheel = function () {
                    if (window.addEventListener) {
                        this.addEventListener('DOMMouseScroll', _onWheel, false);
                        this.addEventListener('mousewheel', _onWheel, false);
                    }
                    else {
                        document.attachEvent("onmousewheel", _onWheel)
                    }
                }

                // attach scroll events
                attachWheel();
                function getBarWidth() {
                    setTimeout(getwidth, 10);
                    // calculate scrollbar width and make sure it is not too small
                }

                function getwidth() {
                    barWidth = Math.max((me.outerWidth() / me[0].scrollWidth) * me.outerWidth(), minBarWidth);
                    bar.css({ width: barWidth + 'px' });

                    var display = barWidth == me.outerWidth() ? 'none' : 'block';
                    bar.css({ display: display });
                    rail.css({ display: display });

                }
                // set up initial width
                getBarWidth();

                function showBar() {
                    // recalculate bar width
                    getBarWidth();
                    clearTimeout(queueHide);

                    // when bar reached left or right
                    if (percentScroll == ~ ~percentScroll) {
                        //release wheel 
                        releaseScroll = o.allowPageScroll;

                        // publish approporiate event
                        if (lastScroll != percentScroll) {
                            var msg = (~ ~percentScroll == 0) ? 'left' : 'right';
                            me.trigger('slimscroll', msg);
                        }
                    }
                    lastScroll = percentScroll;

                    // show only when required
                    if (barWidth >= me.outerWidth()) {
                        //allow window scroll
                        releaseScroll = true;
                        return;
                    }
                    bar.stop(true, true).fadeIn('fast');
                    if (o.railVisible) { rail.stop(true, true).fadeIn('fast'); }
                }

                function hideBar() {
                    // only hide when options allow it
                    if (!o.alwaysVisible) {
                        queueHide = setTimeout(function () {
                            if (!(o.disableFadeOut && isOverPanel) && !isOverBar && !isDragg) {
                                bar.css({ height: o.size });
                                rail.css({ height: o.size });
                                bar.fadeOut('slow');
                                rail.fadeOut('slow');
                            }
                        }, 1);
                    }
                }

                // check start position
                if (o.start == 'right') {
                    // scroll content to right
                    bar.css({ left: me.outerWidth() - bar.outerWidth() });
                    scrollContent(0, true);
                }
                else if (typeof o.start == 'object') {
                    // scroll content
                    scrollContent($(o.start).position().left, null, true);

                    // make sure bar stays hidden
                    if (!o.alwaysVisible) { bar.hide(); }
                }
            });

            // maintain chainability
            return this;
        }
    });

    jQuery.fn.extend({
        slimscrollHorizontal: jQuery.fn.slimScrollHorizontal
    });

})(jQuery);