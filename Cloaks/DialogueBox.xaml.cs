using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Cloaks
{
    public partial class DialogueBox : Window
    {
        private Action<bool> callback;
        private string title;
        private string message;
        private bool error;
        private MainWindow main;
        private bool eula;

        private DialogueBox(string title, string message, bool error, Action<bool> callback, MainWindow main, bool eula)
        {
            this.title = title;
            this.message = message;
            this.error = error;
            this.callback = callback;
            this.main = main;
            this.eula = eula;

            InitializeComponent();
            Hide();
        }

        public static void Show(string title, string message, MainWindow main)
        {
            
            new DialogueBox(title, message, false, null, main, false).ShowDialog();
        }

        public static void ShowWithCallback(string title, string message, Action<bool> callback, MainWindow main)
        {
            
            new DialogueBox(title, message, false, callback, main, false).ShowDialog();
        }

        public static void ShowError(string title, string message, MainWindow main)
        {
           
            new DialogueBox(title, message, true, null, main, false).ShowDialog();
        }

        public static void ShowErrorWithCallback(string title, string message, Action<bool> callback, MainWindow main)
        {
           
            new DialogueBox(title, message, true, callback, main, false).ShowDialog();
        }

        public static void ShowEULA(Action<bool> callback, MainWindow main)
        {
          
            new DialogueBox("Cloaks+ End User License Agreement", @"By clicking 'I Agree' below, you agree to the Cloaks+ End User License Agreement. To view the contents of the agreement, click the 'EULA' button below.", false, callback, main, true).ShowDialog();
        }

        private void Box_Loaded(object sender, RoutedEventArgs e)
        {
            WindowTitle.Content = title;
            DialogueMessage.Text = message;
            if (eula)
            {
                EULAButton.Opacity = 100;
                EULAButton.Visibility = Visibility.Visible;
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
            Close();
            try
            {
                main.Show();
                main.Activate();
            }
            catch (Exception) { };

            callback?.Invoke(result);
        }

        private void EULAButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://cloaksplus.com/terms");
        }
    }
}
