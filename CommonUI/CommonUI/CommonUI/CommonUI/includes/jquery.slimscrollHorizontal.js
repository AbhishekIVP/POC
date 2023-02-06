/*! Copyright (c) 2011 Piotr Rochala (http://rocha.la)
 * Dual licensed under the MIT (http://www.opensource.org/licenses/mit-license.php)
 * and GPL (http://www.opensource.org/licenses/gpl-license.php) licenses.
 *
 * Version: 1.3.1
 *
 */
 (function(e){jQuery.fn.extend({slimScrollH:function(n){var r={width:"250px",height:"auto",size:"7px",color:"#000",position:"bottom",distance:"1px",start:"left",opacity:.4,alwaysVisible:false,disableFadeOut:false,railVisible:false,railColor:"#333",railOpacity:.2,railDraggable:true,railClass:"slimScrollRailH",barClass:"slimScrollBarH",wrapperClass:"slimScrollDiv",allowPageScroll:false,wheelStep:20,touchScrollStep:200,borderRadius:"7px",railBorderRadius:"7px"};var i=e.extend(r,n);this.each(function(){function k(t){if(!r){return}var t=t||window.event;var n=0;if(t.wheelDelta){n=-t.wheelDelta/120}if(t.detail){n=t.detail/3}if(t.wheelDeltaX||t.deltaX||t.axis==1){var s=t.target||t.srcTarget||t.srcElement;if(e(s).closest("."+i.wrapperClass).is(g.parent())){L(n,true)}if(t.preventDefault&&!m){t.preventDefault()}if(!m){t.returnValue=false}}}function L(t,n,r){m=false;var s=t;var u=g.outerWidth()-x.outerWidth();if(y){e(".mask_left").css({display:x.position().left<=0?"none":"block"});e(".mask_right").css({display:x.position().left>=g.width()-x.width()?"none":"block"})}if(n){s=parseInt(x.css("left"))+t*parseInt(i.wheelStep)/100*x.outerWidth();s=Math.min(Math.max(s,0),u);s=t>0?Math.ceil(s):Math.floor(s);x.css({left:s+"px"})}c=parseInt(x.css("left"))/(g.outerWidth()-x.outerWidth());s=c*(g[0].scrollWidth-g.outerWidth());if(r){s=t;var a=s/g[0].scrollWidth*g.outerWidth();a=Math.min(Math.max(a,0),u);x.css({left:a+"px"})}g.scrollLeft(s);g.trigger("slimscrolling",~~s);M();_()}function A(){if(window.addEventListener){this.addEventListener("DOMMouseScroll",k,false);this.addEventListener("mousewheel",k,false);this.addEventListener("MozMousePixelScroll",k,false)}else{document.attachEvent("onmousewheel",k)}}function O(){l=Math.max(g.outerWidth()/g[0].scrollWidth*g.outerWidth(),v);x.css({width:l+"px"});var e=l==g.outerWidth()?"none":"block";x.css({display:e})}function M(){O();clearTimeout(a);if(c==~~c){m=i.allowPageScroll;if(h!=c){var e=~~c==0?"left":"right";g.trigger("slimscroll",e)}}else{m=false}h=c;if(l>=g.outerWidth()){m=true;return}x.stop(true,true).fadeIn("fast");if(i.railVisible){S.stop(true,true).fadeIn("fast")}}function _(){if(!i.alwaysVisible){a=setTimeout(function(){if(!(i.disableFadeOut&&r)&&!s&&!u){x.fadeOut("slow");S.fadeOut("slow")}},1e3)}}var r,s,u,a,f,l,c,h,p="<div></div>",d="<div></div>",v=30,m=false;var g=e(this);var y=g.width()>=i.width;if(g.parent().hasClass(i.wrapperClass)&&e.inArray(i.barClass,g.parent().children().map(function(t,n){return e(n).attr("class")}))>=0||e.inArray(i.railClass,g.parent().children().map(function(t,n){return e(n).attr("class")}))>=0){var b=g.scrollLeft();x=g.parent().find("."+i.barClass);S=g.parent().find("."+i.railClass);O();if(e.isPlainObject(n)){if("height"in n&&n.height=="auto"){g.parent().css("height","auto");g.css("height","auto");var w=g.parent().parent().height();g.parent().css("height",w);g.css("height",w)}if("scrollTo"in n){b=parseInt(i.scrollTo)}else if("scrollBy"in n){b+=parseInt(i.scrollBy)}else if("destroy"in n){x.remove();S.remove();g.unwrap();return}L(b,false,true)}return}i.width=i.width=="auto"?g.parent().width():i.width;i.height=i.height=="auto"?g.parent().height():i.height;var E=g.parent();if(!g.parent().hasClass(i.wrapperClass)){E=e(p).addClass(i.wrapperClass).css({position:"relative",overflow:"hidden",width:i.width,height:i.height});g.css({overflow:"hidden",width:i.width,height:i.height});g.wrap(E)}var S=e(p).addClass(i.railClass).css({width:"100%",height:i.size,position:"absolute",left:0,display:i.alwaysVisible&&i.railVisible?"block":"none","border-radius":i.railBorderRadius,background:i.railColor,opacity:i.railOpacity,zIndex:90});var x=e(p).addClass(i.barClass).css({background:i.color,height:i.size,position:"absolute",left:0,opacity:i.opacity,display:i.alwaysVisible?"block":"none","border-radius":i.borderRadius,BorderRadius:i.borderRadius,MozBorderRadius:i.borderRadius,WebkitBorderRadius:i.borderRadius,zIndex:99});var T=i.position=="top"?{top:i.distance}:{bottom:i.distance};S.css(T);x.css(T);g.parent().append(x);g.parent().append(S);if(y&&i.mask_left_url){var N=e(d).addClass("mask_left").css({width:"64px",height:"100%",position:"absolute",top:0,left:0,display:x.position().left<=0?"none":"block",backgroundImage:"url("+i.mask_left_url+")",backgroundRepeat:"y",zIndex:85});g.parent().append(N)}if(y&&i.mask_right_url){var C=e(d).addClass("mask_right").css({width:"64px",height:"100%",position:"absolute",bottom:0,right:0,display:x.position().left>=g.width()-x.width()?"none":"block",backgroundImage:"url("+i.mask_right_url+")",backgroundRepeat:"y",zIndex:85});g.parent().append(C)}if(i.railDraggable){x.bind("mousedown",function(n){var r=e(document);u=true;t=parseFloat(x.css("left"));pageY=n.pageX;r.bind("mousemove.slimscroll",function(e){currLeft=t+e.pageX-pageY;x.css("left",currLeft);L(0,x.position().left,false)});r.bind("mouseup.slimscroll",function(e){u=false;_();r.unbind(".slimscroll")});return false}).bind("selectstart.slimscroll",function(e){e.stopPropagation();e.preventDefault();return false})}S.hover(function(){M()},function(){_()});x.hover(function(){s=true},function(){s=false});g.hover(function(){r=true;M();_()},function(){r=false;_()});g.bind("touchstart",function(e,t){if(e.originalEvent.touches.length){f=e.originalEvent.touches[0].pageY}});g.bind("touchmove",function(e){if(!m){e.originalEvent.preventDefault()}if(e.originalEvent.touches.length){var t=(f-e.originalEvent.touches[0].pageY)/i.touchScrollStep;L(t,true);f=e.originalEvent.touches[0].pageY}});O();if(i.start==="right"){x.css({left:g.outerWidth()-x.outerWidth()});L(0,true)}else if(i.start!=="left"){L(e(i.start).position().left,null,true);if(!i.alwaysVisible){x.hide()}}A()});return this}});jQuery.fn.extend({slimscrollH:jQuery.fn.slimScrollH})})(jQuery)