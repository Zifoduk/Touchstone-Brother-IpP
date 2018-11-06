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
                catch (Exception)
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
                        if(c.Object.AllLabels.Any(i => i == null))
                        {
                            c.Object.AllLabels.RemoveAll(item => item == null);
                            await firebase.Child("Customers").Child(c.Object.Name).PutAsync(c.Object);
                            
                        }
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

        public async Task<List<TLabel>> RetrieveCustomerLabel(string customersKey)
        {
            List<TLabel> RetrievedList = new List<TLabel>();
            try
            {
                if (IsOnline)
                {
                    var Labels = await firebase.Child("Labels").Child(customersKey).OrderByKey().OnceAsync<TLabel>();
                    foreach (var l in Labels)
                        RetrievedList.Add(l.Object);
                }
                else
                {
                    /////Insert code for offline reading
                }
                return RetrievedList;
            }
            catch (Exception e)
            {
                Console.Write(e);
                var t = new TLabel
                {
                    Name = "error",
                    Address = "error",
                    Barcode = "error",
                    DeliveryDate = "error",
                    ConsignmentNumber = "error",
                    PostCode = "error",
                    Telephone = "error",
                    Location = "error",
                    LocationNumber = "error",
                    ParcelNumber = "error",
                    ParcelSize = "error",
                    Weight = "error"
                };
                RetrievedList.Add(t);
                return RetrievedList;
            }
        }

        private string GenerateUID()
        {
            Guid guid = Guid.NewGuid();
            ShortGuid UID = guid;
            Console.WriteLine("ShortGUID GUID: " + UID.Guid);
            Console.WriteLine("ShortGUID Value: " + UID.Value);
            return "-" + UID;
        }
        public async void InsertCustomer(Customer customer, List<Customer> customersList)
        {
            string customerUID = GenerateUID();
            customer.Key = customerUID;
            await firebase.Child("Customers/").Child(customerUID).PutAsync(customer);
            Thread.Sleep(1000);
            MainWindow.customersPage.RetrieveCustomers();
        }

        public async void InsertLabel(TLabel inLabel/*, List<Customer> customersList*/)
        {
            /*int index = customersList.FindIndex(c => c.Name == inLabel.Name);
            int indexLabel = customersList[index].AllLabels.Count;
            if (index < 0)
                return;
            else
            {
                var name = customersList[index].Name;
                await firebase.Child("Customers/").Child(inLabel.Name).Child("AllLabels").Child(indexLabel.ToString()).PutAsync(inLabel);
            }*/

            await firebase.Child("Labels").Child(inLabel.Key).PostAsync(inLabel, true);
        }
        public async void DeleteLabel(TLabel inLabel, List<Customer> customersList)
        {
            try
            {
                int index = customersList.FindIndex(c => c.Name == inLabel.Name);
                int indexLabel = customersList[index].AllLabels.FindIndex(l => l.Barcode == inLabel.Barcode);
                if (index < 0)
                    return;
                else
                {
                    var name = customersList[index].Name;
                    customersList[index].AllLabels.RemoveAt(indexLabel);
                    customersList[index].AllLabels.RemoveAll(item => item == null);
                    await firebase.Child("Customers").Child(name).PutAsync(customersList[index]);
                    MainWindow.customersPage.RetrieveCustomers();
                }
            }
            catch (Exception e)
            {
                var result = PMessageBox.Show("Failed to delete label for " + inLabel.Name + "\n\r Expection: " + e, "Failed to delete label for" + inLabel.Name, System.Windows.Forms.MessageBoxButtons.OK);
                Console.WriteLine(e);
            }
        }

        public async void DeleteCustomer(Customer inCustomer, List<Customer> customersList)
        {
            try
            {
                int index = customersList.FindIndex(c => c.Name == inCustomer.Name);
                if (index < 0)
                    return;
                else
                {
                    customersList.RemoveAt(index);
                    await firebase.Child("Customers").Child(inCustomer.Name).DeleteAsync();
                    MainWindow.customersPage.RetrieveCustomers();
                    
                }
            }
            catch (Exception e)
            {
                var result = PMessageBox.Show("Failed to delete customer " + inCustomer.Name + "\n\r Expection: " + e, "Failed to delete label for" + inCustomer.Name, System.Windows.Forms.MessageBoxButtons.OK);
                Console.WriteLine(e);
            }
        }

        #endregion 
    }

    public class TLabel
    {
        public string Key { get; set; }
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
        public string Key { get; set; }
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