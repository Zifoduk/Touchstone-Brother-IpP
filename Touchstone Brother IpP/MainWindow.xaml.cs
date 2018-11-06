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
using Touchstone_Brother_IpP.Intergrated;
using Nito;
using Nito.AsyncEx;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Diagnostics;

namespace Touchstone_Brother_IpP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static LocalFilesManagement PdfManage = App.LocalFilesManagement;
        public static PrintManagement PrintManage = App.PrintManagement;
        public static FirebaseManagement FirebaseManage = App.FirebaseManagement;

        public static Home homePage = new Home();
        public static Labels labelsPage = new Labels();
        public static Customers customersPage = new Customers();
        public static MainWindowViewModel ViewModel { get; set; } = new MainWindowViewModel();

        public static bool DDebug;

        public bool tester = false;

        public static bool startup;

        public MainWindow()
        {
            InitializeComponent();

            PdfManage = App.LocalFilesManagement;
            PrintManage = App.PrintManagement;
            FirebaseManage = App.FirebaseManagement;

            FirebaseManage._MainWindow = this;
            MainView.DataContext = this;
            //DebuggingMode = true;
        }
        public void SyncViewModel(MainWindowViewModel viewModel)
        {
            viewModel.isSynced = true;
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            item.IsSelected = true;
            if (item != null && item.IsSelected)
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
        public void LabelsPage()
        {
            MainView.Content = labelsPage;
            Console.WriteLine("Breakline");
            PdfManage.PushToList();
        }
        public void CustomersPage()
        {
            MainView.Content = customersPage;
            customersPage.RetrieveCustomers();
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            startup = true;
            customersPage.RetrieveCustomers();
            MainView.Content = homePage;
            labelsPage.pDFManagment = PdfManage;
            startup = false;
        }

        private void RightSideViewOpenButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Button obtn = (Button)this.FindName("RightSideViewCloseButton") as Button;
            Storyboard myStoryboard = btn.TryFindResource("RightSideViewOpen") as Storyboard;
            myStoryboard.Begin(btn);
            btn.Visibility = Visibility.Collapsed;
            obtn.Visibility = Visibility.Visible;
        }

        private void RightSideViewCloseButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Button obtn = (Button)this.FindName("RightSideViewOpenButton") as Button;
            Storyboard myStoryboard = btn.TryFindResource("RightSideViewClose") as Storyboard;
            myStoryboard.Begin(btn);
            btn.Visibility = Visibility.Collapsed;
            obtn.Visibility = Visibility.Visible;
        }



        //Debug Region
        #region Debug Region

        private void SettingsEnableDebugListViewItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (SettingsEnableDebugCheckbox.IsChecked != null)
                if (SettingsEnableDebugCheckbox.IsChecked == true)
                {
                    SettingsEnableDebugCheckbox.IsChecked = false;
                    ViewModel.Debug = false;
                    DDebug = false;
                }
                else if (SettingsEnableDebugCheckbox.IsChecked == false)
                {
                    SettingsEnableDebugCheckbox.IsChecked = true;
                    ViewModel.Debug = true;
                    DDebug = true;
                }
        }

        public void d_Add_StevePage()
        {
            Customer newcustomer = new Customer
            {
                Name = "steve",
                AllLabels = new List<TLabel> { new TLabel
            {
            Name = "steve",
            Address = "cxds",
            Barcode = "456412313",
            DeliveryDate = "qweqwqe",
            ConsignmentNumber = "987561",
            PostCode = "SLLSA12",
            Telephone = "31288312929",
            Location =  "that place",
            LocationNumber = "8",
            ParcelNumber = "001",
            ParcelSize = "s",
            Weight = "not enough"
            } }
            };
            FirebaseManage.InsertCustomer(newcustomer, customersPage.CustomersList);
        }

        private int intGenerator(int length)
        {
            var random = new Random();
            var tempInt = random.Next(1, 100000).ToString("D" + length).ToCharArray();
            var FinalInt = new String(tempInt);
            return int.Parse(FinalInt);
        }
        private string stringGenerator(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }
        public void d_Add_Steve_LabelPage()
        {

            TLabel L = new TLabel
            {
                Name = "steve",
                Address = stringGenerator(25),
                Barcode = intGenerator(20).ToString(),
                DeliveryDate = intGenerator(8).ToString(),
                ConsignmentNumber = intGenerator(20).ToString(),
                PostCode = stringGenerator(7),
                Telephone = intGenerator(11).ToString(),
                Location = stringGenerator(5),
                LocationNumber = intGenerator(2).ToString(),
                ParcelNumber = "001",
                ParcelSize = "s",
                Weight = "not enough",
                Key = "-LiZoxIyWO0KTnahV4iy"
            };
            FirebaseManage.InsertLabel(L);
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //you gonna wanna change this to have a shutdown button
            App.AppClose();
        }
    }



    public abstract class DebugViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
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
