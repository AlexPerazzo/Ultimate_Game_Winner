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
using Ultimate_Game_Winner;

namespace Ultimate_Game_Winner.Main_Pages
{

    public partial class Leaderboard : Page
    {
        /// <summary>
        /// Displays every single person ranked based off points.
        /// Displayed through LeaderboardPanel UserControl in a stack panel
        /// </summary>
        public Leaderboard()
        {
            
            InitializeComponent();
            //Testing
            //Calls LoadLeaderboard when the page is loaded.
            Loaded += LoadLeaderboard;
        }

        private void LoadLeaderboard(object sender, RoutedEventArgs e)
        {
            
            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\Leaderboard.txt"))
            {

                //Reads each line from the text file and makes a TextBlock out of it
                //Adds that TextBlock to the StackPanel from the xaml page
                
                string? line;
                int placement = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    placement++;
                    
                    LeaderboardPanel panel = new LeaderboardPanel();
                    string[] parts = line.Split(",");
                    panel.PlayerName = parts[0];
                    
                    panel.Points = $"{float.Parse(parts[1])}pts";
                    panel.Placement = placement.ToString();

                   
                    panel.Margin = new Thickness(12, 12, 0, 0);
                    theLeaderboard.Children.Add(panel);

                }
            }
            

        }

        private async void RefreshLeaderboardBtn_Click(object sender, RoutedEventArgs e)
        {
            RefreshLeaderboard();
            theLeaderboard.Children.Clear();
            LoadLeaderboard(sender, e);
            RefreshLeaderboardBtn.Content = "Done!";
            await Task.Delay(2000);
            RefreshLeaderboardBtn.Content = "Refresh Leaderboard";

        }


        public static void RefreshLeaderboard()
        {
            

            Dictionary<String, double> newLeaderboard = new Dictionary<string, double>();


            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\LogofPlayedGames.txt"))
            {
                string line;
                var foo = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");
                var selectedGenre = foo[4];
                while ((line = reader.ReadLine()) != null)
                {
                    String[] parts = line.Split(",,,");
                    int ID = UtilityFunctions.GetID(parts[0]);
                    string genre = UtilityFunctions.GetAPIGenre(ID);

                    if (selectedGenre == "All Games" || selectedGenre == UtilityFunctions.FormatGenre(genre))
                    {

                        for (int i = 0; i < parts.Length; i++)
                        {
                            //skips over Game Name, Number of Players, Date, and Additional Comments
                            if (i != 0 && i != 1 && i != (parts.Length - 1) && i != (parts.Length - 2))
                            {
                                (float playtime, float weight) = UtilityFunctions.GetAPIData(ID);
                                double points = RecordaGame.CalculatePoints(weight, playtime, int.Parse(parts[1]), (i - 1));


                                //checks if person is already in the dictionary and adds them accordingly
                                if (!newLeaderboard.ContainsKey(parts[i]))
                                {
                                    newLeaderboard.Add(parts[i], points);
                                }

                                else
                                {
                                    newLeaderboard[parts[i]] += points;
                                }

                            }
                        }
                    }
                }
            }


            var sortedDictionary = newLeaderboard.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            using (StreamWriter writer = new StreamWriter("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\Leaderboard.txt"))
            {

                writer.Write(string.Empty);
                int i = 0;
                foreach (var entry in sortedDictionary)
                {
                    i += 1;
                    //Write onto Leaderboard.txt by iterating through a sorted dictionary and putting their placement, name, and points
                    var key = entry.Key;
                    var value = entry.Value;
                    string writeThis = $"{key},{value}";
                    writer.WriteLine(writeThis);
                }
            }
        }
    }
}
