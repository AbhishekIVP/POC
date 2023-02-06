function generateGrid(mainContainer, columns, data, height) {
    $(mainContainer).empty();
    $(mainContainer).append('<div id="gridHead" class="gridHead"><div style="display:inline-block; height:20px;width:20px;"></div></div><div id="tBody"></div><div id="gridFooter">RAD<span style="color:white">.CommonTaskManager</span></div>');

    //generate header
    for (var i = 0; i < columns.length; i++) {
        $('#gridHead').append('<div class="cell c' + i + ' ' + columns[i].id + '" style="display:inline-block;' + (columns[i].width != undefined ? ('width:' + columns[i].width + 'px;') : '') + (columns[i].width == 0 ? 'display:none' : '') + '">' + columns[i].name + "</div>");
    }

    //generate body
    for (var j = 0; j < data.length; j++) {
        var gridRow = (data[j].children.length > 0 ? '<div class="expandBtn" ></div>' : '<div style="display:inline-block; height:20px;width:20px;"></div>');
        var children = '';
        for (var i = 0; i < columns.length; i++) {
            //columns[i].type == "DateTime" ? (console.log(data[j][columns[i].id])):'';
            gridRow += "<div class='cell c" + i + " " + columns[i].id + "' " + (columns[i].extra != undefined ? columns[i].extra : '') + " title='" + columns[i].name + "' style='display:inline-block; " + (columns[i].width != undefined ? ('width:' + columns[i].width + 'px;') : '') + (columns[i].width == 0 ? 'display:none' : '') + "'>" + (data[j][columns[i].id] == undefined ? '' : (columns[i].type == "DateTime" ? (eval("new " + data[j][columns[i].id].substring(1, data[j][columns[i].id].length - 1) + ".toLocaleString()")) : data[j][columns[i].id])) + "</div>";
        }
        //console.log(data[j].children.length);
        for (var k = 0; k < data[j].children.length; k++) {

            children += "<div class='children-row row'><div style='display:inline-block; height:20px;width:20px;'></div>";
            for (var l = 0; l < columns.length; l++) {
                children += "<div class='cell c" + l + " " + columns[l].id + "' style='display:inline-block;" + (columns[l].width != undefined ? ('width:' + columns[l].width + 'px;') : '') + (columns[l].width == 0 ? 'display:none' : '') + "'>" + (data[j].children[k][columns[l].id] == undefined ? '' : (columns[l].type == "DateTime" ? '' : data[j].children[k][columns[l].id])) + "</div>"
            }
            children += "</div>";
        }
        $('#tBody').append("<div   class='gridRowContainer'><div class ='gridRow row " + (j % 2 == 0 ? 'even' : 'odd') + "'>" + gridRow + "</div>" + (children != '' ? ("<div class='children'>" + children) : '') + "</div></div>");
    }
    console.log(data);
    $('#tBody').css('height', height + "px");
    try {
        $('.row').each(function (i, e) {
            if ($(e).children('.subscribe_id').text() != "") {
                $(e).children('.subscribe').addClass('subscribed');
            }
        });
        $("#tBody .gridRow .last_run_status").each(
        function (i, e) {
            if ($(e).text() == 'true') {
                $(this).addClass('green');
                $(this).text('');

            }
            else {
                $(this).text('');

            }
        });

        $("#tBody .children-row .last_run_status").each(
        function (i, e) {
            $(e).text('');
        });

        $("#tBody .row .is_muted").each(
        function (i, e) {
            if ($(e).text() == 'true') {
                $(this).addClass('muted');
                $(this).text('');

            }
            else {
                $(this).addClass('unmuted');
                $(this).text('');
                $(this).attr('title', 'Un-muted');

            }
        });
    }
    catch (e) { console.log(e); }
    if (animateGrid == true) {
        $('.row').hide();

        $('.row.even').show('slide', { direction: "left" });
        $('.row.odd').show('slide', { direction: "right" });
        $('.children-row.row').show();
    }
}

