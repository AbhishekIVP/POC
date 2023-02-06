$(function () {
    $.widget("iago-widget.inlineFilter", {

        options: {
            filterInfo: {
                filterPhrase: "{0} by {1}",
                bindingInfo: [{
                    identifier: "tab",
                    data: [{
                        id: 0,
                        text: 'Position'
                    }, {
                        id: 1,
                        text: 'Trade'
                    }, {
                        id: 2,
                        text: 'Position Allocation'
                    }],
                    placeholder: "select one",
                    multiple: false,
                    hideSelected: false,
                    placeholderAlwaysOn: false,
                    class: ' ',
                    hoverClass: ' ',
                    dropDownIcon: true,
					disableAll : false,
                    formatSelection: function (selectedElem) {
                        return '<a class="inlineFilter-select-std">' + selectedElem.text + '</a>';
                    },
                    formatResult: function (currentItem) {
                        return currentItem.text;
                    }
                }, {
                    identifier: "subtab",
                    data: [{
                        id: 0,
                        text: 'Fund'
                    }, {
                        id: 1,
                        text: 'Strategy'
                    }, {
                        id: 2,
                        text: 'Sector'
                    }],
                    defaultValue: 1,
                    multiple: false,
                    class: ' ',
                    hoverClass: ' ',
                    dropDownIcon: true,
                    placeholder: "select another"
                }]
            },
            selectHandler: function (selectedValue, event, filterValueDictionary, selectedIdentifier) { }
        },

        _create: function () {

            var changeText = function (element, placeholderAlwaysOn, forceClear) {
                
                var breakCount = self.options.filterInfo.bindingInfo[parseInt($(element).data("filterindex"))].maxResultsToShow;
                var id = element.id;
                var placeholderText = self.options.filterInfo.bindingInfo[parseInt($(element).data("filterindex"))].placeholder;
                var defaultValues = self.options.filterInfo.bindingInfo[parseInt($(element).data("filterindex"))].defaultValue;
                var dropdownIconRequired = self.options.filterInfo.bindingInfo[parseInt($(element).data("filterindex"))].dropDownIcon;
                var breakCount = (breakCount === undefined) ? Number.MAX_VALUE : breakCount;
                var extraCount = 0;
                var children = $(element).siblings().first().children('.select2-choices').first().children('.select2-search-choice');
                var count = children.size();
                var first = children.first();

                var placeholderInput = $(element).siblings().first().children('.select2-choices').children().find('input');
                if (placeholderInput.length > 0 && placeholderAlwaysOn) {
                    $(placeholderInput).attr('placeholder', placeholderText);
					//var browserName = self.checkBrowserName(); //added by harneet
					//if(browserName!='Chrome')
					//{
					//$('span.multiInlineFilter.inlineFilter .select2-choices input[type=text]').css({"margin-left":"10%"});
					//}

                }
                var res = '';

                if (!first.data('oldText')) {
                    first.data('oldText', first.text());
                }
                res = first.data('oldText');

                $.each(children, function (i, e) {
                    if (i > 0) {
                        if (i > breakCount - 1) {
                            extraCount++;
                        }
                        else {
                            res += ', ' + $(e).text();
                        }
                    }
                });

                if (breakCount > 0 && count > breakCount) {
                    res += "[+" + extraCount + " more]";
                }


                if (placeholderAlwaysOn && defaultValues != "") {
                    first.text("");

                }

                else {
                    if (forceClear) {
                        
                        first.text("");
                        // first.addClass('multiInlineFilter-select-std');
                        // children.not(":first").hide();
                    }
                    else {
                        if (dropdownIconRequired) {

                            first.text(res);

                            if ($('#arrow-span-' + id).length > 0) {
                                $('#arrow-span-' + id).remove();
                            }

                            $('<span id="arrow-span-' + id + '" style="display:inline-block;" class="custom-angle-down-span"><div class="custom-angle-down"></div></span>').insertAfter($(element).parent());


                        }
                        else {

                            first.text(res);
                            //$(placeholderInput).attr('placeholder',res);
                        }
                        //res="";
                        //first.text(res);      
                    }
                }

                first.addClass('multiInlineFilter-select-std');
                children.not(":first").hide();
            };

            var self = this;
            this.inlines = [];
            if (!this.element.attr("id"))
                this.element.attr("id", "if" + iago.handlers.guid()());
            var widgetId = this.element.attr("id");
            var filterInfo = this.options.filterInfo;
            var phrase = filterInfo.filterPhrase;
            var formatter = phrase.match(new RegExp("{[0-9]*}", "g"));
            if (formatter) {
                for (var i = 0; i < formatter.length; i++) {
                    var filterIndex = parseInt(formatter[i].replace("{", "").replace("}", ""));
                    phrase = phrase.replace(formatter[i],
                        '<span class="inlineFilter' + (filterInfo.bindingInfo[i].multiple ? ' multiInlineFilter ' : ' ') + (filterInfo.bindingInfo[i].class ? filterInfo.bindingInfo[i].class : '') + '"><input type="hidden"  class="' + (filterInfo.bindingInfo[i].class ? filterInfo.bindingInfo[i].class : '') + '" data-filterindex="' + filterIndex + '" value="' + ((filterInfo.bindingInfo[filterIndex].defaultValue === undefined || filterInfo.bindingInfo[filterIndex].defaultValue === "") ? "" : filterInfo.bindingInfo[filterIndex].defaultValue) + '" id="inlineFilter_' + filterInfo.bindingInfo[filterIndex].identifier + '" data-identifier="' + filterInfo.bindingInfo[filterIndex].identifier + '"/></span>');
                }
            }

            phrase = "<div>" + phrase + "</div>";
            $(phrase).appendTo(this.element);

            $("#" + widgetId + ' .inlineFilter input[type="hidden"]').each(function (index, elem) {

                var item = filterInfo.bindingInfo[parseInt($(elem).data("filterindex"))];

                $(elem).select2({
                    data: item.data,
                    dropdownCssClass: ' inlineFilterClass ' + $(elem).attr('class'),
                    formatSelection:
                    //(item.formatSelection === undefined || item.formatSelection === "") ?
                        function (selectedElem) {
                            /*  return (item.multiple ?
                            '<a class="inlineFilter-select-std">' + selectedElem.text + '</a>' :
                            '<a class="inlineFilter-select-std">' + selectedElem.text + '</a><b style="padding-top:3px;padding-left:8px;color:#00bcef;" class="fa fa-angle-down custom-angle-down" ></b>');
                            } : item.formatSelection*/
                            if(self.options.filterInfo.dropDownIcon || self.options.filterInfo.dropDownIcon==undefined)
                            {
                                return '<a class="inlineFilter-select-std">' + selectedElem.text + '</a><b style="padding-top:3px;padding-left:5px;" class="fa fa-angle-down"></b>';
                            }
                            else
                            {
                               return '<a style="padding-right:8px" class="inlineFilter-select-std">' + selectedElem.text + '</a>'; 
                            }

                        }
                    ,
                    formatResult: function (currentItem) {
                        var originalOption = currentItem.element;
                        if (!item.multiple) {
                            if (item.formatResult) {
                                return item.formatResult(currentItem);
                            } else {
                                return currentItem.text;
                            }
                        } else {

                            var arr = $(elem).select2('data');
                            var base = "<div class='select2-result-label'>" + currentItem.text;
                            var checkedText = "<span class='pull-right glyphicon glyphicon-ok' style='color: #000'></span>";
                            var isFound = false;
							var isDisabled= false;
                            arr.forEach(function (e) {
                                if (isFound) return;
                                isFound = e.text === currentItem.text;
								isDisabled = e.disabled;
						
                            });
                            //  return base + (isFound ? checkedText : '') + '</div>';
                            if (item.formatResult) {
                                arr.forEach(function (e) {
                                    if (isFound) return;
                                    isFound = e.text === currentItem.text;
                                });
                                var status = isFound ? "Checked" : "Unchecked";
				
                                return item.formatResult(currentItem, status,isDisabled);
                            } else {
                                return base + (isFound ? checkedText : '') + '</div>';
                            }
                        }
                    },
					/*updateResults:function(){
					alert('htyt');
				;
					
					},*/
                    allowClear: false,
                    closeOnSelect: (!item.multiple),
                    width: "style",
                    minimumResultsForSearch: 10,
                    dropdownAutoWidth: true,
                    placeholder: item.placeholder,
                    multiple: item.multiple,
                    sortResults: function (results, container, query) {

                        if (!item.multiple) {
                            if (item.hideSelected) {
                                var selected = [];
                                $.each(results, function (i, e) {
                                    if (e.id !== $(elem).select2('data').id) {
                                        selected.push(e);
                                    }
                                });
                                return selected;
                            }
                            return results;
                        }

                        var selected = [];
                        var arr = $(elem).select2('data');
                        arr.forEach(function (e) {
                            selected.push({
                                id: e.id + 'm',
                                text: e.text
                            });
                        });
                        return selected.concat(results);
                    }
                })
				.on('select2-selecting', function (evt) {
				if(item.disableAll)
				{
				return false;
				}
				
				    if (item.multiple) {
				        var me = this;
				        var $me = $(me);

				        if (item.AllPlaceHolder) {
				            if (evt.object.id == item.AllPlaceHolder) {
				                $me.select2("val", evt.val);
				            } else {
				                new_data = $.grep($me.select2('data'), function (value) {
				                    return value['id'] != item.AllPlaceHolder;
				                });
				                $me.select2('data', new_data);
				            }
				        }
				        var newValue = evt.object.id;
				        var found = false;
				        var arr = $me.select2('data');
				        arr.forEach(function (e) {
				            if (found) return;
				            if (newValue === e.id + 'm') {
				                found = true;
				                var index = arr.indexOf(e);
				                if (index === -1) console.error(e, "NOT FOUND IN ARRAY. ABORT ABORT ABORT.");
				                arr.splice(index, 1);
				                $me.select2('data', arr, true);
				                changeText(me, item.placeholderAlwaysOn, true);
				                $me.select2('updateResults',false);
//				                var sHandler = self.options.selectHandler;
//				                if (typeof (sHandler) === "function") {
//				                    sHandler(e.val, evt, self.getCurrentState(), $(evt.target).data("identifier"));
//				                }
				                evt.preventDefault();
				            }
				        });
						
						
						if(item.disableAll)
				{
				   $('.select2-result-selectable').addClass('select2-result-unselectable select2-disabled').removeClass('select2-result-selectable');
				}
				var me = this;
				        var $me = $(me);
				
			//var dataList= $me.select2('data');
			var dataList = item.data;
			$.each(dataList, function(index,item)
			{
			if(item.disabled)
			{
		
			      $(".select2-result-label").each(function(index,i)
		{
			if(i.textContent==item.text)
			{
			   $(i).parent().removeClass('select2-result-selectable').addClass('select2-result-unselectable select2-disabled');
			}
		})
			   
			}
			});
						
						
				      }
					
				})
                .on('change', function (e) {
                
                var sHandler = self.options.selectHandler;
                if (typeof (sHandler) === "function") {
                sHandler(e.val, e, self.getCurrentState(), $(e.target).data("identifier"));
                }
				
					if(item.disableAll)
				{
				   $('.select2-result-selectable').addClass('select2-result-unselectable select2-disabled').removeClass('select2-result-selectable');
				}
				var me = this;
				        var $me = $(me);
				;
			//var dataList= $me.select2('data');
			var dataList = item.data;
			$.each(dataList, function(index,item)
			{
			if(item.disabled)
			{
		
			      $(".select2-result-label").each(function(index,i)
		{
			if(i.textContent==item.text)
			{
			   $(i).parent().removeClass('select2-result-selectable').addClass('select2-result-unselectable select2-disabled');
			}
		})
			   
			}
			});
			;
				
				
				
				
                })
					
					
				
                /*else
                {
                changeText(this,item.placeholderAlwaysOn,false);
                }
                var sHandler = self.options.selectHandler;
                if (typeof (sHandler) === "function") {
                sHandler(e.val, e, self.getCurrentState(), $(e.target).data("identifier"));
                }
                })*/
				.on('select2-removed', function (e) {
				    if (item.multiple) {
				        changeText(this, item.placeholderAlwaysOn, true);
				    }
//				    var sHandler = self.options.selectHandler;
//				    if (typeof (sHandler) === "function") {
//				        sHandler(e.val, e, self.getCurrentState(), $(e.target).data("identifier"));
//				    }
				}).on('select2-open',function(evt){
				if(item.disableAll)
				{
				   $('.select2-result-selectable').addClass('select2-result-unselectable select2-disabled').removeClass('select2-result-selectable');
				}
				var me = this;
				        var $me = $(me);
				;
			//var dataList= $me.select2('data');
			var dataList = item.data;
			$.each(dataList, function(index,item)
			{
			if(item.disabled)
			{
		
			      $(".select2-result-label").each(function(index,i)
		{
			if(i.textContent==item.text)
			{
			   $(i).parent().removeClass('select2-result-selectable').addClass('select2-result-unselectable select2-disabled');
			}
		})
			   
			}
			});
			;
				})
				.on('select2-loaded', function (evt) {
				    
				    //  $(element).siblings().first().children('.select2-choices').first().children('.select2-search-field').text()
                   //console.log('hello');
				    if (item.multiple) {
				        if (item.placeholderAlwaysOn) {
				            changeText(this, item.placeholderAlwaysOn, true);
				        }
				        //check search box is not empty
				        else if ($(evt.currentTarget).siblings().first().children('.select2-choices').first().children('.select2-search-field').text().trim() != "") {
				            
				            $(evt.currentTarget).parents('ul').find('li.multiInlineFilter-select-std').text("");
				            changeText(this, item.placeholderAlwaysOn, true);
				        }
				        else {
				            changeText(this, item.placeholderAlwaysOn, false);
				        }
				    }
				    else {
				        changeText(this, item.placeholderAlwaysOn, false);
				    }


				    if (item.hoverClass != ' ' || item.hoverClass != undefined || item.hoverClass != null) {
				        $('.inlineFilterClass.definline li').addClass(item.hoverClass);

				    }
				})
				.on('close', function (evt) {
				    if (item.multiple) {
				        changeText(this, true);
				    }


				});
                if (item.multiple) {
                    changeText(this, item.placeholderAlwaysOn);
                }
            
			   ;
            if(item.disableAll===true)
			{
		
		
			self.disableAllOptions(item.identifier);
			
			
			
			
			}

            });

			   
		 
			
			
			
			
			
			
			
			
            var item;
            $("#" + widgetId + ' .inlineFilter input[type="hidden"]').each(function (index, e) {
                
                item = filterInfo.bindingInfo[parseInt($(e).data("filterindex"))];

                $("#" + widgetId + ' .inlineFilter input[type="text"]').each(function (index, elem) {
                    $(elem).unbind('keyup').bind('keyup', function () {
                        
                        if (item.multiple) {
                            $(this).parents('ul').find('li.multiInlineFilter-select-std').text("");

                            changeText(e, false, true);
                        }


                    });

                    /*	 $(elem).change(function(){
                    if (item.multiple) {
                    // $(this).parents('ul').find('li.multiInlineFilter-select-std').text("");
                    changeText(e,false,true);
                    }
				
				
                    });*/
                    $(elem).blur(function () {
                        if (item.multiple) {
                            $(this).parents('ul').find('li.multiInlineFilter-select-std').text("");
                            changeText(e, true);
                        }
                    });

                });



            });
      
            /*  iago.core.registerWidget({
            id: this.options.identifier,
            widget: "inlineFilter"
            }); */
        },
		disableAllOptions :function(identifier)
		{	;
		 var bindingInfo = this.options.filterInfo.bindingInfo;
		 var dataList;
		 $.each(bindingInfo,function(index,item)
		 {
		 if(item.identifier==identifier)
		 { dataList=item.data;
		    return false;
		 }
		 });
		 var idList=[];
		 $.each(dataList,function(index,item)
		 {
	
		 idList.push(item.id);
		 });

			this.disableItems(identifier,idList);
		},
        enableItems: function (identifier, itemArray) {
            var me = this;
            
            var $me = $(me);
            var widgetId = this.element.attr("id");
            var filterInfo = this.options.filterInfo;
            $("#" + widgetId + ' .inlineFilter input[type="hidden"]').each(function (index, elem) {
                
                var item = filterInfo.bindingInfo[parseInt($(elem).data("filterindex"))];
                if (item.identifier == identifier) {
                    var disableItemCounter = 0;
                    var inlineData = item.data;
                    $.map(inlineData, function (i, index) {
                        for (j = 0; j < itemArray.length; j++) {
                            if (i.id == itemArray[j]) {
                                i.disabled = false;
                                disableItemCounter++;
                                return false;
                            }
                        }
                    });
                    return false;
                }

            });
        },
        disableItems: function (identifier, itemArray) {
            var me = this;
            
            var $me = $(me);
            var widgetId = this.element.attr("id");
            var filterInfo = this.options.filterInfo;
            $("#" + widgetId + ' .inlineFilter input[type="hidden"]').each(function (index, elem) {
                
                var item = filterInfo.bindingInfo[parseInt($(elem).data("filterindex"))];
                if (item.identifier == identifier) {
                    var disableItemCounter = 0;
                    var inlineData = item.data;
                    $.map(inlineData, function (i, index) {
                        for (j = 0; j < itemArray.length; j++) {
                            if (i.id == itemArray[j]) {
                                i.disabled = true;
                                disableItemCounter++;
                                return false;
                            }
                        }
                    });
                    return false;
                }

            });

        },
	    checkBrowserName:function() {
		  var nVer = navigator.appVersion;
var nAgt = navigator.userAgent;
var browserName  = navigator.appName;
var fullVersion  = ''+parseFloat(navigator.appVersion); 
var majorVersion = parseInt(navigator.appVersion,10);
var nameOffset,verOffset,ix;

// In Opera 15+, the true version is after "OPR/" 
if ((verOffset=nAgt.indexOf("OPR/"))!=-1) {
 browserName = "Opera";
 fullVersion = nAgt.substring(verOffset+4);
}
// In older Opera, the true version is after "Opera" or after "Version"
else if ((verOffset=nAgt.indexOf("Opera"))!=-1) {
 browserName = "Opera";
 fullVersion = nAgt.substring(verOffset+6);
 if ((verOffset=nAgt.indexOf("Version"))!=-1) 
   fullVersion = nAgt.substring(verOffset+8);
}
// In MSIE, the true version is after "MSIE" in userAgent
else if ((verOffset=nAgt.indexOf("MSIE"))!=-1) {
 browserName = "Microsoft Internet Explorer";
 fullVersion = nAgt.substring(verOffset+5);
}
// In Chrome, the true version is after "Chrome" 
else if ((verOffset=nAgt.indexOf("Chrome"))!=-1) {
 browserName = "Chrome";
 fullVersion = nAgt.substring(verOffset+7);
}
// In Safari, the true version is after "Safari" or after "Version" 
else if ((verOffset=nAgt.indexOf("Safari"))!=-1) {
 browserName = "Safari";
 fullVersion = nAgt.substring(verOffset+7);
 if ((verOffset=nAgt.indexOf("Version"))!=-1) 
   fullVersion = nAgt.substring(verOffset+8);
}
// In Firefox, the true version is after "Firefox" 
else if ((verOffset=nAgt.indexOf("Firefox"))!=-1) {
 browserName = "Firefox";
 fullVersion = nAgt.substring(verOffset+8);
}
// In most other browsers, "name/version" is at the end of userAgent 
else if ( (nameOffset=nAgt.lastIndexOf(' ')+1) < 
          (verOffset=nAgt.lastIndexOf('/')) ) 
{
 browserName = nAgt.substring(nameOffset,verOffset);
 fullVersion = nAgt.substring(verOffset+1);
 if (browserName.toLowerCase()==browserName.toUpperCase()) {
  browserName = navigator.appName;
 }
}
// trim the fullVersion string at semicolon/space if present
if ((ix=fullVersion.indexOf(";"))!=-1)
   fullVersion=fullVersion.substring(0,ix);
if ((ix=fullVersion.indexOf(" "))!=-1)
   fullVersion=fullVersion.substring(0,ix);

majorVersion = parseInt(''+fullVersion,10);
if (isNaN(majorVersion)) {
 fullVersion  = ''+parseFloat(navigator.appVersion); 
 majorVersion = parseInt(navigator.appVersion,10);
}
			
                console.log(browserName);
                return browserName;
		},
        getCurrentState: function () {
            var filterValueDictionary = {};
            var widgetId = this.element.attr("id");
            $("#" + widgetId + ' .inlineFilter input[type="hidden"]').each(function (index, elem) {
                filterValueDictionary[$(elem).data("identifier")] = $(elem).attr("value");
            });
            return filterValueDictionary;
        },

        getCurrentStateObjects: function () {
            var options = this.options;
            var filterValueDictionary = {};
            var widgetId = this.element.attr("id");
            $("#" + widgetId + ' .inlineFilter input[type="hidden"]').each(function (index, elem) {
                var data = options.filterInfo.bindingInfo[0].data;
                var selectedData = $(elem).attr("value").split(',');
                var SelectedArray = [];
                //filterValueDictionary[$(elem).data("identifier")] = $(elem).attr("value");
                /* LOOP HERE */
                $.each(selectedData, function (index, stringId) {
                    var obj = {};
                    obj.id = stringId;
                    for (var k = 0; k < data.length; k++) {
                        if (data[k].id == stringId) {
                            obj.text = data[k].text;
                            SelectedArray.push(obj);
                            break;
                        }
                    }
                });

                filterValueDictionary[$(elem).data("identifier")] = SelectedArray;
            });
            return filterValueDictionary;
        },

        getJson: function () {
            var filterValueDictionary = {};
            var widgetId = this.element.attr("id");
            $("#" + widgetId + ' .inlineFilter input[type="hidden"]').each(function (index, elem) {
                filterValueDictionary[$(elem).data("identifier")] = $(elem).attr("value");
            });
            return filterValueDictionary;
        },
        getSelectedValue:function(){
		return {};
		},
		 getSelectedText:function(){
		return {};
		},
        getViewModel: function () {
            return viewModel;
        },

        getCurrentOptionInfo: function () {
            return {};
        },

        setJson: function (optionData) {
            if (optionData === undefined) return;
            this.viewModel.text(optionData.text);
        },
        open: function (identifier) {
            
            var me = this;
            var $me = $(me);
            var widgetId = this.element.attr("id");

            $("#" + widgetId + ' .inlineFilter input[type="hidden"]').each(function (index, elem) {

                var filterId = 'inlineFilter_' + identifier;
                if (elem.id == filterId) {
                    $(elem).select2("open");
                }


            });
        },
        close: function (identifier) {
         
            var me = this;
            var $me = $(me);
            var widgetId = this.element.attr("id");

            $("#" + widgetId + ' .inlineFilter input[type="hidden"]').each(function (index, elem) {
              
                var filterId = 'inlineFilter_' + identifier;
                if (elem.id == filterId) {
                  
                    $(elem).select2("close");
                }


            });
        },
        _setOptions: function () {
            this._superApply(arguments);
        },

        _setOption: function (key, value) {
            this._super(key, value);
        },

        _destroy: function () {
            /*  iago.core.deRegisterWidget({
            id: this.options.identifier
            }); */
            $("#" + this.element.attr("id") + ' .inlineFilter input[type="hidden"]').each(function (index, elem) {
                $(elem).select2('destroy');
            });
            this.element.html('');
        }

    });
    $('[data-type="iago:inlineFilter"]').each(function (index, value) {
        if ($(value).attr('data-options-type') === "JSON")
            $(value).WidgetName(JSON.parse($(value).attr('data-options')));
    });
});