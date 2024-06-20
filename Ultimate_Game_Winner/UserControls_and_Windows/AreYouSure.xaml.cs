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
using System.Windows.Shapes;
using Ultimate_Game_Winner.Main_Pages;

namespace Ultimate_Game_Winner.UserControls_and_Windows
{
    /// <summary>
    /// This Window will ask a confirmation for deleting all the games that have been recorded since it's a major decision
    /// </summary>
    public partial class AreYouSure : Window
    {
        bool deleteAll;
        string[] allInfo;
        Window oldWindow;
        

        public AreYouSure(bool DeleteAll, string[] AllInfo, Window OldWindow)
        {

            InitializeComponent();
            deleteAll = DeleteAll;
            allInfo = AllInfo;
            oldWindow = OldWindow;
            this.DataContext = this;
            
        }

        private async void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            //This function is used in one of two places:
            //1. Delete all in Settings. This is where the first part of the if clause comes in
            //2. Delete one gameplay. This is where the else part of the if clause comes in

            if (deleteAll)
            {

                //replaces anything in LogofPlayedGames.txt and Leaderboard.txt with an empty line.
                using (StreamWriter writer = new StreamWriter("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\LogofPlayedGames.txt"))
                {
                    writer.Write(string.Empty);
                }
                using (StreamWriter writer = new StreamWriter("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\Leaderboard.txt"))
                {
                    writer.Write(string.Empty);
                }
                var textFile = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");
                using (StreamWriter writer = new StreamWriter("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt"))
                {
                    writer.WriteLine(textFile[0]);
                    writer.WriteLine(textFile[1]);
                    writer.WriteLine(textFile[2]);
                    writer.WriteLine(textFile[3]);
                    writer.WriteLine("All Genres");
                    writer.WriteLine("All Genres");
                }
                ConfirmBtn.Content = "Success!";
                await Task.Delay(500);
                this.Close();
            }

            //Delete one gameplay
            else
            {
                
                string filePath = "C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\LogofPlayedGames.txt";
              
                List<string> lines = new List<string>(File.ReadAllLines(filePath));

                lines.Remove(string.Join(",,,", allInfo));

                File.WriteAllLines(filePath, lines);

                Leaderboard.RefreshLeaderboard();
                
                ConfirmBtn.Content = "Success!";
                await Task.Delay(500);

                
                this.Close();
                oldWindow.Close();
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
