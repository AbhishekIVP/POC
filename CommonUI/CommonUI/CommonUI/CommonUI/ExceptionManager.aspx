<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExceptionManager.aspx.cs" Inherits="com.ivp.common.exceptionManagerUI.ExceptionManagerUI" 
    MasterPageFile="~/CommonUI/SRMMaster.Master" EnableEventValidation="false" %>

<%--<%@ Page Title="Home Page" Language="C#" AutoEventWireup="true" CodeBehind="ExceptionManager.aspx.cs" Inherits="com.ivp.common.exceptionManagerUI.ExceptionManagerUI" %>--%>

<%--<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">--%>
<asp:Content ID="contentMiddle" ContentPlaceHolderID="SRMContentPlaceHolderMiddle" runat="server">
    <%--<link href="css/AquaStyles.css" rel="stylesheet" type="text/css" />
    <link href="css/thirdparty/font-awesome.min.css" rel="stylesheet" type="text/css" />--%>
    <link href="css/SMSlideMenu.css" rel="stylesheet" type="text/css" />
    <%--<link href="css/SMCss/Customjquery.datetimepicker.css" rel="stylesheet" type="text/css" />--%>
    <link href="css/SMCss/SMDatePickerControl.css" rel="stylesheet" type="text/css" />
    <link href="css/ExceptionManager.css" rel="stylesheet" type="text/css" />
    
    <%--<script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
    <script src="js/thirdparty/jquery-ui.min.js" type="text/javascript"></script>--%>
    <script src="includes/jquery.slimscroll1.js" type="text/javascript"></script>
    <script src="includes/jquery.slimscrollHorizontal.js" type="text/javascript"></script>
    <%--<script src="includes/MicrosoftAjax.js" type="text/javascript"></script>--%>
    <script src="ScriptIncludes/SecMasterJSCommon.debug.js" type="text/javascript"></script>
    <script src="includes/RADCommonScripts.js" type="text/javascript"></script>
    <script src="includes/RScriptUtils.debug.js" type="text/javascript"></script>
    <script src="includes/SMSlimscroll.js" type="text/javascript"></script>
    <script src="includes/SMSlideMenu.js" type="text/javascript"></script>
    <%--<script src="includes/script.js" type="text/javascript"></script>--%>
    <%--<script src="includes/updatePanel.js" type="text/javascript"></script>--%>
    <%--<script src="includes/jquery.datetimepicker.js" type="text/javascript"></script>--%>
    <script src="includes/SMDatePickerControl.js" type="text/javascript"></script>
    <script src="js/ExceptionManager.js" type="text/javascript"></script>
<%--</head>
<body>--%>
    <%--<form id="form1" runat="server">
    <%--<asp:ScriptManager ID="commonEM" runat="server" ScriptMode="Release" EnableViewState="true"
        AllowCustomErrorsRedirect="false" AsyncPostBackTimeout="20000" EnablePageMethods="true"
        LoadScriptsBeforeUI="true">
    </asp:ScriptManager>--%>
    <div id="em_pageContainer">
        <%--<div id="em_topSection">
            <div id="tabToggleDiv" style="display: block;">
                <div id="systemsExceptionButton" class="tabToggleClass selectedTabClass" style="display: inline-block;">
                    All Systems</div>
                <div id="secMasterExceptionButton" class="tabToggleClass" style="display: inline-block;">
                    SecMaster</div>
                <div id="refMasterExceptionButton" class="tabToggleClass" style="display: inline-block;">
                    RefMaster</div>
            </div>
        </div>--%>
        
        <div class='DataExceptionsTagsContainer' style="margin:10px auto;margin-bottom: 0px;margin-top: 0px;">
            <i id="em_tagsLeftClick" class="fa fa-chevron-left EM_tagsArrow" style="display:none;"></i>
            <div class='DataExceptionsTagsScrollContainer'>
                <div class='DataExceptionsTagsDiv'>
                </div>
                <div class='dashboardDownArrowDiv' class="downArrowDiv">
                    <span class='fa fa-caret-down dashboardDownArrow' style='left: 108px;'></span>
                </div>
            </div>
            <i id="em_tagsRightClick" class="fa fa-chevron-right EM_tagsArrow" style="display:none;" ></i>
        </div>
        <div id="allsystemsDiv">
        </div>
        <div id="outer-div" style="width: 1140px; margin: 10px auto;">
            <div class="left-section">
                <div class="left-section-head">
                    </br>
                </div>
                <div class="left-section-body" id="exManagerTypeDiv">
                </div>
            </div>
            <div class="right-section">
                <div class="right-section-inner">
                    <div class="right-section-inner-head" id="right-section-inner-head">
                    </div>
                    <div class="right-section-inner-body" id="exceptionTypeDiv">
                    </div>
                </div>
            </div>
        </div>
        <div id="em_errorMsgContainer" style="display:none;"></div>
        <div class="exceptions-details-div">
        </div>
        <%--<div id="loadingImg" class="loadingMsg" style="display: none; z-index: 10000;">
            <asp:Image ID="Image1" runat="server" src="css/images/ajax-working.gif" />
        </div>
        <div id="disableDiv" class="alertBG" style="display: none; z-index: 9999;" align="center">--%>
        </div>
    </div>
    <%--</form>--%>
<%--</body>--%>
<%--</html>--%>
</asp:Content>
<asp:Content ID="contentTop" ContentPlaceHolderID="ContentPlaceHolderFilterSection" runat="server">
    <div id="dateFilter">
        <span id="dateFilterText">Today</span>
        <i class="fa fa-chevron-down" style="padding-left: 5px; font-size: 10px;"></i>
    </div>
    <div id="rightMenuDiv">
        <i class="fa fa-filter"></i>
    </div>
</asp:Content>