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
using Touchstone_Brother_IpP.Models;

namespace Touchstone_Brother_IpP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Settings settingsPage = new Settings();
        public static Labels labelsPage = new Labels();
        public static PDFManagment PdfManage = new PDFManagment();

        public MainWindow()
        {
            InitializeComponent();
            PdfManage.Flush();
            Thread ReadDataThread = new Thread(new ThreadStart(PdfManage.ReadData));
            ReadDataThread.Start();

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

        public void SettingsPage()
        {
            MainView.Content = settingsPage;
        }
        public void LabelsPage()
        {
            MainView.Content = labelsPage;
            
        }
        public void CustomersPage()
        {

        }
        public void PrintPage()
        {

        }
    }
}
