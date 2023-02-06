<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="CommonUI.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>IVP Security Master</title>
    <meta runat="server" id="meta1" http-equiv="X-UA-Compatible" content="IE=EmulateIE10" />
    <link rel="shortcut icon" href="includes/favicon.ico" />
    <script src="js/jquery-1.7.1.js" type="text/javascript"></script>
    <script src="js/bootstrap.min.js" type="text/javascript"></script>
    <link href="css/thirdparty/polaris/Awesome/bootstrap/bootstrap-theme.min.css" rel="stylesheet"
        type="text/css" />
    <link href="css/thirdparty/polaris/Awesome/bootstrap/bootstrap.min.css" rel="stylesheet"
        type="text/css" />
    <style type="text/css">
        @font-face
        {
            font-family: "Lato";
            src: url("App_Themes/fonts/Lato-Regular.ttf") format("truetype");
        }
    </style>
    <%--      
  <script language="javascript" type="text/javascript">
       var _name = 'secmaster_' + Math.round(Math.random() * 10090);
       var regEx = new RegExp('secmaster_');
       if (!regEx.test(window.name)) {
           loadInNewWindow();
       }
       function loadInNewWindow() {
           window.open('login.aspx', _name,
                'toolbars=no, menubar=no, location=no, scrollbars=yes, resizable=yes, status=no, height=' +
                    (screen.availHeight -52) + 'px, top=0,left=0, width=' +
                                screen.availWidth + 'px');
           closeWindow();
       }

       function closeWindow() {
           window.open('', '_self', '');
           window.close();
       }
        
        
    </script>--%>
</head>
<body class="body" onload="javascript:window.name='';" style="background-color: #fff;
    font-family: Lato;">
    <form id="form2" runat="server">
    <asp:ScriptManager ID="smLogin" runat="server">
    </asp:ScriptManager>
    <div>
        <asp:UpdatePanel ID="upLogin" runat="server">
            <ContentTemplate>
                <asp:Image ID="Image4" runat="server" SkinID="imgIVPLoginLogo" class="ivpLoginLogo" />
                <table width="100%" border="0" cellspacing="0" cellpadding="0">
                    <tr>
                        <td height="5" align="center">
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 100px;">
                        </td>
                    </tr>
                    <tr style="height: 60px;">
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <table border="0" align="center" cellpadding="0" cellspacing="0">
                                <tr>
                                    <td align="center">
                                        <asp:Image ID="Image3" runat="server" SkinID="imgSecMasterLogo" />
                                    </td>
                                </tr>
                                <%--<tr>
                                    <td align="center" class="loginHeader">
                                        &nbsp;
                                    </td>
                                </tr>--%>
                                <tr>
                                    <td class="loginArea">
                                        <table width="452" border="0" align="center" cellpadding="0" cellspacing="0">
                                            <%--<tr>
                                                <td class="loginTop">
                                                    Login to <b>IVP Security Master</b>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td width="25%" class="loginHeading">
                                                    Please enter your User ID and Password to access the System
                                                </td>
                                            </tr>--%>
                                            <tr>
                                                <td>
                                                    <table width="100%" border="0" align="center" cellpadding="0" cellspacing="0" class="loginDetails">
                                                        <tr>
                                                            <%--<td width="41%" class="blkTxtLblLeft">
                                                                User ID:
                                                            </td>--%>
                                                            <td style="text-align: left">
                                                                <p class="SMloginLabel">
                                                                    Sign In</p>
                                                                <div class="CommonInputBoxWithImage" id="userNameDiv" style="border-bottom-color: #C5E0EB;
                                                                    margin: 0px auto;">
                                                                    <span class="glyphicon glyphicon-user" style="color: #C5E0EB"></span>
                                                                    <asp:TextBox runat="server" CssClass="input" ID="txtLogin" Style="width: 350px;"
                                                                        placeholder="User Name" onfocus="if(this.value === 'User Name') this.value = '';"
                                                                        onblur="if(this.value === '') this.value = 'User Name';"></asp:TextBox>
                                                                </div>
                                                                <asp:RequiredFieldValidator ID="rfvLogin" runat="server" ControlToValidate="txtLogin"
                                                                    CssClass="errorMessage SMloginErrorMessage" ErrorMessage="Please enter the username"
                                                                    Visible="false" ForeColor=""></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <%--<td class="blkTxtLblLeft">
                                                                Password:
                                                            </td>--%>
                                                            <td style="text-align: left">
                                                                <div class="CommonInputBoxWithImage" id="passwordDiv" style="border-bottom-color: #ECE6B2;
                                                                    margin: 0px auto;">
                                                                    <span class="glyphicon glyphicon-lock" style="color: #ECE6B2;"></span>
                                                                    <asp:TextBox runat="server" CssClass="input" ID="txtPassword" Style="width: 350px;"
                                                                        TextMode="Password" placeholder="Password" onfocus="if(this.value === 'Password') this.value = '';"
                                                                        onblur="if(this.value === '') this.value = 'Password';"></asp:TextBox>
                                                                </div>
                                                                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                                                                    CssClass="errorMessage SMloginErrorMessage" ErrorMessage="Please enter the password"
                                                                    Visible="false" ForeColor=""></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" align="center" class="redLink">
                                                                <asp:ValidationSummary ID="vsLogin" runat="server" CssClass="errorDiv loginError"
                                                                    Width="90%" Style="text-align: center; padding-left: 0; background: none; color: #CF2330;"
                                                                    Visible="false" />
                                                                <asp:Label ID="lblServerMessage" runat="server" CssClass="errorMessage" Visible="False"></asp:Label>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <%--<td>
                                                                &nbsp;
                                                            </td>--%>
                                                            <td style="text-align: center">
                                                                <div class="loginButtonDiv">
                                                                    <asp:Button ID="btnLogin" runat="server" OnClick="btnLogin_Click" Text="Login" />
                                                                    <span>
                                                                        <asp:ImageButton ID="loginBArrow" runat="server" src="App_Themes/Aqua/images/LoginArrow.png"
                                                                            OnClick="btnLogin_Click" />
                                                                    </span>
                                                                </div>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="mirror">
                                        &nbsp;
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btnLogin" />
            </Triggers>
        </asp:UpdatePanel>
        <table width="100%">
            <tr>
                <td align="center">
                    <asp:Label ID="lblLicenceExpired" runat="Server" Text="Your licence for this application has expired."
                        Font-Size="Larger" CssClass="alertLabel" Visible="false"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    </form>
    <div id="tdBottom" style="width: 99.9%; height: 18px; bottom: 0; position: fixed;
        display: block;">
        <%--<asp:ContentPlaceHolder ID="cphBottom" runat="server">
        </asp:ContentPlaceHolder>--%>
        <div style="padding: 2.8px; width: 100%; text-align: right; bottom: 0px; font-size: 10px;
            position: fixed; z-index: 100;">
            Copyright ©
            <asp:Label ID="lblYear" runat="server"></asp:Label>
            <b>Indus Valley Partners</b>. All Rights Reserved &nbsp; &nbsp;&nbsp;&nbsp;&nbsp;
        </div>
    </div>
    <script>
        //        $(function () {
        //            $(document).click(function (e) {
        //                $("#userNameDiv").css('border-bottom-color', '#C5E0EB');
        //                $("#passwordDiv").css('border-bottom-color', '#ECE6B2');
        //                e.stopPropagation();
        //            });

        //            $("[id$='txtLogin']").unbind('click').bind('click',function (e) {
        //                $("#userNameDiv").css('border-bottom-color', '#6ae3ff');
        //                e.stopPropagation();
        //            });

        //            $("[id$='txtPassword']").unbind('click').bind('click',function (e) {
        //                $("#passwordDiv").css('border-bottom-color', '#fff8a6');
        //                e.stopPropagation();
        //            });

        //            $("[id$='txtLogin']").click();
        //        });
    </script>
</body>
</html>
