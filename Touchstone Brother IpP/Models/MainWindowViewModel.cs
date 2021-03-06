﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Touchstone_Brother_IpP.Models
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public bool isSynced;
        private bool _Debug;
        public bool Debug
        {
            get { return _Debug; }
            set
            {
                _Debug = value;
                OnPropertyChanged(nameof(Debug));
            }
        }

        public MainWindowViewModel()
        {
            Debug = false;
        }

        public void TriggerLogout()
        {
            App.PostLogout();
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
