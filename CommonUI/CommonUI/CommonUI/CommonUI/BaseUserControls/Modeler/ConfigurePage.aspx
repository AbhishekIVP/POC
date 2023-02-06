<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfigurePage.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.BaseUserControls.Modeler.ConfigurePage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../../includes/Altshut.js"></script>
    <script src="../../includes/Tags.js"></script>
    <script src="../../js/thirdparty/jquery-1.11.3.min.js"></script>
    <script src="../../includes/ConfigurePage.js"></script>
    <script src="../../includes/SMSlimscroll.js"></script>
    <link href="../../css/SMCss/Altshut.css" rel="stylesheet" />
    <link href="../../css/SMCss/Tags.css" rel="stylesheet" />
    <link href="../../css/thirdparty/ConfigurePage.css" rel="stylesheet" />
    <link href="../../css/thirdparty/bootstrap-theme.css" rel="stylesheet" />
    <link href="../../../App_Themes/Aqua/thirdparty/bootstrap.css" rel="stylesheet" />
</head>
<body>
    
    <form id="SRCform" runat="server">
        <div id="savebut">
            <button id="ConfigureSave">Save</button>
        </div>
        <div id="SRCContainerNew">
          
            <div class="secTypeRow">
                <div class="secTypeColumn">
                    Security Type Name:
                </div>
                <div class="secTypeColumn" style="margin-left:65px;">
                    <input type="text" id="txtSecName" style="width:200px"/>
                </div>
                <div class="secTypeColumn" style="margin-left:80px;">
                    Security type:
                </div>
                <div class="secTypeColumn" style="margin-left:10px;">
                    <input type="radio" id="vanilla" name="SecType" value="vanilla" checked=""/>Vanilla Strucure
                    <input type="radio" id="Exotic" name="SecType" value="exotic" checked=""/>Exotic Structure
                 
                </div>
                <div class="secTypeColumn" id="basketcheck" style="display:inline-block;float:right;">
                    <input type="checkbox" name="basket" id="basketinfo" checked="" />Basket Structure
                   
                </div>
                </div>
            
            <div class="secTypeRow">
                <div class="secTypeColumn">
                    Security type Description:
                </div>
           
                <div class="secTypeColumn" style="margin-left:25px;vertical-align:middle">
                  
                    <textarea rows="3" cols="25" id="txtSecDesc" style="margin-left:10px;"></textarea>
                
                </div>
                <div style="position: relative; display: inline-block;margin-left:80px;">
                    <div style="position: relative;">
                        <div class="secTypeColumnn" style="display: inline-block;">
                            Creation Type:
                        </div>
                        <div class="secTypeColumnn" style="display: inline-block;">
                            <input type="radio" id="scratchcreate" name="CreateOp" disabled="disabled" checked="checked"/>Create from Scratch
                            &nbsp
                            <input type="radio" id="existingcreate" name="CreateOp" disabled="disabled" checked=""/>Create from Existing
                          
                        </div>
                    </div>
                </div>
            </div>
            <div class="secTypeRow">
                    <div style="position: relative;display:inline-block;">
                        <div class="secTypeColumnn"style="display: inline-block;" >
                            Default Security Creation Date:
                        </div>
                        <div class="secTypeColumnn" style="display: inline-block;">
                            <select  id="dateoptions" style="margin-left:5px;width:200px;">
                                <option value="custom" selected="selected">Custom</option>
                                <option value="fcm">First Day of Current Month</option>
                                <option value="fdq">First Day of Current Quarter</option>
                                <option value="fdw">First Day of Current Week</option>
                                <option value="fdy">First Day of Current Year</option>
                                <option value="lpm">Last Day of Previous Month</option>
                                <option value="lpq">Last Day of Previous Quarter</option>
                                <option value="lpw">Last Day of Previous Week</option>
                                <option value="lpy">Last Day of Previous Year</option>
                                <option value="now">Now</option>
                                <option value="TNd">T-N Days</option>

                            </select>
                        </div>
                        <div id="DateSelection" style="display:inline-block;">
                            <div class="secTypeColumnn" style="display: inline-block;margin-left:80px;">
                                    Custom Creation Date:
                            </div>
                            <div class="secTypeColumnn" style="display: inline-block;">
                                <input type="date" id="dateinput" value=""/>
                                  
                            </div>
                        </div>
                        </div>
                </div>
          
            
            <div id="Tr1" class="secTypeRow"  runat="server">
                <div class="secTypeColumnn" style="width: 153px;display:inline-block;">
                    Tags:
                </div>
                <div class="secTypeColumnn" style="width: 364px;display:inline-block;">
                    <div class="tagContainer" id="divTag" runat="server" style="width: 200px;margin-left:30px;overflow:auto;">
                    </div>
                </div>
                
            </div>

            <div id="Tr3" class="secTypeRow" runat="server">
                <div class="secTypeColumnn" >
                    Allowed Users/Groups:
                </div>
                <div style="width: 800px; position: relative; margin: auto;margin-top:10px;margin-left:185px;">
                    <div class="secTypeColumn" style="width: 350px; float: left;">
                        <div runat="server" id="divUsers" clientidmode="Static">
                        </div>
                    </div>
                    <div class="secTypeColumn" style="width: 350px;">
                        <div runat="server" id="divGroups" clientidmode="Static">
                        </div>
                    </div>
                </div>
            </div>

        </div>
        <input type="button" id="ToAttributePage" value="Next" onclick="RedirectToAttributeSetupPage()"/>
       
        <input type="hidden" runat="server" id="hdnModuleId" />
        <input type="hidden" runat="server" id="hdnEntityTypeId" />
        <input type="hidden" class="hdnTag" value="" />
    
    </form>
</body>
</html>
