var srmDqmRM = function () {
    var sm1 = '';
    function DqmRM() { }

    DqmRM.prototype.manageTaskClickEvent = function (chainId, flowId, date, chartContainerID, obj) {
        console.log('rm method invoked');
    };

    return new DqmRM();

} ();
srmDQMStatus.registerSrmDqmClient(6, srmDqmRM);