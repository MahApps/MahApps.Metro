using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        public static Task<LoginDialogData> ShowLoginAsync(this MetroWindow window, string title, string message, LoginDialogSettings settings = null)
        {
            window.Dispatcher.VerifyAccess();
            return HandleOverlayOnShow(settings, window).ContinueWith(z =>
            {
                return (Task<LoginDialogData>)window.Dispatcher.Invoke(new Func<Task<LoginDialogData>>(() =>
                {
                    if (settings == null)
                    {
                        settings = new LoginDialogSettings();
                    }

                    //create the dialog control
                    LoginDialog dialog = new LoginDialog(window, settings)
                    {
                        Title = title,
                        Message = message
                    };

                    SizeChangedEventHandler sizeHandler = SetupAndOpenDialog(window, dialog);
                    dialog.SizeChangedHandler = sizeHandler;

                    return dialog.WaitForLoadAsync().ContinueWith(x =>
                    {
                        if (DialogOpened != null)
                        {
                            window.Dispatcher.BeginInvoke(new Action(() => DialogOpened(window, new DialogStateChangedEventArgs())));
                        }

                        return dialog.WaitForButtonPressAsync().ContinueWith(y =>
                        {
                            //once a button as been clicked, begin removing the dialog.

                            dialog.OnClose();

                            if (DialogClosed != null)
                            {
                                window.Dispatcher.BeginInvoke(new Action(() => DialogClosed(window, new DialogStateChangedEventArgs())));
                            }

                            Task closingTask = (Task)window.Dispatcher.Invoke(new Func<Task>(() => dialog._WaitForCloseAsync()));
                            return closingTask.ContinueWith(a =>
                            {
                                return ((Task)window.Dispatcher.Invoke(new Func<Task>(() =>
                                {
                                    window.SizeChanged -= sizeHandler;

                                    window.RemoveDialog(dialog);

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
        /// <param name="window">The MetroWindow</param>
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
                    var dialog = new InputDialog(window, settings)
                    {
                        Title = title,
                        Message = message,
                        Input = settings.DefaultText
                    };

                    SizeChangedEventHandler sizeHandler = SetupAndOpenDialog(window, dialog);
                    dialog.SizeChangedHandler = sizeHandler;

                    return dialog.WaitForLoadAsync().ContinueWith(x =>
                    {
                        if (DialogOpened != null)
                        {
                            window.Dispatcher.BeginInvoke(new Action(() => DialogOpened(window, new DialogStateChangedEventArgs())));
                        }

                        return dialog.WaitForButtonPressAsync().ContinueWith(y =>
                        {
                            //once a button as been clicked, begin removing the dialog.

                            dialog.OnClose();

                            if (DialogClosed != null)
                            {
                                window.Dispatcher.BeginInvoke(new Action(() => DialogClosed(window, new DialogStateChangedEventArgs())));
                            }

                            Task closingTask = (Task)window.Dispatcher.Invoke(new Func<Task>(() => dialog._WaitForCloseAsync()));
                            return closingTask.ContinueWith(a =>
                            {
                                return ((Task)window.Dispatcher.Invoke(new Func<Task>(() =>
                                {
                                    window.SizeChanged -= sizeHandler;

                                    window.RemoveDialog(dialog);

                                    return HandleOverlayOnHide(settings, window);
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
        /// <param name="window">The MetroWindow</param>
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
                    {
                        settings = window.MetroDialogOptions;
                    }

                    //create the dialog control
                    var dialog = new MessageDialog(window, settings)
                    {
                        Message = message,
                        Title = title,
                        ButtonStyle = style
                    };

                    SizeChangedEventHandler sizeHandler = SetupAndOpenDialog(window, dialog);
                    dialog.SizeChangedHandler = sizeHandler;

                    return dialog.WaitForLoadAsync().ContinueWith(x =>
                    {
                        if (DialogOpened != null)
                        {
                            window.Dispatcher.BeginInvoke(new Action(() => DialogOpened(window, new DialogStateChangedEventArgs())));
                        }

                        return dialog.WaitForButtonPressAsync().ContinueWith(y =>
                        {
                            //once a button as been clicked, begin removing the dialog.

                            dialog.OnClose();

                            if (DialogClosed != null)
                            {
                                window.Dispatcher.BeginInvoke(new Action(() => DialogClosed(window, new DialogStateChangedEventArgs())));
                            }

                            Task closingTask = (Task)window.Dispatcher.Invoke(new Func<Task>(() => dialog._WaitForCloseAsync()));
                            return closingTask.ContinueWith(a =>
                            {
                                return ((Task)window.Dispatcher.Invoke(new Func<Task>(() =>
                                {
                                    window.SizeChanged -= sizeHandler;

                                    window.RemoveDialog(dialog);

                                    return HandleOverlayOnHide(settings, window);
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
        /// <param name="window">The MetroWindow</param>
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
                    var dialog = new ProgressDialog(window)
                    {
                        Message = message,
                        Title = title,
                        IsCancelable = isCancelable
                    };

                    if (settings == null)
                    {
                        settings = window.MetroDialogOptions;
                    }

                    dialog.NegativeButtonText = settings.NegativeButtonText;

                    SizeChangedEventHandler sizeHandler = SetupAndOpenDialog(window, dialog);
                    dialog.SizeChangedHandler = sizeHandler;

                    return dialog.WaitForLoadAsync().ContinueWith(x =>
                    {
                        if (DialogOpened != null)
                        {
                            window.Dispatcher.BeginInvoke(new Action(() => DialogOpened(window, new DialogStateChangedEventArgs())));
                        }

                        return new ProgressDialogController(dialog, () =>
                        {
                            dialog.OnClose();

                            if (DialogClosed != null)
                            {
                                window.Dispatcher.BeginInvoke(new Action(() => DialogClosed(window, new DialogStateChangedEventArgs())));
                            }

                            Task closingTask = (Task)window.Dispatcher.Invoke(new Func<Task>(() => dialog._WaitForCloseAsync()));
                            return closingTask.ContinueWith(a =>
                            {
                                return (Task)window.Dispatcher.Invoke(new Func<Task>(() =>
                                {
                                    window.SizeChanged -= sizeHandler;

                                    window.RemoveDialog(dialog);

                                    return HandleOverlayOnHide(settings, window);
                                }));
                            }).Unwrap();
                        });
                    });
                })));
            }).Unwrap();
        }

        private static Task HandleOverlayOnHide(MetroDialogSettings settings, MetroWindow window)
        {
            if (window.metroActiveDialogContainer.Children.Count == 0)
            {
                return (settings == null || settings.AnimateHide ? window.HideOverlayAsync() : Task.Factory.StartNew(() => window.Dispatcher.Invoke(new Action(window.HideOverlay))));
            }
            else
            {
                var tcs = new System.Threading.Tasks.TaskCompletionSource<object>();
                tcs.SetResult(null);
                return tcs.Task;
            }
        }

        private static Task HandleOverlayOnShow(MetroDialogSettings settings, MetroWindow window)
        {
            if (window.metroActiveDialogContainer.Children.Count == 0)
            {
                return (settings == null || settings.AnimateShow ? window.ShowOverlayAsync() : Task.Factory.StartNew(() => window.Dispatcher.Invoke(new Action(window.ShowOverlay))));
            }
            else
            {
                var tcs = new System.Threading.Tasks.TaskCompletionSource<object>();
                tcs.SetResult(null);
                return tcs.Task;
            }
        }

        /// <summary>
        /// Adds a Metro Dialog instance to the specified window and makes it visible asynchronously.
        /// If you want to wait until the user has closed the dialog, use <see cref="ShowMetroDialogAsyncAwaitable"/>
        /// <para>You have to close the resulting dialog yourself with <see cref="HideMetroDialogAsync"/>.</para>
        /// </summary>
        /// <param name="window">The owning window of the dialog.</param>
        /// <param name="dialog">The dialog instance itself.</param>
        /// <param name="settings">An optional pre-defined settings instance.</param>
        /// <returns>A task representing the operation.</returns>
        /// <exception cref="InvalidOperationException">The <paramref name="dialog"/> is already visible in the window.</exception>
        public static Task ShowMetroDialogAsync(this MetroWindow window, BaseMetroDialog dialog,
            MetroDialogSettings settings = null)
        {
            window.Dispatcher.VerifyAccess();
            if (window.metroActiveDialogContainer.Children.Contains(dialog) || window.metroInactiveDialogContainer.Children.Contains(dialog))
                throw new InvalidOperationException("The provided dialog is already visible in the specified window.");

            return HandleOverlayOnShow(settings, window).ContinueWith(z =>
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
                        DialogOpened(window, new DialogStateChangedEventArgs());
                    }
                })))));
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
        public static Task HideMetroDialogAsync(this MetroWindow window, BaseMetroDialog dialog, MetroDialogSettings settings = null)
        {
            window.Dispatcher.VerifyAccess();
            if (!window.metroActiveDialogContainer.Children.Contains(dialog) && !window.metroInactiveDialogContainer.Children.Contains(dialog))
                throw new InvalidOperationException("The provided dialog is not visible in the specified window.");

            window.SizeChanged -= dialog.SizeChangedHandler;

            dialog.OnClose();

            Task closingTask = (Task)window.Dispatcher.Invoke(new Func<Task>(dialog._WaitForCloseAsync));
            return closingTask.ContinueWith(a =>
            {
                if (DialogClosed != null)
                {
                    window.Dispatcher.BeginInvoke(new Action(() => DialogClosed(window, new DialogStateChangedEventArgs())));
                }

                return (Task)window.Dispatcher.Invoke(new Func<Task>(() =>
                {
                    window.RemoveDialog(dialog);

                    return HandleOverlayOnHide(settings,window);
                }));
            }).Unwrap();
        }

        /// <summary>
        /// Gets the current shown dialog.
        /// </summary>
        /// <param name="window">The dialog owner.</param>
        public static Task<TDialog> GetCurrentDialogAsync<TDialog>(this MetroWindow window) where TDialog : BaseMetroDialog
        {
            window.Dispatcher.VerifyAccess();
            var t = new TaskCompletionSource<TDialog>();
            window.Dispatcher.Invoke((Action)(() =>
            {
                TDialog dialog = window.metroActiveDialogContainer.Children.OfType<TDialog>().LastOrDefault();
                t.TrySetResult(dialog);
            }));
            return t.Task;
        }

        private static SizeChangedEventHandler SetupAndOpenDialog(MetroWindow window, BaseMetroDialog dialog)
        {
            dialog.SetValue(Panel.ZIndexProperty, (int)window.overlayBox.GetValue(Panel.ZIndexProperty) + 1);
            dialog.MinHeight = window.ActualHeight / 4.0;
            dialog.MaxHeight = window.ActualHeight;

            SizeChangedEventHandler sizeHandler = (sender, args) =>
            {
                dialog.MinHeight = window.ActualHeight / 4.0;
                dialog.MaxHeight = window.ActualHeight;
            };

            window.SizeChanged += sizeHandler;

            window.AddDialog(dialog);

            dialog.OnShown();

            return sizeHandler;
        }

        private static void AddDialog(this MetroWindow window, BaseMetroDialog dialog)
        {
            // if there's already an active dialog, move to the background
            var activeDialog = window.metroActiveDialogContainer.Children.Cast<UIElement>().SingleOrDefault();
            if (activeDialog != null)
            {
                window.metroActiveDialogContainer.Children.Remove(activeDialog);
                window.metroInactiveDialogContainer.Children.Add(activeDialog);
            }

            window.metroActiveDialogContainer.Children.Add(dialog); //add the dialog to the container}
        }

        private static void RemoveDialog(this MetroWindow window, BaseMetroDialog dialog)
        {
            if (window.metroActiveDialogContainer.Children.Contains(dialog))
            {
                window.metroActiveDialogContainer.Children.Remove(dialog); //remove the dialog from the container

                // if there's an inactive dialog, bring it to the front
                var dlg = window.metroInactiveDialogContainer.Children.Cast<UIElement>().LastOrDefault();
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
        }

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
            var win = new MetroWindow
            {
                ShowInTaskbar = false,
                ShowActivated = true,
                Topmost = true,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.None,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ShowTitleBar = false,
                ShowCloseButton = false,
                WindowTransitionsEnabled = false
            };

            try
            {
                win.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Controls.xaml") });
                win.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Fonts.xaml") });
                win.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MahApps.Metro;component/Styles/Colors.xaml") });
                win.SetResourceReference(MetroWindow.GlowBrushProperty, "AccentColorBrush");
            }
            catch (Exception) { }

            win.Width = SystemParameters.PrimaryScreenWidth;
            win.MinHeight = SystemParameters.PrimaryScreenHeight / 4.0;
            win.SizeToContent = SizeToContent.Height;

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

        public static event EventHandler<DialogStateChangedEventArgs> DialogOpened;
        public static event EventHandler<DialogStateChangedEventArgs> DialogClosed;
    }
}
