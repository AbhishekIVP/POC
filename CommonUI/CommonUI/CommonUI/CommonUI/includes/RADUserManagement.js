/*! UserManagement-Package - v1.0.0 - 2018-02-26 8:17:32 */

RADUserManagement = {};
RADUserManagement.initialize = function (initObj) {
        $.extend(RADUserManagement, initObj);
        var $userMangementTabDiv = $("<div>", {
            id: "RUserManementTabDiv",
            class: "RUserManementTab"
        });
        $("#" + RADUserManagement.contentBodyId).empty();
        $("#" + RADUserManagement.contentBodyId).append($userMangementTabDiv);

    var obj = new AllUserUsers();
    obj.selectedTabId = (initObj.selectedTabId!="") ? initObj.selectedTabId : "Users";
    obj.init();
};

var AllUserUsers = function () {
    // this.someObjectVariable = "anything";
};
var userName
var selectedAccount
var ActiveUser = new Array();
var Priviliges = new Array();
var ArrayOfRoles = new Array();
var GlobalFlagSearch;
var isEditMode = false;
var isUserCreationAllowed = false;
window.ScrollVariableup = 0
window.ScrollVariableGroupup = 0;
window.ScrollVariableRolesup = 0;
AllUserUsers.instance = undefined;
AllUserUsers.prototype.init = function (sandBox) {
    var self = this
    /* Load HTML Template */
    $("#pageHeaderLabel").empty();
    AllUserUsers.instance = this;
    var selectedTabId = self.selectedTabId;


    AllUserUsers.prototype.tabInfo = {
        tab: [
            {
                id: "Users",
                name: "Users"
            },
            {
                id: "Groups",
                name: "Groups"
            },
            {
                id: "Roles",
                name: "Roles"
            },
            {
                id: "Accounts",
                name: "Accounts"
            }
        ]
    };


    if (selectedTabId != ""){
        var currentTabInfo = $.grep(AllUserUsers.prototype.tabInfo.tab, function (e)
        { return e.id == selectedTabId; });
        if (currentTabInfo.length == 0)
            AllUserUsers.prototype.tabInfo.tab[0].isDefault = true;
        else
            currentTabInfo[0].isDefault = true;
    }
    else {
        AllUserUsers.prototype.tabInfo.tab[0].isDefault = true;

    }
    AllUserUsers.instance.bindTabs();
}

AllUserUsers.prototype.createInitDiv = function () {

    //Adding Spinner Div
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetTagTemplate', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        //Removing Spinner Div
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        //$("#" + RADUserManagement.contentBodyId).empty();
        $(".usermgmt").remove();
        $("#" + RADUserManagement.contentBodyId).append(responseText.d);

        window.ScrollVariable = 0;
        window.ScrollVariableGroup = 0;
        window.ScrollVariableRoles = 0;
        
        var umDynamicTilesHeight = 0;
        if (RADUserManagement.IsIagoDependent)
            var umDynamicTilesHeight = $("#contentBody").height() - ($(".umLeftHeader").height() + $(".RADUserMgmtscrollUpDown").height() + $(".searchBarUm").height());
        else
            var umDynamicTilesHeight = ($("#contentBody").height() - $("#RUserManementTabDiv").height()) + ($(".umLeftHeader").height() + $(".RADUserMgmtscrollUpDown").height() + $(".searchBarUm").height());

        $('.umDynamicTiles').height(umDynamicTilesHeight);

        var umRightBodyHeight = $(".umRightSec").height() - $(".umRightHeaderParentDivClass").height();
        $('.umRightBody').height(umRightBodyHeight);

        var umSec1Padding = $('.umSec1').css('paddingTop').slice(0, -2);
        var umSec1Height = $(".umRightBody").height() - umSec1Padding;
        $('.umSec1').height(umSec1Height);

        var umSec2Padding = $('.umSec2').css('paddingTop').slice(0, -2);
        var umSec2Height = $(".umRightBody").height() - umSec2Padding;
        $('.umSec2').height(umSec2Height);

        var userName;
        AllUserUsers.instance.IsUserCreationAllowed();

        //AllUserUsers.instance.GetAllRolesGroups();        
        AllUserUsers.instance.SelectedRoles = [];
        AllUserUsers.instance.GroupRoleInfo = [];
        AllUserUsers.instance.SelectedRolesArray = [];
        AllUserUsers.instance.SelectedGroups = [];
        AllUserUsers.instance.SelectedGroupsArray = [];
        AllUserUsers.instance.LdapCheckForPasswordReset()
        AllUserUsers.instance.AllGroupList = []
        AllUserUsers.instance.AllRolesList = []
        AllUserUsers.prototype.SelectGroupShowInEditRoles = []
        AllUserUsers.prototype.resultsView = {}
        AllUserUsers.prototype.EditresultsView = {}
        AllUserUsers.instance.GroupValueArray = []
        AllUserUsers.instance.GetUserRolesInfo = []
        AllUserUsers.instance.AllGroupCorrespondingRoles = {}
        AllUserUsers.instance.CopyOFAllGroupCorrespondingRoles = []
        AllUserUsers.instance.SelectedGroupsForCreateSearch = []
        AllUserUsers.instance.SelectedRolesForCreateSearch = []
        AllUserUsers.instance.SelectedCorrespondingRolesSelected = []
        AllUserUsers.instance.GroupValueForDictionary = []
        AllUserUsers.instance.selectedgroupincreatesearch = []
        AllUserUsers.instance.GetSelectedGroupForAccountInCreateSearch = []
        AllUserUsers.instance.GetSelectedGroupsInReadOnlyScreenForSearch = [];
        AllUserUsers.instance.GetSelectedRolesForAccountInCreateSearch = []
        AllUserUsers.instance.GetRolesOrangeForCreateInSearchReadOnly = [];
        AllUserUsers.instance.CurrentSelectedUserDetails = {}

        $('#RADAuditButtonDiv').append("<input id=\"RADUserManagementAuditButton\" class=\"RADUserManagementAuditButton\" type=\"button\" value=\"Audit\" />");
        $("#RADDeleteInactiveButtonDiv").append("<input id=\"RADUserManagementDeleteInActiveButton\" class=\"RADUserManagementDeleteInActiveButton\" type=\"button\" value=\"Delete InActive Users\" />");

        $(".paymentBlotterHeader").remove()
        $('.umSec1').append("<div class=\"scrollUpDownGroup\"></div>")
        $('.scrollUpDownGroup').append("<div id=\"RADUserManagementScrollDownArrowGroup\" class=\"fa fa-caret-down RADUserManagementScrollArrowGroupSmall\"></div>")
        $('.scrollUpDownGroup').append("<div id=\"RADUserManagementScrollUpArrowGroup\" class=\"fa fa-caret-up RADUserManagementScrollArrowGroupSmall\"></div>")
        $('.umSec2').append("<div class=\"scrollUpDownRoles\"></div>")
        $('.scrollUpDownRoles').append("<div id=\"RADUserManagementScrollDownArrowRoles\" class=\"fa fa-caret-down RADUserManagementScrollArrowGroupSmall\"></div>")
        $('.scrollUpDownRoles').append("<div id=\"RADUserManagementScrollUpArrowRoles\" class=\"fa fa-caret-up RADUserManagementScrollArrowGroupSmall\"></div>")
        $("<div class=\"HorizontalRule\"> </div>").insertAfter($('.UMRightHeaderEdit'))

        var umSec1BodyHeight = $(".umSec1").height() - (Number(umSec1Padding) + $(".umSec1Head").height() + $(".scrollUpDownGroup").height())
        $('.umSec1Body').height(umSec1BodyHeight);

        var umSec2BodyHeight = $(".umSec2").height() - (Number(umSec2Padding) + $(".umSec2Head").height() + $(".scrollUpDownRoles").height())
        $('.umSec2Body').height(umSec2BodyHeight);

        AllUserUsers.instance.BindEvents();
        
    });

}

AllUserUsers.prototype.tabClickHandler = function (selectedTabId, tabContentContainer) {
    switch (selectedTabId) {
        case "Groups":
            var groupsObject = new AllUserGroups();
            groupsObject.init();
            break;
        case "Users":
            var usersObject = new AllUserUsers();
            usersObject.createInitDiv();
            break;
        case "Accounts":
            var usersObject = new AllUserAccounts();
            usersObject.init(usersObject);
            break;
        case "Roles":
            var RolesObject = new AllUserRoles();
            RolesObject.init();
            break;
    }
}

AllUserUsers.prototype.bindTabs = function () {

    if (RADUserManagement.IsIagoDependent) {
        $("#pageHeaderTabPanel").tabs({
            tabSchema: AllUserUsers.instance.tabInfo,
            tabClickHandler: AllUserUsers.instance.tabClickHandler,
            tabContentHolder: "contentBody"
        });

    }
    else {
        
        $("#RUserManementTabDiv").empty();
        $("#RUserManementTabDiv").toolKittabs({
            tabSchema: { tab: AllUserUsers.prototype.tabInfo.tab },
            tabClickHandler: AllUserUsers.instance.tabClickHandler,
            tabContentHolder: RADUserManagement.contentBodyId
        });

    }
    
}
AllUserUsers.prototype.GetAuthorizationPrivileges = function (creationAllowed) {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAuthorizationPrivileges', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'

    }).then(function (responseText) {
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        if (responseText.d == "admin") {
            if (creationAllowed)
                $("#RADUserCreatePlusCircle").show()
            $("#RADUserManagementEditButton").show()
            $("#RADUserManagementDeleteButton").show()

            Priviliges = ['Update User', 'Delete User', 'Add User Signature']
            if (creationAllowed)
                Priviliges.push('Add User');
        }
        else {
            var ResponseForCreation = [];
            if (responseText.d.length > 0)
                ResponseForCreation = JSON.parse(responseText.d);
            Priviliges = []
            for (var i = 0; i < ResponseForCreation.length; i++) {
                if (ResponseForCreation[i].pageId == "RAD_User") {
                    for (var j = 0; j < ResponseForCreation[i].Privileges.length; j++) {
                        if (ResponseForCreation[i].Privileges[j] == "Add User" && creationAllowed)
                            Priviliges.push(ResponseForCreation[i].Privileges[j]);
                        else if (ResponseForCreation[i].Privileges[j] != "Add User") {
                            Priviliges.push(ResponseForCreation[i].Privileges[j]);
                        }
                    }
                }
            }
            if (Priviliges.indexOf("Add User") != -1) {
                //here we can make explicitly true, as this method will call only, when the /IsUserCreationAllowed service returns response as true
                AllUserUsers.instance.isUserCreationAllowed = true;
                $('.RADUserCreatePlusCircle').show();
            }
            else {
                $('.RADUserCreatePlusCircle').hide();
            }
            if (Priviliges.indexOf("Delete User") != -1) {
                $("#RADUserManagementDeleteButton").show()
            }
            else {
                $("#RADUserManagementDeleteButton").hide()
            }
            if (Priviliges.indexOf("Update User") != -1) {
                $("#RADUserManagementEditButton").show()
            }
            else {
                $("#RADUserManagementEditButton").hide()
            }
        }
        AllUserUsers.instance.GetAllRolesGroups();
        
    })
}

AllUserUsers.prototype.LdapCheckForPasswordReset = function () {

   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/IsLDAPEnabled',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'

    }).then(function (responseText) {
           //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var ResponseForCreation = responseText.d;        
        AllUserUsers.instance.IsLDAPEnabled = ResponseForCreation;
        if (ResponseForCreation != true) {
            $("#RADResetPasswordButtonDiv").append("<input id=\"RADUserManagementResetButtonPassword\" class=\"RADUserManagementResetButtonPassword\" type=\"button\" value=\"ResetPassword\" />");
        }
     
    })
}

AllUserUsers.prototype.IsUserCreationAllowed = function () {

   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/IsUserCreationAllowed', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'

    }).then(function (responseText) {
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var ResponseForCreation = responseText.d;
        AllUserUsers.instance.isUserCreationAllowed = ResponseForCreation;
        if (!ResponseForCreation)
            $('.RADUserCreatePlusCircle').hide();
        AllUserUsers.instance.GetAuthorizationPrivileges(ResponseForCreation);
        
    })
}
AllUserUsers.prototype.GetAllRolesGroups = function () {

   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllGroups', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        ArrayOfRoles = []
        var res = [];
        if (responseText.d.length > 0)
            res = JSON.parse(responseText.d);
        AllUserUsers.instance.AllGroupList = res;
        var Roles = []
        for (var i = 0; i < AllUserUsers.instance.AllGroupList.length; i++) {
            Roles = []
            for (var j = 0; j < AllUserUsers.instance.AllGroupList[i].Roles.length; j++) {
                Roles.push(AllUserUsers.instance.AllGroupList[i].Roles[j])
            }
            AllUserUsers.instance.AllGroupCorrespondingRoles[AllUserUsers.instance.AllGroupList[i].GroupName] = Roles
        }
        for (var item in AllUserUsers.instance.AllGroupCorrespondingRoles) {
            AllUserUsers.instance.CopyOFAllGroupCorrespondingRoles.push(AllUserUsers.instance.AllGroupCorrespondingRoles[item])
        }
        for (j = 0; j < AllUserUsers.instance.CopyOFAllGroupCorrespondingRoles.length; j++) {
            for (var i = 0; i < AllUserUsers.instance.CopyOFAllGroupCorrespondingRoles[j].length; i++) {
                ArrayOfRoles.push(AllUserUsers.instance.CopyOFAllGroupCorrespondingRoles[j][i])
            }
        }
        AllUserUsers.instance.GetAllUsers();
        
    });

}

AllUserUsers.prototype.GetAllUsers = function () {

   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllUsers', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
         //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var b;
        b = JSON.parse(responseText.d);
        AllUserUsers.instance.AllUsersList = b;
        var firstUser;
        var Editfirstname
        var Editlastname
        var EditEmailId
        for (var j = 0; j < b.length; j++) {
            if (j == 0) {
                firstUser = b[j].UserLoginName
                $('.umDynamicTiles').append('<div title="' + b[j].UserLoginName + '"id="t' + j + '" name="' + b[j].UserLoginName + '"></div>');
                $("#t" + j).addClass('RADUserMgmttiles').addClass('RADTileActive');
                $("#t" + j).closest(".RADUserMgmttiles").append('<div id="arr"></div>');
                $(".umDynamicTiles").find(".RADarrow-right").remove();
                $("#arr").addClass('fa fa-caret-right RADarrow-right').insertAfter("#t" + j);
                $("#t" + j).append('<div  title="' + b[j].UserLoginName + '" id="user' + j + '" name="' + b[j].UserLoginName + '" FirstName="' + b[j].FirstName + '" LastName="' + b[j].LastName + '" EmailId="' + b[j].EmailId + '">' + b[j].UserLoginName + '</div>');
                $("#user" + j).addClass('RADUserMgmtuser_name');
                var first = b[j].FirstName
                $("#t" + j).append('<div title="' + b[j].FirstName + '" id="first' + j + '" FirstName="' + b[j].FirstName + '">' + '(' + b[j].FirstName + " " + '</div>');
                $("#first" + j).addClass('RADUserMgmtfirst_name');
                $("#t" + j).append('<div title="' + b[j].LastName + '" id="last' + j + '" LastName="' + b[j].LastName + '">' + b[j].LastName + ')' + '</div>');
                $("#last" + j).addClass('RADUserMgmtlast_name');
                if (b[j].title != null && b[j].title != "") {
                    $("#t" + j).append('<div title="' + b[j].title + '"id="title' + j + '" Title="' + b[j].title + '">' + b[j].title + '</div>');
                    $("#title" + j).addClass('RADUserMgmtemail_id');
                }
                $("#t" + j).append('<div title="' + b[j].EmailId + '" id="email' + j + '" EmailId="' + b[j].EmailId + '">' + b[j].EmailId + '</div>');
                $("#email" + j).addClass('RADUserMgmtemail_id');
                $("#FirstName").val(b[j].UserLoginName);
                $("#LastName").val(b[j].FirstName);
                $("#Email").val(b[j].EmailId);
                if (Priviliges.indexOf("Add User Signature") != -1)
                    $("#t" + j).closest(".RADUserMgmttiles").append('<div id="tSign' + j + '" class="RADUserMgmtUserSign"></div>')
                AllUserUsers.instance.SetUser = firstUser;
            }
            else {
                $('.umDynamicTiles').append('<div id="t' + j + '" name="' + b[j].UserLoginName + '" FirstName="' + b[j].FirstName + '" LastName="' + b[j].LastName + '" EmailId="' + b[j].EmailId + '"></div>');
                $("#t" + j).addClass('RADUserMgmttiles');
                $("#t" + j).append('<div title="' + b[j].UserLoginName + '"id="user' + j + '">' + b[j].UserLoginName + '</div>');
                $("#user" + j).addClass('RADUserMgmtuser_name');
                $("#t" + j).append('<div  title="' + b[j].FirstName + '" id="first' + j + '">' + '(' + b[j].FirstName + " " + '</div>');
                $("#first" + j).addClass('RADUserMgmtfirst_name');
                $("#t" + j).append('<div title="' + b[j].LastName + '" id="last' + j + '">' + b[j].LastName + ')' + '</div>');
                $("#last" + j).addClass('RADUserMgmtlast_name');
                if (b[j].title != null && b[j].title != "") {
                    $("#t" + j).append('<div title="' + b[j].title + '"id="title' + j + '">' + b[j].title + '</div>');
                    $("#title" + j).addClass('RADUserMgmtemail_id');
                }
                $("#t" + j).append('<div  title="' + b[j].EmailId + '"id="email' + j + '">' + b[j].EmailId + '</div>');
                $("#email" + j).addClass('RADUserMgmtemail_id');
                if (Priviliges.indexOf("Add User Signature") != -1)
                    $("#t" + j).closest(".RADUserMgmttiles").append('<div id="tSign' + j + '" class = "RADUserMgmtUserSign"></div>')
            }
        }

        $("#FirstName").val($(this).attr('FirstName'));
        $("#LastName").val($(this).attr('LastName'));
        $("#Email").val($(this).attr('EmailId'));
        AllUserUsers.instance.GetUserInfo(firstUser);
        $(".RADUserMgmttiles").unbind().click(function (event) {
            AllUserUsers.instance.ClickOnCreateTiles(event);
        });

       
    });
}


AllUserUsers.prototype.GroupSearch = function (event) {
    $(".umSec1Body").empty();
    if (GlobalFlagSearch == true) {
        var GetSelectedGroupsInReadOnlyScreenForSearch = [];
        if (AllUserUsers.instance.GetSelectedGroupsInReadOnlyScreenForSearch.length != 0) {
            GetSelectedGroupsInReadOnlyScreenForSearch = AllUserUsers.instance.GetSelectedGroupsInReadOnlyScreenForSearch;
        }
        for (j = 0; j < AllUserUsers.instance.GetSelectedGroupForAccountInCreateSearch.length; j++) {
            if ((AllUserUsers.instance.GetSelectedGroupForAccountInCreateSearch[j].toLowerCase()).indexOf($(event.target).val().toLowerCase()) != -1) {
                if (AllUserUsers.instance.GetSelectedGroupsInReadOnlyScreenForSearch.indexOf(AllUserUsers.instance.GetSelectedGroupForAccountInCreateSearch[j]) != -1) {
                    var len = $('.umSec1Body').children().length;
                    $('.umSec1Body').append('<div data-toggle = "tooltip" data=\"' + AllUserUsers.instance.GetSelectedGroupForAccountInCreateSearch[j] + '\" id="Gr' + (len + j) + '" Group_name="' + AllUserUsers.instance.GetSelectedGroupForAccountInCreateSearch[j] + '">' + AllUserUsers.instance.GetSelectedGroupForAccountInCreateSearch[j] + '</div>')
                    $("#Gr" + (len + j)).addClass('TextSelected');
                    $("#Gr" + (len + j)).addClass('RADEditBodyContent');
                }
                else {
                    var len = $('.umSec1Body').children().length;
                    $('.umSec1Body').append('<div  data-toggle = "tooltip" data=\"' + AllUserUsers.instance.GetSelectedGroupForAccountInCreateSearch[j] + '\"  id="Gr' + (len + j) + '" Group_name="' + AllUserUsers.instance.GetSelectedGroupForAccountInCreateSearch[j] + '">' + AllUserUsers.instance.GetSelectedGroupForAccountInCreateSearch[j] + '</div>')
                    $("#Gr" + (len + j)).addClass('RADEditBodyContent');
                }
            }
        }
        $(".RADEditBodyContent").unbind().click(function (event) {
            var groupname = $(event.target).text()
            if ($(this).hasClass("TextSelected") == true) {
                $(this).switchClass("TextSelected", "RADEditBodyContent")
                var a = []
                var count = 0
                a.push(AllUserUsers.instance.AllGroupCorrespondingRoles[groupname])
                if (a[0] != null) {
                    for (var i = 0; i < a[0].length; i++) {
                        if (ArrayOfRoles.indexOf(a[0][i]) != -1) {
                            ArrayOfRoles.splice(i, 1)
                        }
                    }

                    for (var k = 0; k < a[0].length; k++) {
                        if (ArrayOfRoles.indexOf(a[0][k]) != -1) {
                            if ($(".umSec2Body").find('div[data="' + a[0][k] + '"]')) {
                                var roleText = $(".umSec2Body").find('div[data="' + a[0][k] + '"]').html();
                                $(".umSec2Body").find('div[data="' + a[0][k] + '"]').removeClass("RADGrouoInfobodycontent")
                                $(".umSec2Body").find('div[data="' + a[0][k] + '"]').addClass("RADEditBodyContentRoles")
                            }
                        }
                    }
                }
                AllUserUsers.instance.selectedgroupincreatesearch.splice(AllUserUsers.instance.SelectedCorrespondingRolesSelected.indexOf(roleText), 1);
                AllUserUsers.instance.SelectedGroupsForCreateSearch.splice(AllUserUsers.instance.SelectedGroupsForCreateSearch.indexOf($(this).text()), 1);
                AllUserUsers.instance.GetSelectedGroupsInReadOnlyScreenForSearch.splice(AllUserUsers.instance.SelectedGroupsForCreateSearch.indexOf($(this).text()), 1);
            }
            else {
                $(this).addClass('TextSelected');
                var groupname = $(event.target).text()
                var a = []
                a.push(AllUserUsers.instance.AllGroupCorrespondingRoles[groupname])
                if (a[0] != null) {
                    for (var k = 0; k < a[0].length; k++) {
                        if ($(".umSec2Body").find('div[data="' + a[0][k] + '"]')) {
                            $(".umSec2Body").find('div[data="' + a[0][k] + '"]').removeClass("RADEditBodyContentRoles")
                            $(".umSec2Body").find('div[data="' + a[0][k] + '"]').addClass("RADGrouoInfobodycontent")

                        }
                    }
                }
                AllUserUsers.instance.selectedgroupincreatesearch.push(roleText);
                AllUserUsers.instance.SelectedGroupsForCreateSearch.push($(this).text())
                AllUserUsers.instance.GetSelectedGroupsInReadOnlyScreenForSearch.push($(this).text())
            }
        });
        $(".RADUserMgmtbodycontent").unbind().click(function (event) {
            if ($(this).hasClass("TextSelected") == true) {
                $(this).switchClass("TextSelected", "RADEditBodyContent")
                AllUserUsers.instance.SelectedGroupsForCreateSearch.splice(AllUserUsers.instance.SelectedGroupsForCreateSearch.indexOf($(this).text()), 1);
                AllUserUsers.instance.GetSelectedGroupsInReadOnlyScreenForSearch.splice(AllUserUsers.instance.SelectedGroupsForCreateSearch.indexOf($(this).text()), 1);
            }
            else {
                $(this).addClass('TextSelected');
                AllUserUsers.instance.SelectedGroupsForCreateSearch.push($(this).text())
                AllUserUsers.instance.GetSelectedGroupsInReadOnlyScreenForSearch.push($(this).text())
            }
        });

    }
    else {

        if ($(event.target).val() == "") {
            for (j = 0; j < AllUserUsers.instance.SelectedGroupsArray.length; j++) {
                $('.umSec1Body').append('<div data-toggle = "tooltip" id="Gr' + j + '" Group_name="' + AllUserUsers.instance.SelectedGroupsArray[j] + '">' + AllUserUsers.instance.SelectedGroupsArray[j] + '</div>')
                $("#Gr" + j).addClass('TextSelected');
                $("#Gr" + j).addClass('RADEditBodyContent');
            }
        }
        else {
            for (j = 0; j < AllUserUsers.instance.SelectedGroupsArray.length; j++) {
                if (AllUserUsers.instance.SelectedGroupsArray[j].indexOf($(event.target).val()) != -1) {
                    $('.umSec1Body').append('<div data-toggle = "tooltip" id="Gr' + j + '" Group_name="' + AllUserUsers.instance.SelectedGroupsArray[j] + '">' + AllUserUsers.instance.SelectedGroupsArray[j] + '</div>')
                    $("#Gr" + j).addClass('TextSelected');
                    $("#Gr" + j).addClass('RADEditBodyContent');
                }
            }
        }

        for (j = 0; j < AllUserUsers.instance.AllGroupList.length; j++) {
            if ((AllUserUsers.instance.AllGroupList[j]["GroupName"].toLowerCase()).indexOf($(event.target).val().toLowerCase()) != -1) {
                if (AllUserUsers.instance.SelectedGroupsArray.indexOf(AllUserUsers.instance.AllGroupList[j].GroupName) == -1) {
                    var len = $('.umSec1Body').children().length;
                    $('.umSec1Body').append('<div data-toggle = "tooltip" data=\"' + AllUserUsers.instance.AllGroupList[j].GroupName + '\" id="Gr' + (len + j) + '" Group_name="' + AllUserUsers.instance.AllGroupList[j].GroupName + '">' + AllUserUsers.instance.AllGroupList[j].GroupName + '</div>')
                    $("#Gr" + (len + j)).addClass('RADEditBodyContent');
                }
            }

        }
        $(".RADEditBodyContent").unbind().click(function (event) {
            if ($(this).hasClass("TextSelected") == true) {
                $(this).switchClass("TextSelected", "RADEditBodyContent")
                var a = []
                var count = 0
                a.push(AllUserUsers.instance.AllGroupCorrespondingRoles[groupname])
                for (var i = 0; i < a[0].length; i++) {
                    if (ArrayOfRoles.indexOf(a[0][i]) != -1) {
                        ArrayOfRoles.splice(i, 1)
                    }
                }
                for (var k = 0; k < a[0].length; k++) {
                    if (ArrayOfRoles.indexOf(a[0][k]) != -1) {
                        if ($(".umSec2Body").find('div[data="' + a[0][k] + '"]')) {
                            var roleText = $(".umSec2Body").find('div[data="' + a[0][k] + '"]').html();
                            $(".umSec2Body").find('div[data="' + a[0][k] + '"]').removeClass("RADGrouoInfobodycontent")
                            $(".umSec2Body").find('div[data="' + a[0][k] + '"]').addClass("RADEditBodyContentRoles")
                        }
                    }
                }
                AllUserUsers.instance.SelectedCorrespondingRolesSelected.splice(AllUserUsers.instance.SelectedCorrespondingRolesSelected.indexOf(roleText), 1);
                AllUserUsers.instance.SelectedGroups.splice(AllUserUsers.instance.SelectedGroups.indexOf($(this).text()), 1);
            }
            else {
                $(this).addClass('TextSelected');
                var groupname = $(event.target).text()
                var a = []
                a.push(AllUserUsers.instance.AllGroupCorrespondingRoles[groupname])
                for (var k = 0; k < a[0].length; k++) {
                    if ($(".umSec2Body").find('div[data="' + a[0][k] + '"]')) {
                        $(".umSec2Body").find('div[data="' + a[0][k] + '"]').removeClass("RADEditBodyContentRoles")
                        $(".umSec2Body").find('div[data="' + a[0][k] + '"]').addClass("RADGrouoInfobodycontent")

                    }
                }
                AllUserUsers.instance.SelectedCorrespondingRolesSelected.push(roleText);
                AllUserUsers.instance.SelectedGroups.push($(this).text())
            }
        });
        $(".bodycontent").unbind().click(function (event) {
            if ($(this).hasClass("TextSelected") == true) {
                $(this).switchClass("TextSelected", "RADEditBodyContent")
                AllUserUsers.instance.SelectedGroups.splice(AllUserUsers.instance.SelectedGroups.indexOf($(this).text()), 1);
            }
            else {
                $(this).addClass('TextSelected');
                AllUserUsers.instance.SelectedGroups.push($(this).text())
            }
        });
    }

}

AllUserUsers.prototype.RoleSearch = function (event) {
    $(".umSec2Body").empty();
    if (GlobalFlagSearch == true) {
        for (j = 0; j < AllUserUsers.instance.AllRoleList.length; j++) {
            if ((AllUserUsers.instance.AllRoleList[j]["RoleName"].toLowerCase()).indexOf($(event.target).val().toLowerCase()) != -1) {
                if ((AllUserUsers.instance.GetRolesOrangeForCreateInSearchReadOnly).indexOf((AllUserUsers.instance.AllRoleList[j].RoleName)) != -1) {
                    var len = $('.umSec2Body').children().length;
                    $('.umSec2Body').append('<div data-toggle = "tooltip" data=\"' + AllUserUsers.instance.AllRoleList[j].RoleName + '\"id="Ro' + (len + j) + '" Role_name="' + AllUserUsers.instance.AllRoleList[j].RoleName + '">' + AllUserUsers.instance.AllRoleList[j].RoleName + '</div>')
                    $("#Ro" + (len + j)).addClass('RADGrouoInfobodycontent');
                }
                else if ((((AllUserUsers.instance.GetSelectedRolesForAccountInCreateSearch)).indexOf((AllUserUsers.instance.AllRoleList[j].RoleName)) != -1) && ((AllUserUsers.instance.GetRolesOrangeForCreateInSearchReadOnly).indexOf(AllUserUsers.instance.AllRoleList[j].RoleName) == -1)) {
                    var len = $('.umSec2Body').children().length;
                    $('.umSec2Body').append('<div data-toggle = "tooltip" data=\"' + AllUserUsers.instance.AllRoleList[j].RoleName + '\"id="Ro' + (len + j) + '" Role_name="' + AllUserUsers.instance.AllRoleList[j].RoleName + '">' + AllUserUsers.instance.AllRoleList[j].RoleName + '</div>')
                    $("#Ro" + (len + j)).addClass('TextSelected');
                }

                else {
                    var len = $('.umSec2Body').children().length;
                    $('.umSec2Body').append('<div data-toggle = "tooltip" data=\"' + AllUserUsers.instance.AllRoleList[j].RoleName + '\" id="Ro' + (len + j) + '" Role_name="' + AllUserUsers.instance.AllRoleList[j].RoleName + '">' + AllUserUsers.instance.AllRoleList[j].RoleName + '</div>')
                    $("#Ro" + (len + j)).addClass('RADEditBodyContentRolesForCreate');
                }

            }

        }
        $(".RADEditBodyContentRolesForCreate").unbind().click(function (event) {
            if ($(this).hasClass("TextSelected") == true) {
                $(this).switchClass("TextSelected", "RADEditBodyContentRolesForCreate")
                AllUserUsers.instance.SelectedRolesForCreateSearch.splice(AllUserUsers.instance.SelectedRolesForCreateSearch.indexOf($(this).text()), 1);
                if (AllUserUsers.instance.GetSelectedRolesForAccountInCreateSearch.indexOf($(this).text()) != -1)
                    AllUserUsers.instance.GetSelectedRolesForAccountInCreateSearch.splice(AllUserUsers.instance.SelectedRolesForCreateSearch.indexOf($(this).text()), 1);
            }
            else {
                $(this).addClass('TextSelected');
                AllUserUsers.instance.SelectedRolesForCreateSearch.push($(this).text())
                if (AllUserUsers.instance.GetSelectedRolesForAccountInCreateSearch.indexOf($(this).text()) == -1)
                    AllUserUsers.instance.GetSelectedRolesForAccountInCreateSearch.push($(this).text())
            }
        });
        $(".bodycontent").unbind().click(function (event) {
            if ($(this).hasClass("TextSelected") == true) {
                $(this).switchClass("TextSelected", "RADEditBodyContentRolesForCreate")
                AllUserUsers.instance.SelectedRolesForCreateSearch.splice(AllUserUsers.instance.SelectedRolesForCreateSearch.indexOf($(this).text()), 1);
                if (AllUserUsers.instance.GetSelectedRolesForAccountInCreateSearch.indexOf($(this).text()) != -1)
                    AllUserUsers.instance.GetSelectedRolesForAccountInCreateSearch.splice(AllUserUsers.instance.SelectedRolesForCreateSearch.indexOf($(this).text()), 1);
            }
            else {
                $(this).addClass('TextSelected');
                AllUserUsers.instance.SelectedRolesForCreateSearch.push($(this).text())
                if (AllUserUsers.instance.GetSelectedRolesForAccountInCreateSearch.indexOf($(this).text()) == -1)
                    AllUserUsers.instance.GetSelectedRolesForAccountInCreateSearch.push($(this).text())

            }
        });
    }
    else {
        if ($(event.target).val() == "") {
            for (j = 0; j < AllUserUsers.instance.SelectedCorrespondingRolesSelected.length; j++) {
                $('.umSec2Body').append('<div data-toggle = "tooltip" data=\"' + AllUserUsers.instance.SelectedCorrespondingRolesSelected[j] + '\" id="ColorRo' + j + '" Role_name="' + AllUserUsers.instance.SelectedCorrespondingRolesSelected[j] + '">' + AllUserUsers.instance.SelectedCorrespondingRolesSelected[j] + '</div>')
                $("#ColorRo" + j).addClass('RADGrouoInfobodycontent');
            }

            for (j = 0; j < AllUserUsers.instance.SelectedRolesArray.length; j++) {
                var a = [];
                for (var i = 0; i < AllUserUsers.instance.GroupValueForDictionary.length; i++) {
                    for (var item in AllUserUsers.instance.AllGroupCorrespondingRoles[AllUserUsers.instance.GroupValueForDictionary[i]]) {
                        if (AllUserUsers.instance.AllGroupCorrespondingRoles[AllUserUsers.instance.GroupValueForDictionary[i]][item].indexOf(AllUserUsers.instance.SelectedRolesArray[j]) != -1) {
                            a.push(AllUserUsers.instance.GroupValueForDictionary[i])
                        }
                    }
                }
                $('.umSec2Body').append('<div data-toggle = "tooltip" data=\"' + AllUserUsers.instance.SelectedRolesArray[j].RoleName + '\" id="Ro' + j + '" Role_name="' + AllUserUsers.instance.SelectedRolesArray[j] + '">' + AllUserUsers.instance.SelectedRolesArray[j] + '</div>')
                $("#Ro" + j).addClass('TextSelected');
            }
        }
        else {
            for (j = 0; j < AllUserUsers.instance.SelectedRolesArray.length; j++) {
                if ((AllUserUsers.instance.SelectedRolesArray[j].toLowerCase()).indexOf($(event.target).val().toLowerCase()) != -1) {
                    $('.umSec2Body').append('<div data-toggle="tooltip" data=\"' + AllUserUsers.instance.AllRoleList[j].RoleName + '\" id="Ro' + j + '" Role_name="' + AllUserUsers.instance.SelectedRolesArray[j] + '">' + AllUserUsers.instance.SelectedRolesArray[j] + '</div>')
                    $("#Ro" + j).addClass('TextSelected');
                }

            }
            for (j = 0; j < AllUserUsers.instance.SelectedCorrespondingRolesSelected.length; j++) {
                var a = [];
                if ((AllUserUsers.instance.SelectedCorrespondingRolesSelected[j].toLowerCase()).indexOf($(event.target).val().toLowerCase()) != -1) {

                    for (var i = 0; i < AllUserUsers.instance.GroupValueForDictionary.length; i++) {
                        for (var item in AllUserUsers.instance.AllGroupCorrespondingRoles[AllUserUsers.instance.GroupValueForDictionary[i]]) {
                            if (AllUserUsers.instance.AllGroupCorrespondingRoles[AllUserUsers.instance.GroupValueForDictionary[i]][item].indexOf(AllUserUsers.instance.SelectedCorrespondingRolesSelected[j]) != -1) {
                                a.push(AllUserUsers.instance.GroupValueForDictionary[i])
                            }
                        }
                    }
                    $('.umSec2Body').append('<div data-toggle = "tooltip" data=\"' + AllUserUsers.instance.SelectedCorrespondingRolesSelected[j] + '\" id="ColorRo' + j + '" Role_name="' + AllUserUsers.instance.SelectedCorrespondingRolesSelected[j] + '">' + AllUserUsers.instance.SelectedCorrespondingRolesSelected[j] + '</div>')
                    $("#ColorRo" + j).addClass('RADGrouoInfobodycontent');
                }
            }
        }


        for (j = 0; j < AllUserUsers.instance.AllRoleList.length; j++) {
            if ((AllUserUsers.instance.AllRoleList[j]["RoleName"].toLowerCase()).indexOf($(event.target).val().toLowerCase()) != -1) {
                if (AllUserUsers.instance.SelectedRolesArray.indexOf(AllUserUsers.instance.AllRoleList[j].RoleName) == -1) {
                    if (AllUserUsers.instance.SelectedCorrespondingRolesSelected.indexOf(AllUserUsers.instance.AllRoleList[j].RoleName) == -1)
                        if (Array.contains(AllUserUsers.instance.SelectedRoles, AllUserUsers.instance.AllRoleList[j].RoleName)) {

                            var len = $('.umSec2Body').children().length;
                            $('.umSec2Body').append('<div data-toggle = "tooltip" data=\"' + AllUserUsers.instance.AllRoleList[j].RoleName + '\" id="Ro' + (len + j) + '" Role_name="' + AllUserUsers.instance.AllRoleList[j].RoleName + '">' + AllUserUsers.instance.AllRoleList[j].RoleName + '</div>')
                            $("#Ro" + (len + j)).addClass('TextSelected');

                        }
                        else {
                            var len = $('.umSec2Body').children().length;
                            $('.umSec2Body').append('<div data-toggle = "tooltip" data=\"' + AllUserUsers.instance.AllRoleList[j].RoleName + '\" id="Ro' + (len + j) + '" Role_name="' + AllUserUsers.instance.AllRoleList[j].RoleName + '">' + AllUserUsers.instance.AllRoleList[j].RoleName + '</div>')
                            $("#Ro" + (len + j)).addClass('RADEditBodyContentRoles');
                        }
                }
            }

        }
        $(".RADEditBodyContentRoles").unbind().click(function (event) {
            if ($(this).hasClass("TextSelected") == true) {
                $(this).switchClass("TextSelected", "RADEditBodyContentRoles")
                AllUserUsers.instance.SelectedRoles.splice(AllUserUsers.instance.SelectedRoles.indexOf($(this).text()), 1);
            }
            else {
                $(this).addClass('TextSelected');
                AllUserUsers.instance.SelectedRoles.push($(this).text())
            }
        });
        $(".bodycontent").unbind().click(function (event) {
            if ($(this).hasClass("TextSelected") == true) {
                $(this).switchClass("TextSelected", "RADEditBodyContentRoles")
                AllUserUsers.instance.SelectedRoles.splice(AllUserUsers.instance.SelectedRoles.indexOf($(this).text()), 1);
            }
            else {
                $(this).addClass('TextSelected');
                AllUserUsers.instance.SelectedRoles.push($(this).text())

            }
        });
    }

}

AllUserUsers.prototype.ClickOnCreateTiles = function (event) {

    if (event.target.className == "RADUserMgmtUserSign") {
        var userLoginName = $(event.target).closest(".RADUserMgmttiles").children()[0].innerText;
        AllUserUsers.instance.GetUserSignatureInfo(userLoginName);
    }
    else {

        if ($("#RADAuditScreenMain").css("display") == "block") {
            $("#RADAuditScreenMain").hide('slow', function () { $("#RADAuditScreenMain").remove(); });
        }
        if ($(".RADUserManagementEditButton").val() == "Save" || $("#RADUserManagementCreateActiveDirectorySaveButton").val() == "Save") {
            if ($("#alertParentDivEdit") != null)
                $("#alertParentDivEdit").remove();
            $("#ContentDiv").append("<div id=\"alertParentDivEdit\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDivEdit").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
            $("#alertParentDivEdit").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDivEdit").append("<div class=\"RADUserErrorDivLeft RADUserAlert\"></div>"); //revisit
            $("#alertParentDivEdit").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Alert</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Your Changes will revert.Want To Continue?</div>");
            $("#alertParentDivEdit").append("<div class=\"RADUserManagementPopUpErrorFooter\"></div>");
            $(".RADUserManagementPopUpErrorFooter").append("<div class=\"RADUserManagementPopUpErrorYes\">Yes</div><div class=\"RADUserManagementPopUpErrorNo\">No</div>")
            $("#alertParentDivEdit").css("top", "-200px");
            $("#alertParentDivEdit").animate({ "top": "0px" });
            $(".RADUserManagementPopUpErrorYes").unbind().click(function (event) {
                isEditMode = false;
                $("#alertParentDivEdit").remove();
                $("#RADUserManagementSaveButton").val("Edit")
                $("#RADUserManagementSaveButton").attr('id', 'RADUserManagementEditButton');
                $("#RADUserManagementCreateSaveButton").val("Edit")
                $("#RADUserManagementCreateSaveButton").attr('id', 'RADUserManagementEditButton');
                $("#RADUserManagementCreateActiveDirectorySaveButton").val("Edit")
                $("#RADUserManagementCreateActiveDirectorySaveButton").attr('id', 'RADUserManagementEditButton')
                $("#RADUserManagementCreateSaveButton").val("Edit")
                $('#RADUserManagementCancelButton').val("Delete");
                $('#RADUserManagementCancelButton').attr('id', 'RADUserManagementDeleteButton')

                //checking for privileges
                AllUserUsers.instance.ShowHideByPrivilegeType("Edit");
                AllUserUsers.instance.ShowHideByPrivilegeType("Delete");

                $("#RADUMGroupSearch").remove()
                $("#RADUMRoleSearch").remove()
                $("#Groupsearch_text").remove()
                $("#Rolesearch_text").remove()
                AllUserUsers.instance.AfterPopUpYes()
            });
            $(".RADUserManagementPopUpErrorNo").unbind().click(function (event) {
                $("#alertParentDivEdit").remove();
            });
        }
        else {
            $(".umDynamicTiles").find(".RADTileActive").removeClass("RADTileActive");
            $(event.target).closest(".RADUserMgmttiles").addClass('RADTileActive')
            Editfirstname = $(".RADTileActive").children()[1].innerText.replace("(", "");;
            Editlastname = $(".RADTileActive").children()[2].innerText.replace(")", "");
            EditEmailId = $(".RADTileActive").children()[3].innerText;
            $(event.target).closest(".RADUserMgmttiles").append('<div id="arr"></div>');
            $(".umDynamicTiles").find(".RADarrow-right").remove();
            userName = $(".RADTileActive").children()[0].innerText;
            AllUserUsers.instance.GetUserInfo(userName);
            $("#arr").addClass('fa fa-caret-right RADarrow-right').insertAfter($(event.target).closest(".RADUserMgmttiles"));
            AllUserUsers.instance.SetUser = $(event.target).closest(".RADUserMgmttiles").children()[0].innerText;
            $('.umSec1Body').empty();
            $("#FirstName").val(Editfirstname).addClass('RADSelectedHtml');
            $("#LastName").val(Editlastname).addClass('RADSelectedHtml');
            $("#Email").val(EditEmailId).addClass('RADSelectedHtml');
        }
    }
}


AllUserUsers.prototype.AfterPopUpYes = function () {
    $(".umRightHeader").empty();
    AllUserUsers.instance.AfterPopUpDetails()
}


AllUserUsers.prototype.AfterPopUpDetails = function () {
    var AfterPopUpUSerName
    $(".RADUserManagementAuditButton").show()
    $("#RADUserManagementResetButtonPassword").show()
    $(".searchBarUm").show();

    //checking for privileges
    AllUserUsers.instance.ShowHideByPrivilegeType("Add");

    $(".umRightHeader").append("<div id=\"RADUserManagaementUserLabel\" class=\"RADUserManagaementLabel\"><label id=\"lblUserLabel\">User :</label></div>");

    if ($('#search_text').val().length > 0) {
        $('#search_text').val('');
        $(".umDynamicTiles").empty();
        AllUserUsers.instance.GetAllUsers();
    }
    else {
        AfterPopUpUSerName = AllUserUsers.instance.SetUser;
        AllUserUsers.instance.GetUserInfoAfterpopUp(AfterPopUpUSerName);
    }

    $("#RADUserManagementSaveButton").val("Edit")
    $("#RADUserManagementSaveButton").attr('id', 'RADUserManagementEditButton');
    $("#RADUserManagementCreateSaveButton").val("Edit")
    $("#RADUserManagementCreateSaveButton").attr('id', 'RADUserManagementEditButton');
    $("#RADUserManagementCreateActiveDirectorySaveButton").val("Edit")
    $("#RADUserManagementCreateActiveDirectorySaveButton").attr('id', 'RADUserManagementEditButton')
    $("#RADUserManagementCreateSaveButton").val("Edit")
    $('#RADUserManagementCancelButton').val("Delete");
    $('#RADUserManagementCancelButton').attr('id', 'RADUserManagementDeleteButton')
    //checking for privileges
    AllUserUsers.instance.ShowHideByPrivilegeType("Edit");
    AllUserUsers.instance.ShowHideByPrivilegeType("Delete");
    $(".RADUserManagementAccountGroupValidation").remove();
}


AllUserUsers.prototype.GetUserInfoAfterpopUp = function (userName) {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetUserDetail', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ userLoginName: userName })
    }).then(function (responseText) {
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var res;
        var UniqueRoles = []
        res = JSON.parse(responseText.d);
        AfterPopUpUSerName = userName;
        //Saving current user details for edit purpose
        AllUserUsers.instance.CurrentSelectedUserDetails = res;

        $(".umRightHeader").append(" <div id=\"lab\" class=\"RADUserMgmtuser_label\">" + AfterPopUpUSerName + "</div>");
        $("#lab").attr('title', AfterPopUpUSerName);
        $(".umRightHeader").append("<div id=\"RADUserManagaementAccountLabeDiv\" class=\"RADUserManagaementLabel\"><label id=\"lblAccountLabel\" >Account :</label></div>");
        $(".umRightHeader").append("<div id=\"lab1\" class=\"RADUSERManagementAccountValue\">" + res.Accounts + "</div>");
        $("#lab1").attr('title', res.Accounts);

        $('.umSec1Body').empty()
        $('.umSec2Body').empty()
        for (var i = 0; i < res.Groups.length; i++) {
            $('.umSec1Body').append('<div data-toggle="tooltip" id="d' + i + '">' + res.Groups[i] + '</div>');
            $("#d" + i).addClass('RADUserMgmtbodycontent')
        }
        for (var i = 0; i < res.Groups.length; i++) {
            var a = [];
            for (var item in AllUserUsers.instance.AllGroupCorrespondingRoles[res.Groups[i]]) {
                if (a.indexOf(AllUserUsers.instance.AllGroupCorrespondingRoles[res.Groups[i]][item]) == -1)
                    a.push(AllUserUsers.instance.AllGroupCorrespondingRoles[res.Groups[i]][item])
            }

            for (var j = 0; j < a.length; j++) {
                if (UniqueRoles.indexOf(a[j]) == -1) {
                    UniqueRoles.push(a[j])
                }
            }
        }
        for (k = 0; k < UniqueRoles.length; k++) {
            if (res.Roles.indexOf(UniqueRoles[k]) == -1) {
                $('.umSec2Body').append('<div  data-toggle = "tooltip" data=\"' + UniqueRoles[k] + '\" id="GetGroupInfo' + j + '" class=\"RADGrouoInfobodycontent">' + UniqueRoles[k] + '</div>');
            }
        }
        for (var i = 0; i < res.Roles.length; i++) {
            
                $('.umSec2Body').append('<div  data-toggle = "tooltip" id="Roles' + i + '">' + res.Roles[i] + '</div>');
                $("#Roles" + i).addClass('RADUserMgmtbodycontent');
            
        }

    });
}



AllUserUsers.prototype.DeleteUser = function (deleteUsername) {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/DeleteUser', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ userLoginName: deleteUsername })
    }).then(function (responseText) {
         //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var res;
        res = JSON.parse(responseText.d);
        if (res == true) {
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperSuccessDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserSuccess\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Success</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">User has been deleted successfully</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () {
                $("#alertParentDiv").remove();

            }, 2700);
            var usersObject = new AllUserUsers();
            usersObject.createInitDiv();
        }
        else {

            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserFailure\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Result</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">User Deletion Failed</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 2700);
            var usersObject = new AllUserUsers();
            usersObject.createInitDiv();
        }
       
    });
}

AllUserUsers.prototype.GetUserInfo = function (userName) {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetUserDetail', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ userLoginName: userName })
    }).then(function (responseText) {
        var res;
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        res = JSON.parse(responseText.d);
        //Saving current user details for edit purpose
        AllUserUsers.instance.CurrentSelectedUserDetails = res;
        var j = 0;
        var UniqueRoles = []        
        selectedAccount = res.Accounts[0];
        $(".umRightHeader").empty();

        $(".umRightHeader").append("<div id=\"RADUserManagaementUserLabel\" class=\"RADUserManagaementLabel\"><label id=\"lblUserLabel\" >User:</label></div>");
        $(".RADUserManagaementLabel").css({ "width": "25%" })
        var AfterPopUpUSerName = $(".RADTileActive").children()[0].innerText;
        $(".umRightHeader").append("<div id=\"lab\" class=\"RADUserMgmtuser_label\">" + AfterPopUpUSerName + "</div>");
        $("#lab").attr('title', AfterPopUpUSerName);

        $(".umRightHeader").append("<div id=\"RADUserManagaementAccountLabeDiv\" class=\"RADUserManagaementLabel\"><label id=\"lblAccountLabel\" >Account:</label></div>");
        $(".umRightHeader").append("<div id=\"lab1\" class=\"RADUSERManagementAccountValue\">" + selectedAccount + "</div>");

        $('#lab').attr('title', userName);
        $("#lab").html(userName).css("display", "block");
        $("#lab").addClass('RADUserMgmtuser_label');

        $("#lab1").html(res.Accounts).css("display", "block");
        $('#lab1').attr('title', res.Accounts);
        $("#lab1").addClass('RADUserMgmtuser_label');
        $('.umSec1Body').empty();
        $('.umSec2Body').empty();
        AllUserUsers.instance.GetUserRolesInfo = res.Roles
        $('.umSec2Body').append('<div class=\"UserAssociatedRoles\"><\div>')
        $('.umSec2Body').prepend('<div class=\"GroupAssociatedRoles\"><\div>')
        for (var i = 0; i < AllUserUsers.instance.GetUserRolesInfo.length; i++) {
            $('.UserAssociatedRoles').append('<div  data-toggle="tooltip" data=\"' + AllUserUsers.instance.GetUserRolesInfo[i] + '\" id="Roles' + i + '" class=\"RADUserMgmtbodycontent">' + AllUserUsers.instance.GetUserRolesInfo[i] + '</div>');
        }
        for (var i = 0; i < res.Groups.length; i++) {
            var a = [];
            for (var item in AllUserUsers.instance.AllGroupCorrespondingRoles[res.Groups[i]]) {
                if (a.indexOf(AllUserUsers.instance.AllGroupCorrespondingRoles[res.Groups[i]][item]) == -1)
                    a.push(AllUserUsers.instance.AllGroupCorrespondingRoles[res.Groups[i]][item])
            }

            for (var j = 0; j < a.length; j++) {
                if (UniqueRoles.indexOf(a[j]) == -1) {
                    UniqueRoles.push(a[j])
                }

            }
        }
        for (var i = 0; i < res.Groups.length; i++) {
            $('.umSec1Body').append('<div data-toggle = "tooltip" id="GetGroup' + i + '">' + res.Groups[i] + '</div>');
            $("#GetGroup" + i).addClass('RADUserMgmtbodycontent')
            if (res.Roles.length != 0) {
                for (var item = 0; item < AllUserUsers.instance.GetUserRolesInfo.length; item++) {
                    for (var f = 0; f < $('.UserAssociatedRoles').children().length; f++) {
                        if (($('.UserAssociatedRoles').children()[f].innerHTML) == AllUserUsers.instance.AllGroupCorrespondingRoles[item]) {
                            $('.UserAssociatedRoles').children()[f].remove()
                        }
                    }
                }
            }
        }

        for (k = 0; k < UniqueRoles.length; k++) {

            if (AllUserUsers.instance.GetUserRolesInfo.indexOf(UniqueRoles[k]) != -1) {
                return;
            }
            else {
                var a = [];
                for (var i = 0; i < res.Groups.length; i++) {

                    for (var item in AllUserUsers.instance.AllGroupCorrespondingRoles[res.Groups[i]]) {
                        if (AllUserUsers.instance.AllGroupCorrespondingRoles[res.Groups[i]][item].indexOf(UniqueRoles[k]) != -1) {
                            a.push(res.Groups[i])
                        }
                    }
                }
                $('.GroupAssociatedRoles').append('<div data-toggle = "tooltip" data=\"' + UniqueRoles[k] + '\" id="GetGroupInfo' + j + '" class=\"RADGrouoInfobodycontent">' + UniqueRoles[k] + '</div>');
                $("#GetGroupInfo" + j).attr('data-original-title', 'Group Associated To This Role Is  ' + a);
                j++;
                for (var item = 0; item < AllUserUsers.instance.GetUserRolesInfo.length; item++) {
                    for (var f = 0; f < $('.UserAssociatedRoles').children().length; f++) {
                        if (($('.UserAssociatedRoles').children()[f].innerHTML) == UniqueRoles[k]) {
                            $('.UserAssociatedRoles').children()[f].remove()
                        }
                    }
                }

            }
           
        }

        $("#ContainerDiv").attr("pointer-events", "");
     
    });
}


AllUserUsers.prototype.RemoveHoverClass = function (event) {
    var groupName = $(event.target).html()
    var element = []
    if (AllUserUsers.prototype.CheckKeyInResultsView(groupName)) {
        for (var i = 0; i < AllUserUsers.prototype.resultsView[groupName].length; i++) {
            element.push($(".umSec2Body").find('div[data="' + AllUserUsers.prototype.resultsView[groupName][i] + '"][title="' + "Group Associated To This Role Is  " + groupName + '"]'));
            element[i].removeClass("hoverBackgroundCOlor");
        }
    }
}
AllUserUsers.prototype.RemoveHoverClassOnmouseOut = function (event) {    
    $('[data-toggle="tooltip"]').tooltip('destroy');    
}

AllUserUsers.prototype.CheckKeyInResultsView = function (groupName) {
    for (var o in AllUserUsers.prototype.resultsView) {
        if (AllUserUsers.prototype.resultsView.hasOwnProperty(o)) {
            if (o == groupName) {
                return true;
            }
        }
    }
    return false;
}

AllUserUsers.prototype.GetHoverRolesInfo = function (event) {
    var RoleName = $(event.target).html()    
    var keys = [];
    var element;
    $.each(AllUserUsers.instance.AllGroupCorrespondingRoles, function (key, element) {
        keys.push(key)
    });
    var a = []
    var TooltipFlag = 0
    var Tooltipdata = []
    var selectedBlue = []
    var tooltipSelectedblue = []
    for (var i = 0; i < keys.length; i++) {
        for (var item in AllUserUsers.instance.AllGroupCorrespondingRoles[keys[i]]) {
            if (AllUserUsers.instance.AllGroupCorrespondingRoles[keys[i]][item].indexOf(RoleName) != -1) {
                a.push(keys[i])
            }
        }
    }
    if ($(".RADUserManagementEditButton").val() == "Edit") {
        for (var j = 0; j < $(".umSec1Body >div").length; j++) {
            for (var k = 0; k < a.length; k++) {

                if ($(".umSec1Body").children()[j].innerHTML == a[k]) {
                    TooltipFlag = 1
                    Tooltipdata.push(a[k])
                }
            }
            if (Tooltipdata != "") {
                $(event.target).attr({ 'data-original-title': "Groups Associated To " + RoleName + " Role Is  " + Tooltipdata })
            }
        }
    }

    if ($(".RADUserManagementEditButton").val() == "Save") {
        if ($(event.target).hasClass("TextSelected") == false) {
            for (var p = 0; p < $(".umSec1Body").find('.TextSelected').length; p++) {
                selectedBlue.push($(".umSec1Body").find('.TextSelected')[p].innerHTML)
            }
            for (var k = 0; k < selectedBlue.length; k++) {
                for (var i = 0; i < a.length; i++) {
                    if (selectedBlue[k] == a[i]) {
                        tooltipSelectedblue.push(a[i])
                    }

                }
                if (tooltipSelectedblue != "") {
                    $(event.target).attr({ 'data-original-title': "Groups Associated To " + RoleName + " Role Is  " + tooltipSelectedblue })
                }
            }
        }
    }

    $('[data-toggle="tooltip"]').tooltip();
}


AllUserUsers.prototype.GetHoverGroupInfo = function (event) {
    var groupName = $(event.target).html()
    var keys = [];
    var element = 0;
    $.each(AllUserUsers.instance.AllGroupCorrespondingRoles, function (key, element) {
        keys.push(key)
    });
    for (var i = 0; i < keys.length; i++) {
        if (groupName == keys[i]) {
            element = 1
        }
    }

    if (element == 1) {
        $(event.target).attr({ 'data-original-title': "Roles Associated to " + groupName + " Group are " + AllUserUsers.instance.AllGroupCorrespondingRoles[groupName] })
        element = 0
    }
    else {
        $(event.target).attr({ 'data-original-title': "There Are No Roles Associated To " + groupName + " Group " })
        element = 0
    }

    $('[data-toggle="tooltip"]').tooltip();
}


AllUserUsers.prototype.ModifyUserDetails = function () {
    var ModifyUserDetail = new Object();
    var Groups = [];
    var Roles = [];
    var ArrGroups = [];
    var ArrRoles = [];
    var Accounts = [];

    //fetching data from drop down
    var dialog = $("#lab1").data("ddn-SelectDropDown");
    Accounts.push(dialog._GetSingleSelectedDataFromWidget("#lab1"));


    if (AllUserUsers.instance.IsLDAPEnabled == false) {
        ModifyUserDetail.FirstName = $("#FirstName").val().replace("(", "")
        ModifyUserDetail.LastName = $("#LastName").val().replace(")", "")
        ModifyUserDetail.EmailId = $("#Email").val()
    }
    ModifyUserDetail.UserLoginName = $(".RADTileActive").children()[0].innerText
    ModifyUserDetail.FirstName = AllUserUsers.instance.CurrentSelectedUserDetails.FirstName;
    ModifyUserDetail.LastName = AllUserUsers.instance.CurrentSelectedUserDetails.LastName;
    ModifyUserDetail.EmailId = AllUserUsers.instance.CurrentSelectedUserDetails.EmailId;
  //  Accounts.push($(".RADUserManagementFirstAccountDiv").text())
    ArrGroups = ($(".umSec1Body").find(".TextSelected"))
    ArrRoles = ($(".umSec2Body").find(".TextSelected"));
    $(ArrGroups).each(function (i, e) {
        Groups.push($(e).text());
    })
    $(ArrRoles).each(function (i, e) {
        Roles.push($(e).text());
    })
    ModifyUserDetail.Roles = Roles;
    ModifyUserDetail.Groups = Groups;
    ModifyUserDetail.Accounts = Accounts;
    var myString = JSON.stringify(ModifyUserDetail);
    AllUserUsers.instance.ModifyUser(myString, userName);

}

AllUserUsers.prototype.ModifyUser = function (ModifyUserDetail, userName) {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/ModifyUser',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ userDetails: ModifyUserDetail })
    }).then(function (responseText) {
         //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        if (responseText.d == true) {
            $(".RADUserManagementCancelButton").val("Delete")
            $(".RADUserManagementCancelButton").attr('id', 'RADUserManagementDeleteButton')
            //checking for privileges
            AllUserUsers.instance.ShowHideByPrivilegeType("Delete");

            $(".umRightHeader").empty();
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperSuccessDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserSuccess\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Result</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">User Has Been Updated Successfully</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 2700);
            AllUserUsers.instance.ReCreateDiv();
        }
        else {
            $(".RADUserManagementCancelButton").val("Delete")
            $(".RADUserManagementCancelButton").attr('id', 'RADUserManagementDeleteButton')
            //checking for privileges
            AllUserUsers.instance.ShowHideByPrivilegeType("Delete");
            $(".umRightHeader").empty();
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserFailure\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Result</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">User Modification Failed</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 5000);
            AllUserUsers.instance.ReCreateDiv();
        }
       
    });
}


AllUserUsers.prototype.GetGroupsForAccount = function (Accounts) {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    selectedAccount = Accounts[0];
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetGroupsForAccount', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ accountName: selectedAccount })
    }).then(function (responseText) {
        var res;
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        //res = JSON.parse(responseText.d);
        if (AllUserUsers.prototype.JSONTest(responseText.d)) {
            res = JSON.parse(responseText.d);
        }
        else {
            res = [];
        }
        AllUserUsers.instance.GetSelectedGroupForAccountInCreateSearch = []
        AllUserUsers.instance.GetSelectedGroupForAccountInCreateSearch = res;
        $(".umSec1Body").empty()
        for (var i = 0; i <= res.length; i++)
            $('.RADEditBodyContent').remove();
        $('.RADUserMgmtbodycontent').remove();

        if (isEditMode) {
            for (var i = 0; i < AllUserUsers.instance.SelectedGroupsArray.length; i++) {
                $('.umSec1Body').append('<div data-toggle = "tooltip"  id="GroupsForAccount' + i + '" data=\"' + AllUserUsers.instance.SelectedGroupsArray[i] + '\" class=\"RADEditBodyContentRoles\">' + AllUserUsers.instance.SelectedGroupsArray[i] + '</div>');
                $("GroupsForAccount" + i).attr('data-original-title', 'Group Associated To This Role Is  ' + AllUserUsers.instance.AllGroupCorrespondingRoles[AllUserUsers.instance.SelectedGroupsArray[i]]);
                $($(".umSec1Body").children()[i]).addClass('TextSelected');
            }
            for (var i = 0; i < res.length; i++) {
                if (AllUserUsers.instance.SelectedGroupsArray.indexOf(res[i]) == -1) {
                    $('.umSec1Body').append('<div data-toggle = "tooltip"  id="GroupsForAccount' + i + '" data=\"' + res[i] + '\" class=\"RADEditBodyContentRoles\">' + res[i] + '</div>');
                    $("GroupsForAccount" + i).attr('data-original-title', 'Group Associated To This Role Is  ' + AllUserUsers.instance.AllGroupCorrespondingRoles[res[i]]);
                }
            }
        }
        else {
            for (var i = 0; i < res.length; i++) {
                $('.umSec1Body').append('<div data-toggle = "tooltip"  id="GroupsForAccount' + i + '" data=\"' + res[i] + '\" class=\"RADEditBodyContentRoles\">' + res[i] + '</div>');
                $("GroupsForAccount" + i).attr('data-original-title', 'Group Associated To This Role Is  ' + AllUserUsers.instance.AllGroupCorrespondingRoles[res[i]]);
            }
        }

        $(".RADEditBodyContentRoles").unbind().click(function (event) {
            var groupname = $(event.target).text()
            if ($(this).hasClass("TextSelected") == true) {

                $(event.target).removeClass('TextSelected')
                $(event.target).addClass('RADEditBodyContentRoles')
                var dummyArray = [];
                for (var i = 0; i < $(".umSec1Body").find(".TextSelected").length; i++) {
                    if (AllUserUsers.instance.AllGroupCorrespondingRoles[$(".umSec1Body").find(".TextSelected")[i].innerText] != null) {
                        dummyArray.push(AllUserUsers.instance.AllGroupCorrespondingRoles[$(".umSec1Body").find(".TextSelected")[i].innerText])
                    }
                }
                var index = parseInt(AllUserUsers.instance.GetSelectedGroupsInReadOnlyScreenForSearch.indexOf(groupname));
                if (index != -1) {
                    AllUserUsers.instance.GetSelectedGroupsInReadOnlyScreenForSearch.splice(index, 1);
                }
                var a = []

                a.push(AllUserUsers.instance.AllGroupCorrespondingRoles[groupname])
                var roletext = "";
                if (a[0] != null) {
                    AllUserUsers.instance.GetRolesOrangeForCreateInSearchReadOnly = a[0];

                    for (var k = 0; k < a[0].length; k++) {
                        if ($(".umSec2Body").find('div[data="' + a[0][k] + '"]')) {                            
                            roletext = $(".umSec2Body").find('div[data="' + a[0][k] + '"]').html()
                            if (dummyArray.length > 0) {
                                if (dummyArray[0].indexOf(roletext) == -1) {
                                    $(".umSec2Body").find('div[data="' + a[0][k] + '"]').removeClass("RADGrouoInfobodycontent")
                                    $(".umSec2Body").find('div[data="' + a[0][k] + '"]').addClass("RADEditBodyContentRolesForCreate")
                                }
                            }
                            else {
                                $(".umSec2Body").find('div[data="' + a[0][k] + '"]').removeClass("RADGrouoInfobodycontent")
                                $(".umSec2Body").find('div[data="' + a[0][k] + '"]').addClass("RADEditBodyContentRolesForCreate")
                            }
                        }
                    }
                }
            }
            else {
                var roletext = "";

                $(event.target).removeClass('RADCreateGroupBodyContent');
                $(event.target).removeClass('RADEditBodyContentRolesForCreate');
                $(event.target).addClass('TextSelected ');
                if (AllUserUsers.instance.GetSelectedGroupsInReadOnlyScreenForSearch.indexOf(groupname) == -1)
                    AllUserUsers.instance.GetSelectedGroupsInReadOnlyScreenForSearch.push(groupname);

                var a = []
                a.push(AllUserUsers.instance.AllGroupCorrespondingRoles[groupname])
                if (a[0] != null) {
                    AllUserUsers.instance.GetRolesOrangeForCreateInSearchReadOnly = a[0];
                    for (var k = 0; k < a[0].length; k++) {
                        if ($(".umSec2Body").find('div[data="' + a[0][k] + '"]')) {
                            if (!$(".umSec2Body").find('div[data="' + a[0][k] + '"]').hasClass("TextSelected")) {
                                $(".umSec2Body").find('div[data="' + a[0][k] + '"]').removeClass("RADCreateGroupBodyContent")
                                $(".umSec2Body").find('div[data="' + a[0][k] + '"]').removeClass(" RADEditBodyContentRolesForCreate")
                                $(".umSec2Body").find('div[data="' + a[0][k] + '"]').addClass("RADGrouoInfobodycontent")
                                roletext = $(".umSec2Body").find('div[data="' + a[0][k] + '"]').html();
                                if (AllUserUsers.instance.GetRolesOrangeForCreateInSearchReadOnly.indexOf(roletext) == -1)
                                    AllUserUsers.instance.GetRolesOrangeForCreateInSearchReadOnly.push(roletext)
                            }
                        }
                    }
                }
            }
        });

        $(".RADUserMgmtbodycontent").unbind().click(function (event) {
            if ($(this).hasClass("TextSelected") == true) {
                $(this).switchClass("TextSelected", "RADCreateGroupBodyContent")
            }
        });

    });
}

AllUserUsers.prototype.CreateGetAllAccounts = function () {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllAccounts',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var res;
        res = JSON.parse(responseText.d);

        $('#LabelHeaderAccount').append("<div id=\"RADUserManagementAccountDetailsParent\" class=\"RADUserManagementAccountDetails\"></div>");
        $('#RADUserManagementAccountDetailsParent').append("<div id=\"RADUserManagementAccountDetails\" class=\"RADUserManagementFirstAccountDiv\">Accounts</div>");
        $("#RADUserManagementAccountDetailsParent").append("<div class=\"fa fa-caret-down downarrowdropdown\"></div>");

        for (var i = 0; i < res.length; i++)
            $('#RADUserManagementAccountDetailsParent').append("<div class=\"RADUserManagementEachAccountDiv\">" + res[i].AccountName + "</div>");
        $(".RADUserManagementFirstAccountDiv").unbind().click(function (event) {
            AllUserUsers.instance.ShowAccountsDropDown();
        });

        $(".RADUserManagementEachAccountDiv").unbind().click(function (event) {
            AllUserUsers.prototype.SelectAccountsOptions(event);
            var Accounts = $(".RADUserManagementFirstAccountDiv").text()
            AllUserUsers.instance.GetGroupsForAccount(Accounts);
        });

        
    })
}

AllUserUsers.prototype.CreateSearchText = function (event) {
    var flagSearch = false
    $(".umDynamicTiles").empty();
    var userLoginNameInLower = "";
    var userFNameInLower = "";
    var userLNameInLower = "";

    for (var j = 0; j < AllUserUsers.instance.AllUsersList.length; j++) {

        userLoginNameInLower = AllUserUsers.instance.AllUsersList[j]["UserLoginName"].toLowerCase();
        userFNameInLower = AllUserUsers.instance.AllUsersList[j]["FirstName"].toLowerCase();
        userLNameInLower = AllUserUsers.instance.AllUsersList[j]["LastName"].toLowerCase();

        $('.umDynamicTiles').append('<div  title="' + AllUserUsers.instance.AllUsersList[j].UserLoginName + '"  id="t' + j + '" name="' + AllUserUsers.instance.AllUsersList[j].UserLoginName + '"></div>');
        if (AllUserUsers.instance.AllUsersList[j]["UserLoginName"] == AllUserUsers.instance.SetUser &&
           ((userLoginNameInLower.indexOf($(event.target).val().toLowerCase()) != -1)
              || (userFNameInLower.indexOf($(event.target).val().toLowerCase()) != -1)
              || (userLNameInLower.indexOf($(event.target).val().toLowerCase()) != -1))) {
            flagSearch = true
            firstUser = AllUserUsers.instance.AllUsersList[j].UserLoginName
            $('.umDynamicTiles').append('<div title="' + AllUserUsers.instance.AllUsersList[j].UserLoginName + '" id="t' + j + '" name="' + AllUserUsers.instance.AllUsersList[j].UserLoginName + '"></div>');
            $("#t" + j).addClass('RADUserMgmttiles').addClass('RADTileActive');
            $("#t" + j).closest(".RADUserMgmttiles").append('<div id="arr"></div>');
            $(".umDynamicTiles").find(".RADarrow-right").remove();
            $("#arr").addClass('fa fa-caret-right RADarrow-right').insertAfter("#t" + j);
            $("#t" + j).append('<div title="' + AllUserUsers.instance.AllUsersList[j].UserLoginName + '" id="user' + j + '" name="' + AllUserUsers.instance.AllUsersList[j].UserLoginName + '" FirstName="' + AllUserUsers.instance.AllUsersList[j].FirstName + '" LastName="' + AllUserUsers.instance.AllUsersList[j].LastName + '" EmailId="' + AllUserUsers.instance.AllUsersList[j].EmailId + '">' + AllUserUsers.instance.AllUsersList[j].UserLoginName + '</div>');
            $("#user" + j).addClass('RADUserMgmtuser_name');
            $("#t" + j).append('<div title="' + AllUserUsers.instance.AllUsersList[j].FirstName + '"id="first' + j + '" FirstName="' + AllUserUsers.instance.AllUsersList[j].FirstName + '">' + '(' + AllUserUsers.instance.AllUsersList[j].FirstName + " " + '</div>');
            $("#first" + j).addClass('RADUserMgmtfirst_name');
            $("#t" + j).append('<div title="' + AllUserUsers.instance.AllUsersList[j].LastName + '"id="last' + j + '" LastName="' + AllUserUsers.instance.AllUsersList[j].LastName + '">' + AllUserUsers.instance.AllUsersList[j].LastName + ')' + '</div>');
            $("#last" + j).addClass('RADUserMgmtlast_name');
            if (AllUserUsers.instance.AllUsersList[j].title != null && AllUserUsers.instance.AllUsersList[j].title != "") {
                $("#t" + j).append('<div title="' + AllUserUsers.instance.AllUsersList[j].title + '"id="title' + j + '" Title="' + AllUserUsers.instance.AllUsersList[j].title + '">' + AllUserUsers.instance.AllUsersList[j].title + '</div>');
                $("#title" + j).addClass('RADUserMgmtemail_id');
            }
            $("#t" + j).append('<div title="' + AllUserUsers.instance.AllUsersList[j].EmailId + '"  id="email' + j + '" EmailId="' + AllUserUsers.instance.AllUsersList[j].EmailId + '">' + AllUserUsers.instance.AllUsersList[j].EmailId + '</div>');
            $("#email" + j).addClass('RADUserMgmtemail_id');
            $("#FirstName").val(AllUserUsers.instance.AllUsersList[j].UserLoginName);
            $("#LastName").val(AllUserUsers.instance.AllUsersList[j].FirstName);
            $("#Email").val(AllUserUsers.instance.AllUsersList[j].EmailId);
            if (Priviliges.indexOf("Add User Signature") != -1)
                $("#t" + j).closest(".RADUserMgmttiles").append('<div id="tSign' + j + '" class="RADUserMgmtUserSign"></div>')

        }

        else if ((userLoginNameInLower.indexOf($(event.target).val().toLowerCase()) != -1) || (userFNameInLower.indexOf($(event.target).val().toLowerCase()) != -1)
                  || (userLNameInLower.indexOf($(event.target).val().toLowerCase()) != -1)) {
            flagSearch = true
            $('.umDynamicTiles').append('<div title="' + AllUserUsers.instance.AllUsersList[j].UserLoginName + '" id="t' + j + '" name="' + AllUserUsers.instance.AllUsersList[j].UserLoginName + '" FirstName="' + AllUserUsers.instance.AllUsersList[j].FirstName + '" LastName="' + AllUserUsers.instance.AllUsersList[j].LastName + '" EmailId="' + AllUserUsers.instance.AllUsersList[j].EmailId + '"></div>');
            $("#t" + j).addClass('RADUserMgmttiles');
            $("#t" + j).append('<div title="' + AllUserUsers.instance.AllUsersList[j].UserLoginName + '" id="user' + j + '">' + AllUserUsers.instance.AllUsersList[j].UserLoginName + '</div>');
            $("#user" + j).addClass('RADUserMgmtuser_name');
            $("#t" + j).append('<div title="' + AllUserUsers.instance.AllUsersList[j].FirstName + '"id="first' + j + '">' + '(' + AllUserUsers.instance.AllUsersList[j].FirstName + " " + '</div>');
            $("#first" + j).addClass('RADUserMgmtfirst_name');
            $("#t" + j).append('<div title="' + AllUserUsers.instance.AllUsersList[j].LastName + '" id="last' + j + '">' + AllUserUsers.instance.AllUsersList[j].LastName + ')' + '</div>');
            $("#last" + j).addClass('RADUserMgmtlast_name');
            if (AllUserUsers.instance.AllUsersList[j].title != null && AllUserUsers.instance.AllUsersList[j].title != "") {
                $("#t" + j).append('<div title="' + AllUserUsers.instance.AllUsersList[j].title + '"id="title' + j + '">' + AllUserUsers.instance.AllUsersList[j].title + '</div>');
                $("#title" + j).addClass('RADUserMgmtemail_id');
            }
            $("#t" + j).append('<div title="' + AllUserUsers.instance.AllUsersList[j].EmailId + '" id="email' + j + '">' + AllUserUsers.instance.AllUsersList[j].EmailId + '</div>');
            $("#email" + j).addClass('RADUserMgmtemail_id');
            if (Priviliges.indexOf("Add User Signature") != -1)
                $("#t" + j).closest(".RADUserMgmttiles").append('<div id="tSign' + j + '" class="RADUserMgmtUserSign"></div>')
        }
    }
    if (flagSearch == false) {

        $('.umDynamicTiles').append('<div id="RADNouserfound" class=\"RADnouserfound"\>Result Not found !!!</div>')
        flagSearch = true
    }

    $(".RADUserMgmttiles").unbind().click(function (event) {
        AllUserUsers.instance.ClickOnCreateTiles(event);

    });
}


AllUserUsers.prototype.CreateButtonClick = function () {
    GlobalFlagSearch = true;
    AllUserUsers.instance.SelectedGroupsForCreateSearch = []
    AllUserUsers.instance.SelectedRolesForCreateSearch = []
    $(".RADUserManagementAuditButton").hide()
    $(".RADUserManagementEditButton").val("Save")
    $(".RADUserManagementEditButton").attr('id', 'RADUserManagementCreateSaveButton')
    $(".RADUserManagementEditButton").show();
    $(".RADUserManagementDeleteButton").val("Cancel")
    $(".RADUserManagementDeleteButton").attr('id', 'RADUserManagementCancelButton')
    $(".RADUserManagementDeleteButton").show();
    $('.RADUserManagaementLabel').remove();
    $('#lab').remove();
    $("#RADUserManagementResetButtonPassword").hide()
    $('.RADUserManagementAccountDetails').remove();
    $('.RADUserManagementFirstAccountDiv').remove();
    $('.RADUserManagementEachAccountDiv').remove();
    $(".umRightHeader").empty();
    $('.RADSCreenSearchParent').remove();
    $('.RADSCreenSearchParentSec2').remove();
    $('#RADUMRoleSearch').remove();
    $('#Rolesearch_text').remove();
    $("<div id=\"RADSCreenSearchParent\"  class=\"RADSCreenSearchParent\"></div>").insertAfter($('.umSec1Head'))

    $(".umRightHeader").css({ "width": "70%" })
    $(".UMRightHeaderEdit").css({ "width": "30%" })

    $(".umRightHeader").append('<div id=\"LabelHeaderUname\" class=\"RADUserMgmtLabelHeader\" ></div>');
    $("#LabelHeaderUname").append('<div class=\"RADEditCreateFirstLabel\" >UserName </div>');
    $('#LabelHeaderUname').append('<input id=\"CreateUserName\" class=\"RADUserManagementCreateDeatil\" contenteditable=\"true\" placeholder=\"UserName\"></input>');

    $(".umRightHeader").append('<div id=\"LabelHeaderFname\" class=\"RADUserMgmtLabelHeader\" ></div>');
    $("#LabelHeaderFname").append('<div class=\"RADEditCreateUserLabel\" >FirstName </div>');
    $('#LabelHeaderFname').append('<input id=\"CreateFirstName\" class=\"RADUserManagementCreateDeatil\" contenteditable=\"true\" placeholder=\"FirstName\"></input>');

    $(".umRightHeader").append('<div id=\"LabelHeaderLname\" class=\"RADUserMgmtLabelHeader\" ></div>');
    $("#LabelHeaderLname").append('<div class=\"RADEditCreateLastLabel\" >LastName </div>');
    $('#LabelHeaderLname').append('<input id=\"CreateLastName\" class=\"RADUserManagementCreateDeatil\" contenteditable=\"true\" placeholder=\"LastName\"></input>');

    $(".umRightHeader").append('<div id=\"LabelHeaderEmail\" class=\"RADUserMgmtLabelHeader\" ></div>');
    $("#LabelHeaderEmail").append('<div class=\"RADEditCreateEmailLabel\" >Email </div>');
    $('#LabelHeaderEmail').append('<input id=\"CreateEmail\" class=\"RADUserManagementCreateDeatil\" contenteditable=\"true\" placeholder=\"Email\"></input>');

    $(".umRightHeader").append('<div id=\"LabelHeaderAccount\" class=\"RADUserMgmtLabelHeader\" ></div>');
    $("#LabelHeaderAccount").append('<div class=\"RADEditCreateAccountLabel\" >Account </div>');


    AllUserUsers.instance.CreateGetAllAccounts();
    $(".RADUserManagementFirstAccountDiv").unbind().click(function (event) {
        AllUserUsers.instance.ShowAccountsDropDown();
    });
    $(".RADUserManagementEachAccountDiv").unbind().click(function (event) {
        AllUserUsers.instance.SelectAccountsOptions(event);
        var Accounts = $(".RADUserManagementFirstAccountDiv").text()
        AllUserUsers.instance.GetGroupsForAccount(Accounts);
    });
    $('.RADUserMgmtbodycontent').remove();
    AllUserUsers.instance.CreateGetAllRoles();
}

//Create user on active Directory basis
AllUserUsers.prototype.CreateActiveDirectoryUser = function () {

    $(".RADUserManagementAuditButton").hide()
    $(".RADUserManagementEditButton").val("Save")
    $(".RADUserManagementEditButton").attr('id', 'RADUserManagementCreateActiveDirectorySaveButton')
    $("#RADUserManagementCreateActiveDirectorySaveButton").show();
    $("#RADUserManagementDeleteButton").val("Cancel")
    $("#RADUserManagementDeleteButton").attr('id', 'RADUserManagementCancelButton')
    $("#RADUserManagementCancelButton").show();
    $('.RADUserManagaementLabel').remove();
    $('#lab').remove();
    $('.RADUserManagementAccountDetails').remove();
    $('.RADUserManagementFirstAccountDiv').remove();
    $('.RADUserManagementEachAccountDiv').remove();
    $(".umRightHeader").empty();

    $(".umRightHeader").append('<div id="RADUserManagaementUserLabel" class="RADUserManagaementLabel"></div>');
    $(".RADUserManagaementLabel").css({ "width": "35%" })
    $('#RADUserManagaementUserLabel').append('<div id=\"CreateActiveUser\" class=\"CreateActiveUser\" ></div>');
    $("#CreateActiveUser").append('<div class=\"RADEditLastLabelActiveUser\" >Users :</div>');

    AllUserUsers.instance.GetActiveDirectoryUser();
    AllUserUsers.instance.GetAllAccountsForActiveUser();
    $(".RADUserManagementFirstAccountDiv").unbind().click(function (event) {
        AllUserUsers.prototype.ShowAccountsDropDown();
    });
    $(".RADUserManagementEachAccountDiv").unbind().click(function (event) {
        AllUserUsers.prototype.SelectAccountsOptions(event);
        var Accounts = $(".RADUserManagementFirstAccountDiv").text()
        AllUserUsers.instance.GetGroupsForAccount(Accounts);
    });
    $('.RADUserMgmtbodycontent').remove();
    AllUserUsers.instance.CreateGetAllRoles();
}


//Get Active Directory User
AllUserUsers.prototype.GetActiveDirectoryUser = function () {

   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllAdUsers', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var AllActiveUsers = AllUserUsers.instance.GetInlineFilterDataForAllActiveUsers(responseText.d);
        AllUserUsers.instance.CreateInlineFilterForActiveDirectoryuser(AllActiveUsers);
        
    })
}

//Creating Inline Filter For Active User
AllUserUsers.prototype.GetInlineFilterDataForAllActiveUsers = function (AllActiveUsers) {
    var AcitveUserDetail = [];
    for (var k = 0; k < AllActiveUsers.length; k++) {
        var myObject = {};
        myObject.id = k;
        myObject.text = AllActiveUsers[k];
        AcitveUserDetail.push(myObject);
    }
    return AcitveUserDetail;
}

AllUserUsers.prototype.CreateInlineFilterForActiveDirectoryuser = function (ArrayOfActiveUser) {
    //if ($("#CreateActiveUser").inlineFilter("instance") != undefined) {
    //    $("#CreateActiveUser").inlineFilter('destroy');
    //}
    //if (ArrayOfActiveUser != null && ArrayOfActiveUser.length > 0) {
    //    $("#CreateActiveUser").inlineFilter({
    //        filterInfo: {
    //            filterPhrase: "{0}",
    //            bindingInfo: [
    //                    {
    //                        identifier: "CreateActiveUser",
    //                        data: ArrayOfActiveUser, //array with objects having keys id and text

    //                        multiple: true,
    //                        maxResultsToShow: 1,
    //                        placeholder: "Active User",
    //                        class: 'RADinlineFilterCSS'
    //                    }]
    //        }
    //                  , selectHandler: AllUserUsers.instance.UserTypeSelectHandler
    //    });
    //}

    var dropDownObject = {};
    dropDownObject["radEntitydefaultValue"] = 'Users';
    dropDownObject["radEntitydropDownAllData"] = ArrayOfActiveUser;
    dropDownObject["radEntityEnableMultiSelect"] = true;
    dropDownObject["radEntityAttributeNameText"] = 'Active |^';
    dropDownObject["radWidgetSelectHandler"] = '';
  
    //$("#CreateActiveUser").SelectDropDown('destroy');
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    $("#CreateActiveUser").SelectDropDown(dropDownObject);
 // AllUserUsers.prototype.AddSpinnerOnContentBody();
    
}

AllUserUsers.prototype.UserTypeSelectHandler = function (selectedValue, e) {
    var users;
    ActiveUser = [];

    if (selectedValue.length > 0) {
        var lengthOfuser = $("#CreateActiveUser").inlineFilter("getCurrentStateObjects")["CreateActiveUser"].length;
        for (var i = 0; i < lengthOfuser; i++) {
            users = ($("#CreateActiveUser").inlineFilter("getCurrentStateObjects")["CreateActiveUser"][i]).text;
            ActiveUser.push(users);
        }
    }
}


AllUserUsers.prototype.UserDetailsOfActiveUsers = function (ActiveUser) {
    var Groups = [];
    var Roles = [];
    var ArrGroups = [];
    var ArrRoles = [];
    var Accounts;
    // Accounts = ($(".RADUserManagementFirstAccountDiv").text())
    Accounts = selectedAccount;
    ArrGroups = ($(".umSec1Body").find(".TextSelected"))
    ArrRoles = ($(".umSec2Body").find(".TextSelected"));
    $(ArrGroups).each(function (i, e) {
        Groups.push($(e).text());
    })
    $(ArrRoles).each(function (i, e) {
        Roles.push($(e).text());
    })

    var dialog = $("#CreateActiveUser").data("ddn-SelectDropDown");
    ActiveUser = dialog._GetSelectedDataFromWidget(event);

    if (Groups == "" && Roles == "") {
        $("<div  class=\"RADUserManagementAccountGroupValidation\">!</div>").insertAfter($(".umSec1Head"));
        $("<div  class=\"RADUserManagementAccountGroupValidation\">!</div>").insertAfter($(".umSec2Head"));
        $(".RADUserManagementAccountGroupValidation").attr("title", "Select Atleast One Group Or Role")
    }

    else if (ActiveUser.length == 0) {

        $("<div id=\"RADUserNameValidation\" class=\"RADUserNameValidation\">!</div>").insertAfter($("#CreateActiveUser"))
        $("#RADUserNameValidation").attr("title", "Select Atleast One User")
    }

    else {        
        //AllUserUsers.instance.UserCreationOfActiveUser(JSON.stringify(Groups), JSON.stringify(Roles), Accounts, JSON.stringify(ActiveUser))
        AllUserUsers.instance.UserCreationOfActiveUser(JSON.stringify(Groups), JSON.stringify(Roles), Accounts, JSON.stringify(ActiveUser));
    }
}


AllUserUsers.prototype.UserCreationOfActiveUser = function (Groups, Roles, Accounts, ActiveUser) {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/CreateUsers',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ selectedUser: ActiveUser, group: Groups, role: Roles, account: Accounts })
    }).then(function (responseText) {
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        if (responseText.d == "") {
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperSuccessDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserSuccess\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Result</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\"> User Has Been Created Successfully!!! </div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 2700);
            var usersObject = new AllUserUsers();
            usersObject.createInitDiv();

        }
        else {
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserFailure\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Result</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">User Creation Failed</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 2700);
            var usersObject = new AllUserUsers();
            usersObject.createInitDiv();

        }

        
    });
}

AllUserUsers.prototype.GetAllAccountsForActiveUser = function () {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllAccounts',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        var res;
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        res = JSON.parse(responseText.d);

        $('.umRightHeader').append(' <div id="RADUserManagaementAccountLabeDiv" class="RADUserManagementLabel_New"><label id="lblAccountLabel" >Accounts :</label></div>');
        $('.umRightHeader').append("<div id=\"lab1\" class=\"RADUSERManagementAccountValue\"></div>");
        //$('.RADUSERManagementAccountValue').append("<div id=\"RADUserManagementAccountDetailsParent\" class=\"RADUserManagementAccountDetails\"></div>");
        //$('#RADUserManagementAccountDetailsParent').append("<div id=\"RADUserManagementAccountDetails\" class=\"RADUserManagementFirstAccountDiv\">Accounts</div>");
        //$("#RADUserManagementAccountDetailsParent").append("<div class=\"fa fa-caret-down downarrowdropdown\"></div>");

        //for (var i = 0; i < res.length; i++)
        //    $('#RADUserManagementAccountDetailsParent').append("<div class=\"RADUserManagementEachAccountDiv\">" + res[i].AccountName + "</div>");
        var ArrayOfActiveAccounts = [];
        
        for (var i = 0; i < res.length; i++) {
            var obj = {};
            obj["id"] = i;
            obj["text"] = res[i].AccountName;
            ArrayOfActiveAccounts.push(obj);
        }
        var dropDownObject = {};
        dropDownObject["radEntitydefaultValue"] = 'Accounts';
        dropDownObject["radEntitydropDownAllData"] = ArrayOfActiveAccounts;
        dropDownObject["radEntityEnableMultiSelect"] = false;
        dropDownObject["radEntityAttributeNameText"] = 'Accounts |^';
        dropDownObject["radWidgetSelectHandler"] = AllUserUsers.prototype.GetGroupsForAccount;
        $('.RADUSERManagementAccountValue').SelectDropDown(dropDownObject);
     

        //AllUserUsers.instance.GetGroupsForAccount("Accounts");
        //var dialog = $(".RADUSERManagementAccountValue").data("ddn-SelectDropDown");
        //var Accounts = dialog._GetSelectedDataFromWidget();
        //AllUserUsers.instance.GetGroupsForAccount(Accounts);

        //$(".RADUserManagementFirstAccountDiv").unbind().click(function (event) {
        //    AllUserUsers.instance.ShowAccountsDropDown();
        ////});
        //$(".RADDimensionDropDownChild_New").unbind().click(function (event) {
        //    //AllUserUsers.prototype.SelectAccountsOptions(event);
        //    //var Accounts = $(".RADUserManagementFirstAccountDiv").text()
        //    var Accounts = $(event.target)[0].innerText;
        //    AllUserUsers.instance.GetGroupsForAccount(Accounts);
        //});

     
    })
}

AllUserUsers.prototype.OnDeleteClick = function () {
    var deleteUser = $(".RADTileActive").children()[0].innerText;
    $("#alertParentDiv").remove();
    $("#ContentDiv").append("<div id=\"alertParentDivdel\" class=\"RADUserManagementAlertPopUpParent\"></div>");
    $("#alertParentDivdel").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
    $("#alertParentDivdel").append("<div class=\"RADUserManagementPopUpError\"></div>")
    $("#alertParentDivdel").append("<div class=\"RADUserErrorDivLeft RADUserAlert\"></div>"); //revisit
    $("#alertParentDivdel").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
    $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Alert</div>")
    $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Are You Sure You want to delete    " + (deleteUser).toUpperCase() + " ?</div>");
    $("#alertParentDivdel").append("<div class=\"RADUserManagementPopUpErrorFooter\"></div>");
    $(".RADUserManagementPopUpErrorFooter").append("<div class=\"RADUserManagementPopUpErrorYes\">Yes</div><div class=\"RADUserManagementPopUpErrorNo\">No</div>")
    $("#alertParentDivdel").css("top", "-200px");
    $("#alertParentDivdel").animate({ "top": "0px" });
    $(".RADUserManagementPopUpErrorYes").unbind().click(function (event) {
        $("#alertParentDivdel").remove();

        $("#RADUserManagementEditButton").val("Edit")
        var deleteUser = $(".RADTileActive").children()[0].innerText;
        AllUserUsers.prototype.DeleteUser(deleteUser);
    });
    $(".RADUserManagementPopUpErrorNo").unbind().click(function (event) {
        $("#alertParentDivdel").remove();
    });
}

AllUserUsers.prototype.BindEvents = function () {
    $(".UMRightHeaderEdit").on("click", "#RADUserManagementCancelButton", function (event) {

        isEditMode = false;
        $(".umRightHeader").css({ "width": "60%" })
        $(".UMRightHeaderEdit").css({ "width": "30%" })

        AllUserUsers.instance.ShowHideByPrivilegeType("Add");
        $("#RADUserManagementResetButtonPassword").show()
        var userName = AllUserUsers.instance.SetUser;
        $(".RADUserManagementAuditButton").show()
        AllUserUsers.instance.ReCreateDiv(userName)

    });
    $(".UMRightHeaderEdit").on("click", "#RADUserManagementDeleteInActiveButton", function (event) {
        $('.RADUserCreatePlusCircle').show();
        isEditMode = false;
        AllUserUsers.instance.DeleteInActiveUsers();
    });
    $(".umRightSec").on('click', '#RADUserManagementResetButtonPassword', function (event) {
        var UserNameForPasswordReset = $(".umDynamicTiles").find(".RADTileActive").children()[0].innerHTML
        AllUserUsers.instance.ResetPasswordButton(UserNameForPasswordReset)
    });

    $(".ContentDiv").unbind().click(function (event) {
        if ($(event.target).closest(".RADUserManagementAccountDetails").length == 0) {
            if ($(".RADUserManagementAccountDetails").height() != 25) {
                $(".RADUserManagementAccountDetails").css({ "height": "25px" })
            }
        }
    });
    $(".umLeftSec").on('click', '.RADsearchIconGridView', function (event) {
        if ($('#search_text:visible').length) {
            $("#search_text").hide("slow", function () {
                // Animation complete.
            });
        }
        else {
            $("#search_text").show("slow", function () {
                // Animation complete.
            });
        }
    });

    $(".umRightSec").on('click', '#RADUserManagementAuditButton', function (event) {
        AllUserUsers.instance.AuditScreen(event)
    });
    $(".umSec1").on('click', '#RADUserManagementScrollDownArrowGroup', function (event) {
        ScrollVariableGroup = ScrollVariableGroup + 50;
        $(".umSec1Body").scrollTop(ScrollVariableGroup);
    });
    $(".umSec1").on('click', '#RADUserManagementScrollUpArrowGroup', function (event) {
        if (ScrollVariableGroup > 0) {
            ScrollVariableGroup = ScrollVariableGroup - 50;
            $(".umSec1Body").scrollTop(ScrollVariableGroup);
        }
    });
    $(".umSec2").on('click', '#RADUserManagementScrollDownArrowRoles', function (event) {
        ScrollVariableRoles = ScrollVariableRoles + 50;
        $(".umSec2Body").scrollTop(ScrollVariableRoles);
    });
    $(".umSec2").on('click', '#RADUserManagementScrollUpArrowRoles', function (event) {
        if (ScrollVariableRoles > 0) {
            ScrollVariableRoles = ScrollVariableRoles - 50;
            $(".umSec2Body").scrollTop(ScrollVariableRoles);
        }
    });
    $("#search_text").keyup(function () {
        AllUserUsers.instance.CreateSearchText(event);
    });
    ;
    $(".umSec2Body").on("mouseover", ".RADGrouoInfobodycontent", function (event) {
        AllUserUsers.instance.GetHoverRolesInfo(event)
    });   
    $(".umSec2Body").on("mouseover", ".RADEditBodyContentRoles ", function (event) {
        AllUserUsers.instance.GetHoverRolesInfo(event)
    });    

    $(".umSec1Body").on("mouseover", ".RADEditBodyContent ", function (event) {
        AllUserUsers.instance.GetHoverGroupInfo(event)
    });
    $(".umSec1Body").on("mouseover", ".RADCreateGroupBodyContent ", function (event) {
        AllUserUsers.instance.GetHoverGroupInfo(event)
    });
    $(".umSec1Body").on("mouseover", ".bodycontent ", function (event) {
        AllUserUsers.instance.GetHoverGroupInfo(event)
    });
    $(".umSec1Body").on("mouseover", ".TextSelected ", function (event) {
        AllUserUsers.instance.GetHoverGroupInfo(event)
    });
  
    $(".umSec1").on("keyup", "#Groupsearch_text", function (event) {

        AllUserUsers.instance.GroupSearch(event)
    });
    $(".umSec2").on("keyup", "#Rolesearch_text", function (event) {
        AllUserUsers.instance.RoleSearch(event)
    });
    $(".UMRightHeaderEdit").on("click", "#RADUserManagementCreateActiveDirectorySaveButton", function (event) {
        AllUserUsers.instance.UserDetailsOfActiveUsers(ActiveUser);
    });
    $("#RADUserManagementScrollDownArrow").unbind().click(function (event) {
        ScrollVariable = ScrollVariable + 50;
        $(".umDynamicTiles").scrollTop(ScrollVariable);
    });
    $("#RADUserManagementScrollUpArrow").unbind().click(function (event) {
        if (ScrollVariable > 0) {
            ScrollVariable = ScrollVariable - 50;
            $(".umDynamicTiles").scrollTop(ScrollVariable);
        }
    });
    $("#RADUserCreatePlusCircle").unbind().click(function (event) {
        $('.RADUserCreatePlusCircle').hide();
        var ResponseForCreation;
       // AllUserUsers.prototype.AddSpinnerOnContentBody();
        var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
        $.ajax({
            url: url + '/IsLDAPEnabled',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json'
        }).then(function (responseText) {
            //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
            var ResponseForCreation = responseText.d;
            AllUserUsers.instance.IsLDAPEnabled = ResponseForCreation;
            $("#RADUserManagementResetButtonPassword").hide()
            if (ResponseForCreation == true) {
                ActiveUser = [];
                AllUserUsers.instance.CreateActiveDirectoryUser();
            }
            else {
                AllUserUsers.instance.CreateButtonClick()
            }
            
        })
    });

    $(".UMRightHeaderEdit").on("click", "#RADUserManagementDeleteButton", function (event) {
        AllUserUsers.instance.OnDeleteClick();
    });
    $(".UMRightHeaderEdit").on("click", ".RADUserManagementEditButton", function (event) {

        if (event.target.id == "RADUserManagementEditButton") {
            isEditMode = true;

            if ($(".RADTileActive").children()[0] == null) {

                if ($("#alertParentDivEdit") != null)
                    $("#alertParentDivEdit").remove();
                $("#ContentDiv").append("<div id=\"alertParentDivEdit\" class=\"RADUserManagementAlertPopUpParent\"></div>");
                $("#alertParentDivEdit").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
                $("#alertParentDivEdit").append("<div class=\"RADUserManagementPopUpError\"></div>")
                $("#alertParentDivEdit").append("<div class=\"RADUserErrorDivLeft RADUserAlert\"></div>"); //revisit
                $("#alertParentDivEdit").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
                $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Alert</div>")
                $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Please select user to edit </div>");
                $("#alertParentDivEdit").append("<div class=\"RADUserManagementPopUpErrorFooter\"></div>");
                $(".RADUserManagementPopUpErrorFooter").append("<div class=\"RADUserManagementPopUpErrorYes\">OK</div>")
                $("#alertParentDivEdit").css("top", "-200px");
                $("#alertParentDivEdit").animate({ "top": "0px" });
                $(".RADUserManagementPopUpErrorYes").unbind().click(function (event) {
                    isEditMode = false;
                    $("#alertParentDivEdit").remove();
                    $("#RADUMGroupSearch").remove();
                    $("#RADUMRoleSearch").remove();
                    $("#Groupsearch_text").remove();
                    $("#Rolesearch_text").remove();

                });

            }
            else {
                $(".RADUserManagementDeleteButton").val("Cancel")
                $(".RADUserManagementDeleteButton").attr('id', 'RADUserManagementCancelButton')
                $("#RADUserManagementCancelButton").show();
                $(event.target).val("Save")
                $(event.target).attr('id', 'RADUserManagementSaveButton');
                userName = $(".RADTileActive").children()[0].innerText;
                AllUserUsers.instance.OnEditClick(userName)
            }
        }
        else if (event.target.id == "RADUserManagementSaveButton") {
            isEditMode = false;
            AllUserUsers.instance.OnSaveModifyValidation();
            //checking for privileges
            AllUserUsers.instance.ShowHideByPrivilegeType("Add");
        }
        else if (event.target.id == "RADUserManagementCreateSaveButton") {
            //checking for privileges
            AllUserUsers.instance.ShowHideByPrivilegeType("Add");
            AllUserUsers.instance.CreateUserDetails();
        }
    });
}

AllUserUsers.prototype.DeleteInActiveUsers = function () {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/DeleteInactiveUsers',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
          //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        $("#ContentDiv").append("<div id=\"alertParentDivInActive\" class=\"RADUserManagementAlertPopUpParent\"></div>");
        $("#alertParentDivInActive").append("<div class=\"RADUserManagementPopupAlertUpperSuccessDiv\"></div>");
        $("#alertParentDivInActive").append("<div class=\"RADUserManagementPopUpError\"></div>")
        $("#alertParentDivInActive").append("<div class=\"RADUserErrorDivLeft RADUserSuccess\"></div>"); //revisit
        $("#alertParentDivInActive").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
        $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Success</div>")
        $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Inactive Users have been deleted successfully</div>");
        $("#alertParentDivInActive").css("top", "-200px");
        $("#alertParentDivInActive").animate({ "top": "0px" });
        setInterval(function () {
            $("#alertParentDivInActive").remove();

        }, 2700);
        var usersObject = new AllUserUsers();
        usersObject.createInitDiv();
      
    });
}

AllUserUsers.prototype.OnSaveModifyValidation = function () {
    if ($("#FirstName").val() == "" || $("#LastName").val() == "" || $("#Email").val() == "" || (($(".umSec1Body").find(".TextSelected").length == 0) && ($(".umSec2Body").find(".TextSelected").length == 0))) {
        if ($("#FirstName").val() == "" || ($(".umRightHeader").find(".RADUserNameModifyValidation") == 0)) {
            $("<div id=\"RADUserNameValidation\" class=\"RADUserNameModifyValidation\">!</div>").insertAfter($("#FirstName"))
            $("#RADUserNameValidation").attr("title", "FirstName Can't Be Empty")
        }
        if ($("#LastName").val() == "" || ($(".umRightHeader").find(".RADLastNameModifyValidation") == 0)) {
            $("<div id=\"RADLastNameValidation\" class=\"RADLastNameModifyValidation\">!</div>").insertAfter($("#LastName"))
            $("#RADLastNameValidation").attr("title", "LastName Can't Be Empty")
        }
        if ($("#Email").val() == "" || ($(".umRightHeader").find(".RADEmailModifyValidation") == 0)) {
            $("<div id=\"RADEmailValidation\" class=\"RADEmailModifyValidation\">!</div>").insertAfter($("#Email"))
            $("#RADEmailValidation").attr("title", "Email Can't Be Empty")
        }
        if ((($(".umSec1Body").find(".TextSelected").length == 0) && ($(".umSec2Body").find(".TextSelected").length == 0)) || ($(".umSec1Head").find(".RADUserManagementModifyAccountGroupValidation") == 0)) {
            $("<div  class=\"RADUserManagementAccountGroupValidation\">!</div>").insertAfter($(".umSec1Head"));
            $("<div  class=\"RADUserManagementAccountGroupValidation\">!</div>").insertAfter($(".umSec2Head"));
            $(".RADUserManagementAccountGroupValidation").attr("title", "Select Atleast One Group Or Role")
        }
    }
    else {
        $(".RADModifyFirstNameValidation").remove();
        $(".RADModifyLastNameValidation").remove();
        $(".RADModifyEmailValidation").remove();
        $(".RADUserManagementModifyAccountGroupValidation").remove();
        AllUserUsers.prototype.ModifyUserDetails();
        $(event.target).val("Edit")
        $(event.target).attr('id', 'RADUserManagementEditButton');
        //checking for privileges
        AllUserUsers.instance.ShowHideByPrivilegeType("Edit");
    }
}


AllUserUsers.prototype.OnEditClick = function (userName) {

    //Get the User Selected Groups/ Roles, based on the current selected user details 
    var currentUserdetails = AllUserUsers.instance.CurrentSelectedUserDetails;

    $(".RADUserManagementAuditButton").hide();
    $("#RADUserManagementResetButtonPassword").hide();
    $(".umRightHeader").css({ "width": "60%" });
    $(".UMRightHeaderEdit").css({ "width": "40%" });
    $(".searchBarUm").hide();
    $('.RADUserCreatePlusCircle').hide();
    $(".umSec1Body").empty();
    $(".umSec2Body").empty();
    

    // Getting all the information at once used for Editing the user (Groups/Roles/Accounts), and save it in userEditInfo object
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    var userEditInfo = new Object();

    //Get All Accounts
    $.ajax({
        url: url + '/GetAllAccounts',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();        
        if (AllUserUsers.prototype.JSONTest(responseText.d))
        {
            userEditInfo.AccountsEditInfo = JSON.parse(responseText.d)
        }
        else
        {
            userEditInfo.AccountsEditInfo = "";
        }
        //Get All Groups
       // AllUserUsers.prototype.AddSpinnerOnContentBody();
        $.ajax({
            url: url + '/GetAllGroups',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json'
        }).then(function (responseText) {
            //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
            userEditInfo.GroupsEditInfo = JSON.parse(responseText.d);
            //Get Groups for User Current Account
             // AllUserUsers.prototype.AddSpinnerOnContentBody();
            $.ajax({
                url: url + '/GetGroupsForAccount',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ accountName: currentUserdetails.Accounts[0] })
            }).then(function (responseText) {
                //AllUserUsers.prototype.RemoveSpinnerOnContentBody();                

                if (AllUserUsers.prototype.JSONTest(responseText.d)) {
                    userEditInfo.GroupsForCurrentUserAccountEditInfo = JSON.parse(responseText.d)
                }
                else {
                    userEditInfo.GroupsForCurrentUserAccountEditInfo = "";
                }
              
                //Get All Roles
               // AllUserUsers.prototype.AddSpinnerOnContentBody();
                $.ajax({
                    url: url + '/GetAllRoles',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json'
                }).then(function (responseText) {
                      //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
                    userEditInfo.RolesEditInfo = JSON.parse(responseText.d);

                    if (AllUserUsers.instance.IsLDAPEnabled == false) {
                        AllUserUsers.instance.BindControlsonEditNonLdapMode(currentUserdetails, userEditInfo);
                    }
                    else {
                        AllUserUsers.instance.BindControlsonEditLdapMode(currentUserdetails, userEditInfo);
                    }

                    AllUserUsers.prototype.BindaccountsOnEdit(currentUserdetails, userEditInfo);
                    AllUserUsers.prototype.BindGroupsOnEdit(currentUserdetails, userEditInfo);
                    AllUserUsers.prototype.BindRolesOnEdit(currentUserdetails, userEditInfo);
                    
                });
            });
        });
    });
}

AllUserUsers.prototype.BindaccountsOnEdit = function (currentUserdetails, userEditInfo) {
    // Show Accounts DropDown
    $(".RADUserManagementFirstAccountDiv").unbind().click(function (event) {
        AllUserUsers.prototype.ShowAccountsDropDown();
    });
    // Get Groups for Account selected from accounts dropdown
    $(".RADUserManagementEachAccountDiv").unbind().click(function (event) {
        AllUserUsers.prototype.SelectAccountsOptions(event);
        var AccountName = $(".RADUserManagementFirstAccountDiv").text()
        AllUserUsers.instance.GetGroupsForSelectedAccountName(AccountName, currentUserdetails);
    });
}

AllUserUsers.prototype.BindRolesOnEdit = function (currentUserdetails, userEditInfo) {
    //GetAllRoles
    $('.GroupAssociatedRoles').empty();
    $('.UserAssociatedRoles').empty();
    $('.AllEditableRoles').empty();

    //Bind Groups Associated Roles of User
    var userGroupsassociatedRolesUnique = [];

    for (var i = 0; i < currentUserdetails.Groups.length; i++) {
        var currentGroup = currentUserdetails.Groups[i];
        var currentGroupInfo = $.grep(userEditInfo.GroupsEditInfo, function (e) { return e.GroupName == currentGroup; });
        var rolesForCurrentGroupInfo = currentGroupInfo[0].Roles;

        for (var j = 0; j < rolesForCurrentGroupInfo.length; j++) {
            if (userGroupsassociatedRolesUnique.indexOf(rolesForCurrentGroupInfo[j]) == -1) {
                userGroupsassociatedRolesUnique.push(rolesForCurrentGroupInfo[j])
            }
        }
    }
    for (var k = 0; k < userGroupsassociatedRolesUnique.length; k++) {
        if(currentUserdetails.Roles.indexOf(userGroupsassociatedRolesUnique[k]) == -1){
        
            $('.GroupAssociatedRoles').append('<div data-toggle = "tooltip" data=\"' + userGroupsassociatedRolesUnique[k] + '\" id="GetGroupInfo' + j + '" class=\"RADGrouoInfobodycontent">' + userGroupsassociatedRolesUnique[k] + '</div>');
            j++
        }
    }

    //Bind Roles Associated to User
    for (var i = 0; i < currentUserdetails.Roles.length; i++) {        
            $('.UserAssociatedRoles').append('<div data-toggle = "tooltip" data=\"' + currentUserdetails.Roles[i] + '\" id="RolesSelected' + i + '" class=\"RADEditBodyContentRoles">' + currentUserdetails.Roles[i] + '</div>');
            $("#RolesSelected" + i).addClass('TextSelected');
    }

    //Bind all remaining available roles
    for (var i = 0; i < userEditInfo.RolesEditInfo.length; i++) {

        if (currentUserdetails.Roles.indexOf(userEditInfo.RolesEditInfo[i].RoleName) == -1 &&
            userGroupsassociatedRolesUnique.indexOf(userEditInfo.RolesEditInfo[i].RoleName) == -1) {
            $('.AllEditableRoles').append('<div data-toggle = "tooltip" data=\"' +
                userEditInfo.RolesEditInfo[i].RoleName + '\" id="Roles' + i + '" class=\"RADEditBodyContentRoles">'
                + userEditInfo.RolesEditInfo[i].RoleName + '</div>');

        }
    }

    $(".RADEditBodyContentRoles,.RADGrouoInfobodycontent").unbind().click(function (event) {
        if ($(this).hasClass("RADGrouoInfobodycontent")) {
            //do nothing
            return;
        }
        else {
            if ($(this).hasClass("TextSelected") == true) {
                if (userGroupsassociatedRolesUnique.indexOf($(event.target).text()) == -1)
                    $(this).switchClass("TextSelected", "RADEditBodyContentRoles");
                else
                    $(this).switchClass("TextSelected", "RADGrouoInfobodycontent");
                currentUserdetails.Roles.splice(currentUserdetails.Roles.indexOf($(event.target).text()));
            }
            else {
                $(this).addClass('TextSelected');
                currentUserdetails.Roles.push($(event.target).text());
            }
        }
    });

}

AllUserUsers.prototype.BindGroupsOnEdit = function (currentUserdetails, userEditInfo) {

    var currentUserdetailsCopy = currentUserdetails;
    var userEditInfoCopy = userEditInfo;    
    $('.umSec2Body').append('<div class=\"GroupAssociatedRoles\"><\div>');
    $('.umSec2Body').append('<div class=\"UserAssociatedRoles\"><\div>');
    $('.umSec2Body').append('<div class=\"AllEditableRoles\"><\div>');

    // first bind the selected groups for user 
    for (var i = 0; i < currentUserdetails.Groups.length; i++) {
        $('.umSec1Body').append('<div data-toggle = "tooltip" data=\"'
            + currentUserdetails.Groups[i] + '\" id="Groups' + i + '" class=\"RADEditBodyContent">'
            + currentUserdetails.Groups[i] + '</div>');
        $($(".umSec1Body").children()[i]).addClass('TextSelected');

    }

    // Then bind the all groups for user for current account of user
    for (var i = 0; i < userEditInfo.GroupsForCurrentUserAccountEditInfo.length; i++) {
        if (currentUserdetails.Groups.indexOf(userEditInfo.GroupsForCurrentUserAccountEditInfo[i]) == -1) {
            $('.umSec1Body').append('<div data-toggle = "tooltip" data=\"' + userEditInfo.GroupsForCurrentUserAccountEditInfo[i]
                + '\" id="GroupBody' + i + '" class=\"RADEditBodyContent">' + userEditInfo.GroupsForCurrentUserAccountEditInfo[i] + '</div>');
        }
    }

    $(".RADEditBodyContent").unbind().click(function (event) {        
        AllUserUsers.instance.HandleSelectUnselectofGroupsOnEdit(currentUserdetails, userEditInfo);
    });
}

AllUserUsers.prototype.HandleSelectUnselectofGroupsOnEdit = function (currentUserdetails, userEditInfo) {

    if ($(event.target).hasClass("TextSelected") == true) {
        $(event.target).switchClass("TextSelected", "RADEditBodyContent");        

        var allRolesForUserSlectedGroups = [];
        var rolesToUnSelect = [];
        var distinctRolesForUserSelectedGroups = [];

        var currentUnselectedGroupName = $(event.target).text();
        currentUserdetails.Groups.splice(currentUserdetails.Groups.indexOf(currentUnselectedGroupName), 1);
        
        var currentUnselectedGroupInfo = $.grep(userEditInfo.GroupsEditInfo, function (e)
        { return e.GroupName == currentUnselectedGroupName; });        
        var rolesForCurrentUnSelectedGroup = currentUnselectedGroupInfo[0].Roles;

        $.each(currentUserdetails.Groups, function (key, element) {
            var currentelementInfo = $.grep(userEditInfo.GroupsEditInfo, function (e)
            { return e.GroupName == element; });
            var currentelementRolesInfo = currentelementInfo[0].Roles;
            $.each(currentelementRolesInfo, function (key2, element2) {
                if (distinctRolesForUserSelectedGroups.indexOf(element2) == -1) {
                    distinctRolesForUserSelectedGroups.push(element2);
                }
            });
        });
       
        $.each(rolesForCurrentUnSelectedGroup, function (key, element) {
            if (distinctRolesForUserSelectedGroups.indexOf(element) == -1) {
                rolesToUnSelect.push(element);
            }
        });

        $.each(rolesToUnSelect, function (key, element) {

            if ($(".umSec2Body").find('div[data="' + element + '"]')) {
                $(".umSec2Body").find('div[data="' + element + '"]').removeClass("RADGrouoInfobodycontent")
                $(".umSec2Body").find('div[data="' + element + '"]').addClass("RADEditBodyContentRoles")
            }
        })
    }
    else {
        $(event.target).addClass('TextSelected');

        var currentSelectedGroupName = $(event.target).text();
        currentUserdetails.Groups.push(currentSelectedGroupName);
        var currentSelectedGroupInfo = $.grep(userEditInfo.GroupsEditInfo, function (e)
        { return e.GroupName == currentSelectedGroupName; });
        var rolesForcurrentSelectedGroup = currentSelectedGroupInfo.length>0 ? currentSelectedGroupInfo[0].Roles : [];

        $.each(rolesForcurrentSelectedGroup, function (key, element) {            
            if (!($(".umSec2Body").find('div[data="' + element + '"]').hasClass('RADGrouoInfobodycontent')
            || $(".umSec2Body").find('div[data="' + element + '"]').hasClass('TextSelected'))) {

                $(".umSec2Body").find('div[data="' + element + '"]').removeClass("RADEditBodyContentRoles")
                $(".umSec2Body").find('div[data="' + element + '"]').addClass("RADGrouoInfobodycontent");
            }
        });
    }
}

AllUserUsers.prototype.BindControlsonEditNonLdapMode = function (currentUserdetails, userEditInfo) {

    $(".umRightHeader").empty();
    $('.RADUserManagementAccountDetails').remove();
    $('.RADUserManagementFirstAccountDiv').remove();
    $('.RADUserManagementEachAccountDiv').remove();

    $(".umRightHeader").append('<div id=\"LabelHeaderFname\" class=\"RADUserMgmtLabelHeader\" ></div>');
    $("#LabelHeaderFname").append('<div class=\"RADEditFirstLabel\" >FirstName </div>');
    $('#LabelHeaderFname').append('<input id=\"FirstName\" class=\"RADUserManagementFirstName\" contenteditable=\"true\" placeholder=\"FirstName\"></input>');

    $(".umRightHeader").append('<div id=\"LabelHeaderLname\" class=\"RADUserMgmtLabelHeader\" ></div>');
    $("#LabelHeaderLname").append('<div class=\"RADEditLastLabel\" >LastName </div>');
    $('#LabelHeaderLname').append('<input id=\"LastName\" class=\"RADUserManagementFirstName\" contenteditable=\"true\" placeholder=\"LastName\"></input>');

    $(".umRightHeader").append('<div id=\"LabelHeaderEmail\" class=\"RADUserMgmtLabelHeader\" ></div>');
    $("#LabelHeaderEmail").append('<div class=\"RADEditEmailLabel\" >Email </div>');
    $('#LabelHeaderEmail').append('<input id=\"Email\" class=\"RADUserManagementFirstName\" contenteditable=\"true\" placeholder=\"Email\"></input>');


    $(".umRightHeader").append('<div id=\"LabelHeaderAccount\" class=\"RADUserMgmtLabelHeader\" ></div>');

    $('#LabelHeaderAccount').append('<div class=\"RADEditEmailLabel\" >Account </div>');
    $('#LabelHeaderAccount').append("<div id=\"RADUserManagementAccountDetailsParent\" class=\"RADUserManagementAccountDetails\"></div>");
    $('#RADUserManagementAccountDetailsParent').append("<div id=\"RADUserManagementAccountDetails\" class=\"RADUserManagementFirstAccountDiv\">"
        + currentUserdetails.Accounts[0] + "</div>");
    $("#RADUserManagementAccountDetailsParent").append("<div class=\"fa fa-caret-down downarrowdropdown\"></div>");

    for (var i = 0; i < userEditInfo.AccountsEditInfo.length; i++) {
        $('#RADUserManagementAccountDetailsParent').append("<div class=\"RADUserManagementEachAccountDiv\">" + userEditInfo.AccountsEditInfo[i].AccountName + "</div>");
    }

    $('.RADUserManagaementLabel').remove();
    $("#FirstName").val($(".RADTileActive").children()[1].innerText.replace("(", "")).addClass('RADSelectedHtml');
    $("#LastName").val($(".RADTileActive").children()[2].innerText.replace(")", "")).addClass('RADSelectedHtml');
    $("#Email").val($(".RADTileActive").children()[3].innerText).addClass('RADSelectedHtml');
}
    

AllUserUsers.prototype.BindControlsonEditLdapMode = function (currentUserdetails, userEditInfo) {

    $(".umRightHeader").empty();
    $(".umRightHeader").append("<div id=\"RADUserManagaementUserLabel\" class=\"RADUserManagaementLabel\"><label id=\"lblUserLabel\" >User :</label></div>");    
    $(".umRightHeader").append("<div id=\"lab\" class=\"RADUserMgmtuser_label\">" + currentUserdetails.UserLoginName + "</div>");
    $("#lab").attr('title', currentUserdetails.UserLoginName);
    $(".umRightHeader").append("<div id=\"RADUserManagaementAccountLabeDiv\" class=\"RADUserManagaementLabel\"><label id=\"lblAccountLabel\" >Account :</label></div>");
    $(".umRightHeader").append("<div id=\"lab1\" class=\"RADUSERManagementAccountValue\"></div>");
    $('.RADUSERManagementAccountValue').append("<div id=\"RADUserManagementAccountDetailsParent\" class=\"RADUserManagementAccountDetails\"></div>");

    $('.RADSCreenSearchParent').remove();
    $('.RADSCreenSearchParentSec2').remove();

    $("<div id=\"RADSCreenSearchParent\"  class=\"RADSCreenSearchParent\"></div>").insertAfter($('.umSec1Head'));

     var ArrayOfActiveAccounts =[];

     for (var i = 0; i < userEditInfo.AccountsEditInfo.length; i++) {
            var obj = { };
            obj["id"]= i;
            obj["text"] = userEditInfo.AccountsEditInfo[i].AccountName;
            ArrayOfActiveAccounts.push(obj);
    }

    var dropDownObject = {};
    dropDownObject["radEntitydefaultValue"] = 'Accounts';
    dropDownObject["radEntitydropDownAllData"] = ArrayOfActiveAccounts;
    dropDownObject["radEntityEnableMultiSelect"] = false
    dropDownObject["radEntityAttributeNameText"] = currentUserdetails.Accounts[0]+ '|^';
    dropDownObject["radWidgetSelectHandler"] = AllUserUsers.prototype.GetGroupsForAccount;
    
    $("#lab1").empty();
    $("#lab1").SelectDropDown(dropDownObject);
    //$('#RADUserManagementAccountDetailsParent').append("<div id=\"RADUserManagementAccountDetails\" class=\"RADUserManagementFirstAccountDiv\">"
    //    + currentUserdetails.Accounts[0] + "</div>");

    //$("#RADUserManagementAccountDetailsParent").append("<div class=\"fa fa-caret-down downarrowdropdown\"></div>");
    //for (var i = 0; i < userEditInfo.AccountsEditInfo.length; i++) {
    //    $('#RADUserManagementAccountDetailsParent').append("<div class=\"RADUserManagementEachAccountDiv\">" + userEditInfo.AccountsEditInfo[i].AccountName + "</div>");
    //}


}

AllUserUsers.prototype.GetGroupsForSelectedAccountName = function (AccountName, currentUserdetails) {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetGroupsForAccount', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ accountName: AccountName })
    }).then(function (responseText) {
         //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var groupNamesForSelectedAccount = JSON.parse(responseText.d);
        $('.umSec1Body').empty();

        // first bind the selected groups for user 
        for (var i = 0; i < currentUserdetails.Groups.length; i++) {
            $('.umSec1Body').append('<div data-toggle = "tooltip" data=\"'
                + currentUserdetails.Groups[i] + '\" id="Groups' + i + '" class=\"RADEditBodyContent">'
                + currentUserdetails.Groups[i] + '</div>');
            $($(".umSec1Body").children()[i]).addClass('TextSelected');
        }

        // Then bind the all groups for user for current account of user
        for (var i = 0; i < groupNamesForSelectedAccount.length; i++) {
            if (currentUserdetails.Groups.indexOf(groupNamesForSelectedAccount[i]) == -1) {
                $('.umSec1Body').append('<div data-toggle = "tooltip" data=\"' + groupNamesForSelectedAccount[i]
                    + '\" id="GroupBody' + i + '" class=\"RADEditBodyContent">' + groupNamesForSelectedAccount[i] + '</div>');
            }
        }
       
    });
}

AllUserUsers.prototype.ShowAccountsDropDown = function () {
    $("#RADUserManagementAccountDetailsParent").css({ "height": "auto" });
}

AllUserUsers.prototype.SelectAccountsOptions = function (event) {
    $(".RADUserManagementFirstAccountDiv").html($(event.target).text());
    $("#RADUserManagementAccountDetailsParent").css({ "height": "25px" });
}

AllUserUsers.prototype.GetAllGroups = function (userName) {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllGroups',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var GroupNames = [];
        var res;
        res = JSON.parse(responseText.d);
        AllUserUsers.instance.AllGroupList = res;
        $('.umSec2Body').append('<div class=\"GroupAssociatedRoles\"><\div>')
        $('.umSec2Body').append('<div class=\"UserAssociatedRoles\"><\div>')
        $('.umSec2Body').append('<div class=\"AllEditableRoles\"><\div>')
        AllUserUsers.prototype.GetSelectedGroups(userName, GroupNames);
      
    });
}


AllUserUsers.prototype.GetSelectedGroups = function (userName, GroupNames) {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var groupname;
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetUserDetail',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ userLoginName: userName })
    }).then(function (responseText) {
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var res;
        res = JSON.parse(responseText.d);
        var SelectedGroupsArray = [];
        var key;
        var size = 0;
        var f = 0
        var j = 0

        SelectedGroupsArray = res["Groups"]
        AllUserUsers.instance.SelectedGroupsArray = res["Groups"];
        AllUserUsers.instance.FirstName = res["FirstName"];
        AllUserUsers.instance.LastName = res["LastName"];
        AllUserUsers.instance.MiddleName = res["MiddleName"];
        AllUserUsers.instance.Email = res["EmailId"];
        for (var i = 0; i < AllUserUsers.instance.SelectedGroupsArray.length; i++) {

            $('.umSec1Body').append('<div data-toggle = "tooltip" data=\"' + AllUserUsers.instance.SelectedGroupsArray[i] + '\" id="Groups' + i + '" class=\"RADEditBodyContent">' + AllUserUsers.instance.SelectedGroupsArray[i] + '</div>');

            $($(".umSec1Body").children()[i]).addClass('TextSelected');

        }
        for (var i = 0; i < AllUserUsers.instance.AllGroupList.length; i++) {

            if (AllUserUsers.instance.SelectedGroupsArray.indexOf(AllUserUsers.instance.AllGroupList[i]) == -1) {
                $('.umSec1Body').append('<div data-toggle = "tooltip" data=\"' + AllUserUsers.instance.AllGroupList[i] + '\" id="GroupBody' + i + '" class=\"RADEditBodyContent">' + AllUserUsers.instance.AllGroupList[i] + '</div>');
            }
        }
        $(".RADEditBodyContent").unbind().click(function (event) {
            if ($(event.target).hasClass("TextSelected") == true) {
                $(event.target).switchClass("TextSelected", "RADEditBodyContent")
                var groupname = $(event.target).text()
                var a = []
                var count = 0
                a.push(AllUserUsers.instance.AllGroupCorrespondingRoles[groupname])
                for (var i = 0; i < a[0].length; i++) {
                    if (ArrayOfRoles.indexOf(a[0][i]) != -1) {
                        ArrayOfRoles.splice(i, 1)
                    }
                }
                AllUserUsers.instance.SelectedGroups.splice(AllUserUsers.instance.SelectedGroups.indexOf($(this).text()), 1);
                for (var k = 0; k < a[0].length; k++) {
                    if (ArrayOfRoles.indexOf(a[0][k]) != -1) {
                        if ($(".umSec2Body").find('div[data="' + a[0][k] + '"]')) {
                            var roleText = $(".umSec2Body").find('div[data="' + a[0][k] + '"]').html();
                            $(".umSec2Body").find('div[data="' + a[0][k] + '"]').removeClass("RADGrouoInfobodycontent")
                            $(".umSec2Body").find('div[data="' + a[0][k] + '"]').addClass("RADEditBodyContentRoles")
                        }
                    }
                }
                AllUserUsers.instance.SelectedCorrespondingRolesSelected.splice(AllUserUsers.instance.SelectedCorrespondingRolesSelected.indexOf(roleText), 1);
            }
            else {
                $(event.target).addClass('TextSelected');
                var groupname = $(event.target).text()
                var a = []
                a.push(AllUserUsers.instance.AllGroupCorrespondingRoles[groupname])
                AllUserUsers.instance.SelectedGroups.splice(AllUserUsers.instance.SelectedGroups.indexOf($(this).text()), 1);
                for (var k = 0; k < a[0].length; k++) {
                    if ($(".umSec2Body").find('div[data="' + a[0][k] + '"]')) {
                        var roleText = $(".umSec2Body").find('div[data="' + a[0][k] + '"]').html();
                        $(".umSec2Body").find('div[data="' + a[0][k] + '"]').removeClass("RADEditBodyContentRoles")
                        $(".umSec2Body").find('div[data="' + a[0][k] + '"]').addClass("RADGrouoInfobodycontent")
                    }
                }
                AllUserUsers.instance.SelectedCorrespondingRolesSelected.push(roleText);
                AllUserUsers.instance.SelectedGroups.push($(this).text())
            }
        });

        
    });
    $('[data-toggle="tooltip"]').tooltip();
}
AllUserUsers.prototype.CreateGetAllRoles = function () {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllRoles',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
         //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var res;
        res = JSON.parse(responseText.d);
        AllUserUsers.instance.AllRoleList = res
        $('.umSec2Body').empty();
        for (var i = 0; i < res.length; i++) {
            $('.umSec2Body').append('<div data-toggle ="tooltip" data=\"' + res[i].RoleName + '\" id="Roles" class=\"RADEditBodyContentRolesForCreate">' + res[i].RoleName + '</div>');
            $("#d" + i).addClass('RADUserMgmtbodycontent');
        }
        $(".RADEditBodyContentRolesForCreate").unbind().click(function (event) {
            if ($(this).hasClass("TextSelected") == true) {
                $(this).removeClass("TextSelected")
                $(this).addClass("RADEditBodyContentRolesForCreate")
                if (AllUserUsers.instance.GetSelectedRolesForAccountInCreateSearch.indexOf(event.target.innerText) != -1) {
                    var index = AllUserUsers.instance.GetSelectedRolesForAccountInCreateSearch.indexOf(event.target.innerText);
                    AllUserUsers.instance.GetSelectedRolesForAccountInCreateSearch.splice(index, 1);
                }
            }
            else {
                $(this).addClass('TextSelected')
                $(this).removeClass("RADEditBodyContentRolesForCreate")
                if (AllUserUsers.instance.GetSelectedRolesForAccountInCreateSearch.indexOf(event.target.innerText) == -1) {
                    AllUserUsers.instance.GetSelectedRolesForAccountInCreateSearch.push(event.target.innerText);

                }
            }
        });
        $(".RADUserMgmtbodycontent").unbind().click(function (event) {
            if ($(this).hasClass("TextSelected") == true) {
                $(this).removeClass("TextSelected")
                $(this).addClass("RADEditBodyContentRolesForCreate")
            }
            else {
                $(this).addClass('TextSelected');
                $(this).removeClass("RADEditBodyContentRolesForCreate")
            }
        });
       
    });
}
AllUserUsers.prototype.GetAllRoles = function () {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllRoles',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var res;
        res = JSON.parse(responseText.d);
        AllUserUsers.instance.AllRoleList = res
        AllUserUsers.prototype.GetSelectedRoles(userName)
    });
}

AllUserUsers.prototype.GetSelectedRoles = function (userName) {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetUserDetail',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ userLoginName: userName })
    }).then(function (responseText) {

        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var res;
        res = JSON.parse(responseText.d);
        var SelectedRolesArray = [];
        var EditresultsView = [];
        var UniqueRoles = [];
        SelectedRolesArray = res["Roles"];
        var j = 0
        $('.GroupAssociatedRoles').empty()
        var arr = []

        for (var i = 0; i < res.Groups.length; i++) {
            var a = [];
            for (var item in AllUserUsers.instance.AllGroupCorrespondingRoles[res.Groups[i]]) {
                if (a.indexOf(AllUserUsers.instance.AllGroupCorrespondingRoles[res.Groups[i]][item]) == -1)
                    a.push(AllUserUsers.instance.AllGroupCorrespondingRoles[res.Groups[i]][item])
            }
            for (var j = 0; j < a.length; j++) {
                if (UniqueRoles.indexOf(a[j]) == -1) {
                    UniqueRoles.push(a[j])
                }

            }
        }

        AllUserUsers.instance.GroupValueForDictionary = res.Groups;

        for (k = 0; k < UniqueRoles.length; k++) {
            $('.GroupAssociatedRoles').append('<div data-toggle = "tooltip" data=\"' + UniqueRoles[k] + '\" id="GetGroupInfo' + j + '" class=\"RADGrouoInfobodycontent">' + UniqueRoles[k] + '</div>');
            j++
        }

        for (var i = 0; i < $(".umSec2Body").find(".RADGrouoInfobodycontent").length; i++) {
            arr.push($(".umSec2Body").find(".RADGrouoInfobodycontent")[i].innerHTML)
        }

        AllUserUsers.instance.SelectedCorrespondingRolesSelected = arr;
        for (var i = 0; i < SelectedRolesArray.length; i++) {
            if (UniqueRoles.indexOf(SelectedRolesArray[i]) == -1) {
                $('.UserAssociatedRoles').append('<div data-toggle = "tooltip" data=\"' + SelectedRolesArray[i] + '\" id="RolesSelected' + i + '" class=\"RADEditBodyContentRoles">' + SelectedRolesArray[i] + '</div>');
                $("#RolesSelected" + i).addClass('TextSelected');
            }
        }

        AllUserUsers.instance.SelectedRolesArray = SelectedRolesArray;
        for (var i = 0; i < AllUserUsers.instance.AllRoleList.length; i++) {

            if (SelectedRolesArray.indexOf(AllUserUsers.instance.AllRoleList[i].RoleName) == -1 && UniqueRoles.indexOf(AllUserUsers.instance.AllRoleList[i].RoleName) == -1) {
                $('.AllEditableRoles').append('<div data-toggle = "tooltip" data=\"' + AllUserUsers.instance.AllRoleList[i].RoleName + '\" id="Roles' + i + '" class=\"RADEditBodyContentRoles">' + AllUserUsers.instance.AllRoleList[i].RoleName + '</div>');

            }
        }

        $(".RADEditBodyContentRoles").unbind().click(function (event) {
            if ($(this).hasClass("TextSelected") == true) {
                {
                    $(this).switchClass("TextSelected", "RADEditBodyContentRoles");
                    AllUserUsers.instance.SelectedRoles.splice(AllUserUsers.instance.SelectedRoles.indexOf($(this).text()), 1);
                }
            }
            else {
                $(this).addClass('TextSelected')
                AllUserUsers.instance.SelectedRoles.push($(this).text())
            }
        });

        $(".RADUserMgmtbodycontent").unbind().click(function (event) {
            if ($(this).hasClass("TextSelected") == true) {
                $(this).switchClass("TextSelected", "RADEditBodyContentRoles")
                AllUserUsers.instance.SelectedRoles.splice(AllUserUsers.instance.SelectedRoles.indexOf($(this).text()), 1);
            }
            else {
                $(this).addClass('TextSelected');
                AllUserUsers.instance.SelectedRoles.push($(this).text())
            }
        });

    });

}

AllUserUsers.prototype.CreateUserDetails = function () {
    var createUserDetail = new Object();
    var Groups = [];
    var Roles = [];
    var ArrGroups = [];
    var ArrRoles = [];
    var Accounts = [];
    createUserDetail.FirstName = $("#CreateFirstName").val().replace("(", "")
    createUserDetail.LastName = $("#CreateLastName").val().replace(")", "")
    createUserDetail.EmailId = $("#CreateEmail").val()
    createUserDetail.UserLoginName = $("#CreateUserName").val()
    var UserLoginName = $("#CreateUserName").val()
    Accounts.push($(".RADUserManagementFirstAccountDiv").text())
    createUserDetail.Accounts = $(".RADUserManagementFirstAccountDiv").text()
    ArrGroups = ($(".umSec1Body").find(".TextSelected"))
    ArrRoles = ($(".umSec2Body").find(".TextSelected"));
    $(ArrGroups).each(function (i, e) {
        Groups.push($(e).text());
    })

    $(ArrRoles).each(function (i, e) {
        Roles.push($(e).text());
    })
    createUserDetail.Accounts = Accounts;
    createUserDetail.Roles = Roles;
    createUserDetail.Groups = Groups;
    var myString = JSON.stringify(createUserDetail);

    if (createUserDetail.UserLoginName == "" || createUserDetail.FirstName == "" || createUserDetail.LastName == "" || createUserDetail.EmailId == "" || (createUserDetail.Groups == 0 && createUserDetail.Roles == 0)) {
        if ($(".RADUserManagementEditButton").val() == "Save") {
            if (createUserDetail.UserLoginName == "" || ($(".RADUserMgmtLabelHeader").find(".RADUserNameValidation") == 0)) {
                $("<div id=\"RADUserNameValidation\" class=\"RADUserNameValidation\">!</div>").insertAfter($("#CreateUserName"))
                $("#RADUserNameValidation").attr("title", "UserName Can't Be Empty")
            }
            if (createUserDetail.FirstName == "" || ($(".RADUserMgmtLabelHeader").find(".RADFirstNameValidation") == 0)) {
                $("<div id=\"RADFirstNameValidation\" class=\"RADFirstNameValidation\">!</div>").insertAfter($("#CreateFirstName"))
                $("#RADFirstNameValidation").attr("title", "FirstName Can't Be Empty")
            }
            if (createUserDetail.LastName == "" || ($(".RADUserMgmtLabelHeader").find(".RADLastNameValidation") == 0)) {
                $("<div id=\"RADLastNameValidation\" class=\"RADLastNameValidation\">!</div>").insertAfter($("#CreateLastName"))
                $("#RADLastNameValidation").attr("title", "LastName Can't Be Empty")
            }
            if (createUserDetail.EmailId == "" || ($(".RADUserMgmtLabelHeader").find(".RADEmailValidation") == 0)) {
                $("<div id=\"RADEmailValidation\" class=\"RADEmailValidation\">!</div>").insertAfter($("#CreateEmail"))
                $("#RADEmailValidation").attr("title", "Email Can't Be Empty")
            }
            if (createUserDetail.Accounts == "RADUserMgmtAccounts" || ($(".RADUserMgmtLabelHeader").find(".RADAccountValidation") == 0)) {
                $("<div id=\"RADAccountValidation\" class=\"RADAccountValidation\">!</div>").insertAfter($("#RADUserManagementAccountDetailsParent"))
                $("#RADAccountValidation").attr("title", "Select Atleast One Account")
            }
            if (createUserDetail.Groups == 0 && createUserDetail.Roles == 0 || ($(".umSec1Head").find(".RADUserManagementAccountGroupValidation") == 0)) {
                $("<div  class=\"RADUserManagementAccountGroupValidation\">!</div>").insertAfter($(".umSec1Head"));
                $("<div  class=\"RADUserManagementAccountGroupValidation\">!</div>").insertAfter($(".umSec2Head"));
                $(".RADUserManagementAccountGroupValidation").attr("title", "Select Atleast One Group Or Role")
            }
        }
    }
    else {
        $(".RADUserNameValidation").remove();
        $(".RADFirstNameValidation").remove();
        $(".RADLastNameValidation").remove();
        $(".RADEmailValidation").remove();
        $(".RADAccountValidation").remove();
        $(".RADUserManagementAccountGroupValidation").remove();
        AllUserUsers.prototype.CreateUser(myString, UserLoginName);
    }
}




AllUserUsers.prototype.CreateUser = function (CreateUserDetail, UserLoginName) {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/CreateUser',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ userDetails: CreateUserDetail })

    }).then(function (responseText) {
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var rolename = $("#CreateUserName").val()
        if (responseText.d != "") {
            $(".RADUserManagementCancelButton").val("Delete")
            $(".RADUserManagementCancelButton").attr('id', 'RADUserManagementDeleteButton')

            //checking for privileges
            AllUserUsers.instance.ShowHideByPrivilegeType("Delete");

            $(".umRightHeader").empty();
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserFailure\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Result</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">User Has Been Created Successfully</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 2700);
            AllUserUsers.instance.ReCreateDiv(rolename)
        }

        else {
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperSuccessDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserSuccess\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Result</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">User Creation Failed</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 2700);
            AllUserUsers.instance.GetAllUsersCreate(rolename)
        }
        
    });
}

AllUserUsers.prototype.ReCreateDiv = function (rolename) {

    $(".umRightHeader").css({ "width": "60%" })
    $(".UMRightHeaderEdit").css({ "width": "40%" })

    $("#RADUserManagementResetButtonPassword").show()
    var UniqueRoles = [];
    if ($("#RADUserManagementEditButton").attr("value") == "Save")
        $("#RADUserManagementEditButton").attr("value", "Edit");
    //Check for privileges
    AllUserUsers.instance.ShowHideByPrivilegeType("Edit")

    if ($("#RADUserManagementAddNewGroupName") != null)
        $("#RADUserManagementAddNewGroupName").remove();
    if ($("#RADUserManagementAddNewGroupDesc") != null)
        $("#RADUserManagementAddNewGroupDesc").remove();

    $(".umRightHeader").empty();
    $('.umSec2Body').empty()
    $("#CreateEmail").remove()

    $("#RADUMGroupSearch").remove()
    $("#RADUMRoleSearch").remove()
    $("#Groupsearch_text").remove()
    $("#Rolesearch_text").remove()
    $("#RADUserGroupNameValidation").remove()
    $("#RADGroupDesceValidation").remove()
    $("#RADAccountValidation").remove()
    $("#RADUserManagaementRoleScreenUsersSSS").remove()
    $("#RADUserManagaementRoleScreenGroupsSSS").remove()
    $("#RADUserNameValidation").remove()
    $("#RADLastNameValidation").remove()
    $("#RADEmailValidation").remove()
    $("#RADEmailValidation").remove()
    $("#RADFirstNameValidation").remove()
    $("#CreateLastName").remove()
    $("#Email").remove()
    AllUserUsers.instance.AfterPopUpDetails()
}
AllUserUsers.prototype.GetAllUsersCreate = function (rolename) {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllUsers',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'

    }).then(function (responseText) {
         //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var b;
        b = JSON.parse(responseText.d);
        AllUserUsers.instance.AllUsersListCreate = b;
        var firstUser;
        var Editfirstname
        var Editlastname
        var EditEmailId

        $(".umRightHeader").css({ "width": "60%" })
        $(".UMRightHeaderEdit").css({ "width": "40%" })

        $("#RADUserManagementResetButtonPassword").show()
        $(".umDynamicTiles").find(".RADTileActive").remove()
        $(".umDynamicTiles").empty()
        for (var j = 0; j < b.length; j++) {
            if (b[j].UserLoginName == rolename) {
                firstUser = b[j].UserLoginName
                $(".umDynamicTiles").prepend('<div title="' + b[j].UserLoginName + '" id="tADD' + j + '" name="' + b[j].UserLoginName + '"></div>');
                $("#tADD" + j).addClass('RADUserMgmttiles').addClass('RADTileActive');
                $("#tADD" + j).closest(".RADUserMgmttiles").append('<div id="arr"></div>');
                $(".umDynamicTiles").find(".RADarrow-right").remove();
                $("#arr").addClass('fa fa-caret-right RADarrow-right').insertAfter("#tADD" + j);
                $("#tADD" + j).append('<div title="' + b[j].UserLoginName + '" id="user' + j + '" name="' + b[j].UserLoginName + '" FirstName="' + b[j].FirstName + '" LastName="' + b[j].LastName + '" EmailId="' + b[j].EmailId + '">' + b[j].UserLoginName + '</div>');
                $("#user" + j).addClass('RADUserMgmtuser_name');
                var first = b[j].FirstName
                $("#tADD" + j).append('<div title="' + b[j].FirstName + '" id="firstADD' + j + '" FirstName="' + b[j].FirstName + '">' + '(' + b[j].FirstName + " " + '</div>');
                $("#firstADD" + j).addClass('RADUserMgmtfirst_name');
                $("#tADD" + j).append('<div title="' + b[j].LastName + '"id="lastADD' + j + '" LastName="' + b[j].LastName + '">' + b[j].LastName + ')' + '</div>');
                $("#lastADD" + j).addClass('RADUserMgmtlast_name');
                if (b[j].title != null && b[j].title != "") {
                    $("#tADD" + j).append('<div title="' + b[j].title + '"id="titleADD' + j + '" Title="' + b[j].title + '">' + b[j].title + '</div>');
                    $("#titleADD" + j).addClass('RADUserMgmtemail_id');
                }
                $("#tADD" + j).append('<div title="' + b[j].EmailId + '" id="emailADD' + j + '" EmailId="' + b[j].EmailId + '">' + b[j].EmailId + '</div>');
                $("#emailADD" + j).addClass('RADUserMgmtemail_id');
                $("#FirstName").val(b[j].UserLoginName);
                $("#LastName").val(b[j].FirstName);
                $("#Email").val(b[j].EmailId);
            }
            else {
                $('.umDynamicTiles').append('<div title="' + b[j].FirstName + '" id="t' + j + '" name="' + b[j].UserLoginName + '" FirstName="' + b[j].FirstName + '" LastName="' + b[j].LastName + '" EmailId="' + b[j].EmailId + '"></div>');
                $("#t" + j).addClass('RADUserMgmttiles');
                $("#t" + j).append('<div title="' + b[j].UserLoginName + '" id="user' + j + '">' + b[j].UserLoginName + '</div>');
                $("#user" + j).addClass('RADUserMgmtuser_name');
                $("#t" + j).append('<div title="' + b[j].FirstName + '" id="first' + j + '">' + '(' + b[j].FirstName + " " + '</div>');
                $("#first" + j).addClass('RADUserMgmtfirst_name');
                $("#t" + j).append('<div title="' + b[j].LastName + '" id="last' + j + '">' + b[j].LastName + ')' + '</div>');
                $("#last" + j).addClass('RADUserMgmtlast_name');
                if (b[j].title != null && b[j].title != "") {
                    $("#t" + j).append('<div title="' + b[j].title + '"id="title' + j + '">' + b[j].title + '</div>');
                    $("#title" + j).addClass('RADUserMgmtemail_id');
                }
                $("#t" + j).append('<div title="' + b[j].EmailId + '"id="email' + j + '">' + b[j].EmailId + '</div>');
                $("#email" + j).addClass('RADUserMgmtemail_id');
            }
        }
        $("#RADUserGroupNameValidation").remove()
        $("#RADGroupDesceValidation").remove()
        $("#RADAccountValidation").remove()
        $("#RADUserManagaementRoleScreenUsersSSS").remove()
        $("#RADUserManagaementRoleScreenGroupsSSS").remove()
        $("#RADUserNameValidation").remove()
        $("#RADLastNameValidation").remove()
        $("#RADEmailValidation").remove()
        $("#RADEmailValidation").remove()
        $("#RADFirstNameValidation").remove()
        $("#CreateLastName").remove()
        $("#CreateUserName").remove()
        $("#CreateFirstName").remove()
        $("#CreateEmail").remove()
        $("#RADUserManagementAccountDetailsParent").remove()
        $("#Email").remove()
        $('.RADSCreenSearchParent').remove();
        $('.RADSCreenSearchParentSec2').remove();
        $("#RADUMGroupSearch").remove()
        $("#RADUMRoleSearch").remove()
        $("#Groupsearch_text").remove()
        $("#Rolesearch_text").remove()
        $("#FirstName").val($(this).attr('FirstName'));
        $("#LastName").val($(this).attr('LastName'));
        $("#Email").val($(this).attr('EmailId'));
        $(".umRightHeader").empty();

        $(".umRightHeader").append("<div id=\"RADUserManagaementUserLabel\" class=\"RADUserManagaementLabel\"><label id=\"lblUserLabel\" >User :</label></div>");

        AfterPopUpUSerName = $(".RADTileActive").children()[0].innerText;
        AllUserUsers.instance.GetUserInfoAfterpopUp(AfterPopUpUSerName)
        $("#RADUserManagementSaveButton").val("Edit")
        $("#RADUserManagementSaveButton").attr('id', 'RADUserManagementEditButton');
        $("#RADUserManagementCreateSaveButton").val("Edit")
        $("#RADUserManagementCreateSaveButton").attr('id', 'RADUserManagementEditButton');
        $("#RADUserManagementCreateActiveDirectorySaveButton").val("Edit")
        $("#RADUserManagementCreateActiveDirectorySaveButton").attr('id', 'RADUserManagementEditButton')
        $("#RADUserManagementCreateSaveButton").val("Edit")
        $('#RADUserManagementCancelButton').val("Delete");
        $('#RADUserManagementCancelButton').attr('id', 'RADUserManagementDeleteButton')

        //checking for privileges
        AllUserUsers.instance.ShowHideByPrivilegeType("Edit");
        AllUserUsers.instance.ShowHideByPrivilegeType("Delete");

        $(".RADUserManagementAccountGroupValidation").remove();
        $("#RADUserMgmtLabelHeader").remove()
        AllUserUsers.prototype.GetUserInfo(firstUser);
        $(".RADUserMgmttiles").unbind().click(function (event) {
            AllUserUsers.instance.ClickOnCreateTiles(event);
        });
       
    });
}
AllUserUsers.prototype.AuditScreen = function (event) {
    var auditfirstname = $(".RADTileActive").children()[0].innerText
    $(".usermgmt").prepend("<div class=\"RADAuditScreenMain\" id=\"RADAuditScreenMain\"></div>");

    $("#RADAuditScreenMain").append("<div class=\"RADAuditHeader\" id=\"RADAuditHeader\"></div>")
    $("#RADAuditHeader").append("<div class=\"RADAuditHeaderHeading\" id=\"RADAuditHeaderHeading\">User Audit</div>")
    $("#RADAuditHeader").append("<div class=\"RADAuditCancelButton\" id=\"RADAuditCancelButton\">X</div>")
    $("#RADAuditScreenMain").append("<div class=\"RADAuditHeaderHeadingOfUserName\" id=\"RADAuditHeaderHeadingOfUserName\">User Name :</div>")
    $("#RADAuditHeaderHeadingOfUserName").append("<div class=\"RADAuditHeaderValueOfUserName\" id=\"RADAuditHeaderValueOfUserName\">" + auditfirstname + "</div>")
    $("#RADAuditScreenMain").append("<div class=\"RADBodyOfAudit\" id=\"RADNeoUserAuditScreen\"></div>")

    var effect = 'slide';
    var options = { direction: 'Right' };
    var duration = 500;
    $("#RADAuditScreenMain").toggle(effect, options, duration);
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAuditDetailsForUser', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ username: auditfirstname })
    }).then(function (responseText) {
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var response = JSON.parse(responseText.d);
        $("#RADNeoUserAuditScreen").append("<table id=\"RADUserAuditTable\" class=\"RADUserAuditTable\"><tbody><tr class=\"RADAuditUserHeaderRow\"><td>Attribute</td><td>Old Value</td><td>New Value</td><td>Updated By</td><td>Updated On</td></tr></tbody></table>")
        for (var i = 0; i < response.length; i++) {
            $("#RADUserAuditTable").append("<tr class=\"RADUserAuditDataRow\"><td title=" + response[i]["Attribute Name"] + ">" + response[i]["Attribute Name"] + "</td><td title=" + response[i]["Old Value"] + ">" + response[i]["Old Value"] + "</td><td title=" + response[i]["New Value"] + ">" + response[i]["New Value"] + "</td><td title=" + response[i]["Updated By"] + ">" + response[i]["Updated By"] + "</td><td>" + moment(response[i]["Updated On"]).format("YYYY/MM/DD HH:mm") + "</td></tr>")
        }
        
    });

    $("#RADAuditCancelButton").unbind().click(function (event) {
        AllUserUsers.instance.AuditScreenCancel(event)
    });
}

AllUserUsers.prototype.AuditScreenCancel = function (event) {
    $("#RADAuditScreenMain").hide("drop", { direction: "right" }, "slow", function () { $("#RADAuditScreenMain").remove(); });

}

AllUserUsers.prototype.SetGridHeight = function (eventtype, id) {
    $("#" + id + "_body_Div").height($("#RADNeoUserAuditScreen").height() - $("#" + id + "_upperHeader_Div").height() - $("#" + id + "_footer_Div").height())
    $("#" + id + "_configutation").css({ "display": "none" });
    $("#" + id + "_upperHeader_Div").css({ "display": "none" });

}

AllUserUsers.prototype.SearchAction = function (event) {
    $("#search_text").css({ 'display': 'block' })
}

AllUserUsers.prototype.ResetPasswordButton = function (UserNameForPasswordReset) {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/ResetPassword',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ userName: UserNameForPasswordReset })

    }).then(function (responseText) {
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        if (responseText.d == true) {
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperSuccessDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserSuccess\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Success</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Password has been reset successfully</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 6000);
        }
        else {
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserFailure\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Result</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Password reset Failed</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 6000);
        }
        
    });
}

AllUserUsers.prototype.GetAllUsersModify = function (TilesModify) {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllUsers',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'

    }).then(function (responseText) {
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var b;
        b = JSON.parse(responseText.d);
        AllUserUsers.instance.AllUsersListModify = b;
        var firstUser;
        var Editfirstname
        var Editlastname
        var EditEmailId
        $(".umDynamicTiles").find(".RADTileActive").remove()
        $(".umDynamicTiles").empty()
        for (var j = 0; j < b.length; j++) {
            if (b[j].UserLoginName == TilesModify) {
                firstUser = b[j].UserLoginName

                $(".umDynamicTiles").prepend('<div title="' + b[j].UserLoginName + '" id="tADD' + j + '" name="' + b[j].UserLoginName + '"></div>');
                $("#tADD" + j).addClass('RADUserMgmttiles').addClass('RADTileActive');
                $("#tADD" + j).closest(".RADUserMgmttiles").append('<div id="arr"></div>');
                $(".umDynamicTiles").find(".RADarrow-right").remove();
                $("#arr").addClass('fa fa-caret-right RADarrow-right').insertAfter("#tADD" + j);
                $("#tADD" + j).append('<div title="' + b[j].UserLoginName + '" id="user' + j + '" name="' + b[j].UserLoginName + '" FirstName="' + b[j].FirstName + '" LastName="' + b[j].LastName + '" EmailId="' + b[j].EmailId + '">' + b[j].UserLoginName + '</div>');
                $("#user" + j).addClass('RADUserMgmtuser_name');
                var first = b[j].FirstName
                $("#tADD" + j).append('<div title="' + b[j].FirstName + '" id="firstADD' + j + '" FirstName="' + b[j].FirstName + '">' + '(' + b[j].FirstName + " " + '</div>');
                $("#firstADD" + j).addClass('RADUserMgmtfirst_name');
                $("#tADD" + j).append('<div title="' + b[j].LastName + '"id="lastADD' + j + '" LastName="' + b[j].LastName + '">' + b[j].LastName + ')' + '</div>');
                $("#lastADD" + j).addClass('RADUserMgmtlast_name');
                if (b[j].title != null && b[j].title != "") {
                    $("#tADD" + j).append('<div title="' + b[j].title + '"id="titleADD' + j + '" Title="' + b[j].title + '">' + b[j].title + '</div>');
                    $("#titleADD" + j).addClass('RADUserMgmtemail_id');
                }
                $("#tADD" + j).append('<div title="' + b[j].EmailId + '" id="emailADD' + j + '" EmailId="' + b[j].EmailId + '">' + b[j].EmailId + '</div>');
                $("#emailADD" + j).addClass('RADUserMgmtemail_id');
                $("#FirstName").val(b[j].UserLoginName);
                $("#LastName").val(b[j].FirstName);
                $("#Email").val(b[j].EmailId);
            }
            else {

                $('.umDynamicTiles').append('<div title="' + b[j].FirstName + '" id="t' + j + '" name="' + b[j].UserLoginName + '" FirstName="' + b[j].FirstName + '" LastName="' + b[j].LastName + '" EmailId="' + b[j].EmailId + '"></div>');
                $("#t" + j).addClass('RADUserMgmttiles');
                $("#t" + j).append('<div title="' + b[j].UserLoginName + '" id="user' + j + '">' + b[j].UserLoginName + '</div>');
                $("#user" + j).addClass('RADUserMgmtuser_name');
                $("#t" + j).append('<div title="' + b[j].FirstName + '" id="first' + j + '">' + '(' + b[j].FirstName + " " + '</div>');
                $("#first" + j).addClass('RADUserMgmtfirst_name');
                $("#t" + j).append('<div title="' + b[j].LastName + '" id="last' + j + '">' + b[j].LastName + ')' + '</div>');
                $("#last" + j).addClass('RADUserMgmtlast_name');
                if (b[j].title != null && b[j].title != "") {
                    $("#t" + j).append('<div title="' + b[j].title + '"id="title' + j + '" Title="' + b[j].title + '">' + b[j].title + '</div>');
                    $("#title" + j).addClass('RADUserMgmtemail_id');
                }
                $("#t" + j).append('<div title="' + b[j].EmailId + '"id="email' + j + '">' + b[j].EmailId + '</div>');
                $("#email" + j).addClass('RADUserMgmtemail_id');
            }
        }

        
        $(".RADUserMgmttiles").unbind().click(function (event) {
            AllUserUsers.instance.ClickOnCreateTiles(event);
        });
    });
}


AllUserUsers.prototype.GetGroupsForSelectedAccount = function (selectedAccountName) {
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetGroupsForAccount',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ accountName: selectedAccountName })
    }).then(function (responseText) {
         //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var GroupNames = [];
        var res;
        res = JSON.parse(responseText.d);
        AllUserUsers.instance.AllGroupList = res;
        $('.umSec2Body').append('<div class=\"GroupAssociatedRoles\"><\div>')
        $('.umSec2Body').append('<div class=\"UserAssociatedRoles\"><\div>')
        $('.umSec2Body').append('<div class=\"AllEditableRoles\"><\div>')
        AllUserUsers.prototype.GetSelectedGroups(userName, GroupNames);
       
    });

}

AllUserUsers.prototype.ShowHideByPrivilegeType = function (PrivilegeType) {

    switch (PrivilegeType) {

        case "Add":
            if ((Priviliges.indexOf("Add User") != -1) && (AllUserUsers.instance.isUserCreationAllowed))
                $('.RADUserCreatePlusCircle').show();
            else
                $('.RADUserCreatePlusCircle').hide();
            break;

        case "Edit":
            if (Priviliges.indexOf("Update User") != -1)
                $("#RADUserManagementEditButton").show();
            else
                $("#RADUserManagementEditButton").hide();
            break;

        case "Delete":
            if (Priviliges.indexOf("Delete User") != -1)
                $("#RADUserManagementDeleteButton").show();
            else
                $("#RADUserManagementDeleteButton").hide();
            break;
        default:
            //do nothing
    }
}

AllUserUsers.prototype.getHTML = function (name, Type) {
    var closure = this;
    closure.Names = name;
    closure.modeOfAction = Type;
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetTagTemplates',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ fileName: 'GROUPSNROLES.htm' })
    }).then(function (responseText) {
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        $("#" + RADUserManagement.contentBodyId).append(responseText.d);
        if (closure.modeOfAction == "Groups") {
            $(".RADUserMgmtHeaderName").text("GROUPS");
        }
        else if (closure.modeOfAction == "Roles") {
            $(".RADUserMgmtHeaderName").text("ROLES");
        }
        else {
            $(".RADUserMgmtHeaderName").text("FUNCTIONAL GROUPS");
        }

        closure.GroupNRolesBindings();
        $(".RADUserMgmtRemove").click(function (event) {
            $(".Groups_n_roles").remove();
        })
        $(".searchNewColumnsInmput").keyup(function (event) {
            var collection = $(".RADUserMgmtAllChilds").find(".RADUserMgmtChildParent");
            var length = collection.length;
            var searchText = $(".searchNewColumnsInmput").val();
            for (var i = 0; i < length; i++) {
                if ($(collection[i]).find(".RADUserMgmtChild").html().toLowerCase().indexOf(searchText.toLowerCase()) > -1) {
                    $(collection[i]).removeClass("attributeNameHidden");
                }
                else {
                    $(collection[i]).addClass("attributeNameHidden");
                }
            }
        });
        
    })
}

AllUserUsers.prototype.GroupNRolesBindings = function () {
    var self = this;
    self.groupsList = [];
    self.BuildHtml = function () {
        var closure = this;
        closure.Instances = ko.observableArray([]);
        closure.SelectAll = function (model, event) {
            if ($(event.target).closest(".Groups_n_roles").find(".RADUserMgmtselec").length < $(".RADUserMgmtAllChilds").find(".RADUserMgmtChildParent").length) {
                $(event.target).closest(".Groups_n_roles").find(".RADUserMgmtChild").addClass("RADUserMgmtselec");
                $(event.target).closest(".Groups_n_roles").find(".RADUserMgmtChildParent").find(".RADUserMgmtselected").show();
            }
            else {
                $(event.target).closest(".Groups_n_roles").find(".RADUserMgmtChild").removeClass("RADUserMgmtselec");
                $(event.target).closest(".Groups_n_roles").find(".RADUserMgmtChildParent").find(".RADUserMgmtselected").hide();
            }
            event.stopPropagation();
            event.preventDefault();
        }
        closure.SelectChild = function (model, event) {
            if ($(event.target).closest(".RADUserMgmtChildParent").find(".RADUserMgmtselec").length == 0) {
                $(event.target).closest(".RADUserMgmtChild").addClass("RADUserMgmtselec");
                $(event.target).closest(".RADUserMgmtChildParent").find(".RADUserMgmtselected").show();
            }
            else {
                $(event.target).closest(".RADUserMgmtChild").removeClass("RADUserMgmtselec");
                $(event.target).closest(".RADUserMgmtChildParent").find(".RADUserMgmtselected").hide();
            }
            event.stopPropagation();
            event.preventDefault();
        }
        closure.GetDetails = function (model, event) {

            for (var i = 0; i < $(".Groups_n_roles").find(".RADUserMgmtselec").length; i++) {
                if ($($(".Groups_n_roles").find(".RADUserMgmtselec")[i]).parent().hasClass('attributeNameHidden') == false) {
                    self.groupsList.push($($(".Groups_n_roles").find(".RADUserMgmtselec")[i]).parent().find(".RADUserMgmtChild").text());
                }
            }
            if (self.modeOfAction == "Groups") {
                for (var i = 0; i < self.groupsList.length; i++) {
                    var cnt = 0;
                    for (var j = 0; j < self.Names.length; j++) {
                        if (self.Names[j].GroupName == self.groupsList[i]) {
                            self.groupsList[i] = self.Names[j];
                            cnt++;
                        }
                    }
                }
            }
            var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
            if (self.modeOfAction == "Groups") {
               // AllUserUsers.prototype.AddSpinnerOnContentBody();
                $.ajax({
                    url: url + '/DownLoadGroups',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ groupsList: JSON.stringify(self.groupsList) })
                }).then(function (responseText) {
                     //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
                    $('.Groups_n_roles').animate({
                        opacity: 0, // animate slideUp
                        marginLeft: '-200px'
                    }, 'slow', 'linear', function () {
                        $(this).remove();
                    });
                    window.open("App_Dynamic_Resource/RUserManagement,com.ivp.rad.RUserManagement.RScripts.RADGridExportToExcel.aspx?eventData=" + responseText.d + "", "_blank", "width=400,height=1,menubar=0,resizable=1");

                    event.stopPropagation();
                    event.preventDefault();
                   
                })
            }
            else if (self.modeOfAction == "Roles") {
               // AllUserUsers.prototype.AddSpinnerOnContentBody();
                $.ajax({
                    url: url + '/DownLoadRoles',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ rolesList: JSON.stringify(self.groupsList) })
                }).then(function (responseText) {
                    //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
                    $('.Groups_n_roles').animate({
                        opacity: 0, // animate slideUp
                        marginLeft: '-200px'
                    }, 'slow', 'linear', function () {
                        $(this).remove();
                    });
                    window.open("App_Dynamic_Resource/RUserManagement,com.ivp.rad.RUserManagement.RScripts.RADGridExportToExcel.aspx?eventData=" + responseText.d + "", "_blank", "width=400,height=1,menubar=0,resizable=1");

                    event.stopPropagation();
                    event.preventDefault();
                    

                })
            }
            else {
               // AllUserUsers.prototype.AddSpinnerOnContentBody();
                $.ajax({
                    url: url + '/DownLoadFunctionalGroup',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ groupList: JSON.stringify(self.groupsList) })
                }).then(function (responseText) {
                    //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
                    $('.Groups_n_roles').animate({
                        opacity: 0, // animate slideUp
                        marginLeft: '-200px'
                    }, 'slow', 'linear', function () {
                        $(this).remove();
                    });
                    window.open("App_Dynamic_Resource/RUserManagement,com.ivp.rad.RUserManagement.RScripts.RADGridExportToExcel.aspx?eventData=" + responseText.d + "", "_blank", "width=400,height=1,menubar=0,resizable=1");

                    event.stopPropagation();
                    event.preventDefault();                    
                });
            }
        }

    }
    self.GroupNRolesInfo = new self.BuildHtml();
    for (var i = 0; i < self.Names.length; i++) {
        if (self.modeOfAction == "Groups") {
            var xyz = self.Names[i].GroupName;
        }
        else if (self.modeOfAction == "Roles") {
            var xyz = self.Names[i].RoleName;
        }
        else {
            var xyz = self.Names[i];
        }
        self.GroupNRolesInfo.Instances.push({ InstancesNames: xyz });
    }
    ko.applyBindings(self.GroupNRolesInfo, $("#Groups_n_roles")[0]);
}

AllUserUsers.prototype.getUploadPopUpHTML = function (item) {
    var closure = this;
    closure.flag = item;
   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetTagTemplates',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ fileName: 'UploadGroupsNRoles.htm' })
    }).then(function (responseText) {
           //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        $("#" + RADUserManagement.contentBodyId).append(responseText.d);

        $("#RADUserMgmtuploadFileDoc").css({ 'display': 'inline-block' });
        $("#RADUserMgmtuploadFileDoc").append("<div style=\"text-indent: 8px;\"> </div>");
        $("#RADUserMgmtuploadFileDoc").append("<div class=\"fa fa-upload RADUserMgmtUploadButton\"></div>");
        $("#RADUserMgmtuploadFileDoc").append("<div class=\"RADUserMgmtDropDiv\" id=\"attachmentButton\"><div style=\"display:inline-block;\">DRAG AND DROP&nbsp&nbsp&nbsp&nbsp</div><div style=\"display:inline-block;\">OR&nbsp&nbsp&nbsp&nbsp</div><div style=\"display:inline-block;text-decoration: underline;\">Browse</div>");

        $("#RADUserMgmtuploadFileDoc").append("<div class=\"WorkflowParentDropDrag\" id=\"parent1\"></div>");
        if ($('#parent1').fileUpload != undefined) {
            $('#parent1').fileUpload({
                'parentControlId': 'parent1',
                'attachementControlId': 'attachmentButton',
                'multiple': false,
                'debuggerDiv': '',
                'deleteEvent': function () {
                }
            });
        }

        $(".RADUserMgmtRemove").click(function (event) {
            $(".RADUserMgmtUploadPopUp").remove();
        })
        $(".RADUserMgmtSyncData").unbind().click(function (event) {
              // AllUserUsers.prototype.AddSpinnerOnContentBody();
            var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
            if (closure.flag == "Groups") {
                $.ajax({
                    url: url + '/SyncGroups',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ cacheKey: "parent1" })
                }).then(function (responseText) {
                       //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
                    if (responseText.d == "") {
                        $(".RADUserMgmtHeaderName").text("Uploading Groups is Successfull");
                        setTimeout(function () {
                            $('.RADUserMgmtUploadPopUp').remove();
                            AllUserGroups.prototype.init();
                        }, 2000);
                    }
                    else {
                        $(".RADUserMgmtHeaderName").text("Uploading Groups Failed. Missing Role is: " + responseText.d);
                    }
                    
                })
            }
            else if (closure.flag == "Roles") {
                  // AllUserUsers.prototype.AddSpinnerOnContentBody();
                $.ajax({
                    url: url + '/SyncRoles',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ cacheKey: "parent1" })
                }).then(function (responseText) {
                       //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
                    if (responseText.d == "") {
                        $(".RADUserMgmtHeaderName").text("Uploading Roles is Successfull.");
                        setTimeout(function () {
                            $('.RADUserMgmtUploadPopUp').remove();
                            AllUserRoles.instance.init();
                        }, 2000);
                    }
                    else {
                        $(".RADUserMgmtHeaderName").text("Uploading Roles Failed. Missing Privilege is: " + responseText.d);
                    }
                })
            }
            else {
                  // AllUserUsers.prototype.AddSpinnerOnContentBody();
                $.ajax({
                    url: url + '/SyncFunctionalGroup',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ cacheKey: "parent1" })
                }).then(function (responseText) {
                       //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
                    if (responseText.d == true) {
                        $(".RADUserMgmtHeader").text("Uploading Document is Successfully Done.");
                        setTimeout(function () {
                            $('.RADUserMgmtUploadPopUp').remove();
                        }, 5000);
                    }
                    else {
                        $(".RADUserMgmtHeader").text("Uploading Document is Unsuccessful.");
                    }
                    AllFunctionalGroups.instance.init();
                });

            }
        })
     
    })

}

AllUserUsers.prototype.GetUserSignatureInfo = function (userLogName) {

   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/IsUserSignatureExists',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ userLoginName: userLogName })
    }).then(function (responseText) {

        var isSignatureExist = responseText.d;

        $.ajax({
            url: url + '/GetTagTemplates',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ fileName: 'AllUserSignatures.html' })
        }).then(function (responseText) {
            //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
            $("#" + RADUserManagement.contentBodyId).append(responseText.d);
            $('.usermgmt').addClass("OpacityForDiv");

            if (isSignatureExist) {
                $("#UserSignCase").removeClass("UserSignCaseDisplayNone");

                var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
                AllUserUsers.prototype.AddSpinnerOnContentBody
                $.ajax({
                    url: url + '/GetUserSignature',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ userLoginName: userLogName })
                }).then(function (responseText) {
                    //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
                    document.getElementById("ImgUserSignatureDisplay").src = "data:image/jpg;base64," + responseText.d;
                });

                $("#DisplaySignFile").on("contextmenu", function (e) {
                    return false;
                });

            }
            else {
                $("#NoUserSignCase").removeClass("UserSignCaseDisplayNone");
                AllUserUsers.instance.ReCreateUserSignUploadControl();
            }

            $(".RADUserMgmtRemove").click(function (event) {
                $('.UploadSignaturePopUpParent').remove();
                $('.usermgmt').removeClass("OpacityForDiv");
            })

            $(".UserSignDeleteButton").click(function (event) {
                AllUserUsers.instance.OnDeleteUserSignature(userLogName);
            });

            $(".UserSignUploadButton").click(function (event) {

                if ($('.fuattachmentDiv').length > 0) {

                    var userSignFileExtObject = AllUserUsers.instance.CheckUserSignFileExtension();

                    if (userSignFileExtObject.IsValidExtension) {
                        $('#RADUserMgmtMessageDiv').text("");
                        AllUserUsers.instance.OnCreateUserSignature(userLogName);
                    }
                    else {
                        $('#RADUserMgmtMessageDiv').text(userSignFileExtObject.ErrorMessage);
                        AllUserUsers.instance.ReCreateUserSignUploadControl();
                    }
                }
                else {
                    $('#RADUserMgmtMessageDiv').text("Please select the user signature to upload");
                }
            });
            
        });

    });
}


AllUserUsers.prototype.DeleteUserSignature = function (userLogName) {

   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/DeleteUserSignature',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ userLoginName: userLogName })
    }).then(function (responseText) {
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var isUserSignatureDeleted = responseText.d;

        if (isUserSignatureDeleted) {
            $("#UserSignCase").addClass("UserSignCaseDisplayNone");
            $("#NoUserSignCase").removeClass("UserSignCaseDisplayNone");
            $("#UploadSignFile").empty();
            AllUserUsers.prototype.ReCreateUserSignUploadControl();
        }
        else {

            //Display error message
        }
      
    });
}

AllUserUsers.prototype.ReCreateUserSignUploadControl = function () {

    $("#UploadSignFile").empty();
    $("#UploadSignFile").append("<div class=\"UserSignParentDropDrag\" id=\"parent1\"></div>");
    $("#UploadSignFile").append("<div id=\"UserSignTextIndent\" class =\"UserSignTextIndent\"></div>");
    $("#UploadSignFile").append("<div class=\"fa fa-upload UserSignUploadIconButton\"></div>");
    $("#UploadSignFile").append("<div class=\"DropUserSignDiv\" id=\"DropUserSignDiv\"><div id=\"DropUserSignDragDrop\" class=\"DropUserSignDragDrop\">DRAG AND DROP</div><div id=\"DropUserSignOR\" class=\"DropUserSignOR\">OR</div><div id=\"DropUserSignBrowse\" class=\"DropUserSignBrowse\">Browse</div></div>");

    if ($('#parent1').fileUpload != undefined) {
        $('#parent1').fileUpload({
            'parentControlId': 'parent1',
            'attachementControlId': 'DropUserSignDiv',
            'multiple': false,
            'debuggerDiv': '',
            'deleteEvent': function () {
            }
        });
    }
}

AllUserUsers.prototype.OnDeleteUserSignature = function (userLogName) {

    $("#alertParentDiv").remove();
    $("#UploadSignaturePopUp").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
    $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
    $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
    $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserAlert\"></div>"); //revisit
    $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
    $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Alert</div>")
    $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Are You Sure You want to delete user signature?</div>");
    $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpErrorFooter\"></div>");
    $(".RADUserManagementPopUpErrorFooter").append("<div class=\"RADUserManagementPopUpErrorYes\">Yes</div><div class=\"RADUserManagementPopUpErrorNo\">No</div>")
    $("#alertParentDiv").css("top", "-200px");
    $("#alertParentDiv").animate({ "top": "0px" });
    $(".RADUserManagementPopUpErrorYes").unbind().click(function (event) {
        $("#alertParentDiv").remove();
        AllUserUsers.prototype.DeleteUserSignature(userLogName);
    });
    $(".RADUserManagementPopUpErrorNo").unbind().click(function (event) {
        $("#alertParentDiv").remove();
    });

}

AllUserUsers.prototype.CheckUserSignFileExtension = function () {

    var userSignFileExtObject = {};
    var acceptedFileExtensions = ['jpg', 'jpeg', 'png', 'bmp'];
    $('#MessageDiv').text("");

    var fileName = $('.fuattachmentDiv').children()[0].innerHTML;
    var lastIndex = ((fileName.length) - (fileName.lastIndexOf(".")));
    lastIndex = (0 - (lastIndex - 1));
    var fileExtension = fileName.slice(lastIndex);

    if (acceptedFileExtensions.indexOf(fileExtension.toLowerCase()) == -1) {
        userSignFileExtObject.IsValidExtension = false;
        var acceptedFileExtensionsText = "";
        for (var i = 0; i < acceptedFileExtensions.length; i++) {
            acceptedFileExtensionsText = acceptedFileExtensionsText + "| " + acceptedFileExtensions[i];
        }

        userSignFileExtObject.ErrorMessage = "Please upload image files with extension " + acceptedFileExtensionsText;
    }
    else {
        userSignFileExtObject.IsValidExtension = true;
        userSignFileExtObject.ErrorMessage = "";

    }
    return userSignFileExtObject;
}

AllUserUsers.prototype.OnCreateUserSignature = function (userLogName) {

   // AllUserUsers.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/CreateUserSignature',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ userLoginName: userLogName, cacheKey: 'parent1' })
        //data: JSON.stringify({ cacheKey: 'parent1', userLoginName: userLogName })
    }).then(function (responseText) {
        //AllUserUsers.prototype.RemoveSpinnerOnContentBody();
        var isUserSignCreated = responseText.d;

        if (isUserSignCreated) {

            $("#NoUserSignCase").addClass("UserSignCaseDisplayNone");
            $("#UserSignCase").removeClass("UserSignCaseDisplayNone");

            var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
            $.ajax({
                url: url + '/GetUserSignature',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ userLoginName: userLogName })
            }).then(function (responseText) {
                document.getElementById("ImgUserSignatureDisplay").src = "data:image/jpg;base64," + responseText.d;
            });

            $("#DisplaySignFile").on("contextmenu", function (e) {
                return false;
            });
        }
        else {
            //display upload failure message
        }
        
    });

}

AllUserUsers.prototype.AddSpinnerOnContentBody = function(){
    $("#contentBody").addClass('spinner');
    $("#contentBody").css({ 'opacity': '0.5' });
}

AllUserUsers.prototype.RemoveSpinnerOnContentBody = function(){
    $("#contentBody").removeClass('spinner');
    $("#contentBody").css({ 'opacity': '1' });
}

AllUserUsers.prototype.JSONTest = function (testString) {
    try {
        var json = JSON.parse(testString);
        return true;
    }
    catch (e) {
        return false;
    }
}

var AllUserGroups = function () {
    // this.someObjectVariable = "anything";
};
window.ScrollVariableUser = 0;
window.ScrollVariableup = 0;
window.ScrollVariableUserup = 0;
window.ScrollVariableRolesup = 0;
AllUserGroups.instance = undefined;
var groupName;
var groupNameReCreateDiv = false;
var GlobalFlagSearch;

AllUserGroups.prototype.init = function (sandBox) {
    AllUserGroups.instance = this;

    //AllUserGroups.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetTagTemplate', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
      //  AllUserGroups.prototype.RemoveSpinnerOnContentBody();
        //$("#" + RADUserManagement.contentBodyId).empty();    
        $(".usermgmt").remove();
        $("#pageHeaderReportRange_dateInfo").css("display", "none")
        $(".daterangepicker").css("display", "none")
        $("#" + RADUserManagement.contentBodyId).append(responseText.d);
        $(".umRightHeader").children()[0].innerText = "Group Name : "; //to remove divs of username        
        $(".umSec1Head").html("USERS")//to set heading as Users in place of groups

        window.ScrollVariable = 0;
        window.ScrollVariableGroup = 0;
        window.ScrollVariableRoles = 0;


        var umDynamicTilesHeight = $("#contentBody").height() - ($(".umLeftHeader").height() + $(".RADUserMgmtscrollUpDown").height() + $(".searchBarUm").height());
        $('.umDynamicTiles').height(umDynamicTilesHeight);

        var umRightBodyHeight = $(".umRightSec").height() - $(".umRightHeader").height();
        $('.umRightBody').height(umRightBodyHeight);


        var umSec1Padding = $('.umSec1').css('paddingTop').slice(0, -2);
        var umSec1Height = $(".umRightBody").height() - umSec1Padding;
        $('.umSec1').height(umSec1Height);

        var umSec2Padding = $('.umSec2').css('paddingTop').slice(0, -2);
        var umSec2Height = $(".umRightBody").height() - umSec2Padding;
        $('.umSec2').height(umSec2Height);


        $.ajax({
            url: url + '/IsLDAPEnabledForGroup',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json'

        }).then(function (responseText) {
            var b = JSON.parse(responseText.d);            
            AllUserGroups.instance.IsUserCreationAllowed = false;
            AllUserGroups.instance.GroupInfo = []
            AllUserGroups.instance.IsLDAPEnabled = b;
            AllUserGroups.instance.SelectedUserArray = []
            AllUserGroups.instance.SelectedUserSearch = [];
            AllUserGroups.instance.AllUserList = []
            AllUserGroups.instance.SelectedRolesArray = []
            AllUserGroups.instance.SelectedRolesSearch = [];
            AllUserGroups.instance.AllRolesList = []
            AllUserGroups.instance.SelectedUserArrayCreateSearch = []
            AllUserGroups.instance.SelectedRolesArrayCreateSearch = []
            $('.umSec1').append("<div class=\"RADUserMgmtscrollUpDownGroup\"></div>")
            $('.scrollUpDownGroup').append("<div id=\"RADUserManagementScrollDownArrowGroup\" class=\"fa fa-caret-down RADUserManagementScrollArrowGroupSmall\"></div>")
            $('.scrollUpDownGroup').append("<div id=\"RADUserManagementScrollUpArrowGroup\" class=\"fa fa-caret-up RADUserManagementScrollArrowGroupSmall\"></div>")
            $('.umSec2').append("<div class=\"RADUserMgmtscrollUpDownRoles\"></div>")
            $('.scrollUpDownRoles').append("<div id=\"RADUserManagementScrollDownArrowRoles\" class=\"fa fa-caret-down RADUserManagementScrollArrowGroupSmall\"></div>")
            $('.scrollUpDownRoles').append("<div id=\"RADUserManagementScrollUpArrowRoles\" class=\"fa fa-caret-up RADUserManagementScrollArrowGroupSmall\"></div>")

            $('#RADAuditButtonDiv').append("<input id=\"RADUserManagementAuditButton\" class=\"RADUserManagementAuditButton\" type=\"button\" value=\"Audit\"/>");
            $("#RADDeleteInactiveButtonDiv").append("<input id=\"RADUserManagementDeleteInActiveButton\" class=\"RADUserManagementDeleteInActiveButton\" type=\"button\" value=\"Delete InActive Groups\"/>");

            $("<div class=\"HorizontalRule\"> </div>").insertAfter($('.UMRightHeaderEdit'))

            var umSec1BodyHeight = $(".umSec1").height() - (Number(umSec1Padding) + $(".umSec1Head").height() + $(".scrollUpDownGroup").height())
            $('.umSec1Body').height(umSec1BodyHeight);

            var umSec2BodyHeight = $(".umSec2").height() - (Number(umSec2Padding) + $(".umSec2Head").height() + $(".scrollUpDownRoles").height())
            $('.umSec2Body').height(umSec2BodyHeight);

            if (b == true) {
                $.ajax({
                    url: url + '/IsUserCreationAllowed',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json'

                }).then(function (responseText) {
                    var b = JSON.parse(responseText.d);
                    AllUserGroups.instance.IsUserCreationAllowed = b;
                });
            }

        });
        AllUserGroups.instance.GetAuthorizationPrivileges()
        AllUserGroups.instance.GetAllGroups();
        AllUserGroups.instance.BindEvents();
    });

}
AllUserGroups.prototype.GetAuthorizationPrivileges = function () {

   // AllUserGroups.prototype.AddSpinnerOnContentBody();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAuthorizationPrivileges', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'

    }).then(function (responseText) {
        //AllUserGroups.prototype.AddSpinnerOnContentBody();
        if (responseText.d == "admin") {
            $(".RADUserCreatePlusCircle").show()
            $("#RADUserManagementEditButton").show()
            $("#RADUserManagementDeleteButton").show()
            $("#RADDownloadButtonDiv").show();
            $("#RADUploadButtonDiv").show();
            Priviliges = ['Add Group', 'Update Group', 'Delete Group', 'Download Configuration', 'Upload Configuration'];
        }
        else {
            var ResponseForCreation = [];
            if (responseText.d.length > 0)
                ResponseForCreation = JSON.parse(responseText.d);            
            Priviliges = []
            for (var i = 0; i < ResponseForCreation.length; i++) {
                if (ResponseForCreation[i].pageId == "RAD_Group") {
                    for (var j = 0; j < ResponseForCreation[i].Privileges.length; j++)
                        Priviliges.push(ResponseForCreation[i].Privileges[j])
                }
            }
            if (Priviliges.indexOf("Add Group") != -1) {
                $('.RADUserCreatePlusCircle').show();
            }
            else {
                $('.RADUserCreatePlusCircle').hide();
            }
            if (Priviliges.indexOf("Delete Group") != -1) {
                $("#RADUserManagementDeleteButton").show()
            }
            else {
                $("#RADUserManagementDeleteButton").hide()
            }
            if (Priviliges.indexOf("Update Group") != -1) {
                $("#RADUserManagementEditButton").show()
            }
            else {
                $("#RADUserManagementEditButton").hide()
            }
            if (Priviliges.indexOf("Download Configuration") != -1) {
                $("#RADDownloadButtonDiv").show();
            }
            else {
                $("#RADDownloadButtonDiv").hide();
            }
            if (Priviliges.indexOf("Upload Configuration") != -1) {
                $("#RADUploadButtonDiv").show();
            }
            else {
                $("#RADUploadButtonDiv").hide();
            }
        }
    })
}
AllUserGroups.prototype.UserSearch = function (event) {

    for (var i = 0; i < $(".umSec1Body").find(".RADUserManagementSelectedUsersInGroup").length; i++) {

        if (!Array.contains(AllUserGroups.instance.SelectedUserSearch,
             $(".umSec1Body").find(".RADUserManagementSelectedUsersInGroup")[i].innerText))
            AllUserGroups.instance.SelectedUserSearch.push
                ($(".umSec1Body").find(".RADUserManagementSelectedUsersInGroup")[i].innerText);
    }

    $(".umSec1Body").empty();

    //This below if statement is written to show, selected items first, when no search text is present
    if ($(event.target).val() == "") {

        for (var k = 0; k < AllUserGroups.instance.SelectedUserSearch.length; k++) {

            $('.umSec1Body').append('<div id="UserSelected' + k + '" User_name="' + AllUserGroups.instance.SelectedUserSearch[k] + '">'
                                                                          + AllUserGroups.instance.SelectedUserSearch[k] + '</div>')
            $("#UserSelected" + k).addClass('RADUserManagementSelectedUsersInGroup');
            $("#UserSelected" + k).addClass('RADUserManagementUsersInGroup');
        }

        for (var j = 0; j < AllUserGroups.instance.AllUserList.length; j++) {

            if (!Array.contains(AllUserGroups.instance.SelectedUserSearch, AllUserGroups.instance.AllUserList[j])) {

                $('.umSec1Body').append('<div id="User' + j + '" User_name="' + AllUserGroups.instance.AllUserList[j] + '">'
                                                                              + AllUserGroups.instance.AllUserList[j] + '</div>')
                $("#User" + j).addClass('RADUserManagementUsersInGroup');
            }
        }
    }
    else {
        for (var j = 0; j < AllUserGroups.instance.AllUserList.length; j++) {

            if ((AllUserGroups.instance.AllUserList[j].toLowerCase()).indexOf($(event.target).val().toLowerCase()) != -1) {

                if (AllUserGroups.instance.SelectedUserSearch.indexOf(AllUserGroups.instance.AllUserList[j]) != -1) {

                    $('.umSec1Body').append('<div id="User' + j + '" User_name="' + AllUserGroups.instance.AllUserList[j] + '">'
                                                                        + AllUserGroups.instance.AllUserList[j] + '</div>')
                    $("#User" + j).addClass('RADUserManagementSelectedUsersInGroup');
                    $("#User" + j).addClass('RADUserManagementUsersInGroup');
                }
                else {
                    $('.umSec1Body').append('<div id="User' + j + '" User_name="' + AllUserGroups.instance.AllUserList[j] + '">'
                                                                                 + AllUserGroups.instance.AllUserList[j] + '</div>')
                    $("#User" + j).addClass('RADUserManagementUsersInGroup');
                }
            }
        }
    }

    $(".RADUserManagementUsersInGroup").unbind().click(function (event) {
        if ($(this).hasClass("RADUserManagementSelectedUsersInGroup") == true) {
            $(this).switchClass("RADUserManagementSelectedUsersInGroup", "RADUserManagementUsersInGroup")
            AllUserGroups.instance.SelectedUserSearch.splice(AllUserGroups.instance.SelectedUserSearch.indexOf($(this).text()), 1);
        }
        else {
            $(this).addClass('RADUserManagementSelectedUsersInGroup');
            if ((!Array.contains(AllUserGroups.instance.SelectedUserSearch, $(this).text()))
                && (Array.contains(AllUserGroups.instance.AllUserList, $(this).text())))
                AllUserGroups.instance.SelectedUserSearch.push($(this).text())
        }
    });
}

AllUserGroups.prototype.RoleSearch = function (event) {

    for (var i = 0; i < $(".umSec2Body").find(".RADUserManagementSelectedUsersInGroup").length; i++) {

        if (!Array.contains(AllUserGroups.instance.SelectedRolesSearch,
            $(".umSec2Body").find(".RADUserManagementSelectedUsersInGroup")[i].innerText))
            AllUserGroups.instance.SelectedRolesSearch.push
                ($(".umSec2Body").find(".RADUserManagementSelectedUsersInGroup")[i].innerText);
    }

    $(".umSec2Body").empty();

    //This below if statement is written to show, selected items first, when no search text is present
    if ($(event.target).val() == "") {

        for (var k = 0; k < AllUserGroups.instance.SelectedRolesSearch.length; k++) {

            $('.umSec2Body').append('<div id="RolesSelected' + k + '" Role_name="' + AllUserGroups.instance.SelectedRolesSearch[k] + '">'
                                                                               + AllUserGroups.instance.SelectedRolesSearch[k] + '</div>')
            $("#RolesSelected" + k).addClass('RADUserManagementSelectedUsersInGroup');
            $("#RolesSelected" + k).addClass('RADUserManagementUsersInGroup');
        }

        for (var j = 0; j < AllUserGroups.instance.AllRolesList.length; j++) {

            if (!Array.contains(AllUserGroups.instance.SelectedRolesSearch, AllUserGroups.instance.AllRolesList[j])) {

                $('.umSec2Body').append('<div id="Rolesarch' + j + '" Role_name="' + AllUserGroups.instance.AllRolesList[j] + '">'
                                                                                   + AllUserGroups.instance.AllRolesList[j] + '</div>')
                $("#Rolesarch" + j).addClass('RADUserManagementUsersInGroup');

            }
        }
    }
    else {

        for (var j = 0; j < AllUserGroups.instance.AllRolesList.length; j++) {

            if ((AllUserGroups.instance.AllRolesList[j].toLowerCase()).indexOf($(event.target).val().toLowerCase()) != -1) {

                if (AllUserGroups.instance.SelectedRolesSearch.indexOf(AllUserGroups.instance.AllRolesList[j]) != -1) {
                    $('.umSec2Body').append('<div id="Rolesarch' + j + '" Role_name="' + AllUserGroups.instance.AllRolesList[j] + '">'
                                                                                               + AllUserGroups.instance.AllRolesList[j] + '</div>')
                    $("#Rolesarch" + j).addClass('RADUserManagementSelectedUsersInGroup');
                    $("#Rolesarch" + j).addClass('RADUserManagementUsersInGroup');
                }
                else {

                    $('.umSec2Body').append('<div id="Rolesarch' + j + '" Role_name="' + AllUserGroups.instance.AllRolesList[j] + '">'
                                                                                  + AllUserGroups.instance.AllRolesList[j] + '</div>')
                    $("#Rolesarch" + j).addClass('RADUserManagementUsersInGroup');
                }
            }
        }
    }

    $(".RADUserManagementUsersInGroup").unbind().click(function (event) {
        if ($(this).hasClass("RADUserManagementSelectedUsersInGroup") == true) {
            $(this).switchClass("RADUserManagementSelectedUsersInGroup", "RADUserManagementUsersInGroup")
            AllUserGroups.instance.SelectedRolesSearch.splice(AllUserGroups.instance.SelectedRolesSearch.indexOf($(this).text()), 1);
        }
        else {
            $(this).addClass('RADUserManagementSelectedUsersInGroup');
            if ((!Array.contains(AllUserGroups.instance.SelectedRolesSearch, $(this).text()))
                && (Array.contains(AllUserGroups.instance.AllRolesList, $(this).text())))
                AllUserGroups.instance.SelectedRolesSearch.push($(this).text())
        }
    });
}


AllUserGroups.prototype.GetAllGroups = function () {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllGroups',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        var b;
        ListOfGroups = JSON.parse(responseText.d);
        var firstGroup = "";
        AllUserGroups.instance.ListOfInfo = ListOfGroups;
        for (var j = 0; j < ListOfGroups.length; j++) {

            $('.umDynamicTiles').append('<div title="' + ListOfGroups[j]["GroupName"] + '"id="t' + j + '" name="' + ListOfGroups[j]["GroupName"] + '"></div>');
            if (j == 0) {
                firstGroup = ListOfGroups[j]["GroupName"];
                $("#t" + j).addClass('RADUserMgmttiles').addClass('RADTileActive');
                $("#t" + j).append('<div title="' + ListOfGroups[j]["GroupName"] + '" id="group' + j + '" name="' + ListOfGroups[j]["GroupName"] + '">' + ListOfGroups[j]["GroupName"] + '</div>');
                $("#t" + j).closest(".RADUserMgmttiles").append('<div id="arr"></div>');
                $(".umDynamicTiles").find(".RADarrow-right").remove();
                $("#arr").addClass('fa fa-caret-right RADarrow-right').insertAfter("#t" + j);
                $("#group" + j).addClass('RADUserMgmtuser_name');
                $("#t" + j).append('<div title="' + ListOfGroups[j]["Description"] + '"id="group_desc' + j + '" Description="' + ListOfGroups[j]["Description"] + '">' + ListOfGroups[j]["Description"] + '</div>');
                $("#group_desc" + j).addClass('RADUserMgmtemail_id');
            }

            else {
                $("#t" + j).addClass('RADUserMgmttiles')
                $("#t" + j).append('<div title="' + ListOfGroups[j]["GroupName"] + '" id="group' + j + '" name="' + ListOfGroups[j]["GroupName"] + '">' + ListOfGroups[j]["GroupName"] + '</div>');

                $("#group" + j).addClass('RADUserMgmtuser_name');
                $("#t" + j).append('<div title="' + ListOfGroups[j]["Description"] + '"  id="group_desc' + j + '" Description="' + ListOfGroups[j]["Description"] + '">' + ListOfGroups[j]["Description"] + '</div>');
                $("#group_desc" + j).addClass('RADUserMgmtemail_id');
            }
        }
        AllUserGroups.instance.getGroupInfo(firstGroup);
        $(".RADUserMgmttiles").unbind().click(function (event) {
            AllUserGroups.instance.ClickOnEachTile(event);

        });
    });
}

AllUserGroups.prototype.ClickOnEachTile = function (event) {
    if ($("#RADAuditScreenMain").css("display") == "block") {
        $("#RADAuditScreenMain").hide('slow', function () { $("#RADAuditScreenMain").remove(); });
    }
    $("#Groupsearch_text").remove()

    $(".RADUserManagementActiveAccountValidation").remove();
    $(".RADUserManagementAccountGroupValidation").remove();
    $(".RADUserManagementAccountGroupValidationInline").remove();
    $(".RADUMGroupSearchMAinParentDiv").remove()
    $(".RADUMGroupSearchMAinParentDivSec2").remove()
    $("#RADUMGroupSearch").remove()
    $("#Rolesearch_text").remove()
    $("#RADUMRoleSearch").remove()
    AllUserGroups.instance.ShowHideByPrivilegeType("Add");
    AllUserGroups.instance.ShowHideByPrivilegeType("Delete");
    AllUserGroups.instance.ShowHideByPrivilegeType("Edit");
    $("#RADUserManagementDeleteButton").val("Delete");    
    $(".searchBarUm").show();
    if ($("#RADUserManagementEditButton").val() == "Edit") {
        $(".RADUserManagementAuditButton").show()

        for (var i = 0; i < $(".umSec2Body").children().length; i++) {
            if ($($(".umSec2Body").children()[i]).hasClass("RADUserManagementSelectedUsersInGroup"))
                $($(".umSec2Body").children()[i]).removeClass("RADUserManagementSelectedUsersInGroup")
        }
        for (var i = 0; i < $(".umSec1Body").children().length; i++) {
            if ($($(".umSec1Body").children()[i]).hasClass("RADUserManagementSelectedUsersInGroup"))
                $($(".umSec1Body").children()[i]).removeClass("RADUserManagementSelectedUsersInGroup")
        }
        AllUserGroups.instance.selectGroup();
        AllUserGroups.instance.SetGroup = $(event.target).closest(".RADUserMgmttiles").children()[0].innerText;
    }
    else {
        $(".RADUserManagementAuditButton").hide()
        $(".RADUserManagementActiveAccountValidation").remove();
        $(".RADUserManagementAccountGroupValidation").remove();
        $(".RADUserManagementAccountGroupValidationInline").remove();


        if ($("#alertParentDivEdit") != null) {
            $("#ContentDiv").find("#alertParentDivedit").remove();
        }

        $("#ContentDiv").append("<div id=\"alertParentDivedit\" class=\"RADUserManagementAlertPopUpParent\"></div>");
        $("#alertParentDivedit").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
        $("#alertParentDivedit").append("<div class=\"RADUserManagementPopUpError\"></div>")
        $("#alertParentDivedit").append("<div class=\"RADUserErrorDivLeft RADUserAlert\"></div>");//revisit
        $("#alertParentDivedit").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
        $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Alert</div>")
        $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Your Changes will revert.Want To Continue?</div>");
        $("#alertParentDivedit").append("<div class=\"RADUserManagementPopUpErrorFooter\"></div>");
        $(".RADUserManagementPopUpErrorFooter").append("<div class=\"RADUserManagementPopUpErrorYes\">Yes</div><div class=\"RADUserManagementPopUpErrorNo\">No</div>")
        $("#alertParentDivedit").css("top", "-200px");
        $("#alertParentDivedit").animate({ "top": "0px" });
        $(".RADUserManagementPopUpErrorYes").unbind().click(function (event) {
            if ($("#RADUserManagementEditButton").attr("creategroup") == "true") {
                $(".RADUserManagementAuditButton").hide()
                $(".RADUserManagementActiveAccountValidation").remove();
                $(".RADUserManagementAccountGroupValidation").remove();
                $(".RADUserManagementAccountGroupValidationInline").remove();
                AllUserGroups.instance.ReCreateDiv();
                $(".umSec1Body").empty();
                $(".umSec2Body").empty();
                for (var i = 0; i < AllUserGroups.instance.UsersInGroup.length; i++) {
                    $(".umSec1Body").append("<div class=\"RADUserManagementUsersInGroup\">" + AllUserGroups.instance.UsersInGroup[i] + "</div>");
                }
                for (var i = 0; i < AllUserGroups.instance.RolesInGroup.length; i++) {
                    $(".umSec2Body").append("<div class=\"RADUserManagementUsersInGroup\">" + AllUserGroups.instance.RolesInGroup[i] + "</div>");
                }
            }
            else if ($("#RADUserManagementEditButton").attr("createadgroup") == "true") {
                $(".RADUserManagementAuditButton").hide()
                $(".RADUserManagementActiveAccountValidation").remove();
                $(".RADUserManagementAccountGroupValidation").remove();
                $(".RADUserManagementAccountGroupValidationInline").remove();
                AllUserGroups.instance.ReCreateDiv();
            }
            else {
                $(".RADUserManagementAuditButton").show()
                $(".RADUserManagementActiveAccountValidation").remove();
                $(".RADUserManagementAccountGroupValidation").remove();
                $(".RADUserManagementAccountGroupValidationInline").remove();
                AllUserGroups.instance.ReCreateDiv();
                $(".umSec1Body").empty();
                $(".umSec2Body").empty();
                for (var i = 0; i < AllUserGroups.instance.UsersInGroup.length; i++) {
                    $(".umSec1Body").append("<div class=\"RADUserManagementUsersInGroup\">" + AllUserGroups.instance.UsersInGroup[i] + "</div>");
                }
                for (var i = 0; i < AllUserGroups.instance.RolesInGroup.length; i++) {
                    $(".umSec2Body").append("<div class=\"RADUserManagementUsersInGroup\">" + AllUserGroups.instance.RolesInGroup[i] + "</div>");
                }
            }

            //$("#alertParentDivedit").remove();
            if ($("#alertParentDivEdit") != null){
                $("#ContentDiv").find("#alertParentDivedit").remove();
            }
            $("#RADUserManagementEditButton").val("Edit")
            if ($("#RADUserManagementEditButton").attr("creategroup") != null)
                $("#RADUserManagementEditButton").removeAttr("creategroup");
            if ($("#RADUserManagementEditButton").attr("createadgroup") != null)
                $("#RADUserManagementEditButton").removeAttr("createadgroup");

        });
        $(".RADUserManagementPopUpErrorNo").unbind().click(function (event) {
            $("#alertParentDivedit").remove();
        });
    }


}
AllUserGroups.prototype.getGroupInfo = function (groupName) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetGroupInfo',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ groupName: groupName })

    }).then(function (responseText) {
        var groupInfo = JSON.parse(responseText.d);
        AllUserGroups.instance.GroupInfo = groupInfo
        var groupName = groupInfo["GroupName"];

        $("#lab").html(groupName);
        $(".RADUserMgmtuser_label").attr('title', groupName);
        $("#lab1").html(groupInfo["AccountName"]);

        var usersInAGroup = [];
        var userRoles = [];
        usersInAGroup = groupInfo["UserName"]
        userRoles = groupInfo["Roles"];
        AllUserGroups.instance.UsersInGroup = groupInfo["UserName"];
        AllUserGroups.instance.RolesInGroup = groupInfo["Roles"];
        $(".umSec1Body").empty();
        $(".umSec2Body").empty();
        for (var i = 0; i < usersInAGroup.length; i++) {
            $(".umSec1Body").append("<div class=\"RADUserManagementUsersInGroup\">" + usersInAGroup[i] + "</div>");
        }
        for (var i = 0; i < userRoles.length; i++) {
            $(".umSec2Body").append("<div class=\"RADUserManagementUsersInGroup\">" + userRoles[i] + "</div>");
        }
        $(".RADUserManagementUsersInGroup").unbind().click(function (event) {
            if ($("#RADUserManagementEditButton").attr("value") == "Save")
                AllUserGroups.instance.selectUnSelectedItems(event);
        });
    });
}
AllUserGroups.prototype.selectGroup = function () {
    $(".umDynamicTiles").find(".RADTileActive").removeClass("RADTileActive");
    $(event.target).closest(".RADUserMgmttiles").append('<div id="arr"></div>');
    $(".umDynamicTiles").find(".RADarrow-right").remove();
    $("#arr").addClass('RADarrow-right fa fa-caret-right').insertAfter($(event.target).closest(".RADUserMgmttiles"));
    $('.umSec1Body').empty();
    $(event.target).closest(".RADUserMgmttiles").addClass('RADTileActive');
    var groupName = $(event.target).closest(".RADUserMgmttiles").children()[0].innerText;
    AllUserGroups.instance.getGroupInfo(groupName);
}

AllUserGroups.prototype.ShowHideByPrivilegeType = function (PrivilegeType) {

    switch (PrivilegeType) {

        case "Add":
            if (Priviliges.indexOf("Add Group") != -1)
                $('.RADUserCreatePlusCircle').show();
            else
                $('.RADUserCreatePlusCircle').hide();
            break;

        case "Edit":
            if (Priviliges.indexOf("Update Group") != -1)
                $("#RADUserManagementEditButton").show();
            else
                $("#RADUserManagementEditButton").hide();
            break;

        case "Delete":
            if (Priviliges.indexOf("Delete Group") != -1)
                $("#RADUserManagementDeleteButton").show();
            else
                $("#RADUserManagementDeleteButton").hide();
            break;
        default:
            //do nothing
    }
}

AllUserGroups.prototype.BindEvents = function () {

    $("#RADUserManagementDownloadButton").unbind().click(function (event) {
        AllUserUsers.instance.getHTML(AllUserGroups.instance.ListOfInfo, "Groups"); 
    });

    $("#RADUserManagementUploadButton").unbind().click(function (event) {
        AllUserUsers.instance.getUploadPopUpHTML("Groups");
    });

    $("#ContentDiv").unbind().click(function (event) {
        if ($(event.target).closest(".RADUserManagementAddNewGroupAccount").length == 0) {
            if ($("#RADUserManagementAddNewGroupAccount").height() != 30)
                $("#RADUserManagementAddNewGroupAccount").css({ "height": "30px" });
        }
    });

    $(".umSec1").on('click', '#RADUserManagementScrollDownArrowGroup', function (event) {
        ScrollVariableGroup = ScrollVariableGroup + 50;
        $(".umSec1Body").scrollTop(ScrollVariableGroup);
    });

    $(".umSec1").on('click', '#RADUserManagementScrollUpArrowGroup', function (event) {
        if (ScrollVariableGroup > 0) {
            ScrollVariableGroup = ScrollVariableGroup - 50;
            $(".umSec1Body").scrollTop(ScrollVariableGroup);
        }
    });

    $(".UMRightHeaderEdit").on("click", "#RADUserManagementDeleteInActiveButton", function (event) {
        AllUserGroups.instance.DeleteInActiveGroups();
    });

    $(".umSec2").on('click', '#RADUserManagementScrollDownArrowRoles', function (event) {
        ScrollVariableRoles = ScrollVariableRoles + 50;
        $(".umSec2Body").scrollTop(ScrollVariableRoles);
    });

    $(".umSec2").on('click', '#RADUserManagementScrollUpArrowRoles', function (event) {
        if (ScrollVariableRoles > 0) {
            ScrollVariableRoles = ScrollVariableRoles - 50;
            $(".umSec2Body").scrollTop(ScrollVariableRoles);
        }
    });

    $(".umRightSec").on('click', '#RADUserManagementAuditButton', function (event) {
        AllUserGroups.instance.AuditScreen(event)
    });

    $(".umLeftSec").on('click', '.RADsearchIconGridView', function (event) {
        if ($('#search_text:visible').length) {            
            $("#search_text").hide("slow", function () {
                // Animation complete.
            });
        }
        else {            
            $("#search_text").show("slow", function () {
                // Animation complete.
            });
        }        
    });

    $(".umSec1").on("keyup", "#Groupsearch_text", function (event) {
        AllUserGroups.instance.UserSearch(event)
    });

    $(".umSec2").on("keyup", "#Rolesearch_text", function (event) {
        AllUserGroups.instance.RoleSearch(event)
    });

    $("#RADUserManagementEditButton").unbind().click(function (event) {

        GlobalFlagSearch = false
        $("#RADUserCreatePlusCircle").hide();
        $(".searchBarUm").hide();
        $(".RADUserManagementAuditButton").hide()
        $('.RADUserCreatePlusCircle').hide();
        $("#RADUserManagementDeleteButton").show();
        $("#RADUserManagementDeleteButton").val("Cancel");
       
        if ($(event.target).attr("value") == "Edit") {//to check if it is in edit mode or not
            AllUserGroups.instance.OnEditGroup();
        }
        else if ($(event.target).attr("value") == "Save" && $(event.target).attr("createGroup") == "true") {            
            AllUserGroups.instance.OnCreateGroup();
        }
        else if ($(event.target).attr("value") == "Save" && $(event.target).attr("createADGroup") == "true") {
            AllUserGroups.instance.CreateADGroup();
        }
        else if ($(event.target).attr("value") == "Save") {
            AllUserGroups.instance.OnModifyGroup();
        }
    });//on clicking edit icon check if it is in edit mode or not if not then allow dit else give popUp

    $(".RADUserCreatePlusCircle").unbind().click(function (event) {
        AllUserGroups.instance.SelectedUserSearch = [];
        AllUserGroups.instance.SelectedRolesSearch = [];
        $('.RADUserCreatePlusCircle').hide();        
        GlobalFlagSearch = true
        $(".RADUserManagementAuditButton").hide()
        $("#RADUserManagementEditButton").val("Save");
        $("#RADUserManagementDeleteButton").val("Cancel");
        $("#RADUserManagementEditButton").show();
        $("#RADUserManagementDeleteButton").show();

        if (AllUserGroups.instance.IsLDAPEnabled == false) {
            $(".umRightHeader").empty();
            $("<div id=\"RADUMGroupSearchMAinParentDiv\"  class=\"RADUMGroupSearchMAinParentDiv\"></div>").insertAfter($('.umSec1Head'))
            $(".RADUMGroupSearchMAinParentDiv").append("<div id=\"RADUMGroupSearch\"  class=\"fa fa-search searchIconGridViewForUserGroup\"></div>")
            $(".RADUMGroupSearchMAinParentDiv").append("<input type=\"text\" name=\"search_text\" id=\"Groupsearch_text\" placeholder=\"Search\"></div>")
            $("<div id=\"RADUMGroupSearchMAinParentDivSec2\"  class=\"RADUMGroupSearchMAinParentDivSec2\"></div>").insertAfter($('.umSec2Head'))
            $(".RADUMGroupSearchMAinParentDivSec2").append("<div id=\"RADUMRoleSearch\"  class=\"fa fa-search searchIconGridViewForUserRoles\"></div>")
            $(".RADUMGroupSearchMAinParentDivSec2").append("<input type=\"text\" name=\"search_text\" id=\"Rolesearch_text\" placeholder=\"Search\"></div>")

            $(".umRightHeader").append('<div id=\"LabelHeaderGname\" class=\"RADUserMgmtLabelHeader\" ></div>');
            $('#LabelHeaderGname').append('<input id=\"RADUserManagementAddNewGroupName\" class=\"RADUserManagementAddNewGroupName\" placeholder=\"Group Name\"></input>');
            
            $(".umRightHeader").append('<div id=\"LabelHeaderGdesc\" class=\"RADUserMgmtLabelHeader\" ></div>');
            $('#LabelHeaderGdesc').append('<input id=\"RADUserManagementAddNewGroupDesc\" class=\"RADUserManagementAddNewGroupDesc\" placeholder=\"Group Description\"></input>');
            
           // $(".umRightHeader").append('<div id=\"LabelHeaderGAccount\" class=\"RADUserMgmtLabelHeader\" ></div>');
           // $("#LabelHeaderGAccount").append("<div id=\"RADUserManagementAddNewGroupAccount\" class=\"RADUserManagementAddNewGroupAccount\"></div>");
           // //$("#RADUserManagementAddNewGroupAccount").append("<div id=\"RADUserManagementGroupFirstAccount\" class=\"RADUserManagementGroupFirstAccount\">Account Name</div>")
           // $("#RADUserManagementAddNewGroupAccount").append("<div id=\"RADUserManagementGroupFirstAccount\"</div>")
            //// $("#RADUserManagementAddNewGroupAccount").append("<div class=\"fa fa-caret-down RADUserManagementGroupDownArrow\"></div>")

            $(".umRightHeader").append("<div id=\"RADUserManagaementAccLabel\" class=\"RADUserManagaementLabel_New\"><label id=\"lblAccLabel\" >Account </label></div>");
            $(".umRightHeader").append("<div id=\"lab1\" class=\"RADUserMgmtuser_label_New\"></div>");
            $("#lab1").append("<div id=\"RADUserManagementAccountInlineFilter\"></div>");

            $(".RADUserMgmtLabelHeader").css({ "width": "30%" });
            $(".RADUserMgmtLabelHeader").css({ "padding-top": "20px" });

            $(".umSec1Body").empty();
            $(".umSec2Body").empty();
            AllUserGroups.instance.SelectedUserArrayCreateSearch = []
            AllUserGroups.instance.SelectedRolesArrayCreateSearch = []
            $("#RADUserManagementEditButton").attr("createGroup", "true");

            AllUserGroups.instance.getAllAccounts(false);
            AllUserGroups.instance.CreategetAllRoles();
        }
        else {
            $(".umRightHeader").empty();
            $(".umSec1").css({ "display": "none" });
            $(".umSec1Body").empty();
            $(".umSec2").css({ "margin-left": "196px" });

            $(".umRightHeader").append("<div id=\"RADUserManagaementUserLabel\" class=\"RADUserManagaementLabel_New\"><label id=\"lblUserLabel\" >Groups </label></div>");
            $(".umRightHeader").append("<div id=\"lab\" class=\"RADUserMgmtuser_label_New\"></div>");
            //$("#lab").append("<div id=\"RADUserManagementGroupInlineFilter\" class=\"RADUserManagementGroupInlineFilter\"></div>");
            $("#lab").append("<div id=\"RADUserManagementGroupInlineFilter\"></div>");

            $(".umRightHeader").append("<div id=\"RADUserManagaementAccLabel\" class=\"RADUserManagaementLabel_New\"><label id=\"lblAccLabel\" >Account </label></div>");
            $(".umRightHeader").append("<div id=\"lab1\" class=\"RADUserMgmtuser_label_New\"></div>");            
            $("#lab1").append("<div id=\"RADUserManagementAccountInlineFilter\"></div>");

            //$(".umRightHeader").append("<div id=\"lab1\" class=\"RADUSERManagementAccountValue\><label id=\"lblAccountLabel\" >Account</label></div></div>");      
            //$("#lab1").append("<div id=\"RADUserManagementAccountInlineFilter\"></div>");
            //$("#lab1").append("<div id=\"RADUserManagementAddNewGroupAccount\" class=\"RADUserManagementAddNewGroupAccount\"></div>");
            //$("#RADUserManagementAddNewGroupAccount").append("<div id=\"RADUserManagementGroupFirstAccount\" class=\"RADUserManagementGroupFirstAccount\">Account Name</div>")
            $("#RADUserManagementAddNewGroupAccount").append("<div id=\"RADUserManagementGroupFirstAccount\"></div>")

            $("#RADUserManagementAddNewGroupAccount").append("<div class=\"fa fa-caret-down RADUserManagementGroupDownArrow\"></div>")


            AllUserGroups.prototype.GetActiveDirectoryGroups();

            $("#RADUserManagementEditButton").attr("createADGroup", "true");
            AllUserGroups.instance.getAllAccounts(true);
            AllUserGroups.instance.CreategetAllRoles();

            $("<div id=\"RADUMGroupSearchMAinParentDivSec2\"  class=\"RADUMGroupSearchMAinParentDivSec2\"></div>").insertAfter($('.umSec2Head'))
            $(".RADUMGroupSearchMAinParentDivSec2").append("<div id=\"RADUMRoleSearch\"  class=\"fa fa-search searchIconGridViewForUserRoles\"></div>")
            $(".RADUMGroupSearchMAinParentDivSec2").append("<input type=\"text\" name=\"search_text\" id=\"Rolesearch_text\" placeholder=\"Search\"></div>")
        }
    });//adding new group

    $("#RADUserManagementDeleteButton").unbind().click(function (event) {// delete a group

        if ($(event.target).attr("value") == "Delete") {
            AllUserGroups.instance.OnDeleteClick();
        }

        else if ($(event.target).attr("value") == "Cancel") {            
            $(".umSec1").css({ "display": "inline-block" });
            $(".umSec2").css({ "margin-left": "0px" });
            $(".searchBarUm").show();
            $(".RADUserManagementAuditButton").show()
            $(".RADUserManagementActiveAccountValidation").remove();
            $(".RADUserManagementAccountGroupValidation").remove();
            $(".RADUserManagementAccountGroupValidationInline").remove();
            $("#Groupsearch_text").remove()
            $("#Rolesearch_text").remove()
            AllUserGroups.instance.ShowHideByPrivilegeType("Add");            
            $("#RADGroupDesceValidation").remove()
            $("#RADUMGroupSearch").remove()
            $(".RADUMGroupSearchMAinParentDiv").remove()
            $(".RADUMGroupSearchMAinParentDivSec2").remove()
            $("#RADUMRoleSearch").remove()
            $("#RADUserGroupNameValidation").remove()
            $("#RADAccountValidation").remove()
            $(".umSec1").find(".RADUserManagementAccountGroupValidation").remove()
            $(".umSec2").find(".RADUserManagementAccountGroupValidation").remove()
            $(event.target).val("Delete");
            AllUserGroups.instance.ShowHideByPrivilegeType("Delete");
            if ($("#RADUserManagementEditButton").attr("createGroup") == "true") {
                AllUserGroups.instance.ReCreateDiv();
                $(".umSec1Body").empty();
                $(".umSec2Body").empty();
                for (var i = 0; i < AllUserGroups.instance.UsersInGroup.length; i++) {
                    $(".umSec1Body").append("<div class=\"RADUserManagementUsersInGroup\">" + AllUserGroups.instance.UsersInGroup[i] + "</div>");
                }
                for (var i = 0; i < AllUserGroups.instance.RolesInGroup.length; i++) {
                    $(".umSec2Body").append("<div class=\"RADUserManagementUsersInGroup\">" + AllUserGroups.instance.RolesInGroup[i] + "</div>");
                }
                $("#RADUserManagementEditButton").removeAttr("createGroup")
            }
            else if ($("#RADUserManagementEditButton").attr("createADGroup") == "true") {
                $(".umSec1Body").empty();
                $(".umSec2Body").empty();
                AllUserGroups.instance.ReCreateDiv();
                AllUserGroups.instance.ShowHideByPrivilegeType("Edit");
                $("#RADUserManagementEditButton").removeAttr("createADGroup");
                $("#RADUserManagementEditButton").val("Edit");
            }
            else {
                $("#RADUserManagementEditButton").val("Edit");
                AllUserGroups.instance.ShowHideByPrivilegeType("Edit");
                AllUserGroups.instance.ReCreateDiv();
                $(".umSec1Body").empty();
                $(".umSec2Body").empty();
                for (var i = 0; i < AllUserGroups.instance.UsersInGroup.length; i++) {
                    $(".umSec1Body").append("<div class=\"RADUserManagementUsersInGroup\">" + AllUserGroups.instance.UsersInGroup[i] + "</div>");
                }
                for (var i = 0; i < AllUserGroups.instance.RolesInGroup.length; i++) {
                    $(".umSec2Body").append("<div class=\"RADUserManagementUsersInGroup\">" + AllUserGroups.instance.RolesInGroup[i] + "</div>");
                }
            }
        }
    });

    $("#RADUserManagementScrollDownArrow").unbind().click(function (event) {
        ScrollVariable = ScrollVariable + 50;
        $(".umDynamicTiles").scrollTop(ScrollVariable);
    });
    $("#RADUserManagementScrollUpArrow").unbind().click(function (event) {
        if (ScrollVariable > 0) {
            ScrollVariable = ScrollVariable - 50;
            $(".umDynamicTiles").scrollTop(ScrollVariable);
        }
    });
    $("#search_text").keyup(function (event) {
        AllUserGroups.instance.CreateSearchText(event);
    });

}
AllUserGroups.prototype.DeleteInActiveGroups = function () {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/DeleteInactiveGroups',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        $("#ContentDiv").append("<div id=\"alertParentDivInActive\" class=\"RADUserManagementAlertPopUpParent\"></div>");
        $("#alertParentDivInActive").append("<div class=\"RADUserManagementPopupAlertUpperSuccessDiv\"></div>");
        $("#alertParentDivInActive").append("<div class=\"RADUserManagementPopUpError\"></div>")
        $("#alertParentDivInActive").append("<div class=\"RADUserErrorDivLeft RADUserSuccess\"></div>"); //revisit
        $("#alertParentDivInActive").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
        $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Success</div>")
        $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Inactive Groups have been deleted successfully</div>");
        $("#alertParentDivInActive").css("top", "-200px");
        $("#alertParentDivInActive").animate({ "top": "0px" });
        setInterval(function () {
            $("#alertParentDivInActive").remove();

        }, 2700);        
        var groupsObject = new AllUserGroups();
        groupsObject.init();
    });
}


//to create inline filter for active groups
AllUserGroups.prototype.GetActiveDirectoryGroups = function () {

    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllAdGroups', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {

        var AllActiveGroups = AllUserGroups.instance.GetInlineFilterDataForAllActiveUsers(responseText.d);
        AllUserGroups.instance.CreateInlineFilterForActiveDirectoryGroups(AllActiveGroups);
    })
}

AllUserGroups.prototype.GetInlineFilterDataForAllActiveUsers = function (AllActiveGroups) {
    var ActiveGroupDetail = [];
    for (var k = 0; k < AllActiveGroups.length; k++) {
        var myObject = {};
        myObject.id = k;
        myObject.text = AllActiveGroups[k];
        ActiveGroupDetail.push(myObject);
    }
    return ActiveGroupDetail;
}

AllUserGroups.prototype.CreateInlineFilterForActiveDirectoryGroups = function (AllActiveGroups) {

    if (AllActiveGroups != null && AllActiveGroups.length > 0) {

        var dropDownObject = {};
        dropDownObject["radEntitydefaultValue"] = 'Groups';
        dropDownObject["radEntitydropDownAllData"] = AllActiveGroups;
        dropDownObject["radEntityEnableMultiSelect"] = true;
        dropDownObject["radEntityAttributeNameText"] = 'Active Groups |^';
        dropDownObject["radWidgetSelectHandler"] = AllUserGroups.prototype.GroupSelectHandler;

        $("#RADUserManagementGroupInlineFilter").empty();        
            $("#RADUserManagementGroupInlineFilter").SelectDropDown(dropDownObject);            
       
    }



    
    //if (AllActiveGroups != null && AllActiveGroups.length > 0) {
    //    $("#RADUserManagementGroupInlineFilter").inlineFilter({
    //        filterInfo: {
    //            filterPhrase: "{0}",
    //            bindingInfo: [
    //            {
    //                identifier: "RADUserManagementGroupInlineFilter",
    //                data: AllActiveGroups, //array with objects having keys id and text

    //                multiple: true,
    //                maxResultsToShow: 1,
    //                placeholder: "Active Group",
    //                class: 'RADinlineFilterCSSGroup'
    //            }]
    //        }
    //          , selectHandler: AllUserGroups.instance.GroupSelectHandler
    //    });
    //}

}

AllUserGroups.prototype.GroupSelectHandler = function (selectedValue, e) {
    AllUserGroups.instance.ActiveUserGroups = [];
    var users;
    for (var i = 0; i < selectedValue.length; i++) {
        //users = ($("#RADUserManagementGroupInlineFilter").inlineFilter("getCurrentStateObjects")["RADUserManagementGroupInlineFilter"][i]).text;
        AllUserGroups.instance.ActiveUserGroups.push(selectedValue[i]);
    }
}

// to filter divs on the basis of search
AllUserGroups.prototype.CreateSearchText = function (event) {
    var flagSearch = false
    var ToLowerCase = []
    var InLowerCase = [];
    $(".umDynamicTiles").empty();

    for (var j = 0; j < AllUserGroups.instance.ListOfInfo.length; j++) {        
        ToLowerCase = AllUserGroups.instance.ListOfInfo[j]["GroupName"]
        InLowerCase = (ToLowerCase).toLowerCase();
        $('.umDynamicTiles').append('<div id="t' + j + '" name="' + AllUserGroups.instance.ListOfInfo[j]["GroupName"] + '"></div>');
        if (AllUserGroups.instance.ListOfInfo[j]["GroupName"] == AllUserGroups.instance.SetGroup && ((InLowerCase.indexOf($(event.target).val().toLowerCase()) != -1) || (InLowerCase.indexOf($(event.target).val().toUpperCase()) != -1))) {
            flagSearch = true
            firstGroup = AllUserGroups.instance.ListOfInfo[j]["GroupName"];
            $("#t" + j).addClass('RADUserMgmttiles').addClass('RADTileActive');
            $("#t" + j).append('<div id="group' + j + '" name="' + AllUserGroups.instance.ListOfInfo[j]["GroupName"] + '">' + AllUserGroups.instance.ListOfInfo[j]["GroupName"] + '</div>');

            $("#t" + j).closest(".RADUserMgmttiles").append('<div id="arr"></div>');
            $(".umDynamicTiles").find(".RADarrow-right").remove();
            $("#arr").addClass('fa fa-caret-right RADarrow-right').insertAfter("#t" + j);
            $("#group" + j).addClass('RADUserMgmtuser_name');
            $("#t" + j).append('<div id="group_desc' + j + '" Description="' + AllUserGroups.instance.ListOfInfo[j]["Description"] + '">' + AllUserGroups.instance.ListOfInfo[j]["Description"] + '</div>');
            $("#group_desc" + j).addClass('RADUserMgmtemail_id');
        }

        else if (((InLowerCase.indexOf($(event.target).val().toLowerCase()) != -1) || (InLowerCase.indexOf($(event.target).val().toUpperCase()) != -1))) {
            flagSearch = true
            $("#t" + j).addClass('RADUserMgmttiles')
            $("#t" + j).append('<div id="group' + j + '" name="' + AllUserGroups.instance.ListOfInfo[j]["GroupName"] + '">' + AllUserGroups.instance.ListOfInfo[j]["GroupName"] + '</div>');

            $("#group" + j).addClass('RADUserMgmtuser_name');
            $("#t" + j).append('<div id="group_desc' + j + '" Description="' + AllUserGroups.instance.ListOfInfo[j]["Description"] + '">' + AllUserGroups.instance.ListOfInfo[j]["Description"] + '</div>');
            $("#group_desc" + j).addClass('RADUserMgmtemail_id');
        }
    }
    if (flagSearch == false) {
        $('.umDynamicTiles').append('<div id="RADNouserfound" class=\"RADnouserfound"\>Sorry This Group Does Not Exist !!!</div>')
        flagSearch = true
    }
    $(".RADUserMgmttiles").unbind().click(function (event) {
        AllUserGroups.instance.ClickOnEachTile(event);

    });

}
AllUserGroups.prototype.CreateADGroup = function () {

    $(".RADUserManagementActiveAccountValidation").remove();
    $(".RADUserManagementAccountGroupValidation").remove();
    $(".RADUserManagementAccountGroupValidationInline").remove();

    var Roles = [];
    for (var i = 0; i < $(".umSec2Body").find(".RADUserManagementSelectedUsersInGroup").length; i++) {
        Roles.push($(".umSec2Body").find(".RADUserManagementSelectedUsersInGroup")[i].innerText)
    }

    // When we do an create after selecting a role by searching it, previous value are not persisiting
    // Below loop is added to add the previous selected values

    for (var j = 0; j < AllUserGroups.instance.SelectedRolesSearch.length; j++) {
        if (!Array.contains(Roles, AllUserGroups.instance.SelectedRolesSearch[j])) {
            Roles.push(AllUserGroups.instance.SelectedRolesSearch[j]);
        }
    }


    if (AllUserGroups.instance.ActiveUserGroups == null || $("#RADUserManagementGroupFirstAccount").html() == "Account Name" || Roles == "") {
        if ($("#RADUserManagementGroupFirstAccount").html() == "Account Name") {
            $("<div  class=\"RADUserManagementActiveAccountValidation\">!</div>").insertAfter($(".RADUserManagementAddNewGroupAccount"));
            $(".RADUserManagementActiveAccountValidation").attr("title", "Select Atleast One Account")
        }
        if (Roles == "") {
            $("<div  class=\"RADUserManagementAccountGroupValidation\">!</div>").insertAfter($(".umSec2Head"));
            $(".RADUserManagementAccountGroupValidation").attr("title", "Select Atleast One Role")
        }
        if (AllUserGroups.instance.ActiveUserGroups == null) {
            $("<div id=\"RADGroupRequired\"  class=\"RADUserManagementAccountGroupValidationInline\">!</div>").insertAfter($(".RADUserManagementGroupInlineFilter"));
            $(".RADUserManagementAccountGroupValidationInline").attr("title", "Select Atleast One Active Group")
        }
    }
    else {

        $(".RADUserManagementActiveAccountValidation").remove();
        $(".RADUserManagementAccountGroupValidation").remove();
        $(".RADUserManagementAccountGroupValidationInline").remove();
        $(event.target).removeAttr("createADGroup");
        $(event.target).val("Edit");
        AllUserGroups.instance.ShowHideByPrivilegeType("Edit");

        var dialog = $('#lab1').data("ddn-SelectDropDown");
        ActiveAcc = dialog._GetSingleSelectedDataFromWidget('#lab1');
        selectedAccount = ActiveAcc;

        var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";

        $.ajax({
            url: url + '/AddAdGroups',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ groups: AllUserGroups.instance.ActiveUserGroups, accountName: selectedAccount, Roles: Roles })
            //data: JSON.stringify({ groups: AllUserGroups.instance.ActiveUserGroups, accountName: $("#RADUserManagementGroupFirstAccount").html(), Roles: Roles })

        }).then(function (responseText) {
            if (responseText.d == "") {
                $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
                $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperSuccessDiv\"></div>");
                $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
                $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserSuccess\"></div>"); //revisit
                $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
                $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Result</div>")
                $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\"> Group Has Been Created Successfully!!! </div>");
                $("#alertParentDiv").css("top", "-200px");
                $("#alertParentDiv").animate({ "top": "0px" });
                setInterval(function () { $("#alertParentDiv").remove() }, 3000);
                var usersObject = new AllUserGroups();
                usersObject.init();

            }
            else {
                $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
                $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
                $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
                $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserFailure\"></div>"); //revisit
                $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
                $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Result</div>")
                $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Group Creation Failed</div>");
                $("#alertParentDiv").css("top", "-200px");
                $("#alertParentDiv").animate({ "top": "0px" });
                setInterval(function () { $("#alertParentDiv").remove() }, 3000);
                var usersObject = new AllUserGroups();
                usersObject.init();

            }
        });
    }
}
AllUserGroups.prototype.CreateGroup = function (createGroup) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/CreateGroup',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ groupDetails: createGroup })

    }).then(function (responseText) {
        if (responseText.d == "") {
            $(".RADUserManagementAuditButton").show()
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperSuccessDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserSuccess\"></div>");//revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Success</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Group has been created successfully</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 3000);
            AllUserGroups.instance.getGroupInfoNewGroup(groupName)
            AllUserGroups.instance.GetAllGroupsCreateTileOnAdd(groupName)

        }
        else {
            $(".RADUserManagementAuditButton").show()
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserFailure\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Result</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Group Creation Failed</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 3000);
            var usersObject = new AllUserGroups();
            usersObject.init();
        }
    });
}
AllUserGroups.prototype.getGroupInfoNewGroup = function (groupName) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetGroupInfo', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ groupName: groupName })

    }).then(function (responseText) {
        var groupInfo = JSON.parse(responseText.d);
        var groupName = groupInfo["GroupName"];
        $(".umRightHeader").find(".RADUserMgmtuser_label").html(groupName);
        $(".RADUserMgmtuser_label").attr('title', groupName);
        $("#RADUserManagementAddNewGroupName").remove()
        $("#RADUserManagementAddNewGroupDesc").remove()
        $("#RADUserManagementAddNewGroupAccount").remove()
        $(".umRightHeader").find(".RADUserMgmtuser_label").css({ "display": "block" });
        $("#lab1").html(groupInfo["AccountName"]);
        var usersInAGroup = [];
        var userRoles = [];
        usersInAGroup = groupInfo["UserName"]
        userRoles = groupInfo["Roles"];
        AllUserGroups.instance.UsersInGroup = groupInfo["UserName"];
        AllUserGroups.instance.RolesInGroup = groupInfo["Roles"];
        $(".umSec1Body").empty();
        $(".umSec2Body").empty();        
        AllUserGroups.instance.ReCreateDiv();
        $(".RADUserManagementUsersInGroup").unbind().click(function (event) {
            if ($("#RADUserManagementEditButton").attr("value") == "Save")
                AllUserGroups.instance.selectUnSelectedItems(event);
            AllUserGroups.instance.GetAllGroupsCreateTileOnAdd(name)
        });
    });
}
AllUserGroups.prototype.getAllAccounts = function (isLdapEnabled) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllAccounts',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        var result = JSON.parse(responseText.d);

        var ArrayOfActiveAccounts = [];
        for (var i = 0; i < result.length; i++) {
            var obj = {};
            obj["id"] = i;
            obj["text"] = result[i].AccountName;
            ArrayOfActiveAccounts.push(obj);
        }

        var dropDownObject = {};
        dropDownObject["radEntitydefaultValue"] = 'Accounts';
        dropDownObject["radEntitydropDownAllData"] = ArrayOfActiveAccounts;
        dropDownObject["radEntityEnableMultiSelect"] = false;
        dropDownObject["radEntityAttributeNameText"] = 'Accounts |^';
        if (isLdapEnabled)
            dropDownObject["radWidgetSelectHandler"] = '';
        else
            dropDownObject["radWidgetSelectHandler"] = AllUserGroups.prototype.getAllUsersForAccountCreate;
        $('#lab1').SelectDropDown(dropDownObject);

        //for (var i = 0; i < result.length; i++)
        //    $('#RADUserManagementAddNewGroupAccount').append("<div class=\"RADUserManagementEachAccountGroupDiv\">" + result[i].AccountName + "</div>");
        //$(".RADUserManagementGroupFirstAccount").unbind().click(function (event) { //to show the dropdown
        //    $("#RADUserManagementAddNewGroupAccount").css({ "height": "auto" })
        //});
        //$(".RADUserManagementEachAccountGroupDiv").unbind().click(function (event) {//to show the selected text in dropdown
        //    $("#RADUserManagementGroupFirstAccount").html($(event.target).html());
        //    $("#RADUserManagementAddNewGroupAccount").css({ "height": "30px" })
        //    var accountName = $(event.target).html();
        //    AllUserGroups.instance.getAllUsersForAccountCreate(accountName);
        //});
    });
}
AllUserGroups.prototype.getAllUsersForAccountCreate = function (accountName) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllUserForAccount', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ accountName: accountName[0] })
    }).then(function (responseText) {
        var allGroupInfo = JSON.parse(responseText.d);
        $(".umSec1Body").empty();

        AllUserGroups.instance.AllUserList = allGroupInfo;

        for (var i = 0; i < allGroupInfo.length; i++) {
            $(".umSec1Body").append("<div class=\"RADUserManagementUsersInGroup\">" + allGroupInfo[i] + "</div>");
        }

        $(".RADUserManagementUsersInGroup").unbind().click(function (event) {
            if ($("#RADUserManagementEditButton").attr("value") == "Save")
                AllUserGroups.instance.selectUnSelectedItems(event);
        });
    });
}
AllUserGroups.prototype.getAllUsersForAccount = function (accountName, selectedUsersInAccount) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllUserForAccount',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ accountName: accountName })
    }).then(function (responseText) {
        var allGroupInfo = JSON.parse(responseText.d);
        $(".umSec1Body").empty();
        AllUserGroups.instance.SelectedUserArray = selectedUsersInAccount
        AllUserGroups.instance.AllUserList = allGroupInfo;
        for (var i = 0; i < selectedUsersInAccount.length; i++) {
            $(".umSec1Body").append("<div class=\"RADUserManagementUsersInGroup RADUserManagementSelectedUsersInGroup EditModeSelect\">" + selectedUsersInAccount[i] + "</div>");
        }
        for (var i = 0; i < allGroupInfo.length; i++) {
            if (selectedUsersInAccount != null) {
                if (selectedUsersInAccount.indexOf(allGroupInfo[i]) != -1) {

                }
                else {
                    $(".umSec1Body").append("<div class=\"RADUserManagementUsersInGroup EditModeSelect\">" + allGroupInfo[i] + "</div>");
                }
            }
            else {
                $(".umSec1Body").append("<div class=\"RADUserManagementUsersInGroup EditModeSelect\">" + allGroupInfo[i] + "</div>");
            }
        }
        $(".RADUserManagementUsersInGroup").unbind().click(function (event) {
            if ($("#RADUserManagementEditButton").attr("value") == "Save")
                AllUserGroups.instance.selectUnSelectedItems(event);
        });
    });
}
AllUserGroups.prototype.CreategetAllRoles = function () {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllRoles',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',

    }).then(function (responseText) {
        var getAllRoles = JSON.parse(responseText.d);
        var RoleArray = []        
        for (var i = 0; i < getAllRoles.length; i++) {
            RoleArray.push(getAllRoles[i]["RoleName"])
        }

        AllUserGroups.instance.AllRolesList = RoleArray        
        $(".umSec2Body").empty();

        for (var i = 0; i < getAllRoles.length; i++) {

            $(".umSec2Body").append("<div class=\"RADUserManagementUsersInGroup\">" + getAllRoles[i]["RoleName"] + "</div>");

        }
        $(".RADUserManagementUsersInGroup").unbind().click(function (event) {
            if ($("#RADUserManagementEditButton").attr("value") == "Save")
                AllUserGroups.instance.selectUnSelectedItems(event);
        });
    });
}

AllUserGroups.prototype.getAllRoles = function (selectedRolesInAccount) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllRoles',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',

    }).then(function (responseText) {
        var getAllRoles = JSON.parse(responseText.d);
        var RoleArray = []
        
        for (var i = 0; i < getAllRoles.length; i++) {
            RoleArray.push(getAllRoles[i]["RoleName"])
        }
        AllUserGroups.instance.SelectedRolesArray = selectedRolesInAccount
        AllUserGroups.instance.AllRolesList = RoleArray
        
        $(".umSec2Body").empty();
        for (var i = 0; i < selectedRolesInAccount.length; i++) {
            $(".umSec2Body").append("<div class=\"RADUserManagementUsersInGroup RADUserManagementSelectedUsersInGroup\">" + selectedRolesInAccount[i] + "</div>");
        }
        for (var i = 0; i < getAllRoles.length; i++) {
            if (selectedRolesInAccount != null) {
                if (selectedRolesInAccount.indexOf(getAllRoles[i]["RoleName"]) != -1) {

                }
                else {
                    $(".umSec2Body").append("<div class=\"RADUserManagementUsersInGroup\">" + getAllRoles[i]["RoleName"] + "</div>");
                }
            }
            else {
                $(".umSec2Body").append("<div class=\"RADUserManagementUsersInGroup\">" + getAllRoles[i]["RoleName"] + "</div>");
            }

        }
        $(".RADUserManagementUsersInGroup").unbind().click(function (event) {
            if ($("#RADUserManagementEditButton").attr("value") == "Save")
                AllUserGroups.instance.selectUnSelectedItems(event);
        });
    });
}

AllUserGroups.prototype.OnDeleteClick = function () {
    var deleteUser = $(".RADTileActive").children()[0].innerText;
    
    $("#ContentDiv").append("<div id=\"alertParentDivdel\" class=\"RADUserManagementAlertPopUpParent\"></div>");
    $("#alertParentDivdel").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
    $("#alertParentDivdel").append("<div class=\"RADUserManagementPopUpError\"></div>")
    $("#alertParentDivdel").append("<div class=\"RADUserErrorDivLeft RADUserAlert\"></div>"); //revisit
    $("#alertParentDivdel").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
    $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Alert</div>")
    $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Are You Sure You want to delete  " + (deleteUser).toUpperCase() + " ?</div>");
    $("#alertParentDivdel").append("<div class=\"RADUserManagementPopUpErrorFooter\"></div>");
    $(".RADUserManagementPopUpErrorFooter").append("<div class=\"RADUserManagementPopUpErrorYes\">Yes</div><div class=\"RADUserManagementPopUpErrorNo\">No</div>")
    $("#alertParentDivdel").css("top", "-200px");
    $("#alertParentDivdel").animate({ "top": "0px" });
    $(".RADUserManagementPopUpErrorYes").unbind().click(function (event) {
        $("#alertParentDivdel").remove();

        $("#RADUserManagementEditButton").val("Edit")
        var deleteUser = $(".RADTileActive").children()[0].innerText;
        AllUserGroups.instance.deleteGroup(deleteUser);
        //$("#" + RADUserManagement.contentBodyId).empty();
        $(".usermgmt").remove();

    });
    $(".RADUserManagementPopUpErrorNo").unbind().click(function (event) {
        $("#alertParentDivdel").remove();
    });

}


AllUserGroups.prototype.deleteGroup = function (groupName) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/DeleteGroup',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ groupName: groupName })

    }).then(function (responseText) {
        var res;
        res = JSON.parse(responseText.d);
        if (res == true) {
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperSuccessDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserSuccess\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Success</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Group has been deleted successfully</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () {
                $("#alertParentDiv").remove();

            }, 3000);
            var groupsObject = new AllUserGroups();
            groupsObject.init();
        }
        else {
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserFailure\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Result</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Group Deletion Failed</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 3000);
        }
    });
}
AllUserGroups.prototype.selectUnSelectedItems = function (event) {

    if ($(event.target).hasClass("RADUserManagementSelectedUsersInGroup")) {
        $(event.target).removeClass("RADUserManagementSelectedUsersInGroup")
        AllUserGroups.instance.SelectedUserArray.splice(AllUserGroups.instance.SelectedUserArray.indexOf($(this).text()), 1);
    }
    else
        $(event.target).addClass("RADUserManagementSelectedUsersInGroup")
    AllUserGroups.instance.SelectedUserArray.push($(this).text())
}

AllUserGroups.prototype.UpdateGroup = function (updateGroupDetail) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/UpdateGroup',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ groupInfo: updateGroupDetail })

    }).then(function (responseText) {
        if (responseText.d == "") {
            AllUserGroups.instance.ShowHideByPrivilegeType("Add");
            $(".searchBarUm").show();
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperSuccessDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserSuccess\"></div>");//revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Success</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Group has been updated successfully</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 3000);
            AllUserGroups.instance.ReCreateEDITDiv()

        }
        else {
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserFailure\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Result</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Group updation Failed</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 3000);
            AllUserGroups.instance.ReCreateEDITDiv()

        }
    });

}


AllUserGroups.prototype.ReCreateDiv = function () {
    $("#RADUserManagementAuditButton").show()
    AllUserGroups.instance.ShowHideByPrivilegeType("Delete");
    AllUserGroups.instance.ShowHideByPrivilegeType("Edit");
    if ($("#RADUserManagementEditButton").attr("value") == "Save")
        $("#RADUserManagementEditButton").attr("value", "Edit");
    if ($("#RADUserManagementEditButton").attr("value") == "Cancel")
        $("#RADUserManagementEditButton").attr("value", "Delete");
    if ($("#RADUserManagementAddNewGroupName") != null)
        $("#RADUserManagementAddNewGroupName").remove();
    if ($("#RADUserManagementAddNewGroupDesc") != null)
        $("#RADUserManagementAddNewGroupDesc").remove();
    $("#RADUserManagementAddNewGroupAccount").remove();

    $(".umRightHeader").empty();
    $(".umRightHeader").append("<div id=\"RADUserManagaementUserLabel\" class=\"RADUserManagaementLabel\"><label id=\"lblUserLabel\" >Group Name :</label></div>");
    $(".umRightHeader").append("<div id=\"lab\" class=\"RADUserMgmtuser_label\"></div>");
    $(".umRightHeader").append("<div id=\"RADUserManagaementAccountLabeDiv\" class=\"RADUserManagaementLabel\"><label id=\"lblAccountLabel\" >Account :</label></div>");
    $(".umRightHeader").append("<div id=\"lab1\" class=\"RADUSERManagementAccountValue\"></div>");

    $(".RADUserManagementGroupInlineFilter").css({ "display": "none" })

    for (var i = 0; i < AllUserGroups.instance.UsersInGroup.length; i++) {
        $(".umSec1Body").append("<div class=\"RADUserManagementUsersInGroup\">" + AllUserGroups.instance.UsersInGroup[i] + "</div>");
    }
    for (var i = 0; i < AllUserGroups.instance.RolesInGroup.length; i++) {
        $(".umSec2Body").append("<div class=\"RADUserManagementUsersInGroup\">" + AllUserGroups.instance.RolesInGroup[i] + "</div>");
    }
    
    var currentGroupName;
    if (groupNameReCreateDiv) {
        currentGroupName = groupName;
        groupNameReCreateDiv = false;
    }
    else {
        currentGroupName = $(".umDynamicTiles").find(".RADTileActive").children()[0].innerText;
    }
    AllUserGroups.instance.getGroupInfo(currentGroupName)
}

AllUserGroups.prototype.ReCreateADDiv = function () {
    $("#RADUserManagementAuditButton").show()
    $(".umSec1Head").css({ "display": "inline-block" });
    $(".umSec1Body").css({ "display": "inline-block" });
    $(".umSec2").css({ "margin-left": "326px" });
    for (var i = 0; i < AllUserGroups.instance.UsersInGroup.length; i++) {
        $(".umSec1Body").append("<div class=\"RADUserManagementUsersInGroup\">" + AllUserGroups.instance.UsersInGroup[i] + "</div>");
    }
    for (var i = 0; i < AllUserGroups.instance.RolesInGroup.length; i++) {
        $(".umSec2Body").append("<div class=\"RADUserManagementUsersInGroup\">" + AllUserGroups.instance.RolesInGroup[i] + "</div>");
    }

    $($(".umRightHeader").children()[1]).css({ "display": "block" });
    $($(".umRightHeader").children()[2]).css({ "display": "block" });
    $($(".umRightHeader").children()[3]).css({ "display": "block" });
    $($(".umRightHeader").children()[4]).css({ "display": "block" });
    $(".RADUserManagaementLabel").css({ "display": "block" });
    $($(".umRightHeader").children()[0]).remove();
    $($(".umRightHeader").children().last()).remove();
    $(".umSec1").empty();
    $(".umSec2").empty();
    for (var i = 0; i < AllUserGroups.instance.UsersInGroup.length; i++) {
        $(".umSec1Body").append("<div class=\"RADUserManagementUsersInGroup\">" + AllUserGroups.instance.UsersInGroup[i] + "</div>");
    }
    for (var i = 0; i < AllUserGroups.instance.RolesInGroup.length; i++) {
        $(".umSec2Body").append("<div class=\"RADUserManagementUsersInGroup\">" + AllUserGroups.instance.RolesInGroup[i] + "</div>");
    }
    AllUserGroups.instance.GetAllGroupsCreateTileOnAdd(name)

}
AllUserGroups.prototype.ReCreateEDITDiv = function () {
    AllUserGroups.instance.ShowHideByPrivilegeType("Add");    
    $(".searchBarUm").show();
    if ($("#RADUserManagementEditButton").attr("value") == "Save")
        $("#RADUserManagementEditButton").attr("value", "Edit");
    if ($("#RADUserManagementAddNewGroupName") != null)
        $("#RADUserManagementAddNewGroupName").remove();
    if ($("#RADUserManagementAddNewGroupDesc") != null)
        $("#RADUserManagementAddNewGroupDesc").remove();
    $("#RADUserManagementAddNewGroupAccount").remove();
    var name = $(".umRightHeader").children()[1].innerHTML
    AllUserGroups.instance.getGroupInfo(name)
}

AllUserGroups.prototype.GetAllGroupsCreateTileOnAdd = function (name) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllGroups',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        var b;
        ListOfGroups = JSON.parse(responseText.d);
        var firstGroup = "";
        AllUserGroups.instance.ListOfInfoOfTile = ListOfGroups;
        $(".umDynamicTiles").find(".RADTileActive").remove()
        $(".umDynamicTiles").empty()
        for (var j = 0; j < ListOfGroups.length; j++) {
            if (ListOfGroups[j]["GroupName"] == name) {
                $(".umDynamicTiles").prepend('<div  title="' + ListOfGroups[j]["GroupName"] + '"id="tADD' + j + '" name="' + ListOfGroups[j]["GroupName"] + '"></div>');
                firstGroup = ListOfGroups[j]["GroupName"];
                $("#tADD" + j).addClass('RADUserMgmttiles').addClass('RADTileActive');
                $("#tADD" + j).append('<div  title="' + ListOfGroups[j]["GroupName"] + '" id="groupADD' + j + '" name="' + ListOfGroups[j]["GroupName"] + '">' + ListOfGroups[j]["GroupName"] + '</div>');
                $("#tADD" + j).closest(".RADUserMgmttiles").append('<div id="arr"></div>');
                $(".umDynamicTiles").find(".RADarrow-right").remove();
                $("#arr").addClass('fa fa-caret-right RADarrow-right').insertAfter("#tADD" + j);
                $("#groupADD" + j).addClass('RADUserMgmtuser_name');
                $("#tADD" + j).append('<div  title="' + ListOfGroups[j]["Description"] + '" id="group_descADD' + j + '" Description="' + ListOfGroups[j]["Description"] + '">' + ListOfGroups[j]["Description"] + '</div>');
                $("#group_descADD" + j).addClass('RADUserMgmtemail_id');
            }
            else {
                $('.umDynamicTiles').append('<div title="' + ListOfGroups[j]["GroupName"] + '" id="t' + j + '" name="' + ListOfGroups[j]["GroupName"] + '"></div>');
                $("#t" + j).addClass('RADUserMgmttiles')
                $("#t" + j).append('<div title="' + ListOfGroups[j]["GroupName"] + '" id="group' + j + '" name="' + ListOfGroups[j]["GroupName"] + '">' + ListOfGroups[j]["GroupName"] + '</div>');

                $("#group" + j).addClass('RADUserMgmtuser_name');
                $("#t" + j).append('<div title="' + ListOfGroups[j]["Description"] + '" id="group_desc' + j + '" Description="' + ListOfGroups[j]["Description"] + '">' + ListOfGroups[j]["Description"] + '</div>');
                $("#group_desc" + j).addClass('RADUserMgmtemail_id');
            }
        }
        $("#RADUserManagementAuditButton").show()
        $(".RADUserMgmttiles").unbind().click(function (event) {
            AllUserGroups.instance.ClickOnEachTile(event);

        });
    });
}
AllUserGroups.prototype.SearchAction = function (event) {

    $("#search_text").css({ 'display': 'block' })
}
AllUserGroups.prototype.AuditScreen = function (event) {
    var auditgroupname = $(".RADTileActive").children()[0].innerText
    $(".usermgmt").prepend("<div class=\"RADAuditScreenMain\" id=\"RADAuditScreenMain\"></div>");
    
    $("#RADAuditScreenMain").append("<div class=\"RADAuditHeader\" id=\"RADAuditHeader\"></div>")
    $("#RADAuditHeader").append("<div class=\"RADAuditHeaderHeading\" id=\"RADAuditHeaderHeading\">User Audit</div>")
    $("#RADAuditHeader").append("<div class=\"RADAuditCancelButton\" id=\"RADAuditCancelButton\">X</div>")
    $("#RADAuditScreenMain").append("<div class=\"RADAuditHeaderHeadingOfUserName\" id=\"RADAuditHeaderHeadingOfUserName\">GROUP NAME :</div>")
    $("#RADAuditHeaderHeadingOfUserName").append("<div class=\"RADAuditHeaderValueOfUserName\" id=\"RADAuditHeaderValueOfUserName\">" + auditgroupname + "</div>")
    $("#RADAuditScreenMain").append("<div class=\"RADBodyOfAudit\" id=\"RADNeoUserAuditScreen\"></div>")

    var effect = 'slide';
    var options = { direction: 'Right' };
    var duration = 500;
    $("#RADAuditScreenMain").toggle(effect, options, duration);
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAuditDetailsForGroup', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ groupName: auditgroupname })
    }).then(function (responseText) {
        var response = JSON.parse(responseText.d);
        $("#RADNeoUserAuditScreen").append("<table id=\"RADUserAuditTable\" class=\"RADUserAuditTable\"><tbody><tr class=\"RADAuditUserHeaderRow\"><td>Attribute</td><td>Old Value</td><td>New Value</td><td>Updated By</td><td>Updated On</td></tr></tbody></table>")
        for (var i = 0; i < response.length; i++) {            
            $("#RADUserAuditTable").append("<tr class=\"RADUserAuditDataRow\"><td title=" + response[i]["Attribute Name"] + ">" + response[i]["Attribute Name"] + "</td><td title=" + response[i]["Old Value"] + ">" + response[i]["Old Value"] + "</td><td title=" + response[i]["New Value"] + ">" + response[i]["New Value"] + "</td><td title=" + response[i]["Updated By"] + ">" + response[i]["Updated By"] + "</td><td>" + moment(response[i]["Updated On"]).format("YYYY/MM/DD HH:mm") + "</td></tr>")
        }

        $("#RADAuditCancelButton").unbind().click(function (event) {
            AllUserGroups.instance.AuditScreenCancel(event)
        });
    });
}
AllUserGroups.prototype.AuditScreenCancel = function (event) {    
    $("#RADAuditScreenMain").hide("drop", { direction: "right" }, "slow", function () { $("#RADAuditScreenMain").remove(); });
}

AllUserGroups.prototype.SetGridHeight = function (eventtype, id) {
    $("#" + id + "_body_Div").height($("#RADNeoUserAuditScreen").height() - $("#" + id + "_upperHeader_Div").height() - $("#" + id + "_footer_Div").height())
    $("#" + id + "_configutation").css({ "display": "none" });
    $("#" + id + "_upperHeader_Div").css({ "display": "none" });

}

AllUserGroups.prototype.OnModifyGroup = function () {

    var updateGroupDetail = new Object();
    var Users = [];
    var Roles = [];
    var ArrUsers = [];
    var ArrRoles = [];
    updateGroupDetail.AccountName = $("#lab1").html();
    ArrUsers = ($(".umSec1Body").find(".RADUserManagementSelectedUsersInGroup"))
    ArrRoles = ($(".umSec2Body").find(".RADUserManagementSelectedUsersInGroup"));
    $(ArrUsers).each(function (i, e) {
        Users.push($(e).text());
    })

    $(ArrRoles).each(function (i, e) {
        Roles.push($(e).text());
    })

    // When we do an create after selecting a role/user by searching it, previous value are not persisiting
    // Below loop is added to add the previous selected values

    for (var j = 0; j < AllUserGroups.instance.SelectedUserSearch.length; j++) {
        if (!Array.contains(Users, AllUserGroups.instance.SelectedUserSearch[j])) {
            Users.push(AllUserGroups.instance.SelectedUserSearch[j]);
        }
    }

    for (var j = 0; j < AllUserGroups.instance.SelectedRolesSearch.length; j++) {
        if (!Array.contains(Roles, AllUserGroups.instance.SelectedRolesSearch[j])) {
            Roles.push(AllUserGroups.instance.SelectedRolesSearch[j]);
        }
    }

    updateGroupDetail.UserName = Users;
    updateGroupDetail.Roles = Roles;
    updateGroupDetail.GroupName = $(".RADTileActive").children()[0].innerText;
    updateGroupDetail.Description = $(".RADTileActive").children()[1].innerText;
    if (updateGroupDetail.GroupName == "" || updateGroupDetail.Roles == 0) {
        $(".umRightHeader").css({ "margin-left": "29%" });
        if (updateGroupDetail.Roles == "" || ($(".RADUserMgmtLabelHeader").find(".RADFirstNameValidation") == 0)) {

            $("<div  class=\"RADUserManagementAccountGroupValidation\">!</div>").insertAfter($(".umSec2Head"));
            $(".RADUserManagementAccountGroupValidation").attr("title", "Select Atleast One  Role")
        }
        if (updateGroupDetail.GroupName == "" || ($(".RADUserMgmtLabelHeader").find(".RADLastNameValidation") == 0)) {
            $(".RADUserMgmtLabelHeader").append("<div id=\"RADLastNameValidation\" class=\"RADLastNameValidation\">!</div>")
            $("#RADLastNameValidation").attr("title", "GroupName Can't Be Empty")
        }
        if (updateGroupDetail.Description == "" || ($(".RADUserMgmtLabelHeader").find(".RADEmailValidation") == 0)) {
            $(".RADUserMgmtLabelHeader").append("<div id=\"RADEmailValidation\" class=\"RADEmailValidation\">!</div>")
            $("#RADEmailValidation").attr("title", "Group Description Can't Be Empty")
        }
    }

    else {
        $("#RADUserManagementDeleteButton").val("Delete");
        $("#RADUserManagementEditButton").val("Edit");
        $(".RADUserManagementAuditButton").show();
        AllUserGroups.instance.ShowHideByPrivilegeType("Add");
        AllUserGroups.instance.ShowHideByPrivilegeType("Delete");
        AllUserGroups.instance.ShowHideByPrivilegeType("Edit");
        $(".searchBarUm").show();
        $(".RADUserNameValidation").remove();
        $(".RADFirstNameValidation").remove();
        $(".RADLastNameValidation").remove();
        $(".RADEmailValidation").remove();
        $(".RADAccountValidation").remove();
        $(".RADUserManagementAccountGroupValidation").remove();
        var myString = JSON.stringify(updateGroupDetail);
        AllUserGroups.instance.UpdateGroup(myString);
        $("#Groupsearch_text").remove()
        $("#RADUMGroupSearch").remove()
        $("#Rolesearch_text").remove()
        $("#RADUMRoleSearch").remove()
        $(".RADUMGroupSearchMAinParentDiv").remove()
        $(".RADUMGroupSearchMAinParentDivSec2").remove()
    }
}

AllUserGroups.prototype.OnEditGroup = function () {

    if ($(".RADTileActive").children()[0] == null) {

        if ($("#alertParentDivEdit") != null)
        {
            $("#ContentDiv").find("#alertParentDivedit").remove();
        }
        $("#ContentDiv").append("<div id=\"alertParentDivEdit\" class=\"RADUserManagementAlertPopUpParent\"></div>");
        $("#alertParentDivEdit").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
        $("#alertParentDivEdit").append("<div class=\"RADUserManagementPopUpError\"></div>")
        $("#alertParentDivEdit").append("<div class=\"RADUserErrorDivLeft RADUserAlert\"></div>"); //revisit
        $("#alertParentDivEdit").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
        $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Alert</div>")
        $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Please select group to edit </div>");
        $("#alertParentDivEdit").append("<div class=\"RADUserManagementPopUpErrorFooter\"></div>");
        $(".RADUserManagementPopUpErrorFooter").append("<div class=\"RADUserManagementPopUpErrorYes\">OK</div>")
        $("#alertParentDivEdit").css("top", "-200px");
        $("#alertParentDivEdit").animate({ "top": "0px" });
        $(".RADUserManagementPopUpErrorYes").unbind().click(function (event) {
            $("#alertParentDivEdit").remove();
        });

    }
    else {
        AllUserGroups.instance.SelectedUserSearch = [];
        AllUserGroups.instance.SelectedRolesSearch = [];
        $(event.target).val("Save");
        var accountName = $("#lab1").html(); //get the account name            
        var selectedUsersInAccount = [];
        var selectedRolesInAccount = [];
        $("<div id=\"RADUMGroupSearchMAinParentDiv\"  class=\"RADUMGroupSearchMAinParentDiv\"></div>").insertAfter($('.umSec1Head'))
        $(".RADUMGroupSearchMAinParentDiv").append("<div id=\"RADUMGroupSearch\"  class=\"fa fa-search searchIconGridViewForUserGroup\"></div>")
        $(".RADUMGroupSearchMAinParentDiv").append("<input type=\"text\" name=\"search_text\" id=\"Groupsearch_text\" placeholder=\"Search\"></div>")
        $("<div id=\"RADUMGroupSearchMAinParentDivSec2\"  class=\"RADUMGroupSearchMAinParentDivSec2\"></div>").insertAfter($('.umSec2Head'))
        $(".RADUMGroupSearchMAinParentDivSec2").append("<div id=\"RADUMRoleSearch\"  class=\"fa fa-search searchIconGridViewForUserRoles\"></div>")
        $(".RADUMGroupSearchMAinParentDivSec2").append("<input type=\"text\" name=\"search_text\" id=\"Rolesearch_text\" placeholder=\"Search\"></div>")
        for (var i = 0; i < $(".umSec1Body").children().length; i++) {//get the selectedUsers 
            selectedUsersInAccount.push($(".umSec1Body").children()[i].innerText);
        }
        for (var i = 0; i < $(".umSec2Body").children().length; i++) {//get the selectedUsers 
            selectedRolesInAccount.push($(".umSec2Body").children()[i].innerText);
        }
        AllUserGroups.instance.getAllUsersForAccount(accountName, selectedUsersInAccount);
        AllUserGroups.instance.getAllRoles(selectedRolesInAccount);
    }
}

AllUserGroups.prototype.OnCreateGroup = function () {

    AllUserGroups.instance.ShowHideByPrivilegeType("Add");
    $(".searchBarUm").show();
    var createGroup = new Object();
    var Users = [];
    var Roles = [];
    var ArrUsers = [];
    var ArrRoles = [];
    groupName = $("#RADUserManagementAddNewGroupName").val();
    groupNameReCreateDiv = true;
    var groupDesc = $("#RADUserManagementAddNewGroupDesc").val();
    var accountName = $(".RADplaceHolderText").html().trim();
    ArrUsers = ($(".umSec1Body").find(".RADUserManagementSelectedUsersInGroup"))
    ArrRoles = ($(".umSec2Body").find(".RADUserManagementSelectedUsersInGroup"));
    $(ArrUsers).each(function (i, e) {
        Users.push($(e).text());
    })

    $(ArrRoles).each(function (i, e) {
        Roles.push($(e).text());
    })

    // When we do an create after selecting a role/user by searching it, previous value are not persisiting
    // Below loop is added to add the previous selected values

    for (var j = 0; j < AllUserGroups.instance.SelectedUserSearch.length; j++) {
        if (!Array.contains(Users, AllUserGroups.instance.SelectedUserSearch[j])) {
            Users.push(AllUserGroups.instance.SelectedUserSearch[j]);
        }
    }

    for (var j = 0; j < AllUserGroups.instance.SelectedRolesSearch.length; j++) {
        if (!Array.contains(Roles, AllUserGroups.instance.SelectedRolesSearch[j])) {
            Roles.push(AllUserGroups.instance.SelectedRolesSearch[j]);
        }
    }


    createGroup.GroupName = groupName;
    createGroup.Description = groupDesc;
    createGroup.AccountName = accountName;
    createGroup.UserName = Users;
    createGroup.Roles = Roles;
    if (createGroup.GroupName == "" || groupDesc == "" || accountName == "Account Name" || createGroup.Roles == 0) {
        if ($(".RADUserManagementEditButton").val() == "Save") {
            if (createGroup.GroupName == "") {
                if (($(".umRightHeader").find("#RADUserGroupNameValidation").length == 0)) {
                    $("<div id=\"RADUserGroupNameValidation\" class=\"RADUserGroupNameValidation\">!</div>").insertAfter($("#RADUserManagementAddNewGroupName"))
                    $("#RADUserGroupNameValidation").attr("title", "GroupName Can't Be Empty")
                }
            }
            if (groupDesc == "") {
                if (($(".umRightHeader").find("#RADGroupDesceValidation").length == 0)) {
                    $("<div id=\"RADGroupDesceValidation\" class=\"RADGroupDesceValidation\">!</div>").insertAfter($("#RADUserManagementAddNewGroupDesc"))
                    $("#RADGroupDesceValidation").attr("title", "GroupName Can't Be Empty")
                }
            }
            if (accountName == "Account Name") {
                if (($(".umRightHeader").find("#RADAccountValidation").length == 0)) {
                    $("<div id=\"RADAccountValidation\" class=\"RADAccountValidationGroup\">!</div>").insertAfter($("#RADUserManagementAddNewGroupAccount"))
                    $("#RADAccountValidation").attr("title", "Select Atleast One Account")
                }
            }
            if (createGroup.Roles == 0 || ($(".umSec1Head").find(".RADUserManagementAccountGroupValidation").length == 0)) {
                if (createGroup.Roles == 0) {
                    $("<div  class=\"RADUserManagementAccountGroupValidation\">!</div>").insertAfter($(".umSec2Head"));
                    $(".RADUserManagementAccountGroupValidation").attr("title", "Select Atleast One  Role")
                }
            }
        }
    }
    else {
        $(event.target).removeAttr("createGroup");
        $("#RADGroupDesceValidation").remove()
        $("#RADUserGroupNameValidation").remove()
        $("#RADAccountValidation").remove()
        $(".RADUMGroupSearchMAinParentDiv").remove()
        $(".RADUMGroupSearchMAinParentDivSec2").remove()
        $("umSec1Head").find("#RADUserManagementAccountGroupValidation").remove()
        $("umSec2Head").find("#RADUserManagementAccountGroupValidation").remove()
        $("#Groupsearch_text").remove()
        $("#RADUMGroupSearch").remove()
        $("#Rolesearch_text").remove()
        $("#RADUMRoleSearch").remove()
        var createGroupToString = JSON.stringify(createGroup);
        AllUserGroups.instance.CreateGroup(createGroupToString);
    }
}

AllUserGroups.prototype.AddSpinnerOnContentBody = function () {
    $("#contentBody").addClass('spinner');
    $("#contentBody").css({ 'opacity': '0.5' });
}

AllUserGroups.prototype.RemoveSpinnerOnContentBody = function () {
    $("#contentBody").removeClass('spinner');
    $("#contentBody").css({ 'opacity': '1' });
}
var AllUserRoles = function () {
    // this.someObjectVariable = "anything";
};
var rolePrivileges = [];
var Priviliges = new Array();
var rolenameGlobal;
AllUserRoles.instance = undefined;
var GlobalFlagSearch;
AllUserRoles.prototype.init = function (sandBox) {
    AllUserRoles.instance = this;
    AllUserRoles.instance.CurrentRoleModuleId = 0;
    window.ScrollVariableUser = 0;
    window.ScrollVariableGroups = 0;
    window.ScrollVariablePrivileges = 0;
    window.ScrollVariable = 0;
    window.ScrollVariableUserup = 0;
    window.ScrollVariableGroupsup = 0;
    window.ScrollVariablePrivilegesup = 0;
    window.ScrollVariableup = 0;
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetTagTemplate', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        //$("#" + RADUserManagement.contentBodyId).empty();
        $(".usermgmt").remove();
        $(".pull-right").css("display", "none")
        $("#" + RADUserManagement.contentBodyId).append(responseText.d);
        AllUserRoles.instance.GetPrivilegesForRole = []
        AllUserRoles.instance.GetModuleForRole = []

        $("<div id=\"ModuleDivLeftMenu\" class=\"RADUSERManagementAccountValuePageLoad\"><div id=\"RADModuleDivLeftMenu\" class=\"RADUserManagementAddNewGroupAccountPageLoad\"></div></div>").insertAfter($('.umLeftHeader'));
        $("#RADModuleDivLeftMenu").append("<div id=\"RADModuleDivLeftMenuFirstAccount\" class=\"RADUserManagementGroupFirstAccount\">All Modules</div>")
        $("#RADModuleDivLeftMenu").append("<div class=\"fa fa-caret-down RADUserManagementGroupDownArrow\"></div>")

        $("#RADModuleDivLeftMenuFirstAccount").attr("module_id", parseInt(0));


        $(".RADSearchParentDiv").find("#RADUserCreatePlusCircle").remove();
        $("#ModuleDivLeftMenu").append("<div id=\"RADUserMgmtAddRoleDiv\" class =\"RADUserMgmtAddRoleDiv\" ><i class=\"fa fa-plus-circle RADUserCreatePlusCircle\" id=\"RADUserCreatePlusCircle\"></i></div>")
        $(".RADUserCreatePlusCircle").css({ "position": "" });
        $(".RADUserCreatePlusCircle").css({ "top": "0" });

        $(".umRightBody").empty();
        $(".umRightBody").css({ "mergin-left": "5%" });
        $("<div class=\"HorizontalRule\"> </div>").insertAfter($('.UMRightHeaderEdit'))
        $(".umRightBody").append("<div id=\"RADUserManagaementRoleScreenUsers\" class=\"RADUserManagaementRoleScreen\"></div>");
        $("#RADUserManagaementRoleScreenUsers").append("<div id=\"RADUserManagaementRoleScreenUsersHead\" class=\"RADUserManagaementRoleScreenHead\">USERS</div>");
        $("#RADUserManagaementRoleScreenUsersHead").css({ "background": "#91AAC0" })
        $("#RADUserManagaementRoleScreenUsers").append("<div id=\"RADUserManagaementRoleScreenUsersBody\" class=\"RADUserManagaementRoleScreenBody\"></div>");
        $(".umRightBody").append("<div id=\"RADUserManagaementRoleScreenGroups\" class=\"RADUserManagaementRoleScreen\"></div>");
        $("#RADUserManagaementRoleScreenGroups").append("<div id=\"RADUserManagaementRoleScreenGroupsHead\" class=\"RADUserManagaementRoleScreenHead\">GROUPS</div>");
        $("#RADUserManagaementRoleScreenGroupsHead").css({ "background": "#93A5A9" })
        $("#RADUserManagaementRoleScreenGroups").append("<div id=\"RADUserManagaementRoleScreenGroupsBody\" class=\"RADUserManagaementRoleScreenBody\"></div>");
        $(".umRightBody").append("<div id=\"RADUserManagaementRoleScreenPrivileges\" class=\"RADUserManagaementRoleScreen\"></div>");
        $("#RADUserManagaementRoleScreenPrivileges").append("<div id=\"RADUserManagaementRoleScreenPrivilegesHead\" class=\"RADUserManagaementRoleScreenHead\">PRIVILEGES</div>");
        $("#RADUserManagaementRoleScreenPrivilegesHead").css({ "background": "grey" });
        $("#RADUserManagaementRoleScreenPrivileges").append("<div id=\"RADUserManagaementRoleScreenPrivilegesBody\" class=\"RADUserManagaementRoleScreenBody\"></div>");
        $('#RADUserManagaementRoleScreenUsers').append("<div class=\"scrollUpDownGroup\"></div>")
        $('.scrollUpDownGroup').append("<div id=\"RADUserManagementScrollDownArrowGroup\" class=\"fa fa-caret-down RADUserManagementScrollArrowGroupSmall\"></div>")
        $('.scrollUpDownGroup').append("<div id=\"RADUserManagementScrollUpArrowGroup\" class=\"fa fa-caret-up RADUserManagementScrollArrowGroupSmall\"></div>")
        $('#RADUserManagaementRoleScreenGroups').append("<div class=\"scrollUpDownRoles\"></div>")
        $('.scrollUpDownRoles').append("<div id=\"RADUserManagementScrollDownArrowRoles\" class=\"fa fa-caret-down RADUserManagementScrollArrowGroupSmall\"></div>")
        $('.scrollUpDownRoles').append("<div id=\"RADUserManagementScrollUpArrowRoles\" class=\"fa fa-caret-up RADUserManagementScrollArrowGroupSmall\"></div>")
        $('#RADUserManagaementRoleScreenPrivileges').append("<div class=\"scrollUpDownPrivileges\"></div>")
        $('.scrollUpDownPrivileges').append("<div id=\"RADUserManagementScrollDownArrowPrivileges\" class=\"fa fa-caret-down RADUserManagementScrollArrowGroupSmall\"></div>")
        $('.scrollUpDownPrivileges').append("<div id=\"RADUserManagementScrollUpArrowPrivileges\" class=\"fa fa-caret-up RADUserManagementScrollArrowGroupSmall\"></div>")

        $(".umRightHeader").empty();
        $('#RADDeleteInactiveButtonDiv').append("<input id=\"RADUserManagementDeleteInActiveButton\" class=\"RADUserManagementDeleteInActiveButton\" type=\"button\" value=\"Delete Inactive Roles\" />");

        var umDynamicTilesHeight = $("#contentBody").height() - ($(".umLeftHeader").height() + $(".RADUserMgmtscrollUpDown").height() + $(".searchBarUm").height() + $("#ModuleDivLeftMenu").height());
        $('.umDynamicTiles').height(umDynamicTilesHeight);

        var umRightBodyHeight = $(".umRightSec").height() - $(".umRightHeader").height();
        $('.umRightBody').height(umRightBodyHeight);

        var RADUserManagaementRoleScreenPadding = $('.RADUserManagaementRoleScreen').css('paddingTop').slice(0, -2);
        var RADUserManagaementRoleScreenUsersHeight = $(".umRightBody").height() - RADUserManagaementRoleScreenPadding;
        $('#RADUserManagaementRoleScreenUsers').height(RADUserManagaementRoleScreenUsersHeight);
        $('#RADUserManagaementRoleScreenGroups').height(RADUserManagaementRoleScreenUsersHeight);
        $('#RADUserManagaementRoleScreenPrivileges').height(RADUserManagaementRoleScreenUsersHeight);

        var RADUserManagaementRoleScreenUsersBodyHeight = $("#RADUserManagaementRoleScreenUsers").height() - (Number(RADUserManagaementRoleScreenPadding)
                                                     + $("#RADUserManagaementRoleScreenUsersHead").height() + $(".scrollUpDownGroup").height())
        $('#RADUserManagaementRoleScreenUsersBody').height(RADUserManagaementRoleScreenUsersBodyHeight);

        var RADUserManagaementRoleScreenGroupsBodyHeight = $("#RADUserManagaementRoleScreenGroups").height() - (Number(RADUserManagaementRoleScreenPadding)
                                                     + $("#RADUserManagaementRoleScreenGroupsHead").height() + $(".scrollUpDownGroup").height())
        $('#RADUserManagaementRoleScreenGroupsBody').height(RADUserManagaementRoleScreenGroupsBodyHeight);

        var RADUserManagaementRoleScreenPrivilegesBodyHeight = $("#RADUserManagaementRoleScreenPrivileges").height() - (Number(RADUserManagaementRoleScreenPadding)
                                                     + $("#RADUserManagaementRoleScreenPrivilegesHead").height() + $(".scrollUpDownGroup").height())
        $('#RADUserManagaementRoleScreenPrivilegesBody').height(RADUserManagaementRoleScreenPrivilegesBodyHeight);
        
        var userName;        
        AllUserRoles.instance.BindEvents();

        AllUserRoles.instance.GetAuthorizationPrivileges();
        AllUserRoles.instance.SelectedUserArray = []
        AllUserRoles.instance.AllUserList = []
        AllUserRoles.instance.SelectedGroupArray = []
        AllUserRoles.instance.SelectedGroupsForSearch = []
        AllUserRoles.instance.AllGroupList = []
        AllUserRoles.instance.SelectedPrivilegesArray = []
        AllUserRoles.instance.SelectedPrivilegesForSearch = []
        AllUserRoles.instance.AllPrivilegesList = []
        AllUserRoles.instance.SelectedUsersForSearch = []
        AllUserRoles.instance.SelectedUserArrayForCreate = []
        AllUserRoles.instance.SelectedGroupArrayForCreate = []
        AllUserRoles.instance.SelectedPrivilegesArrayForCreate = []
        AllUserRoles.instance.GetAllModules("PageLoad");
    });
}

AllUserRoles.prototype.UserSearch = function (event) {

    for (var i = 0; i < $("#RADUserManagaementRoleScreenUsersBody").find(".RADUserManagementSelectedUsersInGroup").length; i++) {

        if (!Array.contains(AllUserRoles.instance.SelectedUsersForSearch,
             $("#RADUserManagaementRoleScreenUsersBody").find(".RADUserManagementSelectedUsersInGroup")[i].innerText))
            AllUserRoles.instance.SelectedUsersForSearch.push
                ($("#RADUserManagaementRoleScreenUsersBody").find(".RADUserManagementSelectedUsersInGroup")[i].innerText);
    }

    $("#RADUserManagaementRoleScreenUsersBody").empty();

    //This below if statement is written to show, selected items first, when no search text is present
    if ($(event.target).val() == "") {

        for (var k = 0; k < AllUserRoles.instance.SelectedUsersForSearch.length; k++) {
            $('#RADUserManagaementRoleScreenUsersBody').append('<div id="UserSelected' + k + '" User_name="'
                + AllUserRoles.instance.SelectedUsersForSearch[k] + '">' + AllUserRoles.instance.SelectedUsersForSearch[k] + '</div>')
            $("#UserSelected" + k).addClass('RADUserManagementSelectedUsersInGroup');
            $("#UserSelected" + k).addClass('RADUserManagementUsersInGroup');
        }

        for (var j = 0; j < AllUserRoles.instance.AllUserList.length; j++) {

            if (!Array.contains(AllUserRoles.instance.SelectedUsersForSearch, AllUserRoles.instance.AllUserList[j])) {
                $('#RADUserManagaementRoleScreenUsersBody').append('<div id="User' + j + '" User_name="'
                   + AllUserRoles.instance.AllUserList[j] + '">' + AllUserRoles.instance.AllUserList[j] + '</div>')
                $("#User" + j).addClass('RADUserManagementUsersInGroup');
            }
        }
    }

    else {

        for (var j = 0; j < AllUserRoles.instance.AllUserList.length; j++) {

            if ((AllUserRoles.instance.AllUserList[j].toLowerCase()).indexOf($(event.target).val().toLowerCase()) != -1) {

                if (AllUserRoles.instance.SelectedUsersForSearch.indexOf(AllUserRoles.instance.AllUserList[j]) != -1) {
                    $('#RADUserManagaementRoleScreenUsersBody').append('<div id="User' + j + '" User_name="'
                        + AllUserRoles.instance.AllUserList[j] + '">' + AllUserRoles.instance.AllUserList[j] + '</div>')
                    $("#User" + j).addClass('RADUserManagementSelectedUsersInGroup');
                    $("#User" + j).addClass('RADUserManagementUsersInGroup');
                }

                else {
                    $('#RADUserManagaementRoleScreenUsersBody').append('<div id="User' + j + '" User_name="'
                   + AllUserRoles.instance.AllUserList[j] + '">' + AllUserRoles.instance.AllUserList[j] + '</div>')
                    $("#User" + j).addClass('RADUserManagementUsersInGroup');
                }
            }
        }
    }

    $(".RADUserManagementUsersInGroup").unbind().click(function (event) {
        if ($(this).hasClass("RADUserManagementSelectedUsersInGroup") == true) {
            $(this).switchClass("RADUserManagementSelectedUsersInGroup", "RADUserManagementUsersInGroup")
            AllUserRoles.instance.SelectedUsersForSearch.splice(AllUserRoles.instance.SelectedUsersForSearch.indexOf($(this).text()), 1);
        }
        else {
            $(this).addClass('RADUserManagementSelectedUsersInGroup');
            if ((!Array.contains(AllUserRoles.instance.SelectedUsersForSearch, $(this).text())) &&
                (Array.contains(AllUserRoles.instance.AllUserList, $(this).text())))
                AllUserRoles.instance.SelectedUsersForSearch.push($(this).text())
        }
    });

}


AllUserRoles.prototype.RoleSearch = function (event) {

    for (var i = 0; i < $("#RADUserManagaementRoleScreenGroupsBody").find(".RADUserManagementSelectedUsersInGroup").length; i++) {

        if (!Array.contains(AllUserRoles.instance.SelectedGroupsForSearch,
             $("#RADUserManagaementRoleScreenGroupsBody").find(".RADUserManagementSelectedUsersInGroup")[i].innerText))
            AllUserRoles.instance.SelectedGroupsForSearch.push
                ($("#RADUserManagaementRoleScreenGroupsBody").find(".RADUserManagementSelectedUsersInGroup")[i].innerText);
    }

    $("#RADUserManagaementRoleScreenGroupsBody").empty();

    //This below if statement is written to show, selected items first, when no search text is present
    if ($(event.target).val() == "") {

        for (var k = 0; k < AllUserRoles.instance.SelectedGroupsForSearch.length; k++) {

            $('#RADUserManagaementRoleScreenGroupsBody').append('<div id="GroupSelected' + k + '" User_name="'
                                                               + AllUserRoles.instance.SelectedGroupsForSearch[k] + '">'
                                                               + AllUserRoles.instance.SelectedGroupsForSearch[k] + '</div>')
            $("#GroupSelected" + k).addClass('RADUserManagementSelectedUsersInGroup');
            $("#GroupSelected" + k).addClass('RADUserManagementUsersInGroup');
        }

        for (var j = 0; j < AllUserRoles.instance.AllGroupList.length; j++) {

            if (!Array.contains(AllUserRoles.instance.SelectedGroupsForSearch, AllUserRoles.instance.AllGroupList[j])) {

                $('#RADUserManagaementRoleScreenGroupsBody').append('<div id="UserRoleGroup' + j + '" User_name="'
                                                            + AllUserRoles.instance.AllGroupList[j] + '">'
                                                            + AllUserRoles.instance.AllGroupList[j] + '</div>')
                $("#UserRoleGroup" + j).addClass('RADUserManagementUsersInGroup');

            }
        }
    }

    else {

        for (var j = 0; j < AllUserRoles.instance.AllGroupList.length; j++) {

            if ((AllUserRoles.instance.AllGroupList[j].toLowerCase()).indexOf($(event.target).val().toLowerCase()) != -1) {

                if (AllUserRoles.instance.SelectedGroupsForSearch.indexOf(AllUserRoles.instance.AllGroupList[j]) != -1) {

                    $('#RADUserManagaementRoleScreenGroupsBody').append('<div id="UserRoleGroup' + j + '" User_name="'
                                                                + AllUserRoles.instance.AllGroupList[j] + '">'
                                                                + AllUserRoles.instance.AllGroupList[j] + '</div>')
                    $("#UserRoleGroup" + j).addClass('RADUserManagementSelectedUsersInGroup');
                    $("#UserRoleGroup" + j).addClass('RADUserManagementUsersInGroup');
                }

                else {
                    $('#RADUserManagaementRoleScreenGroupsBody').append('<div id="UserRoleGroup' + j + '" User_name="'
                                                                + AllUserRoles.instance.AllGroupList[j] + '">'
                                                                + AllUserRoles.instance.AllGroupList[j] + '</div>')
                    $("#UserRoleGroup" + j).addClass('RADUserManagementUsersInGroup');
                }

            }
        }
    }

    $(".RADUserManagementUsersInGroup").unbind().click(function (event) {
        if ($(this).hasClass("RADUserManagementSelectedUsersInGroup") == true) {
            $(this).switchClass("RADUserManagementSelectedUsersInGroup", "RADUserManagementUsersInGroup")
            AllUserRoles.instance.SelectedGroupsForSearch.splice(AllUserRoles.instance.SelectedGroupsForSearch.indexOf($(this).text()), 1);
        }
        else {
            $(this).addClass('RADUserManagementSelectedUsersInGroup');
            if ((!Array.contains(AllUserRoles.instance.SelectedGroupsForSearch, $(this).text()))
                && (Array.contains(AllUserRoles.instance.AllGroupList, $(this).text())))
                AllUserRoles.instance.SelectedGroupsForSearch.push($(this).text())
        }
    });
}


AllUserRoles.prototype.PrivilegesSearch = function (event) {

    for (var i = 0; i < $("#RADUserManagaementRoleScreenPrivilegesBody").find(".RADUserManagementSelectedUsersInGroup").length; i++) {

        if (!Array.contains(AllUserRoles.instance.SelectedPrivilegesForSearch,
             $("#RADUserManagaementRoleScreenPrivilegesBody").find(".RADUserManagementSelectedUsersInGroup")[i].innerText))
            AllUserRoles.instance.SelectedPrivilegesForSearch.push
                ($("#RADUserManagaementRoleScreenPrivilegesBody").find(".RADUserManagementSelectedUsersInGroup")[i].innerText);
    }

    $("#RADUserManagaementRoleScreenPrivilegesBody").empty();

    //This below if statement is written to show, selected items first, when no search text is present
    if ($(event.target).val() == "") {

        for (var k = 0; k < AllUserRoles.instance.SelectedPrivilegesForSearch.length; k++) {

            $('#RADUserManagaementRoleScreenPrivilegesBody').append('<div id="UserRolePrivSelected' + k + '" User_name="'
                        + AllUserRoles.instance.SelectedPrivilegesForSearch[k] + '">' + AllUserRoles.instance.SelectedPrivilegesForSearch[k] + '</div>')
            $("#UserRolePrivSelected" + k).addClass('RADUserManagementSelectedUsersInGroup');
            $("#UserRolePrivSelected" + k).addClass('RADUserManagementUsersInGroup');
        }


        for (var j = 0; j < AllUserRoles.instance.AllPrivilegesList.length; j++) {

            if (!Array.contains(AllUserRoles.instance.SelectedPrivilegesForSearch, AllUserRoles.instance.AllPrivilegesList[j])) {

                $('#RADUserManagaementRoleScreenPrivilegesBody').append('<div id="UserRolePriv' + j + '" User_name="'
                   + AllUserRoles.instance.AllPrivilegesList[j] + '">' + AllUserRoles.instance.AllPrivilegesList[j] + '</div>')
                $("#UserRolePriv" + j).addClass('RADUserManagementUsersInGroup');

            }
        }
    }

    else {

        for (var j = 0; j < AllUserRoles.instance.AllPrivilegesList.length; j++) {

            if ((AllUserRoles.instance.AllPrivilegesList[j].toLowerCase()).indexOf($(event.target).val().toLowerCase()) != -1) {

                if (AllUserRoles.instance.SelectedPrivilegesForSearch.indexOf(AllUserRoles.instance.AllPrivilegesList[j]) != -1) {

                    $('#RADUserManagaementRoleScreenPrivilegesBody').append('<div id="UserRolePriv' + j + '" User_name="'
                        + AllUserRoles.instance.AllPrivilegesList[j] + '">' + AllUserRoles.instance.AllPrivilegesList[j] + '</div>')
                    $("#UserRolePriv" + j).addClass('RADUserManagementSelectedUsersInGroup');
                    $("#UserRolePriv" + j).addClass('RADUserManagementUsersInGroup');
                }

                else {

                    $('#RADUserManagaementRoleScreenPrivilegesBody').append('<div id="UserRolePriv' + j + '" User_name="'
                    + AllUserRoles.instance.AllPrivilegesList[j] + '">' + AllUserRoles.instance.AllPrivilegesList[j] + '</div>')
                    $("#UserRolePriv" + j).addClass('RADUserManagementUsersInGroup');

                }
            }
        }
    }

    $(".RADUserManagementUsersInGroup").unbind().click(function (event) {
        if ($(this).hasClass("RADUserManagementSelectedUsersInGroup") == true) {
            $(this).switchClass("RADUserManagementSelectedUsersInGroup", "RADUserManagementUsersInGroup")
            AllUserRoles.instance.SelectedPrivilegesForSearch.splice(AllUserRoles.instance.SelectedPrivilegesForSearch.indexOf($(this).text()), 1);
        }
        else {
            $(this).addClass('RADUserManagementSelectedUsersInGroup');
            if ((!Array.contains(AllUserRoles.instance.SelectedPrivilegesForSearch, $(this).text()))
                && (Array.contains(AllUserRoles.instance.AllPrivilegesList, $(this).text())))
                AllUserRoles.instance.SelectedPrivilegesForSearch.push($(this).text())
        }
    });

}

AllUserRoles.prototype.GetAuthorizationPrivileges = function () {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAuthorizationPrivileges', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'

    }).then(function (responseText) {

        if (responseText.d == "admin") {
            $("#ModuleDivLeftMenu").show();
            $('.RADUserCreatePlusCircle').show();
            $("#RADUserManagementEditButton").show()
            $("#RADUserManagementDeleteButton").show()
            $("#RADDownloadButtonDiv").show();
            $("#RADUploadButtonDiv").show();
            Priviliges = ['Add Role', 'Delete Role', 'Update Role', 'Download Role Configuration', 'Upload Role Configuration'];
        }
        else {
            var ResponseForCreation = [];
            if (responseText.d.length > 0)
                ResponseForCreation = JSON.parse(responseText.d);
            Priviliges = []
            for (var i = 0; i < ResponseForCreation.length; i++) {
                if (ResponseForCreation[i].pageId == "RAD_Role") {
                    for (var j = 0; j < ResponseForCreation[i].Privileges.length; j++)
                        Priviliges.push(ResponseForCreation[i].Privileges[j])
                }
            }
            if (Priviliges.indexOf("Add Role") != -1) {
                $("#ModuleDivLeftMenu").show();
                $('.RADUserCreatePlusCircle').show();
            }
            else {
                $('.RADUserCreatePlusCircle').hide();
                $("#ModuleDivLeftMenu").hide();

            }
            if (Priviliges.indexOf("Delete Role") != -1) {
                $("#RADUserManagementDeleteButton").show()
            }
            else {
                $("#RADUserManagementDeleteButton").hide()
            }
            if (Priviliges.indexOf("Update Role") != -1) {
                $("#RADUserManagementEditButton").show()
            }
            else {
                $("#RADUserManagementEditButton").hide()
            }
            if (Priviliges.indexOf("Download Role Configuration") != -1) {
                $("#RADDownloadButtonDiv").show();
            }
            else {
                $("#RADDownloadButtonDiv").hide();
            }
            if (Priviliges.indexOf("Upload Role Configuration") != -1) {
                $("#RADUploadButtonDiv").show();
            }
            else {
                $("#RADUploadButtonDiv").hide();
            }
        }
    })
}

AllUserRoles.prototype.ShowHideByPrivilegeType = function (PrivilegeType) {

    switch (PrivilegeType) {

        case "Add":
            if (Priviliges.indexOf("Add Role") != -1)
                $('.RADUserCreatePlusCircle').show();
            else
                $('.RADUserCreatePlusCircle').hide();
            break;

        case "Edit":
            if (Priviliges.indexOf("Update Role") != -1)
                $("#RADUserManagementEditButton").show();
            else
                $("#RADUserManagementEditButton").hide();
            break;

        case "Delete":
            if (Priviliges.indexOf("Delete Role") != -1)
                $("#RADUserManagementDeleteButton").show();
            else
                $("#RADUserManagementDeleteButton").hide();
            break;
        default:
            //do nothing
    }
}
//to get all the roles that exist
AllUserRoles.prototype.GetAllRoles = function () {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllRoles', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'

    }).then(function (responseText) {
        ListOfRoles = JSON.parse(responseText.d);
        var firstRole = "";
        AllUserRoles.instance.ListOfInfo = ListOfRoles;        
        AllUserRoles.instance.CreateTilesOfRoles(ListOfRoles);

    });
}

AllUserRoles.prototype.GetModuleIdsForModuleName = function () {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetModuleDetails', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'

    }).then(function (responseText) {
        var ResponseForCreation = JSON.parse(responseText.d);
        AllUserRoles.instance.GetModuleForRole = ResponseForCreation;
        AllUserRoles.instance.CreateTilesOfRoles(ListOfRoles);        
    })
}

//handle all the click events
AllUserRoles.prototype.BindEvents = function () {
    
    $("#RADUserManagementDownloadButton").unbind().click(function (event) {
        AllUserUsers.instance.getHTML(AllUserRoles.instance.ListOfInfo, "Roles");
    });

    $("#RADUserManagementUploadButton").unbind().click(function (event) {
        AllUserUsers.instance.getUploadPopUpHTML("Roles");

    });

    $("#RADUserManagaementRoleScreenUsers").on('click', '#RADUserManagementScrollDownArrowGroup', function (event) {
        ScrollVariableUser = ScrollVariableUser + 50;
        $("#RADUserManagaementRoleScreenUsersBody").scrollTop(ScrollVariableUser);
    });

    $(".UMRightHeaderEdit").on("click", "#RADUserManagementDeleteInActiveButton", function (event) {
        AllUserRoles.instance.DeleteInActiveRoles();
    });

    $("#RADUserManagaementRoleScreenUsers").on('click', '#RADUserManagementScrollUpArrowGroup', function (event) {
        ScrollVariableUser = ScrollVariableUser - 50;
        $("#RADUserManagaementRoleScreenUsersBody").scrollTop(ScrollVariableUser);
    });

    $("#RADUserManagaementRoleScreenGroups").on('click', '#RADUserManagementScrollDownArrowRoles', function (event) {
        ScrollVariableGroups = ScrollVariableGroups + 50;
        $("#RADUserManagaementRoleScreenGroupsBody").scrollTop(ScrollVariableGroups);
    });

    $("#RADUserManagaementRoleScreenGroups").on('click', '#RADUserManagementScrollUpArrowRoles', function (event) {
        if (ScrollVariableGroups > 0) {
            ScrollVariableGroups = ScrollVariableGroups - 50;
            $("#RADUserManagaementRoleScreenGroupsBody").scrollTop(ScrollVariableGroups);
        }
    });

    $("#RADUserManagaementRoleScreenPrivileges").on('click', '#RADUserManagementScrollDownArrowPrivileges', function (event) {
        ScrollVariablePrivileges = ScrollVariablePrivileges + 50;
        $("#RADUserManagaementRoleScreenPrivilegesBody").scrollTop(ScrollVariablePrivileges);
    });

    $("#RADUserManagaementRoleScreenPrivileges").on('click', '#RADUserManagementScrollUpArrowPrivileges', function (event) {
        if (ScrollVariablePrivileges > 0) {
            ScrollVariablePrivileges = ScrollVariablePrivileges - 50;
            $("#RADUserManagaementRoleScreenPrivilegesBody").scrollTop(ScrollVariablePrivileges);
        }
    });

    $("#RADUserManagaementRoleScreenUsers").on("keyup", "#Groupsearch_text", function (event) {
        AllUserRoles.instance.UserSearch(event)
    });

    $(".umLeftSec").on('click', '.RADsearchIconGridView', function (event) {
        if ($('#search_text:visible').length) {            
            $("#search_text").hide("slow", function () {
                // Animation complete.
            });
        }
        else {            
            $("#search_text").show("slow", function () {
                // Animation complete.
            });
        }        
    });

    $("#RADUserManagaementRoleScreenGroups").on("keyup", "#Rolesearch_text", function (event) {
        AllUserRoles.instance.RoleSearch(event)
    });
    $("#RADUserManagaementRoleScreenPrivileges").on("keyup", "#Privileges_text", function (event) {
        AllUserRoles.instance.PrivilegesSearch(event)
    });
    
    $("#RADUserManagementEditButton").unbind().click(function (event) {

        if ($(event.target).attr("value") == "Edit") {//to check if it is in edit mode or not 
            AllUserRoles.instance.OnEditRole();
        }
        else if ($(event.target).attr("value") == "Save" && $(event.target).attr("createGroup") == "true") {//to create new role
            AllUserRoles.instance.CreateRole();
        }
        else if ($(event.target).attr("value") == "Save") {// only in edit case
            AllUserRoles.instance.ShowHideByPrivilegeType("Edit");
            $(event.target).val("Edit");
            AllUserRoles.instance.UpdateRole();
        }
    });

    $("#ContentDiv").unbind().click(function (event) {
        if ($(event.target).closest(".RADUserManagementAddNewGroupAccount").length == 0) {
            if ($("#RADUserManagementAddNewGroupAccount").height() != 30)
                $("#RADUserManagementAddNewGroupAccount").css({ "height": "30px" });
        }
    });
    $("#search_text").keyup(function (event) {
        AllUserRoles.instance.CreateSearchText(event);
    });
    $("#RADUserManagementDeleteButton").unbind().click(function (event) {
        if ($(event.target).attr("value") == "Delete") {
            AllUserRoles.prototype.OnDeleteClick()
        }
        else {            
            $(".searchBarUm").show();
            $("#ModuleDivLeftMenu").show();
            AllUserRoles.instance.ShowHideByPrivilegeType("Add");            
            $("#Groupsearch_text").remove()
            $("#RADUMGroupSearch").remove()
            $("#Rolesearch_text").remove()
            $("#RADUMRoleSearch").remove()
            $("#Privileges_text").remove()
            $("#RADUMPrivilegesSearch").remove()
            $(event.target).attr("value", "Delete");
            AllUserRoles.instance.ShowHideByPrivilegeType("Delete");
            $("#Groupsearch_text").remove()
            $("#Rolesearch_text").remove()
            $("#Privileges_text").remove()
            $("#btnSelectAllPriviledges").remove()
            $("#ModuleDivLeftMenu").show();

            $("#RADGroupDesceValidation").remove()
            $("#RADUMGroupSearch").remove()
            $("#RADUMRoleSearch").remove()
            $("#RADUMPrivilegesSearch").remove()
            $("#RADUserGroupNameValidation").remove()
            $("#RADAccountValidation").remove()
            AllUserRoles.instance.ReCreateDiv();
            $("#RADUserManagaementRoleScreenPrivilegesBody").empty();
            $("#RADUserManagaementRoleScreenGroupsBody").empty();
            $("#RADUserManagaementRoleScreenUsersBody").empty();
            $("#RADUserManagementEditButton").removeAttr("createGroup");

        }

    });
    $("#RADUserManagementScrollDownArrow").unbind().click(function (event) {
        ScrollVariable = ScrollVariable + 50;
        $(".umDynamicTiles").scrollTop(ScrollVariable);
    });
    $("#RADUserManagementScrollUpArrow").unbind().click(function (event) {
        if (ScrollVariable > 0) {
            ScrollVariable = ScrollVariable - 50;
            $(".umDynamicTiles").scrollTop(ScrollVariable);
        }
    });
    $(".RADUserCreatePlusCircle").unbind().click(function (event) {        
        AllUserRoles.instance.SelectedPrivilegesForSearch = []
        AllUserRoles.instance.SelectedGroupsForSearch = []
        AllUserRoles.instance.SelectedUsersForSearch = []
        $('.RADUserCreatePlusCircle').hide();
        $("#ModuleDivLeftMenu").hide();

        if ($("#RADUserManagementEditButton").attr("createGroup") == null) {
            var GlobalFlagSearch = true
            AllUserRoles.instance.SelectedUserArrayForCreate = []
            AllUserRoles.instance.SelectedGroupArrayForCreate = []
            AllUserRoles.instance.SelectedPrivilegesArrayForCreate = []
            
            $(".umRightHeader").empty();

            $(".umRightHeader").append("<div id=\"RADUserManagaementUserLabel\" class=\"RADUserManagaementLabel\"></div>");
            $("#RADUserManagaementUserLabel").append("<input id=\"RADUserManagementAddNewGroupName\" class=\"RADUserManagementAddNewGroupName\" placeholder=\"Role Name\"></input>");

            $(".umRightHeader").append("<div id=\"lab\" class=\"RADUserMgmtuser_label\"></div>");
            $("#lab").append("<input id=\"RADUserManagementAddNewGroupDesc\" class=\"RADUserManagementAddNewGroupDesc\" placeholder=\"Role Description\"></input>");
                        
            $("<div id=\"searchUserSearch\"</div>").insertAfter($('#RADUserManagaementRoleScreenUsersHead'))
            $("<div id=\"searchmain\"</div>").insertAfter($('#RADUserManagaementRoleScreenGroupsHead'))
            $('#searchUserSearch').append("<div id=\"RADUMGroupSearch\"  class=\"fa fa-search searchIconGridViewForUserGroup\"></div>")
            $('#searchUserSearch').append("<input type=\"text\" name=\"search_text\" id=\"Groupsearch_text\" placeholder=\"Search\"></div>")
            $('#searchmain').append("<div id=\"RADUMRoleSearch\"  class=\"fa fa-search searchIconGridViewForUserRoles\"></div>")
            $('#searchmain').append("<input type=\"text\" name=\"search_text\" id=\"Rolesearch_text\" placeholder=\"Search\"></div>")
            $("<div id=\"searchmain2\"</div>").insertAfter($('#RADUserManagaementRoleScreenPrivilegesHead'))
            $('#searchmain2').append("<div id=\"RADUMPrivilegesSearch\"  class=\"fa fa-search searchIconGridViewForUserPrivileges\"></div>")
            $('#searchmain2').append("<input type=\"text\" name=\"search_text\" id=\"Privileges_text\" placeholder=\"Search\"></div>")
            $('#searchmain2').append("<input type=\"button\" name=\"btnSelectAllPriviledges\" id=\"btnSelectAllPriviledges\" class=\"RADUserManagementSelectAllButton\" value=\"Select All\"/>");
            
            if ($("#RADUserManagementDeleteButton").attr("value") != "Cancel") {
                
                $(".umRightHeader").append("<div id=\"lab1\" class=\"RADUSERManagementAccountValue\"><div id=\"RADUserManagementAddNewGroupAccount\" class=\"RADUserManagementAddNewGroupAccount\"></div></div>");
                $("#RADUserManagementAddNewGroupAccount").append("<div id=\"RADUserManagementGroupFirstAccount\" class=\"RADUserManagementGroupFirstAccount\">Module Name</div>")
                $("#RADUserManagementAddNewGroupAccount").append("<div class=\"fa fa-caret-down RADUserManagementGroupDownArrow\"></div>")

                $(".umSec1Body").empty();
                $("#RADUserManagaementRoleScreenGroupsBody").empty()
                $("#RADUserManagaementRoleScreenUsersBody").empty()
                $("#RADUserManagaementRoleScreenPrivilegesBody").empty()
                $(".umSec2Body").empty();                
                $("#RADUserManagementEditButton").attr("createGroup", "true");
                $("#RADUserManagementEditButton").val("Save");
                $("#RADUserManagementDeleteButton").val("Cancel");
                $("#RADUserManagementEditButton").show();
                $("#RADUserManagementDeleteButton").show();
                $("#RADUserManagaementRoleScreenPrivilegesBody").empty();
                AllUserRoles.instance.GetAllModules("create");
            }
                //on creation
            else {

                $("#RADUserManagementAddNewGroupAccount").css({ "right": "20%" })
                $("#RADUserManagementAddNewGroupAccount").css({ "top": "52px" })

                $("RADUserManagaementRoleScreenGroupsBody").empty()
                $("RADUserManagaementRoleScreenUsersBody").empty()
                $("RADUserManagaementRoleScreenPrivilegesHead").empty()
                AllUserRoles.instance.GetAllModules("create");
            }
        }

    }); //adding new group

}

AllUserRoles.prototype.DeleteInActiveRoles = function () {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/DeleteInactiveRoles', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        $("#ContentDiv").append("<div id=\"alertParentDivInActive\" class=\"RADUserManagementAlertPopUpParent\"></div>");
        $("#alertParentDivInActive").append("<div class=\"RADUserManagementPopupAlertUpperSuccessDiv\"></div>");
        $("#alertParentDivInActive").append("<div class=\"RADUserManagementPopUpError\"></div>")
        $("#alertParentDivInActive").append("<div class=\"RADUserErrorDivLeft RADUserSuccess\"></div>"); //revisit
        $("#alertParentDivInActive").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
        $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Success</div>")
        $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Inactive Roles have been deleted successfully</div>");
        $("#alertParentDivInActive").css("top", "-200px");
        $("#alertParentDivInActive").animate({ "top": "0px" });
        setInterval(function () {
            $("#alertParentDivInActive").remove();

        }, 2700);

        var RolesObject = new AllUserRoles();
        RolesObject.init();
    });
}
//to create the modules dropdown
AllUserRoles.prototype.GetAllModules = function (flag) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetModuleDetails', // to get the entire HTMLRADUserManagementAddNewGroupAccount
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {

        var result = JSON.parse(responseText.d);
        AllUserRoles.instance.AllModules = result;
        AllUserRoles.instance.GetModuleForRole = result;        

        if (flag == "PageLoad") {
            $('#RADModuleDivLeftMenu').append("<div id=\"AllModulesDiv\" data =\"All Modules\" module_id=\"0\"class=\"RADUserManagementEachAccountGroupDivLeftMenu\">All Modules</div>");
            AllUserRoles.instance.GetAllRoles();
        }

        for (var i = 0; i < result.length; i++) {

            if (flag == "PageLoad") {
                $('#RADModuleDivLeftMenu').append("<div data =\"" + result[i].ModuleName + "\" module_id=\"" + result[i].ModuleId + "\"class=\"RADUserManagementEachAccountGroupDivLeftMenu\">" + result[i].ModuleName + "</div>");
            }
            else {
                $('#RADUserManagementAddNewGroupAccount').append("<div data =\"" + result[i].ModuleName + "\" module_id=\"" + result[i].ModuleId + "\"class=\"RADUserManagementEachAccountGroupDiv\">" + result[i].ModuleName + "</div>");
            }
        }

        //to handle click of dropdowns
        if (flag == "edit") {
            AllUserRoles.instance.GetAllUsers(AllUserRoles.instance.GetUsersForRole);
            AllUserRoles.instance.GetAllGroups(AllUserRoles.instance.GetGroupsForRole);
            $(".RADUserManagementGroupFirstAccount").unbind().click(function (event) { //to show the dropdown
                $("#RADUserManagementAddNewGroupAccount").css({ "height": "auto" })
            });
            $(".RADUserManagementEachAccountGroupDiv").unbind().click(function (event) {//to show the selected text in dropdown
                $("#RADUserManagementGroupFirstAccount").html($(event.target).html());
                $("#RADUserManagementGroupFirstAccount").attr("module_id", parseInt($(event.target).attr("module_id")));
                $("#RADUserManagementAddNewGroupAccount").css({ "height": "30px" })
                var moduleId = $(event.target).attr("module_id");
                var isSameModule = AllUserRoles.instance.CurrentRoleModuleId == parseInt(moduleId) ? true : false;
                AllUserRoles.instance.GetAllPrivilegesForModule(parseInt(moduleId), false, isSameModule)
            });

        }
        if (flag == "create") {
            AllUserRoles.instance.GetAllUsersCreate(); //to get all the groups ann users at the time of create
            AllUserRoles.instance.GetAllGroupsCreate();            

            $(".RADUserManagementGroupFirstAccount").unbind().click(function (event) { //to show the dropdown
                $("#RADUserManagementAddNewGroupAccount").css({ "height": "auto" })
            });
            $(".RADUserManagementEachAccountGroupDiv").unbind().click(function (event) {//to show the selected text in dropdown
                $("#RADUserManagementGroupFirstAccount").html($(event.target).html());
                $("#RADUserManagementGroupFirstAccount").attr("module_id", parseInt($(event.target).attr("module_id")));
                $("#RADUserManagementAddNewGroupAccount").css({ "height": "30px" })
                var moduleId = $(event.target).attr("module_id");
                AllUserRoles.instance.GetAllPrivilegesForModuleCreate(parseInt(moduleId));
            });
        }

        $("#RADModuleDivLeftMenuFirstAccount").unbind().click(function (event) { //to show the dropdown
            $("#RADModuleDivLeftMenu").css({ "height": "auto" })
            if ($("#RADModuleDivLeftMenuFirstAccount").text() == "All Modules") {
                $("#AllModulesDiv").hide();
            }
            else {

                $("#AllModulesDiv").show();
            }
        });

        $(".RADUserManagementEachAccountGroupDivLeftMenu").unbind().click(function (event) {//to show the selected text in dropdown
            $("#RADModuleDivLeftMenuFirstAccount").html($(event.target).html());
            $("#RADModuleDivLeftMenuFirstAccount").attr("module_id", parseInt($(event.target).attr("module_id")));
            $("#RADModuleDivLeftMenu").css({ "height": "35px" })
            var moduleId = $(event.target).attr("module_id");
            AllUserRoles.instance.GetRolesByModule(parseInt(moduleId));

        });

        $("#ContentDiv").unbind().click(function (event) {
            if ($(event.target).closest(".RADUserManagementAddNewGroupAccountPageLoad").length == 0) {
                if ($("#RADModuleDivLeftMenu").height() != 35)
                    $("#RADModuleDivLeftMenu").css({ "height": "35px" });
            }
        });
    });
}

AllUserRoles.prototype.GetAllUsersCreate = function () {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllUsers', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        var roleUsers;
        roleUsers = JSON.parse(responseText.d);
        var arr = []
        for (var i = 0; i < roleUsers.length; i++) {
            arr.push(roleUsers[i]["UserLoginName"])
        }
        AllUserRoles.instance.AllUserList = arr
        $("#RADUserManagaementRoleScreenUsersBody").empty();
        for (var i = 0; i < roleUsers.length; i++) {
            $("#RADUserManagaementRoleScreenUsersBody").append("<div class=\"RADUserManagementUsersInGroup\">" + roleUsers[i]["UserLoginName"] + "</div>");
        }
        $(".umRightBody").unbind("click");
        $(".RADUserManagementUsersInGroup").unbind().click(function (event) {        // $(".RADUserManagementUsersInGroup").unbind().click(function(event){
            AllUserRoles.instance.SelectUnSelectedData(event);
        });

    });
}
AllUserRoles.prototype.GetAllUsers = function (GetUsersForRole) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllUsers', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        var roleUsers;
        roleUsers = JSON.parse(responseText.d);
        var arr = []
        AllUserRoles.instance.SelectedUserArray = GetUsersForRole
        for (var i = 0; i < roleUsers.length; i++) {
            arr.push(roleUsers[i]["UserLoginName"])
        }
        AllUserRoles.instance.AllUserList = arr

        for (var i = 0; i < AllUserRoles.instance.GetUsersForRole.length; i++) {
            $("#RADUserManagaementRoleScreenUsersBody").append("<div class=\"RADUserManagementUsersInGroup RADUserManagementSelectedUsersInGroup\">" + AllUserRoles.instance.GetUsersForRole[i] + "</div>");
        }
        if (AllUserRoles.instance.SelectedUserArray != null) {
            for (var i = 0; i < roleUsers.length; i++) {
                if (GetUsersForRole.indexOf(roleUsers[i]["UserLoginName"]) != -1)
                { }

                else {
                    $("#RADUserManagaementRoleScreenUsersBody").append("<div class=\"RADUserManagementUsersInGroup\">" + roleUsers[i]["UserLoginName"] + "</div>");
                }
            }
        }
        else {
            for (var i = 0; i < roleUsers.length; i++) {
                $("#RADUserManagaementRoleScreenUsersBody").append("<div class=\"RADUserManagementUsersInGroup\">" + roleUsers[i]["UserLoginName"] + "</div>");
            }
        }
        $(".umRightBody").unbind("click");
        $(".RADUserManagementUsersInGroup").unbind().click(function (event) {        // $(".RADUserManagementUsersInGroup").unbind().click(function(event){
            AllUserRoles.instance.SelectUnSelectedData(event);
        });

    });
}
AllUserRoles.prototype.CreateSearchText = function (event) {
    var flagSearch = false
    $(".umDynamicTiles").empty();
    var ToLowerCase = []
    var InLowerCase = [];
    var currentModuleId = parseInt($("#RADModuleDivLeftMenuFirstAccount").attr("module_id"));
    for (var j = 0; j < AllUserRoles.instance.ListOfInfo.length; j++) {        
        ToLowerCase = AllUserRoles.instance.ListOfInfo[j]["RoleName"]
        InLowerCase = (ToLowerCase).toLowerCase();
        $('.umDynamicTiles').append('<div id="t' + j + '" name="' + AllUserRoles.instance.ListOfInfo[j]["RoleName"] + '"></div>');
        if (AllUserRoles.instance.ListOfInfo[j]["RoleName"] == AllUserRoles.instance.SetRole && (currentModuleId == 0 || AllUserRoles.instance.ListOfInfo[j]["ModuleId"] == currentModuleId) &&
            ((InLowerCase.indexOf($(event.target).val().toLowerCase()) != -1) || (InLowerCase.indexOf($(event.target).val().toUpperCase()) != -1))) {
            flagSearch = true
            firstGroup = AllUserRoles.instance.ListOfInfo[j]["RoleName"];
            $("#t" + j).addClass('RADUserMgmttiles').addClass('RADTileActive');
            $("#t" + j).append('<div id="group' + j + '" name="' + AllUserRoles.instance.ListOfInfo[j]["RoleName"] + '">' + AllUserRoles.instance.ListOfInfo[j]["RoleName"] + '</div>');

            $("#t" + j).closest(".RADUserMgmttiles").append('<div id="arr"></div>');
            $(".umDynamicTiles").find(".RADarrow-right").remove();
            $("#arr").addClass('fa fa-caret-right RADarrow-right').insertAfter("#t" + j);
            $("#group" + j).addClass('RADUserMgmtuser_name');
            $("#t" + j).append('<div id="group_desc' + j + '" Description="' + AllUserRoles.instance.ListOfInfo[j]["Description"] + '">' + AllUserRoles.instance.ListOfInfo[j]["Description"] + '</div>');
            $("#group_desc" + j).addClass('RADUserMgmtemail_id');
        }

        else if ((currentModuleId == 0 || AllUserRoles.instance.ListOfInfo[j]["ModuleId"] == currentModuleId) && ((InLowerCase.indexOf($(event.target).val().toLowerCase()) != -1) || (InLowerCase.indexOf($(event.target).val().toUpperCase()) != -1))) {
            flagSearch = true
            $("#t" + j).addClass('RADUserMgmttiles')
            $("#t" + j).append('<div id="group' + j + '" name="' + AllUserRoles.instance.ListOfInfo[j]["RoleName"] + '">' + AllUserRoles.instance.ListOfInfo[j]["RoleName"] + '</div>');

            $("#group" + j).addClass('RADUserMgmtuser_name');
            $("#t" + j).append('<div id="group_desc' + j + '" Description="' + AllUserRoles.instance.ListOfInfo[j]["Description"] + '">' + AllUserRoles.instance.ListOfInfo[j]["Description"] + '</div>');
            $("#group_desc" + j).addClass('RADUserMgmtemail_id');
        }
    }
    if (flagSearch == false) {
        $('.umDynamicTiles').append('<div id="RADNouserfound" class=\"RADnouserfound"\>Sorry This Role Does Not Exist !!!</div>')
        flagSearch = true
    }

    $(".RADUserMgmttiles").unbind().click(function (event) {
        AllUserRoles.instance.ClickOnEachTile(event);

    });
}

AllUserRoles.prototype.GetAllGroups = function (getallgroup) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllGroups', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        var roleGroups;        
        var arr = []
        roleGroups = JSON.parse(responseText.d);
        for (var i = 0; i < roleGroups.length; i++) {
            arr.push(roleGroups[i]["GroupName"])
        }
        AllUserRoles.instance.SelectedGroupArray = getallgroup
        AllUserRoles.instance.AllGroupList = arr
        $("#RADUserManagaementRoleScreenGroupsBody").empty()
        for (var i = 0; i < getallgroup.length; i++) {
            $("#RADUserManagaementRoleScreenGroupsBody").append("<div class=\"RADUserManagementUsersInGroup RADUserManagementSelectedUsersInGroup\">" + getallgroup[i] + "</div>");
        }
        var RoleGroupsWithoutSelectedGroups = [];
        for (var i = 0; i < roleGroups.length; i++) {
            RoleGroupsWithoutSelectedGroups.push(roleGroups[i].GroupName)
        }
        for (var i = 0; i < arr.length; i++) {
            if (getallgroup.indexOf(arr[i]) == -1)
                $("#RADUserManagaementRoleScreenGroupsBody").append("<div class=\"RADUserManagementUsersInGroup\">" + roleGroups[i]["GroupName"] + "</div>");
        }
        $(".umRightBody").unbind("click");
        $(".RADUserManagementUsersInGroup").unbind().click(function (event) { 
            AllUserRoles.instance.SelectUnSelectedData(event);
        });

    });
}

AllUserRoles.prototype.GetAllGroupsCreate = function () {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllGroups', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        var roleGroups;

        var arr = []
        roleGroups = JSON.parse(responseText.d);
        for (var i = 0; i < roleGroups.length; i++) {
            arr.push(roleGroups[i]["GroupName"])
        }
        AllUserRoles.instance.AllGroupList = arr
        for (var i = 0; i < roleGroups.length; i++) {
            $("#RADUserManagaementRoleScreenGroupsBody").append("<div class=\"RADUserManagementUsersInGroup\">" + roleGroups[i]["GroupName"] + "</div>");
        }

        $(".umRightBody").unbind("click");
        $(".RADUserManagementUsersInGroup").unbind().click(function (event) {        // $(".RADUserManagementUsersInGroup").unbind().click(function(event){
            AllUserRoles.instance.SelectUnSelectedData(event);
        });

    });
}
//to get privilges with respect to a module
AllUserRoles.prototype.GetAllPrivilegesForModuleCreate = function (moduleId) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllPrivilegesForModule', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ moduleId: moduleId })
    }).then(function (responseText) {
        var result = JSON.parse(JSON.stringify(responseText.d));        
        var arr = []
        AllUserRoles.instance.SelectedPrivilegesArray = rolePrivileges;
        for (var i = 0; i < result.length; i++) {
            arr.push(result[i])
        }

        AllUserRoles.instance.AllPrivilegesList = arr;
        $("#RADUserManagaementRoleScreenPrivilegesBody").empty();


        for (var i = 0; i < result.length; i++) {
            $("#RADUserManagaementRoleScreenPrivilegesBody").append("<div class=\"RADUserManagementUsersInGroup\">" + result[i] + "</div>");
        }
        $(".umRightBody").unbind("click");
        $(".RADUserManagementUsersInGroup").unbind().click(function (event) {  
            AllUserRoles.instance.SelectUnSelectedData(event);
        });

        $("#btnSelectAllPriviledges").unbind().click(function (event) {
            AllUserRoles.instance.SelectUnSelectedAllprivileges(event);
        });
    });

}
AllUserRoles.prototype.GetAllPrivilegesForModule = function (moduleId, isEditCase, isSameModule) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllPrivilegesForModule', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ moduleId: moduleId })
    }).then(function (responseText) {
        var result = JSON.parse(JSON.stringify(responseText.d));

        var arr = []
        $("#RADUserManagaementRoleScreenPrivilegesBody").empty();
        AllUserRoles.instance.SelectedPrivilegesArray = rolePrivileges;
        for (var i = 0; i < result.length; i++) {
            arr.push(result[i])
        }

        AllUserRoles.instance.AllPrivilegesList = arr;

        if (isEditCase || isSameModule) {
            for (var i = 0; i < rolePrivileges.length; i++) {
                $("#RADUserManagaementRoleScreenPrivilegesBody").append("<div class=\"RADUserManagementUsersInGroup RADUserManagementSelectedUsersInGroup\">" + rolePrivileges[i] + "</div>");
            }
        }
        
        if (rolePrivileges != null) {
            for (var i = 0; i < result.length; i++) {
                if (rolePrivileges.indexOf(result[i]) != -1) {

                    
                }
                else
                    $("#RADUserManagaementRoleScreenPrivilegesBody").append("<div class=\"RADUserManagementUsersInGroup\">" + result[i] + "</div>");

            }
        }
        else {
            for (var i = 0; i < result.length; i++) {
                $("#RADUserManagaementRoleScreenPrivilegesBody").append("<div class=\"RADUserManagementUsersInGroup\">" + result[i] + "</div>");
            }
        }
        $(".umRightBody").unbind("click");
        $(".RADUserManagementUsersInGroup").unbind().click(function (event) {        // $(".RADUserManagementUsersInGroup").unbind().click(function(event){
            AllUserRoles.instance.SelectUnSelectedData(event);
        });

        $("#btnSelectAllPriviledges").unbind().click(function (event) {
            AllUserRoles.instance.SelectUnSelectedAllprivileges(event);
        });
    });

}

//to create left side tiles of roles with names and description
AllUserRoles.prototype.CreateTilesOfRoles = function (ListOfRoles) {
    var firstRole = "";
    for (var j = 0; j < ListOfRoles.length; j++) {

        $('.umDynamicTiles').append('<div title="' + ListOfRoles[j]["RoleName"] + '"  id="t' + j + '" name="' + ListOfRoles[j]["RoleName"] + '"></div>');
        if (j == 0) {
            firstRole = ListOfRoles[j]["RoleName"];
            $("#t" + j).addClass('RADUserMgmttiles').addClass('RADTileActive');
            $("#t" + j).append('<div title="' + ListOfRoles[j]["RoleName"] + '" id="group' + j + '" name="' + ListOfRoles[j]["RoleName"] + '">' + ListOfRoles[j]["RoleName"] + '</div>');
            $("#t" + j).closest(".RADUserMgmttiles").append('<div id="arr"></div>');
            $(".umDynamicTiles").find(".RADarrow-right").remove();
            $("#arr").addClass('fa fa-caret-right RADarrow-right').insertAfter("#t" + j);
            $("#group" + j).addClass('RADUserMgmtuser_name');
            $("#t" + j).append('<div title="' + ListOfRoles[j]["Description"] + '"   id="group_desc' + j + '" Description="' + ListOfRoles[j]["Description"] + '">' + ListOfRoles[j]["Description"] + '</div>');
            $("#group_desc" + j).addClass('RADUserMgmtemail_id');
        }

        else {
            $("#t" + j).addClass('RADUserMgmttiles')
            $("#t" + j).append('<div  title="' + ListOfRoles[j]["RoleName"] + '" id="group' + j + '" name="' + ListOfRoles[j]["RoleName"] + '">' + ListOfRoles[j]["RoleName"] + '</div>');

            $("#group" + j).addClass('RADUserMgmtuser_name');
            $("#t" + j).append('<div title="' + ListOfRoles[j]["Description"] + '"  id="group_desc' + j + '" Description="' + ListOfRoles[j]["Description"] + '">' + ListOfRoles[j]["Description"] + '</div>');
            $("#group_desc" + j).addClass('RADUserMgmtemail_id');
        }
    }
    AllUserRoles.instance.GetRoleInfo(firstRole);

    $(".RADUserMgmttiles").unbind().click(function (event) {
        AllUserRoles.instance.ClickOnEachTile();

    });
}
AllUserRoles.prototype.ClickOnEachTile = function () {
    $("#Groupsearch_text").remove()
    $("#RADUMGroupSearch").remove()
    $("#Rolesearch_text").remove()
    $("#RADUMRoleSearch").remove()
    $("#Privileges_text").remove()
    $("#btnSelectAllPriviledges").remove()
    $("#RADUMPrivilegesSearch").remove()
    $("#RADUserManagementDeleteButton").attr("value", "Delete")
    AllUserRoles.instance.ShowHideByPrivilegeType("Delete");
    if ($("#RADUserManagementEditButton").val() == "Edit") {
        for (var i = 0; i < $("#RADUserManagaementRoleScreenUsersBody").children().length; i++) {
            if ($($("#RADUserManagaementRoleScreenUsersBody").children()[i]).hasClass("RADUserManagementSelectedUsersInGroup"))
                $($("#RADUserManagaementRoleScreenUsersBody").children()[i]).removeClass("RADUserManagementSelectedUsersInGroup")
        }
        for (var i = 0; i < $("#RADUserManagaementRoleScreenGroupsBody").children().length; i++) {
            if ($($("#RADUserManagaementRoleScreenGroupsBody").children()[i]).hasClass("RADUserManagementSelectedUsersInGroup"))
                $($("#RADUserManagaementRoleScreenGroupsBody").children()[i]).removeClass("RADUserManagementSelectedUsersInGroup")
        }
        for (var i = 0; i < $("#RADUserManagaementRoleScreenPrivilegesBody").children().length; i++) {
            if ($($("#RADUserManagaementRoleScreenPrivilegesBody").children()[i]).hasClass("RADUserManagementSelectedUsersInGroup"))
                $($("#RADUserManagaementRoleScreenPrivilegesBody").children()[i]).removeClass("RADUserManagementSelectedUsersInGroup")
        }
        AllUserRoles.instance.selectGroup(); //to show the active tile and hit service to get info of selected role
        AllUserRoles.instance.SetRole = $(event.target).closest(".RADUserMgmttiles").children()[0].innerText;
    }
    else {
        $(".RADUserManagementAlertPopUpParent").remove();
        $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
        $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
        $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
        $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserAlert\"></div>"); //revisit
        $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
        $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Alert</div>")
        $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Your Changes will revert.Want To Continue?</div>");
        $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpErrorFooter\"></div>");
        $(".RADUserManagementPopUpErrorFooter").append("<div class=\"RADUserManagementPopUpErrorYes\">Yes</div><div class=\"RADUserManagementPopUpErrorNo\">No</div>")
        $("#alertParentDiv").css("top", "-200px");
        $("#alertParentDiv").animate({ "top": "0px" });
        //  }
        $(".RADUserManagementPopUpErrorYes").unbind().click(function (event) {
            AllUserRoles.instance.ReCreateDiv();
            $("#alertParentDiv").remove();
            $("#RADUserManagementEditButton").val("Edit")
            AllUserRoles.instance.ShowHideByPrivilegeType("Edit");

        });
        $(".RADUserManagementPopUpErrorNo").unbind().click(function (event) {
            $("#alertParentDiv").remove();            
        });


    }

}
AllUserRoles.prototype.selectGroup = function () {
    $(".umDynamicTiles").find(".RADTileActive").removeClass("RADTileActive");
    $(event.target).closest(".RADUserMgmttiles").append('<div id="arr"></div>');
    $(".umDynamicTiles").find(".RADarrow-right").remove();
    $("#arr").addClass('RADarrow-right fa fa-caret-right').insertAfter($(event.target).closest(".RADUserMgmttiles"));
    $('.umSec1Body').empty();
    $(event.target).closest(".RADUserMgmttiles").addClass('RADTileActive');
    var roleName = $(event.target).closest(".RADUserMgmttiles").children()[0].innerText;
    AllUserRoles.instance.GetRoleInfo(roleName);
}

AllUserRoles.prototype.SelectUnSelectedData = function (event) {
    if ($("#RADUserManagementEditButton").attr("value") == "Save") {
        if ($(event.target).hasClass("RADUserManagementSelectedUsersInGroup")) {
            $(event.target).removeClass("RADUserManagementSelectedUsersInGroup")            
        }
        else {
            $(event.target).addClass("RADUserManagementSelectedUsersInGroup")            
        }
    }
}

AllUserRoles.prototype.SelectUnSelectedAllprivileges = function (event) {
    if ($("#RADUserManagementEditButton").attr("value") == "Save") {

        var childLength = $('#RADUserManagaementRoleScreenPrivilegesBody').children().length;

        for (var i = 0; i < childLength; i++) {
            var childElement = $('#RADUserManagaementRoleScreenPrivilegesBody').children()[i];

            if (event.target.value == "Select All") {
                childElement.className = "RADUserManagementUsersInGroup RADUserManagementSelectedUsersInGroup";
            }
            else {
                childElement.className = "RADUserManagementUsersInGroup";
            }
        }

        if (event.target.value == "Select All") {
            $(event.target).removeClass("RADUserManagementSelectAllButton")
            $(event.target).addClass("RADUserManagementUnSelectAllButton")
            $(event.target).val("Clear All");
        }
        else {
            $(event.target).removeClass("RADUserManagementUnSelectAllButton")
            $(event.target).addClass("RADUserManagementSelectAllButton")
            $(event.target).val("Select All");
        }

    }
}

AllUserRoles.prototype.GetRoleInfo = function (roleName) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetRoleInfo', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ roleName: roleName })

    }).then(function (responseText) {

        $(".umRightHeader").empty();
        $(".umRightHeader").append("<div id=\"RADUserManagaementUserLabel\" class=\"RADUserManagaementLabel\"><label id=\"lblUserLabel\">Role Name :</label></div>");
        $(".umRightHeader").append("<div id=\"lab\" class=\"RADUserMgmtuser_label\"></div>");
        $(".umRightHeader").append("<div id=\"RADUserManagaementAccountLabeDiv\" class=\"RADUserManagaementLabel\"><label id=\"lblAccountLabel\" >Module Name : </label></div>");

        $(".umRightHeader").append("<div id=\"lab1\" class=\"RADUSERManagementAccountValue\"></div>");




        $(".umRightHeader").find(".RADUserMgmtuser_label").html(roleName);
        $(".RADUserMgmtuser_label").attr('title', roleName);
        $(".umRightHeader").find(".RADUserMgmtuser_label").css({ "display": "block" });
        var roleInfo = JSON.parse(responseText.d);
        var mod = roleInfo["ModuleId"]
        AllUserRoles.instance.CurrentRoleModuleId = parseInt(mod);
        
        var modulename;
        for (var i = 0; i < AllUserRoles.instance.GetModuleForRole.length; i++) {
            if (AllUserRoles.instance.GetModuleForRole[i]["ModuleId"] == mod) 
                modulename = AllUserRoles.instance.GetModuleForRole[i]["ModuleName"]
        }
        
        $(".umRightHeader").find(".RADUSERManagementAccountValue").html(modulename);

        //ModuleName
        var roleUsers = [];
        var roleGroups = [];
        rolePrivileges = roleInfo["privileges"]
        roleUsers = roleInfo["Users"]
        roleGroups = roleInfo["groups"]
        
        AllUserRoles.instance.GetUsersForRole = roleUsers;
        AllUserRoles.instance.GetGroupsForRole = roleGroups;
        AllUserRoles.instance.GetPrivilegesForRole = rolePrivileges;
        $("#RADUserManagaementRoleScreenPrivilegesBody").empty();
        $("#RADUserManagaementRoleScreenGroupsBody").empty();
        $("#RADUserManagaementRoleScreenUsersBody").empty();
        for (var i = 0; i < rolePrivileges.length; i++) {
            $("#RADUserManagaementRoleScreenPrivilegesBody").append("<div class=\"RADUserManagementUsersInGroup\">" + rolePrivileges[i] + "</div>");
        }
        for (var i = 0; i < roleGroups.length; i++) {
            $("#RADUserManagaementRoleScreenGroupsBody").append("<div class=\"RADUserManagementUsersInGroup\">" + roleGroups[i] + "</div>");
        }
        for (var i = 0; i < roleUsers.length; i++) {
            $("#RADUserManagaementRoleScreenUsersBody").append("<div class=\"RADUserManagementUsersInGroup\">" + roleUsers[i] + "</div>");
        }

    });
    $(".umRightBody").unbind("click");
    $(".RADUserManagementUsersInGroup").unbind().click(function (event) {        // $(".RADUserManagementUsersInGroup").unbind().click(function(event){
        AllUserRoles.instance.SelectUnSelectedData(event);
    });
}

AllUserRoles.prototype.CreateRole = function () {
    var createRole = new Object();
    var roleName = $("#RADUserManagementAddNewGroupName").val();
    var roleDesc = $("#RADUserManagementAddNewGroupDesc").val();
    var moduleId = parseInt($("#RADUserManagementGroupFirstAccount").attr("module_id"));
    var Users = [];
    var Groups = [];
    var Privileges = [];
    var ArrUsers = [];
    var ArrRoles = [];
    var ArrPrivileges = [];
    ArrUsers = ($("#RADUserManagaementRoleScreenUsersBody").find(".RADUserManagementSelectedUsersInGroup"))
    ArrRoles = ($("#RADUserManagaementRoleScreenGroupsBody").find(".RADUserManagementSelectedUsersInGroup"));
    ArrPrivileges = ($("#RADUserManagaementRoleScreenPrivilegesBody").find(".RADUserManagementSelectedUsersInGroup"));
    $(ArrUsers).each(function (i, e) {
        Users.push($(e).text());
    })
    $(ArrRoles).each(function (i, e) {
        Groups.push($(e).text());
    })
    $(ArrPrivileges).each(function (i, e) {
        Privileges.push($(e).text());
    })

    // When we do an create after selecting a role/group/user by searching it, previous value are not persisiting
    // Below loop is added to add the previous selected values

    for (var j = 0; j < AllUserRoles.instance.SelectedUsersForSearch.length; j++) {
        if (!Array.contains(Users, AllUserRoles.instance.SelectedUsersForSearch[j])) {
            Users.push(AllUserRoles.instance.SelectedUsersForSearch[j]);
        }
    }

    for (var j = 0; j < AllUserRoles.instance.SelectedGroupsForSearch.length; j++) {
        if (!Array.contains(Groups, AllUserRoles.instance.SelectedGroupsForSearch[j])) {
            Groups.push(AllUserRoles.instance.SelectedGroupsForSearch[j]);
        }
    }

    for (var j = 0; j < AllUserRoles.instance.SelectedPrivilegesForSearch.length; j++) {
        if (!Array.contains(Privileges, AllUserRoles.instance.SelectedPrivilegesForSearch[j])) {
            Privileges.push(AllUserRoles.instance.SelectedPrivilegesForSearch[j]);
        }
    }


    createRole.RoleName = roleName;
    createRole.Description = roleDesc;
    createRole.ModuleId = moduleId;
    createRole.Users = Users;
    createRole.groups = Groups;
    createRole.privileges = Privileges;
    if (createRole.RoleName == "" || createRole.Description == "" || createRole.ModuleId == "Module Name" || createRole.privileges == "") {
        
        if (createRole.RoleName == "") {
            if (($(".umRightHeader").find("#RADUserGroupNameValidation").length == 0)) {
                $("<div id=\"RADUserGroupNameValidation\" class=\"RADUserGroupNameValidation\">!</div>").insertAfter($("#RADUserManagementAddNewGroupName"))        
                $("#RADUserGroupNameValidation").attr("title", "RoleName Can't Be Empty")
            }
        }
        if (createRole.Description == "") {
            if (($(".umRightHeader").find("#RADGroupDesceValidation").length == 0)) {
                $("<div id=\"RADGroupDesceValidation\" class=\"RADGroupDesceValidation\">!</div>").insertAfter($("#RADUserManagementAddNewGroupDesc"))                
                $("#RADGroupDesceValidation").attr("title", "Description Can't Be Empty")
            }
        }
        if (createRole.ModuleId == "Module Name") {
            if (($(".umRightHeader").find("#RADAccountValidation").length == 0)) {
                $("<div id=\"RADAccountValidation\" class=\"RADAccountValidation\">!</div>").insertAfter($("#RADUserManagementAddNewGroupAccount"))                
                $("#RADAccountValidation").attr("title", "Module Name Can't Be Empty")
            }
        }

        if (createRole.privileges == "") {
            if (($(".umRightBody").find("#RADUserNameValidation").length == 0)) {
                $("<div id=\"RADUserNameValidation\" class=\"RADUserManagaementRoleScreenPrivileges\">!</div>").insertAfter($("#RADUserManagaementRoleScreenPrivilegesHead"))
                $("#RADUserNameValidation").attr("title", "Select Atleast One Privilege")
            }
        }
    }
    else {        
        AllUserRoles.instance.ShowHideByPrivilegeType("Edit");
        $(event.target).val("Edit");
        var createGroupToString = JSON.stringify(createRole);
        AllUserRoles.instance.CreateRoleinfo(createGroupToString, roleName)
    }
}
AllUserRoles.prototype.CreateRoleinfo = function (createGroupToString, roleName) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/CreateRole',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ roleInfo: createGroupToString })

    }).then(function (responseText) {
        if (responseText.d == "") {
            rolenameGlobal = $($(".umRightHeader").children()[1]).text()
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperSuccessDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserSuccess\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Success</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Role has been created successfully</div>");

            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 3000);
            AllUserRoles.instance.ReCreateDivEdit(roleName)
            AllUserRoles.instance.CreateTilesOfRolesADDNew(roleName)
        }
        else {
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserFailure\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Result</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Role Creation Failed</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 3000);
            if ($("#RADUserManagementDeleteButton").attr("value") == "Cancel") {
                $("#RADUserManagementDeleteButton").attr("value", "Delete");
                AllUserRoles.instance.ShowHideByPrivilegeType("Delete");
            }
            AllUserRoles.instance.ReCreateDiv();
        }
    })

}
AllUserRoles.prototype.DeleteRole = function (event) {
    var roleName = $(".umDynamicTiles").find(".RADTileActive").children()[0].innerText;
    $(".umDynamicTiles").find(".RADTileActive").remove();
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/DeleteRole',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ roleName: roleName })

    }).then(function (responseText) {
        var res;
        res = JSON.parse(responseText.d);
        if (res == true) {
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperSuccessDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserSuccess\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Success</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Role has been deleted successfully</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () {
                $("#alertParentDiv").remove();
            }, 3000);
            var RolesObject = new AllUserRoles();
            RolesObject.init();
        }
        else {
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserFailure\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Result</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Role Deletion Failed</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 3000);
            var RolesObject = new AllUserRoles();
            RolesObject.init();
        }

    })
}

AllUserRoles.prototype.OnDeleteClick = function () {
    var deleteUser = $(".RADTileActive").children()[0].innerText;
    $("#alertParentDivdel").remove();
    $("#ContentDiv").append("<div id=\"alertParentDivdel\" class=\"RADUserManagementAlertPopUpParent\"></div>");
    $("#alertParentDivdel").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
    $("#alertParentDivdel").append("<div class=\"RADUserManagementPopUpError\"></div>")
    $("#alertParentDivdel").append("<div class=\"RADUserErrorDivLeft RADUserAlert\"></div>"); //revisit
    $("#alertParentDivdel").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
    $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Alert</div>")
    $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Are You Sure You want to delete  " + (deleteUser).toUpperCase() + " ?</div>");
    $("#alertParentDivdel").append("<div class=\"RADUserManagementPopUpErrorFooter\"></div>");
    $(".RADUserManagementPopUpErrorFooter").append("<div class=\"RADUserManagementPopUpErrorYes\">Yes</div><div class=\"RADUserManagementPopUpErrorNo\">No</div>")
    $("#alertParentDivdel").css("top", "-200px");
    $("#alertParentDivdel").animate({ "top": "0px" });
    $(".RADUserManagementPopUpErrorYes").unbind().click(function (event) {
        $("#alertParentDivdel").remove();
        AllUserRoles.instance.DeleteRole(event);
        AllUserRoles.instance.ShowHideByPrivilegeType("Edit");
        $("#RADUserManagementEditButton").val("Edit")
        var deleteUser = $(".RADTileActive").children()[0].innerText;

        //$("#" + RADUserManagement.contentBodyId).empty();
        $(".usermgmt").remove();

    });
    $(".RADUserManagementPopUpErrorNo").unbind().click(function (event) {
        $("#alertParentDivdel").remove();
    });
}

AllUserRoles.prototype.UpdateRole = function () {
    var updateRoleDetail = new Object();
    var Users = [];
    var Groups = [];
    var Privileges = [];
    var ArrUsers = [];
    var ArrGroups = [];
    var ArrPrivileges = [];
    var moduleid;
    updateRoleDetail.RoleName = $("#lab").html();
    for (var i = 0; i < AllUserRoles.instance.GetModuleForRole.length; i++) {
        if (AllUserRoles.instance.GetModuleForRole[i]["ModuleName"] == $("#RADUserManagementGroupFirstAccount").text()) {
            moduleid = AllUserRoles.instance.GetModuleForRole[i]["ModuleId"]
        }
    }
    
    updateRoleDetail.ModuleId = moduleid;
    ArrUsers = ($("#RADUserManagaementRoleScreenUsersBody").find(".RADUserManagementSelectedUsersInGroup"))
    ArrPrivileges = ($("#RADUserManagaementRoleScreenPrivilegesBody").find(".RADUserManagementSelectedUsersInGroup"));
    ArrGroups = ($("#RADUserManagaementRoleScreenGroupsBody").find(".RADUserManagementSelectedUsersInGroup"));
    $(ArrUsers).each(function (i, e) {
        Users.push($(e).text());
    })

    $(ArrGroups).each(function (i, e) {
        Groups.push($(e).text());
    })
    $(ArrPrivileges).each(function (i, e) {
        Privileges.push($(e).text());
    })


    // When we do an update after selecting a role/group/user by searching it, previous value are not persisiting
    // Below loop is added to add the previous selected values

    for (var j = 0; j < AllUserRoles.instance.SelectedUsersForSearch.length; j++) {
        if (!Array.contains(Users, AllUserRoles.instance.SelectedUsersForSearch[j])) {
            Users.push(AllUserRoles.instance.SelectedUsersForSearch[j]);
        }
    }

    for (var j = 0; j < AllUserRoles.instance.SelectedGroupsForSearch.length; j++) {
        if (!Array.contains(Groups, AllUserRoles.instance.SelectedGroupsForSearch[j])) {
            Groups.push(AllUserRoles.instance.SelectedGroupsForSearch[j]);
        }
    }

    for (var j = 0; j < AllUserRoles.instance.SelectedPrivilegesForSearch.length; j++) {
        if (!Array.contains(Privileges, AllUserRoles.instance.SelectedPrivilegesForSearch[j])) {
            Privileges.push(AllUserRoles.instance.SelectedPrivilegesForSearch[j]);
        }
    }

    updateRoleDetail.Users = Users;
    updateRoleDetail.groups = Groups;
    updateRoleDetail.privileges = Privileges;
    updateRoleDetail.RoleName = $(".RADTileActive").children()[0].innerText;
    updateRoleDetail.Description = $(".RADTileActive").children()[1].innerText;

    if (updateRoleDetail.privileges == "") {
        $("#RADUserManagementEditButton").val("Save");
        if (($(".umRightBody").find("#RADUserNameValidation").length == 0)) {            
            $("<div id=\"RADUserNameValidation\" class=\"RADUserManagaementRoleScreenPrivileges\">!</div>").insertAfter($("#RADUserManagaementRoleScreenPrivilegesHead"))
            $("#RADUserNameValidation").attr("title", "Select Atleast One Privilege")
        }
    }
    else {        
        var myString = JSON.stringify(updateRoleDetail);
        var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
        $.ajax({
            url: url + '/ModifyRole',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ roleInfo: myString })

        }).then(function (responseText) {
            if (responseText.d == "") {
                $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
                $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperSuccessDiv\"></div>");
                $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
                $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserSuccess\"><img src=\"../App_Themes/SuperAwesome/Images/icons/rad_alert_success.png\"></div>"); //revisit
                $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
                $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Success</div>")
                $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Role has been updated successfully</div>");                
                
                $("#alertParentDiv").css("top", "-200px");
                $("#alertParentDiv").animate({ "top": "0px" });
                setInterval(function () { $("#alertParentDiv").remove() }, 3000);
                AllUserRoles.instance.ReCreateDiv()
            }
            else {
                rolenameGlobal = $($(".umRightHeader").children()[1]).text();
                $(".RADUserManagementCancelButton").val("Delete");
                AllUserRoles.instance.ShowHideByPrivilegeType("Delete");
                $(".RADUserManagementCancelButton").attr('id', 'RADUserManagementDeleteButton');
                $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
                $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
                $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
                $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserFailure\"></div>"); //revisit
                $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
                $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Result</div>")
                $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Role Modification Failed</div>");
                $("#alertParentDiv").css("top", "-200px");
                $("#alertParentDiv").animate({ "top": "0px" });
                setInterval(function () { $("#alertParentDiv").remove() }, 3000);
                AllUserRoles.instance.ReCreateDivEdit(rolenameGlobal)
            }
        });
    }
}
AllUserRoles.prototype.ReCreateDivEdit = function (rolenameGlobal) {
    AllUserRoles.instance.ShowHideByPrivilegeType("Edit");
    AllUserRoles.instance.ShowHideByPrivilegeType("Delete");
    if ($("#RADUserManagementEditButton").attr("value") == "Save")
        $("#RADUserManagementEditButton").attr("value", "Edit");
    if ($("#RADUserManagementDeleteButton").attr("value") == "Cancel")
        $("#RADUserManagementDeleteButton").attr("value", "Delete");
    if ($("#RADUserManagementAddNewGroupName") != null)
        $("#RADUserManagementAddNewGroupName").remove();
    if ($("#RADUserManagementAddNewGroupDesc") != null)
        $("#RADUserManagementAddNewGroupDesc").remove();
    $("#RADUserManagementAddNewGroupAccount").remove();
    AllUserRoles.instance.ShowHideByPrivilegeType("Add");

    $("#ModuleDivLeftMenu").show();
    $(".searchBarUm").show();

    $($(".umRightHeader").children()[0]).css({ "display": "block" });
    $($(".umRightHeader").children()[1]).css({ "display": "block" });
    $($(".umRightHeader").children()[2]).css({ "display": "block" });
    $($(".umRightHeader").children()[3]).css({ "display": "block" });    

    $("#RADUMGroupSearch").remove()
    $("#RADUMRoleSearch").remove()
    $("#Groupsearch_text").remove()
    $("#Rolesearch_text").remove()
    $("#Privileges_text").remove()
    $("#btnSelectAllPriviledges").remove()
    $("#RADUMPrivilegesSearch").remove()
    $("#RADUserGroupNameValidation").remove()
    $("#RADGroupDesceValidation").remove()
    $("#RADAccountValidation").remove()
    $("#RADUserManagaementRoleScreenUsersSSS").remove()
    $("#RADUserManagaementRoleScreenGroupsSSS").remove()
    $("#RADUserNameValidation").remove()
    AllUserRoles.instance.GetRoleInfo(rolenameGlobal)
}
AllUserRoles.prototype.ReCreateDiv = function () {
    if ($("#RADUserManagementEditButton").attr("value") == "Save")
        $("#RADUserManagementEditButton").attr("value", "Edit");
    AllUserRoles.instance.ShowHideByPrivilegeType("Edit");
    if ($("#RADUserManagementDeleteButton").attr("value") == "Cancel")
        $("#RADUserManagementDeleteButton").attr("value", "Delete");
    AllUserRoles.instance.ShowHideByPrivilegeType("Delete");
    if ($("#RADUserManagementAddNewGroupName") != null)
        $("#RADUserManagementAddNewGroupName").remove();
    if ($("#RADUserManagementAddNewGroupDesc") != null)
        $("#RADUserManagementAddNewGroupDesc").remove();
    AllUserRoles.instance.ShowHideByPrivilegeType("Add");
    
    $(".searchBarUm").show();
    $("#ModuleDivLeftMenu").show();
    $("#RADUserManagementAddNewGroupAccount").remove();    

    $(".umRightHeader").empty();
    var rolename = $(".umDynamicTiles").find(".RADTileActive").children()[0].innerText;

    $("#RADUMGroupSearch").remove()
    $("#RADUMRoleSearch").remove()
    $("#Groupsearch_text").remove()
    $("#Rolesearch_text").remove()
    $("#Privileges_text").remove()
    $("#btnSelectAllPriviledges").remove()
    $("#RADUMPrivilegesSearch").remove()
    $("#RADUserGroupNameValidation").remove()
    $("#RADGroupDesceValidation").remove()
    $("#RADAccountValidation").remove()
    $("#RADUserManagaementRoleScreenUsersSSS").remove()
    $("#RADUserManagaementRoleScreenGroupsSSS").remove()
    $("#RADUserNameValidation").remove()
    $("#RADUserManagementEditButton").removeAttr("createGroup")
    AllUserRoles.instance.GetRoleInfo(rolename)
}
AllUserRoles.prototype.CreateTilesOfRolesADDNew = function (rolenameADD) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllRoles', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'

    }).then(function (responseText) {
        var firstRole = []
        firstRole = JSON.parse(responseText.d);

        $(".umDynamicTiles").empty()
        $(".umDynamicTiles").find(".RADTileActive").remove()
        for (var j = 0; j < firstRole.length; j++) {
            if (rolenameADD == firstRole[j]["RoleName"]) {
                $(".umDynamicTiles").prepend('<div title="' + firstRole[j]["RoleName"] + '"   id="tADD' + j + '" name="' + firstRole[j]["RoleName"] + '"></div>')
                $("#tADD" + j).addClass('RADUserMgmttiles').addClass('RADTileActive');
                $("#tADD" + j).append('<div title="' + firstRole[j]["RoleName"] + '" id="group' + j + '" name="' + firstRole[j]["RoleName"] + '">' + firstRole[j]["RoleName"] + '</div>');
                $("#tADD" + j).closest(".RADUserMgmttiles").append('<div id="arr"></div>');

                $("#arr").addClass('fa fa-caret-right RADarrow-right').insertAfter("#tADD" + j);
                $("#group" + j).addClass('RADUserMgmtuser_name');
                $("#tADD" + j).append('<div title="' + firstRole[j]["Description"] + '" id="group_desc' + j + '" Description="' + firstRole[j]["Description"] + '">' + firstRole[j]["Description"] + '</div>');
                $("#group_desc" + j).addClass('RADUserMgmtemail_id');
            }
            else {
                $(".umDynamicTiles").append('<div  title="' + firstRole[j]["RoleName"] + '" id="tADD' + j + '" name="' + firstRole[j]["RoleName"] + '"></div>')
                $("#tADD" + j).addClass('RADUserMgmttiles')
                $("#tADD" + j).append('<div title="' + firstRole[j]["RoleName"] + '" id="group' + j + '" name="' + firstRole[j]["RoleName"] + '">' + firstRole[j]["RoleName"] + '</div>');

                $("#group" + j).addClass('RADUserMgmtuser_name');
                $("#tADD" + j).append('<div title="' + firstRole[j]["Description"] + '" id="group_desc' + j + '" Description="' + firstRole[j]["Description"] + '">' + firstRole[j]["Description"] + '</div>');
                $("#group_desc" + j).addClass('RADUserMgmtemail_id');
            }
        }
        $("#RADUserManagementEditButton").removeAttr("createGroup")
        $(".RADUserMgmttiles").unbind().click(function (event) {
            AllUserRoles.instance.ClickOnEachTile();
        });
    });

}

AllUserRoles.prototype.SearchAction = function (event) {

    $("#search_text").css({ 'display': 'block' })
}

AllUserRoles.prototype.GetRolesByModule = function (moduleId) {
    $(".umDynamicTiles").empty();
    var isRoleExistForModule = false;
    var firstRole = "";

    for (var i = 0; i < AllUserRoles.instance.ListOfInfo.length; i++) {
        if (moduleId == 0 || AllUserRoles.instance.ListOfInfo[i]["ModuleId"] == moduleId) {
            if (firstRole == "")
                firstRole = AllUserRoles.instance.ListOfInfo[i]["RoleName"];
            var isRoleExistForModule = true;
            $('.umDynamicTiles').append('<div title="' + AllUserRoles.instance.ListOfInfo[i]["RoleName"] + '"  id="t' + i + '" name="' + AllUserRoles.instance.ListOfInfo[i]["RoleName"] + '"></div>');

            if (AllUserRoles.instance.ListOfInfo[i]["RoleName"] == firstRole)
                $("#t" + i).addClass('RADUserMgmttiles').addClass('RADTileActive');
            else
                $("#t" + i).addClass('RADUserMgmttiles')
            $("#t" + i).append('<div  title="' + AllUserRoles.instance.ListOfInfo[i]["RoleName"] + '" id="group' + i + '" name="' + AllUserRoles.instance.ListOfInfo[i]["RoleName"] + '">' + AllUserRoles.instance.ListOfInfo[i]["RoleName"] + '</div>');
            if (AllUserRoles.instance.ListOfInfo[i]["RoleName"] == firstRole) {
                $("#t" + i).closest(".RADUserMgmttiles").append('<div id="arr"></div>');
                $(".umDynamicTiles").find(".RADarrow-right").remove();
                $("#arr").addClass('fa fa-caret-right RADarrow-right').insertAfter("#t" + i);
            }
            $("#group" + i).addClass('RADUserMgmtuser_name');
            $("#t" + i).append('<div title="' + AllUserRoles.instance.ListOfInfo[i]["Description"] + '"  id="group_desc' + i + '" Description="' + AllUserRoles.instance.ListOfInfo[i]["Description"] + '">' + AllUserRoles.instance.ListOfInfo[i]["Description"] + '</div>');
            $("#group_desc" + i).addClass('RADUserMgmtemail_id');
        }
    }

    if (isRoleExistForModule == false) {
        $('.umDynamicTiles').append('<div id="RADNouserfound" class=\"RADnouserfound"\>Sorry No Roles Exist for this Module !!!</div>')
    }

    else {

        AllUserRoles.instance.GetRoleInfo(firstRole);

        $(".RADUserMgmttiles").unbind().click(function (event) {
            AllUserRoles.instance.ClickOnEachTile(event);

        });
    }
}

AllUserRoles.prototype.OnEditRole = function () {

    if ($(".RADTileActive").children()[0] == null) {

        if ($("#alertParentDivEdit") != null)
            $("#alertParentDivEdit").remove();
        $("#ContentDiv").append("<div id=\"alertParentDivEdit\" class=\"RADUserManagementAlertPopUpParent\"></div>");
        $("#alertParentDivEdit").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
        $("#alertParentDivEdit").append("<div class=\"RADUserManagementPopUpError\"></div>")
        $("#alertParentDivEdit").append("<div class=\"RADUserErrorDivLeft RADUserAlert\"></div>"); //revisit
        $("#alertParentDivEdit").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
        $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Alert</div>")
        $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Please select role to edit </div>");
        $("#alertParentDivEdit").append("<div class=\"RADUserManagementPopUpErrorFooter\"></div>");
        $(".RADUserManagementPopUpErrorFooter").append("<div class=\"RADUserManagementPopUpErrorYes\">OK</div>")
        $("#alertParentDivEdit").css("top", "-200px");
        $("#alertParentDivEdit").animate({ "top": "0px" });
        $(".RADUserManagementPopUpErrorYes").unbind().click(function (event) {
            $("#alertParentDivEdit").remove();
        });


    }
    else {
        AllUserRoles.instance.SelectedPrivilegesForSearch = []
        AllUserRoles.instance.SelectedGroupsForSearch = []
        AllUserRoles.instance.SelectedUsersForSearch = []
        $(event.target).val("Save");
        GlobalFlagSearch = false
        $(".searchBarUm").hide();
        $("#ModuleDivLeftMenu").hide();
        $(".RADUserCreatePlusCircle").hide()
        $("#ModuleDivLeftMenu").hide();
        $("<div id=\"searchmain\"</div>").insertAfter($('#RADUserManagaementRoleScreenGroupsHead'))
        $("<div id=\"searchUserSearch\"</div>").insertAfter($('#RADUserManagaementRoleScreenUsersHead'))
        $('#searchUserSearch').append("<div id=\"RADUMGroupSearch\"  class=\"fa fa-search searchIconGridViewForUserGroup\"></div>")
        $('#searchUserSearch').append("<input type=\"text\" name=\"search_text\" id=\"Groupsearch_text\" placeholder=\"Search\"></div>")
        $('#searchmain').append("<div id=\"RADUMRoleSearch\"  class=\"fa fa-search searchIconGridViewForUserRoles\"></div>")
        $('#searchmain').append("<input type=\"text\" name=\"search_text\" id=\"Rolesearch_text\" placeholder=\"Search\"></div>")
        $("<div id=\"searchmain2\"</div>").insertAfter($('#RADUserManagaementRoleScreenPrivilegesHead'))
        $('#searchmain2').append("<div id=\"RADUMPrivilegesSearch\"  class=\"fa fa-search searchIconGridViewForUserPrivileges\"></div>")
        $('#searchmain2').append("<input type=\"text\" name=\"search_text\" id=\"Privileges_text\" placeholder=\"Search\"></div>")
        $('#searchmain2').append("<input type=\"button\" name=\"btnSelectAllPriviledges\" id=\"btnSelectAllPriviledges\" class=\"RADUserManagementSelectAllButton\" value=\"Select All\"/>");

        var roleName = $(".umRightHeader").find("#lab").text();
        var modulename = $(".umRightHeader").find("#lab1").text();
        $(".umRightHeader").empty();

        $(".umRightHeader").append("<div id=\"RADUserManagaementUserLabel\" class=\"RADUserManagaementLabel\"><label id=\"lblUserLabel\" >Role Name:</label></div>");
        $(".umRightHeader").append("<div id=\"lab\" class=\"RADUserMgmtuser_label\"></div>");
        $(".umRightHeader").append("<div id=\"RADUserManagaementAccountLabeDiv\" class=\"RADUserManagaementLabel\"><label id=\"lblAccountLabel\" >Module Name: </label></div>");

        $(".umRightHeader").find(".RADUserMgmtuser_label").html(roleName);
        $(".RADUserMgmtuser_label").attr('title', roleName);
        $(".umRightHeader").find(".RADUserMgmtuser_label").css({ "display": "block" });

        $(".umRightHeader").append("<div id=\"lab1\" class=\"RADUSERManagementAccountValue\"><div id=\"RADUserManagementAddNewGroupAccount\" class=\"RADUserManagementAddNewGroupAccount\"></div></div>");
        $("#RADUserManagementAddNewGroupAccount").append("<div id=\"RADUserManagementGroupFirstAccount\" class=\"RADUserManagementGroupFirstAccount\">" + modulename + "</div>")
        $("#RADUserManagementAddNewGroupAccount").append("<div class=\"fa fa-caret-down RADUserManagementGroupDownArrow\"></div>")

        for (var i = 0; i < AllUserRoles.instance.GetModuleForRole.length; i++) {
            if (AllUserRoles.instance.GetModuleForRole[i]["ModuleName"] == modulename)
                var module = AllUserRoles.instance.GetModuleForRole[i]["ModuleId"]
        }        
        var isSameModule = AllUserRoles.instance.CurrentRoleModuleId == parseInt(module) ? true : false;
        AllUserRoles.instance.GetAllPrivilegesForModule(parseInt(module), true)
        var moduleId = $(event.target).attr("module_id");

        $("#RADUserManagementDeleteButton").show();
        $("#RADUserManagementDeleteButton").val("Cancel");
        $(".umSec1Body").empty();
        $("#RADUserManagaementRoleScreenPrivilegesBody").empty();
        $("#RADUserManagaementRoleScreenGroupsBody").empty();
        $("#RADUserManagaementRoleScreenUsersBody").empty();
        AllUserRoles.instance.GetAllModules("edit");

    }

}

var AllUserAccounts = function () {
    // this.someObjectVariable = "anything";
};
var Priviliges = new Array();
AllUserAccounts.instance = undefined;
AllUserAccounts.prototype.init = function (sandBox) {
    AllUserAccounts.instance = this;
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetTagTemplate', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {

        //$("#" + RADUserManagement.contentBodyId).empty();
        $(".usermgmt").remove();
        $("#" + RADUserManagement.contentBodyId).append(responseText.d);

        AllUserAccounts.instance.GetAuthorizationPrivileges()

        window.ScrollVariable = 0;
        $("#pageHeaderReportRange_dateInfo").css("display", "none")
        $(".daterangepicker").css("display", "none")
        $(".umRightHeader").children()[0].remove(); //to remove divs of username
        $(".umRightHeader").find(".RADUserMgmtuser_label").remove();
        $(".umSec1").remove()
        $(".umSec2").remove()
        $(".RADUserManagaementLabel").remove();
        $(".RADUserManagementEditButton").remove();
        $(".RADUserManagementDeleteButton").remove();
        $(".UMRightHeaderEdit").empty()
        $(".UMRightHeaderEdit").append("<div id=\"RADUserManagementAccountDeleteButton\" class=\"RADUserManagementAccountDeleteButton\">Delete</div>");
        AllUserAccounts.instance.ShowHideByPrivilegeType("Delete");
        $("<div class=\"HorizontalRule\"> </div>").insertAfter($('.UMRightHeaderEdit'))
        AllUserAccounts.instance.BindEvents();
        AllUserAccounts.instance.GetAllAccounts();
    });
}
AllUserAccounts.prototype.GetAuthorizationPrivileges = function () {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAuthorizationPrivileges', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'

    }).then(function (responseText) {
        if (responseText.d == "admin") {        
            $(".RADUserCreatePlusCircle").show()            
            $(".RADUserManagementAccountDeleteButton").show()
            Priviliges = ['Add Account', 'Delete Account']
        }
        else {
            var ResponseForCreation = [];
            if (responseText.d.length > 0)
                ResponseForCreation = JSON.parse(responseText.d);
            Priviliges = []
            for (var i = 0; i < ResponseForCreation.length; i++) {
                if (ResponseForCreation[i].pageId == "RAD_Account") {
                    for (var j = 0; j < ResponseForCreation[i].Privileges.length; j++)
                        Priviliges.push(ResponseForCreation[i].Privileges[j])
                }
            }

            if (Priviliges.indexOf("Add Account") != -1) {
                $('.RADUserCreatePlusCircle').show();
            }
            else {
                $('.RADUserCreatePlusCircle').hide();
            }
            if (Priviliges.indexOf("Delete Account") != -1) {
                $(".RADUserManagementAccountDeleteButton").show()
            }
            else {
                $(".RADUserManagementAccountDeleteButton").hide()
            }
        }
    })
}
AllUserAccounts.prototype.OnDeleteClick = function () {
    $("#alertParentDiv").remove();
    var AccountName = $(".umDynamicTiles").find(".RADTileActive").children()[0].innerText;
    $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
    $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
    $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
    $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserAlert\"></div>"); //revisit
    $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
    $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Alert</div>")
    $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Are You Sure You want to delete " + (AccountName).toUpperCase() + "?</div>");
    $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpErrorFooter\"></div>");
    $(".RADUserManagementPopUpErrorFooter").append("<div class=\"RADUserManagementPopUpErrorYes\">Yes</div><div class=\"RADUserManagementPopUpErrorNo\">No</div>")
    $("#alertParentDiv").css("top", "-200px");
    $("#alertParentDiv").animate({ "top": "0px" });
    $(".RADUserManagementPopUpErrorYes").unbind().click(function (event) {
        $("#alertParentDiv").remove();
        var AccountName = $(".umDynamicTiles").find(".RADTileActive").children()[0].innerText;
        AllUserAccounts.instance.deleteAccount(AccountName);
        

    });
    $(".RADUserManagementPopUpErrorNo").unbind().click(function (event) {
        $("#alertParentDiv").remove();
    });
}

AllUserAccounts.prototype.BindEvents = function () {
    $(".UMRightHeaderEdit").on("click", ".RADUserManagementAccountDeleteButton", function (event) {
        AllUserAccounts.instance.OnDeleteClick()
    });
    $(".umLeftSec").on('click', '.RADsearchIconGridView', function (event) {
        if ($('#search_text:visible').length) {            
            $("#search_text").hide("slow", function () {
                // Animation complete.
            });
        }
        else {            
            $("#search_text").show("slow", function () {
                // Animation complete.
            });
        }        
    });

    $(".UMRightHeaderEdit").on("click", ".RADUserManagementAccountSaveButton", function (event) {
        var AddNewAccount = $(".RADUserManagementAddNewAccountName").val();
        var AddNewDesc = $(".RADUserManagementAddNewAccountDesc").val();
        
        AllUserAccounts.prototype.CreateAccount(AddNewAccount, AddNewDesc)
        $(".umRightHeader").empty();
        if ($("#alertParentDiv") == null) {
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperSuccessDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserSuccess\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Success</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Account has been created successfully</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 5000);
        }
    });

    $(".UMRightHeaderEdit").on("click", ".RADUserManagementCancelButton", function (event) {
        var NewCreatedAccountName = $(".umDynamicTiles").find(".RADTileActive").children()[0].innerHTML
        var NewCreatedAccountDescription = $(".umDynamicTiles").find(".RADTileActive").children()[1].innerHTML
        AllUserAccounts.instance.DataInfoOntileClick(NewCreatedAccountName, NewCreatedAccountDescription);
    });

    $(".RADUserCreatePlusCircle").unbind().click(function (event) {
        $(".umRightBody").empty();
        var ButtonVal
        $(".umRightBody").append("<div class=\"RADUserManagementEditNewAccountName\"></div>");
        $(".RADUserManagementEditNewAccountName").append("<div class=\"RADUserManagementEditNewAccountNameLabel\">Account Name</div>")
        $(".RADUserManagementEditNewAccountName").append("<input id=\"RADUserManagementAddNewAccountName\" class=\"RADUserManagementAddNewAccountName\" placeholder=\"Account Name\"></input>")
        $(".umRightBody").append("<div class=\"RADUserManagementEditNewAccountDescription\"></div>");
        $(".RADUserManagementEditNewAccountDescription").append("<div class=\"RADUserManagementEditNewAccountNameLabel\">Account Description</div>")
        $(".RADUserManagementEditNewAccountDescription").append("<input id=\"RADUserManagementAddNewAccountDesc\" class=\"RADUserManagementAddNewAccountDesc\" placeholder=\"Account Description\"></input>");
        $(".UMRightHeaderEdit").empty();
        $(".UMRightHeaderEdit").append("<div class=\"RADUserManagementAccountSaveButton\">Save</div>");
        $(".UMRightHeaderEdit").append("<div id=\"RADUserManagementCancelButtonID\" class=\"RADUserManagementCancelButton\">Cancel</div>");
        $("#RADUserManagementCancelButtonID").css({ "margin-top": "22px" });
    }); 

    $("#RADUserManagementScrollDownArrow").unbind().click(function (event) {
        ScrollVariable = ScrollVariable + 50;
        $(".umDynamicTiles").scrollTop(ScrollVariable);
    });
    $("#RADUserManagementScrollUpArrow").unbind().click(function (event) {
        if (ScrollVariable > 0) {
            ScrollVariable = ScrollVariable - 50;
            $(".umDynamicTiles").scrollTop(ScrollVariable);
        }
    });
    $("#search_text").keyup(function (event) {
        AllUserAccounts.instance.CreateSearchText(event);
    });
}

AllUserAccounts.prototype.deleteAccount = function (AccountName) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/DeleteAccount',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ accountName: AccountName })

    }).then(function (responseText) {
        res = JSON.parse(responseText.d);
        if (res == true) {
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperSuccessDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserSuccess\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Success</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Account has been deleted successfully</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () {
                $("#alertParentDiv").remove();
            }, 5000);
            var usersObject = new AllUserAccounts();
            usersObject.init(usersObject);
        }
        else {
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserFailure\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Result</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">account Deletion Failed</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 5000);
        }
    });
}


AllUserAccounts.prototype.CreateAccount = function (AccountName, accountDesc) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/AddAccount',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ accountName: AccountName, accountDesc: accountDesc })

    }).then(function (responseText) {
        var AccountNameNewCreate = $("#RADUserManagementAddNewAccountName").val()
        if (responseText.d == "") {
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperSuccessDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserSuccess\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Success</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Account has been created successfully</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 5000);            
            AllUserAccounts.prototype.GetAllAccountsCreate(AccountNameNewCreate)
        }
        else {
            $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
            $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
            $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserFailure\"></div>"); //revisit
            $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Result</div>")
            $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Account Creation Failed</div>");
            $("#alertParentDiv").css("top", "-200px");
            $("#alertParentDiv").animate({ "top": "0px" });
            setInterval(function () { $("#alertParentDiv").remove() }, 5000);
            var usersObject = new AllUserAccounts();
            usersObject.init(usersObject);
        }
    });
}


AllUserAccounts.prototype.GetAllAccounts = function () {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllAccounts', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'
    }).then(function (responseText) {
        var res;
        ListOfAccounts = JSON.parse(responseText.d);        
        var firstAccount = "";
        var firstAccountdesc = "";
        AllUserAccounts.instance.ListOfInfo = ListOfAccounts;
        for (var j = 0; j < ListOfAccounts.length; j++) {

            $('.umDynamicTiles').append('<div id="t' + j + '" name="' + ListOfAccounts[j]["AccountName"] + '"></div>');
            if (j == 0) {
                firstGroup = ListOfAccounts[j]["AccountName"];
                firstAccountdesc = ListOfAccounts[j]["Description"]
                $("#t" + j).addClass('RADUserMgmttiles').addClass('RADTileActive');
                $("#t" + j).append('<div id="account' + j + '" name="' + ListOfAccounts[j]["AccountName"] + '">' + ListOfAccounts[j]["AccountName"] + '</div>');

                $("#t" + j).closest(".RADUserMgmttiles").append('<div id="arr"></div>');
                $(".umDynamicTiles").find(".RADarrow-right").remove();
                $("#arr").addClass('fa fa-caret-right RADarrow-right').insertAfter("#t" + j);
                $("#account" + j).addClass('RADUserMgmtuser_name');
                $("#t" + j).append('<div id="account_desc' + j + '" Description="' + ListOfAccounts[j]["Description"] + '">' + ListOfAccounts[j]["Description"] + '</div>');
                $("#account_desc" + j).addClass('RADUserMgmtemail_id');
            }

            else {
                $("#t" + j).addClass('RADUserMgmttiles')
                $("#t" + j).append('<div id="account' + j + '" name="' + ListOfAccounts[j]["AccountName"] + '">' + ListOfAccounts[j]["AccountName"] + '</div>');

                $("#account" + j).addClass('RADUserMgmtuser_name');
                $("#t" + j).append('<div id="account_desc' + j + '" Description="' + ListOfAccounts[j]["Description"] + '">' + ListOfAccounts[j]["Description"] + '</div>');
                $("#account_desc" + j).addClass('RADUserMgmtemail_id');
            }
        }
        AllUserAccounts.instance.DataInfoOntileClick(firstGroup, firstAccountdesc)
        $(".RADUserMgmttiles").unbind().click(function (event) {
            AllUserAccounts.instance.selectGroup(event);
            AllUserAccounts.instance.DataOntileClick(event)

        });
    })
}


AllUserAccounts.prototype.selectGroup = function (event) {
    $(".umDynamicTiles").find(".RADTileActive").removeClass("RADTileActive");
    $(event.target).closest(".RADUserMgmttiles").append('<div id="arr"></div>');
    $(".umDynamicTiles").find(".RADarrow-right").remove();
    $("#arr").addClass('RADarrow-right fa fa-caret-right').insertAfter($(event.target).closest(".RADUserMgmttiles"));
    $('.umSec1Body').empty();
    $(event.target).closest(".RADUserMgmttiles").addClass('RADTileActive');
    var groupName = $(event.target).closest(".RADUserMgmttiles").children()[0].innerText;
}


AllUserAccounts.prototype.CreateSearchText = function (event) {
    var flagSearch = false
    $(".umDynamicTiles").empty();
    var ToLowerCase = []
    var InLowerCase = [];
    for (var j = 0; j < AllUserAccounts.instance.ListOfInfo.length; j++) {
        ToLowerCase = AllUserAccounts.instance.ListOfInfo[j]["AccountName"]
        InLowerCase = (ToLowerCase).toLowerCase();
        $('.umDynamicTiles').append('<div id="t' + j + '" name="' + AllUserAccounts.instance.ListOfInfo[j]["AccountName"] + '"></div>');
        if (AllUserAccounts.instance.ListOfInfo[j]["AccountName"] == AllUserAccounts.instance.SetAccount && ((InLowerCase.indexOf($(event.target).val().toLowerCase()) != -1) || (InLowerCase.indexOf($(event.target).val().toUpperCase()) != -1))) {
            flagSearch = true
            firstGroup = ListOfAccounts[j]["AccountName"];
            $("#t" + j).addClass('RADUserMgmttiles').addClass('RADTileActive');
            $("#t" + j).append('<div id="account' + j + '" name="' + AllUserAccounts.instance.ListOfInfo[j]["AccountName"] + '">' + AllUserAccounts.instance.ListOfInfo[j]["AccountName"] + '</div>');

            $("#t" + j).closest(".RADUserMgmttiles").append('<div id="arr"></div>');
            $(".umDynamicTiles").find(".RADarrow-right").remove();
            $("#arr").addClass('fa fa-caret-right RADarrow-right').insertAfter("#t" + j);
            $("#account" + j).addClass('RADUserMgmtuser_name');
            $("#t" + j).append('<div id="account_desc' + j + '" Description="' + AllUserAccounts.instance.ListOfInfo[j]["Description"] + '">' + AllUserAccounts.instance.ListOfInfo[j]["Description"] + '</div>');
            $("#account_desc" + j).addClass('RADUserMgmtemail_id');
        }

        else if (((InLowerCase.indexOf($(event.target).val().toLowerCase()) != -1) || (InLowerCase.indexOf($(event.target).val().toUpperCase()) != -1))) {
            flagSearch = true
            $('.umDynamicTiles').append('<div id="t' + j + '" name="' + AllUserAccounts.instance.ListOfInfo[j]["AccountName"] + '"></div>');
            $("#t" + j).addClass('RADUserMgmttiles')
            $("#t" + j).append('<div id="account' + j + '" name="' + ListOfAccounts[j]["AccountName"] + '">' + ListOfAccounts[j]["AccountName"] + '</div>');

            $("#account" + j).addClass('RADUserMgmtuser_name');
            $("#t" + j).append('<div id="account_desc' + j + '" Description="' + ListOfAccounts[j]["Description"] + '">' + ListOfAccounts[j]["Description"] + '</div>');
            $("#account_desc" + j).addClass('RADUserMgmtemail_id');
        }
    }

    if (flagSearch == false) {
        $('.umDynamicTiles').append('<div id="RADNouserfound" class=\"RADnouserfound"\>Sorry This Account Does Not Exist !!!</div>')
        flagSearch = true
    }

    $(".RADUserMgmttiles").unbind().click(function (event) {
        AllUserAccounts.instance.DataInfoOntileClickSearch(event);

    });
}
AllUserAccounts.prototype.ClickOnEachTile = function (event) {
    AllUserAccounts.instance.selectGroup(event)
}
AllUserAccounts.prototype.DataOntileClick = function (event) {
    if ($(".UMRightHeaderEdit").children().length == 2) {
        if ($("#alertParentDiv") != null)
            $("#alertParentDiv").remove();
        $("#ContentDiv").append("<div id=\"alertParentDiv\" class=\"RADUserManagementAlertPopUpParent\"></div>");
        $("#alertParentDiv").append("<div class=\"RADUserManagementPopupAlertUpperDiv\"></div>");
        $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpError\"></div>")
        $("#alertParentDiv").append("<div class=\"RADUserErrorDivLeft RADUserAlert\"></div>"); //revisit
        $("#alertParentDiv").append("<div class=\"RADUserManagementErrorDivRight\"></div>")
        $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivHeading\">Alert</div>")
        $(".RADUserManagementErrorDivRight").append("<div class=\"RADUserManagementErrorDivText\">Your Changes will revert.Want To Continue?</div>");
        $("#alertParentDiv").append("<div class=\"RADUserManagementPopUpErrorFooter\"></div>");
        $(".RADUserManagementPopUpErrorFooter").append("<div class=\"RADUserManagementPopUpErrorYes\">Yes</div><div class=\"RADUserManagementPopUpErrorNo\">No</div>")
        $("#alertParentDiv").css("top", "-200px");
        $("#alertParentDiv").animate({ "top": "0px" });
        $(".RADUserManagementPopUpErrorYes").unbind().click(function (event) {
            $("#alertParentDiv").remove();
            $(".umRightBody").empty();
            $(".UMRightHeaderEdit").empty()
            $(".RADUserManagementAccountSaveButton").remove()
            $(".UMRightHeaderEdit").append("<div class=\"RADUserManagementAccountDeleteButton\">Delete</div>");
            AllUserAccounts.instance.ShowHideByPrivilegeType("Delete");
            var account_name = $(".umDynamicTiles").find(".RADTileActive").children()[0].innerHTML
            var LabelAccountdesc = $(".umDynamicTiles").find(".RADTileActive").children()[1].innerHTML
            $(".umRightBody").append("<div class=\"RADUserManagementEditNewAccountName\"></div>");
            $(".RADUserManagementEditNewAccountName").append("<div class=\"RADUserManagementEditNewAccountNameMainDiv\"></div>");
            $(".RADUserManagementEditNewAccountNameMainDiv").append("<div class=\"RADUserManagementEditNewAccountNameLabel\">Account Name </div>")
            $(".RADUserManagementEditNewAccountName").append("<div class=\"RADUserManagementEditNewAccountNameMainDivBOX\"></div>");
            $(".RADUserManagementEditNewAccountNameMainDivBOX").append("<div class=\"RADUserManagementAccountNameLabel\">" + account_name + "</div>")
            $("<div class=\"AccountMaindiv\"</div>").insertAfter($('.RADUserManagementEditNewAccountName'))
            $(".AccountMaindiv").append("<div class=\"RADUserManagementEditNewAccountNameLabel2\">Account Description </div>")
            $(".AccountMaindiv").append("<div class=\"RADUserManagementEditNewAccountNameMainDivBOX2\"></div>");
            $(".RADUserManagementEditNewAccountNameMainDivBOX2").append("<div class=\"RADUserManagementAccountNameLabelDesc\">" + LabelAccountdesc + "</div>")
        });
        $(".RADUserManagementPopUpErrorNo").unbind().click(function (event) {
            $("#alertParentDiv").remove();
        });
    } else {

        $(".umRightBody").empty();
        $(".UMRightHeaderEdit").empty()
        $(".RADUserManagementAccountSaveButton").remove()
        $(".UMRightHeaderEdit").append("<div class=\"RADUserManagementAccountDeleteButton\">Delete</div>");
        AllUserAccounts.instance.ShowHideByPrivilegeType("Delete");
        var LabelAccountName = $(".RADTileActive").children()[0].innerHTML;
        var LabelAccountdesc = $(".RADTileActive").children()[1].innerHTML;
        $(".umRightBody").append("<div class=\"RADUserManagementEditNewAccountName\"></div>");
        $(".RADUserManagementEditNewAccountName").append("<div class=\"RADUserManagementEditNewAccountNameMainDiv\"></div>");
        $(".RADUserManagementEditNewAccountNameMainDiv").append("<div class=\"RADUserManagementEditNewAccountNameLabel\">Account Name </div>")
        $(".RADUserManagementEditNewAccountName").append("<div class=\"RADUserManagementEditNewAccountNameMainDivBOX\"></div>");
        $(".RADUserManagementEditNewAccountNameMainDivBOX").append("<div class=\"RADUserManagementAccountNameLabel\">" + LabelAccountName + "</div>")
        $("<div class=\"AccountMaindiv\"</div>").insertAfter($('.RADUserManagementEditNewAccountName'))
        $(".AccountMaindiv").append("<div class=\"RADUserManagementEditNewAccountNameLabel2\">Account Description </div>")
        $(".AccountMaindiv").append("<div class=\"RADUserManagementEditNewAccountNameMainDivBOX2\"></div>");
        $(".RADUserManagementEditNewAccountNameMainDivBOX2").append("<div class=\"RADUserManagementAccountNameLabelDesc\">" + LabelAccountdesc + "</div>")
    }
}
AllUserAccounts.prototype.DataInfoOntileClickSearch = function (event) {
    $(".umRightBody").empty();
    $(".UMRightHeaderEdit").empty()
    $(".RADUserManagementAccountSaveButton").remove()
    $(".UMRightHeaderEdit").append("<div class=\"RADUserManagementAccountDeleteButton\">Delete</div>");
    AllUserAccounts.instance.ShowHideByPrivilegeType("Delete");
    $(event.target).closest(".RADUserMgmttiles").append('<div id="arr"></div>');
    $(".umDynamicTiles").find(".RADarrow-right").remove();
    $("#arr").addClass('RADarrow-right fa fa-caret-right').insertAfter($(event.target).closest(".RADUserMgmttiles"));
    $('.umSec1Body').empty();
    $(".umDynamicTiles").find(".RADTileActive").removeClass("RADTileActive");
    $(event.target).closest(".RADUserMgmttiles").addClass('RADTileActive');
    var LabelAccountName = $(".RADTileActive").children()[0].innerHTML;
    var LabelAccountdesc = $(".RADTileActive").children()[1].innerHTML;
    AllUserAccounts.instance.SetAccount = $(event.target).closest(".RADUserMgmttiles").children()[0].innerText;
    $(".umRightBody").append("<div class=\"RADUserManagementEditNewAccountName\"></div>");
    $(".RADUserManagementEditNewAccountName").append("<div class=\"RADUserManagementEditNewAccountNameMainDiv\"></div>");
    $(".RADUserManagementEditNewAccountNameMainDiv").append("<div class=\"RADUserManagementEditNewAccountNameLabel\">Account Name </div>")
    $(".RADUserManagementEditNewAccountName").append("<div class=\"RADUserManagementEditNewAccountNameMainDivBOX\"></div>");
    $(".RADUserManagementEditNewAccountNameMainDivBOX").append("<div class=\"RADUserManagementAccountNameLabel\">" + LabelAccountName + "</div>")
    $("<div class=\"AccountMaindiv\"</div>").insertAfter($('.RADUserManagementEditNewAccountName'))
    $(".AccountMaindiv").append("<div class=\"RADUserManagementEditNewAccountNameLabel\">Account Description </div>")
    $(".AccountMaindiv").append("<div class=\"RADUserManagementEditNewAccountNameMainDivBOX2\"></div>");
    $(".RADUserManagementEditNewAccountNameMainDivBOX2").append("<div class=\"RADUserManagementAccountNameLabel\">" + LabelAccountdesc + "</div>")
}

AllUserAccounts.prototype.DataInfoOntileClick = function (firstaccount, firstAccountdesc) {
    $(".umRightBody").empty();
    $(".UMRightHeaderEdit").empty()
    $(".RADUserManagementAccountSaveButton").remove()
    $(".UMRightHeaderEdit").append("<div class=\"RADUserManagementAccountDeleteButton\">Delete</div>");
    AllUserAccounts.instance.ShowHideByPrivilegeType("Delete");
    $(".umRightBody").append("<div class=\"RADUserManagementEditNewAccountName\"></div>");
    $(".RADUserManagementEditNewAccountName").append("<div class=\"RADUserManagementEditNewAccountNameMainDiv\"></div>");
    $(".RADUserManagementEditNewAccountNameMainDiv").append("<div class=\"RADUserManagementEditNewAccountNameLabel\">Account Name </div>")
    $(".RADUserManagementEditNewAccountName").append("<div class=\"RADUserManagementEditNewAccountNameMainDivBOX\"></div>");
    $(".RADUserManagementEditNewAccountNameMainDivBOX").append("<div class=\"RADUserManagementAccountNameLabel\">" + firstaccount + "</div>")
    $("<div class=\"AccountMaindiv\"</div>").insertAfter($('.RADUserManagementEditNewAccountName'))
    $(".AccountMaindiv").append("<div class=\"RADUserManagementEditNewAccountNameLabel2\">Account Description </div>")
    $(".AccountMaindiv").append("<div class=\"RADUserManagementEditNewAccountNameMainDivBOX2\"></div>");
    $(".RADUserManagementEditNewAccountNameMainDivBOX2").append("<div class=\"RADUserManagementAccountNameLabelDesc\">" + firstAccountdesc + "</div>")
}
AllUserAccounts.prototype.GetAllAccountsCreate = function (AccountNameNewCreate) {
    var url = RADUserManagement.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetAllAccounts', // to get the entire HTML
        type: 'POST',
        contentType: "application/json",
        dataType: 'json'

    }).then(function (responseText) {
        var b;
        b = JSON.parse(responseText.d);
        var firstUser;
        var Editfirstname
        var Editlastname
        var EditEmailId
        $(".umDynamicTiles").find(".RADTileActive").remove()
        $(".umDynamicTiles").empty()
        for (var j = 0; j < b.length; j++) {


            if (b[j].AccountName == AccountNameNewCreate) {                
                $('.umDynamicTiles').prepend('<div id="t' + j + '" name="' + b[j]["AccountName"] + '"></div>');
                firstGroup = b[j]["AccountName"];
                firstAccountdesc = b[j]["Description"]
                $("#t" + j).addClass('RADUserMgmttiles').addClass('RADTileActive');
                $("#t" + j).append('<div id="account' + j + '" name="' + b[j]["AccountName"] + '">' + b[j]["AccountName"] + '</div>');
                $("#t" + j).closest(".RADUserMgmttiles").append('<div id="arr"></div>');
                $(".umDynamicTiles").find(".RADarrow-right").remove();
                $("#arr").addClass('fa fa-caret-right RADarrow-right').insertAfter("#t" + j);
                $("#account" + j).addClass('RADUserMgmtuser_name');
                $("#t" + j).append('<div id="account_desc' + j + '" Description="' + b[j]["Description"] + '">' + b[j]["Description"] + '</div>');
                $("#account_desc" + j).addClass('RADUserMgmtemail_id');
            }

            else {
                $('.umDynamicTiles').append('<div id="t' + j + '" name="' + b[j]["AccountName"] + '"></div>');
                $("#t" + j).addClass('RADUserMgmttiles')
                $("#t" + j).append('<div id="account' + j + '" name="' + b[j]["AccountName"] + '">' + b[j]["AccountName"] + '</div>');

                $("#account" + j).addClass('RADUserMgmtuser_name');
                $("#t" + j).append('<div id="account_desc' + j + '" Description="' + b[j]["Description"] + '">' + b[j]["Description"] + '</div>');
                $("#account_desc" + j).addClass('RADUserMgmtemail_id');
            }
        }
        var NewCreatedAccountName = $(".RADUserManagementAddNewAccountName").val();
        var NewCreatedAccountDescription = $(".RADUserManagementAddNewAccountDesc").val();
        AllUserAccounts.instance.DataInfoOntileClick(NewCreatedAccountName, NewCreatedAccountDescription)
        $(".RADUserMgmttiles").unbind().click(function (event) {
            AllUserAccounts.instance.selectGroup(event);
            AllUserAccounts.instance.DataOntileClick(event);
        });
    });
}
AllUserAccounts.prototype.SearchAction = function (event) {

    $("#search_text").css({ 'display': 'block' })
}

AllUserAccounts.prototype.ShowHideByPrivilegeType = function (PrivilegeType) {

    switch (PrivilegeType) {

        case "Add":
            if (Priviliges.indexOf("Add Account") != -1)
                $('.RADUserCreatePlusCircle').show();
            else
                $('.RADUserCreatePlusCircle').hide();
            break;

        case "Delete":
            if (Priviliges.indexOf("Delete Account") != -1)
                $(".RADUserManagementAccountDeleteButton").show();
            else
                $(".RADUserManagementAccountDeleteButton").hide();
            break;
        default:
            //do nothing
    }
}


    $.widget("ddn.SelectDropDown", {

        //DROP DOWN PROPERTIES Registration        
        options : {
              
                radEntitydefaultValue: '',
                radEntitydropDownAllData: [],                
                radEntityEnableMultiSelect: true,
                radEntityAttributeNameText: '',
                radWidgetSelectHandler : '',
        },


        //CREATING DROP DOWN  
        _create: function (object) {
            try{
                var self = this;
            
                self._destroy();
                //Merging the options property using jquery extend
                $.extend(this.options,object);
                //fetch the values of current element
                var dropDownObject = self._dropDownConstructor(self);

                //On successful object creation call the current element appender
                self._currentElemAppender(self, dropDownObject);

                //Binding the events
                self._BindDropDwnEvents(self,event);
            }
            catch (e){
                console.log(e.stack);
            }
        },

     
        //destroying the widget
        _destroy: function () {
            try{                
                this.element
                    .unbind("." + this.widgetName)
                    .removeData(this.widgetName);
            }
            catch(e){
                throw e;
            }
        },

        //To be initialised only once during creation for first time
        _dropDownConstructor: function (self) {
            try{
                var dropDownObject = {}
                dropDownObject["radEntitydefaultValue"] = self.options.radEntitydefaultValue;
                dropDownObject["radEntitydropDownAllData"] = self.options.radEntitydropDownAllData;
                dropDownObject["radEntityEnableMultiSelect"] = self.options.radEntityEnableMultiSelect;
                dropDownObject["radEntityAttributeNameText"] = self.options.radEntityAttributeNameText;
                dropDownObject["radWidgetSelectHandler"] = self.options.radWidgetSelectHandler;

                return dropDownObject;
            }
            catch(e){
                throw e;
            }
        },

        //All dropdown data HTML creation and binding to the parent element
        _BindCurrentElemAllDataDdn: function (self, dropDownObject) {
            try{

                if (dropDownObject != undefined  && dropDownObject.hasOwnProperty("radEntitydropDownAllData")) {

                    var selectedDimesionsArray = [];
                    if (dropDownObject.hasOwnProperty("radEntitydefaultValue") && dropDownObject["radEntitydefaultValue"] != null && dropDownObject["radEntitydefaultValue"] !='')
                    {
                        var dimensionalStr = dropDownObject["radEntityAttributeNameText"];
                        //Converts the selected data  string to split into an array
                        selectedDimesionsArray = self._GetSelectedDimensionData(dimensionalStr);
                    }
                               
                    var dimensionData = dropDownObject["radEntitydropDownAllData"];

                    //Binding the selected elements text for drop down
                    self._BindTheSelectedElemHtml(selectedDimesionsArray, dimensionData,self);

                    var $dimensionsMainDiv = $("<div>", {
                        class: "RADFunGroupDimensionsMainDiv_New"
                    });
                    var $currentRowDimensionParent = $("<div>", {
                        class: "RADDimensionDropDownParent"
                    });

                    var $searchParentDiv = $("<div>", {
                        class: "RADDataprivilegeSearchParent"
                    });

                    var $searchDiv = $("<div>", {
                        class: "RADDataprivilegeSearch_New"
                    });

                    if (self.options["radEntityEnableMultiSelect"] == true)
                    var $selectAllRowDiv = $("<div>", {
                        class: "RADDimensionalDropDownChildAll_New",
                        text: "Select All"
                    });
                   
                    var $scrollDiv = $("<div>", {
                        class: "RADDropDownScroll",
                        
                    });


                    if (selectedDimesionsArray.length != dimensionData.length && ($selectAllRowDiv!=undefined))
                        $selectAllRowDiv.attr("isselectedmode", "false");
                    else {
                        if ($selectAllRowDiv != undefined) {
                            $selectAllRowDiv.attr("isselectedmode", "true");
                            $selectAllRowDiv.addClass("RADDimensionDropDownChildUnSelectAll");
                        }
                    }

                    $searchDiv.attr("contenteditable", true);
                    $searchParentDiv.append($searchDiv);
                    $currentRowDimensionParent.append($searchParentDiv);
                    $currentRowDimensionParent.append($selectAllRowDiv);
                    $currentRowDimensionParent.append($scrollDiv);


                    for (var i = 0; i < dimensionData.length; i++) {
                        if (dimensionData[i] != null && dimensionData[i]!='') {
                            var $currentParent = $("<div>", {
                                class: "RADDimensionDropDownChildParent"
                            });
                            var $currentRowDimesion = $("<div>", {
                                class: "RADDimensionDropDownChild_New",
                                text: dimensionData[i].text,
                                id: dimensionData[i].id,
                            });

                            if (self.options["radEntityEnableMultiSelect"]) {
                                var $currentRowDimesionTick = $("<div>", {
                                    class: "RADCreateHierachyDisplayNone RADDimensionDropDownChildTick_New fa fa-check",
                                    isSelected: false
                                });
                                $currentParent.append($currentRowDimesion);
                                $currentParent.append($currentRowDimesionTick);

                                if (selectedDimesionsArray.indexOf(dimensionData[i]) != -1)
                                    $currentRowDimesionTick.removeClass("RADCreateHierachyDisplayNone");
                            }
                            else {
                                $currentRowDimesion.attr("isSelected",false);
                                $currentParent.append($currentRowDimesion);                             
                            }

                            if (selectedDimesionsArray.indexOf(dimensionData[i]) != -1)
                                $currentRowDimesionTick.removeClass("RADCreateHierachyDisplayNone");
                            
                            $scrollDiv.append($currentParent);
                        }
                    }
                    $dimensionsMainDiv.append($currentRowDimensionParent);
                    self.element.find('.RADExistingDataPrivilegeEntitySelect').append($dimensionsMainDiv);                   
                }
            }
            catch(e){
                throw e;
            }
        },

        
        _currentElemAppender: function (self,dropDownObject) {

            if (self!=undefined && dropDownObject != undefined) {                
                try{
                    var currentElement = self.element;
                    if(currentElement!=undefined)
                    {

                        $(currentElement).append("<div class=\"RADFuncGroupCreateEntitySelect\"</div>");
                        $(currentElement).find(".RADFuncGroupCreateEntitySelect").append("<div class=\"RADExistingDataPrivilegeEntitySelect RADDropDwnTextBox\"><div class=\"RADplaceHolderText\"></div></div>");
                        //Attribute addition for radEntityAttributeNameText
                        var attributeNode = $(currentElement).find(".RADExistingDataPrivilegeEntitySelect");                                      
                        attributeNode.attr('radEntityAttributeNameText', dropDownObject['radEntityAttributeNameText']);

                        //Attribute addition for radEntitySelectedValue                       
                        attributeNode = $(currentElement).find(".RADExistingDataPrivilegeEntitySelect");                        
                        attributeNode.attr('radEntitySelectedValue', dropDownObject['radEntitydefaultValue']);

                        //self.element = currentElement;
                        self._BindCurrentElemAllDataDdn(self, dropDownObject);
                    }

                }
                catch(e){
                    throw (e);
                }
            }
        },

        //set the selected dimension data
        _GetSelectedDimensionData: function (dimensionalData) {
            try{
                var dimesionalDataArr = [];
                if (dimensionalData != undefined && dimensionalData.length > 0 && dimensionalData.indexOf('|^')>-1)
                {
                    dimesionalDataArr = dimensionalData.split("|^");
                }
                return dimesionalDataArr;
            }
            catch (e) {
                throw e;
            }
        },

        //for displaying the selected enteries of dropdown
        _BindTheSelectedElemHtml: function (selectedDimesionsArray, dimensionData,self) {
            try{
                if (selectedDimesionsArray.length > 0) {
                    var prevHtml = '';
                    var actualSelectedArr = 0;
                    for (var i = 0; i < selectedDimesionsArray.length && i < 2; i++) {
                        if (selectedDimesionsArray[i] != '') {
                            prevHtml += selectedDimesionsArray[i] + ',';
                            actualSelectedArr++;
                        }
                    }
                    if (prevHtml.length > 0) {
                        prevHtml = prevHtml.replace(/(^[,\s]+)|([,\s]+$)/g, '');
                    }
                    if (actualSelectedArr- 2 > 0) {
                        prevHtml += '+' + actualSelectedArr - 2;
                    }
                    //prevHtml += this.element.find('.RADExistingDataPrivilegeEntitySelect').html();
                    //this.element.find('.RADExistingDataPrivilegeEntitySelect').html(prevHtml);
                    self.element.find('.RADplaceHolderText')[0].innerText = prevHtml;
                    //self.element.find(".RADExistingDataPrivilegeEntitySelect").prepend("<i class=\"fa fa-caret-down\" aria-hidden=\"true\" style=\"display:inline-block;float:right;margin-top:5px;\"></i>");
                    self.element.find(".RADExistingDataPrivilegeEntitySelect").append("<div><i class=\"fa fa-caret-down\" aria-hidden=\"true\" style=\"display:inline-block;margin-top:5px;\"></i></div>");
                    //$(".RADExistingDataPrivilegeEntitySelect").prepend("<i class=\"fa fa-caret-down\" aria-hidden=\"true\" style=\"display:inline-block;\"></i>");
                }
            }
            catch(e)
            {
                throw e;
            }
        },

        //Events for DropDown :: Method Bind Those events
        _BindDropDwnEvents: function (self,event) {
        
            //Event For enabling the drop down elements selection :: It will add the tick on each selected element
           // self = self._GetWidgetInfo();

            $(self.element).find(".RADDimensionDropDownChild_New").unbind().click(function (event) {
                $(event.target).parent().find('.RADDimensionDropDownChildTick_New').css({ 'display': 'inline-block' })
                //set isSelected attribute to true
                if ($(event.target).parent().find('.RADDimensionDropDownChildTick_New').attr('isSelected') == "true") {
                    $(event.target).parent().find('.RADDimensionDropDownChildTick_New').attr('isSelected', false);
                    $(event.target).parent().find('.RADDimensionDropDownChildTick_New').css({ 'display': 'none' });                   
                }
                else {
                    if ($(event.target).parent().find('.RADDimensionDropDownChild_New').attr('isSelected') == "false")
                    {
                        $(event.target).parent().find('.RADDimensionDropDownChild_New').attr('isSelected', true);
                    }
                    $(event.target).parent().find('.RADDimensionDropDownChildTick_New').attr('isSelected', true);                    
                }
                //Bind the above search box html
                self._BindSearchBoxHtml(event,self);
                event.stopPropagation();

                //select handler
                if (self.options["radWidgetSelectHandler"] != undefined && self.options["radWidgetSelectHandler"] != '') {
                    var text = [];
                    text = self._GetSelectedDataFromWidget(event);
                    //var text = $(event.target)[0].innerText;
                    var _FnCallback  = self.options["radWidgetSelectHandler"];
                    _FnCallback(text,event);
                }
                
            });

            //Event handler for select all
            $(self.element).find(".RADDimensionalDropDownChildAll_New").unbind().click(function (e) {
                if (e.target.innerText == "Select All" || $("[isselected=" + "true" + "]").length > 0) {
                    if ($(e.target).parent().find('.RADDimensionDropDownChildTick_New').attr('isselected') != "true") {
                        $(e.target).parent().find('.RADDimensionDropDownChildTick_New').attr('isselected', true);
                        $(e.target).parent().find('.RADDimensionDropDownChildTick_New').css({ 'display': 'inline-block' });
                        // $(e.target).parent().parent().parent().find(".RADplaceHolderText")[0].innerText = 'All Selected';
                        $(e.target).closest('.RADExistingDataPrivilegeEntitySelect').find('.RADplaceHolderText')[0].innerText = "All Selected";
                    }
                  
                    else
                    {
                        $(e.target).parent().find('.RADDimensionDropDownChildTick_New').attr('isselected', false);
                        $(e.target).parent().find('.RADDimensionDropDownChildTick_New').css({ 'display': 'none' });
                        // $(e.target).parent().parent().parent().find(".RADplaceHolderText")[0].innerText = 'None Selected';
                        $(e.target).closest('.RADExistingDataPrivilegeEntitySelect').find('.RADplaceHolderText')[0].innerText = "None Selected";
                    }
                }
                else if ($(e.target).parent().find('.RADDimensionDropDownChildTick_New').attr('isselected') == "false") {
                    $(e.target).parent().find('.RADDimensionDropDownChildTick_New').attr('isselected', true);
                    $(e.target).parent().find('.RADDimensionDropDownChildTick_New').css({ 'display': 'inline-block' });
                    //$(e.target).parent().parent().parent().find(".RADplaceHolderText")[0].innerText[0].innerText = 'All Selected';
                    $(e.target).closest('.RADExistingDataPrivilegeEntitySelect').find('.RADplaceHolderText').innerText[0] = "All Selected";
            }
                else
                {
                    $(e.target).parent().find('.RADDimensionDropDownChildTick_New').attr('isselected', false);
                    $(e.target).parent().find('.RADDimensionDropDownChildTick_New').css({ 'display': 'none' });
                    //$(e.target).parent().parent().parent().find(".RADplaceHolderText")[0].innerText = 'None Selected';
                    $(e.target).closest('.RADExistingDataPrivilegeEntitySelect').find('.RADplaceHolderText').html().trim() = "None Selected";
                    self._BindSearchBoxHtml(e,self);
                }                              
                e.stopPropagation(e);
            });


            $(self.element).find(".RADExistingDataPrivilegeEntitySelect").unbind().click(function (e) {
                if ($(e.target).attr('class') != 'RADDataprivilegeSearch_New') {
                    if ($(".RADFunGroupDimensionsMainDiv_New").css('display') == "none") {
                        $(".RADFunGroupDimensionsMainDiv_New").css({ 'display': 'block' });
                    }
                    else {
                        $(".RADFunGroupDimensionsMainDiv_New").css({ 'display': 'none' });
                    }
                }
               
            });

            //Search on key up for drop down
            
            $(self.element).find(".RADDataprivilegeSearch_New").unbind("click").click(function (e) {
                e.preventDefault();
                e.stopPropagation();
            });
            $(self.element).find(".RADDataprivilegeSearch_New").unbind("keyup").keyup(function (e) {
                self._BindKeyUpHandler(self, e);
                e.preventDefault();
                e.stopPropagation();
            });

            $("#contentBody").unbind("click").click(function (e) {
                if ($(e.target).attr('class') != 'RADFunGroupDimensionsMainDiv_New')
                {
                    $(".RADFunGroupDimensionsMainDiv_New").css({'display' : 'none'});
                }
                e.stopPropagation();
            });

            $(self.element).find(".RADExistingDataPrivilegeEntitySelect ").unbind("click").click(function (e) {
                $(e.target).parent().find(".RADFunGroupDimensionsMainDiv_New").css({ 'display': 'block' });
                e.stopPropagation();
            });

            $(self.element).find(".RADDropDwnTextBox").unbind("click").click(function (e) {
                $(".RADFunGroupDimensionsMainDiv_New").css({ 'display': 'none' });
                $(e.target).parent().find(".RADFunGroupDimensionsMainDiv_New").css({ 'display': 'block' });
                e.preventDefault();
                e.stopPropagation();
            });

        }
        ,

        _BindSearchBoxHtml: function (event,self) {
            var array = $(event.target.parentNode.parentElement).find('div[isselected="true"]').prev();
            var html = '';
            for (var iter = 0; iter < array.length && iter<1; iter++) {
                html += array[iter].innerText;
            }
            
            if(html!=''){
                html += array.length>1?  " + " +  (array.length-1) + "More" : '';
                // $('.RADplaceHolderText')[0].innerText = html;
                // $(event.target).parent().parent().parent().parent().parent().find(".RADplaceHolderText")[0].innerText = html;
                $(event.target).closest('.RADExistingDataPrivilegeEntitySelect').find('.RADplaceHolderText')[0].innerText = html;
            }
            else if (self.options["radEntityEnableMultiSelect"] == false)
            {
                //$(event.target).parent().parent().parent().parent().parent().find(".RADplaceHolderText")[0].innerText = $(event.target)[0].innerText;
                $(event.target).closest('.RADExistingDataPrivilegeEntitySelect').find(".RADplaceHolderText")[0].innerText = $(event.target)[0].innerText;
            }

            //self._GetCurrentDdnData(event);
        },

        //Handler for searching the substring
        _BindKeyUpHandler : function(self,e){
              
            var keyWord = $(e.target).html().trim().toLowerCase();
            var dropDdnArr = $(e.target).closest('.RADExistingDataPrivilegeEntitySelect').find(".RADDimensionDropDownChild_New");
            var event = e;
            
            //   $(".RADDimensionDropDownChildParent").css({ 'display': 'none' });
            // $(e.target).css({ 'display': 'none' });
            $(e.target).parent().parent().find(".RADDimensionDropDownChildParent").css({ 'display': 'none' });

                for(var i = 0;i<dropDdnArr.length;i++)
                {
                    if (dropDdnArr[i].innerText.toLowerCase().indexOf(keyWord) > -1)
                    {                                                                      
                        $(dropDdnArr[i]).parent().css({ 'display': 'block' });
                    }                    
                }                      
        },

        _GetSelectedDataFromWidget: function (event) {
            var response = [];

            if ($(event.target).attr('isselected') == undefined) {
                var arr = $('div[isselected="true"]');
                for (var i = 0; i < arr.length; i++) {
                    if($(arr[i]).hasClass('RADDimensionDropDownChildTick_New')){
                        response.push($(arr[i]).parent().find(".RADDimensionDropDownChild_New")[0].innerText);
                    }
                }
            }

            //case triggered for single selction of dropdown
            else if ($(event.target).attr('isselected')) {
                response.push($(event.target)[0].innerText);
            }
            else {
                var arraySel = $($(event.target.offsetParent).find('div[isselected="true"]'));               
                for (var i = 0; i < arraySel.length; i++) {
                    response.push($(arraySel[i]).parent().find(".RADDimensionDropDownChild_New")[0].innerText);
                }
            }
            return response;
        },

        _GetSingleSelectedDataFromWidget: function (element) {
            return $(element).find(".RADplaceHolderText").html().trim();
        },

       
    });


