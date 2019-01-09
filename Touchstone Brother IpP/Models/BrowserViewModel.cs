using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Touchstone_Brother_IpP.Intergrated;
using static Touchstone_Brother_IpP.Intergrated.CustomersManagement;

namespace Touchstone_Brother_IpP.Models
{

    public class BrowserViewModel : INotifyPropertyChanged
    {

        private object _viewFrameContext;
        public object ViewFrameContext
        {
            get { return _viewFrameContext; }
            set
            {
                _viewFrameContext = value;
                OnPropertyChanged(nameof(ViewFrameContext));
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
