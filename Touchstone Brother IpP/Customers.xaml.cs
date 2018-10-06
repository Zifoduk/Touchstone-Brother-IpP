using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Forms = System.Windows.Forms;
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
        TLabel SelectedLabel = null;
        private readonly TLabel DefaultLabel = new TLabel {
            Name = "Customer Name",
            Address = "Customer Address",
            Barcode = "1234567890",
            DeliveryDate = "--/--/----",
            ConsignmentNumber = "0987654321",
            PostCode = "AB12 3CD",
            Telephone = "+-- 123456789123",
            Location = "Customer Region",
            LocationNumber = "--",
            ParcelNumber = "0--",
            ParcelSize = "-",
            Weight = "char(25)"
        };
        public List<Customer> CustomersList = new List<Customer>();
        public Customers()
        {
            InitializeComponent();
            PushToCustomerView(DefaultLabel);
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

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            RetrieveCustomers();
        }

        public async void RetrieveCustomers()
        {
            if (MainWindow.startup)
            {
                var newCustomerList = new List<Customer>();
                await MainWindow.FirebaseManage.RetrieveCustomers(newCustomerList, true);
                CustomersList = newCustomerList;
                PushtoListView(CustomersList);
            }
            else
            {
                var newCustomerList = new List<Customer>();
                await MainWindow.FirebaseManage.RetrieveCustomers(newCustomerList, false);
                foreach(var cust in newCustomerList)
                {
                    cust.AllLabels = cust.AllLabels.OrderBy(i => i.DeliveryDate).ThenBy(i => i.ConsignmentNumber).ToList();
                }
                CustomersList = newCustomerList;
                PushtoListView(CustomersList);
            }
            
        }

        public void PushtoListView (List<Customer> customerList)
        {
            CustomerListView.ItemsSource = customerList;
        }

        private void CustomerListView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            item.IsSelected = true;
            if(item != null && item.IsSelected)
            {
                var CustomerInformation = CustomerListView.ItemContainerGenerator.ItemFromContainer(item) as Customer;
                ViewLabelsList.ItemsSource = CustomerInformation.AllLabels;
                SelectedLabel = DefaultLabel;
                PushToCustomerView(DefaultLabel);
            }
        }

        private static DependencyObject FindControlParent(DependencyObject dependency, Type type)
        {
            DependencyObject parent = dependency;
            while ((parent = VisualTreeHelper.GetParent(parent)) != null)
            {
                if (parent.GetType() == type)
                {
                    return parent;
                }
            }
            return null;
        }
        private void ViewLabelButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var parentItem = FindControlParent(button, typeof(ListViewItem)) as ListViewItem;
            SelectedLabel = ViewLabelsList.ItemContainerGenerator.ItemFromContainer(parentItem) as TLabel;
            if (SelectedLabel != null)
            {
                PushToCustomerView(SelectedLabel);
            }
        }

        public void PushToCustomerView(TLabel tLabel)
        {
            ViewName.Text = tLabel.Name;
            ViewAddress.Text = tLabel.Address;
            ViewBarcode.Text = tLabel.Barcode;
            ViewDeliveryDate.Text = tLabel.DeliveryDate;
            ViewConsignmentNumber.Text = tLabel.ConsignmentNumber;
            ViewPostcode.Text = tLabel.PostCode;
            ViewTelephone.Text = tLabel.Telephone;
            ViewLocation.Text = tLabel.Location;
            ViewLocationNumber.Text = tLabel.LocationNumber;
            ViewParcelNumber.Text = tLabel.ParcelNumber;
            ViewParcelSize.Text = tLabel.ParcelSize;
            ViewWeight.Text = tLabel.Weight;
        }

        private void ViewPrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedLabel != null && SelectedLabel != DefaultLabel)
            {
                var result = PMessageBox.Show("Print for: " + SelectedLabel.Name + "\nCollection Date is: " + SelectedLabel.DeliveryDate, "Print for: " + SelectedLabel.Name, Forms.MessageBoxButtons.YesNo);
                if (result == Forms.DialogResult.Yes)
                {
                    MainWindow.PrintManage.Print(SelectedLabel);
                }
            }
        }
    }
}
