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
    public partial class TheLog : Page
    {
        public TheLog()
        {
            InitializeComponent();
            Loaded += LoadLog;
        }

        private void LoadLog(object sender, RoutedEventArgs e)
        {
            //List<string> logList = new List<string> {};
            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\LogofPlayedGames.txt"))
            {
                //Reads each line from the LogofPlayedGames.txt file and makes a TextBlock out of it
                //Adds that TextBlock to the StackPanel from the xaml page
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    //logList.Add(line);
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = line;
                    textBlock.FontSize = 20; 
                    textBlock.Margin = new Thickness(12, 12, 0, 0);
                    theLog.Children.Add(textBlock);
                }
            }
            
            
            //foreach (string item in logList){}
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
