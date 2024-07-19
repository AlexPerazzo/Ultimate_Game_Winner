using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using Ultimate_Game_Winner.Main_Pages;
using Ultimate_Game_Winner.UserControls;


namespace Ultimate_Game_Winner.Windows
{
    /// <summary>
    /// Interaction logic for EditGameplayWindow.xaml
    /// </summary>
    public partial class EditGameplayWindow : Window
    {

        private bool numPlayersIsAGo = false;
        private bool gameNameIsAGo = false;
        private bool playerNamesAreAGo = true;

        public ObservableCollection<PlaceholderTextBoxUC> TextBoxCollection { get; set; }
        public ObservableCollection<Label> VerificationLabelCollection { get; set; }

        private List<string> items;
        private List<string> gameNames;

        private string[] allInfo;

        private Window oldWindow;
        
        public EditGameplayWindow(string[] _allInfo, Window _oldWindow)
        {
            InitializeComponent();
            this.DataContext = this;
            TextBoxCollection = new ObservableCollection<PlaceholderTextBoxUC>();
            VerificationLabelCollection = new ObservableCollection<Label>();
            NameTextBoxesControl.ItemsSource = TextBoxCollection;
            NameLabelVerificationControl.ItemsSource = VerificationLabelCollection;

            items = File.ReadAllLines("..\\..\\..\\Text_Files\\GamesAndIDs.txt").ToList();
            gameNames = items.Select(line => line.Split(',')[1]).ToList();
            allInfo = _allInfo;
            oldWindow = _oldWindow;

            LoadGameplayInfo(allInfo);
            GameName.Input.TextChanged += InputTextBox_TextChanged;
            GameName.Input.PreviewKeyDown += GameName_PreviewKeyDown;


        }

        private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e)
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








        private void LoadGameplayInfo(string[] allInfo)
        {
            //Purpose: Fills in current game information for the user to edit

            GameName.Input.Text = allInfo[0];
            SuggestionsListBox.SelectedItem = allInfo[0];
            NumPlayers.Text = allInfo[1];
            var count = 1;
            foreach (PlaceholderTextBoxUC textBox in TextBoxCollection)
            {
                count++;
                textBox.Input.Text = allInfo[count];

                if (allInfo[count] == "N/A")
                    textBox.Input.Text = "";
            }

            GroupComboBox.Text = allInfo[allInfo.Length - 3];
        }


       

        private void UpdateLog(string lineToSave)
        {
            //Purpose: Replaces old info with new info

            //Finds line that's the old info
            string[] allLoggedGames = File.ReadAllLines("..\\..\\..\\Text_Files\\LogofPlayedGames.txt");
            string stringAllInfo = String.Join(",,,", allInfo);
            int index = Array.IndexOf(allLoggedGames, stringAllInfo);

            //Updates it with the new info
            allLoggedGames[index] = lineToSave;

            //Save all information to LogofPlayedGames.txt
            File.WriteAllLines("..\\..\\..\\Text_Files\\LogofPlayedGames.txt", allLoggedGames);

        }

        private (string, bool) CompileInfoToSave(string nameOfGame, string numPlayers)
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
            string previousDate = allInfo[allInfo.Length - 1];
            
            line += previousDate;

            return (line, filterBool);
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
                PlaceholderTextBoxUC textBox = new PlaceholderTextBoxUC { Margin = new Thickness(0, 25, 0, 0) };

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
            //Purpose: Verification for player's names

            //Finds associated Verification Label
            var placeholderTextBox = sender as PlaceholderTextBoxUC;
            int associatedIndex = TextBoxCollection.IndexOf(placeholderTextBox);
            var verificationLabel = VerificationLabelCollection[associatedIndex];

            //Marks it as red and sets bool to false if false
            if (placeholderTextBox.Input.Text == "")
            {
                placeholderTextBox.BindedBorderColor = Brushes.Red;
                verificationLabel.Visibility = Visibility.Visible;
                playerNamesAreAGo = false;
                //placeholderTextBox.Margin = new Thickness(0, 25, 30,0);
            }
            //Otherwise sets it back to normal
            else
            {
                placeholderTextBox.BindedBorderColor = SystemColors.ControlDarkBrush;
                verificationLabel.Visibility = Visibility.Hidden;
                //placeholderTextBox.Margin = new Thickness(0, 25, 0, 0);

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
                //GameName.Margin = new Thickness(0,41,165,0);
            }
            //If valid, puts things to normal
            //Turns necessary variable for submitting to true
            else
            {
                GameName.Input.BorderBrush = SystemColors.ControlDarkBrush;
                GameNameVerification.Visibility = Visibility.Hidden;
                gameNameIsAGo = true;
                //GameName.Margin = new Thickness(0, 41, 0, 0);

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
                //NumPlayers.Margin = new Thickness(0, 81, 185, 0);
                //tbPlaceholder.Margin = new Thickness(0,81,185,0);
            }
            //If valid, puts things to normal
            //Turns necessary variable for submitting to true
            else
            {
                NumPlayers.BorderBrush = SystemColors.ControlDarkBrush;
                NumPlayersVerification.Visibility = Visibility.Hidden;
                numPlayersIsAGo = true;
                //NumPlayers.Margin = new Thickness(0, 81, 0, 0);
                //tbPlaceholder.Margin = new Thickness(0, 81, 0, 0);
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

        private async void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            //Purpose: Collects and replaces old info with new info
            
            //Verifies all player names -- sets it to true to start and then sets it to false if any one is empty
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

            NumPlayers_LostFocus(sender, e);
            GameName_LostFocus(sender, e);

            //If verifications all pass, update info
            if (gameNameIsAGo && numPlayersIsAGo && playerNamesAreAGo)
            {
                var (lineToSave, ShouldAddPoints) = CompileInfoToSave(GameName.Input.Text, NumPlayers.Text);

                if (lineToSave != String.Join(",,,", allInfo))
                {
                    SaveBtn.Content = "This may take a second";
                    await Task.Delay(20);
                    UpdateLog(lineToSave);

                    LeaderboardPage.RefreshLeaderboard();

                    //If game is a new genre, update that option in settings
                    if (GameName.Input.Text != allInfo[0])
                        RecordGameplayPage.UpdateFilterOptions(GameName.Input.Text);

                    //Displays Success and then converts back to normal
                    SaveBtn.Content = "Success!!";

                    oldWindow.Close();
                    this.Close();

                }
                else
                {
                    SaveBtn.Content = "No change detected";
                    await Task.Delay(1500);
                    SaveBtn.Content = "Submit";
                }



            }
            else
            {
                //If invalid inputs, display error to user
                SaveBtn.Content = "Error";
                await Task.Delay(1500);
                SaveBtn.Content = "Submit";

            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            //Purpose: Resets Texts of all TextBox's back to original display
            LoadGameplayInfo(allInfo);

            //Resets verification error messages
            NumPlayers_LostFocus(NumPlayers, e);
            GameName_LostFocus(GameName, e);

            int total = TextBoxCollection.Count;
            int currentIndex = 0;

            foreach (PlaceholderTextBoxUC textBox in TextBoxCollection)
            {
                //skip the additional game notes
                if (currentIndex < total - 1)
                {
                    PlayerName_LostFocus(textBox, e);
                }
                currentIndex++;
            }

        }

        private void SuggestionsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SuggestionsListBox.ScrollIntoView(SuggestionsListBox.SelectedItem);
        }
    }
}
