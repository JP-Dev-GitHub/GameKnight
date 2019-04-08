using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using System.ComponentModel;
using Microsoft.VisualBasic;


namespace GameKnight
{
    public class DataStore
    {
        public List<string> users;
        public List<string> games;
        public List<List<int>> matrix;
    }

    public class RawData
    {
        public List<List<string>> rawData { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Global Information
        public const int UPDATE_DIRECTIVE = 0;
        public const int ADD_USER_DIRECTIVE = 1;
        public const int ADD_GAME_DIRECTIVE = 2;
        public string PATH = @"C:\Users\Joshx\Desktop\GameKnight\";

        public dynamic array;
        public DataStore data;

        public bool pIncludeEveryone = false;
        public bool pUseBallot = false;
        public int pBallotNum = 3;

        public MainWindow()
        {
            InitializeComponent();
            PATH = Directory.GetCurrentDirectory();
            PATH = PATH.Substring(0, PATH.IndexOf("GameKnight") + 10) + "\\";
            Console.WriteLine(PATH);
            UpdateJsonData();
            data = LoadJson(PATH + "data.json");
        }

        // ----------------------------------------------------------- Implementation ----------------------------------------------------------- //

        private bool CheckForDuplicate(string itemName, bool checkGame)
        {
            // refresh JSON data
            DataStore newData = LoadJson(PATH + @"data.json");

            // check if the game is already on the list
            if (checkGame)
            {
                List<string> gameList = newData.games;
                for (int i  = 0; i < gameList.Count; ++i)
                {
                    if(gameList.Contains(itemName))
                    {
                        return true;
                    }
                }

                return false;
            }
            else // were checking for a user
            {
                List<string> userList = newData.users;
                for (int i = 0; i < userList.Count; ++i)
                {
                    Console.WriteLine("Looking for: " + itemName);
                    Console.WriteLine("against: " + userList[i]);

                    if (userList.Contains(itemName))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        private void AddNewGame(object sender, RoutedEventArgs e)
        {
            string newGameName = Interaction.InputBox("Enter the name of a new game that you want to add to the sheet: ", "Add New Game", "New Game Title Here", -1, -1);
            try
            {
                if(CheckForDuplicate(newGameName, true))
                {
                    MessageBox.Show("Game: " + newGameName + " already exists in the spreadsheet!");
                    return;
                }

                string cmd = "python " + PATH + "sheet_handler.py " + ADD_GAME_DIRECTIVE + " " + newGameName;

                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "CMD.exe";
                p.StartInfo.Arguments = "/c " + cmd;
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                MessageBox.Show(output);
                MessageBox.Show("Successfully added");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error! Could not add " + newGameName + ":\n" + ex);
            }
        }

        private void LetsPlay(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("oh boy rick! ");
            MyPopup.IsOpen = true;

            // Fetch all config data
            //UpdateJsonData();
            //SaveJson(PATH + @"\ballot_info.json");

            // Launch python bot
            try
            {
                string cmd = "python " + PATH + @"DiscordBot\gk_bot.py ";

                Console.WriteLine(cmd);
                Process p = new Process();
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.RedirectStandardOutput = false;
                p.StartInfo.FileName = "CMD.exe";
                p.StartInfo.Arguments = "/c " + cmd;
                p.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error! Could not run GK BOT!\n" + ex);
            }
        }

        private void AddNewUser(object sender, RoutedEventArgs e)
        {
            string newUserNickname = Interaction.InputBox("Enter your first name or an alias that you and your friends will recognize:", "Add New Nickname", "Enter your name/nickname.", -1, -1);

            if(newUserNickname == "")
            {
                Console.WriteLine(newUserNickname);
                return;
            }

            string newUserID = Interaction.InputBox("Now enter your Discord ID.\n\nEnable Developer mode in Discord.\n\nRight-click the desired profile and click \"Copy ID\" " 
                + "at the bottom.\n\nPaste the ID here: ", "Add New User ID", "Paste the Discord ID here", -1, -1);

            if (newUserID == "")
            {
                Console.WriteLine(newUserID);
                return;
            }

            if (CheckForDuplicate(newUserID, false))
            {
                MessageBox.Show("ID: " + newUserID + " already exists in the spreadsheet!");
                return;
            }

            try
            {
                string cmd = "python " + PATH + "sheet_handler.py " + ADD_USER_DIRECTIVE + " " + newUserNickname + ':' + newUserID;

                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "CMD.exe";
                p.StartInfo.Arguments = "/c " + cmd;
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                MessageBox.Show(output);
                MessageBox.Show("Successfully added " + newUserID);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error! Could not add " + newUserID + ":\n" + ex);
            }
        }

        private void IncludeEveryone_Checked(object sender, RoutedEventArgs e)
        {
            pIncludeEveryone = (bool)IncludeEveryone_chbx.IsChecked;
        }

        private void UseBallot_chbx_Checked(object sender, RoutedEventArgs e)
        {
            pUseBallot = (bool)UseBallot_chbx.IsChecked;
        }

        private void BallotNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string val = ((ComboBoxItem)BallotNum_cmbx.SelectedItem).Content as string;
            int k = 2;
            if (val != null && Int32.TryParse(val, out k))
                pBallotNum = k;
            else
                pBallotNum = 2; // default
        }

        public DataStore LoadJson(string fp)
        {
            data = new DataStore();
            
            Console.WriteLine("Reading JSON...");
            using (StreamReader r = new StreamReader(fp))
            {
                string json = r.ReadToEnd();
                array = JsonConvert.DeserializeObject(json);
            }

            // convert to C# List of List of strings
            array = array.ToObject<List<List<string>>>();

            // add all users in sheet
            data.games = array[0];

            // add all users in sheet
            data.users = new List<string>(array.Count);
            for (int i = 0; i < array.Count; ++i)
            {
                data.users.Add(array[i][0]);
            }


            foreach (var item in data.games)
            {
                Console.WriteLine(item);
            }

            return data;
        }

        public void SaveJson(string fp)
        {
            Console.WriteLine("Saving JSON data...");

        }

        public void UpdateJsonData()
        {
            string cmd = "python " + PATH + "sheet_handler.py " + UPDATE_DIRECTIVE;
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + cmd;
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}
