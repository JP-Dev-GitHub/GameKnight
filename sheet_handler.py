#! /usr/bin/python
import os
import sys
import json
import gspread
from oauth2client.service_account import ServiceAccountCredentials

DIRECTIVE = int(sys.argv[1]) 

def insertNewUser(newUser, sheet):
    cols = sheet.col_count
    newRow = []
    
    for ii in range(0, cols):
        newRow.append(0)
    
    newRow[0] = newUser
    sheet.append_row(newRow)

def insertNewGame(newGame, sheet):
    rows = sheet.row_count+1
    cols = sheet.col_count+1
    
    sheet.resize(rows-1, cols)
    sheet.update_cell(1, cols, newGame)
    blank_cells = sheet.range(2, cols, rows-1, cols)
    for cell in blank_cells:
        cell.value = 0
    sheet.update_cells(blank_cells)

def serializeBallotInfo():
    pass

# only updates the MATRIX and MASTER_GAME_LIST in JSON data
def updateData(sheet, data_path):
    dataExists = os.path.isfile(data_path)
    data = {}
    oldData = {}
    if dataExists:
        with open(data_path, 'r') as f:
            oldData = json.load(f)
            for key in oldData:
                data[key] = oldData[key]
    data['MATRIX'] = {}
    data['MASTER_GAME_LIST'] = []

    # establish matrix
    matrix = []
    matrix = sheet.get_all_values()
    for ii in matrix[0][1:]:
        data['MASTER_GAME_LIST'].append(ii)
    for ii in matrix[1:]:
        #print (ii) # print rows
        data['MATRIX'][ii[0]] = ii[1:]
    
    if dataExists:
        os.remove(data_path)
    
    with open(data_path, 'w') as outfile:
        json.dump(data, outfile)

def getSheet(secret_path):
    scope = ['https://spreadsheets.google.com/feeds', 'https://www.googleapis.com/auth/drive']
    creds = ServiceAccountCredentials.from_json_keyfile_name(secret_path, scope)
    client = gspread.authorize(creds)

    sheet = client.open('Game Knight').sheet1
    return sheet

if __name__ == "__main__":
    global PATH
    x = os.getcwd()
    PATH = x[:x.find('GameKnight')+10] + '\\'
    sheet = getSheet(PATH + "\\client_secret.json")
    data_path = PATH + "\\ballot_info.json"
    
    if DIRECTIVE == 1: # insert a new user into the google sheet
        insertNewUser(sys.argv[2], sheet)
        updateData(sheet, data_path) # make sure to update the stored data after its been altered
    elif DIRECTIVE == 2: # insert a new game into the google sheet
        insertNewGame(sys.argv[2], sheet)
        updateData(sheet, data_path) # make sure to update the stored data after its been altered
    elif DIRECTIVE == 3: # serialize data from c# application
        serializeBallotInfo()
    else:
        with open(PATH + "test.json", 'r') as f:
            oldData = json.load(f)
            for ii in oldData['MATRIX']:
                print(ii)
            print("done")
        #updateData(sheet, data_path)
