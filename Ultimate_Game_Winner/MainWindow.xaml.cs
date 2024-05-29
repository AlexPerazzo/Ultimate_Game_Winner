using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ultimate_Game_Winner;

namespace Ultimate_Game_Winner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        //Give other xaml.cs's access to the mainFrame
        public Frame MainFrame => mainFrame;

        private void mainFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }
    }
}