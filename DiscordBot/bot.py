#! /usr/bin/python
import os
import sys
from enum import Enum
import json
import discord
    
def read_token():
    pwd = os.getcwd() + "\\DiscordBot\\"
    with open(pwd + "token.txt", "r") as f:
        lines = f.readlines()
        return lines[0].strip()

token = read_token()
client = discord.Client()

@client.event
async def on_ready():
    arg = sys.argv[1]
    if arg == "":
        pass
    elif arg == "":
        pass


@client.event
async def on_message(message):
    print(message.content)
    name = str(message.author)
    nickname = name[:name.find('#')]
    name = name[name.find('#')+1:]

client.run(token)

