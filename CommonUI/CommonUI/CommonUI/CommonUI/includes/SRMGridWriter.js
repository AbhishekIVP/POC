var gridwriter = (function () {
    function gridWriter() {
    }
    var gridwriter = new gridWriter();

    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    var _controlIdInfo = null;
    var _securityInfo = null;

    gridWriter.prototype.GridWriterInitializer = function SMSGridWriter(controlInfo, securityInfo) {
        $(function () {
            _controlIdInfo = eval("(" + controlInfo + ")");
            _securityInfo = eval("(" + securityInfo + ")");

            ResizeGridWriterContainer();
            iago.user.userName = _securityInfo.UserName;

            var obj = {
                identifier: "gridWriterContainer",
                DBName: "IVPSecMaster",
                //dbIdentifier: "SecMDB",
                dbIdentifierList: ["radDB", "SecMDB", "SecMArchiveDB", "SecMVendorDB", "SecMVendorArchiveDB", "CorpActDB", "CorpActArchiveDB", "CorpActVendorDB", "CorpActVendorArchiveDB", "refMDBVendor", "refMDBVendor_Archive", "refMDB", "refMDB_Archive", "CTMDB"],
                //identifierForReport: "identifierxyz",
                moduleid: _securityInfo.ModuleId,
                IsNewReport: true,
                ShowReport: false,
                baseUrl: path,
                AssemblyName: "CommonService",
                ClassName: "CommonService.GridWriterService",
                RequireDataDictionary: false
            }

            if (_securityInfo.ReportName != null && _securityInfo.ReportName.length > 0) {
                obj.reportName = _securityInfo.ReportName;
                obj.IsNewReport = false;
                obj.ShowReport = true;
            }

            rradtoolKit.handlers.initialize(obj);

            if (_securityInfo.TabName != null && _securityInfo.TabName.length > 0) {
                try {
                    window.parent.leftMenu.createTab('', '', '', _securityInfo.TabName);
                }
                catch (e)
                { }
            }

            AttachHandlers();
        });
    }

    function ResizeGridWriterContainer() {
        var SRMGridWriterContainer = $(".SRMGridWriterContainer");

        var height = $(window).height() - $(".SRMGridWriterTopBarContainer").outerHeight(true) - 10;
        SRMGridWriterContainer.height(height);
        $('#gridWriterContainer').height(height - 10);
    }

    function AttachHandlers() {
        $(window).resize(function () {
            ResizeGridWriterContainer();
        });
    }

    return gridwriter;
})();