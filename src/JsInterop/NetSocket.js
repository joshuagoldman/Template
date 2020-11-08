import * as net from 'net';

export function createSocket(ip, port) {
    try{
        var client = new net.Socket();
        client.connect(port, ip, function() {
            console.log('Connected');
        });

        return {ErrorMessage: null, Socket: socket};
    }
    catch(error){
        return {ErrorMessage: error.message, Socket: null};
    }
}

export function addEventListener(handler, socketObj) {
    try{
 
        client.on('data', function(data) {
            handler(data);
        });
        return socketObj;
    }
    catch{
        return null;
    }
}

export function emit(message, socketObj){
    try{
        socketObj.write(message);

        return socketObj;
    }
    catch(error){
        return error.message;
    }
};