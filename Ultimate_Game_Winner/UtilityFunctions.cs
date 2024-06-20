﻿using System;
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

            if (genre.Substring(genre.Length - 5, 5) == "games")
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


        public static void UpdateFilterInLog()
        {
            
            string resultString = "";

            //Grab User Chosen Genre
            var foo = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");
            var chosenGenre = foo[4].Split(",")[0];

            //Read through all Logged Games
            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\LogofPlayedGames.txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] lineList = line.Split(",,,");

                    //If no filter chosen, all games are set to true
                    if (chosenGenre == "All Genres")
                        lineList[lineList.Length - 2] = "true";
                    //Otherwise, check the game's genre and compare it to the chosenGenre. Set to true/false accordingly
                    else
                    {
          
                        var gameName = lineList[0];
                        var ID = UtilityFunctions.GetID(gameName);
                        var theGenre = UtilityFunctions.GetAPIGenre(ID);

                        if (theGenre == chosenGenre)
                            lineList[lineList.Length - 2] = "true";
                        else
                            lineList[lineList.Length - 2] = "false";
                    }

                    //Building updated version of the text file from the ground up
                    //Add newest line version to the resultString
                    var updatedLine = string.Join(",,,", lineList);
                    resultString += updatedLine + Environment.NewLine;
                }
            }
            //Write the resultString to the text file
            File.WriteAllText("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\LogofPlayedGames.txt", resultString);

        }

        public static string FilterVisibility()
        {
            var settingsText = File.ReadAllLines("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\New\\Ultimate_Game_Winner\\Text_Files\\SavedSettings.txt");
            //If No selected Genre, Filter text remains hidden
            if (settingsText[4] == "All Genres")
                return "Hidden";
            else
                return "Visible";
        }
    }
}
