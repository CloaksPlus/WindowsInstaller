using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace Cloaks
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

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


        public MainWindow()
        {
            InitializeComponent();
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

        private async void ExitButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
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

        private void InstallButton_Click(object sender, RoutedEventArgs e)
        {
            
                string contents = File.ReadAllText("C:\\Windows\\System32\\drivers\\etc\\hosts");
                if (contents.Contains("159.203.120.188 s.optifine.net"))
                {

                    MessageBox.Show("You already have Cloaks+", "Cloaks+", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                else
                {


                using (StreamWriter hosts = File.AppendText(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers/etc/hosts")))
                {
                    hosts.WriteLine("\n159.203.120.188 s.optifine.net");
                    this.Activate();
                    MessageBox.Show("Cloaks+ successfully installed!", "Cloaks+", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                }
                
            
            
        }
    }

        private void UninstallButton_Click(object sender, RoutedEventArgs e)
        {
            string contents = File.ReadAllText("C:\\Windows\\System32\\drivers\\etc\\hosts");
            if (contents.Contains("159.203.120.188 s.optifine.net"))
            {

                var fileName = "C:\\Windows\\System32\\drivers\\etc\\hosts";
                File.WriteAllLines(fileName, File.ReadLines(fileName).Where(l => l != "159.203.120.188 s.optifine.net").ToList());
                MessageBox.Show("Cloaks+ successfully uninstalled!", "Cloaks+", MessageBoxButton.OK, MessageBoxImage.Information);
  
            }
            else
            {
                MessageBox.Show("Cloaks+ not detected!", "Cloaks+ Uninstaller", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EulaButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://cloaks.plus/terms.txt");
        }
    }
    }
