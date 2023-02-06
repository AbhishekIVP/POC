<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMFileUpload.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMFileUpload" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Upload File</title>
</head>
<body class="RMpanelCells" style="height: 24px; padding-top: 5px;">
    <form id="frmUploadfile" runat="server" enctype="multipart/form-data" method="POST"
    target="_self">
    <asp:ScriptManager runat="server">
        <Services>
            <asp:ServiceReference Path="~/Services/RADGridService.svc" />
        </Services>
    </asp:ScriptManager>
    
    </form>
</body>
</html>
