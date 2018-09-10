using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        private void RMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SelectedLabel = (TLabel)TestBinding.SelectedItem;
            var namee = SelectedLabel.Name;
            var idd = SelectedLabel.ID;
            Console.WriteLine("Name: " + SelectedLabel.Name + ", ID: " + SelectedLabel.ID);
        }

        private void ButtonPrintAll_Click(object sender, RoutedEventArgs e)
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
            SelectedLabel = TestBinding.ItemContainerGenerator.ItemFromContainer(ParentItem) as TLabel;
            if (SelectedLabel != null)
            {
                MessageBoxResult result = MessageBox.Show("print?", "print?", MessageBoxButton.YesNo);
                if(result == MessageBoxResult.Yes)
                {
                    MainWindow.PrintManage.Print(SelectedLabel);
                }
            }

        }

        private void ButtonSort_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender as Button;
            if(button.Name == "ButtonSortID")
            {
                pDFManagment.SourceLabels = pDFManagment.SourceLabels.OrderBy(o => o.ID).ToList();
                pDFManagment.PushToList();
            }
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
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            pDFManagment.PushToList();
        }
    }
}
