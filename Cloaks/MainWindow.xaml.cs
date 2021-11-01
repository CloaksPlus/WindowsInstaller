using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Shell;
using System.Security.Principal;

namespace Cloaks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static readonly string version = "1.3";
        public static readonly string versionLink = "https://raw.githubusercontent.com/SeizureSaladd/vers/main/vers.txt";
        public static string installerDownload = new WebClient()
        { Proxy = ((IWebProxy)null) }.DownloadString("https://raw.githubusercontent.com/SeizureSaladd/vers/main/download.txt");
        public string hosts = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts");



        Storyboard StoryBoard = new Storyboard();
        TimeSpan duration = TimeSpan.FromMilliseconds(500);
        TimeSpan duration2 = TimeSpan.FromMilliseconds(1000);

        private IEasingFunction Smooth
        {
            get;
            set;
        }
        = new QuarticEase
        {
            EasingMode = EasingMode.EaseInOut
        };

        public void Fade(DependencyObject Object)
        {
            DoubleAnimation FadeIn = new DoubleAnimation()
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(duration),
            };
            Storyboard.SetTarget(FadeIn, Object);
            Storyboard.SetTargetProperty(FadeIn, new PropertyPath("Opacity", 1));
            StoryBoard.Children.Add(FadeIn);
            StoryBoard.Begin();
        }

        public void FadeOut(DependencyObject Object)
        {
            DoubleAnimation Fade = new DoubleAnimation()
            {
                From = 1.0,
                To = 0.0,
                Duration = new Duration(duration),
            };
            Storyboard.SetTarget(Fade, Object);
            Storyboard.SetTargetProperty(Fade, new PropertyPath("Opacity", 1));
            StoryBoard.Children.Add(Fade);
            StoryBoard.Begin();
        }

        public void ObjectShift(DependencyObject Object, Thickness Get, Thickness Set)
        {
            ThicknessAnimation Animation = new ThicknessAnimation()
            {
                From = Get,
                To = Set,
                Duration = duration2,
                EasingFunction = Smooth,
            };
            Storyboard.SetTarget(Animation, Object);
            Storyboard.SetTargetProperty(Animation, new PropertyPath(MarginProperty));
            StoryBoard.Children.Add(Animation);
            StoryBoard.Begin();
        }

        private bool CloaksPlusExist()
        {
            try
            {
                return hosts.Contains("161.35.130.99 s.optifine.net");
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool IsReadonly()
        {
            FileInfo file = new FileInfo(hosts);
            return file.IsReadOnly;
        }

        public bool IsInstalled()
        {
            return CloaksPlusExist();
        }

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public MainWindow()
        {
            if (!IsAdministrator())
            {
                MessageBox.Show("You need to run this application as an administrator!", "Cloaks+", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }
            try
            {
                string getVersion = new WebClient().DownloadString(versionLink);
                if (version != getVersion.Trim())
                {
                    MessageBox.Show("This version of Cloaks+ is outdated. Please press OK to update.", "Cloaks+ | Update avaliable", MessageBoxButton.OK, MessageBoxImage.Error);
                    string ok = Path.GetDirectoryName(Directory.GetCurrentDirectory());
                    if (File.Exists(ok + "\\Cloaks+.exe"))
                        File.Delete(ok + "\\Cloaks+.exe");
                    new WebClient() { Proxy = ((IWebProxy)null) }.DownloadFile(MainWindow.installerDownload, ok + "\\Cloaks+.exe");
                    ProcessStartInfo startInfo = new ProcessStartInfo(ok + "\\Cloaks+.exe");
                    startInfo.Verb = "runas";
                    Process.Start(startInfo);
                    Close();
                    Environment.Exit(0);
                }
            }
            catch (Exception ohShitWhatNow)
            {
                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Error;
                MessageBox.Show("Cloaks+ has encountered an error trying to update. Please send the error message below to the Discord server.\n\n" + ohShitWhatNow.Message + "\nError source: " + ohShitWhatNow.Source, "Cloaks+ Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
            }
            //i'm dumb as shit and there's a \n at the end
            InitializeComponent();
            this.Activate();
        }

        private void Cloaks_Loaded(object sender, RoutedEventArgs e)
        {
            Fade(MainBorder);
            Fade(TopBorder);
            Fade(SelectFrame);

            ObjectShift(MainBorder, MainBorder.Margin, new Thickness(0, 0, 0, 0));
            ObjectShift(TopBorder, TopBorder.Margin, new Thickness(-2, -2, -2, 0));
            ObjectShift(SelectFrame, TopBorder.Margin, new Thickness(28, 30.5, 0, 0));
            ObjectShift(HomeFrame, HomeFrame.Margin, new Thickness(124, 63, 19, 35));

        }

        private void TopBorder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void HomeFrame_Click(object sender, RoutedEventArgs e)
        {
            HomeFrame.Opacity = 100;
            HomeFrame.Visibility = Visibility.Visible;
            InstallFrame.Opacity = 0;
            InstallFrame.Visibility = Visibility.Collapsed;
            CreditsFrame.Visibility = Visibility.Collapsed;
            CreditsFrame.Opacity = 0;
        }

        private void InstallFrame_Click(object sender, RoutedEventArgs e)
        {
            HomeFrame.Visibility = Visibility.Collapsed;
            HomeFrame.Opacity = 0;
            InstallFrame.Opacity = 100;
            InstallFrame.Visibility = Visibility.Visible;
            CreditsFrame.Visibility = Visibility.Collapsed;
            CreditsFrame.Opacity = 0;
        }

        private void CreditsFrame_Click(object sender, RoutedEventArgs e)
        {
            HomeFrame.Visibility = Visibility.Collapsed;
            HomeFrame.Opacity = 0;
            InstallFrame.Visibility = Visibility.Collapsed;
            InstallFrame.Opacity = 0;
            CreditsFrame.Visibility = Visibility.Visible;
            CreditsFrame.Opacity = 100;
        }

        private void InstallCloaks()
        {
            if (File.Exists(hosts))
            {
                if (IsReadonly())
                {
                    File.SetAttributes(hosts, FileAttributes.Normal);
                    InstallCloaks();
                }
                var deleteOptifineShit = "s.optifine.net";
                var read = File.ReadAllLines(hosts);
                var delete = read.Where(line => !line.Contains(deleteOptifineShit));
                File.WriteAllLines(hosts, delete);

                using (StreamWriter hosts = File.AppendText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts")))
                {
                    hosts.WriteLine("\n161.35.130.99 s.optifine.net # LINE INSERTED BY CLOAKS+");
                    MessageBox.Show("Cloaks+ successfully installed!", "Cloaks+", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                File.SetAttributes(hosts, FileAttributes.ReadOnly);
            }
            else
            {
                File.WriteAllText(hosts, "\n161.35.130.99 s.optifine.net # LINE INSERTED BY CLOAKS+");
                MessageBox.Show("Cloaks+ successfully installed!", "Cloaks+", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void UninstallCloaks()
        {
            if (IsInstalled())
            {
                if (IsReadonly())
                {
                    File.SetAttributes(hosts, FileAttributes.Normal);
                    UninstallCloaks();
                }
                File.WriteAllLines(hosts, File.ReadLines(hosts).Where(l => l != "161.35.130.99 s.optifine.net # LINE INSERTED BY CLOAKS+").ToList());
                var deleteOptifineShit = "s.optifine.net";
                var oldLines = File.ReadAllLines(hosts);
                var newLines = oldLines.Where(line => !line.Contains(deleteOptifineShit));
                File.WriteAllLines(hosts, newLines);
                MessageBox.Show("Cloaks+ successfully uninstalled!", "Cloaks+", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            else
            {
                MessageBox.Show("Cloaks+ not detected!", "Cloaks+", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            if (chequeBox.IsChecked == true)
            {
                try
                {
                    this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Indeterminate;
                    InstallCloaks();
                    this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
                }
                catch (Exception bruvIdkHowToSpellExecption)
                {
                    this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Error;
                    MessageBox.Show("Cloaks+ has encountered an error. Please send the error message below to the Discord server.\n\n" + bruvIdkHowToSpellExecption.Message + "\nError source: " + bruvIdkHowToSpellExecption.Source, "Cloaks+ Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
                }
            }
            else
            {
                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Error;
                MessageBox.Show("Please agree to the EULA!", "Cloaks+", MessageBoxButton.OK, MessageBoxImage.Error);
                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
            }
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Indeterminate;
                UninstallCloaks();
                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
            }
            catch (IOException IOError)
            {
                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Error;
                MessageBox.Show("Cloaks+ has encountered an error. Please send the error message below to the Discord server.\n\n" + IOError.Message + "\nError source: " + IOError.Source, "Cloaks+ Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
            }
            catch (Exception exeption) //idk how to spell lmao
            {
                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.Error;
                MessageBox.Show("Cloaks+ has encountered an error. Please send the error message below to the Discord server.\n\n" + exeption.Message + "\nError source: " + exeption.Source, "Cloaks+ Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                this.taskBarItemInfo1.ProgressState = TaskbarItemProgressState.None;
            }
        }

        private void EulaButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://cloaksplus.com/terms.txt");
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            FadeOut(MainBorder);
            FadeOut(TopBorder);
            FadeOut(SelectFrame);
            ObjectShift(MainBorder, MainBorder.Margin, new Thickness(49, 70, 49, 26));
            ObjectShift(TopBorder, TopBorder.Margin, new Thickness(0, -28, 0, 0));
            ObjectShift(SelectFrame, SelectFrame.Margin, new Thickness(-90, 79, 0, 0));
            ObjectShift(HomeFrame, HomeFrame.Margin, new Thickness(101, 0, 199, 230));
            await Task.Delay(1000);
            Application.Current.Shutdown();
            await Task.Delay(1000);
            System.Windows.Forms.Application.Exit();
            await Task.Delay(1000);
            System.Windows.Forms.Application.ExitThread();
            Environment.Exit(0);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
