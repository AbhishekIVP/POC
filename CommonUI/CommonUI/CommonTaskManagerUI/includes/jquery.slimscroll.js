/*! Copyright (c) 2011 Piotr Rochala (http://rocha.la)
* Dual licensed under the MIT (http://www.opensource.org/licenses/mit-license.php)
* and GPL (http://www.opensource.org/licenses/gpl-license.php) licenses.
*
* Version: 1.1.0
*
*/
(function ($) {

    jQuery.fn.extend({
        slimScroll: function (options) {

            var defaults = {

                // width in pixels of the visible scroll area
                width: 'auto',

                // height in pixels of the visible scroll area
                height: '250px',

                // width in pixels of the scrollbar and rail
                size: '6px',

                // scrollbar color, accepts any hex/color value
                color: '#000',

                // scrollbar position - left/right
                position: 'right',

                // distance in pixels between the side edge and the scrollbar
                distance: '1px',

                // default scroll position on load - top / bottom / $('selector')
                start: 'top',

                // sets scrollbar opacity
                opacity: .4,

                // enables always-on mode for the scrollbar
                alwaysVisible: false,

                // check if we should hide the scrollbar when user is hovering over
                disableFadeOut: false,

                // sets visibility of the rail
                railVisible: false,

                // sets rail color
                railColor: '#333',

                // sets rail opacity
                railOpacity: .2,

                // whether  we should use jQuery UI Draggable to enable bar dragging
                railDraggable: true,

                // defautlt CSS class of the slimscroll rail
                railClass: 'slimScrollRail',

                // defautlt CSS class of the slimscroll bar
                barClass: 'slimScrollBar',

                // defautlt CSS class of the slimscroll wrapper
                wrapperClass: 'slimScrollDiv',

                // check if mousewheel should scroll the window if we reach top/bottom
                allowPageScroll: false,

                // scroll amount applied to each mouse wheel step
                wheelStep: 2,

                // scroll amount applied when user is using gestures
                touchScrollStep: 200,

                xlgridid: null,

                controlclick: function () { },

                controlscroll: function () { },

                dragscroll: function () { },

                frozenbodyhover: function () { }


            };
            var o = $.extend(defaults, options);
            var scrollposition = 0;
            var barposition = 0;
            var positionscrolling = 0;
            var scrollfrozendiv = 0;
            var verticalscrolltop = 0


            var scrollvalues = new Array();

            // do it for every element that matches selector
            this.each(function () {

                var isOverPanel, isOverBar, isDragg, queueHide, touchDif,
        barHeight, percentScroll, lastScroll,
        divS = '<div></div>',
        minBarHeight = 30,
        releaseScroll = false;
                // used in event handlers and for better minification
                var me = $(this);
                var flag = true;

                // ensure we are not binding it again
                if (me.parent().hasClass(o.wrapperClass)) {
                    // start from last bar position
                    var offset = me.scrollTop();

                    // find bar and rail
                    bar = me.parent().find('.' + o.barClass);
                    rail = me.parent().find('.' + o.railClass);

                    getBarHeight();

                    // check if we should scroll existing instance
                    if ($.isPlainObject(options)) {
                        if ('scrollTo' in options) {
                            // jump to a static point
                            offset = parseInt(o.scrollTo);
                        }
                        else if ('scrollBy' in options) {
                            // jump by value pixels
                            offset += parseInt(o.scrollBy);
                        }
                        else if ('destroy' in options) {
                            // remove slimscroll elements
                            bar.remove();
                            rail.remove();
                            me.unwrap();
                            return;
                        }

                        // scroll content by the given offset
                        scrollContent(offset, false, true);
                    }

                    return;
                }

                // optionally set height to the parent's height
                o.height = (o.height == 'auto') ? me.parent().innerHeight() : o.height;

                // wrap content
                var wrapper = $(divS)
          .addClass(o.wrapperClass)
          .css({
              position: 'relative',
              overflow: 'hidden',
              width: o.width,
              height: o.height
          });

                // update style for the div
                me.css({
                    overflow: 'hidden',
                    width: o.width,
                    height: o.height
                });

                // create scrollbar rail
                var rail = $(divS)
          .addClass(o.railClass)
          .css({
              width: o.size,
              height: '100%',
              position: 'absolute',
              top: 0,
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
              background: o.color,
              width: o.size,
              position: 'absolute',
              top: 0,
              opacity: o.opacity,
              display: o.alwaysVisible ? 'block' : 'none',
              'border-radius': o.size,
              BorderRadius: o.size,
              MozBorderRadius: o.size,
              WebkitBorderRadius: o.size,
              zIndex: 99
          });

                // set position
                var posCss = (o.position == 'right') ? { right: o.distance} : { left: o.distance };
                rail.css(posCss);
                bar.css(posCss);

                // wrap it
                me.wrap(wrapper);

                // append to parent div
                me.parent().append(bar);
                me.parent().append(rail);

                // make it draggable
                if (o.railDraggable) {
                    bar.draggable({
                        axis: 'y',
                        scrollSensitivity : '1px',
                        containment: 'parent',
                        start: function () { isDragg = true; },
                        stop: function () {
                            isDragg = false; hideBar();
                            var scrollup = false;
                            var barpositiondrag = parseInt(bar.css('top')); //$(this).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id')).offset().top;
                            var scrollfrozendivdrag = me.scrollTop();
                            var verticalscrolltopdrag = parseInt(bar.css('top'));
                            if ((Math.round($(this).closest('.' + o.wrapperClass).find(".slimScrollRail").offset().top + $(this).closest('.' + o.wrapperClass).find(".slimScrollRail").height()) == Math.round($(this).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id')).height() + $(this).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id')).offset().top))) {
                                o.controlscroll.call(this, $(this).closest('.horizonslimScrollDiv').find(".horizonslimScrollBar")[0], $(this).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id'))[0], $(this).closest('.' + o.wrapperClass).find(".slimScrollRail")[0], scrollup, barpositiondrag, scrollfrozendivdrag, verticalscrolltopdrag);
                            }
                            else if ($(this).closest('.' + o.wrapperClass).find(".slimScrollRail").offset().top == $(this).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id')).offset().top) {
                                scrollup = true;
                                o.controlscroll.call(this, $(this).closest('.horizonslimScrollDiv').find(".horizonslimScrollBar")[0], $(this).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id'))[0], $(this).closest('.' + o.wrapperClass).find(".slimScrollRail")[0], scrollup, barpositiondrag, scrollfrozendivdrag, verticalscrolltopdrag);
                            }
                        },
                        drag: function (e, info) {
                            // scroll content
                            scrollContent(0, $(this).position().top, false, info.helper);
                            o.dragscroll.call(this);
                            //$("#xlGid_frozen_bodyDiv").scrollTop($("#xlGid_bodyDiv").scrollTop());

                            //                            var scrollup = false;
                            //                            var barpositiondrag = parseInt(bar.css('top')); //$(this).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id')).offset().top;
                            //                            var scrollfrozendivdrag = me.scrollTop();
                            //                            var verticalscrolltopdrag = parseInt(bar.css('top'));
                            //                            $("#test1").val($("#test").val() + " " + barpositiondrag);
                            //                            if ((Math.round($("#" + me.attr('id')).closest('.' + o.wrapperClass).find(".slimScrollRail").offset().top + $("#" + me.attr('id')).closest('.' + o.wrapperClass).find(".slimScrollRail").height()) == Math.round($("#" + me.attr('id')).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id')).height() + $("#" + me.attr('id')).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id')).offset().top))) {
                            //                                o.controlscroll.call(this, $("#" + me.attr('id')).closest('.horizonslimScrollDiv').find(".horizonslimScrollBar")[0], $("#" + me.attr('id')).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id'))[0], $("#" + me.attr('id')).closest('.' + o.wrapperClass).find(".slimScrollRail")[0], scrollup, barpositiondrag, scrollfrozendivdrag, verticalscrolltopdrag);
                            //                            }
                            //                            else if ($("#" + me.attr('id')).closest('.' + o.wrapperClass).find(".slimScrollRail").offset().top == $("#" + me.attr('id')).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id')).offset().top) {
                            //                                scrollup = true;
                            //                                o.controlscroll.call(this, $("#" + me.attr('id')).closest('.horizonslimScrollDiv').find(".horizonslimScrollBar")[0], $("#" + me.attr('id')).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id'))[0], $("#" + me.attr('id')).closest('.' + o.wrapperClass).find(".slimScrollRail")[0], scrollup, barpositiondrag, scrollfrozendivdrag, verticalscrolltopdrag);
                            //                            }
                        }
                    });
                }

                // on rail over
                rail.hover(function () {
                    showBar();
                    bar.css({ width: '12px' });
                    rail.css({ width: '12px' });
                }, function () {
                    hideBar();
                });

                // on bar over
                bar.hover(function () {
                    isOverBar = true;
                    showBar();
                    bar.css({ width: '12px' });
                    rail.css({ width: '12px' });
                }, function () {
                    isOverBar = false;
                    hideBar();
                });

                // show on parent mouseover
                me.hover(function () {
                    isOverPanel = true;
                    if (flag == true) {
                        showBar();
                    }
                    flag = true;
                    //hideBar();
                }, function () {
                    isOverPanel = false;
                    hideBar();
                });

                $("#" + o.xlgridid + "_frozen_bodyDiv").hover(function () {
                    isOverPanel = true;
                }, function () {
                    isOverPanel = false;
                });

                //o.frozenbodyhover.call(this);

                // support for mobile
                me.bind('touchstart', function (e, b) {
                    if (e.originalEvent.touches.length) {
                        // record where touch started
                        touchDif = e.originalEvent.touches[0].pageY;
                    }
                });

                me.bind('touchmove', function (e) {
                    // prevent scrolling the page
                    e.originalEvent.preventDefault();
                    if (e.originalEvent.touches.length) {
                        // see how far user swiped
                        var diff = (touchDif - e.originalEvent.touches[0].pageY) / o.touchScrollStep;
                        // scroll content
                        scrollContent(diff, true);
                    }
                });

                // check start position
                if (o.start === 'bottom') {
                    // scroll content to bottom
                    bar.css({ top: me.outerHeight() - bar.outerHeight() });
                    scrollContent(0, true);
                }
                else if (o.start !== 'top') {
                    // assume jQuery selector
                    scrollContent($(o.start).position().top, null, true);

                    // make sure bar stays hidden
                    if (!o.alwaysVisible) { bar.hide(); }
                }

                // attach scroll events
                attachWheel();





                // set up initial height
                getBarHeight();

                function _onWheel(e) {
                    // use mouse wheel only when mouse is over

                    if (!isOverPanel) { return; }


                    var gridid = me.attr('id').substring(0, me.attr('id').lastIndexOf('_'));
                    //var lastpageflag = false;
                    var scrollup = false;
                    //                    var ApparentRecordCount = $find(gridid).gridEngine._grid.footer._recordsCount;
                    //                    var pagesize = $find(gridid).gridEngine._grid._gridInfo$2.PageSize;
                    //                    if (($("#" + gridid + "_PageNumberPaging").val() == Math.ceil(ApparentRecordCount / pagesize)) && $("#" + gridid + "_PageNumberPaging").val() != 1) {
                    //                        lastpageflag = true;
                    //                    }

                    var delta = 0;
                    if (e.wheelDelta) { delta = -e.wheelDelta / 120; }
                    if (e.detail) { delta = e.detail / 3; }
                    if (delta < 0) { scrollup = true; }

                    //if ((($("#" + gridid + "_bodyDiv").find("table").height() < $("#" + gridid + "_bodyDiv").height()) && lastpageflag && scrollup) || ($("#" + gridid + "_bodyDiv").find("table").height() > $("#" + gridid + "_bodyDiv").height())) {
                    var target = e.target || e.srcTarget;
                    if ($(target).closest('.' + o.wrapperClass).find(me)) {
                        // scroll content
                        //if (!(($("#" + o.xlgridid + "_bodyDiv").find("table").height() < $("#" + o.xlgridid + "_bodyDiv").height()) && lastpageflag && scrollup)) {
                        scrollContent(delta, true, null, $(target).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id')));

                        //o.controlscroll.call(this, scrollvalues, scrollup, barposition, scrollfrozendiv, verticalscrolltop);
                        if ($(target).closest('.horizonslimScrollDiv').find(".horizonslimScrollBar").length > 0) {
                            o.controlscroll.call(this, $(target).closest('.horizonslimScrollDiv').find(".horizonslimScrollBar")[0], $(target).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id'))[0], $(target).closest('.' + o.wrapperClass).find(".slimScrollRail")[0], scrollup, barposition, scrollfrozendiv, verticalscrolltop);
                        }
                        //o.controlscroll.call(this, $(".horizonslimScrollBar")[0], $(".slimScrollBar")[0], $(".slimScrollRail")[0], scrollup, barposition, scrollfrozendiv, verticalscrolltop);
                        //                        if (o.xlgridid != null) {
                        //                            $find(gridid).footer._onWheel($(".horizonslimScrollBar")[0], $(".slimScrollBar")[0], $(".slimScrollRail")[0], scrollup, barposition, scrollfrozendiv, verticalscrolltop);
                        //                        }
                        //}
                    }
                    else {
                        var a = 1;
                    }


                    //                        if ((Math.round($(".slimScrollRail").offset().top + $(".slimScrollRail").height()) == Math.round($(".slimScrollBar").height() + $(".slimScrollBar").offset().top)) && !scrollup) {
                    //                            if ($find(gridid) && $find(gridid).gridEngine._grid.get_AjaxStart() == false && (($find(gridid).gridEngine._grid._gridInfo$2.PageSize + $find(gridid).gridEngine._grid.get_PreviousStartIndex()) < $find(gridid).gridEngine._grid.footer._recordsCount)) {
                    //                                $find(gridid).gridEngine._grid.set_ScrollDir(2);
                    //                                $find(gridid).gridEngine._grid.set_ScrollPosition(scrollposition);
                    //                                $find(gridid).gridEngine._grid.set_BarPosition(barposition);
                    //                                infinitescroll();
                    //                            }
                    //                        }

                    //                        positionscrolling = parseInt(bar.css('top'));
                    //                        if (positionscrolling == 0) {
                    //                            if ($find(gridid).gridEngine._grid.get_AjaxStart() == false && ($find(gridid).gridEngine._grid.get_PreviousStartIndex() > 0)) {
                    //                                $find(gridid).gridEngine._grid.set_AjaxStart(true);
                    //                                $find(gridid).gridEngine._grid.set_ScrollDir(1);
                    //                                infinitescroll();
                    //                            }
                    //                        }
                    // stop window scroll
                    if (e.preventDefault && !releaseScroll) { e.preventDefault(); }
                    if (!releaseScroll) { e.returnValue = false; }


                    //}
                }


                $("#" + me.attr('id').substring(0, me.attr('id').lastIndexOf('_'))).find(".slimScrollRail").click(function (e) {
                    var pos = 0;

                    var scrollvalues = new Array();

                    scrollvalues[0] = me.parent().find(".slimScrollBar" + me.attr('id')).position().top;
                    scrollvalues[1] = me.parent().find(".slimScrollBar" + me.attr('id')).parent().offset().top;
                    scrollvalues[2] = me.parent().find(".slimScrollBar" + me.attr('id')).height();
                    scrollvalues[3] = me.parent().find(".slimScrollRail").height();

                    o.controlclick.call(this, scrollvalues, e.pageY);
                    //o.controlclick.call(this, me.parent().find(".slimScrollBar")[0], me.parent().find(".slimScrollRail")[0], e.pageY);

                    //var radgridid = '';
                    //radgridid = me.attr('id').substring(0, me.attr('id').lastIndexOf('_'));
                    //if (o.xlgridid != null) {
                    //  $find(o.xlgridid).footer._verticalScrollClick($(".slimScrollBar")[0], $(".slimScrollRail")[0], e.pageY);
                    //}
                    if (e.pageY > (me.parent().find(".slimScrollBar" + me.attr('id')).position().top + me.parent().find(".slimScrollBar" + me.attr('id')).parent().offset().top)) {
                        pos = e.pageY - (me.parent().find(".slimScrollBar" + me.attr('id')).position().top + me.parent().find(".slimScrollBar" + me.attr('id')).parent().offset().top);
                        if (((e.pageY - me.parent().find(".slimScrollBar" + me.attr('id')).parent().offset().top) + me.parent().find(".slimScrollBar" + me.attr('id')).height()) > me.parent().find(".slimScrollRail").height()) {
                            //pos = $(".slimScrollRail").height() - $(".slimScrollBar").height();
                            pos = e.pageY - me.parent().parent().find(".horizonslimScrollBar").parent().offset().top;
                            //$("#" + o.xlgridid + "_bodyDiv").scrollTop((pos - $(".slimScrollBar").height()) * ($("#" + o.xlgridid + "_bodyDiv").find('table').height() / $(".slimScrollRail").height()));
                            me.scrollTop((pos - $(".slimScrollBar" + me.attr('id')).height()) * (me.find('table').height() / $(".slimScrollRail").height()));
                            me.parent().find(".slimScrollBar" + me.attr('id')).css({ 'top': (pos - me.parent().find(".slimScrollBar" + me.attr('id')).height()) });
                            //$(".slimScrollBar").css({ 'top': (pos - $(".slimScrollBar").height()) });
                            //$("#" + o.xlgridid + "_frozen_bodyDiv").scrollTop((pos - $(".slimScrollBar").height()) * ($("#" + o.xlgridid + "_bodyDiv").find('table').height() / $(".slimScrollRail").height()));
                        }
                        else {
                            me.parent().find(".slimScrollBar" + me.attr('id')).css({ 'top': (me.parent().find(".slimScrollBar" + me.attr('id')).position().top + (pos - me.parent().find(".slimScrollBar" + me.attr('id')).height())) + 'px' });
                            //$(".slimScrollBar").css({ 'top': ($(".slimScrollBar").position().top + (pos - $(".slimScrollBar").height())) + 'px' });
                            //$("#" + o.xlgridid + "_bodyDiv").scrollTop((pos - $(".slimScrollBar").height()) * ($("#" + o.xlgridid + "_bodyDiv").find('table').height() / $(".slimScrollRail").height()));
                            me.scrollTop((pos - $(".slimScrollBar" + me.attr('id')).height()) * (me.find('table').height() / $(".slimScrollRail").height()));
                            //$("#" + o.xlgridid + "_frozen_bodyDiv").scrollTop((pos - $(".slimScrollBar").height()) * ($("#" + o.xlgridid + "_bodyDiv").find('table').height() / $(".slimScrollRail").height()));
                        }
                    }
                    else {
                        pos = e.pageY - me.parent().find(".slimScrollBar" + me.attr('id')).parent().offset().top;
                        //$("#" + o.xlgridid + "_bodyDiv").scrollTop((pos) * ($("#" + o.xlgridid + "_bodyDiv").find('table').height() / $(".slimScrollRail").height()));
                        me.scrollTop((pos) * (me.find('table').height() / $(".slimScrollRail").height()));
                        me.parent().find(".slimScrollBar" + me.attr('id')).css({ 'top': (pos) });
                        //$(".slimScrollBar").css({ 'top': (pos) });
                        //$("#" + o.xlgridid + "_frozen_bodyDiv").scrollTop((pos) * ($("#" + o.xlgridid + "_bodyDiv").find('table').height() / $(".slimScrollRail").height()));
                    }
                });


                function scrollContent(y, isWheel, isJump, scrollbar) {
                    var delta = y;
                    var maxTop = me.outerHeight() - (scrollbar == null ? $(".slimScrollBar" + me.attr('id')).height() : scrollbar.height()); // bar.outerHeight();
                    //var maxTop = me.outerHeight() - $(".slimScrollBar").height(); // bar.outerHeight();
                    if (isWheel) {
                        // move bar with mouse wheel
                        delta = parseInt(bar.css('top')) + y * parseInt(o.wheelStep) / 100 * (scrollbar == null ? $(".slimScrollBar" + me.attr('id')).height() : scrollbar.height()); // bar.outerHeight();
                        //delta = parseInt(bar.css('top')) + y * parseInt(o.wheelStep) / 100 * $(".slimScrollBar").height(); // bar.outerHeight();

                        // move bar, make sure it doesn't go out
                        delta = Math.min(Math.max(delta, 0), maxTop);

                        // if scrolling down, make sure a fractional change to the
                        // scroll position isn't rounded away when the scrollbar's CSS is set
                        // this flooring of delta would happened automatically when
                        // bar.css is set below, but we floor here for clarity
                        delta = (y > 0) ? Math.ceil(delta) : Math.floor(delta);

                        // scroll the scrollbar
                        //bar.css({ top: delta + 'px' });
                        scrollbar == null ? bar.css({ top: delta + 'px' }) : scrollbar.css({ top: delta + 'px' });
                        //$("#Test").val($("#Test").val() + " - " +delta);
                        //barposition = delta - (0.1 * delta);
                        barposition = delta; //- (0.1 * delta);
                        verticalscrolltop = scrollbar == null ? parseInt(bar.css('top')) : parseInt(scrollbar.css('top'));
                    }

                    //                    $("#Test").val($("#Test").val() + scrollbar.height());
                    //                    $("#Test2").val($("#Test2").val() + $(".slimScrollBar").height());

                    // calculate actual scroll amount
                    percentScroll = parseInt(bar.css('top')) / (me.outerHeight() - (scrollbar == null ? $(".slimScrollBar" + me.attr('id')).height() : scrollbar.height())); //bar.outerHeight());
                    //percentScroll = parseInt(bar.css('top')) / (me.outerHeight() - $(".slimScrollBar").height()); //bar.outerHeight());
                    delta = percentScroll * (me[0].scrollHeight - me.outerHeight());

                    if (isJump) {
                        delta = y;
                        var offsetTop = delta / me[0].scrollHeight * me.outerHeight();
                        offsetTop = Math.min(Math.max(offsetTop, 0), maxTop);
                        bar.css({ top: offsetTop + 'px' });
                    }

                    // scroll content
                    scrollposition = delta - (0.1 * delta);
                    //                    if (delta >= 0) {
                    //                        $("#" + o.xlgridid + "_frozen_bodyDiv").scrollTop(delta);
                    //                    }
                    me.scrollTop(delta);
                    scrollfrozendiv = delta;
                    // fire scrolling event
                    me.trigger('slimscrolling', ~ ~delta);

                    // ensure bar is visible
                    showBar();

                    // trigger hide when scroll is stopped
                    hideBar();
                }

                function attachWheel() {
                    if (window.addEventListener) {
                        //                        this.addEventListener('DOMMouseScroll', _onWheel, false);
                        //                        this.addEventListener('mousewheel', _onWheel, false);
                        document.getElementById(me.attr('id')).addEventListener('DOMMouseScroll', _onWheel, false);
                        document.getElementById(me.attr('id')).addEventListener('mousewheel', _onWheel, false);
                        if (o.xlgridid != null) {
                            document.getElementById(o.xlgridid + "_frozen_bodyDiv").addEventListener('DOMMOuseScroll', _onWheel, false);
                            document.getElementById(o.xlgridid + "_frozen_bodyDiv").addEventListener('mousewheel', _onWheel, false);
                        }
                    }
                    else {
                        document.attachEvent("onmousewheel", _onWheel)
                    }
                }

                function getBarHeight() {
                    // calculate scrollbar height and make sure it is not too small
                    barHeight = Math.max((me.outerHeight() / me[0].scrollHeight) * me.outerHeight(), minBarHeight);
                    bar.css({ height: barHeight + 'px' });
                    // hide scrollbar if content is not long enough
                    var display = barHeight == me.outerHeight() ? 'none' : 'block';
                    bar.css({ display: display });
                    rail.css({ display: display });
                }

                function infinitescroll() {
                    if ($find(o.xlgridid)) {
                        $find(o.xlgridid).gridEngine._grid.set_HorizontalScrollPosition(parseInt($(".horizonslimScrollBar").css('left')));
                        $find(o.xlgridid).gridEngine._grid.set_AjaxStart(true);
                        $find(o.xlgridid).gridEngine._grid.set_InfiniteScroll(true);
                        $find(o.xlgridid).footer._doPaging();
                    }
                }

                function showBar() {
                    // recalculate bar height
                    getBarHeight();
                    clearTimeout(queueHide);

                    // when bar reached top or bottom
                    if (percentScroll == ~ ~percentScroll) {
                        //release wheel
                        releaseScroll = o.allowPageScroll;

                        // publish approporiate event
                        if (lastScroll != percentScroll) {
                            var msg = (~ ~percentScroll == 0) ? 'top' : 'bottom';
                            me.trigger('slimscroll', msg);
                        }
                    }
                    lastScroll = percentScroll;
                    // show only when required
                    if (barHeight >= me.outerHeight()) {
                        //allow window scroll
                        releaseScroll = true;
                        return;
                    }
                    bar.stop(true, true).fadeIn('fast');
                    if (o.railVisible) { rail.stop(true, true).fadeIn('fast'); }
                }

                function hideBar() {
                    // only hide when options allow it
                    flag = false;
                    if (!o.alwaysVisible) {
                        queueHide = setTimeout(function () {
                            if (!(o.disableFadeOut && isOverPanel) && !isOverBar && !isDragg) {
                                bar.css({ width: o.size });
                                rail.css({ width: o.size });
                            }
                        }, 1);
                    }
                }

            });

            // maintain chainability
            return this;
        }
    });

    jQuery.fn.extend({
        slimscroll: jQuery.fn.slimScroll
    });

})(jQuery);
