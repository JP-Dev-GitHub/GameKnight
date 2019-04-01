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
    
def updateData(sheet, data_path):
    data = {}
    data['games'] = []
    data['users'] = []
    matrix = []
    rows = sheet.row_count
    
    for ii in range(0, rows-1):
        matrix.append([])

    # establish matrix
    matrix = sheet.get_all_values()
    for ii in matrix:
        print (ii) # print rows
    
    dataExists = os.path.isfile(data_path)
    
    if dataExists:
        os.remove(data_path)
    
    with open(data_path, 'w') as outfile:
        json.dump(matrix, outfile)

def getSheet(secret_path):
    scope = ['https://spreadsheets.google.com/feeds', 'https://www.googleapis.com/auth/drive']
    creds = ServiceAccountCredentials.from_json_keyfile_name(secret_path, scope)
    client = gspread.authorize(creds)

    sheet = client.open('Game Knight').sheet1
    return sheet

if __name__ == "__main__":
    sheet = getSheet(os.getcwd() + "\client_secret.json")
    data_path = os.getcwd() + "\data.json"
    
    if DIRECTIVE == 1: # insert a new user into the google sheet
        insertNewUser(sys.argv[2], sheet)
        updateData(sheet, data_path) # make sure to update the stored data after its been altered
    elif DIRECTIVE == 2: # insert a new game into the google sheet
        insertNewGame(sys.argv[2], sheet)
        updateData(sheet, data_path) # make sure to update the stored data after its been altered
    else:
        updateData(sheet, data_path)
