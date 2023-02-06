var srfpmAttributeMetaData = (function () {

    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    if (path.endsWith('/App_Dynamic_Resource'))
        path = path.replace(new RegExp('/App_Dynamic_Resource', 'g'), '');

    function SRFPMAttributeMetaData() {
        //ClickTarget - Div which was clicked. To decide top, left, etc.
        //ContainerDiv - Div in which details are to be added.
        //TypeId - SectypeId / EntityTypeId
        //IsAdditionalLeg - Used only in SecMaster
        //CallbackForDetails - To perform normal operation that would be performed, if this was disabled.
        //CallbackObject - Required to pass to callbackForDetails Method.

        this.clickTarget = null;
        this.containerDiv = null;
        this.moduleId = 0;
        this.typeId = 0;
        this.attributeId = 0;
        this.isAdditionalLeg = false;
        this.callbackForDetails = null;
        this.callbackObject = null;
        this.attributeName = null;
        this.viewDetails = false;
        this.hideIsPrimary = false;
        // Additional prop for ref
        this.isCreateScreen = false;
        this.isMain = true;
    }

    var srfpmAttributeMetaData = new SRFPMAttributeMetaData();

    var docHandler = function (e) {
        e.stopPropagation();
        var target = e.target;
        if ($(target).closest('#AttributeMetaDataPopup').length > 0) { }
        else
        {
            $("#" + srfpmAttributeMetaData.containerDiv).html("");
            $("#" + srfpmAttributeMetaData.containerDiv).hide();
            $(document).unbind('click', docHandler);
        }
    }
    //positioning of popup
    function setPosition(target) {
        //var viewportHeight = $(window).height();
        var top = 0, left = 0;

        if (srfpmAttributeMetaData.moduleId == 3) {
            $('#AttributeMetaDataPopup').css('display', 'none');
            var docHeight = $(document).height();
            $('#AttributeMetaDataPopup').css('display', 'block');
            var popupHeight = $('#AttributeMetaDataPopup').outerHeight();
            var position = $("#" + target).offset();
            if (docHeight - (position.top + popupHeight + $("#divBottomFixed").height()) > 0) {
                top = position.top + $("#" + target).outerHeight() - 10; //- 10
                left = position.left;
            }
            else {
                top = position.top - popupHeight - 15 - 27; //- 10
                left = position.left;
            }
        }
        else {
            top = parseInt($("#" + target).offset().top);
            left = parseInt($("#" + target).offset().left);
        }
        //var diff = top + height - viewportHeight;

        $('#AttributeMetaDataPopup').css({ top: top + 27, left: left - 7, position: 'absolute' });

    }

    SRFPMAttributeMetaData.prototype.create = function create($object) {

        this.moduleId = $object.moduleId;
        this.attributeName = $object.hasOwnProperty("attributeName") ? $object.attributeName : "";
        this.containerDiv = $object.hasOwnProperty("containerDiv") ? $object.containerDiv : "";
        this.clickTarget = $object.clickTarget;
        this.callbackForDetails = $object.callbackForDetails;
        this.callbackObject = $object.callbackObject;
        this.isAdditionalLeg = $object.hasOwnProperty("isAdditionalLeg") ? $object.isAdditionalLeg : false;
        this.viewDetails = $object.hasOwnProperty("viewDetails") ? $object.viewDetails : false;
        this.hideIsPrimary = $object.hasOwnProperty("hideIsPrimary") ? $object.hideIsPrimary : false;
        this.isCreateScreen = $object.hasOwnProperty("isCreateScreen") ? $object.isCreateScreen : false;
        this.isMain = $object.hasOwnProperty("isMain") ? ($object.isMain != undefined && $object.isMain == 0 ? false : true) : true;

        var params = {};
        params.InputObject = {};
        params.InputObject.ModuleId = $object.moduleId;
        params.InputObject.TypeId = $object.typeId;
        params.InputObject.AttributeId = $object.attributeId;
        params.InputObject.IsAdditionalLeg = $object.hasOwnProperty("isAdditionalLeg") ? $object.isAdditionalLeg : false;
        params.InputObject.AttributeName = $object.attributeName;

        CallCommonServiceMethod('GetAttributeMetaData', params, OnSuccess_BindHtml, OnFailure, null, false);

    }


    /////////////////////////////////////////////////
    // Callback Functions for COMMON SERVICE Calls //
    /////////////////////////////////////////////////

    function OnSuccess_BindHtml(result) {
        var toShowDataType = false, toShowDataLength = false, toShowRefTypeName = false, toShowSectypeName = false, toShowDefaultValue = false, toShowRestrictedChar = false, toShowPII = false, toShowEncrypted = false, toShowPrimary = false, toShowTags = false, toShowIsCloneable = false, toShowAttributeDescription = false, toShowI = true;
        var toShowVendorPriority = true;
        var outputData = result.d;
        //var outputData = { attrName: "Attr1", dataType: "VARCHAR", DataLength: "100", ReferenceTypeName: "", SecurityTypeName: "", Tags: "abc,def", IsClonable: true, DefaultValue: "", RestrictedChar: "@", IsPII: true, IsEncrypted: true, IsPrimary: "-1", AttributeDescription: "", VendorPrioritization: "Bloomberg,Reuters" };
        var html = '';

        if (srfpmAttributeMetaData.moduleId == 3) {

            //Data Type
            //Is Cloneable
            //Attribute Description
            if (srfpmAttributeMetaData.attributeName.toLowerCase() != 'underlying_sec_id') {
                toShowDataType = true;
                toShowIsCloneable = true;
                toShowAttributeDescription = true;
            }
            else {
                toShowI = false;
            }

            //Data Length
            if (typeof outputData.DataLength != 'undefined' && outputData.DataLength != null && outputData.DataLength != '' && typeof outputData.DataType != 'undefined' && typeof outputData.DataType != null && outputData.DataType.toUpperCase() == 'STRING' || outputData.DataType.toUpperCase() == 'NUMERIC') {
                toShowDataLength = true;
            }

            //Ref Type Name
            if (typeof outputData.ReferenceEntityTypeName != 'undefined' && outputData.ReferenceEntityTypeName != null && outputData.ReferenceEntityTypeName != '' && outputData.DataType == 'REFERENCE') {
                toShowRefTypeName = true;
            }

            //Sec Type Name
            //NOTE : Can NOT add datatype check, as in case of basket constituent, we dont show data type.
            if (typeof outputData.SecurityTypeName != 'undefined' && outputData.SecurityTypeName != null && outputData.SecurityTypeName != '') {
                toShowSectypeName = true;
            }

            //Is Primary
            if (typeof (srfpmAttributeMetaData.hideIsPrimary) != 'undefined' && srfpmAttributeMetaData.hideIsPrimary) {
                toShowPrimary = false;
            }
            else {
                if (typeof outputData.IsPrimary != 'undefined' && outputData.IsPrimary != -1) {
                    //In Baskets, EXCEPT Additional Legs, Boolean, Reference & File attributes can NOT be primary.
                    if (!(!srfpmAttributeMetaData.isAdditionalLeg && (outputData.DataType.toUpperCase() == 'REFERENCE' || outputData.DataType.toUpperCase() == 'BOOLEAN' || outputData.DataType.toUpperCase() == 'FILE'))) {
                        toShowPrimary = true;
                    }
                }
            }

            //To Show Vendor Priority hidden in case of file type attribute
            if (outputData.DataType.toUpperCase() == 'FILE') {
                toShowVendorPriority = false;
            }

            //Tags
            if (!srfpmAttributeMetaData.isAdditionalLeg && srfpmAttributeMetaData.attributeName.toLowerCase() != 'underlying_sec_id') {
                toShowTags = true;
            }

        }
        else {
            toShowTags = true;
            toShowIsCloneable = true;
            toShowAttributeDescription = true;

            //if (srfpmAttributeMetaData.viewDetails)
            //    toShowI = false;
            if (outputData.IsPrimary != -1) {
                toShowPrimary = true;
                //toShowI = false;
            }
            if (srfpmAttributeMetaData.isCreateScreen || !srfpmAttributeMetaData.isMain)
                toShowI = false;
            if (outputData.DataType.toUpperCase() == 'VARCHAR') {
                toShowDataType = true; toShowDataLength = true; toShowPII = true; toShowEncrypted = true; toShowRestrictedChar = true; toShowDefaultValue = true;
            }
            if (outputData.DataType.toUpperCase() == 'DECIMAL') {
                toShowDataType = true; toShowDataLength = true; toShowPII = true; toShowEncrypted = true; toShowDefaultValue = true;
            }
            if (outputData.DataType.toUpperCase() == 'VARCHARMAX') {
                toShowDataType = true; toShowPII = true; toShowEncrypted = true; toShowRestrictedChar = true; toShowDefaultValue = true;
            }
            if (outputData.DataType.toUpperCase() == 'INT' || outputData.DataType.toUpperCase() == 'DATETIME') {
                toShowDataType = true; toShowPII = true; toShowEncrypted = true; toShowDefaultValue = true;
            }
            if (outputData.DataType.toUpperCase() == 'BIT') {
                toShowDataType = true; toShowPII = true; toShowEncrypted = true; toShowDefaultValue = true;
                if (outputData.IsPrimary != -1) {
                    toShowPrimary = true;
                }
            }
            if (outputData.DataType.toUpperCase() == 'FILE') {
                toShowDataType = true; toShowVendorPriority = false;
                if (outputData.IsPrimary != -1) {
                    toShowPrimary = false;
                }
            }
            if (outputData.DataType.toUpperCase() == 'LOOKUP') {
                toShowDataType = true; toShowRefTypeName = true; toShowDefaultValue = true;
            }
            if (outputData.DataType.toUpperCase() == 'SECURITY_LOOKUP') {
                toShowDataType = true; toShowSectypeName = true; toShowDefaultValue = true;
            }
        }

        html = '<div id="AttributeMetaDataPopup" class="outerDiv"><div class = "metaDataInfoDiv">';
        if (toShowDataType)
            html += '<div class="divDataType positionAlign"><div class ="labelClass" >Data Type  <br></div><div class="valueClass" title = "' + outputData.DataType.toUpperCase() + '">' + outputData.DataType.toUpperCase() + '<br></div></div>';
        if (toShowDataLength)
            html += '<div class="divLength positionAlign"><div class ="labelClass">Length  <br></div><div class="valueClass" title ="' + outputData.DataLength.toUpperCase() + '">' + outputData.DataLength.toUpperCase() + '<br></div></div>';
        if (toShowRefTypeName)
            html += '<div class="divEntityType positionAlign"><div class ="labelClass">Entity Type  <br></div><div class="valueClass" title ="' + outputData.ReferenceEntityTypeName.toUpperCase() + '">' + outputData.ReferenceEntityTypeName.toUpperCase() + '<br></div></div>';
        if (toShowSectypeName)
            html += '<div class="divSecurityType positionAlign"><div class ="labelClass">Security Type(s)  <br></div><div class="valueClass" title ="' + outputData.SecurityTypeName.toUpperCase() + '">' + outputData.SecurityTypeName.toUpperCase() + '<br></div></div>';
        if (toShowIsCloneable)
            html += '<div class="divCloneable positionAlign"><div class ="labelClass">Is Cloneable  <br></div><div class="valueClass" title ="' + (outputData.IsCloneable + '').toUpperCase() + '">' + (outputData.IsCloneable + '').toUpperCase() + '<br></div></div>';
        if (toShowPII)
            html += '<div class="divPII positionAlign"><div class ="labelClass">Is PII  <br></div><div class="valueClass" title ="' + (outputData.IsPII + '').toUpperCase() + '">' + (outputData.IsPII + '').toUpperCase() + '<br></div></div>';
        if (toShowEncrypted)
            html += '<div class="divEncrypted positionAlign"><div class ="labelClass">Is Encrypted  <br></div><div class="valueClass" title ="' + (outputData.IsEncrypted + '').toUpperCase() + '">' + (outputData.IsEncrypted + '').toUpperCase() + '<br></div></div>';
        if (toShowRestrictedChar)
            html += '<div class="divRestrictedChar positionAlign"><div class ="labelClass">Restricted Characters  <br></div><div class="valueClass" title ="' + GetEscapeSequence((outputData.RestrictedCharacters != null && outputData.RestrictedCharacters != "") ? outputData.RestrictedCharacters : 'Not Set') + '">' + ((outputData.RestrictedCharacters != null && outputData.RestrictedCharacters != "") ? GetEscapeSequence(outputData.RestrictedCharacters) : '<I>Not Set</I>') + '<br></div></div>';
        if (toShowDefaultValue)
            html += '<div class="divDefaultValue positionAlign"><div class ="labelClass">Default Value  <br></div><div class="valueClass" title ="' + GetEscapeSequence((outputData.DefaultValue != null && outputData.DefaultValue != "") ? outputData.DefaultValue : 'Not Set') + '">' + ((outputData.DefaultValue != null && outputData.DefaultValue != "") ? GetEscapeSequence(outputData.DefaultValue) : '<I>Not Set</I>') + '<br></div></div>';
        if (toShowPrimary)
            html += '<div class="divPrimary positionAlign"><div class ="labelClass">Is Primary  <br></div><div class="valueClass" title ="' + (outputData.IsPrimary == 1 ? 'TRUE' : 'FALSE') + '">' + (outputData.IsPrimary == 1 ? 'TRUE' : 'FALSE') + ' <br></div></div>';
        if (toShowTags)
            html += '<div class="divTags positionAlign"><div class ="labelClass">Tags  <br></div><div class="valueClass" title ="' + GetEscapeSequence((outputData.Tags != null && outputData.Tags != "") ? outputData.Tags : 'Not Set') + '">' + ((outputData.Tags != null && outputData.Tags != "") ? GetEscapeSequence(outputData.Tags) : '<I>Not Set</I>') + '<br></div></div>';
        html += '</div>';
        if (toShowAttributeDescription)
            html += '<div class = "divAttrDescription" style="width:100%;padding-left:5px;float: left;"><div class ="labelClass">Attribute Description  <br></div><div style="resize: vertical;outline: none;width: 100%;border: none;overflow: auto;white-space: pre-wrap;max-height: 150px;height: 30px;" class="valueClass" title ="' + GetEscapeSequence((outputData.AttributeDescription != null && outputData.AttributeDescription != '') ? outputData.AttributeDescription : 'Not Set') + '">' + ((outputData.AttributeDescription != null && outputData.AttributeDescription != '') ? GetEscapeSequence(outputData.AttributeDescription) : '<I>Not Set</I>') + '<br></div></div>';


        if (toShowVendorPriority)
            html += '<div class = "vendorInfoDiv" style="width:100%;padding-left:5px;float: left;"><div class ="labelClass">Vendor Priority  <br></div><div class="valueClass" title ="' + ((outputData.VendorPriority != null && outputData.VendorPriority != '') ? outputData.VendorPriority : 'Not Set') + '">' + ((outputData.VendorPriority != null && outputData.VendorPriority != '') ? outputData.VendorPriority : '<I>Not Set</I>') + '</div></div>';

        if (toShowI) {
            //if (srfpmAttributeMetaData.moduleId == 3)
            html += '<div class = "infoDiv" style="width:100%;"><i class="fa fa-info-circle iClickClass" onclick = "srfpmAttributeMetaData.callbackfunc()"></i></div>';
            //else
            //html += '<div class = "infoDiv" style="width:100%;"><input type="submit" class="viewAuditTSStyle" value="Click to View Audit and Time Series" onclick = "srfpmAttributeMetaData.callbackfunc()"/></div>';
        }

        html += '</div>';

        $("#" + srfpmAttributeMetaData.containerDiv).html(html);
        $("#" + srfpmAttributeMetaData.containerDiv).css('display', 'block');
        setPosition(srfpmAttributeMetaData.clickTarget);

        $(document).bind('click', docHandler);
        //$(".infoDiv").unbind("click").bind("click", srfpmAttributeMetaData.callbackForDetails);
    }

    SRFPMAttributeMetaData.prototype.callbackfunc = function callbackfunc() {
        $("#" + this.containerDiv).html("");
        $("#" + this.containerDiv).hide();
        $(document).unbind('click', docHandler);

        this.callbackForDetails(this.callbackObject);
    }

    function GetEscapeSequence(str) {
        var resultStr = str.replace(/&/g, '&amp;');
        resultStr = resultStr.replace(/</g, '&lt;');
        resultStr = resultStr.replace(/>/g, '&gt;');
        resultStr = resultStr.replace(/'/g, '&apos;');
        resultStr = resultStr.replace(/"/g, '&quot;');
        return resultStr;
    }

    function OnFailure(result) {
        console.log("Service returned an error" + ((result.responseJSON == null || result.responseJSON.Message == null || result.responseJSON.Message.length == 0) ? "" : (" : " + result.responseJSON.Message)) + ((result.responseText == null || result.responseText.length == 0) ? "" : (" : " + result.responseText)));
    }


    /////////////////////////////////
    // Call Common Service Methods //
    /////////////////////////////////
    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError, userContext, isCrossDomain) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, userContext, isCrossDomain);
    }

    return srfpmAttributeMetaData;
})();