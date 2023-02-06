var SRFPMInputObjectControl = (function(){
    function srfpmInputObjectControl(){
        this.counter = 0;
        this.internalData_ListsOfItems = [];
        this.clickedKnockoutObject = null;
        //Structure of each item in internalData_ListsOfItems is of type - {internalInstanceID : abc, internalListOfItems : xyz}
    }    

    var SRFPMInputObjectControl = new srfpmInputObjectControl();


    /////////////////////
    ////init Function////
    /////////////////////

    //Entry Point into SRFPMInputObjectControl
    srfpmInputObjectControl.prototype.init = function($object){
        if($object !== undefined){
            
            var instanceId = "srfpmInputObjectControl_" + ($object.hasOwnProperty('id') ? $object.id : SRFPMInputObjectControl.counter++); 
            
            $object.internalInstanceID = instanceId;

            if(!$object.hasOwnProperty('container')){
                $object.container = "body";
            }
            
            //Add Main Container Div to the Provided Div
            var htmlMain = "<div id='" + $object.internalInstanceID + "' class='SRFPMInputObjectControlMainClass'></div><input id='hdn_" + $object.internalInstanceID +"_id' type='hidden' />";
            $($object.container).append(htmlMain);
            //Hide Parent Div for now
            $("#" + $object.internalInstanceID).css("display", "none");

            if($object.hasOwnProperty('type') && $object.type !== typeof(string)){
                if($object.hasOwnProperty('data')){
                    if($object.type == "MS"){
                        createMultiSelect($object);
                    }
                    else if($object.type == "SS"){
                        createSingleSelect($object);
                    }
                    else if($object.type == "IB"){
                        createInputBox($object);
                    }
                    else if($object.type == "CB"){
                        createCheckBox($object);
                    }
                }
                else{
                    console.log("SRFPMInputObjectControl - Please specify data to create Input Object");
                }
            }
            else{
                console.log("SRFPMInputObjectControl - Please specify a Type of Input for " + $object.id);
            }
        }
    }


    ////////////////////////
    ////CREATE Functions////
    ////////////////////////

    //Function to Create Multi Select DropDown
    function createMultiSelect($object){
        //Check if exists with Same $object.id. If exists, then delete it.
        var checkMultiSelectID = "smselect_" + $object.id;
        if($("#" + checkMultiSelectID).length > 0){
            $("#" + $object.internalInstanceID).html("");
        }
        
        //Doc Handler
        var SRFPMInputObjectControl_MS_docHandler =  function(e){
            //Call the actual function
            internalDocHandler_SRFPMInputObjectControl_MS($object, e);            
        }

        //Attach Doc Handler
        $(document).unbind('click', SRFPMInputObjectControl_MS_docHandler).click(SRFPMInputObjectControl_MS_docHandler);
        
        //Prepare data to create multiSelect
        var isRunningText = $object.data.hasOwnProperty('isRunningText') ? $object.data.isRunningText : false;
        var listOfItems = $object.data.hasOwnProperty('listOfItems') ? $object.data.listOfItems : [{text: "Please give atleast one item", value: "Please give atleast one item"}];
        var listOfSelectedItems = $object.data.hasOwnProperty('listOfSelectedItems') ? $object.data.listOfSelectedItems : [];
        var isSearch = $object.data.hasOwnProperty('isSearch') ? $object.data.isSearch : true;
        var displayText = $object.data.hasOwnProperty('displayText') ? $object.data.displayText : "Select Items";
        var container = "#" + $object.internalInstanceID;
        var allSelected = $object.data.hasOwnProperty('allSelected') ? $object.data.allSelected : false;

        //Save this object.
        var internalFound = false;
        for (var item in SRFPMInputObjectControl.internalData_ListsOfItems) {
            if (SRFPMInputObjectControl.internalData_ListsOfItems[item].id == $object.id) {
                SRFPMInputObjectControl.internalData_ListsOfItems[item].data = $object;
                internalFound = true;
                break;
            }
        }
        if (!internalFound) {
            SRFPMInputObjectControl.internalData_ListsOfItems.push({ id: $object.id, data: $object });
        }

        ApplyMultiSelect($object.id, isRunningText, listOfItems, isSearch, container, displayText, allSelected, listOfSelectedItems);
    }

    //Function to Create Single Select DropDown
    function createSingleSelect($object){
        //Check if exists with Same $object.id. If exists, then delete it.
        var checkSingleSelectID = "smselect_" + $object.id;
        if($("#" + checkSingleSelectID).length > 0){
            $("#" + $object.internalInstanceID).html("");
        }
        
        //Doc Handler
        var SRFPMInputObjectControl_SS_docHandler = function (e) {
            //Call the actual function
            internalDocHandler_SRFPMInputObjectControl_SS($object, e);
        }

        //Attach Doc Handler
        $(document).unbind('click', SRFPMInputObjectControl_SS_docHandler).click(SRFPMInputObjectControl_SS_docHandler);
        
        //Prepare data to create singleSelect
        var isRunningText = $object.data.hasOwnProperty('isRunningText') ? $object.data.isRunningText : false;
        var listOfItems = $object.data.hasOwnProperty('listOfItems') ? $object.data.listOfItems : [{text: "Please give atleast one item", value: "Please give atleast one item"}];
        var selectedItem = $object.data.hasOwnProperty('selectedItem') ? $object.data.selectedItem : [];
        var isSearch = $object.data.hasOwnProperty('isSearch') ? $object.data.isSearch : true;
        var displayText = $object.data.hasOwnProperty('displayText') ? $object.data.displayText : "Select Item";
        var container = "#" + $object.internalInstanceID;
        
        //Save this object.
        var internalFound = false;
        for (var item in SRFPMInputObjectControl.internalData_ListsOfItems) {
            if (SRFPMInputObjectControl.internalData_ListsOfItems[item].id == $object.id) {
                SRFPMInputObjectControl.internalData_ListsOfItems[item].data = $object;
                internalFound = true;
                break;
            }
        }
        if (!internalFound) {
            SRFPMInputObjectControl.internalData_ListsOfItems.push({ id: $object.id, data: $object });
        }

        ApplySMSelect($object.id, isRunningText, listOfItems, isSearch, container, selectedItem);
    }

    //Function to Create Input Box - INCOMPLETE
    function createInputBox($object){
        //INCOMPLETE
    }
    
    //Function to Create Check Box - INCOMPLETE
    function createCheckBox($object){
        //INCOMPLETE
    }


    //////////////////////
    ////SHOW Functions////
    //////////////////////

    //Function to show Input Object
    srfpmInputObjectControl.prototype.showInputObject = function ($event, $dispObject, $dataObject){
        //Hide any SRFPMInputObjectControl, that has been initialised and is visible
        for (var item in SRFPMInputObjectControl.internalData_ListsOfItems) {
            var $tempObjectForDocHandler = SRFPMInputObjectControl.internalData_ListsOfItems[item].data;

            if ($("#" + $tempObjectForDocHandler.internalInstanceID + ":visible").length > 0) {             //Check if Visible

                //Call Appropriate Doc Handler based on Type
                if ($tempObjectForDocHandler.type == "MS") {
                    internalDocHandler_SRFPMInputObjectControl_MS($tempObjectForDocHandler, $event);
                }
                else if ($tempObjectForDocHandler.type == "SS") {
                    internalDocHandler_SRFPMInputObjectControl_SS($tempObjectForDocHandler, $event);
                }
                else if ($tempObjectForDocHandler.type == "IB") {
                    internalDocHandler_SRFPMInputObjectControl_IB($tempObjectForDocHandler, $event);
                }
                else if ($tempObjectForDocHandler.type == "CB") {
                    internalDocHandler_SRFPMInputObjectControl_CB($tempObjectForDocHandler, $event);
                }
            }
        }


        if ($dispObject !== undefined) {
            var dispObjID = $dispObject.hasOwnProperty('inputControlID') ? $dispObject.inputControlID : "";
             
            var $internalObject = {};

            for(var item in SRFPMInputObjectControl.internalData_ListsOfItems){
                if(SRFPMInputObjectControl.internalData_ListsOfItems[item].id == dispObjID){
                    $internalObject = SRFPMInputObjectControl.internalData_ListsOfItems[item].data;
                    break;
                }
            }

            //Set id_of_clicked_object in Hidden Input, so that it can be passed to the callbackFunction later.
            var id_of_clicked_object = $event.currentTarget.id;
            $("#hdn_" + $internalObject.internalInstanceID +"_id").val(id_of_clicked_object);

            if(typeof $dataObject != typeof undefined){
                SRFPMInputObjectControl.clickedKnockoutObject = $dataObject;
            }

            if(!$internalObject.hasOwnProperty('type')){
                console.log("SRFPMInputObjectControl - Please specify a valid inputControlID OR initialise the said inputControlID first.");
                return false;
            }

            if($internalObject.type == "MS"){
                showMultiSelect($dataObject, $event, $dispObject, $internalObject);
            }
            else if($internalObject.type == "SS"){
                showSingleSelect($dataObject, $event, $dispObject, $internalObject);
            }
            else if($internalObject.type == "IB"){
                showInputBox($dataObject, $event, $dispObject, $internalObject);
            }
            else if($internalObject.type == "CB"){
                showCheckBox($dataObject, $event, $dispObject, $internalObject);
            }

            //SHOW Parent Div
            $("#" + $internalObject.internalInstanceID).css("display", "block");     

            event.stopPropagation();        
        }
    } 

    //Function to Show Multi Select DropDown
    function showMultiSelect($dataObject, $event, $dispObject, $internalObject){
        
        //Assumed, that callbackFunctionDelegate will be given during init only.
        //if($dispObject.hasOwnProperty('callbackFunctionDelegate')){
        //    $internalObject.callbackFunctionDelegate = $dispObject.callbackFunctionDelegate;
        //}
        
        if($dispObject.hasOwnProperty('data')){
            //If we need to create again.
            var createAgain = false;
            if($dispObject.data.hasOwnProperty('listOfItems')){
                $internalObject.data.listOfItems = $dispObject.data.listOfItems;
                createAgain = true;
            }

            if($dispObject.data.hasOwnProperty('listOfSelectedItems')){
                $internalObject.data.listOfSelectedItems = $dispObject.data.listOfSelectedItems;
            }
            else{
                $internalObject.data.listOfSelectedItems = [];
            }

            if(createAgain){
                createMultiSelect($internalObject);
            }
            else{
                //Make the selection
                var targetSMSelect = $("#smselect_" + $internalObject.id);
                smselect.reset(targetSMSelect, true);

                for(var item in $internalObject.data.listOfSelectedItems){
                    smselect.setOptionByText($internalObject.data.listOfSelectedItems[item].text);
                }
            }           
        }
        else{
            //If no data is passed in $dispObject.data
            $internalObject.data.listOfSelectedItems = [];

            //Make the selection
            var targetSMSelect = $("#smselect_" + $internalObject.id);
            smselect.reset(targetSMSelect, true);

            for(var item in $internalObject.data.listOfSelectedItems){
                smselect.setOptionByText($internalObject.data.listOfSelectedItems[item].text);
            }
        }

        //CSS for the internal Main Div.
        //$("#" + $internalObject.internalInstanceID).css({"width" : "fit-content"});
        var top_left_height_width = calculateCSS($event, $dispObject, $internalObject);
        $("#" + $internalObject.internalInstanceID).css({"position" : "absolute", "background" : "white", "z-index":"123123123",  "top": top_left_height_width.top, "left": top_left_height_width.left, "height": top_left_height_width.height, "width": top_left_height_width.width});
        $("#smselect_" + $internalObject.id).css({"height": top_left_height_width.height, "width": top_left_height_width.width});
    }

    //Function to Show Single Select DropDown
    function showSingleSelect($dataObject, $event, $dispObject, $internalObject) {

        //Assumed, that callbackFunctionDelegate will be given during init only.
        //if($dispObject.hasOwnProperty('callbackFunctionDelegate')){
        //    $internalObject.callbackFunctionDelegate = $dispObject.callbackFunctionDelegate;
        //}

        if($dispObject.hasOwnProperty('data')){
            //If we need to create again.
            var createAgain = false;
            if($dispObject.data.hasOwnProperty('listOfItems')){
                $internalObject.data.listOfItems = $dispObject.data.listOfItems;
                createAgain = true;
            }

            if($dispObject.data.hasOwnProperty('selectedItem')){
                $internalObject.data.selectedItem = $dispObject.data.selectedItem;
            }
            else{
                $internalObject.data.selectedItem = [];
            }

            if(createAgain){
                createSingleSelect($internalObject);
            }
            else{
                //Make the selection
                var targetSMSelect = $("#smselect_" + $internalObject.id);
                smselect.reset(targetSMSelect, true);

                if(typeof $internalObject.data.selectedItem.text !== "undefined" ){
                    smselect.setOptionByText($internalObject.data.selectedItem.text);
                }
            }           
        }
        else{
            //If no data is passed in $dispObject.data
            $internalObject.data.selectedItem = [];

            //Make the selection
            var targetSMSelect = $("#smselect_" + $internalObject.id);
            smselect.reset(targetSMSelect, true);

            for(var item in $internalObject.data.selectedItem){
                smselect.setOptionByText($internalObject.data.selectedItem.text);
            }
        }

        //CSS for the internal Main Div.
        //$("#" + $internalObject.internalInstanceID).css({"width" : "fit-content"});
        var top_left_height_width = calculateCSS($event, $dispObject, $internalObject);
        $("#" + $internalObject.internalInstanceID).css({"position" : "absolute", "background" : "white", "z-index":"123123123",  "top": top_left_height_width.top, "left": top_left_height_width.left, "height": top_left_height_width.height, "width": top_left_height_width.width});
        $("#smselect_" + $internalObject.id).css({"height": top_left_height_width.height, "width": top_left_height_width.width});
    }

    //Function to Show Input Box - INCOMPLETE
    function showInputBox($dataObject, $event, $dispObject, $internalObject){
        //INCOMPLETE
    }
    
    //Function to Show Check Box - INCOMPLETE
    function showCheckBox($dataObject, $event, $dispObject, $internalObject){
        //INCOMPLETE
    }


    /////////////////////
    ////Calculate CSS////
    /////////////////////

    //Function calculates the CSS for the Main Div
    function calculateCSS($event, $dispObject, $internalObject){
        var result = {};

        var top = 0, left = 0, height = 0, width = 0;

        var objectTop = $($event.currentTarget).offset().top;
        var objectLeft = $($event.currentTarget).offset().left;
        var objectHeight = $($event.currentTarget).height();
        var objectWidth = $($event.currentTarget).width();

        var windowHeight = window.innerHeight;
        var windowWidth = window.innerWidth;

        // var internalHeight = $("#" + $internalObject.internalInstanceID).height();
        // var internalWidth = $("#" + $internalObject.internalInstanceID).width();
        

        left = objectLeft;
        top = objectTop;
        height = objectHeight;
        width = objectWidth;

        // if(objectLeft + internalWidth + 10 >= windowWidth){
        //     left = windowWidth - internalWidth - 10;
        // }
        // else{
        //     left = objectLeft;
        // }
        // if(objectTop + internalHeight + 5 > windowHeight){
        //     top = windowHeight - internalHeight - 5;
        // }
        // else{
        //     top = objectTop + objectHeight;
        // }

        result.top = top;
        result.left = left;
        result.height = height;
        result.width = width;

        return result;
    }


    ////////////////////
    ////Doc Handlers////
    ////////////////////

    //Doc Handler for Multi Select Dropdown
    function internalDocHandler_SRFPMInputObjectControl_MS($object, e) {
        var id_of_clicked_object = $("#hdn_" + $object.internalInstanceID + "_id").val();

        if (e.target.id != id_of_clicked_object && $("#" + $object.internalInstanceID + ":visible").length > 0) {

            var targetSMSelect = $("#smselect_" + $object.id);

            var dataFinal = smselect.getSelectedOption(targetSMSelect);

            var callbackFunctionDelegate = $object.hasOwnProperty('callbackFunctionDelegate') ? $object.callbackFunctionDelegate : "";

            //Hide Parent Div for now
            $("#" + $object.internalInstanceID).css("display", "none");

            //Pass data back to Callback Function
            if (callbackFunctionDelegate !== "") {
                if (SRFPMInputObjectControl.clickedKnockoutObject != null) {
                    callbackFunctionDelegate({ "data": dataFinal, "id": id_of_clicked_object, "obj": SRFPMInputObjectControl.clickedKnockoutObject });

                    SRFPMInputObjectControl.clickedKnockoutObject = null;
                }
                else {
                    callbackFunctionDelegate({ "data": dataFinal, "id": id_of_clicked_object });
                }
            }
        }
    }

    //Doc Handler for Single Select Dropdown
    function internalDocHandler_SRFPMInputObjectControl_SS($object, e) {
        var id_of_clicked_object = $("#hdn_" + $object.internalInstanceID + "_id").val();

        if (e.target.id != id_of_clicked_object && $("#" + $object.internalInstanceID + ":visible").length > 0) {

            var targetSMSelect = $("#smselect_" + $object.id);

            var dataFinal = smselect.getSelectedOption(targetSMSelect);

            var callbackFunctionDelegate = $object.hasOwnProperty('callbackFunctionDelegate') ? $object.callbackFunctionDelegate : "";

            //Hide Parent Div for now
            $("#" + $object.internalInstanceID).css("display", "none");

            //Pass data back to Callback Function
            if (callbackFunctionDelegate !== "") {
                if (SRFPMInputObjectControl.clickedKnockoutObject != null) {
                    callbackFunctionDelegate({ "data": dataFinal, "id": id_of_clicked_object, "obj": SRFPMInputObjectControl.clickedKnockoutObject });

                    SRFPMInputObjectControl.clickedKnockoutObject = null;
                }
                else {
                    callbackFunctionDelegate({ "data": dataFinal, "id": id_of_clicked_object });
                }
            }
        }
    }

    //Doc Handler for Input Box
    function internalDocHandler_SRFPMInputObjectControl_IB($object, e) {

    }

    //Doc Handler for Check Box
    function internalDocHandler_SRFPMInputObjectControl_CB($object, e) {

    }


    //Utility Functions
    function ApplyMultiSelect(id, isRunningText, data, isSearch, container, text, allSelected, selectedItemsList) {
        var selectedItems = [];

        //If no data provided
        if (data.length == 0) {
            var tempObj = {};
            tempObj.text = "None available";
            tempObj.value = "None available";
            data.push(tempObj);
        }

        if (allSelected) {
            $.each(data, function (key, selectedItemObj) {
                selectedItems.push(selectedItemObj.text);
            });
        }
        else {
            if (typeof (selectedItemsList) != 'undefined' && selectedItemsList.length > 0) {
                $.each(selectedItemsList, function (key, selectedItemObj) {
                    selectedItems.push(selectedItemObj.text);
                });
            }
        }

        smselect.create(
        {
            id: id,
            text: text,
            container: $(container),
            isMultiSelect: true,
            isRunningText: isRunningText,
            data: [{ options: data, text: text }],
            selectedItems: selectedItems,
            showSearch: isSearch,
            ready: function (selectelement) {
                selectelement.css({ /*'border': '1px solid #CECECE',*/ 'border-left': 'none', height: '22px', width: '200px', 'vertical-align': 'middle' });
                selectelement.on('change', function (ee) {
                    ddlOnChangeHandler(ee);
                });
            }
        });
    }

    function ApplySMSelect(id, isRunningText, data, isSearch, container, selectedItem) {

        smselect.create(
        {
            id: id,
            container: $(container),
            isRunningText: isRunningText,
            data: data,
            selectedText: typeof selectedItem != typeof undefined && typeof selectedItem.text != typeof undefined ? selectedItem.text : "",
            showSearch: isSearch,
            ready: function (selectelement) {
                selectelement.css({ /*'border': '1px solid #CECECE',*/ 'border-left': 'none', height: '22px', width: '200px', 'vertical-align': 'middle' });
                selectelement.on('change', function (ee) {
                    ddlOnChangeHandler(ee);
                });

                smselect.setOptionByIndex(selectelement, 0);
            }
        });
    }

    function ddlOnChangeHandler(ee){

    }

    return SRFPMInputObjectControl;
})();