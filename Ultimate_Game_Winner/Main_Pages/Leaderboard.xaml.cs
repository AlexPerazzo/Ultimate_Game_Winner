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

            Filtered = UtilityFunctions.FilterVisibility();

            //Calls LoadLeaderboard when the page is loaded.
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

            //Calls RefreshLeaderboard (which redoes all the math), clears theLeaderboard stackPanel, then repopulates it
            RefreshLeaderboardBtn.Content = "Note: This may take a second";
            await Task.Delay(20);
            RefreshLeaderboard();

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


            UtilityFunctions.UpdateFilterInLog();
            
            Dictionary<String, double> newLeaderboard = new Dictionary<string, double>();


            using (StreamReader reader = new StreamReader("..\\..\\..\\Text_Files\\LogofPlayedGames.txt"))
            {
                string line;
                
                while ((line = reader.ReadLine()) != null)
                {
                    String[] parts = line.Split(",,,");
                    
                    
                    if (parts[parts.Length - 2] == "true")
                    {

                        int ID = UtilityFunctions.GetID(parts[0]);
                        (float playtime, float weight) = UtilityFunctions.GetAPIData(ID);

                        //skips over Game Name, Number of Players, Date, Additional Comments, filterBool, and Group
                        for (int i = 2; i < int.Parse(parts[1]) + 2; i++)
                        {
                            double points = UtilityFunctions.CalculatePoints(weight, playtime, int.Parse(parts[1]), (i - 1));

                            //checks if person is already in the dictionary and adds them accordingly
                            if (!newLeaderboard.ContainsKey(parts[i]))
                                newLeaderboard.Add(parts[i], points);

                            else
                                newLeaderboard[parts[i]] += points;

                        }
                    }
                }
            }


            var sortedDictionary = newLeaderboard.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            using (StreamWriter writer = new StreamWriter("..\\..\\..\\Text_Files\\Leaderboard.txt"))
            {

                writer.Write(string.Empty);
                int i = 0;
                foreach (var entry in sortedDictionary)
                {
                    i += 1;
                    //Write onto Leaderboard.txt by iterating through a sorted dictionary and putting their placement, name, and points
                    var key = entry.Key;
                    var value = entry.Value;
                    
                    writer.WriteLine($"{key},{value}");
                }
            }
            
            
            
        }
    }

}



