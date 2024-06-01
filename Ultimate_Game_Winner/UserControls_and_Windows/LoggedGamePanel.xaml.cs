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
    /// Interaction logic for LoggedGamePanel.xaml
    /// </summary>
    public partial class LoggedGamePanel : UserControl
    {
        public LoggedGamePanel()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string PlayerName { get; set; }
        public string GameName { get; set; }

        public string NumPlayers { get; set; }
        public string Date { get; set; }
        public string[] AllInfo { get; set; }



        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AdditionalGameInfoPanel additionalGameInfoPanel = new AdditionalGameInfoPanel(AllInfo);
            additionalGameInfoPanel.Show();
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            DropShadowEffect dropShadowEffect = new DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 10,
                ShadowDepth = 5,
                Opacity = 0.5
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
