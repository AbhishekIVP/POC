<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMModeler.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMModeler" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="css/SRMModeler.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="https://fonts.google.com/specimen/Oswald?selection.family=Oswald" />
</head>
<body>
    <form id="form1" runat="server">
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

  
        
        <!--<button onclick="myFunction()">Click me</button>-->
        <div class="SRMpopup" style="left: 2149px; top: 808px; position: absolute;" onclick="srmModeler.myFunction1()">
            <%--<img src="images/ModelerPlus.jpg" width="40" height="37">--%>
            <div style="border-radius:30px;background-color:green;padding:4px;padding-left:8px !important;padding-right:8px !important;padding-top:8px !important">
            <i class="fa fa-plus" style="font-size:40px;color:white;"></i>
            </div>
            <div class="popuptext button1" id="myPopup" style="">
                <button type="button" class="button11" style="" onclick="srmModeler.doIt();">Department</button>
                <button type="button" class="button11" style="" onclick="srmModeler.doIt();">Issuer</button>
                <button type="button" class="button11" style="" onclick="srmModeler.doIt();">Upstream</button>
                <button type="button" class="button11" style="" onclick="srmModeler.doIt();">Department2</button>
                <button type="button" class="button11" style="" onclick="srmModeler.doIt();">Issuer2</button>
                <button type="button" class="button11" style="" onclick="srmModeler.doIt();">Upstream2</button>
            </div>
        </div>
        <!--<div ><img src="E:\temp.jpg" width="40" height="37"></div>-->
        <p id="demo" style="display:none;">ghjgj</p>
        <p id="demo1" style="display:none;">FHDFG</p>
        <!--<div style="height:70px;width:50px;left:100px;top:100px;background-color:#FD9759;position:absolute;">temp</div>-->
        <!--<div style="height:70px;width:50px;left:200px;top:150px;clear:both; background-color:#7481DB;position:absolute;">temp2</div>-->
        
  
        <div id="test"></div>
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
        <script src="js/SMAutocomplete.js" type="text/javascript"></script>
        <script type="text/javascript" src="includes/SMSlideMenu.js"></script>
        <script type="text/javascript" src="includes/SRMModeler.js"></script>


        <div id="SRMDetailsView" class="SRMDetailsView" style="display:none;">
            <div id="SRMcloseButton" class="SRMcloseButton">
                <i class="fa fa-close"></i>
            </div>
            <iframe src="" id="SRMDetailsIframe" ></iframe>
        </div>

    </form>
</body>
</html>
