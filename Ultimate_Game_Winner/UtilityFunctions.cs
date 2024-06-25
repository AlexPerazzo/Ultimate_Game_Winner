using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Ultimate_Game_Winner
{
    internal class UtilityFunctions
    {
        public static string ReturnOfficialGameName(string nameOfGame)
        {
            //Purpose: User sometimes adds a game without proper capitalization;
            //this fixes that so other parts of the code (and the GUI) are better

            //reads through all the game names
            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\GamesAndIDs.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] lineList = line.Split(",");
                    //stops when inputed name matches up with real game name
                    if (lineList[1].ToLower() == nameOfGame.ToLower())
                    {
                        //returns real game name
                        return lineList[1];
                    }
                }
                //if no real game name matched the inputted game name, the string "error" is returned
                return "Error";
            }
        }


        public static bool DoesGameExist(string nameOfGame)
        {
            //Purpose: Easy way to check if game is in our text file for purposes of verification

            //Reads list of all games and their ids
            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\GamesAndIDs.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] lineList = line.Split(",");
                    //stops when the names match up
                    if (lineList[1].ToLower() == nameOfGame.ToLower())
                    {
                        //if game exists return true
                        return true;
                    }
                }
            }
            //otherwise return false
            return false;
        }


        public static string CapitalizeEachWord(string input)
        {
            //Purpose: Helper function to help catch user input error in regards to names
            //Capitalizes the first letter of every word in a string
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

        public static void RefreshFramework(FrameworkElement element)
        {
            //Purpose: Certain GUI sections get their info after the page has been loaded
            //This allows a reload to occur so those parts get loaded in

            //turns the data context off and on again
            var oldDataContext = element.DataContext;
            element.DataContext = null;
            element.DataContext = oldDataContext;
        }
        public static string AddOrdinal(int num)
        {
            //Purpose: Add proper ending for purposes of displaying placements
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

        public static (float playtime, float weight) GetAPIData(int gameID)
        {
            //Purpose: Uses API and grabs the needed information: weight and playtime
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

        public static string GetAPIImage(int gameID)
        {
            //Purpose: Uses API to grab an image, which we can display
            XDocument doc;
            //Goes to API
            using (var client = new HttpClient())
            {
                var endpoint = new Uri($"https://boardgamegeek.com/xmlapi2/thing?id={gameID}&stats=1");
                var result = client.GetAsync(endpoint).Result.Content.ReadAsStringAsync().Result;
                doc = XDocument.Parse(result);
            }

            //sorts through that information to grab image of game.
            
            XElement? item = doc.Element("items").Element("item");
            var imageURL = item.Element("thumbnail").Value;

            return imageURL;
        }
        public static string GetAPIGenre(int gameID)
        {
            //Purpose: Uses API to grab a game's genre
            //Used for Filter purposes and for AdditionalGameplayInfo window

            XDocument doc;
            //Goes to API
            using (var client = new HttpClient())
            {
                var endpoint = new Uri($"https://boardgamegeek.com/xmlapi2/thing?id={gameID}&stats=1");
                var result = client.GetAsync(endpoint).Result.Content.ReadAsStringAsync().Result;
                doc = XDocument.Parse(result);
            }

            //sorts through that information to grab image and genre of game.
            string genre;
            XElement? item = doc.Element("items").Element("item");
            
            var almostGenre = item.Element("statistics").Element("ratings").Element("ranks").Elements("rank").ElementAtOrDefault(1);
            if (almostGenre == null)
                genre = "familygames"; 
            
            else
                genre = almostGenre.Attribute("name").Value;

            
            string fixedGenre;

            //Some genres end with the word "games" others don't
            //If they do, cut it off the end and capitalize the first letter

            if (genre.Length < 5)
                fixedGenre = char.ToUpper(genre[0]) + genre.Substring(1, genre.Length - 1);

            else if (genre.Substring(genre.Length - 5, 5) == "games")
                fixedGenre = char.ToUpper(genre[0]) + genre.Substring(1, genre.Length - 6);

            //If they don't, just capitalize the first letter

            else
                fixedGenre = char.ToUpper(genre[0]) + genre.Substring(1, genre.Length - 1);


            
            return fixedGenre;
        }
        
        public static int GetID(string nameOfGame)
        {
            //Purpose: Grab's ID of a game. The ID is critical for access bgg's API

            //Reads list of all games and their ids
            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\GamesAndIDs.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] lineList = line.Split(",");
                    //stops when the names match up
                    if (lineList[1].ToLower() == nameOfGame.ToLower())
                    {
                        //returns associated id
                        int gameID = int.Parse(lineList[0]);
                        return gameID;
                    }
                }
            }
            // Does not work if game does not exist
            return -1;
        }

        public static string FilterVisibility()
        {
            var settingsText = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");
            //If No selected Filters, Filter text remains hidden
            if (settingsText[4] == "All Genres,All Player Counts,All Weights,All Playtimes,All Groups")
                return "Hidden";
            else
                return "Visible";
        }

        public static bool CheckGenre(string chosenGenre, string genre)
        {
            if (chosenGenre == "All Genres")
                return true;
            else
                return (chosenGenre == genre);
        }

        public static bool CheckPlayerCount(string chosenPlayerCount, string playercount)
        {
            switch(chosenPlayerCount)
            {
                case "All Player Counts":
                    return true;

                case "2 Players":
                    return (playercount == "2");

                case "3 Players":
                    return (playercount == "3");

                case "4 Players":
                    return (playercount == "4");

                case "5 Players":
                    return (playercount == "5");

                case "6 Players":
                    return (playercount == "6");

                default:
                    throw new InvalidOperationException("Player Count went wrong.");
                    return true;
            }
        }

        public static bool CheckWeight(string chosenWeight, float weight)
        {
            switch (chosenWeight)
            {
                case "All Weights":
                    return true;

                case "1-2 (Light)":
                    return (weight >= 1 && weight <= 2);

                case "2-3 (Medium-Light)":
                    return (weight >= 2 && weight <= 3);

                case "3-4 (Medium-Heavy)":
                    return (weight  >= 3 && weight <= 4);

                case "4-5 (Heavy)":
                    return (weight >= 4);

                default:
                    throw new InvalidOperationException("Weight went wrong.");
                    return true;
            }
        }

        public static bool CheckPlaytime(string chosenPlaytime, float playtime)
        {

            switch (chosenPlaytime)
            {
                case "All Playtimes":
                    return true;

                case "Less than 30 min":
                    return (playtime <= 30);

                case "30-60 min":
                    return (playtime >= 30 && playtime <= 60);

                case "60-90 min":
                    return (playtime >= 60 && playtime <= 90);

                case "90-120 min":
                    return (playtime >= 90 && playtime <= 120);

                case "Longer than 120 min":
                    return (playtime > 120);

                default:
                    throw new InvalidOperationException("Playtime went wrong.");
                    return false;
            }
        }

        public static bool CheckGroup(string chosenGroup, string group)
        {
            if (chosenGroup == "All Groups")
                return true;
            else
                return (chosenGroup == group);
        }
        public static bool ShouldFilter(string nameOfGame, string playerCount, string group)
        {

            bool filterBool = false;
            var textFile = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");
            var chosenFilters = textFile[4].Split(",");

            var chosenGenre = chosenFilters[0];
            var chosenPlayerCount = chosenFilters[1];
            var chosenWeight = chosenFilters[2];
            var chosenPlaytime = chosenFilters[3];
            var chosenGroup = chosenFilters[4];

            if (chosenGenre == "All Genres" && chosenPlayerCount == "All Player Counts" && chosenWeight == "All Weights" && chosenPlaytime == "All Playtimes" && chosenGroup == "All Groups")
            {
                filterBool = true;
            }
            else
            {
                var ID = UtilityFunctions.GetID(nameOfGame);
                var theGenre = UtilityFunctions.GetAPIGenre(ID);
                (float playtime, float weight) = UtilityFunctions.GetAPIData(ID);

                if (CheckGenre(chosenGenre, theGenre) && CheckPlayerCount(chosenPlayerCount, playerCount) && CheckWeight(chosenWeight, weight) && CheckPlaytime(chosenPlaytime, playtime) && CheckGroup(chosenGroup, group))
                    filterBool = true;
                
                else
                    filterBool = false;

            }
            return filterBool;
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

        public static void UpdateFilterInLog()
        {
            string resultString = "";

            //Read through all Logged Games
            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\LogofPlayedGames.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] lineList = line.Split(",,,");
                    var gameName = lineList[0];
                    var playerCount = lineList[1];
                    var group = lineList[lineList.Length - 3];

                    if (ShouldFilter(gameName, playerCount, group))
                        lineList[lineList.Length - 2] = "true";
                    else
                        lineList[lineList.Length - 2] = "false";

                    //Building updated version of the text file from the ground up
                    //Add newest line version to the resultString
                    var updatedLine = string.Join(",,,", lineList);
                    resultString += updatedLine + Environment.NewLine;
                }
            }
            //Write the resultString to the text file
            File.WriteAllText("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\LogofPlayedGames.txt", resultString);

        }
    }
}
