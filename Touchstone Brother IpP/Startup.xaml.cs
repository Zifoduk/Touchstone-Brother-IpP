using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Touchstone_Brother_IpP;
using Touchstone_Brother_IpP.Intergrated;

namespace Touchstone_Brother_IpP
{
    /// <summary>
    /// Interaction logic for Startup.xaml
    /// </summary>
    public partial class Startup : Window
    {
        private LogState StateLog = LogState.Main;
        private Storyboard sb;

        public Startup()
        {

            InitializeComponent();
            App.startup = this;
        }

        private async void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            switch (StateLog)
            {
                case LogState.Main:
                    {

                        sb = FindResource("SignUpExpand") as Storyboard;
                        sb.Completed += (s, eventargs) => { EmailBox.Visibility = Visibility.Visible; PasswordBox.Visibility = Visibility.Visible; };
                        sb.Begin();
                        StateLog = LogState.Login;
                        break;
                    }
                case LogState.Login:
                    {
                        var Email = EmailBox.Text;
                        var Pass = PasswordBox.Password;
                        var Passed = await Task.Run(() => App.FirebaseManagement.GetAuthorisation(Email, Pass));
                        if (Passed == 1)
                            App.PostLogin();
                        else if (Passed == 2)
                        {
                            LoginResultText.Text = "Bad Credentials. Check Email and Password.";
                            Console.WriteLine("Failed");
                        }
                        else if (Passed == 3)
                        {
                            LoginResultText.Text = "Unable to connect. Check connection.";
                            Console.WriteLine("Failed");
                        }
                        Console.WriteLine("LoginNext");
                        break;
                    }
            }

            ButtonRegister.IsEnabled = false;
            ButtonRegister.Visibility = Visibility.Collapsed;
            ButtonLogin.Content = "Continue";
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private enum LogState
        {
            Main,
            Login,
            Register
        }

        private void EmailBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (EmailBox.Text == "Email")
                EmailBox.Text = "";
        }

        private void EmailBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (EmailBox.Text == "")
                EmailBox.Text = "Email";
        }

        private void PasswordBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            PasswordBox.Password = "";
        }

    }
}
