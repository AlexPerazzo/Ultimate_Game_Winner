using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Ultimate_Game_Winner.UserControls_and_Windows;

namespace Ultimate_Game_Winner.Main_Pages
{
    /// <summary>
    /// Contains options for changing ranking system and clearing the log of all recorded games
    /// </summary>
    public partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent();
            
        }


        private void DeleteLog_Click(object sender, RoutedEventArgs e)
        {
            AreYouSure areYouSure = new AreYouSure(true, [], null);
            areYouSure.Show();
        }

        private void Custom_Selected(object sender, RoutedEventArgs e)
        {
            CustomRankItems.Visibility = Visibility.Visible;
        }

        private void Normal_Selected(object sender, RoutedEventArgs e)
        {
            CustomRankItems.Visibility = Visibility.Collapsed;
            //Uncomment once you get the page state to save to where the user left it
            //using (StreamWriter writer = new StreamWriter("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\CustomRankingNumbers.txt"))
            //{
            //    writer.WriteLine("1.0");
            //    writer.WriteLine("1.0");
            //    writer.WriteLine("1.0");

            //}
        }

       

        private async void SetBtn_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter writer = new StreamWriter("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\CustomRankingNumbers.txt"))
            {
                writer.WriteLine(WeightNum.Text);
                writer.WriteLine(PlaytimeNum.Text);
                writer.WriteLine(PlacementNum.Text);

            }
            RecordaGame.RefreshLeaderboard();
            SetBtn.Content = "Done!";
            await Task.Delay(2500);
            SetBtn.Content = "Set";
            
        }

       

        
    }
    
}




    