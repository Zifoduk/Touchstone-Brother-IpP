using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Touchstone_Brother_IpP.Models;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Touchstone_Brother_IpP
{
    /// <summary>
    /// Interaction logic for Browser.xaml
    /// </summary>
    public partial class Browser : Window
    {
        public static BrowserViewModel ViewModel { get; set; }
        public Browser(BrowserViewModel browserViewModel)
        {
            InitializeComponent();
            ViewModel = browserViewModel;
            DataContext = ViewModel;
        }

        public void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Quit();
        }

        private void DragMove(object sender, RoutedEventArgs e)
        {
            this.DragMove();
        }
    }


}
