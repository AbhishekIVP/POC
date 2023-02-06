// RefMasterJSCommon.js
//


Type.registerNamespace('com.ivp.refmaster.scripts.common.RefMasterJSInfo');

////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSPromoteVendorInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSPromoteVendorInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorInfo() {
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSPromoteVendorInfoControlIDs

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSPromoteVendorInfoControlIDs = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorInfoControlIDs() {
    /// <field name="btnSearchData" type="String">
    /// </field>
    /// <field name="promoteVendorFormContainer" type="String">
    /// </field>
    /// <field name="tabContainer" type="String">
    /// </field>
    /// <field name="divErrorDisplay" type="String">
    /// </field>
    /// <field name="lblSearchError" type="String">
    /// </field>
    /// <field name="lblSuccess" type="String">
    /// </field>
    /// <field name="RMRawEntitiesBtnProto" type="String">
    /// </field>
    /// <field name="RMPromotedEntitiesBtnProto" type="String">
    /// </field>
    /// <field name="btnPromote" type="String">
    /// </field>
    /// <field name="RMDivider" type="String">
    /// </field>
    /// <field name="pnlPromote" type="String">
    /// </field>
    /// <field name="gridSearchData" type="String">
    /// </field>
    /// <field name="gridPromotionStatus" type="String">
    /// </field>
    /// <field name="btnPromoteHierarchy" type="String">
    /// </field>
    /// <field name="btnCancelPromoteHierarchy" type="String">
    /// </field>
    /// <field name="btnPromoteSelected" type="String">
    /// </field>
    /// <field name="pnlPromoteWithoutHierarchy" type="String">
    /// </field>
    /// <field name="btnPromoteHierarchyHidden" type="String">
    /// </field>
    /// <field name="hdnSelectedHierarchy" type="String">
    /// </field>
    /// <field name="IsHierarchyConfigured" type="String">
    /// </field>
    /// <field name="btnCancelPromoteSelected" type="String">
    /// </field>
    /// <field name="rbPromoteSelection" type="String">
    /// </field>
    /// <field name="ctmPromoteVendor" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSPromoteVendorInfoControlIDs.prototype = {
    btnSearchData: null,
    promoteVendorFormContainer: null,
    tabContainer: null,
    divErrorDisplay: null,
    lblSearchError: null,
    lblSuccess: null,
    RMRawEntitiesBtnProto: null,
    RMPromotedEntitiesBtnProto: null,
    btnPromote: null,
    RMDivider: null,
    pnlPromote: null,
    gridSearchData: null,
    gridPromotionStatus: null,
    btnPromoteHierarchy: null,
    btnCancelPromoteHierarchy: null,
    btnPromoteSelected: null,
    pnlPromoteWithoutHierarchy: null,
    btnPromoteHierarchyHidden: null,
    hdnSelectedHierarchy: null,
    IsHierarchyConfigured: null,
    btnCancelPromoteSelected: null,
    rbPromoteSelection: null,
    ctmPromoteVendor: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSPromoteVendorControls

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSPromoteVendorControls = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls(objControlIds) {
    /// <param name="objControlIds" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSPromoteVendorInfoControlIDs">
    /// </param>
    /// <field name="_objPromoteVendorControlIds" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSPromoteVendorInfoControlIDs">
    /// </field>
    this._objPromoteVendorControlIds = objControlIds;
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSPromoteVendorControls.prototype = {
    _objPromoteVendorControlIds: null,
    
    get_promoteVendorFormContainer: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_promoteVendorFormContainer() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.promoteVendorFormContainer);
    },
    
    get_rmPromotedEntitiesBtnProto: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_rmPromotedEntitiesBtnProto() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.RMPromotedEntitiesBtnProto);
    },
    
    get_rmRawEntitiesBtnProto: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_rmRawEntitiesBtnProto() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.RMRawEntitiesBtnProto);
    },
    
    get_btnSearchData: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_btnSearchData() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.btnSearchData);
    },
    
    get_btnPromote: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_btnPromote() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.btnPromote);
    },
    
    get_btnPromoteHierarchy: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_btnPromoteHierarchy() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.btnPromoteHierarchy);
    },
    
    get_btnCancelPromoteHierarchy: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_btnCancelPromoteHierarchy() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.btnCancelPromoteHierarchy);
    },
    
    get_btnPromoteSelected: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_btnPromoteSelected() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.btnPromoteSelected);
    },
    
    get_rbPromoteSelection: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_rbPromoteSelection() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.rbPromoteSelection);
    },
    
    get_pnlPromoteWithoutHierarchy: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_pnlPromoteWithoutHierarchy() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.pnlPromoteWithoutHierarchy);
    },
    
    get_btnPromoteHierarchyHidden: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_btnPromoteHierarchyHidden() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.btnPromoteHierarchyHidden);
    },
    
    get_isHierarchyConfigured: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_isHierarchyConfigured() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.IsHierarchyConfigured);
    },
    
    get_btnCancelPromoteSelected: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_btnCancelPromoteSelected() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.btnCancelPromoteSelected);
    },
    
    get_hdnSelectedHierarchy: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_hdnSelectedHierarchy() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.hdnSelectedHierarchy);
    },
    
    get_gridSearchData: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_gridSearchData() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.gridSearchData);
    },
    
    get_rmDivider: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_rmDivider() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.RMDivider);
    },
    
    get_tabContainer: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_tabContainer() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.tabContainer);
    },
    
    get_lblSuccess: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_lblSuccess() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.lblSuccess);
    },
    
    get_lblSearchError: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_lblSearchError() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.lblSearchError);
    },
    
    get_divErrorDisplay: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_divErrorDisplay() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.divErrorDisplay);
    },
    
    get_ctmPromoteVendor: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSPromoteVendorControls$get_ctmPromoteVendor() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objPromoteVendorControlIds.ctmPromoteVendor);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationInfo() {
    /// <field name="AttributeChangedHandler" type="String">
    /// </field>
    /// <field name="SetAttributeInfoHandler" type="String">
    /// </field>
    /// <field name="IsDerivedHandler" type="String">
    /// </field>
    /// <field name="DerivedHandler" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationInfo.prototype = {
    AttributeChangedHandler: null,
    SetAttributeInfoHandler: null,
    IsDerivedHandler: null,
    DerivedHandler: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationControlInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationControlInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationControlInfo() {
    /// <field name="DdlAttributeId" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationControlInfo.prototype = {
    DdlAttributeId: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationControls

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationControls = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationControls(controlInfo) {
    /// <param name="controlInfo" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationControlInfo">
    /// </param>
    /// <field name="_controlInfo" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationControlInfo">
    /// </field>
    /// <field name="_ddlAttribute" type="Object" domElement="true">
    /// </field>
    this._controlInfo = controlInfo;
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationControls.prototype = {
    _controlInfo: null,
    _ddlAttribute: null,
    
    get_ddlAttribute: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationControls$get_ddlAttribute() {
        /// <value type="Object" domElement="true"></value>
        if (this._ddlAttribute == null) {
            this._ddlAttribute = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.DdlAttributeId);
        }
        return this._ddlAttribute;
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationPerAttributeInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationPerAttributeInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeInfo() {
    /// <field name="AttributeChanged" type="Boolean">
    /// </field>
    /// <field name="AttributeIdChangedClientHandler" type="String">
    /// </field>
    /// <field name="AttributeNameChangedClientHandler" type="String">
    /// </field>
    /// <field name="AttributeId" type="Number" integer="true">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationPerAttributeInfo.prototype = {
    AttributeChanged: false,
    AttributeIdChangedClientHandler: null,
    AttributeNameChangedClientHandler: null,
    AttributeId: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationPerAttributeControlInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationPerAttributeControlInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControlInfo() {
    /// <field name="GridAttributeRulesId" type="String">
    /// </field>
    /// <field name="DivRuleCreationId" type="String">
    /// </field>
    /// <field name="DivAttributeRulesId" type="String">
    /// </field>
    /// <field name="TxtRuleNameId" type="String">
    /// </field>
    /// <field name="RRuleEditorId" type="String">
    /// </field>
    /// <field name="BtnSaveRuleId" type="String">
    /// </field>
    /// <field name="BtnCancelId" type="String">
    /// </field>
    /// <field name="ModalSwitchStateId" type="String">
    /// </field>
    /// <field name="ModalPriorityId" type="String">
    /// </field>
    /// <field name="ModalDeleteId" type="String">
    /// </field>
    /// <field name="ModalMessageSuccessId" type="String">
    /// </field>
    /// <field name="ModalErrorId" type="String">
    /// </field>
    /// <field name="TxtPriorityId" type="String">
    /// </field>
    /// <field name="BtnPrioritySaveId" type="String">
    /// </field>
    /// <field name="BtnPriorityCancelId" type="String">
    /// </field>
    /// <field name="LblRuleToDeleteId" type="String">
    /// </field>
    /// <field name="BtnDeleteYESId" type="String">
    /// </field>
    /// <field name="BtnDeleteNOId" type="String">
    /// </field>
    /// <field name="LblRuleNameSwitchId" type="String">
    /// </field>
    /// <field name="BtnSwitchYesId" type="String">
    /// </field>
    /// <field name="BtnSwitchNoId" type="String">
    /// </field>
    /// <field name="LblSuccessId" type="String">
    /// </field>
    /// <field name="BtnSuccessId" type="String">
    /// </field>
    /// <field name="LblErrorId" type="String">
    /// </field>
    /// <field name="BtnErrorId" type="String">
    /// </field>
    /// <field name="LblSwitchStateId" type="String">
    /// </field>
    /// <field name="HdnRuleInfoId" type="String">
    /// </field>
    /// <field name="HdnPriorityInfoId" type="String">
    /// </field>
    /// <field name="HdnRuleNameInfoId" type="String">
    /// </field>
    /// <field name="HdnUpdateRuleInfoId" type="String">
    /// </field>
    /// <field name="HdnSelectedAttributeNameId" type="String">
    /// </field>
    /// <field name="ModalDerivedWarningId" type="String">
    /// </field>
    /// <field name="BtnDerivedWarningYesId" type="String">
    /// </field>
    /// <field name="BtnDerivedWarningNoId" type="String">
    /// </field>
    /// <field name="HdnDerivedAttributesId" type="String">
    /// </field>
    /// <field name="PanelToDisplayRuleCreationId" type="String">
    /// </field>
    /// <field name="XlGridAttributeRulesId" type="String">
    /// </field>
    /// <field name="HdnRuleInfoForLoggingId" type="String">
    /// </field>
    /// <field name="CpeGridAttributeRulesId" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationPerAttributeControlInfo.prototype = {
    GridAttributeRulesId: null,
    DivRuleCreationId: null,
    DivAttributeRulesId: null,
    TxtRuleNameId: null,
    RRuleEditorId: null,
    BtnSaveRuleId: null,
    BtnCancelId: null,
    ModalSwitchStateId: null,
    ModalPriorityId: null,
    ModalDeleteId: null,
    ModalMessageSuccessId: null,
    ModalErrorId: null,
    TxtPriorityId: null,
    BtnPrioritySaveId: null,
    BtnPriorityCancelId: null,
    LblRuleToDeleteId: null,
    BtnDeleteYESId: null,
    BtnDeleteNOId: null,
    LblRuleNameSwitchId: null,
    BtnSwitchYesId: null,
    BtnSwitchNoId: null,
    LblSuccessId: null,
    BtnSuccessId: null,
    LblErrorId: null,
    BtnErrorId: null,
    LblSwitchStateId: null,
    HdnRuleInfoId: null,
    HdnPriorityInfoId: null,
    HdnRuleNameInfoId: null,
    HdnUpdateRuleInfoId: null,
    HdnSelectedAttributeNameId: null,
    ModalDerivedWarningId: null,
    BtnDerivedWarningYesId: null,
    BtnDerivedWarningNoId: null,
    HdnDerivedAttributesId: null,
    PanelToDisplayRuleCreationId: null,
    XlGridAttributeRulesId: null,
    HdnRuleInfoForLoggingId: null,
    CpeGridAttributeRulesId: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationPerAttributeControls

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationPerAttributeControls = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls(controlInfo) {
    /// <param name="controlInfo" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationPerAttributeControlInfo">
    /// </param>
    /// <field name="_controlInfo" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationPerAttributeControlInfo">
    /// </field>
    /// <field name="_btnDerivedWarningYes" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnDerivedWarningNo" type="Object" domElement="true">
    /// </field>
    /// <field name="_hdnSelectedAttributeName" type="Object" domElement="true">
    /// </field>
    /// <field name="_hdnDerivedAttributes" type="Object" domElement="true">
    /// </field>
    /// <field name="_hdnUpdateRuleInfo" type="Object" domElement="true">
    /// </field>
    /// <field name="_hdnPriorityInfo" type="Object" domElement="true">
    /// </field>
    /// <field name="_hdnRuleNameInfo" type="Object" domElement="true">
    /// </field>
    /// <field name="_hdnRuleInfo" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblSwitchState" type="Object" domElement="true">
    /// </field>
    /// <field name="_divErrorSaveRule" type="Object" domElement="true">
    /// </field>
    /// <field name="_divErrorSavePriority" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblRuleToDelete" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblRuleNameSwitch" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblSuccess" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblError" type="Object" domElement="true">
    /// </field>
    /// <field name="_txtPriority" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnPrioritySave" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnDeleteYES" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnDeleteNO" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnSuccess" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnError" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnSwitchYes" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnSwitchNo" type="Object" domElement="true">
    /// </field>
    /// <field name="_gridAttributeRules" type="Object" domElement="true">
    /// </field>
    /// <field name="_divRuleCreation" type="Object" domElement="true">
    /// </field>
    /// <field name="_divAttributeRules" type="Object" domElement="true">
    /// </field>
    /// <field name="_txtRuleName" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnSaveRule" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnCancel" type="Object" domElement="true">
    /// </field>
    /// <field name="_xlGridAttributeRules" type="Object" domElement="true">
    /// </field>
    /// <field name="_hdnRuleInfoForLogging" type="Object" domElement="true">
    /// </field>
    this._controlInfo = controlInfo;
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationPerAttributeControls.prototype = {
    _controlInfo: null,
    _btnDerivedWarningYes: null,
    
    get_btnDerivedWarningYes: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_btnDerivedWarningYes() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnDerivedWarningYes == null) {
            this._btnDerivedWarningYes = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnDerivedWarningYesId);
        }
        return this._btnDerivedWarningYes;
    },
    
    _btnDerivedWarningNo: null,
    
    get_btnDerivedWarningNo: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_btnDerivedWarningNo() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnDerivedWarningNo == null) {
            this._btnDerivedWarningNo = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnDerivedWarningNoId);
        }
        return this._btnDerivedWarningNo;
    },
    
    _hdnSelectedAttributeName: null,
    
    get_hdnSelectedAttributeName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_hdnSelectedAttributeName() {
        /// <value type="Object" domElement="true"></value>
        if (this._hdnSelectedAttributeName == null) {
            this._hdnSelectedAttributeName = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HdnSelectedAttributeNameId);
        }
        return this._hdnSelectedAttributeName;
    },
    
    _hdnDerivedAttributes: null,
    
    get_hdnDerivedAttributes: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_hdnDerivedAttributes() {
        /// <value type="Object" domElement="true"></value>
        if (this._hdnDerivedAttributes == null) {
            this._hdnDerivedAttributes = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HdnDerivedAttributesId);
        }
        return this._hdnDerivedAttributes;
    },
    
    _hdnUpdateRuleInfo: null,
    
    get_hdnUpdateRuleInfo: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_hdnUpdateRuleInfo() {
        /// <value type="Object" domElement="true"></value>
        if (this._hdnUpdateRuleInfo == null) {
            this._hdnUpdateRuleInfo = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HdnUpdateRuleInfoId);
        }
        return this._hdnUpdateRuleInfo;
    },
    
    _hdnPriorityInfo: null,
    
    get_hdnPriorityInfo: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_hdnPriorityInfo() {
        /// <value type="Object" domElement="true"></value>
        if (this._hdnPriorityInfo == null) {
            this._hdnPriorityInfo = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HdnPriorityInfoId);
        }
        return this._hdnPriorityInfo;
    },
    
    _hdnRuleNameInfo: null,
    
    get_hdnRuleNameInfo: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_hdnRuleNameInfo() {
        /// <value type="Object" domElement="true"></value>
        if (this._hdnRuleNameInfo == null) {
            this._hdnRuleNameInfo = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HdnRuleNameInfoId);
        }
        return this._hdnRuleNameInfo;
    },
    
    _hdnRuleInfo: null,
    
    get_hdnRuleInfo: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_hdnRuleInfo() {
        /// <value type="Object" domElement="true"></value>
        if (this._hdnRuleInfo == null) {
            this._hdnRuleInfo = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HdnRuleInfoId);
        }
        return this._hdnRuleInfo;
    },
    
    _lblSwitchState: null,
    
    get_lblSwitchState: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_lblSwitchState() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblSwitchState == null) {
            this._lblSwitchState = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblSwitchStateId);
        }
        return this._lblSwitchState;
    },
    
    _divErrorSaveRule: null,
    
    get_divErrorSaveRule: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_divErrorSaveRule() {
        /// <value type="Object" domElement="true"></value>
        if (this._divErrorSaveRule == null) {
            this._divErrorSaveRule = com.ivp.rad.rscriptutils.RSValidators.getObject('divErrorSaveRule');
        }
        return this._divErrorSaveRule;
    },
    
    _divErrorSavePriority: null,
    
    get_divErrorSavePriority: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_divErrorSavePriority() {
        /// <value type="Object" domElement="true"></value>
        if (this._divErrorSavePriority == null) {
            this._divErrorSavePriority = com.ivp.rad.rscriptutils.RSValidators.getObject('divErrorSavePriority');
        }
        return this._divErrorSavePriority;
    },
    
    _lblRuleToDelete: null,
    
    get_lblRuleToDelete: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_lblRuleToDelete() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblRuleToDelete == null) {
            this._lblRuleToDelete = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblRuleToDeleteId);
        }
        return this._lblRuleToDelete;
    },
    
    _lblRuleNameSwitch: null,
    
    get_lblRuleNameSwitch: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_lblRuleNameSwitch() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblRuleNameSwitch == null) {
            this._lblRuleNameSwitch = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblRuleNameSwitchId);
        }
        return this._lblRuleNameSwitch;
    },
    
    _lblSuccess: null,
    
    get_lblSuccess: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_lblSuccess() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblSuccess == null) {
            this._lblSuccess = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblSuccessId);
        }
        return this._lblSuccess;
    },
    
    _lblError: null,
    
    get_lblError: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_lblError() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblError == null) {
            this._lblError = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblErrorId);
        }
        return this._lblError;
    },
    
    _txtPriority: null,
    
    get_txtPriority: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_txtPriority() {
        /// <value type="Object" domElement="true"></value>
        if (this._txtPriority == null) {
            this._txtPriority = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.TxtPriorityId);
        }
        return this._txtPriority;
    },
    
    _btnPrioritySave: null,
    
    get_btnPrioritySave: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_btnPrioritySave() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnPrioritySave == null) {
            this._btnPrioritySave = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnPrioritySaveId);
        }
        return this._btnPrioritySave;
    },
    
    _btnDeleteYES: null,
    
    get_btnDeleteYES: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_btnDeleteYES() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnDeleteYES == null) {
            this._btnDeleteYES = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnDeleteYESId);
        }
        return this._btnDeleteYES;
    },
    
    _btnDeleteNO: null,
    
    get_btnDeleteNO: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_btnDeleteNO() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnDeleteNO == null) {
            this._btnDeleteNO = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnDeleteNOId);
        }
        return this._btnDeleteNO;
    },
    
    _btnSuccess: null,
    
    get_btnSuccess: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_btnSuccess() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnSuccess == null) {
            this._btnSuccess = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnSuccessId);
        }
        return this._btnSuccess;
    },
    
    _btnError: null,
    
    get_btnError: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_btnError() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnError == null) {
            this._btnError = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnErrorId);
        }
        return this._btnError;
    },
    
    _btnSwitchYes: null,
    
    get_btnSwitchYes: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_btnSwitchYes() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnSwitchYes == null) {
            this._btnSwitchYes = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnSwitchYesId);
        }
        return this._btnSwitchYes;
    },
    
    _btnSwitchNo: null,
    
    get_btnSwitchNo: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_btnSwitchNo() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnSwitchNo == null) {
            this._btnSwitchNo = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnSwitchNoId);
        }
        return this._btnSwitchNo;
    },
    
    _gridAttributeRules: null,
    
    get_gridAttributeRules: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_gridAttributeRules() {
        /// <value type="Object" domElement="true"></value>
        if (this._gridAttributeRules == null) {
            this._gridAttributeRules = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.GridAttributeRulesId);
        }
        return this._gridAttributeRules;
    },
    
    _divRuleCreation: null,
    
    get_divRuleCreation: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_divRuleCreation() {
        /// <value type="Object" domElement="true"></value>
        if (this._divRuleCreation == null) {
            this._divRuleCreation = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.DivRuleCreationId);
        }
        return this._divRuleCreation;
    },
    
    _divAttributeRules: null,
    
    get_divAttributeRules: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_divAttributeRules() {
        /// <value type="Object" domElement="true"></value>
        if (this._divAttributeRules == null) {
            this._divAttributeRules = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.DivAttributeRulesId);
        }
        return this._divAttributeRules;
    },
    
    _txtRuleName: null,
    
    get_txtRuleName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_txtRuleName() {
        /// <value type="Object" domElement="true"></value>
        if (this._txtRuleName == null) {
            this._txtRuleName = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.TxtRuleNameId);
        }
        return this._txtRuleName;
    },
    
    _btnSaveRule: null,
    
    get_btnSaveRule: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_btnSaveRule() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnSaveRule == null) {
            this._btnSaveRule = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnSaveRuleId);
        }
        return this._btnSaveRule;
    },
    
    _btnCancel: null,
    
    get_btnCancel: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_btnCancel() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnCancel == null) {
            this._btnCancel = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnCancelId);
        }
        return this._btnCancel;
    },
    
    _xlGridAttributeRules: null,
    
    get_xlGridAttributeRules: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_xlGridAttributeRules() {
        /// <value type="Object" domElement="true"></value>
        if (this._xlGridAttributeRules == null) {
            this._xlGridAttributeRules = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.XlGridAttributeRulesId);
        }
        return this._xlGridAttributeRules;
    },
    
    _hdnRuleInfoForLogging: null,
    
    get_hdnRuleInfoForLogging: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleCreationPerAttributeControls$get_hdnRuleInfoForLogging() {
        /// <value type="Object" domElement="true"></value>
        if (this._hdnRuleInfoForLogging == null) {
            this._hdnRuleInfoForLogging = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HdnRuleInfoForLoggingId);
        }
        return this._hdnRuleInfoForLogging;
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSEntitytypeRuleAttributeInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSEntitytypeRuleAttributeInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSEntitytypeRuleAttributeInfo() {
    /// <field name="attributeId" type="Number" integer="true">
    /// </field>
    /// <field name="attributeName" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSEntitytypeRuleAttributeInfo.prototype = {
    attributeId: 0,
    attributeName: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleUINewInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleUINewInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewInfo() {
    /// <field name="IsSingleScreen" type="Boolean">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleUINewInfo.prototype = {
    IsSingleScreen: false
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleUINewControlInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleUINewControlInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControlInfo() {
    /// <field name="NavBtnBackTopId" type="String">
    /// </field>
    /// <field name="NavBtnNextTopId" type="String">
    /// </field>
    /// <field name="ImgBtnHlp1Id" type="String">
    /// </field>
    /// <field name="BtnSaveRuleId" type="String">
    /// </field>
    /// <field name="BtnCancelId" type="String">
    /// </field>
    /// <field name="NavBtnBackBottomId" type="String">
    /// </field>
    /// <field name="NavBtnNextBottomId" type="String">
    /// </field>
    /// <field name="BtnSavePriorityId" type="String">
    /// </field>
    /// <field name="BtnPriorityCancelId" type="String">
    /// </field>
    /// <field name="BtnDeleteYESId" type="String">
    /// </field>
    /// <field name="BtnDeleteNOId" type="String">
    /// </field>
    /// <field name="BtnSwitchYesId" type="String">
    /// </field>
    /// <field name="BtnSwitchNoId" type="String">
    /// </field>
    /// <field name="BtnSuccessId" type="String">
    /// </field>
    /// <field name="BtnErrorId" type="String">
    /// </field>
    /// <field name="HiddenStateId" type="String">
    /// </field>
    /// <field name="HdnPriorityId" type="String">
    /// </field>
    /// <field name="HiddenRuleIDId" type="String">
    /// </field>
    /// <field name="HdnSuccessId" type="String">
    /// </field>
    /// <field name="HiddenRuleSetIDId" type="String">
    /// </field>
    /// <field name="HiddenRuleNameId" type="String">
    /// </field>
    /// <field name="HiddenPriorityId" type="String">
    /// </field>
    /// <field name="HiddenMaxPriorityId" type="String">
    /// </field>
    /// <field name="HiddenExistingPrioritiesId" type="String">
    /// </field>
    /// <field name="LblRuleTitleId" type="String">
    /// </field>
    /// <field name="LblHeadLabel1Id" type="String">
    /// </field>
    /// <field name="LblHeadLabel2Id" type="String">
    /// </field>
    /// <field name="LblHeadLabel3Id" type="String">
    /// </field>
    /// <field name="LblAttributeRulesId" type="String">
    /// </field>
    /// <field name="LblRuleCreationId" type="String">
    /// </field>
    /// <field name="LblRuleNameEditorId" type="String">
    /// </field>
    /// <field name="TxtRuleNameId" type="String">
    /// </field>
    /// <field name="TxtPriorityId" type="String">
    /// </field>
    /// <field name="LblErrorSaveRuleId" type="String">
    /// </field>
    /// <field name="LblRuleToDeleteId" type="String">
    /// </field>
    /// <field name="LblDeleteRefDataId" type="String">
    /// </field>
    /// <field name="LblRuleNameSwitchId" type="String">
    /// </field>
    /// <field name="LblSwitchStateId" type="String">
    /// </field>
    /// <field name="LblSuccessId" type="String">
    /// </field>
    /// <field name="LblErrorId" type="String">
    /// </field>
    /// <field name="GridRulesId" type="String">
    /// </field>
    /// <field name="RADXRuleEditorId" type="String">
    /// </field>
    /// <field name="DivErrorSavePriorityId" type="String">
    /// </field>
    /// <field name="ModalSwitchStateId" type="String">
    /// </field>
    /// <field name="ModalPriorityId" type="String">
    /// </field>
    /// <field name="ModalDeleteId" type="String">
    /// </field>
    /// <field name="ModalMessageSuccessId" type="String">
    /// </field>
    /// <field name="HdnUpdateRuleInfoId" type="String">
    /// </field>
    /// <field name="PanelToDisplayRuleCreationId" type="String">
    /// </field>
    /// <field name="DivErrorSaveRuleId" type="String">
    /// </field>
    /// <field name="HdnPriorityInfoId" type="String">
    /// </field>
    /// <field name="HdnRuleNameInfoId" type="String">
    /// </field>
    /// <field name="HdnRuleInfoId" type="String">
    /// </field>
    /// <field name="HdnErrorId" type="String">
    /// </field>
    /// <field name="ModalErrorId" type="String">
    /// </field>
    /// <field name="LabelToShow" type="String">
    /// </field>
    /// <field name="LabelWithText" type="String">
    /// </field>
    /// <field name="CpeRuleCreationId" type="String">
    /// </field>
    /// <field name="XlGridRuleId" type="String">
    /// </field>
    /// <field name="DivErrorSaveRulePriority" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleUINewControlInfo.prototype = {
    NavBtnBackTopId: null,
    NavBtnNextTopId: null,
    ImgBtnHlp1Id: null,
    BtnSaveRuleId: null,
    BtnCancelId: null,
    NavBtnBackBottomId: null,
    NavBtnNextBottomId: null,
    BtnSavePriorityId: null,
    BtnPriorityCancelId: null,
    BtnDeleteYESId: null,
    BtnDeleteNOId: null,
    BtnSwitchYesId: null,
    BtnSwitchNoId: null,
    BtnSuccessId: null,
    BtnErrorId: null,
    HiddenStateId: null,
    HdnPriorityId: null,
    HiddenRuleIDId: null,
    HdnSuccessId: null,
    HiddenRuleSetIDId: null,
    HiddenRuleNameId: null,
    HiddenPriorityId: null,
    HiddenMaxPriorityId: null,
    HiddenExistingPrioritiesId: null,
    LblRuleTitleId: null,
    LblHeadLabel1Id: null,
    LblHeadLabel2Id: null,
    LblHeadLabel3Id: null,
    LblAttributeRulesId: null,
    LblRuleCreationId: null,
    LblRuleNameEditorId: null,
    TxtRuleNameId: null,
    TxtPriorityId: null,
    LblErrorSaveRuleId: null,
    LblRuleToDeleteId: null,
    LblDeleteRefDataId: null,
    LblRuleNameSwitchId: null,
    LblSwitchStateId: null,
    LblSuccessId: null,
    LblErrorId: null,
    GridRulesId: null,
    RADXRuleEditorId: null,
    DivErrorSavePriorityId: null,
    ModalSwitchStateId: null,
    ModalPriorityId: null,
    ModalDeleteId: null,
    ModalMessageSuccessId: null,
    HdnUpdateRuleInfoId: null,
    PanelToDisplayRuleCreationId: null,
    DivErrorSaveRuleId: null,
    HdnPriorityInfoId: null,
    HdnRuleNameInfoId: null,
    HdnRuleInfoId: null,
    HdnErrorId: null,
    ModalErrorId: null,
    LabelToShow: null,
    LabelWithText: null,
    CpeRuleCreationId: null,
    XlGridRuleId: null,
    DivErrorSaveRulePriority: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleUINewControls

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleUINewControls = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls(controlInfo) {
    /// <param name="controlInfo" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleUINewControlInfo">
    /// </param>
    /// <field name="_controlInfo" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleUINewControlInfo">
    /// </field>
    /// <field name="_navBtnBackTop" type="Object" domElement="true">
    /// </field>
    /// <field name="_navBtnNextTop" type="Object" domElement="true">
    /// </field>
    /// <field name="_imgBtnHlp1" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnSaveRule" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnCancel" type="Object" domElement="true">
    /// </field>
    /// <field name="_navBtnBackBottom" type="Object" domElement="true">
    /// </field>
    /// <field name="_navBtnNextBottom" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnSavePriority" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnPriorityCancel" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnDeleteYES" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnDeleteNO" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnSwitchYes" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnSwitchNo" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnSuccess" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnError" type="Object" domElement="true">
    /// </field>
    /// <field name="_hiddenState" type="Object" domElement="true">
    /// </field>
    /// <field name="_hdnPriority" type="Object" domElement="true">
    /// </field>
    /// <field name="_hiddenRuleID" type="Object" domElement="true">
    /// </field>
    /// <field name="_hdnSuccess" type="Object" domElement="true">
    /// </field>
    /// <field name="_hiddenRuleSetID" type="Object" domElement="true">
    /// </field>
    /// <field name="_hiddenRuleName" type="Object" domElement="true">
    /// </field>
    /// <field name="_hiddenPriority" type="Object" domElement="true">
    /// </field>
    /// <field name="_hiddenMaxPriority" type="Object" domElement="true">
    /// </field>
    /// <field name="_hiddenExistingPriorities" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblRuleTitle" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblHeadLabel1" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblHeadLabel2" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblHeadLabel3" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblAttributeRules" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblRuleCreation" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblRuleNameEditor" type="Object" domElement="true">
    /// </field>
    /// <field name="_txtRuleName" type="Object" domElement="true">
    /// </field>
    /// <field name="_txtPriority" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblErrorSaveRule" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblRuleToDelete" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblDeleteRefData" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblRuleNameSwitch" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblSwitchState" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblSuccess" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblError" type="Object" domElement="true">
    /// </field>
    /// <field name="_gridRules" type="Object" domElement="true">
    /// </field>
    /// <field name="_divErrorSavePriority" type="Object" domElement="true">
    /// </field>
    /// <field name="_hdnUpdateRuleInfo" type="Object" domElement="true">
    /// </field>
    /// <field name="_divErrorSaveRule" type="Object" domElement="true">
    /// </field>
    /// <field name="_divErrorSaveRulePriority" type="Object" domElement="true">
    /// </field>
    /// <field name="_hdnPriorityInfo" type="Object" domElement="true">
    /// </field>
    /// <field name="_hdnRuleNameInfo" type="Object" domElement="true">
    /// </field>
    /// <field name="_hdnRuleInfo" type="Object" domElement="true">
    /// </field>
    /// <field name="_hdnError" type="Object" domElement="true">
    /// </field>
    /// <field name="_cpeRuleCreation" type="RefMasterJSCommon.CollapsiblePanelExtender">
    /// </field>
    /// <field name="_xlGridRules" type="Object" domElement="true">
    /// </field>
    this._controlInfo = controlInfo;
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleUINewControls.prototype = {
    _controlInfo: null,
    _navBtnBackTop: null,
    
    get_navBtnBackTop: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_navBtnBackTop() {
        /// <value type="Object" domElement="true"></value>
        if (this._navBtnBackTop == null) {
            this._navBtnBackTop = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.NavBtnBackTopId);
        }
        return this._navBtnBackTop;
    },
    
    _navBtnNextTop: null,
    
    get_navBtnNextTop: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_navBtnNextTop() {
        /// <value type="Object" domElement="true"></value>
        if (this._navBtnNextTop == null) {
            this._navBtnNextTop = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.NavBtnNextTopId);
        }
        return this._navBtnNextTop;
    },
    
    _imgBtnHlp1: null,
    
    get_imgBtnHlp1: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_imgBtnHlp1() {
        /// <value type="Object" domElement="true"></value>
        if (this._imgBtnHlp1 == null) {
            this._imgBtnHlp1 = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.ImgBtnHlp1Id);
        }
        return this._imgBtnHlp1;
    },
    
    _btnSaveRule: null,
    
    get_btnSaveRule: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_btnSaveRule() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnSaveRule == null) {
            this._btnSaveRule = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnSaveRuleId);
        }
        return this._btnSaveRule;
    },
    
    _btnCancel: null,
    
    get_btnCancel: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_btnCancel() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnCancel == null) {
            this._btnCancel = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnCancelId);
        }
        return this._btnCancel;
    },
    
    _navBtnBackBottom: null,
    
    get_navBtnBackBottom: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_navBtnBackBottom() {
        /// <value type="Object" domElement="true"></value>
        if (this._navBtnBackBottom == null) {
            this._navBtnBackBottom = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.NavBtnBackBottomId);
        }
        return this._navBtnBackBottom;
    },
    
    _navBtnNextBottom: null,
    
    get_navBtnNextBottom: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_navBtnNextBottom() {
        /// <value type="Object" domElement="true"></value>
        if (this._navBtnNextBottom == null) {
            this._navBtnNextBottom = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.NavBtnNextBottomId);
        }
        return this._navBtnNextBottom;
    },
    
    _btnSavePriority: null,
    
    get_btnSavePriority: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_btnSavePriority() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnSavePriority == null) {
            this._btnSavePriority = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnSavePriorityId);
        }
        return this._btnSavePriority;
    },
    
    _btnPriorityCancel: null,
    
    get_btnPriorityCancel: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_btnPriorityCancel() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnPriorityCancel == null) {
            this._btnPriorityCancel = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnPriorityCancelId);
        }
        return this._btnPriorityCancel;
    },
    
    _btnDeleteYES: null,
    
    get_btnDeleteYES: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_btnDeleteYES() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnDeleteYES == null) {
            this._btnDeleteYES = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnDeleteYESId);
        }
        return this._btnDeleteYES;
    },
    
    _btnDeleteNO: null,
    
    get_btnDeleteNO: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_btnDeleteNO() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnDeleteNO == null) {
            this._btnDeleteNO = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnDeleteNOId);
        }
        return this._btnDeleteNO;
    },
    
    _btnSwitchYes: null,
    
    get_btnSwitchYes: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_btnSwitchYes() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnSwitchYes == null) {
            this._btnSwitchYes = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnSwitchYesId);
        }
        return this._btnSwitchYes;
    },
    
    _btnSwitchNo: null,
    
    get_btnSwitchNo: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_btnSwitchNo() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnSwitchNo == null) {
            this._btnSwitchNo = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnSwitchNoId);
        }
        return this._btnSwitchNo;
    },
    
    _btnSuccess: null,
    
    get_btnSuccess: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_btnSuccess() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnSuccess == null) {
            this._btnSuccess = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnSuccessId);
        }
        return this._btnSuccess;
    },
    
    _btnError: null,
    
    get_btnError: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_btnError() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnError == null) {
            this._btnError = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.BtnErrorId);
        }
        return this._btnError;
    },
    
    _hiddenState: null,
    
    get_hiddenState: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_hiddenState() {
        /// <value type="Object" domElement="true"></value>
        if (this._hiddenState == null) {
            this._hiddenState = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HiddenStateId);
        }
        return this._hiddenState;
    },
    
    _hdnPriority: null,
    
    get_hdnPriority: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_hdnPriority() {
        /// <value type="Object" domElement="true"></value>
        if (this._hdnPriority == null) {
            this._hdnPriority = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HdnPriorityId);
        }
        return this._hdnPriority;
    },
    
    _hiddenRuleID: null,
    
    get_hiddenRuleID: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_hiddenRuleID() {
        /// <value type="Object" domElement="true"></value>
        if (this._hiddenRuleID == null) {
            this._hiddenRuleID = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HiddenRuleIDId);
        }
        return this._hiddenRuleID;
    },
    
    _hdnSuccess: null,
    
    get_hdnSuccess: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_hdnSuccess() {
        /// <value type="Object" domElement="true"></value>
        if (this._hdnSuccess == null) {
            this._hdnSuccess = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HdnSuccessId);
        }
        return this._hdnSuccess;
    },
    
    _hiddenRuleSetID: null,
    
    get_hiddenRuleSetID: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_hiddenRuleSetID() {
        /// <value type="Object" domElement="true"></value>
        if (this._hiddenRuleSetID == null) {
            this._hiddenRuleSetID = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HiddenRuleSetIDId);
        }
        return this._hiddenRuleSetID;
    },
    
    _hiddenRuleName: null,
    
    get_hiddenRuleName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_hiddenRuleName() {
        /// <value type="Object" domElement="true"></value>
        if (this._hiddenRuleName == null) {
            this._hiddenRuleName = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HiddenRuleNameId);
        }
        return this._hiddenRuleName;
    },
    
    _hiddenPriority: null,
    
    get_hiddenPriority: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_hiddenPriority() {
        /// <value type="Object" domElement="true"></value>
        if (this._hiddenPriority == null) {
            this._hiddenPriority = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HiddenPriorityId);
        }
        return this._hiddenPriority;
    },
    
    _hiddenMaxPriority: null,
    
    get_hiddenMaxPriority: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_hiddenMaxPriority() {
        /// <value type="Object" domElement="true"></value>
        if (this._hiddenMaxPriority == null) {
            this._hiddenMaxPriority = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HiddenMaxPriorityId);
        }
        return this._hiddenMaxPriority;
    },
    
    _hiddenExistingPriorities: null,
    
    get_hiddenExistingPriorities: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_hiddenExistingPriorities() {
        /// <value type="Object" domElement="true"></value>
        if (this._hiddenExistingPriorities == null) {
            this._hiddenExistingPriorities = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HiddenExistingPrioritiesId);
        }
        return this._hiddenExistingPriorities;
    },
    
    _lblRuleTitle: null,
    
    get_lblRuleTitle: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_lblRuleTitle() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblRuleTitle == null) {
            this._lblRuleTitle = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblRuleTitleId);
        }
        return this._lblRuleTitle;
    },
    
    _lblHeadLabel1: null,
    
    get_lblHeadLabel1: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_lblHeadLabel1() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblHeadLabel1 == null) {
            this._lblHeadLabel1 = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblHeadLabel1Id);
        }
        return this._lblHeadLabel1;
    },
    
    _lblHeadLabel2: null,
    
    get_lblHeadLabel2: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_lblHeadLabel2() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblHeadLabel2 == null) {
            this._lblHeadLabel2 = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblHeadLabel2Id);
        }
        return this._lblHeadLabel2;
    },
    
    _lblHeadLabel3: null,
    
    get_lblHeadLabel3: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_lblHeadLabel3() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblHeadLabel3 == null) {
            this._lblHeadLabel3 = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblHeadLabel3Id);
        }
        return this._lblHeadLabel3;
    },
    
    _lblAttributeRules: null,
    
    get_lblAttributeRules: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_lblAttributeRules() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblAttributeRules == null) {
            this._lblAttributeRules = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblAttributeRulesId);
        }
        return this._lblAttributeRules;
    },
    
    _lblRuleCreation: null,
    
    get_lblRuleCreation: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_lblRuleCreation() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblRuleCreation == null) {
            this._lblRuleCreation = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblRuleCreationId);
        }
        return this._lblRuleCreation;
    },
    
    _lblRuleNameEditor: null,
    
    get_lblRuleNameEditor: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_lblRuleNameEditor() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblRuleNameEditor == null) {
            this._lblRuleNameEditor = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblRuleNameEditorId);
        }
        return this._lblRuleNameEditor;
    },
    
    _txtRuleName: null,
    
    get_txtRuleName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_txtRuleName() {
        /// <value type="Object" domElement="true"></value>
        if (this._txtRuleName == null) {
            this._txtRuleName = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.TxtRuleNameId);
        }
        return this._txtRuleName;
    },
    
    _txtPriority: null,
    
    get_txtPriority: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_txtPriority() {
        /// <value type="Object" domElement="true"></value>
        if (this._txtPriority == null) {
            this._txtPriority = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.TxtPriorityId);
        }
        return this._txtPriority;
    },
    
    _lblErrorSaveRule: null,
    
    get_lblErrorSaveRule: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_lblErrorSaveRule() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblErrorSaveRule == null) {
            this._lblErrorSaveRule = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblErrorSaveRuleId);
        }
        return this._lblErrorSaveRule;
    },
    
    _lblRuleToDelete: null,
    
    get_lblRuleToDelete: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_lblRuleToDelete() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblRuleToDelete == null) {
            this._lblRuleToDelete = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblRuleToDeleteId);
        }
        return this._lblRuleToDelete;
    },
    
    _lblDeleteRefData: null,
    
    get_lblDeleteRefData: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_lblDeleteRefData() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblDeleteRefData == null) {
            this._lblDeleteRefData = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblDeleteRefDataId);
        }
        return this._lblDeleteRefData;
    },
    
    _lblRuleNameSwitch: null,
    
    get_lblRuleNameSwitch: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_lblRuleNameSwitch() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblRuleNameSwitch == null) {
            this._lblRuleNameSwitch = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblRuleNameSwitchId);
        }
        return this._lblRuleNameSwitch;
    },
    
    _lblSwitchState: null,
    
    get_lblSwitchState: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_lblSwitchState() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblSwitchState == null) {
            this._lblSwitchState = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblSwitchStateId);
        }
        return this._lblSwitchState;
    },
    
    _lblSuccess: null,
    
    get_lblSuccess: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_lblSuccess() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblSuccess == null) {
            this._lblSuccess = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblSuccessId);
        }
        return this._lblSuccess;
    },
    
    _lblError: null,
    
    get_lblError: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_lblError() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblError == null) {
            this._lblError = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.LblErrorId);
        }
        return this._lblError;
    },
    
    _gridRules: null,
    
    get_gridRules: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_gridRules() {
        /// <value type="Object" domElement="true"></value>
        if (this._gridRules == null) {
            this._gridRules = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.GridRulesId);
        }
        return this._gridRules;
    },
    
    _divErrorSavePriority: null,
    
    get_divErrorSavePriority: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_divErrorSavePriority() {
        /// <value type="Object" domElement="true"></value>
        if (this._divErrorSavePriority == null) {
            this._divErrorSavePriority = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.DivErrorSavePriorityId);
        }
        return this._divErrorSavePriority;
    },
    
    _hdnUpdateRuleInfo: null,
    
    get_hdnUpdateRuleInfo: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_hdnUpdateRuleInfo() {
        /// <value type="Object" domElement="true"></value>
        if (this._hdnUpdateRuleInfo == null) {
            this._hdnUpdateRuleInfo = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HdnUpdateRuleInfoId);
        }
        return this._hdnUpdateRuleInfo;
    },
    
    _divErrorSaveRule: null,
    
    get_divErrorSaveRule: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_divErrorSaveRule() {
        /// <value type="Object" domElement="true"></value>
        if (this._divErrorSaveRule == null) {
            this._divErrorSaveRule = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.DivErrorSaveRuleId);
        }
        return this._divErrorSaveRule;
    },
    
    _divErrorSaveRulePriority: null,
    
    get_divErrorSaveRulePriority: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_divErrorSaveRulePriority() {
        /// <value type="Object" domElement="true"></value>
        if (this._divErrorSaveRulePriority == null) {
            this._divErrorSaveRulePriority = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.DivErrorSaveRulePriority);
        }
        return this._divErrorSaveRulePriority;
    },
    
    _hdnPriorityInfo: null,
    
    get_hdnPriorityInfo: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_hdnPriorityInfo() {
        /// <value type="Object" domElement="true"></value>
        if (this._hdnPriorityInfo == null) {
            this._hdnPriorityInfo = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HdnPriorityInfoId);
        }
        return this._hdnPriorityInfo;
    },
    
    _hdnRuleNameInfo: null,
    
    get_hdnRuleNameInfo: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_hdnRuleNameInfo() {
        /// <value type="Object" domElement="true"></value>
        if (this._hdnRuleNameInfo == null) {
            this._hdnRuleNameInfo = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HdnRuleNameInfoId);
        }
        return this._hdnRuleNameInfo;
    },
    
    _hdnRuleInfo: null,
    
    get_hdnRuleInfo: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_hdnRuleInfo() {
        /// <value type="Object" domElement="true"></value>
        if (this._hdnRuleInfo == null) {
            this._hdnRuleInfo = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HdnRuleInfoId);
        }
        return this._hdnRuleInfo;
    },
    
    _hdnError: null,
    
    get_hdnError: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_hdnError() {
        /// <value type="Object" domElement="true"></value>
        if (this._hdnError == null) {
            this._hdnError = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.HdnErrorId);
        }
        return this._hdnError;
    },
    
    _cpeRuleCreation: null,
    
    get_cpeRuleCreation: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_cpeRuleCreation() {
        /// <value type="RefMasterJSCommon.CollapsiblePanelExtender"></value>
        if (this._cpeRuleCreation == null) {
            this._cpeRuleCreation = $find(this._controlInfo.CpeRuleCreationId);
        }
        return this._cpeRuleCreation;
    },
    
    _xlGridRules: null,
    
    get_xlGridRules: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRuleUINewControls$get_xlGridRules() {
        /// <value type="Object" domElement="true"></value>
        if (this._xlGridRules == null) {
            this._xlGridRules = com.ivp.rad.rscriptutils.RSValidators.getObject(this._controlInfo.XlGridRuleId);
        }
        return this._xlGridRules;
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSDataReconciliationInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSDataReconciliationInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationInfo() {
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSReconDataControlIds

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSReconDataControlIds = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSReconDataControlIds() {
    /// <field name="BtnAddReconGroup" type="String">
    /// </field>
    /// <field name="PanelAddReconGroup" type="String">
    /// </field>
    /// <field name="ModalPopupExtenderReconGroup" type="String">
    /// </field>
    /// <field name="BtnOKAddReconGroup" type="String">
    /// </field>
    /// <field name="TxtReconGroupName" type="String">
    /// </field>
    /// <field name="LblReconGroupPopup" type="String">
    /// </field>
    /// <field name="ValGroupError" type="String">
    /// </field>
    /// <field name="LblReconGroupError" type="String">
    /// </field>
    /// <field name="HdnGroupId" type="String">
    /// </field>
    /// <field name="HdnDataField" type="String">
    /// </field>
    /// <field name="BtnUpdateReconGroup" type="String">
    /// </field>
    /// <field name="DdlReconGroup" type="String">
    /// </field>
    /// <field name="HdnReconSetupGridCacheKey" type="String">
    /// </field>
    /// <field name="BtnSaveReconSourceDetails" type="String">
    /// </field>
    /// <field name="BtnResetReconSourceDetails" type="String">
    /// </field>
    /// <field name="DdlEntityType" type="String">
    /// </field>
    /// <field name="DdlReportName" type="String">
    /// </field>
    /// <field name="DdlDataSource" type="String">
    /// </field>
    /// <field name="DdlFeed" type="String">
    /// </field>
    /// <field name="TxtEntityDescription" type="String">
    /// </field>
    /// <field name="PnlReconSourceError" type="String">
    /// </field>
    /// <field name="LblErrSourceDetails" type="String">
    /// </field>
    /// <field name="HdnSessionUser" type="String">
    /// </field>
    /// <field name="BtnShowAttributeMapping" type="String">
    /// </field>
    /// <field name="RdInternalType" type="String">
    /// </field>
    /// <field name="HdnSourceDetailValidation" type="String">
    /// </field>
    /// <field name="BtnAddMapping" type="String">
    /// </field>
    /// <field name="HdnIntDateAttrId" type="String">
    /// </field>
    /// <field name="PanelReconTaskSetup" type="String">
    /// </field>
    /// <field name="PanelSetup" type="String">
    /// </field>
    /// <field name="BtnTaskSave" type="String">
    /// </field>
    /// <field name="BtnTaskReset" type="String">
    /// </field>
    /// <field name="ChkLstBreak" type="String">
    /// </field>
    /// <field name="TxtTaskName" type="String">
    /// </field>
    /// <field name="LblAddSuccess" type="String">
    /// </field>
    /// <field name="BtnAddTask" type="String">
    /// </field>
    /// <field name="PnlErrorSetup" type="String">
    /// </field>
    /// <field name="LblReconSetupError" type="String">
    /// </field>
    /// <field name="HdnGridAction" type="String">
    /// </field>
    /// <field name="HdnGridActionBtn" type="String">
    /// </field>
    /// <field name="HdnGridReconGroupId" type="String">
    /// </field>
    /// <field name="HdnGridReconMappingId" type="String">
    /// </field>
    /// <field name="DdlUniqueAttr" type="String">
    /// </field>
    /// <field name="DivRecon_Accordion" type="String">
    /// </field>
    /// <field name="DivDownstreamSys" type="String">
    /// </field>
    /// <field name="TblReconTaskSetup" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSReconDataControlIds.prototype = {
    BtnAddReconGroup: null,
    PanelAddReconGroup: null,
    ModalPopupExtenderReconGroup: null,
    BtnOKAddReconGroup: null,
    TxtReconGroupName: null,
    LblReconGroupPopup: null,
    ValGroupError: null,
    LblReconGroupError: null,
    HdnGroupId: null,
    HdnDataField: null,
    BtnUpdateReconGroup: null,
    DdlReconGroup: null,
    HdnReconSetupGridCacheKey: null,
    BtnSaveReconSourceDetails: null,
    BtnResetReconSourceDetails: null,
    DdlEntityType: null,
    DdlReportName: null,
    DdlDataSource: null,
    DdlFeed: null,
    TxtEntityDescription: null,
    PnlReconSourceError: null,
    LblErrSourceDetails: null,
    HdnSessionUser: null,
    BtnShowAttributeMapping: null,
    RdInternalType: null,
    HdnSourceDetailValidation: null,
    BtnAddMapping: null,
    HdnIntDateAttrId: null,
    PanelReconTaskSetup: null,
    PanelSetup: null,
    BtnTaskSave: null,
    BtnTaskReset: null,
    ChkLstBreak: null,
    TxtTaskName: null,
    LblAddSuccess: null,
    BtnAddTask: null,
    PnlErrorSetup: null,
    LblReconSetupError: null,
    HdnGridAction: null,
    HdnGridActionBtn: null,
    HdnGridReconGroupId: null,
    HdnGridReconMappingId: null,
    DdlUniqueAttr: null,
    DivRecon_Accordion: null,
    DivDownstreamSys: null,
    TblReconTaskSetup: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSDataReconciliationControls

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSDataReconciliationControls = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls(objReconDataControlIds) {
    /// <param name="objReconDataControlIds" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSReconDataControlIds">
    /// </param>
    /// <field name="_objDataReconciliationControlIds" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSReconDataControlIds">
    /// </field>
    this._objDataReconciliationControlIds = objReconDataControlIds;
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSDataReconciliationControls.prototype = {
    _objDataReconciliationControlIds: null,
    
    get_tblReconTaskSetup: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_tblReconTaskSetup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.TblReconTaskSetup);
    },
    
    get_divDownstreamSys: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_divDownstreamSys() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.DivDownstreamSys);
    },
    
    get_divRecon_Accordion: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_divRecon_Accordion() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.DivRecon_Accordion);
    },
    
    get_ddlUniqueAttr: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_ddlUniqueAttr() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.DdlUniqueAttr);
    },
    
    get_hdnGridReconGroupId: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_hdnGridReconGroupId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.HdnGridReconGroupId);
    },
    
    get_hdnGridReconMappingId: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_hdnGridReconMappingId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.HdnGridReconMappingId);
    },
    
    get_hdnGridAction: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_hdnGridAction() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.HdnGridAction);
    },
    
    get_hdnGridActionBtn: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_hdnGridActionBtn() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.HdnGridActionBtn);
    },
    
    get_lblReconSetupError: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_lblReconSetupError() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.LblReconSetupError);
    },
    
    get_pnlErrorSetup: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_pnlErrorSetup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.PnlErrorSetup);
    },
    
    get_btnAddTask: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_btnAddTask() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.BtnAddTask);
    },
    
    get_lblAddSuccess: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_lblAddSuccess() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.LblAddSuccess);
    },
    
    get_txtTaskName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_txtTaskName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.TxtTaskName);
    },
    
    get_chkLstBreak: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_chkLstBreak() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.ChkLstBreak);
    },
    
    get_btnTaskReset: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_btnTaskReset() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.BtnTaskReset);
    },
    
    get_btnTaskSave: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_btnTaskSave() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.BtnTaskSave);
    },
    
    get_panelSetup: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_panelSetup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.PanelSetup);
    },
    
    get_panelReconTaskSetup: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_panelReconTaskSetup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.PanelReconTaskSetup);
    },
    
    get_hdnIntDateAttrId: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_hdnIntDateAttrId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.HdnIntDateAttrId);
    },
    
    get_btnAddMapping: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_btnAddMapping() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.BtnAddMapping);
    },
    
    get_hdnSourceDetailValidation: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_hdnSourceDetailValidation() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.HdnSourceDetailValidation);
    },
    
    get_rdInternalType: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_rdInternalType() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.RdInternalType);
    },
    
    get_btnShowAttributeMapping: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_btnShowAttributeMapping() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.BtnShowAttributeMapping);
    },
    
    get_hdnSessionUser: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_hdnSessionUser() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.HdnSessionUser);
    },
    
    get_pnlReconSourceError: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_pnlReconSourceError() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.PnlReconSourceError);
    },
    
    get_lblErrSourceDetails: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_lblErrSourceDetails() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.LblErrSourceDetails);
    },
    
    get_txtEntityDescription: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_txtEntityDescription() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.TxtEntityDescription);
    },
    
    get_ddlFeed: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_ddlFeed() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.DdlFeed);
    },
    
    get_ddlDataSource: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_ddlDataSource() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.DdlDataSource);
    },
    
    get_ddlReportName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_ddlReportName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.DdlReportName);
    },
    
    get_ddlEntityType: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_ddlEntityType() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.DdlEntityType);
    },
    
    get_btnSaveReconSourceDetails: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_btnSaveReconSourceDetails() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.BtnSaveReconSourceDetails);
    },
    
    get_btnResetReconSourceDetails: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_btnResetReconSourceDetails() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.BtnResetReconSourceDetails);
    },
    
    get_ddlReconGroup: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_ddlReconGroup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.DdlReconGroup);
    },
    
    get_btnUpdateReconGroup: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_btnUpdateReconGroup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.BtnUpdateReconGroup);
    },
    
    get_hdnDataField: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_hdnDataField() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.HdnDataField);
    },
    
    get_hdnGroupId: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_hdnGroupId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.HdnGroupId);
    },
    
    get_btnAddReconGroup: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_btnAddReconGroup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.BtnAddReconGroup);
    },
    
    get_modalPopupExtenderReconGroup: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_modalPopupExtenderReconGroup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.ModalPopupExtenderReconGroup);
    },
    
    get_btnOKAddReconGroup: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_btnOKAddReconGroup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.BtnOKAddReconGroup);
    },
    
    get_panelAddReconGroup: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_panelAddReconGroup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.PanelAddReconGroup);
    },
    
    get_txtReconGroupName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_txtReconGroupName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.TxtReconGroupName);
    },
    
    get_lblReconGroupPopup: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_lblReconGroupPopup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.LblReconGroupPopup);
    },
    
    get_valGroupError: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_valGroupError() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.ValGroupError);
    },
    
    get_lblReconGroupError: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSDataReconciliationControls$get_lblReconGroupError() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objDataReconciliationControlIds.LblReconGroupError);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSReconGroupInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSReconGroupInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSReconGroupInfo() {
    /// <field name="GroupId" type="Number" integer="true">
    /// </field>
    /// <field name="GroupName" type="String">
    /// </field>
    /// <field name="ReportSystemId" type="Number" integer="true">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSReconGroupInfo.prototype = {
    GroupId: 0,
    GroupName: null,
    ReportSystemId: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReconSourceDetailsInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReconSourceDetailsInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReconSourceDetailsInfo() {
    /// <field name="reconGroupId" type="Number" integer="true">
    /// </field>
    /// <field name="ddlEntityTypeIdSelected" type="Number" integer="true">
    /// </field>
    /// <field name="entityTypeNameSelected" type="String">
    /// </field>
    /// <field name="entityTypeDescriptn" type="String">
    /// </field>
    /// <field name="ddlReportIdSelected" type="Number" integer="true">
    /// </field>
    /// <field name="ddlReportNameSelected" type="String">
    /// </field>
    /// <field name="ddlDataSourceNameSelected" type="String">
    /// </field>
    /// <field name="ddlDataSourceIdSelected" type="Number" integer="true">
    /// </field>
    /// <field name="ddlFeedNameSelected" type="String">
    /// </field>
    /// <field name="ddlFeedIdSelected" type="Number" integer="true">
    /// </field>
    /// <field name="internalSourceDataTypeId" type="Number" integer="true">
    /// </field>
    /// <field name="userName" type="String">
    /// </field>
    /// <field name="ddlUniqueAttributeSelected" type="Number" integer="true">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReconSourceDetailsInfo.prototype = {
    reconGroupId: 0,
    ddlEntityTypeIdSelected: 0,
    entityTypeNameSelected: null,
    entityTypeDescriptn: null,
    ddlReportIdSelected: 0,
    ddlReportNameSelected: null,
    ddlDataSourceNameSelected: null,
    ddlDataSourceIdSelected: 0,
    ddlFeedNameSelected: null,
    ddlFeedIdSelected: 0,
    internalSourceDataTypeId: 0,
    userName: null,
    ddlUniqueAttributeSelected: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReconAttrMappingInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReconAttrMappingInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReconAttrMappingInfo() {
    /// <field name="internal_attribute_id" type="Number" integer="true">
    /// </field>
    /// <field name="external_attribute_id" type="Number" integer="true">
    /// </field>
    /// <field name="internal_attribute_real_name" type="String">
    /// </field>
    /// <field name="external_attribute_real_name" type="String">
    /// </field>
    /// <field name="is_recon_key" type="Boolean">
    /// </field>
    /// <field name="tolerance_type" type="String">
    /// </field>
    /// <field name="tolerance_limit_detail" type="Number" integer="true">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReconAttrMappingInfo.prototype = {
    internal_attribute_id: 0,
    external_attribute_id: 0,
    internal_attribute_real_name: null,
    external_attribute_real_name: null,
    is_recon_key: false,
    tolerance_type: null,
    tolerance_limit_detail: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReconBreakTypeInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReconBreakTypeInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReconBreakTypeInfo() {
    /// <field name="allBreak" type="String">
    /// </field>
    /// <field name="mismatches" type="String">
    /// </field>
    /// <field name="missingInternal" type="String">
    /// </field>
    /// <field name="missingExternal" type="String">
    /// </field>
    /// <field name="missReconKey" type="String">
    /// </field>
    /// <field name="reconTaskName" type="String">
    /// </field>
    /// <field name="reconGroupId" type="Number" integer="true">
    /// </field>
    /// <field name="createdBy" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReconBreakTypeInfo.prototype = {
    allBreak: null,
    mismatches: null,
    missingInternal: null,
    missingExternal: null,
    missReconKey: null,
    reconTaskName: null,
    reconGroupId: 0,
    createdBy: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReportSetupControlsInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReportSetupControlsInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReportSetupControlsInfo() {
    /// <field name="TbEntityTypeId" type="String">
    /// </field>
    /// <field name="RsColumnToShowId" type="String">
    /// </field>
    /// <field name="BtnColumnToShowNextId" type="String">
    /// </field>
    /// <field name="BtnColumnToGroupNextId" type="String">
    /// </field>
    /// <field name="RsColumnToGroupId" type="String">
    /// </field>
    /// <field name="TxtEmailIDId" type="String">
    /// </field>
    /// <field name="BtnSaveId" type="String">
    /// </field>
    /// <field name="HdnEntityAttributesId" type="String">
    /// </field>
    /// <field name="HdnEntitySelectedAttributesId" type="String">
    /// </field>
    /// <field name="HdnEntityGroupAttributesId" type="String">
    /// </field>
    /// <field name="TbSaveId" type="String">
    /// </field>
    /// <field name="HdnReportIDId" type="String">
    /// </field>
    /// <field name="TrGroupHeaderId" type="String">
    /// </field>
    /// <field name="TrGroupNextRowId" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReportSetupControlsInfo.prototype = {
    TbEntityTypeId: null,
    RsColumnToShowId: null,
    BtnColumnToShowNextId: null,
    BtnColumnToGroupNextId: null,
    RsColumnToGroupId: null,
    TxtEmailIDId: null,
    BtnSaveId: null,
    HdnEntityAttributesId: null,
    HdnEntitySelectedAttributesId: null,
    HdnEntityGroupAttributesId: null,
    TbSaveId: null,
    HdnReportIDId: null,
    TrGroupHeaderId: null,
    TrGroupNextRowId: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReportSetupControls

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReportSetupControls = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReportSetupControls(controlIds) {
    /// <param name="controlIds" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReportSetupControlsInfo">
    /// The control ids.
    /// </param>
    /// <field name="_controlIds" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReportSetupControlsInfo">
    /// </field>
    this._controlIds = controlIds;
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReportSetupControls.prototype = {
    _controlIds: null,
    
    get_tbEntityType: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReportSetupControls$get_tbEntityType() {
        /// <summary>
        /// Gets the type of the tb entity.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TbEntityTypeId);
    },
    
    get_rsColumnToShow: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReportSetupControls$get_rsColumnToShow() {
        /// <summary>
        /// Gets the rs column to show.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.RsColumnToShowId);
    },
    
    get_btnColumnToShowNext: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReportSetupControls$get_btnColumnToShowNext() {
        /// <summary>
        /// Gets the BTN column to show next.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnColumnToShowNextId);
    },
    
    get_btnColumnToGroupNext: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReportSetupControls$get_btnColumnToGroupNext() {
        /// <summary>
        /// Gets the BTN column to group next.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnColumnToGroupNextId);
    },
    
    get_rsColumnToGroup: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReportSetupControls$get_rsColumnToGroup() {
        /// <summary>
        /// Gets the rs column to group.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.RsColumnToGroupId);
    },
    
    get_txtEmailID: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReportSetupControls$get_txtEmailID() {
        /// <summary>
        /// Gets the TXT email ID.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtEmailIDId);
    },
    
    get_btnSave: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReportSetupControls$get_btnSave() {
        /// <summary>
        /// Gets the BTN save.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnSaveId);
    },
    
    get_hdnEntityAttributes: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReportSetupControls$get_hdnEntityAttributes() {
        /// <summary>
        /// Gets the HDN entity attributes.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnEntityAttributesId);
    },
    
    get_hdnEntitySelectedAttributes: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReportSetupControls$get_hdnEntitySelectedAttributes() {
        /// <summary>
        /// Gets the HDN entity selected attributes.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnEntitySelectedAttributesId);
    },
    
    get_hdnEntityGroupAttributes: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReportSetupControls$get_hdnEntityGroupAttributes() {
        /// <summary>
        /// Gets the HDN entity group attributes.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnEntityGroupAttributesId);
    },
    
    get_tbSave: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReportSetupControls$get_tbSave() {
        /// <summary>
        /// Gets the tb save.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TbSaveId);
    },
    
    get_hdnReportID: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReportSetupControls$get_hdnReportID() {
        /// <summary>
        /// Gets the HDN report ID.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnReportIDId);
    },
    
    get_divErrorDisplay: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReportSetupControls$get_divErrorDisplay() {
        /// <summary>
        /// Gets the div error display.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('divErrorDisplay');
    },
    
    get_trGroupHeader: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReportSetupControls$get_trGroupHeader() {
        /// <summary>
        /// Gets the tr group header.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TrGroupHeaderId);
    },
    
    get_trGroupNextRow: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMReportSetupControls$get_trGroupNextRow() {
        /// <summary>
        /// Gets the tr group next row.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TrGroupNextRowId);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSBackgroundTaskStatusInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSBackgroundTaskStatusInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSBackgroundTaskStatusInfo() {
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMBackGroundTaskStatusControlIDs

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMBackGroundTaskStatusControlIDs = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMBackGroundTaskStatusControlIDs() {
    /// <field name="RMDateDropDownID" type="String">
    /// </field>
    /// <field name="RMDateDropDownCustomDivID" type="String">
    /// </field>
    /// <field name="RMDateDropDownTextID" type="String">
    /// </field>
    /// <field name="RMDateDropDownSelectID" type="String">
    /// </field>
    /// <field name="RMBackgroundTaskStatusID" type="String">
    /// </field>
    /// <field name="RdBetweenDatesID" type="String">
    /// </field>
    /// <field name="RdFromDateID" type="String">
    /// </field>
    /// <field name="RdPriorToDateID" type="String">
    /// </field>
    /// <field name="TxtStartDateID" type="String">
    /// </field>
    /// <field name="TxtEndDateID" type="String">
    /// </field>
    /// <field name="BtnGetTasks" type="String">
    /// </field>
    /// <field name="DivErrorSummaryID" type="String">
    /// </field>
    /// <field name="LblStartDate" type="String">
    /// </field>
    /// <field name="LblEndDate" type="String">
    /// </field>
    /// <field name="HiddenField" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMBackGroundTaskStatusControlIDs.prototype = {
    RMDateDropDownID: null,
    RMDateDropDownCustomDivID: null,
    RMDateDropDownTextID: null,
    RMDateDropDownSelectID: null,
    RMBackgroundTaskStatusID: null,
    RdBetweenDatesID: null,
    RdFromDateID: null,
    RdPriorToDateID: null,
    TxtStartDateID: null,
    TxtEndDateID: null,
    BtnGetTasks: null,
    DivErrorSummaryID: null,
    LblStartDate: null,
    LblEndDate: null,
    HiddenField: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSBackgroundTaskStatusControls

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSBackgroundTaskStatusControls = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSBackgroundTaskStatusControls(objControlIDs) {
    /// <param name="objControlIDs" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMBackGroundTaskStatusControlIDs">
    /// </param>
    /// <field name="_objTaskStatusControlIDs" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMBackGroundTaskStatusControlIDs">
    /// </field>
    /// <field name="_dateDropDown" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDownCustomDiv" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDownText" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateSelect" type="Object" domElement="true">
    /// </field>
    this._objTaskStatusControlIDs = objControlIDs;
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSBackgroundTaskStatusControls.prototype = {
    _objTaskStatusControlIDs: null,
    _dateDropDown: null,
    _dateDropDownCustomDiv: null,
    _dateDropDownText: null,
    _dateSelect: null,
    
    get_rMdateDropDown: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSBackgroundTaskStatusControls$get_rMdateDropDown() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDown == null) {
            this._dateDropDown = document.getElementById(this._objTaskStatusControlIDs.RMDateDropDownID);
        }
        return this._dateDropDown;
    },
    
    get_rmDateDropDownCustomDiv: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSBackgroundTaskStatusControls$get_rmDateDropDownCustomDiv() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDownCustomDiv == null) {
            this._dateDropDownCustomDiv = document.getElementById(this._objTaskStatusControlIDs.RMDateDropDownCustomDivID);
        }
        return this._dateDropDownCustomDiv;
    },
    
    get_rmDateDropDownText: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSBackgroundTaskStatusControls$get_rmDateDropDownText() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDownText == null) {
            this._dateDropDownText = document.getElementById(this._objTaskStatusControlIDs.RMDateDropDownTextID);
        }
        return this._dateDropDownText;
    },
    
    get_rmDateDropDownSelect: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSBackgroundTaskStatusControls$get_rmDateDropDownSelect() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateSelect == null) {
            this._dateSelect = document.getElementById(this._objTaskStatusControlIDs.RMDateDropDownSelectID);
        }
        return this._dateSelect;
    },
    
    get_rmBackgroundTaskStatus: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSBackgroundTaskStatusControls$get_rmBackgroundTaskStatus() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.RMBackgroundTaskStatusID);
    },
    
    get_rdBetweenDates: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSBackgroundTaskStatusControls$get_rdBetweenDates() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.RdBetweenDatesID);
    },
    
    get_rdFromDate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSBackgroundTaskStatusControls$get_rdFromDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.RdFromDateID);
    },
    
    get_rdPriorToDate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSBackgroundTaskStatusControls$get_rdPriorToDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.RdPriorToDateID);
    },
    
    get_hiddenFilterExpression: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSBackgroundTaskStatusControls$get_hiddenFilterExpression() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.HiddenField);
    },
    
    get_txtStartDate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSBackgroundTaskStatusControls$get_txtStartDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.TxtStartDateID);
    },
    
    get_txtEndDate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSBackgroundTaskStatusControls$get_txtEndDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.TxtEndDateID);
    },
    
    get_lblStartDate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSBackgroundTaskStatusControls$get_lblStartDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.LblStartDate);
    },
    
    get_lblEndDate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSBackgroundTaskStatusControls$get_lblEndDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.LblEndDate);
    },
    
    get_btnGetTasks: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSBackgroundTaskStatusControls$get_btnGetTasks() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.BtnGetTasks);
    },
    
    get_divErrorSummary: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSBackgroundTaskStatusControls$get_divErrorSummary() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.DivErrorSummaryID);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRealTimePreferenceInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRealTimePreferenceInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRealTimePreferenceInfo() {
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMRealTimePreferenceControlIDs

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMRealTimePreferenceControlIDs = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMRealTimePreferenceControlIDs() {
    /// <field name="BtnAddPreference" type="String">
    /// </field>
    /// <field name="BtnUpdatePreference" type="String">
    /// </field>
    /// <field name="DivErrorSummaryID" type="String">
    /// </field>
    /// <field name="BtnAddPreferenceModal" type="String">
    /// </field>
    /// <field name="HiddenFilter" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMRealTimePreferenceControlIDs.prototype = {
    BtnAddPreference: null,
    BtnUpdatePreference: null,
    DivErrorSummaryID: null,
    BtnAddPreferenceModal: null,
    HiddenFilter: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRealTimePreferenceControls

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRealTimePreferenceControls = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRealTimePreferenceControls(objControlIds) {
    /// <param name="objControlIds" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMRealTimePreferenceControlIDs">
    /// </param>
    /// <field name="_objRealTimePrefControlIds" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMRealTimePreferenceControlIDs">
    /// </field>
    this._objRealTimePrefControlIds = objControlIds;
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRealTimePreferenceControls.prototype = {
    _objRealTimePrefControlIds: null,
    
    get_hiddenFilter: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRealTimePreferenceControls$get_hiddenFilter() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objRealTimePrefControlIds.HiddenFilter);
    },
    
    get_btnAddPreference: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRealTimePreferenceControls$get_btnAddPreference() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objRealTimePrefControlIds.BtnAddPreference);
    },
    
    get_btnUpdatePreference: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRealTimePreferenceControls$get_btnUpdatePreference() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objRealTimePrefControlIds.BtnUpdatePreference);
    },
    
    get_btnAddPreferenceModal: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRealTimePreferenceControls$get_btnAddPreferenceModal() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objRealTimePrefControlIds.BtnAddPreferenceModal);
    },
    
    get_divErrorSummary: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRealTimePreferenceControls$get_divErrorSummary() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objRealTimePrefControlIds.DivErrorSummaryID);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRulesControlIds

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRulesControlIds = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControlIds() {
    /// <summary>
    /// Class containing controlIds
    /// </summary>
    /// <field name="ruleEditorId" type="String">
    /// </field>
    /// <field name="BtnCancelId" type="String">
    /// </field>
    /// <field name="BtnSaveRuleId" type="String">
    /// </field>
    /// <field name="TxtRuleNameId" type="String">
    /// </field>
    /// <field name="GridRulesId" type="String">
    /// </field>
    /// <field name="ModalPriority" type="String">
    /// </field>
    /// <field name="TxtMaxPriorityId" type="String">
    /// </field>
    /// <field name="LblRuleNameId" type="String">
    /// </field>
    /// <field name="HdnRuleId" type="String">
    /// </field>
    /// <field name="ModalDelete" type="String">
    /// </field>
    /// <field name="HiddenRuleSetID" type="String">
    /// </field>
    /// <field name="HiddenRuleName" type="String">
    /// </field>
    /// <field name="HiddenAddUpdate" type="String">
    /// </field>
    /// <field name="ModalStateChangeConfirm" type="String">
    /// </field>
    /// <field name="ModalSwitchState" type="String">
    /// </field>
    /// <field name="BtnCommitId" type="String">
    /// </field>
    /// <field name="ValPriorityId" type="String">
    /// </field>
    /// <field name="hiddenSwitchTo" type="String">
    /// </field>
    /// <field name="TableRuleEditor" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRulesControlIds.prototype = {
    ruleEditorId: null,
    BtnCancelId: null,
    BtnSaveRuleId: null,
    TxtRuleNameId: null,
    GridRulesId: null,
    ModalPriority: null,
    TxtMaxPriorityId: null,
    LblRuleNameId: null,
    HdnRuleId: null,
    ModalDelete: null,
    HiddenRuleSetID: null,
    HiddenRuleName: null,
    HiddenAddUpdate: null,
    ModalStateChangeConfirm: null,
    ModalSwitchState: null,
    BtnCommitId: null,
    ValPriorityId: null,
    hiddenSwitchTo: null,
    TableRuleEditor: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRulesControls

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRulesControls = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls(controlIds) {
    /// <summary>
    /// Class containing Rules Controls
    /// </summary>
    /// <param name="controlIds" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRulesControlIds">
    /// The control ids.
    /// </param>
    /// <field name="_controlIds" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRulesControlIds">
    /// </field>
    this._controlIds = controlIds;
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRulesControls.prototype = {
    _controlIds: null,
    
    get_rRuleEditor: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_rRuleEditor() {
        /// <summary>
        /// Gets the R rule editor.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.ruleEditorId);
    },
    
    get_btnCancel: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_btnCancel() {
        /// <summary>
        /// Gets the BTN cancel.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnCancelId);
    },
    
    get_btnSaveRule: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_btnSaveRule() {
        /// <summary>
        /// Gets the BTN save rule.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnSaveRuleId);
    },
    
    get_txtRuleName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_txtRuleName() {
        /// <summary>
        /// Gets the name of the TXT rule.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtRuleNameId);
    },
    
    get_gridRules: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_gridRules() {
        /// <summary>
        /// Gets the grid rules.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.GridRulesId);
    },
    
    get_btnShowRuleEditor: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_btnShowRuleEditor() {
        /// <summary>
        /// Gets the BTN show rule editor.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnShowRuleEditor');
    },
    
    get_errorRuleName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_errorRuleName() {
        /// <summary>
        /// Gets the name of the error rule.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('errorRuleName');
    },
    
    get_txtMaxPriority: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_txtMaxPriority() {
        /// <summary>
        /// Gets the TXT max priority.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtMaxPriorityId);
    },
    
    get_lblRuleName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_lblRuleName() {
        /// <summary>
        /// Gets the name of the LBL rule.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.LblRuleNameId);
    },
    
    get_hdnRuleId: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_hdnRuleId() {
        /// <summary>
        /// Gets the HDN rule id.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnRuleId);
    },
    
    get_lblRuleNameToDelete: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_lblRuleNameToDelete() {
        /// <summary>
        /// Gets the LBL rule name to delete.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblRuleNameToDelete');
    },
    
    get_hiddenRuleSetID: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_hiddenRuleSetID() {
        /// <summary>
        /// Gets the hidden rule set ID.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HiddenRuleSetID);
    },
    
    get_hiddenRuleName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_hiddenRuleName() {
        /// <summary>
        /// Gets the name of the hidden rule.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HiddenRuleName);
    },
    
    get_hiddenAddUpdate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_hiddenAddUpdate() {
        /// <summary>
        /// Gets the hidden add update.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HiddenAddUpdate);
    },
    
    get_btnYes: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_btnYes() {
        /// <summary>
        /// Gets the BTN yes.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnYes');
    },
    
    get_lblRuleNameSwitch: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_lblRuleNameSwitch() {
        /// <summary>
        /// Gets the LBL rule name switch.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblRuleNameSwitch');
    },
    
    get_lblSwitchState: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_lblSwitchState() {
        /// <summary>
        /// Gets the state of the LBL switch.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblSwitchState');
    },
    
    get_btnCommit: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_btnCommit() {
        /// <summary>
        /// Gets the BTN commit.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnCommitId);
    },
    
    get_valPriority: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_valPriority() {
        /// <summary>
        /// Gets the val priority.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.ValPriorityId);
    },
    
    get_hiddenSwitchTo: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_hiddenSwitchTo() {
        /// <summary>
        /// Gets the BTN switch yes.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.hiddenSwitchTo);
    },
    
    get_tableRuleEditor: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSRulesControls$get_tableRuleEditor() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TableRuleEditor);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchADVDataInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchADVDataInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchADVDataInfo() {
    /// <field name="SearchData" type="String">
    /// </field>
    /// <field name="EntityTypeId" type="Number" integer="true">
    /// </field>
    /// <field name="IsString" type="Boolean">
    /// </field>
    /// <field name="IsDateTime" type="Boolean">
    /// </field>
    /// <field name="IsDecimal" type="Boolean">
    /// </field>
    this.SearchData = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchADVDataInfo.prototype = {
    EntityTypeId: 0,
    IsString: false,
    IsDateTime: false,
    IsDecimal: false
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchADVDataControlIds

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchADVDataControlIds = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchADVDataControlIds() {
    /// <field name="GridSearchDataId" type="String">
    /// </field>
    /// <field name="PanelGridSearchId" type="String">
    /// </field>
    /// <field name="CtmGridSearchId" type="String">
    /// </field>
    /// <field name="EntityDetailsAsOfDate" type="String">
    /// </field>
    /// <field name="BtnGetEntityDetailsAsOf" type="String">
    /// </field>
    /// <field name="EntityDetailAsOfKnowledgeDate" type="String">
    /// </field>
    /// <field name="HdnSearchGridCacheKey" type="String">
    /// </field>
    /// <field name="PopupEntityDetailsAsOf" type="String">
    /// </field>
    /// <field name="HdnEntityTypeId" type="String">
    /// </field>
    /// <field name="LblDeleteTask" type="String">
    /// </field>
    /// <field name="HdnEntityDetailsAsOfDate" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchADVDataControlIds.prototype = {
    GridSearchDataId: null,
    PanelGridSearchId: null,
    CtmGridSearchId: null,
    EntityDetailsAsOfDate: null,
    BtnGetEntityDetailsAsOf: null,
    EntityDetailAsOfKnowledgeDate: null,
    HdnSearchGridCacheKey: null,
    PopupEntityDetailsAsOf: null,
    HdnEntityTypeId: null,
    LblDeleteTask: null,
    HdnEntityDetailsAsOfDate: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchADVDataControls

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchADVDataControls = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchADVDataControls(objSearchDataControlIds) {
    /// <param name="objSearchDataControlIds" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchADVDataControlIds">
    /// </param>
    /// <field name="_objSearchDataControlIds" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchADVDataControlIds">
    /// </field>
    this._objSearchDataControlIds = objSearchDataControlIds;
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchADVDataControls.prototype = {
    _objSearchDataControlIds: null,
    
    get_gridSearchData: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchADVDataControls$get_gridSearchData() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.GridSearchDataId + '_bodyDiv_Table');
    },
    
    get_panelGridSearch: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchADVDataControls$get_panelGridSearch() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.PanelGridSearchId);
    },
    
    get_ctmGridSearch: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchADVDataControls$get_ctmGridSearch() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.CtmGridSearchId + '_Root_0');
    },
    
    get_divErrorDisplay: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchADVDataControls$get_divErrorDisplay() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('divErrorDisplay');
    },
    
    get_popupEntityDetailsAsOf: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchADVDataControls$get_popupEntityDetailsAsOf() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.PopupEntityDetailsAsOf);
    },
    
    get_entityDetailsAsOfDate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchADVDataControls$get_entityDetailsAsOfDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.EntityDetailsAsOfDate);
    },
    
    get_entityDetailAsOfKnowledgeDate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchADVDataControls$get_entityDetailAsOfKnowledgeDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.EntityDetailAsOfKnowledgeDate);
    },
    
    get_btnGetEntityDetailsAsOf: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchADVDataControls$get_btnGetEntityDetailsAsOf() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.BtnGetEntityDetailsAsOf);
    },
    
    get_hdnSearchGridCacheKey: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchADVDataControls$get_hdnSearchGridCacheKey() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.HdnSearchGridCacheKey);
    },
    
    get_hdnEntityTypeId: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchADVDataControls$get_hdnEntityTypeId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.HdnEntityTypeId);
    },
    
    get_lblDeleteTask: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchADVDataControls$get_lblDeleteTask() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.LblDeleteTask);
    },
    
    get_hdnEntityDetailsAsOfDate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchADVDataControls$get_hdnEntityDetailsAsOfDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.HdnEntityDetailsAsOfDate);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchDataInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchDataInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataInfo() {
    /// <field name="SearchData" type="String">
    /// </field>
    /// <field name="EntityTypeId" type="Number" integer="true">
    /// </field>
    /// <field name="IsString" type="Boolean">
    /// </field>
    /// <field name="IsDateTime" type="Boolean">
    /// </field>
    /// <field name="IsDecimal" type="Boolean">
    /// </field>
    this.SearchData = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchDataInfo.prototype = {
    EntityTypeId: 0,
    IsString: false,
    IsDateTime: false,
    IsDecimal: false
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchDataControlIds

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchDataControlIds = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControlIds() {
    /// <field name="DDLEntityTypeId" type="String">
    /// </field>
    /// <field name="DDLEntityGroupId" type="String">
    /// </field>
    /// <field name="DDLRdStateId" type="String">
    /// </field>
    /// <field name="BtnSearchDataId" type="String">
    /// </field>
    /// <field name="BtnBrowseDataId" type="String">
    /// </field>
    /// <field name="TxtSearchDataId" type="String">
    /// </field>
    /// <field name="GridSearchDataId" type="String">
    /// </field>
    /// <field name="PanelGridSearchId" type="String">
    /// </field>
    /// <field name="CtmGridSearchId" type="String">
    /// </field>
    /// <field name="PanelEditDataId" type="String">
    /// </field>
    /// <field name="EntityDetailsAsOfDate" type="String">
    /// </field>
    /// <field name="BtnGetEntityDetailsAsOf" type="String">
    /// </field>
    /// <field name="LblDeleteTask" type="String">
    /// </field>
    /// <field name="EntityDetailAsOfKnowledgeDate" type="String">
    /// </field>
    /// <field name="HdnEntityActionType" type="String">
    /// </field>
    /// <field name="HdnClickedEntityCode" type="String">
    /// </field>
    /// <field name="HdnSearchGridCacheKey" type="String">
    /// </field>
    /// <field name="HdnEntityCodeFormUpdate" type="String">
    /// </field>
    /// <field name="BtnUpdate" type="String">
    /// </field>
    /// <field name="PopupEntityDetailsAsOf" type="String">
    /// </field>
    /// <field name="BtnCreateEntity" type="String">
    /// </field>
    /// <field name="LabelAttr" type="String">
    /// </field>
    /// <field name="ModalWizard" type="String">
    /// </field>
    /// <field name="HdnSelectedDateOption" type="String">
    /// </field>
    /// <field name="BtnViewEffectiveDetails" type="String">
    /// </field>
    /// <field name="DateCustomEffective" type="String">
    /// </field>
    /// <field name="BtnBackDateEntity" type="String">
    /// </field>
    /// <field name="DateEffectiveDate" type="String">
    /// </field>
    /// <field name="CustomSuccessLabel" type="String">
    /// </field>
    /// <field name="HdnUserName" type="String">
    /// </field>
    /// <field name="IframeDeletionPopup" type="String">
    /// </field>
    /// <field name="PnlDeletionPopup" type="String">
    /// </field>
    /// <field name="ModalDeletePopupBehaviorId" type="String">
    /// </field>
    /// <field name="SearchBarBtnId" type="String">
    /// </field>
    /// <field name="LblSearchError" type="String">
    /// </field>
    /// <field name="HdnEntityCode" type="String">
    /// </field>
    /// <field name="OpenInNewWindow" type="String">
    /// </field>
    /// <field name="BtnTransfer" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchDataControlIds.prototype = {
    DDLEntityTypeId: null,
    DDLEntityGroupId: null,
    DDLRdStateId: null,
    BtnSearchDataId: null,
    BtnBrowseDataId: null,
    TxtSearchDataId: null,
    GridSearchDataId: null,
    PanelGridSearchId: null,
    CtmGridSearchId: null,
    PanelEditDataId: null,
    EntityDetailsAsOfDate: null,
    BtnGetEntityDetailsAsOf: null,
    LblDeleteTask: null,
    EntityDetailAsOfKnowledgeDate: null,
    HdnEntityActionType: null,
    HdnClickedEntityCode: null,
    HdnSearchGridCacheKey: null,
    HdnEntityCodeFormUpdate: null,
    BtnUpdate: null,
    PopupEntityDetailsAsOf: null,
    BtnCreateEntity: null,
    LabelAttr: null,
    ModalWizard: null,
    HdnSelectedDateOption: null,
    BtnViewEffectiveDetails: null,
    DateCustomEffective: null,
    BtnBackDateEntity: null,
    DateEffectiveDate: null,
    CustomSuccessLabel: null,
    HdnUserName: null,
    IframeDeletionPopup: null,
    PnlDeletionPopup: null,
    ModalDeletePopupBehaviorId: null,
    SearchBarBtnId: null,
    LblSearchError: null,
    HdnEntityCode: null,
    OpenInNewWindow: null,
    BtnTransfer: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchDataControls

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchDataControls = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls(objSearchDataControlIds) {
    /// <param name="objSearchDataControlIds" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchDataControlIds">
    /// </param>
    /// <field name="_objSearchDataControlIds" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchDataControlIds">
    /// </field>
    this._objSearchDataControlIds = objSearchDataControlIds;
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchDataControls.prototype = {
    _objSearchDataControlIds: null,
    
    get_customSuccessLabel: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_customSuccessLabel() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.CustomSuccessLabel);
    },
    
    get_pnlDeletionPopup: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_pnlDeletionPopup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.PnlDeletionPopup);
    },
    
    get_iframeDeletionPopup: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_iframeDeletionPopup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.IframeDeletionPopup);
    },
    
    get_ddlEntityType: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_ddlEntityType() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.DDLEntityTypeId);
    },
    
    get_ddlEntityGroup: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_ddlEntityGroup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.DDLEntityGroupId);
    },
    
    get_ddlRdState: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_ddlRdState() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.DDLRdStateId);
    },
    
    get_btnViewEffectiveDetails: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_btnViewEffectiveDetails() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.BtnViewEffectiveDetails);
    },
    
    get_btnBackDateEntity: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_btnBackDateEntity() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.BtnBackDateEntity);
    },
    
    get_hdnSelectedDateOption: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_hdnSelectedDateOption() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.HdnSelectedDateOption);
    },
    
    get_btnUpdate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_btnUpdate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.BtnUpdate);
    },
    
    get_btnSearchData: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_btnSearchData() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.BtnSearchDataId);
    },
    
    get_btnBrowseData: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_btnBrowseData() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.BtnBrowseDataId);
    },
    
    get_txtSearchData: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_txtSearchData() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.TxtSearchDataId);
    },
    
    get_gridSearchData: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_gridSearchData() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.GridSearchDataId + '_bodyDiv_Table');
    },
    
    get_panelGridSearch: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_panelGridSearch() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.PanelGridSearchId);
    },
    
    get_ctmGridSearch: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_ctmGridSearch() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.CtmGridSearchId + '_Root_0');
    },
    
    get_panelEditData: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_panelEditData() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.PanelEditDataId);
    },
    
    get_divErrorDisplay: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_divErrorDisplay() {
        /// <summary>
        /// Gets the DIV to display error
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('divErrorDisplay');
    },
    
    get_popupEntityDetailsAsOf: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_popupEntityDetailsAsOf() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.PopupEntityDetailsAsOf);
    },
    
    get_entityDetailsAsOfDate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_entityDetailsAsOfDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.EntityDetailsAsOfDate);
    },
    
    get_entityDetailAsOfKnowledgeDate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_entityDetailAsOfKnowledgeDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.EntityDetailAsOfKnowledgeDate);
    },
    
    get_dateCustomEffective: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_dateCustomEffective() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.DateCustomEffective);
    },
    
    get_dateEffectiveDate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_dateEffectiveDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.DateEffectiveDate);
    },
    
    get_btnGetEntityDetailsAsOf: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_btnGetEntityDetailsAsOf() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.BtnGetEntityDetailsAsOf);
    },
    
    get_lblDeleteTask: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_lblDeleteTask() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.LblDeleteTask);
    },
    
    get_hdnUserName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_hdnUserName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.HdnUserName);
    },
    
    get_hdnEntityActionType: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_hdnEntityActionType() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.HdnEntityActionType);
    },
    
    get_hdnClickedEntityCode: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_hdnClickedEntityCode() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.HdnClickedEntityCode);
    },
    
    get_hdnSearchGridCacheKey: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_hdnSearchGridCacheKey() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.HdnSearchGridCacheKey);
    },
    
    get_hdnEntityCodeFormUpdate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_hdnEntityCodeFormUpdate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.HdnEntityCodeFormUpdate);
    },
    
    get_btnCreateEntity: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_btnCreateEntity() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.BtnCreateEntity);
    },
    
    get_labelAttr: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_labelAttr() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.LabelAttr);
    },
    
    get_modalWizard: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_modalWizard() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.ModalWizard);
    },
    
    get_searchBarBtnId: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_searchBarBtnId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.SearchBarBtnId);
    },
    
    get_lblSearchError: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_lblSearchError() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.LblSearchError);
    },
    
    get_hdnEntityCode: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_hdnEntityCode() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.HdnEntityCode);
    },
    
    get_btnTransfer: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSSearchDataControls$get_btnTransfer() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objSearchDataControlIds.BtnTransfer);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSTaskStatusInfo

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSTaskStatusInfo = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusInfo() {
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSTaskStatusControlIDs

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSTaskStatusControlIDs = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusControlIDs() {
    /// <field name="ModalDeleteTaskID" type="String">
    /// </field>
    /// <field name="RdBetweenDatesID" type="String">
    /// </field>
    /// <field name="RdFromDateID" type="String">
    /// </field>
    /// <field name="RdPriorToDateID" type="String">
    /// </field>
    /// <field name="TxtStartDateID" type="String">
    /// </field>
    /// <field name="TxtEndDateID" type="String">
    /// </field>
    /// <field name="HiddenTaskStatusID" type="String">
    /// </field>
    /// <field name="LblTaskNameID" type="String">
    /// </field>
    /// <field name="BtnGetTasksID" type="String">
    /// </field>
    /// <field name="DivErrorSummaryID" type="String">
    /// </field>
    /// <field name="TimerControlID" type="String">
    /// </field>
    /// <field name="LblTimerControlID" type="String">
    /// </field>
    /// <field name="HdnTimerControlID" type="String">
    /// </field>
    /// <field name="ModalPopupLogDetail" type="String">
    /// </field>
    /// <field name="TaskLogTextBox" type="String">
    /// </field>
    /// <field name="GridTaskStatusID" type="String">
    /// </field>
    /// <field name="HiddenTaskMasterId" type="String">
    /// </field>
    /// <field name="HdnPostBackControlId" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSTaskStatusControlIDs.prototype = {
    ModalDeleteTaskID: null,
    RdBetweenDatesID: null,
    RdFromDateID: null,
    RdPriorToDateID: null,
    TxtStartDateID: null,
    TxtEndDateID: null,
    HiddenTaskStatusID: null,
    LblTaskNameID: null,
    BtnGetTasksID: null,
    DivErrorSummaryID: null,
    TimerControlID: null,
    LblTimerControlID: null,
    HdnTimerControlID: null,
    ModalPopupLogDetail: null,
    TaskLogTextBox: null,
    GridTaskStatusID: null,
    HiddenTaskMasterId: null,
    HdnPostBackControlId: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSTaskStatusControls

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSTaskStatusControls = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusControls(objControlIDs) {
    /// <param name="objControlIDs" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSTaskStatusControlIDs">
    /// </param>
    /// <field name="_objTaskStatusControlIDs" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSTaskStatusControlIDs">
    /// </field>
    this._objTaskStatusControlIDs = objControlIDs;
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSTaskStatusControls.prototype = {
    _objTaskStatusControlIDs: null,
    
    get_gridTaskStatus: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusControls$get_gridTaskStatus() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.GridTaskStatusID + '_bodyDiv_Table');
    },
    
    get_rdBetweenDates: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusControls$get_rdBetweenDates() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.RdBetweenDatesID);
    },
    
    get_rdFromDate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusControls$get_rdFromDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.RdFromDateID);
    },
    
    get_rdPriorToDate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusControls$get_rdPriorToDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.RdPriorToDateID);
    },
    
    get_txtStartDate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusControls$get_txtStartDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.TxtStartDateID);
    },
    
    get_txtEndDate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusControls$get_txtEndDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.TxtEndDateID);
    },
    
    get_hiddenTaskStatus: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusControls$get_hiddenTaskStatus() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.HiddenTaskStatusID);
    },
    
    get_lblTaskName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusControls$get_lblTaskName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.LblTaskNameID);
    },
    
    get_tdStartDate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusControls$get_tdStartDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('tdStartDate1');
    },
    
    get_btnGetTasks: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusControls$get_btnGetTasks() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.BtnGetTasksID);
    },
    
    get_divErrorSummary: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusControls$get_divErrorSummary() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.DivErrorSummaryID);
    },
    
    get_lblTimerControl: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusControls$get_lblTimerControl() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.LblTimerControlID);
    },
    
    get_hdnTimerControl: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusControls$get_hdnTimerControl() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.HdnTimerControlID);
    },
    
    get_taskLogTextBox: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusControls$get_taskLogTextBox() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.TaskLogTextBox);
    },
    
    get_hiddenTaskMasterId: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusControls$get_hiddenTaskMasterId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.HiddenTaskMasterId);
    },
    
    get_hdnPostBackControlId: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSTaskStatusControls$get_hdnPostBackControlId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskStatusControlIDs.HdnPostBackControlId);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSUIRulesControlIds

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSUIRulesControlIds = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControlIds() {
    /// <summary>
    /// Class containing controlIds
    /// </summary>
    /// <field name="ruleEditorId" type="String">
    /// </field>
    /// <field name="BtnCancelId" type="String">
    /// </field>
    /// <field name="BtnSaveRuleId" type="String">
    /// </field>
    /// <field name="TxtRuleNameId" type="String">
    /// </field>
    /// <field name="GridRulesId" type="String">
    /// </field>
    /// <field name="ModalPriority" type="String">
    /// </field>
    /// <field name="TxtMaxPriorityId" type="String">
    /// </field>
    /// <field name="LblRuleNameId" type="String">
    /// </field>
    /// <field name="HdnRuleId" type="String">
    /// </field>
    /// <field name="ModalDelete" type="String">
    /// </field>
    /// <field name="HiddenRuleSetID" type="String">
    /// </field>
    /// <field name="HiddenRuleName" type="String">
    /// </field>
    /// <field name="HiddenAddUpdate" type="String">
    /// </field>
    /// <field name="ModalStateChangeConfirm" type="String">
    /// </field>
    /// <field name="ModalSwitchState" type="String">
    /// </field>
    /// <field name="BtnCommitId" type="String">
    /// </field>
    /// <field name="ValPriorityId" type="String">
    /// </field>
    /// <field name="hiddenSwitchTo" type="String">
    /// </field>
    /// <field name="ddlSelectDependentValue" type="String">
    /// </field>
    /// <field name="BtnUpdateRule" type="String">
    /// </field>
    /// <field name="TableRuleEditor" type="String">
    /// </field>
    /// <field name="PostDataForSave" type="String">
    /// </field>
    /// <field name="HiddenSaveInfo" type="String">
    /// </field>
    /// <field name="RuleBtnPanel" type="String">
    /// </field>
    /// <field name="HiddenMaxPriority" type="String">
    /// </field>
    /// <field name="HiddenIsDDlChanged" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSUIRulesControlIds.prototype = {
    ruleEditorId: null,
    BtnCancelId: null,
    BtnSaveRuleId: null,
    TxtRuleNameId: null,
    GridRulesId: null,
    ModalPriority: null,
    TxtMaxPriorityId: null,
    LblRuleNameId: null,
    HdnRuleId: null,
    ModalDelete: null,
    HiddenRuleSetID: null,
    HiddenRuleName: null,
    HiddenAddUpdate: null,
    ModalStateChangeConfirm: null,
    ModalSwitchState: null,
    BtnCommitId: null,
    ValPriorityId: null,
    hiddenSwitchTo: null,
    ddlSelectDependentValue: null,
    BtnUpdateRule: null,
    TableRuleEditor: null,
    PostDataForSave: null,
    HiddenSaveInfo: null,
    RuleBtnPanel: null,
    HiddenMaxPriority: null,
    HiddenIsDDlChanged: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSUIRulesControls

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSUIRulesControls = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls(controlIds) {
    /// <summary>
    /// Class containing Rules Controls
    /// </summary>
    /// <param name="controlIds" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSUIRulesControlIds">
    /// The control ids.
    /// </param>
    /// <field name="_controlIds" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSUIRulesControlIds">
    /// </field>
    this._controlIds = controlIds;
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSUIRulesControls.prototype = {
    _controlIds: null,
    
    get_rRuleEditor: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_rRuleEditor() {
        /// <summary>
        /// Gets the R rule editor.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.ruleEditorId);
    },
    
    get_btnCancel: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_btnCancel() {
        /// <summary>
        /// Gets the BTN cancel.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnCancelId);
    },
    
    get_btnSaveRule: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_btnSaveRule() {
        /// <summary>
        /// Gets the BTN save rule.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnSaveRuleId);
    },
    
    get_txtRuleName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_txtRuleName() {
        /// <summary>
        /// Gets the name of the TXT rule.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtRuleNameId);
    },
    
    get_gridRules: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_gridRules() {
        /// <summary>
        /// Gets the grid rules.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.GridRulesId);
    },
    
    get_btnShowRuleEditor: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_btnShowRuleEditor() {
        /// <summary>
        /// Gets the BTN show rule editor.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnShowRuleEditor');
    },
    
    get_errorRuleName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_errorRuleName() {
        /// <summary>
        /// Gets the name of the error rule.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('errorRuleName');
    },
    
    get_txtMaxPriority: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_txtMaxPriority() {
        /// <summary>
        /// Gets the TXT max priority.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtMaxPriorityId);
    },
    
    get_lblRuleName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_lblRuleName() {
        /// <summary>
        /// Gets the name of the LBL rule.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.LblRuleNameId);
    },
    
    get_hdnRuleId: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_hdnRuleId() {
        /// <summary>
        /// Gets the HDN rule id.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnRuleId);
    },
    
    get_lblRuleNameToDelete: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_lblRuleNameToDelete() {
        /// <summary>
        /// Gets the LBL rule name to delete.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblRuleNameToDelete');
    },
    
    get_hiddenRuleSetID: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_hiddenRuleSetID() {
        /// <summary>
        /// Gets the hidden rule set ID.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HiddenRuleSetID);
    },
    
    get_hiddenRuleName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_hiddenRuleName() {
        /// <summary>
        /// Gets the name of the hidden rule.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HiddenRuleName);
    },
    
    get_hiddenAddUpdate: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_hiddenAddUpdate() {
        /// <summary>
        /// Gets the hidden add update.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HiddenAddUpdate);
    },
    
    get_btnYes: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_btnYes() {
        /// <summary>
        /// Gets the BTN yes.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnYes');
    },
    
    get_lblRuleNameSwitch: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_lblRuleNameSwitch() {
        /// <summary>
        /// Gets the LBL rule name switch.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblRuleNameSwitch');
    },
    
    get_lblSwitchState: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_lblSwitchState() {
        /// <summary>
        /// Gets the state of the LBL switch.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblSwitchState');
    },
    
    get_btnCommit: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_btnCommit() {
        /// <summary>
        /// Gets the BTN commit.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnCommitId);
    },
    
    get_valPriority: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_valPriority() {
        /// <summary>
        /// Gets the val priority.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.ValPriorityId);
    },
    
    get_hiddenSwitchTo: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_hiddenSwitchTo() {
        /// <summary>
        /// Gets the BTN switch yes.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.hiddenSwitchTo);
    },
    
    get_ddlSelectDependentValue: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_ddlSelectDependentValue() {
        /// <summary>
        /// Gets the DDL select attribute.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.ddlSelectDependentValue);
    },
    
    get_btnUpdateRule: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_btnUpdateRule() {
        /// <summary>
        /// Gets the BTN update rule.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnUpdateRule);
    },
    
    get_tableRuleEditor: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_tableRuleEditor() {
        /// <summary>
        /// Gets the table rule editor.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TableRuleEditor);
    },
    
    get_postDataForSave: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_postDataForSave() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.PostDataForSave);
    },
    
    get_hiddenSaveInfo: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_hiddenSaveInfo() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HiddenSaveInfo);
    },
    
    get_ruleBtnPanel: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_ruleBtnPanel() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.RuleBtnPanel);
    },
    
    get_hiddenMaxPriority: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_hiddenMaxPriority() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HiddenMaxPriority);
    },
    
    get_hiddenIsDDlChanged: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMSUIRulesControls$get_hiddenIsDDlChanged() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HiddenIsDDlChanged);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMUserBasedLayoutControlIds

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMUserBasedLayoutControlIds = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControlIds() {
    /// <field name="modalAddLayout" type="String">
    /// </field>
    /// <field name="BtnAddLayout" type="String">
    /// </field>
    /// <field name="lblPanelHeaderAtrib" type="String">
    /// </field>
    /// <field name="btnLayoutSave" type="String">
    /// </field>
    /// <field name="HdnCommandValue" type="String">
    /// </field>
    /// <field name="DdlLayoutType" type="String">
    /// </field>
    /// <field name="LblGroupName" type="String">
    /// </field>
    /// <field name="DdlGroupName" type="String">
    /// </field>
    /// <field name="DdlUserName" type="String">
    /// </field>
    /// <field name="TxtLayoutName" type="String">
    /// </field>
    /// <field name="LblDisplayNameError" type="String">
    /// </field>
    /// <field name="GridUserBasedLayoutDetails" type="String">
    /// </field>
    /// <field name="HdnTemplateId" type="String">
    /// </field>
    /// <field name="ModalPopupFailure" type="String">
    /// </field>
    /// <field name="Label1" type="String">
    /// </field>
    /// <field name="ModalDeleteLayout" type="String">
    /// </field>
    /// <field name="HdnLayoutType" type="String">
    /// </field>
    /// <field name="HdnUserDependentId" type="String">
    /// </field>
    /// <field name="HdnGroupDependentId" type="String">
    /// </field>
    /// <field name="HdnPossibleEntityStates" type="String">
    /// </field>
    /// <field name="HdnSelectedEntityStates" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMUserBasedLayoutControlIds.prototype = {
    modalAddLayout: null,
    BtnAddLayout: null,
    lblPanelHeaderAtrib: null,
    btnLayoutSave: null,
    HdnCommandValue: null,
    DdlLayoutType: null,
    LblGroupName: null,
    DdlGroupName: null,
    DdlUserName: null,
    TxtLayoutName: null,
    LblDisplayNameError: null,
    GridUserBasedLayoutDetails: null,
    HdnTemplateId: null,
    ModalPopupFailure: null,
    Label1: null,
    ModalDeleteLayout: null,
    HdnLayoutType: null,
    HdnUserDependentId: null,
    HdnGroupDependentId: null,
    HdnPossibleEntityStates: null,
    HdnSelectedEntityStates: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMUserBasedLayoutControls

com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMUserBasedLayoutControls = function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls(controlIds) {
    /// <param name="controlIds" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMUserBasedLayoutControlIds">
    /// </param>
    /// <field name="_controlIds" type="com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMUserBasedLayoutControlIds">
    /// </field>
    this._controlIds = controlIds;
}
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMUserBasedLayoutControls.prototype = {
    _controlIds: null,
    
    get_btnAddLayout: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls$get_btnAddLayout() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnAddLayout);
    },
    
    get_lblPanelHeaderAtrib: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls$get_lblPanelHeaderAtrib() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.lblPanelHeaderAtrib);
    },
    
    get_btnLayoutSave: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls$get_btnLayoutSave() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.btnLayoutSave);
    },
    
    get_hdnCommandValue: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls$get_hdnCommandValue() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnCommandValue);
    },
    
    get_ddlLayoutType: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls$get_ddlLayoutType() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.DdlLayoutType);
    },
    
    get_lblGroupName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls$get_lblGroupName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.LblGroupName);
    },
    
    get_ddlUserName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls$get_ddlUserName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.DdlUserName);
    },
    
    get_ddlGroupName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls$get_ddlGroupName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.DdlGroupName);
    },
    
    get_txtLayoutName: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls$get_txtLayoutName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtLayoutName);
    },
    
    get_lblDisplayNameError: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls$get_lblDisplayNameError() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.LblDisplayNameError);
    },
    
    get_gridUserBasedLayoutDetails: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls$get_gridUserBasedLayoutDetails() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.GridUserBasedLayoutDetails);
    },
    
    get_hdnTemplateId: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls$get_hdnTemplateId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnTemplateId);
    },
    
    get_label1: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls$get_label1() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.Label1);
    },
    
    get_hdnLayoutType: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls$get_hdnLayoutType() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnLayoutType);
    },
    
    get_hdnUserDependentId: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls$get_hdnUserDependentId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnUserDependentId);
    },
    
    get_hdnGroupDependentId: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls$get_hdnGroupDependentId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnGroupDependentId);
    },
    
    get_hdnPossibleEntityStates: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls$get_hdnPossibleEntityStates() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnPossibleEntityStates);
    },
    
    get_hdnSelectedEntityStates: function com_ivp_refmaster_scripts_common_RefMasterJSInfo_RMUserBasedLayoutControls$get_hdnSelectedEntityStates() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnSelectedEntityStates);
    }
}


Type.registerNamespace('RefMasterJSCommon');

////////////////////////////////////////////////////////////////////////////////
// RefMasterJSCommon.RMSCommons

RefMasterJSCommon.RMSCommons = function RefMasterJSCommon_RMSCommons() {
}
RefMasterJSCommon.RMSCommons.createTab = function RefMasterJSCommon_RMSCommons$createTab(identifier, url, uniqueId, tabText, openInNewWindow) {
    /// <param name="identifier" type="String">
    /// </param>
    /// <param name="url" type="String">
    /// </param>
    /// <param name="uniqueId" type="String">
    /// </param>
    /// <param name="tabText" type="String">
    /// </param>
    /// <param name="openInNewWindow" type="Boolean">
    /// </param>
    if (openInNewWindow) {
        var WINDOWS_OPEN = 'toolbars=no, menubar=no, location=no, scrollbars=yes, resizable=yes, status=no,';
        var windowString = WINDOWS_OPEN + 'height=' + window.screen.height + ',width=' + window.screen.width;
        window.open(url, '', windowString);
    }
    else {
        eval('getRMLeftMenu().createTab(\'' + identifier + '\',\'' + url + '\',\'' + uniqueId + '\',\'' + tabText + '\');');
    }
}
RefMasterJSCommon.RMSCommons.closeTab = function RefMasterJSCommon_RMSCommons$closeTab() {
    eval('var uniqueId = $(\'#RMTabUniqueID\').val(); if (uniqueId != null && uniqueId != \'\') getRMLeftMenu().removeTab(uniqueId);');
}
RefMasterJSCommon.RMSCommons.closeTabWithId = function RefMasterJSCommon_RMSCommons$closeTabWithId(uniqueId) {
    /// <param name="uniqueId" type="String">
    /// </param>
    if (uniqueId != null && uniqueId !== com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY) {
        eval('getRMLeftMenu().removeTab(\'' + uniqueId + '\');');
    }
}
RefMasterJSCommon.RMSCommons.createDropDown = function RefMasterJSCommon_RMSCommons$createDropDown(ddlID, showSearch, isRunningText) {
    /// <param name="ddlID" type="String">
    /// </param>
    /// <param name="showSearch" type="Boolean">
    /// </param>
    /// <param name="isRunningText" type="Boolean">
    /// </param>
    var script = 'var obj=new Object(); obj.select = $(\'[id$=\"' + ddlID + '\"]\'); obj.showSearch = ' + showSearch + '; obj.isRunningText = ' + isRunningText + '; obj.ready = function(){};smselect.create(obj);';
    eval(script);
}
RefMasterJSCommon.RMSCommons.createDisabledDropDown = function RefMasterJSCommon_RMSCommons$createDisabledDropDown(ddlID, showSearch, isRunningText) {
    /// <param name="ddlID" type="String">
    /// </param>
    /// <param name="showSearch" type="Boolean">
    /// </param>
    /// <param name="isRunningText" type="Boolean">
    /// </param>
    var script = 'var obj=new Object(); obj.select = $(\'[id$=\"' + ddlID + '\"]\'); obj.showSearch = ' + showSearch + '; obj.isRunningText = ' + isRunningText + '; obj.ready = function(){};smselect.create(obj);smselect.disable($(\'[id ^=smselect][id$=\"' + ddlID + '\"]\'));';
    eval(script);
}
RefMasterJSCommon.RMSCommons.getGUID = function RefMasterJSCommon_RMSCommons$getGUID() {
    /// <returns type="String"></returns>
    var obj = null;
    eval('obj=GetGUID()');
    return obj;
}
RefMasterJSCommon.RMSCommons.isNullOrEmpty = function RefMasterJSCommon_RMSCommons$isNullOrEmpty(value) {
    /// <param name="value" type="String">
    /// </param>
    /// <returns type="Boolean"></returns>
    var flag = true;
    if (value != null && value !== com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY) {
        flag = false;
    }
    return flag;
}
RefMasterJSCommon.RMSCommons.replaceSpecialCharacterSpace = function RefMasterJSCommon_RMSCommons$replaceSpecialCharacterSpace(val) {
    /// <param name="val" type="String">
    /// </param>
    /// <returns type="String"></returns>
    return val.replace(new RegExp('\ufffd', 'gi'), ' ');
}
RefMasterJSCommon.RMSCommons.checkStringWithRegExOrString = function RefMasterJSCommon_RMSCommons$checkStringWithRegExOrString(ele, expression, ifRegEx, isCaseSensitive) {
    /// <summary>
    /// Checks the string with reg ex or string.
    /// </summary>
    /// <param name="ele" type="Object" domElement="true">
    /// The dom element whose inner value has to be matched.
    /// </param>
    /// <param name="expression" type="Array" elementType="String">
    /// The expression to be searched.
    /// </param>
    /// <param name="ifRegEx" type="Boolean">
    /// if the expression is a regEx set to <c>true</c>
    /// </param>
    /// <param name="isCaseSensitive" type="Boolean">
    /// if set to <c>true</c> [is case sensitive].
    /// </param>
    /// <returns type="Boolean"></returns>
    var value;
    if (ele.nodeName.toLowerCase() === 'input') {
        value = (ele).value;
    }
    else {
        var elemrnt = new com.ivp.rad.controls.scripts.RADBrowserScripts.RADDOMElement();
        value = elemrnt.getInnerContent(ele);
    }
    var flag = false;
    if (ifRegEx && value.trim().length !== 0) {
        var regEx = (isCaseSensitive) ? new RegExp(expression[0]) : new RegExp(expression[0], 'i');
        flag = regEx.test(value);
    }
    else {
        expression[expression.length] = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
        for (var i = 0; i < expression.length; i++) {
            if ((isCaseSensitive && value.trim() === expression[i].trim()) || (!isCaseSensitive && value.trim().toLowerCase() === expression[i].trim().toLowerCase())) {
                flag = true;
                break;
            }
        }
    }
    return flag;
}
RefMasterJSCommon.RMSCommons.findIndexInDdl = function RefMasterJSCommon_RMSCommons$findIndexInDdl(valueToSearch, ddlToSearch) {
    /// <summary>
    /// Finds the index of the option element in DropDownList by Value.
    /// </summary>
    /// <param name="valueToSearch" type="String">
    /// String Value to search.
    /// </param>
    /// <param name="ddlToSearch" type="Object" domElement="true">
    /// DropDownList to search.
    /// </param>
    /// <returns type="Number" integer="true"></returns>
    var length = ddlToSearch.options.length;
    for (var i = 0; i < length; i++) {
        if ((ddlToSearch.options[i]).value === valueToSearch.trim()) {
            return i;
        }
    }
    return 0;
}
RefMasterJSCommon.RMSCommons.findIndexInDdlByText = function RefMasterJSCommon_RMSCommons$findIndexInDdlByText(textToSearch, ddlToSearch) {
    /// <summary>
    /// Finds the index of the option element in the DropDownList by Text.
    /// </summary>
    /// <param name="textToSearch" type="String">
    /// The String Text to search.
    /// </param>
    /// <param name="ddlToSearch" type="Object" domElement="true">
    /// The DropDownList to search.
    /// </param>
    /// <returns type="Number" integer="true"></returns>
    var length = ddlToSearch.options.length;
    for (var i = 0; i < length; i++) {
        if ((ddlToSearch.options[i]).text.trim() === textToSearch.trim()) {
            return i;
        }
    }
    return 0;
}
RefMasterJSCommon.RMSCommons.doControlPostback = function RefMasterJSCommon_RMSCommons$doControlPostback(targetElementID) {
    /// <param name="targetElementID" type="String">
    /// </param>
    eval('__doPostBack(\'' + targetElementID + '\',\'\')');
}
RefMasterJSCommon.RMSCommons.expandCollapsePanel = function RefMasterJSCommon_RMSCommons$expandCollapsePanel(collapseControlID) {
    /// <summary>
    /// Expand/Collapse the collapsable panel.
    /// </summary>
    /// <param name="collapseControlID" type="String">
    /// The CollapseControlID.
    /// </param>
    var collapseControl = com.ivp.rad.rscriptutils.RSValidators.getObject(collapseControlID);
    if (collapseControl != null) {
        collapseControl.click();
    }
}
RefMasterJSCommon.RMSCommons.showError = function RefMasterJSCommon_RMSCommons$showError(divElement, strErrorMsg) {
    /// <summary>
    /// Shows formated error in the divElement.
    /// </summary>
    /// <param name="divElement" type="Object" domElement="true">
    /// The div element to show error.
    /// </param>
    /// <param name="strErrorMsg" type="String">
    /// The error message.
    /// </param>
    var holder = com.ivp.rad.rscriptutils.RSValidators.createErrorMessageHolder(divElement);
    com.ivp.rad.rscriptutils.RSValidators.createErrorMessage(holder, strErrorMsg);
}
RefMasterJSCommon.RMSCommons.createElement = function RefMasterJSCommon_RMSCommons$createElement(element) {
    /// <param name="element" type="Object" domElement="true">
    /// </param>
    /// <returns type="RefMasterJSCommon.WrapperDom"></returns>
    var obj = null;
    eval('obj = $(element);');
    return obj;
}
RefMasterJSCommon.RMSCommons.contains = function RefMasterJSCommon_RMSCommons$contains(main, toFind) {
    /// <summary>
    /// Determines whether "main" string contains "toFind" string.
    /// </summary>
    /// <param name="main" type="String">
    /// String to be searched
    /// </param>
    /// <param name="toFind" type="String">
    /// String(pattern) containing the sequence to match.
    /// </param>
    /// <returns type="Boolean"></returns>
    var regEx = new RegExp(toFind, 'gi');
    return regEx.test(main);
}
RefMasterJSCommon.RMSCommons.getTargetTableRow = function RefMasterJSCommon_RMSCommons$getTargetTableRow(targetElement) {
    /// <summary>
    /// Gets the target table row element.
    /// </summary>
    /// <param name="targetElement" type="Object" domElement="true">
    /// The event target element (e.target).
    /// </param>
    /// <returns type="Object" domElement="true"></returns>
    return com.ivp.rad.rscriptutils.RSCommonScripts.findControl(targetElement, 'TR');
}
RefMasterJSCommon.RMSCommons.openWindowExceptionsNewTab = function RefMasterJSCommon_RMSCommons$openWindowExceptionsNewTab(secIds, objFilterExceptionInfo) {
    /// <param name="secIds" type="String">
    /// </param>
    /// <param name="objFilterExceptionInfo" type="FilterInfoForException">
    /// </param>
    var success = Function.createDelegate(null, RefMasterJSCommon.RMSCommons._onSuccessSetValuesForExceptionNewTab);
    var failed = Function.createDelegate(null, RefMasterJSCommon.RMSCommons._onFailure);
    var objUIService = new com.ivp.refmaster.ui.BaseUserControls.Service.Service();
    var sessionIdentifierKey = RefMasterJSCommon.RMSCommons.getGUID();
    objUIService.SetValuesForException(secIds, sessionIdentifierKey, objFilterExceptionInfo, success, failed, sessionIdentifierKey);
}
RefMasterJSCommon.RMSCommons._onSuccessSetValuesForExceptionNewTab = function RefMasterJSCommon_RMSCommons$_onSuccessSetValuesForExceptionNewTab(result, eventArgs) {
    /// <param name="result" type="Object">
    /// </param>
    /// <param name="eventArgs" type="Object">
    /// </param>
    var url = 'RMHomeInternal.aspx?identifier=RefM_ExceptionManagerOld&SessionIdentifier=' + eventArgs;
    RefMasterJSCommon.RMSCommons.createTab('ExceptionManager', url, RefMasterJSCommon.RMSCommons.getGUID(), 'Manage Entity Exceptions', false);
}
RefMasterJSCommon.RMSCommons._onFailure = function RefMasterJSCommon_RMSCommons$_onFailure(eventArgs) {
    /// <param name="eventArgs" type="Object">
    /// </param>
}
RefMasterJSCommon.RMSCommons.getInnerContent = function RefMasterJSCommon_RMSCommons$getInnerContent(ele) {
    /// <summary>
    /// Gets the inner content of the dom element.
    /// </summary>
    /// <param name="ele" type="Object" domElement="true">
    /// The dom element.
    /// </param>
    /// <returns type="String"></returns>
    var value;
    if (ele.nodeName.toLowerCase() === 'input') {
        value = (ele).value;
    }
    else {
        var elemrnt = new com.ivp.rad.controls.scripts.RADBrowserScripts.RADDOMElement();
        value = elemrnt.getInnerContent(ele);
    }
    return value;
}
RefMasterJSCommon.RMSCommons.clearError = function RefMasterJSCommon_RMSCommons$clearError(divElement) {
    /// <summary>
    /// Clears the error
    /// </summary>
    /// <param name="divElement" type="Object" domElement="true">
    /// The div element to show error.
    /// </param>
    com.ivp.rad.rscriptutils.RSValidators.clearErrorMessages(divElement);
}
RefMasterJSCommon.RMSCommons.openHelpText = function RefMasterJSCommon_RMSCommons$openHelpText(OpenAnimation, id, client_id, lblToShowId, lblWithTextId) {
    /// <param name="OpenAnimation" type="String">
    /// </param>
    /// <param name="id" type="String">
    /// </param>
    /// <param name="client_id" type="String">
    /// </param>
    /// <param name="lblToShowId" type="String">
    /// </param>
    /// <param name="lblWithTextId" type="String">
    /// </param>
    eval('OpenHelpText(\'' + OpenAnimation + '\',\'' + id + '\',\'' + client_id + '\',\'' + lblToShowId + '\',\'' + lblWithTextId + '\')');
}


Type.registerNamespace('com.ivp.refmaster.scripts.common');

////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMDBDataTypes

com.ivp.refmaster.scripts.common.RMDBDataTypes = function() { 
    /// <field name="VARCHAR" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="INT" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="DECIMAL" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="DATETIME" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="BIT" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="VARCHARMMAX" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="LOOKUP" type="Number" integer="true" static="true">
    /// </field>
};
com.ivp.refmaster.scripts.common.RMDBDataTypes.prototype = {
    VARCHAR: 1, 
    INT: 2, 
    DECIMAL: 3, 
    DATETIME: 4, 
    BIT: 5, 
    VARCHARMMAX: 6, 
    LOOKUP: 7
}
com.ivp.refmaster.scripts.common.RMDBDataTypes.registerEnum('com.ivp.refmaster.scripts.common.RMDBDataTypes', false);


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSDBDataTypes

com.ivp.refmaster.scripts.common.RMSDBDataTypes = function() { 
    /// <field name="VARCHAR" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="INT" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="DECIMAL" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="DATETIME" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="BIT" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="VARCHARMMAX" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="LOOKUP" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="FILE" type="Number" integer="true" static="true">
    /// </field>
};
com.ivp.refmaster.scripts.common.RMSDBDataTypes.prototype = {
    VARCHAR: 1, 
    INT: 2, 
    DECIMAL: 3, 
    DATETIME: 4, 
    BIT: 5, 
    VARCHARMMAX: 6, 
    LOOKUP: 7, 
    FILE: 8
}
com.ivp.refmaster.scripts.common.RMSDBDataTypes.registerEnum('com.ivp.refmaster.scripts.common.RMSDBDataTypes', false);


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.EntityStructureType

com.ivp.refmaster.scripts.common.EntityStructureType = function() { 
    /// <field name="master" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="singleInfoLeg" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="multiInfoLeg" type="Number" integer="true" static="true">
    /// </field>
};
com.ivp.refmaster.scripts.common.EntityStructureType.prototype = {
    master: 0, 
    singleInfoLeg: 1, 
    multiInfoLeg: 2
}
com.ivp.refmaster.scripts.common.EntityStructureType.registerEnum('com.ivp.refmaster.scripts.common.EntityStructureType', false);


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSActionIdentifierInfo

com.ivp.refmaster.scripts.common.RMSActionIdentifierInfo = function com_ivp_refmaster_scripts_common_RMSActionIdentifierInfo() {
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSActionIdentifierControlIds

com.ivp.refmaster.scripts.common.RMSActionIdentifierControlIds = function com_ivp_refmaster_scripts_common_RMSActionIdentifierControlIds() {
    /// <summary>
    /// ActionIdentifier Control Ids
    /// </summary>
    /// <field name="txtActionNew" type="String">
    /// </field>
    /// <field name="txtActionUpdate" type="String">
    /// </field>
    /// <field name="txtActionDelete" type="String">
    /// </field>
    /// <field name="ddlFieldColumnNames" type="String">
    /// </field>
    /// <field name="btnSaveActionIds" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSActionIdentifierControlIds.prototype = {
    txtActionNew: null,
    txtActionUpdate: null,
    txtActionDelete: null,
    ddlFieldColumnNames: null,
    btnSaveActionIds: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSActionIdentifierControls

com.ivp.refmaster.scripts.common.RMSActionIdentifierControls = function com_ivp_refmaster_scripts_common_RMSActionIdentifierControls(controlIds) {
    /// <summary>
    /// Action Identifier Controls
    /// </summary>
    /// <param name="controlIds" type="com.ivp.refmaster.scripts.common.RMSActionIdentifierControlIds">
    /// </param>
    /// <field name="_actionIdentifierObj" type="com.ivp.refmaster.scripts.common.RMSActionIdentifierControlIds">
    /// </field>
    this._actionIdentifierObj = new com.ivp.refmaster.scripts.common.RMSActionIdentifierControlIds();
    this._actionIdentifierObj = controlIds;
}
com.ivp.refmaster.scripts.common.RMSActionIdentifierControls.prototype = {
    
    get_txtActionNew: function com_ivp_refmaster_scripts_common_RMSActionIdentifierControls$get_txtActionNew() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._actionIdentifierObj.txtActionNew);
    },
    
    get_txtActionUpdate: function com_ivp_refmaster_scripts_common_RMSActionIdentifierControls$get_txtActionUpdate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._actionIdentifierObj.txtActionUpdate);
    },
    
    get_txtActionDelete: function com_ivp_refmaster_scripts_common_RMSActionIdentifierControls$get_txtActionDelete() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._actionIdentifierObj.txtActionDelete);
    },
    
    get_ddlFieldColumnNames: function com_ivp_refmaster_scripts_common_RMSActionIdentifierControls$get_ddlFieldColumnNames() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._actionIdentifierObj.ddlFieldColumnNames);
    },
    
    get_btnSaveActionIds: function com_ivp_refmaster_scripts_common_RMSActionIdentifierControls$get_btnSaveActionIds() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._actionIdentifierObj.btnSaveActionIds);
    },
    
    get_divErrorDisplay: function com_ivp_refmaster_scripts_common_RMSActionIdentifierControls$get_divErrorDisplay() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('divErrorDisplay');
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMAPIEntityTypeInfo

com.ivp.refmaster.scripts.common.RMAPIEntityTypeInfo = function com_ivp_refmaster_scripts_common_RMAPIEntityTypeInfo() {
    /// <field name="DisplayName" type="String">
    /// </field>
    /// <field name="ID" type="String">
    /// </field>
    /// <field name="ViewName" type="String">
    /// </field>
    /// <field name="EntityTypeName" type="String">
    /// </field>
    /// <field name="EntityDerivedFrom" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMAPIEntityTypeInfo.prototype = {
    DisplayName: null,
    ID: null,
    ViewName: null,
    EntityTypeName: null,
    EntityDerivedFrom: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSAttributeManagementInfo

com.ivp.refmaster.scripts.common.RMSAttributeManagementInfo = function com_ivp_refmaster_scripts_common_RMSAttributeManagementInfo() {
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSAttributeManagementControlsIds

com.ivp.refmaster.scripts.common.RMSAttributeManagementControlsIds = function com_ivp_refmaster_scripts_common_RMSAttributeManagementControlsIds() {
    /// <field name="ButtonAttributeRenameSave" type="String">
    /// </field>
    /// <field name="ButtonSave" type="String">
    /// </field>
    /// <field name="BtnSave1" type="String">
    /// </field>
    /// <field name="ContextMenuTemplateMgr" type="String">
    /// </field>
    /// <field name="ContextMenuTemplateMgrDetail" type="String">
    /// </field>
    /// <field name="TxtAttributeName" type="String">
    /// </field>
    /// <field name="LblAttributeError" type="String">
    /// </field>
    /// <field name="TabContainerTemplateMgr" type="String">
    /// </field>
    /// <field name="TabContainerTemplateMgrDetail" type="String">
    /// </field>
    /// <field name="DropPlaceHolder" type="String">
    /// </field>
    /// <field name="placeHolderMaster" type="String">
    /// </field>
    /// <field name="lstForDuplicateName" type="Array">
    /// </field>
    /// <field name="HdnUIXML" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSAttributeManagementControlsIds.prototype = {
    ButtonAttributeRenameSave: null,
    ButtonSave: null,
    BtnSave1: null,
    ContextMenuTemplateMgr: null,
    ContextMenuTemplateMgrDetail: null,
    TxtAttributeName: null,
    LblAttributeError: null,
    TabContainerTemplateMgr: null,
    TabContainerTemplateMgrDetail: null,
    DropPlaceHolder: null,
    placeHolderMaster: null,
    lstForDuplicateName: null,
    HdnUIXML: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSAttributeManagementControls

com.ivp.refmaster.scripts.common.RMSAttributeManagementControls = function com_ivp_refmaster_scripts_common_RMSAttributeManagementControls(RMSAttributeManagementControlIdsobject) {
    /// <param name="RMSAttributeManagementControlIdsobject" type="com.ivp.refmaster.scripts.common.RMSAttributeManagementControlsIds">
    /// </param>
    /// <field name="_rmsAttributeManagementControlIdsObj" type="com.ivp.refmaster.scripts.common.RMSAttributeManagementControlsIds">
    /// </field>
    this._rmsAttributeManagementControlIdsObj = RMSAttributeManagementControlIdsobject;
}
com.ivp.refmaster.scripts.common.RMSAttributeManagementControls.prototype = {
    _rmsAttributeManagementControlIdsObj: null,
    
    get_buttonAttributeRenameSave: function com_ivp_refmaster_scripts_common_RMSAttributeManagementControls$get_buttonAttributeRenameSave() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsAttributeManagementControlIdsObj.ButtonAttributeRenameSave);
    },
    
    get_buttonSave: function com_ivp_refmaster_scripts_common_RMSAttributeManagementControls$get_buttonSave() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsAttributeManagementControlIdsObj.ButtonSave);
    },
    
    get_btnSave1: function com_ivp_refmaster_scripts_common_RMSAttributeManagementControls$get_btnSave1() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsAttributeManagementControlIdsObj.BtnSave1);
    },
    
    get_contextMenuTemplateMgr: function com_ivp_refmaster_scripts_common_RMSAttributeManagementControls$get_contextMenuTemplateMgr() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsAttributeManagementControlIdsObj.ContextMenuTemplateMgr);
    },
    
    get_txtAttributeName: function com_ivp_refmaster_scripts_common_RMSAttributeManagementControls$get_txtAttributeName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsAttributeManagementControlIdsObj.TxtAttributeName);
    },
    
    get_lblAttributeError: function com_ivp_refmaster_scripts_common_RMSAttributeManagementControls$get_lblAttributeError() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsAttributeManagementControlIdsObj.LblAttributeError);
    },
    
    get_contextMenuTemplateMgrDetail: function com_ivp_refmaster_scripts_common_RMSAttributeManagementControls$get_contextMenuTemplateMgrDetail() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsAttributeManagementControlIdsObj.ContextMenuTemplateMgrDetail);
    },
    
    get_tabContainerTemplateMgr: function com_ivp_refmaster_scripts_common_RMSAttributeManagementControls$get_tabContainerTemplateMgr() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsAttributeManagementControlIdsObj.TabContainerTemplateMgr);
    },
    
    get_tabContainerTemplateMgrDetail: function com_ivp_refmaster_scripts_common_RMSAttributeManagementControls$get_tabContainerTemplateMgrDetail() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsAttributeManagementControlIdsObj.TabContainerTemplateMgrDetail);
    },
    
    get_dropPlaceHolder: function com_ivp_refmaster_scripts_common_RMSAttributeManagementControls$get_dropPlaceHolder() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsAttributeManagementControlIdsObj.DropPlaceHolder);
    },
    
    get_placeHolderMaster: function com_ivp_refmaster_scripts_common_RMSAttributeManagementControls$get_placeHolderMaster() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsAttributeManagementControlIdsObj.placeHolderMaster);
    },
    
    get_lstForDuplicateName: function com_ivp_refmaster_scripts_common_RMSAttributeManagementControls$get_lstForDuplicateName() {
        /// <value type="Array"></value>
        return this._rmsAttributeManagementControlIdsObj.lstForDuplicateName;
    },
    
    get_hdnUixml: function com_ivp_refmaster_scripts_common_RMSAttributeManagementControls$get_hdnUixml() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsAttributeManagementControlIdsObj.HdnUIXML);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSAuditInfo

com.ivp.refmaster.scripts.common.RMSAuditInfo = function com_ivp_refmaster_scripts_common_RMSAuditInfo() {
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.AuditSearchControlIds

com.ivp.refmaster.scripts.common.AuditSearchControlIds = function com_ivp_refmaster_scripts_common_AuditSearchControlIds() {
    /// <field name="UserValueLookUp" type="String">
    /// </field>
    /// <field name="Hdnduration" type="String">
    /// </field>
    /// <field name="HdnTimeSeriesUpdateInfo" type="String">
    /// </field>
    /// <field name="HdnTimeSeriesSearch" type="String">
    /// </field>
    /// <field name="hdnShowDateFilterWithTS" type="String">
    /// </field>
    /// <field name="HdnRadioButtonStateAuditSearch" type="String">
    /// </field>
    /// <field name="UpdateButton" type="String">
    /// </field>
    /// <field name="ViewMoreButton" type="String">
    /// </field>
    /// <field name="BtnRefreshAttrAudit" type="String">
    /// </field>
    /// <field name="BtnRefreshAttrTS" type="String">
    /// </field>
    /// <field name="StartDateInput" type="String">
    /// </field>
    /// <field name="EndDateInput" type="String">
    /// </field>
    /// <field name="RMBetweenDatesID" type="String">
    /// </field>
    /// <field name="RMFromDateID" type="String">
    /// </field>
    /// <field name="RMPriorToDateID" type="String">
    /// </field>
    /// <field name="RMDateDropDownID" type="String">
    /// </field>
    /// <field name="RMDateDropDownCustomDivID" type="String">
    /// </field>
    /// <field name="RMDateDropDownTextID" type="String">
    /// </field>
    /// <field name="RMDateDropDownSelectID" type="String">
    /// </field>
    /// <field name="LblStartDate" type="String">
    /// </field>
    /// <field name="LblEndDate" type="String">
    /// </field>
    /// <field name="DateInputStartDateID" type="String">
    /// </field>
    /// <field name="DateInputEndDateID" type="String">
    /// </field>
    /// <field name="DateInputKnowledgeDateID" type="String">
    /// </field>
    /// <field name="hdnMultiTSError" type="String">
    /// </field>
    /// <field name="KnowledgeDateInput" type="String">
    /// </field>
    /// <field name="RADXGridEditableRow" type="String">
    /// </field>
    /// <field name="hdnEditableGridXML" type="String">
    /// </field>
    /// <field name="hdnClickedCell" type="String">
    /// </field>
    /// <field name="ReplaceableDateInput" type="String">
    /// </field>
    /// <field name="RMDateDropDownTSPID" type="String">
    /// </field>
    /// <field name="RMDateDropDownSelectTSPID" type="String">
    /// </field>
    /// <field name="RMDateDropDownCustomDivTSPID" type="String">
    /// </field>
    /// <field name="RMDateDropDownTextTSPID" type="String">
    /// </field>
    /// <field name="rdBetweenDatesID" type="String">
    /// </field>
    /// <field name="rdFromDateID" type="String">
    /// </field>
    /// <field name="rdPriorToDateID" type="String">
    /// </field>
    /// <field name="rdAll" type="String">
    /// </field>
    /// <field name="TimeSeriesPanelEffectiveFromDate" type="String">
    /// </field>
    /// <field name="TimeSeriesPanelEffectiveEndDate" type="String">
    /// </field>
    /// <field name="EndDateLabel" type="String">
    /// </field>
    /// <field name="StartDateLabel" type="String">
    /// </field>
    /// <field name="RefreshButton" type="String">
    /// </field>
    /// <field name="knowledgeDateLable" type="String">
    /// </field>
    /// <field name="KnowledgeDateTimeSeries" type="String">
    /// </field>
    /// <field name="StartDateTimeSeries" type="String">
    /// </field>
    /// <field name="EndDateTimeSeries" type="String">
    /// </field>
    /// <field name="UserValueDateInput" type="String">
    /// </field>
    /// <field name="btnConfirmUpdateTS" type="String">
    /// </field>
    /// <field name="btnCancelUpdateTS" type="String">
    /// </field>
    /// <field name="RMDateDdlAsOfForAudit" type="String">
    /// </field>
    /// <field name="RMDateDdlKDForAudit" type="String">
    /// </field>
    /// <field name="RMDateDdlAsOfForTimeSeries" type="String">
    /// </field>
    /// <field name="RMDateDdlKDForTimeSeries" type="String">
    /// </field>
    /// <field name="RMDateDdlAsOfForAttrAudit" type="String">
    /// </field>
    /// <field name="RMDateDdlKDForAttrAudit" type="String">
    /// </field>
    /// <field name="AuditContainerId" type="String">
    /// </field>
    /// <field name="HdnStringKnowledgeDate" type="String">
    /// </field>
    /// <field name="HdnStringAsofDate" type="String">
    /// </field>
    /// <field name="HdnStartAsofDate" type="String">
    /// </field>
    /// <field name="HdnEndAsofDate" type="String">
    /// </field>
    /// <field name="HdnStartKnowledgeDate" type="String">
    /// </field>
    /// <field name="HdnEndKnowledgeDate" type="String">
    /// </field>
    /// <field name="HdnSelectedOptionAsofDate" type="String">
    /// </field>
    /// <field name="HdnRdbSelectedAsofDate" type="String">
    /// </field>
    /// <field name="HdnSelectedOptionKnowledgeDate" type="String">
    /// </field>
    /// <field name="HdnRdbSelectedKnowledgeDate" type="String">
    /// </field>
    /// <field name="HdnStringKnowledgeDateTS" type="String">
    /// </field>
    /// <field name="HdnStringAsofDateTS" type="String">
    /// </field>
    /// <field name="HdnStartAsofDateTS" type="String">
    /// </field>
    /// <field name="HdnEndAsofDateTS" type="String">
    /// </field>
    /// <field name="HdnStartKnowledgeDateTS" type="String">
    /// </field>
    /// <field name="HdnEndKnowledgeDateTS" type="String">
    /// </field>
    /// <field name="HdnSelectedOptionAsofDateTS" type="String">
    /// </field>
    /// <field name="HdnRdbSelectedAsofDateTS" type="String">
    /// </field>
    /// <field name="HdnSelectedOptionKnowledgeDateTS" type="String">
    /// </field>
    /// <field name="HdnRdbSelectedKnowledgeDateTS" type="String">
    /// </field>
    /// <field name="AuditHistoryTab" type="String">
    /// </field>
    /// <field name="TimeSeriesTab" type="String">
    /// </field>
    /// <field name="AuditHistoryGrid" type="String">
    /// </field>
    /// <field name="TimeSeriesGrid" type="String">
    /// </field>
    /// <field name="GridTabContainer" type="String">
    /// </field>
    /// <field name="UserValueFileInput" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.AuditSearchControlIds.prototype = {
    UserValueLookUp: null,
    Hdnduration: null,
    HdnTimeSeriesUpdateInfo: null,
    HdnTimeSeriesSearch: null,
    hdnShowDateFilterWithTS: null,
    HdnRadioButtonStateAuditSearch: null,
    UpdateButton: null,
    ViewMoreButton: null,
    BtnRefreshAttrAudit: null,
    BtnRefreshAttrTS: null,
    StartDateInput: null,
    EndDateInput: null,
    RMBetweenDatesID: null,
    RMFromDateID: null,
    RMPriorToDateID: null,
    RMDateDropDownID: null,
    RMDateDropDownCustomDivID: null,
    RMDateDropDownTextID: null,
    RMDateDropDownSelectID: null,
    LblStartDate: null,
    LblEndDate: null,
    DateInputStartDateID: null,
    DateInputEndDateID: null,
    DateInputKnowledgeDateID: null,
    hdnMultiTSError: null,
    KnowledgeDateInput: null,
    RADXGridEditableRow: null,
    hdnEditableGridXML: null,
    hdnClickedCell: null,
    ReplaceableDateInput: null,
    RMDateDropDownTSPID: null,
    RMDateDropDownSelectTSPID: null,
    RMDateDropDownCustomDivTSPID: null,
    RMDateDropDownTextTSPID: null,
    rdBetweenDatesID: null,
    rdFromDateID: null,
    rdPriorToDateID: null,
    rdAll: null,
    TimeSeriesPanelEffectiveFromDate: null,
    TimeSeriesPanelEffectiveEndDate: null,
    EndDateLabel: null,
    StartDateLabel: null,
    RefreshButton: null,
    knowledgeDateLable: null,
    KnowledgeDateTimeSeries: null,
    StartDateTimeSeries: null,
    EndDateTimeSeries: null,
    UserValueDateInput: null,
    btnConfirmUpdateTS: null,
    btnCancelUpdateTS: null,
    RMDateDdlAsOfForAudit: null,
    RMDateDdlKDForAudit: null,
    RMDateDdlAsOfForTimeSeries: null,
    RMDateDdlKDForTimeSeries: null,
    RMDateDdlAsOfForAttrAudit: null,
    RMDateDdlKDForAttrAudit: null,
    AuditContainerId: null,
    HdnStringKnowledgeDate: null,
    HdnStringAsofDate: null,
    HdnStartAsofDate: null,
    HdnEndAsofDate: null,
    HdnStartKnowledgeDate: null,
    HdnEndKnowledgeDate: null,
    HdnSelectedOptionAsofDate: null,
    HdnRdbSelectedAsofDate: null,
    HdnSelectedOptionKnowledgeDate: null,
    HdnRdbSelectedKnowledgeDate: null,
    HdnStringKnowledgeDateTS: null,
    HdnStringAsofDateTS: null,
    HdnStartAsofDateTS: null,
    HdnEndAsofDateTS: null,
    HdnStartKnowledgeDateTS: null,
    HdnEndKnowledgeDateTS: null,
    HdnSelectedOptionAsofDateTS: null,
    HdnRdbSelectedAsofDateTS: null,
    HdnSelectedOptionKnowledgeDateTS: null,
    HdnRdbSelectedKnowledgeDateTS: null,
    AuditHistoryTab: null,
    TimeSeriesTab: null,
    AuditHistoryGrid: null,
    TimeSeriesGrid: null,
    GridTabContainer: null,
    UserValueFileInput: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.AuditSearchControls

com.ivp.refmaster.scripts.common.AuditSearchControls = function com_ivp_refmaster_scripts_common_AuditSearchControls(AuditSearchControlIdsObject) {
    /// <param name="AuditSearchControlIdsObject" type="com.ivp.refmaster.scripts.common.AuditSearchControlIds">
    /// </param>
    /// <field name="_auditSearchControlIdsObj" type="com.ivp.refmaster.scripts.common.AuditSearchControlIds">
    /// </field>
    /// <field name="_dateDropDown" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDownCustomDiv" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDownText" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDownKdText" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateSelect" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDownTsp" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDownCustomDivTsp" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDownTextTsp" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDownTextKdTsp" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateSelectTsp" type="Object" domElement="true">
    /// </field>
    this._auditSearchControlIdsObj = AuditSearchControlIdsObject;
}
com.ivp.refmaster.scripts.common.AuditSearchControls.prototype = {
    _auditSearchControlIdsObj: null,
    _dateDropDown: null,
    _dateDropDownCustomDiv: null,
    _dateDropDownText: null,
    _dateDropDownKdText: null,
    _dateSelect: null,
    _dateDropDownTsp: null,
    _dateDropDownCustomDivTsp: null,
    _dateDropDownTextTsp: null,
    _dateDropDownTextKdTsp: null,
    _dateSelectTsp: null,
    
    get_hdnSelectedOptionAsofDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnSelectedOptionAsofDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnSelectedOptionAsofDate);
    },
    
    get_hdnRdbSelectedAsofDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnRdbSelectedAsofDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnRdbSelectedAsofDate);
    },
    
    get_hdnSelectedOptionKnowledgeDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnSelectedOptionKnowledgeDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnSelectedOptionKnowledgeDate);
    },
    
    get_hdnRdbSelectedKnowledgeDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnRdbSelectedKnowledgeDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnRdbSelectedKnowledgeDate);
    },
    
    get_hdnMultiInfoTSError: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnMultiInfoTSError() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.hdnMultiTSError);
    },
    
    get_hdnSelectedOptionAsofDateTS: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnSelectedOptionAsofDateTS() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnSelectedOptionAsofDateTS);
    },
    
    get_hdnRdbSelectedAsofDateTS: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnRdbSelectedAsofDateTS() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnRdbSelectedAsofDateTS);
    },
    
    get_hdnSelectedOptionKnowledgeDateTS: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnSelectedOptionKnowledgeDateTS() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnSelectedOptionKnowledgeDateTS);
    },
    
    get_hdnRdbSelectedKnowledgeDateTS: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnRdbSelectedKnowledgeDateTS() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnRdbSelectedKnowledgeDateTS);
    },
    
    get_hdnStringKnowledgeDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnStringKnowledgeDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnStringKnowledgeDate);
    },
    
    get_hdnStringAsofDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnStringAsofDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnStringAsofDate);
    },
    
    get_hdnStartAsofDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnStartAsofDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnStartAsofDate);
    },
    
    get_hdnEndAsofDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnEndAsofDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnEndAsofDate);
    },
    
    get_hdnStartKnowledgeDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnStartKnowledgeDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnStartKnowledgeDate);
    },
    
    get_hdnEndKnowledgeDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnEndKnowledgeDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnEndKnowledgeDate);
    },
    
    get_hdnStringKnowledgeDateTS: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnStringKnowledgeDateTS() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnStringKnowledgeDateTS);
    },
    
    get_hdnStringAsofDateTS: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnStringAsofDateTS() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnStringAsofDateTS);
    },
    
    get_hdnStartAsofDateTS: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnStartAsofDateTS() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnStartAsofDateTS);
    },
    
    get_hdnEndAsofDateTS: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnEndAsofDateTS() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnEndAsofDateTS);
    },
    
    get_hdnStartKnowledgeDateTS: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnStartKnowledgeDateTS() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnStartKnowledgeDateTS);
    },
    
    get_hdnEndKnowledgeDateTS: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnEndKnowledgeDateTS() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnEndKnowledgeDateTS);
    },
    
    get_auditContainerId: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_auditContainerId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.AuditContainerId);
    },
    
    get_rmDateDdlAsOfForAudit: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rmDateDdlAsOfForAudit() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.RMDateDdlAsOfForAudit);
    },
    
    get_rmDateDdlKDForAudit: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rmDateDdlKDForAudit() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.RMDateDdlKDForAudit);
    },
    
    get_rmDateDdlAsOfForTimeSeries: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rmDateDdlAsOfForTimeSeries() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.RMDateDdlAsOfForTimeSeries);
    },
    
    get_rmDateDdlKDForTimeSeries: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rmDateDdlKDForTimeSeries() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.RMDateDdlKDForTimeSeries);
    },
    
    get_rmDateDdlAsOfForAttrAudit: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rmDateDdlAsOfForAttrAudit() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.RMDateDdlAsOfForAttrAudit);
    },
    
    get_rmDateDdlKDForAttrAudit: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rmDateDdlKDForAttrAudit() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.RMDateDdlKDForAttrAudit);
    },
    
    get_rmDateDropDownCustomDivTSP: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rmDateDropDownCustomDivTSP() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDownCustomDivTsp == null) {
            this._dateDropDownCustomDivTsp = document.getElementById(this._auditSearchControlIdsObj.RMDateDropDownCustomDivTSPID);
        }
        return this._dateDropDownCustomDivTsp;
    },
    
    get_rmDateDropDownTextTSP: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rmDateDropDownTextTSP() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDownTextTsp == null) {
            this._dateDropDownTextTsp = document.getElementById(this._auditSearchControlIdsObj.RMDateDropDownTextTSPID);
        }
        return this._dateDropDownTextTsp;
    },
    
    get_rmDateDropDownSelectTSP: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rmDateDropDownSelectTSP() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateSelectTsp == null) {
            this._dateSelectTsp = document.getElementById(this._auditSearchControlIdsObj.RMDateDropDownSelectTSPID);
        }
        return this._dateSelectTsp;
    },
    
    get_rMdateDropDown: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rMdateDropDown() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDown == null) {
            this._dateDropDown = document.getElementById(this._auditSearchControlIdsObj.RMDateDropDownID);
        }
        return this._dateDropDown;
    },
    
    get_rmDateDropDownCustomDiv: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rmDateDropDownCustomDiv() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDownCustomDiv == null) {
            this._dateDropDownCustomDiv = document.getElementById(this._auditSearchControlIdsObj.RMDateDropDownCustomDivID);
        }
        return this._dateDropDownCustomDiv;
    },
    
    get_rmDateDropDownText: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rmDateDropDownText() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDownText == null) {
            this._dateDropDownText = document.getElementById(this._auditSearchControlIdsObj.RMDateDropDownTextID);
        }
        return this._dateDropDownText;
    },
    
    get_rmDateDropDownSelect: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rmDateDropDownSelect() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateSelect == null) {
            this._dateSelect = document.getElementById(this._auditSearchControlIdsObj.RMDateDropDownSelectID);
        }
        return this._dateSelect;
    },
    
    get_lblStartDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_lblStartDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.LblStartDate);
    },
    
    get_lblEndDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_lblEndDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.LblEndDate);
    },
    
    get_rmBetweenDates: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rmBetweenDates() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.RMBetweenDatesID);
    },
    
    get_rmFromDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rmFromDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.RMFromDateID);
    },
    
    get_rmPriorToDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rmPriorToDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.RMPriorToDateID);
    },
    
    get_hdnClickedCell: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnClickedCell() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.hdnClickedCell);
    },
    
    get_hdnEditableGridXML: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnEditableGridXML() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.hdnEditableGridXML);
    },
    
    get_userValueLookUp: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_userValueLookUp() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.UserValueLookUp);
    },
    
    get_hdnduration: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnduration() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.Hdnduration);
    },
    
    get_hdnTimeSeriesUpdateInfo: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnTimeSeriesUpdateInfo() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnTimeSeriesUpdateInfo);
    },
    
    get_hdnTimeSeriesSearch: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnTimeSeriesSearch() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnTimeSeriesSearch);
    },
    
    get_hdnShowDateFilterWithTS: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnShowDateFilterWithTS() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.hdnShowDateFilterWithTS);
    },
    
    get_hdnRadioButtonStateAuditSearch: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_hdnRadioButtonStateAuditSearch() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.HdnRadioButtonStateAuditSearch);
    },
    
    get_updateButton: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_updateButton() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.UpdateButton);
    },
    
    get_btnConfirmUpdateTS: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_btnConfirmUpdateTS() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.btnConfirmUpdateTS);
    },
    
    get_rdBetweenDates: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rdBetweenDates() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.rdBetweenDatesID);
    },
    
    get_rdFromDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rdFromDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.rdFromDateID);
    },
    
    get_rdPriorToDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rdPriorToDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.rdPriorToDateID);
    },
    
    get_viewMoreButton: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_viewMoreButton() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.ViewMoreButton);
    },
    
    get_btnRefreshAttrAudit: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_btnRefreshAttrAudit() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.BtnRefreshAttrAudit);
    },
    
    get_btnRefreshAttrTS: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_btnRefreshAttrTS() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.BtnRefreshAttrTS);
    },
    
    get_startDateInput: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_startDateInput() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.StartDateInput);
    },
    
    get_endDateInput: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_endDateInput() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.EndDateInput);
    },
    
    get_knowledgeDateInput: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_knowledgeDateInput() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.KnowledgeDateInput);
    },
    
    get_userValueDateInput: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_userValueDateInput() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.UserValueDateInput);
    },
    
    get_startDateTimeSeries: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_startDateTimeSeries() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.StartDateTimeSeries);
    },
    
    get_endDateTimeSeries: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_endDateTimeSeries() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.EndDateTimeSeries);
    },
    
    get_errorDiv: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_errorDiv() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('divError');
    },
    
    get_radxGridEditableRow: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_radxGridEditableRow() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.RADXGridEditableRow);
    },
    
    get_replaceableDateInput: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_replaceableDateInput() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.ReplaceableDateInput);
    },
    
    get_rdBetweenDates: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rdBetweenDates() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.rdBetweenDatesID);
    },
    
    get_rdFromDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rdFromDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.rdFromDateID);
    },
    
    get_rdPriorToDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rdPriorToDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.rdPriorToDateID);
    },
    
    get_rdAll: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_rdAll() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.rdAll);
    },
    
    get_effectiveFromDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_effectiveFromDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.TimeSeriesPanelEffectiveFromDate);
    },
    
    get_effectiveEndDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_effectiveEndDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.TimeSeriesPanelEffectiveEndDate);
    },
    
    get_knowledgeDate: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_knowledgeDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.KnowledgeDateTimeSeries);
    },
    
    get_endDateLabel: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_endDateLabel() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.EndDateLabel);
    },
    
    get_startDateLabel: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_startDateLabel() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.StartDateLabel);
    },
    
    get_knowledgeDateLable: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_knowledgeDateLable() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.knowledgeDateLable);
    },
    
    get_refreshButton: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_refreshButton() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.RefreshButton);
    },
    
    get_divErrorTimeSeriesPanel: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_divErrorTimeSeriesPanel() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('divErrorTimeSeriesPanel');
    },
    
    get_divErrorAuditPanel: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_divErrorAuditPanel() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('divErorAudit');
    },
    
    get_divErrorAttrAuditPanel: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_divErrorAttrAuditPanel() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('divErorAttrAudit');
    },
    
    get_auditHistoryTab: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_auditHistoryTab() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.AuditHistoryTab);
    },
    
    get_timeSeriesTab: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_timeSeriesTab() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.TimeSeriesTab);
    },
    
    get_gridTabContainer: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_gridTabContainer() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.GridTabContainer);
    },
    
    get_auditHistoryGrid: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_auditHistoryGrid() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.AuditHistoryGrid);
    },
    
    get_timeSeriesGrid: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_timeSeriesGrid() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.TimeSeriesGrid);
    },
    
    get_userValueFileInput: function com_ivp_refmaster_scripts_common_AuditSearchControls$get_userValueFileInput() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._auditSearchControlIdsObj.UserValueFileInput);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSBulkEntityCreateUpdateInfo

com.ivp.refmaster.scripts.common.RMSBulkEntityCreateUpdateInfo = function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateInfo() {
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSBulkEntityCreateUpdateControlInfo

com.ivp.refmaster.scripts.common.RMSBulkEntityCreateUpdateControlInfo = function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControlInfo() {
    /// <field name="StartProcessButton" type="String">
    /// </field>
    /// <field name="HiddenFilePath" type="String">
    /// </field>
    /// <field name="TaskStartDate" type="String">
    /// </field>
    /// <field name="TaskEndDate" type="String">
    /// </field>
    /// <field name="TaskStartDateLabel" type="String">
    /// </field>
    /// <field name="TaskEndDateLabel" type="String">
    /// </field>
    /// <field name="rdFromDate" type="String">
    /// </field>
    /// <field name="rdPriorToDate" type="String">
    /// </field>
    /// <field name="rdBetweenDates" type="String">
    /// </field>
    /// <field name="HdnEntityType" type="String">
    /// </field>
    /// <field name="insert_radio" type="String">
    /// </field>
    /// <field name="update_radio" type="String">
    /// </field>
    /// <field name="both_radio" type="String">
    /// </field>
    /// <field name="UniqueAttributeDDL" type="String">
    /// </field>
    /// <field name="ChildAttributeDDL" type="String">
    /// </field>
    /// <field name="ParentUniqueAttribDDL" type="String">
    /// </field>
    /// <field name="btnGetTasks" type="String">
    /// </field>
    /// <field name="EntityTypeDDL" type="String">
    /// </field>
    /// <field name="RdFromDateID" type="String">
    /// </field>
    /// <field name="RdPriorToDateID" type="String">
    /// </field>
    /// <field name="RdBetweenDatesID" type="String">
    /// </field>
    /// <field name="RMDateDropDownID" type="String">
    /// </field>
    /// <field name="RMDateDropDownCustomDivID" type="String">
    /// </field>
    /// <field name="RMDateDropDownTextID" type="String">
    /// </field>
    /// <field name="RMDateDropDownSelectID" type="String">
    /// </field>
    /// <field name="LblStartDate" type="String">
    /// </field>
    /// <field name="LblEndDate" type="String">
    /// </field>
    /// <field name="DateInputStartDateID" type="String">
    /// </field>
    /// <field name="DateInputEndDateID" type="String">
    /// </field>
    /// <field name="DdlTaskType" type="String">
    /// </field>
    /// <field name="GetTaskStausBtn" type="String">
    /// </field>
    /// <field name="PanelFailure" type="String">
    /// </field>
    /// <field name="LblFailure" type="String">
    /// </field>
    /// <field name="VeiwLogDetails" type="String">
    /// </field>
    /// <field name="HdnClickedRecord" type="String">
    /// </field>
    /// <field name="ModalDeletePopup" type="String">
    /// </field>
    /// <field name="HdnCommandName" type="String">
    /// </field>
    /// <field name="HdnRecordId" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSBulkEntityCreateUpdateControlInfo.prototype = {
    StartProcessButton: null,
    HiddenFilePath: null,
    TaskStartDate: null,
    TaskEndDate: null,
    TaskStartDateLabel: null,
    TaskEndDateLabel: null,
    rdFromDate: null,
    rdPriorToDate: null,
    rdBetweenDates: null,
    HdnEntityType: null,
    insert_radio: null,
    update_radio: null,
    both_radio: null,
    UniqueAttributeDDL: null,
    ChildAttributeDDL: null,
    ParentUniqueAttribDDL: null,
    btnGetTasks: null,
    EntityTypeDDL: null,
    RdFromDateID: null,
    RdPriorToDateID: null,
    RdBetweenDatesID: null,
    RMDateDropDownID: null,
    RMDateDropDownCustomDivID: null,
    RMDateDropDownTextID: null,
    RMDateDropDownSelectID: null,
    LblStartDate: null,
    LblEndDate: null,
    DateInputStartDateID: null,
    DateInputEndDateID: null,
    DdlTaskType: null,
    GetTaskStausBtn: null,
    PanelFailure: null,
    LblFailure: null,
    VeiwLogDetails: null,
    HdnClickedRecord: null,
    ModalDeletePopup: null,
    HdnCommandName: null,
    HdnRecordId: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSBulkEntityCreateUpdateControl

com.ivp.refmaster.scripts.common.RMSBulkEntityCreateUpdateControl = function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl(ControlIdInfo) {
    /// <param name="ControlIdInfo" type="com.ivp.refmaster.scripts.common.RMSBulkEntityCreateUpdateControlInfo">
    /// </param>
    /// <field name="_controlIds" type="com.ivp.refmaster.scripts.common.RMSBulkEntityCreateUpdateControlInfo">
    /// </field>
    /// <field name="_rdFromDate" type="Object" domElement="true">
    /// </field>
    /// <field name="_rdPriorToDate" type="Object" domElement="true">
    /// </field>
    /// <field name="_rdBetweenDates" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDown" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDownCustomDiv" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDownText" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDownSelect" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateInputStartDate" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateInputEndDate" type="Object" domElement="true">
    /// </field>
    this._controlIds = ControlIdInfo;
}
com.ivp.refmaster.scripts.common.RMSBulkEntityCreateUpdateControl.prototype = {
    _controlIds: null,
    _rdFromDate: null,
    _rdPriorToDate: null,
    _rdBetweenDates: null,
    _dateDropDown: null,
    _dateDropDownCustomDiv: null,
    _dateDropDownText: null,
    _dateDropDownSelect: null,
    _dateInputStartDate: null,
    _dateInputEndDate: null,
    
    get_panelFailure: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_panelFailure() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.PanelFailure);
    },
    
    get_lblFailure: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_lblFailure() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.LblFailure);
    },
    
    get_ddlTaskType: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_ddlTaskType() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.DdlTaskType);
    },
    
    get_getTaskStausBtn: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_getTaskStausBtn() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.GetTaskStausBtn);
    },
    
    get_rMdateDropDown: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_rMdateDropDown() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDown == null) {
            this._dateDropDown = document.getElementById(this._controlIds.RMDateDropDownID);
        }
        return this._dateDropDown;
    },
    
    get_rmDateDropDownCustomDiv: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_rmDateDropDownCustomDiv() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDownCustomDiv == null) {
            this._dateDropDownCustomDiv = document.getElementById(this._controlIds.RMDateDropDownCustomDivID);
        }
        return this._dateDropDownCustomDiv;
    },
    
    get_rmDateDropDownText: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_rmDateDropDownText() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDownText == null) {
            this._dateDropDownText = document.getElementById(this._controlIds.RMDateDropDownTextID);
        }
        return this._dateDropDownText;
    },
    
    get_rmDateDropDownSelect: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_rmDateDropDownSelect() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDownSelect == null) {
            this._dateDropDownSelect = document.getElementById(this._controlIds.RMDateDropDownSelectID);
        }
        return this._dateDropDownSelect;
    },
    
    get_rdFromDate: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_rdFromDate() {
        /// <value type="Object" domElement="true"></value>
        if (this._rdFromDate == null) {
            this._rdFromDate = document.getElementById(this._controlIds.RdFromDateID);
        }
        return this._rdFromDate;
    },
    
    get_rdPriorToDate: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_rdPriorToDate() {
        /// <value type="Object" domElement="true"></value>
        if (this._rdPriorToDate == null) {
            this._rdPriorToDate = document.getElementById(this._controlIds.RdPriorToDateID);
        }
        return this._rdPriorToDate;
    },
    
    get_rdBetweenDates: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_rdBetweenDates() {
        /// <value type="Object" domElement="true"></value>
        if (this._rdBetweenDates == null) {
            this._rdBetweenDates = document.getElementById(this._controlIds.RdBetweenDatesID);
        }
        return this._rdBetweenDates;
    },
    
    get_lblStartDate: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_lblStartDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.LblStartDate);
    },
    
    get_lblEndDate: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_lblEndDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.LblEndDate);
    },
    
    get_dateInputStartDate: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_dateInputStartDate() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateInputStartDate == null) {
            this._dateInputStartDate = document.getElementById(this._controlIds.DateInputStartDateID);
        }
        return this._dateInputStartDate;
    },
    
    get_dateInputEndDate: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_dateInputEndDate() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateInputEndDate == null) {
            this._dateInputEndDate = document.getElementById(this._controlIds.DateInputEndDateID);
        }
        return this._dateInputEndDate;
    },
    
    get_startProcessButton: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_startProcessButton() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.StartProcessButton);
    },
    
    get_hiddenFilePath: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_hiddenFilePath() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HiddenFilePath);
    },
    
    get_taskStartDate: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_taskStartDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TaskStartDate);
    },
    
    get_taskEndDate: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_taskEndDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TaskEndDate);
    },
    
    get_taskEndDateLabel: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_taskEndDateLabel() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TaskEndDateLabel);
    },
    
    get_taskStartDateLabel: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_taskStartDateLabel() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TaskStartDateLabel);
    },
    
    get_rdBetweenDates: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_rdBetweenDates() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.rdBetweenDates);
    },
    
    get_rdFromDate: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_rdFromDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.rdFromDate);
    },
    
    get_rdPriorToDate: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_rdPriorToDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.rdPriorToDate);
    },
    
    get_hdnEntityType: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_hdnEntityType() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnEntityType);
    },
    
    get_insert_radio: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_insert_radio() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.insert_radio);
    },
    
    get_update_radio: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_update_radio() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.update_radio);
    },
    
    get_both_radio: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_both_radio() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.both_radio);
    },
    
    get_uniqueAttributeDDL: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_uniqueAttributeDDL() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.UniqueAttributeDDL);
    },
    
    get_taskConfigurationErrorDiv: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_taskConfigurationErrorDiv() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('error_div');
    },
    
    get_taskStatusErrorDiv: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_taskStatusErrorDiv() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('task_error_div');
    },
    
    get_btnGetTasks: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_btnGetTasks() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.btnGetTasks);
    },
    
    get_uniqueAttributeDDLMandatory: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_uniqueAttributeDDLMandatory() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('UniqueAttributeDDLMandatory');
    },
    
    get_entityTypeDDL: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_entityTypeDDL() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.EntityTypeDDL);
    },
    
    get_veiwLogDetails: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_veiwLogDetails() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.VeiwLogDetails);
    },
    
    get_hdnClickedRecord: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_hdnClickedRecord() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnClickedRecord);
    },
    
    get_modalDeletePopup: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_modalDeletePopup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.ModalDeletePopup);
    },
    
    get_hdnCommandName: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_hdnCommandName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnCommandName);
    },
    
    get_hdnRecordId: function com_ivp_refmaster_scripts_common_RMSBulkEntityCreateUpdateControl$get_hdnRecordId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnRecordId);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSEntityCreationInfo

com.ivp.refmaster.scripts.common.RMSEntityCreationInfo = function com_ivp_refmaster_scripts_common_RMSEntityCreationInfo() {
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.EntityCreationControlIds

com.ivp.refmaster.scripts.common.EntityCreationControlIds = function com_ivp_refmaster_scripts_common_EntityCreationControlIds() {
    /// <field name="BtnSave" type="String">
    /// </field>
    /// <field name="PanelClient" type="String">
    /// </field>
    /// <field name="HdnvalueXML" type="String">
    /// </field>
    /// <field name="EntityDisplayNameList" type="String">
    /// </field>
    /// <field name="SelectDerivedClicked" type="String">
    /// </field>
    /// <field name="lblShowErrorDetails" type="String">
    /// </field>
    /// <field name="modalErrorDetails" type="String">
    /// </field>
    /// <field name="radioLst" type="String">
    /// </field>
    /// <field name="CloseFileUploadButton" type="String">
    /// </field>
    /// <field name="CurrentFileAttribute" type="String">
    /// </field>
    /// <field name="HdnDataFromExternal" type="String">
    /// </field>
    /// <field name="HdnComplexEntityCode" type="String">
    /// </field>
    /// <field name="ddlPreferenceName" type="String">
    /// </field>
    /// <field name="btnHitSecurityAPI" type="String">
    /// </field>
    /// <field name="HdnDBValueDictionary" type="String">
    /// </field>
    /// <field name="HdnCopiedOverrides" type="String">
    /// </field>
    /// <field name="HdnRuleExecutionStatusInfo" type="String">
    /// </field>
    /// <field name="HdnModifiedAttribute" type="String">
    /// </field>
    /// <field name="HdnLockedAttribute" type="String">
    /// </field>
    /// <field name="HdnUnLockedAttribute" type="String">
    /// </field>
    /// <field name="HdnDupeLockedAttribute" type="String">
    /// </field>
    /// <field name="HdnMasterEntityCodeForSession" type="String">
    /// </field>
    /// <field name="BtnRefresh" type="String">
    /// </field>
    /// <field name="RadioLstAsOfDate" type="String">
    /// </field>
    /// <field name="RadioLstAsofKnowledgeDate" type="String">
    /// </field>
    /// <field name="BtnPostEntity" type="String">
    /// </field>
    /// <field name="HdnReportSystemId" type="String">
    /// </field>
    /// <field name="BtnOkPostEntity" type="String">
    /// </field>
    /// <field name="LblAddSuccess" type="String">
    /// </field>
    /// <field name="PostentityHeader" type="String">
    /// </field>
    /// <field name="PostEntitySuccessHeader" type="String">
    /// </field>
    /// <field name="HdnFileSessionUniqueKey" type="String">
    /// </field>
    /// <field name="BtnPreview" type="String">
    /// </field>
    /// <field name="AccHead" type="String">
    /// </field>
    /// <field name="EffectiveDateForCreation" type="String">
    /// </field>
    /// <field name="TableDateControl" type="String">
    /// </field>
    /// <field name="RdBtntransferToStatusScreen" type="String">
    /// </field>
    /// <field name="RdBtnContinueWithUpdateEntity" type="String">
    /// </field>
    /// <field name="BtnClosePostEntity" type="String">
    /// </field>
    /// <field name="BtnViewHierarchy" type="String">
    /// </field>
    /// <field name="BtnViewCurve" type="String">
    /// </field>
    /// <field name="HrcEntityTypeId" type="String">
    /// </field>
    /// <field name="HrcEntityCode" type="String">
    /// </field>
    /// <field name="HrcEntityTypeName" type="String">
    /// </field>
    /// <field name="HdnConditionalFilterEntityType" type="String">
    /// </field>
    /// <field name="ModalParentLookupPopup" type="String">
    /// </field>
    /// <field name="IframeParentLookupPopup" type="String">
    /// </field>
    /// <field name="BtnParentClose" type="String">
    /// </field>
    /// <field name="TxtInstrument" type="String">
    /// </field>
    /// <field name="ChkBackground" type="String">
    /// </field>
    /// <field name="BtnAPI" type="String">
    /// </field>
    /// <field name="IsEditLegPrivilege" type="Boolean">
    /// </field>
    /// <field name="ViewMode" type="Boolean">
    /// </field>
    /// <field name="IsDeleteLegPrivilege" type="Boolean">
    /// </field>
    /// <field name="RdbContinueWithSameEntity" type="String">
    /// </field>
    /// <field name="divOverridePopupId" type="String">
    /// </field>
    /// <field name="btnGetOverridesId" type="String">
    /// </field>
    /// <field name="divCountOverridesId" type="String">
    /// </field>
    /// <field name="HdnOverridenAttributeListId" type="String">
    /// </field>
    /// <field name="LoginName" type="String">
    /// </field>
    /// <field name="HdnBtnPreviewClicked" type="String">
    /// </field>
    /// <field name="HdnShortDateFormat" type="String">
    /// </field>
    /// <field name="ModalDeletePopupBehaviorId" type="String">
    /// </field>
    /// <field name="IframeDeletionPopup" type="String">
    /// </field>
    /// <field name="HdnSelectedEntityType" type="String">
    /// </field>
    /// <field name="HdnConditionalFilterAttribueRealNames" type="String">
    /// </field>
    /// <field name="ViewHierarchyList" type="String">
    /// </field>
    /// <field name="EntityTypeLookupReferencesList" type="String">
    /// </field>
    /// <field name="IframeEntityReferences" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.EntityCreationControlIds.prototype = {
    BtnSave: null,
    PanelClient: null,
    HdnvalueXML: null,
    EntityDisplayNameList: null,
    SelectDerivedClicked: null,
    lblShowErrorDetails: null,
    modalErrorDetails: null,
    radioLst: null,
    CloseFileUploadButton: null,
    CurrentFileAttribute: null,
    HdnDataFromExternal: null,
    HdnComplexEntityCode: null,
    ddlPreferenceName: null,
    btnHitSecurityAPI: null,
    HdnDBValueDictionary: null,
    HdnCopiedOverrides: null,
    HdnRuleExecutionStatusInfo: null,
    HdnModifiedAttribute: null,
    HdnLockedAttribute: null,
    HdnUnLockedAttribute: null,
    HdnDupeLockedAttribute: null,
    HdnMasterEntityCodeForSession: null,
    BtnRefresh: null,
    RadioLstAsOfDate: null,
    RadioLstAsofKnowledgeDate: null,
    BtnPostEntity: null,
    HdnReportSystemId: null,
    BtnOkPostEntity: null,
    LblAddSuccess: null,
    PostentityHeader: null,
    PostEntitySuccessHeader: null,
    HdnFileSessionUniqueKey: null,
    BtnPreview: null,
    AccHead: null,
    EffectiveDateForCreation: null,
    TableDateControl: null,
    RdBtntransferToStatusScreen: null,
    RdBtnContinueWithUpdateEntity: null,
    BtnClosePostEntity: null,
    BtnViewHierarchy: null,
    BtnViewCurve: null,
    HrcEntityTypeId: null,
    HrcEntityCode: null,
    HrcEntityTypeName: null,
    HdnConditionalFilterEntityType: null,
    ModalParentLookupPopup: null,
    IframeParentLookupPopup: null,
    BtnParentClose: null,
    TxtInstrument: null,
    ChkBackground: null,
    BtnAPI: null,
    IsEditLegPrivilege: false,
    ViewMode: false,
    IsDeleteLegPrivilege: false,
    RdbContinueWithSameEntity: null,
    divOverridePopupId: null,
    btnGetOverridesId: null,
    divCountOverridesId: null,
    HdnOverridenAttributeListId: null,
    LoginName: null,
    HdnBtnPreviewClicked: null,
    HdnShortDateFormat: null,
    ModalDeletePopupBehaviorId: null,
    IframeDeletionPopup: null,
    HdnSelectedEntityType: null,
    HdnConditionalFilterAttribueRealNames: null,
    ViewHierarchyList: null,
    EntityTypeLookupReferencesList: null,
    IframeEntityReferences: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.EntityCreationControls

com.ivp.refmaster.scripts.common.EntityCreationControls = function com_ivp_refmaster_scripts_common_EntityCreationControls(EntityCreationControlIdsInArgument) {
    /// <param name="EntityCreationControlIdsInArgument" type="com.ivp.refmaster.scripts.common.EntityCreationControlIds">
    /// </param>
    /// <field name="_entityCreationControlIdsObj" type="com.ivp.refmaster.scripts.common.EntityCreationControlIds">
    /// </field>
    this._entityCreationControlIdsObj = EntityCreationControlIdsInArgument;
}
com.ivp.refmaster.scripts.common.EntityCreationControls.prototype = {
    _entityCreationControlIdsObj: null,
    
    get_hdnConditionalFilterEntityType: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnConditionalFilterEntityType() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnConditionalFilterEntityType);
    },
    
    get_btnParentClose: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_btnParentClose() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.BtnParentClose);
    },
    
    get_modalParentLookupPopup: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_modalParentLookupPopup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.ModalParentLookupPopup);
    },
    
    get_iframeParentLookupPopup: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_iframeParentLookupPopup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.IframeParentLookupPopup);
    },
    
    get_btnSave: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_btnSave() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.BtnSave);
    },
    
    get_hrcEntityTypeId: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hrcEntityTypeId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HrcEntityTypeId);
    },
    
    get_hrcEntityCode: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hrcEntityCode() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HrcEntityCode);
    },
    
    get_hrcEntityTypeName: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hrcEntityTypeName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HrcEntityTypeName);
    },
    
    get_btnViewHierarchy: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_btnViewHierarchy() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.BtnViewHierarchy);
    },
    
    get_btnViewCurve: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_btnViewCurve() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.BtnViewCurve);
    },
    
    get_panelClient: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_panelClient() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.PanelClient);
    },
    
    get_hdnvalueXML: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnvalueXML() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnvalueXML);
    },
    
    get_entityDisplayNameList: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_entityDisplayNameList() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.EntityDisplayNameList);
    },
    
    get_selectDerivedClicked: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_selectDerivedClicked() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.SelectDerivedClicked);
    },
    
    get_lblShowErrorDetails: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_lblShowErrorDetails() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.lblShowErrorDetails);
    },
    
    get_modalErrorDetails: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_modalErrorDetails() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.modalErrorDetails);
    },
    
    get_radioLst: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_radioLst() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.radioLst);
    },
    
    get_closeFileUploadButton: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_closeFileUploadButton() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.CloseFileUploadButton);
    },
    
    get_currentFileAttribute: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_currentFileAttribute() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.CurrentFileAttribute);
    },
    
    get_hdnDataFromExternal: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnDataFromExternal() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnDataFromExternal);
    },
    
    get_hdnComplexEntityCode: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnComplexEntityCode() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnComplexEntityCode);
    },
    
    get_ddlPreferenceName: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_ddlPreferenceName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.ddlPreferenceName);
    },
    
    get_btnHitSecurityAPI: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_btnHitSecurityAPI() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.btnHitSecurityAPI);
    },
    
    get_hdnDBValueDictionary: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnDBValueDictionary() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnDBValueDictionary);
    },
    
    get_hdnCopiedOverrides: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnCopiedOverrides() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnCopiedOverrides);
    },
    
    get_hdnRuleExecutionStatusInfo: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnRuleExecutionStatusInfo() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnRuleExecutionStatusInfo);
    },
    
    get_hdnModifiedAttribute: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnModifiedAttribute() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnModifiedAttribute);
    },
    
    get_hdnLockedAttribute: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnLockedAttribute() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnLockedAttribute);
    },
    
    get_hdnUnLockedAttribute: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnUnLockedAttribute() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnUnLockedAttribute);
    },
    
    get_hdnDupeLockedAttribute: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnDupeLockedAttribute() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnDupeLockedAttribute);
    },
    
    get_hdnMasterEntityCodeForSession: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnMasterEntityCodeForSession() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnMasterEntityCodeForSession);
    },
    
    get_btnRefresh: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_btnRefresh() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.BtnRefresh);
    },
    
    get_radioLstAsOfDate: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_radioLstAsOfDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.RadioLstAsOfDate);
    },
    
    get_radioLstAsofKnowledgeDate: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_radioLstAsofKnowledgeDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.RadioLstAsofKnowledgeDate);
    },
    
    get_btnPostEntity: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_btnPostEntity() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.BtnPostEntity);
    },
    
    get_postEntityExternalSystem: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_postEntityExternalSystem() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('postEntityExternalSystem');
    },
    
    get_hdnReportSystemId: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnReportSystemId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnReportSystemId);
    },
    
    get_btnOkPostEntity: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_btnOkPostEntity() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.BtnOkPostEntity);
    },
    
    get_rdBtntransferToStatusScreen: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_rdBtntransferToStatusScreen() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.RdBtntransferToStatusScreen);
    },
    
    get_rdBtnContinueWithUpdateEntity: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_rdBtnContinueWithUpdateEntity() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.RdBtnContinueWithUpdateEntity);
    },
    
    get_rdbContinueWithSameEntity: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_rdbContinueWithSameEntity() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.RdbContinueWithSameEntity);
    },
    
    get_lblAddSuccess: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_lblAddSuccess() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.LblAddSuccess);
    },
    
    get_postentityHeader: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_postentityHeader() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.PostentityHeader);
    },
    
    get_postEntitySuccessHeader: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_postEntitySuccessHeader() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.PostEntitySuccessHeader);
    },
    
    get_postEntityErrorDiv: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_postEntityErrorDiv() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('postEntityErrorDiv');
    },
    
    get_effectiveDateForCreation: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_effectiveDateForCreation() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.EffectiveDateForCreation);
    },
    
    get_effectiveDateForCreationHdnErrorMsg: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_effectiveDateForCreationHdnErrorMsg() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.EffectiveDateForCreation + '_HdnErrorMessage');
    },
    
    get_effectiveDateForCreationErrorMsgHolder: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_effectiveDateForCreationErrorMsgHolder() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('effectiveDateErrorMsg');
    },
    
    get_btnPreview: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_btnPreview() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.BtnPreview);
    },
    
    get_hdnFileSessionUniqueKey: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnFileSessionUniqueKey() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnFileSessionUniqueKey);
    },
    
    get_accHead: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_accHead() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.AccHead);
    },
    
    get_tableDateControl: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_tableDateControl() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.TableDateControl);
    },
    
    get_btnClosePostEntity: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_btnClosePostEntity() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.BtnClosePostEntity);
    },
    
    get_txtInstrument: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_txtInstrument() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.TxtInstrument);
    },
    
    get_chkBackground: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_chkBackground() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.ChkBackground);
    },
    
    get_btnAPI: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_btnAPI() {
        /// <value type="Object" domElement="true"></value>
        var Btn = document.getElementById(this._entityCreationControlIdsObj.BtnAPI);
        if (Btn != null) {
            return document.getElementById(this._entityCreationControlIdsObj.BtnAPI);
        }
        else {
            return null;
        }
    },
    
    get_btnGetOverrides: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_btnGetOverrides() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.btnGetOverridesId);
    },
    
    get_divOverridePopup: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_divOverridePopup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.divOverridePopupId);
    },
    
    get_divCountOverrides: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_divCountOverrides() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.divCountOverridesId);
    },
    
    get_hdnOverridenAttributeList: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnOverridenAttributeList() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnOverridenAttributeListId);
    },
    
    get_hdnBtnPreviewClicked: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnBtnPreviewClicked() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnBtnPreviewClicked);
    },
    
    get_hdnShortDateFormat: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnShortDateFormat() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnShortDateFormat);
    },
    
    get_modalDeletePopupBehaviorId: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_modalDeletePopupBehaviorId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.ModalDeletePopupBehaviorId);
    },
    
    get_iframeDeletionPopup: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_iframeDeletionPopup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.IframeDeletionPopup);
    },
    
    get_hdnSelectedEntityType: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnSelectedEntityType() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnSelectedEntityType);
    },
    
    get_hdnConditionalFilterAttribueRealNames: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_hdnConditionalFilterAttribueRealNames() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.HdnConditionalFilterAttribueRealNames);
    },
    
    get_viewHierarchyList: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_viewHierarchyList() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.ViewHierarchyList);
    },
    
    get_entityTypeLookupReferencesList: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_entityTypeLookupReferencesList() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.EntityTypeLookupReferencesList);
    },
    
    get_iframeEntityReferences: function com_ivp_refmaster_scripts_common_EntityCreationControls$get_iframeEntityReferences() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._entityCreationControlIdsObj.IframeEntityReferences);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMFileAttributeMapInfo

com.ivp.refmaster.scripts.common.RMFileAttributeMapInfo = function com_ivp_refmaster_scripts_common_RMFileAttributeMapInfo() {
    /// <field name="entityAttributeId" type="Number" integer="true">
    /// </field>
    /// <field name="FileName" type="String">
    /// </field>
    /// <field name="FileId" type="Number" integer="true">
    /// </field>
    /// <field name="FileDisplayName" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMFileAttributeMapInfo.prototype = {
    entityAttributeId: 0,
    FileName: null,
    FileId: 0,
    FileDisplayName: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.AttributeValueInfo

com.ivp.refmaster.scripts.common.AttributeValueInfo = function com_ivp_refmaster_scripts_common_AttributeValueInfo() {
    /// <field name="RealName" type="String">
    /// Gets or sets the name of the real.
    /// </field>
    /// <field name="Value" type="String">
    /// Gets or sets the value.
    /// </field>
}
com.ivp.refmaster.scripts.common.AttributeValueInfo.prototype = {
    RealName: null,
    Value: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMRuleExecutionStatusInfo

com.ivp.refmaster.scripts.common.RMRuleExecutionStatusInfo = function com_ivp_refmaster_scripts_common_RMRuleExecutionStatusInfo() {
    /// <field name="AttributeName" type="String">
    /// </field>
    /// <field name="DisplayName" type="String">
    /// </field>
    /// <field name="Message" type="String">
    /// </field>
    /// <field name="Status" type="String">
    /// </field>
    /// <field name="RuleType" type="String">
    /// </field>
    /// <field name="RuleName" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMRuleExecutionStatusInfo.prototype = {
    AttributeName: null,
    DisplayName: null,
    Message: null,
    Status: null,
    RuleType: null,
    RuleName: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMOverridenAttrInfo

com.ivp.refmaster.scripts.common.RMOverridenAttrInfo = function com_ivp_refmaster_scripts_common_RMOverridenAttrInfo() {
    /// <field name="AttributeId" type="Number" integer="true">
    /// </field>
    /// <field name="AttributeLabelId" type="Number" integer="true">
    /// </field>
    /// <field name="EntityCode" type="String">
    /// </field>
    /// <field name="DisplayName" type="String">
    /// </field>
    /// <field name="ExpiresOn" type="String">
    /// </field>
    /// <field name="OverrideCreatedBy" type="String">
    /// </field>
    /// <field name="OverrideCreatedOn" type="Date">
    /// </field>
    /// <field name="AttributeValue" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMOverridenAttrInfo.prototype = {
    AttributeId: 0,
    AttributeLabelId: 0,
    EntityCode: null,
    DisplayName: null,
    ExpiresOn: '',
    OverrideCreatedBy: null,
    OverrideCreatedOn: null,
    AttributeValue: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSEntityUpdationInfo

com.ivp.refmaster.scripts.common.RMSEntityUpdationInfo = function com_ivp_refmaster_scripts_common_RMSEntityUpdationInfo() {
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSEntityUpdationControls

com.ivp.refmaster.scripts.common.RMSEntityUpdationControls = function com_ivp_refmaster_scripts_common_RMSEntityUpdationControls() {
}
com.ivp.refmaster.scripts.common.RMSEntityUpdationControls.prototype = {
    
    get_btnParentClose: function com_ivp_refmaster_scripts_common_RMSEntityUpdationControls$get_btnParentClose() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnParentClose');
    },
    
    get_hdnTabIframeUniqueVal: function com_ivp_refmaster_scripts_common_RMSEntityUpdationControls$get_hdnTabIframeUniqueVal() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('hdnTabIframeUniqueVal');
    },
    
    get_fromDateRadioBtn: function com_ivp_refmaster_scripts_common_RMSEntityUpdationControls$get_fromDateRadioBtn() {
        /// <value type="Object" domElement="true"></value>
        if (document.getElementById('FromDateRadioBtn') != null) {
            return document.getElementById('FromDateRadioBtn');
        }
        else {
            return null;
        }
    },
    
    get_betweenDatesRadioBtn: function com_ivp_refmaster_scripts_common_RMSEntityUpdationControls$get_betweenDatesRadioBtn() {
        /// <value type="Object" domElement="true"></value>
        if (document.getElementById('BetweenDatesRadioBtn') != null) {
            return document.getElementById('BetweenDatesRadioBtn');
        }
        else {
            return null;
        }
    },
    
    get_priorToDateRadioBtn: function com_ivp_refmaster_scripts_common_RMSEntityUpdationControls$get_priorToDateRadioBtn() {
        /// <value type="Object" domElement="true"></value>
        if (document.getElementById('PriorToDateRadioBtn') != null) {
            return document.getElementById('PriorToDateRadioBtn');
        }
        else {
            return null;
        }
    },
    
    get_viewMoreButton: function com_ivp_refmaster_scripts_common_RMSEntityUpdationControls$get_viewMoreButton() {
        /// <value type="Object" domElement="true"></value>
        if (this.get_priorToDateRadioBtn() != null) {
            return document.getElementById('viewMoreTD').childNodes[0];
        }
        else {
            return null;
        }
    },
    
    get_startDateInput: function com_ivp_refmaster_scripts_common_RMSEntityUpdationControls$get_startDateInput() {
        /// <value type="Object" domElement="true"></value>
        if (this.get_viewMoreButton() != null) {
            return document.getElementById('startDateTD').childNodes[0];
        }
        else {
            return null;
        }
    },
    
    get_endDateInput: function com_ivp_refmaster_scripts_common_RMSEntityUpdationControls$get_endDateInput() {
        /// <value type="Object" domElement="true"></value>
        if (this.get_viewMoreButton() != null) {
            return document.getElementById('endDateTD').childNodes[0];
        }
        else {
            return null;
        }
    },
    
    get_errorMsgLabel: function com_ivp_refmaster_scripts_common_RMSEntityUpdationControls$get_errorMsgLabel() {
        /// <value type="Object" domElement="true"></value>
        if (this.get_viewMoreButton() != null) {
            return document.getElementById('divErrorTimeSeriesPanel');
        }
        else {
            return null;
        }
    },
    
    get_knowledgeDateInput: function com_ivp_refmaster_scripts_common_RMSEntityUpdationControls$get_knowledgeDateInput() {
        /// <value type="Object" domElement="true"></value>
        if (this.get_viewMoreButton() != null) {
            return document.getElementById('knowledgeDateTD').childNodes[0];
        }
        else {
            return null;
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSExceptionManagerInfo

com.ivp.refmaster.scripts.common.RMSExceptionManagerInfo = function com_ivp_refmaster_scripts_common_RMSExceptionManagerInfo() {
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.ExceptionManagerUpdateInfo

com.ivp.refmaster.scripts.common.ExceptionManagerUpdateInfo = function com_ivp_refmaster_scripts_common_ExceptionManagerUpdateInfo() {
    /// <field name="exceptionId" type="Number" integer="true">
    /// </field>
    /// <field name="IsSuppressed" type="Boolean">
    /// </field>
    /// <field name="comment" type="String">
    /// </field>
    /// <field name="Value" type="String">
    /// </field>
    /// <field name="EntityTypeId" type="Number" integer="true">
    /// </field>
    /// <field name="EntityAttributeId" type="Number" integer="true">
    /// </field>
    /// <field name="EntityAttributeRealName" type="String">
    /// </field>
    /// <field name="LoginName" type="String">
    /// </field>
    /// <field name="EntityCode" type="String">
    /// </field>
    /// <field name="externalSysIdList" type="Array">
    /// </field>
    /// <field name="ExceptionType" type="Number" integer="true">
    /// </field>
    /// <field name="IsCreateAndUpdate" type="Boolean">
    /// </field>
    this.ExceptionType = -1;
}
com.ivp.refmaster.scripts.common.ExceptionManagerUpdateInfo.prototype = {
    exceptionId: 0,
    IsSuppressed: false,
    comment: null,
    Value: null,
    EntityTypeId: 0,
    EntityAttributeId: 0,
    EntityAttributeRealName: null,
    LoginName: null,
    EntityCode: null,
    externalSysIdList: null,
    IsCreateAndUpdate: false
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSExceptionTypes

com.ivp.refmaster.scripts.common.RMSExceptionTypes = function com_ivp_refmaster_scripts_common_RMSExceptionTypes() {
    /// <field name="ALL" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="comparE_AND_SHOW" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="firsT_VENDOR_VALUE_MISSING" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="nO_VENDOR_DATA_FOUND" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="referencE_DATA_EXCEPTION" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="shoW_AS_EXCEPTION" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="valuE_CHANGED" type="Number" integer="true" static="true">
    /// </field>
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSSuppressDeleteConstants

com.ivp.refmaster.scripts.common.RMSSuppressDeleteConstants = function com_ivp_refmaster_scripts_common_RMSSuppressDeleteConstants() {
    /// <field name="SUPPRESS" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="DELETE" type="Number" integer="true" static="true">
    /// </field>
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSExceptionManagerDSInfo

com.ivp.refmaster.scripts.common.RMSExceptionManagerDSInfo = function com_ivp_refmaster_scripts_common_RMSExceptionManagerDSInfo() {
    /// <field name="DataSourceID" type="Number" integer="true">
    /// </field>
    /// <field name="DataSourceName" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSExceptionManagerDSInfo.prototype = {
    DataSourceID: 0,
    DataSourceName: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSGridColumnIndexInfo

com.ivp.refmaster.scripts.common.RMSGridColumnIndexInfo = function com_ivp_refmaster_scripts_common_RMSGridColumnIndexInfo() {
    /// <field name="ColumnName" type="String">
    /// </field>
    /// <field name="cellIndex" type="Number" integer="true">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSGridColumnIndexInfo.prototype = {
    ColumnName: null,
    cellIndex: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.ExternalSystemInfo

com.ivp.refmaster.scripts.common.ExternalSystemInfo = function com_ivp_refmaster_scripts_common_ExternalSystemInfo() {
    /// <field name="SystemName" type="String">
    /// </field>
    /// <field name="SystemId" type="Number" integer="true">
    /// </field>
}
com.ivp.refmaster.scripts.common.ExternalSystemInfo.prototype = {
    SystemName: '',
    SystemId: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSExceptionManagerControlInfo

com.ivp.refmaster.scripts.common.RMSExceptionManagerControlInfo = function com_ivp_refmaster_scripts_common_RMSExceptionManagerControlInfo() {
    /// <field name="RMDateDropDownID" type="String">
    /// </field>
    /// <field name="RMDateDropDownCustomDivID" type="String">
    /// </field>
    /// <field name="RMDateDropDownTextID" type="String">
    /// </field>
    /// <field name="RMDateDropDownSelectID" type="String">
    /// </field>
    /// <field name="RdBetweenDatesID" type="String">
    /// </field>
    /// <field name="RdFromDateID" type="String">
    /// </field>
    /// <field name="RdPriorToDateID" type="String">
    /// </field>
    /// <field name="LblStartDate" type="String">
    /// </field>
    /// <field name="LblEndDate" type="String">
    /// </field>
    /// <field name="DateInputStartDateID" type="String">
    /// </field>
    /// <field name="DateInputEndDateID" type="String">
    /// </field>
    /// <field name="EntityTypeDDL" type="String">
    /// </field>
    /// <field name="AuditDateRange" type="String">
    /// </field>
    /// <field name="FromDateRadioBtn" type="String">
    /// </field>
    /// <field name="BetweenDatesRadioBtn" type="String">
    /// </field>
    /// <field name="PriorToDateRadioBtn" type="String">
    /// </field>
    /// <field name="StartDateInput" type="String">
    /// </field>
    /// <field name="EndDateInput" type="String">
    /// </field>
    /// <field name="GetExceptionBtn" type="String">
    /// </field>
    /// <field name="DeleteAllExceptionBtn" type="String">
    /// </field>
    /// <field name="hdnSelectedEntityAttribute" type="String">
    /// </field>
    /// <field name="hdnSelectedEntity" type="String">
    /// </field>
    /// <field name="GridContextMenu" type="String">
    /// </field>
    /// <field name="ExceptionGrid" type="String">
    /// </field>
    /// <field name="ExceptionTypeDDL" type="String">
    /// </field>
    /// <field name="ModalUpdate" type="String">
    /// </field>
    /// <field name="btnSaveUpdate" type="String">
    /// </field>
    /// <field name="btnCancelUpdate" type="String">
    /// </field>
    /// <field name="UserValueDateInput" type="String">
    /// </field>
    /// <field name="HdnClickedLinkID" type="String">
    /// </field>
    /// <field name="hdnCreateUpdateEntity" type="String">
    /// </field>
    /// <field name="lblSuccess" type="String">
    /// </field>
    /// <field name="lblFailure" type="String">
    /// </field>
    /// <field name="btnConfirmSupress" type="String">
    /// </field>
    /// <field name="btnConfirmDelete" type="String">
    /// </field>
    /// <field name="hdnGridCacheKey" type="String">
    /// </field>
    /// <field name="SuppressedRadioBtn" type="String">
    /// </field>
    /// <field name="UnsuppressedRadioBtn" type="String">
    /// </field>
    /// <field name="AllRadioBtn" type="String">
    /// </field>
    /// <field name="BtnSuccess" type="String">
    /// </field>
    /// <field name="UpdateEntityPopup" type="String">
    /// </field>
    /// <field name="LoginName" type="String">
    /// </field>
    /// <field name="DivExceptionFilter" type="String">
    /// </field>
    /// <field name="DivExceptionDate" type="String">
    /// </field>
    /// <field name="RMExceptionFilter" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSExceptionManagerControlInfo.prototype = {
    RMDateDropDownID: null,
    RMDateDropDownCustomDivID: null,
    RMDateDropDownTextID: null,
    RMDateDropDownSelectID: null,
    RdBetweenDatesID: null,
    RdFromDateID: null,
    RdPriorToDateID: null,
    LblStartDate: null,
    LblEndDate: null,
    DateInputStartDateID: null,
    DateInputEndDateID: null,
    EntityTypeDDL: null,
    AuditDateRange: null,
    FromDateRadioBtn: null,
    BetweenDatesRadioBtn: null,
    PriorToDateRadioBtn: null,
    StartDateInput: null,
    EndDateInput: null,
    GetExceptionBtn: null,
    DeleteAllExceptionBtn: null,
    hdnSelectedEntityAttribute: null,
    hdnSelectedEntity: null,
    GridContextMenu: null,
    ExceptionGrid: null,
    ExceptionTypeDDL: null,
    ModalUpdate: null,
    btnSaveUpdate: null,
    btnCancelUpdate: null,
    UserValueDateInput: null,
    HdnClickedLinkID: null,
    hdnCreateUpdateEntity: null,
    lblSuccess: null,
    lblFailure: null,
    btnConfirmSupress: null,
    btnConfirmDelete: null,
    hdnGridCacheKey: null,
    SuppressedRadioBtn: null,
    UnsuppressedRadioBtn: null,
    AllRadioBtn: null,
    BtnSuccess: null,
    UpdateEntityPopup: null,
    LoginName: null,
    DivExceptionFilter: null,
    DivExceptionDate: null,
    RMExceptionFilter: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSExceptionManagerControls

com.ivp.refmaster.scripts.common.RMSExceptionManagerControls = function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls(ControlIdInfo) {
    /// <param name="ControlIdInfo" type="com.ivp.refmaster.scripts.common.RMSExceptionManagerControlInfo">
    /// </param>
    /// <field name="_controlIds" type="com.ivp.refmaster.scripts.common.RMSExceptionManagerControlInfo">
    /// </field>
    /// <field name="_dateDropDown" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDownCustomDiv" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDownText" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDownSelect" type="Object" domElement="true">
    /// </field>
    this._controlIds = ControlIdInfo;
}
com.ivp.refmaster.scripts.common.RMSExceptionManagerControls.prototype = {
    _controlIds: null,
    _dateDropDown: null,
    _dateDropDownCustomDiv: null,
    _dateDropDownText: null,
    _dateDropDownSelect: null,
    
    get_rmExceptionFilter: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_rmExceptionFilter() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.RMExceptionFilter);
    },
    
    get_rMdateDropDown: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_rMdateDropDown() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDown == null) {
            this._dateDropDown = document.getElementById(this._controlIds.RMDateDropDownID);
        }
        return this._dateDropDown;
    },
    
    get_rmDateDropDownCustomDiv: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_rmDateDropDownCustomDiv() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDownCustomDiv == null) {
            this._dateDropDownCustomDiv = document.getElementById(this._controlIds.RMDateDropDownCustomDivID);
        }
        return this._dateDropDownCustomDiv;
    },
    
    get_rmDateDropDownText: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_rmDateDropDownText() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDownText == null) {
            this._dateDropDownText = document.getElementById(this._controlIds.RMDateDropDownTextID);
        }
        return this._dateDropDownText;
    },
    
    get_rmDateDropDownSelect: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_rmDateDropDownSelect() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDownSelect == null) {
            this._dateDropDownSelect = document.getElementById(this._controlIds.RMDateDropDownSelectID);
        }
        return this._dateDropDownSelect;
    },
    
    get_rdBetweenDates: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_rdBetweenDates() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.RdBetweenDatesID);
    },
    
    get_rdFromDate: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_rdFromDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.RdFromDateID);
    },
    
    get_rdPriorToDate: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_rdPriorToDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.RdPriorToDateID);
    },
    
    get_lblStartDate: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_lblStartDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.LblStartDate);
    },
    
    get_lblEndDate: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_lblEndDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.LblEndDate);
    },
    
    get_dateInputStartDate: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_dateInputStartDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.DateInputStartDateID);
    },
    
    get_dateInputEndDate: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_dateInputEndDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.DateInputEndDateID);
    },
    
    get_divExceptionFilter: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_divExceptionFilter() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.DivExceptionFilter);
    },
    
    get_divExceptionDate: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_divExceptionDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.DivExceptionDate);
    },
    
    get_entityTypeDDL: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_entityTypeDDL() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.EntityTypeDDL);
    },
    
    get_betweenDatesRadioBtn: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_betweenDatesRadioBtn() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BetweenDatesRadioBtn);
    },
    
    get_endDateInput: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_endDateInput() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.EndDateInput);
    },
    
    get_fromDateRadioBtn: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_fromDateRadioBtn() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.FromDateRadioBtn);
    },
    
    get_priorToDateRadioBtn: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_priorToDateRadioBtn() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.PriorToDateRadioBtn);
    },
    
    get_startDateInput: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_startDateInput() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.StartDateInput);
    },
    
    get_errorWebService: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_errorWebService() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('divError');
    },
    
    get_chkGoToStatusScreen: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_chkGoToStatusScreen() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('ChkGoToStatusScreen');
    },
    
    get_getExceptionBtn: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_getExceptionBtn() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.GetExceptionBtn);
    },
    
    get_deleteAllExceptionBtn: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_deleteAllExceptionBtn() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.DeleteAllExceptionBtn);
    },
    
    get_hdnSelectedEntityAttribute: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_hdnSelectedEntityAttribute() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.hdnSelectedEntityAttribute);
    },
    
    get_hdnSelectedEntity: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_hdnSelectedEntity() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.hdnSelectedEntity);
    },
    
    get_gridContextMenu: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_gridContextMenu() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.GridContextMenu);
    },
    
    get_exceptionGrid: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_exceptionGrid() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.ExceptionGrid);
    },
    
    get_exceptionTypeDDL: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_exceptionTypeDDL() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.ExceptionTypeDDL);
    },
    
    get_modalUpdate: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_modalUpdate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.ModalUpdate);
    },
    
    get_btnSaveUpdate: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_btnSaveUpdate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.btnSaveUpdate);
    },
    
    get_btnCancelUpdate: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_btnCancelUpdate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.btnCancelUpdate);
    },
    
    get_userValueDateInput: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_userValueDateInput() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.UserValueDateInput);
    },
    
    get_suppressCheckBox: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_suppressCheckBox() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('SuppressCheckBox');
    },
    
    get_extSystemChkBox: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_extSystemChkBox() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('ExtSystemChkBox');
    },
    
    get_dynExtSystemDiv: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_dynExtSystemDiv() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('DynExtSystemDiv');
    },
    
    get_hdnClickedLink: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_hdnClickedLink() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnClickedLinkID);
    },
    
    get_userValueAutoExtenderDiv: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_userValueAutoExtenderDiv() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('UserValueAutoExtenderDiv');
    },
    
    get_btnPostBackException: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_btnPostBackException() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnPostBackException');
    },
    
    get_hdnCreateUpdateEntity: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_hdnCreateUpdateEntity() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.hdnCreateUpdateEntity);
    },
    
    get_lblSuccess: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_lblSuccess() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.lblSuccess);
    },
    
    get_lblFailure: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_lblFailure() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.lblFailure);
    },
    
    get_btnConfirmSupress: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_btnConfirmSupress() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.btnConfirmSupress);
    },
    
    get_btnConfirmDelete: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_btnConfirmDelete() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.btnConfirmDelete);
    },
    
    get_txtCommentBox: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_txtCommentBox() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtCommentBox');
    },
    
    get_hdnGridCacheKey: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_hdnGridCacheKey() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.hdnGridCacheKey);
    },
    
    get_suppressedRadioBtn: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_suppressedRadioBtn() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.SuppressedRadioBtn);
    },
    
    get_unsuppressedRadioBtn: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_unsuppressedRadioBtn() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.UnsuppressedRadioBtn);
    },
    
    get_allRadioBtn: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_allRadioBtn() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.AllRadioBtn);
    },
    
    get_btnSuccess: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_btnSuccess() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnSuccess);
    },
    
    get_updateEntityPopup: function com_ivp_refmaster_scripts_common_RMSExceptionManagerControls$get_updateEntityPopup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.UpdateEntityPopup);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMReportAttributeInfo

com.ivp.refmaster.scripts.common.RMReportAttributeInfo = function com_ivp_refmaster_scripts_common_RMReportAttributeInfo() {
    /// <field name="AttributeID" type="Number" integer="true">
    /// </field>
    /// <field name="AttributeName" type="String">
    /// </field>
    /// <field name="AttributeDescription" type="String">
    /// </field>
    /// <field name="AttributeDataType" type="com.ivp.refmaster.scripts.common.RMDBDataTypes">
    /// </field>
    this.AttributeName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.AttributeDescription = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.AttributeDataType = com.ivp.refmaster.scripts.common.RMDBDataTypes.VARCHAR;
}
com.ivp.refmaster.scripts.common.RMReportAttributeInfo.prototype = {
    AttributeID: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMReportAttributeSetupControlsId

com.ivp.refmaster.scripts.common.RMReportAttributeSetupControlsId = function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControlsId() {
    /// <field name="DdlAttributeDataTypeId" type="String">
    /// </field>
    /// <field name="BtnSaveId" type="String">
    /// </field>
    /// <field name="GridAttributeId" type="String">
    /// </field>
    /// <field name="PanelAttributeDetailId" type="String">
    /// </field>
    /// <field name="HdnReportId" type="String">
    /// </field>
    /// <field name="HdnRowId" type="String">
    /// </field>
    /// <field name="AddAtribManually" type="String">
    /// </field>
    /// <field name="ModalAddAttributeBehavId" type="String">
    /// </field>
    /// <field name="LabelDatatype" type="String">
    /// </field>
    /// <field name="BtnAddId" type="String">
    /// </field>
    /// <field name="BtnUpdateId" type="String">
    /// </field>
    /// <field name="LblAddUpdateAttribute" type="String">
    /// </field>
    /// <field name="HdnContainerHeightId" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMReportAttributeSetupControlsId.prototype = {
    DdlAttributeDataTypeId: null,
    BtnSaveId: null,
    GridAttributeId: null,
    PanelAttributeDetailId: null,
    HdnReportId: null,
    HdnRowId: null,
    AddAtribManually: null,
    ModalAddAttributeBehavId: null,
    LabelDatatype: null,
    BtnAddId: null,
    BtnUpdateId: null,
    LblAddUpdateAttribute: null,
    HdnContainerHeightId: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMReportAttributeSetupControls

com.ivp.refmaster.scripts.common.RMReportAttributeSetupControls = function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls(repControl) {
    /// <param name="repControl" type="com.ivp.refmaster.scripts.common.RMReportAttributeSetupControlsId">
    /// The rep control.
    /// </param>
    /// <field name="_controlIds" type="com.ivp.refmaster.scripts.common.RMReportAttributeSetupControlsId">
    /// </field>
    this._controlIds = repControl;
}
com.ivp.refmaster.scripts.common.RMReportAttributeSetupControls.prototype = {
    _controlIds: null,
    
    get_txtAttributeName: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_txtAttributeName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtAttributeName');
    },
    
    get_addAtribManually: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_addAtribManually() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.AddAtribManually);
    },
    
    get_txtDescription: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_txtDescription() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtDescription');
    },
    
    get_ddlAttributeDataType: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_ddlAttributeDataType() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.DdlAttributeDataTypeId);
    },
    
    get_btnAdd: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_btnAdd() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnAddId);
    },
    
    get_btnUpdate: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_btnUpdate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnUpdateId);
    },
    
    get_btnUpdateAttribute: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_btnUpdateAttribute() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnUpdate');
    },
    
    get_gridAttribute: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_gridAttribute() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.GridAttributeId);
    },
    
    get_btnSave: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_btnSave() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnSaveId);
    },
    
    get_btnCancel: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_btnCancel() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnCancel');
    },
    
    get_panelAttributeDetail: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_panelAttributeDetail() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.PanelAttributeDetailId);
    },
    
    get_btnCreateAttribute: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_btnCreateAttribute() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnCreateReportAttribute');
    },
    
    get_hdnReportID: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_hdnReportID() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnReportId);
    },
    
    get_hiddenRowID: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_hiddenRowID() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnRowId);
    },
    
    get_modalAddAttributeBehav: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_modalAddAttributeBehav() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.ModalAddAttributeBehavId);
    },
    
    get_labelDatatype: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_labelDatatype() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.LabelDatatype);
    },
    
    get_lblAddUpdateAttribute: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_lblAddUpdateAttribute() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.LblAddUpdateAttribute);
    },
    
    get_errorAttribute: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_errorAttribute() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('errorAttribute');
    },
    
    get_hdnContainerHeight: function com_ivp_refmaster_scripts_common_RMReportAttributeSetupControls$get_hdnContainerHeight() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnContainerHeightId);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSAttributeLookupInfo

com.ivp.refmaster.scripts.common.RMSAttributeLookupInfo = function com_ivp_refmaster_scripts_common_RMSAttributeLookupInfo() {
    /// <field name="AttributeLookupID" type="Number" integer="true">
    /// </field>
    /// <field name="ParentEntityTypeName" type="String">
    /// </field>
    /// <field name="ChildEntityTypeName" type="String">
    /// </field>
    /// <field name="ParentEntityAttributeName" type="String">
    /// </field>
    /// <field name="ChildEntityAttributeName" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSAttributeLookupInfo.prototype = {
    AttributeLookupID: 0,
    ParentEntityTypeName: null,
    ChildEntityTypeName: null,
    ParentEntityAttributeName: null,
    ChildEntityAttributeName: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSAttributeLookupControlInfo

com.ivp.refmaster.scripts.common.RMSAttributeLookupControlInfo = function com_ivp_refmaster_scripts_common_RMSAttributeLookupControlInfo() {
    /// <field name="BtnSaveAttributeLookupId" type="String">
    /// </field>
    /// <field name="GridAttributeLookupId" type="String">
    /// </field>
    /// <field name="HiddenEntityDataAll" type="String">
    /// </field>
    /// <field name="HiddenEntityDataReduced" type="String">
    /// </field>
    /// <field name="HiddenGridDataToSave" type="String">
    /// </field>
    /// <field name="ErrorWebService" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSAttributeLookupControlInfo.prototype = {
    BtnSaveAttributeLookupId: null,
    GridAttributeLookupId: null,
    HiddenEntityDataAll: null,
    HiddenEntityDataReduced: null,
    HiddenGridDataToSave: null,
    ErrorWebService: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSAttributeLookupControls

com.ivp.refmaster.scripts.common.RMSAttributeLookupControls = function com_ivp_refmaster_scripts_common_RMSAttributeLookupControls(controlIds) {
    /// <param name="controlIds" type="com.ivp.refmaster.scripts.common.RMSAttributeLookupControlInfo">
    /// The control ids.
    /// </param>
    /// <field name="_controlIds" type="com.ivp.refmaster.scripts.common.RMSAttributeLookupControlInfo">
    /// </field>
    this._controlIds = controlIds;
}
com.ivp.refmaster.scripts.common.RMSAttributeLookupControls.prototype = {
    _controlIds: null,
    
    get_btnSaveAttributeLookUp: function com_ivp_refmaster_scripts_common_RMSAttributeLookupControls$get_btnSaveAttributeLookUp() {
        /// <summary>
        /// Gets the BTN save attribute look up.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnSaveAttributeLookupId);
    },
    
    get_gridAttributeLookup: function com_ivp_refmaster_scripts_common_RMSAttributeLookupControls$get_gridAttributeLookup() {
        /// <summary>
        /// Gets the grid attribute lookup.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.GridAttributeLookupId);
    },
    
    get_errorWebService: function com_ivp_refmaster_scripts_common_RMSAttributeLookupControls$get_errorWebService() {
        /// <summary>
        /// Gets the error web service.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.ErrorWebService);
    },
    
    get_hiddenEntityDataAll: function com_ivp_refmaster_scripts_common_RMSAttributeLookupControls$get_hiddenEntityDataAll() {
        /// <summary>
        /// Gets the hidden entity data all.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HiddenEntityDataAll);
    },
    
    get_hiddenEntityDataReduced: function com_ivp_refmaster_scripts_common_RMSAttributeLookupControls$get_hiddenEntityDataReduced() {
        /// <summary>
        /// Gets the hidden entity data reduced.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HiddenEntityDataReduced);
    },
    
    get_hiddenGridDataToSave: function com_ivp_refmaster_scripts_common_RMSAttributeLookupControls$get_hiddenGridDataToSave() {
        /// <summary>
        /// Gets the hidden grid data to save.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HiddenGridDataToSave);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSDropDownInfo

com.ivp.refmaster.scripts.common.RMSDropDownInfo = function com_ivp_refmaster_scripts_common_RMSDropDownInfo() {
    /// <field name="Value" type="Number" integer="true">
    /// </field>
    /// <field name="Text" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSDropDownInfo.prototype = {
    Value: 0,
    Text: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSCustomClassInfo

com.ivp.refmaster.scripts.common.RMSCustomClassInfo = function com_ivp_refmaster_scripts_common_RMSCustomClassInfo() {
    /// <summary>
    /// Class containg info of Custom Class
    /// </summary>
    /// <field name="AssemblyPath" type="String">
    /// </field>
    /// <field name="CallType" type="String">
    /// </field>
    /// <field name="ClassName" type="String">
    /// </field>
    /// <field name="ClassType" type="String">
    /// </field>
    /// <field name="CustomClassId" type="Number" integer="true">
    /// </field>
    /// <field name="ExecSequence" type="Number" integer="true">
    /// </field>
    /// <field name="TaskDetailsId" type="Number" integer="true">
    /// </field>
    /// <field name="TaskMasterId" type="Number" integer="true">
    /// </field>
    this.AssemblyPath = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
}
com.ivp.refmaster.scripts.common.RMSCustomClassInfo.prototype = {
    CallType: null,
    ClassName: null,
    ClassType: null,
    CustomClassId: 0,
    ExecSequence: 0,
    TaskDetailsId: 0,
    TaskMasterId: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSDatasourceInfo

com.ivp.refmaster.scripts.common.RMSDatasourceInfo = function com_ivp_refmaster_scripts_common_RMSDatasourceInfo() {
    /// <field name="DatasourceID" type="Number" integer="true">
    /// </field>
    /// <field name="DatasourceName" type="String">
    /// </field>
    /// <field name="DatasourceDescription" type="String">
    /// </field>
    /// <field name="AccountID" type="Number" integer="true">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSDatasourceInfo.prototype = {
    DatasourceID: 0,
    DatasourceName: null,
    DatasourceDescription: null,
    AccountID: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMFeedSummaryInfo

com.ivp.refmaster.scripts.common.RMFeedSummaryInfo = function com_ivp_refmaster_scripts_common_RMFeedSummaryInfo() {
    /// <field name="FeedSummaryID" type="Number" integer="true">
    /// </field>
    /// <field name="FeedName" type="String">
    /// </field>
    /// <field name="DataSourceID" type="Number" integer="true">
    /// </field>
    /// <field name="FeedTypeID" type="Number" integer="true">
    /// </field>
    /// <field name="RadFileID" type="Number" integer="true">
    /// </field>
    /// <field name="DBProvider" type="String">
    /// </field>
    /// <field name="ConnectionString" type="String">
    /// </field>
    /// <field name="ColumnQuery" type="String">
    /// </field>
    /// <field name="IsComplete" type="Boolean">
    /// </field>
    /// <field name="IsBulkLoaded" type="Boolean">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMFeedSummaryInfo.prototype = {
    FeedSummaryID: 0,
    FeedName: null,
    DataSourceID: 0,
    FeedTypeID: 0,
    RadFileID: 0,
    DBProvider: null,
    ConnectionString: null,
    ColumnQuery: null,
    IsComplete: false,
    IsBulkLoaded: false
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSEntityPrioritizationInfo

com.ivp.refmaster.scripts.common.RMSEntityPrioritizationInfo = function com_ivp_refmaster_scripts_common_RMSEntityPrioritizationInfo() {
    /// <field name="EntityAttributeId" type="Number" integer="true">
    /// </field>
    /// <field name="dspriorityInfoObjectArrayList" type="Array">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSEntityPrioritizationInfo.prototype = {
    EntityAttributeId: 0,
    dspriorityInfoObjectArrayList: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.DatasourcePriorityInfo

com.ivp.refmaster.scripts.common.DatasourcePriorityInfo = function com_ivp_refmaster_scripts_common_DatasourcePriorityInfo() {
    /// <field name="Priority" type="Number" integer="true">
    /// </field>
    /// <field name="DataSourceId" type="Number" integer="true">
    /// </field>
}
com.ivp.refmaster.scripts.common.DatasourcePriorityInfo.prototype = {
    Priority: 0,
    DataSourceId: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSEntityTypePriortizationControlId

com.ivp.refmaster.scripts.common.RMSEntityTypePriortizationControlId = function com_ivp_refmaster_scripts_common_RMSEntityTypePriortizationControlId() {
    /// <field name="DdlShowEntityTypes" type="String">
    /// </field>
    /// <field name="GridAttribues" type="String">
    /// </field>
    /// <field name="PanelTab" type="String">
    /// </field>
    /// <field name="ButtonSave" type="String">
    /// </field>
    /// <field name="HiddenEntityType" type="String">
    /// </field>
    /// <field name="HiddenDataFieldId" type="String">
    /// </field>
    /// <field name="UniqueAttributeGrid" type="String">
    /// </field>
    /// <field name="ErrorLabel" type="String">
    /// </field>
    /// <field name="LabelEntityName" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSEntityTypePriortizationControlId.prototype = {
    DdlShowEntityTypes: null,
    GridAttribues: null,
    PanelTab: null,
    ButtonSave: null,
    HiddenEntityType: null,
    HiddenDataFieldId: null,
    UniqueAttributeGrid: null,
    ErrorLabel: null,
    LabelEntityName: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSEntityTypePriortizationControls

com.ivp.refmaster.scripts.common.RMSEntityTypePriortizationControls = function com_ivp_refmaster_scripts_common_RMSEntityTypePriortizationControls(objEntityTypePriortizationControlId) {
    /// <param name="objEntityTypePriortizationControlId" type="com.ivp.refmaster.scripts.common.RMSEntityTypePriortizationControlId">
    /// </param>
    /// <field name="_objEntityTypePriortizationControlId" type="com.ivp.refmaster.scripts.common.RMSEntityTypePriortizationControlId">
    /// </field>
    this._objEntityTypePriortizationControlId = objEntityTypePriortizationControlId;
}
com.ivp.refmaster.scripts.common.RMSEntityTypePriortizationControls.prototype = {
    _objEntityTypePriortizationControlId: null,
    
    get_ddlEntityTypes: function com_ivp_refmaster_scripts_common_RMSEntityTypePriortizationControls$get_ddlEntityTypes() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypePriortizationControlId.DdlShowEntityTypes);
    },
    
    get_gridAttribues: function com_ivp_refmaster_scripts_common_RMSEntityTypePriortizationControls$get_gridAttribues() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypePriortizationControlId.GridAttribues);
    },
    
    get_panelTab: function com_ivp_refmaster_scripts_common_RMSEntityTypePriortizationControls$get_panelTab() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypePriortizationControlId.PanelTab);
    },
    
    get_buttonSave: function com_ivp_refmaster_scripts_common_RMSEntityTypePriortizationControls$get_buttonSave() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypePriortizationControlId.ButtonSave);
    },
    
    get_hiddenEntityTypeId: function com_ivp_refmaster_scripts_common_RMSEntityTypePriortizationControls$get_hiddenEntityTypeId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypePriortizationControlId.HiddenEntityType);
    },
    
    get_hiddenDataField: function com_ivp_refmaster_scripts_common_RMSEntityTypePriortizationControls$get_hiddenDataField() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypePriortizationControlId.HiddenDataFieldId);
    },
    
    get_uniqueAttributeGrid: function com_ivp_refmaster_scripts_common_RMSEntityTypePriortizationControls$get_uniqueAttributeGrid() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypePriortizationControlId.UniqueAttributeGrid);
    },
    
    get_errorLabel: function com_ivp_refmaster_scripts_common_RMSEntityTypePriortizationControls$get_errorLabel() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypePriortizationControlId.ErrorLabel);
    },
    
    get_labelEntityTypeName: function com_ivp_refmaster_scripts_common_RMSEntityTypePriortizationControls$get_labelEntityTypeName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypePriortizationControlId.LabelEntityName);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMEntityTypeFeedMappingDetailsInfo

com.ivp.refmaster.scripts.common.RMEntityTypeFeedMappingDetailsInfo = function com_ivp_refmaster_scripts_common_RMEntityTypeFeedMappingDetailsInfo() {
    /// <summary>
    /// Class containg info of Entity Type Feed Mapping Details Info
    /// </summary>
    /// <field name="EntityTypeFeedMappingDetailsId" type="Number" integer="true">
    /// </field>
    /// <field name="EntityTypeFeedMappingId" type="Number" integer="true">
    /// </field>
    /// <field name="EntityAttributeId" type="Number" integer="true">
    /// </field>
    /// <field name="FieldId" type="Number" integer="true">
    /// </field>
    /// <field name="CreatedBy" type="String">
    /// </field>
    /// <field name="LastModifiedBy" type="String">
    /// </field>
    /// <field name="DataFormat" type="String">
    /// </field>
    /// <field name="UpdateBlank" type="String">
    /// </field>
    this.CreatedBy = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.LastModifiedBy = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.DataFormat = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.UpdateBlank = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
}
com.ivp.refmaster.scripts.common.RMEntityTypeFeedMappingDetailsInfo.prototype = {
    EntityTypeFeedMappingDetailsId: 0,
    EntityTypeFeedMappingId: 0,
    EntityAttributeId: 0,
    FieldId: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMEntityAttributesFeedFieldsMappingInfo

com.ivp.refmaster.scripts.common.RMEntityAttributesFeedFieldsMappingInfo = function com_ivp_refmaster_scripts_common_RMEntityAttributesFeedFieldsMappingInfo() {
    /// <field name="EntityAttributeId" type="Number" integer="true">
    /// </field>
    /// <field name="EntityAttributeName" type="String">
    /// </field>
    /// <field name="IsMandatory" type="Boolean">
    /// </field>
    /// <field name="EntityAttributeDisplayName" type="String">
    /// </field>
    /// <field name="DataType" type="String">
    /// </field>
    /// <field name="EntityTypeId" type="Number" integer="true">
    /// </field>
    /// <field name="EntityTypeName" type="String">
    /// </field>
    /// <field name="EntityDisplayName" type="String">
    /// </field>
    /// <field name="AttributeLookupId" type="Number" integer="true">
    /// </field>
    /// <field name="ParentEntityTypeName" type="String">
    /// </field>
    /// <field name="ParentEntityAttributeName" type="String">
    /// </field>
    /// <field name="EntityFeedAttributeLookupId" type="String">
    /// </field>
    /// <field name="FeedSummaryId" type="Number" integer="true">
    /// </field>
    /// <field name="FieldId" type="Number" integer="true">
    /// </field>
    /// <field name="IsPrimary" type="Boolean">
    /// </field>
    /// <field name="DateTimeFormat" type="String">
    /// </field>
    /// <field name="IsMandatoryToMapLookupInEntity" type="Boolean">
    /// </field>
    this.EntityAttributeName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.EntityAttributeDisplayName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.DataType = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.EntityTypeName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.EntityDisplayName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.ParentEntityTypeName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.ParentEntityAttributeName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.EntityFeedAttributeLookupId = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.DateTimeFormat = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
}
com.ivp.refmaster.scripts.common.RMEntityAttributesFeedFieldsMappingInfo.prototype = {
    EntityAttributeId: 0,
    IsMandatory: false,
    EntityTypeId: 0,
    AttributeLookupId: 0,
    FeedSummaryId: 0,
    FieldId: 0,
    IsPrimary: false,
    IsMandatoryToMapLookupInEntity: false
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMEntityFeedAttributeLookup

com.ivp.refmaster.scripts.common.RMEntityFeedAttributeLookup = function com_ivp_refmaster_scripts_common_RMEntityFeedAttributeLookup() {
    /// <summary>
    /// Class containg info of Entity Type Feed Mapping Details Info
    /// </summary>
    /// <field name="EntityFeedAttributeLookupId" type="Number" integer="true">
    /// </field>
    /// <field name="AttributeLookupId" type="Number" integer="true">
    /// </field>
    /// <field name="FeedSummaryId" type="Number" integer="true">
    /// </field>
    /// <field name="ParentEntityAttributeName" type="String">
    /// </field>
    /// <field name="CreatedBy" type="String">
    /// </field>
    /// <field name="LastModifiedBy" type="String">
    /// </field>
    this.ParentEntityAttributeName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.CreatedBy = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.LastModifiedBy = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
}
com.ivp.refmaster.scripts.common.RMEntityFeedAttributeLookup.prototype = {
    EntityFeedAttributeLookupId: 0,
    AttributeLookupId: 0,
    FeedSummaryId: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSEntityTypeFeedMappingDetailsControlIds

com.ivp.refmaster.scripts.common.RMSEntityTypeFeedMappingDetailsControlIds = function com_ivp_refmaster_scripts_common_RMSEntityTypeFeedMappingDetailsControlIds() {
    /// <summary>
    /// Class containing control Ids
    /// </summary>
    /// <field name="BtnSaveAttFieldMappingId" type="String">
    /// </field>
    /// <field name="PanelTabId" type="String">
    /// </field>
    /// <field name="GridAttributeFieldMappingId" type="String">
    /// </field>
    /// <field name="HdnMappingDetailsInfoId" type="String">
    /// </field>
    /// <field name="HdnPrimaryFieldId" type="String">
    /// </field>
    /// <field name="HdnEFMappingId" type="String">
    /// </field>
    /// <field name="HdnEntityTypeId" type="String">
    /// </field>
    /// <field name="HdnLookupAttributesId" type="String">
    /// </field>
    /// <field name="HdnEntityFeedLookupAttributesId" type="String">
    /// </field>
    /// <field name="hdnUKPKSetInAnotherFeedId" type="String">
    /// </field>
    /// <field name="ChkIsUpdateOnly" type="String">
    /// </field>
    /// <field name="ChkReplaceExisting" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSEntityTypeFeedMappingDetailsControlIds.prototype = {
    BtnSaveAttFieldMappingId: null,
    PanelTabId: null,
    GridAttributeFieldMappingId: null,
    HdnMappingDetailsInfoId: null,
    HdnPrimaryFieldId: null,
    HdnEFMappingId: null,
    HdnEntityTypeId: null,
    HdnLookupAttributesId: null,
    HdnEntityFeedLookupAttributesId: null,
    hdnUKPKSetInAnotherFeedId: null,
    ChkIsUpdateOnly: null,
    ChkReplaceExisting: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSEntityTypeFeedMappingDetailsControls

com.ivp.refmaster.scripts.common.RMSEntityTypeFeedMappingDetailsControls = function com_ivp_refmaster_scripts_common_RMSEntityTypeFeedMappingDetailsControls(objEntityTypeFeedMappingControlIds) {
    /// <summary>
    /// Class containing Controls
    /// </summary>
    /// <param name="objEntityTypeFeedMappingControlIds" type="com.ivp.refmaster.scripts.common.RMSEntityTypeFeedMappingDetailsControlIds">
    /// The obj entity type feed mapping control ids.
    /// </param>
    /// <field name="_objEntityTypeFeedMappingControlIds" type="com.ivp.refmaster.scripts.common.RMSEntityTypeFeedMappingDetailsControlIds">
    /// </field>
    this._objEntityTypeFeedMappingControlIds = objEntityTypeFeedMappingControlIds;
}
com.ivp.refmaster.scripts.common.RMSEntityTypeFeedMappingDetailsControls.prototype = {
    _objEntityTypeFeedMappingControlIds: null,
    
    get_btnSaveFeedMapping: function com_ivp_refmaster_scripts_common_RMSEntityTypeFeedMappingDetailsControls$get_btnSaveFeedMapping() {
        /// <summary>
        /// Gets the BTN save Attribute Field Mapping
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeFeedMappingControlIds.BtnSaveAttFieldMappingId);
    },
    
    get_panelTab: function com_ivp_refmaster_scripts_common_RMSEntityTypeFeedMappingDetailsControls$get_panelTab() {
        /// <summary>
        /// Gets the Grid Feed Mapping.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeFeedMappingControlIds.PanelTabId);
    },
    
    get_gridAttributeFieldMapping: function com_ivp_refmaster_scripts_common_RMSEntityTypeFeedMappingDetailsControls$get_gridAttributeFieldMapping() {
        /// <summary>
        /// Gets the Grid Feed Mapping.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeFeedMappingControlIds.GridAttributeFieldMappingId);
    },
    
    get_hdnMappingDetailsInfo: function com_ivp_refmaster_scripts_common_RMSEntityTypeFeedMappingDetailsControls$get_hdnMappingDetailsInfo() {
        /// <summary>
        /// Gets the Hdn Att to field mapping info
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeFeedMappingControlIds.HdnMappingDetailsInfoId);
    },
    
    get_hdnUKPKSetInAnotherFeed: function com_ivp_refmaster_scripts_common_RMSEntityTypeFeedMappingDetailsControls$get_hdnUKPKSetInAnotherFeed() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeFeedMappingControlIds.hdnUKPKSetInAnotherFeedId);
    },
    
    get_hdnPrimaryField: function com_ivp_refmaster_scripts_common_RMSEntityTypeFeedMappingDetailsControls$get_hdnPrimaryField() {
        /// <summary>
        /// Gets the Hdn Att to field mapping info
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeFeedMappingControlIds.HdnPrimaryFieldId);
    },
    
    get_hdnEFMapping: function com_ivp_refmaster_scripts_common_RMSEntityTypeFeedMappingDetailsControls$get_hdnEFMapping() {
        /// <summary>
        /// Gets the Hdn Att to field mapping info
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeFeedMappingControlIds.HdnEFMappingId);
    },
    
    get_hdnEntityType: function com_ivp_refmaster_scripts_common_RMSEntityTypeFeedMappingDetailsControls$get_hdnEntityType() {
        /// <summary>
        /// Gets the Hdn Entity type id
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeFeedMappingControlIds.HdnEntityTypeId);
    },
    
    get_hdnLookupAttributes: function com_ivp_refmaster_scripts_common_RMSEntityTypeFeedMappingDetailsControls$get_hdnLookupAttributes() {
        /// <summary>
        /// Gets the Hdn Lookup Attributes
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeFeedMappingControlIds.HdnLookupAttributesId);
    },
    
    get_hdnEntityFeedLookupAttributesInfo: function com_ivp_refmaster_scripts_common_RMSEntityTypeFeedMappingDetailsControls$get_hdnEntityFeedLookupAttributesInfo() {
        /// <summary>
        /// Gets the Hdn Lookup Attributes
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeFeedMappingControlIds.HdnEntityFeedLookupAttributesId);
    },
    
    get_divErrorDisplay: function com_ivp_refmaster_scripts_common_RMSEntityTypeFeedMappingDetailsControls$get_divErrorDisplay() {
        /// <summary>
        /// Gets the DIV to display error
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('divErrorDisplay');
    },
    
    get_chkIsUpdateOnly: function com_ivp_refmaster_scripts_common_RMSEntityTypeFeedMappingDetailsControls$get_chkIsUpdateOnly() {
        /// <summary>
        /// Gets the Check Box Is Update Only
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeFeedMappingControlIds.ChkIsUpdateOnly);
    },
    
    get_chkReplaceExisting: function com_ivp_refmaster_scripts_common_RMSEntityTypeFeedMappingDetailsControls$get_chkReplaceExisting() {
        /// <summary>
        /// Gets the Check Box Replace Existing
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeFeedMappingControlIds.ChkReplaceExisting);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSParentChildMappingInfo

com.ivp.refmaster.scripts.common.RMSParentChildMappingInfo = function com_ivp_refmaster_scripts_common_RMSParentChildMappingInfo() {
    /// <field name="parentAttribute" type="String">
    /// </field>
    /// <field name="childAttribute" type="String">
    /// </field>
    this.parentAttribute = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.childAttribute = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSTaskInfo

com.ivp.refmaster.scripts.common.RMSTaskInfo = function com_ivp_refmaster_scripts_common_RMSTaskInfo() {
    /// <summary>
    /// Class containing the info of the task
    /// </summary>
    /// <field name="TaskMasterId" type="Number" integer="true">
    /// </field>
    /// <field name="TaskName" type="String">
    /// </field>
    /// <field name="TaskDescription" type="String">
    /// </field>
    /// <field name="TaskTypeId" type="Number" integer="true">
    /// </field>
    /// <field name="DependentId" type="Number" integer="true">
    /// </field>
    /// <field name="ScheduledJobId" type="Number" integer="true">
    /// </field>
    /// <field name="TriggerType" type="Number" integer="true">
    /// </field>
    /// <field name="DependentPreTask" type="Number" integer="true">
    /// </field>
    /// <field name="LogDescription" type="String">
    /// </field>
    /// <field name="Status" type="String">
    /// </field>
    /// <field name="TaskInstanceID" type="Number" integer="true">
    /// </field>
    /// <field name="LoginName" type="String">
    /// </field>
    /// <field name="CalenderID" type="Number" integer="true">
    /// </field>
    /// <field name="FlowID" type="Number" integer="true">
    /// </field>
    this.TaskName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.TaskDescription = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.LogDescription = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.Status = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.LoginName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
}
com.ivp.refmaster.scripts.common.RMSTaskInfo.prototype = {
    TaskMasterId: 0,
    TaskTypeId: 0,
    DependentId: 0,
    ScheduledJobId: 0,
    TriggerType: 0,
    DependentPreTask: 0,
    TaskInstanceID: 0,
    CalenderID: 0,
    FlowID: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSScheduledJobInfo

com.ivp.refmaster.scripts.common.RMSScheduledJobInfo = function com_ivp_refmaster_scripts_common_RMSScheduledJobInfo() {
    /// <summary>
    /// Class containing info of scheduler
    /// </summary>
    /// <field name="JobId" type="Number" integer="true">
    /// </field>
    /// <field name="SchedulableJobId" type="Number" integer="true">
    /// </field>
    /// <field name="JobName" type="String">
    /// </field>
    /// <field name="JobDescription" type="String">
    /// </field>
    /// <field name="StartTime" type="String">
    /// </field>
    /// <field name="NoEndDate" type="Boolean">
    /// </field>
    /// <field name="StartDate" type="String">
    /// </field>
    /// <field name="EndDate" type="String">
    /// </field>
    /// <field name="RecurrenceType" type="Boolean">
    /// </field>
    /// <field name="RecurrencePattern" type="String">
    /// </field>
    /// <field name="DaysInterval" type="Number" integer="true">
    /// </field>
    /// <field name="WeekInterval" type="Number" integer="true">
    /// </field>
    /// <field name="MonthInterval" type="Number" integer="true">
    /// </field>
    /// <field name="DaysofWeek" type="Number" integer="true">
    /// </field>
    /// <field name="NoOfRecurrences" type="Number" integer="true">
    /// </field>
    /// <field name="TimeIntervalOfRecurrence" type="Number" integer="true">
    /// </field>
    /// <field name="CreationTime" type="String">
    /// </field>
    /// <field name="ModificationTime" type="String">
    /// </field>
    /// <field name="IsActive" type="Boolean">
    /// </field>
    /// <field name="NextScheduleTime" type="String">
    /// </field>
    /// <field name="NoOfRuns" type="Number" integer="true">
    /// </field>
    this.JobName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.JobDescription = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.EndDate = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.RecurrencePattern = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.CreationTime = com.ivp.rad.rscriptutils.RSConstants.datetimE_EMPTY;
    this.ModificationTime = com.ivp.rad.rscriptutils.RSConstants.datetimE_EMPTY;
    this.NextScheduleTime = com.ivp.rad.rscriptutils.RSConstants.datetimE_EMPTY;
}
com.ivp.refmaster.scripts.common.RMSScheduledJobInfo.prototype = {
    JobId: 0,
    SchedulableJobId: 0,
    StartTime: null,
    NoEndDate: false,
    StartDate: null,
    RecurrenceType: false,
    DaysInterval: 0,
    WeekInterval: 0,
    MonthInterval: 0,
    DaysofWeek: 0,
    NoOfRecurrences: 0,
    TimeIntervalOfRecurrence: 0,
    IsActive: false,
    NoOfRuns: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSFlowSetupControlIds

com.ivp.refmaster.scripts.common.RMSFlowSetupControlIds = function com_ivp_refmaster_scripts_common_RMSFlowSetupControlIds() {
    /// <summary>
    /// Class containing Flow Setup ControlIds
    /// </summary>
    /// <field name="GridTaskFlowDetails" type="String">
    /// </field>
    /// <field name="TasksShuttle" type="String">
    /// </field>
    /// <field name="DdlCalendar" type="String">
    /// </field>
    /// <field name="BtnSaveTask" type="String">
    /// </field>
    /// <field name="TxtStartDate" type="String">
    /// </field>
    /// <field name="TxtStartTime" type="String">
    /// </field>
    /// <field name="ChklstDaysOfWeek" type="String">
    /// </field>
    /// <field name="TxtEndDate" type="String">
    /// </field>
    /// <field name="ModalScheduledJob" type="String">
    /// </field>
    /// <field name="PanelScheduledJob" type="String">
    /// </field>
    /// <field name="HiddenJobID" type="String">
    /// </field>
    /// <field name="HiddenData" type="String">
    /// </field>
    /// <field name="LstSuccessUserId" type="String">
    /// </field>
    /// <field name="LstFailureUserId" type="String">
    /// </field>
    /// <field name="BtnConfirmSubscription" type="String">
    /// </field>
    /// <field name="SearchTaskTxt" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSFlowSetupControlIds.prototype = {
    GridTaskFlowDetails: null,
    TasksShuttle: null,
    DdlCalendar: null,
    BtnSaveTask: null,
    TxtStartDate: null,
    TxtStartTime: null,
    ChklstDaysOfWeek: null,
    TxtEndDate: null,
    ModalScheduledJob: null,
    PanelScheduledJob: null,
    HiddenJobID: null,
    HiddenData: null,
    LstSuccessUserId: null,
    LstFailureUserId: null,
    BtnConfirmSubscription: null,
    SearchTaskTxt: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSFlowSetupControl

com.ivp.refmaster.scripts.common.RMSFlowSetupControl = function com_ivp_refmaster_scripts_common_RMSFlowSetupControl(controlIds) {
    /// <summary>
    /// Control Class of Flow Setup
    /// </summary>
    /// <param name="controlIds" type="com.ivp.refmaster.scripts.common.RMSFlowSetupControlIds">
    /// The control ids.
    /// </param>
    /// <field name="_objFlowSetupControlIds" type="com.ivp.refmaster.scripts.common.RMSFlowSetupControlIds">
    /// </field>
    this._objFlowSetupControlIds = controlIds;
}
com.ivp.refmaster.scripts.common.RMSFlowSetupControl.prototype = {
    _objFlowSetupControlIds: null,
    
    get_lstSuccessUser: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_lstSuccessUser() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFlowSetupControlIds.LstSuccessUserId);
    },
    
    get_lstFailureUser: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_lstFailureUser() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFlowSetupControlIds.LstFailureUserId);
    },
    
    get_btnConfirmSubscription: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_btnConfirmSubscription() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFlowSetupControlIds.BtnConfirmSubscription);
    },
    
    get_lblTaskName: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_lblTaskName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblTaskName');
    },
    
    get_lblListBoxError: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_lblListBoxError() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblListBoxError');
    },
    
    get_btnAddNewTask: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_btnAddNewTask() {
        /// <summary>
        /// Gets the BTN add new task.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnAddNewTask');
    },
    
    get_taskShuttle: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_taskShuttle() {
        /// <summary>
        /// Gets the shuttle ID.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFlowSetupControlIds.TasksShuttle);
    },
    
    get_panelScheduledTask: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_panelScheduledTask() {
        /// <summary>
        /// Gets the panel ID.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFlowSetupControlIds.PanelScheduledJob);
    },
    
    get_gridTaskFlowDetails: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_gridTaskFlowDetails() {
        /// <summary>
        /// Gets the grid ID.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFlowSetupControlIds.GridTaskFlowDetails);
    },
    
    get_ddlCalendarType: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_ddlCalendarType() {
        /// <summary>
        /// Gets the type of the DDL calendar.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFlowSetupControlIds.DdlCalendar);
    },
    
    get_chkDaysofWeek: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_chkDaysofWeek() {
        /// <summary>
        /// Gets the CHK daysof week.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFlowSetupControlIds.ChklstDaysOfWeek);
    },
    
    get_rbRecurrenceType: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_rbRecurrenceType() {
        /// <summary>
        /// Gets the rb recurrence ID.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('rbRecurring');
    },
    
    get_rbNonRecurrencePattern: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_rbNonRecurrencePattern() {
        /// <summary>
        /// Gets the rb non recurrence ID.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('rbNonRecurring');
    },
    
    get_txtStartDate: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_txtStartDate() {
        /// <summary>
        /// Gets the TXT start date.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFlowSetupControlIds.TxtStartDate);
    },
    
    get_txtStartTime: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_txtStartTime() {
        /// <summary>
        /// Gets the TXT start time.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFlowSetupControlIds.TxtStartTime);
    },
    
    get_txtIntervalID: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_txtIntervalID() {
        /// <summary>
        /// Gets the TXT interval ID.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtInterval');
    },
    
    get_txtEndDate: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_txtEndDate() {
        /// <summary>
        /// Gets the TXT end date.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFlowSetupControlIds.TxtEndDate);
    },
    
    get_chkNeverEndJob: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_chkNeverEndJob() {
        /// <summary>
        /// Gets the CHK never end job.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('chkNoEndDate');
    },
    
    get_txtNoOfRecurrence: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_txtNoOfRecurrence() {
        /// <summary>
        /// Gets the TXT no of recurrence.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtNoOfRecurrences');
    },
    
    get_txtRecurrenceTimeInterval: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_txtRecurrenceTimeInterval() {
        /// <summary>
        /// Gets the TXT recurrence time interval.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtTimeIntervalOfRecurrence');
    },
    
    get_btnCancelConfigureTask: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_btnCancelConfigureTask() {
        /// <summary>
        /// Gets the BTN cancel configure task.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnCancelConfigureTask');
    },
    
    get_btnConfigureTask: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_btnConfigureTask() {
        /// <summary>
        /// Gets the BTN configure task.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnConfigureTask');
    },
    
    get_btnSaveTask: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_btnSaveTask() {
        /// <summary>
        /// Gets the BTN save task.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFlowSetupControlIds.BtnSaveTask);
    },
    
    get_btnCancelTaskAddition: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_btnCancelTaskAddition() {
        /// <summary>
        /// Gets the BTN cancel task addition.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnCancelTaskAddition');
    },
    
    get_radioManual: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_radioManual() {
        /// <summary>
        /// Gets the radio manual.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('rdTaskTriggerManual');
    },
    
    get_radioSchedule: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_radioSchedule() {
        /// <summary>
        /// Gets the radio schedule.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('rdTaskTriggerSchedule');
    },
    
    get_hiddenScheduledJobID: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_hiddenScheduledJobID() {
        /// <summary>
        /// Gets the hidden scheduled job ID.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFlowSetupControlIds.HiddenJobID);
    },
    
    get_errorTaskDetails: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_errorTaskDetails() {
        /// <summary>
        /// Gets the error task details.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('errorTaskDetails');
    },
    
    get_lblHeaderScheduledJob: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_lblHeaderScheduledJob() {
        /// <summary>
        /// Gets the LBL header scheduled job.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblHeaderScheduledJob');
    },
    
    get_rbDaily: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_rbDaily() {
        /// <summary>
        /// Gets the rb daily.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('rbDaily');
    },
    
    get_rbWeekly: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_rbWeekly() {
        /// <summary>
        /// Gets the rb weekly.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('rbWeekly');
    },
    
    get_rbMonthly: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_rbMonthly() {
        /// <summary>
        /// Gets the rb monthly.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('rbMonthly');
    },
    
    get_lblInterval: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_lblInterval() {
        /// <summary>
        /// Gets the LBL interval.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblInterval');
    },
    
    get_errorScheeduledJob: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_errorScheeduledJob() {
        /// <summary>
        /// Gets the error scheeduled job.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('errorScheeduledJob');
    },
    
    get_btnPanelAddScheduledJOb: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_btnPanelAddScheduledJOb() {
        /// <summary>
        /// Gets the BTN panel add scheduled J ob.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnPanelAdd');
    },
    
    get_btnPanelOkCancel: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_btnPanelOkCancel() {
        /// <summary>
        /// Gets the BTN panel ok cancel.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnPanelOkCancel');
    },
    
    get_btnShowScheduledJob: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_btnShowScheduledJob() {
        /// <summary>
        /// Gets the BTN show scheduled job.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnShowScheduledJob');
    },
    
    get_hiddenData: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_hiddenData() {
        /// <summary>
        /// Gets the hidden data.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFlowSetupControlIds.HiddenData);
    },
    
    get_searchTaskTxt: function com_ivp_refmaster_scripts_common_RMSFlowSetupControl$get_searchTaskTxt() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFlowSetupControlIds.SearchTaskTxt);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSLoadingTaskInfo

com.ivp.refmaster.scripts.common.RMSLoadingTaskInfo = function com_ivp_refmaster_scripts_common_RMSLoadingTaskInfo() {
    /// <summary>
    /// Class for Loading Task Info
    /// </summary>
    /// <field name="FeedLoadingDetailsID" type="Number" integer="true">
    /// </field>
    /// <field name="TaskMasterID" type="Number" integer="true">
    /// </field>
    /// <field name="FeedSummaryID" type="Number" integer="true">
    /// </field>
    /// <field name="BulkFileDateType" type="Number" integer="true">
    /// </field>
    /// <field name="BulkFileDate" type="String">
    /// </field>
    /// <field name="DiffFileDateType" type="Number" integer="true">
    /// </field>
    /// <field name="DiffFileDate" type="String">
    /// </field>
    /// <field name="BulkFilePath" type="String">
    /// </field>
    /// <field name="DifferenceFilePath" type="String">
    /// </field>
    /// <field name="CustomCallExist" type="Boolean">
    /// </field>
    /// <field name="DiffFileDateDays" type="Number" integer="true">
    /// </field>
    /// <field name="BulkFileDateDays" type="Number" integer="true">
    /// </field>
    this.BulkFileDate = com.ivp.rad.rscriptutils.RSConstants.datetimE_EMPTY;
    this.DiffFileDate = com.ivp.rad.rscriptutils.RSConstants.datetimE_EMPTY;
}
com.ivp.refmaster.scripts.common.RMSLoadingTaskInfo.prototype = {
    FeedLoadingDetailsID: 0,
    TaskMasterID: 0,
    FeedSummaryID: 0,
    BulkFileDateType: 0,
    DiffFileDateType: 0,
    BulkFilePath: '',
    DifferenceFilePath: '',
    CustomCallExist: false,
    DiffFileDateDays: 0,
    BulkFileDateDays: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSLoadingTaskControlIds

com.ivp.refmaster.scripts.common.RMSLoadingTaskControlIds = function com_ivp_refmaster_scripts_common_RMSLoadingTaskControlIds() {
    /// <summary>
    /// Class For Loading Task control Ids
    /// </summary>
    /// <field name="TxtLoadingTaskName" type="String">
    /// </field>
    /// <field name="TxtLoadingTaskDescription" type="String">
    /// </field>
    /// <field name="TxtLoadingFilePathBulk" type="String">
    /// </field>
    /// <field name="DdlLoadingFileDateTypeBulk" type="String">
    /// </field>
    /// <field name="TxtFileDateTypeBulk" type="String">
    /// </field>
    /// <field name="TxtFileDateDaysBulk" type="String">
    /// </field>
    /// <field name="TxtFilePathDiff" type="String">
    /// </field>
    /// <field name="DdlFileDateTypeDiff" type="String">
    /// </field>
    /// <field name="TxtFileDateDiff" type="String">
    /// </field>
    /// <field name="TxtFileDateDaysDiff" type="String">
    /// </field>
    /// <field name="GridCustomClass" type="String">
    /// </field>
    /// <field name="BtnSaveLoading" type="String">
    /// </field>
    /// <field name="HiddenIds" type="String">
    /// </field>
    /// <field name="PopupCustomClass" type="String">
    /// </field>
    /// <field name="ModalCustomClass" type="String">
    /// </field>
    /// <field name="HdnLoadingData" type="String">
    /// </field>
    /// <field name="IsValid" type="String">
    /// </field>
    /// <field name="ModalPopupDeleteId" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSLoadingTaskControlIds.prototype = {
    TxtLoadingTaskName: null,
    TxtLoadingTaskDescription: null,
    TxtLoadingFilePathBulk: null,
    DdlLoadingFileDateTypeBulk: null,
    TxtFileDateTypeBulk: null,
    TxtFileDateDaysBulk: null,
    TxtFilePathDiff: null,
    DdlFileDateTypeDiff: null,
    TxtFileDateDiff: null,
    TxtFileDateDaysDiff: null,
    GridCustomClass: null,
    BtnSaveLoading: null,
    HiddenIds: null,
    PopupCustomClass: null,
    ModalCustomClass: null,
    HdnLoadingData: null,
    IsValid: null,
    ModalPopupDeleteId: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSLoadingControls

com.ivp.refmaster.scripts.common.RMSLoadingControls = function com_ivp_refmaster_scripts_common_RMSLoadingControls(info) {
    /// <summary>
    /// Class For Loading Task Controls
    /// </summary>
    /// <param name="info" type="com.ivp.refmaster.scripts.common.RMSLoadingTaskControlIds">
    /// The info.
    /// </param>
    /// <field name="_rmsLoadingControlIds" type="com.ivp.refmaster.scripts.common.RMSLoadingTaskControlIds">
    /// </field>
    this._rmsLoadingControlIds = info;
}
com.ivp.refmaster.scripts.common.RMSLoadingControls.prototype = {
    _rmsLoadingControlIds: null,
    
    get_txtLoadingTaskName: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_txtLoadingTaskName() {
        /// <summary>
        /// Gets the name of the TXT loading task.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsLoadingControlIds.TxtLoadingTaskName);
    },
    
    get_txtLoadingTaskDescription: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_txtLoadingTaskDescription() {
        /// <summary>
        /// Gets the TXT loading task description.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsLoadingControlIds.TxtLoadingTaskDescription);
    },
    
    get_txtLoadingFilePathBulk: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_txtLoadingFilePathBulk() {
        /// <summary>
        /// Gets the TXT loading file path bulk.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsLoadingControlIds.TxtLoadingFilePathBulk);
    },
    
    get_ddlLoadingFileDateTypeBulk: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_ddlLoadingFileDateTypeBulk() {
        /// <summary>
        /// Gets the DDL loading file date type bulk.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsLoadingControlIds.DdlLoadingFileDateTypeBulk);
    },
    
    get_txtFileDateTypeBulk: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_txtFileDateTypeBulk() {
        /// <summary>
        /// Gets the TXT file date type bulk.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsLoadingControlIds.TxtFileDateTypeBulk);
    },
    
    get_txtFileDateDaysBulk: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_txtFileDateDaysBulk() {
        /// <summary>
        /// Gets the TXT file date days bulk.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsLoadingControlIds.TxtFileDateDaysBulk);
    },
    
    get_txtFilePathDiff: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_txtFilePathDiff() {
        /// <summary>
        /// Gets the TXT file path diff.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsLoadingControlIds.TxtFilePathDiff);
    },
    
    get_ddlFileDateTypeDiff: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_ddlFileDateTypeDiff() {
        /// <summary>
        /// Gets the DDL file date type diff.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsLoadingControlIds.DdlFileDateTypeDiff);
    },
    
    get_txtFileDateDiff: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_txtFileDateDiff() {
        /// <summary>
        /// Gets the TXT file date diff.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsLoadingControlIds.TxtFileDateDiff);
    },
    
    get_txtFileDateDaysDiff: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_txtFileDateDaysDiff() {
        /// <summary>
        /// Gets the TXT file date days diff.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsLoadingControlIds.TxtFileDateDaysDiff);
    },
    
    get_gridCustomClass: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_gridCustomClass() {
        /// <summary>
        /// Gets the grid custom class.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsLoadingControlIds.GridCustomClass);
    },
    
    get_btnSaveLoading: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_btnSaveLoading() {
        /// <summary>
        /// Gets the BTN save loading.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsLoadingControlIds.BtnSaveLoading);
    },
    
    get_hiddenIds: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_hiddenIds() {
        /// <summary>
        /// Gets the hidden ids.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsLoadingControlIds.HiddenIds);
    },
    
    get_errorCustomClass: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_errorCustomClass() {
        /// <summary>
        /// Gets the error custom class.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('errorCustomClass');
    },
    
    get_popupCustomClass: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_popupCustomClass() {
        /// <summary>
        /// Gets the popup custom class.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsLoadingControlIds.PopupCustomClass);
    },
    
    get_btnAddCustomClass: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_btnAddCustomClass() {
        /// <summary>
        /// Gets the BTN add custom class.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnAddCustomClass');
    },
    
    get_rbPreLoading: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_rbPreLoading() {
        /// <summary>
        /// Gets the rb pre loading.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('rbPreLoading');
    },
    
    get_rbPostLoading: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_rbPostLoading() {
        /// <summary>
        /// Gets the rb post loading.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('rbPostLoading');
    },
    
    get_rbScriptExec: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_rbScriptExec() {
        /// <summary>
        /// Gets the rb script exec.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('rbScriptExec');
    },
    
    get_rbCustomClass: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_rbCustomClass() {
        /// <summary>
        /// Gets the rb custom class.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('rbCustomClass');
    },
    
    get_txtClassName: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_txtClassName() {
        /// <summary>
        /// Gets the name of the TXT class.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtClassName');
    },
    
    get_txtAssemblyPath: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_txtAssemblyPath() {
        /// <summary>
        /// Gets the TXT assembly path.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtAssemblyPath');
    },
    
    get_txtSequence: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_txtSequence() {
        /// <summary>
        /// Gets the TXT sequence.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtSequence');
    },
    
    get_btnAddCustomClassToGrid: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_btnAddCustomClassToGrid() {
        /// <summary>
        /// Gets the BTN add custom class to grid.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnAddCustomClassToGrid');
    },
    
    get_errorLoading: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_errorLoading() {
        /// <summary>
        /// Gets the error loading.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('errorLoading');
    },
    
    get_btnDeleteClass: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_btnDeleteClass() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnConfirmClassDelete');
    },
    
    get_hdnLoadingData: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_hdnLoadingData() {
        /// <summary>
        /// Gets the HDN loading data.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsLoadingControlIds.HdnLoadingData);
    },
    
    get_isValid: function com_ivp_refmaster_scripts_common_RMSLoadingControls$get_isValid() {
        /// <summary>
        /// Gets the is valid.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._rmsLoadingControlIds.IsValid);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSPreferenceInfo

com.ivp.refmaster.scripts.common.RMSPreferenceInfo = function com_ivp_refmaster_scripts_common_RMSPreferenceInfo() {
    /// <field name="PreferenceName" type="String">
    /// </field>
    /// <field name="DataSourceName" type="String">
    /// </field>
    /// <field name="PreferenceID" type="Number" integer="true">
    /// </field>
    /// <field name="DataSourceID" type="Number" integer="true">
    /// </field>
    this.PreferenceName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.DataSourceName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
}
com.ivp.refmaster.scripts.common.RMSPreferenceInfo.prototype = {
    PreferenceID: 0,
    DataSourceID: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSPreferenceControlInfo

com.ivp.refmaster.scripts.common.RMSPreferenceControlInfo = function com_ivp_refmaster_scripts_common_RMSPreferenceControlInfo() {
    /// <field name="GridPreferenceMaster" type="String">
    /// </field>
    /// <field name="DdlPDataSource" type="String">
    /// </field>
    /// <field name="PopupAddFieldsId" type="String">
    /// </field>
    /// <field name="SavePreferenceSummary" type="String">
    /// </field>
    /// <field name="HiddenFieldJsn" type="String">
    /// </field>
    /// <field name="ValidationText" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSPreferenceControlInfo.prototype = {
    GridPreferenceMaster: null,
    DdlPDataSource: null,
    PopupAddFieldsId: null,
    SavePreferenceSummary: null,
    HiddenFieldJsn: null,
    ValidationText: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSPreferenceControls

com.ivp.refmaster.scripts.common.RMSPreferenceControls = function com_ivp_refmaster_scripts_common_RMSPreferenceControls(objReferenceControlids) {
    /// <param name="objReferenceControlids" type="com.ivp.refmaster.scripts.common.RMSPreferenceControlInfo">
    /// </param>
    /// <field name="_objReferenceControlsIDs" type="com.ivp.refmaster.scripts.common.RMSPreferenceControlInfo">
    /// </field>
    this._objReferenceControlsIDs = objReferenceControlids;
}
com.ivp.refmaster.scripts.common.RMSPreferenceControls.prototype = {
    _objReferenceControlsIDs: null,
    
    get_gridPrefernceMaster: function com_ivp_refmaster_scripts_common_RMSPreferenceControls$get_gridPrefernceMaster() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objReferenceControlsIDs.GridPreferenceMaster);
    },
    
    get_popUpText: function com_ivp_refmaster_scripts_common_RMSPreferenceControls$get_popUpText() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('tdPopUpText');
    },
    
    get_validationText: function com_ivp_refmaster_scripts_common_RMSPreferenceControls$get_validationText() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objReferenceControlsIDs.ValidationText);
    },
    
    get_hiddenJsnFields: function com_ivp_refmaster_scripts_common_RMSPreferenceControls$get_hiddenJsnFields() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objReferenceControlsIDs.HiddenFieldJsn);
    },
    
    get_dataSource: function com_ivp_refmaster_scripts_common_RMSPreferenceControls$get_dataSource() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objReferenceControlsIDs.DdlPDataSource);
    },
    
    get_btnAddPreference: function com_ivp_refmaster_scripts_common_RMSPreferenceControls$get_btnAddPreference() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnAddPreference');
    },
    
    get_preferenceName: function com_ivp_refmaster_scripts_common_RMSPreferenceControls$get_preferenceName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtPreferenceName');
    },
    
    get_btnSavePreferenceSummary: function com_ivp_refmaster_scripts_common_RMSPreferenceControls$get_btnSavePreferenceSummary() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objReferenceControlsIDs.SavePreferenceSummary);
    },
    
    get_popupAddPreference: function com_ivp_refmaster_scripts_common_RMSPreferenceControls$get_popupAddPreference() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objReferenceControlsIDs.PopupAddFieldsId);
    },
    
    get_btnAddPreferenceInfoToGrid: function com_ivp_refmaster_scripts_common_RMSPreferenceControls$get_btnAddPreferenceInfoToGrid() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnAddPreferenceInfo');
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSReportInfo

com.ivp.refmaster.scripts.common.RMSReportInfo = function com_ivp_refmaster_scripts_common_RMSReportInfo() {
    /// <field name="ReportId" type="Number" integer="true">
    /// </field>
    /// <field name="ReportName" type="String">
    /// </field>
    /// <field name="ReportTypeId" type="Number" integer="true">
    /// </field>
    /// <field name="EmailId" type="String">
    /// </field>
    /// <field name="DependentId" type="Number" integer="true">
    /// </field>
    /// <field name="ReportRepositoryId" type="Number" integer="true">
    /// </field>
    /// <field name="IsLegacyReport" type="Boolean">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSReportInfo.prototype = {
    ReportId: 0,
    ReportName: null,
    ReportTypeId: 0,
    EmailId: null,
    DependentId: 0,
    ReportRepositoryId: 0,
    IsLegacyReport: false
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSReportRepositoryInfo

com.ivp.refmaster.scripts.common.RMSReportRepositoryInfo = function com_ivp_refmaster_scripts_common_RMSReportRepositoryInfo() {
    /// <field name="ReportRepositoryId" type="Number" integer="true">
    /// </field>
    /// <field name="ReportRepositoryName" type="String">
    /// </field>
    /// <field name="ReportRepositoryDescription" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSReportRepositoryInfo.prototype = {
    ReportRepositoryId: 0,
    ReportRepositoryName: null,
    ReportRepositoryDescription: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSReportConfigurationInfo

com.ivp.refmaster.scripts.common.RMSReportConfigurationInfo = function com_ivp_refmaster_scripts_common_RMSReportConfigurationInfo() {
    /// <field name="reportDisplayId" type="Number" integer="true">
    /// </field>
    /// <field name="reportId" type="Number" integer="true">
    /// </field>
    /// <field name="attributeToGroupOn" type="String">
    /// </field>
    /// <field name="attributeToShow" type="String">
    /// </field>
    /// <field name="columnWidth" type="String">
    /// </field>
    /// <field name="calendarId" type="Number" integer="true">
    /// </field>
    /// <field name="decimalPlaces" type="String">
    /// </field>
    /// <field name="startDate" type="Number" integer="true">
    /// </field>
    /// <field name="endDate" type="Number" integer="true">
    /// </field>
    /// <field name="customStartDate" type="String">
    /// </field>
    /// <field name="customEndDate" type="String">
    /// </field>
    /// <field name="isSharable" type="Boolean">
    /// </field>
    /// <field name="isMultisheet" type="Boolean">
    /// </field>
    /// <field name="isIncludeDeactivatedEntity" type="Boolean">
    /// </field>
    /// <field name="sharedUsers" type="String">
    /// </field>
    /// <field name="reportHeader" type="String">
    /// </field>
    /// <field name="MsmqTransport" type="String">
    /// </field>
    /// <field name="ShowEntityCode" type="Boolean">
    /// </field>
    /// <field name="ShowDisplayName" type="Boolean">
    /// </field>
    /// <field name="IsDWHExtract" type="Boolean">
    /// </field>
    /// <field name="PushToFileTransport" type="Boolean">
    /// </field>
    /// <field name="IsLegacyReport" type="Boolean">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSReportConfigurationInfo.prototype = {
    reportDisplayId: 0,
    reportId: 0,
    attributeToGroupOn: null,
    attributeToShow: null,
    columnWidth: null,
    calendarId: 0,
    decimalPlaces: null,
    startDate: 0,
    endDate: 0,
    customStartDate: null,
    customEndDate: null,
    isSharable: false,
    isMultisheet: false,
    isIncludeDeactivatedEntity: false,
    sharedUsers: null,
    reportHeader: null,
    MsmqTransport: null,
    ShowEntityCode: false,
    ShowDisplayName: false,
    IsDWHExtract: false,
    PushToFileTransport: false,
    IsLegacyReport: false
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSReportSystemManagementInfo

com.ivp.refmaster.scripts.common.RMSReportSystemManagementInfo = function com_ivp_refmaster_scripts_common_RMSReportSystemManagementInfo() {
    /// <field name="ReportSystemId" type="Number" integer="true">
    /// </field>
    /// <field name="ReportSystemName" type="String">
    /// </field>
    /// <field name="ReportSystemDescription" type="String">
    /// </field>
    /// <field name="AssemblyPath" type="String">
    /// </field>
    /// <field name="ClassName" type="String">
    /// </field>
    /// <field name="Version" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSReportSystemManagementInfo.prototype = {
    ReportSystemId: 0,
    ReportSystemName: null,
    ReportSystemDescription: null,
    AssemblyPath: null,
    ClassName: null,
    Version: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSReportTaskStatusControlsInfo

com.ivp.refmaster.scripts.common.RMSReportTaskStatusControlsInfo = function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControlsInfo() {
    /// <field name="RdFromDateID" type="String">
    /// </field>
    /// <field name="RdPriorToDateID" type="String">
    /// </field>
    /// <field name="RdBetweenDatesID" type="String">
    /// </field>
    /// <field name="RMDateDropDownID" type="String">
    /// </field>
    /// <field name="RMDateDropDownCustomDivID" type="String">
    /// </field>
    /// <field name="RMDateDropDownTextID" type="String">
    /// </field>
    /// <field name="RMDateDropDownSelectID" type="String">
    /// </field>
    /// <field name="LblStartDate" type="String">
    /// </field>
    /// <field name="LblEndDate" type="String">
    /// </field>
    /// <field name="DateInputStartDateID" type="String">
    /// </field>
    /// <field name="DateInputEndDateID" type="String">
    /// </field>
    /// <field name="DdlAllSystemsID" type="String">
    /// </field>
    /// <field name="DdlSystemStatusID" type="String">
    /// </field>
    /// <field name="TimerTaskStatusID" type="String">
    /// </field>
    /// <field name="LblTimerControlID" type="String">
    /// </field>
    /// <field name="HdnTimerID" type="String">
    /// </field>
    /// <field name="BtnGetTasksID" type="String">
    /// </field>
    /// <field name="GridGetTasksID" type="String">
    /// </field>
    /// <field name="ModalDeleteTaskID" type="String">
    /// </field>
    /// <field name="LblTaskNameToDeleteID" type="String">
    /// </field>
    /// <field name="HiddenTaskStatusID" type="String">
    /// </field>
    /// <field name="ContextMenuID" type="String">
    /// </field>
    /// <field name="HdnGridBtn" type="String">
    /// </field>
    /// <field name="BtnClickConfirmDelete" type="String">
    /// </field>
    /// <field name="HdnReportSystemId" type="String">
    /// </field>
    /// <field name="HdnEntityCode" type="String">
    /// </field>
    /// <field name="ModalRetry" type="String">
    /// </field>
    /// <field name="OpenInNewWindow" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSReportTaskStatusControlsInfo.prototype = {
    RdFromDateID: null,
    RdPriorToDateID: null,
    RdBetweenDatesID: null,
    RMDateDropDownID: null,
    RMDateDropDownCustomDivID: null,
    RMDateDropDownTextID: null,
    RMDateDropDownSelectID: null,
    LblStartDate: null,
    LblEndDate: null,
    DateInputStartDateID: null,
    DateInputEndDateID: null,
    DdlAllSystemsID: null,
    DdlSystemStatusID: null,
    TimerTaskStatusID: null,
    LblTimerControlID: null,
    HdnTimerID: null,
    BtnGetTasksID: null,
    GridGetTasksID: null,
    ModalDeleteTaskID: null,
    LblTaskNameToDeleteID: null,
    HiddenTaskStatusID: null,
    ContextMenuID: null,
    HdnGridBtn: null,
    BtnClickConfirmDelete: null,
    HdnReportSystemId: null,
    HdnEntityCode: null,
    ModalRetry: null,
    OpenInNewWindow: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSReportTaskStatusControls

com.ivp.refmaster.scripts.common.RMSReportTaskStatusControls = function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls(controlsInfo) {
    /// <param name="controlsInfo" type="com.ivp.refmaster.scripts.common.RMSReportTaskStatusControlsInfo">
    /// </param>
    /// <field name="_controlsInfo" type="com.ivp.refmaster.scripts.common.RMSReportTaskStatusControlsInfo">
    /// </field>
    /// <field name="_rdFromDate" type="Object" domElement="true">
    /// </field>
    /// <field name="_rdPriorToDate" type="Object" domElement="true">
    /// </field>
    /// <field name="_rdBetweenDates" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDown" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDownCustomDiv" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDownText" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateDropDownSelect" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateInputStartDate" type="Object" domElement="true">
    /// </field>
    /// <field name="_dateInputEndDate" type="Object" domElement="true">
    /// </field>
    /// <field name="_ddlAllSystems" type="Object" domElement="true">
    /// </field>
    /// <field name="_ddlSystemStatus" type="Object" domElement="true">
    /// </field>
    /// <field name="_timertaskStatus" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblTimerControl" type="Object" domElement="true">
    /// </field>
    /// <field name="_hdnTimer" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnGetTasks" type="Object" domElement="true">
    /// </field>
    /// <field name="_gridGetTasks" type="Object" domElement="true">
    /// </field>
    /// <field name="_errorDiv" type="Object" domElement="true">
    /// </field>
    /// <field name="_lblTaskNameToDelete" type="Object" domElement="true">
    /// </field>
    /// <field name="_hiddenTaskStatus" type="Object" domElement="true">
    /// </field>
    /// <field name="_contextMenu" type="Object" domElement="true">
    /// </field>
    /// <field name="_hdnGridBtn" type="Object" domElement="true">
    /// </field>
    /// <field name="_btnClickConfirmDelete" type="Object" domElement="true">
    /// </field>
    /// <field name="_hdnEntityCode" type="Object" domElement="true">
    /// </field>
    /// <field name="_hdnReportSystemId" type="Object" domElement="true">
    /// </field>
    /// <field name="_modalRetry" type="Object" domElement="true">
    /// </field>
    this._controlsInfo = controlsInfo;
}
com.ivp.refmaster.scripts.common.RMSReportTaskStatusControls.prototype = {
    _controlsInfo: null,
    _rdFromDate: null,
    _rdPriorToDate: null,
    _rdBetweenDates: null,
    _dateDropDown: null,
    _dateDropDownCustomDiv: null,
    _dateDropDownText: null,
    _dateDropDownSelect: null,
    _dateInputStartDate: null,
    _dateInputEndDate: null,
    _ddlAllSystems: null,
    _ddlSystemStatus: null,
    _timertaskStatus: null,
    _lblTimerControl: null,
    _hdnTimer: null,
    _btnGetTasks: null,
    _gridGetTasks: null,
    _errorDiv: null,
    _lblTaskNameToDelete: null,
    _hiddenTaskStatus: null,
    _contextMenu: null,
    _hdnGridBtn: null,
    _btnClickConfirmDelete: null,
    _hdnEntityCode: null,
    _hdnReportSystemId: null,
    _modalRetry: null,
    
    get_rMdateDropDown: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_rMdateDropDown() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDown == null) {
            this._dateDropDown = document.getElementById(this._controlsInfo.RMDateDropDownID);
        }
        return this._dateDropDown;
    },
    
    get_rmDateDropDownCustomDiv: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_rmDateDropDownCustomDiv() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDownCustomDiv == null) {
            this._dateDropDownCustomDiv = document.getElementById(this._controlsInfo.RMDateDropDownCustomDivID);
        }
        return this._dateDropDownCustomDiv;
    },
    
    get_rmDateDropDownText: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_rmDateDropDownText() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDownText == null) {
            this._dateDropDownText = document.getElementById(this._controlsInfo.RMDateDropDownTextID);
        }
        return this._dateDropDownText;
    },
    
    get_rmDateDropDownSelect: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_rmDateDropDownSelect() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateDropDownSelect == null) {
            this._dateDropDownSelect = document.getElementById(this._controlsInfo.RMDateDropDownSelectID);
        }
        return this._dateDropDownSelect;
    },
    
    get_rdFromDate: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_rdFromDate() {
        /// <value type="Object" domElement="true"></value>
        if (this._rdFromDate == null) {
            this._rdFromDate = document.getElementById(this._controlsInfo.RdFromDateID);
        }
        return this._rdFromDate;
    },
    
    get_rdPriorToDate: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_rdPriorToDate() {
        /// <value type="Object" domElement="true"></value>
        if (this._rdPriorToDate == null) {
            this._rdPriorToDate = document.getElementById(this._controlsInfo.RdPriorToDateID);
        }
        return this._rdPriorToDate;
    },
    
    get_rdBetweenDates: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_rdBetweenDates() {
        /// <value type="Object" domElement="true"></value>
        if (this._rdBetweenDates == null) {
            this._rdBetweenDates = document.getElementById(this._controlsInfo.RdBetweenDatesID);
        }
        return this._rdBetweenDates;
    },
    
    get_lblStartDate: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_lblStartDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlsInfo.LblStartDate);
    },
    
    get_lblEndDate: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_lblEndDate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlsInfo.LblEndDate);
    },
    
    get_dateInputStartDate: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_dateInputStartDate() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateInputStartDate == null) {
            this._dateInputStartDate = document.getElementById(this._controlsInfo.DateInputStartDateID);
        }
        return this._dateInputStartDate;
    },
    
    get_dateInputEndDate: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_dateInputEndDate() {
        /// <value type="Object" domElement="true"></value>
        if (this._dateInputEndDate == null) {
            this._dateInputEndDate = document.getElementById(this._controlsInfo.DateInputEndDateID);
        }
        return this._dateInputEndDate;
    },
    
    get_ddlAllSystems: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_ddlAllSystems() {
        /// <value type="Object" domElement="true"></value>
        if (this._ddlAllSystems == null) {
            this._ddlAllSystems = document.getElementById(this._controlsInfo.DdlAllSystemsID);
        }
        return this._ddlAllSystems;
    },
    
    get_ddlSystemStatus: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_ddlSystemStatus() {
        /// <value type="Object" domElement="true"></value>
        if (this._ddlSystemStatus == null) {
            this._ddlSystemStatus = document.getElementById(this._controlsInfo.DdlSystemStatusID);
        }
        return this._ddlSystemStatus;
    },
    
    get_timerTaskStatus: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_timerTaskStatus() {
        /// <value type="Object" domElement="true"></value>
        if (this._timertaskStatus == null) {
            this._timertaskStatus = document.getElementById(this._controlsInfo.TimerTaskStatusID);
        }
        return this._timertaskStatus;
    },
    
    get_lblTimerControl: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_lblTimerControl() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblTimerControl == null) {
            this._lblTimerControl = document.getElementById(this._controlsInfo.LblTimerControlID);
        }
        return this._lblTimerControl;
    },
    
    get_hdnTimer: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_hdnTimer() {
        /// <value type="Object" domElement="true"></value>
        if (this._hdnTimer == null) {
            this._hdnTimer = document.getElementById(this._controlsInfo.HdnTimerID);
        }
        return this._hdnTimer;
    },
    
    get_btnGetTasks: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_btnGetTasks() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnGetTasks == null) {
            this._btnGetTasks = document.getElementById(this._controlsInfo.BtnGetTasksID);
        }
        return this._btnGetTasks;
    },
    
    get_gridGetTasks: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_gridGetTasks() {
        /// <value type="Object" domElement="true"></value>
        if (this._gridGetTasks == null) {
            this._gridGetTasks = document.getElementById(this._controlsInfo.GridGetTasksID);
        }
        return this._gridGetTasks;
    },
    
    get_errorDiv: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_errorDiv() {
        /// <value type="Object" domElement="true"></value>
        if (this._errorDiv == null) {
            this._errorDiv = document.getElementById('errorDiv');
        }
        return this._errorDiv;
    },
    
    get_lblTaskNameToDelete: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_lblTaskNameToDelete() {
        /// <value type="Object" domElement="true"></value>
        if (this._lblTaskNameToDelete == null) {
            this._lblTaskNameToDelete = document.getElementById(this._controlsInfo.LblTaskNameToDeleteID);
        }
        return this._lblTaskNameToDelete;
    },
    
    get_hiddenTaskStatusID: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_hiddenTaskStatusID() {
        /// <value type="Object" domElement="true"></value>
        if (this._hiddenTaskStatus == null) {
            this._hiddenTaskStatus = document.getElementById(this._controlsInfo.HiddenTaskStatusID);
        }
        return this._hiddenTaskStatus;
    },
    
    get_contextMenu: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_contextMenu() {
        /// <value type="Object" domElement="true"></value>
        if (this._contextMenu == null) {
            this._contextMenu = document.getElementById(this._controlsInfo.ContextMenuID);
        }
        return this._contextMenu;
    },
    
    get_hdnGridBtn: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_hdnGridBtn() {
        /// <value type="Object" domElement="true"></value>
        if (this._hdnGridBtn == null) {
            this._hdnGridBtn = document.getElementById(this._controlsInfo.HdnGridBtn);
        }
        return this._hdnGridBtn;
    },
    
    get_btnClickConfirmDelete: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_btnClickConfirmDelete() {
        /// <value type="Object" domElement="true"></value>
        if (this._btnClickConfirmDelete == null) {
            this._btnClickConfirmDelete = document.getElementById(this._controlsInfo.BtnClickConfirmDelete);
        }
        return this._btnClickConfirmDelete;
    },
    
    get_hdnEntityCode: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_hdnEntityCode() {
        /// <value type="Object" domElement="true"></value>
        if (this._hdnEntityCode == null) {
            this._hdnEntityCode = document.getElementById(this._controlsInfo.HdnEntityCode);
        }
        return this._hdnEntityCode;
    },
    
    get_hdnReportSystemId: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_hdnReportSystemId() {
        /// <value type="Object" domElement="true"></value>
        if (this._hdnReportSystemId == null) {
            this._hdnReportSystemId = document.getElementById(this._controlsInfo.HdnReportSystemId);
        }
        return this._hdnReportSystemId;
    },
    
    get_modalRetry: function com_ivp_refmaster_scripts_common_RMSReportTaskStatusControls$get_modalRetry() {
        /// <value type="Object" domElement="true"></value>
        if (this._modalRetry == null) {
            this._modalRetry = document.getElementById(this._controlsInfo.ModalRetry);
        }
        return this._modalRetry;
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSTaskSummaryInfo

com.ivp.refmaster.scripts.common.RMSTaskSummaryInfo = function com_ivp_refmaster_scripts_common_RMSTaskSummaryInfo() {
    /// <summary>
    /// Task Summary Info Class
    /// </summary>
    /// <field name="TaskMasterId" type="Number" integer="true">
    /// </field>
    /// <field name="TaskName" type="String">
    /// </field>
    /// <field name="TaskDescription" type="String">
    /// </field>
    this.TaskName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.TaskDescription = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
}
com.ivp.refmaster.scripts.common.RMSTaskSummaryInfo.prototype = {
    TaskMasterId: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSTaskSummaryControlIds

com.ivp.refmaster.scripts.common.RMSTaskSummaryControlIds = function com_ivp_refmaster_scripts_common_RMSTaskSummaryControlIds() {
    /// <summary>
    /// Class containing controlIds
    /// </summary>
    /// <field name="TxtTransportTaskNameId" type="String">
    /// </field>
    /// <field name="AddTaskPopupBehaviorId" type="String">
    /// </field>
    /// <field name="BtnAddTaskSummaryId" type="String">
    /// </field>
    /// <field name="TxtTaskDescriptionId" type="String">
    /// </field>
    /// <field name="HdnTaskSummaryDataId" type="String">
    /// </field>
    /// <field name="BtnPanelTaskSummaryAddId" type="String">
    /// </field>
    /// <field name="HdnIsTaskSumDataValidId" type="String">
    /// </field>
    /// <field name="TrFileDetailsId" type="String">
    /// </field>
    /// <field name="HdnTransportDetailsId" type="String">
    /// </field>
    /// <field name="GrdCustomClassId" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSTaskSummaryControlIds.prototype = {
    TxtTransportTaskNameId: null,
    AddTaskPopupBehaviorId: null,
    BtnAddTaskSummaryId: null,
    TxtTaskDescriptionId: null,
    HdnTaskSummaryDataId: null,
    BtnPanelTaskSummaryAddId: null,
    HdnIsTaskSumDataValidId: null,
    TrFileDetailsId: null,
    HdnTransportDetailsId: null,
    GrdCustomClassId: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSTaskSummaryControls

com.ivp.refmaster.scripts.common.RMSTaskSummaryControls = function com_ivp_refmaster_scripts_common_RMSTaskSummaryControls(objTaskControlIds) {
    /// <summary>
    /// Class containing controls
    /// </summary>
    /// <param name="objTaskControlIds" type="com.ivp.refmaster.scripts.common.RMSTaskSummaryControlIds">
    /// The obj task control ids.
    /// </param>
    /// <field name="_objTaskControlIds" type="com.ivp.refmaster.scripts.common.RMSTaskSummaryControlIds">
    /// </field>
    this._objTaskControlIds = objTaskControlIds;
}
com.ivp.refmaster.scripts.common.RMSTaskSummaryControls.prototype = {
    _objTaskControlIds: null,
    
    get_txtTransportTaskName: function com_ivp_refmaster_scripts_common_RMSTaskSummaryControls$get_txtTransportTaskName() {
        /// <summary>
        /// Gets the name of the TXT transport task.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskControlIds.TxtTransportTaskNameId);
    },
    
    get_txtTransportTaskDescription: function com_ivp_refmaster_scripts_common_RMSTaskSummaryControls$get_txtTransportTaskDescription() {
        /// <summary>
        /// Gets the TXT transport task description.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskControlIds.TxtTaskDescriptionId);
    },
    
    get_btnAddTaskSummary: function com_ivp_refmaster_scripts_common_RMSTaskSummaryControls$get_btnAddTaskSummary() {
        /// <summary>
        /// Gets the BTN add task summary.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskControlIds.BtnAddTaskSummaryId);
    },
    
    get_hdnTaskSummaryData: function com_ivp_refmaster_scripts_common_RMSTaskSummaryControls$get_hdnTaskSummaryData() {
        /// <summary>
        /// Gets the HDN task summary data.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskControlIds.HdnTaskSummaryDataId);
    },
    
    get_btnPanelTaskSummaryAdd: function com_ivp_refmaster_scripts_common_RMSTaskSummaryControls$get_btnPanelTaskSummaryAdd() {
        /// <summary>
        /// Gets the BTN panel task summary add.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskControlIds.BtnPanelTaskSummaryAddId);
    },
    
    get_errorTaskSummary: function com_ivp_refmaster_scripts_common_RMSTaskSummaryControls$get_errorTaskSummary() {
        /// <summary>
        /// Gets the error task summary.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('errorTask');
    },
    
    get_hdnIsTaskSumDataValid: function com_ivp_refmaster_scripts_common_RMSTaskSummaryControls$get_hdnIsTaskSumDataValid() {
        /// <summary>
        /// Gets the HDN is task sum data valid.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskControlIds.HdnIsTaskSumDataValidId);
    },
    
    get_trFileDetails: function com_ivp_refmaster_scripts_common_RMSTaskSummaryControls$get_trFileDetails() {
        /// <summary>
        /// Gets the tr file details.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskControlIds.TrFileDetailsId);
    },
    
    get_hdnTransportDetails: function com_ivp_refmaster_scripts_common_RMSTaskSummaryControls$get_hdnTransportDetails() {
        /// <summary>
        /// Gets the HDN transport details.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskControlIds.HdnTransportDetailsId);
    },
    
    get_grdCustomClass: function com_ivp_refmaster_scripts_common_RMSTaskSummaryControls$get_grdCustomClass() {
        /// <summary>
        /// Gets the GRD custom class.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTaskControlIds.GrdCustomClassId);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMEntityAttributeInfo

com.ivp.refmaster.scripts.common.RMEntityAttributeInfo = function com_ivp_refmaster_scripts_common_RMEntityAttributeInfo() {
    /// <field name="EntityAttributeId" type="String">
    /// </field>
    /// <field name="EntityTypeId" type="String">
    /// </field>
    /// <field name="AttributeName" type="String">
    /// </field>
    /// <field name="DisplayName" type="String">
    /// </field>
    /// <field name="IsNullable" type="Boolean">
    /// </field>
    /// <field name="DataType" type="com.ivp.refmaster.scripts.common.RMSDBDataTypes">
    /// </field>
    /// <field name="DataTypeLength" type="String">
    /// </field>
    /// <field name="DefaultValue" type="String">
    /// </field>
    /// <field name="IsUnique" type="Boolean">
    /// </field>
    /// <field name="IsSearchView" type="Boolean">
    /// </field>
    /// <field name="IsPrimary" type="Boolean">
    /// </field>
    /// <field name="IsClonable" type="Boolean">
    /// </field>
    /// <field name="SearchViewPosition" type="String">
    /// </field>
    /// <field name="IsInternal" type="Boolean">
    /// </field>
    /// <field name="LookupEntityTypeID" type="String">
    /// </field>
    /// <field name="LookupAttributeID" type="String">
    /// </field>
    /// <field name="LookupEntityTypeName" type="String">
    /// </field>
    /// <field name="LookupAttributeName" type="String">
    /// </field>
    /// <field name="Tags" type="String">
    /// </field>
    /// <field name="RestrictedChars" type="String">
    /// </field>
    this.DataType = com.ivp.refmaster.scripts.common.RMSDBDataTypes.VARCHARMMAX;
}
com.ivp.refmaster.scripts.common.RMEntityAttributeInfo.prototype = {
    EntityAttributeId: null,
    EntityTypeId: null,
    AttributeName: null,
    DisplayName: null,
    IsNullable: false,
    DataTypeLength: null,
    DefaultValue: null,
    IsUnique: false,
    IsSearchView: false,
    IsPrimary: false,
    IsClonable: true,
    SearchViewPosition: null,
    IsInternal: false,
    LookupEntityTypeID: null,
    LookupAttributeID: null,
    LookupEntityTypeName: null,
    LookupAttributeName: null,
    Tags: null,
    RestrictedChars: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMAddAttributeToEntityTypeControlId

com.ivp.refmaster.scripts.common.RMAddAttributeToEntityTypeControlId = function com_ivp_refmaster_scripts_common_RMAddAttributeToEntityTypeControlId() {
    /// <field name="GridEntityTypeAttributesId" type="String">
    /// </field>
    /// <field name="PopupAddAttributesId" type="String">
    /// </field>
    /// <field name="ModalAddAttributeBehavId" type="String">
    /// </field>
    /// <field name="btnSumbitAttributesId" type="String">
    /// </field>
    /// <field name="hdnJasonFilePropertyId" type="String">
    /// </field>
    /// <field name="lblAddUpdataModalId" type="String">
    /// </field>
    /// <field name="HdnEntityTypeId" type="String">
    /// </field>
    /// <field name="dateDefaultValueId" type="String">
    /// </field>
    /// <field name="is_detail" type="Boolean">
    /// </field>
    /// <field name="DeleteDetailPopup" type="String">
    /// </field>
    /// <field name="BtnCancel" type="String">
    /// </field>
    /// <field name="HdnSaveBtn" type="String">
    /// </field>
    /// <field name="IframeDeletionPopup" type="String">
    /// </field>
    /// <field name="PnlAttrDeletionPopup" type="String">
    /// </field>
    /// <field name="ModalDeletePopupBehaviorId" type="String">
    /// </field>
    /// <field name="popUpddlLookupEntityType" type="String">
    /// </field>
    /// <field name="popUpddlLookupAttribute" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMAddAttributeToEntityTypeControlId.prototype = {
    GridEntityTypeAttributesId: null,
    PopupAddAttributesId: null,
    ModalAddAttributeBehavId: null,
    btnSumbitAttributesId: null,
    hdnJasonFilePropertyId: null,
    lblAddUpdataModalId: null,
    HdnEntityTypeId: null,
    dateDefaultValueId: null,
    is_detail: false,
    DeleteDetailPopup: null,
    BtnCancel: null,
    HdnSaveBtn: null,
    IframeDeletionPopup: null,
    PnlAttrDeletionPopup: null,
    ModalDeletePopupBehaviorId: null,
    popUpddlLookupEntityType: null,
    popUpddlLookupAttribute: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSEntityAttributeControls

com.ivp.refmaster.scripts.common.RMSEntityAttributeControls = function com_ivp_refmaster_scripts_common_RMSEntityAttributeControls(objRMAddAttribute) {
    /// <param name="objRMAddAttribute" type="com.ivp.refmaster.scripts.common.RMAddAttributeToEntityTypeControlId">
    /// </param>
    /// <field name="_objRMAddAttribute" type="com.ivp.refmaster.scripts.common.RMAddAttributeToEntityTypeControlId">
    /// </field>
    this._objRMAddAttribute = objRMAddAttribute;
}
com.ivp.refmaster.scripts.common.RMSEntityAttributeControls.prototype = {
    _objRMAddAttribute: null,
    
    get_ddlLookupEntityType: function com_ivp_refmaster_scripts_common_RMSEntityAttributeControls$get_ddlLookupEntityType() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objRMAddAttribute.popUpddlLookupEntityType);
    },
    
    get_ddlLookupAttribute: function com_ivp_refmaster_scripts_common_RMSEntityAttributeControls$get_ddlLookupAttribute() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objRMAddAttribute.popUpddlLookupAttribute);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSEntityTypeInfo

com.ivp.refmaster.scripts.common.RMSEntityTypeInfo = function com_ivp_refmaster_scripts_common_RMSEntityTypeInfo() {
    /// <field name="EntityTypeID" type="Number" integer="true">
    /// </field>
    /// <field name="StructureTypeID" type="Number" integer="true">
    /// </field>
    /// <field name="EntityGroupID" type="Number" integer="true">
    /// </field>
    /// <field name="VisibleOutsideGroup" type="Boolean">
    /// </field>
    /// <field name="IsVector" type="Boolean">
    /// </field>
    /// <field name="DerivedFromEntityTypeID" type="Number" integer="true">
    /// </field>
    /// <field name="IsOneToOne" type="Boolean">
    /// </field>
    /// <field name="HasParent" type="Boolean">
    /// </field>
    /// <field name="EntityTypeName" type="String">
    /// </field>
    /// <field name="EntityDisplayName" type="String">
    /// </field>
    /// <field name="EntityCode" type="String">
    /// </field>
    /// <field name="AccountId" type="Number" integer="true">
    /// </field>
    /// <field name="Tags" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSEntityTypeInfo.prototype = {
    EntityTypeID: 0,
    StructureTypeID: 0,
    EntityGroupID: 0,
    VisibleOutsideGroup: false,
    IsVector: false,
    DerivedFromEntityTypeID: 0,
    IsOneToOne: false,
    HasParent: false,
    EntityTypeName: null,
    EntityDisplayName: null,
    EntityCode: null,
    AccountId: 0,
    Tags: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSEntityTypeGroupInfo

com.ivp.refmaster.scripts.common.RMSEntityTypeGroupInfo = function com_ivp_refmaster_scripts_common_RMSEntityTypeGroupInfo() {
    /// <field name="GroupId" type="Number" integer="true">
    /// </field>
    /// <field name="GroupName" type="String">
    /// </field>
    /// <field name="Description" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSEntityTypeGroupInfo.prototype = {
    GroupId: 0,
    GroupName: null,
    Description: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSEntityTypeTagInfo

com.ivp.refmaster.scripts.common.RMSEntityTypeTagInfo = function com_ivp_refmaster_scripts_common_RMSEntityTypeTagInfo() {
    /// <field name="TagId" type="Number" integer="true">
    /// </field>
    /// <field name="TagName" type="Number" integer="true">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSEntityTypeTagInfo.prototype = {
    TagId: 0,
    TagName: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControlIds

com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControlIds = function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControlIds() {
    /// <field name="GridEntityTypeId" type="String">
    /// </field>
    /// <field name="ModelPopupActionButton" type="String">
    /// </field>
    /// <field name="HiddenDataField" type="String">
    /// </field>
    /// <field name="HiddenValidationField" type="String">
    /// </field>
    /// <field name="EntityTypeGrid" type="String">
    /// </field>
    /// <field name="ErrorDiv" type="String">
    /// </field>
    /// <field name="ModalPopupBehaviourId" type="String">
    /// </field>
    /// <field name="ModalPopupCancelButtonId" type="String">
    /// </field>
    /// <field name="LabelPopupId" type="String">
    /// </field>
    /// <field name="HiddenEntityTypeId" type="String">
    /// </field>
    /// <field name="HiddenParentEntityTypeId" type="String">
    /// </field>
    /// <field name="DeleteRefreshButton" type="String">
    /// </field>
    /// <field name="IframeDeletionPopup" type="String">
    /// </field>
    /// <field name="PnlDeletionPopup" type="String">
    /// </field>
    /// <field name="ModalDeletePopupBehaviorId" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControlIds.prototype = {
    GridEntityTypeId: null,
    ModelPopupActionButton: null,
    HiddenDataField: null,
    HiddenValidationField: null,
    EntityTypeGrid: null,
    ErrorDiv: null,
    ModalPopupBehaviourId: null,
    ModalPopupCancelButtonId: null,
    LabelPopupId: null,
    HiddenEntityTypeId: null,
    HiddenParentEntityTypeId: null,
    DeleteRefreshButton: null,
    IframeDeletionPopup: null,
    PnlDeletionPopup: null,
    ModalDeletePopupBehaviorId: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControls

com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControls = function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls(objControlIds) {
    /// <param name="objControlIds" type="com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControlIds">
    /// </param>
    /// <field name="_buttoN_ADD_MODEL_POPUP" type="String" static="true">
    /// </field>
    /// <field name="_i_ADD_MODEL_POPUP" type="String" static="true">
    /// </field>
    /// <field name="_entitY_TYPE_DISPLAY_NAME" type="String" static="true">
    /// </field>
    /// <field name="_checkboX_IS_ONE_TO_ONE" type="String" static="true">
    /// </field>
    /// <field name="_tR_CHECK_BOX" type="String" static="true">
    /// </field>
    /// <field name="_modeL_POPUP_CONTENT_TABLE" type="String" static="true">
    /// </field>
    /// <field name="_objEntityTypeControlsIds" type="com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControlIds">
    /// </field>
    this._objEntityTypeControlsIds = objControlIds;
}
com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControls.prototype = {
    _objEntityTypeControlsIds: null,
    
    get_addModelPopupButton: function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls$get_addModelPopupButton() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControls._buttoN_ADD_MODEL_POPUP);
    },
    
    get_iAddModelPopupButton: function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls$get_iAddModelPopupButton() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControls._i_ADD_MODEL_POPUP);
    },
    
    get_entityTypeDisplayName: function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls$get_entityTypeDisplayName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControls._entitY_TYPE_DISPLAY_NAME);
    },
    
    get_checkBoxIsOneToOne: function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls$get_checkBoxIsOneToOne() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControls._checkboX_IS_ONE_TO_ONE);
    },
    
    get_modelPopupContentTable: function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls$get_modelPopupContentTable() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControls._modeL_POPUP_CONTENT_TABLE);
    },
    
    get_trCheckBoxs: function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls$get_trCheckBoxs() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControls._tR_CHECK_BOX);
    },
    
    get_iframeDeletionPopup: function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls$get_iframeDeletionPopup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeControlsIds.IframeDeletionPopup);
    },
    
    get_pnlDeletionPopup: function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls$get_pnlDeletionPopup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeControlsIds.PnlDeletionPopup);
    },
    
    get_entityTypeGrid: function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls$get_entityTypeGrid() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeControlsIds.EntityTypeGrid);
    },
    
    get_popupLabel: function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls$get_popupLabel() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeControlsIds.LabelPopupId);
    },
    
    get_modelPopupActionButton: function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls$get_modelPopupActionButton() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeControlsIds.ModelPopupActionButton);
    },
    
    get_hiddenDataField: function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls$get_hiddenDataField() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeControlsIds.HiddenDataField);
    },
    
    get_hiddenValidationField: function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls$get_hiddenValidationField() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeControlsIds.HiddenValidationField);
    },
    
    get_errorDiv: function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls$get_errorDiv() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeControlsIds.ErrorDiv);
    },
    
    get_modelPopupCancelButton: function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls$get_modelPopupCancelButton() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeControlsIds.ModalPopupCancelButtonId);
    },
    
    get_hiddenEntityTypeId: function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls$get_hiddenEntityTypeId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeControlsIds.HiddenEntityTypeId);
    },
    
    get_hiddenParentEntityTypeId: function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls$get_hiddenParentEntityTypeId() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeControlsIds.HiddenParentEntityTypeId);
    },
    
    get_deleteRefreshButton: function com_ivp_refmaster_scripts_common_RMSDetailsEntityTypeControls$get_deleteRefreshButton() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objEntityTypeControlsIds.DeleteRefreshButton);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMFeedMappingInfo

com.ivp.refmaster.scripts.common.RMFeedMappingInfo = function com_ivp_refmaster_scripts_common_RMFeedMappingInfo() {
    /// <summary>
    /// Class containg info of Feed Mapping Class
    /// </summary>
    /// <field name="FeedMappingDetailId" type="Number" integer="true">
    /// </field>
    /// <field name="FeedSummaryId" type="Number" integer="true">
    /// </field>
    /// <field name="PrimaryColumnId" type="Number" integer="true">
    /// </field>
    /// <field name="MappedColumnId" type="Number" integer="true">
    /// </field>
    /// <field name="MapId" type="Number" integer="true">
    /// </field>
    /// <field name="MapState" type="Boolean">
    /// </field>
    /// <field name="PrimaryColumnName" type="String">
    /// </field>
    /// <field name="MappedColumnName" type="String">
    /// </field>
    /// <field name="MappingName" type="String">
    /// </field>
    /// <field name="CreatedBy" type="String">
    /// </field>
    /// <field name="LastModifiedBy" type="String">
    /// </field>
    this.PrimaryColumnName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.MappedColumnName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.MappingName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.CreatedBy = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.LastModifiedBy = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
}
com.ivp.refmaster.scripts.common.RMFeedMappingInfo.prototype = {
    FeedMappingDetailId: 0,
    FeedSummaryId: 0,
    PrimaryColumnId: 0,
    MappedColumnId: 0,
    MapId: 0,
    MapState: false
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSFeedMappingControlIds

com.ivp.refmaster.scripts.common.RMSFeedMappingControlIds = function com_ivp_refmaster_scripts_common_RMSFeedMappingControlIds() {
    /// <summary>
    /// Class containing control Ids
    /// </summary>
    /// <field name="DdlFieldColumnNamesId" type="String">
    /// </field>
    /// <field name="DdlMappingNamesId" type="String">
    /// </field>
    /// <field name="GridFeedMappingId" type="String">
    /// </field>
    /// <field name="BtnSaveFeedMappingId" type="String">
    /// </field>
    /// <field name="DivErrorDisplayId" type="String">
    /// </field>
    /// <field name="HdnFeedMappingInfoId" type="String">
    /// </field>
    /// <field name="HdnFeedMappingDetailsId" type="String">
    /// </field>
    /// <field name="HdnFeedSummaryId" type="String">
    /// </field>
    /// <field name="HdnOldMappedColumnValue" type="String">
    /// </field>
    /// <field name="HdnCounter" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSFeedMappingControlIds.prototype = {
    DdlFieldColumnNamesId: null,
    DdlMappingNamesId: null,
    GridFeedMappingId: null,
    BtnSaveFeedMappingId: null,
    DivErrorDisplayId: null,
    HdnFeedMappingInfoId: null,
    HdnFeedMappingDetailsId: null,
    HdnFeedSummaryId: null,
    HdnOldMappedColumnValue: null,
    HdnCounter: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSFeedMappingControls

com.ivp.refmaster.scripts.common.RMSFeedMappingControls = function com_ivp_refmaster_scripts_common_RMSFeedMappingControls(objFeedMappingControlIds) {
    /// <summary>
    /// Class containing Controls
    /// </summary>
    /// <param name="objFeedMappingControlIds" type="com.ivp.refmaster.scripts.common.RMSFeedMappingControlIds">
    /// The obj feed mapping control ids.
    /// </param>
    /// <field name="_objFeedMappingControlIds" type="com.ivp.refmaster.scripts.common.RMSFeedMappingControlIds">
    /// </field>
    this._objFeedMappingControlIds = objFeedMappingControlIds;
}
com.ivp.refmaster.scripts.common.RMSFeedMappingControls.prototype = {
    _objFeedMappingControlIds: null,
    
    get_ddlFieldColumnNames: function com_ivp_refmaster_scripts_common_RMSFeedMappingControls$get_ddlFieldColumnNames() {
        /// <summary>
        /// Gets the DDL Field Column Names.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedMappingControlIds.DdlFieldColumnNamesId);
    },
    
    get_ddlMappingNames: function com_ivp_refmaster_scripts_common_RMSFeedMappingControls$get_ddlMappingNames() {
        /// <summary>
        /// Gets the DDL Mapping Names.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedMappingControlIds.DdlMappingNamesId);
    },
    
    get_gridFeedMapping: function com_ivp_refmaster_scripts_common_RMSFeedMappingControls$get_gridFeedMapping() {
        /// <summary>
        /// Gets the Grid Feed Mapping.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedMappingControlIds.GridFeedMappingId);
    },
    
    get_btnSaveFeedMapping: function com_ivp_refmaster_scripts_common_RMSFeedMappingControls$get_btnSaveFeedMapping() {
        /// <summary>
        /// Gets the BTN save feed mapping
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedMappingControlIds.BtnSaveFeedMappingId);
    },
    
    get_divErrorDisplay: function com_ivp_refmaster_scripts_common_RMSFeedMappingControls$get_divErrorDisplay() {
        /// <summary>
        /// Gets the DIV to display error
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedMappingControlIds.DivErrorDisplayId);
    },
    
    get_hdnFeedMappingInfo: function com_ivp_refmaster_scripts_common_RMSFeedMappingControls$get_hdnFeedMappingInfo() {
        /// <summary>
        /// Gets the HDN field for feed mapping info
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedMappingControlIds.HdnFeedMappingInfoId);
    },
    
    get_hdnFeedMappingDetail: function com_ivp_refmaster_scripts_common_RMSFeedMappingControls$get_hdnFeedMappingDetail() {
        /// <summary>
        /// Gets the HDN field for feed mapping details id
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedMappingControlIds.HdnFeedMappingDetailsId);
    },
    
    get_hdnFeedSummary: function com_ivp_refmaster_scripts_common_RMSFeedMappingControls$get_hdnFeedSummary() {
        /// <summary>
        /// Gets the HDN field for feed mapping details id
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedMappingControlIds.HdnFeedSummaryId);
    },
    
    get_hdnOldMappedColumnValue: function com_ivp_refmaster_scripts_common_RMSFeedMappingControls$get_hdnOldMappedColumnValue() {
        /// <summary>
        /// Gets the HDN old mapped column value.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedMappingControlIds.HdnOldMappedColumnValue);
    },
    
    get_txtMappedFieldName: function com_ivp_refmaster_scripts_common_RMSFeedMappingControls$get_txtMappedFieldName() {
        /// <summary>
        /// Gets the TXT mapped field name
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtMappedFieldName');
    },
    
    get_rBtnMapStateOn: function com_ivp_refmaster_scripts_common_RMSFeedMappingControls$get_rBtnMapStateOn() {
        /// <summary>
        /// Gets the RBTN Map State On
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('rbtnMapStateOn');
    },
    
    get_rBtnMapStateOff: function com_ivp_refmaster_scripts_common_RMSFeedMappingControls$get_rBtnMapStateOff() {
        /// <summary>
        /// Gets the RBTN Map State Off
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('rbtnMapStateOff');
    },
    
    get_btnAddMapping: function com_ivp_refmaster_scripts_common_RMSFeedMappingControls$get_btnAddMapping() {
        /// <summary>
        /// Gets the Add Feed Mapping
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnAddMapping');
    },
    
    get_btnUpdateMapping: function com_ivp_refmaster_scripts_common_RMSFeedMappingControls$get_btnUpdateMapping() {
        /// <summary>
        /// Gets the Update Feed Mapping
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnUpdateMapping');
    },
    
    get_btnCancelUpdate: function com_ivp_refmaster_scripts_common_RMSFeedMappingControls$get_btnCancelUpdate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnCancelUpdate');
    },
    
    get_hdnCounter: function com_ivp_refmaster_scripts_common_RMSFeedMappingControls$get_hdnCounter() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedMappingControlIds.HdnCounter);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSFeedSummaryInfo

com.ivp.refmaster.scripts.common.RMSFeedSummaryInfo = function com_ivp_refmaster_scripts_common_RMSFeedSummaryInfo() {
    /// <summary>
    /// Info Class Feed Summary
    /// </summary>
    /// <field name="FeedSummaryID" type="Number" integer="true">
    /// </field>
    /// <field name="FeedName" type="String">
    /// </field>
    /// <field name="DataSourceID" type="Number" integer="true">
    /// </field>
    /// <field name="FeedTypeID" type="Number" integer="true">
    /// </field>
    /// <field name="RADFileID" type="Number" integer="true">
    /// </field>
    /// <field name="DBProvider" type="String">
    /// </field>
    /// <field name="ConnectionString" type="String">
    /// </field>
    /// <field name="ColumnQuery" type="String">
    /// </field>
    /// <field name="IsActive" type="Boolean">
    /// </field>
    /// <field name="CreatedBy" type="String">
    /// </field>
    /// <field name="CreatedOn" type="String">
    /// </field>
    /// <field name="LastModifiedBy" type="String">
    /// </field>
    /// <field name="LastModifiedOn" type="String">
    /// </field>
    this.FeedName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.DBProvider = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.ConnectionString = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.ColumnQuery = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.CreatedBy = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.CreatedOn = com.ivp.rad.rscriptutils.RSConstants.datetimE_EMPTY;
    this.LastModifiedBy = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.LastModifiedOn = com.ivp.rad.rscriptutils.RSConstants.datetimE_EMPTY;
}
com.ivp.refmaster.scripts.common.RMSFeedSummaryInfo.prototype = {
    FeedSummaryID: 0,
    DataSourceID: 0,
    FeedTypeID: 0,
    RADFileID: 0,
    IsActive: true
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSFeedFieldDetails

com.ivp.refmaster.scripts.common.RMSFeedFieldDetails = function com_ivp_refmaster_scripts_common_RMSFeedFieldDetails() {
    /// <summary>
    /// Info class for feed field details
    /// </summary>
    /// <field name="FeedFieldDetailsId" type="Number" integer="true">
    /// </field>
    /// <field name="FeedSummaryId" type="Number" integer="true">
    /// </field>
    /// <field name="RADFieldId" type="Number" integer="true">
    /// </field>
    /// <field name="IsBulk" type="Boolean">
    /// </field>
    /// <field name="IsFTP" type="Boolean">
    /// </field>
    /// <field name="IsAPI" type="Boolean">
    /// </field>
    /// <field name="IsPrimary" type="Boolean">
    /// </field>
    /// <field name="IsUnique" type="Boolean">
    /// </field>
    /// <field name="IsActive" type="Boolean">
    /// </field>
    /// <field name="CreatedBy" type="String">
    /// </field>
    /// <field name="CreatedOn" type="String">
    /// </field>
    /// <field name="LastModifiedBy" type="String">
    /// </field>
    /// <field name="LastModifiedOn" type="String">
    /// </field>
    this.CreatedBy = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.CreatedOn = com.ivp.rad.rscriptutils.RSConstants.datetimE_EMPTY;
    this.LastModifiedBy = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.LastModifiedOn = com.ivp.rad.rscriptutils.RSConstants.datetimE_EMPTY;
}
com.ivp.refmaster.scripts.common.RMSFeedFieldDetails.prototype = {
    FeedFieldDetailsId: 0,
    FeedSummaryId: 0,
    RADFieldId: 0,
    IsBulk: false,
    IsFTP: false,
    IsAPI: false,
    IsPrimary: false,
    IsUnique: false,
    IsActive: true
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RADFileFieldDetails

com.ivp.refmaster.scripts.common.RADFileFieldDetails = function com_ivp_refmaster_scripts_common_RADFileFieldDetails() {
    /// <summary>
    /// Class containing file field details
    /// </summary>
    /// <field name="FieldId" type="Number" integer="true">
    /// </field>
    /// <field name="FileId" type="Number" integer="true">
    /// </field>
    /// <field name="FieldName" type="String">
    /// </field>
    /// <field name="FieldDescription" type="String">
    /// </field>
    /// <field name="StartIndex" type="Number" integer="true">
    /// </field>
    /// <field name="EndIndex" type="Number" integer="true">
    /// </field>
    /// <field name="FieldPosition" type="String">
    /// </field>
    /// <field name="Mandatory" type="Boolean">
    /// </field>
    /// <field name="Persistency" type="Boolean">
    /// </field>
    /// <field name="Validation" type="Boolean">
    /// </field>
    /// <field name="AllowTrim" type="Boolean">
    /// </field>
    /// <field name="FieldXPath" type="String">
    /// </field>
    /// <field name="RemoveWhiteSpaces" type="Boolean">
    /// </field>
    /// <field name="CreatedOn" type="Date">
    /// </field>
    /// <field name="CreatedBy" type="String">
    /// </field>
    /// <field name="LastModifiedOn" type="String">
    /// </field>
    /// <field name="LastModifiedBy" type="String">
    /// </field>
    /// <field name="IsActive" type="Boolean">
    /// </field>
    this.FieldName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.CreatedOn = Date.parseInvariant(com.ivp.rad.rscriptutils.RSConstants.datetimE_EMPTY);
    this.CreatedBy = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.LastModifiedOn = com.ivp.rad.rscriptutils.RSConstants.datetimE_EMPTY;
    this.LastModifiedBy = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
}
com.ivp.refmaster.scripts.common.RADFileFieldDetails.prototype = {
    FieldId: 0,
    FileId: 0,
    FieldDescription: null,
    StartIndex: 0,
    EndIndex: 0,
    FieldPosition: null,
    Mandatory: true,
    Persistency: true,
    Validation: true,
    AllowTrim: false,
    FieldXPath: null,
    RemoveWhiteSpaces: false,
    IsActive: false
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RADFileProperty

com.ivp.refmaster.scripts.common.RADFileProperty = function com_ivp_refmaster_scripts_common_RADFileProperty() {
    /// <summary>
    /// Class containong file property
    /// </summary>
    /// <field name="FileId" type="Number" integer="true">
    /// </field>
    /// <field name="FeedName" type="String">
    /// </field>
    /// <field name="FileName" type="String">
    /// </field>
    /// <field name="FileType" type="String">
    /// </field>
    /// <field name="RowDelimiter" type="String">
    /// </field>
    /// <field name="RecordLength" type="Number" integer="true">
    /// </field>
    /// <field name="FieldDelimiter" type="String">
    /// </field>
    /// <field name="CommentChar" type="String">
    /// </field>
    /// <field name="SingleEscape" type="String">
    /// </field>
    /// <field name="PairedEscape" type="String">
    /// </field>
    /// <field name="RootXPath" type="String">
    /// </field>
    /// <field name="RecordXPath" type="String">
    /// </field>
    /// <field name="ExcludeRegEx" type="String">
    /// </field>
    /// <field name="FileDate" type="Date">
    /// </field>
    /// <field name="FieldCount" type="Number" integer="true">
    /// </field>
    /// <field name="CreatedOn" type="Date">
    /// </field>
    /// <field name="CreatedBy" type="String">
    /// </field>
    /// <field name="LastModifiedOn" type="Date">
    /// </field>
    /// <field name="LastModifiedBy" type="String">
    /// </field>
    /// <field name="IsActive" type="Boolean">
    /// </field>
    this.FileType = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.FieldDelimiter = com.ivp.rad.rscriptutils.RSConstants.characteR_EMPTY;
    this.CommentChar = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.SingleEscape = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.PairedEscape = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.FileDate = Date.parseInvariant(com.ivp.rad.rscriptutils.RSConstants.datetimE_EMPTY);
    this.CreatedOn = Date.parseInvariant(com.ivp.rad.rscriptutils.RSConstants.datetimE_EMPTY);
    this.CreatedBy = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.LastModifiedOn = Date.parseInvariant(com.ivp.rad.rscriptutils.RSConstants.datetimE_EMPTY);
    this.LastModifiedBy = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
}
com.ivp.refmaster.scripts.common.RADFileProperty.prototype = {
    FileId: 0,
    FeedName: null,
    FileName: null,
    RowDelimiter: null,
    RecordLength: 0,
    RootXPath: null,
    RecordXPath: null,
    ExcludeRegEx: null,
    FieldCount: 0,
    IsActive: false
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSFeedSummaryControlIds

com.ivp.refmaster.scripts.common.RMSFeedSummaryControlIds = function com_ivp_refmaster_scripts_common_RMSFeedSummaryControlIds() {
    /// <summary>
    /// Control Ids Class of feed summary
    /// </summary>
    /// <field name="DdlFeedSourceId" type="String">
    /// </field>
    /// <field name="DdlFileTypeId" type="String">
    /// </field>
    /// <field name="TxtFeedNameId" type="String">
    /// </field>
    /// <field name="BtnSaveFeedSummaryId" type="String">
    /// </field>
    /// <field name="GridFeedSummaryId" type="String">
    /// </field>
    /// <field name="HdnJasonFilePropertyId" type="String">
    /// </field>
    /// <field name="ValSummaryFeedSummaryId" type="String">
    /// </field>
    /// <field name="ModalAddFieldsExtId" type="String">
    /// </field>
    /// <field name="PopupAddFieldsId" type="String">
    /// </field>
    /// <field name="HdnIsValidDataId" type="String">
    /// </field>
    /// <field name="BtnConnectId" type="String">
    /// </field>
    /// <field name="BtnShowColumnsId" type="String">
    /// </field>
    /// <field name="PopUpLoadFromDBId" type="String">
    /// </field>
    /// <field name="ModalLoadFromDBExtId" type="String">
    /// </field>
    /// <field name="RbServerTypeSQLId" type="String">
    /// </field>
    /// <field name="RbServerTypeOracleId" type="String">
    /// </field>
    /// <field name="TxtServerNameId" type="String">
    /// </field>
    /// <field name="TxtDataBasenameId" type="String">
    /// </field>
    /// <field name="TxtUserId" type="String">
    /// </field>
    /// <field name="txtPasswordId" type="String">
    /// </field>
    /// <field name="LoadFromDB_1" type="String">
    /// </field>
    /// <field name="BtnSaveDBConnectionId" type="String">
    /// </field>
    /// <field name="LoadFromDB_2" type="String">
    /// </field>
    /// <field name="GridColumnsId" type="String">
    /// </field>
    /// <field name="TxtWhereClauseId" type="String">
    /// </field>
    /// <field name="BtnUploadFeedId" type="String">
    /// </field>
    /// <field name="DdlTableViewId" type="String">
    /// </field>
    /// <field name="PopupFileTemplateId" type="String">
    /// </field>
    /// <field name="ModalFileTemplateExtId" type="String">
    /// </field>
    /// <field name="HdnIsFileTemplateId" type="String">
    /// </field>
    /// <field name="LblErrorFileUploadId" type="String">
    /// </field>
    /// <field name="TdLoadFromDB_1_Message" type="String">
    /// </field>
    /// <field name="LblErrorDetailsId" type="String">
    /// </field>
    /// <field name="RdBtnSqlAuthen" type="String">
    /// </field>
    /// <field name="RdBtnWinAuthen" type="String">
    /// </field>
    /// <field name="UserIDTR" type="String">
    /// </field>
    /// <field name="PasswordTR" type="String">
    /// </field>
    /// <field name="ServerNameTR" type="String">
    /// </field>
    /// <field name="DBNameTR" type="String">
    /// </field>
    /// <field name="AuthTypeTR" type="String">
    /// </field>
    /// <field name="RdBtnDirectConn" type="String">
    /// </field>
    /// <field name="RdBtnIndirectConn" type="String">
    /// </field>
    /// <field name="ConnectionStringTR" type="String">
    /// </field>
    /// <field name="LoadFromDB_4" type="String">
    /// </field>
    /// <field name="LnkDownloadFileTemplate" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSFeedSummaryControlIds.prototype = {
    DdlFeedSourceId: null,
    DdlFileTypeId: null,
    TxtFeedNameId: null,
    BtnSaveFeedSummaryId: null,
    GridFeedSummaryId: null,
    HdnJasonFilePropertyId: null,
    ValSummaryFeedSummaryId: null,
    ModalAddFieldsExtId: null,
    PopupAddFieldsId: null,
    HdnIsValidDataId: null,
    BtnConnectId: null,
    BtnShowColumnsId: null,
    PopUpLoadFromDBId: null,
    ModalLoadFromDBExtId: null,
    RbServerTypeSQLId: null,
    RbServerTypeOracleId: null,
    TxtServerNameId: null,
    TxtDataBasenameId: null,
    TxtUserId: null,
    txtPasswordId: null,
    LoadFromDB_1: null,
    BtnSaveDBConnectionId: null,
    LoadFromDB_2: null,
    GridColumnsId: null,
    TxtWhereClauseId: null,
    BtnUploadFeedId: null,
    DdlTableViewId: null,
    PopupFileTemplateId: null,
    ModalFileTemplateExtId: null,
    HdnIsFileTemplateId: null,
    LblErrorFileUploadId: null,
    TdLoadFromDB_1_Message: null,
    LblErrorDetailsId: null,
    RdBtnSqlAuthen: null,
    RdBtnWinAuthen: null,
    UserIDTR: null,
    PasswordTR: null,
    ServerNameTR: null,
    DBNameTR: null,
    AuthTypeTR: null,
    RdBtnDirectConn: null,
    RdBtnIndirectConn: null,
    ConnectionStringTR: null,
    LoadFromDB_4: null,
    LnkDownloadFileTemplate: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSFeedSummaryControls

com.ivp.refmaster.scripts.common.RMSFeedSummaryControls = function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls(objFeedSummaryControlIds) {
    /// <summary>
    /// Control Class of Feed Summary
    /// </summary>
    /// <param name="objFeedSummaryControlIds" type="com.ivp.refmaster.scripts.common.RMSFeedSummaryControlIds">
    /// The obj feed summary control ids.
    /// </param>
    /// <field name="_objFeedSummaryControlIds" type="com.ivp.refmaster.scripts.common.RMSFeedSummaryControlIds">
    /// </field>
    this._objFeedSummaryControlIds = objFeedSummaryControlIds;
}
com.ivp.refmaster.scripts.common.RMSFeedSummaryControls.prototype = {
    _objFeedSummaryControlIds: null,
    
    get_ddlFeedSource: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_ddlFeedSource() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.DdlFeedSourceId);
    },
    
    get_ddlFileType: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_ddlFileType() {
        /// <summary>
        /// Gets the DropDown for type of the file.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.DdlFileTypeId);
    },
    
    get_txtFeedName: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtFeedName() {
        /// <summary>
        /// Gets the name of the feed.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.TxtFeedNameId);
    },
    
    get_txtRecordDelimiter: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtRecordDelimiter() {
        /// <summary>
        /// Gets the TXT record delimiter.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtRecordDelimiter');
    },
    
    get_txtFieldDelimiter: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtFieldDelimiter() {
        /// <summary>
        /// Gets the TXT field delimiter.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtFieldDelimiter');
    },
    
    get_txtCommentChar: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtCommentChar() {
        /// <summary>
        /// Gets the TXT comment char.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtCommentChar');
    },
    
    get_txtSingleEscape: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtSingleEscape() {
        /// <summary>
        /// Gets the TXT single escape.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtSingleEscape');
    },
    
    get_txtPairedEscape: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtPairedEscape() {
        /// <summary>
        /// Gets the TXT paired escape.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtPairedEscape');
    },
    
    get_txtExcludeRegEx: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtExcludeRegEx() {
        /// <summary>
        /// Gets the TXT exclude reg ex.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtExcludeRegex');
    },
    
    get_rbRecordDelimiter: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_rbRecordDelimiter() {
        /// <summary>
        /// Gets the rb record delimiter.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('rbRecordDelimiter');
    },
    
    get_txtRecordLength: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtRecordLength() {
        /// <summary>
        /// Gets the length of the TXT record.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtRecordLength');
    },
    
    get_txtRecordXPath: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtRecordXPath() {
        /// <summary>
        /// Gets the TXT record X path.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtRecordXPath');
    },
    
    get_txtRootXPath: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtRootXPath() {
        /// <summary>
        /// Gets the TXT root X path.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtRootXPath');
    },
    
    get_lblExcludeRegEx: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_lblExcludeRegEx() {
        /// <summary>
        /// Gets the LBL exclude reg ex.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblExcludeRegEx');
    },
    
    get_lblCommentChar: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_lblCommentChar() {
        /// <summary>
        /// Gets the LBL comment char.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblCommentChar');
    },
    
    get_lblFieldDelimiter: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_lblFieldDelimiter() {
        /// <summary>
        /// Gets the LBL field delimiter id.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblFieldDelimiter');
    },
    
    get_grdFeedSummary: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_grdFeedSummary() {
        /// <summary>
        /// Gets the GRD feed summary.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.GridFeedSummaryId);
    },
    
    get_btnSaveFeedSummary: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_btnSaveFeedSummary() {
        /// <summary>
        /// Gets the BTN save feed summary.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.BtnSaveFeedSummaryId);
    },
    
    get_hdnJasonFileProperty: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_hdnJasonFileProperty() {
        /// <summary>
        /// Gets the HDN jason file property.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.HdnJasonFilePropertyId);
    },
    
    get_rbRecordLength: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_rbRecordLength() {
        /// <summary>
        /// Gets the length of the rb record.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('rbRecordLength');
    },
    
    get_valSummaryFeedSummary: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_valSummaryFeedSummary() {
        /// <summary>
        /// Gets the val summary feed summary.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.ValSummaryFeedSummaryId);
    },
    
    get_btnShowFileType: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_btnShowFileType() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnShowFileType');
    },
    
    get_trFeedType: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_trFeedType() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('trFeedType');
    },
    
    get_btnAddFieldDetails: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_btnAddFieldDetails() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnAddFieldDetails');
    },
    
    get_lblAddUpdateFields: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_lblAddUpdateFields() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblAddUpdateFields');
    },
    
    get_txtFieldName: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtFieldName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtFieldName');
    },
    
    get_txtFieldDescription: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtFieldDescription() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtFieldDescription');
    },
    
    get_txtFieldStartIndex: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtFieldStartIndex() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtFieldStartIndex');
    },
    
    get_txtFieldEndIndex: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtFieldEndIndex() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtFieldEndIndex');
    },
    
    get_txtFieldPosition: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtFieldPosition() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtFieldPosition');
    },
    
    get_txtFieldXPath: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtFieldXPath() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtFieldXPath');
    },
    
    get_chkMandatory: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_chkMandatory() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('chkMandatory');
    },
    
    get_chkPersist: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_chkPersist() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('chkPersist');
    },
    
    get_chkAllowTrim: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_chkAllowTrim() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('chkAllowTrim');
    },
    
    get_chkRemoveWhiteSpace: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_chkRemoveWhiteSpace() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('chkRemoveWhiteSpace');
    },
    
    get_chkValidation: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_chkValidation() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('chkValidation');
    },
    
    get_chkBulk: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_chkBulk() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('chkBulk');
    },
    
    get_chkFTP: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_chkFTP() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('chkFTP');
    },
    
    get_chkAPI: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_chkAPI() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('chkAPI');
    },
    
    get_chkPrimary: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_chkPrimary() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('chkPrimary');
    },
    
    get_chkUnique: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_chkUnique() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('chkUnique');
    },
    
    get_errorFieldDetails: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_errorFieldDetails() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('ErrorFieldDetails');
    },
    
    get_btnAddFieldsPopup: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_btnAddFieldsPopup() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnAddFields');
    },
    
    get_popupAddFields: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_popupAddFields() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.PopupAddFieldsId);
    },
    
    get_tablePopUp: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_tablePopUp() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('tablePopUp');
    },
    
    get_hdnIsValidData: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_hdnIsValidData() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.HdnIsValidDataId);
    },
    
    get_lblFileUploadErrorMessage: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_lblFileUploadErrorMessage() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.LblErrorFileUploadId);
    },
    
    get_tdLoadFromDB_1_Message: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_tdLoadFromDB_1_Message() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.TdLoadFromDB_1_Message);
    },
    
    get_lnkDownloadFileTemplate: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_lnkDownloadFileTemplate() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.LnkDownloadFileTemplate);
    },
    
    get_loadFromDB_1: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_loadFromDB_1() {
        /// <summary>
        /// Gets the load from D B_1.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.LoadFromDB_1);
    },
    
    get_rbServerTypeSQL: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_rbServerTypeSQL() {
        /// <summary>
        /// Gets the rb server type SQL.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.RbServerTypeSQLId);
    },
    
    get_rbServerTypeOracle: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_rbServerTypeOracle() {
        /// <summary>
        /// Gets the rb server type oracle.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.RbServerTypeOracleId);
    },
    
    get_txtServerName: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtServerName() {
        /// <summary>
        /// Gets the name of the TXT server.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.TxtServerNameId);
    },
    
    get_txtDataBaseName: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtDataBaseName() {
        /// <summary>
        /// Gets the name of the TXT data base.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.TxtDataBasenameId);
    },
    
    get_txtUserId: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtUserId() {
        /// <summary>
        /// Gets the TXT user id.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.TxtUserId);
    },
    
    get_txtPassword: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtPassword() {
        /// <summary>
        /// Gets the TXT password.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.txtPasswordId);
    },
    
    get_errorLoadFromDB_1: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_errorLoadFromDB_1() {
        /// <summary>
        /// Gets the error load from D B_1.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('ErrorLoadFromDB_1');
    },
    
    get_divLoadFromDb: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_divLoadFromDb() {
        /// <summary>
        /// Gets the div load from db.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('divLoadFromDB');
    },
    
    get_btnConnect: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_btnConnect() {
        /// <summary>
        /// Gets the BTN connect.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.BtnConnectId);
    },
    
    get_btnSaveDBConnection: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_btnSaveDBConnection() {
        /// <summary>
        /// Gets the BTN save DB connection.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.BtnSaveDBConnectionId);
    },
    
    get_btnPanelCancelLoadDBConnection: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_btnPanelCancelLoadDBConnection() {
        /// <summary>
        /// Gets the BTN panel cancel load DB connection.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnPanelCancelLoadDBConnection');
    },
    
    get_loadFromDB_2: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_loadFromDB_2() {
        /// <summary>
        /// Gets the load from D B_2.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.LoadFromDB_2);
    },
    
    get_btnShowColumns: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_btnShowColumns() {
        /// <summary>
        /// Gets the BTN show columns.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.BtnShowColumnsId);
    },
    
    get_errorShowColumns: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_errorShowColumns() {
        /// <summary>
        /// Gets the error show columns.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('ErrorShowColumns');
    },
    
    get_popUpLoadFromDB: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_popUpLoadFromDB() {
        /// <summary>
        /// Gets the pop up load from DB.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.PopUpLoadFromDBId);
    },
    
    get_btnPanelCancelLoadFromDB: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_btnPanelCancelLoadFromDB() {
        /// <summary>
        /// Gets the BTN panel cancel load from DB.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnPanelCancelLoadFromDB');
    },
    
    get_btnReconfigureDBConnection: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_btnReconfigureDBConnection() {
        /// <summary>
        /// Gets the BTN reconfigure DB connection.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnReconfigureDBConnection');
    },
    
    get_gridColumns: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_gridColumns() {
        /// <summary>
        /// Gets the grid columns.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.GridColumnsId);
    },
    
    get_txtWhereClause: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtWhereClause() {
        /// <summary>
        /// Gets the TXT where clause.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.TxtWhereClauseId);
    },
    
    get_btnUploadFeed: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_btnUploadFeed() {
        /// <summary>
        /// Gets the BTN upload feed.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.BtnUploadFeedId);
    },
    
    get_ddlTableView: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_ddlTableView() {
        /// <summary>
        /// Gets the DDL table view.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.DdlTableViewId);
    },
    
    get_lblWhereClause: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_lblWhereClause() {
        /// <summary>
        /// Gets the LBL where clause.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblWhereClause');
    },
    
    get_txtWhereClauseForm: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_txtWhereClauseForm() {
        /// <summary>
        /// Gets the TXT where clause form.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('txtWhereClause');
    },
    
    get_popupFileTemplate: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_popupFileTemplate() {
        /// <summary>
        /// Gets the popup file template.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.PopupFileTemplateId);
    },
    
    get_hdnIsFileTemplate: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_hdnIsFileTemplate() {
        /// <summary>
        /// Gets the HDN is file template.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.HdnIsFileTemplateId);
    },
    
    get_btnShowForm: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_btnShowForm() {
        /// <summary>
        /// Gets the BTN show form.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnShowForm');
    },
    
    get_lblErrorDetails: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_lblErrorDetails() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.LblErrorDetailsId);
    },
    
    get_rdBtnSqlAuthen: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_rdBtnSqlAuthen() {
        /// <summary>
        /// Gets the rd BTN win authen.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.RdBtnSqlAuthen);
    },
    
    get_rdBtnWinAuthen: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_rdBtnWinAuthen() {
        /// <summary>
        /// Gets the rd BTN win authen.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.RdBtnWinAuthen);
    },
    
    get_userIDTR: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_userIDTR() {
        /// <summary>
        /// Gets the user IDTR.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.UserIDTR);
    },
    
    get_passwordTR: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_passwordTR() {
        /// <summary>
        /// Gets the password TR.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.PasswordTR);
    },
    
    get_serverNameTR: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_serverNameTR() {
        /// <summary>
        /// Gets the server name TR.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.ServerNameTR);
    },
    
    get_dbNameTR: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_dbNameTR() {
        /// <summary>
        /// Gets the DB name TR.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.DBNameTR);
    },
    
    get_authTypeTR: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_authTypeTR() {
        /// <summary>
        /// Gets the auth type TR.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.AuthTypeTR);
    },
    
    get_rdBtnDirectConn: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_rdBtnDirectConn() {
        /// <summary>
        /// Gets the rd BTN direct conn.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.RdBtnDirectConn);
    },
    
    get_rdBtnIndirectConn: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_rdBtnIndirectConn() {
        /// <summary>
        /// Gets the rd BTN indirect conn.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.RdBtnIndirectConn);
    },
    
    get_connectionStringTR: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_connectionStringTR() {
        /// <summary>
        /// Gets the connection string TR.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.ConnectionStringTR);
    },
    
    get_loadFromDB_4: function com_ivp_refmaster_scripts_common_RMSFeedSummaryControls$get_loadFromDB_4() {
        /// <summary>
        /// Gets the connection string TR.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objFeedSummaryControlIds.LoadFromDB_4);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSTabManagementInfo

com.ivp.refmaster.scripts.common.RMSTabManagementInfo = function com_ivp_refmaster_scripts_common_RMSTabManagementInfo() {
    /// <summary>
    /// Info class of Tab Management
    /// </summary>
    /// <field name="TabId" type="Number" integer="true">
    /// </field>
    /// <field name="TabName" type="String">
    /// </field>
    /// <field name="TabIndex" type="Number" integer="true">
    /// </field>
    /// <field name="TabAttributeXML" type="String">
    /// </field>
    this.TabName = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    this.TabAttributeXML = com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
}
com.ivp.refmaster.scripts.common.RMSTabManagementInfo.prototype = {
    TabId: 0,
    TabIndex: 0
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSTabManagementControlIds

com.ivp.refmaster.scripts.common.RMSTabManagementControlIds = function com_ivp_refmaster_scripts_common_RMSTabManagementControlIds() {
    /// <summary>
    /// class containing control ids
    /// </summary>
    /// <field name="BtnSaveAllId" type="String">
    /// </field>
    /// <field name="GridAddTabId" type="String">
    /// </field>
    /// <field name="ListTabId" type="String">
    /// </field>
    /// <field name="ModalAddTabId" type="String">
    /// </field>
    /// <field name="ModalUpdateTabId" type="String">
    /// </field>
    /// <field name="TextboxTab" type="String">
    /// </field>
    /// <field name="TextBoxTabUpdateName" type="String">
    /// </field>
    /// <field name="HdnTabManagementInfoId" type="String">
    /// </field>
    /// <field name="HdnTabID" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSTabManagementControlIds.prototype = {
    BtnSaveAllId: null,
    GridAddTabId: null,
    ListTabId: null,
    ModalAddTabId: null,
    ModalUpdateTabId: null,
    TextboxTab: null,
    TextBoxTabUpdateName: null,
    HdnTabManagementInfoId: null,
    HdnTabID: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSTabManagementControl

com.ivp.refmaster.scripts.common.RMSTabManagementControl = function com_ivp_refmaster_scripts_common_RMSTabManagementControl(objTabManagementControlIds) {
    /// <summary>
    /// Class defining the controlIds of tab management
    /// </summary>
    /// <param name="objTabManagementControlIds" type="com.ivp.refmaster.scripts.common.RMSTabManagementControlIds">
    /// </param>
    /// <field name="_objTabManagementControlIds" type="com.ivp.refmaster.scripts.common.RMSTabManagementControlIds">
    /// </field>
    this._objTabManagementControlIds = objTabManagementControlIds;
}
com.ivp.refmaster.scripts.common.RMSTabManagementControl.prototype = {
    _objTabManagementControlIds: null,
    
    get_btnSaveAll: function com_ivp_refmaster_scripts_common_RMSTabManagementControl$get_btnSaveAll() {
        /// <summary>
        /// Gets the BTN save all.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTabManagementControlIds.BtnSaveAllId);
    },
    
    get_gridAddTabId: function com_ivp_refmaster_scripts_common_RMSTabManagementControl$get_gridAddTabId() {
        /// <summary>
        /// Gets the grid add tab id.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTabManagementControlIds.GridAddTabId);
    },
    
    get_lstTabId: function com_ivp_refmaster_scripts_common_RMSTabManagementControl$get_lstTabId() {
        /// <summary>
        /// Gets the LST tab id.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTabManagementControlIds.ListTabId);
    },
    
    get_textboxTab: function com_ivp_refmaster_scripts_common_RMSTabManagementControl$get_textboxTab() {
        /// <summary>
        /// Gets the textbox tab.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTabManagementControlIds.TextboxTab);
    },
    
    get_textBoxTabUpdateName: function com_ivp_refmaster_scripts_common_RMSTabManagementControl$get_textBoxTabUpdateName() {
        /// <summary>
        /// Gets the name of the text box tab update.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTabManagementControlIds.TextBoxTabUpdateName);
    },
    
    get_hdnTabManagementInfoId: function com_ivp_refmaster_scripts_common_RMSTabManagementControl$get_hdnTabManagementInfoId() {
        /// <summary>
        /// Gets the HDN tab management info id.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTabManagementControlIds.HdnTabManagementInfoId);
    },
    
    get_hdnTabID: function com_ivp_refmaster_scripts_common_RMSTabManagementControl$get_hdnTabID() {
        /// <summary>
        /// Gets the HDN tab ID.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._objTabManagementControlIds.HdnTabID);
    },
    
    get_btnAddTab: function com_ivp_refmaster_scripts_common_RMSTabManagementControl$get_btnAddTab() {
        /// <summary>
        /// Gets the BTN add tab.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnAddTab');
    },
    
    get_btnTabSave: function com_ivp_refmaster_scripts_common_RMSTabManagementControl$get_btnTabSave() {
        /// <summary>
        /// Gets the BTN tab save.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnTabSave');
    },
    
    get_btnTabSaveCancel: function com_ivp_refmaster_scripts_common_RMSTabManagementControl$get_btnTabSaveCancel() {
        /// <summary>
        /// Gets the BTN tab save cancel.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnTabCancel');
    },
    
    get_btnTabUpdate: function com_ivp_refmaster_scripts_common_RMSTabManagementControl$get_btnTabUpdate() {
        /// <summary>
        /// Gets the BTN tab update.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnTabUpdate');
    },
    
    get_btnTabUpdateCancel: function com_ivp_refmaster_scripts_common_RMSTabManagementControl$get_btnTabUpdateCancel() {
        /// <summary>
        /// Gets the BTN tab update cancel.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnTabUpdateCancel');
    },
    
    get_btnTabPriorityUp: function com_ivp_refmaster_scripts_common_RMSTabManagementControl$get_btnTabPriorityUp() {
        /// <summary>
        /// Gets the BTN tab priority up.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('imgTabPriorityUp');
    },
    
    get_btnTabPriorityDown: function com_ivp_refmaster_scripts_common_RMSTabManagementControl$get_btnTabPriorityDown() {
        /// <summary>
        /// Gets the BTN tab priority down.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('imgTabPriorityDown');
    },
    
    get_lblTabNameError: function com_ivp_refmaster_scripts_common_RMSTabManagementControl$get_lblTabNameError() {
        /// <summary>
        /// Gets the LBL tab name error.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblTabNameError');
    },
    
    get_lblTabUpdateNameError: function com_ivp_refmaster_scripts_common_RMSTabManagementControl$get_lblTabUpdateNameError() {
        /// <summary>
        /// Gets the LBL tab update name error.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblTabUpdateNameError');
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSTransportControlIds

com.ivp.refmaster.scripts.common.RMSTransportControlIds = function com_ivp_refmaster_scripts_common_RMSTransportControlIds() {
    /// <summary>
    /// Class containing control ids of transport task
    /// </summary>
    /// <field name="BtnDeleteTransportId" type="String">
    /// </field>
    /// <field name="PanelAddTransportTaskId" type="String">
    /// </field>
    /// <field name="DdlTransportTypeId" type="String">
    /// </field>
    /// <field name="DdlFileDateTypeId" type="String">
    /// </field>
    /// <field name="TxtCustomDateId" type="String">
    /// </field>
    /// <field name="BtnSaveTransportId" type="String">
    /// </field>
    /// <field name="HdnIsValidDataId" type="String">
    /// </field>
    /// <field name="TxtTrasnportRemoteFileId" type="String">
    /// </field>
    /// <field name="TxtTransportRemoteLocationId" type="String">
    /// </field>
    /// <field name="TxtTransportlocalFileId" type="String">
    /// </field>
    /// <field name="TxtTransportlocalLocationId" type="String">
    /// </field>
    /// <field name="TxtGpgUserNameId" type="String">
    /// </field>
    /// <field name="TxtGpgUserPassphraseId" type="String">
    /// </field>
    /// <field name="TxtFileDateDaysId" type="String">
    /// </field>
    /// <field name="ErrorTransportTaskId" type="String">
    /// </field>
    /// <field name="ModalTransportBehId" type="String">
    /// </field>
    /// <field name="HdnTransportTaskId" type="String">
    /// </field>
    /// <field name="ChkUseDefaultPathId" type="String">
    /// </field>
    /// <field name="ChkExtractAllId" type="String">
    /// </field>
    /// <field name="HdnStateId" type="String">
    /// </field>
    /// <field name="SearchTransportTask" type="String">
    /// </field>
    /// <field name="gridTaskSummary" type="String">
    /// </field>
    /// <field name="cvTaskName" type="String">
    /// </field>
    /// <field name="ValidationSummary1" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSTransportControlIds.prototype = {
    BtnDeleteTransportId: null,
    PanelAddTransportTaskId: null,
    DdlTransportTypeId: null,
    DdlFileDateTypeId: null,
    TxtCustomDateId: null,
    BtnSaveTransportId: null,
    HdnIsValidDataId: null,
    TxtTrasnportRemoteFileId: null,
    TxtTransportRemoteLocationId: null,
    TxtTransportlocalFileId: null,
    TxtTransportlocalLocationId: null,
    TxtGpgUserNameId: null,
    TxtGpgUserPassphraseId: null,
    TxtFileDateDaysId: null,
    ErrorTransportTaskId: null,
    ModalTransportBehId: null,
    HdnTransportTaskId: null,
    ChkUseDefaultPathId: null,
    ChkExtractAllId: null,
    HdnStateId: null,
    SearchTransportTask: null,
    gridTaskSummary: null,
    cvTaskName: null,
    ValidationSummary1: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSTransportControls

com.ivp.refmaster.scripts.common.RMSTransportControls = function com_ivp_refmaster_scripts_common_RMSTransportControls(_controlIds) {
    /// <summary>
    /// Class containing controls of transport task
    /// </summary>
    /// <param name="_controlIds" type="com.ivp.refmaster.scripts.common.RMSTransportControlIds">
    /// The _control ids.
    /// </param>
    /// <field name="_controlIds" type="com.ivp.refmaster.scripts.common.RMSTransportControlIds">
    /// </field>
    this._controlIds = _controlIds;
}
com.ivp.refmaster.scripts.common.RMSTransportControls.prototype = {
    _controlIds: null,
    
    get_btnDeleteTransport: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_btnDeleteTransport() {
        /// <summary>
        /// Gets the BTN delete transport.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnDeleteTransportId);
    },
    
    get_panelAddTransportTask: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_panelAddTransportTask() {
        /// <summary>
        /// Gets the panel add transport task.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.PanelAddTransportTaskId);
    },
    
    get_ddlTransportType: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_ddlTransportType() {
        /// <summary>
        /// Gets the type of the DDL transport.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.DdlTransportTypeId);
    },
    
    get_txtRemoteFileName: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_txtRemoteFileName() {
        /// <summary>
        /// Gets the name of the TXT remote file.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtTrasnportRemoteFileId);
    },
    
    get_txtRemoteFileLocation: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_txtRemoteFileLocation() {
        /// <summary>
        /// Gets the TXT remote file location.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtTransportRemoteLocationId);
    },
    
    get_txtLocalFileName: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_txtLocalFileName() {
        /// <summary>
        /// Gets the name of the TXT local file.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtTransportlocalFileId);
    },
    
    get_txtLocalFileLocation: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_txtLocalFileLocation() {
        /// <summary>
        /// Gets the TXT local file location.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtTransportlocalLocationId);
    },
    
    get_txtGpGUserName: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_txtGpGUserName() {
        /// <summary>
        /// Gets the name of the TXT gp G user.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtGpgUserNameId);
    },
    
    get_txtGpGPassPhrase: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_txtGpGPassPhrase() {
        /// <summary>
        /// Gets the TXT gp G pass phrase.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtGpgUserPassphraseId);
    },
    
    get_ddlFileDateType: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_ddlFileDateType() {
        /// <summary>
        /// Gets the type of the DDL file date.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.DdlFileDateTypeId);
    },
    
    get_txtFileDateDays: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_txtFileDateDays() {
        /// <summary>
        /// Gets the TXT file date days.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtFileDateDaysId);
    },
    
    get_txtCustomDate: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_txtCustomDate() {
        /// <summary>
        /// Gets the TXT custom date.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtCustomDateId);
    },
    
    get_btnAddTransport: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_btnAddTransport() {
        /// <summary>
        /// Gets the BTN add transport.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnSaveTransportId);
    },
    
    get_txtIsValidData: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_txtIsValidData() {
        /// <summary>
        /// Gets the TXT is valid data.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnIsValidDataId);
    },
    
    get_errorTransportTask: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_errorTransportTask() {
        /// <summary>
        /// Gets the validation summary.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.ErrorTransportTaskId);
    },
    
    get_hdnTransportTask: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_hdnTransportTask() {
        /// <summary>
        /// Gets the HDN transport task.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnTransportTaskId);
    },
    
    get_chkUseDefaultPath: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_chkUseDefaultPath() {
        /// <summary>
        /// Gets the CHK use default path.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.ChkUseDefaultPathId);
    },
    
    get_chkExtractAll: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_chkExtractAll() {
        /// <summary>
        /// Gets the CHK extract all.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.ChkExtractAllId);
    },
    
    get_lblSwitchState: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_lblSwitchState() {
        /// <summary>
        /// Gets the state of the LBL switch.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblSwitchState');
    },
    
    get_lblAddUpdateTransportTask: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_lblAddUpdateTransportTask() {
        /// <summary>
        /// Gets the LBL add update transport task.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblAddUpdateTransportTask');
    },
    
    get_hdnState: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_hdnState() {
        /// <summary>
        /// Gets the state of the HDN.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnStateId);
    },
    
    get_searchTransportTask: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_searchTransportTask() {
        /// <summary>
        /// Gets the search transport task.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.SearchTransportTask);
    },
    
    get_gridTaskSummary: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_gridTaskSummary() {
        /// <summary>
        /// Gets the grid task summary.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.gridTaskSummary);
    },
    
    get_cvTaskName: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_cvTaskName() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.cvTaskName);
    },
    
    get_validationSummary1: function com_ivp_refmaster_scripts_common_RMSTransportControls$get_validationSummary1() {
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.ValidationSummary1);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSTaskControlIds

com.ivp.refmaster.scripts.common.RMSTaskControlIds = function com_ivp_refmaster_scripts_common_RMSTaskControlIds() {
    /// <summary>
    /// Control Ids Class of Task
    /// </summary>
    /// <field name="BtnDeleteTaskId" type="String">
    /// </field>
    /// <field name="ModalDeleteTaskBehId" type="String">
    /// </field>
    /// <field name="ModalAddTaskBehId" type="String">
    /// </field>
    /// <field name="BtnSaveTaskSummaryId" type="String">
    /// </field>
    /// <field name="TxtTaskNameId" type="String">
    /// </field>
    /// <field name="PanelAddTaskId" type="String">
    /// </field>
    /// <field name="ErrorTaskId" type="String">
    /// </field>
    /// <field name="TxtTaskDescriptionId" type="String">
    /// </field>
    /// <field name="HdnAddUpdateId" type="String">
    /// </field>
    /// <field name="HdnTaskMasterIdValueId" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSTaskControlIds.prototype = {
    BtnDeleteTaskId: null,
    ModalDeleteTaskBehId: null,
    ModalAddTaskBehId: null,
    BtnSaveTaskSummaryId: null,
    TxtTaskNameId: null,
    PanelAddTaskId: null,
    ErrorTaskId: null,
    TxtTaskDescriptionId: null,
    HdnAddUpdateId: null,
    HdnTaskMasterIdValueId: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSTaskControls

com.ivp.refmaster.scripts.common.RMSTaskControls = function com_ivp_refmaster_scripts_common_RMSTaskControls(controlIDs) {
    /// <summary>
    /// Controls Class of Task
    /// </summary>
    /// <param name="controlIDs" type="com.ivp.refmaster.scripts.common.RMSTaskControlIds">
    /// The control I ds.
    /// </param>
    /// <field name="_controlIds" type="com.ivp.refmaster.scripts.common.RMSTaskControlIds">
    /// </field>
    this._controlIds = controlIDs;
}
com.ivp.refmaster.scripts.common.RMSTaskControls.prototype = {
    _controlIds: null,
    
    get_btnDeleteTask: function com_ivp_refmaster_scripts_common_RMSTaskControls$get_btnDeleteTask() {
        /// <summary>
        /// Gets the BTN delete task.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnDeleteTaskId);
    },
    
    get_btnSaveTaskSummaryId: function com_ivp_refmaster_scripts_common_RMSTaskControls$get_btnSaveTaskSummaryId() {
        /// <summary>
        /// Gets the BTN save task summary id.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnSaveTaskSummaryId);
    },
    
    get_txtTaskName: function com_ivp_refmaster_scripts_common_RMSTaskControls$get_txtTaskName() {
        /// <summary>
        /// Gets the name of the TXT task.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtTaskNameId);
    },
    
    get_panelAddTask: function com_ivp_refmaster_scripts_common_RMSTaskControls$get_panelAddTask() {
        /// <summary>
        /// Gets the panel add task.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.PanelAddTaskId);
    },
    
    get_errorTask: function com_ivp_refmaster_scripts_common_RMSTaskControls$get_errorTask() {
        /// <summary>
        /// Gets the error task.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.ErrorTaskId);
    },
    
    get_btnShowTaskSummaryPopup: function com_ivp_refmaster_scripts_common_RMSTaskControls$get_btnShowTaskSummaryPopup() {
        /// <summary>
        /// Gets the BTN show task summary popup.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('btnAddTaskSummary');
    },
    
    get_txtTaskDescription: function com_ivp_refmaster_scripts_common_RMSTaskControls$get_txtTaskDescription() {
        /// <summary>
        /// Gets the TXT task description.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtTaskDescriptionId);
    },
    
    get_hdnAddUpdate: function com_ivp_refmaster_scripts_common_RMSTaskControls$get_hdnAddUpdate() {
        /// <summary>
        /// Gets the HDN add update.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnAddUpdateId);
    },
    
    get_hdnTaskMasterIdValue: function com_ivp_refmaster_scripts_common_RMSTaskControls$get_hdnTaskMasterIdValue() {
        /// <summary>
        /// Gets the HDN task master id value.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnTaskMasterIdValueId);
    },
    
    get_lblTaskSummary: function com_ivp_refmaster_scripts_common_RMSTaskControls$get_lblTaskSummary() {
        /// <summary>
        /// Gets the LBL task summary.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById('lblTaskSummary');
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSCustomControlIds

com.ivp.refmaster.scripts.common.RMSCustomControlIds = function com_ivp_refmaster_scripts_common_RMSCustomControlIds() {
    /// <summary>
    /// Control Ids class of Custom Class
    /// </summary>
    /// <field name="BtnDeleteCustomClassId" type="String">
    /// </field>
    /// <field name="PanelCustomClassId" type="String">
    /// </field>
    /// <field name="RbScriptExecutableId" type="String">
    /// </field>
    /// <field name="RbCustomClassId" type="String">
    /// </field>
    /// <field name="TxtClassNameId" type="String">
    /// </field>
    /// <field name="TxtAssemblyPathId" type="String">
    /// </field>
    /// <field name="TxtSequenceNumberId" type="String">
    /// </field>
    /// <field name="ErrorCustomClassId" type="String">
    /// </field>
    /// <field name="BtnAddCustomClassId" type="String">
    /// </field>
    /// <field name="ModalCustomClassBehId" type="String">
    /// </field>
    /// <field name="HdnCustomClassId" type="String">
    /// </field>
    /// <field name="RbPreCustomId" type="String">
    /// </field>
    /// <field name="RbPostCustomId" type="String">
    /// </field>
    /// <field name="LblCustomClassId" type="String">
    /// </field>
    /// <field name="HdnCustomClassSequenceId" type="String">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSCustomControlIds.prototype = {
    BtnDeleteCustomClassId: null,
    PanelCustomClassId: null,
    RbScriptExecutableId: null,
    RbCustomClassId: null,
    TxtClassNameId: null,
    TxtAssemblyPathId: null,
    TxtSequenceNumberId: null,
    ErrorCustomClassId: null,
    BtnAddCustomClassId: null,
    ModalCustomClassBehId: null,
    HdnCustomClassId: null,
    RbPreCustomId: null,
    RbPostCustomId: null,
    LblCustomClassId: null,
    HdnCustomClassSequenceId: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSCustomControls

com.ivp.refmaster.scripts.common.RMSCustomControls = function com_ivp_refmaster_scripts_common_RMSCustomControls(controlIds) {
    /// <summary>
    /// Control Class of Custom Class
    /// </summary>
    /// <param name="controlIds" type="com.ivp.refmaster.scripts.common.RMSCustomControlIds">
    /// The control ids.
    /// </param>
    /// <field name="_controlIds" type="com.ivp.refmaster.scripts.common.RMSCustomControlIds">
    /// </field>
    this._controlIds = controlIds;
}
com.ivp.refmaster.scripts.common.RMSCustomControls.prototype = {
    _controlIds: null,
    
    get_btnDeleteCustomClass: function com_ivp_refmaster_scripts_common_RMSCustomControls$get_btnDeleteCustomClass() {
        /// <summary>
        /// Gets the BTN delete custom class.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnDeleteCustomClassId);
    },
    
    get_panelCustomClass: function com_ivp_refmaster_scripts_common_RMSCustomControls$get_panelCustomClass() {
        /// <summary>
        /// Gets the panel custom class.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.PanelCustomClassId);
    },
    
    get_rbScriptExecutable: function com_ivp_refmaster_scripts_common_RMSCustomControls$get_rbScriptExecutable() {
        /// <summary>
        /// Gets the rb script executable.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.RbScriptExecutableId);
    },
    
    get_rbCustomClass: function com_ivp_refmaster_scripts_common_RMSCustomControls$get_rbCustomClass() {
        /// <summary>
        /// Gets the rb custom class.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.RbCustomClassId);
    },
    
    get_txtClassName: function com_ivp_refmaster_scripts_common_RMSCustomControls$get_txtClassName() {
        /// <summary>
        /// Gets the name of the TXT class.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtClassNameId);
    },
    
    get_txtAssembleName: function com_ivp_refmaster_scripts_common_RMSCustomControls$get_txtAssembleName() {
        /// <summary>
        /// Gets the name of the TXT assemble.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtAssemblyPathId);
    },
    
    get_txtSequence: function com_ivp_refmaster_scripts_common_RMSCustomControls$get_txtSequence() {
        /// <summary>
        /// Gets the TXT sequence.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.TxtSequenceNumberId);
    },
    
    get_errorCustomClass: function com_ivp_refmaster_scripts_common_RMSCustomControls$get_errorCustomClass() {
        /// <summary>
        /// Gets the error custom class.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.ErrorCustomClassId);
    },
    
    get_btnAddCustomClass: function com_ivp_refmaster_scripts_common_RMSCustomControls$get_btnAddCustomClass() {
        /// <summary>
        /// Gets the BTN add custom class.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.BtnAddCustomClassId);
    },
    
    get_hdnCustomClassId: function com_ivp_refmaster_scripts_common_RMSCustomControls$get_hdnCustomClassId() {
        /// <summary>
        /// Gets the HDN custom class id.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnCustomClassId);
    },
    
    get_rbPostCustom: function com_ivp_refmaster_scripts_common_RMSCustomControls$get_rbPostCustom() {
        /// <summary>
        /// Gets the rb post custom.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.RbPostCustomId);
    },
    
    get_rbPreCustom: function com_ivp_refmaster_scripts_common_RMSCustomControls$get_rbPreCustom() {
        /// <summary>
        /// Gets the rb pre custom.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.RbPreCustomId);
    },
    
    get_lblCustomClass: function com_ivp_refmaster_scripts_common_RMSCustomControls$get_lblCustomClass() {
        /// <summary>
        /// Gets the LBL custom class.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.LblCustomClassId);
    },
    
    get_hdnCustomClassSequence: function com_ivp_refmaster_scripts_common_RMSCustomControls$get_hdnCustomClassSequence() {
        /// <summary>
        /// Gets the HDN custom class sequence.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return document.getElementById(this._controlIds.HdnCustomClassSequenceId);
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSTransportTaskInfo

com.ivp.refmaster.scripts.common.RMSTransportTaskInfo = function com_ivp_refmaster_scripts_common_RMSTransportTaskInfo() {
    /// <summary>
    /// Info Class of Transport Task
    /// </summary>
    /// <field name="TransportMasterId" type="Number" integer="true">
    /// </field>
    /// <field name="TransportDetailsId" type="Number" integer="true">
    /// </field>
    /// <field name="TransportName" type="String">
    /// </field>
    /// <field name="RemoteFileName" type="String">
    /// </field>
    /// <field name="RemoteFileLocation" type="String">
    /// </field>
    /// <field name="LocalFileName" type="String">
    /// </field>
    /// <field name="LocalFileLocation" type="String">
    /// </field>
    /// <field name="UseDefaultPath" type="Boolean">
    /// </field>
    /// <field name="FileDateType" type="String">
    /// </field>
    /// <field name="CustomDate" type="String">
    /// </field>
    /// <field name="FileDateDays" type="Number" integer="true">
    /// </field>
    /// <field name="CustomCallExists" type="Boolean">
    /// </field>
    /// <field name="GpgPassPhrase" type="String">
    /// </field>
    /// <field name="GpgUserName" type="String">
    /// </field>
    /// <field name="ExtractAll" type="Boolean">
    /// </field>
    /// <field name="State" type="Boolean">
    /// </field>
}
com.ivp.refmaster.scripts.common.RMSTransportTaskInfo.prototype = {
    TransportMasterId: 0,
    TransportDetailsId: 0,
    TransportName: null,
    RemoteFileName: null,
    RemoteFileLocation: null,
    LocalFileName: null,
    LocalFileLocation: null,
    UseDefaultPath: false,
    FileDateType: null,
    CustomDate: null,
    FileDateDays: 0,
    CustomCallExists: false,
    GpgPassPhrase: null,
    GpgUserName: null,
    ExtractAll: false,
    State: false
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.scripts.common.RMSSearchMasterDetailGrid

com.ivp.refmaster.scripts.common.RMSSearchMasterDetailGrid = function com_ivp_refmaster_scripts_common_RMSSearchMasterDetailGrid() {
}
com.ivp.refmaster.scripts.common.RMSSearchMasterDetailGrid.searchMasterDetailGrid = function com_ivp_refmaster_scripts_common_RMSSearchMasterDetailGrid$searchMasterDetailGrid(SearchText, MasterGridClientId, MasterGridServerId, DetailGridServerId, DetailGridSummary, HeaderInnerText) {
    /// <summary>
    /// Searches the data source.
    /// </summary>
    /// <param name="SearchText" type="String">
    /// The search text.
    /// </param>
    /// <param name="MasterGridClientId" type="String">
    /// The master grid client id.
    /// </param>
    /// <param name="MasterGridServerId" type="String">
    /// The master grid server id.
    /// </param>
    /// <param name="DetailGridServerId" type="String">
    /// The detail grid server id.
    /// </param>
    /// <param name="DetailGridSummary" type="String">
    /// The detail grid summary.
    /// </param>
    /// <param name="HeaderInnerText" type="String">
    /// The header inner text.
    /// </param>
    var EntityGroupGrid = document.getElementById(MasterGridClientId).children[1];
    var tr = null;
    for (var i = 0; i < EntityGroupGrid.rows.length; i++) {
        tr = EntityGroupGrid.rows[i];
        if (!(tr.id.search(new RegExp(MasterGridServerId, 'i')) > 0 && tr.id.search(new RegExp(DetailGridServerId, 'i')) < 0)) {
            continue;
        }
        tr.style.display = '';
        if (tr.cells[0].className === 'collapseGridButton') {
            eval('RAD_ToggleRow(tr.cells[0]);');
        }
        if (tr.innerText.replace(new RegExp(DetailGridSummary, 'i'), '').search(new RegExp(SearchText, 'i')) < 0) {
            if (tr.cells[0].className != null) {
                if (tr.cells[0].className === 'expandGridButton' || tr.cells[0].className === 'collapseGridButton') {
                    if (tr.nextSibling.innerText.replace(new RegExp(DetailGridSummary, 'i'), '').replace(new RegExp(HeaderInnerText, 'i'), '').search(new RegExp(SearchText, 'i')) > 0) {
                        tr.style.display = '';
                        continue;
                    }
                    else if (SearchText !== '') {
                        tr.style.display = 'none';
                    }
                }
                else if (tr.cells[0].className !== 'expandGridButton' || tr.cells[0].className !== 'collapseGridButton') {
                    if (tr.cells[0].innerText.search(new RegExp(SearchText, 'i')) > 0) {
                        tr.style.display = '';
                        continue;
                    }
                    else if (SearchText !== '') {
                        tr.style.display = 'none';
                    }
                }
            }
            else if (SearchText !== '') {
                tr.style.display = 'none';
            }
        }
    }
}
com.ivp.refmaster.scripts.common.RMSSearchMasterDetailGrid.disableControlsPanel = function com_ivp_refmaster_scripts_common_RMSSearchMasterDetailGrid$disableControlsPanel(ele, disable) {
    /// <param name="ele" type="Object" domElement="true">
    /// </param>
    /// <param name="disable" type="Boolean">
    /// </param>
    var inputCollection = ele.getElementsByTagName('INPUT');
    for (var i = 0; i < inputCollection.length; i++) {
        if (inputCollection[i].getAttribute('type').toString() === 'text') {
            (inputCollection[i]).disabled = disable;
            (inputCollection[i]).title = (inputCollection[i]).value;
        }
        if (inputCollection[i].getAttribute('type').toString() === 'checkbox') {
            (inputCollection[i]).disabled = disable;
        }
        if (inputCollection[i].getAttribute('type').toString() === 'radio') {
            (inputCollection[i]).disabled = disable;
        }
        if (inputCollection[i].getAttribute('type').toString() === 'image') {
            (inputCollection[i]).disabled = disable;
        }
        if (inputCollection[i].getAttribute('type').toString() === 'submit' || inputCollection[i].getAttribute('type').toString() === 'button') {
            (inputCollection[i]).disabled = disable;
        }
    }
    inputCollection = ele.getElementsByTagName('SELECT');
    for (var i = 0; i < inputCollection.length; i++) {
        (inputCollection[i]).disabled = disable;
    }
    inputCollection = ele.getElementsByTagName('LABEL');
    for (var i = 0; i < inputCollection.length; i++) {
        inputCollection[i].disabled = disable;
    }
}


Type.registerNamespace('com.ivp.refmaster.refmasterwebservices');

////////////////////////////////////////////////////////////////////////////////
// com.ivp.refmaster.refmasterwebservices.RMSWebServiceException

com.ivp.refmaster.refmasterwebservices.RMSWebServiceException = function com_ivp_refmaster_refmasterwebservices_RMSWebServiceException() {
}
com.ivp.refmaster.refmasterwebservices.RMSWebServiceException.prototype = {
    
    get_message: function com_ivp_refmaster_refmasterwebservices_RMSWebServiceException$get_message() {
        /// <summary>
        /// Get_messages this instance.
        /// </summary>
        /// <returns type="String"></returns>
        return com.ivp.rad.rscriptutils.RSConstants.strinG_EMPTY;
    },
    
    get_statusCode: function com_ivp_refmaster_refmasterwebservices_RMSWebServiceException$get_statusCode() {
        /// <summary>
        /// Get_statuses the code.
        /// </summary>
        /// <returns type="Number" integer="true"></returns>
        return 0;
    }
}


com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSPromoteVendorInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSPromoteVendorInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSPromoteVendorInfoControlIDs.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSPromoteVendorInfoControlIDs');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSPromoteVendorControls.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSPromoteVendorControls');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationControlInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationControlInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationControls.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationControls');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationPerAttributeInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationPerAttributeInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationPerAttributeControlInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationPerAttributeControlInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationPerAttributeControls.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleCreationPerAttributeControls');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSEntitytypeRuleAttributeInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSEntitytypeRuleAttributeInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleUINewInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleUINewInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleUINewControlInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleUINewControlInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleUINewControls.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRuleUINewControls');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSDataReconciliationInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSDataReconciliationInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSReconDataControlIds.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSReconDataControlIds');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSDataReconciliationControls.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSDataReconciliationControls');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSReconGroupInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSReconGroupInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReconSourceDetailsInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReconSourceDetailsInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReconAttrMappingInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReconAttrMappingInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReconBreakTypeInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReconBreakTypeInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReportSetupControlsInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReportSetupControlsInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReportSetupControls.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMReportSetupControls');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSBackgroundTaskStatusInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSBackgroundTaskStatusInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMBackGroundTaskStatusControlIDs.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMBackGroundTaskStatusControlIDs');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSBackgroundTaskStatusControls.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSBackgroundTaskStatusControls');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRealTimePreferenceInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRealTimePreferenceInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMRealTimePreferenceControlIDs.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMRealTimePreferenceControlIDs');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRealTimePreferenceControls.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRealTimePreferenceControls');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRulesControlIds.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRulesControlIds');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRulesControls.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSRulesControls');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchADVDataInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchADVDataInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchADVDataControlIds.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchADVDataControlIds');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchADVDataControls.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchADVDataControls');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchDataInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchDataInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchDataControlIds.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchDataControlIds');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchDataControls.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSSearchDataControls');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSTaskStatusInfo.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSTaskStatusInfo');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSTaskStatusControlIDs.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSTaskStatusControlIDs');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSTaskStatusControls.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSTaskStatusControls');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSUIRulesControlIds.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSUIRulesControlIds');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSUIRulesControls.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMSUIRulesControls');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMUserBasedLayoutControlIds.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMUserBasedLayoutControlIds');
com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMUserBasedLayoutControls.registerClass('com.ivp.refmaster.scripts.common.RefMasterJSInfo.RMUserBasedLayoutControls');
RefMasterJSCommon.RMSCommons.registerClass('RefMasterJSCommon.RMSCommons');
com.ivp.refmaster.scripts.common.RMSActionIdentifierInfo.registerClass('com.ivp.refmaster.scripts.common.RMSActionIdentifierInfo');
com.ivp.refmaster.scripts.common.RMSActionIdentifierControlIds.registerClass('com.ivp.refmaster.scripts.common.RMSActionIdentifierControlIds');
com.ivp.refmaster.scripts.common.RMSActionIdentifierControls.registerClass('com.ivp.refmaster.scripts.common.RMSActionIdentifierControls');
com.ivp.refmaster.scripts.common.RMAPIEntityTypeInfo.registerClass('com.ivp.refmaster.scripts.common.RMAPIEntityTypeInfo');
com.ivp.refmaster.scripts.common.RMSAttributeManagementInfo.registerClass('com.ivp.refmaster.scripts.common.RMSAttributeManagementInfo');
com.ivp.refmaster.scripts.common.RMSAttributeManagementControlsIds.registerClass('com.ivp.refmaster.scripts.common.RMSAttributeManagementControlsIds');
com.ivp.refmaster.scripts.common.RMSAttributeManagementControls.registerClass('com.ivp.refmaster.scripts.common.RMSAttributeManagementControls');
com.ivp.refmaster.scripts.common.RMSAuditInfo.registerClass('com.ivp.refmaster.scripts.common.RMSAuditInfo');
com.ivp.refmaster.scripts.common.AuditSearchControlIds.registerClass('com.ivp.refmaster.scripts.common.AuditSearchControlIds');
com.ivp.refmaster.scripts.common.AuditSearchControls.registerClass('com.ivp.refmaster.scripts.common.AuditSearchControls');
com.ivp.refmaster.scripts.common.RMSBulkEntityCreateUpdateInfo.registerClass('com.ivp.refmaster.scripts.common.RMSBulkEntityCreateUpdateInfo');
com.ivp.refmaster.scripts.common.RMSBulkEntityCreateUpdateControlInfo.registerClass('com.ivp.refmaster.scripts.common.RMSBulkEntityCreateUpdateControlInfo');
com.ivp.refmaster.scripts.common.RMSBulkEntityCreateUpdateControl.registerClass('com.ivp.refmaster.scripts.common.RMSBulkEntityCreateUpdateControl');
com.ivp.refmaster.scripts.common.RMSEntityCreationInfo.registerClass('com.ivp.refmaster.scripts.common.RMSEntityCreationInfo');
com.ivp.refmaster.scripts.common.EntityCreationControlIds.registerClass('com.ivp.refmaster.scripts.common.EntityCreationControlIds');
com.ivp.refmaster.scripts.common.EntityCreationControls.registerClass('com.ivp.refmaster.scripts.common.EntityCreationControls');
com.ivp.refmaster.scripts.common.RMFileAttributeMapInfo.registerClass('com.ivp.refmaster.scripts.common.RMFileAttributeMapInfo');
com.ivp.refmaster.scripts.common.AttributeValueInfo.registerClass('com.ivp.refmaster.scripts.common.AttributeValueInfo');
com.ivp.refmaster.scripts.common.RMRuleExecutionStatusInfo.registerClass('com.ivp.refmaster.scripts.common.RMRuleExecutionStatusInfo');
com.ivp.refmaster.scripts.common.RMOverridenAttrInfo.registerClass('com.ivp.refmaster.scripts.common.RMOverridenAttrInfo');
com.ivp.refmaster.scripts.common.RMSEntityUpdationInfo.registerClass('com.ivp.refmaster.scripts.common.RMSEntityUpdationInfo');
com.ivp.refmaster.scripts.common.RMSEntityUpdationControls.registerClass('com.ivp.refmaster.scripts.common.RMSEntityUpdationControls');
com.ivp.refmaster.scripts.common.RMSExceptionManagerInfo.registerClass('com.ivp.refmaster.scripts.common.RMSExceptionManagerInfo');
com.ivp.refmaster.scripts.common.ExceptionManagerUpdateInfo.registerClass('com.ivp.refmaster.scripts.common.ExceptionManagerUpdateInfo');
com.ivp.refmaster.scripts.common.RMSExceptionTypes.registerClass('com.ivp.refmaster.scripts.common.RMSExceptionTypes');
com.ivp.refmaster.scripts.common.RMSSuppressDeleteConstants.registerClass('com.ivp.refmaster.scripts.common.RMSSuppressDeleteConstants');
com.ivp.refmaster.scripts.common.RMSExceptionManagerDSInfo.registerClass('com.ivp.refmaster.scripts.common.RMSExceptionManagerDSInfo');
com.ivp.refmaster.scripts.common.RMSGridColumnIndexInfo.registerClass('com.ivp.refmaster.scripts.common.RMSGridColumnIndexInfo');
com.ivp.refmaster.scripts.common.ExternalSystemInfo.registerClass('com.ivp.refmaster.scripts.common.ExternalSystemInfo');
com.ivp.refmaster.scripts.common.RMSExceptionManagerControlInfo.registerClass('com.ivp.refmaster.scripts.common.RMSExceptionManagerControlInfo');
com.ivp.refmaster.scripts.common.RMSExceptionManagerControls.registerClass('com.ivp.refmaster.scripts.common.RMSExceptionManagerControls');
com.ivp.refmaster.scripts.common.RMReportAttributeInfo.registerClass('com.ivp.refmaster.scripts.common.RMReportAttributeInfo');
com.ivp.refmaster.scripts.common.RMReportAttributeSetupControlsId.registerClass('com.ivp.refmaster.scripts.common.RMReportAttributeSetupControlsId');
com.ivp.refmaster.scripts.common.RMReportAttributeSetupControls.registerClass('com.ivp.refmaster.scripts.common.RMReportAttributeSetupControls');
com.ivp.refmaster.scripts.common.RMSAttributeLookupInfo.registerClass('com.ivp.refmaster.scripts.common.RMSAttributeLookupInfo');
com.ivp.refmaster.scripts.common.RMSAttributeLookupControlInfo.registerClass('com.ivp.refmaster.scripts.common.RMSAttributeLookupControlInfo');
com.ivp.refmaster.scripts.common.RMSAttributeLookupControls.registerClass('com.ivp.refmaster.scripts.common.RMSAttributeLookupControls');
com.ivp.refmaster.scripts.common.RMSDropDownInfo.registerClass('com.ivp.refmaster.scripts.common.RMSDropDownInfo');
com.ivp.refmaster.scripts.common.RMSCustomClassInfo.registerClass('com.ivp.refmaster.scripts.common.RMSCustomClassInfo');
com.ivp.refmaster.scripts.common.RMSDatasourceInfo.registerClass('com.ivp.refmaster.scripts.common.RMSDatasourceInfo');
com.ivp.refmaster.scripts.common.RMFeedSummaryInfo.registerClass('com.ivp.refmaster.scripts.common.RMFeedSummaryInfo');
com.ivp.refmaster.scripts.common.RMSEntityPrioritizationInfo.registerClass('com.ivp.refmaster.scripts.common.RMSEntityPrioritizationInfo');
com.ivp.refmaster.scripts.common.DatasourcePriorityInfo.registerClass('com.ivp.refmaster.scripts.common.DatasourcePriorityInfo');
com.ivp.refmaster.scripts.common.RMSEntityTypePriortizationControlId.registerClass('com.ivp.refmaster.scripts.common.RMSEntityTypePriortizationControlId');
com.ivp.refmaster.scripts.common.RMSEntityTypePriortizationControls.registerClass('com.ivp.refmaster.scripts.common.RMSEntityTypePriortizationControls');
com.ivp.refmaster.scripts.common.RMEntityTypeFeedMappingDetailsInfo.registerClass('com.ivp.refmaster.scripts.common.RMEntityTypeFeedMappingDetailsInfo');
com.ivp.refmaster.scripts.common.RMEntityAttributesFeedFieldsMappingInfo.registerClass('com.ivp.refmaster.scripts.common.RMEntityAttributesFeedFieldsMappingInfo');
com.ivp.refmaster.scripts.common.RMEntityFeedAttributeLookup.registerClass('com.ivp.refmaster.scripts.common.RMEntityFeedAttributeLookup');
com.ivp.refmaster.scripts.common.RMSEntityTypeFeedMappingDetailsControlIds.registerClass('com.ivp.refmaster.scripts.common.RMSEntityTypeFeedMappingDetailsControlIds');
com.ivp.refmaster.scripts.common.RMSEntityTypeFeedMappingDetailsControls.registerClass('com.ivp.refmaster.scripts.common.RMSEntityTypeFeedMappingDetailsControls');
com.ivp.refmaster.scripts.common.RMSParentChildMappingInfo.registerClass('com.ivp.refmaster.scripts.common.RMSParentChildMappingInfo');
com.ivp.refmaster.scripts.common.RMSTaskInfo.registerClass('com.ivp.refmaster.scripts.common.RMSTaskInfo');
com.ivp.refmaster.scripts.common.RMSScheduledJobInfo.registerClass('com.ivp.refmaster.scripts.common.RMSScheduledJobInfo');
com.ivp.refmaster.scripts.common.RMSFlowSetupControlIds.registerClass('com.ivp.refmaster.scripts.common.RMSFlowSetupControlIds');
com.ivp.refmaster.scripts.common.RMSFlowSetupControl.registerClass('com.ivp.refmaster.scripts.common.RMSFlowSetupControl');
com.ivp.refmaster.scripts.common.RMSLoadingTaskInfo.registerClass('com.ivp.refmaster.scripts.common.RMSLoadingTaskInfo');
com.ivp.refmaster.scripts.common.RMSLoadingTaskControlIds.registerClass('com.ivp.refmaster.scripts.common.RMSLoadingTaskControlIds');
com.ivp.refmaster.scripts.common.RMSLoadingControls.registerClass('com.ivp.refmaster.scripts.common.RMSLoadingControls');
com.ivp.refmaster.scripts.common.RMSPreferenceInfo.registerClass('com.ivp.refmaster.scripts.common.RMSPreferenceInfo');
com.ivp.refmaster.scripts.common.RMSPreferenceControlInfo.registerClass('com.ivp.refmaster.scripts.common.RMSPreferenceControlInfo');
com.ivp.refmaster.scripts.common.RMSPreferenceControls.registerClass('com.ivp.refmaster.scripts.common.RMSPreferenceControls');
com.ivp.refmaster.scripts.common.RMSReportInfo.registerClass('com.ivp.refmaster.scripts.common.RMSReportInfo');
com.ivp.refmaster.scripts.common.RMSReportRepositoryInfo.registerClass('com.ivp.refmaster.scripts.common.RMSReportRepositoryInfo');
com.ivp.refmaster.scripts.common.RMSReportConfigurationInfo.registerClass('com.ivp.refmaster.scripts.common.RMSReportConfigurationInfo');
com.ivp.refmaster.scripts.common.RMSReportSystemManagementInfo.registerClass('com.ivp.refmaster.scripts.common.RMSReportSystemManagementInfo');
com.ivp.refmaster.scripts.common.RMSReportTaskStatusControlsInfo.registerClass('com.ivp.refmaster.scripts.common.RMSReportTaskStatusControlsInfo');
com.ivp.refmaster.scripts.common.RMSReportTaskStatusControls.registerClass('com.ivp.refmaster.scripts.common.RMSReportTaskStatusControls');
com.ivp.refmaster.scripts.common.RMSTaskSummaryInfo.registerClass('com.ivp.refmaster.scripts.common.RMSTaskSummaryInfo');
com.ivp.refmaster.scripts.common.RMSTaskSummaryControlIds.registerClass('com.ivp.refmaster.scripts.common.RMSTaskSummaryControlIds');
com.ivp.refmaster.scripts.common.RMSTaskSummaryControls.registerClass('com.ivp.refmaster.scripts.common.RMSTaskSummaryControls');
com.ivp.refmaster.scripts.common.RMEntityAttributeInfo.registerClass('com.ivp.refmaster.scripts.common.RMEntityAttributeInfo');
com.ivp.refmaster.scripts.common.RMAddAttributeToEntityTypeControlId.registerClass('com.ivp.refmaster.scripts.common.RMAddAttributeToEntityTypeControlId');
com.ivp.refmaster.scripts.common.RMSEntityAttributeControls.registerClass('com.ivp.refmaster.scripts.common.RMSEntityAttributeControls');
com.ivp.refmaster.scripts.common.RMSEntityTypeInfo.registerClass('com.ivp.refmaster.scripts.common.RMSEntityTypeInfo');
com.ivp.refmaster.scripts.common.RMSEntityTypeGroupInfo.registerClass('com.ivp.refmaster.scripts.common.RMSEntityTypeGroupInfo');
com.ivp.refmaster.scripts.common.RMSEntityTypeTagInfo.registerClass('com.ivp.refmaster.scripts.common.RMSEntityTypeTagInfo');
com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControlIds.registerClass('com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControlIds');
com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControls.registerClass('com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControls');
com.ivp.refmaster.scripts.common.RMFeedMappingInfo.registerClass('com.ivp.refmaster.scripts.common.RMFeedMappingInfo');
com.ivp.refmaster.scripts.common.RMSFeedMappingControlIds.registerClass('com.ivp.refmaster.scripts.common.RMSFeedMappingControlIds');
com.ivp.refmaster.scripts.common.RMSFeedMappingControls.registerClass('com.ivp.refmaster.scripts.common.RMSFeedMappingControls');
com.ivp.refmaster.scripts.common.RMSFeedSummaryInfo.registerClass('com.ivp.refmaster.scripts.common.RMSFeedSummaryInfo');
com.ivp.refmaster.scripts.common.RMSFeedFieldDetails.registerClass('com.ivp.refmaster.scripts.common.RMSFeedFieldDetails');
com.ivp.refmaster.scripts.common.RADFileFieldDetails.registerClass('com.ivp.refmaster.scripts.common.RADFileFieldDetails');
com.ivp.refmaster.scripts.common.RADFileProperty.registerClass('com.ivp.refmaster.scripts.common.RADFileProperty');
com.ivp.refmaster.scripts.common.RMSFeedSummaryControlIds.registerClass('com.ivp.refmaster.scripts.common.RMSFeedSummaryControlIds');
com.ivp.refmaster.scripts.common.RMSFeedSummaryControls.registerClass('com.ivp.refmaster.scripts.common.RMSFeedSummaryControls');
com.ivp.refmaster.scripts.common.RMSTabManagementInfo.registerClass('com.ivp.refmaster.scripts.common.RMSTabManagementInfo');
com.ivp.refmaster.scripts.common.RMSTabManagementControlIds.registerClass('com.ivp.refmaster.scripts.common.RMSTabManagementControlIds');
com.ivp.refmaster.scripts.common.RMSTabManagementControl.registerClass('com.ivp.refmaster.scripts.common.RMSTabManagementControl');
com.ivp.refmaster.scripts.common.RMSTransportControlIds.registerClass('com.ivp.refmaster.scripts.common.RMSTransportControlIds');
com.ivp.refmaster.scripts.common.RMSTransportControls.registerClass('com.ivp.refmaster.scripts.common.RMSTransportControls');
com.ivp.refmaster.scripts.common.RMSTaskControlIds.registerClass('com.ivp.refmaster.scripts.common.RMSTaskControlIds');
com.ivp.refmaster.scripts.common.RMSTaskControls.registerClass('com.ivp.refmaster.scripts.common.RMSTaskControls');
com.ivp.refmaster.scripts.common.RMSCustomControlIds.registerClass('com.ivp.refmaster.scripts.common.RMSCustomControlIds');
com.ivp.refmaster.scripts.common.RMSCustomControls.registerClass('com.ivp.refmaster.scripts.common.RMSCustomControls');
com.ivp.refmaster.scripts.common.RMSTransportTaskInfo.registerClass('com.ivp.refmaster.scripts.common.RMSTransportTaskInfo');
com.ivp.refmaster.scripts.common.RMSSearchMasterDetailGrid.registerClass('com.ivp.refmaster.scripts.common.RMSSearchMasterDetailGrid');
com.ivp.refmaster.refmasterwebservices.RMSWebServiceException.registerClass('com.ivp.refmaster.refmasterwebservices.RMSWebServiceException');
com.ivp.refmaster.scripts.common.RMSExceptionTypes.ALL = 0;
com.ivp.refmaster.scripts.common.RMSExceptionTypes.comparE_AND_SHOW = 1;
com.ivp.refmaster.scripts.common.RMSExceptionTypes.firsT_VENDOR_VALUE_MISSING = 6;
com.ivp.refmaster.scripts.common.RMSExceptionTypes.nO_VENDOR_DATA_FOUND = 3;
com.ivp.refmaster.scripts.common.RMSExceptionTypes.referencE_DATA_EXCEPTION = 2;
com.ivp.refmaster.scripts.common.RMSExceptionTypes.shoW_AS_EXCEPTION = 5;
com.ivp.refmaster.scripts.common.RMSExceptionTypes.valuE_CHANGED = 4;
com.ivp.refmaster.scripts.common.RMSSuppressDeleteConstants.SUPPRESS = 1;
com.ivp.refmaster.scripts.common.RMSSuppressDeleteConstants.DELETE = 0;
com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControls._buttoN_ADD_MODEL_POPUP = 'btnAddModelPopup';
com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControls._i_ADD_MODEL_POPUP = 'iAddModelPopup';
com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControls._entitY_TYPE_DISPLAY_NAME = 'txtEntityTypeName';
com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControls._checkboX_IS_ONE_TO_ONE = 'chkBoxIsOneToOne';
com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControls._tR_CHECK_BOX = 'trCheckBox';
com.ivp.refmaster.scripts.common.RMSDetailsEntityTypeControls._modeL_POPUP_CONTENT_TABLE = 'tableModelPopupContentElements';

// ---- Do not remove this footer ----
// This script was generated using Script# v0.5.5.0 (http://projects.nikhilk.net/ScriptSharp)
// -----------------------------------
