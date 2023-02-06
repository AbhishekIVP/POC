
var attrModeler = (function () {
    function AttrModeler() {
        this._path = '';
    }
    AttrModeler.prototype.setPath = function setPath() {
        var path = window.location.protocol + '//' + window.location.host;
        var pathname = window.location.pathname.split('/');

        $.each(pathname, function (ii, ee) {
            if ((ii !== 0) && (ii !== pathname.length - 1))
                path = path + '/' + ee;
        });
        path = path.replace('Modeler', '');
        attrModeler._path = path;
    }

    AttrModeler.prototype.ajaxCall = function ajaxCall(methodName, parameters, onSuccess, onError, onComplete) {
        //onServiceUpdating();
        $("input").css('visibility', 'hidden')
        $.ajax({
            type: 'POST',
            url: attrModeler._path + 'Service/AttributeSetupPage.asmx/' + methodName,
            contentType: "application/json",
            dataType: "json",
            success: onSuccess,
            error: onError,
            complete: onComplete,
            data: JSON.stringify(parameters)
        });
    }

    var attrModeler = new AttrModeler();
    attrModeler.setPath();
    return attrModeler;
}());

self.onKeypress = function (obj, event) {
    var value = $(event.target).val() + event.key;
    var pattern = /[^0-9a-zA-Z ]/;
    if (pattern.test(value)) {
        return false;
    }
    else {
        return true;
    }
}

RenderData = function (data, moduleId) {
    var modelcontainer = {};
    modelcontainer.data = data;

    viewmodelobj = ko.mapping.fromJS(modelcontainer);
    viewmodelobj.deletedAttributes = ko.observableArray();
    viewmodelobj.newAttributes = ko.observableArray();
    viewmodelobj.moduleId = ko.observable(moduleId);
    //viewmodelobj.onClickDeleteAttrInfo = function (obj, event) {

    //    viewmodelobj.deletedAttributes.push(obj);
    //    viewmodelobj.data.remove(obj);

    //    event.stopPropagation();
    //}

    //viewmodelobj.onClickAddAttrInfo = function (obj, event) {
    //    var AttrNameEntered = $('#smAttribute_attrName').val();
    //    if (AttrNameEntered == '') {
    //        alert("Attribute Name field is empty. Please enter name.");
    //        return false;
    //    }
    //    var attrIsMandatory = $('#smAttribute_isMandatory').is(":checked");
    //    var attrIsCloneable = $('#smAttribute_isCloneable').is(":checked");
    //    var TagsInfo = $("#smAttribute_Tags").find(".hdnTag").val();
    //    var DataTypeSelected = smselect.getSelectedOption($("#smselect_" + "datatype"));
    //    if (DataTypeSelected.length == 0) {
    //        alert("Please select Data type.");
    //        return false;
    //    }
    //    else {
    //        var DataTypeInfo = DataTypeSelected[0].text;
    //    }

    //    if (moduleId == 3) {


    //        if (DataTypeInfo.toUpperCase() === "REFERENCE") {


    //            var RefTypeSelected = smselect.getSelectedOption($("#smselect_" + "reftype"));
    //            if (RefTypeSelected.length == 0) {
    //                alert("Please select Ref type.");
    //                return false;
    //            }
    //            else {
    //                var ReferenceTypeInfo = RefTypeSelected[0].text;
    //            }

    //            var RefAttrSelected = smselect.getSelectedOption($("#smselect_" + "refattr"));
    //            if (RefAttrSelected.length == 0) {
    //                alert("Please select Ref Attribute.");
    //                return false;
    //            }
    //            else {
    //                var ReferenceAttributeInfo = RefAttrSelected[0].text;
    //            }

    //            var RefLegAttrSelected = smselect.getSelectedOption($("#smselect_" + "reflegattr"));
    //            if (RefLegAttrSelected.length != 0) {
    //                var ReferenceLegAttributeInfo = RefLegAttrSelected[0].text;
    //            }


    //            var refDisplayAttributes = smselect.getSelectedOption($("#smselect_" + "refdispAttr"));
    //            if (refDisplayAttributes.length >= 1) {
    //                var attributeDisplayReferenceType = [];
    //                for (var i = 0; i < refDisplayAttributes.length; i++) {
    //                    attributeDisplayReferenceType.push(refDisplayAttributes[i].value);
    //                }
    //            }
    //        }
    //        var AttrDescriptionInfo = $('#smAttribute_attrDescription').val();
    //        var attrLength = null;
    //        if (DataTypeInfo.toUpperCase() === "STRING") {
    //            var stringAttrLength = $("#smAttribute_stringAttrLength").val();
    //            attrLength = stringAttrLength;

    //        }
    //        else if (DataTypeInfo.toUpperCase() === "NUMERIC") {
    //            var integerLength = $('#smAttribute_numericAttrLengthIntegerPart').val();
    //            var fractionalLength = $('#smAttribute_numericAttrLengthFractionPart').val();
    //            attrLength = integerLength + "." + fractionalLength;
    //        }
    //    }
    //    else if (moduleId != 3) {
    //        if (DataTypeInfo.toUpperCase() === "LOOKUP") {
    //            var LookupTypeSelected = smselect.getSelectedOption($("smselect_" + "lookuptype"));
    //            if (LookupTypeSelected.length == 0) {
    //                alert("Please select Lookup type.");
    //                return false;
    //            }
    //            else {
    //                var LookupTypeInfo = LookupTypeSelected[0].text;
    //            }
    //            var LookupAttributeSelected = smselect.getSelectedOption($("smselect_" + "lookupattr"));
    //            if (LookupAttributeSelected.length == 0) {
    //                alert("Please select Lookup Attribute.");
    //                return false;
    //            }
    //            else {
    //                var LookupAttributeInfo = LookupAttributeSelected[0].text;
    //            }
    //        }
    //        var attrIsEncrypted = $('#smAttribute_isEncrypted').is(":checked");
    //        var attrIsVisibleInSearch = $('#smAttribute_isVisibleInSearch').is(":checked");
    //        var defaultvalue = $('#defaultvalue').val();
    //        var searchviewposition = $('#searchviewpos').val();
    //        var restrictedchar = $('#restrictedchar').val();
    //        var attrLength = null;
    //        if (DataTypeInfo.toUpperCase() === "VARCHAR") {
    //            var stringAttrLength = $("#smAttribute_stringAttrLength").val();
    //            attrLength = stringAttrLength;

    //        }
    //        else if (DataTypeInfo.toUpperCase() === "DECIMAL") {
    //            var integerLength = $('#smAttribute_numericAttrLengthIntegerPart').val();
    //            var fractionalLength = $('#smAttribute_numericAttrLengthFractionPart').val();
    //            attrLength = integerLength + "." + fractionalLength;
    //        }

    //    }

    //    if ((DataTypeInfo.toUpperCase() != "REFERENCE") || (DataTypeInfo.toUpperCase() != "LOOKUP")|| (RefTypeSelected.length != 0 && RefAttrSelected.length != 0) || LookupTypeSelected.length != 0 && LookupAttributeSelected.length != 0) {
    //        var finalInfoObj = {
    //            AttrName: AttrNameEntered,
    //            DataType: DataTypeInfo,
    //            ReferenceType: (moduleId == 3) ? (typeof ReferenceTypeInfo === 'undefined' ? '' : ReferenceTypeInfo) : "",
    //            ReferenceAttribute: (moduleId == 3) ? (typeof ReferenceAttributeInfo === 'undefined' ? '' : ReferenceAttributeInfo) : "",
    //            ReferenceLegAttribute: (moduleId == 3) ? (typeof ReferenceLegAttributeInfo === 'undefined' ? '' : ReferenceLegAttributeInfo) : "",
    //            ReferenceDisplayAttribute: (moduleId == 3) ? (typeof attributeDisplayReferenceType === 'undefined' ? '' : attributeDisplayReferenceType.join(",")) : "",
    //            LookupType: (moduleId != 3) ? (typeof LookupTypeInfo === 'undefined' ? '' : LookupTypeInfo) : "",
    //            LookupAttribute: (moduleId != 3) ? (typeof LookupAttributeInfo === 'undefined' ? '' : LookupAttributeInfo) : "",
    //            DefaultValue: (moduleId != 3) ? defaultvalue : "",
    //            SearchViewPosition: (moduleId != 3) ? searchviewposition : "",
    //            Length: attrLength,
    //            Mandatory: attrIsMandatory,
    //            IsCloneable: attrIsCloneable,
    //            Encrypted: (moduleId != 3) ? attrIsEncrypted : "",
    //            VisibleInSearch: (moduleId != 3) ? attrIsVisibleInSearch : "",
    //            Tags: TagsInfo,
    //            AttrDescription: (moduleId == 3) ? AttrDescriptionInfo : "",
    //            RestrictedChar: (moduleId != 3) ? restrictedchar : ""
    //        }
    //        createIdentifiers(finalInfoObj, viewmodelobj);


    //        if (finalInfoObj.DataType.toUpperCase() === "STRING" || finalInfoObj.DataType.toUpperCase() === "NUMERIC" || finalInfoObj.DataType.toUpperCase() === "VARCHAR" || finalInfoObj.DataType.toUpperCase() === "DECIMAL") {
    //            if (finalInfoObj.Length == '0' || finalInfoObj.Length == '0.0') {
    //                alert("Please enter length.");
    //                return false;
    //            }
    //        }
    //        finalInfoObj = ko.mapping.fromJS(finalInfoObj);
    //        viewmodelobj.newAttributes.push(finalInfoObj);
    //        viewmodelobj.data.push(finalInfoObj);
    //        binddropdowns(finalInfoObj, moduleId);
    //    }


    //    AttrNameEntered = $('#smAttribute_attrName').val('');
    //    AttrDescriptionInfo = $('#smAttribute_attrDescription').val('');
    //    //if((moduleId==3)? (smselect.setOptionByText($("#smselect_" + "datatype"), "STRING")): (smselect.setOptionByText($("#smselect_" + "datatype"), "INT")));
    //    $('#smAttribute_isMandatory').attr("checked", false);
    //    $('#smAttribute_isCloneable').attr("checked", true);
    //    $("#smAttribute_Tags").find(".hdnTag").val('');

    //}

    for (var info in viewmodelobj.data()) {
        viewmodelobj.data()[info].identifier = info;

        viewmodelobj.data()[info].DataTypeIdentifier = viewmodelobj.data()[info].identifier + "_datatypedropdown";

        viewmodelobj.data()[info].ReferenceTypeIdentifier = viewmodelobj.data()[info].identifier + "_reftypedropdown";

        viewmodelobj.data()[info].ReferenceAttributeIdentifier = viewmodelobj.data()[info].identifier + "_refdropdown";

        viewmodelobj.data()[info].ReferenceDisplayIdentifier = viewmodelobj.data()[info].identifier + "_refdisplaydropdown";

        viewmodelobj.data()[info].ReferenceLegAttributeIdentifier = viewmodelobj.data()[info].identifier + "_reflegdropdown";

        viewmodelobj.data()[info].LookupTypeIdentifier = viewmodelobj.data()[info].identifier + "_lookuptypedropdown";

        viewmodelobj.data()[info].LookupAttributeIdentifier = viewmodelobj.data()[info].identifier + "_lookupattrdropdown";


    }
    ko.applyBindings(viewmodelobj, document.getElementById("smAttributeContainer"));

    for (var info in viewmodelobj.data()) {

        //"text":data(()[info].lookup(),"value":data(()[info].lookup())

        if (moduleId == 3) {
            //createSMSelectDropDown($('#' + viewmodelobj.data()[info].identifier + '_datatypedropdown'), availableDataTypes, false, "200px", null, "Data Type", null, [], false, null, "92px");
            //smselect.setOptionByValue($('#smselect_' + viewmodelobj.data()[info].identifier + '_datatypedropdown'), Dictionary_availableDataTypes[viewmodelobj.data()[info].DataType()]);
            //smselect.disable($('#' + viewmodelobj.data()[info].identifier + '_datatypedropdown'));
            createDisabledSMSelectLookAlike(viewmodelobj.data()[info].identifier + '_datatypedropdown', viewmodelobj.data()[info].DataType());
            //    if (viewmodelobj.data()[info].DataType().toUpperCase() === "STRING" || viewmodelobj.data()[info].DataType().toUpperCase() === "NUMERIC") {
            //        var stringAttrLength = $("#smAttribute_stringAttrLength").val();
            //        Length = stringAttrLength;

            //    }
            //    else if (DataTypeInfo.toUpperCase() === "NUMERIC") {
            //        var integerLength = $('#smAttribute_numericAttrLengthIntegerPart').val();
            //        var fractionalLength = $('#smAttribute_numericAttrLengthFractionPart').val();
            //        Length = integerLength + "." + fractionalLength;
            //    }
            //}
            if (viewmodelobj.data()[info].DataType().toUpperCase() === "REFERENCE") {
                //createSMSelectDropDown($('#' + viewmodelobj.data()[info].identifier + '_reftypedropdown'), [{ "text": viewmodelobj.data()[info].ReferenceType(), "value": viewmodelobj.data()[info].ReferenceType() }], false, "200px", null, "Reference Type", null, [], false, null, "85px");
                //smselect.setOptionByValue($('#smselect_' + viewmodelobj.data()[info].identifier + '_reftypedropdown'), viewmodelobj.data()[info].ReferenceType());
                //smselect.disable($('#' + viewmodelobj.data()[info].identifier + '_reftypedropdown'));
                createDisabledSMSelectLookAlike(viewmodelobj.data()[info].identifier + '_reftypedropdown', viewmodelobj.data()[info].ReferenceType());

                //createSMSelectDropDown($('#' + viewmodelobj.data()[info].identifier + '_refdropdown'), [{ "text": viewmodelobj.data()[info].ReferenceAttribute(), "value": viewmodelobj.data()[info].ReferenceAttribute() }], false, "200px", null, "Reference Attribute", null, [], false, null, "85px");
                //smselect.setOptionByValue($('#smselect_' + viewmodelobj.data()[info].identifier + '_refdropdown'), viewmodelobj.data()[info].ReferenceAttribute());
                //smselect.disable($('#' + viewmodelobj.data()[info].identifier + '_refdropdown'));
                createDisabledSMSelectLookAlike(viewmodelobj.data()[info].identifier + '_refdropdown', viewmodelobj.data()[info].ReferenceAttribute());

                //createSMSelectDropDown($('#' + viewmodelobj.data()[info].identifier + '_reflegdropdown'), [{
                //    "text": viewmodelobj.data()[info].ReferenceLegAttribute(), "value": viewmodelobj.data()[info].ReferenceLegAttribute()
                //}], false, "200px", null, "", null, [], false, null, "85px");
                //smselect.setOptionByValue($('#smselect_' + viewmodelobj.data()[info].identifier + '_reflegdropdown'), viewmodelobj.data()[info].ReferenceLegAttribute());
                //smselect.disable($('#' + viewmodelobj.data()[info].identifier + '_reflegdropdown'));

                //if (viewmodelobj.data()[info].referencedisplayattribute().split(',').length > 1) {
                //    var refdisplayarray = viewmodelobj.data()[info].referencedisplayattribute().split(',');
                //    var textvaluecollection = [];
                //    for (var item in refdisplayarray) {
                //        textvaluecollection.push({ text: refdisplayarray[item], value: refdisplayarray[item] });
                //    }
                //    createDisabledSMSelectLookAlikeDisplay(viewmodelobj.data()[info].identifier + '_refdisplaydropdown', textvaluecollection)
                //    //createsmselectdropdown($('#' + viewmodelobj.data()[info].identifier + '_refdisplaydropdown'), [{ "text": "reference attributes", "options": textvaluecollection }], false, "200px", null, "reference attributes", null, [], true, "reference  attributes", "80px");
                //}
                //else
                //createDisabledSMSelectLookAlikeDisplay(viewmodelobj.data()[info].identifier + '_refdisplaydropdown', viewmodelobj.data()[info].ReferenceDisplayAttribute());
                //createSMSelectDropDown($('#' + viewmodelobj.data()[info].identifier + '_refdisplaydropdown'), [{
                //    "text": "Reference Attributes", "options": [{ "text": viewmodelobj.data()[info].ReferenceDisplayAttribute(), "value": viewmodelobj.data()[info].ReferenceDisplayAttribute() }]
                //}], false, "200px", null, "Reference Attributes", null, [], true, "Reference  Attributes", "80px");
                if (viewmodelobj.data()[info].ReferenceDisplayAttribute().length > 0) {
                    var sectypeArr = viewmodelobj.data()[info].ReferenceDisplayAttribute().split(",");
                    //for (var item in sectypeArr) {
                    createDisabledSMSelectLookAlikeDisplay(viewmodelobj.data()[info].identifier + '_refdisplaydropdown', sectypeArr);
                    }
                    else
                    createDisabledSMSelectLookAlike(viewmodelobj.data()[info].identifier + '_refdisplaydropdown', viewmodelobj.data()[info].ReferenceDisplayAttribute());
                //smselect.setOptionByValue($("#smselect_" + viewmodelobj.data()[info].identifier + '_refdisplaydropdown'), sectypeArr[item]);
                //}
                //smselect.disable($('#' + viewmodelobj.data()[info].identifier + '_refdisplaydropdown'));
            }
            }
                //var tagLength=viewmodelobj.data()[info].Tags().length;
                //if (tagLength > 0) {
                //    for (var i = 0; i < tagLength ; i++) {
                //        viewmodelobj.data()[info].Tags += viewmodelobj.data()[info].Tags[i];
                //        if (i != tagLength - 1)
                    //            viewmodelobj.data()[info].Tags += ", ";
                    //    }
                    //}
                else if (moduleId != 3) {
                    //if (viewmodelobj.data()[info].ReferenceDisplayAttribute().length > 0) {
                    //    var sectypeArr = viewmodelobj.data()[info].ReferenceDisplayAttribute().split(",");
                    //}
                    createDisabledSMSelectLookAlike(viewmodelobj.data()[info].identifier + '_datatypedropdown', viewmodelobj.data()[info].DataType());
                    //createSMSelectDropDown($('#' + viewmodelobj.data()[info].identifier + '_datatypedropdown'), availableRefDataTypes, false, "200px", null, "Data Type", null, [], false, null, "95px");
                    //smselect.setOptionByValue($('#smselect_' + viewmodelobj.data()[info].identifier + '_datatypedropdown'), Dictionary_availableRefDataTypes[viewmodelobj.data()[info].DataType()]);
                        //smselect.disable($('#' + viewmodelobj.data()[info].identifier + '_datatypedropdown'));
                            if (viewmodelobj.data()[info].DataType().toUpperCase() === "LOOKUP") {
                            createDisabledSMSelectLookAlike(viewmodelobj.data()[info].identifier + '_lookuptypedropdown', viewmodelobj.data()[info].LookupType());
                        //createSMSelectDropDown($('#' + viewmodelobj.data()[info].identifier + '_lookuptypedropdown'), [{ "text": viewmodelobj.data()[info].LookupType(), "value": viewmodelobj.data()[info].LookupType() }], false, "200px", null, "Lookup Type", null, [], false, null, "95px");
                        //smselect.setOptionByValue($('#smselect_' + viewmodelobj.data()[info].identifier + '_lookuptypedropdown'), viewmodelobj.data()[info].LookupType());
                        //smselect.disable($('#' + viewmodelobj.data()[info].identifier + '_lookuptypedropdown'));

                            createDisabledSMSelectLookAlike(viewmodelobj.data()[info].identifier + '_lookupattrdropdown', viewmodelobj.data()[info].LookupAttribute());
                        //createSMSelectDropDown($('#' + viewmodelobj.data()[info].identifier + '_lookupattrdropdown'), [{ "text": viewmodelobj.data()[info].LookupAttribute(), "value": viewmodelobj.data()[info].LookupAttribute() }], false, "200px", null, "Lookup Attribute", null, [], false, null, "95px");
        //smselect.setOptionByValue($('#smselect_' + viewmodelobj.data()[info].identifier + '_lookupattrdropdown'), viewmodelobj.data()[info].LookupAttribute());
        //smselect.disable($('#' + viewmodelobj.data()[info].identifier + '_lookupattrdropdown'));
    }
    }

    }

        //var disptagdata = [];
        //disptagdata = viewmodelobj.data().Tags;


        //if (moduleId == 3) {
        //    createSMSelectDropDown($('#datatype'), availableDataTypes, false, "200px", null, "Data Type", onChangeDataTypeDropDown, [], false, null, "114px");
        //    createSMSelectDropDown($('#refdispAttr'), Data, false, "200px", null, "Reference Display Attributes", null, [], true, "Reference  Attributes", "114px");
        //    createSMSelectDropDown($('#reftype'), availableReferenceTypes, false, "200px", null, "Reference Type", null, [], false, null, "114px");
        //    createSMSelectDropDown($('#refattr'), availableReferenceAttributes, false, "200px", null, "Reference Attributes", null, [], false, null, "114px");
        //    createSMSelectDropDown($('#reflegattr'), availableReferenceLegAttributes, false, "200px", null, "Reference Leg Attributes", null, [], false, null, "114px");
        //}
        //else if (moduleId != 3) {
        //    createSMSelectDropDown($('#datatype'), availableRefDataTypes, false, "175px", null, "Data Type", onChangeDataTypeDropDown, [], false, null, "95px");
        //    createSMSelectDropDown($('#lookuptype'), availablelookuptype, false, "200px", null, "Lookup Type", null, [], false, null, "95px");
        //    createSMSelectDropDown($('#lookupattr'), availablelookupattr, false, "200px", null, "Lookup Attributes", null, [], false, null, "95px");
        //}

        //var disptag = new Object();
            //disptag.container = $("#smAttribute_Tags");
            //disptag.list = disptagdata;
        ////disptag.list = viewmodelobj.data().Tags;
        //tag.create(disptag);


        if (viewmodelobj.moduleId() == 3)
            $('#SecMasterHeader').css('display', 'block');
            else
            $('#RPFManagerHeader').css('display', 'block');
        $('#smAttributeContainer').find('*').prop('disabled', true);

        }
        


    var availableDataTypes =[{ "text": "STRING", "value": "STRING"
    }, { "text": "NUMERIC", "value": "NUMERIC"
    }, { "text": "DATETIME", "value": "DATETIME"
    }, { "text": "BOOLEAN", "value": "BOOLEAN"
    },
                               { "text": "REFERENCE", "value": "REFERENCE"
                               }, { "text": "FILE", "value": "FILE"
                               }, { "text": "STRING(MAX)", "value": "STRING(MAX)"
                               }, { "text": "TIME", "value": "TIME"
                               }, { "text": "DATE", "value": "DATE"
                               }];

var availableRefDataTypes =[{ "text": "VARCHAR", "value": "VARCHAR"
}, { "text": "INT", "value": "INT"
}, { "text": "DECIMAL", "value": "DECIMAL"
}, { "text": "DATETIME", "value": "DATETIME"
},
                                   { "text": "BIT", "value": "BIT"
                               }, { "text": "VARCHARMAX", "value": "VARCHARMAX"
                               }, { "text": "FILE", "value": "FILE"
                               }, { "text": "LOOKUP", "value": "LOOKUP"
                               }]


    var availableReferenceTypes =[{ "text": "COUNTRY", "value": "COUNTRY" }, { "text": "CODE", "value": "CODE" }, { "text": "CURRENCY", "value": "CURRENCY"
                               }];

var availableReferenceAttributes =[{ "text": "who", "value": "who"
}, { "text": "what", "value": "what"
}, { "text": "when", "value": "when"
}, { "text": "where", "value": "where"
}
                                        , { "text": "why", "value": "why"
}];


    var availableReferenceLegAttributes =[{ "text": "who", "value": "who"
    }, { "text": "what", "value": "what"
    }, { "text": "when", "value": "when"
    }, { "text": "where", "value": "where"
    }
                                        , { "text": "why", "value": "why"
                                        }];

var availablelookuptype =[{ "text": "Bloomberg Industry", "value": "Bloomberg Industry"
    }, { "text": "Country", "value": "Country"
    }, { "text": "Bloomberg Sector", "value": "Bloomberg Sector"
    }, { "text": "Compliance Restriction Type", "value": "Compliance Restriction Type"
    },
                               { "text": "Entity Type", "value": "Entity Type"
    }, { "text": "Issuer", "value": "Issuer"
                               }, { "text": "GICS Industry", "value": "GICS Industry"
                               }, { "text": "ICB Industry", "value": "ICB Industry"
                               },
                                   { "text": "Moodys Rating", "value": "Moodys Rating"
                            }, { "text": "Portfolio", "value": "Portfolio"
                            }, { "text": "MSCI Restriction Categories", "value": "MSCI Restriction Categories"
                            }, { "text": "SnP Rating", "value": "SnP Rating"
                            }, { "text": "Tier", "value": "Tier"
                            }];

var availablelookupattr =[{ "text": "Name", "value": "Name"
}, { "text": "ISO Code", "value": "ISO Code"
}, { "text": "Entity Type", "value": "Entity Type"
}, { "text": "Bloomberg Company ID", "value": "Bloomberg Company ID"
},
                               { "text": "Issuer Name", "value": "Issuer Name"
                               }, { "text": "Industry", "value": "Industry"
                               }, { "text": "Code", "value": "Code"
                               }, { "text": "MSCI Name", "value": "Code"
                               }, { "text": "Code", "value": "Code"
                               }];

var Dictionary_availablelookuptype = {
};
for (var key in availablelookuptype)
        Dictionary_availablelookuptype[availablelookuptype[key].text]= availablelookuptype[key].value;

    var Dictionary_availableDataTypes = {
    };
for (var key in availableDataTypes)
        Dictionary_availableDataTypes[availableDataTypes[key].text]= availableDataTypes[key].value;

    var Dictionary_availableRefDataTypes = {
    };
for (var key in availableRefDataTypes)
        Dictionary_availableRefDataTypes[availableRefDataTypes[key].text]= availableRefDataTypes[key].value;

    var Dictionary_availableReferenceTypes = {
};
for (var key in availableReferenceTypes)
        Dictionary_availableReferenceTypes[availableReferenceTypes[key].text]= availableReferenceTypes[key].value;

    var Dictionary_availableReferenceAttributes = {
    };
for (var key in availableReferenceAttributes)
        Dictionary_availableReferenceAttributes[availableReferenceAttributes[key].text]= availableReferenceAttributes[key].value;

    var Dictionary_availableReferenceLegAttributes = {
};
for (var key in availableReferenceLegAttributes)
        Dictionary_availableReferenceLegAttributes[availableReferenceLegAttributes[key].text]= availableReferenceLegAttributes[key].value;


    var attrRefDisp =[{ "text": "none", "value": "none"
        }, { "text": "do", "value": "do"
        }, { "text": "what", "value": "what"
        },
{ "text": "you", "value": "you"
                                                   }, { "text": "love", "value": "love"
                                                   }];
var Data =[{ "text": "Reference Display Attributes", "options": attrRefDisp
}];

var Dictionary_attrRefDisp = {
};
for (var key in attrRefDisp)
        Dictionary_attrRefDisp[attrRefDisp[key].text]= attrRefDisp[key].value;


    function createIdentifiers(currObj, viewModelObj) {
        var info = viewModelObj.data().length;
        currObj.identifier = info;
        currObj.AttrNameIdentifier = currObj.identifier + "_attrName";
        currObj.DataTypeIdentifier = currObj.identifier + "_datatypedropdown";
        currObj.LengthIdentifier = currObj.identifier + "_length";
        currObj.CloneIdentifier = currObj.identifier + "_clonable";
        currObj.MandateIdentifier = currObj.identifier + "_mandatory";
        currObj.TagsIdentifier = currObj.identifier + "_tags";
        currObj.ReferenceTypeIdentifier = currObj.identifier + "_reftypedropdown";
        currObj.ReferenceAttributeIdentifier = currObj.identifier + "_refdropdown";
        currObj.ReferenceDisplayIdentifier = currObj.identifier + "_refdisplaydropdown";
        currObj.ReferenceLegAttributeIdentifier = currObj.identifier + "_reflegdropdown";
        currObj.AttrDescriptionIdentifier = currObj.identifier + "_attrdescription";
        currObj.LookupTypeIdentifier = currObj.identifier + "_lookuptypedropdown";
        currObj.LookupAttrIdentifier = currObj.identifier + "_lookupattrdropdown";
        currObj.DefaultValueIdentifier = currObj.identifier + "_defaultvalue";
        currObj.SearchViewPosIdentifier = currObj.identifier + "_searchviewpos";
        currObj.EncryptedIdentifier = currObj.identifier + "_encrypted";
        currObj.VisibleInSearchIdentifier = currObj.identifier + "_visibleinserach";
        currObj.RestrictedCharIdentifier = currObj.identifier + "_restrictedchar";
        }



    $(document).ready(function () {

        var parameters = new Object();
        parameters.moduleId = parseInt($('#hdnmoduleId').val());
        parameters.EntityTypeId = parseInt($('#hdnEntityTypeId').val());
        parameters.isLeg = ($('#hdnIsLeg').val().toLowerCase() == 'true') ? true: false;
        parameters.isAdditionalLeg = ($('#hdnAdditionalLeg').val().toLowerCase() == 'true') ? true: false;
        parameters.TemplateId = parseInt($('#hdnTemplateId').val());
        //parameters.moduleId = 6;
        moduleId = parameters.moduleId;
        var onSuccess = function (data) {

            data = JSON.parse(data.d);
            RenderData(data, moduleId);

            }

        var onError = function onError() {

        }
        var onComplete = function onComplete() {
            $("input").css('visibility', 'visible');
            //onServiceUpdated();
        }
        var totalHeight = $(window).height();
        var totalwidth = $(window).width() -12;
        $(".smAttribute_headerSection").css('width', totalwidth);
            //$("#smAttributeContainer").css('width', totalwidth - 20);
            $("#smAttribute_addedattributescontainer").css('width', totalwidth);
            var offset = $("#smAttribute_addedattributescontainer").offset().top;
            $("#smAttribute_addedattributescontainer").css('height', totalHeight -49 -21);
            $('.smAttribute_attrInsertionRowStyle').css('width', totalwidth -15);
        if (moduleId == 3 || moduleId == 9) {
                //short widths

                let unit = totalwidth / (2 * 3 + ((11 - 3) * 3));
                $(".AttributeWidthAddRow").css('width', (3 * unit) -8);
                $(".AttributeWidthAddRowFixed").css('width', (2 * unit) -8);


                }
            else {
                let unit = totalwidth / (2 * 5 + ((13 - 5) * 3));
            $(".AttributeWidthAddRow").css('width', (3 * unit) -7);
            $(".AttributeWidthAddRowFixed").css('width', (2 * unit) -7);
            //$(".AttributeWidthAddRow").css('width', (totalwidth / 13) - 6);
            }
//$(".SecMasterData").css('width', totalwidth-100);
//$('.smAttributeContainer').css('margin-left', '40px');
attrModeler.ajaxCall('GetAttributeData', parameters, onSuccess, onError, onComplete);

});



createDisabledSMSelectLookAlike = function (dropDownId, selectedText) {
var a = "";
    //if (selectedText.length > 1)
a += '<div class="parentCustomSMSelectDiv"><div class="customSMSelectDiv" title="' + selectedText + '">' + selectedText + '</div></div>';
    //else
//    a = '<div class="parentCustomSMSelectDiv"><div class="customSMSelectDiv">' + selectedText + '</div></div>';
//if (selectedText.length() > 1)
    //    a += selectedText
    //+ '</div><i class="fa fa-sort-desc customSMSelectIcon"></i><div>';
    $('#' +dropDownId).html(a);
    }
createDisabledSMSelectLookAlikeDisplay = function (dropDownId, selectedText) {
    var title = "";
    //selectedText.reduce(x => title += x + ",");
    for (var i = 0; i < selectedText.length ; i++) {
        title += selectedText[i];
        if (i != selectedText.length -1)
            title += ", ";

        }


    var a = "";
    if (selectedText.length == 0) {
        a = "";
        }
        else if (selectedText.length > 1)
            a += '<div class="parentCustomSMSelectDiv"><div class="customSMSelectDiv" title="' + title + '">' +(selectedText[0]) + ' + ' +(selectedText.length -1) + ' other' + '</div></div>';
else
    a = '<div class="parentCustomSMSelectDiv"><div class="customSMSelectDiv" title="' + title + '">' +(selectedText[0]) + '</div></div>';
//if (selectedText.length() > 1)
//    a += selectedText
//+ '</div><i class="fa fa-sort-desc customSMSelectIcon"></i><div>';
$('#' +dropDownId).html(a);
}

createSMSelectDropDown = function (dropDownId, smdata, showSearch, width, callback, heading, onChangeHandler, selectedItems, isMulti, multiText, headerWidth) {

        var obj = {
};
        obj.container = dropDownId;
        obj.id = dropDownId.attr("id");
        if (heading !== null) {
    obj.heading = heading;
    }
obj.data = smdata;
if (showSearch) {
            obj.showSearch = true;
            }
if (isMulti) {
    obj.isMultiSelect = true;
    obj.text = multiText;
    }
if (selectedItems !== undefined && selectedItems.length !== 0) {
    obj.selectedItems = selectedItems;
    }
obj.isRunningText = false;
obj.ready = function (e) {
            e.width(width + "px");
            if (typeof onChangeHandler === "function") {
                e.on('change', function (ee) {
                    onChangeHandler(ee);
                });
                }
                }
smselect.create(obj);

$("#smselect_" +dropDownId.attr("id")).find(".smselectrun2").css('width', headerWidth);
$("#smselect_" +dropDownId.attr("id")).find(".smselectanchorcontainer2").css('width', '60%');

//if (dropDownId.selector == SMAdditionalLegsSetup.controls.smAdditionalLeg_dataType().selector) {
//    $("#smselect_" + dropDownId.attr("id")).find(".smselectanchorcontainer").css('width', '86%');

    //}

        if (typeof callback === "function") {
            callback();
        }
        };

onChangeDataTypeDropDown = function (e) {
        var divID = 'smAttribute_attributeLength';
        var dataTypeDivID = 'datatype';
        var selectedDataType = smselect.getSelectedOption($("#smselect_" +dataTypeDivID));
        var dataTypes = "";

        if (selectedDataType.length !== 0) {
            dataTypes = selectedDataType[0].text.toUpperCase();
        }
        else {
            dataTypes = "";
            }

        if (moduleId == 3) {
            createSMSelectDropDown($('#reftype'), [{ "text": "", "value": "" }], false, "200px", null, "Reference Type", null, [], false, null, "114px");
            smselect.disable($("#smselect_" + "reftype"));

            //Disable Reference Attribute DropDown
            createSMSelectDropDown($('#refattr'), [{ "text": "", "value": "" }], false, "200px", null, "Reference Attribute", null, [], false, null, "114px");
            smselect.disable($("#smselect_" + "refattr"));

            //Disable Reference leg Attributes
            createSMSelectDropDown($('#reflegattr'), [{ "text": "", "value": "" }], false, "200px", null, "Reference Type", null, [], false, null, "114px");
            smselect.disable($("#smselect_" + "reflegattr"));

            //Disable Reference Display Attribute DropDown
            createSMSelectDropDown($('#refdispAttr'), [{ "text": "", "value": "" }], false, "200px", null, "Reference Display Attribute", null, [], false, null, "114px");
            smselect.disable($("#smselect_" + "refdispAttr"));

            switch (dataTypes) {
                case "STRING":

                    $("#" +divID).find(".smAttribute_attrLengthInner").find("#numlength").hide();

                    $("#" +divID).find(".smAttribute_attrLengthInner").show();
                    $("#" +divID).find(".smAttribute_attrLengthInner").find("#stringlen").show();
                    break;
                case "STRING(MAX)":
                    $("#" +divID).find(".smAttribute_attrLengthInner").hide();
                    break;
                case "DATE":
                    $("#" +divID).find(".smAttribute_attrLengthInner").hide();
                    break;
                case "DATETIME":
                    $("#" +divID).find(".smAttribute_attrLengthInner").hide();
                    break;
                case "TIME":
                    $("#" +divID).find(".smAttribute_attrLengthInner").hide();
                    break;
                case "NUMERIC":
                    $("#" +divID).find(".smAttribute_attrLengthInner").find("#stringlen").hide();

                    $("#" +divID).find(".smAttribute_attrLengthInner").show();
                    $("#" +divID).find(".smAttribute_attrLengthInner").find("#numlength").show();
                    break;
                case "REFERENCE":
                    createSMSelectDropDown($('#refdispAttr'), Data, false, "200px", null, "Reference Display Attributes", null, [], true, "reference  attributes", "114px");
                    createSMSelectDropDown($('#reftype'), availableReferenceTypes, false, "200px", null, "Reference Type", null, [], false, null, "114px");
                    createSMSelectDropDown($('#refattr'), availableReferenceAttributes, false, "200px", null, "Reference Attributes", null, [], false, null, "114px");
                    createSMSelectDropDown($('#reflegattr'), availableReferenceLegAttributes, false, "200px", null, "Reference Leg Attributes", null, [], false, null, "114px");
                    break;

                case "FILE":
                    $("#" +divID).find(".smAttribute_attrLengthInner").hide();
                    break;


                case "BOOLEAN":
                    $("#" +divID).find(".smAttribute_attrLengthInner").hide();
                    break;

                default:
                    $("#" +divID).find(".smAttribute_attrLengthInner").hide();
                    break;
        }
        }
        else if (moduleId != 3) {
            createSMSelectDropDown($('#lookuptype'), [{ "text": "", "value": "" }], false, "300px", null, "Lookup Type", null, [], false, null, "200px");
            smselect.disable($("#smselect_" + "lookuptype"));

            createSMSelectDropDown($('#lookupattr'), [{ "text": "", "value": "" }], false, "300px", null, "Lookup Attributes", null, [], false, null, "200px");
            smselect.disable($("#smselect_" + "lookupattr"));

            switch (dataTypes) {
                case "VARCHAR":

                    $("#" +divID).find(".smAttribute_attrLengthInner").find("#numlength").hide();

                    $("#" +divID).find(".smAttribute_attrLengthInner").show();
                    $("#" +divID).find(".smAttribute_attrLengthInner").find("#stringlen").show();
                    break;
                case "VARCHARMAX":
                    $("#" +divID).find(".smAttribute_attrLengthInner").hide();
                    break;
                case "INT":
                    $("#" +divID).find(".smAttribute_attrLengthInner").hide();
                    break;
                case "DATETIME":
                    $("#" +divID).find(".smAttribute_attrLengthInner").hide();
                    break;
                case "BIT":
                    $("#" +divID).find(".smAttribute_attrLengthInner").hide();
                    break;
                case "DECIMAL":
                    $("#" +divID).find(".smAttribute_attrLengthInner").find("#stringlen").hide();

                    $("#" +divID).find(".smAttribute_attrLengthInner").show();
                    $("#" +divID).find(".smAttribute_attrLengthInner").find("#numlength").show();
                    break;
                case "LOOKUP":
                    createSMSelectDropDown($('#lookuptype'), availablelookuptype, false, "300px", null, "Lookup Type", null, [], false, null, "200px");
                    createSMSelectDropDown($('#lookupattr'), availableReferenceAttributes, false, "300px", null, "Lookup Attributes", null, [], false, null, "200px");
                    break;

                case "FILE":
                    $("#" +divID).find(".smAttribute_attrLengthInner").hide();
                        break;

                    default:
                    $("#" +divID).find(".smAttribute_attrLengthInner").hide();
                    break;
                    }
                    }
}

binddropdowns = function (currObj, moduleId) {
        if (moduleId == 3) {

            createSMSelectDropDown($('#' +currObj.DataTypeIdentifier()), availableDataTypes, false, "200px", null, "Data Type", null, [], false, null, "114px");
            smselect.setOptionByValue($('#smselect_' +currObj.DataTypeIdentifier()), Dictionary_availableDataTypes[currObj.DataType()]);

            if (currObj.DataType().toUpperCase() == "REFERENCE") {
                createSMSelectDropDown($('#' +currObj.ReferenceTypeIdentifier()), availableReferenceTypes, false, "200px", null, "Reference Type", null, [], false, null, "114px");
                smselect.setOptionByValue($('#smselect_' +currObj.ReferenceTypeIdentifier()), Dictionary_availableReferenceTypes[currObj.ReferenceType()]);

                createSMSelectDropDown($('#' +currObj.ReferenceAttributeIdentifier()), availableReferenceAttributes, false, "200px", null, "Reference Attribute", null, [], false, null, "114px");
                smselect.setOptionByValue($('#smselect_' +currObj.ReferenceAttributeIdentifier()), Dictionary_availableReferenceAttributes[currObj.ReferenceAttribute()]);

                createSMSelectDropDown($('#' +currObj.ReferenceLegAttributeIdentifier()), availableReferenceLegAttributes, false, "200px", null, "Reference Leg Attributes", null, [], false, null, "114px");
                smselect.setOptionByValue($('#smselect_' +currObj.ReferenceLegAttributeIdentifier()), Dictionary_availableReferenceLegAttributes[currObj.ReferenceLegAttribute()]);

                createSMSelectDropDown($('#' +currObj.ReferenceDisplayIdentifier()), Data, false, "200px", null, "Reference Display Attributes", null, [], true, "Reference  Attributes", "114px");
                var sectypeArr = currObj.ReferenceDisplayAttribute().split(",");
                    for (var item in sectypeArr) {

                    smselect.setOptionByValue($('#smselect_' +currObj.ReferenceDisplayIdentifier()), Dictionary_attrRefDisp[sectypeArr[item]]);
                    }
                    }
        }
        else if (moduleId != 3) {
            createSMSelectDropDown($('#' +currObj.DataTypeIdentifier()), availableRefDataTypes, false, "300px", null, "Data Type", null, [], false, null, "200px");
            smselect.setOptionByValue($('#smselect_' +viewmodelobj.data()[info].identifier + '_datatypedropdown'), Dictionary_availableRefDataTypes[viewmodelobj.data()[info].DataType()]);

            if (currObj.DataType().toUpperCase() == "LOOKUP") {
                createSMSelectDropDown($('#' +currObj.LookupTypeIdentifier()), availableReferenceTypes, false, "300px", null, "Lookup Type", null, [], false, null, "200px");
                smselect.setOptionByValue($('#smselect_' +currObj.LookupTypeIdentifier()), Dictionary_availablelookuptype[currObj.LookupType()]);

                createSMSelectDropDown($('#' +currObj.LookupAttrIdentifier()), availableReferenceTypes, false, "300px", null, "Lookup Attribute", null, [], false, null, "200px");
                smselect.setOptionByValue($('#smselect_' +currObj.LookupAttrIdentifier()), Dictionary_availableReferenceTypes[currObj.LookupAttribute()]);
}
}
}


//function UpdateInfo() {
//    var updatedData = {};
//    updatedData.updateAttributeInfo = [];
//    for (var info in viewmodelobj.data()) {
//        var tempdata = {};
//        tempdata.AttrName = viewmodelobj.data()[info].AttrName();
//        var DataTypeSelected = smselect.getSelectedOption($("#smselect_" + info + "_datatypedropdown"));
//        tempdata.DataType = DataTypeSelected[0].text;
//        tempdata.Mandatory = viewmodelobj.data()[info].Mandatory();
//        tempdata.IsCloneable = viewmodelobj.data()[info].IsCloneable();
//        tempdata.Tags = viewmodelobj.data()[info].Tags();

//        if (moduleId == 3) {
//            var RefTypeSelected = smselect.getSelectedOption($("#smselect_" + info + "_reftypedropdown"));
//            tempdata.ReferenceType = RefTypeSelected[0].text;

//            var RefAttrSelected = smselect.getSelectedOption($("#smselect_" + info + "_refdropdown"));
//            tempdata.ReferenceAttribute = RefAttrSelected[0].text;

//            var RefLegAttrSelected = smselect.getSelectedOption($("#smselect_" + info + "_reflegdropdown"));
//            tempdata.ReferenceLegAttribute = RefLegAttrSelected[0].text;


//            var refDisplayAttributes = smselect.getSelectedOption($("#smselect_" + info + "_refdisplaydropdown"));
//            if (refDisplayAttributes.length >= 1) {
//                var attributeDisplayReferenceType = [];
//                for (var i = 0; i < refDisplayAttributes.length; i++) {
//                    attributeDisplayReferenceType.push(refDisplayAttributes[i].text);
//                }
//            }

//            tempdata.ReferenceDisplayAttribute = attributeDisplayReferenceType.join(',');
//            tempdata.Length = viewmodelobj.data()[info].Length();


//            tempdata.AttrDescription = viewmodelobj.data()[info].AttrDescription();
//        }
//        else if (moduleId != 3) {
//            var LookupTypeSelected = smselect.getSelectedOption($("#smselect_" + info + "_lookuptypedropdown"));
//            tempdata.LookupType = LookupTypeSelected[0].text;

//            var LookupAttrSelected = smselect.getSelectedOption($("#smselect_" + info + "_lookupattrdropdown"));
//            tempdata.LookupAttribute = LookupAttrSelected[0].text;

//            tempdata.DefaultValue = viewmodelobj.data()[info].DefaultValue();
//            tempdata.SearchViewPosition = viewmodelobj.data()[info].SearchViewPosition();
//            tempdata.RestrictedChar = viewmodelobj.data()[info].RestrictedChar();
//            tempdata.Encrypted = viewmodelobj.data()[info].Encrypted();
//            tempdata.VisibleInSearch = viewmodelobj.data()[info].VisibleInSearch();
//        }
//        updatedData.updateAttributeInfo.push(tempdata);
//    }
//}

//function RedirectToLegAttribute() {
//    window.location.href = "./AttributeLegSetupPage.aspx";
//}