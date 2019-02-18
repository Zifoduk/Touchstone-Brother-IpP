using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Touchstone_Brother_IpP.Intergrated;
using Touchstone_Brother_IpP.Pages.Browser;
using Touchstone_Brother_IpP.Pages.Browser.Models;

namespace Touchstone_Brother_IpP.Models
{

    public class BrowserViewModel : INotifyPropertyChanged, IDisposable
    {
        private static Browser _Browser;

        public object result = null;

        private string _selectedObject;
        public string selectedObject
        {
            get { return _selectedObject; }
            set
            {
                _selectedObject = value;
                OnPropertyChanged(nameof(selectedObject));
            }
        }

        private int _width;
        public int width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged(nameof(width));
            }

        }


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

        
        public BrowserViewModel(BrowserSettings Settings = BrowserSettings.Empty, SelectionMode selectionMode = SelectionMode.Single)
        {
            _Browser = new Browser(this);
            _Browser.Show();
            GenerateFrame(Settings, selectionMode);
        }

        private void GenerateFrame(BrowserSettings settings, SelectionMode selectionMode)
        {
            switch (settings)
            {
                case BrowserSettings.Customers:
                    ViewFrameContext = new ListCustomersModel(this, selectionMode).view;
                    break;
                case BrowserSettings.Files:
                    ViewFrameContext = null;//Filebrowserpage
                    break;
                case BrowserSettings.Empty:
                    //Change ViewFrameContext to an actual page
                    ViewFrameContext = null;
                    break;
                default:
                    break;
            }
        }

        public void Quit()
        {
            _Browser.Close();
            result = null;
        }

        public void Dispose()
        {
            _Browser = null;

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

        public enum BrowserSettings
        {
            Customers,
            Files,
            Empty
        }
    }
}
