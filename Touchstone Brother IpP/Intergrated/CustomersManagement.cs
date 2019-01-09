using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;
using System.Windows.Media;
using Touchstone_Brother_IpP;
using Touchstone_Brother_IpP.Models;
using Touchstone_Brother_IpP.Intergrated;
using System.Windows.Media.Imaging;

namespace Touchstone_Brother_IpP.Intergrated
{
    public class CustomersManagement
    {
        Core AppCore { get { return App.Core; } }
        private bool IsOnline { get { return App.OfflineManagement.IsOnline; } }

        List<List<TLabel>> l;
        TLabel SelectedLabel = null;
        private List<Customer> CustomersList = new List<Customer>();
        public Customer CurrentCustomer = null;

        public CustomersManagement()
        {

        }
        
        public async Task<List<Customer>> RetrieveCustomersList()
        {
            try
            {
                if (IsOnline)
                { 
                    CurrentCustomer = null;
                    var newCustomerList = new List<Customer>();
                    await MainWindow.FirebaseManage.FetchCustomers(newCustomerList, false);
                    CustomersList = newCustomerList;
                    AppCore.CurrentCustomerList = CustomersList;
                    App.OfflineManagement.OfflineExport(OfflineConfig.CustomerList);
                    return newCustomerList;
                }
                else
                {
                    CurrentCustomer = null;
                    var newCustomerList = new List<Customer>();
                    newCustomerList = App.OfflineManagement.OfflineImport<List<Customer>>(OfflineConfig.CustomerList);
                    CustomersList = newCustomerList;
                    AppCore.CurrentCustomerList = CustomersList;
                    return newCustomerList;
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

        }

        public class CustomerViewInformation
        {
            public List<TLabel> RetrievedLabels { get; set; }
            public BitmapImage RetrievedQRImage { get; set; }
        }
        public async Task<CustomerViewInformation> RetrieveCustomerView(Customer CustomerInformation)
        {
            try
            {
                CustomerViewInformation customerViewInformation = new CustomerViewInformation();
                if (IsOnline)
                {
                    l = await App.FirebaseManagement.FetchAllLabels();
                    customerViewInformation.RetrievedLabels = await App.FirebaseManagement.FetchCustomerLabels(CustomerInformation.Key);
                    App.OfflineManagement.OfflineExport(OfflineConfig.CustomerLabels, labels: customerViewInformation.RetrievedLabels, key: CustomerInformation.Key);
                }
                else
                {
                    customerViewInformation.RetrievedLabels = App.OfflineManagement.OfflineImport<List<TLabel>>(OfflineConfig.CustomerLabels, CustomerInformation.Key);
                }
                customerViewInformation.RetrievedQRImage = App.BarcodeManagement.GenerateDisplayQR(CustomerInformation.Key);
                return customerViewInformation;
            }
            catch (Exception e)
            {
                Console.Write(e);
                return null;
            }

        }
        
    }

    public class Customer
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public List<TLabel> AllLabels { get; set; }
    }
    public class TLabel
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Barcode { get; set; }
        public string DeliveryDate { get; set; }
        public string ConsignmentNumber { get; set; }
        public string PostCode { get; set; }
        public string Telephone { get; set; }
        public string Location { get; set; }
        public string LocationNumber { get; set; }
        public string ParcelNumber { get; set; }
        public string ParcelSize { get; set; }
        public string Weight { get; set; }
    }
}
