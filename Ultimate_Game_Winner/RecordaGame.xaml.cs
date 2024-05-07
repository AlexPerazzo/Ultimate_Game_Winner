using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace Ultimate_Game_Winner
{
    /// <summary>
    /// Interaction logic for RecordaGame.xaml
    /// </summary>
    public partial class RecordaGame : Page
    {
        
        public RecordaGame()
        {
            InitializeComponent();
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Name.Text = "Name of Game";
            NumPlayers.Text = "# of Players";
            First.Text = "1st Place";
            Second.Text = "2nd Place";
            Third.Text = "3rd Place";
        }
    }
}
