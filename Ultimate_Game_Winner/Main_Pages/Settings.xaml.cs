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
        private bool weightIsAGo = true;
        private bool playtimeIsAGo = true;
        private bool placementIsAGo = true;



        public Settings()
        {
            InitializeComponent();
            Loaded += LoadSettings;
        }

        private void LoadSettings(object sender, RoutedEventArgs e)
        {
            string[] values;
            values = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");
            WeightNum.Text = values[0];
            PlaytimeNum.Text = values[1];
            PlacementNum.Text = values[2];
            RankingSysBox.Text = values[3];
        }

        private void DeleteLog_Click(object sender, RoutedEventArgs e)
        {
            AreYouSure areYouSure = new AreYouSure(true, [], null);
            areYouSure.Show();
        }

        private void Custom_Selected(object sender, RoutedEventArgs e)
        {
            string[] values;
            values = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");
            WeightNum.Text = values[0];
            PlaytimeNum.Text = values[1];
            PlacementNum.Text = values[2];
            WeightNum_LostFocus(sender, e);
            PlaytimeNum_LostFocus(sender, e);
            PlacementNum_LostFocus(sender, e);
            CustomRankItems.Visibility = Visibility.Visible;
        }

        private void Normal_Selected(object sender, RoutedEventArgs e)
        {
            CustomRankItems.Visibility = Visibility.Collapsed;

            var textFileToChange = File.ReadAllText("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");
            if (textFileToChange != $"1.0\r\n1.0\r\n1.0\r\nNormal\r\n")
            {
                using (StreamWriter writer = new StreamWriter("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt"))
                {
                    writer.WriteLine("1.0");
                    writer.WriteLine("1.0");
                    writer.WriteLine("1.0");
                    writer.WriteLine("Normal");
                }
                RecordaGame.RefreshLeaderboard();
                
            }
        }



        private async void SetBtn_Click(object sender, RoutedEventArgs e)
        {
            if (weightIsAGo && playtimeIsAGo && placementIsAGo)
                {

                var weight = WeightNum.Text;
                var playtime = PlaytimeNum.Text;
                var placement = PlacementNum.Text;

                var textFileToChange = File.ReadAllText("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");
                if (textFileToChange == $"{weight}\r\n{playtime}\r\n{placement}\r\nCustom\r\n" || textFileToChange == $"{weight}\r\n{playtime}\r\n{placement}\r\nNormal\r\n")
                {
                    SetBtn.Content = "No change detected";
                    await Task.Delay(2000);
                    SetBtn.Content = "Set";
                }
                else
                {
                    SetBtn.Content = "Note: This may take a second";
                    await Task.Delay(10);
                    using (StreamWriter writer = new StreamWriter("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt"))
                    {
                        writer.WriteLine(weight);
                        writer.WriteLine(playtime);
                        writer.WriteLine(placement);
                        writer.WriteLine("Custom");
                    }
                    RecordaGame.RefreshLeaderboard();
                    SetBtn.Content = "Done!";
                    await Task.Delay(2500);
                    SetBtn.Content = "Set";
                }
            }
            else
            {
                SetBtn.Content = "Error";
                WeightNum_LostFocus(sender, e);
                PlaytimeNum_LostFocus(sender, e);
                PlacementNum_LostFocus(sender, e);
                await Task.Delay(2500);
                SetBtn.Content = "Set";
            }
            
        }

        private void WeightNum_LostFocus(object sender, RoutedEventArgs e)
        {
            if (float.TryParse(WeightNum.Text, out _))
            {
                WeightNum.BorderBrush = SystemColors.ControlDarkBrush;
                WeightVerification.Visibility = Visibility.Hidden;
                weightIsAGo = true;
            }
            else
            {
                
                WeightNum.BorderBrush = Brushes.Red;
                WeightVerification.Visibility = Visibility.Visible;
                weightIsAGo = false;
            }
        }

        private void PlaytimeNum_LostFocus(object sender, RoutedEventArgs e)
        {
            if (float.TryParse(PlaytimeNum.Text, out _))
            {
                PlaytimeNum.BorderBrush = SystemColors.ControlDarkBrush;
                PlaytimeVerification.Visibility = Visibility.Hidden;
                playtimeIsAGo = true;
            }
            else
            {

                PlaytimeNum.BorderBrush = Brushes.Red;
                PlaytimeVerification.Visibility = Visibility.Visible;
                playtimeIsAGo = false;
            }
        }

        private void PlacementNum_LostFocus(object sender, RoutedEventArgs e)
        {
            if (float.TryParse(PlacementNum.Text, out _))
            {
                PlacementNum.BorderBrush = SystemColors.ControlDarkBrush;
                PlacementVerification.Visibility = Visibility.Hidden;
                placementIsAGo = true;
            }
            else
            {

                PlacementNum.BorderBrush = Brushes.Red;
                PlacementVerification.Visibility = Visibility.Visible;
                placementIsAGo = false;
            }
        }
    }
    
}




    