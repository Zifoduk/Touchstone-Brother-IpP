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
    /// Interaction logic for Customers.xaml
    /// </summary>
    public partial class Customers : Page
    {
        public Customers()
        {
            InitializeComponent();
        }
        
        private void ButtonSort_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender as Button;
            if (button.Name == "ButtonSortName")
            {
               
            }
            if (button.Name == "ButtonSortPostCode")
            {
            }
            if (button.Name == "ButtonSortDeliveryDate")
            {
            }
        }

        private async void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            var newCustomerList = new List<Customer>();
            await MainWindow.FirebaseManage.RetrieveCustomers(newCustomerList);
            MainWindow.FirebaseManage.PushtoListView(newCustomerList);
        }
    }
}
