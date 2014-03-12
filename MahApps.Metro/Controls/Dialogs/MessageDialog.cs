using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MahApps.Metro.Controls.Dialogs
{
    /// <summary>
    /// An internal control that represents a message dialog. Please use MetroWindow.ShowMessage instead!
    /// </summary>
    public partial class MessageDialog : BaseMetroDialog
    {
        //private const string PART_AffirmativeButton = "PART_AffirmativeButton";
        //private const string PART_NegativeButton = "PART_NegativeButton";

        //private Button AffirmativeButton = null;
        //private Button NegativeButton = null;

        //static MessageDialog()
        //{
        //    //DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageDialog), new FrameworkPropertyMetadata(typeof(MessageDialog)));
        //}

        internal MessageDialog(MetroWindow parentWindow)
            : this(parentWindow, null)
        {
        }
        internal MessageDialog(MetroWindow parentWindow, MetroDialogSettings settings)
            : base(parentWindow, settings)
        {
            InitializeComponent();
        }

        internal Task<MessageDialogResult> WaitForButtonPressAsync()
        {
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.Focus();

                    //kind of acts like a selective 'IsDefault' mechanism.
                    if (ButtonStyle == MessageDialogStyle.Affirmative)
                        PART_AffirmativeButton.Focus();
                    else if (ButtonStyle == MessageDialogStyle.AffirmativeAndNegative)
                        PART_NegativeButton.Focus();
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

            Action cleanUpHandlers = () =>
            {
                PART_NegativeButton.Click -= negativeHandler;
                PART_AffirmativeButton.Click -= affirmativeHandler;
                PART_FirstAuxiliaryButton.Click -= firstAuxHandler;
                PART_SecondAuxiliaryButton.Click -= secondAuxHandler;

                PART_NegativeButton.KeyDown -= negativeKeyHandler;
                PART_AffirmativeButton.KeyDown -= affirmativeKeyHandler;
                PART_FirstAuxiliaryButton.KeyDown -= firstAuxKeyHandler;
                PART_SecondAuxiliaryButton.KeyDown -= secondAuxKeyHandler;
            };


            negativeKeyHandler = new KeyEventHandler((sender, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        cleanUpHandlers();

                        tcs.TrySetResult(MessageDialogResult.Negative);
                    }
                });

            affirmativeKeyHandler = new KeyEventHandler((sender, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        cleanUpHandlers();

                        tcs.TrySetResult(MessageDialogResult.Affirmative);
                    }
                });
            firstAuxKeyHandler = new KeyEventHandler((sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.FirstAuxiliary);
                }
            });
            secondAuxKeyHandler = new KeyEventHandler((sender, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.SecondAuxiliary);
                }
            });


            negativeHandler = new RoutedEventHandler((sender, e) =>
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.Negative);

                    e.Handled = true;
                });

            affirmativeHandler = new RoutedEventHandler((sender, e) =>
                {
                    cleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.Affirmative);

                    e.Handled = true;
                });

            firstAuxHandler = new RoutedEventHandler((sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(MessageDialogResult.FirstAuxiliary);

                e.Handled = true;
            });

            secondAuxHandler = new RoutedEventHandler((sender, e) =>
            {
                cleanUpHandlers();

                tcs.TrySetResult(MessageDialogResult.SecondAuxiliary);

                e.Handled = true;
            });

            PART_NegativeButton.KeyDown += negativeKeyHandler;
            PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;
            PART_FirstAuxiliaryButton.KeyDown += firstAuxKeyHandler;
            PART_SecondAuxiliaryButton.KeyDown += secondAuxKeyHandler;

            PART_NegativeButton.Click += negativeHandler;
            PART_AffirmativeButton.Click += affirmativeHandler;
            PART_FirstAuxiliaryButton.Click += firstAuxHandler;
            PART_SecondAuxiliaryButton.Click += secondAuxHandler;

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
            if (md.PART_AffirmativeButton == null) return;

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
                    md.PART_NegativeButton.Style = md.FindResource("HighlightedSquareButtonStyle") as Style;
                    md.PART_FirstAuxiliaryButton.Style = md.FindResource("HighlightedSquareButtonStyle") as Style;
                    md.PART_SecondAuxiliaryButton.Style = md.FindResource("HighlightedSquareButtonStyle") as Style;
                    break;
                default:
                    break;
            }
        }

        private void Dialog_Loaded(object sender, RoutedEventArgs e)
        {
            SetButtonState(this);
        }

        public MessageDialogStyle ButtonStyle
        {
            get { return (MessageDialogStyle)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public string AffirmativeButtonText
        {
            get { return (string)GetValue(AffirmativeButtonTextProperty); }
            set { SetValue(AffirmativeButtonTextProperty, value); }
        }
        public string NegativeButtonText
        {
            get { return (string)GetValue(NegativeButtonTextProperty); }
            set { SetValue(NegativeButtonTextProperty, value); }
        }
        public string FirstAuxiliaryButtonText
        {
            get { return (string)GetValue(FirstAuxiliaryButtonTextProperty); }
            set { SetValue(FirstAuxiliaryButtonTextProperty, value); }
        }
        public string SecondAuxiliaryButtonText
        {
            get { return (string)GetValue(SecondAuxiliaryButtonTextProperty); }
            set { SetValue(SecondAuxiliaryButtonTextProperty, value); }
        }


        public override void OnApplyTemplate()
        {
            //AffirmativeButton = GetTemplateChild(PART_AffirmativeButton) as Button;
            //NegativeButton = GetTemplateChild(PART_NegativeButton) as Button;

            base.OnApplyTemplate();
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
