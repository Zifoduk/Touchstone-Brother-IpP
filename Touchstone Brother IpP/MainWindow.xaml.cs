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

        public static LocalFilesManagement LocalFilesManage
        {
            get { return App.LocalFilesManagement; }
        }
        public static OfflineManagement OfflineManagement
        {
            get { return App.OfflineManagement; }
        }
        public static PrintManagement PrintManage
        {
            get { return App.PrintManagement; }
        }
        public static FirebaseManagement FirebaseManage
        {
            get { return App.FirebaseManagement; }
        }

        public static Home homePage = new Home();
        public static Labels labelsPage = new Labels();
        public static Customers customersPage = new Customers();
        public static MainWindowViewModel ViewModel { get; set; } = new MainWindowViewModel();

        private bool LogoutState = false;
        public bool tester = false;

        public static bool startup;

        public MainWindow()
        {
            InitializeComponent();
            
            OfflineManagement._MainWindow = this;
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
            LocalFilesManage.PushToList();
        }
        public void CustomersPage()
        {
            MainView.Content = customersPage;
            App.CustomerPageViewModel.GenerateCustomerList();
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            startup = true;
            App.CustomerPageViewModel.GenerateCustomerList();
            MainView.Content = homePage;
            labelsPage.pDFManagment = LocalFilesManage;
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
                }
                else if (SettingsEnableDebugCheckbox.IsChecked == false)
                {
                    SettingsEnableDebugCheckbox.IsChecked = true;
                    ViewModel.Debug = true;
                }
        }

        public void d_Add_StevePage()
        {
            Customer newcustomer = new Customer
            {
                Name = "JANE MAKINA",               
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
                Name = "MARIA TANYANYIWA",
                Address = "FLAT 8,\r\nRADLEY COURT,\r\n34 BEACHBOROUGH ROAD,\r\nBROMLEY",
                Barcode = "ABR1 540617530372272001",
                DeliveryDate = "07/09/2018",
                ConsignmentNumber = "40617530372272",
                PostCode = "BR1 5RL",
                Telephone = "+44 7887 420381",
                Location = "DARTFORD",
                LocationNumber = "30",
                ParcelNumber = "001",
                ParcelSize = "L",
                Weight = "up to 15.0 kg"
            };
            FirebaseManage.InsertLabel(L);
        }
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //you gonna wanna change this to have a shutdown button
            if(!LogoutState)
                App.AppClose();
        }

        private void LogoutListViewItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LogoutState = true;
            ViewModel.TriggerLogout();
        }
    }
}
