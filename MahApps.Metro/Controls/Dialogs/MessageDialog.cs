using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            : base(parentWindow)
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

            Action cleanUpHandlers = () =>
            {
                PART_NegativeButton.Click -= negativeHandler;
                PART_AffirmativeButton.Click -= affirmativeHandler;

                PART_NegativeButton.KeyDown -= negativeKeyHandler;
                PART_AffirmativeButton.KeyDown -= affirmativeKeyHandler;
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

            PART_NegativeButton.KeyDown += negativeKeyHandler;
            PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;

            PART_NegativeButton.Click += negativeHandler;
            PART_AffirmativeButton.Click += affirmativeHandler;

            return tcs.Task;
        }

        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(MessageDialog), new PropertyMetadata(default(string)));
        public static readonly DependencyProperty AffirmativeButtonTextProperty = DependencyProperty.Register("AffirmativeButtonText", typeof(string), typeof(MessageDialog), new PropertyMetadata("OK"));
        public static readonly DependencyProperty NegativeButtonTextProperty = DependencyProperty.Register("NegativeButtonText", typeof(string), typeof(MessageDialog), new PropertyMetadata("Cancel"));
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register("ButtonStyle", typeof(MessageDialogStyle), typeof(MessageDialog), new PropertyMetadata(MessageDialogStyle.Affirmative, new PropertyChangedCallback((s,e) =>
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
                    }
                    break;
                case MessageDialogStyle.AffirmativeAndNegative:
                    {
                        md.PART_AffirmativeButton.Visibility = Visibility.Visible;
                        md.PART_NegativeButton.Visibility = Visibility.Visible;
                    }
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
    }
}
