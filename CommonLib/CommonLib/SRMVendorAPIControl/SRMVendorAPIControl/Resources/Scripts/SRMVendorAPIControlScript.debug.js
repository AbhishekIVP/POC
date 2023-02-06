// vendorScript.js
//


Type.registerNamespace('com.ivp.srm.controls.vendor.script');

////////////////////////////////////////////////////////////////////////////////
// com.ivp.srm.controls.vendor.script.IClientMethods

com.ivp.srm.controls.vendor.script.IClientMethods = function() { 
    /// <summary>
    /// an interface for client methods
    /// </summary>
};
com.ivp.srm.controls.vendor.script.IClientMethods.prototype = {
    clearControl : null,
    displayControl : null,
    getOutputXML : null,
    requestClicked : null,
    responseClicked : null,
    hideShowTransport : null
}
com.ivp.srm.controls.vendor.script.IClientMethods.registerInterface('com.ivp.srm.controls.vendor.script.IClientMethods');


////////////////////////////////////////////////////////////////////////////////
// com.ivp.srm.controls.vendor.script.IServerMethods

com.ivp.srm.controls.vendor.script.IServerMethods = function() { 
    /// <summary>
    /// interface for the server side methods
    /// </summary>
};
com.ivp.srm.controls.vendor.script.IServerMethods.prototype = {
    rBbgRequestType : null,
    rBbgInstrumentIdType : null,
    rBbgMarketSector : null,
    rReuterRequestType : null,
    rwsoInstrumentIdType : null,
    rReuterInstrumentIdType : null,
    rReuterAssetTypes : null,
    getAllTransports : null,
    getRequestType : null,
    bindApplicationSpecificData : null
}
com.ivp.srm.controls.vendor.script.IServerMethods.registerInterface('com.ivp.srm.controls.vendor.script.IServerMethods');


////////////////////////////////////////////////////////////////////////////////
// com.ivp.srm.controls.vendor.script.FTPType

com.ivp.srm.controls.vendor.script.FTPType = function() { 
    /// <field name="none" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="request" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="response" type="Number" integer="true" static="true">
    /// </field>
};
com.ivp.srm.controls.vendor.script.FTPType.prototype = {
    none: 0, 
    request: 1, 
    response: 2
}
com.ivp.srm.controls.vendor.script.FTPType.registerEnum('com.ivp.srm.controls.vendor.script.FTPType', false);


////////////////////////////////////////////////////////////////////////////////
// com.ivp.srm.controls.vendor.script.DataType

com.ivp.srm.controls.vendor.script.DataType = function() { 
    /// <field name="dataSet" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="arrayList" type="Number" integer="true" static="true">
    /// </field>
};
com.ivp.srm.controls.vendor.script.DataType.prototype = {
    dataSet: 0, 
    arrayList: 1
}
com.ivp.srm.controls.vendor.script.DataType.registerEnum('com.ivp.srm.controls.vendor.script.DataType', false);


////////////////////////////////////////////////////////////////////////////////
// com.ivp.srm.controls.vendor.script.ClientMethodNames

com.ivp.srm.controls.vendor.script.ClientMethodNames = function com_ivp_srm_controls_vendor_script_ClientMethodNames() {
    /// <summary>
    /// info class for all the client methods
    /// </summary>
    /// <field name="_clearHandler" type="String" static="true">
    /// </field>
    /// <field name="_displayControl" type="String" static="true">
    /// </field>
    /// <field name="_getOutputXML" type="String" static="true">
    /// </field>
    /// <field name="_requestClicked" type="String" static="true">
    /// </field>
    /// <field name="_responseClicked" type="String" static="true">
    /// </field>
    /// <field name="_hideShowTransport" type="String" static="true">
    /// </field>
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.srm.controls.vendor.script.ServerMethodNames

com.ivp.srm.controls.vendor.script.ServerMethodNames = function com_ivp_srm_controls_vendor_script_ServerMethodNames() {
    /// <summary>
    /// info class for all the server side method names
    /// </summary>
    /// <field name="_rBbgRequestType" type="String" static="true">
    /// </field>
    /// <field name="_rBbgInstrumentIdType" type="String" static="true">
    /// </field>
    /// <field name="_rBbgMarketSector" type="String" static="true">
    /// </field>
    /// <field name="_bindShuttleForIdentifiers" type="String" static="true">
    /// </field>
    /// <field name="_rReuterRequestType" type="String" static="true">
    /// </field>
    /// <field name="_rReuterInstrumentIdType" type="String" static="true">
    /// </field>
    /// <field name="_rReuterAssetTypes" type="String" static="true">
    /// </field>
    /// <field name="_rwsoInstrumentIdType" type="String" static="true">
    /// </field>
    /// <field name="_getAllTransports" type="String" static="true">
    /// </field>
    /// <field name="_getRequestType" type="String" static="true">
    /// </field>
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.srm.controls.vendor.script.ManageHandlers

com.ivp.srm.controls.vendor.script.ManageHandlers = function com_ivp_srm_controls_vendor_script_ManageHandlers() {
    /// <field name="_elementList" type="Array" static="true">
    /// </field>
}
com.ivp.srm.controls.vendor.script.ManageHandlers.get__elementList = function com_ivp_srm_controls_vendor_script_ManageHandlers$get__elementList() {
    /// <value type="Array"></value>
    if (com.ivp.srm.controls.vendor.script.ManageHandlers._elementList == null) {
        com.ivp.srm.controls.vendor.script.ManageHandlers._elementList = [];
    }
    return com.ivp.srm.controls.vendor.script.ManageHandlers._elementList;
}
com.ivp.srm.controls.vendor.script.ManageHandlers.addHandler = function com_ivp_srm_controls_vendor_script_ManageHandlers$addHandler(methodName, eventType, clientID, instance) {
    /// <summary>
    /// Adds the handler.
    /// </summary>
    /// <param name="methodName" type="String">
    /// Name of the method.
    /// </param>
    /// <param name="eventType" type="String">
    /// Type of the event.
    /// </param>
    /// <param name="clientID" type="String">
    /// The client ID.
    /// </param>
    /// <param name="instance" type="com.ivp.srm.controls.vendor.script.VendorScript">
    /// The instance.
    /// </param>
    switch (methodName) {
        case com.ivp.srm.controls.vendor.script.ClientMethodNames._clearHandler:
            $addHandler(com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(clientID), eventType, Function.createDelegate(instance, instance.clearControl));
            break;
        case com.ivp.srm.controls.vendor.script.ClientMethodNames._displayControl:
            $addHandler(com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(clientID), eventType, Function.createDelegate(instance, instance.displayControl));
            break;
        case com.ivp.srm.controls.vendor.script.ClientMethodNames._getOutputXML:
            $addHandler(com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(clientID), eventType, Function.createDelegate(instance, instance.getOutputXML));
            break;
        case com.ivp.srm.controls.vendor.script.ClientMethodNames._requestClicked:
            $addHandler(com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(clientID), eventType, Function.createDelegate(instance, instance.requestClicked));
            break;
        case com.ivp.srm.controls.vendor.script.ClientMethodNames._responseClicked:
            $addHandler(com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(clientID), eventType, Function.createDelegate(instance, instance.responseClicked));
            break;
        case com.ivp.srm.controls.vendor.script.ClientMethodNames._hideShowTransport:
            $addHandler(com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(clientID), eventType, Function.createDelegate(instance, instance.hideShowTransport));
            break;
    }
    if (clientID != null && clientID !== '') {
        Array.add(com.ivp.srm.controls.vendor.script.ManageHandlers.get__elementList(), clientID);
    }
}
com.ivp.srm.controls.vendor.script.ManageHandlers.removeHandler = function com_ivp_srm_controls_vendor_script_ManageHandlers$removeHandler(clientID) {
    /// <summary>
    /// Removes the handler.
    /// </summary>
    /// <param name="clientID" type="String">
    /// The client ID.
    /// </param>
    $clearHandlers(com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(clientID));
}
com.ivp.srm.controls.vendor.script.ManageHandlers.removeHandlers = function com_ivp_srm_controls_vendor_script_ManageHandlers$removeHandlers() {
    var element = null;
    for (var i = 0; i < com.ivp.srm.controls.vendor.script.ManageHandlers.get__elementList().length; i++) {
        element = com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(com.ivp.srm.controls.vendor.script.ManageHandlers.get__elementList()[i].toString());
        if (element != null) {
            $clearHandlers(element);
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.srm.controls.vendor.script.VendorCommons

com.ivp.srm.controls.vendor.script.VendorCommons = function com_ivp_srm_controls_vendor_script_VendorCommons() {
}
com.ivp.srm.controls.vendor.script.VendorCommons.isNullEmpty = function com_ivp_srm_controls_vendor_script_VendorCommons$isNullEmpty(text) {
    /// <summary>
    /// Determines whether it is null or empty [the specified text].
    /// </summary>
    /// <param name="text" type="String">
    /// The text.
    /// </param>
    /// <returns type="Boolean"></returns>
    if (text != null && text.trim() !== '') {
        return false;
    }
    return true;
}
com.ivp.srm.controls.vendor.script.VendorCommons.createEventMethod = function com_ivp_srm_controls_vendor_script_VendorCommons$createEventMethod(pairs) {
    /// <summary>
    /// Creates an arraylist of method-event pairs
    /// </summary>
    /// <param name="pairs" type="String">
    /// a string of pairs
    /// </param>
    /// <returns type="Array"></returns>
    var pipeSeperated = [];
    var methodEventPair = [];
    Array.addRange(pipeSeperated, pairs.split('|'));
    var methodEventPairInfo;
    for (var i = 0; i < pipeSeperated.length; i++) {
        methodEventPairInfo = new com.ivp.srm.controls.vendor.script.MethodEventPair();
        methodEventPairInfo.methodName = pipeSeperated[i].toString().split('~')[0];
        methodEventPairInfo.eventName = pipeSeperated[i].toString().split('~')[1];
        Array.add(methodEventPair, methodEventPairInfo);
    }
    return methodEventPair;
}
com.ivp.srm.controls.vendor.script.VendorCommons.bindDropDown = function com_ivp_srm_controls_vendor_script_VendorCommons$bindDropDown(selectElement, text, value) {
    /// <summary>
    /// Binds the drop down.
    /// </summary>
    /// <param name="selectElement" type="Object" domElement="true">
    /// The select element.
    /// </param>
    /// <param name="text" type="Array">
    /// The text.
    /// </param>
    /// <param name="value" type="Array">
    /// The value.
    /// </param>
    var s = selectElement;
    com.ivp.srm.controls.vendor.script.VendorCommons.clearDropDown(s);
    var _optionElement;
    _optionElement = document.createElement('OPTION');
    _optionElement.value = '-1';
    _optionElement.text = '--Select One--';
    s.add(_optionElement);
    for (var i = 0; i < text.length; i++) {
        _optionElement = document.createElement('OPTION');
        if (value == null) {
            _optionElement.value = text[i].toString();
            _optionElement.text = text[i].toString();
        }
        else {
            _optionElement.value = (value[i]).text;
            _optionElement.text = (text[i]).text;
        }
        s.add(_optionElement);
    }
}
com.ivp.srm.controls.vendor.script.VendorCommons.clearDropDown = function com_ivp_srm_controls_vendor_script_VendorCommons$clearDropDown(s) {
    /// <summary>
    /// Clears the drop down.
    /// </summary>
    /// <param name="s" type="Object" domElement="true">
    /// The s.
    /// </param>
    var length = s.options.length;
    for (var i = 0; i < length; i++) {
        s.remove(0);
    }
}
com.ivp.srm.controls.vendor.script.VendorCommons.createErrorMessageHolder = function com_ivp_srm_controls_vendor_script_VendorCommons$createErrorMessageHolder(container) {
    /// <summary>
    /// Creates the error message holder.
    /// </summary>
    /// <param name="container" type="Object" domElement="true">
    /// The container.
    /// </param>
    /// <returns type="Object" domElement="true"></returns>
    if (container != null) {
        if (container.hasChildNodes()) {
            container.removeChild(container.childNodes[0]);
        }
        var ulElement = document.createElement('ul');
        container.appendChild(ulElement);
        container.style.display = 'none';
        return ulElement;
    }
    return null;
}
com.ivp.srm.controls.vendor.script.VendorCommons.createErrorMessage = function com_ivp_srm_controls_vendor_script_VendorCommons$createErrorMessage(errorHolder, errorMessage) {
    /// <summary>
    /// Creates the error message.
    /// </summary>
    /// <param name="errorHolder" type="Object" domElement="true">
    /// The error holder.
    /// </param>
    /// <param name="errorMessage" type="String">
    /// The error message.
    /// </param>
    if (errorHolder != null && !com.ivp.srm.controls.vendor.script.VendorCommons.isNullEmpty(errorMessage)) {
        var liElemenent = document.createElement('li');
        liElemenent.innerHTML = errorMessage;
        errorHolder.appendChild(liElemenent);
        if (errorHolder.parentNode != null) {
            errorHolder.parentNode.style.display = '';
        }
    }
}
com.ivp.srm.controls.vendor.script.VendorCommons.clearErrorHolder = function com_ivp_srm_controls_vendor_script_VendorCommons$clearErrorHolder(container) {
    /// <summary>
    /// Clears the error holder.
    /// </summary>
    /// <param name="container" type="Object" domElement="true">
    /// The container.
    /// </param>
    if (container != null && container.hasChildNodes()) {
        container.removeChild(container.childNodes[0]);
        container.style.display = 'none';
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.srm.controls.vendor.script.VendorAttributeConstants

com.ivp.srm.controls.vendor.script.VendorAttributeConstants = function com_ivp_srm_controls_vendor_script_VendorAttributeConstants() {
    /// <summary>
    /// info class for attributes in html controls
    /// </summary>
    /// <field name="_clientMethodName" type="String" static="true">
    /// </field>
    /// <field name="_serverMethodName" type="String" static="true">
    /// </field>
    /// <field name="_clientEventType" type="String" static="true">
    /// </field>
    /// <field name="_serverEventType" type="String" static="true">
    /// </field>
    /// <field name="_id" type="String" static="true">
    /// </field>
    /// <field name="_displayText" type="String" static="true">
    /// </field>
    /// <field name="_key" type="String" static="true">
    /// </field>
    /// <field name="_visible" type="String" static="true">
    /// </field>
    /// <field name="_isMandatory" type="String" static="true">
    /// </field>
    /// <field name="_dataTextField" type="String" static="true">
    /// </field>
    /// <field name="_dataValueField" type="String" static="true">
    /// </field>
    /// <field name="_savedValue" type="String" static="true">
    /// </field>
    /// <field name="_contextType" type="String" static="true">
    /// </field>
    /// <field name="_assmebly" type="String" static="true">
    /// </field>
    /// <field name="_classname" type="String" static="true">
    /// </field>
    /// <field name="_namespace" type="String" static="true">
    /// </field>
    /// <field name="_vendorPropertyName" type="String" static="true">
    /// </field>
    /// <field name="_customClientMethod" type="String" static="true">
    /// </field>
    /// <field name="_serverMethodArguments" type="String" static="true">
    /// </field>
    /// <field name="_isThirdPartyControl" type="String" static="true">
    /// </field>
    /// <field name="_autoCompleteControlID" type="String" static="true">
    /// </field>
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.srm.controls.vendor.script.MethodEventPair

com.ivp.srm.controls.vendor.script.MethodEventPair = function com_ivp_srm_controls_vendor_script_MethodEventPair() {
    /// <summary>
    /// pairs of method name and event name
    /// </summary>
    /// <field name="methodName" type="String">
    /// </field>
    /// <field name="eventName" type="String">
    /// </field>
}
com.ivp.srm.controls.vendor.script.MethodEventPair.prototype = {
    methodName: null,
    eventName: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.srm.controls.vendor.script.VendorControls

com.ivp.srm.controls.vendor.script.VendorControls = function com_ivp_srm_controls_vendor_script_VendorControls() {
    /// <summary>
    /// _elementDictionary contains the controls
    /// </summary>
}
com.ivp.srm.controls.vendor.script.VendorControls.getElementByID = function com_ivp_srm_controls_vendor_script_VendorControls$getElementByID(clientID) {
    /// <param name="clientID" type="String">
    /// </param>
    /// <returns type="Object" domElement="true"></returns>
    return document.getElementById(clientID);
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.srm.controls.vendor.script.TypeOfVendorControl

com.ivp.srm.controls.vendor.script.TypeOfVendorControl = function com_ivp_srm_controls_vendor_script_TypeOfVendorControl() {
    /// <summary>
    /// info class for all the types of html
    /// </summary>
    /// <field name="_API" type="String" static="true">
    /// </field>
    /// <field name="_apI_Bloomberg" type="String" static="true">
    /// </field>
    /// <field name="_apI_Reuters" type="String" static="true">
    /// </field>
    /// <field name="_apI_MarkitWSO" type="String" static="true">
    /// </field>
    /// <field name="_FTP" type="String" static="true">
    /// </field>
    /// <field name="_ftP_Bloomberg" type="String" static="true">
    /// </field>
    /// <field name="_ftP_Reuters" type="String" static="true">
    /// </field>
    /// <field name="_realtimepreferencE_Bloomberg" type="String" static="true">
    /// </field>
    /// <field name="_realtimepreferencE_Reuters" type="String" static="true">
    /// </field>
    /// <field name="_REALTIMEPREFERENCE" type="String" static="true">
    /// </field>
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.srm.controls.vendor.script.ApplicationSpecificData

com.ivp.srm.controls.vendor.script.ApplicationSpecificData = function com_ivp_srm_controls_vendor_script_ApplicationSpecificData() {
    /// <summary>
    /// ReturnData: the data returned by third party methods
    /// DataType: the type of data returned
    /// ControlId: the id of the control to which data would be bound
    /// </summary>
    /// <field name="ReturnData" type="Object">
    /// </field>
    /// <field name="DataType" type="com.ivp.srm.controls.vendor.script.DataType">
    /// </field>
    /// <field name="ControlId" type="String">
    /// </field>
}
com.ivp.srm.controls.vendor.script.ApplicationSpecificData.prototype = {
    ReturnData: null,
    DataType: 0,
    ControlId: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.srm.controls.vendor.script.VendorParser

com.ivp.srm.controls.vendor.script.VendorParser = function com_ivp_srm_controls_vendor_script_VendorParser() {
}
com.ivp.srm.controls.vendor.script.VendorParser.genericParser = function com_ivp_srm_controls_vendor_script_VendorParser$genericParser(xml) {
    /// <summary>
    /// parsing through XMLDOM
    /// </summary>
    /// <param name="xml" type="String">
    /// The XML.
    /// </param>
    /// <returns type="Object" domElement="true"></returns>
    var xmlDoc = new ActiveXObject('Microsoft.XMLDOM');
    xmlDoc.loadXML(xml);
    xmlDoc.setProperty('SelectionLanguage', 'XPath');
    return xmlDoc;
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.srm.controls.vendor.script.VendorScript

com.ivp.srm.controls.vendor.script.VendorScript = function com_ivp_srm_controls_vendor_script_VendorScript(element) {
    /// <param name="element" type="Object" domElement="true">
    /// The <see cref="T:System.DHTML.DOMElement" /> object to associate with the behavior.
    /// </param>
    /// <field name="_vendorHTML$2" type="String">
    /// </field>
    /// <field name="_buttonSubmitID$2" type="String">
    /// </field>
    /// <field name="_buttonCancelID$2" type="String">
    /// </field>
    /// <field name="_listNodes$2" type="Array">
    /// </field>
    /// <field name="_selectedVendorTypeValue$2" type="String">
    /// </field>
    /// <field name="_typeOfControl$2" type="String">
    /// </field>
    /// <field name="_hasSavedValue$2" type="Boolean">
    /// </field>
    /// <field name="_savedValues$2" type="Object">
    /// </field>
    /// <field name="_errorDiv$2" type="Object" domElement="true">
    /// </field>
    /// <field name="_requestORresponse$2" type="com.ivp.srm.controls.vendor.script.FTPType">
    /// </field>
    /// <field name="_taskName$2" type="String">
    /// </field>
    /// <field name="_taskDescription$2" type="String">
    /// </field>
    /// <field name="_replaceSubmitBtn$2" type="Boolean">
    /// </field>
    /// <field name="_readOnlyDatasourceCtrl$2" type="Boolean">
    /// </field>
    /// <field name="_setDefaultVendorType$2" type="String">
    /// </field>
    /// <field name="_readOnlyVendorType$2" type="Boolean">
    /// </field>
    /// <field name="_disableKey$2" type="Array">
    /// </field>
    /// <field name="_assemblyName$2" type="String">
    /// Gets or sets the has custom identifier.(for third party server side methods)
    /// </field>
    /// <field name="_moduleId$2" type="String">
    /// </field>
    /// <field name="_htmlLoc$2" type="String">
    /// </field>
    /// <field name="_readdObject$2" type="com.ivp.srm.vendorapi.VendorInterfaceAPI">
    /// creates a reference to the proxy class
    /// </field>
    com.ivp.srm.controls.vendor.script.VendorScript.initializeBase(this, [ element ]);
    this._readdObject$2 = new com.ivp.srm.vendorapi.VendorInterfaceAPI();
    this._savedValues$2 = {};
    this._taskName$2 = null;
    this._taskDescription$2 = null;
    this.set_id(this.get_element().id);
}
com.ivp.srm.controls.vendor.script.VendorScript.prototype = {
    _vendorHTML$2: null,
    _buttonSubmitID$2: null,
    _buttonCancelID$2: null,
    _listNodes$2: null,
    _selectedVendorTypeValue$2: null,
    _typeOfControl$2: null,
    _hasSavedValue$2: false,
    _savedValues$2: null,
    _errorDiv$2: null,
    _requestORresponse$2: 0,
    _taskName$2: null,
    _taskDescription$2: null,
    _replaceSubmitBtn$2: false,
    _readOnlyDatasourceCtrl$2: false,
    _setDefaultVendorType$2: null,
    _readOnlyVendorType$2: false,
    _disableKey$2: null,
    
    get_vendorHTML: function com_ivp_srm_controls_vendor_script_VendorScript$get_vendorHTML() {
        /// <summary>
        /// Gets or sets the vendor HTML.
        /// </summary>
        /// <value type="String"></value>
        return this._vendorHTML$2;
    },
    set_vendorHTML: function com_ivp_srm_controls_vendor_script_VendorScript$set_vendorHTML(value) {
        /// <summary>
        /// Gets or sets the vendor HTML.
        /// </summary>
        /// <value type="String"></value>
        this._vendorHTML$2 = value;
        return value;
    },
    
    get_buttonSubmitID: function com_ivp_srm_controls_vendor_script_VendorScript$get_buttonSubmitID() {
        /// <summary>
        /// Gets or sets the button submit ID.
        /// </summary>
        /// <value type="String"></value>
        return this._buttonSubmitID$2;
    },
    set_buttonSubmitID: function com_ivp_srm_controls_vendor_script_VendorScript$set_buttonSubmitID(value) {
        /// <summary>
        /// Gets or sets the button submit ID.
        /// </summary>
        /// <value type="String"></value>
        this._buttonSubmitID$2 = value;
        return value;
    },
    
    get_buttonCancelID: function com_ivp_srm_controls_vendor_script_VendorScript$get_buttonCancelID() {
        /// <value type="String"></value>
        return this._buttonCancelID$2;
    },
    set_buttonCancelID: function com_ivp_srm_controls_vendor_script_VendorScript$set_buttonCancelID(value) {
        /// <value type="String"></value>
        this._buttonCancelID$2 = value;
        return value;
    },
    
    get_typeOfControl: function com_ivp_srm_controls_vendor_script_VendorScript$get_typeOfControl() {
        /// <summary>
        /// Gets or sets the type of control.
        /// </summary>
        /// <value type="String"></value>
        return this._typeOfControl$2;
    },
    set_typeOfControl: function com_ivp_srm_controls_vendor_script_VendorScript$set_typeOfControl(value) {
        /// <summary>
        /// Gets or sets the type of control.
        /// </summary>
        /// <value type="String"></value>
        this._typeOfControl$2 = value;
        return value;
    },
    
    get_hasSavedValue: function com_ivp_srm_controls_vendor_script_VendorScript$get_hasSavedValue() {
        /// <summary>
        /// Gets or sets a value indicating whether this instance has saved value of controls.
        /// </summary>
        /// <value type="Boolean"></value>
        return this._hasSavedValue$2;
    },
    set_hasSavedValue: function com_ivp_srm_controls_vendor_script_VendorScript$set_hasSavedValue(value) {
        /// <summary>
        /// Gets or sets a value indicating whether this instance has saved value of controls.
        /// </summary>
        /// <value type="Boolean"></value>
        this._hasSavedValue$2 = value;
        return value;
    },
    
    _assemblyName$2: null,
    _moduleId$2: null,
    
    get_assemblyName: function com_ivp_srm_controls_vendor_script_VendorScript$get_assemblyName() {
        /// <value type="String"></value>
        return this._assemblyName$2;
    },
    set_assemblyName: function com_ivp_srm_controls_vendor_script_VendorScript$set_assemblyName(value) {
        /// <value type="String"></value>
        this._assemblyName$2 = value;
        return value;
    },
    
    _htmlLoc$2: null,
    
    get_htmlLoc: function com_ivp_srm_controls_vendor_script_VendorScript$get_htmlLoc() {
        /// <value type="String"></value>
        return this._htmlLoc$2;
    },
    set_htmlLoc: function com_ivp_srm_controls_vendor_script_VendorScript$set_htmlLoc(value) {
        /// <value type="String"></value>
        this._htmlLoc$2 = value;
        return value;
    },
    
    get_replaceSubmitBtn: function com_ivp_srm_controls_vendor_script_VendorScript$get_replaceSubmitBtn() {
        /// <value type="Boolean"></value>
        return this._replaceSubmitBtn$2;
    },
    set_replaceSubmitBtn: function com_ivp_srm_controls_vendor_script_VendorScript$set_replaceSubmitBtn(value) {
        /// <value type="Boolean"></value>
        this._replaceSubmitBtn$2 = value;
        return value;
    },
    
    get_readOnlyDatasourceCtrl: function com_ivp_srm_controls_vendor_script_VendorScript$get_readOnlyDatasourceCtrl() {
        /// <value type="Boolean"></value>
        return this._readOnlyDatasourceCtrl$2;
    },
    set_readOnlyDatasourceCtrl: function com_ivp_srm_controls_vendor_script_VendorScript$set_readOnlyDatasourceCtrl(value) {
        /// <value type="Boolean"></value>
        this._readOnlyDatasourceCtrl$2 = value;
        return value;
    },
    
    get_setDefaultVendorType: function com_ivp_srm_controls_vendor_script_VendorScript$get_setDefaultVendorType() {
        /// <value type="String"></value>
        return this._setDefaultVendorType$2;
    },
    set_setDefaultVendorType: function com_ivp_srm_controls_vendor_script_VendorScript$set_setDefaultVendorType(value) {
        /// <value type="String"></value>
        this._setDefaultVendorType$2 = value;
        return value;
    },
    
    get_readOnlyVendorType: function com_ivp_srm_controls_vendor_script_VendorScript$get_readOnlyVendorType() {
        /// <value type="Boolean"></value>
        return this._readOnlyVendorType$2;
    },
    set_readOnlyVendorType: function com_ivp_srm_controls_vendor_script_VendorScript$set_readOnlyVendorType(value) {
        /// <value type="Boolean"></value>
        this._readOnlyVendorType$2 = value;
        return value;
    },
    
    get_disableKey: function com_ivp_srm_controls_vendor_script_VendorScript$get_disableKey() {
        /// <value type="Array"></value>
        return this._disableKey$2;
    },
    set_disableKey: function com_ivp_srm_controls_vendor_script_VendorScript$set_disableKey(value) {
        /// <value type="Array"></value>
        this._disableKey$2 = value;
        return value;
    },
    
    get_moduleId: function com_ivp_srm_controls_vendor_script_VendorScript$get_moduleId() {
        /// <value type="String"></value>
        return this._moduleId$2;
    },
    set_moduleId: function com_ivp_srm_controls_vendor_script_VendorScript$set_moduleId(value) {
        /// <value type="String"></value>
        this._moduleId$2 = value;
        return value;
    },
    
    _readdObject$2: null,
    
    updated: function com_ivp_srm_controls_vendor_script_VendorScript$updated() {
        /// <summary>
        /// Has no built-in functionality; the Updated method is a placeholder for post-update logic
        /// in derived classes.
        /// </summary>
        com.ivp.srm.controls.vendor.script.VendorScript.callBaseMethod(this, 'updated');
        this._reCreateControl$2();
        if (!this.get_hasSavedValue()) {
            this.displayControl(null);
        }
    },
    
    dispose: function com_ivp_srm_controls_vendor_script_VendorScript$dispose() {
        this._savedValues$2 = null;
        com.ivp.srm.controls.vendor.script.ManageHandlers.removeHandlers();
        com.ivp.srm.controls.vendor.script.VendorScript.callBaseMethod(this, 'dispose');
    },
    
    _reCreateControl$2: function com_ivp_srm_controls_vendor_script_VendorScript$_reCreateControl$2() {
        /// <summary>
        /// Recreates the control.
        /// </summary>
        this._createControl$2();
        this._attachEventsAndProperties$2();
        this._fillVendorType$2();
        if (this.get_hasSavedValue()) {
            this._requestORresponse$2 = com.ivp.srm.controls.vendor.script.FTPType.request;
        }
    },
    
    _createControl$2: function com_ivp_srm_controls_vendor_script_VendorScript$_createControl$2() {
        /// <summary>
        /// Creates div to which vendor html is attached
        /// Creates error div
        /// </summary>
        var nameDiv = document.getElementById(this.get_element().id + '_parentDiv');
        if (nameDiv == null) {
            nameDiv = document.createElement('DIV');
            nameDiv.id = this.get_element().id + '_parentDiv';
            this.get_element().appendChild(nameDiv);
        }
        var submitButton = null;
        if (!com.ivp.srm.controls.vendor.script.VendorCommons.isNullEmpty(this.get_buttonSubmitID())) {
            submitButton = document.getElementById(this.get_buttonSubmitID());
        }
        nameDiv.innerHTML = this._vendorHTML$2;
        this._placeServerSubmitButton$2(submitButton);
        this._errorDiv$2 = document.getElementById(this.get_element().id + '_errorDiv');
        if (this._errorDiv$2 == null) {
            this._errorDiv$2 = document.createElement('DIV');
            this._errorDiv$2.id = this.get_element().id + '_errorDiv';
            this._errorDiv$2.className = 'errorDiv';
            this._errorDiv$2.style.display = 'none';
            this.get_element().appendChild(this._errorDiv$2);
        }
        if (this.get_typeOfControl() === 'FTP') {
            if (this._requestORresponse$2 === com.ivp.srm.controls.vendor.script.FTPType.request) {
                document.getElementById(this.get_element().id + '_requestButton').className = 'tabBtnVisited';
                document.getElementById(this.get_element().id + '_responseButton').className = 'tabBtn';
            }
            else if (this._requestORresponse$2 === com.ivp.srm.controls.vendor.script.FTPType.response) {
                document.getElementById(this.get_element().id + '_requestButton').className = 'tabBtn';
                document.getElementById(this.get_element().id + '_responseButton').className = 'tabBtnVisited';
            }
        }
    },
    
    _placeServerSubmitButton$2: function com_ivp_srm_controls_vendor_script_VendorScript$_placeServerSubmitButton$2(submitbutton) {
        /// <summary>
        /// Places the server submit button.
        /// replacing the dummy sumbit button with the button provided by the user
        /// </summary>
        /// <param name="submitbutton" type="Object" domElement="true">
        /// The submitbutton.
        /// </param>
        if (submitbutton != null) {
            var dummyBtn = document.getElementById(this.get_element().id + '_dummy');
            submitbutton.className = dummyBtn.className;
            if (this.get_replaceSubmitBtn()) {
                dummyBtn.parentNode.replaceChild(submitbutton, dummyBtn);
            }
            else {
                dummyBtn.style.display = 'none';
            }
        }
        if (!com.ivp.srm.controls.vendor.script.VendorCommons.isNullEmpty(this.get_buttonCancelID())) {
            var cancelButton = com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(this.get_buttonCancelID());
            if (cancelButton != null) {
                var dummyCancelBtn = document.getElementById(this.get_element().id + '_Clear');
                cancelButton.className = dummyCancelBtn.className;
                dummyCancelBtn.parentNode.replaceChild(cancelButton, dummyCancelBtn);
            }
        }
    },
    
    _attachEventsAndProperties$2: function com_ivp_srm_controls_vendor_script_VendorScript$_attachEventsAndProperties$2() {
        /// <summary>
        /// Attaches the events to controls
        /// </summary>
        this._listNodes$2 = com.ivp.srm.controls.vendor.script.VendorParser.genericParser(this.get_vendorHTML()).selectNodes('//td[@ischildcontrol=\"true\"]/*');
        var _attributeCollection = null;
        var _serverMethodName, _clientMethodName, _customClientMethod;
        var _serverEventType;
        var _id;
        var _clientMethodEventPair = null;
        var _customMethodEventPair = null;
        for (var i = 0; i < this._listNodes$2.length; i++) {
            _clientMethodEventPair = [];
            _customMethodEventPair = [];
            _attributeCollection = (this._listNodes$2[i]).attributes;
            _serverMethodName = _attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._serverMethodName);
            _clientMethodName = _attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._clientMethodName);
            _serverEventType = _attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._serverEventType);
            _customClientMethod = _attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._customClientMethod);
            _id = _attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._id);
            if (_id.value === this.get_element().id + '_dummy' && !com.ivp.srm.controls.vendor.script.VendorCommons.isNullEmpty(this.get_buttonSubmitID())) {
                _id.value = this.get_buttonSubmitID();
            }
            if (_id.value === this.get_element().id + '_Clear' && !com.ivp.srm.controls.vendor.script.VendorCommons.isNullEmpty(this.get_buttonCancelID())) {
                _id.value = this.get_buttonCancelID();
            }
            if (_clientMethodName != null && _clientMethodName.value !== '') {
                _clientMethodEventPair = com.ivp.srm.controls.vendor.script.VendorCommons.createEventMethod(_clientMethodName.value);
            }
            if (_customClientMethod != null && _customClientMethod.value !== '') {
                _customMethodEventPair = com.ivp.srm.controls.vendor.script.VendorCommons.createEventMethod(_customClientMethod.value);
            }
            if (_serverMethodName != null && !com.ivp.srm.controls.vendor.script.VendorCommons.isNullEmpty(_serverMethodName.value) && _serverEventType != null) {
                com.ivp.srm.controls.vendor.script.ManageHandlers.addHandler(_serverMethodName.value, _serverEventType.value, _id.value, this);
            }
            if (_clientMethodEventPair.length !== 0) {
                for (var j = 0; j < _clientMethodEventPair.length; j++) {
                    com.ivp.srm.controls.vendor.script.ManageHandlers.addHandler((_clientMethodEventPair[j]).methodName, (_clientMethodEventPair[j]).eventName, _id.value, this);
                }
            }
            if (_customMethodEventPair.length !== 0) {
                for (var j = 0; j < _customMethodEventPair.length; j++) {
                    var customclientmethodHandler = Function.createDelegate(this, this.customClientMethod);
                    $addHandler(document.getElementById(_id.value), (_customMethodEventPair[j]).eventName, customclientmethodHandler);
                }
            }
        }
        this._manageStyles$2();
        if (this.get_typeOfControl() === 'FTP' && this._requestORresponse$2 !== com.ivp.srm.controls.vendor.script.FTPType.none) {
            if (this._taskName$2 != null) {
                (this._getControlByNameAndRequestType$2('Task Name', 'Request')).value = this._taskName$2;
                (this._getControlByNameAndRequestType$2('Task Name', 'Response')).value = this._taskName$2;
            }
            if (this._taskDescription$2 != null) {
                (this._getControlByNameAndRequestType$2('Task Description', 'Request')).value = this._taskDescription$2;
                (this._getControlByNameAndRequestType$2('Task Description', 'Response')).value = this._taskDescription$2;
            }
        }
        else {
            if (this._taskName$2 != null) {
                (this._getControlByDisplayName$2('Task Name')).value = this._taskName$2;
            }
            if (this._taskDescription$2 != null) {
                (this._getControlByDisplayName$2('Task Description')).value = this._taskDescription$2;
            }
        }
    },
    
    _fillVendorType$2: function com_ivp_srm_controls_vendor_script_VendorScript$_fillVendorType$2() {
        /// <summary>
        /// Fills the type of the vendor.
        /// </summary>
        this._readdObject$2.GetVendorTypes(this.get_moduleId(), this.get_typeOfControl(), Function.createDelegate(this, this._onGetVendorTypeSuccess$2), Function.createDelegate(this, this._onFailure$2));
    },
    
    _manageStyles$2: function com_ivp_srm_controls_vendor_script_VendorScript$_manageStyles$2() {
        /// <summary>
        /// Hides and shows the controls according to visible attribute
        /// Hides and shows the div according to request or response in case of FTP
        /// Invoke the server side methods
        /// Save the savedValues of control in dictionary
        /// </summary>
        var attributeCollection = null;
        var _visible;
        var _id;
        var _name;
        var _savedValue;
        var contextType;
        var _serverMethodName;
        var _assembly;
        var _classname;
        var _namespace;
        var _serverMethodArgument;
        var _keyAttribute;
        var _isthirdPartyControl;
        var _autoCompleteControlID;
        var autoCompleteComponentId = '';
        var ele;
        var DisableKeyLINQObj = new LINQ(this.get_disableKey());
        var PipeSeperatedList = null;
        var SavedOptions = null;
        var key;
        for (var i = 0; i < this._listNodes$2.length; i++) {
            attributeCollection = (this._listNodes$2[i]).attributes;
            _visible = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._visible);
            _id = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._id);
            _name = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._displayText);
            _savedValue = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._savedValue);
            contextType = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._contextType);
            _serverMethodName = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._serverMethodName);
            _assembly = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._assmebly);
            _classname = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._classname);
            _namespace = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._namespace);
            _serverMethodArgument = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._serverMethodArguments);
            _keyAttribute = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._key);
            _isthirdPartyControl = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._isThirdPartyControl);
            _autoCompleteControlID = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._autoCompleteControlID);
            if (_isthirdPartyControl != null && _autoCompleteControlID != null) {
                if (Boolean.parse(_isthirdPartyControl.value)) {
                    document.getElementById(_autoCompleteControlID.value).style.display = '';
                    document.getElementById(_id.value).appendChild(document.getElementById(_autoCompleteControlID.value));
                    if (_savedValue != null && _savedValue.value.trim() !== '') {
                        autoCompleteComponentId = document.getElementById(_autoCompleteControlID.value).children[0].id.trim().substr(4);
                        var autocomplete = $find(autoCompleteComponentId);
                        PipeSeperatedList = [];
                        SavedOptions = [];
                        Array.addRange(PipeSeperatedList, _savedValue.value.split('|'));
                        Array.forEach(PipeSeperatedList, Function.createDelegate(this, function(o) {
                            var keyValue = o.toString().split('~');
                            var info = new com.ivp.rad.controls.rautocompletecontrolscripts.OptionInfo(keyValue[0], keyValue[1]);
                            Array.add(SavedOptions, info);
                        }));
                        autocomplete.setSelectedTokens(SavedOptions);
                    }
                }
            }
            else {
                if (_serverMethodName != null && _serverMethodName.value.trim() !== '' && _visible.value === 'true') {
                    this._invokeServerMethods$2(_serverMethodName.value.trim(), (_assembly == null) ? null : _assembly.value, (_classname == null) ? null : _classname.value, _id.value, (_namespace == null) ? null : _namespace.value, (_serverMethodArgument == null) ? null : _serverMethodArgument.value);
                }
                if (_id.value === this.get_element().id + '_dummy' && !com.ivp.srm.controls.vendor.script.VendorCommons.isNullEmpty(this.get_buttonSubmitID())) {
                    _id.value = this.get_buttonSubmitID();
                }
                if (_id.value === this.get_element().id + '_Clear') {
                    if (!com.ivp.srm.controls.vendor.script.VendorCommons.isNullEmpty(this.get_buttonCancelID())) {
                        _id.value = this.get_buttonCancelID();
                    }
                    if (this.get_disableKey().length > 0) {
                        document.getElementById(_id.value).style.display = 'none';
                        _visible.value = 'false';
                    }
                }
                if (_name != null && _savedValue != null && _savedValue.value !== '' && _name.value === 'Vendor Type') {
                    this._selectedVendorTypeValue$2 = _savedValue.value;
                }
                if (_savedValue != null && _savedValue.value.trim() !== '') {
                    if (_savedValue.value.trim() === '') {
                        _savedValue.value = '-1';
                    }
                    key = _name.value;
                    if (this.get_typeOfControl() === 'FTP') {
                        if (contextType != null) {
                            if (contextType.value === 'Request') {
                                key += '_Request';
                            }
                            else if (contextType.value === 'Response') {
                                key += '_Response';
                            }
                        }
                        this._requestORresponse$2 = com.ivp.srm.controls.vendor.script.FTPType.request;
                    }
                    this._savedValues$2[key] = _savedValue.value;
                }
                ele = com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(_id.value);
                var sendTrueFalse = (Boolean.parse(_visible.value)) ? false : true;
                this._hideShowElementsAndItsTag$2(ele, sendTrueFalse);
                if (_keyAttribute != null) {
                    if (DisableKeyLINQObj.Where(Function.createDelegate(this, function(o, index) {
                        return o.toString() === _keyAttribute.value;
                    })).Count(null) > 0) {
                        ele.parentNode.disabled = true;
                    }
                }
            }
        }
        this._setSavedValuesToControl$2();
        if (this.get_typeOfControl() === 'FTP') {
            switch (this._requestORresponse$2) {
                case com.ivp.srm.controls.vendor.script.FTPType.request:
                    com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(this.get_element().id + '_ResponseDiv').style.display = 'none';
                    com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(this.get_element().id + '_RequestDiv').style.display = '';
                    break;
                case com.ivp.srm.controls.vendor.script.FTPType.response:
                    com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(this.get_element().id + '_ResponseDiv').style.display = '';
                    com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(this.get_element().id + '_RequestDiv').style.display = 'none';
                    break;
            }
            this._selectedFTPTransport$2();
            if (this._selectedVendorTypeValue$2 === 'Reuters') {
                this._selectedOutgoingIncomingFTP$2();
            }
        }
    },
    
    _invokeServerMethods$2: function com_ivp_srm_controls_vendor_script_VendorScript$_invokeServerMethods$2(methodName, assemblyName, className, id, nameSpace, serverMethodArguments) {
        /// <summary>
        /// Invokes the server methods.
        /// </summary>
        /// <param name="methodName" type="String">
        /// Name of the method.
        /// </param>
        /// <param name="assemblyName" type="String">
        /// Name of the assembly.
        /// </param>
        /// <param name="className" type="String">
        /// Name of the class.
        /// </param>
        /// <param name="id" type="String">
        /// The id.
        /// </param>
        /// <param name="nameSpace" type="String">
        /// The name space.
        /// </param>
        /// <param name="serverMethodArguments" type="String">
        /// The server method arguments.
        /// </param>
        if (assemblyName != null && assemblyName.trim() !== '') {
            this.bindApplicationSpecificData(className, methodName, assemblyName, id, nameSpace);
            return;
        }
        switch (methodName) {
            case com.ivp.srm.controls.vendor.script.ServerMethodNames._rBbgInstrumentIdType:
                this.rBbgInstrumentIdType();
                break;
            case com.ivp.srm.controls.vendor.script.ServerMethodNames._rBbgMarketSector:
                this.rBbgMarketSector();
                break;
            case com.ivp.srm.controls.vendor.script.ServerMethodNames._rBbgRequestType:
                this.rBbgRequestType();
                break;
            case com.ivp.srm.controls.vendor.script.ServerMethodNames._rReuterAssetTypes:
                this.rReuterAssetTypes();
                break;
            case com.ivp.srm.controls.vendor.script.ServerMethodNames._rReuterInstrumentIdType:
                this.rReuterInstrumentIdType();
                break;
            case com.ivp.srm.controls.vendor.script.ServerMethodNames._rReuterRequestType:
                this.rReuterRequestType();
                break;
            case com.ivp.srm.controls.vendor.script.ServerMethodNames._rwsoInstrumentIdType:
                this.rwsoInstrumentIdType();
                break;
            case com.ivp.srm.controls.vendor.script.ServerMethodNames._getAllTransports:
                if (this.get_typeOfControl() === 'FTP') {
                    this.incomingOutgoingFTP_GetAllTransports();
                }
                else {
                    this.getAllTransports();
                }
                break;
            case com.ivp.srm.controls.vendor.script.ServerMethodNames._getRequestType:
                var parameters = serverMethodArguments.split(',');
                this.getRequestType(Number.parseInvariant(parameters[0]), Number.parseInvariant(parameters[1]));
                break;
        }
    },
    
    clearControl: function com_ivp_srm_controls_vendor_script_VendorScript$clearControl(e) {
        /// <summary>
        /// Clears the control.
        /// </summary>
        /// <param name="e" type="Sys.UI.DomEvent">
        /// The e.
        /// </param>
        if (!this.get_readOnlyVendorType()) {
            this.placeAutoCompleteControlBack();
        }
        var inputCollection = null;
        inputCollection = this.get_element().getElementsByTagName('INPUT');
        for (var i = 0; i < inputCollection.length; i++) {
            if (inputCollection[i].getAttribute('type').toString() === 'text' || inputCollection[i].getAttribute('type').toString() === 'password') {
                (inputCollection[i]).value = '';
                (inputCollection[i]).disabled = false;
            }
            if (inputCollection[i].getAttribute('type').toString() === 'checkbox') {
                (inputCollection[i]).checked = false;
                (inputCollection[i]).disabled = false;
            }
        }
        inputCollection = this.get_element().getElementsByTagName('SELECT');
        for (var i = 0; i < inputCollection.length; i++) {
            if ((inputCollection[i]).getAttribute('key') != null && (inputCollection[i]).getAttribute('key').toString().toLowerCase() === 'vendor' && this.get_readOnlyVendorType()) {
                continue;
            }
            if ((inputCollection[i]).getAttribute('key') != null && (inputCollection[i]).getAttribute('key').toString().toLowerCase() === 'datasource' && this.get_readOnlyDatasourceCtrl() && this.get_readOnlyVendorType()) {
                continue;
            }
            (inputCollection[i]).selectedIndex = 0;
            (inputCollection[i]).disabled = false;
        }
        inputCollection = this.get_element().getElementsByTagName('LABEL');
        for (var i = 0; i < inputCollection.length; i++) {
            inputCollection[i].innerText = '';
        }
        inputCollection = this.get_element().getElementsByTagName('TEXTAREA');
        for (var i = 0; i < inputCollection.length; i++) {
            if (inputCollection[i].getAttribute('type').toString() === 'textarea') {
                (inputCollection[i]).value = '';
                (inputCollection[i]).disabled = false;
            }
        }
        this.set_hasSavedValue(false);
        this._selectedVendorTypeValue$2 = '-1';
        if (!this.get_readOnlyVendorType()) {
            switch (this.get_typeOfControl()) {
                case 'API':
                    this._readdObject$2.ReturnHTML(this.get_element().id, com.ivp.srm.controls.vendor.script.TypeOfVendorControl._API, this.get_assemblyName(), this.get_htmlLoc(), Function.createDelegate(this, this._onSuccessGetHTML$2), Function.createDelegate(this, this._onFailure$2));
                    break;
                case 'REALTIMEPREFERENCE':
                    this._readdObject$2.ReturnHTML(this.get_element().id, com.ivp.srm.controls.vendor.script.TypeOfVendorControl._REALTIMEPREFERENCE, this.get_assemblyName(), this.get_htmlLoc(), Function.createDelegate(this, this._onSuccessGetHTML$2), Function.createDelegate(this, this._onFailure$2));
                    break;
                case 'FTP':
                    this._requestORresponse$2 = com.ivp.srm.controls.vendor.script.FTPType.none;
                    this._readdObject$2.ReturnHTML(this.get_element().id, com.ivp.srm.controls.vendor.script.TypeOfVendorControl._FTP, this.get_assemblyName(), this.get_htmlLoc(), Function.createDelegate(this, this._onSuccessGetHTML$2), Function.createDelegate(this, this._onFailure$2));
                    break;
            }
        }
        com.ivp.srm.controls.vendor.script.VendorCommons.clearErrorHolder(this._errorDiv$2);
        this._taskName$2 = null;
        this._taskDescription$2 = null;
    },
    
    placeAutoCompleteControlBack: function com_ivp_srm_controls_vendor_script_VendorScript$placeAutoCompleteControlBack() {
        for (var i = 0; i < this._listNodes$2.length; i++) {
            var attributeCollection = (this._listNodes$2[i]).attributes;
            var _id = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._id);
            var _isthirdPartyControl = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._isThirdPartyControl);
            var _autoCompleteControlID = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._autoCompleteControlID);
            var parentDIV = document.getElementById(_id.value);
            if (_isthirdPartyControl != null && _autoCompleteControlID != null) {
                if (Boolean.parse(_isthirdPartyControl.value)) {
                    var autoCompleteComponentId = document.getElementById(_autoCompleteControlID.value).children[0].id.trim().substr(4);
                    var autocomplete = $find(autoCompleteComponentId);
                    autocomplete.setSelectedTokens(null);
                    if (parentDIV != null && parentDIV.children.length > 0) {
                        document.getElementById(_autoCompleteControlID.value).style.display = 'none';
                        document.body.appendChild(document.getElementById(_autoCompleteControlID.value));
                    }
                }
            }
        }
    },
    
    hideShowTransport: function com_ivp_srm_controls_vendor_script_VendorScript$hideShowTransport(e) {
        /// <summary>
        /// Hides and shows transport based on selected value of selectelement.
        /// </summary>
        /// <param name="e" type="Sys.UI.DomEvent">
        /// The e.
        /// </param>
        var selectElemnt = e.target;
        var ele = this._getControlByDisplayName$2('Transport');
        if (selectElemnt.value.toUpperCase() === 'FTP' || selectElemnt.value.toUpperCase().indexOf('FTP') >= 0) {
            this._hideShowElementsAndItsTag$2(ele, false);
        }
        else {
            this._hideShowElementsAndItsTag$2(ele, true);
        }
    },
    
    customClientMethod: function com_ivp_srm_controls_vendor_script_VendorScript$customClientMethod(e) {
        /// <param name="e" type="Sys.UI.DomEvent">
        /// </param>
        var attributeCollection = null;
        var _id;
        var _customClientMethodName;
        for (var i = 0; i < this._listNodes$2.length; i++) {
            attributeCollection = (this._listNodes$2[i]).attributes;
            _id = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._id);
            _customClientMethodName = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._customClientMethod);
            if (e.target.id === _id.value) {
                eval(_customClientMethodName.value.split('~')[0] + ';');
                break;
            }
        }
    },
    
    displayControl: function com_ivp_srm_controls_vendor_script_VendorScript$displayControl(e) {
        /// <summary>
        /// Displays the control according to vendor type
        /// </summary>
        /// <param name="e" type="Sys.UI.DomEvent">
        /// The e.
        /// </param>
        this.placeAutoCompleteControlBack();
        if (e == null) {
            if (this.get_setDefaultVendorType() != null) {
                var firstLetter = this.get_setDefaultVendorType().charAt(0).toString().toUpperCase();
                var RestOfTheword = this.get_setDefaultVendorType().substr(1).toLowerCase();
                this._selectedVendorTypeValue$2 = firstLetter + RestOfTheword;
            }
        }
        else {
            this._selectedVendorTypeValue$2 = (e.target).value;
        }
        if (this._selectedVendorTypeValue$2 != null) {
            this._savedValues$2 = null;
            com.ivp.srm.controls.vendor.script.ManageHandlers.removeHandlers();
            switch (this._selectedVendorTypeValue$2) {
                case 'Bloomberg':
                    if (this.get_typeOfControl() === 'API') {
                        this._readdObject$2.ReturnHTML(this.get_element().id, com.ivp.srm.controls.vendor.script.TypeOfVendorControl._apI_Bloomberg, this.get_assemblyName(), this.get_htmlLoc(), Function.createDelegate(this, this._onSuccessGetHTML$2), Function.createDelegate(this, this._onFailure$2));
                    }
                    else if (this.get_typeOfControl() === 'REALTIMEPREFERENCE') {
                        this._readdObject$2.ReturnHTML(this.get_element().id, com.ivp.srm.controls.vendor.script.TypeOfVendorControl._realtimepreferencE_Bloomberg, this.get_assemblyName(), this.get_htmlLoc(), Function.createDelegate(this, this._onSuccessGetHTML$2), Function.createDelegate(this, this._onFailure$2));
                    }
                    else if (this.get_typeOfControl() === 'FTP') {
                        if (this._requestORresponse$2 === com.ivp.srm.controls.vendor.script.FTPType.none) {
                            this._requestORresponse$2 = com.ivp.srm.controls.vendor.script.FTPType.request;
                        }
                        this._readdObject$2.ReturnHTML(this.get_element().id, com.ivp.srm.controls.vendor.script.TypeOfVendorControl._ftP_Bloomberg, this.get_assemblyName(), this.get_htmlLoc(), Function.createDelegate(this, this._onSuccessGetHTML$2), Function.createDelegate(this, this._onFailure$2));
                    }
                    break;
                case 'Reuters':
                    if (this.get_typeOfControl() === 'API') {
                        this._readdObject$2.ReturnHTML(this.get_element().id, com.ivp.srm.controls.vendor.script.TypeOfVendorControl._apI_Reuters, this.get_assemblyName(), this.get_htmlLoc(), Function.createDelegate(this, this._onSuccessGetHTML$2), Function.createDelegate(this, this._onFailure$2));
                    }
                    else if (this.get_typeOfControl() === 'REALTIMEPREFERENCE') {
                        this._readdObject$2.ReturnHTML(this.get_element().id, com.ivp.srm.controls.vendor.script.TypeOfVendorControl._realtimepreferencE_Reuters, this.get_assemblyName(), this.get_htmlLoc(), Function.createDelegate(this, this._onSuccessGetHTML$2), Function.createDelegate(this, this._onFailure$2));
                    }
                    else if (this.get_typeOfControl() === 'FTP') {
                        if (this._requestORresponse$2 === com.ivp.srm.controls.vendor.script.FTPType.none) {
                            this._requestORresponse$2 = com.ivp.srm.controls.vendor.script.FTPType.request;
                        }
                        this._readdObject$2.ReturnHTML(this.get_element().id, com.ivp.srm.controls.vendor.script.TypeOfVendorControl._ftP_Reuters, this.get_assemblyName(), this.get_htmlLoc(), Function.createDelegate(this, this._onSuccessGetHTML$2), Function.createDelegate(this, this._onFailure$2));
                    }
                    break;
                case 'MarkitWSO':
                    if (this.get_typeOfControl() === 'API') {
                        this._readdObject$2.ReturnHTML(this.get_element().id, com.ivp.srm.controls.vendor.script.TypeOfVendorControl._apI_MarkitWSO, this.get_assemblyName(), this.get_htmlLoc(), Function.createDelegate(this, this._onSuccessGetHTML$2), Function.createDelegate(this, this._onFailure$2));
                    }
                    break;
                default:
                    if (this.get_typeOfControl() === 'API') {
                        this._readdObject$2.ReturnHTML(this.get_element().id, com.ivp.srm.controls.vendor.script.TypeOfVendorControl._API, this.get_assemblyName(), this.get_htmlLoc(), Function.createDelegate(this, this._onSuccessGetHTML$2), Function.createDelegate(this, this._onFailure$2));
                    }
                    else if (this.get_typeOfControl() === 'REALTIMEPREFERENCE') {
                        this._readdObject$2.ReturnHTML(this.get_element().id, com.ivp.srm.controls.vendor.script.TypeOfVendorControl._REALTIMEPREFERENCE, this.get_assemblyName(), this.get_htmlLoc(), Function.createDelegate(this, this._onSuccessGetHTML$2), Function.createDelegate(this, this._onFailure$2));
                    }
                    else if (this.get_typeOfControl() === 'FTP') {
                        this._requestORresponse$2 = com.ivp.srm.controls.vendor.script.FTPType.none;
                        this._readdObject$2.ReturnHTML(this.get_element().id, com.ivp.srm.controls.vendor.script.TypeOfVendorControl._FTP, this.get_assemblyName(), this.get_htmlLoc(), Function.createDelegate(this, this._onSuccessGetHTML$2), Function.createDelegate(this, this._onFailure$2));
                    }
                    break;
            }
        }
        if (e != null) {
            this._taskName$2 = (this._getControlByDisplayName$2('Task Name')).value;
            this._taskDescription$2 = (this._getControlByDisplayName$2('Task Description')).value;
        }
    },
    
    getOutputXML: function com_ivp_srm_controls_vendor_script_VendorScript$getOutputXML(e) {
        /// <summary>
        /// Gets the output XML.
        /// </summary>
        /// <param name="e" type="Sys.UI.DomEvent">
        /// The e.
        /// </param>
        var returnValue = this._checkMandatoryFields$2();
        if (!returnValue) {
            e.preventDefault();
            e.rawEvent.returnValue = false;
        }
        else {
            this._setOutputXmlToHiddenField$2();
        }
    },
    
    _setOutputXmlToHiddenField$2: function com_ivp_srm_controls_vendor_script_VendorScript$_setOutputXmlToHiddenField$2() {
        /// <summary>
        /// Converts the html into required output XML
        /// Sets the output XML to hidden field.
        /// </summary>
        var displayText;
        var id;
        var _attributeCollection;
        var key;
        var element;
        var contextType;
        var visible;
        var vendorpropertyname;
        var _isthirdPartyControl;
        var _autoCompleteControlID;
        var autoCompleteComponentId = '';
        var _isset = false;
        var _outputXml = new Sys.StringBuilder();
        _outputXml.append('&lt;');
        _outputXml.append(this.get_typeOfControl());
        _outputXml.append('&gt;');
        if (this.get_typeOfControl() === 'FTP') {
            _outputXml.append('&lt;REQUEST&gt;');
        }
        for (var i = 0; i < this._listNodes$2.length; i++) {
            _attributeCollection = (this._listNodes$2[i]).attributes;
            id = _attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._id);
            displayText = _attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._displayText);
            key = _attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._key);
            contextType = _attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._contextType);
            visible = _attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._visible);
            vendorpropertyname = _attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._vendorPropertyName);
            _isthirdPartyControl = _attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._isThirdPartyControl);
            _autoCompleteControlID = _attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._autoCompleteControlID);
            if (id.value === this.get_element().id + '_dummy' && !com.ivp.srm.controls.vendor.script.VendorCommons.isNullEmpty(this.get_buttonSubmitID())) {
                id.value = this.get_buttonSubmitID();
            }
            if (id.value === this.get_element().id + '_Clear' && !com.ivp.srm.controls.vendor.script.VendorCommons.isNullEmpty(this.get_buttonCancelID())) {
                id.value = this.get_buttonCancelID();
            }
            element = com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(id.value);
            if (visible.value === 'true' && element.style.display !== 'none') {
                if (this.get_typeOfControl() === 'FTP' && contextType != null && contextType.value === 'Response' && !_isset) {
                    _outputXml.append('&lt;/REQUEST&gt;');
                    _outputXml.append('&lt;RESPONSE&gt;');
                    _isset = true;
                }
                switch (element.tagName.toUpperCase()) {
                    case 'INPUT':
                        var t_ele = element;
                        if (t_ele.type.toUpperCase() === 'TEXT') {
                            _outputXml.append('&lt;control text=\"');
                            _outputXml.append(t_ele.value.trim());
                            _outputXml.append('\" value=\"');
                            _outputXml.append(t_ele.value.trim());
                            _outputXml.append('\" key=\"');
                            _outputXml.append(key.value);
                            _outputXml.append('\" name=\"');
                            _outputXml.append(displayText.value);
                            _outputXml.append('\" vendorpropertyname=\"');
                            _outputXml.append(vendorpropertyname.value);
                            _outputXml.append('\" /&gt;');
                        }
                        break;
                    case 'SELECT':
                        var s_ele = element;
                        _outputXml.append('&lt;control text=\"');
                        _outputXml.append((s_ele.options[s_ele.selectedIndex]).text);
                        _outputXml.append('\" value=\"');
                        _outputXml.append(s_ele.value);
                        _outputXml.append('\" key=\"');
                        _outputXml.append(key.value);
                        _outputXml.append('\" name=\"');
                        _outputXml.append(displayText.value);
                        _outputXml.append('\" vendorpropertyname=\"');
                        _outputXml.append(vendorpropertyname.value);
                        _outputXml.append('\" /&gt;');
                        break;
                    case 'DIV':
                        if (_autoCompleteControlID != null && _isthirdPartyControl != null) {
                            if (Boolean.parse(_isthirdPartyControl.value)) {
                                autoCompleteComponentId = document.getElementById(_autoCompleteControlID.value).children[0].id.trim().substr(4);
                                var autocomplete = $find(autoCompleteComponentId);
                                _outputXml.append('&lt;control text=\"');
                                var list = autocomplete.getSelectedTokens();
                                var options = null;
                                var SB = new Sys.StringBuilder();
                                Array.forEach(list, Function.createDelegate(this, function(o) {
                                    options = o;
                                    SB.append(options.Key);
                                    SB.append('~');
                                    SB.append(options.Value);
                                    SB.append('|');
                                }));
                                if (!SB.isEmpty()) {
                                    _outputXml.append(SB.toString().substring(0, SB.toString().length - 1));
                                }
                                else {
                                    _outputXml.append('');
                                }
                                _outputXml.append('\" value=\"');
                                if (!SB.isEmpty()) {
                                    _outputXml.append(SB.toString().substring(0, SB.toString().length - 1));
                                }
                                else {
                                    _outputXml.append('');
                                }
                                _outputXml.append('\" key=\"');
                                _outputXml.append(key.value);
                                _outputXml.append('\" name=\"');
                                _outputXml.append(displayText.value);
                                _outputXml.append('\" vendorpropertyname=\"');
                                _outputXml.append(vendorpropertyname.value);
                                _outputXml.append('\" /&gt;');
                            }
                        }
                        break;
                }
            }
        }
        if (this.get_typeOfControl() === 'FTP') {
            _outputXml.append('&lt;/RESPONSE&gt;');
        }
        _outputXml.append('&lt;/');
        _outputXml.append(this.get_typeOfControl());
        _outputXml.append('&gt;');
        (com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(this.get_element().id + '_HdnErrorMessage')).value = _outputXml.toString();
    },
    
    requestClicked: function com_ivp_srm_controls_vendor_script_VendorScript$requestClicked(e) {
        /// <summary>
        /// Handles request button click
        /// </summary>
        /// <param name="e" type="Sys.UI.DomEvent">
        /// The e.
        /// </param>
        document.getElementById(this.get_element().id + '_requestButton').className = 'tabBtnVisited';
        document.getElementById(this.get_element().id + '_responseButton').className = 'tabBtn';
        this._requestORresponse$2 = com.ivp.srm.controls.vendor.script.FTPType.request;
        var s = this._getControlByDisplayName$2('Vendor Type');
        if (s != null && s.selectedIndex !== 0) {
            this._fillVendorType$2();
            com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(this.get_element().id + '_ResponseDiv').style.display = 'none';
            com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(this.get_element().id + '_RequestDiv').style.display = '';
        }
    },
    
    responseClicked: function com_ivp_srm_controls_vendor_script_VendorScript$responseClicked(e) {
        /// <summary>
        /// Handles response button click
        /// </summary>
        /// <param name="e" type="Sys.UI.DomEvent">
        /// The e.
        /// </param>
        document.getElementById(this.get_element().id + '_requestButton').className = 'tabBtn';
        document.getElementById(this.get_element().id + '_responseButton').className = 'tabBtnVisited';
        this._requestORresponse$2 = com.ivp.srm.controls.vendor.script.FTPType.response;
        var s = this._getControlByDisplayName$2('Vendor Type');
        if (s != null && s.selectedIndex !== 0) {
            this._fillVendorType$2();
            com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(this.get_element().id + '_ResponseDiv').style.display = '';
            com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(this.get_element().id + '_RequestDiv').style.display = 'none';
        }
    },
    
    incomingOutgoingFTP_GetAllTransports: function com_ivp_srm_controls_vendor_script_VendorScript$incomingOutgoingFTP_GetAllTransports() {
        /// <summary>
        /// Incomings the outgoing FT p_ get all transports.
        /// </summary>
        this._readdObject$2.GetGetAllTransports(Function.createDelegate(this, this._onSuccess_IncomingOutgoingFTP_GetAllTransports$2), Function.createDelegate(this, this._onFailure$2));
    },
    
    _onSuccess_APIType_RequestType$2: function com_ivp_srm_controls_vendor_script_VendorScript$_onSuccess_APIType_RequestType$2(result, args) {
        /// <summary>
        /// Called when [success_ API type_ request type].
        /// </summary>
        /// <param name="result" type="Object">
        /// The result.
        /// </param>
        /// <param name="args" type="Object">
        /// The args.
        /// </param>
        var listResult = result;
        com.ivp.srm.controls.vendor.script.VendorCommons.bindDropDown(this._getControlByDisplayName$2('API Type'), listResult, null);
        if (this.get_hasSavedValue()) {
            (this._getControlByDisplayName$2('API Type')).value = this._savedValues$2['API Type'].toString();
            if (this.get_typeOfControl() === 'REALTIMEPREFERENCE' && this._savedValues$2['API Type'].toString().toUpperCase() === 'FTP') {
                this._hideShowElementsAndItsTag$2(this._getControlByDisplayName$2('Transport'), false);
            }
            else if (this.get_typeOfControl() === 'REALTIMEPREFERENCE' && this._savedValues$2['API Type'].toString().toUpperCase() !== 'FTP') {
                this._hideShowElementsAndItsTag$2(this._getControlByDisplayName$2('Transport'), true);
            }
        }
    },
    
    _onSuccess_VendorIdentifier_InstrumentIdType$2: function com_ivp_srm_controls_vendor_script_VendorScript$_onSuccess_VendorIdentifier_InstrumentIdType$2(result, args) {
        /// <summary>
        /// Called when [success_ vendor identifier_ instrument id type].
        /// </summary>
        /// <param name="result" type="Object">
        /// The result.
        /// </param>
        /// <param name="args" type="Object">
        /// The args.
        /// </param>
        var listResult = result;
        com.ivp.srm.controls.vendor.script.VendorCommons.bindDropDown(this._getControlByDisplayName$2('Vendor Identifier'), listResult, null);
        if (this.get_hasSavedValue()) {
            var key = 'Vendor Identifier';
            if (this.get_typeOfControl() === 'FTP') {
                key += '_Request';
            }
            (this._getControlByDisplayName$2('Vendor Identifier')).value = this._savedValues$2[key].toString();
        }
    },
    
    _onSuccess_MarketSector$2: function com_ivp_srm_controls_vendor_script_VendorScript$_onSuccess_MarketSector$2(result, args) {
        /// <summary>
        /// Called when [success_ market sector].
        /// </summary>
        /// <param name="result" type="Object">
        /// The result.
        /// </param>
        /// <param name="args" type="Object">
        /// The args.
        /// </param>
        var listResult = result;
        com.ivp.srm.controls.vendor.script.VendorCommons.bindDropDown(this._getControlByDisplayName$2('Market Sector'), listResult, null);
        if (this.get_hasSavedValue()) {
            var key = 'Market Sector';
            if (this.get_typeOfControl() === 'FTP') {
                key += '_Request';
            }
            (this._getControlByDisplayName$2('Market Sector')).value = this._savedValues$2[key].toString();
        }
    },
    
    _onGetVendorTypeSuccess$2: function com_ivp_srm_controls_vendor_script_VendorScript$_onGetVendorTypeSuccess$2(result, args) {
        /// <summary>
        /// Called when [get vendor type success].
        /// </summary>
        /// <param name="result" type="Object">
        /// The result.
        /// </param>
        /// <param name="args" type="Object">
        /// The args.
        /// </param>
        var listResult = result;
        if (this.get_typeOfControl() === 'FTP' && this._requestORresponse$2 !== com.ivp.srm.controls.vendor.script.FTPType.none) {
            this._bindSelectedVendorType$2(listResult, this._getControlByNameAndRequestType$2('Vendor Type', 'Request'));
            this._bindSelectedVendorType$2(listResult, this._getControlByNameAndRequestType$2('Vendor Type', 'Response'));
        }
        else {
            this._bindSelectedVendorType$2(listResult, this._getControlByDisplayName$2('Vendor Type'));
        }
        if ((this.get_readOnlyVendorType().toString() != null)) {
            this._getControlByDisplayName$2('Vendor Type').disabled = this.get_readOnlyVendorType();
        }
    },
    
    _bindSelectedVendorType$2: function com_ivp_srm_controls_vendor_script_VendorScript$_bindSelectedVendorType$2(listResult, element) {
        /// <param name="listResult" type="Array">
        /// </param>
        /// <param name="element" type="Object" domElement="true">
        /// </param>
        com.ivp.srm.controls.vendor.script.VendorCommons.bindDropDown(element, listResult, null);
        if (!com.ivp.srm.controls.vendor.script.VendorCommons.isNullEmpty(this._selectedVendorTypeValue$2)) {
            var ele = element;
            var option = null;
            for (var i = 0; i < ele.options.length; i++) {
                option = ele.options[i];
                if (option.value.toLowerCase() === this._selectedVendorTypeValue$2.toLowerCase()) {
                    option.selected = true;
                    break;
                }
            }
        }
    },
    
    _onSuccess_AssetTypes$2: function com_ivp_srm_controls_vendor_script_VendorScript$_onSuccess_AssetTypes$2(result, args) {
        /// <summary>
        /// Called when [success_ asset types].
        /// </summary>
        /// <param name="result" type="Object">
        /// The result.
        /// </param>
        /// <param name="args" type="Object">
        /// The args.
        /// </param>
        var listResult = result;
        com.ivp.srm.controls.vendor.script.VendorCommons.bindDropDown(this._getControlByDisplayName$2('Asset Type'), listResult, null);
        if (this.get_hasSavedValue()) {
            var key = 'Asset Type';
            if (this.get_typeOfControl() === 'FTP') {
                key += '_Request';
            }
            (this._getControlByDisplayName$2('Asset Type')).value = this._savedValues$2[key].toString();
        }
    },
    
    _onSuccess_Transport_GetAllTransports$2: function com_ivp_srm_controls_vendor_script_VendorScript$_onSuccess_Transport_GetAllTransports$2(result, args) {
        /// <summary>
        /// Called when [success_ transport_ get all transports].
        /// </summary>
        /// <param name="result" type="Object">
        /// The result.
        /// </param>
        /// <param name="args" type="Object">
        /// The args.
        /// </param>
        var s1 = result.toString();
        var _dataSetDOM = com.ivp.srm.controls.vendor.script.VendorParser.genericParser(s1);
        var s = this._getControlByDisplayName$2('Transport');
        var _texts = _dataSetDOM.selectNodes('//' + s.attributes.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._dataTextField).value);
        var _values = _dataSetDOM.selectNodes('//' + s.attributes.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._dataValueField).value);
        com.ivp.srm.controls.vendor.script.VendorCommons.bindDropDown(s, _texts, _values);
        if (this.get_hasSavedValue()) {
            if (this._savedValues$2['Transport'] != null) {
                (this._getControlByDisplayName$2('Transport')).value = this._savedValues$2['Transport'].toString();
            }
        }
    },
    
    _onSuccess_IncomingOutgoingFTP_GetAllTransports$2: function com_ivp_srm_controls_vendor_script_VendorScript$_onSuccess_IncomingOutgoingFTP_GetAllTransports$2(result, args) {
        /// <summary>
        /// Called when [success_ incoming outgoing FT p_ get all transports].
        /// </summary>
        /// <param name="result" type="Object">
        /// The result.
        /// </param>
        /// <param name="args" type="Object">
        /// The args.
        /// </param>
        var s1 = result.toString();
        var _dataSetDOM = com.ivp.srm.controls.vendor.script.VendorParser.genericParser(s1);
        var s = this._getControlByDisplayName$2('Outgoing FTP');
        var _texts = _dataSetDOM.selectNodes('//' + s.attributes.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._dataTextField).value);
        var _values = _dataSetDOM.selectNodes('//' + s.attributes.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._dataValueField).value);
        com.ivp.srm.controls.vendor.script.VendorCommons.bindDropDown(s, _texts, _values);
        var key;
        if (this.get_hasSavedValue()) {
            key = 'Outgoing FTP';
            if (this.get_typeOfControl() === 'FTP') {
                key += '_Request';
            }
            (this._getControlByDisplayName$2('Outgoing FTP')).value = this._savedValues$2[key].toString();
        }
        s = this._getControlByDisplayName$2('Incoming FTP');
        com.ivp.srm.controls.vendor.script.VendorCommons.bindDropDown(s, _texts, _values);
        if (this.get_hasSavedValue()) {
            key = 'Incoming FTP';
            if (this.get_typeOfControl() === 'FTP') {
                key += '_Response';
            }
            (this._getControlByDisplayName$2('Incoming FTP')).value = this._savedValues$2[key].toString();
        }
    },
    
    _onSuccessGetHTML$2: function com_ivp_srm_controls_vendor_script_VendorScript$_onSuccessGetHTML$2(result, args) {
        /// <summary>
        /// Called when [success get HTML]. Gets the html page according to vendor type
        /// </summary>
        /// <param name="result" type="Object">
        /// The result.
        /// </param>
        /// <param name="args" type="Object">
        /// The args.
        /// </param>
        if (this._getControlByDisplayName$2('Task Name') != null) {
            if (!com.ivp.srm.controls.vendor.script.VendorCommons.isNullEmpty(this.get_buttonSubmitID())) {
                com.ivp.srm.controls.vendor.script.ManageHandlers.removeHandler(this.get_buttonSubmitID());
            }
            if (!com.ivp.srm.controls.vendor.script.VendorCommons.isNullEmpty(this.get_buttonCancelID())) {
                com.ivp.srm.controls.vendor.script.ManageHandlers.removeHandler(this.get_buttonCancelID());
            }
            this.set_vendorHTML(result.toString());
            this.set_hasSavedValue(false);
            this._reCreateControl$2();
        }
    },
    
    _onSuccessGetRequestType$2: function com_ivp_srm_controls_vendor_script_VendorScript$_onSuccessGetRequestType$2(result, args) {
        /// <summary>
        /// Called when [success get request type].
        /// </summary>
        /// <param name="result" type="Object">
        /// The result.
        /// </param>
        /// <param name="args" type="Object">
        /// The args.
        /// </param>
        var listResult = result;
        com.ivp.srm.controls.vendor.script.VendorCommons.bindDropDown(this._getControlByDisplayName$2('API Type'), listResult, null);
        if (this.get_hasSavedValue()) {
            (this._getControlByDisplayName$2('API Type')).value = this._savedValues$2['API Type'].toString();
            if (this.get_typeOfControl() === 'REALTIMEPREFERENCE' && this._savedValues$2['API Type'].toString().toUpperCase() === 'FTP') {
                this._hideShowElementsAndItsTag$2(this._getControlByDisplayName$2('Transport'), false);
            }
            else if (this.get_typeOfControl() === 'REALTIMEPREFERENCE' && this._savedValues$2['API Type'].toString().toUpperCase() !== 'FTP') {
                this._hideShowElementsAndItsTag$2(this._getControlByDisplayName$2('Transport'), true);
            }
        }
    },
    
    _onSuccessBindApplicationSpecificData$2: function com_ivp_srm_controls_vendor_script_VendorScript$_onSuccessBindApplicationSpecificData$2(result, args) {
        /// <summary>
        /// Called when [success bind application specific data].
        /// Binds the data for third party server methods
        /// </summary>
        /// <param name="result" type="Object">
        /// The result.
        /// </param>
        /// <param name="args" type="Object">
        /// The args.
        /// </param>
        var data = Sys.Serialization.JavaScriptSerializer.deserialize(result.toString());
        if (data.DataType === com.ivp.srm.controls.vendor.script.DataType.dataSet) {
            var _dataSetDOM = com.ivp.srm.controls.vendor.script.VendorParser.genericParser(data.ReturnData.toString());
            var s = com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(data.ControlId);
            var _texts = _dataSetDOM.selectNodes('//' + s.attributes.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._dataTextField).value);
            var _values = _dataSetDOM.selectNodes('//' + s.attributes.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._dataValueField).value);
            com.ivp.srm.controls.vendor.script.VendorCommons.bindDropDown(s, _texts, _values);
            if (this.get_hasSavedValue()) {
                var key = s.attributes.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._displayText).value;
                if (this.get_typeOfControl() === 'FTP') {
                    key += '_Request';
                }
                if (this._savedValues$2[key] != null) {
                    s.value = this._savedValues$2[key].toString();
                }
            }
            if (s.attributes.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._displayText).value === 'DataSource') {
                if (this.get_typeOfControl() === 'REALTIMEPREFERENCE') {
                    (s).disabled = this.get_readOnlyDatasourceCtrl();
                }
            }
        }
    },
    
    _onFailure$2: function com_ivp_srm_controls_vendor_script_VendorScript$_onFailure$2(args) {
        /// <summary>
        /// Called when [failure].
        /// </summary>
        /// <param name="args" type="Object">
        /// The args.
        /// </param>
        var exception = args;
        alert(exception.get_message());
    },
    
    _selectedFTPTransport$2: function com_ivp_srm_controls_vendor_script_VendorScript$_selectedFTPTransport$2() {
        /// <summary>
        /// sets the selected value for Transport type if the control has saved values
        /// </summary>
        if (this.get_hasSavedValue()) {
            (this._getControlByNameAndRequestType$2('Transport Type', 'Request')).value = this._savedValues$2['Transport Type_Request'].toString();
            (this._getControlByNameAndRequestType$2('Transport Type', 'Response')).value = this._savedValues$2['Transport Type_Response'].toString();
        }
    },
    
    _selectedOutgoingIncomingFTP$2: function com_ivp_srm_controls_vendor_script_VendorScript$_selectedOutgoingIncomingFTP$2() {
        /// <summary>
        /// sets the selected value for Outgoing and Incoming FTP if the control has saved values
        /// </summary>
        if (this.get_hasSavedValue()) {
            (this._getControlByDisplayName$2('Outgoing FTP')).value = this._savedValues$2['Outgoing FTP_Request'].toString();
            (this._getControlByDisplayName$2('Incoming FTP')).value = this._savedValues$2['Incoming FTP_Response'].toString();
        }
    },
    
    _setSavedValuesToControl$2: function com_ivp_srm_controls_vendor_script_VendorScript$_setSavedValuesToControl$2() {
        /// <summary>
        /// Sets the saved values to Task Name and Task Description
        /// </summary>
        if (this.get_hasSavedValue()) {
            if (this.get_typeOfControl() === 'FTP') {
                (this._getControlByNameAndRequestType$2('Task Name', 'Request')).value = this._savedValues$2['Task Name_Request'].toString();
                if (this._savedValues$2['Task Description_Request'] != null && this._savedValues$2['Task Description_Request'].toString() !== '-1') {
                    (this._getControlByNameAndRequestType$2('Task Description', 'Request')).value = this._savedValues$2['Task Description_Request'].toString();
                }
                (this._getControlByNameAndRequestType$2('Task Name', 'Response')).value = this._savedValues$2['Task Name_Response'].toString();
                if (this._savedValues$2['Task Description_Response'] != null && this._savedValues$2['Task Description_Response'].toString() !== '-1') {
                    (this._getControlByNameAndRequestType$2('Task Description', 'Response')).value = this._savedValues$2['Task Description_Response'].toString();
                }
                if (this._getControlByNameAndRequestType$2('Data Request Type', 'Request') != null && this._savedValues$2['Data Request Type_Request'] != null) {
                    (this._getControlByNameAndRequestType$2('Data Request Type', 'Request')).value = this._savedValues$2['Data Request Type_Request'].toString();
                }
            }
            else {
                (this._getControlByDisplayName$2('Task Name')).value = this._savedValues$2['Task Name'].toString();
                if (this._savedValues$2['Task Description'] != null && this._savedValues$2['Task Description'].toString() !== '-1') {
                    (this._getControlByDisplayName$2('Task Description')).value = this._savedValues$2['Task Description'].toString();
                }
            }
        }
    },
    
    _checkMandatoryFields$2: function com_ivp_srm_controls_vendor_script_VendorScript$_checkMandatoryFields$2() {
        /// <summary>
        /// Checks the mandatory fields and displays error in mandatory fields.
        /// </summary>
        /// <returns type="Boolean"></returns>
        var attributeCollection = null;
        var is_mandatory;
        var is_visible;
        var display_text;
        var contexttype;
        var contextTypeAttrib;
        var ele;
        var _id;
        var isValid = true;
        var _isthirdPartyControl;
        var _autoCompleteControlID;
        var autoCompleteComponentId;
        var errorHolder = com.ivp.srm.controls.vendor.script.VendorCommons.createErrorMessageHolder(this._errorDiv$2);
        for (var i = 0; i < this._listNodes$2.length; i++) {
            attributeCollection = (this._listNodes$2[i]).attributes;
            is_mandatory = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._isMandatory);
            is_visible = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._visible);
            display_text = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._displayText);
            _id = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._id);
            contexttype = ((contextTypeAttrib = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._contextType)) == null) ? '' : ' in ' + contextTypeAttrib.value + ' Section.';
            _isthirdPartyControl = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._isThirdPartyControl);
            _autoCompleteControlID = attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._autoCompleteControlID);
            if (_id.value === this.get_element().id + '_dummy' && !com.ivp.srm.controls.vendor.script.VendorCommons.isNullEmpty(this.get_buttonSubmitID())) {
                _id.value = this.get_buttonSubmitID();
            }
            if (_id.value === this.get_element().id + '_Clear' && !com.ivp.srm.controls.vendor.script.VendorCommons.isNullEmpty(this.get_buttonCancelID())) {
                _id.value = this.get_buttonCancelID();
            }
            ele = com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(_id.value);
            if (ele.style.display !== 'none' && is_mandatory != null && Boolean.parse(is_mandatory.value)) {
                switch (ele.tagName.toUpperCase()) {
                    case 'SELECT':
                        if ((ele).selectedIndex === 0) {
                            com.ivp.srm.controls.vendor.script.VendorCommons.createErrorMessage(errorHolder, display_text.value + ' cannot be left unselected ' + contexttype);
                            isValid = false;
                        }
                        break;
                    case 'INPUT':
                        if ((ele).type.toUpperCase() === 'TEXT' && (ele).value.trim() === '') {
                            com.ivp.srm.controls.vendor.script.VendorCommons.createErrorMessage(errorHolder, display_text.value + ' cannot be left blank ' + contexttype);
                            isValid = false;
                        }
                        break;
                    case 'DIV':
                        if (_autoCompleteControlID != null && _isthirdPartyControl != null) {
                            if (Boolean.parse(_isthirdPartyControl.value)) {
                                autoCompleteComponentId = document.getElementById(_autoCompleteControlID.value).children[0].id.trim().substr(4);
                                var autocomplete = $find(autoCompleteComponentId);
                                var list = autocomplete.getSelectedTokens();
                                if (list.length === 0) {
                                    com.ivp.srm.controls.vendor.script.VendorCommons.createErrorMessage(errorHolder, display_text.value + ' cannot be left blank ' + contexttype);
                                    isValid = false;
                                }
                            }
                        }
                        break;
                }
            }
        }
        return isValid;
    },
    
    _getControlByDisplayName$2: function com_ivp_srm_controls_vendor_script_VendorScript$_getControlByDisplayName$2(displayName) {
        /// <summary>
        /// Gets the control by Display Name
        /// </summary>
        /// <param name="displayName" type="String">
        /// The display name.
        /// </param>
        /// <returns type="Object" domElement="true"></returns>
        var attributeCollection;
        var element = null;
        for (var i = 0; i < this._listNodes$2.length; i++) {
            attributeCollection = (this._listNodes$2[i]).attributes;
            if (attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._displayText) != null && attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._displayText).value === displayName) {
                element = com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._id).value);
                return element;
            }
        }
        return null;
    },
    
    _hideShowElementsAndItsTag$2: function com_ivp_srm_controls_vendor_script_VendorScript$_hideShowElementsAndItsTag$2(_element, hide) {
        /// <summary>
        /// Hides and shows elements and its tag.
        /// </summary>
        /// <param name="_element" type="Object" domElement="true">
        /// The _element.
        /// </param>
        /// <param name="hide" type="Boolean">
        /// if set to <c>true</c> [hide].
        /// </param>
        if (_element != null) {
            var _display = (hide) ? 'none' : '';
            if ((_element).type.toUpperCase() === 'BUTTON' || (_element).type.toUpperCase() === 'SUBMIT') {
                _element.style.display = _display;
                return;
            }
            _element.style.display = _display;
            _element.parentNode.style.display = _display;
            var prevControl = this._getPrevControl$2(_element.parentNode);
            prevControl.style.display = _display;
        }
    },
    
    _getPrevControl$2: function com_ivp_srm_controls_vendor_script_VendorScript$_getPrevControl$2(dOMElement) {
        /// <param name="dOMElement" type="Object" domElement="true">
        /// </param>
        /// <returns type="Object" domElement="true"></returns>
        if (dOMElement.previousSibling.nodeName === '#text') {
            return this._getPrevControl$2(dOMElement.previousSibling);
        }
        else {
            return dOMElement.previousSibling;
        }
    },
    
    _getControlByNameAndRequestType$2: function com_ivp_srm_controls_vendor_script_VendorScript$_getControlByNameAndRequestType$2(displayName, contextType) {
        /// <summary>
        /// Gets control by name and context type (request/response).
        /// </summary>
        /// <param name="displayName" type="String">
        /// The display name.
        /// </param>
        /// <param name="contextType" type="String">
        /// Type of the context.
        /// </param>
        /// <returns type="Object" domElement="true"></returns>
        var attributeCollection;
        var element = null;
        for (var i = 0; i < this._listNodes$2.length; i++) {
            attributeCollection = (this._listNodes$2[i]).attributes;
            if (attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._displayText) != null && attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._displayText).value === displayName && attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._contextType) != null && attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._contextType).value === contextType) {
                element = com.ivp.srm.controls.vendor.script.VendorControls.getElementByID(attributeCollection.getNamedItem(com.ivp.srm.controls.vendor.script.VendorAttributeConstants._id).value);
                return element;
            }
        }
        return null;
    },
    
    rBbgRequestType: function com_ivp_srm_controls_vendor_script_VendorScript$rBbgRequestType() {
        this._readdObject$2.GetBbgRequestType(Function.createDelegate(this, this._onSuccess_APIType_RequestType$2), Function.createDelegate(this, this._onFailure$2));
    },
    
    rBbgInstrumentIdType: function com_ivp_srm_controls_vendor_script_VendorScript$rBbgInstrumentIdType() {
        this._readdObject$2.GetBbgInstrumentIdType(Function.createDelegate(this, this._onSuccess_VendorIdentifier_InstrumentIdType$2), Function.createDelegate(this, this._onFailure$2));
    },
    
    rBbgMarketSector: function com_ivp_srm_controls_vendor_script_VendorScript$rBbgMarketSector() {
        this._readdObject$2.GetBbgMarketSector(Function.createDelegate(this, this._onSuccess_MarketSector$2), Function.createDelegate(this, this._onFailure$2));
    },
    
    rReuterRequestType: function com_ivp_srm_controls_vendor_script_VendorScript$rReuterRequestType() {
        this._readdObject$2.GetRReuterRequestType(Function.createDelegate(this, this._onSuccess_APIType_RequestType$2), Function.createDelegate(this, this._onFailure$2));
    },
    
    rReuterInstrumentIdType: function com_ivp_srm_controls_vendor_script_VendorScript$rReuterInstrumentIdType() {
        this._readdObject$2.GetRReuterInstrumentIdType(Function.createDelegate(this, this._onSuccess_VendorIdentifier_InstrumentIdType$2), Function.createDelegate(this, this._onFailure$2));
    },
    
    rReuterAssetTypes: function com_ivp_srm_controls_vendor_script_VendorScript$rReuterAssetTypes() {
        this._readdObject$2.GetRReuterAssetTypes(Function.createDelegate(this, this._onSuccess_AssetTypes$2), Function.createDelegate(this, this._onFailure$2));
    },
    
    rwsoInstrumentIdType: function com_ivp_srm_controls_vendor_script_VendorScript$rwsoInstrumentIdType() {
        this._readdObject$2.GetWSOInstrumentIdType(Function.createDelegate(this, this._onSuccess_VendorIdentifier_InstrumentIdType$2), Function.createDelegate(this, this._onFailure$2));
    },
    
    getAllTransports: function com_ivp_srm_controls_vendor_script_VendorScript$getAllTransports() {
        this._readdObject$2.GetGetAllTransports(Function.createDelegate(this, this._onSuccess_Transport_GetAllTransports$2), Function.createDelegate(this, this._onFailure$2));
    },
    
    bindApplicationSpecificData: function com_ivp_srm_controls_vendor_script_VendorScript$bindApplicationSpecificData(className, methodName, assembly, id, nameSpace) {
        /// <param name="className" type="String">
        /// </param>
        /// <param name="methodName" type="String">
        /// </param>
        /// <param name="assembly" type="String">
        /// </param>
        /// <param name="id" type="String">
        /// </param>
        /// <param name="nameSpace" type="String">
        /// </param>
        this._readdObject$2.BindApplicationSpecificData(className, methodName, assembly, id, nameSpace, Function.createDelegate(this, this._onSuccessBindApplicationSpecificData$2), Function.createDelegate(this, this._onFailure$2));
    },
    
    getRequestType: function com_ivp_srm_controls_vendor_script_VendorScript$getRequestType(vendorType, licenseType) {
        /// <param name="vendorType" type="Number" integer="true">
        /// </param>
        /// <param name="licenseType" type="Number" integer="true">
        /// </param>
        this._readdObject$2.GetRequestType(vendorType, licenseType, Function.createDelegate(this, this._onSuccessGetRequestType$2), Function.createDelegate(this, this._onFailure$2));
    }
}


Type.registerNamespace('com.ivp.srm.vendorapi');

////////////////////////////////////////////////////////////////////////////////
// com.ivp.srm.vendorapi.RADWebServiceException

com.ivp.srm.vendorapi.RADWebServiceException = function com_ivp_srm_vendorapi_RADWebServiceException() {
}
com.ivp.srm.vendorapi.RADWebServiceException.prototype = {
    
    get_message: function com_ivp_srm_vendorapi_RADWebServiceException$get_message() {
        /// <summary>
        /// Get_messages this instance.
        /// </summary>
        /// <returns type="String"></returns>
        return '';
    },
    
    get_statusCode: function com_ivp_srm_vendorapi_RADWebServiceException$get_statusCode() {
        /// <summary>
        /// Get_statuses the code.
        /// </summary>
        /// <returns type="Number" integer="true"></returns>
        return 0;
    }
}


com.ivp.srm.controls.vendor.script.ClientMethodNames.registerClass('com.ivp.srm.controls.vendor.script.ClientMethodNames');
com.ivp.srm.controls.vendor.script.ServerMethodNames.registerClass('com.ivp.srm.controls.vendor.script.ServerMethodNames');
com.ivp.srm.controls.vendor.script.ManageHandlers.registerClass('com.ivp.srm.controls.vendor.script.ManageHandlers');
com.ivp.srm.controls.vendor.script.VendorCommons.registerClass('com.ivp.srm.controls.vendor.script.VendorCommons');
com.ivp.srm.controls.vendor.script.VendorAttributeConstants.registerClass('com.ivp.srm.controls.vendor.script.VendorAttributeConstants');
com.ivp.srm.controls.vendor.script.MethodEventPair.registerClass('com.ivp.srm.controls.vendor.script.MethodEventPair');
com.ivp.srm.controls.vendor.script.VendorControls.registerClass('com.ivp.srm.controls.vendor.script.VendorControls');
com.ivp.srm.controls.vendor.script.TypeOfVendorControl.registerClass('com.ivp.srm.controls.vendor.script.TypeOfVendorControl');
com.ivp.srm.controls.vendor.script.ApplicationSpecificData.registerClass('com.ivp.srm.controls.vendor.script.ApplicationSpecificData');
com.ivp.srm.controls.vendor.script.VendorParser.registerClass('com.ivp.srm.controls.vendor.script.VendorParser');
com.ivp.srm.controls.vendor.script.VendorScript.registerClass('com.ivp.srm.controls.vendor.script.VendorScript', Sys.UI.Behavior, com.ivp.srm.controls.vendor.script.IClientMethods, com.ivp.srm.controls.vendor.script.IServerMethods);
com.ivp.srm.vendorapi.RADWebServiceException.registerClass('com.ivp.srm.vendorapi.RADWebServiceException');
com.ivp.srm.controls.vendor.script.ClientMethodNames._clearHandler = 'ClearControl';
com.ivp.srm.controls.vendor.script.ClientMethodNames._displayControl = 'DisplayControl';
com.ivp.srm.controls.vendor.script.ClientMethodNames._getOutputXML = 'GetOutputXML';
com.ivp.srm.controls.vendor.script.ClientMethodNames._requestClicked = 'RequestClicked';
com.ivp.srm.controls.vendor.script.ClientMethodNames._responseClicked = 'ResponseClicked';
com.ivp.srm.controls.vendor.script.ClientMethodNames._hideShowTransport = 'HideShowTransport';
com.ivp.srm.controls.vendor.script.ServerMethodNames._rBbgRequestType = 'RBbgRequestType';
com.ivp.srm.controls.vendor.script.ServerMethodNames._rBbgInstrumentIdType = 'RBbgInstrumentIdType';
com.ivp.srm.controls.vendor.script.ServerMethodNames._rBbgMarketSector = 'RBbgMarketSector';
com.ivp.srm.controls.vendor.script.ServerMethodNames._bindShuttleForIdentifiers = 'BindShuttleForIdentifiers';
com.ivp.srm.controls.vendor.script.ServerMethodNames._rReuterRequestType = 'RReuterRequestType';
com.ivp.srm.controls.vendor.script.ServerMethodNames._rReuterInstrumentIdType = 'RReuterInstrumentIdType';
com.ivp.srm.controls.vendor.script.ServerMethodNames._rReuterAssetTypes = 'RReuterAssetTypes';
com.ivp.srm.controls.vendor.script.ServerMethodNames._rwsoInstrumentIdType = 'RWSOInstrumentIdType';
com.ivp.srm.controls.vendor.script.ServerMethodNames._getAllTransports = 'GetAllTransports';
com.ivp.srm.controls.vendor.script.ServerMethodNames._getRequestType = 'GetRequestType';
com.ivp.srm.controls.vendor.script.ManageHandlers._elementList = null;
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._clientMethodName = 'clientMethodName';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._serverMethodName = 'serverMethodName';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._clientEventType = 'clientEventType';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._serverEventType = 'serverEventType';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._id = 'id';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._displayText = 'display_text';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._key = 'key';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._visible = 'visible';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._isMandatory = 'is_mandatory';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._dataTextField = 'datatextfield';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._dataValueField = 'datavaluefield';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._savedValue = 'saved_value';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._contextType = 'contexttype';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._assmebly = 'assembly';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._classname = 'classname';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._namespace = 'namespace';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._vendorPropertyName = 'vendorpropertyname';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._customClientMethod = 'customclientmethod';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._serverMethodArguments = 'serverMethodArguments';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._isThirdPartyControl = 'isthirdPartyControl';
com.ivp.srm.controls.vendor.script.VendorAttributeConstants._autoCompleteControlID = 'autoCompleteControlID';
com.ivp.srm.controls.vendor.script.TypeOfVendorControl._API = 'API';
com.ivp.srm.controls.vendor.script.TypeOfVendorControl._apI_Bloomberg = 'API_Bloomberg';
com.ivp.srm.controls.vendor.script.TypeOfVendorControl._apI_Reuters = 'API_Reuters';
com.ivp.srm.controls.vendor.script.TypeOfVendorControl._apI_MarkitWSO = 'API_MarkitWSO';
com.ivp.srm.controls.vendor.script.TypeOfVendorControl._FTP = 'FTP';
com.ivp.srm.controls.vendor.script.TypeOfVendorControl._ftP_Bloomberg = 'FTP_Bloomberg';
com.ivp.srm.controls.vendor.script.TypeOfVendorControl._ftP_Reuters = 'FTP_Reuters';
com.ivp.srm.controls.vendor.script.TypeOfVendorControl._realtimepreferencE_Bloomberg = 'REALTIMEPREFERENCE_Bloomberg';
com.ivp.srm.controls.vendor.script.TypeOfVendorControl._realtimepreferencE_Reuters = 'REALTIMEPREFERENCE_Reuters';
com.ivp.srm.controls.vendor.script.TypeOfVendorControl._REALTIMEPREFERENCE = 'REALTIMEPREFERENCE';

// ---- Do not remove this footer ----
// This script was generated using Script# v0.5.5.0 (http://projects.nikhilk.net/ScriptSharp)
// -----------------------------------
