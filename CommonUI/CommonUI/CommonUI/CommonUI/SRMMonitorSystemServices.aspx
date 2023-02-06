<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMMonitorSystemServices.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMMonitorSystemServices" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="css/jquery-ui.min.css" rel="stylesheet" />
    <script src="js/thirdparty/jquery-1.11.3.min.js"></script>
    <script src="js/thirdparty/jquery-ui.min.js"></script>
    <script src="js/thirdparty/react.min.js" type='text/javascript'></script>
    <script src="js/thirdparty/react-dom.min.js" type='text/javascript'></script>
    <script src="js/thirdparty/babel.min.js" type='text/javascript'></script>
    <script src="includes/SRMMonitorSystemServices.js" type='text/babel'></script>
    <script src="includes/updatePanel.js"></script>
    <link href="css/SRMMonitorSystemServices.css" rel="stylesheet" />
    <link href="css/thirdparty/font-awesome.css" rel="stylesheet" />
    <link href="css/thirdparty/font-awesome.min.css" rel="stylesheet" />
</head>
<body>
    <div>
        <div class="poolCls">Application Pool</div>
        <div id="StartPoolId" class="poolActionCls">Start</div>
        <div id="StopPoolId" class="poolActionCls">Stop</div>
    </div>
    <div id="app" style="padding-top: 25px;">
    </div>
    <div id="panelError" class="errorPopupDiv" style="display: none; z-index: 9999; width: 55%; overflow-x: hidden; position: absolute; top: 0px; left: 415px; }">
        <div class="popupErrorUpperDiv" style="height: 8px;">
        </div>
        <div class="errorDivWrapper" style="padding-bottom: 20px;">
            <div class="errorTopHead" style="background: #8080801c;">
                <div class="errorDivLeft" style="margin-top: 0px;">
                    <img src="../App_Themes/Aqua/images/icons/fail_icon.png" style="height: 30px;" />
                    <div class="errorDivHeading" style="color: #666; height: 30px; display: inline-block;">
                        <p class="errorTopTxtCls" style="margin: 0px; color: #C7898C; font-size: 14px;">
                            Error
                        </p>
                    </div>
                </div>
                <div class="errorDivClose">
                    <i id="btnErrorOk" onclick="javascript:return false" class="fa fa-times modalCancelRemarkBtn"
                        style="cursor: pointer; font-size: 14px; float: right;"></i>
                </div>
            </div>
            <div class="errorDivRight" style="width: 96%;">

                <div class="errorDivText mostly-customized-scrollbar" style="overflow-x: hidden; max-height: 188px;">
                    <div id="lblError">
                    </div>
                </div>
                <div id="divErrorAlertButton" style="text-align: center; display: inline-block">
                </div>
                <div style="text-align: center; margin-top: 10px;">
                    <div id="lblErrorText" style="display: inline-block; font-size: 13px;">
                    </div>
                </div>
            </div>

        </div>
    </div>

</body>
</html>
