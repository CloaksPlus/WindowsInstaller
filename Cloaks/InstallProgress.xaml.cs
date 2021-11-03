using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace Cloaks
{
    public partial class InstallProgress : Window
    {

        private Timer windowTimer = new Timer();
        private string title;

        private InstallProgress(string title)
        {
            this.title = title;

            InitializeComponent();
            Activate();
        }

        public static void Show(string title, MainWindow main)
        {
            new InstallProgress(title).ShowDialog();
            main.Activate();
        }

        private void Box_Loaded(object sender, RoutedEventArgs e)
        {
            Title.Content = title;

            windowTimer.Enabled = true;
            windowTimer.Start();
            windowTimer.Interval = 1;
            InstallProgressBar.Maximum = 25;

            windowTimer.Tick += new EventHandler(OnTimerTick);

        }

        private void OnTimerTick(Object source, EventArgs e)
        {
            if (InstallProgressBar.Value < 25)
            {
                if (new Random().NextDouble() < 0.5)
                    InstallProgressBar.Value++;
            }
            else
            {
                windowTimer.Stop();
                Close();
            }
        }

        private void TopBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
    }



}
