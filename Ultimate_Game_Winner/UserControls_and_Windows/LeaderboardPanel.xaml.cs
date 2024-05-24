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
    }
}
