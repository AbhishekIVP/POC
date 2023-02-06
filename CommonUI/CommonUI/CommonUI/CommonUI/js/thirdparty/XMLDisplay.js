function SM_LoadXML(ParentElementID, URL) {
		var xmlHolderElement = SM_GetParentElement(ParentElementID);
		if (xmlHolderElement==null) { return false; }
		SM_ToggleElementVisibility(xmlHolderElement);
		return SM_RequestURL(URL,SM_URLReceiveCallback,ParentElementID);
}

function SM_LoadXMLDom(ParentElementID,xmlDoc,ShowToLevel) {
	if (xmlDoc) {
		var xmlHolderElement = SM_GetParentElement(ParentElementID);
		if (xmlHolderElement==null) { return false; }
		while (xmlHolderElement.childNodes.length) { xmlHolderElement.removeChild(xmlHolderElement.childNodes.item(xmlHolderElement.childNodes.length-1));	}
		var Result = SM_ShowXML(xmlHolderElement,xmlDoc.documentElement,0,ShowToLevel);
		
		//var ReferenceElement = document.createElement('div');
		//var Link = document.createElement('a');		
		//Link.setAttribute('href','http://www.levmuchnik.net/Content/ProgrammingTips/WEB/XMLDisplay/DisplayXMLFileWithJavascript.html');
		//var TextNode = document.createTextNode('Source: Lev Muchnik');
		//Link.appendChild(TextNode);

		//xmlHolderElement.appendChild(Link);
		return Result;
	}
	else { return false; }
}

/*
	"SM_LoadXMLString" is the one of the root functions of this file, i.e.,
	this is one of the 2 functions using which we call this Library.
	
	Here "ShowToLevel" has been given a default value of 100, which is
	can be overwritten by the calling function.
	0 value specifies that only the Root Node of the XML Tree should 
	be displayed and rest all should be collapsed inside it. 
*/
function SM_LoadXMLString(ParentElementID,XMLString,ShowToLevel) {
    if(typeof(ShowToLevel) == 'undefined' || ShowToLevel == null)
    {
        ShowToLevel = 100;
    }
    
    // The line below actually creates our XML Tree	
	xmlDoc = SM_CreateXMLDOM(XMLString);

	// The line below just loads the XML Tree created in the above 
	// statment into the desired DOM element.
	return SM_LoadXMLDom(ParentElementID,xmlDoc,ShowToLevel) ;
}


////////////////////////////////////////////////////////////
// HELPER FUNCTIONS - SHOULD NOT BE DIRECTLY CALLED BY USERS
////////////////////////////////////////////////////////////

/* 
	This function "SM_GetParentElement" returns an Element Object, 
	representing an element with the specified ID, i.e. "ParentElementID". 
	Returns null if no elements with the specified ID exists
*/ 
function SM_GetParentElement(ParentElementID) {
	if (typeof(ParentElementID)=='string') {	return document.getElementById(ParentElementID);	}
	else if (typeof(ParentElementID)=='object') { return ParentElementID;} 
	else { return null; }
}

function SM_URLReceiveCallback(httpRequest,xmlHolderElement) {
	  try {
            if (httpRequest.readyState == 4) {
                if (httpRequest.status == 200) {
					var xmlDoc = httpRequest.responseXML;
					if (xmlHolderElement && xmlHolderElement!=null) {
							xmlHolderElement.innerHTML = '';
							return SM_LoadXMLDom(xmlHolderElement,xmlDoc);
					}
                } else {
                    return false;
                }
            }
        }
        catch( e ) {
            return false;
        }	
}

function SM_RequestURL(url,callback,ExtraData) { // based on: http://developer.mozilla.org/en/docs/AJAX:Getting_Started
        var httpRequest;
        if (window.XMLHttpRequest) { // Mozilla, Safari, ...
            httpRequest = new XMLHttpRequest();
            if (httpRequest.overrideMimeType) { httpRequest.overrideMimeType('text/xml'); }
        } 
        else if (window.ActiveXObject) { // IE
            try { httpRequest = new ActiveXObject("Msxml2.XMLHTTP");   } 
            catch (e) {
				   try { httpRequest = new ActiveXObject("Microsoft.XMLHTTP"); } 
				   catch (e) {}
            }
        }
        if (!httpRequest) { return false;   }
        httpRequest.onreadystatechange = function() { callback(httpRequest,ExtraData); };
        httpRequest.open('GET', url, true);
        httpRequest.send('');
		return true;
}

function SM_CreateXMLDOM(XMLStr) {
	if (window.ActiveXObject)
	 {
		  xmlDoc=new ActiveXObject("Microsoft.XMLDOM"); 
		  xmlDoc.loadXML(XMLStr);	
		  return xmlDoc;
	}
	else if (document.implementation && document.implementation.createDocument)	  {
		  var parser=new DOMParser();
		  return parser.parseFromString(XMLStr,"text/xml");
	}
	else {
		return null;
	}
}		

// The "IDCounter" provides us a way to give generic IDs to  
// all elements as per their occurence in the XML String.
var IDCounter = 1;

// The "NestingIndent" defines how much indentation is to be
// inserted when moving to next level inside the XML Tree.
var NestingIndent = 15;

function SM_ShowXML(xmlHolderElement,RootNode,indent,ShowToLevel) {

	if (RootNode==null || xmlHolderElement==null) { return false; }
	var Result  = true;

	// We add an empty div inside every div.
	// This was present in the version of the library downloaded from internet.
	var TagEmptyElement = document.createElement('div');
	TagEmptyElement.className = 'Element';
	TagEmptyElement.style.position = 'relative';
	TagEmptyElement.style.left = NestingIndent + 'px';
	
	if (RootNode.childNodes.length==0) { 
		var ClickableElement = SM_AddTextNode(TagEmptyElement,'','Clickable') ;
		ClickableElement.id = 'div_empty_' + IDCounter;	  
		SM_AddTextNode(TagEmptyElement,'<','Utility') ;
		SM_AddTextNode(TagEmptyElement,RootNode.nodeName ,'NodeName') 
		for (var i = 0; RootNode.attributes && i < RootNode.attributes.length; ++i) {
		CurrentAttribute  = RootNode.attributes.item(i);
		SM_AddTextNode(TagEmptyElement,' ' + CurrentAttribute.nodeName ,'AttributeName') ;
		SM_AddTextNode(TagEmptyElement,'=','Utility') ;
		SM_AddTextNode(TagEmptyElement,'"' + CurrentAttribute.nodeValue + '"','AttributeValue') ;
		}
		SM_AddTextNode(TagEmptyElement,' />') ;
		xmlHolderElement.appendChild(TagEmptyElement);	
		//SM_SetVisibility(TagEmptyElement,true);    
	}
	else { // No child nodes
    
		//-----------------------------------------
		// Beginning of Invisible Empty Div Element
		//-----------------------------------------
				
		var ClickableElement = SM_AddTextNode(TagEmptyElement,'+','Clickable') ;
		ClickableElement.onclick  = function() {SM_ToggleElementVisibility(this); }
		ClickableElement.id = 'div_empty_' + IDCounter;	
			
		SM_AddTextNode(TagEmptyElement,'<','Utility') ;
		SM_AddTextNode(TagEmptyElement,RootNode.nodeName ,'NodeName') 
		for (var i = 0; RootNode.attributes && i < RootNode.attributes.length; ++i) {
			CurrentAttribute  = RootNode.attributes.item(i);
			SM_AddTextNode(TagEmptyElement,' ' + CurrentAttribute.nodeName ,'AttributeName') ;
			SM_AddTextNode(TagEmptyElement,'=','Utility') ;
			SM_AddTextNode(TagEmptyElement,'"' + CurrentAttribute.nodeValue + '"','AttributeValue') ;
    	}

		SM_AddTextNode(TagEmptyElement,'>  </','Utility') ;
		SM_AddTextNode(TagEmptyElement,RootNode.nodeName,'NodeName') ;
		SM_AddTextNode(TagEmptyElement,'>','Utility') ;
		xmlHolderElement.appendChild(TagEmptyElement);	
		SM_SetVisibility(TagEmptyElement,false);
    	
		//-----------------------------------
		// End of Invisible Empty Div Element
		//-----------------------------------


		//---------------------------------
		// Beginning of Visible Div Element
		//---------------------------------
    	
		var TagElement = document.createElement('div');
		TagElement.className = 'Element';
		TagElement.style.position = 'relative';
		TagElement.style.left = NestingIndent+'px';

		// Clickable Element (i.e. + or -) for the Current XML Node
		ClickableElement = SM_AddTextNode(TagElement,'-','Clickable') ;
		ClickableElement.onclick  = function() {
			SM_ToggleElementVisibility(this); 
		}
		ClickableElement.id = 'div_content_' + IDCounter;	
		++IDCounter;

		//Debugging
		//console.log(ClickableElement);
		//SM_ToggleElementVisibility(document.getElementById('div_content_' + IDCounter));
		
		// Left Utility
		SM_AddTextNode(TagElement,'<','Utility') ;
		
		// Node Name
		SM_AddTextNode(TagElement,RootNode.nodeName ,'NodeName') ;
		
		for (var i = 0; RootNode.attributes && i < RootNode.attributes.length; ++i) {
			CurrentAttribute  = RootNode.attributes.item(i);
			SM_AddTextNode(TagElement,' ' + CurrentAttribute.nodeName ,'AttributeName') ;
			SM_AddTextNode(TagElement,'=','Utility') ;
			SM_AddTextNode(TagElement,'"' + CurrentAttribute.nodeValue + '"','AttributeValue') ;
		}
		
		// Right Utility
		SM_AddTextNode(TagElement,'>','Utility') ;
		
		TagElement.appendChild(document.createElement('br'));
		
		// The following code inserts the next level data inside the current XML Node.
		var NodeContent = null;
		
		for (var i = 0; RootNode.childNodes && i < RootNode.childNodes.length; ++i) {
			if (RootNode.childNodes.item(i).nodeName != '#text') {
				Result &= SM_ShowXML(TagElement,RootNode.childNodes.item(i),indent+1,ShowToLevel-1);
			}
			else {
				NodeContent =RootNode.childNodes.item(i).nodeValue;
			}					
		}			

		if (RootNode.nodeValue) {
			NodeContent = RootNode.nodeValue;
		}

		if (NodeContent) {	
			var ContentElement = document.createElement('div');
			ContentElement.style.position = 'relative';
			ContentElement.style.left = NestingIndent+'px';			
			SM_AddTextNode(ContentElement,NodeContent ,'NodeValue') ;
			TagElement.appendChild(ContentElement);
		}			
		SM_AddTextNode(TagElement,'  </','Utility') ;
		SM_AddTextNode(TagElement,RootNode.nodeName,'NodeName') ;
		SM_AddTextNode(TagElement,'>','Utility') ;
		xmlHolderElement.appendChild(TagElement);
		

		//Debugging
		//ClickableElement.onclick();
		//SM_ToggleElementVisibility(ClickableElement);
		if(ShowToLevel<=0) {
			SM_SetVisibility(TagElement,false);
			SM_SetVisibility(TagEmptyElement,true);
		} 		
  }
	
	 //if (indent==0) { SM_ToggleElementVisibility(TagElement.childNodes(0)); } //- uncomment to collapse the external element
	return Result;
}

/*
	This function SM_AddTextNode adds a span tag, gives it text, class & parent node.
*/
function SM_AddTextNode(ParentNode,Text,Class) {
	NewNode = document.createElement('span');
	if (Class) {  NewNode.className  = Class;}
	if (Text) { NewNode.appendChild(document.createTextNode(Text)); }
	if (ParentNode) { ParentNode.appendChild(NewNode); }
	return NewNode;		
}

/*
	Returns the DOM Object of the specified ID
*/
function SM_CompatibleGetElementByID(id) {
	if (!id) { return null; }
	if (document.getElementById) { // DOM3 = IE5, NS6
		return document.getElementById(id);
	}
	else {
		if (document.layers) { // Netscape 4
			return document.id;
		}
		else { // IE 4
			return document.all.id;
		}
	}
}

function SM_SetVisibility(HTMLElement,Visible) {
	if (!HTMLElement) { return; }
	var VisibilityStr  = (Visible) ? 'block' : 'none';
	if (document.getElementById) { // DOM3 = IE5, NS6
		HTMLElement.style.display = VisibilityStr; 
		
		//Debugging
		//console.log(HTMLElement);
	}
	else {
		if (document.layers) { // Netscape 4
			HTMLElement.display = VisibilityStr; 
		}
		else { // IE 4
			HTMLElement.id.style.display = VisibilityStr; 
		}
	}
}

/*
	Toggles the visibilty of 
*/
function SM_ToggleElementVisibility(Element) {

	if (!Element|| !Element.id) { return; }
	try {
		ElementType = Element.id.slice(0,Element.id.lastIndexOf('_')+1);
		ElementID = parseInt(Element.id.slice(Element.id.lastIndexOf('_')+1));
	}
	catch(e) { return ; }

	// "ElementToHide" and "ElementToShow" contain names of IDs of the elements.
	var ElementToHide = null;
	var ElementToShow= null;
	if (ElementType=='div_content_') {
		ElementToHide = 'div_content_' + ElementID;
		ElementToShow = 'div_empty_' + ElementID;
	}
	else if (ElementType=='div_empty_') {
		ElementToShow= 'div_content_' + ElementID;
		ElementToHide  = 'div_empty_' + ElementID;
	}

	// In the following statements, "ElementToHide" and "ElementToShow"
	// get the reference to the object, whose IDs they were holding before.
	ElementToHide = SM_CompatibleGetElementByID(ElementToHide);
	ElementToShow = SM_CompatibleGetElementByID(ElementToShow);

	if (ElementToHide) { ElementToHide = ElementToHide.parentNode;}
	if (ElementToShow) { ElementToShow = ElementToShow.parentNode;}

	SM_SetVisibility(ElementToHide,false);
	SM_SetVisibility(ElementToShow,true);
}