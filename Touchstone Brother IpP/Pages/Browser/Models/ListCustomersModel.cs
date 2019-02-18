using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touchstone_Brother_IpP;
using Touchstone_Brother_IpP.Intergrated;
using Touchstone_Brother_IpP.Models;
using System.Windows.Controls;

namespace Touchstone_Brother_IpP.Pages.Browser.Models
{
    public class ListCustomersModel : INotifyPropertyChanged
    {
        Core AppCore { get { return App.Core; } }

        public ListCustomers view
        {
            get{ return _view; }
            set
            {
                _view = value;
                OnPropertyChanged(nameof(view));
            }
        }
        private ListCustomers _view;

        public SelectionMode selectionMode
        {
            get { return _selectionMode; }
            set
            {
                _selectionMode = value;
                OnPropertyChanged(nameof(selectionMode));
            }
        }
        private SelectionMode _selectionMode;

        public BrowserViewModel browserWindow
        {
            get { return _browserWindow; }
            set
            {
                _browserWindow = value;
                OnPropertyChanged(nameof(browserWindow));
            }
        }
        private BrowserViewModel _browserWindow;

        public List<Customer> contextList
        {
            get { return _contextList; }
            set
            {
                _contextList = value;
                OnPropertyChanged(nameof(contextList));
            }
        }
        private List<Customer> _contextList;

        private Customer _selectedCustomer;
        public Customer selectedCustomer
        {
            get { return _selectedCustomer; }
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged(nameof(selectedCustomer));
            }
        }

        private IList<Customer> _selectedCustomers;
        public IList<Customer> selectedCustomers
        {
            get
            {
                return _selectedCustomers;
            }
            set
            {
                _selectedCustomers = value;
                OnPropertyChanged(nameof(selectedCustomers));
            }
        }

        public ListCustomersModel(BrowserViewModel BrowserWindow, SelectionMode SelectionMode)
        {
            view = new ListCustomers(this);
            selectionMode = SelectionMode;
            contextList = AppCore.CurrentCustomerList;
            browserWindow = BrowserWindow;
            browserWindow.width = 300;       
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
                throw new ArgumentNullException(GetType().Name + "does not contain property: " + propertyName);
        }
        
    }


}
