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

        public AdditionalPlayerInfo(string PlayerName)
        {
            InitializeComponent();
            this.DataContext = this;
            playerName = PlayerName;
            Loaded += LoadGames;
        }

        private void LoadGames(object sender, RoutedEventArgs e)
        {

            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\LogofPlayedGames.txt"))
            {
                //Reads each line from the LogofPlayedGames.txt file and makes a TextBlock out of it
                //Adds that TextBlock to the StackPanel from the xaml page
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    var correctPlayer = false;
                    string[] parts = line.Split(",,,");
                    var numPlayers = parts.Length - 4;

                    
                    var ID = RecordaGame.GetID(parts[0]);
                    (float thePlaytime, float theWeight) = RecordaGame.GetAPIData(ID);
                    PlacementPointsPanel statsPanel = new PlacementPointsPanel();
                    

                    for (var i = 2; i < numPlayers + 2; i++)
                    {
                        if (parts[i] == playerName)
                        {
                            LoggedGamePanel panel = new LoggedGamePanel();

                            //Normal:
                            //panel.GameName = parts[0];
                            //panel.PlayerName = parts[2];
                            //Weird:

                            //
                            panel.GameName = $"{RecordaGame.AddOrdinal(i - 1)}";

                            //Game Name in place of PlayerName
                            panel.PlayerName = parts[0];
                            var points = RecordaGame.CalculatePoints(theWeight, thePlaytime, int.Parse(parts[1]), i - 1);
                            statsPanel.Placement = $"{i - 1}";
                            statsPanel.Points = $"{points}";
                            statsPanel.Margin = new Thickness(12,12,0,0);
                            StatsPanels.Children.Insert(0, statsPanel);
                            correctPlayer = true;
                            panel.NumPlayers = $"{numPlayers} players";
                            panel.Date = parts[parts.Length - 1];
                            panel.Margin = new Thickness(12, 12, 0, 0);
                            panel.HorizontalAlignment= HorizontalAlignment.Left;
                            panel.AllInfo = parts;
                            AllGamesPlayed.Children.Insert(0, panel);
                            break;
                        }
                    }

                    if (correctPlayer)
                    {

                    //logList.Add(line);





                    correctPlayer = false;
                    }
                }
            }


        }
    }
}
