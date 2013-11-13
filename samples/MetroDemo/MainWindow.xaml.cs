using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;
using System.Windows.Data;

namespace MetroDemo
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();
            var t = new DispatcherTimer(TimeSpan.FromSeconds(2), DispatcherPriority.Normal, Tick, this.Dispatcher);

            CollectionViewSource.GetDefaultView(groupingComboBox.ItemsSource).GroupDescriptions.Add(new PropertyGroupDescription("Artist"));
        }

        void Tick(object sender, EventArgs e)
        {
            var dateTime = DateTime.Now;
            transitioning.Content = new TextBlock { Text = "Transitioning Content! " + dateTime, SnapsToDevicePixels = true };
            customTransitioning.Content = new TextBlock { Text = "Custom transistion! " + dateTime, SnapsToDevicePixels = true };
        }

        private void ThemeLight(object sender, RoutedEventArgs e)
        {
            var theme = ThemeManager.DetectTheme(Application.Current);
            ThemeManager.ChangeTheme(Application.Current, theme.Item2, Theme.Light);
        }

        private void ThemeDark(object sender, RoutedEventArgs e)
        {
            var theme = ThemeManager.DetectTheme(Application.Current);
            ThemeManager.ChangeTheme(Application.Current, theme.Item2, Theme.Dark);
        }

        private void LaunchVisualStudioDemo(object sender, RoutedEventArgs e)
        {
            new VSDemo().Show();
        }

        private void LaunchFlyoutDemo(object sender, RoutedEventArgs e)
        {
            new FlyoutDemo().Show();
        }

        private void LaunchPanoramaDemo(object sender, RoutedEventArgs e)
        {
            new PanoramaDemo().Show();
        }

        private void LaunchIcons(object sender, RoutedEventArgs e)
        {
            new IconsWindow().Show();
        }

        private void LauchCleanDemo(object sender, RoutedEventArgs e)
        {
            new CleanWindowDemo().Show();
        }

        private void LaunchRibbonDemo(object sender, RoutedEventArgs e)
        {
#if NET_4_5
            //new RibbonDemo().Show();
#else
            MessageBox.Show("Ribbon is only supported on .NET 4.5 or higher.");
#endif
        }

        private void ShowMessageDialog(object sender, RoutedEventArgs e)
        {
            this.ShowMessageAsync("Hello!", "Welcome to the world of metro!", MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative).ContinueWith(x =>
                {
                    Dispatcher.BeginInvoke(new Action(() =>
                        {
                            this.ShowMessageAsync("Result", "You said: " + (x.Result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative ? "OK" : "Cancel"));
                        }));
                });
        }

        private void ShowSimpleDialog(object sender, RoutedEventArgs e)
        {
            var dialog = this.Resources["SimpleDialogTest"] as BaseMetroDialog;

            this.ShowMetroDialogAsync("This dialog allows arbitrary content. It will close in 5 seconds.", dialog).ContinueWith(x => System.Threading.Thread.Sleep(5000)).ContinueWith(y =>
                {
                    Dispatcher.Invoke(new Action(() =>
                        {
                            this.HideMetroDialogAsync(dialog);
                        }));
                });
        }
        private void ShowProgressDialog(object sender, RoutedEventArgs e)
        {
            var remoteTask = this.ShowProgressAsync("Please wait...", "We are cooking up some cupcakes!");


            //Ugly demo code that doesn't use 'await'. This would be much cleaner WITH await.
            ProgressDialogController remote = null;

            System.Threading.Tasks.Task.Factory.StartNew(() => System.Threading.Thread.Sleep(5000)).ContinueWith(x => Dispatcher.Invoke(new Action(() =>
                {
                    remote = remoteTask.Result;
                }))).ContinueWith(x =>
                    {
                        remote.SetCancelable(true);

                        double i = 0.0;
                        while (i < 6.0)
                        {
                            Dispatcher.Invoke(new Action(() =>
                                {
                                    double val = (i / 100.0) * 20.0;
                                    remote.SetProgress(val);
                                    remote.SetMessage("Baking cupcake: " + i.ToString() + "...");
                                }));

                            if (remote.IsCanceled)
                                break; //canceled progressdialog auto closes.

                            i += 1.0;
                            System.Threading.Thread.Sleep(2000);
                        }

                        Dispatcher.Invoke(new Action(() =>
                        {
                            remote.CloseAsync().ContinueWith(y =>
                                {
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        if (remote.IsCanceled)
                                        {
                                            this.ShowMessageAsync("No cupcakes!", "You stopped baking!");
                                        }
                                        else
                                        {
                                            this.ShowMessageAsync("Cupcakes!", "Your cupcakes are finished! Enjoy!");
                                        }
                                    }));
                                });
                        }));
                    });
        }

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var flipview = ((FlipView)sender);
            switch (flipview.SelectedIndex)
            {
                case 0:
                    flipview.BannerText = "Cupcakes!";
                    break;
                case 1:
                    flipview.BannerText = "Xbox!";
                    break;
                case 2:
                    flipview.BannerText = "Chess!";
                    break;
            }
        }

        private void MetroTabControl_TabItemClosingEvent(object sender, BaseMetroTabControl.TabItemClosingEventArgs e)
        {
            if (e.ClosingTabItem.Header.ToString().StartsWith("sizes"))
                e.Cancel = true;
        }

        private void InteropDemo(object sender, RoutedEventArgs e)
        {
            new InteropDemo().Show();

        }
    }
}
