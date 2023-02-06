var tabName = {};
tabName[3] = "Security Level";
tabName[6] = "Entity Level";
tabName[18] = "Fund Level";
tabName[20] = "Party Level";

var instrumentNamePrefix = {};
instrumentNamePrefix[3] = "Security Type";
instrumentNamePrefix[6] = "Entity Type";
instrumentNamePrefix[18] = "Fund Type";
instrumentNamePrefix[20] = "Party Type";

var actionTypeName = {};
actionTypeName[1] = "Create";
actionTypeName[2] = "Update";
actionTypeName[3] = "Attribute Update";
actionTypeName[4] = "Delete";
actionTypeName[5] = "Draft";
actionTypeName[6] = "Workflow Request Initiate";
actionTypeName[7] = "Workflow Request Approve";
actionTypeName[8] = "Workflow Request Reject";
actionTypeName[9] = "Workflow Request Edit";
actionTypeName[10] = "Workflow Request Cancel";
actionTypeName[11] = "Workflow Request Delete";
actionTypeName[12] = "Exception Raised";
actionTypeName[13] = "Exception Resolved";
actionTypeName[14] = "Exception Delete";
actionTypeName[15] = "Exception Suppress";
actionTypeName[16] = "Exception Unsuppress";

var instrumentLevelActions = [1, 2, 4, 5, 6, 7, 8, 9, 10, 12, 13, 14, 15, 16];
var attributeLegLevelActions = [3, 6, 7, 8, 9, 10, 12, 13, 14, 15, 16];

var actionItem = '<div class="action {0}" actionId="{1}"><div>{2}</div><i class="fa fas fa-lg fa-check-circle"></i><i class="fa fas fa-lg fa-circle-thin"></i></div>';
var configuredAttributeLegListItem = '<li class="{2}" configIndex="{0}"><div>{1}</div><i class="fa fa-lg fas fa-caret-right"></i><i class="fa fas fa-trash"></i></li>';
var availableAttributeLegListItem = "<li class='{2}' configIndex='{0}'><div>{1}</div><i class='fa fa-lg fas fa-caret-right'></i></li>";
var transportContainerItem = '<div class="multiSelectTag {1}">{0}<span>X</span></div>';
if (!String.format) {
    String.format = function (format) {
        var args = Array.prototype.slice.call(arguments, 1);
        return format.replace(/{(\d+)}/g, function (match, number) {
            return typeof args[number] != 'undefined'
                ? args[number]
                : match
                ;
        });
    };
}