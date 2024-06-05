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
    /// Interaction logic for LeaderboardPanel.xaml
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
            DropShadowEffect dropShadowEffect = new DropShadowEffect
            {
                Color = Colors.Black,
                ShadowDepth = 6,
                
            };

            // Apply the drop shadow effect to the StackPanel
            LeaderPanel.Effect = dropShadowEffect;
        }

        private void Rectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            DropShadowEffect dropShadowEffect = new DropShadowEffect
            {
                Color = Colors.Gray,
            };
            LeaderPanel.Effect = dropShadowEffect;
        }

        private void LeaderPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AdditionalPlayerInfoPanel additionalPlayerInfoPanel = new AdditionalPlayerInfoPanel(PlayerName);
            additionalPlayerInfoPanel.Title = $"{PlayerName}'s Games Played";


            additionalPlayerInfoPanel.Show();
        }
    }
}
