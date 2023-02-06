exports.notify = function (req, io, connArray) {
    try {
        if (req.body) {
            //Insert
            if (!req.body.data.IsApiCallOver) {
                var IsApiCallOver = req.body.data.IsApiCallOver;
                var ApiUniqueId = req.body.data.ApiUniqueId;
                var ChainName = req.body.data.ChainName;
                var ChainURL = req.body.data.ChainURL;
                var ChainMethod = req.body.data.ChainMethod;
                var ChainClientMachine = req.body.data.ChainClientMachine;
                var ChainClientIP = req.body.data.ChainClientIP;
                var ChainPort = req.body.data.ChainPort;
                var ChainDetailsDataFormatRequest = req.body.data.ChainDetailsDataFormatRequest;
                var ChainStartDateTime = req.body.data.ChainStartDateTime;
                var ChainDetailsData = req.body.data.ChainDetailsData;
                var ThreadId = req.body.data.ThreadId;


                var notificationData = {
                    IsApiCallOver: IsApiCallOver,
                    ApiUniqueId: ApiUniqueId,
                    ChainName: ChainName,
                    ChainURL: ChainURL,
                    ChainMethod: ChainMethod,
                    ChainClientMachine: ChainClientMachine,
                    ChainClientIP: ChainClientIP,
                    ChainPort: ChainPort,
                    ChainDetailsDataFormatRequest: ChainDetailsDataFormatRequest,
                    ChainStartDateTime: ChainStartDateTime,
                    ChainDetailsData: ChainDetailsData,
                    ThreadId: ThreadId
                }
                //console.log(connArray.length);
                io.sockets.emit("ApiCallDataPostToClients", { data: notificationData });
            }
            //Update
            else if(req.body.data.IsApiCallOver)
            {
                var IsApiCallOver = req.body.data.IsApiCallOver;
                var ApiUniqueId = req.body.data.ApiUniqueId;
                var ChainDetailsData = req.body.data.ChainDetailsData;
                var ChainDetailsDataFormatResponse = req.body.data.ChainDetailsDataFormatResponse;
                var ChainEndDateTime = req.body.data.ChainEndDateTime;
                var ChainTimeTaken = req.body.data.ChainTimeTaken;

                var notificationData = {
                    IsApiCallOver: IsApiCallOver,
                    ApiUniqueId: ApiUniqueId,
                    ChainDetailsData: ChainDetailsData,
                    ChainDetailsDataFormatResponse: ChainDetailsDataFormatResponse,
                    ChainEndDateTime: ChainEndDateTime,
                    ChainTimeTaken: ChainTimeTaken
                }
                //console.log(connArray.length);
                io.sockets.emit("ApiCallDataPostToClients", { data: notificationData });
            }
        }
        return;
    }
    catch (ex) {
        console.log(ex);
    }
};


exports.removeConnection = function (connArray, socketId) {
    try {
        for (var i = 0; i < connArray.length; i++) {
            for (var obj in connArray[i]) {
                if (obj === socketId) {
                    connArray.splice(i, 1);
                    return;
                }
            }
        }
    }
    catch (ex) {
        console.log(ex);
    }
};