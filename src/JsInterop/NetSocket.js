import * as io from 'socket.io-client';

export function createSocket(uri) {
    try{

        var socket = io.connect(uri);

        return {ErrorMessage: null, Socket: socket};
    }
    catch(error){
        return {ErrorMessage: error.message, Socket: null};
    }
}

export function addEventListener(handler, socketObj) {
    try{
 
        socketObj.on('data', function(data) {
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