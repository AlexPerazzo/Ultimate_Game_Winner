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
    /// When the User Clicks on a Gameplay, Information about that gameplay
    /// (including players participating, their placements, and information about the game itself)
    /// will be displayed through this Window
    /// </summary>
    public partial class AdditionalGameplayInfo : Window
    {
        private string[] allInfo;
       
        public string gameName { get; set; }
        public string weight { get; set; }
        public string playtime { get; set; }
        public string genre { get; set; }
        public string additionalNotes { get; set; }
        public string date { get; set; }

        

       
        public AdditionalGameplayInfo(string[] alltheInfo)
        {
            InitializeComponent();
            
            this.DataContext = this;
            allInfo = alltheInfo;
            
            Loaded += LoadPlayers;

        }
        

        
        private void LoadPlayers(object sender, RoutedEventArgs e)
        {
            
            //Gather Information from API
            
            var ID = UtilityFunctions.GetID(allInfo[0]);
            (float thePlaytime, float theWeight) = UtilityFunctions.GetAPIData(ID);
            var (URL, theGenre) = UtilityFunctions.GetAPIImageGenre(ID);
            var imageUri = new Uri(URL);
            var bitmap = new BitmapImage(imageUri);
            BoardGameImage.Source = bitmap;
            var fixedGenre = char.ToUpper(theGenre[0]) + theGenre.Substring(1, theGenre.Length-6);

            //Set all Data Bindings
            genre = $"Genre: {fixedGenre}";
            weight = $"Weight: {theWeight.ToString("0.00")}/5";
            playtime = $"Average Playtime: {thePlaytime.ToString()} min";
            gameName = allInfo[0];
            date = $"Date Recorded: {allInfo[allInfo.Length-1]}";
            additionalNotes = $"Additional Notes: {allInfo[allInfo.Length-2]}";
            

            //Populate Players StackPanel (loop through to calculate proper number of points)
            for (int i = 2; i < allInfo.Length - 2; i++)
            {
                var points = RecordaGame.CalculatePoints(theWeight, thePlaytime, int.Parse(allInfo[1]), i-1);

                LeaderboardPanel playerPanel = new LeaderboardPanel();
                //playerPanel.Padding = new Thickness(5);
                playerPanel.PlayerName = allInfo[i];
                playerPanel.Placement = $"{i-1}";
                playerPanel.Points = $"received {points} pts";
                PlayersPanel.Children.Add(playerPanel);
            }
            
            //Refresh Window so set Data Bindings will apply
            UtilityFunctions.RefreshFramework(this);
        }

        private void DeleteGameplayBtn_Click(object sender, RoutedEventArgs e)
        {
            AreYouSure areYouSure = new AreYouSure(false, allInfo, this);
            areYouSure.Show();
        }
    }
}
