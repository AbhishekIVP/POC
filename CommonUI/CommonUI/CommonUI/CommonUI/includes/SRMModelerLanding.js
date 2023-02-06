var srmModeler = (function () {
    function SRMModeler() {
        this._path = '';
    }
    SRMModeler.prototype.setPath = function setPath() {
        var path = window.top.location.protocol + '//' + window.top.location.host;
        var pathname = window.top.location.pathname.split('/');

        $.each(pathname, function (ii, ee) {
            if ((ii !== 0) && (ii !== pathname.length - 1))
                path = path + '/' + ee;
        });
        path = path.replace('Modeler', '');
        srmModeler._path = path;
    }

    SRMModeler.prototype.ajaxCall = function ajaxCall(methodName, parameters, onSuccess, onError, onComplete) {
        $.ajax({
            type: 'POST',
            url: srmModeler._path + 'Service/Modeler.asmx/' + methodName,
            contentType: "application/json",
            dataType: "json",
            success: onSuccess,
            error: onError,
            complete: onComplete,
            data: JSON.stringify(parameters)
        });
    }

    var srmModeler = new SRMModeler();
    srmModeler.setPath();
    return srmModeler;
}());

$(document).ready(function (e) {
    var parameters = new Object();
    parameters.moduleId = parseInt($('#hdnModuleId').val());

    var onSuccess = function (data) {
        var htmltobind = "";
        
        htmltobind += '<div class="SRCHeader"><div class="SRCColumn config">Configure</div><div class="SRCColumn" style="padding-left:30px;" >Entity Type</div><div class="SRCColumn" >Tags</div><div class="SRCColumn delete" style="padding=left:70px;">Delete</div></div>';
        htmltobind += '<div class="SRCBody">';
        data = JSON.parse(data.d);

        if (parameters.moduleId == 3) {
            $.each(data, function (i, obj) {

                htmltobind += '<div class="SRCRow bordered">';
                htmltobind += '<div class="col config" style="width:15px;">' + '<i class="fa fa-gear" style="font-size:13px" onclick="RedirectToConfig('+obj.EntityTypeId+')"></i>' + '</div>' + '<div class="col sectype text"title="' + obj.Type + '">' + obj.Type + '</div>'
                + '<div class="col tag text" style="width:300px">';
                for (var j = 0; j < obj.Tags.length; j++) {
                    if (obj.Tags[j] != "")
                        htmltobind += '<div class="tagcell" title="' + obj.Tags[j] + '">' + " " + obj.Tags[j] + '</div>';
                }
                htmltobind += '</div>';
                htmltobind += '<div class="btn-group col" id="hoverbuts"><button>A</button><button>L</button><button>R</button><button>D</button></div>'
                    + '<div class="col delete">' + '<i class="fa fa-trash-o" style="font-size:13px;"></i>' + '</div>';
                htmltobind += '</div>';
                
            })
           
            $("#changetext").val('Add SecType');
            $("#searchtext").attr('placeholder', 'Search Security Type or Tags');
        };

        if (parameters.moduleId == 6) {
            $.each(data, function (i, obj) {
                htmltobind += '<div class="SRCRow bordered">';
                htmltobind += '<div class="col config" style="width:15px;">' + '<i class="fa fa-gear" style="font-size:13px" onclick="RedirectToConfig('+obj.EntityTypeId+')"></i>' + '</div>' + '<div class="col entitytype text" title="' + obj.Type + '">' + obj.Type + '</div>'
                    + '<div class="col tag text" style="width:300px">';
                for (var j = 0; j < obj.Tags.length; j++) {
                    if (obj.Tags[j] != "")
                        htmltobind += '<div class="tagcell" title="' + obj.Tags[j] + '">' + " " + obj.Tags[j] + '</div>';
                }
                htmltobind += '</div>';
                htmltobind += '<div class="btn-group col" id="hoverbuts"><button>A</button><button>L</button><button>R</button><button>D</button></div>' + '<div class="col delete">' + '<i class="fa fa-trash-o" style="font-size:13px;"></i>' + '</div>';
                htmltobind += '</div>';

            })
            $("#changetext").val('Add Entity Type');
            $("#searchtext").attr('placeholder', 'Search Entity Type or Tags');
        };

        if (parameters.moduleId == 9) {
            $.each(data, function (i, obj) {
                htmltobind += '<div class="SRCRow bordered">';
                htmltobind += '<div class="col config" style="width:15px;">' + '<i class="fa fa-gear" style="font-size:13px" onclick="RedirectToConfig(' + obj.EntityTypeId + ')"></i>' + '</div>' + '<div class="col corpactiontype text" title="' + obj.Type + '">' + obj.Type + '</div>'
                    + '<div class="col tag text" style="width:300px">';
                for (var j = 0; j < obj.Tags.length; j++) {
                    if (obj.Tags[j] != "")
                        htmltobind += '<div class="tagcell" title="' + obj.Tags[j] + '">' + " " + obj.Tags[j] + '</div>';
                }
                htmltobind += '</div>';
                htmltobind += '<div class="btn-group col" id="hoverbuts"><button>A</button><button>L</button><button>R</button><button>D</button></div>' + '<div class="col delete">' + '<i class="fa fa-trash-o" style="font-size:13px;"></i>' + '</div>';
                htmltobind += '</div>';


            })
            $("#changetext").val('Add CorpAction');
            $("#searchtext").attr('placeholder', 'Search CorpAction Type');
        };
        htmltobind += '</div>';
        $("#SRCContainer").html(htmltobind);

    }
    

    srmModeler.ajaxCall('GetAllTypes', parameters, onSuccess);
});

function searchrecord() {
   
    var tosearch = $("#searchtext").val();
    var getdivs = $(".SRCRow");
    //var tosearch = document.getElementById("searchtext").value;
    //var getdivs = document.getElementsByClassName("SRCRow");

    for (var counter = 0; counter < getdivs.length; counter++) {
        var found = false;
        var findcells = $(getdivs).eq(counter).find('.text');

        for (var cellcount = 0; cellcount < findcells.length; cellcount++) {
            if (findcells[cellcount].innerHTML.indexOf(tosearch) > -1) {

                found = true;
                break;
            }
        }

        if (found) {

            getdivs[counter].style.display = "block";

        }
        else {
            getdivs[counter].style.display = "none";
        }
    }
};

function RedirectToConfig(id) {
    
    //var grid = $(".SRCRow").data('EntityTypeId');
    //var item = grid.dataItem(grid.select());
    
    var parameters = new Object();
    parameters.moduleId = parseInt($('#hdnModuleId').val());
    var argdata = {
        module: parameters.moduleId,
        EntityTypeId:id
    }
    window.location = "./ConfigurePage.aspx?"+$.param(argdata);
    
};





