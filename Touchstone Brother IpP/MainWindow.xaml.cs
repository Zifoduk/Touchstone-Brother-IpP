using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Forms = System.Windows.Forms;
using Touchstone_Brother_IpP.Models;
using Nito;
using Nito.AsyncEx;

namespace Touchstone_Brother_IpP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<Customer> CustomersList = new List<Customer>();

        public static Home homePage = new Home();
        public static Settings settingsPage = new Settings();
        public static Labels labelsPage = new Labels();
        public static Customers customers = new Customers();
        public static PDFManagment PdfManage = new PDFManagment();
        public static PrintManagement PrintManage = new PrintManagement();
        public static FirebaseManagement FirebaseManage = new FirebaseManagement();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            item.IsSelected = true;
            if(item != null && item.IsSelected)
            {
                string itemName = item.Name;
                string methodName = itemName + "Page";
                Type thisType = this.GetType();
                MethodInfo method = null;
                method = thisType.GetMethod(methodName);
                method.Invoke(this, null);
            }
        }
        public void HomePage()
        {
            MainView.Content = homePage;
        }
        public void SettingsPage()
        {
            MainView.Content = settingsPage;
        }
        public void LabelsPage()
        {
            MainView.Content = labelsPage;
            PdfManage.PushToList();
        }
        public void CustomersPage()
        {
            MainView.Content = customers;
            FirebaseManage.PushtoListView(CustomersList);
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //FirebaseManage.Initialize();
            Customer newcustomer = new Customer { Name = "steve", AllLabels = new List<TLabel> { new TLabel
            {
            Name = "steve",
            Address = "cxds",
            Barcode = "898805",
            DeliveryDate = "sunday",
            ConsignmentNumber = "8974605",
            PostCode = "SLLSA12",
            Telephone = "31288312929",
            Location =  "that place",
            LocationNumber = "8",
            ParcelNumber = "001", 
            ParcelSize = "s",
            Weight = "not enough"
            } } };
            //Task task = new Task(() => FirebaseManage.InsertCustomer(newcustomer));
            FirebaseManage.InsertCustomer(newcustomer);
            Thread.Sleep(500);
            await FirebaseManage.RetrieveCustomers(CustomersList);
            MainView.Content = homePage;
            //PdfManage.Initialize();
            labelsPage.pDFManagment = PdfManage;
        }
    }
}
