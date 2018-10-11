using System;
using System.Collections.Generic;
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

        public void Sync()
        {
            while(!isSynced)
            {
                
            }
        }
    }
}
