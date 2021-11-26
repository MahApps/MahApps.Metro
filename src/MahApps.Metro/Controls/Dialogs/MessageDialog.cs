// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ControlzEx;

namespace MahApps.Metro.Controls.Dialogs
{
    /// <summary>
    /// An internal control that represents a message dialog. Please use MetroWindow.ShowMessage instead!
    /// </summary>
    [TemplatePart(Name = nameof(PART_AffirmativeButton), Type = typeof(Button))]
    [TemplatePart(Name = nameof(PART_NegativeButton), Type = typeof(Button))]
    [TemplatePart(Name = nameof(PART_FirstAuxiliaryButton), Type = typeof(Button))]
    [TemplatePart(Name = nameof(PART_SecondAuxiliaryButton), Type = typeof(Button))]
    [TemplatePart(Name = nameof(PART_MessageScrollViewer), Type = typeof(ScrollViewer))]
    public class MessageDialog : BaseMetroDialog
    {
        private CancellationTokenRegistration cancellationTokenRegistration;

        #region Controls

        private Button? PART_AffirmativeButton;
        private Button? PART_NegativeButton;
        private Button? PART_FirstAuxiliaryButton;
        private Button? PART_SecondAuxiliaryButton;
        private ScrollViewer? PART_MessageScrollViewer;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.PART_AffirmativeButton = this.GetTemplateChild(nameof(this.PART_AffirmativeButton)) as Button;
            this.PART_NegativeButton = this.GetTemplateChild(nameof(this.PART_NegativeButton)) as Button;
            this.PART_FirstAuxiliaryButton = this.GetTemplateChild(nameof(this.PART_FirstAuxiliaryButton)) as Button;
            this.PART_SecondAuxiliaryButton = this.GetTemplateChild(nameof(this.PART_SecondAuxiliaryButton)) as Button;
            this.PART_MessageScrollViewer = this.GetTemplateChild(nameof(this.PART_MessageScrollViewer)) as ScrollViewer;

            if (this.PART_MessageScrollViewer is not null)
            {
                this.PART_MessageScrollViewer.Height = this.DialogSettings.MaximumBodyHeight;
            }
        }

        #endregion Controls

        #region DependencyProperties

        /// <summary>Identifies the <see cref="Message"/> dependency property.</summary>
        public static readonly DependencyProperty MessageProperty
            = DependencyProperty.Register(nameof(Message),
                                          typeof(string),
                                          typeof(MessageDialog),
                                          new PropertyMetadata(default(string)));

        public string? Message
        {
            get => (string?)this.GetValue(MessageProperty);
            set => this.SetValue(MessageProperty, value);
        }

        /// <summary>Identifies the <see cref="AffirmativeButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty AffirmativeButtonTextProperty
            = DependencyProperty.Register(nameof(AffirmativeButtonText),
                                          typeof(string),
                                          typeof(MessageDialog),
                                          new PropertyMetadata("OK"));

        public string AffirmativeButtonText
        {
            get => (string)this.GetValue(AffirmativeButtonTextProperty);
            set => this.SetValue(AffirmativeButtonTextProperty, value);
        }

        /// <summary>Identifies the <see cref="NegativeButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty NegativeButtonTextProperty
            = DependencyProperty.Register(nameof(NegativeButtonText),
                                          typeof(string),
                                          typeof(MessageDialog),
                                          new PropertyMetadata("Cancel"));

        public string NegativeButtonText
        {
            get => (string)this.GetValue(NegativeButtonTextProperty);
            set => this.SetValue(NegativeButtonTextProperty, value);
        }

        /// <summary>Identifies the <see cref="FirstAuxiliaryButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty FirstAuxiliaryButtonTextProperty
            = DependencyProperty.Register(nameof(FirstAuxiliaryButtonText),
                                          typeof(string),
                                          typeof(MessageDialog),
                                          new PropertyMetadata(default(string)));

        public string? FirstAuxiliaryButtonText
        {
            get => (string?)this.GetValue(FirstAuxiliaryButtonTextProperty);
            set => this.SetValue(FirstAuxiliaryButtonTextProperty, value);
        }

        /// <summary>Identifies the <see cref="SecondAuxiliaryButtonText"/> dependency property.</summary>
        public static readonly DependencyProperty SecondAuxiliaryButtonTextProperty
            = DependencyProperty.Register(nameof(SecondAuxiliaryButtonText),
                                          typeof(string),
                                          typeof(MessageDialog),
                                          new PropertyMetadata(default(string)));

        public string? SecondAuxiliaryButtonText
        {
            get => (string?)this.GetValue(SecondAuxiliaryButtonTextProperty);
            set => this.SetValue(SecondAuxiliaryButtonTextProperty, value);
        }

        /// <summary>Identifies the <see cref="ButtonStyle"/> dependency property.</summary>
        public static readonly DependencyProperty ButtonStyleProperty
            = DependencyProperty.Register(nameof(ButtonStyle),
                                          typeof(MessageDialogStyle),
                                          typeof(MessageDialog),
                                          new PropertyMetadata(MessageDialogStyle.Affirmative, OnButtonStylePropertyChangedCallback));

        private static void OnButtonStylePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((MessageDialog)d).OnButtonStyleChanged((MessageDialogStyle)e.OldValue, (MessageDialogStyle)e.NewValue);
        }

        public MessageDialogStyle ButtonStyle
        {
            get => (MessageDialogStyle)this.GetValue(ButtonStyleProperty);
            set => this.SetValue(ButtonStyleProperty, value);
        }

        public static readonly DependencyProperty DefaultButtonFocusProperty
            = DependencyProperty.Register(nameof(DefaultButtonFocus),
                                          typeof(MessageDialogResult),
                                          typeof(MessageDialog),
                                          new PropertyMetadata(MessageDialogResult.Negative));

        public MessageDialogResult DefaultButtonFocus
        {
            get => (MessageDialogResult)this.GetValue(DefaultButtonFocusProperty);
            set => this.SetValue(DefaultButtonFocusProperty, value);
        }

        #endregion DependencyProperties

        #region Constructor

        internal MessageDialog()
            : this(null)
        {
        }

        internal MessageDialog(MetroWindow? parentWindow)
            : this(parentWindow, null)
        {
        }

        internal MessageDialog(MetroWindow? parentWindow, MetroDialogSettings? settings)
            : base(parentWindow, settings)
        {
            this.SetCurrentValue(AffirmativeButtonTextProperty, this.DialogSettings.AffirmativeButtonText);
            this.SetCurrentValue(NegativeButtonTextProperty, this.DialogSettings.NegativeButtonText);
            this.SetCurrentValue(FirstAuxiliaryButtonTextProperty, this.DialogSettings.FirstAuxiliaryButtonText);
            this.SetCurrentValue(SecondAuxiliaryButtonTextProperty, this.DialogSettings.SecondAuxiliaryButtonText);

            var defaultButtonFocus = this.DialogSettings.DefaultButtonFocus;

            //Ensure it's a valid option
            if (!this.IsApplicable(defaultButtonFocus))
            {
                defaultButtonFocus = this.ButtonStyle == MessageDialogStyle.Affirmative
                    ? MessageDialogResult.Affirmative
                    : MessageDialogResult.Negative;
            }

            this.SetCurrentValue(DefaultButtonFocusProperty, defaultButtonFocus);

            this.ApplyDefaultButtonFocus();
        }

        static MessageDialog()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MessageDialog), new FrameworkPropertyMetadata(typeof(MessageDialog)));
        }

        #endregion Constructor

        #region Event Handler

        private RoutedEventHandler? negativeHandler = null;
        private KeyEventHandler? negativeKeyHandler = null;
        private RoutedEventHandler? affirmativeHandler = null;
        private KeyEventHandler? affirmativeKeyHandler = null;
        private RoutedEventHandler? firstAuxHandler = null;
        private KeyEventHandler? firstAuxKeyHandler = null;
        private RoutedEventHandler? secondAuxHandler = null;
        private KeyEventHandler? secondAuxKeyHandler = null;
        private KeyEventHandler? escapeKeyHandler = null;

        #endregion Event Handler

        private void OnButtonStyleChanged(MessageDialogStyle oldStyle, MessageDialogStyle newStyle)
        {
            this.ApplyDefaultButtonFocus();
        }

        private void ApplyDefaultButtonFocus()
        {
            var defaultButtonFocus = this.DialogSettings.DefaultButtonFocus;

            //Ensure it's a valid option
            if (!this.IsApplicable(defaultButtonFocus))
            {
                defaultButtonFocus = this.ButtonStyle == MessageDialogStyle.Affirmative
                    ? MessageDialogResult.Affirmative
                    : MessageDialogResult.Negative;
            }

            this.SetCurrentValue(DefaultButtonFocusProperty, defaultButtonFocus);
        }

        internal Task<MessageDialogResult> WaitForButtonPressAsync()
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.Focus();

                    var defaultButtonFocus = this.DefaultButtonFocus;

                    switch (defaultButtonFocus)
                    {
                        //kind of acts like a selective 'IsDefault' mechanism.
                        case MessageDialogResult.Affirmative:
                            KeyboardNavigationEx.Focus(this.PART_AffirmativeButton);
                            break;
                        case MessageDialogResult.Negative:
                            KeyboardNavigationEx.Focus(this.PART_NegativeButton);
                            break;
                        case MessageDialogResult.FirstAuxiliary:
                            KeyboardNavigationEx.Focus(this.PART_FirstAuxiliaryButton);
                            break;
                        case MessageDialogResult.SecondAuxiliary:
                            KeyboardNavigationEx.Focus(this.PART_SecondAuxiliaryButton);
                            break;
                        case MessageDialogResult.Canceled:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }));

            var tcs = new TaskCompletionSource<MessageDialogResult>();

            void CleanUpHandlers()
            {
                if (this.PART_NegativeButton is not null)
                {
                    this.PART_NegativeButton.Click -= this.negativeHandler;
                    this.PART_NegativeButton.KeyDown -= this.negativeKeyHandler;
                }

                if (this.PART_AffirmativeButton is not null)
                {
                    this.PART_AffirmativeButton.Click -= this.affirmativeHandler;
                    this.PART_AffirmativeButton.KeyDown -= this.affirmativeKeyHandler;
                }

                if (this.PART_FirstAuxiliaryButton is not null)
                {
                    this.PART_FirstAuxiliaryButton.Click -= this.firstAuxHandler;
                    this.PART_FirstAuxiliaryButton.KeyDown -= this.firstAuxKeyHandler;
                }

                if (this.PART_SecondAuxiliaryButton is not null)
                {
                    this.PART_SecondAuxiliaryButton.Click -= this.secondAuxHandler;
                    this.PART_SecondAuxiliaryButton.KeyDown -= this.secondAuxKeyHandler;
                }

                this.KeyDown -= this.escapeKeyHandler;

                this.cancellationTokenRegistration.Dispose();
            }

            this.cancellationTokenRegistration = this.DialogSettings
                                                     .CancellationToken
                                                     .Register(() =>
                                                         {
                                                             this.BeginInvoke(() =>
                                                                 {
                                                                     CleanUpHandlers();
                                                                     tcs.TrySetResult(this.ButtonStyle == MessageDialogStyle.Affirmative ? MessageDialogResult.Affirmative : MessageDialogResult.Negative);
                                                                 });
                                                         });

            this.negativeKeyHandler = (_, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        CleanUpHandlers();

                        tcs.TrySetResult(MessageDialogResult.Negative);
                    }
                };

            this.affirmativeKeyHandler = (_, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        CleanUpHandlers();

                        tcs.TrySetResult(MessageDialogResult.Affirmative);
                    }
                };

            this.firstAuxKeyHandler = (_, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        CleanUpHandlers();

                        tcs.TrySetResult(MessageDialogResult.FirstAuxiliary);
                    }
                };

            this.secondAuxKeyHandler = (_, e) =>
                {
                    if (e.Key == Key.Enter)
                    {
                        CleanUpHandlers();

                        tcs.TrySetResult(MessageDialogResult.SecondAuxiliary);
                    }
                };

            this.negativeHandler = (_, e) =>
                {
                    CleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.Negative);

                    e.Handled = true;
                };

            this.affirmativeHandler = (_, e) =>
                {
                    CleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.Affirmative);

                    e.Handled = true;
                };

            this.firstAuxHandler = (_, e) =>
                {
                    CleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.FirstAuxiliary);

                    e.Handled = true;
                };

            this.secondAuxHandler = (_, e) =>
                {
                    CleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.SecondAuxiliary);

                    e.Handled = true;
                };

            this.escapeKeyHandler = (_, e) =>
                {
                    if (e.Key == Key.Escape || (e.Key == Key.System && e.SystemKey == Key.F4))
                    {
                        CleanUpHandlers();

                        tcs.TrySetResult(this.DialogSettings.DialogResultOnCancel ?? (this.ButtonStyle == MessageDialogStyle.Affirmative ? MessageDialogResult.Affirmative : MessageDialogResult.Negative));
                    }
                    else if (e.Key == Key.Enter)
                    {
                        CleanUpHandlers();

                        tcs.TrySetResult(MessageDialogResult.Affirmative);
                    }
                };

            if (this.PART_AffirmativeButton is not null)
            {
                this.PART_AffirmativeButton.KeyDown += this.affirmativeKeyHandler;
                this.PART_AffirmativeButton.Click += this.affirmativeHandler;
            }

            if (this.PART_NegativeButton is not null)
            {
                this.PART_NegativeButton.KeyDown += this.negativeKeyHandler;
                this.PART_NegativeButton.Click += this.negativeHandler;
            }

            if (this.PART_FirstAuxiliaryButton is not null)
            {
                this.PART_FirstAuxiliaryButton.KeyDown += this.firstAuxKeyHandler;
                this.PART_FirstAuxiliaryButton.Click += this.firstAuxHandler;
            }

            if (this.PART_SecondAuxiliaryButton is not null)
            {
                this.PART_SecondAuxiliaryButton.KeyDown += this.secondAuxKeyHandler;
                this.PART_SecondAuxiliaryButton.Click += this.secondAuxHandler;
            }

            this.KeyDown += this.escapeKeyHandler;

            return tcs.Task;
        }

        private void OnKeyCopyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var message = this.Message;
            if (message != null)
            {
                Clipboard.SetDataObject(message);
            }
        }

        private bool IsApplicable(MessageDialogResult value)
        {
            return value switch
            {
                MessageDialogResult.Affirmative => this.ButtonStyle == MessageDialogStyle.Affirmative,
                MessageDialogResult.Negative => this.ButtonStyle != MessageDialogStyle.Affirmative,
                MessageDialogResult.FirstAuxiliary => this.ButtonStyle is MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary or MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary,
                MessageDialogResult.SecondAuxiliary => this.ButtonStyle == MessageDialogStyle.AffirmativeAndNegativeAndDoubleAuxiliary,
                _ => false
            };
        }
    }
}