exports.notify = function(connArray,req,io,logger){
    try{
        if(req.body.data.length > 0){
            var securityID = req.body.data[0].securityId;
            var systemName = req.body.data[0].systemName;
            var systemId = req.body.data[0].systemId;
            var postStatus = req.body.data[0].postStatus;
            var postStatusMessage = req.body.data[0].postStatusMessage;
            var lastModifiedOn = req.body.data[0].lastModifiedOn;
            var lastModifiedBy = req.body.data[0].lastModifiedBy;

            var notiifcationData = {
                securityID: securityID,
                systemName:systemName,
                systemId:systemId,
                postStatus:postStatus,
                postStatusMessage:postStatusMessage,
                lastModifiedOn:lastModifiedOn,
                lastModifiedBy:lastModifiedBy
            }
            
            io.sockets.emit("CUPostToDownstreamStatus",{ data : notiifcationData });
        }
        return;
    }
    catch(ex){
        console.log(ex);
    }
};
exports.chatnotify = function(msg,socketId, connArray){
   
    try{
		for(var i = 0; i < connArray.length; i++){
           for(var obj in connArray[i]){
				if(obj === socketId){
					var soc = connArray[i][obj];
						console.log("Notify: " + msg)
						soc.broadcast.emit("ChatApplication",{ data : msg });
					return;
				}
			}
		}
	}
	catch(ex){
		console.log(ex);
	}
};