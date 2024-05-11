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

namespace Ultimate_Game_Winner
{
    /// <summary>
    /// Interaction logic for RecordaGame.xaml
    /// </summary>
    public partial class RecordaGame : Page
    {
        
        public RecordaGame()
        {
            InitializeComponent();
        }

        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            //Gather all needed information
            string nameOfGame = Name.Text;
            string numPlayers = NumPlayers.Text;
            string firstPlace = First.Text;
            string secondPlace = Second.Text;
            string thirdPlace = Third.Text;
            string fourthPlace = Fourth.Text;

            string stringToSave = $"{nameOfGame},{numPlayers},{firstPlace},{secondPlace},{thirdPlace},{fourthPlace}";
            //Save all information to LogofPlayedGames.txt
            using (StreamWriter writer = new StreamWriter("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\LogofPlayedGames.txt", true))
            {
                    writer.WriteLine(stringToSave);
            }
                    //Reset page's text
                    Cancel_Click(sender, e);


                    int gameID = GetID(nameOfGame);
                    (float averagePlaytime, float averageWeight) = GetAPIData(gameID);

                    //Update Leaderboard
                    UpdateLeaderboard(averageWeight, averagePlaytime);

                    Submit.Content = "Success!!";
                    await Task.Delay(2500);
                    Submit.Content = "SUBMIT";

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            //Resets Texts of all TextBox's back to original display
            Name.Text = "Name of Game";
            NumPlayers.Text = "# of Players";
            First.Text = "1st Place";
            Second.Text = "2nd Place";
            Third.Text = "3rd Place";
            Fourth.Text = "4th Place";
        }

        private (float, float) GetAPIData(int gameID)
        {
            XDocument doc;
            using (var client = new HttpClient())
            {
                var endpoint = new Uri($"https://boardgamegeek.com/xmlapi2/thing?id={gameID}&stats=1");
                var result = client.GetAsync(endpoint).Result.Content.ReadAsStringAsync().Result;
                doc = XDocument.Parse(result);
            }
            XElement item = doc.Element("items").Element("item");
            string averageWeightString = item.Element("statistics").Element("ratings").Element("averageweight").Attribute("value").Value;
            var minPlaytimeString = item.Element("minplaytime").Attribute("value").Value;
            var maxPlaytimeString = item.Element("maxplaytime").Attribute("value").Value;


            float averageWeight = float.Parse(averageWeightString);
            float minPlaytime = float.Parse(minPlaytimeString);
            float maxPlaytime = float.Parse(maxPlaytimeString);
            float averagePlaytime = (minPlaytime + maxPlaytime) / 2;


            return (averagePlaytime, averageWeight);
        }

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            //Resets Texts of all TextBox's back to original display
            int gameID = GetID(Name.Text);
            (float averagePlaytime, float averageWeight) = GetAPIData(gameID);

            Name.Text = "Test";
            NumPlayers.Text = "Test";
            First.Text = "Test";
            Second.Text = "Test";
            Third.Text = "Test";
            Fourth.Text = "Test";
            //TestAPIText.Text = gameID.ToString();



            CalculatePoints(averageWeight, averagePlaytime, int.Parse(NumPlayers.Text), 2);

            Name.Text = averagePlaytime.ToString();
            NumPlayers.Text = averageWeight.ToString();


            //using (var client = new HttpClient())
            //{
            //    var endpoint = new Uri($"https://boardgamegeek.com/xmlapi2/thing?id={gameID}&stats=1");
            //    var result = client.GetAsync(endpoint).Result.Content.ReadAsStringAsync().Result;
            //    TestAPIText.Text = result;
            //    //var result = client.GetAsync(endpoint).Result;
            //    //var json = result.Content.ReadAsStringAsync().Result;
            //}
        }

        private int GetID(string nameOfGame)
        {
            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\GamesAndIDs.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] lineList = line.Split(",");
                    if (lineList[1] == $"\"{nameOfGame}\"")
                    {
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
            //Parameters:
            //    Weight: Weight of game
            //    Playtime: Playtime of game
            //    Placement_Percentage: A percent based off the placement of a player
            //Return:
            //    Point value

            var weightFactor = (.25) * weight + (.75);
            var playtimeFactor = ((-0.00487789 * Math.Pow(playtime, 2)) + (2.02538 * playtime) - (1.95751)) / 100;
            var placementFactor = CalculatePlacementPercentage(placement, numOfPlayers);
            
            var points = 10 * weightFactor * playtimeFactor * placementFactor;

            return points;

            double CalculatePlacementPercentage(int placement, int numOfPlayers)
            {
                using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\PlacementPercentages.txt"))
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
                    return -1;
                }
            }
        }

        private void UpdateLeaderboard(float weight, float playtime)
        {
            
            Dictionary<String, double> newLeaderboard = new Dictionary<string, double>();


            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\LogofPlayedGames.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null) 
                {
                    String[] parts = line.Split(',');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        //skips over Game Name and Number of Players
                        if (i != 0 && i != 1)
                        {
                            double points = CalculatePoints(weight, playtime, int.Parse(parts[1]), (i-1));
                            //checks if person is already in the dictionary and adds them accordingly
                            if (!newLeaderboard.ContainsKey(parts[i]))
                            {
                                newLeaderboard.Add(parts[i], points);
                            }

                            else
                            {
                                newLeaderboard[parts[i]] += points;
                            }
                            //5/8/24 Points assigned is currently -2, -3, -4, -5 points for 1st, 2nd, 3rd, 4th respectively
                            //This is placeholder to get it up and running; number of points assigned will be a more complicated process
                            //that depends upon information that is planned to be obtained through use of an API
                        }
                    }
                }
            }


            var sortedDictionary = newLeaderboard.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            using (StreamWriter writer = new StreamWriter("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Leaderboard.txt"))
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

    }
}
