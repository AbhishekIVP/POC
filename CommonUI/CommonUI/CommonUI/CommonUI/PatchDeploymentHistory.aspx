<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PatchDeploymentHistory.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.PatchDeploymentHistory" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="css/thirdparty/font-awesome.css" rel="stylesheet" type="text/css" />
    <link href="css/PatchDeploymentHistory.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="patchDeploymentHistoryRoot" runat="server">
        <div>
            <div class="deploymentTitle">
                Patch Deployment History
                <div class="exportButton">Export
                    <i class="fa fa-caret-down"></i>
                    <div class="exportOptions">
                        <div class="exportOption">Build History</div>
                        <div class="exportOption">Hotfix History</div>
                    </div>
                </div>
            </div>
            <div id="deploymentHistory">
                <div class="deploymentHistoryHeaderRow">
                    <div class="deploymenHeader">Version</div>
                    <div class="deploymenHeader">SM Version</div>
                    <div class="deploymenHeader">RM Version</div>
                    <div class="deploymenHeader">RAD Version</div>
                    <div class="deploymenHeader">Deployment Date</div>
                    <div class="deploymenHeader"></div>
                    <div class="deploymenHeader"></div>
                </div>
                <div class="deploymentHistoryRows" id="deploymentHistoryRows">
                </div>
            </div>
        </div>
        <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/jquery-ui.min.js" type="text/javascript"></script>
        <script src="includes/SecMasterScripts.js"></script>
        <script src="includes/PatchDeploymentHistory.js"></script>
    </form>
</body>
</html>

