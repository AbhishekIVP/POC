var path = window.location.protocol + '//' + window.location.host;
var pathname = window.location.pathname.split('/');

$.each(pathname, function (ii, ee) {
    if ((ii !== 0) && (ii !== pathname.length - 1))
        path = path + '/' + ee;
});


var RuleDivId = null;
var BtnSaveId = null;
var HdnRuleClassInfoId = null;

function RuleEditorInitiator(flagBindRuleEditor, RuleTypeRAD, DDLBasketSelectedValue, DdlBasketUniqueId, lstReferenceAttributeName, ruleDivId, btnSaveId, hdnRuleClassInfoId, SecurityTypeId) {
    RuleDivId = ruleDivId;
    BtnSaveId = btnSaveId;
    HdnRuleClassInfoId = hdnRuleClassInfoId;
    sectype_table_id_Split = DDLBasketSelectedValue.split('<$>');
    sectype_table_id = parseInt(sectype_table_id_Split[0]);
    is_additional_leg = false;
    if (sectype_table_id_Split.length > 1)
        is_additional_leg = (sectype_table_id_Split[1] == "true");

    var parameters = {
    };
    parameters.flagBindRuleEditor = flagBindRuleEditor;
    parameters.RuleTypeRAD = RuleTypeRAD;
    parameters.DDLBasketSelectedValue = sectype_table_id;
    parameters.DdlBasketUniqueId = parseInt(sectype_table_id);
    parameters.lstReferenceAttributeName = lstReferenceAttributeName;
    parameters.SecurityTypeId = SecurityTypeId;
    parameters.is_additional_leg = is_additional_leg;


    callService('POST', path + '/BaseUserControls/Service/SMDashboardService.svc', 'PrepareRuleGrammarInfo', parameters, onSuccess_RuleEditorInitiator, OnFailure, null, null, false);
}

function onSuccess_RuleEditorInitiator(result) {
    if (result.d != null) {
        if (typeof ($("#" + RuleDivId).data('ruleEngine')) !== "undefined")
            $("#" + RuleDivId).ruleEngine().data('ruleEngine').Destroy();
        $("#" + RuleDivId).empty();
        $("#" + RuleDivId).ruleEngine({ grammarInfo: result.d, serviceUrl: path + "/BaseUserControls/Service/RADXRuleEditorService.svc", ExternalFunction: RuleCompleteHandler });
    }
}

function RuleCompleteHandler(state) {
    setTimeout(function () {
        if (state) {
            $('#' + BtnSaveId).show();
            var ruleClass = $("#" + RuleDivId).ruleEngine().data('ruleEngine').getGeneratedCode();
            var ruleText = $("#ruleTxt").val();
            $('#' + HdnRuleClassInfoId).val($('<div/>').text(ruleClass[0] + "||$$||" + ruleClass[1] + "||$$||" + ruleText).html());
        }
        else
            $('#' + BtnSaveId).hide();
    }, 200);
}