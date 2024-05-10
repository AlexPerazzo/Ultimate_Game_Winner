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
            using (StreamWriter writer = new StreamWriter("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\Ultimate_Game_Winner\\Ultimate_Game_Winner\\LogofPlayedGames.txt", true))
            {
                    writer.WriteLine(stringToSave);
            }
                    //Reset page's text
                    Cancel_Click(sender, e);
                    
                    //Update Leaderboard
                    CalculatePoints();

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

        private void Test_Click(object sender, RoutedEventArgs e)
        {
            //Resets Texts of all TextBox's back to original display
            Name.Text = "Test";
            NumPlayers.Text = "Test";
            First.Text = "Test";
            Second.Text = "Test";
            Third.Text = "Test";
            Fourth.Text = "Test";
            using (var client = new HttpClient())
            {
                var endpoint = new Uri("https://boardgamegeek.com/xmlapi2/thing?id=342942&stats=1");
                var result = client.GetAsync(endpoint).Result.Content.ReadAsStringAsync().Result;
                TestAPIText.Text = result;
                //var result = client.GetAsync(endpoint).Result;
                //var json = result.Content.ReadAsStringAsync().Result;
            }
        }

        private void CalculatePoints()
        {
            
            Dictionary<String, int> newLeaderboard = new Dictionary<string, int>();


            using (StreamReader reader = new StreamReader("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\Ultimate_Game_Winner\\Ultimate_Game_Winner\\LogofPlayedGames.txt"))
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
                            //checks if person is already in the dictionary and adds them accordingly
                            if (!newLeaderboard.ContainsKey(parts[i]))
                            {
                                newLeaderboard.Add(parts[i], i * -1);
                            }

                            else
                            {
                                newLeaderboard[parts[i]] += i * -1;
                            }
                            //5/8/24 Points assigned is currently -2, -3, -4, -5 points for 1st, 2nd, 3rd, 4th respectively
                            //This is placeholder to get it up and running; number of points assigned will be a more complicated process
                            //that depends upon information that is planned to be obtained through use of an API
                        }
                    }
                }
            }

            
            var sortedDictionary = newLeaderboard.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            using (StreamWriter writer = new StreamWriter("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\Ultimate_Game_Winner\\Ultimate_Game_Winner\\Leaderboard.txt"))
            {

                writer.Write(string.Empty);
                int i = 0;
                foreach (var entry in sortedDictionary)
                {
                    i += 1;
                    //Write onto Leaderboard.txt by iterating through a sorted dictionary and putting their placement, name, and points
                    writer.WriteLine($"#{i} {entry.Key} with {entry.Value} points");
                }
            }
        }

    }
}
