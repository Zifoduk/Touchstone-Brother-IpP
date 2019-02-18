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
using Touchstone_Brother_IpP;
using Touchstone_Brother_IpP.Intergrated;
using Touchstone_Brother_IpP.Pages.Browser.Models;

namespace Touchstone_Brother_IpP.Pages.Browser
{
    /// <summary>
    /// Interaction logic for CustomersList.xaml
    /// </summary>
    public partial class ListCustomers : Page
    {
        private ListCustomersModel viewModel;

        public ListCustomers(ListCustomersModel Model)
        {
            InitializeComponent();
            viewModel = Model;
            DataContext = viewModel;
        }

        private void ListViewItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if(item.IsSelected)
            {
                var SelectedCustomer = viewModel.selectedCustomer as Customer;
                Console.WriteLine(SelectedCustomer.Name);

            }
        }
    }
}
    