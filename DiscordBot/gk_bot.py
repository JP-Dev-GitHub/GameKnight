#! /usr/bin/python
import os
import sys
from datetime import datetime
import random
import math
from enum import Enum
from enum import IntFlag
import json
import operator
import discord
from emoji.unicode_codes import UNICODE_EMOJI

NCAP_DEFINES = ["0\\N{COMBINING ENCLOSING KEYCAP}", "1\\N{COMBINING ENCLOSING KEYCAP}", "2\\N{COMBINING ENCLOSING KEYCAP}", 
                "3\\N{COMBINING ENCLOSING KEYCAP}", "4\\N{COMBINING ENCLOSING KEYCAP}", "5\\N{COMBINING ENCLOSING KEYCAP}", 
                "6\\N{COMBINING ENCLOSING KEYCAP}", "7\\N{COMBINING ENCLOSING KEYCAP}", "8\\N{COMBINING ENCLOSING KEYCAP}", "9\\N{COMBINING ENCLOSING KEYCAP}"]
EMOJI_TEXT = [":one:", ":two:", ":three:", ":four:", ":five:", ":six:", ":seven:", ":eight:", ":nine:" ]
PATH = ''
CDNTR_ROLE = 0
DATE_LIMIT = 3

CONFIG = {}
BOT_ID = 0
CDNTR_ID = 0
CHANNEL_ID = ''
CHANNEL = 0

GAME_LIST = []
DATE_LIST = []
FINAL_DATE = ''
FINAL_GAME = ''
VOTES = {}
DONE_VOTING = {}
REROLL_VOTERS = {}
TOTAL_PLAYERS = 0

class state(IntFlag):
    NONE = 0
    NEW_POLL = 1
    W4_RSVP = 2
    W4_VOTE = 3
    VETO = 4
    REROLLING = 5

STATE = state.NEW_POLL

################################   FUNCTIONS   ################################

# READ TOKEN
def readToken():
    with open(PATH + "DiscordBot\\token.txt", "r") as f:
        lines = f.readlines()
        return lines[0].strip()


def saveState():
    global STATE
    fp = PATH + "ballot_info.json"
    newInfo = {}
    if os.path.isfile(fp): # check if file exists
        with open(fp, "r") as f:
            newInfo = json.load(f)
            newInfo['STATE'] = STATE+0
    else:
        print("ERROR: No json data to read from and save!")
        exit(1)
    # delete the old data and rewrite it
    os.remove(fp)
    # save the state and other data to ballot_info
    with open(fp, 'w+') as outfile:  
        json.dump(newInfo, outfile)


# LOAD CONFIG
def loadConfig(client):
    global CONFIG
    global STATE
    global BOT_ID
    global CDNTR_ID
    global CHANNEL_ID
    global DATE_LIST
    global CDNTR_ROLE
    global CHANNEL
    global GUILD



#    if message.channel.id != CHANNEL_ID and not is_direct_msg:
    fp = PATH + "ballot_info.json"
    if os.path.isfile(fp): # check if the config exists
        with open(fp, "r") as f:
            d = json.load(f)
            STATE = state(d['STATE'])
            print("loaded state: " + str(d['STATE']))
            CHANNEL_ID = int(d['GK_CHANNEL'])
            DATE_LIST = d['DATE_LIST']
            BOT_ID = int(d['GK_ID'])
            CDNTR_ROLE = int(d['CDNTR_ROLE'])
    else: # need to create a new config
        #saveConfig(client, True)
        #loadConfig(client) # need to reload now that there is config
        print('ERROR: No ballot_info.json file to read from!')
        exit(1)

    CHANNEL = client.get_channel(id=CHANNEL_ID)
    GUILD = CHANNEL.guild
    # grab bot and coordinator IDs
    for ii in GUILD.members:
        if CDNTR_ID != 0:
            break
        for role in ii.roles:
            if role.id  == CDNTR_ROLE:
                CDNTR_ID = ii.id
                break


def log(finalGame, finalDate, gameList):
    fp = PATH + "record.log"
    with open(fp, 'w+') as f:
        f.write('log for ' + finalDate + ' recorded at: ' + datetime.now().strftime('%Y-%m-%d %H:%M:%S') + '\n')
        f.write('Selected game: ' + finalGame + ':\n')
        f.write('Possible games: \n')
        for ii in range(0, len(gameList)):
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
    fp = PATH + "ballot_info.json"
    with open(fp, "r") as f:
        data = json.load(f)
        matrix = data['MATRIX']
        print(matrix)
        ignoreList = data['IGNORE_LIST']
        totalGames = data['TOTAL_GAMES']
        useEveryone = True if data['EVERYONE'].capitalize() == 'True' else False

        playerList = {}
        newMatrix = {}
        for ii in matrix:
            key = ii
            newKey = key[key.find(':')+1:]
            newMatrix[newKey] = matrix[ii]
            playerList[newKey] = key[:key.find(':')]

    return newMatrix, playerList, ignoreList, totalGames, useEveryone


# takes in a dictionary of votes
def getGameDate(votes):
    global DATE_LIST
    voteTally = []
    for ii in range(0, DATE_LIMIT):
        voteTally.append(0)
    for key in votes:
        for i in range(0, len(votes[key])):
            voteTally[i] += votes[key][i]
            #print('ind: ' + str(i) + ', val: ' + str(voteTally[i]))
    m = 0
    index = 0
    for ii in range(0, len(voteTally)):
        if voteTally[ii] > m:
            m = voteTally[ii]
            index = ii
    print('max: ' + str(m) + ' @:' + str(index))
    return DATE_LIST[index]


# takes a dict matrix and list ignoreList
def initGameList(matrix, ignoreList):
    global GAME_LIST
    viableGameIndexes = []
    fp = PATH + "ballot_info.json"
    with open(fp, "r") as f:
        data = json.load(f)
        GAME_LIST = data['MASTER_GAME_LIST']

    x = str(next(iter(matrix)))
    if len(GAME_LIST) != len(matrix[x]):
        print('ERROR: GAME_LIST length does not match matrix length!')
        return []

    for index in range(0, len(GAME_LIST)):
        if GAME_LIST[index] not in ignoreList: # skip any games in ignore list
            viableGameIndexes.append(index)
    return viableGameIndexes


# NOTE: Game IDs must be changed to lower case and spaces removed
def generateBallot(matrix, ignoreList, totalGames, useEveryone, playerList):
    print('GENERATING BALLOT')
    ballot = []
    
    viableGames = initGameList(matrix, ignoreList)
    if not viableGames: # double check that we have a list of games
        exit(1) 

    # LOGIC FOR WHEN WE CAN ONLY USE GAMES THAT EVERYONE OWNS
    if useEveryone:
        # loop through all viable games
        for col in viableGames:
            allOwn = True
            for key in playerList: # only check players that are attending
                #print("({0},{1})".format(key, str(col)) + " = " + str(matrix[key][col]))
                if matrix[key][col] != 1:
                    allOwn = False
                    break # we cant use this game
            if allOwn: # we got through all players without issue, add the game
                ballot.append(GAME_LIST[col])
        # randomly cut the list down to size
        while len(ballot) > totalGames:
            r1 = random.randint(0, len(ballot)-1)
            #print("POPPING: " + ballot[r1])
            ballot.pop(r1)

    else: # we can choose games that not everyone owns
        # loop through all viable games and
        # randomly cut the list down to size
        while len(viableGames) > totalGames:
            viableGames.pop(random.randint(0, len(viableGames)-1))
        # store the actual game names in the ballot
        for game in viableGames:
            ballot.append(GAME_LIST[game])
    return ballot

def getFinalGame(GAME_LIST, gameVotes):
    percents = {}
    total = 0

    for ii in gameVotes:
        total += gameVotes[ii]

    if total <= 0.0: # unexpected error
        exit(1)

    # get a list of percentages
    for ii in gameVotes:
        x = float(gameVotes[ii]) / float(total)
        x *= 100.0
        percents[ii] = x

    # sort the percentages from least to greatest:
    sorted_percents = sorted(percents.items(), key=lambda kv: kv[1])
    rand = random.randint(1, 100)
    rand = float(rand) + float(random.randint(0, 99) / 99)
    x = min(x, 99)
    print("Rand: " + str(rand))
    ii = 0
    for game in sorted_percents:
        #print('check for: ' + game[0])
        #print('comparing: ' + str(rand) + ' <= ' + str(game[1]))
        if rand <= game[1]:
            #print('Returning: ' + str(ii) + ", " + game[0])
            return game[0]
        ii += 1
    #print('Have to return: ' + str(ii) + ", " + GAME_LIST[len(GAME_LIST) - 1])
    return GAME_LIST[len(GAME_LIST) - 1]


def reset():
    global STATE
    global DATE_LIST
    global VOTES
    global GAME_LIST
    global FINAL_DATE
    global FINAL_GAME
    global REROLL_VOTERS
    global DONE_VOTING
    global TOTAL_PLAYERS

    STATE = state.NEW_POLL
    VOTES = {}
    GAME_LIST = []
    FINAL_DATE = ''
    FINAL_GAME = ''
    DONE_VOTING = {}
    REROLL_VOTERS = {}
    TOTAL_PLAYERS = 0


################################   Main()   ################################

x = os.getcwd()
PATH = x[:x.find('GameKnight')+10] + '\\'
token = readToken() # this must be the first step!
client = discord.Client()

################################   Main()   ################################

# ON READY EVENT
@client.event
async def on_ready():
    global STATE
    global DATE_LIST
    global CDNTR_ROLE
    global CHANNEL

    loadConfig(client)
    STATE = state.NEW_POLL

    await CHANNEL.send("Greetings! I am here to assist you in your game selection!")
    # if STATE == state.NEW_POLL:
    #     for channel in client.get_all_channels():
    #         if channel.id == CHANNEL_ID:
    #             await channel.send(":trumpet: @everyone :trumpet: Hear ye! Hear ye! A call for aid from Lord Game-Night-Coordinator!\n" +
    #                 "A new Game Cruscade is upon us! Will you answer the call to glory?\n" +
    #                 "Please **!vote** here for **ALL** RSVP options below that you can attend (you can DM me as well, if you're a coward): ")
    #             for ii in range(0, len(DATE_LIST)):
    #                 await channel.send('{0}   **-   {1}**'.format(EMOJI_TEXT[ii], DATE_LIST[ii]) + '\n')
    #             STATE = state.W4_RSVP
    #             saveState()
    #             print('NEW RSVP FINISHED, STARTING W4RSVP...')

@client.event
async def on_message(message):
    global STATE
    global VOTES
    global GAME_LIST
    global DATE_LIST
    global DONE_VOTING
    global FINAL_DATE
    global FINAL_GAME
    global REROLL_VOTERS
    global TOTAL_PLAYERS
    global CDNTR_ID
    global BOT_ID
    global CHANNEL
    
    # Get variables based on message sent
    id = message.author.id
    nickname = message.author.display_name
    msg = message.content
    is_direct_msg = isinstance(message.channel, discord.abc.PrivateChannel)
    
    # only active in GK channel
    if message.channel.id != CHANNEL_ID and not is_direct_msg:
        return

    if is_direct_msg and id not in VOTES:
        return

    if STATE != state.REROLLING:
        # ignore our own messages
        if STATE != state.NEW_POLL and id == BOT_ID:
            return
    else: # STATE = REROLLING so we need to continue to the W4_VOTE state
        STATE = state.W4_VOTE
        saveState()
################################    COMMANDS    ################################
    
    # !HELP COMMAND
    if (msg.find("!commands") != -1) or (msg.find("!help") != -1):
        await message.channel.send(
        "Greetings traveler! How may I be of assistance?\n" + 
        "      -      **!help** / **!commands**  :  Issues the current help screen to view commands and other helpful information.\n" +
        "      -      **!vote**  :  When a ballot is posted, use this cmd followed by a number emoji (:one:,:two:,:three:, etc.) to cast your vote.\n" +
        "      -      **!status**  :  When a ballot is posted, use this cmd followed by a number emoji (:one:,:two:,:three:, etc.) to cast your vote.\n" +
        "      -      **!veto**  :  Each player can use a veto command once when a game as been selected. If **more than half** the player veto, " +
                                "then ballot is rerolled.\n" +
        "      -      **!close**  :  [Coordinator CMD] Close the current RSVP poll. **No other users can join the poll after this command is used!**\n" +
        "      -      **!reset**  :  [Coordinator CMD] Close the current RSVP poll. **No other users can join the poll after this command is used!**\n" +
        "      -      **!kill**  :  [Coordinator CMD] Terminate the active poll. Use this when a poll is completed or " +
                                "you want to restart. **WARNING: This will delete ALL current poll data!**\n\n")
        return

    # !KILL COMMAND
    if (msg.find("!kill") != -1) and id == CDNTR_ID:
        await message.channel.send("We've been routed! The battle is lost! Fall back, men!!  :arrow_left::horse_racing:\n" + 
            ":robot: **Poll has been terminated** :robot:")
        exit(0)
    # !RESET COMMAND
    if msg.find("!reset") != -1 and id == CDNTR_ID:
        reset()

    # # !STATUS COMMAND
    if msg.find("!status") != -1:
        if STATE == state.NONE:
            await message.channel.send("**STATUS** :  None. There is no active poll. Try **!kill** to turn me off. :flushed: ")
        elif STATE == state.NEW_POLL:
            await message.channel.send("**STATUS** :  NEW POLL. A new poll has just been started by the Coordinator.")
        elif STATE == state.W4_RSVP:
            await message.channel.send("**STATUS** :  W4 RSVP. Currently waiting for everyone to declare availability for upcoming game night.")
        elif STATE == state.W4_VOTE:
            await message.channel.send("**STATUS** :  W4 VOTE. Currently waiting for at-tendees to declare their preferred game from the ballot.")
        elif STATE == state.VETO:
            await message.channel.send("**STATUS** :  VETO. A game has been selected, you may now cast **!veto** if you want a different game.")
        elif STATE == state.REROLLING:
            await message.channel.send("**STATUS** :  REROLLING. Players didn't like the last game. Now rerolling ballot options.")

################################    STATE LOGIC    ################################

    if STATE == state.NEW_POLL:
        for channel in client.get_all_channels():
            if channel.id == CHANNEL_ID:
                STATE = state.W4_RSVP
                await channel.send(":trumpet: @everyone :trumpet: Hear ye! Hear ye! A call for aid from Lord Game-Night-Coordinator!\n" +
                    "A new Game Cruscade is upon us! Will you answer the call to glory?\n" +
                    "Please **!vote** here for **ALL** RSVP options below that you can attend (you can DM me as well, if you're a coward): ")
                for ii in range(0, len(DATE_LIST)):
                    await channel.send('{0}   **-   {1}**'.format(EMOJI_TEXT[ii], DATE_LIST[ii]) + '\n')
                saveState()
                print('NEW RSVP FINISHED, STARTING W4RSVP...')
    elif STATE == state.W4_RSVP:
        print(message.author)
        print(msg)
        if id == CDNTR_ID: # coordinator issued that the date vote be closed
            if message.content.find("!close") != -1:
                FINAL_DATE = getGameDate(VOTES)
                await message.channel.send('The Almighty Coordinator has closed the RSVP poll!')
                await message.channel.send(':trumpet: @everyone :trumpet: The selected Game Night will be: **' + FINAL_DATE + '**' )

                matrix, playerList, ignoreList, totalGames, useEveryone = loadBallotInfo()
                playerList = createPlayerList(VOTES)
                TOTAL_PLAYERS = len(playerList)
                GAME_LIST = generateBallot(matrix, ignoreList, totalGames, useEveryone, playerList)
                print('FINISHED W4RSVP, STARTING NEW POLL...')
                await message.channel.send(':trumpet: @everyone :trumpet: The gods have spoken! Below are your glorious game choices:\n' + 
                                            '[ *Remember* - you may only vote for 1 game!]')
                botStr = ''
                for ii in range(0, len(GAME_LIST)):
                    print(GAME_LIST[ii])
                    botStr += '{0}   **-   {1}**'.format(EMOJI_TEXT[ii], GAME_LIST[ii]) + "\n\n"
                    # post each option that they can vote for
                await message.channel.send(botStr)
                if len(GAME_LIST) <= 1:
                    FINAL_GAME = GAME_LIST[0]
                    await message.channel.send(":trumpet: @everyone  HUZZAH! :trumpet:\nOur game has been selected at last!\n" +
                                        "We will be playing **" + FINAL_GAME + "** on the date of **" + FINAL_DATE + "**\n:shield:  ***To arms!***  :dagger:")
                    # log the results
                    log(FINAL_GAME, FINAL_DATE, GAME_LIST)
                    STATE = state.VETO
                    saveState()
                else:
                    STATE = state.W4_VOTE
                    saveState()
                print('FINISHED W4_RSVP, STARTING W4_VOTE...')


        # check if the player voted for a date
        if message.content.find("!vote") != -1:
            validVote = False
            id = str(id)
            # reset / add the users votes
            VOTES[id] = [ 0,0,0 ]
            x = message.content.encode("ascii", 'namereplace').decode('utf-8')
            print(len(VOTES))
            for ii in range(0, len(DATE_LIST)):
                print("looking for: " + str(ii) + " in: " + x)
                if ii < DATE_LIMIT and not validVote and (x.find(NCAP_DEFINES[ii+1]) != -1 or x.find(str(ii+1)) != -1):
                    VOTES[id][ii] += 1
                    validVote = True
            #print(nickname + " voted for:")
            #for ii in VOTES[id]:
            #    print(ii)

            # check that the vote is a valid response
            if validVote:
                DONE_VOTING[id] = False
                await message.author.send("Thanks for voting " + nickname + "!\n" +
                                        "I'll be sure to inform you when the council has adjourned!\n")
                # inform channel
                await message.channel.send(":trumpet:*Trumpet Sounds*:trumpet:\nAhem! Sir " + nickname + " has voted!\n")
            else:
                print('REMOVING ID: ' + id)
                VOTES.pop(id, None)
                await message.channel.send("I'm terribly sorry " + nickname + ", but that's not a valid option... \n" +
                    "Why don't you give it another shot?\n" + 
                    "Make sure to use numbers or emojis! \ni.e. !vote 1 or !vote :three:")

    elif STATE == state.W4_VOTE:
        if len(GAME_LIST) > 1:
            if message.content.find("!vote") != -1:
                validVote = False
                id = str(id)
                # reset / add the users votes
                gameVotes = {}
                for ii in GAME_LIST:
                    gameVotes[ii] = 0
                x = message.content.encode("ascii", 'namereplace').decode('utf-8') 
                for ii in range(1, len(NCAP_DEFINES)):
                    print("looking for: " + str(ii) + " in: " + x)
                    # make sure we havent already found a valid vote, that were in bounds, and that it matches a recognized option
                    if not validVote and ii <= len(GAME_LIST) and (x.find(NCAP_DEFINES[ii]) != -1 or x.find(str(ii)) != -1):
                        index = ii-1
                        gameVotes[GAME_LIST[index]] += 1
                        validVote = True
                        break
                # check that the vote is a valid response
                if validVote:
                    DONE_VOTING[id] = True
                    # direct message player
                    await message.author.send("Good gods, it's about time! Nonetheless, we will acknowledge your selection of **" + GAME_LIST[index] + "**, Sir " + nickname + "!\n" +
                                            "I'll be sure to inform you when the others are ready to ride out.\n")
                    # inform channel
                    await message.channel.send(":trumpet:*Trumpet Sounds*:trumpet:\nAhem! Sir " + nickname + " has voted!\n")
                    print("CHECK DONE_VOTING:")
                    for ii in DONE_VOTING:
                        print(ii)
                        if DONE_VOTING[ii] == False:
                            return
                    # All players have finished voting, sounds the warhorns!
                    #STATE = state.NONE
                    FINAL_GAME = getFinalGame(GAME_LIST, gameVotes)
                    await message.channel.send(":trumpet: @everyone  HUZZAH! :trumpet:\nOur game has been selected at last!\n\n" +
                                            "We will be playing **" + FINAL_GAME + "** on the date of **" + FINAL_DATE + "**\n\n" + 
                                            "::crossed_swords:  ***To arms!***  :crossed_swords:\n")
                    # log the results
                    log(FINAL_GAME, FINAL_DATE, GAME_LIST)
                    STATE = state.VETO
                    saveState()
                    return
                else:
                    await message.channel.send("I'm terribly sorry " + nickname + ", but that's not a valid option... \n" +
                        "Why don't you give it another shot?\n" + 
                        "Make sure to use emojis or numbers :wink:\ni.e. !vote :one:, :two:, :three: or 1, 2, 3 etc.")

    elif STATE == state.VETO:
        if message.content.find("!veto") != -1:
            if id not in REROLL_VOTERS:
                REROLL_VOTERS[id] = True
                #print(client.get_user(id).display_name + ' has vetoed!')
                # TODO: DM the person who vetoed so that they know
                await message.author.send("Sir " + nickname + " has vetoed " + FINAL_GAME + "!")
                print('veto percent: ' + str(len(REROLL_VOTERS) / TOTAL_PLAYERS))
                if len(REROLL_VOTERS) / TOTAL_PLAYERS > 0.50: # need majority
                    await message.channel.send("@everyone :dagger::boom::goat: [VETO SUCCEEDED]\nThe will of the gods has wavered! " +
                                            "The roundtable will now propose a new vote...\n\n")
                    # veto succeeded, repeat the process...
                    # TODO: ensure all necessary variables are reset 
                    await message.channel.send('@everyone The selected Game Night will be: **' + FINAL_DATE + '**' )
                    matrix, playerList, ignoreList, totalGames, useEveryone = loadBallotInfo()
                    playerList = createPlayerList(VOTES)
                    TOTAL_PLAYERS = len(playerList)
                    GAME_LIST = generateBallot(matrix, ignoreList, totalGames, useEveryone, playerList)
                    print('FINISHED REROLL, STARTING NEW POLL...')
                    await message.channel.send('@everyone The gods have spoken! Again...\nBelow are your **new** glorious game choices:\n' + 
                                                '[ *Remember* - you may only vote for 1 game using :one:, :two:, :three:, etc. ]')
                    botStr = ''
                    for ii in range(0, len(GAME_LIST)):
                        print(GAME_LIST[ii])
                        botStr += '{0}   **-   {1}**'.format(EMOJI_TEXT[ii], GAME_LIST[ii]) + "\n\n"
                    # post each option that they can vote for
                    await message.channel.send(botStr)

                    print('GAME_LIST[0]: ' + str(GAME_LIST[0]))

                    if len(GAME_LIST) <= 1: # only 1 game on the ballot, skip voting
                        FINAL_GAME = GAME_LIST[0]
                        await message.channel.send(":trumpet: @everyone  HUZZAH! :trumpet:\nOur game has been selected at last!\n" +
                                            "We will be playing **" + FINAL_GAME + "** on the date of **" + FINAL_DATE + "**\n:shield: To arms! :dagger:")
                        # log the results
                        log(FINAL_GAME, FINAL_DATE, GAME_LIST)
                        STATE = state.VETO
                        saveState()
                    else: # more than 1 game on ballot, go to reroll
                        STATE = state.REROLLING
                        saveState()

                    print('FINISHED VETO, STARTING REROLL...')
            else:
                await message.channel.send(":shield:  Stand down, miscreant! :dagger: You have already cast your veto right!")
    elif STATE == state.NONE:
        if message.content.find("!vote") != -1:
            await message.author.send("My apologies good Sir, but the Great Coordinator has already given marching orders...\n" +
                    "If you feel there is an error, you can try !veto to reroll the selected game or contact the Coordinator ({0}) directly to rally a Saracen-scorching party!\n".format(client.get_user(CDNTR_ID)))

client.run(token)

