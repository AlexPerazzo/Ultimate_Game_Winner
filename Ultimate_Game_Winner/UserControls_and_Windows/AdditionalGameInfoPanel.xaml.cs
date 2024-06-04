using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using System.Xml.Linq;
using Ultimate_Game_Winner.Main_Pages;

namespace Ultimate_Game_Winner.UserControls_and_Windows
{
    /// <summary>
    /// Interaction logic for AdditionalGameInfoPanel.xaml
    /// </summary>
    public partial class AdditionalGameInfoPanel : Window
    {
        private string[] allInfo;
        public string gameName { get; set; }
        public string weight { get; set; }
        public string playtime { get; set; }

        public string genre { get; set; }

        public string additionalNotes { get; set; }
        public string date { get; set; }

        public AdditionalGameInfoPanel(string[] alltheInfo)
        {
            InitializeComponent();

            this.DataContext = this;
            allInfo = alltheInfo;
            
            Loaded += LoadPlayers;

        }
        private (string, string) GetAPIImageGenre(int gameID)
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
            


            var imageURL = item.Element("thumbnail").Value;
            var genre = item.Element("statistics").Element("ratings").Element("ranks").Elements("rank").ElementAt(1).Attribute("name").Value;


            return (imageURL, genre);
        }

        private void RefreshWindow()
        {
            // Assuming 'this' is your window
            var oldDataContext = this.DataContext;
            this.DataContext = null;
            this.DataContext = oldDataContext;
        }
        private void LoadPlayers(object sender, RoutedEventArgs e)
        {
            
            RecordaGame recordaGame = new RecordaGame();
            var ID = recordaGame.GetID(allInfo[0]);
            (float thePlaytime, float theWeight) = recordaGame.GetAPIData(ID);
            var (URL, theGenre) = GetAPIImageGenre(ID);
            var imageUri = new Uri(URL);
            var bitmap = new BitmapImage(imageUri);
            BoardGameImage.Source = bitmap;

            var fixedGenre = char.ToUpper(theGenre[0]) + theGenre.Substring(1, theGenre.Length-6);

            genre = $"Genre: {fixedGenre}";
            weight = $"Weight: {theWeight.ToString("0.00")}/5";
            playtime = $"Average Playtime: {thePlaytime.ToString()} min";
            gameName = allInfo[0];
            date = $"Date Recorded: {allInfo[allInfo.Length-1]}";
            additionalNotes = $"Additional Notes: {allInfo[allInfo.Length-2]}";
            //AdditionalNotes.Text = URL;


            for (int i = 2; i < allInfo.Length - 2; i++)
            {
                var points = recordaGame.CalculatePoints(theWeight, thePlaytime, int.Parse(allInfo[1]), i-1);

                LeaderboardPanel playerPanel = new LeaderboardPanel();
                //playerPanel.Padding = new Thickness(5);
                playerPanel.PlayerName = allInfo[i];
                playerPanel.Placement = $"{i-1}";
                playerPanel.Points = $"received {points} pts";
                PlayersPanel.Children.Add(playerPanel);
            }
            RefreshWindow();
        }
    }
}
