#! /usr/bin/python
import os
import sys
from enum import Enum
import json
import discord


BOT_ID = '4103'

class Color(Enum):
    NEW_RSVP = 1
    W4_RSVP = 2
    NEW_POLL = 3
    W4_VOTE = 4
    

def read_token():
    pwd = os.getcwd() + "\\DiscordBot\\"
    with open(pwd + "token.txt", "r") as f:
        lines = f.readlines()
        return lines[0].strip()

token = read_token()

client = discord.Client()

@client.event
async def on_member_join(member):
    pass
    # for channel in member.server.channels:
    #     await client.send("")


@client.event
async def on_message(message):
    print(message.content)
    name = str(message.author)
    nickname = name[:name.find('#')]
    name = name[name.find('#')+1:]
    if name != BOT_ID: # ignore our own messages
        print(type(nickname))
        await message.channel.send("Thanks for voting " + nickname + "!")
    if message.content.find("!vote") != -1:
        await message.channel.send("Your vote has been submitted successfully!")

client.run(token)

