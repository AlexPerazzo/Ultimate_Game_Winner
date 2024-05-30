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
using Ultimate_Game_Winner.UserControls_and_Windows;

namespace Ultimate_Game_Winner.Main_Pages
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
            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\LogofPlayedGames.txt"))
            {
                //Reads each line from the LogofPlayedGames.txt file and makes a TextBlock out of it
                //Adds that TextBlock to the StackPanel from the xaml page
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    //logList.Add(line);
                    LoggedGamePanel panel = new LoggedGamePanel();
                    string[] parts = line.Split(",,,");
                    panel.GameName = parts[0];
                    var numPlayers = parts.Length - 3;
                    panel.NumPlayers = $"{numPlayers} players";
                    panel.PlayerName = parts[2];
                    panel.Date = parts[parts.Length - 1];
                    panel.Margin = new Thickness(12, 12, 0, 0);


                    theLog.Children.Add(panel);
                }
            }


            
        }


    }
}
