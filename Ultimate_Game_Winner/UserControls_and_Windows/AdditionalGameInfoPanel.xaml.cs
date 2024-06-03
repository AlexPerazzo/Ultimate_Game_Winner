using System;
using System.Collections.Generic;
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
    /// Interaction logic for AdditionalGameInfoPanel.xaml
    /// </summary>
    public partial class AdditionalGameInfoPanel : Window
    {
        private string[] allInfo;
        public AdditionalGameInfoPanel(string[] alltheInfo)
        {
            InitializeComponent();
            //this.DataContext = this;
            allInfo = alltheInfo;
            Loaded += LoadPlayers;

        }


        private void LoadPlayers(object sender, RoutedEventArgs e)
        {

            for (int i = 2; i < allInfo.Length - 2; i++)
            {
                RecordaGame recordaGame = new RecordaGame();
                var ID = recordaGame.GetID(allInfo[0]);
                (float weight, float playtime) = recordaGame.GetAPIData(ID);
                var points = recordaGame.CalculatePoints(weight, playtime, int.Parse(allInfo[1]), i-1);

                LeaderboardPanel playerPanel = new LeaderboardPanel();
                playerPanel.PlayerName = allInfo[i];
                playerPanel.Placement = $"{i-1}";
                playerPanel.Points = $"received {points}";
                PlayersPanel.Children.Add(playerPanel);
            }
        }
    }
}
