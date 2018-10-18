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
        private bool GoodPass = false;
        private bool started = false;

        public Startup()
        {
            InitializeComponent();
            App.startup = this;            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            started = true;
        }

        private async void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
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
                            StageText.Text = "Provide your account email and password";
                            ButtonLogin.Content = "Continue";
                        };
                        sb.Begin();
                        StateLog = LogState.Login;
                        ButtonRegister.IsEnabled = false;
                        ButtonRegister.Visibility = Visibility.Collapsed;
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
                            LoginResultText.Foreground = Brushes.Red;
                            LoginResultText.Text = "Bad Credentials. Check Email and Password.";
                            Console.WriteLine("Failed");
                        }
                        else if (Passed == 3)
                        {
                            LoginResultText.Foreground = Brushes.Orange;
                            LoginResultText.Text = "Unable to connect. Check connection.";
                            Console.WriteLine("Failed");
                        }
                        Console.WriteLine("LoginNext");
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
                            if(success == 1)
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
                                    ButtonLogin.Content = "Login";
                                    ButtonRegister.Visibility = Visibility.Visible;
                                    ButtonRegister.IsEnabled = true;
                                    StageText.Text = "Please Signin";
                                };
                                sb.Begin();
                                StateLog = LogState.Main;
                                break;
                            }
                            else if(success == 2)
                            {
                                LoginResultText.Foreground = Brushes.Orange;
                                LoginResultText.Text = "Password is too weak, must be 6 Characters or longer. For strong password include symbols and numbers";
                            }
                            else if (success == 3)
                            {
                                LoginResultText.Foreground = Brushes.Orange;
                                LoginResultText.Text = "Failed to create account, try again";
                            }
                            else if (success == 4)
                            {
                                LoginResultText.Foreground = Brushes.Orange;
                                LoginResultText.Text = "Unable to connect. Check connection.";
                                Console.WriteLine("Failed");
                            }
                        }
                        else if (!Passed)
                        {
                            LoginResultText.Foreground = Brushes.Red;
                            LoginResultText.Text = "Failed reCatcha, Try again";
                            NumberBox.BorderBrush = Brushes.Red;                            
                        }
                        break;
                    }
            }
        }

        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {
            switch (StateLog)
            {
                case LogState.Main:
                    {
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
            if(PasswordBox.Password == "password")
                PasswordBox.Password = "";
        }

        private void NumberBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (PasswordBox.Password == "")
                PasswordBox.Password = "password";
        }

        private void NumberBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (NumberBox.Text == "Number")
                NumberBox.Text = "";
        }

        private void PasswordBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (NumberBox.Text == "")
                NumberBox.Text = "Number";
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (StateLog)
            {
                case LogState.Login:
                    {
                        ButtonLogin.Content = "";
                        sb = FindResource("SignInRetract") as Storyboard;
                        LoginResultText.Text = "";

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
