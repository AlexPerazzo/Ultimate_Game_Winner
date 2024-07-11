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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ultimate_Game_Winner.Windows;


namespace Ultimate_Game_Winner.UserControls
{
    /// <summary>
    /// Each gameplay individually will be displayed on the Log with the winner, game, # of players, and date recorded
    /// Each individual game with said info will be displayed through this UserControl
    /// </summary>
    public partial class LoggedGamePanelUC : UserControl
    {
        public string PlayerName { get; set; }
        public string GameName { get; set; }

        public string NumPlayers { get; set; }
        public string Date { get; set; }
        public string[] AllInfo { get; set; }

        public bool IsClickable { get; set; } = true;

        public LoggedGamePanelUC()
        {
            InitializeComponent();
            this.DataContext = this;
            
        }




        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsClickable)
            {

            //Purpose Display AdditionalGameplayInfo Window when clicked upon
            AdditionalGameplayInfoWindow additionalGameplayInfo = new AdditionalGameplayInfoWindow(AllInfo);
            
            
            additionalGameplayInfo.Show();
            }
            //This else clause is just for PlayersWinsWindow
            else
            {
                AdditionalPlayerInfoWindow additionalPlayerInfo = new AdditionalPlayerInfoWindow(PlayerName);
                additionalPlayerInfo.Title = $"{PlayerName}'s Games Played";
                additionalPlayerInfo.Show();
            }

        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            

            //Purpose: Add drop shadow when hovered upon
            DropShadowEffect dropShadowEffect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 10,
                ShadowDepth = 5,
                Opacity = 0.8
            };

            LogPanel.Effect = dropShadowEffect;
            
        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            

            //Purpose: Remove drop shadow when not hovered upon
            LogPanel.Effect = null;
            
        }
    }
}
