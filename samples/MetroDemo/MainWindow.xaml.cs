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

        private Window flyoutDemo;
        private void LaunchFlyoutDemo(object sender, RoutedEventArgs e)
        {
            if (flyoutDemo == null) {
                flyoutDemo = new FlyoutDemo();
                flyoutDemo.Closed += (o, args) => flyoutDemo = null;
            }
            if (flyoutDemo.IsVisible)
                flyoutDemo.Hide();
            else
                flyoutDemo.Show();
        }

        private void LaunchPanoramaDemo(object sender, RoutedEventArgs e)
        {
            new PanoramaDemo().Show();
        }

        private void LaunchIcons(object sender, RoutedEventArgs e)
        {
            new IconsWindow().Show();
        }

        private Window cleanWindowDemo;
        private void LauchCleanDemo(object sender, RoutedEventArgs e)
        {
            if (cleanWindowDemo == null) {
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
            var dialog = (BaseMetroDialog) this.Resources["SimpleDialogTest"];
            dialog = dialog.ShowDialogExternally();

            await TaskEx.Delay(5000);

            dialog.RequestClose();
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

            var dialog = (BaseMetroDialog) this.Resources["SimpleDialogTest"];

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

        
        private void RangeSlider_OnLowerValueChanged(object sender, RangeParameterChangedEventArgs e)
        {
            //MessageBox.Show(e.OldValue.ToString() + "->" + e.NewValue.ToString());
        }

        private void RangeSlider_OnUpperValueChanged(object sender, RangeParameterChangedEventArgs e)
        {
            //MessageBox.Show(e.OldValue.ToString() + "->" + e.NewValue.ToString());
        }

        private void RangeSlider_OnLowerThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            //TestBlock.Text = "lower thumb drag started";
        }

        private void RangeSlider_OnLowerThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            //TestBlock.Text = "lower thumb drag completed";
        }

        private void RangeSlider_OnUpperThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            //TestBlock.Text = "upper thumb drag started";
        }

        private void RangeSlider_OnUpperThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            //TestBlock.Text = "upper thumb drag completed";
        }

        private void RangeSlider_OnCentralThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            //TestBlock.Text = "central thumb drag started";
        }

        private void RangeSlider_OnCentralThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            //TestBlock.Text = "central thumb drag completed";
        }

        private void RangeSlider_OnLowerThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            TestBlock.Text = RangeSlider.Result;
        }

        private void RangeSlider_OnUpperThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            TestBlock.Text = RangeSlider.Result;
        }

        private void RangeSlider_OnCentralThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            //TestBlock.Text = "central thumb drag delta";
        }
    }
}
