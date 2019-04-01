#! /usr/bin/python
import os
import sys
import json
import discord

def read_token():
    with open("token.txt", "r") as f:
        lines = f.readlines()
        return lines[0].strip()

token = read_token()

client = discord.Client()

@client.event
async def on_member_join(member):
    for channel in member.server.channels:
        await client.send("")


@client.event
async def on_message(message):
    print(message.content)
    if message.content.find("!vote") != -1:
        await message.channel.send("Your vote has been submitted successfully!")

client.run(token)

