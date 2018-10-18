using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Reactive.Linq;

namespace Touchstone_Brother_IpP.Intergrated
{
    [System.Runtime.InteropServices.Guid("6EE69DE3-0BBC-4AED-86AB-D7C322F76737")]
    public class FirebaseManagement
    {
        //change so authtokenasyncfactory has to be manually typed in at settings
        FirebaseClient firebase;
        public bool IsOnline;
        public MainWindow _MainWindow;

        public FirebaseManagement()
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

        public void SaveDataOffline()
        {
            //start this
        }


        private string previouskey = "";
        public void CustomerSubscribedEvent(FirebaseEvent<Customer> sender)
        {
            //fix this
            string newkey = sender.Key;
            if(newkey != previouskey)
                Console.WriteLine("Update, Name" + sender.Object.Name);
            previouskey = newkey;
        }

        #region Connect to Firebase

        public async Task<int> GetAuthorisation(string email, string password)
        {
            var token = "";
            try
            {
                var auther = new FirebaseAuth();
                var authprovider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyAc99sNicQUibOoDsoqr1Cl56_UPIRYmU4"));
                try
                {
                    var auth = await authprovider.SignInWithEmailAndPasswordAsync(email, password);
                    Console.WriteLine(auth);
                    token = auth.FirebaseToken;

                    if (token != "")
                    {
                        try
                        {

                            firebase = new FirebaseClient("https://touchstoneipp.firebaseio.com/", new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult(token) });
                            Console.WriteLine("Connected");
                            SaveDataOffline();
                            return 1;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            Console.WriteLine("connection error");
                            return 3;
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Wrong Details");
                    //App.AccountManagement.FailedSignIn(AccountManagement.FailedReason.Credentials);
                    return 2;
                }

            }
            catch (FirebaseAuthException)
            {
                Console.WriteLine("firebase auth exception");
                throw;
            }
            catch (Exception)
            {
                Console.WriteLine("connection error");
                //App.AccountManagement.FailedSignIn(AccountManagement.FailedReason.Connection);
                return 3;
            }
        }

        public async Task<int> CreateUser(string email, string password, bool goodPass)
        {
            try
            {
                var auther = new FirebaseAuth();
                var authprovider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyAc99sNicQUibOoDsoqr1Cl56_UPIRYmU4"));
                try
                {
                    if (goodPass)
                    {
                        var auth = await authprovider.CreateUserWithEmailAndPasswordAsync(email, password, email, false);
                        Console.WriteLine("registered");
                        return 1;
                    }
                    else
                        return 2;
                }
                catch (Exception e)
                {                    
                    Console.WriteLine("Failed to register");
                    return 3;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Connection Failed");
                return 4;
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
                    var customers = await firebase.Child("Customers").OrderByKey().OnceAsync<Customer>();
                    Console.WriteLine(customers);
                    return;
                }
                else
                {
                    var customers = await firebase.Child("Customers").OrderByKey().OnceAsync<Customer>();
                    customersList.Clear();
                    foreach (var c in customers)
                    {
                        customersList.Add(c.Object);
                    }
                    try
                    {
                        var observed = firebase.Child("Customers").AsObservable<Customer>(elementRoot: "Customer").Subscribe(e => CustomerSubscribedEvent(e));
                        Console.WriteLine("subscribed");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Unable to subscribe");
                        Console.WriteLine("///////////////////////////");
                        Console.WriteLine(e);
                        Console.WriteLine("///////////////////////////");
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
                //MainWindow.customersPage.RetrieveCustomers();
            }
            else
            {
                await firebase.Child("Customers/").Child(customer.Name).PutAsync(customer);
                //MainWindow.customersPage.RetrieveCustomers();
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

    public class ListenChanges<T> : IObserver<FirebaseEvent<T>>
    {
        public void OnCompleted()
        {
        }
        public void OnError(Exception error)
        {
            //log error
        }
        public void OnNext(FirebaseEvent<T> value)
        {
            //process value
        }
    }
}