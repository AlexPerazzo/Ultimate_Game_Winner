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
    /// Interaction logic for RecordaGame.xaml
    /// </summary>
    public partial class RecordaGame : Page
    {
        public ObservableCollection<TextBox> TextBoxCollection { get; set; }
        public RecordaGame()
        {
            InitializeComponent();
            TextBoxCollection = new ObservableCollection<TextBox>();
            NameTextBoxesControl.ItemsSource = TextBoxCollection;
        }

        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            string nameOfGame = GameName.Text; string numPlayers = NumPlayers.Text;
            string lineSaved = SaveToLog(nameOfGame, numPlayers);
            //Reset page's text
            Cancel_Click(sender, e);


            //Update Leaderboard
            UpdateLeaderboard(lineSaved);

            //Displays Success and then converts back to normal
            Submit.Content = "Success!!";
            await Task.Delay(2500);
            Submit.Content = "SUBMIT";

        }

        private static string CapitalizeEachWord(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            string[] words = input.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                }
            }

            return string.Join(' ', words);
        }
        private string SaveToLog(string nameOfGame, string numPlayers)
        {
            
            
            // Append each item from the list, separated by commas
            string line = $"{nameOfGame},,,{numPlayers},,,";
            foreach (TextBox textBox in TextBoxCollection)
            {
                if (textBox.Text == "" || textBox.Text == "Additional gameplay notes... (leave blank or don't touch if none are wanted)")
                    line += "N/A,,,";
                else
                    line += CapitalizeEachWord(textBox.Text) + ",,,";
                
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
            return line;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //Resets Texts of all TextBox's back to original display
            GameName.Text = "Name of Game";
            NumPlayers.Text = "# of Players";
            TextBoxCollection.Clear();
        }

        private (float playtime, float weight) GetAPIData(int gameID)
        {
            //Uses API and grabs the needed information: weight and playtime
            XDocument doc;
            //Goes to API and gets information
            using (var client = new HttpClient())
            {
                var endpoint = new Uri($"https://boardgamegeek.com/xmlapi2/thing?id={gameID}&stats=1");
                var result = client.GetAsync(endpoint).Result.Content.ReadAsStringAsync().Result;
                doc = XDocument.Parse(result);
            }

            XElement? item = doc.Element("items").Element("item");

            //sorts thorugh that information to grab what we need.
            string? averageWeightString = item.Element("statistics").Element("ratings").Element("averageweight").Attribute("value").Value;
            var minPlaytimeString = item.Element("minplaytime").Attribute("value").Value;
            var maxPlaytimeString = item.Element("maxplaytime").Attribute("value").Value;


            float averageWeight = float.Parse(averageWeightString); float minPlaytime = float.Parse(minPlaytimeString); float maxPlaytime = float.Parse(maxPlaytimeString); float averagePlaytime = (minPlaytime + maxPlaytime) / 2;


            return (averagePlaytime, averageWeight);
        }


        private int GetID(string nameOfGame)
        {
            //Reads list of all games and their ids
            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\GamesAndIDs.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] lineList = line.Split(",");
                    //stops when the names match up
                    if (lineList[1] == nameOfGame)
                    {
                        //returns associated id
                        int gameID = int.Parse(lineList[0]);
                        return gameID;
                    }
                }
            }
            return -1;
        }

        private double CalculatePoints(float weight, float playtime, int numOfPlayers, int placement)
        {
            //Creates a point value, either positive or negative.

            
            var weightFactor = 3 * Math.Log10(weight);
            var playtimeFactor = 2 * Math.Log10(playtime / 10);
            var placementFactor = 1 * CalculatePlacementPercentage(placement, numOfPlayers);
            
            var points = (weightFactor + playtimeFactor) * placementFactor;
            
            return points;

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
            string[] gameParts = gameplayToAdd.Split(",,,");

            int gameID = GetID(gameParts[0]);
            (float averagePlaytime, float averageWeight) = GetAPIData(gameID);


            Dictionary<String, double> newLeaderboard = new Dictionary<string, double>();

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
                for (int i = 2; i < gameParts.Length - 2; i++)
                {

                    double points = CalculatePoints(averageWeight, averagePlaytime, int.Parse(gameParts[1]), i-1);
                    
                    //checks if person is already in the dictionary and adds them accordingly
                    if (!newLeaderboard.ContainsKey(gameParts[i]))
                    {
                        newLeaderboard.Add(gameParts[i], points);
                    }

                    else
                    {
                        newLeaderboard[gameParts[i]] += points;
                    }
                }
            
            var sortedDictionary = newLeaderboard.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            using (StreamWriter writer = new StreamWriter("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\Leaderboard.txt"))
            {
                
                writer.Write(string.Empty);
                int i = 0;
                foreach (var entry in sortedDictionary)
                {
                    i += 1;
                    //Write onto Leaderboard.txt by iterating through a sorted dictionary and putting their placement, name, and points
                    var key = entry.Key;
                    var value = entry.Value;
                    string writeThis = $"{key},{value:F2}";
                    writer.WriteLine(writeThis);
                }
            }
        }

        public void RefreshLeaderboard()
        {

            Dictionary<String, double> newLeaderboard = new Dictionary<string, double>();


            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\LogofPlayedGames.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    String[] parts = line.Split(",,,");
                    for (int i = 0; i < parts.Length; i++)
                    {
                        //skips over Game Name and Number of Players
                        if (i != 0 && i != 1)
                        {
                            int ID = GetID(parts[0]);
                            (float weight, float playtime) = GetAPIData(ID);
                            double points = CalculatePoints(weight, playtime, int.Parse(parts[1]), (i - 1));


                            //checks if person is already in the dictionary and adds them accordingly
                            if (!newLeaderboard.ContainsKey(parts[i]))
                            {
                                newLeaderboard.Add(parts[i], points);
                            }

                            else
                            {
                                newLeaderboard[parts[i]] += points;
                            }

                        }
                    }
                }
            }


            var sortedDictionary = newLeaderboard.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            using (StreamWriter writer = new StreamWriter("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\Leaderboard.txt"))
            {

                writer.Write(string.Empty);
                int i = 0;
                foreach (var entry in sortedDictionary)
                {
                    i += 1;
                    //Write onto Leaderboard.txt by iterating through a sorted dictionary and putting their placement, name, and points
                    var key = entry.Key;
                    var value = entry.Value;
                    string writeThis = $"#{i} {key} with {value} points";
                    writer.WriteLine(writeThis);
                }
            }
        }
        private void NumPlayers_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(NumPlayers.Text, out int numberOfTextBoxes) && numberOfTextBoxes >= 2 && numberOfTextBoxes <= 6)
            {
                // Update the collection based on the number of textboxes required
                UpdateTextBoxCollection(numberOfTextBoxes);
            }
            else
            {
                
            }
        }

        public static string AddOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
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
                textBox.Text = $"{AddOrdinal(i+1)} place...";
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

        
    }
}
