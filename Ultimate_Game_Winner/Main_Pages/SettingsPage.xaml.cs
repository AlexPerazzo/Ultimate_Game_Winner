﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Permissions;
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
using System.Xml.Linq;
using Ultimate_Game_Winner;
using Ultimate_Game_Winner.UserControls;
using Ultimate_Game_Winner.Windows;

namespace Ultimate_Game_Winner.Main_Pages
{
    /// <summary>
    /// Contains options for changing ranking system and clearing the log of all recorded games
    /// </summary>
    public partial class SettingsPage : Page
    {
        private bool weightIsAGo = true;
        private bool playtimeIsAGo = true;
        private bool placementIsAGo = true;
        


        public SettingsPage()
        {
            InitializeComponent();
            Loaded += LoadSettings;
        }

        // Page Display Functions
        private void LoadSettings(object sender, RoutedEventArgs e)
        {
            //Purpose: Loads savedsettings in so it displays the same as the user last saved it
            
            string[] savedSettings = File.ReadAllLines("..\\..\\..\\Text_Files\\SavedSettings.txt");
            
            LoadFilter(savedSettings);
            LoadRankingSystem(savedSettings);

        }
        private void LoadRankingSystem(string[] savedSettings)
        {
            //Purpose: Loads Ranking System with previous user saved information
            WeightNum.Text = savedSettings[0];
            PlaytimeNum.Text = savedSettings[1];
            PlacementNum.Text = savedSettings[2];

            if (savedSettings[3] == "# of Wins")
                RankingSysBox.Text = savedSettings[3];
            else if (float.Parse(savedSettings[0]) == 1 && float.Parse(savedSettings[1]) == 1 && float.Parse(savedSettings[2]) == 1)
                RankingSysBox.Text = "Normal";
            else
                RankingSysBox.Text = savedSettings[3];
        }
        private void LoadFilter(string[] savedSettings)
        {
            //Purpose: Loads Filter with previous user saved information
            var ItemsList = savedSettings[5].Split(",");

            GenreBox.ItemsSource = ItemsList;
            PlayerBox.ItemsSource = new string[] { "All Player Counts", "2 Players", "3 Players", "4 Players", "5 Players", "6 Players" };
            WeightBox.ItemsSource = new string[] { "All Weights", "1-2 (Light)", "2-3 (Medium-Light)", "3-4 (Medium-Heavy)", "4-5 (Heavy)" };
            PlaytimeBox.ItemsSource = new string[] { "All Playtimes", "Less than 30 min", "30-60 min", "60-90 min", "90-120 min", "Longer than 120 min" };
            GroupBox.ItemsSource = new string[] { "All Groups", "Family", "Friends", "Extended Family", "Work/Colleagues", "Neighbors", "School/Classmates", "Other" };


            var filterOptions = savedSettings[4].Split(",");
            GenreBox.Text = filterOptions[0];
            PlayerBox.Text = filterOptions[1];
            WeightBox.Text = filterOptions[2];
            PlaytimeBox.Text = filterOptions[3];
            GroupBox.Text = filterOptions[4];
        }


        // Ranking System Options Event Listeners
        private void Custom_Selected(object sender, RoutedEventArgs e)
        {
            //Purpose: Displays custom ranking options when custom is selected

            string[] values;
            values = File.ReadAllLines("..\\..\\..\\Text_Files\\SavedSettings.txt");
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
            //Purpose: Resets values back to normal and refreshes leaderboard with updated values when normal is selected

            var values = File.ReadAllLines("..\\..\\..\\Text_Files\\SavedSettings.txt");
            bool refreshLeaderboard = false;

            CustomRankItems.Visibility = Visibility.Collapsed;

            var textFileToChange = File.ReadAllLines("..\\..\\..\\Text_Files\\SavedSettings.txt");

            //Doesn't waste time doing API calls if the input is already in place
            
            if (!(float.Parse(textFileToChange[0]) == 1 && float.Parse(textFileToChange[1]) == 1 && float.Parse(textFileToChange[2]) == 1))
            {
                //checks if leaderboard should be refreshed
                refreshLeaderboard = true;
                
            }

            //sets the settings regardless (due to # of wins existing; can't just clump it into one)
            using (StreamWriter writer = new StreamWriter("..\\..\\..\\Text_Files\\SavedSettings.txt"))
            {
                writer.WriteLine("1.0");
                writer.WriteLine("1.0");
                writer.WriteLine("1.0");
                writer.WriteLine("Normal");
                writer.WriteLine(values[4]);
                writer.WriteLine(values[5]);
            }

            //refreshes leaderboard if necessary
            if (refreshLeaderboard)
                LeaderboardPage.RefreshLeaderboard();
        }
        private void NumofWins_Selected(object sender, RoutedEventArgs e)
        {
            //Purpose: Edits SavedSettings.txt with "# of wins" so Leaderboard displays correctly
            CustomRankItems.Visibility = Visibility.Collapsed;
            var textFileToChange = File.ReadAllLines("..\\..\\..\\Text_Files\\SavedSettings.txt");
            using (StreamWriter writer = new StreamWriter("..\\..\\..\\Text_Files\\SavedSettings.txt"))
            {
                writer.WriteLine(textFileToChange[0]);
                writer.WriteLine(textFileToChange[1]);
                writer.WriteLine(textFileToChange[2]);
                writer.WriteLine("# of Wins");
                writer.WriteLine(textFileToChange[4]);
                writer.WriteLine(textFileToChange[5]);
            }
        }


        // Verification Event Listeners
        private void WeightNum_LostFocus(object sender, RoutedEventArgs e)
        {
            //Purpose: User verification for WeightNum

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
            //Purpose: User verification for PlaytimeNum

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
            //Purpose: User verification for PlacementNum

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

        // Button Event Listeners
        private async void FilterSetBtn_Click(object sender, RoutedEventArgs e)
        {
            //Purpose: Sets selected Filter options in SavedSettings and refreshes Leaderboard
            //Note: From there, RefreshLeaderboard will update each game's status as to whether it passes the filters or not

            string filePath = "..\\..\\..\\Text_Files\\SavedSettings.txt";
            string[] lines = File.ReadAllLines(filePath);

            var filterChoices = lines[4].Split(",");
            filterChoices[0] = GenreBox.SelectedItem.ToString();
            filterChoices[1] = PlayerBox.SelectedItem.ToString();
            filterChoices[2] = WeightBox.SelectedItem.ToString();
            filterChoices[3] = PlaytimeBox.SelectedItem.ToString();
            filterChoices[4] = GroupBox.SelectedItem.ToString();

            var updatedFilterChoices = string.Join(",", filterChoices);
            if (lines[4] != updatedFilterChoices)
            {
                FilterSetBtn.Content = "Note: This may take a second";
                await Task.Delay(20);

                //Updates Filter Choices then uses RefreshLeaderboard to update the game log depending on those choices
                lines[4] = updatedFilterChoices;
                File.WriteAllLines(filePath, lines);
                LeaderboardPage.RefreshLeaderboard();

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
        private async void FilterResetBtn_Click(object sender, RoutedEventArgs e)
        {
            //Purpose: Resets everything to non-filtered state

            string filePath = "..\\..\\..\\Text_Files\\SavedSettings.txt";
            string[] lines = File.ReadAllLines(filePath);

            var filterChoices = lines[4].Split(",");
            filterChoices[0] = "All Genres";
            filterChoices[1] = "All Player Counts";
            filterChoices[2] = "All Weights";
            filterChoices[3] = "All Playtimes";
            filterChoices[4] = "All Groups";
            GenreBox.SelectedItem = filterChoices[0];
            PlayerBox.SelectedItem = filterChoices[1];
            WeightBox.SelectedItem = filterChoices[2];
            PlaytimeBox.SelectedItem = filterChoices[3];
            GroupBox.SelectedItem = filterChoices[4];

            var updatedFilterChoices = string.Join(",", filterChoices);
            
            
            if (lines[4] != updatedFilterChoices)
            {
                FilterResetBtn.Content = "Note: This may take a second";
                await Task.Delay(20);

                //Updates Filter Choices then uses RefreshLeaderboard to update the game log depending on those choices
                lines[4] = updatedFilterChoices;
                File.WriteAllLines(filePath, lines);
                LeaderboardPage.RefreshLeaderboard();

                FilterResetBtn.Content = "Done!";
                await Task.Delay(2500);
                FilterResetBtn.Content = "Reset";
            }
            else
            {
                FilterResetBtn.Content = "No Change Detected";
                await Task.Delay(2000);
                FilterResetBtn.Content = "Reset";
            }
        }
        private async void CustomSetBtn_Click(object sender, RoutedEventArgs e)
        {
            //Purpose: Edits SavedSettings.txt with user inputted data, which is used when calculating points

            //If legal input continue
            if (weightIsAGo && playtimeIsAGo && placementIsAGo)
            {

                var weight = WeightNum.Text;
                var playtime = PlaytimeNum.Text;
                var placement = PlacementNum.Text;

                var textFileToChange = File.ReadAllLines("..\\..\\..\\Text_Files\\SavedSettings.txt");

                //Doesn't run API all over again if the input is already in place
                if (textFileToChange[0] == weight && textFileToChange[1] == playtime && textFileToChange[2] == placement)
                {
                    CustomSetBtn.Content = "No change detected";
                    await Task.Delay(2000);
                    CustomSetBtn.Content = "Set";
                }

                //Saves the user's input and refreshes the leaderboard
                else
                {

                    CustomSetBtn.Content = "Note: This may take a second";
                    await Task.Delay(20);
                    using (StreamWriter writer = new StreamWriter("..\\..\\..\\Text_Files\\SavedSettings.txt"))
                    {
                        writer.WriteLine(weight);
                        writer.WriteLine(playtime);
                        writer.WriteLine(placement);
                        writer.WriteLine("Custom");
                        writer.WriteLine(textFileToChange[4]);
                        writer.WriteLine(textFileToChange[5]);

                    }
                    LeaderboardPage.RefreshLeaderboard();
                    CustomSetBtn.Content = "Done!";
                    await Task.Delay(2000);
                    CustomSetBtn.Content = "Set";
                }
            }
            //If not legal input display invalid inputs
            else
            {
                CustomSetBtn.Content = "Error";
                WeightNum_LostFocus(sender, e);
                PlaytimeNum_LostFocus(sender, e);
                PlacementNum_LostFocus(sender, e);
                await Task.Delay(2000);
                CustomSetBtn.Content = "Set";
            }

        }


        // Delete Log Event Listener
        private void DeleteLog_Click(object sender, RoutedEventArgs e)
        {
            //Purpose: Displays a window which will ask for confirmation and delete the entire log
            
            AreYouSureWindow areYouSure = new AreYouSureWindow(true, [], null);
            areYouSure.Show();
        }
    }
    
}




    