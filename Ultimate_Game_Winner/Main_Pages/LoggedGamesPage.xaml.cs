﻿using System;
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
using Ultimate_Game_Winner.UserControls;

namespace Ultimate_Game_Winner.Main_Pages
{
    public partial class LoggedGamesPage : Page
    {
        /// <summary>
        /// Displays every single recorded game using the LoggedGamePanel UserControl in a stack panel
        /// </summary>
        public string Filtered { get; set; }

        public LoggedGamesPage()
        {
            InitializeComponent();
            this.DataContext = this;
            Filtered = UtilityFunctions.FilterLabelVisibility();
            Loaded += LoadLog;
            Loaded += ResizePage;
        }

        private void LoadLog(object sender, RoutedEventArgs e)
        {
            
            using (StreamReader reader = new StreamReader("..\\..\\..\\Text_Files\\LogofPlayedGames.txt"))
            {
                //Purpose: Populates StackPanel with LoggedGamePanel User Controls

                //Reads each line from the LogofPlayedGames.txt file and makes a TextBlock out of it
                //Adds that TextBlock to the StackPanel from the xaml page
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(",,,");

                    if (parts[parts.Length-2] == "true")
                    {
                        
                        LoggedGamePanelUC panel = new LoggedGamePanelUC();
                        panel.GameName = parts[0];
                        var numPlayers = parts[1];
                        panel.NumPlayers = $"{numPlayers} players";
                        panel.PlayerName = parts[2];
                        panel.Date = parts[parts.Length - 1];
                        panel.Margin = new Thickness(12, 12, 0, 0);
                        panel.AllInfo = parts;
                        

                        theLog.Children.Insert(0, panel);
                    }
                }
            }


            
        }
        private void ResizePage(object sender, RoutedEventArgs e)
        {
            // Get the parent window
            Window parentWindow = Window.GetWindow(this);

            if (parentWindow != null)
            {
                // Adjust the size of the parent window
                parentWindow.SizeToContent = SizeToContent.Width;
                parentWindow.Height = parentWindow.Height;
            }
        }

        private async void RefreshLogBtn_Click(object sender, RoutedEventArgs e)
        {
            theLog.Children.Clear();
            LoadLog(sender, e);
            RefreshLogBtn.Content = "Done!";
            await Task.Delay(2000);
            RefreshLogBtn.Content = "Refresh Log";
        }
    }
}
