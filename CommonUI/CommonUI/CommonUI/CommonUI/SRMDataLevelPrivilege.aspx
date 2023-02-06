<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMDataLevelPrivilege.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMDataLevelPrivilege" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="css/jquery-ui.min.css" rel="stylesheet" />
    <script src="js/thirdparty/jquery-1.11.3.min.js"></script>
    <script src="js/thirdparty/jquery-ui.min.js"></script>
    <script src="js/thirdparty/knockout-3.4.0.js"></script> 
    <link href="css/thirdparty/bootstrap.min.css" rel="stylesheet" />
    <script src="includes/RADDataPrivileges.js"></script>
    <link href="css/RADFunctionalGroupV2.css" rel="stylesheet" />
    <script src="includes/jquery.qtip.min.js"></script>
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
            initObj.contentBodyId = "contentBody";
            initObj.baseUrl = path;
            initObj.IsIagoDependent = false;
            RADDataPrivilegesConfig.initialize(initObj);
        });


        $(window).resize(function () {
            $("#contentBody").height($(window).height());
            $("#contentBody").width($(window).width());
        })
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="contentBody"></div>
    </form>
</body>
</html>
