using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Ultimate_Game_Winner
{
    internal class UtilityFunctions
    {
        public static string ReturnOfficialGameName(string nameOfGame)
        {
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
            //Helper function to help catch user input error in regards to names
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
            var oldDataContext = element.DataContext;
            element.DataContext = null;
            element.DataContext = oldDataContext;
        }
        public static string AddOrdinal(int num)
        {
            //Helper function for placements
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

        public static string GetAPIImage(int gameID)
        {
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

            return genre;
        }
        public static int GetID(string nameOfGame)
        {
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

        public static string FormatGenre(string theGenre)
        {
            string fixedGenre;
            if (theGenre.Substring(theGenre.Length - 5, 5) == "games")
                fixedGenre = char.ToUpper(theGenre[0]) + theGenre.Substring(1, theGenre.Length - 6);
            else
                fixedGenre = char.ToUpper(theGenre[0]) + theGenre.Substring(1, theGenre.Length - 1);

            return fixedGenre;
        }

        public static void UpdateFilterInLog()
        {
            string resultString = "";
            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\LogofPlayedGames.txt"))
            {
                var foo = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");
                var chosenGenre = foo[4];
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] lineList = line.Split(",,,");
                    if (chosenGenre == "All Games")
                        lineList[lineList.Length - 2] = "true";
                    else
                    {
                        var gameName = lineList[0];
                        var ID = UtilityFunctions.GetID(gameName);
                        var theGenre = UtilityFunctions.GetAPIGenre(ID);

                        if (UtilityFunctions.FormatGenre(theGenre) == chosenGenre)
                            lineList[lineList.Length - 2] = "true";
                        else
                            lineList[lineList.Length - 2] = "false";
                    }

                    var updatedLine = string.Join(",,,", lineList);
                    resultString += updatedLine + Environment.NewLine;
                }
            }

            File.WriteAllText("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\LogofPlayedGames.txt", resultString);

        }

        public static string FilterVisibility()
        {
            var settingsText = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");

            if (settingsText[4] == "All Games")
                return "Hidden";
            else
                return "Visible";
        }
    }
}
