#! /usr/bin/python
import os
import sys
from datetime import datetime
import json
import paramiko


# This file exists on the client side (Windows OS) and delivers the config file
# to the server before running the bot so that the configurations are updated

################################   DEFINES   ################################

PATH = ""
SRC_FILE = sys.argv[1]
DEST_FILE = sys.argv[2]

################################   FUNCTIONS   ################################


# READ TOKEN
def scopyFile():
    ssh = paramiko.SSHClient() 
    ssh.load_host_keys(os.path.expanduser(os.path.join("~", ".ssh", "known_hosts")))
    ssh.connect(server, username=username, password=password)
    sftp = ssh.open_sftp()
    sftp.put(localpath, remotepath)
    sftp.close()
    ssh.close()



if __name__ == "__main__":
    global PATH
    x = os.getcwd()
    PATH = x[:x.find('GameKnight')+10] + '\\'
    # Use SRC_FILE to copy the file from surce to destination
    scopyFile()
