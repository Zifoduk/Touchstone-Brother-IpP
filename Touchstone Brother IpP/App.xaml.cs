using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Touchstone_Brother_IpP.Models;
using Touchstone_Brother_IpP;
using Touchstone_Brother_IpP.Intergrated;

namespace Touchstone_Brother_IpP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static FirebaseManagement FirebaseManagement = new FirebaseManagement();
        public static PDFManagement PDFManagement = new PDFManagement();
        public static PrintManagement PrintManagement = new PrintManagement();
        public static Startup startup;

        public static void PostLogin()
        {
            MainWindow mw = new MainWindow();
            mw.Show();
        }

    }
}
