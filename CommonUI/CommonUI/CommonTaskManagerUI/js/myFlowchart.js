function findHeadNode(flows) {
    var head;
    $.each(flows, function (i, e) { if (!e.dependant_on_id) { head = e; return false; } });
    return head;
}

function generateDependancyGraphItems(flows) {
    $('#taskChart').empty();
    $('#taskChart').height(0);
    $('#taskChart').width(0);

    //$('#taskChart').text('Dependancy Chart ');
    var taskChartItemHeight = 17;
    var i = 0; var x = 10; var y = 20;
    //var xoffset = 10;
    var head = findHeadNode(flows);
    try {
        findAllDepsAndDraw(head, x, y, flows);
    }
    catch (e) { alert(e.ToString()); }
    drawedNodes = Array();
}


var xoffset = 100;
var yoffset = 50;
var drawedNodes = Array();
function findAllDepsAndDraw(node, x, y, flows) {
    if (drawedNodes.indexOf(node) == -1) {
        drawNode(node, x, y);
        drawedNodes.push(node);
    }

    var deps = findAllDepOf(node, flows);
    if (deps.length > 0) {
        $.each(deps, function (i, e) {

            findAllDepsAndDraw(e, x + xoffset, y + i * yoffset, flows);

        })
    }
}

function drawNode(node, x, y) {
    try {
        $('#taskChart').append('<div class="taskChartItems" id="flow' + node.flow_id + '" style="left:' + parseInt(x) + 'px;top:' + parseInt(y) + 'px">' + node.task_name + "</div>");
        if (x + xoffset > $('#taskChart').width()) { $('#taskChart').width(x + xoffset-50); }
        if (y + yoffset > $('#taskChart').height()) { $('#taskChart').height(y + yoffset); }
    }
    catch (e) { console.log(e.toString()); }
}

function findAllDepOf(node, flows) {
    var deps = Array();
    for (var i = 0; i < flows.length; i++) {
        if (flows[i].dependant_on_id.toString().indexOf(node.flow_id.toString()) > -1) {
            deps.push(flows[i]);
        }
    }
    return deps;
}
