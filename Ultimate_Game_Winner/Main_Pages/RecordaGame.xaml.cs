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
using System.Collections.Specialized;
using Ultimate_Game_Winner.UserControls_and_Windows;

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
        public ObservableCollection<PlaceholderTextBox> TextBoxCollection { get; set; }
        public ObservableCollection<TextBox> OldTextBoxCollection { get; set; }

        public RecordaGame()
        {
            InitializeComponent();
            TextBoxCollection = new ObservableCollection<PlaceholderTextBox>();
            NameTextBoxesControl.ItemsSource = TextBoxCollection;
            OldTextBoxCollection = new ObservableCollection<TextBox>();
        }

        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            //Event Listener for Submit Game Button
            //Purpose: If boxes have valid input, saves text to log file and updates points in leaderboard

            if (gameNameIsAGo && numPlayersIsAGo)
            {
                Submit.Content = "Note: This may take a second";
                await Task.Delay(20);
                string nameOfGame = GameName.Input.Text; string numPlayers = NumPlayers.Text;
                (string lineSaved, bool filterBool) = SaveToLog(nameOfGame, numPlayers);
                //Reset page's text
                Cancel_Click(sender, e);

                if (filterBool)
                    UpdateLeaderboard(lineSaved);

                //If game is a new genre, update that option in settings
                UpdateFilterOptions(nameOfGame);

                //Displays Success and then converts back to normal
                Submit.Content = "Success!!";
                await Task.Delay(2500);
                Submit.Content = "Submit";
            }
            else 
            {
                
                NumPlayers_LostFocus(sender, e);
                GameName_LostFocus(sender, e);
                Submit.Content = "Error";
                await Task.Delay(2500);
                Submit.Content = "Submit";

            }

        }

        private void UpdateFilterOptions(string nameOfGame)
        {
            //Purpose: Checks if game's genre is new to the system. If it is, it adds it to the options displayed in settings

            //Grabs all genres currently in file and the genre of the game being added
            string filePath = "C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt";
            var ID = UtilityFunctions.GetID(nameOfGame);
            var theGenre = UtilityFunctions.GetAPIGenre(ID);
            
            var textFile = File.ReadAllLines(filePath);
            var currentGenres = textFile[5].Split(",");
            bool addGameIsAGo = false;

            


            if (!currentGenres.Contains(theGenre))
                addGameIsAGo = true;

            //adds genre if it's new
            if (addGameIsAGo)
            {
                textFile[5] += $",{theGenre}";
                File.WriteAllLines(filePath, textFile);
            }

        }

        private (string, bool) SaveToLog(string nameOfGame, string numPlayers)  
        {
            //Purpose: Records gameplay with all associated information to LogofPlayedGames.txt
            //All associated information: Name of Game, # of players, player's names in order of placement, additional gameplay notes, if it passes the filter tests, and the date

            bool filterBool;
            var officialName = UtilityFunctions.ReturnOfficialGameName(nameOfGame);
            // Append each item from the list, separated by commas
            string line = $"{officialName},,,{numPlayers},,,";

            int count = -1;
            foreach (PlaceholderTextBox textBox in TextBoxCollection)
            {
                count++;
                //Checks if it's a player
                if (count == int.Parse(numPlayers))
                {
                //If it's not, it does the checks for the Additional Gameplay notes
                    if (textBox.Input.Text == "")
                        line += "N/A,,,";
                    else
                        line += textBox.Input.Text + ",,,";
                }
                else
                    line += UtilityFunctions.CapitalizeEachWord(textBox.Input.Text) + ",,,";
                    
                
            }

            //Add on the group
            line += GroupComboBox.Text + ",,,";


            //Add on the true or false for the filter
            if (UtilityFunctions.ShouldFilter(officialName, numPlayers, GroupComboBox.Text))
            {
                line += "true,,,";
                filterBool = true;
            }
            else
            {
                line += "false,,,";
                filterBool = false;
            }

            //Add the date
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
            //Purpose: Resets entered information when clicked upon

            //Resets Texts of all TextBox's back to original display
            GameName.Input.Text = "";
            NumPlayers.Text = "";
            GroupComboBox.Text = "No Group";
            TextBoxCollection.Clear();

            //Resets verification error messages
            NumPlayers.BorderBrush = SystemColors.ControlDarkBrush;
            NumPlayersVerification.Visibility = Visibility.Hidden;
            GameName.Input.BorderBrush = SystemColors.ControlDarkBrush;
            GameNameVerification.Visibility = Visibility.Hidden;
            gameNameIsAGo = false;
            numPlayersIsAGo = false;

            
        }
        

        

        private void UpdateLeaderboard(string gameplayToAdd)
        {
            //Purpose updates current total points with new points from a gameplay
            //Unlike RefreshLeaderboard, this adds one game to the already established leaderboard rather than building it from the ground up

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
                //Loops through from first player (index 2) to last player (index fourth to last (there's a group, date, and notes at the end))
                for (int i = 2; i < gameParts.Length - 4; i++)
                {

                    double points = UtilityFunctions.CalculatePoints(averageWeight, averagePlaytime, int.Parse(gameParts[1]), i-1);
                    
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


        
        
        
        
        private void UpdateTextBoxCollection(int count)
        {
            //Purpose: Adds an associated number of TextBoxes based on the number of players inputted (between 2 and 6)
            

            // Clear the existing collection
            TextBoxCollection.Clear();

            // Add the required number of textboxes
            for (int i = 0; i < count; i++)
            {
                PlaceholderTextBox textBox = new PlaceholderTextBox {Margin = new Thickness(0, 25, 0, 0) };
                TextBox oldtextBox = new TextBox { Width = 100, Margin = new Thickness(0, 25, 0, 0) };
                textBox.BindedWidth = "100";
                textBox.BindedHeight = "18";
                textBox.placeholderText = $"{UtilityFunctions.AddOrdinal(i + 1)} place...";
                textBox.VerticalAlignment = VerticalAlignment.Center;
                textBox.HorizontalAlignment = HorizontalAlignment.Center;
                textBox.BindedWrap = "NoWrap";
                
                TextBoxCollection.Add(textBox);
            }

            // Adds additional box for gameplay notes
            PlaceholderTextBox notes = new PlaceholderTextBox { Margin = new Thickness(0, 25, 0, 0) };
            notes.BindedWidth = "300";
            notes.BindedHeight = "70";
            notes.placeholderText = "Additional gameplay notes...";
            notes.VerticalAlignment = VerticalAlignment.Center;
            notes.HorizontalAlignment = HorizontalAlignment.Center;
            
            notes.BindedWrap = "Wrap";
            TextBoxCollection.Add(notes);


        }

        private void GameName_LostFocus(object sender, RoutedEventArgs e)
        {
            //Purpose: Validates User Input for Game Name

            //If Invalid input, makes box red and pops up message
            //Turns necessary variable for submitting to false
            if (GameName.Input.Text == "" || !UtilityFunctions.DoesGameExist(GameName.Input.Text))
            {
                GameName.Input.BorderBrush = Brushes.Red;
                GameNameVerification.Visibility = Visibility.Visible;
                gameNameIsAGo = false;
            }
            //If valid, puts things to normal
            //Turns necessary variable for submitting to true
            else
            {
                GameName.Input.BorderBrush = SystemColors.ControlDarkBrush;
                GameNameVerification.Visibility = Visibility.Hidden;
                gameNameIsAGo = true;
            }
        }

        private void NumPlayers_LostFocus(object sender, RoutedEventArgs e)
        {
            //Purpose: Validates User Input for Number of Players

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

        private void NumPlayers_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(NumPlayers.Text))
                tbPlaceholder.Visibility = Visibility.Visible;
            else
                tbPlaceholder.Visibility = Visibility.Hidden;

            if (int.TryParse(NumPlayers.Text, out int numberOfTextBoxes) && numberOfTextBoxes >= 2 && numberOfTextBoxes <= 6)
            {
                UpdateTextBoxCollection(numberOfTextBoxes);
            }

        }
    }
}
