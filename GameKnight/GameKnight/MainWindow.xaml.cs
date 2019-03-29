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


namespace GameKnight
{
    public class DataStore
    {
        public List<string> participants;
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
        public dynamic array;
        public DataStore data;

        public bool pIncludeEveryone = false;
        public bool pUseBallot = false;
        public int pBallotNum = 3;

        public MainWindow()
        {
            InitializeComponent();

            data = LoadJson(@"C:\Users\Joshx\Desktop\GameKnight\data.json");
        }

        private void AddNewGame(object sender, RoutedEventArgs e)
        {

        }

        private void LetsPlay(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("oh boy rick! ");
            MyPopup.IsOpen = true;
            pUseBallot = pUseBallot;
        }

        private void AddNewParticipant(object sender, RoutedEventArgs e)
        {
            
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
            if (val != null)
                pBallotNum = Int32.Parse(val);
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

            data.games = array[0];
            //foreach (var item in array)
            //{
            //    var arr = item.ToObject<List<string>>();
            //    Console.WriteLine(item.GetType());
            //}

            //for(int i = 0; i < temp[0].Length; ++i)
            //{
            //    data.games.Add(temp);
            //}


            foreach (var item in data.games)
            {
                Console.WriteLine(item);
                
            }

            return data;
        }


    }
}
