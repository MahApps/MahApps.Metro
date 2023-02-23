// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ControlzEx.Theming;
using MahApps.Metro.ValueBoxes;

namespace MahApps.Metro.Controls.Dialogs
{
    public static class DialogManager
    {
        /// <summary>
        /// Creates a LoginDialog inside of the current window.
        /// </summary>
        /// <param name="window">The window that is the parent of the dialog.</param>
        /// <param name="title">The title of the LoginDialog.</param>
        /// <param name="message">The message contained within the LoginDialog.</param>
        /// <param name="settings">Optional settings that override the global metro dialog settings.</param>
        /// <returns>The text that was entered or null (Nothing in Visual Basic) if the user cancelled the operation.</returns>
        public static async Task<LoginDialogData?> ShowLoginAsync(this MetroWindow window, object title, string message, LoginDialogSettings? settings = null)
        {
            window.Dispatcher.VerifyAccess();

            settings ??= new LoginDialogSettings();

            await HandleOverlayOnShowAsync(settings, window);

            // create the dialog control
            LoginDialog dialog = new LoginDialog(window, settings)
                                 {
                                     Title = title,
                                     Message = message
                                 };

            SetDialogFontSizes(settings, dialog);

            dialog.SizeChangedHandler = SetupAndAddDialog(window, dialog);

            await dialog.WaitForLoadAsync();

            DialogOpened?.Invoke(window, new DialogStateChangedEventArgs(dialog));

            var result = await dialog.WaitForButtonPressAsync();

            // once a button as been clicked, begin removing the dialog.
            dialog.FireOnClose();

            DialogClosed?.Invoke(window, new DialogStateChangedEventArgs(dialog));

            await dialog.WaitForCloseAsync();

            window.SizeChanged -= dialog.SizeChangedHandler;
            window.RemoveDialog(dialog);

            await HandleOverlayOnHideAsync(settings, window);

            return result;
        }

        /// <summary>
        /// Creates a InputDialog inside of the current window.
        /// </summary>
        /// <param name="window">The MetroWindow</param>
        /// <param name="title">The title of the MessageDialog.</param>
        /// <param name="message">The message contained within the MessageDialog.</param>
        /// <param name="settings">Optional settings that override the global metro dialog settings.</param>
        /// <returns>The text that was entered or null (Nothing in Visual Basic) if the user cancelled the operation.</returns>
        public static async Task<string?> ShowInputAsync(this MetroWindow window, object title, string message, MetroDialogSettings? settings = null)
        {
            window.Dispatcher.VerifyAccess();

            settings ??= window.MetroDialogOptions;

            await HandleOverlayOnShowAsync(settings, window);

            // create the dialog control
            var dialog = new InputDialog(window, settings)
                         {
                             Title = title,
                             Message = message,
                             Input = settings?.DefaultText,
                         };

            SetDialogFontSizes(settings, dialog);

            dialog.SizeChangedHandler = SetupAndAddDialog(window, dialog);

            await dialog.WaitForLoadAsync();

            DialogOpened?.Invoke(window, new DialogStateChangedEventArgs(dialog));

            var result = await dialog.WaitForButtonPressAsync();

            // once a button as been clicked, begin removing the dialog.
            dialog.FireOnClose();

            DialogClosed?.Invoke(window, new DialogStateChangedEventArgs(dialog));

            await dialog.WaitForCloseAsync();

            window.SizeChanged -= dialog.SizeChangedHandler;
            window.RemoveDialog(dialog);

            await HandleOverlayOnHideAsync(settings, window);

            return result;
        }

        /// <summary>
        /// Creates a MessageDialog inside of the current window.
        /// </summary>
        /// <param name="window">The MetroWindow</param>
        /// <param name="title">The title of the MessageDialog.</param>
        /// <param name="message">The message contained within the MessageDialog.</param>
        /// <param name="style">The type of buttons to use.</param>
        /// <param name="settings">Optional settings that override the global metro dialog settings.</param>
        /// <returns>A task promising the result of which button was pressed.</returns>
        public static async Task<MessageDialogResult> ShowMessageAsync(this MetroWindow window, object title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings? settings = null)
        {
            window.Dispatcher.VerifyAccess();

            settings ??= window.MetroDialogOptions;

            await HandleOverlayOnShowAsync(settings, window);

            // create the dialog control
            var dialog = new MessageDialog(window, settings)
                         {
                             Message = message,
                             Title = title,
                             ButtonStyle = style
                         };

            SetDialogFontSizes(settings, dialog);

            dialog.SizeChangedHandler = SetupAndAddDialog(window, dialog);

            await dialog.WaitForLoadAsync();

            DialogOpened?.Invoke(window, new DialogStateChangedEventArgs(dialog));

            var result = await dialog.WaitForButtonPressAsync();

            // once a button as been clicked, begin removing the dialog.
            dialog.FireOnClose();

            DialogClosed?.Invoke(window, new DialogStateChangedEventArgs(dialog));

            await dialog.WaitForCloseAsync();

            window.SizeChanged -= dialog.SizeChangedHandler;
            window.RemoveDialog(dialog);

            await HandleOverlayOnHideAsync(settings, window);

            return result;
        }

        /// <summary>
        /// Creates a ProgressDialog inside of the current window.
        /// </summary>
        /// <param name="window">The MetroWindow</param>
        /// <param name="title">The title of the ProgressDialog.</param>
        /// <param name="message">The message within the ProgressDialog.</param>
        /// <param name="isCancelable">Determines if the cancel button is visible.</param>
        /// <param name="settings">Optional Settings that override the global metro dialog settings.</param>
        /// <returns>A task promising the instance of ProgressDialogController for this operation.</returns>
        public static async Task<ProgressDialogController> ShowProgressAsync(this MetroWindow window, object title, string message, bool isCancelable = false, MetroDialogSettings? settings = null)
        {
            window.Dispatcher.VerifyAccess();

            settings ??= window.MetroDialogOptions;

            await HandleOverlayOnShowAsync(settings, window);

            //create the dialog control
            var dialog = new ProgressDialog(window, settings)
                         {
                             Title = title,
                             Message = message,
                             IsCancelable = isCancelable
                         };

            SetDialogFontSizes(settings, dialog);

            dialog.SizeChangedHandler = SetupAndAddDialog(window, dialog);

            await dialog.WaitForLoadAsync();

            DialogOpened?.Invoke(window, new DialogStateChangedEventArgs(dialog));

            async Task CloseCallBack()
            {
                // once a button as been clicked, begin removing the dialog.
                dialog.FireOnClose();

                DialogClosed?.Invoke(window, new DialogStateChangedEventArgs(dialog));

                await dialog.WaitForCloseAsync();

                window.SizeChanged -= dialog.SizeChangedHandler;
                window.RemoveDialog(dialog);

                await HandleOverlayOnHideAsync(settings, window);
            }

            return new ProgressDialogController(dialog, CloseCallBack);
        }

        private static async Task HandleOverlayOnHideAsync(MetroDialogSettings? settings, MetroWindow window)
        {
            if (window.metroActiveDialogContainer is null)
            {
                throw new InvalidOperationException("Active dialog container could not be found.");
            }

            var isAnyDialogOpen = window.metroActiveDialogContainer.Children.OfType<BaseMetroDialog>().Any();
            if (!isAnyDialogOpen)
            {
                if (settings is null || settings.AnimateHide)
                {
                    await window.HideOverlayAsync();
                }
                else
                {
                    // ReSharper disable once MethodHasAsyncOverload
                    window.HideOverlay();
                }
            }

            if (window.metroActiveDialogContainer.Children.Count == 0)
            {
                window.SetValue(MetroWindow.IsCloseButtonEnabledWithDialogPropertyKey, BooleanBoxes.TrueBox);
                window.RestoreFocus();
            }
            else
            {
                var onTopShownDialogSettings = window.metroActiveDialogContainer.Children.OfType<BaseMetroDialog>().LastOrDefault()?.DialogSettings;
                var isCloseButtonEnabled = window.ShowDialogsOverTitleBar || onTopShownDialogSettings is null || onTopShownDialogSettings.OwnerCanCloseWithDialog;
                window.SetValue(MetroWindow.IsCloseButtonEnabledWithDialogPropertyKey, BooleanBoxes.Box(isCloseButtonEnabled));
            }
        }

        private static async Task HandleOverlayOnShowAsync(MetroDialogSettings? settings, MetroWindow window)
        {
            var isCloseButtonEnabled = window.ShowDialogsOverTitleBar || settings is null || settings.OwnerCanCloseWithDialog;
            window.SetValue(MetroWindow.IsCloseButtonEnabledWithDialogPropertyKey, BooleanBoxes.Box(isCloseButtonEnabled));

            if (window.metroActiveDialogContainer is null)
            {
                throw new InvalidOperationException("Active dialog container could not be found.");
            }

            var isAnyDialogOpen = window.metroActiveDialogContainer.Children.OfType<BaseMetroDialog>().Any();
            if (!isAnyDialogOpen)
            {
                if (settings is null || settings.AnimateShow)
                {
                    await window.ShowOverlayAsync();
                }
                else
                {
                    // ReSharper disable once MethodHasAsyncOverload
                    window.ShowOverlay();
                }
            }
        }

        /// <summary>
        /// Adds a Metro Dialog instance to the specified window and makes it visible asynchronously.
        /// If you want to wait until the user has closed the dialog, use <see cref="BaseMetroDialog.WaitUntilUnloadedAsync"/>
        /// <para>You have to close the resulting dialog yourself with <see cref="HideMetroDialogAsync"/>.</para>
        /// </summary>
        /// <param name="window">The owning window of the dialog.</param>
        /// <param name="dialog">The dialog instance itself.</param>
        /// <param name="settings">An optional pre-defined settings instance.</param>
        /// <returns>A task representing the operation.</returns>
        /// <exception cref="InvalidOperationException">The <paramref name="dialog"/> is already visible in the window.</exception>
        public static async Task ShowMetroDialogAsync(this MetroWindow window, BaseMetroDialog dialog, MetroDialogSettings? settings = null)
        {
            window.Dispatcher.VerifyAccess();

            if (window.metroActiveDialogContainer is null)
            {
                throw new InvalidOperationException("Active dialog container could not be found.");
            }

            if (window.metroInactiveDialogContainer is null)
            {
                throw new InvalidOperationException("Inactive dialog container could not be found.");
            }

            if (window.metroActiveDialogContainer.Children.Contains(dialog) || window.metroInactiveDialogContainer.Children.Contains(dialog))
            {
                throw new InvalidOperationException("The provided dialog is already visible in the specified window.");
            }

            settings ??= dialog.DialogSettings;

            await HandleOverlayOnShowAsync(settings, window);

            SetDialogFontSizes(settings, dialog);

            dialog.SizeChangedHandler = SetupAndAddDialog(window, dialog);

            await dialog.WaitForLoadAsync();

            DialogOpened?.Invoke(window, new DialogStateChangedEventArgs(dialog));
        }

        /// <summary>
        /// Adds a Metro Dialog instance of the given type to the specified window and makes it visible asynchronously.
        /// If you want to wait until the user has closed the dialog, use <see cref="BaseMetroDialog.WaitUntilUnloadedAsync"/>
        /// <para>You have to close the resulting dialog yourself with <see cref="HideMetroDialogAsync"/>.</para>
        /// </summary>
        /// <param name="window">The owning window of the dialog.</param>
        /// <param name="settings">An optional pre-defined settings instance.</param>
        /// <returns>A task with the dialog representing the operation.</returns>
        public static async Task<TDialog> ShowMetroDialogAsync<TDialog>(this MetroWindow window, MetroDialogSettings? settings = null)
            where TDialog : BaseMetroDialog
        {
            window.Dispatcher.VerifyAccess();

            settings ??= window.MetroDialogOptions;

            var dialog = (TDialog)Activator.CreateInstance(typeof(TDialog), window, settings)!;
            await window.ShowMetroDialogAsync(dialog);
            return dialog;
        }

        /// <summary>
        /// Hides a visible Metro Dialog instance.
        /// </summary>
        /// <param name="window">The window with the dialog that is visible.</param>
        /// <param name="dialog">The dialog instance to hide.</param>
        /// <param name="settings">An optional pre-defined settings instance.</param>
        /// <returns>A task representing the operation.</returns>
        /// <exception cref="InvalidOperationException">
        /// The <paramref name="dialog"/> is not visible in the window.
        /// This happens if <see cref="ShowMetroDialogAsync"/> hasn't been called before.
        /// </exception>
        public static async Task HideMetroDialogAsync(this MetroWindow window, BaseMetroDialog dialog, MetroDialogSettings? settings = null)
        {
            window.Dispatcher.VerifyAccess();

            if (window.metroActiveDialogContainer is null)
            {
                throw new InvalidOperationException("Active dialog container could not be found.");
            }

            if (window.metroInactiveDialogContainer is null)
            {
                throw new InvalidOperationException("Inactive dialog container could not be found.");
            }

            if (!window.metroActiveDialogContainer.Children.Contains(dialog) && !window.metroInactiveDialogContainer.Children.Contains(dialog))
            {
                throw new InvalidOperationException("The provided dialog is not visible in the specified window.");
            }

            // once a button as been clicked, begin removing the dialog.
            dialog.FireOnClose();

            DialogClosed?.Invoke(window, new DialogStateChangedEventArgs(dialog));

            await dialog.WaitForCloseAsync();

            window.SizeChanged -= dialog.SizeChangedHandler;
            window.RemoveDialog(dialog);

            settings ??= dialog.DialogSettings;
            await HandleOverlayOnHideAsync(settings, window);
        }

        /// <summary>
        /// Gets the current shown dialog in async way.
        /// </summary>
        /// <param name="window">The dialog owner.</param>
        public static Task<TDialog?> GetCurrentDialogAsync<TDialog>(this MetroWindow window)
            where TDialog : BaseMetroDialog
        {
            window.Dispatcher.VerifyAccess();

            var dialog = window.metroActiveDialogContainer?.Children.OfType<TDialog>().LastOrDefault();

            return Task.FromResult(dialog);
        }

        private static SizeChangedEventHandler SetupAndAddDialog(MetroWindow window, BaseMetroDialog dialog)
        {
            dialog.SetValue(Panel.ZIndexProperty, (int)(window.overlayBox?.GetValue(Panel.ZIndexProperty) ?? 0) + 1);

            var fixedMinHeight = dialog.MinHeight > 0;
            var fixedMaxHeight = dialog.MaxHeight is not double.PositiveInfinity && dialog.MaxHeight > 0;

            void CalculateMinAndMaxHeight()
            {
                if (!fixedMinHeight)
                {
                    dialog.SetCurrentValue(FrameworkElement.MinHeightProperty, window.ActualHeight / 4.0);
                }

                if (!fixedMaxHeight)
                {
                    dialog.SetCurrentValue(FrameworkElement.MaxHeightProperty, window.ActualHeight);
                }
                else
                {
                    dialog.SetCurrentValue(FrameworkElement.MinHeightProperty, Math.Min(dialog.MinHeight, dialog.MaxHeight));
                }
            }

            CalculateMinAndMaxHeight();

            void OnWindowSizeChanged(object sender, SizeChangedEventArgs args)
            {
                CalculateMinAndMaxHeight();
            }

            window.SizeChanged += OnWindowSizeChanged;

            window.AddDialog(dialog);

            dialog.FireOnShown();

            return OnWindowSizeChanged;
        }

        private static void AddDialog(this MetroWindow window, BaseMetroDialog dialog)
        {
            if (window.metroActiveDialogContainer is null)
            {
                throw new InvalidOperationException("Active dialog container could not be found.");
            }

            if (window.metroInactiveDialogContainer is null)
            {
                throw new InvalidOperationException("Inactive dialog container could not be found.");
            }

            window.StoreFocus();

            // if there's already an active dialog, move to the background
            var activeDialog = window.metroActiveDialogContainer.Children.OfType<BaseMetroDialog>().SingleOrDefault();
            if (activeDialog != null)
            {
                window.metroActiveDialogContainer.Children.Remove(activeDialog);
                window.metroInactiveDialogContainer.Children.Add(activeDialog);
            }

            window.metroActiveDialogContainer.Children.Add(dialog); //add the dialog to the container}

            window.SetValue(MetroWindow.IsAnyDialogOpenPropertyKey, BooleanBoxes.TrueBox);
        }

        private static void RemoveDialog(this MetroWindow window, BaseMetroDialog dialog)
        {
            if (window.metroActiveDialogContainer is null)
            {
                throw new InvalidOperationException("Active dialog container could not be found.");
            }

            if (window.metroInactiveDialogContainer is null)
            {
                throw new InvalidOperationException("Inactive dialog container could not be found.");
            }

            if (window.metroActiveDialogContainer.Children.Contains(dialog))
            {
                window.metroActiveDialogContainer.Children.Remove(dialog); //remove the dialog from the container

                // if there's an inactive dialog, bring it to the front
                var dlg = window.metroInactiveDialogContainer.Children.OfType<BaseMetroDialog>().LastOrDefault();
                if (dlg != null)
                {
                    window.metroInactiveDialogContainer.Children.Remove(dlg);
                    window.metroActiveDialogContainer.Children.Add(dlg);
                }
            }
            else
            {
                window.metroInactiveDialogContainer.Children.Remove(dialog);
            }

            window.SetValue(MetroWindow.IsAnyDialogOpenPropertyKey, BooleanBoxes.Box(window.metroActiveDialogContainer.Children.Count > 0));
        }

        private static MetroWindow CreateExternalWindow(Window? windowOwner = null)
        {
            var window = new MetroWindow
                         {
                             ShowInTaskbar = false,
                             ShowActivated = true,
                             Topmost = true,
                             ResizeMode = ResizeMode.NoResize,
                             WindowStartupLocation = WindowStartupLocation.CenterScreen,
                             BorderThickness = new Thickness(0),
                             ShowTitleBar = false,
                             ShowCloseButton = false,
                             ShowMinButton = false,
                             ShowMaxRestoreButton = false,
                             ShowSystemMenu = false,
                             WindowTransitionsEnabled = false,
                             Owner = windowOwner
                         };

            // If there is no Application then we need to add our default resources
            if (Application.Current is null)
            {
                window.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml", UriKind.RelativeOrAbsolute) });
                window.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml", UriKind.RelativeOrAbsolute) });

                if (windowOwner is not null)
                {
                    var theme = ThemeManager.Current.DetectTheme(windowOwner);
                    if (theme != null)
                    {
                        ThemeManager.Current.ChangeTheme(window, theme);
                    }
                }
            }

            return window;
        }

        private static MetroWindow CreateModalExternalWindow(MetroWindow windowOwner)
        {
            var win = CreateExternalWindow(windowOwner);
            win.Topmost = false; // It is not necessary here because the owner is set
            win.WindowStartupLocation = WindowStartupLocation.CenterOwner; // WindowStartupLocation should be CenterOwner

            // Set Width and Height maximum according Owner
            if (windowOwner.WindowState != WindowState.Maximized)
            {
                win.Width = windowOwner.ActualWidth;
                win.MaxHeight = windowOwner.ActualHeight;
            }
            else
            {
                // Get the monitor working area
                var monitorWorkingArea = windowOwner.GetMonitorWorkSize();
                if (monitorWorkingArea != default)
                {
                    win.Width = monitorWorkingArea.Width;
                    win.MaxHeight = monitorWorkingArea.Height;
                }
                else
                {
                    win.Width = windowOwner.ActualWidth;
                    win.MaxHeight = windowOwner.ActualHeight;
                }
            }

            win.SizeToContent = SizeToContent.Height;

            return win;
        }

        /// <summary>
        /// Creates a LoginDialog outside of the current window.
        /// </summary>
        /// <param name="window">The window that is the parent of the dialog.</param>
        /// <param name="title">The title of the LoginDialog.</param>
        /// <param name="message">The message contained within the LoginDialog.</param>
        /// <param name="settings">Optional settings that override the global metro dialog settings.</param>
        /// <returns>The text that was entered or null (Nothing in Visual Basic) if the user cancelled the operation.</returns>
        public static LoginDialogData? ShowModalLoginExternal(this MetroWindow window, string title, string message, LoginDialogSettings? settings = null)
        {
            var win = CreateModalExternalWindow(window);

            settings ??= new LoginDialogSettings();

            //create the dialog control
            LoginDialog dialog = new LoginDialog(win, settings)
                                 {
                                     Title = title,
                                     Message = message
                                 };

            SetDialogFontSizes(settings, dialog);

            win.Content = dialog;

            LoginDialogData? result = null;
            dialog.WaitForLoadAsync()
                  .ContinueWith(x =>
                      {
                          dialog.WaitForButtonPressAsync()
                                .ContinueWith(task =>
                                    {
                                        result = task.Result;
                                        win.Invoke(win.Close);
                                    });
                      });

            HandleOverlayOnShowAsync(settings, window).ConfigureAwait(true);
            win.ShowDialog();
            HandleOverlayOnHideAsync(settings, window).ConfigureAwait(true);
            return result;
        }

        /// <summary>
        /// Creates a InputDialog outside of the current window.
        /// </summary>
        /// <param name="window">The MetroWindow</param>
        /// <param name="title">The title of the MessageDialog.</param>
        /// <param name="message">The message contained within the MessageDialog.</param>
        /// <param name="settings">Optional settings that override the global metro dialog settings.</param>
        /// <returns>The text that was entered or null (Nothing in Visual Basic) if the user cancelled the operation.</returns>
        public static string? ShowModalInputExternal(this MetroWindow window, string title, string message, MetroDialogSettings? settings = null)
        {
            var win = CreateModalExternalWindow(window);

            settings ??= window.MetroDialogOptions;

            //create the dialog control
            var dialog = new InputDialog(win, settings)
                         {
                             Message = message,
                             Title = title,
                             Input = settings?.DefaultText
                         };

            SetDialogFontSizes(settings, dialog);

            win.Content = dialog;

            string? result = null;
            dialog.WaitForLoadAsync()
                  .ContinueWith(x =>
                      {
                          dialog.WaitForButtonPressAsync()
                                .ContinueWith(task =>
                                    {
                                        result = task.Result;
                                        win.Invoke(win.Close);
                                    });
                      });

            HandleOverlayOnShowAsync(settings, window).ConfigureAwait(true);
            win.ShowDialog();
            HandleOverlayOnHideAsync(settings, window).ConfigureAwait(true);
            return result;
        }

        /// <summary>
        /// Creates a MessageDialog outside of the current window.
        /// </summary>
        /// <param name="window">The MetroWindow</param>
        /// <param name="title">The title of the MessageDialog.</param>
        /// <param name="message">The message contained within the MessageDialog.</param>
        /// <param name="style">The type of buttons to use.</param>
        /// <param name="settings">Optional settings that override the global metro dialog settings.</param>
        /// <returns>A task promising the result of which button was pressed.</returns>
        public static MessageDialogResult ShowModalMessageExternal(this MetroWindow window, string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings? settings = null)
        {
            var win = CreateModalExternalWindow(window);

            settings ??= window.MetroDialogOptions;

            //create the dialog control
            var dialog = new MessageDialog(win, settings)
                         {
                             Message = message,
                             Title = title,
                             ButtonStyle = style
                         };

            SetDialogFontSizes(settings, dialog);

            win.Content = dialog;

            MessageDialogResult result = MessageDialogResult.Affirmative;
            dialog.WaitForLoadAsync()
                  .ContinueWith(x =>
                      {
                          dialog.WaitForButtonPressAsync()
                                .ContinueWith(task =>
                                    {
                                        result = task.Result;
                                        win.Invoke(win.Close);
                                    });
                      });

            HandleOverlayOnShowAsync(settings, window).ConfigureAwait(true);
            win.ShowDialog();
            HandleOverlayOnHideAsync(settings, window).ConfigureAwait(true);
            return result;
        }

        private static void SetDialogFontSizes(MetroDialogSettings? settings, BaseMetroDialog dialog)
        {
            if (settings is null)
            {
                return;
            }

            if (!double.IsNaN(settings.DialogTitleFontSize))
            {
                dialog.DialogTitleFontSize = settings.DialogTitleFontSize;
            }

            if (!double.IsNaN(settings.DialogMessageFontSize))
            {
                dialog.DialogMessageFontSize = settings.DialogMessageFontSize;
            }

            if (!double.IsNaN(settings.DialogButtonFontSize))
            {
                dialog.DialogButtonFontSize = settings.DialogButtonFontSize;
            }
        }

        public static event EventHandler<DialogStateChangedEventArgs>? DialogOpened;

        public static event EventHandler<DialogStateChangedEventArgs>? DialogClosed;
    }
}