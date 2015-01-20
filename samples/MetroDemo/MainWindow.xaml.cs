using System;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MetroDemo.ExampleWindows;

namespace MetroDemo
{
    public partial class MainWindow
    {
        private bool _shutdown;
        private readonly MainWindowViewModel _viewModel;
        private FlyoutDemo flyoutDemo;

        public MainWindow()
        {
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
            InitializeComponent();
            flyoutDemo = new FlyoutDemo();
            flyoutDemo.Closed += (o, args) => flyoutDemo = null;
            Closing += (s, e) =>
                {
                    if (!e.Cancel && flyoutDemo != null)
                    {
                        flyoutDemo.Dispose();
                    }
                };
        }

        public static readonly DependencyProperty ToggleFullScreenProperty =
            DependencyProperty.Register("ToggleFullScreen",
                                        typeof(bool),
                                        typeof(MainWindow),
                                        new PropertyMetadata(default(bool), ToggleFullScreenPropertyChangedCallback));

        private static void ToggleFullScreenPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var metroWindow = (MetroWindow)dependencyObject;
            if (e.OldValue != e.NewValue)
            {
                var fullScreen = (bool)e.NewValue;
                if (fullScreen)
                {
                    metroWindow.UseNoneWindowStyle = true;
                    metroWindow.IgnoreTaskbarOnMaximize = true;
                    metroWindow.ShowMinButton = false;
                    metroWindow.ShowMaxRestoreButton = false;
                    metroWindow.ShowCloseButton = false;
                    metroWindow.WindowState = WindowState.Maximized;
                }
                else
                {
                    metroWindow.UseNoneWindowStyle = false;
                    metroWindow.ShowTitleBar = true; // <-- this must be set to true
                    metroWindow.IgnoreTaskbarOnMaximize = false;
                    metroWindow.ShowMinButton = true;
                    metroWindow.ShowMaxRestoreButton = true;
                    metroWindow.ShowCloseButton = true;
                    metroWindow.WindowState = WindowState.Normal;
                }
            }
        }

        public bool ToggleFullScreen
        {
            get { return (bool)GetValue(ToggleFullScreenProperty); }
            set { SetValue(ToggleFullScreenProperty, value); }
        }

        private void LaunchMahAppsOnGitHub(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/MahApps/MahApps.Metro");
        }

        private void LaunchSizeToContentDemo(object sender, RoutedEventArgs e)
        {
            new SizeToContentDemo() { Owner = this }.Show();
        }

        private void LaunchVisualStudioDemo(object sender, RoutedEventArgs e)
        {
            new VSDemo().Show();
        }

        private void LaunchFlyoutDemo(object sender, RoutedEventArgs e)
        {
            if (flyoutDemo == null)
            {
                flyoutDemo = new FlyoutDemo();
                flyoutDemo.Closed += (o, args) => flyoutDemo = null;
            }
            flyoutDemo.ShowDialog();
        }

        private void LaunchIcons(object sender, RoutedEventArgs e)
        {
            new IconsWindow().Show();
        }

        private Window cleanWindowDemo;
        private void LauchCleanDemo(object sender, RoutedEventArgs e)
        {
            if (cleanWindowDemo == null)
            {
                cleanWindowDemo = new CleanWindowDemo();
                cleanWindowDemo.Closed += (o, args) => cleanWindowDemo = null;
            }
            if (cleanWindowDemo.IsVisible)
                cleanWindowDemo.Hide();
            else
                cleanWindowDemo.Show();
        }

        private void LaunchRibbonDemo(object sender, RoutedEventArgs e)
        {
#if NET4_5
            //new RibbonDemo().Show();
#else
            MessageBox.Show("Ribbon is only supported on .NET 4.5 or higher.");
#endif
        }

        private async void ShowDialogOutside(object sender, RoutedEventArgs e)
        {
            var dialog = (BaseMetroDialog)this.Resources["CustomDialogTest"];
            dialog = dialog.ShowDialogExternally();

            await TaskEx.Delay(5000);

            await dialog.RequestCloseAsync();
        }

        private async void ShowMessageDialog(object sender, RoutedEventArgs e)
        {
            // This demo runs on .Net 4.0, but we're using the Microsoft.Bcl.Async package so we have async/await support
            // The package is only used by the demo and not a dependency of the library!
            MetroDialogOptions.ColorScheme = UseAccentForDialogsMenuItem.IsChecked ? MetroDialogColorScheme.Accented : MetroDialogColorScheme.Theme;

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Hi",
                NegativeButtonText = "Go away!",
                FirstAuxiliaryButtonText = "Cancel",
                ColorScheme = UseAccentForDialogsMenuItem.IsChecked ? MetroDialogColorScheme.Accented : MetroDialogColorScheme.Theme
            };

            MessageDialogResult result = await this.ShowMessageAsync("Hello!", "Welcome to the world of metro!",
                MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, mySettings);

            if (result != MessageDialogResult.FirstAuxiliary)
                await this.ShowMessageAsync("Result", "You said: " + (result == MessageDialogResult.Affirmative ? mySettings.AffirmativeButtonText : mySettings.NegativeButtonText +
                    Environment.NewLine + Environment.NewLine + "This dialog will follow the Use Accent setting."));
        }


        private async void ShowLimitedMessageDialog(object sender, RoutedEventArgs e)
        {
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Hi",
                NegativeButtonText = "Go away!",
                FirstAuxiliaryButtonText = "Cancel",
                MaximumBodyHeight = 100,
                ColorScheme = UseAccentForDialogsMenuItem.IsChecked ? MetroDialogColorScheme.Accented : MetroDialogColorScheme.Theme
            };

            MessageDialogResult result = await this.ShowMessageAsync("Hello!", "Welcome to the world of metro!" + string.Join(Environment.NewLine, "abc","def","ghi", "jkl","mno","pqr","stu","vwx","yz"),
                MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, mySettings);

            if (result != MessageDialogResult.FirstAuxiliary)
                await this.ShowMessageAsync("Result", "You said: " + (result == MessageDialogResult.Affirmative ? mySettings.AffirmativeButtonText : mySettings.NegativeButtonText +
                    Environment.NewLine + Environment.NewLine + "This dialog will follow the Use Accent setting."));
        }

        private async void ShowCustomDialog(object sender, RoutedEventArgs e)
        {
            this.MetroDialogOptions.ColorScheme = UseAccentForDialogsMenuItem.IsChecked ? MetroDialogColorScheme.Accented : MetroDialogColorScheme.Theme;

            var dialog = (BaseMetroDialog)this.Resources["CustomDialogTest"];

            await this.ShowMetroDialogAsync(dialog);

            await TaskEx.Delay(5000);

            await this.HideMetroDialogAsync(dialog);
        }
        private async void ShowProgressDialog(object sender, RoutedEventArgs e)
        {
            this.MetroDialogOptions.ColorScheme = UseAccentForDialogsMenuItem.IsChecked ? MetroDialogColorScheme.Accented : MetroDialogColorScheme.Theme;

            var controller = await this.ShowProgressAsync("Please wait...", "We are baking some cupcakes!");

            await TaskEx.Delay(5000);

            controller.SetCancelable(true);

            double i = 0.0;
            while (i < 6.0)
            {
                double val = (i / 100.0) * 20.0;
                controller.SetProgress(val);
                controller.SetMessage("Baking cupcake: " + i + "...");

                if (controller.IsCanceled)
                    break; //canceled progressdialog auto closes.

                i += 1.0;

                await TaskEx.Delay(2000);
            }

            await controller.CloseAsync();

            if (controller.IsCanceled)
            {
                await this.ShowMessageAsync("No cupcakes!", "You stopped baking!");
            }
            else
            {
                await this.ShowMessageAsync("Cupcakes!", "Your cupcakes are finished! Enjoy!");
            }
        }

        private async void ShowInputDialog(object sender, RoutedEventArgs e)
        {
            this.MetroDialogOptions.ColorScheme = UseAccentForDialogsMenuItem.IsChecked ? MetroDialogColorScheme.Accented : MetroDialogColorScheme.Theme;

            var result = await this.ShowInputAsync("Hello!", "What is your name?");

            if (result == null) //user pressed cancel
                return;

            await this.ShowMessageAsync("Hello", "Hello " + result + "!");
        }

        private async void ShowLoginDialog(object sender, RoutedEventArgs e)
        {
            this.MetroDialogOptions.ColorScheme = UseAccentForDialogsMenuItem.IsChecked ? MetroDialogColorScheme.Accented : MetroDialogColorScheme.Theme;
            LoginDialogData result = await this.ShowLoginAsync("Authentication", "Enter your credentials", new LoginDialogSettings { ColorScheme = this.MetroDialogOptions.ColorScheme, InitialUsername = "MahApps"});
            if (result == null)
            {
                //User pressed cancel
            }
            else
            {
                MessageDialogResult messageResult = await this.ShowMessageAsync("Authentication Information", String.Format("Username: {0}\nPassword: {1}", result.Username, result.Password));
            }
        }

        private void InteropDemo(object sender, RoutedEventArgs e)
        {
            new InteropDemo().Show();

        }

        private void LaunchNavigationDemo(object sender, RoutedEventArgs e)
        {
            var navWin = new MetroNavigationWindow();
            navWin.Title = "Navigation Demo";

            //uncomment the next two lines if you want the clean style.
            //navWin.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Clean/CleanWindow.xaml", UriKind.Absolute) });
            //navWin.SetResourceReference(StyleProperty, "CleanWindowStyleKey");

            navWin.Show();
            navWin.Navigate(new Navigation.HomePage());
        }

        private async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !_shutdown && _viewModel.QuitConfirmationEnabled;
            if (_shutdown) return;

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Quit",
                NegativeButtonText = "Cancel",
                AnimateShow = true,
                AnimateHide = false
            };

            var result = await this.ShowMessageAsync("Quit application?",
                "Sure you want to quit application?",
                MessageDialogStyle.AffirmativeAndNegative, mySettings);

            _shutdown = result == MessageDialogResult.Affirmative;

            if (_shutdown)
                Application.Current.Shutdown();
        }
    }
}
