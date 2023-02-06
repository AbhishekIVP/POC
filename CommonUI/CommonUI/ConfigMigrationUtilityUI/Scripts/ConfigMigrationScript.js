function GetTabs(module) {
    $('[id$="Div_Option_Sources"]').slideUp();
    $("#OptionsDiv").css("display", "none");

    var viewPortWidth = viewportSize.getWidth();
    var mr = $('.middle-right').width();
    var width = viewPortWidth -mr;
    $('.middleContainerBefore').css('width', width);

    if ($('[id*="hdn_app"]').val() === '') {
        switch (module) {
            case 'secmaster':
                $('[id $= "li_1"]').addClass('tab-selected');
                $('[id $= "li_2"]').addClass('tab-normal');
                break;
            case 'refmaster':
                $('[id $= "li_2"]').addClass('tab-selected');
                $('[id $= "li_1"]').addClass('tab-normal');

                break;
    }

        $('.tab-class').click(function (e) {
            if (!$(this).hasClass('tab-selected')) {
                $(this).removeClass('tab-normal');
                $(this).addClass('tab-selected');

                if (e.target.id.indexOf('li_1') > -1) {
                    $('[id $= "li_2"]').removeClass('tab-selected');
                    $('[id $= "li_2"]').addClass('tab-normal');
    } else {
        $('[id $= "li_1"]').removeClass('tab-selected');
        $('[id $= "li_1"]').addClass('tab-normal');
    }
    }
            var text = $('.tab-selected').eq(0).text().trim();
            if (text.indexOf("SecMaster") > -1) {
                new SMSMigrationUtilityMethods().CompareAllSuccess();
            }
    else if (text.indexOf("RefMaster") > -1) {
                new RMSConfigMigrationUtilityMethods().CompareAllSuccess();
    }
    });
    }
    if ($('[id*="hdn_app"]').val() === 'refmaster') {
        //changes for standalone ref
        $(".middle").css('margin-top', '27px');
        $('.middleContainerBefore').css('width', viewPortWidth);
        $('.middleContainerBefore').css('margin-left', '0px');
        $('.middle-right').css('display', 'none');
    }



    $('[id$="UploadConfigFileBtn"]').prop("disabled", true);
    $('[id$="DownloadConfigBtn"]').prop("disabled", true);

    $('[id$="Div_Option_Sources"]').on('click', '.Option_Source', function (e) {
        e.stopPropagation();
        var ahtml;
        var isFileSource = false;
        $('#psuedo_Div_Source').html($(this).text() + '<input type="hidden" style="display:none" value="' +$(this)[0].children[0].value + '">' + '<div class="Div_Arrow"></div>');
        $('[id$="Div_Option_Sources"]').slideUp('slow');
        $('[id$="Div_Option_Sources"]').css("display", "none"); $("#OptionsDiv").css("display", "none");
        $('[id$="CompareBtn"]').removeClass('middle-center-button-disabled');
        $('.middle-center').css('top', '0px');
        $(this).css('outline', 'none');

        if (e.target.textContent == 'File Source') {
            isFileSource = true;
            $('#SRMigrationFileUpload').css('display', '');
            SRMigrationUploadFileWidget('SRMigrationFileUploadWidget', 'SRMigrationFileUploadAttachement');
        }
        else {
            isFileSource = false;
            $('#SRMigrationFileUpload').css('display', 'none');
        }

        $('[id$="CompareBtn"]').unbind('click').click(function (e) {
            e.stopPropagation();
            var tabSelectedobject = new Object();
            tabSelectedobject.selectedTab = $('.tab-selected').eq(0).text().trim();
            tabSelectedobject.selectedInstance = $("#psuedo_Div_Source").text();
            var obb = new Object();
            var temp = $("#psuedo_Div_Source")[0].children[0].value;
            var arr = temp.trim().split('|');
            var hasSecm = false;
            var hasRefm = false;

            if (isFileSource) {
                hasRefm = false;
                hasSecm = false;
                tabSelectedobject.SourceType = 'File Source';
                var selTab = $('.tab-selected').eq(0).text().trim();
                if (selTab === '' || selTab.indexOf("RefMaster") > -1)
                    hasRefm = true;
                if (selTab.indexOf("SecMaster") > -1)
                    hasSecm = true;
                tabSelectedobject.serviceURL = '';
                    }
else {
                if (arr.length == 2) {
                    var arrt = arr[0].trim().split(',');
                    var arrt2 = arr[1].trim().split(',');
                        if (arrt2.length === 0)
                        arrt2 =['', ''];
                    for (var c = 0; c < arrt.length; c++) {
                        if (arrt[c].toLowerCase() === 'secmaster')
                            hasSecm = true;
else if (arrt[c].toLowerCase() === 'refmaster')
    hasRefm = true;
                        eval('obb.' + arrt[c].toLowerCase() + ' = arrt2[c].trim();');
                    }
                }
                tabSelectedobject.serviceURL = obb;
            }

            if (hasSecm) {
                var secmObj = new SMSMigrationUtilityMethods();
                secmObj.CompareAll(tabSelectedobject);
                        }
            if (hasRefm) {
                $('#SRMigrationFileUpload').css('display', 'none');
                var refmObj = new RMSConfigMigrationUtilityMethods();
                refmObj.CompareAll(tabSelectedobject);
            }
            return false;
            });
            });

    $('[id$="Div_Option_Sources"]').on('mouseover', '.Option_Source', function (e) {
        $(this).css({ "border-left": "4px solid rgb(45, 190, 253)", "cursor": "pointer", "background": "#EDEDED" });
            });

    $('[id$="Div_Option_Sources"]').on('mouseout', '.Option_Source', function (e) {
        $(this).css({ "border-left": "4px solid white", "cursor": "default"
                        });
                        });

    $('#psuedo_Div_Source').mouseover(function () {
        $(this).css('outline', '1px solid rgb(0, 102, 255)');
        var arrow = $(this).children('.Div_Arrow');
        $(arrow).css({ "background-color" : "rgb(216,216,216)", "cursor": "pointer", "border-left": "1px solid black" });
        });

    $('#psuedo_Div_Source').mouseout(function () {
        $(this).css('outline', 'none');
        var arrow = $(this).children('.Div_Arrow');
        $(arrow).css({
"background-color": "white", "cursor" : "default", "border-left": "none" });
        });

    $('#psuedo_Div_Source').click(function (e) {
        e.stopPropagation();
        if ($('[id$="Div_Option_Sources"]').css('display') === 'none') {
            $(this).css('outline', '1px solid rgb(0, 102, 255)');
            $('[id$="Div_Option_Sources"]').css("display", "");
                            $("#OptionsDiv").css("display", "");
                            $('[id$="Div_Option_Sources"]').slideDown('slow');
        }

                            else {
            $('[id$="Div_Option_Sources"]').slideUp('slow');
            $('[id$="Div_Option_Sources"]').css("display", "none"); $("#OptionsDiv").css("display", "none");
            $(this).css('outline', 'none');
            }

        if ($('[id*="hdn_app"]').val() === '') {
            var text = $('.tab-selected').eq(0).text().trim();
            if (text.indexOf("SecMaster") > -1) {
                closeDDLs("psuedo_Div_Source");
                }
                else if (text.indexOf("RefMaster") > -1) {
                var obj = new RMSConfigMigrationUtilityMethods();
                obj.Refm_closeDDLs("psuedo_Div_Source");
                }
                }
                else {
            var obj = new RMSConfigMigrationUtilityMethods();
            obj.Refm_closeDDLs("psuedo_Div_Source");
                }
                });

    $('[id$="DownloadBtn"]').unbind('click').click(function (e) {
        var selected = $('.tabs-list').find('.tab-selected').text();
		if (selected.trim() === '')
			selected = 'refmaster';
        if (selected.trim().toLowerCase() === 'refmaster') {
            var refmObj = new RMSConfigMigrationUtilityMethods();
            refmObj.DownloadConfig('SRMigrationfiledownloadIframe');
            }
            else if (selected.trim().toLowerCase() === 'secmaster') {
            var secmObj = new SMSMigrationUtilityMethods();
            secmObj.DownloadConfig();
        }
        });
        }


var SRMigrationUploadFileWidget = function (parentId, attachmentId) {

    $('#' +parentId).remove();
    $('#SRMigrationFileUpload').append('<div  style="border:0px;" id="' + parentId + '"><div class="SRMigrationLabeledInput" style="width: 100%; border:0px;padding: 0px; text-indent: 8px;" id="' +attachmentId + '"> Click here or drop a file to upload</div>')
    if ($('#' +parentId).fileUpload != undefined) {
        $('#' +parentId).fileUpload({
            'parentControlId': parentId,
            'attachementControlId': attachmentId,
            'multiple': false,
            'debuggerDiv': '',
            'returnEvent': function () {
                var selected = $('.tabs-list').find('.tab-selected').text();

                if (selected.trim().toLowerCase() === 'refmaster') {
                    //                    var refmObj = new RMSConfigMigrationUtilityMethods();
                    //                    refmObj.DownloadConfig('SRMigrationfiledownloadIframe');
                }
                else if (selected.trim().toLowerCase() === 'secmaster') {
                    var secmObj = new SMSMigrationUtilityMethods();
                    secmObj.UploadConfig();
                }
            },
            'deleteEvent': function () {
                //alert('deleted'); } 
            }
        });
        }
        }
