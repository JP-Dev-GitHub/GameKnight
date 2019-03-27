#!/usr/bin/python
import gspread
from oauth2client.service_account import ServiceAccountCredentials

def main():
    scope = ['https://spreadsheets.google.com/feeds', 'https://www.googleapis.com/auth/drive']
    creds = ServiceAccountCredentials.from_json_keyfile_name('Z:\GameKnight\client_secret.json', scope)
    client = gspread.authorize(creds)

    sheet = client.open('Game Knight').sheet1

    #records = sheet.get_all_records()
    print("got here fam")
    val = sheet.acell('B1').value
    print(val)

if __name__ == "__main__":
    main()