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
        private const string ACCENT_BUTTON_STYLE = "MahApps.Styles.Button.Dialogs.Accent";
        private const string ACCENT_HIGHLIGHT_BUTTON_STYLE = "MahApps.Styles.Button.Dialogs.AccentHighlight";
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
                                          new PropertyMetadata(MessageDialogStyle.Affirmative, ButtonStylePropertyChangedCallback));

        private static void ButtonStylePropertyChangedCallback(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is MessageDialog dialog)
            {
                SetButtonState(dialog);
            }
        }

        public MessageDialogStyle ButtonStyle
        {
            get => (MessageDialogStyle)this.GetValue(ButtonStyleProperty);
            set => this.SetValue(ButtonStyleProperty, value);
        }

        /// <summary>Identifies the <see cref="Icon"/> dependency property.</summary>
        public static readonly DependencyProperty IconProperty
            = DependencyProperty.Register(nameof(Icon),
                                          typeof(object),
                                          typeof(MessageDialog),
                                          new PropertyMetadata());

        public object? Icon
        {
            get => this.GetValue(IconProperty);
            set => this.SetValue(IconProperty, value);
        }

        /// <summary>Identifies the <see cref="IconTemplate"/> dependency property.</summary>
        public static readonly DependencyProperty IconTemplateProperty
            = DependencyProperty.Register(nameof(IconTemplate),
                                          typeof(DataTemplate),
                                          typeof(MessageDialog));

        public DataTemplate? IconTemplate
        {
            get => (DataTemplate?)this.GetValue(IconTemplateProperty);
            set => this.SetValue(IconTemplateProperty, value);
        }

        #endregion DependencyProperties

        #region Constructor

        internal MessageDialog() : this(null)
        { }

        internal MessageDialog(MetroWindow? parentWindow) : this(parentWindow, null)
        { }

        internal MessageDialog(MetroWindow? parentWindow, MetroDialogSettings? settings) : base(parentWindow, settings)
        {
            this.SetCurrentValue(FirstAuxiliaryButtonTextProperty, "Cancel");
            this.SetCurrentValue(SecondAuxiliaryButtonTextProperty, "Cancel");

            if (this.PART_MessageScrollViewer is not null)
            {
                this.PART_MessageScrollViewer.Height = this.DialogSettings.MaximumBodyHeight;
            }
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
                        if (this.PART_AffirmativeButton is not null)
                        {
                            this.PART_AffirmativeButton.SetResourceReference(StyleProperty, ACCENT_BUTTON_STYLE);
                            KeyboardNavigationEx.Focus(this.PART_AffirmativeButton);
                        }
                        break;

                    case MessageDialogResult.Negative:
                        if (this.PART_NegativeButton is not null)
                        {
                            this.PART_NegativeButton.SetResourceReference(StyleProperty, ACCENT_BUTTON_STYLE);
                            KeyboardNavigationEx.Focus(this.PART_NegativeButton);
                        }
                        break;

                    case MessageDialogResult.FirstAuxiliary:
                        if (this.PART_FirstAuxiliaryButton is not null)
                        {
                            this.PART_FirstAuxiliaryButton.SetResourceReference(StyleProperty, ACCENT_BUTTON_STYLE);
                            KeyboardNavigationEx.Focus(this.PART_FirstAuxiliaryButton);
                        }
                        break;

                    case MessageDialogResult.SecondAuxiliary:
                        if (this.PART_SecondAuxiliaryButton is not null)
                        {
                            this.PART_SecondAuxiliaryButton.SetResourceReference(StyleProperty, ACCENT_BUTTON_STYLE);
                            KeyboardNavigationEx.Focus(this.PART_SecondAuxiliaryButton);
                        }
                        break;
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

            negativeKeyHandler = (_, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    CleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.Negative);
                }
            };

            affirmativeKeyHandler = (_, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    CleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.Affirmative);
                }
            };

            firstAuxKeyHandler = (_, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    CleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.FirstAuxiliary);
                }
            };

            secondAuxKeyHandler = (_, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    CleanUpHandlers();

                    tcs.TrySetResult(MessageDialogResult.SecondAuxiliary);
                }
            };

            negativeHandler = (_, e) =>
            {
                CleanUpHandlers();

                tcs.TrySetResult(MessageDialogResult.Negative);

                e.Handled = true;
            };

            affirmativeHandler = (_, e) =>
            {
                CleanUpHandlers();

                tcs.TrySetResult(MessageDialogResult.Affirmative);

                e.Handled = true;
            };

            firstAuxHandler = (_, e) =>
            {
                CleanUpHandlers();

                tcs.TrySetResult(MessageDialogResult.FirstAuxiliary);

                e.Handled = true;
            };

            secondAuxHandler = (_, e) =>
            {
                CleanUpHandlers();

                tcs.TrySetResult(MessageDialogResult.SecondAuxiliary);

                e.Handled = true;
            };

            escapeKeyHandler = (_, e) =>
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
                this.PART_AffirmativeButton.KeyDown += affirmativeKeyHandler;
                this.PART_AffirmativeButton.Click += affirmativeHandler;
            }

            if (this.PART_NegativeButton is not null)
            {
                this.PART_NegativeButton.KeyDown += negativeKeyHandler;
                this.PART_NegativeButton.Click += negativeHandler;
            }

            if (this.PART_FirstAuxiliaryButton is not null)
            {
                this.PART_FirstAuxiliaryButton.KeyDown += firstAuxKeyHandler;
                this.PART_FirstAuxiliaryButton.Click += firstAuxHandler;
            }

            if (this.PART_SecondAuxiliaryButton is not null)
            {
                this.PART_SecondAuxiliaryButton.KeyDown += secondAuxKeyHandler;
                this.PART_SecondAuxiliaryButton.Click += secondAuxHandler;
            }

            this.KeyDown += escapeKeyHandler;

            return tcs.Task;
        }

        private static void SetButtonState(MessageDialog md)
        {
            if (md.PART_AffirmativeButton is null)
            {
                return;
            }

            if (md.PART_AffirmativeButton is null
                || md.PART_NegativeButton is null
                || md.PART_FirstAuxiliaryButton is null
                || md.PART_SecondAuxiliaryButton is null)
            {
                return;
            }

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

            if (md.DialogSettings.ColorScheme == MetroDialogColorScheme.Accented)
            {
                md.PART_AffirmativeButton.SetResourceReference(StyleProperty, ACCENT_HIGHLIGHT_BUTTON_STYLE);
                md.PART_NegativeButton.SetResourceReference(StyleProperty, ACCENT_HIGHLIGHT_BUTTON_STYLE);
                md.PART_FirstAuxiliaryButton.SetResourceReference(StyleProperty, ACCENT_HIGHLIGHT_BUTTON_STYLE);
                md.PART_SecondAuxiliaryButton.SetResourceReference(StyleProperty, ACCENT_HIGHLIGHT_BUTTON_STYLE);
            }
        }

        protected override void OnLoaded()
        {
            this.Icon = this.DialogSettings.Icon;
            this.IconTemplate = this.DialogSettings.IconTemplate;

            SetButtonState(this);
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
            if (this.PART_AffirmativeButton is null
                || this.PART_NegativeButton is null
                || this.PART_FirstAuxiliaryButton is null
                || this.PART_SecondAuxiliaryButton is null)
            {
                return false;
            }

            return value switch
            {
                MessageDialogResult.Affirmative => this.PART_AffirmativeButton.IsVisible,
                MessageDialogResult.Negative => this.PART_NegativeButton.IsVisible,
                MessageDialogResult.FirstAuxiliary => this.PART_FirstAuxiliaryButton.IsVisible,
                MessageDialogResult.SecondAuxiliary => this.PART_SecondAuxiliaryButton.IsVisible,
                _ => false
            };
        }
    }
}