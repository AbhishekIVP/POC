<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InstrumentEventConfig.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.InstrumentEventConfig" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="css/thirdparty/font-awesome.css" rel="stylesheet" type="text/css" />
    <link href="css/InstrumentEventConfig.css" rel="stylesheet" />
</head>
<body>
    <form id="ConfigRoot" runat="server">
        <div id="mainContainer" style="display: none; flex-direction: column">
            <div class="configTabContainerDiv">
                <div class="leftMainGroup">
                    <div id="InstrumentTitle">
                    </div>
                </div>
                <div>
                    <div class="configTab configTabSelected">Instrument Level</div>
                    <div class="configTab">Attribute Level</div>
                    <div class="configTab">Leg Level</div>
                </div>
                <div class="rightMainGroup">
                    <div class="backButton">
                        <i class="fa fas fa-lg fa-th-large"></i>
                    </div>
                    <div id="saveButton" class="saveButton">Save</div>
                </div>
            </div>
            <div id="InstrumentLevel" class="configTabContent" style="display: block">
                <div class="actionConfig" style="display: block">
                    <div class="configRightPanel fullscreenPanel">
                        <div class="multiSelectTagWidget">
                            <div class="multiSelectTitle">Queues</div>
                            <div class="multiSelectTagContainer"></div>
                            <div class="multiSelectTagDropDown">
                                <div></div>
                            </div>
                        </div>
                        <div class="configBodyTopBar">
                            <div class="selectAllDiv">
                                <i class="fa fas fa-lg fa-check-circle"></i>
                                <div>Select All</div>
                            </div>
                        </div>
                        <div class="configBody">
                            <div class="actionsContainer">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div id="AttributeLevel" class="configTabContent" style="display: none">
                <div class="configLeftPanel">
                    <div class="configuredAttributeSection">
                        <div class="configuredAttributeTitle">
                            Configured Attributes
                        </div>
                        <ul class="configuredAttributeLegList">
                        </ul>
                    </div>
                    <div class="availableAttributeSection">
                        <div class="availableAttributeTitle">
                            Non Configured Attributes
                        </div>
                        <div class="availableAttributeSearch">
                            <i class="fa fas fa-search"></i>
                            <input />
                        </div>
                        <ul class="availableAttributeLegList">
                        </ul>
                    </div>
                </div>
                <div class="configRightPanel">
                    <div class="actionConfig">
                        <div class="multiSelectTagWidget">
                            <div class="multiSelectTitle">Queues</div>
                            <div class="multiSelectTagContainer">
                            </div>
                            <div class="multiSelectTagDropDown">
                                <div></div>
                            </div>
                        </div>
                        <div class="configBodyTopBar">
                            <div class="selectAllDiv">
                                <i class="fa fas fa-lg fa-check-circle"></i>
                                <div>Select All</div>
                            </div>
                        </div>
                        <div class="configBody">
                            <div class="actionsContainer">
                            </div>
                        </div>
                    </div>
                    <div class="placeholderForSelection">Please select a attribute.</div>
                </div>
            </div>
            <div id="LegLevel" class="configTabContent" style="display: none">
                <div class="configLeftPanel">
                    <div class="configuredAttributeSection">
                        <div class="configuredAttributeTitle">
                            Configured Legs
                        </div>
                        <ul class="configuredAttributeLegList">
                        </ul>
                    </div>
                    <div class="availableAttributeSection">
                        <div class="availableAttributeTitle">
                            Non Configured Legs
                        </div>
                        <div class="availableAttributeSearch">
                            <i class="fa fas fa-search"></i>
                            <input />
                        </div>
                        <ul class="availableAttributeLegList">
                        </ul>
                    </div>
                </div>
                <div class="configRightPanel">
                    <div class="actionConfig">
                        <div class="multiSelectTagWidget">
                            <div class="multiSelectTitle">Queues</div>
                            <div class="multiSelectTagContainer">
                            </div>
                            <div class="multiSelectTagDropDown">
                                <div></div>
                            </div>
                        </div>
                        <div class="configBodyTopBar">
                            <div class="selectAllDiv">
                                <i class="fa fas fa-lg fa-check-circle"></i>
                                <div>Select All</div>
                            </div>
                        </div>
                        <div class="configBody">
                            <div class="actionsContainer">
                            </div>
                        </div>
                    </div>
                    <div class="placeholderForSelection">Please select a leg.</div>
                </div>
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
        <script src="includes/SecMasterScripts.js"></script>
        <script src="includes/InstrumentEventConfigInfo.js"></script>
        <script src="includes/InstrumentEventConfig.js"></script>
    </form>
</body>
</html>
