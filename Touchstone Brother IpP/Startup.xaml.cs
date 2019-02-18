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
using Touchstone_Brother_IpP.Properties;
using Touchstone_Brother_IpP.Intergrated;
using Touchstone_Brother_IpP.Models;
using forme = System.Windows.Forms;

namespace Touchstone_Brother_IpP
{
    /// <summary>
    /// Interaction logic for Startup.xaml
    /// </summary>
    public partial class Startup : Window
    {
        private LogState StateLog = LogState.Main;
        private Storyboard sb;
        private bool GoodPass = false;
        private bool started = false;
        private bool SaveEmail = false;
        private bool IsWorking = false;

        public Startup()
        {
            InitializeComponent();
            App.startup = this;            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            started = true;
            SaveEmail = Settings.Default.UsePrevEmail;
            if (SaveEmail)
                EmailBox.Text = Settings.Default.Email;
            SaveEmailCheck.IsChecked = SaveEmail;
            SaveEmailCheck.Click += SaveEmailCheck_Click;


            /*
            Customer newcustomer = new Customer
            {
                Name = "Daniel",
                Key = "lolskey",
                AllLabels = new List<TLabel> { new TLabel
            {
            Name = "Daniel",
            Address = "jusadk",
            Barcode = "45846213",
            DeliveryDate = "7/12/11",
            ConsignmentNumber = "6731529",
            PostCode = "ZZ231YJ",
            Telephone = "071536352728",
            Location =  "this place",
            LocationNumber = "33",
            ParcelNumber = "001",
            ParcelSize = "L",
            Weight = "too heavy",
            Key = "lolskey"
            } }
            };

            App.LocalFilesManagement.SaveCustomerInformation(newcustomer);
            var loadedcustomer = App.OfflineManagement.JsonImport(JsonImportConfig.SpecificCustomer, "lolskey");*/
        }

        private void SaveEmailCheck_Click(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox.IsChecked == true)
                SaveEmail = true;
            else
                SaveEmail = false;
        }

        private async void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            //Testing Browser
            BrowserViewModel broser = new BrowserViewModel(BrowserViewModel.BrowserSettings.Customers, SelectionMode.Extended);
            if (!IsWorking)
            {
                IsWorking = true;
                ButtonLogin.IsEnabled = false;
                switch (StateLog)
                {
                    case LogState.Main:
                        {
                            ButtonLogin.Content = "";
                            sb = FindResource("SignInExpand") as Storyboard;
                            StageText.Text = "";
                            LoginResultText.Text = "";
                            sb.Completed += (s, eventargs) =>
                            {
                                EmailBox.Visibility = Visibility.Visible;
                                PasswordBox.Visibility = Visibility.Visible;
                                BackButton.Visibility = Visibility.Visible;
                                SaveEmailCheck.Visibility = Visibility.Visible;
                                StageText.Text = "Provide your account email and password";
                                ButtonLogin.Content = "Continue";
                                ButtonLogin.IsEnabled = true;
                            };
                            sb.Begin();
                            StateLog = LogState.Login;
                            ButtonRegister.IsEnabled = false;
                            ButtonRegister.Visibility = Visibility.Collapsed;
                            EmailBox.KeyUp += TextBox_KeyUp;
                            PasswordBox.KeyUp += TextBox_KeyUp;
                            IsWorking = false;
                            break;
                        }
                    case LogState.Login:
                        {
                            var Email = EmailBox.Text;
                            var Pass = PasswordBox.Password;
                            var Passed = await Task.Run(() => App.FirebaseManagement.GetAuthorisation(Email, Pass));
                            if (Passed == 1)
                            {
                                if (SaveEmail)
                                {
                                    Settings.Default.Email = Email;
                                    Settings.Default.UsePrevEmail = true;
                                    IsWorking = false;
                                    App.PostLogin();
                                }
                                else
                                {
                                    Settings.Default.Email = "";
                                    Settings.Default.UsePrevEmail = false;
                                    IsWorking = false;
                                    App.PostLogin();
                                }
                            }
                            else if (Passed == 2)
                            {
                                LoginResultText.Foreground = Brushes.Red;
                                LoginResultText.Text = "Bad Credentials. Check Email and Password.";
                                Console.WriteLine("Failed");
                                IsWorking = false;
                            }
                            else if (Passed == 3)
                            {
                                LoginResultText.Foreground = Brushes.Orange;
                                LoginResultText.Text = "Unable to connect. Check connection.";
                                Console.WriteLine("Failed");
                                IsWorking = false;
                            }
                            Console.WriteLine("LoginNext");
                            IsWorking = false;
                            break;
                        }
                    case LogState.Register:
                        {
                            var Email = EmailBox.Text;
                            var Pass = PasswordBox.Password;

                            bool Passed = false;
                            if (NumberGen.Text == NumberBox.Text)
                                Passed = true;
                            else
                                Passed = false;

                            if (Passed)
                            {
                                var success = await Task.Run(() => App.FirebaseManagement.CreateUser(Email, Pass, GoodPass));
                                if (success == 1)
                                {
                                    sb = FindResource("SignUpRetract") as Storyboard;
                                    LoginResultText.Text = "";
                                    ButtonLogin.Content = "";
                                    EmailBox.Visibility = Visibility.Hidden;
                                    PasswordBox.Visibility = Visibility.Hidden;
                                    NumberBox.Visibility = Visibility.Hidden;
                                    NumberGen.Visibility = Visibility.Hidden;
                                    NumberGenText.Visibility = Visibility.Hidden;
                                    PasswordStrength.Visibility = Visibility.Hidden;
                                    BackButton.Visibility = Visibility.Hidden;
                                    StageText.Text = "";
                                    sb.Completed += (s, eventargs) =>
                                    {
                                        ButtonLogin.IsEnabled = true;
                                        ButtonLogin.Content = "Login";
                                        ButtonRegister.Visibility = Visibility.Visible;
                                        ButtonRegister.IsEnabled = true;
                                        StageText.Text = "Please Signin";
                                    };
                                    sb.Begin();
                                    StateLog = LogState.Main;
                                    IsWorking = false;
                                    break;
                                }
                                else if (success == 2)
                                {
                                    ButtonLogin.IsEnabled = true;
                                    LoginResultText.Foreground = Brushes.Orange;
                                    LoginResultText.Text = "Password is too weak, must be 6 Characters or longer. For strong password include symbols and numbers";
                                    IsWorking = false;
                                }
                                else if (success == 3)
                                {
                                    ButtonLogin.IsEnabled = true;
                                    LoginResultText.Foreground = Brushes.Orange;
                                    LoginResultText.Text = "Failed to create account, try again";
                                    IsWorking = false;
                                }
                                else if (success == 4)
                                {
                                    ButtonLogin.IsEnabled = true;
                                    LoginResultText.Foreground = Brushes.Orange;
                                    LoginResultText.Text = "Unable to connect. Check connection.";
                                    Console.WriteLine("Failed");
                                    IsWorking = false;
                                }
                            }
                            else if (!Passed)
                            {
                                ButtonLogin.IsEnabled = true;
                                LoginResultText.Foreground = Brushes.Red;
                                LoginResultText.Text = "Failed reCatcha, Try again";
                                NumberBox.BorderBrush = Brushes.Red;
                                IsWorking = false;
                            }
                            IsWorking = false;
                            break;
                        }
                }
            }
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ButtonLogin_Click(null, null);
        }

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            switch (StateLog)
            {
                case LogState.Main:
                    {
                        EmailBox.KeyUp -= TextBox_KeyUp;
                        PasswordBox.KeyUp -= TextBox_KeyUp;
                        StageText.Text = "";
                        LoginResultText.Text = "";
                        ButtonRegister.Visibility = Visibility.Hidden;
                        ButtonLogin.Content = "";
                        sb = FindResource("SignUpExpand") as Storyboard;
                        sb.Completed += (s, eventargs) => { EmailBox.Visibility = Visibility.Visible;
                            PasswordBox.Visibility = Visibility.Visible;
                            NumberBox.Visibility = Visibility.Visible;
                            NumberGen.Visibility = Visibility.Visible;
                            NumberGenText.Visibility = Visibility.Visible;
                            PasswordStrength.Visibility = Visibility.Visible;
                            BackButton.Visibility = Visibility.Visible;
                            Random random = new Random();
                            string captcha = random.Next(0, 9999).ToString("D4");
                            NumberGen.Text = captcha;
                            ButtonLogin.Content = "Confirm";
                            StageText.Text = "Provide an Email and a password(Must be longer than 6 characters). For strong password include symbols and numbers";
                        };
                        sb.Begin();
                        StateLog = LogState.Register;
                        break;
                    }
            }
            ButtonRegister.IsEnabled = false;
            ButtonRegister.Visibility = Visibility.Collapsed;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        //All events with regards to getting and lossing focus 
        #region Focus Events     
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
            if(PasswordBox.Password == "pass")
                PasswordBox.Password = "";
        }

        private void PasswordBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (PasswordBox.Password == "")
                PasswordBox.Password = "pass";
        }

        private void NumberBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (NumberBox.Text == "")
                NumberBox.Text = "Number";
        }

        private void NumberBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (NumberBox.Text == "Number")
                NumberBox.Text = "";
        }


        #endregion

        private enum LogState
        {
            Main,
            Login,
            Register
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (started)
            {
                PasswordBox passwordbox = sender as PasswordBox;
                int numberOfDigits = 0, numberOfLetters = 0, numberOfSymbols = 0;
                foreach (char c in passwordbox.Password)
                {
                    if (char.IsDigit(c))
                        numberOfDigits++;
                    else if (char.IsLetter(c))
                        numberOfLetters++;
                    else if (char.IsSymbol(c))
                        numberOfSymbols++;
                }

                int length = passwordbox.Password.Length;

                if (numberOfDigits > 0 && numberOfSymbols > 0 && length > 5)
                {
                    PasswordStrength.Fill = Brushes.LightGreen;
                    GoodPass = true;
                }
                else if (numberOfDigits.Equals(0) && numberOfSymbols.Equals(0) && length > 5)
                {
                    PasswordStrength.Fill = Brushes.Orange;
                    GoodPass = true;
                }
                else if (numberOfDigits.Equals(0) && numberOfSymbols.Equals(0) && length < 6 || passwordbox.Password == "password")
                {
                    PasswordStrength.Fill = Brushes.Red;
                    GoodPass = false;
                }
                else if(length > 5)
                {
                    PasswordStrength.Fill = Brushes.Orange;
                    GoodPass = true;
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            switch (StateLog)
            {
                case LogState.Login:
                    {

                        EmailBox.KeyUp -= TextBox_KeyUp;
                        PasswordBox.KeyUp -= TextBox_KeyUp;
                        ButtonLogin.Content = "";
                        sb = FindResource("SignInRetract") as Storyboard;
                        LoginResultText.Text = "";
                        SaveEmailCheck.Visibility = Visibility.Hidden;
                        EmailBox.Visibility = Visibility.Hidden;
                        PasswordBox.Visibility = Visibility.Hidden;
                        BackButton.Visibility = Visibility.Hidden;
                        StageText.Text = "";
                        ButtonLogin.Content = "";
                        sb.Completed += (s, eventargs) => {
                            ButtonLogin.Content = "Login";
                            ButtonRegister.Visibility = Visibility.Visible;
                            ButtonRegister.IsEnabled = true;
                            StageText.Text = "Please Signin";
                            ButtonLogin.Content = "Login";
                        };
                        sb.Begin();
                        StateLog = LogState.Main;
                        break;
                    }
                case LogState.Register:
                    {
                        EmailBox.KeyUp -= TextBox_KeyUp;
                        PasswordBox.KeyUp -= TextBox_KeyUp;
                        sb = FindResource("SignUpRetract") as Storyboard;
                        LoginResultText.Text = "";
                        ButtonLogin.Content = "";
                        EmailBox.Visibility = Visibility.Hidden;
                        PasswordBox.Visibility = Visibility.Hidden;
                        NumberBox.Visibility = Visibility.Hidden;
                        NumberGen.Visibility = Visibility.Hidden;
                        NumberGenText.Visibility = Visibility.Hidden;
                        PasswordStrength.Visibility = Visibility.Hidden;
                        BackButton.Visibility = Visibility.Hidden;
                        StageText.Text = "";
                        sb.Completed += (s, eventargs) =>
                        {
                            ButtonLogin.Content = "Login";
                            ButtonRegister.Visibility = Visibility.Visible;
                            ButtonRegister.IsEnabled = true;
                            StageText.Text = "Please Signin";
                        };
                            sb.Begin();
                        StateLog = LogState.Main;
                        break;
                    }
            }
        }
    }
}
