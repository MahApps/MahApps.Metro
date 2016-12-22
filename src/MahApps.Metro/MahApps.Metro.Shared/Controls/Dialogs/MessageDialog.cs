using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ControlzEx;

namespace MahApps.Metro.Controls.Dialogs
{
    /// <summary>
    /// An internal control that represents a message dialog. Please use MetroWindow.ShowMessage instead!
    /// </summary>
    public partial class MessageDialog : BaseMetroDialog
    {
        internal MessageDialog()
            : this(null)
        {
        }

        internal MessageDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }

        internal MessageDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            : base(parentWindow, settings)
        {
            this.InitializeComponent();

            this.PART_MessageScrollViewer.Height = this.DialogSettings.MaximumBodyHeight;
        }

        internal Task<MessageDialogResult> WaitForButtonPressAsync()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
                                                       {
                                                           this.Focus();

                                                           var defaultButtonFocus = this.DialogSettings.DefaultButtonFocus;

                                                           //Ensure it's a valid option
                                                           if (!this.IsApplicable(defaultButtonFocus))
                                                           {
                                                               defaultButtonFocus = this.ButtonStyle == MessageDialogStyle.Affirmative
                                                                   ? MessageDialogResult.Affirmative
                                                                   : MessageDialogResult.Negative;
                                                           }

                                                           //kind of acts like a selective 'IsDefault' mechanism.
                                                           switch (defaultButtonFocus)
                                                           {
                                                               case MessageDialogResult.Affirmative:
                                                                   this.PART_AffirmativeButton.SetResourceReference(StyleProperty, "AccentedDialogSquareButton");
                                                                   KeyboardNavigationEx.Focus(this.PART_AffirmativeButton);
                                                                   break;
                                                               case MessageDialogResult.Negative:
                                                                   this.PART_NegativeButton.SetResourceReference(StyleProperty, "AccentedDialogSquareButton");
                                                                   KeyboardNavigationEx.Focus(this.PART_NegativeButton);
                                                                   break;
                                                               case MessageDialogResult.FirstAuxiliary:
                                                                   this.PART_FirstAuxiliaryButton.SetResourceReference(StyleProperty, "AccentedDialogSquareButton");
                                                                   KeyboardNavigationEx.Focus(this.PART_FirstAuxiliaryButton);
                                                                   break;
                                                               case MessageDialogResult.SecondAuxiliary:
                                                                   this.PART_SecondAuxiliaryButton.SetResourceReference(StyleProperty, "AccentedDialogSquareButton");
                                                                   KeyboardNavigationEx.Focus(this.PART_SecondAuxiliaryButton);
                                                                   break;
                                                           }
                                                       }));

            TaskCompletionSource<MessageDialogResult> tcs = new TaskCompletionSource<MessageDialogResult>();

            RoutedEventHandler negativeHandler = null;
            KeyEventHandler negativeKeyHandler = null;

            RoutedEventHandler affirmativeHandler = null;
            KeyEventHandler affirmativeKeyHandler = null;

            RoutedEventHandler firstAuxHandler = null;
            KeyEventHandler firstAuxKeyHandler = null;

            RoutedEventHandler secondAuxHandler = null;
            KeyEventHandler secondAuxKeyHandler = null;

            KeyEventHandler escapeKeyHandler = null;

            Action cleanUpHandlers = null;

            var cancellationTokenRegistration = this.DialogSettings.CancellationToken.Register(() =>
                                                                                                   {
                                                                                                       cleanUpHandlers?.Invoke();
                                                                                                       tcs.TrySetResult(this.ButtonStyle == MessageDialogStyle.Affirmative ? MessageDialogResult.Affirmative : MessageDialogResult.Negative);
                                                                                                   });

            cleanUpHandlers = () =>
                {
                    this.PART_NegativeButton.Click -= negativeHandler;
                    this.PART_AffirmativeButton.Click -= affirmativeHandler;
                    this.PART_FirstAuxiliaryButton.Click -= firstAuxHandler;
                    this.PART_SecondAuxiliaryButton.Click -= secondAuxHandler;

                    this.PART_NegativeButton.KeyDown -= negativeKeyHandler;
                    this.PART_AffirmativeButton.KeyDown -= affirmativeKeyHandler;
                    this.PART_FirstAuxiliaryButton.KeyDown -= firstAuxKeyHandler;
                    this.PART_SecondAuxiliaryButton.KeyDown -= secondAuxKeyHandler;

                    this.KeyDown -= escapeKeyHandler;

                    cancellationTokenRegistration.Dispose();
                };

            negativeKeyHandler = (sender, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        cleanUpHandlers();

                        tcs.TrySetResult(MessageDialogResult.Negative);
                    }
                };

            affirmativeKeyHandler = (sender, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        cleanUpHandlers();

                        tcs.TrySetResult(MessageDialogResult.Affirmative);
                    }
                };

            firstAuxKeyHandler = (sender, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        cleanUpHandlers();

                        tcs.TrySetResult(MessageDialogResult.FirstAuxiliary);
                    }
                };

            secondAuxKeyHandler = (sender, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        cleanUpHandlers();

                        tcs.TrySetResult(MessageDialogResult.SecondAuxiliary);
                    }
                };

            negativeHandler = (sender, e) =>
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.Negative);

                    e.Handled = true;
                };

            affirmativeHandler = (sender, e) =>
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.Affirmative);

                    e.Handled = true;
                };

            firstAuxHandler = (sender, e) =>
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.FirstAuxiliary);

                    e.Handled = true;
                };

            secondAuxHandler = (sender, e) =>
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.SecondAuxiliary);

                    e.Handled = true;
                };

            escapeKeyHandler = (sender, e) =>
                {
                    if (e.Key == Key.Escape)
                    {
                        cleanUpHandlers();

                        tcs.TrySetResult(this.ButtonStyle == MessageDialogStyle.Affirmative ? MessageDialogResult.Affirmative : MessageDialogResult.Negative);
                    }
                    else if (e.Key == Key.Enter)
                    {
                        cleanUpHandlers();

                        tcs.TrySetResult(MessageDialogResult.Affirmative);
                    }
                };

            this.PART_NegativeButton.KeyDown += negativeKeyHandler;
            this.PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;
            this.PART_FirstAuxiliaryButton.KeyDown += firstAuxKeyHandler;
            this.PART_SecondAuxiliaryButton.KeyDown += secondAuxKeyHandler;

            this.PART_NegativeButton.Click += negativeHandler;
            this.PART_AffirmativeButton.Click += affirmativeHandler;
            this.PART_FirstAuxiliaryButton.Click += firstAuxHandler;
            this.PART_SecondAuxiliaryButton.Click += secondAuxHandler;

            this.KeyDown += escapeKeyHandler;

            return tcs.Task;
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(MessageDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty AffirmativeButtonTextProperty = DependencyProperty.Register("AffirmativeButtonText", typeof(string), typeof(MessageDialog), new PropertyMetadata("OK"));
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register("NegativeButtonText", typeof(string), typeof(MessageDialog), new PropertyMetadata("Cancel"));
        public static readonly DependencyProperty FirstAuxiliaryButtonTextProperty = DependencyProperty.Register("FirstAuxiliaryButtonText", typeof(string), typeof(MessageDialog), new PropertyMetadata("Cancel"));
        public static readonly DependencyProperty SecondAuxiliaryButtonTextProperty = DependencyProperty.Register("SecondAuxiliaryButtonText", typeof(string), typeof(MessageDialog), new PropertyMetadata("Cancel"));
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register("ButtonStyle", typeof(MessageDialogStyle), typeof(MessageDialog), new PropertyMetadata(MessageDialogStyle.Affirmative, new PropertyChangedCallback((s, e) =>
                                                                                                                                                                                                                                                           {
                                                                                                                                                                                                                                                               MessageDialog md = (MessageDialog)s;

                                                                                                                                                                                                                                                               SetButtonState(md);
                                                                                                                                                                                                                                                           })));

        private static void SetButtonState(MessageDialog md)
        {
            if (md.PART_AffirmativeButton == null)
                return;

            switch (md.ButtonStyle)
            {
                case MessageDialogStyle.Affirmative:
                {
                    md.PART_AffirmativeButton.Visibility = Visibility.Visible;
                    md.PART_NegativeButton.Visibility = Visibility.Collapsed;
                    md.PART_FirstAuxiliaryButton.Visibility = Visibility.Collapsed;
                    md.PART_SecondAuxiliaryButton.Visibility = Visibility.Collapsed;
                }
                    break;
                case MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary:
                case MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary:
                case MessageDialogStyle.AffirmativeAndNegative:
                {
                    md.PART_AffirmativeButton.Visibility = Visibility.Visible;
                    md.PART_NegativeButton.Visibility = Visibility.Visible;

                    if (md.ButtonStyle == MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary || md.ButtonStyle == MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary)
                    {
                        md.PART_FirstAuxiliaryButton.Visibility = Visibility.Visible;
                    }

                    if (md.ButtonStyle == MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary)
                    {
                        md.PART_SecondAuxiliaryButton.Visibility = Visibility.Visible;
                    }
                }
                    break;
            }

            md.AffirmativeButtonText = md.DialogSettings.AffirmativeButtonText;
            md.NegativeButtonText = md.DialogSettings.NegativeButtonText;
            md.FirstAuxiliaryButtonText = md.DialogSettings.FirstAuxiliaryButtonText;
            md.SecondAuxiliaryButtonText = md.DialogSettings.SecondAuxiliaryButtonText;

            switch (md.DialogSettings.ColorScheme)
            {
                case MetroDialogColorScheme.Accented:
                    md.PART_AffirmativeButton.Style = md.FindResource("AccentedDialogHighlightedSquareButton") as Style;
                    md.PART_NegativeButton.Style = md.FindResource("AccentedDialogHighlightedSquareButton") as Style;
                    md.PART_FirstAuxiliaryButton.Style = md.FindResource("AccentedDialogHighlightedSquareButton") as Style;
                    md.PART_SecondAuxiliaryButton.Style = md.FindResource("AccentedDialogHighlightedSquareButton") as Style;
                    break;
            }
        }

        protected override void OnLoaded()
        {
            SetButtonState(this);
        }

        public MessageDialogStyle ButtonStyle
        {
            get { return (MessageDialogStyle)this.GetValue(ButtonStyleProperty); }
            set { this.SetValue(ButtonStyleProperty, value); }
        }

        public string Message
        {
            get { return (string)this.GetValue(MessageProperty); }
            set { this.SetValue(MessageProperty, value); }
        }

        public string AffirmativeButtonText
        {
            get { return (string)this.GetValue(AffirmativeButtonTextProperty); }
            set { this.SetValue(AffirmativeButtonTextProperty, value); }
        }

        public string NegativeButtonText
        {
            get { return (string)this.GetValue(NegativeButtonTextProperty); }
            set { this.SetValue(NegativeButtonTextProperty, value); }
        }

        public string FirstAuxiliaryButtonText
        {
            get { return (string)this.GetValue(FirstAuxiliaryButtonTextProperty); }
            set { this.SetValue(FirstAuxiliaryButtonTextProperty, value); }
        }

        public string SecondAuxiliaryButtonText
        {
            get { return (string)this.GetValue(SecondAuxiliaryButtonTextProperty); }
            set { this.SetValue(SecondAuxiliaryButtonTextProperty, value); }
        }

        private void OnKeyCopyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Clipboard.SetDataObject(this.Message);
        }

        private bool IsApplicable(MessageDialogResult value)
        {
            switch (value)
            {
                case MessageDialogResult.Affirmative:
                    return this.PART_AffirmativeButton.IsVisible;
                case MessageDialogResult.Negative:
                    return this.PART_NegativeButton.IsVisible;
                case MessageDialogResult.FirstAuxiliary:
                    return this.PART_FirstAuxiliaryButton.IsVisible;
                case MessageDialogResult.SecondAuxiliary:
                    return this.PART_SecondAuxiliaryButton.IsVisible;
            }

            return false;
        }
    }

    /// <summary>
    /// An enum representing the result of a Message Dialog.
    /// </summary>
    public enum MessageDialogResult
    {
        Negative = 0,
        Affirmative = 1,
        FirstAuxiliary,
        SecondAuxiliary,
    }

    /// <summary>
    /// An enum representing the different button states for a Message Dialog.
    /// </summary>
    public enum MessageDialogStyle
    {
        /// <summary>
        /// Just "OK"
        /// </summary>
        Affirmative = 0,
        /// <summary>
        /// "OK" and "Cancel"
        /// </summary>
        AffirmativeAndNegative = 1,
        AffirmativeAndNegativeAndSingleAuxiliary = 2,
        AffirmativeAndNegativeAndDoubleAuxiliary = 3
    }
}