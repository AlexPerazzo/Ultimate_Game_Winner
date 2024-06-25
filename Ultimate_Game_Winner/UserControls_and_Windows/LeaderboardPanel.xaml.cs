using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ultimate_Game_Winner.UserControls_and_Windows
{
    /// <summary>
    /// Each person individually will be displayed on the Leaderboard with their ranking and point totals
    /// Each individual person with said info will be displayed through this UserControl
    /// </summary>
    public partial class LeaderboardPanel : UserControl
    {
        public LeaderboardPanel()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string PlayerName { get; set; }
        public string Placement { get; set; }
        public string Points { get; set; }

        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            //Purpose: Add Dropshadow when hovering over
            DropShadowEffect dropShadowEffect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 5,
                ShadowDepth = 6,
                
            };

            LeaderPanel.Effect = dropShadowEffect;
        }

        private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            //Purpose: Turn Dropshadow back to normal when mouse leaves
            DropShadowEffect dropShadowEffect = new DropShadowEffect
            {
                Color = Colors.Gray,
            };
            LeaderPanel.Effect = dropShadowEffect;
        }

        private void LeaderPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Purpose: Displays AddtionalPlayerInfo Window when clicked upon
            AdditionalPlayerInfo additionalPlayerInfo = new AdditionalPlayerInfo(PlayerName);
            additionalPlayerInfo.Title = $"{PlayerName}'s Games Played";
            additionalPlayerInfo.Show();
        }
    }
}
