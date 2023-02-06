// RADBrowserScripts.js
//


Type.registerNamespace('com.ivp.rad.controls.scripts.RADBrowserScripts');

////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.controls.scripts.RADBrowserScripts.RADDOMElement

com.ivp.rad.controls.scripts.RADBrowserScripts.RADDOMElement = function com_ivp_rad_controls_scripts_RADBrowserScripts_RADDOMElement() {
    /// <field name="_isElementFound" type="Boolean">
    /// </field>
}
com.ivp.rad.controls.scripts.RADBrowserScripts.RADDOMElement.prototype = {
    
    getInnerContent: function com_ivp_rad_controls_scripts_RADBrowserScripts_RADDOMElement$getInnerContent(element) {
        /// <param name="element" type="Object" domElement="true">
        /// </param>
        /// <returns type="String"></returns>
        var domElement = element;
        if (Sys.Browser.name === 'Firefox') {
            return domElement.textContent;
        }
        else {
            return domElement.innerText;
        }
    },
    
    setInnerContent: function com_ivp_rad_controls_scripts_RADBrowserScripts_RADDOMElement$setInnerContent(element, innerText) {
        /// <param name="element" type="Object" domElement="true">
        /// </param>
        /// <param name="innerText" type="String">
        /// </param>
        var domElement = element;
        if (Sys.Browser.name === 'Firefox') {
            domElement.textContent = innerText;
        }
        else {
            domElement.innerText = innerText;
        }
    },
    
    addOptionElement: function com_ivp_rad_controls_scripts_RADBrowserScripts_RADDOMElement$addOptionElement(element, option) {
        /// <param name="element" type="Object" domElement="true">
        /// </param>
        /// <param name="option" type="Object" domElement="true">
        /// </param>
        if (Sys.Browser.name === 'Firefox') {
            element.appendChild(option);
        }
        else {
            element.add(option);
        }
    },
    
    _isElementFound: false,
    
    containsElement: function com_ivp_rad_controls_scripts_RADBrowserScripts_RADDOMElement$containsElement(element, findElement) {
        /// <param name="element" type="Object" domElement="true">
        /// </param>
        /// <param name="findElement" type="Object" domElement="true">
        /// </param>
        /// <returns type="Boolean"></returns>
        if (Sys.Browser.name === 'Firefox') {
            this._isElementFound = false;
            if (element == null || findElement == null) {
                return this._isElementFound;
            }
            if (element === findElement) {
                this._isElementFound = true;
                return true;
            }
            for (var i = 0; i < element.childNodes.length; i++) {
                if (this._isElementFound) {
                    break;
                }
                this.containsElement(element.childNodes[i], findElement);
            }
            return this._isElementFound;
        }
        else {
            if (element == null || findElement == null) {
                return false;
            }
            return element === findElement || element.contains(findElement);
        }
    }
}


com.ivp.rad.controls.scripts.RADBrowserScripts.RADDOMElement.registerClass('com.ivp.rad.controls.scripts.RADBrowserScripts.RADDOMElement');

// ---- Do not remove this footer ----
// This script was generated using Script# v0.5.5.0 (http://projects.nikhilk.net/ScriptSharp)
// -----------------------------------
