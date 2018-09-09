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
using Touchstone_Brother_IpP.Models;

namespace Touchstone_Brother_IpP
{
    /// <summary>
    /// Interaction logic for Labels.xaml
    /// </summary>
    public partial class Labels : Page
    {
        public PDFManagment.Label SelectedLabel { get; set; }
        public Labels()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            SelectedLabel = (PDFManagment.Label)TestBinding.SelectedItem;
            var namee = SelectedLabel.Name;
            var idd = SelectedLabel.ID;
            Console.WriteLine("Name: " + SelectedLabel.Name + ", ID: " + SelectedLabel.ID);
        }
    }
}
