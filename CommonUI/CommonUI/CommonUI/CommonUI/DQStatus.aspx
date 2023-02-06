 <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DQStatus.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.DQStatus" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="css/DataQualityStatus.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        @font-face {
            font-family: "Lato";
            src: url("css/fonts/Lato-Regular.ttf") format("truetype");
        }
    </style>
    <link href="css/thirdparty/font-awesome.css" rel="stylesheet" type="text/css" />
    <link href="css/SMCss/Customjquery.datetimepicker.css" rel="stylesheet" type="text/css" />
    <link href="css/SMCss/SMDatePickerControl.css" rel="stylesheet" type="text/css" />
    <!-- To be Removed -->
    <link href="css/AquaStyles.css" rel="stylesheet" type="text/css" />
    <!-- To be Removed -->
</head>
<body>
    <form id="dataQualityStatusForm" runat="server">

        <asp:ScriptManager ID="smExternalSystemStatus" runat="server" ScriptMode="debug">
            <Scripts>
                <asp:ScriptReference Name="com.ivp.rad.raduicontrols.Scripts.RADCommonScripts.js"
                    Assembly="RADUIControls" />
                <asp:ScriptReference Name="com.ivp.rad.raduicontrols.Scripts.RADUIScripts.debug.js"
                    Assembly="RADUIControls" />
                <asp:ScriptReference Name="com.ivp.rad.raduicontrols.Scripts.RScriptUtils.debug.js"
                    Assembly="RADUIControls" />
            </Scripts>
        </asp:ScriptManager>
    
     <div class="HeaderFilter">
         <span id="DqmDatesFilter" class="DqmDatesFilter">
                            <span id="DqmDatesFilterText">Today</span>
                            <i class="fa fa-chevron-down" style="padding-left: 5px; font-size: 10px;"></i>
         </span>       
               
    </div>



    <div class="mainDqmBody">
                        <!--The below div contains the Date Filter-->
   
      
    <div class="srmDQMBody" id ="srmDQMBody">
        <%--<div class="srmDQMWarningHeader">Warning</div>--%>
        <div data-bind="foreach:chainListing" class="srmDQMchainParentDiv">
            <div class="srmDQMChain" data-bind="style: { borderColor: chainInfoBackgroundColor }">
                <div class="srmDQMRowIcon" data-bind="click:srmDQMStatus.onClickChain">
                    <i data-bind="visible:(showTasks() === false)" class="fa fa-caret-right" style="line-height: 20px;"></i>            <!-- !showTasks se bhi could have written-->
                    <i data-bind="visible:showTasks" class="fa fa-caret-down" style="line-height: 20px;"></i>                           
                </div>
                <div class="srmDQMIcon" data-bind="style: { background: chainStatusIconStyle }">
                    <i data-bind="visible:(state == 'Failed')" class="fa fa-close" style="line-height: 20px;"></i>
                    <i data-bind="visible:(state == 'Warning')" class="fa fa-exclamation" style="line-height: 20px;"></i>               <!--//decided backgr upon status-->
                    <i data-bind="visible:(state == 'Passed')" class="fa fa-check" style="line-height: 20px;"></i>
                </div>
                <div class="srmDQMChainName" data-bind="text:chainName,click:srmDQMStatus.onClickChain,style: { color: chainStatusIconStyle }"></div>
                <div class="srmDQMChainInfo" data-bind="style: { background: chainInfoBackgroundColor }">
                    <div class="srmDQMChainSubInfo1">
                        <div class="srmDQMChainSubInfo" data-bind="text:'Time : '+chainTime"></div>
                        <div class="srmDQMChainSubInfo" data-bind="text:'Date : '+chainDate"></div>
                    </div>
                    <div class="srmDQMChainSubInfo2">
                        <div class="srmDQMChainSubInfo" data-bind="text:'Avg : '+chainAvgDuration"></div>
                        <div class="srmDQMChainSubInfo" data-bind="text:'Duration : '+chainDuration"></div>
                    </div>
                </div>
                <%--<div class="srmDQMWarningIcon" data-bind="style: { width: (warningCount > 0) ? '10.6%' : '10%' }">--%>
                <%--<div class="srmDQMWarningIcon">
                    <i data-bind="visible:(state == 'Passed')" class="fa fa-thumbs-up" style="line-height: 34px;"></i>
                    <i data-bind="visible:(state == 'Failed')" class="fa fa-thumbs-down" style="line-height: 34px;"></i>
                    <span data-bind="text:warningCount,visible:(state == 'Warning')" style="line-height: 34px;">></span>
                </div>--%>
                <div data-bind="visible:showTasks">
                    <div class="srmDQMTaskInfoHead">Task Name</div>
                    <div class="srmDQMTaskInfoHead">Task Type</div>
                    <div class="srmDQMTaskInfoHead">Task Duration</div>
                </div>
                <div data-bind="foreach:taskListing,visible:showTasks">                                     <!-- tasks displayed-->
                    <div class="" data-bind="click:srmDQMStatus.onClickTask">
                        <div class="srmDQMTaskInfo" data-bind="text:taskName"></div>
                        <div class="srmDQMTaskInfo" data-bind="text:taskType"></div>
                        <div class="srmDQMTaskInfo" data-bind="text:taskDuration"></div>
                    </div>
                </div>
            </div>
        </div>
        <div class="srmDQMWarningParentDiv">
            <div class="srmDQMWarningHeader">Warning</div>
            <div data-bind="foreach:chainListing">
                <div class="srmDQMWarningIcon">
                    <i data-bind="visible:(state == 'Passed')" class="fa fa-thumbs-up srmDQMWarningThumbIcon"></i>
                    <i data-bind="visible:(state == 'Failed')" class="fa fa-thumbs-up srmDQMWarningThumbIcon"></i>
                    <span data-bind="text:warningCount,visible:(state == 'Warning')" style="line-height: 34px;">></span>
                </div>
            </div>
        </div>
        <div class="fa fa-caret-right srmDQMChartArrow"></div>        
    </div>
    <div id="srmDQMChartContainer">
            <%--<div id="sqmDQMChart" class="sqmDQMChart" style="width:800px;height:400px;">
            </div>--%>
    

    </div>  
        
        
        <script src="js/thirdparty/jquery-1.11.3.min.js" type="text/javascript"></script>
        <script src="js/thirdparty/--ui.min.js" type="text/javascript"></script>

        <script src="includes/SMSlimscroll.js" type="text/javascript"></script>


        <script src="includes/RADCommonScripts.js" type="text/javascript"></script>
        <script src="includes/RScriptUtils.debug.js" type="text/javascript"></script>
        

        <script src="includes/script.js" type="text/javascript"></script>


        <script src="includes/jquery.datetimepicker.js" type="text/javascript"></script>
        <script src="includes/SMDatePickerControl.js" type="text/javascript"></script>
        


        <script src="js/thirdparty/knockout-3.4.0.js" type="text/javascript"></script>
        <script src="includes/highcharts.js" type="text/javascript"></script>
        <script src="includes/highstock.js" type="text/javascript"></script>
        <%--<script src="includes/DataQuality.js" type="text/javascript"></script>--%>


        <script src="includes/srmDqmCommon.js" type="text/javascript"></script>
        <script src="includes/srmDqmSM.js" type="text/javascript"></script>
        <script src="includes/srmDqmRM.js" type="text/javascript"></script>
        <script src="includes/dataQualityJSON.js" type="text/javascript"></script>    
       
        <asp:HiddenField ID ="hdnTopOption" runat ="server" />

    </form>
</body>

</html>
