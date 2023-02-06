<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMApiAuthentication.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMApiAuthentication" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
<link href="css/jquery-ui.min.css" rel="stylesheet" />
    <script src="js/thirdparty/jquery-1.11.3.min.js"></script>
    <script src="js/thirdparty/jquery-ui.min.js"></script>
    <script src="js/thirdparty/knockout-3.4.0.js"></script> 
    <link href="css/thirdparty/bootstrap.min.css" rel="stylesheet" />
    <link href="css/all.css" rel="stylesheet" />
    <script src="js/thirdparty/moment.js"></script>
    <%--<script src="js/thirdparty/react.min.js"></script>
    <script src="js/thirdparty/react-dom.min.js"></script>--%>
    <script crossorigin src="https://unpkg.com/react@16/umd/react.development.js"></script>
    <script crossorigin src="https://unpkg.com/react-dom@16/umd/react-dom.development.js"></script>


    <script src="js/thirdparty/redux.min.js"></script>
    <script src="js/thirdparty/react-redux.min.js"></script>
    
    <script src="includes/jquery.qtip.min.js"></script>
    
    <script src="includes/RADClientIDSecretModule.js"></script>

    <link href="css/RADClientIDSecretStyle.css" rel="stylesheet" />
    <link href="css/thirdparty/font-awesome.css" rel="stylesheet" />
    <link href="css/thirdparty/font-awesome.min.css" rel="stylesheet" />
     <script type="text/javascript">
        $(document).ready(function (e) {
            var path = window.location.protocol + '//' + window.location.host;
            var pathname = window.location.pathname.split('/');

            $.each(pathname, function (ii, ee) {
                if ((ii !== 0) && (ii !== pathname.length - 1))
                    path = path + '/' + ee;
            });
            debugger;
            $("#contentBody").height($(window).height());
            $("#contentBody").width($(window).width());

            RADClientIDSecretModule.initialize({ divId: "contentBody",serviceUrl:path });

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
