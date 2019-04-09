using System;
using System.Collections;
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
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.ComponentModel;
using Microsoft.VisualBasic;


namespace GameKnight
{
    public class DataStore
    {
        public List<string> users;
        public List<string> nicknames;
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
        // DEFINES
        public const int UPDATE_DIRECTIVE = 0;
        public const int ADD_USER_DIRECTIVE = 1;
        public const int ADD_GAME_DIRECTIVE = 2;
        public const int DISCORD_ID_LENGTH = 18;
        public string PATH = @"C:\path\to\GameKnight\";

        // Global Information
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
            // update JSON Matrix data
            UpdateJsonData();
        }

        // ----------------------------------------------------------- Implementation ----------------------------------------------------------- //
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateJsonData();
            //foreach(var item in data.games)
            //{
            //    Console.WriteLine(item);
            //}
            MessageBox.Show("Refreshed!");
        }

        private bool CheckForDuplicate(string itemName, bool checkGame)
        {
            itemName = itemName.ToLower().Replace(" ", "");

            // check if the game is already on the list
            if (checkGame)
            {
                List<string> gameList = data.games;
                for (int i  = 0; i < gameList.Count; ++i)
                {
                    //Console.WriteLine("Comparing: " + itemName + " with: " + gameList[i].ToLower().Replace(" ", ""));
                    if (itemName == gameList[i].ToLower().Replace(" ", ""))
                        return true;
                }

                return false;
            }
            else // were checking for a user
            {
                List<string> userList = data.users;
                for (int i = 0; i < userList.Count; ++i)
                {
                    //Console.WriteLine("Looking for: " + itemName);
                    //Console.WriteLine("against: " + userList[i]);
                    if (itemName == userList[i].ToLower().Replace(" ", ""))
                        return true;
                }

                return false;
            }
        }

        private void AddNewGame(object sender, RoutedEventArgs e)
        {
            string startString = "New Game Title Here";

            string newGameName = Interaction.InputBox("Enter the name of a new game that you want to add to the sheet: ", "Add New Game", startString, -1, -1);

            // ensure an entry was made
            if (newGameName == "" || newGameName == startString)
                return;

            try
            {
                if (CheckForDuplicate(newGameName, true))
                {
                    MessageBox.Show("Game: " + newGameName + " already exists in the spreadsheet!");
                    return;
                }

                NewGame_btn.IsEnabled = false;

                string cmd = "python " + PATH + "sheet_handler.py " + ADD_GAME_DIRECTIVE + " " + newGameName;

                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "CMD.exe";
                p.StartInfo.Arguments = "/c " + cmd;
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                UpdateJsonData();
                data = LoadJsonDataStore(PATH + "ballot_info.json");
                //Console.WriteLine(output);
                MessageBox.Show("Successfully added " + newGameName);
            
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error! Could not add " + newGameName + ":\n" + ex);
            }

            NewGame_btn.IsEnabled = true;
        }

        private void AddNewUser(object sender, RoutedEventArgs e)
        {
            string startString = "Enter your name/nickname.";

            string newUserNickname = Interaction.InputBox("Enter your first name or an alias that you and your friends will recognize:", "Add New Nickname", startString, -1, -1);

            // ensure an entry was made
            if (newUserNickname == "" || newUserNickname == startString)
                return;

            startString = "Paste the Discord ID here";
            string newUserID = Interaction.InputBox("Now enter your 18-digit Discord ID.\n\nEnable Developer mode in Discord.\n\nRight-click the desired profile and click \"Copy ID\" " 
                + "at the bottom.\n\nPaste the ID here: ", "Add New User ID", startString, -1, -1);

            // ensure an entry was made
            if (newUserID == "" || newUserID == startString)
                return;

            // looking for non-18-digit id
            if (newUserID.Length != DISCORD_ID_LENGTH)
            {
                MessageBox.Show("Incorrect ID length!\n\nDiscord IDs are exactly " + DISCORD_ID_LENGTH + "-digits long.");
                return;
            }

            // check duplicates
            if(CheckForDuplicate(newUserID, false))
            {
                MessageBox.Show("ID: " + newUserID + " already exists in the spreadsheet!");
                return;
            }

            NewUser_btn.IsEnabled = false;

            try
            {
                string cmd = "python " + PATH + "sheet_handler.py " + ADD_USER_DIRECTIVE + " " + newUserNickname + ':' + newUserID;

                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.FileName = "CMD.exe";
                p.StartInfo.Arguments = "/c " + cmd;
                p.Start();

                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
                UpdateJsonData();
                //Console.WriteLine(output);
                MessageBox.Show("Successfully added " + newUserNickname + ":" + newUserID);
                NewUser_btn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                NewUser_btn.IsEnabled = true;
                MessageBox.Show("Error! Could not add " + newUserID + ":\n" + ex);
            }
        }

        private void LetsPlay(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("oh boy rick! Lets play!");
            MyPopup.IsOpen = true;

            // Fetch all config data
            UpdateJsonData();
            //TODO: replace with ballot_info
            SaveJson(PATH + @"test.json");

            // Launch python bot
            //try
            //{
            //    string cmd = "python " + PATH + @"DiscordBot\gk_bot.py ";

            //    Console.WriteLine(cmd);
            //    Process p = new Process();
            //    p.StartInfo.UseShellExecute = true;
            //    p.StartInfo.RedirectStandardOutput = false;
            //    p.StartInfo.FileName = "CMD.exe";
            //    p.StartInfo.Arguments = "/c " + cmd;
            //    p.Start();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Error! Could not run GK BOT!\n" + ex);
            //}
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

        private void CheckGame(object sender, RoutedEventArgs e)
        {
            string gameName = Interaction.InputBox("Enter the name of the game you want to check and click \"Ok\".\n\n", "Check Game Ownership", "Game name here", -1, -1);

            if(gameName.Length <= 1)
            {
                MessageBox.Show("Game not recognized! Please enter a valid game title.");
                return;
            }
            
            List<string> games = data.games;
            List<string> users = data.users;
            List<string> nicknames = data.nicknames;
            List<List<int>> matrix = data.matrix;
            List<string> usersThatOwn = new List<string>();
            List<string> usersDontOwn = new List<string>();
            bool found = false;
            int gameIndex = 0;

            // Validate that the user entered an actual game name
            foreach (string game in games)
            {
                if (game.Contains(gameName))
                {
                    found = true;
                    Console.WriteLine("Index of game: " + gameIndex.ToString());
                    break;
                }
                gameIndex++;   
            }

            if (!found)
            {
                MessageBox.Show("Game not recognized! Please enter a valid game title.");
                return;
            }

            string msgStr = "These users own " + gameName + ":\n";
            //Console.WriteLine("finding name: " + gameName);
            for (int i = 0; i < games.Count(); ++i)
            {
                if (matrix[i][gameIndex] != 1)
                {
                    usersDontOwn.Add(nicknames[i]);
                }
                else
                {
                    usersThatOwn.Add(nicknames[i]);
                    msgStr += nicknames[i] + "\n";
                } 
            }

            msgStr += "\n\n\n\nThe following users do NOT own " + gameName + ":\n";

            foreach (var user in usersDontOwn)
            {
                msgStr += user + "\n";
            }
            // TODO: Create actual popup response box
            MessageBox.Show(msgStr);
        }

        // Gets currently stored DataStore (matrix, game list, users)
        // and adds new variables to the JSON like:
        // EVERYONE, TOTAL_GAMES, STATE, CHANNELS, GK_ROLE, and CDNTR_ROLE
        public void SaveJson(string fp)
        {
            Console.WriteLine("Saving JSON data...");
            UpdateJsonData();
            //var newData = new Dictionary<string, List<object>> { { "Property", "foo" } };

            dynamic x = File.ReadAllText(PATH + "ballot_info.json");
            JObject jo = JObject.Parse(x);

            Dictionary<string, List<int>> matrix = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(jo["MATRIX"].ToString());
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic["MATRIX"] = matrix;
            foreach(var ii in matrix)
                Console.WriteLine(ii);
            var temp = JsonConvert.SerializeObject(dic, Formatting.None);

            using (StreamWriter file = File.CreateText(fp))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, temp);
            }

            var fs = File.Open(fp, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            var sr = new StreamReader(fs);
            string line;

            line = sr.ReadLine();
            line = line.Remove(0,1);
            line =  line.Replace(@"\", "");
            fs.Close();

            File.Delete(fp);
            using (StreamWriter file = File.CreateText(fp))
            {
                file.WriteLine(line);
            }

            Console.WriteLine("Finished!");
        }

        public DataStore LoadJsonDataStore(string fp)
        {
            data = new DataStore();
            Console.WriteLine("Reading JSON...");
            dynamic x = File.ReadAllText(fp);
            JObject jo = JObject.Parse(x);
            // convert JToken data in JSON to usable types
            data.games = JsonConvert.DeserializeObject<List<string>>(jo["MASTER_GAME_LIST"].ToString());

            Dictionary<string, List<int>> matrix = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(jo["MATRIX"].ToString());
            data.matrix = new List<List<int>>();
            data.users = new List<string>();
            data.nicknames = new List<string>();
            foreach (var item in matrix)
            {
                // only get the ID for our purposes
                string s = item.Key.ToString();
                // seperate the values of nickname:discord_id
                data.nicknames.Add(s.Substring(0, s.IndexOf(':')));
                data.users.Add(s.Substring(s.IndexOf(':') + 1));
                data.matrix.Add(item.Value);
            }

            return data;
        }

        // Calls a python script to update the JSON data's:
        // Matrix, game list, and user list
        public void UpdateJsonData()
        {
            Console.WriteLine("UPDATING JSON USING PYTHON...");
            string cmd = "python " + PATH + "sheet_handler.py " + UPDATE_DIRECTIVE;
            Console.WriteLine(cmd);
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + cmd;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();

            // refresh global DataStore
            data = LoadJsonDataStore(PATH + "ballot_info.json");
        }
    }
}
