var srmModelerConfig = (function () {
    function SRMModeler() {
        this._path = '';
        this.users = '';
        this.groups = '';
    }
    SRMModeler.prototype.setPath = function setPath() {
        var path = window.location.protocol + '//' + window.location.host;
        var pathname = window.location.pathname.split('/');

        $.each(pathname, function (ii, ee) {
            if ((ii !== 0) && (ii !== pathname.length - 1))
                path = path + '/' + ee;
        });
        path = path.replace('Modeler', '');
        srmModelerConfig._path = path;
    }

    SRMModeler.prototype.ajaxCall = function ajaxCall(methodName, parameters, onSuccess, onError, onComplete) {
        $.ajax({
            type: 'POST',
            url: srmModelerConfig._path + 'Service/Modeler.asmx/' + methodName,
            contentType: "application/json",
            dataType: "json",
            success: onSuccess,
            error: onError,
            complete: onComplete,
            data: JSON.stringify(parameters)
        });
    }

    var srmModelerConfig = new SRMModeler();
    srmModelerConfig.setPath();
    return srmModelerConfig;
}());

$(document).ready(function () {
    var parameters = new Object();
    parameters.moduleId = parseInt($('#hdnModuleId').val());
    parameters.EntityTypeId = parseInt($('#hdnEntityTypeId').val());
  
    var onSuccess = function (data) {
        data = JSON.parse(data.d);

        $("#txtSecName").val(data.SecTypeName);
        $("#basketcheck").css("display", "none");
        if (data.IsVanilla == true) {
            $("#vanilla").attr('checked', true);
            $("#basketcheck").css("display", "block");
        }
        if (data.IsExotic == true) {
            $("#exotic").attr('checked', true);
            $("#basketcheck").css("display", "none");
        }
       
        $('input[type=radio][name=SecType]').change(function () {
            var sectypeselected = this.value;
            if (sectypeselected == "vanilla") {
                $("#vanilla").attr('checked', true);
                $("#basketcheck").css("display", "block");
            }
            if (sectypeselected == "exotic") {
                $("#exotic").attr('checked', true);
                $("#basketcheck").css("display", "none");
            }
        });
    

       
        $("#txtSecDesc").text(data.SecTypeDesc);

      
      
        $('#dateoptions').on("change", function () {
            var selectedoption = this.value;
            if (selectedoption == "custom") {
                $("#DateSelection").css("display", "inline-block");
                
                $("#dateinput").attr('value', data.DefaultDate);
            }
           else {
               $("#DateSelection").css("display", "none");
           }
           $("select option[value='"+selectedoption+"']").attr("selected", "selected");
        });

     
        var fortagdisp = new Object();
        fortagdisp.container = $("#divTag");
        fortagdisp.list = data.Tags;
        tag.create(fortagdisp);

        srmModelerConfig.users = data.Users;
        srmModelerConfig.groups = data.Groups;
    }
    srmModelerConfig.ajaxCall('GetAllConfigInfo', parameters, onSuccess);

    var onSuccessU = function (data) {
        data = JSON.parse(data.d);
        var newdata = [];
        var exixtinguser = [];
        $.each(data, function (i, obj) {
            var text = obj.FirstName + " " + obj.LastName + "(" + obj.username + ")";
            if (srmModelerConfig.users.indexOf(obj.email) > -1)
                exixtinguser.push(text);
            newdata.push({ text: text, value: obj.email });
        });
        var userlist = { id: 'users', text: 'Users', heading: 'Users', showSearch: true, container: $('#divUsers'), data: newdata, selectedItems: exixtinguser };
        altshut.create(userlist);
    }
    srmModelerConfig.ajaxCall('GetAllUsers', parameters, onSuccessU);

    var onSuccessG = function (data) {
        data = JSON.parse(data.d);
        var newdata = [];
        var exixtinggroup = [];
        $.each(data, function (i, obj) {
            var text = obj.groupname;
            if (srmModelerConfig.groups.indexOf(obj.groupname) > -1)
                exixtinggroup.push(text);
            newdata.push({ text:text , value: obj.groupname });
        })
        var grouplist = { id: 'groups', text: 'Groups', heading: 'Groups', showSearch: true, container: $('#divGroups'), data: newdata, selectedItems: exixtinggroup };
        altshut.create(grouplist);
    }
    srmModelerConfig.ajaxCall('GetAllGroups', parameters, onSuccessG);
});

function RedirectToAttributeSetupPage() {
    var parameters = new Object();
    parameters.moduleId = parseInt($('#hdnModuleId').val());
    parameters.TypeId = parseInt($('#hdnEntityTypeId').val());
    var argdata = {
        module: parameters.moduleId,
        TypeId: parameters.TypeId
    }
    window.location.href = "./AttributeSetupPage.aspx?"+$.param(argdata);
}