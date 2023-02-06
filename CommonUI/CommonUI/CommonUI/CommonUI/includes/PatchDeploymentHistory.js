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

var patchDeploymentHistory = (function () {
    var patchDeploymentHistory;

    function PatchDeploymentHistoryClass() {
    }

    patchDeploymentHistory = new PatchDeploymentHistoryClass();
    var moduleid = 3;
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    var buildRow = '<div class="BuildRow {5}" hotfixRN="{6}" RN="{7}"><div>{0}</div><div>{1}</div><div>{2}</div><div>{3}</div><div>{4}</div><div>Hotfixes</div><div>Release Notes</div></div>';
    var hotfixTemplate = '<div class="hotfixRowsContainer"><div class="hotfixHeaderRow"><div class="hotfixHeader">Hotfix ID</div><div class="hotfixHeader">Case ID</div><div class="hotfixHeader">Publish Date</div><div class="hotfixHeader">Deployment Date</div></div><div class="hotfixRows">{0}</div></div>';
    var hotfixRow = '<div class="hotfixRow"><div>{0}</div><div><a href="https://ivp.fogbugz.com/f/cases/{1}" ' +
        'target="_blank" rel="noopener noreferrer">{1}</a></div><div>{3}</div><div>{4}</div></div>';

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });


    PatchDeploymentHistoryClass.prototype.init = function (obj) {
        var objInfo = eval("(" + obj + ")");
        if (objInfo.moduleId == 0)
            return;
        moduleId = objInfo.moduleId;
        if (moduleId != 3) {
            $(".deploymentHistoryHeaderRow").addClass("refHistory");
            $(".deploymentHistoryRows ").addClass("refHistory");
            buildRow = '<div class="BuildRow {4}" hotfixRN="{5}" RN="{6}"><div>{0}</div><div></div><div>{1}</div><div>{2}</div><div>{3}</div><div>Hotfixes</div><div>Release Notes</div></div>';
        }
        $(document).ready(function () {
            CallCommonServiceMethod("GetHotfixes", { allDeployedVersions: true, moduleId: moduleId }, OnSuccessGetHotfixes, OnFailure)
        });

    }

    function download(data, filename, type) {
        var file = new Blob([data], { type: type });
        if (window.navigator.msSaveOrOpenBlob) // IE10+
            window.navigator.msSaveOrOpenBlob(file, filename);
        else { // Others
            var a = document.createElement("a"),
                url = URL.createObjectURL(file);
            a.href = url;
            a.download = filename;
            document.body.appendChild(a);
            a.click();
            setTimeout(function () {
                document.body.removeChild(a);
                window.URL.revokeObjectURL(url);
            }, 0);
        }
    }
    function CallCommonServiceMethod(methodName, parameters, ajaxSuccess, ajaxError) {
        callService('POST', path + '/BaseUserControls/Service/CommonService.svc', methodName, parameters, ajaxSuccess, ajaxError, null, false, false);
    }

    function OnSuccessGetHotfixes(result) {
        result = result.d;
        if (moduleId == 3)
            OnSucessSecGetHotfixes(result);
        else
            OnSucessRefGetHotfixes(result);
        registerEventListeners();
    }

    function OnSucessSecGetHotfixes(result) {
        var html = "";
        var fileNames = $(window.parent.document).find("#hdnReleaseNotesFileNames").val().split("*");
        result.forEach((build, buildIndex) => {
            var hasHotfixes = build.Hotfixes != null && build.Hotfixes.length > 0;
            var hotfixReleaseNote = fileNames.find((fileName) => fileName == 'IVP SRM Hotfix Release Notes v' + build.Version + ".pdf");
            var releaseNotes = fileNames.find((fileName) => fileName.match(new RegExp("IVP SRM Release Notes [(][0-9].[0-9].[A-Z]?[0-9]*[.]*[A-Z]*[a-z]*[0-9]*[a-z]* to " + build.Version + "[)]")));
            hotfixReleaseNote = hotfixReleaseNote == undefined ? '' : hotfixReleaseNote;
            releaseNotes = releaseNotes == undefined ? '' : releaseNotes;
            var hasReleaseNotes = hotfixReleaseNote.length > 0 || releaseNotes.length > 0;

            html += String.format(buildRow, build.Version, build.SMVersion, build.RMVersion, build.RADVersion, build.DeploymentDate, (hasHotfixes ? "" : "noHotfix ") + (hasReleaseNotes ? "" : "noReleaseNotes"),
                hasReleaseNotes ? hotfixReleaseNote : "", hasReleaseNotes ? releaseNotes : "");

            if (hasHotfixes) {
                var hotfixRowsHtml = "";
                build.Hotfixes.forEach((hotfix, hotfixIndex) => {
                    hotfixRowsHtml += String.format(hotfixRow, hotfix.HotfixId, hotfix.CaseId, hotfix.Version, hotfix.PublishDate, hotfix.DeploymentDate);
                });
                html += String.format(hotfixTemplate, hotfixRowsHtml);
            }
        });
        $("#deploymentHistoryRows").html(html);
    }
    function OnSucessRefGetHotfixes(result) {
        var html = "";
        var fileNames = $(window.parent.document).find("#hdnReleaseNotesFileNames").val().split("*");
        result.forEach((build, buildIndex) => {
            var hasHotfixes = build.Hotfixes != null && build.Hotfixes.length > 0;
            var hotfixReleaseNote = fileNames.find((fileName) => fileName == 'IVP RefMaster Hotfix Release Notes v' + build.Version + ".pdf");
            var releaseNotes = fileNames.find((fileName) => fileName.match(new RegExp("IVP RefMaster Release Notes [(][0-9].[0-9].[A-Z][0-9]*[.]*[A-Z]*[a-z]*[0-9]*[a-z]* to " + build.Version + "[)]")));
            hotfixReleaseNote = hotfixReleaseNote == undefined ? '' : hotfixReleaseNote;
            releaseNotes = releaseNotes == undefined ? '' : releaseNotes;
            var hasReleaseNotes = hotfixReleaseNote.length > 0 || releaseNotes.length > 0;

            html += String.format(buildRow, build.Version, build.RMVersion, build.RADVersion, build.DeploymentDate, (hasHotfixes ? "" : "noHotfix ") + (hasReleaseNotes ? "" : "noReleaseNotes"),
                hasReleaseNotes ? hotfixReleaseNote : "", hasReleaseNotes ? releaseNotes : "");

            if (hasHotfixes) {
                var hotfixRowsHtml = "";
                build.Hotfixes.forEach((hotfix, hotfixIndex) => {
                    hotfixRowsHtml += String.format(hotfixRow, hotfix.HotfixId, hotfix.CaseId, hotfix.Version, hotfix.PublishDate, hotfix.DeploymentDate);
                });
                html += String.format(hotfixTemplate, hotfixRowsHtml);
            }
        });
        $("#deploymentHistoryRows").html(html);
    }

    function registerEventListeners() {
        $(".BuildRow div:nth-child(6)").click((e) => {
            var target = $(e.target).closest(".BuildRow");
            if (target.hasClass('hotfixShown'))
                target.removeClass("hotfixShown");
            else {
                target.addClass("hotfixShown");
            }
        });
        $(".BuildRow div:nth-child(7)").click((e) => {
            var target = $(e.target).closest(".BuildRow");
            var releaseNotes = target.attr('RN');
            var version = target.find(":first-child").text();
            var hotfixReleaseNote = target.attr('hotfixRN');
            if (releaseNotes != null && releaseNotes != undefined && releaseNotes.length > 0)
                window.parent.leftMenu.createTab('Release Notes', "Release Notes/" + releaseNotes, 'unique_ReleaseNotes' + new Date().getTime(), 'Release Notes v' + version);
            if (hotfixReleaseNote != null && hotfixReleaseNote != undefined && hotfixReleaseNote.length > 0)
                window.parent.leftMenu.createTab('Hotfix Release Notes', "Release Notes/" + hotfixReleaseNote, 'unique_HotfixReleaseNotes' + new Date().getTime(), 'Hotfix Release Notes v' + version);
            window.parent.leftMenu.closePatchDeploymentHistory();
        })
        $(".exportOption").unbind("click").click((e) => {
            var targetText = $(e.target).text();
            switch (targetText) {
                case "Build History":
                    console.log("Build History");

                    CallCommonServiceMethod("getBuildInfo", { moduleId: moduleId }, (result) => {
                        var data = JSON.parse(result.d);
                        console.log("success");
                        console.log(data);
                        var exportData;
                        if (moduleId = 3) {
                            exportData = "Version,SM Version,RM Version,RAD Version,Deployment Date,Publish Date,Is Deployed Version,Is Active\n";
                            data.forEach(function (item, index) {
                                exportData += item.client_version + "," + item.version + "," + item["RefMaster version"] + "," + item["RAD version"] + "," + item.deployed_on + "," + item.published_on + "," + item.is_deployed_version + "," + item.is_active + "\n";
                            });
                        } else {
                            exportData = "Version,RM Version,RAD Version,Deployment Date,Publish Date,Is Deployed Version,Is Active\n";
                            data.forEach(function (item, index) {
                                exportData += item.client_version + "," + item.version + "," + item["RAD version"] + "," + item.deployed_on + "," + item.published_on + "," + item.is_deployed_version + "," + item.is_active + "\n";
                            });
                        }
                        var instanceName = $(window.parent.document).find("#environment_div").text();
                        download(exportData, "Build History-" + instanceName + ".csv", "text/csv");
                    }, OnFailure)
                    break;
                case "Hotfix History":
                    console.log("Hotfix History");
                    CallCommonServiceMethod("getHotfixInfo", null, (result) => {
                        var data = JSON.parse(result.d);
                        console.log("success");
                        console.log(data);
                        var exportData;
                        exportData = "Hotfix ID,Case ID,Version,Publish Date,Deployment Date,Included In Patch\n";
                        data.forEach(function (item, index) {
                            exportData += item.hotfix_id + "," + item.case_id + "," + item.version + "," + item.published_on + "," + item.deployment_date + "," + item.included_in_patch + "\n";
                        });
                        var instanceName = $(window.parent.document).find("#environment_div").text();
                        download(exportData, "Hotfix History-" + instanceName + ".csv", "text/csv");
                    }, OnFailure)
                    break;
            }
        })
    }

    function OnFailure() { }

    return patchDeploymentHistory;
})();

