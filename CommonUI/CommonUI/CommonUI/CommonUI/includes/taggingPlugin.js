$(function () {
    $.widget("iago-widget.tagging", {
        options: {
            isPersistantTextBoxRequired: false,
            DateColumnName: "",
            tagHeading: "",
            DialogTop: "",
            serviceURL: "",
            pageIdentifier: "popup198",
            gridInfo: "",
            actionBy: "user134",
            startDate: "20141030",
            endDate: "20141120",
            allowTagType: "both",
            filterQuery: "",
            ruleEditorInfo: "",
            assemblyName: "",
            className: "",
            ruleEditorServiceUrl: "",
            CalendarDateFormat: "MM/dd/yyyy",
            isSaveTagClicked: false,
            IsBindGrid: true,
            getAllTags: false,
            ExternalFunction: function () { },
			ruleInfoForThisTag: "",
            ExternalFunctionForSaveTag: function (columnInfo) { }
        },
        _create: function () {
            var self = this;
            var that = self;
            var editingtag = false;
            //add one more dynamic property to check if it is persistent tag or not
            self.options.isPersistantTag = false;


            //to have a check if plugin is initialized or not
            var InitDiv = document.createElement("div");
            InitDiv.setAttribute("id", "initDiv");
            InitDiv.setAttribute("style", "display:none");
            this.element.append(InitDiv);

            /*self.socket = io.connect("http://localhost:5020/PositionMasterClients");
            self.socket.on('taskTriggered', function (data) {
            rad.TagManager.PingFromServer(data.message);
            $("#tag_config_form_header")[0].style.padding = "5px 0px 5px";
            $("#tag_config_form_header").prepend("<div id=\"msgDiv\">Tag has been executed successfully!! </div>");

            setTimeout(function () {
            $("#tag_config_form_header #msgDiv").fadeOut(2000);
            $("#tag_config_form_header")[0].removeChild($("#msgDiv")[0]);
            $("#tag_config_form_header")[0].style.padding = "25px 0px 5px";
            }, 5000);
            });
            */

            var tagNames = new Array();
            var tagNameExists, conflictFlag, confirmDialog = false;

            rad.TagManager.currRuleIdInEdit = "";
            rad.TagManager.editTagFlag, rad.TagManager.addTagFlag, rad.TagManager.EditRule, rad.TagManager.editTagDetailFlag = false;
            rad.TagManager.isPersistent, rad.TagManager.showIsPersistantCheckbox, rad.TagManager.toggleTag = true;
            var prevRuleSection, tag_name_attr, tag_name_val, prevRuleEditorDivId, toExpandRuleTxt, prevRuleFormDivId, toCollapseRuleTxt, prevExpandedTag = "";
            var tagInformation = new rad.TagManager.models.tagInfo();

            if (this.options.DialogTop == '')
                this.options.DialogTop = 15;
            var screenHeight = ($(window).outerHeight() - 65 - this.options.DialogTop - 89); //height of dialog window.innerHeight - 65 - this.options.DialogTop
            var screenHeightRight = ($(window).outerHeight() - 65 - this.options.DialogTop - 60);


            var currView = "LIST";

            //decribing the datatable
            var tagInfoColumns2 = [{ "data": "TagName",
                "sTitle": "Tag Name",
                "sWidth": ""
            },
                                 { "data": "TagDesc",
                                     "sTitle": "Tag Description",
                                     "sWidth": ""
                                 },
                                  { "data": "ReferenceDimension",
                                      "sTitle": "Ref Dimension",
                                      "sWidth": ""
                                  },
                                  { "data": "ReferenceAttribute",
                                      "sTitle": "Ref Attribute",
                                      "sWidth": ""
                                  },
                                  { "data": "DefaultValue",
                                      "sTitle": "Default Value",
                                      "sWidth": ""
                                  },
                                  { "data": "CustTagPriority",
                                      "sTitle": "Priority",
                                      "sWidth": "40px"
                                  },
                                  { "data": "Edit",
                                      "sTitle": " ",
                                      "sWidth": "1%"
                                  },
                                  { "data": "Delete",
                                      "sTitle": " ",
                                      "sWidth": "1%"
                                  }];

            ko.observableArray.fn.subscribeArrayChanged = function (addCallback, deleteCallback) {
                var previousValue = undefined;
                this.subscribe(function (_previousValue) {
                    previousValue = _previousValue.slice(0);
                }, undefined, 'beforeChange');
                this.subscribe(function (latestValue) {
                    var editScript = ko.utils.compareArrays(previousValue, latestValue);
                    for (var i = 0, j = editScript.length; i < j; i++) {
                        switch (editScript[i].status) {
                            case "retained":
                                break;
                            case "deleted":
                                if (deleteCallback)
                                    deleteCallback(editScript[i].value);
                                break;
                            case "added":
                                if (addCallback)
                                    addCallback(editScript[i].value);
                                break;
                        }
                    }
                    previousValue = undefined;
                });
            };
            //onclick="rad.TagManager.DeletePopUpHandler(this);return false;"
            var delBtnSet = '<button id="DelTagBtn" class="custom_runTagBtn deleteBtnPopUp DelTagBtn" '
                             + ' style="margin-right: 5px; width: 72px;margin-left: 29%;" >'
                             + 'Delete</button>';
            //onclick="rad.TagManager.modalClose(\'.delete-dialog-close-btn\', false);return false;"
            var cancelBtn = '<button class="custom_runTagBtn cancelBtn cancelBtnPopUp"'
                             + ' style="color:#d45453 !important;font-weight:normal !important;margin-left: 0px;text-align: -webkit-left;" >Cancel</button>';
            var runTagsBtn = '<button id="runTagBtn" class="custom_runTagBtn runTagBtn" data-toggle="tooltip" data-placement="bottom"'
                             + 'title="Apply Tags" style="margin-right: 5px; width: 72px;margin-left: 25%;float:none !important;margin-top:5px">'
                             + 'Apply</button>';
            //onclick="rad.TagManager.UpdateHandler(this);return false;"
            var updateBTn = '<button id="updateBtn" class="custom_runTagBtn updateBtn" data-toggle="tooltip" data-placement="bottom"'
                             + 'title="Apply Tags" style="margin-right: 5px; width: 72px;margin-left: 26%;float:none !important;margin-top:5px" >'
                             + 'Update</button>';

            var conflictDialogHTML = '<div id="deleteContainer" class="deleteContainer" style="">'
                                   + '<div id="conflictListDiv" style="padding: 15px 13px 0px;color: rgb(157, 156, 156);text-align:center">Priority change leads to conflict/s. Cannot Update.<div></div></div>'
                                   + '<button id="conflictCloseBtn" class="custom_runTagBtn cancelBtn cancelBtnPopUp conflictCloseBtn" data-toggle="tooltip" data-placement="bottom"'
                                   + 'title="Cancel" style="color:#d45453!important;font-weight:normal !important;margin-left: 0px;text-align: -webkit-left;" >Ok</button>'
                                   + '</div>';

            var deleteTagDialogHTML = '<div id = "deleteContainer" class = "deleteContainer" style = "" >'
                                     + '<div id = "deleteBody"  class="delPopUpTagNameSpan">Are you sure to delete  </div >'
                                     + delBtnSet + cancelBtn + '<div id = "delTagChkList" style="padding:16px 13px 2px;text-align: center;" >'
                                     + '</div ></div >';

            var chkTagNameDialogHTML = '<div id = "deleteContainer" class = "deleteContainer" style = "" >'
                                     + '<div id = "deleteBody" class="delPopUpTagNameSpan" >Are you sure to delete  </div> '
                                     + updateBTn + '<button class="custom_runTagBtn cancelBtn cancelBtnPopUp updateCancelBtn" data-toggle="tooltip" data-placement="bottom"'
                                     + 'title="Cancel" style="color:#d45453 !important;font-weight:normal !important;margin-left: 0px;text-align: -webkit-left;" onclick="rad.TagManager.modalClose(\'.delete-dialog-close-btn\', false);return false;">Cancel</button>' + '</div><div id="TagNameChkList" style="padding: 6px 13px 0px;"></div>';

            var RunTagsDialogHTML = '<div id = "RunTagsContainer" class = "deleteContainer" style = "margin-bottom:5px" >'
                                  + '<div style="padding: 15px 13px 0px;color: rgb(157, 156, 156);text-align:center"><div style="" >Apply Tags for <b>[ Start Date - End Date ] </b> </div ><div style="text-align: center;height: 26px;padding: 4px 4px 3px 4px;margin: auto;margin-top:5px" class="runTagsDateControl" id="runTagsDateControl"></div></div>'
                                  + runTagsBtn + '<button id="runTagCloseBtn" class="custom_runTagBtn cancelBtn cancelBtnPopUp runTagCloseBtn" data-toggle="tooltip" data-placement="bottom"'
                                  + 'title="Cancel" style="color:#d45453!important;font-weight:normal !important;margin-left: 0px;text-align: -webkit-left;" >Cancel</button>'
                                  + '<div id = "runTagChkList" data-bind = "foreach: TagNamesCHKDEL" >  <b data-bind = "text:$data" >  </b >'
                                  + '</div >  </div >';


            rad.TagManager.EditDelPopupHandler = function (bootboxModalBody) {
                bootboxModalBody.find('.close').addClass("delete-dialog-close-btn");
                bootboxModalBody.parent().addClass("delete-modal-dialog position-del-dialog");
                bootboxModalBody.next('div').children().addClass("delete-dialog-btns");
                bootboxModalBody.next('div').addClass("delete-dialog-footer");
            };



            rad.TagManager.callInitializeTagManager = function (pageIdentifier, actionBy, grid_Info, filter_Query, assembly_Name, class_Name, date_columnName) {
                $.ajax({
                    url: self.options.serviceURL + '/InitializeTagManager',
                    type: 'POST',
                    contentType: "application/json",
                    data: JSON.stringify({ identifier: pageIdentifier, user: actionBy, gridInfo: grid_Info, filterquery: JSON.stringify(filter_Query), assemblyName: assembly_Name, className: class_Name, dateColumnName: date_columnName, getAllTags: self.options.getAllTags ,isBindGrid:self.options.IsBindGrid}),
                    dataType: 'json'
                }).then(function (responseText) {
                    if (self.options.IsBindGrid)
                        $find(self.options.gridInfo.GridId).refreshGrid();
                }).fail(function (a) {
                    console.log("Initialization Failed:" + JSON.stringify(a));
                });
            };


            rad.TagManager.RunQuitBtnClick = function () {
                console.log("You clicked run");
            }


            // Rendering: to get tag config html and intialize the popup                 
            self.render = function (ele, options) {
                $.ajax({
                    url: self.options.serviceURL + '/GetTagTemplate',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json'
                }).then(function (responseText) {
                    self.initPopUp(responseText);
                }).fail(function (a) {
                    alert(a);
                });
            };
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            self.initPopUp = function (responseText) {
                // Initializing the popup
                var htmlToRender = responseText.d;
                //                if (!self.options.IsBindGrid)
                //                    htmlToRender.find("#runQuitBtn").css({ 'display': 'none' });
                $(".bootbox.modal").remove();
                var box = bootbox.dialog({
                    message: htmlToRender,
                    classNAme: "tagging-bootbox my-modal-dialog",
                    show: false
                });

                this.options.box = box;
                var modalContentHeight = $(".my-modal-content").height() - 60;

                $(function () {
                    $("[data-toggle='tooltip']").tooltip();
                    $(".tagTypeDiv .tagTypeEditBtn").click(function (e) {
                        e.stopPropagation();
                        $("#tagTypeDiv").show();
                    });

                    $(document).click(function (e) {
                        if ($(e.target).closest('.tagTypeDiv').length === 0 && $(e.target).attr('id') != 'tagTypeEditBtn')
                            $(".tagTypeDiv").hide();
                        else if ($(e.target).closest('.tagTypeDiv').length > 0)
                            $("#tagTypeDiv").show();
                    });
                    //initializing dropdowns 
                    rad.TagManager.InitializeRunQtip();
                    rad.TagManager.initDropdowns();
                    if (self.options.allowTagType == "both" || self.options.allowTagType == "persistant") {
                        self.options.ShowDataSet = true;
                    }
                });


                var tagFilterInfo = new rad.TagManager.models.TagFilterInfo(); // Binding tag list
                tagFilterInfo.SearchTerm = "%";
                tagFilterInfo.CurrentPageIndex = 1;
                rad.TagManager.bindTagList(tagFilterInfo, false);


                //minor changes //in css
                $(".tag_config_div").closest(".bootbox-body")[0].style.height = "100%";
                var bootboxBodyParent = $('.bootbox-body').parent();
                var bootboxBodyGrandParent = bootboxBodyParent.parent();
                bootboxBodyParent.addClass("my-custom-modal-body");
                bootboxBodyGrandParent.addClass("my-modal-content");
                bootboxBodyGrandParent.parent().addClass("tagging-bootbox my-modal-dialog");
                $(document).find(".close").addClass("custom-close-btn");
                $(".modal-backdrop").attr("onclick", "rad.TagManager.modalClose('.bootbox-close-button');");
                $(".tag_config_list_body").css({ "max-height": (screenHeight) });
                $(".rightContentDiv").css({ "max-height": screenHeightRight });
                $(".tag_config_form").hide();
                $(".my-modal-dialog")[0].style.top = this.options.DialogTop + "px";
                $(".my-modal-dialog").height(($(window).outerHeight() - 65 - this.options.DialogTop));
                console.log("the width is " + ($(".bootbox-body")[0].offsetWidth - 100));

                if (this.options.show) {
                    this.options.show = false;
                    box.modal('show');
                }
                //setting the header
                if (this.options.tagHeading != "")
                    $(".tag_config_heading_span")[0].innerText = this.options.tagHeading;


                $("#tp-is-dimension").show();

                //in case the oprion is set //Polaris requirement
                if (this.options.isPersistantTextBoxRequired) {
                    $("#tp-is-persistant").removeAttr("checked");
                    $("#tp-is-persistant,.tp-is-persistant-div").show();
                }
                else {
                    $("#tp-is-persistant").removeAttr("checked");
                    $("#tp-is-persistant,.tp-is-persistant-div").hide();
                }
            };


            this.options.identifier = this.element.attr("id");
            self.render(this.element, self.options);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //to initialize the run qtip
            rad.TagManager.InitializeRunQtip = function () {
                $("#runQuitBtn").qtip({                 // run tag popup
                    content: RunTagsDialogHTML,
                    show: 'click',
                    overwrite: false,
                    hide: 'false',
                    style: {
                        classes: 'qtip-light qtip-shadow'
                    },
                    events: {
                        show: function (event, api) {
                            console.log("I am trying to show it");

                            $(".runTagsDateControl").dateControl().dateControl("destroy");
                            $(".runTagsDateControl").dateControl({
                                /*$("#runQuitBtn")*/formatter: "iago", startDate: "12/31/2014", endDate: "12/31/2014"
                            });    // intialize date control for run tag popup

                            //UI Changes
                            //$(document).find(".opensright").removeClass("opensright").addClass("opensleft");
                            //$("#runTagsDateControl_dateInfo.opensleft").prepend($("#runTagsDateControl_dateInfo .calendar.left"));
                            $(".runQuitBtn").css({ "cursor": "not-allowed", "opacity": "0.5" });
                            $(".tag_config_list_container").addClass("tag_config_list_container_fade");

                            $('.runTagCloseBtn', api.elements.content).click(function (e) {        // cancel button on qtip
                                api.hide(e);
                                //$(".runQuitBtn").removeAttr("disabled");
                                $(".runQuitBtn").css({ "cursor": "pointer", "opacity": "1" });
                                $(".tag_config_list_container").removeClass("tag_config_list_container_fade");
                            });

                            //$('.runTagBtn', api.elements.content).unbind("click");
                            $('.runTagBtn', api.elements.content).click(function (e) {            // apply btn on qtip
                                api.hide(e);
                                rad.TagManager.RunTagsHandler();
                                //$(".runQuitBtn").removeAttr("disabled");
                                $(".runQuitBtn").css({ "cursor": "pointer", "opacity": "1" });
                                $(".tag_config_list_container").removeClass("tag_config_list_container_fade");
                                //api.destroy();
                                $(".runTagsDateControl").dateControl().dateControl("destroy");
                                $(".runTagsDateControl").dateControl({
                                    formatter: "iago"
                                });
                                bootbox.hideAll();
                            });
                        }
                    },
                    position: {
                        my: 'top center',  // Position my top left...
                        at: 'bottom center'  // at the bottom right of...
                    }
                });
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            rad.TagManager.initDropdowns = function () {
                //initializing dropdowns in the tag configuration form
                $("#negative-select").select2({
                    formatSelection: rad.TagManager.format,
                    minimumResultsForSearch: -1
                });
                var data = [{ id: "String", text: 'String' }, { id: "Number", text: 'Number' }, { id: "Currency", text: 'Currency' }, { id: "Precentage", text: 'Precentage'}];
                // $("#tagTypeSelect").select2({
                // data : data,
                // minimumResultsForSearch: -1
                // });
                //$("#tagTypeSelect").select2('val','String');
                $("#units-select").select2({
                    minimumResultsForSearch: -1
                });
            };
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            rad.TagManager.PingFromServer = function (message) {
                console.log("Message : " + message + " reached successfully!");
            }


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            rad.TagManager.bindTagList = function (tagFilterInfo, hasBinding) {
                //binding the tag info and making the side tiles
                //making all the information required
                //filling tag ingo array//// and bla bla
                var tag_FilterInfo = JSON.stringify(tagFilterInfo);
                //
                if (self.options.gridInfo.RequireTagging) {
                    $.ajax({
                        url: self.options.serviceURL + '/GetTagList',
                        type: 'POST',
                        data: JSON.stringify({ searchFilterInfo: JSON.stringify(tagFilterInfo), identifier: self.options.pageIdentifier, user: self.options.actionBy, getAllTags: self.options.getAllTags }),
                        contentType: "application/json",
                        dataType: 'json'

                    }).then(function (responseText) {

                        var data = JSON.parse(responseText.d);
                        // console.log("taglist:");
                        // console.log(data);
                        if (self.options.gridInfo.ComputedNewColumns === undefined)
                            self.options.gridInfo.ComputedNewColumns = [];
                        if (data != null) {

                            for (var i = 0; i < data.tagList.length; i++) {
                                tagNames.push(data.tagList[i].TagName);
                                //push column Names in gridinfo
                                self.options.gridInfo.ComputedNewColumns.push(data.tagList[i].TagRealName);

                            }
                            //call grid service by then 
                            rad.TagManager.callInitializeTagManagerWithoutBindings(self.options.pageIdentifier, self.options.actionBy, JSON.stringify(self.options.gridInfo), self.options.filterQuery, self.options.assemblyName, self.options.className, self.options.DateColumnName); // Initialize Tag Manager //D:\\RAD_WORK\\Polaris.UI\\PolarisUI\\bin\\PPnL.PositionMaster.dll //PPnL.PositionMaster.TagServiceImplemantor

                            if ((data == null)) {
                                return;
                            }
                            rad.TagManager.TagModel = new rad.TagManager.viewModel(data);
                            rad.TagManager.bindDemoTable();
                            ko.cleanNode(document.getElementById("gridviewDiv"));
                            ko.applyBindings(rad.TagManager.TagModel, document.getElementById("gridviewDiv"));
                        }
                    }).fail(function (a) {
                        alert(a);
                    });
                }
                else {
                    rad.TagManager.callInitializeTagManagerWithoutBindings(self.options.pageIdentifier, self.options.actionBy, JSON.stringify(self.options.gridInfo), self.options.filterQuery, self.options.assemblyName, self.options.className, self.options.DateColumnName); // Initialize Tag Manager //D:\\RAD_WORK\\Polaris.UI\\PolarisUI\\bin\\PPnL.PositionMaster.dll //PPnL.PositionMaster.TagServiceImplemantor
                }
            };



            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            rad.TagManager.callInitializeTagManagerWithoutBindings = function (pageIdentifier, actionBy, grid_Info, filter_Query, assembly_Name, class_Name, date_columnName) {

                $.ajax({
                    url: self.options.serviceURL + '/InitializeTagManager',
                    type: 'POST',
                    contentType: "application/json",
                    data: JSON.stringify({ identifier: pageIdentifier, user: actionBy, gridInfo: grid_Info, filterquery: JSON.stringify(filter_Query), assemblyName: assembly_Name, className: class_Name, dateColumnName: date_columnName, getAllTags: self.options.getAllTags, isBindGrid: self.options.IsBindGrid }),
                    dataType: 'json'
                }).then(function (responseText) {
                    if (self.options.IsBindGrid)
                        neogridloader.create(self.options.gridInfo.GridId, self.options.actionBy, self.options.gridInfo, "");
                    else
                        $("#" + self.element.attr("id")).tagging("show");
                    //$find(self.options.gridInfo.GridId).refreshGrid();
                }).fail(function (a) {
                    console.log("Initialization Failed:" + JSON.stringify(a));
                });
            };

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
            rad.TagManager.viewModel = function (data) {
                //so that we can attach our things to this object
                var self = this;

                self.tagInfo = ko.observableArray([]);
                self.query = ko.observable("");
                self.RuleInfo = ko.observableArray([]);
                self.RuleFilterInfo = ko.observableArray([]);
                self.TagFilterInfo = ko.observableArray([]);

                if (data.tagList) {
                    for (var i = 0; i < data.tagList.length; i++) {
                        data.tagList[i].Edit = '<div class="form-element tag-list-col-icon col-edit fa fa-pencil-square-o" style="margin-top: 4px !important;color: black !important; display: inline-block;font-size:15px" value="' + data.tagList[i].TagId + '" tag-name="' + data.tagList[i].TagName + '" title="Edit Tag" onclick="var event = arguments[0] || window.event;event.stopPropagation;rad.TagManager.editTag(this);return false;"></div>';
                        data.tagList[i].Delete = '<div class="form-element tag-list-col-icon col-delete fa fa-trash-o" style="margin-top: 4px !important;color: black !important;font-size:15px; display: inline-block;" id="dt_del_tag_btn_' + data.tagList[i].TagId + '" value="' + data.tagList[i].TagId + '" tag-name="' + data.tagList[i].TagName + '" onclick="var event = arguments[0] || window.event;event.stopPropagation; rad.TagManager.deleteTagData(this,event);" title="Delete Tag"></div>';
                        data.tagList[i].CustTagPriority = '<div class="form-element tag-list-col cust_tag_prior prior_align" title="Tag Priority" style="">' + data.tagList[i].TagPriority + '</div>'

                        if ((that.options.allowTagType == "both" || that.options.allowTagType == "persistant") && (data.tagList[i].IsPersistant == true)) {
                            data.tagList[i].TagIsEditable = true;
                        }
                        else if ((that.options.allowTagType == "nonpersistant") && (data.tagList[i].IsPersistant == true))
                            data.tagList[i].TagIsEditable = false;
                        else if (data.tagList[i].IsPersistant == false) {
                            if (data.tagList[i].IsEditable)
                                data.tagList[i].TagIsEditable = true;
                            else
                                data.tagList[i].TagIsEditable = false;
                        }
                        //Rajul Mittal
                        rad.TagManager.tagPriorties.push(data.tagList[i].TagPriority);
                    }

                    self.tagInfo($.map(data.tagList, function (individualTag) {
                        return new rad.TagManager.tag(individualTag);
                    }));

                    //#changed Rajul Mittal
                    /*for (var i = 0; i < data.tagList.length; i++)
                    rad.TagManager.tagPriorties.push(data.tagList[i].TagPriority);*/

                    self.filteredValues = ko.dependentObservable(function () {
                        var search = self.query().toLowerCase();
                        return ko.utils.arrayFilter(self.tagInfo(), function (elem) {
                            return (elem.TagName().toLowerCase().indexOf(search) >= 0) || (elem.MappedAttributes().toLowerCase().indexOf(search) >= 0);
                        });
                    });

                    self.filteredValues.subscribe(function () {
                        console.log("search changed");
                        $(".selected_div").addClass("selected_div_fixed_collpased");
                    });
                }
                if (data.ruleList) {
                    //filling the data in ruleInfo
                    self.RuleInfo($.map(data.ruleList, function (individualRule) {
                        return new rad.TagManager.rule(individualRule);
                    }));

                    for (var i = 0; i < data.ruleList.length; i++)
                        rad.TagManager.rulePriorties.push({
                            tagId: data.ruleList[i].TagId,
                            priority: data.ruleList[i].Priority,
                            isOverride: data.ruleList[i].IsOverride
                        });
                }
            };


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////


            rad.TagManager.bindDemoTable = function () {


                //if (typeof demoTable == 'undefined') {
                if ($.fn.dataTable.isDataTable('#tag_config_list_body2')) {
                    demoTable = $('#tag_config_list_body2').DataTable();
                }
                else {
                    for (var i = 0; i < rad.TagManager.TagModel.tagInfo().length; i++) {
                        if (rad.TagManager.TagModel.tagInfo()[i].TagIsEditable() == false) {
                            rad.TagManager.TagModel.tagInfo()[i].Edit = "";
                        }
                    }
                    demoTable = $("#tag_config_list_body2").DataTable({
                        "sScrollY": "500px",
                        "data": rad.TagManager.TagModel.tagInfo(),
                        "columns": tagInfoColumns2,
                        "bsort": true,
                        "searching": true,
                        "paging": false,
                        "bInfo": false,
                        "fnDrawCallback ": function (nHead, aData, iStart, iEnd, aiDisplay) {
                            $(".tag_config_list_body2_div").find(".dataTable").addClass("datatable_header");
                        },
                        "createdRow": function (row, data, index) {
                            $(row).addClass('datatable_row_cust');
                            $(row).attr('id', 'tag_list_item_' + data.TagId());
                            $(row).attr('value', data.TagId());
                            $(row).attr('tag_name', data.TagName());
                            $(row).attr('onclick', 'rad.TagManager.toggleRuleSection(this)');
                        },
                        "aaSorting": [[5, 'asc']],
                        "columnDefs": [{ className: "prior_align", "targets": [5] },
                                    { className: "datatableEdit", "targets": [6] },
                                    { className: "datatableDel", "targets": [7] },
                                    { className: "datatableDefVal", "targets": [4] },
                                    { width: "1%", "targets": [6, 7] },
                                    { "orderable": false, "targets": [6, 7]}]
                    });

                }
                //demoTable.destroy();

                $(".tag_config_list_body2_div").find(".dataTables_scrollHeadInner .dataTable").addClass("datatable_header");

                var screenHeightDT = screenHeight - 37;
                $(".tag_config_list_body2_div .dataTables_scrollBody").css({ "max-height": screenHeightDT });
                //create tag //
                /*var iTag = document.createElement("i");
                iTag.className = "fa fa-search SearchIcon";
                //appending the image of searcher
                $("#tag_config_list_body2_filter").appendChild(iTag);
                $("#tag_config_list_body2_filter").click(function (e) {
                if (e.target.className == "dataTables_filter TextBoxHidden") //if className is this that means search is close
                e.target.className = "dataTables_filter TextBoxHidden TextBoxShown";
                else if (e.target.className == "dataTables_filter TextBoxHidden TextBoxShown")
                e.target.className = "dataTables_filter TextBoxHidden";

                });*/


                var DOMINPUT = $("#tag_config_list_body2_filter input");
                DOMINPUT.addClass("taglistSearchInpt");
                DOMINPUT.attr('placeholder', 'Search'); //saving the input

                var ParentDOMInput = $("#tag_config_list_body2_filter label").parent()[0]; //saving the parent

                var iTag = document.createElement("i"); //making new itag
                iTag.className = "fa fa-search SearchIcon";
                $(iTag).bind("click", function (e) {
                    console.log("Hey I am clicked " + e.target.id);
                    if (e.target.parentNode.className == "dataTables_filter SearchBoxHidden")
                        e.target.parentNode.className = "dataTables_filter SearchBoxHidden SearchBoxVisible";
                    else if (e.target.parentNode.className == "dataTables_filter SearchBoxHidden SearchBoxVisible")
                        e.target.parentNode.className = "dataTables_filter SearchBoxHidden";
                });


                $("#tag_config_list_body2_filter").html(""); // innerHtml = ""; //html of the parent div becomes empty
                $("#tag_config_list_body2_filter").addClass("SearchBoxHidden");

                if (ParentDOMInput != undefined) {
                    ParentDOMInput.appendChild(iTag);
                    ParentDOMInput.appendChild(DOMINPUT[0]);
                }
                $("#tag_config_list_body2_wrapper").prepend(ParentDOMInput);
                $(".datatable_header").find(".datatableDefVal").removeClass("datatableDefVal");
                rad.TagManager.TagModel.tagInfo.subscribeArrayChanged(
                    function (addedItem) {
                        demoTable.row.add(addedItem).draw();
                    },
                    function (deletedItem) {
                        var rowIdx = demoTable.column(0).data().indexOf(deletedItem.TagName());
                        demoTable.row(rowIdx).remove().draw();
                    }
                  );
            };


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////     

            rad.TagManager.showAddTagPanel = function () {
                // Handling Add New Tag Configuartion Form             
                rad.TagManager.addTagFlag = true;
                rad.TagManager.editTagFlag = false;
                editingtag = false;
				$(".btnSaveTag").css({ 'pointer-events': '' })
				$("#tp-is-dimension").parent().hide()
                //to reset all the edit buttons
                rad.TagManager.resetAllEditButtons(false);
                self.tagIdentifier = -1;
                //get the tag ID of currenlty opened tag
                var tagID;
                $(".tagTypeEditBtn,.tag-expand-div,.tag_activity_div").hide(); //,.tag-detail-colon
                //hiding means clearing its InnerHTml
                $.each($(".tag-detail-colon"), function (e, elem) { elem.innerText = ""; });
                //removing the class and also making the display of arrow none
                if ($(".tag_config_list_body").find(".selected_div_bckgrnd").length > 0) {
                    tagID = $($(".tag_config_list_body").find(".selected_div_bckgrnd").children()[0]).attr("value");
                    $($(".tag_config_list_body").find(".selected_div_bckgrnd").children()[0]).children().removeClass("Inner_Selected_Tile");
                    $($(".tag_config_list_body").find(".selected_div_bckgrnd").children()[0]).removeClass("selected_Inner_Div");
                    $(".tag_config_list_body").find(".selected_div_bckgrnd").children(".tag_tile_arrow")[0].style.display = "none";
                    //selected_Inner_Div
                    $(".tag_config_list_body").find(".selected_div_bckgrnd").removeClass("selected_div_bckgrnd");
                    //empty the content of rule engine block and keep it saved
                    //var listItem = $("div#tag_list_item_" + tagID);
                    var prevRuleSection = $("#rule_section_" + tagID);
                    if (prevRuleSection != "" && prevRuleSection != undefined && prevRuleSection[0] != "" && prevRuleSection[0] != undefined) {
                        //listItem.parent().append(prevRuleSection);
                        //search tagListItem
                        prevRuleSection[0].style.display = "none";
                        var prevTagID = prevRuleSection.attr("tagID");
                        $("div#tag_list_item_" + prevTagID).parent().append(prevRuleSection);
                    }
                    $(".tag_expand_div_content").html('');
                }

                if (self.options.ShowDataSet) {
                    $("#PMDataSetContainer").hide();
                }

                //resetting the clikced attribute
                if ($("#tp-is-persistant").attr("clicked")) {
                    $("#tp-is-persistant")[0].setAttribute("clicked", false);
                }

                //resetting all default values
                rad.TagManager.resetTagForm(tagID);                                              // reset the add tag form
                $(".spinner").spinner();
                var operation = "show";
                rad.TagManager.getReferenceDimension("-1", -1, operation);
                //#ToBeAddedHere Rajul Mittal
                /*  if (self.options.pageIdentifier == "pnl_positiondetails" || self.options.pageIdentifier == "pnl_positionallocation") {
                if ($("#tp-is-persistant").attr('checked'))
                rad.TagManager.getDataSet(-1);
                }*/


                //rad.TagManager.getReferenceData("-1", "-1", "", -1);
                //to set the priority on its own
                rad.TagManager.setMaxPriorityForTag();
                rad.TagManager.initRefDimRefAttrDropdowns(-1);

                $("div[id^=ruleForm_]").slideUp();                                         // hide any previously opened rule forms                
                $("div[id^=editRule_]").slideUp();

                if ($(".tag_header_view_iconSpan").find(".fa-th-large").length > 0) {     // check for the curr view if it is grid or list
                    currView = "LIST";
                    rad.TagManager.setGridView();
                } else {
                    currView = "GRID";
                }

                rad.TagManager.setGridViewClasses();

                switch (self.options.allowTagType) {
                    case 'both': rad.TagManager.isPersistent = true; rad.TagManager.showIsPersistantCheckbox = true; break;
                    case 'persistant': rad.TagManager.isPersistent = true; rad.TagManager.showIsPersistantCheckbox = false; break;
                    case 'nonpersistant': rad.TagManager.isPersistent = false; rad.TagManager.showIsPersistantCheckbox = false; break;
                }

                //change the content to show at the top
                $(".tag_config_form_header,.addTagTitle").show();                         // ADD TAG title in header
                $(".tagNameHolderSpan_div,.editTagDetails").hide();

                if (self.options.isPersistantTextBoxRequired) {
                    $("#tp-is-persistant").removeAttr("checked");
                    $("#tp-is-persistant,.tp-is-persistant-div").show();
                }

                else {
                    $("#tp-is-persistant").removeAttr("checked");
                    $("#tp-is-persistant,.tp-is-persistant-div").hide();
                }
                $(".tag_config_form").slideDown();                                                                        // show add tag form
                $(".tag_header_right_btnset").addClass("tag_header_right_btnset_after");                                  // main header-adjust css (setting position for add tag, view, activity icon)
                $('a[controlType="closealert"]').unbind().bind("click", rad.TagManager.closeAlertManager);
                $(".tPrecision").val('0');
                //$("#tp-tag-default-value-container--1").empty();
                $("#tp-tag-default-value-container--1").append('<input id="tp-tag-default-value" type="text" class="form-control input-sm"/>');

            };


            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////          
            //called on editTagDetails, AddNewTag
            rad.TagManager.resetAllEditButtons = function (flag) {
                $("#tagAlertMessanger").hide(); $("#tagAlertMessanger2").hide(); $(".err-input").removeClass("err-input")
                $("#tp-tagname, #tp-tagdesc, #tp-reference-dimension-container--1, #tp-reference-attribute-container--1, #tp-tag-default-value-container--1,#tp_tag_dataset_container__1, #s2id_tagTypeSelect,#tp-priority ").fadeIn(1000, function (e) { console.log(e); });
                //$("#tp-reference-dimension-container--1").html("-");
                $("#tp-priority").closest(".tag_config_inputs").fadeIn(1000); //closest spinner
                //this is to make the content of each empty                
                $.each($(".tag-detail-colon"), function (e, elem) { elem.innerText = ""; });
                //that edit image is hidden
                $(".config_form_span,.editTagDetails").hide(); //,.tag-detail-colon

                //setting maximum height
                var tag_config_sec_height = $(".tag_config_form").height() + $("#tag_config_form_header")[0].offsetHeight + 90;
                if (tag_config_sec_height == 0) //in case it is in edit mode in different way
                    var screenHeightRuleSec = screenHeightRight - tag_config_sec_height - 159 - 175; //just a guess of 175
                else
                    var screenHeightRuleSec = screenHeightRight - tag_config_sec_height - 25;
                console.log("max height is :" + screenHeightRuleSec);
                //change the class if needed
                if ($(".tag_config_collapsed").length > 0) {
                    $(".tag_config_collapsed").removeClass("tag_config_collapsed").addClass("tag_config_expanded");
                }

                //enable checkboxes
                $("#tp-is-dimension").attr("disabled", false);
                if (flag == true)
                    $("#tp-is-persistant").attr("disabled", true);
                else
                    $("#tp-is-persistant").attr("disabled", false);

                $(".tag_config_expanded").css({ "max-height": screenHeightRuleSec });
                console.log("expanded set " + screenHeightRuleSec)
                console.log("Hey I am resetting it " + screenHeightRuleSec + "_" + tag_config_sec_height + "__" + screenHeight);

                //showing save and cancel buttons
                $("#tagTypeSelect").select2('val', $("#tagTypeSpan").html());
                $(".tag_config_form_btnset").show();


                $(" #tp-reference-attribute-container--1").closest(".form-element").css({ 'pointer-events': '' });
                $(" #s2id_tagTypeSelect").closest(".form-element").css({ 'pointer-events': '' });
            }

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            //on addnew or modal close
            rad.TagManager.resetTagForm = function (tagID) {
                if ($(".err-validation")) {
                    $(".err-validation").hide();

                }
                $("#tp-tagname,#tp-tagdesc,#tp-tag-default-value").val("");
                $("#tp-reference-dimension-").val("-");
                //$("#tp-reference-dimension-container").html("-");
                $("#tp-reference-attribute-container").html("Select entity type first !");
                $("#tp-is-dimension").show();

                $("#tp-reference-dimension-" + tagID).select2("val", "-");
                $("#tp-reference-attribute-" + tagID).select2("val", "-1");
                if ($("#tp-tag-default-value-" + tagID).length > 0) {
                    $("#tp-tag-default-value-" + tagID).select2("val", "-1");
                }

                if (self.options.isPersistantTextBoxRequired) {
                    $("#tp-is-persistant").removeAttr("checked");
                    $("#tp-is-persistant,.tp-is-persistant-div").show();
                }
                else {
                    $("#tp-is-persistant").removeAttr("checked");
                    $("#tp-is-persistant,.tp-is-persistant-div").hide();
                }

                if (self.options.allowTagType == "both" || self.options.allowTagType == "persistant") {
                    $("#tp-dataSet-" + tagID).select2("val", "-1");
                }

                $("#tp-is-dimension,#tp-is-persistant").prop('checked', false);
                $(".btnSaveTag").attr("actiontype", "insert");
                $(".btnSaveTag").attr("tagid", "-1");
            };
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////      

            rad.TagManager.getReferenceDimension = function (selectedEntityTypeName, tagId, operation) {
                $.ajax({
                    url: self.options.serviceURL + '/GetEntityTypeListCombo',
                    type: 'POST',
                    data: JSON.stringify({ entityTypeSelectedValue: selectedEntityTypeName, tagId: tagId, identifier: self.options.pageIdentifier, user: self.options.actionBy }),
                    contentType: "application/json",
                    dataType: 'json'
                }).then(function (responseText) {
                    //var data = JSON.parse(responseText.d);
                    $("#tp-reference-dimension-container--1").html(responseText.d);
                    // $("#tp-reference-dimension-span").html(responseText.d);
                    $("#tp-reference-dimension-" + tagId).unbind().bind("change", function () {
                        rad.TagManager.getReferenceAttribute($("#tp-reference-dimension-" + tagId).val(), "-1", tagId);
                        rad.TagManager.getReferenceData($("#tp-reference-dimension-" + tagId).val(), "", "", tagId);
                    });
                    if (operation == "show") {
                        rad.TagManager.getReferenceAttribute($("#tp-reference-dimension-" + tagId).val(), "-1", tagId);
                        rad.TagManager.getReferenceData($("#tp-reference-dimension-" + tagId).val(), "", "", tagId);
                    }
                    //rad.TagManager.initRefDimRefAttrDropdowns(tagId);
                    //applying select2 to this dropdown
                    $("#tp-reference-dimension-" + tagId).select2({ minimumResultsForSearch: -1 });
                    //$("#tp-reference-dimension-" + tagId).select2('val','-');
                    //$("#tp-reference-dimension-" + tagId).val('-');
                    //$("#tagTypeSelect").select2("destroy");
                    var display = $("#tagTypeSelect").prev().css('display');
                    $("#tagTypeSelect").select2({
                        data: [{ id: "String", text: 'String' }, { id: "Number", text: 'Number' }, { id: "Currency", text: 'Currency' }, { id: "Percentage", text: 'Percentage'}],
                        minimumResultsForSearch: -1
                    });
                    $("#operationSelect").select2({
                        data: [{ id: "None", text: 'None' }, { id: "Min", text: 'Min' }, { id: "Max", text: 'Max' }, { id: "Count", text: 'Count' }, { id: "Sum", text: 'Sum' },
                         { id: "Average", text: 'Average'}],
                        minimumResultsForSearch: -1
                    });
                    $("#operationSelect").select2("val", "Max");
                    if (tagId != -1) {
                        if (operation == "update") {
                            $("#tagTypeSelect").prev().css({ 'display': '' }); //changed 
                            display = "none";
                            if (display == 'none') {//changed
                                $("#tagTypeSelect").prev().css({ 'display': '' });
                            }
                            //$("#tagTypeSelect").prev().select2('val',$("#s2id_tagTypeSelect").text());
                        }

                        else {
                            $("#tagTypeSelect").prev().css({ 'display': 'none' }); //changed 
                            display = "none";
                            if (display == 'none') {//changed
                                $("#tagTypeSelect").prev().css({ 'display': 'none' });
                            }
                        }
                        if ($("#tagTypeSelect").prev().prev().attr('id') === 's2id_tagTypeSelect') {
                            $("#tagTypeSelect").prev().prev().remove();
                        }
                        if (display == 'none') {//changed
                            $("#tagTypeSelect").prev().css({ 'display': '' });
                        }
                        if (operation === "showtagdetails") {
                            $("#tagTypeSelect").prev().css({ 'display': 'none' });
                        }
                    }

                    else {
                        if (operation === "show") {
                            if ($("#tagTypeSelect").prev().prev().attr('id') === 's2id_tagTypeSelect') {
                                $("#tagTypeSelect").prev().prev().remove();
                            }
                            $("#tagTypeSelect").prev().css({ 'display': '' });
                            $("#tagTypeSelect").prev().select2('val', 'String');
                        }
                        else if (operation === "showtagdetails") {
                            $("#tagTypeSelect").prev().css({ 'display': '' });
                            //$("#tagTypeSelect").prev().select2('val',$("#s2id_tagTypeSelect").text());

                        }
                        else if (operation === "update") {
                            $("#tagTypeSelect").prev().css({ 'display': '' });
                            //$("#tagTypeSelect").prev().select2('val',$("#s2id_tagTypeSelect").text());

                        }
                    }
                    if (operation != "show") {
                        $("#tagTypeSelect").prev().select2('val', $('#tagTypeSpan').html());
                    }
                    $("#tagTypeSelect").on('change', function (e) {
                        rad.TagManager.tagTypeHandler(e.target);
                    });

                }).fail(function (a) {
                    alert(a);
                });
            };
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            rad.TagManager.getDataSet = function (tagId) {
                $.ajax({
                    url: self.options.serviceURL + '/GetSystemTableList',
                    type: 'POST',
                    data: JSON.stringify({ tagId: tagId, identifier: self.options.pageIdentifier, user: self.options.actionBy }),
                    contentType: "application/json",
                    dataType: 'json'
                }).then(function (responseText) {
                    $("#tp_tag_dataset_container__1").html(responseText.d);
                    var selectedCount = $("#tp_tag_dataset_container__1").find("option[selected=true]");
                    var selectedOne = selectedCount[selectedCount.length - 1];
                    if (selectedOne != undefined) {
                        $("#tp_tag_dataSet_span").html(selectedOne.innerText);
                    }
                    $("#tp_dataSet_" + tagId).select2({ minimumResultsForSearch: -1 });
                    return selectedOne;
                }).fail(function (a) {
                    alert(a);
                });
            };
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            rad.TagManager.setMaxPriorityForTag = function () {

                $.ajax({
                    url: self.options.serviceURL + '/GetMaxPriorityForTag',
                    type: 'POST',
                    data: JSON.stringify({ isPersistant: rad.TagManager.isPersistent, identifier: self.options.pageIdentifier }),
                    contentType: "application/json",
                    dataType: 'json'
                }).then(function (responseText) {
                    // var data = JSON.parse(responseText.d);
                    $("#tp-priority").val(responseText.d);
                    //        $("#tp-priority").attr("max_priority", a);
                    $("#tp-priority").attr("value", responseText.d);
                }).fail(function (a) {
                    alert(a);
                });
            };
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            rad.TagManager.initRefDimRefAttrDropdowns = function (tagId) {

                $("#tp-reference-dimension-" + tagId).select2({ minimumResultsForSearch: -1 });
                $("#tp-reference-attribute-" + tagId).select2({ minimumResultsForSearch: -1 });
                $("select#tp-tag-default-value-" + tagId).select2({ minimumResultsForSearch: -1 });
            };

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            rad.TagManager.setGridView = function () {
                //to remove the background border b;lue
                if ($(".tag_config_list_body").find(".selected_Inner_Div").length > 0) {
                    $(".tag_config_list_body").find(".selected_Inner_Div").children().removeClass("Inner_Selected_Tile");
                    $(".tag_config_list_body").find(".selected_Inner_Div").siblings(".tag_tile_arrow")[0].style.display = "none";
                    $(".tag_config_list_body").find(".selected_Inner_Div").removeClass("selected_Inner_Div");
                    //tag_tile_arrow
                    //selected_Inner_Div
                }
                $(".tag_header_view_iconSpan").find(".fa-th-large").removeClass("fa-th-large").addClass("fa-th-list");  // toggle the view icon
                $(".tag_header_view_iconSpan").removeAttr('data-original-title');
                $(".tag_header_view_iconSpan").attr('data-original-title', 'List View');                                // set tooltip for respective icon
                $(".tag_config_list_body2_div").hide();                     // hide datatbles list view section
                $(".gridviewDiv").show();                                   //show grid view

            };
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            rad.TagManager.setGridViewClasses = function () {
                $(".gridViewSearchDiv").addClass("searchAftrToggle"); //wrapper of the textbox //gridViewSearchDiv
                $(".taglistSearchInpt").addClass("taglistSearchInptAfter");
                $(".searchIconGridView").show();
                $(".tag_configList_content").addClass("onToggleTag");  // resize to tiled view of tags
                $(".tag_config_content_body").addClass("tag_config_content_body_after");
                $(".gridviewDiv").addClass("gridviewDiv_after");
                $(".selected_div").addClass("selected_div_fixed_collpased");
            };
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            rad.TagManager.getReferenceAttribute = function (selectedEntityTypeName, selectedEntityTypeAttribute, tagId) {

                $.ajax({
                    url: self.options.serviceURL + '/GetEntityAttributeListCombo',
                    type: 'POST',
                    data: JSON.stringify({ entityTypeName: selectedEntityTypeName, attributeSelectedValue: selectedEntityTypeAttribute, tagId: tagId, identifier: self.options.pageIdentifier, user: self.options.actionBy }),
                    contentType: "application/json",
                    dataType: 'json'
                }).then(function (responseText) {

                    // var data = JSON.parse(responseText.d);
                    $("#tp-reference-attribute-container--1").html(responseText.d);
                    $("#tp-reference-attribute-" + tagId).unbind().bind("change", function () {
                        rad.TagManager.getReferenceData($("#tp-reference-dimension-" + tagId).val(), $("#tp-reference-attribute-" + tagId).val(), "-1", tagId);
                    });
                    //rad.TagManager.initRefDimRefAttrDropdowns(tagId);
                    //applying select2 on this dropdown
                    $("#tp-reference-attribute-" + tagId).select2({ minimumResultsForSearch: -1 });
                }).fail(function (a) {
                    alert(a);
                });
            };
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            rad.TagManager.getReferenceData = function (entityTypeName, entityAttributeName, entitySelectedValue, tagId) {

                $.ajax({
                    url: self.options.serviceURL + '/GetEntityValueList',
                    type: 'POST',
                    data: JSON.stringify({ entityTypeName: entityTypeName, entityAttributeName: entityAttributeName, entitySelectedValue: entitySelectedValue, tagId: tagId, identifier: self.options.pageIdentifier, user: self.options.actionBy }),
                    contentType: "application/json",
                    dataType: 'json'
                }).then(function (responseText) {

                    $("#tp-tag-default-value-container--1").html(responseText.d);
                    var selectedCount = $("#tp-tag-default-value-container--1").find("option[selected=true]");
                    var selectedOne = selectedCount[selectedCount.length - 1];
                    if (selectedOne != undefined) {
                        $("#tp-tag-default-span").html(selectedOne.innerText);
                    }
                    $("select#tp-tag-default-value-" + tagId).select2({ minimumResultsForSearch: -1 });
                }).fail(function (a) {
                    alert(a);
                });
                // rad.TagManager.initRefDimRefAttrDropdowns(tagId);
            };

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


            //on change of tabs
            rad.TagManager.ruleTabClickHandler = function (isOverride, elem) {
                // Normal rules and Override rules tab handling                     

                var tagId = $(elem).attr('value');
                var tabElem = elem;
                if ($(elem).prop("tagName") === "A")
                    tabElem = elem.parentNode;
                $(tabElem).removeClass("active").addClass("active");

                if (isOverride) {
                    $(tabElem).prev().removeClass("active");
                    $("#btnAddRule_" + tagId).attr("pol-isOverride", "1");
                    $("#ruleForm_" + tagId).attr("pol-isOverride", "1");
                    $("#rulelist_" + tagId).attr("pol-isOverride", "1");
                    $("#btnAddRule_" + tagId).html("Add Override Rule");
                    $("div[id^=ruleForm_]").hide();

                } else {
                    $(tabElem).next().removeClass("active");
                    $("#btnAddRule_" + tagId).attr("pol-isOverride", "0");
                    $("#ruleForm_" + tagId).attr("pol-isOverride", "0");
                    $("#rulelist_" + tagId).attr("pol-isOverride", "0");
                    $("#btnAddRule_" + tagId).html("Add Rule");
                    $("div[id^=ruleForm_]").hide();
                }

                rad.TagManager.bindRule(tagId, isOverride, false);
            };


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            rad.TagManager.bindRule = function (tagId, isOverride, IneditTag) {

                var ruleFilterInfo = new rad.TagManager.models.RuleFilterInfo();
                ruleFilterInfo.EffectiveStartDate = "";
                ruleFilterInfo.EffectiveEndDate = "";
                ruleFilterInfo.IsOverrideRule = isOverride;
                ruleFilterInfo.SearchTerm = "";
                ruleFilterInfo.CurrentPageIndex = 1;
                ruleFilterInfo.TagIds = tagId.toString();
                rad.TagManager.bindRuleList(ruleFilterInfo, IneditTag);
                return false;
            };

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            rad.TagManager.bindRuleList = function (ruleFilterInfo, IneditTag) {

                $.ajax({
                    url: self.options.serviceURL + '/GetRuleList',
                    type: 'POST',
                    data: JSON.stringify({ ruleFilterInfo: JSON.stringify(ruleFilterInfo), dateFormat: self.options.CalendarDateFormat }),
                    contentType: "application/json",
                    dataType: 'json'
                }).then(function (responseText) {
                    var data = JSON.parse(responseText.d);
					self.options.ruleInfoForThisTag = data;
                    rad.TagManager.RuleModel = new rad.TagManager.viewModel(data);
                    //if no data is there
                    if ((data == null) || data.ruleList.length == 0) {
                        $('#rulelist_' + ruleFilterInfo.TagIds).html('No rules defined !');
                        //if not in edit tag //then open add rule
                        if (IneditTag == false) {
                            //$("#btnAddRule_" + ruleFilterInfo.TagIds).click();
                        }
                        return;
                    }

                    var id = '#rulelist_' + ruleFilterInfo.TagIds;
                    //1 more ajax to get rule template
                    $.ajax({
                        url: self.options.serviceURL + '/GetRuleTemplate',
                        type: 'POST',
                        contentType: "application/json",
                        dataType: 'json'
                    }).then(function (responseText) {
                        //putting html in rulebody

                        $(id).html(responseText.d);
                        $(function () {
                            $("[data-toggle='tooltip']").tooltip();
                        });
                        try {
                            ko.cleanNode($(id).children()[0]);
                            ko.applyBindings(rad.TagManager.RuleModel, $(id).children()[0]);
                        }
                        catch ($e1) {
                            console.log($e1);
                        }
                    }).fail(function (a) {
                        alert(a);
                    });
                    for (var rule; rule < data.ruleList.length; rule++) {
                        var ruleTextDiv = $("#ruleTextDiv_" + data.ruleList.RuleId);
                        var ruleTextDivHeight = $("#ruleTextDiv_" + data.ruleList.RuleId).height();
                        if (ruleTextDivHeight > 50) {
                            ruleTextDiv.addClass("ruleTextDivHeight");
                            ruleTextDiv.next(".expandIcon").show();
                        }
                    }
                }).fail(function (a) {
                    alert(a);
                });
            };


            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////       

            rad.TagManager.PersistantClicked = function (target) {
                var boolHere = false;
                if (target.hasAttribute("clicked")) {
                    if (JSON.parse(target.getAttribute("clicked")) == false) {
                        target.setAttribute("clicked", true);
                        boolHere = true;
                        if (self.options.ShowDataSet)
                            $(target).closest("#tp-is-persistant-div").siblings("#PMDataSetContainer").show();
                    }
                    else {
                        target.setAttribute("clicked", false);
                        boolHere = false;
                        $(target).closest("#tp-is-persistant-div").siblings("#PMDataSetContainer").hide();
                    }
                }
                else {
                    target.setAttribute("clicked", true);
                    boolHere = true;
                    if (self.options.ShowDataSet)
                        $(target).closest("#tp-is-persistant-div").siblings("#PMDataSetContainer").show();
                }

                if (boolHere) {
                    $(target).attr("checked", true);
                }
                else
                    $(target).attr("checked", false);

                if ($(target).attr('checked') == 'checked')
                    if (self.tagIdentifier == -1)
                        rad.TagManager.getDataSet(-1);
                    else
                        rad.TagManager.getDataSet(-1);
            }


            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 



            //on click of tag configuration header
            rad.TagManager.showTagDetails = function (elem) {
                $(".tagTypeEditBtn").hide();
                var configFormSection = $(".tag_config_form");
                $("#tp-tagname").next().css({ 'display': 'none' });
                $("#tp-tagname").next().next().css({ 'display': '' });
                //if it is visible //that means we want to close it
                if (configFormSection.is(':visible')) {
                    $(".editTagDetails").hide(); //.tag-detail-colon,
                    //to make the content empty
                    $.each($(".tag-detail-colon"), function (e, elem) { elem.innerText = ""; });
                    //hide the one and change the direction of arrow
                    $(elem).find(".fa-caret-down").removeClass("fa-caret-down").addClass("fa-caret-right");
                    //config form is slided up
                    $(".tag_config_form").slideUp();
                    //$(".tag-expand-div").removeClass("tag-expand-div-after");
                    $(".tag-expand-div,.tag_expand_div_content,.tag_config_form_btnset").show();
                    $(".tag-expand-div").removeClass("tag-expand-div-after");

                    //change the class set
                    $(".list-group").removeClass("tag_config_expanded");
                    $(".list-group").addClass("tag_config_collapsed");

                    var headerHeight = $("#tag_config_form_header")[0].offsetHeight + 40;
                    var screenHeightRuleSec = screenHeightRight - headerHeight - 25; //-40 for close

                    //as we want to close it //collapsed class is applied
                    $(".tag_config_collapsed").css({ "max-height": screenHeightRuleSec });
                    console.log("setting height of collapsed " + screenHeightRuleSec);
                    $(".tag_config_collapsed").animate({
                        scrollTop: 100
                    }, 300);
                } else {
                    //$(".tag-detail-colon").show();

                    //to make the content visible
                    $.each($(".tag-detail-colon"), function (e, elem) { elem.innerText = ":"; });
                    $(elem).find(".fa-caret-right").removeClass("fa-caret-right").addClass("fa-caret-down");
                    //edit tag is called to fill the things //textboxes and spans
                    $(".editTagDetails").show();
                    rad.TagManager.editTag(elem, false, false); //last false is for no calling od edittagdetails
                    //$("#tp-tagname, #tp-tagdesc, #tp-priority, #tp-reference-dimension-container--1, #tp-reference-attribute-container--1, #tp-tag-default-value-container--1, .tagTypeSelect").animate({ opacity: 0 }, 2000, function () { $("#tp-tagname, #tp-tagdesc, #tp-priority, #tp-reference-dimension-container--1, #tp-reference-attribute-container--1, #tp-tag-default-value-container--1, .tagTypeSelect").addClass("display_none", 1000, "easeOutQuad"); }); //.addClass("display_none", 1000, "easeOutQuad");

                    //hiding all the containers and all
                    $("#tp-tagname, #tp-tagdesc, #tp-reference-dimension-container--1, #tp-reference-attribute-container--1, #tp-tag-default-value-container--1,#tp_tag_dataset_container__1, #s2id_tagTypeSelect,#tp-priority").hide(); //.fadeOut(1000, function () { console.log("Fading out"); });
                    //$("#s2id_tagTypeSelect").css({'display' :'none'});
                    $("#s2id_tagTypeSelect").hide();
                    $("#tp-priority").closest(".tag_config_inputs").hide();

                    //showing the spans with data
                    $(".config_form_span").show();

                    //$(".config_form_span").addClass("inlineBlckTagDetails");
                    //hide btnset of the 
                    $(".tag_config_form_btnset,.tagTypeEditBtn").hide();
                    $(".tag-expand-div").addClass("tag-expand-div-after");

                    //change the classes set
                    $(".list-group").removeClass("tag_config_collapsed");
                    $(".list-group").addClass("tag_config_expanded");

                    //disable checkboxes
                    $("#tp-is-dimension").attr("disabled", true);
                    $("#tp-is-persistant").attr("disabled", true);

                    var tag_config_sec_height = $(".tag_config_form").height() + $("#tag_config_form_header")[0].offsetHeight + 85;
                    var screenHeightRuleSec = screenHeightRight - tag_config_sec_height - 25; //-25 for close  
                    console.log("max height is :" + screenHeightRuleSec);
                    $(".tag_config_expanded").css({ "max-height": screenHeightRuleSec });
                    console.log("expanded max-height " + screenHeightRuleSec);
                    $(".tag_config_expanded").animate({
                        scrollTop: 100
                    }, 300);
                }
            };

            rad.TagManager.setListView = function () {
                $(".tag_config_list_body2_div").show();                     // hide datatbles list view section
                $(".tag_config_list_body2_div").find(".dataTables_scrollHeadInner .dataTable").css("width", (($(".my-custom-modal-body .bootbox-body")[0].offsetWidth - 100) + "px"));

                $(".gridviewDiv").hide();
                $(".tag_header_view_iconSpan").find(".fa-th-list").removeClass("fa-th-list").addClass("fa-th-large");  // toggle the view icon
                $(".tag_header_view_iconSpan").removeAttr('data-original-title');
                $(".tag_header_view_iconSpan").attr('data-original-title', 'Grid View');
            };



            rad.TagManager.removeGridViewClasses = function () {
                $(".tag_config_list_container").removeClass("onToggleTag");
                $(".tag_configList_content").removeClass('onToggleTag');
                $(".tag_config_content_body").removeClass("tag_config_content_body_after");
                $(".gridviewDiv").removeClass("gridviewDiv_after");
                $(".selected_div").removeClass("selected_div_fixed_collpased");
                $(".gridViewSearchDiv").removeClass("searchAftrToggle");
                $(".taglistSearchInpt").removeClass("taglistSearchInptAfter");
                $(".searchIconGridView").hide();
            };





            rad.TagManager.hideAddTagPanel = function () {
                $.each($(".col_edit_selected"), function (e, elem) {
                    $(elem).removeClass("col_edit_selected");
                });
                if (rad.TagManager.editTagDetailFlag || rad.TagManager.editTagFlag && rad.TagManager.addTagFlag == false) {
                    $(".tag_config_form").slideUp();
                    //   $("div[id ^= rule_section_]").hide();
                    $(".tagTypeEditBtn").hide();
                    $(".tag_expand_div_content").show();
                    $(".tag_header_right_btnset").removeClass("tag_header_right_btnset_after");
                    $(".tagConfigRightCaret").find(".fa-caret-down").removeClass("fa-caret-down").addClass("fa-caret-right");
                }
                else {
                    $("div[id ^= rule_section_]").hide();
                    $(".tag_config_form_header").hide();
                    $(".tag_config_form_header,.tag_expand_div_content").hide();
                    $(".tag_config_form").slideUp();
                    setTimeout(function () {
                        rad.TagManager.removeGridViewClasses();
                        if (currView == "LIST") {
                            rad.TagManager.setListView();
                        } else if (currView == "GRID") {
                            rad.TagManager.setGridView();
                        }
                    }, 500);
                    $(".addTagTitle,.tagNameHolderSpan_div").hide();
                    $(".tag_header_right_btnset").removeClass("tag_header_right_btnset_after");
                    $("div").removeClass('selected_div_highlight_edit');
                    $(".tag-expand-div").removeClass("tag-expand-div-after");
                    //removing the class and also making the display of arrow none
                    if ($(".tag_config_list_body").find(".selected_div_bckgrnd").length > 0) {
                        $($(".tag_config_list_body").find(".selected_div_bckgrnd").children()[0]).children().removeClass("Inner_Selected_Tile");
                        $($(".tag_config_list_body").find(".selected_div_bckgrnd").children()[0]).removeClass("selected_Inner_Div");
                        $(".tag_config_list_body").find(".selected_div_bckgrnd").children(".tag_tile_arrow")[0].style.display = "none";
                    } $(".tag_config_list_body").find(".selected_div_bckgrnd").removeClass("selected_div_bckgrnd");
                }
                $(".tag-expand-div").removeClass("tag-expand-div-after");
            };




            rad.TagManager.format = function (state) {

                if (state.id == "Colored" || state.id == "ColoredBrac") {
                    return "<span style='color:red'> " + state.text + "</span>";
                }
                else
                    return state.text;
            };







            rad.TagManager.showAddRuleFromTabIcon = function (elem) {

                var tagId = $(elem).attr('value');
                $("#btnAddRule_" + tagId).unbind().click();
                return false;
            };

            rad.TagManager.hideRuleEditor = function (elem) {

                var tagId = $(elem).attr('value');
                rad.TagManager.resetRuleForm(tagId, $("#txtRulePriority_" + tagId).val());
                $("#ruleForm_" + tagId).slideUp();
                $('.list-group').css({ "margin-top": "-25px !important" });
            };

            rad.TagManager.hideRuleEditorUpdate = function () {

                $("#editRule_" + rad.TagManager.currRuleIdInEdit).slideUp();
                $("#ruleRow_" + rad.TagManager.currRuleIdInEdit).show();
            };

            // Rule Editor Binding helpers ------------------------------------------ starts
            rad.TagManager.getDataType = function (dataType) {
                var dataTypeArray = ["Var", "Text", "Numeric", "DateTime", "None"];
                return dataTypeArray[dataType];
            }

            rad.TagManager.getColumnType = function (colType) {
                var colTypeArray = ["Both", "Expression", "Action"];
                return colTypeArray[colType];
            }

            rad.TagManager.RuleColumnApplicability = function (colApp) {
                var colAppArray = ["BOTH", "RHS", "LHS"];
                return colAppArray[colApp];
            }

            rad.TagManager.RuleExecutionMode = function (execMode) {
                var ruleExecutionModeArray = ["None", "Priority", "Order"];
                return ruleExecutionModeArray[execMode];
            }

            rad.TagManager.loadGrammarInfo = function (data) {
                //
                return data.RadxRuleInfo.GrammarInfo;

            };

            //warning when adding new rule //and then try to edit
            rad.TagManager.onDiscardUpdate = function () {

            }

            rad.TagManager.onCancelUpdate = function () {
                $(".WarningOnEditPopUp").hide();
            }



            rad.TagManager.GetRuleBindingInfo = function (alpha, polarisRuleMappingId, action, isOverride) {
                var ruleRowIndex = parseInt($(alpha.parentNode).attr("index"));
                var AlphaTop = $(alpha).parent().offset().top;
                //var ActualTopOfRuleInRuleList = AlphaTop - RuleListDivTop;

                //var AlphaOffsetTop = alpha.parentNode.offsetTop;
                if (action == 'UPDATE') {
                    //populating the data
                    var ruleId = $(alpha).attr('value');
                    //getting the ruleID
                    if (rad.TagManager.currRuleIdInEdit == "")
                        rad.TagManager.currRuleIdInEdit = ruleId;

                    //if something already opened //hide it
                    if (rad.TagManager.currRuleIdInEdit != ruleId && rad.TagManager.currRuleIdInEdit != "") {
                        rad.TagManager.hideRuleEditorUpdate(rad.TagManager.currRuleIdInEdit);
                    }
                    rad.TagManager.currRuleIdInEdit = ruleId;

                    //setting all the variables
                    var tagId = $(alpha).attr('tag_id');
                    if ($("#ruleForm_" + tagId)[0].style.display != "none") {
                        //                        $(".WarningOnEditPopUp").show();
                        bootbox.dialog({
                            message: "Do you want to save the rule",
                            className: "Save_Discard_Warning_Dialog",
                            backdrop: true,
                            animate: true,
                            buttons: {
                                Save: {
                                    label: "Save",
                                    className: "Save_EditRule_Btn",
                                    callback: function () {
                                        console.log("You clicked Save");
                                        //return false;
                                    }
                                },
                                cancel: {
                                    label: "Discard",
                                    className: "Cancel_EditRule_Btn",
                                    callback: function () {
                                        /*var polarisRuleMappingID = $(alpha).attr('pol-rule-mapping-id');
                                        var isOverride = $(alpha).attr('isOverride');
                                        var startDate = $(alpha).attr('start_date');
                                        var endDate = $(alpha).attr('end_date');
                                        var mydate = new Date(startDate);
                                        var month = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"][mydate.getMonth()];
                                        var formatted_sdate = month + ' ' + mydate.getDate() + ' ' + mydate.getFullYear();
                                        var mydate = new Date(endDate);
                                        var formatted_edate = month + ' ' + mydate.getDate() + ' ' + mydate.getFullYear();*/

                                        var customObj = {};
                                        customObj.ruleRowIndex = ruleRowIndex;
                                        customObj.ruleId = ruleId;
                                        customObj.polarisRuleMappingID = $(alpha).attr('pol-rule-mapping-id');
                                        customObj.isOverride = $(alpha).attr('isOverride');
                                        customObj.startDate = $(alpha).attr('start_date');
                                        customObj.endDate = $(alpha).attr('end_date');
                                        //changin the format of date
                                        // var datePeices = customObj.startDate.split('-');
                                        //customObj.startDate = datePeices[1] + '-' + datePeices[0] + '-' + datePeices[2];
                                        //datePeices = customObj.endDate.split('-');
                                        //customObj.endDate = datePeices[1] + '-' + datePeices[0] + '-' + datePeices[2];

                                        var mydate = new Date(customObj.startDate);
                                        var month = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"][mydate.getMonth()];
                                        customObj.formatted_sdate = month + ' ' + mydate.getDate() + ' ' + mydate.getFullYear();
                                        month = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"][mydate.getMonth()];
                                        var mydate = new Date(customObj.endDate);
                                        customObj.formatted_edate = month + ' ' + mydate.getDate() + ' ' + mydate.getFullYear();

                                        console.log("You clicked cancel");
                                        rad.TagManager.SaveRuleFunc(tagId, alpha, action, customObj);
                                    }
                                }
                            }
                        });
                    }
                    else {
                        var customObj = {};
                        customObj.ruleRowIndex = ruleRowIndex;
                        customObj.ruleId = ruleId;
                        customObj.polarisRuleMappingID = $(alpha).attr('pol-rule-mapping-id');
                        customObj.isOverride = $(alpha).attr('isOverride');
                        customObj.startDate = $(alpha).attr('start_date');
                        customObj.endDate = $(alpha).attr('end_date');
                        //changin the format of date
                        //var datePeices = customObj.startDate.split('-');
                        //customObj.startDate = datePeices[1] + '-' + datePeices[0] + '-' + datePeices[2];
                        //datePeices = customObj.endDate.split('-');
                        // customObj.endDate = datePeices[1] + '-' + datePeices[0] + '-' + datePeices[2];
                        var mydate = new Date(customObj.startDate);
                        var month = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"][mydate.getMonth()];
                        customObj.formatted_sdate = month + ' ' + mydate.getDate() + ' ' + mydate.getFullYear();
                        var mydate = new Date(customObj.endDate);
                        month = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"][mydate.getMonth()];
                        customObj.formatted_edate = month + ' ' + mydate.getDate() + ' ' + mydate.getFullYear();

                        console.log("You clicked cancel");
                        rad.TagManager.SaveRuleFunc(tagId, alpha, action, customObj);
                    }



                } else {
                    var tagId = $(alpha).attr('value');
                    var customObj = {};
                    customObj.ruleRowIndex = ruleRowIndex;
                    customObj.polarisRuleMappingID = polarisRuleMappingId;
                    rad.TagManager.SaveRuleFunc(tagId, alpha, action, customObj);

                }
                //var tagId = 57; //change

            };
            // Rule Editor Binding helpers ------------------------------------------ ends

            rad.TagManager.SaveRuleFunc = function (tagId, alpha, action, customObj) {
                //var tagId = tagId;
                //var polarisRuleMappingID = polarisRuleMappingID;
                $.ajax({
                    url: self.options.serviceURL + '/GetRuleBindingInfo',
                    type: 'POST',
                    data: JSON.stringify({ tagId: tagId, polarisRuleMappingId: customObj.polarisRuleMappingID, action: action, identifier: self.options.pageIdentifier, user: self.options.actionBy }),
                    contentType: "application/json",
                    dataType: 'json'
                }).then(function (responseText) {

                    var data = JSON.parse(responseText.d);
                    if (action == 'UPDATE') {


                        var TempTagID = $(alpha).attr('tag_id');
                        var RuleListDivTop = $("#rulelist_" + TempTagID).offset().top; //getting top of the static div
                        var ParentDivMaxHeight = $("#rulelist_" + TempTagID).parent()[0].offsetHeight;
                        //var availableSpace = ParentDivTop + ParentDivMaxHeight; //if the calculated saamaan is less than this then not a problem

                        //var ActualTopOfRuleInRuleList = AlphaTop - RuleListDivTop;
                        if (prevRuleEditorDivId != '') {
                            if (prevRuleEditorDivId == ('ruleEditorDiv_rule_' + customObj.ruleId)) {
                                $('#' + prevRuleEditorDivId).slideDown();
                            } else {
                                //
                                if ($('#' + prevRuleEditorDivId + '_ruleEditor').length > 0) {
                                    $('#' + prevRuleEditorDivId + '_ruleEditor').data('ruleEngine').Destroy(); //.ruleEngine()
                                    $('#' + prevRuleEditorDivId + '_ruleEditor').remove();
                                }
                            }
                        }
                        $("div[id^=ruleForm_]").slideUp();
                        $("#editRuleFormDateControl_" + customObj.ruleId).dateControl({ formatter: "iago", startDate: customObj.formatted_sdate, endDate: customObj.formatted_edate });
                        $(".ruleEditorUpdatePanel").addClass("ruleEditorUpdatePanel_edit");
                        setTimeout(function () {
                            $('#editRule_' + customObj.ruleId).slideDown();
                            //$(document).find(".opensright").removeClass("opensright").addClass("opensleft");  // for handling date time control opening position
                            //$("#editRuleFormDateControl_" + customObj.ruleId + "_dateInfo.opensleft").prepend($("#editRuleFormDateControl_" + customObj.ruleId + "_dateInfo .calendar.left")); // for handling date time control opening position
                        }, 500);
                        $(alpha).parent().hide();              // hide the curr rule row
                        $("#editTxtRuleName_" + customObj.ruleId).val($(alpha).attr('rule_name'));
                        $("#editTxtRulePriority_" + customObj.ruleId).val($(alpha).attr('rule_prior'));
                        $("#editTxtRulePriority_" + customObj.ruleId).attr("old_priority", $(alpha).attr('rule_prior'));
                        $("#editTxtRulePriority_" + customObj.ruleId).attr("max_priority", $(alpha).attr('rule_prior'));
                        $(".spinner").spinner();

                        //setting the start and end date too



                        prevRuleEditorDivId = "ruleEditorDiv_rule_" + customObj.ruleId;
                        $('#' + prevRuleEditorDivId).append('<div id="' + prevRuleEditorDivId + '_ruleEditor' + '"></div>');



                        $('#' + prevRuleEditorDivId + '_ruleEditor').ruleEngine({
                            grammarInfo: rad.TagManager.loadGrammarInfo(data),
                            ruleText: data.RuleText,
                            serviceUrl: self.options.ruleEditorServiceUrl
                        });



                        setTimeout(function () {
                            $("#rulelist_" + tagId).parent().scrollTop((customObj.ruleRowIndex) * 60);
                            console.log("Scrolling here " + ((customObj.ruleRowIndex) * 60));
                        }, 300);
                        prevRuleFormDivId = 'editRule_' + customObj.ruleId;

                    } else {
                        $("div[id^=ruleRow_]").show();
                        if (prevRuleEditorDivId != '') {
                            if (prevRuleEditorDivId == ("ruleEditorDiv_tag_" + tagId) && $('#' + prevRuleEditorDivId + '_ruleEditor').length > 0) {
                                $('#' + prevRuleEditorDivId + '_ruleEditor').data('ruleEngine').Destroy(); //.ruleEngine()
                                $('#' + prevRuleEditorDivId + '_ruleEditor').remove();
                            } else {
                                if ($('#' + prevRuleEditorDivId + '_ruleEditor').length > 0) {
                                    $('#' + prevRuleEditorDivId + '_ruleEditor').data('ruleEngine').Destroy(); //.ruleEngine()
                                    $('#' + prevRuleEditorDivId + '_ruleEditor').remove();
                                }
                            }
                        }
                        if ($(alpha).attr('pol-isoverride') == 0) {
                            var isoverride = false;
                        } else if ($(alpha).attr('pol-isoverride') == 1) {
                            var isoverride = true;
                        }
                        $.ajax({
                            url: self.options.serviceURL + '/GetMaxPriorityForRuleInTag',
                            type: 'POST',
                            data: JSON.stringify({ tagId: Number(tagId), isOverride: isoverride }),
                            contentType: "application/json",
                            dataType: 'json'
                        }).then(function (response) {
                            var data = JSON.parse(response.d);
                            rad.TagManager.resetRuleForm(tagId, data);

                        }).fail(function (a) {
                            alert(a);
                        });

                        $(".spinner").spinner();
                        prevRuleEditorDivId = "ruleEditorDiv_tag_" + tagId;
                        $('#' + prevRuleEditorDivId).append('<div id="' + prevRuleEditorDivId + '_ruleEditor' + '"></div>');
                        $('#' + prevRuleEditorDivId + '_ruleEditor').ruleEngine({
                            grammarInfo: rad.TagManager.loadGrammarInfo(data),
                            serviceUrl: self.options.ruleEditorServiceUrl
                        });
                        $("#ruleEditorDiv_tag_" + tagId).slideDown();
                        $("div[id^=editRule_]").slideUp();
                        if (rad.TagManager.editTagFlag == false) {
                            $(".tag_config_form").slideUp();
                        }


                        $(".ruleEditorUpdatePanel").addClass("ruleEditorUpdatePanel_edit");
                        setTimeout(function () {
                            $("#ruleForm_" + tagId).slideDown();
                            //$(document).find(".opensright").removeClass("opensright").addClass("opensleft");
                            //$("#ruleFormDateControl_" + tagId + "_dateInfo.opensleft").prepend($("#ruleFormDateControl_" + tagId + "_dateInfo .calendar.left"));
                            $('.list-group').css({ "margin-top": "0px !important" });
                        }, 500);
                    }
                }).fail(function (a) {
                    alert(a);
                });
            }

            rad.TagManager.resetRuleForm = function (id, maxPriority) {

                $("#txtRuleName_" + id).val('');
                $("#txtRulePriority_" + id).val(maxPriority);
                $("#txtRulePriority_" + id).attr("max_priority", maxPriority);
                $("#ruleFormDateControl_" + id).dateControl({
                    "startDate": "January 01 2014",
                    formatter: "iago"
                });
                $("#ruleTxt").val('');
                $("#ruleEditorDiv_tag_" + id).slideDown();
                $("#ruleForm_" + id).slideDown();
                //  $("#prettyCode").html('');
            }





            rad.TagManager.toggleViewSection = function (elem) {

                $("div[id ^= rule_section_]").hide();
                if ($(".tag_config_list_body2_div").is(':visible')) {
                    rad.TagManager.removeGridViewClasses();
                    rad.TagManager.setGridView();
                }
                else {
                    rad.TagManager.removeGridViewClasses();
                    rad.TagManager.setListView();
                }
                //removing the class and also making the display of arrow none
                if ($(".selected_div_bckgrnd").length > 0) {
                    $($(".selected_div_bckgrnd").children()[0]).children().addClass("Inner_Selected_Tile");
                    $($(".selected_div_bckgrnd").children()[0]).addClass("selected_Inner_Div");
                    $(".selected_div_bckgrnd").children(".tag_tile_arrow")[0].style.display = "";


                } $(".selected_div_bckgrnd").removeClass("selected_div_bckgrnd");
                $(".tag_header_right_btnset").removeClass("tag_header_right_btnset_after");
                $(".tag_config_form_header,.tag-expand-div,.tag_config_form,.addTagTitle,.tagNameHolderSpan_div").hide();
            };

            rad.TagManager.toggleRuleSection = function (alpha) {

                if (!rad.TagManager.toggleTag) {
                    rad.TagManager.toggleTag = true;
                    return;
                }
                $(alpha).parent().toggleClass('selected_div_highlight');
                $(".tag-expand-div").removeClass("tag-expand-div-after");
                //changing the direction of arrow
                $(".tag_configuration_span").find(".fa-caret-down").removeClass("fa-caret-down").addClass("fa-caret-right");

                //filling the data
                var tagId = $(alpha).attr('value');
                var tag_name = $(alpha).attr('tag_name');
                var ruleSection = $("#rule_section_" + tagId);
                var listItem = $("div#tag_list_item_" + tagId);
                var tagItem = $("#tag_accordion_btn_" + tagId);
                $(".tag_configuration_span").attr('value', tagId); //setting the value

                if (prevRuleSection != "" && prevRuleSection != undefined && prevRuleSection[0] != "" && prevRuleSection[0] != undefined) {
                    //listItem.parent().append(prevRuleSection);
                    //search tagListItem
                    prevRuleSection[0].style.display = "none";
                    var prevTagID = prevRuleSection.attr("tagID");
                    $("div#tag_list_item_" + prevTagID).parent().append(prevRuleSection);
                }
                if (ruleSection.is(':visible')) {
                    // do-nothing
                } else {
                    if ($(".tag_header_view_iconSpan").find(".fa-th-large")) {
                        currView = "LIST";
                        rad.TagManager.setGridView();
                    } else {
                        currView = "GRID";
                    }
                    rad.TagManager.setGridViewClasses();

                    //
                    if ($(".tag_expand_div_content").length > 0) {
                        $(".tag_expand_div_content").html('');
                    }
                    prevRuleSection = ruleSection;
                    //ruleSection.style.display = "";
                    //putting the whole content in this
                    $(".tag_expand_div_content").append(ruleSection);

                    ruleSection.show();

                    $(".tag_header_right_btnset").addClass("tag_header_right_btnset_after");
                    $(".tag_config_form,.addTagTitle,.editTagDetails").hide();


                    //removing the class and also making the display of arrow none
                    if ($(".tag_config_list_body").find(".selected_div_bckgrnd").length > 0) {
                        $($(".tag_config_list_body").find(".selected_div_bckgrnd").children()[0]).children().removeClass("Inner_Selected_Tile");
                        $($(".tag_config_list_body").find(".selected_div_bckgrnd").children()[0]).removeClass("selected_Inner_Div");
                        $(".tag_config_list_body").find(".selected_div_bckgrnd").children(".tag_tile_arrow")[0].style.display = "none";
                    }
                    $(".tag_config_list_body").find(".selected_div_bckgrnd").removeClass("selected_div_bckgrnd");
                    //make the arrow div visible to this
                    if ($(".gridviewDiv div#tag_list_item_" + tagId).length > 0) {
                        $(".gridviewDiv div#tag_list_item_" + tagId).children().addClass("Inner_Selected_Tile");
                        $(".gridviewDiv div#tag_list_item_" + tagId).addClass("selected_Inner_Div");
                        $(".gridviewDiv div#tag_list_item_" + tagId).siblings(".tag_tile_arrow")[0].style.display = "";
                        $(".gridviewDiv div#tag_list_item_" + tagId).parent().addClass("selected_div_bckgrnd");
                    }

                    $(".tagConfigRightCaret,.tag_config_form_header,.tag-expand-div,.tagNameHolderSpan_div").show();
                    $(".tagNameHolderSpan").html(tag_name);
                    $(".tagNameHolderSpan").attr('title', tag_name);

                    $("#ruleForm_" + tagId).slideUp();
                    ruleSection.find('li[pol-tab-type="rule-tab"]').removeClass().addClass("active");
                    ruleSection.find('li[pol-tab-type="override-rule-tab"]').removeClass("active");
                    $("#btnAddRule_" + tagId).html("Add Rule");
                    rad.TagManager.bindRule(tagId, false, false);
                    $(".tag_expand_div_content").slideDown();
                    //setting the maximum height
                    //$(".tag_expand_div_content").find(".tag_config_expanded")
                }
                $('.list-group').css({ "margin-top": "-25px !important" });

                //changing the classes heree
                //change the class set
                if ($(".list-group").hasClass("tag_config_expanded")) {
                    $(".list-group").removeClass("tag_config_expanded");
                    $(".list-group").addClass("tag_config_collapsed");
                }

                var headerHeight = $("#tag_config_form_header")[0].offsetHeight + 40;
                var screenHeightRuleSec = screenHeightRight - headerHeight - 25;

                //as we want to close it //collapsed class is applied
                $(".tag_config_collapsed").css({ "max-height": screenHeightRuleSec });
                console.log("collapsed " + screenHeightRuleSec);
                $(".tag_config_collapsed").animate({
                    scrollTop: 100
                }, 300);
            };

            rad.TagManager.closeExpandedTag = function (elem) {

                var tagId = $(elem).attr('value');
                var listItem = $("div#tag_list_item_" + tagId);
                var ruleSection = $("#rule_section_" + tagId);
                ruleSection.hide();
                listItem.removeClass("cust_tag_row_expanded");
                $("#close-tag-" + tagId).hide();
                $("#edit-del-div-" + tagId).removeClass("edit-del-btn-onTagExpand");
                $("#curr-expanded-tag" + tagId).append(listItem);
                $("#curr-expanded-tag" + tagId).append(ruleSection);
                rad.TagManager.removeGridViewClasses();
            };

            rad.TagManager.delViewModel = function (data, tag_name, elemId, confirmFlag) {

                var self = this;
                self.TagNamesCHKDEL = ko.observableArray([]);
                if (data.length > 0) {
                    for (var i = data.length - 1; i >= 0; i--) {
                        if (data[i].TagName == tag_name) {
                            data.splice(i, 1);
                        }
                    }
                }
                if (confirmFlag) {
                    if (data != "" && data != null) {
                        $(elemId).append('<div><div> Conflicted Tag Details  :  </div><div class="tagDetailHeader"><div class="colHeader">Tag: </div><div class="colHeader">Rule: </div></div><div class="tagDetailContent" data-bind = "foreach: TagNamesCHKDEL" ><div class="tagDetailContentRow"><div class="colHeader" data-bind="text: tagName"></div><div class="colHeader" data-bind="text: ruleName"></div></div></div></div>');
                    }
                }
                else {
                    if (data != "" && data != null) {
                        $(elemId).append('<div><div> The tag is in use for the following :  </div><div class="tagDetailHeader"><div class="colHeader">Tag: </div><div class="colHeader" style="margin-left: 10px;">Rule: </div></div>  <div class="tagDetailContent" data-bind = "foreach: TagNamesCHKDEL" ><div class="tagDetailContentRow"><div class="colHeader" data-bind="text: tagName"></div><div class="colHeader" data-bind="text: ruleName"></div></div></div></div>');
                    }
                }
                self.TagNamesCHKDEL($.map(data, function (individualTag) {
                    return new rad.TagManager.CHKDEL(individualTag);
                }));
            };

            rad.TagManager.tagPriorties = [];
            rad.TagManager.rulePriorties = [];



            //to validate the form
            rad.TagManager.validateTagInfo = function (tagInfo, editFlag) {
                var message = "", err_fields = [];
                var new_priority = Number($("#tp-priority").val());
                var curr_priority = $("#tp-priority").attr("curr_priority");

                //tag Name is empty
                if (tagInfo.TagName.trim() === "" || tagInfo.TagName === null) {
                    message += "* Tag Name is mandatory.";
                    err_fields.push({ elemId: "#tp-tagname", msgTip: "Enter Tag Name", triggerType: 'click' });
                }
                if ((tagInfo.TagName.indexOf('!') != -1) || (tagInfo.TagName.indexOf('@') != -1) || (tagInfo.TagName.indexOf('#') != -1) || (tagInfo.TagName.indexOf('$') != -1) || (tagInfo.TagName.indexOf('%') != -1) || (tagInfo.TagName.indexOf('&') != -1) || (tagInfo.TagName.indexOf('*') != -1)) {
                    message += "Special Characters for Tag name not allowed.";
                    err_fields.push({ elemId: '#tp-tagname', msgTip: "Special Characters for Tag name not allowed", triggerType: 'click' });

                }
                if (tagInfo.DataTypeDetails.DataType != "String") {
                    if (tagInfo.DefaultValue != "") {
                        if (isNaN(Number(tagInfo.DefaultValue))) {
                            message += "*Default Value must be Number.";
                            err_fields.push({ elemId: '#tp-tag-default-span', msgTip: "Type Mismatched", triggerType: 'click' });
                        }
                    }
                }
                //if any of the above case
                if ((tagInfo.ReferenceDimension != "-" && tagInfo.ReferenceDimension != null) &&
                    (tagInfo.ReferenceAttribute == "-1")) {
                    message += "* Reference Attribute is required.";
                    err_fields.push({ elemId: "#tp-reference-dimension-" + tagInfo.TagId, msgTip: "Enter Ref. Dimension", triggerType: 'click' },
                    { elemId: "#tp-reference-attribute-" + tagInfo.TagId, msgTip: "Enter Ref. Attribute", triggerType: 'click' }
                    );
                }
                if (tagInfo.IsPersistant) {
                    if (tagInfo.DataSet == "" || tagInfo.DataSet == -1 || tagInfo.DataSet == null || tagInfo.DataSet == "NoData" || tagInfo.DataSet == "-") {
                        message += "* `DataSet is required.";
                        err_fields.push({ elemId: "#tp_dataSet_" + tagInfo.TagId, msgTip: "Enter DataSet", triggerType: 'click' },
                        { elemId: "#tp_dataSet_" + tagInfo.TagId, msgTip: "Enter DataSet", triggerType: 'click' }
                        );
                    }
                }

                //if this is not entered
                if (Number($("#tp-priority").val()) === undefined) {
                    message += "Please provide a valid priority";
                    err_fields.push({ elemId: "#tp-priority", msgTip: "Enter Tag Priority", triggerType: 'mouseenter' });
                }
                //if priority value not set
                if (Number($("#tp-priority").val()) <= 0) {
                    message += "Priority should be greater than zero";
                    err_fields.push({ elemId: "#tp-priority", msgTip: "Enter Tag Priority", triggerType: 'mouseenter' });
                }
                console.log(rad.TagManager.tagPriorties);

                // If priority is modified, remove old one
                if (curr_priority != new_priority) {
                    for (var i = 0; i < rad.TagManager.tagPriorties.length; i++) {
                        if (rad.TagManager.tagPriorties[i] == curr_priority) {
                            rad.TagManager.tagPriorties.splice(i, 1);
                        }
                    }
                }
                // To check priority already exists
                if (new_priority != tagInfo.oldPriority) {
                    for (var i = 0; i < rad.TagManager.tagPriorties.length; i++) {
                        if (rad.TagManager.tagPriorties[i] == new_priority) {
                            message += "* Priority already exists.";
                            err_fields.push({ elemId: "#tp-priority", msgTip: "Enter Tag Priority", triggerType: 'click' });
                            break;
                        }
                    }
                }

                //alert message if it is not empty
                if (message != "") {
                    rad.TagManager.alertMessanger("tagAlertMessanger", "ERROR", message);
                    rad.TagManager.errorValidationHandler(err_fields);
                    //there is even no need of service hit in that case
                    /*if (err_fields.length > 0) {
                    for (var i = 0; i < err_fields.length; i++) {

                    $(err_fields[i].elemId).parent().closest('div').find(".err-validation").hide();
                    return false;
                    }
                    }*/
                    return false;
                }
                else {

                }

                //if someone is trying to edit the things //then give a service hit
                if (editFlag) {
                    $.ajax({
                        url: self.options.serviceURL + '/CheckTagPriority',
                        type: 'POST',
                        data: JSON.stringify({ tagId: tagInfo.TagId, oldPriority: curr_priority, newProirity: new_priority }),
                        contentType: "application/json",
                        dataType: 'json'
                    }).then(function (responseText) {
                        //
                        data = JSON.parse(responseText.d);
                        //if there are conflicting rules //then push the data in the array
                        if (data != "" || data.length > 0) {
                            rad.TagManager.errorValidationHandler(err_fields);
                            return false;
                        }
                    }).fail(function (a) {
                        alert(a);
                    });
                    return true;
                }
                else {
                    return true;
                }
            };

            rad.TagManager.errorValidationHandler = function (err_fields) {

                if (err_fields.length > 0) {
                    for (var i = 0; i < err_fields.length; i++) {

                        $(err_fields[i].elemId).parent().closest('div').find(".err-validation").show();

                        $(err_fields[i].elemId).qtip({
                            content: err_fields[i].msgTip,
                            show: err_fields[i].triggerType,
                            hide: 'unfocus',
                            style: {
                                classes: 'qtip-light qtip-shadow'
                            },
                            position: {
                                my: 'top right',  // Position my top left...
                                at: 'bottom left'  // at the bottom right of...
                            }
                        });

                        if (err_fields[i].elemId == "#tp-priority")
                            $(err_fields[i].elemId).parent().closest('.ui-spinner').addClass("err-input");
                        else if (err_fields[i].elemId.startsWith('#txtRulePriority_') || err_fields[i].elemId.startsWith('#editTxtRulePriority_'))
                            $(err_fields[i].elemId).parent().closest('.ui-spinner').addClass("err-input");
                        else
                            $(err_fields[i].elemId).addClass("err-input");
                    }
                }
            };

             rad.TagManager.validateRule = function (ruleInfoObj,alpha) {
                var message = "", err_fields = [];
                if (ruleInfoObj.UserAction == "INSERT") {
                    var maxRulePriority = Number($("#txtRulePriority_" + ruleInfoObj.TagId).attr("max_priority"));
                }
                if (ruleInfoObj.UserAction == "UPDATE") {
                    var maxRulePriority = Number($("#editTxtRulePriority_" + ruleInfoObj.RuleId).attr("max_priority"));
                }
                var priority = Number(ruleInfoObj.Priority);
                var startDate = ruleInfoObj.EffectiveStartDate;
                var endDate = ruleInfoObj.EffectiveEndDate;
                if (!rad.TagManager.EditRule) {
					if(self.options.ruleInfoForThisTag.ruleList != null){
						for (var i = 0; i < self.options.ruleInfoForThisTag.ruleList.length; i++) {
							if (self.options.ruleInfoForThisTag.ruleList[i].RuleName == ruleInfoObj.RuleName) {
								message += "* Rule Name already exist";
								err_fields.push({ elemId: "#txtRuleName_" + ruleInfoObj.TagId, msgTip: "Enter Rule Name", triggerType: 'click' });
							   
								break;
							}
						}
					}
                }
                else
                {
					if(self.options.ruleInfoForThisTag.ruleList != null){
						for (var i = 0; i < self.options.ruleInfoForThisTag.ruleList.length; i++) {
							if(self.options.ruleInfoForThisTag.ruleList[i].RuleSetId != parseInt($(alpha).attr("rule_setid")))
							{
								if (self.options.ruleInfoForThisTag.ruleList[i].RuleName == ruleInfoObj.RuleName) {
									message += "* Rule Name already exist";
									err_fields.push({ elemId: "#editTxtRuleName_" + ruleInfoObj.RuleId, msgTip: "Enter Rule Name", triggerType: 'click' });
									break;
								}
							}
							
						}
					}
                }
                if (ruleInfoObj.RuleName === "") {
                    message += "* Rule Name cannot be empty";
                    if (ruleInfoObj.UserAction == "INSERT") {
                        err_fields.push({ elemId: "#txtRuleName_" + ruleInfoObj.TagId, msgTip: "Enter Rule Name", triggerType: 'click' });
                    } else {
                        err_fields.push({ elemId: "#editTxtRuleName_" + ruleInfoObj.RuleId, msgTip: "Enter Rule Name", triggerType: 'click' });
                    }
                }
                if (startDate === "") {
                    message += "* Start date cannot be empty";
                    if (ruleInfoObj.UserAction == "INSERT") {
                        err_fields.push({ elemId: "#ruleFormDateControl_" + ruleInfoObj.TagId, msgTip: "Enter Start Date", triggerType: 'mouseenter' });
                    } else {
                        err_fields.push({ elemId: "#editRuleFormDateControl_" + ruleInfoObj.RuleId, msgTip: "Enter Start Date", triggerType: 'mouseenter' });
                    }

                }
                if (endDate === "") {
                    message += "* End date cannot be empty";
                    if (ruleInfoObj.UserAction == "INSERT") {
                        err_fields.push({ elemId: "#ruleFormDateControl_" + ruleInfoObj.TagId, msgTip: "Enter End Date", triggerType: 'mouseenter' });
                    } else {
                        err_fields.push({ elemId: "#editRuleFormDateControl_" + ruleInfoObj.RuleId, msgTip: "Enter End Date", triggerType: 'mouseenter' });
                    }
                }

                if (priority === undefined) {
                    message += "* Valid priority has to be defined";
                    if (ruleInfoObj.UserAction == "INSERT") {
                        err_fields.push({ elemId: "#txtRulePriority_" + ruleInfoObj.TagId, msgTip: "Enter Rule Priority", triggerType: 'click' });
                    } else {
                        err_fields.push({ elemId: "#editTxtRulePriority_" + ruleInfoObj.RuleId, msgTip: "Enter Rule Priority", triggerType: 'click' });
                    }
                }

                if (priority <= 0) {
                    message += "* Priority should be greater than zero";
                    if (ruleInfoObj.UserAction == "INSERT") {
                        err_fields.push({ elemId: "#txtRulePriority_" + ruleInfoObj.TagId, msgTip: "Enter Rule Priority", triggerType: 'click' });
                    } else {
                        err_fields.push({ elemId: "#editTxtRulePriority_" + ruleInfoObj.RuleId, msgTip: "Enter Rule Priority", triggerType: 'click' });
                    }
                }

                var targetId = "ruleSaveAlert_" + ruleInfoObj.TagId;
                console.log(rad.TagManager.rulePriorties);
                if (priority != ruleInfoObj.OldPriority && ruleInfoObj.OldPriority != undefined) {
                    for (var i = 0; i < rad.TagManager.rulePriorties.length; i++) {
                        if (rad.TagManager.rulePriorties[i].priority == ruleInfoObj.OldPriority && rad.TagManager.rulePriorties[i].tagId == ruleInfoObj.TagId && rad.TagManager.rulePriorties[i].isOverride == ruleInfoObj.IsOverride) {
                            rad.TagManager.rulePriorties.splice(i, 1);
                        }
                    }
                }
                console.log(rad.TagManager.rulePriorties);
                if (priority != ruleInfoObj.OldPriority) {
                    if (rad.TagManager.rulePriorties.length != 0) {
                        for (var i = 0; i < rad.TagManager.rulePriorties.length; i++) {
                            if (rad.TagManager.rulePriorties[i].priority == priority && rad.TagManager.rulePriorties[i].tagId == ruleInfoObj.TagId && rad.TagManager.rulePriorties[i].isOverride == ruleInfoObj.IsOverride) {
                                message += "* Priority already exists.";
                                if (ruleInfoObj.UserAction == "INSERT") {
                                    err_fields.push({ elemId: "#txtRulePriority_" + ruleInfoObj.TagId, msgTip: "Enter Rule Priority", triggerType: 'click' });
                                } else {
                                    err_fields.push({ elemId: "#editTxtRulePriority_" + ruleInfoObj.RuleId, msgTip: "Enter Rule Priority", triggerType: 'click' });
                                }
                                break;
                            }
                        }
                    }
                }
                if (message != "") {
                    rad.TagManager.alertMessanger(targetId, "ERROR", message);
                    rad.TagManager.errorValidationHandler(err_fields);
                    return false;
                }
                return true;
            };
                

            rad.TagManager.tagTypeHandler = function (e) {
                if ($("#negative-select").prev().attr('id') == "s2id_negative-select") {
                    $("#negative-select").prev().remove();

                }
                $("#negative-select").select2({
                    formatSelection: rad.TagManager.format,
                    minimumResultsForSearch: -1
                });
                if ($("#units-select").prev().attr('id') == "s2id_units-select") {
                    $("#units-select").prev().remove();
                }
                $("#units-select").select2({
                    minimumResultsForSearch: -1
                });
                var tag_type = $("#tagTypeSelect").val();
                switch (tag_type) {
                    case "String":
                        $(".tagTypeDiv,.typeCntrl,.tagTypeEditBtn").hide();
                        $(".prefix-input").val('');
						$("#tp-reference-attribute-container--1").css({ 'pointer-events': '' });
						$("#tp-reference-dimension-container--1").css({ 'pointer-events': '' });
                        break;
                    case "Number":
                        $(".tagTypeDiv").slideDown();
                        $(".tagTypeDiv")[0].style.left = $(e).siblings(".custom-dropdown")[0].offsetLeft + "px";
                        $(".tagTypeDiv")[0].style.top = ($(e).siblings(".custom-dropdown")[0].offsetTop + 25) + "px";
                        $(".tagTypeDiv")[0].style.width = $(e).siblings(".custom-dropdown")[0].offsetWidth + "px";
                        $(".negative-select-div,.units-select-div,.tPrecision-div,.tagTypeEditBtn").show();
                        $(".tPrecision").val('0');
                        $(".prefix-input-div").hide();
						$("#tp-reference-dimension--1").val('-').trigger('change');
						$("#tp-reference-attribute-container--1").css({ 'pointer-events': 'none' });
						$("#tp-reference-dimension-container--1").css({ 'pointer-events': 'none' });
                        break;
                    case "Percentage":
                        $(".tPrecision").val('0');
                        $(".tagTypeDiv").slideDown();
                        $(".tagTypeDiv")[0].style.left = $(e).siblings(".custom-dropdown")[0].offsetLeft + "px";
                        $(".tagTypeDiv")[0].style.top = ($(e).siblings(".custom-dropdown")[0].offsetTop + 25) + "px";
                        $(".tagTypeDiv")[0].style.width = $(e).siblings(".custom-dropdown")[0].offsetWidth + "px";
                        $(".negative-select-div,.tPrecision-div,.tagTypeEditBtn").show();
                        $(".prefix-input-div,.units-select-div").hide();
						$("#tp-reference-dimension--1").val('-').trigger('change');
						$("#tp-reference-attribute-container--1").css({ 'pointer-events': 'none' });
						$("#tp-reference-dimension-container--1").css({ 'pointer-events': 'none' });
                        break;
                    case "Currency":
                        $(".prefix-input").val('$');
                        $(".tPrecision").val('0');
                        $(".tagTypeDiv").slideDown();
                        $(".tagTypeDiv")[0].style.left = $(e).siblings(".custom-dropdown")[0].offsetLeft + "px";
                        $(".tagTypeDiv")[0].style.top = ($(e).siblings(".custom-dropdown")[0].offsetTop + 25) + "px";
                        $(".tagTypeDiv")[0].style.width = $(e).siblings(".custom-dropdown")[0].offsetWidth + "px";
                        $('#negative-select').select2('val', 'DefaultBrac');
                        $(".prefix-input-div,.tPrecision-div,.negative-select-div,.units-select-div,.tagTypeEditBtn").show();
						$("#tp-reference-dimension--1").val('-').trigger('change');
						$("#tp-reference-attribute-container--1").css({ 'pointer-events': 'none' });
						$("#tp-reference-dimension-container--1").css({ 'pointer-events': 'none' });
						
                        break;
                }
            };

            rad.TagManager.editTypeFormat = function (elem) {

                $(".tagTypeDiv").show();
                $(".tagTypeDiv")[0].style.left = $(elem).siblings(".tagTypeSelect")[0].offsetLeft + "px !important";
                $(".tagTypeDiv")[0].style.top = ($(elem).siblings(".tagTypeSelect")[0].offsetTop + 25) + "px !important";
                $(".tagTypeDiv")[0].style.width = $(elem).siblings(".tagTypeSelect")[0].offsetWidth + "px !important";
            };

            //to fetch the info of the tag
            rad.TagManager.fetchTagInfo = function () {
                tag_name_attr = $("#tp-tagname").attr("tag_name");
                tag_name_val = $("#tp-tagname").val();
                tagInformation.TagId = parseInt($(".btnSaveTag").attr("tagid"));
                tagInformation.TagName = tag_name_val;
                tagInformation.TagDesc = $("#tp-tagdesc").val();
                var refDimensionVal = $("#tp-reference-dimension-" + tagInformation.TagId).val();
                tagInformation.ReferenceDimension = (refDimensionVal == "-1") ? "" : refDimensionVal;
                tagInformation.ReferenceAttribute = $("#tp-reference-attribute-" + tagInformation.TagId).val();
                if (document.getElementById("tp-tag-default-value") != null)
                    tagInformation.DefaultValue = $("#tp-tag-default-value").val();
                else {
                    if ($("#tp-tag-default-value--1").val() == "-1")
                        tagInformation.DefaultValue = "";
                    else
                        tagInformation.DefaultValue = $("#tp-tag-default-value--1").val();
                }


                tagInformation.IsAlsoDimension = $("#tp-is-dimension").is(':checked');
                tagInformation.IsPersistant = $("#tp-is-persistant").is(':checked');


                if (self.options.ShowDataSet && tagInformation.IsPersistant) {
                    if (editingtag == false)
                        tagInformation.DataSet = $("#tp_dataSet_" + tagInformation.TagId).val();
                    else
                        tagInformation.DataSet = $("#tp_dataSet_" + "-1").val();
                }
                else
                    tagInformation.DataSet = "NoData";

                //also fill current tag status
                if (tagInformation.IsPersistant)
                    self.options.isPersistantTag = true;

                tagInformation.Action = $(".btnSaveTag").attr("actiontype");
                tagInformation.DataTypeDetails = {};
                tagInformation.DataTypeDetails.DataType = $("#tagTypeSelect").val();
                tagInformation.DataTypeDetails.Prefix = $("#prefix-input").val();
                tagInformation.DataTypeDetails.Unit = $("#units-select").val();
                tagInformation.DataTypeDetails.NegativeValue = $("#negative-select").val();
                if ($("#tPrecision").val() === "" || $("#tPrecision").val() === undefined)
                    tagInformation.DataTypeDetails.DecimalPlaces = "0";
                else tagInformation.DataTypeDetails.DecimalPlaces = $("#tPrecision").val();

                tagInformation.ActionBy = self.options.actionBy;
                tagInformation.TagPriority = $("#tp-priority").val();
                tagInformation.oldPriority = $("#tp-priority").attr("curr_priority");
                tagInformation.Operation = $("#operationSelect").val();
                tagInformation.IsBindGrid = self.options.IsBindGrid;
            };

            rad.TagManager.saveTagData = function (elem) {
                var elemId = $(elem).attr('id');
                var top = elem.offsetTop + 14;
                var left = elem.offsetLeft - 380;
                rad.TagManager.fetchTagInfo();
                $.each($(".col_edit_selected"), function (e, elem) {
                    $(elem).removeClass("col_edit_selected");
                });
				$(".btnSaveTag").css({ 'pointer-events': 'none' })
                tagNameExists = false;
                if (rad.TagManager.editTagFlag == true) {
                    if (tag_name_val != null) {
                        if (tag_name_val != tag_name_attr) {
                            //if same tag name exists //then toh no scene at all
                            for (var i = 0; i < tagNames.length; i++) {
                                if (tag_name_val.toUpperCase() === tagNames[i].toUpperCase()) {
                                    rad.TagManager.alertMessanger("tagAlertMessanger", "ERROR", "Invalid. Tag Name already exists.");
                                    tagNameExists = true;
                                }
                            }
                            //then 1stly validate the data //and validation is fine
                            if (tagNameExists == false) {
                                if (rad.TagManager.validateTagInfo(tagInformation, rad.TagManager.editTagFlag) === true) {

                                    rad.TagManager.saveTagInfo(tagInformation);
                                }
                            }
                        }
                        else {
                            if (rad.TagManager.validateTagInfo(tagInformation, rad.TagManager.editTagFlag) === true) {
                                rad.TagManager.saveTagInfo(tagInformation);
                            }
                        }
                    }
                }
                //it is addTag case
                else {
                    for (var i = 0; i < tagNames.length; i++) { //added
                        if (tag_name_val.toUpperCase() === tagNames[i].toUpperCase()) {
                            rad.TagManager.alertMessanger("tagAlertMessanger", "ERROR", "Invalid. Tag Name already exists.");
                            tagNameExists = true;
                        }
                    }
                    if (tagNameExists == false) {
                        if (rad.TagManager.validateTagInfo(tagInformation, rad.TagManager.editTagFlag) === true) {
                            rad.TagManager.saveTagInfo(tagInformation);
                        }
						else
						{
							$(".btnSaveTag").css({ 'pointer-events': '' })
						}
                    }
                }
            };

            rad.TagManager.dialog_handler = function (data, html, divId, confirmDialog, top, left, tag_name_val, elemId) {

                var arr = new Array();
                if (confirmDialog) {
                    rad.TagManager.qtipHandler(elemId, conflictDialogHTML, "left", rad.TagManager.conflictDialogFunc, "");
                }
                else {
                    arr.push(tag_name_val, divId, confirmDialog, data);
                    rad.TagManager.qtipHandler(elemId, html, "left", rad.TagManager.updateTagFunc, arr);
                }
            };

            rad.TagManager.saveTagHandler = function (top, left, tag_name_val, elemId) {

                $.ajax({
                    url: self.options.serviceURL + '/CheckDeleteTag',
                    type: 'POST',
                    data: JSON.stringify({ tagId: tagInformation.TagId }),
                    contentType: "application/json",
                    dataType: 'json'
                }).then(function (responseText) {
                    //
                    data = JSON.parse(responseText.d);
                    $.ajax({
                        url: self.options.serviceURL + '/CheckTagPriority',
                        type: 'POST',
                        data: JSON.stringify({ tagId: tagInformation.TagId, oldPriority: tagInformation.oldPriority, newProirity: tagInformation.TagPriority }),
                        contentType: "application/json",
                        dataType: 'json'
                    }).then(function (responseText) {
                        data = JSON.parse(responseText.d);
                        //
                        if (data != "" || data.length > 0)
                            conflictFlag = true;
                        else
                            conflictFlag = false;

                        if (conflictFlag)
                            rad.TagManager.dialog_handler(data, conflictDialogHTML, "conflictListDiv", true, top, left, tag_name_val, elemId);
                        else
                            rad.TagManager.dialog_handler(data, chkTagNameDialogHTML, "TagNameChkList", false, top, left, tag_name_val, elemId);
                    }).fail(function (a) {
                        alert(a);
                    });
                }).fail(function (a) {
                    alert(a);
                });
            };

            rad.TagManager.expandRuleText = function (elem) {

                var ruleId = $(elem).attr('rule_id');
                toExpandRuleTxt = $(elem).find(".fa-plus");
                toCollapseRuleTxt = $(elem).find(".fa-minus");
                if (toExpandRuleTxt.length != 0) {
                    $("#ruleTextDiv_" + ruleId).removeClass("ruleTextDivHeight");
                    $(elem).find("span").html('');
                    toExpandRuleTxt.removeClass("fa-plus").addClass("fa-minus");
                }
                else if (toCollapseRuleTxt.length != 0) {
                    $("#ruleTextDiv_" + ruleId).addClass("ruleTextDivHeight");
                    $(elem).find("span").html('. . .');
                    toCollapseRuleTxt.removeClass("fa-minus").addClass("fa-plus");
                }
            };

            rad.TagManager.alertMessanger = function (messangerId, alertType, message) {

                var messanger = $("#" + messangerId);
                switch (alertType) {
                    case "SUCCESS":
                        messanger.find('span').html(message);
                        messanger.removeClass().addClass("successAlert cust_alert_mssngr");
                        messanger.show();
                        setTimeout(function () {
                            messanger.hide(500);
                        }, 4000);
                        break;

                    case "ALERT":
                        messanger.find('span').html(message);
                        messanger.removeClass().addClass("softAlert cust_alert_mssngr");
                        messanger.show();
                        break;

                    case "ERROR":
                        messanger.find('span').html(message);
                        messanger.removeClass().addClass("failureAlert cust_alert_mssngr");
                        messanger.show();
                        break;

                    case "HIDE":
                        messanger.find('span').html();
                        messanger.hide();
                        break;
                }
            };

            rad.TagManager.closeAlertManager = function (event) {

                $(event.target).parent().hide();
            };

            rad.TagManager.fetchStartDate = function (id) {

                var sDate = $(id).dateControl('getJson').startDate;
                if (typeof (sDate) === "string") {
                    return moment(sDate, "MMM DD YYYY").format(self.options.CalendarDateFormat.toUpperCase());
                } else {
                    return sDate.format(self.options.CalendarDateFormat.toUpperCase());
                }
            };

            rad.TagManager.fetchEndDate = function (id) {

                var eDate = $(id).dateControl('getJson').endDate;
                if (typeof (eDate) === "string") {
                    return moment(eDate, "MMM DD YYYY").format(self.options.CalendarDateFormat.toUpperCase());
                } else {
                    return eDate.format(self.options.CalendarDateFormat.toUpperCase());
                }
            };

            rad.TagManager.saveRule = function (alpha) {

                var ruleInfo = {};
                var tagId = $(alpha).attr('tag_id');
                var targetId = "ruleSaveAlert_" + tagId;
                var ruleId = $(alpha).attr('rule_id');
                ruleInfo.UserAction = $(alpha).attr('user-action');
                ruleInfo.TagId = tagId;

                if (ruleInfo.UserAction == "INSERT") {
                    rad.TagManager.EditRule = false;
                    ruleInfo.PolarisRuleMappingId = "-1";
                    ruleInfo.RuleId = "0";
                    ruleInfo.RuleSetId = "0";
                    ruleInfo.RuleName = $("#txtRuleName_" + tagId).val();
                    ruleInfo.Priority = $("#txtRulePriority_" + tagId).val();
                    ruleInfo.OldPriority = $("#txtRulePriority_" + tagId).attr("old_priority");
                    var pol_override_value = $("#btnAddRule_" + tagId).attr("pol-isoverride"); // $(alpha).parent().parent().parent().parent().parent().find("button").first() //Changed Rajul Mittal
                    if (pol_override_value == 1) {
                        ruleInfo.IsOverride = true;
                    } else if (pol_override_value == 0) {
                        ruleInfo.IsOverride = false;
                    }
                    ruleInfo.RuleText = $("#ruleEditorDiv_tag_" + tagId + " textarea").val();
                    ruleInfo.EffectiveStartDate = rad.TagManager.fetchStartDate("#ruleFormDateControl_" + tagId);
                    ruleInfo.EffectiveEndDate = rad.TagManager.fetchEndDate("#ruleFormDateControl_" + tagId);
                    var ruleClass = $("#ruleEditorDiv_tag_" + tagId + "_ruleEditor").ruleEngine().data('ruleEngine').getGeneratedCode()[0];
                    var classDoc = $("#ruleEditorDiv_tag_" + tagId + "_ruleEditor").ruleEngine().data('ruleEngine').getGeneratedCode()[1];
                    var prettyText = $('#ruleEditorDiv_tag_' + tagId + "_ruleEditor").ruleEngine().data('ruleEngine').PrettifyText();

                }

                else if (ruleInfo.UserAction == "UPDATE") {
                    rad.TagManager.EditRule = true;
                    ruleInfo.PolarisRuleMappingId = $(alpha).attr('pol-rule-mapping-id');
                    //
                    ruleInfo.RuleId = ruleId;
                    ruleInfo.RuleSetId = $(alpha).attr('rule_setID');
                    rad.TagManager.polarisRuleMappingId = ruleInfo.PolarisRuleMappingId;
                    ruleInfo.RuleName = $("#editTxtRuleName_" + ruleId).val();
                    ruleInfo.Priority = $("#editTxtRulePriority_" + ruleId).val();
                    ruleInfo.OldPriority = $("#editTxtRulePriority_" + ruleId).attr("old_priority");
                    if ($("#editIsOverride_" + ruleId).html() == "false")
                        ruleInfo.IsOverride = false;
                    else if ($("#editIsOverride_" + ruleId).html() == "true")
                        ruleInfo.IsOverride = true;
                    ruleInfo.RuleText = $("#ruleEditorDiv_rule_" + ruleId + " textarea").val();
                    ruleInfo.EffectiveStartDate = rad.TagManager.fetchStartDate("#editRuleFormDateControl_" + ruleId);
                    ruleInfo.EffectiveEndDate = rad.TagManager.fetchEndDate("#editRuleFormDateControl_" + ruleId);
                    var ruleClass = $('#ruleEditorDiv_rule_' + ruleId + "_ruleEditor").ruleEngine().data('ruleEngine').getGeneratedCode()[0];
                    var classDoc = $('#ruleEditorDiv_rule_' + ruleId + "_ruleEditor").ruleEngine().data('ruleEngine').getGeneratedCode()[1];
                    var prettyText = $('#ruleEditorDiv_rule_' + ruleId + "_ruleEditor").ruleEngine().data('ruleEngine').PrettifyText();
                }



                if (rad.TagManager.validateRule(ruleInfo,alpha) === true) {

                    $.ajax({
                        url: self.options.serviceURL + '/SaveRule',
                        type: 'POST',
                        data: JSON.stringify({ ruleInfo: JSON.stringify(ruleInfo), ruleText: ruleInfo.RuleText, ruleClass: ruleClass, classDocument: classDoc, dateFormat: self.options.CalendarDateFormat }),
                        contentType: "application/json",
                        dataType: 'json'
                    }).then(function (responseText) {

                        var data = JSON.parse(responseText.d);
                        var tempData = {}; var myMessage = "";
                        tempData.ruleList = data;

                        if (rad.TagManager.RuleModel === undefined) {
                            rad.TagManager.RuleModel = new rad.TagManager.viewModel(tempData);
                        }
                        if (rad.TagManager.EditRule == false) {
                            rad.TagManager.RuleModel.RuleInfo.push(new rad.TagManager.rule(data));
                            $(".onSaveRuleNameHolder").html(data.RuleName);
                            $(".onSaveRuleMsgHolder").html(" has been added.");
                            $(".onSaveRulePopup").show();
                            $(".tag_expand_div_content").addClass("tag_config_list_container_fade");

                        }
                        else if (rad.TagManager.EditRule == true) {
                            for (i = 0; i < rad.TagManager.RuleModel.RuleInfo().length; i++) {
                                if (rad.TagManager.RuleModel.RuleInfo()[i].PolarisRuleMappingId() == rad.TagManager.polarisRuleMappingId) {
                                    rad.TagManager.RuleModel.RuleInfo.splice(i, 1, new rad.TagManager.rule(data));
                                }
                            }
                            // $(".onSaveRuleNameHolder").html(data.RuleName);
                            // $(".onSaveRuleMsgHolder").html(" has been updated.");
                            $("#ruleUpdateMessage_" + tagId).fadeIn(2000);
                            myMessage = data.RuleName + " has been updated";
                            $("#ruleUpdateMessage_" + tagId)[0].innerText = myMessage;
                        }

                        $(".addRulePopupBtn").attr("ruleId", ruleId);
                        $(".addRulePopupBtn").attr("tagId", tagId);

                        //
                        rad.TagManager.bindRule(tagId, false, false);
                        // TODO: get tag list and replace it with existing
                        setTimeout(function () {
                            $("#ruleForm_" + tagId).slideUp();
                            $("#ruleUpdateMessage_" + tagId).fadeOut(2000);
                        }, 2000);

                    }).fail(function (a) {
                        alert(a);
                    });
                }
            };

            rad.TagManager.addRulePopupHandler = function (elem) {

                $(".tag_expand_div_content").removeClass("tag_config_list_container_fade");
                $(".onSaveRulePopup").hide();
                var tag_id = $(elem).attr('tagId');
                $("#btnAddRule_" + tag_id).click();
            };

            rad.TagManager.onRuleSaveClose = function () {

                $(".tag_expand_div_content").removeClass("tag_config_list_container_fade");
                $(".onSaveRulePopup").hide();
            };

            rad.TagManager.qtipHandler = function (elemId, htmlToRender, position, calledFunc, args) {    // qtip js dialogs handler

                if (position == "left") {
                    $("#" + elemId).qtip({                      // element id on which qtip will appear
                        content: htmlToRender,                  // html to render
                        show: {
                            ready: true,
                            solo: true,
                            effect: { type: 'shake' }
                        },
                        hide: 'unfocus',
                        style: {
                            classes: 'qtip-light-left qtip-shadow',
                            width: '300px',
                            maxWidth: '500px'
                        },
                        events: {
                            show: function (event, api) {
                                calledFunc(args, api);        // functionality handler function  
                            }
                        },
                        position: {
                            my: 'right middle',  // Position my top right...
                            at: 'bottom left'  // at the bottom right of...
                        }
                    });
                }
                else if (position == "right") {

                    $("#" + elemId).qtip({                      // element id on which qtip will appear
                        content: htmlToRender,                  // html to render
                        show: {
                            ready: true,
                            solo: true
                        },
                        hide: 'unfocus',
                        style: {
                            classes: 'qtip-light-right qtip-shadow',
                            width: '300px',
                            maxWidth: '500px'
                        },
                        events: {
                            show: function (event, api) {
                                calledFunc(args, api);        // functionality handler function  
                            }
                        },
                        position: {
                            my: 'left middle',  // Position my top right...
                            at: 'right middle'  // at the bottom right of...
                        }
                    });
                }
            };

            rad.TagManager.deleteRuleData = function (elem) {

                var arr = new Array();
                var elemId = $(elem).attr('id');
                var ruleId = $(elem).attr('rule_id');
                var ruleName = $(elem).attr('ruleName');
                var rule_setID = $(elem).attr('rule_setID');
                var tagId = $(elem).attr('tag_id');
                var pol_rule_mapping_id = $(elem).attr('pol-rule-mapping-id');
                var isOverride = $(elem).attr('isOverride');
                var priority = $(elem).attr('priority');
                arr.push(ruleName, tagId, priority, rule_setID, pol_rule_mapping_id);
                rad.TagManager.qtipHandler(elemId, deleteTagDialogHTML, "left", rad.TagManager.deleteRuleFunc, arr);
                $.each($(".col_rule_deleteSelected"), function (e, elem) {
                    $(elem).removeClass("col_rule_deleteSelected");
                });
                $(elem).addClass("col_rule_deleteSelected");
            };

            rad.TagManager.DeletePopUpHandler = function (elem) {
                console.log("DeletePopUpHandler");
                var tagId = $(elem).attr('tagId');
                var del_for = $(elem).attr('del_for');
                if (del_for == 'rule') {
                    var isOverride = $(elem).attr('isOverride');
                    var rule_setID = $(elem).attr('rule_setID');
                    var priority = $(elem).attr('priority');
                    var pol_rule_mapping_id = $(elem).attr('pol_rule_mapping_id');
                    rad.TagManager.deleteRule(pol_rule_mapping_id, isOverride, tagId, rule_setID, priority);
                    rad.TagManager.bindRule(tagId, false, false);
                }
                else {
                    rad.TagManager.deleteTag(tagId);
                }
                $(".delete-dialog-close-btn").click();
            };

            rad.TagManager.deleteRule = function (polarisRuleMappingId, isOverriden, tagId, rule_setID, priority) {

                rad.TagManager.polarisRuleMappingId = polarisRuleMappingId;
                $.ajax({
                    url: self.options.serviceURL + '/DeleteRules',
                    type: 'POST',
                    data: JSON.stringify({ RuleSetId: rule_setID, userName: self.options.actionBy }),
                    contentType: "application/json",
                    dataType: 'json'
                }).then(function (responseText) {

                    var data = JSON.parse(responseText.d);
                    for (i = 0; i < rad.TagManager.RuleModel.RuleInfo().length; i++) {
                        if (rad.TagManager.RuleModel.RuleInfo()[i].PolarisRuleMappingId() == rad.TagManager.polarisRuleMappingId)
                            rad.TagManager.RuleModel.RuleInfo.remove(rad.TagManager.RuleModel.RuleInfo()[i]);
                    }
                    for (var i = 0; i < rad.TagManager.rulePriorties.length; i++) {
                        if (tagId == rad.TagManager.rulePriorties[i].tagId && priority == rad.TagManager.rulePriorties[i].priority && isOverriden == rad.TagManager.rulePriorties[i].isOverride.toString())
                            rad.TagManager.rulePriorties.splice(i, 1);
                    }

                    //in any case delete the content of ruleEditor


                }).fail(function (a) {
                    alert(a);
                });
            };

            // Functionality handlers used in rad.TagManager.qtipHandler() respectively ----- starts
            rad.TagManager.deleteTagFunc = function (args, api) {

                $(".delPopUpTagNameSpan").html('');
                $(".delPopUpTagNameSpan").html('Are you sure to delete  ' + args[1] + " ?");
                $(".delPopUpTagNameSpan").attr("title", args[1]);
                $('.cancelBtnPopUp', api.elements.content).click(function (e) {        // cancel button on qtip
                    api.hide(e);
                    api.destroy();
                    $.each($(".col_delete_selected"), function (e, elem) {
                        $(elem).removeClass("col_delete_selected");
                    });
                });
                $('.deleteBtnPopUp', api.elements.content).click(function (e) {        // cancel button on qtip

                    api.hide(e);
                    $(".DelTagBtn").attr('tagId', args[0]);
                    $(".DelTagBtn").attr('del_for', 'tag');
                    rad.TagManager.DeletePopUpHandler($(".DelTagBtn"));
                    api.destroy();
                });
            };

            rad.TagManager.deleteRuleFunc = function (args, api) {

                $(".delPopUpTagNameSpan").html('');
                $(".delPopUpTagNameSpan").html('Are you sure to delete  ' + args[0] + " ?");
                $(".delPopUpTagNameSpan").attr("title", args[0]);
                $('.cancelBtnPopUp', api.elements.content).click(function (e) {        // cancel button on qtip
                    api.hide(e);
                    api.destroy();
                    $.each($(".col_rule_deleteSelected"), function (e, elem) {
                        $(elem).removeClass("col_rule_deleteSelected");
                    });
                });
                $('.deleteBtnPopUp', api.elements.content).click(function (e) {        // cancel button on qtip
                    api.hide(e);
                    $(".DelTagBtn").attr('tagId', args[1]);
                    $(".DelTagBtn").attr('priority', args[2]);
                    $(".DelTagBtn").attr('rule_setID', args[3]);
                    $(".DelTagBtn").attr('pol_rule_mapping_id', args[4]);
                    $(".DelTagBtn").attr('del_for', 'rule');
                    if ($("li.active[isOverride][tagID='" + args[1] + "']").length == 0)
                        $(".DelTagBtn").attr('isOverride', false);
                    else
                        $(".DelTagBtn").attr('isOverride', true);
                    rad.TagManager.DeletePopUpHandler($(".DelTagBtn"));
                    api.destroy();
                });
            };

            rad.TagManager.conflictDialogFunc = function (args, api) {

                $(".delPopUpTagNameSpan").html('');
                $(".delPopUpTagNameSpan").html('Are you sure to delete  ' + tag_name_val + " ?");
                $(".delPopUpTagNameSpan").attr("title", tag_name_val);
                $('.conflictCloseBtn', api.elements.content).click(function (e) {        // cancel button on qtip
                    api.hide(e);
                });
            };

            rad.TagManager.updateTagFunc = function (args, api) {

                $(".delPopUpTagNameSpan").html('');
                $(".delPopUpTagNameSpan").html('Are you sure to delete  ' + args[0] + " ?");
                $(".delPopUpTagNameSpan").attr("title", args[0]);
                $('.updateCancelBtn', api.elements.content).click(function (e) {        // cancel button on qtip
                    api.hide(e);
                });
                $('.updateBtn', api.elements.content).click(function (e) {            // update btn on qtip
                    api.hide(e);
                    rad.TagManager.saveTagInfo(tagInformation);
                    api.destroy();
                });
                rad.TagManager.updateTagName = new rad.TagManager.delViewModel(args[3], tagInformation.TagName, "#" + args[1], args[2]);
                ko.cleanNode(document.getElementById(args[1]));
                ko.applyBindings(rad.TagManager.updateTagName, document.getElementById(args[1]));
            };
            // Functionality handlers used in rad.TagManager.qtipHandler() respectively ----- ends

            rad.TagManager.deleteTagData = function (elem, event) {

                rad.TagManager.toggleTag = false;
                var tagId = $(elem).attr('value');
                var elemId = $(elem).attr('id');
                var tagName = $(elem).attr('tag-name');
                var data = "";
                var arr = new Array();
                arr.push(tagId, tagName);
                $.ajax({
                    url: self.options.serviceURL + '/CheckDeleteTag',
                    type: 'POST',
                    data: JSON.stringify({ tagId: tagId }),
                    contentType: "application/json",
                    dataType: 'json'
                }).then(function (responseText) {

                    data = JSON.parse(responseText.d);
                    rad.TagManager.qtipHandler(elemId, deleteTagDialogHTML, "right", rad.TagManager.deleteTagFunc, arr); //opening pop up to right
                    //elem.style.color = "black";
                    $.each($(".col_delete_selected"), function (e, elem) {
                        $(elem).removeClass("col_delete_selected");
                    });
                    $(elem).addClass("col_delete_selected");
                    //changing color of the element

                }).fail(function (a) {
                    alert(a);
                });
            };

            rad.TagManager.deleteTag = function (tagId) {

                rad.TagManager.tagId = tagId;
                $.ajax({
                    url: self.options.serviceURL + '/DeleteTags',
                    type: 'POST',
                    data: JSON.stringify({ tagId: tagId, identifier: self.options.pageIdentifier, user: self.options.actionBy, gridInfo: JSON.stringify(self.options.gridInfo) }),
                    contentType: "application/json",
                    dataType: 'json'
                }).then(function (responseText) {
                    var data = JSON.parse(responseText.d);

                    //change the selected one
                    //detroy rule_section // and also destroy what is present in right container

                    //rad.TagManager.TagModel.tagInfo.push(new rad.TagManager.tag(tagInfo, demoTable));
                    if (tagId == $(".selected_Inner_Div").attr("value")) {

                        var prevCollection = $("div#tag_list_item_" + tagId).closest(".selected_div").prev();
                        var nextCollection = $("div#tag_list_item_" + tagId).closest(".selected_div").next();

                        if (nextCollection.length > 0) {
                            //$(collection[collection.length - 1]).click();
                            rad.TagManager.toggleTag = true;
                            rad.TagManager.toggleRuleSection($(nextCollection[0]).children()[0]);
                        }
                        else if (prevCollection.length > 0) {
                            rad.TagManager.toggleTag = true;
                            rad.TagManager.toggleRuleSection($(prevCollection[prevCollection.length - 1]).children()[0]);
                        }
                        else {
                            rad.TagManager.showAddTagPanel();
                        }
                        //rad.TagManager.TagModel.tagInfo.push(new rad.TagManager.tag(tagInfo, demoTable));
                    }



                    //remove it from 
                    for (i = 0; i < rad.TagManager.TagModel.tagInfo().length; i++) {
                        if (rad.TagManager.TagModel.tagInfo()[i].TagId() == rad.TagManager.tagId) {
                            for (j = 0; j < rad.TagManager.tagPriorties.length; j++) {
                                if (rad.TagManager.TagModel.tagInfo()[i].TagPriority() == rad.TagManager.tagPriorties[j]) {
                                    rad.TagManager.tagPriorties.splice(j, 1);
                                    tagNames = tagNames.filter(function (e) { return e !== rad.TagManager.TagModel.tagInfo()[i].TagName() });
                                    break;
                                }
                            }
                            rad.TagManager.TagModel.tagInfo.remove(rad.TagManager.TagModel.tagInfo()[i]);
                        }
                    }
                    if (self.options.IsBindGrid)
                        $find(self.options.gridInfo.GridId).refreshGrid();
                }).fail(function (a) {
                    alert(a);
                });
            };


            //just toggle the visibility of classes
            rad.TagManager.editTagDetails = function (flag) {
                rad.TagManager.editTagDetailFlag = true;
                //resetting all the edit buttona
                rad.TagManager.resetAllEditButtons(flag);
                if ($('#tagTypeSelect').val() == 'String') {
                    //do nothing
                }
                else {
                    $('#tagTypeEditBtn').show();
                }
                //CHECK ONCE 
                //$(".config_form_span").removeClass("inlineBlckTagDetails");

            };

            //to fill the enteries
            rad.TagManager.editTag = function (elem, hideRuleSection, InEditTag) {
                var flag = false;
                rad.TagManager.toggleTag = false;
                rad.TagManager.editTagFlag = true;
                editingtag = true;
                if (self.options.ShowDataSet == 'true')
                    $("#tp-is-persistant").attr("disabled", true);

                $("#tagAlertMessanger").hide();
                $("#tagAlertMessanger2").hide();
                $(".err-input").removeClass("err-input");

                //to remove other black color in edit button
                $.each($(".col_edit_selected"), function (e, elem) {
                    $(elem).removeClass("col_edit_selected");
                });
                //adding class  to the current one
                $(elem).addClass("col_edit_selected");

                var tagId = Number($(elem).attr('value'));
                self.tagIdentifier = Number($(elem).attr('value'));
                var ruleSection = $("#rule_section_" + tagId);

                //buttons are hidden
                $(".addTagTitle").hide();

                $("div").removeClass('selected_div_highlight_edit');

                //removing the class and also making the display of arrow none
                if ($(".tag_config_list_body").find(".selected_div_bckgrnd").length > 0) {
                    //append the rule editor
                    $($(".tag_config_list_body").find(".selected_div_bckgrnd").children()[0]).children().removeClass("Inner_Selected_Tile");
                    $($(".tag_config_list_body").find(".selected_div_bckgrnd").children()[0]).removeClass("selected_Inner_Div");
                    $(".tag_config_list_body").find(".selected_div_bckgrnd").children(".tag_tile_arrow")[0].style.display = "none";
                    //to make the background of previously selected one normal
                    $(".tag_config_list_body").find(".selected_div_bckgrnd").removeClass("selected_div_bckgrnd");
                }

                //make the arrow div visible to this
                $(".gridviewDiv div#tag_list_item_" + tagId).children().addClass("Inner_Selected_Tile");
                $(".gridviewDiv div#tag_list_item_" + tagId).addClass("selected_Inner_Div");
                $(".gridviewDiv div#tag_list_item_" + tagId).siblings(".tag_tile_arrow")[0].style.display = "";
                $(".gridviewDiv div#tag_list_item_" + tagId).parent().addClass("selected_div_bckgrnd");

                $.ajax({
                    url: self.options.serviceURL + '/GetTagInfoForUpdate',
                    type: 'POST',
                    data: JSON.stringify({ tagId: tagId, identifier: self.options.pageIdentifier, user: self.options.actionBy }),
                    contentType: "application/json",
                    dataType: 'json'
                }).then(function (responseText) {
                    //getting the information to fill the edit dropdowns

                    var tagInformation = JSON.parse(responseText.d);
                    if (self.options.getAllTags) {
                        if (tagInformation.CreatedBy == self.options.actionBy)
                            tagInformation.IsEditable = true;
                        else
                            tagInformation.IsEditable = false;
                    }
                    else
                        tagInformation.IsEditable = true;
                    if (tagInformation.IsPersistant)
                        tagInformation.IsEditable = false;
                    if (tagInformation['IsPersistant'] == true)//hello234
                    {
                        $("#tp-is-persistant").attr("disabled", true);
                        console.log(tagInformation);
                        flag = true;
                    }

                    $(".btnSaveTag").attr("tagid", tagInformation.TagId);
                    $(".btnSaveTag").attr("actiontype", "update");
                    $("#tp-tagname").val(tagInformation.TagName);
                    $("#tp-tagname").attr("tag_name", tagInformation.TagName);
                    $("#tp-tagdesc").val(tagInformation.TagDesc);
                    $("#tp-priority").val(tagInformation.TagPriority);
                    $("div[id*=tp_tag_dataset_container]").html(tagInformation.DataSet);
                    $("div[id*=tp_tag_dataset_container]").css({ 'display': '' });

                    //as per the datatype //dropdowns are filled
                    switch (tagInformation.DataTypeDetails.DataType) {
                        case 0:
                            $('#tagTypeSelect').select2('val', 'String');
                            $(".typeCntrl").hide();
                            $(".tagTypeSpan").html('String');
                            break;
                        case 1:
                            $('#tagTypeSelect').select2('val', 'Number');
                            $(".tagTypeSpan").html('Number');
                            $(".negative-select-div,.units-select-div,.tPrecision-div,.tagTypeEditBtn").show();
                            $(".prefix-input-div").hide();
                            break;
                        case 2:
                            $('#tagTypeSelect').select2('val', 'Currency');
                            $(".tagTypeSpan").html('Currency');
                            $(".prefix-input-div,.tPrecision-div,.negative-select-div,.units-select-div,.tagTypeEditBtn").show();
                            break;
                        case 3:
                            $('#tagTypeSelect').select2('val', 'Percentage');
                            $(".tagTypeSpan").html('Percentage');
                            $(".negative-select-div,.tPrecision-div,.tagTypeEditBtn").show();
                            $(".prefix-input-div,.units-select-div").hide();
                            break;
                    }

                    //in case of string no need to fill data //rest cases we need to fill the data
                    if (tagInformation.DataTypeDetails.DataType != 0) {
                        $(".prefix-input").val(tagInformation.DataTypeDetails.Prefix);
                        $(".tPrecision").val(tagInformation.DataTypeDetails.DecimalPlaces);
                        switch (tagInformation.DataTypeDetails.Unit) {
                            case 0:
                                $("#units-select").select2('val', 'None');
                                break;
                            case 1:
                                $("3units-select").select2('val', 'Thousands');
                                break;
                            case 2:
                                $("#units-select").select2('val', 'Millions');
                                break;
                            case 3:
                                $("#units-select").select2('val', 'Billions');
                                break;
                        }
                        switch (tagInformation.DataTypeDetails.NegativeValue) {
                            case 0:
                                $("#negative-select").select2('val', 'Default');
                                break;
                            case 1:
                                $("#negative-select").select2('val', 'DefaultBrac');
                                break;
                            case 2:
                                $("#negative-select").select2('val', 'Colored');
                                break;
                            case 3:
                                $("#negative-select").select2('val', 'ColoredBrac');
                                break;
                        }
                    }

                    $("#tp-priority").attr("max_priority", tagInformation.MaxValidTagPriority);
                    $("#tp-priority").attr("curr_priority", tagInformation.TagPriority);

                    //also fill the spans with data
                    $(".tp-tagname-span").html(tagInformation.TagName);
                    $(".tp-tagdesc-span").html(tagInformation.TagDesc);
                    $(".tp-priority-span").html(tagInformation.TagPriority);
                    $(".tagTypeSpan").html(tagInformation.tagType);
                    $(".tp-reference-dimension-span").html(tagInformation.ReferenceDimension);
                    if (tagInformation.ReferenceAttribute != "-1") {
                        $(".tp-reference-attribute-span").html(tagInformation.ReferenceAttribute);
                    }
                    else {
                        $(".tp-reference-attribute-span").html("-");
                    }
                    $(".tp-tag-default-span").html(tagInformation.DefaultValue);
                    $(".tag_configuration_span").attr('value', tagInformation.TagId);


                    //checkboxes filled as per the data
                    if (tagInformation.IsAlsoDimension == true) {
                        $("#tp-is-dimension").attr("checked", "checked");

                    }
                    else
                        $("#tp-is-dimension").removeAttr("checked");


                    if ($("#tp-is-persistant").attr("clicked")) {
                        $("#tp-is-persistant")[0].setAttribute("clicked", false);
                    }

                    if (tagInformation.IsPersistant == true) {
                        $(".tag_config_form_header").find("#editTagDetails")[0].style.display = "none";
                        $("#tp-is-persistant,.tp-is-persistant-div").show();
                        $("#tp-is-persistant")[0].checked = true;
                        $("#tp-is-persistant").attr("checked", true);
                        $("#tp-is-persistant").attr("disabled", true)
                        if ($("#tp-is-persistant").attr("clicked")) {
                            $("#tp-is-persistant")[0].setAttribute("clicked", true);
                        }
                        if (self.options.ShowDataSet) {
                            $("#PMDataSetContainer").show();
                        }
                    }
                    else if (self.options.isPersistantTextBoxRequired) {
                        $("#tp-is-persistant").removeAttr("checked");
                        $("#tp-is-persistant,.tp-is-persistant-div").show();
                    }
                    else {
                        $("#tp-is-persistant").removeAttr("checked");
                        $("#tp-is-persistant,.tp-is-persistant-div").hide();
                    }

                    if (tagInformation.IsEditable)
                        $(".tag_config_form_header").find("#editTagDetails")[0].style.display = "";
                    else
                        $(".tag_config_form_header").find("#editTagDetails")[0].style.display = "none";


                    //TOASK
                    rad.TagManager.setGridViewClasses();

                    //$(".gridviewDiv").addClass("gridviewDiv_after");
                    $(".tag_config_form_header,.tagConfigRightCaret,.tagNameHolderSpan_div,.tag-expand-div,.tag_expand_div_content").show();

                    //caret shown downward
                    $(".tagConfigRightCaret").find(".fa-caret-right").removeClass("fa-caret-right").addClass("fa-caret-down");

                    $(".tagNameHolderSpan").html(tagInformation.TagName);
                    $(".tagNameHolderSpan").attr('title', tagInformation.TagName);

                    //TOASK
                    $(".tag_header_right_btnset").addClass("tag_header_right_btnset_after");
                    // main header-adjust css (setting position for add tag, view, activity icon)

                    //getting the list item and appending it to this temporary place
                    if (prevRuleSection != "" && prevRuleSection != undefined && prevRuleSection[0] != "" && prevRuleSection[0] != undefined) {
                        //listItem.parent().append(prevRuleSection);
                        //search tagListItem
                        prevRuleSection[0].style.display = "none";
                        var prevTagID = prevRuleSection.attr("tagID");
                        $("div#tag_list_item_" + prevTagID).parent().append(prevRuleSection);
                    }


                    //clearing the html of rule section
                    if ($(".tag_expand_div_content").length > 0 && $(".tag_expand_div_content").is(':visible')) {
                        $(".tag_expand_div_content").html('');
                    }

                    if (hideRuleSection) {
                        $(".tag-expand-div").hide();
                    }
                    //filling the html with ruleSection
                    //$(".tag_expand_div_content").append(ruleSection);
                    prevRuleSection = ruleSection;
                    //putting the whole content in this
                    $(".tag_expand_div_content").append(ruleSection);

                    ruleSection.show();
                    //to get the border
                    $(".tag-expand-div").addClass("tag-expand-div-after");

                    //bind the rules now
                    if (InEditTag == false)
                        rad.TagManager.bindRule(tagId, false, true);
                    else
                        rad.TagManager.bindRule(tagId, false, false);

                    //apply spinner in edit mode
                    //make the tag config form slide down
                    $(".spinner").spinner();
                    $(".tag_config_form").slideDown();

                    var ListViewSection = $(".tag_config_list_body2_div");                      //datatables list view             
                    var GridViewSection = $(".gridviewDiv");

                    if (ListViewSection.is(':visible')) {
                        currView = "LIST";
                        rad.TagManager.setGridView();
                    }
                    else {
                        currView = "GRID";
                        ListViewSection.hide();
                        GridViewSection.show();
                    }

                    if (InEditTag == true) {
                        var operation = "update";
                    }
                    if (InEditTag == false) {
                        var operation = "showtagdetails";
                    }
                    rad.TagManager.getReferenceDimension(tagInformation.ReferenceDimension, tagId, operation);
                    rad.TagManager.getReferenceAttribute(tagInformation.ReferenceDimension, tagInformation.ReferenceAttribute, tagId);
                    rad.TagManager.getReferenceData(tagInformation.ReferenceDimension, tagInformation.ReferenceAttribute, tagInformation.DefaultValue, tagId);
                    //#toBeAdded Rajul Mittal
                    /* if (self.options.pageIdentifier == "pnl_positiondetails" || self.options.pageIdentifier == "pnl_positionallocation") {
                    if ($("#tp-is-persistant").attr('checked'))
                    rad.TagManager.getDataSet(tagId);
                    }*/

                    if (InEditTag) {
                        rad.TagManager.editTagDetails(flag);
                    }

                    $("#tp-is-persistant").attr("disabled", true);
                    if (tagInformation.IsAlsoDimension == true) {
                        $("#tp-is-dimension").attr("checked", "checked");
                        $("#tp-is-dimension")[0].checked = true;
                        $("#tp-is-dimension").attr("disabled", true);
                    }
                    else {
                        $("#tp-is-dimension").removeAttr("checked");
                        $("#tp-is-dimension")[0].checked = false;
                    }
                    $(" #tp-reference-attribute-container--1").closest(".form-element").css({ 'pointer-events': 'none' });
                    $(" #s2id_tagTypeSelect").closest(".form-element").css({ 'pointer-events': 'none' });
                    //$("#s2id_tagTypeSelect").css({'display' : ''});
                    //rad.TagManager.initRefDimRefAttrDropdowns(tagId);
                }).fail(function (a) {
                    alert(a);
                });
            };

            rad.TagManager.saveTagInfo = function (tagInfo) {

                self.options.isSaveTagClicked = true;
                $.ajax({
                    url: self.options.serviceURL + '/SaveTagInfoNeo',
                    type: 'POST',
                    data: JSON.stringify({ tagInfoJson: JSON.stringify(tagInfo), identifier: self.options.pageIdentifier, user: self.options.actionBy, gridInfo: JSON.stringify(self.options.gridInfo), filterquery: JSON.stringify(self.options.filterQuery), assemblyName: self.options.assemblyName, className: self.options.className, dateColumnName: self.options.DateColumnName }),
                    contentType: "application/json",
                    dataType: 'json'
                }).then(function (responseText) {
                    //  rad.TagManager.callInitializeTagManager(self.options.pageIdentifier, self.options.actionBy, JSON.stringify(self.options.gridInfo), self.options.filterQuery, self.options.assemblyName, self.options.className, self.options.DateColumnName); // Initialize Tag Manager
                    var tagInfo = JSON.parse(responseText.d);

                    if (self.options.getAllTags) {
                        if (tagInfo.CreatedBy == self.options.actionBy) {
                            tagInfo.TagIsEditable = true;
                            tagInfo.IsEditable = true;
                        }
                        else {
                            tagInfo.TagIsEditable = true;
                            tagInfo.IsEditable = false;
                        }
                    }
                    else {
                        if (!tagInfo.IsPersistant) {
                            tagInfo.TagIsEditable = true;
                            tagInfo.IsEditable = true;
                        }
                    }
                    if (self.options.ExternalFunctionForSaveTag != null)
                        self.options.ExternalFunctionForSaveTag(tagInfo);
                    //  rad.TagManager.callInitializeTagManager(self.options.pageIdentifier, self.options.actionBy, JSON.stringify(self.options.gridInfo), self.options.filterQuery, self.options.assemblyName, self.options.className, self.options.DateColumnName); // Initialize Tag Manager
                    var tagId = tagInfo.TagId;
                    var temp = $("#tag_list_item" + tagId);
                    tagInfo.Edit = '<div class="form-element tag-list-col-icon col-edit fa fa-pencil-square-o" style="margin-top: 4px !important; display: inline-block;font-size:15px" value="' + tagInfo.TagId + '" tag-name="' + tagInfo.TagName + '" title="Edit Tag" onclick="var event = arguments[0] || window.event;event.stopPropagation;rad.TagManager.editTag(this);return false;"></div>';
                    tagInfo.Delete = '<div class="form-element tag-list-col-icon col-delete fa fa-trash-o" style="margin-top: 4px !important;font-size:15px; display: inline-block;" id="dt_del_tag_btn_' + tagInfo.TagId + '"  value="' + tagInfo.TagId + '" tag-name="' + tagInfo.TagName + '" onclick="var event = arguments[0] || window.event;event.stopPropagation;rad.TagManager.deleteTagData(this);" title="Delete Tag"></div>';
                    tagInfo.CustTagPriority = '<div class="form-element tag-list-col cust_tag_prior prior_align" title="Tag Priority" style="">' + tagInfo.TagPriority + '</div>'

                    if ($find(self.options.gridInfo.GridId) != null)
                        $find(self.options.gridInfo.GridId).get_GridInfo().ComputedNewColumns.push(tagInfo.TagRealName);

                    if (rad.TagManager.editTagFlag == false) {
                        //rad.TagManager.TagModel.tagInfo.push(new rad.TagManager.tag(tagInfo));
                        tagNames.push(tagInfo.TagName);
                        rad.TagManager.tagPriorties.push(tagInfo.TagPriority);
                        if (tagInfo.ReferenceAttribute == "-1")
                            tagInfo.ReferenceAttribute = "-";
                        rad.TagManager.TagModel.tagInfo.push(new rad.TagManager.tag(tagInfo, demoTable));
                        rad.TagManager.alertMessanger("tagAlertMessanger2", "SUCCESS", "Tag Saved successfully! Now add rules to tag...");

                    } else if (rad.TagManager.editTagFlag == true) {
                        for (i = 0; i < rad.TagManager.TagModel.tagInfo().length; i++) {
                            if (rad.TagManager.TagModel.tagInfo()[i].TagId() == tagId) {
                                if (tagInfo.ReferenceAttribute == "-1")
                                    tagInfo.ReferenceAttribute = "-";
                                rad.TagManager.TagModel.tagInfo.splice(i, 1, new rad.TagManager.tag(tagInfo, demoTable));
                            }
                        }
                        //rad.TagManager.TagModel.tagInfo.push(new rad.TagManager.tag(tagInfo, demoTable));
                        rad.TagManager.alertMessanger("tagAlertMessanger2", "SUCCESS", "Tag Updated successfully!");
                        $(".tag_configuration_span").find(".fa-caret-down").removeClass("fa-caret-down").addClass("fa-caret-right");
                        $(".tag-expand-div").removeClass("tag-expand-div-after");
                        rad.TagManager.bindRule(tagId, false, false);
                    }
                    $(".selected_div").addClass("selected_div_fixed_collpased");

                    //make the arrow div visible to this
                    $(".gridviewDiv div#tag_list_item_" + tagId).children().addClass("Inner_Selected_Tile");
                    $(".gridviewDiv div#tag_list_item_" + tagId).addClass("selected_Inner_Div");
                    $(".gridviewDiv div#tag_list_item_" + tagId).siblings(".tag_tile_arrow")[0].style.display = "";
                    $(".gridviewDiv div#tag_list_item_" + tagId).parent().addClass("selected_div_bckgrnd");

                    //changing the rule Section now
                    $(".tag_config_form").delay(1000).slideUp(function () {
                        var currRuleSection = $("#rule_section_" + tagId);
                        $(".tag_configuration_span").attr('value', tagId);

                        //$(".tag_expand_div_content").html('');
                        prevRuleSection = currRuleSection; //update the previous rule section
                        $(".tag_expand_div_content").html("");
                        $(".tag_expand_div_content").append(currRuleSection);
                        $(".tag_config_form_header").addClass("tag_config_form_header_after");
                        currRuleSection.show();
                        $(".tagConfigRightCaret,.tag-expand-div,.tagNameHolderSpan_div").show();
                        if (rad.TagManager.editTagFlag == false) {
                            $("#btnAddRule_" + tagId).click();
                        }
                        $(".addTagTitle").hide();
                        $(".tagNameHolderSpan").html(tagInfo.TagName);
                        $(".tagNameHolderSpan").attr('title', tagInfo.TagName);
                    });
                    if (tagInfo.IsEditable)
                        $(".tag_config_form_header").find("#editTagDetails")[0].style.display = "";
                    else
                        $(".tag_config_form_header").find("#editTagDetails")[0].style.display = "none";
                    // rad.TagManager.callInitializeTagManager(self.options.pageIdentifier, self.options.actionBy, JSON.stringify(self.options.gridInfo), self.options.filterQuery, self.options.assemblyName, self.options.className, self.options.DateColumnName); 

                }).fail(function (a) {
                    alert(a);
                });
            };

            rad.TagManager.activityClose = function () {

                var activitySection = $(".tag_activity_div");
                if (activitySection.is(':visible')) {
                    activitySection.hide();
                    $(".tag_config_list").addClass("tag_config_list_expanded");
                    $(".tag_header_info_icon").removeClass("info_icon_after");
                }
            };

            rad.TagManager.showActivitySection = function () {

                var activitySection = $(".tag_activity_div");
                if (activitySection.is(':visible')) {
                    $(".tag_header_info_icon").removeClass("info_icon_after");
                    activitySection.hide();
                    $(".tag_config_list").addClass("tag_config_list_expanded");
                    $(".tag_header_info_iconSpan").removeAttr('data-original-title');
                    $(".tag_header_info_iconSpan").attr('data-original-title', 'View Details');
                } else {
                    $(".tag_header_info_icon").addClass("info_icon_after");
                    activitySection.show();
                    $(".tag_config_list").removeClass("tag_config_list_expanded");
                    $(".tag_header_info_iconSpan").removeAttr('data-original-title');
                    $(".tag_header_info_iconSpan").attr('data-original-title', 'Hide Details');
                }
            };

            rad.TagManager.modalClose = function (id, notDelModal) {
                if (self.options.isSaveTagClicked == true) {
                    //                    $find(self.options.gridInfo.GridId).get_GridInfo().ClearSerializationData = false;
                    self.options.isSaveTagClicked = false;
                    if (self.options.IsBindGrid)
                        $find(self.options.gridInfo.GridId).refreshGridWithoutClientSideBinding();

                    // rad.TagManager.callInitializeTagManager(self.options.pageIdentifier, self.options.actionBy, JSON.stringify(self.options.gridInfo), self.options.filterQuery, self.options.assemblyName, self.options.className, self.options.DateColumnName);

                }
                if (self.options.IsBindGrid) {
                    if ($("#" + self.options.gridInfo.GridId).hasClass("xlOpacityForGrid")) {
                        $("#" + self.options.gridInfo.GridId).removeClass(); //check
                    }
                    if ($("#" + self.options.gridInfo.GridId + "_upperHeader_Div").hasClass("xlOpacityForGrid")) {
                        $("#" + self.options.gridInfo.GridId + "_upperHeader_Div").removeClass();

                    }
                    if ($("#" + self.options.gridInfo.GridId + "_bodyDiv").hasClass("xlOpacityForGrid")) {
                        $("#" + self.options.gridInfo.GridId + "_bodyDiv").removeClass();

                    }
                    if ($("#" + self.options.gridInfo.GridId + "_headerDiv").hasClass("xlOpacityForGrid")) {
                        $("#" + self.options.gridInfo.GridId + "_headerDiv").removeClass();

                    }
                }
                if (notDelModal) {
                    var tagID = "";
                    //removing the class and also making the display of arrow none
                    if ($(".selected_div_bckgrnd").length > 0) {
                        tagID = $($(".selected_div_bckgrnd").children()[0]).attr("value");
                        $($(".selected_div_bckgrnd").children()[0]).removeClass("selected_Inner_Div");
                        $($(".selected_div_bckgrnd").children()[0]).children().removeClass("Inner_Selected_Tile");
                        $(".selected_div_bckgrnd").children(".tag_tile_arrow")[0].style.display = "none";
                        $(".selected_div_bckgrnd").removeClass("selected_div_bckgrnd");
                    }
                    $(".tag_header_right_btnset").removeClass("tag_header_right_btnset_after");
                    $("div[id^=rule_section_]").hide();

                    // var listItem = $("div#tag_list_item_" + tagID);
                    var prevRuleSection = $("#rule_section_" + tagID);
                    if (prevRuleSection != "" && prevRuleSection != undefined && prevRuleSection[0] != "" && prevRuleSection[0] != undefined) {
                        //listItem.parent().append(prevRuleSection);
                        //search tagListItem
                        prevRuleSection[0].style.display = "none";
                        var prevTagID = prevRuleSection.attr("tagID");
                        $("div#tag_list_item_" + prevTagID).parent().append(prevRuleSection);
                    }

                    $(".tag_expand_div_content").html('');

                    $(".tag_config_form,.addTagTitle,.tagNameHolderSpan_div,.tag_config_form_header,.tag-expand-div").hide();    // hide Edit btn for tag configuration header
                    rad.TagManager.removeGridViewClasses();
                    rad.TagManager.setListView();
                    //to insert //reset the default values
                    rad.TagManager.resetTagForm(tagID);

                    //destroying the ruleeditor //if it is present there
                    if ($("div[id$=_ruleEditor]").length > 0) {
                        $("div[id$=_ruleEditor]").data('ruleEngine').Destroy();
                        $("div[id$=_ruleEditor]").remove();
                        $("div[id^=editRuleFormDateControl_]").html('');
                        $("div[id^=ruleFormDateControl_]").html('');
                    }
                }

                //$("#tagTypeSelect").select2("destroy");
                //$("#tagTypeSelect").prev().remove();
                // $("#tagTypeSelect").select2({
                // data : [{ id: "String", text: 'String' }, { id: "Number", text: 'Number' }, { id: "Currency", text: 'Currency' }, { id: "Precentage", text: 'Precentage' }],
                // minimumResultsForSearch: -1
                // });
                //$("#tagTypeSelect").select2();
                $(id).click();
                $(".tag_config_list_container").removeClass("tag_config_list_container_fade");
                $(".tag_expand_div_content").removeClass("tag_config_list_container_fade");
                $(".runQuitBtn").removeAttr("disabled");
                $(".runQuitBtn").css({ "cursor": "pointer", "opacity": "1" });

                //rad.TagManager.initDropdowns();
            };




            rad.TagManager.RunTagsHandler = function () {
                var CustomFilterQuery = {};
                var startDate = $(".runTagsDateControl").dateControl('getJson').startDate;
                CustomFilterQuery.startDate = startDate.format("YYYYMMDD");

                var endDate = $(".runTagsDateControl").dateControl('getJson').endDate;
                CustomFilterQuery.endDate = endDate.format("YYYYMMDD");

                var collection = rad.TagManager.TagModel.tagInfo();
                self.options.isPersistantTag = false;
                for (var i = 0; i < collection.length; i++) {
                    if (collection[i].IsPersistant()) {
                        self.options.isPersistantTag = true;
                        break;
                    }
                }
                if (self.options.isPersistantTag) {

                    $.ajax({
                        url: self.options.serviceURL + '/TriggerTaggingTask',
                        type: 'POST',
                        data: JSON.stringify({ startDate: CustomFilterQuery.startDate, endDate: CustomFilterQuery.endDate, identifier: self.options.pageIdentifier, user: self.options.actionBy, getAllTags: self.options.getAllTags }),
                        contentType: "application/json",
                        dataType: 'json'
                    }).then(function (responseText) {
                        //function call here
                        var collection = rad.TagManager.TagModel.tagInfo();
                        self.options.isPersistantTag = false;
                        for (var i = 0; i < collection.length; i++) {
                            if (collection[i].IsPersistant()) {
                                self.options.isPersistantTag = true;
                                break;
                            }
                        }
                        if (self.options.isPersistantTag)//rule is non-persistent //then don't call this
                            self.options.ExternalFunction();
                        //	rad.TagManager.callInitializeTagManager(self.options.pageIdentifier, self.options.actionBy, JSON.stringify(self.options.gridInfo), self.options.filterQuery, self.options.assemblyName, self.options.className, self.options.DateColumnName); // Initialize Tag Manager
                    }).fail(function (a) {
                        //$(".tag_config_list_container").removeClass("tag_config_list_container_fade");
                        alert(a);
                    });
                }
                else {

                    rad.TagManager.callInitializeTagManager(self.options.pageIdentifier, self.options.actionBy, JSON.stringify(self.options.gridInfo), self.options.filterQuery, self.options.assemblyName, self.options.className, self.options.DateColumnName); // Initialize Tag Manager
                }


                $(".runQuitBtn").removeAttr("disabled");
                $(".runQuitBtn").css({ "cursor": "pointer", "opacity": "1" });
            };
        },
        /* -->1 Leave as it is */
        getJson: function () {

            return this.options;
        },

        reInitializeGrid: function (gridInfo) {
            //then load info again
            gridInfo.ComputedNewColumns = this.options.gridInfo.ComputedNewColumns;
            neogridloader.create(this.options.gridInfo.GridId, this.options.actionBy, gridInfo, "");
            var gridID = this.options.gridInfo.GridId;

            $.ajax({
                url: this.options.serviceURL + '/InitializeTagManager',
                type: 'POST',
                contentType: "application/json",
                data: JSON.stringify({ identifier: this.options.pageIdentifier, user: this.options.actionBy, gridInfo: JSON.stringify(gridInfo), filterquery: JSON.stringify(this.options.filterQuery), assemblyName: this.options.assemblyName, className: this.options.className, dateColumnName: this.options.DateColumnName, getAllTags: self.options.getAllTags, isBindGrid: self.options.IsBindGrid }),
                dataType: 'json'
            }).then(function (responseText) {
                if (self.options.IsBindGrid)
                    $find(gridID).refreshGrid();
                $(".runTagsDateControl").dateControl("destroy");
                rad.TagManager.InitializeRunQtip();
            }).fail(function (a) {
                console.log("Initialization Failed:" + a);
            });
        },



        show: function () {

            if (this.options.box) {
                this.options.box.modal('show');
                rad.TagManager.InitializeRunQtip();
                $(".tag_config_list_body2_div").find(".dataTables_scrollHeadInner .dataTable").css("width", (($(".my-custom-modal-body .bootbox-body")[0].offsetWidth - 100) + "px"));

                //in case fade class is there
                $(".tag_config_list_container").removeClass("tag_config_list_container_fade");

                //changing the landing page as per the condition
                var collection = $("#tag_config_list_body2 tbody tr");
                if (collection.length < 12) {
                    if (collection.length == 0)
                        rad.TagManager.showAddTagPanel();
                    else
                        $(collection[0]).click();
                }
            }
            else
                this.options.show = true;
        },

        destroyPlugin: function () {

            $(".my-modal-dialog").data('modal', null);
            $(".my-modal-dialog").parent().remove();
            var myDiv = document.createElement("div");
            var id = this.element[0].id;

            myDiv.setAttribute("id", id);
            this.element.parent().prepend(myDiv);
            this.element.remove();
        },


        FilterData: function () {

            rad.TagManager.callInitializeTagManager(this.options.pageIdentifier, this.options.actionBy, JSON.stringify(this.options.gridInfo), this.options.filterQuery, this.options.assemblyName, this.options.className, this.options.DateColumnName); // Initialize Tag Manager
        },
        getViewModel: function () {

            return viewModel;
        },
        setJson: function (optionData) {

        },
        _setOptions: function () {

            this._superApply(arguments);
        },
        _setOption: function (key, value) {

            this._super(key, value);
        },
        _destroy: function () {

            //TODO framework.deRegisterWidget(this);
        }
    });
    $('div[type="iago:tagging"]').each(function (index, value) {

        if ($(value).attr('data-options-type') === "JSON")
            $(value).popup(JSON.parse($(value).attr('data-options')));
    });
});


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

/*$('tp-tagname-span').keydown(function(){
var input=$(this);

var is_special=input.val();
if(is_special == '!' || is_special =='$' || is_special =='@' || is_special =='#'||is_special =='%' ||is_special =='&'||is_special =='*')
{
			

}
	
	
})*/
