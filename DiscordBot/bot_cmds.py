#! /usr/bin/python
import os
import sys
import random
from enum import Enum
from enum import IntFlag
import json
import discord
from emoji.unicode_codes import UNICODE_EMOJI

NCAP_DEfINES = ["0\\N{COMBINING ENCLOSING KEYCAP}", "1\\N{COMBINING ENCLOSING KEYCAP}", "2\\N{COMBINING ENCLOSING KEYCAP}", 
                "3\\N{COMBINING ENCLOSING KEYCAP}", "4\\N{COMBINING ENCLOSING KEYCAP}", "5\\N{COMBINING ENCLOSING KEYCAP}", 
                "6\\N{COMBINING ENCLOSING KEYCAP}", "7\\N{COMBINING ENCLOSING KEYCAP}", "8\\N{COMBINING ENCLOSING KEYCAP}", "9\\N{COMBINING ENCLOSING KEYCAP}"]
EMOJI_TEXT = [":one:", ":two:", ":three:", ":four:", ":five:", ":six:", ":seven:", ":eight:", ":nine:" ]
GK_ROLE = 0 #561292277352235009 TODO: change this to Game Knight role when added to server 
CDNTR_ROLE = 0 #511721787604729866 TODO: Change back to coordinator role: 550387319190978571

CONFIG = {}
BOT_ID = 0
CDNTR_ID = 0
CHANNELS = ['game-night']


GAME_LIST = ['test', 'list', 'kappa']
DATE_LIST = ['01/01', '01/02', '01/03' ]
VOTES = { "143128900036329473" : [ 0,0,0 ],
          "561291243179606061" : [ 0,0,0 ],
          "142461721875972096" : [ 0,0,0 ]}
DONE_VOTING = {}

class state(IntFlag):
    NONE = 0
    NEW_RSVP = 1
    W4_RSVP = 2
    NEW_POLL = 3
    W4_VOTE = 4

STATE = state.NEW_RSVP

# READ TOKEN
def readToken():
    pwd = os.getcwd() + "\\DiscordBot\\"
    with open(pwd + "token.txt", "r") as f:
        lines = f.readlines()
        return lines[0].strip()

# SAVE CONFIG
def saveConfig(client, init = False):
    global CONFIG
    global STATE
    global BOT_ID
    global CDNTR_ID
    global CHANNELS

    if init: # need to get default values
        print('GENERATING NEW CONFIG!')
        STATE = state.NEW_RSVP+0 # need arithmatic to convert to int?
        CHANNELS = ['game-night']
        GK_ROLE = 561292277352235009
        CDNTR_ROLE = 511721787604729866

    config = {}  
    config['data'] = []  
    config['data'].append({  
        'STATE': STATE,
        'CHANNELS': CHANNELS,
        'GK_ROLE': GK_ROLE,
        'CDNTR_ROLE': CDNTR_ROLE
    })
    # save the data to config
    fp = os.getcwd() + "\\config.json"
    with open(fp, 'w+') as outfile:  
        json.dump(config, outfile)


# LOAD CONFIG
def loadConfig(client):
    global CONFIG
    global STATE
    global BOT_ID
    global CDNTR_ID
    global CHANNELS
    global CDNTR_ROLE

    fp = os.getcwd() + "\\config.json"
    if os.path.isfile(fp): # check if the config exists
        with open(fp, "r") as f:
            CONFIG = json.load(f)
            d = CONFIG['data'][0]
            CHANNELS = d['CHANNELS']
    else: # need to create a new config
        saveConfig(client, True)
        loadConfig(client) # need to reload now that there is config
        print('RELOADING...')

    # grab bot and coordinator IDs
    for ii in client.get_all_members():
        for role in ii.roles:
            if role.id == CDNTR_ROLE:
                CDNTR_ID = ii.id
            elif role.id == GK_ROLE:
                BOT_ID = ii.id


# return a list of all players that voted
def createPlayerList(votes):
    players = []
    for ii in votes:
        print(ii)
        print(ii.key)
        players.append(ii)
    return players


# return the matrix of game data,
# the list of games we should ignore,
# the total number of games to place on the ballot,
# and whether or not we include everyone's library in the ballot
def loadBallotInfo():
    fp = os.getcwd() + "\\ballot_info.json"
    with open(fp, "r") as f:
        data = json.load(f)
        info = data['info'][0]
        print(info)
        matrix = info['MATRIX']
        ignoreList = info['IGNORE_LIST']
        totalGames = info['TOTAL_GAMES']
        useEveryone = info['EVERYONE']
    
    return matrix, ignoreList, totalGames, useEveryone


# takes in a dictionary of votes
def getGameDate(votes):
    max = 0
    index = 0
    voteTally = [ 0,0,0 ]
    for i in range(0, len(votes)):
        for ii in i:
            voteTally[ii] += i
    for ii in range(0, len(voteTally)):
        print(voteTally[ii])
        if votes[ii] > max:
            max = voteTally[ii]
            index = ii

    return DATE_LIST[index]


def generateBallot(matrix, ignoreList, totalGames, useEveryone, playerList):
    ballot = []

    # LOGIC FOR WHEN WE CAN ONLY USE GAMES THAT EVERYONE OWNS
    if useEveryone:
        random.randint(1,100)
        pass
    else: # we can choose games that not everyone owns
        pass

    return ballot

# ----- Main() ----- #

token = readToken() # this must be the first step!
client = discord.Client()

# ----- Main() ----- #


# ON READY EVENT
@client.event
async def on_ready():
    global STATE
    # for ii in client.get_all_members():
    #     #print(ii)
    #     for role in ii.roles:
    #         #print(role, ": " + str(role.id))
    #         if role.id == CDNTR_ROLE:
    #             print(ii.name + " is the coordinator")
    #         if role.id == GK_ROLE:
    #             print(ii.name + " is the GK")

    #TESTING!
    # bd = {}
    # bd['info'] = []  
    # matrix = { '561291243179606061': [0,0,1,1,1,0], 
    #            '143128900036329473': [0,1,1,0,1,0],
    #            '433646881927593996': [1,0,0,0,1,0],
    #            '142461721875972096': [1,1,1,1,1,0], }

    # test_list = [ 'gearsofwar', 'nak', 'crashbandicoot' ]

    # bd['info'].append({  
    #     'MATRIX': matrix,
    #     'IGNORE_LIST': test_list,
    #     'TOTAL_GAMES': 4,
    #     'EVERYONE': True
    # })

    # m = bd['info'][0]['MATRIX']
    # ignoreList = bd['info'][0]['IGNORE_LIST']
    # tg = bd['info'][0]['TOTAL_GAMES']
    # everyone = bd['info'][0]['EVERYONE']
    # for ii in ignoreList:
    #     print(ii)
    # for ii in m:
    #     for i in ii:
    #         print(i)
    # print("total games: " + str(tg))
    # print("everyone: " + str(everyone))
    
    # fp = os.getcwd() + "\\ballot_info.json"
    # with open(fp, 'w+') as outfile:  
    #     json.dump(bd, outfile)
    # print('DONE M8')

    loadConfig(client)
    print("Bot_ID: ", BOT_ID)
    print("CDNTR: ", CDNTR_ID)
    
    if STATE == state.NEW_RSVP:
        for channel in client.get_all_channels():
            if channel.name == 'game-night':
                pass
                #await channel.send('WTF is a :one:?') # test message
                # await channel.send("Hear ye! Hear ye! A message from the great Lord Game-Night-Coordinator!\n" +
                #     "A new Game Cruscade is upon us! Will you answer the call to glory?\n" +
                #     "Please *!vote* here for **ALL** RSVP options below that you can attend (you can DM me as well, if you're a coward): ")
                # await channel.send(':one: - ' + DATE_LIST[0] + " ~8:00pm")
                # await channel.send(':two: - ' + DATE_LIST[1] + " ~8:00pm")
                # await channel.send(':three: - ' + DATE_LIST[2] + " ~8:00pm")
                # STATE = state.W4_RSVP

@client.event
async def on_message(message):
    global STATE
    global VOTES
    global GAME_LIST
    global DONE_VOTING

    # Get variables based on message sent 
    if str(message.channel) in CHANNELS:
        #print(message.content)
        id = message.author.id
        nickname = message.author.display_name()
        print(id, nickname)

##########################################

        if STATE == state.W4_RSVP:
            if id != BOT_ID: # ignore our own messages
                if id == CDNTR_ID: # coordinator issued that the date vote be closed
                    if message.content.find("!close") != -1:
                        for i in VOTES:
                            for ii in range(0, 2):
                                await message.channel.send(str(i[ii]))
                        await message.channel.send('The Almighty Coordinator has closed the RSVP poll!')
                        await message.channel.send('@everyone The selected Game Night will be: *' + getGameDate(VOTES) + '*' )
                        #await message.channel.send('Please await furth instruction from the crown!')
                        matrix, ignoreList, totalGames, useEveryone = loadBallotInfo()
                        playerList = createPlayerList(VOTES)
                        GAME_LIST = generateBallot(matrix, ignoreList, totalGames, useEveryone, playerList)
                        STATE = state.NEW_POLL
                        await message.channel.send('The gods have spoken! Below are your glorious game choices:\n' + 
                                                    '[ *Remember* - you may only vote for 1 game using :one:, :two:, :three:, etc. ]')

                if message.content.find("!vote") != -1:
                    validVote = False
                    id = str(id)
                    # reset / add the users votes
                    VOTES[id] = [ 0,0,0 ]
                    x = message.content.encode("ascii", 'namereplace').decode('utf-8')
                    if  x.find(NCAP_DEfINES[1]) != -1:
                        VOTES[id][0] = 1
                        validVote = True
                    if  x.find(NCAP_DEfINES[2]) != -1:
                        VOTES[id][1] = 1
                        validVote = True
                    if  x.find(NCAP_DEfINES[3]) != -1:
                        VOTES[id][2] = 1
                        validVote = True
                    
                    # print(nickname + " voted for:")
                    # for ii in VOTES[name]:
                    #     print(ii)

                    # check that the vote is a valid response
                    if validVote:
                        DONE_VOTING[''] = True
                        await message.channel.send("Thanks for voting " + nickname + "!\n" +
                                                "I'll be sure to inform you when the council has adjourned!")
                    else:
                        await message.channel.send("I'm terribly sorry " + nickname + ", but that's not a valid option... \n" +
                            "Why don't you give it another shot?\n" + 
                            "Make sure to use your emojis :wink:\ni.e. !vote :one: :two: :three:")
        elif STATE == state.NEW_POLL:
            print("GAME LIST: ")
            for ii in range(0, len(GAME_LIST)):
                print(GAME_LIST[ii])
                # post each option that they can vote for
                await message.channel.send('{0} - '.format(EMOJI_TEXT[ii]) + GAME_LIST[ii])
                STATE = state.W4_VOTE
        elif STATE == state.W4_VOTE:
            if id != BOT_ID: # ignore our own messages
                if message.content.find("!vote") != -1:
                    validVote = False
                    id = str(id)
                    # reset / add the users votes
                    gameVotes = []
                    for ii in range(0, len(GAME_LIST)):
                        gameVotes.append(0)
                    x = message.content.encode("ascii", 'namereplace').decode('utf-8')
                    print("x= " + x)
                    for ii in range(1, len(NCAP_DEfINES)):
                        print(NCAP_DEfINES[ii])
                        if x.find(NCAP_DEfINES[ii]) != -1:
                            gameVotes[ii] += 1
                            validVote = True
                    # check that the vote is a valid response
                    if validVote:
                        DONE_VOTING[id] = True
                        await message.channel.send("Good god, it's about time! Nonetheless, we will acknowledge your vote, Sir " + nickname + "!\n" +
                                                "I'll be sure to inform you when the others are ready to ride out!")
                        for ii in DONE_VOTING:
                            if DONE_VOTING[ii] == False:
                                return
                        # All players have finished voting, sounds the warhorns!
                        STATE = state.NONE
                        await message.channel.send("Good god, it's about time! Nonetheless, we will acknowledge your vote, Sir " + nickname + "...\n" +
                                                "I'll be sure to inform you when the others are ready to ride out!")
                    else:
                        print(message.content)
                        await message.channel.send("I'm terribly sorry " + nickname + ", but that's not a valid option... \n" +
                            "Why don't you give it another shot?\n" + 
                            "Make sure to use your emoji might :wink:\ni.e. !vote :one: :two: or :three: etc.")

client.run(token)

