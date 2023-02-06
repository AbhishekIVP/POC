
var legModeler = (function () {
    function LegModeler() {
        this._path = '';
        this._leftMenuPath = {};
        //this._moduleId = 3
    }



    LegModeler.prototype.setPath = function setPath() {
        var path = window.location.protocol + '//' + window.location.host;
        var pathname = window.location.pathname.split('/');

        $.each(pathname, function (ii, ee) {
            if ((ii !== 0) && (ii !== pathname.length - 1))
                path = path + '/' + ee;
        });
        path = path.replace('Modeler', '');
        legModeler._path = path;
    }

    LegModeler.prototype.ajaxCall = function ajaxCall(methodName, parameters, onSuccess, onError, onComplete) {
        $.ajax({
            type: 'POST',
            url: legModeler._path + 'Service/AttributeSetupPage.asmx/' + methodName,
            contentType: "application/json",
            dataType: "json",
            success: onSuccess,
            error: onError,
            complete: onComplete,
            data: JSON.stringify(parameters)
        });
    }

    var legModeler = new LegModeler();


    var leftMenuPath = "";
    if (typeof (window.parent.leftMenu) !== "undefined")
        leftMenuPath = window.parent.leftMenu;
    else if (typeof (window.parent.parent.leftMenu) !== "undefined")
        leftMenuPath = window.parent.parent.leftMenu;
    legModeler._leftMenuPath = leftMenuPath;



    legModeler.setPath();
    return legModeler;
}());

var parameters = new Object();

$(document).ready(function () {
    //var parameters = {};

    parameters.moduleId = parseInt($('#hdnmoduleId').val());
    parameters.templateId = parseInt($('#hdnTemplateId').val());
    parameters.TypeId = parseInt($('#hdnTypeId').val());


    //parameters.moduleId = 6;
    moduleId = parameters.moduleId;
    typeId = parameters.TypeId;
    var onSuccess = function (data) {

        data = JSON.parse(data.d);
        RenderLegData(data, moduleId);


        var listParent = $("#LegAttributePage");
        var noResultDiv = $("#SRMLegAttributePageNoResult");
        //show/hide no legs
        if (data.length == 0) {
            noResultDiv.css('display', 'block');
            noResultDiv.css('height', listParent.height());
            listParent.css('display', 'none');
            if (legModeler._leftMenuPath)
                legModeler._leftMenuPath.showNoRecordsMsg('No legs configured.', noResultDiv);
            noResultDiv.css('width', "100%");
        }
        else {
            noResultDiv.css('display', 'none');
            if (legModeler._leftMenuPath)
                legModeler._leftMenuPath.hideNoRecordsMsg();
            listParent.css('display', 'block');
        }

    }
    var onError = function onError(err) {
        debugger;
    }
    //var onComplete = function onComplete() {

    //}
    var totalHeight = $(window).height();
    $('#LegAttributePage').css('height', totalHeight - 25 - 20 - 3);
    $("#SRMLandingPageListParent").css('height', totalHeight - 25 - 20 - 3);

    //var offset = $("#RenderLegInfo").offset().top;
    //$("#RenderLegInfo").css('height', totalHeight - offset);
    var totalwidth = $(window).width();
    if (moduleId == 3 || moduleId == 9) {
        $(".SetWidth").css('width', (totalwidth / 6) - 31);
    }
    else {
        $(".SetWidth").css('width', (totalwidth / 6) - 31);
    }

    //set scroll
    var topHeight = $(".smLegAttributePage_headerSection").offset().top + $(".smLegAttributePage_headerSection").height() + 15;
    $("#LegAttributePage").css('visibility', '');
    $("#RenderLegInfo").css('height', $(window).height() - topHeight).css('overflow-y','auto');
    legModeler.ajaxCall('GetLegDetails', parameters, onSuccess, onError);


    //close Button


});
viewmodelobj = {};
RenderLegData = function (data, moduleId) {
    var modelcontainer = {};
    modelcontainer.data = data;
    viewmodelobj = ko.mapping.fromJS(modelcontainer);
    viewmodelobj.deletedAttributes = ko.observableArray();
    viewmodelobj.newAttributes = ko.observableArray();
    viewmodelobj.moduleId = ko.observable(moduleId);
    ko.applyBindings(viewmodelobj, document.getElementById("LegAttributePageParent"));
    viewmodelobj.onClickShowAttrInfo = function (obj, event) {

        var customattrLegId = obj.LegId();
        //var legName = obj.LegName();
        var customattrIsAdditional = obj.IsAdditionalLeg();
        parameters.isLeg = true
        var i;
        var attrPagePath = legModeler._path + "Modeler/AttributeSetupPage.aspx?";
        attrPagePath += "module=" + parameters.moduleId + "&";
        attrPagePath += "typeid=" + customattrLegId + "&";
        attrPagePath += "templateId=" + parameters.templateId + "&";
        attrPagePath += "isLeg=" + parameters.isLeg + "&";
        attrPagePath += "isAdditionalLeg=" + customattrIsAdditional;

        var setcollapse = $(event.target).closest('.LegRow');
        if (setcollapse.attr('iscollapsed') == '1') {
            setcollapse.find('.fa-minus').removeClass('fa-minus').addClass('fa-plus');
            setcollapse.attr('iscollapsed', '0');
            $('#SRMAttributeLegIFrameContainer' + customattrLegId).css('display', 'none');

        }
        else {
            setcollapse.find('.fa-plus').removeClass('fa-plus').addClass('fa-minus');
            $(".LegRow[iscollapsed=1]").find('.fa-minus').removeClass('fa-minus').addClass('fa-plus');
            $(".LegRow[iscollapsed=1]").attr('iscollapsed', '0');
            setcollapse.attr('iscollapsed', '1');
            $(".SRMAttributeLegIFrameContainer").css('display', 'none');
            $("#SRMAttributeLegIFrameContainer" + customattrLegId).css('display', 'block');
        }



        //clear 
        //$(".LegRow").attr('iscollapsed', 0);
        //current selection
        var iframe = $("#SRMAttributeLegIFrame" + customattrLegId);

        if (!iframe.attr('src'))
            iframe.attr('src', attrPagePath);

        let left = $(".LegFirst").offset().left + $(".LegFirst").width() + 2;
        iframe.css('margin-left', left);
        //iframe.width($(window).width() - 400);
        iframe.height(200);

        iframe.css('width', $(".LegRow").width() - ($(".LegFirst").width() + 2));

        var totalHeight = $(window).height();
        $('#LegAttributePageParent').css('height', totalHeight - 25 - 20 - 3);
        //var offset = iframe.offset().top;
        //iframe.css('margin-left', '260px');
        //iframe.css('margin-top', '10px');
        iframe.css('border', 'none');
        iframe.css('background', 'white');
        //iframe.css('border-radius', '10px');
        iframe.css('box-shadow', '0px 0px 5px 3px #dedede');


        $(".closeButton").off('click').on('click', function (event) {
            $(event.target).closest('.SRMAttributeLegIFrameContainer').css('display', 'none');
        });
        $('#PopUpAttributePage').prop('src', attrPagePath);
    }
    viewmodelobj.ToggleUnderlier = function (obj, event) {
        //var targetevent = $(event.target);
        //if (targetevent.hasClass('ToggleYes')) {
        //    obj.HasUnderlier('true');
        //    $('.SelectedClass').removeClass("SelectedClass");
        //    targetevent.addClass("SelectedClass");
        //    //$('.UnderlierYes').css('background-color', '#43D9C6');
        //    $('.SelectedClass').css('background-color', '#43D9C6');
        //}
        //else {
        //    obj.HasUnderlier('false');
        //    // $('.SelectedClass').removeClass("SelectedClass");
        //    //targetevent.addClass("SelectedClass");
        //    //$('.SelectedClass').css('background-color', '#43D9C6');
        //}
    }

}



