var srmCorpActionRules = (function () {
    function SRMCorpActionRules() {
        this._path = '';
    }
    SRMCorpActionRules.prototype.setPath = function setPath() {
        var path = window.top.location.protocol + '//' + window.top.location.host;
        var pathname = window.top.location.pathname.split('/');


        $.each(pathname, function (ii, ee) {
            if ((ii !== 0) && (ii !== pathname.length - 1))
                path = path + '/' + ee;
        });

        srmCorpActionRules._path = path;
    }

    SRMCorpActionRules.prototype.OpenSRMIframe = function OpenSRMIframe(RuleType, CorpTypeId, CorpTypeName, TemplateId, AttributeId, AttributeName, ModuleID, AttributeChanged) {

        var iframe = $("[id$='iframeLandingRulesScreen']");

        url = srmCorpActionRules._path + '/../SMAttributeRuleSetup.aspx?RuleType=' + RuleType + '&SecurityTypeId=' + CorpTypeId + '&SecTypeName=' + CorpTypeName + '&TemplateId=' + TemplateId + '&AttributeId=' + AttributeId + '&AttributeName=' + AttributeName + '&ModuleId=' + ModuleID + '&AttributeChanged=' + AttributeChanged;

        iframe.attr("src", url);
        SRMIframeResize();

    }

    function SRMIframeResize() {
        var iframeHeight, width;
        iframeHeight = $(window).height() - ($('.SRMCorpActionHeaderSection').height() + $('.SRMCorpActionDDLSection').height() + 20);
        width = $(window).width();
        $("[id$='iframeLandingRulesScreen']").height(iframeHeight).outerWidth(width);
    }

    function getRulesData() {
        var RuleType = 'MnemonicsRuleSetup';
        var CorpTypeId = $('#hdnCorpActionTypeID').val();
        var CorpTypeName = $('#hdnCorpActionTypeName').val();
        var TemplateId = '0';
        var AttributeId = (($('#hdnAttributeID').val() == '') ? '-1' : $('#hdnAttributeID').val());
        var AttributeName = (($('#hdnAttributeName').val() == '') ? '' : $('#hdnAttributeName').val());
        var ModuleID = 'CA';
        var AttributeChanged = (AttributeId != '-1' && AttributeId != '') ? 'true' : 'false';


        srmCorpActionRules.OpenSRMIframe(RuleType, CorpTypeId, CorpTypeName, TemplateId, AttributeId, AttributeName, ModuleID, AttributeChanged);
    }

    var srmCorpActionRules = new SRMCorpActionRules();
    srmCorpActionRules.setPath();
    return srmCorpActionRules;

}());


$(document).ready(function () {
    srmCorpActionRules.getRulesData();
});