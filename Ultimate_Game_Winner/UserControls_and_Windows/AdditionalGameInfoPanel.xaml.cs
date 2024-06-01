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
                LeaderboardPanel playerPanel = new LeaderboardPanel();
                playerPanel.PlayerName = allInfo[i];
                playerPanel.Placement = $"{i-1}";
                playerPanel.Points = "received 2.3 pts";
                PlayersPanel.Children.Add(playerPanel);
            }
        }
    }
}
