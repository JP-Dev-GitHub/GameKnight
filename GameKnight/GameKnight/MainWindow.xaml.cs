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
        public const int ADD_USER_DIRECTIVE = 1;
        public const int ADD_GAME_DIRECTIVE = 1;
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
            data = LoadJson(PATH + "data.json");
        }

        private void AddNewGame(object sender, RoutedEventArgs e)
        {

        }

        private void LetsPlay(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("oh boy rick! ");
            MyPopup.IsOpen = true;
        }

        private void AddNewUser(object sender, RoutedEventArgs e)
        {
            string newUser = "sam";
            string cmd = "python" + PATH + "sheet_handler.py " + ADD_USER_DIRECTIVE + newUser;
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

        //string GetPath()
        //{
        //    string parms = @"pwd";
        //    string output = "";
        //    string error = string.Empty;

        //    ProcessStartInfo psi = new ProcessStartInfo("reg.exe", parms);

        //    psi.RedirectStandardOutput = true;
        //    psi.RedirectStandardError = true;
        //    psi.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
        //    psi.UseShellExecute = false;
        //    System.Diagnostics.Process reg;
        //    reg = System.Diagnostics.Process.Start(psi);
        //    using (System.IO.StreamReader myOutput = reg.StandardOutput)
        //    {
        //        output = myOutput.ReadToEnd();
        //    }
        //    using (System.IO.StreamReader myError = reg.StandardError)
        //    {
        //        error = myError.ReadToEnd();

        //    }
        //    return output;
        //}
    }
}
