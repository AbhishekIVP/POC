<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InstrumentListEventScreen.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.InstrumentListEventScreen" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="css/InstrumentListEventScreen.css" rel="stylesheet" />
</head>
<body>
    <form id="root" runat="server">
        <div id="EventScreen">
            <div class="searchContainer">
                <div id="instrumentSearch">
                    <i class="fa fa-lg fas fa-search"></i>
                    <input placeholder="Search"/>
                </div>
            </div>
            <div id="ModuleContainer">
            </div>
        </div>
        <div id="eventConfigErrorDiv_messagePopUp">
            <div class="eventConfigError_popupContainer">
                <div class="eventConfigError_popupCloseBtnContainer">
                    <div class="fa fa-times" id="eventConfigError_popupCloseBtn" onclick="javascript:$('#eventConfigErrorDiv_messagePopUp').hide(); $('#eventConfig_disableDiv').hide();"></div>
                </div>
                <div class="eventConfigError_popupHeader">
                    <div class="eventConfigError_imageContainer">
                        <img id="eventConfigError_ImageURL" />
                    </div>
                    <div class="eventConfigError_messageContainer">
                        <div class="eventConfigError_popupTitle">
                        </div>
                    </div>
                </div>
                <div class="eventConfigError_popupMessageContainer">
                    <div class="eventConfigError_popupMessage">
                    </div>
                </div>
                <div class="eventConfig_btns" style="display:none;">
                    <div class="eventConfig_popupOkBtn">Yes</div>
                </div>
            </div>
        </div>
        <div id="eventConfig_disableDiv" class="eventConfigDisableDiv">
        </div>
        <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/jquery-ui.min.js" type="text/javascript"></script>
        <script src="includes/MicrosoftAjax.js"></script>
        <script src="includes/SecMasterScripts.js"></script>
        <script src="includes/RefMasterUpgradeScript.js"></script>
        <script src="includes/XMLHttpSyncExecutor.js"></script>
        <script src="includes/InstrumentListEventScreen.js"></script>
    </form>
</body>
</html>
