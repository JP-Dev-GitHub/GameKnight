var Discord = require('discord.io');
var logger = require('winston');
var auth = require('./auth.json');
var fetch = require("node-fetch");
// Configure logger settings
logger.remove(logger.transports.Console);
logger.add(new logger.transports.Console, {
    colorize: true
});
logger.level = 'debug';
//var loc = window.location.pathname;
var dir = __dirname.substring(0, __dirname.lastIndexOf('/'));
//var state = readStateData('file://' + dir + '//config.ini');
console.log('7')
// var data = fetch('http://localhost/config.ini')
//   .then(response => response.text())
//   .then((data) => {
//     console.log(data)
//   })

fetch('http://localhost/config.ini')
.then(res => res.text())
.then(body => console.log(body));

 switch(data)
{
    case 'new_pull':
        console.log(data);
}

// Initialize Discord Bot
var bot = new Discord.Client({
   token: auth.token,
   autorun: true
});
bot.on('ready', function (evt) {
    logger.info('Connected');
    logger.info('Logged in as: ');
    logger.info(bot.username + ' - (' + bot.id + ')');
});
bot.on('message', function (user, userID, channelID, message, evt) {
	switch (state)
    {
        case 'new_poll': // Initial case for handling a new poll
            if (message.substring(0, 1) == '!') {
                var args = message.substring(1).split(' ');
                var cmd = args[0];
            
                args = args.splice(1);
                switch(cmd) {
                    // !ping
                    case 'ping':
                        bot.sendMessage({
                            to: channelID,
                            message: 'Pong!'
                        });
                    case 'ohcontrare':
                        bot.sendMessage({
                            to: channelID,
                            message: 'I already have!'
                        });
                    break;
                    // Just add any case commands if you want to..
                }
            }
        case 'new_poll':
            if (message.substring(0, 1) == '!') {
                var args = message.substring(1).split(' ');
                var cmd = args[0];

                args = args.splice(1);
                switch (cmd) {
                    // !ping
                    case 'ping':
                        bot.sendMessage({
                            to: channelID,
                            message: 'Pong!'
                        });
                    case 'ohcontrare':
                        bot.sendMessage({
                            to: channelID,
                            message: 'I already have!'
                        });
                        break;
                    // Just add any case commands if you want to..
                }
            }
	}
});

function readStateData(file) {
    var rawFile = new XMLHttpRequest();
    rawFile.open("GET", file, false);
    rawFile.onreadystatechange = function () {
        if (rawFile.readyState === 4) {
            if (rawFile.status === 200 || rawFile.status == 0) {
                var allText = rawFile.responseText;
                alert(allText);
            }
        }
    }
    rawFile.send(null);
    return allText;
}

function loadFile(o)
{
    var fr = new FileReader();
    fr.onload = function(e)
        {
            showDataFile(e, o);
        };
    fr.readAsText(o.files[0]);

}