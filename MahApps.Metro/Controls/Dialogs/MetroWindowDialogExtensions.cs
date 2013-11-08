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
        /// <returns></returns>
        public static Task<MessageDialogResult> ShowMessageAsync(this MetroWindow window, string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative)
        {
            //create the dialog control
            MessageDialog dialog = new MessageDialog(window);
            dialog.Message = message;
            dialog.ButtonStyle = style;

            dialog.AffirmativeButtonText = window.MessageDialogOptions.AffirmativeButtonText;
            dialog.NegativeButtonText = window.MessageDialogOptions.NegativeButtonText;

            SizeChangedEventHandler sizeHandler = SetupAndOpenDialog(window, title, dialog);

            return dialog.WaitForLoadAsync().ContinueWith<System.Threading.Tasks.Task<MessageDialogResult>>(x =>
            {
                return dialog.WaitForButtonPressAsync().ContinueWith<MessageDialogResult>(y =>
                {
                    //once a button as been clicked, begin removing the dialog.
                    window.Dispatcher.Invoke(new Action(() =>
                    {
                        window.SizeChanged -= sizeHandler;

                        window.messageDialogContainer.Children.Remove(dialog); //removed the dialog from the container

                        window.overlayBox.Visibility = System.Windows.Visibility.Hidden; //deactive the overlay effect
                    }));

                    return y.Result;
                });
            }).ContinueWith(x =>
                    x.Result.Result);
        }

        public static Task ShowProgressAsync(this MetroWindow window, string title)
        {
            //create the dialog control
            ProgressDialog dialog = new ProgressDialog(window);
            SizeChangedEventHandler sizeHandler = SetupAndOpenDialog(window, title, dialog);


            return dialog.WaitForLoadAsync().ContinueWith(x =>
            {
                Thread.Sleep(10000);
                window.Dispatcher.Invoke(new Action(() =>
                {
                    window.SizeChanged -= sizeHandler;

                    window.messageDialogContainer.Children.Remove(dialog); //removed the dialog from the container

                    window.overlayBox.Visibility = System.Windows.Visibility.Hidden; //deactive the overlay effect
                }));
            });
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

            window.overlayBox.Visibility = Visibility.Visible; //activate the overlay effect

            window.messageDialogContainer.Children.Add(dialog); //add the dialog to the container

            if (window.TextBlockStyle != null && !dialog.Resources.Contains(typeof(TextBlock)))
            {
                dialog.Resources.Add(typeof(TextBlock), window.TextBlockStyle);
            }
            return sizeHandler;
        }
    }
}
