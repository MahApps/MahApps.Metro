using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls.Dialogs
{
    public static class MetroWindowDialogExtensions
    {
        /// <summary>
        /// Creates a MessageDialog inside of the current window.
        /// </summary>
        /// <param name="title">The title of the MessageDialog.</param>
        /// <param name="message">The message contained within the MessageDialog.</param>
        /// <param name="style">The type of buttons to use.</param>
        /// <returns>A task promising the result of which button was pressed.</returns>
        public static Task<MessageDialogResult> ShowMessageAsync(this MetroWindow window, string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            window.Dispatcher.VerifyAccess();
            return window.ShowOverlayAsync().ContinueWith(z =>
                {
                    return window.Dispatcher.Invoke(new Func<object>(() =>
                        {
                            //create the dialog control
                            MessageDialog dialog = new MessageDialog(window);
                            dialog.Message = message;
                            dialog.ButtonStyle = style;

                            dialog.AffirmativeButtonText = window.MetroDialogOptions.AffirmativeButtonText;
                            dialog.NegativeButtonText = window.MetroDialogOptions.NegativeButtonText;

                            SizeChangedEventHandler sizeHandler = SetupAndOpenDialog(window, title, dialog);
                            dialog.SizeChangedHandler = sizeHandler;

                            return dialog.WaitForLoadAsync().ContinueWith(x =>
                            {
                                return dialog.WaitForButtonPressAsync().ContinueWith<MessageDialogResult>(y =>
                                {
                                    //once a button as been clicked, begin removing the dialog.

                                    dialog.OnClose();

                                    return ((Task)window.Dispatcher.Invoke(new Func<Task>(() =>
                                    {
                                        window.SizeChanged -= sizeHandler;

                                        window.messageDialogContainer.Children.Remove(dialog); //remove the dialog from the container

                                        return window.HideOverlayAsync();
                                        //window.overlayBox.Visibility = System.Windows.Visibility.Hidden; //deactive the overlay effect
                                    }))).ContinueWith(y3 => y).Result.Result;

                                }).Result;
                            });
                        }));
                }).ContinueWith(x => ((Task<MessageDialogResult>)x.Result).Result);
        }
        /// <summary>
        /// Creates a ProgressDialog inside of the current window.
        /// </summary>
        /// <param name="title">The title of the ProgressDialog.</param>
        /// <param name="message">The message within the ProgressDialog.</param>
        /// <param name="isCancelable">Determines if the cancel button is visible.</param>
        /// <returns>A task promising the instance of ProgressDialogController for this operation.</returns>
        public static Task<ProgressDialogController> ShowProgressAsync(this MetroWindow window, string title, string message, bool isCancelable = false)
        {
            window.Dispatcher.VerifyAccess();

            return window.ShowOverlayAsync().ContinueWith<ProgressDialogController>(z =>
            {
                return ((Task<ProgressDialogController>)window.Dispatcher.Invoke(new Func<Task<ProgressDialogController>>(() =>
                    {
                        //create the dialog control
                        ProgressDialog dialog = new ProgressDialog(window);
                        dialog.Message = message;
                        dialog.IsCancelable = isCancelable;
                        dialog.NegativeButtonText = window.MetroDialogOptions.NegativeButtonText;
                        SizeChangedEventHandler sizeHandler = SetupAndOpenDialog(window, title, dialog);
                        dialog.SizeChangedHandler = sizeHandler;

                        return dialog.WaitForLoadAsync().ContinueWith(x =>
                        {
                            return new ProgressDialogController(dialog, () =>
                            {
                                dialog.OnClose();

                                return (Task)window.Dispatcher.Invoke(new Func<Task>(() =>
                                {
                                    window.SizeChanged -= sizeHandler;

                                    window.messageDialogContainer.Children.Remove(dialog); //remove the dialog from the container

                                    return window.HideOverlayAsync();
                                    //window.overlayBox.Visibility = System.Windows.Visibility.Hidden; //deactive the overlay effect
                                }));
                            });
                        });
                    }))).Result;
            });
        }

        /// <summary>
        /// Adds a Metro Dialog instance to the specified window and makes it visible.
        /// </summary>
        /// <param name="window">The owning window of the dialog.</param>
        /// <param name="title">The title to be set in the dialog.</param>
        /// <param name="dialog">The dialog instance itself.</param>
        /// <returns>A task representing the operation.</returns>
        public static Task ShowMetroDialogAsync(this MetroWindow window, string title, BaseMetroDialog dialog)
        {
            window.Dispatcher.VerifyAccess();
            if (window.messageDialogContainer.Children.Contains(dialog))
                throw new Exception("The provided dialog is already visible in the specified window.");

            return window.ShowOverlayAsync().ContinueWith(z =>
                {
                    dialog.Dispatcher.Invoke(new Action(() =>
                        {
                            SizeChangedEventHandler sizeHandler = SetupAndOpenDialog(window, title, dialog);
                            dialog.SizeChangedHandler = sizeHandler;
                        }));
                }).ContinueWith(y =>
                    ((Task)dialog.Dispatcher.Invoke(new Func<Task>(() => dialog.WaitForLoadAsync()))));
        }
        /// <summary>
        /// Hides a visible Metro Dialog instance.
        /// </summary>
        /// <param name="window">The window with the dialog that is visible.</param>
        /// <param name="dialog">The dialog instance to hide.</param>
        /// <returns>A task representing the operation.</returns>
        public static Task HideMetroDialogAsync(this MetroWindow window, BaseMetroDialog dialog)
        {
            window.Dispatcher.VerifyAccess();
            if (!window.messageDialogContainer.Children.Contains(dialog))
                throw new Exception("The provided dialog is not visible in the specified window.");

            window.SizeChanged -= dialog.SizeChangedHandler;

            window.messageDialogContainer.Children.Remove(dialog); //remove the dialog from the container

            return window.HideOverlayAsync();
        }

        private static SizeChangedEventHandler SetupAndOpenDialog(MetroWindow window, string title, BaseMetroDialog dialog)
        {
            dialog.SetValue(Panel.ZIndexProperty, (int)window.overlayBox.GetValue(Panel.ZIndexProperty) + 1);
            dialog.MinHeight = window.ActualHeight / 4.0;
            dialog.Title = title;

            SizeChangedEventHandler sizeHandler = null; //an event handler for auto resizing an open dialog.
            sizeHandler = new SizeChangedEventHandler((sender, args) =>
            {
                dialog.MinHeight = window.ActualHeight / 4.0;
            });

            window.SizeChanged += sizeHandler;

            //window.overlayBox.Visibility = Visibility.Visible; //activate the overlay effect

            window.messageDialogContainer.Children.Add(dialog); //add the dialog to the container

            dialog.OnShown();

            if (window.TextBlockStyle != null && !dialog.Resources.Contains(typeof(TextBlock)))
            {
                dialog.Resources.Add(typeof(TextBlock), window.TextBlockStyle);
            }
            return sizeHandler;
        }
    }
}
