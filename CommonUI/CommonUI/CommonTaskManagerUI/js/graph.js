function Edge(v1, v2) {
    this.v1 = v1;
    this.v2 = v2;
}

function Graph() {
    this.V = {};
    this.E = [];
}

Graph.prototype.addEdge = function (v2, v1) {
    weight = 1;
    if (!this.V[v1.toString()])
        this.V[v1.toString()] = {};
    if (!this.V[v2.toString()])
        this.V[v2.toString()] = {};
    this.V[v1.toString()][v2.toString()] = weight;
    this.E.push(new Edge(v1.toString(), v2.toString()))
}

Graph.prototype.explore = function (v, proc, prefunc, postfunc) {
    visited = {}
    graph = this
    function helper(v) {
        prefunc(v)
        visited[v] = true;
        proc(v)
        $.each(graph.V[v], function (k, v) {
            if (!visited[k])
                helper(k);
        })
        postfunc(v)
    }
    helper(v)
}

var pre = {};
var post = {};
var ccn = 1;
function previsit(v) {
    pre[v] = ccn;
    ccn++;
}
function postvisit(v) {
    post[v] = ccn;
    ccn++;
}
function action(v) {
   
}
Array.prototype.getUnique = function () {
    var u = {}, a = [];
    for (var i = 0, l = this.length; i < l; ++i) {
        if (u.hasOwnProperty(this[i])) {
            continue;
        }
        a.push(this[i]);
        u[this[i]] = 1;
    }
    return a;
}

function findDeadlock(g) {
    var deadlockAt = Array();
    $.each(g.V, function (i, e) {
        for (var i in e) {
            g.explore(i.toString(), action, previsit, postvisit);
            $.each(g.E, function (i, e) {
                v1 = e.v1;
                v2 = e.v2;
                if (pre[v1] > pre[v2] && post[v2] > post[v1]) {
                    deadlockAt.push({vert1: v1 ,vert2: v2});
                }
            })
        }
    });
    pre = {};
    post = {};
    ccn = 1;
    return deadlockAt.getUnique();
}