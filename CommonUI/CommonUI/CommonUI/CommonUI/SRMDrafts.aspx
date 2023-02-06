<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMDrafts.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMDrafts" 
    MasterPageFile="~/CommonUI/SRMMaster.Master" EnableEventValidation="false" %>

<asp:Content ID="contentMiddle" ContentPlaceHolderID="SRMContentPlaceHolderMiddle" runat="server">
    <script src="includes/SRMDrafts.js" type="text/javascript"></script>
       <link href="css/thirdparty/bootstrap.min.css" rel="stylesheet" type="text/css" />
    <link href="css/SRMDrafts.css" rel="stylesheet" type="text/css" />
    <div id="draftsScreenSection">
        <div id="draftsActionSection" class="draftsActionSectionStyle">
            <div id="draftsActionList" class="draftsActionListStyle">
                <div id="draftsStatusDeleteAction" class="draftsAction draftsDisabled">Delete</div>
                <div id="draftsStatusRefreshButton" class="draftsStatusRefreshButtonParent">
                    <i class="fa fa-refresh btnClassHeader draftsStatusRefreshButtonDiv"></i>
                </div>
            </div>
            <div style="float: right;">
                
            </div>
        </div>
        <div id="draftsNoGridSection" style="display: none;">
            <div class="draftsNoGridSection"></div>
        </div>
        <div id="draftsStatusNeoGrid">
        </div>
    </div>
                
    </asp:Content>

<asp:Content ID="contentBottom" ContentPlaceHolderID="SRMContentPlaceHolderBottom" runat="server">
    <!--Error Popup-->
                <div id="os_messagePopup" class="draftsMessagePopupStyle" style="display: none;">
                    <div class="fa fa-times draftsPopupCloseBtn">
                    </div>
                    <div class="draftsUpperSection">
                        <div class="draftspopupImage draftsHorizontalAlign">
                            <img />
                        </div>
                        <div class="draftspopupTitle draftsHorizontalAlign">
                        </div>
                    </div>
                    <div class="draftsMessage">
                    </div>
                </div>
</asp:Content>