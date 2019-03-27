#!/usr/bin/python
import gspread
from oauth2client.service_account import ServiceAccountCredentials

SECRET_PATH = '/home/josh/Desktop/gameknight/GameKnight/client_secret.json'


def main():
    scope = ['https://spreadsheets.google.com/feeds', 'https://www.googleapis.com/auth/drive']
    creds = ServiceAccountCredentials.from_json_keyfile_name(SECRET_PATH, scope)
    client = gspread.authorize(creds)

    sheet = client.open('Game Knight').sheet1

    #records = sheet.get_all_records()
    val = sheet.acell('B1').value
    print(val)

if __name__ == "__main__":
    main()