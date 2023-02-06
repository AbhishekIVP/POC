var SRMCustomFileUploader = function (fileUploaderUrl, fileUploaderParams) {
    var allowedFileTypesForUpload = "*";

    function uploadFile(e, target) {
        if (e.originalEvent.dataTransfer || e.target) {
            var files;
            if (e.originalEvent.dataTransfer)
                files = e.originalEvent.dataTransfer.files;
            else if (e.target)
                files = e.target.files;

            if (isFilesUploaded(target))
                return;

            var data = new FormData();
            for (var i = 0; i < files.length; i++) {
                if (checkFileType(files[i].name))
                    data.append('file-' + i, files[i]);
                else {
                    $("div.upload-file-error", target).text("File type not supported");
                    $("div.upload-file-placeholder", target).hide();
                    $("div.uploading-file-progress", target).hide();
                    $("div.upload-file-error", target).show();
                    return;
                }
            }

            if (e.target)
                $(e.target).val("");

            $("div.uploading-file-progress", target).show();
            $("div.upload-file-error", target).hide();
            $("div.upload-file-placeholder", target).hide();
            $.ajax({
                url: getFileUploaderUrl(target),
                data: data,
                cache: false,
                contentType: false,
                processData: false,
                type: 'POST',
                error: function (jqXHR, textStatus, errorThrown) {
                    $("div.uploading-file-progress", target).hide();
                    $("div.upload-file-error", target).text("Error Uploading File");
                    $("div.upload-file-error", target).show();
                    console.log("File Uploader: textStatus-" + textStatus + " errorThrown-" + errorThrown);
                }
            }).done(function (result) {
                if (result != "FAILED") {
                    $("div.uploading-file-progress", target).hide();
                    var fileDetails = result.split('|');
                    addFile(target, fileDetails[1], fileDetails[1], false, fileDetails[0], fileDetails[2]);
                    $(target).attr("CurrentFilesLoaded", result);
                    callbackOnUploadSuccess();
                }
                else {
                    $("div.uploading-file-progress", target).hide();
                    $("div.upload-file-error", target).text("Error Uploading File");
                    $("div.upload-file-error", target).show();
                }
            });
        }
    }

    function getFileUploaderUrl(target) {
        var url = "";
        if ($(target).attr("fileUploaderPath") == undefined)
            url = fileUploaderPath;
        else
            url = $(target).attr("fileUploaderPath");

        var paramNames;
        if ($(target).attr("params") == undefined)
            paramNames = fileUploaderParamNames.split(',');
        else
            paramNames = $(target).attr("params").split(',');

        var paramString = "";
        for (var i = 0; i < paramNames.length; i++) {
            if (paramNames[i] == "")
                continue;
            paramString += paramNames[i] + "=";
            paramString += $(target).attr(paramNames[i]);
            paramString += "&";
        }
        if (paramString != "") {
            paramString = paramString.substr(0, paramString.length - 1);
            url += "?" + paramString;
        }

        return url;
    }

    function checkFileType(filename) {
        if (allowedFileTypesForUpload == "" || allowedFileTypesForUpload == "*")
            return true;

        var index = filename.lastIndexOf('.');
        if (index == -1)
            return false;
        var extension = filename.substr(index);
        index = allowedFileTypesForUpload.toLowerCase().split('|').indexOf(extension.toLowerCase());
        if (index == -1)
            return false;
        return true;
    }

    function addFile(target, fileName, fileDisplayName, isReadOnly, filePath, fileExtension) {
        var span = getFileContainer(fileName, fileDisplayName, isReadOnly, filePath, fileExtension);
        $(".file-uploader-files-list", target).append($(span));
        span = getDeleteFile();
        $(".file-uploader-files-list", target).append($(span));
        //$(".icon", target).removeClass("iconUpload").addClass("iconDownload");
        $(".icon", target).removeClass("iconUpload").removeClass("fa-upload").removeClass("upload-specific-class");
        $(".upload-file-placeholder", target).hide();
        $("input[type='file'].inputFileControl", target).attr("disabled", "disabled");
        unbindEvents($(target));
    }

    function isFilesUploaded(target) {
        if ($(".file-uploader-files-list", target).html() == "")
            return false;
        return true;
    }

    function downloadFileClickHandler(e) {
        return false;
        e.stopPropagation();
        document.location = fileUploaderPath + "?downloadFile=true&filename=" + encodeURIComponent($(e.target).attr("realFileName"));
    }

    function removeFileClickHandler(e) {
        var divParent = $(e.target).parents("div")[0];
        $(divParent).attr("currentfilesloaded", "");
        $(".icon", divParent).removeClass("iconDownload");
        $(".icon", divParent).removeClass("iconPreview fa fa-eye");
        $(".icon", divParent).addClass("iconUpload");
        $(".upload-file-placeholder", divParent).show();
        $("input[type='file'].inputFileControl", divParent).removeAttr("disabled");
        bindEvents($(divParent));
        $(e.target).parent().parent().html("");
    }

    function getDeleteFile() {
        var span = $("<span>");
        $(span).addClass("fa").addClass("fa-trash").addClass("delete-file");
        $(span).off('click').on('click', closeButtonCallback);
        return $(span);
    }

    function getFileContainer(fileName, fileDisplayName, isReadOnly, filePath, fileExtension) {
        var span = $("<span>");
        $(span).addClass("srmmigration-upload-path-class");
        $(span).text(fileDisplayName);
        $(span).attr("realFileName", fileName);
        $(span).attr("title", fileDisplayName);
        $(span).attr("filePath", filePath);
        //alternatively can use a span with content
        $(span).attr("fileExtension", fileExtension);

        //$(span).on('click', downloadFileClickHandler);

        //if (isReadOnly == false || (isReadOnly != true && isReadOnly.toLowerCase() == "false")) {
        //    var delBtn = $("<span>");
        //    $(delBtn).addClass("SRMCancelBtn");
        //    //$(delBtn).css('float', 'none');
        //    $(delBtn).on('click', function (e) {
        //        e.stopPropagation();
        //        removeFileClickHandler(e);
        //    });
        //    $(span).append($(delBtn));
        //}

        return $(span);
    }

    function getInputFileControl() {
        var input = $("<input>");
        $(input).attr("type", "file");
        $(input).addClass('inputFileControl');
        $(input).hide();

        $(input).on('change', function (e) {
            uploadFile(e, $(this).parent());
        });
        $(input).on('click', function (e) {
            e.stopPropagation();
        });

        return $(input);
    }

    function getUploadedFilesContainer() {
        var div = $("<div>");
        $(div).addClass("file-uploader-files-list");

        return $(div);
    }

    function getFileUploaderIcon() {
        var div = $("<div>");
        $(div).addClass("icon");
        $(div).addClass("iconUpload");
        $(div).addClass("fa");
        //$(div).addClass("fa-upload");
        $(div).addClass("upload-specific-class");
        $(div).unbind('click').on('click', function (e) {
            var divParent = $(e.target).parent();
            if ($(this).hasClass("iconUpload")) {
                var fileControl = $("input[type='file'][disabled=''].inputFileControl", divParent);
                if (fileControl != undefined)
                    $(fileControl).trigger('click');
            }
            else if ($(this).hasClass("iconDownload")) {
                $(".file-uploader-files-list span[realfilename]", divParent).trigger('click');
            }
        });
        return $(div);
    }

    function getProgressControl() {
        var div = $("<div>");
        $(div).text("Uploading File...");
        $(div).addClass("uploading-file-progress");
        $(div).hide();

        return $(div);
    }

    function getUploadErrorControl() {
        var div = $("<div>");
        $(div).text("Error Uploading File");
        $(div).addClass("upload-file-error");
        $(div).hide();

        return $(div);
    }

    function getPlaceholderControl() {
        var div = $("<div>");
        $(div).text("Drag and Drop or click here to Upload ");
        $(div).addClass("upload-file-placeholder");

        return $(div);
    }

    function bindEvents(jqueryElemRef) {
        $(jqueryElemRef).unbind('dragover').on('dragover', function (e) {
            e.preventDefault();
            e.stopPropagation();
            $(this).addClass("dragover");
        });

        $(jqueryElemRef).unbind('dragleave').on('dragleave', function (e) {
            e.preventDefault();
            e.stopPropagation();
            $(this).removeClass("dragover");
        });

        $(jqueryElemRef).unbind('drop').on('drop', function (e) {
            e.preventDefault();
            e.stopPropagation();
            $(this).removeClass("dragover");
            uploadFile(e, this);
        });

        $(jqueryElemRef).unbind('click').on('click', function (e) {
            $("input.inputFileControl", this).trigger('click');
        });
    }

    function unbindEvents(jqueryElemRef) {
        $(jqueryElemRef).unbind('dragover');
        $(jqueryElemRef).unbind('dragleave');
        $(jqueryElemRef).unbind('drop');
        $(jqueryElemRef).unbind('click');
    }

    var fileUploaderPath;
    var fileUploaderParamNames;
    var callbackOnUploadSuccess;
    var closeButtonCallback;

    return {
        init: function (allReadOnly, allowedFileTypes, fileControlClass, successfulUploadCallback, closeButtonCallbackFunction) {
            callbackOnUploadSuccess = successfulUploadCallback;
            fileUploaderPath = fileUploaderUrl;
            fileUploaderParamNames = fileUploaderParams;
            if (allReadOnly == undefined)
                allReadOnly = false;
            if (allowedFileTypes != undefined)
                allowedFileTypesForUpload = allowedFileTypes;

            closeButtonCallback = closeButtonCallbackFunction;
            
            //not required : Instead Use a div to where bindings are applied
            var fileUploadersCommonClass = $("." + fileControlClass);
            $(fileUploadersCommonClass).addClass("SRMfile-uploader-parent");
            bindEvents($(fileUploadersCommonClass));

            if (allReadOnly == true || (allReadOnly != false && allReadOnly.toLowerCase() == "true")) {
                unbindEvents($(fileUploadersCommonClass));
            }

            for (var i = 0; i < $(fileUploadersCommonClass).length; i++) {
                var elem = $(fileUploadersCommonClass)[i];
                $(elem).append(getUploadedFilesContainer());
                $(elem).append(getFileUploaderIcon());
                var loadedFilesString = $(elem).attr("CurrentFilesLoaded").trim();

                var isReadOnly = false;

                if (allReadOnly == true || (allReadOnly != false && allReadOnly.toLowerCase() == "true"))
                    isReadOnly = allReadOnly;
                else
                    isReadOnly = $(elem).attr("isReadOnly");

                var inputFileControl = getInputFileControl();
                $(elem).append(getPlaceholderControl());
                $(elem).append(inputFileControl);
                $(elem).append(getProgressControl());
                $(elem).append(getUploadErrorControl());

                if (loadedFilesString != "" && loadedFilesString != '||' && loadedFilesString != '|') {
                    var fileInfo = loadedFilesString.split('|');
                    addFile(elem, fileInfo[1], fileInfo[1], isReadOnly, fileInfo[0], fileInfo[2]);
                }

                if (isReadOnly == true || (isReadOnly != false && isReadOnly.toLowerCase() == "true")) {
                    $(inputFileControl).attr("disabled", "disabled");
                    unbindEvents($(elem));
                    $(elem).addClass("disabled");
                }
            }

            $(fileUploadersCommonClass).addClass("SRMCustomFileUploader");
        },
        refreshUploadedFiles: function (fileUploadersCommonClass) {
            if (fileUploadersCommonClass == undefined)
                fileUploadersCommonClass = $("div[CustomFileUploadControl]");

            for (var i = 0; i < $(fileUploadersCommonClass).length; i++) {
                var elem = $(fileUploadersCommonClass)[i];
                var loadedFilesString = $(elem).attr("CurrentFilesLoaded").trim();

                var isReadOnly = isReadOnly = $(elem).attr("isReadOnly");

                $(".file-uploader-files-list", elem).html("");
                $(".icon", elem).removeClass("iconDownload");
                $(".icon", elem).removeClass("iconPreview fa fa-eye");
                $(".icon", elem).addClass("iconUpload");
                $(".upload-file-placeholder", elem).show();
                $("input[type='file'].inputFileControl", elem).removeAttr("disabled");
                bindEvents($(elem));

                if (isReadOnly == true || (isReadOnly != false && isReadOnly.toLowerCase() == "true")) {
                    unbindEvents($(elem));
                    $(elem).addClass("disabled");
                }

                if (loadedFilesString != "" && loadedFilesString != '||' && loadedFilesString != '|') {
                    var fileInfo = loadedFilesString.split('|');
                    addFile(elem, fileInfo[1], fileInfo[1], isReadOnly, fileInfo[0], fileInfo[2]);
                }
            }
        }
    }
};

//var SRMFileUpload = new SRMCustomFileUploader("App_Dynamic_Resource/RefMasterUI,com.ivp.refmaster.ui.BaseUserControls.Entity.FileUpload.aspx", "directUploadFile")

//init se call the file upload control and set all reaed only and allowed file types too
//
//