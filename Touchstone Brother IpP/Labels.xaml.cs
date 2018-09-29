using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Touchstone_Brother_IpP.Models;

namespace Touchstone_Brother_IpP
{
    /// <summary>
    /// Interaction logic for Labels.xaml
    /// </summary>
    public partial class Labels : Page
    {
        public TLabel SelectedLabel { get; set; }
        public PDFManagment pDFManagment;

        public Labels()
        {
            InitializeComponent();
        }

        public void UIUpdate()
        {
            /*foreach(var item in LabelListView)
            {
                Button button = VisualTreeHelper.GetChild(ButtonAddDatabase, 0) as Button;
                button.Background = Brushes.Red;
            }*/
        }

        private void RMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SelectedLabel = (TLabel)LabelListView.SelectedItem;
            Console.WriteLine("Name: " + SelectedLabel.Name);
        }

        private void ButtonPrintAll_Click(object sender, RoutedEventArgs e)
        {
                
        }

        private void ButtonFindProfile_Click(object sender, RoutedEventArgs e)
        {

        }

        public static DependencyObject FindControlParent(DependencyObject dependency, Type type)
        {
            DependencyObject parent = dependency;
            while((parent = VisualTreeHelper.GetParent(parent)) != null)
            {
                if (parent.GetType() == type)
                {
                    return parent;
                }
            }
            return null;
        }
        private void ButtonPrint_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var ParentItem = FindControlParent(button, typeof(ListViewItem)) as ListViewItem;
            SelectedLabel = LabelListView.ItemContainerGenerator.ItemFromContainer(ParentItem) as TLabel;
            if (SelectedLabel != null)
            {
                var result = PMessageBox.Show("Print for: " + SelectedLabel.Name + "\nCollection Date is: " + SelectedLabel.DeliveryDate, "Print for: " + SelectedLabel.Name, Forms.MessageBoxButtons.YesNo);
                if (result == Forms.DialogResult.Yes)
                {
                    MainWindow.PrintManage.Print(SelectedLabel);
                }
            }
        }

        private void ButtonSort_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender as Button;
            if (button.Name == "ButtonSortName")
            {
                pDFManagment.SourceLabels = pDFManagment.SourceLabels.OrderBy(o => o.Name).ToList();
                pDFManagment.PushToList();
            }
            if (button.Name == "ButtonSortPostCode")
            {
                pDFManagment.SourceLabels = pDFManagment.SourceLabels.OrderBy(o => o.PostCode).ToList();
                pDFManagment.PushToList();
            }
            if (button.Name == "ButtonSortDeliveryDate")
            {
                pDFManagment.SourceLabels = pDFManagment.SourceLabels.OrderBy(o => o.DeliveryDate).ToList();
                pDFManagment.PushToList();
            }
            if (button.Name == "ButtonSortConsignment")
            {
                pDFManagment.SourceLabels = pDFManagment.SourceLabels.OrderBy(o => o.ConsignmentNumber).ToList();
                pDFManagment.PushToList();
            }
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            pDFManagment.Flush();
            pDFManagment.ExtractData();
            pDFManagment.PushToList();
        }

        private void ButtonAddDatabase_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var ParentItem = FindControlParent(button, typeof(ListViewItem)) as ListViewItem;
            SelectedLabel = LabelListView.ItemContainerGenerator.ItemFromContainer(ParentItem) as TLabel;
            var newCustomer = new Customer();
            if (SelectedLabel != null)
            {
                newCustomer.Name = SelectedLabel.Name;
                newCustomer.AllLabels = new List<TLabel>();
                newCustomer.AllLabels.Add(SelectedLabel);
                MainWindow.FirebaseManage.InsertCustomer(newCustomer, MainWindow.customersPage.CustomersList);
            }
        }
    }
}
