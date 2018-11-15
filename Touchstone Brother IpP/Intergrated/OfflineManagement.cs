using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json;
using Brushes = System.Windows.Media.Brushes;

namespace Touchstone_Brother_IpP.Intergrated
{
    public class OfflineManagement
    {
        public bool IsOnline;
        public MainWindow _MainWindow;
        FirebaseManagement FirebaseManagement
        {
            get { return App.FirebaseManagement; }
        }
        
        public OfflineManagement()
        {
            var CheckConnectionThread = new Thread(CheckForInternetConnection);
            CheckConnectionThread.Start();
        }


        #region OnlineConnection

        bool previousIsOnline = false;
        public void CheckForInternetConnection()
        {
            while (true)
            {
                Console.WriteLine("Check internet " + DateTime.Now.ToLongTimeString().ToString());
                try
                {
                    using (var client = new WebClient())
                    using (client.OpenRead("http://www.microsoft.com"))
                    {
                        if (!previousIsOnline)
                        {
                            IsOnline = true;
                            var result = OfflineExport(OfflineConfig.CustomerList);
                            if (result) { }
                            //Finish
                            else { }
                            //Finish
                        }
                    }

                    /*Uri Url = new Uri("http://www.microsoft.com");

                    WebRequest WebReq;
                    WebResponse Resp;
                    WebReq = WebRequest.Create(Url);

                    try
                    {
                        Resp = WebReq.GetResponse();
                        Resp.Close();
                        WebReq = null;
                        if (!previousIsOnline)
                        {
                            IsOnline = true;
                            var result = OfflineExport(OfflineConfig.CustomerList);
                        }
                    }

                    catch
                    {
                        WebReq = null;
                        IsOnline = false;
                        if (previousIsOnline)
                        {
                            IsOnline = false;
                        }
                    }*/
                }
                catch(Exception)
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
                Console.WriteLine(IsOnline);
                Console.WriteLine("Check internet End " + DateTime.Now.ToLongTimeString().ToString());
                Thread.Sleep(500);
            }
        }
        #endregion

        #region Json Methods
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

        #endregion

        #region Offline Handling

        public bool OfflineExport(OfflineConfig exportConfig, Customer customer = null, List<TLabel> labels = null, Bitmap qrImage = null, string key = null)
        {
            try
            {
                object RetrievedExport = null;
                object Export = null;
                switch (exportConfig)
                {
                    case OfflineConfig.CustomerList:
                        var RetrievedList = App.CustomersManagement.CustomersList;
                        if (RetrievedList.Count == 0)
                            throw new NullReferenceException();
                        RetrievedExport = (List<Customer>)RetrievedList;
                        Export = JsonExport(RetrievedExport);
                        Console.WriteLine(RetrievedExport);
                        Console.WriteLine(Export);
                        App.LocalFilesManagement.Save((string)Export, null, SaveConfig.CustomerList);
                        break;
                    case OfflineConfig.CustomerLabels:
                        Export = JsonExport(labels);
                        Console.WriteLine(Export);
                        App.LocalFilesManagement.Save((string)Export, key, SaveConfig.CustomerLabels);
                        break;
                    case OfflineConfig.CustomerInformation:
                        Export = JsonExport(customer);
                        Console.WriteLine(Export);
                        App.LocalFilesManagement.Save((string)Export, key, SaveConfig.CustomerInfomation);
                        break;
                    case OfflineConfig.CustomerQR:
                        Export = JsonExport(qrImage);
                        Console.WriteLine(Export);
                        App.LocalFilesManagement.Save((Bitmap)Export, key, SaveConfig.QRCode);
                        break;
                }
                return true;
            }
            catch (Exception e)
            {
                //Finish
                Console.WriteLine(e);
                return false;
            }
        }
        public T OfflineImport<T>(OfflineConfig importConfig, string key = null)
        {
            try
            {
                object Import = null;
                switch (importConfig)
                {
                    case OfflineConfig.CustomerList:
                        Import = App.LocalFilesManagement.LoadCustomerList();
                        return (T) Convert.ChangeType(Import, typeof(T));
                    case OfflineConfig.CustomerLabels:
                        Import = App.LocalFilesManagement.LoadCustomerLabels(key);
                        return (T)Convert.ChangeType(Import, typeof(T));
                    case OfflineConfig.CustomerInformation:
                        break;
                    case OfflineConfig.CustomerQR:
                        break;
                }
                return default(T);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return default(T);
            }


        }


        #endregion




        //finish
        private List<Customer> OfflineCustomers()
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
    public enum OfflineConfig
    {
        CustomerList,
        CustomerLabels,
        CustomerInformation,
        CustomerQR
    }
}