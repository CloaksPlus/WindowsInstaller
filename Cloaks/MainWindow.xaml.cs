using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;
using System.Windows.Threading;

namespace Cloaks
{
    public partial class MainWindow : Window
    {
        // Animation 
        private readonly Animator animator = new Animator();

        // Updater 
        private static readonly string VERSION_LINK = "https://api.github.com/repos/CloaksPlus/NewInstaller/releases/latest";

        // Hosts
        private static readonly string HOSTS_PATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts");

        // Frame Link Colors
        private static readonly Color HighlightColor = Color.FromArgb(0xFF, 0x43, 0x43, 0x43);
        private static readonly Color DarkColor = Color.FromArgb(0xFF, 0x1C, 0x1C, 0x1C);

        AutoResetEvent updateHandle = new AutoResetEvent(false);

        public MainWindow()
        {
            if (!IsAdministrator())
            {
                DialogueBox.Show("Cloaks+", "You need to run this application as an administrator!", this);
                Close();
                Environment.Exit(0);
            }

            try
            {
                InitializeComponent();
                Activate();

                // Create a thread
                Thread newWindowThread = new Thread(new ThreadStart(() =>
                {
                    CheckForUpdate();
                    Dispatcher.Run();
                }));
                newWindowThread.SetApartmentState(ApartmentState.STA);
                newWindowThread.IsBackground = true;
                newWindowThread.Start();

                updateHandle.WaitOne();
            }
            catch (Exception ex)
            {
                ThrowError(ex, "trying to update");
            }
        }

        private void ThrowError(Exception ex, string action)
        {
            DialogueBox.ShowError("Cloaks+ Error!", "Cloaks+ has encountered an error while " + action + ". Please send the error message below to the Discord server.\n\n" + ex.Message + "\nError source: " + ex.Source, this);
            Environment.Exit(0);
        }

        /* UPDATE CHECK */

        private void CheckForUpdate()
        {
            // Remove Old Update Files
            {
                string fileName = Process.GetCurrentProcess().MainModule.FileName;
                File.SetAttributes(fileName, FileAttributes.Normal);
                if (File.Exists(fileName + "_"))
                {
                    File.SetAttributes(fileName + "_", FileAttributes.Normal);
                    File.Delete(fileName + "_");
                }
            }

            WebClient webClient = new WebClient();
            webClient.Headers.Add("User-Agent", "Windows / Cloaks+ Installer");

            dynamic githubResponse = JsonConvert.DeserializeObject(webClient.DownloadString(VERSION_LINK));

            string githubVersion = "" + githubResponse.tag_name;

            Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
            Version latestVersion = new Version(githubVersion.Substring(0, 1) == "v" ? githubVersion.Substring(1) : githubVersion);

            int versionComapre = currentVersion.CompareTo(latestVersion);

            if (versionComapre > 0 || versionComapre == 0 || (bool)githubResponse.prerelease || (bool)githubResponse.draft)
            {
                updateHandle.Set();
                return; // Dont update if the current version is higher (dev build) or equal (up to date)
            }

            bool res = DialogueBox.Show("Cloaks+ | Update avaliable", "This version of Cloaks+ is outdated. Please press OK to update.", this);

            if (!res)
            {
                Environment.Exit(0);
                return;
            }

            try
            {
                // Download New Version
                string fileName = Process.GetCurrentProcess().MainModule.FileName;
                File.SetAttributes(fileName, FileAttributes.Normal);
                if (File.Exists(fileName + "_"))
                {
                    File.SetAttributes(fileName + "_", FileAttributes.Normal);
                    File.Delete(fileName + "_");
                }

                File.Move(fileName, fileName + "_");

                string tempName = Path.GetDirectoryName(fileName) + "\\temp.exe";
                webClient.Proxy = null;
                webClient.DownloadFile("" + githubResponse.assets[0].browser_download_url, tempName);
                File.Move(tempName, fileName);

                // Start new Process and Terminate the running one
                ProcessStartInfo startInfo = new ProcessStartInfo(fileName) { Verb = "runas" };
                Process.Start(startInfo);
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                ThrowError(ex, "trying to update");
            }
        }

        ///* ANIMATION */

        private void Cloaks_Loaded(object sender, RoutedEventArgs e)
        {
            animator.Fade(MainBorder);
            animator.Fade(TopBorder);
            animator.Fade(SelectFrame);

            Activate();
        }

        /* WINDOW INTERACTIONS */

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

            SideHomeButton.Background = new SolidColorBrush(HighlightColor);
            InstallButtonSide.Background = new SolidColorBrush(DarkColor);
            CreditsButtonSide.Background = new SolidColorBrush(DarkColor);
        }

        private void InstallFrame_Click(object sender, RoutedEventArgs e)
        {
            HomeFrame.Visibility = Visibility.Collapsed;
            HomeFrame.Opacity = 0;
            InstallFrame.Opacity = 100;
            InstallFrame.Visibility = Visibility.Visible;
            CreditsFrame.Visibility = Visibility.Collapsed;
            CreditsFrame.Opacity = 0;

            SideHomeButton.Background = new SolidColorBrush(DarkColor);
            InstallButtonSide.Background = new SolidColorBrush(HighlightColor);
            CreditsButtonSide.Background = new SolidColorBrush(DarkColor);
        }

        private void CreditsFrame_Click(object sender, RoutedEventArgs e)
        {
            HomeFrame.Visibility = Visibility.Collapsed;
            HomeFrame.Opacity = 0;
            InstallFrame.Visibility = Visibility.Collapsed;
            InstallFrame.Opacity = 0;
            CreditsFrame.Visibility = Visibility.Visible;
            CreditsFrame.Opacity = 100;

            SideHomeButton.Background = new SolidColorBrush(DarkColor);
            InstallButtonSide.Background = new SolidColorBrush(DarkColor);
            CreditsButtonSide.Background = new SolidColorBrush(HighlightColor);
        }

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            bool result = DialogueBox.ShowEULA(this);

            if (!result)
            {
                // If Dialogue was canceled, do nothing
                return;
            }

            InstallProgress.Show("Installing", this);

            // If Dialogue was successful try installing
            taskBarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
            try
            {
                InstallCloaks();
            }
            catch (Exception ex)
            {
                ThrowError(ex, "installing");
            }
            taskBarItemInfo.ProgressState = TaskbarItemProgressState.None;
        }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                taskBarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
                UninstallCloaks();
                taskBarItemInfo.ProgressState = TaskbarItemProgressState.None;
            }
            catch (Exception ex)
            {
                ThrowError(ex, "uninstalling");
            }
        }

        private async void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            animator.FadeOut(MainBorder);
            animator.FadeOut(TopBorder);
            animator.FadeOut(SelectFrame);
            await Task.Delay(600);
            Environment.Exit(0);
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        /* LOGIC HELPERS */

        private bool CloaksPlusExists()
        {
            try
            {
                return File.ReadAllText(HOSTS_PATH).Contains("161.35.130.99 s.optifine.net");
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool HostsIsReadonly()
        {
            FileInfo file = new FileInfo(HOSTS_PATH);
            return file.IsReadOnly;
        }

        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void RemoveAllInstallations()
        {
            // Filter our all lines with "s.optifine.net" (or old Cloaks+ content) and write the valid lines back
            string OPTIFINE_URL = "s.optifine.net";
            string OLD_CLOAKS_MARKER = "INSERTED BY CLOAKS+";
            var hostsContent = File.ReadAllLines(HOSTS_PATH);
            var validLines = hostsContent.Where(line => !(line.Contains(OPTIFINE_URL) || line.Contains(OLD_CLOAKS_MARKER)));
            File.WriteAllLines(HOSTS_PATH, validLines);
        }

        /* PROGRAM LOGIC */

        private void InstallCloaks()
        {
            // Check if the hosts file exists at all
            if (!File.Exists(HOSTS_PATH))
            {
                File.WriteAllText(HOSTS_PATH, "\n161.35.130.99 s.optifine.net # LINE INSERTED BY CLOAKS+");
                DialogueBox.Show("Cloaks+", "Cloaks+ successfully installed!", this);
                return;
            }

            if (HostsIsReadonly())
            {
                File.SetAttributes(HOSTS_PATH, FileAttributes.Normal);
            }

            string message = "Cloaks+ successfully installed!";

            if (CloaksPlusExists())
            {
                message = "Cloaks+ installation was successfully repaired!";
            }

            RemoveAllInstallations();

            // Append to the end of the file
            using (StreamWriter hosts = File.AppendText(HOSTS_PATH))
            {
                hosts.WriteLine("\n161.35.130.99 s.optifine.net # LINE INSERTED BY CLOAKS+");
                DialogueBox.Show("Cloaks+", message, this);
            }
            File.SetAttributes(HOSTS_PATH, FileAttributes.ReadOnly | FileAttributes.System);

        }

        private void UninstallCloaks()
        {
            InstallProgress.Show("Uninstalling", this);

            if (!CloaksPlusExists())
            {
                DialogueBox.Show("Not Found", "Cloaks+ installation was not found on system.", this);
                return;
            }

            if (HostsIsReadonly())
            {
                File.SetAttributes(HOSTS_PATH, FileAttributes.Normal);
            }

            RemoveAllInstallations();
            DialogueBox.Show("Cloaks+", "Cloaks+ successfully uninstalled!", this);
        }
    }
}
