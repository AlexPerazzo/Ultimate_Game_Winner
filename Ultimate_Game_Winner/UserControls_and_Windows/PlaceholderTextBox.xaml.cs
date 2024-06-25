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
using Ultimate_Game_Winner.Main_Pages;

namespace Ultimate_Game_Winner.UserControls_and_Windows
{
    /// <summary>
    /// Interaction logic for PlaceholderTextBox.xaml
    /// </summary>
    /// 

    public partial class PlaceholderTextBox : UserControl
    {

        public string placeholderText { get; set; }
        public string BindedHeight { get; set; }
        public string BindedWidth { get; set; }
        public string BindedWrap {  get; set; }
        public string BindedTextChanged { get; set; }

        public string BrushColor { get; set; }

        public PlaceholderTextBox()
        {
            InitializeComponent();
            this.DataContext = this;
            
        }


        
        private void Input_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Purpose: Makes Placeholder Text Visible or Hidden
            if (string.IsNullOrEmpty(Input.Text))
                tbPlaceholder.Visibility = Visibility.Visible;
            else
                tbPlaceholder.Visibility = Visibility.Hidden;

            
        }
    }
}
