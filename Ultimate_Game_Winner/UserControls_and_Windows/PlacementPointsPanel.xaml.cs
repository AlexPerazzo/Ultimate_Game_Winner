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
    /// Meant to work in tandem with LoggedGamePanels when shown in the AdditionalPlayerInfo Windows
    /// The LoggedGamePanel will pop up with its normal info. 
    /// This UserControl will pop up next to it with info about that player's placement and points earned in each game.
    /// </summary>
    public partial class PlacementPointsPanel : UserControl
    {
        public PlacementPointsPanel()
        {
            InitializeComponent();
        }
    }
}
