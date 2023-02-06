<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMGrid.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMGrid" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <div id="SRMGridParent">
                <div id="SRMGridHeader">
                    <div class="SRMGridCommon SRMGridHeaderParent">
                        <div class="SRMGridHeaderText">Attribute</div>
                    </div>
                    <div class="SRMGridCommon SRMGridHeaderParent">
                        <div class="SRMGridHeaderText">Tab Name</div>
                    </div>
                    <div class="SRMGridCommon SRMGridHeaderParent">
                        <div class="SRMGridHeaderText">Sub Tab</div>
                    </div>
                    <div class="SRMGridCommon2 SRMGridHeaderParent">
                        <div class="SRMGridHeaderText">Mandatory</div>
                    </div>
                    <div class="SRMGridCommon2 SRMGridHeaderParent">
                        <div class="SRMGridHeaderText">Hidden</div>
                    </div>
                    <div class="SRMGridCommon2 SRMGridHeaderParent">
                        <div class="SRMGridHeaderText">Read Only</div>
                    </div>
                </div>

                <div id="SRMGridData" data-bind="foreach: templateList">
                    <div class="SRMGridRow">
                        <div class="SRMGridCommon SRMGridCellParent">
                            <div class="SRMGridText" data-bind="text: AttributeName"></div>
                        </div>
                        <div class="SRMGridCommon SRMGridCellParent">
                            <div class="SRMGridText" data-bind="text: TabName"></div>
                        </div>
                        <div class="SRMGridCommon SRMGridCellParent">
                            <div class="SRMGridText" data-bind="text: SubTabName"></div>
                        </div>
                        <div class="SRMGridCommon2 SRMGridTextAlign SRMGridCellParent">
                            <div class="SRMGridText" data-bind="text: Mandatory"></div>
                        </div>
                        <div class="SRMGridCommon2 SRMGridTextAlign SRMGridCellParent">
                            <div class="SRMGridText" data-bind="text: Hidden"></div>
                        </div>
                        <div class="SRMGridCommon2 SRMGridTextAlign SRMGridCellParent">
                            <div class="SRMGridText" data-bind="text: ReadOnly"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <link href="css/SRMGrid.css" rel="stylesheet" />
        <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/jquery-ui.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/knockout-3.4.0.js" type="text/javascript"></script>
        <script src="includes/MicrosoftAjax.js" type="text/javascript"></script>
        <script src="includes/RADCommonScripts.js" type="text/javascript"></script>
        <script src="includes/SecMasterScripts.js" type="text/javascript"></script>
        <script src="includes/RScriptUtils.debug.js" type="text/javascript"></script>
        <script src="includes/script.js" type="text/javascript"></script>
        <script src="includes/SRMGrid.js" type="text/javascript"></script>

    </form>
</body>
</html>
