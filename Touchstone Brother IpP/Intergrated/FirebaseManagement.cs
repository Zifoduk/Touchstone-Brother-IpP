using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using Firebase.Auth;
using System.Windows.Media;

namespace Touchstone_Brother_IpP.Intergrated
{
    public class FirebaseManagement
    {
        //change so authtokenasyncfactory has to be manually typed in at settings
        FirebaseClient firebase;
        public bool IsOnline;
        public MainWindow _MainWindow;

        public FirebaseManagement()
        {
            
            //var CheckConnectionThread = new Thread(CheckForInternetConnection);
            //CheckConnectionThread.Start();
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
                Thread.Sleep(100);
            }
        }

        public void SaveDataOffline()
        {
            ///Save Retreieved Data Offline
        }

        #region Connect to Firebase

        public async Task<int> GetAuthorisation(string email, string password)
        {
            var token = "";
            try
            {
                var authprovider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyAc99sNicQUibOoDsoqr1Cl56_UPIRYmU4"));
                try
                {
                    var auth = await authprovider.SignInWithEmailAndPasswordAsync(email, password);
                    Console.WriteLine(auth);
                    token = auth.FirebaseToken;
                    try
                    {
                        firebase = new FirebaseClient("https://touchstoneipp.firebaseio.com/", new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token) });
                        Console.WriteLine("Connected");
                        return 1;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("connection error");
                        return 3;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Wrong Details");
                    //App.AccountManagement.FailedSignIn(AccountManagement.FailedReason.Credentials);
                    return 2;
                }

            }
            catch (Exception)
            {
                Console.WriteLine("connection error");
                //App.AccountManagement.FailedSignIn(AccountManagement.FailedReason.Connection);
                return 3;
            }
        }

        #endregion

        #region Database Manipulation

        public async Task RetrieveCustomers(List<Customer> customersList, bool test)
        {
            if (IsOnline)
            {
                if (test)
                {
                    var customers = await firebase.Child("Customers/").OrderByKey().OnceAsync<Customer>();
                    Console.WriteLine(customers);
                    return;
                }
                else
                {
                    var customers = await firebase.Child("Customers/").OrderByKey().OnceAsync<Customer>();
                    customersList.Clear();
                    foreach (var c in customers)
                    {
                        customersList.Add(c.Object);
                    }
                    return;
                }
            }
            else
            {
                /////Insert code for offline reading
            }
        }

        public async void InsertCustomer(Customer customer, List<Customer> customersList)
        {
            if(customersList.Exists(c => c.Name == customer.Name))
            {
                foreach (var l in customer.AllLabels)
                    InsertLabel(l, customersList);
                MainWindow.customersPage.RetrieveCustomers();
            }
            else
            {
                await firebase.Child("Customers/").Child(customer.Name).PutAsync(customer);
                MainWindow.customersPage.RetrieveCustomers();
            }
            Thread.Sleep(1000);
            return;
        }

        public async void InsertLabel(TLabel inLabel, List<Customer> customersList)
        {
            int index = customersList.FindIndex(c => c.Name == inLabel.Name);
            int indexLabel = customersList[index].AllLabels.Count;
            if (index < 0)
                return;
            else
            {
                var name = customersList[index].Name;
                await firebase.Child("Customers/").Child(inLabel.Name).Child("AllLabels").Child(indexLabel.ToString()).PutAsync(inLabel);
            }
        }

        #endregion 
    }

    public class TLabel
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Barcode { get; set; }
        public string DeliveryDate { get; set; }
        public string ConsignmentNumber { get; set; }
        public string PostCode { get; set; }
        public string Telephone { get; set; }
        public string Location { get; set; }
        public string LocationNumber { get; set; }
        public string ParcelNumber { get; set; }
        public string ParcelSize { get; set; }
        public string Weight { get; set; }
    }

    public class Customer
    {
        public string Name { get; set; }
        public List<TLabel> AllLabels { get; set; }
    }
}