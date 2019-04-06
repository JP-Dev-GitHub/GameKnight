#! /usr/bin/python
import os
import sys
from datetime import datetime
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


GAME_LIST = ['test', 'list', 'kappa', 'nein', 'hitler', 'leggo bible']
DATE_LIST = ['01/01 ~1pm', '01/02 ~2pm', '01/03 ~3pm' ]
FINAL_DATE = ''
FINAL_GAME = ''
# VOTES = { "143128900036329473" : [ 0,0,0 ],
#           "561291243179606061" : [ 0,0,0 ],
#           "142461721875972096" : [ 0,0,0 ]}
VOTES = {}
DONE_VOTING = {}

class state(IntFlag):
    NONE = 0
    NEW_POLL = 1
    W4_RSVP = 2
    W4_VOTE = 3

STATE = state.NEW_POLL

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
    global GK_ROLE
    global CDNTR_ROLE

    if init: # need to get default values
        print('GENERATING NEW CONFIG!')
        STATE = state.NEW_POLL+0 # need arithmatic to convert to int?
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
    global GK_ROLE
    global CDNTR_ROLE

    fp = os.getcwd() + "\\config.json"
    if os.path.isfile(fp): # check if the config exists
        with open(fp, "r") as f:
            CONFIG = json.load(f)
            d = CONFIG['data'][0]
            CHANNELS = d['CHANNELS']
            GK_ROLE = d['GK_ROLE']
            CDNTR_ROLE = d['CDNTR_ROLE']
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


def log(finalGame, finalDate, gameList):
    fp = os.getcwd() + "\\record.log"
    with open(fp, 'w+') as f:
        f.write('log for ' + finalDate + ' recorded at: ' + datetime.now().strftime('%Y-%m-%d %H:%M:%S') + '\n')
        f.write('Selected game: ' + finalGame + ':\n')
        f.write('Possible games: \n')
        for ii in gameList:
            f.write('       ' + str(ii) + ": " + gameList[ii] + "\n")

# return a list of all players that voted
def createPlayerList(votes):
    players = []
    for ii in votes:
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
        matrix = info['MATRIX']
        print(matrix)
        ignoreList = info['IGNORE_LIST']
        totalGames = info['TOTAL_GAMES']
        useEveryone = info['EVERYONE']
    
    return matrix, ignoreList, totalGames, useEveryone


# takes in a dictionary of votes
def getGameDate(votes):
    voteTally = [ 0,0,0 ]
    for key in votes:
        for i in range(0, len(votes[key])):
            voteTally[i] += votes[key][i]
    index = max(voteTally)
    return DATE_LIST[index]

# takes a dict matrix and list ignoreList
def initGameList(matrix, ignoreList):
    viableGameIndexes = []
    x = str(next(iter(matrix)))
    print(len(matrix[x]))
    if len(GAME_LIST) != len(matrix[x]):
        print('ERROR: GAME_LIST length does not match matrix length!')
        return []

    for index in range(0, len(GAME_LIST)):
        if GAME_LIST[index] not in ignoreList: # skip any games in ignore list
            viableGameIndexes.append(index)
    return viableGameIndexes


# NOTE: Game IDs must be changed to lower case and spaces removed
def generateBallot(matrix, ignoreList, totalGames, useEveryone, playerList):
    ballot = []
    print('GENERATING BALLOT')

    viableGames = initGameList(matrix, ignoreList)
    if not viableGames: # double check that we have a list of games
        exit(1) 
    
    # LOGIC FOR WHEN WE CAN ONLY USE GAMES THAT EVERYONE OWNS
    if useEveryone:
        # loop through all viable games
        for col in viableGames:
            # loop through all players
            for key in matrix:
                if key in playerList: # only check players that are attending
                    #print("({0},{1})".format(key, str(col)) + " = " + str(matrix[key][col]))
                    if matrix[key][col] != 1:
                        break # we cant use this game
            # we got through all players without issue, add the game
            ballot.append(GAME_LIST[col])
        # randomly cut the list down to size
        while len(ballot) > totalGames:
            x = random.randint(0, len(ballot)-1)
            ballot.pop(x)

    else: # we can choose games that not everyone owns
        # loop through all viable games and
        # randomly cut the list down to size
        while len(viableGames) > totalGames:
            viableGames.pop(random.randint(0, len(viableGames)-1))
        # store the actual game names in the ballot
        for game in viableGames:
            ballot.append(GAME_LIST[game])
        for ii in ballot:
            print(ii)
    return ballot

def getFinalGame(GAME_LIST, gameVotes):
    percents = []
    finalGame = ''
    for ii in range(0, len(gameVotes)):
        print(gameVotes[ii])
        if gameVotes[ii] == 0:
            GAME_LIST.pop(ii)
    total = sum(gameVotes)
    print("total = " + str(total))
    # get a list of percentages
    for ii in range(0, len(gameVotes)):
        x = float(gameVotes[ii]) / float(total)
        x *= 100
        percents.append(x)
    # sort the percentages from least to greatest:
    percents.sort()
    for ii in percents:
        print(ii)
    rand = random.randint(0, 100)
    rand = float(rand) + float(random.randint(0, 99) / 99)
    print("Rand: " + str(rand))
    for ii in range(0, len(percents-1)):
        if rand < percents[ii]:
            print('Returning: ' + str(ii) + ", " + GAME_LIST[ii])
            return GAME_LIST[ii]
    print('Returning: ' + str(ii) + ", " + GAME_LIST[len(GAME_LIST) - 1])
    return GAME_LIST[len(GAME_LIST) - 1]
# ----- Main() ----- #

token = readToken() # this must be the first step!
client = discord.Client()

# ----- Main() ----- #

# ON READY EVENT
@client.event
async def on_ready():
    global STATE
    global DATE_LIST
    global GK_ROLE
    global CDNTR_ROLE

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
    # gameVotes = [ 0, 4, 2, 1 ]
    # GAME_LIST.pop(0)
    # GAME_LIST.pop(0)
    # game = getFinalGame(GAME_LIST, gameVotes)
    # print(game)

    loadConfig(client)
    print("Bot_ID: ", BOT_ID)
    print("CDNTR: ", CDNTR_ID)
    
    if STATE == state.NEW_POLL:
        for channel in client.get_all_channels():
            if channel.name == 'game-night':
                pass
                #await channel.send('WTF is a :one:?') # test message
                await channel.send("Hear ye! Hear ye! A message from the great Lord Game-Night-Coordinator!\n" +
                    "A new Game Cruscade is upon us! Will you answer the call to glory?\n" +
                    "Please *!vote* here for **ALL** RSVP options below that you can attend (you can DM me as well, if you're a coward): ")
                for ii in range(0, len(DATE_LIST)):
                    await channel.send('{0} - '.format(EMOJI_TEXT[ii]) + DATE_LIST[ii])
                STATE = state.W4_RSVP
                print('NEW RSVP FINISHED, STARTING W4RSVP...')

@client.event
async def on_message(message):
    global STATE
    global VOTES
    global GAME_LIST
    global DONE_VOTING
    global FINAL_DATE
    global FINAL_GAME

    
    # only active in certain channels
    if str(message.channel) not in CHANNELS:
        return
    # Get variables based on message sent 
    #print(message.content)
    #print('State: ' + str(STATE))
    id = message.author.id
    nickname = message.author.display_name

##########################################

    if id == BOT_ID: # ignore our own messages
        return
    if STATE == state.W4_RSVP:
        #print('Comparing ID: ' + str(id) + " with BOT_ID: " + str(BOT_ID))
        if id == CDNTR_ID: # coordinator issued that the date vote be closed
            if message.content.find("!close") != -1:
                FINAL_DATE = getGameDate(VOTES)
                await message.channel.send('The Almighty Coordinator has closed the RSVP poll!')
                await message.channel.send('@everyone The selected Game Night will be: **' + FINAL_DATE + '**' )
                matrix, ignoreList, totalGames, useEveryone = loadBallotInfo()
                playerList = createPlayerList(VOTES)
                GAME_LIST = generateBallot(matrix, ignoreList, totalGames, useEveryone, playerList)
                print('FINISHED W4RSVP, STARTING NEW POLL...')
                await message.channel.send('The gods have spoken! Below are your glorious game choices:\n' + 
                                            '[ *Remember* - you may only vote for 1 game using :one:, :two:, :three:, etc. ]')
                print("GAME LIST: ")
                for ii in range(0, len(GAME_LIST)):
                    print(GAME_LIST[ii])
                    # post each option that they can vote for
                    await message.channel.send('{0} - '.format(EMOJI_TEXT[ii]) + GAME_LIST[ii])
                if len(GAME_LIST) <= 1:
                    STATE = state.W4_VOTE
                else:
                    STATE = state.W4_VOTE
                print('FINISHED W4_RSVP, STARTING W4_VOTE...')

        if message.content.find("!vote") != -1:
            validVote = False
            id = str(id)
            # reset / add the users votes
            VOTES[id] = [ 0,0,0 ]
            x = message.content.encode("ascii", 'namereplace').decode('utf-8')
            for ii in range(1, len(NCAP_DEfINES)):
                if x.find(NCAP_DEfINES[ii]) != -1:
                    VOTES[id][ii-1] += 1
                    validVote = True
            
            print(nickname + " voted for:")
            for ii in VOTES[id]:
                print(ii)

            # check that the vote is a valid response
            if validVote:
                DONE_VOTING[''] = True
                await message.channel.send("Thanks for voting " + nickname + "!\n" +
                                        "I'll be sure to inform you when the council has adjourned!")
            else:
                print('REMOVING ID: ' + id)
                VOTES.pop(id, None)
                await message.channel.send("I'm terribly sorry " + nickname + ", but that's not a valid option... \n" +
                    "Why don't you give it another shot?\n" + 
                    "Make sure to use your emojis :wink:\ni.e. !vote :one: :two: :three:")
    elif STATE == state.W4_VOTE:
        if len(GAME_LIST) > 1:
            if message.content.find("!vote") != -1:
                validVote = False
                id = str(id)
                # reset / add the users votes
                gameVotes = []
                for ii in range(0, len(GAME_LIST)):
                    gameVotes.append(0)
                x = message.content.encode("ascii", 'namereplace').decode('utf-8')
                for ii in range(1, len(NCAP_DEfINES)):
                    if x.find(NCAP_DEfINES[ii]) != -1:
                        gameVotes[ii] += 1
                        validVote = True
                # check that the vote is a valid response
                if validVote:
                    DONE_VOTING[id] = True
                    print('Done VOTING STATUS: ' + str(DONE_VOTING[id]))
                    await message.channel.send("Good god, it's about time! Nonetheless, we will acknowledge your vote, Sir " + nickname + "!\n" +
                                            "I'll be sure to inform you when the others are ready to ride out!")
                    for ii in DONE_VOTING:
                        if DONE_VOTING[ii] == False:
                            STATE = state.NONE
                            # All players have finished voting, sounds the warhorns!
                            FINAL_GAME = getFinalGame(GAME_LIST, gameVotes)
                            await message.channel.send("HUZZAH!\nOur game has been selected at last!\n" +
                                                    "We will be playing **" + FINAL_GAME + "** on the date of **" + FINAL_DATE + "**")
                            # log the results
                            log(FINAL_GAME, FINAL_DATE, GAME_LIST)
                            return
                else:
                    print(message.content)
                    await message.channel.send("I'm terribly sorry " + nickname + ", but that's not a valid option... \n" +
                        "Why don't you give it another shot?\n" + 
                        "Make sure to use your emoji might :wink:\ni.e. !vote :one: :two: or :three: etc.")
    elif STATE == state.NONE:
        if message.content.find("!vote") != -1:
            await message.channel.send("My apologies good Sir, but the Great Coordinator has not yet given orders to march...\n" +
                    "If you feel there is an error, contact the Coordinator directly to rally a Saracen-scorching party!\n")

client.run(token)

