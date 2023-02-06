

//function for RExtGridView
function TglRow(ctl)
{
	var row = ctl.parentNode.parentNode;
	var tbl = row.parentNode;
	var crow = tbl.rows[row.rowIndex + 1];
	var ihExp = ctl.parentNode.getElementsByTagName('input').item(0);

	tbl = tbl.parentNode;

	var expandClass = tbl.attributes.getNamedItem('expandClass').value;
	var collapseClass = tbl.attributes.getNamedItem('collapseClass').value;
	var expandText = tbl.attributes.getNamedItem('expandText').value;
	var collapseText = tbl.attributes.getNamedItem('collapseText').value;

	
	if (crow.style.display == 'none')
	{
		crow.style.display = '';
		ctl.innerHTML = collapseText;
		ctl.className = collapseClass;
		ctl.title = "Click to collapse";
		ihExp.value = '1';
	}
	else
	{
		crow.style.display = 'none';
		ctl.innerHTML = expandText;
		ctl.className = expandClass;
		ctl.title = "Click to expand.";
		ihExp.value = '';
	}
}

function FillGridResolutionTextBox(chk,idGridResCode,idGridRemarks,idDdlResCode,idRemarks)
    {
        var ddl = document.getElementById(idDdlResCode);
        var txtGridResCode = document.getElementById(idGridResCode);
        var txtGridRemarks = document.getElementById(idGridRemarks);
        var txtRemarks = document.getElementById(idRemarks);
        var gridCheckBox = document.getElementById(chk);
        if(gridCheckBox.checked)
        {
            txtGridRemarks.value += txtRemarks.value;
            	for (var i = 0; i < ddl.options.length; i++)
	            {
		            if(ddl.options[i].selected)
		            {
			            txtGridResCode.value = ddl.options[i].text;
			            return;
		            }
	            }
        }       
        else
       {
            txtGridRemarks.value = "";
            txtGridResCode.value = "";
       } 
     
    }
function showHide(chk,txt,txtDefault)
    {
        var textBox = document.getElementById(txt);
        var textBoxDefault = document.getElementById(txtDefault);
        var checkBox = document.getElementById(chk);
        if(checkBox.checked)
        {            
            textBox.readOnly = false;                        
            //alert("HI i m checked");
        }
        else
        {
            textBox.readOnly = true;
            textBox.value = textBoxDefault.value;
            //alert("Hi i m unchecked");
        }
    }
    
   
    function EnableDisableTextBox(chk,txt)
    {
        var textBox = document.getElementById(txt);
        var checkBox = document.getElementById(chk);        
        if(checkBox.checked)
        {            
            textBox.readOnly = false;
            textBox.focus();
        }
        else
        {
            textBox.readOnly = true;
        }
    }
    
    function SetGridCommand(commandName)
    {
        document.getElementById('commandName').value = commandName;        
    }
    function SetGridCommandWithId(commandName,object)
    {
        document.getElementById('commandName').value = commandName;        
        document.getElementById('id').value = object.value;        
    }
    
function __showContextMenu(menu, rowID,columnData)
{
	var menuOffset = 2;
    menu.style.left = window.event.x - menuOffset;
    menu.style.top = window.event.y - menuOffset;
    menu.style.display = "";
 
    var _rowID = document.getElementById('__ROWID');
    if (_rowID != null)
    _rowID.value = rowID;
    
    var _columnData = document.getElementById('__DATA');
    if(_columnData != null)
    _columnData.value = columnData;
    
    window.event.cancelBubble = true;
    return false;
}

function __trapESC(menu)
{
	var key = window.event.keyCode;
	if (key == 27)
	{
		menu.style.display = 'none';
	}
}

function __CheckAll(me)
{
    var index = me.name.indexOf('_');  
    var prefix = me.name.substr(0,index); 
    for(i=0; i<document.forms[0].length; i++) 
    { 
        var o = document.forms[0][i]; 
        if (o.type == 'checkbox') 
        { 
            if (me.name != o.name) 
            {
                if (o.name.substring(0, prefix.length) == prefix) 
                {
                    // Must be this way
                    o.checked = !me.checked; 
                    o.click(); 
                }
            }
        } 
    } 
}

function ApplyStyle(me, selectedForeColor, selectedBackColor, foreColor, backColor, bold, checkBoxHeaderId) 
{ 
    var td = me.parentNode; 
    if (td == null) 
        return; 
        
    var tr = td.parentNode;
    if (me.checked)
    { 
       tr.style.fontWeight = 700; // bold
       tr.style.color = selectedForeColor; 
       tr.style.backgroundColor = selectedBackColor; 
    } 
    else 
    { 
       document.getElementById(checkBoxHeaderId).checked = false;
       tr.style.fontWeight = bold; 
       tr.style.color = foreColor; 
       tr.style.backgroundColor = backColor; 
    } 
}

var mouseOutColor;
function __MouseOverRow(source, mouseOverColor)
{
	mouseOutColor = source.style.backgroundColor;
	source.style.backgroundColor = mouseOverColor;
}

function __MouseOutRow(source)
{
	source.style.backgroundColor = mouseOutColor;
}



function changeTextStyle(checkId, rowId){
    var checkBox = document.getElementById(checkId);
    var row = document.getElementById(rowId);
    if(checkBox.checked)
        row.className = 'gridDataBold';
    else 
        row.className = 'dataRow';
}