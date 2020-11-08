
const express = require('express');
const bodyParser = require('body-parser');
var cors = require('cors');
const PORT = process.env.PORT || 3001;
var socket  = require('socket.io');
var shell = require('shelljs');
var net = require('net')
var regex = require('../src/JsInterop/Regex')

const app = express();
app.use(cors())
app.use(express.json());
app.use(express.urlencoded(({extended:true})));
app.use(bodyParser.json());
app.use(fileUpload());
app.use('/files', express.static(__dirname + '/../public'))
app.use(bodyParser.json({limit: '900mb'}));
app.use(bodyParser.urlencoded({limit: '900mb', extended: true}));

var server = app.listen(PORT, () => {
    console.log( `Server listening on port ${PORT}...`);
});

net.createServer(socket => {
    socket.on('data', data => {
        let msg = data.toString()

        console.log(msg);
    })
})

net.listen(300);

var io = socket(server);

io.sockets.on(`connection`, (sckt) => {
    sckt.on('message',(msgObj) =>{
        sckt.broadcast.emit('message', msgObj);
    })

    sckt.on('finished',(msgObj) =>{
        sckt.broadcast.emit('finished', msgObj);
    })
});

// --------------------------------------------------------------------------------------------------------------
// Shell commands
// --------------------------------------------------------------------------------------------------------------
app.post("/shellcommand", (req, res) => {
    var commandsAsString = req.body.shellCommand;

    console.log(commandsAsString);
    let commands = commandsAsString.split(";");

    let responses = new Array();
    let errors = new Array();

    function decideFaith(ans){
        if(ans.code !== 0){
            errors.push(ans)
        } 
        else{
            responses.push(ans.stdout); 
        } 
    }

    commands.forEach(command => {
        let ans = "";
        if(command.includes("cd ")){
            if(command === "cd server"){
                ans = shell.cd(__dirname.replace(/\\/g,"/") + '/../public');
            }
            else{
                ans = shell.cd(command.replace("cd ",""));
            }
        }
        else{
            ans = shell.exec(command);
        }

        decideFaith(ans);
   
    });

    if(errors.length != 0){
        let errorsAll = errors.map(e =>
            `shell command status code ${e.code}: ${e.stderr}`
        );
        const errorsCombined = errorsAll.join('\n');
        return res.send(errorsCombined);
    }
    else{
        console.log(responses);
        const responsesCombined = responses.join('\n');
        return res.send(responsesCombined);
    }
});