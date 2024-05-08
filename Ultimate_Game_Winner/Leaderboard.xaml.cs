using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for Leaderboard.xaml
    /// </summary>
    public partial class Leaderboard : Page
    {
        public Leaderboard()
        {
            InitializeComponent();
            Loaded += LoadLeaderboard;
        }

        private void LoadLeaderboard(object sender, RoutedEventArgs e)
        {
            List<String> leaderboardList = new List<String> {};
            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\Ultimate_Game_Winner\\Ultimate_Game_Winner\\Leaderboard.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    leaderboardList.Add(line);
                }
            }

            foreach (string item in leaderboardList) 
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Text = item;
                textBlock.FontSize = 20;
                textBlock.Margin = new Thickness(12, 12, 0, 0);
                theLeaderboard.Children.Add(textBlock);
            }
        }

    }
}
