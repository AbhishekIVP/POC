var srmAccess = (function () {
    var srmAccess;
    this._controlIdInfo = null;
    String.prototype.replaceAll = function (find, replace) {
        return this.replace(new RegExp(find, 'g'), replace);
    }


    var html = "";
    var params = {};
    //params.secTypeId = 2;
    var t;
    var conInfo = [];
    var conInfo1 = [];
    var conInfo2 = [];
    var conInfo3 = [];
    var Users = [];
    //var Users = ["Abhishek Gupta", "Atul Ahuja", "Abhishek Gupta", "Atul Ahuja"];
    //var NameGroup = ["System Level", "Fund3 Template", "Risk Template"];
    //var UserGroup = ["System", "Fund User", "Risk User"];
    var NameGroup = [];
    var UserGroup = [];
    var GroupsId = [];
    var Groups = [];
    // var Groups = ["Ops Group", "Complaince", "Research", "Testing"];
    var conditions = [];// ["Has Position Equals True", "Has Position Equals True"];
    var Templates = ["NameUserGroup"];
    var conditionInfo = [{ values: ["Abhishek Gupta", "Atul Ahuja", "Abhishek Gupta", "Atul Ahuja"] }, { values: ["Abhishek Gupta", "Atul Ahuja", "Abhishek Gupta", "Atul Ahuja"] }];
    var conditionInfo1 = [];//[{ values: ["Ops Group", "Complaince", "Research", "Testing"] }, { values: ["Ops Group", "Complaince", "Research", "Testing"] }];
    var templateInfo = [{ values: ["value1"] }];
    var headings = ["USERS", "GROUPS"];
    var text;
    var temp;
    var flag = 0;


    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    var iframePath = path;
    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            iframePath = iframePath + '/' + ee;
    });






    function SRMAccess() {
        this._username;
    }


    srmAccess = new SRMAccess();



    SRMAccess.prototype.Init = function Init(userName, moduleId, typeId) {
        srmAccess._username = userName;
        params.typeId = typeId;
        params.moduleId = moduleId;
        if (moduleId != 3) {

            $("#SRMAccessTypeId").text("ENTITY TYPE LEVEL");

        }
        //buildScreen();
        dataInsertion();
    }
    function dataInsertion() {
        var resultGrid = CallCommonServiceMethod('GetAllowedUsers', params, onSuccess_GetBulkUploadStatusData2, OnFailure, null, false);
    }

    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        callService('POST', iframePath + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }

    function onSuccess_GetBulkUploadStatusData2(result) {


        for (var i = 0; i < result.d.length; i++) {
            Users.push(result.d[i]);
            // Users.push("aman");


        }





        var resultGrid2 = CallCommonServiceMethod('GetAllowedGroups', params, onSuccess_GetBulkUploadStatusData1, OnFailure, null, false);
    }

    function onSuccess_GetBulkUploadStatusData1(result) {



        for (var i = 0; i < result.d.length; i++) {
            Groups.push(result.d[i]);


        }




        var resultGrid2 = CallCommonServiceMethod('GetTemplateInfo', params, onSuccess_GetBulkUploadStatusData3, OnFailure, null, false);

    }

    function onSuccess_GetBulkUploadStatusData5(result) {



        for (var i = 0; i < result.d.length; i++) {

            conditions.push(result.d[i].Rule);
            var t = { "values": result.d[i].Groups };
            conditionInfo1.push(t);


        }





        buildScreen();
    }




    function onSuccess_GetBulkUploadStatusData3(result) {


        for (var i = 0; i < (result.d).length; i++) {
            GroupsId.push(result.d[i].templateId);
            NameGroup.push(result.d[i].templateName);
            UserGroup.push(result.d[i].userGroup);

        }
        var resultGrid2 = CallCommonServiceMethod('GetRuleAccessDetails', params, onSuccess_GetBulkUploadStatusData5, OnFailure, null, false);
    }

    function onClick(e) {
        var t;

    }

    function buildScreen() {


        var htmlUser = "";
        var htmlGroup = "";
        var texttypeId = "SECURITIES";
        if (params.moduleId != 3)
            texttypeId = "ENTITIES";
        var htmlCondition = "<div style=\"margin-left:10px;font-weight:bold;margin-top:20px;\">" + texttypeId + "</div>";
        var htmlTemplate = "<div style=\"font-weight:bold;\">ATTRIBUTE LEVEL</div>";
        var t = 0;
        var extraUsers;

        //offset
        //$("")

        var itemsCount = 0;
        itemsCount = parseInt(($(window).width() - ($("#UsersText").offset().left + $("#UsersText").width() + 40)) / 160);
        var itemsCount2 = 0;
        itemsCount2 = parseInt(($(window).width() - ($("#GroupsText").offset().left + $("#GroupsText").width() + 40)) / 160);

        
        var index1 = 0;
        var index2 = 0;
        for (var i = 0; i < Users.length; i++) {
            t++;

            if (t == itemsCount) {
                extraUsers = Users.length - t ;
                index1 = itemsCount ;
                htmlUser = htmlUser + "<div class=\"htmlUser SRMDivClass\" id=\"usersMoreDiv\" count=\"" + i +  "\"><div>+" + extraUsers + " More </div>"
                break;
            } else {
                htmlUser = htmlUser + "<div class=\"htmlUser SRMDivClass\"  >" + Users[i] + "</div>"
            }
        }
        
        
        

        var t1 = 0;
        var extraGroup;
        for (var i = 0; i < Groups.length; i++) {
            t1++;
            if (t1 == itemsCount2) {
                extraGroup = Groups.length - t1 ;
                index2 = itemsCount ;
                htmlGroup = htmlGroup + "<div class=\"htmlUser SRMDivClass\" id=\"groupsMoreDiv\" count=\"" + i + "\">+" + extraGroup + " More </div>"
                break;
            } else {
                htmlGroup = htmlGroup + "<div class=\"htmlUser SRMDivClass\" >" + Groups[i] + "</div>"
            }
        }
        var t = 14;

        for (var i = 0; i < conditions.length; i++) {
            //htmlCondition = htmlCondition + "<div style=\"border-top:3px solid #6EA6B3;margin-left:10px;border-bottom:2px solid #EEF1F3;border-left:2px solid #EEF1F3;border-right:2px solid #EEF1F3;width:98%;top:" + t + "px;position:relative;background-color:white;font-size:14px; height:155px;text-align:center;\"><div style=\"width:90%;height:25px;display:block;border-bottom:1px solid #D3D3D3;padding-top:10px;margin-left:40px;text-align:left;font-weight:bold;\">" + conditions[i] + "</div><div style=\"width:10%;display:inline-block;text-align:center;font-weight:bold;color:#91BAD6;\">USERS:</div><div style=\"text-align:left;padding-left:0px;display:inline-block;height:56px;top:5px;position:relative;\">";
            htmlCondition = htmlCondition + "<div style=\"border-top:3px solid #6EA6B3;margin-left:10px;border-bottom:2px solid #EEF1F3;border-left:2px solid #EEF1F3;border-right:2px solid #EEF1F3;width:98%;top:" + t + "px;position:relative;background-color:white;font-size:14px; height:155px;text-align:left;\"><div style=\"width:90%;height:25px;display:block;border-bottom:1px solid #D3D3D3;padding-top:6px;margin-left:21px;text-align:left;font-weight:bold;\">" + conditions[i] + "</div>";
            //for (var j = 0; j < conditionInfo[i].values.length; j++) {

            //    htmlCondition = htmlCondition + "<div class=\"htmlConditionValue\" style=\"width:130px;color:#91BAD6;border-radius: 20px;margin-top:14px;border:2px solid #91BAD6;background-color: white;\">" + conditionInfo[i].values[j] + "</div>"

            //}
            //htmlCondition = htmlCondition + "</div><div style=\"width:10%;font-size:14px;display:inline-block;text-align:center;font-weight:bold;color:#91BAD6;\">GROUPS:</div><div style=\"text-align:left;padding-left:0px;display:inline-block;height:59px;top:0px;position:relative;border-top:2px dashed #D3D3D3;margin-top:10px;\">";
            htmlCondition = htmlCondition + "<div style=\"width:10%;font-size:14px;display:inline-block;text-align:left;font-weight:bold;color:#91BAD6;margin-left:20px;\">GROUPS:</div><div style=\"text-align:left;padding-left:0px;display:inline-block;height:59px;top:0px;position:relative;margin-top:10px;\">";
            for (var j = 0; j < conditionInfo1[i].values.length; j++) {
                //if (j == 0) {
                //    htmlCondition = htmlCondition + "<div class=\"htmlConditionValue\" style=\"margin-top:13px;height:34px;padding-top:3px;width:130px;border-radius: 20px;border:2px solid #D3D3D3;background-color: #EEF1F3;\">" + conditionInfo1[i].values[j] + "</div>"
                //} else {
                htmlCondition = htmlCondition + "<div class=\"htmlConditionValue\" style=\"width:130px;border-radius: 20px;border:2px solid #D3D3D3;background-color: #EEF1F3;\">" + conditionInfo1[i].values[j] + "</div>"
                //}
            }
            htmlCondition = htmlCondition + "</div></div>";
            t = t + 17;
        }
        var tt = 20;
        for (var i = 0; i < Templates.length; i++) {
            htmlTemplate = htmlTemplate + "<div style=\"width:100%;top:" + tt + "px;position:relative; height:100px;text-align:center;\"><div style=\"background-color:#EEF1F3;height:35px;font-size:14px;margin-right:7px;\"><div style=\"width:45%;height:30px;line-height: 33px;display:inline-block;padding-top:0px;text-align:left;font-weight:bold;\">Name</div><div style=\"width:45%;padding-left:5%;height:30px;line-height: 33px;display:inline-block;text-align:left;font-weight:bold;\">UserGroup</div></div>";

            for (var j = 0; j < UserGroup.length; j++) {

                htmlTemplate = htmlTemplate + "<div style=\"width:100%;font-size:14px;text-align:left;height:45px;    margin-top: 10px; border-bottom:1px solid  #EEF1F3;\"><div style=\"width:44%;padding-left:15px;display:inline-block;font-weight:;\">" + NameGroup[j] + "</div><div class=\"htmlUser SRMDivClass\">" + UserGroup[j] + "</div><div style=\"padding-left: 13%;display:inline-block;\"><div class=\"button1 templateViewClick\" style=\"background-color:#4EAAE4; border:#4EAAE4;color:white;border-radius:4px;width:50px;padding: 2px;text-align: center;\" attr=\"" + GroupsId[j] + "\"   attr2=\"" + NameGroup[j] + "\">View</div></div></div>"
            }
            htmlTemplate = htmlTemplate + "<div style=\"width:100%;border-bottom:2px solid #EEF1F3;;height:45px;\"></div>";
            htmlTemplate = htmlTemplate + "</div></div>";
            tt = tt + 50;
        }



        document.getElementById("Users").innerHTML = htmlUser;
        document.getElementById("Group").innerHTML = htmlGroup;
        document.getElementById("condition").innerHTML = htmlCondition;
        document.getElementById("template").innerHTML = htmlTemplate;
        
        var ele;
        if (index1 > 0) {
            ele = $('<div>');
            //ele.css('top', 10 + $("#usersMoreDiv").offset().top + $("#usersMoreDiv").height());
            //ele.css('left', 10 + (($("#usersMoreDiv").offset().left + $("#usersMoreDiv").width()) / 2));
            //ele.css('background', 'white');
            //ele.css('width', $("#usersMoreDiv").width());
            for (i = index1; i < Users.length; i++) {
                ele.append($('<div>').text(Users[i]).addClass('SRMAccessElement'));
            }
        }
        $("#SRMAccessPopup1").append(ele);
        ele = null;
        
        if (index2 > 0) {
            ele = $('<div>');
            //ele.css('top', 10 + $("#usersMoreDiv").offset().top + $("#usersMoreDiv").height());
            //ele.css('left', 10 + (($("#usersMoreDiv").offset().left + $("#usersMoreDiv").width()) / 2));
            //ele.css('background', 'white');
            //ele.css('width', $("#usersMoreDiv").width());
            for (i = index2; i < Groups.length; i++) {
                ele.append($('<div>').text(Groups[i]).addClass('SRMAccessElement'));
            }
        }
        $("#SRMAccessPopup2").append(ele);


        
        

        //apply click bindings
        $(".templateViewClick").off('click').on('click', function (event) {
            var moduleId = parseInt(params.moduleId);
            var templateId = parseInt($(event.target).attr('attr'));
            $("#SRMAccessGridValue").text($(event.target).attr('attr2')).css('text-transform','uppercase');

            $("#SRMAccessIframeContainer").show();
            $("#SRMAccessIframeContainer").css('width', $(window).height() - 2);
            $("#SRMAccessIframe").css('height', $(window).height() - 2 - 1 - 50);
            $("#SRMAccessIframeContainer").animate({
                left: '400px', width: $(window).width() - 402
            }, '500',
            function () {
                //$("#SRMDataSourceAndSystemMappingGrid").css('background', 'white');
                //$("svg").hide();

                //close button binding
                $("#SRMAccessIframe").css('width', $(window).width() - 402 - 1 - 10);

                $("#SRMAccessIframe").attr('src', iframePath + "/SRMGrid.aspx?moduleId=" + moduleId + "&templateId=" + templateId);
                $("#SRMAccessHeaderClose").off('click').on('click', function () {
                    $("#SRMAccessIframe").css('right', "");
                    $("#SRMAccessIframe").attr('src', "");
                    $("#SRMAccessIframe").animate({
                        width: 0, left: '400px'
                    }, '500', function () {
                        $("#SRMAccessIframe").css('right', 0);
                        $("#SRMAccessIframe").css('left', "");
                        $("#SRMAccessIframeContainer").css('display', 'none');
                    });

                })
            });


        });


        $("#usersMoreDiv").off('click').on('click', function (event) {
            var ele = $("#SRMAccessPopup1");
            if (ele.css('display') == 'block') {
                ele.css('display', 'none');
            }
            else {
                ele.css('display', 'block');
                ele.css('position', 'absolute');
                ele.css('top', 10 + $("#usersMoreDiv").offset().top + $("#usersMoreDiv").height());
                ele.css('left', 10 + $("#usersMoreDiv").offset().left);
                ele.css('background', 'white');
                ele.css('width', $("#usersMoreDiv").width());
            }
        });

        $("#groupsMoreDiv").off('click').on('click', function (event) {
            var ele = $("#SRMAccessPopup2");
            if (ele.css('display') == 'block') {
                ele.css('display', 'none');
            }
            else {
                ele.css('display', 'block');
                ele.css('position', 'absolute');
                ele.css('top', 10 + $("#groupsMoreDiv").offset().top + $("#groupsMoreDiv").height());
                ele.css('left', 10 + $("#groupsMoreDiv").offset().left);
                ele.css('background', 'white');
                ele.css('width', $("#groupsMoreDiv").width());
            }
        });

    }


    return srmAccess;


})();