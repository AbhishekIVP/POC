<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScreenerSecurityView.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.ScreenerSecurityView" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <link href="css/AquaStyles.css" rel="stylesheet" type="text/css" />
    <!--CSS FILES-->
    <link href="css/ScreenerSecurityView.css" rel="stylesheet" type="text/css" />

    <!--JS FILES-->
    <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
    <script src="js/thirdparty/knockout-3.4.0.js" type="text/javascript"></script>
    <script src="js/thirdparty/highstock.js" type="text/javascript"></script>
    <script src="includes/SMSlimscroll.js" type="text/javascript"></script>
    <script src="js/ScreenerSecurityView.js" type="text/javascript"></script>
</head>
<body>
    <form id="smScreenerSecurityView_form" runat="server">
    <div id="smScreenerSecurityView_container" class="smScreenerSecurityView_container">
        <div style="margin: 15px 0px;margin-left:11%;">
            <%--<div class="SMScreenerSecurityView_horizontalAlign smScreenerSecurityView_secName">Security Name</div>--%>
            <div class="SMScreenerSecurityView_horizontalAlign smScreenerSecurityView_secValue" data-bind="text: SecurityName"></div>
        </div>
        <div style="margin-left:11%;">
            <div data-bind="foreach: AttributeFirstSection" class="SMScreenerSecurityView_horizontalAlign SMScreenerSecurityView_attributeSection">
                <div>
                    <div data-bind="text: AttributeName" class="SMScreenerSecurityView_horizontalAlign SMScreenerSecurityView_Label"></div>
                    <div data-bind="text: AttributeValue" class="SMScreenerSecurityView_horizontalAlign SMScreenerSecurityView_Value"></div>
                </div>
            </div>
            <div data-bind="foreach: AttributeSecondSection" class="SMScreenerSecurityView_horizontalAlign SMScreenerSecurityView_attributeSection">
                <div>
                    <div data-bind="text: AttributeName" class="SMScreenerSecurityView_horizontalAlign SMScreenerSecurityView_Label"></div>
                    <div data-bind="text: AttributeValue" class="SMScreenerSecurityView_horizontalAlign SMScreenerSecurityView_Value"></div>
                </div>
            </div>
            <div data-bind="foreach: AttributeThirdSection" class="SMScreenerSecurityView_horizontalAlign SMScreenerSecurityView_attributeSection">
                <div>
                    <div data-bind="text: AttributeName" class="SMScreenerSecurityView_horizontalAlign SMScreenerSecurityView_Label"></div>
                    <div data-bind="text: AttributeValue" class="SMScreenerSecurityView_horizontalAlign SMScreenerSecurityView_Value"></div>
                </div>
            </div>
        </div>
        <div id="smScreenerSecurityView_splineChart" class="smScreenerSecurityView_chartStyle"></div>
        <%--<div id="smScreenerSecurityView_barChart" class="smScreenerSecurityView_chartStyle"></div>--%>
    </div>
    </form>
</body>
</html>
