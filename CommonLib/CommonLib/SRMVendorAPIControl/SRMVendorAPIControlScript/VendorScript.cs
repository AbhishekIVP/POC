/***************************************************************************************************
 * 
 *  This source forms a part of the IVP RAD Software System and is a copyright of 
 *  Indus Valley Partners (Pvt) Ltd.

 *  All rights are reserved with IVP. No part of this work may be reproduced, stored, 
 *  adopted or transmitted in any form or by any means including but not limiting to 
 *  electronic, mechanical, photographic, graphic, optic recording or otherwise, 
 *  translated in any language or computer language, without the prior written permission 
 *  of

 *  Indus Valley Partners (Pvt) Ltd
 *  Unit 7&8, Bldg 4
 *  Vardhman Trade Center
 *  Nehru Place Greens
 *  New Delhi - 19

 *  Copyright 2007-2008 Indus Valley Partners (Pvt) Ltd.
 * 
 * 
 * Change History
 * Version      Date            Author          Comments
 * -------------------------------------------------------------------------------------------------
 * 1            05-11-2009      Mukul Saini     Initial Version
 **************************************************************************************************/
using System;
using System.DHTML;
using Sys;
using Sys.UI;
using com.ivp.srm.vendorapi;
using Sys.Serialization;
using com.ivp.rad.rscriptutils;
using com.ivp.rad.controls.rautocompletecontrolscripts;
namespace com.ivp.srm.controls.vendor.script
{
    public class VendorScript : Behavior, IClientMethods, IServerMethods
    {

        #region "Fields and Properties"
        private string _vendorHTML;
        private string _buttonSubmitID;
        private string _buttonCancelID;
        private ArrayList _listNodes;
        private string selectedVendorTypeValue = null;
        private string _typeOfControl;
        private bool _hasSavedValue;
        private Dictionary savedValues;
        private DOMElement errorDiv;
        private FTPType requestORresponse;
        private string taskName;
        private string taskDescription;

        private bool _replaceSubmitBtn;
        private bool _readOnlyDatasourceCtrl;
        private string _setDefaultVendorType;
        private bool _readOnlyVendorType;
        private ArrayList _disableKey;
        //private string hasCustomIdentifier;

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the vendor HTML.
        /// </summary>
        /// <value>The vendor HTML.</value>
        public string VendorHTML
        {
            get
            {
                return _vendorHTML;
            }
            set
            {
                _vendorHTML = value;
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the button submit ID.
        /// </summary>
        /// <value>The button submit ID.</value>
        public string ButtonSubmitID
        {
            get
            {
                return _buttonSubmitID;
            }
            set
            {
                _buttonSubmitID = value;
            }
        }

        public string ButtonCancelID
        {
            get
            {
                return _buttonCancelID;
            }
            set
            {
                _buttonCancelID = value;
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the type of control.
        /// </summary>
        /// <value>The type of control.</value>
        public string TypeOfControl
        {
            get
            {
                return _typeOfControl;
            }
            set
            {
                _typeOfControl = value;
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets a value indicating whether this instance has saved value of controls.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has saved value; otherwise, <c>false</c>.
        /// </value>
        public bool HasSavedValue
        {
            get
            {
                return _hasSavedValue;
            }
            set
            {
                _hasSavedValue = value;
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the has custom identifier.(for third party server side methods)
        /// </summary>
        /// <value>The has custom identifier.</value>
        //public string HasCustomIdentifier
        //{
        //    get
        //    {
        //        return hasCustomIdentifier;
        //    }
        //    set
        //    {
        //        hasCustomIdentifier = value;
        //    }
        //}

        private string _assemblyName;
        private string _moduleId;
        public string AssemblyName
        {
            get
            {
                return _assemblyName;
            }
            set
            {
                _assemblyName = value;
            }
        }

        private string _htmlLoc;
        public string HtmlLoc
        {
            get
            {
                return _htmlLoc;
            }
            set
            {
                _htmlLoc = value;
            }
        }

        public bool ReplaceSubmitBtn
        {
            get
            {
                return _replaceSubmitBtn;
            }
            set
            {
                _replaceSubmitBtn = value;
            }
        }


        public bool ReadOnlyDatasourceCtrl
        {
            get
            {
                return _readOnlyDatasourceCtrl;
            }
            set
            {
                _readOnlyDatasourceCtrl = value;
            }
        }


        public string SetDefaultVendorType
        {
            get
            {
                return _setDefaultVendorType;
            }
            set
            {
                _setDefaultVendorType = value;
            }
        }


        public bool ReadOnlyVendorType
        {
            get
            {
                return _readOnlyVendorType;
            }
            set
            {
                _readOnlyVendorType = value;
            }

        }

        public ArrayList DisableKey
        {
            get
            {
                return _disableKey;
            }
            set
            {
                _disableKey = value;
            }
        }

        public string ModuleId
        {
            get
            {
                return _moduleId;
            }
            set
            {
                _moduleId = value;
            }
        }

        #endregion


        //------------------------------------------------------------------------------------------
        /// <summary>
        /// creates a reference to the proxy class
        /// </summary>
        private VendorInterfaceAPI readdObject;

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Initializes a new instance of the <see cref="VendorScript"/> class.
        /// </summary>
        /// <param name="element">The <see cref="T:System.DHTML.DOMElement"/> object to associate with the behavior.</param>
        public VendorScript(DOMElement element)
            : base(element)
        {
            readdObject = new VendorInterfaceAPI();
            savedValues = new Dictionary();
            taskName = null;
            taskDescription = null;
            this.ID = Element.ID;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Has no built-in functionality; the Updated method is a placeholder for post-update logic
        /// in derived classes.
        /// </summary>
        protected override void Updated()
        {
            base.Updated();
            ReCreateControl();
            if (HasSavedValue == false)
            {
                DisplayControl(null);
            }
        }

        public override void Dispose()
        {
            savedValues = null;
            ManageHandlers.RemoveHandlers();
            base.Dispose();
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Recreates the control.
        /// </summary>
        private void ReCreateControl()
        {
            CreateControl();
            AttachEventsAndProperties();
            FillVendorType();
            if (HasSavedValue == true)
            {
                // for setting the default when ftp type is loaded
                requestORresponse = FTPType.Request;
            }
        }

        #region "Creating controls"

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates div to which vendor html is attached
        /// Creates error div
        /// </summary>
        private void CreateControl()
        {
            DivElement nameDiv = (DivElement)Document.GetElementById(Element.ID + "_parentDiv");
            if (nameDiv == null)
            {
                nameDiv = (DivElement)Document.CreateElement("DIV");
                nameDiv.ID = Element.ID + "_parentDiv";
                Element.AppendChild(nameDiv);
            }
            DOMElement submitButton = null;
            if (VendorCommons.IsNullEmpty(ButtonSubmitID) == false)
            {
                submitButton = Document.GetElementById(ButtonSubmitID);
            }

            nameDiv.InnerHTML = _vendorHTML;
            PlaceServerSubmitButton(submitButton);

            errorDiv = (DivElement)Document.GetElementById(Element.ID + "_errorDiv");
            if (errorDiv == null)
            {
                errorDiv = (DivElement)Document.CreateElement("DIV");
                errorDiv.ID = Element.ID + "_errorDiv";
                errorDiv.ClassName = "errorDiv";
                errorDiv.Style.Display = "none";
                Element.AppendChild(errorDiv);
            }
            if (TypeOfControl == "FTP")
            {
                if (requestORresponse == FTPType.Request)
                {
                    (Document.GetElementById(Element.ID + "_requestButton")).ClassName = "tabBtnVisited";
                    (Document.GetElementById(Element.ID + "_responseButton")).ClassName = "tabBtn";
                }
                else if (requestORresponse == FTPType.Response)
                {
                    (Document.GetElementById(Element.ID + "_requestButton")).ClassName = "tabBtn";
                    (Document.GetElementById(Element.ID + "_responseButton")).ClassName = "tabBtnVisited";
                }
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Places the server submit button.
        /// replacing the dummy sumbit button with the button provided by the user
        /// </summary>
        /// <param name="submitbutton">The submitbutton.</param>
        private void PlaceServerSubmitButton(DOMElement submitbutton)
        {
            if (submitbutton != null)
            {
                DOMElement dummyBtn = Document.GetElementById(Element.ID + "_dummy");
                submitbutton.ClassName = dummyBtn.ClassName;

                if (ReplaceSubmitBtn)
                {
                    dummyBtn.ParentNode.ReplaceChild(submitbutton, dummyBtn);
                }
                else
                    dummyBtn.Style.Display = "none";
            }
            if (VendorCommons.IsNullEmpty(ButtonCancelID) == false)
            {
                DOMElement cancelButton = VendorControls.GetElementByID(ButtonCancelID);
                if (cancelButton != null)
                {
                    DOMElement dummyCancelBtn = Document.GetElementById(Element.ID + "_Clear");
                    cancelButton.ClassName = dummyCancelBtn.ClassName;
                    dummyCancelBtn.ParentNode.
                        ReplaceChild(cancelButton,
                            dummyCancelBtn);
                }
            }
        }
        #endregion

        #region "Attach events and properties and managing style"

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Attaches the events to controls
        /// </summary>
        private void AttachEventsAndProperties()
        {
            _listNodes = VendorParser.GenericParser(VendorHTML).SelectNodes("//td[@ischildcontrol=\"true\"]/*");
            DOMAttributeCollection _attributeCollection = null;
            DOMAttribute _serverMethodName, _clientMethodName, _customClientMethod;
            DOMAttribute _serverEventType;
            DOMAttribute _id;
            ArrayList _clientMethodEventPair = null;
            ArrayList _customMethodEventPair = null;
            for (int i = 0; i < _listNodes.Length; i++)
            {
                _clientMethodEventPair = new ArrayList();
                _customMethodEventPair = new ArrayList();
                _attributeCollection = ((DOMElement)_listNodes[i]).Attributes;
                _serverMethodName = _attributeCollection.GetNamedItem(VendorAttributeConstants.ServerMethodName);
                _clientMethodName = _attributeCollection.GetNamedItem(VendorAttributeConstants.ClientMethodName);
                _serverEventType = _attributeCollection.GetNamedItem(VendorAttributeConstants.ServerEventType);
                _customClientMethod = _attributeCollection.GetNamedItem(VendorAttributeConstants.CustomClientMethod);

                _id = _attributeCollection.GetNamedItem(VendorAttributeConstants.Id);

                if (_id.Value == Element.ID + "_dummy" && VendorCommons.IsNullEmpty(ButtonSubmitID) == false)
                {
                    _id.Value = ButtonSubmitID;
                }
                if (_id.Value == Element.ID + "_Clear" && VendorCommons.IsNullEmpty(ButtonCancelID) == false)
                {
                    _id.Value = ButtonCancelID;
                }

                if (_clientMethodName != null && _clientMethodName.Value != "")
                    _clientMethodEventPair = VendorCommons.CreateEventMethod(_clientMethodName.Value);

                if (_customClientMethod != null && _customClientMethod.Value != "")
                    _customMethodEventPair = VendorCommons.CreateEventMethod(_customClientMethod.Value);

                if (_serverMethodName != null && VendorCommons.IsNullEmpty(_serverMethodName.Value) == false
                    && _serverEventType != null)
                {
                    ManageHandlers.AddHandler(_serverMethodName.Value, _serverEventType.Value, _id.Value, this);
                }

                if (_clientMethodEventPair.Length != 0)
                {
                    for (int j = 0; j < _clientMethodEventPair.Length; j++)
                    {
                        ManageHandlers.AddHandler(((MethodEventPair)_clientMethodEventPair[j]).MethodName,
                            ((MethodEventPair)_clientMethodEventPair[j]).EventName, _id.Value, this);
                    }
                }



                if (_customMethodEventPair.Length != 0)
                {
                    for (int j = 0; j < _customMethodEventPair.Length; j++)
                    {
                        DomEventHandler customclientmethodHandler = new DomEventHandler(CustomClientMethod);
                        DomEvent.AddHandler(Document.GetElementById(_id.Value), ((MethodEventPair)_customMethodEventPair[j]).EventName, customclientmethodHandler);
                    }
                }


            }
            ManageStyles();
            if (TypeOfControl == "FTP" && requestORresponse != FTPType.None)
            {
                if (taskName != null)
                {
                    ((TextElement)(GetControlByNameAndRequestType("Task Name", "Request"))).Value = taskName;
                    ((TextElement)(GetControlByNameAndRequestType("Task Name", "Response"))).Value = taskName;
                }

                if (taskDescription != null)
                {
                    ((TextElement)(GetControlByNameAndRequestType("Task Description", "Request"))).Value = taskDescription;
                    ((TextElement)(GetControlByNameAndRequestType("Task Description", "Response"))).Value = taskDescription;
                }
            }
            else
            {
                if (taskName != null)
                    ((TextElement)(GetControlByDisplayName("Task Name"))).Value = taskName;
                if (taskDescription != null)
                    ((TextElement)(GetControlByDisplayName("Task Description"))).Value = taskDescription;

            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Fills the type of the vendor.
        /// </summary>
        private void FillVendorType()
        {
            readdObject.GetVendorTypes(ModuleId, TypeOfControl, OnGetVendorTypeSuccess, OnFailure);
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Hides and shows the controls according to visible attribute
        /// Hides and shows the div according to request or response in case of FTP
        /// Invoke the server side methods
        /// Save the savedValues of control in dictionary
        /// </summary>
        private void ManageStyles()
        {
            DOMAttributeCollection attributeCollection = null;
            DOMAttribute _visible;
            DOMAttribute _id;
            DOMAttribute _name;
            DOMAttribute _savedValue;
            DOMAttribute contextType;
            DOMAttribute _serverMethodName;
            DOMAttribute _assembly;
            DOMAttribute _classname;
            DOMAttribute _namespace;
            DOMAttribute _serverMethodArgument;
            DOMAttribute _keyAttribute;
            DOMAttribute _isthirdPartyControl;
            DOMAttribute _autoCompleteControlID;
            string autoCompleteComponentId = "";

            DOMElement ele;
            LINQ DisableKeyLINQObj = new LINQ(DisableKey);
            ArrayList PipeSeperatedList = null;
            ArrayList SavedOptions = null;

            string key;
            for (int i = 0; i < _listNodes.Length; i++)
            {
                attributeCollection = ((DOMElement)_listNodes[i]).Attributes;
                _visible = attributeCollection.GetNamedItem(VendorAttributeConstants.Visible);
                _id = attributeCollection.GetNamedItem(VendorAttributeConstants.Id);
                _name = attributeCollection.GetNamedItem(VendorAttributeConstants.DisplayText);
                _savedValue = attributeCollection.GetNamedItem(VendorAttributeConstants.SavedValue);
                contextType = attributeCollection.GetNamedItem(VendorAttributeConstants.ContextType);
                _serverMethodName = attributeCollection.GetNamedItem(VendorAttributeConstants.ServerMethodName);
                _assembly = attributeCollection.GetNamedItem(VendorAttributeConstants.Assmebly);
                _classname = attributeCollection.GetNamedItem(VendorAttributeConstants.Classname);
                _namespace = attributeCollection.GetNamedItem(VendorAttributeConstants.Namespace);
                _serverMethodArgument = attributeCollection.GetNamedItem(VendorAttributeConstants.ServerMethodArguments);
                _keyAttribute = attributeCollection.GetNamedItem(VendorAttributeConstants.Key);
                _isthirdPartyControl = attributeCollection.GetNamedItem(VendorAttributeConstants.IsThirdPartyControl);
                _autoCompleteControlID = attributeCollection.GetNamedItem(VendorAttributeConstants.AutoCompleteControlID);

                if (_isthirdPartyControl != null && _autoCompleteControlID != null)
                {
                    if (bool.Parse(_isthirdPartyControl.Value) == true)
                    {
                        Document.GetElementById(_autoCompleteControlID.Value).Style.Display = "";
                        Document.GetElementById(_id.Value).AppendChild(Document.GetElementById(_autoCompleteControlID.Value));
                        if (_savedValue != null && _savedValue.Value.Trim() != "")
                        {

                            autoCompleteComponentId = Document.GetElementById(_autoCompleteControlID.Value).Children[0].ID.Trim().Substr(4);
                            RAutoCompleteControlScript autocomplete = (RAutoCompleteControlScript)Application.FindComponent(autoCompleteComponentId);
                            PipeSeperatedList = new ArrayList();
                            SavedOptions = new ArrayList();
                            ArrayList.AddRange(PipeSeperatedList, _savedValue.Value.Split('|'));
                            ArrayList.ForEach(PipeSeperatedList, delegate(object o)
                            {
                                string[] keyValue = o.ToString().Split('~');
                                OptionInfo info = new OptionInfo(keyValue[0], keyValue[1]);
                                ArrayList.Add(SavedOptions, info);
                            });
                            autocomplete.SetSelectedTokens(SavedOptions);
                        }
                    }
                }
                else
                {

                    if (_serverMethodName != null && _serverMethodName.Value.Trim() != "" && _visible.Value == "true")
                    {
                        InvokeServerMethods(_serverMethodName.Value.Trim(), _assembly == null ? null : _assembly.Value,
                            _classname == null ? null : _classname.Value, _id.Value, _namespace == null ? null : _namespace.Value, _serverMethodArgument == null ? null : _serverMethodArgument.Value);
                    }
                    if (_id.Value == Element.ID + "_dummy" && VendorCommons.IsNullEmpty(ButtonSubmitID) == false)
                    {
                        _id.Value = ButtonSubmitID;
                    }
                    if (_id.Value == Element.ID + "_Clear")
                    {
                        if (VendorCommons.IsNullEmpty(ButtonCancelID) == false)
                            _id.Value = ButtonCancelID;
                        if (DisableKey.Length > 0)
                        {
                            Document.GetElementById(_id.Value).Style.Display = "none";
                            _visible.Value = "false";
                        }
                    }

                    if (_name != null && _savedValue != null && _savedValue.Value != "" && _name.Value == "Vendor Type")
                    {
                        selectedVendorTypeValue = _savedValue.Value;
                    }
                    if (_savedValue != null && _savedValue.Value.Trim() != "")
                    {
                        if (_savedValue.Value.Trim() == "")
                            _savedValue.Value = "-1";

                        key = _name.Value;

                        if (TypeOfControl == "FTP")
                        {
                            if (contextType != null)
                            {
                                if (contextType.Value == "Request")
                                {
                                    key += "_Request";
                                }
                                else if (contextType.Value == "Response")
                                {
                                    key += "_Response";
                                }
                            }
                            requestORresponse = FTPType.Request;
                        }

                        savedValues[key] = _savedValue.Value;
                    }

                    ele = VendorControls.GetElementByID(_id.Value);

                    bool sendTrueFalse = (bool.Parse(_visible.Value)) ? false : true;
                    HideShowElementsAndItsTag(ele, sendTrueFalse);

                    if (_keyAttribute != null)
                    {
                        if (DisableKeyLINQObj.Where(delegate(object o, int index)
                            {
                                return o.ToString() == _keyAttribute.Value;
                            }).Count(null) > 0)
                        {
                            ele.ParentNode.Disabled = true;
                        }
                    }
                }
            }
            SetSavedValuesToControl();
            //for FTP
            if (TypeOfControl == "FTP")
            {
                switch (requestORresponse)
                {
                    case FTPType.Request:
                        VendorControls.GetElementByID(Element.ID + "_ResponseDiv").Style.Display = "none";
                        VendorControls.GetElementByID(Element.ID + "_RequestDiv").Style.Display = "";
                        break;
                    case FTPType.Response:
                        VendorControls.GetElementByID(Element.ID + "_ResponseDiv").Style.Display = "";
                        VendorControls.GetElementByID(Element.ID + "_RequestDiv").Style.Display = "none";
                        break;
                }

                SelectedFTPTransport();
                if (selectedVendorTypeValue == "Reuters")
                    SelectedOutgoingIncomingFTP();
            }
        }

        #endregion

        #region "Client Methods"

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Invokes the server methods.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="id">The id.</param>
        /// <param name="nameSpace">The name space.</param>
        /// <param name="serverMethodArguments">The server method arguments.</param>
        private void InvokeServerMethods(string methodName, string assemblyName,
            string className, string id, string nameSpace, string serverMethodArguments)
        {
            // calls the third party server methods
            if (assemblyName != null && assemblyName.Trim() != "")
            {
                BindApplicationSpecificData(className, methodName, assemblyName, id, nameSpace);
                return;
            }
            // calls the RAD server Methods
            switch (methodName)
            {
                case ServerMethodNames.RBbgInstrumentIdType:
                    RBbgInstrumentIdType();
                    break;
                case ServerMethodNames.RBbgMarketSector:
                    RBbgMarketSector();
                    break;
                case ServerMethodNames.RBbgRequestType:
                    RBbgRequestType();
                    break;
                // case ServerMethodNames.BindShuttleForIdentifiers:
                //   BindShuttleForIdentifiers();
                //  break;
                case ServerMethodNames.RReuterAssetTypes:
                    RReuterAssetTypes();
                    break;
                case ServerMethodNames.RReuterInstrumentIdType:
                    RReuterInstrumentIdType();
                    break;
                case ServerMethodNames.RReuterRequestType:
                    RReuterRequestType();
                    break;
                case ServerMethodNames.RWSOInstrumentIdType:
                    RWSOInstrumentIdType();
                    break;
                case ServerMethodNames.GetAllTransports:
                    if (TypeOfControl == "FTP")
                    {
                        IncomingOutgoingFTP_GetAllTransports();
                    }
                    else
                    {
                        GetAllTransports();
                    }
                    break;
                case ServerMethodNames.GetRequestType:

                    string[] parameters = serverMethodArguments.Split(',');
                    GetRequestType(int.ParseInvariant(parameters[0]), int.ParseInvariant(parameters[1]));
                    break;


            }
        }


        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Clears the control.
        /// </summary>
        /// <param name="e">The e.</param>
        public void ClearControl(DomEvent e)
        {
            if (ReadOnlyVendorType == false)
            PlaceAutoCompleteControlBack();

            DOMElementCollection inputCollection = null;
            inputCollection = Element.GetElementsByTagName("INPUT");
            for (int i = 0; i < inputCollection.Length; i++)
            {
                if (inputCollection[i].GetAttribute("type").ToString() == "text" ||
                    inputCollection[i].GetAttribute("type").ToString() == "password")
                {
                    ((TextElement)inputCollection[i]).Value = "";
                    ((TextElement)inputCollection[i]).Disabled = false;
                }
                if (inputCollection[i].GetAttribute("type").ToString() == "checkbox")
                {
                    ((CheckBoxElement)inputCollection[i]).Checked = false;
                    ((CheckBoxElement)inputCollection[i]).Disabled = false;
                }
            }
            inputCollection = Element.GetElementsByTagName("SELECT");
            for (int i = 0; i < inputCollection.Length; i++)
            {

                if (((SelectElement)inputCollection[i]).GetAttribute("key") != null && ((SelectElement)inputCollection[i]).GetAttribute("key").ToString().ToLowerCase() == "vendor" && ReadOnlyVendorType == true)
                    continue;

                if (((SelectElement)inputCollection[i]).GetAttribute("key") != null && ((SelectElement)inputCollection[i]).GetAttribute("key").ToString().ToLowerCase() == "datasource" && ReadOnlyDatasourceCtrl == true && ReadOnlyVendorType == true)
                    continue;

                ((SelectElement)inputCollection[i]).SelectedIndex = 0;
                ((SelectElement)inputCollection[i]).Disabled = false;
            }

            inputCollection = Element.GetElementsByTagName("LABEL");
            for (int i = 0; i < inputCollection.Length; i++)
            {
                inputCollection[i].InnerText = "";
            }
            inputCollection = Element.GetElementsByTagName("TEXTAREA");
            for (int i = 0; i < inputCollection.Length; i++)
            {
                if (inputCollection[i].GetAttribute("type").ToString() == "textarea")
                {
                    ((TextAreaElement)inputCollection[i]).Value = "";
                    ((TextAreaElement)inputCollection[i]).Disabled = false;
                }
            }

            
            HasSavedValue = false;
            selectedVendorTypeValue = "-1";
            //for clearing controls on clear

            if (ReadOnlyVendorType == false)
            switch (TypeOfControl)
            {
                case "API":
                    readdObject.ReturnHTML(Element.ID, TypeOfVendorControl.API, AssemblyName, HtmlLoc, //HasCustomIdentifier,
                         OnSuccessGetHTML, OnFailure);
                    break;

                case "REALTIMEPREFERENCE":
                    readdObject.ReturnHTML(Element.ID,
                           TypeOfVendorControl.REALTIMEPREFERENCE,
                           AssemblyName, HtmlLoc,
                        //HasCustomIdentifier,
                           OnSuccessGetHTML, OnFailure);
                    break;
                case "FTP":
                    requestORresponse = FTPType.None;
                    readdObject.ReturnHTML(Element.ID,
                        TypeOfVendorControl.FTP,
                        AssemblyName, HtmlLoc,
                        //HasCustomIdentifier,
                        OnSuccessGetHTML, OnFailure);
                    break;
            }

            //clearing the errormessages
            VendorCommons.ClearErrorHolder(errorDiv);
            taskName = null;
            taskDescription = null;

        }

       


        public void PlaceAutoCompleteControlBack()
        {
            for (int i = 0; i < _listNodes.Length; i++)
            {
                DOMAttributeCollection attributeCollection = ((DOMElement)_listNodes[i]).Attributes;
                DOMAttribute _id = attributeCollection.GetNamedItem(VendorAttributeConstants.Id);
                DOMAttribute _isthirdPartyControl = attributeCollection.GetNamedItem(VendorAttributeConstants.IsThirdPartyControl);
                DOMAttribute _autoCompleteControlID = attributeCollection.GetNamedItem(VendorAttributeConstants.AutoCompleteControlID);
                DOMElement parentDIV = Document.GetElementById(_id.Value);
                if (_isthirdPartyControl != null && _autoCompleteControlID != null)
                {
                    if (bool.Parse(_isthirdPartyControl.Value) == true)
                    {
                        string autoCompleteComponentId = Document.GetElementById(_autoCompleteControlID.Value).Children[0].ID.Trim().Substr(4);
                        RAutoCompleteControlScript autocomplete = (RAutoCompleteControlScript)Application.FindComponent(autoCompleteComponentId);
                        autocomplete.SetSelectedTokens(null);
                        if (parentDIV != null && parentDIV.Children.Length > 0)
                        {
                            Document.GetElementById(_autoCompleteControlID.Value).Style.Display = "none";
                            Document.Body.AppendChild(Document.GetElementById(_autoCompleteControlID.Value));
                        }
                    }
                }
            }

        }


        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Hides and shows transport based on selected value of selectelement.
        /// </summary>
        /// <param name="e">The e.</param>
        public void HideShowTransport(DomEvent e)
        {
            SelectElement selectElemnt = (SelectElement)e.Target;
            DOMElement ele = GetControlByDisplayName("Transport");
            if (selectElemnt.Value.ToUpperCase() == "FTP" || 
                selectElemnt.Value.ToUpperCase().IndexOf("FTP") >= 0)
            {
                HideShowElementsAndItsTag(ele, false);
            }
            else
            {
                HideShowElementsAndItsTag(ele, true);
            }
        }



        //for secmaster realtimepreference
        public void CustomClientMethod(DomEvent e)
        {
            DOMAttributeCollection attributeCollection = null;
            DOMAttribute _id;
            DOMAttribute _customClientMethodName;


            for (int i = 0; i < _listNodes.Length; i++)
            {
                attributeCollection = ((DOMElement)_listNodes[i]).Attributes;
                _id = attributeCollection.GetNamedItem(VendorAttributeConstants.Id);
                _customClientMethodName = attributeCollection.GetNamedItem(VendorAttributeConstants.CustomClientMethod);
                if (e.Target.ID == _id.Value)
                {
                    Script.Eval((_customClientMethodName.Value.Split('~'))[0] + ";");
                    break;
                }
            }
        }


        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Displays the control according to vendor type
        /// </summary>
        /// <param name="e">The e.</param>
        public void DisplayControl(DomEvent e)
        {
            PlaceAutoCompleteControlBack();
            if (e == null)
            {
                if (SetDefaultVendorType != null)
                {
                    string firstLetter = SetDefaultVendorType.CharAt(0).ToString().ToUpperCase();
                    string RestOfTheword = SetDefaultVendorType.Substr(1).ToLowerCase();
                    selectedVendorTypeValue = firstLetter + RestOfTheword;

                }
            }
            else
                selectedVendorTypeValue = ((SelectElement)e.Target).Value;

            if (selectedVendorTypeValue != null)
            {
                savedValues = null;
                ManageHandlers.RemoveHandlers();
                switch (selectedVendorTypeValue)
                {
                    //Bloomberg 
                    #region "CASE "Bloomberg":"
                    case "Bloomberg":
                        if (TypeOfControl == "API")
                        {
                            readdObject.ReturnHTML(Element.ID,
                                TypeOfVendorControl.API_Bloomberg,
                                AssemblyName, HtmlLoc,
                                //HasCustomIdentifier,
                                OnSuccessGetHTML, OnFailure);

                        }
                        else if (TypeOfControl == "REALTIMEPREFERENCE")
                        {
                            readdObject.ReturnHTML(Element.ID,
                                TypeOfVendorControl.REALTIMEPREFERENCE_Bloomberg,
                                AssemblyName, HtmlLoc,
                                //HasCustomIdentifier,
                                OnSuccessGetHTML, OnFailure);

                        }
                        else if (TypeOfControl == "FTP")
                        {
                            if (requestORresponse == FTPType.None)
                            {
                                requestORresponse = FTPType.Request;
                            }
                            readdObject.ReturnHTML(Element.ID,
                                TypeOfVendorControl.FTP_Bloomberg,
                                AssemblyName, HtmlLoc,
                                //HasCustomIdentifier,
                                OnSuccessGetHTML, OnFailure);
                        }

                        break;
                    #endregion

                    //Reuters
                    #region "CASE "Reuters":"
                    case "Reuters":
                        if (TypeOfControl == "API")
                        {
                            readdObject.ReturnHTML(Element.ID,
                                TypeOfVendorControl.API_Reuters,
                                AssemblyName, HtmlLoc,
                                //HasCustomIdentifier,
                                OnSuccessGetHTML, OnFailure);

                        }
                        else if (TypeOfControl == "REALTIMEPREFERENCE")
                        {
                            readdObject.ReturnHTML(Element.ID,
                                TypeOfVendorControl.REALTIMEPREFERENCE_Reuters,
                                AssemblyName, HtmlLoc,
                                //HasCustomIdentifier,
                                OnSuccessGetHTML, OnFailure);

                        }
                        else if (TypeOfControl == "FTP")
                        {
                            if (requestORresponse == FTPType.None)
                            {
                                requestORresponse = FTPType.Request;
                            }
                            readdObject.ReturnHTML(Element.ID,
                                TypeOfVendorControl.FTP_Reuters,
                                AssemblyName, HtmlLoc,
                                //HasCustomIdentifier,
                                OnSuccessGetHTML, OnFailure);
                        }

                        break;
                    #endregion

                    //Markit WSO
                    #region "CASE "Markit WSO":"
                    case "MarkitWSO":
                        if (TypeOfControl == "API")
                        {
                            readdObject.ReturnHTML(Element.ID,
                                TypeOfVendorControl.API_MarkitWSO,
                                AssemblyName, HtmlLoc,
                                //HasCustomIdentifier,
                                OnSuccessGetHTML, OnFailure);

                        }
                        break;
                    #endregion

                    //Default
                    #region "Default"
                    default:
                        if (TypeOfControl == "API")
                        {
                            readdObject.ReturnHTML(Element.ID,
                                TypeOfVendorControl.API,
                                AssemblyName, HtmlLoc,
                                //HasCustomIdentifier,
                                OnSuccessGetHTML, OnFailure);

                        }
                        else if (TypeOfControl == "REALTIMEPREFERENCE")
                        {
                            readdObject.ReturnHTML(Element.ID,
                                TypeOfVendorControl.REALTIMEPREFERENCE,
                                //HasCustomIdentifier,
                                AssemblyName, HtmlLoc,
                                OnSuccessGetHTML, OnFailure);

                        }
                        else if (TypeOfControl == "FTP")
                        {
                            requestORresponse = FTPType.None;
                            readdObject.ReturnHTML(Element.ID,
                                TypeOfVendorControl.FTP,
                                AssemblyName, HtmlLoc,
                                //HasCustomIdentifier,
                                OnSuccessGetHTML, OnFailure);
                        }

                        break;
                    #endregion
                }
            }
            if (e != null)
            {
                taskName = ((TextElement)(GetControlByDisplayName("Task Name"))).Value;
                taskDescription = ((TextElement)(GetControlByDisplayName("Task Description"))).Value;
            }
        }


        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the output XML.
        /// </summary>
        /// <param name="e">The e.</param>
        public void GetOutputXML(DomEvent e)
        {
            bool returnValue = CheckMandatoryFields();
            if (returnValue == false)
            {
                e.PreventDefault();
                e.RawEvent.ReturnValue = false;
            }
            else
            {
                SetOutputXmlToHiddenField();
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Converts the html into required output XML
        /// Sets the output XML to hidden field.
        /// </summary>
        private void SetOutputXmlToHiddenField()
        {
            DOMAttribute displayText;
            DOMAttribute id;
            DOMAttributeCollection _attributeCollection;
            DOMAttribute key;
            DOMElement element;
            DOMAttribute contextType;
            DOMAttribute visible;
            DOMAttribute vendorpropertyname;
            DOMAttribute _isthirdPartyControl;
            DOMAttribute _autoCompleteControlID;
            string autoCompleteComponentId = "";
            bool _isset = false;

            StringBuilder _outputXml = new StringBuilder();
            _outputXml.Append("&lt;");
            _outputXml.Append(TypeOfControl);
            _outputXml.Append("&gt;");

            if (TypeOfControl == "FTP")
            {
                _outputXml.Append("&lt;REQUEST&gt;");
            }

            for (int i = 0; i < _listNodes.Length; i++)
            {
                _attributeCollection = ((DOMElement)_listNodes[i]).Attributes;
                id = _attributeCollection.GetNamedItem(VendorAttributeConstants.Id);
                displayText = _attributeCollection.GetNamedItem(VendorAttributeConstants.DisplayText);
                key = _attributeCollection.GetNamedItem(VendorAttributeConstants.Key);
                contextType = _attributeCollection.GetNamedItem(VendorAttributeConstants.ContextType);
                visible = _attributeCollection.GetNamedItem(VendorAttributeConstants.Visible);
                vendorpropertyname = _attributeCollection.GetNamedItem(VendorAttributeConstants.VendorPropertyName);
                _isthirdPartyControl = _attributeCollection.GetNamedItem(VendorAttributeConstants.IsThirdPartyControl);
                _autoCompleteControlID = _attributeCollection.GetNamedItem(VendorAttributeConstants.AutoCompleteControlID);

                if (id.Value == Element.ID + "_dummy" && VendorCommons.IsNullEmpty(ButtonSubmitID) == false)
                {
                    id.Value = ButtonSubmitID;
                }

                if (id.Value == Element.ID + "_Clear" && VendorCommons.IsNullEmpty(ButtonCancelID) == false)
                {
                    id.Value = ButtonCancelID;
                }
                element = VendorControls.GetElementByID(id.Value);
                if (visible.Value == "true" && element.Style.Display != "none")
                {
                    if (TypeOfControl == "FTP" && contextType != null && contextType.Value == "Response" && _isset == false)
                    {
                        _outputXml.Append("&lt;/REQUEST&gt;");
                        _outputXml.Append("&lt;RESPONSE&gt;");
                        _isset = true;
                    }

                    switch (element.TagName.ToUpperCase())
                    {
                        case "INPUT":
                            TextElement t_ele = (TextElement)element;
                            if (t_ele.Type.ToUpperCase() == "TEXT")
                            {
                                _outputXml.Append("&lt;control text=\"");
                                _outputXml.Append(t_ele.Value.Trim());
                                _outputXml.Append("\" value=\"");
                                _outputXml.Append(t_ele.Value.Trim());
                                _outputXml.Append("\" key=\"");
                                _outputXml.Append(key.Value);
                                _outputXml.Append("\" name=\"");
                                _outputXml.Append(displayText.Value);
                                _outputXml.Append("\" vendorpropertyname=\"");
                                _outputXml.Append(vendorpropertyname.Value);
                                _outputXml.Append("\" /&gt;");
                            }
                            break;
                        case "SELECT":
                            SelectElement s_ele = (SelectElement)element;

                            _outputXml.Append("&lt;control text=\"");
                            _outputXml.Append(((OptionElement)s_ele.Options[s_ele.SelectedIndex]).Text);
                            _outputXml.Append("\" value=\"");
                            _outputXml.Append(s_ele.Value);
                            _outputXml.Append("\" key=\"");
                            _outputXml.Append(key.Value);
                            _outputXml.Append("\" name=\"");
                            _outputXml.Append(displayText.Value);
                            _outputXml.Append("\" vendorpropertyname=\"");
                            _outputXml.Append(vendorpropertyname.Value);
                            _outputXml.Append("\" /&gt;");
                            break;
                        case "DIV":
                            if (_autoCompleteControlID != null && _isthirdPartyControl != null)
                            {
                                if (bool.Parse(_isthirdPartyControl.Value) == true)
                                {
                                    autoCompleteComponentId = Document.GetElementById(_autoCompleteControlID.Value).Children[0].ID.Trim().Substr(4); 
                                     RAutoCompleteControlScript autocomplete = (RAutoCompleteControlScript)Application.FindComponent(autoCompleteComponentId);
                                    _outputXml.Append("&lt;control text=\"");
                                    
                                    ArrayList list = autocomplete.GetSelectedTokens();
                                    OptionInfo options = null;
                                    StringBuilder SB = new StringBuilder();
                                    ArrayList.ForEach(list, delegate(object o)
                                        {
                                            options = (OptionInfo)o;
                                            SB.Append(options.Key);
                                            SB.Append("~");
                                            SB.Append(options.Value);
                                            SB.Append("|");
                                        });
                                    if (!SB.IsEmpty())
                                        _outputXml.Append(SB.ToString().Substring(0,SB.ToString().Length-1));
                                    else
                                        _outputXml.Append("");

                                    _outputXml.Append("\" value=\"");
                                    if (!SB.IsEmpty())
                                        _outputXml.Append(SB.ToString().Substring(0, SB.ToString().Length - 1));
                                    else
                                        _outputXml.Append("");

                                    _outputXml.Append("\" key=\"");
                                    _outputXml.Append(key.Value);
                                    _outputXml.Append("\" name=\"");
                                    _outputXml.Append(displayText.Value);
                                    _outputXml.Append("\" vendorpropertyname=\"");
                                    _outputXml.Append(vendorpropertyname.Value);
                                    _outputXml.Append("\" /&gt;");
                                }
                            }
                            break;
                    }
                }
            }

            if (TypeOfControl == "FTP")
            {
                _outputXml.Append("&lt;/RESPONSE&gt;");
            }
            _outputXml.Append("&lt;/");
            _outputXml.Append(TypeOfControl);
            _outputXml.Append("&gt;");

            ((TextElement)VendorControls.GetElementByID(Element.ID + "_HdnErrorMessage")).Value =
                _outputXml.ToString();
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Handles request button click
        /// </summary>
        /// <param name="e">The e.</param>
        public void RequestClicked(DomEvent e)
        {
            (Document.GetElementById(Element.ID + "_requestButton")).ClassName = "tabBtnVisited";
            (Document.GetElementById(Element.ID + "_responseButton")).ClassName = "tabBtn";


            requestORresponse = FTPType.Request;
            SelectElement s = (SelectElement)GetControlByDisplayName("Vendor Type");
            if (s != null && s.SelectedIndex != 0)
            {
                FillVendorType();
                VendorControls.GetElementByID(Element.ID + "_ResponseDiv").Style.Display = "none";
                VendorControls.GetElementByID(Element.ID + "_RequestDiv").Style.Display = "";
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Handles response button click
        /// </summary>
        /// <param name="e">The e.</param>
        public void ResponseClicked(DomEvent e)
        {
            (Document.GetElementById(Element.ID + "_requestButton")).ClassName = "tabBtn";
            (Document.GetElementById(Element.ID + "_responseButton")).ClassName = "tabBtnVisited";
            requestORresponse = FTPType.Response;
            SelectElement s = (SelectElement)GetControlByDisplayName("Vendor Type");
            if (s != null && s.SelectedIndex != 0)
            {
                FillVendorType();
                VendorControls.GetElementByID(Element.ID + "_ResponseDiv").Style.Display = "";
                VendorControls.GetElementByID(Element.ID + "_RequestDiv").Style.Display = "none";
            }
        }

        #endregion

        #region "WebService Region"

        //------------------------------------------------------------------------------------------
        ///// <summary>
        ///// Binds the shuttle for identifiers.
        ///// </summary>
        //public void BindShuttleForIdentifiers()
        //{
        //    readdObject.BindShuttleForIdentifiers(OnBindShuttleForIdentifiers, OnFailure);
        //}



        //public void BindDataSourceForRealTimePreferenceFunction(DomEvent e)
        //{
        //    readdObject.GetBindDataSourceForRealTimePreference(OnSuccess_DataSource_BindDataSourceForRealTimePreference, OnFailure);
        //}


        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Incomings the outgoing FT p_ get all transports.
        /// </summary>
        public void IncomingOutgoingFTP_GetAllTransports()
        {
            readdObject.GetGetAllTransports(OnSuccess_IncomingOutgoingFTP_GetAllTransports, OnFailure);
        }


        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Called when [success_ API type_ request type].
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="args">The args.</param>
        private void OnSuccess_APIType_RequestType(object result, object args)
        {
            ArrayList listResult = (ArrayList)result;
            VendorCommons.BindDropDown(GetControlByDisplayName("API Type"), listResult, null);
            if (HasSavedValue)
            {
                ((SelectElement)GetControlByDisplayName("API Type")).Value = savedValues["API Type"].ToString();
                if (TypeOfControl == "REALTIMEPREFERENCE" && savedValues["API Type"].ToString().ToUpperCase() == "FTP")
                    HideShowElementsAndItsTag(GetControlByDisplayName("Transport"), false);
                else if (TypeOfControl == "REALTIMEPREFERENCE" && savedValues["API Type"].ToString().ToUpperCase() != "FTP")
                    HideShowElementsAndItsTag(GetControlByDisplayName("Transport"), true);
            }
        }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Called when [success_ vendor identifier_ instrument id type].
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="args">The args.</param>
        private void OnSuccess_VendorIdentifier_InstrumentIdType(object result, object args)
        {
            ArrayList listResult = (ArrayList)result;
            VendorCommons.BindDropDown(GetControlByDisplayName("Vendor Identifier"), listResult, null);
            if (HasSavedValue)
            {
                string key = "Vendor Identifier";
                if (TypeOfControl == "FTP")
                {
                    key += "_Request";
                }
                ((SelectElement)GetControlByDisplayName("Vendor Identifier")).Value =
                    savedValues[key].ToString();
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Called when [success_ market sector].
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="args">The args.</param>
        private void OnSuccess_MarketSector(object result, object args)
        {
            ArrayList listResult = (ArrayList)result;
            VendorCommons.BindDropDown(GetControlByDisplayName("Market Sector"), listResult, null);
            if (HasSavedValue)
            {
                string key = "Market Sector";
                if (TypeOfControl == "FTP")
                {
                    key += "_Request";
                }
                ((SelectElement)GetControlByDisplayName("Market Sector")).Value =
                    savedValues[key].ToString();
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Called when [get vendor type success].
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="args">The args.</param>
        private void OnGetVendorTypeSuccess(object result, object args)
        {
            ArrayList listResult = (ArrayList)result;
            if (TypeOfControl == "FTP" && requestORresponse != FTPType.None)
            {
                BindSelectedVendorType(listResult, GetControlByNameAndRequestType("Vendor Type", "Request"));
                BindSelectedVendorType(listResult, GetControlByNameAndRequestType("Vendor Type", "Response"));
            }
            else
            {
                BindSelectedVendorType(listResult, GetControlByDisplayName("Vendor Type"));
            }
            if ((ReadOnlyVendorType.ToString() != null))
                GetControlByDisplayName("Vendor Type").Disabled = ReadOnlyVendorType;
        }

        private void BindSelectedVendorType(ArrayList listResult, DOMElement element)
        {
            VendorCommons.BindDropDown(element, listResult, null);
            if (VendorCommons.IsNullEmpty(selectedVendorTypeValue) == false)
            {
                SelectElement ele = (SelectElement)element;
                OptionElement option = null;
                for (int i = 0; i < ele.Options.Length; i++)
                {
                    option = (OptionElement)ele.Options[i];
                    if (option.Value.ToLowerCase() == selectedVendorTypeValue.ToLowerCase())
                    {
                        option.Selected = true;
                        break;
                    }
                }
            }


        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Called when [success_ asset types].
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="args">The args.</param>
        private void OnSuccess_AssetTypes(object result, object args)
        {
            ArrayList listResult = (ArrayList)result;
            VendorCommons.BindDropDown(GetControlByDisplayName("Asset Type"), listResult, null);
            if (HasSavedValue)
            {
                string key = "Asset Type";
                if (TypeOfControl == "FTP")
                {
                    key += "_Request";
                }
                ((SelectElement)GetControlByDisplayName("Asset Type")).Value =
                    savedValues[key].ToString();
            }
        }


        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Called when [success_ transport_ get all transports].
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="args">The args.</param>
        private void OnSuccess_Transport_GetAllTransports(object result, object args)
        {
            string s1 = result.ToString();
            XMLDOM _dataSetDOM = VendorParser.GenericParser(s1);

            SelectElement s = (SelectElement)GetControlByDisplayName("Transport");
            ArrayList _texts = _dataSetDOM.SelectNodes("//" +
                s.Attributes.GetNamedItem(VendorAttributeConstants.DataTextField).Value);
            ArrayList _values = _dataSetDOM.SelectNodes("//" +
                s.Attributes.GetNamedItem(VendorAttributeConstants.DataValueField).Value);
            VendorCommons.BindDropDown(s, _texts, _values);
            if (HasSavedValue)
            {
                if (savedValues["Transport"] != null)
                    ((SelectElement)GetControlByDisplayName("Transport")).Value =
                        savedValues["Transport"].ToString();
            }
        }



        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Called when [success_ incoming outgoing FT p_ get all transports].
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="args">The args.</param>
        private void OnSuccess_IncomingOutgoingFTP_GetAllTransports(object result, object args)
        {
            string s1 = result.ToString();
            XMLDOM _dataSetDOM = VendorParser.GenericParser(s1);
            SelectElement s = (SelectElement)GetControlByDisplayName("Outgoing FTP");
            ArrayList _texts = _dataSetDOM.SelectNodes("//" + s.Attributes.GetNamedItem(VendorAttributeConstants.DataTextField).Value);
            ArrayList _values = _dataSetDOM.SelectNodes("//" + s.Attributes.GetNamedItem(VendorAttributeConstants.DataValueField).Value);

            VendorCommons.BindDropDown(s, _texts, _values);
            string key;
            if (HasSavedValue)
            {
                key = "Outgoing FTP";
                if (TypeOfControl == "FTP")
                {
                    key += "_Request";
                }
                ((SelectElement)GetControlByDisplayName("Outgoing FTP")).Value =
                    savedValues[key].ToString();
            }

            s = (SelectElement)GetControlByDisplayName("Incoming FTP");
            VendorCommons.BindDropDown(s, _texts, _values);
            if (HasSavedValue)
            {
                key = "Incoming FTP";
                if (TypeOfControl == "FTP")
                {
                    key += "_Response";
                }
                ((SelectElement)GetControlByDisplayName("Incoming FTP")).Value =
                    savedValues[key].ToString();
            }
        }


        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Called when [success get HTML]. Gets the html page according to vendor type
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="args">The args.</param>
        private void OnSuccessGetHTML(object result, object args)
        {
            //string taskNameRequest = null;
            //string taskDescriptionRequest = null;
            //string taskNameResponse = null;
            //string taskDescriptionResponse = null;
            //string taskName = null;
            //string taskDescription = null;
            if (GetControlByDisplayName("Task Name") != null)
            {
                if (VendorCommons.IsNullEmpty(ButtonSubmitID) == false)
                    ManageHandlers.RemoveHandler(ButtonSubmitID);
                if (VendorCommons.IsNullEmpty(ButtonCancelID) == false)
                    ManageHandlers.RemoveHandler(ButtonCancelID);
                VendorHTML = result.ToString();

                HasSavedValue = false;
                ReCreateControl();
            }
        }


        /// <summary>
        /// Called when [success get request type].
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="args">The args.</param>
        private void OnSuccessGetRequestType(object result, object args)
        {
            ArrayList listResult = (ArrayList)result;
            VendorCommons.BindDropDown(GetControlByDisplayName("API Type"), listResult, null);
            if (HasSavedValue)
            {
                ((SelectElement)GetControlByDisplayName("API Type")).Value = savedValues["API Type"].ToString();
                if (TypeOfControl == "REALTIMEPREFERENCE" && savedValues["API Type"].ToString().ToUpperCase() == "FTP")
                    HideShowElementsAndItsTag(GetControlByDisplayName("Transport"), false);
                else if (TypeOfControl == "REALTIMEPREFERENCE" && savedValues["API Type"].ToString().ToUpperCase() != "FTP")
                    HideShowElementsAndItsTag(GetControlByDisplayName("Transport"), true);
            }

        }



        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Called when [success bind application specific data].
        /// Binds the data for third party server methods
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="args">The args.</param>
        private void OnSuccessBindApplicationSpecificData(object result, object args)
        {
            ApplicationSpecificData data = (ApplicationSpecificData)JavaScriptSerializer.Deserialize(result.ToString());

            if (data.DataType == DataType.DataSet)
            {
                XMLDOM _dataSetDOM = VendorParser.GenericParser(data.ReturnData.ToString());
                SelectElement s = (SelectElement)VendorControls.GetElementByID(data.ControlId);
                ArrayList _texts = _dataSetDOM.SelectNodes("//" + s.Attributes.GetNamedItem(VendorAttributeConstants.DataTextField).Value);
                ArrayList _values = _dataSetDOM.SelectNodes("//" + s.Attributes.GetNamedItem(VendorAttributeConstants.DataValueField).Value);

                VendorCommons.BindDropDown(s, _texts, _values);
                if (HasSavedValue)
                {
                    string key = s.Attributes.GetNamedItem(VendorAttributeConstants.DisplayText).Value;
                    if (TypeOfControl == "FTP")
                    {
                        key += "_Request";
                    }
                    if (savedValues[key] != null)
                        s.Value = savedValues[key].ToString();

                }

                //disable Datasource control
                if (s.Attributes.GetNamedItem(VendorAttributeConstants.DisplayText).Value == "DataSource")
                {
                    if (TypeOfControl == "REALTIMEPREFERENCE")
                        ((SelectElement)s).Disabled = ReadOnlyDatasourceCtrl;
                }
            }
        }





        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Called when [failure].
        /// </summary>
        /// <param name="args">The args.</param>
        /// 
        private void OnFailure(object args)
        {
            RADWebServiceException exception = (RADWebServiceException)args;
            Script.Alert(exception.Get_message());
        }


        #endregion

        #region "Helper Methods"

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// sets the selected value for Transport type if the control has saved values
        /// </summary>
        private void SelectedFTPTransport()
        {
            if (HasSavedValue)
            {
                ((SelectElement)GetControlByNameAndRequestType("Transport Type", "Request")).Value = savedValues["Transport Type_Request"].ToString();
                ((SelectElement)GetControlByNameAndRequestType("Transport Type", "Response")).Value = savedValues["Transport Type_Response"].ToString();
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// sets the selected value for Outgoing and Incoming FTP if the control has saved values
        /// </summary>
        private void SelectedOutgoingIncomingFTP()
        {
            if (HasSavedValue)
            {
                ((SelectElement)GetControlByDisplayName("Outgoing FTP")).Value = savedValues["Outgoing FTP_Request"].ToString();
                ((SelectElement)GetControlByDisplayName("Incoming FTP")).Value = savedValues["Incoming FTP_Response"].ToString();
            }
        }


        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Sets the saved values to Task Name and Task Description
        /// </summary>
        private void SetSavedValuesToControl()
        {
            if (HasSavedValue == true)
            {
                if (TypeOfControl == "FTP")
                {
                    ((TextElement)GetControlByNameAndRequestType("Task Name", "Request")).Value = savedValues["Task Name_Request"].ToString();
                    if (savedValues["Task Description_Request"] != null && savedValues["Task Description_Request"].ToString() != "-1")
                        ((TextElement)GetControlByNameAndRequestType("Task Description", "Request")).Value = savedValues["Task Description_Request"].ToString();

                    ((TextElement)GetControlByNameAndRequestType("Task Name", "Response")).Value = savedValues["Task Name_Response"].ToString();
                    if (savedValues["Task Description_Response"] != null && savedValues["Task Description_Response"].ToString() != "-1")
                        ((TextElement)GetControlByNameAndRequestType("Task Description", "Response")).Value = savedValues["Task Description_Response"].ToString();

                    // binding Request Type
                    if (GetControlByNameAndRequestType("Data Request Type", "Request") != null && savedValues["Data Request Type_Request"] != null)
                        ((SelectElement)GetControlByNameAndRequestType("Data Request Type", "Request")).Value = savedValues["Data Request Type_Request"].ToString();

                }
                else
                {
                    ((TextElement)GetControlByDisplayName("Task Name")).Value = savedValues["Task Name"].ToString();
                    if (savedValues["Task Description"] != null && savedValues["Task Description"].ToString() != "-1")
                        ((TextElement)GetControlByDisplayName("Task Description")).Value = savedValues["Task Description"].ToString();
                }
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Checks the mandatory fields and displays error in mandatory fields.
        /// </summary>
        /// <returns></returns>
        private bool CheckMandatoryFields()
        {
            DOMAttributeCollection attributeCollection = null;
            DOMAttribute is_mandatory;
            DOMAttribute is_visible;
            DOMAttribute display_text;
            string contexttype;
            DOMAttribute contextTypeAttrib;
            DOMElement ele;
            DOMAttribute _id;
            bool isValid = true;
            DOMAttribute _isthirdPartyControl;
            DOMAttribute _autoCompleteControlID;

            string autoCompleteComponentId;

            DOMElement errorHolder = VendorCommons.CreateErrorMessageHolder(errorDiv);

            for (int i = 0; i < _listNodes.Length; i++)
            {
                attributeCollection = ((DOMElement)_listNodes[i]).Attributes;
                is_mandatory = attributeCollection.GetNamedItem(VendorAttributeConstants.IsMandatory);
                is_visible = attributeCollection.GetNamedItem(VendorAttributeConstants.Visible);
                display_text = attributeCollection.GetNamedItem(VendorAttributeConstants.DisplayText);
                _id = attributeCollection.GetNamedItem(VendorAttributeConstants.Id);
                contexttype = (contextTypeAttrib = attributeCollection.GetNamedItem(VendorAttributeConstants.ContextType)) == null ? "" :" in " + contextTypeAttrib.Value + " Section.";
                _isthirdPartyControl = attributeCollection.GetNamedItem(VendorAttributeConstants.IsThirdPartyControl);
                _autoCompleteControlID = attributeCollection.GetNamedItem(VendorAttributeConstants.AutoCompleteControlID);

                if (_id.Value == Element.ID + "_dummy" && VendorCommons.IsNullEmpty(ButtonSubmitID) == false)
                {
                    _id.Value = ButtonSubmitID;
                }

                if (_id.Value == Element.ID + "_Clear" && VendorCommons.IsNullEmpty(ButtonCancelID) == false)
                {
                    _id.Value = ButtonCancelID;
                }

                ele = VendorControls.GetElementByID(_id.Value);

                if (ele.Style.Display != "none"
                    && is_mandatory != null
                    && bool.Parse(is_mandatory.Value))
                {
                    switch (ele.TagName.ToUpperCase())
                    {
                        case "SELECT":
                            if (((SelectElement)ele).SelectedIndex == 0)
                            {
                                VendorCommons.CreateErrorMessage(errorHolder, display_text.Value +
                                    " cannot be left unselected "+contexttype);
                                isValid = false;
                            }
                            break;
                        case "INPUT":
                            if (((InputElement)ele).Type.ToUpperCase() == "TEXT" && ((TextElement)ele).Value.Trim() == "")
                            {
                                VendorCommons.CreateErrorMessage(errorHolder, display_text.Value +
                                    " cannot be left blank "+contexttype);
                                isValid = false;
                            }
                            break;
                        case "DIV":
                            if (_autoCompleteControlID != null && _isthirdPartyControl != null)
                            {
                                if (bool.Parse(_isthirdPartyControl.Value) == true)
                                {
                                    autoCompleteComponentId = Document.GetElementById(_autoCompleteControlID.Value).Children[0].ID.Trim().Substr(4);
                                    RAutoCompleteControlScript autocomplete = (RAutoCompleteControlScript)Application.FindComponent(autoCompleteComponentId);
                                    ArrayList list = autocomplete.GetSelectedTokens();
                                    if (list.Length == 0)
                                    {
                                        VendorCommons.CreateErrorMessage(errorHolder, display_text.Value +
                                    " cannot be left blank " + contexttype);
                                        isValid = false;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            return isValid;
        }



        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the control by Display Name
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <returns></returns>
        private DOMElement GetControlByDisplayName(string displayName)
        {
            DOMAttributeCollection attributeCollection;
            DOMElement element = null;
            for (int i = 0; i < _listNodes.Length; i++)
            {
                attributeCollection = ((DOMElement)_listNodes[i]).Attributes;
                if (attributeCollection.GetNamedItem(VendorAttributeConstants.DisplayText) != null &&
                    attributeCollection.GetNamedItem(VendorAttributeConstants.DisplayText).Value == displayName)
                {
                    element = VendorControls.GetElementByID(attributeCollection.GetNamedItem(VendorAttributeConstants.Id).Value);
                    return element;
                }
            }
            return null;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Hides and shows elements and its tag.
        /// </summary>
        /// <param name="_element">The _element.</param>
        /// <param name="hide">if set to <c>true</c> [hide].</param>
        private void HideShowElementsAndItsTag(DOMElement _element, bool hide)
        {
            if (_element != null)
            {
                string _display = hide ? "none" : "";
                if (((InputElement)_element).Type.ToUpperCase() == "BUTTON" ||
                    ((InputElement)_element).Type.ToUpperCase() == "SUBMIT")
                {
                    _element.Style.Display = _display;
                    return;
                }
                _element.Style.Display = _display;
                _element.ParentNode.Style.Display = _display;
                DOMElement prevControl = GetPrevControl(_element.ParentNode);
                prevControl.Style.Display = _display;
            }
        }

        private DOMElement GetPrevControl(DOMElement dOMElement)
        {
            if (dOMElement.PreviousSibling.NodeName == "#text")
                return GetPrevControl(dOMElement.PreviousSibling);
            else
                return dOMElement.PreviousSibling;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets control by name and context type (request/response).
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="contextType">Type of the context.</param>
        /// <returns></returns>
        private DOMElement GetControlByNameAndRequestType(string displayName, string contextType)
        {
            DOMAttributeCollection attributeCollection;
            DOMElement element = null;
            for (int i = 0; i < _listNodes.Length; i++)
            {
                attributeCollection = ((DOMElement)_listNodes[i]).Attributes;
                if (attributeCollection.GetNamedItem(VendorAttributeConstants.DisplayText) != null &&
                    attributeCollection.GetNamedItem(VendorAttributeConstants.DisplayText).Value == displayName &&
                    attributeCollection.GetNamedItem(VendorAttributeConstants.ContextType) != null &&
                    attributeCollection.GetNamedItem(VendorAttributeConstants.ContextType).Value == contextType)
                {
                    element = VendorControls.GetElementByID(attributeCollection.GetNamedItem(VendorAttributeConstants.Id).Value);
                    return element;
                }
            }
            return null;
        }





        #endregion

        #region IServerMethods Members


        public void RBbgRequestType()
        {
            readdObject.GetBbgRequestType(OnSuccess_APIType_RequestType, OnFailure);
        }

        public void RBbgInstrumentIdType()
        {
            readdObject.GetBbgInstrumentIdType(OnSuccess_VendorIdentifier_InstrumentIdType, OnFailure);
        }

        public void RBbgMarketSector()
        {
            readdObject.GetBbgMarketSector(OnSuccess_MarketSector, OnFailure);
        }

        public void RReuterRequestType()
        {
            readdObject.GetRReuterRequestType(OnSuccess_APIType_RequestType, OnFailure);
        }

        public void RReuterInstrumentIdType()
        {
            readdObject.GetRReuterInstrumentIdType(OnSuccess_VendorIdentifier_InstrumentIdType, OnFailure);
        }

        public void RReuterAssetTypes()
        {
            readdObject.GetRReuterAssetTypes(OnSuccess_AssetTypes, OnFailure);
        }

        public void RWSOInstrumentIdType()
        {
            readdObject.GetWSOInstrumentIdType(OnSuccess_VendorIdentifier_InstrumentIdType, OnFailure);
        }

        public void GetAllTransports()
        {
            readdObject.GetGetAllTransports(OnSuccess_Transport_GetAllTransports, OnFailure);
        }

        public void BindApplicationSpecificData(string className, string methodName,
            string assembly, string id, string nameSpace)
        {
            readdObject.BindApplicationSpecificData(className, methodName, assembly, id, nameSpace,
                OnSuccessBindApplicationSpecificData, OnFailure);
        }

        public void GetRequestType(int vendorType, int licenseType)
        {
            readdObject.GetRequestType(vendorType, licenseType, OnSuccessGetRequestType, OnFailure);

        }

        #endregion


    }
}
