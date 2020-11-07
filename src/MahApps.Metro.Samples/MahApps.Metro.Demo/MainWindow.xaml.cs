// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MetroDemo.ExampleWindows;

namespace MetroDemo
{
    public partial class MainWindow : MetroWindow
    {
        private bool _shutdown;
        private readonly MainWindowViewModel _viewModel;
        private FlyoutDemo flyoutDemo;

        public MainWindow()
        {
            _viewModel = new MainWindowViewModel(DialogCoordinator.Instance);
            DataContext = _viewModel;

            InitializeComponent();

            flyoutDemo = new FlyoutDemo();
            flyoutDemo.ApplyTemplate();
            flyoutDemo.Closed += (o, e) => flyoutDemo = null;
        }

        public static readonly DependencyProperty ToggleFullScreenProperty =
            DependencyProperty.Register(nameof(ToggleFullScreen),
                                        typeof(bool),
                                        typeof(MainWindow),
                                        new PropertyMetadata(default(bool), OnToggleFullScreenChanged));

        private static void OnToggleFullScreenChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                var window = (MainWindow)dependencyObject;
                var fullScreen = (bool)e.NewValue;
                if (fullScreen)
                {
                    window.SetCurrentValue(IgnoreTaskbarOnMaximizeProperty, true);
                    window.SetCurrentValue(WindowStateProperty, WindowState.Maximized);
                    window.SetCurrentValue(UseNoneWindowStyleProperty, true);
                }
                else
                {
                    window.SetCurrentValue(WindowStateProperty, WindowState.Normal);
                    window.SetCurrentValue(UseNoneWindowStyleProperty, false);
                    window.SetCurrentValue(ShowTitleBarProperty, true); // <-- this must be set to true
                    window.SetCurrentValue(IgnoreTaskbarOnMaximizeProperty, false);
                }
            }
        }

        public bool ToggleFullScreen
        {
            get { return (bool)GetValue(ToggleFullScreenProperty); }
            set { SetValue(ToggleFullScreenProperty, value); }
        }

        public static readonly DependencyProperty UseAccentForDialogsProperty =
            DependencyProperty.Register(nameof(UseAccentForDialogs),
                                        typeof(bool),
                                        typeof(MainWindow),
                                        new PropertyMetadata(default(bool), OnUseAccentForDialogsChanged));

        private static void OnUseAccentForDialogsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                var window = (MainWindow)dependencyObject;
                var useAccentForDialogs = (bool)e.NewValue;
                window.MetroDialogOptions.ColorScheme = useAccentForDialogs ? MetroDialogColorScheme.Accented : MetroDialogColorScheme.Theme;
            }
        }

        public bool UseAccentForDialogs
        {
            get { return (bool)GetValue(UseAccentForDialogsProperty); }
            set { SetValue(UseAccentForDialogsProperty, value); }
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

            flyoutDemo.Launch();
        }

        private void LaunchIcons(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
                          {
                              FileName = "https://github.com/MahApps/MahApps.Metro.IconPacks",
                              // UseShellExecute is default to false on .NET Core while true on .NET Framework.
                              // Only this value is set to true, the url link can be opened.
                              UseShellExecute = true,
                          });
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

        private async void ShowDialogOutside(object sender, RoutedEventArgs e)
        {
            var dialog = new CustomDialog(this.MetroDialogOptions) { Content = this.Resources["CustomDialogTest"], Title = "This dialog allows arbitrary content." };
            dialog = dialog.ShowDialogExternally();

            await Task.Delay(5000);

            await dialog.RequestCloseAsync();
        }

        private async void ShowMessageDialog(object sender, RoutedEventArgs e)
        {
            // This demo runs on .Net 4.0, but we're using the Microsoft.Bcl.Async package so we have async/await support
            // The package is only used by the demo and not a dependency of the library!
            var mySettings = new MetroDialogSettings()
                             {
                                 AffirmativeButtonText = "Hi",
                                 NegativeButtonText = "Go away!",
                                 FirstAuxiliaryButtonText = "Cancel",
                                 ColorScheme = MetroDialogOptions.ColorScheme,
                                 DialogButtonFontSize = 20D
                             };

            MessageDialogResult result = await this.ShowMessageAsync("Hello!", "Welcome to the world of metro!",
                                                                     MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, mySettings);

            if (result != MessageDialogResult.FirstAuxiliary)
                await this.ShowMessageAsync("Result", "You said: " + (result == MessageDialogResult.Affirmative
                                                ? mySettings.AffirmativeButtonText
                                                : mySettings.NegativeButtonText +
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
                                 ColorScheme = MetroDialogOptions.ColorScheme
                             };

            MessageDialogResult result = await this.ShowMessageAsync("Hello!", "Welcome to the world of metro!" + string.Join(Environment.NewLine, "abc", "def", "ghi", "jkl", "mno", "pqr", "stu", "vwx", "yz"),
                                                                     MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, mySettings);

            if (result != MessageDialogResult.FirstAuxiliary)
                await this.ShowMessageAsync("Result", "You said: " + (result == MessageDialogResult.Affirmative
                                                ? mySettings.AffirmativeButtonText
                                                : mySettings.NegativeButtonText +
                                                  Environment.NewLine + Environment.NewLine + "This dialog will follow the Use Accent setting."));
        }

        private async void ShowCustomDialog(object sender, RoutedEventArgs e)
        {
            var dialog = new CustomDialog(this.MetroDialogOptions) { Content = this.Resources["CustomDialogTest"], Title = "This dialog allows arbitrary content." };

            await this.ShowMetroDialogAsync(dialog);

            var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
            textBlock.Text = "A message box will appear in 3 seconds.";

            await Task.Delay(3000);

            await this.ShowMessageAsync("Secondary dialog", "This message is shown on top of another.", MessageDialogStyle.Affirmative, new MetroDialogSettings() { OwnerCanCloseWithDialog = true, ColorScheme = this.MetroDialogOptions.ColorScheme });

            textBlock.Text = "The dialog will close in 2 seconds.";
            await Task.Delay(2000);

            await this.HideMetroDialogAsync(dialog);
        }

        private async void ShowAwaitCustomDialog(object sender, RoutedEventArgs e)
        {
            EventHandler<DialogStateChangedEventArgs> dialogManagerOnDialogOpened = null;
            dialogManagerOnDialogOpened = (o, args) =>
                {
                    DialogManager.DialogOpened -= dialogManagerOnDialogOpened;
                    Console.WriteLine("Custom Dialog opened!");
                };
            DialogManager.DialogOpened += dialogManagerOnDialogOpened;

            EventHandler<DialogStateChangedEventArgs> dialogManagerOnDialogClosed = null;
            dialogManagerOnDialogClosed = (o, args) =>
                {
                    DialogManager.DialogClosed -= dialogManagerOnDialogClosed;
                    Console.WriteLine("Custom Dialog closed!");
                };
            DialogManager.DialogClosed += dialogManagerOnDialogClosed;

            var dialog = new CustomDialog(this.MetroDialogOptions) { Content = this.Resources["CustomCloseDialogTest"], Title = "Custom Dialog which is awaitable" };

            await this.ShowMetroDialogAsync(dialog);
            await dialog.WaitUntilUnloadedAsync();
        }

        private async void CloseCustomDialog(object sender, RoutedEventArgs e)
        {
            var dialog = (sender as DependencyObject).TryFindParent<BaseMetroDialog>();

            await this.HideMetroDialogAsync(dialog);
            await this.ShowMessageAsync("Dialog gone", "The custom dialog has closed");
        }

        private async void ShowLoginDialogPasswordPreview(object sender, RoutedEventArgs e)
        {
            LoginDialogData result = await this.ShowLoginAsync("Authentication", "Enter your credentials", new LoginDialogSettings { ColorScheme = this.MetroDialogOptions.ColorScheme, InitialUsername = "MahApps", EnablePasswordPreview = true });
            if (result == null)
            {
                //User pressed cancel
            }
            else
            {
                MessageDialogResult messageResult = await this.ShowMessageAsync("Authentication Information", String.Format("Username: {0}\nPassword: {1}", result.Username, result.Password));
            }
        }

        private async void ShowLoginDialogOnlyPassword(object sender, RoutedEventArgs e)
        {
            LoginDialogData result = await this.ShowLoginAsync("Authentication", "Enter your password", new LoginDialogSettings { ColorScheme = this.MetroDialogOptions.ColorScheme, ShouldHideUsername = true });
            if (result == null)
            {
                //User pressed cancel
            }
            else
            {
                MessageDialogResult messageResult = await this.ShowMessageAsync("Authentication Information", String.Format("Password: {0}", result.Password));
            }
        }

        private async void ShowLoginDialogWithRememberCheckBox(object sender, RoutedEventArgs e)
        {
            LoginDialogData result = await this.ShowLoginAsync("Authentication", "Enter your password", new LoginDialogSettings { ColorScheme = this.MetroDialogOptions.ColorScheme, RememberCheckBoxVisibility = Visibility.Visible });
            if (result == null)
            {
                //User pressed cancel
            }
            else
            {
                MessageDialogResult messageResult = await this.ShowMessageAsync("Authentication Information", String.Format("Username: {0}\nPassword: {1}\nShouldRemember: {2}", result.Username, result.Password, result.ShouldRemember));
            }
        }

        private async void ShowProgressDialog(object sender, RoutedEventArgs e)
        {
            var mySettings = new MetroDialogSettings()
                             {
                                 NegativeButtonText = "Close now",
                                 AnimateShow = false,
                                 AnimateHide = false,
                                 ColorScheme = this.MetroDialogOptions.ColorScheme
                             };

            var controller = await this.ShowProgressAsync("Please wait...", "We are baking some cupcakes!", settings: mySettings);
            controller.SetIndeterminate();

            await Task.Delay(5000);

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

                await Task.Delay(2000);
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
            var result = await this.ShowInputAsync("Hello!", "What is your name?");

            if (result == null) //user pressed cancel
                return;

            await this.ShowMessageAsync("Hello", "Hello " + result + "!");
        }

        private async void ShowInputDialogCustomButtonSizes(object sender, RoutedEventArgs e)
        {
            var settings = new MetroDialogSettings
                           {
                               DialogButtonFontSize = 30D
                           };
            var result = await this.ShowInputAsync("Hello!", "What is your name?", settings);

            if (result == null) //user pressed cancel
                return;

            await this.ShowMessageAsync("Hello", "Hello " + result + "!");
        }

        private async void ShowLoginDialog(object sender, RoutedEventArgs e)
        {
            LoginDialogData result = await this.ShowLoginAsync("Authentication", "Enter your credentials", new LoginDialogSettings { ColorScheme = this.MetroDialogOptions.ColorScheme, InitialUsername = "MahApps" });
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
            //navWin.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Clean/MetroWindow.xaml", UriKind.Absolute) });
            //navWin.SetResourceReference(StyleProperty, "MahApps.Styles.MetroWindow.Clean");

            navWin.Show();
            navWin.Navigate(new Navigation.HomePage());
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e.Cancel)
            {
                return;
            }

            if (_viewModel.QuitConfirmationEnabled
                && _shutdown == false)
            {
                e.Cancel = true;

                // We have to delay the execution through BeginInvoke to prevent potential re-entrancy
                Dispatcher.BeginInvoke(new Action(async () => await this.ConfirmShutdown()));
            }
            else
            {
                flyoutDemo?.Dispose();

                _viewModel.Dispose();
            }
        }

        private async Task ConfirmShutdown()
        {
            var mySettings = new MetroDialogSettings
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
            {
                Application.Current.Shutdown();
            }
        }

        private MetroWindow testWindow;

        private MetroWindow GetTestWindow()
        {
            if (testWindow != null)
            {
                testWindow.Close();
            }

            testWindow = new MetroWindow()
                         {
                             Owner = this,
                             WindowStartupLocation = WindowStartupLocation.CenterOwner,
                             Title = "Another Test...",
                             Width = 500,
                             Height = 300
                         };
            testWindow.Closed += (o, args) => testWindow = null;
            return testWindow;
        }

        private void MenuWindowWithoutBorderOnClick(object sender, RoutedEventArgs e)
        {
            var w = this.GetTestWindow();
            w.Content = new TextBlock() { Text = "MetroWindow without Border", FontSize = 28, FontWeight = FontWeights.Light, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            w.BorderThickness = new Thickness(0);
            w.Show();
        }

        private void MenuWindowWithBorderOnClick(object sender, RoutedEventArgs e)
        {
            var w = this.GetTestWindow();
            w.Content = new TextBlock() { Text = "MetroWindow with Border", FontSize = 28, FontWeight = FontWeights.Light, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            w.Show();
        }

        private void MenuWindowWithGlowOnClick(object sender, RoutedEventArgs e)
        {
            var w = this.GetTestWindow();
            w.Content = new Button() { Content = "MetroWindow with Glow", ToolTip = "And test tool tip", FontSize = 28, FontWeight = FontWeights.Light, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            w.BorderThickness = new Thickness(1);
            w.BorderBrush = null;
            w.SetResourceReference(MetroWindow.GlowBrushProperty, "MahApps.Brushes.Accent");
            w.Show();
        }

        private void MenuWindowWithShadowOnClick(object sender, RoutedEventArgs e)
        {
            var w = this.GetTestWindow();
            w.Content = new TextBlock() { Text = "Window with drop shadow", FontSize = 28, FontWeight = FontWeights.Light, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            w.BorderThickness = new Thickness(0);
            w.BorderBrush = null;
            w.GlowBrush = Brushes.Black;
            w.Show();
        }

        private void ShowInputDialogOutside(object sender, RoutedEventArgs e)
        {
            var result = this.ShowModalInputExternal("Hello!", "What is your name?");

            if (result == null) //user pressed cancel
                return;

            this.ShowModalMessageExternal("Hello", "Hello " + result + "!");
        }

        private void ShowLoginDialogOutside(object sender, RoutedEventArgs e)
        {
            LoginDialogData result = this.ShowModalLoginExternal("Authentication", "Enter your credentials", new LoginDialogSettings { ColorScheme = this.MetroDialogOptions.ColorScheme, InitialUsername = "MahApps", EnablePasswordPreview = true });
            if (result == null)
            {
                //User pressed cancel
            }
            else
            {
                MessageDialogResult messageResult = this.ShowModalMessageExternal("Authentication Information", String.Format("Username: {0}\nPassword: {1}", result.Username, result.Password));
            }
        }

        private void ShowMessageDialogOutside(object sender, RoutedEventArgs e)
        {
            var mySettings = new MetroDialogSettings()
                             {
                                 AffirmativeButtonText = "Hi",
                                 NegativeButtonText = "Go away!",
                                 FirstAuxiliaryButtonText = "Cancel",
                                 ColorScheme = MetroDialogOptions.ColorScheme
                             };

            MessageDialogResult result = this.ShowModalMessageExternal("Hello!", "Welcome to the world of metro!",
                                                                       MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, mySettings);

            if (result != MessageDialogResult.FirstAuxiliary)
                this.ShowModalMessageExternal("Result", "You said: " + (result == MessageDialogResult.Affirmative
                                                  ? mySettings.AffirmativeButtonText
                                                  : mySettings.NegativeButtonText +
                                                    Environment.NewLine + Environment.NewLine + "This dialog will follow the Use Accent setting."));
        }
    }
}