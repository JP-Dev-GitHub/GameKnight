#!/usr/bin/python
import sys
from oauth2client.service_account import ServiceAccountCredentials

DIRECTIVE = sys.argv[1] 

def insertNewUser(newUser, sheet):
	rows = sheet.row_count
    cols = sheet.col_count



if __name__ == "__main__":
	scope = ['https://spreadsheets.google.com/feeds', 'https://www.googleapis.com/auth/drive']
    creds = ServiceAccountCredentials.from_json_keyfile_name(SECRET_PATH, scope)
    client = gspread.authorize(creds)

    sheet = client.open('Game Knight').sheet1

    if DIRECTIVE == 1: # insert a new user into the google sheet
		insertNewUser(sys.argv[2], sheet)
	elif DIRECTIVE == 2: # insert a new game into the google sheet
		insertNewUser(sys.argv[2], sheet)
	else:
		pass