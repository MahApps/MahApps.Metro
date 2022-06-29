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
        private bool shutdown;
        private readonly MainWindowViewModel viewModel;
        private FlyoutDemo? flyoutDemo;

        public MainWindow()
        {
            this.viewModel = new MainWindowViewModel(DialogCoordinator.Instance);
            this.DataContext = this.viewModel;

            this.InitializeComponent();

            DialogManager.DialogOpened += (_, args) => Debug.WriteLine($"Dialog {args.Dialog} - '{args.Dialog.Title}' opened.");
            DialogManager.DialogClosed += (_, args) => Debug.WriteLine($"Dialog {args.Dialog} - '{args.Dialog.Title}' closed.");
        }

        #region DependencyProperties

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
            get => (bool)this.GetValue(ToggleFullScreenProperty);
            set => this.SetValue(ToggleFullScreenProperty, value);
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

                if (useAccentForDialogs == true && window.MetroDialogOptions!.ColorScheme == MetroDialogColorScheme.Inverted)
                {
                    window.SetValue(UseInvertForDialogsProperty, false);
                }

                window.MetroDialogOptions!.ColorScheme = useAccentForDialogs ? MetroDialogColorScheme.Accented : MetroDialogColorScheme.Theme;
            }
        }

        public bool UseAccentForDialogs
        {
            get => (bool)this.GetValue(UseAccentForDialogsProperty);
            set => this.SetValue(UseAccentForDialogsProperty, value);
        }

        public static readonly DependencyProperty UseInvertForDialogsProperty =
            DependencyProperty.Register(nameof(UseInvertForDialogs),
                                        typeof(bool),
                                        typeof(MainWindow),
                                        new PropertyMetadata(default(bool), OnUseInvertForDialogsChanged));

        private static void OnUseInvertForDialogsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                var window = (MainWindow)dependencyObject;
                var useInvertForDialogs = (bool)e.NewValue;

                if (useInvertForDialogs == true && window.MetroDialogOptions!.ColorScheme == MetroDialogColorScheme.Accented)
                {
                    window.SetValue(UseAccentForDialogsProperty, false);
                }

                window.MetroDialogOptions!.ColorScheme = useInvertForDialogs ? MetroDialogColorScheme.Inverted : MetroDialogColorScheme.Theme;
            }
        }

        public bool UseInvertForDialogs
        {
            get => (bool)this.GetValue(UseInvertForDialogsProperty);
            set => this.SetValue(UseInvertForDialogsProperty, value);
        }

        public static readonly DependencyProperty ShowIconOnDialogsProperty =
            DependencyProperty.Register(nameof(ShowIconOnDialogs),
                                        typeof(bool),
                                        typeof(MainWindow),
                                        new PropertyMetadata(default(bool), OnShowIconOnDialogsChanged));

        private static void OnShowIconOnDialogsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != e.NewValue)
            {
                var window = (MainWindow)dependencyObject;
                var showIconOnDialogs = (bool)e.NewValue;
                window.MetroDialogOptions!.Icon = showIconOnDialogs
                    ? new MahApps.Metro.IconPacks.PackIconMaterial()
                      {
                          Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Duck,
                          Width = 75,
                          Height = 75,
                          Foreground = Brushes.Goldenrod,
                      }
                    : null;
            }
        }

        public bool ShowIconOnDialogs
        {
            get => (bool)this.GetValue(ShowIconOnDialogsProperty);
            set => this.SetValue(ShowIconOnDialogsProperty, value);
        }

        #endregion DependencyProperties

        private void LaunchMahAppsOnGitHub(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/MahApps/MahApps.Metro");
        }

        private void LaunchSizeToContentDemo(object sender, RoutedEventArgs e)
        {
            new SizeToContentDemo { Owner = this }.Show();
        }

        private void LaunchVisualStudioDemo(object sender, RoutedEventArgs e)
        {
            new VSDemo().Show();
        }

        private void LaunchFlyoutDemo(object sender, RoutedEventArgs e)
        {
            if (this.flyoutDemo is null)
            {
                this.flyoutDemo = new FlyoutDemo();
                this.flyoutDemo.Closed += (o, args) => this.flyoutDemo = null;
            }

            this.flyoutDemo.Launch();
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

        private Window? cleanWindowDemo;

        private void LauchCleanDemo(object sender, RoutedEventArgs e)
        {
            if (this.cleanWindowDemo == null)
            {
                this.cleanWindowDemo = new CleanWindowDemo();
                this.cleanWindowDemo.Closed += (o, args) => this.cleanWindowDemo = null;
            }

            if (this.cleanWindowDemo.IsVisible)
            {
                this.cleanWindowDemo.Hide();
            }
            else
            {
                this.cleanWindowDemo.Show();
            }
        }

        #region Show Dialogs

        private async void ShowMessageDialog(object sender, RoutedEventArgs e)
        {
            // This demo runs on .Net 4.0, but we're using the Microsoft.Bcl.Async package so we have async/await support
            // The package is only used by the demo and not a dependency of the library!
            var settings = new MetroDialogSettings(this.MetroDialogOptions)
                           {
                               AffirmativeButtonText = "Hi",
                               NegativeButtonText = "Go away!",
                               FirstAuxiliaryButtonText = "Cancel",
                               DialogButtonFontSize = 20D
                           };

            MessageDialogResult result = await this.ShowMessageAsync("Hello!",
                                                                     "Welcome to the world of metro!",
                                                                     MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
                                                                     settings);

            await this.ShowMessageAsync("Result", $"You said ({result}): {(result == MessageDialogResult.Affirmative ? settings.AffirmativeButtonText : result == MessageDialogResult.FirstAuxiliary ? settings.FirstAuxiliaryButtonText : settings.NegativeButtonText)}");
        }

        private async void ShowLimitedMessageDialog(object sender, RoutedEventArgs e)
        {
            var settings = new MetroDialogSettings(this.MetroDialogOptions)
                           {
                               AffirmativeButtonText = "Hi",
                               NegativeButtonText = "Go away!",
                               FirstAuxiliaryButtonText = "Cancel",
                               MaximumBodyHeight = 100
                           };

            MessageDialogResult result = await this.ShowMessageAsync("Hello!",
                                                                     "Welcome to the world of metro!" + string.Join(Environment.NewLine, "abc", "def", "ghi", "jkl", "mno", "pqr", "stu", "vwx", "yz"),
                                                                     MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary,
                                                                     settings);

            await this.ShowMessageAsync("Result", $"You said ({result}): {(result == MessageDialogResult.Affirmative ? settings.AffirmativeButtonText : result == MessageDialogResult.FirstAuxiliary ? settings.FirstAuxiliaryButtonText : settings.NegativeButtonText)}");
        }

        private async void ShowCustomDialog(object sender, RoutedEventArgs e)
        {
            var dialog = new CustomDialog(this.MetroDialogOptions) { Content = this.Resources["CustomDialogTest"], Title = "This dialog allows arbitrary content." };

            await this.ShowMetroDialogAsync(dialog);

            var textBlock = dialog.FindChild<TextBlock>("MessageTextBlock");
            textBlock!.Text = "A message box will appear in 3 seconds.";

            await Task.Delay(3000);

            await this.ShowMessageAsync("Secondary dialog", "This message is shown on top of another.", MessageDialogStyle.Affirmative, new MetroDialogSettings(this.MetroDialogOptions) { OwnerCanCloseWithDialog = true });

            textBlock.Text = "The dialog will close in 2 seconds.";
            await Task.Delay(2000);

            await this.HideMetroDialogAsync(dialog);
        }

        private async void ShowAwaitCustomDialog(object sender, RoutedEventArgs e)
        {
            var tcs = new TaskCompletionSource<bool>();
            var dialog = new CustomDialog(this.MetroDialogOptions) { Content = this.Resources["CustomCloseDialogTest"], Title = "Custom Dialog which is awaitable" };
            dialog.Tag = tcs;
            await this.ShowMetroDialogAsync(dialog);
            await tcs.Task;
            await this.HideMetroDialogAsync(dialog);
            await this.ShowMessageAsync("Dialog gone", "The custom dialog is now closed.");
        }

        private async void ShowSecondCustomDialog(object sender, RoutedEventArgs e)
        {
            await this.ShowMessageAsync("Second Dialog", "The first custom dialog is now behind this dialog.");
        }

        private void CloseCustomDialog(object sender, RoutedEventArgs e)
        {
            var dialog = ((DependencyObject)sender).TryFindParent<BaseMetroDialog>()!;
            var tcs = dialog.Tag as TaskCompletionSource<bool>;
            tcs?.TrySetResult(true);
        }

        private async void ShowLoginDialogPasswordPreview(object sender, RoutedEventArgs e)
        {
            var result = await this.ShowLoginAsync("Authentication", "Enter your credentials", new LoginDialogSettings(this.MetroDialogOptions) { InitialUsername = "MahApps", EnablePasswordPreview = true });
            if (result == null)
            {
                //User pressed cancel
            }
            else
            {
                await this.ShowMessageAsync("Authentication Information", $"Username: {result.Username}\nPassword: {result.Password}");
            }
        }

        private async void ShowLoginDialogOnlyPassword(object sender, RoutedEventArgs e)
        {
            var result = await this.ShowLoginAsync("Authentication", "Enter your password", new LoginDialogSettings(this.MetroDialogOptions) { ShouldHideUsername = true });
            if (result == null)
            {
                //User pressed cancel
            }
            else
            {
                await this.ShowMessageAsync("Authentication Information", $"Password: {result.Password}");
            }
        }

        private async void ShowLoginDialogWithRememberCheckBox(object sender, RoutedEventArgs e)
        {
            var result = await this.ShowLoginAsync("Authentication", "Enter your password", new LoginDialogSettings(this.MetroDialogOptions) { RememberCheckBoxVisibility = Visibility.Visible });
            if (result == null)
            {
                //User pressed cancel
            }
            else
            {
                await this.ShowMessageAsync("Authentication Information", $"Username: {result.Username}\nPassword: {result.Password}\nShouldRemember: {result.ShouldRemember}");
            }
        }

        private async void ShowProgressDialog(object sender, RoutedEventArgs e)
        {
            var settings = new MetroDialogSettings(this.MetroDialogOptions)
                           {
                               NegativeButtonText = "Close now",
                               AnimateShow = false,
                               AnimateHide = false
                           };

            var controller = await this.ShowProgressAsync("Please wait...", "We are baking now some cupcakes!", settings: settings);

            controller.SetIndeterminate();

            await Task.Delay(3000);

            controller.SetCancelable(true);

            double i = 0.0;
            while (i < 6.0)
            {
                if (controller.IsCanceled)
                {
                    break;
                }

                var val = (i / 100.0) * 20.0;
                controller.SetProgress(val);
                controller.SetMessage("Baking cupcake: " + i + "...");

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
            string? result = await this.ShowInputAsync("Hello!", "What is your name?");

            if (string.IsNullOrWhiteSpace(result)) //user pressed cancel
            {
                return;
            }

            await this.ShowMessageAsync("Hello", "Hello " + result + "!");
        }

        private async void ShowInputDialogCustomButtonSizes(object sender, RoutedEventArgs e)
        {
            var settings = new MetroDialogSettings(this.MetroDialogOptions)
                           {
                               DialogButtonFontSize = 24D
                           };

            var result = await this.ShowInputAsync("Hello!", "What is your name?", settings);

            if (result == null) //user pressed cancel
            {
                return;
            }

            await this.ShowMessageAsync("Hello", "Hello " + result + "!");
        }

        private async void ShowLoginDialog(object sender, RoutedEventArgs e)
        {
            var result = await this.ShowLoginAsync("Authentication", "Enter your credentials", new LoginDialogSettings(this.MetroDialogOptions) { InitialUsername = "MahApps" });
            if (result == null)
            {
                //User pressed cancel
            }
            else
            {
                await this.ShowMessageAsync("Authentication Information", $"Username: {result.Username}\nPassword: {result.Password}");
            }
        }

        #endregion

        #region Show Dialog Outside

        private void ShowInputDialogOutside(object sender, RoutedEventArgs e)
        {
            var result = this.ShowModalInputExternal("Hello!", "What is your name?");

            if (result == null) //user pressed cancel
            {
                return;
            }

            this.ShowModalMessageExternal("Hello", "Hello " + result + "!");
        }

        private void ShowLoginDialogOutside(object sender, RoutedEventArgs e)
        {
            var result = this.ShowModalLoginExternal("Authentication", "Enter your credentials", new LoginDialogSettings(this.MetroDialogOptions) { InitialUsername = "MahApps", EnablePasswordPreview = true });
            if (result == null)
            {
                //User pressed cancel
            }
            else
            {
                MessageDialogResult messageResult = this.ShowModalMessageExternal("Authentication Information", $"Username: {result.Username}\nPassword: {result.Password}");
            }
        }

        private void ShowMessageDialogOutside(object sender, RoutedEventArgs e)
        {
            var settings = new MetroDialogSettings(this.MetroDialogOptions)
                           {
                               AffirmativeButtonText = "Hi",
                               NegativeButtonText = "Go away!",
                               FirstAuxiliaryButtonText = "Cancel",
                               ColorScheme = this.MetroDialogOptions!.ColorScheme
                           };

            MessageDialogResult result = this.ShowModalMessageExternal("Hello!", "Welcome to the world of metro!",
                                                                       MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);

            if (result != MessageDialogResult.FirstAuxiliary)
            {
                this.ShowModalMessageExternal("Result", "You said: " + (result == MessageDialogResult.Affirmative
                                                  ? settings.AffirmativeButtonText
                                                  : settings.NegativeButtonText +
                                                    Environment.NewLine + Environment.NewLine + "This dialog will follow the Use Accent setting."));
            }
        }

        private async void ShowDialogOutside(object sender, RoutedEventArgs e)
        {
            var dialog = new CustomDialog(this.MetroDialogOptions) { Content = this.Resources["CustomDialogTest"], Title = "This dialog allows arbitrary content." };
            dialog = dialog.ShowDialogExternally();

            await Task.Delay(5000);

            await dialog.RequestCloseAsync();
        }

        #endregion

        private void InteropDemo(object sender, RoutedEventArgs e)
        {
            new InteropDemo().Show();
        }

        private void LaunchNavigationDemo(object sender, RoutedEventArgs e)
        {
            var navWin = new MetroNavigationWindow
                         {
                             Title = "Navigation Demo",
                             Width = 800,
                             Height = 600,
                             ShowHomeButton = true
                         };

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

            if (this.viewModel.QuitConfirmationEnabled
                && this.shutdown == false)
            {
                e.Cancel = true;

                // We have to delay the execution through BeginInvoke to prevent potential re-entrancy
                this.Dispatcher.BeginInvoke(new Action(async () => await this.ConfirmShutdown()));
            }
            else
            {
                this.flyoutDemo?.Dispose();

                this.viewModel.Dispose();
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
                                                     MessageDialogStyle.AffirmativeAndNegative,
                                                     mySettings);

            this.shutdown = result == MessageDialogResult.Affirmative;

            if (this.shutdown)
            {
                Application.Current.Shutdown();
            }
        }

        private MetroWindow? testWindow;

        private MetroWindow GetTestWindow()
        {
            if (this.testWindow != null)
            {
                this.testWindow.Close();
            }

            this.testWindow = new MetroWindow
                              {
                                  Owner = this,
                                  WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                  Title = "Another Test...",
                                  Width = 500,
                                  Height = 300
                              };
            this.testWindow.Closed += (o, args) => this.testWindow = null;
            return this.testWindow;
        }

        private void MenuWindowWithoutBorderOnClick(object sender, RoutedEventArgs e)
        {
            var w = this.GetTestWindow();
            w.Content = new TextBlock { Text = "MetroWindow without Border", FontSize = 28, FontWeight = FontWeights.Light, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            w.BorderThickness = new Thickness(0);
            w.Show();
        }

        private void MenuWindowWithBorderOnClick(object sender, RoutedEventArgs e)
        {
            var w = this.GetTestWindow();
            w.Content = new TextBlock { Text = "MetroWindow with Border", FontSize = 28, FontWeight = FontWeights.Light, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            w.GlowColor = null;
            w.NonActiveGlowColor = null;
            w.Show();
        }

        private void MenuWindowWithGlowOnClick(object sender, RoutedEventArgs e)
        {
            var w = this.GetTestWindow();
            w.Content = new Button { Content = "MetroWindow with Glow", ToolTip = "And test tool tip", FontSize = 28, FontWeight = FontWeights.Light, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            w.BorderThickness = new Thickness(1);
            w.BorderBrush = null;
            w.SetResourceReference(GlowColorProperty, "MahApps.Colors.Accent");
            w.Show();
        }

        private void MenuWindowWithShadowOnClick(object sender, RoutedEventArgs e)
        {
            var w = this.GetTestWindow();
            w.Content = new TextBlock { Text = "Window with drop shadow", FontSize = 28, FontWeight = FontWeights.Light, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            w.BorderThickness = new Thickness(0);
            w.BorderBrush = null;
            w.GlowColor = Colors.Black;
            w.Show();
        }
    }
}