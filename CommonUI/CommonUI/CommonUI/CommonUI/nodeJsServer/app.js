var app = require('express')();
//var server = require('http').Server(app);
var bodyParser = require('body-parser');
var winston = require('winston');
var logger = new (winston.Logger)({
    transports: [
    new (winston.transports.Console)({ json: false, timestamp: true }),
    new winston.transports.File({ filename: __dirname + '/logs/SMPushNotification.log', json: false })
  ],
    exitOnError: false
});
var http = require('http');
var fs = require('fs');
var xml2js = require('xml2js');
var defaultPort = 8888;
var io = null;
app.use(bodyParser.json());
var serverInstance = null;
var environmentType = process.argv;     //Copying the arguments passed to node, while starting the server. Eg. node app.js dev --> app.js is 1st arg and dev is 2nd arg
var config = {};

if (environmentType[2].toLowerCase() === "dev") {
    config.appSettingsLocation = __dirname + "/../../Web.config";
    config.environmentType = "DEV"
}
else {
    config.appSettingsLocation = __dirname + "/../../ConfigFiles/appSettings.config";
    config.environmentType = "DEPLOY"
}

//Read Port Number from appSettings.config
fs.readFile(config.appSettingsLocation, function (err, data) {
    var parser = new xml2js.Parser();
    var port = defaultPort;
    parser.parseString(data, function (err, result) {
        logger.info("AppSettings Parsing Start");
        if (config.environmentType === "DEV") {
            for (var i = 0; i < result.configuration.appSettings[0].add.length; i++) {
                var ele = result.configuration.appSettings[0].add[i];
                if (ele['$'].key === 'NodeJsURl') {
                    port = ele['$'].value;
                    break;
                }
            }
        }
        else {
            for (var i = 0; i < result.appSettings.add.length; i++) {
                var ele = result.appSettings.add[i];
                if (ele['$'].key === 'NodeJsURl') {
                    port = ele['$'].value;
                    break;
                }
            }
        }
        logger.info("AppSettings Parsing End");
        var p = port.split(':')[2];
        var appPort = parseInt(p);
        var appUrl = port.substring(0, ((port.length - p.length) - 1));
        logger.info("AppSettings Port : " + appPort);

        // Node Server Created
        serverInstance = http.createServer(app).listen(appPort);

        //"io" linked to the Node Server created
        io = require('socket.io')(serverInstance)

        logger.info("SecMaster Post Notification Server Started at port : " + appPort);
        InitializeServer(serverInstance);
    });
});

    //serverInstance = http.createServer(app).listen(defaultPort);
    //io = require('socket.io')(serverInstance)
    //InitializeServer(serverInstance);

function InitializeServer(serverInstance) {
    var changeSectype = require("./changeSectype");
    var notifyStatus = require('./notifyStatus');
    var notifyApiCall = require('./notifyApiCall');
    var connArray = [];

    app.get('/TestConnection', function (req, res) {
        res.send("SecMaster Node JS Server running");
    });

    app.get('/PostNotification', function (req, res) {
        logger.info("Connection Array Length:" + connArray.length)
        logger.info("Post Notification Request Came: " + req.query.id);
        changeSectype.changeSectypeNotification(connArray, req, logger);
        logger.info("Post Notification - Request Completed");
        res.send("Post Notification - Request Completed");
    });

    app.get('/CloseConnection', function (req, res) {
        logger.info("Close Connection Request Came: " + req.query.id);
        changeSectype.closeConnection(connArray, req);
        logger.info("Close Connection - Request Completed");
        res.send("Close Connection - Request Completed");
    });

    app.get('/RemoveConnection', function (req, res) {
        logger.info("Remove Connection Request Came: " + req.query.id);
        changeSectype.removeConnection(connArray, req);
        logger.info("Remove Connection - Request Completed");
        res.send("Remove Connection - Request Completed");
    });

    app.post('/CUPostToDownstreamStatus', function (req, res) {
        logger.info("Post To Downstream Status Request Came: " + req.body.id);
        notifyStatus.notify(connArray, req, io, logger);
        logger.info("Post To Downstream Status Request Completed");
        res.send("Post To Downstream Status Request Completed");
    });

    app.post('/ChatApplication', function (req, res) {
        //logger.info("Chat Request Came: " + req.body.data.socketId);
        notifyStatus.chatnotify(req.body.data.name, req.body.data.socketId, connArray);
        //logger.info("Chat Request Completed");
        res.send(req.body.data.name);
    });


    //ApiMonitoring BEGIN

    //Function for DB to call and send Data to Node Server
    app.post('/ApiCallDataPostToNodeServer', function (req, res) {
        //console.log(req.body);
        logger.info("API Call Post To Clients Request Came." );
        notifyApiCall.notify(req, io, connArray);
        logger.info("API Call Post To Clients Request Completed");
        res.send("API Call Post To Clients Request Completed");
    });
    
    //ApiMonitoring END   


    io.on('connection', function (socket) {
        try {
            logger.info("Connection Established: " + socket.id);
            var socketId = socket.id;
            var tempObj = {};
            tempObj[socketId] = socket;
            connArray.push(tempObj);
            
            /*
            socket.on('end', function () {
                socket.disconnect(0);
                logger.info("Disconnected from : " + socket.id);

            });
            */
            socket.on('disconnect', function () {
                //socket.disconnect(0);
                notifyApiCall.removeConnection(connArray, socket.id);
                logger.info("Disconnected from : " + socket.id);
                
            });

        }
        catch (ex) {
            logger.info("Exception Occured in Socket.io Connection: " + ex);
        }
    });

    
    
};