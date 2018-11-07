using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Touchstone_Brother_IpP.Intergrated
{
    public class OfflineManagement
    {
        public bool IsOnline;
        public MainWindow _MainWindow;

        public OfflineManagement()
        {
            var CheckConnectionThread = new Thread(CheckForInternetConnection);
            CheckConnectionThread.Start();
        }

        bool previousIsOnline = false;
        public void CheckForInternetConnection()
        {
            while (true)
            {
                try
                {
                    using (var client = new WebClient())
                    using (client.OpenRead("http://clients3.google.com/generate_204"))
                    {
                        if (!previousIsOnline)
                        {
                            IsOnline = true;
                        }
                    }
                }
                catch
                {
                    if (previousIsOnline)
                    {
                        IsOnline = false;
                    }
                }

                try
                {
                    if (_MainWindow != null)
                    {
                        if (_MainWindow.IsOnlineEllipse != null)
                            if (IsOnline && !previousIsOnline)
                            {
                                _MainWindow.IsOnlineEllipse.Dispatcher.Invoke(() => (_MainWindow.IsOnlineEllipse.Fill = Brushes.Green));
                                previousIsOnline = true;
                            }
                            else if (!IsOnline && previousIsOnline)
                            {
                                _MainWindow.IsOnlineEllipse.Dispatcher.Invoke(() => (_MainWindow.IsOnlineEllipse.Fill = Brushes.Red));
                                previousIsOnline = false;
                            }
                    }
                }
                catch (Exception)
                {

                    Console.WriteLine("unable to changed mainwindow UI");
                }

                Thread.Sleep(100);
            }
        }

        public string JsonExport(object Data)
        {
            string JsonResponce = JsonConvert.SerializeObject(Data, Data.GetType(), Formatting.Indented, null);
            return JsonResponce;
        }
        public object JsonImport(JsonImportConfig importConfig, string key)
        {
            try
            {
                object ReturnObject = null;
                string loadedFile = null;
                switch (importConfig)
                {
                    case JsonImportConfig.CustomerList:
                        loadedFile = File.ReadAllText(App.LocalFilesManagement.RetrieveCustomerList());
                        ReturnObject = JsonConvert.DeserializeObject<List<Customer>>(loadedFile) as List<Customer>;
                        loadedFile = null;
                        break;
                    case JsonImportConfig.SpecificCustomer:
                        loadedFile = File.ReadAllText(App.LocalFilesManagement.RetrieveCustomerInformation(key));
                        ReturnObject = JsonConvert.DeserializeObject<Customer>(loadedFile) as Customer;
                        loadedFile = null;
                        break;
                    case JsonImportConfig.CustomerLabels:
                        loadedFile = File.ReadAllText(App.LocalFilesManagement.RetrieveCustomerLabels(key));
                        ReturnObject = JsonConvert.DeserializeObject<List<TLabel>>(loadedFile) as List<TLabel>;
                        loadedFile = null;
                        break;

                }
                return ReturnObject;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        //finish
        public List<Customer> OfflineCustomers()
        {
            List<Customer> customerlist = null;

            return customerlist;
        }
    }

    public enum JsonImportConfig
    {
        CustomerList,
        SpecificCustomer,
        CustomerLabels
    }
}