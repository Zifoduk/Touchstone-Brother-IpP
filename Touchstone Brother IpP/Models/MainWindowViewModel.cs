using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touchstone_Brother_IpP.Models
{
    public class MainWindowViewModel : DebugViewModel
    {
        public bool isSynced;
        private bool _Debug;
        public bool Debug
        {
            get { return _Debug; }
            set
            {
                _Debug = value;
                OnPropertyChanged("Debug");
            }
        }

        public MainWindowViewModel()
        {
            Debug = false;
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
