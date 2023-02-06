/// <reference path="jquery-1.4.1-vsdoc.js" />
(function ($) {
    $.fn.fileUpload = function (method) {
        var methods = {
            //public methods
            init: function (options) {
                settings = this.fileUpload.settings = $.extend({}, this.fileUpload.defaults, options);
                return this.each(function () {
                    var $element = $(this), // reference to the jQuery version of the current DOM element
                         element = this;      // reference to the actual DOM element
                    helpers.settings = settings;
                    helpers.DeleteAll();
                    helpers.createControl();

                });
            }
        }

        var helpers = {
            settings: undefined,
            timeInterval: 0,
            //private memebers
            createControl: function () {

                if (this.settings.parentControlId == undefined || this.settings.parentControlId == '')
                    $.error('Property parentControlId does not exist in fileUpload plugin!');
                var parent = $("#" + this.settings.parentControlId);
                var containerDiv = $('<div/>').attr({ 'class': 'fucontainerDiv', 'id': 'containerDiv' + this.settings.parentControlId });
                var fileElement =
                            $('<input/>').attr({
                                'type': 'file',
                                'id': 's' + this.settings.parentControlId
                            }).css('display', 'none');
                if (this.settings.multiple)
                    fileElement.attr('multiple', '');
                containerDiv.append(fileElement);
                containerDiv.append(
                            $('<div/>').attr({
                                'id': 'dropWindowDiv' + this.settings.parentControlId,
                                'class': 'fudropWindowDiv'
                            }).html('Please drop a file here').hide()
                        );
                containerDiv.append(
                            $('<div/>').attr({
                                'id': 'uploadedFiles' + this.settings.parentControlId
                            })
                        );

                parent.append(containerDiv);
                parent.append(
                            $('<div/>').attr({
                                'id': 'error' + this.settings.parentControlId,
                                'class': 'fuerrorMessage'
                            })
                        );

                $('#s' + this.settings.parentControlId).unbind('change').on('change', function (e) {
                    var files = e.target.files;
                    helpers.UploadFiles(files);
                    e.target.value = null;
                });

                $("#" + this.settings.attachementControlId).on('click', function () {
                    $('#s' + helpers.settings.parentControlId).click();
                });
                $(document).on('dragover', function (e) {
                    try {
                        if ($.inArray('Files', e.originalEvent.dataTransfer.types) >= 0) {
                            $("#" + helpers.settings.debuggerDiv).append('<br />File dragover');
                            $('#dropWindowDiv' + helpers.settings.parentControlId).show();
                            $('#containerDiv' + helpers.settings.parentControlId).show();
                            e.stopPropagation();
                            e.preventDefault();
                        }
                    }
                    catch (e) { alert(e); }
                });
                $(document).on('dragleave', function (e) {
                    if (e.originalEvent.clientX == 0) {
                        $("#" + helpers.settings.debuggerDiv).append('<br />File dragleave');
                        $('#dropWindowDiv' + helpers.settings.parentControlId).hide();
                        if ($('#uploadedFiles' + helpers.settings.parentControlId).contents().length == 0)
                            $('#containerDiv' + helpers.settings.parentControlId).hide();
                        e.stopPropagation();
                        e.preventDefault();
                    }
                });
                $(document).on(
                        'drop',
                        function (e) {
                            e.preventDefault();
                            e.stopPropagation();
                            $('#dropWindowDiv' + helpers.settings.parentControlId).hide();
                            if ($('#uploadedFiles' + helpers.settings.parentControlId).contents().length == 0)
                                $('#containerDiv' + helpers.settings.parentControlId).hide();
                            $("#" + helpers.settings.debuggerDiv).append('<br />Dropped on Document');
                        });
                $('#dropWindowDiv' + this.settings.parentControlId).on(
                        'drop', function (e) {
                            if (e.originalEvent.dataTransfer) {
                                if (e.originalEvent.dataTransfer.files.length > 0) {
                                    $("#" + helpers.settings.debuggerDiv).append('<br />File Dropped');
                                    //e.stopPropagation();
                                    e.preventDefault();
                                    helpers.UploadFiles(e.originalEvent.dataTransfer.files);
                                    $("#" + helpers.settings.debuggerDiv).append('<br />File Uploaded');
                                }
                                else {
                                    if ($('#uploadedFiles' + helpers.settings.parentControlId).contents().length == 0)
                                        $('#containerDiv' + helpers.settings.parentControlId).hide();
                                }
                            }
                        });
                $('#dropWindowDiv' + helpers.settings.parentControlId).css('top', parent.offset().top);
                $('#dropWindowDiv' + helpers.settings.parentControlId).css('left', parent.offset().left);
                $('#dropWindowDiv' + helpers.settings.parentControlId).css('width', parent.width());
            },

            UploadFiles: function (selectedFiles) {
                $('#dropWindowDiv' + helpers.settings.parentControlId).hide();
                if (helpers.settings.multiple == false && (selectedFiles.length > 1 || $('#uploadedFiles' + helpers.settings.parentControlId).contents().length == 1)) {
                    $("#error" + helpers.settings.parentControlId).html("Multiple files are not supported!");
                    if ($('#uploadedFiles' + helpers.settings.parentControlId).contents().length == 0)
                        $('#containerDiv' + helpers.settings.parentControlId).hide();
                    return;
                }
                if (selectedFiles.length == 0) {
                    if ($('#uploadedFiles' + helpers.settings.parentControlId).contents().length == 0)
                        $('#containerDiv' + helpers.settings.parentControlId).hide();
                    return;
                }
                var tobreak = false;
                $(selectedFiles).each(function (index, file) {
                    if (file.size > (MAX_FILE_SIZE * 1024 * 1024)) {
                        $("#error" + helpers.settings.parentControlId).html("There was error uploading files!! <b /> Max size limit is : " + MAX_FILE_SIZE + " MB");
                        tobreak = true;
                    }
                    if (tobreak == false) {
                        var divCtrl = $('<div/>').addClass("fuattachmentDiv");
                        var img = $('<span/>').attr(
                                            {
                                                'fileName': file.name,
                                                'class': "furemoveAttachment"
                                            }).click(helpers.Delete);
                        divCtrl.
                            append("<div>" + file.name + "</div>").
                            append("<div> (" + (file.size > 1024 ? Math.ceil(file.size / 1024) + " Kb)" : file.size + " b)") + "</div>").
                            append(img);
                        selectedFiles[index].spanNumber = 'span' + Math.random().toString().replace('.', '');
                        var progressBar = $('<div/>').attr({ 'isProgressBar': 'progressbar', 'spanNumber': selectedFiles[index].spanNumber });
                        divCtrl.append(progressBar);
                        $('#uploadedFiles' + helpers.settings.parentControlId).append(divCtrl);
                        progressBar.progressbar({ value: 100 });
                    }
                });
                if (tobreak) {
                    if ($('#uploadedFiles' + this.settings.parentControlId).contents().length == 0)
                        $('#containerDiv' + helpers.settings.parentControlId).hide();
                    return;
                }
                function callAjaxHelper(index, selectedFiles) {
                    //to create progress                    
                    //helpers.timeInterval = window.setInterval(function () {
                    //    var value = $("[isProgressBar='progressbar']").progressbar("value");
                    //    $("[isProgressBar='progressbar']").progressbar("value", value + (file.size / 1024));
                    //}, 100);
                    var progressBar = $("#uploadedFiles" + helpers.settings.parentControlId + " div[spanNumber='" + selectedFiles[index].spanNumber + "']");
                    var data = new FormData();
                    data.append(selectedFiles[index].name, selectedFiles[index]);

                    $.ajax({
                        type: "POST",
                        url: "RADXLFileUpload.ashx?controlId=" + helpers.settings.parentControlId + "&operation=Upload",
                        cache: false,
                        contentType: false,
                        processData: false,
                        data: data,
                        index: index,
                        selectedFiles: selectedFiles,
                        progressBar: progressBar,
                        success: function (result) {
                            $("#error" + helpers.settings.parentControlId).html('');
                            $('#containerDiv' + helpers.settings.parentControlId).show();
                            progressBar.progressbar("value", 100);
                            progressBar.progressbar("destroy");
                            progressBar.removeAttr('isProgressBar');
                            if (index + 1 < selectedFiles.length) {
                                callAjaxHelper(index + 1, selectedFiles);
                            }
                            else {
                                if (helpers.settings.returnEvent != undefined)
                                    helpers.settings.returnEvent(selectedFiles[index]);
                            }
                        },
                        error: function (q) {
                            $("#error" + helpers.settings.parentControlId).html("There was error uploading files!");
                        }
                    });
                }
                callAjaxHelper(0, selectedFiles);
            },
            Delete: function (e) {
                var data = new FormData();
                var fileName = $(e.target).attr("fileName");
                fileName=/[^/]*$/.exec(fileName)[0];
                $.ajax({
                    type: "POST",
                    url: "RADXLFileUpload.ashx?controlId=" + helpers.settings.parentControlId + "&operation=Delete&FileName=" + fileName,
                    cache: false,
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (result) {
                        $(e.target).parent().remove();
                        $("#error" + helpers.settings.parentControlId).html('');
                        if ($('#uploadedFiles' + helpers.settings.parentControlId).contents().length == 0)
                            $('#containerDiv' + helpers.settings.parentControlId).hide();
                        if (helpers.settings.deleteEvent != undefined)
                            helpers.settings.deleteEvent($(e.target).attr("fileName"));
                    },
                    error: function (q) {
                      $(e.target).parent().remove();
                        $("#error" + helpers.settings.parentControlId).html('');
                        if ($('#uploadedFiles' + helpers.settings.parentControlId).contents().length == 0)
                            $('#containerDiv' + helpers.settings.parentControlId).hide();
                        if (helpers.settings.deleteEvent != undefined)
                            helpers.settings.deleteEvent($(e.target).attr("fileName"));
                     //   $("#error" + helpers.settings.parentControlId).html("There was error deleting files!");
                    }
                });
            },

            DeleteAll: function (e) {
                var data = new FormData();
                $.ajax({
                    type: "POST",
                    url: "RADXLFileUpload.ashx?controlId=" + this.settings.parentControlId + "&operation=DeleteAll",
                    cache: false,
                    contentType: false,
                    processData: false,
                    data: data
                });
            }
        }

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method "' + method + '" does not exist in fileUpload plugin!');
        }
    }

    $.fn.fileUpload.defaults = {
        parentControlId: '',
        attachementControlId: '',
        multiple: true,
        debuggerDiv: '',
        returnEvent: undefined,
        deleteEvent: undefined
    }

    $.fn.fileUpload.settings = {}
})(jQuery);


var SPEED = 10; //(kbps)
var MAX_FILE_SIZE = 10; //(MB)