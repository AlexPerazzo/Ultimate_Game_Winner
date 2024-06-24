using System;
using System.Collections.Generic;
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

namespace Ultimate_Game_Winner.UserControls_and_Windows
{
    /// <summary>
    /// Interaction logic for PlaceholderTextBox.xaml
    /// </summary>
    public partial class PlaceholderTextBox : UserControl
    {
        public PlaceholderTextBox()
        {
            InitializeComponent();
        }

        

        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(Input.Text))
                tbPlaceholder.Visibility = Visibility.Visible;
            else
                tbPlaceholder.Visibility = Visibility.Hidden;
        }
    }
}
