var srmModeler = (function () {
    var srmModeler;
    this._controlIdInfo = null;
    String.prototype.replaceAll = function (find, replace) {
        return this.replace(new RegExp(find, 'g'), replace);
    }
    function SRMModeler() {
    }
    srmModeler = new SRMModeler();
    SRMModeler.prototype.myFunction1 = function myFunction1() {
        var popup = document.getElementById("myPopup");
        popup.classList.toggle("show");

    }
    var html = "";
    var t = 0;
    var nodes = [];

    SRMModeler.prototype.alertMe = function alertMe() {
        $('.SRMDetailsView').css('display', 'block');
        $('#SRMDetailsIframe').attr('src', pathXyz + "/SRMDataSourceSystemMapping.aspx?selectedTab=attributes&EntityTypeId=661&Module=20");
        $('#SRMDetailsIframe').css('width', $(window).width() - 20);
        $('#SRMDetailsIframe').css('height', $(window).height() - 20);
    }
    SRMModeler.prototype.alertMe2 = function alertMe2(e) {
        $('.SRMDetailsView').css('display', 'block');
        $('#SRMDetailsIframe').attr('src', pathXyz + "/SRMDataSourceSystemMapping.aspx?selectedTab=mapping");
        $('#SRMDetailsIframe').css('width', $(window).width() - 20);
        $('#SRMDetailsIframe').css('height', $(window).height() - 20);
    }

    SRMModeler.prototype.doIt = function doIt() {
        $('.button1').unbind('click').on('click', function (e) {
            html = document.getElementById("test").innerHTML;
            var add = "<div class=\"item\" id=\"node" + (t++) + "\" style=\"height:60px; width:196px; left:608px; top:40px;  border-radius:5px;  z-index:1;  text-align:center; background-color:#F8F8F8;\">";
            add = add + "<div style=\"width:100%; margin-left:0px; height:0%; padding-top:4px; background-color:#538bae; color:white;border-radius:5px;  \"></div><div style=\"text-align:center;height:30px;padding-top: 5px;padding-left:10px;font-family:oswald;color:#538bae; font-size:13px;\">" + e.target.innerHTML + "</div>";

            var first1 = -25;

            var my = 1;
            var fillVar1 = 0;
            var info1 = [{ colNames: ["D", "U", "R"], colFillOrNO: ["yes", "no", "no"], colColors: ["#7eafbd", "#9195bc", "#b9af92"] }];
            for (var f = 0; f < info1[0].colNames.length; f++) {
                var prop1;

                if (info1[0].colFillOrNO[fillVar1] == "yes") {
                    prop1 = "background-color:" + info1[0].colColors[fillVar1++] + "; color:white;  position:absolute; border: 1px solid " + info1[0].colColors[(fillVar1 - 1)] + ";";
                }
                else {
                    prop1 = "border: 1px solid " + info1[0].colColors[fillVar1++] + ";  color:" + info1[0].colColors[(fillVar1 - 1)] + ";"
                }

                if (f == 0) {

                    add = add + "<div class=\"childDiv\" style=\" margin-left:" + first1 + "px;position:absolute; margin-top:4px; " + prop1 + "  \" >" + info1[0].colNames[f] + "</div>";
                } else {
                    if (f == 1) {
                        first1 = -3;
                    } else {
                        first1 = 18;
                    }
                    add = add + "<div class=\"childDiv\" style=\" margin-left:" + (first1) + "px;position:absolute; margin-top:4px; " + prop1 + "  \" >" + info1[0].colNames[f] + "</div>";
                    my++;
                }

            }

            html = html.substring(0, html.length - 6).concat(add).concat("</div>");

            document.getElementById("test").innerHTML = html;
            var r = "node" + (t - 1);
            nodes.push(r)

            for (var g = 0; g < nodes.length; g++) {
                jsPlumb.draggable(nodes[g]);
            }
            jsPlumb.bind("beforeDrop", function (info) {
                alert(info.connection.endpoints[0].getUuid());
                return true; /* if you need to establish connection */
            });
            var common = {
                isSource: true,
                isTarget: true,
                anchor: "Continuous",
                //connector: "Straight",
                connector: ["Bezier", { curviness: 10 }],
                endpoint: ['Rectangle', { width: 10, height: 10, zIndex: 1 }],
                paintStyle: { fill: "white", outlineStroke: "#FD9759", strokeWidth: 3 },
                hoverPaintStyle: { outlineStroke: "light#FD9759" },
                connectorStyle: { outlineStroke: "green", strokeWidth: 1 },
                connectorHoverStyle: { strokeWidth: 2 },
                endpointStyles: [
               { fill: "lightgray", outlineStroke: "lightgray", outlineWidth: 1  }
//{ fill: "green" }
                ]
                // connector: ["Flowchart"],
                ,deleteEndpointsOnDetach: false,
                dragAllowedWhenFull: false
            };

            var endpointOptions = {
                isTarget: true,
                createEndpoint: true,
                endpoint: "Rectangle",
                paintStyle: { fill: "gray" }
            };

            var endpointOptions1 = {
                isSource: true,
                isTarget: true,
                createEndpoint: true,

                //connector: ["Flowchart", { midpoint: 0, gap: [5, 6] }],
                //connector: ["Straight"],
                connector: ["Bezier", { curviness: 40 }],
                //connector: ["Flowchart", { midpoint: 0, gap: [5, 6] }],
                //connectorStyle: { stroke: "#8c7668", strokeWidth: 2 },
                paintStyle: { stroke: "#ececec", strokeWidth: 2 },
                //connectorStyle: { strokeStyle: 'Red', lineWidth: 12, dashstyle: "2 1 2 1" },
                //anchors: [["Bottom", { shape: "Circle" }], ["Top", { shape: "Circle" }]],
                anchor: "Continuous",
                //endpoints: ["Blank", "Blank"],
                endpoint: ['Rectangle', { width: 2, height: 2, zIndex: -1 }],
                //paintStyle: { fill: "gray" },
                //paintStyle: { stroke: "#8c7668", strokeWidth: 2, dashstyle: "5 5 5 5", fill: "gray" },
                maxConnections: 8
                //connector: ["Flowchart", { midpoint: 0, gap: [5, 6] }],
                //connector: "Straight",
                //connectorOverlays: [["Arrow", { location: 0.5, zIndex: -1 }]]
       , connectorOverlays: [["Arrow", { width: 5, length: 5, location: 0.67, zIndex: -1 }]],
         endpointStyles: [
          { fill: "lightgray", outlineStroke: "lightgray", outlineWidth: 1  }

       ]

            };
            //var endpointOptions1 = {
            //    isSource: true,
            //    isTarget: true,
            //    createEndpoint: true,
            //    //endpoint: ["Rectangle", { width: 13, height: 13, zIndex: -1 }],
            //    //                   paintStyle: { fill: "gray" },
            //    maxConnections: 8,
            //    //                 connector: "Straight",
            //    //connectorOverlays: [["Arrow", { location: 0.5, zIndex: -1 }]]
            //    //    connectorOverlays: [["Arrow", { width: 12, length: 12, location: 0.67, zIndex: -1 }]],



            //jsPlumb.makeTarget("node9", endpointOptions1);
            //jsPlumb.makeSource("node9", endpointOptions1);
            jsPlumb.makeSource(r, endpointOptions1);
            jsPlumb.makeTarget(r, endpointOptions1);
            //jsPlumb.makeSource("node7", endpointOptions1);
            //jsPlumb.makeTarget("node7", endpointOptions1);
            //jsPlumb.makeSource("node6", endpointOptions1);
            //jsPlumb.makeTarget("node6", endpointOptions1);

        });
    }
    jsPlumb.ready(function () {

       
        var info1 = [{ type: "rect", isParent: "no", len: 116, height: 122, locx: 25, locy: 24, fill: "yes", color: "#3ca8b9", text: "hello", heading: "Party Ownership Structure", colNames: ["D", "U", "R"], colFillOrNO: ["yes", "No", "yes"], colColors: ["#7eafbd", "#9195bc", "#b9af92"], colValues: [10, 20, 30] }, { type: "NewLeg", isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yes"], len: 116, height: 122, locx: 943, locy: 40, color: "#3ca8b9", text: "hello", fill: "yes", heading: "Party Status", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { type: "NewLeg2", isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yess", "yes", "yes"], len: 116, height: 122, locx: 37, locy: 654, color: "#3ca8b9", text: "hello", heading: "Job Role", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { type: "NewLeg2", isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yes"], len: 116, height: 122, locx: 359, locy: 205, color: "#3ca8b9", text: "hello", heading: "Party Relationship", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { type: "NewLeg2", isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yes"], len: 116, height: 122, locx: 24, locy: 122, color: "#3ca8b9", text: "hello", heading: "Relationship Type", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { type: "rect", isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yesS"], len: 116, height: 122, locx: 609, locy: 192, color: "#3ca8b9", text: "hello", heading: "Party", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { type: "NewLeg2", isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yes"], len: 116, height: 122, locx: 28, locy: 392, color: "#3ca8b9", text: "hello", heading: "Department", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { type: "NewLeg", isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yess", "yes", "yes"], len: 116, height: 122, locx: 25, locy: 262, color: "#73AE42", text: "hello", heading: "Agreement", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { type: "NewLeg", isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yes"], len: 116, height: 122, locx: 937, locy: 243, color: "#b1846f", text: "hello", heading: "Party Alternate Identifies", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { type: "NewLeg", isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yes"], len: 116, height: 122, locx: 941, locy: 137, color: "#669a85", text: "hello", heading: "Party Bussiness Risk Rating", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { type: "NewLeg", isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yess"], len: 116, height: 122, locx: 418, locy: 554, color: "#669a85", text: "hello", heading: "Issued Capital", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { type: "NewLeg2", isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yess", "No", "yes"], len: 116, height: 122, locx: 327, locy: 630, color: "#669a85", text: "hello", heading: "Party Credit Rating", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { type: "NewLeg2", isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yes"], len: 116, height: 122, locx: 1045, locy: 689, color: "#669a85", text: "hello", heading: "Rating Type", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { type: "rect", isParent: "yes", subModuleId: 1, colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yes"], len: 513, height: 122, locx: 310, locy: 377, color: "#669a85", text: "hello", heading: "Department14", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { type: "NewLeg2", isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yes"], len: 116, height: 122, locx: 1266, locy: 148, color: "#669a85", text: "hello", heading: "ID Type", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { type: "NewLeg", isParent: "no", subModuleId: 1, colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yes"], len: 513, height: 122, locx: 616, locy: 691, color: "#669a85", text: "hello", heading: "Party Credit Rating Outlook", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { type: "NewLeg", isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yes"], len: 116, height: 122, locx: 894, locy: 557, color: "#669a85", text: "hello", heading: "Trading Hours", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { type: "NewLeg", isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yes"], len: 116, height: 122, locx: 1150, locy: 577, color: "#669a85", text: "hello", heading: "Trading Venue Non-working Days", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }];
        var subModuleInfo = [{ subModuleId: 1, subModuleList: [{ isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yes"], len: 116, height: 122, locx: 10, locy: 10, color: "#669a85", text: "hello", heading: "Issuer", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yes"], len: 116, height: 122, locx: 10, locy: 10, color: "#669a85", text: "hello", heading: "Broker", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yes"], len: 116, height: 122, locx: 10, locy: 10, color: "#669a85", text: "hello", heading: "Counter Party", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yes"], len: 116, height: 122, locx: 10, locy: 10, color: "#669a85", text: "hello", heading: "Trading Venue", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yes"], len: 116, height: 122, locx: 420, locy: 10, color: "#669a85", text: "hello", heading: "Client", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }, { isParent: "no", colColors: ["#7eafbd", "#9195bc", "#b9af92"], colFillOrNO: ["yes", "No", "yes"], len: 116, height: 122, locx: 220, locy: 10, color: "#669a85", text: "hello", heading: "Rating Provider", colNames: ["D", "U", "R"], colValues: [10, 20, 30] }] }];
        var linkInfo1 = [{ node1: "node5", node2: "node11" }, { node1: "node5", node2: "node1" }, { node1: "node5", node2: "node9" }, { node1: "node5", node2: "node8" }, { node1: "node8", node2: "node14" }, { node1: "node5", node2: "node6" }, { node1: "node12", node2: "node11" }, { node1: "node6", node2: "node2" }, { node1: "node3", node2: "node7" }, { node1: "node3", node2: "node4" }, { node1: "node5", node2: "node0" }, { node1: "node11", node2: "node15" }, { node1: "node6", node2: "node3" }, { node1: "nodee0", node2: "node10" }, { node1: "nodee3", node2: "node17" }, { node1: "nodee3", node2: "node16" }];
        var info2 = { dataInfo: info1, linkInfo: linkInfo1 };
        var info = info2.dataInfo;
        var link = info2.linkInfo;
        html = "<div id=\"diagramContainer\" style=\"backgound-color:#F8F7F7\">";
        for (var i = 0; i < info.length; i++) {
            if (!(info[i].type == "circle1")) {
                if (!(info[i].isParent == "yes")) {
                    html = html + "<div class=\"item jtk-endpoint jtk-endpoint-anchor jtk-draggable jtk-droppable  \" id=\"node" + (t++) + "\" style=\"align:center; width:" + 196 + "px; border-radius:5px;  z-index:1;  text-align:center; background-color:#F8F8F8; height:" + 60 + "px; left:" + info[i].locx + "px; top:" + info[i].locy + "px;\" ></div>";
                }
                else {
                    var len = subModuleInfo[info[i].subModuleId - 1].subModuleList.length;
                    var width = 196 * len + 10 * (len + 1);
                    html = html + "<div class=\"item jtk-endpoint jtk-endpoint-anchor jtk-draggable jtk-droppable  \" id=\"node" + (t++) + "\" style=\"width:" + width + "px; border-radius:5px;  z-index:1;  text-align:center; background-color:#ececec; border: solid 1px #6ba7da; height:" + (60 + 40) + "px; left:" + info[i].locx + "px; top:" + info[i].locy + "px;\" ></div>";
                }
            } else {


                html = html + "<div class=\"item dot jtk-endpoint jtk-endpoint-anchor jtk-draggable jtk-droppable  \" id=\"node" + (t++) + "\" style=  \"z-index:1; background-color:#F3F9FA; text-align:center;   left:" + info[i].locx + "px; top:" + info[i].locy + "px;\" ></div>";


            }
            var r = "node" + (t - 1);
            nodes.push(r);

        }


        html = html + "</div>";
        var temp = "";
        document.getElementById("test").innerHTML = html;
        for (var k = (t - 1) ; k >= 0; k--) {

            if ((!(info[k].isParent == "yes"))) {
                if (info[k].type != "circle1") {
                    var col = "#538bae";
                    var txtCol = "#538bae";
                    if (info[k].type == "NewLeg") {
                        col = "#9c938a";
                        txtCol = "#534b43";
                    } else if (info[k].type == "NewLeg2") {
                        col = "green";
                        txtCol = "#534b43";
                    }
                    temp = "<div style=\"width:100%; margin-left:0%; height:0%; padding-top:3px;padding-bottom:3px; background-color:" + col + "; color:white;font-family:oswald; border-radius:5px; \" ></div><div onclick=\"srmModeler.alertMe()\" style=\"text-align:center;height:30px;padding-top: 2px;padding-left:10px;font-family:oswald;color:" + txtCol + "; font-size:13px;\" >" + info[k].heading + "</div>";
                } else {
                    temp = "<div style=\"width:85%; margin-left:5%; height:18%; padding-top:3px;padding-bottom:3px; background-color:#538bae; color:white;font-family:oswald; top:60px; position:absolute; \"  onclick=\"alertMe()\"></div>";
                }
            } else {
                createInternalNode(k, k);
            }
            var ty = 20;
            var first = 28;
            var second = 69;
            var third = 120;
            var m = 1;
            var fillVar = 0;
            //var wid = -88;
            if ((info[k].isParent != "yes")) {
                for (var f = 0; f < info[k].colNames.length; f++) {
                    var prop;

                    if (info[k].colFillOrNO[fillVar] == "yes") {
                        prop = "background-color:" + info[k].colColors[fillVar++] + "; color:white; border: 1px solid " + info[k].colColors[(fillVar - 1)] + ";";
                    }
                    else {
                        prop = "border: 1px solid " + info[k].colColors[fillVar++] + ";  color:" + info[k].colColors[(fillVar - 1)] + ";"
                    }
                    if (f == 0) {

                        if (info[k].type != "circle1") {
                            temp = temp + "<div class=\"childDiv\"  onclick=\"srmModeler.alertMe2()\"  style=\" " + prop + "  \">" + info[k].colNames[f] + "</div>";
                        } else {
                            temp = temp + "<div class=\"childDiv\"  onclick=\"srmModeler.alertMe2()\"  style=\" margin-top:118px;" + prop + "   \" >" + info[k].colNames[f] + "</div>";
                        }
                    } else {
                        if (info[k].type != "circle1") {
                            //wid = wid + 23;
                            temp = temp + "<div class=\"childDiv\"   onclick=\"srmModeler.alertMe2()\"  style=\" " + prop + " \" >" + info[k].colNames[f] + "</div>";
                            m++;
                        } else {
                            temp = temp + "<div class=\"childDiv\"   onclick=\"srmModeler.alertMe2()\"  style=\" margin-top:118px;" + prop + "  \" >" + info[k].colNames[f] + "</div>";
                            m++;
                        }
                    }
                }
            }
            if (!(info[k].isParent == "yes")) {
                document.getElementById("node" + (k)).innerHTML = temp;
            }
        }
        var data = [];
        for (var j = 0; j < link.length; j++) {
            linkConnect(link[j].node1, link[j].node2);
        }

        function createInternalNode(i, k) {
            var sub = subModuleInfo[info[i].subModuleId - 1].subModuleList;
            var html = "";
            var spaceBetweenNodes = 10;
            for (var loop1 = 0; loop1 < sub.length; loop1++) {

                html = html + "<div class=\"item jtk-endpoint jtk-endpoint-anchor jtk-draggable jtk-droppable  \" id=\"nodee" + (loop1) + "\" style=\"width:" + 196 + "px; border-radius:5px;  z-index:1;  text-align:center; background-color:#F8F8F8; height:" + 60 + "px; left:" + spaceBetweenNodes + "px; top:" + (sub[loop1].locy + 10) + "px;\" ></div>";

                spaceBetweenNodes = spaceBetweenNodes + 206;

            }

            document.getElementById("node" + (k)).innerHTML = html;
            for (var loop = 0; loop < sub.length; loop++) {
                //var wid = -88;
                var temp = "";

                temp = "<div style=\"width:100%; margin-left:0%; height:0%; padding-top:3px;padding-bottom:3px; background-color:#538bae; color:white;font-family:oswald; border-radius:5px; \"  ></div><div onclick=\"srmModeler.alertMe()\" style=\"text-align:center;height:30px;padding-top: 5px;padding-left:10px;font-family:oswald;color:#538bae; font-size:13px;\">" + sub[loop].heading + "</div>";

                var ty = 20;
                var first = 28;
                var second = 69;
                var third = 120;
                var m = 1;
                var fillVar = 0;
                for (var f = 0; f < sub[loop].colNames.length; f++) {
                    var prop;

                    if (sub[loop].colFillOrNO[fillVar] == "yes") {
                        prop = "background-color:" + sub[loop].colColors[fillVar++] + "; color:white; border: 1px solid " + sub[loop].colColors[(fillVar - 1)] + ";";
                    }
                    else {
                        prop = "border: 1px solid " + sub[loop].colColors[fillVar++] + ";  color:" + sub[loop].colColors[(fillVar - 1)] + ";"
                    }
                    if (f == 0) {
                        temp = temp + "<div class=\"childDiv\"  onclick=\"srmModeler.alertMe2()\"  style=\" " + prop + "  \" >" + sub[loop].colNames[f] + "</div>";
                    } else {
                        //wid = wid + 23;
                        temp = temp + "<div class=\"childDiv\"   onclick=\"srmModeler.alertMe2()\"  style=\" " + prop + "  \" >" + sub[loop].colNames[f] + "</div>";
                        m++;
                    }
                }

                document.getElementById("nodee" + (loop)).innerHTML = temp;

            }

        }

        var endPointOptionForJson = {
            isSource: true,
            isTarget: true,
            createEndpoint: true,
            //connector: ["Flowchart", { midpoint: 0, gap: [5, 6] }],
            //connector: ["Straight"],
            //connector: ["Bezier", { curviness: 40 }],
            connector: ["Flowchart", { midpoint: 0, gap: [5, 6] }],
            endpoint: ['Rectangle', { width: 2, height: 2, zIndex: -1 }],
            connectorStyle: { stroke: "#ececec", strokeWidth: 2 },
            //connectorStyle: { strokeStyle: 'Red', lineWidth: 12, dashstyle: "2 1 2 1" },
            //anchors: [["Bottom", { shape: "Circle" }], ["Top", { shape: "Circle" }]],
            anchor: "Continuous",
            paintStyle: { stroke: "#ececec", strokeWidth: 2 },
            endpointStyles: [
      { fill: "lightgray", outlineStroke: "lightgray", outlineWidth: 1  }
      //{ fill: "green" }
            ],
            //endpoints: ["Blank", "Blank"],
           
            //paintStyle: { fill: "gray" },
            //paintStyle: { stroke: "#8c7668", strokeWidth: 2, dashstyle: "5 5 5 5", fill: "gray" },
            maxConnections: 8
            //connector: ["Flowchart", { midpoint: 0, gap: [5, 6] }],
            //connector: "Straight",
            //connectorOverlays: [["Arrow", { location: 0.5, zIndex: -1 }]]
       , connectorOverlays: [["Arrow", { width: 5, length: 5, location: 0.67, zIndex: -1 }]],

        };

        function linkConnect(node1, node2) {


            jsPlumb.connect({
                source: node1,
                target: node2,
                maxConnections: 8,
                endpoint: ['Rectangle', { width: 2, height: 2, zIndex: -1 }],
                endpointStyles: [
       { fill: "lightgray", outlineStroke: "lightgray", outlineWidth: 1 }
       
                ],
                anchor: "Continuous",
                paintStyle: { stroke: "#ececec", strokeWidth: 2 },
                //paintStyle: { strokeStyle: 'Red', fillStyle: "Red" },
                //connectorStyle: { strokeStyle: 'Red', lineWidth: 12, dashstyle: "2 1 2 1" },
                //connector: ["Straight"],
                connector: ["Flowchart", { midpoint: 0, gap: [5, 6] }],
              //  paintStyle: { fill: "white", outlineStroke: "#FD9759", strokeWidth: 3 },
                //connector: ["Bezier", { curviness: 40 }],
                overlays: [
                                ["Arrow", { width: 10, length: 10, location: 0.67, zIndex: -1 }]
                ]


            });
        }
        for (var g = 0; g < nodes.length; g++) {
            jsPlumb.makeSource(nodes[g], endPointOptionForJson);

            jsPlumb.makeTarget(nodes[g], endPointOptionForJson);

            jsPlumb.draggable(nodes[g]);
        }




    });

    return srmModeler;
})();

var pathXyz = "";

$(document).ready(function () {
    $("#SRMcloseButton").off('click').on('click', function () {
        $("#SRMDetailsView").css('display', 'none');
        $("#SRMcloseButton").attr('src', ""); 
    });
    var path = window.location.protocol + '//' + window.location.host;
    var pathname = window.location.pathname.split('/');

    $.each(pathname, function (ii, ee) {
        if ((ii !== 0) && (ii !== pathname.length - 1))
            path = path + '/' + ee;
    });

    pathXyz = path;
})
