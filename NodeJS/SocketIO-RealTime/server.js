var io = require("socket.io")(5055);

console.log("socket listen at port 5055");

var playerIDList = [];

io.on("connection", (socket)=>{

    console.log("client connected : "+socket.id);

    ClientConnect(io, socket);

    ClientFetchPlayerList(socket);

    socket.on("disconnect", ()=>{

        console.log("client disconnected : "+socket.id);

        ClientDisconnect(io, socket);
    });
});

var ClientConnect = (io, socket)=>{

    var data = {
        "uid":socket.id
    };

    playerIDList.push(data.uid);

    CountPlayer();

    socket.emit("OnOwnerClientConnect", data);
    io.emit("OnClientConnect", data);
}

var ClientDisconnect = (io, socket)=>{

    var data = {
        "uid":socket.id
    };

    for(var i = 0; i < playerIDList.length; i++)
    {
        if(playerIDList[i] == data.uid)
        {
            playerIDList.splice(i, 1);
            console.log("delete player : " + data.uid);
        }
    }

    CountPlayer();

    io.emit("OnClientDisconnect", data);
}

var ClientFetchPlayerList = (socket)=>{

    socket.on("OnClientFetchPlayerList", ()=>{

        data = {
            "playerIDList":playerIDList
        };
    
        socket.emit("OnClientFetchPlayerList", data);
    });
} 

var CountPlayer = ()=>{
    console.log("Player total : "+ playerIDList.length);
}
