// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Gallery.Core;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// The async dialogs of a MetroWindow, each in the Metro, the Windows 10 and the WinUI look.
    /// The cards show the regions of this file, so what runs is what they show.
    /// </summary>
    public partial class DialogsPage : UserControl
    {
        public DialogsPage()
        {
            this.InitializeComponent();
        }

        private MetroWindow Window => (MetroWindow)System.Windows.Window.GetWindow(this)!;

        #region Look
        // The look comes with the settings. Without a dictionary a dialog wears whatever the
        // application merged, and this gallery merged the WinUI set.
        private static ResourceDictionary Look(object sender)
        {
            return ((FrameworkElement)sender).Tag switch
            {
                "Win10" => new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Win10/Dialogs.xaml") },
                "WinUI" => new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/WinUI/Dialogs.xaml") },
                _ => DefaultStyleSet.Dictionary
            };
        }
        #endregion

        #region Message
        private async void OnMessage(object sender, RoutedEventArgs e)
        {
            var settings = new MetroDialogSettings
                           {
                               AffirmativeButtonText = "Hi",
                               NegativeButtonText = "Go away!",
                               FirstAuxiliaryButtonText = "Cancel",
                               CustomResourceDictionary = Look(sender)
                           };

            var result = await this.Window.ShowMessageAsync("Hello!", "Welcome to the world of metro!", MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, settings);

            this.MessageAnswer.Text = $"The dialog handed back: {result}";
        }
        #endregion

        #region Input
        private async void OnInput(object sender, RoutedEventArgs e)
        {
            var answer = await this.Window.ShowInputAsync("Deep Thought", "What is the answer to everything?", new MetroDialogSettings { CustomResourceDictionary = Look(sender) });

            this.InputAnswer.Text = answer is null ? "Cancelled" : $"The answer is {answer}";
        }
        #endregion

        #region Login
        private async void OnLogin(object sender, RoutedEventArgs e)
        {
            var settings = new LoginDialogSettings
                           {
                               InitialUsername = "MahApps",
                               RememberCheckBoxVisibility = Visibility.Visible,
                               CustomResourceDictionary = Look(sender)
                           };

            var login = await this.Window.ShowLoginAsync("Authentication", "Enter your credentials", settings);

            this.LoginAnswer.Text = login is null ? "Cancelled" : $"Signed in as {login.Username}, remembered: {login.ShouldRemember}";
        }
        #endregion

        #region Progress
        private async void OnProgress(object sender, RoutedEventArgs e)
        {
            var controller = await this.Window.ShowProgressAsync("Please wait...", "We are baking now some cupcakes!", true, new MetroDialogSettings { CustomResourceDictionary = Look(sender) });

            for (var cupcake = 1; cupcake <= 12 && !controller.IsCanceled; cupcake++)
            {
                controller.SetProgress(cupcake / 12.0);
                controller.SetMessage($"Baking cupcake {cupcake} of 12...");
                await Task.Delay(250);
            }

            await controller.CloseAsync();

            this.ProgressAnswer.Text = controller.IsCanceled ? "You stopped baking" : "Twelve cupcakes";
        }
        #endregion

        #region Custom
        private async void OnCustom(object sender, RoutedEventArgs e)
        {
            var dialog = new CustomDialog(this.Window, new MetroDialogSettings { CustomResourceDictionary = Look(sender) }) { Title = "Deep Thought" };

            var close = new Button { Content = "Close", HorizontalAlignment = HorizontalAlignment.Right, Margin = new Thickness(0, 16, 0, 0), MinWidth = 120 };

            // the content is the application's, so it wears the set the application merged; a
            // button that should match the Windows 10 card says so itself
            if (((FrameworkElement)sender).Tag is "Win10")
            {
                close.SetResourceReference(StyleProperty, "MahApps.Styles.Button.Win10");
            }

            close.Click += async (_, _) => await this.Window.HideMetroDialogAsync(dialog);
            dialog.Content = new StackPanel
                             {
                                 Children =
                                 {
                                     new TextBlock { Text = "Anything can go in here. This one is a line of text and a button.", TextWrapping = TextWrapping.Wrap },
                                     close
                                 }
                             };

            await this.Window.ShowMetroDialogAsync(dialog);
            await dialog.WaitUntilUnloadedAsync();

            this.CustomAnswer.Text = "Closed";
        }
        #endregion
    }
}
