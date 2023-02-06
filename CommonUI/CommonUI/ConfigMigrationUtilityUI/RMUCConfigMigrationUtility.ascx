<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RMUCConfigMigrationUtility.ascx.cs"
    Inherits="ConfigMigrationUtilityUI.RMUCConfigMigrationUtility" %>
<%--<div id="divcolor" class="ConfigheaderBlue">
    <div style="margin-left: 10px;">
        Config Migration Utility</div>
</div>--%>
<div class="middleContainerBefore">
</div>
<div class="middleContainer">
    <div class="middle-right">
        <%--<asp:Button ID="DownloadConfigBtn" runat="server" Text="Download Config" class="middle-right-button"
            Visible="false" />--%>
        <div id="tabsList" class="tabs-list" runat="server">
        </div>
    </div>
    <div class="middle">
        <div class="middle-left">
            <label id="abc" runat="server" class="middle-left-label">
                Source</label>
            <%--<asp:DropDownList ID="SourceList" runat="server" class="middle-left-dropDown">
        </asp:DropDownList>
            --%>
            <div id="Div_Source_Section">
                <div id="Div_Source">
                    <div id="Div_Select_Sources" onselectstart="return false;" runat="server">
                        <div id="psuedo_Div_Source" style="color: Black !important; text-indent: 4px; font-weight: normal !important;
                            line-height: 19px; background: #FFFFFF">
                            Select Source
                            <div class="Div_Arrow">
                            </div>
                        </div>
                        <div id="OptionsDiv">
                            <div id="Div_Option_Sources" style="display: inline" onselectstart="return false;"
                                runat="server">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <%--        <select id="DropDownDiv" runat="server">
        <option>Select Source</option>
        </select>--%>
            <asp:Button runat="server" Text="Upload Config File" ID="UploadConfigFileBtn" class="middle-left-button"
                Visible="false" />
        </div>
        <div class="middle-center">
            <asp:Label ID="Label1" Text="Destination : " runat="server" class="middle-center-label"></asp:Label>
            <%--<asp:TextBox ID="DestinationTB" runat="server" class="middle-center-textbox" Text="Production(this instance)"
            Enabled="false">
            </asp:TextBox>
            --%>
            <div id="Div_Destination_Section">
                <div id="Div_Destination" onselectstart="return false;" style="text-indent: 4px;
                    border-radius: 3px; height: 19px; line-height: 20px;" runat="server">
                </div>
            </div>
            <div id="CompareBtn" runat="server" class="middle-center-button middle-center-button-disabled">
                Compare</div>
            <div id="DownloadBtn" runat="server" class="middle-center-button">
                Download</div>
        </div>
    </div>
    <div id="SRMigrationFileUpload" style="width: 480px; margin-left: 55px;margin-bottom:5px;display:none;">
        <div style="width: 100%; cursor: pointer; margin: 0px auto; border: 0px;" id="SRMigrationFileUploadWidget">
            <div class="SRMigrationLabeledInput" style="width: 97%; border: 0px; padding: 0px; text-indent: 8px;"
                id="FileUploadAttachement">
                Click here or drop a file to upload
            </div>            
        </div>
    </div>
</div>
<div style="display: none">
    <iframe id="SRMigrationfiledownloadIframe" width="100%" style="text-align: center"></iframe>
</div>
<div id="ConfigPanelDiv">
    <div id="ConfigTabs" runat="server">
    </div>
</div>
<input type="hidden" runat="server" id="hdn_app" value="" />