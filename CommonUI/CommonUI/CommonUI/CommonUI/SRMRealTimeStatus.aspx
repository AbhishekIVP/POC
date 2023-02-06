<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMRealTimeStatus.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMRealTimeStatus"
    MasterPageFile="~/CommonUI/SRMMaster.Master" EnableEventValidation="false" %>

<%@ OutputCache Location="None" VaryByParam="None" %>

<%-- Changed By Dhruv --%>
<asp:Content ID="contentFilterSection" ContentPlaceHolderID="ContentPlaceHolderFilterSection" runat="server">
    <span style="line-height:35px; float:right; padding-right: 5%;">
        <a href="#" id="labeldateselectiondiv" runat="server" style="border-bottom: none !important; color: rgb(0, 191, 242);"
        class="LinkExceptionFilterItems">Today</a>
        <i id="RealTimeDateFilterDownArrow" class="fa fa-chevron-down" style="margin-left: 3px; font-size: 9px; color:#4197ce !important; cursor: pointer;"></i>

        <%--Refresh--%>
        <i id="RealTimeStatusRefreshButton" class="fa fa-refresh" style="display: inline-block; font-size: 17px; padding-left: 10px; color: rgb(210, 210, 210); cursor: pointer;"></i>

    </span>
</asp:Content>

<asp:Content ID="contentMiddle" ContentPlaceHolderID="SRMContentPlaceHolderMiddle" runat="server">
    <link href="css/SMCss/SMDatePickerControl.css" rel="stylesheet" type="text/css" />
    <script src="includes/SMDatePickerControl.js" type="text/javascript"></script>
    <link href="App_Themes/Awesome/bootstrap/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="App_Themes/Awesome/bootstrap/bootstrap-theme.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        input[type=radio] {
            margin: 3px 3px 0px 5px;
        }

        .nav > li > a:hover, .nav > li > a:focus {
            color: #4197ce !important;
            background-color: White;
            border-color: White;
        }

        label {
            font-weight: normal;
        }

        .ExceptionItem td[marked="0"]:hover {
            color: Black;
        }

        .ExceptionItem td[marked="1"]:hover {
            color: rgb(0, 191, 242);
        }

        #divExceptionDate tr:hover {
            background: #f9f9f9; /*  color: rgb(178, 178, 178);*/
        }

        #trExceptionDateCustom:hover {
            background-color: #FFF !important;
        }

        #trExceptionDateCustom td table tbody tr:hover {
            background-color: #FFF !important;
        }

        .xdsoft_scrollbar > .xdsoft_scroller:hover {
            width: 4px;
        }

        .InnerFrame {
            width: 100%;
            height: 100%;
            margin: 0px;
            padding: 0px;
        }

        .TabscssAfter {
            padding-right: 0px !important;
            padding-left: 0px !important;
            color: #4197ce !important;
            border-bottom: 3px solid !important;
            border-bottom-color: #4197ce !important;
            font-size: 12px;
            font-family: Lato;
            cursor: pointer;
        }

        .TabscssBefore {
            padding-right: 0px !important;
            padding-left: 0px !important;
            color: #666 !important;
            border-bottom: none !important;
            border-bottom-color: #666 !important;
        }

        .nav > li > a {
            padding: 7px 15px;
        }

        Label {
            margin-bottom: 0px;
            font-weight: normal;
        }
    </style>
    <script src="js/bootstrap.min.js" type="text/javascript"></script>
    <script src="includes/SRMRealTimeStatus.js" type="text/javascript"></script>
    <%--<customScript:ContainerControl ID="Status" runat="server">--%>
    <input type="hidden" id="isPostBack" value="<%=Page.IsPostBack.ToString()%>" />
    <asp:Panel ID="panelTopDownStreamStatus" runat="server" CssClass="panelTopBorderBottom"
        Style="padding-bottom: 5px; box-shadow: none;">
        <%--border-bottom-color: rgb(205, 205, 205); border-bottom-width: 1px;
            border-bottom-style: solid;--%>
        <div id="topDiv">
            <%-- style="width: 97%;"--%>
            <%--<div id="tabContainer" class="tabClass" style="position: absolute; display: inline-block;
                    top: 0px; left: 0px; background-color: white;">
                    <ul class="nav nav-tabs" style="border-bottom: none; margin-top: 11px; padding-left: 10px;
                        box-shadow: 2px 0px 10px #DADADA; border-radius: 3px;">
                        <li><a target="TabsInnerIframe1" id="tab1" class="tabs" style="padding-right: 0px;
                            padding-left: 0px; margin-right: 40px; color: #666; font-size: 14px; font-family: Lato;
                            cursor: pointer">Securities</a></li>
                        <li><a target="TabsInnerIframe2" id="tab2" class="tabs" style="padding-right: 0px;
                            padding-left: 0px; margin-right: 40px; color: #666; font-size: 14px; font-family: Lato;
                            cursor: pointer">Reference Data</a></li>
                    </ul>
                </div>--%>
            <%--     runningTextClass-> padding-bottom: 4px;   position: absolute;margin-bottom:4px;--%>
            <!-- <div class="runningTextClass" style="display: inline-block; vertical-align: top;
                    /*margin-top: 14px; border: 1px solid #ddd;*/ margin-left: 0px; height: 30px; width: 100%;">
                    <%-- box-shadow: 0px 0px 10px #CACACA width: 75.6%;--%>
                    <table id="tble" style="vertical-align: top; margin-left: auto; margin-right: auto;"
                        border="0" cellspacing="0" cellpadding="0">
                        <tr>
                            <td style="padding-top: 5px;">
                                <span class="LinkExceptionFilterText" style="margin-right: 0px; padding-right: 3px;
                                    padding-left: 3px; color: #666;">
                                    <label id="lblRunningText" text="">
                                    </label>
                                </span>
                            </td>
                            <td>
                                <div id="divFilter" class="ExceptionFilter" style="vertical-align: middle; border-top-left-radius: 10px;
                                    border-top-right-radius: 10px;">
                                    <table style="width: 100%;">
                                        <tr>
                                            <td style="padding-top: 5px;">
                                                <span><a href="#" id="labeldateselectiondiv____" runat="server" style="border-bottom: 1px solid rgb(0, 191, 242);
                                                    border-bottom-style: dashed; color: rgb(0, 191, 242);" class="LinkExceptionFilterItems">
                                                    Today</a></span>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            <%--    <div id="divFilterDDLs">
                                    <div id="divExceptionDate" class="ExceptionDateDropDownsVisible CommonDropDownRows"
                                        state="0" isvalid="1" style="display: none; top: 25px; padding-left: 0px; color: rgb(0,0,0);">
                                        <table cellpadding="0" cellspacing="0" border="0" class="ExceptionItem" width="100%">
                                            <tr>
                                                <td value="0" marked="0">
                                                    Today
                                                </td>
                                            </tr>
                                            <tr>
                                                <td value="1" marked="0">
                                                    Since Yesterday
                                                </td>
                                            </tr>
                                            <tr>
                                                <td value="2" marked="0">
                                                    This Week
                                                </td>
                                            </tr>
                                            <tr>
                                                <td value="3" marked="0">
                                                    Any Time
                                                </td>
                                            </tr>
                                            <tr>
                                                <td value="4" marked="0">
                                                    Custom Date
                                                </td>
                                            </tr>
                                            <%--style="display: none;"
                                            <tr id="trExceptionDateCustom" class="customDate">
                                                <td>
                                                    <table cellpadding="0" cellspacing="0" border="0" width="100%" style="padding-right: 20px;">
                                                        <tr>
                                                            <td>
                                                                <%-- -----------------------------------------------------------------------------------------------------------------
                                                                <hr />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <asp:RadioButtonList ID="rblstDate" runat="server" RepeatLayout="Table" RepeatDirection="Horizontal">
                                                                    <asp:ListItem Value="0" Selected="True">Between</asp:ListItem>
                                                                    <asp:ListItem Value="1">From</asp:ListItem>
                                                                    <asp:ListItem Value="2">Prior To</asp:ListItem>
                                                                </asp:RadioButtonList>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td>
                                                                <table border="0" cellpadding="0" cellspacing="2" width="100%">
                                                                    <tr>
                                                                        <td style="padding-left: 5px;">
                                                                            <%--width="20%"--
                                                                            <asp:Label ID="lblStartDate" runat="server">Start Date : </asp:Label>
                                                                        </td>
                                                                        <td style="padding-right: 5px;">
                                                                            <%--width="25%"--
                                                                            <asp:TextBox ID="TextStartDate" runat="server" isClicked="0" CssClass="DateTimeAttributeText" style="width: 180px;"
                                                                                ToolTip="<%# SessionInfo.CultureInfo.ShortDateFormat %>"><%--Style="width: 59%;"--
                                                                            </asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style="padding-left: 5px;">
                                                                            <%--width="20%"--
                                                                            <asp:Label ID="lblEndDate" runat="server">End Date : </asp:Label>
                                                                        </td>
                                                                        <td style="padding-right: 5px;">
                                                                            <%--width="25%"--
                                                                            <asp:TextBox ID="TextEndDate" isClicked="0" runat="server" CssClass="DateAttributeText"  style="width: 180px;"
                                                                                ToolTip="<%# SessionInfo.CultureInfo.ShortDateFormat %>">
                                                                            </asp:TextBox>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="2">
                                                                            <asp:Label ID="lblStartDateError" Style="color: red; display: none;" runat="server"></asp:Label>
                                                                        </td>
                                                                        <td colspan="2">
                                                                            <asp:Label ID="lblEndDateError" Style="color: red; display: none;" runat="server"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan="4" style="padding-left: 5px;">
                                                                            <asp:Label ID="lblValidationSummary" Style="color: red; display: none;" runat="server"></asp:Label>
                                                                        </td>
                                                                    </tr>
                                                                </table>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                        <input type="hidden" runat="server" id="hdnExceptionDate" value="0" enableviewstate="true" />
                                    </div>
                                </div>--%>
                            </td>
                            <td style="padding-left: 20px;">
                                <asp:Button ID="btnGetStatus" CssClass="button" runat="server" Text="Apply" OnClientClick="return false;"
                                    Width="60px" style="margin-top: 3px;"/>
                            </td>
                        </tr>
                    </table>
                </div> -->
            <%--    Style="position: absolute; right: 0px; top: 18px;"--%>
            <%-- <asp:Button ID="btnGetStatus" CssClass="button" runat="server" Text="Apply" OnClientClick="return false;"
                    Style="position: absolute; right: 0px; top: 18px;" Width="60px" />--%>
            <%--<div id="btnGetStatusid" style="vertical-align: top; display: inline-block; margin: 3px 15px 0px 30px;
                    text-align: right; width: 70px; position: absolute; top: 0px; right: 10px;">--%>
            <%--&nbsp;</div>--%>
        </div>
        <div id="container1">
            <iframe src="" name="TabsInnerIframe1" class="InnerFrame" id="InnerFrameId1" style="border: none;"></iframe>
        </div>
        <%--<div id="container2" style="display: none">
                <iframe src="" name="TabsInnerIframe2" class="InnerFrame" id="InnerFrameId2" style="border: none;
                    padding-top: 10px;"></iframe>
            </div>--%>
    </asp:Panel>
    <%--</customScript:ContainerControl>--%>
    <input type="hidden" id="hdnLongDateFormat" runat="server" />
    <input type="hidden" id="hdnTypeId" runat="server" />
    <input id="hdnCustomRadioOption" runat="server" type="hidden" value="0" />
    <input id="hdnTopOption" runat="server" type="hidden" value="-1" />
    <input id="hdnStartDateCalender" runat="server" type="hidden" value="-1" />
    <input id="hdnEndDateCalender" runat="server" type="hidden" value="-1" />
    <asp:TextBox ID="TextStartDate" type="block" runat="server" isClicked="0" CssClass="DateTimeAttributeText" Style="width: 180px; display: none;"
        ToolTip="<%# SessionInfo.CultureInfo.ShortDateFormat %>">
    </asp:TextBox>
    <asp:TextBox ID="TextEndDate" type="block" isClicked="0" runat="server" CssClass="DateAttributeText" Style="width: 180px; display: none;"
        ToolTip="<%# SessionInfo.CultureInfo.ShortDateFormat %>">
    </asp:TextBox>

    <div id="divScript" runat="server">
        <script type="text/javascript" language="javascript">
            //            var height = document.documentElement.clientHeight;//  - ($("#tdBottom").height() + $("#tdTop").height());
            //            $("div[id$='divWrapperContainer']").outerHeight(height - 2);
            //            $("#table").outerHeight(height);

            //  if (parseInt($('#tble').css('margin-left')) < 385) {
            //                    $('#tble').css('margin-left', '390px');
            //                }
            //                else {
            //                    $('#tble').css('margin', '0 auto');
            //

            //                }

            $(document).ready()
            {
                if ($(window).width() < 1280)
                    $('#tble').css('margin-left', '390px');
                else
                    $('#tble').css('margin', '0 auto');
            }
        </script>
    </div>
</asp:Content>
