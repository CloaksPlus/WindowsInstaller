using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Cloaks
{
    public partial class DialogueBox : Window
    {
        private string title;
        private string message;
        private bool error;
        private MainWindow main;
        private bool eula;

        private bool result;

        private DialogueBox(string title, string message, bool error, MainWindow main, bool eula)
        {
            this.title = title;
            this.message = message;
            this.error = error;
            this.main = main;
            this.eula = eula;

            InitializeComponent();
            Hide();
        }

        public static bool Show(string title, string message, MainWindow main)
        {

            DialogueBox w = new DialogueBox(title, message, false, main, false);
            w.Activate();
            w.ShowDialog();

            return w.result;
        }

        public static bool ShowError(string title, string message, MainWindow main)
        {
            DialogueBox w = new DialogueBox(title, message, true, main, false);
            w.Activate();
            w.ShowDialog();

            return w.result;
        }

        public static bool ShowTOU(MainWindow main)
        {
            DialogueBox w = new DialogueBox("Cloaks+ Terms and Conditions of Use", @"By clicking 'I Agree' below, you agree to the Cloaks+ Terms and Conditions of Use. To view the contents of the agreement, click the 'Terms' button below.", false, main, true);
            w.Activate();
            w.ShowDialog();

            return w.result;
        }

        private void Box_Loaded(object sender, RoutedEventArgs e)
        {
            WindowTitle.Content = title;
            DialogueMessage.Text = message;
            if (eula)
            {
                TermsButton.Opacity = 100;
                TermsButton.Visibility = Visibility.Visible;
                OKButton.Content = "I Agree";
            }

            if (error)
            {
                CancelButton.Opacity = 0;
                CancelButton.Visibility = Visibility.Collapsed;
            }
        }

        private void TopBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseDialogue(false);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CloseDialogue(false);
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            CloseDialogue(true);
        }

        private void CloseDialogue(bool result)
        {
            this.result = result;

            try
            {
                main.Show();
                main.Activate();
            }
            catch (Exception) { };

            Close();
        }

        private void TermsButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://cloaksplus.com/terms");
        }
    }
}
