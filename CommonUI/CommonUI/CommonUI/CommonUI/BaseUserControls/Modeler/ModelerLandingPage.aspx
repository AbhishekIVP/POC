<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ModelerLandingPage.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.BaseUserControls.Modeler.ModelerLandingPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <%--<script src="../../js/thirdparty/bootstrap.min.js"></script>--%>
    <script src="../../js/thirdparty/jquery-1.11.3.min.js"></script>
    <link href="../../../App_Themes/Aqua/thirdparty/font-awesome.css" rel="stylesheet" />
    <link href="../../css/ModelerLandingPage.css" rel="stylesheet" />
    <script src="../../includes/SRMModelerLanding.js"></script>
   
</head>
<body>
    <form id="form1" runat="server">
        <div>
            
            <i class="fa fa-search" id="searchicon"></i>
            <input type="text" id="searchtext" class="search" placeholder="Search Entity Type or Tags" onkeyup="searchrecord()" />

            <input type="button" id="changetext" class="AddBut" value="Add Type"/>

            <div id="SRCContainer"></div>
           
           
        </div>
        
        <input type="hidden" runat="server" id="hdnModuleId" />
    </form>
</body>
</html>
