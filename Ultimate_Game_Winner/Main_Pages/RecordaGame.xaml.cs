using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
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
using System.Net.Http;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using Ultimate_Game_Winner;

namespace Ultimate_Game_Winner.Main_Pages
{
    /// <summary>
    /// Gathers information about a gameplay.
    /// (including Game Name, # of Players, their placements, and any other optional gameplay notes)
    /// Saves that information for later use in the Log. Also calculate points and updates the Leaderboard.
    /// The fact that this xaml/xaml.cs dabbles into both the Log and the Leaderboard may suggest that it should be better organized
    /// </summary>
    public partial class RecordaGame : Page
    {
        private bool numPlayersIsAGo = false;
        private bool gameNameIsAGo = false;
        public ObservableCollection<TextBox> TextBoxCollection { get; set; }
        public RecordaGame()
        {
            InitializeComponent();
            TextBoxCollection = new ObservableCollection<TextBox>();
            NameTextBoxesControl.ItemsSource = TextBoxCollection;
        }

        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (gameNameIsAGo && numPlayersIsAGo)
            {

                string nameOfGame = GameName.Text; string numPlayers = NumPlayers.Text;
                (string lineSaved, bool filterBool) = SaveToLog(nameOfGame, numPlayers);
                //Reset page's text
                Cancel_Click(sender, e);


                var foo = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");
                var selectedGenre = foo[4];

                if (filterBool)
                    UpdateLeaderboard(lineSaved);


                UpdateFilterOptions(nameOfGame);

                //Displays Success and then converts back to normal
                Submit.Content = "Success!!";
                await Task.Delay(2500);
                Submit.Content = "SUBMIT";
            }
            else 
            {
                
                NumPlayers_LostFocus(sender, e);
                GameName_LostFocus(sender, e);
                Submit.Content = "Error";
                await Task.Delay(2500);
                Submit.Content = "SUBMIT";

            }

        }

        private void UpdateFilterOptions(string nameOfGame)
        {
            string filePath = "C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt";
            var ID = UtilityFunctions.GetID(nameOfGame);
            var theGenre = UtilityFunctions.GetAPIGenre(ID);
            
            var textFile = File.ReadAllLines(filePath);
            var currentGenres = textFile[5].Split(",");
            bool addGameIsAGo = false;

            string fixedGenre = UtilityFunctions.FormatGenre(theGenre);


            if (!currentGenres.Contains(fixedGenre))
                addGameIsAGo = true;

            if (addGameIsAGo)
            {
                textFile[5] += $",{fixedGenre}";
                File.WriteAllLines(filePath, textFile);
            }

        }

        private (string, bool) SaveToLog(string nameOfGame, string numPlayers)
        {
            bool filterBool;
            var officialName = UtilityFunctions.ReturnOfficialGameName(nameOfGame);
            // Append each item from the list, separated by commas
            string line = $"{officialName},,,{numPlayers},,,";
            foreach (TextBox textBox in TextBoxCollection)
            {
                if (textBox.Text == "" || textBox.Text == "Additional gameplay notes... (leave blank or don't touch if none are wanted)")
                    line += "N/A,,,";
                else
                    line += UtilityFunctions.CapitalizeEachWord(textBox.Text) + ",,,";
                
            }

            var foo = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");
            var selectedGenre = foo[4];

            if (selectedGenre == "All Games")
            {
                line += "true,,,";
                filterBool = true;
            }
            else
            {
                var ID = UtilityFunctions.GetID(nameOfGame);
                var theGenre = UtilityFunctions.GetAPIGenre(ID);
                if (selectedGenre == UtilityFunctions.FormatGenre(theGenre))
                {
                    line += "true,,,";
                    filterBool = true;
                }
            
                else
                {
                    line += "false,,,";
                    filterBool = false;
                }



            }

            DateTime currentDate = DateTime.Now;
            var correctFormatDate = currentDate.ToString("M/d/yy");
            line += correctFormatDate;
            
            //Save all information to LogofPlayedGames.txt
            SaveStringIntoTxt(line, "C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\LogofPlayedGames.txt");

            void SaveStringIntoTxt(string stringToSave, string fileName)
            {
                //writes line to file
                using (StreamWriter writer = new StreamWriter(fileName, true))
                {
                    writer.WriteLine(stringToSave);
                }
            }
            return (line, filterBool);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //Resets Texts of all TextBox's back to original display
            GameName.Text = "Name of Game";
            NumPlayers.Text = "# of Players";
            TextBoxCollection.Clear();

            //Resets verification error messages
            NumPlayers.BorderBrush = SystemColors.ControlDarkBrush;
            NumPlayersVerification.Visibility = Visibility.Hidden;
            GameName.BorderBrush = SystemColors.ControlDarkBrush;
            GameNameVerification.Visibility = Visibility.Hidden;
            gameNameIsAGo = false;
            numPlayersIsAGo = false;

            
        }
        

        public static double CalculatePoints(float weight, float playtime, int numOfPlayers, int placement)
        {
            //Gathers weights of different factors (in case custom ranking system is on)
            string[] values;
            values = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");
            
            //Creates three factors that go into point total
            var weightFactor = float.Parse(values[0]) * Math.Log10(weight) * 3;
            if (playtime <= 10)
            {
                playtime = 10;
            }
            var playtimeFactor = float.Parse(values[1]) * Math.Log10(playtime / 10) * 2;
            var placementFactor = float.Parse(values[2]) * CalculatePlacementPercentage(placement, numOfPlayers);
  
            //Creates a point value using the three factors
            var points = (weightFactor + playtimeFactor) * placementFactor;

            string stringPoints = points.ToString("0.00");
            var finalPoints = double.Parse(stringPoints);


            return finalPoints;

            double CalculatePlacementPercentage(int placement, int numOfPlayers)
            {
                //reads from PlacementPercentages.txt and grabs the associated information needed for the math calculations.
                using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\PlacementPercentages.txt"))
                {
                    reader.ReadLine();
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] lineList = line.Split(",");
                        if (int.Parse(lineList[0]) == numOfPlayers)
                        {
                            return double.Parse(lineList[placement]);
                        }
                    }
                }
                return -1;
            }
        }

        private void UpdateLeaderboard(string gameplayToAdd)
        {
            //Gathers necessary information about game
            string[] gameParts = gameplayToAdd.Split(",,,");
            int gameID = UtilityFunctions.GetID(gameParts[0]);
            (float averagePlaytime, float averageWeight) = UtilityFunctions.GetAPIData(gameID);


            Dictionary<String, double> newLeaderboard = new Dictionary<string, double>();

            //puts currently leaderboard onto newLeaderboard
            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\Leaderboard.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null) 
                {
                    String[] parts = line.Split(',');
                    newLeaderboard.Add(parts[0], double.Parse(parts[1]));
                }

            }
                //Loops through from first player (index 2) to last player (index third to last (there's a date and notes at the end))
                for (int i = 2; i < gameParts.Length - 3; i++)
                {

                    double points = CalculatePoints(averageWeight, averagePlaytime, int.Parse(gameParts[1]), i-1);
                    
                    //If new person, add them with their points to newLeaderboard
                    if (!newLeaderboard.ContainsKey(gameParts[i]))
                    {
                        newLeaderboard.Add(gameParts[i], points);
                    }
                    //Otherwise simply adds the points to their previous total
                    else
                    {
                        newLeaderboard[gameParts[i]] += points;
                    }
                }
            
            //sort so Leaderboard is already in correct ranking order when saved to text file
            var sortedDictionary = newLeaderboard.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            //Write onto Leaderboard.txt by iterating through a sorted dictionary and putting their placement, name, and points
            using (StreamWriter writer = new StreamWriter("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\Leaderboard.txt"))
            {
                
                writer.Write(string.Empty);
                int i = 0;
                foreach (var entry in sortedDictionary)
                {
                    i += 1;
                    var key = entry.Key;
                    var value = entry.Value;
                    string writeThis = $"{key},{value:F2}";
                    writer.WriteLine(writeThis);
                }
            }
        }


        
        
        private void NumPlayers_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Adds an associated number of TextBoxes based on the number of players inputted (between 2 and 6)
            if (int.TryParse(NumPlayers.Text, out int numberOfTextBoxes) && numberOfTextBoxes >= 2 && numberOfTextBoxes <= 6)
            {
                UpdateTextBoxCollection(numberOfTextBoxes);
            }
            
        }

        private void UpdateTextBoxCollection(int count)
        {

            // Clear the existing collection
            TextBoxCollection.Clear();

            // Add the required number of textboxes
            for (int i = 0; i < count; i++)
            {
                TextBox textBox = new TextBox { Width = 100, Margin = new Thickness(0, 25, 0, 0) };
                textBox.Text = $"{UtilityFunctions.AddOrdinal(i+1)} place...";
                textBox.VerticalAlignment = VerticalAlignment.Center;
                textBox.HorizontalAlignment = HorizontalAlignment.Center;
                TextBoxCollection.Add(textBox);
            }

            // Adds additional box for gameplay notes
                TextBox notes = new TextBox { Width = 300, Height = 70, Margin = new Thickness(0, 25, 0, 0) };
                notes.Text = "Additional gameplay notes... (leave blank or don't touch if none are wanted)";
                notes.VerticalAlignment = VerticalAlignment.Center;
                notes.HorizontalAlignment = HorizontalAlignment.Center;
                notes.TextWrapping = TextWrapping.Wrap;
                TextBoxCollection.Add(notes);
        }

        private void GameName_LostFocus(object sender, RoutedEventArgs e)
        {
            //If Invalid input, makes box read and pops up message
            //Turns necessary variable for submitting to false
            if (!UtilityFunctions.DoesGameExist(GameName.Text))
            {
                GameName.BorderBrush = Brushes.Red;
                GameNameVerification.Visibility = Visibility.Visible;
                gameNameIsAGo = false;
            }
            //If valid, puts things to normal
            //Turns necessary variable for submitting to true
            else
            {
                GameName.BorderBrush = SystemColors.ControlDarkBrush;
                GameNameVerification.Visibility = Visibility.Hidden;
                gameNameIsAGo = true;
            }
        }

        private void NumPlayers_LostFocus(object sender, RoutedEventArgs e)
        {
            //If Invalid input, makes box read and pops up message
            //Turns necessary variable for submitting to false
            if (NumPlayers.Text != "2" && NumPlayers.Text != "3" && NumPlayers.Text != "4" && NumPlayers.Text != "5" && NumPlayers.Text != "6")
            {
                NumPlayers.BorderBrush = Brushes.Red;
                NumPlayersVerification.Visibility = Visibility.Visible;
                numPlayersIsAGo = false;
            }
            //If valid, puts things to normal
            //Turns necessary variable for submitting to true
            else
            {
                NumPlayers.BorderBrush = SystemColors.ControlDarkBrush;
                NumPlayersVerification.Visibility = Visibility.Hidden;
                numPlayersIsAGo = true;
            }
        }

        
       


    }
}
