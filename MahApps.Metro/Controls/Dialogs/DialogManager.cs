using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MahApps.Metro.Behaviours;

namespace MahApps.Metro.Controls.Dialogs
{
    public static class DialogManager
    {
        #region In-Window Extension Methods
        /// <summary>
        /// Creates a LoginDialog inside of the current window.
        /// </summary>
        /// <param name="title">The title of the LoginDialog.</param>
        /// <param name="message">The message contained within the LoginDialog.</param>
        /// <param name="settings">Optional settings that override the global metro dialog settings.</param>
        /// <returns>The text that was entered or null (Nothing in Visual Basic) if the user cancelled the operation.</returns>
        public static Task<LoginDialogData> ShowLoginAsync(this MetroWindow window, string title, string message, LoginDialogSettings settings = null)
        {
            window.Dispatcher.VerifyAccess();
            return HandleOverlayOnShow(settings, window).ContinueWith(z =>
            {
                return (Task<LoginDialogData>)window.Dispatcher.Invoke(new Func<Task<LoginDialogData>>(() =>
                {
                    if (settings == null) { settings = new LoginDialogSettings(); }

                    //create the dialog control
                    LoginDialog dialog = new LoginDialog(window, settings);
                    dialog.Title = title;
                    dialog.Message = message;

                    SizeChangedEventHandler sizeHandler = SetupAndOpenDialog(window, dialog);
                    dialog.SizeChangedHandler = sizeHandler;

                    return dialog.WaitForLoadAsync().ContinueWith(x =>
                    {
                        if (DialogOpened != null)
                        {
                            window.Dispatcher.BeginInvoke(new Action(() => DialogOpened(window, new DialogStateChangedEventArgs() { })));
                        }

                        return dialog.WaitForButtonPressAsync().ContinueWith(y =>
                        {
                            //once a button as been clicked, begin removing the dialog.

                            dialog.OnClose();

                            if (DialogClosed != null)
                            {
                                window.Dispatcher.BeginInvoke(new Action(() => DialogClosed(window, new DialogStateChangedEventArgs() { })));
                            }

                            Task closingTask = (Task)window.Dispatcher.Invoke(new Func<Task>(() => dialog._WaitForCloseAsync()));
                            return closingTask.ContinueWith<Task<LoginDialogData>>(a =>
                            {
                                return ((Task)window.Dispatcher.Invoke(new Func<Task>(() =>
                                {
                                    window.SizeChanged -= sizeHandler;

                                    window.metroDialogContainer.Children.Remove(dialog); //remove the dialog from the container

                                    return HandleOverlayOnHide(settings, window);
                                    //window.overlayBox.Visibility = System.Windows.Visibility.Hidden; //deactive the overlay effect

                                }))).ContinueWith(y3 => y).Unwrap();
                            });
                        }).Unwrap();
                    }).Unwrap().Unwrap();
                }));
            }).Unwrap();
        }
        /// <summary>
        /// Creates a InputDialog inside of the current window.
        /// </summary>
        /// <param name="title">The title of the MessageDialog.</param>
        /// <param name="message">The message contained within the MessageDialog.</param>
        /// <param name="settings">Optional settings that override the global metro dialog settings.</param>
        /// <returns>The text that was entered or null (Nothing in Visual Basic) if the user cancelled the operation.</returns>
        public static Task<string> ShowInputAsync(this MetroWindow window, string title, string message, MetroDialogSettings settings = null)
        {
            window.Dispatcher.VerifyAccess();
            return HandleOverlayOnShow(settings, window).ContinueWith(z =>
                {
                    return (Task<string>)window.Dispatcher.Invoke(new Func<Task<string>>(() =>
                        {
                            if (settings == null)
                                settings = window.MetroDialogOptions;

                            //create the dialog control
                            InputDialog dialog = new InputDialog(window, settings);
                            dialog.Title = title;
                            dialog.Message = message;
                            dialog.Input = settings.DefaultText;

                            SizeChangedEventHandler sizeHandler = SetupAndOpenDialog(window, dialog);
                            dialog.SizeChangedHandler = sizeHandler;

                            return dialog.WaitForLoadAsync().ContinueWith(x =>
                            {
                                if (DialogOpened != null)
                                {
                                    window.Dispatcher.BeginInvoke(new Action(() => DialogOpened(window, new DialogStateChangedEventArgs()
                                    {
                                    })));
                                }

                                return dialog.WaitForButtonPressAsync().ContinueWith(y =>
                                {
                                    //once a button as been clicked, begin removing the dialog.

                                    dialog.OnClose();

                                    if (DialogClosed != null)
                                    {
                                        window.Dispatcher.BeginInvoke(new Action(() => DialogClosed(window, new DialogStateChangedEventArgs()
                                        {
                                        })));
                                    }

                                    Task closingTask = (Task)window.Dispatcher.Invoke(new Func<Task>(() => dialog._WaitForCloseAsync()));
                                    return closingTask.ContinueWith<Task<string>>(a =>
                                        {
                                            return ((Task)window.Dispatcher.Invoke(new Func<Task>(() =>
                                            {
                                                window.SizeChanged -= sizeHandler;

                                                window.metroDialogContainer.Children.Remove(dialog); //remove the dialog from the container

                                                return HandleOverlayOnHide(settings, window);
                                                //window.overlayBox.Visibility = System.Windows.Visibility.Hidden; //deactive the overlay effect

                                            }))).ContinueWith(y3 => y).Unwrap();
                                        });
                                }).Unwrap();
                            }).Unwrap().Unwrap();
                        }));
                }).Unwrap();
        }
        /// <summary>
        /// Creates a MessageDialog inside of the current window.
        /// </summary>
        /// <param name="title">The title of the MessageDialog.</param>
        /// <param name="message">The message contained within the MessageDialog.</param>
        /// <param name="style">The type of buttons to use.</param>
        /// <param name="settings">Optional settings that override the global metro dialog settings.</param>
        /// <returns>A task promising the result of which button was pressed.</returns>
        public static Task<MessageDialogResult> ShowMessageAsync(this MetroWindow window, string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative, MetroDialogSettings settings = null)
        {
            window.Dispatcher.VerifyAccess();
            return HandleOverlayOnShow(settings, window).ContinueWith(z =>
                {
                    return (Task<MessageDialogResult>)window.Dispatcher.Invoke(new Func<Task<MessageDialogResult>>(() =>
                        {
                            if (settings == null)
                                settings = window.MetroDialogOptions;

                            //create the dialog control
                            MessageDialog dialog = new MessageDialog(window, settings);
                            dialog.Message = message;
                            dialog.Title = title;
                            dialog.ButtonStyle = style;

                            SizeChangedEventHandler sizeHandler = SetupAndOpenDialog(window, dialog);
                            dialog.SizeChangedHandler = sizeHandler;

                            return dialog.WaitForLoadAsync().ContinueWith(x =>
                            {
                                if (DialogOpened != null)
                                {
                                    window.Dispatcher.BeginInvoke(new Action(() => DialogOpened(window, new DialogStateChangedEventArgs()
                                    {
                                    })));
                                }

                                return dialog.WaitForButtonPressAsync().ContinueWith(y =>
                                {
                                    //once a button as been clicked, begin removing the dialog.

                                    dialog.OnClose();

                                    if (DialogClosed != null)
                                    {
                                        window.Dispatcher.BeginInvoke(new Action(() => DialogClosed(window, new DialogStateChangedEventArgs()
                                        {
                                        })));
                                    }

                                    Task closingTask = (Task)window.Dispatcher.Invoke(new Func<Task>(() => dialog._WaitForCloseAsync()));
                                    return closingTask.ContinueWith<Task<MessageDialogResult>>(a =>
                                        {
                                            return ((Task)window.Dispatcher.Invoke(new Func<Task>(() =>
                                            {
                                                window.SizeChanged -= sizeHandler;

                                                window.metroDialogContainer.Children.Remove(dialog); //remove the dialog from the container

                                                return HandleOverlayOnHide(settings, window);
                                                //window.overlayBox.Visibility = System.Windows.Visibility.Hidden; //deactive the overlay effect

                                            }))).ContinueWith(y3 => y).Unwrap();
                                        });
                                }).Unwrap();
                            }).Unwrap().Unwrap();
                        }));
                }).Unwrap();
        }

        /// <summary>
        /// Creates a ProgressDialog inside of the current window.
        /// </summary>
        /// <param name="title">The title of the ProgressDialog.</param>
        /// <param name="message">The message within the ProgressDialog.</param>
        /// <param name="isCancelable">Determines if the cancel button is visible.</param>
        /// <param name="settings">Optional Settings that override the global metro dialog settings.</param>
        /// <returns>A task promising the instance of ProgressDialogController for this operation.</returns>
        public static Task<ProgressDialogController> ShowProgressAsync(this MetroWindow window, string title, string message, bool isCancelable = false, MetroDialogSettings settings = null)
        {
            window.Dispatcher.VerifyAccess();

            return HandleOverlayOnShow(settings, window).ContinueWith(z =>
            {
                return ((Task<ProgressDialogController>)window.Dispatcher.Invoke(new Func<Task<ProgressDialogController>>(() =>
                    {
                        //create the dialog control
                        ProgressDialog dialog = new ProgressDialog(window);
                        dialog.Message = message;
                        dialog.Title = title;
                        dialog.IsCancelable = isCancelable;

                        if (settings == null)
                            settings = window.MetroDialogOptions;

                        dialog.NegativeButtonText = settings.NegativeButtonText;

                        SizeChangedEventHandler sizeHandler = SetupAndOpenDialog(window, dialog);
                        dialog.SizeChangedHandler = sizeHandler;

                        return dialog.WaitForLoadAsync().ContinueWith(x =>
                        {
                            if (DialogOpened != null)
                            {
                                window.Dispatcher.BeginInvoke(new Action(() => DialogOpened(window, new DialogStateChangedEventArgs()
                                {
                                })));
                            }

                            return new ProgressDialogController(dialog, () =>
                            {
                                dialog.OnClose();

                                if (DialogClosed != null)
                                {
                                    window.Dispatcher.BeginInvoke(new Action(() => DialogClosed(window, new DialogStateChangedEventArgs()
                                    {
                                    })));
                                }

                                Task closingTask = (Task)window.Dispatcher.Invoke(new Func<Task>(() => dialog._WaitForCloseAsync()));
                                return closingTask.ContinueWith<Task>(a =>
                                {
                                    return (Task)window.Dispatcher.Invoke(new Func<Task>(() =>
                                    {
                                        window.SizeChanged -= sizeHandler;

                                        window.metroDialogContainer.Children.Remove(dialog); //remove the dialog from the container

                                        return HandleOverlayOnHide(settings, window);
                                        //window.overlayBox.Visibility = System.Windows.Visibility.Hidden; //deactive the overlay effect
                                    }));
                                }).Unwrap();
                            });
                        });
                    })));
            }).Unwrap();
        }

        private static Task HandleOverlayOnHide(MetroDialogSettings settings, MetroWindow window)
        {
            return (settings == null || settings.AnimateHide ? window.HideOverlayAsync() : Task.Factory.StartNew(() => window.Dispatcher.Invoke(new Action(() => window.HideOverlay()))));
        }
        private static Task HandleOverlayOnShow(MetroDialogSettings settings, MetroWindow window)
        {
            return (settings == null || settings.AnimateShow ? window.ShowOverlayAsync() : Task.Factory.StartNew(() => window.Dispatcher.Invoke(new Action(() => window.ShowOverlay()))));
        }

        /// <summary>
        /// Adds a Metro Dialog instance to the specified window and makes it visible.
        /// <para>Note that this method returns as soon as the dialog is loaded and won't wait on a call of <see cref="HideMetroDialogAsync"/>.</para>
        /// <para>You can still close the resulting dialog with <see cref="HideMetroDialogAsync"/>.</para>
        /// </summary>
        /// <param name="window">The owning window of the dialog.</param>
        /// <param name="dialog">The dialog instance itself.</param>
        /// <returns>A task representing the operation.</returns>
        /// <exception cref="InvalidOperationException">The <paramref name="dialog"/> is already visible in the window.</exception>
        public static Task ShowMetroDialogAsync(this MetroWindow window, BaseMetroDialog dialog)
        {
            window.Dispatcher.VerifyAccess();
            if (window.metroDialogContainer.Children.Contains(dialog))
                throw new InvalidOperationException("The provided dialog is already visible in the specified window.");

            return window.ShowOverlayAsync().ContinueWith(z =>
                {
                    dialog.Dispatcher.Invoke(new Action(() =>
                        {
                            SizeChangedEventHandler sizeHandler = SetupAndOpenDialog(window, dialog);
                            dialog.SizeChangedHandler = sizeHandler;
                        }));
                }).ContinueWith(y =>
                    ((Task)dialog.Dispatcher.Invoke(new Func<Task>(() => dialog.WaitForLoadAsync().ContinueWith(x =>
                        {
                            dialog.OnShown();

                            if (DialogOpened != null)
                            {
                                DialogOpened(window, new DialogStateChangedEventArgs()
                                {
                                });
                            }
                        })))));
        }
        /// <summary>
        /// Hides a visible Metro Dialog instance.
        /// </summary>
        /// <param name="window">The window with the dialog that is visible.</param>
        /// <param name="dialog">The dialog instance to hide.</param>
        /// <returns>A task representing the operation.</returns>
        /// <exception cref="InvalidOperationException">
        /// The <paramref name="dialog"/> is not visible in the window.
        /// This happens if <see cref="ShowMetroDialogAsync"/> hasn't been called before.
        /// </exception>
        public static Task HideMetroDialogAsync(this MetroWindow window, BaseMetroDialog dialog)
        {
            window.Dispatcher.VerifyAccess();
            if (!window.metroDialogContainer.Children.Contains(dialog))
                throw new InvalidOperationException("The provided dialog is not visible in the specified window.");

            window.SizeChanged -= dialog.SizeChangedHandler;

            dialog.OnClose();

            Task closingTask = (Task)window.Dispatcher.Invoke(new Func<Task>(() => dialog._WaitForCloseAsync()));
            return closingTask.ContinueWith<Task>(a =>
            {
                if (DialogClosed != null)
                {
                    window.Dispatcher.BeginInvoke(new Action(() => DialogClosed(window, new DialogStateChangedEventArgs()
                    {
                    })));
                }

                return (Task)window.Dispatcher.Invoke(new Func<Task>(() =>
                {
                    window.metroDialogContainer.Children.Remove(dialog); //remove the dialog from the container

                    return window.HideOverlayAsync();
                }));
            }).Unwrap();
        }

        private static SizeChangedEventHandler SetupAndOpenDialog(MetroWindow window, BaseMetroDialog dialog)
        {
            dialog.SetValue(Panel.ZIndexProperty, (int)window.overlayBox.GetValue(Panel.ZIndexProperty) + 1);
            dialog.MinHeight = window.ActualHeight / 4.0;
            dialog.MaxHeight = window.ActualHeight;

            SizeChangedEventHandler sizeHandler = null; //an event handler for auto resizing an open dialog.
            sizeHandler = new SizeChangedEventHandler((sender, args) =>
            {
                dialog.MinHeight = window.ActualHeight / 4.0;
                dialog.MaxHeight = window.ActualHeight;
            });

            window.SizeChanged += sizeHandler;

            //window.overlayBox.Visibility = Visibility.Visible; //activate the overlay effect

            window.metroDialogContainer.Children.Add(dialog); //add the dialog to the container

            dialog.OnShown();

            return sizeHandler;
        }
        #endregion

        #region External Windowed Dialog Methods
        public static BaseMetroDialog ShowDialogExternally(this BaseMetroDialog dialog)
        {
            Window win = SetupExternalDialogWindow(dialog);

            dialog.OnShown();
            win.Show();

            return dialog;
        }

        public static BaseMetroDialog ShowModalDialogExternally(this BaseMetroDialog dialog)
        {
            Window win = SetupExternalDialogWindow(dialog);

            dialog.OnShown();
            win.ShowDialog();

            return dialog;
        }

        private static Window SetupExternalDialogWindow(BaseMetroDialog dialog)
        {
            MetroWindow win = new MetroWindow();
            win.ShowInTaskbar = false;
            win.ShowActivated = true;
            win.Topmost = true;
            win.ResizeMode = ResizeMode.NoResize;
            win.WindowStyle = WindowStyle.None;
            win.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            win.ShowTitleBar = false;
            win.ShowCloseButton = false;
            win.WindowTransitionsEnabled = false;
            win.Background = dialog.Background;

            try
            {
                win.GlowBrush = win.TryFindResource("AccentColorBrush") as SolidColorBrush;
            }
            catch (Exception) { }

            win.Width = SystemParameters.PrimaryScreenWidth;
            win.MinHeight = SystemParameters.PrimaryScreenHeight / 4.0;
            win.SizeToContent = SizeToContent.Height;

            GlowWindowBehavior glowWindow = new GlowWindowBehavior();
            glowWindow.Attach(win);

            dialog.ParentDialogWindow = win; //THIS IS ONLY, I REPEAT, ONLY SET FOR EXTERNAL DIALOGS!

            win.Content = dialog;

            EventHandler closedHandler = null;
            closedHandler = (sender, args) => 
            {
                win.Closed -= closedHandler;
                dialog.ParentDialogWindow = null;
                win.Content = null;
            };
            win.Closed += closedHandler;

            return win;
        }
        #endregion

        public delegate void DialogStateChangedHandler(object sender, DialogStateChangedEventArgs args);

        public static event DialogStateChangedHandler DialogOpened;
        public static event DialogStateChangedHandler DialogClosed;
    }
}
