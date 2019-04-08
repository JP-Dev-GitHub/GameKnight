#! /usr/bin/python
import os
import sys
import json
import gspread
from oauth2client.service_account import ServiceAccountCredentials

DIRECTIVE = int(sys.argv[1])
PATH = ''

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

# updates the matrix only
def updateData(sheet, data_path):
    matrix = []
    rows = sheet.row_count

    # check if we have older data to work with
    dataExists = os.path.isfile(data_path)
    if dataExists:
        with open(data_path, 'r') as old:
            new_data = json.load(old)
            print('PRINTING OLD MATRIX: ')
            print(new_data['MATRIX'])
    else:
        print('WARNING: no file found at ' + data_path)
        return

    for ii in range(0, rows-1):
        matrix.append([])

    # GENERATE MATRIX
    matrix = sheet.get_all_values()
    # GET GAME LIST
    new_data['MASTER_GAME_LIST'] = matrix[0][1:]

    for ii in matrix[1:]:
        c = ii[1:]
        s = ii[0][:ii[0].find(':')]
        new_data['MATRIX'][ii[0]] = c # get all users ids and use them as keys

    # delete old json data
    os.remove(data_path)
    # replace the old data with new data
    with open(data_path, 'w') as outfile:
        json.dump(new_data, outfile)

def getSheet(secret_path):
    scope = ['https://spreadsheets.google.com/feeds', 'https://www.googleapis.com/auth/drive']
    creds = ServiceAccountCredentials.from_json_keyfile_name(secret_path, scope)
    client = gspread.authorize(creds)

    sheet = client.open('Game Knight').sheet1
    return sheet

if __name__ == "__main__":
    PATH = os.getcwd()
    PATH = PATH[:PATH.find('GameKnight')+10]
    sheet = getSheet(PATH + "\\client_secret.json")
    data_path = os.getcwd() + "\\ballot_info.json"
    
    if DIRECTIVE == 1: # insert a new user into the google sheet
        insertNewUser(sys.argv[2], sheet)
        updateData(sheet, data_path) # make sure to update the stored data after its been altered
    elif DIRECTIVE == 2: # insert a new game into the google sheet
        insertNewGame(sys.argv[2], sheet)
        updateData(sheet, data_path) # make sure to update the stored data after its been altered
    else:
        updateData(sheet, data_path)
