<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMMonitorTaskManager.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMMonitorTaskManager" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <%-- <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min.js"></script>--%>
    <link href="css/jquery-ui.min.css" rel="stylesheet" />
    <script src="js/thirdparty/jquery-1.11.3.min.js"></script>
    <script src="js/thirdparty/jquery-ui.min.js"></script>
    <script src="js/thirdparty/react.min.js" type='text/javascript'></script>
    <script src="js/thirdparty/react-dom.min.js" type='text/javascript'></script>
    <script src="js/thirdparty/babel.min.js" type='text/javascript'></script>
    <script src="includes/SRMMonitorTaskManager.js" type='text/babel'></script>
    <link href="css/MonitorTaskManager.css" rel="stylesheet" />
    <script src="https://cdnjs.cloudflare.com/ajax/libs/alasql/0.4.8/alasql-worker.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/alasql/0.4.8/alasql.min.js"></script>
    <link href="css/thirdparty/font-awesome.css" rel="stylesheet" />
    <link href="css/thirdparty/font-awesome.min.css" rel="stylesheet" />
</head>
<body>
    <link href="css/MonitorTaskManager.css" rel="stylesheet" />
    <div id="disableTaskPg" style="display: none; background: red; position: absolute; top: 20px; background: rgba(238, 238, 238, 0.5)">
        <div id="divShowServices" style="display: none; position: absolute; height: 420px; right: 0px; top: 30px; background: white; box-shadow: 0px -1px 9px 0px #aaaaaa;">
            <div id="iframeCloseBtn" style="right: 10px; color: #808080de; position: absolute; top: 5px;"><i class="fa fa-close"></i></div>
            <iframe class="InnerFrame" name="TabsInnerIframe1" src="" style="width: 1180px; height: 420px; background: white; border: none;"></iframe>
        </div>
    </div>
    <div id="btnShowServices" class="openServicesCls">Open services</div>
    <div id="app" style="padding-top: 25px;">
    </div>
</body>
</html>
