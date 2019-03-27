#!/usr/bin/python
import gspread
import json
from oauth2client.service_account import ServiceAccountCredentials

SECRET_PATH = '/home/josh/Desktop/gameknight/GameKnight/client_secret.json'
GAME_ID_PATH = '/home/josh/Desktop/gameknight/GameKnight/game_ids.txt'
JSON_PATH = '/home/josh/Desktop/gameknight/GameKnight/data.json' 


def main():
    scope = ['https://spreadsheets.google.com/feeds', 'https://www.googleapis.com/auth/drive']
    creds = ServiceAccountCredentials.from_json_keyfile_name(SECRET_PATH, scope)
    client = gspread.authorize(creds)

    sheet = client.open('Game Knight').sheet1

    data = {}
    data['games'] = []
    data['users'] = []
    matrix = []
    rows = sheet.row_count
    cols = sheet.col_count
    for ii in range(0, rows-1):
        matrix.append([])

    # establish matrix
    matrix = sheet.get_all_values()
    for ii in matrix:
        print (ii) # print rows
    
    with open('data.json', 'w') as outfile:
        json.dump(matrix, outfile)
    

if __name__ == "__main__":
    main()