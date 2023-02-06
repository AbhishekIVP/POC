<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="QuantModule.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.QuantModule" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="css/thirdparty/bootstrap.min.css" rel="stylesheet" type="text/css" />
<%--    <link href="css/xlneogrid.css" rel="stylesheet" type="text/css" />
    <link href="css/grid.css" rel="stylesheet" type="text/css" />
    <link rel="Stylesheet" href="css/neogrid.css" type="text/css" />--%>
    <link href="css/thirdparty/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <link rel="Stylesheet" href="css/quantStyle.css" type="text/css" />

    <link href="css/Customjquery.datetimepicker.css" rel="stylesheet" type="text/css" />
    <link href="css/jquery.datetimepicker.css" rel="stylesheet" type="text/css" />
    <link href="css/RADDateTimePicker.css" rel="stylesheet" type="text/css" />
    <%--<link href="css/thirdparty/Lato.css" rel='stylesheet' type='text/css' />
    <link href="css/thirdparty/IstokWeb.css" rel='stylesheet' type='text/css' />--%>
</head>
<body>
    <form id="form1" runat="server" onsubmit="return false;">
    <asp:ScriptManager ID="commonUIOverrideStatus" runat="server" ScriptMode="Release"
        EnableViewState="true" AllowCustomErrorsRedirect="false" AsyncPostBackTimeout="20000"
        EnablePageMethods="true" LoadScriptsBeforeUI="true">
        <Services>
            <asp:ServiceReference Path="~/CommonUI/BaseUserControls/Service/RADNeoGridService.svc" />
        </Services>
        <Scripts>
            <asp:ScriptReference Path="~/CommonUI/includes/RADBrowserScripts.debug.js" />
            <asp:ScriptReference Path="~/CommonUI/includes/RADDragDropScripts.debug.js" />
            <asp:ScriptReference Path="~/CommonUI/includes/RScriptUtils.debug.js" />
        </Scripts>
    </asp:ScriptManager>
    <div class="SRM-parent-container">
        <div class="SRM-quant-container" id="quantModule">
            <div id="complete-query-section" class="complete-query-section hidden">
                <%--<div class="query"></div>--%>
            </div>
            <div class="query-section">
                <div class="query-box">
                    <div class="query-token editable">
                        <input class="query-token-editable" type="text" />
                        <div class="token-suggestions">
                        </div>
                    </div>
                </div>
                <div style="display: inline-block;">
                    <div class="btn btn-md query-submit-btn">
                        Search</div>
                    <div class="btn btn-md query-clear-btn" style="display: none;">
                        Clear</div>
                </div>
            </div>
            <div id="result-section" class="result-section hidden">
                <div class="result-section-grid">
                    <radneoGrid:RADNeoGrid runat="server" ID="RADXLGridTest" Height="500px" PageSize="25"
                        Visible="true"></radneoGrid:RADNeoGrid>
                </div>
            </div>
            <div id="SRM-savedSearchesTitle">
                <div id="SRM-savedSearchesTitle">
                    SAVED SEARCHES</div>
                <div id="SRM-savedSearchListDiv">
                </div>
            </div>
            <div id="buttons-section" class="buttons-section hidden">
                <div>
                    <div class="btn btn-sm add-query-section-btn">
                        Refine Further</div>
                    <div class="btn btn-sm save-query-btn">
                        Save</div>
                    <div class="btn-email">
                    </div>
                    <div class="fa fa-refresh btn-refresh">
                    </div>
                </div>
            </div>
        </div>
        <div class="loading-screen-placeholder hidden">
            <div class="loading-elem">
                <image src="images/ajax-working.gif"></image>
            </div>
        </div>
    </div>
    <div id="srm_quant_messagePopup" class="srmQuantMessagePopupStyle" style="display: none;">
        <div class="fa fa-times srmQuantPopupCloseBtn">
        </div>
        <div class="srmQuantUpperSection">
            <div class="srmQuantpopupImage srmQuantHorizontalAlign">
                <img />
            </div>
            <div class="srmQuantStatuspopupTitle srmQuantHorizontalAlign">
            </div>
        </div>
        <div class="srmQuantStatusMessage">
        </div>
    </div>
    <%--<div id="srm_quant_emailPopup" style="display: none;">
        <div id="srm_quant_emailHeading">
            COMMUNICATE
        </div>
        <div class="fa fa-times srm_EmailCloseBtn">
        </div>
        <div class="srm_email_to_section"></div>
        <div class="srm_email_subject_section"></div>
        <div class="srm_email_body_section">
            <div class="srm_email_attachment_section">            
                <div class="fa fa-times srmQuantPopupCloseBtn"></div>
            </div>
        </div>
        <div class="srm_email_footer_section">
            <div class="">Send</div>
        </div>
    </div>--%>
    <div id="srm_quant_emailPopup" style="display: none;">
        <div id="communicateMainDiv1" class="communicateMainDiv" style="z-index: 5000; display: block;">
        <div id="addinbody" class="radaddinbody">
            <div id="addinheader" class="radaddinheader">EMAIL RESULTS
                <div id="radaddindestroy" class="fa fa-times" style="float:right;padding-right:6px;padding-top:3px; color:#B3B3B3;"></div>
            </div>
            <div id="addinstatus" class="radaddinstatus">
                <table id="main" style="border-bottom: 1px solid #E5E5E5;margin-left: 14px;margin-right: 24px;position: relative;   width: 97%;">
                    <tbody>
                        <tr id="addinsendparent" class="radaddinsendparent" style="height: 35px;line-height: 35px;">
                            <td id="addinsendto" class="radaddinsend">To</td>
                            <td style="width:100%;">
                                <div class="radaddinsendappenddiv ui-sortable" id="addinlistdiv"></div>
                                <div id="addinsendtext" contenteditable="true" class="radaddinsendtext"></div>
                                <div id="addincc" class="radaddincc">CC</div>
                                <div id="addinbcc" class="radaddinbcc">BCC</div>
                            </td>
                        </tr>

                        <tr id="addinccparent" style="height: 35px;line-height: 35px;display:none;">
                            <td id="addinccto" class="radaddinccattached" style="">CC</td>
                            <td>
                                <div class="radaddinccappenddiv ui-sortable" id="addincclistdiv">
                                    <div id="ccreconmailservice@ivp.in_1" class="radaddinappendeddiv">secmaster@ivp.in
                                        <div id="cross_reconmailservice@ivp.in" class="fa fa-times radaddincrossicon" style="display:none"></div>
                                    </div>
                                </div>
                                <div id="addincctext" contenteditable="true" class="radaddincctext" style="display: none; width: 0px;"></div>
                            </td>
                        </tr>
                        <tr id="addinbccparent" style="height: 35px;    line-height: 35px;display:none; position:relative">
                            <td id="addinbccto" class="radaddinbccattached" style="">BCC</td>
                            <td>
                                <div class="radaddinbccappenddiv ui-sortable" id="addinbcclistdiv"></div>
                                <div id="addinbcctext" contenteditable="true" class="radaddinbcctext" style="display:none"></div>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <div id="addinsubject" class="radaddinsubject" placeholder="Subject">
                    <input id="addinsubjecttext" class="radaddinsubjecttext" placeholder="Subject" onclick="clickSubject()">
                </div>

                <div class="jqte">
                    <div class="jqte_toolbar unselectable" role="toolbar" unselectable="on" style="user-select: none;"><div class="jqte_tool jqte_tool_1 unselectable" role="button" data-tool="0" unselectable="on" style="user-select: none;"><a class="jqte_tool_label unselectable" unselectable="on" style="user-select: none;"><span class="jqte_tool_text unselectable" unselectable="on" style="user-select: none;">Normal</span><span class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></span></a><div class="jqte_formats unselectable" unselectable="on" style="user-select: none; display: none;"><a jqte-formatval="p" class="jqte_format jqte_format_0 unselectable" role="menuitem" unselectable="on" style="user-select: none;">Normal</a><a jqte-formatval="h1" class="jqte_format jqte_format_1 unselectable" role="menuitem" unselectable="on" style="user-select: none;">Header 1</a><a jqte-formatval="h2" class="jqte_format jqte_format_2 unselectable" role="menuitem" unselectable="on" style="user-select: none;">Header 2</a><a jqte-formatval="h3" class="jqte_format jqte_format_3 unselectable" role="menuitem" unselectable="on" style="user-select: none;">Header 3</a><a jqte-formatval="h4" class="jqte_format jqte_format_4 unselectable" role="menuitem" unselectable="on" style="user-select: none;">Header 4</a><a jqte-formatval="h5" class="jqte_format jqte_format_5 unselectable" role="menuitem" unselectable="on" style="user-select: none;">Header 5</a><a jqte-formatval="h6" class="jqte_format jqte_format_6 unselectable" role="menuitem" unselectable="on" style="user-select: none;">Header 6</a><a jqte-formatval="pre" class="jqte_format jqte_format_7 unselectable" role="menuitem" unselectable="on" style="user-select: none;">Preformatted</a></div></div><div class="jqte_tool jqte_tool_2 unselectable" role="button" data-tool="1" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a><div class="jqte_fontsizes unselectable" unselectable="on" style="user-select: none;"><a jqte-styleval="10" class="jqte_fontsize unselectable" style="font-size: 10px; user-select: none;" role="menuitem" unselectable="on">Abcdefgh...</a><a jqte-styleval="12" class="jqte_fontsize unselectable" style="font-size: 12px; user-select: none;" role="menuitem" unselectable="on">Abcdefgh...</a><a jqte-styleval="16" class="jqte_fontsize unselectable" style="font-size: 16px; user-select: none;" role="menuitem" unselectable="on">Abcdefgh...</a><a jqte-styleval="18" class="jqte_fontsize unselectable" style="font-size: 18px; user-select: none;" role="menuitem" unselectable="on">Abcdefgh...</a><a jqte-styleval="20" class="jqte_fontsize unselectable" style="font-size: 20px; user-select: none;" role="menuitem" unselectable="on">Abcdefgh...</a><a jqte-styleval="24" class="jqte_fontsize unselectable" style="font-size: 24px; user-select: none;" role="menuitem" unselectable="on">Abcdefgh...</a><a jqte-styleval="28" class="jqte_fontsize unselectable" style="font-size: 28px; user-select: none;" role="menuitem" unselectable="on">Abcdefgh...</a></div></div><div class="jqte_tool jqte_tool_3 unselectable" role="button" data-tool="2" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a><div class="jqte_cpalette unselectable" unselectable="on" style="user-select: none;"><a jqte-styleval="0,0,0" class="jqte_color unselectable" style="background-color: rgb(0, 0, 0); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="68,68,68" class="jqte_color unselectable" style="background-color: rgb(68, 68, 68); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="102,102,102" class="jqte_color unselectable" style="background-color: rgb(102, 102, 102); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="153,153,153" class="jqte_color unselectable" style="background-color: rgb(153, 153, 153); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="204,204,204" class="jqte_color unselectable" style="background-color: rgb(204, 204, 204); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="238,238,238" class="jqte_color unselectable" style="background-color: rgb(238, 238, 238); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="243,243,243" class="jqte_color unselectable" style="background-color: rgb(243, 243, 243); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="255,255,255" class="jqte_color unselectable" style="background-color: rgb(255, 255, 255); user-select: none;" role="gridcell" unselectable="on"></a><div class="jqte_colorSeperator"></div><a jqte-styleval="255,0,0" class="jqte_color unselectable" style="background-color: rgb(255, 0, 0); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="255,153,0" class="jqte_color unselectable" style="background-color: rgb(255, 153, 0); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="255,255,0" class="jqte_color unselectable" style="background-color: rgb(255, 255, 0); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="0,255,0" class="jqte_color unselectable" style="background-color: rgb(0, 255, 0); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="0,255,255" class="jqte_color unselectable" style="background-color: rgb(0, 255, 255); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="0,0,255" class="jqte_color unselectable" style="background-color: rgb(0, 0, 255); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="153,0,255" class="jqte_color unselectable" style="background-color: rgb(153, 0, 255); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="255,0,255" class="jqte_color unselectable" style="background-color: rgb(255, 0, 255); user-select: none;" role="gridcell" unselectable="on"></a><div class="jqte_colorSeperator"></div><a jqte-styleval="244,204,204" class="jqte_color unselectable" style="background-color: rgb(244, 204, 204); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="252,229,205" class="jqte_color unselectable" style="background-color: rgb(252, 229, 205); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="255,242,204" class="jqte_color unselectable" style="background-color: rgb(255, 242, 204); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="217,234,211" class="jqte_color unselectable" style="background-color: rgb(217, 234, 211); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="208,224,227" class="jqte_color unselectable" style="background-color: rgb(208, 224, 227); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="207,226,243" class="jqte_color unselectable" style="background-color: rgb(207, 226, 243); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="217,210,233" class="jqte_color unselectable" style="background-color: rgb(217, 210, 233); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="234,209,220" class="jqte_color unselectable" style="background-color: rgb(234, 209, 220); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="234,153,153" class="jqte_color unselectable" style="background-color: rgb(234, 153, 153); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="249,203,156" class="jqte_color unselectable" style="background-color: rgb(249, 203, 156); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="255,229,153" class="jqte_color unselectable" style="background-color: rgb(255, 229, 153); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="182,215,168" class="jqte_color unselectable" style="background-color: rgb(182, 215, 168); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="162,196,201" class="jqte_color unselectable" style="background-color: rgb(162, 196, 201); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="159,197,232" class="jqte_color unselectable" style="background-color: rgb(159, 197, 232); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="180,167,214" class="jqte_color unselectable" style="background-color: rgb(180, 167, 214); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="213,166,189" class="jqte_color unselectable" style="background-color: rgb(213, 166, 189); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="224,102,102" class="jqte_color unselectable" style="background-color: rgb(224, 102, 102); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="246,178,107" class="jqte_color unselectable" style="background-color: rgb(246, 178, 107); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="255,217,102" class="jqte_color unselectable" style="background-color: rgb(255, 217, 102); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="147,196,125" class="jqte_color unselectable" style="background-color: rgb(147, 196, 125); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="118,165,175" class="jqte_color unselectable" style="background-color: rgb(118, 165, 175); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="111,168,220" class="jqte_color unselectable" style="background-color: rgb(111, 168, 220); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="142,124,195" class="jqte_color unselectable" style="background-color: rgb(142, 124, 195); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="194,123,160" class="jqte_color unselectable" style="background-color: rgb(194, 123, 160); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="204,0,0" class="jqte_color unselectable" style="background-color: rgb(204, 0, 0); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="230,145,56" class="jqte_color unselectable" style="background-color: rgb(230, 145, 56); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="241,194,50" class="jqte_color unselectable" style="background-color: rgb(241, 194, 50); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="106,168,79" class="jqte_color unselectable" style="background-color: rgb(106, 168, 79); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="69,129,142" class="jqte_color unselectable" style="background-color: rgb(69, 129, 142); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="61,133,198" class="jqte_color unselectable" style="background-color: rgb(61, 133, 198); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="103,78,167" class="jqte_color unselectable" style="background-color: rgb(103, 78, 167); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="166,77,121" class="jqte_color unselectable" style="background-color: rgb(166, 77, 121); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="153,0,0" class="jqte_color unselectable" style="background-color: rgb(153, 0, 0); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="180,95,6" class="jqte_color unselectable" style="background-color: rgb(180, 95, 6); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="191,144,0" class="jqte_color unselectable" style="background-color: rgb(191, 144, 0); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="56,118,29" class="jqte_color unselectable" style="background-color: rgb(56, 118, 29); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="19,79,92" class="jqte_color unselectable" style="background-color: rgb(19, 79, 92); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="11,83,148" class="jqte_color unselectable" style="background-color: rgb(11, 83, 148); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="53,28,117" class="jqte_color unselectable" style="background-color: rgb(53, 28, 117); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="116,27,71" class="jqte_color unselectable" style="background-color: rgb(116, 27, 71); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="102,0,0" class="jqte_color unselectable" style="background-color: rgb(102, 0, 0); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="120,63,4" class="jqte_color unselectable" style="background-color: rgb(120, 63, 4); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="127,96,0" class="jqte_color unselectable" style="background-color: rgb(127, 96, 0); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="39,78,19" class="jqte_color unselectable" style="background-color: rgb(39, 78, 19); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="12,52,61" class="jqte_color unselectable" style="background-color: rgb(12, 52, 61); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="7,55,99" class="jqte_color unselectable" style="background-color: rgb(7, 55, 99); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="32,18,77" class="jqte_color unselectable" style="background-color: rgb(32, 18, 77); user-select: none;" role="gridcell" unselectable="on"></a><a jqte-styleval="76,17,48" class="jqte_color unselectable" style="background-color: rgb(76, 17, 48); user-select: none;" role="gridcell" unselectable="on"></a></div></div><div class="jqte_tool jqte_tool_4 unselectable" role="button" data-tool="3" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a></div><div class="jqte_tool jqte_tool_5 unselectable" role="button" data-tool="4" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a></div><div class="jqte_tool jqte_tool_6 unselectable" role="button" data-tool="5" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a></div><div class="jqte_tool jqte_tool_7 unselectable" role="button" data-tool="6" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a></div><div class="jqte_tool jqte_tool_8 unselectable" role="button" data-tool="7" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a></div><div class="jqte_tool jqte_tool_9 unselectable" role="button" data-tool="8" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a></div><div class="jqte_tool jqte_tool_10 unselectable" role="button" data-tool="9" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a></div><div class="jqte_tool jqte_tool_11 unselectable" role="button" data-tool="10" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a></div><div class="jqte_tool jqte_tool_12 unselectable" role="button" data-tool="11" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a></div><div class="jqte_tool jqte_tool_13 unselectable" role="button" data-tool="12" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a></div><div class="jqte_tool jqte_tool_14 unselectable" role="button" data-tool="13" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a></div><div class="jqte_tool jqte_tool_15 unselectable" role="button" data-tool="14" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a></div><div class="jqte_tool jqte_tool_16 unselectable" role="button" data-tool="15" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a></div><div class="jqte_tool jqte_tool_17 unselectable" role="button" data-tool="16" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a></div><div class="jqte_tool jqte_tool_18 unselectable" role="button" data-tool="17" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a></div><div class="jqte_tool jqte_tool_19 unselectable" role="button" data-tool="18" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a></div><div class="jqte_tool jqte_tool_20 unselectable" role="button" data-tool="19" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a></div><div class="jqte_tool jqte_tool_21 unselectable" role="button" data-tool="20" unselectable="on" style="user-select: none;"><a class="jqte_tool_icon unselectable" unselectable="on" style="user-select: none;"></a></div></div>
                    <div class="jqte_editor" contenteditable="true">
                    </div>
                </div>

                <div style="width: 100px;position: absolute;bottom: -9px;left: 20px;">
                    <div id="attachmenticon" class="radaddinattachment fa fa-paperclip" style="-webkit-transform: rotate(45deg);"></div>
                    <div class="parentDiv" style="width:90px ; float:left; position:relative; top:-18px; left:17px ;">
                        <div class="radaddinattachmentText" id="attchmenttext" style="">Attachment</div>

                        <div style="width: 100%;" id="BulkTrans">
                            <div class="fuemailcontainerDiv" id="containerDivBulkTrans">
                                <input type="file" id="sBulkTrans" multiple="multiple" style="display: none;">
                                <div id="dropWindowDivBulkTrans" class="fudropWindowDiv" style="display: none; left: 743px;">Drop Files Here</div>
                                <div id="uploadedFilesBulkTrans">
                                    <div id="attachments1" class="radaddinattachmentinfocompose" +="" path="E:\IVPReconQASetup\CosmosReconFiles\ReconLoadingFiles\..\EmailAttachments\BreakDetails_20170412132146103.xls">Search Results.xls<i class="fa fa-times radaddinattachmentscross"></i>
                                    </div>
                                </div>
                            </div>
                            <div id="errorBulkTrans" class="fuerrorMessage"></div>
                        </div>
                    </div>
                    <!--div id="attchmenttext" class="radaddinattachmentText">Attachment<input type="file" id="attachingfile" style="display:inline;"/></div!-->
                </div>
                <div id="addinsend" class="radaddinsavemail" style="float: right;position: absolute;bottom: 3px;right: 10px;">
                    <input value="Send" type="button" class="sendClassbtn">
                    <div id="trash" style="display:none"></div>
                    <!-- <div id="addinfontdiv" class="radaddinfontdiv fa fa-text-width" class="radaddinfontsizediv"></div> -->
                    <div id="addinfontparentdiv" class="radaddinfontparentdiv" style="display:none;">
                        <div id="addinsmallfont" class="radaddinsmallfont">Small</div>
                        <div id="addinmediumfont" class="radaddinmediumfont">Normal</div>
                        <div id="addinlargefont" class="radaddinlargefont">Large</div>
                        <div id="addinhugefont" class="radaddinhugefont">Huge</div>
                    </div>
                </div>                
            </div>
        </div>
    </div>
    </div>
    <div id="srm_quant_savePopup" style="display: none;">
        <div class="fa fa-times srmQuantPopupCloseBtn" id="closeSavePopup">
        </div>
        <div class="srm_save_section1">
            <div class="srm_name">Name</div>
            <div class="srm_value">
                <input type="text" id="srm_save_searchName"/>
            </div>
        </div>
        <div class="srm_save_section2">
            <%--<input type="checkbox" id="srm_save_search_report" style="display: none;" /> --%>
            Do you want to create a report
            <div class="srm-toggle-body">
                <div class="srm_toggle ">On</div>
                <div class="srm_toggle active">Off</div>
            </div>
        </div>
        <div class="srm_save_section3" style="display: none;">
            <%--<input type="checkbox" id="srm_save_search_historical"  />--%> 
            Include Historical Data
            <div class="srm-toggle-body" style="margin-left: 56px;">
                <div class="srm_toggle active">On</div>
                <div class="srm_toggle">Off</div>
            </div>
            <div class="srm_save_subSection3">
                <span>Period</span>
                <div class="srm_save_reporting">
                    <span>From</span>
                    <input type = "text" id="fromDate" style="width:75px;border:none;border-bottom: dashed 1px #cecece;color: #00bff2; margin-left: 2px;" />
                    <%--<span id="fromDate" style="border-bottom: dashed 1px #cecece;">T-30</span>--%>
                    <span>To</span>
                    <%--<span id="toDate"style="border-bottom: dashed 1px #cecece;">Now</span>--%>
                    <input type = "text" id="toDate" style="width:75px;border:none;border-bottom: dashed 1px #cecece;color: #00bff2; margin-left: 2px;" />
                </div>
            </div>
        </div>
        <div class="srm_save_section4">
            <input value="Ok" type="button" class="sendClassbtn" id="saveQuertBtn">
        </div>
    </div>
    <script type="text/javascript">
        var radGridClientId = "<%= radGridClientId %>";
    </script>
    <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
    <script src="includes/jquery-ui.min.js" type="text/javascript"></script>
    <script src="js/thirdparty/knockout-3.4.0.js" type="text/javascript"></script>
    <script src="includes/xlgrid.loader.js" type="text/javascript"></script>
    <script src="includes/neogrid.client.js" type="text/javascript"></script>
    <script src="js/thirdparty/bootstrap.min.js" type="text/javascript"></script>
    <script src="includes/bootstrap2-toggle.js" type="text/javascript"></script>
    <script src="includes/jquery.slimscrollHorizontal.js" type="text/javascript"></script>
    <script src="includes/slimScrollHorizontal.js" type="text/javascript"></script>
    <script src="includes/jquery.slimscroll.js" type="text/javascript"></script>
    <script src="includes/ruleEditorScroll.js" type="text/javascript"></script>
    <script src="includes/ViewPort.js" type="text/javascript"></script>
    <script src="includes/RADCustomDateTimePicker.js" type="text/javascript"></script>
    <script src="includes/jquery.datetimepicker.js" type="text/javascript"></script>
    <script src="includes/script.js" type="text/javascript"></script>
    <script src="includes/SecMasterScripts.js" type="text/javascript"></script>
    <script type="text/javascript" src="js/quantJs.js"></script>
    </form>
</body>
</html>
