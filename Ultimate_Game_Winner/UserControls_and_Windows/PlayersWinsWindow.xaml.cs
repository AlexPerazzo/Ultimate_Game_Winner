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
    /// Interaction logic for PlayersWinsWindow.xaml
    /// </summary>
    public partial class PlayersWinsWindow : Window
    {
        public PlayersWinsWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Loaded += LoadWinsLeaderboard;
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
                        for (int i = 2; i < 4; i++)
                        {

                            if (!playerWins.ContainsKey(parts[i]))
                            {
                                playerWins.Add(parts[i], [0, 0, 0]);
                            }

                            if (i == 2)
                                playerWins[parts[i]][0] += 1;
                            else
                                playerWins[parts[i]][1] += 1;
                        
                            playerWins[parts[i]][2] += 1;

                        }

                    }
                }

                foreach (var item in playerWins)
                {



                    {

                        LoggedGamePanel panel = new LoggedGamePanel();
                        //restructure everything
                        //GameName is now # of 1sts
                        //
                        //NumPlayers is now # of 2nds
                        //Date is now # of total games

                        panel.PlayerName = item.Key;

                        var firstPlaces = item.Value[0];
                        panel.GameName = $"{firstPlaces} 1sts";

                        var secondPlaces = item.Value[1];
                        panel.NumPlayers = $"{secondPlaces} 2nds";

                        var gamesPlayed = item.Value[2];
                        panel.Date = $"{gamesPlayed} Games Played";
                        panel.Margin = new Thickness(12, 12, 0, 0);
                        //panel.AllInfo = parts;


                        panel.IsClickable = false;
                        winsLeaderboard.Children.Insert(0, panel);
                    }
                }
            }



        }
    }
}
