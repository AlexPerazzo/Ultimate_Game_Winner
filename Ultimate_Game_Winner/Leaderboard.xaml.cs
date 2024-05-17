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
            //Calls LoadLeaderboard when the page is loaded.
            Loaded += LoadLeaderboard;
        }

        private void LoadLeaderboard(object sender, RoutedEventArgs e)
        {
            //List<String> leaderboardList = new List<String> {};
            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Leaderboard.txt"))
            {

                //Reads each line from the text file and makes a TextBlock out of it
                //Adds that TextBlock to the StackPanel from the xaml page
                
                string line;
                int count = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    count ++;
                    //leaderboardList.Add(line);

                    TextBlock textBlock = new TextBlock();
                    string[] parts = line.Split(",");

                    textBlock.Text = $"${count} {parts[0]} with {parts[1]} points";
                    textBlock.FontSize = 20;
                    textBlock.Margin = new Thickness(12, 12, 0, 0);
                    theLeaderboard.Children.Add(textBlock);

                }
            }
            

        }

    }
}
