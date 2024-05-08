using System;
using System.Collections.Generic;
using System.IO;
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

namespace Ultimate_Game_Winner
{
    /// <summary>
    /// Interaction logic for TheLog.xaml
    /// </summary>
    public partial class TheLog : Page
    {
        public TheLog()
        {
            InitializeComponent();
            Loaded += MyPage_Loaded;
        }

        private void MyPage_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> logList = new List<string> {};
            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\Ultimate_Game_Winner\\Ultimate_Game_Winner\\LogofPlayedGames.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    logList.Add(line);
                }
            }
            // Put the code here to iterate through the list and display it
            
            foreach (string item in logList)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = item;
                textBlock.FontSize = 20; 
                textBlock.Margin = new Thickness(12, 12, 0, 0);
                theLog.Children.Add(textBlock);
            }
        }

        private void DeleteLog_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\Ultimate_Game_Winner\\Ultimate_Game_Winner\\LogofPlayedGames.txt"))
            {
                writer.Write(string.Empty);
            }
            DeleteLog.Content = "Refresh to Load";
        }
    }
}
