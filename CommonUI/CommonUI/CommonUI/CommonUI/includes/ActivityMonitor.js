var path = window.location.protocol + '//' + window.location.host;
var pathname = window.location.pathname.split('/');

$.each(pathname, function (ii, ee) {
    if ((ii !== 0) && (ii !== pathname.length - 1))
        path = path + '/' + ee;
});
var commonServiceLocation = '/BaseUserControls/Service/CommonService.svc';

$(function initDataTable() {
    $.ajax({
        type: "POST",
        url: path + commonServiceLocation + "/GetActivityMonitorDetails",
        contentType: "application/json",
        dataType: "json",
        success: function (data) {
            var querydata = JSON.parse(data.d);
            console.log(querydata);
            var queryTable = $('#TableID').DataTable({
                paging: false,
                searching: false,
                bDestroy: true,
                data: querydata,
                //scrollX: "200%",
                //scrollY: "95vh",
                //scrollCollapse: true,
                //columnDefs: [{ "width":"20%"}],
                language: { "emptyTable": "No Data Found" },
                columns: [
                    { 'data': 'SPID' },
                    { 'data': 'Status' },
                    { 'data': 'LOGIN' },
                    { 'data': 'HostName' },
                    { 'data': 'BlkBy' },
                    { 'data': 'DBName' },
                    { 'data': 'Command' },
                    { 'data': 'CPUTime' },
                    { 'data': 'DiskIO' },
                    { 'data': 'LastBatch' },
                    { 'data': 'ProgramName' },
                    { 'data': 'SPID_1' },
                    { 'data': 'REQUESTID' },
                    {
                        'data': 'Text',
                        render: function (data, type, row) {
                                return '<a tabindex="0" class="btn graph-info-icon"' +
                                    ' role="button" data-html="true" data-toggle="popover" data-placement="left" title="Info" data-trigger="focus"' +
                                    ' data-content="' + data + '">'+
                                    '<i class="fa fa-info-circle" style="color:gray"></i></a >';
                        }
                    }
                ]
            });

            $('.graph-info-icon').popover({ trigger: 'focus' });
            
            $('#refresh').click(function () {
                initDataTable();
            });
        },
        error: function (result) {
           console.log(result);
        }
    });
});

