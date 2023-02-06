// DragDropScripts.js
//


Type.registerNamespace('com.ivp.rad.controls.scripts.raddragdropscripts');

////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.controls.scripts.raddragdropscripts.DragObjectType

com.ivp.rad.controls.scripts.raddragdropscripts.DragObjectType = function() { 
    /// <field name="dropableObject" type="Number" integer="true" static="true">
    /// </field>
    /// <field name="droppedObject" type="Number" integer="true" static="true">
    /// </field>
};
com.ivp.rad.controls.scripts.raddragdropscripts.DragObjectType.prototype = {
    dropableObject: 0, 
    droppedObject: 1
}
com.ivp.rad.controls.scripts.raddragdropscripts.DragObjectType.registerEnum('com.ivp.rad.controls.scripts.raddragdropscripts.DragObjectType', false);


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.controls.scripts.raddragdropscripts.DragDrop

com.ivp.rad.controls.scripts.raddragdropscripts.DragDrop = function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop(componentID) {
    /// <summary>
    /// DragDrop Class for Handling all types of Drag Drop functioalitites
    /// </summary>
    /// <param name="componentID" type="String">
    /// The component ID.
    /// </param>
    /// <field name="_dragDropInfos$1" type="Array">
    /// </field>
    /// <field name="_onDragStart$1" type="Sys.UI.DomEventHandler">
    /// </field>
    /// <field name="_onDragMove$1" type="Sys.UI.DomEventHandler">
    /// </field>
    /// <field name="_onDragEnd$1" type="Sys.UI.DomEventHandler">
    /// </field>
    /// <field name="_radDOMElement$1" type="com.ivp.rad.controls.scripts.RADBrowserScripts.RADDOMElement">
    /// </field>
    /// <field name="_dragStarted$1" type="Boolean">
    /// </field>
    /// <field name="_objectIndex$1" type="Number" integer="true">
    /// </field>
    /// <field name="_visualElementHolder$1" type="Object" domElement="true">
    /// </field>
    /// <field name="_visualElement$1" type="Object" domElement="true">
    /// </field>
    /// <field name="_dropContainer$1" type="Object" domElement="true">
    /// </field>
    /// <field name="_draggableObject$1" type="Object" domElement="true">
    /// </field>
    /// <field name="_dragObjectType$1" type="com.ivp.rad.controls.scripts.raddragdropscripts.DragObjectType">
    /// </field>
    /// <field name="_targetRaisedEvent$1" type="Object" domElement="true">
    /// </field>
    /// <field name="_index$1" type="String">
    /// </field>
    /// <field name="_createVisualClue$1" type="Boolean">
    /// </field>
    /// <field name="_dragDropPropertyChangedHandler$1" type="Sys.PropertyChangedEventHandler">
    /// </field>
    this._objectIndex$1 = -1;
    com.ivp.rad.controls.scripts.raddragdropscripts.DragDrop.initializeBase(this);
    this._radDOMElement$1 = new com.ivp.rad.controls.scripts.RADBrowserScripts.RADDOMElement();
    com.ivp.rad.controls.scripts.raddragdropscripts.DragDrop.callBaseMethod(this, 'set_id', [ componentID ]);
    if ($find(componentID) == null) {
        Sys.Application.addComponent(this);
    }
}
com.ivp.rad.controls.scripts.raddragdropscripts.DragDrop.prototype = {
    _dragDropInfos$1: null,
    _onDragStart$1: null,
    _onDragMove$1: null,
    _onDragEnd$1: null,
    _radDOMElement$1: null,
    _dragStarted$1: false,
    _visualElementHolder$1: null,
    _visualElement$1: null,
    _dropContainer$1: null,
    _draggableObject$1: null,
    _dragObjectType$1: 0,
    _targetRaisedEvent$1: null,
    _index$1: null,
    _createVisualClue$1: false,
    _dragDropPropertyChangedHandler$1: null,
    
    get_dragDropInfos: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$get_dragDropInfos() {
        /// <summary>
        /// Gets or sets the drag drop infos.
        /// </summary>
        /// <value type="Array"></value>
        if (this._dragDropInfos$1 == null) {
            this._dragDropInfos$1 = [];
        }
        return this._dragDropInfos$1;
    },
    set_dragDropInfos: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$set_dragDropInfos(value) {
        /// <summary>
        /// Gets or sets the drag drop infos.
        /// </summary>
        /// <value type="Array"></value>
        this._dragDropInfos$1 = value;
        return value;
    },
    
    get_draggableObject: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$get_draggableObject() {
        /// <summary>
        /// Gets the draggable object to be dragged.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return this._draggableObject$1;
    },
    
    get_dropContainer: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$get_dropContainer() {
        /// <summary>
        /// Gets the drop container.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return this._dropContainer$1;
    },
    
    get_dragObjectType: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$get_dragObjectType() {
        /// <summary>
        /// Gets the type of the drag object.
        /// </summary>
        /// <value type="com.ivp.rad.controls.scripts.raddragdropscripts.DragObjectType"></value>
        return this._dragObjectType$1;
    },
    
    get_index: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$get_index() {
        /// <summary>
        /// Gets the index of the drag object [used for the collection of draggable objects].
        /// </summary>
        /// <value type="String"></value>
        return this._index$1;
    },
    
    get_targetRaisedEvent: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$get_targetRaisedEvent() {
        /// <summary>
        /// Gets the target which raised event.
        /// </summary>
        /// <value type="Object" domElement="true"></value>
        return this._targetRaisedEvent$1;
    },
    
    get_createVisualClue: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$get_createVisualClue() {
        /// <summary>
        /// Gets or sets a value indicating whether to create visual clue.
        /// </summary>
        /// <value type="Boolean"></value>
        return this._createVisualClue$1;
    },
    set_createVisualClue: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$set_createVisualClue(value) {
        /// <summary>
        /// Gets or sets a value indicating whether to create visual clue.
        /// </summary>
        /// <value type="Boolean"></value>
        this._createVisualClue$1 = value;
        return value;
    },
    
    get_dragDropPropertyChangedHandler: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$get_dragDropPropertyChangedHandler() {
        /// <value type="Sys.PropertyChangedEventHandler"></value>
        return this._dragDropPropertyChangedHandler$1;
    },
    set_dragDropPropertyChangedHandler: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$set_dragDropPropertyChangedHandler(value) {
        /// <value type="Sys.PropertyChangedEventHandler"></value>
        this._dragDropPropertyChangedHandler$1 = value;
        return value;
    },
    
    updated: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$updated() {
        /// <summary>
        /// Has no built-in functionality; the Updated method is a placeholder for post-update logic
        /// in derived classes.
        /// </summary>
        com.ivp.rad.controls.scripts.raddragdropscripts.DragDrop.callBaseMethod(this, 'updated');
        this._createVisualClueElementHolder$1();
        this._createHandlers$1();
    },
    
    dispose: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$dispose() {
        /// <summary>
        /// Raises the <see cref="E:Sys.Component.Disposing" /> event of the current <see cref="T:Sys.Component" /> object
        /// and removes the component from the application.
        /// </summary>
        this._removeHandlers$1();
        this._removeVisualClueElementHolder$1();
        com.ivp.rad.controls.scripts.raddragdropscripts.DragDrop.callBaseMethod(this, 'dispose');
    },
    
    _createHandlers$1: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$_createHandlers$1() {
        /// <summary>
        /// Creates the handlers.
        /// </summary>
        try {
            this._removeHandlers$1();
        }
        catch ($e1) {
        }
        if (this._dragDropInfos$1 != null && this._dragDropInfos$1.length > 0) {
            this._onDragStart$1 = Function.createDelegate(this, this._dragStart$1);
            this._onDragEnd$1 = Function.createDelegate(this, this._dragEnd$1);
            for (var i = 0; i < this._dragDropInfos$1.length; i++) {
                this._addHandler$1((this._dragDropInfos$1[i]).draggableObject, 'mousedown', this._onDragStart$1);
            }
            this._onDragMove$1 = Function.createDelegate(this, this._dragging$1);
            this._addHandler$1(document.body, 'mousemove', this._onDragMove$1);
            this._addHandler$1(document.body, 'mouseup', this._onDragEnd$1);
        }
    },
    
    _removeHandlers$1: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$_removeHandlers$1() {
        /// <summary>
        /// Removes the handlers.
        /// </summary>
        if (this._dragDropInfos$1 != null && this._dragDropInfos$1.length > 0) {
            for (var i = 0; i < this._dragDropInfos$1.length; i++) {
                try {
                    $clearHandlers((this._dragDropInfos$1[i]).draggableObject);
                }
                catch ($e1) {
                }
            }
            if (this._onDragMove$1 != null) {
                try {
                    $removeHandler(document.body, 'mousemove', this._onDragMove$1);
                }
                catch ($e2) {
                }
            }
            if (this._onDragEnd$1 != null) {
                try {
                    $removeHandler(document.body, 'mouseup', this._onDragEnd$1);
                }
                catch ($e3) {
                }
            }
        }
    },
    
    _dragStart$1: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$_dragStart$1(e) {
        /// <summary>
        /// Fired when Drag is started.
        /// </summary>
        /// <param name="e" type="Sys.UI.DomEvent">
        /// The e.
        /// </param>
        var _found = false;
        this._draggableObject$1 = e.target;
        for (var i = 0; i < this._dragDropInfos$1.length; i++) {
            if (this._radDOMElement$1.containsElement((this._dragDropInfos$1[i]).draggableObject, this._draggableObject$1)) {
                this._draggableObject$1 = (this._dragDropInfos$1[i]).draggableObject;
                this._visualElement$1 = (this._dragDropInfos$1[i]).visualMovingElement;
                this._dragObjectType$1 = (this._dragDropInfos$1[i]).dragObjectType;
                this._index$1 = (this._dragDropInfos$1[i]).index;
                this._objectIndex$1 = i;
                _found = true;
                break;
            }
        }
        if (e.button === Sys.UI.MouseButton.rightButton || !_found) {
            return;
        }
        this.raisePropertyChanged('DragStarted');
        if (this._createVisualClue$1) {
            this._visualElementHolder$1.appendChild(this._visualElement$1);
            this._visualElement$1.style.position = 'absolute';
            this._visualElement$1.style.left = (e.clientX + 5 + document.documentElement.scrollLeft).toString() + 'px';
            this._visualElement$1.style.top = (e.clientY + document.documentElement.scrollTop).toString() + 'px';
            this._dragStarted$1 = true;
            e.preventDefault();
        }
    },
    
    _dragging$1: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$_dragging$1(domEvent) {
        /// <summary>
        /// Fires while Dragging.
        /// </summary>
        /// <param name="domEvent" type="Sys.UI.DomEvent">
        /// </param>
        if (this._dragStarted$1) {
            this._visualElement$1.style.left = (domEvent.clientX + 5 + document.documentElement.scrollLeft).toString() + 'px';
            this._visualElement$1.style.top = (domEvent.clientY + document.documentElement.scrollTop).toString() + 'px';
            domEvent.rawEvent.returnValue = false;
            domEvent.preventDefault();
        }
    },
    
    _dragEnd$1: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$_dragEnd$1(domEvent) {
        /// <summary>
        /// Fires when Drag is ended.
        /// </summary>
        /// <param name="domEvent" type="Sys.UI.DomEvent">
        /// </param>
        if (this._visualElementHolder$1.hasChildNodes()) {
            this._visualElementHolder$1.removeChild(this._visualElement$1);
        }
        if (this._dragStarted$1) {
            this._targetRaisedEvent$1 = domEvent.target;
            var _success = false;
            var _listContainers = (this._dragDropInfos$1[this._objectIndex$1]).dropContainers;
            for (var i = 0; i < _listContainers.length; i++) {
                this._dropContainer$1 = _listContainers[i];
                if (this._radDOMElement$1.containsElement(this._dropContainer$1, domEvent.target)) {
                    _success = true;
                    break;
                }
            }
            if (_success) {
                this.raisePropertyChanged('DragEndSuccess');
            }
            else {
                this.raisePropertyChanged('DragEndUnSuccess');
            }
            this._dragStarted$1 = false;
            domEvent.preventDefault();
        }
    },
    
    removeElement: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$removeElement(element) {
        /// <summary>
        /// Removes the draggable object from the DOM.
        /// </summary>
        /// <param name="element" type="Object" domElement="true">
        /// The element.
        /// </param>
        this._removeHandlers$1();
        var _found = false;
        var _dragDropInfo = null;
        for (var i = 0; i < this._dragDropInfos$1.length; i++) {
            _dragDropInfo = this._dragDropInfos$1[i];
            if (_dragDropInfo.draggableObject === element) {
                _found = true;
                break;
            }
        }
        if (_found) {
            Array.remove(this._dragDropInfos$1, _dragDropInfo);
            element.parentNode.removeChild(element);
        }
    },
    
    _removeVisualClueElementHolder$1: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$_removeVisualClueElementHolder$1() {
        if (this._radDOMElement$1 != null && this._visualElementHolder$1 != null && this._radDOMElement$1.containsElement(document.body, this._visualElementHolder$1)) {
            document.body.removeChild(this._visualElementHolder$1);
        }
    },
    
    _createVisualClueElementHolder$1: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$_createVisualClueElementHolder$1() {
        this._visualElementHolder$1 = document.getElementById(this.get_id() + '_visualElementHolder');
        if (this._visualElementHolder$1 == null) {
            this._visualElementHolder$1 = document.createElement('DIV');
            this._visualElementHolder$1.id = this.get_id() + '_visualElementHolder';
            document.body.appendChild(this._visualElementHolder$1);
        }
    },
    
    _addHandler$1: function com_ivp_rad_controls_scripts_raddragdropscripts_DragDrop$_addHandler$1(element, eventName, handler) {
        /// <summary>
        /// Adds the handler.
        /// </summary>
        /// <param name="element" type="Object" domElement="true">
        /// The element.
        /// </param>
        /// <param name="eventName" type="String">
        /// Name of the event.
        /// </param>
        /// <param name="handler" type="Sys.UI.DomEventHandler">
        /// The handler.
        /// </param>
        var _events = (element)._events;
        var _found = false;
        if (_events != null) {
            var _eventCache = _events[eventName];
            if (_eventCache != null) {
                for (var i = 0; i < _eventCache.length; i++) {
                    if ((_eventCache[i]).handler === handler) {
                        _found = true;
                        break;
                    }
                }
            }
        }
        if (!_found) {
            $addHandler(element, eventName, handler);
        }
    }
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.controls.scripts.raddragdropscripts.DragDropInfo

com.ivp.rad.controls.scripts.raddragdropscripts.DragDropInfo = function com_ivp_rad_controls_scripts_raddragdropscripts_DragDropInfo() {
    /// <field name="draggableObject" type="Object" domElement="true">
    /// </field>
    /// <field name="visualMovingElement" type="Object" domElement="true">
    /// </field>
    /// <field name="dropContainers" type="Array">
    /// </field>
    /// <field name="dragObjectType" type="com.ivp.rad.controls.scripts.raddragdropscripts.DragObjectType">
    /// </field>
    /// <field name="index" type="String">
    /// </field>
    this.dropContainers = [];
}
com.ivp.rad.controls.scripts.raddragdropscripts.DragDropInfo.prototype = {
    draggableObject: null,
    visualMovingElement: null,
    dragObjectType: 0,
    index: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.controls.scripts.raddragdropscripts._dragDomElement

com.ivp.rad.controls.scripts.raddragdropscripts._dragDomElement = function com_ivp_rad_controls_scripts_raddragdropscripts__dragDomElement() {
    /// <field name="_events" type="Object">
    /// </field>
    com.ivp.rad.controls.scripts.raddragdropscripts._dragDomElement.initializeBase(this);
}
com.ivp.rad.controls.scripts.raddragdropscripts._dragDomElement.prototype = {
    _events: null
}


////////////////////////////////////////////////////////////////////////////////
// com.ivp.rad.controls.scripts.raddragdropscripts._dragDomEventHandler

com.ivp.rad.controls.scripts.raddragdropscripts._dragDomEventHandler = function com_ivp_rad_controls_scripts_raddragdropscripts__dragDomEventHandler() {
    /// <field name="handler" type="Object">
    /// </field>
}
com.ivp.rad.controls.scripts.raddragdropscripts._dragDomEventHandler.prototype = {
    handler: null
}


com.ivp.rad.controls.scripts.raddragdropscripts.DragDrop.registerClass('com.ivp.rad.controls.scripts.raddragdropscripts.DragDrop', Sys.Component);
com.ivp.rad.controls.scripts.raddragdropscripts.DragDropInfo.registerClass('com.ivp.rad.controls.scripts.raddragdropscripts.DragDropInfo');
com.ivp.rad.controls.scripts.raddragdropscripts._dragDomElement.registerClass('com.ivp.rad.controls.scripts.raddragdropscripts._dragDomElement', Object);
com.ivp.rad.controls.scripts.raddragdropscripts._dragDomEventHandler.registerClass('com.ivp.rad.controls.scripts.raddragdropscripts._dragDomEventHandler');

// ---- Do not remove this footer ----
// This script was generated using Script# v0.5.5.0 (http://projects.nikhilk.net/ScriptSharp)
// -----------------------------------
