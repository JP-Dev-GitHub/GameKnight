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
        List<string> participants;
        List<string> games;
        List<List<int>> matrix;
    }

    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DataStore data;

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
        }

        private void AddNewParticipant(object sender, RoutedEventArgs e)
        {
            
        }

        public DataStore LoadJson(string fp)
        {
            DataStore data = new DataStore();
            dynamic array;
            Console.WriteLine("Reading JSON...");
            using (StreamReader r = new StreamReader(fp))
            {
                string json = r.ReadToEnd();
                //List<Item> items = JsonConvert.DeserializeObject<List<Item>>(json);
                array = JsonConvert.DeserializeObject(json);
            }

            
            foreach (var item in array)
            {
                Console.WriteLine(item);
            }

            return data;
        }

    }
}
