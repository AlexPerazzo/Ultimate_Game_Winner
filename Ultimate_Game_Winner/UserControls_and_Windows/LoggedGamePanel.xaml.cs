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

namespace Ultimate_Game_Winner.UserControls_and_Windows
{
    /// <summary>
    /// Each gameplay individually will be displayed on the Log with the winner, game, # of players, and date recorded
    /// Each individual game with said info will be displayed through this UserControl
    /// </summary>
    public partial class LoggedGamePanel : UserControl
    {
        public string PlayerName { get; set; }
        public string GameName { get; set; }

        public string NumPlayers { get; set; }
        public string Date { get; set; }
        public string[] AllInfo { get; set; }
        
        public LoggedGamePanel()
        {
            InitializeComponent();
            this.DataContext = this;
            
        }




        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AdditionalGameplayInfo additionalGameplayInfo = new AdditionalGameplayInfo(AllInfo);
            
            
            additionalGameplayInfo.Show();
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            DropShadowEffect dropShadowEffect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 10,
                ShadowDepth = 5,
                Opacity = 0.8
            };

            // Apply the drop shadow effect to the StackPanel
            LogPanel.Effect = dropShadowEffect;
        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            LogPanel.Effect = null;
        }
    }
}
