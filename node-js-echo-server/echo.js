#!/usr/local/bin/node
var conn = [];
var WebSocketServer = require('websocket').server;
var http = require('http');
var originIsAllowed = function(){return true;}
var server = http.createServer(function(request, response) {
        console.log((new Date()) + " Received request for " + request.url);
        response.writeHead(404);
        response.end();
});

server.listen(10888, function() {
        console.log((new Date()) + "Echo Server is listening on port 10888");
});

wsServer = new WebSocketServer({
        httpServer: server,
    autoAcceptConnections: false
});

wsServer.on('close', function(request) {
        console.log("Closed");
});

wsServer.on('request', function(request) {
        if (!originIsAllowed(request.origin)) {
                request.reject();
                console.log((new Date()) + " Connection from origin " + request.origin + " rejected.");
                return;
        }
        console.log("Connected");
        var con = request.accept(null, request.origin)
        conn.push(con);
        con.on('message', function(mg) {
                console.log(mg.utf8Data);
                if(mg.utf8Data.search("all:") == 0){
                	for(var c = 0;c < conn.length;c++){
                        	if(!conn[c].closed){
                                	conn[c].sendUTF("Somebody cried \""+mg.utf8Data.substring(4)+"\".");
                        	}
                	}
                }else{
                        con.sendUTF("Server said \""+mg.utf8Data+"\" too.");
		}
        });
});