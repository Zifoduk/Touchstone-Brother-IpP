using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Touchstone_Brother_IpP.Intergrated;

namespace Touchstone_Brother_IpP.Intergrated
{
    public class Core : INotifyPropertyChanged
    {
        private List<Customer> _CurrentCustomerList;
        public List<Customer> CurrentCustomerList
        {
            get { return _CurrentCustomerList; }
            set
            {
                _CurrentCustomerList = value;
                OnPropertyChanged(nameof(CurrentCustomerList));
            }
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
