<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="UserGroupLayoutPriorityReport.aspx.cs" 
    Inherits="com.ivp.common.CommonUI.CommonUI.UserGroupLayoutPriorityReport"  
    MasterPageFile="~/CommonUI/SRMMaster.Master" EnableEventValidation="false" %>
<asp:Content ID="contentMiddle" ContentPlaceHolderID="SRMContentPlaceHolderMiddle" runat="server">

    <script src="includes/UserGroupLayoutPriorityReport.js"></script>
   
    <link href="css/RADNeoClientSideGrid.css" rel="stylesheet" />
    <link href="css/thirdparty/fontawesome.css" rel="stylesheet" />
    <link href="css/thirdparty/fontawesome.min.css" rel="stylesheet" />
    <link href="css/SMSlideMenu.css" rel="stylesheet" />
    <link href="css/thirdparty/font-awesome.css" rel="stylesheet" type="text/css" />
      <link href="css/thirdparty/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="css/SMCss/SMSelect.css" rel="stylesheet" type="text/css" />
    <link href="css/CommonModuleTabs.css" rel="stylesheet" type="text/css" />

    <link href="css/Z_IagoCommonThemes.css" rel="stylesheet" type="text/css" />
    <link href="css/UserGroupLayoutPriorityReport.css" rel="stylesheet" />

    <%------------------------HTML------------------------------------------------------------%>
    
    <div id="UserGroupLayoutReportContainer">
         <div id="workFlowDetailsErrorDiv_messagePopUp" style="display: none;">
                <div class="workFlowDetailsError_popupContainer">
                    <div class="workFlowDetailsError_popupCloseBtnContainer">
                        <div class="fa fa-times" id="workFlowDetailsError_popupCloseBtn" onclick="javascript:$('#workFlowDetailsErrorDiv_messagePopUp').hide(); $('#workflowSetup_disableDiv').hide();"></div>
                    </div>
                    <div class="workFlowDetailsError_popupHeader">
                        <div class="workFlowDetailsError_imageContainer">
                            <img id="workFlowDetailsError_ImageURL" />
                        </div>
                        <div class="workFlowDetailsError_messageContainer">
                            <div class="workFlowDetailsError_popupTitle">
                            </div>
                        </div>
                    </div>
                    <div class="workFlowDetailsError_popupMessageContainer">
                        <div class="workFlowDetailsError_popupMessage">
                        </div>
                    </div>
                </div>
            </div>
    <div id="srm_UserGroupLayoutReport_RadNeoClientGridDiv"></div>

    </div>
     <div id="srm_mandatorydatareport_NoRecordsDiv" class="srm_mandatorydatareport_NoRecordDiv">
                    <div id="srm_mandatorydatareport_NoRecordsIcon"></div>
                    <div id="srm_mandatorydatareport_NoRecordMsg">No Records to display.</div>
                </div>
    <input type="text" hidden="hidden" id="preventSubmit"/>
</asp:Content>
