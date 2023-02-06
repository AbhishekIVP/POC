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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Reflection;
using System.Web;
using System.Xml;

namespace com.ivp.srm.controls.vendor
{
    [ToolboxData("<{0}:VendorAPIControl runat=server></{0}:VendorAPIControl>")]
    public class VendorAPIControl : ScriptControl
    {
        #region "properies"

        private string VendorHTML { get; set; }
        private HiddenField _hiddenXml = null;

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the hidden XML.
        /// </summary>
        /// <value>The hidden XML.</value>
        internal HiddenField HiddenXml
        {
            get
            {
                if (_hiddenXml == null)
                {
                    _hiddenXml = new HiddenField();
                    _hiddenXml.ID = this.ClientID + "_HdnErrorMessage";
                }
                return _hiddenXml;
            }
        }

        #endregion

        #region "Public Properties"
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the type of control.
        /// </summary>
        /// <value>The type of control.</value>
        public TypeOfControl TypeOfControl { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the button submit ID.
        /// </summary>
        /// <value>The button submit ID.</value>
        public string ButtonSubmitID { get; set; }

        public string ButtonCancelID { get; set; }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets or sets the input XML.
        /// </summary>
        /// <value>The input XML.</value>
        public string InputXml { get; set; }


        public string ModuleId { get; set; }
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the output XML.
        /// </summary>
        /// <value>The output XML.</value>
        public string OutputXml
        {
            get
            {
                return GetContextValue(HiddenXml.ClientID).Replace("&gt;", ">").Replace("&lt;", "<");
            }
        }

        public List<string> DisableKey { get; set; }

        public string AssemblyName { get; set; }
        
        public string HtmlLoc { get; set; }

        //new property added for secmaster realtimepreference control
        public bool ReplaceSubmitBtn { get; set; }
        public bool ReadOnlyDatasourceCtrl { get; set; }
        public string SetDefaultVendorType { get; set; }
        public bool ReadOnlyVendorType { get; set; }





        #endregion

        #region IScriptControl Members

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the script descriptors.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            LoadAllAndDefaultHTML();
            ScriptBehaviorDescriptor descriptor = new ScriptBehaviorDescriptor
               ("com.ivp.srm.controls.vendor.script.VendorScript", ClientID);
            descriptor.AddProperty("vendorHTML", VendorHTML);
            descriptor.AddProperty("buttonSubmitID", ButtonSubmitID);
            descriptor.AddProperty("buttonCancelID", ButtonCancelID);
            descriptor.AddProperty("typeOfControl", TypeOfControl.ToString());
            descriptor.AddProperty("hasSavedValue", string.IsNullOrEmpty(InputXml) ? false : true);
            descriptor.AddProperty("htmlLoc", HtmlLoc == null ? string.Empty : HtmlLoc);
            descriptor.AddProperty("assemblyName", AssemblyName == null ? string.Empty : AssemblyName);

            descriptor.AddProperty("replaceSubmitBtn", ReplaceSubmitBtn);
            descriptor.AddProperty("readOnlyDatasourceCtrl", ReadOnlyDatasourceCtrl);
            descriptor.AddProperty("setDefaultVendorType", SetDefaultVendorType);
            descriptor.AddProperty("readOnlyVendorType", ReadOnlyVendorType);
            descriptor.AddProperty("disableKey", DisableKey == null ? new List<string>() : DisableKey);
            descriptor.AddProperty("moduleId", string.IsNullOrEmpty(ModuleId) ? "" : ModuleId);
            //descriptor.AddProperty("hasCustomIdentifier", HasCustomIdentifier.ToString().ToLower());
            return new ScriptBehaviorDescriptor[] { descriptor };
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the script references.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<ScriptReference> GetScriptReferences()
        {
            ScriptReference reference = new ScriptReference
                ("com.ivp.srm.controls.vendor.Resources.Scripts.SRMVendorAPIControlScript.debug.js", "SRMVendorAPIControl");
            return new ScriptReference[] { reference };
        }

        #endregion

        #region "Render"

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Renders the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
            HiddenXml.RenderControl(writer);
        }

        #endregion

        #region "Helper Methods"

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Gets the context value.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <returns></returns>
        private string GetContextValue(string clientId)
        {
            foreach (string key in HttpContext.Current.Request.Form.AllKeys)
            {
                if (key != null && key.EndsWith(clientId))
                    return HttpContext.Current.Request.Form.Get(key);
            }
            return string.Empty;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Loads all and default HTML.
        /// </summary>
        private void LoadAllAndDefaultHTML()
        {
            Assembly assembly;
            AssemblyName vendorAssemblyName;
            string defaultType = TypeOfVendorControl.API;
            StreamReader textStreamReader = null;
            string dataRead = string.Empty;
            XmlDocument doc = null;

            try
            {
                vendorAssemblyName = new AssemblyName(AssemblyName);
                assembly = Assembly.Load(vendorAssemblyName);
                if (string.IsNullOrEmpty(InputXml) == false)
                {
                    doc = new XmlDocument();
                    doc.LoadXml(InputXml);
                    defaultType = (doc.ChildNodes[0].Name + "_" + doc.SelectNodes("//control[@key=\"vendor\"]")[0].Attributes["text"].Value).ToLower();
                }
                else
                {
                    switch (TypeOfControl)
                    {
                        case TypeOfControl.API:
                            defaultType = TypeOfVendorControl.API.ToLower();
                            break;
                        case TypeOfControl.REALTIMEPREFERENCE:
                            defaultType = TypeOfVendorControl.REALTIMEPREFERENCE.ToLower();
                            break;
                        case TypeOfControl.FTP:
                            defaultType = TypeOfVendorControl.FTP.ToLower();
                            break;
                    }
                }

                textStreamReader = new StreamReader(assembly.GetManifestResourceStream
                    //("com.ivp.srm.controls.vendor.Resources.HTML." + defaultType + ".htm"));
                    (HtmlLoc + "." + defaultType + ".htm"));
                dataRead = textStreamReader.ReadToEnd();

                switch (defaultType)
                {
                    #region "API"
                    case TypeOfVendorControl.API:
                        dataRead = string.Format(
                            dataRead,
                            ClientID,
                            string.Empty,
                            string.Empty,
                            string.Empty);
                        break;
                    #endregion

                    #region "API_Bloomberg"
                    case TypeOfVendorControl.API_Bloomberg:
                        dataRead = string.Format(
                            dataRead,
                            ClientID,
                            doc.SelectNodes("//control[@key=\"taskname\"]")[0].Attributes["text"].Value,
                            doc.SelectNodes("//control[@key=\"taskdescription\"]")[0].Attributes["text"].Value,
                            doc.SelectNodes("//control[@key=\"vendor\"]")[0].Attributes["value"].Value,
                            doc.SelectNodes("//control[@key=\"apitype\"]")[0].Attributes["value"].Value,
                            doc.SelectNodes("//control[@key=\"vendoridentifier\"]")[0].Attributes["value"].Value,
                            doc.SelectNodes("//control[@key=\"MarketSector\"]")[0].Attributes["value"].Value,
                            doc.SelectNodes("//control[@key=\"secmvendoridentifier\"]").Count > 0
                                ? doc.SelectNodes("//control[@key=\"secmvendoridentifier\"]")[0].Attributes["value"].Value
                                : string.Empty,
                                doc.SelectNodes("//control[@key=\"secmastermarketsectormappedattributerequest\"]").Count > 0
                                ? doc.SelectNodes("//control[@key=\"secmastermarketsectormappedattributerequest\"]")[0].Attributes["value"].Value
                                : string.Empty);
                            //HasCustomIdentifier.ToString().ToLower());
                        break;
                    #endregion

                    #region "API_MarkitWSO"
                    case TypeOfVendorControl.API_MarkitWSO:
                        dataRead = string.Format(
                            dataRead,
                            ClientID,
                            doc.SelectNodes("//control[@key=\"taskname\"]")[0].Attributes["text"].Value,
                            doc.SelectNodes("//control[@key=\"taskdescription\"]")[0].Attributes["text"].Value,
                            doc.SelectNodes("//control[@key=\"vendor\"]")[0].Attributes["value"].Value,
                            doc.SelectNodes("//control[@key=\"vendoridentifier\"]")[0].Attributes["value"].Value,
                            doc.SelectNodes("//control[@key=\"secmvendoridentifier\"]").Count > 0
                                ? doc.SelectNodes("//control[@key=\"secmvendoridentifier\"]")[0].Attributes["value"].Value
                                : string.Empty);
                        //HasCustomIdentifier.ToString().ToLower());
                        break;
                    #endregion

                    #region "API_Reuters"
                    case TypeOfVendorControl.API_Reuters:
                        dataRead = string.Format(
                            dataRead,
                            ClientID,
                            doc.SelectNodes("//control[@key=\"taskname\"]")[0].Attributes["text"].Value,
                            doc.SelectNodes("//control[@key=\"taskdescription\"]")[0].Attributes["text"].Value,
                            doc.SelectNodes("//control[@key=\"vendor\"]")[0].Attributes["value"].Value,
                            doc.SelectNodes("//control[@key=\"apitype\"]")[0].Attributes["value"].Value,
                            doc.SelectNodes("//control[@key=\"vendoridentifier\"]")[0].Attributes["value"].Value,
                            doc.SelectNodes("//control[@key=\"assettype\"]")[0].Attributes["value"].Value,
                            doc.SelectNodes("//control[@key=\"secmvendoridentifier\"]").Count > 0
                                ? doc.SelectNodes("//control[@key=\"secmvendoridentifier\"]")[0].Attributes["value"].Value
                                : string.Empty,
                            doc.SelectNodes("//control[@key=\"secmastermarketsectormappedattributerequest\"]").Count > 0
                                ? doc.SelectNodes("//control[@key=\"secmastermarketsectormappedattributerequest\"]")[0].Attributes["value"].Value
                                : string.Empty);
                            //HasCustomIdentifier.ToString().ToLower());
                        break;
                    #endregion

                    #region "REALTIMEPREFERENCE"
                    case TypeOfVendorControl.REALTIMEPREFERENCE :
                        dataRead = string.Format(
                            dataRead,
                            ClientID,
                            string.Empty,
                            string.Empty,
                            string.Empty);
                        break;
                    #endregion

                    #region "REALTIMEPREFERENCE_Bloomberg"
                    case TypeOfVendorControl.REALTIMEPREFERENCE_Bloomberg:
                        dataRead = string.Format(
                            dataRead,
                            ClientID,
                            doc.SelectNodes("//control[@key=\"name\"]")[0].Attributes["text"].Value,
                            doc.SelectNodes("//control[@key=\"description\"]")[0].Attributes["text"].Value,
                            doc.SelectNodes("//control[@key=\"vendor\"]")[0].Attributes["value"].Value,
                            doc.SelectNodes("//control[@key=\"apitype\"]")[0].Attributes["value"].Value,
                            doc.SelectNodes("//control[@key=\"transport\"]").Count > 0
                                ? doc.SelectNodes("//control[@key=\"transport\"]")[0].Attributes["value"].Value
                                : string.Empty,
                            doc.SelectNodes("//control[@key=\"vendoridentifier\"]")[0].Attributes["value"].Value,
                            doc.SelectNodes("//control[@key=\"marketsector\"]")[0].Attributes["value"].Value,
                            doc.SelectNodes("//control[@key=\"datasource\"]").Count > 0
                                ? doc.SelectNodes("//control[@key=\"datasource\"]")[0].Attributes["value"].Value
                                : string.Empty);
                        break;

                    #endregion

                    #region "REALTIMEPREFERENCE_Reuters"
                    case TypeOfVendorControl.REALTIMEPREFERENCE_Reuters:
                        dataRead = string.Format(
                            dataRead,
                            ClientID,
                            doc.SelectNodes("//control[@key=\"name\"]")[0].Attributes["text"].Value,
                            doc.SelectNodes("//control[@key=\"description\"]")[0].Attributes["text"].Value,
                            doc.SelectNodes("//control[@key=\"vendor\"]")[0].Attributes["value"].Value,
                            doc.SelectNodes("//control[@key=\"apitype\"]")[0].Attributes["value"].Value,
                            doc.SelectNodes("//control[@key=\"transport\"]").Count > 0
                                ? doc.SelectNodes("//control[@key=\"transport\"]")[0].Attributes["value"].Value
                                : string.Empty,
                            doc.SelectNodes("//control[@key=\"vendoridentifier\"]")[0].Attributes["value"].Value,
                            doc.SelectNodes("//control[@key=\"assettype\"]")[0].Attributes["value"].Value,
                            doc.SelectNodes("//control[@key=\"datasource\"]").Count > 0
                                ? doc.SelectNodes("//control[@key=\"datasource\"]")[0].Attributes["value"].Value
                                : string.Empty);
                        break;
                    #endregion

                    #region "FTP"
                    case TypeOfVendorControl.FTP:
                        dataRead = string.Format(
                            dataRead,
                            ClientID,
                            string.Empty,
                            string.Empty,
                            string.Empty);
                        break;
                    #endregion

                    #region "FTP_Bloomberg"
                    case TypeOfVendorControl.FTP_Bloomberg:
                        dataRead = string.Format(
                        dataRead,
                        ClientID,
                        doc.SelectNodes("//control[@key=\"tasknamerequest\"]")[0].Attributes["text"].Value,
                        doc.SelectNodes("//control[@key=\"taskdescriptionrequest\"]")[0].Attributes["text"].Value,
                        doc.SelectNodes("//control[@key=\"vendor\"]")[0].Attributes["value"].Value,
                        doc.SelectNodes("//control[@key=\"transporttyperequest\"]")[0].Attributes["value"].Value,
                        doc.SelectNodes("//control[@key=\"secmasteridentifierrequest\"]").Count > 0
                            ? doc.SelectNodes("//control[@key=\"secmasteridentifierrequest\"]")[0].Attributes["value"].Value
                            : string.Empty,
                        doc.SelectNodes("//control[@key=\"vendoridentifierrequest\"]")[0].Attributes["value"].Value,
                        doc.SelectNodes("//control[@key=\"outgoingftprequest\"]")[0].Attributes["value"].Value,
                        doc.SelectNodes("//control[@key=\"marketsectorrequest\"]")[0].Attributes["value"].Value,
                        doc.SelectNodes("//control[@key=\"tasknameresponse\"]")[0].Attributes["text"].Value,
                        doc.SelectNodes("//control[@key=\"taskdescriptionresponse\"]")[0].Attributes["text"].Value,
                        doc.SelectNodes("//control[@key=\"vendor\"]")[0].Attributes["value"].Value,
                        doc.SelectNodes("//control[@key=\"transporttyperesponse\"]")[0].Attributes["value"].Value,
                        doc.SelectNodes("//control[@key=\"incomingftpresponse\"]")[0].Attributes["value"].Value,
                        doc.SelectNodes("//control[@key=\"datarequesttype\"]").Count > 0
                            ? doc.SelectNodes("//control[@key=\"datarequesttype\"]")[0].Attributes["value"].Value
                            : string.Empty,
                        doc.SelectNodes("//control[@key=\"secmastermarketsectormappedattributerequest\"]").Count > 0
                            ? doc.SelectNodes("//control[@key=\"secmastermarketsectormappedattributerequest\"]")[0].Attributes["value"].Value
                            : string.Empty,
                        doc.SelectNodes("//control[@key=\"secmasterbvalmappedattributerequest\"]").Count > 0
                            ? doc.SelectNodes("//control[@key=\"secmasterbvalmappedattributerequest\"]")[0].Attributes["value"].Value
                            : string.Empty);
                        
                        //HasCustomIdentifier.ToString().ToLower());
                        break;
                    #endregion

                    #region "FTP_Reuters"
                    case TypeOfVendorControl.FTP_Reuters:
                        dataRead = string.Format(
                        dataRead,
                        ClientID,

                        doc.SelectNodes("//control[@key=\"tasknamerequest\"]")[0].Attributes["text"].Value,
                        doc.SelectNodes("//control[@key=\"taskdescriptionrequest\"]")[0].Attributes["text"].Value,
                        doc.SelectNodes("//control[@key=\"vendor\"]")[0].Attributes["value"].Value,
                        doc.SelectNodes("//control[@key=\"transporttyperequest\"]")[0].Attributes["value"].Value,
                        doc.SelectNodes("//control[@key=\"secmasteridentifierrequest\"]").Count > 0
                            ? doc.SelectNodes("//control[@key=\"secmasteridentifierrequest\"]")[0].Attributes["value"].Value
                            : string.Empty,
                        doc.SelectNodes("//control[@key=\"vendoridentifierrequest\"]")[0].Attributes["value"].Value,
                        doc.SelectNodes("//control[@key=\"outgoingftprequest\"]")[0].Attributes["value"].Value,
                        doc.SelectNodes("//control[@key=\"assettyperequest\"]")[0].Attributes["value"].Value,
                        doc.SelectNodes("//control[@key=\"tasknameresponse\"]")[0].Attributes["text"].Value,
                        doc.SelectNodes("//control[@key=\"taskdescriptionresponse\"]")[0].Attributes["text"].Value,
                        doc.SelectNodes("//control[@key=\"vendor\"]")[0].Attributes["value"].Value,
                        doc.SelectNodes("//control[@key=\"transporttyperesponse\"]")[0].Attributes["value"].Value,
                        doc.SelectNodes("//control[@key=\"incomingftpresponse\"]")[0].Attributes["value"].Value,
                        doc.SelectNodes("//control[@key=\"datarequesttype\"]").Count > 0
                            ? doc.SelectNodes("//control[@key=\"datarequesttype\"]")[0].Attributes["value"].Value
                            : string.Empty,
                        doc.SelectNodes("//control[@key=\"secmastermarketsectormappedattributerequest\"]").Count > 0
                            ? doc.SelectNodes("//control[@key=\"secmastermarketsectormappedattributerequest\"]")[0].Attributes["value"].Value
                            : string.Empty);
                        //HasCustomIdentifier.ToString().ToLower());
                        break;
                    #endregion

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (textStreamReader != null)
                {
                    textStreamReader.Close();
                    textStreamReader.Dispose();
                    textStreamReader = null;
                }
                if (doc != null)
                {
                    doc.RemoveAll();
                    doc = null;
                }
            }
            VendorHTML = dataRead;
        }
        #endregion
    }

    public enum TypeOfControl 
    {
        API,
        FTP,
        REALTIMEPREFERENCE
    }

    /// <summary>
    /// contains info for all the types of html of controls
    /// </summary>
    public class TypeOfVendorControl
    {
        public const string API = "api";
        public const string API_Bloomberg = "api_bloomberg";
        public const string API_MarkitWSO = "api_markitwso";
        public const string API_Reuters = "api_reuters";
        public const string FTP = "ftp";
        public const string FTP_Bloomberg = "ftp_bloomberg";
        public const string FTP_Reuters = "ftp_reuters";
        public const string REALTIMEPREFERENCE_Bloomberg = "realtimepreference_bloomberg";
        public const string REALTIMEPREFERENCE_Reuters = "realtimepreference_reuters";
        public const string REALTIMEPREFERENCE = "realtimepreference";
    }
}
