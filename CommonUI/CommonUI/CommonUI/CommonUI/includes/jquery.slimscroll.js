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
                height: 'auto',

                // width in pixels of the scrollbar and rail
                size: '9px',

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

                frozenbodyhover: function () { },

                TopPosition: 0,
                getScrollBarHeight: function () { },

                setVerticalScroll: function () { }


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
                var isWorking = 0;
                var version = checkVersion();
                if (version == 8 && o.wheelStep == 'undefined') {
                    o.wheelStep = 2;
                }
                //$("#text1").val(0);
                var isOverPanel, isOverBar, isDragg, queueHide, touchDif,
        barHeight, percentScroll, lastScroll,
        divS = '<div></div>',
        minBarHeight = 30,
        releaseScroll = false;
                // used in event handlers and for better minification
                var me = $(this);
                var flag = true;
                var wid = me.css('width');
                // ensure we are not binding it again
                if (me.parent().hasClass(o.wrapperClass)) {
                    // start from last bar position
                    //me.css('height')

                    $("#" + o.xlgridid + "_headerDiv").css({ width: me[0].style.width != "" ? me[0].style.width : o.width });
                    me.css({ width: me[0].style.width != "" ? me[0].style.width : o.width })
                    me.parent().css({ width: me[0].style.width != "" ? me[0].style.width : o.width })
                    $("#" + o.xlgridid + "_frozen_bodyDiv").css({ height: o.height });
                    me.css({ height: o.height })
                    me.parent().css({ height: o.height })
                    var offset = me.scrollTop();
                    //me.parent().css('height', me.css('height'));
                    // find bar and rail
                    bar = me.parent().find('.' + o.barClass + me.attr('id'));
                    rail = me.parent().find('.' + o.railClass);

                    if (o.xlgridid != null) {
                        if ($find(o.xlgridid).get_GridInfo().IsMasterChildGrid) {
                            if ($find(o.xlgridid).get_GridInfo().MasterGridId == o.xlgridid) {
                                getBarHeight();
                            }
                        }
                        else {
                            getBarHeight();
                        }
                    }
                    else {
                        getBarHeight();
                    }

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

                        //me.css('height' , o.height);
                        // scroll content by the given offset
                        scrollContent(offset, false, true);
                        bar.css({ 'top': o.TopPosition + "px" });
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
              //width: wid, //o.width,
              //width: o.width,
              width: me[0].style.width != "" ? me[0].style.width : o.width,
              //width: parseInt(me.children()[0].style.width) > parseInt(me[0].style.width) ? o.width : wid,
              height: o.height
          });

                // update style for the div
                me.css({
                    overflow: 'hidden',
                    //width: o.width ,
                    width: me[0].style.width != "" ? me[0].style.width : o.width,
                    //width: parseInt(me.children()[0].style.width) > parseInt(me[0].style.width) ? o.width : wid,
                    height: o.height
                });

                //$("#" + o.xlgridid + "_headerDiv").css({ width: o.width });
                $("#" + o.xlgridid + "_headerDiv").css({ width: me[0].style.width != "" ? me[0].style.width : o.width });
                $("#" + o.xlgridid + "_frozen_bodyDiv").css({ height: o.height });


                var railflag = true;
                if (o.xlgridid != null) {
                    if ($find(o.xlgridid).get_GridInfo().IsMasterChildGrid) {
                        if ($find(o.xlgridid).get_GridInfo().MasterGridId != o.xlgridid) {
                            railflag = false;
                        }
                        else {
                            railflag = true;
                        }
                    }
                    else {
                        railflag = true;
                    }
                }
                // create scrollbar rail
                var rail = $(divS)
          .addClass(o.railClass)
          .css({
              width: o.size,
              height: '100%',
              position: 'absolute',
              top: 0,
              display: railflag ? ((o.alwaysVisible && o.railVisible) ? 'block' : 'none') : 'none',
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
              top: o.TopPosition + 'px',
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
                        scrollSensitivity: '1px',
                        containment: 'parent',
                        start: function () { isDragg = true; },
                        stop: function () {
                            isDragg = false; hideBar();
                            var scrollup = false;
                            var barpositiondrag = parseInt(bar.css('top')); //$(this).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id')).offset().top;
                            var scrollfrozendivdrag = me.scrollTop();
                            var verticalscrolltopdrag = parseInt(bar.css('top'));
                            o.setVerticalScroll.call(this, barpositiondrag);
                            if ((Math.round($(this).closest('.' + o.wrapperClass).find(".slimScrollRail").offset().top + $(this).closest('.' + o.wrapperClass).find(".slimScrollRail").height()) == Math.round($(this).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id')).height() + $(this).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id')).offset().top))) {
                                o.controlscroll.call(this, $(this).closest('.horizonslimScrollDiv').find(".horizonslimScrollBar")[0], $(this).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id'))[0], $(this).closest('.' + o.wrapperClass).find(".slimScrollRail")[0], scrollup, barpositiondrag, scrollfrozendivdrag, verticalscrolltopdrag, 0);
                            }
                            else if ($(this).closest('.' + o.wrapperClass).find(".slimScrollRail").offset().top == $(this).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id')).offset().top) {
                                scrollup = true;
                                o.controlscroll.call(this, $(this).closest('.horizonslimScrollDiv').find(".horizonslimScrollBar")[0], $(this).closest('.' + o.wrapperClass).find(".slimScrollBar" + me.attr('id'))[0], $(this).closest('.' + o.wrapperClass).find(".slimScrollRail")[0], scrollup, barpositiondrag, scrollfrozendivdrag, verticalscrolltopdrag, 0);
                            }
                        },
                        drag: function (e, info) {
                            // scroll content
                            scrollContent(0, $(this).position().top, false, info.helper);
                            if (info.originalPosition.top > info.position.top) {
                                groupedHeaderRow(true);
                            }
                            else {
                                groupedHeaderRow(false);
                            }
                            o.dragscroll.call(this);
                        }
                    });
                }

                // on rail over
                rail.hover(function () {
                    showBar();
                    bar.css({ width: '9px' });
                    rail.css({ width: '9px' });
                }, function () {
                    hideBar();
                });

                // on bar over
                bar.hover(function () {
                    isOverBar = true;
                    showBar();
                    bar.css({ width: '9px' });
                    rail.css({ width: '9px' });
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
                    //alert('test');
                    isOverPanel = false;
                    hideBar();
                });

                $("#" + o.xlgridid + "_frozen_bodyDiv").hover(function () {
                    isOverPanel = true;

                }, function () {
                    //alert('test1');
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
                detachwheel();
                attachWheel();





                // set up initial height
                if (o.xlgridid != null) {
                    if ($find(o.xlgridid).get_GridInfo().IsMasterChildGrid) {
                        if ($find(o.xlgridid).get_GridInfo().MasterGridId == o.xlgridid) {
                            getBarHeight();
                        }
                    }
                    else {
                        getBarHeight();
                    }
                }
                else {
                    getBarHeight();
                }

                function _onWheel(e) {
                    // use mouse wheel only when mouse is over
                    if (o.wheelStep == 'undefined') {

                    }
                    else {
                        var target = e.target || e.srcTarget || e.srcElement;
                        if (version <= 8) {
                            //detachwheel();
                        }
                        $("#text2").val("Is Working" + isWorking);
                        //                    console.log("Event FIred");
                        //                    console.log(new Date().getTime().toString());
                        if (true) {
                            if (!isOverPanel) { return; }


                            var gridid = me.attr('id').substring(0, me.attr('id').lastIndexOf('_'));
                            //var lastpageflag = false;
                            if (version <= 8) {
                                isWorking = 1;
                                $("#text2").val("Is Working" + isWorking);
                            }
                            var scrollup = false;
                            var delta = 0;
                            if (e.wheelDelta) { delta = -e.wheelDelta / 120; }
                            if (e.detail) { delta = e.detail / 3; }
                            if (e.originalEvent) { delta = -e.originalEvent.wheelDelta / 120 };
                            if (delta < 0) { scrollup = true; }

                            //if ((($("#" + gridid + "_bodyDiv").find("table").height() < $("#" + gridid + "_bodyDiv").height()) && lastpageflag && scrollup) || ($("#" + gridid + "_bodyDiv").find("table").height() > $("#" + gridid + "_bodyDiv").height())) {

                            if ($(target).closest('.' + o.wrapperClass).find(me)) {
                                // scroll content
                                //if (!(($("#" + o.xlgridid + "_bodyDiv").find("table").height() < $("#" + o.xlgridid + "_bodyDiv").height()) && lastpageflag && scrollup)) {
                                scrollContent(delta, true, null, $(target).closest('#' + me.attr('id').substring(0, me.attr('id').indexOf('_bodyDiv'))).find(".slimScrollBar" + me.attr('id')));
                                if (version <= 8) {
                                    setTimeout(function () {
                                        //attachWheel();
                                    }, 100);
                                }
                                groupedHeaderRow(scrollup);
                                //                            console.log("Set Is Workin to 1");
                                //                            console.log(new Date().getTime().toString());
                                //o.controlscroll.call(this, scrollvalues, scrollup, barposition, scrollfrozendiv, verticalscrolltop);
                             var gridid = me.attr('id');

                               if ($(target).closest('#' + me.attr('id').substring(0, me.attr('id').indexOf('_bodyDiv'))).find(".horizonslimScrollBar" + gridid).length > 0) {
                                   o.controlscroll.call(this, $(target).closest('#' + me.attr('id').substring(0, me.attr('id').indexOf('_bodyDiv'))).find(".horizonslimScrollBar" + gridid)[0], $(target).closest('#' + me.attr('id').substring(0, me.attr('id').indexOf('_bodyDiv'))).find(".slimScrollBar" + me.attr('id'))[0], $(target).closest('#' + me.attr('id').substring(0, me.attr('id').indexOf('_bodyDiv'))).find(".slimScrollRail")[0], scrollup, barposition, scrollfrozendiv, verticalscrolltop, 2);
                               }
                            }
                            else {
                                var a = 1;
                            }
                            if (e.preventDefault && !releaseScroll) { e.preventDefault(); }
                            if (!releaseScroll) { e.returnValue = false; }

                            //}
                        }
                    }
                }


                function groupedHeaderRow(scrollup) {
                    var currentTop = $("#" + o.xlgridid + "_bodyDiv").offset().top + 22;
                    var topElement = null;
                    var i = 0;

                    var groupHeaderRowCollection = $("#" + o.xlgridid + "_repeatedBodyDiv").find('div[grouprowid]');

                    if (groupHeaderRowCollection.length > 0) {
                        if (!scrollup) {
                            while ($(groupHeaderRowCollection[i]).offset().top - currentTop <= 0) {
                                topElement = groupHeaderRowCollection[i];
                                i++;
                                if (i > groupHeaderRowCollection.length - 1) {
                                    break;
                                }
                            }
                        }
                        else {
                            i = groupHeaderRowCollection.length - 1
                            while (($(groupHeaderRowCollection[i]).offset().top) - (currentTop) >= 0) {
                                i--
                                topElement = groupHeaderRowCollection[i];
                                if (i < 0) {
                                    break;
                                }
                            }
                        }
                        if (!(scrollup && i == groupHeaderRowCollection.length - 1)) {
                            $("#" + o.xlgridid + "_FixedHeaderRow").empty();
                            $("#" + o.xlgridid + "_FixedHeaderRow").append($(topElement).clone());
                        }

                        //                        if (scrollup) {
                        //                            groupHeaderRowCollection[i - 1].style.display = "none";
                        //                        }
                    }
                }

                $("#" + me.attr('id').substring(0, me.attr('id').lastIndexOf('_'))).find(".slimScrollRail").click(function (e) {
                    var pos = 0;

                    var scrollvalues = new Array();

                    scrollvalues[0] = me.parent().find(".slimScrollBar" + me.attr('id')).position().top;
                    scrollvalues[1] = me.parent().find(".slimScrollBar" + me.attr('id')).parent().offset().top;
                    scrollvalues[2] = me.parent().find(".slimScrollBar" + me.attr('id')).height();
                    scrollvalues[3] = me.parent().find(".slimScrollRail").height();

                    o.controlclick.call(this, scrollvalues, e.pageY);

                    if (e.pageY > (me.parent().find(".slimScrollBar" + me.attr('id')).position().top + me.parent().find(".slimScrollBar" + me.attr('id')).parent().offset().top)) {
                        pos = e.pageY - (me.parent().find(".slimScrollBar" + me.attr('id')).position().top + me.parent().find(".slimScrollBar" + me.attr('id')).parent().offset().top);
                        if (((e.pageY - me.parent().find(".slimScrollBar" + me.attr('id')).parent().offset().top) + me.parent().find(".slimScrollBar" + me.attr('id')).height()) > me.parent().find(".slimScrollRail").height()) {
                            pos = e.pageY - me.parent().parent().find(".horizonslimScrollBar").parent().offset().top;
                            me.scrollTop((pos - $(".slimScrollBar" + me.attr('id')).height()) * (me.find('table').height() / $(".slimScrollRail").height()));
                            me.parent().find(".slimScrollBar" + me.attr('id')).css({ 'top': (pos - me.parent().find(".slimScrollBar" + me.attr('id')).height()) });
                        }
                        else {
                            me.parent().find(".slimScrollBar" + me.attr('id')).css({ 'top': (me.parent().find(".slimScrollBar" + me.attr('id')).position().top + (pos - me.parent().find(".slimScrollBar" + me.attr('id')).height())) + 'px' });
                            me.scrollTop((pos - $(".slimScrollBar" + me.attr('id')).height()) * (me.find('table').height() / $(".slimScrollRail").height()));
                        }
                    }
                    else {
                        pos = e.pageY - me.parent().find(".slimScrollBar" + me.attr('id')).parent().offset().top;
                        me.scrollTop((pos) * (me.find('table').height() / $(".slimScrollRail").height()));
                        me.parent().find(".slimScrollBar" + me.attr('id')).css({ 'top': (pos) });
                    }
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

                function scrollContent(y, isWheel, isJump, scrollbar) {
                    var delta = y;
                    var maxTop = me.outerHeight() - (scrollbar == null ? $(".slimScrollBar" + me.attr('id')).height() : scrollbar.height()); // bar.outerHeight();
                    //var maxTop = me.outerHeight() - $(".slimScrollBar").height(); // bar.outerHeight();
                    if (isWheel) {
                        // move bar with mouse wheel
                        if (version > 8) {
                            delta = parseInt(bar.css('top')) + y * parseInt(o.wheelStep) / 100 * (scrollbar == null ? $(".slimScrollBar" + me.attr('id')).height() : scrollbar.height()); // 
                        }
                        else {
                            delta = parseInt(scrollbar.css('top')) + y * parseInt(o.wheelStep) / 100 * (scrollbar == null ? $(".slimScrollBar" + me.attr('id')).height() : scrollbar.height()); // bar.outerHeight();
                        }

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
                        barposition = delta; //- (0.1 * delta);
                        verticalscrolltop = scrollbar == null ? parseInt(bar.css('top')) : parseInt(scrollbar.css('top'));
                    }

                    // calculate actual scroll amount
                    if (version > 8) {
                        percentScroll = parseInt(bar.css('top')) / (me.outerHeight() - (scrollbar == null ? $(".slimScrollBar" + me.attr('id')).height() : scrollbar.height())); //bar.outerHeight());
                    }
                    else {
                        percentScroll = parseInt(scrollbar == null ? bar.css('top') : scrollbar.css('top')) / (me.outerHeight() - (scrollbar == null ? $(".slimScrollBar" + me.attr('id')).height() : scrollbar.height())); //bar.outerHeight());
                    }

                    //percentScroll = parseInt(bar.css('top')) / (me.outerHeight() - $(".slimScrollBar").height()); //bar.outerHeight());
                    if (version > 8) {
                        delta = percentScroll * (me[0].scrollHeight - me.outerHeight());
                    }
                    else {
                        delta = percentScroll * ($("#" + o.xlgridid + "_bodyDiv_Table").height() - me.outerHeight());
                    }

                    if (isJump) {
                        delta = y;
                        var offsetTop = delta / me[0].scrollHeight * me.outerHeight();
                        offsetTop = Math.min(Math.max(offsetTop, 0), maxTop);
                        bar.css({ top: offsetTop + 'px' });
                    }


                    // scroll content

                    scrollposition = delta - (0.1 * delta);
                    //me.scrollTop(delta);
                    if (version > 8) {
                        me.scrollTop(delta);
                        scrollfrozendiv = delta;
                    }
                    else {
                        $("#" + o.xlgridid + "_bodyDiv").scrollTop(delta);
                         $("#" + o.xlgridid + "_frozen_bodyDiv").scrollTop(delta);
                        scrollfrozendiv = delta;
                    }
                    // fire scrolling event
                    me.trigger('slimscrolling', ~ ~delta);


                    //$("#text1").val(parseInt($("#text1").val()) + 1);

                    // ensure bar is visible
                    showBar();

                    // trigger hide when scroll is stopped
                    hideBar();
                }

                function detachwheel() {
                    if (window.addEventListener) {
                        document.getElementById(me.attr('id')).removeEventListener('DOMMouseScroll', _onWheel, false);
                        document.getElementById(me.attr('id')).removeEventListener('mousewheel', _onWheel, false);
                        if (o.xlgridid != null) {
                            document.getElementById(o.xlgridid + "_frozen_bodyDiv").removeEventListener('DOMMOuseScroll', _onWheel, false);
                            document.getElementById(o.xlgridid + "_frozen_bodyDiv").removeEventListener('mousewheel', _onWheel, false);
                        }
                    }
                    else {
                        //document.detachEvent("onmousewheel", _onWheel);
                        $(document).off("mousewheel");
                        //$(document).unbind("mousewheel")//, _onWheel);
                    }
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
                        //document.attachEvent("onmousewheel", _onWheel);
                        $(document).on("mousewheel", _onWheel);
                    }
                }

                function getBarHeight() {
                    // calculate scrollbar height and make sure it is not too small
                    //                    if (me[0].scrollHeight - me.outerHeight() < 5) {
                    //                        me.outerHeight(me[0].scrollHeight);
                    //                    }
                    barHeight = Math.max((me.outerHeight() / me[0].scrollHeight) * me.outerHeight(), minBarHeight);
                    bar.css({ height: barHeight + 'px' });
                    // hide scrollbar if content is not long enough
                    if (barHeight > me.outerHeight()) {
                        barHeight = me.outerHeight();
                    }
                    if (me.outerHeight() - barHeight > 0 && me.outerHeight() - barHeight < 1) {
                        barHeight = me.outerHeight();
                    }
                    var display = barHeight == me.outerHeight() ? 'none' : 'block';
                    bar.css({ display: display });
                    rail.css({ display: display });
                    o.getScrollBarHeight.call(this, bar[0]);
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
                    if (o.xlgridid != null) {
                        if ($find(o.xlgridid).get_GridInfo().IsMasterChildGrid) {
                            if ($find(o.xlgridid).get_GridInfo().MasterGridId == o.xlgridid) {
                                getBarHeight();
                            }
                            else {
                                bar.css({ display: 'none' });
                                rail.css({ display: 'none' });
                            }
                        }
                        else {
                            getBarHeight();
                        }
                    }
                    else {
                        getBarHeight();
                    }
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
                    //bar.stop(true, true).fadeIn('fast');
                    if (o.railVisible) {
                        if (o.xlgridid != null) {
                            if ($find(o.xlgridid).get_GridInfo().IsMasterChildGrid) {
                                if ($find(o.xlgridid).get_GridInfo().MasterGridId == o.xlgridid) {
                                    bar.stop(true, true).fadeIn('fast');
                                }
                            }
                            else {
                                bar.stop(true, true).fadeIn('fast');
                            }
                        }
                        else {
                            bar.stop(true, true).fadeIn('fast');
                        }
                    }

                    if (o.railVisible) {
                        if (o.xlgridid != null) {
                            if ($find(o.xlgridid).get_GridInfo().IsMasterChildGrid) {
                                if ($find(o.xlgridid).get_GridInfo().MasterGridId == o.xlgridid) {
                                    rail.stop(true, true).fadeIn('fast');
                                }
                            }
                            else {
                                rail.stop(true, true).fadeIn('fast');
                            }
                        }
                        else {
                            rail.stop(true, true).fadeIn('fast');
                        }
                    }
                    //if (o.railVisible) { rail.stop(true, true).fadeIn('fast'); }
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
