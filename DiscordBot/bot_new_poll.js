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
//--------------------//
//  NEW POLL - BOT
//--------------------//
var game1 = 'REPLACE_TEXT'
// Initialize Discord Bot
var bot = new Discord.Client({
   token: auth.token,
   autorun: true
});
bot.on('ready', function (evt) {
    logger.info('Connected');
    logger.info('Logged in as: ');
    logger.info(bot.username + ' - (' + bot.id + ')');
    //console.log(`Logged in as ${bot.user.tag}!`);
    bot.channels.get("511721066905862167").send("oh hai");
    console.log('got here');
});

// bot.on('ready', () => {
//     // INTRO MESSAGE
//     var date = getDate();
//     var welcome_msg = 'New Game Night Poll! ' + date + '\n';
//     //

// });
// END INTRO MESSAGE
bot.on('message', function (user, userID, channelID, message, evt) {
    if (message.substring(0, 1) == '!') {
        var args = message.substring(1).split(' ');
        var cmd = args[0];
    
        args = args.splice(1);
        switch(cmd) {
            case 'vote':
                bot.sendMessage({
                    to: channelID,
                    message: 'I already have!'
                });
            case 'test':
                bot.channels.get("511721066905862167").send("oh hai");
            break;
        }
    }
});

function getDate() 
{
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();
    
    today = mm + '/' + dd + '/' + yyyy;
    return today;
}