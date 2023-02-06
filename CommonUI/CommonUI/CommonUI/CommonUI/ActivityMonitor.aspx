<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ActivityMonitor.aspx.cs" Inherits="com.ivp.common.CommonUI.CommonUI.ActivityMonitor" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>

<body>
    <form id="form1" runat="server">
        <div style="width:auto;margin-top:20px;" align="right">
            <input type="button" id="refresh" value="Refresh" />
        </div>
        <div style="padding: 10px; z-index: 1; border: 5px solid grey; margin-top: 20px" class="container-fluid">
            <table id="TableID" class="display">
                <thead>
                    <tr>
                        <th>SPID</th>
                        <th>Status</th>
                        <th>Login</th>  
                        <th>Host_Name</th>  
                        <th>Blocked_By</th>  
                        <th>Database_Name</th>  
                        <th>Command</th>  
                        <th>CPU_Time</th>
                        <th>Disk_IO</th>  
                        <th>Last_Batch</th>  
                        <th>Program_Name</th>  
                        <th>Head_Blocker</th>
                        <th>Request_Id</th>
                        <th>Text</th>
                    </tr>
                </thead>
            </table>
        </div>
        <script src="js/thirdparty/jquery-1.11.3.min.js"></script>
        <link href="css/datatable.css" rel="stylesheet" />
        <script src="includes/datatable.js"></script>
        <link rel="Stylesheet" type="text/css" href="css/ActivityMonitor.css" />
         <link type="text/css" rel="stylesheet" href="css/thirdparty/bootstrap.min.css" />
        <script src="js/thirdparty/popper.min.js"></script>
        <script type="text/javascript" src="js/thirdparty/bootstrap.min.js"></script>
        <script src="includes/ActivityMonitor.js"></script>
    </form>
</body>
</html>
