using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Touchstone_Brother_IpP.Intergrated;
using static Touchstone_Brother_IpP.Intergrated.CustomersManagement;

namespace Touchstone_Brother_IpP.Models
{

    public class CustomerPageViewModel : INotifyPropertyChanged
    {
        public static PrintManagement PrintManage
        {
            get { return App.PrintManagement; }
        }

        private List<TLabel> _viewLabelListSource;
        public List<TLabel> ViewLabelListSource
        {
            get { return _viewLabelListSource; }
            set
            {
                _viewLabelListSource = value;
                OnPropertyChanged(nameof(ViewLabelListSource));
            }
        }

        private List<Customer> _customerListViewSource;
        public List<Customer> CustomerListViewSource
        {
            get { return _customerListViewSource; }
            set
            {
                _customerListViewSource = value;
                OnPropertyChanged(nameof(CustomerListViewSource));
            }
        }

        private readonly TLabel DefaultLabel = new TLabel
        {
            Name = "Customer Name",
            Address = "Customer Address",
            Barcode = "----------------",
            DeliveryDate = "--/--/----",
            ConsignmentNumber = "--------------",
            PostCode = "Customer Postcode",
            Telephone = "Customer Telephome",
            Location = "Customer Region",
            LocationNumber = "--",
            ParcelNumber = "0--",
            ParcelSize = "-",
            Weight = "char(25)"
        };
        private TLabel _viewSelectedLabelSource;
        public TLabel ViewSelectedLabelSource
        {
            get { return _viewSelectedLabelSource; }
            set
            {
                _viewSelectedLabelSource = value;
                OnPropertyChanged(nameof(ViewSelectedLabelSource));
            }
        }

        private BitmapImage _viewQRCodeSource;
        public BitmapImage ViewQRCodeSource
        {
            get { return _viewQRCodeSource; }
            set
            {
                _viewQRCodeSource = value;
                OnPropertyChanged(nameof(ViewQRCodeSource));
            }
        }


        public async void GenerateCustomerList()
        {
            List<Customer> Fetched = await App.CustomersManagement.RetrieveCustomersList();
            CustomerListViewSource = Fetched;
        }

        public async void GenerateCustomerView(Customer SelectedCustomer)
        {
            CustomerViewInformation Fetched = await App.CustomersManagement.RetrieveCustomerView(SelectedCustomer);
            ViewQRCodeSource = Fetched.RetrievedQRImage;
            ViewLabelListSource = Fetched.RetrievedLabels;
        }

        public async void PrintQR(Customer SelectedCustomer)
        {
            CustomerViewInformation Fetched = await App.CustomersManagement.RetrieveCustomerView(SelectedCustomer);
            var BitmapQR = Fetched.RetrievedQRImage;
            PrintManage.Print(BitmapQR);
        }

        public void ResetLabelView()
        {
            GenerateLabelView(DefaultLabel);
        }
        public void GenerateLabelView(TLabel SelectedLabel)
        {
            ViewSelectedLabelSource = SelectedLabel;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        [Conditional("DEBUG")]
        private void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                throw new ArgumentNullException(GetType().Name + " does not contain property: " + propertyName);
        }
    }
}
