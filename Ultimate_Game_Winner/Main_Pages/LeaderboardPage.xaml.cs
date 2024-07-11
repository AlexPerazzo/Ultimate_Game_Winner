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
using Ultimate_Game_Winner.UserControls;
using Ultimate_Game_Winner;
using System.Collections;

namespace Ultimate_Game_Winner.Main_Pages
{

    public partial class LeaderboardPage : Page
    {
        /// <summary>
        /// Displays every single person ranked based off points.
        /// Displayed through LeaderboardPanel UserControl in a stack panel
        /// </summary>
        public string Filtered { get; set; }

        public LeaderboardPage()
        {
            
            InitializeComponent();
            this.DataContext = this;

            //Checks if "(Filtered)" should appear on screen
            Filtered = UtilityFunctions.FilterVisibility();

            Loaded += LoadLeaderboard;
            Loaded += ResizePage;
        }
        private void LoadLeaderboard(object sender, RoutedEventArgs e)
        {
            string[] savedSettings = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");
            if (savedSettings[3] == "# of Wins")
                LoadWinsLeaderboard(sender, e);
            else
                LoadNormalLeaderboard(sender, e);
        }
        private void LoadNormalLeaderboard(object sender, RoutedEventArgs e)
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
                    
                    LeaderboardPanelUC panel = new LeaderboardPanelUC();
                    string[] parts = line.Split(",");
                    panel.PlayerName = parts[0];
                    
                    panel.Points = $"{float.Parse(parts[1])}pts";
                    panel.Placement = placement.ToString();

                   
                    panel.Margin = new Thickness(12, 12, 0, 0);
                    theLeaderboard.Children.Add(panel);

                }
            }
            

        }

        private void LoadWinsLeaderboard(object sender, RoutedEventArgs e)
        {

            using (StreamReader reader = new StreamReader("..\\..\\..\\Text_Files\\LogofPlayedGames.txt"))
            {
                //Purpose: Populates StackPanel with LoggedGamePanel User Controls

                //Reads each line from the LogofPlayedGames.txt file and makes a TextBlock out of it
                //Adds that TextBlock to the StackPanel from the xaml page

                var playerWins = new Dictionary<string, int[]>();
                string? line;
                while ((line = reader.ReadLine()) != null)
                {

                    string[] parts = line.Split(",,,");
                    if (parts[parts.Length - 2] == "true")
                    {
                        for (int i = 2; i < int.Parse(parts[1]) + 2; i++)
                        {

                            if (!playerWins.ContainsKey(parts[i]))
                            {
                                playerWins.Add(parts[i], [0, 0, 0]);
                            }

                            if (i == 2)
                                playerWins[parts[i]][0] += 1;
                            else if (i == 3)
                                playerWins[parts[i]][1] += 1;

                            playerWins[parts[i]][2] += 1;

                        }

                    }
                }

                
                //sort Dictionary for Leaderboard
                var sortedDictionary = playerWins
                .OrderByDescending(kv => kv.Value[0])    // Sort by the first int (descending)
                .ThenByDescending(kv => kv.Value[1])     // Sort by the second int (descending)
                .ThenBy(kv => kv.Value[2])               // Sort by the third int (ascending)
                .ToDictionary(kv => kv.Key, kv => kv.Value);


                foreach (var item in sortedDictionary)
                {



                    {

                        LoggedGamePanelUC panel = new LoggedGamePanelUC();
                        //restructure everything
                        //GameName is now # of 1sts
                        //
                        //NumPlayers is now # of 2nds
                        //Date is now # of total games

                        panel.PlayerName = item.Key;

                        var firstPlaces = item.Value[0];
                        panel.GameName = $"{firstPlaces} Wins";

                        var secondPlaces = item.Value[1];
                        panel.NumPlayers = $"{secondPlaces} Runner-ups";

                        var gamesPlayed = item.Value[2];
                        panel.Date = $"{gamesPlayed} Games Played";
                        panel.Margin = new Thickness(12, 12, 0, 0);
                        //panel.AllInfo = parts;


                        panel.IsClickable = false;
                        theLeaderboard.Children.Add(panel);
                        //theLeaderboard.Children.Insert(0, panel);
                    }
                }
            }



        }

        private void ResizePage(object sender, RoutedEventArgs e)
        {
            // Get the parent window
            Window parentWindow = Window.GetWindow(this);

            parentWindow.SizeToContent = SizeToContent.Width;
            if (parentWindow.ActualWidth < 800)
            {
                parentWindow.Width = 800;
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
            Dictionary<String, (float, float)> repeatedGameInfo = new Dictionary<string, (float, float)>();

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
                        float playtime;
                        float weight;

                        //If we've already done the API call once, just look up the information in dictionary
                        if (repeatedGameInfo.ContainsKey(parts[0]))
                        {
                            (playtime, weight) = repeatedGameInfo[parts[0]];
                        }
                        else
                        {
                            //Gather API Information
                            int ID = UtilityFunctions.GetID(parts[0]);
                            (playtime, weight) = UtilityFunctions.GetAPIData(ID);
                            repeatedGameInfo.Add(parts[0], (playtime, weight));
                        }


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



