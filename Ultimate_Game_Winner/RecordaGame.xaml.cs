using System;
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
            string nameOfGame = Name.Text;
            string numPlayers = NumPlayers.Text;
            string firstPlace = First.Text;
            string secondPlace = Second.Text;
            string thirdPlace = Third.Text;
            string fourthPlace = Fourth.Text;

            string stringToSave = $"{nameOfGame},{numPlayers},{firstPlace},{secondPlace},{thirdPlace},{fourthPlace}";

            using (StreamWriter writer = new StreamWriter("C:\\Users\\alexa\\OneDrive\\Desktop\\Senior Project\\Ultimate_Game_Winner\\Ultimate_Game_Winner\\LogofPlayedGames.txt", true))
            {
                    writer.WriteLine(stringToSave);
            }
                    Cancel_Click(sender, e);
                    Submit.Content = "Success!!";
                    await Task.Delay(2500);
                    Submit.Content = "SUBMIT";

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Name.Text = "Name of Game";
            NumPlayers.Text = "# of Players";
            First.Text = "1st Place";
            Second.Text = "2nd Place";
            Third.Text = "3rd Place";
            Fourth.Text = "4th Place";
        }

    }
}
