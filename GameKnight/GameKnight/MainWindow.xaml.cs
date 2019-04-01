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
            UpdateJsonData();
            data = LoadJson(PATH + "data.json");
        }

        // ----------------------------------------------------------- Implementation ----------------------------------------------------------- //

        private void AddNewGame(object sender, RoutedEventArgs e)
        {
            string newGameName = Interaction.InputBox("Enter new game name: ", "Add New Game", "-", -1, -1);
            try
            {
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
                MessageBox.Show("Error! Could not add " + newGameName + ":\n" + e);
            }
        }

        private void LetsPlay(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("oh boy rick! ");
            MyPopup.IsOpen = true;
        }

        private void AddNewUser(object sender, RoutedEventArgs e)
        {
            string newUser = "sam";
            string cmd = "python " + PATH + "sheet_handler.py " + ADD_USER_DIRECTIVE + " " + newUser;
            //cmd = "python test.py";
            
            // Start the child process.
            Process p = new Process();
            // Redirect the output stream of the child process.
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "CMD.exe";
            p.StartInfo.Arguments = "/c " + cmd;
            p.Start();

            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            MessageBox.Show(output);
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
