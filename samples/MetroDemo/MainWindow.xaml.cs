using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Threading;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;
using System.Windows.Data;
using MetroDemo.ExampleWindows;

namespace MetroDemo
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            DataContext = new MainWindowViewModel();
            InitializeComponent();
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

        private Window flyoutDemo;
        private void LaunchFlyoutDemo(object sender, RoutedEventArgs e)
        {
            if (flyoutDemo == null)
            {
                flyoutDemo = new FlyoutDemo();
                flyoutDemo.Closed += (o, args) => flyoutDemo = null;
            }
            if (flyoutDemo.IsVisible)
                flyoutDemo.Hide();
            else
                flyoutDemo.Show();
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
#if NET_4_5
            //new RibbonDemo().Show();
#else
            MessageBox.Show("Ribbon is only supported on .NET 4.5 or higher.");
#endif
        }

        private async void ShowDialogOutside(object sender, RoutedEventArgs e)
        {
            var dialog = (BaseMetroDialog)this.Resources["SimpleDialogTest"];
            dialog = dialog.ShowDialogExternally();

            await TaskEx.Delay(5000);

            await dialog.RequestCloseAsync();
        }

        private async void ShowMessageDialog(object sender, RoutedEventArgs e)
        {
            // This demo runs on .Net 4.0, but we're using the Microsoft.Bcl.Async package so we have async/await support
            // The package is only used by the demo and not a dependency of the library!
            this.MetroDialogOptions.ColorScheme = UseAccentForDialogsMenuItem.IsChecked ? MetroDialogColorScheme.Accented : MetroDialogColorScheme.Theme;

            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Hi",
                NegativeButtonText = "Go away!",
                FirstAuxiliaryButtonText = "Cancel"
            };

            MessageDialogResult result = await this.ShowMessageAsync("Hello!", "Welcome to the world of metro! ",
                MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, mySettings);

            if (result != MessageDialogResult.FirstAuxiliary)
                await this.ShowMessageAsync("Result", "You said: " + (result == MessageDialogResult.Affirmative ? mySettings.AffirmativeButtonText : mySettings.NegativeButtonText +
                    Environment.NewLine + Environment.NewLine + "This dialog will follow the Use Accent setting."));
        }

        private async void ShowSimpleDialog(object sender, RoutedEventArgs e)
        {
            this.MetroDialogOptions.ColorScheme = UseAccentForDialogsMenuItem.IsChecked ? MetroDialogColorScheme.Accented : MetroDialogColorScheme.Theme;

            var dialog = (BaseMetroDialog)this.Resources["SimpleDialogTest"];

            await this.ShowMetroDialogAsync(dialog);

            await TaskEx.Delay(5000);

            await this.HideMetroDialogAsync(dialog);
        }
        private async void ShowProgressDialog(object sender, RoutedEventArgs e)
        {
            this.MetroDialogOptions.ColorScheme = UseAccentForDialogsMenuItem.IsChecked ? MetroDialogColorScheme.Accented : MetroDialogColorScheme.Theme;

            var controller = await this.ShowProgressAsync("Please wait...", "We are cooking up some cupcakes!");

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
    }
}
