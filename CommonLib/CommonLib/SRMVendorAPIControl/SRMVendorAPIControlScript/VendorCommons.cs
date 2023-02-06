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

namespace com.ivp.srm.controls.vendor.script
{
    public class VendorCommons
    {
        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Determines whether it is null or empty [the specified text].
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if [is null empty] [the specified text]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullEmpty(string text)
        {
            if(text != null && text.Trim() != "")
                return false;
            return true;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates an arraylist of method-event pairs
        /// </summary>
        /// <param name="pairs">a string of pairs</param>
        /// <returns>returns an arraylist of MethodEventPair</returns>
        public static ArrayList CreateEventMethod(string pairs)
        {
            ArrayList pipeSeperated = new ArrayList();
            ArrayList methodEventPair = new ArrayList();
            ArrayList.AddRange(pipeSeperated, pairs.Split('|'));
            MethodEventPair methodEventPairInfo;
            for (int i = 0; i < pipeSeperated.Length; i++)
            {
                methodEventPairInfo = new MethodEventPair();
                methodEventPairInfo.MethodName = pipeSeperated[i].ToString().Split('~')[0];
                methodEventPairInfo.EventName = pipeSeperated[i].ToString().Split('~')[1];
                ArrayList.Add(methodEventPair, methodEventPairInfo);
            }

            return methodEventPair;
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Binds the drop down.
        /// </summary>
        /// <param name="selectElement">The select element.</param>
        /// <param name="text">The text.</param>
        /// <param name="value">The value.</param>
        public static void BindDropDown(DOMElement selectElement, ArrayList text, ArrayList value)
        {
            SelectElement s = (SelectElement)selectElement;
            ClearDropDown(s);
            OptionElement _optionElement;
            _optionElement = (OptionElement)Document.CreateElement("OPTION");
            _optionElement.Value = "-1";
            _optionElement.Text = "--Select One--";
            s.Add(_optionElement);

            for (int i = 0; i < text.Length; i++)
            {
                _optionElement = (OptionElement)Document.CreateElement("OPTION");
                if (value == null)
                {
                    _optionElement.Value = text[i].ToString();
                    _optionElement.Text = text[i].ToString();
                }
                else
                {
                    _optionElement.Value = ((XMLDOM)value[i]).Text;
                    _optionElement.Text = ((XMLDOM)text[i]).Text;
                }

                s.Add(_optionElement);
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Clears the drop down.
        /// </summary>
        /// <param name="s">The s.</param>
        public static void ClearDropDown(SelectElement s)
        {
            int length = s.Options.Length;
            for (int i = 0; i < length; i++)
            {
                s.Remove(0);
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates the error message holder.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns></returns>
        public static DOMElement CreateErrorMessageHolder(DOMElement container)
        {
            if (container != null)
            {
                if (container.HasChildNodes())
                {
                    container.RemoveChild(container.ChildNodes[0]);
                }
                DOMElement ulElement = Document.CreateElement("ul");
                container.AppendChild(ulElement);
                container.Style.Display = "none";
                return ulElement;
            }
            return null;
        }


        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates the error message.
        /// </summary>
        /// <param name="errorHolder">The error holder.</param>
        /// <param name="errorMessage">The error message.</param>
        public static void CreateErrorMessage(DOMElement errorHolder,
            string errorMessage)
        {
            if (errorHolder != null && IsNullEmpty(errorMessage) == false)
            {
                DOMElement liElemenent = Document.CreateElement("li");
                liElemenent.InnerHTML = errorMessage;
                errorHolder.AppendChild(liElemenent);
                if (errorHolder.ParentNode != null)
                {
                    errorHolder.ParentNode.Style.Display = "";
                }
            }
        }

        //------------------------------------------------------------------------------------------
        /// <summary>
        /// Clears the error holder.
        /// </summary>
        /// <param name="container">The container.</param>
        public static void ClearErrorHolder(DOMElement container)
        {
            if (container != null && container.HasChildNodes())
            {
                container.RemoveChild(container.ChildNodes[0]);
                container.Style.Display = "none";
            }
        }
    }

    //------------------------------------------------------------------------------------------
    /// <summary>
    /// info class for attributes in html controls
    /// </summary>
    public class VendorAttributeConstants
    {
        internal const string ClientMethodName = "clientMethodName";
        internal const string ServerMethodName = "serverMethodName";
        internal const string ClientEventType = "clientEventType";
        internal const string ServerEventType = "serverEventType";
        internal const string Id = "id";
        internal const string DisplayText = "display_text";
        internal const string Key = "key";
        internal const string Visible = "visible";
        internal const string IsMandatory = "is_mandatory";
        internal const string DataTextField = "datatextfield";
        internal const string DataValueField = "datavaluefield";
        internal const string SavedValue = "saved_value";
        internal const string ContextType = "contexttype";
        internal const string Assmebly = "assembly";
        internal const string Classname = "classname";
        internal const string Namespace = "namespace";
        internal const string VendorPropertyName = "vendorpropertyname";
        internal const string CustomClientMethod = "customclientmethod";
        internal const string ServerMethodArguments = "serverMethodArguments";
        internal const string IsThirdPartyControl = "isthirdPartyControl";
        internal const string AutoCompleteControlID = "autoCompleteControlID";
    }

    //------------------------------------------------------------------------------------------
    /// <summary>
    /// pairs of method name and event name
    /// </summary>
    public class MethodEventPair
    {
        public string MethodName;
        public string EventName;
    }

    //------------------------------------------------------------------------------------------
    /// <summary>
    /// _elementDictionary contains the controls
    /// </summary>
    public class VendorControls
    {
        //static Dictionary _elementDictionary;
        //static VendorControls()
        //{
        //    _elementDictionary = new Dictionary();
        //}

        public static DOMElement GetElementByID(string clientID)
        {
            return Document.GetElementById(clientID);
        }
    }

    //------------------------------------------------------------------------------------------
    /// <summary>
    /// info class for all the types of html 
    /// </summary>
    public class TypeOfVendorControl
    {
        internal const string API = "API";
        internal const string API_Bloomberg = "API_Bloomberg";
        internal const string API_Reuters = "API_Reuters";
        internal const string API_MarkitWSO = "API_MarkitWSO";
        internal const string FTP = "FTP";
        internal const string FTP_Bloomberg = "FTP_Bloomberg";
        internal const string FTP_Reuters = "FTP_Reuters";
        internal const string REALTIMEPREFERENCE_Bloomberg = "REALTIMEPREFERENCE_Bloomberg";
        internal const string REALTIMEPREFERENCE_Reuters = "REALTIMEPREFERENCE_Reuters";
        internal const string REALTIMEPREFERENCE = "REALTIMEPREFERENCE";
    }


    public enum FTPType
    {
        None = 0,
        Request = 1,
        Response = 2
    }

    //------------------------------------------------------------------------------------------
    /// <summary>
    /// ReturnData: the data returned by third party methods
    /// DataType: the type of data returned
    /// ControlId: the id of the control to which data would be bound
    /// </summary>
    public class ApplicationSpecificData
    {
        [PreserveCase]
        public object ReturnData;
        [PreserveCase]
        public DataType DataType;
        [PreserveCase]
        public string ControlId;
    }

    public enum DataType
    {
        DataSet = 0,
        ArrayList = 1
    }
}
