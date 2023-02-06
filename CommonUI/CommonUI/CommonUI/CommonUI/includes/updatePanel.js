
//script Used in SMReportingTaskSetup.ascx for handling grid resize in collapsible panel

function SMApplySlimScroll(containerSelector, bodySelector) {
    setTimeout(function () {
        var scrollBodyContainer = $(containerSelector);
        var scrollBody = scrollBodyContainer.find(bodySelector);

        for (var index = 0; index < scrollBodyContainer.length; index++) {
            if (scrollBody.eq(index).length > 0 && scrollBodyContainer.eq(index).height() < scrollBody.eq(index).height()) {
                scrollBody.eq(index).smslimscroll({ height: scrollBodyContainer.eq(index).height() + 'px', alwaysVisible: true, position: 'right', distance: '2px' });
            }
        }
    }, 500);
}

function attachHandlerToCollapsiblePanel(collapsePanelId, gridId, topPanelId, middlePanelId, bottomPanelId, subtractHeight, subtractWidth) {
    var collapsiblePanel = $find(collapsePanelId);
    if (collapsiblePanel != null && collapsiblePanel != undefined)
        collapsiblePanel.add_expanded(expandHandler);
    resizeGridFinal(gridId, topPanelId, middlePanelId, bottomPanelId, subtractWidth, subtractHeight, true, false);
}

function expandHandler(sender, args) {
    $find($("[id$=xlGridReportingTask]").attr('id')).refreshWithCache();
}

var flagHoldOnUpdatingForValidate = false;
function onUpdating() {
    IsPageReloading = true;
    try {
        if (!flagHoldOnUpdatingForValidate) {
            $get('disableDiv').style.display = '';
            $get('loadingImg').style.display = '';
            scrollHeight = document.body.scrollHeight;
            scrollWidth = document.body.scrollWidth;
            //windowHeight = document.body.parentNode.offsetHeight;
            windowHeight = document.body.parentNode.clientHeight;
            IsPageReloading = true;
            $get('disableDiv').style.height = scrollHeight + "px";
            $get('disableDiv').style.width = scrollWidth + "px";
        }
        flagHoldOnUpdatingForValidate = false;
    }
    catch (err) {
    }
    return true;
}

var donotHide = false;
function onUpdated() {
    IsPageReloading = false;
    try {
        if (donotHide == undefined || donotHide == false) {
            $get('disableDiv').style.display = 'none';
            $get('loadingImg').style.display = 'none';
        }
        donotHide = false;
        IsPageReloading = false;
        document.getElementById('identifier').value = "";
        $("div[id$='_divMain']").find('option').each(function (index, value) {
            $(value).attr('title', $(value).textNBR());
        });

        var hdnSecurityLevelPrioritization = $("input[type='hidden'][id$='hdnSecurityLevelPrioritization']");
        if (hdnSecurityLevelPrioritization.length > 0) {
            $(document).keydown(function (e) {
                if (e.keyCode == 13) {
                    e.preventDefault();
                }
            });
            var txtSearch = $("input[type='text'][id$='txtSearch']");
            var btnSearch = $("input[type='submit'][id$='btnSearch']");
            txtSearch.unbind('keydown').keydown(function (e) {
                if (e.keyCode == 13) {
                    btnSearch.click();
                }
            });
        }
    }
    catch (err) {
    }
}

function onServiceUpdating() {
    $get('disableDiv').style.display = '';
    $get('loadingImg').style.display = '';
    scrollHeight = document.body.scrollHeight;
    scrollWidth = document.body.scrollWidth;
    //windowHeight = document.body.parentNode.offsetHeight;
    windowHeight = document.body.parentNode.clientHeight;
    if (windowHeight > scrollHeight)
        scrollHeight = windowHeight;
    $get('disableDiv').style.height = scrollHeight + "px";
    $get('disableDiv').style.width = scrollWidth + "px";
}

function onServiceUpdated() {
    $get('disableDiv').style.display = 'none';
    $get('loadingImg').style.display = 'none';
}

function onUpdatedLeft() {
    try {
        $get('disableDiv').style.display = 'none';
        $get('loadingImg').style.display = 'none';
        ClearFields();
    }
    catch (err) {
    }
}


function onUpdatingNew() {

    parent.document.getElementById('disableDiv').style.display = '';
    parent.document.getElementById('loadingImg').style.display = '';
    var scrollHeight = parent.document.body.scrollHeight;
    var scrollWidth = parent.document.body.scrollWidth;
    parent.document.getElementById('disableDiv').style.height = scrollHeight + "px";
    parent.document.getElementById('disableDiv').style.width = scrollWidth + "px";
}

function onUpdatedNew() {
    parent.document.getElementById('disableDiv').style.display = 'none';
    parent.document.getElementById('loadingImg').style.display = 'none';
}

function onUpdatingHomeLite() {
    document.getElementById('disablediv').style.display = '';
    document.getElementById('loadingimg').style.display = '';
    var scrollHeight = document.body.scrollHeight;
    var scrollWidth = document.body.scrollWidth;
    document.getElementById('disablediv').style.height = scrollHeight + "px";
    document.getElementById('disablediv').style.width = scrollWidth + "px";
}