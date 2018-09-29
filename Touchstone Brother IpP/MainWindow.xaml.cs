﻿using System;
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
using System.Windows.Media.Animation;

namespace Touchstone_Brother_IpP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Home homePage = new Home();
        public static Settings settingsPage = new Settings();
        public static Labels labelsPage = new Labels();
        public static Customers customersPage = new Customers();
        public static PDFManagment PdfManage = new PDFManagment();
        public static PrintManagement PrintManage = new PrintManagement();
        public static FirebaseManagement FirebaseManage = new FirebaseManagement();

        public static bool startup;

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
            /*if(SideView.Content != settingsPage)
            {
                SideView.Content = settingsPage;
                Storyboard sb = this.FindResource("SideViewOpen") as Storyboard;
                sb.Begin();
            }
            else
            {
                SideView.Content = null;
                Storyboard sb = this.FindResource("SideViewClose") as Storyboard;
                sb.Begin();
            }*/
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
        public void LabelsPage()
        {
            MainView.Content = labelsPage;
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
            //Task task = new Task(() => FirebaseManage.InsertCustomer(newcustomer));
            customersPage.RetrieveCustomers();
            MainView.Content = homePage;
            //PdfManage.Initialize();
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
    }
}
