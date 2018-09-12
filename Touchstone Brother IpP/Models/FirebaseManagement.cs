using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Database.Streaming;

namespace Touchstone_Brother_IpP.Models
{
    public class FirebaseManagement
    {
        //change so authtokenasyncfactory has to be manually typed in at settings
        FirebaseClient firebase;
        private List<Customer> customers;

        public void Initialize()
        {
            firebase = new FirebaseClient("https://touchstoneipp.firebaseio.com/", new FirebaseOptions { AuthTokenAsyncFactory = () => Task.FromResult("IXN3HqrEuAbzokJmi5b61ERW5jynEDBiqXtuRKj7") });
        }

        public async Task RetrieveCustomers(List<Customer> customersList)
        {
            var customers = await firebase.Child("Customers/").OrderByKey().OnceAsync<Customer>();
            customersList.Clear();
            foreach (var c in customers)
            {            
                customersList.Add(c.Object);
            }
            return;
        }

        public async void InsertCustomer(Customer customer)
        {
            await firebase.Child("Customers/").Child(customer.Name).PutAsync(customer);
            Thread.Sleep(1000);
            return;
        }

        public async void InsertLabel(TLabel inLabel, List<Customer> customersList)
        {
            var index = customersList.FindIndex(c => c.Name == inLabel.Name);
            if (index < 0)
                return;
            else
            {
                var name = customersList[index].Name;
                await firebase.Child("Customers/").Child(inLabel.Name).PostAsync(inLabel);
            }
        }

        public void PushtoListView(List<Customer> customerList)
        {
            MainWindow.customers.CustomerListView.ItemsSource = customerList;
        }
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