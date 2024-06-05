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

                    RecordaGame recordaGame = new RecordaGame();
                    var ID = recordaGame.GetID(parts[0]);
                    (float thePlaytime, float theWeight) = recordaGame.GetAPIData(ID);
                    PlacementPointsPanel statsPanel = new PlacementPointsPanel();

                    for (var i = 2; i < numPlayers + 2; i++)
                    {
                        if (parts[i] == playerName)
                        {
                            var points = recordaGame.CalculatePoints(theWeight, thePlaytime, int.Parse(parts[1]), i - 1);
                            statsPanel.Placement = $"{i - 1}";
                            statsPanel.Points = $"{points}";
                            statsPanel.Margin = new Thickness(12,12,0,0);
                            StatsPanels.Children.Insert(0, statsPanel);
                            correctPlayer = true;
                            break;
                        }
                    }

                    if (correctPlayer)
                    {

                    //logList.Add(line);
                    LoggedGamePanel panel = new LoggedGamePanel();
                    panel.GameName = parts[0];
                    panel.NumPlayers = $"{numPlayers} players";
                    panel.PlayerName = parts[2];
                    panel.Date = parts[parts.Length - 1];
                    panel.Margin = new Thickness(12, 12, 0, 0);

                    panel.AllInfo = parts;
                    AllGamesPlayed.Children.Insert(0, panel);



                    correctPlayer = false;
                    }
                }
            }


        }
    }
}
