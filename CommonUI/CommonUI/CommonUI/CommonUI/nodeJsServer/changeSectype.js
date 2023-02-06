exports.changeSectypeNotification = function(connArray,req, logger){
    try{
        logger.info("Clone Multiple Response data: " + req.query.msg);
        for(var i = 0; i < connArray.length; i++){
            logger.info("Clone Multiple Response data: " + req.query.msg);
            for(var obj in connArray[i]){
                if(obj === req.query.id){
                    var soc = connArray[i][obj];
                    soc.emit("cloneMultipleNotification",{ data : req.query.msg });
                    logger.info("Clone Multiple Response data: " + req.query.msg);
                    return;
                }
            }
        }
    }
    catch(ex){
        console.log(ex);
    }
};

exports.closeConnection = function(connArray,req){
    try{
        for(var i = 0; i < connArray.length; i++){
            for(var obj in connArray[i]){
                if(obj === req.query.id){
                    var soc = connArray[i][obj];
                    soc.emit("cloneMultipleNotificationCompleted",{ data : req.query.id });
                    return;
                }
            }
        }
    }
    catch(ex){
        console.log(ex);
    }
};

exports.removeConnection = function (connArray, req) {
    try{
        for (var i = 0; i < connArray.length; i++) {
            for (var obj in connArray[i]) {
                if (obj === req.query.id) {
                    var soc = connArray[i][obj];
                    soc.emit("connectionRemoved", { data: req.query.id });
                    connArray.splice(i, 1);
                    return;
                }
            }
        }
    }
    catch(ex){
        console.log(ex);
    }
};