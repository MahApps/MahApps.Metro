// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MahApps.Metro.Controls;
using MahApps.Metro.Gallery.Core;
using MahApps.Metro.Gallery.Pages.ContentDialogExamples;
using ShowMeTheXAML;

namespace MahApps.Metro.Gallery.Pages
{
    /// <summary>
    /// Interaction logic for ContentDialogPage.xaml
    /// </summary>
    public partial class ContentDialogPage : UserControl
    {
        /// <summary>
        /// The dialogs by card and look, ThreeButtons0 the Metro one, ThreeButtons1 the Windows 10 one, ThreeButtons2 the WinUI one.
        /// </summary>
        private readonly Dictionary<string, ContentDialog> dialogs = new();

        public ContentDialogPage()
        {
            this.InitializeComponent();

            this.Loaded += this.OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.dialogs.Count > 0)
            {
                return;
            }

            var texts = new[]
                        {
                            ContentDialog.TitleProperty,
                            ContentDialog.PrimaryButtonTextProperty,
                            ContentDialog.SecondaryButtonTextProperty,
                            ContentDialog.CloseButtonTextProperty,
                            ContentDialog.DefaultButtonProperty
                        };
            var enabled = new[] { ContentDialog.IsPrimaryButtonEnabledProperty, ContentDialog.IsSecondaryButtonEnabledProperty };
            var titleBar = new[] { ChildWindow.ShowTitleBarProperty, ChildWindow.AllowMoveProperty, ChildWindow.ShowTitleBarCloseButtonProperty };

            var threeButtons = this.TakeDialogs("ThreeButtons", "ThreeButtonsDisplay", texts.Concat(enabled));
            this.TakeDialogs("Result", "ResultDisplay", texts);
            this.TakeDialogs("Input", "InputDisplay", texts);
            var withTitleBar = this.TakeDialogs("TitleBar", "TitleBarDisplay", texts.Concat(titleBar));

            this.ThreeButtonsExample.Watch(threeButtons,
                                           ContentDialog.TitleProperty,
                                           ContentDialog.PrimaryButtonTextProperty,
                                           ContentDialog.SecondaryButtonTextProperty,
                                           ContentDialog.CloseButtonTextProperty,
                                           ContentDialog.DefaultButtonProperty,
                                           ContentDialog.IsPrimaryButtonEnabledProperty,
                                           ContentDialog.IsSecondaryButtonEnabledProperty);

            this.TitleBarExample.Watch(withTitleBar,
                                       ChildWindow.ShowTitleBarProperty,
                                       ChildWindow.AllowMoveProperty,
                                       ChildWindow.ShowTitleBarCloseButtonProperty);
        }

        /// <summary>
        /// Takes the three dialogs out of the display, binds the Windows 10 and the WinUI one to the Metro one
        /// for the given properties, and returns the Metro one. They are samples, so they wear the default set,
        /// not the one of the gallery, apart from the style each of the two copies names.
        /// </summary>
        private ContentDialog TakeDialogs(string name, string displayKey, IEnumerable<DependencyProperty> followed)
        {
            var display = (XamlDisplay)this.FindResource(displayKey);
            var panel = (Panel)display.Content;
            var three = panel.Children.OfType<ContentDialog>().ToList();
            panel.Children.Clear();

            var first = three[0];
            foreach (var property in followed)
            {
                foreach (var copy in three.Skip(1))
                {
                    BindingOperations.SetBinding(copy, property, new Binding(property.Name) { Source = first });
                }
            }

            // the Metro one wears the default set; the two copies name the style of their look on
            // the dialog and on whatever control is in it, the way the other pages show a look
            DefaultStyleSet.Apply(three[0]);

            for (var i = 0; i < three.Count; i++)
            {
                this.dialogs[name + i] = three[i];
            }

            return first;
        }

        private async void OnShow(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement { Tag: string name }
                && this.dialogs.TryGetValue(name, out var dialog)
                && dialog is { IsOpen: false, Parent: null }
                && System.Windows.Window.GetWindow(this) is MetroWindow window)
            {
                var result = await dialog.ShowAsync(window);
                if (name.StartsWith("Result", StringComparison.Ordinal))
                {
                    this.ResultText.Text = $"The dialog handed back: {result}";
                }
            }
        }

        private void OnAnswerTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox box && box.TryFindParent<ContentDialog>() is { } dialog)
            {
                dialog.SetCurrentValue(ContentDialog.IsPrimaryButtonEnabledProperty, !string.IsNullOrWhiteSpace(box.Text));
            }
        }

        private MetroWindow Window => (MetroWindow)System.Windows.Window.GetWindow(this)!;

        #region Look
        // The style names the look of the dialog. The controls in the content take the set the
        // application merged, which is the WinUI one in this gallery, so the other two looks put
        // their own set into the resources of the dialog. A set is large: load it once and share it.
        private static ResourceDictionary? win10Set;

        private static ContentDialog InLook(ContentDialog dialog, object sender)
        {
            switch (((FrameworkElement)sender).Tag)
            {
                case "Win10":
                    dialog.Style = (Style)Application.Current.FindResource("MahApps.Styles.ContentDialog.Win10");
                    win10Set ??= new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Win10/Controls.xaml") };
                    dialog.Resources.MergedDictionaries.Add(win10Set);
                    break;
                case "WinUI":
                    dialog.Style = (Style)Application.Current.FindResource("MahApps.Styles.ContentDialog.WinUI");
                    break;
                default:
                    DefaultStyleSet.Apply(dialog);
                    break;
            }

            return dialog;
        }
        #endregion

        #region Message
        private async void OnCodeMessage(object sender, RoutedEventArgs e)
        {
            var dialog = InLook(new ContentDialog
                                {
                                    Title = "Deep Thought",
                                    Content = "Come back in seven and a half million years.",
                                    CloseButtonText = "OK"
                                },
                                sender);

            await dialog.ShowAsync(this.Window);

            this.CodeMessageAnswer.Text = "Read";
        }
        #endregion

        #region Login
        private async void OnCodeLogin(object sender, RoutedEventArgs e)
        {
            var model = new LoginViewModel();
            var content = new LoginContent { DataContext = model };
            var dialog = InLook(new ContentDialog
                                {
                                    Title = "Sign in",
                                    Content = content,
                                    PrimaryButtonText = "Sign in",
                                    CloseButtonText = "Cancel",
                                    DefaultButton = ContentDialogButton.Primary
                                },
                                sender);
            dialog.SetBinding(ContentDialog.IsPrimaryButtonEnabledProperty, new Binding(nameof(LoginViewModel.CanSignIn)) { Source = model });

            var result = await dialog.ShowAsync(this.Window);

            this.CodeLoginAnswer.Text = result == ContentDialogResult.Primary
                                            ? $"Signed in as {model.UserName} with a password of {content.Password.Length} characters, remembered: {model.RememberMe}"
                                            : "Cancelled";
        }
        #endregion

        #region Input
        private async void OnCodeInput(object sender, RoutedEventArgs e)
        {
            var model = new InputViewModel { Question = "What is the answer to everything?" };
            var dialog = InLook(new ContentDialog
                                {
                                    Title = "Deep Thought",
                                    Content = new InputContent { DataContext = model },
                                    PrimaryButtonText = "OK",
                                    CloseButtonText = "Cancel",
                                    DefaultButton = ContentDialogButton.Primary
                                },
                                sender);
            dialog.SetBinding(ContentDialog.IsPrimaryButtonEnabledProperty, new Binding(nameof(InputViewModel.HasText)) { Source = model });

            var result = await dialog.ShowAsync(this.Window);

            this.CodeInputAnswer.Text = result == ContentDialogResult.Primary ? $"The answer is {model.Text}" : "Cancelled";
        }
        #endregion

        #region Progress
        private async void OnCodeProgress(object sender, RoutedEventArgs e)
        {
            var model = new ProgressViewModel();
            using var cancel = new CancellationTokenSource();
            var dialog = InLook(new ContentDialog
                                {
                                    Title = "Please wait...",
                                    Content = new ProgressContent { DataContext = model },
                                    CloseButtonText = "Cancel"
                                },
                                sender);

            // the close button, Escape and the X all come through here; Hide does not
            dialog.CloseButtonClick += (_, _) => cancel.Cancel();

            var showing = dialog.ShowAsync(this.Window);

            try
            {
                for (var cupcake = 1; cupcake <= 12; cupcake++)
                {
                    model.Message = $"Baking cupcake {cupcake} of 12...";
                    model.Value = cupcake / 12.0;
                    await Task.Delay(250, cancel.Token);
                }

                dialog.Hide();
            }
            catch (TaskCanceledException)
            {
                // the dialog closed itself through its button
            }

            await showing;

            this.CodeProgressAnswer.Text = cancel.IsCancellationRequested ? "You stopped baking" : "Twelve cupcakes";
        }
        #endregion

        #region Selection
        private async void OnCodeSelection(object sender, RoutedEventArgs e)
        {
            var model = new SelectionViewModel { People = SamplePeople.All };
            var dialog = InLook(new ContentDialog
                                {
                                    Title = "Who goes first?",
                                    Content = new SelectionContent { DataContext = model },
                                    PrimaryButtonText = "Choose",
                                    CloseButtonText = "Cancel",
                                    DefaultButton = ContentDialogButton.Primary
                                },
                                sender);
            dialog.SetBinding(ContentDialog.IsPrimaryButtonEnabledProperty, new Binding(nameof(SelectionViewModel.HasSelection)) { Source = model });

            var result = await dialog.ShowAsync(this.Window);

            this.CodeSelectionAnswer.Text = result == ContentDialogResult.Primary && model.Selected is { } person ? $"{person.Name} goes first" : "Cancelled";
        }
        #endregion
    }
}
