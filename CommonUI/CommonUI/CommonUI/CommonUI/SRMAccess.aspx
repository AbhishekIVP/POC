<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SRMAccess.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.SRMAccess" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
     <link href="css/SRMAccess.css" rel="stylesheet" />
</head>
<body style="overflow:hidden">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="smMaster" runat="server" ScriptMode="Release" EnableViewState="true"
            AllowCustomErrorsRedirect="false" OnAsyncPostBackError="smMaster_AsyncPostBackError"
            AsyncPostBackTimeout="20000" EnablePageMethods="true" LoadScriptsBeforeUI="true">
            <Services>
                <asp:ServiceReference Path="~/CommonUI/BaseUserControls/Service/RADNeoGridService.svc" />
            </Services>
            <Scripts>
                <asp:ScriptReference Path="ScriptIncludes/SecMasterJSCommon.debug.js" />
                <asp:ScriptReference Path="includes/RADBrowserScripts.debug.js" />
                <asp:ScriptReference Path="includes/RADDragDropScripts.debug.js" />
                <asp:ScriptReference Path="includes/RScriptUtils.debug.js" />
            </Scripts>
        </asp:ScriptManager>
        <div style="background-color: white; width: 100%;">
            <div style="display: block;border-bottom:2px solid #D3D3D3;border-top:2px solid #D3D3D3; background-color: #EEF1F3; height: 220px; width: 100%; padding-left: 10px; padding-top: 10px; font-size: 15px;">
                <div id="SRMAccessTypeId" style="width:98%;margin-top:15px;margin-left:10px;font-weight:bold;">SECURITY TYPE LEVEL</div>
    <div style="width: 97%; margin-left:10px; top: 60px;border-right:2px solid #D3D3D3; background-color:white; position: absolute; height: 70px; text-align: left;padding-left:10px">
        <div id="UsersText" style="height: 60px;font-weight:bold; display: inline-block; text-align:left; padding-top: 30px;margin-left: 7px;">
            USERS :
        </div>
        <div id="Users" style="text-align:left;display: inline-block;  height: 60px; padding-top: 10px;">
          
        </div>
    </div>
                <div style="width: 97%;margin-left:10px;top: 130px;font-size:14px; border-right:2px solid #D3D3D3; border-bottom:2px solid #D3D3D3;background-color:white; position: absolute; height: 60px; text-align: left;padding-left:10px">
                    <div id="GroupsText" style=" height: 60px;text-align:left;font-weight:bold; display: inline-block;  padding-top: 10px;">
                        GROUPS:
                    </div>
                    <div id="Group" style="text-align:left; display: inline-block;  height: 60px; border-top:2px dashed #EEF1F3; padding-top: 5px;">
                        NAMES
                    </div>
                </div>
            </div>

            <div ID="condition" style="display: inline-block; background-color:white; height: 366px; width: 50%; padding-left: 10px; padding-top: 10px; font-size: 15px;">
                SECURITIES
           <%--<div style="width: 100%; top: 70px; position: relative; height: 100px; text-align: center;">
               <div style="width: 20%; height: 60px; display: block;  padding-top: 20px;">
                   Condition 1
               </div>
               <div style="width: 79%; display: block;  height: 80px; padding-top: 30px; top: 5px; position: relative;">
                   NAMES
               </div>
           </div>

                <div style="width: 100%; top: 170px; position: relative; height: 100px; text-align: center;">
                    <div style="width: 20%; height: 60px; display: block; padding-top: 20px;">
                        Condition 1
                    </div>
                    <div style="width: 79%; display: block;  height: 80px; padding-top: 30px; top: 5px; position: relative;">
                        NAMES
                    </div>
                </div>--%>

            </div>
            <div id="template" style="display: inline-block; background-color: #FBFCFC; height: 366px; margin-top:20px; margin-left:30px;  width: 45%; border:1px solid #EEF1F3; padding-left: 10px; padding-top: 10px; position: absolute; font-size: 15px;">
                ATTRIBUTES
               <div style="width: 100%; top: 100px; position: relative; height: 100px; text-align: center;">
                   <div style="width: 20%; height: 80px; display: inline-block;  padding-top: 30px;">
                       Template 1
                   </div>
                   <div style="width: 79%; display: inline-block;  height: 80px; padding-top: 30px; position: relative;">
                       NAMES
                   </div>
               </div>

                <div style="width: 100%; top: 150px; position: relative; height: 100px; text-align: center;">
                    <div style="width: 20%; height: 80px; display: inline-block;  padding-top: 30px;">
                        Template 2
                    </div>
                    <div style="width: 79%; display: inline-block;  height: 80px; padding-top: 30px; position: relative;">
                        NAMES
                    </div>
                </div>
            </div>
        </div>

        <div id="SRMAccessIframeContainer" style="display:none">
            <div id="iframeHeader" class="SRMDataSourceAndMappingGridHeaders">
                <div class="SRMAccessHeaderColumn">               
                    <div class="SRMAccessGridValue" id="SRMAccessGridValue"></div>
                </div>
                <div class="SRMAccessGridValue_set_editdiv">
                    <div id="SRMAccessGridValue_edit" class="srmdatasourcesystemmapping_edit_button">Edit</div>
                </div>
                <div class="SRMAccessGridValue_set_deletediv">
                    <div id="SRMAccessHeaderClose" class="srmdatasourcesystemmapping_edit_button">Close</div>
                </div>
                
            </div>
            <iframe id="SRMAccessIframe" src="">

            </iframe>
        </div>

        <div id="SRMAccessPopup1" style="display:none"></div>
        <div id="SRMAccessPopup2" style="display:none"></div>

        
         

        <script type="text/javascript" src="js/thirdparty/jquery-1.11.3.min.js"></script>

        <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/jquery-ui.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/bootstrap.min.js" type="text/javascript"></script>
        <script src="includes/bootstrap2-toggle.js" type="text/javascript"></script>
        <script src="js/thirdparty/knockout-3.4.0.js" type="text/javascript"></script>
        <script src="includes/knockout-fast-foreach.min.js" type="text/javascript"></script>
        <script src="includes/jquery.slimscrollHorizontal.js" type="text/javascript"></script>
        <script src="includes/slimScrollHorizontal.js" type="text/javascript"></script>
        <script src="includes/jquery.slimscroll.js" type="text/javascript"></script>
        <script src="includes/ruleEditorScroll.js" type="text/javascript"></script>
        <script type="text/javascript" src="includes/xlgrid.loader.js"></script>
        <script type="text/javascript" src="includes/neogrid.client.js"></script>
        <script src="includes/XMLHttpSyncExecutor.js" type="text/javascript"></script>
        <script src="includes/viewManager.js" type="text/javascript"></script>
        <script src="includes/updatePanel.js" type="text/javascript"></script>
        <script src="includes/ViewPort.js" type="text/javascript"></script>
        <script src="includes/SecMasterScripts.js" type="text/javascript"></script>
        <script src="includes/script.js" type="text/javascript"></script>
        <script src="includes/grid.js" type="text/javascript"></script>
        <script src="includes/SMSelect.js" type="text/javascript"></script>
        <script src="includes/RADCustomDateTimePicker.js" type="text/javascript"></script>
        <script src="includes/jquery.datetimepicker.js" type="text/javascript"></script>
        <script src="includes/SMSlimscroll.js" type="text/javascript"></script>
        <script type="text/javascript" src="includes/SRMAssociation.js"></script>
        <script type="text/javascript" src="includes/SRMAccess.js"></script>
        <script type="text/javascript" src="includes/SMSlideMenu.js"></script>
    </form>
    <script type="text/javascript">
     
    </script>
</body>
</html>