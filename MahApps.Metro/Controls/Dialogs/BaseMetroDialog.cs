using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MahApps.Metro.Controls.Dialogs
{
    public abstract class BaseMetroDialog : Control
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(BaseMetroDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty DialogBodyProperty = DependencyProperty.Register("DialogBody", typeof(object), typeof(BaseMetroDialog), new PropertyMetadata(null));
        public static readonly DependencyProperty DialogTopProperty = DependencyProperty.Register("DialogTop", typeof(object), typeof(BaseMetroDialog), new PropertyMetadata(null));
        public static readonly DependencyProperty DialogBottomProperty = DependencyProperty.Register("DialogBottom", typeof(object), typeof(BaseMetroDialog), new PropertyMetadata(null));

        /// <summary>
        /// Gets/sets the dialog's title.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Gets/sets arbitrary content in the "message" area in the dialog. 
        /// </summary>
        public object DialogBody
        {
            get { return GetValue(DialogBodyProperty); }
            set { SetValue(DialogBodyProperty, value); }
        }

        /// <summary>
        /// Gets/sets arbitrary content on top of the dialog.
        /// </summary>
        public object DialogTop
        {
            get { return GetValue(DialogTopProperty); }
            set { SetValue(DialogTopProperty, value); }
        }

        /// <summary>
        /// Gets/sets arbitrary content below the dialog.
        /// </summary>
        public object DialogBottom
        {
            get { return GetValue(DialogBottomProperty); }
            set { SetValue(DialogBottomProperty, value); }
        }

        internal SizeChangedEventHandler SizeChangedHandler { get; set; }


        static BaseMetroDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BaseMetroDialog), new FrameworkPropertyMetadata(typeof(BaseMetroDialog)));
        }

        /// <summary>
        /// Initializes a new MahApps.Metro.Controls.BaseMetroDialog.
        /// </summary>
        /// <param name="owningWindow">The window that is the parent of the dialog.</param>
        public BaseMetroDialog(MetroWindow owningWindow)
        {
            switch (owningWindow.MetroDialogOptions.ColorScheme)
            {
                case MetroDialogColorScheme.Theme:
                    this.SetResourceReference(BackgroundProperty, "WhiteColorBrush");
                    break;
                case MetroDialogColorScheme.Accented:
                    this.SetResourceReference(BackgroundProperty, "AccentColorBrush");
                    this.SetResourceReference(ForegroundProperty, "WhiteColorBrush");
                    break;
            }
        }
        /// <summary>
        /// Initializes a new MahApps.Metro.Controls.BaseMetroDialog.
        /// </summary>
        public BaseMetroDialog()
        {
        }

        /// <summary>
        /// Waits for the dialog to become ready for interaction.
        /// </summary>
        /// <returns>A task that represents the operation and it's status.</returns>
        public Task WaitForLoadAsync()
        {
            Dispatcher.VerifyAccess();

            if (this.IsLoaded) return new Task(() => { });

            TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();

            RoutedEventHandler handler = null;
            handler = new RoutedEventHandler((sender, args) =>
                {
                    this.Loaded -= handler;

                    tcs.TrySetResult(null);
                });

            this.Loaded += handler;

            return tcs.Task;
        }

        internal protected virtual void OnShown() { }
        internal protected virtual void OnClose() { }
    }

    /// <summary>
    /// A class that represents the settings used by Metro Dialogs.
    /// </summary>
    public class MetroDialogSettings
    {
        internal MetroDialogSettings()
        {
            AffirmativeButtonText = "OK";
            NegativeButtonText = "Cancel";

            ColorScheme = MetroDialogColorScheme.Theme;
        }

        /// <summary>
        /// Gets/sets the text used for the Affirmative button. For example: "OK" or "Yes".
        /// </summary>
        public string AffirmativeButtonText { get; set; }
        /// <summary>
        /// Gets/sets the text used for the Negative bytton. For example: "Cancel" or "No".
        /// </summary>
        public string NegativeButtonText { get; set; }

        /// <summary>
        /// Gets/sets whether the metro dialog should use the default black/white appearance (theme) or try to use the current accent.
        /// </summary>
        public MetroDialogColorScheme ColorScheme { get; set; }
    }

    /// <summary>
    /// An enum representing the different choices for a color scheme in a Metro Dialog.
    /// </summary>
    public enum MetroDialogColorScheme
    {
        Theme = 0,
        Accented = 1
    }
}
