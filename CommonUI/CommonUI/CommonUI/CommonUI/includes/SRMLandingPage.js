//$(document).ready(function () {
//    srmLandingPage.init();
//});

var srmLandingPage = {
    _pageViewModelInstance: {},
    _commonServiceLocation: "/BaseUserControls/Service/CommonService.svc",
    _path: "",
    _moduleID_moduleName_map: [],
    _isBindingsApplied: false,
    _leftMenuPath: "",
    _moduleID: "",
    _widthForAnimation: "",
    setPath: function () {
        var path = window.location.protocol + '//' + window.location.host;
        var pathname = window.location.pathname.split('/');

        $.each(pathname, function (ii, ee) {
            if ((ii !== 0) && (ii !== pathname.length - 1))
                path = path + '/' + ee;
        });

        srmLandingPage._path = path;


        var leftMenuPath = "";
        if (typeof (window.parent.leftMenu) !== "undefined")
            leftMenuPath = window.parent.leftMenu;
        else if (typeof (window.parent.parent.leftMenu) !== "undefined")
            leftMenuPath = window.parent.parent.leftMenu;
        srmLandingPage._leftMenuPath = leftMenuPath;
    },
    initValues: function () {
        let height = $(window).height() - $("#SRMLandingPageListParent").offset().top - 20;
        $("#SRMLandingPageListParent").css('height', height);

    },
    init: function () {

        srmLandingPage.setPath();
        srmLandingPage.initValues();
        srmLandingPage.setWidth();

        var listofTabsToBindFunctionsWith = CommonModuleTabs.ModuleList();

        for (i in listofTabsToBindFunctionsWith) {
            item = listofTabsToBindFunctionsWith[i];
            srmLandingPage._moduleID_moduleName_map[item.displayName.toLowerCase()] = item.moduleId;
            //draftsStatus._moduleID = item.moduleId;
            switch (item.displayName.toLowerCase().trim()) {
                case "securities":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: srmLandingPage.secMasterInitLandingPage });
                    break;
                case "refdata":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: srmLandingPage.refMasterInitLandingPage });
                    break;
                case "funds":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: srmLandingPage.fundMasterInitLandingPage });
                    break;
                case "parties":
                    SRMProductTabs.setCallback({ key: item.displayName.toLowerCase(), value: srmLandingPage.partyMasterInitLandingPage });
                    break;
            }
        }

    },
    secMasterInitLandingPage: function () {
        srmLandingPage._moduleID = srmLandingPage._moduleID_moduleName_map["securities"];
        srmLandingPage.initSpecificModule();
    },
    refMasterInitLandingPage: function () {
        srmLandingPage._moduleID = srmLandingPage._moduleID_moduleName_map["refdata"];
        srmLandingPage.initSpecificModule();
    },
    fundMasterInitLandingPage: function () {
        srmLandingPage._moduleID = srmLandingPage._moduleID_moduleName_map["funds"];
        srmLandingPage.initSpecificModule();
    },
    partyMasterInitLandingPage: function () {
        srmLandingPage._moduleID = srmLandingPage._moduleID_moduleName_map["parties"];
        srmLandingPage.initSpecificModule();
    },
    setWidth: function () {
        $("#SRMLandingPageList").css('width', $(window).width() - 10);
        srmLandingPage._widthForAnimation = $(window).width() - 1;
    },
    refreshValues: function(){
        $("#SRMLandingPageSearchTextInputBox").val("");
        srmLandingPage._pageViewModelInstance.searchText("");
        $("#SRMLandingPageListParent").scrollTop(0);
    },
    initSpecificModule(){
        //srmLandingPage.refreshValues();
        srmLandingPage.getData();
        srmLandingPage.applyBindings();
    },
    applyBindings: function(){
        $(window).resize(function () {
            return false;
        });
    },
    getData: function () {
        onServiceUpdating();
        $.ajax({
            type: 'POST',
            url: srmLandingPage._path + srmLandingPage._commonServiceLocation + "/GetLandingPageInfo",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify({ moduleId: srmLandingPage._moduleID }),
            success: function (data) {
                srmLandingPage.onSuccessDataFetch(data.d)

            },
            error: function (m) {
                console.log("Data cannot be fetched");
            },
            complete: function () {
                onServiceUpdated();
            }
        });


    },
    onSuccessDataFetch: function (data) {
        srmLandingPage.applyKOBindings(JSON.parse(data));
    },
    applyKOBindings: function (data) {

        if (!srmLandingPage._isBindingsApplied) {
            srmLandingPage._pageViewModelInstance = new srmLandingPage.pageViewModel(data);
            srmLandingPage._isBindingsApplied = true;
            ko.applyBindings(srmLandingPage._pageViewModelInstance, document.getElementById("SRMLandingPageParent"));
        }
        else {
            srmLandingPage._pageViewModelInstance.dataList.removeAll();
            var self = srmLandingPage._pageViewModelInstance.dataList;
            for (var i in data) {
                let currData = data[i];
                self.push(new srmLandingPage.chainViewModel(currData));
            }
        }
        srmLandingPage.refreshValues();
    },
    pageViewModel: function (data) {
        var self = this;
        self.dataList = ko.observableArray([]);
        //self.dataListLeft = ko.observableArray([]);
        //self.dataListRight = ko.observableArray([]);
        self.searchText = ko.observable("");

            for (var i in data) {
                let currData = data[i];
                self.dataList.push(new srmLandingPage.chainViewModel(currData));
        }


        //filters search into two separate lists
        self.searchResult = ko.computed(function () {
            var searchText = self.searchText().toLowerCase();

            var res = ko.utils.arrayFilter(self.dataList(), function (item) {
                return item.name.toLowerCase().indexOf(searchText) >= 0
            });


            var noResultDiv = $("#SRMLandingPageNoResult");
            var listParent = $("#SRMLandingPageListParent");
            if (res.length == 0) {
                noResultDiv.css('display', 'block');
                listParent.css('display', 'none');
                if (srmLandingPage._leftMenuPath)
                    srmLandingPage._leftMenuPath.showNoRecordsMsg('No types exist.', noResultDiv);
                noResultDiv.css('width', "100%");
            }
            else {
                noResultDiv.css('display', 'none');
                if (srmLandingPage._leftMenuPath)
                    srmLandingPage._leftMenuPath.hideNoRecordsMsg();
                listParent.css('display', 'block');
            }

            return res;
        });

        self.clickedType = function (item, event) {
            console.log('clicked');
            let name = item.name;
            let id = item.id;
            let currentDiv = $(event.target);
            let parentDiv = currentDiv.closest('.SRMLandingPageListItem');
            let top = parentDiv.offset().top;
            let left = parentDiv.offset().left;

            srmLandingPage.animateDiv(top, left, name);

            srmLandingPage.applyTabClick(item.id, srmLandingPage._moduleID, item.templateId);
            srmLandingPage.selectedTabToDisplay(currentDiv.attr('tabidentifier'));

        }

    },
    chainViewModel: function (currData) {
        var self = this;
        self.name = currData.sectype_name ? currData.sectype_name : currData.entity_display_name;
        self.attrCount = currData.attribute_count <= 9 ? '0' + currData.attribute_count : currData.attribute_count;
        self.legCount = currData.leg_count <= 9 ? '0' + currData.leg_count : currData.leg_count;
        self.upstreamCount = currData.upstream_count <= 9 ? '0' + currData.upstream_count : currData.upstream_count;
        self.downstreamCount = currData.downstream_count <= 9 ? '0' + currData.downstream_count : currData.downstream_count;
        self.id = currData.sectype_id ? currData.sectype_id : currData.entity_type_id;
        self.templateId = currData.template_id ? currData.template_id : -1;
    },

    animateDiv: function (top, left, name) {
        $("#SRMLandingPageModules").css('display', 'block');
        $("#SRMLandingPageModules").css('left', left);
        $("#SRMLandingPageModules").css('top', top);
        $("#SRMLandingPageModules").animate({
            left: 0, width: srmLandingPage._widthForAnimation, height: $(window).height() - 10, top: 0
        }, '500',
        function () {
            //hide first divs
            $("#SRMLandingPageParent").css('display', 'none');
            $("#srm_moduleTabs").parent().css('display', 'none');

            //show, hide roles div
            if (srmLandingPage._moduleID == 18 || srmLandingPage._moduleID == 20) {
                $("#SRMRoles").css('visibility', 'visible');
                $("#SRMTabAssociation").css('visibility', 'hidden');
            }
            else if (srmLandingPage._moduleID == 6) {
                $("#SRMTabAssociation").css('visibility', 'hidden');
                $("#SRMRoles").css('visibility', 'hidden');
            }    
            else {
                $("#SRMRoles").css('visibility', 'hidden');
                $("#SRMTabAssociation").css('visibility', 'visible');
            }


            //close button binding
            $("#SRMLandingPageCloseButton").off('click').on('click', function () {
                $("#SRMLandingPageModules").css('width', 0).css('height', 0).css('display', 'none');
                $("#SRMLandingPageParent").css('display', 'block');
                $("#srm_moduleTabs").parent().css('display', 'block');
                $("#InnerFrameId1").attr('src', "");
            });

            



        });
        //set text
        $("#SRMTypeHeaderText").text(name);



        //display new div


    },
    applyTabClick: function (typeId, moduleId, templateId) {
        $("#SRMTabAttributes").off('click').on('click', function () {
            let path = srmLandingPage._path + "/BaseUserControls/Modeler/AttributeSetupPage.aspx?module=" + moduleId + "&TypeId=" + typeId + "&templateId=" + templateId;
            srmLandingPage.toggleIframe(path);
            $("#SRMTabAttributes").addClass("SRMSelectedTabNamesHeader");
        });

       $("#SRMTabLegs").off('click').on('click', function () {
           let path = srmLandingPage._path + "/BaseUserControls/Modeler/AttributeLegSetupPage.aspx?module=" +moduleId + "&TypeId=" + typeId + "&templateId=" + templateId;
           srmLandingPage.toggleIframe(path);
           $("#SRMTabLegs").addClass("SRMSelectedTabNamesHeader");
           });

        $("#SRMTabRules").off('click').on('click', function () {
            let path = srmLandingPage._path + "/SRMRuleCatalogParent.aspx?moduleID=" + moduleId + "&typeID=" + typeId + "&identifier=SRM_RuleCatalog";
            srmLandingPage.toggleIframe(path);
            $("#SRMTabRules").addClass("SRMSelectedTabNamesHeader");
        });

        $("#SRMTabAssociation").off('click').on('click', function () {
            let path = srmLandingPage._path + "/SRMAccessAssociation.aspx?&typeId=" + typeId;
            srmLandingPage.toggleIframe(path);
            $("#SRMTabAssociation").addClass("SRMSelectedTabNamesHeader");
        });

        $("#SRMTabAccess").off('click').on('click', function () {
            let path = srmLandingPage._path + "/SRMAccess.aspx?moduleId=" + moduleId + "&typeId=" + typeId;
            srmLandingPage.toggleIframe(path);
            $("#SRMTabAccess").addClass("SRMSelectedTabNamesHeader");
        });

        $("#SRMTabMappings").off('click').on('click', function () {
            let path = srmLandingPage._path + "/SRMDataSourceSystemMapping.aspx?moduleId=" + moduleId + "&typeId=" + typeId + "&templateId=" + templateId;
            srmLandingPage.toggleIframe(path);
            $("#SRMTabMappings").addClass("SRMSelectedTabNamesHeader");
        });

        $("#SRMRoles").off('click').on('click', function () {
            let path = srmLandingPage._path + "/SRMLegRoleMap.aspx?&typeId=" + typeId;
            srmLandingPage.toggleIframe(path);
            $("#SRMRoles").addClass("SRMSelectedTabNamesHeader");
        });

    },
    

    toggleIframe: function (source) {
        srmLandingPage.resizeIframe();
        $("#InnerFrameId1").attr('src', source);
        $(".SRMTabItem").removeClass("SRMSelectedTabNamesHeader");
    },
    resizeIframe: function () {
        var iframeHeight, width;
        iframeHeight = $(window).height() - 35; //50 height of headers
        width = $(window).width() - 5;
        $("#InnerFrameId1").height(iframeHeight).width(width);
    },
    selectedTabToDisplay: function (tabName) {
        switch (tabName) {
            case "attribute": $("#SRMTabAttributes").click(); break;
            case "upstream": $("#SRMTabMappings").click(); break;
            case "downstream": $("#SRMTabMappings").click(); break;
            case "leg": $("#SRMTabLegs").click(); break;
            case "rules": $("#SRMTabRules").click(); break;
            //access, associations, roles
            default:
                if (srmLandingPage._moduleID == 18 || srmLandingPage._moduleID == 20) {
                    $("#SRMRoles").click();
                }

                else {
                    $("#SRMTabAttributes").click();
                }
                break;
        }
    }


}