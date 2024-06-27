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
        public string Filtered { get; set; }

        public Leaderboard()
        {
            
            InitializeComponent();
            this.DataContext = this;

            //Checks if "(Filtered)" should appear on screen
            Filtered = UtilityFunctions.FilterVisibility();

            Loaded += LoadLeaderboard;
        }

        private void LoadLeaderboard(object sender, RoutedEventArgs e)
        {
            //Purpose: Populates main StackPanel with LeaderboardPanel UserControls

            using (StreamReader reader = new StreamReader("..\\..\\..\\Text_Files\\Leaderboard.txt"))
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
            //Purpose: Event-listener for the RefreshLeaderboard Button

            //Calls RefreshLeaderboard (which redoes all the math)
            RefreshLeaderboardBtn.Content = "Note: This may take a second";
            await Task.Delay(20);
            RefreshLeaderboard();

            //Clears theLeaderboard stackPanel, then repopulates it
            theLeaderboard.Children.Clear();
            LoadLeaderboard(sender, e);

            
            RefreshLeaderboardBtn.Content = "Done!";
            await Task.Delay(2000);
            RefreshLeaderboardBtn.Content = "Refresh Leaderboard";

        }


        public static void RefreshLeaderboard()
        {
            //Purpose: Builds the Leaderboard point totals from the ground up
            //Notes: Refreshes filters and incorporates custom ranking system

            //Checks each game and updates in LogofPlayedGames whether it should be filtered out or not
            UtilityFunctions.UpdateFilterInLog();
            
            Dictionary<String, double> newLeaderboard = new Dictionary<string, double>();

            //Reads from LogofPlayedGames line by line
            using (StreamReader reader = new StreamReader("..\\..\\..\\Text_Files\\LogofPlayedGames.txt"))
            {
                string line;
                
                while ((line = reader.ReadLine()) != null)
                {
                    String[] parts = line.Split(",,,");
                    
                    //If it shouldn't be filtered out
                    if (parts[parts.Length - 2] == "true")
                    {

                        //Gather API Information
                        int ID = UtilityFunctions.GetID(parts[0]);
                        (float playtime, float weight) = UtilityFunctions.GetAPIData(ID);

                        //Goes through players in the game played (skips over everything except the names of the players)
                        for (int i = 2; i < int.Parse(parts[1]) + 2; i++)
                        {
                            double points = UtilityFunctions.CalculatePoints(weight, playtime, int.Parse(parts[1]), (i - 1));

                            //checks if person is already in the dictionary
                            //initializes them in the dictionary or simply adds to their ongoing total of points
                            if (!newLeaderboard.ContainsKey(parts[i]))
                                newLeaderboard.Add(parts[i], points);

                            else
                                newLeaderboard[parts[i]] += points;

                        }
                    }
                }
            }

            //Sorts the dictionary so 1st place is the first item in the dictionary
            var sortedDictionary = newLeaderboard.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            //Updates Leaderboard.txt with our new completely remade leaderboard
            using (StreamWriter writer = new StreamWriter("..\\..\\..\\Text_Files\\Leaderboard.txt"))
            {
                //empties out the old leaderboard
                writer.Write(string.Empty);
                int i = 0;

                
                //Adds each person's new point total into Leaderboard.txt
                foreach (var entry in sortedDictionary)
                {
                    i += 1;
                    var key = entry.Key;
                    var value = entry.Value;
                    
                    writer.WriteLine($"{key},{value}");
                }
            }
            
            
            
        }
    }

}



