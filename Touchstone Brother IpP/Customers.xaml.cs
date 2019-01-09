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
using Touchstone_Brother_IpP.Intergrated;
using Touchstone_Brother_IpP.Models;
using Nito.AsyncEx;

namespace Touchstone_Brother_IpP
{
    /// <summary>
    /// Interaction logic for Customers.xaml
    /// </summary>
    public partial class Customers : Page
    {
        private CustomerPageViewModel _viewModel => App.CustomerPageViewModel;

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
        public Customer CurrentCustomer = null;

        public Customers()
        {
            InitializeComponent();
            DataContext = _viewModel;
            _viewModel.GenerateLabelView(DefaultLabel);
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
            _viewModel.GenerateCustomerList();
        }
        
        private void CustomerListView_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            item.IsSelected = true;
            if(item != null && item.IsSelected)
            {
                var CustomerInformation = CustomerListView.ItemContainerGenerator.ItemFromContainer(item) as Customer;

                CurrentCustomer = CustomerInformation;
                AsyncContext.Run(() => _viewModel.GenerateCustomerView(CustomerInformation));
                _viewModel.ResetLabelView();
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
                _viewModel.GenerateLabelView(SelectedLabel);
            }
        }

        private void ViewPrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedLabel != null && SelectedLabel != DefaultLabel)
            {
                var result = (Forms.DialogResult)PMessageBox.Show("Print for: " + SelectedLabel.Name + "\nCollection Date is: " + SelectedLabel.DeliveryDate, "Print for: " + SelectedLabel.Name, Forms.MessageBoxButtons.YesNo);
                if (result == Forms.DialogResult.Yes)
                {
                    MainWindow.PrintManage.Print(SelectedLabel);
                }
            }
        }

        private void DeleteCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var ParentItem = FindControlParent(button, typeof(ListViewItem)) as ListViewItem;
            var SelectedCustomer = CustomerListView.ItemContainerGenerator.ItemFromContainer(ParentItem) as Customer;
            if (SelectedCustomer != null)
            {
                var result = (Forms.DialogResult)PMessageBox.Show("Delete Customer: " + SelectedCustomer.Name, "Delete Customer: " + SelectedLabel.Name, Forms.MessageBoxButtons.YesNo);
                if (result == Forms.DialogResult.Yes)
                {
                    MainWindow.FirebaseManage.DeleteCustomer(SelectedCustomer ,CustomersList);
                }
            }
        }

        private void ViewDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedLabel != null && SelectedLabel != DefaultLabel)
            {
                var result = (Forms.DialogResult)PMessageBox.Show("Delete for: " + SelectedLabel.Name + "\nCollection Date is: " + SelectedLabel.DeliveryDate, "Print for: " + SelectedLabel.Name, Forms.MessageBoxButtons.YesNo);
                if (result == Forms.DialogResult.Yes)
                {
                    MainWindow.FirebaseManage.DeleteLabel(SelectedLabel, CustomersList);
                }
            }
        }

        private void ViewQRButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewQRCode.Source != null && CurrentCustomer != null)
            {
                _viewModel.PrintQR(CurrentCustomer);
            }
        }
    }
}
