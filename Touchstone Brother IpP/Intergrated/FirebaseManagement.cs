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
        Core AppCore { get { return App.Core; } }
        FirebaseClient firebase;
        private bool IsOnline
        {
            get { return App.OfflineManagement.IsOnline; }
        }

        public FirebaseManagement()
        {    
            
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
        public async Task FetchCustomers(List<Customer> customersList, bool test)
        {
            try
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
                            customersList.Add(c.Object);
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
            catch(Exception e)
            {
                //Insert code
            }
        }
        public async Task<List<TLabel>> FetchCustomerLabels(string customerKey)
        {
            List<TLabel> RetrievedList = new List<TLabel>();
            try
            {
                if (IsOnline)
                {
                    var Labels = await firebase.Child("Labels").Child(customerKey).OrderByKey().OnceAsync<TLabel>();
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

        class ylabel { List<TLabel> lebel { get; set; } }

        public async Task<List<List<TLabel>>> FetchAllLabels()
        {
            List<List<TLabel>> RetrievedList = new List<List<TLabel>>();
            try
            {
                if (IsOnline)
                {
                    var Labels = await firebase.Child("Labels").OrderByKey().OnceAsync<String>();
                    Console.WriteLine("lol");
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
                return null;
            }
        }

        /*private string FindUID(InputType inputType, Customer customer = null, TLabel label = null)
        {
            switch (inputType)
            {
                case InputType.Customer:
                    break;
                case InputType.Label:
                    List<string> PotentialCustomers = new List<string>();
                    List<string> PotentialLabels = new List<string>();
                    foreach (var cust in AppCore.CurrentCustomerList)
                    {
                        if (cust.Name == label.Name || cust.a)
                            PotentialCustomers.Add(customer.Name);
                    }
                    PotentialCustomers.Add("None");
                    string result = (string)PMessageBox.Show("Select Correct Customer", "Select Customer", null, true, PotentialCustomers);
                    Console.WriteLine(result);
                    return result;
                case InputType.String:
                    break;
                default:
                    break;
            }
            
        }*/
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
            App.CustomerPageViewModel.GenerateCustomerList();
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
                    App.CustomerPageViewModel.GenerateCustomerList();

                }
            }
            catch (Exception e)
            {
                var result = PMessageBox.Show("Failed to delete customer " + inCustomer.Name + "\n\r Expection: " + e, "Failed to delete label for" + inCustomer.Name, System.Windows.Forms.MessageBoxButtons.OK);
                Console.WriteLine(e);
            }
        }

        public async void InsertLabel(TLabel inLabel)
        {
           //FindUID(inLabel.Name);
            //await firebase.Child("Labels").Child(inLabel.Key).PostAsync(inLabel, true);
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
                    App.CustomerPageViewModel.GenerateCustomerList();
                }
            }
            catch (Exception e)
            {
                var result = PMessageBox.Show("Failed to delete label for " + inLabel.Name + "\n\r Expection: " + e, "Failed to delete label for" + inLabel.Name, System.Windows.Forms.MessageBoxButtons.OK);
                Console.WriteLine(e);
            }
        }
        #endregion 
    }


    public enum InputType
    {
        Customer,
        Label,
        String
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