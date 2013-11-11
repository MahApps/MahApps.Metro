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

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public object DialogBody
        {
            get { return GetValue(DialogBodyProperty); }
            set { SetValue(DialogBodyProperty, value); }
        }

        public object DialogTop
        {
            get { return GetValue(DialogTopProperty); }
            set { SetValue(DialogTopProperty, value); }
        }

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
        public BaseMetroDialog()
        {
        }

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

    public class MetroDialogSettings
    {
        internal MetroDialogSettings()
        {
            AffirmativeButtonText = "OK";
            NegativeButtonText = "Cancel";

            ColorScheme = MetroDialogColorScheme.Theme;
        }

        public string AffirmativeButtonText { get; set; }
        public string NegativeButtonText { get; set; }

        public MetroDialogColorScheme ColorScheme { get; set; }
    }

    public enum MetroDialogColorScheme
    {
        Theme = 0,
        Accented = 1
    }
}
