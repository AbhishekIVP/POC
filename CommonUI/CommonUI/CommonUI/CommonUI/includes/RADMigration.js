var RADMigration = {};

RADMigration.initialize = function (initObj) {
    $.extend(RADMigration, initObj);

    if (!initObj.IsIagoDependent) {
        $(".iago-page-title").css("display", "none");
        $("#" + RADMigration.contentBodyId).empty();
    }
    else {
        $("#RUserManementTabDiv").css("display", "none");
        if ($("#pageHeaderTabPanel").data("iago-widget-tabs") != null)
            $("#pageHeaderTabPanel").data("iago-widget-tabs").destroy();
    }

    var obj = new Migration();
    obj.init();
};


var Migration = function () {

};


Migration.prototype.init = function () {

    Migration.instance = this;
    var self = this;
    var url = RADMigration.baseUrl + "/Resources/Services/RADUserManagement.svc";
    $.ajax({
        url: url + '/GetTagTemplates',
        type: 'POST',
        contentType: "application/json",
        dataType: 'json',
        data: JSON.stringify({ fileName: 'RadMigration.html' })
    }).then(function (responseText) {
        $("#" + RADMigration.contentBodyId).empty();
        $("#" + RADMigration.contentBodyId).append(responseText.d);
        $(".action").click(function (event) {
            self.clickHandler(event);
        });
        $('#GroupsScrollLeft').click(function (event) {
            self.clickHandler(event);
        });
        $('#GroupsScrollRight').click(function (event) {
            self.clickHandler(event);
        });
        $("#GroupActionDownload").hide();
        $("#GroupActionUpload").hide();
        //$(".MgrClearAll").hide();
        var grps = $('.MigrationGroups');
        $(grps[0]).find('.Downloadaction').click();
    });
};


Migration.prototype.clickHandler = function (event) {
    if ($(event.target).hasClass("action") && !($(event.target).hasClass("selected"))) {
        $('.action').removeClass('selected');
        $('.MigrationGroups').removeClass("selected");
        $(event.target).addClass("selected");
        $(event.target).closest('.MigrationGroups').addClass("selected");
        if ($(event.target).hasClass('Uploadaction')) {
            $("#GroupActionDownload").hide();
            $("#GroupActionUpload").show();
        }
        else {
            $("#GroupActionUpload").hide();
            $("#GroupActionDownload").show();
        }
        var groupname = $(event.target).closest(".MigrationGroups").find(".MigrationGroupName").html();
        switch (groupname) {
            case 'Users': this.Useractions(event);
                break;
            case 'Groups': this.Groupactions(event);
                break;
            case 'Roles': this.Roleactions(event);
                break;
            case 'Transport': this.Transportactions(event);
                break;
            case 'Reports': this.Reportactions(event);
                break;
            case 'Workflow': this.Workflowactions(event);
                break;
            case 'Accounts': this.Accountactions(event);
                break;
            case 'Mappings': this.Mappingactions(event);
                break;
            case 'Calenders': this.Calenderactions(event);
                break;
        }
    }
    //if ($(event.target).hasClass('MigrationGroups')) {
    //    $(event.target)
    //}
    if ($(event.target).hasClass("usereachname")) {
        //$(event.target).closest('.usereach').find('.usercheck').toggleClass("fa fa-check-circle");
        $(event.target).closest('.usereach').toggleClass("selected");
    }
    if ($(event.target).hasClass('uploadeachname')) {
        $(event.target).closest('.uploadeach').toggleClass("selected");
    }
    if ($(event.target).hasClass("MgrActionText")) {
        if ($(event.target).text() == "Select All") {
            var options = $('.usereach');
            for (var i = 0; i < options.length; i++) {
                if (!$(options[i]).hasClass('selected') && !$(options[i]).hasClass('hidden')) {
                    //$(options[i]).find('.usercheck').addClass("fa fa-check-circle");
                    $(options[i]).addClass('selected')
                }
            }
        }
        if ($(event.target).text() == "Clear All") {
            var options = $('.usereach');
            for (var i = 0; i < options.length; i++) {
                if ($(options[i]).hasClass('selected')) {
                    //$(options[i]).find('.usercheck').removeClass("fa fa-check-circle");
                    $(options[i]).removeClass('selected')
                }
            }
        }
    }
    if ($(event.target).hasClass('MgrSelectAll') && !$(event.target).hasClass('MgrSelected')) {
        $(event.target).addClass('MgrSelected');
        $(event.target).closest('.MgrSelectToggleParent').find('.MgrSelectSelected').removeClass('MgrSelected');
        var usernames = $(".usereach");
        for (var i = 0; i < usernames.length; i++) {
            if ($(usernames[i]).hasClass('hidden'))
                $(usernames[i]).removeClass('hidden');
        }
        var usergroups = $('.usergroup');
        for (var i = 0; i < usergroups.length; i++) {
            $(usergroups[i]).show();
        }
    }
    if ($(event.target).hasClass('MgrSelectSelected') && !$(event.target).hasClass('MgrSelected')) {
        $(event.target).addClass('MgrSelected');
        $(event.target).closest('.MgrSelectToggleParent').find('.MgrSelectAll').removeClass('MgrSelected');
        var usernames = $(".usereach");
        for (var i = 0; i < usernames.length; i++) {
            if (!$(usernames[i]).hasClass('selected'))
                $(usernames[i]).addClass('hidden');
        }
        var usergroups = $('.usergroup');
        for (var i = 0; i < usergroups.length; i++) {
            if ($(usergroups[i]).find(".usereach").length == $(usergroups[i]).find(".usereach.hidden").length)
                $(usergroups[i]).hide();
            else
                $(usergroups[i]).show();
        }
    }
    if ($(event.target).attr("id") == 'ConfirmDownload' || $(event.target).attr("id") == 'ConfirmUpload') {
        var mgrgroups = $('.MigrationGroups');
        for (var i = 0; i < mgrgroups.length; i++) {
            if ($(mgrgroups[i]).hasClass('selected'))
                var groupname = $(mgrgroups[i]).find('.MigrationGroupName').text();
        }
        switch (groupname) {
            case 'Users': this.Useractions(event);
                break;
            case 'Groups': this.Groupactions(event);
                break;
            case 'Roles': this.Roleactions(event);
                break;
            case 'Transport': this.Transportactions(event);
                break;
            case 'Reports': this.Reportactions(event);
                break;
            case 'Workflow': this.Workflowactions(event);
                break;
            case 'Accounts': this.Accountactions(event);
                break;
            case 'Mappings': this.Mappingactions(event);
                break;
            case 'Calenders': this.Calenderactions(event);
                break;
        }
    }
    if ($(event.target).attr("id") == 'GroupsScrollRight') {
        var x = $('#MigrationGroupsParent').scrollLeft();
        $('#MigrationGroupsParent').scrollLeft(x + 50);
    }
    if ($(event.target).attr("id") == 'GroupsScrollLeft') {
        var x = $('#MigrationGroupsParent').scrollLeft();
        $('#MigrationGroupsParent').scrollLeft(x - 50);
    }
};

Migration.prototype.keyupHandler = function (e) {
    if ($(e.target).attr('id') == 'GroupActionDownloadSearch') {
        var usernames = $(".usereach");
        for (var i = 0; i < usernames.length; i++) {
            if ($(usernames[i]).text().toLowerCase().indexOf($(e.target).val().toLowerCase()) != -1)
                $(usernames[i]).removeClass('hidden');
            else
                $(usernames[i]).addClass('hidden');
        }
        var usergroups = $('.usergroup');
        for (var i = 0; i < usergroups.length; i++) {
            if ($(usergroups[i]).find(".usereach").length == $(usergroups[i]).find(".usereach.hidden").length)
                $(usergroups[i]).hide();
            else
                $(usergroups[i]).show();
        }
    }
};


Migration.prototype.Transportactions = function (e) {
    var self = this;
    if ($(e.target).hasClass('Uploadaction')) {
        this.ShowUploadOptions();
    }
    else if ($(e.target).hasClass('Downloadaction')) {
        var url = RADMigration.baseUrl + "/Resources/Services/RADTransport.svc";
        $.ajax({
            url: url + '/GetAllTransportNames',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json'
        }).then(function (responseText) {
            var transports = JSON.parse(responseText.d);
            self.ShowDownloadOptions(transports);
        });
    }
    else if ($(event.target).attr("id") == 'ConfirmUpload') {
        var url = RADMigration.baseUrl + "/Resources/Services/RADTransport.svc";
        $.ajax({
            url: url + '/GetTransportDetailsFromFile',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ cacheKey: "parent1" })
        }).then(function (responseText) {
            if (responseText.d != null) {
                Migration.instance.transports = JSON.parse(responseText.d);
                var newarr = [];
                var updatedarr = [];
                for (o of Migration.instance.transports) {
                    if (!o.IsExistingEntity)
                        newarr.push(o.EntityName);
                    else
                        updatedarr.push(o.EntityName);
                }
                self.ShowConfirmUploadScreen(newarr, updatedarr);
            }
        });
    }
    else if ($(event.target).attr("id") == 'ConfirmDownload') {
        var selectedwrkflws = [];
        var wrkflws = $('.usereach.selected');
        //for (var i = 0; i < wrkflws.length; i++) {
        //    selectedwrkflws.push($(wrkflws[i]).find('.usereachname').text());
        //}
        for (var i = 0; i < wrkflws.length; i++) {
            selectedwrkflws.push($(wrkflws[i]).find('.usereachname').text());
        }
        if (selectedwrkflws.length != 0) {
            var url = RADMigration.baseUrl + "/Resources/Services/RADTransport.svc";
            $.ajax({
                url: url + '/DownloadTransportInfo',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ transportNames: JSON.stringify(selectedwrkflws) })
            }).then(function (responseText) {
                window.open("App_Dynamic_Resource/RUserManagement,com.ivp.rad.RUserManagement.RScripts.RADGridExportToExcel.aspx?eventData=" + "TransportDetails.xlsx", "_blank", "width=400,height=1,menubar=0,resizable=1");
            });
        }
    }
}

Migration.prototype.Useractions = function (e) {
    var self = this;
    if ($(e.target).hasClass('Uploadaction')) {
        this.ShowUploadOptions();
    }
    else if ($(e.target).hasClass('Downloadaction')) {
        var url = RADMigration.baseUrl + "/Resources/Services/RADUserManagement.svc";
        $.ajax({
            url: url + '/GetAllUsers',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json'
        }).then(function (responseText) {
            var users = JSON.parse(responseText.d);
            self.ShowDownloadOptions(users, 'UserLoginName', 'UserName');
        });
    }
    else if ($(event.target).attr("id") == 'ConfirmUpload') {
        var url = RADMigration.baseUrl + "/Resources/Services/RADUserManagement.svc";
        $.ajax({
            url: url + '/GetUserDetailsFromFile',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ cacheKey: "parent1" })
        }).then(function (responseText) {
            if (responseText.d != null) {
                Migration.instance.users = JSON.parse(responseText.d);
                var newarr = [];
                var updatedarr = [];
                for (o of Migration.instance.users) {
                    if (!o.IsExistingEntity)
                        newarr.push(o.EntityName);
                    else
                        updatedarr.push(o.EntityName);
                }
                self.ShowConfirmUploadScreen(newarr, updatedarr);
            }
        });
    }
    else if ($(event.target).attr("id") == 'ConfirmDownload') {
        var selectedwrkflws = [];
        var wrkflws = $('.usereach.selected');
        //for (var i = 0; i < wrkflws.length; i++) {
        //    selectedwrkflws.push($(wrkflws[i]).find('.usereachname').text());
        //}
        for (var i = 0; i < wrkflws.length; i++) {
            selectedwrkflws.push($(wrkflws[i]).find('.usereachname').attr('user'));
        }
        if (selectedwrkflws.length != 0) {
            var url = RADMigration.baseUrl + "/Resources/Services/RADUserManagement.svc";
            $.ajax({
                url: url + '/DownloadUsers',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ userLoginNames: JSON.stringify(selectedwrkflws) })
            }).then(function (responseText) {
                window.open("App_Dynamic_Resource/RUserManagement,com.ivp.rad.RUserManagement.RScripts.RADGridExportToExcel.aspx?eventData=" + "UsersDetails.xlsx", "_blank", "width=400,height=1,menubar=0,resizable=1");
                if (responseText.d != null) {
                    //Migration.instance.alertPopUp(true, 'Users Downloaded Successfully');
                }
                else
                    Migration.instance.alertPopUp(false, 'Failed to Download Users');
            });
        }
    }
};

Migration.prototype.Roleactions = function (e) {
    var self = this;
    if ($(e.target).hasClass('Uploadaction')) {
        this.ShowUploadOptions();
    }
    else if ($(e.target).hasClass('Downloadaction')) {
        var url = RADMigration.baseUrl + "/Resources/Services/RADUserManagement.svc";
        $.ajax({
            url: url + '/GetAllRoles',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json'
        }).then(function (responseText) {
            var roles = JSON.parse(responseText.d);
            self.ShowDownloadOptions(roles, 'RoleName');
        });
    }
    else if ($(event.target).attr("id") == 'ConfirmUpload') {
        var url = RADMigration.baseUrl + "/Resources/Services/RADUserManagement.svc";
        $.ajax({
            url: url + '/GetRoleDetailsFromFile',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ cacheKey: "parent1" })
        }).then(function (responseText) {
            if (responseText.d != null) {
                Migration.instance.roles = JSON.parse(responseText.d);
                var newarr = [];
                var updatedarr = [];
                for (o of Migration.instance.roles) {
                    if (!o.IsExistingEntity)
                        newarr.push(o.EntityName);
                    else
                        updatedarr.push(o.EntityName);
                }
                self.ShowConfirmUploadScreen(newarr, updatedarr);
            }
        });
    }
    else if ($(event.target).attr("id") == 'ConfirmDownload') {
        var selectedwrkflws = [];
        var wrkflws = $('.usereach.selected');
        //for (var i = 0; i < wrkflws.length; i++) {
        //    selectedwrkflws.push($(wrkflws[i]).find('.usereachname').text());
        //}
        for (var i = 0; i < wrkflws.length; i++) {
            selectedwrkflws.push($(wrkflws[i]).find('.usereachname').text());
        }
        if (selectedwrkflws.length != 0) {
            var url = RADMigration.baseUrl + "/Resources/Services/RADUserManagement.svc";
            $.ajax({
                url: url + '/DownloadRolesInfo',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ roleNames: JSON.stringify(selectedwrkflws) })
            }).then(function (responseText) {
                window.open("App_Dynamic_Resource/RUserManagement,com.ivp.rad.RUserManagement.RScripts.RADGridExportToExcel.aspx?eventData=" + "RoleDetails.xlsx", "_blank", "width=400,height=1,menubar=0,resizable=1");
                if (responseText.d != null) {
                    //Migration.instance.alertPopUp(true, 'Roles Downloaded Successfully');
                }
                else
                    Migration.instance.alertPopUp(false, 'Failed to Download Roles');
            });
        }
    }
};

Migration.prototype.Groupactions = function (e) {
    var self = this;
    if ($(e.target).hasClass('Uploadaction')) {
        this.ShowUploadOptions();
    }
    else if ($(e.target).hasClass('Downloadaction')) {
        var url = RADMigration.baseUrl + "/Resources/Services/RADUserManagement.svc";
        $.ajax({
            url: url + '/GetAllGroups',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json'
        }).then(function (responseText) {
            var groups = JSON.parse(responseText.d);
            self.ShowDownloadOptions(groups, 'GroupName');
        });
    }
    else if ($(event.target).attr("id") == 'ConfirmUpload') {
        var url = RADMigration.baseUrl + "/Resources/Services/RADUserManagement.svc";
        $.ajax({
            url: url + '/GetGroupDetailsFromFile',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ cacheKey: "parent1" })
        }).then(function (responseText) {
            if (responseText.d != null) {
                Migration.instance.groups = JSON.parse(responseText.d);
                var newarr = [];
                var updatedarr = [];
                for (o of Migration.instance.groups) {
                    if (!o.IsExistingEntity)
                        newarr.push(o.EntityName);
                    else
                        updatedarr.push(o.EntityName);
                }
                self.ShowConfirmUploadScreen(newarr, updatedarr);
            }
        });
    }
    else if ($(event.target).attr("id") == 'ConfirmDownload') {
        var selectedwrkflws = [];
        var wrkflws = $('.usereach.selected');
        //for (var i = 0; i < wrkflws.length; i++) {
        //    selectedwrkflws.push($(wrkflws[i]).find('.usereachname').text());
        //}
        for (var i = 0; i < wrkflws.length; i++) {
            selectedwrkflws.push($(wrkflws[i]).find('.usereachname').text());
        }
        if (selectedwrkflws.length != 0) {
            var url = RADMigration.baseUrl + "/Resources/Services/RADUserManagement.svc";
            $.ajax({
                url: url + '/DownloadGroupsInfo',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ groupNames: JSON.stringify(selectedwrkflws) })
            }).then(function (responseText) {
                window.open("App_Dynamic_Resource/RUserManagement,com.ivp.rad.RUserManagement.RScripts.RADGridExportToExcel.aspx?eventData=" + "GroupDetails.xlsx", "_blank", "width=400,height=1,menubar=0,resizable=1");
                if (responseText.d != null) {
                    //Migration.instance.alertPopUp(true, 'Groups Downloaded Successfully');
                }
                else
                    Migration.instance.alertPopUp(false, 'Failed to Download Groups');
            });
        }
    }
};

Migration.prototype.Reportactions = function (e) {
    var self = this;
    if ($(e.target).hasClass('Uploadaction')) {
        this.ShowUploadOptions();
    }
    else if ($(e.target).hasClass('Downloadaction')) {
        var url = RADMigration.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
        $.ajax({
            url: url + '/GetAllReportsToDownLoad',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ moduleId: 12 })
        }).then(function (responseText) {
            var reports = JSON.parse(responseText.d);
            self.ShowDownloadOptions(reports, 'ReportName');
        });
    }
    else if ($(event.target).attr("id") == 'ConfirmDownload') {
        var selectedwrkflws = "";
        var wrkflws = $('.usereach.selected');
        //for (var i = 0; i < wrkflws.length; i++) {
        //    selectedwrkflws.push($(wrkflws[i]).find('.usereachname').text());
        //}
        for (var i = 0; i < wrkflws.length; i++) {
            if (i != wrkflws.length - 1)
                selectedwrkflws += ($(wrkflws[i]).find('.usereachname').text() + ",");
            else
                selectedwrkflws += $(wrkflws[i]).find('.usereachname').text();
        }
        if (selectedwrkflws.length != 0) {
            var url = RADMigration.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
            $.ajax({
                url: url + '/DownloadReportExcel',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ reportNames: selectedwrkflws })
            }).then(function (responseText) {
                window.open("App_Dynamic_Resource/RGridWriterToolKit,com.ivo.rad.RGridWriterToolKit.Resources.RADGridExportToExcel.aspx?eventData=" + "ReportExcel.xlsx", "_blank", "width=400,height=1,menubar=0,resizable=1");
                if (responseText.d != null) {
                    //Migration.instance.alertPopUp(true, 'Reports Downloaded Successfully');
                }
                else
                    Migration.instance.alertPopUp(false, 'Failed to Download Reports');
            });
        }
    }
    else if ($(event.target).attr("id") == 'ConfirmUpload') {
        var url = RADMigration.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
        $.ajax({
            url: url + '/UploadReportDiff',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ cacheKey: "parent1", moduleid: 12 })
        }).then(function (responseText) {
            if (responseText.d != null) {
                Migration.instance.reports = JSON.parse(responseText.d);
                var newarr = [];
                var updatedarr = [];
                for (o of Migration.instance.reports) {
                    if (!o.IsExistingEntity)
                        newarr.push(o.EntityName);
                    else
                        updatedarr.push(o.EntityName);
                }
                self.ShowConfirmUploadScreen(newarr, updatedarr);
            }
        });
    }
};

Migration.prototype.Workflowactions = function (e) {
    var self = this;
    if ($(e.target).hasClass('Uploadaction')) {
        this.ShowUploadOptions();
    }
    else if ($(e.target).hasClass('Downloadaction')) {
        var url = RADMigration.baseUrl + "/Resources/Services/RADWorkFlow.svc";
        $.ajax({
            url: url + '/GetAllWorkFLows',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json'
        }).then(function (responseText) {
            var workflows = responseText.d;
            self.ShowDownloadOptions(workflows, 'WorkflowName');
        });
    }
    else if ($(event.target).attr("id") == 'ConfirmDownload') {
        var selectedwrkflws = "";
        var wrkflws = $('.usereach.selected');
        //for (var i = 0; i < wrkflws.length; i++) {
        //    selectedwrkflws.push($(wrkflws[i]).find('.usereachname').text());
        //}
        for (var i = 0; i < wrkflws.length; i++) {
            if (i != wrkflws.length - 1)
                selectedwrkflws += ($(wrkflws[i]).find('.usereachname').text() + ",");
            else
                selectedwrkflws += $(wrkflws[i]).find('.usereachname').text();
        }
        if (selectedwrkflws.length != 0) {
            var url = RADMigration.baseUrl + "/Resources/Services/RADWorkFlow.svc";
            $.ajax({
                url: url + '/DownLoadWorkflowExcel',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ workflowNames: selectedwrkflws })
            }).then(function (responseText) {
                window.open("App_Dynamic_Resource/RGridWriterToolKit,com.ivo.rad.RGridWriterToolKit.Resources.RADGridExportToExcel.aspx?eventData=" + "ReportExcel.xlsx", "_blank", "width=400,height=1,menubar=0,resizable=1");
                if (responseText.d != null) {
                    //Migration.instance.alertPopUp(true, 'Workflows Downloaded Successfully');
                }
                else
                    Migration.instance.alertPopUp(false, 'Failed to Download Workflows');
            });
        }
    }
    else if ($(event.target).attr("id") == 'ConfirmUpload') {
        var url = RADMigration.baseUrl + "/Resources/Services/RADWorkFlow.svc";
        $.ajax({
            url: url + '/UploadWorkflowDiff',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ cacheKey: "parent1" })
        }).then(function (responseText) {
            if (responseText.d != null) {
                Migration.instance.workflows = JSON.parse(responseText.d);
                var newarr = [];
                var updatedarr = [];
                for (o of Migration.instance.workflows) {
                    if (!o.IsExistingEntity)
                        newarr.push(o.EntityName);
                    else
                        updatedarr.push(o.EntityName);
                }
                self.ShowConfirmUploadScreen(newarr, updatedarr);
            }
        });
    }
};

Migration.prototype.Accountactions = function (e) {
    var self = this;
    if ($(e.target).hasClass('Uploadaction')) {
        this.ShowUploadOptions();
    }
    else if ($(e.target).hasClass('Downloadaction')) {
        var url = RADMigration.baseUrl + "/Resources/Services/RADUserManagement.svc";
        $.ajax({
            url: url + '/GetAllAccounts',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json'
        }).then(function (responseText) {
            var accounts = JSON.parse(responseText.d);
            self.ShowDownloadOptions(accounts, 'AccountName', 'UserName');
        });
    }
    else if ($(event.target).attr("id") == 'ConfirmUpload') {
        var url = RADMigration.baseUrl + "/Resources/Services/RADUserManagement.svc";
        $.ajax({
            url: url + '/GetAccountDetailsFromFile',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ cacheKey: "parent1" })
        }).then(function (responseText) {
            if (responseText.d != null) {
                Migration.instance.accounts = JSON.parse(responseText.d);
                var newarr = [];
                var updatedarr = [];
                for (o of Migration.instance.accounts) {
                    if (!o.IsExistingEntity)
                        newarr.push(o.EntityName);
                    else
                        updatedarr.push(o.EntityName);
                }
                self.ShowConfirmUploadScreen(newarr, updatedarr);
            }
        });
    }
    else if ($(event.target).attr("id") == 'ConfirmDownload') {
        var selectedwrkflws = [];
        var wrkflws = $('.usereach.selected');
        for (var i = 0; i < wrkflws.length; i++) {
            selectedwrkflws.push($(wrkflws[i]).find('.usereachname').text());
        }
        //for (var i = 0; i < wrkflws.length; i++) {
        //    selectedwrkflws.push($(wrkflws[i]).find('.usereachname').attr('user'));
        //}
        if (selectedwrkflws.length != 0) {
            var url = RADMigration.baseUrl + "/Resources/Services/RADUserManagement.svc";
            $.ajax({
                url: url + '/DownloadAccountsInfo',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ accountNames: JSON.stringify(selectedwrkflws) })
            }).then(function (responseText) {
                window.open("App_Dynamic_Resource/RUserManagement,com.ivp.rad.RUserManagement.RScripts.RADGridExportToExcel.aspx?eventData=" + "AccountDetails.xlsx", "_blank", "width=400,height=1,menubar=0,resizable=1");
                if (responseText.d != null) {
                    //Migration.instance.alertPopUp(true, 'Users Downloaded Successfully');
                }
                else
                    Migration.instance.alertPopUp(false, 'Failed to Download Accounts');
            });
        }
    }
};

Migration.prototype.Mappingactions = function (e) {
    var self = this;
    if ($(e.target).hasClass('Uploadaction')) {
        this.ShowUploadOptions();
    }
    else if ($(e.target).hasClass('Downloadaction')) {
        var url = RADMigration.baseUrl + "/Resources/Services/RADMappingService.svc";
        $.ajax({
            url: url + '/GetMappingSummary',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ showAllAccounts: true })
        }).then(function (responseText) {
            var mappings = JSON.parse(responseText.d);
            self.ShowDownloadOptions(mappings, 'MappingName');
        });
    }
    else if ($(event.target).attr("id") == 'ConfirmUpload') {
        var url = RADMigration.baseUrl + "/Resources/Services/RADMappingService.svc";
        $.ajax({
            url: url + '/GetMappingDetailsFromFile',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ cacheKey: "parent1" })
        }).then(function (responseText) {
            if (responseText.d != null) {
                Migration.instance.mappings = JSON.parse(responseText.d);
                var newarr = [];
                var updatedarr = [];
                for (o of Migration.instance.mappings) {
                    if (!o.IsExistingEntity)
                        newarr.push(o.EntityName);
                    else
                        updatedarr.push(o.EntityName);
                }
                self.ShowConfirmUploadScreen(newarr, updatedarr);
            }
        });
    }
    else if ($(event.target).attr("id") == 'ConfirmDownload') {
        var selectedwrkflws = [];
        var wrkflws = $('.usereach.selected');
        for (var i = 0; i < wrkflws.length; i++) {
            selectedwrkflws.push($(wrkflws[i]).find('.usereachname').text());
        }
        //for (var i = 0; i < wrkflws.length; i++) {
        //    selectedwrkflws.push($(wrkflws[i]).find('.usereachname').attr('user'));
        //}
        if (selectedwrkflws.length != 0) {
            var url = RADMigration.baseUrl + "/Resources/Services/RADMappingService.svc";
            $.ajax({
                url: url + '/DownloadMappings',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ mappingNames: JSON.stringify(selectedwrkflws) })
            }).then(function (responseText) {
                window.open("App_Dynamic_Resource/RUserManagement,com.ivp.rad.RUserManagement.RScripts.RADGridExportToExcel.aspx?eventData=" + "MappingDetails.xlsx", "_blank", "width=400,height=1,menubar=0,resizable=1");
                if (responseText.d != null) {
                    //Migration.instance.alertPopUp(true, 'Users Downloaded Successfully');
                }
                else
                    Migration.instance.alertPopUp(false, 'Failed to Download Mappings');
            });
        }
    }
};

Migration.prototype.Calenderactions = function (e) {
    var self = this;
    if ($(e.target).hasClass('Uploadaction')) {
        this.ShowUploadOptions();
    }
    else if ($(e.target).hasClass('Downloadaction')) {
        var url = RADMigration.baseUrl + "/Resources/Services/RADBusinessCalendar.svc";
        $.ajax({
            url: url + '/GetAllCalendars',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json'
        }).then(function (responseText) {
            var calenders = JSON.parse(responseText.d);
            self.ShowDownloadOptions(calenders, 'CalendarName');
        });
    }
    else if ($(event.target).attr("id") == 'ConfirmUpload') {
        var url = RADMigration.baseUrl + "/Resources/Services/RADBusinessCalendar.svc";
        $.ajax({
            url: url + '/GetCalendarDetailsFromFile',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ cacheKey: "parent1" })
        }).then(function (responseText) {
            if (responseText.d != null) {
                Migration.instance.calenders = JSON.parse(responseText.d);
                var newarr = [];
                var updatedarr = [];
                for (o of Migration.instance.calenders) {
                    if (!o.IsExistingEntity)
                        newarr.push(o.EntityName);
                    else
                        updatedarr.push(o.EntityName);
                }
                self.ShowConfirmUploadScreen(newarr, updatedarr);
            }
        });
    }
    else if ($(event.target).attr("id") == 'ConfirmDownload') {
        var selectedwrkflws = [];
        var wrkflws = $('.usereach.selected');
        for (var i = 0; i < wrkflws.length; i++) {
            selectedwrkflws.push($(wrkflws[i]).find('.usereachname').text());
        }
        //for (var i = 0; i < wrkflws.length; i++) {
        //    selectedwrkflws.push($(wrkflws[i]).find('.usereachname').attr('user'));
        //}
        if (selectedwrkflws.length != 0) {
            var url = RADMigration.baseUrl + "/Resources/Services/RADBusinessCalendar.svc";
            $.ajax({
                url: url + '/DownloadCalendarDetails',
                type: 'POST',
                contentType: "application/json",
                dataType: 'json',
                data: JSON.stringify({ Calendarnames: JSON.stringify(selectedwrkflws) })
            }).then(function (responseText) {
                window.open("App_Dynamic_Resource/RUserManagement,com.ivp.rad.RUserManagement.RScripts.RADGridExportToExcel.aspx?eventData=" + "CalendarDetails.xlsx", "_blank", "width=400,height=1,menubar=0,resizable=1");
                if (responseText.d != null) {
                    //Migration.instance.alertPopUp(true, 'Users Downloaded Successfully');
                }
                else
                    Migration.instance.alertPopUp(false, 'Failed to Download Calenders');
            });
        }
    }
};

Migration.prototype.ShowDownloadOptions = function (arr, property, attr) {
    $("#downloadwindow").empty();
    $(".groupactiontitle").text("Download");
    if (property != null)
        arr = _.sortBy(arr, function (obj) { return obj[property].toLowerCase(); });
    else
        arr = _.sortBy(arr, function (obj) { return obj.toLowerCase(); });
    var last = '';
    var currgroup;
    var currentIteration = "";
    for (user of arr) {
        if (property != null)
            currentIteration = user[property][0].toLowerCase();
        else
            currentIteration = user[0].toLowerCase();
        if (currentIteration != last.toLowerCase()) {
            last = currentIteration;
            var group = $('<div>');
            group.addClass("usergroup");
            var letter = $('<div>');
            letter.addClass("usergroupsymbol");
            letter.text(last.toUpperCase());

            group.append(letter);
            currgroup = group;
        }
        var each = $('<div>');
        each.addClass("usereach");
        var name = $('<div>');
        name.addClass("usereachname");
        var check = $('<div>');
        check.addClass('usercheck');

        if (property != null) {
            name.text(user[property]);
            name.attr({ "title": user[property] });
        }
        else {
            name.text(user);
            name.attr({ "title": user });
        }
        if (attr != null)
            name.attr('user', user[attr]);
        each.append(check);
        each.append(name);
        currgroup.append(each);
        $("#downloadwindow").append(currgroup);
    }
    $('.MgrSelectAll').addClass('MgrSelected');
    $('.MgrSelectSelected').removeClass('MgrSelected');
    $('.MgrActionText, .MgrSelectAll, .MgrSelectSelected, .MgrActionButton').unbind('click').click(function (e) {
        Migration.instance.clickHandler(e);
    });
    $('.usereach').unbind('click').click(function (e) {
        Migration.instance.clickHandler(e);
    });
    $("#GroupActionDownloadSearch").keyup(function (e) {
        Migration.instance.keyupHandler(e);
    });
};

Migration.prototype.ShowUploadOptions = function () {
    $("#UploadWindow").empty();
    $(".groupactiontitle").text("Upload");
    $("#UploadWindow").append($('<div>', {
        class: 'radWorkflowDragNDropRegion'
    }))
    $(".radWorkflowDragNDropRegion").css({ 'display': 'inline-block' });
    $(".radWorkflowDragNDropRegion").append("<div style=\"text-indent: 8px;\"> </div>");
    //$(".radWorkflowDragNDropRegion").append("<div class=\"fa fa-upload radWFUploadButton\"></div>");

    $(".radWorkflowDragNDropRegion").append("<div class=\"radWFDropDiv radWFDropDivImg\" id=\"attachmentButton\"><div class='radWFUploadButton' style=\"display:inline-block;\">DRAG AND DROP&nbsp&nbsp&nbsp&nbsp</div><div style=\"display:inline-block;\">OR&nbsp&nbsp</div><div id=\"MgrUploadWindowBrowseText\" style=\"display:inline-block;text-decoration: underline;\">Browse</div>");
    $("#UploadWindow").append("<div class=\"radWorkflowParentDropDrag\" id=\"parent1\"></div>");
    if ($('#parent1').fileUpload != undefined) {
        $('#parent1').fileUpload({
            'parentControlId': 'parent1',
            'attachementControlId': 'attachmentButton',
            'multiple': false,
            'debuggerDiv': '',
            'returnEvent':function(){
                $(".radWFDropDiv").removeClass("radWFDropDivImg");
            },
            'deleteEvent': function () {
                $(".radWFDropDiv").addClass("radWFDropDivImg");
            }
        });
    }
    $('#ConfirmUpload').unbind('click').click(function (e) {
        Migration.instance.clickHandler(e);
    });
    $('#ConfirmUpload').text("Done");
};

Migration.prototype.ShowConfirmUploadScreen = function (newarr, updatedarr) {
    $("#UploadWindow").empty();
    var main = $('<div>');
    main.addClass("UploadWindowContent");
    $("#UploadWindow").append(main);
    var newentrydiv = $('<div>');
    newentrydiv.addClass("MgrUploadsection");
    var title = $('<div>');
    title.addClass('MgrUploadsectionTitle');
    title.text("New Entry");
    newentrydiv.append(title);
    var updatedentrydiv = $('<div>');
    var title2 = $('<div>');
    title2.addClass('MgrUploadsectionTitle');
    title2.text("Updated Entry");
    updatedentrydiv.append(title2);
    updatedentrydiv.addClass("MgrUploadsection");
    updatedentrydiv.attr({ "id": "UploadUpdatedSection" });
    var sameentrydiv = $('<div>');
    sameentrydiv.addClass("MgrUploadsection");
    var child1 = $('<div>');
    var child2 = $('<div>');
    child1.addClass('EntrydivChild');
    child2.addClass('EntrydivChild');
    newentrydiv.append(child1);
    updatedentrydiv.append(child2);
    //$("#UploadWindow").append(newentrydiv);
    //$("#UploadWindow").append(updatedentrydiv);
    //$("#UploadWindow").append(sameentrydiv);

    for(item of newarr) {
        var each = $('<div>');
        each.addClass("uploadeach selected");
        var name = $('<div>');
        name.addClass("uploadeachname");
        name.text(item);
        name.attr({ "title": item });
        each.append(name);
        child1.append(each);
    }
    main.append(newentrydiv);
    for(item of updatedarr) {
        var each = $('<div>');
        each.addClass("uploadeach selected");
        var name = $('<div>');
        name.addClass("uploadeachname");
        name.text(item);
        name.attr({ "title": item });
        each.append(name);
        child2.append(each);
    }

    main.append(updatedentrydiv);
    $('.uploadeachname').click(function (e) {
        Migration.instance.clickHandler(e);
    });
    $('#ConfirmUpload').unbind().click(function (e) {
        Migration.instance.ConfirmSelectedUpload(e);
    });
    $('#ConfirmUpload').text("Sync");
    //var button = $('<div>');
    //button.addClass('MgrActionButton');
    //button.text('Diff');
    //$('.MgrButtonBar').append(button);
};

Migration.prototype.ConfirmSelectedUpload = function (e) {
    var mgrgroups = $('.MigrationGroups');
    for (var i = 0; i < mgrgroups.length; i++) {
        if ($(mgrgroups[i]).hasClass('selected'))
            var groupname = $(mgrgroups[i]).find('.MigrationGroupName').text();
    }
    switch (groupname) {
        case 'Users':
            var selectedusers = [];
            var users = $('.uploadeach.selected');
            for (var i = 0; i < users.length; i++) {
                selectedusers.push(Migration.instance.users.filter(function (user) { return (user.EntityName == $(users[i]).find('.uploadeachname').text()) })[0]);
            }
            Migration.instance.users = selectedusers;
            if (selectedusers.length != 0) {
                var url = RADMigration.baseUrl + "/Resources/Services/RADUserManagement.svc";
                $.ajax({
                    url: url + '/SyncUsers',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ cacheKey: "parent1", usersSyncInfo: JSON.stringify(selectedusers) })
                }).then(function (responseText) {
                    if (responseText.d != null) {
                        //Migration.instance.alertPopUp(true, 'Workflows Uploaded Successfully');
                        if (responseText.d != "")
                            Migration.instance.ShowPostUploadScreen(Migration.instance.users, JSON.parse(responseText.d));
                        else
                            Migration.instance.ShowPostUploadScreen(Migration.instance.users, []);
                    }
                    else
                        Migration.instance.alertPopUp(false, 'Failed to Upload Workflows');
                });
            }
            break;
        case 'Groups':
            var selectedgroups = [];
            var groups = $('.uploadeach.selected');
            for (var i = 0; i < groups.length; i++) {
                selectedgroups.push(Migration.instance.groups.filter(function (group) { return (group.EntityName == $(groups[i]).find('.uploadeachname').text()) })[0]);
            }
            Migration.instance.groups = selectedgroups;
            if (selectedgroups.length != 0) {
                var url = RADMigration.baseUrl + "/Resources/Services/RADUserManagement.svc";
                $.ajax({
                    url: url + '/SyncGroupsNew',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ cacheKey: "parent1", groupsSyncInfo: JSON.stringify(selectedgroups) })
                }).then(function (responseText) {
                    if (responseText.d != null) {
                        //Migration.instance.alertPopUp(true, 'Workflows Uploaded Successfully');
                        if (responseText.d != "")
                            Migration.instance.ShowPostUploadScreen(Migration.instance.groups, JSON.parse(responseText.d));
                        else
                            Migration.instance.ShowPostUploadScreen(Migration.instance.groups, []);
                    }
                    else
                        Migration.instance.alertPopUp(false, 'Failed to Upload Workflows');
                });
            }
            break;
        case 'Roles':
            var selectedroles = [];
            var roles = $('.uploadeach.selected');
            for (var i = 0; i < roles.length; i++) {
                selectedroles.push(Migration.instance.roles.filter(function (role) { return (role.EntityName == $(roles[i]).find('.uploadeachname').text()) })[0]);
            }
            Migration.instance.roles = selectedroles;
            if (selectedroles.length != 0) {
                var url = RADMigration.baseUrl + "/Resources/Services/RADUserManagement.svc";
                $.ajax({
                    url: url + '/SyncRolesNew',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ cacheKey: "parent1", rolesSyncInfo: JSON.stringify(selectedroles) })
                }).then(function (responseText) {
                    if (responseText.d != null) {
                        //Migration.instance.alertPopUp(true, 'Workflows Uploaded Successfully');
                        if (responseText.d != "")
                            Migration.instance.ShowPostUploadScreen(Migration.instance.roles, JSON.parse(responseText.d));
                        else
                            Migration.instance.ShowPostUploadScreen(Migration.instance.roles, []);
                    }
                    else
                        Migration.instance.alertPopUp(false, 'Failed to Upload Workflows');
                });
            }
            break;
        case 'Accounts':
            var selectedaccounts = [];
            var accounts = $('.uploadeach.selected');
            for (var i = 0; i < accounts.length; i++) {
                selectedaccounts.push(Migration.instance.accounts.filter(function (account) { return (account.EntityName == $(accounts[i]).find('.uploadeachname').text()) })[0]);
            }
            Migration.instance.accounts = selectedaccounts;
            if (selectedaccounts.length != 0) {
                var url = RADMigration.baseUrl + "/Resources/Services/RADUserManagement.svc";
                $.ajax({
                    url: url + '/SyncAccounts',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ cacheKey: "parent1", accountsSyncInfo: JSON.stringify(selectedaccounts) })
                }).then(function (responseText) {
                    if (responseText.d != null) {
                        //Migration.instance.alertPopUp(true, 'Workflows Uploaded Successfully');
                        if (responseText.d != "")
                            Migration.instance.ShowPostUploadScreen(Migration.instance.accounts, JSON.parse(responseText.d));
                        else
                            Migration.instance.ShowPostUploadScreen(Migration.instance.accounts, []);
                    }
                    else
                        Migration.instance.alertPopUp(false, 'Failed to Upload Workflows');
                });
            }
            break;
        case 'Transport':
            var selectedtransports = [];
            var transports = $('.uploadeach.selected');
            for (var i = 0; i < transports.length; i++) {
                selectedtransports.push(Migration.instance.transports.filter(function (transport) { return (transport.EntityName == $(transports[i]).find('.uploadeachname').text()) })[0]);
            }
            Migration.instance.transports = selectedtransports;
            if (selectedtransports.length != 0) {
                var url = RADMigration.baseUrl + "/Resources/Services/RADTransport.svc";
                $.ajax({
                    url: url + '/SyncTransports',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ cacheKey: "parent1", transportsSyncInfo: JSON.stringify(selectedtransports) })
                }).then(function (responseText) {
                    if (responseText.d != null) {
                        //Migration.instance.alertPopUp(true, 'Workflows Uploaded Successfully');
                        if (responseText.d != "")
                            Migration.instance.ShowPostUploadScreen(Migration.instance.transports, JSON.parse(responseText.d));
                        else
                            Migration.instance.ShowPostUploadScreen(Migration.instance.transports, []);
                    }
                    else
                        Migration.instance.alertPopUp(false, 'Failed to Upload Workflows');
                });
            }
            break;
        case 'Mappings':
            var selectedmappings = [];
            var mappings = $('.uploadeach.selected');
            for (var i = 0; i < mappings.length; i++) {
                selectedmappings.push(Migration.instance.mappings.filter(function (mapping) { return (mapping.EntityName == $(mappings[i]).find('.uploadeachname').text()) })[0]);
            }
            Migration.instance.mappings = selectedmappings;
            if (selectedmappings.length != 0) {
                var url = RADMigration.baseUrl + "/Resources/Services/RADMappingService.svc";
                $.ajax({
                    url: url + '/SyncMappings',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ cacheKey: "parent1", mappingsSyncInfo: JSON.stringify(selectedmappings) })
                }).then(function (responseText) {
                    if (responseText.d != null) {
                        //Migration.instance.alertPopUp(true, 'Workflows Uploaded Successfully');
                        if (responseText.d != "")
                            Migration.instance.ShowPostUploadScreen(Migration.instance.mappings, JSON.parse(responseText.d));
                        else
                            Migration.instance.ShowPostUploadScreen(Migration.instance.mappings, []);
                    }
                    else
                        Migration.instance.alertPopUp(false, 'Failed to Upload Workflows');
                });
            }
            break;
        case 'Calenders':
            var selectedcalenders = [];
            var calenders = $('.uploadeach.selected');
            for (var i = 0; i < calenders.length; i++) {
                selectedcalenders.push(Migration.instance.calenders.filter(function (calender) { return (calender.EntityName == $(calenders[i]).find('.uploadeachname').text()) })[0]);
            }
            Migration.instance.calenders = selectedcalenders;
            if (selectedcalenders.length != 0) {
                var url = RADMigration.baseUrl + "/Resources/Services/RADBusinessCalendar.svc";
                $.ajax({
                    url: url + '/SyncCalendar',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ cacheKey: "parent1", calendars: JSON.stringify(selectedcalenders) })
                }).then(function (responseText) {
                    if (responseText.d != null) {
                        //Migration.instance.alertPopUp(true, 'Workflows Uploaded Successfully');
                        if (responseText.d != "")
                            Migration.instance.ShowPostUploadScreen(Migration.instance.calenders, JSON.parse(responseText.d));
                        else
                            Migration.instance.ShowPostUploadScreen(Migration.instance.calenders, []);
                    }
                    else
                        Migration.instance.alertPopUp(false, 'Failed to Upload Workflows');
                });
            }
            break;
        case 'Reports': this.UploadSelectedReports(event);
            break;
        case 'Workflow':
            var selectedwrkflws = [];
            var wrkflws = $('.uploadeach.selected');
            for (var i = 0; i < wrkflws.length; i++) {
                selectedwrkflws.push($(wrkflws[i]).find('.uploadeachname').text());
            }
            Migration.instance.workflows = selectedwrkflws;
            if (selectedwrkflws.length != 0) {
                var url = RADMigration.baseUrl + "/Resources/Services/RADWorkFlow.svc";
                $.ajax({
                    url: url + '/UploadWorkflowExcel',
                    type: 'POST',
                    contentType: "application/json",
                    dataType: 'json',
                    data: JSON.stringify({ cacheKey: "parent1", workflowNames: JSON.stringify(selectedwrkflws) })
                }).then(function (responseText) {
                    if (responseText.d != null) {
                        //Migration.instance.alertPopUp(true, 'Workflows Uploaded Successfully');
                        if (responseText.d != "")
                            Migration.instance.ShowPostUploadScreen(Migration.instance.workflows, JSON.parse(responseText.d));
                        else
                            Migration.instance.ShowPostUploadScreen(Migration.instance.workflows, []);
                    }
                    else
                        Migration.instance.alertPopUp(false, 'Failed to Upload Workflows');
                });
            }
            break;
    }
};


Migration.prototype.UploadSelectedReports = function (event) {
    var self = this;
    var selectedrpts = [];
    var reports = $('.uploadeach.selected');
    for (var i = 0; i < reports.length; i++) {
        selectedrpts.push($(reports[i]).find('.uploadeachname').text());
    }
    Migration.instance.reports = selectedrpts;
    if (selectedrpts.length != 0) {
        var url = RADMigration.baseUrl + "/Resources/Services/RADGridWriterToolKit.svc";
        $.ajax({
            url: url + '/UploadReportFromExcel',
            type: 'POST',
            contentType: "application/json",
            dataType: 'json',
            data: JSON.stringify({ cacheKey: "parent1", reportCollection: JSON.stringify(selectedrpts) })
        }).then(function (responseText) {
            //if (responseText.d != null) {
            //    Migration.instance.alertPopUp(true, 'Workflows Uploaded Successfully');
            //}
            //else
            //    Migration.instance.alertPopUp(false, 'Failed to Upload Workflows');
            if (responseText.d != "")
                Migration.instance.ShowPostUploadScreen(Migration.instance.reports, JSON.parse(responseText.d));
            else
                Migration.instance.ShowPostUploadScreen(Migration.instance.reports, []);
        });
    }
}


Migration.prototype.alertPopUp = function (success, msg) {
    var self = this;
    //$("#ContentDiv").append("<div id=\"alertParentDiv1\" class=\"ToolKitAlertPopUpParent\"></div>");
    $("#" + RADMigration.contentBodyId).append("<div id=\"alertParentDiv1\" class=\"ToolKitAlertPopUpParent\"></div>");
    if (success)
        $("#alertParentDiv1").append("<div class=\"ToolKitPopupAlertUpperSuccessDiv\"></div>");
    else
        $("#alertParentDiv1").append("<div class=\"ToolKitPopupAlertUpperErrorDiv\"></div>");
    $("#alertParentDiv1").append("<div class=\"ToolKitPopUpError\"></div>")
    $("#alertParentDiv1").append("<div class=\"ToolKitErrorDivLeft ToolKitUserSuccess\"></div>"); //revisit
    $("#alertParentDiv1").append("<div class=\"ToolKitErrorDivRight\"></div>")
    $("#alertParentDiv1").css("top", "-200px");
    $("#alertParentDiv1").animate({ "top": "0px" });
    setTimeout(function () { $("#alertParentDiv1").remove() }, 1000);
    $(".ToolKitErrorDivRight").append("<div class=\"RADUserManagementSuccessDivHeading\">Result</div>")
    $(".ToolKitErrorDivRight").append("<div class=\"ToolKitErrorDivText\">" + msg + "</div>");
}

Migration.prototype.ShowPostUploadScreen = function (arr , msg) {
    $("#UploadWindow").empty();
    $(".groupactiontitle").text("Upload Results");
    $("#ConfirmUpload").text("Back");
    var main = $('<div>');
    main.addClass('PostUploadMain');
    var success = $('<div>');
    success.addClass('MainChildSuccess');
    var failure = $('<div>');
    failure.addClass('MainChildFailure');
    main.append(success);
    main.append(failure);

    for(obj of arr) {
        if (obj.EntityName) {
            var row = $('<div>');
            var status = $('<div>');
            var name = $('<div>');
            var fmsg = $('<div>');
            var x = msg.filter(function (user) { return (user.EntityName == obj.EntityName) });
            if (x.length == 0) {
                row.addClass('PostUploadRow MgrSuccess');
                status.addClass('PostUploadStatus fas fa-check-circle');
                name.addClass('PostUploadEntityName');
                name.text(obj.EntityName);
                fmsg.addClass('PostUploadMessage');
                fmsg.text("Upload Successful");
                row.append(status);
                row.append(name);
                row.append(fmsg);
                success.append(row);
            }
            else {
                row.addClass('PostUploadRow MgrFailure');
                status.addClass('PostUploadStatus fas fa-times-circle');
                name.addClass('PostUploadEntityName');
                name.text(obj.EntityName);
                fmsg.addClass('PostUploadMessage');
                fmsg.text(x[0].FailureMessages);
                row.append(status);
                row.append(name);
                row.append(fmsg);
                failure.append(row);
            }
        }
        else {
            var row = $('<div>');
            var status = $('<div>');
            var name = $('<div>');
            var fmsg = $('<div>');
            var x = msg.filter(function (user) { return (user.EntityName == obj) });
            if (x.length == 0) {
                row.addClass('PostUploadRow MgrSuccess');
                status.addClass('PostUploadStatus fas fa-check-circle');
                name.addClass('PostUploadEntityName');
                name.text(obj);
                fmsg.addClass('PostUploadMessage');
                fmsg.text("Upload Successful");
                row.append(status);
                row.append(name);
                row.append(fmsg);
                success.append(row);
            }
            else {
                row.addClass('PostUploadRow MgrFailure');
                status.addClass('PostUploadStatus fas fa-times-circle');
                name.addClass('PostUploadEntityName');
                name.text(obj);
                fmsg.addClass('PostUploadMessage');
                fmsg.text(x[0].FailureMessages);
                row.append(status);
                row.append(name);
                row.append(fmsg);
                failure.append(row);
            }
        }
    }
    $("#UploadWindow").append(main);
    $("#ConfirmUpload").unbind('click').click(function () {
        Migration.instance.ShowUploadOptions();
    });
};