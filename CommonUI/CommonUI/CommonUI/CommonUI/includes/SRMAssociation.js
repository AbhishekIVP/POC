var srmAssociation = (function () {
    var srmAssociation;
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
    var text;
    var temp;
    var flag = 0;
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');


    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    function SRMAssociation() {
        this._username;
    }
    srmAssociation = new SRMAssociation();


    SRMAssociation.prototype.Init = function Init(userName, typeId) {

        srmAssociation._username = userName;
        params.secTypeId = parseInt(typeId);
        start();

    }
    function start() {
        var resultGrid = CallCommonServiceMethod('ConstituentRight', params, onSuccess_GetBulkUploadStatusData2, OnFailure, null, false);
    }
    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }

    function onSuccess_GetBulkUploadStatusData(result) {
        var t = 0;

        for (var i = 0; i < JSON.parse(result.d).length; i++) {
          conInfo.push(JSON.parse(result.d)[i].sectype_name);

            t++;
        

        }
   
        var resultGrid2 = CallCommonServiceMethod('Derivatives', params, onSuccess_GetBulkUploadStatusData1, OnFailure, null, false);
    }

    function onSuccess_GetBulkUploadStatusData4(result) {
        var t = 0;
        text = JSON.parse(result.d)[0].sectype_name;

  
        flag = flag + 1;

        processIt();
    }

    function onSuccess_GetBulkUploadStatusData3(result) {
        var t = 0;
        for (var i = 0; i < JSON.parse(result.d).length; i++) {
          conInfo3.push(JSON.parse(result.d)[i].sectype_name);
            t++;  
        }
      
        var resultGrid2 = CallCommonServiceMethod('SecTypeNameText', params, onSuccess_GetBulkUploadStatusData4, OnFailure, null, false);

    }
    function onSuccess_GetBulkUploadStatusData1(result) {
        var t = 0;
        for (var i = 0; i < JSON.parse(result.d).length; i++) {
            conInfo1.push(JSON.parse(result.d)[i].sectype_name);
            t++;          

        }

        console.log(JSON.parse(result.d));

        flag = flag + 1;

        var resultGrid2 = CallCommonServiceMethod('ConstituentRightMost', params, onSuccess_GetBulkUploadStatusData3, OnFailure, null, false);


    }
    function onSuccess_GetBulkUploadStatusData2(result) {
        var t = 0;
        for (var i = 0; i < JSON.parse(result.d).length; i++) {
             conInfo2.push(JSON.parse(result.d)[i].sectype_name);

            t++;

         
        }

        
        var resultGrid1 = CallCommonServiceMethod('DerivativesLeft', params, onSuccess_GetBulkUploadStatusData, OnFailure, null, false);
    }

    function OnFailure(err) {

        console.log(err);

    }

    function processIt() {



        html = html + "<div id=\"dataFirstDiv\" class=\"Constituent  font\" style=\"border-top:5px solid #7FB0D9; position:relative;width:226px; margin-left:35px;overflow:hidden;background-color:#FFFFFF;\"><div  class=\"font2\" >CONSTITUENT OF</div><div id=\"scroll1\" class=\"scroll\" style=\" width:222px;\">";
        if (conInfo.length == 0) {

            html = html + "<div style=\"\"><div id=\"testt\" style=\"padding-top:10px;position:absolute;width:210px;\">NO DERIVATIVES CONFIGURED</div></div>";
        } else {
            for (var i = 0; i < conInfo.length; i++) {

                html = html + "<div class=\"  font1\" style=\"height:36px;padding-left: 28px;\">" + conInfo[i] + "</div>"
            }
        }
        html = html + "</div></div>";
        html = html + "<div id=\"dataSecondDiv\" class=\"Derivatives font\" style=\"position:relative; width:226px; background-color:#FFFFFF;border-top:5px solid #A4ACCF;margin-left:35px;\"><div class=\"font2\"  >DERIVATIVES</div><div id=\"scroll2\" class=\"scroll\" style=\" width:222px;\">";
        if (conInfo1.length == 0) {


            html = html + "<div><div id=\"test2\" style=\"padding-top:10px;position:absolute;width:210px;\">NO DERIVATIVES CONFIGURED</div></div>";
        } else {
            for (var i = 0; i < conInfo1.length; i++) {

                html = html + "<div class=\"font1\" style=\"height:36px;padding-left: 28px;\">" + conInfo1[i] + "</div>"
            }
        }
        html = html + "</div></div><div id=\"dataFifthDiv\" style=\"position:realtive; \">";
        html = html + "<div style=\" margin-top:27%; margin-left:25px;display:inline-block;\"><img src=\"images/arrowleft1.jpg\"></div>";

        html = html + "<div style=\"position:realtive;margin-top:0%;display:inline-block;margin-left:20px;font-family:oswald;border-radius:4px;background-color:#405B79;color:white;font-size:17px;width1:160px;height:65px;text-align:center;padding-top:18px;padding-left:7px;padding-right:7px;min-width:100px;\">" +text + "</div>";
        html = html + "<div style=\"margin-top:0%;position:realtive; margin-left:20px;display:inline-block;\"><img src=\"images/arrow3.jpg\"></div></div>";

        html = html + "<div id=\"dataThirdDiv\" class=\"Derivatives  font\" style=\"border-top:5px solid #99BEBF;position:relative; margin-left:20px;width:226px; background-color:#FFFFFF;\"><div class=\"font2\"  >UNDERLYERS</div><div id=\"scroll3\" class=\"scroll\" style=\" width:222px; text-align:center;\">";
        if (conInfo2.length == 0) {

            html = html + "<div style=\"text-align:center;\"><div id=\"test3\" style=\"padding-top:10px;position:absolute;width:210px;\">NO UNDERLYERS  CONFIGURED</div></div>";
        } else {
            for (var i = 0; i < conInfo2.length; i++) {

                html = html + "<div class=\"font1\" style=\"height:36px;padding-left: 28px;\"\">" + conInfo2[i] + "</div>"
            }
        }
        html = html + "</div></div>";
        html = html + "<div id=\"dataFourthDiv\" class=\"Derivatives font\" style=\"border-top:5px solid #A9DBDF;position:relative; margin-right:22px;margin-left:20px;width:226px; background-color:#FFFFFF;\"><div class=\"font2\"  >CONSTITUENTS</div><div id=\"scroll4\" class=\"scroll\" style=\" width:222px;\">";
        if (conInfo3.length == 0) {

            html = html + "<div style=\"text-align:center;\"><div id=\"test4\" style=\"padding-top:10px;position:absolute;width:210px;\">NO CONSTITUENTS CONFIGURED</div></div>";
        } else {
            for (var i = 0; i < conInfo3.length; i++) {

                html = html + "<div class=\"font1\" style=\"height:36px;padding-left: 28px;\">" + conInfo3[i] + "</div>"
            }
        }
        html = html + "</div></div>";

        document.getElementById("test").innerHTML = html;
        var max = 0;
        var current = 0;
        current = $(document.getElementById("scroll1")).height();
        if (current > max) {
            max = current;
        }
        current = $(document.getElementById("scroll2")).height();
        if (current > max) {
            max = current;
        }
        current = $(document.getElementById("scroll3")).height();
        if (current > max) {
            max = current;
        }
        current = $(document.getElementById("scroll4")).height();
        if (current > max) {
            max = current;
        }

        document.getElementById("scroll1").style.height = max + "px";
        document.getElementById("scroll2").style.height = max + "px";
        document.getElementById("scroll3").style.height = max + "px";
        document.getElementById("scroll4").style.height = max + "px";
        var topp = $(document.getElementById("scroll4")).offset().top;
        var updateTop = (max / 2);
        if (conInfo.length == 0) {
            document.getElementById("testt").style.top = updateTop + "px";
        }
        if (conInfo1.length == 0) {
            document.getElementById("test2").style.top = updateTop + "px";
        }
        if (conInfo2.length == 0) {
            document.getElementById("test3").style.top = updateTop + "px";
        }
        if (conInfo3.length == 0) {
            document.getElementById("test4").style.top = updateTop + "px";
        }

        var y = $(document.getElementById("test1")).height();
        var r = $(document.getElementById("dataFirstDiv")).height();
        var e = (y - r) / 2;
        var u = (r - 200) / 2;
        document.getElementById("dataFirstDiv").style.marginTop = e + "px";
         document.getElementById("dataSecondDiv").style.marginTop= e + "px";
          document.getElementById("dataThirdDiv").style.marginTop= e + "px";
          document.getElementById("dataFourthDiv").style.marginTop = e + "px";
         document.getElementById("dataFifthDiv").style.marginTop= (e+u) + "px";

    }

    return srmAssociation;


})();