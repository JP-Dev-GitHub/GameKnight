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
using Xceed.Wpf.Toolkit;
using MessageBox = System.Windows.MessageBox;

namespace GameKnight
{

    public class DataStore
    {
        public List<string> users;
        public List<string> nicknames;
        public List<string> games;
        public List<List<int>> matrix;
        public List<string> ignoreList;
        public List<string> channels;
        public List<string> dates;
        public bool useEveryone;
        public bool useBallot;
        public int totalGames;
        public int state;
        public string coordinatorID;
        public string gameKnightID;
        public string gameKnightChannel;
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
        public const int REMOVE_GAME_DIRECTIVE = 3;
        public const int REMOVE_USER_DIRECTIVE = 4;
        public const int DISCORD_ID_LENGTH = 18;
        public string PATH = @"C:\path\to\GameKnight\";

        // Global Information
        public dynamic array;
        public DataStore data;
        public bool initialized = false;
        public Process gkProc;

        public MainWindow()
        {
            PATH = Directory.GetCurrentDirectory();
            PATH = PATH.Substring(0, PATH.IndexOf("GameKnight") + 10) + "\\";
            // update JSON data
            UpdateJsonData();
            // if we are starting a new poll, clear the dates
            if(data.state == 0)
                data.dates.Clear();
            // Start Window
            InitializeComponent();
            UpdateGUI();
            initialized = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // Close GK process
            if(gkProc != null)
                gkProc.Close();
            // Save when the app closes
            SaveJson(PATH + @"ballot_info.json");
        }

        // ----------------------------------------------------------- Implementation ----------------------------------------------------------- //
        private void UpdateGUI()
        {
            foreach (var ii in data.ignoreList)
            {
                IgnoreList_box.Items.Insert(0, ii);
            }

            foreach (var ii in data.dates)
            {
                DateList_box.Items.Insert(0, ii.Substring(ii.IndexOf(", ") + 2));
            }

            UseBallot_chbx.IsChecked = data.useBallot;
            IncludeEveryone_chbx.IsChecked = data.useEveryone;
            if(data.totalGames > 1)
                BallotNum_cmbx.SelectedItem = BallotNum_cmbx.Items.GetItemAt(data.totalGames-2);
            GameKnightID_box.Text = data.gameKnightID;
            CoordinatorRole_box.Text = data.coordinatorID;
            GameKnightChannel_box.Text = data.gameKnightChannel;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateJsonData();
            MessageBox.Show("Refreshed!");
        }

        private void Kill_Click(object sender, RoutedEventArgs e)
        {
            if (gkProc == null)
            {
                MessageBox.Show("Game Knight Bot is not running!");
                return;
            }
                
            string question = "Are you sure you want to terminate Game Knight BOT?\n\n*WARNING*: All current poll data will be lost!";
            if (MessageBox.Show(question, "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                return;
            if (gkProc != null)
                gkProc.Close();
            MessageBox.Show("Game Knight has stopped running!");
        }

        // Returns the index of the found duplicate, and -1 if there was no duplicate found
        private int CheckForDuplicate(string itemName, string list2check)
        {
            itemName = itemName.ToLower().Replace(" ", "");

            // check if the game is already on the list
            if (list2check == "game")
            {
                List<string> gameList = data.games;
                Console.WriteLine(gameList);
                for (int i  = 0; i < gameList.Count; ++i)
                {
                    string gameName = gameList[i].ToLower().Replace(" ", "");
                    Console.WriteLine("Comparing: " + itemName + " with: " + gameName);
                    if (itemName == gameName)
                        return i;
                }

                return -1;
            }
            else if (list2check == "user") // were checking for a user
            {
                List<string> userList = data.users;
                for (int i = 0; i < userList.Count; ++i)
                {
                    if (itemName == userList[i].ToLower().Replace(" ", ""))
                        return i;
                }

                return -1;
            }
            else if (list2check == "ignore") // were checking ignore list
            {
                List<string> ignoreList = data.ignoreList;
                for (int i = 0; i < ignoreList.Count; ++i)
                {
                    if (itemName == ignoreList[i].ToLower().Replace(" ", ""))
                        return i;
                }

                return -1;
            }
            else 
            {
                System.Windows.MessageBox.Show("Error: No list was chosen for duplicate check!");
                return -1; // no list was checked
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
                int index = CheckForDuplicate(newGameName, "game");
                if (index != -1)
                {
                    MessageBox.Show("Game: " + newGameName + " already exists in the spreadsheet!");
                    return;
                }

                NewGame_btn.IsEnabled = false;

                string cmd = "python " + PATH + "sheet_handler.py " + ADD_GAME_DIRECTIVE + " " + data.games[index];

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
                data = LoadJsonDataStore(PATH + @"ballot_info.json");
                MessageBox.Show("Successfully added " + newGameName);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error! Could not add " + newGameName + ":\n" + ex);
            }

            NewGame_btn.IsEnabled = true;
        }

        private void RemoveGame(object sender, RoutedEventArgs e)
        {
            string startString = "Game Title Here";

            string gameName = Interaction.InputBox("Enter the name of a game that you want to permanently delete from the sheet: ", "Remove Game", startString, -1, -1);

            // ensure an entry was made
            if (gameName == "" || gameName == startString)
                return;

            try
            {
                int index = CheckForDuplicate(gameName, "game");

                if (index < 0)
                {
                    MessageBox.Show(gameName + " was not found on the game list!");
                    return;
                }
                else
                {
                    // destructive action check
                    string question = "*WARNING*\nPerforming this action will delete all ownership data for this game!\n    " +
                        "Are you sure you want to delete" + data.games[index] + "?\n";
                    if (MessageBox.Show(question, "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        return;
                }

                RemoveGame_btn.IsEnabled = false;

                string cmd = "python " + PATH + "sheet_handler.py " + REMOVE_GAME_DIRECTIVE + " " + data.games[index];

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
                data = LoadJsonDataStore(PATH + @"ballot_info.json");
                MessageBox.Show("Successfully deleted " + gameName);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error! Could not remove " + gameName + ":\n" + ex);
            }

            RemoveGame_btn.IsEnabled = true;
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
            if(CheckForDuplicate(newUserID, "user") != -1)
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
                MessageBox.Show("Successfully added " + newUserNickname + ":" + newUserID);
                NewUser_btn.IsEnabled = true;
            }
            catch (Exception ex)
            {
                NewUser_btn.IsEnabled = true;
                MessageBox.Show("Error! Could not add " + newUserID + ":\n" + ex);
            }
        }

        private void RemoveUser(object sender, RoutedEventArgs e)
        {
            string startString = "Enter your name/nickname.";

            string newUserNickname = Interaction.InputBox("Enter your first name or an alias that you and your friends will recognize:", "Add New Nickname", startString, -1, -1);

        }

        private void LetsPlay(object sender, RoutedEventArgs e)
        {
            if(data.dates.Count() < 1)
            {
                MessageBox.Show("You must have at least one date added to the RSVP Option list!");
                return;
            }

            if(data.state != 0)
            {
                string question = "*WARNING* There is already a poll in progress!\n    Do you want to start a new one?\n    (all vote data will be lost)";
                if (MessageBox.Show(question, "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    return;
            }

            data.state = 1;

            // Fetch all config data
            SaveJson(PATH + @"ballot_info.json");
            // Launch python bot
            try
            {
                Console.WriteLine("Launching Discord bot...");
                string cmd = "python " + PATH + @"DiscordBot\gk_bot.py ";

                Console.WriteLine(cmd);
                gkProc = new Process();
                gkProc.StartInfo.UseShellExecute = false;
                gkProc.StartInfo.RedirectStandardInput = true;
                gkProc.StartInfo.RedirectStandardOutput = false;
                gkProc.StartInfo.CreateNoWindow = true;
                gkProc.StartInfo.FileName = "CMD.exe";
                gkProc.StartInfo.Arguments = "/c " + cmd;
                gkProc.Start();

                MessageBox.Show("Game Knight is now active. Dilly dilly!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error! Could not run GK BOT!\n" + ex);
            }
        }

        private void AddIgnore_Click(object sender, RoutedEventArgs e)
        {
            string startString = "Enter Ignored Game Here";
            string newGame = Interaction.InputBox("Enter a game name to be ignored. Games on the Ignore List are not possible candidates " +
                "for a new ballot.", "Ignore New Game", startString, -1, -1);

            // ensure an entry was made
            if (newGame == "" || newGame == startString)
                return;
            int gameIndex = CheckForDuplicate(newGame, "game");
            // check that its on the game list
            if (gameIndex < 0)
            {
                MessageBox.Show(newGame + " is not a recognized game on the google sheet.\nYou can add it using 'Add New Game' button.");
                return;
            }
            // check that its a duplicate
            if (CheckForDuplicate(newGame, "ignore") != -1)
            {
                MessageBox.Show(newGame + " is already on the ignore list.");
                return;
            }

            data.ignoreList.Add(data.games[gameIndex]);
            IgnoreList_box.Items.Insert(0, data.games[gameIndex]);
        }

        private void RemoveIgnore_Click(object sender, RoutedEventArgs e)
        {
            if (IgnoreList_box.SelectedItem == null)
                return;
            string newGame = IgnoreList_box.SelectedItem.ToString();
            data.ignoreList.Remove(newGame);
            IgnoreList_box.Items.Remove(newGame);
        }

        private void AddDate_Click(object sender, RoutedEventArgs e)
        {
            // get current date settings:
            if(DateSelector_slct.SelectedDate == null)
            {
                MessageBox.Show("No date selected.\n\nPlease choose a date from the calendar.");
                return;
            }

            DateTime selectedDT = (DateTime)DateSelector_slct.SelectedDate;
            string dateStr = selectedDT.ToString("dddd, MM/dd");
            string timeStr = ((ComboBoxItem)DateTime_cmbx.SelectedItem).Content as string;
            string ampm = ((ComboBoxItem)AMPM_cmbx.SelectedItem).Content as string;
            dateStr += " at " + timeStr + ampm;
            //string cbxStr = dateStr.Substring(dateStr.IndexOf(", ") + 2);
            string cbxStr = dateStr.Remove(dateStr.IndexOf(":00"), 3);

            data.dates.Add(dateStr);
            DateList_box.Items.Insert(0, cbxStr);
        }

        private void RemoveDate_Click(object sender, RoutedEventArgs e)
        {
            if(DateList_box.SelectedItem == null)
                return;

            int index = DateList_box.SelectedIndex;
            data.dates.RemoveAt(index);
            DateList_box.Items.RemoveAt(index);
        }

        private void IncludeEveryone_Checked(object sender, RoutedEventArgs e)
        {
            if (!initialized)
                return;
            data.useEveryone = (bool)IncludeEveryone_chbx.IsChecked;
        }

        private void UseBallot_chbx_Checked(object sender, RoutedEventArgs e)
        {
            if (!initialized)
                return;
            data.useBallot = (bool)UseBallot_chbx.IsChecked;
        }

        private void BallotNumber_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!initialized)
                return;
            string val = ((ComboBoxItem)BallotNum_cmbx.SelectedItem).Content as string;
            int k = 2;
            if (val != null && Int32.TryParse(val, out k))
                data.totalGames = k;
            else
                data.totalGames = 2; // default
        }

        private void GameKnightID_box_TextChanged(object sender, RoutedEventArgs e)
        {
            if (!initialized)
                return;
            string def = "Ex: 123456789012345678";
            string txt = GameKnightID_box.Text;

            if (txt.Length != DISCORD_ID_LENGTH)
            {
                GameKnightID_box.Text = def;
                return;
            }

            Console.WriteLine(txt);
            data.gameKnightID = txt;
        }

        private void CoordinatorRole_box_TextChanged(object sender, RoutedEventArgs e)
        {
            if (!initialized)
                return;
            string def = "Ex: 123456789012345678";
            string txt = CoordinatorRole_box.Text;

            if(txt.Length != DISCORD_ID_LENGTH)
            {
                CoordinatorRole_box.Text = def;
                return;
            }

            Console.WriteLine(txt);
            data.coordinatorID = txt;
        }

        private void GameKnightChannel_box_TextChanged(object sender, RoutedEventArgs e)
        {
            if (!initialized)
                return;
            var i = e;
            string def = "Ex: 123456789012345678";
            string txt = GameKnightChannel_box.Text;

            if (txt.Length != DISCORD_ID_LENGTH)
            {
                CoordinatorRole_box.Text = def;
                return;
            }
            
            data.gameKnightChannel = txt;
        }

        private void CheckGame(object sender, RoutedEventArgs e)
        {
            string gameName = Interaction.InputBox("Enter the name of the game you want to check and click \"Ok\".\n\n", "Check Game Ownership", "Game name here", -1, -1);
            
            if (gameName.Length <= 1)
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
                if (game.ToLower().Replace(" ", "") == gameName)
                {
                    found = true;
                    break;
                }
                gameIndex++;   
            }

            if (!found)
            {
                MessageBox.Show("Game not recognized! Please enter a valid game title.");
                return;
            }

            string msgStr = "These users own " + games[gameIndex] + ":\n";
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

            msgStr += "\n\n\n\nThe following users do NOT own " + games[gameIndex] + ":\n";

            foreach (var user in usersDontOwn)
            {
                msgStr += user + "\n";
            }
            // TODO: Create actual popup response box
            MessageBox.Show(msgStr);
        }

        // Gets currently stored DataStore (matrix, game list, users)
        // and adds new variables to the JSON like:
        // EVERYONE, TOTAL_GAMES, STATE, CHANNELS, GK_ID, and CDNTR_ROLE
        public void SaveJson(string fp)
        {
            Console.WriteLine("Saving JSON data...");
            //UpdateJsonData();

            dynamic x = File.ReadAllText(fp);
            JObject jo = JObject.Parse(x);

            Dictionary<string, List<int>> matrix = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(jo["MATRIX"].ToString());
            Dictionary<string, object> dic = new Dictionary<string, object>();
            // ADD MATRIX
            dic["MATRIX"] = matrix;
            dic["TOTAL_GAMES"] = data.useBallot ? data.totalGames : 1;
            dic["IGNORE_LIST"] = data.ignoreList;
            dic["DATE_LIST"] = data.dates;
            dic["MASTER_GAME_LIST"] = data.games;
            dic["STATE"] = data.state;
            dic["CHANNELS"] = data.channels;
            dic["GK_ID"] = data.gameKnightID;
            dic["CDNTR_ROLE"] = data.coordinatorID;
            dic["GK_CHANNEL"] = data.gameKnightChannel;
            dic["USE_BALLOT"] = data.useBallot.ToString();
            dic["EVERYONE"] = data.useEveryone.ToString();


            // CONVERT JObject
            var cjo = JsonConvert.SerializeObject(dic, Formatting.None);

            using (StreamWriter file = File.CreateText(fp))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, cjo);
            }

            var fs = File.Open(fp, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            var sr = new StreamReader(fs);
            string line;
            line = sr.ReadLine();

            if (line[0] == '\"')
                line = line.Remove(0, 1);
            if (line[line.Length - 1] == '\"')
                line = line.Remove(line.Length - 1, 1);
            line = line.Replace(@"\", "");
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
            if (!File.Exists(fp))
            {
                MessageBox.Show("Error! No ballot info JSON.");
                // TODO: Create new JSON Default File
                System.Windows.Application.Current.Shutdown();
                return new DataStore();
            }

            data = new DataStore();
            Console.WriteLine("Reading JSON...");
            dynamic x = File.ReadAllText(fp);
            JObject jo = JObject.Parse(x);
            // convert JToken data in JSON to usable types
            Dictionary<string, List<int>> matrix = JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(jo["MATRIX"].ToString());
            data.totalGames = JsonConvert.DeserializeObject<int>(jo["TOTAL_GAMES"].ToString());
            data.useEveryone = Convert.ToBoolean(JsonConvert.DeserializeObject<string>(jo["EVERYONE"].ToString().ToLower()));
            data.useBallot = Convert.ToBoolean(JsonConvert.DeserializeObject<string>(jo["USE_BALLOT"].ToString().ToLower()));
            data.ignoreList = JsonConvert.DeserializeObject<List<string>>(jo["IGNORE_LIST"].ToString());
            data.games = JsonConvert.DeserializeObject<List<string>>(jo["MASTER_GAME_LIST"].ToString());
            data.dates = JsonConvert.DeserializeObject<List<string>>(jo["DATE_LIST"].ToString());
            data.state = JsonConvert.DeserializeObject<int>(jo["STATE"].ToString());
            data.channels = JsonConvert.DeserializeObject<List<string>>(jo["CHANNELS"].ToString());
            data.gameKnightID = JsonConvert.DeserializeObject<string>(jo["GK_ID"].ToString());
            data.coordinatorID = JsonConvert.DeserializeObject<string>(jo["CDNTR_ROLE"].ToString());
            data.gameKnightChannel = JsonConvert.DeserializeObject<string>(jo["GK_CHANNEL"].ToString());

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
