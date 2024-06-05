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
using Ultimate_Game_Winner;

namespace Ultimate_Game_Winner.UserControls_and_Windows
{
    /// <summary>
        /// The four different buttons on bottom all navigate the mainFrame to their associated xaml pages
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
            mainFrame.Navigate(new Uri("Main_Pages\\RecordaGame.xaml", UriKind.Relative));
        }

        private void Leaderboardbtn_Click(object sender, RoutedEventArgs e)
        {                        
            mainFrame.Navigate(new Uri("Main_Pages\\Leaderboard.xaml", UriKind.Relative));
        }

        private void Logbtn_Click(object sender, RoutedEventArgs e)
        {            
            mainFrame.Navigate(new Uri("Main_Pages\\TheLog.xaml", UriKind.Relative));
        }

        private void Settingsbtn_Click(object sender, RoutedEventArgs e)
        {
            mainFrame.Navigate(new Uri("Main_Pages\\Settings.xaml", UriKind.Relative));
        }
    }
}
