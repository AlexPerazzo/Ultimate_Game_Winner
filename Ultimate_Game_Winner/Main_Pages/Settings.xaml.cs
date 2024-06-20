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
using System.Windows.Media.Media3D;
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
        private bool genreBoxChange = false;


        public Settings()
        {
            InitializeComponent();
            Loaded += LoadSettings;
        }

        private void LoadSettings(object sender, RoutedEventArgs e)
        {
            //Loads savedsettings in so it displays the same as the user last saved it
            string[] values;
            values = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");
            WeightNum.Text = values[0];
            PlaytimeNum.Text = values[1];
            PlacementNum.Text = values[2];
            var ItemsList = values[5].Split(",");

            GenreBox.ItemsSource = ItemsList;
            PlayerBox.ItemsSource = new string[] { "All Player Counts", "2 Players", "3 Players", "4 Players", "5 Players", "6 Players" };
            WeightBox.ItemsSource = new string[] { "All Weights", "1-2 (Light)", "2-3 (Medium-Light)", "3-4 (Medium-Heavy)", "4-5 (Heavy)"};
            PlaytimeBox.ItemsSource = new string[] { "All Playtimes", "Less than 30 min", "30-60 min", "60-90 min", "90-120 min", "Longer than 120 min"};

            var filterOptions = values[4].Split(",");
            GenreBox.Text = filterOptions[0];
            PlayerBox.Text = filterOptions[1];
            WeightBox.Text = filterOptions[2];
            PlaytimeBox.Text = filterOptions[3];


            if (float.Parse(values[0]) == 1 && float.Parse(values[1]) == 1 && float.Parse(values[2]) == 1)
                RankingSysBox.Text = "Normal";
            else
                RankingSysBox.Text = values[3];

        }

        private void DeleteLog_Click(object sender, RoutedEventArgs e)
        {
            //Asks for confirmation. AreYouSure will do the work from there
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
            var values = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");
            

            CustomRankItems.Visibility = Visibility.Collapsed;

            var textFileToChange = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");

            //Doesn't waste time doing API calls if the input is already in place
            //textFileToChange[0] == weight && textFileToChange[1] == playtime && textFileToChange[2] == placement
            if (!(textFileToChange[0] == "1.0" && textFileToChange[1] == "1.0" && textFileToChange[2] == "1.0"))
            {
                //sets the settings back to normal and refreshes the leaderboard
                using (StreamWriter writer = new StreamWriter("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt"))
                {
                    writer.WriteLine("1.0");
                    writer.WriteLine("1.0");
                    writer.WriteLine("1.0");
                    writer.WriteLine("Normal");
                    writer.WriteLine(values[4]);
                    writer.WriteLine(values[5]);
                }
                Leaderboard.RefreshLeaderboard();
                
            }
        }



        private async void SetBtn_Click(object sender, RoutedEventArgs e)
        {
            //Edits SavedSettings.txt which is used when calculating points
            if (weightIsAGo && playtimeIsAGo && placementIsAGo)
                {

                var weight = WeightNum.Text;
                var playtime = PlaytimeNum.Text;
                var placement = PlacementNum.Text;

                var textFileToChange = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");
                
                //Doesn't run API all over again if the input is already in place
                if (textFileToChange[0] == weight && textFileToChange[1] == playtime && textFileToChange[2] == placement)
                {
                    SetBtn.Content = "No change detected";
                    await Task.Delay(2000);
                    SetBtn.Content = "Set";
                }

                //Saves the user's input and refreshes the leaderboard
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
                        writer.WriteLine(textFileToChange[4]);
                        writer.WriteLine(textFileToChange[5]);

                    }
                    Leaderboard.RefreshLeaderboard();
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
            //If valid, puts things to normal
            //Turns necessary variable for submitting to true
            if (float.TryParse(WeightNum.Text, out _))
            {
                WeightNum.BorderBrush = SystemColors.ControlDarkBrush;
                WeightVerification.Visibility = Visibility.Hidden;
                weightIsAGo = true;
            }
            //If Invalid input, makes box read and pops up message
            //Turns necessary variable for submitting to false
            else
            {
                
                WeightNum.BorderBrush = Brushes.Red;
                WeightVerification.Visibility = Visibility.Visible;
                weightIsAGo = false;
            }
        }

        private void PlaytimeNum_LostFocus(object sender, RoutedEventArgs e)
        {
            //If valid, puts things to normal
            //Turns necessary variable for submitting to true
            if (float.TryParse(PlaytimeNum.Text, out _))
            {
                PlaytimeNum.BorderBrush = SystemColors.ControlDarkBrush;
                PlaytimeVerification.Visibility = Visibility.Hidden;
                playtimeIsAGo = true;
            }
            //If Invalid input, makes box read and pops up message
            //Turns necessary variable for submitting to false
            else
            {

                PlaytimeNum.BorderBrush = Brushes.Red;
                PlaytimeVerification.Visibility = Visibility.Visible;
                playtimeIsAGo = false;
            }
        }

        private void PlacementNum_LostFocus(object sender, RoutedEventArgs e)
        {
            //If valid, puts things to normal
            //Turns necessary variable for submitting to true
            if (float.TryParse(PlacementNum.Text, out _))
            {
                PlacementNum.BorderBrush = SystemColors.ControlDarkBrush;
                PlacementVerification.Visibility = Visibility.Hidden;
                placementIsAGo = true;
            }
            //If Invalid input, makes box read and pops up message
            //Turns necessary variable for submitting to false
            else
            {

                PlacementNum.BorderBrush = Brushes.Red;
                PlacementVerification.Visibility = Visibility.Visible;
                placementIsAGo = false;
            }
        }

        private async void RefreshLeaderboardBtn_Click(object sender, RoutedEventArgs e)
        {
            Leaderboard.RefreshLeaderboard();
            RefreshLeaderboardBtn.Content = "Done!";
            await Task.Delay(2000);
            RefreshLeaderboardBtn.Content = "Refresh Leaderboard";
        }

        

        private async void FilterSetBtn_Click(object sender, RoutedEventArgs e)
        {
            string filePath = "C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt";
            string?[] lines = File.ReadAllLines(filePath);

            var filterChoices = lines[4].Split(",");
            filterChoices[0] = GenreBox.SelectedItem.ToString();
            filterChoices[1] = PlayerBox.SelectedItem.ToString();
            filterChoices[2] = WeightBox.SelectedItem.ToString();
            filterChoices[3] = PlaytimeBox.SelectedItem.ToString();

            var updatedFilterChoices = string.Join(",", filterChoices);
            if (lines[4] != updatedFilterChoices)
            {
                lines[4] = updatedFilterChoices;
                File.WriteAllLines(filePath, lines);
                Leaderboard.RefreshLeaderboard();
                FilterSetBtn.Content = "Done!";
                await Task.Delay(2500);
                FilterSetBtn.Content = "Set";
            }
            else
            {
                FilterSetBtn.Content = "No Change Detected";
                await Task.Delay(2000);
                FilterSetBtn.Content = "Set";
            }
        }
    }
    
}




    