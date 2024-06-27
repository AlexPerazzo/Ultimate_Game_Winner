using System;
using System.Collections.Generic;
using System.Dynamic;
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
    /// When the User Clicks on a Person, Information about that person
    /// (including all games they've played in and their placements/points earned during those games)
    /// will be displayed through this Window
    /// </summary>
    public partial class AdditionalPlayerInfo : Window
    {

        string playerName {  get; set; }
        string Title { get; set; }

        public string Filtered {  get; set; }

        public AdditionalPlayerInfo(string PlayerName)
        {
            InitializeComponent();
            this.DataContext = this;
            playerName = PlayerName;
            Filtered = UtilityFunctions.FilterVisibility();
            Loaded += LoadGames;
        }

        private void LoadGames(object sender, RoutedEventArgs e)
        {
            //Purpose: Populates Window

            using (StreamReader reader = new StreamReader("..\\..\\..\\Text_Files\\LogofPlayedGames.txt"))
            {
                //Reads each line from the LogofPlayedGames.txt file and makes a TextBlock out of it
                //Adds that TextBlock to the StackPanel from the xaml page
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    
                    string[] parts = line.Split(",,,");

                    //If it passes the filters, and the player is part of that gameplay: gather and add the info
                    if (parts[parts.Length - 2] == "true" && parts.Contains(playerName))
                    {
                        int numPlayers = int.Parse(parts[1]);
                        var ID = UtilityFunctions.GetID(parts[0]);
                        (float thePlaytime, float theWeight) = UtilityFunctions.GetAPIData(ID);

                        PlacementPointsPanel statsPanel = new PlacementPointsPanel();
                        LoggedGamePanel panel = new LoggedGamePanel();

                        //Swaps various normal parts of LoggedGamePanel with different wanted information
                        //Placement in place of Game Name
                        //Game Name in place of PlayerName
                        //Points and Date stay points and date

                        var placement = Array.IndexOf(parts, playerName) - 1;
                        var points = UtilityFunctions.CalculatePoints(theWeight, thePlaytime, int.Parse(parts[1]), placement);
                        
                        //Panel is main information
                        panel.GameName = $"{UtilityFunctions.AddOrdinal(placement)}";
                        panel.PlayerName = parts[0];
                        panel.NumPlayers = $"{numPlayers} players";
                        panel.Date = parts[parts.Length - 1];
                        panel.Margin = new Thickness(12, 12, 0, 0);
                        panel.HorizontalAlignment= HorizontalAlignment.Left;
                        panel.AllInfo = parts;
                        
                        //Statspanel has points earned displayed on it
                        statsPanel.Points = $"{points}";
                        statsPanel.Margin = new Thickness(12,12,0,0);
                        

                        //Insert Panels Into respective StackPanels
                        StatsPanels.Children.Insert(0, statsPanel);
                        AllGamesPlayed.Children.Insert(0, panel);
                    }
                }
            }
        }
    }
}
