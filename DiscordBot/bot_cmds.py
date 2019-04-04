#! /usr/bin/python
import os
import sys
from enum import Enum
import json
import discord
import emoji


BOT_ID = '4103'
CHANNELS = ['game-night']
GAME_LIST = ['test', 'list', 'kappa']
DATE_LIST = ['01/01', '01/02', '01/03' ]
DATE_VOTES = [ 0,0,0 ]
COORDINATOR_ID = '5473'#'9959'


class status(Enum):
    NEW_RSVP = 1
    W4_RSVP = 2
    NEW_POLL = 3
    W4_VOTE = 4

STATUS = status.NEW_RSVP

def read_token():
    pwd = os.getcwd() + "\\DiscordBot\\"
    with open(pwd + "token.txt", "r") as f:
        lines = f.readlines()
        return lines[0].strip()

token = read_token()

client = discord.Client()

# use this for sending messages on ready!
@client.event
async def on_ready():
    global STATUS
            
    if STATUS == status.NEW_RSVP:
        for channel in client.get_all_channels():
            if channel.name == 'game-night':
                await channel.send('WTF is a :one:?') # test message
                # await channel.send("Hear ye! Hear ye! A message from the great Lord Game-Night-Coordinator!\n \
                #     A new Game Cruscade is upon us! Will you answer the call to glory?\n \
                #     Please choose all RSVP options below that you can certainly attend: ")
                # await channel.send(':one: - ' + DATE_LIST[0] + " ~8:00pm")
                # await channel.send(':two: - ' + DATE_LIST[1] + " ~8:00pm")
                # await channel.send(':three: - ' + DATE_LIST[2] + " ~8:00pm")
                STATUS = status.W4_RSVP

@client.event
async def on_message(message):
    global STATUS
    global DATE_VOTES
    # Get variables based on message sent 
    if str(message.channel) in CHANNELS:
        #print(message.content)
        name = str(message.author)
        nickname = name[:name.find('#')]
        name = name[name.find('#')+1:]
##########################################
        if STATUS == status.W4_RSVP:
            if name != BOT_ID: # ignore our own messages
                if message.content.find("!vote") != -1:
                    validVote = True
                    if message.content.find(":one:") != -1:
                        DATE_VOTES[0] += 1
                    elif message.content.find(":two:") != -1:
                        DATE_VOTES[1] += 1
                    elif message.content.find(":three:") != -1:
                        DATE_VOTES[2] += 1
                    else:
                        validVote = False
                    # check that the vote is a valid response
                    if validVote:
                        await message.channel.send("Thanks for voting " + nickname + "! \n \
                                                I'll be sure to inform you when the council has adjourned!")
                    else:
                        print(message.content)
                        await message.channel.send("I'm terribly sorry " + nickname + ", but that's not a valid option... \n \
                            Why don't you give it another shot? Make sure to use your emojis :wink:\ni.e. !vote followed by :one:, :two:, or :three:")
            if name == COORDINATOR_ID: # coordinator issued that the date vote be closed
                if message.content.find("!close") != -1:
                    STATUS = status.NEW_POLL
                    message.channel.send('The Almighty Coordinator has closed the RSVP poll!')
                    message.channel.send('@everyone The selected Game Night will be: *' + getGameDate(DATE_VOTES) + '*' )
                    message.channel.send('Please await furth instruction from the crown!')
        elif STATUS == status.NEW_POLL:
            pass
        elif STATUS == status.W4_VOTE:
            if name != BOT_ID: # ignore our own messages
                if message.content.find("!vote") != -1:
                    gameVote = message.content[message.content.find('!vote')+6:]
                    print(gameVote)
                    if gameVote in GAME_LIST:
                        await message.channel.send("Thanks for voting " + nickname + "! \n \
                            I'll be sure to inform you when the others are ready to ride out!")
                    else:
                        await message.channel.send("I'm terribly sorry " + nickname + ", but that's not a valid option... \n \
                            Why don't you give it another shot? Give it your all this time!")

client.run(token)

def getGameDate(votes):
    max = 0
    index = 0
    for ii in range(0, len(votes)):
        if votes[ii] > max:
            max = votes[ii]
            index = ii

    return DATE_LIST[ii]

