<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMMappingSetup.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMMappingSetup" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="css/jquery-ui.min.css" rel="stylesheet" />
    <script src="js/thirdparty/jquery-1.11.3.min.js"></script>
    <script src="js/thirdparty/jquery-ui.min.js"></script>

    <script src="includes/radTabWidget.js"></script>
    <script src="includes/RADMappingConfiguration.pagescript.js"></script>
    <link href="css/thirdparty/bootstrap.min.css" rel="stylesheet" />
    <link href="css/RADMappingSetup.css" rel="stylesheet" />
    <link href="css/radTabWidget.css" rel="stylesheet" />
    <script src="includes/MicrosoftAjax.js"></script>
    <script src="includes/RADBrowserScripts.debug.js"></script>
    <script src="includes/RADDragDropScripts.debug.js"></script>
    <script src="js/thirdparty/knockout-3.4.0.js" type="text/javascript"></script>
    <script src="includes/knockout-fast-foreach.min.js" type="text/javascript"></script>
    <script src="includes/neogrid.client.js"></script>
    <script src="includes/neogrid.loader.js"></script>
    <script src="includes/ruleEditorScroll.js"></script>
    <script src="includes/fileUploadScript.js"></script>
    <link href="css/neogrid.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript">
        $(document).ready(function (e) {
            var path = window.location.protocol + '//' + window.location.host;
            var pathname = window.location.pathname.split('/');

            $.each(pathname, function (ii, ee) {
                if ((ii !== 0) && (ii !== pathname.length - 1))
                    path = path + '/' + ee;
            });

            $("#contentBody").height($(window).height());
            $("#contentBody").width($(window).width());

            var initObj = {};
            initObj.selectedTabId = "Users";
            initObj.contentBodyId = "contentBody";
            initObj.baseUrl = path;
            initObj.IsIagoDependent = false;
            RADMappingConfig.initialize(initObj);
        });


        $(window).resize(function () {
            $("#contentBody").height($(window).height());
            $("#contentBody").width($(window).width());
        })

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="commonUIOverrideStatus" runat="server" ScriptMode="Release"
            EnableViewState="true" AllowCustomErrorsRedirect="false" AsyncPostBackTimeout="20000"
            EnablePageMethods="true" LoadScriptsBeforeUI="true">
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
        <div id="contentBody">
        </div>
    </form>
</body>
</html>
