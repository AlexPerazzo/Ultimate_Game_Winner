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
using Ultimate_Game_Winner.UserControls;
using Microsoft.VisualBasic;

namespace Ultimate_Game_Winner.Main_Pages
{
    /// <summary>
    /// Gathers information about a gameplay.
    /// (including Game Name, # of Players, their placements, group, and any other optional gameplay notes)
    /// Saves that information for later use in the Log. Also calculate points and updates the Leaderboard.
    /// </summary>
    public partial class RecordGameplayPage : Page
    {
        private bool numPlayersIsAGo = false;
        private bool gameNameIsAGo = false;
        private bool playerNamesAreAGo = true;
        
        public ObservableCollection<PlaceholderTextBoxUC> TextBoxCollection { get; set; }
        public ObservableCollection<Label> VerificationLabelCollection { get; set; }

        private List<string> items;
        private List<string> gameNames;


        public RecordGameplayPage()
        {
            InitializeComponent();
            this.DataContext = this;
            TextBoxCollection = new ObservableCollection<PlaceholderTextBoxUC>();
            VerificationLabelCollection = new ObservableCollection<Label>();
            NameTextBoxesControl.ItemsSource = TextBoxCollection;
            NameLabelVerificationControl.ItemsSource = VerificationLabelCollection;

            items = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\GamesAndIDs.txt").ToList();
            gameNames = items.Select(line => line.Split(',')[1]).ToList();
            GameName.Input.TextChanged += InputTextBox_TextChanged;
            this.Width = 800;
            
            GameName.Input.PreviewKeyDown += GameName_PreviewKeyDown;

            
            
        }

        private async void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            string query = GameName.Input.Text;
            if (string.IsNullOrWhiteSpace(query))
            {
                SuggestionsListBox.ItemsSource = null;
                SuggestionsListBox.Visibility = Visibility.Collapsed;
                return;
            }

            
            
            var suggestions = GetSuggestions(query);

            SuggestionsListBox.ItemsSource = suggestions;
            SuggestionsListBox.Visibility = Visibility.Visible;
            
        }

        private List<string> GetSuggestions(string query)
        {
            return gameNames.Where(item => item.StartsWith(query, StringComparison.OrdinalIgnoreCase)).Take(25).ToList();
        }



        private void SuggestionsListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SuggestionsListBox.SelectedItem != null)
                GameName.Input.Text = SuggestionsListBox.SelectedItem.ToString();
            GameName_LostFocus(sender, e);
            SuggestionsListBox.Visibility = Visibility.Collapsed;
            NumPlayers.Focus();
        }


        private void GameName_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            int currentIndex = SuggestionsListBox.SelectedIndex;

            if (e.Key == Key.Up)
            {
                if (currentIndex == 0)
                {
                    SuggestionsListBox.SelectedIndex = SuggestionsListBox.Items.Count - 1;
                }
                else
                {
                    if (SuggestionsListBox.SelectedItem != null)
                        SuggestionsListBox.SelectedIndex--;
                }
            }
            else if (e.Key == Key.Down)
            {
                if (currentIndex == SuggestionsListBox.Items.Count - 1)
                {
                    SuggestionsListBox.SelectedIndex = 0;
                }
                else
                {
                    SuggestionsListBox.SelectedIndex++;
                }
            }
            else if (e.Key == Key.Enter)
            {
                if (SuggestionsListBox.SelectedItem != null)
                    GameName.Input.Text = SuggestionsListBox.SelectedItem.ToString();
                GameName_LostFocus(sender, e);
                SuggestionsListBox.Visibility = Visibility.Collapsed;
                NumPlayers.Focus();
            }
        }

        private void SuggestionsListBox_KeyDown(object sender, KeyEventArgs e)
        {
            //int currentIndex = SuggestionsListBox.SelectedIndex;

            //if (e.Key == Key.Up)
            //{
            //    if (currentIndex == 0)
            //    {
            //        SuggestionsListBox.SelectedIndex = SuggestionsListBox.Items.Count - 1;
            //    }
            //    else
            //    {
            //        SuggestionsListBox.SelectedIndex--;
            //    }
            //}
            //else if (e.Key == Key.Down)
            //{
            //    if (currentIndex == SuggestionsListBox.Items.Count - 1)
            //    {
            //        SuggestionsListBox.SelectedIndex = 0;
            //    }
            //    else
            //    {
            //        SuggestionsListBox.SelectedIndex++;
            //    }
            //}
            //else if (e.Key == Key.Enter)
            //{
            //    if (SuggestionsListBox.SelectedItem != null)
            //        GameName.Input.Text = SuggestionsListBox.SelectedItem.ToString();
            //    GameName_LostFocus(sender, e);
            //}
        }



















        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            //Event Listener for Submit Game Button
            //Purpose: If boxes have valid input, saves text to log file and updates points in leaderboard
            
            playerNamesAreAGo = true;


            int count = TextBoxCollection.Count;
            int index = 0;
            foreach (PlaceholderTextBoxUC textBox in TextBoxCollection)
            {
                
                if (index < count - 1)
                {
                    PlayerName_LostFocus(textBox, e);
                    
                }
                index++;
            }


            if (gameNameIsAGo && numPlayersIsAGo && playerNamesAreAGo)
            {
                Submit.Content = "Note: This may take a second";
                await Task.Delay(20);

                //Save information
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
                await Task.Delay(2000);
                Submit.Content = "Submit";
            }
            else 
            {
                //If invalid inputs, display error to user
                NumPlayers_LostFocus(sender, e);
                GameName_LostFocus(sender, e);
                Submit.Content = "Error";
                await Task.Delay(1500);
                Submit.Content = "Submit";

            }

        }

        public static void UpdateFilterOptions(string nameOfGame)
        {
            //Purpose: Checks if game's genre is new to the system. If it is, it adds it to the options displayed in settings

            //Grabs all genres currently in file and the genre of the game being added
            string filePath = "..\\..\\..\\Text_Files\\SavedSettings.txt";
            var ID = UtilityFunctions.GetID(nameOfGame);
            var theGenre = UtilityFunctions.GetAPIGenre(ID);
            
            var textFile = File.ReadAllLines(filePath);
            var currentGenres = textFile[5].Split(",");
            

            //adds genre if it's new
            if (!currentGenres.Contains(theGenre))
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

            
            string line = $"{officialName},,,{numPlayers},,,";

            int count = -1;
            // Append each item from the list, separated by commas
            foreach (PlaceholderTextBoxUC textBox in TextBoxCollection)
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
            if (UtilityFunctions.ShouldShow(officialName, numPlayers, GroupComboBox.Text))
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
            using (StreamWriter writer = new StreamWriter("..\\..\\..\\Text_Files\\LogofPlayedGames.txt", true))
            {
                writer.WriteLine(line);
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
            VerificationLabelCollection.Clear();

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

            //puts current leaderboard into newLeaderboard
            using (StreamReader reader = new StreamReader("..\\..\\..\\Text_Files\\Leaderboard.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null) 
                {
                    String[] parts = line.Split(',');
                    newLeaderboard.Add(parts[0], double.Parse(parts[1]));
                }

            }
                //Loops through from first player (index 2) to last player (index fourth to last (there's a group, date, and notes at the end))
                for (int i = 2; i < int.Parse(gameParts[1]) + 2; i++)
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
            using (StreamWriter writer = new StreamWriter("..\\..\\..\\Text_Files\\Leaderboard.txt"))
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
            VerificationLabelCollection.Clear();

            for (int i = 0; i < count; i++)
            {
            // Add the required number of textboxes
                PlaceholderTextBoxUC textBox = new PlaceholderTextBoxUC {Margin = new Thickness(0, 25, 0, 0) };
                
                textBox.BindedWidth = "100";
                textBox.BindedHeight = "18";
                textBox.BindedWrap = "NoWrap";
                
                textBox.placeholderText = $"{UtilityFunctions.AddOrdinal(i + 1)} place...";

                textBox.VerticalAlignment = VerticalAlignment.Center;
                textBox.HorizontalAlignment = HorizontalAlignment.Center;

                //
                textBox.LostFocus += PlayerName_LostFocus;
                TextBoxCollection.Add(textBox);

                // Add Matching Verification Labels
                Label verificationLabel = new Label();
                verificationLabel.Content = "*Please Enter a Name";
                verificationLabel.HorizontalAlignment = HorizontalAlignment.Center;
                verificationLabel.VerticalAlignment = VerticalAlignment.Center;
                verificationLabel.Foreground = Brushes.Red;
                verificationLabel.Margin = new Thickness(0, 17, 0, 0);
                verificationLabel.Visibility = Visibility.Hidden;
                VerificationLabelCollection.Add(verificationLabel);
            }

            // Adds additional box for gameplay notes
            PlaceholderTextBoxUC notes = new PlaceholderTextBoxUC { Margin = new Thickness(0, 25, 0, 0) };
            notes.BindedWidth = "300";
            notes.BindedHeight = "70";
            notes.placeholderText = "Additional gameplay notes...";
            notes.BindedWrap = "Wrap";

            notes.VerticalAlignment = VerticalAlignment.Center;
            notes.HorizontalAlignment = HorizontalAlignment.Center;
            
            TextBoxCollection.Add(notes);


        }

        private void PlayerName_LostFocus(object sender, RoutedEventArgs e) 
        {
            var placeholderTextBox = sender as PlaceholderTextBoxUC;
            int associatedIndex = TextBoxCollection.IndexOf(placeholderTextBox);
            var verificationLabel = VerificationLabelCollection[associatedIndex];

            if (placeholderTextBox.Input.Text == "")
            {
                placeholderTextBox.BindedBorderColor = Brushes.Red;
                verificationLabel.Visibility = Visibility.Visible;
                playerNamesAreAGo = false;
            }
            else
            {
                placeholderTextBox.BindedBorderColor = SystemColors.ControlDarkBrush;
                verificationLabel.Visibility = Visibility.Hidden;
            }

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
            //Purpose: When NumPlayers is changed, calls UpdateTextBoxCollection with number
            //Do to event listener conflicts, I had to make a PlaceholderTextBox in this .xaml instead of just using a PlaceholderTextBox
            
            //Sets placeholder text to visible or hidden
            if (string.IsNullOrEmpty(NumPlayers.Text))
                tbPlaceholder.Visibility = Visibility.Visible;
            else
                tbPlaceholder.Visibility = Visibility.Hidden;

            //If number is valid, calls UpdateTextBoxCollection
            if (int.TryParse(NumPlayers.Text, out int numberOfTextBoxes) && numberOfTextBoxes >= 2 && numberOfTextBoxes <= 6)
            {
                UpdateTextBoxCollection(numberOfTextBoxes);
            }

        }

        private void SuggestionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SuggestionsListBox.ScrollIntoView(SuggestionsListBox.SelectedItem);
        }
    }
}
