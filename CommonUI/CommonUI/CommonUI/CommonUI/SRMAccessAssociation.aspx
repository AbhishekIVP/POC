<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMAccessAssociation.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMAccessAssociation" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link rel="stylesheet" type="text/css" href="https://fonts.google.com/specimen/Oswald?selection.family=Oswald" />  
    <link href="css/thirdparty/bootstrap.min.css" rel="stylesheet" />
    <link href="css/SRMAssociation.css" rel="stylesheet" />
      
    <title></title>
     <style>
      
    </style>
</head>
<body>
    <form id="form1" style="height:100%;" runat="server">
             <asp:ScriptManager ID="smMaster" runat="server" ScriptMode="Release" EnableViewState="true"
            AllowCustomErrorsRedirect="false" OnAsyncPostBackError="smMaster_AsyncPostBackError"
            AsyncPostBackTimeout="20000" EnablePageMethods="true" LoadScriptsBeforeUI="true">
            <Services>
                <asp:ServiceReference Path="~/CommonUI/BaseUserControls/Service/RADNeoGridService.svc" />
            </Services>
            <Scripts>
                <asp:ScriptReference Path="ScriptIncludes/SecMasterJSCommon.debug.js" />
                <asp:ScriptReference Path="includes/RADBrowserScripts.debug.js" />
                <asp:ScriptReference Path="includes/RADDragDropScripts.debug.js" />
                <asp:ScriptReference Path="includes/RScriptUtils.debug.js" />
            </Scripts> 
        </asp:ScriptManager>
        
     <div class="container4" id="test1" style="position:absolute;">
         <div id="test" style="display:flex;position:relative;justify-content:space-around;">
             </div>

  
        </div>
         <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
        <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
        <script src="https://cdnjs.cloudflare.com/ajax/libs/jsPlumb/2.5.8/js/jsplumb.min.js"></script>

        <script type="text/javascript" src="js/thirdparty/jquery-1.11.3.min.js"></script>

        <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/jquery-ui.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/bootstrap.min.js" type="text/javascript"></script>
        <script src="includes/bootstrap2-toggle.js" type="text/javascript"></script>
        <script src="js/thirdparty/knockout-3.4.0.js" type="text/javascript"></script>
        <script src="includes/knockout-fast-foreach.min.js" type="text/javascript"></script>
        <script src="includes/jquery.slimscrollHorizontal.js" type="text/javascript"></script>
        <script src="includes/slimScrollHorizontal.js" type="text/javascript"></script>
        <script src="includes/jquery.slimscroll.js" type="text/javascript"></script>
        <script src="includes/ruleEditorScroll.js" type="text/javascript"></script>
        <script type="text/javascript" src="includes/xlgrid.loader.js"></script>
        <script type="text/javascript" src="includes/neogrid.client.js"></script>
        <script src="includes/XMLHttpSyncExecutor.js" type="text/javascript"></script>
        <script src="includes/viewManager.js" type="text/javascript"></script>
        <script src="includes/updatePanel.js" type="text/javascript"></script>
        <script src="includes/ViewPort.js" type="text/javascript"></script>
        <script src="includes/SecMasterScripts.js" type="text/javascript"></script>
        <script src="includes/script.js" type="text/javascript"></script>
        <script src="includes/grid.js" type="text/javascript"></script>
        <script src="includes/SMSelect.js" type="text/javascript"></script>
        <script src="includes/RADCustomDateTimePicker.js" type="text/javascript"></script>
        <script src="includes/jquery.datetimepicker.js" type="text/javascript"></script>
        <script src="includes/SMSlimscroll.js" type="text/javascript"></script>
        <script type="text/javascript" src="includes/SRMAssociation.js"></script>

        <script type="text/javascript" src="includes/SMSlideMenu.js"></script>
    </form>
     
   
   
</body>
</html>
