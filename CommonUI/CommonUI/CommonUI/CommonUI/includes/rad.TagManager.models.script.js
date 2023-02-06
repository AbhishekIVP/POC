//Type.registerNamespace('rad.TagManager');
var rad = function rad() { };
rad.TagManager = {};
rad.TagManager.models = {
    tagInfo: function () {
        this.TagId;
        this.TagName;
        this.TagDesc;
        this.ReferenceDimension;
        this.ReferenceAttribute;
        this.DefaultValue;
        this.DataSet;
        this.IsAlsoDimension;
        this.ActionBy;
        this.UserAction;
        this.TagPriority;
        this.RuleInfo;
    },
    RuleInfo: function () {
        this.TagId;
        this.EffectiveStartDate;
        this.EffectiveEndDate;
        this.RuleId;
        this.RuleName;
        this.PolarisRuleMappingId;
        this.IsOverride;
        this.UserAction;
        this.TagOverrideValue;
        this.Priority;
    },
    RuleFilterInfo: function () {
        this.EffectiveStartDate;
        this.EffectiveEndDate;
        this.SearchTerm;
        this.TagIds;
        this.CurrentPageIndex;
        this.IsOverrideRule;
    },
    TagFilterInfo: function () {
        this.SearchTerm;
        this.CurrentPageIndex;
    }
};
rad.TagManager.tag = function (data, dt) {
    this.TagName = ko.observable(data.TagName);
    this.TagDesc = ko.observable(data.TagDesc);
    this.TagId = ko.observable(data.TagId);
    this.ReferenceDimension = ko.observable(data.ReferenceDimension);
    this.ReferenceAttribute = ko.observable(data.ReferenceAttribute);
    this.DefaultValue = ko.observable(data.DefaultValue);
    this.DataSet = ko.observable(data.DataSet);
    this.IsAlsoDimension = ko.observable(data.IsAlsoDimension);
    this.ActionBy = ko.observable(data.ActionBy);
    this.TagPriority = ko.observable(data.TagPriority);
    this.Action = ko.observable(data.Action);
    this.MaxValidTagPriority = ko.observable(data.MaxValidTagPriority);
    this.tagType = ko.observable(data.TagType);
    this.IsPersistant = ko.observable(data.IsPersistant);
    this.TagIsEditable = ko.observable(data.TagIsEditable);
    this.MappedAttributes = ko.observable(data.MappedAttributes);
    this.Edit = data.Edit;
    this.Delete = data.Delete;
    this.CustTagPriority = data.CustTagPriority;
    var that = this;
    $.each(['TagName', 'TagDesc', 'ReferenceDimension', 'DefaultValue', 'ReferenceAttribute', 'TagPriority', 'MappedAttributes', 'TagId', 'DataSet'], function (i, prop) {
        that[prop].subscribe(function (val) {
            var rowIdx = dt.column(0).data().indexOf(that.TagId);
            dt.row(rowIdx).invalidate();
        });
    });
}

rad.TagManager.rule = function (data) {
    this.TagId = ko.observable(data.TagId);
    this.EffectiveStartDate = ko.observable(data.EffectiveStartDate);
    this.EffectiveEndDate = ko.observable(data.EffectiveEndDate);
    this.RuleId = ko.observable(data.RuleId);
    this.RuleName = ko.observable(data.RuleName);
    this.PolarisRuleMappingId = ko.observable(data.PolarisRuleMappingId);
    this.IsOverride = ko.observable(data.IsOverride);
    this.UserAction = ko.observable(data.UserAction);
    this.TagOverrideValue = ko.observable(data.TagOverrideValue);
    this.Priority = ko.observable(data.Priority);
    this.RuleText = ko.observable(data.RuleText);
    this.RuleSetId = ko.observable(data.RuleSetId);
}

rad.TagManager.ruleFilter = function (data) {
    this.EffectiveStartDate = ko.observable(data.EffectiveStartDate);
    this.EffectiveEndDate = ko.observable(data.EffectiveEndDate);
    this.SearchTerm = ko.observable(data.SearchTerm);
    this.TagIds = ko.observable(data.TagIds);
    this.CurrentPageIndex = ko.observable(data.CurrentPageIndex);
    this.IsOverrideRule = ko.observable(data.IsOverrideRule);
}

rad.TagManager.CHKDEL = function (data) {
    this.tagName = ko.observable(data.TagName);
    this.ruleName = ko.observable(data.ruleName);
}