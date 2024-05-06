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

namespace Ultimate_Game_Winner
{
    /// <summary>
    /// Interaction logic for BottomButtons.xaml
    /// </summary>
    public partial class BottomButtons : UserControl
    {
        Frame mainFrame = ((MainWindow)Application.Current.MainWindow).MainFrame;
        public BottomButtons()
        {
            InitializeComponent();
            
            
        }
        private void Recordbtn_Click(object sender, RoutedEventArgs e)
        {
            Recordbtn.Content = "Testing";
            mainFrame.Navigate(new Uri("RecordaGame.xaml", UriKind.Relative));
        }

        private void Leaderboardbtn_Click(object sender, RoutedEventArgs e)
        {
            Leaderboardbtn.Content = "Testing2";
            
            mainFrame.Navigate(new Uri("Leaderboard.xaml", UriKind.Relative));
        }

        private void Logbtn_Click(object sender, RoutedEventArgs e)
        {
            Recordbtn.Content = "Get tricked!";
            mainFrame.Navigate(new Uri("TheLog.xaml", UriKind.Relative));
        }
    }
}
