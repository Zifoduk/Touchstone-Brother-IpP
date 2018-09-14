using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Forms = System.Windows.Forms;
using System.Windows.Threading;
using Button = System.Windows.Controls.Button;

namespace Touchstone_Brother_IpP
{
    /// <summary>
    /// Interaction logic for PopDialog.xaml
    /// </summary>
    partial class PopDialog : Window
    {
        public Forms.DialogResult result { get; set; }
        Forms.MessageBoxButtons MessageboxButtons;
        public PopDialog(string content, string title, Forms.MessageBoxButtons buttons)
        {
            InitializeComponent();
            List<MessageButton> buttonsCollection = new List<MessageButton>();
            MessageboxButtons = buttons;
            switch (buttons)
            {
                case Forms.MessageBoxButtons.OK:
                    buttonsCollection.Add(new MessageButton { ContentButton = new Button(), ContentButtonText = "OK" });
                    break;
                case Forms.MessageBoxButtons.OKCancel:
                    buttonsCollection.Add(new MessageButton { ContentButton = new Button(), ContentButtonText = "OK" });
                    buttonsCollection.Add(new MessageButton { ContentButton = new Button(), ContentButtonText = "Cancel" });
                    break;
                case Forms.MessageBoxButtons.AbortRetryIgnore:
                    buttonsCollection.Add(new MessageButton { ContentButton = new Button(), ContentButtonText = "Abort" });
                    buttonsCollection.Add(new MessageButton { ContentButton = new Button(), ContentButtonText = "Retry" });
                    buttonsCollection.Add(new MessageButton { ContentButton = new Button(), ContentButtonText = "Ignore" });
                    break;
                case Forms.MessageBoxButtons.RetryCancel:
                    buttonsCollection.Add(new MessageButton { ContentButton = new Button(), ContentButtonText = "Retry" });
                    buttonsCollection.Add(new MessageButton { ContentButton = new Button(), ContentButtonText = "Cancel" });
                    break;
                case Forms.MessageBoxButtons.YesNo:
                    buttonsCollection.Add(new MessageButton { ContentButton = new Button(), ContentButtonText = "Yes" });
                    buttonsCollection.Add(new MessageButton { ContentButton = new Button(), ContentButtonText = "No" });
                    break;
                case Forms.MessageBoxButtons.YesNoCancel:
                    buttonsCollection.Add(new MessageButton { ContentButton = new Button(), ContentButtonText = "Yes" });
                    buttonsCollection.Add(new MessageButton { ContentButton = new Button(), ContentButtonText = "No" });
                    buttonsCollection.Add(new MessageButton { ContentButton = new Button(), ContentButtonText = "Cancel" });
                    break;
            }
            ContentText.Text = content;
            ContentTitle.Text = title;
            MessageBoxContentButtons.ItemsSource = buttonsCollection;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {   
            Task closeTask = new Task(new Action(() => 
            {
                Task.Delay(1000);
                this.Dispatcher.Invoke(DispatcherPriority.Normal, new ThreadStart(this.Close));
            }));

            result = Forms.DialogResult.Cancel;
            closeTask.Start();
        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ContentButton_Click(object sender, RoutedEventArgs e)
        {
            var objbutton = (Button)sender;
            string buttonType = (string)objbutton.Content;
            switch(MessageboxButtons)
            {
                case Forms.MessageBoxButtons.OK:
                    if (buttonType == "OK")
                        result = Forms.DialogResult.OK;
                    break;
                case Forms.MessageBoxButtons.OKCancel:
                    if (buttonType == "OK")
                        result = Forms.DialogResult.OK;
                    else if (buttonType == "Cancel")
                        result = Forms.DialogResult.Cancel;
                    break;
                case Forms.MessageBoxButtons.AbortRetryIgnore:
                    if (buttonType == "Abort")
                        result = Forms.DialogResult.Abort;
                    else if (buttonType == "Retry")
                        result = Forms.DialogResult.Retry;
                    else if (buttonType == "Ignore")
                        result = Forms.DialogResult.Ignore;
                    break;
                case Forms.MessageBoxButtons.RetryCancel:
                    if (buttonType == "Retry")
                        result = Forms.DialogResult.Retry;
                    else if (buttonType == "Cancel")
                        result = Forms.DialogResult.Cancel;
                    break;
                case Forms.MessageBoxButtons.YesNo:
                    if (buttonType == "Yes")
                        result = Forms.DialogResult.Yes;
                    else if (buttonType == "No")
                        result = Forms.DialogResult.No;
                    break;
                case Forms.MessageBoxButtons.YesNoCancel:
                    if (buttonType == "Yes")
                        result = Forms.DialogResult.Yes;
                    else if (buttonType == "No")
                        result = Forms.DialogResult.No;
                    else if (buttonType == "Cancel")
                        result = Forms.DialogResult.Cancel;
                    break;
            }
            this.Close();
        }
    }

    public static class PMessageBox
    {
        public static Forms.DialogResult Show(string Content, string Title, Forms.MessageBoxButtons Buttons)
        {
            var messagebox = new PopDialog(Content, Title, Buttons);
            messagebox.ShowDialog();
            return messagebox.result;
        }
    }

    public class MessageButton
    {
        public Button ContentButton { get; set; }
        public string ContentButtonText { get; set; }
    }
}
