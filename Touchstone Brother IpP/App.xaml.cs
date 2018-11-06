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
using Touchstone_Brother_IpP.Properties;

namespace Touchstone_Brother_IpP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static FirebaseManagement FirebaseManagement = new FirebaseManagement();
        public static LocalFilesManagement LocalFilesManagement = new LocalFilesManagement();
        public static BarcodeManagement BarcodeManagement = new BarcodeManagement();
        public static PrintManagement PrintManagement = new PrintManagement();
        public static Startup startup;
        public static MainWindow mainWindow;

        public static void PostLogin()
        {
            SaveData();
            MainWindow mw = new MainWindow();
            mainWindow = mw;
            mainWindow.Show();
            startup.Close();
        }
        
        public static void SaveData()
        {
            Settings.Default.Save();
        }

        public static void AppClose()
        {
            Environment.Exit(0);
        }
    }
}
