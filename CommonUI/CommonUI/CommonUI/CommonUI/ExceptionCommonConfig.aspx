<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="~/CommonUI/ExceptionCommonConfig.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.ExceptionCommonConfig" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
    <script src="includes/SecMasterScripts.js"></script>
    <link href="css/ExceptionCommonConfig.css" rel="stylesheet" />
    <script src="includes/ExceptionCommonConfig.js"></script>
    <link href="css/thirdparty/font-awesome.min.css" rel="stylesheet" type="text/css" />
    <form id="form1" runat="server">
        <div>

            <div style="
    padding-bottom: 5px;
    border-bottom: 1px solid #f2f2f2;
">
                <div style="float: left; padding: 7px 0 0 9px; font-size: 11px; font-weight: bold;">
                    Exception Preference
                </div>
                <div style="text-align: right; padding: 2px 15px 0 0;font-family:lato">
                    <div class="button" id="saveButton">Save</div>
                </div>
            </div>
            <div id="exceptionList" class="filtersection" style="display: none">
                <div class="filterItem filterSelectAll">
                    <div class="filterItemCheckbox"><i class="fa fas fa-check fa-xs"></i></div>
                    <div class="filterItemText">Select All</div>
                </div>
                <div class="filterItem filterOptions">
                    <div class="filterItemCheckbox"><i class="fa fas fa-check fa-xs"></i></div>
                    <div class="filterItemText">Alerts</div>
                </div>
                <div class="filterItem filterOptions">
                    <div class="filterItemCheckbox"><i class="fa fas fa-check fa-xs"></i></div>
                    <div class="filterItemText">Duplicates</div>
                </div>
                <div class="filterItem filterOptions">
                    <div class="filterItemCheckbox"><i class="fa fas fa-check fa-xs"></i></div>
                    <div class="filterItemText">1st Vendor Value Missing</div>
                </div>
                <div class="filterItem filterOptions">
                    <div class="filterItemCheckbox"><i class="fa fas fa-check fa-xs"></i></div>
                    <div class="filterItemText">Invalid Data</div>
                </div>
                <div class="filterItem filterOptions">
                    <div class="filterItemCheckbox"><i class="fa fas fa-check fa-xs"></i></div>
                    <div class="filterItemText">No Vendor Value</div>
                </div>
                <div class="filterItem filterOptions">
                    <div class="filterItemCheckbox"><i class="fa fas fa-check fa-xs"></i></div>
                    <div class="filterItemText">Ref Data Missing</div>
                </div>
                <div class="filterItem filterOptions">
                    <div class="filterItemCheckbox"><i class="fa fas fa-check fa-xs"></i></div>
                    <div class="filterItemText">Show As Exception</div>
                </div>
                <div class="filterItem filterOptions">
                    <div class="filterItemCheckbox"><i class="fa fas fa-check fa-xs"></i></div>
                    <div class="filterItemText">Validations</div>
                </div>
                <div class="filterItem filterOptions">
                    <div class="filterItemCheckbox"><i class="fa fas fa-check fa-xs"></i></div>
                    <div class="filterItemText">Value Change</div>
                </div>
                <div class="filterItem filterOptions">
                    <div class="filterItemCheckbox"><i class="fa fas fa-check fa-xs"></i></div>
                    <div class="filterItemText">Vendor Mismatch</div>
                </div>
                <div id="underlierDiv" class="filterItem filterOptions" style="display: none">
                    <div class="filterItemCheckbox"><i class="fa fas fa-check fa-xs"></i></div>
                    <div class="filterItemText">Underlier Missing</div>
                </div>
            </div>
            <div id="exceptionConfigErrorDiv_messagePopUp" style="display: none;">
                <div class="exceptionConfigError_popupContainer">
                    <div class="exceptionConfigError_popupCloseBtnContainer">
                        <div class="fa fa-times" id="exceptionConfigError_popupCloseBtn" onclick="javascript:$('#exceptionConfigErrorDiv_messagePopUp').hide(); $('#exceptionConfig_disableDiv').hide();"></div>
                    </div>
                    <div class="exceptionConfigError_popupHeader">
                        <div class="exceptionConfigError_imageContainer">
                            <img id="exceptionConfigError_ImageURL" />
                        </div>
                        <div class="exceptionConfigError_messageContainer">
                            <div class="exceptionConfigError_popupTitle">
                            </div>
                        </div>
                    </div>
                    <div class="exceptionConfigError_popupMessageContainer">
                        <div class="exceptionConfigError_popupMessage">
                        </div>
                    </div>
                </div>
            </div>
            <div id="exceptionConfig_disableDiv" class="exceptionConfigDisableDiv" style="display: none; z-index: 99998;" align="center">
            </div>
        </div>
    </form>
</body>
</html>
