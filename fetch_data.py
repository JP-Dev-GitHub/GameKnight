#! /usr/bin/python
import os
import gspread
import json
from oauth2client.service_account import ServiceAccountCredentials

### DEFINES ###
global SECRET_PATH
global JSON_PATH


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
    
    for ii in range(0, rows-1):
        matrix.append([])

    # establish matrix
    matrix = sheet.get_all_values()
    for ii in matrix:
        print (ii) # print rows
    
    with open(JSON_PATH, 'w') as outfile:
        json.dump(matrix, outfile)
    

if __name__ == "__main__":
    global SECRET_PATH
    global JSON_PATH
    SECRET_PATH = os.getcwd() + "\client_secret.json"
    JSON_PATH = os.getcwd() + "\data.json"
    print("MY SECRET PATH IS: " + SECRET_PATH)
    main()